using BiUpdateHelper.PerformanceData;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BiUpdateHelper
{
	public partial class PerformanceDataForm : Form
	{
		public PerformanceDataForm()
		{
			InitializeComponent();
		}

		private void btnView_Click(object sender, EventArgs e)
		{
			Process.Start("https://biupdatehelper.hopto.org/default.html#stats");
		}

		private void btnUpload_Click(object sender, EventArgs e)
		{
			if (PerformanceDataCollector.CreatePerfDataReport(true, false))
				MessageBox.Show("Started generating performance data report." + Environment.NewLine
					+ "This takes about 10 seconds, then the data will be uploaded." + Environment.NewLine
					+ "If the data is not uploaded, check the log file for errors.");
			else
				MessageBox.Show("A performance data report is already being generated.  Please wait a moment and try again.");
		}

		private void btnSave_Click(object sender, EventArgs e)
		{
			if (PerformanceDataCollector.CreatePerfDataReport(false, true))
				MessageBox.Show("Started generating performance data report." + Environment.NewLine
					+ "This takes about 10 seconds, then the data will be saved to perfdata.json in the current directory." + Environment.NewLine
					+ "If the data is not saved, check the log file for errors.");
			else
				MessageBox.Show("A performance data report is already being generated.  Please wait a moment and try again.");
		}
	}
}
