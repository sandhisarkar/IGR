namespace ImageHeaven
{
    partial class frmsysCon
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
            this.grpMain = new System.Windows.Forms.GroupBox();
            this.lblRO = new System.Windows.Forms.Label();
            this.LblDis = new System.Windows.Forms.Label();
            this.btnSave = new System.Windows.Forms.Button();
            this.cmbWhereReg = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.cmbDis = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.grpMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpMain
            // 
            this.grpMain.Controls.Add(this.lblRO);
            this.grpMain.Controls.Add(this.LblDis);
            this.grpMain.Controls.Add(this.btnSave);
            this.grpMain.Controls.Add(this.cmbWhereReg);
            this.grpMain.Controls.Add(this.label3);
            this.grpMain.Controls.Add(this.cmbDis);
            this.grpMain.Controls.Add(this.label1);
            this.grpMain.Location = new System.Drawing.Point(0, 0);
            this.grpMain.Name = "grpMain";
            this.grpMain.Size = new System.Drawing.Size(325, 100);
            this.grpMain.TabIndex = 0;
            this.grpMain.TabStop = false;
            this.grpMain.Text = "Select District and Ro Office";
            // 
            // lblRO
            // 
            this.lblRO.AutoSize = true;
            this.lblRO.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblRO.Location = new System.Drawing.Point(122, 75);
            this.lblRO.Name = "lblRO";
            this.lblRO.Size = new System.Drawing.Size(0, 18);
            this.lblRO.TabIndex = 9;
            // 
            // LblDis
            // 
            this.LblDis.AutoSize = true;
            this.LblDis.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LblDis.Location = new System.Drawing.Point(13, 75);
            this.LblDis.Name = "LblDis";
            this.LblDis.Size = new System.Drawing.Size(0, 18);
            this.LblDis.TabIndex = 8;
            // 
            // btnSave
            // 
            this.btnSave.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSave.Location = new System.Drawing.Point(261, 71);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(56, 23);
            this.btnSave.TabIndex = 6;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // cmbWhereReg
            // 
            this.cmbWhereReg.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbWhereReg.FormattingEnabled = true;
            this.cmbWhereReg.Location = new System.Drawing.Point(116, 44);
            this.cmbWhereReg.Name = "cmbWhereReg";
            this.cmbWhereReg.Size = new System.Drawing.Size(201, 21);
            this.cmbWhereReg.TabIndex = 5;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Times New Roman", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(5, 46);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(105, 15);
            this.label3.TabIndex = 4;
            this.label3.Text = "Where Registered:";
            // 
            // cmbDis
            // 
            this.cmbDis.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbDis.FormattingEnabled = true;
            this.cmbDis.Location = new System.Drawing.Point(116, 17);
            this.cmbDis.Name = "cmbDis";
            this.cmbDis.Size = new System.Drawing.Size(201, 21);
            this.cmbDis.TabIndex = 2;
            this.cmbDis.Leave += new System.EventHandler(this.cmbDis_Leave);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Times New Roman", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(59, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(51, 15);
            this.label1.TabIndex = 3;
            this.label1.Text = "District:";
            // 
            // frmsysCon
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(330, 104);
            this.Controls.Add(this.grpMain);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.Name = "frmsysCon";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.TopMost = true;
            this.Load += new System.EventHandler(this.frmsysCon_Load);
            this.grpMain.ResumeLayout(false);
            this.grpMain.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox grpMain;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.ComboBox cmbWhereReg;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cmbDis;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblRO;
        private System.Windows.Forms.Label LblDis;
    }
}