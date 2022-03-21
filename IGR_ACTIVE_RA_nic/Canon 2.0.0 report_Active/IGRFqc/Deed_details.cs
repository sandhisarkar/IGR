#define BzerUse
using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DataLayerDefs;
using nControls;
using System.Data.Odbc;
using NovaNet.Utils;
using NvUtils;
using igr_base;
using valUtils;

namespace IGRFqc
{
	public partial class Deed_details : Form
	{
		
		private PopulateCombo pCom;
		Credentials crd = new Credentials();
		private OdbcConnection sqlCon = null;
		OdbcTransaction txn;
		DeedDetails _deed = null;
        igr_deed m_deed;
        //deedDetailsException _Dexpc;
        List<deedDetailsException> _Dexpc = null;

		//Configuration cfg = new ImageConfig(

		//igr_deed _mdeed;
		string mismatchPage = "N";
		Mode _isEditing;
		//The method to be invoked when the user has accepted the changes
        public delegate void OnAccept(DeedDetails retDeed);
		OnAccept m_OnAccept;
		//The method to be invoked when the user aborts all operations
		public delegate void OnAbort();
		OnAbort m_OnAbort;
		string deed_year = string.Empty;
		string deed_vol = string.Empty;
		string exception = string.Empty;
		string uuid = string.Empty;



        string iniPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase).Remove(0, 6) + "\\" + "IhConfiguration.ini";
        INIFile ini = new INIFile();
        bool isOutsideWB = false;

        public Deed_details(OdbcConnection prmCon, OdbcTransaction pTxn, Credentials prmCrd, DeedDetails deed, Mode p_isEditing, OnAccept AcceptCallBack, OnAbort AbortCallBack, List<deedDetailsException> mexp,igr_deed mdeed)
		{
			InitializeComponent();
            find_OWP_or_Not();
			sqlCon = prmCon;
			txn = pTxn;
			crd = prmCrd;
            m_deed = mdeed;
			pCom = new PopulateCombo(sqlCon,txn, crd);
			_isEditing = p_isEditing;
			deed_init(deed);
			_deed = deed;
            _Dexpc = mexp;
			// _expc = expc;
			m_OnAccept = AcceptCallBack;
			m_OnAbort = AbortCallBack;

		}
        public Deed_details(OdbcConnection prmCon, OdbcTransaction pTxn, Credentials prmCrd, DeedDetails deed, OnAccept AcceptCallBack, OnAbort AbortCallBack, List<deedDetailsException> mexp)
		{
			InitializeComponent();

            find_OWP_or_Not();
			sqlCon = prmCon;
			txn = pTxn;
			crd = prmCrd;
			//_mdeed = new igr_deed(sqlCon, crd);
			pCom = new PopulateCombo(sqlCon,txn, crd);
			_isEditing = Mode._Add;
            _Dexpc = mexp;
			deed_init(deed);
			_deed = deed;
            
			m_OnAccept = AcceptCallBack;
			m_OnAbort = AbortCallBack;
		}
        public Deed_details(OdbcConnection prmCon, OdbcTransaction pTxn, Credentials prmCrd, DeedDetails deed, OnAccept AcceptCallBack, OnAbort AbortCallBack, string deeedyr, string vol, List<deedDetailsException> mexp)
		{
			InitializeComponent();
            find_OWP_or_Not();
			sqlCon = prmCon;
			txn = pTxn;
			crd = prmCrd;
			//_mdeed = new igr_deed(sqlCon, crd);
			pCom = new PopulateCombo(sqlCon,txn,crd);
			_isEditing = Mode._Add;
            _Dexpc = mexp;
			deed_init(deed);
			deed_year = deeedyr;
			deed_vol = vol;
			_deed = deed;
			m_OnAccept = AcceptCallBack;
			m_OnAbort = AbortCallBack;
		}
        private void find_OWP_or_Not()
        {
            if (File.Exists(iniPath) == true)
            {
                string owp = ini.ReadINI("OWP", "OWP", string.Empty, iniPath);
                if (owp.ToString().Trim().ToLower().Contains("true"))
                {
                    isOutsideWB = true;
                }
            }
        }
		private void deed_init(DeedDetails  pDeed)
		{
			//txn = sqlCon.BeginTransaction();
			PopulateAllCombo();
            if (frmVolume._isOutsideWB == true)
            { chkOutsideWB.Checked = true; }
            else { chkOutsideWB.Checked = false; }
            
			if (_isEditing == Mode._Edit)
			{
				grpDeedControl.Enabled = false;
				cmbDistrict.Enabled = false;
				cmbWhereReg.Enabled = false;
				cmbBook.Enabled = false;
				txtDeedYear.Enabled = false;

				if (cmbDistrict.DataSource != null)
				{
					//cmbDistrict.SelectedValue = pDeed.Deed_control.District_code;
					string districtCode = pDeed.Deed_control.District_code;
					//string districtCode = cmbDistrict.SelectedValue.ToString();
					cmbWhereReg.DataSource = pCom.GetROffice(districtCode).Tables[0];
					cmbWhereReg.DisplayMember = "RO_name";
					cmbWhereReg.ValueMember = "RO_code";
					cmbWhereReg.SelectedValue = pDeed.Deed_control.RO_code;

				}
				cmbBook.SelectedValue = pDeed.Deed_control.Book;
				DataTable dt = pCom.GetKeyBook(pDeed.Deed_control.Book);
				cmbBook.Text = dt.Rows[0][0].ToString().Trim();
				if (cmbBook.Text != string.Empty)
				{
					cmbTransactionType.DataSource = pCom.GetTransactionTypeMajor(cmbBook.Text).Tables[0];
					cmbTransactionType.DisplayMember = "tran_maj_name";
					cmbTransactionType.ValueMember = "tran_maj_code";
				}
				cmbTransactionType.SelectedValue = pDeed.tran_maj_code;
				if (cmbTransactionType.DataSource != null)
				{
					cmbTransMinor.DataSource = pCom.GetTransactionTypeMinor(cmbTransactionType.SelectedValue.ToString()).Tables[0];
					cmbTransMinor.DisplayMember = "tran_name";
					cmbTransMinor.ValueMember = "tran_min_code";
					cmbTransMinor.SelectedValue = pDeed.tran_min_code;
				}
				cmbdocType.DataSource = pCom.GetDocType().Tables[0];
				cmbdocType.DisplayMember = "doc_name";
				cmbdocType.ValueMember = "doc_type";
				cmbdocType.SelectedValue = pDeed.scan_doc_type;
				txtDeedYear.Text = pDeed.Deed_control.Deed_year;
				txtVolumeNumber.Text = pDeed.volume_no;
				txtDeedNum.Text = pDeed.Deed_control.Deed_no;
				txtpgFrom.Text = pDeed.page_from;
				txtpageto.Text = pDeed.page_to;
				txtaditionalPages.Text = pDeed.addl_pages;
				//dtpComplition.Text = pDeed.date_of_completion;
				//dtpdelivery.Text = pDeed.date_of_delivery;
				if (pDeed.date_of_completion != null && pDeed.date_of_completion != "")
				{
					DateTime dateCompletion = Convert.ToDateTime((pDeed.date_of_completion));
					string dtCompletion = dateCompletion.ToString("ddMMyyyy");
					dtpComplition.Text = dtCompletion;
				}
				else
				{
					dtpComplition.Text = null;
				}
				if (pDeed.date_of_delivery != null && pDeed.date_of_delivery != "")
				{
					DateTime dateDelivery = Convert.ToDateTime((pDeed.date_of_delivery));
					string dtDelivery = dateDelivery.ToString("ddMMyyyy");
					dtpdelivery.Text = dtDelivery;
				}
				else
				{
					dtpdelivery.Text = null;
				}
				txtDeedRemarks.Text = pDeed.deed_remarks;
                if (pDeed.hold == "Y")
                {
                    Chkhold.Checked = true;
                    txtHoldRemarks.Visible = true;
                    txtHoldRemarks.Text = pDeed.hold_reason;
                }
                else
                {
                    Chkhold.Checked = false;
                    txtHoldRemarks.Visible = false;
                    txtHoldRemarks.Text = pDeed.hold_reason;
                }
				cmbTransactionType.Focus();
			}
			else
			{
				grpDeedControl.Enabled = true;
				cmbDistrict.Focus();
			}
		}


		private void PopulateAllCombo()
		{
			if (pCom.GetDistrict_Active().Tables[0].Rows.Count > 0)
			{
				cmbDistrict.DataSource = pCom.GetDistrict_Active().Tables[0];
				cmbDistrict.DisplayMember = "district_name";
				cmbDistrict.ValueMember = "district_code";
				cmbDistrict.SelectedIndex = 0;
			}
			if (cmbDistrict.DataSource != null)
			{
				string districtCode = cmbDistrict.SelectedValue.ToString();
				cmbWhereReg.DataSource = pCom.GetROffice_Active(districtCode).Tables[0];
				cmbWhereReg.DisplayMember = "RO_name";
				cmbWhereReg.ValueMember = "RO_code";
				cmbWhereReg.SelectedIndex = 0;
			}
			DataSet dsBook = pCom.GetBookType();
			if (dsBook.Tables[0].Rows.Count > 0)
			{
				cmbBook.DataSource = pCom.GetBookType().Tables[0];
				cmbBook.DisplayMember = "key_book";
				cmbBook.ValueMember = "value_book";
				cmbBook.SelectedIndex = 0;
			}
			
			cmbdocType.DataSource = pCom.GetDocType().Tables[0];
			cmbdocType.DisplayMember = "doc_name";
			cmbdocType.ValueMember = "doc_type";
			cmbdocType.SelectedIndex = 0;
			if (frmVolume.deed_year != null && frmVolume.deed_year != "")
			{
				txtDeedYear.Text = frmVolume.deed_year;
			}
            if (frmVolume.deed_book != null && frmVolume.deed_book != "")
            {
                cmbBook.Text = frmVolume.deed_book;
            }
			if (frmVolume.deed_vol != null && frmVolume.deed_vol != "")
			{
				txtVolumeNumber.Text = frmVolume.deed_vol;
			}
            if (cmbBook.SelectedValue.ToString() == "3" || cmbBook.SelectedValue.ToString() == "4")
            {
                frmVolume._isProperty = false;
                chkOutsideWB.Visible = false;
            }
            else
            {
                frmVolume._isProperty = true;
                chkOutsideWB.Visible = true;
            }
		}
		private Boolean validateFields()
		{
			bool flag = true;
			string msg = string.Empty;
			msg = DeedValidation.ChkBook(cmbBook.SelectedValue.ToString());
			if (msg.Trim().Length > 0)
			{
				MessageBox.Show(this, msg, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
				cmbBook.Focus();
				flag = false;
			}
			msg = DeedValidation.ChkDeedYear(txtDeedYear.Text);
			if (msg.Trim().Length > 0)
			{
				MessageBox.Show(this, msg, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
				txtDeedYear.Focus();
				flag = false;
			}
			msg = DeedValidation.ChkVolumeNo(txtVolumeNumber.Text);
			if (msg.Trim().Length > 0)
			{
				MessageBox.Show(this, msg, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
				txtVolumeNumber.Focus();
				flag = false;
			}
            //msg = DeedValidation.ChkDeedNo(txtDeedNum.Text);
            //if (msg.Trim().Length > 0)
            //{
            //    MessageBox.Show(this, msg, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    txtDeedNum.Focus();
            //    flag = false;
            //}
			msg = DeedValidation.ChkTranMajCode(cmbTransactionType.SelectedValue.ToString());
			if (msg.Trim().Length > 0)
			{
				MessageBox.Show(this, msg, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
				cmbTransactionType.Focus();
				flag = false;
			}
			msg = DeedValidation.ChkTranMinCode(cmbTransMinor.SelectedValue.ToString());
			if (msg.Trim().Length > 0)
			{
				MessageBox.Show(this, msg, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
				cmbTransMinor.Focus();
				flag = false;
			}
			msg = DeedValidation.ChkPageFrom(txtpgFrom.Text);
			if (msg.Trim().Length > 0)
			{
				MessageBox.Show(this, msg, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
				txtpgFrom.Focus();
				flag = false;
			}
			msg = DeedValidation.ChkPageTo(txtpageto.Text);
			if (msg.Trim().Length > 0)
			{
				MessageBox.Show(this, msg, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
				txtpageto.Focus();
				flag = false;
			}
            msg = DeedValidation.IsNumeric(txtaditionalPages.Text);
            if (msg.Trim().Length > 0)
            {
                MessageBox.Show(this, msg, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtaditionalPages.Focus();
                flag = false;
            }
			msg = DeedValidation.ChkDeedRemarks(txtDeedRemarks.Text);
			if (msg.Trim().Length > 0)
			{
				MessageBox.Show(this, msg, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
				txtDeedRemarks.Focus();
				flag = false;
			}
            if (txtDeedNum.Text =="00000")
            {
                MessageBox.Show(this, "Invalid Deed No...", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtDeedNum.Focus();
                flag = false;
            }
            if (Convert.ToInt32(txtpgFrom.Text) >= Convert.ToInt32(txtpageto.Text))
            {
                MessageBox.Show(this, "Page No error", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);     
                txtpgFrom.Select();
                flag = false;
            }
			return flag;
		}
		
		private void cmdSaveDeed_Click(object sender, EventArgs e)
		{   
            
            _Dexpc.Clear();
            if (chkOutsideWB.Checked == true)
            { frmVolume._isOutsideWB = true; }
            else { frmVolume._isOutsideWB = false; }
            if (cmbBook.SelectedValue.ToString() == "3" || cmbBook.SelectedValue.ToString() == "4")
            {
                frmVolume._isProperty = false;
            }
            else
            {
                frmVolume._isProperty = true;
            }
            if (_isEditing == Mode._Edit)
            {
                if (chkOutsideWB.Checked == false && m_deed._GetDeed.PropertiesoutWB.Count >= 1)
                {
                    DialogResult dialogResult = MessageBox.Show("Deed contains outside WB deed details, Do you want to delete property information ???", "Morut Data Entry...", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (dialogResult == DialogResult.Yes)
                    {
                        DialogResult dialogResult1 = MessageBox.Show("Process cann't be rolled back,Are You Sure ??", "Morut Data Entry...", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                        if (dialogResult1 == DialogResult.Yes)
                        {
                            m_deed._GetDeed.PropertiesoutWB.Clear();
                            m_deed._GetDeed.D_Excp.Clear();
                            frmVolume._isOutsideWB = false;
                        }
                        else
                        {

                            frmVolume._isOutsideWB = true; 
                        }

                    }
                    else
                    {

                    }
                    
                }
                if (chkOutsideWB.Checked == true && m_deed._GetDeed.Properties.Count >= 1)
                {
                    DialogResult dialogResult = MessageBox.Show("Deed contains property details, Do you want to delete property information ???", "Morut Data Entry...", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (dialogResult == DialogResult.Yes)
                    {
                        DialogResult dialogResult1 = MessageBox.Show("Process cann't be rolled back,Are You Sure ??", "Morut Data Entry...", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                        if (dialogResult1 == DialogResult.Yes)
                        {
                            m_deed._GetDeed.Properties.Clear();
                            m_deed._GetDeed.Pro_Excp.Clear();
                            frmVolume._isOutsideWB = true;
                        }
                        else
                        {

                            frmVolume._isOutsideWB = false;
                        }

                    }
                    else
                    {

                    }
                }
                if (cmbBook.SelectedValue.ToString() == "3" || cmbBook.SelectedValue.ToString() == "4")
                {
                    frmVolume._isProperty = false;
                }
                else
                {
                    frmVolume._isProperty = true;
                }
                
            }
            //uuid = pCom.GetUUID();
			if (validateFields())
			{
                

				frmVolume.deed_vol = txtVolumeNumber.Text;
				frmVolume.deed_year = txtDeedYear.Text;
                frmVolume.deed_book = cmbBook.Text;

                if (cmbBook.SelectedValue.ToString() == "3" || cmbBook.SelectedValue.ToString() == "4")
                {
                    frmVolume._isProperty = false;
                }
                else
                {
                    frmVolume._isProperty = true;
                }
                

				string msg = DeedValidation.ChkDeedNo(txtDeedNum.Text);
                if (msg.Trim().ToString().Length  > 0)
                {
                    if (msg == "~1 Alphabet Present~")
                    {
                        DialogResult dialogResult = MessageBox.Show(this, "Alphanumeric deed no.,Do you want to add exception ??", "", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                        if (dialogResult == DialogResult.Yes)
                        {

                            _Dexpc.Clear();
                            deedDetailsException expc = new deedDetailsException();
                            expc.district_code = cmbDistrict.SelectedValue.ToString();
                            expc.RO_code = cmbWhereReg.SelectedValue.ToString();
                            expc.Book = cmbBook.SelectedValue.ToString();
                            expc.Deed_year = txtDeedYear.Text;
                            expc.Deed_no = txtDeedNum.Text;
                            expc.exception = constants.Duplicate_Deed_No;
                            expc.excDetails = pCom.GetException(constants.Duplicate_Deed_No).Tables[0].Rows[0][0].ToString();
                            _Dexpc.Add(expc);
                        }
                    }
                    else
                    {
                        MessageBox.Show(this, "More than 1 Alphabet Present , Invalid Deed No", "", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        txtDeedNum.Focus();
                        return;
                    }
                }
                if (chkOutsideWB.Checked == true)
                {
                    DialogResult dialogResult = MessageBox.Show(this, "OutSide West Bengal Deed,Do you want to add exception ??", "", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                    if (dialogResult == DialogResult.Yes)
                    {
                        deedDetailsException expc = new deedDetailsException();
                        expc.district_code = cmbDistrict.SelectedValue.ToString();
                        expc.RO_code = cmbWhereReg.SelectedValue.ToString();
                        expc.Book = cmbBook.SelectedValue.ToString();
                        expc.Deed_year = txtDeedYear.Text;
                        expc.Deed_no = txtDeedNum.Text;
                        expc.exception = constants.OutSide_WB_Property;
                        expc.excDetails = pCom.GetException(constants.OutSide_WB_Property).Tables[0].Rows[0][0].ToString();
                        _Dexpc.Add(expc);

                    }
                }
				if (Chkhold.Checked == true && txtHoldRemarks.Text.Length == 0)
				{
					MessageBox.Show(this, "Please Enter Hold Reason...", "", MessageBoxButtons.OK, MessageBoxIcon.Stop);
					txtHoldRemarks.Focus();
					return;
				}
				if (cmbDistrict.SelectedValue != null && cmbWhereReg.SelectedValue != null && cmbBook.SelectedValue != null && txtDeedYear.Text.Trim().Length > 0 && txtDeedNum.Text.Trim().Length > 0)
				{
					DeedControl deedC = new DeedControl();
					deedC.District_code = cmbDistrict.SelectedValue.ToString();
					deedC.RO_code = cmbWhereReg.SelectedValue.ToString();
					deedC.Deed_no = txtDeedNum.Text;
					deedC.Book = cmbBook.SelectedValue.ToString();
                    if (cmbBook.SelectedValue.ToString() == "3" || cmbBook.SelectedValue.ToString() == "4")
                    {
                        frmVolume._isProperty = false;
                    }
                    else
                    {
                        frmVolume._isProperty = true;
                    }
					deedC.Deed_year = txtDeedYear.Text;
					_deed.Deed_control = deedC;
					_deed.addl_pages = txtaditionalPages.Text;
					_deed.created_system = "B";
					if (dtpComplition.Text.Trim().Length > 0)
					{
						_deed.date_of_completion = dtpComplition.DateBritish;
					}
					else
					{
						_deed.date_of_completion = null;
					}
					if (dtpdelivery.Text.Trim().Length > 0)
					{
						_deed.date_of_delivery = dtpdelivery.DateBritish;
					}
					else
					{
						_deed.date_of_delivery = null;
					}
					_deed.Deed_control.Deed_no = txtDeedNum.Text;
					_deed.deed_remarks = txtDeedRemarks.Text;
					_deed.Deed_control.Deed_year = txtDeedYear.Text;
					_deed.Deed_control.District_code = cmbDistrict.SelectedValue.ToString();
					_deed.doc_type = cmbdocType.SelectedValue.ToString();
                    if (Chkhold.Checked == true)
                    {
                        _deed.hold = "Y";
                    }
                    else
                    {
                        _deed.hold = "N";
                    }
					_deed.hold_reason = txtHoldRemarks.Text;
					_deed.page_from = txtpgFrom.Text;
					_deed.page_to = txtpageto.Text;
					_deed.scan_doc_type = cmbdocType.SelectedValue.ToString();
					_deed.Serial_no = txtDeedNum.Text;
					_deed.Serial_year = txtDeedYear.Text;
					if (Chkhold.Checked != true)
					{
						_deed.status = constants.Data_edited;
					}
					else
					{
						_deed.status = constants.Data_hold;
					}
					_deed.tran_maj_code = cmbTransactionType.SelectedValue.ToString();
					_deed.tran_min_code = cmbTransMinor.SelectedValue.ToString();
					_deed.version = "";
					_deed.volume_no = txtVolumeNumber.Text;
					//_deed.exception = uuid;
					//_mdeed._GetDeed.DeedHeader = _deed;
                    
				}
				else
				{
					MessageBox.Show(this, "Please Select Deed Control Parameters....", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
					return;
				}
                
				m_OnAccept(_deed);
                
				this.Close();
			}
		}

		private void cmbDistrict_Leave(object sender, EventArgs e)
		{
			if (cmbDistrict.DataSource != null)
			{
				string districtCode = cmbDistrict.SelectedValue.ToString();
				cmbWhereReg.DataSource = pCom.GetROffice(districtCode).Tables[0];
				cmbWhereReg.DisplayMember = "RO_name";
				cmbWhereReg.ValueMember = "RO_code";
			}
		}

		private void cmbTransactionType_Leave(object sender, EventArgs e)
		{
			if (_isEditing == Mode._Add)
			{
				if (cmbTransactionType.DataSource != null)
				{
					cmbTransMinor.DataSource = pCom.GetTransactionTypeMinor(cmbTransactionType.SelectedValue.ToString()).Tables[0];
					cmbTransMinor.DisplayMember = "tran_name";
					cmbTransMinor.ValueMember = "tran_min_code";
					if (cmbTransMinor.Items.Count > 0)
						cmbTransMinor.SelectedIndex = 0;
				}
			}
			else
			{
				if (cmbTransactionType.DataSource != null)
				{
					cmbTransMinor.DataSource = pCom.GetTransactionTypeMinor(cmbTransactionType.SelectedValue.ToString()).Tables[0];
					cmbTransMinor.DisplayMember = "tran_name";
					cmbTransMinor.ValueMember = "tran_min_code";
					if (cmbTransMinor.Items.Count > 0)
						cmbTransMinor.SelectedValue = _deed.tran_min_code.ToString();
				}
			}
			//string msg = DeedValidation.ChkTranMajCode(cmbTransactionType.SelectedValue.ToString());
			//if (msg.Trim().Length > 0)
			//{
			//    MessageBox.Show(this, msg, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
			//    cmbTransactionType.Focus();
			//}
		}

		private void Chkhold_CheckedChanged(object sender, EventArgs e)
		{
			if (this.Chkhold.Checked == true)
			{
				Chkhold.Checked = true;
				txtHoldRemarks.Visible = true;
				deLabel17.Visible = true;
			}
			else
			{
				Chkhold.Checked = false;
				txtHoldRemarks.Visible = false;
				deLabel17.Visible = false;
			}
		}

		private void cmbBook_Leave_1(object sender, EventArgs e)
		{
			//string msg = DeedValidation.ChkBook(cmbBook.SelectedValue.ToString());
			//if (msg.Trim().Length > 0)
			//{
			//    MessageBox.Show(this, msg, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
			//    cmbBook.Focus();
			//}
			if (cmbBook.Text != string.Empty)
			{
				cmbTransactionType.DataSource = pCom.GetTransactionTypeMajor(cmbBook.Text).Tables[0];
				cmbTransactionType.DisplayMember = "tran_maj_name";
				cmbTransactionType.ValueMember = "tran_maj_code";
				cmbTransactionType.SelectedIndex = 0;
			}
            if (cmbBook.SelectedValue.ToString() == "3" || cmbBook.SelectedValue.ToString() == "4")
            {
                frmVolume._isProperty = false;
                chkOutsideWB.Visible = false;
            }
            else
            {
                frmVolume._isProperty = true;
                chkOutsideWB.Visible = true;
            }
		}

		private void cmbDistrict_Leave_1(object sender, EventArgs e)
		{
			if (cmbDistrict.SelectedValue != null)
			{
				string districtCode = cmbDistrict.SelectedValue.ToString();
				cmbWhereReg.DataSource = pCom.GetROffice(districtCode).Tables[0];
				cmbWhereReg.DisplayMember = "RO_name";
				cmbWhereReg.ValueMember = "RO_code";
				cmbWhereReg.SelectedIndex = 0;
			}
		}
		private void txtpageto_Leave(object sender, EventArgs e)
		{
			string msg = DeedValidation.ChkPageTo(txtpageto.Text);
			if (msg.Trim().Length > 0)
			{
				MessageBox.Show(this, msg, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
				txtpageto.Focus();
			}

			int temp = 0;
			if (txtpgFrom.Text != "" && txtpageto.Text != "" && cmbDistrict.Text != "" && cmbWhereReg.Text != "" && cmbBook.Text != "" && txtDeedNum.Text != "" && txtVolumeNumber.Text != "")
			{
				if (Convert.ToInt32(txtpgFrom.Text) > Convert.ToInt32(txtpageto.Text))
				{
					MessageBox.Show(this, "Page from cannot be greater then Pageto... ", "IGR Data Entry...", MessageBoxButtons.OK, MessageBoxIcon.Error);
					txtpageto.Focus();
				}
				temp = pCom.checkPageSequence(txtpgFrom.Text, txtpageto.Text, cmbDistrict.SelectedValue.ToString(), cmbWhereReg.SelectedValue.ToString(), cmbBook.SelectedValue.ToString(), txtDeedYear.Text, txtVolumeNumber.Text);
				if (temp > 0)
				{

					DialogResult dialogResult = MessageBox.Show("Invalid Entry, Do you still want to continue...?", "IGR Data Entry...", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
					if (dialogResult == DialogResult.Yes)
					{
						mismatchPage = "Y";
						txtpgFrom.BackColor = Color.Red;
						txtpageto.BackColor = Color.Red;
					}
					else
					{
						txtpgFrom.Text = "";
						txtpageto.Text = "";
						txtpgFrom.Focus();
					}
				}
			}
		}

		private void cmdAbort_Click(object sender, EventArgs e)
		{
			DialogResult retVal = MessageBox.Show(this, "You sure want to abort?", "Warning you!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);
			if (retVal == DialogResult.Yes)
			{
				m_OnAbort();
				this.Close();
			}
		}

		private void Deed_details_Load(object sender, EventArgs e)
		{
            if (isOutsideWB)
            {
                chkOutsideWB.Visible = true;
            }
            else
            {
                chkOutsideWB.Visible = false;
            }
            if (cmbBook.SelectedValue.ToString() == "3" || cmbBook.SelectedValue.ToString() == "4")
            {
                frmVolume._isProperty = false;
            }
            else
            {
                frmVolume._isProperty = true;
            }
			DateTime date = DateTime.Now;
			this.panelForm.Top = 0;
			this.panelForm.Left = 0;
			this.panelForm.Size = this.Size;
			dtpComplition.Text = date.ToString("ddMMyyyy");
			var incrementedDate = date.AddDays(1);
			dtpdelivery.Text = incrementedDate.ToString("ddMMyyyy");
		}

		private void txtDeedNum_Leave(object sender, EventArgs e)
		{
			
			txtDeedNum.Text = txtDeedNum.Text.PadLeft(5, '0').ToUpper();
			DataSet ds = pCom.GetDuplicateEntry(cmbDistrict.SelectedValue.ToString(), cmbWhereReg.SelectedValue.ToString(), cmbBook.SelectedValue.ToString(), txtDeedYear.Text.Trim(), txtDeedNum.Text.Trim());
			int duplicateCount = ds.Tables[0].Rows.Count;
			if (duplicateCount > 0)
			{
				MessageBox.Show(this, "Record Already Exists in volume - " + ds.Tables[0].Rows[0]["volume_no"].ToString(), "Duplicate Entry", MessageBoxButtons.OK, MessageBoxIcon.Stop);
				txtDeedNum.Focus();
				return;
			}
            //string msg = DeedValidation.ChkDeedNo(txtDeedNum.Text);
            //if (msg.Trim().Length > 0)
            //{
            //    MessageBox.Show(this, "Invalid Deed No", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    //txtDeedNum.Focus();
            //    DialogResult dialogResult = MessageBox.Show("Invalid Deed No, Do you want to Add Exception...?", "Morut Data Entry...", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            //    if (dialogResult == DialogResult.Yes)
            //    {
            //        exception = "03";
            //    }
            //    else
            //    {
            //        exception = "";
            //    }
            //}
			
		}

		private void txtpageto_LocationChanged(object sender, EventArgs e)
		{

		}

		private void panelForm_Paint(object sender, PaintEventArgs e)
		{

		}

		private void txtVolumeNumber_Leave(object sender, EventArgs e)
		{
			if (string.IsNullOrEmpty(txtVolumeNumber.Text))
			{
				txtVolumeNumber.Focus();
			}
			
			
		}

		private void txtDeedNum_KeyPress(object sender, KeyPressEventArgs e)
		{
			e.Handled = !char.IsLetter(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != (char)8;
		}

		private void txtDeedYear_Leave(object sender, EventArgs e)
		{
            int val = 0;
            bool res = Int32.TryParse(txtDeedYear.Text, out val);
            if (res == true && val >= 1985 && val <= 2010)
            {
                
            }
            else
            {
                MessageBox.Show("Please input Valid Year, In Between 1985 and 2010...");
                txtDeedYear.Focus();
            }
            //string msg = DeedValidation.ChkDeedYear(txtDeedYear.Text);
            //if (msg.Trim().Length > 0)
            //{
            //    MessageBox.Show(this, msg, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    txtDeedYear.Focus();
            //}
		}

		private void cmbTransMinor_Leave(object sender, EventArgs e)
		{
			//string msg = DeedValidation.ChkTranMinCode(cmbTransMinor.SelectedValue.ToString());
			//if (msg.Trim().Length > 0)
			//{
			//    MessageBox.Show(this, msg, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
			//    cmbTransMinor.Focus();
			//}
		}

		private void txtpgFrom_Leave(object sender, EventArgs e)
		{
            string msg = DeedValidation.ChkPageFrom(txtpgFrom.Text);
            if (msg.Trim().Length > 0)
            {
                MessageBox.Show(this, msg, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtpgFrom.Focus();
            }
		}

		private void txtaditionalPages_Leave(object sender, EventArgs e)
		{
			//string msg = DeedValidation.IsNumericorBlank(txtaditionalPages.Text);
			//if (msg.Trim().Length > 0)
			//{
			//    MessageBox.Show(this, msg, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
			//    txtaditionalPages.Focus();
			//}
		}

		private void cmbdocType_Leave(object sender, EventArgs e)
		{
			//string msg = DeedValidation.NotBlank(cmbdocType.Text);
			//if (msg.Trim().Length > 0)
			//{
			//    MessageBox.Show(this, msg, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
			//    cmbdocType.Focus();
			//}
		}

		private void dtpComplition_Leave(object sender, EventArgs e)
		{
			//string msg = DeedValidation.ChkDate(dtpComplition.Text);
			//if (msg.Trim().Length > 0)
			//{
			//    MessageBox.Show(this, msg, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
			//    dtpComplition.Focus();
			//}

		}

		private void dtpdelivery_Leave(object sender, EventArgs e)
		{
			//string msg = DeedValidation.ChkDate(dtpdelivery.Text);
			//if (msg.Trim().Length > 0)
			//{
			//    MessageBox.Show(this, msg, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
			//    dtpdelivery.Focus();
			//}
		}

		private void txtDeedRemarks_Leave(object sender, EventArgs e)
		{
			//string msg = DeedValidation.ChkDeedRemarks(txtDeedRemarks.Text);
			//if (msg.Trim().Length > 0)
			//{
			//    MessageBox.Show(this, msg, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
			//    txtDeedRemarks.Focus();
			//}
		}

		private void deLabel17_Click(object sender, EventArgs e)
		{

		}

        private void chkOutsideWB_CheckedChanged(object sender, EventArgs e)
        {
            //if (chkOutsideWB.Checked == true && m_deed._GetDeed.PropertiesoutWB.Count >0)
            //{
            //    DialogResult dialogResult = MessageBox.Show(this, "Property Details Present do you want to delete records ??", "", MessageBoxButtons.YesNo, MessageBoxIcon.Hand);
            //    if (dialogResult == DialogResult.Yes)
            //    {
            //        m_deed._GetDeed.
            //    }

            //}
        }

        private void chkOutsideWB_Click(object sender, EventArgs e)
        {
            
 
        }
	}
}
