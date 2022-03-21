/*
 * Created by SharpDevelop.
 * User: SubhajitB
 * Date: 13/4/2008
 * Time: 6:21 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Drawing;
using System.Windows.Forms;
using NovaNet.Utils;
using NovaNet.wfe;
using LItems;
using System.Data;
using System.Data.Odbc;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Data.SqlClient;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.IO;
//using Microsoft.SqlServer.Management.Common;
//using Microsoft.SqlServer.Management.Smo;


namespace ImageHeaven
{
    /// <summary>
    /// Description of aeExport.
    /// </summary>
    public partial class aeExport : Form, StateData
    {
        MemoryStream stateLog;
        byte[] tmpWrite;
        NovaNet.Utils.dbCon dbcon;
        OdbcConnection sqlCon = null;
        eSTATES[] state;
        wfeProject tmpProj = null;
        SqlConnection sqls = null;
        DataSet ds = null;
        DataSet dsexport = new DataSet();
        string batchCount;
        CtrlBox ctrlBox = null;
        wfeBox box = null;
        string sqlFileName = null;
        CtrlPolicy ctrPol = null;
        //		CtrlPolicy ctrlPolicy = null;
        wfePolicy policy = null;
        wfePolicy wPolicy = null;
        CtrlPolicy pPolicy = null;
        CtrlImage pImage = null;
        wfeImage wImage = null;
        wfeBatch wBatch = null;
        private udtPolicy policyData = null;
        StreamWriter sw;
        StreamWriter expLog;
        FileorFolder exportFile;
        string error = null;
        string sqlIp = null;
        string exportPath = null;
        string globalPath = string.Empty;
        string[] imageName;
        Credentials crd = new Credentials();
        private long expImageCount = 0;
        private long expPolicyCount = 0;
        private CtrlBox pBox = null;
        public static NovaNet.Utils.exLog.Logger exMailLog = new NovaNet.Utils.exLog.emailLogger("./errLog.log", NovaNet.Utils.exLog.LogLevel.Dev, Constants._MAIL_TO, Constants._MAIL_FROM, Constants._SMTP);
        public static NovaNet.Utils.exLog.Logger exTxtLog = new NovaNet.Utils.exLog.txtLogger("./errLog.log", NovaNet.Utils.exLog.LogLevel.Dev);
        public Imagery img;
        private ImageConfig config = null;
        public aeExport(OdbcConnection prmCon, Credentials prmCrd)
        {
            System.Diagnostics.Process.GetCurrentProcess().PriorityClass = System.Diagnostics.ProcessPriorityClass.RealTime;
            //
            // The InitializeComponent() call is required for Windows Forms designer support.
            //
            InitializeComponent();
            this.Text = "Export: Volume level";
            dbcon = new NovaNet.Utils.dbCon();
            sqlCon = prmCon;
            crd = prmCrd;
            exMailLog.SetNextLogger(exTxtLog);
            img = IgrFactory.GetImagery(Constants.IGR_CLEARIMAGE);
            ReadINI();
            //
            // TODO: Add constructor code after the InitializeComponent() call.
            //
        }
        private void ReadINI()
        {
            string iniPath = System.IO.Path.GetDirectoryName(Application.ExecutablePath) + "\\Configuration.ini";
            if (File.Exists(iniPath))
            {
                NovaNet.Utils.INIFile ini = new NovaNet.Utils.INIFile();
                sqlIp = ini.ReadINI("SQLSERVERIP", "IP", "", iniPath);
                sqlIp = sqlIp.Replace("\0", "").Trim();
                exportPath = ini.ReadINI("EXPORTPATH", "SQLDBEXPORTPATH", "", iniPath);
                exportPath = exportPath.Replace("\0", "").Trim();
            }
        }
        MemoryStream StateData.StateLog()
        {
            return stateLog;
        }
        private void PopulateBatchCombo()
        {
            string projKey = null;
            DataSet ds = new DataSet();
            bool reExport = false;
            dbcon = new NovaNet.Utils.dbCon();

            wfeBatch tmpBatch = new wfeBatch(sqlCon);
            if (cmbrunNum.SelectedValue != null)
            {
                projKey = cmbrunNum.SelectedValue.ToString();
                if (chkReExport.Checked == false)
                {
                    ds = tmpBatch.GetAllValuesExported(Convert.ToInt32(projKey), reExport);
                }
                else
                {
                    reExport = true;
                    ds = tmpBatch.GetAllValuesExported(Convert.ToInt32(projKey), reExport);
                }
                //batchCount = ds.Tables[0].Rows.Count;
                cmbProject.DataSource = ds.Tables[0];
                cmbProject.DisplayMember = ds.Tables[0].Columns[1].ToString();
                cmbProject.ValueMember = ds.Tables[0].Columns[0].ToString();
            }
        }
        private void PopulateBox()
        {
            string batchKey = null;
            DataSet ds = new DataSet();

            dbcon = new NovaNet.Utils.dbCon();

            int policyCount;

            ctrlBox = new CtrlBox(Convert.ToInt32(cmbrunNum.SelectedValue.ToString()), Convert.ToInt32(cmbProject.SelectedValue.ToString()), "0");
            wfeBox tmpBox = new wfeBox(sqlCon, ctrlBox);
            if (cmbProject.SelectedValue != null)
            {
                lvwExportList.Items.Clear();
                batchKey = cmbProject.SelectedValue.ToString();
                state = new eSTATES[3];
                state[0] = eSTATES.BOX_FQC;
                state[1] = eSTATES.BOX_INDEXED;
                state[2] = eSTATES.BOX_EXPORTED;
                ds = tmpBox.GetExportableBox(state);
                if (ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            ctrPol = new CtrlPolicy(Convert.ToInt32(cmbrunNum.SelectedValue.ToString()), Convert.ToInt32(cmbProject.SelectedValue.ToString()), ds.Tables[0].Rows[i]["box_number"].ToString(), "0");
                            policy = new wfePolicy(sqlCon, ctrPol);
                            if (chkReExport.Checked == false)
                            {
                                state = new eSTATES[4];
                                state[0] = eSTATES.POLICY_CHECKED;
                                state[1] = eSTATES.POLICY_FQC;
                                state[2] = eSTATES.POLICY_INDEXED;
                                state[3] = eSTATES.POLICY_EXCEPTION;
                            }
                            else
                            {
                                state = new eSTATES[5];
                                state[0] = eSTATES.POLICY_CHECKED;
                                state[1] = eSTATES.POLICY_FQC;
                                state[2] = eSTATES.POLICY_INDEXED;
                                state[3] = eSTATES.POLICY_EXCEPTION;
                                state[4] = eSTATES.POLICY_EXPORTED;
                            }
                            policyCount = policy.GetPolicyCount(state);
                            state = new eSTATES[2];
                            state[0] = eSTATES.POLICY_ON_HOLD;
                            state[1] = eSTATES.POLICY_MISSING;
                            int holdMissingPolCount = policy.GetPolicyCount(state);
                            ListViewItem lvwItem = lvwExportList.Items.Add(ds.Tables[0].Rows[i]["box_number"].ToString());
                            lvwItem.SubItems.Add(policyCount.ToString());
                            lvwItem.SubItems.Add(holdMissingPolCount.ToString());
                        }
                    }
                }
            }
        }
        private void PopulateProjectCombo()
        {
            DataSet ds = new DataSet();

            tmpProj = new wfeProject(sqlCon);
            //cmbProject.Items.Add("Select");
            ds = tmpProj.GetAllValuesRun();
            if (ds.Tables[0].Rows.Count > 0)
            {
                cmbrunNum.DataSource = ds.Tables[0];
                cmbrunNum.DisplayMember = ds.Tables[0].Columns[0].ToString();
                cmbrunNum.ValueMember = ds.Tables[0].Columns[0].ToString();
            }
        }
        private void PopulateProject()
        {
            DataSet ds = new DataSet();

            tmpProj = new wfeProject(sqlCon);
            //cmbProject.Items.Add("Select");
            ds = tmpProj.GetAllValues();
            if (ds.Tables[0].Rows.Count > 0)
            {
                cmbProject.DataSource = ds.Tables[0];
                cmbProject.DisplayMember = ds.Tables[0].Columns[1].ToString();
                cmbProject.ValueMember = ds.Tables[0].Columns[0].ToString();
                cmbProject.SelectedIndex = 0;
            }
        }
        private void PopulateBatch()
        {
            DataSet ds = new DataSet();

            tmpProj = new wfeProject(sqlCon);
            //cmbProject.Items.Add("Select");
            ds = tmpProj.GetAllBatchRun(cmbrunNum.Text.ToString(), chkReExport.Checked);
            dgvbatch.DataSource = ds.Tables[0];
            dgvbatch.Columns[0].Width = 25;
            dgvbatch.Columns[1].Width = 160;
            dgvbatch.Columns[1].ReadOnly = true;
            dgvbatch.Columns[0].Visible = false;
            dgvbatch.Columns[2].Visible = false;
            dgvbatch.Columns[3].Visible = false;
        }
        void CmbProjectLeave(object sender, EventArgs e)
        {
            //PopulateBatchCombo();
            PopulateBatch();
        }

        void CmbBatchLeave(object sender, EventArgs e)
        {
            //PopulateBox();
            PopulateBatch();
        }
        void AeExportLoad(object sender, EventArgs e)
        {
            //System.Diagnostics.Process.GetCurrentProcess().PriorityClass = System.Diagnostics.ProcessPriorityClass.RealTime;
            PopulateProject();
            PopulateProjectCombo();
            btnExport.Enabled = false;
            btnExport.Enabled = false;
            config = new ImageConfig(ihConstants.CONFIG_FILE_PATH);
            string Val = config.GetValue(ihConstants.EXPORT_FOLDER_SECTION, ihConstants.EXPORT_FOLDER_KEY).Trim();
            int len = Val.IndexOf('\0');
            Val = Val.Substring(0, len);
            if (Val == string.Empty)
            {
                config = new ImageConfig(ihConstants.CONFIG_FILE_PATH);
                config.SetValue(ihConstants.EXPORT_FOLDER_SECTION, ihConstants.EXPORT_FOLDER_KEY, ihConstants._EXPORT_DRIVE);
            }
            wPolicy = new wfePolicy(sqlCon);
        }
        DataRelation SetRelationPolicyRawData(DataSet pDs, DataSet rDs)
        {
            DataColumn dcpolicyNm = pDs.Tables[0].Columns["policy_number"];
            DataColumn dcRwPolicyNm = rDs.Tables[0].Columns["policy_no"];
            DataColumn[] dcPolicy = new DataColumn[1];
            DataColumn[] dcRaw = new DataColumn[1];
            dcPolicy[0] = dcpolicyNm;
            dcRaw[0] = dcRwPolicyNm;
            return pDs.Relations.Add("PolicyMasterToRawData", dcpolicyNm, dcRwPolicyNm);
        }
        void CmdExportClick(object sender, EventArgs e)
        {
            string divisionCode;
            string branchCode;
            string batchSerial;
            string boxno = string.Empty;
            ArrayList arrPolicy = new ArrayList();
            wQuery pQuery = new ihwQuery(sqlCon);
            string policyNumber = string.Empty;
            DataTable policyDtls = new DataTable();
            DataTable imageDs = new DataTable();
            string imagePath;
            string vendorCode;
            string versionNumber;
            string ImageCount = string.Empty;
            string DocTypeCount = string.Empty;
            //string totalBatchCount;
            string Scanuploadflag;
            string scanneddate;
            //string IncrementedScan;
            string Policyholdername;
            string DOB;
            string dateofcommencement;
            string cust_id;
            string[] imageName;
            string batchPath = string.Empty;
            string exportPath = string.Empty;
            int status;
            string policyPath;
            //string imagePath;
            string fileName = string.Empty;
            string documentType = null;
            //string FileName=null;
            string rootPath;
            exportFile = new FileorFolder();
            ArrayList docType = new ArrayList();
            ArrayList multiPageFileName = new ArrayList();
            OdbcTransaction exportTrans = null;
            bool expBol = true;
            OdbcConnection expSqlCon = null;
            int totPolicyCount = 0;
            string serial_no;
            int policyTotPage = 0;
            bool proposalExists;
            bool signatureExists;
            int pgCountWhlErr = 0;
            string appendPath = string.Empty;
            int tmp = 0;
            OdbcDataAdapter pAdp = new OdbcDataAdapter();
            OdbcDataAdapter iAdp = null;
            DataSet pDs = new DataSet();
            DataSet rDs = new DataSet();
            DataSet iDs = null;
            int maxSerial = 0;
            int normalSerial = 0;
            string[] lastLine = null;
            string tempPath = string.Empty;
            int totImage = 0;
            string expFolder;
            //System.Runtime.GCSettings.LatencyMode = System.Runtime.GCLatencyMode.LowLatency;
            try
            {
                config = new ImageConfig(ihConstants.CONFIG_FILE_PATH);
                expFolder = config.GetValue(ihConstants.EXPORT_FOLDER_SECTION, ihConstants.EXPORT_FOLDER_KEY).Trim();
                int len = expFolder.IndexOf('\0');
                expFolder = expFolder.Substring(0, len);

                tmpProj = new wfeProject(sqlCon);
                ds = new DataSet();
                ds = tmpProj.GetConfiguration();

                vendorCode = ds.Tables[0].Rows[0]["VENDOR_CODE"].ToString();
                versionNumber = ds.Tables[0].Rows[0]["VERSION_NUMBER"].ToString();
                ds.Dispose();
                txtMsg.Text = string.Empty;
                ds = new DataSet();
                ds = tmpProj.GetMainConfiguration(Convert.ToInt32(cmbrunNum.SelectedValue.ToString()), Convert.ToInt32(cmbProject.SelectedValue.ToString()));
                divisionCode = ds.Tables[0].Rows[0]["DO_CODE"].ToString();
                branchCode = ds.Tables[0].Rows[0]["BO_CODE"].ToString();
                wBatch = new wfeBatch(sqlCon);
                batchPath = wBatch.GetPath(Convert.ToInt32(cmbrunNum.SelectedValue.ToString()), Convert.ToInt32(cmbProject.SelectedValue.ToString()));
                //batchPath = "C:"; 
                if (expFolder != string.Empty)
                {
                    if (Directory.Exists(expFolder))
                    {
                        appendPath = expFolder;
                        txtMsg.Text = "Export folder : \r\n";
                        txtMsg.Text = txtMsg.Text + appendPath + "\\" + ihConstants._EXPORT_FOLDER + "\r\n";
                        txtMsg.SelectionStart = txtMsg.Text.Length;
                        txtMsg.ScrollToCaret();
                        txtMsg.Refresh();
                    }
                    else
                    {

                        appendPath = ihConstants._EXPORT_DRIVE;
                        txtMsg.Text = "Given folder does not exists, export folder : \r\n";
                        txtMsg.Text = txtMsg.Text + appendPath + "\\" + ihConstants._EXPORT_FOLDER + "\r\n";
                        txtMsg.SelectionStart = txtMsg.Text.Length;
                        txtMsg.ScrollToCaret();
                        txtMsg.Refresh();
                    }
                }
                else
                {
                    appendPath = ihConstants._EXPORT_DRIVE;
                    txtMsg.Text = "Export to default folder : \r\n";
                    txtMsg.Text = txtMsg.Text + appendPath + "\\" + ihConstants._EXPORT_FOLDER + "\r\n";
                    txtMsg.SelectionStart = txtMsg.Text.Length;
                    txtMsg.ScrollToCaret();
                    txtMsg.Refresh();
                }

                tempPath = appendPath + "\\" + ihConstants._EXPORT_FOLDER + "\\" + ihConstants._LOCAL_APPEND_TEXT_FOLDER;
                if (Directory.Exists(appendPath + ihConstants._EXPORT_FOLDER + "\\" + ihConstants._LOCAL_APPEND_TEXT_FOLDER) == false)
                {
                    Directory.CreateDirectory(appendPath + "\\" + ihConstants._EXPORT_FOLDER + "\\" + ihConstants._LOCAL_APPEND_TEXT_FOLDER);
                }
                if (Directory.Exists(appendPath + "\\" + ihConstants._EXPORT_FOLDER) == false)
                {
                    Directory.CreateDirectory(appendPath + "\\" + ihConstants._EXPORT_FOLDER);
                    Directory.CreateDirectory(appendPath + "\\" + ihConstants._EXPORT_FOLDER + "\\" + cmbProject.Text);
                }
                else
                {
                    if (Directory.Exists(appendPath + "\\" + ihConstants._EXPORT_FOLDER + "\\" + cmbProject.Text))
                    {
                        if (chkReExport.Checked == true)
                        {
                            Directory.Delete(appendPath + "\\" + ihConstants._EXPORT_FOLDER + "\\" + cmbProject.Text, true);
                            Directory.CreateDirectory(appendPath + "\\" + ihConstants._EXPORT_FOLDER + "\\" + cmbProject.Text);
                        }
                    }
                    else
                    {
                        Directory.CreateDirectory(appendPath + "\\" + ihConstants._EXPORT_FOLDER + "\\" + cmbProject.Text);
                    }
                }
                if (wBatch.GetBatchStatus(Convert.ToInt32(cmbProject.SelectedValue.ToString())) == (int)eSTATES.BATCH_READY_FOR_UAT)
                {  
                    DateTime stDt = DateTime.Now;
                    txtMsg.Text = txtMsg.Text + "Batch Number : \r\n";
                    txtMsg.Text = txtMsg.Text + cmbProject.Text + "\r\n";
                    txtMsg.SelectionStart = txtMsg.Text.Length;
                    txtMsg.ScrollToCaret();
                    txtMsg.Refresh();

                    tblExp.SelectedIndex = 1;
                    DateTime stDtTot = DateTime.Now;
                    NovaNet.Utils.dbCon dbcon;
                    dbcon = new NovaNet.Utils.dbCon();
                    //expSqlCon = dbcon.Connect();

                    ctrlBox = new CtrlBox(Convert.ToInt32(cmbrunNum.SelectedValue.ToString()), Convert.ToInt32(cmbProject.SelectedValue.ToString()), "0");
                    box = new wfeBox(sqlCon, ctrlBox);
                    policy = new wfePolicy(sqlCon);

                    pDs = policy.GetAllPolicyDetails(box, out pAdp);
                    rDs = policy.GetAllPolicyDetailsRaw(box);
                    maxSerial = 0;
                    batchCount = policy.GetBatchSerial(box);
                    batchSerial = divisionCode + "-" + branchCode.PadRight(4, Convert.ToChar(" ")) + "-" + batchCount;
                    fileName = vendorCode + "_" + divisionCode + "-" + branchCode.PadRight(4, Convert.ToChar(" ")) + "-" + batchCount + "_" + versionNumber + ".TXT";
                    if (chkReExport.Checked != true)
                    {
                        if (File.Exists(appendPath + "\\" + ihConstants._EXPORT_FOLDER + "\\" + cmbProject.Text + "\\" + fileName))
                        {
                            lastLine = File.ReadAllLines(appendPath + "\\" + ihConstants._EXPORT_FOLDER + "\\" + cmbProject.Text + "\\" + fileName);
                            maxSerial = Convert.ToInt32(lastLine[lastLine.Length - 1].Substring(0, 5));
                        }
                    }
                    else
                    {
                        maxSerial = 0;
                    }

                    for (int i = 0; i < lvwExportList.Items.Count; i++) //Counter for Box
                    {
                        if (lvwExportList.Items[i].Checked == true)
                        {
                            if ((lvwExportList.Items[i].SubItems[1].Text.ToString() != "0") || ((lvwExportList.Items[i].SubItems[2].Text.ToString() != "0")))
                            {
                                DataRow[] dtr = null;
                                sw = new StreamWriter(appendPath + "\\" + ihConstants._EXPORT_FOLDER + "\\" + cmbProject.Text + "\\" + fileName, true);

                                state = new eSTATES[0];
                                if (pDs.Tables.Count > 0)
                                {

                                    if (pDs.Tables[0].Rows.Count > 0)
                                    {

                                        dtr = pDs.Tables[0].Select("proj_key = " + Convert.ToInt32(cmbrunNum.SelectedValue.ToString()) + " and batch_key=" + Convert.ToInt32(cmbProject.SelectedValue.ToString()) + " and box_number=" + Convert.ToInt32(lvwExportList.Items[i].SubItems[0].Text.ToString()));
                                        //rDtR = rDs.Tables[0].Select("policy_no=" + policyNumber);
                                    }
                                }
                                boxno = lvwExportList.Items[i].SubItems[0].Text.ToString().PadLeft(3, Convert.ToChar("0"));
                                //ctrlBox = new CtrlBox(Convert.ToInt32(cmbProject.SelectedValue.ToString()), Convert.ToInt32(cmbBatch.SelectedValue.ToString()), Convert.ToInt32(lvwExportList.Items[i].SubItems[0].Text.ToString()));
                                //box = new wfeBox(sqlCon, ctrlBox);
                                //arrPolicy = pQuery.GetItems(eITEMS.POLICY, state, box);
                                expImageCount = 0;
                                expPolicyCount = 0;
                                pgCountWhlErr = 0;
                                expBol = true;

                                //txtMsg.Text = txtMsg.Text + "Wait while exporting box : ";
                                //txtMsg.Text = txtMsg.Text + boxno + "\r\n";
                                //txtMsg.SelectionStart = txtMsg.Text.Length;
                                //txtMsg.ScrollToCaret();
                                //txtMsg.Refresh();

                                iAdp = new OdbcDataAdapter();
                                ///For all policy details

                                //drelPolicyRaw = SetRelationPolicyRawData(pDs, rDs);
                                pImage = new CtrlImage(Convert.ToInt32(cmbrunNum.SelectedValue.ToString()), Convert.ToInt32(cmbProject.SelectedValue.ToString()), boxno.ToString(), "0", string.Empty, string.Empty);
                                wImage = new wfeImage(sqlCon, pImage);
                                iDs = new DataSet();
                                iDs = wImage.GetAllImages(out iAdp);

                                for (int j = 0; j < dtr.Length; j++) //Loop within box for all the policies
                                {
                                    int docCount = 0;
                                    //DataRow[] pDtR = new DataRow[1];
                                    DataRow[] rDtR = new DataRow[1];
                                    DataRow[] iDtr = null;
                                    //expBol = true;
                                    pgCountWhlErr = 0;
                                    //exportTrans = expSqlCon.BeginTransaction();
                                    boxno = lvwExportList.Items[i].SubItems[0].Text.ToString().PadLeft(3, Convert.ToChar("0"));
                                    expPolicyCount = expPolicyCount + 1;
                                    //imageDs = new DataSet();
                                    //ctrPol = (CtrlPolicy)arrPolicy[j];
                                    policyNumber = dtr[j]["policy_number"].ToString(); //ctrPol.PolicyNumber.ToString();
                                    policy = new wfePolicy(sqlCon, ctrPol);
                                    if (pDs.Tables.Count > 0)
                                    {
                                        if (pDs.Tables[0].Rows.Count > 0)
                                        {
                                            //policyDtls = pDs.Tables[0];
                                            //pDtR = policyDtls.Select("proj_key = " + Convert.ToInt32(cmbProject.SelectedValue.ToString()) + " and batch_key=" + Convert.ToInt32(cmbBatch.SelectedValue.ToString()) + " and box_number=" + Convert.ToInt32(lvwExportList.Items[i].SubItems[0].Text.ToString()) + " and policy_number=" + policyNumber);
                                            rDtR = rDs.Tables[0].Select("trim(policy_no)=" + policyNumber);
                                        }
                                    }

                                    //batchCount = rDtR[0]["batch_serial"].ToString(); 
                                    //policyDtls.Rows[j]["serial_number"].ToString();

                                    //policyData = (udtPolicy)policy.LoadValuesFromDB();
                                    //batchPath = appendPath + "\\" + ihConstants._EXPORT_FOLDER + "\\" + cmbBatch.Text;
                                    policyPath = batchPath + "\\" + lvwExportList.Items[i].SubItems[0].Text.ToString() + "\\" + policyNumber;


                                    imagePath = "\\" + vendorCode + "_" + divisionCode + "-" + branchCode.PadRight(4, Convert.ToChar(" ")) + "-" + batchCount + "_" + versionNumber + "\\" + policyNumber.PadLeft(9, '0');
                                    rootPath = imagePath;


                                    //if (Directory.Exists(".\\" + ihConstants._LOCAL_APPEND_TEXT_FOLDER) == false)
                                    //{
                                    //    Directory.CreateDirectory(".\\" + ihConstants._LOCAL_APPEND_TEXT_FOLDER);
                                    //}
                                    //if (File.Exists(batchPath + "\\" + ihConstants._EXPORT_FOLDER + "\\" + fileName))
                                    //{
                                    //    File.Move(batchPath + "\\" + ihConstants._EXPORT_FOLDER + "\\" + fileName, ".\\" + ihConstants._LOCAL_APPEND_TEXT_FOLDER + "\\" + fileName);
                                    //}
                                    //sw = new StreamWriter(".\\" + ihConstants._LOCAL_APPEND_TEXT_FOLDER + "\\" + fileName, true);
                                    //policyDtls	= policy.GetPolicyDetails();

                                    Policyholdername = rDtR[0]["name_of_policyholder"].ToString().PadRight(30, Convert.ToChar(" ")); //policyDtls.Rows[j]["name_of_policyholder"].ToString().PadRight(30, Convert.ToChar(" "));
                                    DOB = rDtR[0]["date_of_birth"].ToString(); //policyDtls.Rows[j]["date_of_birth"].ToString();
                                    dateofcommencement = rDtR[0]["date_of_commencement"].ToString(); //policyDtls.Rows[j]["date_of_commencement"].ToString();
                                    scanneddate = dtr[j]["scanned_date"].ToString(); //policyDtls.Rows[j]["scanned_date"].ToString();

                                    Scanuploadflag = ihConstants.SCAN_SUCCESS_FLAG; //dtr[j]["Scan_upload_flag"].ToString(); //policyDtls.Rows[j]["Scan_upload_flag"].ToString();
                                    cust_id = rDtR[0]["customer_id"].ToString(); //policyDtls.Rows[j]["customer_id"].ToString();
                                    status = Convert.ToInt32(dtr[j]["status"].ToString()); //Convert.ToInt32(policyDtls.Rows[j]["status"].ToString());
                                    if ((status != (int)eSTATES.POLICY_EXPORTED) && (chkReExport.Checked != true))
                                    {
                                        //toolStatus.Text ="Box: " + lvwExportList.Items[i].SubItems[0].Text.ToString() + " policy - " + policyNumber;
                                        //txtMsg.Text = txtMsg.Text + " Policy : " + policyNumber;
                                        //txtMsg.SelectionStart = txtMsg.Text.Length;
                                        //txtMsg.ScrollToCaret();
                                        //txtMsg.Refresh();
                                        //Application.DoEvents();
                                    }
                                    if (chkReExport.Checked == true)
                                    {
                                        //toolStatus.Text = "Box: " + lvwExportList.Items[i].SubItems[0].Text.ToString() + " policy - " + policyNumber;
                                        //txtMsg.Text = txtMsg.Text + " Policy : " + policyNumber;
                                        //txtMsg.SelectionStart = txtMsg.Text.Length;
                                        //txtMsg.ScrollToCaret();
                                        //txtMsg.Refresh();
                                        //  Application.DoEvents();
                                    }
                                    if (string.IsNullOrEmpty(scanneddate) && (status != (int)eSTATES.POLICY_MISSING))
                                    {
                                        DataSet dt = policy.GetMaxScannedDate();
                                        scanneddate = dt.Tables[0].Rows[0]["scanned_date"].ToString();
                                        dt.Dispose();
                                    }
                                    serial_no = rDtR[0]["serial_number"].ToString().PadLeft(5, Convert.ToChar("0"));
                                    normalSerial = Convert.ToInt32(serial_no);
                                    if ((status != (int)eSTATES.POLICY_MISSING) && ((status != (int)eSTATES.POLICY_ON_HOLD)) && (status != (int)eSTATES.POLICY_SCANNED) && (status != (int)eSTATES.POLICY_QC) && (status != (int)eSTATES.POLICY_INITIALIZED) && (status != (int)eSTATES.POLICY_EXCEPTION) && (status != (int)eSTATES.POLICY_NOT_INDEXED) && ((status != (int)eSTATES.POLICY_EXPORTED) || chkReExport.Checked == true))
                                    {

                                        /////For update policy status
                                        //CtrlPolicy exppPolicy = new CtrlPolicy(Convert.ToInt32(cmbProject.SelectedValue.ToString()), Convert.ToInt32(cmbBatch.SelectedValue.ToString()), Convert.ToInt32(lvwExportList.Items[i].SubItems[0].Text.ToString()), Convert.ToInt32(policyNumber));
                                        //wfePolicy expwPolicy = new wfePolicy(sqlCon, exppPolicy);
                                        //expwPolicy.UpdateStatus(eSTATES.POLICY_EXPORTED, crd);

                                        /////Update image status
                                        //CtrlImage exppImage = new CtrlImage(Convert.ToInt32(cmbProject.SelectedValue.ToString()), Convert.ToInt32(cmbBatch.SelectedValue.ToString()), boxNo.ToString(), Convert.ToInt32(policyNumber), string.Empty, string.Empty);
                                        //wfeImage expwImage = new wfeImage(expSqlCon, exppImage);
                                        //expwImage.UpdateAllImageStatus(eSTATES.PAGE_EXPORTED, crd, exportTrans);

                                        if (status == (int)eSTATES.POLICY_INDEXED)
                                        {
                                            imagePath = policyPath + "\\" + ihConstants._INDEXING_FOLDER;
                                        }
                                        else if (status == (int)eSTATES.POLICY_FQC)
                                        {
                                            imagePath = policyPath + "\\" + ihConstants._FQC_FOLDER;
                                        }
                                        else
                                        {
                                            imagePath = policyPath + "\\" + ihConstants._FQC_FOLDER;
                                        }
                                        if (Directory.Exists(policyPath + "\\" + ihConstants._FQC_FOLDER))
                                        {
                                            imagePath = policyPath + "\\" + ihConstants._FQC_FOLDER;
                                        }
                                        else
                                        {
                                            imagePath = policyPath + "\\" + ihConstants._INDEXING_FOLDER;
                                        }
                                        exportPath = appendPath + "\\" + ihConstants._EXPORT_FOLDER + "\\" + cmbProject.Text + "\\" + vendorCode + "_" + divisionCode + "-" + branchCode.PadRight(4, Convert.ToChar(" ")) + "-" + batchCount + "_" + versionNumber + "\\" + policyNumber.PadLeft(9, '0');
                                        //appendPath = exportPath; //".\\" + ihConstants._LOCAL_APPEND_IMAGE_FOLDER;
                                        if (Directory.Exists(exportPath) == false)
                                        {
                                            Directory.CreateDirectory(exportPath);
                                        }
                                        if (Directory.Exists(tempPath) == false)
                                        {
                                            Directory.CreateDirectory(tempPath);
                                        }
                                        if (imagePath != string.Empty)
                                        {
                                            DeleteLocalFile(imagePath, tempPath);
                                        }
                                        ////Check whether rescanned images have not been indexed
                                        //if (wImage.GetImageCount(eSTATES.PAGE_RESCANNED_NOT_INDEXED) == true)
                                        //{
                                        //    MessageBox.Show("Items rescanned for " + policyNumber + " have not been indexed, aborting...");
                                        //    ///Rollback transaction
                                        //    exportTrans.Rollback();
                                        //    expBol = false;
                                        //    break;
                                        //}
                                        //state = new eSTATES[5];
                                        //state[0] = eSTATES.PAGE_FQC;
                                        //state[1] = eSTATES.PAGE_INDEXED;
                                        //state[2] = eSTATES.PAGE_CHECKED;
                                        //state[3] = eSTATES.PAGE_EXCEPTION;
                                        //state[4] = eSTATES.PAGE_EXPORTED;
                                        //iDtr = new DataRow[iDs.Tables[0].Rows.Count];
                                        if (iDs.Tables.Count > 0)
                                        {
                                            if (iDs.Tables[0].Rows.Count > 0)
                                            {
                                                //imageDs = iDs.Tables[0];
                                                iDtr = iDs.Tables[0].Select("proj_key = " + Convert.ToInt32(cmbrunNum.SelectedValue.ToString()) + " and batch_key=" + Convert.ToInt32(cmbProject.SelectedValue.ToString()) + " and box_number=" + Convert.ToInt32(lvwExportList.Items[i].SubItems[0].Text.ToString()) + " and policy_number=" + policyNumber);
                                            }
                                        }

                                        ImageCount = iDtr.Length.ToString().PadLeft(3, Convert.ToChar("0"));

                                        int pgCount = 0; //Holds the pagecount for a given doctype
                                        for (tmp = 0; tmp < iDtr.Length; tmp++)
                                        {
                                            if (iDtr[tmp]["doc_type"].ToString() == ihConstants.PROPOSALFORM_FILE)
                                                pgCount++;
                                        }
                                        //End: Calculates count for doc types
                                        imageName = new string[pgCount]; //Initialize the array with no. of images for the doc type

                                        int pgCurrent = 0; 	//Temporary variable where count is increased when
                                        //the doc type is found, so that the name can be stored
                                        //in the right index

                                        //Rotate through all the images having doctype = Proposal Form
                                        policyNumber = policyNumber.PadLeft(9, '0');
                                        for (int propCount = 0; propCount < iDtr.Length; propCount++)
                                        {
                                            if (iDtr[propCount]["doc_type"].ToString() == ihConstants.PROPOSALFORM_FILE)
                                            {
                                                DateTime dt = DateTime.Now;
                                                if (pgCurrent == 0)
                                                {
                                                    docType.Add(ihConstants.PROPOSALFORM_FILE);
                                                    multiPageFileName.Add(policyNumber + "_" + ihConstants.PROPOSALFORM_FILE + ".TIF");
                                                    docCount++;
                                                }
                                                //Changed on 19/09/2009 for managing file copying problem in FQC
                                                if (File.Exists(imagePath + "\\" + iDtr[propCount]["page_index_name"].ToString()) == false)
                                                {
                                                    imagePath = policyPath + "\\" + ihConstants._INDEXING_FOLDER;
                                                }
                                                imageName[pgCurrent] = tempPath + "\\" + iDtr[propCount]["page_index_name"].ToString();

                                                expImageCount = expImageCount + 1;
                                                policyTotPage = policyTotPage + 1;
                                                pgCountWhlErr = pgCountWhlErr + 1;
                                                proposalExists = true;

                                                DateTime edDt = DateTime.Now;
                                                TimeSpan tsp = edDt - dt;
                                                //Increment the variable if doctype is found
                                                pgCurrent++;
                                            }
                                            //MessageBox.Show(tsp.Milliseconds.ToString());
                                        }
                                        if (img.CombineTif(imageName, exportPath + "\\" + policyNumber + "_" + ihConstants.PROPOSALFORM_FILE + ".TIF") == false)
                                        {
                                            //exportTrans.Rollback();
                                            expBol = false;
                                            break;
                                        }
                                        pgCount = 0; //Holds the pagecount for a given doctype

                                        for (tmp = 0; tmp < iDtr.Length; tmp++)
                                        {
                                            if (iDtr[tmp]["doc_type"].ToString() == ihConstants.PHOTOADDENDUM_FILE)
                                                pgCount++;
                                        }
                                        //End: Calculates count for doc types
                                        imageName = new string[pgCount]; //Initialize the array with no. of images for the doc type

                                        pgCurrent = 0; 	//Temporary variable where count is increased when
                                        //the doc type is found, so that the name can be stored
                                        //in the right index

                                        //Rotate through all the images having doctype = Proposal Form
                                        for (int propCount = 0; propCount < iDtr.Length; propCount++)
                                        {
                                            if (iDtr[propCount]["doc_type"].ToString() == ihConstants.PHOTOADDENDUM_FILE)
                                            {
                                                DateTime dt = DateTime.Now;
                                                if (pgCurrent == 0)
                                                {
                                                    docType.Add(ihConstants.PHOTOADDENDUM_FILE);
                                                    multiPageFileName.Add(policyNumber + "_" + ihConstants.PHOTOADDENDUM_FILE + ".TIF");
                                                    docCount++;
                                                }
                                                //Changed on 19/09/2009 for managing file copying problem in FQC
                                                if (File.Exists(imagePath + "\\" + iDtr[propCount]["page_index_name"].ToString()) == false)
                                                {
                                                    imagePath = policyPath + "\\" + ihConstants._INDEXING_FOLDER;
                                                }
                                                imageName[pgCurrent] = tempPath + "\\" + iDtr[propCount]["page_index_name"].ToString();

                                                expImageCount = expImageCount + 1;
                                                policyTotPage = policyTotPage + 1;
                                                pgCountWhlErr = pgCountWhlErr + 1;
                                                proposalExists = true;

                                                DateTime edDt = DateTime.Now;
                                                TimeSpan tsp = edDt - dt;
                                                //Increment the variable if doctype is found
                                                pgCurrent++;
                                            }
                                            //MessageBox.Show(tsp.Milliseconds.ToString());
                                        }
                                        if (img.CombineTif(imageName, exportPath + "\\" + policyNumber + "_" + ihConstants.PHOTOADDENDUM_FILE + ".TIF") == false)
                                        {
                                            //exportTrans.Rollback();
                                            expBol = false;
                                            break;
                                        }
                                        pgCount = 0; //Holds the pagecount for a given doctype

                                        for (tmp = 0; tmp < iDtr.Length; tmp++)
                                        {
                                            if (iDtr[tmp]["doc_type"].ToString() == ihConstants.PROPOSALREVIEWSLIP_FILE)
                                                pgCount++;
                                        }
                                        //End: Calculates count for doc types
                                        imageName = new string[pgCount]; //Initialize the array with no. of images for the doc type

                                        pgCurrent = 0; 	//Temporary variable where count is increased when
                                        //the doc type is found, so that the name can be stored
                                        //in the right index

                                        //Rotate through all the images having doctype = Proposal Form
                                        for (int propCount = 0; propCount < iDtr.Length; propCount++)
                                        {
                                            if (iDtr[propCount]["doc_type"].ToString() == ihConstants.PROPOSALREVIEWSLIP_FILE)
                                            {
                                                DateTime dt = DateTime.Now;
                                                if (pgCurrent == 0)
                                                {
                                                    docType.Add(ihConstants.PROPOSALREVIEWSLIP_FILE);
                                                    multiPageFileName.Add(policyNumber + "_" + ihConstants.PROPOSALREVIEWSLIP_FILE + ".TIF");
                                                    docCount++;
                                                }
                                                //Changed on 19/09/2009 for managing file copying problem in FQC
                                                if (File.Exists(imagePath + "\\" + iDtr[propCount]["page_index_name"].ToString()) == false)
                                                {
                                                    imagePath = policyPath + "\\" + ihConstants._INDEXING_FOLDER;
                                                    tempPath = policyPath + "\\" + ihConstants._INDEXING_FOLDER;
                                                }
                                                imageName[pgCurrent] = tempPath + "\\" + iDtr[propCount]["page_index_name"].ToString();

                                                expImageCount = expImageCount + 1;
                                                policyTotPage = policyTotPage + 1;
                                                pgCountWhlErr = pgCountWhlErr + 1;
                                                proposalExists = true;

                                                DateTime edDt = DateTime.Now;
                                                TimeSpan tsp = edDt - dt;
                                                //Increment the variable if doctype is found
                                                pgCurrent++;
                                            }
                                            //MessageBox.Show(tsp.Milliseconds.ToString());
                                        }
                                        if (img.CombineTif(imageName, exportPath + "\\" + policyNumber + "_" + ihConstants.PROPOSALREVIEWSLIP_FILE + ".TIF") == false)
                                        {
                                            //exportTrans.Rollback();
                                            expBol = false;
                                            break;
                                        }
                                        pgCount = 0; //Holds the pagecount for a given doctype
                                        for (tmp = 0; tmp < iDtr.Length; tmp++)
                                        {
                                            if (iDtr[tmp]["doc_type"].ToString() == ihConstants.POLICYLOANS_FILE)
                                                pgCount++;
                                        }
                                        //End: Calculates count for doc types
                                        imageName = new string[pgCount]; //Initialize the array with no. of images for the doc type

                                        pgCurrent = 0; 	//Temporary variable where count is increased when
                                        //the doc type is found, so that the name can be stored
                                        //in the right index

                                        //Rotate through all the images having doctype = Proposal Form
                                        for (int propCount = 0; propCount < iDtr.Length; propCount++)
                                        {
                                            if (iDtr[propCount]["doc_type"].ToString() == ihConstants.POLICYLOANS_FILE)
                                            {
                                                DateTime dt = DateTime.Now;
                                                if (pgCurrent == 0)
                                                {
                                                    docType.Add(ihConstants.POLICYLOANS_FILE);
                                                    multiPageFileName.Add(policyNumber + "_" + ihConstants.POLICYLOANS_FILE + ".TIF");
                                                    docCount++;
                                                }
                                                //Changed on 19/09/2009 for managing file copying problem in FQC
                                                if (File.Exists(imagePath + "\\" + iDtr[propCount]["page_index_name"].ToString()) == false)
                                                {
                                                    imagePath = policyPath + "\\" + ihConstants._INDEXING_FOLDER;
                                                    tempPath = policyPath + "\\" + ihConstants._INDEXING_FOLDER;
                                                }
                                                imageName[pgCurrent] = tempPath + "\\" + iDtr[propCount]["page_index_name"].ToString();

                                                expImageCount = expImageCount + 1;
                                                policyTotPage = policyTotPage + 1;
                                                pgCountWhlErr = pgCountWhlErr + 1;
                                                proposalExists = true;

                                                DateTime edDt = DateTime.Now;
                                                TimeSpan tsp = edDt - dt;
                                                //Increment the variable if doctype is found
                                                pgCurrent++;
                                            }
                                            //MessageBox.Show(tsp.Milliseconds.ToString());
                                        }
                                        if (img.CombineTif(imageName, exportPath + "\\" + policyNumber + "_" + ihConstants.POLICYLOANS_FILE + ".TIF") == false)
                                        {
                                            //exportTrans.Rollback();
                                            expBol = false;
                                            break;
                                        }
                                        pgCount = 0; //Holds the pagecount for a given doctype
                                        for (tmp = 0; tmp < iDtr.Length; tmp++)
                                        {
                                            if (iDtr[tmp]["doc_type"].ToString() == ihConstants.MEDICALREPORT_FILE)
                                                pgCount++;
                                        }
                                        //End: Calculates count for doc types
                                        imageName = new string[pgCount]; //Initialize the array with no. of images for the doc type

                                        pgCurrent = 0; 	//Temporary variable where count is increased when
                                        //the doc type is found, so that the name can be stored
                                        //in the right index

                                        //Rotate through all the images having doctype = Proposal Form
                                        for (int propCount = 0; propCount < iDtr.Length; propCount++)
                                        {
                                            if (iDtr[propCount]["doc_type"].ToString() == ihConstants.MEDICALREPORT_FILE)
                                            {
                                                DateTime dt = DateTime.Now;
                                                if (pgCurrent == 0)
                                                {
                                                    docType.Add(ihConstants.MEDICALREPORT_FILE);
                                                    multiPageFileName.Add(policyNumber + "_" + ihConstants.MEDICALREPORT_FILE + ".TIF");
                                                    docCount++;
                                                }
                                                //Changed on 19/09/2009 for managing file copying problem in FQC
                                                if (File.Exists(imagePath + "\\" + iDtr[propCount]["page_index_name"].ToString()) == false)
                                                {
                                                    imagePath = policyPath + "\\" + ihConstants._INDEXING_FOLDER;
                                                }
                                                imageName[pgCurrent] = tempPath + "\\" + iDtr[propCount]["page_index_name"].ToString();

                                                expImageCount = expImageCount + 1;
                                                policyTotPage = policyTotPage + 1;
                                                pgCountWhlErr = pgCountWhlErr + 1;
                                                proposalExists = true;

                                                DateTime edDt = DateTime.Now;
                                                TimeSpan tsp = edDt - dt;
                                                //Increment the variable if doctype is found
                                                pgCurrent++;
                                            }
                                            //MessageBox.Show(tsp.Milliseconds.ToString());
                                        }
                                        if (img.CombineTif(imageName, exportPath + "\\" + policyNumber + "_" + ihConstants.MEDICALREPORT_FILE + ".TIF") == false)
                                        {
                                            //exportTrans.Rollback();
                                            expBol = false;
                                            break;
                                        }
                                        pgCount = 0; //Holds the pagecount for a given doctype
                                        for (tmp = 0; tmp < iDtr.Length; tmp++)
                                        {
                                            if (iDtr[tmp]["doc_type"].ToString() == ihConstants.NOMINATION_FILE)
                                                pgCount++;
                                        }
                                        //End: Calculates count for doc types
                                        imageName = new string[pgCount]; //Initialize the array with no. of images for the doc type

                                        pgCurrent = 0; 	//Temporary variable where count is increased when
                                        //the doc type is found, so that the name can be stored
                                        //in the right index

                                        //Rotate through all the images having doctype = Proposal Form
                                        for (int propCount = 0; propCount < iDtr.Length; propCount++)
                                        {
                                            if (iDtr[propCount]["doc_type"].ToString() == ihConstants.NOMINATION_FILE)
                                            {
                                                DateTime dt = DateTime.Now;
                                                if (pgCurrent == 0)
                                                {
                                                    docType.Add(ihConstants.NOMINATION_FILE);
                                                    multiPageFileName.Add(policyNumber + "_" + ihConstants.NOMINATION_FILE + ".TIF");
                                                    docCount++;
                                                }
                                                //Changed on 19/09/2009 for managing file copying problem in FQC
                                                if (File.Exists(imagePath + "\\" + iDtr[propCount]["page_index_name"].ToString()) == false)
                                                {
                                                    imagePath = policyPath + "\\" + ihConstants._INDEXING_FOLDER;
                                                }
                                                imageName[pgCurrent] = tempPath + "\\" + iDtr[propCount]["page_index_name"].ToString();

                                                expImageCount = expImageCount + 1;
                                                policyTotPage = policyTotPage + 1;
                                                pgCountWhlErr = pgCountWhlErr + 1;
                                                proposalExists = true;

                                                DateTime edDt = DateTime.Now;
                                                TimeSpan tsp = edDt - dt;
                                                //Increment the variable if doctype is found
                                                pgCurrent++;
                                            }
                                            //MessageBox.Show(tsp.Milliseconds.ToString());
                                        }
                                        if (img.CombineTif(imageName, exportPath + "\\" + policyNumber + "_" + ihConstants.NOMINATION_FILE + ".TIF") == false)
                                        {
                                            //exportTrans.Rollback();
                                            expBol = false;
                                            break;
                                        }
                                        pgCount = 0; //Holds the pagecount for a given doctype
                                        for (tmp = 0; tmp < iDtr.Length; tmp++)
                                        {
                                            if (iDtr[tmp]["doc_type"].ToString() == ihConstants.ASSIGNMENT_FILE)
                                                pgCount++;
                                        }
                                        //End: Calculates count for doc types
                                        imageName = new string[pgCount]; //Initialize the array with no. of images for the doc type

                                        pgCurrent = 0; 	//Temporary variable where count is increased when
                                        //the doc type is found, so that the name can be stored
                                        //in the right index

                                        //Rotate through all the images having doctype = Proposal Form
                                        for (int propCount = 0; propCount < iDtr.Length; propCount++)
                                        {
                                            if (iDtr[propCount]["doc_type"].ToString() == ihConstants.ASSIGNMENT_FILE)
                                            {
                                                DateTime dt = DateTime.Now;
                                                if (pgCurrent == 0)
                                                {
                                                    docType.Add(ihConstants.ASSIGNMENT_FILE);
                                                    multiPageFileName.Add(policyNumber + "_" + ihConstants.ASSIGNMENT_FILE + ".TIF");
                                                    docCount++;
                                                }
                                                //Changed on 19/09/2009 for managing file copying problem in FQC
                                                if (File.Exists(imagePath + "\\" + iDtr[propCount]["page_index_name"].ToString()) == false)
                                                {
                                                    imagePath = policyPath + "\\" + ihConstants._INDEXING_FOLDER;
                                                }
                                                imageName[pgCurrent] = tempPath + "\\" + iDtr[propCount]["page_index_name"].ToString();

                                                expImageCount = expImageCount + 1;
                                                policyTotPage = policyTotPage + 1;
                                                pgCountWhlErr = pgCountWhlErr + 1;
                                                proposalExists = true;

                                                DateTime edDt = DateTime.Now;
                                                TimeSpan tsp = edDt - dt;
                                                //Increment the variable if doctype is found
                                                pgCurrent++;
                                            }
                                            //MessageBox.Show(tsp.Milliseconds.ToString());
                                        }
                                        if (img.CombineTif(imageName, exportPath + "\\" + policyNumber + "_" + ihConstants.ASSIGNMENT_FILE + ".TIF") == false)
                                        {
                                            //exportTrans.Rollback();
                                            expBol = false;
                                            break;
                                        }
                                        pgCount = 0; //Holds the pagecount for a given doctype
                                        for (tmp = 0; tmp < iDtr.Length; tmp++)
                                        {
                                            if (iDtr[tmp]["doc_type"].ToString() == ihConstants.ALTERATION_FILE)
                                                pgCount++;
                                        }
                                        //End: Calculates count for doc types
                                        imageName = new string[pgCount]; //Initialize the array with no. of images for the doc type

                                        pgCurrent = 0; 	//Temporary variable where count is increased when
                                        //the doc type is found, so that the name can be stored
                                        //in the right index

                                        //Rotate through all the images having doctype = Proposal Form
                                        for (int propCount = 0; propCount < iDtr.Length; propCount++)
                                        {
                                            if (iDtr[propCount]["doc_type"].ToString() == ihConstants.ALTERATION_FILE)
                                            {
                                                DateTime dt = DateTime.Now;
                                                if (pgCurrent == 0)
                                                {
                                                    docType.Add(ihConstants.ALTERATION_FILE);
                                                    multiPageFileName.Add(policyNumber + "_" + ihConstants.ALTERATION_FILE + ".TIF");
                                                    docCount++;
                                                }
                                                //Changed on 19/09/2009 for managing file copying problem in FQC
                                                if (File.Exists(imagePath + "\\" + iDtr[propCount]["page_index_name"].ToString()) == false)
                                                {
                                                    imagePath = policyPath + "\\" + ihConstants._INDEXING_FOLDER;
                                                }
                                                imageName[pgCurrent] = tempPath + "\\" + iDtr[propCount]["page_index_name"].ToString();

                                                expImageCount = expImageCount + 1;
                                                policyTotPage = policyTotPage + 1;
                                                pgCountWhlErr = pgCountWhlErr + 1;
                                                proposalExists = true;

                                                DateTime edDt = DateTime.Now;
                                                TimeSpan tsp = edDt - dt;
                                                //Increment the variable if doctype is found
                                                pgCurrent++;
                                            }
                                            //MessageBox.Show(tsp.Milliseconds.ToString());
                                        }
                                        if (img.CombineTif(imageName, exportPath + "\\" + policyNumber + "_" + ihConstants.ALTERATION_FILE + ".TIF") == false)
                                        {
                                            //exportTrans.Rollback();
                                            expBol = false;
                                            break;
                                        }
                                        pgCount = 0; //Holds the pagecount for a given doctype
                                        for (tmp = 0; tmp < iDtr.Length; tmp++)
                                        {
                                            if (iDtr[tmp]["doc_type"].ToString() == ihConstants.CLAIMS_FILE)
                                                pgCount++;
                                        }
                                        //End: Calculates count for doc types
                                        imageName = new string[pgCount]; //Initialize the array with no. of images for the doc type

                                        pgCurrent = 0; 	//Temporary variable where count is increased when
                                        //the doc type is found, so that the name can be stored
                                        //in the right index

                                        //Rotate through all the images having doctype = Proposal Form
                                        for (int propCount = 0; propCount < iDtr.Length; propCount++)
                                        {
                                            if (iDtr[propCount]["doc_type"].ToString() == ihConstants.CLAIMS_FILE)
                                            {
                                                DateTime dt = DateTime.Now;
                                                if (pgCurrent == 0)
                                                {
                                                    docType.Add(ihConstants.CLAIMS_FILE);
                                                    multiPageFileName.Add(policyNumber + "_" + ihConstants.CLAIMS_FILE + ".TIF");
                                                    docCount++;
                                                }
                                                //Changed on 19/09/2009 for managing file copying problem in FQC
                                                if (File.Exists(imagePath + "\\" + iDtr[propCount]["page_index_name"].ToString()) == false)
                                                {
                                                    imagePath = policyPath + "\\" + ihConstants._INDEXING_FOLDER;
                                                }
                                                imageName[pgCurrent] = tempPath + "\\" + iDtr[propCount]["page_index_name"].ToString();

                                                expImageCount = expImageCount + 1;
                                                policyTotPage = policyTotPage + 1;
                                                pgCountWhlErr = pgCountWhlErr + 1;
                                                proposalExists = true;

                                                DateTime edDt = DateTime.Now;
                                                TimeSpan tsp = edDt - dt;
                                                //Increment the variable if doctype is found
                                                pgCurrent++;
                                            }
                                            //MessageBox.Show(tsp.Milliseconds.ToString());
                                        }
                                        if (img.CombineTif(imageName, exportPath + "\\" + policyNumber + "_" + ihConstants.CLAIMS_FILE + ".TIF") == false)
                                        {
                                            //exportTrans.Rollback();
                                            expBol = false;
                                            break;
                                        }
                                        pgCount = 0; //Holds the pagecount for a given doctype
                                        for (tmp = 0; tmp < iDtr.Length; tmp++)
                                        {
                                            if (iDtr[tmp]["doc_type"].ToString() == ihConstants.CORRESPONDENCE_FILE)
                                                pgCount++;
                                        }
                                        //End: Calculates count for doc types
                                        imageName = new string[pgCount]; //Initialize the array with no. of images for the doc type

                                        pgCurrent = 0; 	//Temporary variable where count is increased when
                                        //the doc type is found, so that the name can be stored
                                        //in the right index

                                        //Rotate through all the images having doctype = Proposal Form
                                        for (int propCount = 0; propCount < iDtr.Length; propCount++)
                                        {
                                            if (iDtr[propCount]["doc_type"].ToString() == ihConstants.CORRESPONDENCE_FILE)
                                            {
                                                DateTime dt = DateTime.Now;
                                                if (pgCurrent == 0)
                                                {
                                                    docType.Add(ihConstants.CORRESPONDENCE_FILE);
                                                    multiPageFileName.Add(policyNumber + "_" + ihConstants.CORRESPONDENCE_FILE + ".TIF");
                                                    docCount++;
                                                }
                                                //Changed on 19/09/2009 for managing file copying problem in FQC
                                                if (File.Exists(imagePath + "\\" + iDtr[propCount]["page_index_name"].ToString()) == false)
                                                {
                                                    imagePath = policyPath + "\\" + ihConstants._INDEXING_FOLDER;
                                                }
                                                imageName[pgCurrent] = tempPath + "\\" + iDtr[propCount]["page_index_name"].ToString();

                                                expImageCount = expImageCount + 1;
                                                policyTotPage = policyTotPage + 1;
                                                pgCountWhlErr = pgCountWhlErr + 1;
                                                proposalExists = true;

                                                DateTime edDt = DateTime.Now;
                                                TimeSpan tsp = edDt - dt;
                                                //Increment the variable if doctype is found
                                                pgCurrent++;
                                            }
                                            //MessageBox.Show(tsp.Milliseconds.ToString());
                                        }
                                        if (img.CombineTif(imageName, exportPath + "\\" + policyNumber + "_" + ihConstants.CORRESPONDENCE_FILE + ".TIF") == false)
                                        {
                                            //exportTrans.Rollback();
                                            expBol = false;
                                            break;
                                        }
                                        pgCount = 0; //Holds the pagecount for a given doctype
                                        for (tmp = 0; tmp < iDtr.Length; tmp++)
                                        {
                                            if (iDtr[tmp]["doc_type"].ToString() == ihConstants.PROPOSALENCLOSERS_FILE)
                                                pgCount++;
                                        }
                                        //End: Calculates count for doc types
                                        imageName = new string[pgCount]; //Initialize the array with no. of images for the doc type

                                        pgCurrent = 0; 	//Temporary variable where count is increased when
                                        //the doc type is found, so that the name can be stored
                                        //in the right index

                                        //Rotate through all the images having doctype = Proposal Form
                                        for (int propCount = 0; propCount < iDtr.Length; propCount++)
                                        {
                                            if (iDtr[propCount]["doc_type"].ToString() == ihConstants.PROPOSALENCLOSERS_FILE)
                                            {
                                                DateTime dt = DateTime.Now;
                                                if (pgCurrent == 0)
                                                {
                                                    docType.Add(ihConstants.PROPOSALENCLOSERS_FILE);
                                                    multiPageFileName.Add(policyNumber + "_" + ihConstants.PROPOSALENCLOSERS_FILE + ".TIF");
                                                    docCount++;
                                                }
                                                //Changed on 19/09/2009 for managing file copying problem in FQC
                                                if (File.Exists(imagePath + "\\" + iDtr[propCount]["page_index_name"].ToString()) == false)
                                                {
                                                    imagePath = policyPath + "\\" + ihConstants._INDEXING_FOLDER;
                                                }
                                                imageName[pgCurrent] = tempPath + "\\" + iDtr[propCount]["page_index_name"].ToString();

                                                expImageCount = expImageCount + 1;
                                                policyTotPage = policyTotPage + 1;
                                                pgCountWhlErr = pgCountWhlErr + 1;
                                                proposalExists = true;

                                                DateTime edDt = DateTime.Now;
                                                TimeSpan tsp = edDt - dt;
                                                //Increment the variable if doctype is found
                                                pgCurrent++;
                                            }
                                            //MessageBox.Show(tsp.Milliseconds.ToString());
                                        }
                                        if (img.CombineTif(imageName, exportPath + "\\" + policyNumber + "_" + ihConstants.PROPOSALENCLOSERS_FILE + ".TIF") == false)
                                        {
                                            //exportTrans.Rollback();
                                            expBol = false;
                                            break;
                                        }
                                        pgCount = 0; //Holds the pagecount for a given doctype
                                        for (tmp = 0; tmp < iDtr.Length; tmp++)
                                        {
                                            if (iDtr[tmp]["doc_type"].ToString() == ihConstants.POLICYBOND_FILE)
                                                pgCount++;
                                        }
                                        //End: Calculates count for doc types
                                        imageName = new string[pgCount]; //Initialize the array with no. of images for the doc type

                                        pgCurrent = 0; 	//Temporary variable where count is increased when
                                        //the doc type is found, so that the name can be stored
                                        //in the right index

                                        //Rotate through all the images having doctype = Proposal Form
                                        for (int propCount = 0; propCount < iDtr.Length; propCount++)
                                        {
                                            if (iDtr[propCount]["doc_type"].ToString() == ihConstants.POLICYBOND_FILE)
                                            {
                                                DateTime dt = DateTime.Now;
                                                if (pgCurrent == 0)
                                                {
                                                    docType.Add(ihConstants.POLICYBOND_FILE);
                                                    multiPageFileName.Add(policyNumber + "_" + ihConstants.POLICYBOND_FILE + ".TIF");
                                                    docCount++;
                                                }
                                                //Changed on 19/09/2009 for managing file copying problem in FQC
                                                if (File.Exists(imagePath + "\\" + iDtr[propCount]["page_index_name"].ToString()) == false)
                                                {
                                                    imagePath = policyPath + "\\" + ihConstants._INDEXING_FOLDER;
                                                }
                                                imageName[pgCurrent] = tempPath + "\\" + iDtr[propCount]["page_index_name"].ToString();

                                                expImageCount = expImageCount + 1;
                                                policyTotPage = policyTotPage + 1;
                                                pgCountWhlErr = pgCountWhlErr + 1;
                                                proposalExists = true;

                                                DateTime edDt = DateTime.Now;
                                                TimeSpan tsp = edDt - dt;
                                                //Increment the variable if doctype is found
                                                pgCurrent++;
                                            }
                                            //MessageBox.Show(tsp.Milliseconds.ToString());
                                        }
                                        if (img.CombineTif(imageName, exportPath + "\\" + policyNumber + "_" + ihConstants.POLICYBOND_FILE + ".TIF") == false)
                                        {
                                            //exportTrans.Rollback();
                                            expBol = false;
                                            break;
                                        }
                                        pgCount = 0; //Holds the pagecount for a given doctype
                                        for (tmp = 0; tmp < iDtr.Length; tmp++)
                                        {
                                            if (iDtr[tmp]["doc_type"].ToString() == ihConstants.SURRENDER_FILE)
                                                pgCount++;
                                        }
                                        //End: Calculates count for doc types
                                        imageName = new string[pgCount]; //Initialize the array with no. of images for the doc type

                                        pgCurrent = 0; 	//Temporary variable where count is increased when
                                        //the doc type is found, so that the name can be stored
                                        //in the right index

                                        //Rotate through all the images having doctype = Proposal Form
                                        for (int propCount = 0; propCount < iDtr.Length; propCount++)
                                        {
                                            if (iDtr[propCount]["doc_type"].ToString() == ihConstants.SURRENDER_FILE)
                                            {
                                                DateTime dt = DateTime.Now;
                                                if (pgCurrent == 0)
                                                {
                                                    docType.Add(ihConstants.SURRENDER_FILE);
                                                    multiPageFileName.Add(policyNumber + "_" + ihConstants.SURRENDER_FILE + ".TIF");
                                                    docCount++;
                                                }
                                                //Changed on 19/09/2009 for managing file copying problem in FQC
                                                if (File.Exists(imagePath + "\\" + iDtr[propCount]["page_index_name"].ToString()) == false)
                                                {
                                                    imagePath = policyPath + "\\" + ihConstants._INDEXING_FOLDER;
                                                }
                                                imageName[pgCurrent] = tempPath + "\\" + iDtr[propCount]["page_index_name"].ToString();

                                                expImageCount = expImageCount + 1;
                                                policyTotPage = policyTotPage + 1;
                                                pgCountWhlErr = pgCountWhlErr + 1;
                                                proposalExists = true;

                                                DateTime edDt = DateTime.Now;
                                                TimeSpan tsp = edDt - dt;
                                                //Increment the variable if doctype is found
                                                pgCurrent++;
                                            }
                                            //MessageBox.Show(tsp.Milliseconds.ToString());
                                        }
                                        if (img.CombineTif(imageName, exportPath + "\\" + policyNumber + "_" + ihConstants.SURRENDER_FILE + ".TIF") == false)
                                        {
                                            //exportTrans.Rollback();
                                            expBol = false;
                                            break;
                                        }
                                        pgCount = 0; //Holds the pagecount for a given doctype
                                        for (tmp = 0; tmp < iDtr.Length; tmp++)
                                        {
                                            if (iDtr[tmp]["doc_type"].ToString() == ihConstants.REVIVALS_FILE)
                                                pgCount++;
                                        }
                                        //End: Calculates count for doc types
                                        imageName = new string[pgCount]; //Initialize the array with no. of images for the doc type

                                        pgCurrent = 0; 	//Temporary variable where count is increased when
                                        //the doc type is found, so that the name can be stored
                                        //in the right index

                                        //Rotate through all the images having doctype = Proposal Form
                                        for (int propCount = 0; propCount < iDtr.Length; propCount++)
                                        {
                                            if (iDtr[propCount]["doc_type"].ToString() == ihConstants.REVIVALS_FILE)
                                            {
                                                DateTime dt = DateTime.Now;
                                                if (pgCurrent == 0)
                                                {
                                                    docType.Add(ihConstants.REVIVALS_FILE);
                                                    multiPageFileName.Add(policyNumber + "_" + ihConstants.REVIVALS_FILE + ".TIF");
                                                    docCount++;
                                                }
                                                //Changed on 19/09/2009 for managing file copying problem in FQC
                                                if (File.Exists(imagePath + "\\" + iDtr[propCount]["page_index_name"].ToString()) == false)
                                                {
                                                    imagePath = policyPath + "\\" + ihConstants._INDEXING_FOLDER;
                                                }
                                                imageName[pgCurrent] = tempPath + "\\" + iDtr[propCount]["page_index_name"].ToString();

                                                expImageCount = expImageCount + 1;
                                                policyTotPage = policyTotPage + 1;
                                                pgCountWhlErr = pgCountWhlErr + 1;
                                                proposalExists = true;

                                                DateTime edDt = DateTime.Now;
                                                TimeSpan tsp = edDt - dt;
                                                //Increment the variable if doctype is found
                                                pgCurrent++;
                                            }
                                            //MessageBox.Show(tsp.Milliseconds.ToString());
                                        }
                                        if (img.CombineTif(imageName, exportPath + "\\" + policyNumber + "_" + ihConstants.REVIVALS_FILE + ".TIF") == false)
                                        {
                                            //exportTrans.Rollback();
                                            expBol = false;
                                            break;
                                        }
                                        pgCount = 0; //Holds the pagecount for a given doctype
                                        for (tmp = 0; tmp < iDtr.Length; tmp++)
                                        {
                                            if (iDtr[tmp]["doc_type"].ToString() == ihConstants.SIGNATUREPAGE_FILE)
                                                pgCount++;
                                        }
                                        //End: Calculates count for doc types
                                        imageName = new string[pgCount]; //Initialize the array with no. of images for the doc type

                                        pgCurrent = 0; 	//Temporary variable where count is increased when
                                        //the doc type is found, so that the name can be stored
                                        //in the right index

                                        //Rotate through all the images having doctype = Proposal Form
                                        for (int propCount = 0; propCount < iDtr.Length; propCount++)
                                        {
                                            if (iDtr[propCount]["doc_type"].ToString() == ihConstants.SIGNATUREPAGE_FILE)
                                            {
                                                DateTime dt = DateTime.Now;
                                                if (pgCurrent == 0)
                                                {
                                                    docType.Add(ihConstants.SIGNATUREPAGE_FILE);
                                                    multiPageFileName.Add(policyNumber + "_" + ihConstants.SIGNATUREPAGE_FILE + ".TIF");
                                                    docCount++;
                                                }
                                                //Changed on 19/09/2009 for managing file copying problem in FQC
                                                if (File.Exists(imagePath + "\\" + iDtr[propCount]["page_index_name"].ToString()) == false)
                                                {
                                                    imagePath = policyPath + "\\" + ihConstants._INDEXING_FOLDER;
                                                }
                                                imageName[pgCurrent] = tempPath + "\\" + iDtr[propCount]["page_index_name"].ToString();

                                                expImageCount = expImageCount + 1;
                                                policyTotPage = policyTotPage + 1;
                                                pgCountWhlErr = pgCountWhlErr + 1;
                                                proposalExists = true;

                                                DateTime edDt = DateTime.Now;
                                                TimeSpan tsp = edDt - dt;
                                                //Increment the variable if doctype is found
                                                pgCurrent++;
                                            }
                                            //MessageBox.Show(tsp.Milliseconds.ToString());
                                        }
                                        if (img.CombineTif(imageName, exportPath + "\\" + policyNumber + "_" + ihConstants.SIGNATUREPAGE_FILE + ".TIF") == false)
                                        {
                                            //exportTrans.Rollback();
                                            expBol = false;
                                            break;
                                        }
                                        pgCount = 0; //Holds the pagecount for a given doctype
                                        for (tmp = 0; tmp < iDtr.Length; tmp++)
                                        {
                                            if (iDtr[tmp]["doc_type"].ToString() == ihConstants.OTHERS_FILE)
                                                pgCount++;
                                        }
                                        //End: Calculates count for doc types
                                        imageName = new string[pgCount]; //Initialize the array with no. of images for the doc type

                                        pgCurrent = 0; 	//Temporary variable where count is increased when
                                        //the doc type is found, so that the name can be stored
                                        //in the right index

                                        //Rotate through all the images having doctype = Proposal Form
                                        for (int propCount = 0; propCount < iDtr.Length; propCount++)
                                        {
                                            if (iDtr[propCount]["doc_type"].ToString() == ihConstants.OTHERS_FILE)
                                            {
                                                DateTime dt = DateTime.Now;
                                                if (pgCurrent == 0)
                                                {
                                                    docType.Add(ihConstants.OTHERS_FILE);
                                                    multiPageFileName.Add(policyNumber + "_" + ihConstants.OTHERS_FILE + ".TIF");
                                                    docCount++;
                                                }
                                                //Changed on 19/09/2009 for managing file copying problem in FQC
                                                if (File.Exists(imagePath + "\\" + iDtr[propCount]["page_index_name"].ToString()) == false)
                                                {
                                                    imagePath = policyPath + "\\" + ihConstants._INDEXING_FOLDER;
                                                }
                                                imageName[pgCurrent] = tempPath + "\\" + iDtr[propCount]["page_index_name"].ToString();

                                                expImageCount = expImageCount + 1;
                                                policyTotPage = policyTotPage + 1;
                                                pgCountWhlErr = pgCountWhlErr + 1;
                                                proposalExists = true;

                                                DateTime edDt = DateTime.Now;
                                                TimeSpan tsp = edDt - dt;
                                                //Increment the variable if doctype is found
                                                pgCurrent++;
                                            }
                                            //MessageBox.Show(tsp.Milliseconds.ToString());
                                        }
                                        if (img.CombineTif(imageName, exportPath + "\\" + policyNumber + "_" + ihConstants.OTHERS_FILE + ".TIF") == false)
                                        {
                                            //exportTrans.Rollback();
                                            expBol = false;
                                            break;
                                        }
                                        pgCount = 0;
                                        for (tmp = 0; tmp < iDtr.Length; tmp++)
                                        {
                                            if (iDtr[tmp]["doc_type"].ToString() == ihConstants.KYCDOCUMENT_FILE)
                                                pgCount++;
                                        }
                                        //End: Calculates count for doc types
                                        imageName = new string[pgCount]; //Initialize the array with no. of images for the doc type

                                        //the doc type is found, so that the name can be stored
                                        //in the right index
                                        pgCurrent = 0;
                                        //Rotate through all the images having doctype = Proposal Form
                                        for (int propCount = 0; propCount < iDtr.Length; propCount++)
                                        {
                                            if (iDtr[propCount]["doc_type"].ToString() == ihConstants.KYCDOCUMENT_FILE)
                                            {
                                                DateTime dt = DateTime.Now;
                                                if (pgCurrent == 0)
                                                {
                                                    docType.Add(ihConstants.KYCDOCUMENT_FILE);
                                                    multiPageFileName.Add(policyNumber + "_" + ihConstants.KYCDOCUMENT_FILE + ".TIF");
                                                    docCount++;
                                                }
                                                //Changed on 19/09/2009 for managing file copying problem in FQC
                                                if (File.Exists(imagePath + "\\" + iDtr[propCount]["page_index_name"].ToString()) == false)
                                                {
                                                    imagePath = policyPath + "\\" + ihConstants._INDEXING_FOLDER;
                                                }
                                                imageName[pgCurrent] = tempPath + "\\" + iDtr[propCount]["page_index_name"].ToString();

                                                expImageCount = expImageCount + 1;
                                                policyTotPage = policyTotPage + 1;
                                                pgCountWhlErr = pgCountWhlErr + 1;
                                                proposalExists = true;

                                                DateTime edDt = DateTime.Now;
                                                TimeSpan tsp = edDt - dt;
                                                //Increment the variable if doctype is found
                                                pgCurrent++;
                                            }
                                            //MessageBox.Show(tsp.Milliseconds.ToString());
                                        }
                                        if (img.CombineTif(imageName, exportPath + "\\" + policyNumber + "_" + ihConstants.KYCDOCUMENT_FILE + ".TIF") == false)
                                        {
                                            //exportTrans.Rollback();
                                            expBol = false;
                                            break;
                                        }

                                        pgCount = 0; //Holds the pagecount for a given doctype
                                        DocTypeCount = docCount.ToString().PadLeft(2, Convert.ToChar("0"));
                                        if (policyTotPage <= ihConstants._MAX_POLICY_PAGE_COUNT)
                                        {
                                            MessageBox.Show("Policy - " + policyNumber + " has less pages, aborting...");
                                            ///Rollback transaction
                                            //exportTrans.Rollback();
                                            expBol = false;
                                            break;
                                        }
                                        /*
                                        if ((proposalExists != true) || (signatureExists != true))
                                        {
                                            MessageBox.Show("Proposal form or signature page missing for the policy - " + policyNumber + ", aborting...");
                                            ///Rollback transaction
                                            exportTrans.Rollback();
                                            expBol = false;
                                            break;
                                        }
                                         * */

                                    }
                                    else
                                    {
                                        if ((status == (int)eSTATES.POLICY_MISSING))
                                        {
                                            boxno = "000";
                                            ImageCount = "000";
                                            DocTypeCount = "00";
                                            Scanuploadflag = "02";
                                            //exportPath = batchPath + "\\" + ihConstants._EXPORT_FOLDER + "\\" + vendorCode + "_" + divisionCode + "-" + branchCode.PadRight(4, Convert.ToChar(" ")) + "-" + batchCount + "_" + versionNumber + "\\" + policyNumber;
                                            //if (Directory.Exists(exportPath) == false)
                                            //{
                                            //    Directory.CreateDirectory(exportPath);
                                            //}
                                        }
                                        else
                                        {
                                            if ((status == (int)eSTATES.POLICY_INITIALIZED))
                                            {
                                                MessageBox.Show("Item " + policyNumber + " have not been scanned, aborting...");
                                                ///Rollback transaction
                                                //exportTrans.Rollback();
                                                expBol = false;
                                                break;
                                            }
                                            if ((status == (int)eSTATES.POLICY_NOT_INDEXED))
                                            {
                                                MessageBox.Show("Item " + policyNumber + " have not been indexed, aborting...");
                                                ///Rollback transaction
                                                //exportTrans.Rollback();
                                                expBol = false;
                                                break;
                                            }
                                            if ((status == (int)eSTATES.POLICY_SCANNED))
                                            {
                                                MessageBox.Show("QC not done for policy - " + policyNumber + " , aborting...");
                                                ///Rollback transaction
                                                //exportTrans.Rollback();
                                                expBol = false;
                                                break;
                                            }
                                            if ((status == (int)eSTATES.POLICY_EXCEPTION))
                                            {
                                                MessageBox.Show("LIC exception not cleared for the policy - " + policyNumber + " , aborting...");
                                                ///Rollback transaction
                                                //exportTrans.Rollback();
                                                expBol = false;
                                                break;
                                            }
                                            if ((status == (int)eSTATES.POLICY_QC))
                                            {
                                                MessageBox.Show("Indexing not done for policy - " + policyNumber + " , aborting...");
                                                ///Rollback transaction
                                                //exportTrans.Rollback();
                                                expBol = false;
                                                break;
                                            }
                                            if ((status == (int)eSTATES.POLICY_ON_HOLD))
                                            {
                                                ImageCount = "000";
                                                DocTypeCount = "00";
                                                Scanuploadflag = "02";
                                                exportPath = batchPath + "\\" + ihConstants._EXPORT_FOLDER + "\\" + vendorCode + "_" + divisionCode + "-" + branchCode.PadRight(4, Convert.ToChar(" ")) + "-" + batchCount + "_" + versionNumber + "\\" + policyNumber;
                                            }
                                        }
                                    }
                                    for (int p = 0; p < docType.Count; p++)
                                    {
                                        documentType = documentType + "," + docType[p].ToString() + "," + multiPageFileName[p].ToString(); ;
                                    }
                                    totPolicyCount = totPolicyCount + 1;
                                    if ((status != (int)eSTATES.POLICY_EXPORTED) || (chkReExport.Checked == true))
                                    {
                                        if ((status != (int)eSTATES.POLICY_MISSING) && ((status != (int)eSTATES.POLICY_ON_HOLD)) && (status != (int)eSTATES.POLICY_SCANNED) && (status != (int)eSTATES.POLICY_QC) && (status != (int)eSTATES.POLICY_INITIALIZED) && (status != (int)eSTATES.POLICY_EXCEPTION) && (status != (int)eSTATES.POLICY_NOT_INDEXED))
                                        {
                                            dtr[j]["status"] = eSTATES.POLICY_EXPORTED;
                                            ///For updating all image status in datarow
                                            //                                                for (int st = 0; st < iDtr.Length; st++)
                                            //                                                {
                                            //                                                    iDtr[st]["status"] = eSTATES.PAGE_EXPORTED;
                                            //                                                }
                                        }
                                        if (normalSerial > maxSerial)
                                        {
                                            sw.WriteLine(string.Concat
                                                         (serial_no + ","
                                                          , policyNumber.PadLeft(9, '0') + ","
                                                          , divisionCode + ","
                                                          , branchCode.PadRight(4, Convert.ToChar(" ")) + ","
                                                          , batchSerial + ","
                                                          , boxno + ","
                                                          , Scanuploadflag + ","
                                                          , scanneddate + ","
                                                          , ihConstants.INCREMENTEDSCAN + ","
                                                          , Policyholdername + ","
                                                          , DOB + ","
                                                          , dateofcommencement + ","
                                                          , cust_id.PadRight(16, Convert.ToChar(" ")) + ","
                                                          , ImageCount + ","
                                                          , DocTypeCount + ","
                                                          , rootPath
                                                          , documentType));
                                            if (Directory.Exists(tempPath))
                                            {
                                                Directory.Delete(tempPath, true);
                                            }
                                        }
                                        //txtMsg.Text = txtMsg.Text + "  Page : " + ImageCount + "\r\n";
                                        //txtMsg.SelectionStart = txtMsg.Text.Length;
                                        //txtMsg.ScrollToCaret();
                                        //txtMsg.Refresh();
                                        totImage = totImage + Convert.ToInt32(ImageCount);
                                        //toolStatus.Text = toolStatus.Text + " Page : " + ImageCount + "/" + totImage;
                                        Application.DoEvents();
                                        //if (tempPath != string.Empty)
                                        //    DeleteLocalFile(tempPath, exportPath);
                                    }
                                    //sw.Close();
                                    //sw.Dispose();
                                    //sw.Flush();
                                    imageDs.Dispose();
                                    docType.Clear();
                                    multiPageFileName.Clear();
                                    documentType = string.Empty;
                                    proposalExists = false;
                                    signatureExists = false;
                                    policyTotPage = 0;
                                    //exportTrans.Commit();
                                    DateTime endDt = DateTime.Now;
                                    TimeSpan tp = endDt - stDt;
                                    //MessageBox.Show("Total time-" + tp.Milliseconds);
                                }

                                img.Close();
                                img = IgrFactory.GetImagery(Constants.IGR_CLEARIMAGE);
                                expLog = new StreamWriter(appendPath + "\\" + ihConstants._EXPORT_FOLDER + "\\" + cmbProject.Text + "\\" + "Export_Log.txt", true);
                                expLog.WriteLine("Project Name - " + cmbrunNum.Text);
                                expLog.WriteLine("Batch Name - " + cmbProject.Text);
                                expLog.WriteLine("Box number - " + lvwExportList.Items[i].SubItems[0].Text.ToString());
                                expLog.WriteLine("Policy Exported - " + (expPolicyCount));
                                expLog.WriteLine("Images Exported - " + expImageCount);
                                if (expBol == true)
                                {
                                    ///Update box status
                                    pBox = new CtrlBox(Convert.ToInt32(cmbrunNum.SelectedValue.ToString()), Convert.ToInt32(cmbProject.SelectedValue.ToString()), boxno);
                                    box = new wfeBox(sqlCon, pBox);
                                    box.UpdateStatus(eSTATES.BOX_EXPORTED, exportTrans);

                                    //toolStatus.Text = "Box Number - " + lvwExportList.Items[i].SubItems[0].Text.ToString() + " Exported successfully........";

                                    txtMsg.Text = txtMsg.Text + "Exported successfully \r\n";
                                    txtMsg.Text = txtMsg.Text + "Summary........ \r\n";
                                    txtMsg.Text = txtMsg.Text + "Total Policy : " + expPolicyCount + " Total Images : " + expImageCount + "\r\n";
                                    txtMsg.SelectionStart = txtMsg.Text.Length;
                                    txtMsg.ScrollToCaret();
                                    txtMsg.Refresh();
                                    sw.Close();
                                    if (Directory.Exists(tempPath))
                                    {
                                        Directory.Delete(tempPath, true);
                                    }

                                }
                                else
                                {
                                    sw.Close();
                                    expLog.WriteLine("Error in policy - " + policyNumber);
                                    expLog.Close();

                                    // toolStatus.Text = "Error while exporting selected box.........";
                                    //exportTrans.Rollback();
                                    if (Directory.Exists(exportPath))
                                    {
                                        Directory.Delete(exportPath, true);
                                    }
                                    txtMsg.Text = txtMsg.Text + "\r\n" + "Export error in : " + policyNumber + "\r\n";
                                    txtMsg.Text = txtMsg.Text + "Summary........ \r\n";
                                    txtMsg.Text = txtMsg.Text + "Total Policy : " + (expPolicyCount - 1) + " Total Images Exported : " + (expImageCount - pgCountWhlErr) + "\r\n";
                                    txtMsg.SelectionStart = txtMsg.Text.Length;
                                    txtMsg.ScrollToCaret();
                                    txtMsg.Refresh();
                                    //if ((appendPath != string.Empty) && (exportPath != string.Empty))
                                    //{
                                    //    DeleteLocalFile(appendPath, exportPath);
                                    //    break;
                                    //}
                                    if (Directory.Exists(tempPath))
                                    {
                                        Directory.Delete(tempPath, true);
                                    }
                                    break;
                                }
                                expLog.Close();
                                sw.Close();
                            }
                        }
                    }
                    //update policy_master set status = dataset.tables[0].rows[i]["status"] where policy_master.proj_key=x and batch_key=y and box_number=z and policy_number=p;
                    if ((pAdp != null) && (pDs != null))
                    {
                        //                            OdbcCommandBuilder pCommandBuilder = new OdbcCommandBuilder(pAdp);
                        //                            pAdp.UpdateCommand = pCommandBuilder.GetUpdateCommand();
                        //                            pAdp.Update(pDs.Tables[0]);
                        UpdateAllPolicy(pDs);
                    }
                    //                                    ///Batch update for Image
                    //                                    if ((iAdp != null) && (iDs != null))
                    //                                    {
                    //                                        OdbcCommandBuilder iCommandBuilder = new OdbcCommandBuilder(iAdp);
                    //                                        iAdp.UpdateCommand = iCommandBuilder.GetUpdateCommand();
                    //                                        iAdp.Update(iDs.Tables[0]);
                    //                                    }
                    //expSqlCon.Close();
                    DateTime endDtTot = DateTime.Now;
                    TimeSpan tspTot = endDtTot - stDt;
                    MessageBox.Show("Total time - " + tspTot.Minutes);
                }
                else
                {
                    MessageBox.Show("This batch is not ready for UAT, export is not possible....");
                }
            }
            catch (Exception ex)
            {
                error = ex.Message;
                MessageBox.Show("Error while exporting data...... " + error, "Export Error", MessageBoxButtons.OK);
                //exportTrans.Rollback();
                if (Directory.Exists(exportPath))
                {
                    Directory.Delete(exportPath, true);
                }
                ///Batch update for policy
                //                if ((pAdp != null) && (pDs != null))
                //                {
                //                    OdbcCommandBuilder pCommandBuilder = new OdbcCommandBuilder(pAdp);
                //                    pAdp.UpdateCommand = pCommandBuilder.GetUpdateCommand();
                //                    pAdp.Update(pDs.Tables[0]);
                //                }
                UpdateAllPolicy(pDs);
                if (Directory.Exists(tempPath))
                {
                    Directory.Delete(tempPath, true);
                }
                //                ///Batch update for Image
                //                if ((iAdp != null) && (iDs != null))
                //                {
                //                    OdbcCommandBuilder iCommandBuilder = new OdbcCommandBuilder(iAdp);
                //                    iAdp.UpdateCommand = iCommandBuilder.GetUpdateCommand();
                //                    iAdp.Update(iDs.Tables[0]);
                //                }
                expSqlCon.Close();
                sw.Close();
                stateLog = new MemoryStream();
                tmpWrite = new System.Text.ASCIIEncoding().GetBytes("Project-" + cmbrunNum.Text + " ,Batch-" + cmbProject.Text + " ,Box number-" + boxno + " ,Policy number-" + policyNumber + "\n");
                stateLog.Write(tmpWrite, 0, tmpWrite.Length);
                exMailLog.Log(ex, this);
                //DeleteLocalFile(appendPath, exportPath);
            }
            finally
            {
                if (sw != null)
                {
                    sw.Close();
                }
                PopulateBox();
                //if ((fileName != string.Empty) && (batchPath != string.Empty))
                //{
                //    File.Copy(".\\" + ihConstants._LOCAL_APPEND_TEXT_FOLDER + "\\" + fileName, batchPath + "\\" + ihConstants._EXPORT_FOLDER + "\\" + fileName);
                //    File.Delete(".\\" + ihConstants._LOCAL_APPEND_TEXT_FOLDER + "\\" + fileName);
                //}
            }
        }
        private void UpdateAllPolicy(DataSet pDs)
        {
            CtrlPolicy exppPolicy;
            wfePolicy expwPolicy;
            string policyNo;
            string boxNo;
            for (int i = 0; i < pDs.Tables[0].Rows.Count; i++)
            {
                policyNo = pDs.Tables[0].Rows[i]["policy_number"].ToString();
                boxNo = pDs.Tables[0].Rows[i]["box_number"].ToString();
                exppPolicy = new CtrlPolicy(Convert.ToInt32(cmbrunNum.SelectedValue.ToString()), Convert.ToInt32(cmbProject.SelectedValue.ToString()), boxNo.ToString(), policyNo);
                expwPolicy = new wfePolicy(sqlCon, exppPolicy);
                expwPolicy.UpdateStatus(Convert.ToInt32(pDs.Tables[0].Rows[i]["status"].ToString()), crd);
            }
        }
        /*
        void BatchUpdate(OdbcDataAdapter prmPAdap, OdbcDataAdapter prmIAdap)
        {
            ///Batch update for policy
            OdbcCommandBuilder pCommandBuilder = new OdbcCommandBuilder(prmPAdap);
            prmPAdap.UpdateCommand = pCommandBuilder.GetUpdateCommand();
            prmPAdap.Update(pDs.Tables[0]);

            ///Batch update for Image
            OdbcCommandBuilder iCommandBuilder = new OdbcCommandBuilder(iAdp);
            iAdp.UpdateCommand = iCommandBuilder.GetUpdateCommand();
            iAdp.Update(iDs.Tables[0]);
        }
         */
        void DeleteLocalFile(string pSourcePath, string pDestPath)
        {
            string[] replFiles = Directory.GetFiles(pSourcePath);
            try
            {
                if (replFiles.Length > 0)
                {
                    for (int i = 0; i < replFiles.Length; i++)
                    {
                        File.Copy(replFiles[i], pDestPath + "\\" + Path.GetFileName(replFiles[i]), false);
                        //File.Delete(replFiles[i]);
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        void AeExportFormClosing(object sender, FormClosingEventArgs e)
        {
            //sqlCon.Close();
        }

        private void cmdCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        void CmbBatchSelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void cmdSave_Click(object sender, EventArgs e)
        {
            SaveFileDialog svFile = new SaveFileDialog();
            Stream myStream;
            svFile.Filter = "Text files (*.txt)|*.txt";
            svFile.FileName = cmbProject.Text + "_Export_Result.txt";
            svFile.FilterIndex = 2;
            svFile.RestoreDirectory = true;

            if (svFile.ShowDialog() == DialogResult.OK)
            {
                if ((myStream = svFile.OpenFile()) != null)
                {
                    StreamWriter wText = new StreamWriter(myStream);
                    wText.Write(txtMsg.Text);
                    wText.Flush();
                    wText.Close();
                    myStream.Close();
                }
            }
        }

        private void cmdValidate_Click(object sender, EventArgs e)
        {

            string boxno = string.Empty;
            ArrayList arrPolicy = new ArrayList();
            wQuery pQuery = new ihwQuery(sqlCon);
            string policyNumber = string.Empty;
            DataTable policyDtls = new DataTable();
            DataTable imageDs = new DataTable();
            string imagePath;

            string ImageCount = string.Empty;
            string DocTypeCount = string.Empty;
            string[] imageName;
            string batchPath = string.Empty;
            string exportPath = string.Empty;
            int status;
            string policyPath;
            //string imagePath;
            string fileName = string.Empty;
            string documentType = null;
            //string FileName=null;
            string rootPath;
            exportFile = new FileorFolder();
            ArrayList docType = new ArrayList();
            ArrayList multiPageFileName = new ArrayList();
            OdbcTransaction exportTrans = null;
            bool expBol = true;
            OdbcConnection expSqlCon = null;
            int totPolicyCount = 0;
            string serial_no;
            int policyTotPage = 0;
            bool proposalExists;
            bool signatureExists;
            int pgCountWhlErr = 0;
            string appendPath = string.Empty;
            int tmp = 0;
            OdbcDataAdapter pAdp = new OdbcDataAdapter();
            OdbcDataAdapter iAdp = null;
            DataSet pDs = new DataSet();
            DataSet rDs = new DataSet();
            DataSet iDs = null;
            int maxSerial = 0;
            int normalSerial = 0;
            string[] lastLine = null;
            string tempPath = string.Empty;
            int validity = 0;
            //System.Runtime.GCSettings.LatencyMode = System.Runtime.GCLatencyMode.LowLatency;
            wBatch = new wfeBatch(sqlCon);
            batchPath = wBatch.GetPath(Convert.ToInt32(cmbrunNum.SelectedValue.ToString()), Convert.ToInt32(cmbProject.SelectedValue.ToString()));

            if ((wBatch.GetBatchStatus(Convert.ToInt32(cmbProject.SelectedValue.ToString())) == (int)eSTATES.BATCH_READY_FOR_UAT) || (wBatch.GetBatchStatus(Convert.ToInt32(cmbProject.SelectedValue.ToString())) != (int)eSTATES.BATCH_READY_FOR_UAT))
            {
                DateTime stDt = DateTime.Now;
                txtMsg.Text = "Batch Number : \r\n";
                txtMsg.Text = txtMsg.Text + cmbProject.Text + "\r\n";
                txtMsg.SelectionStart = txtMsg.Text.Length;
                txtMsg.ScrollToCaret();
                txtMsg.Refresh();

                tblExp.SelectedIndex = 1;
                DateTime stDtTot = DateTime.Now;
                NovaNet.Utils.dbCon dbcon;
                dbcon = new NovaNet.Utils.dbCon();
                //expSqlCon = dbcon.Connect();

                ctrlBox = new CtrlBox(Convert.ToInt32(cmbrunNum.SelectedValue.ToString()), Convert.ToInt32(cmbProject.SelectedValue.ToString()), "0");
                box = new wfeBox(sqlCon, ctrlBox);
                policy = new wfePolicy(sqlCon);

                pDs = policy.GetAllPolicyDetails(box, out pAdp);
                maxSerial = 0;

                for (int i = 0; i < lvwExportList.Items.Count; i++) //Counter for Box
                {
                    if (lvwExportList.Items[i].Checked == true)
                    {
                        if ((lvwExportList.Items[i].SubItems[1].Text.ToString() == "0") || (lvwExportList.Items[i].SubItems[1].Text.ToString() != "0"))
                        {
                            DataRow[] dtr = null;
                            DataRow[] pDtR = new DataRow[1];
                            state = new eSTATES[0];
                            if (pDs.Tables.Count > 0)
                            {

                                if (pDs.Tables[0].Rows.Count > 0)
                                {

                                    dtr = pDs.Tables[0].Select("proj_key = " + Convert.ToInt32(cmbrunNum.SelectedValue.ToString()) + " and batch_key=" + Convert.ToInt32(cmbProject.SelectedValue.ToString()) + " and box_number=" + Convert.ToInt32(lvwExportList.Items[i].SubItems[0].Text.ToString()));
                                    //rDtR = rDs.Tables[0].Select("policy_no=" + policyNumber);
                                }
                            }
                            boxno = lvwExportList.Items[i].SubItems[0].Text.ToString().PadLeft(3, Convert.ToChar("0"));
                            //ctrlBox = new CtrlBox(Convert.ToInt32(cmbProject.SelectedValue.ToString()), Convert.ToInt32(cmbBatch.SelectedValue.ToString()), Convert.ToInt32(lvwExportList.Items[i].SubItems[0].Text.ToString()));
                            //box = new wfeBox(sqlCon, ctrlBox);
                            //arrPolicy = pQuery.GetItems(eITEMS.POLICY, state, box);
                            expImageCount = 0;
                            expPolicyCount = 0;
                            pgCountWhlErr = 0;
                            expBol = true;

                            txtMsg.Text = txtMsg.Text + "Wait while validating box : ";
                            txtMsg.Text = txtMsg.Text + boxno + "\r\n";
                            txtMsg.SelectionStart = txtMsg.Text.Length;
                            txtMsg.ScrollToCaret();
                            txtMsg.Refresh();

                            iAdp = new OdbcDataAdapter();
                            ///For all policy details

                            //drelPolicyRaw = SetRelationPolicyRawData(pDs, rDs);
                            pImage = new CtrlImage(Convert.ToInt32(cmbrunNum.SelectedValue.ToString()), Convert.ToInt32(cmbProject.SelectedValue.ToString()), boxno.ToString(), "0", string.Empty, string.Empty);
                            wImage = new wfeImage(sqlCon, pImage);
                            iDs = new DataSet();
                            iDs = wImage.GetAllImages(out iAdp);

                            for (int j = 0; j < dtr.Length; j++) //Loop within box for all the policies
                            {
                                int docCount = 0;
                                validity = 0;
                                DataRow[] iDtr = null;
                                //expBol = true;
                                pgCountWhlErr = 0;
                                //exportTrans = expSqlCon.BeginTransaction();
                                boxno = lvwExportList.Items[i].SubItems[0].Text.ToString().PadLeft(3, Convert.ToChar("0"));
                                expPolicyCount = expPolicyCount + 1;
                                //imageDs = new DataSet();
                                //ctrPol = (CtrlPolicy)arrPolicy[j];
                                policyNumber = dtr[j]["policy_number"].ToString(); //ctrPol.PolicyNumber.ToString();
                                if (pDs.Tables.Count > 0)
                                {
                                    if (pDs.Tables[0].Rows.Count > 0)
                                    {
                                        policyDtls = pDs.Tables[0];
                                        pDtR = policyDtls.Select("proj_key = " + Convert.ToInt32(cmbrunNum.SelectedValue.ToString()) + " and batch_key=" + Convert.ToInt32(cmbProject.SelectedValue.ToString()) + " and box_number=" + Convert.ToInt32(lvwExportList.Items[i].SubItems[0].Text.ToString()) + " and policy_number=" + policyNumber);
                                    }
                                }


                                policy = new wfePolicy(sqlCon, ctrPol);

                                policyPath = batchPath + "\\" + lvwExportList.Items[i].SubItems[0].Text.ToString() + "\\" + policyNumber;

                                status = Convert.ToInt32(pDtR[0]["status"].ToString());
                                if ((status != (int)eSTATES.POLICY_EXPORTED) && (chkReExport.Checked != true))
                                {
                                    //toolStatus.Text = "Box: " + lvwExportList.Items[i].SubItems[0].Text.ToString() + " policy - " + policyNumber;
                                    txtMsg.Text = txtMsg.Text + " Policy: " + policyNumber;
                                    txtMsg.SelectionStart = txtMsg.Text.Length;
                                    txtMsg.ScrollToCaret();
                                    txtMsg.Refresh();
                                    Application.DoEvents();
                                }
                                if (chkReExport.Checked == true)
                                {
                                    // toolStatus.Text = "Box: " + lvwExportList.Items[i].SubItems[0].Text.ToString() + " policy - " + policyNumber;
                                    txtMsg.Text = txtMsg.Text + " Policy: " + policyNumber;
                                    txtMsg.SelectionStart = txtMsg.Text.Length;
                                    txtMsg.ScrollToCaret();
                                    txtMsg.Refresh();
                                    Application.DoEvents();
                                }
                                if ((status != (int)eSTATES.POLICY_MISSING) && ((status != (int)eSTATES.POLICY_ON_HOLD)) && (status != (int)eSTATES.POLICY_SCANNED) && (status != (int)eSTATES.POLICY_QC) && (status != (int)eSTATES.POLICY_INITIALIZED) && (status != (int)eSTATES.POLICY_EXCEPTION) && (status != (int)eSTATES.POLICY_NOT_INDEXED) && ((status != (int)eSTATES.POLICY_EXPORTED) || chkReExport.Checked == true))
                                {

                                    /////For update policy status
                                    //CtrlPolicy exppPolicy = new CtrlPolicy(Convert.ToInt32(cmbProject.SelectedValue.ToString()), Convert.ToInt32(cmbBatch.SelectedValue.ToString()), Convert.ToInt32(lvwExportList.Items[i].SubItems[0].Text.ToString()), Convert.ToInt32(policyNumber));
                                    //wfePolicy expwPolicy = new wfePolicy(sqlCon, exppPolicy);
                                    //expwPolicy.UpdateStatus(eSTATES.POLICY_EXPORTED, crd);

                                    /////Update image status
                                    //CtrlImage exppImage = new CtrlImage(Convert.ToInt32(cmbProject.SelectedValue.ToString()), Convert.ToInt32(cmbBatch.SelectedValue.ToString()), boxNo.ToString(), Convert.ToInt32(policyNumber), string.Empty, string.Empty);
                                    //wfeImage expwImage = new wfeImage(expSqlCon, exppImage);
                                    //expwImage.UpdateAllImageStatus(eSTATES.PAGE_EXPORTED, crd, exportTrans);

                                    if (status == (int)eSTATES.POLICY_INDEXED)
                                    {
                                        imagePath = policyPath + "\\" + ihConstants._INDEXING_FOLDER;
                                    }
                                    else if (status == (int)eSTATES.POLICY_FQC)
                                    {
                                        imagePath = policyPath + "\\" + ihConstants._FQC_FOLDER;
                                    }
                                    else
                                    {
                                        imagePath = policyPath + "\\" + ihConstants._FQC_FOLDER;
                                    }
                                    if (Directory.Exists(policyPath + "\\" + ihConstants._FQC_FOLDER))
                                    {
                                        imagePath = policyPath + "\\" + ihConstants._FQC_FOLDER;
                                    }
                                    else
                                    {
                                        imagePath = policyPath + "\\" + ihConstants._INDEXING_FOLDER;
                                    }
                                    //exportPath = appendPath + "\\" + ihConstants._EXPORT_FOLDER + "\\" + cmbBatch.Text + "\\" + vendorCode + "_" + divisionCode + "-" + branchCode.PadRight(4, Convert.ToChar(" ")) + "-" + batchCount + "_" + versionNumber + "\\" + policyNumber;
                                    if (iDs.Tables.Count > 0)
                                    {
                                        if (iDs.Tables[0].Rows.Count > 0)
                                        {
                                            //imageDs = iDs.Tables[0];
                                            iDtr = iDs.Tables[0].Select("proj_key = " + Convert.ToInt32(cmbrunNum.SelectedValue.ToString()) + " and batch_key=" + Convert.ToInt32(cmbProject.SelectedValue.ToString()) + " and box_number=" + Convert.ToInt32(lvwExportList.Items[i].SubItems[0].Text.ToString()) + " and policy_number=" + policyNumber);
                                        }
                                    }

                                    ImageCount = iDtr.Length.ToString().PadLeft(3, Convert.ToChar("0"));

                                    int pgCount = 0; //Holds the pagecount for a given doctype
                                    for (tmp = 0; tmp < iDtr.Length; tmp++)
                                    {
                                        if (iDtr[tmp]["doc_type"].ToString() == ihConstants.PROPOSALFORM_FILE)
                                            pgCount++;
                                    }

                                    //End: Calculates count for doc types
                                    imageName = new string[pgCount]; //Initialize the array with no. of images for the doc type

                                    int pgCurrent = 0; 	//Temporary variable where count is increased when
                                    //the doc type is found, so that the name can be stored
                                    //in the right index

                                    //Rotate through all the images having doctype = Proposal Form

                                    for (int propCount = 0; propCount < iDtr.Length; propCount++)
                                    {
                                        if (iDtr[propCount]["doc_type"].ToString() == ihConstants.PROPOSALFORM_FILE)
                                        {
                                            DateTime dt = DateTime.Now;
                                            if (pgCurrent == 0)
                                            {
                                                docType.Add(ihConstants.PROPOSALFORM_FILE);
                                                multiPageFileName.Add(policyNumber + "_" + ihConstants.PROPOSALFORM_FILE + ".TIF");
                                                docCount++;
                                            }
                                            //Changed on 19/09/2009 for managing file copying problem in FQC
                                            if (File.Exists(imagePath + "\\" + iDtr[propCount]["page_index_name"].ToString()) == false)
                                            {
                                                imagePath = policyPath + "\\" + ihConstants._INDEXING_FOLDER;
                                            }
                                            //imageName[pgCurrent] = tempPath + "\\" + iDtr[propCount]["page_index_name"].ToString();
                                            if (File.Exists(imagePath + "\\" + iDtr[propCount]["page_index_name"].ToString()) == false)
                                            {
                                                txtMsg.Text = txtMsg.Text + " Image invalid: " + iDtr[propCount]["page_index_name"].ToString();
                                                txtMsg.SelectionStart = txtMsg.Text.Length;
                                                txtMsg.ScrollToCaret();
                                                txtMsg.Refresh();
                                                validity = validity + 1;
                                            }
                                            expImageCount = expImageCount + 1;
                                            policyTotPage = policyTotPage + 1;
                                            pgCountWhlErr = pgCountWhlErr + 1;
                                            proposalExists = true;

                                            DateTime edDt = DateTime.Now;
                                            TimeSpan tsp = edDt - dt;
                                            //Increment the variable if doctype is found
                                            pgCurrent++;
                                        }
                                        //MessageBox.Show(tsp.Milliseconds.ToString());
                                    }

                                    pgCount = 0; //Holds the pagecount for a given doctype
                                    for (tmp = 0; tmp < iDtr.Length; tmp++)
                                    {
                                        if (iDtr[tmp]["doc_type"].ToString() == ihConstants.PHOTOADDENDUM_FILE)
                                            pgCount++;
                                    }

                                    //End: Calculates count for doc types
                                    imageName = new string[pgCount]; //Initialize the array with no. of images for the doc type

                                    pgCurrent = 0; 	//Temporary variable where count is increased when
                                    //the doc type is found, so that the name can be stored
                                    //in the right index

                                    //Rotate through all the images having doctype = Proposal Form
                                    for (int propCount = 0; propCount < iDtr.Length; propCount++)
                                    {
                                        if (iDtr[propCount]["doc_type"].ToString() == ihConstants.PHOTOADDENDUM_FILE)
                                        {
                                            DateTime dt = DateTime.Now;
                                            if (pgCurrent == 0)
                                            {
                                                docType.Add(ihConstants.PHOTOADDENDUM_FILE);
                                                multiPageFileName.Add(policyNumber + "_" + ihConstants.PHOTOADDENDUM_FILE + ".TIF");
                                                docCount++;
                                            }
                                            //Changed on 19/09/2009 for managing file copying problem in FQC
                                            if (File.Exists(imagePath + "\\" + iDtr[propCount]["page_index_name"].ToString()) == false)
                                            {
                                                imagePath = policyPath + "\\" + ihConstants._INDEXING_FOLDER;
                                                //tempPath = policyPath + "\\" + ihConstants._INDEXING_FOLDER;
                                            }
                                            //imageName[pgCurrent] = tempPath + "\\" + iDtr[propCount]["page_index_name"].ToString();
                                            if (File.Exists(imagePath + "\\" + iDtr[propCount]["page_index_name"].ToString()) == false)
                                            {
                                                txtMsg.Text = txtMsg.Text + " Image invalid: " + iDtr[propCount]["page_index_name"].ToString();
                                                txtMsg.SelectionStart = txtMsg.Text.Length;
                                                txtMsg.ScrollToCaret();
                                                txtMsg.Refresh();
                                                validity = validity + 1;
                                            }
                                            expImageCount = expImageCount + 1;
                                            policyTotPage = policyTotPage + 1;
                                            pgCountWhlErr = pgCountWhlErr + 1;
                                            proposalExists = true;

                                            DateTime edDt = DateTime.Now;
                                            TimeSpan tsp = edDt - dt;
                                            //Increment the variable if doctype is found
                                            pgCurrent++;
                                        }
                                        //MessageBox.Show(tsp.Milliseconds.ToString());
                                    }

                                    pgCount = 0; //Holds the pagecount for a given doctype
                                    for (tmp = 0; tmp < iDtr.Length; tmp++)
                                    {
                                        if (iDtr[tmp]["doc_type"].ToString() == ihConstants.KYCDOCUMENT_FILE)
                                            pgCount++;
                                    }

                                    //End: Calculates count for doc types
                                    imageName = new string[pgCount]; //Initialize the array with no. of images for the doc type

                                    pgCurrent = 0; 	//Temporary variable where count is increased when
                                    //the doc type is found, so that the name can be stored
                                    //in the right index

                                    //Rotate through all the images having doctype = Proposal Form
                                    for (int propCount = 0; propCount < iDtr.Length; propCount++)
                                    {
                                        if (iDtr[propCount]["doc_type"].ToString() == ihConstants.KYCDOCUMENT_FILE)
                                        {
                                            DateTime dt = DateTime.Now;
                                            if (pgCurrent == 0)
                                            {
                                                docType.Add(ihConstants.KYCDOCUMENT_FILE);
                                                multiPageFileName.Add(policyNumber + "_" + ihConstants.KYCDOCUMENT_FILE + ".TIF");
                                                docCount++;
                                            }
                                            //Changed on 19/09/2009 for managing file copying problem in FQC
                                            if (File.Exists(imagePath + "\\" + iDtr[propCount]["page_index_name"].ToString()) == false)
                                            {
                                                imagePath = policyPath + "\\" + ihConstants._INDEXING_FOLDER;
                                                //tempPath = policyPath + "\\" + ihConstants._INDEXING_FOLDER;
                                            }
                                            //imageName[pgCurrent] = tempPath + "\\" + iDtr[propCount]["page_index_name"].ToString();
                                            if (File.Exists(imagePath + "\\" + iDtr[propCount]["page_index_name"].ToString()) == false)
                                            {
                                                txtMsg.Text = txtMsg.Text + " Image invalid: " + iDtr[propCount]["page_index_name"].ToString();
                                                txtMsg.SelectionStart = txtMsg.Text.Length;
                                                txtMsg.ScrollToCaret();
                                                txtMsg.Refresh();
                                                validity = validity + 1;
                                            }
                                            expImageCount = expImageCount + 1;
                                            policyTotPage = policyTotPage + 1;
                                            pgCountWhlErr = pgCountWhlErr + 1;
                                            proposalExists = true;

                                            DateTime edDt = DateTime.Now;
                                            TimeSpan tsp = edDt - dt;
                                            //Increment the variable if doctype is found
                                            pgCurrent++;
                                        }
                                        //MessageBox.Show(tsp.Milliseconds.ToString());
                                    }

                                    pgCount = 0; //Holds the pagecount for a given doctype
                                    for (tmp = 0; tmp < iDtr.Length; tmp++)
                                    {
                                        if (iDtr[tmp]["doc_type"].ToString() == ihConstants.PROPOSALREVIEWSLIP_FILE)
                                            pgCount++;
                                    }

                                    //End: Calculates count for doc types
                                    imageName = new string[pgCount]; //Initialize the array with no. of images for the doc type

                                    pgCurrent = 0; 	//Temporary variable where count is increased when
                                    //the doc type is found, so that the name can be stored
                                    //in the right index

                                    //Rotate through all the images having doctype = Proposal Form
                                    for (int propCount = 0; propCount < iDtr.Length; propCount++)
                                    {
                                        if (iDtr[propCount]["doc_type"].ToString() == ihConstants.PROPOSALREVIEWSLIP_FILE)
                                        {
                                            DateTime dt = DateTime.Now;
                                            if (pgCurrent == 0)
                                            {
                                                docType.Add(ihConstants.PROPOSALREVIEWSLIP_FILE);
                                                multiPageFileName.Add(policyNumber + "_" + ihConstants.PROPOSALREVIEWSLIP_FILE + ".TIF");
                                                docCount++;
                                            }
                                            //Changed on 19/09/2009 for managing file copying problem in FQC
                                            if (File.Exists(imagePath + "\\" + iDtr[propCount]["page_index_name"].ToString()) == false)
                                            {
                                                imagePath = policyPath + "\\" + ihConstants._INDEXING_FOLDER;
                                                //tempPath = policyPath + "\\" + ihConstants._INDEXING_FOLDER;
                                            }
                                            //imageName[pgCurrent] = tempPath + "\\" + iDtr[propCount]["page_index_name"].ToString();
                                            if (File.Exists(imagePath + "\\" + iDtr[propCount]["page_index_name"].ToString()) == false)
                                            {
                                                txtMsg.Text = txtMsg.Text + " Image invalid: " + iDtr[propCount]["page_index_name"].ToString();
                                                txtMsg.SelectionStart = txtMsg.Text.Length;
                                                txtMsg.ScrollToCaret();
                                                txtMsg.Refresh();
                                                validity = validity + 1;
                                            }
                                            expImageCount = expImageCount + 1;
                                            policyTotPage = policyTotPage + 1;
                                            pgCountWhlErr = pgCountWhlErr + 1;
                                            proposalExists = true;

                                            DateTime edDt = DateTime.Now;
                                            TimeSpan tsp = edDt - dt;
                                            //Increment the variable if doctype is found
                                            pgCurrent++;
                                        }
                                        //MessageBox.Show(tsp.Milliseconds.ToString());
                                    }

                                    pgCount = 0; //Holds the pagecount for a given doctype
                                    for (tmp = 0; tmp < iDtr.Length; tmp++)
                                    {
                                        if (iDtr[tmp]["doc_type"].ToString() == ihConstants.POLICYLOANS_FILE)
                                            pgCount++;
                                    }
                                    //End: Calculates count for doc types
                                    imageName = new string[pgCount]; //Initialize the array with no. of images for the doc type

                                    pgCurrent = 0; 	//Temporary variable where count is increased when
                                    //the doc type is found, so that the name can be stored
                                    //in the right index

                                    //Rotate through all the images having doctype = Proposal Form
                                    for (int propCount = 0; propCount < iDtr.Length; propCount++)
                                    {
                                        if (iDtr[propCount]["doc_type"].ToString() == ihConstants.POLICYLOANS_FILE)
                                        {
                                            DateTime dt = DateTime.Now;
                                            if (pgCurrent == 0)
                                            {
                                                docType.Add(ihConstants.POLICYLOANS_FILE);
                                                multiPageFileName.Add(policyNumber + "_" + ihConstants.POLICYLOANS_FILE + ".TIF");
                                                docCount++;
                                            }
                                            //Changed on 19/09/2009 for managing file copying problem in FQC
                                            if (File.Exists(imagePath + "\\" + iDtr[propCount]["page_index_name"].ToString()) == false)
                                            {
                                                imagePath = policyPath + "\\" + ihConstants._INDEXING_FOLDER;
                                            }
                                            //imageName[pgCurrent] = tempPath + "\\" + iDtr[propCount]["page_index_name"].ToString();
                                            if (File.Exists(imagePath + "\\" + iDtr[propCount]["page_index_name"].ToString()) == false)
                                            {
                                                txtMsg.Text = txtMsg.Text + " Image invalid: " + iDtr[propCount]["page_index_name"].ToString();
                                                txtMsg.SelectionStart = txtMsg.Text.Length;
                                                txtMsg.ScrollToCaret();
                                                txtMsg.Refresh();
                                                validity = validity + 1;
                                            }
                                            expImageCount = expImageCount + 1;
                                            policyTotPage = policyTotPage + 1;
                                            pgCountWhlErr = pgCountWhlErr + 1;
                                            proposalExists = true;

                                            DateTime edDt = DateTime.Now;
                                            TimeSpan tsp = edDt - dt;
                                            //Increment the variable if doctype is found
                                            pgCurrent++;
                                        }
                                        //MessageBox.Show(tsp.Milliseconds.ToString());
                                    }

                                    pgCount = 0; //Holds the pagecount for a given doctype
                                    for (tmp = 0; tmp < iDtr.Length; tmp++)
                                    {
                                        if (iDtr[tmp]["doc_type"].ToString() == ihConstants.MEDICALREPORT_FILE)
                                            pgCount++;
                                    }
                                    //End: Calculates count for doc types
                                    imageName = new string[pgCount]; //Initialize the array with no. of images for the doc type

                                    pgCurrent = 0; 	//Temporary variable where count is increased when
                                    //the doc type is found, so that the name can be stored
                                    //in the right index

                                    //Rotate through all the images having doctype = Proposal Form
                                    for (int propCount = 0; propCount < iDtr.Length; propCount++)
                                    {
                                        if (iDtr[propCount]["doc_type"].ToString() == ihConstants.MEDICALREPORT_FILE)
                                        {
                                            DateTime dt = DateTime.Now;
                                            if (pgCurrent == 0)
                                            {
                                                docType.Add(ihConstants.MEDICALREPORT_FILE);
                                                multiPageFileName.Add(policyNumber + "_" + ihConstants.MEDICALREPORT_FILE + ".TIF");
                                                docCount++;
                                            }
                                            //Changed on 19/09/2009 for managing file copying problem in FQC
                                            if (File.Exists(imagePath + "\\" + iDtr[propCount]["page_index_name"].ToString()) == false)
                                            {
                                                imagePath = policyPath + "\\" + ihConstants._INDEXING_FOLDER;
                                            }
                                            //                                                imageName[pgCurrent] = tempPath + "\\" + iDtr[propCount]["page_index_name"].ToString();
                                            if (File.Exists(imagePath + "\\" + iDtr[propCount]["page_index_name"].ToString()) == false)
                                            {
                                                txtMsg.Text = txtMsg.Text + " Image invalid: " + iDtr[propCount]["page_index_name"].ToString();
                                                txtMsg.SelectionStart = txtMsg.Text.Length;
                                                txtMsg.ScrollToCaret();
                                                txtMsg.Refresh();
                                                validity = validity + 1;
                                            }
                                            expImageCount = expImageCount + 1;
                                            policyTotPage = policyTotPage + 1;
                                            pgCountWhlErr = pgCountWhlErr + 1;
                                            proposalExists = true;

                                            DateTime edDt = DateTime.Now;
                                            TimeSpan tsp = edDt - dt;
                                            //Increment the variable if doctype is found
                                            pgCurrent++;
                                        }
                                        //MessageBox.Show(tsp.Milliseconds.ToString());
                                    }

                                    pgCount = 0; //Holds the pagecount for a given doctype
                                    for (tmp = 0; tmp < iDtr.Length; tmp++)
                                    {
                                        if (iDtr[tmp]["doc_type"].ToString() == ihConstants.NOMINATION_FILE)
                                            pgCount++;
                                    }
                                    //End: Calculates count for doc types
                                    imageName = new string[pgCount]; //Initialize the array with no. of images for the doc type

                                    pgCurrent = 0; 	//Temporary variable where count is increased when
                                    //the doc type is found, so that the name can be stored
                                    //in the right index

                                    //Rotate through all the images having doctype = Proposal Form
                                    for (int propCount = 0; propCount < iDtr.Length; propCount++)
                                    {
                                        if (iDtr[propCount]["doc_type"].ToString() == ihConstants.NOMINATION_FILE)
                                        {
                                            DateTime dt = DateTime.Now;
                                            if (pgCurrent == 0)
                                            {
                                                docType.Add(ihConstants.NOMINATION_FILE);
                                                multiPageFileName.Add(policyNumber + "_" + ihConstants.NOMINATION_FILE + ".TIF");
                                                docCount++;
                                            }
                                            //Changed on 19/09/2009 for managing file copying problem in FQC
                                            if (File.Exists(imagePath + "\\" + iDtr[propCount]["page_index_name"].ToString()) == false)
                                            {
                                                imagePath = policyPath + "\\" + ihConstants._INDEXING_FOLDER;
                                            }
                                            //imageName[pgCurrent] = tempPath + "\\" + iDtr[propCount]["page_index_name"].ToString();
                                            if (File.Exists(imagePath + "\\" + iDtr[propCount]["page_index_name"].ToString()) == false)
                                            {
                                                txtMsg.Text = txtMsg.Text + " Image invalid: " + iDtr[propCount]["page_index_name"].ToString();
                                                txtMsg.SelectionStart = txtMsg.Text.Length;
                                                txtMsg.ScrollToCaret();
                                                txtMsg.Refresh();
                                                validity = validity + 1;
                                            }

                                            expImageCount = expImageCount + 1;
                                            policyTotPage = policyTotPage + 1;
                                            pgCountWhlErr = pgCountWhlErr + 1;
                                            proposalExists = true;

                                            DateTime edDt = DateTime.Now;
                                            TimeSpan tsp = edDt - dt;
                                            //Increment the variable if doctype is found
                                            pgCurrent++;
                                        }
                                        //MessageBox.Show(tsp.Milliseconds.ToString());
                                    }

                                    pgCount = 0; //Holds the pagecount for a given doctype
                                    for (tmp = 0; tmp < iDtr.Length; tmp++)
                                    {
                                        if (iDtr[tmp]["doc_type"].ToString() == ihConstants.ASSIGNMENT_FILE)
                                            pgCount++;
                                    }
                                    //End: Calculates count for doc types
                                    imageName = new string[pgCount]; //Initialize the array with no. of images for the doc type

                                    pgCurrent = 0; 	//Temporary variable where count is increased when
                                    //the doc type is found, so that the name can be stored
                                    //in the right index

                                    //Rotate through all the images having doctype = Proposal Form
                                    for (int propCount = 0; propCount < iDtr.Length; propCount++)
                                    {
                                        if (iDtr[propCount]["doc_type"].ToString() == ihConstants.ASSIGNMENT_FILE)
                                        {
                                            DateTime dt = DateTime.Now;
                                            if (pgCurrent == 0)
                                            {
                                                docType.Add(ihConstants.ASSIGNMENT_FILE);
                                                multiPageFileName.Add(policyNumber + "_" + ihConstants.ASSIGNMENT_FILE + ".TIF");
                                                docCount++;
                                            }
                                            //Changed on 19/09/2009 for managing file copying problem in FQC
                                            if (File.Exists(imagePath + "\\" + iDtr[propCount]["page_index_name"].ToString()) == false)
                                            {
                                                imagePath = policyPath + "\\" + ihConstants._INDEXING_FOLDER;
                                            }
                                            //        imageName[pgCurrent] = tempPath + "\\" + iDtr[propCount]["page_index_name"].ToString();
                                            if (File.Exists(imagePath + "\\" + iDtr[propCount]["page_index_name"].ToString()) == false)
                                            {
                                                txtMsg.Text = txtMsg.Text + " Image invalid: " + iDtr[propCount]["page_index_name"].ToString();
                                                txtMsg.SelectionStart = txtMsg.Text.Length;
                                                txtMsg.ScrollToCaret();
                                                txtMsg.Refresh();
                                                validity = validity + 1;
                                            }
                                            expImageCount = expImageCount + 1;
                                            policyTotPage = policyTotPage + 1;
                                            pgCountWhlErr = pgCountWhlErr + 1;
                                            proposalExists = true;

                                            DateTime edDt = DateTime.Now;
                                            TimeSpan tsp = edDt - dt;
                                            //Increment the variable if doctype is found
                                            pgCurrent++;
                                        }
                                        //MessageBox.Show(tsp.Milliseconds.ToString());
                                    }

                                    pgCount = 0; //Holds the pagecount for a given doctype
                                    for (tmp = 0; tmp < iDtr.Length; tmp++)
                                    {
                                        if (iDtr[tmp]["doc_type"].ToString() == ihConstants.ALTERATION_FILE)
                                            pgCount++;
                                    }
                                    //End: Calculates count for doc types
                                    imageName = new string[pgCount]; //Initialize the array with no. of images for the doc type

                                    pgCurrent = 0; 	//Temporary variable where count is increased when
                                    //the doc type is found, so that the name can be stored
                                    //in the right index

                                    //Rotate through all the images having doctype = Proposal Form
                                    for (int propCount = 0; propCount < iDtr.Length; propCount++)
                                    {
                                        if (iDtr[propCount]["doc_type"].ToString() == ihConstants.ALTERATION_FILE)
                                        {
                                            DateTime dt = DateTime.Now;
                                            if (pgCurrent == 0)
                                            {
                                                docType.Add(ihConstants.ALTERATION_FILE);
                                                multiPageFileName.Add(policyNumber + "_" + ihConstants.ALTERATION_FILE + ".TIF");
                                                docCount++;
                                            }
                                            //Changed on 19/09/2009 for managing file copying problem in FQC
                                            if (File.Exists(imagePath + "\\" + iDtr[propCount]["page_index_name"].ToString()) == false)
                                            {
                                                imagePath = policyPath + "\\" + ihConstants._INDEXING_FOLDER;
                                            }
                                            //        imageName[pgCurrent] = tempPath + "\\" + iDtr[propCount]["page_index_name"].ToString();
                                            if (File.Exists(imagePath + "\\" + iDtr[propCount]["page_index_name"].ToString()) == false)
                                            {
                                                txtMsg.Text = txtMsg.Text + " Image invalid: " + iDtr[propCount]["page_index_name"].ToString();
                                                txtMsg.SelectionStart = txtMsg.Text.Length;
                                                txtMsg.ScrollToCaret();
                                                txtMsg.Refresh();
                                                validity = validity + 1;
                                            }
                                            expImageCount = expImageCount + 1;
                                            policyTotPage = policyTotPage + 1;
                                            pgCountWhlErr = pgCountWhlErr + 1;
                                            proposalExists = true;

                                            DateTime edDt = DateTime.Now;
                                            TimeSpan tsp = edDt - dt;
                                            //Increment the variable if doctype is found
                                            pgCurrent++;
                                        }
                                        //MessageBox.Show(tsp.Milliseconds.ToString());
                                    }

                                    pgCount = 0; //Holds the pagecount for a given doctype
                                    for (tmp = 0; tmp < iDtr.Length; tmp++)
                                    {
                                        if (iDtr[tmp]["doc_type"].ToString() == ihConstants.CLAIMS_FILE)
                                            pgCount++;
                                    }
                                    //End: Calculates count for doc types
                                    imageName = new string[pgCount]; //Initialize the array with no. of images for the doc type

                                    pgCurrent = 0; 	//Temporary variable where count is increased when
                                    //the doc type is found, so that the name can be stored
                                    //in the right index

                                    //Rotate through all the images having doctype = Proposal Form
                                    for (int propCount = 0; propCount < iDtr.Length; propCount++)
                                    {
                                        if (iDtr[propCount]["doc_type"].ToString() == ihConstants.CLAIMS_FILE)
                                        {
                                            DateTime dt = DateTime.Now;
                                            if (pgCurrent == 0)
                                            {
                                                docType.Add(ihConstants.CLAIMS_FILE);
                                                multiPageFileName.Add(policyNumber + "_" + ihConstants.CLAIMS_FILE + ".TIF");
                                                docCount++;
                                            }
                                            //Changed on 19/09/2009 for managing file copying problem in FQC
                                            if (File.Exists(imagePath + "\\" + iDtr[propCount]["page_index_name"].ToString()) == false)
                                            {
                                                imagePath = policyPath + "\\" + ihConstants._INDEXING_FOLDER;
                                            }
                                            //        imageName[pgCurrent] = tempPath + "\\" + iDtr[propCount]["page_index_name"].ToString();
                                            if (File.Exists(imagePath + "\\" + iDtr[propCount]["page_index_name"].ToString()) == false)
                                            {
                                                txtMsg.Text = txtMsg.Text + " Image invalid: " + iDtr[propCount]["page_index_name"].ToString();
                                                txtMsg.SelectionStart = txtMsg.Text.Length;
                                                txtMsg.ScrollToCaret();
                                                txtMsg.Refresh();
                                                validity = validity + 1;
                                            }
                                            expImageCount = expImageCount + 1;
                                            policyTotPage = policyTotPage + 1;
                                            pgCountWhlErr = pgCountWhlErr + 1;
                                            proposalExists = true;

                                            DateTime edDt = DateTime.Now;
                                            TimeSpan tsp = edDt - dt;
                                            //Increment the variable if doctype is found
                                            pgCurrent++;
                                        }
                                        //MessageBox.Show(tsp.Milliseconds.ToString());
                                    }

                                    pgCount = 0; //Holds the pagecount for a given doctype
                                    for (tmp = 0; tmp < iDtr.Length; tmp++)
                                    {
                                        if (iDtr[tmp]["doc_type"].ToString() == ihConstants.CORRESPONDENCE_FILE)
                                            pgCount++;
                                    }
                                    //End: Calculates count for doc types
                                    imageName = new string[pgCount]; //Initialize the array with no. of images for the doc type

                                    pgCurrent = 0; 	//Temporary variable where count is increased when
                                    //the doc type is found, so that the name can be stored
                                    //in the right index

                                    //Rotate through all the images having doctype = Proposal Form
                                    for (int propCount = 0; propCount < iDtr.Length; propCount++)
                                    {
                                        if (iDtr[propCount]["doc_type"].ToString() == ihConstants.CORRESPONDENCE_FILE)
                                        {
                                            DateTime dt = DateTime.Now;
                                            if (pgCurrent == 0)
                                            {
                                                docType.Add(ihConstants.CORRESPONDENCE_FILE);
                                                multiPageFileName.Add(policyNumber + "_" + ihConstants.CORRESPONDENCE_FILE + ".TIF");
                                                docCount++;
                                            }
                                            //Changed on 19/09/2009 for managing file copying problem in FQC
                                            if (File.Exists(imagePath + "\\" + iDtr[propCount]["page_index_name"].ToString()) == false)
                                            {
                                                imagePath = policyPath + "\\" + ihConstants._INDEXING_FOLDER;
                                            }
                                            //        imageName[pgCurrent] = tempPath + "\\" + iDtr[propCount]["page_index_name"].ToString();
                                            if (File.Exists(imagePath + "\\" + iDtr[propCount]["page_index_name"].ToString()) == false)
                                            {
                                                txtMsg.Text = txtMsg.Text + " Image invalid: " + iDtr[propCount]["page_index_name"].ToString();
                                                txtMsg.SelectionStart = txtMsg.Text.Length;
                                                txtMsg.ScrollToCaret();
                                                txtMsg.Refresh();
                                                validity = validity + 1;
                                            }
                                            expImageCount = expImageCount + 1;
                                            policyTotPage = policyTotPage + 1;
                                            pgCountWhlErr = pgCountWhlErr + 1;
                                            proposalExists = true;

                                            DateTime edDt = DateTime.Now;
                                            TimeSpan tsp = edDt - dt;
                                            //Increment the variable if doctype is found
                                            pgCurrent++;
                                        }
                                        //MessageBox.Show(tsp.Milliseconds.ToString());
                                    }

                                    pgCount = 0; //Holds the pagecount for a given doctype
                                    for (tmp = 0; tmp < iDtr.Length; tmp++)
                                    {
                                        if (iDtr[tmp]["doc_type"].ToString() == ihConstants.PROPOSALENCLOSERS_FILE)
                                            pgCount++;
                                    }
                                    //End: Calculates count for doc types
                                    imageName = new string[pgCount]; //Initialize the array with no. of images for the doc type

                                    pgCurrent = 0; 	//Temporary variable where count is increased when
                                    //the doc type is found, so that the name can be stored
                                    //in the right index

                                    //Rotate through all the images having doctype = Proposal Form
                                    for (int propCount = 0; propCount < iDtr.Length; propCount++)
                                    {
                                        if (iDtr[propCount]["doc_type"].ToString() == ihConstants.PROPOSALENCLOSERS_FILE)
                                        {
                                            DateTime dt = DateTime.Now;
                                            if (pgCurrent == 0)
                                            {
                                                docType.Add(ihConstants.PROPOSALENCLOSERS_FILE);
                                                multiPageFileName.Add(policyNumber + "_" + ihConstants.PROPOSALENCLOSERS_FILE + ".TIF");
                                                docCount++;
                                            }
                                            //Changed on 19/09/2009 for managing file copying problem in FQC
                                            if (File.Exists(imagePath + "\\" + iDtr[propCount]["page_index_name"].ToString()) == false)
                                            {
                                                imagePath = policyPath + "\\" + ihConstants._INDEXING_FOLDER;
                                            }
                                            //        imageName[pgCurrent] = tempPath + "\\" + iDtr[propCount]["page_index_name"].ToString();
                                            if (File.Exists(imagePath + "\\" + iDtr[propCount]["page_index_name"].ToString()) == false)
                                            {
                                                txtMsg.Text = txtMsg.Text + " Image invalid: " + iDtr[propCount]["page_index_name"].ToString();
                                                txtMsg.SelectionStart = txtMsg.Text.Length;
                                                txtMsg.ScrollToCaret();
                                                txtMsg.Refresh();
                                                validity = validity + 1;
                                            }
                                            expImageCount = expImageCount + 1;
                                            policyTotPage = policyTotPage + 1;
                                            pgCountWhlErr = pgCountWhlErr + 1;
                                            proposalExists = true;

                                            DateTime edDt = DateTime.Now;
                                            TimeSpan tsp = edDt - dt;
                                            //Increment the variable if doctype is found
                                            pgCurrent++;
                                        }
                                        //MessageBox.Show(tsp.Milliseconds.ToString());
                                    }

                                    pgCount = 0; //Holds the pagecount for a given doctype
                                    for (tmp = 0; tmp < iDtr.Length; tmp++)
                                    {
                                        if (iDtr[tmp]["doc_type"].ToString() == ihConstants.POLICYBOND_FILE)
                                            pgCount++;
                                    }
                                    //End: Calculates count for doc types
                                    imageName = new string[pgCount]; //Initialize the array with no. of images for the doc type

                                    pgCurrent = 0; 	//Temporary variable where count is increased when
                                    //the doc type is found, so that the name can be stored
                                    //in the right index

                                    //Rotate through all the images having doctype = Proposal Form
                                    for (int propCount = 0; propCount < iDtr.Length; propCount++)
                                    {
                                        if (iDtr[propCount]["doc_type"].ToString() == ihConstants.POLICYBOND_FILE)
                                        {
                                            DateTime dt = DateTime.Now;
                                            if (pgCurrent == 0)
                                            {
                                                docType.Add(ihConstants.POLICYBOND_FILE);
                                                multiPageFileName.Add(policyNumber + "_" + ihConstants.POLICYBOND_FILE + ".TIF");
                                                docCount++;
                                            }
                                            //Changed on 19/09/2009 for managing file copying problem in FQC
                                            if (File.Exists(imagePath + "\\" + iDtr[propCount]["page_index_name"].ToString()) == false)
                                            {
                                                imagePath = policyPath + "\\" + ihConstants._INDEXING_FOLDER;
                                            }
                                            //        imageName[pgCurrent] = tempPath + "\\" + iDtr[propCount]["page_index_name"].ToString();
                                            if (File.Exists(imagePath + "\\" + iDtr[propCount]["page_index_name"].ToString()) == false)
                                            {
                                                txtMsg.Text = txtMsg.Text + " Image invalid: " + iDtr[propCount]["page_index_name"].ToString();
                                                txtMsg.SelectionStart = txtMsg.Text.Length;
                                                txtMsg.ScrollToCaret();
                                                txtMsg.Refresh();
                                                validity = validity + 1;
                                            }
                                            expImageCount = expImageCount + 1;
                                            policyTotPage = policyTotPage + 1;
                                            pgCountWhlErr = pgCountWhlErr + 1;
                                            proposalExists = true;

                                            DateTime edDt = DateTime.Now;
                                            TimeSpan tsp = edDt - dt;
                                            //Increment the variable if doctype is found
                                            pgCurrent++;
                                        }
                                        //MessageBox.Show(tsp.Milliseconds.ToString());
                                    }

                                    pgCount = 0; //Holds the pagecount for a given doctype
                                    for (tmp = 0; tmp < iDtr.Length; tmp++)
                                    {
                                        if (iDtr[tmp]["doc_type"].ToString() == ihConstants.SURRENDER_FILE)
                                            pgCount++;
                                    }
                                    //End: Calculates count for doc types
                                    imageName = new string[pgCount]; //Initialize the array with no. of images for the doc type

                                    pgCurrent = 0; 	//Temporary variable where count is increased when
                                    //the doc type is found, so that the name can be stored
                                    //in the right index

                                    //Rotate through all the images having doctype = Proposal Form
                                    for (int propCount = 0; propCount < iDtr.Length; propCount++)
                                    {
                                        if (iDtr[propCount]["doc_type"].ToString() == ihConstants.SURRENDER_FILE)
                                        {
                                            DateTime dt = DateTime.Now;
                                            if (pgCurrent == 0)
                                            {
                                                docType.Add(ihConstants.SURRENDER_FILE);
                                                multiPageFileName.Add(policyNumber + "_" + ihConstants.SURRENDER_FILE + ".TIF");
                                                docCount++;
                                            }
                                            //Changed on 19/09/2009 for managing file copying problem in FQC
                                            if (File.Exists(imagePath + "\\" + iDtr[propCount]["page_index_name"].ToString()) == false)
                                            {
                                                imagePath = policyPath + "\\" + ihConstants._INDEXING_FOLDER;
                                            }
                                            //        imageName[pgCurrent] = tempPath + "\\" + iDtr[propCount]["page_index_name"].ToString();
                                            if (File.Exists(imagePath + "\\" + iDtr[propCount]["page_index_name"].ToString()) == false)
                                            {
                                                txtMsg.Text = txtMsg.Text + " Image invalid: " + iDtr[propCount]["page_index_name"].ToString();
                                                txtMsg.SelectionStart = txtMsg.Text.Length;
                                                txtMsg.ScrollToCaret();
                                                txtMsg.Refresh();
                                                validity = validity + 1;
                                            }
                                            expImageCount = expImageCount + 1;
                                            policyTotPage = policyTotPage + 1;
                                            pgCountWhlErr = pgCountWhlErr + 1;
                                            proposalExists = true;

                                            DateTime edDt = DateTime.Now;
                                            TimeSpan tsp = edDt - dt;
                                            //Increment the variable if doctype is found
                                            pgCurrent++;
                                        }
                                        //MessageBox.Show(tsp.Milliseconds.ToString());
                                    }

                                    pgCount = 0; //Holds the pagecount for a given doctype
                                    for (tmp = 0; tmp < iDtr.Length; tmp++)
                                    {
                                        if (iDtr[tmp]["doc_type"].ToString() == ihConstants.REVIVALS_FILE)
                                            pgCount++;
                                    }
                                    //End: Calculates count for doc types
                                    imageName = new string[pgCount]; //Initialize the array with no. of images for the doc type

                                    pgCurrent = 0; 	//Temporary variable where count is increased when
                                    //the doc type is found, so that the name can be stored
                                    //in the right index

                                    //Rotate through all the images having doctype = Proposal Form
                                    for (int propCount = 0; propCount < iDtr.Length; propCount++)
                                    {
                                        if (iDtr[propCount]["doc_type"].ToString() == ihConstants.REVIVALS_FILE)
                                        {
                                            DateTime dt = DateTime.Now;
                                            if (pgCurrent == 0)
                                            {
                                                docType.Add(ihConstants.REVIVALS_FILE);
                                                multiPageFileName.Add(policyNumber + "_" + ihConstants.REVIVALS_FILE + ".TIF");
                                                docCount++;
                                            }
                                            //Changed on 19/09/2009 for managing file copying problem in FQC
                                            if (File.Exists(imagePath + "\\" + iDtr[propCount]["page_index_name"].ToString()) == false)
                                            {
                                                imagePath = policyPath + "\\" + ihConstants._INDEXING_FOLDER;
                                            }
                                            //        imageName[pgCurrent] = tempPath + "\\" + iDtr[propCount]["page_index_name"].ToString();
                                            if (File.Exists(imagePath + "\\" + iDtr[propCount]["page_index_name"].ToString()) == false)
                                            {
                                                txtMsg.Text = txtMsg.Text + " Image invalid: " + iDtr[propCount]["page_index_name"].ToString();
                                                txtMsg.SelectionStart = txtMsg.Text.Length;
                                                txtMsg.ScrollToCaret();
                                                txtMsg.Refresh();
                                                validity = validity + 1;
                                            }
                                            expImageCount = expImageCount + 1;
                                            policyTotPage = policyTotPage + 1;
                                            pgCountWhlErr = pgCountWhlErr + 1;
                                            proposalExists = true;

                                            DateTime edDt = DateTime.Now;
                                            TimeSpan tsp = edDt - dt;
                                            //Increment the variable if doctype is found
                                            pgCurrent++;
                                        }
                                        //MessageBox.Show(tsp.Milliseconds.ToString());
                                    }

                                    pgCount = 0; //Holds the pagecount for a given doctype
                                    for (tmp = 0; tmp < iDtr.Length; tmp++)
                                    {
                                        if (iDtr[tmp]["doc_type"].ToString() == ihConstants.SIGNATUREPAGE_FILE)
                                            pgCount++;
                                    }
                                    //End: Calculates count for doc types
                                    imageName = new string[pgCount]; //Initialize the array with no. of images for the doc type

                                    pgCurrent = 0; 	//Temporary variable where count is increased when
                                    //the doc type is found, so that the name can be stored
                                    //in the right index

                                    //Rotate through all the images having doctype = Proposal Form
                                    for (int propCount = 0; propCount < iDtr.Length; propCount++)
                                    {
                                        if (iDtr[propCount]["doc_type"].ToString() == ihConstants.SIGNATUREPAGE_FILE)
                                        {
                                            DateTime dt = DateTime.Now;
                                            if (pgCurrent == 0)
                                            {
                                                docType.Add(ihConstants.SIGNATUREPAGE_FILE);
                                                multiPageFileName.Add(policyNumber + "_" + ihConstants.SIGNATUREPAGE_FILE + ".TIF");
                                                docCount++;
                                            }
                                            //Changed on 19/09/2009 for managing file copying problem in FQC
                                            if (File.Exists(imagePath + "\\" + iDtr[propCount]["page_index_name"].ToString()) == false)
                                            {
                                                imagePath = policyPath + "\\" + ihConstants._INDEXING_FOLDER;
                                            }
                                            //        imageName[pgCurrent] = tempPath + "\\" + iDtr[propCount]["page_index_name"].ToString();
                                            if (File.Exists(imagePath + "\\" + iDtr[propCount]["page_index_name"].ToString()) == false)
                                            {
                                                txtMsg.Text = txtMsg.Text + " Image invalid: " + iDtr[propCount]["page_index_name"].ToString();
                                                txtMsg.SelectionStart = txtMsg.Text.Length;
                                                txtMsg.ScrollToCaret();
                                                txtMsg.Refresh();
                                                validity = validity + 1;
                                            }
                                            expImageCount = expImageCount + 1;
                                            policyTotPage = policyTotPage + 1;
                                            pgCountWhlErr = pgCountWhlErr + 1;
                                            proposalExists = true;

                                            DateTime edDt = DateTime.Now;
                                            TimeSpan tsp = edDt - dt;
                                            //Increment the variable if doctype is found
                                            pgCurrent++;
                                        }
                                        //MessageBox.Show(tsp.Milliseconds.ToString());
                                    }

                                    pgCount = 0; //Holds the pagecount for a given doctype
                                    for (tmp = 0; tmp < iDtr.Length; tmp++)
                                    {
                                        if (iDtr[tmp]["doc_type"].ToString() == ihConstants.OTHERS_FILE)
                                            pgCount++;
                                    }
                                    //End: Calculates count for doc types
                                    imageName = new string[pgCount]; //Initialize the array with no. of images for the doc type

                                    pgCurrent = 0; 	//Temporary variable where count is increased when
                                    //the doc type is found, so that the name can be stored
                                    //in the right index

                                    //Rotate through all the images having doctype = Proposal Form
                                    for (int propCount = 0; propCount < iDtr.Length; propCount++)
                                    {
                                        if (iDtr[propCount]["doc_type"].ToString() == ihConstants.OTHERS_FILE)
                                        {
                                            DateTime dt = DateTime.Now;
                                            if (pgCurrent == 0)
                                            {
                                                docType.Add(ihConstants.OTHERS_FILE);
                                                multiPageFileName.Add(policyNumber + "_" + ihConstants.OTHERS_FILE + ".TIF");
                                                docCount++;
                                            }
                                            //Changed on 19/09/2009 for managing file copying problem in FQC
                                            if (File.Exists(imagePath + "\\" + iDtr[propCount]["page_index_name"].ToString()) == false)
                                            {
                                                imagePath = policyPath + "\\" + ihConstants._INDEXING_FOLDER;
                                            }
                                            //        imageName[pgCurrent] = tempPath + "\\" + iDtr[propCount]["page_index_name"].ToString();
                                            if (File.Exists(imagePath + "\\" + iDtr[propCount]["page_index_name"].ToString()) == false)
                                            {
                                                txtMsg.Text = txtMsg.Text + " Image invalid: " + iDtr[propCount]["page_index_name"].ToString();
                                                txtMsg.SelectionStart = txtMsg.Text.Length;
                                                txtMsg.ScrollToCaret();
                                                txtMsg.Refresh();
                                                validity = validity + 1;
                                            }
                                            expImageCount = expImageCount + 1;
                                            policyTotPage = policyTotPage + 1;
                                            pgCountWhlErr = pgCountWhlErr + 1;
                                            proposalExists = true;

                                            DateTime edDt = DateTime.Now;
                                            TimeSpan tsp = edDt - dt;
                                            //Increment the variable if doctype is found
                                            pgCurrent++;
                                        }
                                        //MessageBox.Show(tsp.Milliseconds.ToString());
                                    }

                                    DocTypeCount = docCount.ToString().PadLeft(2, Convert.ToChar("0"));
                                    if (policyTotPage <= ihConstants._MAX_POLICY_PAGE_COUNT)
                                    {
                                        MessageBox.Show("Policy - " + policyNumber + " has less pages, aborting...");
                                        ///Rollback transaction
                                        //exportTrans.Rollback();
                                        expBol = false;
                                        break;
                                    }
                                    /*
                                    if ((proposalExists != true) || (signatureExists != true))
                                    {
                                        MessageBox.Show("Proposal form or signature page missing for the policy - " + policyNumber + ", aborting...");
                                        ///Rollback transaction
                                        exportTrans.Rollback();
                                        expBol = false;
                                        break;
                                    }
                                     * */

                                }
                                else
                                {
                                    if ((status == (int)eSTATES.POLICY_MISSING))
                                    {
                                        boxno = "000";
                                        ImageCount = "000";
                                        DocTypeCount = "00";
                                        //Scanuploadlag = "02";
                                        //exportPath = batchPath + "\\" + ihConstants._EXPORT_FOLDER + "\\" + vendorCode + "_" + divisionCode + "-" + branchCode.PadRight(4, Convert.ToChar(" ")) + "-" + batchCount + "_" + versionNumber + "\\" + policyNumber;
                                        //if (Directory.Exists(exportPath) == false)
                                        //{
                                        //    Directory.CreateDirectory(exportPath);
                                        //}
                                    }
                                    else
                                    {
                                        if ((status == (int)eSTATES.POLICY_INITIALIZED))
                                        {
                                            MessageBox.Show("Item " + policyNumber + " have not been scanned, aborting...");
                                            ///Rollback transaction
                                            //exportTrans.Rollback();
                                            expBol = false;
                                            break;
                                        }
                                        if ((status == (int)eSTATES.POLICY_NOT_INDEXED))
                                        {
                                            MessageBox.Show("Item " + policyNumber + " have not been indexed, aborting...");
                                            ///Rollback transaction
                                            //exportTrans.Rollback();
                                            expBol = false;
                                            break;
                                        }
                                        if ((status == (int)eSTATES.POLICY_SCANNED))
                                        {
                                            MessageBox.Show("QC not done for policy - " + policyNumber + " , aborting...");
                                            ///Rollback transaction
                                            //exportTrans.Rollback();
                                            expBol = false;
                                            break;
                                        }
                                        if ((status == (int)eSTATES.POLICY_EXCEPTION))
                                        {
                                            MessageBox.Show("LIC exception not cleared for the policy - " + policyNumber + " , aborting...");
                                            ///Rollback transaction
                                            //exportTrans.Rollback();
                                            expBol = false;
                                            break;
                                        }
                                        if ((status == (int)eSTATES.POLICY_QC))
                                        {
                                            MessageBox.Show("Indexing not done for policy - " + policyNumber + " , aborting...");
                                            ///Rollback transaction
                                            //exportTrans.Rollback();
                                            expBol = false;
                                            break;
                                        }
                                        if ((status == (int)eSTATES.POLICY_ON_HOLD))
                                        {
                                            ImageCount = "000";
                                            DocTypeCount = "00";
                                            //  Scanuploadflag = "02";
                                            //exportPath = batchPath + "\\" + ihConstants._EXPORT_FOLDER + "\\" + vendorCode + "_" + divisionCode + "-" + branchCode.PadRight(4, Convert.ToChar(" ")) + "-" + batchCount + "_" + versionNumber + "\\" + policyNumber;
                                        }
                                    }
                                }
                                if (validity == 0)
                                {
                                    txtMsg.Text = txtMsg.Text + " : Valid";
                                    txtMsg.SelectionStart = txtMsg.Text.Length;
                                    txtMsg.ScrollToCaret();
                                    txtMsg.Refresh();
                                }
                                txtMsg.Text = txtMsg.Text + "  Page : " + ImageCount + "\r\n";
                                txtMsg.SelectionStart = txtMsg.Text.Length;
                                txtMsg.ScrollToCaret();
                                txtMsg.Refresh();
                                //toolStatus.Text = toolStatus.Text + " Page : " + ImageCount;
                                //Application.DoEvents();
                                //if (tempPath != string.Empty)
                                //    DeleteLocalFile(tempPath, exportPath);
                            }
                            //sw.Close();
                            //sw.Dispose();
                            //sw.Flush();
                            imageDs.Dispose();
                            docType.Clear();
                            multiPageFileName.Clear();
                            documentType = string.Empty;
                            proposalExists = false;
                            signatureExists = false;
                            policyTotPage = 0;
                            //exportTrans.Commit();

                        }
                    }
                }
                DateTime endDt = DateTime.Now;
                TimeSpan tp = endDt - stDt;
                MessageBox.Show("Validation finished......");
            }

        }
        private bool LoadImage(string pPath)
        {

            if (img.LoadBitmapFromFile(pPath) == IGRStatus.Success)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void chkReExport_CheckedChanged(object sender, EventArgs e)
        {
            //PopulateBox();
        }

        private void populateDeeds(string batch_key, string proj_key)
        {
            cleargrid();
            string batchKey = null;
            int holdDeed = 0;
            dbcon = new NovaNet.Utils.dbCon();
            ctrlBox = new CtrlBox(Convert.ToInt32(proj_key), Convert.ToInt32(batch_key), "0");
            wfeBox tmpBox = new wfeBox(sqlCon, ctrlBox);


            batchKey = batch_key;
            dsexport = tmpBox.GetExportableDeed(state);

            if (dsexport.Tables[0].Rows.Count > 0)
            {
                dgvexport.DataSource = dsexport.Tables[0];
                dgvexport.Columns[0].Width = 110;
                dgvexport.Columns[1].Visible = false;
                dgvexport.Columns[2].Visible = false;
                dgvexport.Columns[3].Visible = false;
                dgvexport.Columns[4].Visible = false;
                dgvexport.Columns[5].Visible = false;
                dgvexport.Columns[6].Visible = false;
                dgvexport.Columns[7].Visible = false;
                dgvexport.Columns[8].Visible = false;
                dgvexport.Columns[9].Visible = false;
                dgvexport.Columns[10].Visible = false;
                dgvexport.Columns[11].Visible = false;
            }
            dgvexport.Columns.Add("Status", "Status");

            //for (int i = 0; i < dgvexport.Rows.Count; i++)
            //{
            //    if (dgvexport.Rows[i].Cells[11].Value.ToString() == "Y")
            //    {
            //        dgvexport.Rows[i].DefaultCellStyle.BackColor = Color.Yellow;
            //        dgvexport.Rows[i].DefaultCellStyle.ForeColor  = Color.Red;
            //        holdDeed = holdDeed +1;
            //    }
            //    dgvexport.Rows[i].Selected = false;
            //}
            lbldeedCount.Text = dgvexport.Rows.Count.ToString();
            lblholdDeed.Text = holdDeed.ToString();
           // btnExport.Enabled = true;
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            try
            {

                btnExport.Enabled = false;

                

                string batchPath = string.Empty;
                string batchName = string.Empty;
                string resultMsg = "Hold Deeds" + "\r\n";
                DataTable deedEx = new DataTable();
                DataTable NameEx = new DataTable();
                DataTable PropEx = new DataTable();
                DataTable CSVPropEx = new DataTable();
                DataTable CSVPropEx1 = new DataTable();
                DataTable PlotEx = new DataTable();
                DataTable KhatianEx = new DataTable();
                string expFolder = string.Empty;
                bool isDeleted = false;
                int MaxExportCount = 0;
                int StatusStep = 0;
                config = new ImageConfig(ihConstants.CONFIG_FILE_PATH);
                expFolder = config.GetValue(ihConstants.EXPORT_FOLDER_SECTION, ihConstants.EXPORT_FOLDER_KEY).Trim();

                System.Text.StringBuilder Builder1 = new System.Text.StringBuilder();
                Builder1.Append(PropEx.Rows.Count.ToString());
                Builder1.Append(",");

                int len = expFolder.IndexOf('\0');
                expFolder = expFolder.Substring(0, len);
                List<DeedImageDetails> dList = new List<DeedImageDetails>();


                lblFinalStatus.Text = "Please wait while Exporting....  ";
                Application.DoEvents();
                if (dgvbatch.Rows.Count > 0)
                {
                    

                    StatusStep = dgvbatch.Rows.Count;
                    progressBar2.Value = 0;
                    progressBar1.Value = 0;
                    int step = 100 / StatusStep;
                    progressBar2.Step = step;
                    for (int z = 0; z < dgvbatch.Rows.Count; z++)
                    {

                        dgvexport.DataSource = null;
                        dataGridView1.DataSource = null;
                        dataGridView2.DataSource = null;
                        dataGridView3.DataSource = null;
                        dgvKhatian.DataSource = null;
                        dgvPlot.DataSource = null;
                        deedEx.Clear();
                        NameEx.Clear();
                        CSVPropEx.Clear();
                        CSVPropEx1.Clear();
                        PlotEx.Clear();
                        KhatianEx.Clear();
                        populateDeeds(dgvbatch.Rows[z].Cells[2].Value.ToString(), dgvbatch.Rows[z].Cells[3].Value.ToString());
                        MaxExportCount = wPolicy.getMaxExportCount(cmbProject.SelectedValue.ToString(), dgvbatch.Rows[z].Cells[2].Value.ToString());
                        if (dgvexport.Rows.Count > 0)
                        {
                            for (int i = 0; i < dgvexport.Rows.Count; i++)
                            {
                                if (Convert.ToInt32(dgvexport.Rows[i].Cells["status"].Value.ToString()) == 30 || Convert.ToInt32(dgvexport.Rows[i].Cells["status"].Value.ToString()) == 21)
                                {
                                    dgvexport.Rows[i].DefaultCellStyle.BackColor = Color.Red;
                                    MessageBox.Show("There is some Problem in one or more deeds, Please Check and Retry..., Export Failed");
                                    btnExport.Enabled = true;
                                    return;
                                }
                            }
                        }

                        if (dgvexport.Rows.Count > 0)
                        {
                            Application.DoEvents();
                            dgvbatch.Rows[z].DefaultCellStyle.BackColor = Color.GreenYellow;
                            int i1 = 100 / dsexport.Tables[0].Rows.Count;
                            progressBar1.Step = i1;
                            progressBar1.Increment(i1);
                            Application.DoEvents();
                            wBatch = new wfeBatch(sqlCon);
                            batchPath = wBatch.GetPath(Convert.ToInt32(cmbProject.SelectedValue.ToString()), Convert.ToInt32(dgvbatch.Rows[z].Cells[2].Value.ToString()));
                            batchPath = batchPath + "\\1\\Export";


                            if (!Directory.Exists(expFolder + "\\Export\\" + cmbrunNum.Text))
                            {
                                Directory.CreateDirectory(expFolder + "\\Export\\" + cmbrunNum.Text);
                            }
                            //else
                            //{
                            //    Directory.Delete(expFolder + "\\Export\\" + cmbrunNum.Text, true);
                            //    Directory.CreateDirectory(expFolder + "\\Export\\" + cmbrunNum.Text);
                            //}
                            for (int x = 0; x < dsexport.Tables[0].Rows.Count; x++)
                            {
                                //if (dsexport.Tables[0].Rows[x][11].ToString() != "Y")
                                //{
                                DeedImageDetails imgDetails = new DeedImageDetails();
                                wfeImage tmpBox = new wfeImage(sqlCon);
                                DataSet dsimage = new DataSet();
                                Application.DoEvents();
                                lbl.Text = "Exporting :" + dgvbatch.Rows[z].Cells[1].Value.ToString();
                                Application.DoEvents();
                                string aa = wBatch.GetPath(Convert.ToInt32(cmbProject.SelectedValue.ToString()), Convert.ToInt32(dgvbatch.Rows[z].Cells[2].Value.ToString()));
                                sqlFileName = dsexport.Tables[0].Rows[x][0].ToString();
                                int index = sqlFileName.IndexOf('[');
                                sqlFileName = sqlFileName.Substring(0, index);
                                batchName = sqlFileName.Substring(0, index);
                                batchName = batchName + dsexport.Tables[0].Rows[x][9].ToString() + "_" + MaxExportCount.ToString();
                                sqlFileName = sqlFileName + dsexport.Tables[0].Rows[x][9].ToString() + ".mdf";
                                dsimage = tmpBox.GetAllExportedImage(dsexport.Tables[0].Rows[x][1].ToString(), dsexport.Tables[0].Rows[x][2].ToString(), dsexport.Tables[0].Rows[x][3].ToString(), dsexport.Tables[0].Rows[x][0].ToString());
                                imageName = new string[dsimage.Tables[0].Rows.Count];
                                string IMGName = dsexport.Tables[0].Rows[x][0].ToString();
                                string IMGName1 = IMGName.Split(new char[] { '[', ']' })[1];
                                IMGName = IMGName.Replace("[", "");
                                IMGName = IMGName.Replace("]", "");
                                string fileName = dsexport.Tables[0].Rows[x][0].ToString();
                                if (dsimage.Tables[0].Rows.Count > 0)
                                {
                                    for (int a = 0; a < dsimage.Tables[0].Rows.Count; a++)
                                    {
                                        //imageName[a] = dsexport.Tables[0].Rows[x][4].ToString() + "\\QC" + "\\" + dsimage.Tables[0].Rows[a]["page_name"].ToString();
                                        imageName[a] = aa + "\\1\\" + fileName + "\\QC" + "\\" + dsimage.Tables[0].Rows[a]["page_name"].ToString();
                                    }
                                    if (imageName.Length != 0)
                                    {
                                        if (Directory.Exists(expFolder + "\\Export\\" + cmbrunNum.Text + "\\" + batchName) && isDeleted == false)
                                        {
                                            Directory.Delete(expFolder + "\\Export\\" + cmbrunNum.Text + "\\" + batchName, true);

                                        }
                                        if (!Directory.Exists(expFolder + "\\Export\\" + cmbrunNum.Text + "\\" + batchName + "\\" + dsexport.Tables[0].Rows[0][5].ToString() + "\\" + dsexport.Tables[0].Rows[0][6].ToString()))
                                        {
                                            Directory.CreateDirectory(expFolder + "\\Export\\" + cmbrunNum.Text + "\\" + batchName + "\\" + dsexport.Tables[0].Rows[0][5].ToString() + "\\" + dsexport.Tables[0].Rows[0][6].ToString());
                                            isDeleted = true;
                                        }

                                        ////sumit for export problem
                                        //for (int i = 0; i < imageName.Length; i++)
                                        //{
                                        //    if (File.Exists(imageName[i]))
                                        //    {

                                        //    }
                                        //    else
                                        //    {
                                        //        MessageBox.Show("There is a problem in " + imageName[i]);
                                        //        return;
                                        //    }

                                        //}



                                        if (img.TifToPdf(imageName, 80, expFolder + "\\Export\\" + cmbrunNum.Text + "\\" + batchName + "\\" + dsexport.Tables[0].Rows[0][5].ToString() + "\\" + dsexport.Tables[0].Rows[0][6].ToString() + "\\" + IMGName + ".pdf") == true)
                                        {
                                        }
                                        else
                                        {
                                            MessageBox.Show("There is a problem in one or more pages of Deed No: " + IMGName + "\n The error is: " + img.GetError());
                                            return;
                                        }

                                    }
                                }

                                DataTable dt = tmpBox.GetAllDeedEX(dsexport.Tables[0].Rows[x][5].ToString(), dsexport.Tables[0].Rows[x][6].ToString(), dsexport.Tables[0].Rows[x][7].ToString(), dsexport.Tables[0].Rows[x][8].ToString(), IMGName1);
                                if (dt.Rows.Count > 0)
                                {
                                    imgDetails.DistrictCode = dt.Rows[0][0].ToString();
                                    imgDetails.RoCode = dt.Rows[0][1].ToString();
                                    imgDetails.Book = dt.Rows[0][2].ToString();
                                    imgDetails.DeedYear = dt.Rows[0][3].ToString();
                                    imgDetails.DeedNumber = dt.Rows[0][4].ToString();
                                    imgDetails.DeedImage = "\\" + cmbrunNum.Text + "\\" + batchName + "\\" + dsexport.Tables[0].Rows[0][5].ToString() + "\\" + dsexport.Tables[0].Rows[0][6].ToString() + "\\" + IMGName + ".pdf";
                                }

                                if (deedEx.Rows.Count < 1)
                                {

                                    deedEx = dt.Clone();
                                }

                                foreach (DataRow dr in dt.Select())
                                {
                                    deedEx.ImportRow(dr);
                                }

                                DataTable dt1 = tmpBox.GetAllNameEX1(dsexport.Tables[0].Rows[x][5].ToString(), dsexport.Tables[0].Rows[x][6].ToString(), dsexport.Tables[0].Rows[x][7].ToString(), dsexport.Tables[0].Rows[x][8].ToString(), IMGName1);
                                if (NameEx.Rows.Count < 1)
                                {
                                    NameEx = dt1.Clone();
                                }

                                foreach (DataRow dr1 in dt1.Select())
                                {
                                    NameEx.ImportRow(dr1);
                                }
                                DataTable dt2 = tmpBox.GetAllPropEX1(dsexport.Tables[0].Rows[x][5].ToString(), dsexport.Tables[0].Rows[x][6].ToString(), dsexport.Tables[0].Rows[x][7].ToString(), dsexport.Tables[0].Rows[x][8].ToString(), IMGName1);
                                if (PropEx.Rows.Count < 1)
                                {
                                    PropEx = dt2.Clone();
                                }

                                foreach (DataRow dr2 in dt2.Select())
                                {
                                    PropEx.ImportRow(dr2);
                                }
                                DataTable csvdt1 = tmpBox.GetcsvAllPropEX1(dsexport.Tables[0].Rows[x][5].ToString(), dsexport.Tables[0].Rows[x][6].ToString(), dsexport.Tables[0].Rows[x][7].ToString(), dsexport.Tables[0].Rows[x][8].ToString(), IMGName1);
                                if (CSVPropEx1.Rows.Count < 1)
                                {
                                    CSVPropEx1 = csvdt1.Clone();
                                }

                                foreach (DataRow dr3 in csvdt1.Select())
                                {
                                    CSVPropEx1.ImportRow(dr3);
                                }
                                //
                                DataTable csvdt = tmpBox.GetAlloutsideWBDeedEX(dsexport.Tables[0].Rows[x][5].ToString(), dsexport.Tables[0].Rows[x][6].ToString(), dsexport.Tables[0].Rows[x][7].ToString(), dsexport.Tables[0].Rows[x][8].ToString(), IMGName1);
                                if (CSVPropEx.Rows.Count < 1)
                                {
                                    CSVPropEx = csvdt.Clone();
                                }

                                foreach (DataRow dr3 in csvdt.Select())
                                {
                                    CSVPropEx.ImportRow(dr3);
                                }
                                //
                                DataTable dtPlot = tmpBox.GetAllOtherPlot(dsexport.Tables[0].Rows[x][5].ToString(), dsexport.Tables[0].Rows[x][6].ToString(), dsexport.Tables[0].Rows[x][7].ToString(), dsexport.Tables[0].Rows[x][8].ToString(), IMGName1);
                                if (PlotEx.Rows.Count < 1)
                                {
                                    PlotEx = dtPlot.Clone();
                                }

                                foreach (DataRow dr in dtPlot.Select())
                                {
                                    PlotEx.ImportRow(dr);
                                }

                                DataTable dtKhatian = tmpBox.GetAllOtherKhatian(dsexport.Tables[0].Rows[x][5].ToString(), dsexport.Tables[0].Rows[x][6].ToString(), dsexport.Tables[0].Rows[x][7].ToString(), dsexport.Tables[0].Rows[x][8].ToString(), IMGName1);
                                if (KhatianEx.Rows.Count < 1)
                                {
                                    KhatianEx = dtKhatian.Clone();
                                }

                                foreach (DataRow dr in dtKhatian.Select())
                                {
                                    KhatianEx.ImportRow(dr);
                                }
                                dList.Add(imgDetails);

                                //else
                                //{
                                //dgvexport.Rows[x].Cells[12].Value = "Skiped(Hold Deed)";
                                //resultMsg = resultMsg + dsexport.Tables[0].Rows[x][0].ToString() + "\r\n";
                                //dgvexport.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                                //dgvexport.CurrentCell = dgvexport.Rows[x].Cells[12];
                                //progressBar1.PerformStep();
                                //}
                                if (dsexport.Tables[0].Rows[x][11].ToString() != "Y")
                                {
                                    dgvexport.Rows[x].Cells[12].Value = "Exported";
                                    dgvexport.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                                    dgvexport.CurrentCell = dgvexport.Rows[x].Cells[12];
                                    progressBar1.PerformStep();
                                }
                                else
                                {
                                    dgvexport.Rows[x].Cells[12].Value = "Exported";
                                    dgvexport.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                                    dgvexport.CurrentCell = dgvexport.Rows[x].Cells[12];
                                    progressBar1.PerformStep();
                                }

                            }
                        }
                        //NameEx.Columns.Remove("Created_by");
                        //NameEx.Columns.Remove("Created_DTTM");
                        dataGridView1.DataSource = deedEx;
                        dataGridView2.DataSource = NameEx;
                        dataGridView3.DataSource = CSVPropEx1;
                        dgvoutside.DataSource = CSVPropEx;
                        dgvPlot.DataSource = PlotEx;
                        dgvKhatian.DataSource = KhatianEx;
                        globalPath = expFolder + "\\Export\\" + cmbrunNum.Text + "\\" + batchName + "\\" + dsexport.Tables[0].Rows[0][5].ToString() + "\\" + dsexport.Tables[0].Rows[0][6].ToString();
                        if (!Directory.Exists(expFolder + "\\Export\\" + cmbrunNum.Text + "\\" + batchName + "\\" + dsexport.Tables[0].Rows[0][5].ToString() + "\\" + dsexport.Tables[0].Rows[0][6].ToString() + "\\csv"))
                        {
                            Directory.CreateDirectory(expFolder + "\\Export\\" + cmbrunNum.Text + "\\" + batchName + "\\" + dsexport.Tables[0].Rows[0][5].ToString() + "\\" + dsexport.Tables[0].Rows[0][6].ToString() + "\\csv");
                        }
                        tabTextFile(dataGridView1, expFolder + "\\Export\\" + cmbrunNum.Text + "\\" + batchName + "\\" + dsexport.Tables[0].Rows[0][5].ToString() + "\\" + dsexport.Tables[0].Rows[0][6].ToString() + "\\csv\\deed_details.csv");
                        tabTextFile(dataGridView2, expFolder + "\\Export\\" + cmbrunNum.Text + "\\" + batchName + "\\" + dsexport.Tables[0].Rows[0][5].ToString() + "\\" + dsexport.Tables[0].Rows[0][6].ToString() + "\\" + "\\csv\\index_of_name.csv");
                        tabTextFile(dataGridView3, expFolder + "\\Export\\" + cmbrunNum.Text + "\\" + batchName + "\\" + "\\" + dsexport.Tables[0].Rows[0][5].ToString() + "\\" + dsexport.Tables[0].Rows[0][6].ToString() + "\\csv\\index_of_property.csv");
                        tabTextFile(dgvPlot, expFolder + "\\Export\\" + cmbrunNum.Text + "\\" + batchName + "\\" + "\\" + dsexport.Tables[0].Rows[0][5].ToString() + "\\" + dsexport.Tables[0].Rows[0][6].ToString() + "\\csv\\other_plots.csv");
                        tabTextFile(dgvKhatian, expFolder + "\\Export\\" + cmbrunNum.Text + "\\" + batchName + "\\" + dsexport.Tables[0].Rows[0][5].ToString() + "\\" + dsexport.Tables[0].Rows[0][6].ToString() + "\\" + "\\csv\\other_khatian.csv");
                        tabTextFile(dgvoutside, expFolder + "\\Export\\" + cmbrunNum.Text + "\\" + batchName + "\\" + dsexport.Tables[0].Rows[0][5].ToString() + "\\" + dsexport.Tables[0].Rows[0][6].ToString() + "\\" + "\\csv\\index_of_property_out_wb.csv");
                        System.Text.StringBuilder theBuilder = new System.Text.StringBuilder();

                        string total_scanned = wPolicy.getTotalScanImageCount(cmbProject.SelectedValue.ToString(), dgvbatch.Rows[z].Cells[2].Value.ToString()).Tables[0].Rows[0][0].ToString();
                        string hold_deed = wPolicy.getTotalDeedCountonHold(cmbProject.SelectedValue.ToString(), dgvbatch.Rows[z].Cells[2].Value.ToString()).Tables[0].Rows.Count.ToString();
                        string scan_image_hold = wPolicy.getTotalScanImageCountonHold(cmbProject.SelectedValue.ToString(), dgvbatch.Rows[z].Cells[2].Value.ToString()).Tables[0].Rows[0][0].ToString();
                        string addl_Records = string.Empty;
                        int nos = ((NameEx.Rows.Count + PropEx.Rows.Count+ CSVPropEx.Rows.Count ) - (deedEx.Rows.Count * 3));
                        if (nos < 0)
                        {
                            addl_Records = "0";
                        }
                        else
                        {
                            addl_Records = nos.ToString();
                        }
                        System.Text.StringBuilder Builder = new System.Text.StringBuilder();
                        Builder.Append(deedEx.Rows.Count.ToString());
                        Builder.Append(",");
                        Builder.Append(NameEx.Rows.Count.ToString());
                        Builder.Append(",");
                       // Builder.Append(CSVPropEx.Rows.Count + PropEx.Rows.Count).ToString();
                       // Builder.Append(",");
                       // Builder.Append(CSVPropEx1.Rows.Count.ToString());
                       // Builder.Append(",");
                        Builder.Append((CSVPropEx1.Rows.Count +  CSVPropEx.Rows.Count).ToString());
                        Builder.Append(",");
                        Builder.Append(total_scanned);
                        Builder.Append(",");
                        Builder.Append(hold_deed);
                        Builder.Append(",");
                        Builder.Append(scan_image_hold);
                        Builder.Append(",");
                        Builder.Append(addl_Records);
                        using (StreamWriter theWriter = new StreamWriter(expFolder + "\\Export\\" + cmbrunNum.Text + "\\" + batchName + "\\" + dsexport.Tables[0].Rows[0][5].ToString() + "\\" + dsexport.Tables[0].Rows[0][6].ToString() + "\\csv\\summary.csv"))
                        {
                            string detail = "Total Deeds,Total_index1,Total_index2,Total_Scanned_image,Total_Hold_deed,Total_scanned_image_onHold,Additional_records";
                            theWriter.WriteLine(detail);
                            theWriter.Write(Builder.ToString());
                        }
                        string deed_list = string.Empty;
                        deed_list = deedCount(dsexport.Tables[0].Rows[0][5].ToString(), dsexport.Tables[0].Rows[0][6].ToString(), dsexport.Tables[0].Rows[0][7].ToString(), dsexport.Tables[0].Rows[0][8].ToString(), dsexport.Tables[0].Rows[0][9].ToString());
                        System.Text.StringBuilder theBuilder1 = new System.Text.StringBuilder();
                        theBuilder1.Append(deed_list);
                        using (StreamWriter theWriter = new StreamWriter(expFolder + "\\Export\\" + cmbrunNum.Text + "\\" + batchName + "\\" + dsexport.Tables[0].Rows[0][5].ToString() + "\\" + dsexport.Tables[0].Rows[0][6].ToString() + "\\csv\\deedCount.csv"))
                        {
                            string head = "Deeds";
                            theWriter.WriteLine(head);
                            theWriter.Write(theBuilder1.ToString());
                        }
                        Application.DoEvents();
                        lbl.Text = "Exported :" + dgvbatch.Rows[z].Cells[1].Value.ToString();

                        Application.DoEvents();
                        //if (ExportSqlServer(deedEx, NameEx, CSVPropEx, dList, PlotEx, KhatianEx,"aaa") == true)
                        //{

                        //lbl.Text = "Hurray, " + (Convert.ToInt32(lbldeedCount.Text) - Convert.ToInt32(lblholdDeed.Text)).ToString() + " Data Exported to sql Server....";


                        Application.DoEvents();
                        progressBar2.PerformStep();
                        Application.DoEvents();
                        ChangeStatus(dgvbatch.Rows[z].Cells[2].Value.ToString());
                        InsertExportLog(cmbProject.SelectedValue.ToString(), dgvbatch.Rows[z].Cells[2].Value.ToString(), crd, MaxExportCount);
                        pBox = new CtrlBox(Convert.ToInt32(cmbProject.SelectedValue.ToString()), Convert.ToInt32(dgvbatch.Rows[z].Cells[2].Value.ToString()), "1");
                        wfeBox box = new wfeBox(sqlCon, pBox);
                        box.UpdateStatus(eSTATES.BOX_EXPORTED);
                        sqlFileName = string.Empty;
                        txtMsg.Text = resultMsg;
                        btnExport.Enabled = true;


                        //}
                        //else
                        //{
                        //    lblFinalStatus.Text = "Oops, there is an error....";
                        //    sqlFileName = string.Empty;
                        //    btnExport.Enabled = true;

                        //}
                    }
                    progressBar1.Value = 100;
                    lblFinalStatus.Text = "Finished....";
                    
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }
        //public bool checkPage(string[] imageName)
        //{
        //    bool flag = true;
        //}
        private string deedCount(string do_code, string ro_code, string book, string deed_yr, string vol)
        {
            DataSet ds = new DataSet();
            string tmp = string.Empty;
            List<int> dlist = new List<int>();
            List<string> tempList = new List<string>();
            string sql = "select deed_no from deed_details where district_code = '" + do_code + "' and ro_code = '" + ro_code + "' and book = '" + book + "' and deed_year ='" + deed_yr + "' and volume_no = '" + vol + "' order by convert(deed_no,signed int)";
            OdbcDataAdapter sqlAdap = new OdbcDataAdapter(sql, sqlCon);
            sqlAdap.Fill(ds);
            if (ds.Tables[0].Rows.Count > 0)
            {
                int[] str = new int[Convert.ToInt32(ds.Tables[0].Rows.Count)];
                int[] tmpstr = new int[Convert.ToInt32(ds.Tables[0].Rows.Count)];
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    try
                    {
                        dlist.Add(Convert.ToInt32(ds.Tables[0].Rows[i][0].ToString()));
                        tmpstr[i] = Convert.ToInt32(ds.Tables[0].Rows[i][0].ToString());
                        if (tmpstr[i] != 0)
                        {
                            str[i] = tmpstr[i];
                        }
                    }
                    catch (Exception ex)
                    {
                        tempList.Add(ds.Tables[0].Rows[i][0].ToString());
                    }
                }
                List<ArrayList> FilteredLst = GetNumberRange(str);
                for (int i = 0; i < FilteredLst.Count; i++)
                {
                        if (FilteredLst[i].Count == 1 && FilteredLst[i][0].ToString() != "0")
                        {

                            tmp = tmp + FilteredLst[i][0].ToString().PadLeft(5, '0') + ";";

                        }
                        else if (FilteredLst[i][0].ToString() != "0")
                        {
                            tmp = tmp + FilteredLst[i][0].ToString().PadLeft(5, '0') + "-" + FilteredLst[i][FilteredLst[i].Count - 1].ToString().PadLeft(5, '0') + ";";
                        }
                        else
                        {
                            tmp = tmp;
                        }
                    }
                    
                
            }
            for (int b = 0; b < tempList.Count; b++)
            {
                tmp = tmp + tempList[b].ToString().PadLeft(5, '0') + ";";
            }
            tmp = tmp.TrimEnd(';');

            return tmp;
        }
        private List<ArrayList> GetNumberRange(int[] pStr)
        {
            List<ArrayList> lst = new List<ArrayList>();
            ArrayList intLst = null;
            if (pStr == null) { return lst; }
            if (pStr.Length == 0) { return lst; }
            int[] str = pStr;
            for (int i = 0; i < str.Length; i++)
            {
                if (i == 0)
                {
                    intLst = new ArrayList();
                }
                
                    intLst.Add(str[i]);
                
               
                if ((str[i + 1] - str[i]) != 1) //if (i+1)-i!=1 then add the arraylist into the generic list
                {
                    lst.Add(intLst);
                    intLst = new ArrayList();
                    if ((i + 2) == str.Length) //If last value
                    {
                        intLst.Add(str[i + 1]);
                        lst.Add(intLst);
                        break;
                    }
                }
                else
                {
                    if ((i + 2) == str.Length) // If last value
                    {
                        intLst.Add(str[i + 1]);
                        lst.Add(intLst);
                        break;
                    }
                }
            }
            return lst;
        }
        public void cleargrid()
        {
            dgvexport.DataSource = null;
            dataGridView1.DataSource = null;
            dataGridView2.DataSource = null;
            dataGridView3.DataSource = null;
            dgvPlot.DataSource = null;
            dgvKhatian.DataSource = null;
            if (dgvexport.Columns.Contains("Status"))
            {
                dgvexport.Columns.Remove("Status");
            }
            lbl.Text = "";
            progressBar1.Value = 0;
            txtMsg.Text = "";
            tblExp.SelectedTab = tabPage1;
        }
        private void InsertExportLog(string proj_key, string batch_key, Credentials crd, int count)
        {
            wPolicy.UpdateExportLog(proj_key, batch_key, crd, count);

        }
        static DataTable ConvertListToDataTable(List<string[]> list)
        {
            DataTable table = new DataTable();
            int columns = 0;
            foreach (var array in list)
            {
                if (array.Length > columns)
                {
                    columns = array.Length;
                }
            }
            for (int i = 0; i < columns; i++)
            {
                table.Columns.Add();
            }
            foreach (var array in list)
            {
                table.Rows.Add(array);
            }
            return table;
        }

        private Boolean TakeBackup(string path)
        {
            //string sqlConnectionString = "Server=localhost\\SQLEXPRESS;Initial Catalog=IGR;Integrated Security=True;Trusted_Connection=Yes;";
            //string script = "BACKUP DATABASE [IGR] TO  DISK = '"+path+"\\IGR.bak"+"' WITH  INIT ,  NOUNLOAD ,  NAME = N'IGR backup',  NOSKIP ,  STATS = 10,  NOFORMAT";
            //SqlConnection conn = new SqlConnection(sqlConnectionString);
            //Server server = new Server(new ServerConnection(conn));
            //server.ConnectionContext.ExecuteNonQuery(script);
            return true;
        }
        static DataTable ConvertListToDataTable(List<DeedImageDetails> list)
        {
            // New table.
            DataTable table = new DataTable();

            // Get max columns.
            int columns = 0;
            foreach (var array in list)
            {
                columns = 6;
            }

            // Add columns.
            for (int i = 0; i < columns; i++)
            {
                table.Columns.Add();
            }

            // Add rows.
            foreach (DeedImageDetails array in list)
            {
                //string id = DistrictCode + dtls.RoCode + dtls.DeedYear + dtls.Book + dtls.DeedNumber;
                table.Rows.Add(array);
            }

            return table;
        }
        public void tabTextFile(DataGridView dg, string filename)
        {

            DataSet ds = new DataSet();
            DataTable dtSource = null;
            DataTable dt = new DataTable();
            DataRow dr;
            if (dg.DataSource != null)
            {
                if (dg.DataSource.GetType() == typeof(DataSet))
                {
                    DataSet dsSource = (DataSet)dg.DataSource;
                    if (dsSource.Tables.Count > 0)
                    {
                        string strTables = string.Empty;
                        foreach (DataTable dt1 in dsSource.Tables)
                        {
                            strTables += TableToString(dt1);
                            strTables += "\r\n\r\n";
                        }
                        if (strTables != string.Empty)
                            SaveDataGridData(strTables, filename);
                    }
                }
                else
                {
                    if (dg.DataSource.GetType() == typeof(DataTable))
                        dtSource = (DataTable)dg.DataSource;
                    if (dtSource != null)

                        SaveDataGridData(TableToString(dtSource), filename);
                }
            }

        }
        private void SaveDataGridData(string strData, string strFileName)
        {
            FileStream fs;
            TextWriter tw = null;
            try
            {
                if (File.Exists(strFileName))
                {
                    fs = new FileStream(strFileName, FileMode.Open);
                }
                else
                {
                    fs = new FileStream(strFileName, FileMode.Create);
                }
                tw = new StreamWriter(fs);
                tw.Write(strData);
            }
            finally
            {
                if (tw != null)
                {
                    tw.Flush();
                    tw.Close();
                }
            }
        }
        private string TableToString(DataTable dt)
        {
            string strData = string.Empty;
            string sep = string.Empty;
            if (dt.Rows.Count > 0)
            {
                foreach (DataColumn c in dt.Columns)
                {
                    if (c.DataType != typeof(System.Guid) &&
                    c.DataType != typeof(System.Byte[]))
                    {
                        strData += sep + c.ColumnName;
                        sep = ",";
                    }
                }
                strData += "\r\n";
                foreach (DataRow r in dt.Rows)
                {
                    sep = string.Empty;
                    foreach (DataColumn c in dt.Columns)
                    {
                        if (c.DataType != typeof(System.Guid) &&
                        c.DataType != typeof(System.Byte[]))
                        {
                            if (!Convert.IsDBNull(r[c.ColumnName]))

                                strData += sep +
                                '"' + r[c.ColumnName].ToString().Replace("\n", " ").Replace(",", "-") + '"';

                            else

                                strData += sep + "";
                            sep = ",";

                        }
                    }
                    strData += "\r\n";

                }
            }
            else
            {
                //strData += "\r\n---> Table was empty!";
                foreach (DataColumn c in dt.Columns)
                {
                    if (c.DataType != typeof(System.Guid) &&
                    c.DataType != typeof(System.Byte[]))
                    {
                        strData += sep + c.ColumnName;
                        sep = ",";
                    }
                }
                strData += "\r\n";
            }
            return strData;
        }

        private void ChangeStatus()
        {

            if (dgvexport.Rows.Count > 0)
            {
                for (int i = 0; i < dgvexport.Rows.Count; i++)
                {
                    if (dgvexport.Rows[i].Cells[11].Value.ToString() == "N")
                    {
                        pImage = new CtrlImage(Convert.ToInt32(cmbrunNum.SelectedValue.ToString()), Convert.ToInt32(cmbProject.SelectedValue.ToString()), "1", dgvexport.Rows[i].Cells[0].Value.ToString(), string.Empty, string.Empty);
                        wImage = new wfeImage(sqlCon, pImage);
                        pPolicy = new CtrlPolicy(Convert.ToInt32(cmbrunNum.SelectedValue.ToString()), Convert.ToInt32(cmbProject.SelectedValue.ToString()), "1", dgvexport.Rows[i].Cells[0].Value.ToString());
                        wPolicy = new wfePolicy(sqlCon, pPolicy);
                        if (wImage.GetImageCount(eSTATES.PAGE_SCANNED) == false)
                        {
                            crd.created_dttm = dbcon.GetCurrenctDTTM(1, sqlCon);
                            wPolicy.UpdateStatus(eSTATES.POLICY_EXPORTED, crd, true);
                            wPolicy.UnLockPolicy();
                            wPolicy.UpdateBatchStatus(cmbrunNum.SelectedValue.ToString(), cmbProject.SelectedValue.ToString());
                            /////update into transaction log
                            //wPolicy.UpdateTransactionLog(eSTATES.POLICY_EXPORTED, crd);
                        }
                    }
                }

            }
        }
        private void ChangeStatus(string batchKey)
        {

            if (dgvexport.Rows.Count > 0)
            {
                for (int i = 0; i < dgvexport.Rows.Count; i++)
                {
                    if (dgvexport.Rows[i].Cells[11].Value.ToString() == "N")
                    {
                        pImage = new CtrlImage(Convert.ToInt32(cmbrunNum.SelectedValue.ToString()), Convert.ToInt32(batchKey), "1", dgvexport.Rows[i].Cells[0].Value.ToString(), string.Empty, string.Empty);
                        wImage = new wfeImage(sqlCon, pImage);
                        pPolicy = new CtrlPolicy(Convert.ToInt32(cmbrunNum.SelectedValue.ToString()), Convert.ToInt32(batchKey), "1", dgvexport.Rows[i].Cells[0].Value.ToString());
                        wPolicy = new wfePolicy(sqlCon, pPolicy);
                        if (wImage.GetImageCount(eSTATES.PAGE_SCANNED) == false)
                        {
                            crd.created_dttm = dbcon.GetCurrenctDTTM(1, sqlCon);
                            wPolicy.UpdateStatus(eSTATES.POLICY_EXPORTED, crd, true);
                            wPolicy.UnLockPolicy();
                            wPolicy.UpdateBatchStatus(cmbrunNum.SelectedValue.ToString(), batchKey);
                            /////update into transaction log
                            //wPolicy.UpdateTransactionLog(eSTATES.POLICY_EXPORTED, crd);
                        }
                    }
                }

            }
        }
        private bool ExportSqlServer(DataTable DeedSQL, DataTable NameSQL, DataTable PropertySQL, List<DeedImageDetails> pList, DataTable plotSQL, DataTable KhatianSQL, string commment)
        {
            return true;
        }
        private bool ExportSqlServer(DataTable DeedSQL, DataTable NameSQL, DataTable PropertySQL, List<DeedImageDetails> pList, DataTable plotSQL, DataTable KhatianSQL)
        {
            SqlConnection CN = null;
            try
            {
                string tempfilename = DeedSQL.Rows[0]["district_code"].ToString() + DeedSQL.Rows[0]["ro_code"].ToString() + DeedSQL.Rows[0]["book"].ToString() + DeedSQL.Rows[0]["deed_year"].ToString() + DeedSQL.Rows[0]["volume_no"].ToString();
                string dbTemplatePath = System.IO.Path.GetDirectoryName(Application.ExecutablePath) + "\\db_Template" + "\\IGR.mdf";
                string destDbFilePath = exportPath + "\\SQLSERVEREXPORT\\" + tempfilename;

                if (!File.Exists(dbTemplatePath))
                {
                    MessageBox.Show("Template mdf not present");
                    return false;
                }

                if (Directory.Exists(destDbFilePath))
                {
                    Directory.Delete(destDbFilePath, true);

                }
                Directory.CreateDirectory(destDbFilePath);
                File.Copy(dbTemplatePath, destDbFilePath + "\\" + sqlFileName, true);
                DirectoryInfo dinfo = new DirectoryInfo(@destDbFilePath);
                //FileInfo[] Files = dinfo.GetFiles("*.ldf");
                //foreach (FileInfo file in Files)
                //{
                //    file.Delete();
                //}

                string sql = @"Server=" + sqlIp + @"\SQLEXPRESS;AttachDbFilename=" + destDbFilePath + "\\" + sqlFileName + "; Database=IGR;Trusted_Connection=Yes;";
                CN = new SqlConnection(sql);
                bool result = false;
                CN.Open();
                SqlTransaction sqltrans = null;
                try
                {
                    sqltrans = CN.BeginTransaction();
                }
                catch (Exception ex)
                {
                    CN.Open();
                    sqltrans = CN.BeginTransaction();
                }
                if (SaveDeedDetails(CN, DeedSQL, sqltrans) == true)
                {
                    if (SaveDeedImage(CN, pList, sqltrans) == true)
                    {
                        if (SaveIndex1(CN, NameSQL, sqltrans) == true)
                        {
                            if (SaveIndex2(CN, PropertySQL, sqltrans) == true)
                            {
                                if (SaveOtherPlot(CN, plotSQL, sqltrans) == true)
                                {
                                    if (SaveOtherKhatian(CN, KhatianSQL, sqltrans) == true)
                                    {
                                        sqltrans.Commit();
                                        if (Directory.Exists(globalPath + "\\" + tempfilename))
                                        {
                                            Directory.Delete(globalPath + "\\" + tempfilename, true);
                                        }
                                        Directory.CreateDirectory(globalPath + "\\" + tempfilename);
                                        TakeBackup(destDbFilePath);
                                        DropDb(CN);
                                        //DetachDb(CN);
                                        if (CN.State == ConnectionState.Open)
                                        {
                                            CN.Close();
                                            SqlConnection.ClearPool(CN);
                                        }
                                        string[] filenames = Directory.GetFiles(destDbFilePath);
                                        foreach (string s in filenames)
                                        {
                                            string fileName = System.IO.Path.GetFileName(s);

                                            string destFile = System.IO.Path.Combine(globalPath + "\\" + tempfilename, fileName);
                                            System.IO.File.Copy(s, destFile, true);
                                            File.Move(globalPath + "\\" + tempfilename + "\\" + fileName, globalPath + "\\" + tempfilename + "\\" + "IGR_" + tempfilename + ".bak");
                                        }
                                        result = true;
                                    }
                                }
                            }

                        }
                    }

                }
                return result;
            }
            catch (Exception ex)
            {
                DropDb(CN);
                CN.Close();
                MessageBox.Show(this, "Error while exporting....Please Restart your Application..." + ex.Message.ToString(), "IGR....");
                return false;
            }
            finally
            {
                if (CN.State == ConnectionState.Open)
                {
                    CN.Close();
                    SqlConnection.ClearPool(CN);
                } //DetachDb(CN); }
            }
        }
        private bool DetachDb(SqlConnection pSqlCon)
        {
            try
            {
                pSqlCon.Close();
                string sql = @"Data Source=" + sqlIp + @"\SQLEXPRESS;Initial Catalog=master;Integrated Security=SSPI;";
                SqlConnection CN = new SqlConnection(sql);
                CN.Open();

                SqlCommand sqlCom = new SqlCommand();
                sqlCom.Connection = CN;
                sqlCom.CommandText = "sys.sp_detach_db IGR";
                sqlCom.ExecuteNonQuery();
                CN.Close();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        private bool DropDb(SqlConnection pSqlCon)
        {
            try
            {
                pSqlCon.Close();
                string qry = "ALTER DATABASE IGR SET SINGLE_USER WITH ROLLBACK IMMEDIATE;";
                string sql = @"Data Source=" + sqlIp + @"\SQLEXPRESS;Initial Catalog=master;Integrated Security=SSPI;";
                SqlConnection CN = new SqlConnection(sql);
                CN.Open();
                SqlCommand sqlCom1 = new SqlCommand();
                sqlCom1.Connection = CN;
                sqlCom1.CommandText = qry;
                sqlCom1.ExecuteNonQuery();
                SqlCommand sqlCom = new SqlCommand();
                sqlCom.Connection = CN;
                sqlCom.CommandText = "drop database IGR";
                sqlCom.ExecuteNonQuery();
                CN.Close();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        private bool SaveDeedDetails(SqlConnection CN, DataTable dt, SqlTransaction sqltrans)
        {
            try
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    string qry = "insert into Deed_details(District_Code,RO_Code,Book,Deed_year,Deed_no,Serial_No,Serial_Year,tran_maj_code,tran_min_code,Volume_No,Page_From,Page_To,Date_of_Completion,Date_of_Delivery,Deed_Remarks,scan_doc_type) values ('" + dt.Rows[i][0].ToString() + "','" + dt.Rows[i][1].ToString() + "','" + dt.Rows[i][2].ToString() + "','" + dt.Rows[i][3].ToString() + "','" + dt.Rows[i][4].ToString() + "','" + dt.Rows[i][5].ToString() + "','" + dt.Rows[i][6].ToString() + "','" + dt.Rows[i][7].ToString() + "','" + dt.Rows[i][8].ToString() + "','" + dt.Rows[i][9].ToString() + "','" + dt.Rows[i][10].ToString() + "','" + dt.Rows[i][11].ToString() + "','" + dt.Rows[i][12].ToString() + "','" + dt.Rows[i][13].ToString() + "','" + dt.Rows[i][14].ToString() + "','" + dt.Rows[i][15].ToString() + "')";
                    //Initialize SqlCommand object for insert.
                    SqlCommand SqlCom = new SqlCommand(qry, CN);
                    SqlCom.Transaction = sqltrans;
                    SqlCom.ExecuteNonQuery();
                }
                return true;
            }
            catch (Exception ex)
            { return false; }
        }
        private bool SaveDeedImage(SqlConnection CN, List<DeedImageDetails> pList, SqlTransaction sqltrans)
        {
            try
            {
                foreach (DeedImageDetails dtls in pList)
                {
                    //byte[] img = dt.Rows[i][16];
                    //string qry = "insert into Deed_Image(District_Code,RO_Code,Book,Deed_year,Deed_no,Deed_Image) values ('" + dtls.DistrictCode + "','" + dtls.RoCode + "','" + dtls.Book  + "','" + dtls.DeedYear + "','" + dtls.DeedNumber + "'," + (Object)dtls.DeedImage + ")";
                    ////Initialize SqlCommand object for insert.
                    //SqlCommand SqlCom = new SqlCommand(qry, CN);
                    //SqlCom.Transaction = sqltrans;
                    //SqlCom.ExecuteNonQuery();
                    string qry = "insert into Deed_Image(id_no,Deed_Image) values (@id_no,@DeedImage)";
                    //Initialize SqlCommand object for insert.
                    SqlCommand SqlCom = new SqlCommand(qry, CN);
                    SqlCom.Transaction = sqltrans;
                    //We are passing Original Image Path and Image byte data as sql parameters.
                    //SqlCom.Parameters.Add(new SqlParameter("@District_code", (object)dtls.DistrictCode));
                    //SqlCom.Parameters.Add(new SqlParameter("@Ro_Code", (object)dtls.RoCode));
                    //SqlCom.Parameters.Add(new SqlParameter("@Book", (object)dtls.Book));
                    //SqlCom.Parameters.Add(new SqlParameter("@Deed_Year", (object)dtls.DeedYear));
                    string id = dtls.DistrictCode + dtls.RoCode + dtls.DeedYear + dtls.Book + dtls.DeedNumber;
                    //SqlCom.Parameters.Add(new SqlParameter("@Deed_Number", (object)dtls.DeedNumber));
                    SqlCom.Parameters.Add(new SqlParameter("@id_no", id));
                    SqlCom.Parameters.Add(new SqlParameter("@DeedImage", dtls.DeedImage));
                    SqlCom.ExecuteNonQuery();
                }
                return true;
            }
            catch (Exception ex)
            { return false; }
        }
        private bool SaveIndex1(SqlConnection CN, DataTable dt, SqlTransaction sqltrans)
        {
            try
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    string qry = "insert into index_of_name(District_Code,RO_Code,Book,Deed_year,Deed_no,Item_no,initial_name,First_name,Last_name,Party_code,Admit_code,Address,Address_district_code,Address_district_name,Address_ps_code,Address_ps_name,Father_mother,Rel_code,Relation,occupation_code,religion_code,more,pin,city,other_party_code,linked_to) values('" + dt.Rows[i][0].ToString() + "','" + dt.Rows[i][1].ToString() + "','" + dt.Rows[i][2].ToString() + "','" + dt.Rows[i][3].ToString() + "','" + dt.Rows[i][4].ToString() + "','" + dt.Rows[i][5].ToString() + "','" + dt.Rows[i][6].ToString() + "','" + dt.Rows[i][7].ToString() + "','" + dt.Rows[i][8].ToString() + "','" + dt.Rows[i][9].ToString() + "','" + dt.Rows[i][10].ToString() + "','" + dt.Rows[i][11].ToString() + "','" + dt.Rows[i][12].ToString() + "','" + dt.Rows[i][13].ToString() + "','" + dt.Rows[i][14].ToString() + "','" + dt.Rows[i][15].ToString() + "','" + dt.Rows[i][16].ToString() + "','" + dt.Rows[i][17].ToString() + "','" + dt.Rows[i][18].ToString() + "','" + dt.Rows[i][19].ToString() + "','" + dt.Rows[i][20].ToString() + "','" + dt.Rows[i][21].ToString() + "','" + dt.Rows[i][22].ToString() + "','" + dt.Rows[i][23].ToString() + "','" + dt.Rows[i][24].ToString() + "','" + dt.Rows[i][25].ToString() + "')";
                    //Initialize SqlCommand object for insert.
                    SqlCommand SqlCom = new SqlCommand(qry, CN);
                    SqlCom.Transaction = sqltrans;
                    SqlCom.ExecuteNonQuery();
                }
                return true;
            }
            catch (Exception ex)
            { return false; }
        }
        private bool SaveIndex2(SqlConnection CN, DataTable dt, SqlTransaction sqltrans)
        {
            try
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    //string qry = "insert into index_of_property(District_Code,RO_Code,Book,Deed_year,Deed_no,Item_no,Property_district_code,Property_ro_code,ps_code,moucode,Area_type,GP_Muni_Corp_Code,Ward,Holding,Premises,road_code,Plot_code_type,Road,Plot_No,Bata_No,Khatian_type,khatian_No,bata_khatian_no,property_type,Land_Area_acre,Land_Area_bigha,Land_Area_decimal,Land_Area_katha,Land_Area_chatak,Land_Area_sqfeet,Structure_area_in_sqFeet,ref_ps,ref_mouza,jl_no,other_plots,other_khatian) values ('" + dt.Rows[i][0].ToString() + "','" + dt.Rows[i][1].ToString() + "','" + dt.Rows[i][2].ToString() + "','" + dt.Rows[i][3].ToString() + "','" + dt.Rows[i][4].ToString() + "','" + dt.Rows[i][5].ToString() + "','" + dt.Rows[i][6].ToString() + "','" + dt.Rows[i][7].ToString() + "','" + dt.Rows[i][8].ToString() + "','" + dt.Rows[i][9].ToString() + "','" + dt.Rows[i][10].ToString() + "','" + dt.Rows[i][11].ToString() + "','" + dt.Rows[i][12].ToString() + "','" + dt.Rows[i][13].ToString() + "','" + dt.Rows[i][14].ToString() + "','" + dt.Rows[i][15].ToString() + "','" + dt.Rows[i][16].ToString() + "','" + dt.Rows[i][17].ToString() + "','" + dt.Rows[i][18].ToString() + "','" + dt.Rows[i][19].ToString() + "','" + dt.Rows[i][20].ToString() + "','" + dt.Rows[i][21].ToString() + "','" + dt.Rows[i][22].ToString() + "','" + dt.Rows[i][23].ToString() + "','" + dt.Rows[i][24].ToString() + "','" + dt.Rows[i][25].ToString() + "','" + dt.Rows[i][26].ToString() + "','" + dt.Rows[i][27].ToString() + "','" + dt.Rows[i][28].ToString() + "','" + dt.Rows[i][29].ToString() + "','" + dt.Rows[i][30].ToString() + "','" + dt.Rows[i][31].ToString() + "','" + dt.Rows[i][32].ToString() + "','" + dt.Rows[i][33].ToString() + "','" + dt.Rows[i][34].ToString() + "','" + dt.Rows[i][35].ToString() + "')";
                    string qry = "insert into index_of_property(District_Code,RO_Code,Book,Deed_year,Deed_no,Item_no,Property_district_code,Property_district_name,Property_ro_code,ps_code,ps_name,moucode,mouja,Area_type,GP_Muni_Corp_Code,GP_Muni_name,Ward,Holding,Premises,road_code,Plot_code_type,Road,Plot_No,Bata_No,Khatian_type,khatian_No,bata_khatian_no,property_type,Land_Area_acre,Land_Area_bigha,Land_Area_decimal,Land_Area_katha,Land_Area_chatak,Land_Area_sqfeet,Structure_area_in_sqFeet,ref_ps,ref_mouza,jl_no,other_plots,other_khatian,land_type,refjl_no) values ('" + dt.Rows[i][0].ToString() + "','" + dt.Rows[i][1].ToString() + "','" + dt.Rows[i][2].ToString() + "','" + dt.Rows[i][3].ToString() + "','" + dt.Rows[i][4].ToString() + "','" + dt.Rows[i][5].ToString() + "','" + dt.Rows[i][6].ToString() + "','" + dt.Rows[i][7].ToString() + "','" + dt.Rows[i][8].ToString() + "','" + dt.Rows[i][9].ToString() + "','" + dt.Rows[i][10].ToString() + "','" + dt.Rows[i][11].ToString() + "','" + dt.Rows[i][12].ToString() + "','" + dt.Rows[i][13].ToString() + "','" + dt.Rows[i][14].ToString() + "','" + dt.Rows[i][15].ToString() + "','" + dt.Rows[i][16].ToString() + "','" + dt.Rows[i][17].ToString() + "','" + dt.Rows[i][18].ToString() + "','" + dt.Rows[i][19].ToString() + "','" + dt.Rows[i][20].ToString() + "','" + dt.Rows[i][21].ToString() + "','" + dt.Rows[i][22].ToString() + "','" + dt.Rows[i][23].ToString() + "','" + dt.Rows[i][24].ToString() + "','" + dt.Rows[i][25].ToString() + "','" + dt.Rows[i][26].ToString() + "','" + dt.Rows[i][27].ToString() + "','" + dt.Rows[i][28].ToString() + "','" + dt.Rows[i][29].ToString() + "','" + dt.Rows[i][30].ToString() + "','" + dt.Rows[i][31].ToString() + "','" + dt.Rows[i][32].ToString() + "','" + dt.Rows[i][33].ToString() + "','" + dt.Rows[i][34].ToString() + "','" + dt.Rows[i][35].ToString() + "','" + dt.Rows[i][36].ToString() + "','" + dt.Rows[i][37].ToString() + "','" + dt.Rows[i][38].ToString() + "','" + dt.Rows[i][39].ToString() + "','" + dt.Rows[i][40].ToString() + "','" + dt.Rows[i][41].ToString() + "')";
                    //Initialize SqlCommand object for insert.
                    SqlCommand SqlCom = new SqlCommand(qry, CN);
                    SqlCom.Transaction = sqltrans;
                    SqlCom.ExecuteNonQuery();
                }
                return true;
            }
            catch (Exception ex)
            { return false; }
        }
        private bool SaveOtherPlot(SqlConnection CN, DataTable dt, SqlTransaction sqltrans)
        {
            try
            {
                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        string qry = "insert into other_plots(District_Code,RO_Code,Book,Deed_year,Deed_no,Item_no,other_plots) values ('" + dt.Rows[i][0].ToString() + "','" + dt.Rows[i][1].ToString() + "','" + dt.Rows[i][2].ToString() + "','" + dt.Rows[i][3].ToString() + "','" + dt.Rows[i][4].ToString() + "','" + dt.Rows[i][5].ToString() + "','" + dt.Rows[i][6].ToString() + "')";
                        //Initialize SqlCommand object for insert.
                        SqlCommand SqlCom = new SqlCommand(qry, CN);
                        SqlCom.Transaction = sqltrans;
                        SqlCom.ExecuteNonQuery();
                    }
                }
                return true;
            }
            catch (Exception ex)
            { return false; }
        }
        private bool SaveOtherKhatian(SqlConnection CN, DataTable dt, SqlTransaction sqltrans)
        {
            try
            {
                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        string qry = "insert into other_khatian(District_Code,RO_Code,Book,Deed_year,Deed_no,Item_no,other_khatian) values ('" + dt.Rows[i][0].ToString() + "','" + dt.Rows[i][1].ToString() + "','" + dt.Rows[i][2].ToString() + "','" + dt.Rows[i][3].ToString() + "','" + dt.Rows[i][4].ToString() + "','" + dt.Rows[i][5].ToString() + "','" + dt.Rows[i][6].ToString() + "')";
                        //Initialize SqlCommand object for insert.
                        SqlCommand SqlCom = new SqlCommand(qry, CN);
                        SqlCom.Transaction = sqltrans;
                        SqlCom.ExecuteNonQuery();
                    }
                }
                return true;
            }
            catch (Exception ex)
            { return false; }
        }

        private void dgvbatch_Click(object sender, EventArgs e)
        {

        }

        private void cmbProject_Click(object sender, EventArgs e)
        {
            PopulateBatch();
        }

        private void dgvbatch_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            populateDeeds(dgvbatch.CurrentRow.Cells[2].Value.ToString(), dgvbatch.CurrentRow.Cells[3].Value.ToString());

        }
        private void PopulateSelectedBatchCount()
        {
            int StatusStep = 0;
            try
            {
                if (dgvbatch.Rows.Count > 0)
                {
                    for (int x = 0; x < dgvbatch.Rows.Count; x++)
                    {
                        if (Convert.ToBoolean(dgvbatch.Rows[x].Cells[0].Value))
                        {
                            StatusStep = StatusStep + 1;
                        }

                    }
                    lblBatchSelected.Text = StatusStep.ToString();
                }
            }
            catch (Exception ex)
            {
                lblBatchSelected.Text = "0";
            }
        }

        private void dgvbatch_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 0)
            {
                dgvbatch.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }

        private void dgvbatch_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 0)
                PopulateSelectedBatchCount();
        }

        private void btnPopulate_Click(object sender, EventArgs e)
        {

        }

        private void cmdValidatefiles_Click(object sender, EventArgs e)
        {
            try
            {

                btnExport.Enabled = false;
                string batchPath = string.Empty;
                string batchName = string.Empty;
                string resultMsg = "Hold Deeds" + "\r\n";
                DataTable deedEx = new DataTable();
                DataTable NameEx = new DataTable();
                DataTable PropEx = new DataTable();
                DataTable CSVPropEx = new DataTable();
                DataTable PlotEx = new DataTable();
                DataTable KhatianEx = new DataTable();
                string expFolder = string.Empty;
                bool isDeleted = false;
                int MaxExportCount = 0;
                int StatusStep = 0;
                config = new ImageConfig(ihConstants.CONFIG_FILE_PATH);
                expFolder = config.GetValue(ihConstants.EXPORT_FOLDER_SECTION, ihConstants.EXPORT_FOLDER_KEY).Trim();
                int len = expFolder.IndexOf('\0');
                expFolder = expFolder.Substring(0, len);
                List<DeedImageDetails> dList = new List<DeedImageDetails>();


                lblFinalStatus.Text = "Please wait while Validating....  ";
                Application.DoEvents();
                if (dgvbatch.Rows.Count > 0)
                {
                    //for (int x = 0; x < dgvbatch.Rows.Count; x++)
                    //{
                    //    //if (dgvbatch.Rows[x].Cells[0].Value))
                    //    //{
                    //    //    StatusStep = StatusStep + 1;
                    //    //    dgvbatch.Rows[x].Selected = false; 
                    //    //}
                    //}
                    StatusStep = dgvbatch.Rows.Count;

                    int step = 100 / StatusStep;
                    progressBar2.Step = step;
                    for (int z = 0; z < dgvbatch.Rows.Count; z++)
                    {

                        dgvexport.DataSource = null;
                        dataGridView1.DataSource = null;
                        dataGridView2.DataSource = null;
                        dataGridView3.DataSource = null;
                        dgvKhatian.DataSource = null;
                        dgvPlot.DataSource = null;
                        deedEx.Clear();
                        NameEx.Clear();
                        CSVPropEx.Clear();
                        PlotEx.Clear();
                        KhatianEx.Clear();
                        populateDeeds(dgvbatch.Rows[z].Cells[2].Value.ToString(), dgvbatch.Rows[z].Cells[3].Value.ToString());
                        MaxExportCount = wPolicy.getMaxExportCount(cmbProject.SelectedValue.ToString(), dgvbatch.Rows[z].Cells[2].Value.ToString());
                        if (dgvexport.Rows.Count > 0)
                        {
                            for (int i = 0; i < dgvexport.Rows.Count; i++)
                            {
                                if (Convert.ToInt32(dgvexport.Rows[i].Cells["status"].Value.ToString()) == 30 || Convert.ToInt32(dgvexport.Rows[i].Cells["status"].Value.ToString()) == 21)
                                {
                                    dgvexport.Rows[i].DefaultCellStyle.BackColor = Color.Red;
                                    MessageBox.Show("There is some Problem in one or more deeds, Please Check and Retry..., Export Failed");
                                    btnExport.Enabled = true;
                                    return;
                                }
                            }
                        }

                        if (dgvexport.Rows.Count > 0)
                        {
                            Application.DoEvents();
                            dgvbatch.Rows[z].DefaultCellStyle.BackColor = Color.GreenYellow;
                            int i1 = 100 / dsexport.Tables[0].Rows.Count;
                            progressBar1.Step = i1;
                            progressBar1.Increment(i1);
                            Application.DoEvents();
                            wBatch = new wfeBatch(sqlCon);
                            batchPath = wBatch.GetPath(Convert.ToInt32(cmbProject.SelectedValue.ToString()), Convert.ToInt32(dgvbatch.Rows[z].Cells[2].Value.ToString()));
                            batchPath = batchPath + "\\1\\Export";
                            for (int x = 0; x < dsexport.Tables[0].Rows.Count; x++)
                            {

                                DeedImageDetails imgDetails = new DeedImageDetails();
                                wfeImage tmpBox = new wfeImage(sqlCon);
                                DataSet dsimage = new DataSet();
                                Application.DoEvents();
                                lbl.Text = "Validating :" + dgvbatch.Rows[z].Cells[1].Value.ToString();
                                Application.DoEvents();
                                string aa = wBatch.GetPath(Convert.ToInt32(cmbProject.SelectedValue.ToString()), Convert.ToInt32(dgvbatch.Rows[z].Cells[2].Value.ToString()));
                                sqlFileName = dsexport.Tables[0].Rows[x][0].ToString();
                                int index = sqlFileName.IndexOf('[');
                                sqlFileName = sqlFileName.Substring(0, index);
                                batchName = sqlFileName.Substring(0, index);
                                batchName = batchName + dsexport.Tables[0].Rows[x][9].ToString() + "_" + MaxExportCount.ToString();
                                sqlFileName = sqlFileName + dsexport.Tables[0].Rows[x][9].ToString() + ".mdf";
                                dsimage = tmpBox.GetAllExportedImage(dsexport.Tables[0].Rows[x][1].ToString(), dsexport.Tables[0].Rows[x][2].ToString(), dsexport.Tables[0].Rows[x][3].ToString(), dsexport.Tables[0].Rows[x][0].ToString());
                                imageName = new string[dsimage.Tables[0].Rows.Count];
                                string IMGName = dsexport.Tables[0].Rows[x][0].ToString();
                                string IMGName1 = IMGName.Split(new char[] { '[', ']' })[1];
                                IMGName = IMGName.Replace("[", "");
                                IMGName = IMGName.Replace("]", "");
                                string fileName = dsexport.Tables[0].Rows[x][0].ToString();
                                if (dsimage.Tables[0].Rows.Count > 0)
                                {
                                    for (int a = 0; a < dsimage.Tables[0].Rows.Count; a++)
                                    {
                                        //                                        imageName[a] = dsexport.Tables[0].Rows[x][4].ToString() + "\\QC" + "\\" + dsimage.Tables[0].Rows[a]["page_name"].ToString();
                                        imageName[a] = aa + "\\1\\" + fileName + "\\QC" + "\\" + dsimage.Tables[0].Rows[a]["page_name"].ToString();
                                    }
                                    
                                    if (imageName.Length != 0)
                                    {

                                        //sumit for export problem
                                        for (int i = 0; i < imageName.Length; i++)
                                        {
                                            if (File.Exists(imageName[i]))
                                            {

                                            }
                                            else
                                            {
                                                MessageBox.Show("File not found or may be corrupted... " + imageName[i] + " Volume No: " + dsexport.Tables[0].Rows[i][9].ToString() + " Year = " + dsexport.Tables[0].Rows[i][8].ToString());
                                                return;
                                            }

                                        }

                                    }

                                }
                                else
                                {
                                    MessageBox.Show(this,"No Image found for Deed No: " + fileName + ",Export aborted","Error",MessageBoxButtons.OK,MessageBoxIcon.Error);
                                    return;
                                }
                            }
                        }

                    }
                }
                lblFinalStatus.Text = "Data Validated successfully...";
                btnExport.Enabled = true;
                progressBar2.Increment(100);
                progressBar1.Increment(100);
                //dgvexport.Rows[i].DefaultCellStyle.BackColor = Color.Red;
                for (int i = 0; i < dgvbatch.Rows.Count; i++)
                {
                    dgvbatch.Rows[i].DefaultCellStyle.BackColor = Color.White;
                }
            }


            catch (Exception ex)
            {

            }


        }
        public class DeedImageDetails
        {
            public string DistrictCode { get; set; }
            public string RoCode { get; set; }
            public string DeedNumber { get; set; }
            public string Book { get; set; }
            public string DeedYear { get; set; }
            public string DeedImage { get; set; }
        }

    }
}