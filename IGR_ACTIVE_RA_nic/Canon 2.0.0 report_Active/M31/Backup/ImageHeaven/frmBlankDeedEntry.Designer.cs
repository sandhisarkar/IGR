namespace ImageHeaven
{
    partial class frmBlankDeedEntry
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
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.cmbBook = new nControls.deComboBox();
            this.cmbWhereReg = new nControls.deComboBox();
            this.cmbDistrict = new nControls.deComboBox();
            this.txtDeed = new nControls.deTextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.cmbVolume = new nControls.deTextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.cmbDeedYear = new nControls.deTextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.cmdSave = new System.Windows.Forms.Button();
            this.cmdCancel = new System.Windows.Forms.Button();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.cmbBook);
            this.groupBox3.Controls.Add(this.cmbWhereReg);
            this.groupBox3.Controls.Add(this.cmbDistrict);
            this.groupBox3.Controls.Add(this.txtDeed);
            this.groupBox3.Controls.Add(this.label6);
            this.groupBox3.Controls.Add(this.cmbVolume);
            this.groupBox3.Controls.Add(this.label3);
            this.groupBox3.Controls.Add(this.label4);
            this.groupBox3.Controls.Add(this.cmbDeedYear);
            this.groupBox3.Controls.Add(this.label5);
            this.groupBox3.Controls.Add(this.label1);
            this.groupBox3.Controls.Add(this.label2);
            this.groupBox3.Location = new System.Drawing.Point(1, 12);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(409, 194);
            this.groupBox3.TabIndex = 2;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Control Fields";
            // 
            // cmbBook
            // 
            this.cmbBook.BackColor = System.Drawing.Color.White;
            this.cmbBook.Enabled = false;
            this.cmbBook.ForeColor = System.Drawing.Color.Black;
            this.cmbBook.FormattingEnabled = true;
            this.cmbBook.Location = new System.Drawing.Point(67, 74);
            this.cmbBook.Mandatory = true;
            this.cmbBook.Name = "cmbBook";
            this.cmbBook.Size = new System.Drawing.Size(337, 21);
            this.cmbBook.TabIndex = 94;
            // 
            // cmbWhereReg
            // 
            this.cmbWhereReg.BackColor = System.Drawing.Color.White;
            this.cmbWhereReg.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbWhereReg.Enabled = false;
            this.cmbWhereReg.ForeColor = System.Drawing.Color.Black;
            this.cmbWhereReg.FormattingEnabled = true;
            this.cmbWhereReg.Location = new System.Drawing.Point(67, 48);
            this.cmbWhereReg.Mandatory = true;
            this.cmbWhereReg.Name = "cmbWhereReg";
            this.cmbWhereReg.Size = new System.Drawing.Size(337, 21);
            this.cmbWhereReg.TabIndex = 93;
            // 
            // cmbDistrict
            // 
            this.cmbDistrict.BackColor = System.Drawing.Color.White;
            this.cmbDistrict.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbDistrict.Enabled = false;
            this.cmbDistrict.ForeColor = System.Drawing.Color.Black;
            this.cmbDistrict.FormattingEnabled = true;
            this.cmbDistrict.Location = new System.Drawing.Point(66, 22);
            this.cmbDistrict.Mandatory = true;
            this.cmbDistrict.Name = "cmbDistrict";
            this.cmbDistrict.Size = new System.Drawing.Size(338, 21);
            this.cmbDistrict.TabIndex = 92;
            // 
            // txtDeed
            // 
            this.txtDeed.BackColor = System.Drawing.Color.White;
            this.txtDeed.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtDeed.ForeColor = System.Drawing.Color.Black;
            this.txtDeed.Location = new System.Drawing.Point(66, 156);
            this.txtDeed.Mandatory = true;
            this.txtDeed.MaxLength = 5;
            this.txtDeed.Name = "txtDeed";
            this.txtDeed.Size = new System.Drawing.Size(84, 23);
            this.txtDeed.TabIndex = 3;
            this.txtDeed.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtDeed_KeyPress);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(29, 162);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(36, 13);
            this.label6.TabIndex = 1;
            this.label6.Text = "&Deed:";
            // 
            // cmbVolume
            // 
            this.cmbVolume.BackColor = System.Drawing.Color.White;
            this.cmbVolume.Enabled = false;
            this.cmbVolume.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbVolume.ForeColor = System.Drawing.Color.Black;
            this.cmbVolume.Location = new System.Drawing.Point(66, 128);
            this.cmbVolume.Mandatory = true;
            this.cmbVolume.Name = "cmbVolume";
            this.cmbVolume.Size = new System.Drawing.Size(84, 23);
            this.cmbVolume.TabIndex = 2;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(20, 134);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(45, 13);
            this.label3.TabIndex = 1;
            this.label3.Text = "&Volume:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(30, 78);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(35, 13);
            this.label4.TabIndex = 2;
            this.label4.Text = "&Book:";
            // 
            // cmbDeedYear
            // 
            this.cmbDeedYear.BackColor = System.Drawing.Color.White;
            this.cmbDeedYear.Enabled = false;
            this.cmbDeedYear.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbDeedYear.ForeColor = System.Drawing.Color.Black;
            this.cmbDeedYear.Location = new System.Drawing.Point(66, 100);
            this.cmbDeedYear.Mandatory = true;
            this.cmbDeedYear.Name = "cmbDeedYear";
            this.cmbDeedYear.Size = new System.Drawing.Size(84, 23);
            this.cmbDeedYear.TabIndex = 0;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(39, 50);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(26, 13);
            this.label5.TabIndex = 1;
            this.label5.Text = "&RO:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(23, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(42, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "&District:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(4, 106);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(61, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Deed &Year:";
            // 
            // cmdSave
            // 
            this.cmdSave.Location = new System.Drawing.Point(254, 212);
            this.cmdSave.Name = "cmdSave";
            this.cmdSave.Size = new System.Drawing.Size(75, 23);
            this.cmdSave.TabIndex = 3;
            this.cmdSave.Text = "&Save";
            this.cmdSave.UseVisualStyleBackColor = true;
            this.cmdSave.Click += new System.EventHandler(this.cmdSave_Click);
            // 
            // cmdCancel
            // 
            this.cmdCancel.Location = new System.Drawing.Point(335, 212);
            this.cmdCancel.Name = "cmdCancel";
            this.cmdCancel.Size = new System.Drawing.Size(75, 23);
            this.cmdCancel.TabIndex = 3;
            this.cmdCancel.Text = "Ca&ncel";
            this.cmdCancel.UseVisualStyleBackColor = true;
            this.cmdCancel.Click += new System.EventHandler(this.cmdCancel_Click);
            // 
            // frmBlankDeedEntry
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(413, 240);
            this.Controls.Add(this.cmdCancel);
            this.Controls.Add(this.cmdSave);
            this.Controls.Add(this.groupBox3);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmBlankDeedEntry";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "New Deed Entry";
            this.Load += new System.EventHandler(this.frmBlankDeedEntry_Load);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label1;
        private nControls.deTextBox cmbVolume;
        private nControls.deTextBox cmbDeedYear;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button cmdSave;
        private System.Windows.Forms.Button cmdCancel;
        private nControls.deTextBox txtDeed;
        private nControls.deComboBox cmbBook;
        private nControls.deComboBox cmbWhereReg;
        private nControls.deComboBox cmbDistrict;
    }
}