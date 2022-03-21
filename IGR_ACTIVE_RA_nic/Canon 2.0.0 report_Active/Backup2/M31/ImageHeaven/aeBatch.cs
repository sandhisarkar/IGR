/*
 * Created by SharpDevelop.
 * User: SubhajitB
 * Date: 19/2/2008
 * Time: 4:24 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Data;
using System.Data.Odbc;
using NovaNet.wfe;
using NovaNet.Utils;
using LItems;
using System.IO;

namespace ImageHeaven
{
	/// <summary>
	/// Description of aeBatch.
	/// </summary>
    public partial class aeBatch : frmAddEdit, StateData
	{
		wfeBatch crtBatch=null;	
		OdbcConnection sqlCon=null;
        MemoryStream stateLog;
        byte[] tmpWrite;
        public static NovaNet.Utils.exLog.Logger exMailLog = new NovaNet.Utils.exLog.emailLogger("./errLog.log", NovaNet.Utils.exLog.LogLevel.Dev, Constants._MAIL_TO, Constants._MAIL_FROM, Constants._SMTP);
        public static NovaNet.Utils.exLog.Logger exTxtLog = new NovaNet.Utils.exLog.txtLogger("./errLog.log", NovaNet.Utils.exLog.LogLevel.Dev);

//		public aeBatch()
//		{
//			//
//			// The InitializeComponent() call is required for Windows Forms designer support.
//			//
//			InitializeComponent();
//			this.Text="B'Zer - Add Batch";
//			//
//			// TODO: Add constructor code after the InitializeComponent() call.
//			//
//		}
		public aeBatch(wItem prmCmd,OdbcConnection prmCon)
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
            //this.Icon = 
            exMailLog.SetNextLogger(exTxtLog);
            
			crtBatch = (wfeBatch) prmCmd;
            sqlCon = prmCon;
			if (crtBatch.GetMode()==Constants._ADDING)
				this.Text = "B'Zer - Add Batch";
			else
				this.Text = "B'Zer - Edit Batch";
			//
			// TODO: Add constructor code after the InitializeComponent() call.
			//
		}
        MemoryStream StateData.StateLog()
        {
            return stateLog;
        }
		void AeBatchResizeBegin(object sender, EventArgs e)
		{
			base.Width=this.Width;
			base.Height=this.Height;
		}
		void AeBatchLoad(object sender, System.EventArgs e)
		{
			DataSet DsGen = new DataSet();
			
			panelButton.Location = new System.Drawing.Point(470,146);
			NovaNet.Utils.dbCon dbcon=new NovaNet.Utils.dbCon();
			
			wfeProject crtProj=new wfeProject(sqlCon);
			
            cmbProject.Items.Clear();
            DsGen = crtProj.GetAllValues();
            if (DsGen.Tables[0].Rows.Count!=0)
            {
	            cmbProject.DataSource = DsGen.Tables[0];
	            cmbProject.DisplayMember = DsGen.Tables[0].Columns["proj_code"].ToString();
	            cmbProject.ValueMember = DsGen.Tables[0].Columns["proj_key"].ToString();
            }
            PopulateCombo();
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
                txtCode.Enabled = false;
                txtName.Enabled = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

		protected override void CmdSaveClick(object sender, EventArgs e)
		{
			NovaNet.Utils.dbCon dbcon=new NovaNet.Utils.dbCon();
            udtBatch objBatch = new udtBatch();
			try
			{
				
				crtBatch=new wfeBatch(sqlCon);
				
				objBatch.proj_code=Convert.ToInt32(cmbProject.SelectedValue);
				objBatch.batch_code=txtCode.Text;
				objBatch.batch_name=txtName.Text;
				objBatch.Created_DTTM=dbcon.GetCurrenctDTTM(1,sqlCon);
			
				if (crtBatch.TransferValues(objBatch)==true)
				{
					statusStrip1.Items.Add("Status: Data SucessFully Saved");
					statusStrip1.ForeColor=System.Drawing.Color.Black;
					ClearAllField();
				}
				else
				{
					statusStrip1.Items.Add("Status: Data Can not be Saved");
					statusStrip1.ForeColor=System.Drawing.Color.Red;
				}
			}
			catch(KeyCheckException ex)
			{
				MessageBox.Show(ex.Message,"B'Zer",MessageBoxButtons.OK,MessageBoxIcon.Error);
                stateLog = new MemoryStream();
                tmpWrite = new System.Text.ASCIIEncoding().GetBytes("Batch Key-" + objBatch.batch_key + "\n" + "project Key-" + objBatch.proj_code + "\n");
                stateLog.Write(tmpWrite, 0, tmpWrite.Length);
                exMailLog.Log(ex, this);
			}
			catch(DbCommitException dbex)
			{
				MessageBox.Show(dbex.Message,"B'Zer",MessageBoxButtons.OK,MessageBoxIcon.Error);
                stateLog = new MemoryStream();
                tmpWrite = new System.Text.ASCIIEncoding().GetBytes("Error while Commit" + "Batch Key-" + objBatch.batch_key + "\n" + "project Key-" + objBatch.proj_code + "\n");
                stateLog.Write(tmpWrite, 0, tmpWrite.Length);
                exMailLog.Log(dbex, this);
			}
			catch(CreateFolderException folex)
			{
				MessageBox.Show(folex.Message,"B'Zer",MessageBoxButtons.OK,MessageBoxIcon.Error);
                stateLog = new MemoryStream();
                tmpWrite = new System.Text.ASCIIEncoding().GetBytes("Error while Create Folder" + "Batch Key-" + objBatch.batch_key + "\n" + "project Key-" + objBatch.proj_code + "\n");
                stateLog.Write(tmpWrite, 0, tmpWrite.Length);
                exMailLog.Log(folex, this);
			}
			catch (DBConnectionException conex)
			{
				MessageBox.Show(conex.Message,"B'Zer",MessageBoxButtons.OK,MessageBoxIcon.Error);
                stateLog = new MemoryStream();
                tmpWrite = new System.Text.ASCIIEncoding().GetBytes("Error while Connection error" + "Batch Key-" + objBatch.batch_key + "\n" + "project Key-" + objBatch.proj_code + "\n");
                stateLog.Write(tmpWrite, 0, tmpWrite.Length);
                exMailLog.Log(conex, this);
			}
			catch(INIFileException iniex)
			{
				MessageBox.Show(iniex.Message,"B'Zer",MessageBoxButtons.OK,MessageBoxIcon.Error);
                stateLog = new MemoryStream();
                tmpWrite = new System.Text.ASCIIEncoding().GetBytes("Error while INI read error" + "Batch Key-" + objBatch.batch_key + "\n" + "project Key-" + objBatch.proj_code + "\n");
                stateLog.Write(tmpWrite, 0, tmpWrite.Length);
                exMailLog.Log(iniex, this);
			}
		}
		void TxtCodeKeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
		{
			if ((e.KeyChar==92) || (e.KeyChar==39) || (e.KeyChar==47))
			{
				e.Handled=true;
			}
		}
		void TxtNameKeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
		{
			if ((e.KeyChar==92) || (e.KeyChar==39) || (e.KeyChar==47))
			{
				e.Handled=true;
			}
		}
		private void ClearAllField()
		{
			txtName.Text=string.Empty;
			txtCode.Text=string.Empty;
		}

        private void button1_Click(object sender, EventArgs e)
        {
            txtCode.Text = cmbDistrict.SelectedValue.ToString() + cmbWhereReg.SelectedValue.ToString() + cmbBook.SelectedValue.ToString() + txtYear.Text + cmbVol.Text;
            txtName.Text = cmbDistrict.SelectedValue.ToString() + cmbWhereReg.SelectedValue.ToString() + cmbBook.SelectedValue.ToString() + txtYear.Text + cmbVol.Text;
        }
	}
}
