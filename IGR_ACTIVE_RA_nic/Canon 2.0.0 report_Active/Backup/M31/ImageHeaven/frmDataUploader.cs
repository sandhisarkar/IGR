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
            try
            {
                grdCsv.DataSource = null;
                dlgCSV.Filter = "MDB File|*.mdb";
                dlgCSV.FileName = string.Empty;
                dlgCSV.Title = "B'Zer - MDB Files";
                dlgCSV.ShowDialog();
                txtPath.Text = dlgCSV.FileName.ToString();
                string filename = Path.GetFileNameWithoutExtension(txtPath.Text);
                if (filename != cmbBatch.Text)
                {
                    MessageBox.Show("Selected File is not for this Batch, Please choose correct file...");
                    return;
                }
                bool readFlag = ReadDatabase();
                if (readFlag == true)
                {
                    bool insertFlag = InsertintoMysqlDb();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        private bool ReadDatabase()
        {
            try
            {
                grdCsv.DataSource = null;
                dsdeed = new DataSet();
                index1 = new DataSet();
                index2 = new DataSet();
                plot = new DataSet();
                khatian = new DataSet();
                System.Data.OleDb.OleDbConnection conn1 = new System.Data.OleDb.OleDbConnection();
                conn1.ConnectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;" +
                                        @"Data source= " + txtPath.Text;
                string sql = "Select * from deed_details ";
                string sql1 = "select * from index_of_name";
                string sql2 = "select * from Index_of_Property";
                string sqlPlot = "select * from tbl_other_plots";
                string sqlKhatian = "select * from tbl_other_khatian";
                OleDbDataAdapter odap = new OleDbDataAdapter(sql, conn1);
                odap.Fill(dsdeed);
                OleDbDataAdapter odap1 = new OleDbDataAdapter(sql1, conn1);
                odap1.Fill(index1);
                OleDbDataAdapter odap2 = new OleDbDataAdapter(sql2, conn1);
                odap2.Fill(index2);
                OleDbDataAdapter odap3 = new OleDbDataAdapter(sqlPlot, conn1);
                odap3.Fill(plot);
                OleDbDataAdapter odap4 = new OleDbDataAdapter(sqlKhatian, conn1);
                odap4.Fill(khatian);
                grdCsv.DataSource = dsdeed.Tables[0];
                return true;
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message.ToString());
                statusStrip1.Text = ex.Message.ToString();
                return false;
            }
        }
        private bool InsertintoMysqlDb()
        {
            OdbcTransaction sqlTrans = null;
            sqlTrans = sqlCon.BeginTransaction();
            try
            {
                string sqlStr = null;
                
                for (int i = 0; i < dsdeed.Tables[0].Rows.Count;i++ )
                {
                    OdbcCommand sqlCmd = new OdbcCommand();
                    //Change on 19/02/2014 due to the problem with date_of_completion and date_of_delivery
                    sqlStr = "Insert into deed_details(District_Code,RO_Code,Book,Deed_year,Deed_no,Serial_No,Serial_Year,tran_maj_code,tran_min_code,Volume_No,Page_From,Page_To,Date_of_Completion,Date_of_Delivery,Deed_Remarks,Created_DTTM,scan_doc_type,addl_pages,hold,hold_reason,status,created_system,version,exported,created_by,modified_by,modified_dttm,mismatch,proj_key,batch_key,box_key) values ('" + dsdeed.Tables[0].Rows[i][0] + "','" + dsdeed.Tables[0].Rows[i][1] + "','" + dsdeed.Tables[0].Rows[i][2] + "','" + dsdeed.Tables[0].Rows[i][3] + "','" + dsdeed.Tables[0].Rows[i][4] + "','" + dsdeed.Tables[0].Rows[i][5] + "','" + dsdeed.Tables[0].Rows[i][6] + "','" + dsdeed.Tables[0].Rows[i][7] + "','" + dsdeed.Tables[0].Rows[i][8] + "'," + dsdeed.Tables[0].Rows[i][9] + "," + dsdeed.Tables[0].Rows[i][10] + "," + dsdeed.Tables[0].Rows[i][11] + ",'" + dsdeed.Tables[0].Rows[i][12] + "','" + dsdeed.Tables[0].Rows[i][13] + "','" + dsdeed.Tables[0].Rows[i][14] + "','" + dsdeed.Tables[0].Rows[i][15] + "','" + dsdeed.Tables[0].Rows[i][16] + "','" + dsdeed.Tables[0].Rows[i][17] + "','" + dsdeed.Tables[0].Rows[i][18] + "','" + dsdeed.Tables[0].Rows[i][19] + "','" + dsdeed.Tables[0].Rows[i][20] + "','" + dsdeed.Tables[0].Rows[i][21] + "','" + dsdeed.Tables[0].Rows[i][22] + "','" + dsdeed.Tables[0].Rows[i][23] + "','" + dsdeed.Tables[0].Rows[i][24] + "','" + dsdeed.Tables[0].Rows[i][25] + "','" + dsdeed.Tables[0].Rows[i][26] + "','" + dsdeed.Tables[0].Rows[i][27] + "','" + cmbProject.SelectedValue.ToString() + "','" + cmbBatch.SelectedValue.ToString() + "','1')";
                    sqlCmd.Connection = sqlCon;
                    sqlCmd.Transaction = sqlTrans;
                    sqlCmd.CommandText = sqlStr;
                    sqlCmd.ExecuteNonQuery();
                }

                for (int a = 0; a < index1.Tables[0].Rows.Count; a++)
                {
                    OdbcCommand sqlCmd2 = new OdbcCommand();
                    sqlStr = "insert into Index_of_name(District_Code,RO_Code,Book,Deed_year,Deed_no,Item_no,initial_name,First_name,Last_name,Party_code,Admit_code,Address,Address_district_code,Address_district_name,address_ps_code,Address_ps_name,Father_mother,Rel_code,Relation,occupation_code,religion_code,more,pin,city,other_party_code,linked_to,Created_by) values('" + index1.Tables[0].Rows[a][0] + "','" + index1.Tables[0].Rows[a][1] + "','" + index1.Tables[0].Rows[a][2] + "','" + index1.Tables[0].Rows[a][3] + "','" + index1.Tables[0].Rows[a][4] + "','" + index1.Tables[0].Rows[a][5] + "','" + index1.Tables[0].Rows[a][6] + "','" + index1.Tables[0].Rows[a][7] + "','" + index1.Tables[0].Rows[a][8] + "','" + index1.Tables[0].Rows[a][9] + "','" + index1.Tables[0].Rows[a][10] + "','" + index1.Tables[0].Rows[a][11] + "','" + index1.Tables[0].Rows[a][12] + "','" + index1.Tables[0].Rows[a][13] + "','" + index1.Tables[0].Rows[a][14] + "','" + index1.Tables[0].Rows[a][15] + "','" + index1.Tables[0].Rows[a][16] + "','" + index1.Tables[0].Rows[a][17] + "','" + index1.Tables[0].Rows[a][18] + "','" + index1.Tables[0].Rows[a][19] + "','" + index1.Tables[0].Rows[a][20] + "','" + index1.Tables[0].Rows[a][22] + "','" + index1.Tables[0].Rows[a][23] + "','" + index1.Tables[0].Rows[a][24] + "','" + index1.Tables[0].Rows[a][25] + "','" + index1.Tables[0].Rows[a][26] + "','" + index1.Tables[0].Rows[a][27] + "')";
                    sqlCmd2.Connection = sqlCon;
                    sqlCmd2.Transaction = sqlTrans;
                    sqlCmd2.CommandText = sqlStr;
                    sqlCmd2.ExecuteNonQuery();
                }

                for (int x = 0; x < index2.Tables[0].Rows.Count; x++)
                {
                    OdbcCommand sqlCmd1 = new OdbcCommand();
                    sqlStr = "insert into Index_of_Property(District_Code,RO_Code,Book,Deed_year,Deed_no,Item_no,Property_district_code,Property_ro_code,ps_code,moucode,Area_type,GP_Muni_Corp_Code,Ward,Holding,Premises,road_code,Plot_code_type,Road,plot_no,Bata_no,Khatian_type,Khatian_no,bata_khatian_no,property_type,Land_Area_acre,Land_Area_bigha,Land_Area_decimal,Land_Area_katha,Land_Area_chatak,Land_Area_sqfeet,Structure_area_in_sqFeet,Created_DTTM,ref_ps,ref_mouza,jl_no,other_plots,other_khatian,land_type,refjl_no,Created_by) values('" + index2.Tables[0].Rows[x][0] + "','" + index2.Tables[0].Rows[x][1] + "','" + index2.Tables[0].Rows[x][2] + "','" + index2.Tables[0].Rows[x][3] + "','" + index2.Tables[0].Rows[x][4] + "','" + index2.Tables[0].Rows[x][5] + "','" + index2.Tables[0].Rows[x][6] + "','" + index2.Tables[0].Rows[x][7] + "','" + index2.Tables[0].Rows[x][8] + "','" + index2.Tables[0].Rows[x][9] + "','" + index2.Tables[0].Rows[x][10] + "','" + index2.Tables[0].Rows[x][11] + "','" + index2.Tables[0].Rows[x][12] + "','" + index2.Tables[0].Rows[x][13] + "','" + index2.Tables[0].Rows[x][14] + "','" + index2.Tables[0].Rows[x][15] + "','" + index2.Tables[0].Rows[x][16] + "','" + index2.Tables[0].Rows[x][17] + "','" + index2.Tables[0].Rows[x][18] + "','" + index2.Tables[0].Rows[x][19] + "','" + index2.Tables[0].Rows[x][20] + "','" + index2.Tables[0].Rows[x][21] + "','" + index2.Tables[0].Rows[x][22] + "','" + index2.Tables[0].Rows[x][23] + "','" + index2.Tables[0].Rows[x][24] + "','" + index2.Tables[0].Rows[x][25] + "','" + index2.Tables[0].Rows[x][26] + "','" + index2.Tables[0].Rows[x][27] + "','" + index2.Tables[0].Rows[x][28] + "','" + index2.Tables[0].Rows[x][29] + "','" + index2.Tables[0].Rows[x][30] + "','" + index2.Tables[0].Rows[x][31] + "','" + index2.Tables[0].Rows[x][32] + "','" + index2.Tables[0].Rows[x][33] + "','" + index2.Tables[0].Rows[x][34] + "','" + index2.Tables[0].Rows[x][35] + "','" + index2.Tables[0].Rows[x][36] + "','" + index2.Tables[0].Rows[x][37] + "','" + index2.Tables[0].Rows[x][38] + "','" + index2.Tables[0].Rows[x][39] + "')";
                    sqlCmd1.Connection = sqlCon;
                    sqlCmd1.Transaction = sqlTrans;
                    sqlCmd1.CommandText = sqlStr;
                    sqlCmd1.ExecuteNonQuery();
                }

                for (int x = 0; x < plot.Tables[0].Rows.Count; x++)
                {
                    OdbcCommand sqlCmd1 = new OdbcCommand();
                    sqlStr = "insert into tblother_plots(District_Code,RO_Code,Book,Deed_year,Deed_no,Item_no,other_plots) values('" + plot.Tables[0].Rows[x][0] + "','" + plot.Tables[0].Rows[x][1] + "','" + plot.Tables[0].Rows[x][2] + "','" + plot.Tables[0].Rows[x][3] + "','" + plot.Tables[0].Rows[x][4] + "','" + plot.Tables[0].Rows[x][5] + "','" + plot.Tables[0].Rows[x][6] + "')";
                    sqlCmd1.Connection = sqlCon;
                    sqlCmd1.Transaction = sqlTrans;
                    sqlCmd1.CommandText = sqlStr;
                    sqlCmd1.ExecuteNonQuery();
                }

                for (int x = 0; x < khatian.Tables[0].Rows.Count; x++)
                {
                    OdbcCommand sqlCmd1 = new OdbcCommand();
                    sqlStr = "insert into tbl_other_khatian(District_Code,RO_Code,Book,Deed_year,Deed_no,Item_no,other_khatian) values('" + khatian.Tables[0].Rows[x][0] + "','" + khatian.Tables[0].Rows[x][1] + "','" + khatian.Tables[0].Rows[x][2] + "','" + khatian.Tables[0].Rows[x][3] + "','" + khatian.Tables[0].Rows[x][4] + "','" + khatian.Tables[0].Rows[x][5] + "','" + khatian.Tables[0].Rows[x][6] + "')";
                    sqlCmd1.Connection = sqlCon;
                    sqlCmd1.Transaction = sqlTrans;
                    sqlCmd1.CommandText = sqlStr;
                    sqlCmd1.ExecuteNonQuery();
                }

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
            PopulateProjectCombo();
            PopulateBatchCombo();
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
    }
}
