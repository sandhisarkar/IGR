using System;
using System.Drawing;
using System.Windows.Forms;
using NovaNet.Utils;
using System.Data;
using System.Data.Odbc;
using LItems;
using NovaNet.wfe;
using System.IO;
using batchManagement;

namespace ImageHeaven
{
    public partial class frmBatchRename : Form
    {
        NovaNet.Utils.dbCon dbcon;
        //Credentials udtCrd;
        ControlInfo udtInfo;
        MemoryStream stateLog;
        byte[] tmpWrite;
        OdbcConnection sqlCon = null;
        Credentials crd = new Credentials();
        public frmBatchRename(OdbcConnection prmCon, Credentials prmCrd)
        {
            InitializeComponent();
            sqlCon = prmCon;
            this.Text = "B'Zer - Batch Renamer";
            crd = prmCrd;
        }

        private void frmBatchRename_Load(object sender, EventArgs e)
        {
            PopulateProjectCombo();
            PopulateBatchCombo();
        }

        private void cmdCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void PopulateProjectCombo()
        {
            DataSet ds = new DataSet();

            dbcon = new NovaNet.Utils.dbCon();

            wfeProject tmpProj = new wfeProject(sqlCon);
            //cmbProject.Items.Add("Select");
            ds = tmpProj.GetAllValues();
            if (ds.Tables[0].Rows.Count > 0)
            {
                cmbProject.DataSource = ds.Tables[0];
                cmbProject.DisplayMember = ds.Tables[0].Columns[1].ToString();
                cmbProject.ValueMember = ds.Tables[0].Columns[0].ToString();
            }
        }
        void CmbProjectLeave(object sender, EventArgs e)
        {
            PopulateBatchCombo();
        }
        private void PopulateBatchCombo()
        {
            string projKey = null;
            DataSet ds = new DataSet();

            dbcon = new NovaNet.Utils.dbCon();

            wfeBatch tmpBatch = new wfeBatch(sqlCon);
            if (cmbProject.SelectedValue != null)
            {
                projKey = cmbProject.SelectedValue.ToString();
                ds = tmpBatch.GetAllValues(Convert.ToInt32(projKey));
                cmbBatch.DataSource = ds.Tables[0];
                cmbBatch.DisplayMember = ds.Tables[0].Columns[1].ToString();
                cmbBatch.ValueMember = ds.Tables[0].Columns[0].ToString();
            }	//cmbBatch.Items.Insert(0,"Select");
        }

        private void cmdRem_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult dlg;
                dlg = MessageBox.Show(this, "You are going to rename batch are you sure ?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);
                if (dlg == DialogResult.Yes)
                {
                    DialogResult dlgconfirm;
                    dlgconfirm = MessageBox.Show(this, "Transaction cannot be rolllback once renamed, are you sure ?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);
                    if (dlg == DialogResult.Yes)
                    {
                        ///////
                        batchRenamer batrem = new batchRenamer(Convert.ToInt32(cmbBatch.SelectedValue.ToString()), txtNewBatch.Text.Trim(), sqlCon);

                        //////
                    }
                    else
                    {
                        MessageBox.Show(this, "Aborted by user", "Aborting");
                        return;
                    }

                }
                else
                {
                    MessageBox.Show(this, "Aborted by user", "Aborting");
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }
    }
}
