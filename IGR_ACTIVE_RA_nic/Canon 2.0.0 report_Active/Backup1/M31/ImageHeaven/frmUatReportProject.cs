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
    public partial class frmUatReportProject : Form
    {
        MemoryStream stateLog;
        byte[] tmpWrite;
        NovaNet.Utils.dbCon dbcon;
        OdbcConnection sqlCon = null;
        Credentials crd = new Credentials();
        CtrlPolicy pPolicy = null;
        wfePolicy wPolicy = null;
        public frmUatReportProject(OdbcConnection prmCon, Credentials prmCrd)
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
                int Count = 0;
                DataSet ds = new DataSet();
                DataSet dsproj = new DataSet();
                string proj_key = string.Empty;
                string batch_key = string.Empty;
                ds = wPolicy.GetUAT_Resultproject(cmbDistrict.SelectedValue.ToString(), cmbRo.SelectedValue.ToString(), cmbBook.Text, cmbDeedYear.Text);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    for (int x = 0; x < ds.Tables[0].Rows.Count; x++)
                    {
                        if (x == 0)
                        {
                            ds.Tables[0].Columns.Add("Total_Scaned_Image");
                            ds.Tables[0].Columns.Add("Deeds_Hold");
                            ds.Tables[0].Columns.Add("Total_scaned_Image_Hold");
                            ds.Tables[0].Columns.Add("Deed_Submitted");
                            ds.Tables[0].Columns.Add("Image_Submitted");
                        }
                        ds.Tables[0].Rows[x]["Total_Scaned_Image"] = "0";
                        ds.Tables[0].Rows[x]["Deeds_Hold"] = "0";
                        ds.Tables[0].Rows[x]["Total_scaned_Image_Hold"] = "0";
                        ds.Tables[0].Rows[x]["Deed_Submitted"] = "0";
                        ds.Tables[0].Rows[x]["Image_Submitted"] = "0";
                        dsproj = wPolicy.GetProj_Batch(cmbDistrict.SelectedValue.ToString(), cmbRo.SelectedValue.ToString(), cmbBook.Text, cmbDeedYear.Text, ds.Tables[0].Rows[x][4].ToString());
                        if (dsproj.Tables[0].Rows.Count > 0)
                        {
                            proj_key = dsproj.Tables[0].Rows[0][0].ToString();
                            batch_key = dsproj.Tables[0].Rows[0][1].ToString();
                            

                            for (int i = 0; i < dsproj.Tables[0].Rows.Count; i++)
                            {
                                string total_scanned = wPolicy.getTotalScanImageCount(proj_key, batch_key).Tables[0].Rows[0][0].ToString();
                                string hold_deed = wPolicy.getTotalDeedCountonHold(proj_key, batch_key).Tables[0].Rows.Count.ToString();
                                string scan_image_hold = wPolicy.getTotalScanImageCountonHold(proj_key, batch_key).Tables[0].Rows[0][0].ToString();
                                ds.Tables[0].Rows[x]["Total_Scaned_Image"] = total_scanned;
                                ds.Tables[0].Rows[x]["Deeds_Hold"] = hold_deed;
                                ds.Tables[0].Rows[x]["Total_scaned_Image_Hold"] = scan_image_hold;
                                ds.Tables[0].Rows[x]["Deed_Submitted"] = (Convert.ToInt32(ds.Tables[0].Rows[x][5].ToString()) - Convert.ToInt32(hold_deed));
                                ds.Tables[0].Rows[x]["Image_Submitted"] = (Convert.ToInt32(total_scanned) - Convert.ToInt32(scan_image_hold));


                            }
                            Count++;
                        }
                    }
                    dtGrdresult.DataSource = ds.Tables[0];
                    
                }
                else
                {
                    ClearFields();
                    dtGrdresult.DataSource = null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ClearFields()
        {
            dtGrdresult.DataSource = null;
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
        private void GenerateTextFile()
        {
            try
            {
                svFile.Filter = "Text files (*.xls)|*.xls";
                svFile.FilterIndex = 2;
                svFile.RestoreDirectory = true;
                svFile.ShowDialog();
                string filename = svFile.FileName;
                Microsoft.Office.Interop.Excel._Application app = new Microsoft.Office.Interop.Excel.Application();
                Microsoft.Office.Interop.Excel._Workbook workbook = app.Workbooks.Add(Type.Missing);
                Microsoft.Office.Interop.Excel._Worksheet worksheet = (Microsoft.Office.Interop.Excel._Worksheet)workbook.Sheets["Sheet1"];
                app.Visible = true;
                worksheet = (Microsoft.Office.Interop.Excel._Worksheet)workbook.ActiveSheet;
                for (int i = 1; i < dtGrdresult.Columns.Count + 1; i++)
                {
                    worksheet.Cells[1, i] = dtGrdresult.Columns[i - 1].HeaderText;
                }

                for (int i = 0; i < dtGrdresult.Rows.Count - 1; i++)
                {
                    for (int j = 0; j < dtGrdresult.Columns.Count; j++)
                    {
                        if (j == 0)
                        {
                            worksheet.Cells[i + 2, j + 1] = cmbDistrict.Text;
                        }
                        else if (j == 1)
                        {
                            worksheet.Cells[i + 2, j + 1] = cmbRo.Text;
                        }
                        else
                        {
                            worksheet.Cells[i + 2, j + 1] = dtGrdresult.Rows[i].Cells[j].Value.ToString();
                        }
                    }
                }

                workbook.SaveAs(filename, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlExclusive, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);

                app.Quit();

               
                MessageBox.Show(this, "File Saved to: " + filename);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        private void deButton1_Click(object sender, EventArgs e)
        {
            GenerateTextFile();
        }


    }
}
