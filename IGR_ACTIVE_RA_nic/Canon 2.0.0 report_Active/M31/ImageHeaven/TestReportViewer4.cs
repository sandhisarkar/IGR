using System;
using System.Drawing;
using System.Windows.Forms;
using nControls;
using NovaNet.Utils;
using System.Data.Odbc;
using System.Collections.Generic;

namespace TestComponents
{
    public partial class TestReportViewer4 : Form
    {
        rptViewer rptV = null;
        OdbcConnection _con;
        List<NamedQuery> _lstNQ;
        public TestReportViewer4(OdbcConnection pdbCon)
        {
            InitializeComponent();
            string newdbConStr = pdbCon.ConnectionString;
            newdbConStr = newdbConStr.Replace("3.51", "5.1");
            newdbConStr = newdbConStr.Replace("root;", "root;PASSWORD=root;");
            OdbcConnection newdbCon = new OdbcConnection(newdbConStr);
            newdbCon.Open();
            _con = newdbCon;
            //_con = pdbCon;
            rptV = new rptViewer(_con);
            this.Controls.Add(rptV);

            TestReportViewer4Resize(null, null);

            _lstNQ = new List<NamedQuery>();
            //sfdUAT.Filter = "CSV files (*.csv)|*.csv";
            //sfdUAT.FilterIndex = 2;
            //sfdUAT.RestoreDirectory = true;

            //sfdUAT.ShowDialog();

            rptV.FilePath = "C:\\test.csv";
        }
        void TestReportViewer4Resize(object sender, EventArgs e)
        {
           
        }

        private void TestReportViewer4_Load(object sender, EventArgs e)
        {
            rptV.NamedQueries = rptViewer.ReadTabDelimitedFile(@"queries4.csv");
            
        }

        private void TestReportViewer4_FormClosing(object sender, FormClosingEventArgs e)
        {
            this._con.Close();
        }

        private void TestReportViewer4_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
        }

        private void TestReportViewer4_Resize(object sender, EventArgs e)
        {
            rptV.Left = 0;
            rptV.Top = 0;
            rptV.Height = this.ClientSize.Height;
            rptV.Width = this.ClientSize.Width;
        }
    }
}
