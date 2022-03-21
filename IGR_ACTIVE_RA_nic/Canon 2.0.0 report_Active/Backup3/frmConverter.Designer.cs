namespace LandAreaConversion
{
    partial class frmConverter
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
            this.btnReset = new System.Windows.Forms.Button();
            this.btnConvert = new System.Windows.Forms.Button();
            this.grpPlots = new System.Windows.Forms.GroupBox();
            this.txtPlotCount = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.cmdClose = new System.Windows.Forms.Button();
            this.ConvtabControl = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.txtsqfeetLand = new System.Windows.Forms.TextBox();
            this.txtDecimal = new System.Windows.Forms.TextBox();
            this.label17 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.txtChatP = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtKathaP = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.txtBighaP = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.txtAcrP = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.txtChatC = new System.Windows.Forms.TextBox();
            this.label14 = new System.Windows.Forms.Label();
            this.txtKathaC = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.txtBighaC = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.txtAcrC = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.txtChattak = new System.Windows.Forms.TextBox();
            this.txtKatha = new System.Windows.Forms.TextBox();
            this.txtBigha = new System.Windows.Forms.TextBox();
            this.txtAcre = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.txtSqFeet = new System.Windows.Forms.TextBox();
            this.label15 = new System.Windows.Forms.Label();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.rdoStructure = new System.Windows.Forms.RadioButton();
            this.rdoLand = new System.Windows.Forms.RadioButton();
            this.txtsqMtr = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.grpPlots.SuspendLayout();
            this.ConvtabControl.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnReset
            // 
            this.btnReset.Location = new System.Drawing.Point(12, 331);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(75, 23);
            this.btnReset.TabIndex = 9;
            this.btnReset.Text = "&Reset";
            this.btnReset.UseVisualStyleBackColor = true;
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // btnConvert
            // 
            this.btnConvert.Location = new System.Drawing.Point(194, 331);
            this.btnConvert.Name = "btnConvert";
            this.btnConvert.Size = new System.Drawing.Size(75, 23);
            this.btnConvert.TabIndex = 8;
            this.btnConvert.Text = "Convert";
            this.btnConvert.UseVisualStyleBackColor = true;
            this.btnConvert.Visible = false;
            this.btnConvert.Click += new System.EventHandler(this.btnConvert_Click);
            // 
            // grpPlots
            // 
            this.grpPlots.Controls.Add(this.txtPlotCount);
            this.grpPlots.Controls.Add(this.label7);
            this.grpPlots.Location = new System.Drawing.Point(6, 5);
            this.grpPlots.Name = "grpPlots";
            this.grpPlots.Size = new System.Drawing.Size(347, 53);
            this.grpPlots.TabIndex = 0;
            this.grpPlots.TabStop = false;
            this.grpPlots.Text = "Plots";
            // 
            // txtPlotCount
            // 
            this.txtPlotCount.Location = new System.Drawing.Point(95, 19);
            this.txtPlotCount.Name = "txtPlotCount";
            this.txtPlotCount.Size = new System.Drawing.Size(75, 20);
            this.txtPlotCount.TabIndex = 1;
            this.txtPlotCount.TextChanged += new System.EventHandler(this.txtPlotCount_TextChanged);
            this.txtPlotCount.Enter += new System.EventHandler(this.txtPlotCount_Enter);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(7, 22);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(85, 13);
            this.label7.TabIndex = 14;
            this.label7.Text = "Number of Plots:";
            // 
            // cmdClose
            // 
            this.cmdClose.Location = new System.Drawing.Point(281, 331);
            this.cmdClose.Name = "cmdClose";
            this.cmdClose.Size = new System.Drawing.Size(75, 23);
            this.cmdClose.TabIndex = 6;
            this.cmdClose.Text = "&Done!";
            this.cmdClose.UseVisualStyleBackColor = true;
            this.cmdClose.Click += new System.EventHandler(this.cmdClose_Click);
            // 
            // ConvtabControl
            // 
            this.ConvtabControl.Controls.Add(this.tabPage1);
            this.ConvtabControl.Controls.Add(this.tabPage2);
            this.ConvtabControl.Location = new System.Drawing.Point(6, 63);
            this.ConvtabControl.Name = "ConvtabControl";
            this.ConvtabControl.SelectedIndex = 0;
            this.ConvtabControl.ShowToolTips = true;
            this.ConvtabControl.Size = new System.Drawing.Size(347, 232);
            this.ConvtabControl.TabIndex = 1;
            this.ConvtabControl.SelectedIndexChanged += new System.EventHandler(this.ConvtabControl_SelectedIndexChanged);
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.txtsqfeetLand);
            this.tabPage1.Controls.Add(this.txtDecimal);
            this.tabPage1.Controls.Add(this.label17);
            this.tabPage1.Controls.Add(this.label16);
            this.tabPage1.Controls.Add(this.groupBox3);
            this.tabPage1.Controls.Add(this.groupBox2);
            this.tabPage1.Controls.Add(this.groupBox1);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(339, 206);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "conv1 - F1";
            this.tabPage1.ToolTipText = "Conversion betwen Area , Bigha , Katha , Chattak";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // txtsqfeetLand
            // 
            this.txtsqfeetLand.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtsqfeetLand.Location = new System.Drawing.Point(172, 177);
            this.txtsqfeetLand.Name = "txtsqfeetLand";
            this.txtsqfeetLand.Size = new System.Drawing.Size(38, 20);
            this.txtsqfeetLand.TabIndex = 5;
            this.txtsqfeetLand.Visible = false;
            this.txtsqfeetLand.Enter += new System.EventHandler(this.txtsqfeetLand_Enter);
            // 
            // txtDecimal
            // 
            this.txtDecimal.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtDecimal.Location = new System.Drawing.Point(59, 177);
            this.txtDecimal.Name = "txtDecimal";
            this.txtDecimal.Size = new System.Drawing.Size(38, 20);
            this.txtDecimal.TabIndex = 4;
            this.txtDecimal.Enter += new System.EventHandler(this.txtDecimal_Enter);
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(111, 180);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(59, 13);
            this.label17.TabIndex = 46;
            this.label17.Text = "Sqf (Land):";
            this.label17.Visible = false;
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(10, 180);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(48, 13);
            this.label16.TabIndex = 45;
            this.label16.Text = "Decimal:";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.txtChatP);
            this.groupBox3.Controls.Add(this.label4);
            this.groupBox3.Controls.Add(this.txtKathaP);
            this.groupBox3.Controls.Add(this.label6);
            this.groupBox3.Controls.Add(this.txtBighaP);
            this.groupBox3.Controls.Add(this.label8);
            this.groupBox3.Controls.Add(this.txtAcrP);
            this.groupBox3.Controls.Add(this.label9);
            this.groupBox3.Location = new System.Drawing.Point(229, 28);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(105, 145);
            this.groupBox3.TabIndex = 44;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Split Values ";
            // 
            // txtChatP
            // 
            this.txtChatP.Enabled = false;
            this.txtChatP.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtChatP.Location = new System.Drawing.Point(59, 106);
            this.txtChatP.Name = "txtChatP";
            this.txtChatP.Size = new System.Drawing.Size(36, 20);
            this.txtChatP.TabIndex = 15;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 109);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(47, 13);
            this.label4.TabIndex = 14;
            this.label4.Text = "Chattak:";
            // 
            // txtKathaP
            // 
            this.txtKathaP.Enabled = false;
            this.txtKathaP.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtKathaP.Location = new System.Drawing.Point(59, 80);
            this.txtKathaP.Name = "txtKathaP";
            this.txtKathaP.Size = new System.Drawing.Size(36, 20);
            this.txtKathaP.TabIndex = 11;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(15, 83);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(38, 13);
            this.label6.TabIndex = 10;
            this.label6.Text = "Katha:";
            // 
            // txtBighaP
            // 
            this.txtBighaP.Enabled = false;
            this.txtBighaP.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtBighaP.Location = new System.Drawing.Point(59, 54);
            this.txtBighaP.Name = "txtBighaP";
            this.txtBighaP.Size = new System.Drawing.Size(36, 20);
            this.txtBighaP.TabIndex = 9;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(16, 54);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(37, 13);
            this.label8.TabIndex = 8;
            this.label8.Text = "Bigha:";
            // 
            // txtAcrP
            // 
            this.txtAcrP.Enabled = false;
            this.txtAcrP.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtAcrP.Location = new System.Drawing.Point(59, 28);
            this.txtAcrP.Name = "txtAcrP";
            this.txtAcrP.Size = new System.Drawing.Size(36, 20);
            this.txtAcrP.TabIndex = 7;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(21, 28);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(32, 13);
            this.label9.TabIndex = 2;
            this.label9.Text = "Acre:";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.txtChatC);
            this.groupBox2.Controls.Add(this.label14);
            this.groupBox2.Controls.Add(this.txtKathaC);
            this.groupBox2.Controls.Add(this.label12);
            this.groupBox2.Controls.Add(this.txtBighaC);
            this.groupBox2.Controls.Add(this.label11);
            this.groupBox2.Controls.Add(this.txtAcrC);
            this.groupBox2.Controls.Add(this.label10);
            this.groupBox2.Location = new System.Drawing.Point(114, 28);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(109, 145);
            this.groupBox2.TabIndex = 43;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Converted Values";
            // 
            // txtChatC
            // 
            this.txtChatC.Enabled = false;
            this.txtChatC.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtChatC.Location = new System.Drawing.Point(58, 106);
            this.txtChatC.Name = "txtChatC";
            this.txtChatC.Size = new System.Drawing.Size(38, 20);
            this.txtChatC.TabIndex = 15;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(5, 109);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(47, 13);
            this.label14.TabIndex = 14;
            this.label14.Text = "Chattak:";
            // 
            // txtKathaC
            // 
            this.txtKathaC.Enabled = false;
            this.txtKathaC.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtKathaC.Location = new System.Drawing.Point(58, 80);
            this.txtKathaC.Name = "txtKathaC";
            this.txtKathaC.Size = new System.Drawing.Size(38, 20);
            this.txtKathaC.TabIndex = 11;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(14, 83);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(38, 13);
            this.label12.TabIndex = 10;
            this.label12.Text = "Katha:";
            // 
            // txtBighaC
            // 
            this.txtBighaC.Enabled = false;
            this.txtBighaC.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtBighaC.Location = new System.Drawing.Point(58, 54);
            this.txtBighaC.Name = "txtBighaC";
            this.txtBighaC.Size = new System.Drawing.Size(38, 20);
            this.txtBighaC.TabIndex = 9;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(15, 54);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(37, 13);
            this.label11.TabIndex = 8;
            this.label11.Text = "Bigha:";
            // 
            // txtAcrC
            // 
            this.txtAcrC.Enabled = false;
            this.txtAcrC.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtAcrC.Location = new System.Drawing.Point(58, 28);
            this.txtAcrC.Name = "txtAcrC";
            this.txtAcrC.Size = new System.Drawing.Size(38, 20);
            this.txtAcrC.TabIndex = 7;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(20, 28);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(32, 13);
            this.label10.TabIndex = 2;
            this.label10.Text = "Acre:";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.txtChattak);
            this.groupBox1.Controls.Add(this.txtKatha);
            this.groupBox1.Controls.Add(this.txtBigha);
            this.groupBox1.Controls.Add(this.txtAcre);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(3, 28);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(105, 145);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "values ";
            // 
            // txtChattak
            // 
            this.txtChattak.Location = new System.Drawing.Point(56, 106);
            this.txtChattak.Name = "txtChattak";
            this.txtChattak.Size = new System.Drawing.Size(36, 20);
            this.txtChattak.TabIndex = 3;
            this.txtChattak.Leave += new System.EventHandler(this.txtChattak_Leave);
            this.txtChattak.KeyUp += new System.Windows.Forms.KeyEventHandler(this.txtChattak_KeyUp);
            // 
            // txtKatha
            // 
            this.txtKatha.Location = new System.Drawing.Point(56, 80);
            this.txtKatha.Name = "txtKatha";
            this.txtKatha.Size = new System.Drawing.Size(36, 20);
            this.txtKatha.TabIndex = 2;
            this.txtKatha.Leave += new System.EventHandler(this.txtKatha_Leave);
            this.txtKatha.KeyUp += new System.Windows.Forms.KeyEventHandler(this.txtKatha_KeyUp);
            // 
            // txtBigha
            // 
            this.txtBigha.Location = new System.Drawing.Point(56, 54);
            this.txtBigha.Name = "txtBigha";
            this.txtBigha.Size = new System.Drawing.Size(36, 20);
            this.txtBigha.TabIndex = 1;
            this.txtBigha.Leave += new System.EventHandler(this.txtBigha_Leave);
            this.txtBigha.KeyUp += new System.Windows.Forms.KeyEventHandler(this.txtBigha_KeyUp);
            // 
            // txtAcre
            // 
            this.txtAcre.Location = new System.Drawing.Point(56, 28);
            this.txtAcre.Name = "txtAcre";
            this.txtAcre.Size = new System.Drawing.Size(36, 20);
            this.txtAcre.TabIndex = 0;
            this.txtAcre.Leave += new System.EventHandler(this.txtAcre_Leave);
            this.txtAcre.KeyUp += new System.Windows.Forms.KeyEventHandler(this.txtAcre_KeyUp);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(7, 106);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(47, 13);
            this.label5.TabIndex = 4;
            this.label5.Text = "Chattak:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(16, 80);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(38, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Katha:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(17, 54);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(37, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Bigha:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(22, 28);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(32, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Acre:";
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.groupBox5);
            this.tabPage2.Controls.Add(this.groupBox4);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(339, 206);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "conv2 - F2";
            this.tabPage2.ToolTipText = "Square Meter to Square Feet";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.txtSqFeet);
            this.groupBox5.Controls.Add(this.label15);
            this.groupBox5.Location = new System.Drawing.Point(150, 6);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(169, 77);
            this.groupBox5.TabIndex = 1;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Converted Values";
            // 
            // txtSqFeet
            // 
            this.txtSqFeet.Enabled = false;
            this.txtSqFeet.Location = new System.Drawing.Point(97, 19);
            this.txtSqFeet.Name = "txtSqFeet";
            this.txtSqFeet.Size = new System.Drawing.Size(66, 20);
            this.txtSqFeet.TabIndex = 4;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(23, 22);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(68, 13);
            this.label15.TabIndex = 0;
            this.label15.Text = "Square Feet:";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.rdoStructure);
            this.groupBox4.Controls.Add(this.rdoLand);
            this.groupBox4.Controls.Add(this.txtsqMtr);
            this.groupBox4.Controls.Add(this.label13);
            this.groupBox4.Location = new System.Drawing.Point(7, 6);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(135, 77);
            this.groupBox4.TabIndex = 0;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Values";
            // 
            // rdoStructure
            // 
            this.rdoStructure.AutoSize = true;
            this.rdoStructure.Location = new System.Drawing.Point(61, 19);
            this.rdoStructure.Name = "rdoStructure";
            this.rdoStructure.Size = new System.Drawing.Size(68, 17);
            this.rdoStructure.TabIndex = 2;
            this.rdoStructure.Text = "Structure";
            this.rdoStructure.UseVisualStyleBackColor = true;
            // 
            // rdoLand
            // 
            this.rdoLand.AutoSize = true;
            this.rdoLand.Checked = true;
            this.rdoLand.Location = new System.Drawing.Point(6, 19);
            this.rdoLand.Name = "rdoLand";
            this.rdoLand.Size = new System.Drawing.Size(49, 17);
            this.rdoLand.TabIndex = 1;
            this.rdoLand.TabStop = true;
            this.rdoLand.Text = "Land";
            this.rdoLand.UseVisualStyleBackColor = true;
            // 
            // txtsqMtr
            // 
            this.txtsqMtr.Location = new System.Drawing.Point(84, 47);
            this.txtsqMtr.Name = "txtsqMtr";
            this.txtsqMtr.Size = new System.Drawing.Size(36, 20);
            this.txtsqMtr.TabIndex = 3;
            this.txtsqMtr.Leave += new System.EventHandler(this.txtsqMtr_Leave);
            this.txtsqMtr.KeyUp += new System.Windows.Forms.KeyEventHandler(this.txtsqMtr_KeyUp);
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(6, 50);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(74, 13);
            this.label13.TabIndex = 0;
            this.label13.Text = "Square Meter:";
            // 
            // frmConverter
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(371, 365);
            this.ControlBox = false;
            this.Controls.Add(this.ConvtabControl);
            this.Controls.Add(this.btnReset);
            this.Controls.Add(this.cmdClose);
            this.Controls.Add(this.btnConvert);
            this.Controls.Add(this.grpPlots);
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmConverter";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Converter";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.Form1_Load);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmConverter_FormClosed);
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.frmConverter_KeyPress);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.frmConverter_KeyUp);
            this.grpPlots.ResumeLayout(false);
            this.grpPlots.PerformLayout();
            this.ConvtabControl.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnConvert;
        private System.Windows.Forms.Button btnReset;
        private System.Windows.Forms.GroupBox grpPlots;
        private System.Windows.Forms.TextBox txtPlotCount;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button cmdClose;
        private System.Windows.Forms.TabControl ConvtabControl;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TextBox txtChatP;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtKathaP;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtBighaP;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox txtAcrP;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox txtChatC;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.TextBox txtKathaC;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox txtBighaC;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox txtAcrC;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox txtChattak;
        private System.Windows.Forms.TextBox txtKatha;
        private System.Windows.Forms.TextBox txtBigha;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.TextBox txtsqMtr;
        private System.Windows.Forms.TextBox txtSqFeet;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.TextBox txtAcre;
        private System.Windows.Forms.RadioButton rdoStructure;
        private System.Windows.Forms.RadioButton rdoLand;
        private System.Windows.Forms.TextBox txtsqfeetLand;
        private System.Windows.Forms.TextBox txtDecimal;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Label label16;
    }
}

