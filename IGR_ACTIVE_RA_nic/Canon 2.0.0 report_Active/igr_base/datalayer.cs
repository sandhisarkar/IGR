using System;
using System.Collections.Generic;
using System.Text;
using DataLayerDefs;
using System.IO;
using System.Data.OleDb;
using System.Data;
using System.Data.Odbc;
using System.ComponentModel;
using System.Collections;
using NovaNet.Utils;
using System.Windows.Forms;

namespace IGR
{
    public enum OutputJob
    {
        Export,
        Reexport
    }
    public class DataLayer
    {
        Credentials crd = new Credentials();
        OdbcConnection conn = new OdbcConnection();
        OdbcTransaction txn;
       // ExceptionDetailsDeed exp;
        public DataLayer(OdbcConnection pCon, OdbcTransaction pTxn, Credentials prmCrd)
        {
            conn = pCon;
            crd = prmCrd;
            txn = pTxn;
        }
#region v2 
#region v2.selection

        public DataTable _GetAllDeeds()
        {
            DataTable dt = new DataTable();
            string sql = "Select * from deed_details";
            OdbcCommand cmd = new OdbcCommand(sql, conn, txn);
            OdbcDataAdapter odap = new OdbcDataAdapter(cmd);
            odap.Fill(dt);
            return dt;
        }
        public DataTable _GetAllVolumes()
        {
        	DataTable dt = new DataTable();
            string sql = "select distinct a.district_code as District_Code, b.district_name as District_Name, a.Ro_Code as RO_Code, c.ro_name as RO_Name, a.deed_year as Year, a.book as Book, a.volume_no as Volume, count(*) as Nos from deed_details a, district b, ro_master c where a.district_code=b.district_code and a.District_Code=c.district_code and a.RO_Code=c.ro_code group by a.District_Code, a.RO_Code, a.deed_year, a.book, a.volume_no";
            OdbcCommand cmd = new OdbcCommand(sql, conn, txn);
            OdbcDataAdapter odap = new OdbcDataAdapter(cmd);
            odap.Fill(dt);
            return dt;
        }
        public DataTable _GetDeeds_Volume(Volumes pVol)
        {
        	DataTable dt = new DataTable();
            string sql = 	"select district_code as District_code, ro_code as RO_Code, " +
				           	"deed_year as Year, book as Book, volume_no as Volume, deed_no as Deed_no " +
            				"from deed_details where district_code='" + pVol.District_Code + "' " +
		            		"and ro_code='" + pVol.Ro_Code + "' and deed_year='" + pVol.Year + "' " +
                            "and Book='" + pVol.Book + "' and Volume_no='" + pVol.Volume + "' ";
            OdbcCommand cmd = new OdbcCommand(sql, conn, txn);
            OdbcDataAdapter odap = new OdbcDataAdapter(cmd);
            odap.Fill(dt);
            return dt;
        }
        public DataTable _GetDeed(DeedControl pDc)
        {
        	DataTable dt = new DataTable();
            string sql = "SELECT District_Code,RO_Code,Book, Deed_year, Deed_no, Serial_No, Serial_Year, tran_maj_code, tran_min_code, Volume_No, Page_From, Page_To,date_format(Date_of_Completion,'%Y/%m/%d') as Date_of_Completion,date_format(Date_of_Delivery,'%Y/%m/%d') as Date_of_Delivery,Deed_Remarks, Created_DTTM,Exported,created_by,modified_by,modified_dttm,MisMatch,Scan_doc_type,addl_pages,hold,hold_reason,status,created_system,version FROM deed_details " +
            				"where district_code='" + pDc.District_code + "' " +
		            		"and ro_code='" + pDc.RO_code + "' and deed_year='" + pDc.Deed_year + "' " + 
            				"and Book='" + pDc.Book + "' and deed_no='" + pDc.Deed_no + "'";
            OdbcCommand cmd = new OdbcCommand(sql, conn, txn);
            OdbcDataAdapter odap = new OdbcDataAdapter(cmd);
            odap.Fill(dt);
            return dt;
        }
        public DataTable _GetDeedExp(DeedControl pDc)
        {
        	DataTable dt = new DataTable();
            string sql = "select District_Code,RO_Code,Book,deed_year,Deed_no,srl_no,Exception,details from deed_details_exception " +
            				"where district_code='" + pDc.District_code + "' " +
		            		"and ro_code='" + pDc.RO_code + "' and deed_year='" + pDc.Deed_year + "' " + 
            				"and Book='" + pDc.Book + "' and deed_no='" + pDc.Deed_no + "'";
            OdbcCommand cmd = new OdbcCommand(sql, conn, txn);
            OdbcDataAdapter odap = new OdbcDataAdapter(cmd);
            odap.Fill(dt);
            return dt;
        }
        public DataTable _GetoutWBList(DeedControl pDc)
        {
            DataTable dt = new DataTable();
            string sql = "select District_Code,RO_Code,Book,deed_year,Deed_no,item_no,isOutsideWB from tbloutsidewblist " +
                            "where district_code='" + pDc.District_code + "' " +
                            "and ro_code='" + pDc.RO_code + "' and deed_year='" + pDc.Deed_year + "' " +
                            "and Book='" + pDc.Book + "' and deed_no='" + pDc.Deed_no + "'";
            OdbcCommand cmd = new OdbcCommand(sql, conn, txn);
            OdbcDataAdapter odap = new OdbcDataAdapter(cmd);
            odap.Fill(dt);
            return dt;
        }
        public DataTable  _GetDeedExists(DeedControl pDc)
        {
            DataTable dt = new DataTable();
            string sql = "SELECT count(*),volume_no FROM deed_details " +
                            "where district_code='" + pDc.District_code + "' " +
                            "and ro_code='" + pDc.RO_code + "' and deed_year='" + pDc.Deed_year + "' " +
                            "and Book='" + pDc.Book + "' and deed_no='" + pDc.Deed_no + "' group by volume_no";
            OdbcCommand cmd = new OdbcCommand(sql, conn, txn);
            OdbcDataAdapter odap = new OdbcDataAdapter(cmd);
            odap.Fill(dt);
            return dt;
        }
        public DataTable _GetPersons(DeedControl pDc)
        {
        	DataTable dt = new DataTable();
            string sql = 	"select b.District_Code, b.RO_Code,b.Book,b.Deed_year,b.Deed_no,b.Item_no,initial_name,First_name,Last_name,Party_code,Admit_code,Address,Address_district_code,Address_district_name,Address_ps_code,Address_ps_name,Father_mother,Rel_code,Relation,occupation_code,religion_code,b.Created_DTTM,b.Created_by,more,pin,city,other_party_code,linked_to FROM index_of_name b, deed_details a " +
            				"where a.district_code='" + pDc.District_code + "' " +
		            		"and a.ro_code='" + pDc.RO_code + "' and a.deed_year='" + pDc.Deed_year + "' " + 
            				"and a.Book='" + pDc.Book + "' and a.deed_no='" + pDc.Deed_no + "' " +
            				"and a.district_code=b.district_code and a.ro_code = b.ro_code and a.deed_year=b.deed_year and a.book=b.book and a.deed_no=b.deed_no " +
            				"order by b.Item_no";
            OdbcCommand cmd = new OdbcCommand(sql, conn, txn);
            OdbcDataAdapter odap = new OdbcDataAdapter(cmd);
            odap.Fill(dt);
            return dt;
        }
        public DataTable _GetPersonsExcp(DeedControl pDc)
        {
        	DataTable dt = new DataTable();
            string sql = "select b.District_Code, b.RO_Code, b.Book, b.Deed_year, b.Deed_no,b.item_no,b.srl_no,b.Exception,b.details from index_of_name_exception b, deed_details a " +
            				"where a.district_code='" + pDc.District_code + "' " +
		            		"and a.ro_code='" + pDc.RO_code + "' and a.deed_year='" + pDc.Deed_year + "' " +
                            "and a.Book='" + pDc.Book + "' and a.deed_no='" + pDc.Deed_no + "' " +
                            "and a.district_code=b.district_code and a.ro_code = b.ro_code and a.deed_year=b.deed_year and a.book=b.book and a.deed_no=b.deed_no " +
            				"order by b.srl_no";
            OdbcCommand cmd = new OdbcCommand(sql, conn, txn);
            OdbcDataAdapter odap = new OdbcDataAdapter(cmd);
            odap.Fill(dt);
            return dt;
        }
        public DataTable _GetPersonsExcpforDisplay(DeedControl pDc)
        {
            DataTable dt = new DataTable();
            string sql = "select b.District_Code, b.RO_Code, b.Book, b.Deed_year, b.Deed_no,b.item_no,b.srl_no,c.ExcepTion_Name as Exception from index_of_name_exception b, deed_details a, tblexception c " +
                            "where a.district_code='" + pDc.District_code + "' " +
                            "and a.ro_code='" + pDc.RO_code + "' and a.deed_year='" + pDc.Deed_year + "' " +
                            "and a.Book='" + pDc.Book + "' and a.deed_no='" + pDc.Deed_no + "' and b.Exception = c.ExcepTion_Code " +
                            "and a.district_code=b.district_code and a.ro_code = b.ro_code and a.deed_year=b.deed_year and a.book=b.book and a.deed_no=b.deed_no " +
                            "order by b.srl_no";
            OdbcCommand cmd = new OdbcCommand(sql, conn, txn);
            OdbcDataAdapter odap = new OdbcDataAdapter(cmd);
            odap.Fill(dt);
            return dt;
        }
        public DataTable _GetPropertyExcp(DeedControl pDc)
        {
        	DataTable dt = new DataTable();
            string sql = "select b.District_Code, b.RO_Code, b.Book, b.Deed_year, b.Deed_no,b.item_no,b.srl_no,b.Exception,b.details from index_of_property_exception b, deed_details a " +
            				"where a.district_code='" + pDc.District_code + "' " +
		            		"and a.ro_code='" + pDc.RO_code + "' and a.deed_year='" + pDc.Deed_year + "' " +
                            "and a.Book='" + pDc.Book + "' and a.deed_no='" + pDc.Deed_no + "' " +
            				"and a.district_code=b.district_code and a.ro_code = b.ro_code and a.deed_year=b.deed_year and a.book=b.book and a.deed_no=b.deed_no " +
            				"order by b.srl_no";
            OdbcCommand cmd = new OdbcCommand(sql, conn, txn);
            OdbcDataAdapter odap = new OdbcDataAdapter(cmd);
            odap.Fill(dt);
            return dt;
        }
        public DataTable _GetPropertyExcpforDisplay(DeedControl pDc)
        {
            DataTable dt = new DataTable();
            string sql = "select b.District_Code, b.RO_Code, b.Book, b.Deed_year, b.Deed_no,b.item_no,b.srl_no,c.ExcepTion_Name as Exception from index_of_property_exception b, deed_details a, tblexception c " +
                            "where a.district_code='" + pDc.District_code + "' " +
                            "and a.ro_code='" + pDc.RO_code + "' and a.deed_year='" + pDc.Deed_year + "' " +
                            "and a.Book='" + pDc.Book + "' and a.deed_no='" + pDc.Deed_no + "' and b.Exception = c.ExcepTion_Code " +
                            "and a.district_code=b.district_code and a.ro_code = b.ro_code and a.deed_year=b.deed_year and a.book=b.book and a.deed_no=b.deed_no " +
                            "order by b.srl_no";
            OdbcCommand cmd = new OdbcCommand(sql, conn, txn);
            OdbcDataAdapter odap = new OdbcDataAdapter(cmd);
            odap.Fill(dt);
            return dt;
        }

        public string _GetExceptionNo(string Tablename, DeedControl pDc, int item_no = -1)
        {
            DataSet dt = new DataSet();
            string _expNo = string.Empty;
            string sql = "select Exception from " + Tablename +
                         "where district_code='" + pDc.District_code + "' " +
                            "and ro_code='" + pDc.RO_code + "' and deed_year='" + pDc.Deed_year + "' and Book='" + pDc.Book + "' and deed_no='" + pDc.Deed_no + "'";

            OdbcCommand cmd = new OdbcCommand(sql, conn, txn);
            OdbcDataAdapter odap = new OdbcDataAdapter(cmd);
            odap.Fill(dt);
            if (dt.Tables[0].Rows.Count > 0)
            {
                _expNo = dt.Tables[0].Rows[0][0].ToString();
            }
            return _expNo;
        }
        public DataTable _GetPropertiesWB(DeedControl pDc)
        {
            DataTable dt = new DataTable();
            string sql = "SELECT a.District_Code, a.RO_Code, a.Book, a.Deed_year, a.Deed_no,Item_no,Property_country_code,Property_state_code,Property_district_code,thana,moucode,plot_code_type,plot_no,khatian_type,khatian_no,land_use,property_type,b.created_dttm,b.created_by,b.local_body_type,b.other_details,b.area_bigha,b.area_decimal,b.area_katha,b.area_chatak,b.area_sqf,b.total_area_decimal,b.struct_sqfeet,b.area_acre FROM index_of_property_out_wb b, deed_details a " +
                            "where a.district_code='" + pDc.District_code + "' " +
                            "and a.ro_code='" + pDc.RO_code + "' and a.deed_year='" + pDc.Deed_year + "' " +
                            "and a.Book='" + pDc.Book + "' and a.deed_no='" + pDc.Deed_no + "' " +
                            "and a.district_code=b.district_code and a.ro_code = b.ro_code and a.deed_year=b.deed_year and a.book=b.book and a.deed_no=b.deed_no " +
                            "order by b.Item_no";
            OdbcCommand cmd = new OdbcCommand(sql, conn, txn);
            OdbcDataAdapter odap = new OdbcDataAdapter(cmd);
            odap.Fill(dt);
            return dt;
        }
        public DataTable _GetProperties(DeedControl pDc)
        {
        	DataTable dt = new DataTable();
            string sql = 	"SELECT a.District_Code, a.RO_Code, a.Book, a.Deed_year, a.Deed_no,Item_no,Property_district_code,Property_ro_code,ps_code,moucode,Area_type,GP_Muni_Corp_Code,Ward,Holding,Premises,road_code,Plot_code_type,Road,Plot_No,Bata_No,Khatian_type,khatian_No,bata_khatian_no,property_type,Land_Area_acre,Land_Area_bigha,Land_Area_decimal,Land_Area_katha,Land_Area_chatak,Land_Area_sqfeet,Structure_area_in_sqFeet,b.Created_DTTM,b.Created_by,ref_ps,ref_mouza,JL_no,Other_plots,Other_Khatian,land_type,RefJL_no FROM index_of_property b, deed_details a " +
            				"where a.district_code='" + pDc.District_code + "' " +
		            		"and a.ro_code='" + pDc.RO_code + "' and a.deed_year='" + pDc.Deed_year + "' " + 
            				"and a.Book='" + pDc.Book + "' and a.deed_no='" + pDc.Deed_no + "' " +
            				"and a.district_code=b.district_code and a.ro_code = b.ro_code and a.deed_year=b.deed_year and a.book=b.book and a.deed_no=b.deed_no " +
            				"order by b.Item_no";
            OdbcCommand cmd = new OdbcCommand(sql, conn, txn);
            OdbcDataAdapter odap = new OdbcDataAdapter(cmd);
            odap.Fill(dt);
            return dt;
        }
        public DataTable _GetOtherPlots(DeedControl pDc)
        {
        	DataTable dt = new DataTable();
            string sql = 	"SELECT a.district_code,a.ro_code,book,a.deed_year,a.deed_no,a.item_no,a.other_plots FROM tblother_plots a " +
            				"where a.district_code='" + pDc.District_code + "' " +
		            		"and a.ro_code='" + pDc.RO_code + "' and a.deed_year='" + pDc.Deed_year + "' " + 
            				"and a.Book='" + pDc.Book + "' and a.deed_no='" + pDc.Deed_no + "' " +
            				"order by a.Item_no";
            OdbcCommand cmd = new OdbcCommand(sql, conn, txn);
            System.Diagnostics.Debug.Print(sql);
            OdbcDataAdapter odap = new OdbcDataAdapter(cmd);
            odap.Fill(dt);
            return dt;
        }
        public DataTable _GetOtherKhatians(DeedControl pDc)
        {
        	DataTable dt = new DataTable();
            string sql = 	"SELECT a.district_code,a.ro_code,book,a.deed_year,a.deed_no,a.item_no,a.other_khatian FROM tbl_other_khatian a " +
            				"where a.district_code='" + pDc.District_code + "' " +
		            		"and a.ro_code='" + pDc.RO_code + "' and a.deed_year='" + pDc.Deed_year + "' " + 
            				"and a.Book='" + pDc.Book + "' and a.deed_no='" + pDc.Deed_no + "' " +
            				"order by a.Item_no";
            OdbcCommand cmd = new OdbcCommand(sql, conn, txn);
            System.Diagnostics.Debug.Print(sql);
            OdbcDataAdapter odap = new OdbcDataAdapter(cmd);
            odap.Fill(dt);
            return dt;
        }
#endregion        
#region v2.updates
        
		public bool _UpdateDeed(Deed pDd)
		{
			bool retVal = false;
            string sql = string.Empty;

            sql = "UPDATE deed_details SET   " +
                    "`District_Code` = '" + pDd.DeedHeader.Deed_control.District_code + "', " +
                    "`RO_Code` = '" + pDd.DeedHeader.Deed_control.RO_code + "', " +
                    "`Book` = '" + pDd.DeedHeader.Deed_control.Book + "', " +
                    "`Deed_year` = '" + pDd.DeedHeader.Deed_control.Deed_year + "', " +
                    "`Deed_no` = '" + pDd.DeedHeader.Deed_control.Deed_no + "', " +
                    "`Serial_No` = '" + pDd.DeedHeader.Serial_no + "', " +
                    "`Serial_Year` = '" + pDd.DeedHeader.Serial_year + "', " +
                    "`tran_maj_code` = '" + pDd.DeedHeader.tran_maj_code + "', " +
                    "`tran_min_code` = '" + pDd.DeedHeader.tran_min_code + "', " +
                    "`Volume_No` = '" + pDd.DeedHeader.volume_no + "', " +
                    "`Page_From` = '" + pDd.DeedHeader.page_from + "', " +
                    "`Page_To` = '" + pDd.DeedHeader.page_to + "', " +
                    "`Deed_Remarks` = '" + pDd.DeedHeader.deed_remarks + "', " +
                    "`Exported` = '" + pDd.DeedHeader.Exported + "', " +
                    "`Scan_doc_type` = '" + pDd.DeedHeader.scan_doc_type + "', " +
                    "`addl_pages` = '" + pDd.DeedHeader.addl_pages + "', " +
                    "`hold` = '" + pDd.DeedHeader.hold + "', " +
                    "`hold_reason` = '" + pDd.DeedHeader.hold_reason + "', " +
                    "`status` = '" + pDd.DeedHeader.status + "', " +
                    "`created_system` = '" + pDd.DeedHeader.created_system + "', " +
                    "`created_by` = '" + crd.created_by + "', " +
                    "`created_dttm` = '" + crd.created_dttm + "', " +
                    "`version` = '" + pDd.DeedHeader.version + "' ,";
            if (pDd.DeedHeader.date_of_completion != null && pDd.DeedHeader.date_of_completion != "")
                sql = sql + "`Date_of_Completion` = date_format('" + pDd.DeedHeader.date_of_completion + "','%Y/%m/%d'), ";
            if (pDd.DeedHeader.date_of_delivery != null && pDd.DeedHeader.date_of_delivery != "")
                sql = sql + "`Date_of_Delivery` = date_format('" + pDd.DeedHeader.date_of_delivery + "','%Y/%m/%d'), ";
            sql = sql.Substring(0, sql.Trim().Length -1);
            sql = sql + "WHERE `District_Code` = '" + pDd.DeedHeader.Deed_control.District_code + "' AND '" + pDd.DeedHeader.Deed_control.RO_code + "' AND `Book` = '" + pDd.DeedHeader.Deed_control.Book + "' AND `Deed_year` = '" + pDd.DeedHeader.Deed_control.Deed_year + "' AND `Deed_no` = '" + pDd.DeedHeader.Deed_control.Deed_no + "'"; 
					

            System.Diagnostics.Debug.Print(sql);
            OdbcCommand cmd = new OdbcCommand(sql, conn, txn);

            if (cmd.ExecuteNonQuery() >= 0)
            {
				retVal = true;
            }
            return retVal;
		}

        public bool _RenameDeedNo(DeedControl pDd,DeedControl ndc, string pVolumeNo)
		{

			bool retVal = false;
            string sql = string.Empty;

            sql = "UPDATE deed_details SET   " +
                    "`Deed_no` = '" + ndc.Deed_no + "', " +
                    "`Serial_No` = '" + ndc.Deed_no + "', " +
            		"`volume_no` = '" + pVolumeNo + "', " +
                    "Deed_year = '" + ndc.Deed_year + "'";
            sql = sql + "WHERE `District_Code` = '" + pDd.District_code + "' AND '" + pDd.RO_code + "' AND `Book` = '" + pDd.Book + "' AND `Deed_year` = '" + pDd.Deed_year + "' AND `Deed_no` = '" + pDd.Deed_no + "'"; 
					

            System.Diagnostics.Debug.Print(sql);
            OdbcCommand cmd = new OdbcCommand(sql, conn, txn);

            if (cmd.ExecuteNonQuery() > 0)
            {
				retVal = true;
            }
            
            return retVal;
            return true;
		}
        public bool _RenameDeedVolume(DeedControl pDd,string pVolume)
		{

			bool retVal = false;
            string sql = string.Empty;

            sql = "UPDATE deed_details SET   " +
                    "volume_no = '" + pVolume + "' ";
            sql = sql + "WHERE `District_Code` = '" + pDd.District_code + "' AND '" + pDd.RO_code + "' AND `Book` = '" + pDd.Book + "' AND `Deed_year` = '" + pDd.Deed_year + "' AND `Deed_no` = '" + pDd.Deed_no + "'"; 
					

            System.Diagnostics.Debug.Print(sql);
            OdbcCommand cmd = new OdbcCommand(sql, conn, txn);

            if (cmd.ExecuteNonQuery() > 0)
            {
				retVal = true;
            }
            
            return retVal;
            return true;
		}
        public bool _RenameIndex1(DeedControl pDd,DeedControl ndc)
        {
            bool retVal = false;
            string sql = string.Empty;

            sql = "UPDATE index_of_name SET   " +
                    "`Deed_year` = '" + ndc.Deed_year + "', " +
                    "`Deed_no` = '" + ndc.Deed_no + "' ";
            sql = sql + "WHERE `District_Code` = '" + pDd.District_code + "' AND '" + pDd.RO_code + "' AND `Book` = '" + pDd.Book + "' AND `Deed_year` = '" + pDd.Deed_year + "' AND `Deed_no` = '" + pDd.Deed_no + "'";


            System.Diagnostics.Debug.Print(sql);
            OdbcCommand cmd = new OdbcCommand(sql, conn, txn);

            if (cmd.ExecuteNonQuery() >= 0)
            {
                retVal = true;
            }
            return retVal;
        }
        public bool _RenameIndex2(DeedControl pDd,DeedControl ndc)
        {
            bool retVal = false;
            string sql = string.Empty;

            sql = "UPDATE index_of_property SET   " +
                    "`Deed_year` = '" + ndc.Deed_year + "', " +
                    "`Deed_no` = '" + ndc.Deed_no + "' ";
            sql = sql + "WHERE `District_Code` = '" + pDd.District_code + "' AND '" + pDd.RO_code + "' AND `Book` = '" + pDd.Book + "' AND `Deed_year` = '" + pDd.Deed_year + "' AND `Deed_no` = '" + pDd.Deed_no + "'";


            System.Diagnostics.Debug.Print(sql);
            OdbcCommand cmd = new OdbcCommand(sql, conn, txn);

            if (cmd.ExecuteNonQuery() >= 0)
            {
                retVal = true;
            }
            return retVal;
        }
        public bool _RenameOtherplots(DeedControl pDd, DeedControl ndc)
        {
            bool retVal = false;
            string sql = string.Empty;

            sql = "UPDATE tblother_plots SET   " +
                    "`Deed_year` = '" + ndc.Deed_year + "', " +
                    "`Deed_no` = '" + ndc.Deed_no + "' ";
            sql = sql + "WHERE `District_Code` = '" + pDd.District_code + "' AND '" + pDd.RO_code + "' AND `Book` = '" + pDd.Book + "' AND `Deed_year` = '" + pDd.Deed_year + "' AND `Deed_no` = '" + pDd.Deed_no + "'";


            System.Diagnostics.Debug.Print(sql);
            OdbcCommand cmd = new OdbcCommand(sql, conn, txn);

            if (cmd.ExecuteNonQuery() >= 0)
            {
                retVal = true;
            }
            return retVal;
        }
        public bool _RenameOtherKhatian(DeedControl pDd, DeedControl ndc)
        {
            bool retVal = false;
            string sql = string.Empty;

            sql = "UPDATE tbl_other_khatian SET   " +
                    "`Deed_year` = '" + ndc.Deed_year + "', " +
                    "`Deed_no` = '" + ndc.Deed_no + "' ";
            sql = sql + "WHERE `District_Code` = '" + pDd.District_code + "' AND '" + pDd.RO_code + "' AND `Book` = '" + pDd.Book + "' AND `Deed_year` = '" + pDd.Deed_year + "' AND `Deed_no` = '" + pDd.Deed_no + "'";


            System.Diagnostics.Debug.Print(sql);
            OdbcCommand cmd = new OdbcCommand(sql, conn, txn);

            if (cmd.ExecuteNonQuery() >= 0)
            {
                retVal = true;
            }
            return retVal;
        }
#endregion
#region v2.delete
        public bool _DeleteIndex1(DeedControl pDd)
        {
            bool retVal = false;
            string sql = string.Empty;

            sql = "delete from index_of_name "+
                  "WHERE `District_Code` = '" + pDd.District_code + "' AND '" + pDd.RO_code + "' AND `Book` = '" + pDd.Book  + "' AND `Deed_year` = '" + pDd.Deed_year  + "' AND `Deed_no` = '" + pDd.Deed_no  + "'";
            OdbcCommand cmd = new OdbcCommand(sql, conn, txn);
            if (cmd.ExecuteNonQuery() >= 0)
            {
                retVal = true;
            }
            return retVal;
        }
        public bool _DeletedeedExp(DeedControl pDd)
        {
            bool retVal = false;
            string sql = string.Empty;

            sql = "delete from deed_details_exception " +
                  "WHERE `District_Code` = '" + pDd.District_code + "' AND '" + pDd.RO_code + "' AND `Book` = '" + pDd.Book + "' AND `Deed_year` = '" + pDd.Deed_year + "' AND `Deed_no` = '" + pDd.Deed_no + "'";
            OdbcCommand cmd = new OdbcCommand(sql, conn, txn);
            if (cmd.ExecuteNonQuery() >= 0)
            {
                retVal = true;
            }
            return retVal;
        }
        public bool _DeletepersonExp(DeedControl pDd)
        {
            bool retVal = false;
            string sql = string.Empty;

            sql = "delete from index_of_name_exception " +
                  "WHERE `District_Code` = '" + pDd.District_code + "' AND '" + pDd.RO_code + "' AND `Book` = '" + pDd.Book + "' AND `Deed_year` = '" + pDd.Deed_year + "' AND `Deed_no` = '" + pDd.Deed_no + "'";
            OdbcCommand cmd = new OdbcCommand(sql, conn, txn);
            if (cmd.ExecuteNonQuery() >= 0)
            {
                retVal = true;
            }
            return retVal;
        }
        public bool _DeletePropertyExp(DeedControl pDd)
        {
            bool retVal = false;
            string sql = string.Empty;

            sql = "delete from index_of_property_exception " +
                  "WHERE `District_Code` = '" + pDd.District_code + "' AND '" + pDd.RO_code + "' AND `Book` = '" + pDd.Book + "' AND `Deed_year` = '" + pDd.Deed_year + "' AND `Deed_no` = '" + pDd.Deed_no + "'";
            OdbcCommand cmd = new OdbcCommand(sql, conn, txn);
            if (cmd.ExecuteNonQuery() >= 0)
            {
                retVal = true;
            }
            return retVal;
        }
        public bool _DeleteDeed(DeedControl pDd)
        {
            bool retVal = false;
            string sql = string.Empty;

            sql = "delete from deed_details " +
                  "WHERE `District_Code` = '" + pDd.District_code + "' AND '" + pDd.RO_code + "' AND `Book` = '" + pDd.Book + "' AND `Deed_year` = '" + pDd.Deed_year + "' AND `Deed_no` = '" + pDd.Deed_no + "'";
            OdbcCommand cmd = new OdbcCommand(sql, conn, txn);
            if (cmd.ExecuteNonQuery() >= 0)
            {
                retVal = true;
            }
            return retVal;
        }

        public bool _DeleteIndex2(DeedControl pDd)
        {
            bool retVal = false;
            string sql = string.Empty;

            sql = "delete from index_of_property " +
                  "WHERE `District_Code` = '" + pDd.District_code + "' AND '" + pDd.RO_code  + "' AND `Book` = '" + pDd.Book + "' AND `Deed_year` = '" + pDd.Deed_year + "' AND `Deed_no` = '" + pDd.Deed_no + "'";
            OdbcCommand cmd = new OdbcCommand(sql, conn, txn);
            if (cmd.ExecuteNonQuery() >= 0)
            {
                retVal = true;
            }
            return retVal;
        }
        public bool _DeleteIndex2WB(DeedControl pDd)
        {
            bool retVal = false;
            string sql = string.Empty;

            sql = "delete from index_of_property_out_wb " +
                  "WHERE `District_Code` = '" + pDd.District_code + "' AND '" + pDd.RO_code + "' AND `Book` = '" + pDd.Book + "' AND `Deed_year` = '" + pDd.Deed_year + "' AND `Deed_no` = '" + pDd.Deed_no + "'";
            OdbcCommand cmd = new OdbcCommand(sql, conn, txn);
            if (cmd.ExecuteNonQuery() >= 0)
            {
                retVal = true;
            }
            return retVal;
        }
        public bool _DeleteIndex2WBList(DeedControl pDd)
        {
            bool retVal = false;
            string sql = string.Empty;

            sql = "delete from tbloutsidewblist " +
                  "WHERE `District_Code` = '" + pDd.District_code + "' AND '" + pDd.RO_code + "' AND `Book` = '" + pDd.Book + "' AND `Deed_year` = '" + pDd.Deed_year + "' AND `Deed_no` = '" + pDd.Deed_no + "'";
            OdbcCommand cmd = new OdbcCommand(sql, conn, txn);
            if (cmd.ExecuteNonQuery() >= 0)
            {
                retVal = true;
            }
            return retVal;
        }
        public bool _DeleteOtherPlot(DeedControl pDd)
        {
            bool retVal = false;
            string sql = string.Empty;

            sql = "delete from tblother_plots " +
                  "WHERE `District_Code` = '" + pDd.District_code + "' AND '" + pDd.RO_code + "' AND `Book` = '" + pDd.Book + "' AND `Deed_year` = '" + pDd.Deed_year + "' AND `Deed_no` = '" + pDd.Deed_no + "'";
            OdbcCommand cmd = new OdbcCommand(sql, conn, txn);
            if (cmd.ExecuteNonQuery() >= 0)
            {
                retVal = true;
            }
            return retVal;
        }

        public bool _DeleteOtherKhatian(DeedControl pDd)
        {
            bool retVal = false;
            string sql = string.Empty;

            sql = "delete from tbl_other_khatian " +
                  "WHERE `District_Code` = '" + pDd.District_code + "' AND '" + pDd.RO_code + "' AND `Book` = '" + pDd.Book + "' AND `Deed_year` = '" + pDd.Deed_year + "' AND `Deed_no` = '" + pDd.Deed_no + "'";
            OdbcCommand cmd = new OdbcCommand(sql, conn, txn);
            if (cmd.ExecuteNonQuery() >= 0)
            {
                retVal = true;
            }
            return retVal;
        }

#endregion
        #region v2.insert
        public bool _InsertDeed(Deed pDd)
        {
            bool retVal = false;
            string sql = string.Empty;

            sql = "insert into deed_details(`District_Code`,`RO_Code`,`Book`,`Deed_year`,`Deed_no`,`Serial_No`,`Serial_Year`,`tran_maj_code`,`tran_min_code`,`Volume_No`,`Page_From`,`Page_To`,`Deed_Remarks`,`Exported`,`Scan_doc_type`,`addl_pages`,`hold`,`hold_reason`,`status`,`created_system`,`version`,Created_DTTM,created_by,`Date_of_Completion`,`Date_of_Delivery`)   " +
                    "values ('" + pDd.DeedHeader.Deed_control.District_code + "', " +
                    "'" + pDd.DeedHeader.Deed_control.RO_code + "', " +
                    "'" + pDd.DeedHeader.Deed_control.Book + "', " +
                    "'" + pDd.DeedHeader.Deed_control.Deed_year + "', " +
                    "'" + pDd.DeedHeader.Deed_control.Deed_no + "', " +
                    "'" + pDd.DeedHeader.Deed_control.Deed_no + "', " +                   //pDd.DeedHeader.Serial_no
                    "'" + pDd.DeedHeader.Deed_control.Deed_year + "', " +                 //pDd.DeedHeader.Serial_year
                    "'" + pDd.DeedHeader.tran_maj_code + "', " +
                    "'" + pDd.DeedHeader.tran_min_code + "', " +
                    "'" + pDd.DeedHeader.volume_no + "', " +
                    "'" + pDd.DeedHeader.page_from + "', " +
                    "'" + pDd.DeedHeader.page_to + "', " +
                    "'" + pDd.DeedHeader.deed_remarks + "', " +
                    "'" + pDd.DeedHeader.Exported + "', " +
                    "'" + pDd.DeedHeader.scan_doc_type + "', " +
                    "'" + pDd.DeedHeader.addl_pages + "', " +
                    "'" + pDd.DeedHeader.hold + "', " +
                    "'" + pDd.DeedHeader.hold_reason + "', " +
                    "'" + pDd.DeedHeader.status + "', " +
                    "'" + pDd.DeedHeader.created_system + "', " +
                    "'" + pDd.DeedHeader.version + "'," +
                    "'" + crd.created_dttm + "'," +
                    "'" + crd.created_by + "',";
            if (pDd.DeedHeader.date_of_completion != null)
            {
                sql = sql + "date_format('" + pDd.DeedHeader.date_of_completion + "','%Y/%m/%d'), ";
            }
            else
            {
                sql = sql + " null,";
            }
            if (pDd.DeedHeader.date_of_delivery != null)
            {
                sql = sql + "date_format('" + pDd.DeedHeader.date_of_delivery + "','%Y/%m/%d'))";
            }
            else
            {
                sql = sql + "null)";
            }
            

            System.Diagnostics.Debug.Print(sql);
            OdbcCommand cmd = new OdbcCommand(sql, conn, txn);
            if (cmd.ExecuteNonQuery() > 0)
            {
                retVal = true;
            }
            return retVal;
        }
        public bool _InsertDeedException(List<deedDetailsException> pdd)
        {
            bool retVal = false;
            string sql = string.Empty;
            int item_no = 1;
            foreach (deedDetailsException pDd in pdd)
            {

                sql = "insert into deed_details_exception(District_Code,RO_Code,Book,Deed_year,Deed_no,srl_no,exception,details)" +
                    "values('" + pDd.district_code + "', " +
                        "'" + pDd.RO_code + "', " +
                        "'" + pDd.Book + "', " +
                        "'" + pDd.Deed_year + "', " +
                        "'" + pDd.Deed_no + "', " +
                        "'" + item_no + "','" + pDd.exception + "','"+pDd.excDetails+"')";

                System.Diagnostics.Debug.Print(sql);
                OdbcCommand cmd = new OdbcCommand(sql, conn, txn);
                if (cmd.ExecuteNonQuery() > 0)
                {
                    retVal = true;
                    item_no = item_no + 1;
                }
                else
                {
                    retVal = false;
                }
            }
            return retVal;
        }
        public bool _InsertPersonException(List<PersonDetailsException> pdd)
        {
            bool retVal = false;
            string sql = string.Empty;
            int item_no = 1;
            foreach (PersonDetailsException pDd in pdd)
            {

                sql = "insert into index_of_name_exception(District_Code,RO_Code,Book,Deed_year,Deed_no,item_no,srl_no,exception,details)" +
                    "values('" + pDd.district_code + "', " +
                        "'" + pDd.RO_code + "', " +
                        "'" + pDd.Book + "', " +
                        "'" + pDd.Deed_year + "', " +
                        "'" + pDd.Deed_no + "', '"+pDd.item_no+"'," +
                        "'" + item_no + "','" + pDd.exception + "','"+pDd.excDetails+"')";

                System.Diagnostics.Debug.Print(sql);
                OdbcCommand cmd = new OdbcCommand(sql, conn, txn);
                if (cmd.ExecuteNonQuery() > 0)
                {
                    retVal = true;
                    item_no = item_no + 1;
                }
                else
                {
                    retVal = false;
                }
            }
            return retVal;
        }
        public bool _InsertPropertyException(List<PropertyDetailsException> pdd)
        {
            bool retVal = false;
            string sql = string.Empty;
            int item_no = 1;
            foreach (PropertyDetailsException pDd in pdd)
            {

                sql = "insert into index_of_property_exception(District_Code,RO_Code,Book,Deed_year,Deed_no,item_no,srl_no,exception,details)" +
                    "values('" + pDd.district_code + "', " +
                        "'" + pDd.RO_code + "', " +
                        "'" + pDd.Book + "', " +
                        "'" + pDd.Deed_year + "', " +
                        "'" + pDd.Deed_no + "', '" + pDd.item_no + "'," +
                        "'" + item_no + "','" + pDd.exception + "','"+pDd.excDetails+"')";

                System.Diagnostics.Debug.Print(sql);
                OdbcCommand cmd = new OdbcCommand(sql, conn, txn);
                if (cmd.ExecuteNonQuery() > 0)
                {
                    retVal = true;
                    item_no = item_no + 1;
                }
                else
                {
                    retVal = false;
                }
            }
            return retVal;
        }
        public bool _InsertPersonDetails(List<PersonDetails> pdd)
        {
            bool retVal = false;
            string sql = string.Empty;
            int item_no = 1;
            foreach (PersonDetails pDd in pdd)
            {

                sql = "insert into index_of_name(District_Code,RO_Code,Book,Deed_year,Deed_no,Item_no,initial_name,First_name,Last_name,Party_code,Address,Address_district_code,Address_district_name,Address_ps_code,Address_ps_name,Father_mother,Rel_code,Relation,occupation_code,religion_code,more,pin,city,other_party_code,linked_to,Created_DTTM,Created_by)" +
                    "values('" + pDd.district_code + "', " +
                        "'" + pDd.RO_code + "', " +
                        "'" + pDd.Book + "', " +
                        "'" + pDd.Deed_year + "', " +
                        "'" + pDd.Deed_no + "', " +
                        "'" + item_no + "','" + pDd.initial_name + "','" + pDd.First_name + "', " +
                        "'" + pDd.Last_Name + "','" + pDd.Status_code + "','" + pDd.Address + "', " +
                        "'" + pDd.Address_district_code + "','" + pDd.Address_district_name + "','" + pDd.Address_ps_code + "','" + pDd.Address_ps_Name + "', " +
                        "'" + pDd.Father_mother + "','" + pDd.Rel_code + "','" + pDd.Relation + "','"+pDd.Proffession +"','"+pDd.Cast +"','" + pDd.more + "','" + pDd.PIN + "','" + pDd.City_Name + "', " +
                        "'" + pDd.other_party_code + "','" + pDd.linked_to + "','"+crd.created_dttm +"','"+crd.created_by +"')";

                System.Diagnostics.Debug.Print(sql);
                OdbcCommand cmd = new OdbcCommand(sql, conn, txn);
                if (cmd.ExecuteNonQuery() > 0)
                {
                    retVal = true;
                    item_no = item_no + 1;
                }
                else
                {
                    retVal = false;
                }
            }
            return retVal;
        }
        public bool _InsertPropertyDetails(List<PropertyDetails> pdd)
        {
            bool retVal = false;
            string sql = string.Empty;
            int item_no = 1;
            foreach (PropertyDetails pDd in pdd)
            {
                sql = "insert into index_of_property(District_Code,RO_Code,Book,Deed_year,Deed_no,Item_no,Property_district_code,Property_ro_code,ps_code,moucode,Area_type,GP_Muni_Corp_Code,Ward,Holding,Premises,road_code,Plot_code_type,Road,Plot_No,Bata_No,Khatian_type,khatian_No,bata_khatian_no,property_type,Land_Area_acre,Land_Area_bigha,Land_Area_decimal,Land_Area_katha,Land_Area_chatak,Land_Area_sqfeet,Structure_area_in_sqFeet,ref_ps,ref_mouza,JL_no,Other_plots,Other_Khatian,land_type,RefJL_no,Created_DTTM,Created_by)" +
                    "values('" + pDd.district_code + "', " +
                        "'" + pDd.RO_code + "', " +
                        "'" + pDd.Book + "', " +
                        "'" + pDd.Deed_year + "', " +
                        "'" + pDd.Deed_no + "', " +
                        "'" + pDd.Serial + "','" + pDd.Property_district_code + "','" + pDd.Property_ro_code + "', " +
                        "'" + pDd.ps_code + "','" + pDd.moucode + "','" + pDd.Area_type + "', " +
                        "'" + pDd.GP_Muni_Corp_Code + "','" + pDd.Ward + "','" + pDd.Holding + "','" + pDd.Premises + "', " +
                        "'" + pDd.road_code + "','" + pDd.Plot_code_type + "','" + pDd.Road + "','" + pDd.Plot_No + "','" + pDd.Bata_No + "','" + pDd.Khatian_type + "', " +
                        "'" + pDd.khatian_No + "','" + pDd.bata_khatian_no + "','" + pDd.property_type + "','" + pDd.Land_Area_acre + "','" + pDd.Land_Area_bigha + "','" + pDd.Land_Area_decimal + "', " +
                "'" + pDd.Land_Area_katha + "','" + pDd.Land_Area_chatak + "','" + pDd.Land_Area_sqfeet + "','" + pDd.Structure_area_in_sqFeet + "','" + pDd.Ref_ps + "','" + pDd.Ref_mou + "','" + pDd.JL_NO + "', "+
                "'" + pDd.other_plots + "','" + pDd.other_Khatian + "','" + pDd.land_type + "','" + pDd.Ref_JL_Number + "','" + crd.created_dttm + "','" + crd.created_by + "')";

                System.Diagnostics.Debug.Print(sql);
                OdbcCommand cmd = new OdbcCommand(sql, conn, txn);
               
                    if (cmd.ExecuteNonQuery() > 0)
                    {
                        retVal = true;
                        item_no = item_no + 1;
                    }
                    else
                    {
                        retVal = false;
                    }
               
                
            }
            return retVal;
        }
        public bool _InsertPropertyDetailsWB(List<PropertyDetailsWB> pdd)
        {
            bool retVal = false;
            string sql = string.Empty;
            int item_no = 1;
            foreach (PropertyDetailsWB pDd in pdd)
            {
                sql = "insert into index_of_property_out_wb(District_Code,RO_Code,Book,Deed_year,Deed_no,Item_no,Property_country_code,Property_state_code,Property_district_code,thana,moucode,plot_code_type,plot_no,khatian_type,khatian_no,land_use,property_type,local_body_type,other_details,Created_DTTM,Created_by,area_bigha,area_decimal,area_katha,area_chatak,area_sqf,total_area_decimal,struct_sqfeet,area_acre)" +
                    "values('" + pDd.district_code + "', " +
                        "'" + pDd.RO_code + "', " +
                        "'" + pDd.Book + "', " +
                        "'" + pDd.Deed_year + "', " +
                        "'" + pDd.Deed_no + "', " +
                        "'" + item_no + "','" + pDd.Property_country_code + "','" + pDd.Property_state_code + "','" + pDd.Property_district_code + "', " +
                        "'" + pDd.thana + "','" + pDd.mouza + "','" + pDd.Plot_code_type + "', " +
                        "'" + pDd.Plot_No + "','" + pDd.Khatian_type + "','" + pDd.khatian_No + "','"+pDd.land_use+"','" + pDd.property_type + "', " +
                        "'" + pDd.local_body_type + "','" + pDd.other_details + "', " +
                        "'" + crd.created_dttm + "','" + crd.created_by + "','" + pDd.Area_Bigha + "','" + pDd.Area_Decimal + "','" + pDd.Area_Katha + "','" + pDd.Area_Chatak + "','" + pDd.Area_SqtL + "','" + pDd.Total_decimal + "','" + pDd.structure_sqt + "','" + pDd.Area_acre + "')";

                System.Diagnostics.Debug.Print(sql);
                OdbcCommand cmd = new OdbcCommand(sql, conn, txn);
                if (cmd.ExecuteNonQuery() > 0)
                {
                    retVal = true;
                    item_no = item_no + 1;
                }
                else
                {
                    retVal = false;
                }
            }
            return retVal;
        }
        public bool _InsertOtherPlots(List<PropertyDetails_other_plot> pdd)
        {
            bool retVal = false;
            string sql = string.Empty;

            foreach (PropertyDetails_other_plot pDd in pdd)
            {
                sql = "insert into tblother_plots(District_Code,RO_Code,Book,Deed_year,Deed_no,Item_no,other_plots)" +
                    "values('" + pDd.district_code + "', " +
                        "'" + pDd.RO_code + "', " +
                        "'" + pDd.Book + "', " +
                        "'" + pDd.Deed_year + "', " +
                        "'" + pDd.Deed_no + "', " +
                        "'" + pDd.item_no + "','" + pDd.other_plot_no + "')";

                System.Diagnostics.Debug.Print(sql);
                OdbcCommand cmd = new OdbcCommand(sql, conn, txn);
                if (cmd.ExecuteNonQuery() > 0)
                {
                    retVal = true;
                }
                else
                {
                    retVal = false;
                }
            }
            return retVal;
        }
        public bool _InsertWBList(List<outSideWBList > pdd)
        {
            bool retVal = false;
            string sql = string.Empty;
            int item_no = 1;
            foreach (outSideWBList  pDd in pdd)
            {
                sql = "insert into tbloutsidewblist(District_Code,RO_Code,Book,Deed_year,Deed_no,Item_no,isOutsideWB)" +
                    "values('" + pDd.district_code + "', " +
                        "'" + pDd.RO_code + "', " +
                        "'" + pDd.Book + "', " +
                        "'" + pDd.Deed_year + "', " +
                        "'" + pDd.Deed_no + "', " +
                        "'" + item_no + "','" + pDd.exception + "')";

                System.Diagnostics.Debug.Print(sql);
                OdbcCommand cmd = new OdbcCommand(sql, conn, txn);
                if (cmd.ExecuteNonQuery() > 0)
                {
                    retVal = true;
                    item_no = item_no + 1;
                }
                else
                {
                    retVal = false;
                }
            }
            return retVal;
        }
        public bool _InsertOtherkhatian(List<PropertyDetails_other_khatian> pdd)
        {
            bool retVal = false;
            string sql = string.Empty;

            foreach (PropertyDetails_other_khatian pDd in pdd)
            {
                sql = "insert into tbl_other_khatian(District_Code,RO_Code,Book,Deed_year,Deed_no,Item_no,other_khatian)" +
                    "values('" + pDd.district_code + "', " +
                        "'" + pDd.RO_code + "', " +
                        "'" + pDd.Book + "', " +
                        "'" + pDd.Deed_year + "', " +
                        "'" + pDd.Deed_no + "', " +
                        "'" + pDd.item_no + "','" + pDd.other_Khatian_no + "')";

                System.Diagnostics.Debug.Print(sql);
                OdbcCommand cmd = new OdbcCommand(sql, conn, txn);
                if (cmd.ExecuteNonQuery() > 0)
                {
                    retVal = true;
                }
                else
                {
                    retVal = false;
                }
            }
            return retVal;
        }
        #endregion
#endregion


        #region v1
        /*
        public DataSet GetDistrict()
        {
            string sql = "Select distinct district_code,trim(district_name) as district_name from district";
            DataSet ds = new DataSet();
            OdbcDataAdapter odap = new OdbcDataAdapter(sql, conn);
            odap.Fill(ds);
            return ds;
        }
        public DataSet GetMappingSugg(string discode,string ps,string mouza,string jlno)
        {
            string sql = "select b.district_name,b.ps_name,a.eng_mouname,a.jlno,b.district_code,b.ps_code,a.moucode  from moucode a, (select c.district_code, c.ps_code, d.district_name,c.ps_name from ps c, district d where c.district_code=d.district_code and d.district_code in (select district_code from district e where e.district_name like '%"+discode+"%' and e.active='N') and c.ps_name like '%"+ps+"%') b where (a.district_code=b.district_code) and (a.ps_code=b.ps_code) and a.eng_mouname like '%"+mouza+"%'and a.jlno like '%"+jlno+"%'";
            DataSet ds = new DataSet();
            OdbcDataAdapter odap = new OdbcDataAdapter(sql, conn);
            odap.Fill(ds);
            return ds;
        }

        public DataSet GetotherPlot(string docode,string rocode,string book,string deed_year,string deed_no)
        {
            string sql = "Select other_plots from tblother_plots where district_code = '"+docode+"'and ro_code = '"+rocode+"'and book = '"+book+"'and deed_year = '"+deed_year+"'and deed_no = '"+deed_no+"' order by item_no";
            DataSet ds = new DataSet();
            OdbcDataAdapter odap = new OdbcDataAdapter(sql, conn);
            odap.Fill(ds);
            return ds;
        }
        public DataSet GetotherKhatian(string docode, string rocode, string book, string deed_year, string deed_no)
        {
            string sql = "Select other_khatian from tbl_other_khatian where district_code = '" + docode + "'and ro_code = '" + rocode + "'and book = '" + book + "'and deed_year = '" + deed_year + "'and deed_no = '" + deed_no + "' order by item_no";
            DataSet ds = new DataSet();
            OdbcDataAdapter odap = new OdbcDataAdapter(sql, conn);
            odap.Fill(ds);
            return ds;
        }
        public DataSet Getland_type(string do_code,string ro_code)
        {
            string sql = "select land_use,trim(eng_desc) as eng_desc from tbl_land_type where do_code = '" + do_code + "' and ro_code = '" + ro_code + "'";
            DataSet ds = new DataSet();
            OdbcDataAdapter odap = new OdbcDataAdapter(sql, conn);
            odap.Fill(ds);
            return ds;
        }
        public DataSet GetOther_PartyCode()
        {
            string sql = "select trim(ec_name) as ec_name,ec_code from party_code where DependentOn = 'Y'";
            DataSet ds = new DataSet();
            OdbcDataAdapter odap = new OdbcDataAdapter(sql, conn);
            odap.Fill(ds);
            return ds;
        }
        public bool Party_dependency_Check(string party_code)
        {
            string sql = "select HasDependency from party_code where ec_code = '"+party_code+"'";
            DataSet ds = new DataSet();
            OdbcDataAdapter odap = new OdbcDataAdapter(sql, conn);
            odap.Fill(ds);
            if(ds.Tables[0].Rows[0][0].ToString() == "Y")
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        
        public DataSet GetIndex1Count()
        {
            string sql = "select sysValues from sysconfig where syskeys = 'DATA_ENTRY_COUNT_INDEX1'";
            DataSet ds = new DataSet();
            OdbcDataAdapter odap = new OdbcDataAdapter(sql, conn);
            odap.Fill(ds);
            return ds;
        }
        public DataSet GetDuplicateEntry(string pDist,string pRo,string pBook,string pDeed_yr,string pDeed_no)
        {
            string sql = "select count(*) from deed_details where District_Code = '"+pDist+"' and ro_code = '"+pRo+"' and book = '"+pBook+"' and deed_year = '"+pDeed_yr+"' and deed_no = '"+pDeed_no+"'";
            DataSet ds = new DataSet();
            OdbcDataAdapter odap = new OdbcDataAdapter(sql, conn);
            odap.Fill(ds);
            return ds;
        }
        public DataSet GetIndex1MinCount()
        {
            string sql = "select sysValues from sysconfig where syskeys = 'DATA_ENTRY_MIN_COUNT_INDEX1'";
            DataSet ds = new DataSet();
            OdbcDataAdapter odap = new OdbcDataAdapter(sql, conn);
            odap.Fill(ds);
            return ds;
        }
        public DataSet GetIndex1MaxCount()
        {
            string sql = "select sysValues from sysconfig where syskeys = 'DATA_ENTRY_MAX_COUNT_INDEX1'";
            DataSet ds = new DataSet();
            OdbcDataAdapter odap = new OdbcDataAdapter(sql, conn);
            odap.Fill(ds);
            return ds;
        }
        public DataSet GetIndex2Count()
        {
            string sql = "select sysValues from sysconfig where syskeys = 'DATA_ENTRY_COUNT_INDEX2'";
            DataSet ds = new DataSet();
            OdbcDataAdapter odap = new OdbcDataAdapter(sql, conn);
            odap.Fill(ds);
            return ds;
        }
        public DataSet GetDistrict_Active()
        {
            string sql = "Select distinct district_code,district_name from district where active = 'Y'";
            DataSet ds = new DataSet();
            OdbcDataAdapter odap = new OdbcDataAdapter(sql, conn);
            odap.Fill(ds);
            return ds;
        }
        public DataSet GetDocType()
        {
            string sql = "Select distinct doc_type,doc_name from tbldoc_type";
            DataSet ds = new DataSet();
            OdbcDataAdapter odap = new OdbcDataAdapter(sql, conn);
            odap.Fill(ds);
            return ds;
        }
        public DataSet Suggesion(string dist,string ps,string mou)
        {
            string sql = "select a.jlno, b.ps_code, b.ps_name, a.moucode,a.eng_mouname from moucode a, ps b where b.district_code = '"+dist+"' and a.ps_code=b.ps_code and b.ps_name like '%"+ps+"%' and a.eng_mouname like '%"+mou+"%'";
            DataSet ds = new DataSet();
            OdbcDataAdapter odap = new OdbcDataAdapter(sql, conn);
            odap.Fill(ds);
            return ds;
        }
        public DataSet GetBookType()
        {
            string sql = "select key_book,value_book from tbl_book order by value_book";
            DataSet ds = new DataSet();
            OdbcDataAdapter odap = new OdbcDataAdapter(sql, conn);
            odap.Fill(ds);
            return ds;
        }

        public DataSet GetallMouza()
        {
            string sql = "Select moucode,eng_mouname from moucode order by eng_mouname";
            DataSet ds = new DataSet();
            OdbcDataAdapter odap = new OdbcDataAdapter(sql, conn);
            odap.Fill(ds);
            return ds;
        }
        public DataSet GetPSbyMouza(string mouname,string moucode,string discode)
        {
            string sql = "select ps_code from moucode where eng_mouname = '"+mouname+"' and moucode = '"+moucode+"' and District_Code = '"+discode+"'";
            DataSet ds = new DataSet();
            OdbcDataAdapter odap = new OdbcDataAdapter(sql, conn);
            odap.Fill(ds);
            return ds;
        }
        public DataSet GetPSbyDistrict(string pDistcode)
        {
            string sql = "select ps_code, ps_name from ps where district_code = '" + pDistcode + "'";
            DataSet ds = new DataSet();
            OdbcDataAdapter odap = new OdbcDataAdapter(sql, conn);
            odap.Fill(ds);
            return ds;
        }
        public DataSet GetMouCodebyDistrictPS(string pDistcode, string pPSCode)
        {
            string sql = "select moucode, eng_mouname from moucode where district_code = '" + pDistcode + "' and ps_code = '" + pPSCode + "'";
            DataSet ds = new DataSet();
            OdbcDataAdapter odap = new OdbcDataAdapter(sql, conn);
            odap.Fill(ds);
            return ds;
        }
        public DataSet GetJLNobyDistrictPS(string pDistcode, string pPSCode)
        {
            string sql = "select jlno from moucode where district_code = '" + pDistcode + "' and ps_code = '" + pPSCode + "'";
            DataSet ds = new DataSet();
            OdbcDataAdapter odap = new OdbcDataAdapter(sql, conn);
            odap.Fill(ds);
            return ds;
        }
        public DataSet GetUser()
        {
            string sql = "Select distinct user_id,user_name from ac_user order by user_name";
            DataSet ds = new DataSet();
            OdbcDataAdapter odap = new OdbcDataAdapter(sql, conn);
            odap.Fill(ds);
            return ds;
        }
        public DataSet GetYear()
        {
            string sql = "Select distinct deed_year from deed_details";
            DataSet ds = new DataSet();
            OdbcDataAdapter odap = new OdbcDataAdapter(sql, conn);
            odap.Fill(ds);
            return ds;
        }
        public DataSet GetVol(string dis,string ro,string book,string deedyear)
        {
            string sql = "select distinct Volume_No from deed_details where District_Code = '" + dis + "' and RO_Code = '" + ro + "' and Book = '" + book + "' and Deed_year = '" + deedyear + "' order by Convert(volume_no,UNSIGNED INTEGER)";
            DataSet ds = new DataSet();
            OdbcDataAdapter odap = new OdbcDataAdapter(sql, conn);
            odap.Fill(ds);
            return ds;
        }
        public DataSet GetDeedDetails(string district, string ro_code, string book, string year)
        {
            string sql = "Select District_Code,RO_Code,Book,Deed_year,Deed_no,Serial_No,Serial_Year,tran_maj_code,tran_min_code,Volume_No,Page_From,Page_To,date_format(Date_of_Completion,'%d/%m/%Y'),date_format(Date_of_Delivery,'%d/%m/%Y'),Deed_Remarks,Created_DTTM,Exported,created_by,modified_by,modified_dttm,MisMatch,Scan_doc_type from deed_details where district_code ='" + district + "' and ro_code = '" + ro_code + "'and book = '" + book + "'and deed_year = '" + year + "' and exported <> 'Y'";
            DataSet ds = new DataSet();
            OdbcDataAdapter odap = new OdbcDataAdapter(sql, conn);
            odap.Fill(ds);
            return ds;
        }
        public DataSet GetOtherPlots(DataSet ds1)
        {
            DataSet dsplot = new DataSet();
            for (int i = 0; i < ds1.Tables[0].Rows.Count; i++)
            {
                string sql = "select * from tblother_plots where district_code ='" + ds1.Tables[0].Rows[i][0] + "' and ro_code = '" + ds1.Tables[0].Rows[i][1] + "'and book = '" + ds1.Tables[0].Rows[i][2] + "'and deed_year = '" + ds1.Tables[0].Rows[i][3] + "' and vol = '" + ds1.Tables[0].Rows[i][9] + "' order by CONVERT(item_no,unsigned integer) ";
                OdbcDataAdapter odap = new OdbcDataAdapter(sql, conn);
                odap.Fill(dsplot);
            }
            return dsplot;
        }
        public DataSet GetOtherKhatian(DataSet ds1)
        {
            DataSet dskhatian = new DataSet();
            for (int i = 0; i < ds1.Tables[0].Rows.Count; i++)
            {
                string sql = "select * from tbl_other_khatian where district_code ='" + ds1.Tables[0].Rows[i][0] + "' and ro_code = '" + ds1.Tables[0].Rows[i][1] + "'and book = '" + ds1.Tables[0].Rows[i][2] + "'and deed_year = '" + ds1.Tables[0].Rows[i][3] + "' and vol = '" + ds1.Tables[0].Rows[i][9] + "' order by CONVERT(item_no,unsigned integer) ";
                OdbcDataAdapter odap = new OdbcDataAdapter(sql, conn);
                odap.Fill(dskhatian);
            }
            return dskhatian;
        }
        public DataSet GetDeedDetails(string district, string ro_code, string book, string year,string vol)
        {
            string sql = "Select District_Code,RO_Code,Book,Deed_year,Deed_no,Serial_No,Serial_Year,tran_maj_code,tran_min_code,Volume_No,Page_From,Page_To,Date_of_Completion,Date_of_Delivery,Deed_Remarks,Created_DTTM,scan_doc_type,addl_pages,hold,hold_reason,status,created_system,version from deed_details where district_code ='" + district + "' and ro_code = '" + ro_code + "'and book = '" + book + "'and deed_year = '" + year + "' and volume_no = '" + vol + "'and exported <> 'Y'";
            DataSet ds = new DataSet();
            OdbcDataAdapter odap = new OdbcDataAdapter(sql, conn);
            odap.Fill(ds);
            return ds;
        }

        public DataTable GetDeeds(string district, string ro_code, string book, string year)
        {
            DataTable dt = new DataTable();
            string sql = "Select * from deed_details where district_code ='" + district + "' and ro_code = '" + ro_code + "'and book = '" + book + "'and deed_year = '" + year + "'and exported <> 'Y'";
            //DataSet ds = new DataSet();
            OdbcDataAdapter odap = new OdbcDataAdapter(sql, conn);
            odap.Fill(dt);
            return dt;
        }
    
        public DataTable GetDeeds(string district, string ro_code, string book, string year,string vol)
        {
            DataTable dt = new DataTable();
            string sql = "Select District_Code,RO_Code,Book,Deed_year,Deed_no,Serial_No,Serial_Year,tran_maj_code,tran_min_code,Volume_No,Page_From,Page_To,Date_of_Completion,Date_of_Delivery,Deed_Remarks,Created_DTTM,MisMatch,scan_doc_type,addl_pages,hold,hold_reason,status,created_system,version from deed_details where district_code ='" + district + "' and ro_code = '" + ro_code + "'and book = '" + book + "'and deed_year = '" + year + "'and volume_no = '" + vol + "'and exported <> 'Y' order by deed_no";
            //DataSet ds = new DataSet();
            OdbcDataAdapter odap = new OdbcDataAdapter(sql, conn);
            odap.Fill(dt);
            return dt;
        }
        public DataTable GetDeedByNumber(string district, string ro_code, string book, string year, string vol,string deedNo)
        {
            DataTable dt = new DataTable();
            string sql = "Select District_Code,RO_Code,Book,Deed_year,Deed_no,Serial_No,Serial_Year,tran_maj_code,tran_min_code,Volume_No,Page_From,Page_To,Date_of_Completion,Date_of_Delivery,Deed_Remarks,Created_DTTM,MisMatch,scan_doc_type from deed_details where district_code ='" + district + "' and ro_code = '" + ro_code + "'and book = '" + book + "'and deed_year = '" + year + "'and volume_no = '" + vol + "'and deed_no = '"+deedNo +"' and exported <> 'Y' order by deed_no";
            //DataSet ds = new DataSet();
            OdbcDataAdapter odap = new OdbcDataAdapter(sql, conn);
            odap.Fill(dt);
            return dt;
        }
        public DataSet Get_Index1_details(DataSet ds1)
        {
            DataSet ds = new DataSet();
            for (int i = 0; i < ds1.Tables[0].Rows.Count;i++ )
            {
                string sql = "Select District_Code,RO_Code,Book,Deed_year,Deed_no,Item_no,initial_name,First_name,Last_name,Party_code,Admit_code,Address,Address_district_code,Address_district_name,Address_ps_code,Address_ps_name,Father_mother,Rel_code,Relation,occupation_code,religion_code,Created_DTTM,more,pin,city,other_party_code,linked_to from index_of_name where district_code ='" + ds1.Tables[0].Rows[i][0] + "' and ro_code = '" + ds1.Tables[0].Rows[i][1] + "'and book = '" + ds1.Tables[0].Rows[i][2] + "'and deed_year = '" + ds1.Tables[0].Rows[i][3] + "' and deed_no = '" + ds1.Tables[0].Rows[i][4] + "'";
                OdbcDataAdapter odap = new OdbcDataAdapter(sql, conn);
                odap.Fill(ds);
            }
            return ds;
        }

        public DataSet Get_Index2_details(DataSet ds1)
        {
            DataSet ds = new DataSet();
            for (int i = 0; i < ds1.Tables[0].Rows.Count; i++)
            {
                string sql = "Select District_Code,RO_Code,Book,Deed_year,Deed_no,Item_no,Property_district_code,Property_ro_code,ps_code,moucode,Area_type,GP_Muni_Corp_Code,Ward,Holding,Premises,road_code,Plot_code_type,Road,Plot_No,Bata_No,Khatian_type,khatian_No,bata_khatian_no,property_type,Land_Area_acre,Land_Area_bigha,Land_Area_decimal,Land_Area_katha,Land_Area_chatak,Land_Area_sqfeet,Structure_area_in_sqFeet,Created_DTTM,ref_ps,ref_mouza,jl_no,Other_plots,Other_Khatian,land_type,Refjl_no from Index_of_Property where district_code ='" + ds1.Tables[0].Rows[i][0] + "' and ro_code = '" + ds1.Tables[0].Rows[i][1] + "'and book = '" + ds1.Tables[0].Rows[i][2] + "'and deed_year = '" + ds1.Tables[0].Rows[i][3] + "' and deed_no = '" + ds1.Tables[0].Rows[i][4] + "'";
                OdbcDataAdapter odap = new OdbcDataAdapter(sql, conn);
                odap.Fill(ds);
            }
            return ds;
        }
        public string GetDistrictByCode(string pCode)
        {
            string name = string.Empty;
            string sql = "Select distinct district_name from district where district_code='" + pCode + "'";
            DataSet ds = new DataSet();
            OdbcDataAdapter odap = new OdbcDataAdapter(sql, conn);
            odap.Fill(ds);
            if (ds.Tables.Count > 0)
            {
                if (ds.Tables[0].Rows.Count > 0)
                { name = ds.Tables[0].Rows[0][0].ToString(); }
            }
            return name;
        }
        public int GetMaxVal()
        {
            int count = 0;
            DataSet ds = new DataSet();
            string sql = "Select max(count) from tbl_Export_Count";
            OdbcDataAdapter odap = new OdbcDataAdapter(sql, conn);
            odap.Fill(ds);
            if (ds.Tables.Count > 0)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    if (ds.Tables[0].Rows[0][0].ToString() != "")
                    {
                        count = Convert.ToInt32(ds.Tables[0].Rows[0][0].ToString());
                    }
                    else
                    {
                        count = count + 1;
                    }
                }
                if (count == 0) { count = count + 1; }
            }
            return count;

        }
        public int checkPageSequence(string pagefrom,string pageto,string dis,string ro,string book,string deedno,string vol)
        {
            int fromPg = Convert.ToInt32(pagefrom);
            int toPg = Convert.ToInt32(pageto);
            int number = 0;
            string sql = "SELECT count(*) from deed_details where ((Page_From >= '" + fromPg + "' and Page_To <='" + toPg + "') or (Page_From <= '" + toPg + "' and Page_To >= '" + fromPg + "')) and District_Code = '" + dis + "' and ro_code = '" + ro + "' and book = '" + book + "' and Deed_year = '" + deedno + "' and volume_no = '" + vol + "'";
            DataSet ds = new DataSet();
            OdbcDataAdapter odap = new OdbcDataAdapter(sql, conn);
            odap.Fill(ds);
            if (ds.Tables.Count > 0)
            {
                if (ds.Tables[0].Rows.Count > 0)
                { 
                    number = Convert.ToInt32(ds.Tables[0].Rows[0][0].ToString()); 
                }
            }
            return number;
        }
        public DataSet checkPageSequencedetails(string pagefrom, string pageto, string dis, string ro, string book, string deedno, string vol,string deed_yr)
        {
            int fromPg = Convert.ToInt32(pagefrom);
            int toPg = Convert.ToInt32(pageto);
            int number = 0;
            string sql = "SELECT * from deed_details where ((Page_From >= '" + fromPg + "' and Page_To <='" + toPg + "') or (Page_From <= '" + toPg + "' and Page_To >= '" + fromPg + "')) and District_Code = '" + dis + "' and ro_code = '" + ro + "' and book = '" + book + "' and Deed_year = '" + deed_yr + "' and volume_no = '" + vol + "'";
            DataSet ds = new DataSet();
            OdbcDataAdapter odap = new OdbcDataAdapter(sql, conn);
            odap.Fill(ds);
            return ds;
        }
        public DataSet GetROffice(string districtCode)
        {
            string sql = "Select RO_code,RO_name from RO_MASTER where district_code='" + districtCode + "'";
            DataSet ds = new DataSet();
            OdbcDataAdapter odap = new OdbcDataAdapter(sql, conn);
            odap.Fill(ds);
            return ds;
        }
        public DataSet GetROffice_Active(string districtCode)
        {
            string sql = "Select RO_code,RO_name from RO_MASTER where district_code='" + districtCode + "' and active = 'Y'";
            DataSet ds = new DataSet();
            OdbcDataAdapter odap = new OdbcDataAdapter(sql, conn);
            odap.Fill(ds);
            return ds;
        }
        public string GetROByCode(string pCode)
        {
            string name = string.Empty;
            string sql = "Select RO_name from RO_MASTER where RO_code='" + pCode + "'";
            DataSet ds = new DataSet();
            OdbcDataAdapter odap = new OdbcDataAdapter(sql, conn);
            odap.Fill(ds);
            if (ds.Tables.Count > 0)
            {
                if (ds.Tables[0].Rows.Count > 0)
                { name = ds.Tables[0].Rows[0][0].ToString(); }
            }
            return name;
        }
        public DataSet GetTransactionTypeMajor(string bookNumber)
        {
            string sql = "Select tran_maj_code,tran_maj_name from party where book_no='" + bookNumber + "'";
            DataSet ds = new DataSet();
            OdbcDataAdapter odap = new OdbcDataAdapter(sql, conn);
            odap.Fill(ds);
            return ds;
        }
        public DataSet GetTransactionTypeMajorCode(string bookNumber)
        {
            string sql = "Select a.tran_maj_code,a.tran_maj_name from party a,tbl_book b where a.book_no = b.key_book and b.value_book ='"+bookNumber+"'";
            DataSet ds = new DataSet();
            OdbcDataAdapter odap = new OdbcDataAdapter(sql, conn);
            odap.Fill(ds);
            return ds;
        }
        public string GetTranMajorByCode(string pCode)
        {
            string name = string.Empty;
            string sql = "Select tran_maj_name from party where tran_maj_code='" + pCode + "'";
            DataSet ds = new DataSet();
            OdbcDataAdapter odap = new OdbcDataAdapter(sql, conn);
            odap.Fill(ds);
            if (ds.Tables.Count > 0)
            {
                if (ds.Tables[0].Rows.Count > 0)
                { name = ds.Tables[0].Rows[0][0].ToString(); }
            }
            return name;
        }
        public DataSet GetTransactionTypeMinor(string tranMajCode)
        {
            string sql = "Select tran_min_code,tran_name from tranlist_code where tran_maj_code='" + tranMajCode + "'";
            DataSet ds = new DataSet();
            OdbcDataAdapter odap = new OdbcDataAdapter(sql, conn);
            odap.Fill(ds);
            return ds;
        }
        public string GetTranMinorByCode(string pMinCode,string pCode)
        {
            string name = string.Empty;
            string sql = "Select tran_name from tranlist_code where tran_maj_code='" + pMinCode + "' and tran_min_code='" + pCode + "'";
            DataSet ds = new DataSet();
            OdbcDataAdapter odap = new OdbcDataAdapter(sql, conn);
            odap.Fill(ds);
            if (ds.Tables.Count > 0)
            {
                if (ds.Tables[0].Rows.Count > 0)
                { name = ds.Tables[0].Rows[0][0].ToString(); }
            }
            return name;
        }
        public string GetProfByCode(string pCode)
        {
            string name = string.Empty;
            string sql = "Select occupation_name from occupation where occupation_code='" + pCode + "'";
            DataSet ds = new DataSet();
            OdbcDataAdapter odap = new OdbcDataAdapter(sql, conn);
            odap.Fill(ds);
            if (ds.Tables.Count > 0)
            {
                if (ds.Tables[0].Rows.Count > 0)
                { name = ds.Tables[0].Rows[0][0].ToString(); }
            }
            return name;
        }

        public string GetReligionByCode(string pCode)
        {
            string name = string.Empty;
            string sql = "Select religion_name from religion where religion_code='" + pCode + "'";
            DataSet ds = new DataSet();
            OdbcDataAdapter odap = new OdbcDataAdapter(sql, conn);
            odap.Fill(ds);
            if (ds.Tables.Count > 0)
            {
                if (ds.Tables[0].Rows.Count > 0)
                { name = ds.Tables[0].Rows[0][0].ToString(); }
            }
            return name;
        }

        public DataSet GetStatus()
        {
            string sql = "Select status_code,status_desc from ex_status";
            DataSet ds = new DataSet();
            OdbcDataAdapter odap = new OdbcDataAdapter(sql, conn);
            odap.Fill(ds);
            return ds;
        }
        public DataSet GetPartyCode()
        {
            string sql = "Select ec_code,ec_name from Party_code order by ec_name";
            DataSet ds = new DataSet();
            OdbcDataAdapter odap = new OdbcDataAdapter(sql, conn);
            odap.Fill(ds);
            return ds;
        }
        public DataSet GetPS(string districtCode)
        {
            string sql = "Select PS_code,trim(PS_name) as PS_name from ps where district_code='" + districtCode + "' order by PS_name";
            DataSet ds = new DataSet();
            OdbcDataAdapter odap = new OdbcDataAdapter(sql, conn);
            odap.Fill(ds);
            return ds;
        }
        public string GetPSByCode(string pDistrictCode, string pCode)
        {
            string name = string.Empty;
            string sql = "Select trim(PS_name) as PS_name from ps where district_code='" + pDistrictCode + "' and ps_code='" + pCode + "'";
            DataSet ds = new DataSet();
            OdbcDataAdapter odap = new OdbcDataAdapter(sql, conn);
            odap.Fill(ds);
            if (ds.Tables.Count > 0)
            {
                if (ds.Tables[0].Rows.Count > 0)
                { name = ds.Tables[0].Rows[0][0].ToString(); }
            }
            return name;
        }
        public DataSet GetRoad(string districtCode, string RoCode)
        {
            string sql = "Select road_code,road_name from road_corporation where district_code='" + districtCode + "' and ro_code='" + RoCode + "' order by road_name";
            DataSet ds = new DataSet();
            OdbcDataAdapter odap = new OdbcDataAdapter(sql, conn);
            odap.Fill(ds);
            return ds;
        }
        public DataSet GetSubdivision(string districtCode)
        {
            string sql = "Select municipality_code,municipality_name from municipality where district_code='" + districtCode + "' order by municipality_name";
            DataSet ds = new DataSet();
            OdbcDataAdapter odap = new OdbcDataAdapter(sql, conn);
            odap.Fill(ds);
            return ds;
        }
        public string GetSubByCode(string pDisCode, string pCode)
        {
            string name = string.Empty;
            string sql = "Select municipality_name from municipality where district_code = '" + pDisCode + "' and municipality_code='" + pCode + "'";
            DataSet ds = new DataSet();
            OdbcDataAdapter odap = new OdbcDataAdapter(sql, conn);
            odap.Fill(ds);
            if (ds.Tables.Count > 0)
            {
                if (ds.Tables[0].Rows.Count > 0)
                { name = ds.Tables[0].Rows[0][0].ToString(); }
            }
            return name;
        }
        public DataSet GetGramPanchayet(string districtCode, string pscode)
        {
            string sql = "Select gp_code,gp_desc from gram_panchayat where district_code='" + districtCode + "' and ps_code='" + pscode + "' order by gp_desc";
            DataSet ds = new DataSet();
            OdbcDataAdapter odap = new OdbcDataAdapter(sql, conn);
            odap.Fill(ds);
            return ds;
        }
        public AutoCompleteStringCollection GetGramPanchayetAutoFill(string districtCode, string pscode)
        {
            DataSet ds = GetGramPanchayet(districtCode, pscode);
            AutoCompleteStringCollection gp = new AutoCompleteStringCollection();
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                gp.Add(ds.Tables[0].Rows[i][1].ToString());
            }
            return gp;
        }
        public string GetGPByCode(string pDistCode, string pPSCode, string pCode)
        {
            string name = string.Empty;
            string sql = "Select gp_desc from gram_panchayat where district_code='" + pDistCode + "' and ps_code='" + pPSCode + "' and gp_code='" + pCode + "'";
            DataSet ds = new DataSet();
            OdbcDataAdapter odap = new OdbcDataAdapter(sql, conn);
            odap.Fill(ds);
            if (ds.Tables.Count > 0)
            {
                if (ds.Tables[0].Rows.Count > 0)
                { name = ds.Tables[0].Rows[0][0].ToString(); }
            }
            return name;
        }
        public DataSet GetMouza(string districtCode, string pPscode)
        {
            string sql = "Select moucode,eng_mouname from moucode where district_code='" + districtCode + "' and ps_code='" + pPscode + "' order by eng_mouname";
            DataSet ds = new DataSet();
            OdbcDataAdapter odap = new OdbcDataAdapter(sql, conn);
            odap.Fill(ds);
            return ds;
        }
        public DataSet GetJL(string districtCode, string pPscode)
        {
            string sql = "Select distinct jlno from moucode where district_code='" + districtCode + "' and ps_code='" + pPscode + "' order by jlno";
            DataSet ds = new DataSet();
            OdbcDataAdapter odap = new OdbcDataAdapter(sql, conn);
            odap.Fill(ds);
            ds.Tables[0].Rows.Add("Other");
            return ds;
        }
        public string GetMouzaByCode(string pDistrictCode, string pPSCode, string mouzaCode)
        {
            string name = string.Empty;
            string sql = "Select eng_mouname from moucode where district_code='" + pDistrictCode + "' and ps_code='" + pPSCode + "' and moucode='" + mouzaCode + "'";
            DataSet ds = new DataSet();
            OdbcDataAdapter odap = new OdbcDataAdapter(sql, conn);
            odap.Fill(ds);
            if (ds.Tables.Count > 0)
            {
                if (ds.Tables[0].Rows.Count > 0)
                { name = ds.Tables[0].Rows[0][0].ToString(); }
            }
            return name;
        }
        public DataSet GetPropertyType()
        {
            string sql = "Select apartment_type_code,description from property_type order by description";
            DataSet ds = new DataSet();
            OdbcDataAdapter odap = new OdbcDataAdapter(sql, conn);
            odap.Fill(ds);
            return ds;
        }
        public string GetPropertyTypeByCode(string pCode)
        {
            string name = string.Empty;
            string sql = "Select description from property_type where apartment_type_code='" + pCode + "'";
            DataSet ds = new DataSet();
            OdbcDataAdapter odap = new OdbcDataAdapter(sql, conn);
            odap.Fill(ds);
            if (ds.Tables.Count > 0)
            {
                if (ds.Tables[0].Rows.Count > 0)
                { name = ds.Tables[0].Rows[0][0].ToString(); }
            }
            return name;
        }
        public DataSet GetOccupation()
        {
            string sql = "Select occupation_code,occupation_name from occupation order by occupation_name";
            DataSet ds = new DataSet();
            OdbcDataAdapter odap = new OdbcDataAdapter(sql, conn);
            odap.Fill(ds);
            return ds;
        }
        public DataSet GetReligion()
        {
            string sql = "Select religion_code,religion_name from religion";
            DataSet ds = new DataSet();
            OdbcDataAdapter odap = new OdbcDataAdapter(sql, conn);
            odap.Fill(ds);
            return ds;
        }
        public static string SaveDeedDetails(OdbcConnection pCon, DeedDetails pDd, System.ComponentModel.BindingList<PersonDetails> pIndex1, System.ComponentModel.BindingList<PropertyDetails> pIndex2, Credentials crd, string mismatch, System.ComponentModel.BindingList<PropertyDetails_other_plot> pIndexplot, System.ComponentModel.BindingList<PropertyDetails_other_khatian> pIndexkhatian)
        {
            bool indxInserted = true;
            string error = string.Empty;
            OdbcTransaction trans = pCon.BeginTransaction();
            try
            {
                OdbcCommand cmd = new OdbcCommand();
                string sql = string.Empty;

                sql = "insert into deed_details(district_code,RO_code,Book,Deed_year,Deed_no,Serial_no,Serial_year,tran_maj_code,tran_min_code,volume_no,page_from,page_to,created_dttm,created_by,MisMatch,Scan_doc_type,Date_of_Completion,Date_of_Delivery,Deed_Remarks,addl_pages,hold,hold_reason,status,created_system,version) values('" + pDd.Deed_control.District_code + "','" + pDd.Deed_control.RO_code + "','" + pDd.Deed_control.Book + "'" +
                       ",'" + pDd.Deed_control.Deed_year + "','" + pDd.Deed_control.Deed_no + "','" + pDd.Deed_control.Deed_no + "','" + pDd.Deed_control.Deed_year + "','" + pDd.tran_maj_code + "'" +
                       ",'" + pDd.tran_min_code + "','" + pDd.volumn_no + "','" + pDd.page_from + "','" + pDd.page_to + "','" + crd.created_dttm + "','" + crd.created_by + "','" + mismatch + "','" + pDd.doc_type + "',";
                if (pDd.date_of_completion != null)
                {
                    sql = sql + "'" + pDd.date_of_completion + "'";
                }
                else { sql = sql + "null"; }
                if (pDd.date_of_delivery != null)
                {
                    sql = sql + " ,'" + pDd.date_of_delivery + "'";
                }
                else { sql = sql + ",null"; }
                sql = sql + " ,'" + pDd.deed_remarks + "','"+pDd.addl_pages+"','"+pDd.hold+"','"+pDd.hold_reason+"','"+pDd.status+"','"+pDd.created_system+"','"+pDd.version+"')";
                cmd.CommandText = sql;
                cmd.Connection = pCon;
                cmd.Transaction = trans;
                if (cmd.ExecuteNonQuery() > 0)
                {
                    int indxCount = 1;
                    int indx2Count = 1;
                    OdbcCommand index1Cmd = new OdbcCommand();
                    index1Cmd.Connection = pCon;
                    index1Cmd.Transaction = trans;
                    OdbcCommand index2Cmd = new OdbcCommand();
                    index2Cmd.Connection = pCon;
                    index2Cmd.Transaction = trans;
                    OdbcCommand index1plotCmd = new OdbcCommand();
                    index1plotCmd.Connection = pCon;
                    index1plotCmd.Transaction = trans;
                    OdbcCommand index1khatianCmd = new OdbcCommand();
                    index1khatianCmd.Connection = pCon;
                    index1khatianCmd.Transaction = trans;
                    foreach (PersonDetails indx in pIndex1)
                    {
                        sql = "insert into index_of_name values('" + pDd.Deed_control.District_code + "','" + pDd.Deed_control.RO_code + "','" + pDd.Deed_control.Book + "'," +
                              "'" + pDd.Deed_control.Deed_year + "','" + pDd.Deed_control.Deed_no + "','" + indxCount++ + "'," +
                              "'" + indx.initial_name + "','" + indx.First_name + "','" + indx.Last_Name + "'," +
                              "'" + indx.Status_code + "','" + indx.Admit_code + "','" + indx.Address + "','" + indx.Address_district_code + "'," +
                              "'" + indx.Address_district_name + "','" + indx.Address_ps_code + "','" + indx.Address_ps_Name + "','" + indx.Father_mother + "'," +
                              "'" + indx.Rel_code + "','" + indx.Relation + "','" + indx.Proffession + "','" + indx.Cast + "','"+crd.created_dttm+"','"+crd.created_by+"','"+indx.more+"','"+indx.PIN+"','"+indx.City_Name+"','"+indx.other_party_code+"','"+indx.linked_to+"')";
                        //sql = sql.Replace('/',' ');
                        index1Cmd.CommandText = sql;

                        if (index1Cmd.ExecuteNonQuery() > 0)
                        { }
                        else { trans.Rollback(); error = "Error while saving the person's details...."; return error; }
                    }
                    foreach (PropertyDetails indx in pIndex2)
                    {
                        sql = "insert into index_of_property values('" + pDd.Deed_control.District_code + "','" + pDd.Deed_control.RO_code + "','" + pDd.Deed_control.Book + "'," +
                              "'" + pDd.Deed_control.Deed_year + "','" + pDd.Deed_control.Deed_no + "','" + indx2Count++ + "','" + indx.Property_district_code + "'," +
                              "'" + indx.Property_ro_code + "','" + indx.ps_code + "','" + indx.moucode + "','" + indx.Area_type + "','" + indx.GP_Muni_Corp_Code + "'," +
                              "'" + indx.Ward + "','" + indx.Holding + "','" + indx.Premises + "','" + indx.road_code + "','" + indx.Plot_code_type + "','" + indx.Road + "'," +
                              indx.Plot_No + "," + indx.Bata_No + ",'" + indx.Khatian_type + "','" + indx.khatian_No + "','" + indx.bata_khatian_no + "'," +
                              "'" + indx.property_type + "'," + indx.Land_Area_acre + "," + indx.Land_Area_bigha + "," + indx.Land_Area_decimal + "," + indx.Land_Area_katha + "," + indx.Land_Area_chatak +
                              "," + indx.Land_Area_sqfeet + "," + indx.Structure_area_in_sqFeet + ",'" + crd.created_dttm + "','" + crd.created_by + "','" + indx.Ref_ps + "','" + indx.Ref_mou + "','"+indx.JL_NO+"','"+indx.other_plots+"','"+indx.other_Khatian+"','"+indx.land_type+"','"+indx.Ref_JL_Number+"')";
                        index2Cmd.CommandText = sql;
                        if (index2Cmd.ExecuteNonQuery() > 0)
                        { }
                        else { trans.Rollback(); error = "Error while saving the property details...."; return error; }
                    }
                    if (pIndexplot.Count > 0)
                    {
                        foreach (PropertyDetails_other_plot indx in pIndexplot)
                        {
                            sql = "insert into tblother_plots values('"+indx.district_code+"','"+indx.RO_code+"','"+indx.Book+"','"+indx.Deed_year+"','"+indx.Deed_no+"','"+indx.item_no +"','"+indx.other_plot_no+"','"+indx.vol+"')";
                            index1plotCmd.CommandText = sql;

                            if (index1plotCmd.ExecuteNonQuery() > 0)
                            { }
                            else { trans.Rollback(); error = "Error while saving the Other Plot Details...."; return error; }
                        }
                        
                    }

                    if (pIndexkhatian.Count > 0)
                    {
                        foreach (PropertyDetails_other_khatian indx in pIndexkhatian)
                        {
                            sql = "insert into tbl_other_khatian values('" + indx.district_code + "','" + indx.RO_code + "','" + indx.Book + "','" + indx.Deed_year + "','" + indx.Deed_no + "','" + indx.item_no + "','" + indx.other_Khatian_no + "','"+indx.vol+"')";
                            index1khatianCmd.CommandText = sql;

                            if (index1khatianCmd.ExecuteNonQuery() > 0)
                            { }
                            else { trans.Rollback(); error = "Error while saving the Other Khatian Details...."; return error; }
                        }

                    }

                    if ((indxCount > 0) && (indx2Count > 0))

                    { 
                        trans.Commit(); error = ""; return error; 
                    }
                    else
                    { trans.Rollback(); error = "No records found to be inserted......"; return error; }
                }
                else { trans.Rollback(); error = "Error while saving the details...."; return error; }

            }
            catch (Exception ex)
            {
                trans.Rollback();
                return error = ex.Message;
            }
        }

        public static string UpdateDeedDetails(OdbcConnection pCon, DeedDetails pDd, System.ComponentModel.BindingList<PersonDetails> pIndex1, System.ComponentModel.BindingList<PropertyDetails> pIndex2, bool pDoubleEntry, Credentials crd, System.ComponentModel.BindingList<PropertyDetails_other_plot> pIndexPlot, System.ComponentModel.BindingList<PropertyDetails_other_khatian> pIndexKhatian)
        {
            bool indxInserted = true;
            bool deleted = false;
            string error = string.Empty;
            OdbcTransaction trans = pCon.BeginTransaction();
            try
            {
                OdbcCommand cmd = new OdbcCommand();
                OdbcCommand delCmd = new OdbcCommand();
                OdbcCommand insertCmd = new OdbcCommand();
                OdbcCommand index1plotCmd = new OdbcCommand();
                index1plotCmd.Connection = pCon;
                index1plotCmd.Transaction = trans;
                OdbcCommand index1khatianCmd = new OdbcCommand();
                index1khatianCmd.Connection = pCon;
                index1khatianCmd.Transaction = trans;
                string sql = string.Empty;

                if (pDoubleEntry)
                {
                    sql = "insert into deed_details_history select * from deed_details where district_code='" + pDd.Deed_control.District_code + "' and RO_code='" + pDd.Deed_control.RO_code+ "' and book='" + pDd.Deed_control.Book + "' and deed_year='" + pDd.Deed_control.Deed_year + "' and deed_no='" + pDd.Deed_control.Deed_no + "'";
                    insertCmd.CommandText = sql;
                    insertCmd.Connection = pCon;
                    insertCmd.Transaction = trans;
                    insertCmd.ExecuteNonQuery();

                    sql = "insert into index_of_name_history select * from index_of_name where district_code='" + pDd.Deed_control.District_code + "' and RO_code='" + pDd.Deed_control.RO_code + "' and book='" + pDd.Deed_control.Book + "' and deed_year='" + pDd.Deed_control.Deed_year + "' and deed_no='" + pDd.Deed_control.Deed_no + "'";
                    insertCmd.CommandText = sql;
                    insertCmd.Connection = pCon;
                    insertCmd.Transaction = trans;
                    insertCmd.ExecuteNonQuery();

                    sql = "insert into index_of_property_history select * from index_of_property where district_code='" + pDd.Deed_control.District_code + "' and RO_code='" + pDd.Deed_control.RO_code + "' and book='" + pDd.Deed_control.Book + "' and deed_year='" + pDd.Deed_control.Deed_year + "' and deed_no='" + pDd.Deed_control.Deed_no + "'";
                    insertCmd.CommandText = sql;
                    insertCmd.Connection = pCon;
                    insertCmd.Transaction = trans;
                    insertCmd.ExecuteNonQuery();
                }


                sql = "Delete from deed_details where district_code='" + pDd.Deed_control.District_code + "' and RO_code='" + pDd.Deed_control.RO_code + "' and book='" + pDd.Deed_control.Book + "' and deed_year='" + pDd.Deed_control.Deed_year + "' and deed_no='" + pDd.Deed_control.Deed_no + "'";
                delCmd.CommandText = sql;
                delCmd.Connection = pCon;
                delCmd.Transaction = trans;
                if (delCmd.ExecuteNonQuery() > 0)
                {
                    sql = "Delete from index_of_name where district_code='" + pDd.Deed_control.District_code + "' and RO_code='" + pDd.Deed_control.RO_code + "' and book='" + pDd.Deed_control.Book + "' and deed_year='" + pDd.Deed_control.Deed_year + "' and deed_no='" + pDd.Deed_control.Deed_no + "'";
                    delCmd.CommandText = sql;
                    if (delCmd.ExecuteNonQuery() > 0)
                    {
                        sql = "Delete from index_of_property where district_code='" + pDd.Deed_control.District_code + "' and RO_code='" + pDd.Deed_control.RO_code + "' and book='" + pDd.Deed_control.Book + "' and deed_year='" + pDd.Deed_control.Deed_year + "' and deed_no='" + pDd.Deed_control.Deed_no + "'";
                        delCmd.CommandText = sql;
                        if (delCmd.ExecuteNonQuery() > 0)
                        {
                            sql = "Delete from tblother_plots where district_code='" + pDd.Deed_control.District_code + "' and RO_code='" + pDd.Deed_control.RO_code + "' and book='" + pDd.Deed_control.Book + "' and deed_year='" + pDd.Deed_control.Deed_year + "' and deed_no='" + pDd.Deed_control.Deed_no + "'";
                            delCmd.CommandText = sql;
                            if (delCmd.ExecuteNonQuery() >= 0)
                            {
                                sql = "Delete from tbl_other_khatian where district_code='" + pDd.Deed_control.District_code + "' and RO_code='" + pDd.Deed_control.RO_code + "' and book='" + pDd.Deed_control.Book + "' and deed_year='" + pDd.Deed_control.Deed_year + "' and deed_no='" + pDd.Deed_control.Deed_no + "'";
                                delCmd.CommandText = sql;
                                delCmd.ExecuteNonQuery();
                                deleted = true;
                            }
                        }
                        
                    }
                    else { trans.Rollback(); error = "Error while deleting the person's details...."; return error; }
                }
                else
                { trans.Rollback(); error = "Error while deleting the deed details...."; return error; }
                if (deleted)
                {
                    if (pDoubleEntry)
                    {
                        sql = "insert into deed_details(district_code,RO_code,Book,Deed_year,Deed_no,Serial_no,Serial_year,tran_maj_code,tran_min_code,volume_no,page_from,page_to,created_dttm) values('" + pDd.Deed_control.District_code + "','" + pDd.Deed_control.RO_code + "','" + pDd.Deed_control.Book + "'" +
                               ",'" + pDd.Deed_control.Deed_year + "','" + pDd.Deed_control.Deed_no + "','" + pDd.Deed_control.Deed_no + "','" + pDd.Deed_control.Deed_year + "','" + pDd.tran_maj_code + "'" +
                               ",'" + pDd.tran_min_code + "','" + pDd.volumn_no + "','" + pDd.page_from + "','" + pDd.page_to + "','" + crd.created_dttm + "','" + crd.created_by + "')";
                    }
                    else
                    {
                    //    sql = "insert into deed_details(district_code,RO_code,Book,Deed_year,Deed_no,Serial_no,Serial_year,tran_maj_code,tran_min_code,volume_no,page_from,page_to,created_dttm,created_by,MisMatch,Scan_doc_type,Date_of_Completion,Date_of_Delivery,Deed_Remarks) values('" + pDd.Deed_control.District_code + "','" + pDd.Deed_control.RO_code + "','" + pDd.Deed_control.Book + "'" +
                    //       ",'" + pDd.Deed_control.Deed_year + "','" + pDd.Deed_control.Deed_no + "','" + pDd.Deed_control.Deed_no + "','" + pDd.Deed_control.Deed_year + "','" + pDd.tran_maj_code + "'" +
                    //       ",'" + pDd.tran_min_code + "','" + pDd.volumn_no + "','" + pDd.page_from + "','" + pDd.page_to + "','"+crd.created_dttm+"','"+crd.created_by+"','"+pDd.scan_doc_type +"','"+pDd.date_of_completion+"','"+pDd.date_of_delivery+"','"+pDd.deed_remarks+"')";
                        sql = "insert into deed_details(district_code,RO_code,Book,Deed_year,Deed_no,Serial_no,Serial_year,tran_maj_code,tran_min_code,volume_no,page_from,page_to,created_dttm,created_by,Scan_doc_type,Date_of_Completion,Date_of_Delivery,Deed_Remarks,addl_pages,hold,hold_reason,status,created_system,version) values('" + pDd.Deed_control.District_code + "','" + pDd.Deed_control.RO_code + "','" + pDd.Deed_control.Book + "'" +
                           ",'" + pDd.Deed_control.Deed_year + "','" + pDd.Deed_control.Deed_no + "','" + pDd.Deed_control.Deed_no + "','" + pDd.Deed_control.Deed_year + "','" + pDd.tran_maj_code + "'" +
                           ",'" + pDd.tran_min_code + "','" + pDd.volumn_no + "','" + pDd.page_from + "','" + pDd.page_to + "','" + crd.created_dttm + "','" + crd.created_by + "','" + pDd.doc_type + "',";
                        if (pDd.date_of_completion != null)
                        {
                            sql = sql + "'" + pDd.date_of_completion + "'";
                        }
                        else { sql = sql + "null"; }
                        if (pDd.date_of_delivery != null)
                        {
                            sql = sql + " ,'" + pDd.date_of_delivery + "'";
                        }
                        else { sql = sql + ",null"; }
                        sql = sql + " ,'" + pDd.deed_remarks + "','" + pDd.addl_pages + "','" + pDd.hold + "','" + pDd.hold_reason + "','" + pDd.status + "','" + pDd.created_system + "','"+pDd.version+"')";
                    }

                    cmd.CommandText = sql;
                    cmd.Connection = pCon;
                    cmd.Transaction = trans;
                    if (cmd.ExecuteNonQuery() > 0)
                    {
                        int indxCount = 1;
                        int indx2Count = 1;
                        OdbcCommand index1Cmd = new OdbcCommand();
                        index1Cmd.Connection = pCon;
                        index1Cmd.Transaction = trans;
                        OdbcCommand index2Cmd = new OdbcCommand();
                        index2Cmd.Connection = pCon;
                        index2Cmd.Transaction = trans;

                        if (pDoubleEntry)
                        {
                            foreach (PersonDetails indx in pIndex1)
                            {
                                if (indx.Selected)
                                {
                                    sql = "insert into index_of_name values('" + pDd.Deed_control.District_code + "','" + pDd.Deed_control.RO_code + "','" + pDd.Deed_control.Book + "'," +
                                          "'" + pDd.Deed_control.Deed_year + "','" + pDd.Deed_control.Deed_no + "','" + indxCount++ + "'," +
                                          "'" + indx.initial_name + "','" + indx.First_name + "','" + indx.Last_Name + "'," +
                                          "'" + indx.Status_code + "','" + indx.Admit_code + "','" + indx.Address + "','" + indx.Address_district_code + "'," +
                                          "'" + indx.Address_district_name + "','" + indx.Address_ps_code + "','" + indx.Address_ps_Name + "','" + indx.Father_mother + "'," +
                                          "'" + indx.Rel_code + "','" + indx.Relation + "','" + indx.Proffession + "','" + indx.Cast + "','" + crd.created_dttm + "','" + crd.created_by + "')";
                                    //sql = sql.Replace('/',' ');
                                    index1Cmd.CommandText = sql;

                                    if (index1Cmd.ExecuteNonQuery() > 0)
                                    { }
                                    else { trans.Rollback(); error = "Error while saving the person's details...."; return error; }
                                }
                            }
                        }
                        else
                        {
                            foreach (PersonDetails indx in pIndex1)
                            {
                                sql = "insert into index_of_name values('" + pDd.Deed_control.District_code + "','" + pDd.Deed_control.RO_code + "','" + pDd.Deed_control.Book + "'," +
                                        "'" + pDd.Deed_control.Deed_year + "','" + pDd.Deed_control.Deed_no + "','" + indxCount++ + "'," +
                                        "'" + indx.initial_name + "','" + indx.First_name + "','" + indx.Last_Name + "'," +
                                        "'" + indx.Status_code + "','" + indx.Admit_code + "','" + indx.Address + "','" + indx.Address_district_code + "'," +
                                        "'" + indx.Address_district_name + "','" + indx.Address_ps_code + "','" + indx.Address_ps_Name + "','" + indx.Father_mother + "'," +
                                        "'" + indx.Rel_code + "','" + indx.Relation + "','" + indx.Proffession + "','" + indx.Cast + "','" + crd.created_dttm + "','" + crd.created_by + "','"+indx.more+"','"+indx.PIN+"','"+indx.City_Name+"','"+indx.other_party_code+"','"+indx.linked_to+"')";
                                //sql = sql.Replace('/',' ');
                                index1Cmd.CommandText = sql;

                                if (index1Cmd.ExecuteNonQuery() > 0)
                                { }
                                else { trans.Rollback(); error = "Error while saving the person's details...."; return error; }
                            }
                        }
                        if (pDoubleEntry)
                        {
                            foreach (PropertyDetails indx in pIndex2)
                            {
                                if (indx.Selected)
                                {
                                    sql = "insert into index_of_property values('" + pDd.Deed_control.District_code + "','" + pDd.Deed_control.RO_code + "','" + pDd.Deed_control.Book + "'," +
                                          "'" + pDd.Deed_control.Deed_year + "','" + pDd.Deed_control.Deed_no + "','" + indx2Count++ + "','" + indx.Property_district_code + "'," +
                                          "'" + indx.Property_ro_code + "','" + indx.ps_code + "','" + indx.moucode + "','" + indx.Area_type + "','" + indx.GP_Muni_Corp_Code + "'," +
                                          "'" + indx.Ward + "','" + indx.Holding + "','" + indx.Premises + "','" + indx.road_code + "','" + indx.Plot_code_type + "','" + indx.Road + "'," +
                                          indx.Plot_No + "," + indx.Bata_No + ",'" + indx.Khatian_type + "','" + indx.khatian_No + "','" + indx.bata_khatian_no + "'," +
                                          "'" + indx.property_type + "'," + indx.Land_Area_acre + "," + indx.Land_Area_bigha + "," + indx.Land_Area_decimal + "," + indx.Land_Area_katha + "," + indx.Land_Area_chatak +
                                          "," + indx.Land_Area_sqfeet + "," + indx.Structure_area_in_sqFeet + ",'" + crd.created_dttm + "','" + crd.created_by + "','" + indx.Ref_ps + "','" + indx.Ref_mou + "')";
                                    index2Cmd.CommandText = sql;
                                    if (index2Cmd.ExecuteNonQuery() > 0)
                                    { }
                                    else { trans.Rollback(); error = "Error while saving the property details...."; return error; }
                                }
                            }
                        }
                        else
                        {
                            foreach (PropertyDetails indx in pIndex2)
                            {
                                sql = "insert into index_of_property values('" + pDd.Deed_control.District_code + "','" + pDd.Deed_control.RO_code + "','" + pDd.Deed_control.Book + "'," +
                                        "'" + pDd.Deed_control.Deed_year + "','" + pDd.Deed_control.Deed_no + "','" + indx2Count++ + "','" + indx.Property_district_code + "'," +
                                        "'" + indx.Property_ro_code + "','" + indx.ps_code + "','" + indx.moucode + "','" + indx.Area_type + "','" + indx.GP_Muni_Corp_Code + "'," +
                                        "'" + indx.Ward + "','" + indx.Holding + "','" + indx.Premises + "','" + indx.road_code + "','" + indx.Plot_code_type + "','" + indx.Road + "'," +
                                        indx.Plot_No + "," + indx.Bata_No + ",'" + indx.Khatian_type + "','" + indx.khatian_No + "','" + indx.bata_khatian_no + "'," +
                                        "'" + indx.property_type + "'," + indx.Land_Area_acre + "," + indx.Land_Area_bigha + "," + indx.Land_Area_decimal + "," + indx.Land_Area_katha + "," + indx.Land_Area_chatak +
                                        "," + indx.Land_Area_sqfeet + "," + indx.Structure_area_in_sqFeet + ",'" + crd.created_dttm + "','" + crd.created_by + "','" + indx.Ref_ps + "','" + indx.Ref_mou + "','"+indx.JL_NO+"','"+indx.other_plots+"','"+indx.other_Khatian+"','"+indx.land_type+"','"+indx.Ref_JL_Number+"')";
                                index2Cmd.CommandText = sql;
                                if (index2Cmd.ExecuteNonQuery() > 0)
                                { }
                                else { trans.Rollback(); error = "Error while saving the property details...."; return error; }
                            }
                        }
                        if (pIndexPlot.Count > 0)
                        {
                            foreach (PropertyDetails_other_plot indx in pIndexPlot)
                            {
                                sql = "insert into tblother_plots values('" + indx.district_code + "','" + indx.RO_code + "','" + indx.Book + "','" + indx.Deed_year + "','" + indx.Deed_no + "','"+indx.item_no+"','" + indx.other_plot_no + "','"+indx.vol+"')";
                                index1plotCmd.CommandText = sql;

                                if (index1plotCmd.ExecuteNonQuery() > 0)
                                { }
                                else { trans.Rollback(); error = "Error while saving the Other Plot Details...."; return error; }
                            }

                        }

                        if (pIndexKhatian.Count > 0)
                        {
                            foreach (PropertyDetails_other_khatian indx in pIndexKhatian)
                            {
                                sql = "insert into tbl_other_khatian values('" + indx.district_code + "','" + indx.RO_code + "','" + indx.Book + "','" + indx.Deed_year + "','" + indx.Deed_no + "','" + indx.item_no + "','" + indx.other_Khatian_no + "','"+indx.vol+"')";
                                index1khatianCmd.CommandText = sql;

                                if (index1khatianCmd.ExecuteNonQuery() > 0)
                                { }
                                else { trans.Rollback(); error = "Error while saving the Other Khatian Details...."; return error; }
                            }

                        }
                        if ((indxCount > 0) || (indx2Count > 0))
                        { trans.Commit(); error = ""; return error; }
                        else
                        { trans.Rollback(); error = "Error while saving deed details...."; return error; }
                    }
                    else { trans.Rollback(); error = "Error while saving the deed details...."; return error; }
                }
                else { trans.Rollback(); error = "Error while deleting the deed details...."; return error; }
            }
            catch (Exception ex)
            {
                trans.Rollback();
                return error = "Error while saving the details... " + ex.Message;
            }
        }


        public DeedDetails GetDeedDetails(DeedDetails pDeed)
        {
            DeedDetails newDeed = new DeedDetails();
            try
            {
                string sql = "SELECT District_Code,RO_Code,Book,Deed_year,Deed_no,Serial_No,Serial_Year,tran_maj_code" +
                            ",tran_min_code,Volume_No,Page_From,Page_To,Date_of_Completion,Date_of_Delivery" +
                            ",Deed_Remarks,scan_doc_type,addl_pages,hold,hold_reason FROM Deed_details where district_code='" + pDeed.Deed_control.District_code + "' and RO_code='" + pDeed.Deed_control.RO_code + "' and book='" + pDeed.Deed_control.Book + "' and deed_year='" + pDeed.Deed_control.Deed_year + "' and deed_no='" + pDeed.Deed_control.Deed_no + "' and volume_no = '" + pDeed.volumn_no + "'";
                DataSet ds = new DataSet();
                OdbcDataAdapter odap = new OdbcDataAdapter(sql, conn);
                odap.Fill(ds);
                if (ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        newDeed.Deed_control.District_code = ds.Tables[0].Rows[0]["district_code"].ToString();
                        newDeed.Deed_control.RO_code = ds.Tables[0].Rows[0]["RO_code"].ToString();
                        newDeed.Deed_control.Book = ds.Tables[0].Rows[0]["Book"].ToString();
                        newDeed.Deed_control.Deed_year = ds.Tables[0].Rows[0]["Deed_year"].ToString();
                        newDeed.Deed_control.Deed_no = ds.Tables[0].Rows[0]["deed_no"].ToString();
                        newDeed.Serial_no = ds.Tables[0].Rows[0]["Serial_No"].ToString();
                        newDeed.tran_maj_code = ds.Tables[0].Rows[0]["tran_maj_code"].ToString();
                        newDeed.tran_min_code = ds.Tables[0].Rows[0]["tran_min_code"].ToString();
                        newDeed.volumn_no = ds.Tables[0].Rows[0]["Volume_No"].ToString();
                        newDeed.page_from = ds.Tables[0].Rows[0]["Page_From"].ToString();
                        newDeed.page_to = ds.Tables[0].Rows[0]["Page_To"].ToString();
                        newDeed.date_of_completion = ds.Tables[0].Rows[0]["Date_of_Completion"].ToString();
                        newDeed.date_of_delivery = ds.Tables[0].Rows[0]["Date_of_Delivery"].ToString();
                        newDeed.deed_remarks = ds.Tables[0].Rows[0]["Deed_Remarks"].ToString();
                        newDeed.doc_type = ds.Tables[0].Rows[0]["scan_doc_type"].ToString();
                        newDeed.addl_pages = ds.Tables[0].Rows[0]["addl_pages"].ToString();
                        newDeed.hold = ds.Tables[0].Rows[0]["hold"].ToString();
                        newDeed.hold_reason = ds.Tables[0].Rows[0]["hold_reason"].ToString();
                    }
                }
            }
            catch
            {

            }
            return newDeed;
        }

        public DataSet GetDeed(DeedDetails pDeed)
        {
            DataSet ds = new DataSet();
            try
            {
                string sql = "SELECT District_Code,RO_Code,Book,Deed_year,Deed_no " +
                            "FROM Deed_details where district_code='" + pDeed.Deed_control.District_code + "' and RO_code='" + pDeed.Deed_control.RO_code + "' and book='" + pDeed.Deed_control.Book + "' and deed_year='" + pDeed.Deed_control.Deed_year + "'";
                
                OdbcDataAdapter odap = new OdbcDataAdapter(sql, conn);
                odap.Fill(ds);
            }
            catch
            {

            }
            return ds;
        }

        private string GetLast(string s, int tail_length)
        {
            if (tail_length >= s.Length)
                return s;
            return s.Substring(s.Length - tail_length);
        }

        public BindingList<PersonDetails> GetIndex1Details(DeedDetails pDeed)
        {
            BindingList<PersonDetails> indx1List = new BindingList<PersonDetails>();
            try
            {
                string sql = "SELECT A.District_Code,A.RO_Code,A.Book,A.Deed_year,A.Deed_no,A.Item_no,A.initial_name,A.First_name,A.Last_Name" +
                              ",A.Party_code,D.ec_name,A.Admit_code ,A.Address ,A.Address_district_code,A.Address_district_name,A.Address_ps_code,A.Address_ps_Name" +
                               ",A.Father_mother,A.Rel_code,A.Relation ,A.occupation_code,A.religion_code,A.more,A.pin,A.city,A.other_party_code,A.linked_to FROM Index_of_name A,Party_code D where district_code='" + pDeed.Deed_control.District_code + "' and RO_code='" + pDeed.Deed_control.RO_code + "' and book='" + pDeed.Deed_control.Book + "' and deed_year='" + pDeed.Deed_control.Deed_year + "' and deed_no='" + pDeed.Deed_control.Deed_no + "' and A.party_code = D.ec_code order by a.item_no";
                DataSet ds = new DataSet();
                OdbcDataAdapter odap = new OdbcDataAdapter(sql, conn);
                odap.Fill(ds);
                if (ds.Tables.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        PersonDetails indx = new PersonDetails();
                        indx.district_code = ds.Tables[0].Rows[i]["District_code"].ToString();
                        indx.RO_code = ds.Tables[0].Rows[i]["RO_code"].ToString();
                        indx.Book = ds.Tables[0].Rows[i]["Book"].ToString();
                        indx.Deed_year = ds.Tables[0].Rows[i]["Deed_year"].ToString();
                        indx.Deed_no = ds.Tables[0].Rows[i]["Deed_no"].ToString();
                        indx.Serial = ds.Tables[0].Rows[i]["Item_no"].ToString();
                        //indx.EX_CL = ds.Tables[0].Rows[i]["EX_CL"].ToString();
                        indx.initial_name = ds.Tables[0].Rows[i]["initial_name"].ToString();
                        indx.First_name = ds.Tables[0].Rows[i]["First_name"].ToString();
                        indx.Last_Name = ds.Tables[0].Rows[i]["Last_name"].ToString();
                        indx.Status_code = ds.Tables[0].Rows[i]["Party_code"].ToString();
                        indx.Status_name = ds.Tables[0].Rows[i]["ec_name"].ToString();
                        indx.Admit_code = ds.Tables[0].Rows[i]["Admit_code"].ToString();
                        indx.Address = ds.Tables[0].Rows[i]["Address"].ToString();
                        indx.Address_district_code = ds.Tables[0].Rows[i]["Address_district_code"].ToString();
                        //////if (indx.Address.Length > 7)
                        //////{
                        //////    string validatePIN = GetLast(indx.Address, 7);
                        //////    string newPIN = GetLast(indx.Address, 6);
                        //////    int output = 0; 
                        //////    if ((validatePIN.Contains("-")) && int.TryParse(newPIN,out output))
                        //////    {
                        //////        if (indx.Address.IndexOf('-') > 0)
                        //////        {
                        //////            char[] arrAdd = indx.Address.ToCharArray();
                        //////            Array.Reverse(arrAdd);
                        //////            string revAdd = new string(arrAdd);
                        //////            indx.PIN = revAdd.Substring(0, revAdd.IndexOf('-'));
                        //////            indx.City_Name = revAdd.Substring(revAdd.IndexOf('-') + 1, revAdd.IndexOf(' ') - revAdd.IndexOf('-'));

                        //////            char[] arrpin = indx.PIN.ToCharArray();
                        //////            Array.Reverse(arrpin);
                        //////            indx.PIN = new string(arrpin);
                        //////            char[] arrcity = indx.City_Name.ToCharArray();
                        //////            Array.Reverse(arrcity);
                        //////            indx.City_Name = new string(arrcity);
                        //////        }
                        //////        else { indx.PIN = string.Empty; indx.City_Name = string.Empty; }
                        //////    }
                        //////    else { indx.PIN = string.Empty; indx.City_Name = string.Empty; }
                        //////}
                        //////else { indx.PIN = string.Empty; indx.City_Name = string.Empty; }
                        //indx.Address = indx.Address.Replace(indx.City_Name + "-" + indx.PIN, "");
                        indx.Address_district_name = ds.Tables[0].Rows[i]["Address_district_name"].ToString();
                        indx.Address_ps_code = ds.Tables[0].Rows[i]["Address_ps_code"].ToString();
                        indx.Address_ps_Name = ds.Tables[0].Rows[i]["Address_ps_name"].ToString();
                        indx.Father_mother = ds.Tables[0].Rows[i]["Father_mother"].ToString();
                        indx.Rel_code = ds.Tables[0].Rows[i]["Rel_code"].ToString();
                        indx.Relation = ds.Tables[0].Rows[i]["Relation"].ToString();
                        indx.more = ds.Tables[0].Rows[i]["more"].ToString();
                        indx.PIN = ds.Tables[0].Rows[i]["pin"].ToString();
                        indx.City_Name  = ds.Tables[0].Rows[i]["city"].ToString();
                        indx.other_party_code = ds.Tables[0].Rows[i]["other_party_code"].ToString();
                        indx.linked_to = ds.Tables[0].Rows[i]["linked_to"].ToString();
                        //if (indx.Relation.IndexOf(".") > 0)
                        //{
                        //    indx.F_Initial_name = indx.Relation.Substring(0, indx.Relation.IndexOf(".") + 1);
                        //}
                        //else { indx.F_Initial_name = ""; }
                        if (indx.Relation.Length > 0)
                        {
                            //char[] arr = indx.Relation.ToCharArray();
                            //Array.Reverse(arr);
                            //string rev = new string(arr);
                            //indx.F_Last_Name = rev.Substring(0, rev.IndexOf(' '));
                            //char[] arrLast = indx.F_Last_Name.ToCharArray();
                            //Array.Reverse(arrLast);
                            //indx.F_Last_Name = new string(arrLast);
                        }
                        if (indx.F_Initial_name != string.Empty)
                        {
                            //indx.F_First_name = indx.Relation.Substring((indx.F_Initial_name.Length + 1), (indx.Relation.Length - (indx.F_Last_Name.Length + indx.F_Initial_name.Length + 2)));
                            indx.F_First_name = indx.Relation;
                        }
                        else
                        {
                            //indx.F_First_name = indx.Relation.Replace(indx.F_Last_Name, "").Trim();
                            indx.F_First_name = indx.Relation;
                        }

                        if (!string.IsNullOrEmpty(ds.Tables[0].Rows[i]["occupation_code"].ToString()))
                        {
                            indx.Proffession = ds.Tables[0].Rows[i]["occupation_code"].ToString();
                            indx.Proffession_Name = GetProfByCode(ds.Tables[0].Rows[i]["occupation_code"].ToString());
                        }
                        else { indx.Proffession = ds.Tables[0].Rows[i]["occupation_code"].ToString(); }
                        if (!string.IsNullOrEmpty(ds.Tables[0].Rows[i]["religion_code"].ToString()))
                        {
                            indx.Cast = ds.Tables[0].Rows[i]["religion_code"].ToString();
                            indx.Cast_Name = GetReligionByCode(ds.Tables[0].Rows[i]["religion_code"].ToString());
                        }
                        else { indx.Cast = ds.Tables[0].Rows[i]["religion_code"].ToString(); }
                        indx1List.Add(indx);
                    }
                }
            }
            catch
            {

            }
            return indx1List;
        }

        public DataTable GetIndex1Rpt(string pYear, string pVolume)
        {
            DataTable dt = new DataTable();
            int index = 0;
            try
            {
                string sql = "SELECT A.District_Code,A.RO_Code,B.District_name,C.RO_name,A.Book,A.Deed_year,A.Deed_no,A.Item_no,A.initial_name,A.First_name,A.Last_Name" +
                              ",A.Party_code,D.ec_name,A.Admit_code ,A.Address ,A.Address_district_code,A.Address_district_name,A.Address_ps_code,A.Address_ps_Name" +
                               ",A.Father_mother,A.Rel_code,A.Relation ,A.occupation_code,A.religion_code,E.Page_from,E.page_to,E.tran_maj_code,E.tran_min_code,E.Volume_No " +
                               "FROM Index_of_name A,Party_code D,district B,RO_master C,deed_details E " +
                               " where A.district_code=E.district_code and A.RO_code=E.RO_code and A.book=E.book and A.deed_year = E.deed_year " +
                               "and A.deed_no=E.deed_no " +
                               "and A.party_code = D.ec_code and E.district_code=B.district_code and E.RO_code=C.RO_Code and E.deed_year='" + pYear + "' and E.volume_no=" + pVolume + " order by E.Volume_No,val(E.deed_no)";
                DataSet ds = new DataSet();

                dt.Columns.Add("First_name");
                dt.Columns.Add("Relation");
                dt.Columns.Add("Address");
                dt.Columns.Add("Occupation_code");
                dt.Columns.Add("Religion_code");
                dt.Columns.Add("Party_code");
                dt.Columns.Add("tran_maj_code");
                dt.Columns.Add("A_RO_code");
                dt.Columns.Add("B_Deed_year");
                dt.Columns.Add("Page_from");

                DataRow dr;
                OdbcDataAdapter odap = new OdbcDataAdapter(sql, conn);
                odap.Fill(ds);

                if (ds.Tables.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        dr = dt.NewRow();
                        string name = ds.Tables[0].Rows[i]["initial_name"].ToString().Trim() + " " + ds.Tables[0].Rows[i]["First_name"].ToString().Trim() + " " + ds.Tables[0].Rows[i]["Last_name"].ToString().Trim();
                        string relation = string.Empty;

                        if (ds.Tables[0].Rows[i]["Rel_code"].ToString().Trim() == "S")
                        {
                            relation = "Son of";
                        }
                        if (ds.Tables[0].Rows[i]["Rel_code"].ToString().Trim() == "D")
                        {
                            relation = "Daughter of";
                        }
                        if (ds.Tables[0].Rows[i]["Rel_code"].ToString().Trim() == "W")
                        {
                            relation = "Wife of";
                        }
                        if (ds.Tables[0].Rows[i]["Rel_code"].ToString().Trim() == "R")
                        {
                            relation = "Rep. of";
                        }
                        relation = relation + " " + ds.Tables[0].Rows[i]["Relation"].ToString().Trim();
                        string address = ds.Tables[0].Rows[i]["Address"].ToString().Trim();
                        string PIN = string.Empty;
                        if (address.IndexOf('-') > 0)
                        {
                            char[] arrAdd = address.ToCharArray();
                            Array.Reverse(arrAdd);
                            string revAdd = new string(arrAdd);
                            PIN = revAdd.Substring(0, revAdd.IndexOf('-'));

                            char[] arrpin = PIN.ToCharArray();
                            Array.Reverse(arrpin);
                            string newPin = new string(arrpin);
                            PIN = newPin;
                        }
                        address = address.Replace("-" + PIN, "");
                        if (!string.IsNullOrEmpty(ds.Tables[0].Rows[i]["Address_ps_name"].ToString()))
                        { address = address + " Thana:-" + ds.Tables[0].Rows[i]["Address_ps_name"].ToString().Trim(); }
                        if (!string.IsNullOrEmpty(ds.Tables[0].Rows[i]["Address_district_name"].ToString()))
                        { address = address + " District:-" + ds.Tables[0].Rows[i]["Address_district_name"].ToString().Trim(); }
                        address = address + " PIN:-" + PIN;
                        string occupation = "Profession:-";
                        string cast = "Cast:-";
                        if (!string.IsNullOrEmpty(ds.Tables[0].Rows[i]["occupation_code"].ToString()))
                        {
                            occupation = occupation + GetProfByCode(ds.Tables[0].Rows[i]["occupation_code"].ToString());
                        }
                        else { occupation = occupation + "None"; }
                        if (!string.IsNullOrEmpty(ds.Tables[0].Rows[i]["religion_code"].ToString()))
                        {
                            cast = cast + GetReligionByCode(ds.Tables[0].Rows[i]["religion_code"].ToString());
                        }
                        else { cast = cast + "None"; }
                        string party = ds.Tables[0].Rows[i]["ec_name"].ToString();
                        string transaction = string.Empty;
                        if (!string.IsNullOrEmpty(ds.Tables[0].Rows[i]["tran_maj_code"].ToString()))
                        { transaction = GetTranMajorByCode(ds.Tables[0].Rows[i]["tran_maj_code"].ToString()).Trim(); }
                        if (!string.IsNullOrEmpty(ds.Tables[0].Rows[i]["tran_min_code"].ToString()))
                        { transaction = transaction + "," + GetTranMinorByCode(ds.Tables[0].Rows[i]["tran_maj_code"].ToString(), ds.Tables[0].Rows[i]["tran_min_code"].ToString()); }
                        string roname = ds.Tables[0].Rows[i]["RO_name"].ToString().Trim();
                        string deed_year = ds.Tables[0].Rows[i]["Deed_no"].ToString().Trim() + "," + ds.Tables[0].Rows[i]["Deed_year"].ToString().Trim() + "," + ds.Tables[0].Rows[i]["Volume_No"].ToString().Trim();
                        string page = ds.Tables[0].Rows[i]["page_from"].ToString() + "-" + ds.Tables[0].Rows[i]["page_to"].ToString().Trim();
                        dr["First_name"] = name;
                        dr["Relation"] = relation;
                        dr["Address"] = address;
                        dr["Occupation_code"] = occupation;
                        dr["Religion_code"] = cast;
                        dr["Party_code"] = party;
                        dr["tran_maj_code"] = transaction;
                        dr["A_RO_code"] = roname;
                        dr["B_Deed_year"] = deed_year;
                        dr["Page_from"] = page;
                        if (i == 180)
                        { string l = string.Empty; }
                        dt.Rows.Add(dr);

                    }
                }
            }
            catch (Exception ex)
            {
                string err = ex.Message;
            }
            return dt;
        }

        public DataTable GetIndex1RptForDotMatrix(string pYear, string pVolumeNo)
        {
            DataTable dt = new DataTable();
            int index = 0;
            try
            {
                string sql = "SELECT A.District_Code,A.RO_Code,B.District_name,C.RO_name,A.Book,A.Deed_year,A.Deed_no,A.Item_no,A.initial_name,A.First_name,A.Last_Name" +
                              ",A.Party_code,D.ec_name,A.Admit_code ,A.Address ,A.Address_district_code,A.Address_district_name,A.Address_ps_code,A.Address_ps_Name" +
                               ",A.Father_mother,A.Rel_code,A.Relation ,A.occupation_code,A.religion_code,E.Page_from,E.page_to,E.tran_maj_code,E.tran_min_code,E.Volume_No " +
                               "FROM Index_of_name A,Party_code D,district B,RO_master C,deed_details E " +
                               " where A.district_code=E.district_code and A.RO_code=E.RO_code and A.book=E.book and A.deed_year = E.deed_year " +
                               "and A.deed_no=E.deed_no " +
                               "and A.party_code = D.ec_code and E.district_code=B.district_code and E.RO_code=C.RO_Code and E.deed_year='" + pYear + "' and E.volume_no=" + pVolumeNo + " order by E.Volume_No,val(E.deed_no)";
                DataSet ds = new DataSet();

                dt.Columns.Add("First_name");
                dt.Columns.Add("Relation");
                dt.Columns.Add("Address");
                dt.Columns.Add("district");
                dt.Columns.Add("thana");
                dt.Columns.Add("pin");
                dt.Columns.Add("Occupation_code");
                dt.Columns.Add("Religion_code");
                dt.Columns.Add("Party_code");
                dt.Columns.Add("tran_maj_code");
                dt.Columns.Add("A_RO_code");
                dt.Columns.Add("B_Deed_year");
                dt.Columns.Add("Page_from");

                DataRow dr;
                OdbcDataAdapter odap = new OdbcDataAdapter(sql, conn);
                odap.Fill(ds);

                if (ds.Tables.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        dr = dt.NewRow();
                        string name = ds.Tables[0].Rows[i]["initial_name"].ToString().Trim() + " " + ds.Tables[0].Rows[i]["First_name"].ToString().Trim() + " " + ds.Tables[0].Rows[i]["Last_name"].ToString().Trim();
                        string relation = string.Empty;

                        if (ds.Tables[0].Rows[i]["Rel_code"].ToString().Trim() == "S")
                        {
                            relation = "Son of";
                        }
                        if (ds.Tables[0].Rows[i]["Rel_code"].ToString().Trim() == "D")
                        {
                            relation = "Daughter of";
                        }
                        if (ds.Tables[0].Rows[i]["Rel_code"].ToString().Trim() == "W")
                        {
                            relation = "Wife of";
                        }
                        if (ds.Tables[0].Rows[i]["Rel_code"].ToString().Trim() == "R")
                        {
                            relation = "Rep. of";
                        }
                        relation = relation + " " + ds.Tables[0].Rows[i]["Relation"].ToString().Trim();
                        string address = ds.Tables[0].Rows[i]["Address"].ToString().Trim();
                        string thana = string.Empty;
                        string district = string.Empty;
                        string PIN = string.Empty;
                        if (address.IndexOf('-') > 0)
                        {
                            char[] arrAdd = address.ToCharArray();
                            Array.Reverse(arrAdd);
                            string revAdd = new string(arrAdd);
                            PIN = revAdd.Substring(0, revAdd.IndexOf('-'));

                            char[] arrpin = PIN.ToCharArray();
                            Array.Reverse(arrpin);
                            string newPin = new string(arrpin);
                            PIN = newPin;
                        }
                        address = address.Replace("-" + PIN, "");
                        if (!string.IsNullOrEmpty(ds.Tables[0].Rows[i]["Address_ps_name"].ToString()))
                        { thana = " Thana:-" + ds.Tables[0].Rows[i]["Address_ps_name"].ToString().Trim(); }
                        if (!string.IsNullOrEmpty(ds.Tables[0].Rows[i]["Address_district_name"].ToString()))
                        { district = " District:-" + ds.Tables[0].Rows[i]["Address_district_name"].ToString().Trim(); }
                        PIN = " PIN:-" + PIN;
                        string occupation = "Profession:-";
                        string cast = "Cast:-";
                        if (!string.IsNullOrEmpty(ds.Tables[0].Rows[i]["occupation_code"].ToString()))
                        {
                            occupation = occupation + GetProfByCode(ds.Tables[0].Rows[i]["occupation_code"].ToString());
                        }
                        else { occupation = occupation + "None"; }
                        if (!string.IsNullOrEmpty(ds.Tables[0].Rows[i]["religion_code"].ToString()))
                        {
                            cast = cast + GetReligionByCode(ds.Tables[0].Rows[i]["religion_code"].ToString());
                        }
                        else { cast = cast + "None"; }
                        string party = ds.Tables[0].Rows[i]["ec_name"].ToString();
                        string transaction = string.Empty;
                        if (!string.IsNullOrEmpty(ds.Tables[0].Rows[i]["tran_maj_code"].ToString()))
                        { transaction = GetTranMajorByCode(ds.Tables[0].Rows[i]["tran_maj_code"].ToString()).Trim(); }
                        if (!string.IsNullOrEmpty(ds.Tables[0].Rows[i]["tran_min_code"].ToString()))
                        { transaction = transaction + "," + GetTranMinorByCode(ds.Tables[0].Rows[i]["tran_maj_code"].ToString(), ds.Tables[0].Rows[i]["tran_min_code"].ToString()); }
                        string roname = ds.Tables[0].Rows[i]["RO_name"].ToString().Trim();
                        string deed_year = ds.Tables[0].Rows[i]["Deed_no"].ToString().Trim() + "," + ds.Tables[0].Rows[i]["Deed_year"].ToString().Trim() + "," + ds.Tables[0].Rows[i]["Volume_No"].ToString().Trim();
                        string page = ds.Tables[0].Rows[i]["page_from"].ToString() + "-" + ds.Tables[0].Rows[i]["page_to"].ToString().Trim();
                        dr["First_name"] = name;
                        dr["Relation"] = relation;
                        dr["Address"] = address;
                        dr["district"] = district;
                        dr["thana"] = thana;
                        dr["pin"] = PIN;
                        dr["Occupation_code"] = occupation;
                        dr["Religion_code"] = cast;
                        dr["Party_code"] = party;
                        dr["tran_maj_code"] = transaction;
                        dr["A_RO_code"] = roname;
                        dr["B_Deed_year"] = deed_year;
                        dr["Page_from"] = page;
                        if (i == 180)
                        { string l = string.Empty; }
                        dt.Rows.Add(dr);

                    }
                }
            }
            catch (Exception ex)
            {
                string err = ex.Message;
            }
            return dt;
        }



        public BindingList<PropertyDetails> GetIndex2Details(DeedDetails pDeed)
        {
            BindingList<PropertyDetails> indx2List = new BindingList<PropertyDetails>();
            try
            {
                string sql = "SELECT District_Code ,RO_Code,Book,Deed_year,Deed_no ,Item_no,Property_district_code,Property_ro_code,ps_code" +
                            ",moucode,Area_type,GP_Muni_Corp_Code,Ward,Holding,Premises,road_code,Plot_code_type,Road,Plot_No" +
                            ",Bata_No,khatian_type,Khatian_No,bata_khatian_no,property_type,Land_Area_acre,land_area_bigha,land_area_decimal" +
                            ",land_area_katha,land_area_chatak,land_area_sqfeet,Structure_area_in_sqFeet,ref_ps,ref_mouza,jl_no,RefJL_no,land_type FROM Index_of_Property where district_code='" + pDeed.Deed_control.District_code + "' and RO_code='" + pDeed.Deed_control.RO_code + "' and book='" + pDeed.Deed_control.Book + "' and deed_year='" + pDeed.Deed_control.Deed_year + "' and deed_no='" + pDeed.Deed_control.Deed_no + "' order by item_no";
                DataSet ds = new DataSet();
                OdbcDataAdapter odap = new OdbcDataAdapter(sql, conn);
                odap.Fill(ds);
                if (ds.Tables.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        PropertyDetails indx = new PropertyDetails();
                        indx.district_code = ds.Tables[0].Rows[i]["District_code"].ToString();
                        indx.RO_code = ds.Tables[0].Rows[i]["RO_code"].ToString();
                        indx.Book = ds.Tables[0].Rows[i]["Book"].ToString();
                        indx.Deed_year = ds.Tables[0].Rows[i]["Deed_year"].ToString();
                        indx.Deed_no = ds.Tables[0].Rows[i]["Deed_no"].ToString();
                        indx.Serial = ds.Tables[0].Rows[i]["Item_no"].ToString();
                        indx.Property_district_code = ds.Tables[0].Rows[i]["Property_district_code"].ToString();
                        indx.Property_ro_code = ds.Tables[0].Rows[i]["Property_ro_code"].ToString();
                        indx.ps_code = ds.Tables[0].Rows[i]["ps_code"].ToString();
                        indx.moucode = ds.Tables[0].Rows[i]["moucode"].ToString();
                        indx.Area_type = ds.Tables[0].Rows[i]["Area_type"].ToString();
                        indx.GP_Muni_Corp_Code = ds.Tables[0].Rows[i]["GP_Muni_Corp_Code"].ToString();
                        indx.Ward = ds.Tables[0].Rows[i]["ward"].ToString();
                        indx.Holding = ds.Tables[0].Rows[i]["Holding"].ToString();
                        indx.Premises = ds.Tables[0].Rows[i]["premises"].ToString();
                        indx.road_code = ds.Tables[0].Rows[i]["road_code"].ToString();
                        indx.Plot_code_type = ds.Tables[0].Rows[i]["Plot_code_type"].ToString();
                        indx.Road = ds.Tables[0].Rows[i]["Road"].ToString();
                        indx.Plot_No = string.Format(ds.Tables[0].Rows[i]["plot_No"].ToString(), "{0:0.0000}");
                        indx.Bata_No = string.Format(ds.Tables[0].Rows[i]["Bata_no"].ToString(), "{0:0.0000}");
                        indx.Khatian_type = ds.Tables[0].Rows[i]["Khatian_type"].ToString();
                        indx.khatian_No = ds.Tables[0].Rows[i]["khatian_No"].ToString();
                        indx.bata_khatian_no = ds.Tables[0].Rows[i]["bata_khatian_no"].ToString();
                        indx.property_type = ds.Tables[0].Rows[i]["property_type"].ToString();
                        indx.Land_Area_acre = string.Format(ds.Tables[0].Rows[i]["Land_Area_acre"].ToString(), "{0:0.0000}");
                        indx.Land_Area_bigha = string.Format(ds.Tables[0].Rows[i]["Land_Area_bigha"].ToString(), "{0:0.0000}");
                        indx.Land_Area_chatak = string.Format(ds.Tables[0].Rows[i]["Land_Area_chatak"].ToString(), "{0:0.0000}");
                        indx.Land_Area_decimal = string.Format(ds.Tables[0].Rows[i]["Land_Area_decimal"].ToString(), "{0:0.0000}");
                        indx.Land_Area_katha = string.Format(ds.Tables[0].Rows[i]["Land_Area_katha"].ToString(), "{0:0.0000}");
                        indx.Land_Area_sqfeet = string.Format(ds.Tables[0].Rows[i]["Land_Area_sqfeet"].ToString(), "{0:0.0000}");
                        indx.Structure_area_in_sqFeet = string.Format(ds.Tables[0].Rows[i]["Structure_area_in_sqFeet"].ToString(), "{0:0.0000}");
                        string amount = string.Empty;
                        if (!string.IsNullOrEmpty(indx.Land_Area_acre) && (indx.Land_Area_acre != "0.0000"))
                        { amount = indx.Land_Area_acre + " Acre "; }
                        else { indx.Land_Area_acre = "0"; }
                        if (!string.IsNullOrEmpty(indx.Land_Area_bigha) && (indx.Land_Area_bigha != "0.0000"))
                        { amount = amount + indx.Land_Area_bigha + " Bigha "; }
                        else { indx.Land_Area_bigha = "0"; }
                        if (!string.IsNullOrEmpty(indx.Land_Area_decimal) && (indx.Land_Area_decimal != "0.0000"))
                        { amount = amount + indx.Land_Area_decimal + " Decimal "; }
                        else { indx.Land_Area_decimal = "0"; }
                        if (!string.IsNullOrEmpty(indx.Land_Area_katha) && (indx.Land_Area_katha != "0.0000"))
                        { amount = amount + indx.Land_Area_katha + " Katha "; }
                        else { indx.Land_Area_katha = "0"; }
                        if (!string.IsNullOrEmpty(indx.Land_Area_chatak) && (indx.Land_Area_chatak != "0.0000"))
                        { amount = amount + indx.Land_Area_chatak + " Chatak "; }
                        else { indx.Land_Area_chatak = "0"; }
                        if (!string.IsNullOrEmpty(indx.Land_Area_katha) && (indx.Land_Area_katha != "0.0000"))
                        { amount = amount + indx.Land_Area_katha + " Katha "; }
                        else { indx.Land_Area_katha = "0"; }
                        if (!string.IsNullOrEmpty(indx.Land_Area_sqfeet) && (indx.Land_Area_sqfeet != "0.0000"))
                        { amount = amount + indx.Land_Area_sqfeet + " Sqfeet "; }
                        else { indx.Land_Area_sqfeet = "0"; }
                        if (!string.IsNullOrEmpty(indx.Structure_area_in_sqFeet) && (indx.Structure_area_in_sqFeet != "0.0000"))
                        { amount = amount + indx.Structure_area_in_sqFeet + " Sqfeet "; }
                        else { indx.Structure_area_in_sqFeet = "0"; }
                        //indx.Structure_area_in_sqFeet = ds.Tables[0].Rows[i]["Structure_area_in_sqFeet"].ToString();
                        if (indx.Area_type == "C" || indx.Area_type == "M")
                        {
                            string t = (indx.GP_Muni_Corp_Code != string.Empty) ? GetSubByCode(indx.Property_district_code, indx.GP_Muni_Corp_Code) : "None";
                            indx.Property_details = indx.Area_type + ":" + t + "." + "Premises: " + indx.Premises + "." + indx.property_type + " Area:" + amount;
                        }
                        else
                        {
                            string t = (indx.GP_Muni_Corp_Code != string.Empty) ? GetGPByCode(indx.Property_district_code, indx.ps_code, indx.GP_Muni_Corp_Code) : "None";
                            indx.Property_details = indx.Area_type + ":" + t + "." + "Premises: " + indx.Premises + "." + indx.property_type + " Area:" + amount;
                        }
                        if (!string.IsNullOrEmpty(indx.ps_code))
                        { indx.Police_Station = GetPSByCode(indx.Property_district_code, indx.ps_code); }
                        if (!string.IsNullOrEmpty(indx.Property_district_code))
                        { indx.District = GetDistrictByCode(indx.Property_district_code); }
                        if (!string.IsNullOrEmpty(indx.Property_ro_code))
                        { indx.Where_Registered = GetROByCode(indx.Property_ro_code); }
                        string transaction = string.Empty;
                        if (!string.IsNullOrEmpty(pDeed.tran_maj_code))
                        { transaction = GetTranMajorByCode(pDeed.tran_maj_code); }
                        if (!string.IsNullOrEmpty(pDeed.tran_min_code))
                        { transaction = transaction + "," + GetTranMinorByCode(pDeed.tran_maj_code,pDeed.tran_min_code); }
                        indx.Nature_of_Transaction = transaction;
                        indx.Ref_ps = ds.Tables[0].Rows[i]["ref_ps"].ToString();
                        indx.Ref_mou = ds.Tables[0].Rows[i]["ref_mouza"].ToString();
                        indx.JL_NO = ds.Tables[0].Rows[i]["jl_no"].ToString();
                        indx.Ref_JL_Number = ds.Tables[0].Rows[i]["Refjl_no"].ToString();
                        indx.land_type = ds.Tables[0].Rows[i]["land_type"].ToString();
                        indx2List.Add(indx);
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return indx2List;
        }

        public DataTable GetIndex2Rpt(string pDeedYear, string pVolume)
        {
            DataRow dr = null;
            DataTable dt = new DataTable();
            try
            {
                string sql = "SELECT A.District_Code ,A.RO_Code,B.District_name,C.RO_name,A.Book,A.Deed_year,A.Deed_no ,A.Item_no,A.Property_district_code,A.Property_ro_code,A.ps_code" +
                            ",A.moucode,A.Area_type,A.GP_Muni_Corp_Code,A.Ward,A.Holding,A.Premises,A.road_code,A.Plot_code_type,A.Road,A.Plot_No" +
                            ",A.Bata_No,A.khatian_type,A.Khatian_No,A.bata_khatian_no,A.property_type,A.Land_Area_acre,A.land_area_bigha,A.land_area_decimal" +
                            ",A.land_area_katha,A.land_area_chatak,A.land_area_sqfeet,A.Structure_area_in_sqFeet,E.Page_from,E.page_to,E.tran_maj_code,E.tran_min_code,E.Volume_No" +
                            " FROM Index_of_Property A,district B,RO_master C,deed_details E " +
                            " where A.district_code=E.district_code and A.RO_code=E.RO_code and A.book=E.book and A.deed_year = E.deed_year " +
                            " and A.deed_no=E.deed_no and A.deed_year='" + pDeedYear + "' and E.volume_no=" + pVolume +
                            " and A.district_code=B.district_code and A.RO_code=C.RO_Code order by E.Volume_No,val(E.deed_no)";
                DataSet ds = new DataSet();

                dt.Columns.Add("Road");
                dt.Columns.Add("ps_code");
                dt.Columns.Add("Property_district_code");
                dt.Columns.Add("Property_ro_code");
                dt.Columns.Add("tran_maj_code");
                dt.Columns.Add("B_Deed_year");
                dt.Columns.Add("Page_from");

                OdbcDataAdapter odap = new OdbcDataAdapter(sql, conn);
                odap.Fill(ds);
                if (ds.Tables.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        dr = dt.NewRow();
                        PropertyDetails indx = new PropertyDetails();
                        indx.district_code = ds.Tables[0].Rows[i]["District_code"].ToString();
                        indx.RO_code = ds.Tables[0].Rows[i]["RO_code"].ToString();
                        indx.Book = ds.Tables[0].Rows[i]["Book"].ToString();
                        indx.Deed_year = ds.Tables[0].Rows[i]["Deed_year"].ToString();
                        indx.Deed_no = ds.Tables[0].Rows[i]["Deed_no"].ToString();
                        indx.Serial = ds.Tables[0].Rows[i]["Item_no"].ToString();
                        indx.Property_district_code = ds.Tables[0].Rows[i]["Property_district_code"].ToString();
                        indx.Property_ro_code = ds.Tables[0].Rows[i]["Property_ro_code"].ToString();
                        indx.ps_code = ds.Tables[0].Rows[i]["ps_code"].ToString();
                        indx.moucode = ds.Tables[0].Rows[i]["moucode"].ToString();
                        indx.Area_type = ds.Tables[0].Rows[i]["Area_type"].ToString();
                        indx.GP_Muni_Corp_Code = ds.Tables[0].Rows[i]["GP_Muni_Corp_Code"].ToString();
                        indx.Ward = ds.Tables[0].Rows[i]["ward"].ToString();
                        indx.Holding = ds.Tables[0].Rows[i]["Holding"].ToString();
                        indx.Premises = ds.Tables[0].Rows[i]["premises"].ToString();
                        indx.road_code = ds.Tables[0].Rows[i]["road_code"].ToString();
                        indx.Plot_code_type = ds.Tables[0].Rows[i]["Plot_code_type"].ToString();
                        indx.Road = ds.Tables[0].Rows[i]["Road"].ToString();
                        indx.Plot_No = ds.Tables[0].Rows[i]["plot_No"].ToString();
                        indx.Bata_No = ds.Tables[0].Rows[i]["Bata_no"].ToString();
                        indx.Khatian_type = ds.Tables[0].Rows[i]["Khatian_type"].ToString();
                        indx.khatian_No = ds.Tables[0].Rows[i]["khatian_No"].ToString();
                        indx.bata_khatian_no = ds.Tables[0].Rows[i]["bata_khatian_no"].ToString();
                        indx.property_type = GetPropertyTypeByCode(ds.Tables[0].Rows[i]["property_type"].ToString());
                        indx.Land_Area_acre = ds.Tables[0].Rows[i]["Land_Area_acre"].ToString();
                        indx.Land_Area_bigha = ds.Tables[0].Rows[i]["Land_Area_bigha"].ToString();
                        indx.Land_Area_chatak = ds.Tables[0].Rows[i]["Land_Area_chatak"].ToString();
                        indx.Land_Area_decimal = ds.Tables[0].Rows[i]["Land_Area_decimal"].ToString();
                        indx.Land_Area_katha = ds.Tables[0].Rows[i]["Land_Area_katha"].ToString();
                        indx.Land_Area_sqfeet = ds.Tables[0].Rows[i]["Land_Area_sqfeet"].ToString();
                        indx.Structure_area_in_sqFeet = ds.Tables[0].Rows[i]["Structure_area_in_sqFeet"].ToString();
                        string amount = string.Empty;
                        if (!string.IsNullOrEmpty(indx.Land_Area_acre) && (indx.Land_Area_acre != "0.0000"))
                        { amount = Math.Round(Convert.ToDecimal(indx.Land_Area_acre), 2) + " Acre "; }
                        else { indx.Land_Area_acre = "0"; }
                        if (!string.IsNullOrEmpty(indx.Land_Area_bigha) && (indx.Land_Area_bigha != "0.0000"))
                        { amount = amount + Math.Round(Convert.ToDecimal(indx.Land_Area_bigha), 2) + " Bigha "; }
                        else { indx.Land_Area_bigha = "0"; }
                        if (!string.IsNullOrEmpty(indx.Land_Area_decimal) && (indx.Land_Area_decimal != "0.0000"))
                        { amount = amount + Math.Round(Convert.ToDecimal(indx.Land_Area_decimal), 2) + " Decimal "; }
                        else { indx.Land_Area_decimal = "0"; }
                        if (!string.IsNullOrEmpty(indx.Land_Area_katha) && (indx.Land_Area_katha != "0.0000"))
                        { amount = amount + Math.Round(Convert.ToDecimal(indx.Land_Area_katha), 2) + " Katha "; }
                        else { indx.Land_Area_katha = "0"; }
                        if (!string.IsNullOrEmpty(indx.Land_Area_chatak) && (indx.Land_Area_chatak != "0.0000"))
                        { amount = amount + Math.Round(Convert.ToDecimal(indx.Land_Area_chatak), 2) + " Chatak "; }
                        else { indx.Land_Area_chatak = "0"; }
                        if (!string.IsNullOrEmpty(indx.Land_Area_sqfeet) && (indx.Land_Area_sqfeet != "0.0000"))
                        { amount = amount + Math.Round(Convert.ToDecimal(indx.Land_Area_sqfeet), 2) + " Sqfeet "; }
                        else { indx.Land_Area_sqfeet = "0"; }
                        if (!string.IsNullOrEmpty(indx.Structure_area_in_sqFeet) && (indx.Structure_area_in_sqFeet != "0.0000"))
                        { amount = amount + Math.Round(Convert.ToDecimal(indx.Structure_area_in_sqFeet), 2) + " Sqfeet "; }
                        else { indx.Structure_area_in_sqFeet = "0"; }
                        //indx.Structure_area_in_sqFeet = ds.Tables[0].Rows[i]["Structure_area_in_sqFeet"].ToString();
                        if (indx.Area_type == "C" || indx.Area_type == "M")
                        {

                            string t = (indx.GP_Muni_Corp_Code != string.Empty) ? GetSubByCode(indx.district_code, indx.GP_Muni_Corp_Code) : "None";
                            indx.Property_details = (indx.Area_type == "C") ? "Corp." : "Muni" + ":" + t;// +"." + "Premises: " + indx.Premises + "." + indx.property_type + " Area:" + amount;
                        }
                        else if (indx.Area_type == "G")
                        {
                            string t = (indx.GP_Muni_Corp_Code != string.Empty) ? GetGPByCode(indx.Property_district_code, indx.ps_code, indx.GP_Muni_Corp_Code) : "None";
                            indx.Property_details = "GP" + ":" + t;// ;
                        }
                        if (!string.IsNullOrEmpty(indx.Road)) { indx.Property_details = indx.Property_details + " " + indx.Road; }
                        if (!string.IsNullOrEmpty(indx.moucode)) { indx.Property_details = indx.Property_details + "Mouza: " + GetMouzaByCode(indx.Property_district_code, indx.ps_code, indx.moucode); }
                        indx.Property_details = indx.Property_details + " " + "Premises: " + indx.Premises + "." + indx.property_type + " Area:" + amount;
                        if (!string.IsNullOrEmpty(indx.ps_code))
                        { indx.Police_Station = GetPSByCode(indx.Property_district_code, indx.ps_code); }
                        if (!string.IsNullOrEmpty(indx.Property_district_code))
                        { indx.District = GetDistrictByCode(indx.Property_district_code); }
                        if (!string.IsNullOrEmpty(indx.Property_ro_code))
                        { indx.Where_Registered = GetROByCode(indx.Property_ro_code); }
                        string transaction = string.Empty;
                        if (!string.IsNullOrEmpty(ds.Tables[0].Rows[i]["tran_maj_code"].ToString()))
                        { transaction = GetTranMajorByCode(ds.Tables[0].Rows[i]["tran_maj_code"].ToString()).Trim(); }
                        if (!string.IsNullOrEmpty(ds.Tables[0].Rows[i]["tran_min_code"].ToString()))
                        { transaction = transaction + "," + GetTranMinorByCode(ds.Tables[0].Rows[i]["tran_maj_code"].ToString(),ds.Tables[0].Rows[i]["tran_min_code"].ToString()); }
                        indx.Nature_of_Transaction = transaction;

                        string deed_year = indx.Deed_no + "," + indx.Deed_year + "," + ds.Tables[0].Rows[i]["Volume_No"].ToString().Trim();
                        string page = ds.Tables[0].Rows[i]["page_from"].ToString() + "-" + ds.Tables[0].Rows[i]["page_to"].ToString().Trim();
                        dr["Road"] = indx.Property_details;
                        dr["ps_code"] = indx.Police_Station;
                        dr["Property_district_code"] = indx.District;
                        dr["Property_ro_code"] = indx.Where_Registered;
                        dr["tran_maj_code"] = transaction;
                        dr["B_Deed_year"] = deed_year;
                        dr["Page_from"] = page;
                        if (i == 180)
                        { string l = string.Empty; }
                        dt.Rows.Add(dr);
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return dt;
        }

        public DataTable GetIndex2RptForDotMatrix(string pDeedYear, string pVolume)
        {
            DataRow dr = null;
            DataTable dt = new DataTable();
            try
            {
                string sql = "SELECT A.District_Code ,A.RO_Code,B.District_name,C.RO_name,A.Book,A.Deed_year,A.Deed_no ,A.Item_no,A.Property_district_code,A.Property_ro_code,A.ps_code" +
                            ",A.moucode,A.Area_type,A.GP_Muni_Corp_Code,A.Ward,A.Holding,A.Premises,A.road_code,A.Plot_code_type,A.Road,A.Plot_No" +
                            ",A.Bata_No,A.khatian_type,A.Khatian_No,A.bata_khatian_no,A.property_type,A.Land_Area_acre,A.land_area_bigha,A.land_area_decimal" +
                            ",A.land_area_katha,A.land_area_chatak,A.land_area_sqfeet,A.Structure_area_in_sqFeet,E.Page_from,E.page_to,E.tran_maj_code,E.tran_min_code,E.Volume_No" +
                            " FROM Index_of_Property A,district B,RO_master C,deed_details E " +
                            " where A.district_code=E.district_code and A.RO_code=E.RO_code and A.book=E.book and A.deed_year = E.deed_year " +
                            " and A.deed_no=E.deed_no and A.deed_year='" + pDeedYear + "' and E.Volume_no=" + pVolume +
                            " and A.district_code=B.district_code and A.RO_code=C.RO_Code order by E.Volume_No,val(E.deed_no)";
                DataSet ds = new DataSet();

                dt.Columns.Add("GP");
                dt.Columns.Add("Road");
                dt.Columns.Add("Mouza");
                dt.Columns.Add("Premises");
                dt.Columns.Add("Area");
                dt.Columns.Add("ps_code");
                dt.Columns.Add("Property_district_code");
                dt.Columns.Add("Property_ro_code");
                dt.Columns.Add("tran_maj_code");
                dt.Columns.Add("B_Deed_year");
                dt.Columns.Add("Page_from");
                dt.Columns.Add("Dag");
                dt.Columns.Add("Khatian");

                OdbcDataAdapter odap = new OdbcDataAdapter(sql, conn);
                odap.Fill(ds);
                if (ds.Tables.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        dr = dt.NewRow();
                        PropertyDetails indx = new PropertyDetails();
                        indx.district_code = ds.Tables[0].Rows[i]["District_code"].ToString();
                        indx.RO_code = ds.Tables[0].Rows[i]["RO_code"].ToString();
                        indx.Book = ds.Tables[0].Rows[i]["Book"].ToString();
                        indx.Deed_year = ds.Tables[0].Rows[i]["Deed_year"].ToString();
                        indx.Deed_no = ds.Tables[0].Rows[i]["Deed_no"].ToString();

                        indx.Serial = ds.Tables[0].Rows[i]["Item_no"].ToString();
                        indx.Property_district_code = ds.Tables[0].Rows[i]["Property_district_code"].ToString();
                        indx.Property_ro_code = ds.Tables[0].Rows[i]["Property_ro_code"].ToString();
                        indx.ps_code = ds.Tables[0].Rows[i]["ps_code"].ToString();
                        indx.moucode = ds.Tables[0].Rows[i]["moucode"].ToString();
                        indx.Area_type = ds.Tables[0].Rows[i]["Area_type"].ToString();
                        indx.GP_Muni_Corp_Code = ds.Tables[0].Rows[i]["GP_Muni_Corp_Code"].ToString();
                        indx.Ward = ds.Tables[0].Rows[i]["ward"].ToString();
                        indx.Holding = ds.Tables[0].Rows[i]["Holding"].ToString();
                        indx.Premises = ds.Tables[0].Rows[i]["premises"].ToString();
                        indx.road_code = ds.Tables[0].Rows[i]["road_code"].ToString();
                        indx.Plot_code_type = ds.Tables[0].Rows[i]["Plot_code_type"].ToString();
                        indx.Road = ds.Tables[0].Rows[i]["Road"].ToString();
                        indx.Plot_No = ds.Tables[0].Rows[i]["plot_No"].ToString();
                        indx.Bata_No = ds.Tables[0].Rows[i]["Bata_no"].ToString();
                        indx.Khatian_type = ds.Tables[0].Rows[i]["Khatian_type"].ToString();
                        indx.khatian_No = ds.Tables[0].Rows[i]["khatian_No"].ToString();
                        indx.bata_khatian_no = ds.Tables[0].Rows[i]["bata_khatian_no"].ToString();
                        string Plot = "Plot Type: " + indx.Plot_code_type + " Plot No.: " + indx.Plot_No + " Bata No.: " + (indx.Bata_No != "0" ? indx.Bata_No : "");
                        string khatian = "Khatian Type: " + indx.Khatian_type + " Khatian No.: " + indx.khatian_No + " Khatian Bata: " + (indx.bata_khatian_no != "0" ? indx.bata_khatian_no : "");
                        indx.property_type = GetPropertyTypeByCode(ds.Tables[0].Rows[i]["property_type"].ToString());
                        indx.Land_Area_acre = ds.Tables[0].Rows[i]["Land_Area_acre"].ToString();
                        indx.Land_Area_bigha = ds.Tables[0].Rows[i]["Land_Area_bigha"].ToString();
                        indx.Land_Area_chatak = ds.Tables[0].Rows[i]["Land_Area_chatak"].ToString();
                        indx.Land_Area_decimal = ds.Tables[0].Rows[i]["Land_Area_decimal"].ToString();
                        indx.Land_Area_katha = ds.Tables[0].Rows[i]["Land_Area_katha"].ToString();
                        indx.Land_Area_sqfeet = ds.Tables[0].Rows[i]["Land_Area_sqfeet"].ToString();
                        indx.Structure_area_in_sqFeet = ds.Tables[0].Rows[i]["Structure_area_in_sqFeet"].ToString();
                        string amount = string.Empty;
                        if (!string.IsNullOrEmpty(indx.Land_Area_acre) && (indx.Land_Area_acre != "0.0000") && (indx.Land_Area_acre != "0"))
                        { amount = Math.Round(Convert.ToDecimal(indx.Land_Area_acre), 2) + " Acre "; }
                        else { indx.Land_Area_acre = "0"; }
                        if (!string.IsNullOrEmpty(indx.Land_Area_bigha) && (indx.Land_Area_bigha != "0.0000") && (indx.Land_Area_bigha != "0"))
                        { amount = amount + Math.Round(Convert.ToDecimal(indx.Land_Area_bigha), 2) + " Bigha "; }
                        else { indx.Land_Area_bigha = "0"; }
                        if (!string.IsNullOrEmpty(indx.Land_Area_decimal) && (indx.Land_Area_decimal != "0.0000") && (indx.Land_Area_decimal != "0"))
                        { amount = amount + Math.Round(Convert.ToDecimal(indx.Land_Area_decimal), 2) + " Decimal "; }
                        else { indx.Land_Area_decimal = "0"; }
                        if (!string.IsNullOrEmpty(indx.Land_Area_katha) && (indx.Land_Area_katha != "0.0000") && (indx.Land_Area_katha != "0"))
                        { amount = amount + Math.Round(Convert.ToDecimal(indx.Land_Area_katha), 2) + " Katha "; }
                        else { indx.Land_Area_katha = "0"; }
                        if (!string.IsNullOrEmpty(indx.Land_Area_chatak) && (indx.Land_Area_chatak != "0.0000") && (indx.Land_Area_chatak != "0"))
                        { amount = amount + Math.Round(Convert.ToDecimal(indx.Land_Area_chatak), 2) + " Chatak "; }
                        else { indx.Land_Area_chatak = "0"; }
                        if (!string.IsNullOrEmpty(indx.Land_Area_sqfeet) && (indx.Land_Area_sqfeet != "0.0000") && (indx.Land_Area_sqfeet != "0"))
                        { amount = amount + Math.Round(Convert.ToDecimal(indx.Land_Area_sqfeet), 2) + " Sqfeet "; }
                        else { indx.Land_Area_sqfeet = "0"; }
                        if (!string.IsNullOrEmpty(indx.Structure_area_in_sqFeet) && (indx.Structure_area_in_sqFeet != "0.0000") && (indx.Structure_area_in_sqFeet != "0"))
                        { amount = amount + Math.Round(Convert.ToDecimal(indx.Structure_area_in_sqFeet), 2) + " Sqfeet "; }
                        else { indx.Structure_area_in_sqFeet = "0"; }
                        //indx.Structure_area_in_sqFeet = ds.Tables[0].Rows[i]["Structure_area_in_sqFeet"].ToString();
                        if (indx.Area_type == "C" || indx.Area_type == "M")
                        {

                            string t = (indx.GP_Muni_Corp_Code != string.Empty) ? GetSubByCode(indx.Property_district_code, indx.GP_Muni_Corp_Code) : "None";
                            indx.Property_details = (indx.Area_type == "C") ? "Corp." : "Muni" + ":" + t;// +"." + "Premises: " + indx.Premises + "." + indx.property_type + " Area:" + amount;
                        }
                        else if (indx.Area_type == "G")
                        {
                            string t = (indx.GP_Muni_Corp_Code != string.Empty) ? GetGPByCode(indx.Property_district_code, indx.ps_code, indx.GP_Muni_Corp_Code) : "None";
                            indx.Property_details = "GP" + ":" + t;// ;
                        }
                        string gp = indx.Property_details;
                        if (!string.IsNullOrEmpty(indx.Road)) { indx.Property_details = indx.Property_details + " " + indx.Road; }
                        string road = indx.Road;
                        string mouza = string.Empty;
                        if (!string.IsNullOrEmpty(indx.moucode)) { mouza = " Mouza:" + GetMouzaByCode(indx.Property_district_code, indx.ps_code, indx.moucode); indx.Property_details = indx.Property_details + mouza; }
                        string premises = "Premises: " + indx.Premises + mouza + " Road:" + indx.Road;
                        string area = indx.property_type.Trim() + " Area:" + amount;
                        indx.Property_details = indx.Property_details + " " + premises;

                        if (!string.IsNullOrEmpty(indx.ps_code))
                        { indx.Police_Station = GetPSByCode(indx.Property_district_code, indx.ps_code); }
                        if (!string.IsNullOrEmpty(indx.Property_district_code))
                        { indx.District = GetDistrictByCode(indx.Property_district_code); }
                        if (!string.IsNullOrEmpty(indx.Property_ro_code))
                        { indx.Where_Registered = GetROByCode(indx.Property_ro_code); }
                        string transaction = string.Empty;
                        if (!string.IsNullOrEmpty(ds.Tables[0].Rows[i]["tran_maj_code"].ToString()))
                        { transaction = GetTranMajorByCode(ds.Tables[0].Rows[i]["tran_maj_code"].ToString()).Trim(); }
                        if (!string.IsNullOrEmpty(ds.Tables[0].Rows[i]["tran_min_code"].ToString()))
                        { transaction = transaction + "," + GetTranMinorByCode(ds.Tables[0].Rows[i]["tran_maj_code"].ToString(),ds.Tables[0].Rows[i]["tran_min_code"].ToString()); }
                        indx.Nature_of_Transaction = transaction;

                        string deed_year = indx.Deed_no + "," + indx.Deed_year + "," + ds.Tables[0].Rows[i]["Volume_No"].ToString().Trim();
                        string page = ds.Tables[0].Rows[i]["page_from"].ToString() + "-" + ds.Tables[0].Rows[i]["page_to"].ToString().Trim();
                        dr["gp"] = gp;
                        dr["Road"] = road;
                        dr["Mouza"] = mouza;
                        dr["Premises"] = premises;
                        dr["Area"] = area;
                        dr["ps_code"] = indx.Police_Station;
                        dr["Property_district_code"] = indx.District;
                        dr["Property_ro_code"] = indx.Where_Registered;
                        dr["tran_maj_code"] = transaction;
                        dr["B_Deed_year"] = deed_year;
                        dr["Page_from"] = page;
                        dr["Dag"] = Plot;
                        dr["Khatian"] = khatian;
                        if (i == 180)
                        { string l = string.Empty; }
                        dt.Rows.Add(dr);
                    }
                }
            }
            catch (Exception ex)
            {
                string u = "Hi";
            }
            return dt;
        }


        public ArrayList GetTotalDaily(Credentials crd)
        {
            ArrayList totList = new ArrayList();
            string sql = "Select district_code from deed_details where date_format(created_DTTM,'dd/MM/yyyy')=date_format(now(),'dd/MM/yyyy') and created_by = '"+crd.created_by+"'";
            DataSet ds = new DataSet();
            OdbcDataAdapter odap = new OdbcDataAdapter(sql, conn);
            odap.Fill(ds);
            if (ds.Tables.Count > 0)
            { totList.Add(ds.Tables[0].Rows.Count); }
            else { totList.Add("0"); }

            sql = "Select district_code from index_of_name where date_format(created_DTTM,'dd/MM/yyyy')=date_format(now(),'dd/MM/yyyy') and created_by = '" + crd.created_by + "'";
            ds = new DataSet();
            odap = new OdbcDataAdapter(sql, conn);
            odap.Fill(ds);
            if (ds.Tables.Count > 0)
            { totList.Add(ds.Tables[0].Rows.Count); }
            else { totList.Add("0"); }

            sql = "Select district_code from index_of_property where date_format(created_DTTM,'dd/MM/yyyy')=date_format(now(),'dd/MM/yyyy') and created_by = '" + crd.created_by + "'";
            ds = new DataSet();
            odap = new OdbcDataAdapter(sql, conn);
            odap.Fill(ds);
            if (ds.Tables.Count > 0)
            { totList.Add(ds.Tables[0].Rows.Count); }
            else { totList.Add("0"); }

            return totList;
        }
        public ArrayList GetTotalDaily(string stdate, string enddate,Credentials crd)
        {
            ArrayList totList = new ArrayList();
            string sql = "Select district_code from deed_details where date_format(created_DTTM,'dd/MM/yyyy') between date_format('" + stdate + "','dd/MM/yyyy') and date_format('" + enddate + "','dd/MM/yyyy') and created_by = '" + crd.created_by + "' order by created_dttm";
            DataSet ds = new DataSet();
            OdbcDataAdapter odap = new OdbcDataAdapter(sql, conn);
            odap.Fill(ds);
            if (ds.Tables.Count > 0)
            { totList.Add(ds.Tables[0].Rows.Count); }
            else { totList.Add("0"); }

            sql = "Select district_code from index_of_name where date_format(created_DTTM,'dd/MM/yyyy') between date_format('" + stdate + "','dd/MM/yyyy') and date_format('" + enddate + "','dd/MM/yyyy') and created_by = '" + crd.created_by + "' order by created_dttm";
            ds = new DataSet();
            odap = new OdbcDataAdapter(sql, conn);
            odap.Fill(ds);
            if (ds.Tables.Count > 0)
            { totList.Add(ds.Tables[0].Rows.Count); }
            else { totList.Add("0"); }

            sql = "Select district_code from index_of_property where date_format(created_DTTM,'dd/MM/yyyy') between date_format('" + stdate + "','dd/MM/yyyy') and date_format('" + enddate + "','dd/MM/yyyy') and created_by = '" + crd.created_by + "' order by created_dttm";
            ds = new DataSet();
            odap = new OdbcDataAdapter(sql, conn);
            odap.Fill(ds);
            if (ds.Tables.Count > 0)
            { totList.Add(ds.Tables[0].Rows.Count); }
            else { totList.Add("0"); }

            return totList;
        }
        public DataSet GetTotalDaily(Credentials crd,string user_id,string stdate,string enddate)
        {
            ArrayList totList = new ArrayList();
            string sql = "Select * from deed_details where date_format(created_DTTM,'dd/MM/yyyy') between date_format('" + stdate + "','dd/MM/yyyy') and date_format('" + enddate + "','dd/MM/yyyy') and created_by = '" + user_id + "' order by created_dttm";
            DataSet ds = new DataSet();
            OdbcDataAdapter odap = new OdbcDataAdapter(sql, conn);
            odap.Fill(ds);
            
             return ds; 
            

            //sql = "Select district_code from index_of_name where date_format(created_DTTM,'dd/MM/yyyy')=date_format('" + stdate + "','dd/MM/yyyy') and created_by = '" + user_id + "'";
            //ds = new DataSet();
            //odap = new OdbcDataAdapter(sql, conn);
            //odap.Fill(ds);
            //if (ds.Tables.Count > 0)
            //{ totList.Add(ds.Tables[0].Rows.Count); }
            //else { totList.Add("0"); }

            //sql = "Select district_code from index_of_property where date_format(created_DTTM,'dd/MM/yyyy')=date_format('" + stdate + "','dd/MM/yyyy') and created_by = '" + user_id + "'";
            //ds = new DataSet();
            //odap = new OdbcDataAdapter(sql, conn);
            //odap.Fill(ds);
            //if (ds.Tables.Count > 0)
            //{ totList.Add(ds.Tables[0].Rows.Count); }
            //else { totList.Add("0"); }

            //sql = "Select district_code from deed_details where date_format(created_DTTM,'dd/MM/yyyy')=date_format('" + stdate + "','dd/MM/yyyy') and created_by = '" + user_id + "' and mismatch = 'Y'";
            //ds = new DataSet();
            //odap = new OdbcDataAdapter(sql, conn);
            //odap.Fill(ds);
            //if (ds.Tables.Count > 0)
            //{ totList.Add(ds.Tables[0].Rows.Count); }
            //else { totList.Add("0"); }
        }
        public AutoCompleteStringCollection GetSuggestions(string tblName, string fldName)
        {
            AutoCompleteStringCollection x = new AutoCompleteStringCollection();
            string sql = "Select distinct " + fldName + " from " + tblName;
            DataSet ds = new DataSet();
            OdbcDataAdapter odap = new OdbcDataAdapter(sql, conn);
            odap.Fill(ds);
            if (ds.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                     x.Add(ds.Tables[0].Rows[i][0].ToString().Trim());
                }
            }
            x.Add("Others");
            return x;
        }
        public AutoCompleteStringCollection GetFilteredSuggestions(string tblName, string fldName, string fldFilterName, string fldFIlterValue)
        {
            AutoCompleteStringCollection x = new AutoCompleteStringCollection();
            string sql = "Select distinct " + fldName + " from " + tblName + " where " + fldFilterName.Trim() + " = '" + fldFIlterValue.Trim() + "'";
            DataSet ds = new DataSet();
            OdbcDataAdapter odap = new OdbcDataAdapter(sql, conn);
            odap.Fill(ds);
            if (ds.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    x.Add(ds.Tables[0].Rows[i][0].ToString().Trim());
                }
                x.Add("Others");
            }
            return x;
        }
        */
    }
    
#endregion 
}
