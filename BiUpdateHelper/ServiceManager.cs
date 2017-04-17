using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Timer = System.Windows.Forms.Timer;

namespace BiUpdateHelper
{
	public partial class ServiceManager : Form
	{
		Timer timer;
		BackgroundWorker bw = null;
		string statusStr = "";
		Thread thrPreloadSettings;
		public ServiceManager()
		{
			thrPreloadSettings = new Thread(() =>
			{
			});
			thrPreloadSettings.IsBackground = true;
			thrPreloadSettings.Start();
			Application.EnableVisualStyles();
			InitializeComponent();
		}

		private void ServiceManager_Load(object sender, EventArgs e)
		{
			UpdateStatus();
			timer = new Timer();
			timer.Tick += Timer_Tick;
			timer.Interval = 1000;
			timer.Start();
		}

		private void Timer_Tick(object sender, EventArgs e)
		{
			UpdateStatus();
		}

		private void btnInstall_Click(object sender, EventArgs e)
		{
			if (btnInstall.Tag is string)
			{
				if ((string)btnInstall.Tag == "INSTALL")
				{
					DoInBackground((string)btnInstall.Tag);
				}
				else if ((string)btnInstall.Tag == "UNINSTALL")
				{
					DoInBackground((string)btnInstall.Tag);
				}
			}
		}

		private void btnStart_Click(object sender, EventArgs e)
		{
			if (btnStart.Tag is string)
			{
				if ((string)btnStart.Tag == "START")
				{
					DoInBackground((string)btnStart.Tag);
				}
				else if ((string)btnStart.Tag == "STOP")
				{
					DoInBackground((string)btnStart.Tag);
				}
			}
		}

		private void DoInBackground(string tag)
		{
			if (bw != null)
				return;
			statusStr = "";
			progressBar.Visible = true;
			bw = new BackgroundWorker();
			bw.DoWork += Bw_DoWork;
			bw.RunWorkerCompleted += Bw_RunWorkerCompleted;
			bw.WorkerSupportsCancellation = true;
			bw.RunWorkerAsync(tag);
			UpdateStatus();
		}

		private void Bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			bw = null;
			progressBar.Visible = false;
			UpdateStatus();
		}

		private void Bw_DoWork(object sender, DoWorkEventArgs e)
		{
			try
			{
				string command = (string)e.Argument;
				if (command == "INSTALL")
				{
					InstallService();
				}
				else if (command == "UNINSTALL")
				{
					UninstallService();
				}
				else if (command == "START")
				{
					StartService();
				}
				else if (command == "STOP")
				{
					StopService();
				}
			}
			catch (Exception ex)
			{
				statusStr = ex.ToString();
			}
		}

		private void UpdateStatus()
		{
			ServiceController service = ServiceController.GetServices().FirstOrDefault(sc => sc.ServiceName == "BiUpdateHelper");
			if (service == null)
			{
				lblService.Text = "Service Status: Not installed";
				btnInstall.Text = "Install Service";
				btnInstall.Tag = "INSTALL";
				btnInstall.Enabled = (bw == null);
				btnStart.Text = "Start Service";
				btnStart.Tag = "";
				btnStart.Enabled = false;
			}
			else
			{
				btnInstall.Text = "Uninstall Service";
				btnInstall.Tag = "UNINSTALL";
				ServiceControllerStatus status = service.Status;
				lblService.Text = "Service Status: " + status.ToString();
				if (status == ServiceControllerStatus.Running)
				{
					btnStart.Text = "Stop Service";
					btnStart.Tag = "STOP";
					btnStart.Enabled = (bw == null);
					btnInstall.Enabled = false;
				}
				else if (status == ServiceControllerStatus.Stopped)
				{
					btnStart.Text = "Start Service";
					btnStart.Tag = "START";
					btnStart.Enabled = (bw == null);
					btnInstall.Enabled = (bw == null);
				}
				else
				{
					btnStart.Text = "Start Service";
					btnStart.Tag = "";
					btnStart.Enabled = false;
					btnInstall.Enabled = false;
				}
			}
			if (txtStatus.Text != statusStr)
				txtStatus.Text = statusStr;
		}

		private void ServiceManager_FormClosing(object sender, FormClosingEventArgs e)
		{
			timer?.Stop();
		}

		private void InstallService()
		{
			// sc create BiUpdateHelper binPath= "%~dp0BiUpdateHelper.exe" start= auto
			// sc failure BiUpdateHelper reset= 0 actions= restart/60000/restart/60000/restart/60000
			string std, err;
			RunProcessAndWait("sc", "create BiUpdateHelper binPath= \"" + Application.ExecutablePath + "\" start= auto", out std, out err);
			statusStr = std + Environment.NewLine + err;
			RunProcessAndWait("sc", "failure BiUpdateHelper reset= 0 actions= restart/60000/restart/60000/restart/60000", out std, out err);
			statusStr = std + Environment.NewLine + err;
		}

		private void UninstallService()
		{
			// sc delete BiUpdateHelper
			string std, err;
			RunProcessAndWait("sc", "delete BiUpdateHelper", out std, out err);
			statusStr = std + Environment.NewLine + err;
		}

		private void StartService()
		{
			// NET START BiUpdateHelper
			string std, err;
			RunProcessAndWait("NET", "START BiUpdateHelper", out std, out err);
			statusStr = std + Environment.NewLine + err;
		}

		private void StopService()
		{
			// NET STOP BiUpdateHelper
			string std, err;
			RunProcessAndWait("NET", "STOP BiUpdateHelper", out std, out err);
			statusStr = std + Environment.NewLine + err;
		}

		private int RunProcessAndWait(string fileName, string arguments, out string output, out string errorOutput)
		{
			ProcessStartInfo psi = new ProcessStartInfo(fileName, arguments);
			psi.UseShellExecute = false;
			psi.CreateNoWindow = true;
			psi.RedirectStandardOutput = true;
			psi.RedirectStandardError = true;

			StringBuilder sbOutput = new StringBuilder();
			StringBuilder sbError = new StringBuilder();

			Process p = Process.Start(psi);

			p.OutputDataReceived += (sender, e) =>
			{
				sbOutput.Append(e.Data);
			};
			p.ErrorDataReceived += (sender, e) =>
			{
				sbError.Append(e.Data);
			};
			p.BeginOutputReadLine();
			p.BeginErrorReadLine();

			p.WaitForExit();

			output = sbOutput.ToString();
			errorOutput = sbError.ToString();

			return p.ExitCode;
		}

		private void btnSettings_Click(object sender, EventArgs e)
		{
			ServiceSettings settingsDialog = new ServiceSettings();
			settingsDialog.ShowDialog();
		}
	}
}
