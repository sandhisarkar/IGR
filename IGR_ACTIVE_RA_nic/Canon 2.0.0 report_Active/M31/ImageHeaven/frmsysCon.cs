using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using igr_base;
using System.Collections;
using NovaNet.Utils;
using System.Data.Odbc;
using System.Data.OleDb;
using IGRFqc;

namespace ImageHeaven
{
    
    public partial class frmsysCon : Form
    {
        Credentials crd = new Credentials();
        //private DataLayer dly;
        OdbcConnection sqlCon = new OdbcConnection();
        private PopulateCombo pCom;
        public frmsysCon(OdbcConnection prmCon, Credentials prmCrd)
        {
            InitializeComponent();
            sqlCon = prmCon;
            crd = prmCrd;
            pCom = new PopulateCombo(sqlCon, null, crd);
            populate_combo();
        }

        private void frmsysCon_Load(object sender, EventArgs e)
        {
            _Show();
        }
        private void _Show()
        {
            try
            {
                string sql = "select a.district_name,b.ro_name from district a, ro_master b where a.district_code = b.district_code and b.active = 'Y'";
                DataSet ds = new DataSet();
                OdbcDataAdapter odap = new OdbcDataAdapter(sql, sqlCon);
                odap.Fill(ds);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    LblDis.Text = ds.Tables[0].Rows[0][0].ToString();
                    lblRO.Text = ds.Tables[0].Rows[0][1].ToString();
                    cmbDis.Text = ds.Tables[0].Rows[0][0].ToString();
                    cmbWhereReg.Text = ds.Tables[0].Rows[0][0].ToString();
                    btnSave.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }
        private void populate_combo()
        {
            cmbDis.DataSource = pCom.GetDistrict().Tables[0];
            if (pCom.GetDistrict().Tables[0].Rows.Count > 0)
            {
                cmbDis.DisplayMember = "district_name";
                cmbDis.ValueMember = "district_code";
                cmbDis.SelectedIndex = 0;
            }
         }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                string qry = "update district set active = 'Y' where district_code = '" + cmbDis.SelectedValue.ToString() + "'";
                OdbcCommand cmd = new OdbcCommand(qry, sqlCon);
                cmd.ExecuteNonQuery();

                string qry1 = "update ro_master set active = 'Y' where district_code = '" + cmbDis.SelectedValue.ToString() + "' and ro_code = '" + cmbWhereReg.SelectedValue.ToString() + "'";
                OdbcCommand cmd1 = new OdbcCommand(qry1, sqlCon);
                cmd1.ExecuteNonQuery();
                MessageBox.Show("Data successfully Saved....");
                _Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }

        }

        private void btnDel_Click(object sender, EventArgs e)
        {
            
            DialogResult result = (MessageBox.Show("Do you want to delete....?", "IGR...",MessageBoxButtons.YesNo,MessageBoxIcon.Warning));
            try
            {
                if (result == DialogResult.Yes)
                {
                    string qry = "update district set active = 'N'";
                    OdbcCommand cmd = new OdbcCommand(qry, sqlCon);
                    cmd.ExecuteNonQuery();

                    string qry1 = "update ro_master set active = 'N' ";
                    OdbcCommand cmd1 = new OdbcCommand(qry1, sqlCon);
                    cmd1.ExecuteNonQuery();
                    MessageBox.Show("Data successfully Deleted....");
                    LblDis.Text = "";
                    lblRO.Text = "";
                    populate_combo();
                    btnSave.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        private void cmbDis_Leave(object sender, EventArgs e)
        {
            if (cmbDis.DataSource != null)
            {
                string districtCode = cmbDis.SelectedValue.ToString();
                cmbWhereReg.DataSource = pCom.GetROffice(districtCode).Tables[0];
                cmbWhereReg.DisplayMember = "RO_name";
                cmbWhereReg.ValueMember = "RO_code";
            }
        }
    }
}
