namespace LandAreaConversion
{
    partial class frmEntPlotKhaitan
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
            this.grpData = new System.Windows.Forms.GroupBox();
            this.lstVwPlKh = new System.Windows.Forms.ListView();
            this.colData = new System.Windows.Forms.ColumnHeader();
            this.grpDelete = new System.Windows.Forms.GroupBox();
            this.cmdClr = new System.Windows.Forms.Button();
            this.cmdDelete = new System.Windows.Forms.Button();
            this.grpSave = new System.Windows.Forms.GroupBox();
            this.button2 = new System.Windows.Forms.Button();
            this.cmdDone = new System.Windows.Forms.Button();
            this.grpData.SuspendLayout();
            this.grpDelete.SuspendLayout();
            this.grpSave.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpData
            // 
            this.grpData.Controls.Add(this.lstVwPlKh);
            this.grpData.Location = new System.Drawing.Point(12, 12);
            this.grpData.Name = "grpData";
            this.grpData.Size = new System.Drawing.Size(205, 195);
            this.grpData.TabIndex = 0;
            this.grpData.TabStop = false;
            this.grpData.Text = "Plot/Khaitan";
            // 
            // lstVwPlKh
            // 
            this.lstVwPlKh.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colData});
            this.lstVwPlKh.GridLines = true;
            this.lstVwPlKh.LabelEdit = true;
            this.lstVwPlKh.Location = new System.Drawing.Point(6, 19);
            this.lstVwPlKh.MultiSelect = false;
            this.lstVwPlKh.Name = "lstVwPlKh";
            this.lstVwPlKh.Size = new System.Drawing.Size(193, 170);
            this.lstVwPlKh.TabIndex = 0;
            this.lstVwPlKh.UseCompatibleStateImageBehavior = false;
            this.lstVwPlKh.View = System.Windows.Forms.View.Details;
            this.lstVwPlKh.AfterLabelEdit += new System.Windows.Forms.LabelEditEventHandler(this.lstVwPlKh_AfterLabelEdit);
            this.lstVwPlKh.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.lstVwPlKh_KeyPress);
            this.lstVwPlKh.BeforeLabelEdit += new System.Windows.Forms.LabelEditEventHandler(this.lstVwPlKh_BeforeLabelEdit);
            this.lstVwPlKh.KeyDown += new System.Windows.Forms.KeyEventHandler(this.lstVwPlKh_KeyDown);
            // 
            // colData
            // 
            this.colData.Text = "Data";
            this.colData.Width = 188;
            // 
            // grpDelete
            // 
            this.grpDelete.Controls.Add(this.cmdClr);
            this.grpDelete.Controls.Add(this.cmdDelete);
            this.grpDelete.Location = new System.Drawing.Point(12, 213);
            this.grpDelete.Name = "grpDelete";
            this.grpDelete.Size = new System.Drawing.Size(90, 42);
            this.grpDelete.TabIndex = 1;
            this.grpDelete.TabStop = false;
            // 
            // cmdClr
            // 
            this.cmdClr.Location = new System.Drawing.Point(45, 15);
            this.cmdClr.Name = "cmdClr";
            this.cmdClr.Size = new System.Drawing.Size(34, 23);
            this.cmdClr.TabIndex = 4;
            this.cmdClr.Text = "&clr";
            this.cmdClr.UseVisualStyleBackColor = true;
            this.cmdClr.Click += new System.EventHandler(this.cmdClr_Click);
            // 
            // cmdDelete
            // 
            this.cmdDelete.Location = new System.Drawing.Point(6, 15);
            this.cmdDelete.Name = "cmdDelete";
            this.cmdDelete.Size = new System.Drawing.Size(33, 23);
            this.cmdDelete.TabIndex = 2;
            this.cmdDelete.TabStop = false;
            this.cmdDelete.Text = "De&l";
            this.cmdDelete.UseVisualStyleBackColor = true;
            this.cmdDelete.Click += new System.EventHandler(this.cmdDelete_Click);
            // 
            // grpSave
            // 
            this.grpSave.Controls.Add(this.button2);
            this.grpSave.Controls.Add(this.cmdDone);
            this.grpSave.Location = new System.Drawing.Point(102, 213);
            this.grpSave.Name = "grpSave";
            this.grpSave.Size = new System.Drawing.Size(115, 43);
            this.grpSave.TabIndex = 2;
            this.grpSave.TabStop = false;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(6, 15);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(52, 23);
            this.button2.TabIndex = 3;
            this.button2.TabStop = false;
            this.button2.Text = "Canc&el";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // cmdDone
            // 
            this.cmdDone.Location = new System.Drawing.Point(64, 15);
            this.cmdDone.Name = "cmdDone";
            this.cmdDone.Size = new System.Drawing.Size(47, 23);
            this.cmdDone.TabIndex = 1;
            this.cmdDone.Text = "&Done!!";
            this.cmdDone.UseVisualStyleBackColor = true;
            this.cmdDone.Click += new System.EventHandler(this.cmdDone_Click);
            // 
            // frmEntPlotKhaitan
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(227, 262);
            this.ControlBox = false;
            this.Controls.Add(this.grpSave);
            this.Controls.Add(this.grpDelete);
            this.Controls.Add(this.grpData);
            this.KeyPreview = true;
            this.Name = "frmEntPlotKhaitan";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Plot Khaitan Helper";
            this.Load += new System.EventHandler(this.frmEntPlotKhaitan_Load);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.frmEntPlotKhaitan_KeyUp);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmEntPlotKhaitan_FormClosing);
            this.grpData.ResumeLayout(false);
            this.grpDelete.ResumeLayout(false);
            this.grpSave.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox grpData;
        private System.Windows.Forms.ListView lstVwPlKh;
        private System.Windows.Forms.GroupBox grpDelete;
        private System.Windows.Forms.Button cmdDelete;
        private System.Windows.Forms.GroupBox grpSave;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button cmdDone;
        private System.Windows.Forms.ColumnHeader colData;
        private System.Windows.Forms.Button cmdClr;
    }
}