/*
 * Created by SharpDevelop.
 * User: USER
 * Date: 21/03/2017
 * Time: 11:41 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Collections.Generic;
using Microsoft.VisualBasic.FileIO;
using System.Globalization;
using System.Text;
using System.Reflection;

namespace valUtils
{
	/// <summary>
	/// Description of FileUtils.
	/// </summary>
	/// 
	public class FileMap {
		public string DeedControl;
		public string ItemNo;
		public string flName;
		public string Result;
	}
	public class ErrorLogMap : FileMap {
		public ErrorLogMap(FileMap flm) {
			this.DeedControl = flm.DeedControl;
			this.ItemNo = flm.ItemNo;
			this.flName = flm.flName;
			this.Result = flm.Result;
		}
		public string UpdateValue;
	}
	public class FileRecordMap : FileMap {
		public FileRecordMap(FileMap flm) {
			this.DeedControl = flm.DeedControl;
			this.ItemNo = flm.ItemNo;
			this.flName = flm.flName;
			this.Result = flm.Result;
		}
		public Dictionary<string, string> Values;
	}
	public class FileUtils
	{

		public FileUtils()
		{
			
		}
		public static string GetNextVersionOfFile(string path, string flName, string wCard="*", int startIndex=1) {
			string flDir = path;
			long newNo = GetNextVersion(path, flName, wCard, startIndex);
			string retVal = path + "\\" + flName + Convert.ToString(newNo);
			return retVal;
			//string newNameWithoutExt =

		}
		public static long GetNextVersion(string path, string flName, string wCard="*", int startIndex=1) {
			string flDir = path;
			string flFilePathWithExt = Path.GetFileName(path);
			string flFilePathWithoutExt = Path.GetFileName(path);
			string lastFile = Path.GetFileNameWithoutExtension(Directory.GetFiles(flDir, flName+"*."+wCard)
			                                                   .ToList()
			                                                   .OrderBy(x => x)
			                                                   .LastOrDefault());
			
			if (null == lastFile || string.Empty == lastFile) {
				return startIndex;
			}
			else {
				string tmp = Reverse(lastFile);
				tmp = Reverse(GetFirstDigits(tmp));
				long n=startIndex;
				n = Convert.ToInt64(tmp);
				return n+1;
			}
		}
		public static string GetFirstDigits(string fld) {
			string retVal = string.Empty;
			foreach (char x in fld) {
				if (Char.IsDigit(x)) {
					retVal += Convert.ToString(x);
				}
				else {
					break;
				}
			}
			return retVal;
		}
		public static string Reverse( string s )
		{
			char[] charArray = s.ToCharArray();
			Array.Reverse( charArray );
			return new string( charArray );
		}
		public static bool WriteCSVGen<T>(string outPath, List<T> data, string delimiter=",", bool WithHeader=false) {
			
			using (StreamWriter sw = new StreamWriter(outPath)) {
				foreach (var item in data) {
					string ot=string.Empty;
					foreach (var fld in item.GetType().GetFields()) {
						if (WithHeader) {
							ot += "\"" + fld.Name + "=" + fld.GetValue(item) + "\"" + delimiter;
						}
						else {
							ot += "\"" + fld.GetValue(item) + "\"" + delimiter;
						}
						
					}
					if (ot.Length > 0) {
						sw.WriteLine(ot.Substring(0, ot.Length-1));
					}
					else {
						sw.WriteLine(ot);
					}
				}
				
			}

			return true;
		}
		public static bool WriteCSV(string outPath, List<Dictionary<string, string>> data, string delimiter=",") {
			
			using (StreamWriter sw = new StreamWriter(outPath)) {
				sw.WriteLine(data.FirstOrDefault()
				             .ToList()
				             .Aggregate(new StringBuilder(), (i, j) => i.Append("\"" +j.Key + "\"" + delimiter), i => i.ToString(0, i.Length-1)));
				
				data.Select(line =>
				            line
				            .ToList()
				            .Aggregate(new StringBuilder(), (i, j) => i.Append("\"" +j.Value + "\"" + delimiter), i => i.ToString(0, i.Length-1)))
					.ToList()
					.ForEach(val => sw.WriteLine(val));
				
			}

			return true;
		}
       #if Framework45
		public static List<string> ExtractFiles(string zf, string targetPath, bool OverWrite=false) {
			if (!Directory.Exists(targetPath)) {
				Directory.CreateDirectory(targetPath);
			}
			ZipArchive za = ZipFile.OpenRead(zf);
			if (OverWrite) {
				za.Entries.Select(e =>
				                  Path.Combine(targetPath,e.Name))
					.ToList()
					.Where(File.Exists)
					.ToList()
					.ForEach(x => File.Delete(x));
			}
			try{
				ZipFile.ExtractToDirectory(zf, targetPath);
			}
			catch (IOException ex) {
				
				return za.Entries.Select(e =>
				                         Path.Combine(targetPath,e.Name))
					.ToList();
			}
			
			return za.Entries.Select(e =>
			                         Path.Combine(targetPath,e.Name))
				.ToList();
			
		}
       
		public static bool ZipFiles(List<string> zf, string targetPath, bool OverWrite=false) {
			if (!Directory.Exists(Path.GetDirectoryName(targetPath))) {
				Directory.CreateDirectory(targetPath);
			}
			else {
				if (File.Exists(targetPath)) {
					File.Delete(targetPath);
				}
			}
			ZipArchive za = ZipFile.Open(targetPath, ZipArchiveMode.Create);
			zf.ForEach(x => {
			           	za.CreateEntryFromFile(x, Path.GetFileName(x),CompressionLevel.Optimal);
			           });
			za.Dispose();
			return true;
		
		}
       #endif
		public static List<List<string>> ReadLines(string fl) {
			List<List<string>> ret = new List<List<string>>();
			using (TextFieldParser parser = new TextFieldParser(fl))
			{
				parser.TextFieldType = FieldType.Delimited;
				parser.SetDelimiters(",");
				while (!parser.EndOfData)
				{
					//Processing row
					List<string> fields = parser.ReadFields().ToList();
					ret.Add(fields);
				}
			}
			return ret;
		}

		public static List<string> ReturnColumn(List<List<string>> lstSource, int colNo) {
			return lstSource.Select(l =>
			                        l.ElementAt(colNo))
				.ToList();
			
		}
		public static Dictionary<string, string> TransformRow(List<string> lst, List<string> head) {
			Dictionary<string, string> dicValues = new Dictionary<string, string>();
			Enumerable.Range(0, head.Count)
				.ToList()
				.ForEach(i => dicValues.Add(head[i], lst[i]));
			return dicValues;
		}
	}
}
