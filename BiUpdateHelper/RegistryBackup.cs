using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BPUtil;

namespace BiUpdateHelper
{
	public static class RegistryBackup
	{
		public static void BackupNow(string destinationFile)
		{
			{
				FileInfo fi7z = new FileInfo(destinationFile + ".7z");
				if (fi7z.Exists)
					return;
				FileInfo fi = new FileInfo(destinationFile);
				if (fi.Exists)
					return;
			}
			Thread thr = new Thread(() =>
			{
				try
				{
					FileInfo fi7z = new FileInfo(destinationFile + ".7z");
					if (fi7z.Exists)
						return;
					FileInfo fi = new FileInfo(destinationFile);
					if (fi.Exists)
						return;
					if (!fi.Directory.Exists)
						Directory.CreateDirectory(fi.Directory.FullName);
					Process p = Process.Start(GetRegeditPath(Program.settings.bi32OnWin64), "/e \"" + destinationFile + "\" \"" + GetBlueIrisKeyPath() + "\"");
					p.WaitForExit();
					fi.Refresh();
					if (fi.Exists)
					{
						ZipFile(destinationFile, destinationFile + ".7z");
						fi.Delete();
						Logger.Info("Registry backup complete: " + destinationFile);
					}
					else
					{
						Logger.Info("Registry backup failed.  Probably, the registry key \"" + GetBlueIrisKeyPath() + "\" does not exist.");
					}
				}
				catch (ThreadAbortException)
				{
					Logger.Debug("Process aborted while backing up registry file: " + destinationFile);
				}
				catch (Exception ex)
				{
					Logger.Debug(ex);
				}
			});
			thr.Name = "Registry Backup";
			thr.Start();
		}
		public static string GetRegeditPath(bool x86)
		{
			string sysPath = "";
			if (Environment.Is64BitOperatingSystem && Environment.Is64BitProcess && x86)
				sysPath = Environment.GetFolderPath(Environment.SpecialFolder.Windows).TrimEnd(Path.DirectorySeparatorChar)
					+ Path.DirectorySeparatorChar + "SysWOW64" + Path.DirectorySeparatorChar;
			return sysPath + "regedit.exe";
		}
		private static string GetBlueIrisKeyPath()
		{
			return "HKEY_LOCAL_MACHINE\\SOFTWARE\\Perspective Software\\Blue Iris";
		}
		private static void ZipFile(string SourcePath, string TargetFile)
		{
			// Run single-threaded 7zip with BelowNormal priority
			ProcessStartInfo psi = new ProcessStartInfo("7zip\\7za.exe", "a -t7z -mmt1 \"" + TargetFile + "\" \"" + SourcePath + "\"");
			psi.CreateNoWindow = true;
			psi.UseShellExecute = false;
			Process proc = Process.Start(psi);
			proc.PriorityClass = ProcessPriorityClass.BelowNormal;
			proc.WaitForExit();
		}
	}
}
