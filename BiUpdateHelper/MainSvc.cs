using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BPUtil;

namespace BiUpdateHelper
{
	public partial class MainSvc : ServiceBase
	{
		private int frozenStateCounter = 0;
		private bool systemInFrozenState = false;
		private bool blueIrisServiceStopping = false;
		Thread thrMain;
		/// <summary>
		/// During one of Blue Iris's failures to shut down in a timely manner:
		/// 1) Total system CPU usage typically hits 100.00% and stays there
		/// 2) % Interrupt time goes into double-digits
		/// 3) The number of System Calls/sec goes into the millions (an increase of over 10x from normal).
		/// </summary>
		PerformanceCounter cpuCounterTotal;
		PerformanceCounter interruptCounterTotal;
		//PerformanceCounter("System", "System Calls/sec", null);

		public MainSvc()
		{
			InitializeComponent();
		}

		public void DoStart()
		{
			OnStart(null);
		}

		public void DoStop()
		{
			OnStop();
		}

		protected override void OnStart(string[] args)
		{
			thrMain = new Thread(UpdateWatch);
			thrMain.Name = "Main Logic";
			thrMain.Start();
		}

		protected override void OnStop()
		{
			thrMain?.Abort();
		}

		private void UpdateWatch()
		{
			try
			{
				try
				{
					Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.AboveNormal;
				}
				catch (ThreadAbortException) { throw; }
				catch { }
				cpuCounterTotal = new PerformanceCounter("Processor", "% Processor Time", "_Total");
				interruptCounterTotal = new PerformanceCounter("Processor", "% Interrupt Time", "_Total");
				DateTime lastDailyRegistryBackup = DateTime.MinValue;
				while (true)
				{
					Thread.Sleep(1500);
					Verbose("Starting Iteration");
					try
					{
						DateTime now = DateTime.Now;
						if (lastDailyRegistryBackup.Year != now.Year || lastDailyRegistryBackup.Month != now.Month || lastDailyRegistryBackup.Day != now.Day)
						{
							lastDailyRegistryBackup = now;
							if (Program.settings.dailyRegistryBackups)
								RegistryBackup.BackupNow(BiUpdateHelperSettings.GetDailyRegistryBackupLocation() + Path.DirectorySeparatorChar + "BI_REG_" + DateTime.Now.ToString("yyyy-MM-dd") + ".reg");
						}
						// Build a list of unique directories that have an active blueiris.exe.
						// There is not likely to be more than one directory, though a single directory 
						// can easily have two blueiris.exe (service and GUI).
						List<BiUpdateMapping> biUpdateMap = GetUpdateInfo();

						if (biUpdateMap.Count == 0)
						{
							Verbose("No Blue Iris processes detected");
						}
						else
						{
							foreach (BiUpdateMapping mapping in biUpdateMap)
							{
								if (mapping.updateProcs.Length > 0)
								{
									// Blue Iris is currently being updated.  Kill the blueiris.exe processes if configured to do so.
									Logger.Info("Blue Iris update detected in path: " + mapping.dirPath);
									if (Program.settings.includeRegistryWithUpdateBackup)
									{
										BiVersionInfo versionInfo = GetBiVersionInfo(mapping);
										TryBackupRegistryForBiVersion(versionInfo);
									}
									if (Program.settings.killBlueIrisProcessesDuringUpdate)
									{
										Verbose("Killing Blue Iris processes");
										mapping.KillBiProcs();
									}
									Verbose("Waiting for update to complete");
									mapping.WaitUntilUpdateProcsStop(TimeSpan.FromMinutes(5));
								}
								else
								{
									// Blue Iris is not being updated in this directory.

									if (Program.settings.killBlueIrisProcessesDuringUpdate && (blueIrisServiceStopping || systemInFrozenState))
									{
										mapping.KillBiProcs();

										if (blueIrisServiceStopping)
											Logger.Info("Blue Iris service found in stopping state. Killed Blue Iris processes.");
										else if (systemInFrozenState)
											Logger.Info("System freeze with high interrupt % detected. Killed Blue Iris processes.");
										continue;
									}

									// Back up the update file if configured to do so.
									if (!Program.settings.backupUpdateFiles)
										continue;

									//Check for the existence of an update.exe file
									FileInfo fiUpdate = new FileInfo(mapping.dirPath + "update.exe");
									if (!fiUpdate.Exists)
									{
										Verbose("No update file to back up in path: " + mapping.dirPath);
										continue;
									}

									Verbose("Backing up update file: " + fiUpdate.FullName);

									// Get Blue Iris process(es) (necessary to learn if it is 64 or 32 bit)
									BiVersionInfo versionInfo = GetBiVersionInfo(mapping);
									if (versionInfo == null)
										continue; // BI is probably not running

									FileInfo targetUpdateFile = new FileInfo(mapping.dirPath + "update" + versionInfo.cpu_32_64 + "_" + versionInfo.version + ".exe");
									if (targetUpdateFile.Exists)
									{
										// A backed-up update file for the active Blue Iris version already exists, so we should do nothing now
										Verbose("Target update file \"" + targetUpdateFile.FullName + "\" already exists.  Probably, the new update file hasn't been installed yet.");
										TryBackupRegistryForBiVersion(versionInfo);
									}
									else
									{
										// Find out if the file can be opened exclusively (meaning it is finished downloading, etc)
										bool fileIsUnlocked = false;
										try
										{
											using (FileStream fs = new FileStream(fiUpdate.FullName, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
											{
												fileIsUnlocked = true;
											}
										}
										catch (ThreadAbortException) { throw; }
										catch (Exception) { }

										if (fileIsUnlocked)
										{
											// This is a pretty good sign that the update is not curently being installed, so we should be safe to rename the update file.
											Logger.Info("Renaming update file to: " + targetUpdateFile.FullName);
											fiUpdate.MoveTo(targetUpdateFile.FullName);
										}
										else
											Verbose("Update file could not be exclusively locked. Backup will not occur this iteration.");
									}
								}
							}
						}
					}
					catch (ThreadAbortException) { throw; }
					catch (Exception ex)
					{
						Logger.Debug(ex);
					}
				}
			}
			catch (ThreadAbortException) { }
			catch (Exception ex)
			{
				Logger.Debug(ex);
			}
		}

		private BiVersionInfo GetBiVersionInfo(BiUpdateMapping mapping)
		{
			try
			{
				if (mapping?.biProcs?.Length > 0)
				{
					string cpu_32_64 = Is64Bit(mapping.biProcs[0].process) ? "64" : "32";

					FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(mapping.biProcs[0].path);
					string version = fvi.FileVersion;

					return new BiVersionInfo(cpu_32_64, version);
				}
			}
			catch (ThreadAbortException) { throw; }
			catch (Exception ex)
			{
				Logger.Debug(ex);
			}
			return null;
		}
		private void TryBackupRegistryForBiVersion(BiVersionInfo versionInfo)
		{
			if (versionInfo != null && Program.settings.includeRegistryWithUpdateBackup)
				RegistryBackup.BackupNow(BiUpdateHelperSettings.GetBeforeUpdatesRegistryBackupLocation() + Path.DirectorySeparatorChar + "BI_REG_" + versionInfo.cpu_32_64 + "-" + versionInfo.version + ".reg");
		}

		private List<BiUpdateMapping> GetUpdateInfo()
		{
			// This method must not be called too often if querying PerformanceCounter (at LEAST 100ms between calls or the returned data will be highly inaccurate).
			systemInFrozenState = false;
			blueIrisServiceStopping = false;
			List<RelatedProcessInfo> allBiProcs = new List<RelatedProcessInfo>();
			List<RelatedProcessInfo> allUpdateProcs = new List<RelatedProcessInfo>();
			List<string> biPaths;
			{
				Process[] allProcs = Process.GetProcesses();
				HashSet<string> hsBiPaths = new HashSet<string>();
				foreach (Process p in allProcs)
				{
					string name = p.ProcessName;
					string nameLower = name.ToLower();
					if (nameLower == "blueiris")
					{
						string path = GetPath(p);
						if (path == null)
						{
							Logger.Info("Unable to get file path for process " + p.Id + ": " + name);
							continue;
						}
						string dirPath = new FileInfo(path).Directory.FullName.TrimEnd('\\', '/') + Path.DirectorySeparatorChar;
						if (!hsBiPaths.Contains(dirPath))
							hsBiPaths.Add(dirPath);
						allBiProcs.Add(new RelatedProcessInfo(p, name, path));
					}
					else if (nameLower.StartsWith("update"))
					{
						string path = GetPath(p);
						if (path == null)
						{
							Logger.Info("Unable to get file path for process " + p.Id + ": " + name);
							continue;
						}
						allUpdateProcs.Add(new RelatedProcessInfo(p, name, path));
					}
					//else if (!blueIrisServiceStopping && nameLower == "blueirisservice")
					//{ // DISABLED because this was aborting updates and leaving Blue Iris not running.
					//	using (ServiceController sc = new ServiceController("BlueIris"))
					//		blueIrisServiceStopping = sc.Status == ServiceControllerStatus.StopPending;
					//}
				}
				biPaths = hsBiPaths.ToList();
			}
			List<BiUpdateMapping> biUpdateMap = new List<BiUpdateMapping>();
			foreach (string path in biPaths)
			{
				RelatedProcessInfo[] biProcs = allBiProcs.Where(p => p.path.StartsWith(path)).ToArray();
				RelatedProcessInfo[] updateProcs = allUpdateProcs.Where(p => p.path.StartsWith(path)).ToArray();
				Verbose("Found " + biProcs.Length + " blue iris processes and " + updateProcs.Length + " update processes running under path: " + path);
				biUpdateMap.Add(new BiUpdateMapping(biProcs, updateProcs, path));
			}
			// Read performance counters to guess if the system is in a frozen state
			float cpu = cpuCounterTotal.NextValue();
			float interrupt = interruptCounterTotal.NextValue();
			//float total = cpu + interrupt;
			//File.AppendAllText(Globals.ApplicationDirectoryBase + "CPU.txt", DateTime.Now.ToString() + ": CPU % " + cpu + ", interrupt % " + interrupt + ", CPU + Interrupt: " + (cpu + interrupt) + Environment.NewLine);
			if (biPaths.Count > 0 && Math.Abs(100f - cpu) < 0.05f && interrupt > 15)
			{
				systemInFrozenState = frozenStateCounter >= 1;
				frozenStateCounter++;
			}
			else
				frozenStateCounter = 0;
			return biUpdateMap;
		}

		private static string GetPath(Process p)
		{
			try
			{
				return p.MainModule.FileName;
			}
			catch (ThreadAbortException) { throw; }
			catch (Exception) { }
			return null;
		}
		private void Verbose(string str)
		{
			if (Program.settings.logVerbose)
				Logger.Info(str);
		}
		private bool Is64Bit(Process p)
		{
			if (!Environment.Is64BitOperatingSystem)
				return false;

			bool isWow64;
			if (!IsWow64Process(p.Handle, out isWow64))
				throw new Win32Exception("IsWow64Process call returned false");
			return !isWow64;
		}
		[DllImport("kernel32.dll", SetLastError = true, CallingConvention = CallingConvention.Winapi)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool IsWow64Process([In] IntPtr process, [Out] out bool wow64Process);
	}
	public class BiVersionInfo
	{
		public string cpu_32_64;
		public string version;
		public BiVersionInfo(string cpu_32_64, string version)
		{
			this.cpu_32_64 = cpu_32_64;
			this.version = version;
		}
	}
	public class RelatedProcessInfo
	{
		public Process process;
		public string name;
		public string path;
		public RelatedProcessInfo(Process process, string name, string path)
		{
			this.process = process;
			this.name = name;
			this.path = path;
		}
	}
	/// <summary>
	/// Represents one or more blueiris.exe processes and one or more update processes that are all running from the same directory.
	/// </summary>
	public class BiUpdateMapping
	{
		public RelatedProcessInfo[] biProcs;
		public RelatedProcessInfo[] updateProcs;
		public string dirPath;
		public BiUpdateMapping(RelatedProcessInfo[] biProcs, RelatedProcessInfo[] updateProcs, string dirPath)
		{
			this.biProcs = biProcs;
			this.updateProcs = updateProcs;
			this.dirPath = dirPath;
		}

		public void KillBiProcs()
		{
			foreach (RelatedProcessInfo rpi in biProcs)
			{
				try
				{
					if (!rpi.process.HasExited)
						rpi.process.Kill();
				}
				catch (ThreadAbortException) { throw; }
				catch (Exception ex)
				{
					Logger.Debug(ex, "Unable to kill process: " + rpi.path);
				}
			}
		}

		public void WaitUntilUpdateProcsStop(TimeSpan timeout)
		{
			if (timeout < TimeSpan.Zero)
				timeout = TimeSpan.FromMinutes(5);
			Stopwatch sw = new Stopwatch();
			sw.Start();
			while (sw.Elapsed < timeout)
			{
				bool allUpdateProcsExited = true;
				foreach (RelatedProcessInfo rpi in updateProcs)
				{
					if (!rpi.process.HasExited)
					{
						allUpdateProcsExited = false;
						break;
					}
				}
				if (allUpdateProcsExited)
					break;
				Thread.Sleep(1000);
			}
		}
	}
}
