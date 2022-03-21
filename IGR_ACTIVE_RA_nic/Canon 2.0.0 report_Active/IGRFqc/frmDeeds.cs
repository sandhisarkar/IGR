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
    public partial class frmDeeds : Form
    {
        public static int i;

        NovaNet.Utils.dbCon dbcon = new NovaNet.Utils.dbCon();
        List<Volumes> volumes = new List<Volumes>();
        List<Volumes> temp = new List<Volumes>();
        List<DeedControl> pList = new List<DeedControl>();
        Credentials crd = new Credentials();
        private OdbcConnection sqlCon;
        OdbcTransaction txn;
        private PopulateCombo pCom = null;
        string volume_no = string.Empty;
        //To open the deed summary page on Double Clicking the list
        frmDeedsummery ds;
        

        public frmDeeds()
        {
            InitializeComponent();
            this.txtdeedSearch.Focus();
        }
        public frmDeeds(OdbcConnection prmCon, Credentials prmCrd,List<DeedControl> _pList, string pVolume_no)
        {
            InitializeComponent();
            sqlCon = prmCon;
            crd = prmCrd;
            pList = _pList;
            volume_no = pVolume_no;

            for (int i = 0; i < pList.Count; i++)
            {
                lstDeeds.Items.Add(pList[i].Deed_no.ToString());
                lblBook.Text = pList[i].Book.ToString();
                lblDeedYr.Text = pList[i].Deed_year.ToString();
                lblVol.Text = pVolume_no;

                string policy_num = pList[i].District_code.ToString() + pList[i].RO_code.ToString() + pList[i].Book.ToString() + pList[i].Deed_year.ToString() +"["+ lblVol.Text+"]";

                DataTable dt = new DataTable();
                string sql = "select policy_number from policy_master where  do_code = '" + pList[i].District_code.ToString() + "' and br_code = '" + pList[i].RO_code.ToString() + "' and year = '" + pList[i].Book.ToString() + "' and deed_year = '" + pList[i].Deed_year.ToString() + "' and deed_no = '" + pList[i].Deed_no.ToString() + "' and deed_vol = '" + lblVol.Text + "'";
                OdbcCommand cmd = new OdbcCommand(sql, sqlCon, txn);
                OdbcDataAdapter odap = new OdbcDataAdapter(cmd);
                odap.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    updateDeedToolStripMenuItem.Enabled = false;
                    deleteDeedToolStripMenuItem.Enabled = false;
                }
                else
                {
                    updateDeedToolStripMenuItem.Enabled = true;
                    deleteDeedToolStripMenuItem.Enabled = true;
                }
            }
            this.txtdeedSearch.Focus();

        }

        private void frmDeeds_Load(object sender, EventArgs e)
        {
            this.panelForm.Top = 0;
            this.panelForm.Left = 0;
            this.panelForm.Size = this.Size;


           
        }

        private void frmDeeds_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
            
        }

        private void lstDeeds_DoubleClick_1(object sender, EventArgs e)
        {
            i = lstDeeds.FocusedItem.Index;
            DeedControl pdc = new DeedControl();
            pdc.Book = pList[i].Book;
            pdc.Deed_no = pList[i].Deed_no;
            pdc.Deed_year = pList[i].Deed_year;
            pdc.District_code = pList[i].District_code;
            pdc.RO_code = pList[i].RO_code;
            txn = sqlCon.BeginTransaction();
            igr_deed igr = new igr_deed(sqlCon,txn, crd, pdc);
            ds = new frmDeedsummery(sqlCon,txn, crd, igr);
            //ds.OnCommit += new EventHandler<DeedEventArgs>(OnAbortClick);
            //ds.OnAbort += new EventHandler<DeedEventArgs>(OnAbortClick);
            //this.lstDeeds.MouseClick += new System.Windows.Forms.MouseEventHandler(this.lstDeeds_MouseClick);
            //ds.OnAbort
            ds.ShowDialog();


            i = frmDeedsummery.nxt;

            if ((lstDeeds.Items.Count -1) >= frmDeedsummery.nxt)
            {
                lstDeeds.Items[frmDeedsummery.nxt].Selected = true;
                lstDeeds.Select();
                lstDeeds.Items[frmDeedsummery.nxt].EnsureVisible();
                
            }
            else
            {
                MessageBox.Show("Deed end for this volume, control is now ready again to indicate the starting position");
                lstDeeds.Items[0].Selected = true;
                lstDeeds.Select();
                lstDeeds.Items[0].EnsureVisible();
            }
            
        }
        private void OnAbortClick(object sender, DeedEventArgs pDeedArgs)
        {
        	//MessageBox.Show(pDeedArgs._Deed.DeedHeader.page_from.ToString());
        }
        private void cmdSearch_Click_1(object sender, EventArgs e)
        {
            string text = txtdeedSearch.Text.PadLeft(5, '0');

            for (int i = 0; i < lstDeeds.Items.Count; i++)
            {
                if (lstDeeds.Items[i].Text.Equals(text))
                {
                    lstDeeds.Items[i].Selected = true;
                    lstDeeds.Select();
                    lstDeeds.Items[i].EnsureVisible();
                    return;
                }
                else
                {
                    lstDeeds.Items[i].Selected = false;
                }

            }
        }

        private void lstDeeds_MouseClick(object sender, MouseEventArgs e)
        {
           
        if (e.Button == MouseButtons.Right)
        {
            if (lstDeeds.FocusedItem.Bounds.Contains(e.Location) == true)
            {
                cmsDeeds.Show(Cursor.Position);
            }
        } 

        }

        public void OnAccept_Delete()
        {
            int i = lstDeeds.FocusedItem.Index;
            pList.RemoveAt(i);
            lstDeeds.Items.RemoveAt(i);
            lstDeeds.Refresh();
        }
        public void OnAbort_Delete()
        {

        }
        public void OnAccept_Update()
        {

        }
        public void OnAbort_Update()
        {

        }
        private void updateDeedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int i = lstDeeds.FocusedItem.Index;
            DeedControl pdc = new DeedControl();
            pdc.Book = pList[i].Book;
            pdc.Deed_no = pList[i].Deed_no;
            pdc.Deed_year = pList[i].Deed_year;
            pdc.District_code = pList[i].District_code;
            pdc.RO_code = pList[i].RO_code;
            frmdeedControl frmdc = new frmdeedControl(sqlCon,crd,pdc,Mode._Rename,volume_no,OnAccept_Update,OnAbort_Update);
            frmdc.ShowDialog(this);
        }

        private void deleteDeedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int i = lstDeeds.FocusedItem.Index;
            DeedControl pdc = new DeedControl();
            pdc.Book = pList[i].Book;
            pdc.Deed_no = pList[i].Deed_no;
            pdc.Deed_year = pList[i].Deed_year;
            pdc.District_code = pList[i].District_code;
            pdc.RO_code = pList[i].RO_code;
            frmdeedControl frmdc = new frmdeedControl(sqlCon, crd, pdc, Mode._Delete, volume_no,OnAccept_Delete, OnAbort_Delete);
            frmdc.ShowDialog(this);
        }

        private void lstDeeds_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void lstDeeds_Enter(object sender, EventArgs e)
        {
            
        }

        private void lstDeeds_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.D)
            {

                if(lstDeeds.SelectedItems.Count > 0)
                {
                    int cou;
                    for (cou = 0; cou <= lstDeeds.Items.Count - 1; cou++)
                    {
                        i = cou;
                        if (lstDeeds.Items[i].Selected == true)
                        {
                            
                            DeedControl pdc = new DeedControl();
                            pdc.Book = pList[i].Book;
                            pdc.Deed_no = pList[i].Deed_no;
                            pdc.Deed_year = pList[i].Deed_year;
                            pdc.District_code = pList[i].District_code;
                            pdc.RO_code = pList[i].RO_code;
                            txn = sqlCon.BeginTransaction();
                            igr_deed igr = new igr_deed(sqlCon, txn, crd, pdc);
                            ds = new frmDeedsummery(sqlCon, txn, crd, igr);
                            //ds.OnCommit += new EventHandler<DeedEventArgs>(OnAbortClick);
                            //ds.OnAbort += new EventHandler<DeedEventArgs>(OnAbortClick);
                            //this.lstDeeds.MouseClick += new System.Windows.Forms.MouseEventHandler(this.lstDeeds_MouseClick);
                            //ds.OnAbort
                            ds.ShowDialog();


                            i = frmDeedsummery.nxt;

                            if ((lstDeeds.Items.Count - 1) >= frmDeedsummery.nxt)
                            {
                                lstDeeds.Items[frmDeedsummery.nxt].Selected = true;
                                lstDeeds.Select();
                                lstDeeds.Items[frmDeedsummery.nxt].EnsureVisible();
                                break;
                            }
                            else
                            {
                                MessageBox.Show("Deed end for this volume, control is now ready again to indicate the starting position");
                                lstDeeds.Items[0].Selected = true;
                                lstDeeds.Select();
                                lstDeeds.Items[0].EnsureVisible();
                                break;
                            }
                            break;
                        }
                    }
                }

                
            }
        }

    }
}
