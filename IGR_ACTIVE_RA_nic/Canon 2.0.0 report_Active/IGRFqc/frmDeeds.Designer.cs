namespace IGRFqc
{
    partial class frmDeeds
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmDeeds));
            this.panelForm = new System.Windows.Forms.Panel();
            this.lblHeader = new nControls.deLabel();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.lstDeeds = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.cmdSearch = new nControls.deButton();
            this.txtdeedSearch = new nControls.deTextBox();
            this.label1 = new nControls.deLabel();
            this.lblVol = new nControls.deLabel();
            this.lblBook = new nControls.deLabel();
            this.lblDeedYr = new nControls.deLabel();
            this.label4 = new nControls.deLabel();
            this.label3 = new nControls.deLabel();
            this.label2 = new nControls.deLabel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cmsDeeds = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.updateDeedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteDeedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.groupBox4.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.cmsDeeds.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelForm
            // 
            this.panelForm.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelForm.Location = new System.Drawing.Point(3, 36);
            this.panelForm.Name = "panelForm";
            this.panelForm.Size = new System.Drawing.Size(516, 488);
            this.panelForm.TabIndex = 6;
            // 
            // lblHeader
            // 
            this.lblHeader.AutoSize = true;
            this.lblHeader.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblHeader.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblHeader.Location = new System.Drawing.Point(16, 9);
            this.lblHeader.Name = "lblHeader";
            this.lblHeader.Size = new System.Drawing.Size(132, 20);
            this.lblHeader.TabIndex = 7;
            this.lblHeader.Text = "Deed Switchboard";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.lstDeeds);
            this.groupBox4.Location = new System.Drawing.Point(10, 84);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(167, 361);
            this.groupBox4.TabIndex = 3;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Deeds";
            // 
            // lstDeeds
            // 
            this.lstDeeds.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
            this.lstDeeds.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lstDeeds.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
            this.lstDeeds.FullRowSelect = true;
            this.lstDeeds.GridLines = true;
            this.lstDeeds.LabelEdit = true;
            this.lstDeeds.Location = new System.Drawing.Point(7, 15);
            this.lstDeeds.MultiSelect = false;
            this.lstDeeds.Name = "lstDeeds";
            this.lstDeeds.Size = new System.Drawing.Size(154, 339);
            this.lstDeeds.TabIndex = 0;
            this.lstDeeds.UseCompatibleStateImageBehavior = false;
            this.lstDeeds.View = System.Windows.Forms.View.Details;
            this.lstDeeds.SelectedIndexChanged += new System.EventHandler(this.lstDeeds_SelectedIndexChanged);
            this.lstDeeds.DoubleClick += new System.EventHandler(this.lstDeeds_DoubleClick_1);
            this.lstDeeds.Enter += new System.EventHandler(this.lstDeeds_Enter);
            this.lstDeeds.KeyUp += new System.Windows.Forms.KeyEventHandler(this.lstDeeds_KeyUp);
            this.lstDeeds.MouseClick += new System.Windows.Forms.MouseEventHandler(this.lstDeeds_MouseClick);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Deed Number";
            this.columnHeader1.Width = 128;
            // 
            // groupBox2
            // 
            this.groupBox2.Location = new System.Drawing.Point(183, 84);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(313, 361);
            this.groupBox2.TabIndex = 4;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Summary";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.cmdSearch);
            this.groupBox3.Controls.Add(this.txtdeedSearch);
            this.groupBox3.Controls.Add(this.label1);
            this.groupBox3.Controls.Add(this.lblVol);
            this.groupBox3.Controls.Add(this.lblBook);
            this.groupBox3.Controls.Add(this.lblDeedYr);
            this.groupBox3.Controls.Add(this.label4);
            this.groupBox3.Controls.Add(this.label3);
            this.groupBox3.Controls.Add(this.label2);
            this.groupBox3.Location = new System.Drawing.Point(10, 11);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(486, 67);
            this.groupBox3.TabIndex = 5;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Deed Details:";
            // 
            // cmdSearch
            // 
            this.cmdSearch.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("cmdSearch.BackgroundImage")));
            this.cmdSearch.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.cmdSearch.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.cmdSearch.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmdSearch.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmdSearch.Location = new System.Drawing.Point(391, 16);
            this.cmdSearch.Name = "cmdSearch";
            this.cmdSearch.Size = new System.Drawing.Size(85, 32);
            this.cmdSearch.TabIndex = 11;
            this.cmdSearch.Text = "&Search";
            this.cmdSearch.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.cmdSearch.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.cmdSearch.UseCompatibleTextRendering = true;
            this.cmdSearch.UseVisualStyleBackColor = true;
            this.cmdSearch.Click += new System.EventHandler(this.cmdSearch_Click_1);
            // 
            // txtdeedSearch
            // 
            this.txtdeedSearch.BackColor = System.Drawing.Color.White;
            this.txtdeedSearch.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtdeedSearch.ForeColor = System.Drawing.Color.Black;
            this.txtdeedSearch.Location = new System.Drawing.Point(341, 22);
            this.txtdeedSearch.Mandatory = true;
            this.txtdeedSearch.Name = "txtdeedSearch";
            this.txtdeedSearch.Size = new System.Drawing.Size(48, 23);
            this.txtdeedSearch.TabIndex = 10;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(283, 25);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(56, 15);
            this.label1.TabIndex = 9;
            this.label1.Text = "Deed No:";
            // 
            // lblVol
            // 
            this.lblVol.AutoSize = true;
            this.lblVol.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblVol.Location = new System.Drawing.Point(243, 25);
            this.lblVol.Name = "lblVol";
            this.lblVol.Size = new System.Drawing.Size(38, 15);
            this.lblVol.TabIndex = 8;
            this.lblVol.Text = "label6";
            // 
            // lblBook
            // 
            this.lblBook.AutoSize = true;
            this.lblBook.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblBook.Location = new System.Drawing.Point(150, 25);
            this.lblBook.Name = "lblBook";
            this.lblBook.Size = new System.Drawing.Size(38, 15);
            this.lblBook.TabIndex = 7;
            this.lblBook.Text = "label5";
            // 
            // lblDeedYr
            // 
            this.lblDeedYr.AutoSize = true;
            this.lblDeedYr.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDeedYr.Location = new System.Drawing.Point(71, 25);
            this.lblDeedYr.Name = "lblDeedYr";
            this.lblDeedYr.Size = new System.Drawing.Size(38, 15);
            this.lblDeedYr.TabIndex = 6;
            this.lblDeedYr.Text = "label5";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(111, 25);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(37, 15);
            this.label4.TabIndex = 5;
            this.label4.Text = "Book:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(190, 25);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(50, 15);
            this.label3.TabIndex = 4;
            this.label3.Text = "Volume:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(6, 25);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(62, 15);
            this.label2.TabIndex = 3;
            this.label2.Text = "Deed Year:";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.groupBox3);
            this.groupBox1.Controls.Add(this.groupBox2);
            this.groupBox1.Controls.Add(this.groupBox4);
            this.groupBox1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.groupBox1.Location = new System.Drawing.Point(10, 53);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(502, 451);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            // 
            // cmsDeeds
            // 
            this.cmsDeeds.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.updateDeedToolStripMenuItem,
            this.deleteDeedToolStripMenuItem});
            this.cmsDeeds.Name = "cmsDeeds";
            this.cmsDeeds.Size = new System.Drawing.Size(143, 48);
            // 
            // updateDeedToolStripMenuItem
            // 
            this.updateDeedToolStripMenuItem.Name = "updateDeedToolStripMenuItem";
            this.updateDeedToolStripMenuItem.Size = new System.Drawing.Size(142, 22);
            this.updateDeedToolStripMenuItem.Text = "&Update Deed";
            this.updateDeedToolStripMenuItem.Click += new System.EventHandler(this.updateDeedToolStripMenuItem_Click);
            // 
            // deleteDeedToolStripMenuItem
            // 
            this.deleteDeedToolStripMenuItem.Name = "deleteDeedToolStripMenuItem";
            this.deleteDeedToolStripMenuItem.Size = new System.Drawing.Size(142, 22);
            this.deleteDeedToolStripMenuItem.Text = "&Delete Deed";
            this.deleteDeedToolStripMenuItem.Click += new System.EventHandler(this.deleteDeedToolStripMenuItem_Click);
            // 
            // frmDeeds
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(524, 549);
            this.ControlBox = false;
            this.Controls.Add(this.lblHeader);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.panelForm);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmDeeds";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Deeds in Volume";
            this.Load += new System.EventHandler(this.frmDeeds_Load);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.frmDeeds_KeyUp);
            this.groupBox4.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.cmsDeeds.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panelForm;
        private nControls.deLabel lblHeader;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.ListView lstDeeds;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox3;
        private nControls.deButton cmdSearch;
        private nControls.deTextBox txtdeedSearch;
        private nControls.deLabel label1;
        private nControls.deLabel lblVol;
        private nControls.deLabel lblBook;
        private nControls.deLabel lblDeedYr;
        private nControls.deLabel label4;
        private nControls.deLabel label3;
        private nControls.deLabel label2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ContextMenuStrip cmsDeeds;
        private System.Windows.Forms.ToolStripMenuItem updateDeedToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteDeedToolStripMenuItem;

    }
}