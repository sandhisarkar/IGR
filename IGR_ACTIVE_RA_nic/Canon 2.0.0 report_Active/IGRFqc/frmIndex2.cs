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
using LandAreaConversion;
using valUtils;

namespace IGRFqc
{
	public partial class frmIndex2 : Form
	{
		private PopulateCombo pCom = null;
		double acreForeach = 0;
		double sqFeetStructure = 0;
		double BighaForeach = 0;
		double KathaForeach = 0;
		double ChatakForeach = 0;
        double decimalforeach = 0;
        double sqftLandForeach = 0;
        double sqftStructForeach = 0;
		bool land = false;
		string setDistrict = null;
		string setPs = null;
		string setMouza = null;
		string setjlno = null;
		string setOtherPs = null;
		string setOtherMouza = null;
		string setOtherJlno = null;
		int no_of_split = 0;
		Mode _isEditing;
		bool PartEntry = false;
		Credentials crd = new Credentials();
		private OdbcConnection sqlCon = null;
		OdbcTransaction txn;
		List<string> pList = new List<string >();
		List<string> kList = new List<string>();
		PropertyDetails property;
		List<PropertyDetails_other_plot> OtherPlot = null;
		List<PropertyDetails_other_khatian> OtherKhatian = null;
        List<PropertyDetailsException> propertyExp = null;
		//bool RefreshGrid = false;
        int pCount=-1;
		public delegate void OnAccept(PropertyDetails person);
		OnAccept m_OnAccept;
		//Parameter to keep a track of whether the value being edited is part of a deed which is on hold
		string isOnHold = "N";
		string Exception = string.Empty;
		//The method to be invoked when the user aborts all operations
		public delegate void OnAbort();
		OnAbort m_OnAbort;

        public frmIndex2(OdbcConnection prmCon, OdbcTransaction pTxn, Credentials prmCrd, ref PropertyDetails pPropperty, Mode p_IsEditing, OnAccept pOnAccept, OnAbort pOnAbort, List<PropertyDetails_other_plot> pPlot, List<PropertyDetails_other_khatian> pKhtiann, string pisOnHold, List<PropertyDetailsException> exp, int mCount)
		{
			InitializeComponent();
			sqlCon = prmCon;
			txn = pTxn;
			crd = prmCrd;
			property = pPropperty;
			OtherPlot = pPlot;
			OtherKhatian = pKhtiann;
            propertyExp = exp;
            pCount = mCount;
			pCom = new PopulateCombo(sqlCon,txn,crd);
			_isEditing = p_IsEditing;
			m_OnAccept = pOnAccept;
			m_OnAbort = pOnAbort;
			property_init(property);
			formatForm();
			isOnHold = pisOnHold;
		}
        public frmIndex2(OdbcConnection prmCon, OdbcTransaction pTxn, Credentials prmCrd, ref PropertyDetails pPropperty, Mode p_IsEditing, OnAccept pOnAccept, OnAbort pOnAbort, List<PropertyDetails_other_plot> pPlot, List<PropertyDetails_other_khatian> pKhtiann, string pisOnHold, List<PropertyDetailsException> exp)
		{
			InitializeComponent();
			sqlCon = prmCon;
			txn = pTxn;
			crd = prmCrd;
            property = pPropperty;
            propertyExp = exp;
			OtherPlot = pPlot;
			OtherKhatian = pKhtiann;
            //pCount = -1;
			pCom = new PopulateCombo(sqlCon,txn,crd);
			_isEditing = p_IsEditing;
			m_OnAccept = pOnAccept;
			m_OnAbort = pOnAbort;
			property_init(property);
			formatForm();
			isOnHold = pisOnHold;
		}
        public void GetResultsFromAnadi(int pNoSplits, double pAcre, double pBigha, double pKatha, double pChhatak, bool pLand, double pSqFeet, double pDecimal, double psqftLand, double psqftStruct)
		{
			acreForeach = pAcre;
			BighaForeach = pBigha;
			KathaForeach = pKatha;
			ChatakForeach = pChhatak;
			no_of_split = pNoSplits;
			sqFeetStructure = pSqFeet;
            decimalforeach = pDecimal;
            sqftLandForeach = psqftLand;
            sqftStructForeach = psqftStruct;
			land = pLand;

            double fstTab = acreForeach + BighaForeach + KathaForeach + ChatakForeach + decimalforeach + sqftLandForeach + sqftStructForeach;
            if (fstTab > 0 && sqFeetStructure > 0)
            {
                DialogResult dialogResult = MessageBox.Show("Do you want to accept both values?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2);
                if (dialogResult == DialogResult.Yes)
                {
                    PopulateSplitVal();
                }
                else
                {
                    //LandAreaConversion.frmConverter frm = new frmConverter(GetResultsFromAnadi);
                    //frm.ShowDialog();
                    //frm.Activate();
                    //if (no_of_split > 1)
                    //{
                    //    PartEntry = true;
                    //}
                    //else
                    //{
                    //    PartEntry = false;
                    //}
                    //SendKeys.Send("{Tab}");

                    txtAcre.Text = "";
                    txtbigha.Text = "";
                    txtDecimal.Text = "";
                    txtKatha.Text = "";
                    txtChatak.Text = "";
                    txtSqfeetL.Text = "";
                    txtSqfeetStructure.Text = "";


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

            //PopulateSplitVal();
		}
		private void PopulateSplitVal()
		{
            if (acreForeach.ToString() != "0")
            {
                rdoAcre.Checked = true;
                txtAcre.Enabled = true;
            }
            else
            {
                rdoAcre.Checked = false;
                txtAcre.Enabled = false;
            }
			if (BighaForeach.ToString() != "0")
			{
				rdoBigha.Checked = true;
				txtbigha.Enabled = true;
			}
            else
            {
                rdoBigha.Checked = false;
                txtbigha.Enabled = false;
            }
			if (KathaForeach.ToString() != "0")
			{
				rdoKatha.Checked = true;
				txtKatha.Enabled = true;
			}
            else
            {
                rdoKatha.Checked = false;
                txtKatha.Enabled = false;
            }
			if (ChatakForeach.ToString() != "0")
			{
				rdoChatak.Checked = true;
				txtChatak.Enabled = true;
			}
            else
            {
                rdoChatak.Checked = false;
                txtChatak.Enabled = false;
            }
            if (decimalforeach.ToString() != "0")
            {
                rdoDecimal.Checked = true;
                txtDecimal.Enabled = true;
            }
            else
            {
                rdoDecimal.Checked = false;
                txtDecimal.Enabled = false;
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
            if (sqFeetStructure.ToString() != "0")
            {
                if (!land)
                {
                    rdoStructureSqFeet.Checked = true;
                    txtSqfeetStructure.Enabled = true;
                    txtSqfeetStructure.Text = sqFeetStructure.ToString();
                    roSqFeet.Checked = false;
                    roSqFeet.Enabled = false;
                    txtSqfeetL.Text = "0";
                }
                else
                {
                    roSqFeet.Checked = true;
                    txtSqfeetL.Enabled = true;
                    txtSqfeetL.Text = sqFeetStructure.ToString();
                    rdoStructureSqFeet.Checked = false;
                    txtSqfeetStructure.Enabled = false;
                    txtSqfeetStructure.Text = "0";
                }
            }
            else
            {
                //roSqFeet.Checked = false;
                //txtSqfeetL.Text = "0";
                //rdoStructureSqFeet.Checked = false;
                //txtSqfeetStructure.Text = "0";
                txtSqfeetL.Text = sqftLandForeach.ToString();
                txtSqfeetStructure.Text = sqftStructForeach.ToString();
            }
			txtAcre.Text = acreForeach.ToString();
			txtbigha.Text = BighaForeach.ToString();
			txtKatha.Text = KathaForeach.ToString();
			txtChatak.Text = ChatakForeach.ToString();
            txtDecimal.Text = decimalforeach.ToString();
		}
		private void property_init(PropertyDetails mproperty)
		{
			if (_isEditing == Mode._Edit)
			{
				cmbIndex2District.DataSource = pCom.GetDistrict().Tables[0];
				cmbIndex2District.DisplayMember = "district_name";
				cmbIndex2District.ValueMember = "district_code";

				if (cmbIndex2District.Items.Count > 1)
				{
					cmbIndex2District.SelectedValue = mproperty.Property_district_code;
				}
				cmbIndex2ROName.DataSource = pCom.GetROffice(cmbIndex2District.SelectedValue.ToString()).Tables[0];
				cmbIndex2ROName.DisplayMember = "RO_name";
				cmbIndex2ROName.ValueMember = "RO_code";
				if (cmbIndex2ROName.Items.Count > 1)
				{
					cmbIndex2ROName.SelectedValue = mproperty.Property_ro_code;
				}
				
				
				if (cmbIndex2District.DataSource != null)
				{

					if (mproperty.Property_district_code != null)
					{
						string districtCode = cmbIndex2District.SelectedValue.ToString();
						cmbIndex2PS.DataSource = GetDataSetWithNewField(pCom.GetPS(districtCode), "PS_name", "PS_code");
						cmbIndex2PS.DisplayMember = "PS_name";
						cmbIndex2PS.ValueMember = "PS_code";
						cmbIndex2PS.SelectedValue = mproperty.ps_code.PadLeft(2, '0').ToString();
					}
					if (cmbIndex2District.SelectedValue != null && (cmbIndex2PS.SelectedValue != null))
					{
						string districtCode = cmbIndex2District.SelectedValue.ToString();
						string pscode = cmbIndex2PS.SelectedValue.ToString();
						cmbMouza.DataSource = pCom.GetMouza(districtCode, pscode).Tables[0];
						cmbMouza.DisplayMember = "eng_mouname";
						cmbMouza.ValueMember = "moucode";
						if (cmbMouza.Items.Count > 0)
							cmbMouza.SelectedIndex = 0;
						//

						cmbJL.DataSource = pCom.GetJL(districtCode, pscode).Tables[0];
						cmbJL.DisplayMember = "jlno";
						cmbJL.ValueMember = "jlno";
						if (cmbJL.Items.Count > 0)
							cmbJL.SelectedIndex = 0;
					}

					cmbPropertyType.DataSource = pCom.GetPropertyType().Tables[0];
					cmbPropertyType.DisplayMember = "description";
					cmbPropertyType.ValueMember = "apartment_type_code";
					cmbPropertyType.SelectedIndex = 0;
					txtRefJL.Text = mproperty.Ref_JL_Number;
					if (mproperty.Ref_ps != null)
					{
						txtRefPS.Text = mproperty.Ref_ps;
					}
					if (mproperty.Ward != null)
					{
						txtWard.Text = mproperty.Ward;
					}
					if (mproperty.Holding != null)
						txtHolding.Text = mproperty.Holding;

					cmbIndex2ROName_Leave(this, null);
					if (string.IsNullOrEmpty(mproperty.road_code)) { cmbIndex2Road.Text = "Others"; txtIndex2Road.Text = mproperty.Road; txtIndex2Road.Enabled = true; }
					else { cmbIndex2Road.SelectedValue = mproperty.road_code; }

					txtIndex2Premises.Text = mproperty.Premises;
					if (!string.IsNullOrEmpty(mproperty.moucode))
					{ cmbMouza.SelectedValue = mproperty.moucode; }
					cmbPlotCode.SelectedText = mproperty.Plot_code_type;
					txtPlotNumer.Text = mproperty.Plot_No;
					txtKhatianNumber.Text = mproperty.khatian_No;
					txtBataNumber.Text = mproperty.Bata_No;
					txtKhatianBataNNumber.Text = mproperty.bata_khatian_no;

					cmbKhatianType.Text = mproperty.Khatian_type;
					cmbPropertyType.SelectedValue = mproperty.property_type;
					if (cmbPropertyType.Text == "Land")
					{
						cmbLandtype.Enabled = false;
						DataTable ds = new DataTable();
						ds = pCom.Getland_type(cmbIndex2District.SelectedValue.ToString(), cmbIndex2ROName.SelectedValue.ToString());
						if (ds.Rows.Count > 0 && mproperty.land_type != null)
						{
							cmbLandtype.DataSource = ds;
							cmbLandtype.DisplayMember = ds.Columns[1].ToString();
							cmbLandtype.ValueMember = ds.Columns[0].ToString();
							cmbLandtype.SelectedValue = mproperty.land_type;
						}
					}
					cmbPlotCode.Text = mproperty.Plot_code_type;
					txtRefmou.Text = mproperty.Ref_mou;
					txtKhatianBataNNumber.Text = mproperty.bata_khatian_no;
					cmbKhatianType.Text = mproperty.Khatian_type;
					txtRefPS.Text = mproperty.Ref_ps;
					if (mproperty.JL_NO != null)
					{
						cmbJL.SelectedValue = mproperty.JL_NO;
					}


				}
				rdaNA.Checked = true;
				switch (mproperty.Area_type)
				{
					case "G":
						{
							rdoGP.Checked = true;
							rdoGP_Click(this.rdoGP, null);
							break;
						}
					case "M":
						{
							rdoMuni.Checked = true;
							rdoMuni_Click(this.rdoMuni, null);
							break;
						}
					case "C":
						{
							rdoCorp.Checked = true;
							rdoCorp_Click(this.rdoCorp, null);
							break;
						}
				}
				cmbAreaType.SelectedValue = mproperty.GP_Muni_Corp_Code;
				if (mproperty.Land_Area_acre != "0") { rdoAcre.Checked = true; txtAcre.Enabled = true; txtAcre.Text = mproperty.Land_Area_acre; }
				if (mproperty.Land_Area_bigha != "0") { rdoBigha.Checked = true; txtbigha.Enabled = true; txtbigha.Text = mproperty.Land_Area_bigha; }
				if (mproperty.Land_Area_chatak != "0") { rdoChatak.Checked = true; txtChatak.Enabled = true; txtChatak.Text = mproperty.Land_Area_chatak; }
				if (mproperty.Land_Area_decimal != "0") { rdoDecimal.Checked = true; txtDecimal.Enabled = true; txtDecimal.Text = mproperty.Land_Area_decimal; }
				if (mproperty.Land_Area_katha != "0") { rdoKatha.Checked = true; txtKatha.Enabled = true; txtKatha.Text = mproperty.Land_Area_katha; }
				if (mproperty.Land_Area_sqfeet != "0") { roSqFeet.Checked = true; txtSqfeetL.Enabled = true; txtSqfeetL.Text = mproperty.Land_Area_sqfeet; }
				if (mproperty.Structure_area_in_sqFeet != "0")
				{
					rdoStructureSqFeet.Checked = true;
					txtSqfeetStructure.Enabled = true;
					txtSqfeetStructure.Text = mproperty.Structure_area_in_sqFeet;
				}
				if (mproperty.property_type == "FL" || mproperty.property_type=="CM")
				{
					btnOtherPlots.Enabled = true;
					btnOtherKhatian.Enabled = true;
				}
			}
			else
			{
				cmbIndex2District.DataSource = pCom.GetDistrict().Tables[0];
				cmbIndex2District.DisplayMember = "district_name";
				cmbIndex2District.ValueMember = "district_code";
				//cmbIndex2District.SelectedIndex = 0;
				if (pCom.GetDistrict().Tables[0].Rows.Count > 0)
				{
					cmbIndex2District.SelectedValue = "99";
				}

				string districtCode = cmbIndex2District.SelectedValue.ToString();
				cmbIndex2PS.DataSource = pCom.GetPS(districtCode).Tables[0];
				cmbIndex2PS.DisplayMember = "PS_name";
				cmbIndex2PS.ValueMember = "PS_code";
				if (pCom.GetPS(districtCode).Tables[0].Rows.Count > 0)
				{
					cmbIndex2PS.SelectedValue = pCom.GetPS(districtCode).Tables[0].Rows[0][0].ToString();
				}
				cmbIndex2ROName.DataSource = pCom.GetROffice(cmbIndex2District.SelectedValue.ToString()).Tables[0];
				cmbIndex2ROName.DisplayMember = "RO_name";
				cmbIndex2ROName.ValueMember = "RO_code";
				cmbIndex2ROName.SelectedIndex = 0;
				cmbPlotCode.SelectedIndex = 0;
				cmbKhatianType.SelectedIndex = 0;
				cmbPropertyType.DataSource = pCom.GetPropertyType().Tables[0];
				cmbPropertyType.DisplayMember = "description";
				cmbPropertyType.ValueMember = "apartment_type_code";
				cmbPropertyType.SelectedIndex = 0;
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
		private DataTable GetNewDatatable(DataTable pDt, string pDisplayColumn, string pValueColumn)
		{
			DataRow dr = pDt.NewRow();
			dr[pDisplayColumn] = "None";
			dr[pValueColumn] = "";
			pDt.Rows.Add(dr);
			return pDt;
		}
		private void rdoGP_Click(object sender, EventArgs e)
		{
			if (rdoGP.Checked == true)
			{
				if ((cmbIndex2District.SelectedValue != null) && (cmbIndex2PS.SelectedValue != null))
				{
					string dist = cmbIndex2District.SelectedValue.ToString();
					string ps = cmbIndex2PS.SelectedValue.ToString();
					cmbAreaType.DataSource = GetNewDatatable(pCom.GetGramPanchayet(dist, ps).Tables[0], "gp_desc", "gp_code");
					cmbAreaType.DisplayMember = "gp_desc";
					cmbAreaType.ValueMember = "gp_code";
					if (cmbAreaType.Items.Count > 0)
						cmbAreaType.SelectedIndex = 0;
					//if (constants._SUGGEST == true)
					//{
					//    this.cmbAreaType.AutoCompleteCustomSource = pCom.GetSuggestions("gram_panchayat", "gp_desc");
					//    this.cmbAreaType.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
					//    this.cmbAreaType.AutoCompleteSource = AutoCompleteSource.CustomSource;
					//}
				}
			}
		}

		private void rdoMuni_Click(object sender, EventArgs e)
		{
			if (rdoMuni.Checked == true)
			{
				if ((cmbIndex2District.DataSource != null))
				{
					string dist = cmbIndex2District.SelectedValue.ToString();
					cmbAreaType.DataSource = GetNewDatatable(pCom.GetSubdivision(dist).Tables[0], "municipality_name", "municipality_code");
					cmbAreaType.DisplayMember = "municipality_name";
					cmbAreaType.ValueMember = "municipality_code";
					if (cmbAreaType.Items.Count > 0)
						cmbAreaType.SelectedIndex = 0;
					//if (constants._SUGGEST == true)
					//{
					//    this.cmbAreaType.AutoCompleteCustomSource = pCom.GetSuggestions("municipality", "municipality_name");
					//    this.cmbAreaType.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
					//    this.cmbAreaType.AutoCompleteSource = AutoCompleteSource.CustomSource;
					//}
				}
			}
		}

		private void rdoCorp_Click(object sender, EventArgs e)
		{
			if (rdoCorp.Checked == true)
			{
				if ((cmbIndex2District.DataSource != null))
				{
					string dist = cmbIndex2District.SelectedValue.ToString();
					cmbAreaType.DataSource = GetNewDatatable(pCom.GetSubdivision(dist).Tables[0], "municipality_name", "municipality_code");
					cmbAreaType.DisplayMember = "municipality_name";
					cmbAreaType.ValueMember = "municipality_code";
					if (cmbAreaType.Items.Count > 0)
						cmbAreaType.SelectedIndex = 0;

					//if (constants._SUGGEST == true)
					//{
					//    this.cmbAreaType.AutoCompleteCustomSource = pCom.GetSuggestions("municipality", "municipality_name");
					//    this.cmbAreaType.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
					//    this.cmbAreaType.AutoCompleteSource = AutoCompleteSource.CustomSource;
					//}
				}
			}
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
			SendKeys.Send("{Tab}");
		}
		public void populateSuggestedValue(string psetDistrict, string psetPs, string psetMouza, string psetjlno, string psetOtherPs, string psetOtherMouza, string psetOtherJlno,string pAreaType,string pAreaTypeValue,string pRO,string pRoad,string pOtherRoad,string pPropertyType,string pLandType)
		{
			setDistrict = psetDistrict;
			setPs = psetPs;
			setMouza = psetMouza;
			setjlno = psetjlno;
			setOtherPs = psetOtherPs;
			setOtherMouza = psetOtherMouza;
			setOtherJlno = psetOtherJlno;

            if (setPs == "0")
            {
                MessageBox.Show("PS Error! Please select again");
                return;
            }

			cmbIndex2District.SelectedValue = setDistrict.ToString();

			if (cmbIndex2District.DataSource != null)
			{
				cmbIndex2ROName.DataSource = pCom.GetROffice(setDistrict).Tables[0];
				cmbIndex2ROName.DisplayMember = "RO_name";
				cmbIndex2ROName.ValueMember = "RO_code";
				cmbIndex2ROName.SelectedValue = pRO;

				cmbIndex2PS.DataSource = pCom.GetPS(setDistrict).Tables[0];
				cmbIndex2PS.DisplayMember = "ps_name";
				cmbIndex2PS.ValueMember = "ps_code";
				cmbIndex2PS.SelectedValue = setPs;

				if (pAreaType == "G")
				{
					rdoGP.Checked = true;
					cmbAreaType.DataSource = GetNewDatatable(pCom.GetGramPanchayet(setDistrict, setPs).Tables[0], "gp_desc", "gp_code");
					cmbAreaType.DisplayMember = "gp_desc";
					cmbAreaType.ValueMember = "gp_code";
					cmbAreaType.SelectedIndex = 0;
					cmbAreaType.SelectedValue = pAreaTypeValue;
				}
				else if (pAreaType == "M" || pAreaType == "C")
				{
					if (pAreaType == "M") { rdoMuni.Checked = true; }
					if (pAreaType == "C") { rdoCorp.Checked = true; }
					cmbAreaType.DataSource = GetNewDatatable(pCom.GetSubdivision(setDistrict).Tables[0], "municipality_name", "municipality_code");
					cmbAreaType.DisplayMember = "municipality_name";
					cmbAreaType.ValueMember = "municipality_code";
					cmbAreaType.SelectedIndex = 0;
					cmbAreaType.SelectedValue = pAreaTypeValue;
				}
				else { rdaNA.Checked = true; }

				cmbJL.DataSource = pCom.GetJLNobyDistrictPS(setDistrict, setPs).Tables[0];
				cmbJL.DisplayMember = "jlno";
				cmbJL.ValueMember = "jlno";
				cmbJL.SelectedValue = setjlno;

				cmbMouza.DataSource = pCom.GetMouza(setDistrict, setPs).Tables[0];
				cmbMouza.DisplayMember = "eng_mouname";
				cmbMouza.ValueMember = "moucode";

				cmbMouza.SelectedValue = setMouza;

				if (cmbIndex2ROName.DataSource != null)
				{
					cmbIndex2Road.DataSource = GetDataSetWithNewField(pCom.GetRoad(setDistrict, pRO), "road_name", "road_code");
					cmbIndex2Road.DisplayMember = "road_name";
					cmbIndex2Road.ValueMember = "road_code";
					if (cmbIndex2Road.Items.Count > 0)
						cmbIndex2Road.SelectedValue = pRoad;
				}
				txtIndex2Road.Text = pOtherRoad;

				cmbPropertyType.DataSource = pCom.GetPropertyType().Tables[0];
				cmbPropertyType.DisplayMember = "description";
				cmbPropertyType.ValueMember = "apartment_type_code";
				cmbPropertyType.SelectedValue = pPropertyType;

				btnOtherPlots.Enabled = false;
				btnOtherKhatian.Enabled = false;
				if (cmbPropertyType.SelectedValue != null)
				{
					if (cmbPropertyType.SelectedValue.ToString() == "LL")
					{
						cmbLandtype.DataSource = null;
						cmbLandtype.Items.Clear();
						DataTable ds = new DataTable();
						ds = pCom.Getland_type(setDistrict, pRO);
						if (ds.Rows.Count > 0)
						{
							cmbLandtype.DataSource = ds;
							cmbLandtype.DisplayMember = ds.Columns[1].ToString();
							cmbLandtype.ValueMember = ds.Columns[0].ToString();
							cmbLandtype.SelectedValue = pLandType;
						}
					}
					else
					{
						cmbLandtype.DataSource = null;
						cmbLandtype.Enabled = false;
					}
					if (cmbPropertyType.SelectedValue.ToString() == "FL" || cmbPropertyType.SelectedValue.ToString() == "CM")
					{
						btnOtherPlots.Enabled = true;
						btnOtherKhatian.Enabled = true;
					}
				}
			}
			txtRefJL.Text = setOtherJlno;
			txtRefmou.Text = setOtherMouza;
			txtRefPS.Text = setOtherPs;
			this.groupBox7.Focus();
			this.txtWard.Focus();
		}

		private void cmdjuri_Click(object sender, EventArgs e)
		{
			frmSuggession sugg;
			if ((_isEditing == Mode._Add) && (property.Property_district_code != null))
			{
				sugg = new frmSuggession(sqlCon, txn, crd, this, property, true);
			}
			else
			{
				sugg = new frmSuggession(sqlCon, txn, crd, this, property,false);
			}
			
			sugg.ShowDialog(this);
            
                groupBox7.Focus();
                txtWard.Focus();

		}

		private void cmdClose_Click(object sender, EventArgs e)
		{
			this.Close();
		}

		private void btnOtherPlots_Click(object sender, EventArgs e)
		{
			pList.Clear();
			for (int i = 0; i < OtherPlot.Count; i++)
			{
				pList.Add(OtherPlot[i].other_plot_no);
			}
			frmEntPlotKhaitan frm = new frmEntPlotKhaitan(getPlot, pList);
			frm.ShowDialog();
			frm.Activate();
			SendKeys.Send("{Tab}");
		}

		private void btnOtherKhatian_Click(object sender, EventArgs e)
		{
			kList.Clear();
			for (int i = 0; i < OtherKhatian.Count; i++)
			{
				kList.Add(OtherKhatian[i].other_Khatian_no);
			}
			frmEntPlotKhaitan frm = new frmEntPlotKhaitan(getKhatian, kList );
			frm.ShowDialog();
			frm.Activate();
			SendKeys.Send("{Tab}");
		}

		public void getPlot(List<string> pList)
		{
			OtherPlot.Clear();
			for (int i = 0; i < pList.Count; i++)
			{
				PropertyDetails_other_plot plot = new PropertyDetails_other_plot();
				plot.district_code = property.district_code;
				plot.RO_code = property.RO_code;
				plot.Book = property.Book;
				plot.Deed_no = property.Deed_no;
				plot.item_no = i.ToString();
				plot.Deed_year = property.Deed_year;
				plot.other_plot_no = pList[i].ToString();
				OtherPlot.Add(plot);
			}
		}
		public void getKhatian(List<string> klist)
		{
			OtherKhatian.Clear();
			for (int i = 0; i < klist.Count; i++)
			{
				PropertyDetails_other_khatian plot = new PropertyDetails_other_khatian();
				plot.district_code = property.district_code;
				plot.RO_code = property.RO_code;
				plot.Book = property.Book;
				plot.Deed_no = property.Deed_no;
				plot.Deed_year = property.Deed_year;
				plot.item_no = i.ToString();
				plot.other_Khatian_no = klist[i].ToString();
				OtherKhatian.Add(plot);
			}
		}
		private void InsertOtherKhatian()
		{
		}
		private Boolean ValidateInputData()
		{
			bool flag = true;

            if (cmbPlotCode.Text != "LR" && cmbPlotCode.Text != "CS" && cmbPlotCode.Text != "RS" )
            {
                MessageBox.Show(this, "Enter right Plot Type","Error", MessageBoxButtons.OK,MessageBoxIcon.Stop);
                this.cmbPlotCode.Focus();
                flag = false;
            }

            if (cmbKhatianType.Text != "LR" && cmbKhatianType.Text != "CS" && cmbKhatianType.Text != "RS")
            {
                MessageBox.Show(this, "Enter right Khatian Type", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                this.cmbKhatianType.Focus();
                flag = false;
            }

			if (txtIndex2Premises.Text.Length == 0 && txtPlotNumer.Text.Length == 0)
			{
				if (isOnHold != "Y")
				{
					MessageBox.Show(this, "Enter either Plot No/Premises No", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
					this.txtPlotNumer.Focus();
					flag = false;
				}
			}
			if (txtPlotNumer.Text.Trim().Length > 0)
			{
				string msg = DeedValidation.ChkPlotNumber(txtPlotNumer.Text.Trim());
				if (msg.Trim().Length > 0)
				{
					MessageBox.Show(this, msg, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
					txtPlotNumer.Focus();
					flag = false;
				}

			}
            if (cmbIndex2PS.Text.Trim() == null || cmbIndex2PS.Text.Trim() =="")
            {
                MessageBox.Show("PS Code Error! Select Again..", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                cmdjuri.Focus();
                flag = false;
            }
			
			if (txtBataNumber.Text.Trim().Length > 0)
			{
				string msg = DeedValidation.ChkNumberWithSymbols(txtBataNumber.Text.Trim());
				if (msg.Trim().Length > 0)
				{
					MessageBox.Show(this, msg, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
					txtBataNumber.Focus();
					flag = false;
				}
			}

			if (txtKhatianNumber.Text.Trim().Length > 0)
			{
				string msg = DeedValidation.ChkEmptyOrNumber(txtKhatianNumber.Text.Trim());
				if (msg.Trim().Length > 0)
				{
					MessageBox.Show(this, msg, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
					txtKhatianNumber.Focus();
					flag = false;
				}
			}

			if (txtKhatianBataNNumber.Text.Trim().Length > 0)
			{
				string msg = DeedValidation.ChkEmptyOrNumber(txtKhatianBataNNumber.Text.Trim());
				if (msg.Trim().Length > 0)
				{
					MessageBox.Show(this, msg, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
					txtKhatianBataNNumber.Focus();
					flag = false;
				}
			}

			if (txtWard.Text.Trim().Length > 0)
			{
				string msg = DeedValidation.ChkEmptyOrNumber(txtWard.Text.Trim());
				if (msg.Trim().Length > 0)
				{
					MessageBox.Show(this, msg, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
					txtWard.Focus();
					flag = false;
				}
			}
			if (txtHolding.Text.Trim().Length > 0)
			{
				string msg = DeedValidation.ChkEmptyOrMax50Digits(txtWard.Text.Trim());
				if (msg.Trim().Length > 0)
				{
					MessageBox.Show(this, msg, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
					txtHolding.Focus();
					flag = false;
				}
			}
            if (txtBataNumber.Text.Trim().Length > 0)
            {
                string msg = DeedValidation.ChkEmptyOrNumber(txtBataNumber.Text.Trim());
                if (msg.Trim().Length > 0)
                {
                    MessageBox.Show(this, msg, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtBataNumber.Focus();
                    flag = false;
                }
            }
            if (txtAcre.Text.Trim().Length > 0)
            {
                string msg = DeedValidation.IsDecimal(txtAcre.Text.Trim());
                if (msg.Trim().Length > 0)
                {
                    MessageBox.Show(this, msg, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtAcre.Focus();
                    flag = false;
                }
            }
            if (txtbigha.Text.Trim().Length > 0)
            {
                string msg = DeedValidation.IsDecimal(txtbigha.Text.Trim());
                if (msg.Trim().Length > 0)
                {
                    MessageBox.Show(this, msg, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtbigha.Focus();
                    flag = false;
                }
            }
            if (txtDecimal.Text.Trim().Length > 0)
            {
                string msg = DeedValidation.IsDecimal(txtDecimal.Text.Trim());
                if (msg.Trim().Length > 0)
                {
                    MessageBox.Show(this, msg, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtDecimal.Focus();
                    flag = false;
                }
            }
            if (txtKatha.Text.Trim().Length > 0)
            {
                string msg = DeedValidation.IsDecimal(txtKatha.Text.Trim());
                if (msg.Trim().Length > 0)
                {
                    MessageBox.Show(this, msg, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtKatha.Focus();
                    flag = false;
                }
            }
            if (txtChatak.Text.Trim().Length > 0)
            {
                string msg = DeedValidation.IsDecimal(txtChatak.Text.Trim());
                if (msg.Trim().Length > 0)
                {
                    MessageBox.Show(this, msg, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtChatak.Focus();
                    flag = false;
                }
            }
            if (txtSqfeetL.Text.Trim().Length > 0)
            {
                string msg = DeedValidation.IsDecimal(txtSqfeetL.Text.Trim());
                if (msg.Trim().Length > 0)
                {
                    MessageBox.Show(this, msg, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtSqfeetL.Focus();
                    flag = false;
                }
            }
            if (txtSqfeetStructure.Text.Trim().Length > 0)
            {
                string msg = DeedValidation.IsDecimal(txtSqfeetStructure.Text.Trim());
                if (msg.Trim().Length > 0)
                {
                    MessageBox.Show(this, msg, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtSqfeetStructure.Focus();
                    flag = false;
                }
            }
			
			return flag;
		}
		private void cmdSave_Click(object sender, EventArgs e)
		{
            try
            {
                if (_isEditing == Mode._Edit)
                {
                    foreach (var x in propertyExp.ToList())
                    {
                        if (x.item_no.ToString().Equals(property.Serial))
                        {
                            int idx = propertyExp.FindIndex(a => a.item_no.ToString().Equals(property.Serial));
                            propertyExp.RemoveAt(idx);

                        }

                    }
                    propertyExp.ToList();
                    //propertyExp.Clear();
                }
                if (ValidateInputData())
                {
                    double mesurement = 0;
                    if (_isEditing == Mode._Add)
                    {

                        property.Serial = pCount.ToString();

                    }

                    if (cmbIndex2District.SelectedValue.ToString() == "99" && cmbIndex2ROName.SelectedValue.ToString() == "99" && cmbIndex2PS.SelectedValue.ToString() == "999")
                    {

                        DialogResult dialogResult = MessageBox.Show("Property Missing, Do you want to Add Exception...?", "Morut Data Entry...", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                        if (dialogResult == DialogResult.Yes)
                        {
                            PropertyDetailsException pExp = new PropertyDetailsException();
                            pExp.district_code = property.district_code;
                            pExp.RO_code = property.RO_code;
                            pExp.Book = property.Book;
                            pExp.Deed_year = property.Deed_year;
                            pExp.Deed_no = property.Deed_no;
                            pExp.item_no = property.Serial;
                            pExp.exception = constants.Property_Missing;
                            pExp.excDetails = pCom.GetException(constants.Property_Missing).Tables[0].Rows[0][0].ToString();
                            propertyExp.Add(pExp);
                        }

                    }
                    if (txtIndex2Premises.Text == "0" || txtPlotNumer.Text == "0")
                    {

                        DialogResult dialogResult = MessageBox.Show("Plot No/Premises No Missing, Do you want to Add Exception...?", "Morut Data Entry...", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                        if (dialogResult == DialogResult.Yes)
                        {
                            PropertyDetailsException pExp = new PropertyDetailsException();
                            pExp.district_code = property.district_code;
                            pExp.RO_code = property.RO_code;
                            pExp.Book = property.Book;
                            pExp.Deed_year = property.Deed_year;
                            pExp.Deed_no = property.Deed_no;
                            pExp.item_no = property.Serial;
                            pExp.exception = constants.Plot_and_Premisses_Missing;
                            pExp.excDetails = pCom.GetException(constants.Plot_and_Premisses_Missing).Tables[0].Rows[0][0].ToString();
                            propertyExp.Add(pExp);
                        }
                    }
                    string areaAmount = string.Empty;

                    if (rdoGP.Checked == true) { property.Area_type = "G"; }
                    if (rdoMuni.Checked == true) { property.Area_type = "M"; }
                    if (rdoCorp.Checked == true) { property.Area_type = "C"; }
                    if (rdaNA.Checked == true) { property.Area_type = null; }

                    property.GP_Muni_Corp_Code = Convert.ToString(cmbAreaType.SelectedValue);
                    property.Holding = txtHolding.Text.Trim();
                    property.khatian_No = txtKhatianNumber.Text.Trim();
                    property.bata_khatian_no = txtKhatianBataNNumber.Text.Trim();
                    property.Khatian_type = cmbKhatianType.Text;
                    property.Property_district_code = cmbIndex2District.SelectedValue.ToString();

                    if (cmbIndex2ROName.SelectedValue != null)
                    {
                        property.Property_ro_code = cmbIndex2ROName.SelectedValue.ToString();
                    }
                    else { property.Property_ro_code = "0"; }
                    if (cmbIndex2PS.Items.Count > 0 && cmbIndex2PS.SelectedValue != null)
                    {
                        property.ps_code = cmbIndex2PS.SelectedValue.ToString();
                    }
                    else
                    {
                        MessageBox.Show("PS Code Error, Please select again!");

                        //property.ps_code = "0"; 
                    }
                    if (property.ps_code == "0")
                    {
                        MessageBox.Show("PS Code is 0, Please select again!");
                        return;
                    }
                    if (cmbIndex2Road.Text == "Others") { property.road_code = string.Empty; property.Road = txtIndex2Road.Text; }
                    else
                    {
                        if (cmbIndex2Road.SelectedValue != null)
                        {
                            property.road_code = cmbIndex2Road.SelectedValue.ToString();
                            property.Road = cmbIndex2Road.Text;
                        }
                    }
                    property.Ward = txtWard.Text;
                    if (txtIndex2Premises.Text.Length > 0)
                    {
                        property.Premises = txtIndex2Premises.Text.Trim();
                    }
                    else
                    {
                        property.Premises = "";
                    }
                    property.Plot_code_type = cmbPlotCode.Text.Trim();
                    if (txtPlotNumer.Text.Length > 0)
                    {
                        property.Plot_No = txtPlotNumer.Text.Trim();
                    }
                    else
                    {
                        property.Plot_No = "";
                    }
                    property.property_type = cmbPropertyType.SelectedValue.ToString();
                    if (cmbMouza.Items.Count > 0)
                    {
                        if (cmbMouza.SelectedValue != null)
                        {
                            property.moucode = cmbMouza.SelectedValue.ToString();
                        }
                    }
                    property.Land_Area_acre = "0";
                    property.Land_Area_bigha = "0";
                    property.Land_Area_chatak = "0";
                    property.Land_Area_decimal = "0";
                    property.Land_Area_katha = "0";
                    property.Land_Area_sqfeet = "0";
                    property.Structure_area_in_sqFeet = "0";
                    if (cmbJL.Text != "Other")
                    {
                        if (cmbJL.Items.Count > 0)
                        {
                            if (cmbJL.SelectedValue != null)
                            {
                                property.JL_NO = cmbJL.SelectedValue.ToString();
                            }
                        }
                    }
                    else { property.JL_NO = null; }
                    property.Ref_JL_Number = txtRefJL.Text;

                    if (rdoAcre.Checked) { property.Land_Area_acre = txtAcre.Text.Trim(); areaAmount = txtAcre.Text.Trim() + " Acre "; }
                    if (rdoBigha.Checked) { property.Land_Area_bigha = txtbigha.Text.Trim(); areaAmount = areaAmount + txtbigha.Text.Trim() + " Bigha "; }
                    if (rdoChatak.Checked) { property.Land_Area_chatak = txtChatak.Text.Trim(); areaAmount = areaAmount + txtChatak.Text.Trim() + " Chatak "; }
                    if (rdoDecimal.Checked) { property.Land_Area_decimal = txtDecimal.Text.Trim(); areaAmount = areaAmount + txtDecimal.Text.Trim() + " Decimal "; }
                    if (rdoKatha.Checked) { property.Land_Area_katha = txtKatha.Text.Trim(); areaAmount = areaAmount + txtKatha.Text.Trim() + " Katha "; }
                    if (roSqFeet.Checked) { property.Land_Area_sqfeet = txtSqfeetL.Text.Trim(); areaAmount = areaAmount + txtSqfeetL.Text.Trim() + " Sqfeet "; }
                    if (rdoStructureSqFeet.Checked) { property.Structure_area_in_sqFeet = txtSqfeetStructure.Text.Trim(); areaAmount = areaAmount + txtSqfeetStructure.Text.Trim() + " Sqfeet"; }
                    if (txtBataNumber.Text.Trim() != string.Empty)
                    {
                        property.Bata_No = txtBataNumber.Text.Trim();
                    }
                    else
                    { property.Bata_No = "0"; }
                    property.Property_details = property.Area_type + ":" + cmbAreaType.Text + "." + "Premises:" + txtIndex2Premises.Text + "." + cmbPropertyType.Text + "Area:" + areaAmount;
                    property.Police_Station = cmbIndex2PS.Text.Trim();
                    property.District = cmbIndex2District.Text.Trim();
                    property.Where_Registered = cmbIndex2ROName.Text.Trim();
                    property.Ref_ps = txtRefPS.Text;
                    property.Ref_mou = txtRefmou.Text;
                    if (cmbLandtype.DataSource != null)
                    {
                        if (cmbLandtype.SelectedValue != null && cmbLandtype.Text != "")
                        {
                            property.land_type = cmbLandtype.SelectedValue.ToString();
                        }
                        else { property.land_type = null; }
                    }
                    else
                    {
                        property.land_type = null;
                    }
                    /*if (DeedValidation.IsNumericReturnBOOL(txtAcre.Text)) ;
                    {
                        mesurement = mesurement + Convert.ToDouble(txtAcre.Text);
                    }
                    if (DeedValidation.IsNumericReturnBOOL(txtbigha.Text))
                    {
                        mesurement = mesurement + Convert.ToDouble(txtbigha.Text);
                    }
                    if (DeedValidation.IsNumericReturnBOOL(txtDecimal.Text))
                    {
                        mesurement = mesurement + Convert.ToDouble(txtDecimal.Text);
                    }
                    if (DeedValidation.IsNumericReturnBOOL(txtKatha.Text))
                    {
                        mesurement = mesurement + Convert.ToDouble(txtKatha.Text);
                    }
                    if (DeedValidation.IsNumericReturnBOOL(txtChatak.Text))
                    {
                        mesurement = mesurement + Convert.ToDouble(txtChatak.Text);
                    }
                    if (DeedValidation.IsNumericReturnBOOL(txtSqfeetL.Text))
                    {
                        mesurement = mesurement + Convert.ToDouble(txtSqfeetL.Text);
                    }
                    if (DeedValidation.IsNumericReturnBOOL(txtSqfeetStructure.Text))
                    {
                        mesurement = mesurement + Convert.ToDouble(txtSqfeetStructure.Text);
                    }
                     */
                    if (txtAcre.Text.Trim().Length > 0)
                    {
                        string msg = DeedValidation.IsDecimal(txtAcre.Text.Trim());
                        if (msg.Trim().Length == 0)
                        {
                            mesurement = mesurement + Convert.ToDouble(txtAcre.Text);
                        }
                    }
                    if (txtbigha.Text.Trim().Length > 0)
                    {
                        string msg = DeedValidation.IsDecimal(txtbigha.Text.Trim());
                        if (msg.Trim().Length == 0)
                        {
                            mesurement = mesurement + Convert.ToDouble(txtbigha.Text);
                        }
                    }
                    if (txtDecimal.Text.Trim().Length > 0)
                    {
                        string msg = DeedValidation.IsDecimal(txtDecimal.Text.Trim());
                        if (msg.Trim().Length == 0)
                        {
                            mesurement = mesurement + Convert.ToDouble(txtDecimal.Text);
                        }
                    }
                    if (txtKatha.Text.Trim().Length > 0)
                    {
                        string msg = DeedValidation.IsDecimal(txtKatha.Text.Trim());
                        if (msg.Trim().Length == 0)
                        {
                            mesurement = mesurement + Convert.ToDouble(txtKatha.Text);
                        }
                    }
                    if (txtChatak.Text.Trim().Length > 0)
                    {
                        string msg = DeedValidation.IsDecimal(txtChatak.Text.Trim());
                        if (msg.Trim().Length == 0)
                        {
                            mesurement = mesurement + Convert.ToDouble(txtChatak.Text);
                        }
                    }
                    if (txtSqfeetL.Text.Trim().Length > 0)
                    {
                        string msg = DeedValidation.IsDecimal(txtSqfeetL.Text.Trim());
                        if (msg.Trim().Length == 0)
                        {
                            mesurement = mesurement + Convert.ToDouble(txtSqfeetL.Text);
                        }
                    }
                    if (txtSqfeetStructure.Text.Trim().Length > 0)
                    {
                        string msg = DeedValidation.IsDecimal(txtSqfeetStructure.Text.Trim());
                        if (msg.Trim().Length == 0)
                        {
                            mesurement = mesurement + Convert.ToDouble(txtSqfeetStructure.Text);
                        }
                    }
                    if (mesurement == 0)
                    {
                        DialogResult dialogResult = MessageBox.Show("Measurement Missing, Do you want to Add Exception...?", "Morut Data Entry...", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                        if (dialogResult == DialogResult.Yes)
                        {
                            PropertyDetailsException pExp = new PropertyDetailsException();
                            pExp.district_code = property.district_code;
                            pExp.RO_code = property.RO_code;
                            pExp.Book = property.Book;
                            pExp.Deed_year = property.Deed_year;
                            pExp.Deed_no = property.Deed_no;
                            pExp.item_no = property.Serial;
                            pExp.exception = constants.Measurment_Missing;
                            pExp.excDetails = pCom.GetException(constants.Measurment_Missing).Tables[0].Rows[0][0].ToString();
                            propertyExp.Add(pExp);
                        }

                    }

                    ///Invoke the callback to pass the record back to the calling form
                    m_OnAccept.Invoke(property);
                    ///Display a prompt to continue editing when the user has requested adding records
                    if (this._isEditing == Mode._Add)
                    {
                        DialogResult dialogResult = MessageBox.Show("You sure wanna exit...?", " ", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                        if (dialogResult == DialogResult.Yes)
                        {
                            PartEntry = false;
                            this.Close();
                        }
                        else
                        {
                            //property_init(property);
                            pCount = pCount + 1;
                            PartEntry = true;
                            cmdjuri.TabStop = false;
                            cmdConvert.TabStop = false;
                            txtIndex2Premises.Focus();
                        }
                    }
                    //If user is editing a current row, exit the form then
                    else
                    {
                        this.Close();
                    }
                }
                else
                {
                    MessageBox.Show("Validate Input Error!","",MessageBoxButtons.OK,MessageBoxIcon.Error);
                    return;
                }
            }
            catch (Exception ex)
            {
                string xx = ex.Message.ToString();
            }
		}

		private void cmbIndex2ROName_Leave(object sender, EventArgs e)
		{
			if ((cmbIndex2District.DataSource != null) && (cmbIndex2ROName.DataSource != null))
			{
				string districtCode = cmbIndex2District.SelectedValue.ToString();
				string whReg = cmbIndex2ROName.SelectedValue.ToString();
				cmbIndex2Road.DataSource = GetDataSetWithNewField(pCom.GetRoad(districtCode, whReg), "road_name", "road_code");
				cmbIndex2Road.DisplayMember = "road_name";
				cmbIndex2Road.ValueMember = "road_code";
				if (cmbIndex2Road.Items.Count > 0)
					cmbIndex2Road.SelectedIndex = 0;
			}
		}

		private void cmbIndex2District_Leave(object sender, EventArgs e)
		{
			if (_isEditing != Mode._Edit)
			{
				if (cmbIndex2District.DataSource != null)
				{
					string districtCode = cmbIndex2District.SelectedValue.ToString();

					cmbIndex2ROName.DataSource = pCom.GetROffice(districtCode).Tables[0];
					cmbIndex2ROName.DisplayMember = "RO_name";
					cmbIndex2ROName.ValueMember = "RO_code";
					cmbIndex2ROName.SelectedIndex = 0;
					cmbIndex2PS.DataSource = GetDataSetWithNewField(pCom.GetPS(districtCode), "ps_name", "ps_code");
					//cmbIndex2PS.DataSource = dly.GetPS(districtCode).Tables[0];
					cmbIndex2PS.DisplayMember = "PS_name";
					cmbIndex2PS.ValueMember = "PS_code";
					if (cmbIndex2PS.Items.Count > 0)
						cmbIndex2PS.SelectedIndex = 0;
                    //if (property.ps_code == "0")
                    //{
                    //    MessageBox.Show("PS Code Error, Please select again!");
                    //    return;
                    //}
					string whReg = cmbIndex2ROName.SelectedValue.ToString();
					cmbIndex2Road.DataSource = GetDataSetWithNewField(pCom.GetRoad(districtCode, whReg), "road_name", "road_code");
					cmbIndex2Road.DisplayMember = "road_name";
					cmbIndex2Road.ValueMember = "road_code";
					if (cmbIndex2Road.Items.Count > 0)
						cmbIndex2Road.SelectedIndex = 0;

				}
			}
			else
			{
				if (cmbIndex2District.DataSource != null)
				{
					string districtCode = cmbIndex2District.SelectedValue.ToString();

					cmbIndex2ROName.DataSource = pCom.GetROffice(districtCode).Tables[0];
					cmbIndex2ROName.DisplayMember = "RO_name";
					cmbIndex2ROName.ValueMember = "RO_code";
					cmbIndex2ROName.SelectedValue = property.Property_ro_code;
					cmbIndex2PS.DataSource = GetDataSetWithNewField(pCom.GetPS(districtCode), "ps_name", "ps_code");
					cmbIndex2PS.DisplayMember = "PS_name";
					cmbIndex2PS.ValueMember = "PS_code";
					if (cmbIndex2PS.Items.Count > 0)
						cmbIndex2PS.SelectedValue = property.ps_code;
                    //if (property.ps_code == "0")
                    //{
                    //    MessageBox.Show("PS Code Error, Please select again!");
                    //    return;
                    //}
					string whReg = cmbIndex2ROName.SelectedValue.ToString();
					cmbIndex2Road.DataSource = GetDataSetWithNewField(pCom.GetRoad(districtCode, whReg), "road_name", "road_code");
					cmbIndex2Road.DisplayMember = "road_name";
					cmbIndex2Road.ValueMember = "road_code";
					if (cmbIndex2Road.Items.Count > 0)
						cmbIndex2Road.SelectedIndex = 0;

				}
			}
		}

		private void cmbPropertyType_Leave(object sender, EventArgs e)
		{
			try
			{
				if (constants._SUGGEST == true)
				{
					AutoCompleteStringCollection tmp = new AutoCompleteStringCollection();
					for (int i = 0; i < pCom.Getland_type(cmbIndex2District.SelectedValue.ToString(), cmbIndex2ROName.SelectedValue.ToString()).Rows.Count; i++)
					{
						tmp.Add(pCom.Getland_type(cmbIndex2District.SelectedValue.ToString(), cmbIndex2ROName.SelectedValue.ToString()).Rows[i][1].ToString());
					}
					this.cmbLandtype.AutoCompleteCustomSource = tmp;
					this.cmbLandtype.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
					this.cmbLandtype.AutoCompleteSource = AutoCompleteSource.CustomSource;
				}
				if (cmbPropertyType.SelectedValue != null)
				{

					if (cmbPropertyType.SelectedValue.ToString() == "LL")
					{
						cmbLandtype.Enabled = false;
						cmbLandtype.DataSource = null;
						cmbLandtype.Items.Clear();
						btnOtherPlots.Enabled = false;
						btnOtherKhatian.Enabled = false;
						DataTable ds = new DataTable();
						ds = pCom.Getland_type(cmbIndex2District.SelectedValue.ToString(), cmbIndex2ROName.SelectedValue.ToString());
						if (ds.Rows.Count > 0)
						{
							cmbLandtype.DataSource = ds;
							cmbLandtype.DisplayMember = ds.Columns[1].ToString();
							cmbLandtype.ValueMember = ds.Columns[0].ToString();

						}
					}
					else
					{
						cmbLandtype.DataSource = null;
						cmbLandtype.Enabled = false;
					}
					if (cmbPropertyType.SelectedValue.ToString() == "FL" || cmbPropertyType.SelectedValue.ToString() == "CM")
					{
						btnOtherPlots.Enabled = true;
						btnOtherKhatian.Enabled = true;
					}
					//cmbLandtype.Focus();
				}

			}
			catch (Exception ex)
			{
				ex.Message.ToString();
			}
		}

		private void formatForm()
		{

			if (constants._SUGGEST)
			{
				this.cmbLandtype.AutoCompleteCustomSource = pCom.GetSuggestions("tbl_land_type", "eng_desc");
				this.cmbLandtype.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
				this.cmbLandtype.AutoCompleteSource = AutoCompleteSource.CustomSource;

				this.cmbPropertyType.AutoCompleteCustomSource = pCom.GetSuggestions("property_type", "description");
				this.cmbPropertyType.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
				this.cmbPropertyType.AutoCompleteSource = AutoCompleteSource.CustomSource;

				AutoCompleteStringCollection tmp = new AutoCompleteStringCollection();
				tmp.Add("CS");
				tmp.Add("RS");
				tmp.Add("LR");
				this.cmbPlotCode.AutoCompleteCustomSource = tmp;
				this.cmbPlotCode.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
				this.cmbPlotCode.AutoCompleteSource = AutoCompleteSource.CustomSource;

				this.cmbKhatianType.AutoCompleteCustomSource = tmp;
				this.cmbKhatianType.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
				this.cmbKhatianType.AutoCompleteSource = AutoCompleteSource.CustomSource;
			}
			if (PartEntry == true)
			{
				cmdjuri.TabStop = false;
				cmdConvert.TabStop = false;
			}
		}

		private void cmdAbort_Click(object sender, EventArgs e)
		{
			DialogResult retVal = MessageBox.Show(this, "You sure want to abort?", "Warning you!", MessageBoxButtons.YesNo,MessageBoxIcon.Warning,MessageBoxDefaultButton.Button2);
			if (retVal == DialogResult.Yes)
			{
				m_OnAbort.Invoke();
				this.Close();
			}
		}

		private void frmIndex2_Load(object sender, EventArgs e)
		{
			this.panelForm.Top = 0;
			this.panelForm.Left = 0;
			this.panelForm.Size = this.Size;
		}

		private void rdaNA_KeyUp(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Return)
				SendKeys.Send("{Tab}");
		}

		private void rdoGP_KeyUp(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Return)
				SendKeys.Send("{Tab}");
		}

		private void rdoMuni_KeyUp(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Return)
				SendKeys.Send("{Tab}");
		}

		private void rdoCorp_KeyUp(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Return)
				SendKeys.Send("{Tab}");
		}

		private void cmdjuri_Leave(object sender, EventArgs e)
		{
			this.rdaNA.Focus();
		}

		private void rdoAcre_Click(object sender, EventArgs e)
		{
			if (rdoAcre.Checked)
			{ txtAcre.Enabled = true;
				txtAcre.Focus();
			}
			else { txtAcre.Enabled = false; txtAcre.Text = "0"; }
		}

		private void rdoBigha_Click(object sender, EventArgs e)
		{
			if (rdoBigha.Checked)
			{ txtbigha.Enabled = true;
				txtbigha.Focus();
			}
			else { txtbigha.Enabled = false; txtbigha.Text = "0"; }
		}

		private void rdoDecimal_Click(object sender, EventArgs e)
		{
			if (rdoDecimal.Checked)
			{ txtDecimal.Enabled = true;
				txtDecimal.Focus();
			}
			else { txtDecimal.Enabled = false; txtDecimal.Text = "0"; }
		}

		private void rdoKatha_Click(object sender, EventArgs e)
		{
			if (rdoKatha.Checked)
			{ txtKatha.Enabled = true;
				txtKatha.Focus();
			}
			else { txtKatha.Enabled = false; txtKatha.Text = "0"; }
		}

		private void rdoChatak_Click(object sender, EventArgs e)
		{
			if (rdoChatak.Checked)
			{ txtChatak.Enabled = true;
				txtChatak.Focus();
			}
			else { txtChatak.Enabled = false; txtChatak.Text = "0"; }
		}

		private void roSqFeet_Click(object sender, EventArgs e)
		{
			if (roSqFeet.Checked)
			{ txtSqfeetL.Enabled = true;
				txtSqfeetL.Focus();}
			else { txtSqfeetL.Enabled = false; txtSqfeetL.Text = "0"; }
		}

		private void rdoStructureSqFeet_Click(object sender, EventArgs e)
		{
			if (rdoStructureSqFeet.Checked)
			{ txtSqfeetStructure.Enabled = true;
				txtSqfeetStructure.Focus();
			}
			else { txtSqfeetStructure.Enabled = false; txtSqfeetStructure.Text = "0"; }
		}

		private void txtPlotNumer_Enter(object sender, EventArgs e)
		{
			txtPlotNumer.SelectAll();
		}

		private void rdaNA_CheckedChanged(object sender, EventArgs e)
		{

		}

		private void rdaNA_Click(object sender, EventArgs e)
		{
			cmbAreaType.DataSource = null;
			cmbAreaType.Items.Clear();
		}

		private void txtIndex2Premises_Leave(object sender, EventArgs e)
		{
			//if(txtIndex2Premises.Text.Trim().Equals("0"))
			//{
			//    MessageBox.Show(this,"Premises no cann't be zero","",MessageBoxButtons.OK,MessageBoxIcon.Stop);
			//    txtIndex2Premises.Focus();
			//    return;
			//}
		}

		private void txtPlotNumer_Leave(object sender, EventArgs e)
		{
			//string msg = DeedValidation.IsNumericorBlank(txtPlotNumer.Text.Trim());
			//if (msg.Trim().Length > 0)
			//{
			//    MessageBox.Show(this, msg, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
			//    txtPlotNumer.Focus();
			//}
		}

		private void txtBataNumber_Leave(object sender, EventArgs e)
		{
			//string msg = DeedValidation.IsNumericorBlank(txtBataNumber.Text.Trim());
			//if (msg.Trim().Length > 0)
			//{
			//    MessageBox.Show(this, msg, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
			//    txtBataNumber.Focus();
			//}
		}

		private void txtKhatianNumber_Leave(object sender, EventArgs e)
		{
			//string msg = DeedValidation.IsNumericorBlank(txtKhatianNumber.Text.Trim());
			//if (msg.Trim().Length > 0)
			//{
			//    MessageBox.Show(this, msg, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
			//    txtKhatianNumber.Focus();
			//}
		}

		private void txtKhatianBataNNumber_Leave(object sender, EventArgs e)
		{
			//string msg = DeedValidation.IsNumericorBlank(txtKhatianBataNNumber.Text.Trim());
			//if (msg.Trim().Length > 0)
			//{
			//    MessageBox.Show(this, msg, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
			//    txtKhatianBataNNumber.Focus();
			//}
		}

		private void txtWard_Leave(object sender, EventArgs e)
		{
			//string msg = DeedValidation.IsNumericorBlank(txtWard.Text.Trim());
			//if (msg.Trim().Length > 0)
			//{
			//    MessageBox.Show(this, msg, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
			//    txtWard.Focus();
			//}
		}

        private void txtWard_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtWard_KeyPress(object sender, KeyPressEventArgs e)
        {

        }


	}

}
