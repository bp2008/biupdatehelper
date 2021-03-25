using BPUtil;
using BlueIrisRegistryReader;
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
			cb_killBlueIrisProcessesDuringUpdate.Checked = settings.killBlueIrisProcessesDuringUpdate2;
			cb_backupUpdateFiles.Checked = settings.backupUpdateFiles;
			cb_includeRegistryWithUpdateBackup.Checked = settings.includeRegistryWithUpdateBackup;
			cb_dailyRegistryBackups.Checked = settings.dailyRegistryBackups;
			cb_logVerbose.Checked = settings.logVerbose;
			cb_BI32Win64.Checked = settings.bi32OnWin64;
			txtRegistryBackupsPath.Text = settings.registryBackupsFolderPath;
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
				settings.killBlueIrisProcessesDuringUpdate2 = cb_killBlueIrisProcessesDuringUpdate.Checked;
				settings.backupUpdateFiles = cb_backupUpdateFiles.Checked;
				settings.includeRegistryWithUpdateBackup = cb_includeRegistryWithUpdateBackup.Checked;
				settings.dailyRegistryBackups = cb_dailyRegistryBackups.Checked;
				settings.logVerbose = cb_logVerbose.Checked;
				settings.bi32OnWin64 = cb_BI32Win64.Checked;
				settings.registryBackupsFolderPath = txtRegistryBackupsPath.Text;
				RegistryUtil.Force32BitRegistryAccess = settings.bi32OnWin64;

				settings.Save();
			}
		}

		private void txtRegistryBackupsPath_TextChanged(object sender, EventArgs e)
		{
			SaveSettings();
		}


		private void btnViewRegistryBackups_Click(object sender, EventArgs e)
		{
			Process.Start(Program.settings.GetRegistryBackupLocation());
		}

		private void btnLaunch32BitRegedit_Click(object sender, EventArgs e)
		{
			Process.Start(RegistryBackup.GetRegeditPath(true));
		}

		private void btnRegistryBackupsBrowse_Click(object sender, EventArgs e)
		{
			if (!string.IsNullOrEmpty(txtRegistryBackupsPath.Text))
				folderBrowserRegistryBackups.SelectedPath = txtRegistryBackupsPath.Text;
			DialogResult dr = folderBrowserRegistryBackups.ShowDialog();
			if (dr == DialogResult.OK)
				txtRegistryBackupsPath.Text = folderBrowserRegistryBackups.SelectedPath;
		}
	}
}
