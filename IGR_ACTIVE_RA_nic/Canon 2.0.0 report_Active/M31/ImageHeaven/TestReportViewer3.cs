using System;
using System.Drawing;
using System.Windows.Forms;
using nControls;
using NovaNet.Utils;
using System.Data.Odbc;
using System.Collections.Generic;

namespace ImageHeaven
{
    public partial class TestReportViewer3 : Form
    {
        rptViewer rptV = null;
        OdbcConnection _con;
        List<NamedQuery> _lstNQ;

        public TestReportViewer3(OdbcConnection pdbCon)
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

            TestReportViewer3Resize(null, null);

            _lstNQ = new List<NamedQuery>();
            //sfdUAT.Filter = "CSV files (*.csv)|*.csv";
            //sfdUAT.FilterIndex = 2;
            //sfdUAT.RestoreDirectory = true;

            //sfdUAT.ShowDialog();

            rptV.FilePath = "C:\\test.csv";
        }
        void TestReportViewer3Resize(object sender, EventArgs e)
        {

        }

        private void TestReportViewer3_Load(object sender, EventArgs e)
        {
            rptV.NamedQueries = rptViewer.ReadTabDelimitedFile(@"queries3.csv");
        }

        private void TestReportViewer3_Resize(object sender, EventArgs e)
        {
            rptV.Left = 0;
            rptV.Top = 0;
            rptV.Height = this.ClientSize.Height;
            rptV.Width = this.ClientSize.Width;
        }

        private void TestReportViewer3_FormClosing(object sender, FormClosingEventArgs e)
        {
            this._con.Close();
        }

        private void TestReportViewer3_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
        }
    }
}
