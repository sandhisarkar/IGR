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

namespace IGRFqc
{
    public partial class frmIndex1 : Form
    {
        private PopulateCombo pCom=null;
        Credentials crd = new Credentials();
        private OdbcConnection sqlCon=null;
        OdbcTransaction txn;
        //igr_deed _mdeed;
        PersonDetails person;
        List<PersonDetailsException> _pexpc = null;
        //DeedControl _mdeed_control;
        Mode _isEditing;
        int pCount;
        //The method to be invoked when the user is accepting values
        public delegate void OnAccept(PersonDetails person);
        OnAccept m_OnAccept;
        //The method to be invoked when the user aborts all operations
        public delegate void OnAbort();
        OnAbort m_OnAbort;

        public frmIndex1(OdbcConnection prmCon,OdbcTransaction pTxn,Credentials prmCrd, PersonDetails pPerson, Mode p_IsEditing, OnAccept pOnAccpet, OnAbort pOnAbort,List<PersonDetailsException> pExp,int mCount)
        {
            InitializeComponent();
            sqlCon = prmCon;
            txn = pTxn;
            crd = prmCrd;
            person = pPerson;
            pCom = new PopulateCombo(sqlCon,txn, crd);
            pCount = mCount;
            _isEditing = p_IsEditing;
            _pexpc = pExp;
            person_init(person);
            //Assign the callbacks
            m_OnAccept = pOnAccpet;
            m_OnAbort = pOnAbort;
            formatForm();
        }
        public frmIndex1(OdbcConnection prmCon, OdbcTransaction pTxn, Credentials prmCrd, DeedControl pDeedControl, OnAccept pOnAccpet, OnAbort pOnAbort, List<PersonDetailsException> pExp, int mCount)
        {
            InitializeComponent();
            sqlCon = prmCon;
            txn = pTxn;
            crd = prmCrd;
            pCom = new PopulateCombo(sqlCon,txn, crd);
            _isEditing = Mode._Add;
            pCount = mCount;
            _pexpc = pExp;
            person_init(person);
            //Assign the callbacks
            m_OnAccept = pOnAccpet;
            m_OnAbort = pOnAbort;
            formatForm();
        }
        private void person_init(PersonDetails personList)
        {
            //txtlinkedto.Visible = true;
            txtlinkedto.Text = person.linked_to;
            cmbCast.DataSource = GetNewDatatable(pCom.GetReligion().Tables[0], "religion_name", "religion_code");
            cmbCast.DisplayMember = "religion_name";
            cmbCast.ValueMember = "religion_code";


            DataTable oDt = pCom.GetOccupation().Tables[0];
            cmbProffession.DataSource = GetNewDatatable(oDt, "occupation_name", "occupation_code");
            cmbProffession.DisplayMember = "occupation_name";
            cmbProffession.ValueMember = "occupation_code";


            cmbInterest.DataSource = pCom.GetPartyCode().Tables[0];
            cmbInterest.DisplayMember = "ec_name";
            cmbInterest.ValueMember = "ec_code";

            cmbIndex1District.DataSource = GetDataSetWithNewField(pCom.GetDistrict(), "district_name", "district_code");
            cmbIndex1District.DisplayMember = "district_name";
            cmbIndex1District.ValueMember = "district_code";
            //cmbIndex1District.SelectedIndex = 0;
            cmbIndex1District.SelectedValue = pCom.GetDistrict_Active().Tables[0].Rows[0][0].ToString();
            string districtCode = cmbIndex1District.SelectedValue.ToString();
            cmbIndex1Ps.DataSource = GetDataSetWithNewField(pCom.GetPS(districtCode), "PS_name", "PS_code");
            cmbIndex1Ps.DisplayMember = "PS_name";
            cmbIndex1Ps.ValueMember = "PS_code";
            cmbIndex1Ps.SelectedIndex = 0;
            if (person.Address_district_code != null && cmbIndex1District.SelectedValue != null)
            {
                districtCode = cmbIndex1District.SelectedValue.ToString();
                cmbIndex1Ps.DataSource = GetDataSetWithNewField(pCom.GetPS(districtCode), "PS_name", "PS_code");
                cmbIndex1Ps.DisplayMember = "PS_name";
                cmbIndex1Ps.ValueMember = "PS_code";
            }
            Populate_other_party();
            cmbIndex1City.SelectedIndex = 0;
            //txtlinkedto.Visible = false;
            lblLink.Visible = false;
            if (_isEditing == Mode._Edit)
            {

                if (person.Address_district_code != null && person.Address_district_code !="")
                {
                    cmbIndex1District.SelectedValue = person.Address_district_code;
                    cmbIndex1Ps.DataSource = GetDataSetWithNewField(pCom.GetPS(cmbIndex1District.SelectedValue.ToString()), "PS_name", "PS_code");
                    cmbIndex1Ps.DisplayMember = "PS_name";
                    cmbIndex1Ps.ValueMember = "PS_code";
                }
                else
                {
                    cmbIndex1District.Text = "Others";
                    txtIndex1District.Text = person.Address_district_name;
                }
                if (person.Address_ps_code != null && person.Address_ps_code != "")
                {
                    cmbIndex1Ps.SelectedValue = person.Address_ps_code.PadLeft(2, '0').ToString();
                }
                else
                {
                    cmbIndex1Ps.Text = "Others";
                    txtIndex1Ps.Text = person.Address_ps_Name;
                }
                cmbInterest.SelectedValue = person.Status_code;
                if (person.other_party_code != null && person.other_party_code.ToString() != "")
                {
                    cmbPartytype.Enabled = true;
                    lblPartyType.Enabled = true;
                    cmbPartytype.SelectedValue = person.other_party_code;
                }
                    cmbPInitial.Text = person.initial_name == string.Empty ? "None" : person.initial_name;
                    txtPFirstandMiddle.Text = person.First_name;
                    txtLast.Text = person.Last_Name;
                    cmbProffession.SelectedValue = person.Proffession;
                    cmbCast.SelectedValue = person.Cast;
                    switch (person.Rel_code)
                    {
                        case "S":
                            { rdoSonOf.Checked = true; break; }
                        case "D":
                            { rdoDOf.Checked = true; break; }
                        case "W":
                            { rdoWOf.Checked = true; break; }
                        case "R":
                            { rdoRepOf.Checked = true; break; }
                    }
                    switch (person.Father_mother)
                    {
                        case "F":
                            { rdoFatherName.Checked = true; break; }
                        case "M":
                            { rdoMotherName.Checked = true; break; }
                        case "N":
                            { rdoNotApplicable.Checked = true; break; }
                    }
                    if (person.F_Initial_name != string.Empty)
                    {
                        cmbFInitial.Text = person.F_Initial_name;
                    }
                    else { cmbFInitial.SelectedIndex = 0; }
                    txtFFirstName.Text = person.Relation;
                    txtAddress.Text = person.Address;
                    if (!string.IsNullOrEmpty(person.Address_district_code))
                    {
                        cmbIndex1District.SelectedValue = person.Address_district_code;
                    }
                    else { cmbIndex1District.SelectedText = "Others"; txtIndex1District.Enabled = true; txtIndex1District.Text = person.Address_district_name; }
                    if (!string.IsNullOrEmpty(person.Address_ps_code))
                    { cmbIndex1Ps.SelectedValue = person.Address_ps_code.PadLeft(2, '0').ToString(); }
                    else { cmbIndex1Ps.SelectedText = "Others"; txtIndex1Ps.Enabled = true; txtIndex1Ps.Text = person.Address_ps_Name; }
                    if(person.City_Name.Equals("N/A"))
                    {
                    cmbIndex1City.Text = person.City_Name;
                    }
                    else
                    {
                     cmbIndex1City.SelectedText = "Others";
                    cmbIndex1City.SelectedText = person.City_Name;
                    txtIndex1City.Text = person.City_Name;
                    }
                    txtPin.Text = person.PIN;
                    if (person.more == "Y")
                    {
                        chkmore.Checked = true;
                    }
                    else
                    {
                        chkmore.Checked = false;
                    }
                    if (cmbIndex1District.Text.Equals("N/A"))
                    {
                        txtIndex1District.Enabled = false;
                    }
                    else
                    {
                        txtIndex1District.Enabled = true;
                    }
                    if (person.Address_ps_Name.Equals("N/A"))
                    {
                        cmbIndex1Ps.Text = "N/A";
                        txtIndex1Ps.Enabled = false;
                    }
                    //else
                    //{
                    //    cmbIndex1City.Text = "N/A";
                    //    txtIndex1Ps.Enabled = true;
                    //}
                    if (person.City_Name.Equals("N/A"))
                    {
                        cmbIndex1City.Text = "N/A";
                        txtIndex1City.Enabled = false;
                    }
                    else
                    {
                        cmbIndex1City.Text = "Others";
                        txtIndex1City.Enabled = true;
                    }
            }
        }


        private DataTable GetNewDatatable(DataTable pDt, string pDisplayColumn, string pValueColumn)
        {
            DataRow dr = pDt.NewRow();
            dr[pDisplayColumn] = "None";
            dr[pValueColumn] = "";
            pDt.Rows.Add(dr);
            return pDt;
        }
        private DataTable GetDataSetWithNewField(DataSet pDs, string pName, string pCode)
        {
            DataRow newRow = pDs.Tables[0].NewRow();
            newRow[pName] = "Others";
            newRow[pCode] = "0";
            pDs.Tables[0].Rows.Add(newRow);
            return pDs.Tables[0];
        }

        private void cmdClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private Boolean validateFields()
        {
            bool flag = true;
            string msg = DeedValidation.ChkNumeric6DigitOrEmpty(txtPin.Text.Trim());
            if (msg.Trim().Length > 0)
            {
                MessageBox.Show(this, msg, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtPin.Focus();
                flag = false;
            }
            if (txtPFirstandMiddle.Text.Trim().Length == 0)
            {
                MessageBox.Show(this, "First Name cannot be blank...", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtPFirstandMiddle.Focus();
                flag = false;
            }
            if (rdoSonOf.Checked == false && rdoDOf.Checked == false && rdoWOf.Checked == false && rdoRepOf.Checked == false && rdoNotApplicable.Checked == false)
            {
                MessageBox.Show(this, "Please select relation...", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                flag = false;

            }
            if ((rdoSonOf.Checked == true || rdoDOf.Checked == true || rdoWOf.Checked == true || rdoRepOf.Checked == true) && txtFFirstName.Text.Trim().Length == 0 )
            {
                if (rdoNotApplicable.Checked == false)
                {
                    MessageBox.Show(this, "Please Select Relation Name...", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtFFirstName.Focus();
                    flag = false;
                }

            }
            
            return flag;
        }
        private void cmdSave_Click(object sender, EventArgs e)
        {
            if (validateFields())
            {
                
                if (_isEditing == Mode._Edit)
                {
                   // person.Serial = pCount.ToString();
                    foreach (var x in _pexpc.ToList())
                    {
                        if (x.item_no.ToString().Equals(person.Serial))
                        {
                            int idx = _pexpc.FindIndex(a => a.item_no.ToString().Equals(person.Serial));
                            _pexpc.RemoveAt(idx);

                        }

                    }
                    _pexpc.ToList();
                }
                if (cmbIndex1District.Text.ToLower().Equals("others") && cmbIndex1City.Text.ToLower().Equals("others") && cmbIndex1Ps.Text.ToLower().Equals("others") && (txtPin.Text.Trim().Length == 0 || txtPin.Text.Equals("000000")) && txtIndex1District.Text.Trim().Length == 0 && txtIndex1Ps.Text.Trim().Length == 0 && txtIndex1City.Text.Trim().Length == 0)
                {
                    DialogResult dialogResult = MessageBox.Show("Address Missing, Do you want to Add Exception...?", "Morut Data Entry...", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (dialogResult == DialogResult.Yes)
                    {
                        if (_isEditing == Mode._Add)
                        {

                            person.Serial = pCount.ToString();
                        }
                        PersonDetailsException expc = new PersonDetailsException();
                        expc.district_code = person.district_code;
                        expc.RO_code = person.RO_code;
                        expc.Book = person.Book;
                        expc.Deed_year = person.Deed_year;
                        expc.Deed_no = person.Deed_no;
                        expc.item_no = person.Serial;
                        expc.exception = constants.Address_Missing;
                        expc.excDetails = pCom.GetException(constants.Address_Missing).Tables[0].Rows[0][0].ToString();
                        _pexpc.Add(expc);
                    }
                }

                if (cmbIndex1District.Text.ToLower().Equals("n/a") && cmbIndex1City.Text.ToLower().Equals("others") && cmbIndex1Ps.Text.ToLower().Equals("n/a") && (txtPin.Text.Trim().Length == 0 || txtPin.Text.Equals("000000")) && txtIndex1District.Text.Trim().Length == 0 && txtIndex1Ps.Text.Trim().Length == 0 && txtIndex1City.Text.Trim().Length == 0)
                {
                    DialogResult dialogResult = MessageBox.Show("Address Missing, Do you want to Add Exception...?", "Morut Data Entry...", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (dialogResult == DialogResult.Yes)
                    {
                        if (_isEditing == Mode._Add)
                        {

                            person.Serial = pCount.ToString();
                        }
                        PersonDetailsException expc = new PersonDetailsException();
                        expc.district_code = person.district_code;
                        expc.RO_code = person.RO_code;
                        expc.Book = person.Book;
                        expc.Deed_year = person.Deed_year;
                        expc.Deed_no = person.Deed_no;
                        expc.item_no = person.Serial;
                        expc.exception = constants.Address_Missing;
                        expc.excDetails = pCom.GetException(constants.Address_Missing).Tables[0].Rows[0][0].ToString();
                        _pexpc.Add(expc);
                    }
                }
                if (cmbIndex1District.Text.ToLower().Equals("n/a") && cmbIndex1City.Text.ToLower().Equals("n/a") && cmbIndex1Ps.Text.ToLower().Equals("n/a") && (txtPin.Text.Trim().Length == 0 || txtPin.Text.Equals("000000")) && txtIndex1District.Text.Trim().Length == 0 && txtIndex1Ps.Text.Trim().Length == 0 && txtIndex1City.Text.Trim().Length == 0)
                {
                    DialogResult dialogResult = MessageBox.Show("Address Missing, Do you want to Add Exception...?", "Morut Data Entry...", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (dialogResult == DialogResult.Yes)
                    {
                        if (_isEditing == Mode._Add)
                        {

                            person.Serial = pCount.ToString();
                        }
                        PersonDetailsException expc = new PersonDetailsException();
                        expc.district_code = person.district_code;
                        expc.RO_code = person.RO_code;
                        expc.Book = person.Book;
                        expc.Deed_year = person.Deed_year;
                        expc.Deed_no = person.Deed_no;
                        expc.item_no = person.Serial;
                        expc.exception = constants.Address_Missing;
                        expc.excDetails = pCom.GetException(constants.Address_Missing).Tables[0].Rows[0][0].ToString();
                        _pexpc.Add(expc);
                    }
                }
                if (_isEditing == Mode._Add)
                {

                    person.Serial = pCount.ToString();
                }
                person.initial_name = cmbPInitial.Text.Trim() == "None" ? string.Empty : cmbPInitial.Text.Trim();
                person.First_name = txtPFirstandMiddle.Text.Trim();
                person.Last_Name = txtLast.Text.Trim();
                if (chkmore.Checked == true)
                {
                    person.more = "Y";
                }
                else
                {
                    person.more = "N";
                }
                person.F_Initial_name = cmbFInitial.Text.Trim() == "None" ? string.Empty : cmbFInitial.Text.Trim();
                person.F_First_name = txtFFirstName.Text.Trim();
                if (cmbInterest.SelectedValue.ToString() != "22")
                {
                    person.F_Last_Name = txtFLastName.Text.Trim();
                }
                else
                {
                    person.F_Last_Name = null;
                }
                person.Status_code = Convert.ToString(cmbInterest.SelectedValue);
                person.Status_name = cmbInterest.Text.Trim();
                string pin = txtPin.Text.Trim() != string.Empty ? txtPin.Text.Trim() : "000000";
                person.Address = txtAddress.Text.Trim();
                if (txtPin.Text.Trim() != string.Empty)
                {
                    person.PIN = txtPin.Text.Trim();
                }
                else { person.PIN = "000000"; }
                if(cmbIndex1City.Text.Equals("N/A"))
                {
                    person.City_Name = "N/A";
                }
                else 
                {
                    person.City_Name = txtIndex1City.Text.Trim();
                }
                //person.City_Name = txtIndex1City.Text.Trim();
                if (cmbIndex1District.Text == "Others")
                {
                    person.Address_district_name = txtIndex1District.Text.Trim();
                    person.Address_district_code = null;
                }
                else
                {
                    person.Address_district_code = cmbIndex1District.SelectedValue.ToString();
                    person.Address_district_name = cmbIndex1District.Text.Trim();
                }
                if (cmbIndex1Ps.Text == "Others")
                {
                    person.Address_ps_Name = txtIndex1Ps.Text;
                    person.Address_ps_code = null;
                }
                else
                {
                    person.Address_ps_code = Convert.ToString(cmbIndex1Ps.SelectedValue);
                    person.Address_ps_Name = cmbIndex1Ps.Text;
                }
                if (rdoFatherName.Checked)
                { person.Father_mother = "F"; }
                else if (rdoMotherName.Checked)
                { person.Father_mother = "M"; }
                else { person.Father_mother = "N"; }
                if (rdoSonOf.Checked)
                { person.Rel_code = "S"; }
                else if (rdoDOf.Checked)
                { person.Rel_code = "D"; }
                else if (rdoWOf.Checked)
                { person.Rel_code = "W"; }
                else
                { person.Rel_code = "R"; }
                person.Proffession = Convert.ToString(cmbProffession.SelectedValue);
                person.Cast = Convert.ToString(cmbCast.SelectedValue);
                person.Proffession_Name = Convert.ToString(cmbProffession.Text);
                person.Cast_Name = Convert.ToString(cmbCast.Text);
                string fInitial = cmbFInitial.Text.Trim() == "None" ? string.Empty : cmbFInitial.Text.Trim();
                person.Relation = txtFFirstName.Text.Trim() + " " + txtFLastName.Text.Trim();
                person.Relation = person.Relation.Trim();
                person.linked_to = txtlinkedto.Text;
                if (cmbPartytype.SelectedValue != null)
                {
                    person.other_party_code = cmbPartytype.SelectedValue.ToString();
                }
                else
                {
                    person.other_party_code = null;
                }
                if (_isEditing == Mode._Add)
                {

                }
                m_OnAccept.Invoke(person);
                this.Close();
            }
        }

        private void cmbIndex1District_SelectedIndexChanged(object sender, EventArgs e)
        {
            DataTable ds = new DataTable();
            if (cmbIndex1District.SelectedValue != null && cmbIndex1District.SelectedValue.ToString() != "")
            {
                string districtCode = cmbIndex1District.SelectedValue.ToString();
                ds = GetDataSetWithNewField(pCom.GetPS(districtCode), "PS_name", "PS_code");
                cmbIndex1Ps.DataSource = ds;
                cmbIndex1Ps.DisplayMember = "PS_name";
                cmbIndex1Ps.ValueMember = "PS_code";
                if (constants._SUGGEST == true)
                {
                    AutoCompleteStringCollection tmp = new AutoCompleteStringCollection();
                    for (int i = 0; i < ds.Rows.Count; i++)
                    {
                        tmp.Add(ds.Rows[i][1].ToString());
                    }
                    this.cmbIndex1Ps.AutoCompleteCustomSource = tmp;
                    this.cmbIndex1Ps.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                    this.cmbIndex1Ps.AutoCompleteSource = AutoCompleteSource.CustomSource;
                }
            }
        }
        private void formatForm()
        {

                if (constants._SUGGEST == true)
                {
                    this.txtPFirstandMiddle.AutoCompleteCustomSource = pCom.GetSuggestions("index_of_name", "First_name");
                    this.txtPFirstandMiddle.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                    this.txtPFirstandMiddle.AutoCompleteSource = AutoCompleteSource.CustomSource;

                    this.txtLast.AutoCompleteCustomSource = pCom.GetSuggestions("index_of_name", "last_name");
                    this.txtLast.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                    this.txtLast.AutoCompleteSource = AutoCompleteSource.CustomSource;

                    this.cmbProffession.AutoCompleteCustomSource = pCom.GetSuggestions("occupation", "occupation_name");
                    this.cmbProffession.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                    this.cmbProffession.AutoCompleteSource = AutoCompleteSource.CustomSource;

                    this.cmbInterest.AutoCompleteCustomSource = pCom.GetSuggestions("party_code","ec_name");
                    this.cmbInterest.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                    this.cmbInterest.AutoCompleteSource = AutoCompleteSource.CustomSource;

                    this.cmbCast.AutoCompleteCustomSource = pCom.GetSuggestions("religion", "religion_name");
                    this.cmbCast.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                    this.cmbCast.AutoCompleteSource = AutoCompleteSource.CustomSource;

                    //this.cmbIndex1Ps.AutoCompleteCustomSource = pCom.GetSuggestions("ps", "ps_name");
                    //this.cmbIndex1Ps.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                    //this.cmbIndex1Ps.AutoCompleteSource = AutoCompleteSource.CustomSource;

                    this.txtFFirstName.AutoCompleteCustomSource = pCom.GetSuggestions("index_of_name", "First_name");
                    this.txtFFirstName.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                    this.txtFFirstName.AutoCompleteSource = AutoCompleteSource.CustomSource;


                    this.txtFLastName.AutoCompleteCustomSource = pCom.GetSuggestions("index_of_name", "last_name");
                    this.txtFLastName.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                    this.txtFLastName.AutoCompleteSource = AutoCompleteSource.CustomSource;

                    this.txtAddress.AutoCompleteCustomSource = pCom.GetSuggestions("index_of_name", "Address");
                    this.txtAddress.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                    this.txtAddress.AutoCompleteSource = AutoCompleteSource.CustomSource;

                    this.cmbIndex1District.AutoCompleteCustomSource = pCom.GetSuggestions("district", "district_name");
                    this.cmbIndex1District.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                    this.cmbIndex1District.AutoCompleteSource = AutoCompleteSource.CustomSource;

                    AutoCompleteStringCollection tmp = new AutoCompleteStringCollection();
                    tmp.Add("None");
                    tmp.Add("Mr.");
                    tmp.Add("Ms.");
                    tmp.Add("Mrs.");
                    this.cmbPInitial.AutoCompleteCustomSource = tmp;
                    this.cmbPInitial.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                    this.cmbPInitial.AutoCompleteSource = AutoCompleteSource.CustomSource;


                }
            }

        private void cmbInterest_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbInterest.Text.ToUpper().Contains("REPRESENTATIVE"))
            {
                rdoRepOf.Checked = true;
                rdoNotApplicable.Checked = true;
                txtFLastName.Text = "";
            }
            if (cmbInterest.Text.ToUpper().Contains("ATTORNEY") && frmVolume.deed_book != "IV")
            {
                rdoRepOf.Checked = true;
                rdoNotApplicable.Checked = true;
                txtFLastName.Text = "";
            }

            if (pCom.Party_dependency_Check(cmbInterest.SelectedValue.ToString()) == true && frmVolume.deed_book != "IV")
            {
                Populate_other_party();
                lblPartyType.Enabled = true;
                cmbPartytype.Enabled = true;
                
            }
            else
            {
                cmbPartytype.DataSource = null;
                lblPartyType.Enabled = false;
                cmbPartytype.Enabled = false;

            }
            if (cmbInterest.Text.ToUpper().Contains("GUARDIAN"))
            {
                txtlinkedto.Visible = true;
                lblLink.Visible = true;
                txtlinkedto.Text = person.linked_to;
            }
            else
            {
                txtlinkedto.Visible = false;
                lblLink.Visible = false;
                txtlinkedto.Text = "";
            }
        }
        private void Populate_other_party()
        {
            cmbPartytype.DataSource = pCom.GetOther_PartyCode().Tables[0];
            cmbPartytype.DisplayMember = "ec_name";
            cmbPartytype.ValueMember = "ec_code";
        }

        private void cmdAbort_Click(object sender, EventArgs e)
        {
            DialogResult retVal = MessageBox.Show(this, "You sure want to abort?", "Warning you!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);
            if (retVal == DialogResult.Yes)
            {
                m_OnAbort.Invoke();
                this.Close();
            }
        }

        private void frmIndex1_Load(object sender, EventArgs e)
        {
            this.panelForm.Top = 0;
            this.panelForm.Left = 0;
            this.panelForm.Size = this.Size;
        }

        private void txtAddress_Enter(object sender, EventArgs e)
        {
            txtAddress.DeselectAll();
        }

        private void txtAddress_Leave(object sender, EventArgs e)
        {
            txtAddress.Text = txtAddress.Text.Trim();
        }

        private void chkmore_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return)
                SendKeys.Send("{Tab}");
        }

        private void rdoSonOf_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return)
                SendKeys.Send("{Tab}");
        }

        private void rdoDOf_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return)
                SendKeys.Send("{Tab}");
        }

        private void rdoWOf_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return)
                SendKeys.Send("{Tab}");
        }

        private void rdoRepOf_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return)
                SendKeys.Send("{Tab}");
        }

        private void rdoFatherName_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return)
                SendKeys.Send("{Tab}");
        }

        private void rdoMotherName_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return)
                SendKeys.Send("{Tab}");
        }

        private void rdoNotApplicable_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return)
                SendKeys.Send("{Tab}");
        }

        private void txtLast_Leave(object sender, EventArgs e)
        {
            if (txtLast.Text.Trim() != string.Empty)
            { txtFLastName.Text = txtLast.Text; }
            if (cmbInterest.Text.ToUpper().Contains("REPRESENTATIVE"))
            {
                txtFLastName.Text = "";
            }
            if (cmbInterest.Text.ToUpper().Contains("ATTORNEY") && frmVolume.deed_book != "IV")
            {
                txtFLastName.Text = "";
            }
        }

        private void rdoSonOf_Click(object sender, EventArgs e)
        {
            if (rdoSonOf.Checked == true)
            {
                rdoFatherName.Checked = true;
            }
        }

        private void rdoRepOf_Click(object sender, EventArgs e)
        {
            if(rdoRepOf.Checked == true)
            {
                rdoNotApplicable.Checked = true;
            }
        }

        private void cmbIndex1Ps_Enter(object sender, EventArgs e)
        {
            
        }

        private void rdoDOf_Click(object sender, EventArgs e)
        {
            if (rdoDOf.Checked == true)
            {
                rdoFatherName.Checked = true;
            }
        }

        private void rdoWOf_Click(object sender, EventArgs e)
        {
            if (rdoWOf.Checked == true)
            {
                rdoNotApplicable.Checked = true;
            }
        }

        private void txtFFirstName_Leave(object sender, EventArgs e)
        {
            //string msg = DeedValidation.ChkName(txtFFirstName.Text);
            //if (msg.Trim().Length > 0)
            //{
            //    MessageBox.Show(this, msg, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    txtFFirstName.Focus();
            //}
        }

        private void cmbIndex1Ps_Leave(object sender, EventArgs e)
        {
            if (cmbIndex1District.Text.ToLower().Equals("n/a") && cmbIndex1Ps.Text.ToLower().Equals("n/a"))
            {
                txtIndex1District.Enabled = false;
                txtIndex1Ps.Enabled = false;
            }
            else
            {
                txtIndex1District.Enabled = true;
                txtIndex1Ps.Enabled = true;
            }
        }

        private void cmbIndex1District_Leave(object sender, EventArgs e)
        {
            if (cmbIndex1District.Text.ToLower().Equals("n/a") && cmbIndex1Ps.Text.ToLower().Equals("n/a"))
            {
                txtIndex1District.Enabled = false;
                txtIndex1Ps.Enabled = false;
                cmbIndex1Ps.Enabled = false;
                cmbIndex1City.Text = "N/A";
                cmbIndex1City.Enabled = false;
                txtIndex1City.Enabled = false;
            }
            else
            {
                txtIndex1District.Enabled = true;
                txtIndex1Ps.Enabled = true;
                cmbIndex1Ps.Enabled = true;
                cmbIndex1City.Enabled = true;
                txtIndex1City.Enabled = true;
            }
            if (cmbIndex1District.Text.ToLower().Equals("others") && cmbIndex1Ps.Text.ToLower().Equals("others"))
            {
                cmbIndex1City.Text = "Others";
            }
        }

        private void panelForm_Paint(object sender, PaintEventArgs e)
        {

        }

        private void cmbIndex1City_Leave(object sender, EventArgs e)
        {
            if (cmbIndex1City.Text.ToLower().Equals("n/a"))
            {
                txtIndex1City.Enabled = false;
            }
            else
            {
                txtIndex1City.Enabled = true;
            }
        }
    }
}
