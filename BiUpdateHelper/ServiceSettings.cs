using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BiUpdateHelper
{
	public partial class ServiceSettings : Form
	{
		public ServiceSettings()
		{
			InitializeComponent();
		}

		private void ServiceSettings_Load(object sender, EventArgs e)
		{
			BiUpdateHelperSettings settings = new BiUpdateHelperSettings();
			settings.Load();

			cb_killBlueIrisProcessesDuringUpdate.Checked = settings.killBlueIrisProcessesDuringUpdate;
			cb_backupUpdateFiles.Checked = settings.backupUpdateFiles;
			cb_logVerbose.Checked = settings.logVerbose;
		}

		private void cb_killBlueIrisProcessesDuringUpdate_CheckedChanged(object sender, EventArgs e)
		{
			SaveSettings();
		}

		private void cb_backupUpdateFiles_CheckedChanged(object sender, EventArgs e)
		{
			SaveSettings();
		}

		private void cb_logVerbose_CheckedChanged(object sender, EventArgs e)
		{
			SaveSettings();
		}

		private void SaveSettings()
		{
			BiUpdateHelperSettings settings = new BiUpdateHelperSettings();
			settings.Load();

			settings.killBlueIrisProcessesDuringUpdate = cb_killBlueIrisProcessesDuringUpdate.Checked;
			settings.backupUpdateFiles = cb_backupUpdateFiles.Checked;
			settings.logVerbose = cb_logVerbose.Checked;

			settings.Save();
		}
	}
}
