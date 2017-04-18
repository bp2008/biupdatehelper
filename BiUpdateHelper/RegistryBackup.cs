using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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
					Process p = Process.Start("regedit.exe", "/e \"" + destinationFile + "\" \"HKEY_LOCAL_MACHINE\\SOFTWARE\\Perspective Software\\Blue Iris\"");
					p.WaitForExit();
					ZipFile(destinationFile, destinationFile + ".7z");
					fi.Refresh();
					fi.Delete();
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
