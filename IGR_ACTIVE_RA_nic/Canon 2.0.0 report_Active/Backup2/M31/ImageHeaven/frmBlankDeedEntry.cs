using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DataLayerDefs;
using IGRFqc;
using System.Data.Odbc;
using NovaNet.Utils;
using LItems;
using igr_base;
namespace ImageHeaven
{
    

    public partial class frmBlankDeedEntry : Form
    {
        public event EventHandler<DeedEventArgs> OnCommit;
        public event EventHandler<DeedEventArgs> OnAbort;
        PolicyDetails _File = new PolicyDetails();
        private PopulateCombo pCom;
        DeedDetails _Details = new DeedDetails();
        OdbcConnection sqlCon = new OdbcConnection();
        Credentials crd = new Credentials();
        OdbcTransaction txn = null;
        igr_base.igr_deed igr = null;
        public frmBlankDeedEntry(OdbcConnection prmCon, Credentials prmCrd, PolicyDetails pFile, DeedDetails pDetails,OdbcTransaction pTxn)
        {
            InitializeComponent();
            _File = pFile;
            _Details = pDetails;
            sqlCon = prmCon;
            crd = prmCrd;
            txn = pTxn;
            pCom = new PopulateCombo(sqlCon, txn, crd);
        }

        private void frmBlankDeedEntry_Load(object sender, EventArgs e)
        {
            if (pCom.GetDistrict_Active().Tables[0].Rows.Count > 0)
            {
                cmbDistrict.DataSource = pCom.GetDistrict_Active().Tables[0];
                cmbDistrict.DisplayMember = "district_name";
                cmbDistrict.ValueMember = "district_code";
                cmbDistrict.SelectedIndex = 0;
                cmbDistrict.SelectedValue = _Details.Deed_control.District_code;
            }
            if (cmbDistrict.DataSource != null)
            {
                cmbWhereReg.DataSource = pCom.GetROffice(_Details.Deed_control.District_code).Tables[0];
                cmbWhereReg.DisplayMember = "RO_name";
                cmbWhereReg.ValueMember = "RO_code";
                cmbWhereReg.SelectedValue = _Details.Deed_control.RO_code;
            }
            DataSet dsBook = pCom.GetBookType();
            if (dsBook.Tables[0].Rows.Count > 0)
            {
                cmbBook.DataSource = pCom.GetBookType().Tables[0];
                cmbBook.DisplayMember = "key_book";
                cmbBook.ValueMember = "value_book";
                cmbBook.SelectedIndex = 0;
                cmbBook.SelectedValue = _Details.Deed_control.Book;
            }
            cmbDeedYear.Text = _Details.Deed_control.Deed_year;
            cmbVolume.Text = _File.vol;
        }

        private void cmdSave_Click(object sender, EventArgs e)
        {
            if (txtDeed.Text.Trim().Length < 5)
            {
                MessageBox.Show("Deed number length should not be less than 5...");
                return;
            }
            if (cmbVolume.Text.Trim() == "")
            {
                MessageBox.Show("Volume number cannot be null...");
                return;
            }
            _File.policy_number = _File.policy_number.Replace(_Details.Deed_control.Deed_no, txtDeed.Text);
            _Details.Deed_control.Deed_no = txtDeed.Text.Trim();
            _Details.hold = "Y";
            _Details.hold_reason = "Manually inserted";
            _File.policy_path = _File.policy_path + "\\" + _File.policy_number;
            _File.policy_path = _File.policy_path.Replace("\\","\\\\");
            _File.Deed_no = txtDeed.Text.Trim();
            _File.Page_from = _Details.page_from;
            _File.Page_to = _Details.page_to;
            _File.Book = _Details.Deed_control.Book;
            
            igr = new igr_base.igr_deed(sqlCon,txn,crd);
            igr._GetDeed.DeedHeader = _Details;
            DataTable _dt = igr.DeedExists(_Details.Deed_control);
            if (_dt.Rows.Count <= 0)
            {
                if (igr.SaveDeed())
                {
                    wfePolicy wPolicy = new wfePolicy(sqlCon);
                    if (wPolicy.SaveBlankDeeds(_File, txn))
                    {
                        if(wPolicy.UpdateDeeds(txn,igr._GetDeed.DeedHeader.Deed_control.District_code,igr._GetDeed.DeedHeader.Deed_control.RO_code,igr._GetDeed.DeedHeader.Deed_control.Book,igr._GetDeed.DeedHeader.Deed_control.Deed_year,igr._GetDeed.DeedHeader.Deed_control.Deed_no,_File))
                        {
                        EventHandler<DeedEventArgs> CommitHandler = OnCommit;
                        if (CommitHandler != null)
                        {
                            DeedEventArgs tmpDeedEventArgs = new DeedEventArgs(igr._GetDeed);
                            OnCommit(this, tmpDeedEventArgs);
                        }
                        else
                        {
                            EventHandler<DeedEventArgs> AbortHandler = OnAbort;
                            if (AbortHandler != null)
                            {
                                DeedEventArgs tmpDeedEventArgs = new DeedEventArgs(igr._GetDeed);
                                OnAbort(this, tmpDeedEventArgs);
                            }
                        }
                        this.Close();
                        }
                    }
                    else
                    {
                        MessageBox.Show("Error while saving the new deed.....");
                    }
                }
                else { MessageBox.Show("Error while saving the new deed....."); }
            }
            else
            {
                { MessageBox.Show("Deed already exists in " + _dt.Rows[0]["volume_no"]); }
            }
        }

        private void igr_OnCommit(object sender, igr_base.DeedEventArgs e)
        {
            _File.Page_from = e._Deed.DeedHeader.page_from;
            _File.Page_to = e._Deed.DeedHeader.page_to;
        }
        private void igr_OnAbort(object sender, igr_base.DeedEventArgs e)
        {
            //MessageBox.Show("Not updating B'zer tables...");
        }
        private void cmdCancel_Click(object sender, EventArgs e)
        {
            EventHandler<DeedEventArgs> AbortHandler = OnAbort;
            if (AbortHandler != null)
            {
                DeedEventArgs tmpDeedEventArgs = new DeedEventArgs(igr!= null ?igr._GetDeed : null);
                OnAbort(this, tmpDeedEventArgs);
            }
            this.Close();
        }

        private void txtDeed_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsLetter(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != (char)8;
        }
    }
   
}
