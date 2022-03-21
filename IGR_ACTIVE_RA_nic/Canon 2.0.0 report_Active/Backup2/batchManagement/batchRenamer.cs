/*
 * Created by SharpDevelop.
 * User: arpanp
 * Date: 23/09/2016
 * Time: PM 02:01
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using System.Data.Odbc;
using System.Security.Authentication;

namespace batchManagement
{
	/// <summary>
	/// Description of batchRenamer.
	/// </summary>
	public class batchRenamer
	{
		private OdbcConnection dbCon;
		private int mBatchKey;
		private string mNewBatchName = null;
		private string mOldBatchName = null;
		private string mBatchPath = null;
		private string mDirPath = null;
		private string mNewBatchPath = null;
		private bool isOk;
		private int mYear;
		private string mBoxPath = null;
		
		public batchRenamer(int pBatchKey, string pNewBatchName, OdbcConnection pCon)
		{
			mBatchKey = pBatchKey;
			mNewBatchName = pNewBatchName;
			dbCon = pCon;
			mOldBatchName = setBatchName(mBatchKey);
			mBatchPath = getBatchPath(mBatchKey);
			get_deedYear();
			isOk = validateNewBatch(mNewBatchName);
		}
		
		private string setBatchName(int batchKey)
		{
			string temp_batch_name = null;
			string qry = "select batch_name from batch_master where batch_key = '"+ batchKey +"'";
			OdbcDataReader rdr = null;
			OdbcCommand cmd = new OdbcCommand(qry, dbCon);
			try{
				rdr = cmd.ExecuteReader();
				while (rdr.Read())
				{
					temp_batch_name = rdr.GetString(0).ToString();
				}
				if(rdr != null)
				{
					rdr.Close();
				}
			}
			catch(OdbcException ex)
			{
				Debug.Print(ex.ToString());
			}
			return temp_batch_name;
		}
		
		private bool validateNewBatch(string newBatch)
		{
			string DCode = newBatch.Substring(0, 2);
			string ROCode = newBatch.Substring(2, 2);
			string Book = newBatch.Substring(4, 1);
			int year = Convert.ToInt32(newBatch.Substring(5, 4));
			//mYear = year;
			string  vol_no = newBatch.Substring(9, newBatch.Length - 9);
			string distCODE = getDistCode();
			string roCODE = getROCode();
			bool volNUMBER = validateVolNo(newBatch);
			if(mYear != year)
			{
				throw  new System.ArgumentException("Deed year in new batch should be equal to " + mYear + " for batch no. " + mNewBatchName);
				}
			else
			{
				if(DCode.Equals(distCODE) && ROCode.Equals(roCODE) && Book.Length == 1 && (year > 1800 && year < Convert.ToInt32(DateTime.Now.Year.ToString()))/*&& volNUMBER == false*/)
				{
//				getBatchPath(batchKey,batchName);
//				renameBatch(batchKey,batchName,newBatch);
					int stat = renameBatch();
					if(stat == 1)
					{
						return true;
					}
					else
					{
						return false;
					}
				}
				else
				{
					//MessageBox.Show("Not a valid batch name...","Error",MessageBoxButtons.OK,MessageBoxIcon.Error);
					return false;
				}
			}
		}
		
		private string getDistCode()
		{
			string dCode = null;
			string qry = "select district_code from district where active = 'Y'";
			OdbcDataReader rdr = null;
			OdbcCommand cmd = new OdbcCommand(qry,dbCon);
			try{
				rdr = cmd.ExecuteReader();
				while (rdr.Read())
				{
					dCode = rdr.GetString(0).ToString();
				}
				if(rdr != null)
				{
					
					rdr.Close();
				}
				
				
			}
			catch(OdbcException ex)
			{
				MessageBox.Show(ex.ToString());
			}
			return dCode;
		}
		
		private string getROCode()
		{
			string roCode = null;
			string qry = "select ro_code from ro_master where active = 'Y'";
			OdbcDataReader rdr = null;
			OdbcCommand cmd = new OdbcCommand(qry,dbCon);
			try{
				rdr = cmd.ExecuteReader();
				while (rdr.Read())
				{
					roCode = rdr.GetString(0).ToString();
				}
				if(rdr != null)
				{
					
					rdr.Close();
				}
				
				
			}
			catch(OdbcException ex)
			{
				MessageBox.Show(ex.ToString());
			}
			return roCode;
		}
		
		private bool validateVolNo(string batchName)
		{
			bool flag = false;
			string qry = "select batch_name from batch_master where batch_name = '"+ batchName +"'";
			OdbcDataReader rdr = null;
			OdbcCommand cmd = new OdbcCommand(qry,dbCon);
			try{
				rdr = cmd.ExecuteReader();
				while (rdr.Read())
				{
					string batchNAME = rdr.GetString(0).ToString();
					flag = true;
				}
				if(rdr != null)
				{
					
					rdr.Close();
				}
				
				
			}
			catch(OdbcException ex)
			{
				MessageBox.Show(ex.ToString());
			}
			if(flag == true)
			{
				return flag;
			}
			else
			{
				return flag;
			}
		}

		private string getBatchPath(int batchKey)
		{
			string batchPath = null;
			OdbcDataReader rdr = null;
			string query = "select batch_path from batch_master where batch_key = '"+ batchKey +"'";
			OdbcCommand cmd = new OdbcCommand(query,dbCon);
			rdr = cmd.ExecuteReader();
			while (rdr.Read())
			{
				batchPath = rdr.GetString(0).ToString();
			}
			if(rdr != null)
			{
				rdr.Close();
			}
			return batchPath;
		}
		
		private void get_deedYear()
		{
			DataSet ds = new DataSet();
			OdbcDataAdapter oda = null;
			string query = "select distinct deed_year from deed_details where batch_key = '"+ mBatchKey +"'";
			oda = new OdbcDataAdapter(query,dbCon);
			oda.Fill(ds);
			if(ds.Tables[0].Rows.Count == 1)
			{
				mYear = Convert.ToInt32(ds.Tables[0].Rows[0][0].ToString());
			}
			else if(ds.Tables[0].Rows.Count > 1)
			{
				string yrs = null;
				for(int i = 0; i < ds.Tables[0].Rows.Count; i++)
				{
					yrs = yrs + ds.Tables[0].Rows[i][0].ToString() + "," ;
				}
				yrs = yrs.TrimEnd(',');
				throw new System.ArgumentException("Deed year in deed details contains more than one year. Correct through Data Entry and Retry. Years found: " + yrs);
				
			}
			else if(ds.Tables[0].Rows.Count == 0)
			{
				throw new System.ArgumentException("No deed year found in deed details.");
				
			}
		}
		
		
		private int renameBatch()
		{
			OdbcTransaction sqlTrans = null;
			int temp1 = -2, temp2 = -2, temp3 = -2, temp4 = -2;
			try
			{
				sqlTrans = dbCon.BeginTransaction();
				temp1 = updateBatchMaster(sqlTrans);
				temp2 = updateBoxMaster(sqlTrans);
				temp3 = updatePolicyMaster(sqlTrans);
				temp4 = updateImageMaster(sqlTrans);
				sqlTrans.Commit();
			}
			catch(Exception ex)
			{
				MessageBox.Show(ex.Message.ToString());
				sqlTrans.Rollback();
			}
			if(temp1 == 1 && temp2 == 1 && temp3 == 1 && temp4 == 1)
			{
				return 1;
			}
			else
			{
				return -1;
			}
		}
		
		
		private int getBatchKey(string batchName, string batchCode, string batchPath, OdbcTransaction trans)
		{
			OdbcDataReader rdr = null;
			int box_batch = 0;
			string query = "select batch_key from batch_master where batch_name = '"+ batchName +"' and batch_code = '"+ batchCode +"' and batch_path = '"+ batchPath +"'";
			OdbcCommand cmd = new OdbcCommand(query,dbCon,trans);
			try{
				rdr = cmd.ExecuteReader();
				while (rdr.Read())
				{
					box_batch = Convert.ToInt32(rdr.GetString(0).ToString());
				}
				if(rdr != null)
				{
					
					rdr.Close();
				}
				
				
			}
			catch(OdbcException ex)
			{
				MessageBox.Show(ex.ToString());
			}
			return box_batch;
		}
		
		private int updateBatchMaster(OdbcTransaction trans)
		{
			//batchName = Path.GetFileName(batchPath.TrimEnd(Path.DirectorySeparatorChar));
			DirectoryInfo parentDir = Directory.GetParent(mBatchPath);
			//dirPath = parentDir.Parent.FullName;
			mDirPath = parentDir.FullName;
			string tempBatchName = mNewBatchName;
			string newBatchPath = mDirPath.Replace("\\","\\\\") + "\\\\" + mNewBatchName;
			
			/*rename old batch*/
			bool folderExists = Directory.Exists(mBatchPath);
			string old_BatchFolder = mDirPath.Replace("\\","\\\\") + "\\\\old_" + mOldBatchName;

			
			/* changes in batch_master table */
			int flag = -1;
			string str_pre = null;
			string str_post = null;
			string str_final = null;
			OdbcDataReader rdr = null;
			string query = "select batch_key, proj_code, Batch_code, batch_name, Created_By, Created_DTTM, Modified_by, Modified_DTTM, Batch_path, status, Active, run_no " +
				"from batch_master where batch_key = '"+ mBatchKey +"' and batch_name = '"+ mOldBatchName +"'";
			OdbcCommand cmd = new OdbcCommand(query,dbCon,trans);
			try{
				
				/* copy batch folder */
//				if(folderExists)
//				{
				if(!folderExists)
				{
					MessageBox.Show("Folder not found for batch: " + mOldBatchName);
				}
				//Create all of the directories
				foreach (string dirPath in Directory.GetDirectories(mBatchPath, "*",
				                                                    SearchOption.AllDirectories))
					Directory.CreateDirectory(dirPath.Replace(mBatchPath, newBatchPath));
				//Copy all the files & Replaces any files with the same name
				foreach (string newPath in Directory.GetFiles(mBatchPath, "*.*",
				                                              SearchOption.AllDirectories))
					File.Copy(newPath, newPath.Replace(mBatchPath, newBatchPath), true);
				old_BatchFolder = old_BatchFolder.Replace("\\\\","\\");
				Directory.Move(mBatchPath, old_BatchFolder);
//				}
//				if(!folderExists)
//				{
//					MessageBox.Show("Folder not found for batch: " + mOldBatchName);
//				}
				
				
				
				rdr = cmd.ExecuteReader();
				//List<string> fldNames
				while (rdr.Read())
				{
					str_pre = "insert into batch_master (batch_name, batch_code, batch_path, created_dttm, ";
					str_post = " values ('" + mNewBatchName + "','" + mNewBatchName + "', '" + newBatchPath + "', '" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + "',";

					str_post = str_post + "'"+ Enumerable.Range(0, rdr.FieldCount).Where(l => {
					                                                                     	if (!rdr.IsDBNull(l)) {
					                                                                     		return true;
					                                                                     	}
					                                                                     	else {
					                                                                     		return false;
					                                                                     	}})
						.ToList()
						.Where(x => !rdr.GetName(x).Equals("Batch_path") && !rdr.GetName(x).Equals("batch_name")  && !rdr.GetName(x).Equals("Batch_code") && !rdr.GetName(x).Equals("batch_key") && !rdr.GetName(x).Equals("Created_DTTM"))
						.ToList()
						.Select(x => rdr.GetValue(x).ToString())
						.ToList()
						.Select(l => l.ToString())
						.Aggregate((a, b) => a + "'"+"," + "'" + b ) + "')";
					str_pre = str_pre + Enumerable.Range(0, rdr.FieldCount).Where(l => {
					                                                              	if (!rdr.IsDBNull(l)) {
					                                                              		return true;
					                                                              	}
					                                                              	else {
					                                                              		return false;
					                                                              	}})
						.ToList()
						.Where(x => !rdr.GetName(x).Equals("Batch_path") && !rdr.GetName(x).Equals("batch_name")  && !rdr.GetName(x).Equals("Batch_code") && !rdr.GetName(x).Equals("batch_key") && !rdr.GetName(x).Equals("Created_DTTM"))
						.ToList()
						.Select(x => rdr.GetName(x).ToString())
						.ToList()
						.Select(l => l.ToString())
						.Aggregate((a, b) => a  + "," + b ) + ")";
					str_final = str_pre + str_post;
					OdbcCommand cmd1 = new OdbcCommand(str_final, dbCon, trans);
					cmd1.ExecuteNonQuery();
					flag = 1;
				}
				if(rdr != null)
				{
					rdr.Close();
				}
			}
			catch(OdbcException ex)
			{
				Debug.Print(ex.ToString());
			}
			return flag;
		}
		
		
		private int updateBoxMaster(OdbcTransaction trans)
		{
			//batchName = Path.GetFileName(batchPath.TrimEnd(Path.DirectorySeparatorChar));
			DirectoryInfo parentDir = Directory.GetParent(mBatchPath);
			//dirPath = parentDir.Parent.FullName;
			mDirPath = parentDir.FullName;
			string tempBatchName = mNewBatchName;
			string newBatchPath = mDirPath.Replace("\\","\\\\") + "\\\\" + mNewBatchName;
			mNewBatchPath = newBatchPath;
			
			/* changes in box_master table */
			int flag = -1;
			string str_pre = null;
			string str_post = null;
			string str_final = null;
			OdbcDataReader rdr1 = null;
			string query1 = "select proj_key, batch_key, box_number, box_path, status, created_by, created_dttm, modified_by, modified_dttm, Exported from box_master where batch_key = '"+ mBatchKey +"'";
			OdbcCommand cmd2 = new OdbcCommand(query1,dbCon,trans);
			try{
				rdr1 = cmd2.ExecuteReader();
				while (rdr1.Read())
				{
					string temp_batch_key = rdr1.GetString(1).ToString();
					string temp_box_no = rdr1.GetString(2).ToString();
					if(temp_batch_key.Equals(mBatchKey.ToString()))
					{
						int box_batch_key = getBatchKey(mNewBatchName, mNewBatchName, newBatchPath, trans);
						string temp_boxBatchPath = newBatchPath + "\\\\" + temp_box_no;
						mBoxPath = temp_boxBatchPath;
						str_pre = "insert into box_master (batch_key, box_path, created_dttm, ";
						str_post = " values ('" + box_batch_key + "','" + temp_boxBatchPath + "', '" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + "',";
						
						str_post = str_post + "'"+ Enumerable.Range(0, rdr1.FieldCount).Where(l => {
						                                                                      	if (!rdr1.IsDBNull(l)) {
						                                                                      		return true;
						                                                                      	}
						                                                                      	else {
						                                                                      		return false;
						                                                                      	}})
							.ToList()
							.Where(x => !rdr1.GetName(x).Equals("batch_key") && !rdr1.GetName(x).Equals("box_path")  && !rdr1.GetName(x).Equals("created_dttm"))
							.ToList()
							.Select(x => rdr1.GetValue(x).ToString())
							//.ForEach(b => Debug.Print(rdr.GetValue(b).ToString() + " - " + rdr.GetName(b) + "//"));
							.ToList()
							.Select(l => l.ToString())
							.Aggregate((a, b) => a + "'"+"," + "'" + b ) + "')";
						
						str_pre = str_pre + Enumerable.Range(0, rdr1.FieldCount).Where(l => {
						                                                               	if (!rdr1.IsDBNull(l)) {
						                                                               		return true;
						                                                               	}
						                                                               	else {
						                                                               		return false;
						                                                               	}})
							.ToList()
							.Where(x => !rdr1.GetName(x).Equals("batch_key") && !rdr1.GetName(x).Equals("box_path")  && !rdr1.GetName(x).Equals("created_dttm"))
							.ToList()
							.Select(x => rdr1.GetName(x).ToString())
							.ToList()
							.Select(l => l.ToString())
							
							.Aggregate((a, b) => a  + "," + b ) + ")";
						
						str_final = str_pre + str_post;
						
						OdbcCommand cmd3 = new OdbcCommand(str_final, dbCon, trans);
						cmd3.ExecuteNonQuery();
						flag = 1;
					}
					else
					{
						MessageBox.Show("No records found for Batch Name: " + mOldBatchName);
					}
				}
				if(rdr1 != null)
				{
					rdr1.Close();
				}
			}
			catch(OdbcException ex)
			{
				MessageBox.Show(ex.ToString());
			}
			return flag;
		}
		
		private int updatePolicyMaster(OdbcTransaction trans)
		{
//			DirectoryInfo parentDir = Directory.GetParent(mBatchPath);
//			//dirPath = parentDir.Parent.FullName;
//			mDirPath = parentDir.FullName;
//			string tempBatchName = mNewBatchName;
//			string newBatchPath = mDirPath.Replace("\\","\\\\") + "\\\\" + mNewBatchName;
			
			
			/*rename old policy*/
			//bool folderExists = Directory.Exists(mBatchPath);
			//string old_BatchFolder = mDirPath.Replace("\\","\\\\") + "\\\\old_" + mOldBatchName;
			
			
			/* changes in policy_master table */
			int flag = -1;
			string str_pre = null;
			string str_post = null;
			string str_final = null;
			OdbcDataReader rdr1 = null;
			int policy_batch_key = getBatchKey(mNewBatchName, mNewBatchName, mNewBatchPath, trans);
			
			string query1 = "select proj_key, batch_key, box_number, policy_number, policy_path, created_by," +
				"created_dttm, modified_by, modified_dttm, count_of_pages, status, scan_upload_flag, scanned_date, incremented_scan, " +
				"lic_checked, photo, custom_exception, locked_uid, expires_dttm, invalid, validation_status, do_code, br_code, year, " +
				"deed_year, deed_no, deed_vol, page_from, page_to, run_no from policy_master where batch_key = '"+ mBatchKey +"'";
			OdbcCommand cmd2 = new OdbcCommand(query1,dbCon,trans);
			try{
				rdr1 = cmd2.ExecuteReader();
				while (rdr1.Read())
				{
					string old_policy_no = rdr1.GetString(3).ToString();
					string old_policy_path = rdr1.GetString(4).ToString();
					string temp_deed_no = rdr1.GetString(25).ToString();
					string temp_policy_number = mNewBatchName.Substring(0, 9) + "[" + temp_deed_no + "]";
					string temp_policy_path = mBoxPath + "\\\\" + temp_policy_number;
					int deedYEAR = Convert.ToInt32(rdr1.GetString(24).ToString());
					//DateTime dt = Convert.ToDateTime(rdr1.GetValue(12));
					
					if(mYear != deedYEAR)
					{
						/* copy batch folder */
						string pre_dir = (mBoxPath + "\\\\" + old_policy_no).Replace("\\\\","\\");
						string post_dir = temp_policy_path.Replace("\\\\","\\");
						
						Directory.Move(pre_dir,post_dir);
						
						bool folderExists = Directory.Exists(post_dir);
						
						if(folderExists)
						{
							//Directory.Move(temp_policy_path, old_policy_path);
							if(Directory.Exists(post_dir + "\\" + "Scan"))
							{
								DirectoryInfo d = new DirectoryInfo(post_dir + "\\" + "Scan");
								FileInfo[] infos = d.GetFiles();
								foreach(FileInfo f in infos)
								{
									int pos = f.Name.IndexOf('_');
									
									string newFile = post_dir + "\\" + "Scan" + "\\" + temp_policy_number + f.Name.Substring(pos, f.Name.Length - pos);
									//string newFile = temp_policy_number.Substring(0, temp_policy_number.IndexOf('_')) + f.Name.Substring(f.Name.IndexOf('_'), f.Name.Length - f.Name.IndexOf('_')).ToString();
									//string newFile = f.FullName.Substring(0, 9).ToString();
									//newFile = newFile.Replace(newFile, temp_policy_number);
									File.Move(f.FullName, newFile);
								}
							}
							
							if(Directory.Exists(post_dir + "\\" + "QC"))
							{
								DirectoryInfo d = new DirectoryInfo(post_dir + "\\" + "QC");
								FileInfo[] infos = d.GetFiles();
								foreach(FileInfo f in infos)
								{
									int pos = f.Name.IndexOf('_');
									
									string newFile = post_dir + "\\" + "QC" + "\\" + temp_policy_number + f.Name.Substring(pos, f.Name.Length - pos);
									//string newFile = temp_policy_number.Substring(0, temp_policy_number.IndexOf('_')) + f.Name.Substring(f.Name.IndexOf('_'), f.Name.Length - f.Name.IndexOf('_')).ToString();
									//string newFile = f.FullName.Substring(0, 9).ToString();
									//newFile = newFile.Replace(newFile, temp_policy_number);
									File.Move(f.FullName, newFile);
								}
							}
						}
					}
					
					
					
					
					str_pre = "insert into policy_master (batch_key, policy_number, policy_path, deed_year, created_dttm, modified_dttm, scanned_date, ";
					str_post = " values ('" + policy_batch_key + "','" + temp_policy_number + "','" + temp_policy_path + "','" + mYear + "','" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + "','" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + "','" + DateTime.Now.ToString("yyyy-MM-dd") + "',";
					
					str_post = str_post + "'"+ Enumerable.Range(0, rdr1.FieldCount).Where(l => {
					                                                                      	if (!rdr1.IsDBNull(l)) {
					                                                                      		return true;
					                                                                      	}
					                                                                      	else {
					                                                                      		return false;
					                                                                      	}})
						.ToList()
						.Where(x => !rdr1.GetName(x).Equals("batch_key") && !rdr1.GetName(x).Equals("policy_number")  && !rdr1.GetName(x).Equals("policy_path") && !rdr1.GetName(x).Equals("deed_year") && !rdr1.GetName(x).Equals("created_dttm") && !rdr1.GetName(x).Equals("modified_dttm")&& !rdr1.GetName(x).Equals("scanned_date"))
						.ToList()
						.Select(x => rdr1.GetValue(x).ToString())
						//.ForEach(b => Debug.Print(rdr.GetValue(b).ToString() + " - " + rdr.GetName(b) + "//"));
						.ToList()
						.Select(l => l.ToString())
						.Aggregate((a, b) => a + "'"+"," + "'" + b ) + "')";
					
					str_pre = str_pre + Enumerable.Range(0, rdr1.FieldCount).Where(l => {
					                                                               	if (!rdr1.IsDBNull(l)) {
					                                                               		return true;
					                                                               	}
					                                                               	else {
					                                                               		return false;
					                                                               	}})
						.ToList()
						.Where(x => !rdr1.GetName(x).Equals("batch_key") && !rdr1.GetName(x).Equals("policy_number")  && !rdr1.GetName(x).Equals("policy_path") && !rdr1.GetName(x).Equals("deed_year") && !rdr1.GetName(x).Equals("created_dttm") && !rdr1.GetName(x).Equals("modified_dttm") && !rdr1.GetName(x).Equals("scanned_date"))
						.ToList()
						.Select(x => rdr1.GetName(x).ToString())
						.ToList()
						.Select(l => l.ToString())
						
						.Aggregate((a, b) => a  + "," + b ) + ")";
					
					str_final = str_pre + str_post;
					
					OdbcCommand cmd3 = new OdbcCommand(str_final, dbCon, trans);
					cmd3.ExecuteNonQuery();
					flag = 1;
				}
				if(rdr1 != null)
				{
					rdr1.Close();
				}
			}
			catch(OdbcException ex)
			{
				MessageBox.Show(ex.ToString());
			}
			return flag;
		}
		
		
		private int updateImageMaster(OdbcTransaction trans)
		{
			
			/* changes in image_master table */
			int flag = -1;
			string str_pre = null;
			string str_post = null;
			string str_final = null;
			OdbcDataReader rdr1 = null;
			int image_batch_key = getBatchKey(mNewBatchName, mNewBatchName, mNewBatchPath, trans);
			
			string query1 = "select proj_key, batch_key, box_number, policy_number, serial_no, page_index_name, " +
				"created_by, created_dttm, modified_by, modified_dttm, Page_name, status, Doc_Type, SCanned_size, " +
				"QC_size, fqc_size, index_size, Photo, Image_seq from image_master where batch_key = '"+ mBatchKey +"'";
			OdbcCommand cmd2 = new OdbcCommand(query1,dbCon,trans);
			try{
				rdr1 = cmd2.ExecuteReader();
				while (rdr1.Read())
				{
					string policy_no = rdr1.GetString(3).ToString();
					string page_index_name = rdr1.GetString(5).ToString();
					string page_name = rdr1.GetString(10).ToString();
					
					policy_no = mNewBatchName.Substring(0, 9) + policy_no.Substring(9, policy_no.Length - 9);
					page_index_name = mNewBatchName.Substring(0, 9) + page_index_name.Substring(9, page_index_name.Length - 9);
					page_name = mNewBatchName.Substring(0, 9) + page_name.Substring(9, page_name.Length - 9);
					
					//DateTime dt = Convert.ToDateTime(rdr1.GetValue(12));
					
					
					str_pre = "insert into image_master (batch_key, policy_number, page_index_name, page_name, created_dttm, modified_dttm, ";
					str_post = " values ('" + image_batch_key + "','" + policy_no + "','" + page_index_name + "','" + page_name + "','" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + "','" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + "',";
					
					str_post = str_post + "'"+ Enumerable.Range(0, rdr1.FieldCount).Where(l => {
					                                                                      	if (!rdr1.IsDBNull(l)) {
					                                                                      		return true;
					                                                                      	}
					                                                                      	else {
					                                                                      		return false;
					                                                                      	}})
						.ToList()
						.Where(x => !rdr1.GetName(x).Equals("batch_key") && !rdr1.GetName(x).Equals("policy_number")  && !rdr1.GetName(x).Equals("page_index_name") && !rdr1.GetName(x).Equals("Page_name") && !rdr1.GetName(x).Equals("created_dttm") && !rdr1.GetName(x).Equals("modified_dttm"))
						.ToList()
						.Select(x => rdr1.GetValue(x).ToString())
						//.ForEach(b => Debug.Print(rdr.GetValue(b).ToString() + " - " + rdr.GetName(b) + "//"));
						.ToList()
						.Select(l => l.ToString())
						.Aggregate((a, b) => a + "'"+"," + "'" + b ) + "')";
					
					str_pre = str_pre + Enumerable.Range(0, rdr1.FieldCount).Where(l => {
					                                                               	if (!rdr1.IsDBNull(l)) {
					                                                               		return true;
					                                                               	}
					                                                               	else {
					                                                               		return false;
					                                                               	}})
						.ToList()
						.Where(x => !rdr1.GetName(x).Equals("batch_key") && !rdr1.GetName(x).Equals("policy_number")  && !rdr1.GetName(x).Equals("page_index_name") && !rdr1.GetName(x).Equals("Page_name") && !rdr1.GetName(x).Equals("created_dttm") && !rdr1.GetName(x).Equals("modified_dttm"))
						.ToList()
						.Select(x => rdr1.GetName(x).ToString())
						.ToList()
						.Select(l => l.ToString())
						
						.Aggregate((a, b) => a  + "," + b ) + ")";
					
					str_final = str_pre + str_post;
					
					OdbcCommand cmd3 = new OdbcCommand(str_final, dbCon, trans);
					cmd3.ExecuteNonQuery();
					flag = 1;
				}
				if(rdr1 != null)
				{
					rdr1.Close();
				}
			}
			catch(OdbcException ex)
			{
				MessageBox.Show(ex.ToString());
			}
			return flag;
		}
		
	}
}
