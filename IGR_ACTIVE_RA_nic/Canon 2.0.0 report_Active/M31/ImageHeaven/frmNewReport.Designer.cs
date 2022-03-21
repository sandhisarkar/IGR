namespace ImageHeaven
{
    partial class frmNewReport
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
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.buttonPopulate = new System.Windows.Forms.Button();
            this.cmbRunnum = new System.Windows.Forms.ComboBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.labelRunNo = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.buttonReport = new System.Windows.Forms.Button();
            this.sfdUAT = new System.Windows.Forms.SaveFileDialog();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.Location = new System.Drawing.Point(0, 0);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.Size = new System.Drawing.Size(511, 291);
            this.dataGridView1.TabIndex = 0;
            // 
            // buttonPopulate
            // 
            this.buttonPopulate.Location = new System.Drawing.Point(214, 85);
            this.buttonPopulate.Name = "buttonPopulate";
            this.buttonPopulate.Size = new System.Drawing.Size(75, 23);
            this.buttonPopulate.TabIndex = 3;
            this.buttonPopulate.Text = "Populate";
            this.buttonPopulate.UseVisualStyleBackColor = true;
            this.buttonPopulate.Click += new System.EventHandler(this.buttonPopulate_Click);
            // 
            // cmbRunnum
            // 
            this.cmbRunnum.FormattingEnabled = true;
            this.cmbRunnum.Location = new System.Drawing.Point(217, 34);
            this.cmbRunnum.Name = "cmbRunnum";
            this.cmbRunnum.Size = new System.Drawing.Size(121, 21);
            this.cmbRunnum.TabIndex = 2;
            this.cmbRunnum.Leave += new System.EventHandler(this.cmbRunnum_Leave);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.dataGridView1);
            this.panel1.Location = new System.Drawing.Point(-2, 140);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(511, 291);
            this.panel1.TabIndex = 5;
            // 
            // labelRunNo
            // 
            this.labelRunNo.AutoSize = true;
            this.labelRunNo.Location = new System.Drawing.Point(158, 37);
            this.labelRunNo.Name = "labelRunNo";
            this.labelRunNo.Size = new System.Drawing.Size(53, 13);
            this.labelRunNo.TabIndex = 6;
            this.labelRunNo.Text = "Run No. :";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(211, 120);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(0, 15);
            this.label1.TabIndex = 7;
            // 
            // buttonReport
            // 
            this.buttonReport.Location = new System.Drawing.Point(203, 85);
            this.buttonReport.Name = "buttonReport";
            this.buttonReport.Size = new System.Drawing.Size(104, 23);
            this.buttonReport.TabIndex = 4;
            this.buttonReport.Text = "Report Generate";
            this.buttonReport.UseVisualStyleBackColor = true;
            this.buttonReport.Visible = false;
            this.buttonReport.Click += new System.EventHandler(this.buttonReport_Click);
            // 
            // frmNewReport
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(507, 430);
            this.Controls.Add(this.buttonReport);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.labelRunNo);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.cmbRunnum);
            this.Controls.Add(this.buttonPopulate);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "frmNewReport";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "UAT Report";
            this.Load += new System.EventHandler(this.frmNewReport_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Button buttonPopulate;
        private System.Windows.Forms.ComboBox cmbRunnum;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label labelRunNo;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button buttonReport;
        private System.Windows.Forms.SaveFileDialog sfdUAT;
    }
}