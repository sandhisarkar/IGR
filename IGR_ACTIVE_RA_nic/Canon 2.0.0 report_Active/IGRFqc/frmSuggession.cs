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
    public partial class frmSuggession : Form
    {
        public delegate void SendValues(string pDistCode, string pPSCode, string pMouzaCode, string pJLNo, string pOthPS, string pOthMouza, string pOthJLNo);

        private OdbcConnection conn = new OdbcConnection();
        OdbcTransaction txn;
        Credentials crd = new Credentials();
        private PopulateCombo dly;
        frmIndex2 frmSource=null;
        SendValues m_CallBack=null;
        PropertyDetails _pDetails = new PropertyDetails();
        bool _hasPreviousData = false;

        public frmSuggession(OdbcConnection pCon,OdbcTransaction pTxn, Credentials prmcrd, frmIndex2 pfrmSource)
        {
            InitializeComponent();
            conn = pCon;
            txn = pTxn;
            crd = prmcrd;
            dly = new PopulateCombo(conn,txn, crd);
            cmdReset_Click(this,null);
            frmSource = pfrmSource;
            formatForm();
        }
        public frmSuggession(OdbcConnection pCon, OdbcTransaction pTxn, Credentials prmcrd, frmIndex2 pfrmSource, PropertyDetails pDetails, bool pHasPreviousData)
        {
            InitializeComponent();
            conn = pCon;
            txn = pTxn;
            crd = prmcrd;
            dly = new PopulateCombo(conn, txn, crd);
            cmdReset_Click(this, null);
            frmSource = pfrmSource;
            _pDetails = pDetails;
            _hasPreviousData = pHasPreviousData;
            formatForm();
            SetPreviousValues();
        }

        private void SetPreviousValues()
        {
            if (_hasPreviousData)
            {
                PopulateCombo pCombo = new PopulateCombo(conn, txn,crd);
                txtSelDistrict.Text = pCombo.GetDistrictName(_pDetails.Property_district_code);
                txtSelPS.Text = pCombo.GetPSName(_pDetails.Property_district_code, _pDetails.ps_code);
                txtSelMouza.Text = pCombo.GetMouzaName(_pDetails.Property_district_code, _pDetails.ps_code, _pDetails.moucode);
                txtSelJLNo.Text = _pDetails.JL_NO;

                txtOutDistrict.Text = _pDetails.Property_district_code;
                txtOutPS.Text = _pDetails.ps_code;
                txtOutMouza.Text = _pDetails.moucode;
                txtOutJLNo.Text = _pDetails.JL_NO;

                txtOthMouza.Text = _pDetails.Ref_mou;
                txtOthPS.Text = _pDetails.Ref_ps;
                txtOthJLNo.Text = _pDetails.Ref_JL_Number;

                CmdPMJOkClick(this, null);

                switch (_pDetails.Area_type)
                {
                    case "":
                        {
                            cmbGMC.SelectedIndex = 0;
                            break;
                        }
                    case "G":
                        {
                            cmbGMC.SelectedIndex = 1;
                            break;
                        }
                    case "M":
                        {
                            cmbGMC.SelectedIndex = 2;
                            break;
                        }
                    case "C":
                        {
                            cmbGMC.SelectedIndex = 3;
                            break;
                        }
                }
                CmbGMCValueEnter(this, null);
                cmbGMCValue.SelectedValue = _pDetails.GP_Muni_Corp_Code;
                CmdAreaTypeClick(this, null);
                CmbIndex2RONameEnter(this, null);
                cmbIndex2ROName.SelectedValue = _pDetails.Property_ro_code;
                CmbIndex2RoadEnter(this, null);
                if (!string.IsNullOrEmpty(_pDetails.road_code))
                {
                    cmbIndex2Road.SelectedValue = _pDetails.road_code;
                }
                else
                {
                    cmbIndex2Road.SelectedValue = "0";
                }
                CmbIndex2RoadLeave(this, null);
                txtIndex2Road.Text = _pDetails.Road;
                cmdRORoadClick(this, null);
            }
        }

        public frmSuggession(OdbcConnection pCon, OdbcTransaction pTxn, Credentials prmcrd, SendValues pCallBack)
        {
            InitializeComponent();
            
            conn = pCon;
            crd = prmcrd;
            dly = new PopulateCombo(conn,txn,crd);
            cmdReset_Click(this, null);
            m_CallBack = pCallBack;
            formatForm();
        }

        private void frmSuggession_Load(object sender, EventArgs e)
        {
            if (_hasPreviousData)
            {
                populatedistrict();
                CmbIndex2RONameEnter(this, null);
                cmbIndex2ROName.SelectedValue = _pDetails.Property_ro_code;
            }
            else
            {
                populatedistrict();
            }
        }
        
        private void populatedistrict()
        {
            string district = string.Empty;
            if (dly.GetDistrict_Active().Tables[0].Rows.Count > 0)
            {
                district = dly.GetDistrict_Active().Tables[0].Rows[0][0].ToString();
                //cmbDistrict.DataSource = dly.GetDistrict().Tables[0];
                //cmbDistrict.DisplayMember = "district_name";
                //cmbDistrict.ValueMember = "district_code";
                //cmbDistrict.SelectedIndex = 0;
                
            }
            if (dly.GetROffice_Active(district).Tables[0].Rows.Count > 0)
            {
                cmbIndex2ROName.DataSource = dly.GetROffice_Active(district).Tables[0];
                cmbIndex2ROName.DisplayMember = "ra_name";
                cmbIndex2ROName.ValueMember = "ro_code";
                cmbIndex2ROName.SelectedIndex = 0;
            }
        }
        /// <summary>
        /// This will load the different types of Areas in the combo
        /// </summary>
        private void populateAreaType()
        {
        	Dictionary<string, string> dictAreaTypes = new Dictionary<string, string>() 
        	{ 
        		{"N","N/A"}, {"G","Gram Panchayat"}, {"M","Municipality"}, {"C","Corporation"} 
        	};
        	this.cmbGMC.Items.Clear();
        	this.cmbGMC.DataSource = new BindingSource(dictAreaTypes,null);
        	this.cmbGMC.DisplayMember = "Value";
        	this.cmbGMC.ValueMember = "Key";
        	this.cmbGMC.SelectedIndex = 0;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private DataTable GetDataSetWithNewField(DataSet pDs, string pName, string pCode)
        {
            DataRow newRow = pDs.Tables[0].NewRow();
            newRow[pName] = "Others";
            newRow[pCode] = "0";
            pDs.Tables[0].Rows.Add(newRow);
            return pDs.Tables[0];
        }

        private void dataGridView1_DoubleClick(object sender, EventArgs e)
        {
            //frmIGR frmig = (frmIGR)this.Owner;
            //frmig.setvalue("fffff");
            
            //this.Close();
            
        }

        private void cmdReset_Click(object sender, EventArgs e)
        {
            this.grpDisplay.Enabled = false;
            this.grpOutput.Enabled = false;
            this.cmdDone.Enabled = false;
            this.grpSearch.Enabled = true;
            this.grpAreaType.Enabled = false;
            this.grpRoad.Enabled = false;
            this.txtDistrict.Text = "";
            this.txtPS.Text = "";
            this.txtMouza.Text = "";
            this.txtJLNo.Text = "";
            this.txtDistrict.Focus();
            this.rdAllValues.Checked = true;

            grdResult.DataSource = null;
            
            this.cmbGMCValue.DataSource = null;
            this.cmbIndex2Road.DataSource = null;
            this.cmbIndex2ROName.DataSource = null;
            this.cmbPropertyType.DataSource = null;
            this.cmbLandtype.DataSource = null;
            this.txtIndex2Road.Text = string.Empty;
        }

        private void cmdSearch_Click(object sender, EventArgs e)
        {
            string dis_code = txtDistrict.Text;
            string ps_name = txtPS.Text;
            string mouza = txtMouza.Text;
            string jlNo = txtJLNo.Text;
            grdResult.DataSource = null;
            DataSet dtResults = dly.GetMappingSugg(dis_code, ps_name, mouza, jlNo);
            if (dtResults.Tables[0].Rows.Count > 0)
            {
                grdResult.DataSource = dly.GetMappingSugg(dis_code, ps_name, mouza, jlNo).Tables[0];
                this.grpDisplay.Enabled = true;
                this.grpDisplay.Focus();
                this.lblCount.Text = dtResults.Tables[0].Rows.Count.ToString() + " record(s) found";
            }
            else
            {
                MessageBox.Show("No Data Found...");
            }
            this.grdResult.Focus();
        }

        private void cmdAcceptResults_Click(object sender, EventArgs e)
        {
            GetSelectedValue();
        }
        private void GetSelectedValue()
        {
            this.grpDisplay.Enabled = false;
            this.grpSearch.Enabled = false;
            this.grpOutput.Enabled = true;
            txtOthPS.Focus();
            //cmdDone.Enabled = true;
            int index = grdResult.CurrentRow.Index;
            txtOutDistrict.Text = grdResult.CurrentRow.Cells[4].Value.ToString();
            txtOutPS.Text = grdResult.CurrentRow.Cells[5].Value.ToString();
            txtOutMouza.Text = grdResult.CurrentRow.Cells[6].Value.ToString();
            txtOutJLNo.Text = grdResult.CurrentRow.Cells[3].Value.ToString();

            txtSelDistrict.Text = grdResult.CurrentRow.Cells[0].Value.ToString();
            txtSelPS.Text = grdResult.CurrentRow.Cells[1].Value.ToString();
            txtSelMouza.Text = grdResult.CurrentRow.Cells[2].Value.ToString();
            txtSelJLNo.Text = grdResult.CurrentRow.Cells[3].Value.ToString();

            if (txtOutPS.Text == "0" )
            {
                MessageBox.Show("Wrong PS Code, select again...");
                return;
            }
        }

        private void grdResult_SelectionChanged(object sender, EventArgs e)
        {

        }

        private void txtOthPS_Enter(object sender, EventArgs e)
        {
            TextBox x = (TextBox)sender;
            x.SelectAll();
        }

        private void txtOthMouza_Enter(object sender, EventArgs e)
        {
            TextBox x = (TextBox)sender;
            x.SelectAll();
        }

        private void txtOthJLNo_TextChanged(object sender, EventArgs e)
        {
            TextBox x = (TextBox)sender;
            x.SelectAll();
        }

        private void cmdDone_Click(object sender, EventArgs e)
        {
            System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex("^[0-9]+$");
            if (!regex.IsMatch(txtOthJLNo.Text) && (txtOthJLNo.Text != null) && (txtOthJLNo.Text != ""))
            {
               // String.IsNullOrEmpty(txtOthJLNo.Text);
                MessageBox.Show("Enter only number, it may contain digits(0-9)");
                grpOutput.Enabled = true;
                txtOthJLNo.Select();
                return;
            }
            if (txtOutPS.Text == "0")
            {
                MessageBox.Show("Wrong PS Code, select again...");
                return;
            }
            if (m_CallBack != null)
            {
                m_CallBack.Invoke(txtOutDistrict.Text, txtOutPS.Text, txtOutMouza.Text, txtOutJLNo.Text, txtOthPS.Text, txtOthMouza.Text, txtOthJLNo.Text);
            }
            if (frmSource != null)
            {
                string LandType = string.Empty;
                string GMCType = string.Empty;
                string GMCValue = string.Empty;
                string Road = string.Empty;
                
                if (cmbLandtype.DataSource != null && cmbLandtype.SelectedValue != null) { LandType = cmbLandtype.SelectedValue.ToString(); }
                if (cmbGMC.SelectedValue.ToString().ToLower()!="n") 
                { 
                	GMCType = cmbGMC.SelectedValue.ToString();
                	if (cmbGMCValue.SelectedValue.ToString() != null)
                	{
                		GMCValue = cmbGMCValue.SelectedValue.ToString();
                	}
                }
                if (cmbIndex2Road.SelectedValue != null)
                {
                	Road = cmbIndex2Road.SelectedValue.ToString();
                }
                frmSource.populateSuggestedValue(txtOutDistrict.Text, txtOutPS.Text, txtOutMouza.Text, txtOutJLNo.Text, txtOthPS.Text, txtOthMouza.Text, txtOthJLNo.Text,GMCType,GMCValue,cmbIndex2ROName.SelectedValue.ToString(),Road,txtIndex2Road.Text,cmbPropertyType.SelectedValue.ToString(),LandType);
            }
            this.Close();
        }

        private void formatForm()
        {
        	populateAreaType();
            if (constants._SUGGEST)
            {
                
                this.txtDistrict.AutoCompleteCustomSource = dly.GetSuggestions("district", "district_name");
                this.txtDistrict.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                this.txtDistrict.AutoCompleteSource = AutoCompleteSource.CustomSource;

                this.txtPS.AutoCompleteCustomSource = dly.GetSuggestions("ps", "ps_name");
                this.txtPS.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                this.txtPS.AutoCompleteSource = AutoCompleteSource.CustomSource;

                this.txtMouza.AutoCompleteCustomSource = dly.GetSuggestions("moucode", "eng_mouname");
                this.txtMouza.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                this.txtMouza.AutoCompleteSource = AutoCompleteSource.CustomSource;

                this.txtJLNo.AutoCompleteCustomSource = dly.GetSuggestions("moucode", "jlno");
                this.txtJLNo.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                this.txtJLNo.AutoCompleteSource = AutoCompleteSource.CustomSource;
            }
            txtDistrict.Focus();
        }
        public static List<Control> GetControls(Control form)
        {
            var controlList = new List<Control>();

            foreach (Control childControl in form.Controls)
            {
                // Recurse child controls.
                controlList.AddRange(GetControls(childControl));
                controlList.Add(childControl);
            }
            return controlList;
        }
        private void frmSuggession_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
        }
        
        /// <summary>
        /// It's time to select Area Type
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void CmdPMJOkClick(object sender, EventArgs e)
        {
        	
            this.grpOutput.Enabled = false;
        	this.grpDisplay.Enabled = false;
        	this.grpAreaType.Enabled = true;
        	this.cmbGMC.Focus();
        }
        
        void CmdAreaTypeClick(object sender, System.EventArgs e)
        {
        	this.grpRoad.Enabled = true;
			this.grpOutput.Enabled = false;
        	this.grpDisplay.Enabled = false;
        	this.grpAreaType.Enabled = false;
			this.grpProperty.Enabled = false;
            string district = string.Empty;
            if (dly.GetDistrict_Active().Tables[0].Rows.Count > 0)
            {
                district = dly.GetDistrict_Active().Tables[0].Rows[0][0].ToString();

            }
            if (txtOutDistrict.Text.Trim() == dly.GetDistrict_Active().Tables[0].Rows[0][0].ToString())
            {
                if (dly.GetROffice_Active(district).Tables[0].Rows.Count > 0)
                {

                    cmbIndex2ROName.SelectedValue = dly.GetROffice_Active(district).Tables[0].Rows[0][0].ToString();
                }
            }
			cmbIndex2ROName.Focus();
        }
        
        private DataTable GetNewDatatable(DataTable pDt, string pDisplayColumn, string pValueColumn)
        {
            DataRow dr = pDt.NewRow();
            dr[pDisplayColumn] = "None";
            dr[pValueColumn] = "";
            pDt.Rows.Add(dr);
            return pDt;
        }
        
        void CmbGMCValueEnter(object sender, EventArgs e)
        {
        	cmbGMCValue.DataSource = null;
        	cmbGMCValue.Items.Clear();
        	cmbGMCValue.Mandatory = true;
        	if (cmbGMC.SelectedValue.ToString() == "G")
        	{
        		cmbGMCValue.DataSource = GetNewDatatable(dly.GetGramPanchayet(this.txtOutDistrict.Text.Trim(),this.txtOutPS.Text.Trim()).Tables[0],"gp_desc","gp_code");
                cmbGMCValue.DisplayMember = "gp_desc";
                cmbGMCValue.ValueMember = "gp_code";
                cmbGMCValue.SelectedIndex = 0;
        	}
        	else if (cmbGMC.SelectedValue.ToString() == "M" || cmbGMC.SelectedValue.ToString() == "C")
        	{
        		cmbGMCValue.DataSource = GetNewDatatable(dly.GetSubdivision(this.txtOutDistrict.Text.Trim()).Tables[0],"municipality_name","municipality_code");
                cmbGMCValue.DisplayMember = "municipality_name";
                cmbGMCValue.ValueMember = "municipality_code";
                cmbGMCValue.SelectedIndex = 0;
        	}
			else
			{
				cmbGMCValue.Mandatory = false;
			}
        }
        
        void CmbIndex2RoadEnter(object sender, EventArgs e)
        {
        	if(!string.IsNullOrEmpty(txtOutDistrict.Text) && cmbIndex2ROName.DataSource !=null)
        	{
        		cmbIndex2Road.DataSource = GetDataSetWithNewField(dly.GetRoad(txtOutDistrict.Text, cmbIndex2ROName.SelectedValue.ToString()), "road_name", "road_code");
				cmbIndex2Road.DisplayMember = "road_name";
	            cmbIndex2Road.ValueMember = "road_code";
            if(cmbIndex2Road.Items.Count > 0)
	        {
                    cmbIndex2Road.SelectedIndex = 0;
                /*
					AutoCompleteStringCollection x = new AutoCompleteStringCollection();
					for (int i=0; i<cmbIndex2Road.Items.Count; 	i++)
					{
						x.Add(cmbIndex2Road.Items[i].ToString());
					}
	                this.cmbIndex2Road.AutoCompleteCustomSource = x;
	                this.cmbIndex2Road.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
	                this.cmbIndex2Road.AutoCompleteSource = AutoCompleteSource.CustomSource;
                 */
	            }
        	}
            this.cmbIndex2Road.Refresh();
            this.cmbIndex2Road.Show();
            cmbIndex2Road.Mandatory = true;
        }
        
        void CmbIndex2RONameEnter(object sender, EventArgs e)
        {
        	if(!string.IsNullOrEmpty(txtOutDistrict.Text))
        	{
        		cmbIndex2ROName.DataSource = dly.GetROffice(txtOutDistrict.Text).Tables[0];
                cmbIndex2ROName.DisplayMember = "RO_name";
                cmbIndex2ROName.ValueMember = "RO_code";
        	}
        }
        
        void CmbIndex2RoadLeave(object sender, EventArgs e)
        {
        	if(cmbIndex2Road.Text.ToString() == "Others")
        	{
        		txtIndex2Road.Enabled = true;
                txtIndex2Road.Focus();
                //txtIndex2Road.Mandatory = true;
        	}
        	else
        	{
        		txtIndex2Road.Enabled = false;
        	}
        }
        
        void cmdRORoadClick(object sender, EventArgs e)
        {
        	this.grpProperty.Enabled = true;
			this.grpRoad.Enabled = false;
			this.grpOutput.Enabled = false;
        	this.grpDisplay.Enabled = false;
        	this.grpAreaType.Enabled = false;
			cmbPropertyType.Focus();
        }
        
        void CmbPropertyTypeEnter(object sender, EventArgs e)
        {
        	cmbPropertyType.DataSource = dly.GetPropertyType().Tables[0];
			cmbPropertyType.DisplayMember = "description";
            cmbPropertyType.ValueMember = "apartment_type_code";
            cmbPropertyType.SelectedIndex = 0;
            cmbPropertyType.Mandatory = true;
        }
        
        void CmbLandtypeEnter(object sender, EventArgs e)
        {
            cmbLandtype.Mandatory = false;
        	if (cmbPropertyType.SelectedValue != null)
            {
                if (cmbPropertyType.SelectedValue.ToString() == "LL")
                {
                    cmbLandtype.Enabled = true;
                    cmbLandtype.DataSource = null;
                    DataTable ds = new DataTable();
                    ds = dly.Getland_type(txtOutDistrict.Text, cmbIndex2ROName.SelectedValue.ToString());
                    if (ds.Rows.Count > 0)
                    {
                        cmbLandtype.DataSource = ds;
                        cmbLandtype.DisplayMember = ds.Columns[1].ToString();
                        cmbLandtype.ValueMember = ds.Columns[0].ToString();
                        cmbLandtype.Mandatory = true;
                    }
                }
                else
                {
                    cmbLandtype.DataSource = null;
                    cmbLandtype.Enabled = false;
                }
            }
        }
        private void cmdProperty_Click(object sender, EventArgs e)
        {
            this.grpProperty.Enabled = false;
            this.grpRoad.Enabled = false;
            this.grpOutput.Enabled = false;
            this.grpDisplay.Enabled = false;
            this.grpAreaType.Enabled = false;
            cmdDone.Enabled = true;
            cmdDone.Focus();
        }

        private void grdResult_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if(e.RowIndex >=0)
                GetSelectedValue();
        }
        
        void CmbIndex2RoadSelectedIndexChanged(object sender, EventArgs e)
        {
        	
        }
        
        void CmbPropertyTypeLeave(object sender, EventArgs e)
        {
            /*
        	cmbLandtype.Items.Clear();
        	cmbLandtype.DataSource = null;
        	if (cmbPropertyType.SelectedValue.ToString() == "LL")
        	{
        		cmbLandtype.Enabled = true;
        		cmbLandtype.Focus();
        	}
        	else
        	{
        		cmbLandtype.Enabled = false;
        	}
             */
        }

        private void txtOutMouza_TextChanged(object sender, EventArgs e)
        {

        }
    }

}
