using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Data.Odbc;
using NovaNet.Utils;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft;
using Microsoft.Office;
using Microsoft.Office.Interop.Excel;

namespace ImageHeaven
{
    public partial class L1DeedReport : Form
    {
        OdbcConnection sqlCon;
        public L1DeedReport(OdbcConnection prmCon)
        {
            InitializeComponent();
            sqlCon = prmCon;
        }

        

        private void L1DeedReport_Load(object sender, EventArgs e)
        {
            OdbcCommand sqlCmd = new OdbcCommand();
            string sql = "update ac_role set role_description = 'IGR Audit' where role_description = 'LIC'";
            sqlCmd.Connection = sqlCon;
            sqlCmd.CommandText = sql;
            sqlCmd.ExecuteNonQuery();

            label1.Text = "District : " + populateDis().Rows[0][1].ToString();
            label2.Text = "Where Registered : " + populateRo().Rows[0][1].ToString();

            populateBook();
            

            button2.Enabled = false;

            dataGridView1.DataSource = null;
        }
        public System.Data.DataTable populateDis()
        {
            DataSet ds = new DataSet();
            System.Data.DataTable dt = new System.Data.DataTable();
            string sqlquery = "select district_code,district_name from district where active = 'Y'";
            OdbcDataAdapter odap = new OdbcDataAdapter(sqlquery, sqlCon);
            odap.Fill(dt);
            return dt;
        }
        public System.Data.DataTable populateRo()
        {
            DataSet ds = new DataSet();
            System.Data.DataTable dt = new System.Data.DataTable();
            string sqlquery = "select ro_code,ro_name from ro_master where active = 'Y'";
            OdbcDataAdapter odap = new OdbcDataAdapter(sqlquery, sqlCon);
            odap.Fill(dt);
            return dt;
        }

        public void populateBook()
        {
            DataSet ds = new DataSet();
            System.Data.DataTable dt = new System.Data.DataTable();
            string sqlquery = "select key_book,value_book from tbl_book";
            OdbcDataAdapter odap = new OdbcDataAdapter(sqlquery, sqlCon);
            odap.Fill(dt);
            
            if(dt.Rows.Count > 0)
            {
                comboBox1.DataSource = dt;
                comboBox1.DisplayMember = "key_book";
                comboBox1.ValueMember = "value_book";
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string sql = "select district_code as 'District Code',ro_code as 'RO Code',book as 'Book',deed_year as 'Deed Year',deed_no as 'Deed No',volume_no as 'Volume No' from deed_details where district_code = '" + populateDis().Rows[0][0].ToString() + "' and ro_code = '" + populateRo().Rows[0][0].ToString() + "' and deed_year = '" + textBox1.Text + "' and book = '" + comboBox1.SelectedValue.ToString() + "' AND deed_no NOT IN (select deed_no from index_of_name where district_code = '" + populateDis().Rows[0][0].ToString() + "' and ro_code = '" + populateRo().Rows[0][0].ToString() + "' and deed_year = '" + textBox1.Text + "' and book = '" + comboBox1.SelectedValue.ToString() + "') AND deed_no NOT IN (select deed_no from index_of_property where district_code = '" + populateDis().Rows[0][0].ToString() + "' and ro_code = '" + populateRo().Rows[0][0].ToString() + "' and deed_year = '" + textBox1.Text + "' and book = '" + comboBox1.SelectedValue.ToString() + "')";

            System.Data.DataTable dt = new System.Data.DataTable();
            System.Data.DataSet ds = new System.Data.DataSet();
            OdbcCommand cmd = new OdbcCommand(sql, sqlCon);
            OdbcDataAdapter odap = new OdbcDataAdapter(cmd);
            odap.Fill(dt);


            if (dt.Rows.Count > 0)
            {
                dataGridView1.DataSource = dt;
                button1.Enabled = false;
                button2.Enabled = true;
            }
            else
            {
                dataGridView1.DataSource = dt;
                button1.Enabled = true;
                button2.Enabled = false;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Microsoft.Office.Interop.Excel._Application app = new Microsoft.Office.Interop.Excel.Application();
            Microsoft.Office.Interop.Excel._Workbook workbook = app.Workbooks.Add(Type.Missing);

            Microsoft.Office.Interop.Excel._Worksheet worksheet = null;

            app.Visible = false;

            worksheet = (Microsoft.Office.Interop.Excel._Worksheet)workbook.Sheets["Sheet1"];

            
            worksheet = (Microsoft.Office.Interop.Excel._Worksheet)workbook.ActiveSheet;

            worksheet.Name = "Deed Report";

            worksheet.Cells[1, 3] = "Deed Report";
            Range range44 = worksheet.get_Range("C1");
            range44.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Red);
            
            worksheet.Rows.AutoFit();
            worksheet.Columns.AutoFit();


            worksheet.Cells[3, 1] = "District Name : " + populateDis().Rows[0][1].ToString();
            Range range43 = worksheet.get_Range("A3");
            range43.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Yellow);
            worksheet.Rows.AutoFit();
            worksheet.Columns.AutoFit();

            worksheet.Cells[4, 1] = "RO Name : " + populateRo().Rows[0][1].ToString();
            Range range33 = worksheet.get_Range("A4");
            range33.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Yellow);
            worksheet.Rows.AutoFit();
            worksheet.Columns.AutoFit();

            Range range = worksheet.get_Range("A3", "A4");
            range.Borders.Color = ColorTranslator.ToOle(Color.Black);


            Range range1 = worksheet.get_Range("A6", "F6");
            range1.Borders.Color = ColorTranslator.ToOle(Color.Black);

            for (int i = 1; i < dataGridView1.Columns.Count + 1; i++)
            {
                

                Range range2 = worksheet.get_Range("A6", "F6");
                range2.Borders.Color = ColorTranslator.ToOle(Color.Black);
                range2.EntireRow.AutoFit();
                range2.EntireColumn.AutoFit();
                worksheet.Cells[6, i] = dataGridView1.Columns[i - 1].HeaderText;
            }

            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                for (int j = 0; j < dataGridView1.Columns.Count; j++)
                {
                    Range range3 = worksheet.Cells;
                    //range3.Borders.Color = ColorTranslator.ToOle(Color.Black);
                    range3.EntireRow.AutoFit();
                    range3.EntireColumn.AutoFit();
                    worksheet.Cells[i + 7, j + 1] = dataGridView1.Rows[i].Cells[j].Value.ToString();

                }

            }

            string namexls = populateDis().Rows[0][0].ToString() + populateRo().Rows[0][0].ToString() + "_" + "Deed_Report_" + ".xls";
            string path = Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath);
            sfdUAT.Filter = "Xls files (*.xls)|*.xls";
            sfdUAT.FilterIndex = 2;
            sfdUAT.RestoreDirectory = true;
            sfdUAT.FileName = namexls;
            sfdUAT.ShowDialog();

            workbook.SaveAs(sfdUAT.FileName, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlExclusive, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);

            app.Quit();

        }
    }
}
