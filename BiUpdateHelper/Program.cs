using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using BPUtil;
using BPUtil.Forms;

namespace BiUpdateHelper
{
	static class Program
	{
		public static BiUpdateHelperSettings settings;
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		static void Main()
		{
			string exePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
			Globals.Initialize(exePath);
			PrivateAccessor.SetStaticFieldValue(typeof(Globals), "errorFilePath", Globals.WritableDirectoryBase + "BiUpdateHelper_Log.txt");

			FileInfo fiExe = new FileInfo(exePath);
			Environment.CurrentDirectory = fiExe.Directory.FullName;

			settings = new BiUpdateHelperSettings();
			settings.Load();
			if (string.IsNullOrWhiteSpace(settings.secret))
			{
				settings.secret = Hash.GetSHA256Hex(Guid.NewGuid().ToString());
				settings.Save();
			}
			settings.SaveIfNoExist();

			RegistryUtil.Force32BitRegistryAccess = settings.bi32OnWin64;

			AutoFixBad32BitSetting();

			if (Environment.UserInteractive)
			{
				string Title = "BiUpdateHelper " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString() + " Service Manager";
				string ServiceName = "BiUpdateHelper";
				ButtonDefinition btnRegKey = new ButtonDefinition("BI Registration Info", btnRegkey_Click);
				ButtonDefinition btnSettings = new ButtonDefinition("Edit Service Settings", btnSettings_Click);
				ButtonDefinition btnCameraConfigLinks = new ButtonDefinition("Camera Config Links", btnCameraConfigLinks_Click);
				ButtonDefinition btnRegistryBackupNow = new ButtonDefinition("Take Registry Backup Now", btnRegistryBackupNow_Click);
				ButtonDefinition btnPerfData = new ButtonDefinition("Performance Data", btnPerfData_Click);
				ButtonDefinition[] customButtons = new ButtonDefinition[] { btnRegKey, btnSettings, btnCameraConfigLinks, btnRegistryBackupNow, btnPerfData };

				System.Windows.Forms.Application.Run(new ServiceManager(Title, ServiceName, customButtons));
			}
			else
			{
				ServiceBase[] ServicesToRun;
				ServicesToRun = new ServiceBase[]
				{
					new MainSvc()
				};
				ServiceBase.Run(ServicesToRun);
			}
		}

		private static void AutoFixBad32BitSetting()
		{
			if (Environment.Is64BitOperatingSystem)
			{
				// This is a 64 bit OS, so it is possible that our bi32OnWin64 setting is wrong.
				if (RegistryUtil.GetHKLMKey(@"SOFTWARE\Perspective Software\Blue Iris") == null)
				{
					// No BI detected. Try the opposite setting.
					RegistryUtil.Force32BitRegistryAccess = !RegistryUtil.Force32BitRegistryAccess;
					if (RegistryUtil.GetHKLMKey(@"SOFTWARE\Perspective Software\Blue Iris") == null)
						RegistryUtil.Force32BitRegistryAccess = !RegistryUtil.Force32BitRegistryAccess; // No BI detected. Revert setting.
					else
					{
						// Found BI using the opposite setting.  Save the setting.
						settings.bi32OnWin64 = RegistryUtil.Force32BitRegistryAccess;
						settings.Save();
					}
				}
			}
		}

		private static void btnRegkey_Click(object sender, EventArgs e)
		{
			RegKey regKeyDialog = new RegKey();
			regKeyDialog.ShowDialog();
		}

		private static void btnSettings_Click(object sender, EventArgs e)
		{
			ServiceSettings settingsDialog = new ServiceSettings();
			settingsDialog.ShowDialog();
		}

		private static void btnCameraConfigLinks_Click(object sender, EventArgs e)
		{
			FileInfo fiExe = new FileInfo(System.Windows.Forms.Application.ExecutablePath);
			string path = fiExe.Directory.FullName.TrimEnd('\\', '/') + Path.DirectorySeparatorChar + "CameraConfigLinks.html";
			CameraWebInterfaceLinker.GenerateWebInterfaceLinkDocument(path);
		}

		private static void btnRegistryBackupNow_Click(object sender, EventArgs e)
		{
			RegistryBackup.BackupNow(BiUpdateHelperSettings.GetManualRegistryBackupLocation() + Path.DirectorySeparatorChar + "BI_REG_" + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".reg");
		}

		private static void btnPerfData_Click(object sender, EventArgs e)
		{
			PerformanceData pd = new PerformanceData();
			pd.ShowDialog();
		}
	}
}
