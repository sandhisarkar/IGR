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

namespace IGRFqc
{
	

	public partial class frmDeedsummery : Form
    {
        public static int nxt;
	
        private OdbcConnection sqlCon = null;
        OdbcTransaction txn;
        Credentials crd = new Credentials();
        bool saveFlag = false;
        List<Volumes> volumes = new List<Volumes>();
        List<Volumes> temp = new List<Volumes>();
        List<DeedDetails> plist = new List<DeedDetails>();
        outSideWBList pExp;
        igr_deed m_deed;
        Mode _mode = Mode._Edit;
        Mode _Property_Mode = Mode._Edit;
        Mode _PropertyWB_Mode = Mode._Edit;
        Mode _Person_Mode = Mode._Edit;
        private PopulateCombo pCom = null;
        private bool Commit= true;
        int count = 0;
        List<PersonDetailsException> _pexpc = null;


        string iniPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase).Remove(0, 6) + "\\" + "IhConfiguration.ini";
        INIFile ini = new INIFile();
        bool isOutsideWB = false;
        bool isProperty = true;

        
        public frmDeedsummery(OdbcConnection prmCon,OdbcTransaction pTxn,Credentials prmCrd, igr_deed deed)
        {
            InitializeComponent();
            find_OWP_or_Not();
            sqlCon = prmCon;
            txn = pTxn;
            pCom = new PopulateCombo(sqlCon, txn, crd);
            crd = prmCrd;
            m_deed = deed;
            init_deed(deed);
        }
        public frmDeedsummery(OdbcConnection prmCon,OdbcTransaction pTxn,Credentials prmCrd, igr_deed deed, Mode pMode)
        {
            InitializeComponent();
            find_OWP_or_Not();
            sqlCon = prmCon;
            txn = pTxn;
            crd = prmCrd;
            pCom = new PopulateCombo(sqlCon, txn, crd);
            m_deed = deed;
            _mode = pMode;
            init_deed(deed);
            if (_mode == Mode._View) { this.cmdUpdateDeed.Enabled = false; }
        }
        public frmDeedsummery(OdbcConnection prmCon, OdbcTransaction pTxn, Credentials prmCrd, igr_deed deed, Mode pMode,bool pCommit)
        {
            InitializeComponent();
            find_OWP_or_Not();
            sqlCon = prmCon;
            txn = pTxn;
            crd = prmCrd;
            pCom = new PopulateCombo(sqlCon, txn, crd);
            m_deed = deed;
            Commit = pCommit;
            _mode = pMode;
            init_deed(deed);
            if (_mode == Mode._View) { this.cmdUpdateDeed.Enabled = false; }
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

        private void init_deed(igr_deed deed)
        {

            if (frmVolume._isProperty == false)
            {
                GrpProperty.Enabled = false;
                cmdAddwbProperty.Enabled = false;
            }
            else
            {
                GrpProperty.Enabled = true;
                cmdAddwbProperty.Enabled = true;
            }

            if (deed._GetDeed.DeedHeader.Deed_control.Book == "3" || deed._GetDeed.DeedHeader.Deed_control.Book == "4")
            {
                frmVolume._isProperty = false;
                GrpProperty.Enabled = false;
                cmdAddwbProperty.Enabled = false;
                
            }
            else
            {
                frmVolume._isProperty = true;
                GrpProperty.Enabled = true;
                cmdAddwbProperty.Enabled = true;
            }

            label1.Text = "Book Type : " + deed._GetDeed.DeedHeader.Deed_control.Book;

            PopulateAllCombo();
           
            if (_mode == Mode._Edit || _mode == Mode._View)
            {
                label1.Text = "Book Type : " + deed._GetDeed.DeedHeader.Deed_control.Book;

                cmbDistrict.SelectedValue = deed._GetDeed.DeedHeader.Deed_control.District_code;
                if (cmbDistrict.DataSource != null)
                {
                    //string districtCode = cmbDistrict.SelectedValue.ToString();
                    string districtCode = deed._GetDeed.DeedHeader.Deed_control.District_code;
                    cmbRO.DataSource = pCom.GetROffice(districtCode).Tables[0];
                    cmbRO.DisplayMember = "RO_name";
                    cmbRO.ValueMember = "RO_code";
                }
                cmbRO.SelectedValue = deed._GetDeed.DeedHeader.Deed_control.RO_code;
                cmbBook.SelectedValue = deed._GetDeed.DeedHeader.Deed_control.Book;
                txtDeedNo.Text = deed._GetDeed.DeedHeader.Deed_control.Deed_no;
                txtdeedYear.Text = deed._GetDeed.DeedHeader.Deed_control.Deed_year;
                TxtVol.Text = deed._GetDeed.DeedHeader.volume_no;
                dgvDeed.DataSource = deed._GetDeed.DeedHeader;
                dgvIndex1.DataSource = deed._GetDeed.Persons;
                dgvIndex2.DataSource = null;
                //if (deed._GetDeed.Properties.Count > 0)
                //{
                (dgvIndex2.BindingContext[m_deed._GetDeed.Properties] as CurrencyManager).Refresh();
                this.dgvIndex2.DataSource = null;    
                dgvIndex2.DataSource = deed._GetDeed.Properties;
                //}
                  (dgvPropertyExp.BindingContext[m_deed._GetDeed.Pro_Excp] as CurrencyManager).Refresh();
                    this.dgvPropertyExp.DataSource = null;

                    dgvPropertyExp.DataSource = deed._GetDeed.Pro_Excp;
                dgvPropertyExp.Refresh();
                //dgvPropertyExp.ReadOnly = false;
                dgvPropertyExp.Columns[0].Visible = false;
                dgvPropertyExp.Columns[1].Visible = false;
                dgvPropertyExp.Columns[2].Visible = false;
                dgvPropertyExp.Columns[3].Visible = false;
                dgvPropertyExp.Columns[4].Visible = false;
                dgvPropertyExp.Columns[6].Visible = false;
               // dgvPropertyExp.ColumnHeadersVisible = false;
                //dgvPropertyExp.ReadOnly = true;
                (dgvPersonExp.BindingContext[m_deed._GetDeed.P_Excp] as CurrencyManager).Refresh();
                this.dgvPersonExp.DataSource = null;
                dgvPersonExp.DataSource = deed._GetDeed.P_Excp;

                dgvPersonExp.Columns[0].Visible = false;
                dgvPersonExp.Columns[1].Visible = false;
                dgvPersonExp.Columns[2].Visible = false;
                dgvPersonExp.Columns[3].Visible = false;
                dgvPersonExp.Columns[4].Visible = false;
                dgvPersonExp.Columns[6].Visible = false;
                //dgvPersonExp.ColumnHeadersVisible = false;
                dgvPersonExp.ReadOnly = true;

                dgvDeedExp.DataSource = deed._GetDeed.D_Excp;
                dgvDeedExp.Columns[0].Visible = false;
                dgvDeedExp.Columns[1].Visible = false;
                dgvDeedExp.Columns[2].Visible = false;
                dgvDeedExp.Columns[3].Visible = false;
                dgvDeedExp.Columns[4].Visible = false;
                dgvDeedExp.Columns[5].Visible = false;
                //dgvDeedExp.ColumnHeadersVisible = false;
                dgvoutsideWBIndex2.DataSource = null;
                //if (m_deed._GetDeed.PropertiesoutWB.Count > 0)
                //{
                    dgvoutsideWBIndex2.DataSource = m_deed._GetDeed.PropertiesoutWB;
                //}
                DeedDetails ddList = new DeedDetails();

                ddList.Deed_control.District_code = deed._GetDeed.DeedHeader.Deed_control.District_code;//
                ddList.Deed_control.RO_code = deed._GetDeed.DeedHeader.Deed_control.RO_code;//
                ddList.Deed_control.Book = deed._GetDeed.DeedHeader.Deed_control.Book;//
                if (deed._GetDeed.DeedHeader.Deed_control.Book == "3" || deed._GetDeed.DeedHeader.Deed_control.Book == "4")
                {
                    frmVolume._isProperty = false;
                    GrpProperty.Enabled = false;
                    cmdAddwbProperty.Enabled = false;
                }
                else
                {
                    frmVolume._isProperty = true;
                    GrpProperty.Enabled = true;
                    cmdAddwbProperty.Enabled = true;
                }
                ddList.Deed_control.Deed_year = deed._GetDeed.DeedHeader.Deed_control.Deed_year;//
                ddList.Deed_control.Deed_no = deed._GetDeed.DeedHeader.Deed_control.Deed_no;//
                ddList.Serial_no = deed._GetDeed.DeedHeader.Serial_no;//
                ddList.Serial_year = deed._GetDeed.DeedHeader.Serial_year;//
                ddList.tran_maj_code = deed._GetDeed.DeedHeader.tran_maj_code;
                ddList.tran_min_code = deed._GetDeed.DeedHeader.tran_min_code;
                ddList.volume_no = deed._GetDeed.DeedHeader.volume_no;//    
                ddList.page_from = deed._GetDeed.DeedHeader.page_from;
                ddList.page_to = deed._GetDeed.DeedHeader.page_to;//
                ddList.date_of_completion = deed._GetDeed.DeedHeader.date_of_completion;
                ddList.date_of_delivery = deed._GetDeed.DeedHeader.date_of_delivery;
                ddList.deed_remarks = deed._GetDeed.DeedHeader.deed_remarks;
                ddList.doc_type = deed._GetDeed.DeedHeader.doc_type;//
                ddList.scan_doc_type = deed._GetDeed.DeedHeader.scan_doc_type;
                ddList.addl_pages = deed._GetDeed.DeedHeader.addl_pages;
                ddList.hold = deed._GetDeed.DeedHeader.hold;
                ddList.hold_reason = deed._GetDeed.DeedHeader.hold_reason;
                ddList.status = deed._GetDeed.DeedHeader.status;
                ddList.created_system = deed._GetDeed.DeedHeader.created_system;
                ddList.version = deed._GetDeed.DeedHeader.version;
                plist.Add(ddList);
                dgvDeed.DataSource = plist;
                if (m_deed._GetDeed.PropertiesoutWB.Count > 0)
                {
                    cmdAddwbProperty.Enabled = false;
                    cmdPropertyoutWB.Enabled = true;
                }
                else
                {
                    cmdAddwbProperty.Enabled = true;
                    cmdPropertyoutWB.Enabled = false;
                }
            }
            if (deed._GetDeed.DeedHeader.Deed_control.Book == "3" || deed._GetDeed.DeedHeader.Deed_control.Book == "4")
            {
                frmVolume._isProperty = false;
                GrpProperty.Enabled = false;
                cmdAddwbProperty.Enabled = false;
            }
            else
            {
                frmVolume._isProperty = true;
                GrpProperty.Enabled = true;
                cmdAddwbProperty.Enabled = true;
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
                cmbRO.DataSource = pCom.GetROffice_Active(districtCode).Tables[0];
                cmbRO.DisplayMember = "RO_name";
                cmbRO.ValueMember = "RO_code";
                cmbRO.SelectedIndex = 0;
            }
            DataSet dsBook = pCom.GetBookType();
            if (dsBook.Tables[0].Rows.Count > 0)
            {
                cmbBook.DataSource = pCom.GetBookType().Tables[0];
                cmbBook.DisplayMember = "key_book";
                cmbBook.ValueMember = "value_book";
                cmbBook.SelectedIndex = 0;
            }

        }

        private void dgvDeed_DoubleClick(object sender, EventArgs e)
        {
            if (m_deed._GetDeed.PropertiesoutWB.Count > 0)
            {
                frmVolume._isOutsideWB = true;
            }
            if (m_deed._GetDeed.DeedHeader.Deed_control.Book == "3" || m_deed._GetDeed.DeedHeader.Deed_control.Book == "4")
            {
                frmVolume._isProperty = false;
                GrpProperty.Enabled = false;
                cmdAddwbProperty.Enabled = false;
            }
            else
            {
                frmVolume._isProperty = true;
                GrpProperty.Enabled = true;
                cmdAddwbProperty.Enabled = true;
            }
            Deed_details dd = new Deed_details(sqlCon,txn, crd, m_deed._GetDeed.DeedHeader, Mode._Edit, OnAccept, OnAbort_DeedDetails,m_deed._GetDeed.D_Excp,m_deed);
            dd.ShowDialog();
        }

        private void dgvIndex1_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                if (dgvIndex1.Rows.Count > 0)
                {
                    this._Person_Mode = Mode._Edit;
                    frmIndex1 indx1 = new frmIndex1(sqlCon, txn, crd, m_deed._GetDeed.Persons[dgvIndex1.CurrentRow.Index], Mode._Edit, OnAccept_Person, OnAbort_Person, m_deed._GetDeed.P_Excp, m_deed._GetDeed.Persons.Count + 1);
                    indx1.ShowDialog();
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void dgvIndex2_DoubleClick(object sender, EventArgs e)
        {

            try
            {
                if (dgvIndex2.Rows.Count >= 0)
                {



                    this._Property_Mode = Mode._Edit;
                    PropertyDetails p = m_deed._GetDeed.Properties[dgvIndex2.CurrentRow.Index];
                    frmIndex2 indx2 = new frmIndex2(sqlCon, txn, crd, ref p, Mode._Edit, Refreshgrid, OnAbort_Property, m_deed._GetDeed.Lst_other_plots, m_deed._GetDeed.Lst_other_khatians, m_deed._GetDeed.DeedHeader.hold, m_deed._GetDeed.Pro_Excp);
                    indx2.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                string xx = ex.Message.ToString();
            }
        }

        private void cmdSaveDeed_Click(object sender, EventArgs e)
        {
            frmVolume._isOutsideWB = false;
            if (m_deed._GetDeed.DeedHeader.Deed_control.Book == "3" && dgvIndex1.Rows.Count >= 1)
            {
                saveFlag = m_deed.SaveDeed();
                if (saveFlag == true)
                {
                    EventHandler<DeedEventArgs> CommitHandler = OnCommit;
                    if (CommitHandler != null)
                    {
                        DeedEventArgs tmpDeedEventArgs = new DeedEventArgs(m_deed._GetDeed);
                        OnCommit(this, tmpDeedEventArgs);
                    }
                    if (Commit)
                    {
                        txn.Commit();
                    }
                    MessageBox.Show(this, "Saved deed - " + m_deed._GetDeed.DeedHeader.Deed_control.ToString(), "Record Management", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Close();

                    nxt = frmDeeds.i;
                    nxt++;
                }
                else
                {
                    MessageBox.Show(this, "Ooops!!! There is an Error - Record not Saved...", " ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    EventHandler<DeedEventArgs> AbortHandler = OnAbort;
                    if (AbortHandler != null)
                    {
                        DeedEventArgs tmpDeedEventArgs = new DeedEventArgs(m_deed._GetDeed);
                        OnAbort(this, tmpDeedEventArgs);
                    }
                }
            }
            else if (m_deed._GetDeed.DeedHeader.Deed_control.Book == "4" && dgvIndex1.Rows.Count ==1)
            {
                saveFlag = m_deed.SaveDeed();
                if (saveFlag == true)
                {
                    EventHandler<DeedEventArgs> CommitHandler = OnCommit;
                    if (CommitHandler != null)
                    {
                        DeedEventArgs tmpDeedEventArgs = new DeedEventArgs(m_deed._GetDeed);
                        OnCommit(this, tmpDeedEventArgs);
                    }
                    if (Commit)
                    {
                        txn.Commit();
                    }
                    MessageBox.Show(this, "Saved deed - " + m_deed._GetDeed.DeedHeader.Deed_control.ToString(), "Record Management", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Close();

                    nxt = frmDeeds.i;
                    nxt++;
                }
                else
                {
                    MessageBox.Show(this, "Ooops!!! There is an Error - Record not Saved...", " ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    EventHandler<DeedEventArgs> AbortHandler = OnAbort;
                    if (AbortHandler != null)
                    {
                        DeedEventArgs tmpDeedEventArgs = new DeedEventArgs(m_deed._GetDeed);
                        OnAbort(this, tmpDeedEventArgs);
                    }
                }
            }
            //else
            //{
            //    MessageBox.Show(this, "Ooops!!! Error - Record not Saved...", " ", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //    EventHandler<DeedEventArgs> AbortHandler = OnAbort;
            //    if (AbortHandler != null)
            //    {
            //        DeedEventArgs tmpDeedEventArgs = new DeedEventArgs(m_deed._GetDeed);
            //        OnAbort(this, tmpDeedEventArgs);
            //    }
            //}

            else if (dgvIndex1.Rows.Count >= 1 && dgvIndex2.Rows.Count >= 1 && (m_deed._GetDeed.DeedHeader.Deed_control.Book == "1" || m_deed._GetDeed.DeedHeader.Deed_control.Book == "5"))
            {
                saveFlag = m_deed.SaveDeed();
                if (saveFlag == true)
                {
                    EventHandler<DeedEventArgs> CommitHandler = OnCommit;
                    if (CommitHandler != null)
                    {
                        DeedEventArgs tmpDeedEventArgs = new DeedEventArgs(m_deed._GetDeed);
                        OnCommit(this, tmpDeedEventArgs);
                    }
                    if (Commit)
                    {
                        txn.Commit();
                    }
                    MessageBox.Show(this, "Saved deed - " + m_deed._GetDeed.DeedHeader.Deed_control.ToString(), "Record Management", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Close();

                    nxt = frmDeeds.i;
                    nxt++;
                }
                else
                {
                    MessageBox.Show(this, "Ooops!!! There is an Error - Record not Saved...", " ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    EventHandler<DeedEventArgs> AbortHandler = OnAbort;
                    if (AbortHandler != null)
                    {
                        DeedEventArgs tmpDeedEventArgs = new DeedEventArgs(m_deed._GetDeed);
                        OnAbort(this, tmpDeedEventArgs);
                    }
                }
            }
            //You should enter minimum 1 Person Details and minimum 1 Property Details
            else
            {
                MessageBox.Show(this, "Ooops!!!  Record not Saved...", " ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                EventHandler<DeedEventArgs> AbortHandler = OnAbort;
                if (AbortHandler != null)
                {
                    DeedEventArgs tmpDeedEventArgs = new DeedEventArgs(m_deed._GetDeed);
                    OnAbort(this, tmpDeedEventArgs);
                }
            }
            
        }

        private void cmdaddperson_Click(object sender, EventArgs e)
        {
            //Set the mode tag to Add
            this._Person_Mode = Mode._Add;
            PersonDetails person = new PersonDetails();
            
            person.district_code = m_deed._GetDeed.DeedHeader.Deed_control.District_code;
            person.RO_code = m_deed._GetDeed.DeedHeader.Deed_control.RO_code;
            person.Book = m_deed._GetDeed.DeedHeader.Deed_control.Book;
            person.Deed_year = m_deed._GetDeed.DeedHeader.Deed_control.Deed_year;
            person.Deed_no = m_deed._GetDeed.DeedHeader.Deed_control.Deed_no; 
           
            frmIndex1 indx1 = new frmIndex1(sqlCon,txn,crd, person, Mode._Add, OnAccept_Person,OnAbort_Person,m_deed._GetDeed.P_Excp,m_deed._GetDeed.Persons.Count+1);
            indx1.ShowDialog();
        }

        private void cmdAddProperty_Click(object sender, EventArgs e)
        {
            this._Property_Mode = Mode._Add;
            PropertyDetails property = new PropertyDetails();
            if (dgvIndex2.Rows.Count > 0)
            {
                property = m_deed._GetDeed.Properties[(dgvIndex2.Rows.Count - 1)];
            }
            property.district_code = m_deed._GetDeed.DeedHeader.Deed_control.District_code;
            property.RO_code = m_deed._GetDeed.DeedHeader.Deed_control.RO_code;
            property.Book = m_deed._GetDeed.DeedHeader.Deed_control.Book;
            property.Deed_year = m_deed._GetDeed.DeedHeader.Deed_control.Deed_year;
            property.Deed_no = m_deed._GetDeed.DeedHeader.Deed_control.Deed_no;
            frmIndex2 indx2 = new frmIndex2(sqlCon, 
                txn, crd, ref property, Mode._Add, 
                Refreshgrid, OnAbort_Property, m_deed._GetDeed.Lst_other_plots, m_deed._GetDeed.Lst_other_khatians, 
                m_deed._GetDeed.DeedHeader.hold, m_deed._GetDeed.Pro_Excp, m_deed._GetDeed.Properties.Count + 1);
            indx2.ShowDialog();
        }
        private void Refreshgrid(PropertyDetails property)
        {
            //When a record add has been requested
            if (this._Property_Mode == Mode._Add)
            {
                //if (property.Serial == null || property.Serial.Trim().Length == 0)
                //{
                //    property.Serial = Convert.ToString(m_deed._GetDeed.Properties.Count + 1);
                //}
                m_deed._GetDeed.Properties.Add(property);
            }
            //When a current record is being edited
            if (this._Property_Mode == Mode._Edit)
            {
                int x = this.dgvIndex2.CurrentRow.Index;
                m_deed._GetDeed.Properties.RemoveAt(x);
                m_deed._GetDeed.Properties.Insert(x, property);
            }
            (dgvIndex2.BindingContext[m_deed._GetDeed.Properties] as CurrencyManager).Refresh();
            this.dgvIndex2.DataSource = null;
            this.dgvIndex2.DataSource = m_deed._GetDeed.Properties;
            this.dgvIndex2.Refresh();
            this.dgvPropertyExp.AutoGenerateColumns = true;
            (dgvPropertyExp.BindingContext[m_deed._GetDeed.Pro_Excp] as CurrencyManager).Refresh();
            this.dgvPropertyExp.DataSource = null;
            this.dgvPropertyExp.Refresh();
            this.dgvPropertyExp.DataSource = m_deed._GetDeed.Pro_Excp;
            
            dgvPropertyExp.Columns[0].Visible = false;
            dgvPropertyExp.Columns[1].Visible = false;
            dgvPropertyExp.Columns[2].Visible = false;
            dgvPropertyExp.Columns[3].Visible = false;
            dgvPropertyExp.Columns[4].Visible = false;
            dgvPropertyExp.Columns[6].Visible = false;
            //dgvPropertyExp.ColumnHeadersVisible = false;
        }

        private void dgvIndex1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                DialogResult dialogResult = MessageBox.Show("Do you Want to Delete the selected row", " ", MessageBoxButtons.YesNo,MessageBoxIcon.Warning);
                if (dialogResult == DialogResult.Yes)
                {
                    if (dgvIndex1.Rows.Count > 0)
                    {
                        int currentRow = dgvIndex1.CurrentRow.Index;
                        string idx = dgvIndex1.Rows[currentRow].Cells[0].Value.ToString();
                        m_deed._GetDeed.Persons.RemoveAt(currentRow);
                        foreach (var x in m_deed._GetDeed.P_Excp.ToList())
                        {
                            if (x.item_no.ToString().Equals(idx))
                            {
                                int idxn = m_deed._GetDeed.P_Excp.FindIndex(a => a.item_no.ToString().Equals(idx));
                                m_deed._GetDeed.P_Excp.RemoveAt(idxn);

                            }

                        }
                        (dgvPersonExp.BindingContext[m_deed._GetDeed.P_Excp] as CurrencyManager).Refresh();
                        this.dgvPersonExp.DataSource = null;
                        dgvPersonExp.DataSource = m_deed._GetDeed.P_Excp;
                        dgvPersonExp.Columns[0].Visible = false;
                        dgvPersonExp.Columns[1].Visible = false;
                        dgvPersonExp.Columns[2].Visible = false;
                        dgvPersonExp.Columns[3].Visible = false;
                        dgvPersonExp.Columns[4].Visible = false;
                        dgvPersonExp.Columns[6].Visible = false;
                        dgvIndex1.DataSource = null;
                        dgvIndex1.DataSource = m_deed._GetDeed.Persons;
                    }
                }
            }
        }

        private void dgvIndex2_KeyUp(object sender, KeyEventArgs e)
        {
            if (dgvIndex2.Rows.Count > 0)
            {
                if (e.KeyCode == Keys.Delete)
                {
                    DialogResult dialogResult = MessageBox.Show("Do you Want to Delete the selected row", " ", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (dialogResult == DialogResult.Yes)
                    {
                        if (dgvIndex2.Rows.Count > 0)
                        {
                            int currentRow = dgvIndex2.CurrentRow.Index;
                            string idx = dgvIndex2.Rows[currentRow].Cells[0].Value.ToString();
                            m_deed._GetDeed.Properties.RemoveAt(currentRow);
                            foreach (var x in m_deed._GetDeed.Pro_Excp.ToList())
                            {
                                if (x.item_no.ToString().Equals(idx))
                                {
                                    int idxn = m_deed._GetDeed.Pro_Excp.FindIndex(a => a.item_no.ToString().Equals(idx));
                                    m_deed._GetDeed.Pro_Excp.RemoveAt(idxn);

                                }

                            }
                            (dgvPropertyExp.BindingContext[m_deed._GetDeed.Pro_Excp] as CurrencyManager).Refresh();
                            this.dgvPropertyExp.DataSource = null;
                            dgvPropertyExp.DataSource = m_deed._GetDeed.Pro_Excp;
                            dgvPropertyExp.Columns[0].Visible = false;
                            dgvPropertyExp.Columns[1].Visible = false;
                            dgvPropertyExp.Columns[2].Visible = false;
                            dgvPropertyExp.Columns[3].Visible = false;
                            dgvPropertyExp.Columns[4].Visible = false;
                            dgvPropertyExp.Columns[6].Visible = false;
                            dgvIndex2.DataSource = null;
                            dgvIndex2.DataSource = m_deed._GetDeed.Properties;
                        }
                    }
                }
            }
        }

        private void frmDeedsummery_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.Z)
            {
                cmdaddperson_Click(this, e);
            }

            if (e.Alt && e.KeyCode == Keys.D)
            {
                    dgvDeed_DoubleClick(this, e);
            }
           
        }

        private void frmDeedsummery_Load(object sender, EventArgs e)
        {
            
            

            if (isOutsideWB)
            {
                groupBox1.Visible = true;
            }
            else
            {
                groupBox1.Visible = false;
            }
            this.panelForm.Top = 0;
            this.panelForm.Left = 0;
            this.panelForm.Size = this.Size;
            this.dgvPropertyExp.AutoGenerateColumns = true;
            this.dgvDeedExp.AutoGenerateColumns = true;
            this.dgvPersonExp.AutoGenerateColumns = true;
            if (_mode == Mode._Add)
            {
                Deed_details ds = new Deed_details(sqlCon,txn, crd, m_deed._GetDeed.DeedHeader, OnAccept, OnAbort_DeedDetails,m_deed._GetDeed.D_Excp);
                ds.ShowDialog();
            }
        }
        public void OnAccept(DeedDetails pdd)
        {
            m_deed._GetDeed.DeedHeader = pdd;

            DeedDetails ddList = new DeedDetails();

            ddList.Deed_control.District_code = m_deed._GetDeed.DeedHeader.Deed_control.District_code;//
            ddList.Deed_control.RO_code = m_deed._GetDeed.DeedHeader.Deed_control.RO_code;//
            ddList.Deed_control.Book = m_deed._GetDeed.DeedHeader.Deed_control.Book;//
            ddList.Deed_control.Deed_year = m_deed._GetDeed.DeedHeader.Deed_control.Deed_year;//
            ddList.Deed_control.Deed_no = m_deed._GetDeed.DeedHeader.Deed_control.Deed_no;//
            ddList.Serial_no = m_deed._GetDeed.DeedHeader.Serial_no;//
            ddList.Serial_year = m_deed._GetDeed.DeedHeader.Serial_year;//
            ddList.tran_maj_code = m_deed._GetDeed.DeedHeader.tran_maj_code;
            ddList.tran_min_code = m_deed._GetDeed.DeedHeader.tran_min_code;
            ddList.volume_no = m_deed._GetDeed.DeedHeader.volume_no;//    
            ddList.page_from = m_deed._GetDeed.DeedHeader.page_from;
            ddList.page_to = m_deed._GetDeed.DeedHeader.page_to;//
            ddList.date_of_completion = m_deed._GetDeed.DeedHeader.date_of_completion;
            ddList.date_of_delivery = m_deed._GetDeed.DeedHeader.date_of_delivery;
            ddList.deed_remarks = m_deed._GetDeed.DeedHeader.deed_remarks;
            ddList.doc_type = m_deed._GetDeed.DeedHeader.doc_type;//
            ddList.scan_doc_type = m_deed._GetDeed.DeedHeader.scan_doc_type;
            ddList.addl_pages = m_deed._GetDeed.DeedHeader.addl_pages;
            ddList.hold = m_deed._GetDeed.DeedHeader.hold;
            ddList.hold_reason = m_deed._GetDeed.DeedHeader.hold_reason;
            ddList.status = m_deed._GetDeed.DeedHeader.status;
            ddList.created_system = m_deed._GetDeed.DeedHeader.created_system;
            ddList.version = m_deed._GetDeed.DeedHeader.version;
           // ddList.exception = m_deed._GetDeed.DeedHeader.exception;
            m_deed._GetDeed.DeedHeader = ddList;
            plist.Clear();
            plist.Add(ddList);
            this.dgvDeed.DataSource = null;
            dgvDeed.DataSource = plist;
            dgvDeedExp.DataSource = null;
            dgvDeedExp.DataSource = m_deed._GetDeed.D_Excp;
            //dgvDeedExp.ColumnHeadersVisible = false;
            dgvDeedExp.Columns[0].Visible = false;
            dgvDeedExp.Columns[1].Visible = false;
            dgvDeedExp.Columns[2].Visible = false;
            dgvDeedExp.Columns[3].Visible = false;
            dgvDeedExp.Columns[4].Visible = false;
            dgvDeedExp.Columns[5].Visible = false;
            if (_mode == Mode._Add)
            {
                label1.Text = "Book Type : " + m_deed._GetDeed.DeedHeader.Deed_control.Book;
                dgvoutsideWBIndex2.DataSource = null;
                if (m_deed._GetDeed.PropertiesoutWB.Count > 0)
                {
                    dgvoutsideWBIndex2.DataSource = m_deed._GetDeed.PropertiesoutWB;
                }
                dgvIndex2.DataSource = null;
                if (m_deed._GetDeed.Properties.Count > 0)
                {
                    dgvIndex2.DataSource = m_deed._GetDeed.Properties;
                }
                dgvPropertyExp.DataSource = null;
                if (m_deed._GetDeed.Pro_Excp.Count > 0)
                {
                    dgvPropertyExp.DataSource = m_deed._GetDeed.Pro_Excp;
                }
            }
            if (_mode == Mode._Edit)
            {
                (dgvoutsideWBIndex2.BindingContext[dgvoutsideWBIndex2.DataSource] as CurrencyManager).Refresh();
                (dgvIndex2.BindingContext[dgvIndex2.DataSource] as CurrencyManager).Refresh();
                (dgvPropertyExp.BindingContext[dgvPropertyExp.DataSource] as CurrencyManager).Refresh();
                label1.Text = "Book Type : " + m_deed._GetDeed.DeedHeader.Deed_control.Book;
                if (m_deed._GetDeed.PropertiesoutWB.Count > 0)
                {
                    dgvoutsideWBIndex2.DataSource = null;
                    dgvoutsideWBIndex2.DataSource = m_deed._GetDeed.PropertiesoutWB;
                }
                
                if (m_deed._GetDeed.Properties.Count > 0)
                {
                    dgvIndex2.DataSource = null;
                    dgvIndex2.DataSource = m_deed._GetDeed.Properties;
                }
                
                if (m_deed._GetDeed.Pro_Excp.Count > 0)
                {
                    dgvPropertyExp.DataSource = null;
                    dgvPropertyExp.DataSource = m_deed._GetDeed.Pro_Excp;
                    dgvPropertyExp.Columns[0].Visible = false;
                    dgvPropertyExp.Columns[1].Visible = false;
                    dgvPropertyExp.Columns[2].Visible = false;
                    dgvPropertyExp.Columns[3].Visible = false;
                    dgvPropertyExp.Columns[4].Visible = false;
                    dgvPropertyExp.Columns[6].Visible = false;
                    //dgvPropertyExp.ColumnHeadersVisible = false;
                }
            }


            if (frmVolume._isOutsideWB == true)
            {
                cmdAddwbProperty.Enabled = false;
                cmdPropertyoutWB.Enabled = true;
            }
            else
            {
                cmdAddwbProperty.Enabled = true;
                cmdPropertyoutWB.Enabled = false;
            }
        }
        public void OnAccept_Person(PersonDetails pPerson)
        {
            //When a record add has been requested
            if (this._Person_Mode == Mode._Add)
            {
                m_deed._GetDeed.Persons.Add(pPerson);
            }
            //When a current record is being edited
            if (this._Person_Mode == Mode._Edit)
            {
                int x = this.dgvIndex1.CurrentRow.Index;
                m_deed._GetDeed.Persons.RemoveAt(x);
                m_deed._GetDeed.Persons.Insert(x, pPerson);
            }
            (dgvIndex1.BindingContext[m_deed._GetDeed.Persons] as CurrencyManager).Refresh();
            this.dgvIndex1.DataSource = null;
            this.dgvIndex1.DataSource = m_deed._GetDeed.Persons;
            this.dgvIndex1.Refresh();
            (dgvPersonExp.BindingContext[m_deed._GetDeed.P_Excp] as CurrencyManager).Refresh();
            this.dgvPersonExp.DataSource = null;
            this.dgvPersonExp.DataSource = m_deed._GetDeed.P_Excp;
            //dgvPersonExp.ColumnHeadersVisible = false;
            dgvPersonExp.Columns[0].Visible = false;
            dgvPersonExp.Columns[1].Visible = false;
            dgvPersonExp.Columns[2].Visible = false;
            dgvPersonExp.Columns[3].Visible = false;
            dgvPersonExp.Columns[4].Visible = false;
            dgvPersonExp.Columns[6].Visible = false;
        }
        public void OnAccept_PropertyWB(PropertyDetailsWB  pPropertyWB)
        {
            //When a record add has been requested
            if (this._PropertyWB_Mode == Mode._Add)
            {
                m_deed._GetDeed.PropertiesoutWB.Add(pPropertyWB);
            }
            //When a current record is being edited
            if (this._PropertyWB_Mode == Mode._Edit)
            {
                int x = this.dgvoutsideWBIndex2.CurrentRow.Index;
                m_deed._GetDeed.PropertiesoutWB.RemoveAt(x);
                m_deed._GetDeed.PropertiesoutWB.Insert(x, pPropertyWB);
            }
            (dgvoutsideWBIndex2.BindingContext[m_deed._GetDeed.PropertiesoutWB] as CurrencyManager).Refresh();
            this.dgvoutsideWBIndex2.DataSource = null;
            this.dgvoutsideWBIndex2.DataSource = m_deed._GetDeed.PropertiesoutWB;
            this.dgvoutsideWBIndex2.Refresh();
            //this.dgvPropertyExp.DataSource = null;
            //this.dgvPropertyExp.DataSource = m_deed._GetDeed.Pro_Excp;
            //this.dgvPropertyExp.Refresh();
            //dgvPropertyExp.Columns[0].Visible = false;
            //dgvPropertyExp.Columns[1].Visible = false;
            //dgvPropertyExp.Columns[2].Visible = false;
            //dgvPropertyExp.Columns[3].Visible = false;
            //dgvPropertyExp.Columns[4].Visible = false;
            //dgvPropertyExp.ColumnHeadersVisible = false;
        }
        public void OnAbort_DeedDetails()
        {
            //In case of new entry a transaction is being passed and we should get it closed before we close
            //this form
            if (_mode == Mode._Add)
            {
                txn.Rollback();
                this.Close();
            }
        }
        public void OnAbort_Property()
        {
            MessageBox.Show(this, "No changes made in Property Details", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        public void OnAbort_PropertyWB()
        {
           // dgvPropertyExp.DataSource = null;
            //dgvPropertyExp.DataSource = m_deed._GetDeed.Pro_Excp;
            MessageBox.Show(this, "No changes made in Property Details outside WB", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        public void OnAbort_Person()
        {
            MessageBox.Show(this, "No changes made in Party Details", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void cmdAbort_Click(object sender, EventArgs e)
        {
            DialogResult retVal = MessageBox.Show(this, "You sure want to abort?", "Warning you!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);
            if (retVal == DialogResult.Yes)
            {
            	EventHandler<DeedEventArgs> AbortHandler = OnAbort;
            	if (AbortHandler != null)
            	{
            		DeedEventArgs tmpDeedEventArgs = new DeedEventArgs(m_deed._GetDeed);
            		OnAbort(this, tmpDeedEventArgs);
            	}
				if (Commit)
				{
                	txn.Rollback();
				}
                frmVolume._isOutsideWB = false;
                this.Close();
            }
        }
        //public delegate void CommitHandler(object _formObject, DeedEventArgs _deedArgs);
        public event EventHandler<DeedEventArgs> OnCommit;
        public event EventHandler<DeedEventArgs> OnAbort;

        private void GrpProperty_Enter(object sender, EventArgs e)
        {

        }

        private void deButton1_Click(object sender, EventArgs e)
        {
            this._PropertyWB_Mode = Mode._Add;
            PropertyDetailsWB propertyWB = new PropertyDetailsWB();
            outSideWBList pExp = new outSideWBList();
            propertyWB.district_code = m_deed._GetDeed.DeedHeader.Deed_control.District_code;
            propertyWB.RO_code = m_deed._GetDeed.DeedHeader.Deed_control.RO_code;
            propertyWB.Book = m_deed._GetDeed.DeedHeader.Deed_control.Book;
            propertyWB.Deed_year = m_deed._GetDeed.DeedHeader.Deed_control.Deed_year;
            propertyWB.Deed_no = m_deed._GetDeed.DeedHeader.Deed_control.Deed_no;

            frmIndex2outsideWB outwb = new frmIndex2outsideWB(sqlCon, txn, crd, propertyWB, Mode._Add, OnAccept_PropertyWB, OnAbort_PropertyWB, m_deed._GetDeed.DeedHeader.Deed_control,m_deed._GetDeed.Pro_ExcpWb);
            outwb.ShowDialog();
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void grpMain_Paint(object sender, PaintEventArgs e)
        {
            
        }

        private void deDataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dgvPropertyExp_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                DialogResult dialogResult = MessageBox.Show("Do you Want to Remove the selected Exception", "Property Exception ", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (dialogResult == DialogResult.Yes)
                {
                    if (dgvPropertyExp.Rows.Count > 0)
                    {
                        int currentRow = dgvPropertyExp.CurrentRow.Index;
                        m_deed._GetDeed.Pro_Excp.RemoveAt(currentRow);
                        dgvPropertyExp.DataSource = null;
                        dgvPropertyExp.DataSource = m_deed._GetDeed.Pro_Excp;
                    }
                }
            }
        }

        private void deDataGridView1_CellContentClick_1(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dgvPersonExp_KeyUp(object sender, KeyEventArgs e)
        {
            //if (e.KeyCode == Keys.Delete)
            //{
            //    DialogResult dialogResult = MessageBox.Show("Do you Want to Remove the selected Exception", "Person Exception ", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            //    if (dialogResult == DialogResult.Yes)
            //    {
            //        if (dgvPersonExp.Rows.Count > 0)
            //        {
            //            int currentRow = dgvPersonExp.CurrentRow.Index;
            //            m_deed._GetDeed.P_Excp.RemoveAt(currentRow);
            //            dgvPersonExp.DataSource = null;
            //            dgvPersonExp.DataSource = m_deed._GetDeed.P_Excp;
            //        }
            //    }
            //}
        }

        private void dgvDeedExp_KeyUp(object sender, KeyEventArgs e)
        {
            //if (e.KeyCode == Keys.Delete)
            //{
            //    DialogResult dialogResult = MessageBox.Show("Do you Want to Remove the selected Exception", "Deed Exception ", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            //    if (dialogResult == DialogResult.Yes)
            //    {
            //        if (dgvDeedExp.Rows.Count > 0)
            //        {
            //            int currentRow = dgvDeedExp.CurrentRow.Index;
            //            m_deed._GetDeed.D_Excp.RemoveAt(currentRow);
            //            dgvDeedExp.DataSource = null;
            //            dgvDeedExp.DataSource = m_deed._GetDeed.D_Excp;
            //        }
            //    }
            //}
        }

        private void dgvoutsideWBIndex2_DoubleClick(object sender, EventArgs e)
        {
            if (dgvoutsideWBIndex2.Rows.Count > 0 )
            {
                this._PropertyWB_Mode = Mode._Edit;
                frmIndex2outsideWB indx1 = new frmIndex2outsideWB(sqlCon, txn, crd, m_deed._GetDeed.PropertiesoutWB[dgvoutsideWBIndex2.CurrentRow.Index], Mode._Edit, OnAccept_PropertyWB, OnAbort_PropertyWB, m_deed._GetDeed.DeedHeader.Deed_control, m_deed._GetDeed.Pro_ExcpWb);
                indx1.ShowDialog();
            }
        }

        private void dgvPersonExp_DataError(object sender, DataGridViewDataErrorEventArgs e)
        
    {
        e.Cancel = true;
        e.ThrowException = false;            
    
    }

        private void dgvPropertyExp_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.Cancel = true;
            e.ThrowException = false;  
        }

        private void dgvIndex2_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.Cancel = true;
            e.ThrowException = false;  
        }

        private void dgvIndex1_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.Cancel = true;
            e.ThrowException = false;  
        }

        private void frmDeedsummery_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.Z)
            {
                cmdaddperson_Click(this, e);
            }
        }

        private void frmDeedsummery_KeyPress(object sender, KeyPressEventArgs e)
        {
           
        }

        private void dgvDeed_Enter(object sender, EventArgs e)
        {
            dgvDeed_DoubleClick(this, e);
        }
        
        
    }
}
