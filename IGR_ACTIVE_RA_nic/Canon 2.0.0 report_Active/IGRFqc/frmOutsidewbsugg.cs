using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Data.Odbc;
using System.Data.OleDb;
using DataLayerDefs;
using System.Collections;
using System.IO;
using NovaNet.Utils;

namespace IGRFqc
{
    public partial class frmOutsidewbsugg : Form
    {
        public delegate void SendValues(string pConCode,string pStateCode,string pDistCode);

        private OdbcConnection conn = new OdbcConnection();
        OdbcTransaction txn;
        Credentials crd = new Credentials();
        private PopulateCombo dly;
        frmIndex2outsideWB  frmSource = null;
        SendValues m_CallBack = null;
        PropertyDetailsWB _pDetails = new PropertyDetailsWB();
        bool _hasPreviousData = false;
        private string selectedContryCode = string.Empty;
        private string selectedStateCode = string.Empty;
        private string selectedDisCode = string.Empty;
        public frmOutsidewbsugg(OdbcConnection pCon, OdbcTransaction pTxn, Credentials prmcrd, frmIndex2outsideWB  pfrmSource)
        {
            InitializeComponent();
            conn = pCon;
            txn = pTxn;
            crd = prmcrd;
            dly = new PopulateCombo(conn, txn, crd);
            frmSource = pfrmSource;
            formatForm();
        }
        public frmOutsidewbsugg(OdbcConnection pCon, OdbcTransaction pTxn, Credentials prmcrd, frmIndex2outsideWB pfrmSource, PropertyDetailsWB pDetails, bool pHasPreviousData)
        {
            InitializeComponent();
            conn = pCon;
            txn = pTxn;
            crd = prmcrd;
            dly = new PopulateCombo(conn, txn, crd);
            frmSource = pfrmSource;
            _pDetails = pDetails;
            _hasPreviousData = pHasPreviousData;
            formatForm();
            SetPreviousValues();
        }
        private void formatForm()
        {
            if (constants._SUGGEST)
            {

                this.txtstate.AutoCompleteCustomSource = dly.GetSuggestions("state_master", "state_name");
                this.txtstate.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                this.txtstate.AutoCompleteSource = AutoCompleteSource.CustomSource;

                this.txtDistrict.AutoCompleteCustomSource = dly.GetSuggestions("district_master", "dis_name");
                this.txtDistrict.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                this.txtDistrict.AutoCompleteSource = AutoCompleteSource.CustomSource;
            }
        }
        private void SetPreviousValues()
        {
            if (_hasPreviousData)
            {
                PopulateCombo pCombo = new PopulateCombo(conn, txn,crd);
                txtCountry.Text = pCombo.GetCountryName(_pDetails.Property_country_code);
                txtstate.Text = pCombo.GetStateNameWB(_pDetails.Property_country_code,_pDetails.Property_state_code);
                txtDistrict.Text = pCombo.GetDistrictNameWB(_pDetails.Property_country_code, _pDetails.Property_state_code,_pDetails.Property_district_code);
                
            }
        }

        public frmOutsidewbsugg(OdbcConnection pCon, OdbcTransaction pTxn, Credentials prmcrd, SendValues pCallBack)
        {
            InitializeComponent();
            
            conn = pCon;
            crd = prmcrd;
            dly = new PopulateCombo(conn,txn,crd);
            m_CallBack = pCallBack;
            formatForm();
        }
        private void frmOutsidewbsugg_Load(object sender, EventArgs e)
        {

        }
        public void populateSuggestedValue(string psetDistrict, string psetPs, string psetMouza, string psetjlno, string psetOtherPs, string psetOtherMouza, string psetOtherJlno, string pAreaType, string pAreaTypeValue, string pRO, string pRoad, string pOtherRoad, string pPropertyType, string pLandType)
        {

        }
        private void cmdSearch_Click(object sender, EventArgs e)
        {
            string dis_code = txtDistrict.Text;
            string con_code = txtCountry.Text;
            string state_code = txtstate.Text;

            grdResult.DataSource = null;
            DataSet dtResults = dly.GetMappingSugg(con_code, state_code, dis_code);
            if (dtResults.Tables[0].Rows.Count > 0)
            {
                grdResult.DataSource = dly.GetMappingSugg(con_code, state_code, dis_code).Tables[0];
                this.grpDisplay.Enabled = true;
                this.grpDisplay.Focus();
               // this.lblCount.Text = dtResults.Tables[0].Rows.Count.ToString() + " record(s) found";
            }
            else
            {
                MessageBox.Show("No Data Found...");
            }
            this.grdResult.Focus();
        }

        private void frmOutsidewbsugg_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
        }

        private void cmdAcceptResults_Click(object sender, EventArgs e)
        {
            
        }
        private void GetSelectedValue()
        {
            selectedContryCode = grdResult.CurrentRow.Cells[0].Value.ToString();
            selectedStateCode = grdResult.CurrentRow.Cells[2].Value.ToString();
            selectedDisCode = grdResult.CurrentRow.Cells[4].Value.ToString();
        }

        private void cmdDone_Click(object sender, EventArgs e)
        {
            GetSelectedValue();
            if (m_CallBack != null)
            {
                m_CallBack.Invoke(selectedContryCode, selectedStateCode, selectedDisCode);
            }
            frmSource.populateSuggestedValue(selectedContryCode, selectedStateCode, selectedDisCode);
            this.Close();
        }
    }
}
