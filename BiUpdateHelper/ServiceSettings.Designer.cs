﻿namespace BiUpdateHelper
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
			this.cb_BI32Win64 = new System.Windows.Forms.CheckBox();
			this.txtRegistryBackupsPath = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.btnRegistryBackupsBrowse = new System.Windows.Forms.Button();
			this.btnUpdateBackupsBrowse = new System.Windows.Forms.Button();
			this.label4 = new System.Windows.Forms.Label();
			this.txtUpdateBackupsPath = new System.Windows.Forms.TextBox();
			this.btnViewRegistryBackups = new System.Windows.Forms.Button();
			this.label2 = new System.Windows.Forms.Label();
			this.btnLaunch32BitRegedit = new System.Windows.Forms.Button();
			this.folderBrowserRegistryBackups = new System.Windows.Forms.FolderBrowserDialog();
			this.folderBrowserUpdateBackups = new System.Windows.Forms.FolderBrowserDialog();
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
			this.cb_killBlueIrisProcessesDuringUpdate.Size = new System.Drawing.Size(205, 17);
			this.cb_killBlueIrisProcessesDuringUpdate.TabIndex = 1;
			this.cb_killBlueIrisProcessesDuringUpdate.Text = "Help Blue Iris close itself as necessary";
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
			this.cb_logVerbose.Location = new System.Drawing.Point(12, 229);
			this.cb_logVerbose.Name = "cb_logVerbose";
			this.cb_logVerbose.Size = new System.Drawing.Size(205, 17);
			this.cb_logVerbose.TabIndex = 10;
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
			this.cb_includeRegistryWithUpdateBackup.Location = new System.Drawing.Point(12, 133);
			this.cb_includeRegistryWithUpdateBackup.Name = "cb_includeRegistryWithUpdateBackup";
			this.cb_includeRegistryWithUpdateBackup.Size = new System.Drawing.Size(195, 17);
			this.cb_includeRegistryWithUpdateBackup.TabIndex = 5;
			this.cb_includeRegistryWithUpdateBackup.Text = "Backup registry before each update";
			this.toolTip1.SetToolTip(this.cb_includeRegistryWithUpdateBackup, resources.GetString("cb_includeRegistryWithUpdateBackup.ToolTip"));
			this.cb_includeRegistryWithUpdateBackup.UseVisualStyleBackColor = true;
			this.cb_includeRegistryWithUpdateBackup.CheckedChanged += new System.EventHandler(this.cb_CheckedChanged);
			// 
			// cb_dailyRegistryBackups
			// 
			this.cb_dailyRegistryBackups.AutoSize = true;
			this.cb_dailyRegistryBackups.Location = new System.Drawing.Point(12, 156);
			this.cb_dailyRegistryBackups.Name = "cb_dailyRegistryBackups";
			this.cb_dailyRegistryBackups.Size = new System.Drawing.Size(124, 17);
			this.cb_dailyRegistryBackups.TabIndex = 6;
			this.cb_dailyRegistryBackups.Text = "Daily registry backup";
			this.toolTip1.SetToolTip(this.cb_dailyRegistryBackups, "(Default: enabled)\r\n\r\nIf enabled, Blue Iris\'s registry settings will be backed up" +
        " \r\neach day and stored alongside BiUpdateHelper.");
			this.cb_dailyRegistryBackups.UseVisualStyleBackColor = true;
			this.cb_dailyRegistryBackups.CheckedChanged += new System.EventHandler(this.cb_CheckedChanged);
			// 
			// cb_BI32Win64
			// 
			this.cb_BI32Win64.AutoSize = true;
			this.cb_BI32Win64.Location = new System.Drawing.Point(12, 252);
			this.cb_BI32Win64.Name = "cb_BI32Win64";
			this.cb_BI32Win64.Size = new System.Drawing.Size(183, 17);
			this.cb_BI32Win64.TabIndex = 11;
			this.cb_BI32Win64.Text = "32 bit Blue Iris on 64 bit Windows";
			this.toolTip1.SetToolTip(this.cb_BI32Win64, "You should only need to check this box if you want this \r\nprogram to work with 32" +
        " bit Blue Iris on 64 bit Windows.");
			this.cb_BI32Win64.UseVisualStyleBackColor = true;
			this.cb_BI32Win64.CheckedChanged += new System.EventHandler(this.cb_CheckedChanged);
			// 
			// txtRegistryBackupsPath
			// 
			this.txtRegistryBackupsPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.txtRegistryBackupsPath.Location = new System.Drawing.Point(12, 203);
			this.txtRegistryBackupsPath.Name = "txtRegistryBackupsPath";
			this.txtRegistryBackupsPath.Size = new System.Drawing.Size(275, 20);
			this.txtRegistryBackupsPath.TabIndex = 8;
			this.toolTip1.SetToolTip(this.txtRegistryBackupsPath, "The RegistryBackups folder will be \r\ncreated as a subdirectory of this.\r\n\r\nIf emp" +
        "ty or otherwise invalid, then \r\nthe RegistryBackups folder will be \r\nlocated nex" +
        "t to the BiUpdateHelper\r\nexecutable.");
			this.txtRegistryBackupsPath.TextChanged += new System.EventHandler(this.txtRegistryBackupsPath_TextChanged);
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.BackColor = System.Drawing.Color.Transparent;
			this.label3.Location = new System.Drawing.Point(12, 184);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(152, 13);
			this.label3.TabIndex = 11;
			this.label3.Text = "Custom RegistryBackups path:";
			this.toolTip1.SetToolTip(this.label3, "The RegistryBackups folder will be \r\ncreated as a subdirectory of this.\r\n\r\nIf emp" +
        "ty or otherwise invalid, then \r\nthe RegistryBackups folder will be \r\nlocated nex" +
        "t to the BiUpdateHelper\r\nexecutable.");
			// 
			// btnRegistryBackupsBrowse
			// 
			this.btnRegistryBackupsBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnRegistryBackupsBrowse.Location = new System.Drawing.Point(293, 201);
			this.btnRegistryBackupsBrowse.Name = "btnRegistryBackupsBrowse";
			this.btnRegistryBackupsBrowse.Size = new System.Drawing.Size(75, 23);
			this.btnRegistryBackupsBrowse.TabIndex = 9;
			this.btnRegistryBackupsBrowse.Text = "browse";
			this.toolTip1.SetToolTip(this.btnRegistryBackupsBrowse, "The RegistryBackups folder will be \r\ncreated as a subdirectory of this.\r\n\r\nIf emp" +
        "ty or otherwise invalid, then \r\nthe RegistryBackups folder will be \r\nlocated nex" +
        "t to the BiUpdateHelper\r\nexecutable.");
			this.btnRegistryBackupsBrowse.UseVisualStyleBackColor = true;
			this.btnRegistryBackupsBrowse.Click += new System.EventHandler(this.btnRegistryBackupsBrowse_Click);
			// 
			// btnUpdateBackupsBrowse
			// 
			this.btnUpdateBackupsBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnUpdateBackupsBrowse.Location = new System.Drawing.Point(293, 103);
			this.btnUpdateBackupsBrowse.Name = "btnUpdateBackupsBrowse";
			this.btnUpdateBackupsBrowse.Size = new System.Drawing.Size(75, 23);
			this.btnUpdateBackupsBrowse.TabIndex = 4;
			this.btnUpdateBackupsBrowse.Text = "browse";
			this.toolTip1.SetToolTip(this.btnUpdateBackupsBrowse, "All Blue Iris update files will be \r\nsaved into this folder.\r\n\r\nIf empty or other" +
        "wise invalid, then \r\nthe update files will be saved in the \r\nsame folder as Blue" +
        "Iris.exe.");
			this.btnUpdateBackupsBrowse.UseVisualStyleBackColor = true;
			this.btnUpdateBackupsBrowse.Click += new System.EventHandler(this.btnUpdateBackupsBrowse_Click);
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.BackColor = System.Drawing.Color.Transparent;
			this.label4.Location = new System.Drawing.Point(12, 86);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(149, 13);
			this.label4.TabIndex = 14;
			this.label4.Text = "Custom update backups path:\r\n";
			this.toolTip1.SetToolTip(this.label4, "All Blue Iris update files will be \r\nsaved into this folder.\r\n\r\nIf empty or other" +
        "wise invalid, then \r\nthe update files will be saved in the \r\nsame folder as Blue" +
        "Iris.exe.");
			// 
			// txtUpdateBackupsPath
			// 
			this.txtUpdateBackupsPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.txtUpdateBackupsPath.Location = new System.Drawing.Point(12, 105);
			this.txtUpdateBackupsPath.Name = "txtUpdateBackupsPath";
			this.txtUpdateBackupsPath.Size = new System.Drawing.Size(275, 20);
			this.txtUpdateBackupsPath.TabIndex = 3;
			this.toolTip1.SetToolTip(this.txtUpdateBackupsPath, "All Blue Iris update files will be \r\nsaved into this folder.\r\n\r\nIf empty or other" +
        "wise invalid, then \r\nthe update files will be saved in the \r\nsame folder as Blue" +
        "Iris.exe.");
			this.txtUpdateBackupsPath.TextChanged += new System.EventHandler(this.txtUpdateBackupsPath_TextChanged);
			// 
			// btnViewRegistryBackups
			// 
			this.btnViewRegistryBackups.Location = new System.Drawing.Point(174, 152);
			this.btnViewRegistryBackups.Name = "btnViewRegistryBackups";
			this.btnViewRegistryBackups.Size = new System.Drawing.Size(194, 23);
			this.btnViewRegistryBackups.TabIndex = 7;
			this.btnViewRegistryBackups.Text = "View Registry Backups";
			this.btnViewRegistryBackups.UseVisualStyleBackColor = true;
			this.btnViewRegistryBackups.Click += new System.EventHandler(this.btnViewRegistryBackups_Click);
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(12, 272);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(256, 57);
			this.label2.TabIndex = 8;
			this.label2.Text = "Note: To restore registry backups of 32 bit Blue Iris taken on 64 bit Windows, us" +
    "e the 32 bit regedit program.";
			// 
			// btnLaunch32BitRegedit
			// 
			this.btnLaunch32BitRegedit.Location = new System.Drawing.Point(274, 272);
			this.btnLaunch32BitRegedit.Name = "btnLaunch32BitRegedit";
			this.btnLaunch32BitRegedit.Size = new System.Drawing.Size(94, 41);
			this.btnLaunch32BitRegedit.TabIndex = 12;
			this.btnLaunch32BitRegedit.Text = "Launch 32 bit regedit";
			this.btnLaunch32BitRegedit.UseVisualStyleBackColor = true;
			this.btnLaunch32BitRegedit.Click += new System.EventHandler(this.btnLaunch32BitRegedit_Click);
			// 
			// ServiceSettings
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(380, 330);
			this.Controls.Add(this.btnUpdateBackupsBrowse);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.txtUpdateBackupsPath);
			this.Controls.Add(this.btnRegistryBackupsBrowse);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.txtRegistryBackupsPath);
			this.Controls.Add(this.btnLaunch32BitRegedit);
			this.Controls.Add(this.label2);
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
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Button btnLaunch32BitRegedit;
		private System.Windows.Forms.TextBox txtRegistryBackupsPath;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Button btnRegistryBackupsBrowse;
		private System.Windows.Forms.FolderBrowserDialog folderBrowserRegistryBackups;
		private System.Windows.Forms.Button btnUpdateBackupsBrowse;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TextBox txtUpdateBackupsPath;
		private System.Windows.Forms.FolderBrowserDialog folderBrowserUpdateBackups;
	}
}