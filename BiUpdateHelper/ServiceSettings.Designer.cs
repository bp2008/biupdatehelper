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
			this.label1 = new System.Windows.Forms.Label();
			this.cb_killBlueIrisProcessesDuringUpdate = new System.Windows.Forms.CheckBox();
			this.cb_backupUpdateFiles = new System.Windows.Forms.CheckBox();
			this.cb_logVerbose = new System.Windows.Forms.CheckBox();
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
			this.cb_killBlueIrisProcessesDuringUpdate.Size = new System.Drawing.Size(203, 17);
			this.cb_killBlueIrisProcessesDuringUpdate.TabIndex = 1;
			this.cb_killBlueIrisProcessesDuringUpdate.Text = "Kill Blue Iris Processes During Update";
			this.cb_killBlueIrisProcessesDuringUpdate.UseVisualStyleBackColor = true;
			this.cb_killBlueIrisProcessesDuringUpdate.CheckedChanged += new System.EventHandler(this.cb_killBlueIrisProcessesDuringUpdate_CheckedChanged);
			// 
			// cb_backupUpdateFiles
			// 
			this.cb_backupUpdateFiles.AutoSize = true;
			this.cb_backupUpdateFiles.Location = new System.Drawing.Point(12, 62);
			this.cb_backupUpdateFiles.Name = "cb_backupUpdateFiles";
			this.cb_backupUpdateFiles.Size = new System.Drawing.Size(125, 17);
			this.cb_backupUpdateFiles.TabIndex = 2;
			this.cb_backupUpdateFiles.Text = "Backup Update Files";
			this.cb_backupUpdateFiles.UseVisualStyleBackColor = true;
			this.cb_backupUpdateFiles.CheckedChanged += new System.EventHandler(this.cb_backupUpdateFiles_CheckedChanged);
			// 
			// cb_logVerbose
			// 
			this.cb_logVerbose.AutoSize = true;
			this.cb_logVerbose.Location = new System.Drawing.Point(12, 85);
			this.cb_logVerbose.Name = "cb_logVerbose";
			this.cb_logVerbose.Size = new System.Drawing.Size(206, 17);
			this.cb_logVerbose.TabIndex = 3;
			this.cb_logVerbose.Text = "Log Verbose (for debugging purposes)";
			this.cb_logVerbose.UseVisualStyleBackColor = true;
			this.cb_logVerbose.CheckedChanged += new System.EventHandler(this.cb_logVerbose_CheckedChanged);
			// 
			// ServiceSettings
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(380, 121);
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
	}
}