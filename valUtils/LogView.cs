/*
 * Created by SharpDevelop.
 * User: USER
 * Date: 28/03/2017
 * Time: 9:42 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;
using System.Collections.Generic;


namespace valUtils
{
	/// <summary>
	/// Description of LogView.
	/// </summary>
	public partial class LogView : Form
	{
		List<List<string>> lstData;
		bool HasHeader;
		public LogView(List<List<string>> pData, bool pHasHeader=false)
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			//
			// TODO: Add constructor code after the InitializeComponent() call.
			//
			lstData = new List<List<string>>();
			lstData = pData;
			HasHeader = pHasHeader;
			init();
		}
		void init() {
			lvwData.Columns.AddRange(
				Enumerable.Range(1, lstData.FirstOrDefault().Count)
				.ToList()
				.Select(x => new ColumnHeader(Convert.ToString(x)))
				.ToArray());
			Enumerable.Range(1, lvwData.Columns.Count)
				.ToList()
				.ForEach(x => lvwData.Columns[x-1].Text = Convert.ToString(x));
			lstData.ForEach(row => {
			                	ListViewItem item = lvwData.Items.Add(row.ElementAt(0));
			                	Enumerable.Range(1, row.Count-1)
			                		.ToList()
			                		.ForEach(x => item.SubItems.Add(row.ElementAt(x)));
			                });
		}
		
		void LogViewKeyUp(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Escape) {
				this.Close();
			}
		}
		
		void CmdCloseClick(object sender, EventArgs e)
		{
			this.Close();
		}
		
	}
}
