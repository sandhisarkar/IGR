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
using System.Collections;

namespace IGRFqc
{
    public partial class frmVolume : Form
    {
        List<Volumes> volumes = new List<Volumes>();
        List<Volumes> temp = new List<Volumes>();
        public static string deed_vol = string.Empty;
        public static bool _isOutsideWB = false;
        public static bool _isProperty = true;
        public static string deed_year = string.Empty;
        public static string deed_book = string.Empty;
        Credentials crd = new Credentials();
        private OdbcConnection sqlCon;
        OdbcTransaction txn;
        private PopulateCombo pCom;
        
        public frmVolume(OdbcConnection prmCon,Credentials prmCrd)
        {
            InitializeComponent();
            sqlCon = prmCon;
            crd = prmCrd;
            init();
        }
        private void cmbDistrict_Leave(object sender, EventArgs e)
        {
            if (cmbDistrict.DataSource != null)
            {
                string districtCode = cmbDistrict.SelectedValue.ToString();
                pCom = new PopulateCombo(sqlCon,txn, crd);
                cmbRo.DataSource = pCom.GetROffice(districtCode).Tables[0];
                cmbRo.DisplayMember = "RO_name";
                cmbRo.ValueMember = "RO_code";
            }
        }
        private void init()
        {
            pCom = new PopulateCombo(sqlCon,txn, crd);
            if (pCom.GetDistrict_Active().Tables[0].Rows.Count > 0)
            {
                cmbDistrict.DataSource = pCom.GetDistrict_Active().Tables[0];
                cmbDistrict.DisplayMember = "district_name";
                cmbDistrict.ValueMember = "district_code";
                cmbDistrict.SelectedIndex = 0;
            }

            volumes = igr_base.igr_deed.GetAllVolumes(sqlCon,txn,crd);
            //var query = from v in volumes
            //            where v.Year == "2008"
            //            select v;
            //List<Volumes> temp = query.ToList();
             dtGrdVol.DataSource = volumes;

             FormatDataGridView();
            //if (volumes.Count > 0)
            //    dtGrdVol.DataSource = volumes;
            
            DataSet ds = pCom.GetYear();
            if (ds.Tables[0].Rows.Count>0)
            {
            //cmbDeedYear.DataSource = ds.Tables[0];
            //cmbDeedYear.DisplayMember = "deed_year";
            //cmbDeedYear.ValueMember = "deed_year";
            //cmbDeedYear.SelectedIndex = 0;
            }
            DataSet dsBook = pCom.GetBookType();
            if (dsBook.Tables[0].Rows.Count > 0)
            {
               // cmbBook.DataSource = pCom.GetBookType().Tables[0];
                //cmbBook.DisplayMember = "key_book";
                //cmbBook.ValueMember = "value_book";
                //cmbBook.SelectedIndex = 0;
            }
            this.dtGrdVol.Refresh();
            this.cmbDeedYear.Text = "";
            this.cmbBook.Text = "";
            this.cmbVolume.Text = "";
            this.cmbDeedYear.Focus();

        }

        private void cmbBook_Leave(object sender, EventArgs e)
        {
            //cmbVolume.DataSource = pCom.GetVol(cmbDistrict.SelectedValue.ToString(), cmbRo.SelectedValue.ToString(), cmbBook.SelectedValue.ToString(), cmbDeedYear.Text).Tables[0];
            //cmbVolume.DisplayMember = "Volume_No";
            //cmbVolume.ValueMember = "Volume_No";
            //cmbVolume.SelectedIndex = 0;
        }

        private void cmdSearch_Click(object sender, EventArgs e)
        {

            if (cmbDeedYear.Text != "" && cmbBook.Text != "" && cmbVolume.Text != "")
            {
                temp.Clear();
                var query = from v in volumes
                            where
                            v.Year == cmbDeedYear.Text
                            && v.Book == cmbBook.Text
                            && v.Volume == cmbVolume.Text
                            select v;
                temp = query.ToList();
                dtGrdVol.DataSource = null;
                dtGrdVol.Rows.Clear();
                dtGrdVol.DataSource = temp;
            }
            else if (cmbDeedYear.Text == "" && cmbBook.Text != "" && cmbVolume.Text != "")
            {
                temp.Clear();
                var query = from v in volumes
                            where
                            v.Book == cmbBook.Text
                            && v.Volume == cmbVolume.Text
                            select v;
                temp = query.ToList();
                dtGrdVol.DataSource = null;
                dtGrdVol.Rows.Clear();
                dtGrdVol.DataSource = temp;
            }
            else if (cmbDeedYear.Text != "" && cmbBook.Text == "" && cmbVolume.Text != "")
            {
                temp.Clear();
                var query = from v in volumes
                            where
                            v.Year == cmbDeedYear.Text
                            && v.Volume == cmbVolume.Text
                            select v;
                 temp = query.ToList();
                dtGrdVol.DataSource = null;
                dtGrdVol.Rows.Clear();
                dtGrdVol.DataSource = temp;
            }
            else if (cmbDeedYear.Text == "" && cmbBook.Text == "" && cmbVolume.Text != "")
            {
                temp.Clear(); 
                var query = from v in volumes
                            where

                            v.Volume == cmbVolume.Text
                            select v;
                 temp = query.ToList();
                dtGrdVol.DataSource = null;
                dtGrdVol.Rows.Clear();
                dtGrdVol.DataSource = temp;
            }
            else if (cmbDeedYear.Text != "" && cmbBook.Text != "" && cmbVolume.Text == "")
            {
                temp.Clear();
                var query = from v in volumes
                            where
                            v.Year == cmbDeedYear.Text
                            && v.Book == cmbBook.Text
                            select v;
                 temp = query.ToList();
                dtGrdVol.DataSource = null;
                dtGrdVol.Rows.Clear();
                dtGrdVol.DataSource = temp;
            }
            else if (cmbDeedYear.Text == "" && cmbBook.Text != "" && cmbVolume.Text == "")
            {
                temp.Clear(); 
                var query = from v in volumes
                            where
                            v.Book == cmbBook.Text

                            select v;
                temp = query.ToList();
                dtGrdVol.DataSource = null;
                dtGrdVol.Rows.Clear();
                dtGrdVol.DataSource = temp;
            }
            else if (cmbDeedYear.Text != "" && cmbBook.Text == "" && cmbVolume.Text == "")
            {
                temp.Clear();
                var query = from v in volumes
                            where
                            v.Year == cmbDeedYear.Text

                            select v;
                 temp = query.ToList();
                dtGrdVol.DataSource = null;
                dtGrdVol.Rows.Clear();
                dtGrdVol.DataSource = temp;
            }
            else if (cmbDeedYear.Text == "" && cmbBook.Text == "" && cmbVolume.Text == "")
            {
                dtGrdVol.DataSource = null;
                dtGrdVol.Rows.Clear();
                dtGrdVol.DataSource = volumes;

            }
            else
            {
                dtGrdVol.DataSource = null;
                dtGrdVol.Rows.Clear();
            }
            FormatDataGridView();
            dtGrdVol.Focus();
        }

        private void dtGrdVol_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            List<DeedControl> pVol = new List<DeedControl>();
            Volumes nVol = new Volumes();
            nVol.District_Code = dtGrdVol.CurrentRow.Cells[0].Value.ToString();
            nVol.Ro_Code = dtGrdVol.CurrentRow.Cells[2].Value.ToString();
            nVol.Book = dtGrdVol.CurrentRow.Cells[5].Value.ToString();
            nVol.Year = dtGrdVol.CurrentRow.Cells[4].Value.ToString();
            nVol.Volume = dtGrdVol.CurrentRow.Cells[6].Value.ToString();

            pVol = igr_base.igr_deed.GetDeedsByVolume(sqlCon,txn, crd, nVol);
            frmDeeds frmd = new frmDeeds(sqlCon, crd, pVol,nVol.Volume);
            frmd.ShowDialog();
        }

        private void frmVolume_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                this.Close();
        }
        NovaNet.Utils.GetProfile pData;
        NovaNet.Utils.ChangePassword pCPwd;
        NovaNet.Utils.Profile p;
        dbCon dbcon;
        public static NovaNet.Utils.IntrRBAC rbc;
        private void frmVolume_Load(object sender, EventArgs e)
        {
            this.panelForm.Top = 0;
            this.panelForm.Left = 0;
            this.panelForm.Size = this.Size;
            //pData = getData;
            //pCPwd = getCPwd;
            ////dbcon = new NovaNet.Utils.dbCon();
            ////rbc = new NovaNet.Utils.RBAC(sqlCon, dbcon, pData, pCPwd);
            //////GetChallenge gc = new GetChallenge(getData);
            //////gc.ShowDialog(this);
            ///////get credential for the logged user
            ////crd = rbc.getCredentials(p);
            ////if (crd.created_by == null) { this.Close(); }
            ////if (crd.role == "Admin") { lblDeedCount.Visible = true; lblDeedCount.Visible = true; }
            ////else { lblDeedCount.Visible = false; }
            /*Testing*/
            //frmSuggession x = new frmSuggession(conn, crd);
            //x.ShowDialog();
            //x.Activate();
            /*End: Testing*/
            ArrayList lst = GetTotalDaily(crd);
            for (int i = 0; i < lst.Count; i++)
            {
                lblDeedCount.Text = "Today You Have Done: " + lst[0].ToString() + " Deeds";
            }
        }

        public ArrayList GetTotalDaily(NovaNet.Utils.Credentials crd)
        {
            ArrayList totList = new ArrayList();
            string sql = "Select district_code from deed_details where date_format(created_DTTM,'%Y-%m-%d')=date_format(now(),'%Y-%m-%d') and created_by = '" + crd.created_by + "'";
            //string sql = "Select district_code from deed_details where created_DTTM like now() and created_by = '" + crd.created_by + "'";
            DataSet ds = new DataSet();
            OdbcDataAdapter odap = new OdbcDataAdapter(sql, sqlCon);
            odap.Fill(ds);
            if (ds.Tables.Count > 0)
            { totList.Add(ds.Tables[0].Rows.Count); }
            else { totList.Add("0"); }



            return totList;
        }

        private void FormatDataGridView()
        {
            //Format the Data Grid View
            dtGrdVol.Columns[0].Visible = false;
            dtGrdVol.Columns[2].Visible = false;
            //Format Colors
            
            //Set Autosize on for all the columns
            for (int i = 0; i < dtGrdVol.Columns.Count; i++)
            {
                dtGrdVol.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            }
        }

        private void dtGrdVol_DoubleClick(object sender, EventArgs e)
        {
            
            if (dtGrdVol.SelectedRows.Count > 0)
            {
                List<DeedControl> pVol = new List<DeedControl>();
                Volumes nVol = new Volumes();
                nVol.District_Code = dtGrdVol.CurrentRow.Cells[0].Value.ToString();
                nVol.Ro_Code = dtGrdVol.CurrentRow.Cells[2].Value.ToString();
                nVol.Book = dtGrdVol.CurrentRow.Cells[5].Value.ToString();
                nVol.Year = dtGrdVol.CurrentRow.Cells[4].Value.ToString();
                nVol.Volume = dtGrdVol.CurrentRow.Cells[6].Value.ToString();

                pVol = igr_base.igr_deed.GetDeedsByVolume(sqlCon,txn,crd, nVol);
                frmDeeds frmd = new frmDeeds(sqlCon, crd, pVol, nVol.Volume);
                frmd.ShowDialog();
            }
             
        }

        private void cmdnew_Click(object sender, EventArgs e)
        {
            txn = sqlCon.BeginTransaction();
            igr_deed _mdeed = new igr_deed(sqlCon, txn ,crd);
            frmDeedsummery ds = new frmDeedsummery(sqlCon, txn , crd, _mdeed,Mode._Add);
            ds.ShowDialog();
        }

        private void cmdReset_Click(object sender, EventArgs e)
        {
            init();
        }

        private void panelForm_Paint(object sender, PaintEventArgs e)
        {

        }

        
     }
}
