namespace BiUpdateHelper
{
	partial class ServiceSettings
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ServiceSettings));
			this.label1 = new System.Windows.Forms.Label();
			this.cb_killBlueIrisProcessesDuringUpdate = new System.Windows.Forms.CheckBox();
			this.cb_backupUpdateFiles = new System.Windows.Forms.CheckBox();
			this.cb_logVerbose = new System.Windows.Forms.CheckBox();
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			this.cb_includeRegistryWithUpdateBackup = new System.Windows.Forms.CheckBox();
			this.cb_dailyRegistryBackups = new System.Windows.Forms.CheckBox();
			this.btnViewRegistryBackups = new System.Windows.Forms.Button();
			this.cb_BI32Win64 = new System.Windows.Forms.CheckBox();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(12, 9);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(252, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "Changes take effect the next time the service starts.";
			// 
			// cb_killBlueIrisProcessesDuringUpdate
			// 
			this.cb_killBlueIrisProcessesDuringUpdate.AutoSize = true;
			this.cb_killBlueIrisProcessesDuringUpdate.Location = new System.Drawing.Point(12, 39);
			this.cb_killBlueIrisProcessesDuringUpdate.Name = "cb_killBlueIrisProcessesDuringUpdate";
			this.cb_killBlueIrisProcessesDuringUpdate.Size = new System.Drawing.Size(198, 17);
			this.cb_killBlueIrisProcessesDuringUpdate.TabIndex = 1;
			this.cb_killBlueIrisProcessesDuringUpdate.Text = "Kill Blue Iris processes during update";
			this.toolTip1.SetToolTip(this.cb_killBlueIrisProcessesDuringUpdate, resources.GetString("cb_killBlueIrisProcessesDuringUpdate.ToolTip"));
			this.cb_killBlueIrisProcessesDuringUpdate.UseVisualStyleBackColor = true;
			this.cb_killBlueIrisProcessesDuringUpdate.CheckedChanged += new System.EventHandler(this.cb_CheckedChanged);
			// 
			// cb_backupUpdateFiles
			// 
			this.cb_backupUpdateFiles.AutoSize = true;
			this.cb_backupUpdateFiles.Location = new System.Drawing.Point(12, 62);
			this.cb_backupUpdateFiles.Name = "cb_backupUpdateFiles";
			this.cb_backupUpdateFiles.Size = new System.Drawing.Size(120, 17);
			this.cb_backupUpdateFiles.TabIndex = 2;
			this.cb_backupUpdateFiles.Text = "Backup update files";
			this.toolTip1.SetToolTip(this.cb_backupUpdateFiles, resources.GetString("cb_backupUpdateFiles.ToolTip"));
			this.cb_backupUpdateFiles.UseVisualStyleBackColor = true;
			this.cb_backupUpdateFiles.CheckedChanged += new System.EventHandler(this.cb_CheckedChanged);
			// 
			// cb_logVerbose
			// 
			this.cb_logVerbose.AutoSize = true;
			this.cb_logVerbose.Location = new System.Drawing.Point(12, 131);
			this.cb_logVerbose.Name = "cb_logVerbose";
			this.cb_logVerbose.Size = new System.Drawing.Size(205, 17);
			this.cb_logVerbose.TabIndex = 6;
			this.cb_logVerbose.Text = "Log verbose (for debugging purposes)";
			this.toolTip1.SetToolTip(this.cb_logVerbose, resources.GetString("cb_logVerbose.ToolTip"));
			this.cb_logVerbose.UseVisualStyleBackColor = true;
			this.cb_logVerbose.CheckedChanged += new System.EventHandler(this.cb_CheckedChanged);
			// 
			// toolTip1
			// 
			this.toolTip1.AutoPopDelay = 30000;
			this.toolTip1.InitialDelay = 500;
			this.toolTip1.IsBalloon = true;
			this.toolTip1.ReshowDelay = 100;
			// 
			// cb_includeRegistryWithUpdateBackup
			// 
			this.cb_includeRegistryWithUpdateBackup.AutoSize = true;
			this.cb_includeRegistryWithUpdateBackup.Location = new System.Drawing.Point(12, 85);
			this.cb_includeRegistryWithUpdateBackup.Name = "cb_includeRegistryWithUpdateBackup";
			this.cb_includeRegistryWithUpdateBackup.Size = new System.Drawing.Size(195, 17);
			this.cb_includeRegistryWithUpdateBackup.TabIndex = 3;
			this.cb_includeRegistryWithUpdateBackup.Text = "Backup registry before each update";
			this.toolTip1.SetToolTip(this.cb_includeRegistryWithUpdateBackup, resources.GetString("cb_includeRegistryWithUpdateBackup.ToolTip"));
			this.cb_includeRegistryWithUpdateBackup.UseVisualStyleBackColor = true;
			this.cb_includeRegistryWithUpdateBackup.CheckedChanged += new System.EventHandler(this.cb_CheckedChanged);
			// 
			// cb_dailyRegistryBackups
			// 
			this.cb_dailyRegistryBackups.AutoSize = true;
			this.cb_dailyRegistryBackups.Location = new System.Drawing.Point(12, 108);
			this.cb_dailyRegistryBackups.Name = "cb_dailyRegistryBackups";
			this.cb_dailyRegistryBackups.Size = new System.Drawing.Size(124, 17);
			this.cb_dailyRegistryBackups.TabIndex = 4;
			this.cb_dailyRegistryBackups.Text = "Daily registry backup";
			this.toolTip1.SetToolTip(this.cb_dailyRegistryBackups, "(Default: enabled)\r\n\r\nIf enabled, Blue Iris\'s registry settings will be backed up" +
        " \r\neach day and stored alongside BiUpdateHelper.");
			this.cb_dailyRegistryBackups.UseVisualStyleBackColor = true;
			this.cb_dailyRegistryBackups.CheckedChanged += new System.EventHandler(this.cb_CheckedChanged);
			// 
			// btnViewRegistryBackups
			// 
			this.btnViewRegistryBackups.Location = new System.Drawing.Point(174, 104);
			this.btnViewRegistryBackups.Name = "btnViewRegistryBackups";
			this.btnViewRegistryBackups.Size = new System.Drawing.Size(194, 23);
			this.btnViewRegistryBackups.TabIndex = 5;
			this.btnViewRegistryBackups.Text = "View Registry Backups";
			this.btnViewRegistryBackups.UseVisualStyleBackColor = true;
			this.btnViewRegistryBackups.Click += new System.EventHandler(this.btnViewRegistryBackups_Click);
			// 
			// cb_BI32Win64
			// 
			this.cb_BI32Win64.AutoSize = true;
			this.cb_BI32Win64.Location = new System.Drawing.Point(12, 154);
			this.cb_BI32Win64.Name = "cb_BI32Win64";
			this.cb_BI32Win64.Size = new System.Drawing.Size(183, 17);
			this.cb_BI32Win64.TabIndex = 7;
			this.cb_BI32Win64.Text = "32 bit Blue Iris on 64 bit Windows";
			this.toolTip1.SetToolTip(this.cb_BI32Win64, "You should only need to check this box if you want this \r\nprogram to work with 32" +
        " bit Blue Iris on 64 bit Windows.");
			this.cb_BI32Win64.UseVisualStyleBackColor = true;
			this.cb_BI32Win64.CheckedChanged += new System.EventHandler(this.cb_CheckedChanged);
			// 
			// ServiceSettings
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(380, 178);
			this.Controls.Add(this.cb_BI32Win64);
			this.Controls.Add(this.btnViewRegistryBackups);
			this.Controls.Add(this.cb_dailyRegistryBackups);
			this.Controls.Add(this.cb_includeRegistryWithUpdateBackup);
			this.Controls.Add(this.cb_logVerbose);
			this.Controls.Add(this.cb_backupUpdateFiles);
			this.Controls.Add(this.cb_killBlueIrisProcessesDuringUpdate);
			this.Controls.Add(this.label1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Name = "ServiceSettings";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "BiUpdateHelper Settings";
			this.Load += new System.EventHandler(this.ServiceSettings_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.CheckBox cb_killBlueIrisProcessesDuringUpdate;
		private System.Windows.Forms.CheckBox cb_backupUpdateFiles;
		private System.Windows.Forms.CheckBox cb_logVerbose;
		private System.Windows.Forms.ToolTip toolTip1;
		private System.Windows.Forms.CheckBox cb_includeRegistryWithUpdateBackup;
		private System.Windows.Forms.CheckBox cb_dailyRegistryBackups;
		private System.Windows.Forms.Button btnViewRegistryBackups;
		private System.Windows.Forms.CheckBox cb_BI32Win64;
	}
}