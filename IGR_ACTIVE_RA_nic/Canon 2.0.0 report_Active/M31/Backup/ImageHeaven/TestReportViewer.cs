/*
 * Created by SharpDevelop.
 * User: RahulN
 * Date: 19/02/2014
 * Time: 1:13 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Drawing;
using System.Windows.Forms;
using nControls;
using NovaNet.Utils;
using System.Data.Odbc;
using System.Collections.Generic;

namespace TestComponents
{
	/// <summary>
	/// Description of TestReportViewer.
	/// </summary>
	public partial class TestReportViewer : Form
	{
		rptViewer rptV = null;
		OdbcConnection _con;
		List<NamedQuery> _lstNQ;
		public TestReportViewer(OdbcConnection pdbCon)
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
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
			
			TestReportViewerResize(null, null);
            
			_lstNQ = new List<NamedQuery>();
            //sfdUAT.Filter = "CSV files (*.csv)|*.csv";
            //sfdUAT.FilterIndex = 2;
            //sfdUAT.RestoreDirectory = true;
            
            //sfdUAT.ShowDialog();

            rptV.FilePath = "C:\\test.csv";
			
			//
			// TODO: Add constructor code after the InitializeComponent() call.
			//
		}
		
		void TestReportViewerKeyUp(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Escape)
			{
				this.Close();
			}
		}
		
		void TestReportViewerResize(object sender, EventArgs e)
		{
			rptV.Left = 0;
			rptV.Top = 0;
			rptV.Height = this.ClientSize.Height;
			rptV.Width = this.ClientSize.Width;
		}
		
		void TestReportViewerLoad(object sender, EventArgs e)
		{
			//_con = _dbCon.Connect();
			//rptV.Connection = _con;
			rptV.NamedQueries = rptViewer.ReadTabDelimitedFile(@"queries.csv");
		}

        private void TestReportViewer_FormClosing(object sender, FormClosingEventArgs e)
        {
            this._con.Close();
        }
	}
}
