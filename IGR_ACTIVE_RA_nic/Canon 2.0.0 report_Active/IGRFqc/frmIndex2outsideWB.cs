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
using valUtils;
using LandAreaConversion;

namespace IGRFqc
{
    public partial class frmIndex2outsideWB : Form
    {
        private PopulateCombo pCom = null;
        Credentials crd = new Credentials();
        private OdbcConnection sqlCon = null;
        OdbcTransaction txn;
        //igr_deed _mdeed;
        bool land = false;
        DeedControl _deedC;
        double acreForeach = 0;
        double decimalForeach = 0;
        double sqFeetStructure = 0;
        double BighaForeach = 0;
        double KathaForeach = 0;
        double ChatakForeach = 0;
        double sqftLandForeach = 0;
        double sqftStructForeach = 0;
        bool PartEntry = false;
        private PopulateCombo dly;
        PropertyDetailsWB propertyWB;
        string setcountry = null;
		string setState = null;
		string setDistrict = null;
        int no_of_split = 0;
        List<outSideWBList> _pexpc =  null;
        //DeedControl _mdeed_control;
        Mode _isEditing;
        int pCount;
        //The method to be invoked when the user is accepting values
        public delegate void OnAccept(PropertyDetailsWB propertyWB);
        OnAccept m_OnAccept;
        //The method to be invoked when the user aborts all operations
        public delegate void OnAbort();
        OnAbort m_OnAbort;
        public frmIndex2outsideWB(OdbcConnection prmCon, OdbcTransaction pTxn, Credentials prmCrd, PropertyDetailsWB pPropertyWB, Mode p_IsEditing, OnAccept pOnAccpet, OnAbort pOnAbort, DeedControl deedC,List<outSideWBList> pExp)
        {
            InitializeComponent();
            sqlCon = prmCon;
            txn = pTxn;
            crd = prmCrd;
            propertyWB = pPropertyWB;
            pCom = new PopulateCombo(sqlCon, txn, crd);
            //pCount = mCount;
            _isEditing = p_IsEditing;
            _pexpc = pExp;
            _deedC = deedC;
            formatForm();
            propertyWB_init(propertyWB);
            //Assign the callbacks
            m_OnAccept = pOnAccpet;
            m_OnAbort = pOnAbort;
            
        }
        public frmIndex2outsideWB(OdbcConnection prmCon, OdbcTransaction pTxn, Credentials prmCrd,PropertyDetailsWB pPropertyWB, Mode p_IsEditing, OnAccept pOnAccpet, OnAbort pOnAbort, DeedControl deedC, int mCount,List<outSideWBList> pExp)
        {
            InitializeComponent();
            sqlCon = prmCon;
            txn = pTxn;
            crd = prmCrd;
            pCom = new PopulateCombo(sqlCon, txn, crd);
            pCount = mCount;
            _isEditing = p_IsEditing;
            _pexpc = pExp;
            _deedC = deedC;
            formatForm();
            propertyWB_init(propertyWB);
            //Assign the callbacks
            m_OnAccept = pOnAccpet;
            m_OnAbort = pOnAbort;
            
        }
        private void formatForm()
        {

            if (constants._SUGGEST == true)
            {
                

                AutoCompleteStringCollection tmp = new AutoCompleteStringCollection();
                tmp.Add("CS");
                tmp.Add("RS");
                tmp.Add("LR");
                this.cmbPlotCode.AutoCompleteCustomSource = tmp;
                this.cmbKhatianType.AutoCompleteCustomSource = tmp;
                this.cmbPlotCode.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                this.cmbPlotCode.AutoCompleteSource = AutoCompleteSource.CustomSource;
                this.cmbKhatianType.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                this.cmbKhatianType.AutoCompleteSource = AutoCompleteSource.CustomSource;
                this.cmbPlotCode.SelectedIndex = 0;
                this.cmbKhatianType.SelectedIndex = 0;
            }
        }
        private DataTable GetDataSetWithNewField(DataSet pDs, string pName, string pCode)
        {
            DataRow newRow = pDs.Tables[0].NewRow();
            newRow[pName] = "Others";
            newRow[pCode] = "0";
            pDs.Tables[0].Rows.Add(newRow);
            return pDs.Tables[0];
        }
        private void propertyWB_init(PropertyDetailsWB propertyWBList)
        {
            cmbIndex2WBCountry.DataSource = pCom.GetCountry().Tables[0];
            cmbIndex2WBCountry.DisplayMember = "cou_name";
            cmbIndex2WBCountry.ValueMember = "cou_code";
            cmbIndex2WBCountry.SelectedValue = "111";
            string contry_Code = cmbIndex2WBCountry.SelectedValue.ToString();
            cmbIndex2WBState.DataSource = pCom.GetState(contry_Code).Tables[0];
            cmbIndex2WBState.DisplayMember = "state_name";
            cmbIndex2WBState.ValueMember = "state_code";
            //cmbIndex2WBState.SelectedIndex = 0;
            string state_code = cmbIndex2WBState.SelectedValue.ToString();
            cmbIndex2WBDistrict.DataSource = pCom.GetDistrictoutsideWB(contry_Code,state_code).Tables[0];
            cmbIndex2WBDistrict.DisplayMember = "dis_name";
            cmbIndex2WBDistrict.ValueMember = "dis_code";
            //cmbIndex2WBDistrict.SelectedIndex = 0;
            
            if (_isEditing == Mode._Edit)
            {
                _pexpc.Clear();
                outSideWBList expc = new outSideWBList();
                expc.district_code = propertyWB.district_code;
                expc.RO_code = propertyWB.RO_code;
                expc.Book = propertyWB.Book;
                expc.Deed_year = propertyWB.Deed_year;
                expc.Deed_no = propertyWB.Deed_no;
                expc.serial = pCount.ToString();
                expc.exception = "Y";
                _pexpc.Add(expc);
                if (propertyWB.Property_country_code != null && propertyWB.Property_country_code != "")
                {
                    //cmbIndex2WBCountry.SelectedValue = propertyWB.Property_country_code;
                    //cmbIndex2WBState.DataSource = pCom.GetState(cmbIndex2WBCountry.SelectedValue.ToString());
                    //cmbIndex2WBState.DisplayMember = "state_name";
                    //cmbIndex2WBState.ValueMember = "state_code";
                    cmbIndex2WBState.SelectedValue = propertyWB.Property_state_code;
                    if (propertyWB.Property_state_code != null && propertyWB.Property_state_code != "")
                    {
                        //cmbIndex2WBDistrict.DataSource = pCom.GetDistrictoutsideWB(cmbIndex2WBCountry.SelectedValue.ToString(), cmbIndex2WBState.SelectedValue.ToString());
                        //cmbIndex2WBDistrict.DisplayMember = "dis_name";
                        //cmbIndex2WBDistrict.ValueMember = "dis_code";
                        cmbIndex2WBDistrict.SelectedValue = propertyWB.Property_district_code;
                    }
                    
                }
                txtThana.Text = propertyWB.thana;
                txtMouza1.Text = propertyWB.mouza;
                cmbPlotCode.Text = propertyWB.Plot_code_type;
                txtPlotNumer.Text = propertyWB.Plot_No;
                cmbKhatianType.Text = propertyWB.Khatian_type;
                txtKhatianNumber.Text = propertyWB.khatian_No;
                txtLandUse.Text = propertyWB.land_use;
                //txtArea.Text = propertyWB.Area;
                txtAcre.Text = propertyWB.Area_acre;
                txtbigha.Text = propertyWB.Area_Bigha;
                txtChatak.Text = propertyWB.Area_Chatak;
                txtDecimal.Text = propertyWB.Area_Decimal;
                txtKatha.Text = propertyWB.Area_Katha;
                txtSqfeetL.Text = propertyWB.Area_SqtL;
                txtstruxturesqft.Text = propertyWB.structure_sqt;
                txtTotalDecimal.Text = propertyWB.Total_decimal;
                txtOtherdetails.Text = propertyWB.other_details;
                txtLandUse.Text = propertyWB.land_use;
                switch (propertyWB.property_type)
                {
                    case "L":
                        { rdaLandStructure.Checked = true; break; }
                    case "A":
                        { rdoApartment.Checked = true; break; }
                }
                switch (propertyWB.local_body_type)
                {
                    case "G":
                        { rdogramP.Checked = true; break; }
                    case "M":
                        { rdomunicipality.Checked = true; break; }
                    case "C":
                        { rdoCorp.Checked = true; break; }
                }
                
            }
        }
        private void cmdAbort_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult retVal = MessageBox.Show(this, "You sure want to abort?", "Warning you!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);
                if (retVal == DialogResult.Yes)
                {
                    m_OnAbort.Invoke();
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        private void cmbIndex2PS_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void cmbIndex2WBCountry_SelectedIndexChanged(object sender, EventArgs e)
        {
            DataTable ds = new DataTable();
            if (cmbIndex2WBCountry.SelectedValue != null && cmbIndex2WBCountry.SelectedValue.ToString() != "")
            {
                string conCode = cmbIndex2WBCountry.SelectedValue.ToString();
                ds = pCom.GetState(conCode).Tables[0];
                cmbIndex2WBState.DataSource = ds;
                cmbIndex2WBState.DisplayMember = "state_name";
                cmbIndex2WBState.ValueMember = "state_code";
                //if (constants._SUGGEST == true)
                //{
                //    AutoCompleteStringCollection tmp = new AutoCompleteStringCollection();
                //    for (int i = 0; i < ds.Rows.Count; i++)
                //    {
                //        tmp.Add(ds.Rows[i][1].ToString());
                //    }
                //    this.cmbIndex2WBState.AutoCompleteCustomSource = tmp;
                //    this.cmbIndex2WBState.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                //    this.cmbIndex2WBState.AutoCompleteSource = AutoCompleteSource.CustomSource;
                //}
            }
        }

        private void cmbIndex2WBState_SelectedIndexChanged(object sender, EventArgs e)
        {
            DataTable ds = new DataTable();
            if (cmbIndex2WBState.SelectedValue != null && cmbIndex2WBState.SelectedValue.ToString() != "")
            {
                string conCode = cmbIndex2WBCountry.SelectedValue.ToString();
                string stateCode = cmbIndex2WBState.SelectedValue.ToString();
                ds = pCom.GetDistrictoutsideWB(conCode,stateCode).Tables[0];
                cmbIndex2WBDistrict.DataSource = ds;
                cmbIndex2WBDistrict.DisplayMember = "dis_name";
                cmbIndex2WBDistrict.ValueMember = "dis_code";
                //if (constants._SUGGEST == true)
                //{
                //    AutoCompleteStringCollection tmp = new AutoCompleteStringCollection();
                //    for (int i = 0; i < ds.Rows.Count; i++)
                //    {
                //        tmp.Add(ds.Rows[i][1].ToString());
                //    }
                //    this.cmbIndex2WBDistrict.AutoCompleteCustomSource = tmp;
                //    this.cmbIndex2WBDistrict.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                //    this.cmbIndex2WBDistrict.AutoCompleteSource = AutoCompleteSource.CustomSource;
                //}
            }
        }
        private Boolean ValidateInputData()
        {
            bool flag = true;
            
            //if (txtPlotNumer.Text.Trim().Length == 0)
            //{
            //    MessageBox.Show(this, "Please Enter Plot Number... ", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    flag = false;
            //}
            //if (txtMouza1.Text.Trim().Length > 0)
            //{
            //    string msg = DeedValidation.ChkPlotNumber(txtMouza1.Text.Trim());
            //    if (msg.Trim().Length > 0)
            //    {
            //        MessageBox.Show(this, msg, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //        txtMouza1.Focus();
            //        flag = false;
            //    }
            //}
           

            return flag;
        }
        private void cmdSave_Click(object sender, EventArgs e)
        {
            if (ValidateInputData())
            {
                //_pexpc.Clear();
                //outSideWBList expc = new outSideWBList();
                //expc.district_code = propertyWB.district_code;
                //expc.RO_code = propertyWB.RO_code;
                //expc.Book = propertyWB.Book;
                //expc.Deed_year = propertyWB.Deed_year;
                //expc.Deed_no = propertyWB.Deed_no;
                //expc.serial = pCount.ToString();
                //expc.exception = constants.OutSide_WB_Property;
                //_pexpc.Add(expc);
                propertyWB.Property_country_code = cmbIndex2WBCountry.SelectedValue.ToString();
                propertyWB.Property_state_code = cmbIndex2WBState.SelectedValue.ToString();
                propertyWB.Property_district_code = cmbIndex2WBDistrict.SelectedValue.ToString();
                propertyWB.thana = txtThana.Text;
                propertyWB.mouza = txtMouza1.Text;
                propertyWB.Plot_code_type = cmbPlotCode.Text;
                propertyWB.Plot_No = txtPlotNumer.Text;
                propertyWB.Khatian_type = cmbKhatianType.Text;
                propertyWB.khatian_No = txtKhatianNumber.Text;
                propertyWB.land_use = txtLandUse.Text;
                //propertyWB.Area = txtArea.Text;
                propertyWB.Area_acre = txtAcre.Text;
                propertyWB.Area_Bigha = txtbigha.Text;
                propertyWB.Area_Decimal = txtDecimal.Text;
                propertyWB.Area_Katha = txtKatha.Text;
                propertyWB.Area_Chatak = txtChatak.Text;
                propertyWB.Area_SqtL = txtSqfeetL.Text;
                propertyWB.Total_decimal = txtTotalDecimal.Text;
                propertyWB.structure_sqt = txtstruxturesqft.Text;
                if (rdaLandStructure.Checked == true)
                {
                    propertyWB.property_type = "L";
                }
                if (rdoApartment.Checked == true)
                {
                    propertyWB.property_type = "A";
                }
                if (rdogramP.Checked == true)
                {
                    propertyWB.local_body_type = "G";
                }
                if (rdomunicipality.Checked == true)
                {
                    propertyWB.local_body_type = "M";
                }
                if (rdoCorp.Checked == true)
                {
                    propertyWB.local_body_type = "C";
                }
                propertyWB.other_details = txtOtherdetails.Text;
                //
                if (_isEditing == Mode._Add)
                {

                }
                m_OnAccept.Invoke(propertyWB);
                if (_isEditing == Mode._Add)
                {
                    DialogResult dialogResult = MessageBox.Show("You sure wanna exit...?", " ", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (dialogResult == DialogResult.Yes)
                    {
                        this.Close();
                    }
                    else
                    {
                        txtThana.Focus();
                    }
                }
                else
                {
                    this.Close();
                }
            }
        }

        //private void rdaLandStructure_KeyUp(object sender, KeyEventArgs e)
        //{
        //    //if (e.KeyCode == Keys.Return)
        //    //    SendKeys.Send("{Tab}");
        //}

        //private void rdoApartment_KeyUp(object sender, KeyEventArgs e)
        //{
        //    //if (e.KeyCode == Keys.Return)
        //    //    SendKeys.Send("{Tab}");
        //}

        //private void rdogramP_KeyUp(object sender, KeyEventArgs e)
        //{
        //    if (e.KeyCode == Keys.Return)
        //        SendKeys.Send("{Tab}");
        //}

        //private void rdomunicipality_KeyUp(object sender, KeyEventArgs e)
        //{
        //    if (e.KeyCode == Keys.Return)
        //        SendKeys.Send("{Tab}");
        //}

        //private void rdoCorp_KeyUp(object sender, KeyEventArgs e)
        //{
        //    if (e.KeyCode == Keys.Return)
        //        SendKeys.Send("{Tab}");
        //}

        private void rdaLandStructure_Click(object sender, EventArgs e)
        {
            if (rdaLandStructure.Checked == true)
            {
                rdogramP.Checked = true;
            }
        }

        private void rdoApartment_Click(object sender, EventArgs e)
        {
            if (rdoApartment.Checked == true)
            {
                rdogramP.Checked = true;
            }
        }
        public void GetResultsFromAnadi(int pNoSplits, double pAcre, double pBigha, double pKatha, double pChhatak, bool pLand, double pSqFeet, double pDecimal, double psqftLand, double psqftStruct)
        {
            acreForeach = pAcre;
            BighaForeach = pBigha;
            KathaForeach = pKatha;
            ChatakForeach = pChhatak;
            land = pLand;
            no_of_split = pNoSplits;
            decimalForeach = pDecimal;
            sqFeetStructure = pSqFeet;
            sqftLandForeach = psqftLand;
            sqftStructForeach = psqftStruct;
            double fstTab = acreForeach + BighaForeach + KathaForeach + ChatakForeach + decimalForeach + sqftLandForeach + sqftStructForeach;
            if (fstTab > 0 && sqFeetStructure > 0)
            {
                DialogResult dialogResult = MessageBox.Show("Do you want to accept both values?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2);
                if (dialogResult == DialogResult.Yes)
                {
                    PopulateSplitVal();
                }
                else
                {
                    txtAcre.Text = "";
                    txtbigha.Text = "";
                    txtDecimal.Text = "";
                    txtKatha.Text = "";
                    txtChatak.Text = "";
                    txtSqfeetL.Text = "";
                    txtTotalDecimal.Text = "";
                    txtstruxturesqft.Text = "0";

                    rdoAcre.Checked = false;
                    rdoBigha.Checked = false;
                    rdoDecimal.Checked = false;
                    rdoKatha.Checked = false;
                    rdoChatak.Checked = false;
                    roSqFeet.Checked = false;
                    rdoStructureSqFeet.Checked = false;
                }
            }
            else
            {
                PopulateSplitVal();
            }
        }
        private void PopulateSplitVal()
        {
            double mesurement = 0;
            if (acreForeach.ToString() != "0")
            {
                rdoAcre.Checked = true;
                //txtAcre.Enabled = true;
            }
            else
            {
                rdoAcre.Checked = false;
            }
            if (BighaForeach.ToString() != "0")
            {
                rdoBigha.Checked = true;
               // txtbigha.Enabled = true;
            }
            else
            {
                rdoBigha.Checked = false;
            }
            if (KathaForeach.ToString() != "0")
            {
                rdoKatha.Checked = true;
                //txtKatha.Enabled = true;
            }
            else
            {
                rdoKatha.Checked = false;
            }
            if (ChatakForeach.ToString() != "0")
            {
                rdoChatak.Checked = true;
                //txtChatak.Enabled = true;
                //txtChatak.Text = ChatakForeach.ToString();
            }
            else
            {
                rdoChatak.Checked = false;
            }
            if (decimalForeach.ToString() != "0")
            {
                rdoDecimal.Checked = true;
                //txtChatak.Enabled = true;
                //txtChatak.Text = ChatakForeach.ToString();
            }
            else
            {
                rdoDecimal.Checked = false;
            }
            if (sqftLandForeach.ToString() != "0")
            {
                roSqFeet.Checked = true;
            }
            else
            {
                roSqFeet.Checked = false;
            }
            if (sqftStructForeach.ToString() != "0")
            {
                rdoStructureSqFeet.Checked = true;
            }
            else
            {
                rdoStructureSqFeet.Checked = false;
            }

            /* Off by Arpan */

            if (sqFeetStructure.ToString() != "0")
            {

                if (land)
                {
                    roSqFeet.Checked = true;
                    //txtSqfeetL.Enabled = true;
                    txtSqfeetL.Text = sqFeetStructure.ToString();
                    rdoStructureSqFeet.Checked = false;
                    txtstruxturesqft.Text = "0";
                }
                else
                {
                    rdoStructureSqFeet.Checked = true;
                    //txtSqfeetStructure.Enabled = true;
                    roSqFeet.Checked = false;
                    txtSqfeetL.Text = "0";
                    txtTotalDecimal.Text = "0";
                    txtstruxturesqft.Text = sqFeetStructure.ToString();
                }

            }
            else
            {
                txtSqfeetL.Text = sqftLandForeach.ToString();
                txtstruxturesqft.Text = sqftStructForeach.ToString();
                //roSqFeet.Checked = false;
                //txtSqfeetL.Text = "0";
                //rdoStructureSqFeet.Checked = false;
                //txtstruxturesqft.Text = "0";
            }

            /* END */

            txtDecimal.Enabled = false;
            txtAcre.Text = acreForeach.ToString();
            txtbigha.Text = BighaForeach.ToString();
            txtKatha.Text = KathaForeach.ToString();
            txtChatak.Text = ChatakForeach.ToString();
            txtDecimal.Text = decimalForeach.ToString();
            
            // calculate total dcimal value
            if (txtAcre.Text.Trim().Length > 0)
            {
                    mesurement = mesurement + Convert.ToDouble(txtAcre.Text) * 100;
            }
            if (txtbigha.Text.Trim().Length > 0)
            {

                    mesurement = mesurement + Convert.ToDouble(txtbigha.Text) * 33;
            }
            if (txtDecimal.Text.Trim().Length > 0)
            {
                    mesurement = mesurement + Convert.ToDouble(txtDecimal.Text)* 1;
            }
            if (txtKatha.Text.Trim().Length > 0)
            {
                    mesurement = mesurement + Convert.ToDouble(txtKatha.Text) * 1.65;
            }
            if (txtChatak.Text.Trim().Length > 0)
            {
                    mesurement = mesurement + Convert.ToDouble(txtChatak.Text) * 0.103125;
            }
            if (txtSqfeetL.Text.Trim().Length > 0)
            {

                    mesurement = mesurement + Convert.ToDouble(txtSqfeetL.Text) * 0.00229167;
            }
            txtTotalDecimal.Text = mesurement.ToString();
            //txtDecimal.Text = "0";
            //txtSqfeetL.Text = "0";
            
        }
        private void cmdConvert_Click(object sender, EventArgs e)
        {
            LandAreaConversion.frmConverter frm = new frmConverter(GetResultsFromAnadi);
            frm.ShowDialog();
            frm.Activate();
            if (no_of_split > 1)
            {
                PartEntry = true;
            }
            else
            {
                PartEntry = false;
            }
            txtOtherdetails.Select();
            //SendKeys.Send("{Tab}");
        }

        private void cmdjuri_Click(object sender, EventArgs e)
        {
            frmOutsidewbsugg sugg;
            if ((_isEditing == Mode._Add) && (propertyWB.Property_district_code != null))
            {
                sugg = new frmOutsidewbsugg(sqlCon, txn, crd, this, propertyWB, true);
            }
            else
            {
                sugg = new frmOutsidewbsugg(sqlCon, txn, crd, this, propertyWB, false);
            }

            sugg.ShowDialog(this);

            groupBox7.Focus();
            txtThana.Focus();
            
        }
        public void populateSuggestedValue(string psetCountry, string psetState, string psetdist)
        {
            setcountry = psetCountry;
            setState = psetState;
            setDistrict = psetdist;
            cmbIndex2WBCountry.SelectedValue = setcountry;
            cmbIndex2WBState.SelectedValue = setState;
            cmbIndex2WBDistrict.SelectedValue = setDistrict;
            groupBox3.Focus();
            txtThana.Focus();
        }

        private void frmIndex2outsideWB_Load(object sender, EventArgs e)
        {

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (rdoStructureSqFeet.Checked == true)
            {
                txtstruxturesqft.Focus();
                txtstruxturesqft.SelectAll();
            }
            else
            {
                txtstruxturesqft.Text = "";
                groupBox10.Focus();
            }
        }

        private void txtstruxturesqft_Enter(object sender, EventArgs e)
        {
            txtstruxturesqft.SelectAll();
        }

        //private void rdaLandStructure_KeyUp_1(object sender, KeyEventArgs e)
        //{
        //    if (e.KeyCode == Keys.Return)
        //        SendKeys.Send("{Tab}");
        //}

        //private void rdoApartment_KeyUp_1(object sender, KeyEventArgs e)
        //{
        //    if (e.KeyCode == Keys.Return)
        //        SendKeys.Send("{Tab}");
        //}

        //private void rdogramP_KeyUp_1(object sender, KeyEventArgs e)
        //{
        //    if (e.KeyCode == Keys.Return)
        //        SendKeys.Send("{Tab}");
        //}

        //private void rdomunicipality_KeyUp_1(object sender, KeyEventArgs e)
        //{
        //    if (e.KeyCode == Keys.Return)
        //        SendKeys.Send("{Tab}");
        //}

        //private void rdoCorp_KeyUp_1(object sender, KeyEventArgs e)
        //{
        //    if (e.KeyCode == Keys.Return)
        //        SendKeys.Send("{Tab}");
        //}

        private void rdoApartment_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void rdaLandStructure_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Return)
                SendKeys.Send("{Tab}");
        }

        private void rdoApartment_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Return)
                SendKeys.Send("{Tab}");
        }

        private void rdogramP_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Return)
                SendKeys.Send("{Tab}");
        }

        private void rdomunicipality_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Return)
                SendKeys.Send("{Tab}");
        }

        private void rdoCorp_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Return)
                SendKeys.Send("{Tab}");
        }

        private void txtSqfeetL_TextChanged(object sender, EventArgs e)
        {

        }

        

        
    }
}
