namespace BiUpdateHelper
{
	partial class PerformanceData
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PerformanceData));
			this.btnView = new System.Windows.Forms.Button();
			this.btnUpload = new System.Windows.Forms.Button();
			this.btnSave = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// btnView
			// 
			this.btnView.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.btnView.Location = new System.Drawing.Point(12, 98);
			this.btnView.Name = "btnView";
			this.btnView.Size = new System.Drawing.Size(333, 23);
			this.btnView.TabIndex = 0;
			this.btnView.Text = "View Performance Data Online";
			this.btnView.UseVisualStyleBackColor = true;
			this.btnView.Click += new System.EventHandler(this.btnView_Click);
			// 
			// btnUpload
			// 
			this.btnUpload.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.btnUpload.Location = new System.Drawing.Point(12, 127);
			this.btnUpload.Name = "btnUpload";
			this.btnUpload.Size = new System.Drawing.Size(333, 23);
			this.btnUpload.TabIndex = 1;
			this.btnUpload.Text = "Upload Fresh Performance Data Now";
			this.btnUpload.UseVisualStyleBackColor = true;
			this.btnUpload.Click += new System.EventHandler(this.btnUpload_Click);
			// 
			// btnSave
			// 
			this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.btnSave.Location = new System.Drawing.Point(12, 156);
			this.btnSave.Name = "btnSave";
			this.btnSave.Size = new System.Drawing.Size(333, 23);
			this.btnSave.TabIndex = 2;
			this.btnSave.Text = "Save a Local Copy of My Performance Data";
			this.btnSave.UseVisualStyleBackColor = true;
			this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(13, 13);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(332, 82);
			this.label1.TabIndex = 3;
			this.label1.Text = resources.GetString("label1.Text");
			// 
			// PerformanceData
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(357, 192);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.btnSave);
			this.Controls.Add(this.btnUpload);
			this.Controls.Add(this.btnView);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Name = "PerformanceData";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Performance Data";
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button btnView;
		private System.Windows.Forms.Button btnUpload;
		private System.Windows.Forms.Button btnSave;
		private System.Windows.Forms.Label label1;
	}
}