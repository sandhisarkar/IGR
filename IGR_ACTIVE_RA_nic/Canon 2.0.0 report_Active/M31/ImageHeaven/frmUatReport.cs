using System;
using System.Drawing;
using System.Windows.Forms;
using NovaNet.Utils;
using NovaNet.wfe;
using LItems;
using System.Data;
using System.Data.Odbc;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Collections.ObjectModel;


namespace ImageHeaven
{
    public partial class frmUatReport : Form
    {
        MemoryStream stateLog;
        byte[] tmpWrite;
        NovaNet.Utils.dbCon dbcon;
        OdbcConnection sqlCon = null;
        Credentials crd = new Credentials();
        CtrlPolicy pPolicy = null;
        wfePolicy wPolicy = null;
        public frmUatReport(OdbcConnection prmCon, Credentials prmCrd)
        {
            InitializeComponent();
            sqlCon = prmCon;
            crd = prmCrd;
            string newdbConStr = sqlCon.ConnectionString;
            newdbConStr = newdbConStr.Replace("3.51", "5.1");
            newdbConStr = newdbConStr.Replace("root;", "root;PASSWORD=root;");
            OdbcConnection newdbCon = new OdbcConnection(newdbConStr);
            newdbCon.Open();
            wPolicy = new wfePolicy(newdbCon);
            ClearFields();
            init_form();
        }

        private void init_form()
        {
            try
            {
                //For district.......................................
                if (wPolicy.GetDistrict_Active().Tables[0].Rows.Count > 0)
                {
                    cmbDistrict.DataSource = wPolicy.GetDistrict_Active().Tables[0];
                    cmbDistrict.DisplayMember = "district_name";
                    cmbDistrict.ValueMember = "district_code";
                    cmbDistrict.SelectedIndex = 0;
                }
                if (cmbDistrict.DataSource != null)
                {
                    string districtCode = cmbDistrict.SelectedValue.ToString();
                    if (wPolicy.GetROffice(districtCode).Tables[0].Rows.Count > 0)
                    {
                        cmbRo.DataSource = wPolicy.GetROffice(districtCode).Tables[0];
                        cmbRo.DisplayMember = "RO_name";
                        cmbRo.ValueMember = "RO_code";
                        cmbRo.SelectedIndex = 0;
                    }
                }
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }
        private void frmUatReport_Load(object sender, EventArgs e)
        {

        }



        private void cmdSearch_Click(object sender, EventArgs e)
        {
            try
            {
                DataSet ds = new DataSet();
                DataSet dsproj = new DataSet();
                string proj_key = string.Empty;
                string batch_key = string.Empty;
                ds = wPolicy.GetUAT_Result(cmbDistrict.SelectedValue.ToString(), cmbRo.SelectedValue.ToString(), cmbBook.Text, cmbDeedYear.Text, cmbVolume.Text);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    //dtGrdresult.DataSource = ds.Tables[0];
                    dsproj = wPolicy.GetProj_Batch(cmbDistrict.SelectedValue.ToString(), cmbRo.SelectedValue.ToString(), cmbBook.Text, cmbDeedYear.Text, cmbVolume.Text);
                    if (dsproj.Tables[0].Rows.Count > 0)
                    {
                        proj_key = dsproj.Tables[0].Rows[0][0].ToString();
                        batch_key = dsproj.Tables[0].Rows[0][1].ToString();
                        ds.Tables[0].Columns.Add("Total_Scaned_Image");
                        ds.Tables[0].Columns.Add("Deeds_Hold");
                        ds.Tables[0].Columns.Add("Total_scaned_Image_Hold");
                        ds.Tables[0].Columns.Add("Deed_Submitted");
                        ds.Tables[0].Columns.Add("Image_Submitted");
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            string total_scanned = wPolicy.getTotalScanImageCount(proj_key, batch_key).Tables[0].Rows[0][0].ToString();
                            string hold_deed = wPolicy.getTotalDeedCountonHold(proj_key, batch_key).Tables[0].Rows.Count.ToString();
                            string scan_image_hold = wPolicy.getTotalScanImageCountonHold(proj_key, batch_key).Tables[0].Rows[0][0].ToString();
                            ds.Tables[0].Rows[i]["Total_Scaned_Image"] = total_scanned;
                            ds.Tables[0].Rows[i]["Deeds_Hold"] = hold_deed;
                            ds.Tables[0].Rows[i]["Total_scaned_Image_Hold"] = scan_image_hold;
                            ds.Tables[0].Rows[i]["Deed_Submitted"] = (Convert.ToInt32(ds.Tables[0].Rows[i][5].ToString()) - Convert.ToInt32(hold_deed));
                            ds.Tables[0].Rows[i]["Image_Submitted"] = (Convert.ToInt32(total_scanned) - Convert.ToInt32(scan_image_hold));
                        }
                    }
                    dtGrdresult.DataSource = ds.Tables[0];
                    lblTotalDeed.Text = ds.Tables[0].Rows[0][5].ToString();
                    lblTotalperson.Text = ds.Tables[0].Rows[0][6].ToString();
                    lbltotalProperty.Text = ds.Tables[0].Rows[0][7].ToString();
                    lbltotalImagesscanned.Text = ds.Tables[0].Rows[0][8].ToString();
                    lblTotalhold.Text = ds.Tables[0].Rows[0][9].ToString();
                    lbltotalimagehold.Text = ds.Tables[0].Rows[0][10].ToString();
                    lblDeedsubmitted.Text = ds.Tables[0].Rows[0][11].ToString();
                    lblImageSubmitted.Text = ds.Tables[0].Rows[0][12].ToString();
                }
                else
                {
                    ClearFields();
                }
            }
            catch (Exception ex)
            {
                ClearFields();
                MessageBox.Show("Error");
            }
        }

        private void ClearFields()
        {
            dtGrdresult.DataSource = null;
            lblTotalDeed.Text = "0";
            lblTotalperson.Text = "0";
            lbltotalProperty.Text = "0";
            lbltotalImagesscanned.Text = "0";
            lblTotalhold.Text = "0";
            lbltotalimagehold.Text = "0";
            lblDeedsubmitted.Text = "0";
            lblImageSubmitted.Text = "0";
        }

        private void cmbDistrict_Leave(object sender, EventArgs e)
        {
            if (cmbDistrict.DataSource != null)
            {
                string districtCode = cmbDistrict.SelectedValue.ToString();
                if (wPolicy.GetROffice(districtCode).Tables[0].Rows.Count > 0)
                {
                    cmbRo.DataSource = wPolicy.GetROffice(districtCode).Tables[0];
                    cmbRo.DisplayMember = "RO_name";
                    cmbRo.ValueMember = "RO_code";
                    cmbRo.SelectedIndex = 0;
                }
            }
        }

        private void frmUatReport_FormClosing(object sender, FormClosingEventArgs e)
        {
           
        }

        private void lbltotalImagesscanned_Click(object sender, EventArgs e)
        {

        }


    }
}
