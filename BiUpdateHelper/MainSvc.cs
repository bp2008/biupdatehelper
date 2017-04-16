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

namespace BiUpdateHelper
{
	public partial class MainSvc : ServiceBase
	{
		Thread thrMain;
		BiUpdateHelperSettings settings;

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
			string exePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
			FileInfo fiExe = new FileInfo(exePath);
			Environment.CurrentDirectory = fiExe.Directory.FullName;

			settings = new BiUpdateHelperSettings();
			settings.Load();
			settings.SaveDefaultIfNoExist();

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
				while (true)
				{
					Thread.Sleep(1500);
					Verbose("Starting Iteration");
					try
					{
						// Build a list of unique directories that have an active blueiris.exe.
						// There is not likely to be more than one directory, though a single directory 
						// can easily have two blueiris.exe (service and GUI).
						List<BiUpdateMapping> biUpdateMap = new List<BiUpdateMapping>();

						Process[] allProcs = Process.GetProcesses();
						Process[] allBiProcs = allProcs.Where(p => string.Compare("blueiris", p.ProcessName, true) == 0).ToArray();
						Verbose("Found " + allBiProcs.Length + " blueiris.exe processes");
						HashSet<string> biPaths = new HashSet<string>();
						foreach (Process p in allBiProcs)
						{
							FileInfo fi = new FileInfo(GetPath(p));
							string path = fi.Directory.FullName.TrimEnd('\\', '/') + Path.DirectorySeparatorChar;
							if (!biPaths.Contains(path))
							{
								Verbose("Found path: " + path);
								biPaths.Add(path);
							}
						}
						foreach (string path in biPaths)
						{
							Process[] everythingInDir = allProcs.Where(p => GetPath(p).StartsWith(path)).ToArray();
							Process[] updateProcs = everythingInDir.Where(p => p.ProcessName.ToLower().StartsWith("update")).ToArray();
							Verbose("Found " + updateProcs.Length + " update processes running under path:" + path);
							if (updateProcs.Length > 0)
							{
								Process[] biProcs = everythingInDir.Where(p => string.Compare("blueiris", p.ProcessName, true) == 0).ToArray();
								BiUpdateMapping mapping = new BiUpdateMapping(biProcs, updateProcs);
								biUpdateMap.Add(mapping);
							}
						}

						if (biUpdateMap.Count > 0)
						{
							// Blue Iris is currently being updated.  Kill the blueiris.exe processes if configured to do so.
							foreach (BiUpdateMapping mapping in biUpdateMap)
							{
								Verbose("Blue Iris update detected!");
								if (settings.killBlueIrisProcessesDuringUpdate)
								{
									Verbose("Killing Blue Iris processes");
									mapping.KillBiProcs();
								}
								Verbose("Waiting for update to complete");
								mapping.WaitUntilUpdateProcsStop(TimeSpan.FromMinutes(5));
							}
						}
						else
						{
							// Blue Iris is not being updated.
							if (settings.backupUpdateFiles)
							{
								//Check for the existence of an update.exe file in any path that has blueiris.exe currently running.
								foreach (string path in biPaths)
								{
									Verbose("Backing up update files in path: " + path);
									FileInfo fiBi = new FileInfo(path + "blueiris.exe");
									if (fiBi.Exists)
									{
										FileInfo fiUpdate = new FileInfo(path + "update.exe");
										if (fiUpdate.Exists)
										{
											// An update file exists with the default name.
											Verbose("An update file exists with the default name");

											// Get Blue Iris process(es) (necessary to learn if it is 64 or 32 bit)
											Process[] biProcs = allBiProcs.Where(p => GetPath(p).StartsWith(path)).ToArray();
											if (biProcs.Length > 0)
											{
												string cpu_32_64 = Is64Bit(biProcs[0]) ? "64" : "32";

												// Get current Blue Iris version.
												FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(GetPath(biProcs[0]));
												string version = fvi.FileVersion;

												FileInfo targetUpdateFile = new FileInfo(path + "update" + cpu_32_64 + "_" + version + ".exe");
												Verbose("Target update file is: " + targetUpdateFile.FullName);
												if (targetUpdateFile.Exists)
												{
													// A backed-up update file for the active Blue Iris version already exists, so we should do nothing now
													Verbose("Target update file already exists");
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
		private static string GetPath(Process p)
		{
			try
			{
				return p.MainModule.FileName;
			}
			catch (ThreadAbortException) { throw; }
			catch (Exception) { }
			return "";
		}
		private void Verbose(string str)
		{
			if (settings.logVerbose)
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
	/// <summary>
	/// Represents one or more blueiris.exe processes and one or more update processes that are all running from the same directory.
	/// </summary>
	public class BiUpdateMapping
	{
		public Process[] biProcs;
		public Process[] updateProcs;
		public BiUpdateMapping(Process[] biProcs, Process[] updateProcs)
		{
			this.biProcs = biProcs;
			this.updateProcs = updateProcs;
		}

		public void KillBiProcs()
		{
			foreach (Process p in biProcs)
			{
				try
				{
					p.Kill();
				}
				catch (ThreadAbortException ex) { throw; }
				catch (Exception ex)
				{
					Logger.Debug(ex, "Unable to kill blueiris.exe process");
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
				foreach (Process p in updateProcs)
				{
					if (!p.HasExited)
					{
						allUpdateProcsExited = false;
						break;
					}
				}
				if (allUpdateProcsExited)
					break;
			}
		}
	}
}
