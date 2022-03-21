/*
 * Created by SharpDevelop.
 * User: SubhajitB
 * Date: 9/4/2009
 * Time: 3:27 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
namespace ImageHeaven
{
	partial class aeFQC
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
            NovaNet.wfe.eSTATES[] state = new NovaNet.wfe.eSTATES[9];
            NovaNet.wfe.eSTATES[] imageState = new NovaNet.wfe.eSTATES[1];
            state[0] = NovaNet.wfe.eSTATES.POLICY_FQC;
            state[1] = NovaNet.wfe.eSTATES.POLICY_INDEXED;
            state[2] = NovaNet.wfe.eSTATES.POLICY_EXCEPTION;
            state[3] = NovaNet.wfe.eSTATES.POLICY_ON_HOLD;
            state[4] = NovaNet.wfe.eSTATES.POLICY_CHECKED;
            state[5] = NovaNet.wfe.eSTATES.POLICY_NOT_INDEXED;
            state[6] = NovaNet.wfe.eSTATES.POLICY_EXPORTED;
            state[7] = NovaNet.wfe.eSTATES.POLICY_QC;
            state[8] = NovaNet.wfe.eSTATES.POLICY_SUBMITTED;
            this.components = new System.ComponentModel.Container();
            this.dockPanel = new WeifenLuo.WinFormsUI.Docking.DockPanel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.tabControl2 = new System.Windows.Forms.TabControl();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.pictureControl = new System.Windows.Forms.PictureBox();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.lnkPage3 = new System.Windows.Forms.LinkLabel();
            this.lnkPage2 = new System.Windows.Forms.LinkLabel();
            this.lnkPage1 = new System.Windows.Forms.LinkLabel();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.pictureBox6 = new System.Windows.Forms.PictureBox();
            this.pictureBox5 = new System.Windows.Forms.PictureBox();
            this.pictureBox4 = new System.Windows.Forms.PictureBox();
            this.pictureBox3 = new System.Windows.Forms.PictureBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.groupBox8 = new System.Windows.Forms.GroupBox();
            this.cmbFlatType = new System.Windows.Forms.ComboBox();
            this.cmdFetch = new System.Windows.Forms.Button();
            this.cmbTransaction = new System.Windows.Forms.ComboBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.cmbDocType = new System.Windows.Forms.ComboBox();
            this.lblHold = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.lblTotalDeedCount = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            //this.txtNewdeed = new OnlyNumbers.NumberTextBox();
            this.txtNewdeed = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.txtOldDeedno = new System.Windows.Forms.TextBox();
            this.cmdRenameDeed = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label11 = new System.Windows.Forms.Label();
            this.txtposition = new System.Windows.Forms.TextBox();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.rdoDuplex = new System.Windows.Forms.RadioButton();
            this.rdosimplex = new System.Windows.Forms.RadioButton();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.rdogrey = new System.Windows.Forms.RadioButton();
            this.rdoBW = new System.Windows.Forms.RadioButton();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.rdoFlatbed = new System.Windows.Forms.RadioButton();
            this.rdoAdf = new System.Windows.Forms.RadioButton();
            this.cmdNewDeedEntry = new System.Windows.Forms.Button();
            this.cmdDeedDetails = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.cmdOk = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.txtComments = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txtExceptionType = new System.Windows.Forms.TextBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.listView1 = new System.Windows.Forms.ListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
            this.cmdResolved = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.cmdCustExcp = new System.Windows.Forms.Button();
            this.rdoAll = new System.Windows.Forms.RadioButton();
            this.rdoLIC = new System.Windows.Forms.RadioButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cmdUpdate = new System.Windows.Forms.Button();
            this.txtCommDt = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtDOB = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtName = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtPolicyNumber = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.type = new System.Windows.Forms.GroupBox();
            this.lvwDockTypes = new System.Windows.Forms.ListView();
            this.clmDockType = new System.Windows.Forms.ColumnHeader();
            this.clmShrtCut = new System.Windows.Forms.ColumnHeader();
            this.clmCount = new System.Windows.Forms.ColumnHeader();
            this.BoxDtls = new wSelect.BoxDetails(wBox, sqlCon, state, imageState);
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.markNotReadyHoldPolicyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.markReadyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ShowBatchSummary = new System.Windows.Forms.ToolStripMenuItem();
            this.lblPolicyNumber = new System.Windows.Forms.Label();
            this.cmbPolicy = new System.Windows.Forms.ComboBox();
            this.fileDlg = new System.Windows.Forms.OpenFileDialog();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.lblRecordCount = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.tabControl2.SuspendLayout();
            this.tabPage3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureControl)).BeginInit();
            this.tabPage4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox6)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.panel2.SuspendLayout();
            this.groupBox8.SuspendLayout();
            this.groupBox7.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.type.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // dockPanel
            // 
            this.dockPanel.ActiveAutoHideContent = null;
            this.dockPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dockPanel.Location = new System.Drawing.Point(0, 0);
            this.dockPanel.Name = "dockPanel";
            this.dockPanel.Size = new System.Drawing.Size(1028, 746);
            this.dockPanel.TabIndex = 0;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.tabControl2);
            this.panel1.Location = new System.Drawing.Point(257, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(512, 712);
            this.panel1.TabIndex = 4;
            // 
            // tabControl2
            // 
            this.tabControl2.Controls.Add(this.tabPage3);
            this.tabControl2.Controls.Add(this.tabPage4);
            this.tabControl2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl2.Location = new System.Drawing.Point(0, 0);
            this.tabControl2.Name = "tabControl2";
            this.tabControl2.SelectedIndex = 0;
            this.tabControl2.Size = new System.Drawing.Size(512, 712);
            this.tabControl2.TabIndex = 2;
            // 
            // tabPage3
            // 
            this.tabPage3.AutoScroll = true;
            this.tabPage3.Controls.Add(this.pictureControl);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(504, 686);
            this.tabPage3.TabIndex = 0;
            this.tabPage3.Text = "Single";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // pictureControl
            // 
            this.pictureControl.Location = new System.Drawing.Point(-1, 3);
            this.pictureControl.Name = "pictureControl";
            this.pictureControl.Size = new System.Drawing.Size(502, 680);
            this.pictureControl.TabIndex = 1;
            this.pictureControl.TabStop = false;
            this.pictureControl.MouseMove += new System.Windows.Forms.MouseEventHandler(this.PictureControlMouseMove);
            this.pictureControl.MouseDown += new System.Windows.Forms.MouseEventHandler(this.PictureControlMouseDown);
            this.pictureControl.MouseUp += new System.Windows.Forms.MouseEventHandler(this.PictureControlMouseUp);
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.lnkPage3);
            this.tabPage4.Controls.Add(this.lnkPage2);
            this.tabPage4.Controls.Add(this.lnkPage1);
            this.tabPage4.Controls.Add(this.pictureBox2);
            this.tabPage4.Controls.Add(this.pictureBox6);
            this.tabPage4.Controls.Add(this.pictureBox5);
            this.tabPage4.Controls.Add(this.pictureBox4);
            this.tabPage4.Controls.Add(this.pictureBox3);
            this.tabPage4.Controls.Add(this.pictureBox1);
            this.tabPage4.Location = new System.Drawing.Point(4, 22);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage4.Size = new System.Drawing.Size(504, 686);
            this.tabPage4.TabIndex = 1;
            this.tabPage4.Text = "All";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // lnkPage3
            // 
            this.lnkPage3.Location = new System.Drawing.Point(510, 639);
            this.lnkPage3.Name = "lnkPage3";
            this.lnkPage3.Size = new System.Drawing.Size(76, 15);
            this.lnkPage3.TabIndex = 1;
            this.lnkPage3.TabStop = true;
            this.lnkPage3.Text = "Page three";
            this.lnkPage3.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkPage3_LinkClicked);
            // 
            // lnkPage2
            // 
            this.lnkPage2.Location = new System.Drawing.Point(510, 623);
            this.lnkPage2.Name = "lnkPage2";
            this.lnkPage2.Size = new System.Drawing.Size(55, 16);
            this.lnkPage2.TabIndex = 1;
            this.lnkPage2.TabStop = true;
            this.lnkPage2.Text = "Page two";
            this.lnkPage2.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkPage2_LinkClicked);
            // 
            // lnkPage1
            // 
            this.lnkPage1.Location = new System.Drawing.Point(510, 607);
            this.lnkPage1.Name = "lnkPage1";
            this.lnkPage1.Size = new System.Drawing.Size(55, 16);
            this.lnkPage1.TabIndex = 1;
            this.lnkPage1.TabStop = true;
            this.lnkPage1.Text = "Page one";
            this.lnkPage1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkPage1_LinkClicked);
            // 
            // pictureBox2
            // 
            this.pictureBox2.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.pictureBox2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pictureBox2.Location = new System.Drawing.Point(175, 19);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(163, 328);
            this.pictureBox2.TabIndex = 0;
            this.pictureBox2.TabStop = false;
            this.pictureBox2.DoubleClick += new System.EventHandler(this.PictureBox2DoubleClick);
            // 
            // pictureBox6
            // 
            this.pictureBox6.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.pictureBox6.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pictureBox6.Location = new System.Drawing.Point(341, 353);
            this.pictureBox6.Name = "pictureBox6";
            this.pictureBox6.Size = new System.Drawing.Size(163, 325);
            this.pictureBox6.TabIndex = 0;
            this.pictureBox6.TabStop = false;
            this.pictureBox6.DoubleClick += new System.EventHandler(this.PictureBox6DoubleClick);
            // 
            // pictureBox5
            // 
            this.pictureBox5.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.pictureBox5.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pictureBox5.Location = new System.Drawing.Point(6, 353);
            this.pictureBox5.Name = "pictureBox5";
            this.pictureBox5.Size = new System.Drawing.Size(163, 325);
            this.pictureBox5.TabIndex = 0;
            this.pictureBox5.TabStop = false;
            this.pictureBox5.DoubleClick += new System.EventHandler(this.PictureBox5DoubleClick);
            // 
            // pictureBox4
            // 
            this.pictureBox4.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.pictureBox4.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pictureBox4.Location = new System.Drawing.Point(175, 353);
            this.pictureBox4.Name = "pictureBox4";
            this.pictureBox4.Size = new System.Drawing.Size(163, 325);
            this.pictureBox4.TabIndex = 0;
            this.pictureBox4.TabStop = false;
            this.pictureBox4.DoubleClick += new System.EventHandler(this.PictureBox4DoubleClick);
            // 
            // pictureBox3
            // 
            this.pictureBox3.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.pictureBox3.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pictureBox3.Location = new System.Drawing.Point(341, 19);
            this.pictureBox3.Name = "pictureBox3";
            this.pictureBox3.Size = new System.Drawing.Size(163, 328);
            this.pictureBox3.TabIndex = 0;
            this.pictureBox3.TabStop = false;
            this.pictureBox3.DoubleClick += new System.EventHandler(this.PictureBox3DoubleClick);
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.pictureBox1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pictureBox1.Location = new System.Drawing.Point(6, 19);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(163, 328);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.DoubleClick += new System.EventHandler(this.PictureBox1DoubleClick);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.groupBox8);
            this.panel2.Controls.Add(this.lblHold);
            this.panel2.Controls.Add(this.label15);
            this.panel2.Controls.Add(this.lblTotalDeedCount);
            this.panel2.Controls.Add(this.label14);
            this.panel2.Controls.Add(this.groupBox7);
            this.panel2.Controls.Add(this.groupBox3);
            this.panel2.Controls.Add(this.cmdNewDeedEntry);
            this.panel2.Controls.Add(this.cmdDeedDetails);
            this.panel2.Controls.Add(this.groupBox2);
            this.panel2.Controls.Add(this.cmdCustExcp);
            this.panel2.Controls.Add(this.rdoAll);
            this.panel2.Controls.Add(this.rdoLIC);
            this.panel2.Controls.Add(this.groupBox1);
            this.panel2.Controls.Add(this.type);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel2.Location = new System.Drawing.Point(770, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(258, 746);
            this.panel2.TabIndex = 5;
            // 
            // groupBox8
            // 
            this.groupBox8.BackColor = System.Drawing.Color.Azure;
            this.groupBox8.Controls.Add(this.lblRecordCount);
            this.groupBox8.Controls.Add(this.label9);
            this.groupBox8.Controls.Add(this.cmbFlatType);
            this.groupBox8.Controls.Add(this.cmdFetch);
            this.groupBox8.Controls.Add(this.cmbTransaction);
            this.groupBox8.Controls.Add(this.textBox1);
            this.groupBox8.Controls.Add(this.comboBox1);
            this.groupBox8.Controls.Add(this.cmbDocType);
            this.groupBox8.Location = new System.Drawing.Point(9, 518);
            this.groupBox8.Name = "groupBox8";
            this.groupBox8.Size = new System.Drawing.Size(239, 82);
            this.groupBox8.TabIndex = 17;
            this.groupBox8.TabStop = false;
            this.groupBox8.Text = "Filter";
            // 
            // cmbFlatType
            // 
            this.cmbFlatType.FormattingEnabled = true;
            this.cmbFlatType.Location = new System.Drawing.Point(125, 26);
            this.cmbFlatType.Name = "cmbFlatType";
            this.cmbFlatType.Size = new System.Drawing.Size(112, 21);
            this.cmbFlatType.TabIndex = 19;
            this.cmbFlatType.Visible = false;
            // 
            // cmdFetch
            // 
            this.cmdFetch.Location = new System.Drawing.Point(161, 51);
            this.cmdFetch.Name = "cmdFetch";
            this.cmdFetch.Size = new System.Drawing.Size(75, 23);
            this.cmdFetch.TabIndex = 14;
            this.cmdFetch.Text = "Fetch";
            this.cmdFetch.UseVisualStyleBackColor = true;
            this.cmdFetch.Click += new System.EventHandler(this.cmdFetch_Click_1);
            // 
            // cmbTransaction
            // 
            this.cmbTransaction.FormattingEnabled = true;
            this.cmbTransaction.Location = new System.Drawing.Point(126, 25);
            this.cmbTransaction.Name = "cmbTransaction";
            this.cmbTransaction.Size = new System.Drawing.Size(110, 21);
            this.cmbTransaction.TabIndex = 18;
            this.cmbTransaction.Visible = false;
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(175, 25);
            this.textBox1.MaxLength = 2;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(61, 20);
            this.textBox1.TabIndex = 13;
            // 
            // comboBox1
            // 
            this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Items.AddRange(new object[] {
            ">",
            "=",
            "<"});
            this.comboBox1.Location = new System.Drawing.Point(130, 26);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(40, 21);
            this.comboBox1.TabIndex = 12;
            // 
            // cmbDocType
            // 
            this.cmbDocType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbDocType.FormattingEnabled = true;
            this.cmbDocType.Items.AddRange(new object[] {
            "All",
            "Hold",
            "Index of Name",
            "Index of Property",
            "Property Type",
            "Transaction Type(Major)"});
            this.cmbDocType.Location = new System.Drawing.Point(6, 26);
            this.cmbDocType.Name = "cmbDocType";
            this.cmbDocType.Size = new System.Drawing.Size(118, 21);
            this.cmbDocType.TabIndex = 11;
            this.cmbDocType.SelectedIndexChanged += new System.EventHandler(this.cmbDocType_SelectedIndexChanged_1);
            // 
            // lblHold
            // 
            this.lblHold.AutoSize = true;
            this.lblHold.Location = new System.Drawing.Point(73, 35);
            this.lblHold.Name = "lblHold";
            this.lblHold.Size = new System.Drawing.Size(13, 13);
            this.lblHold.TabIndex = 16;
            this.lblHold.Text = "0";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(5, 35);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(59, 13);
            this.label15.TabIndex = 15;
            this.label15.Text = "Total Hold:";
            // 
            // lblTotalDeedCount
            // 
            this.lblTotalDeedCount.AutoSize = true;
            this.lblTotalDeedCount.Location = new System.Drawing.Point(70, 9);
            this.lblTotalDeedCount.Name = "lblTotalDeedCount";
            this.lblTotalDeedCount.Size = new System.Drawing.Size(13, 13);
            this.lblTotalDeedCount.TabIndex = 16;
            this.lblTotalDeedCount.Text = "0";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(5, 9);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(63, 13);
            this.label14.TabIndex = 15;
            this.label14.Text = "Total Deed:";
            // 
            // groupBox7
            // 
            this.groupBox7.BackColor = System.Drawing.Color.Azure;
            this.groupBox7.Controls.Add(this.txtNewdeed);
            this.groupBox7.Controls.Add(this.label13);
            this.groupBox7.Controls.Add(this.label12);
            this.groupBox7.Controls.Add(this.txtOldDeedno);
            this.groupBox7.Controls.Add(this.cmdRenameDeed);
            this.groupBox7.Location = new System.Drawing.Point(7, 439);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.Size = new System.Drawing.Size(239, 73);
            this.groupBox7.TabIndex = 14;
            this.groupBox7.TabStop = false;
            this.groupBox7.Text = "Rename Deed";
            // 
            // txtNewdeed
            // 
            this.txtNewdeed.Location = new System.Drawing.Point(70, 48);
            this.txtNewdeed.MaxLength = 5;
            this.txtNewdeed.Name = "txtNewdeed";
            this.txtNewdeed.Size = new System.Drawing.Size(89, 20);
            this.txtNewdeed.TabIndex = 15;
            this.txtNewdeed.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtNewdeedKeyUp);
            // 
            // label13
            // 
            this.label13.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label13.Location = new System.Drawing.Point(2, 23);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(52, 18);
            this.label13.TabIndex = 28;
            this.label13.Text = "Old No:";
            // 
            // label12
            // 
            this.label12.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label12.Location = new System.Drawing.Point(2, 48);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(62, 18);
            this.label12.TabIndex = 26;
            this.label12.Text = "New No:";
            // 
            // txtOldDeedno
            // 
            this.txtOldDeedno.Enabled = false;
            this.txtOldDeedno.Location = new System.Drawing.Point(70, 21);
            this.txtOldDeedno.Name = "txtOldDeedno";
            this.txtOldDeedno.Size = new System.Drawing.Size(89, 20);
            this.txtOldDeedno.TabIndex = 27;
            // 
            // cmdRenameDeed
            // 
            this.cmdRenameDeed.ForeColor = System.Drawing.Color.Red;
            this.cmdRenameDeed.Location = new System.Drawing.Point(172, 42);
            this.cmdRenameDeed.Name = "cmdRenameDeed";
            this.cmdRenameDeed.Size = new System.Drawing.Size(58, 26);
            this.cmdRenameDeed.TabIndex = 24;
            this.cmdRenameDeed.Text = "&Change";
            this.cmdRenameDeed.UseVisualStyleBackColor = true;
            this.cmdRenameDeed.Click += new System.EventHandler(this.cmdRenameDeed_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.BackColor = System.Drawing.Color.Azure;
            this.groupBox3.Controls.Add(this.label11);
            this.groupBox3.Controls.Add(this.txtposition);
            this.groupBox3.Controls.Add(this.groupBox6);
            this.groupBox3.Controls.Add(this.groupBox5);
            this.groupBox3.Controls.Add(this.groupBox4);
            this.groupBox3.Location = new System.Drawing.Point(6, 293);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(239, 142);
            this.groupBox3.TabIndex = 13;
            this.groupBox3.TabStop = false;
            // 
            // label11
            // 
            this.label11.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label11.Location = new System.Drawing.Point(16, 117);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(56, 18);
            this.label11.TabIndex = 20;
            this.label11.Text = "Position:";
            // 
            // txtposition
            // 
            this.txtposition.Location = new System.Drawing.Point(77, 116);
            this.txtposition.Name = "txtposition";
            this.txtposition.Size = new System.Drawing.Size(51, 20);
            this.txtposition.TabIndex = 21;
            // 
            // groupBox6
            // 
            this.groupBox6.BackColor = System.Drawing.Color.Azure;
            this.groupBox6.Controls.Add(this.rdoDuplex);
            this.groupBox6.Controls.Add(this.rdosimplex);
            this.groupBox6.Location = new System.Drawing.Point(7, 77);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(224, 34);
            this.groupBox6.TabIndex = 2;
            this.groupBox6.TabStop = false;
            // 
            // rdoDuplex
            // 
            this.rdoDuplex.AutoSize = true;
            this.rdoDuplex.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rdoDuplex.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
            this.rdoDuplex.Location = new System.Drawing.Point(130, 11);
            this.rdoDuplex.Name = "rdoDuplex";
            this.rdoDuplex.Size = new System.Drawing.Size(64, 17);
            this.rdoDuplex.TabIndex = 2;
            this.rdoDuplex.Text = "Duplex";
            this.rdoDuplex.UseVisualStyleBackColor = true;
            // 
            // rdosimplex
            // 
            this.rdosimplex.AutoSize = true;
            this.rdosimplex.CheckAlign = System.Drawing.ContentAlignment.TopLeft;
            this.rdosimplex.Checked = true;
            this.rdosimplex.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rdosimplex.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
            this.rdosimplex.Location = new System.Drawing.Point(8, 11);
            this.rdosimplex.Name = "rdosimplex";
            this.rdosimplex.Size = new System.Drawing.Size(68, 17);
            this.rdosimplex.TabIndex = 1;
            this.rdosimplex.TabStop = true;
            this.rdosimplex.Text = "Simplex";
            this.rdosimplex.UseVisualStyleBackColor = true;
            // 
            // groupBox5
            // 
            this.groupBox5.BackColor = System.Drawing.Color.Azure;
            this.groupBox5.Controls.Add(this.rdogrey);
            this.groupBox5.Controls.Add(this.rdoBW);
            this.groupBox5.Location = new System.Drawing.Point(7, 43);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(224, 34);
            this.groupBox5.TabIndex = 1;
            this.groupBox5.TabStop = false;
            // 
            // rdogrey
            // 
            this.rdogrey.AutoSize = true;
            this.rdogrey.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rdogrey.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
            this.rdogrey.Location = new System.Drawing.Point(130, 11);
            this.rdogrey.Name = "rdogrey";
            this.rdogrey.Size = new System.Drawing.Size(83, 17);
            this.rdogrey.TabIndex = 2;
            this.rdogrey.TabStop = true;
            this.rdogrey.Text = "GreyScale";
            this.rdogrey.UseVisualStyleBackColor = true;
            // 
            // rdoBW
            // 
            this.rdoBW.AutoSize = true;
            this.rdoBW.CheckAlign = System.Drawing.ContentAlignment.TopLeft;
            this.rdoBW.Checked = true;
            this.rdoBW.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rdoBW.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
            this.rdoBW.Location = new System.Drawing.Point(8, 11);
            this.rdoBW.Name = "rdoBW";
            this.rdoBW.Size = new System.Drawing.Size(119, 17);
            this.rdoBW.TabIndex = 1;
            this.rdoBW.TabStop = true;
            this.rdoBW.Text = "Black and White";
            this.rdoBW.UseVisualStyleBackColor = true;
            // 
            // groupBox4
            // 
            this.groupBox4.BackColor = System.Drawing.Color.Azure;
            this.groupBox4.Controls.Add(this.rdoFlatbed);
            this.groupBox4.Controls.Add(this.rdoAdf);
            this.groupBox4.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.groupBox4.Location = new System.Drawing.Point(7, 7);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(226, 36);
            this.groupBox4.TabIndex = 0;
            this.groupBox4.TabStop = false;
            // 
            // rdoFlatbed
            // 
            this.rdoFlatbed.AutoSize = true;
            this.rdoFlatbed.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rdoFlatbed.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
            this.rdoFlatbed.Location = new System.Drawing.Point(129, 12);
            this.rdoFlatbed.Name = "rdoFlatbed";
            this.rdoFlatbed.Size = new System.Drawing.Size(80, 17);
            this.rdoFlatbed.TabIndex = 1;
            this.rdoFlatbed.TabStop = true;
            this.rdoFlatbed.Text = "FLATBED";
            this.rdoFlatbed.UseVisualStyleBackColor = true;
            this.rdoFlatbed.CheckedChanged += new System.EventHandler(this.rdoFlatbed_CheckedChanged);
            // 
            // rdoAdf
            // 
            this.rdoAdf.AutoSize = true;
            this.rdoAdf.Checked = true;
            this.rdoAdf.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rdoAdf.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
            this.rdoAdf.Location = new System.Drawing.Point(8, 13);
            this.rdoAdf.Name = "rdoAdf";
            this.rdoAdf.Size = new System.Drawing.Size(49, 17);
            this.rdoAdf.TabIndex = 0;
            this.rdoAdf.TabStop = true;
            this.rdoAdf.Text = "ADF";
            this.rdoAdf.UseVisualStyleBackColor = true;
            // 
            // cmdNewDeedEntry
            // 
            this.cmdNewDeedEntry.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmdNewDeedEntry.Location = new System.Drawing.Point(143, 258);
            this.cmdNewDeedEntry.Name = "cmdNewDeedEntry";
            this.cmdNewDeedEntry.Size = new System.Drawing.Size(102, 30);
            this.cmdNewDeedEntry.TabIndex = 12;
            this.cmdNewDeedEntry.Text = "New Deed Entry";
            this.cmdNewDeedEntry.UseVisualStyleBackColor = true;
            this.cmdNewDeedEntry.Click += new System.EventHandler(this.cmdNewDeedEntry_Click);
            // 
            // cmdDeedDetails
            // 
            this.cmdDeedDetails.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmdDeedDetails.Location = new System.Drawing.Point(9, 258);
            this.cmdDeedDetails.Name = "cmdDeedDetails";
            this.cmdDeedDetails.Size = new System.Drawing.Size(102, 29);
            this.cmdDeedDetails.TabIndex = 12;
            this.cmdDeedDetails.Text = "Get Details";
            this.cmdDeedDetails.UseVisualStyleBackColor = true;
            this.cmdDeedDetails.Click += new System.EventHandler(this.button1_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.tabControl1);
            this.groupBox2.Location = new System.Drawing.Point(6, 51);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(239, 201);
            this.groupBox2.TabIndex = 2;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Exceptions";
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Location = new System.Drawing.Point(3, 19);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(236, 179);
            this.tabControl1.TabIndex = 0;
            this.tabControl1.SelectedIndexChanged += new System.EventHandler(this.tabControl1_SelectedIndexChanged);
            // 
            // tabPage1
            // 
            this.tabPage1.BackColor = System.Drawing.Color.Transparent;
            this.tabPage1.Controls.Add(this.cmdOk);
            this.tabPage1.Controls.Add(this.label6);
            this.tabPage1.Controls.Add(this.txtComments);
            this.tabPage1.Controls.Add(this.label5);
            this.tabPage1.Controls.Add(this.txtExceptionType);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(228, 153);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "IGR QA";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // cmdOk
            // 
            this.cmdOk.Location = new System.Drawing.Point(126, 125);
            this.cmdOk.Name = "cmdOk";
            this.cmdOk.Size = new System.Drawing.Size(96, 23);
            this.cmdOk.TabIndex = 8;
            this.cmdOk.Text = "Mark ok (Policy)";
            this.cmdOk.UseVisualStyleBackColor = true;
            this.cmdOk.Click += new System.EventHandler(this.CmdOkClick);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(5, 10);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(56, 13);
            this.label6.TabIndex = 7;
            this.label6.Text = "Comments";
            // 
            // txtComments
            // 
            this.txtComments.Location = new System.Drawing.Point(6, 26);
            this.txtComments.Multiline = true;
            this.txtComments.Name = "txtComments";
            this.txtComments.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtComments.Size = new System.Drawing.Size(216, 93);
            this.txtComments.TabIndex = 6;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label5.Location = new System.Drawing.Point(3, 10);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(31, 13);
            this.label5.TabIndex = 5;
            this.label5.Text = "Type";
            // 
            // txtExceptionType
            // 
            this.txtExceptionType.Location = new System.Drawing.Point(6, 26);
            this.txtExceptionType.Multiline = true;
            this.txtExceptionType.Name = "txtExceptionType";
            this.txtExceptionType.Size = new System.Drawing.Size(216, 46);
            this.txtExceptionType.TabIndex = 4;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.listView1);
            this.tabPage2.Controls.Add(this.cmdResolved);
            this.tabPage2.Controls.Add(this.label7);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(228, 153);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Custom";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // listView1
            // 
            this.listView1.CheckBoxes = true;
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
            this.listView1.GridLines = true;
            this.listView1.Location = new System.Drawing.Point(7, 23);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(215, 73);
            this.listView1.TabIndex = 3;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Exception Type";
            this.columnHeader1.Width = 100;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Remarks";
            this.columnHeader2.Width = 100;
            // 
            // cmdResolved
            // 
            this.cmdResolved.Location = new System.Drawing.Point(147, 102);
            this.cmdResolved.Name = "cmdResolved";
            this.cmdResolved.Size = new System.Drawing.Size(75, 23);
            this.cmdResolved.TabIndex = 2;
            this.cmdResolved.Text = "Resolved";
            this.cmdResolved.UseVisualStyleBackColor = true;
            this.cmdResolved.Click += new System.EventHandler(this.cmdClick);
            // 
            // label7
            // 
            this.label7.ForeColor = System.Drawing.SystemColors.ActiveCaption;
            this.label7.Location = new System.Drawing.Point(3, 3);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(88, 16);
            this.label7.TabIndex = 0;
            this.label7.Text = "Exception Type";
            // 
            // cmdCustExcp
            // 
            this.cmdCustExcp.Location = new System.Drawing.Point(6, 682);
            this.cmdCustExcp.Name = "cmdCustExcp";
            this.cmdCustExcp.Size = new System.Drawing.Size(240, 23);
            this.cmdCustExcp.TabIndex = 11;
            this.cmdCustExcp.Text = "Show Custom Exception List";
            this.cmdCustExcp.UseVisualStyleBackColor = true;
            this.cmdCustExcp.Visible = false;
            this.cmdCustExcp.Click += new System.EventHandler(this.cmdCustExcp_Click);
            // 
            // rdoAll
            // 
            this.rdoAll.Location = new System.Drawing.Point(35, 719);
            this.rdoAll.Name = "rdoAll";
            this.rdoAll.Size = new System.Drawing.Size(73, 24);
            this.rdoAll.TabIndex = 4;
            this.rdoAll.TabStop = true;
            this.rdoAll.Text = "Show All";
            this.rdoAll.UseVisualStyleBackColor = true;
            this.rdoAll.Visible = false;
            this.rdoAll.CheckedChanged += new System.EventHandler(this.RdoAllCheckedChanged);
            // 
            // rdoLIC
            // 
            this.rdoLIC.Location = new System.Drawing.Point(114, 719);
            this.rdoLIC.Name = "rdoLIC";
            this.rdoLIC.Size = new System.Drawing.Size(104, 24);
            this.rdoLIC.TabIndex = 3;
            this.rdoLIC.TabStop = true;
            this.rdoLIC.Text = "LIC Exception";
            this.rdoLIC.UseVisualStyleBackColor = true;
            this.rdoLIC.Visible = false;
            this.rdoLIC.CheckedChanged += new System.EventHandler(this.RdoLICCheckedChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.cmdUpdate);
            this.groupBox1.Controls.Add(this.txtCommDt);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.txtDOB);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.txtName);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.txtPolicyNumber);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(140, 514);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(55, 37);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Policy Details";
            this.groupBox1.Visible = false;
            // 
            // cmdUpdate
            // 
            this.cmdUpdate.Location = new System.Drawing.Point(140, 97);
            this.cmdUpdate.Name = "cmdUpdate";
            this.cmdUpdate.Size = new System.Drawing.Size(75, 23);
            this.cmdUpdate.TabIndex = 2;
            this.cmdUpdate.Text = "Update";
            this.cmdUpdate.UseVisualStyleBackColor = true;
            this.cmdUpdate.Click += new System.EventHandler(this.CmdUpdateClick);
            // 
            // txtCommDt
            // 
            this.txtCommDt.Enabled = false;
            this.txtCommDt.Location = new System.Drawing.Point(160, 71);
            this.txtCommDt.Name = "txtCommDt";
            this.txtCommDt.Size = new System.Drawing.Size(55, 20);
            this.txtCommDt.TabIndex = 1;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(111, 74);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(45, 13);
            this.label4.TabIndex = 0;
            this.label4.Text = "Com.Dt:";
            // 
            // txtDOB
            // 
            this.txtDOB.Enabled = false;
            this.txtDOB.Location = new System.Drawing.Point(41, 71);
            this.txtDOB.Name = "txtDOB";
            this.txtDOB.Size = new System.Drawing.Size(64, 20);
            this.txtDOB.TabIndex = 1;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 74);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(33, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "DOB:";
            // 
            // txtName
            // 
            this.txtName.Enabled = false;
            this.txtName.Location = new System.Drawing.Point(41, 45);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(174, 20);
            this.txtName.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(7, 48);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(38, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Name:";
            // 
            // txtPolicyNumber
            // 
            this.txtPolicyNumber.Enabled = false;
            this.txtPolicyNumber.Location = new System.Drawing.Point(41, 19);
            this.txtPolicyNumber.Name = "txtPolicyNumber";
            this.txtPolicyNumber.Size = new System.Drawing.Size(174, 20);
            this.txtPolicyNumber.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(-1, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(47, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Number:";
            // 
            // type
            // 
            this.type.Controls.Add(this.lvwDockTypes);
            this.type.Location = new System.Drawing.Point(31, 514);
            this.type.Name = "type";
            this.type.Size = new System.Drawing.Size(70, 47);
            this.type.TabIndex = 0;
            this.type.TabStop = false;
            this.type.Text = "DockTypes";
            this.type.Visible = false;
            // 
            // lvwDockTypes
            // 
            this.lvwDockTypes.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.clmDockType,
            this.clmShrtCut,
            this.clmCount});
            this.lvwDockTypes.GridLines = true;
            this.lvwDockTypes.Location = new System.Drawing.Point(7, 19);
            this.lvwDockTypes.Name = "lvwDockTypes";
            this.lvwDockTypes.Size = new System.Drawing.Size(39, 22);
            this.lvwDockTypes.TabIndex = 0;
            this.lvwDockTypes.UseCompatibleStateImageBehavior = false;
            this.lvwDockTypes.View = System.Windows.Forms.View.Details;
            this.lvwDockTypes.Visible = false;
            this.lvwDockTypes.SelectedIndexChanged += new System.EventHandler(this.lvwDockTypes_SelectedIndexChanged);
            this.lvwDockTypes.Click += new System.EventHandler(this.lvwDockTypes_Click);
            // 
            // clmDockType
            // 
            this.clmDockType.Text = "DocTypes";
            this.clmDockType.Width = 140;
            // 
            // clmShrtCut
            // 
            this.clmShrtCut.Text = "Keys";
            this.clmShrtCut.Width = 40;
            // 
            // clmCount
            // 
            this.clmCount.Text = "Count";
            this.clmCount.Width = 40;
            // 
            // BoxDtls
            // 
            this.BoxDtls.Location = new System.Drawing.Point(0, 0);
            this.BoxDtls.Name = "BoxDtls";
            this.BoxDtls.Size = new System.Drawing.Size(240, 450);
            this.BoxDtls.TabIndex = 9;
            this.BoxDtls.LstImageIndex += new wSelect.LstImageIndexKeyPress(this.BoxDtlsLstImageIndex);
            this.BoxDtls.BoxLoaded += new wSelect.BoxDetailsLoaded(this.BoxDtlsLoaded);
            this.BoxDtls.cmbChanged += new wSelect.ComboValueChanged(this.BoxDtls_cmbChanged);
            this.BoxDtls.LstDelIamgeInsert += new wSelect.LstDelImageKeyPress(this.BoxDtlsLstDelIamgeInsert);
            this.BoxDtls.NextClicked += new wSelect.NextClickedHandler(this.BoxDtlsNextClicked);
            this.BoxDtls.PreviousClicked += new wSelect.PreviousClickedHandler(this.BoxDtls_PreviousClicked);
            this.BoxDtls.MouseDown += new System.Windows.Forms.MouseEventHandler(this.BoxDtlsMouseDown);
            this.BoxDtls.LstDelImgClick += new wSelect.LstDelImageClick(this.BoxDtls_LstDelImgClick);
            this.BoxDtls.BoxMouseClick += new wSelect.BoxDetailsMouseClick(this.BoxDtlsBoxMouseClick);
            this.BoxDtls.LstImgClick += new wSelect.LstImageClick(this.BoxDtls_LstImgClick);
            this.BoxDtls.ImageChanged += new wSelect.ImageChangeHandler(this.BoxDtlsImageChanged);
            this.BoxDtls.PolicyChanged += new wSelect.PolicyChangeHandler(this.BoxDtlsPolicyChanged);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.markNotReadyHoldPolicyToolStripMenuItem,
            this.markReadyToolStripMenuItem,
            this.ShowBatchSummary});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(225, 70);
            this.contextMenuStrip1.Click += new System.EventHandler(this.HoldPolicyToolStripMenuItemClick);
            // 
            // markNotReadyHoldPolicyToolStripMenuItem
            // 
            this.markNotReadyHoldPolicyToolStripMenuItem.Name = "markNotReadyHoldPolicyToolStripMenuItem";
            this.markNotReadyHoldPolicyToolStripMenuItem.Size = new System.Drawing.Size(224, 22);
            this.markNotReadyHoldPolicyToolStripMenuItem.Text = "Mark Not Ready (Hold Policy)";
            this.markNotReadyHoldPolicyToolStripMenuItem.Click += new System.EventHandler(this.HoldPolicyToolStripMenuItemClick);
            // 
            // markReadyToolStripMenuItem
            // 
            this.markReadyToolStripMenuItem.Name = "markReadyToolStripMenuItem";
            this.markReadyToolStripMenuItem.Size = new System.Drawing.Size(224, 22);
            this.markReadyToolStripMenuItem.Text = "Mark Ready";
            this.markReadyToolStripMenuItem.Click += new System.EventHandler(this.markReadyToolStripMenuItemClick);
            // 
            // ShowBatchSummary
            // 
            this.ShowBatchSummary.Name = "ShowBatchSummary";
            this.ShowBatchSummary.Size = new System.Drawing.Size(224, 22);
            this.ShowBatchSummary.Text = "Show Batch Summay";
            this.ShowBatchSummary.Visible = false;
            this.ShowBatchSummary.Click += new System.EventHandler(this.ShowBatchSummary_Click);
            // 
            // lblPolicyNumber
            // 
            this.lblPolicyNumber.AutoSize = true;
            this.lblPolicyNumber.Location = new System.Drawing.Point(12, 499);
            this.lblPolicyNumber.Name = "lblPolicyNumber";
            this.lblPolicyNumber.Size = new System.Drawing.Size(82, 13);
            this.lblPolicyNumber.TabIndex = 11;
            this.lblPolicyNumber.Text = "Copy/Move To:";
            // 
            // cmbPolicy
            // 
            this.cmbPolicy.FormattingEnabled = true;
            this.cmbPolicy.Location = new System.Drawing.Point(98, 423);
            this.cmbPolicy.Name = "cmbPolicy";
            this.cmbPolicy.Size = new System.Drawing.Size(132, 21);
            this.cmbPolicy.TabIndex = 12;
            // 
            // label8
            // 
            this.label8.ForeColor = System.Drawing.Color.Blue;
            this.label8.Location = new System.Drawing.Point(6, 428);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(82, 18);
            this.label8.TabIndex = 14;
            this.label8.Text = "Copy/Move To:";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.Location = new System.Drawing.Point(6, 54);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(99, 13);
            this.label9.TabIndex = 20;
            this.label9.Text = "Record Found : ";
            // 
            // lblRecordCount
            // 
            this.lblRecordCount.AutoSize = true;
            this.lblRecordCount.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblRecordCount.Location = new System.Drawing.Point(105, 54);
            this.lblRecordCount.Name = "lblRecordCount";
            this.lblRecordCount.Size = new System.Drawing.Size(14, 13);
            this.lblRecordCount.TabIndex = 21;
            this.lblRecordCount.Text = "0";
            // 
            // aeFQC
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1028, 746);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.cmbPolicy);
            this.Controls.Add(this.BoxDtls);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.dockPanel);
            this.Controls.Add(this.lblPolicyNumber);
            this.KeyPreview = true;
            this.Name = "aeFQC";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "aeFQC";
            this.WindowState = System.Windows.Forms.FormWindowState.Minimized;
            this.Load += new System.EventHandler(this.AeFQCLoad);
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.AeFQCKeyPress);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.AeFQCKeyUp);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.AeFQCFormClosing);
            this.Resize += new System.EventHandler(this.aeFQC_Resize);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.aeFQC_KeyDown);
            this.panel1.ResumeLayout(false);
            this.tabControl2.ResumeLayout(false);
            this.tabPage3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureControl)).EndInit();
            this.tabPage4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox6)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.groupBox8.ResumeLayout(false);
            this.groupBox8.PerformLayout();
            this.groupBox7.ResumeLayout(false);
            this.groupBox7.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.type.ResumeLayout(false);
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		private System.Windows.Forms.LinkLabel lnkPage1;
		private System.Windows.Forms.LinkLabel lnkPage2;
		private System.Windows.Forms.LinkLabel lnkPage3;
        private System.Windows.Forms.Button cmdCustExcp;
		
		
		private System.Windows.Forms.RadioButton rdoLIC;
		private System.Windows.Forms.RadioButton rdoAll;
        private System.Windows.Forms.Label label8;
		private System.Windows.Forms.ToolStripMenuItem markReadyToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem markNotReadyHoldPolicyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ShowBatchSummary;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
		private System.Windows.Forms.GroupBox groupBox2;
		private WeifenLuo.WinFormsUI.Docking.DockPanel dockPanel;
		private System.Windows.Forms.ColumnHeader clmCount;
		private System.Windows.Forms.ColumnHeader clmShrtCut;
		private System.Windows.Forms.ColumnHeader clmDockType;
		private System.Windows.Forms.ListView lvwDockTypes;
		private System.Windows.Forms.GroupBox type;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox txtPolicyNumber;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox txtName;
		
		
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox txtDOB;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TextBox txtCommDt;
		private System.Windows.Forms.Button cmdUpdate;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel1;
		private wSelect.BoxDetails BoxDtls;
        //private System.Windows.Forms.Button cmdReScan;
		
		
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.TabPage tabPage4;
		
		
        private System.Windows.Forms.PictureBox pictureControl;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.PictureBox pictureBox6;
        private System.Windows.Forms.PictureBox pictureBox5;
        private System.Windows.Forms.PictureBox pictureBox4;
        private System.Windows.Forms.PictureBox pictureBox3;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.TabControl tabControl2;
        private System.Windows.Forms.Label lblPolicyNumber;
        private System.Windows.Forms.ComboBox cmbPolicy;
		
		
		
		
        private System.Windows.Forms.OpenFileDialog fileDlg;
        private System.Windows.Forms.Button cmdDeedDetails;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.RadioButton rdogrey;
        private System.Windows.Forms.RadioButton rdoBW;
        private System.Windows.Forms.RadioButton rdoFlatbed;
        private System.Windows.Forms.RadioButton rdoAdf;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.RadioButton rdoDuplex;
        private System.Windows.Forms.RadioButton rdosimplex;
        private System.Windows.Forms.TextBox txtposition;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.GroupBox groupBox7;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.TextBox txtOldDeedno;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Button cmdRenameDeed;
        private System.Windows.Forms.TextBox txtNewdeed;
        private System.Windows.Forms.Button cmdNewDeedEntry;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.Button cmdOk;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtComments;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtExceptionType;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.Button cmdResolved;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label lblTotalDeedCount;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label lblHold;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.GroupBox groupBox8;
        private System.Windows.Forms.Button cmdFetch;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.ComboBox cmbDocType;
        private System.Windows.Forms.ComboBox cmbFlatType;
        private System.Windows.Forms.ComboBox cmbTransaction;
        private System.Windows.Forms.Label lblRecordCount;
        private System.Windows.Forms.Label label9;
	}
}
