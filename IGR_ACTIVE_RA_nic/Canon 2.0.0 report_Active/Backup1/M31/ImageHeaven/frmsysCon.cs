using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using igr_base;
using System.Collections;
using System.Data.Odbc;
using NovaNet.Utils;
using LItems;



namespace ImageHeaven
{
    
    public partial class frmsysCon : Form
    {
        Credentials crd = new Credentials();
        OdbcConnection sqlCon = new OdbcConnection();
        wfePolicy wPolicy = null;
        public frmsysCon(OdbcConnection prmCon, Credentials prmCrd)
        {
            InitializeComponent();
            sqlCon = prmCon;
            crd = prmCrd;
            wPolicy = new wfePolicy(sqlCon);
            populate_combo();
        }

       
        
        private void populate_combo()
        {
            cmbDis.DataSource = wPolicy.GetDistrict().Tables[0];
            if (wPolicy.GetDistrict().Tables[0].Rows.Count > 0)
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
                if (cmbDis.SelectedValue != null && cmbWhereReg.SelectedValue != null)
                {
                    if (wPolicy.set_Active(cmbDis.SelectedValue.ToString(),cmbWhereReg.SelectedValue.ToString()) == true)
                    {
                        MessageBox.Show("Data successfully Saved....");
                        this.Close();
                    }
                    else
                    {
                        lblRO.Text = "Error!!!";
                    }
                }
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }

        }

        private void btnDel_Click(object sender, EventArgs e)
        {
            
        }

        private void cmbDis_Leave(object sender, EventArgs e)
        {
            if (cmbDis.SelectedValue != null && cmbDis.SelectedValue != "")
            {
                string districtCode = cmbDis.SelectedValue.ToString();
                cmbWhereReg.DataSource = wPolicy.GetRO(districtCode).Tables[0];
                cmbWhereReg.DisplayMember = "RO_name";
                cmbWhereReg.ValueMember = "RO_code";
                cmbWhereReg.SelectedIndex = 0;
            }
        }

        private void frmsysCon_Load(object sender, EventArgs e)
        {

        }
    }
}
