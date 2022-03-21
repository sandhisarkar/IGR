using System;
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

namespace IGRFqc
{
    public partial class frmdeedControl : Form
    {
        NovaNet.Utils.dbCon dbcon = new NovaNet.Utils.dbCon();
        DeedControl pList = new DeedControl();
        Credentials crd = new Credentials();
        private OdbcConnection sqlCon;
        OdbcTransaction trans;
        Mode pMode;
        string Volumeno = string.Empty;
        string NewVolumeno = string.Empty;
        public delegate void OnAccept();
        OnAccept m_OnAccept;
        //The method to be invoked when the user aborts all operations
        public delegate void OnAbort();
        OnAbort m_OnAbort;
        bool userauthenticated = false;
        public frmdeedControl(OdbcConnection prmCon, Credentials prmCrd, DeedControl _pList, Mode _pMode, string Vol, OnAccept pOnAccpet, OnAbort pOnAbort)
        {
            InitializeComponent();
            sqlCon = prmCon;
            crd = prmCrd;
            pList = _pList;
            pMode = _pMode;
            Volumeno = Vol;
            m_OnAbort = pOnAbort;
            m_OnAccept = pOnAccpet;
            init_dControl();

        }
        
        public frmdeedControl(OdbcConnection prmCon, Credentials prmCrd, DeedControl _pList, Mode _pMode,string newdeedno,OdbcTransaction pTxn)
        {
            InitializeComponent();
            sqlCon = prmCon;
            crd = prmCrd;
            pList = _pList;
            pMode = _pMode;
            init_dControl();
            txtnewdeedno.Text  = newdeedno;
            txtnewdeedno.Enabled = false;
            txtnewdeedyear.Enabled = false;
            txtnewvol.Enabled = false;
            trans = pTxn;

        }
        private void init_dControl()
        {
        	if (!crd.role.ToLower().Equals("admin"))
            {
            	MessageBox.Show("You can't afford this function!","What the ****",MessageBoxButtons.OK,MessageBoxIcon.Stop, MessageBoxDefaultButton.Button1);
            	cmbConfirm.Enabled = false;
            	return;
            }
            if (pMode == Mode._Delete)
            {
                lblstatus.Text = "Deleting Deed";
                cmbConfirm.Text = "Delete";
            }
            if (pMode == Mode._Rename)
            {
                lblstatus.Text = "Updating Deed";
                cmbConfirm.Text = "Update";
            }

                txtdocode.Text = pList.District_code.ToString();
                txtroCode.Text = pList.RO_code.ToString();
                txtBook.Text = pList.Book.ToString();
                txtDeedYear.Text = pList.Deed_year.ToString();
                txtdeedNumber.Text = pList.Deed_no.ToString();
                txtnewdeedno.Text = pList.Deed_no.ToString();
                txtVol.Text = Volumeno;
                txtnewvol.Text = Volumeno;
                txtnewdeedyear.Text = pList.Deed_year.ToString();
        }
        private void frmdeedControl_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                m_OnAbort.Invoke();
                this.Close();
            }
        }

        private void cmbConfirm_Click(object sender, EventArgs e)
        {
            if (trans == null)
            {
                trans = sqlCon.BeginTransaction();
            }
            if (pMode == Mode._Rename)
            {
                igr_deed igr = new igr_deed(sqlCon, trans, crd, pList);
                if (txtnewdeedno.Text == "")
                {
                    lblWarning.Text = "";
                    lblWarning.Text = "Enter New Deed No. ";
                    return;
                }
                NewVolumeno = txtnewvol.Text.Trim();
                DeedControl ndc = new DeedControl();
                ndc.Book = txtBook.Text;
                ndc.Deed_no = txtnewdeedno.Text;
                ndc.Serial_no = ndc.Deed_no;
                ndc.Deed_year = txtnewdeedyear.Text;
                ndc.District_code = txtdocode.Text;
                ndc.RO_code = txtroCode.Text;

                
                DataTable dt = igr.DeedExists(ndc);
                if (dt.Rows.Count > 0)
                {
                    if (Convert.ToInt32(dt.Rows[0][0].ToString()) > 0)
                    {
                        lblWarning.Text = "";
                        lblWarning.Text = "Duplicate Deed Present in Vol: " + dt.Rows[0][1].ToString();
                        DialogResult dr = MessageBox.Show("Do you want to update Volume only?", "Duplicate Deed Present in Vol: " + dt.Rows[0][1].ToString(), MessageBoxButtons.YesNo,MessageBoxIcon.Question,MessageBoxDefaultButton.Button2);
                    	if (dr == DialogResult.Yes)
                    	{
                    		if (igr.RenameVolume(ndc, NewVolumeno))
                    		{
                    			lblWarning.Text = "Volume updated to " + NewVolumeno;
                    			trans.Commit();
                    			init_dControl();
                    			return;
                    		}
                            
                    	}
                        else
                        {
	                        txtnewdeedno.Focus();
	                        trans.Rollback();
	                        return;
                        }
                    }
                }
                
                
                if (igr.RenameDeed(ndc, Volumeno, NewVolumeno) == true)
                {
                    trans.Commit();
                    lblWarning.Text = "";
                    lblWarning.Text = "Deed Updated";
                    cmbConfirm.Enabled = false;
                    m_OnAccept.Invoke();
                }
                else
                {
                    trans.Rollback();
                    lblWarning.Text = "";
                    lblWarning.Text = "Something wrong has happened!";
                    m_OnAbort.Invoke();
                }
            }
            else
            {
                DialogResult result = (MessageBox.Show(this, "Do you want to delete the selected Deed", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2));
                if (result == DialogResult.Yes)
                {
                    frmauthentication auth = new frmauthentication(sqlCon, crd, _OnAccept, _OnAbort, trans);
                    auth.ShowDialog(this);
                    if (userauthenticated == true)
                    {
                        DialogResult result1 = (MessageBox.Show(this, "You are about to delete selected Deed, Are you sure??", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2));
                        if (result1 == DialogResult.Yes)
                        {
                            igr_deed igr = new igr_deed(sqlCon, trans, crd, pList);
                            if (igr.DeleteDeed() == true)
                            {
                                trans.Commit();
                                lblWarning.Text = "";
                                lblWarning.Text = "Deed Deleted";
                                m_OnAccept.Invoke();
                            }
                            else
                            {
                                trans.Rollback();
                                lblWarning.Text = "";
                                lblWarning.Text = "Something wrong has happened!";
                                m_OnAbort.Invoke();
                            }
                        }
                    }
                    else
                    {
                        trans.Rollback();
                        lblWarning.Text = "";
                        lblWarning.Text = "Password not Verified";
                    }
                }
                
            }
        }
        public void _OnAccept(bool _result)
        {
            userauthenticated = _result;
        }
        public void _OnAbort()
        {
            userauthenticated = false;
        }
        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (linkLabel1.Text == "Advance Editing ON")
            {
                grpadvanceEdit.Enabled = true;
                linkLabel1.Text = "Advance Editing OFF";
                return;
            }
            if (linkLabel1.Text == "Advance Editing OFF")
            {
                grpadvanceEdit.Enabled = false;
                linkLabel1.Text = "Advance Editing ON";
                return;
            }
        }
        void FrmdeedControlLoad(object sender, EventArgs e)
        {
        	//Draw a thin line around the form
			this.panelForm.Top = 0;
            this.panelForm.Left = 0;
            this.panelForm.Size = this.Size;
        }
    }
}
