using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BiUpdateHelper
{
	public partial class ServiceSettings : Form
	{
		BiUpdateHelperSettings settings = Program.settings;
		volatile bool isLoaded = false;

		public ServiceSettings()
		{
			InitializeComponent();
		}

		private void ServiceSettings_Load(object sender, EventArgs e)
		{
			isLoaded = false;
			settings.Load();
			cb_killBlueIrisProcessesDuringUpdate.Checked = settings.killBlueIrisProcessesDuringUpdate;
			cb_backupUpdateFiles.Checked = settings.backupUpdateFiles;
			cb_includeRegistryWithUpdateBackup.Checked = settings.includeRegistryWithUpdateBackup;
			cb_dailyRegistryBackups.Checked = settings.dailyRegistryBackups;
			cb_logVerbose.Checked = settings.logVerbose;
			cb_BI32Win64.Checked = settings.bi32OnWin64;
			isLoaded = true;
		}

		private void cb_CheckedChanged(object sender, EventArgs e)
		{
			SaveSettings();
		}

		private void SaveSettings()
		{
			if (isLoaded)
			{
				settings.killBlueIrisProcessesDuringUpdate = cb_killBlueIrisProcessesDuringUpdate.Checked;
				settings.backupUpdateFiles = cb_backupUpdateFiles.Checked;
				settings.includeRegistryWithUpdateBackup = cb_includeRegistryWithUpdateBackup.Checked;
				settings.dailyRegistryBackups = cb_dailyRegistryBackups.Checked;
				settings.logVerbose = cb_logVerbose.Checked;
				settings.bi32OnWin64 = cb_BI32Win64.Checked;
				RegistryUtil.Force32BitRegistryAccess = settings.bi32OnWin64;

				settings.Save();
			}
		}


		private void btnViewRegistryBackups_Click(object sender, EventArgs e)
		{
			Process.Start(BiUpdateHelperSettings.GetRegistryBackupLocation());
		}

		private void btnLaunch32BitRegedit_Click(object sender, EventArgs e)
		{
			Process.Start(RegistryBackup.GetRegeditPath(true));
		}
	}
}
