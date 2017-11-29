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
		/// <summary>
		/// If true, the helper service will step-in and kill the Blue Iris process during BI updates and when it is believed that BI is trying to shut down.
		/// </summary>
		public bool killBlueIrisProcessesDuringUpdate = true;
		public bool backupUpdateFiles = true;
		public bool includeRegistryWithUpdateBackup = true;
		public bool dailyRegistryBackups = true;
		public bool logVerbose = false;
		public bool bi32OnWin64 = false;
		public string secret = "";
		public long lastUsageReportAt = 0;

		/// <summary>
		/// Returns the absolute path to the RegistryBackups/Daily folder, not including trailing Path.DirectorySeparatorChar.  This method will also create that directory if it does not exist.
		/// </summary>
		/// <returns></returns>
		public static string GetDailyRegistryBackupLocation()
		{
			string path = GetRegistryBackupLocation() + Path.DirectorySeparatorChar + "Daily";
			if (!Directory.Exists(path))
				Directory.CreateDirectory(path);
			return path;
		}
		/// <summary>
		/// Returns the absolute path to the RegistryBackups/BeforeUpdates folder, not including trailing Path.DirectorySeparatorChar.  This method will also create that directory if it does not exist.
		/// </summary>
		/// <returns></returns>
		public static string GetBeforeUpdatesRegistryBackupLocation()
		{
			string path = GetRegistryBackupLocation() + Path.DirectorySeparatorChar + "BeforeUpdates";
			if (!Directory.Exists(path))
				Directory.CreateDirectory(path);
			return path;
		}
		/// <summary>
		/// Returns the absolute path to the RegistryBackups/Manual folder, not including trailing Path.DirectorySeparatorChar.  This method will also create that directory if it does not exist.
		/// </summary>
		/// <returns></returns>
		public static string GetManualRegistryBackupLocation()
		{
			string path = GetRegistryBackupLocation() + Path.DirectorySeparatorChar + "Manual";
			if (!Directory.Exists(path))
				Directory.CreateDirectory(path);
			return path;
		}
		/// <summary>
		/// Returns the absolute path to the RegistryBackups folder, not including trailing Path.DirectorySeparatorChar.  This method will also create that directory if it does not exist.
		/// </summary>
		/// <returns></returns>
		public static string GetRegistryBackupLocation()
		{
			FileInfo fiExe = new FileInfo(Assembly.GetExecutingAssembly().Location);
			string path = fiExe.Directory.FullName.TrimEnd('\\', '/') + Path.DirectorySeparatorChar + "RegistryBackups";
			if (!Directory.Exists(path))
				Directory.CreateDirectory(path);
			return path;
		}
	}
}
