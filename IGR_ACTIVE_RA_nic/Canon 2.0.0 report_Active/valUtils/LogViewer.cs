/*
 * Created by SharpDevelop.
 * User: USER
 * Date: 28/03/2017
 * Time: 9:28 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Linq;
using System.Collections.Generic;

namespace valUtils
{
	/// <summary>
	/// Description of LogViewer.
	/// </summary>
	public class LogViewerData
	{
		string FilePath = string.Empty;
		List<List<string>> lstData;
		public LogViewerData(string pFilePath)
		{
			FilePath = pFilePath;
			init();
		}
		void init() {
			lstData = new List<List<string>>();
			lstData = FileUtils.ReadLines(FilePath);
//				.ForEach(x =>
//				         {
//				         	x.ForEach(y => System.Diagnostics.Debug.Print(y));
//				         });
		}
		public void Show() {
			LogView lgv = new LogView(lstData);
			lgv.Show();
		}
//		public List<string> GetFilteredData() {
//			
//		}
	}
}
