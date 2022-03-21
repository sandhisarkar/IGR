using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using NovaNet.Utils;
using NovaNet.wfe;
using LItems;
using System.Data.Odbc;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace ImageHeaven
{
    public partial class frmNewReport : Form
    {
        OdbcConnection sqlCon;
        public frmNewReport(OdbcConnection prmCon)
        {
            InitializeComponent();
            sqlCon = prmCon;
        }

        private void frmNewReport_Load(object sender, EventArgs e)
        {
            panel1.Visible = false;
            dataGridView1.DataSource = null;

            

            populateRunNum();
            
        }
        private void populateRunNum()
        {
            DataSet ds = new DataSet();
            wfeBox dly = new wfeBox(sqlCon);
            ds = dly.GetRunnum();
            if (ds.Tables[0].Rows.Count > 0)
            {
                cmbRunnum.DataSource = ds.Tables[0];
                cmbRunnum.DisplayMember = "run_no";
                cmbRunnum.ValueMember = "run_no";
            }

        }

        private void buttonPopulate_Click(object sender, EventArgs e)
        {
            string sql = "select distinct  a.deed_no as Deed, a.modified_dttm as Date,a.modified_by as User from policy_master a, lic_qa_log b, district c, ro_master d,ac_user_role_map e  where a.do_code=c.district_code and a.run_no='" + cmbRunnum.Text.ToString().Trim() + "'and a.policy_number=b.policy_number and b.qa_status=0 and d.district_code=a.do_code and d.ro_code=a.br_code and a.modified_by=e.user_id and e.role_id = '8' and (a.status = 31 or a.status= 30)";
            
            DataTable dt = new DataTable();
            DataSet ds = new DataSet();
            OdbcCommand cmd = new OdbcCommand(sql, sqlCon);
            OdbcDataAdapter odap = new OdbcDataAdapter(cmd);
            odap.Fill(dt);

            //string abc = "select *  from ac_user_role_map where user_id = '" + dt.Rows[1].ToString() + "' and role_id = '8'";
            //DataTable dt1 = new DataTable();
            //DataSet ds1 = new DataSet();
            //OdbcCommand cmd1 = new OdbcCommand(abc, sqlCon);
            //OdbcDataAdapter odap1 = new OdbcDataAdapter(cmd1);
            //odap.Fill(dt1);

            panel1.Visible = true;
            dataGridView1.DataSource = dt;
            //dataGridView1.AutoSizeColumnsMode();

            label1.Text = "Total deed :" + dt.Rows.Count;

            if (dt.Rows.Count > 0)
            {
                buttonPopulate.Hide();
                buttonReport.Show();
                buttonReport.Focus();
                buttonReport.Select();
            }
        }

        private void buttonReport_Click(object sender, EventArgs e)
        {
            DataSet dsVol = new DataSet();
            OdbcDataAdapter sqlAdap = null;
            DataSet dsDeed = new DataSet();
            OdbcDataAdapter sqlTemp = null;
            DataSet dsVoltot = new DataSet();
            OdbcDataAdapter sqlAdaptot = null;
            DataSet dsDsRo = new DataSet();
            OdbcDataAdapter sqldr = null;
            string sql = "select run_no,do_code,br_code,deed_no as Deed,created_dttm as Date, created_by as User,deed_year from policy_master a where run_no='" + cmbRunnum.Text.ToString().Trim() + "' ";
            sqlAdap = new OdbcDataAdapter(sql, sqlCon);
            sqlAdap.Fill(dsVol);
            string sqlDR = "select a.run_no as 'Submission No.', c.district_name as District,d.ro_name as RO, a.deed_year as Year, count(*) as 'Total' from policy_master a, district c, ro_master d where a.do_code=c.district_code and a.run_no='" + cmbRunnum.Text.ToString().Trim() + "' and d.district_code=a.do_code and d.ro_code=a.br_code";
            sqldr = new OdbcDataAdapter(sqlDR, sqlCon);
            sqldr.Fill(dsDsRo);

            string qry = "select distinct a.deed_vol as Volume, a.deed_no as Deed,a.status as Status, a.modified_dttm as Date,a.modified_by as User from policy_master a, lic_qa_log b, district c, ro_master d, ac_user_role_map e  where a.do_code=c.district_code and a.run_no='" + cmbRunnum.Text.ToString().Trim() + "'and a.policy_number=b.policy_number and b.qa_status=0 and d.district_code=a.do_code and d.ro_code=a.br_code and a.modified_by=e.user_id and e.role_id = '8' and (a.status = 31 or a.status= 30)";
            sqlTemp = new OdbcDataAdapter(qry, sqlCon);
            sqlTemp.Fill(dsDeed);
            string sqltot = "select count(deed_no)  from policy_master  where run_no='" + cmbRunnum.Text.ToString().Trim() + "'  ";
            sqlAdaptot = new OdbcDataAdapter(sqltot, sqlCon);
            sqlAdaptot.Fill(dsVoltot);
            sfdUAT.Filter = "Pdf files (*.pdf)|*.pdf";
            sfdUAT.FilterIndex = 2;
            sfdUAT.RestoreDirectory = true;
            sfdUAT.FileName = dsVol.Tables[0].Rows[0][0].ToString() + "_report";
            sfdUAT.ShowDialog();

            FileStream fs = new FileStream(sfdUAT.FileName, FileMode.Create, FileAccess.Write, FileShare.None);
            Document doc = new Document();
            PdfWriter writer = PdfWriter.GetInstance(doc, fs);
            doc.Open();


            doc.Add(new Header("-----Deed Details-----",""));

            doc.Add(new Paragraph("Lot No. " + dsVol.Tables[0].Rows[0][0].ToString()));
            doc.Add(new Paragraph("District. " + dsDsRo.Tables[0].Rows[0][1].ToString()));
            doc.Add(new Paragraph("RO. " + dsDsRo.Tables[0].Rows[0][2].ToString()));
            doc.Add(new Paragraph("Deed Year. " + dsVol.Tables[0].Rows[0][6].ToString()));
            doc.Add(new Paragraph("Total Deed in this Lot Number. " + dsVoltot.Tables[0].Rows[0][0].ToString()));
            //doc.Add(new Paragraph("Checked User. " + dsVol.Tables[0].Rows[0][5].ToString()));
            //doc.Add(new Paragraph("Total Deed checked by this user. " + dsVol.Tables[0].Rows.Count.ToString()));
            
            //doc.Add(new Paragraph("Year. " + dsVol.Tables[0].Rows[0][3].ToString()));
            //doc.Add(new Paragraph("Total. " + dsVol.Tables[0].Rows[0][3].ToString()));
            //doc.Add(new Paragraph("Checked Date-Time. " + dsVol.Tables[0].Rows[0][4].ToString()));
            
            
            doc.Add(new Paragraph("\n"));
            doc.Add(new Paragraph("\n"));

            
            
            PdfPTable table1 = new PdfPTable(6);
            table1.WidthPercentage = 90;
            PdfPTable table2 = new PdfPTable(6);
            table2.WidthPercentage = 90;
            PdfPCell cell1 = new PdfPCell();
            PdfPCell cell2 = new PdfPCell();
            PdfPCell cell3 = new PdfPCell();
            PdfPCell cell4 = new PdfPCell();
            PdfPCell cell5 = new PdfPCell();
            PdfPCell cell6 = new PdfPCell();

            cell1.AddElement(new Paragraph("Sl No. "));
            cell2.AddElement(new Paragraph("Deed No. "));
            cell3.AddElement(new Paragraph("Volume Number "));
            cell4.AddElement(new Paragraph("Checked Date-Time "));
            cell5.AddElement(new Paragraph("Checked User  "));
            cell6.AddElement(new Paragraph("Status  "));
            table2.AddCell(cell1);
            table2.AddCell(cell2);
            table2.AddCell(cell3);
            table2.AddCell(cell4);
            table2.AddCell(cell5);
            table2.AddCell(cell6);
            cell1.VerticalAlignment = Element.ALIGN_CENTER;
            cell2.VerticalAlignment = Element.ALIGN_CENTER;
            cell3.VerticalAlignment = Element.ALIGN_CENTER;
            cell4.VerticalAlignment = Element.ALIGN_CENTER;
            cell5.VerticalAlignment = Element.ALIGN_CENTER;
            cell6.VerticalAlignment = Element.ALIGN_CENTER;
            doc.Add(table2);

            for (int i = 0; i < dsDeed.Tables[0].Rows.Count; i++)
            {
                PdfPCell cell11 = new PdfPCell();
                PdfPCell cell12 = new PdfPCell();
                PdfPCell cell13 = new PdfPCell();
                PdfPCell cell14 = new PdfPCell();
                PdfPCell cell15 = new PdfPCell();
                PdfPCell cell16 = new PdfPCell();

                int k = i + 1;

                cell11.AddElement(new Paragraph(k.ToString()));

                cell12.AddElement(new Paragraph(dsDeed.Tables[0].Rows[i][1].ToString()));

                cell13.AddElement(new Paragraph(dsDeed.Tables[0].Rows[i][0].ToString()));

                cell14.AddElement(new Paragraph(dsDeed.Tables[0].Rows[i][3].ToString()));

                cell15.AddElement(new Paragraph(dsDeed.Tables[0].Rows[i][4].ToString()));

                //cell13.AddElement(new Paragraph(dsDeed.Tables[0].Rows[i][1].ToString()));
                if (dsDeed.Tables[0].Rows[i][2].ToString() == "31")
                {
                    cell16.AddElement(new Paragraph("Accepted"));
                }
                else
                {
                    cell16.AddElement(new Paragraph("Correction Required"));
                }

                cell11.VerticalAlignment = Element.ALIGN_CENTER;
                cell12.VerticalAlignment = Element.ALIGN_CENTER;
                cell13.VerticalAlignment = Element.ALIGN_CENTER;
                cell14.VerticalAlignment = Element.ALIGN_CENTER;
                cell15.VerticalAlignment = Element.ALIGN_CENTER;
                cell16.VerticalAlignment = Element.ALIGN_CENTER;
                table1.AddCell(cell11);
                table1.AddCell(cell12);
                table1.AddCell(cell13);
                table1.AddCell(cell14);
                table1.AddCell(cell15);
                table1.AddCell(cell16);
            }
            doc.Add(table1);
            doc.Close();
        }

        private void cmbRunnum_Leave(object sender, EventArgs e)
        {
            buttonReport.Hide();
            buttonPopulate.Show();
            
            
        }
    }
}
