using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using BPUtil;
using Microsoft.VisualBasic.Devices;
using Microsoft.Win32;
using Newtonsoft.Json;

namespace BiUpdateHelper
{
	public class Upload_Record
	{
		public string Secret;
		public string HelperVersion;
		public string OS;
		public string BiVersion;
		public string CpuModel;
		public int CpuMHz;
		public byte CpuUsage;
		public byte BiCpuUsage;
		public int MemMB;
		public int BiMemUsageMB;
		public int BiPeakVirtualMemUsageMB;
		public int MemFreeMB;
		public byte HwAccel;
		public float RamGiB;
		public ushort RamChannels;
		public ushort RamMHz;
		public Upload_Camera[] cameras;
		public Upload_Gpu[] gpus;
	}
	public class Upload_Camera
	{
		public int Pixels;
		public byte FPS;
		public bool LimitDecode;
		public byte Hwaccel;
		public byte Type;
		public byte CapType;
		public bool MotionDetector;
		public byte RecordTriggerType;
		public byte RecordFormat;
		public bool DirectToDisk;
		public string VCodec;
	}
	public class Upload_Gpu
	{
		public string Name;
		public string Version;
	}
	public static class UsageInfoCollector
	{
		private static DateTime startedUsageReportAt = DateTime.MinValue;
		/// <summary>
		/// If it is time to submit an anonymous Blue Iris usage report, do it in a background thread.
		/// </summary>
		public static void HandlePossibleUsageReport()
		{
			DateTime lastReportAt = TimeUtil.DateTimeFromEpochMS(Program.settings.lastUsageReportAt);
			DateTime now = DateTime.UtcNow;
			if (now < lastReportAt)
				lastReportAt = DateTime.MinValue;
			if ((now - lastReportAt).TotalDays < 7)
				return; // It hasn't been 7 days since the last report.  Don't generate a new report.
			if ((now - startedUsageReportAt).TotalMinutes < 10)
				return; // It hasn't been 10 minutes since the last attempt.  Don't generate a new report.

			startedUsageReportAt = now;
			Thread thrUsageReport = new Thread(UsageReportBackground);
			thrUsageReport.Name = "Usage Report";
			thrUsageReport.IsBackground = true;
			thrUsageReport.Start();
		}
		/// <summary>
		/// Submits a usage report synchronously.  Should be called from a background thread.
		/// </summary>
		private static void UsageReportBackground()
		{
			try
			{
				// Generate record.
				Upload_Record record = GetUsageRecord();
				if (record == null)
					return;

				// Submit record.
				//File.WriteAllText("UsageRecord.json", JsonConvert.SerializeObject(record, Formatting.Indented));
				string jsonPayload = JsonConvert.SerializeObject(record);
				string md5 = Hash.GetMD5Hex(jsonPayload);
				using (WebClient wc = new WebClient())
				{
					wc.UploadString("https://biupdatehelper.hopto.org/api/uploadUsageRecord1", jsonPayload + md5);
				}

				// Save the time so this doesn't happen again for a while.
				Program.settings.Load();
				Program.settings.lastUsageReportAt = TimeUtil.GetTimeInMsSinceEpoch();
				Program.settings.Save();
			}
			catch (ThreadAbortException) { }
			catch (Exception ex)
			{
				Logger.Debug(ex, "Unable to generate anonymous usage record.");
			}
		}
		/// <summary>
		/// Creates an anonymous usage record.  This method spends 10 seconds measuring CPU usage.  Returns null if any BlueIris.exe processes close while CPU usage is being measured, or if no BlueIris.exe processes were open.
		/// </summary>
		/// <returns></returns>
		public static Upload_Record GetUsageRecord()
		{
			Upload_Record record = new Upload_Record();

			// Begin measuring CPU usage.
			using (PerformanceCounter totalCpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total"))
			{
				totalCpuCounter.NextValue();
				Stopwatch sw = new Stopwatch();
				sw.Start();
				List<Process> biProcs = Process.GetProcesses().Where(p => p.ProcessName.ToLower() == "blueiris").ToList();
				if (biProcs.Count < 1)
				{
					Logger.Info("Unable to generate anonymous usage record because Blue Iris is not running.");
					return null;
				}
				TimeSpan[] startTimes = new TimeSpan[biProcs.Count];
				for (int i = 0; i < biProcs.Count; i++)
				{
					startTimes[i] = biProcs[i].TotalProcessorTime;
				}

				// Wait for CPU usage to happen.
				Thread.Sleep(10000);

				// Take CPU usage measurements.
				record.CpuUsage = (byte)Math.Round(totalCpuCounter.NextValue());
				sw.Stop();
				TimeSpan totalTime = TimeSpan.Zero;
				for (int i = 0; i < biProcs.Count; i++)
				{
					biProcs[i].Refresh();
					if (biProcs[i].HasExited)
					{
						Logger.Info("Unable to generate anonymous usage record because Blue Iris exited while CPU usage was being measured.");
						return null;
					}
					totalTime += biProcs[i].TotalProcessorTime - startTimes[i];
				}
				double fraction = totalTime.TotalMilliseconds / sw.Elapsed.TotalMilliseconds;
				record.BiCpuUsage = (byte)Math.Round((fraction / Environment.ProcessorCount) * 100);

				long physicalMemUsage = 0;
				long virtualMemUsage = 0;
				int n = 0;
				foreach (Process p in biProcs)
				{
					if (record.BiVersion == null)
						record.BiVersion = p.MainModule.FileVersionInfo.FileVersion + " " + (MainSvc.Is64Bit(p) ? "x64" : "x86");
					physicalMemUsage += p.WorkingSet64;
					virtualMemUsage += p.VirtualMemorySize64;
					n++;
				}
				record.BiMemUsageMB = (int)(physicalMemUsage / 1000000);
				record.BiPeakVirtualMemUsageMB = (int)(virtualMemUsage / 1000000);
			}

			record.Secret = Program.settings.secret;
			record.OS = GetOsVersion();
			CpuInfo cpuInfo = GetCpuInfo();
			if (cpuInfo == null)
			{
				record.CpuModel = "Unknown";
				record.CpuMHz = NumberUtil.ParseInt(cpuInfo.maxClockSpeed);
			}
			else
			{
				record.CpuModel = cpuInfo.GetModel();
				record.CpuMHz = NumberUtil.ParseInt(cpuInfo.maxClockSpeed);
			}
			record.HelperVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
			record.HwAccel = (byte)RegistryUtil.GetHKLMValue<int>(@"SOFTWARE\Perspective Software\Blue Iris\Options", "hwaccel", 0);

			ComputerInfo computerInfo = new ComputerInfo();
			record.MemMB = (int)(computerInfo.TotalPhysicalMemory / 1000000);
			record.MemFreeMB = (int)(computerInfo.AvailablePhysicalMemory / 1000000);

			RamInfo ramInfo = GetRamInfo();
			record.RamGiB = ramInfo.GiB;
			record.RamChannels = ramInfo.Channels;
			record.RamMHz = ramInfo.MHz;

			// Get camera info.
			// Get frame rates (only accessible via BI's web server).
			Dictionary<string, double> fpsMap = new Dictionary<string, double>();

			BiServerInfo.Reload();
			if (BiServerInfo.enabled)
			{
				try
				{
					using (WebClient wc = new WebClient())
					{
						string session = CameraWebInterfaceLinker.GetSecureAuthenticatedSession(wc);
						string response = wc.UploadString(CameraWebInterfaceLinker.GetJsonURL(), "{\"cmd\":\"camlist\",\"session\":\"" + session + "\"}");
						wc.UploadString(CameraWebInterfaceLinker.GetJsonURL(), "{\"cmd\":\"logout\",\"session\":\"" + session + "\"}");
						CamListResponse camListResponse = JsonConvert.DeserializeObject<CamListResponse>(response);
						if (camListResponse != null && camListResponse.result == "success")
						{
							foreach (CameraListCamera camera in camListResponse.data)
							{
								if (camera.group == null)
									fpsMap[camera.optionValue] = camera.FPS;
							}
						}
					}
				}
				catch (Exception ex)
				{
					Logger.Debug(ex, "Unable to get usage data from web server.");
				}
			}

			// Get camera info from registry
			List<Upload_Camera> cameras = new List<Upload_Camera>();

			RegistryKey camerasKey = RegistryUtil.GetHKLMKey(@"SOFTWARE\Perspective Software\Blue Iris\Cameras");
			foreach (string camName in camerasKey.GetSubKeyNames())
			{
				RegistryKey camKey = camerasKey.OpenSubKey(camName);
				if (RegistryUtil.GetIntValue(camKey, "enabled", 0) != 1)
					continue;
				string shortName = RegistryUtil.GetStringValue(camKey, "shortname");
				Upload_Camera cam = new Upload_Camera();
				if (fpsMap.TryGetValue(shortName, out double fps))
					cam.FPS = (byte)Math.Round(fps);
				else
					cam.FPS = (byte)NumberUtil.Clamp(Math.Round(10000000.0 / RegistryUtil.GetIntValue(camKey, "interval", 1000000)), 0, 255);
				cam.CapType = (byte)RegistryUtil.GetIntValue(camKey, "screencap", 0);
				cam.Hwaccel = (byte)RegistryUtil.GetIntValue(camKey, "ip_hwaccel", 0);
				cam.LimitDecode = RegistryUtil.GetIntValue(camKey, "smartdecode", 0) == 1;
				cam.Pixels = RegistryUtil.GetIntValue(camKey, "fullxres", 0) * RegistryUtil.GetIntValue(camKey, "fullyres", 0);
				cam.Type = (byte)RegistryUtil.GetIntValue(camKey, "type", 0);
				RegistryKey motionKey = camKey.OpenSubKey("Motion");
				cam.MotionDetector = RegistryUtil.GetIntValue(motionKey, "enabled", 0) == 1;
				cam.RecordTriggerType = (byte)RegistryUtil.GetIntValue(motionKey, "continuous", 0);
				RegistryKey clipsKey = camKey.OpenSubKey("Clips");
				cam.RecordFormat = (byte)RegistryUtil.GetIntValue(clipsKey, "movieformat", 0);
				cam.DirectToDisk = RegistryUtil.GetIntValue(clipsKey, "transcode", 0) == 0;
				cam.VCodec = RegistryUtil.GetStringValue(clipsKey, "vcodec");
				cameras.Add(cam);
			}

			record.cameras = cameras.ToArray();
			record.gpus = GetGpuInfo().Select(g => new Upload_Gpu() { Name = g.Name, Version = g.DriverVersion }).ToArray();

			return record;
		}
#pragma warning disable 0649
		private class CamListResponse
		{
			public string result;
			public CameraListCamera[] data;
		}
		private class CameraListCamera
		{
			public string optionValue;
			public string[] group;
			public double FPS;
		}
#pragma warning restore 0649

		private static string GetDebugInfo(Process process)
		{
			return process.TotalProcessorTime.ToString();
		}

		public static string GetOsVersion()
		{
			StringBuilder sb = new StringBuilder();
			string prodName = RegistryUtil.GetHKLMValue(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion", "ProductName", "Unknown");
			sb.Append(prodName);

			string release = RegistryUtil.GetHKLMValue(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion", "ReleaseId", "");
			if (string.IsNullOrWhiteSpace(release))
				sb.Append(" v" + RegistryUtil.GetHKLMValue(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion", "CurrentVersion", "Unknown"));
			else
			{
				sb.Append(" v" + release);
				string build = RegistryUtil.GetHKLMValue(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion", "CurrentBuildNumber", "");
				if (string.IsNullOrWhiteSpace(build))
					build = RegistryUtil.GetHKLMValue(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion", "CurrentBuildNumber", "");
				if (!string.IsNullOrWhiteSpace(build))
					sb.Append(" b" + build);
			}
			if (Environment.Is64BitOperatingSystem)
				sb.Append(" (64 bit)");
			else
				sb.Append(" (32 bit)");
			return sb.ToString();
		}
		public class CpuInfo
		{
			public string clockSpeed, maxClockSpeed, procName, manufacturer, version;
			public CpuInfo(ManagementObject obj)
			{
				maxClockSpeed = obj["MaxClockSpeed"].ToString();
				clockSpeed = obj["CurrentClockSpeed"].ToString();
				procName = obj["Name"].ToString();
				manufacturer = obj["Manufacturer"].ToString();
				version = obj["Version"].ToString();
			}
			public string GetModel()
			{
				if (!string.IsNullOrWhiteSpace(procName))
					return procName;
				else if (!string.IsNullOrWhiteSpace("version"))
					return manufacturer + " " + version;
				else
					return manufacturer;
			}
			public override string ToString()
			{
				return GetModel() + " (" + maxClockSpeed + "MHz)";
			}
		}
		public static CpuInfo GetCpuInfo()
		{
			// win32CompSys = new ManagementObjectSearcher("select * from Win32_ComputerSystem")
			using (ManagementObjectSearcher win32Proc = new ManagementObjectSearcher("select * from Win32_Processor"))
			{
				foreach (ManagementObject obj in win32Proc.Get())
				{
					CpuInfo info = new CpuInfo(obj);
					return info;
				}
			}
			return null;
		}
		public class GpuInfo
		{
			public string Name;
			public string DriverVersion;
			public GpuInfo()
			{
			}
			public GpuInfo(ManagementObject obj)
			{
				Name = obj["Name"].ToString();
				DriverVersion = obj["DriverVersion"].ToString();
			}
		}
		public static List<GpuInfo> GetGpuInfo()
		{
			using (ManagementObjectSearcher objSearcher = new ManagementObjectSearcher("select * from Win32_VideoController"))
			{

				List<GpuInfo> gpus = new List<GpuInfo>();
				foreach (ManagementObject obj in objSearcher.Get())
					gpus.Add(new GpuInfo(obj));
				return gpus;
			}
		}
		private class RamInfo_Internal
		{
			public long Capacity;
			public string DeviceLocator;
			public int Speed;
			public RamInfo_Internal() { }
			public RamInfo_Internal(ManagementObject obj)
			{
				Capacity = NumberUtil.ParseLong(obj["Capacity"].ToString());
				DeviceLocator = obj["DeviceLocator"].ToString();
				Speed = NumberUtil.ParseInt(obj["Speed"].ToString());
				//sb.AppendLine("Bank Label: " + obj["BankLabel"]);
				//sb.AppendLine("Capacity: " + obj["Capacity"]);
				//sb.AppendLine("Data Width: " + obj["DataWidth"]);
				//sb.AppendLine("Description: " + obj["Description"]);
				//sb.AppendLine("Device Locator: " + obj["DeviceLocator"]);
				//sb.AppendLine("Form Factor: " + obj["FormFactor"]);
				//sb.AppendLine("Hot Swappable: " + obj["HotSwappable"]);
				//sb.AppendLine("Manufacturer: " + obj["Manufacturer"]);
				//sb.AppendLine("Memory Type: " + obj["MemoryType"]);
				//sb.AppendLine("Name: " + obj["Name"]);
				//sb.AppendLine("Part Number: " + obj["PartNumber"]);
				//sb.AppendLine("Position In Row: " + obj["PositionInRow"]);
				//sb.AppendLine("Speed: " + obj["Speed"]);
				//sb.AppendLine("Tag: " + obj["Tag"]);
				//sb.AppendLine("Type Detail: " + obj["TypeDetail"]);
			}
		}
		public class RamInfo
		{
			public float GiB;
			public ushort Channels;
			public ushort MHz;
			public RamInfo() { }
			public RamInfo(float GiB, ushort Channels, ushort MHz)
			{
				this.GiB = GiB;
				this.Channels = Channels;
				this.MHz = MHz;
			}
		}
		private static Regex rxGetChannel = new Regex("Channel(.+)-", RegexOptions.Compiled);
		public static RamInfo GetRamInfo()
		{
			List<RamInfo_Internal> dimms = new List<RamInfo_Internal>();
			using (ManagementObjectSearcher win32Memory = new ManagementObjectSearcher("select * from Win32_PhysicalMemory"))
			{
				foreach (ManagementObject obj in win32Memory.Get())
					dimms.Add(new RamInfo_Internal(obj));
			}
			HashSet<string> channels = new HashSet<string>();
			long capacity = 0;
			int speed = 0;
			foreach (RamInfo_Internal dimm in dimms)
			{
				Match m = rxGetChannel.Match(dimm.DeviceLocator);
				if (m.Success)
					channels.Add(m.Groups[1].Value);
				capacity += dimm.Capacity;
				if (speed == 0)
					speed = dimm.Speed;
			}
			return new RamInfo((float)NumberUtil.BytesToGiB(capacity), (ushort)channels.Count, (ushort)speed);
		}
	}
}
