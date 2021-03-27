using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using BlueIrisRegistryReader;
using BPUtil;
using Microsoft.VisualBasic.Devices;
using Microsoft.Win32;
using Newtonsoft.Json;

namespace BiUpdateHelper.PerformanceData
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
		public short CpuThreads; // New in v2(.1)
		public int MemMB;
		public int BiMemUsageMB;
		public int BiPeakVirtualMemUsageMB;
		public int MemFreeMB;
		public byte HwAccel;
		public float RamGiB;
		public ushort RamChannels;
		public string DimmLocations; // New in v2
		public ushort RamMHz;
		public bool ServiceMode; // New in v2
		public bool ConsoleOpen; // New in v2
		public short ConsoleWidth = -1; // New in v2
		public short ConsoleHeight = -1; // New in v2
		public short LivePreviewFPS = -1; // New in v2
		/// <summary>
		/// Total number of pixels, in millions, of all cameras together.
		/// </summary>
		public float Total_Megapixels; // New in 1.6.4.3, ignored by server
		/// <summary>
		/// Total number of frames per second being input into the system.
		/// </summary>
		public float Total_FPS; // New in 1.6.4.3, ignored by server
		/// <summary>
		/// Megapixels per second being input into the system.
		/// </summary>
		public float Total_MPPS; // New in 1.6.4.3, ignored by server
		public byte webserverState = 0; // New in 1.7.0.0. Can be used to better determine which fields are accurate.  0: Failed to get things from web server. 1: Got all non-admin things. 2: Got all admin-requiring things.
		public bool ProfileConfirmed; // New in 1.7.1.0. Indicates that the profile number came from the web server so related settings can be considered reliable.
		public bool AllFPSConfirmed; // New in 1.7.1.0. True if all the FPS values came from the web server and are therefore reliable.
		public Upload_Camera[] cameras;
		public Upload_Gpu[] gpus;
	}
	public class Upload_Camera
	{
		public int Pixels;
		public int MainPixels; // New in 1.9.0.0. If nonzero, the camera is believed to have a sub stream configured.
		public byte FPS;
		public bool FPSConfirmed; // New in 1.7.1.0. True if the FPS came from the web server and is therefore reliable.
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
	public static class PerformanceDataCollector
	{
		private static DateTime startedReportAt = DateTime.MinValue;
		private static SemaphoreSlim sem = new SemaphoreSlim(1, 1);
		private static bool uploadNextReport;
		private static bool saveLocalNextReport;
		/// <summary>
		/// REQUIRED callback that returns the secret string for this machine's performance data uploads.
		/// </summary>
		public static Func<string> getSecretString;
		/// <summary>
		/// REQUIRED callback that is called when a report is uploaded.
		/// </summary>
		public static Action onReportUploaded;
		/// <summary>
		/// If it is time to submit an anonymous Blue Iris usage report, do it in a background thread.
		/// </summary>
		/// <param name="lastUsageReportAt">Epoch time (milliseconds) of the last usage report.</param>
		public static void HandlePossiblePerfDataReport(long lastUsageReportAt)
		{
			DateTime lastReportAt = TimeUtil.DateTimeFromEpochMS(lastUsageReportAt);
			DateTime now = DateTime.UtcNow;
			if (now < lastReportAt)
				lastReportAt = DateTime.MinValue;
			if ((now - lastReportAt).TotalDays < 7)
				return; // It hasn't been 7 days since the last report.  Don't generate a new report.
			if ((now - startedReportAt).TotalMinutes < 60)
				return; // It hasn't been 60 minutes since the last attempt.  Don't generate a new report.

			CreatePerfDataReport(true, false);
		}
		/// <summary>
		/// Starts a performance data (CPU usage) report on a background thread and returns true.  Returns false without starting a report if another report is currently being generated.
		/// </summary>
		/// <param name="upload">If true, the report will be uploaded to the website.</param>
		/// <param name="saveLocal">If true, the report will be saved to perfdata.json in the current directory.</param>
		/// <returns></returns>
		public static bool CreatePerfDataReport(bool upload, bool saveLocal)
		{
			if (!sem.Wait(0))
				return false;

			uploadNextReport = upload;
			saveLocalNextReport = saveLocal;

			startedReportAt = DateTime.UtcNow;
			Thread thrPerfDataReport = new Thread(PerfDataReportBackground);
			thrPerfDataReport.Name = "Performance Report";
			thrPerfDataReport.IsBackground = true;
			thrPerfDataReport.Start();
			return true;
		}
		/// <summary>
		/// Submits a usage report synchronously.  Should be called from a background thread.
		/// </summary>
		private static void PerfDataReportBackground()
		{
			try
			{
				// Generate record.
				Upload_Record record = GetPerfDataRecord();
				if (record == null)
					return;

				// Submit record.
				if (saveLocalNextReport)
					File.WriteAllText("perfdata.json", JsonConvert.SerializeObject(record, Formatting.Indented));

				if (uploadNextReport)
				{
					string jsonPayload = JsonConvert.SerializeObject(record);
					string md5 = Hash.GetMD5Hex(jsonPayload);
					using (WebClient wc = new WebClient())
					{
						wc.UploadString("https://biupdatehelper.hopto.org/api/uploadUsageRecord2", jsonPayload + md5);
					}
				}

				// Save the time so this doesn't happen again for a while.
				onReportUploaded();
			}
			catch (ThreadAbortException) { }
			catch (Exception ex)
			{
				Logger.Debug(ex, "Unable to generate anonymous performance data record.");
			}
			finally
			{
				sem.Release();
			}
		}
		/// <summary>
		/// Creates an anonymous usage record.  This method spends 10 seconds measuring CPU usage.  Returns null if any BlueIris.exe processes close while CPU usage is being measured, or if no BlueIris.exe processes were open.
		/// </summary>
		/// <returns></returns>
		private static Upload_Record GetPerfDataRecord()
		{
			Upload_Record record = new Upload_Record();

			BlueIrisConfiguration c = new BlueIrisConfiguration();
			c.Load();

			record.CpuUsage = c.activeStats.CpuUsage;
			record.BiCpuUsage = c.activeStats.BiCpuUsage;
			record.CpuThreads = (short)Environment.ProcessorCount;
			record.BiVersion = c.activeStats.BiVersion;
			record.BiMemUsageMB = c.activeStats.BiMemUsageMB;
			record.BiPeakVirtualMemUsageMB = c.activeStats.BiPeakVirtualMemUsageMB;
			record.ConsoleOpen = c.activeStats.ConsoleOpen;
			record.ConsoleWidth = c.activeStats.ConsoleWidth;
			record.ConsoleHeight = c.activeStats.ConsoleHeight;
			record.Secret = getSecretString();
			record.OS = c.OS;

			if (c.cpu == null)
			{
				record.CpuModel = "Unknown";
				record.CpuMHz = 0;
			}
			else
			{
				record.CpuModel = c.cpu.GetModel();
				record.CpuMHz = NumberUtil.ParseInt(c.cpu.maxClockSpeed);
			}

			record.HelperVersion = System.Reflection.Assembly.GetEntryAssembly().GetName().Version.ToString();
			record.HwAccel = (byte)c.global.HardwareAcceleration;
			record.ServiceMode = c.global.ServiceMode;
			record.LivePreviewFPS = (short)c.global.LivePreviewFPS;

			record.MemMB = c.activeStats.MemMB;
			record.MemFreeMB = c.activeStats.MemFreeMB;

			record.RamGiB = c.mem.GiB;
			record.RamChannels = c.mem.Channels;
			record.DimmLocations = c.mem.DimmLocations;
			record.RamMHz = c.mem.MHz;

			// Get camera info.
			// Get frame rates, current profile (only accessible via BI's web server).
			Dictionary<string, double> fpsMap = new Dictionary<string, double>();

			int currentProfile = 1;
			bool isAdmin = false;
			bool gotCamlist = false;
			bool gotStatus = false;
			BiServerInfo.Reload();
			//BiUserInfo.Reload();
			if (BiServerInfo.enabled)
			{
				try
				{
					using (WebClient wc = new WebClient())
					{
						UserInfo user = BiUserInfo.CreateTemporaryUser();
						string session = CameraWebInterfaceLinker.GetSecureAuthenticatedSession(wc, out isAdmin, user.name, user.GetDecodedPassword());
						try
						{
							try
							{
								string response = wc.UploadString(CameraWebInterfaceLinker.GetJsonURL(), "{\"cmd\":\"camlist\",\"session\":\"" + session + "\"}");
								CamListResponse camListResponse = JsonConvert.DeserializeObject<CamListResponse>(response);
								if (camListResponse != null && camListResponse.result == "success")
								{
									foreach (CameraListCamera camera in camListResponse.data)
									{
										if (camera.group == null)
											fpsMap[camera.optionValue] = camera.FPS;
									}
									gotCamlist = true;
								}
							}
							catch (Exception ex)
							{
								Logger.Debug(ex, "Error reading camera list from web server.");
							}

							try
							{
								string response = wc.UploadString(CameraWebInterfaceLinker.GetJsonURL(), "{\"cmd\":\"status\",\"session\":\"" + session + "\"}");
								StatusResponse statusResponse = JsonConvert.DeserializeObject<StatusResponse>(response);
								if (statusResponse != null && statusResponse.result == "success")
								{
									currentProfile = statusResponse.data.profile;
									gotStatus = true;
									if (currentProfile < 1 || currentProfile > 7)
									{
										currentProfile = 1;
										gotStatus = false;
									}
									else
										record.ProfileConfirmed = true;
								}
							}
							catch (Exception ex)
							{
								Logger.Debug(ex, "Error reading camera list from web server.");
							}
						}
						finally
						{
							wc.UploadString(CameraWebInterfaceLinker.GetJsonURL(), "{\"cmd\":\"logout\",\"session\":\"" + session + "\"}");
						}
					}
				}
				catch (Exception ex)
				{
					Logger.Debug(ex, "Error dealing with web server.");
				}
			}
			record.webserverState = 0;
			if (gotCamlist && gotStatus)
			{
				record.webserverState = 1;
				if (isAdmin) // and any future admin-requiring stuff all worked
					record.webserverState = 2;
			}


			// Get camera info
			List<Upload_Camera> cameras = new List<Upload_Camera>();
			foreach (Camera camSrc in c.cameras.Values)
			{
				if (!camSrc.enabled)
					continue;

				Upload_Camera cam = new Upload_Camera();

				if (fpsMap.TryGetValue(camSrc.shortname, out double fps))
				{
					cam.FPS = (byte)Math.Round(fps).Clamp(0, 255);
					cam.FPSConfirmed = true;
				}
				else
					cam.FPS = (byte)(Math.Round(camSrc.MaxRate).Clamp(0, 255));
				cam.CapType = (byte)camSrc.CapType;
				cam.Hwaccel = (byte)camSrc.Hwva;
				cam.LimitDecode = camSrc.LimitDecode;
				cam.Pixels = camSrc.Pixels;
				if (camSrc.hasSubStream)
					cam.MainPixels = camSrc.MainPixels;
				cam.Type = (byte)camSrc.Type;
				cam.MotionDetector = camSrc.triggerSettings[currentProfile].motionDetectionEnabled;
				cam.RecordTriggerType = (byte)camSrc.recordSettings[currentProfile].triggerType;
				cam.RecordFormat = (byte)camSrc.recordSettings[currentProfile].recordingFormat;
				cam.DirectToDisk = camSrc.recordSettings[currentProfile].DirectToDisc;
				cam.VCodec = camSrc.recordSettings[currentProfile].VCodec;
				cameras.Add(cam);
			}

			record.AllFPSConfirmed = cameras.All(cam => cam.FPSConfirmed); // Ignored and recalculated by server. This exists here for the sake of local json output.
			record.cameras = cameras.ToArray();
			record.gpus = c.gpus.Select(g => new Upload_Gpu() { Name = g.Name, Version = g.DriverVersion }).ToArray();

			record.Total_FPS = record.Total_Megapixels = record.Total_MPPS = 0;
			foreach (Upload_Camera cam in cameras)
			{
				float MP = cam.Pixels / 1000000f;
				record.Total_Megapixels += MP;
				record.Total_FPS += cam.FPS;
				record.Total_MPPS += MP * cam.FPS;
			}

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
		private class StatusResponse
		{
			public string result;
			public StatusData data;
		}
		private class StatusData
		{
			public string result;
			public int profile;
		}
#pragma warning restore 0649

		private static string GetDebugInfo(Process process)
		{
			return process.TotalProcessorTime.ToString();
		}
	}
}
