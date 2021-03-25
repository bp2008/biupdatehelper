using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using BPUtil;

namespace BiUpdateHelper
{
	public class BiUpdateHelperSettings : SerializableObjectBase
	{
		///// <summary>
		///// If true, the helper service will step-in and kill the Blue Iris process during BI updates and when it is believed that BI is trying to shut down.
		///// </summary>
		public bool killBlueIrisProcessesDuringUpdate2 = false;
		public bool backupUpdateFiles = true;
		public bool includeRegistryWithUpdateBackup = true;
		public bool dailyRegistryBackups = true;
		public bool logVerbose = false;
		public bool bi32OnWin64 = false;
		public string secret = "";
		public long lastUsageReportAt = 0;
		public string registryBackupsFolderPath = "";

		/// <summary>
		/// Returns the absolute path to the RegistryBackups/Daily folder, not including trailing Path.DirectorySeparatorChar.  This method will also create that directory if it does not exist.
		/// </summary>
		/// <returns></returns>
		public string GetDailyRegistryBackupLocation()
		{
			string path = Path.Combine(GetRegistryBackupLocation(), "Daily");
			if (!Directory.Exists(path))
				Directory.CreateDirectory(path);
			return path;
		}
		/// <summary>
		/// Returns the absolute path to the RegistryBackups/BeforeUpdates folder, not including trailing Path.DirectorySeparatorChar.  This method will also create that directory if it does not exist.
		/// </summary>
		/// <returns></returns>
		public string GetBeforeUpdatesRegistryBackupLocation()
		{
			string path = Path.Combine(GetRegistryBackupLocation(), "BeforeUpdates");
			if (!Directory.Exists(path))
				Directory.CreateDirectory(path);
			return path;
		}
		/// <summary>
		/// Returns the absolute path to the RegistryBackups/Manual folder, not including trailing Path.DirectorySeparatorChar.  This method will also create that directory if it does not exist.
		/// </summary>
		/// <returns></returns>
		public string GetManualRegistryBackupLocation()
		{
			string path = Path.Combine(GetRegistryBackupLocation(), "Manual");
			if (!Directory.Exists(path))
				Directory.CreateDirectory(path);
			return path;
		}
		/// <summary>
		/// Returns the absolute path to the RegistryBackups folder, not including trailing Path.DirectorySeparatorChar.  This method will also create that directory if it does not exist.
		/// </summary>
		/// <returns></returns>
		public string GetRegistryBackupLocation()
		{
			string path;
			if (!string.IsNullOrWhiteSpace(registryBackupsFolderPath))
			{
				try
				{
					path = Path.Combine(registryBackupsFolderPath, "RegistryBackups");
					if (!Directory.Exists(path))
						Directory.CreateDirectory(path);
					if (Directory.Exists(path))
						return path;
				}
				catch (Exception ex)
				{
					Logger.Debug(ex);
				}
			}
			// Fallback to application directory.
			FileInfo fiExe = new FileInfo(Assembly.GetExecutingAssembly().Location);
			path = fiExe.Directory.FullName.TrimEnd('\\', '/') + Path.DirectorySeparatorChar + "RegistryBackups";
			if (!Directory.Exists(path))
				Directory.CreateDirectory(path);
			return path;
		}
	}
}
