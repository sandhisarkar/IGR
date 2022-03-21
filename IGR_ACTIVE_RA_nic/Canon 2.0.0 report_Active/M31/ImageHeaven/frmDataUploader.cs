using System;
using System.Drawing;
using System.Windows.Forms;
using NovaNet.Utils;
using System.Data;
using System.Data.Odbc;
using System.Data.OleDb;
using LItems;
using NovaNet.wfe;
using System.IO;

namespace ImageHeaven
{
    public partial class frmDataUploader : Form
    {
        NovaNet.Utils.dbCon dbcon;
        //Credentials udtCrd;
        ControlInfo udtInfo;
        MemoryStream stateLog;
        byte[] tmpWrite;
        OdbcConnection sqlCon = null;
        Credentials crd = new Credentials();
        public static NovaNet.Utils.exLog.Logger exMailLog = new NovaNet.Utils.exLog.emailLogger("./errLog.log", NovaNet.Utils.exLog.LogLevel.Dev, Constants._MAIL_TO, Constants._MAIL_FROM, Constants._SMTP);
        public static NovaNet.Utils.exLog.Logger exTxtLog = new NovaNet.Utils.exLog.txtLogger("./errLog.log", NovaNet.Utils.exLog.LogLevel.Dev);
        public string fDoCode;
        public string fBoCode;
        DataSet dsdeed = null;
        DataSet index1 = null;
        DataSet index2 = null;
        DataSet plot = null;
        DataSet khatian = null;
        public frmDataUploader(OdbcConnection prmCon, Credentials prmCrd)
        {
            InitializeComponent();
            sqlCon = prmCon;
            crd = prmCrd;
        }

        private void cmdPath_Click(object sender, EventArgs e)
        {
            //try
            //{
            //    grdCsv.DataSource = null;
            //    dlgCSV.Filter = "MDB File|*.mdb";
            //    dlgCSV.FileName = string.Empty;
            //    dlgCSV.Title = "B'Zer - MDB Files";
            //    dlgCSV.ShowDialog();
            //    txtPath.Text = dlgCSV.FileName.ToString();
            //    string filename = Path.GetFileNameWithoutExtension(txtPath.Text);
            //    if (filename != cmbBatch.Text)
            //    {
            //        MessageBox.Show("Selected File is not for this Batch, Please choose correct file...");
            //        return;
            //    }
            //    //bool readFlag = ReadDatabase();
            //    if (readFlag == true)
            //    {
            //        bool insertFlag = InsertintoMysqlDb();
            //    }
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex.Message.ToString());
            //}
        }

        private DataSet ReadDatabase()
        {
            DataSet ds = new DataSet();
            dsdeed = new DataSet();
            try
            {
                
                string sql = "select * from deed_details where District_Code = '" + cmbDistrict.SelectedValue.ToString() + "' and RO_Code = '" + cmbWhereReg.SelectedValue.ToString() + "' and Book = '" + cmbBook.SelectedValue.ToString() + "' and Deed_year = '" + txtYear.Text + "' and volume_no = '" + cmbVol.Text + "' and proj_key is null and batch_key is null and box_key is null order by convert(deed_no,UNSIGNED INTEGER)";
                
                
                OdbcDataAdapter odap = new OdbcDataAdapter(sql, sqlCon);
                odap.Fill(ds);
                odap.Fill(dsdeed);
                
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message.ToString());
                statusStrip1.Text = ex.Message.ToString();
            }
            return ds;
        }
        private bool InsertintoMysqlDb()
        {
            OdbcTransaction sqlTrans = null;
            sqlTrans = sqlCon.BeginTransaction();
            try
            {
                string sqlStr = null;
                
                //for (int i = 0; i < dsdeed.Tables[0].Rows.Count;i++ )
                //{
                    OdbcCommand sqlCmd = new OdbcCommand();
                    //Change on 19/02/2014 due to the problem with date_of_completion and date_of_delivery
                    //sqlStr = "Insert into deed_details(District_Code,RO_Code,Book,Deed_year,Deed_no,Serial_No,Serial_Year,tran_maj_code,tran_min_code,Volume_No,Page_From,Page_To,Date_of_Completion,Date_of_Delivery,Deed_Remarks,Created_DTTM,scan_doc_type,addl_pages,hold,hold_reason,status,created_system,version,exported,created_by,modified_by,modified_dttm,mismatch,proj_key,batch_key,box_key) values ('" + dsdeed.Tables[0].Rows[i][0] + "','" + dsdeed.Tables[0].Rows[i][1] + "','" + dsdeed.Tables[0].Rows[i][2] + "','" + dsdeed.Tables[0].Rows[i][3] + "','" + dsdeed.Tables[0].Rows[i][4] + "','" + dsdeed.Tables[0].Rows[i][5] + "','" + dsdeed.Tables[0].Rows[i][6] + "','" + dsdeed.Tables[0].Rows[i][7] + "','" + dsdeed.Tables[0].Rows[i][8] + "'," + dsdeed.Tables[0].Rows[i][9] + "," + dsdeed.Tables[0].Rows[i][10] + "," + dsdeed.Tables[0].Rows[i][11] + ",'" + dsdeed.Tables[0].Rows[i][12] + "','" + dsdeed.Tables[0].Rows[i][13] + "','" + dsdeed.Tables[0].Rows[i][14] + "','" + dsdeed.Tables[0].Rows[i][15] + "','" + dsdeed.Tables[0].Rows[i][16] + "','" + dsdeed.Tables[0].Rows[i][17] + "','" + dsdeed.Tables[0].Rows[i][18] + "','" + dsdeed.Tables[0].Rows[i][19] + "','" + dsdeed.Tables[0].Rows[i][20] + "','" + dsdeed.Tables[0].Rows[i][21] + "','" + dsdeed.Tables[0].Rows[i][22] + "','" + dsdeed.Tables[0].Rows[i][23] + "','" + dsdeed.Tables[0].Rows[i][24] + "','" + dsdeed.Tables[0].Rows[i][25] + "','" + dsdeed.Tables[0].Rows[i][26] + "','" + dsdeed.Tables[0].Rows[i][27] + "','" + cmbProject.SelectedValue.ToString() + "','" + cmbBatch.SelectedValue.ToString() + "','1')";
                    sqlStr = "update deed_details set proj_key = '" + cmbProject.SelectedValue.ToString() + "',batch_key = '" + cmbBatch.SelectedValue.ToString() + "',box_key = '1',Exported = 'Y' where District_Code = '" + cmbDistrict.SelectedValue.ToString() + "' and RO_Code = '" + cmbWhereReg.SelectedValue.ToString() + "' and Book = '" + cmbBook.SelectedValue.ToString() + "' and Deed_year = '" + txtYear.Text + "' and volume_no = '" + cmbVol.Text + "'";
                    sqlCmd.Connection = sqlCon;
                    sqlCmd.Transaction = sqlTrans;
                    sqlCmd.CommandText = sqlStr;
                    sqlCmd.ExecuteNonQuery();
                //}

               
                SaveIntoBzer(sqlTrans,dsdeed);
                sqlTrans.Commit();
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
                sqlTrans.Rollback();
                return false;
            }
        }
        private bool SaveIntoBzer(OdbcTransaction trans,DataSet deed)
        {
            try
            {
                if ((cmbProject.Text != string.Empty) && (cmbBatch.Text != string.Empty))
                {
                    NovaNet.Utils.dbCon dbcon = new NovaNet.Utils.dbCon();


                    wItemCreator boxCreate = new BoxCreator(sqlCon);

                    udtInfo = new ControlInfo();

                    udtInfo.batch_Key = Convert.ToInt32(cmbBatch.SelectedValue.ToString());
                    udtInfo.proj_Key = Convert.ToInt32(cmbProject.SelectedValue.ToString());

                    statusStrip1.Items.Add("Status: Wait While Uploading the Database......");

                    int policyStatus = (int)eSTATES.POLICY_CREATED;
                    int boxStatus = (int)eSTATES.BOX_CREATED;
                    if (boxCreate.CreateBox(crd, udtInfo, boxStatus, policyStatus,dsdeed,trans) == true)
                    {
                        statusStrip1.Items.Clear();
                        statusStrip1.Items.Add("Status: Database SucessFully Uploaded");
                    }
                    else
                    {
                        statusStrip1.Items.Clear();
                        statusStrip1.Items.Add("Status: Uploading Cannot be Completed");
                    }
                    exTxtLog.Log("Test From B'Zer");
                    exTxtLog.Log("Test From B'Zer");
                }
            }
            catch (Exception ex)
            {
                statusStrip1.Items.Clear();
                MessageBox.Show(ex.Message, "B'Zer", MessageBoxButtons.OK, MessageBoxIcon.Error);
                stateLog = new MemoryStream();
                tmpWrite = new System.Text.ASCIIEncoding().GetBytes("Error while Commit");
                stateLog.Write(tmpWrite, 0, tmpWrite.Length);

            }

            return true;
        }
        private void frmDataUploader_Load(object sender, EventArgs e)
        {
            PopulateCombo();
            PopulateProjectCombo();
            PopulateBatchCombo();
        }
        private void PopulateCombo()
        {

            try
            {
                wfeProject tmpProj = new wfeProject(sqlCon);
                //For district.......................................
                if (tmpProj.GetDistrict_Active().Tables[0].Rows.Count > 0)
                {
                    cmbDistrict.DataSource = tmpProj.GetDistrict_Active().Tables[0];
                    cmbDistrict.DisplayMember = "district_name";
                    cmbDistrict.ValueMember = "district_code";
                    cmbDistrict.SelectedIndex = 0;
                }
                if (tmpProj.GetYear().Tables[0].Rows.Count > 0)
                {
                    txtYear.DataSource = tmpProj.GetYear().Tables[0];
                    txtYear.DisplayMember = "Year";
                    txtYear.ValueMember = "Deed_Year";
                    txtYear.SelectedIndex = 0;
                }

                if (tmpProj.GetBookType().Tables[0].Rows.Count > 0)
                {
                    cmbBook.DataSource = tmpProj.GetBookType().Tables[0];
                    cmbBook.DisplayMember = "key_book";
                    cmbBook.ValueMember = "value_book";
                    cmbBook.SelectedIndex = 0;
                }


                if (cmbDistrict.DataSource != null)
                {
                    string districtCode = cmbDistrict.SelectedValue.ToString();
                    if (tmpProj.GetROffice_Active(districtCode).Tables[0].Rows.Count > 0)
                    {
                        cmbWhereReg.DataSource = tmpProj.GetROffice_Active(districtCode).Tables[0];
                        cmbWhereReg.DisplayMember = "RO_name";
                        cmbWhereReg.ValueMember = "RO_code";
                    }
                }

                // cmbBook.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        private void PopulateProjectCombo()
        {
            DataSet ds = new DataSet();

            dbcon = new NovaNet.Utils.dbCon();

            wfeProject tmpProj = new wfeProject(sqlCon);
            //cmbProject.Items.Add("Select");
            ds = tmpProj.GetAllValues();
            if (ds.Tables[0].Rows.Count > 0)
            {
                cmbProject.DataSource = ds.Tables[0];
                cmbProject.DisplayMember = ds.Tables[0].Columns[1].ToString();
                cmbProject.ValueMember = ds.Tables[0].Columns[0].ToString();
            }
        }
        private void PopulateBatchCombo()
        {
            string projKey = null;
            DataSet ds = new DataSet();

            dbcon = new NovaNet.Utils.dbCon();

            wfeBatch tmpBatch = new wfeBatch(sqlCon);
            if (cmbProject.SelectedValue != null)
            {
                projKey = cmbProject.SelectedValue.ToString();
                ds = tmpBatch.GetAllValues(Convert.ToInt32(projKey));
                cmbBatch.DataSource = ds.Tables[0];
                cmbBatch.DisplayMember = ds.Tables[0].Columns[1].ToString();
                cmbBatch.ValueMember = ds.Tables[0].Columns[0].ToString();
            }	//cmbBatch.Items.Insert(0,"Select");
        }

        private void cmbProject_Leave(object sender, EventArgs e)
        {
            PopulateBatchCombo();
        }

        private void cmdUpload_Click(object sender, EventArgs e)
        {

        }

        private void txtYear_Leave(object sender, EventArgs e)
        {
            wfeProject tmpProj = new wfeProject(sqlCon);
            if (cmbDistrict.Text != "" && cmbWhereReg.Text != "" && cmbBook.SelectedValue.ToString() != "" && txtYear.Text != "")
            {
                if (tmpProj.GetVol(cmbDistrict.SelectedValue.ToString(), cmbWhereReg.SelectedValue.ToString(), cmbBook.SelectedValue.ToString(), txtYear.Text, true).Tables[0].Rows.Count > 0)
                {
                    cmbVol.DataSource = tmpProj.GetVol(cmbDistrict.SelectedValue.ToString(), cmbWhereReg.SelectedValue.ToString(), cmbBook.SelectedValue.ToString(), txtYear.Text, true).Tables[0];
                    cmbVol.DisplayMember = "Volume";
                    cmbVol.ValueMember = "Volume_No";
                    cmbVol.SelectedIndex = 0;
                }
            }
        }

        private void cmdsearch_Click(object sender, EventArgs e)
        {
            grdCsv.DataSource = null;
            grdCsv.DataSource = ReadDatabase().Tables[0];
            if (grdCsv.Rows.Count > 0)
            {
                cmdExport.Enabled = true;
            }
            else
            {
                cmdExport.Enabled = false;
            }
        }

        private void cmdExport_Click(object sender, EventArgs e)
        {
            try
            {
                if ((cmbProject.Text == string.Empty) && (cmbBatch.Text == string.Empty))
                {
                    MessageBox.Show(this, "Please Select Project and Batch...","",MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                InsertintoMysqlDb();
                cmdExport.Enabled = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        private void cmbVol_Leave(object sender, EventArgs e)
        {
            string batch_code = string.Empty;
            try
            {
                if (cmbVol.Text != "")
                {
                    batch_code = cmbDistrict.SelectedValue.ToString() + cmbWhereReg.SelectedValue.ToString() + cmbBook.SelectedValue.ToString() + txtYear.Text + cmbVol.Text;
                    cmbBatch.Text = batch_code;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "Please create Batch before Uploading, Batch Not Found", "No Batch", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
