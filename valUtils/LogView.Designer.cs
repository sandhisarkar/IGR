/*
 * Created by SharpDevelop.
 * User: USER
 * Date: 28/03/2017
 * Time: 9:42 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
namespace valUtils
{
	partial class LogView
	{
		/// <summary>
		/// Designer variable used to keep track of non-visual components.
		/// </summary>
		private System.ComponentModel.IContainer components = null;
		
		/// <summary>
		/// Disposes resources used by the form.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing) {
				if (components != null) {
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}
		
		/// <summary>
		/// This method is required for Windows Forms designer support.
		/// Do not change the method contents inside the source code editor. The Forms designer might
		/// not be able to load this method if it was changed manually.
		/// </summary>
		private void InitializeComponent()
		{
			this.grpFilter = new System.Windows.Forms.GroupBox();
			this.grpData = new System.Windows.Forms.GroupBox();
			this.lvwData = new System.Windows.Forms.ListView();
			this.grpFunctions = new System.Windows.Forms.GroupBox();
			this.cmdClose = new System.Windows.Forms.Button();
			this.grpData.SuspendLayout();
			this.grpFunctions.SuspendLayout();
			this.SuspendLayout();
			// 
			// grpFilter
			// 
			this.grpFilter.Location = new System.Drawing.Point(12, 12);
			this.grpFilter.Name = "grpFilter";
			this.grpFilter.Size = new System.Drawing.Size(337, 58);
			this.grpFilter.TabIndex = 0;
			this.grpFilter.TabStop = false;
			this.grpFilter.Text = "Filters";
			// 
			// grpData
			// 
			this.grpData.Controls.Add(this.lvwData);
			this.grpData.Location = new System.Drawing.Point(12, 76);
			this.grpData.Name = "grpData";
			this.grpData.Size = new System.Drawing.Size(337, 218);
			this.grpData.TabIndex = 1;
			this.grpData.TabStop = false;
			this.grpData.Text = "Log Data";
			// 
			// lvwData
			// 
			this.lvwData.FullRowSelect = true;
			this.lvwData.Location = new System.Drawing.Point(6, 19);
			this.lvwData.Name = "lvwData";
			this.lvwData.Size = new System.Drawing.Size(325, 193);
			this.lvwData.Sorting = System.Windows.Forms.SortOrder.Ascending;
			this.lvwData.TabIndex = 0;
			this.lvwData.UseCompatibleStateImageBehavior = false;
			this.lvwData.View = System.Windows.Forms.View.Details;
			// 
			// grpFunctions
			// 
			this.grpFunctions.Controls.Add(this.cmdClose);
			this.grpFunctions.Location = new System.Drawing.Point(12, 300);
			this.grpFunctions.Name = "grpFunctions";
			this.grpFunctions.Size = new System.Drawing.Size(337, 43);
			this.grpFunctions.TabIndex = 2;
			this.grpFunctions.TabStop = false;
			// 
			// cmdClose
			// 
			this.cmdClose.Location = new System.Drawing.Point(245, 14);
			this.cmdClose.Name = "cmdClose";
			this.cmdClose.Size = new System.Drawing.Size(75, 23);
			this.cmdClose.TabIndex = 0;
			this.cmdClose.Text = "&Close";
			this.cmdClose.UseVisualStyleBackColor = true;
			this.cmdClose.Click += new System.EventHandler(this.CmdCloseClick);
			// 
			// LogView
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(361, 355);
			this.Controls.Add(this.grpFunctions);
			this.Controls.Add(this.grpData);
			this.Controls.Add(this.grpFilter);
			this.KeyPreview = true;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "LogView";
			this.Text = "Log Viewer";
			this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.LogViewKeyUp);
			this.grpData.ResumeLayout(false);
			this.grpFunctions.ResumeLayout(false);
			this.ResumeLayout(false);
		}
		private System.Windows.Forms.ListView lvwData;
		private System.Windows.Forms.Button cmdClose;
		private System.Windows.Forms.GroupBox grpFunctions;
		private System.Windows.Forms.GroupBox grpData;
		private System.Windows.Forms.GroupBox grpFilter;
	}
}
