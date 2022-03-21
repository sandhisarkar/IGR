namespace ImageHeaven
{
    partial class frmUatReport
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle10 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle11 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle12 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmUatReport));
            this.grpMain = new System.Windows.Forms.GroupBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.cmbRo = new nControls.deComboBox();
            this.cmbDistrict = new nControls.deComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.tlpDetails = new System.Windows.Forms.TableLayoutPanel();
            this.lblImageSubmitted = new System.Windows.Forms.Label();
            this.lblDeedsubmitted = new System.Windows.Forms.Label();
            this.lbltotalimagehold = new System.Windows.Forms.Label();
            this.lblTotalhold = new System.Windows.Forms.Label();
            this.lbltotalImagesscanned = new System.Windows.Forms.Label();
            this.lbltotalProperty = new System.Windows.Forms.Label();
            this.lblTotalperson = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.lblTotalDeed = new System.Windows.Forms.Label();
            this.dtGrdresult = new nControls.deDataGridView();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cmbVolume = new nControls.deTextBox();
            this.cmbBook = new nControls.deTextBox();
            this.cmbDeedYear = new nControls.deTextBox();
            this.cmdSearch = new nControls.deButton();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.label19 = new System.Windows.Forms.Label();
            this.label20 = new System.Windows.Forms.Label();
            this.label21 = new System.Windows.Forms.Label();
            this.label22 = new System.Windows.Forms.Label();
            this.label23 = new System.Windows.Forms.Label();
            this.label24 = new System.Windows.Forms.Label();
            this.grpMain.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.tlpDetails.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dtGrdresult)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpMain
            // 
            this.grpMain.Controls.Add(this.groupBox3);
            this.grpMain.Controls.Add(this.groupBox2);
            this.grpMain.Controls.Add(this.groupBox1);
            this.grpMain.Location = new System.Drawing.Point(7, 0);
            this.grpMain.Name = "grpMain";
            this.grpMain.Size = new System.Drawing.Size(461, 380);
            this.grpMain.TabIndex = 0;
            this.grpMain.TabStop = false;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.cmbRo);
            this.groupBox3.Controls.Add(this.cmbDistrict);
            this.groupBox3.Controls.Add(this.label5);
            this.groupBox3.Controls.Add(this.label1);
            this.groupBox3.Location = new System.Drawing.Point(6, 9);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(446, 47);
            this.groupBox3.TabIndex = 2;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Where Registered";
            // 
            // cmbRo
            // 
            this.cmbRo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbRo.Enabled = false;
            this.cmbRo.FormattingEnabled = true;
            this.cmbRo.Location = new System.Drawing.Point(263, 16);
            this.cmbRo.Mandatory = true;
            this.cmbRo.Name = "cmbRo";
            this.cmbRo.Size = new System.Drawing.Size(168, 21);
            this.cmbRo.TabIndex = 1;
            // 
            // cmbDistrict
            // 
            this.cmbDistrict.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbDistrict.Enabled = false;
            this.cmbDistrict.FormattingEnabled = true;
            this.cmbDistrict.Location = new System.Drawing.Point(54, 15);
            this.cmbDistrict.Mandatory = true;
            this.cmbDistrict.Name = "cmbDistrict";
            this.cmbDistrict.Size = new System.Drawing.Size(168, 21);
            this.cmbDistrict.TabIndex = 0;
            this.cmbDistrict.Leave += new System.EventHandler(this.cmbDistrict_Leave);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(231, 19);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(29, 13);
            this.label5.TabIndex = 1;
            this.label5.Text = "&RO:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(6, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(51, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "&District:";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.tlpDetails);
            this.groupBox2.Controls.Add(this.dtGrdresult);
            this.groupBox2.Location = new System.Drawing.Point(6, 112);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(446, 259);
            this.groupBox2.TabIndex = 4;
            this.groupBox2.TabStop = false;
            // 
            // tlpDetails
            // 
            this.tlpDetails.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Outset;
            this.tlpDetails.ColumnCount = 3;
            this.tlpDetails.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 63F));
            this.tlpDetails.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 65.51724F));
            this.tlpDetails.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 34.48276F));
            this.tlpDetails.Controls.Add(this.label24, 0, 8);
            this.tlpDetails.Controls.Add(this.lblImageSubmitted, 2, 8);
            this.tlpDetails.Controls.Add(this.lblDeedsubmitted, 2, 7);
            this.tlpDetails.Controls.Add(this.lbltotalimagehold, 2, 6);
            this.tlpDetails.Controls.Add(this.lblTotalhold, 2, 5);
            this.tlpDetails.Controls.Add(this.lbltotalImagesscanned, 2, 4);
            this.tlpDetails.Controls.Add(this.lbltotalProperty, 2, 3);
            this.tlpDetails.Controls.Add(this.label8, 1, 8);
            this.tlpDetails.Controls.Add(this.label9, 1, 7);
            this.tlpDetails.Controls.Add(this.label10, 1, 6);
            this.tlpDetails.Controls.Add(this.label11, 1, 5);
            this.tlpDetails.Controls.Add(this.label12, 1, 4);
            this.tlpDetails.Controls.Add(this.label13, 1, 3);
            this.tlpDetails.Controls.Add(this.label14, 1, 2);
            this.tlpDetails.Controls.Add(this.label15, 1, 1);
            this.tlpDetails.Controls.Add(this.label6, 1, 0);
            this.tlpDetails.Controls.Add(this.label7, 2, 0);
            this.tlpDetails.Controls.Add(this.label16, 0, 0);
            this.tlpDetails.Controls.Add(this.label17, 0, 1);
            this.tlpDetails.Controls.Add(this.label18, 0, 2);
            this.tlpDetails.Controls.Add(this.label19, 0, 3);
            this.tlpDetails.Controls.Add(this.label20, 0, 4);
            this.tlpDetails.Controls.Add(this.label21, 0, 5);
            this.tlpDetails.Controls.Add(this.label22, 0, 6);
            this.tlpDetails.Controls.Add(this.label23, 0, 7);
            this.tlpDetails.Controls.Add(this.lblTotalperson, 2, 2);
            this.tlpDetails.Controls.Add(this.lblTotalDeed, 2, 1);
            this.tlpDetails.Location = new System.Drawing.Point(22, 19);
            this.tlpDetails.Name = "tlpDetails";
            this.tlpDetails.RowCount = 9;
            this.tlpDetails.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 45.65217F));
            this.tlpDetails.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 54.34783F));
            this.tlpDetails.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 24F));
            this.tlpDetails.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 22F));
            this.tlpDetails.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 24F));
            this.tlpDetails.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tlpDetails.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tlpDetails.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 27F));
            this.tlpDetails.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tlpDetails.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlpDetails.Size = new System.Drawing.Size(409, 231);
            this.tlpDetails.TabIndex = 3;
            // 
            // lblImageSubmitted
            // 
            this.lblImageSubmitted.AutoSize = true;
            this.lblImageSubmitted.Dock = System.Windows.Forms.DockStyle.Right;
            this.lblImageSubmitted.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblImageSubmitted.Location = new System.Drawing.Point(404, 203);
            this.lblImageSubmitted.Name = "lblImageSubmitted";
            this.lblImageSubmitted.Size = new System.Drawing.Size(0, 26);
            this.lblImageSubmitted.TabIndex = 19;
            // 
            // lblDeedsubmitted
            // 
            this.lblDeedsubmitted.AutoSize = true;
            this.lblDeedsubmitted.Dock = System.Windows.Forms.DockStyle.Right;
            this.lblDeedsubmitted.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDeedsubmitted.Location = new System.Drawing.Point(404, 174);
            this.lblDeedsubmitted.Name = "lblDeedsubmitted";
            this.lblDeedsubmitted.Size = new System.Drawing.Size(0, 27);
            this.lblDeedsubmitted.TabIndex = 18;
            // 
            // lbltotalimagehold
            // 
            this.lbltotalimagehold.AutoSize = true;
            this.lbltotalimagehold.Dock = System.Windows.Forms.DockStyle.Right;
            this.lbltotalimagehold.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbltotalimagehold.Location = new System.Drawing.Point(404, 147);
            this.lbltotalimagehold.Name = "lbltotalimagehold";
            this.lbltotalimagehold.Size = new System.Drawing.Size(0, 25);
            this.lbltotalimagehold.TabIndex = 17;
            // 
            // lblTotalhold
            // 
            this.lblTotalhold.AutoSize = true;
            this.lblTotalhold.Dock = System.Windows.Forms.DockStyle.Right;
            this.lblTotalhold.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTotalhold.Location = new System.Drawing.Point(404, 120);
            this.lblTotalhold.Name = "lblTotalhold";
            this.lblTotalhold.Size = new System.Drawing.Size(0, 25);
            this.lblTotalhold.TabIndex = 16;
            // 
            // lbltotalImagesscanned
            // 
            this.lbltotalImagesscanned.AutoSize = true;
            this.lbltotalImagesscanned.Dock = System.Windows.Forms.DockStyle.Right;
            this.lbltotalImagesscanned.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbltotalImagesscanned.Location = new System.Drawing.Point(404, 94);
            this.lbltotalImagesscanned.Name = "lbltotalImagesscanned";
            this.lbltotalImagesscanned.Size = new System.Drawing.Size(0, 24);
            this.lbltotalImagesscanned.TabIndex = 15;
            this.lbltotalImagesscanned.Click += new System.EventHandler(this.lbltotalImagesscanned_Click);
            // 
            // lbltotalProperty
            // 
            this.lbltotalProperty.AutoSize = true;
            this.lbltotalProperty.Dock = System.Windows.Forms.DockStyle.Right;
            this.lbltotalProperty.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbltotalProperty.Location = new System.Drawing.Point(404, 70);
            this.lbltotalProperty.Name = "lbltotalProperty";
            this.lbltotalProperty.Size = new System.Drawing.Size(0, 22);
            this.lbltotalProperty.TabIndex = 14;
            // 
            // lblTotalperson
            // 
            this.lblTotalperson.AutoSize = true;
            this.lblTotalperson.Dock = System.Windows.Forms.DockStyle.Right;
            this.lblTotalperson.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTotalperson.Location = new System.Drawing.Point(404, 44);
            this.lblTotalperson.Name = "lblTotalperson";
            this.lblTotalperson.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.lblTotalperson.Size = new System.Drawing.Size(0, 24);
            this.lblTotalperson.TabIndex = 13;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(70, 203);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(116, 15);
            this.label8.TabIndex = 4;
            this.label8.Text = "Image Submitted";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.Location = new System.Drawing.Point(70, 174);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(110, 15);
            this.label9.TabIndex = 5;
            this.label9.Text = "Deed Submitted";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.Location = new System.Drawing.Point(70, 147);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(144, 15);
            this.label10.TabIndex = 6;
            this.label10.Text = "Total Images on Hold";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label11.Location = new System.Drawing.Point(70, 120);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(138, 15);
            this.label11.TabIndex = 7;
            this.label11.Text = "Total Deeds on Hold";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label12.Location = new System.Drawing.Point(70, 94);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(143, 15);
            this.label12.TabIndex = 8;
            this.label12.Text = "Total Image Scanned";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label13.Location = new System.Drawing.Point(70, 70);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(96, 15);
            this.label13.TabIndex = 9;
            this.label13.Text = "Total Property";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label14.Location = new System.Drawing.Point(70, 44);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(88, 15);
            this.label14.TabIndex = 10;
            this.label14.Text = "Total Person";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label15.Location = new System.Drawing.Point(70, 21);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(84, 15);
            this.label15.TabIndex = 11;
            this.label15.Text = "Total Deeds";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.ForeColor = System.Drawing.Color.Blue;
            this.label6.Location = new System.Drawing.Point(70, 2);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(80, 15);
            this.label6.TabIndex = 2;
            this.label6.Text = "Description";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.ForeColor = System.Drawing.Color.Blue;
            this.label7.Location = new System.Drawing.Point(293, 2);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(43, 15);
            this.label7.TabIndex = 4;
            this.label7.Text = "Value";
            // 
            // lblTotalDeed
            // 
            this.lblTotalDeed.AutoSize = true;
            this.lblTotalDeed.Dock = System.Windows.Forms.DockStyle.Right;
            this.lblTotalDeed.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTotalDeed.Location = new System.Drawing.Point(404, 21);
            this.lblTotalDeed.Name = "lblTotalDeed";
            this.lblTotalDeed.Size = new System.Drawing.Size(0, 21);
            this.lblTotalDeed.TabIndex = 12;
            // 
            // dtGrdresult
            // 
            this.dtGrdresult.AllowUserToAddRows = false;
            this.dtGrdresult.AllowUserToDeleteRows = false;
            dataGridViewCellStyle10.SelectionBackColor = System.Drawing.SystemColors.ActiveCaption;
            dataGridViewCellStyle10.SelectionForeColor = System.Drawing.Color.White;
            this.dtGrdresult.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle10;
            this.dtGrdresult.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dtGrdresult.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle11.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle11.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle11.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle11.ForeColor = System.Drawing.SystemColors.ControlDark;
            dataGridViewCellStyle11.SelectionBackColor = System.Drawing.Color.WhiteSmoke;
            dataGridViewCellStyle11.SelectionForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle11.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dtGrdresult.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle11;
            this.dtGrdresult.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dtGrdresult.Location = new System.Drawing.Point(6, 19);
            this.dtGrdresult.Name = "dtGrdresult";
            this.dtGrdresult.ReadOnly = true;
            this.dtGrdresult.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle12.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle12.BackColor = System.Drawing.SystemColors.ControlLightLight;
            dataGridViewCellStyle12.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle12.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle12.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle12.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle12.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dtGrdresult.RowHeadersDefaultCellStyle = dataGridViewCellStyle12;
            this.dtGrdresult.RowHeadersVisible = false;
            this.dtGrdresult.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dtGrdresult.Size = new System.Drawing.Size(13, 10);
            this.dtGrdresult.TabIndex = 1;
            this.dtGrdresult.Visible = false;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.cmbVolume);
            this.groupBox1.Controls.Add(this.cmbBook);
            this.groupBox1.Controls.Add(this.cmbDeedYear);
            this.groupBox1.Controls.Add(this.cmdSearch);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Location = new System.Drawing.Point(6, 58);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(446, 49);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Filter";
            // 
            // cmbVolume
            // 
            this.cmbVolume.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbVolume.Location = new System.Drawing.Point(283, 18);
            this.cmbVolume.Name = "cmbVolume";
            this.cmbVolume.Size = new System.Drawing.Size(50, 23);
            this.cmbVolume.TabIndex = 2;
            // 
            // cmbBook
            // 
            this.cmbBook.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbBook.Location = new System.Drawing.Point(172, 18);
            this.cmbBook.Name = "cmbBook";
            this.cmbBook.Size = new System.Drawing.Size(50, 23);
            this.cmbBook.TabIndex = 1;
            // 
            // cmbDeedYear
            // 
            this.cmbDeedYear.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbDeedYear.Location = new System.Drawing.Point(73, 18);
            this.cmbDeedYear.Name = "cmbDeedYear";
            this.cmbDeedYear.Size = new System.Drawing.Size(50, 23);
            this.cmbDeedYear.TabIndex = 0;
            // 
            // cmdSearch
            // 
            this.cmdSearch.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("cmdSearch.BackgroundImage")));
            this.cmdSearch.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.cmdSearch.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.cmdSearch.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmdSearch.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmdSearch.Location = new System.Drawing.Point(346, 11);
            this.cmdSearch.Name = "cmdSearch";
            this.cmdSearch.Size = new System.Drawing.Size(85, 32);
            this.cmdSearch.TabIndex = 3;
            this.cmdSearch.Text = "&Search";
            this.cmdSearch.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.cmdSearch.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.cmdSearch.UseCompatibleTextRendering = true;
            this.cmdSearch.UseVisualStyleBackColor = true;
            this.cmdSearch.Click += new System.EventHandler(this.cmdSearch_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(129, 22);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(40, 13);
            this.label4.TabIndex = 2;
            this.label4.Text = "&Book:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(229, 22);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(52, 13);
            this.label3.TabIndex = 1;
            this.label3.Text = "&Volume:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(6, 22);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(71, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Deed &Year:";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label16.ForeColor = System.Drawing.Color.Blue;
            this.label16.Location = new System.Drawing.Point(5, 2);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(47, 15);
            this.label16.TabIndex = 20;
            this.label16.Text = "Srl No";
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label17.ForeColor = System.Drawing.Color.Black;
            this.label17.Location = new System.Drawing.Point(5, 21);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(15, 15);
            this.label17.TabIndex = 21;
            this.label17.Text = "1";
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label18.ForeColor = System.Drawing.Color.Black;
            this.label18.Location = new System.Drawing.Point(5, 44);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(15, 15);
            this.label18.TabIndex = 22;
            this.label18.Text = "2";
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label19.ForeColor = System.Drawing.Color.Black;
            this.label19.Location = new System.Drawing.Point(5, 70);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(15, 15);
            this.label19.TabIndex = 23;
            this.label19.Text = "3";
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label20.ForeColor = System.Drawing.Color.Black;
            this.label20.Location = new System.Drawing.Point(5, 94);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(15, 15);
            this.label20.TabIndex = 24;
            this.label20.Text = "4";
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label21.ForeColor = System.Drawing.Color.Black;
            this.label21.Location = new System.Drawing.Point(5, 120);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(15, 15);
            this.label21.TabIndex = 25;
            this.label21.Text = "5";
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label22.ForeColor = System.Drawing.Color.Black;
            this.label22.Location = new System.Drawing.Point(5, 147);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(15, 15);
            this.label22.TabIndex = 26;
            this.label22.Text = "6";
            // 
            // label23
            // 
            this.label23.AutoSize = true;
            this.label23.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label23.ForeColor = System.Drawing.Color.Black;
            this.label23.Location = new System.Drawing.Point(5, 174);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(15, 15);
            this.label23.TabIndex = 27;
            this.label23.Text = "7";
            // 
            // label24
            // 
            this.label24.AutoSize = true;
            this.label24.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label24.ForeColor = System.Drawing.Color.Black;
            this.label24.Location = new System.Drawing.Point(5, 203);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(15, 15);
            this.label24.TabIndex = 28;
            this.label24.Text = "8";
            // 
            // frmUatReport
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(475, 386);
            this.Controls.Add(this.grpMain);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmUatReport";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "UAT Report...";
            this.Load += new System.EventHandler(this.frmUatReport_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmUatReport_FormClosing);
            this.grpMain.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.tlpDetails.ResumeLayout(false);
            this.tlpDetails.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dtGrdresult)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox grpMain;
        private System.Windows.Forms.GroupBox groupBox2;
        private nControls.deDataGridView dtGrdresult;
        private System.Windows.Forms.GroupBox groupBox3;
        private nControls.deComboBox cmbRo;
        private nControls.deComboBox cmbDistrict;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox1;
        private nControls.deTextBox cmbVolume;
        private nControls.deTextBox cmbBook;
        private nControls.deTextBox cmbDeedYear;
        private nControls.deButton cmdSearch;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TableLayoutPanel tlpDetails;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label lblImageSubmitted;
        private System.Windows.Forms.Label lblDeedsubmitted;
        private System.Windows.Forms.Label lbltotalimagehold;
        private System.Windows.Forms.Label lblTotalhold;
        private System.Windows.Forms.Label lbltotalImagesscanned;
        private System.Windows.Forms.Label lbltotalProperty;
        private System.Windows.Forms.Label lblTotalperson;
        private System.Windows.Forms.Label lblTotalDeed;
        private System.Windows.Forms.Label label24;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.Label label23;
    }
}