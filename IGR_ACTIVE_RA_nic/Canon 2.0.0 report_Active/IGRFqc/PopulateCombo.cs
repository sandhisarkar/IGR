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

namespace IGRFqc
{
    public class PopulateCombo
    {
        Credentials crd = new Credentials();
        OdbcConnection conn = new OdbcConnection();
        OdbcTransaction txn;
        public PopulateCombo(OdbcConnection pCon,OdbcTransaction pTxn, Credentials prmCrd)
        {
            conn = pCon;
            crd = prmCrd;
            txn = pTxn;
        }
        public DataSet GetDocType()
        {
            string sql = "Select distinct doc_type,doc_name from tbldoc_type";
            DataSet ds = new DataSet();
            OdbcCommand cmd = new OdbcCommand(sql, conn, txn);
            OdbcDataAdapter odap = new OdbcDataAdapter(cmd);
            odap.Fill(ds);
            return ds;
        }
        public string GetUUID()
        {
            string uuid = string.Empty;
            string sql = "Select uuid()";
            DataSet ds = new DataSet();
            OdbcCommand cmd = new OdbcCommand(sql, conn,txn);
            OdbcDataAdapter odap = new OdbcDataAdapter(cmd);
            odap.Fill(ds);
            if (ds.Tables[0].Rows.Count > 0)
            {
                uuid = ds.Tables[0].Rows[0][0].ToString();
            }
            return uuid;

        }
        public DataSet GetDistrict_Active()
        {
            string sql = "Select distinct district_code,trim(district_name) as district_name from district where active = 'Y'";
            DataSet ds = new DataSet();
            OdbcCommand cmd = new OdbcCommand(sql, conn, txn);
            OdbcDataAdapter odap = new OdbcDataAdapter(cmd);
            odap.Fill(ds);
            return ds;
        }
        public DataSet GetCountry()
        {
            string sql = "select cou_code,cou_name from country_master order by cou_name";
            DataSet ds = new DataSet();
            OdbcCommand cmd = new OdbcCommand(sql, conn, txn);
            OdbcDataAdapter odap = new OdbcDataAdapter(cmd);
            odap.Fill(ds);
            return ds;
        }
        public DataSet GetState(string con_code)
        {
            string sql = "select state_code,state_name from state_master where con_code = '" + con_code + "' order by state_name";
            DataSet ds = new DataSet();
            OdbcCommand cmd = new OdbcCommand(sql, conn, txn);
            OdbcDataAdapter odap = new OdbcDataAdapter(cmd);
            odap.Fill(ds);
            return ds;
        }
        public DataSet GetDistrictoutsideWB(string con_code,string state_code)
        {
            string sql = "select dis_code,dis_name from district_master where con_code = '"+con_code+"' and state_code = '"+state_code+"' order by dis_name";
            DataSet ds = new DataSet();
            OdbcCommand cmd = new OdbcCommand(sql, conn, txn);
            OdbcDataAdapter odap = new OdbcDataAdapter(cmd);
            odap.Fill(ds);
            return ds;
        }
        public DataSet GetException(string excep)
        {
            string sql = "select ExcepTion_Name from tblexception where ExcepTion_Code = '"+excep+"'";
            DataSet ds = new DataSet();
            OdbcCommand cmd = new OdbcCommand(sql, conn, txn);
            OdbcDataAdapter odap = new OdbcDataAdapter(cmd);
            odap.Fill(ds);
            return ds;
        }

        public DataSet GetROffice(string districtCode)
        {
            string sql = "Select RO_code,trim(RO_name) as RO_name from RO_MASTER where district_code='" + districtCode + "'";
            DataSet ds = new DataSet();
            OdbcCommand cmd = new OdbcCommand(sql, conn, txn);
            OdbcDataAdapter odap = new OdbcDataAdapter(cmd);
            odap.Fill(ds);
            return ds;
        }
        //public DataSet GetROffice_Active(string districtCode)
        //{
        //    string sql = "Select RO_code,trim(RO_name) as RO_name from RO_MASTER where district_code='" + districtCode + "' and active = 'Y'";
        //    DataSet ds = new DataSet();
        //    OdbcDataAdapter odap = new OdbcDataAdapter(sql, conn);
        //    odap.Fill(ds);
        //    return ds;
        //}
        public DataSet GetRoad(string districtCode, string RoCode)
        {
            string sql = "Select road_code,trim(road_name) as road_name from road_corporation where district_code='" + districtCode + "' and ro_code='" + RoCode + "' order by road_name";
            DataSet ds = new DataSet();
            OdbcCommand cmd = new OdbcCommand(sql, conn, txn);
            OdbcDataAdapter odap = new OdbcDataAdapter(cmd);
            odap.Fill(ds);
            return ds;
        }
        public DataSet GetBookType()
        {
            string sql = "select value_book,trim(key_book) as key_book from tbl_book order by value_book";
            DataSet ds = new DataSet();
            OdbcCommand cmd = new OdbcCommand(sql, conn, txn);
            OdbcDataAdapter odap = new OdbcDataAdapter(cmd);
            odap.Fill(ds);
            return ds;
        }
        public DataTable GetKeyBook(string Bvalue)
        {
            string sql = "select key_book from tbl_book where value_book='"+Bvalue+"'";
            DataTable ds = new DataTable();
            OdbcCommand cmd = new OdbcCommand(sql, conn, txn);
            OdbcDataAdapter odap = new OdbcDataAdapter(cmd);
            odap.Fill(ds);
            return ds;
        }
        public DataSet GetYear()
        {
            string sql = "Select distinct deed_year from deed_details";
            DataSet ds = new DataSet();
            OdbcCommand cmd = new OdbcCommand(sql, conn, txn);
            OdbcDataAdapter odap = new OdbcDataAdapter(cmd);
            odap.Fill(ds);
            return ds;
        }
        public DataSet GetDeedgroupbyNo()
        {
            string sql = "select deed_year,book,volume_no,count(*) as No_of_Deeds from deed_details group by deed_year,book,volume_no order by Convert(volume_no,UNSIGNED INTEGER)";
            DataSet ds = new DataSet();
            OdbcCommand cmd = new OdbcCommand(sql, conn, txn);
            OdbcDataAdapter odap = new OdbcDataAdapter(cmd);
            odap.Fill(ds);
            return ds;
        }
        public DataSet GetDeedgroupbyNo(string dis, string ro, string book, string deedyear,string vol)
        {
            string sql = "select deed_year,book,volume_no,count(*) as No_of_Deeds from deed_details where district_code ='" + dis + "' and ro_code = '" + ro + "'and book = '" + book + "'and deed_year = '" + deedyear + "' and volume_no = '" + vol + "' group by deed_year,book,volume_no order by Convert(volume_no,UNSIGNED INTEGER)";
            DataSet ds = new DataSet();
            OdbcCommand cmd = new OdbcCommand(sql, conn, txn);
            OdbcDataAdapter odap = new OdbcDataAdapter(cmd);
            odap.Fill(ds);
            return ds;
        }
        public DataSet GetVol(string dis, string ro, string book, string deedyear, bool pReexport)
        {
            string sql = string.Empty;
            if (pReexport == false)
            {
                sql = "select distinct Volume_No from deed_details where District_Code = '" + dis + "' and RO_Code = '" + ro + "' and Book = '" + book + "' and Deed_year = '" + deedyear + "' and exported <> 'Y' order by Convert(volume_no,UNSIGNED INTEGER)";
            }
            else
            {
                sql = "select distinct Volume_No from deed_details where District_Code = '" + dis + "' and RO_Code = '" + ro + "' and Book = '" + book + "' and Deed_year = '" + deedyear + "' order by Convert(volume_no,UNSIGNED INTEGER)";
            }
            DataSet ds = new DataSet();
            OdbcCommand cmd = new OdbcCommand(sql, conn, txn);
            OdbcDataAdapter odap = new OdbcDataAdapter(cmd);
            odap.Fill(ds);
            return ds;
        }
        public DataSet GetDeedDetails(string district, string ro_code, string book, string year,string vol)
        {
            //string sql = "select distinct a.district_code as District_Code, b.district_name as District_Name, a.Ro_Code as RO_Code, c.ro_name as RO_Name, a.deed_year as Year, a.book as Book, a.volume_no as Volume, count(*) as Nos from deed_details a, district b, ro_master c where a.district_code=b.district_code and a.District_Code=c.district_code and a.RO_Code=c.ro_code group by a.District_Code, a.RO_Code, a.deed_year, a.book, a.volume_no where district_code ='" + district + "' and ro_code = '" + ro_code + "'and book = '" + book + "'and deed_year = '" + year + "' and volume_no = '"+vol+"'";
            string sql = "Select District_Code,RO_Code,Book,Deed_year,Deed_no,Serial_No,Serial_Year,tran_maj_code,tran_min_code,Volume_No,Page_From,Page_To,date_format(Date_of_Completion,'%d/%m/%Y'),date_format(Date_of_Delivery,'%d/%m/%Y'),Deed_Remarks,Created_DTTM,Exported,created_by,modified_by,modified_dttm,MisMatch,Scan_doc_type from deed_details where district_code ='" + district + "' and ro_code = '" + ro_code + "'and book = '" + book + "'and deed_year = '" + year + "' and volume_no = '"+vol+"'";
            DataSet ds = new DataSet();
            OdbcCommand cmd = new OdbcCommand(sql, conn, txn);
            OdbcDataAdapter odap = new OdbcDataAdapter(cmd);
            odap.Fill(ds);
            return ds;
        }
        public DataSet GetTransactionTypeMajor(string bookNumber)
        {
            string sql = "Select tran_maj_code,trim(tran_maj_name) as tran_maj_name from party where book_no='" + bookNumber + "'";
            DataSet ds = new DataSet();
            OdbcCommand cmd = new OdbcCommand(sql, conn, txn);
            OdbcDataAdapter odap = new OdbcDataAdapter(cmd);
            odap.Fill(ds);
            return ds;
        }
        public DataSet GetTransactionTypeMinor(string tranMajCode)
        {
            string sql = "Select tran_min_code,trim(tran_name) as tran_name from tranlist_code where tran_maj_code='" + tranMajCode + "'";
            DataSet ds = new DataSet();
            OdbcCommand cmd = new OdbcCommand(sql, conn, txn);
            OdbcDataAdapter odap = new OdbcDataAdapter(cmd);
            odap.Fill(ds);
            return ds;
        }
        public DataSet GetPartyCode()
        {
            string sql = "Select ec_code,trim(ec_name) as ec_name from Party_code order by ec_name";
            DataSet ds = new DataSet();
            OdbcCommand cmd = new OdbcCommand(sql, conn, txn);
            OdbcDataAdapter odap = new OdbcDataAdapter(cmd);
            odap.Fill(ds);
            return ds;
        }
        public DataSet GetOccupation()
        {
            string sql = "Select occupation_code,trim(occupation_name) as occupation_name from occupation order by occupation_name";
            DataSet ds = new DataSet();
            OdbcCommand cmd = new OdbcCommand(sql, conn, txn);
            OdbcDataAdapter odap = new OdbcDataAdapter(cmd);
            odap.Fill(ds);
            return ds;
        }
        public DataSet GetReligion()
        {
            string sql = "Select religion_code,trim(religion_name) as religion_name from religion";
            DataSet ds = new DataSet();
            OdbcCommand cmd = new OdbcCommand(sql, conn, txn);
            OdbcDataAdapter odap = new OdbcDataAdapter(cmd);
            odap.Fill(ds);
            return ds;
        }
        public DataSet GetDistrict()
        {
            string sql = "Select distinct district_code,trim(district_name) as district_name from district";
            DataSet ds = new DataSet();
            OdbcCommand cmd = new OdbcCommand(sql, conn, txn);
            OdbcDataAdapter odap = new OdbcDataAdapter(cmd);
            odap.Fill(ds);
            return ds;
        }
        public bool _UpdateDeedforWB(DeedControl pDd, string outsideWB)
        {

            bool retVal = false;
            string sql = string.Empty;

            sql = "UPDATE deed_details SET Outside_wb = '" + outsideWB + "' ";
                    
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
        public string GetDistrictName(string pDistrictCode)
        {
            string sql = "Select distinct district_code,trim(district_name) as district_name from district where district_code='"+pDistrictCode+"'";
            DataSet ds = new DataSet();
            OdbcCommand cmd = new OdbcCommand(sql, conn, txn);
            OdbcDataAdapter odap = new OdbcDataAdapter(cmd);
            odap.Fill(ds);
            return ds.Tables[0].Rows[0][1].ToString();
        }
        public string GetCountryName(string pCountryCode)
        {
            string sql = "Select distinct cou_code,trim(cou_name) as cou_name from country_master where cou_code='" + pCountryCode + "'";
            DataSet ds = new DataSet();
            OdbcCommand cmd = new OdbcCommand(sql, conn, txn);
            OdbcDataAdapter odap = new OdbcDataAdapter(cmd);
            odap.Fill(ds);
            return ds.Tables[0].Rows[0][1].ToString();
        }
        public string GetStateNameWB(string pCountryCode,string pStateCode)
        {
            string sql = "Select distinct state_code,trim(state_name) as state_name from state_master where con_code='"+pCountryCode+"' and state_code = '"+pStateCode+"'";
            DataSet ds = new DataSet();
            OdbcCommand cmd = new OdbcCommand(sql, conn, txn);
            OdbcDataAdapter odap = new OdbcDataAdapter(cmd);
            odap.Fill(ds);
            return ds.Tables[0].Rows[0][1].ToString();
        }
        public string GetDistrictNameWB(string pCountryCode, string pStateCode,string pDistrictCode)
        {
            string sql = "Select distinct dis_code,trim(dis_name) as dis_name from district_master where con_code='"+pCountryCode +"' and state_code = '"+pStateCode +"' and dis_code = '"+pDistrictCode +"'";
            DataSet ds = new DataSet();
            OdbcCommand cmd = new OdbcCommand(sql, conn, txn);
            OdbcDataAdapter odap = new OdbcDataAdapter(cmd);
            odap.Fill(ds);
            return ds.Tables[0].Rows[0][1].ToString();
        }
        public DataSet GetPS(string districtCode)
        {
            string sql = "Select PS_code,trim(PS_name) as PS_name from ps where district_code='" + districtCode + "' order by PS_name";
            DataSet ds = new DataSet();
            OdbcCommand cmd = new OdbcCommand(sql, conn, txn);
            OdbcDataAdapter odap = new OdbcDataAdapter(cmd);
            odap.Fill(ds);
            return ds;
        }
        public string GetPSName(string districtCode,string psCode)
        {
            string sql = "Select PS_code,trim(PS_name) as PS_name from ps where district_code='" + districtCode + "' and ps_code='"+psCode+"' order by PS_name";
            DataSet ds = new DataSet();
            OdbcCommand cmd = new OdbcCommand(sql, conn, txn);
            OdbcDataAdapter odap = new OdbcDataAdapter(cmd);
            odap.Fill(ds);
            if (ds.Tables[0].Rows.Count > 0) { return ds.Tables[0].Rows[0][1].ToString(); }
            else {return string.Empty;}
        }
        public DataSet GetOther_PartyCode()
        {
            string sql = "select ec_code,trim(ec_name) as ec_name from party_code where DependentOn = 'Y'";
            DataSet ds = new DataSet();
            OdbcCommand cmd = new OdbcCommand(sql, conn, txn);
            OdbcDataAdapter odap = new OdbcDataAdapter(cmd);
            odap.Fill(ds);
            return ds;
        }
        public bool Party_dependency_Check(string party_code)
        {
            bool flag = false;
            string sql = "select HasDependency from party_code where ec_code = '" + party_code + "'";
            DataSet ds = new DataSet();
            OdbcCommand cmd = new OdbcCommand(sql, conn, txn);
            OdbcDataAdapter odap = new OdbcDataAdapter(cmd);
            odap.Fill(ds);
            if (ds.Tables[0].Rows.Count > 0)
            {
                if (ds.Tables[0].Rows[0][0].ToString() == "Y")
                {
                    flag = true;
                }
                else
                {
                    flag = false;
                }
            }
            return flag;
        }
        public bool Validate_user(string password)
        {
            string sql = "select user_pwd from ac_user where user_name = '" + crd.userName + "'";
            DataSet ds = new DataSet();
            OdbcCommand cmd = new OdbcCommand(sql, conn, txn);
            OdbcDataAdapter odap = new OdbcDataAdapter(cmd);
            odap.Fill(ds);
            if (ds.Tables[0].Rows[0][0].ToString() == password)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public DataSet GetMouza(string districtCode, string pPscode)
        {
            string sql = "Select moucode,trim(eng_mouname) as eng_mouname from moucode where district_code='" + districtCode + "' and ps_code='" + pPscode + "' order by eng_mouname";
            DataSet ds = new DataSet();
            OdbcCommand cmd = new OdbcCommand(sql, conn, txn);
            OdbcDataAdapter odap = new OdbcDataAdapter(cmd);
            odap.Fill(ds);
            return ds;
        }
        public string GetMouzaName(string districtCode, string pPscode,string pMouzaCode)
        {
            string sql = "Select moucode,trim(eng_mouname) as eng_mouname from moucode where district_code='" + districtCode + "' and ps_code='" + pPscode + "' and moucode='"+pMouzaCode+"' order by eng_mouname";
            DataSet ds = new DataSet();
            OdbcCommand cmd = new OdbcCommand(sql, conn, txn);
            OdbcDataAdapter odap = new OdbcDataAdapter(cmd);
            odap.Fill(ds);
            if (ds.Tables[0].Rows.Count > 0) { return ds.Tables[0].Rows[0][1].ToString(); }
            else { return string.Empty; }
        }
        public DataSet GetJL(string districtCode, string pPscode)
        {
            string sql = "Select distinct trim(jlno) as jlno from moucode where district_code='" + districtCode + "' and ps_code='" + pPscode + "' order by jlno";
            DataSet ds = new DataSet();
            OdbcCommand cmd = new OdbcCommand(sql, conn, txn);
            OdbcDataAdapter odap = new OdbcDataAdapter(cmd);
            odap.Fill(ds);
            ds.Tables[0].Rows.Add("Other");
            return ds;
        }
        public DataSet GetPropertyType()
        {
            string sql = "Select apartment_type_code,trim(description) as description from property_type order by description";
            DataSet ds = new DataSet();
            OdbcCommand cmd = new OdbcCommand(sql, conn, txn);
            OdbcDataAdapter odap = new OdbcDataAdapter(cmd);
            odap.Fill(ds);
            return ds;
        }
        public DataTable Getland_type(string do_code, string ro_code)
        {
            string sql = "select land_use,trim(eng_desc) as eng_desc from tbl_land_type where do_code = '" + do_code + "' and ro_code = '" + ro_code + "'";
            DataTable ds = new DataTable();
            OdbcCommand cmd = new OdbcCommand(sql, conn, txn);
            OdbcDataAdapter odap = new OdbcDataAdapter(cmd);
            odap.Fill(ds);
            return ds;
        }
        public DataSet GetGramPanchayet(string districtCode, string pscode)
        {
            string sql = "Select gp_code,trim(gp_desc) as gp_desc from gram_panchayat where district_code='" + districtCode + "' and ps_code='" + pscode + "' order by gp_desc";
            DataSet ds = new DataSet();
            OdbcCommand cmd = new OdbcCommand(sql, conn, txn);
            OdbcDataAdapter odap = new OdbcDataAdapter(cmd);
            odap.Fill(ds);
            return ds;
        }
        public DataSet GetSubdivision(string districtCode)
        {
            string sql = "Select municipality_code,trim(municipality_name) as municipality_name from municipality where district_code='" + districtCode + "' order by municipality_name";
            DataSet ds = new DataSet();
            OdbcCommand cmd = new OdbcCommand(sql, conn, txn);
            OdbcDataAdapter odap = new OdbcDataAdapter(cmd);
            odap.Fill(ds);
            return ds;
        }
        public string GetSubByCode(string pDisCode, string pCode)
        {
            string name = string.Empty;
            string sql = "Select trim(municipality_name) as municipality_name from municipality where district_code = '" + pDisCode + "' and municipality_code='" + pCode + "'";
            DataSet ds = new DataSet();
            OdbcCommand cmd = new OdbcCommand(sql, conn, txn);
            OdbcDataAdapter odap = new OdbcDataAdapter(cmd);
            odap.Fill(ds);
            if (ds.Tables.Count > 0)
            {
                if (ds.Tables[0].Rows.Count > 0)
                { name = ds.Tables[0].Rows[0][0].ToString(); }
            }
            return name;
        }
        public DataSet GetMappingSugg(string discode, string ps, string mouza, string jlno)
        {
            //string sql = "select b.district_name,b.ps_name,a.eng_mouname,a.jlno,b.district_code,b.ps_code,a.moucode  from moucode a, (select c.district_code, c.ps_code, d.district_name,c.ps_name from ps c, district d where c.district_code=d.district_code and d.district_code in (select district_code from district e where e.district_name like '%" + discode + "%') and c.ps_name like '%" + ps + "%') b where (a.district_code=b.district_code) and (a.ps_code=b.ps_code) and a.eng_mouname like '%" + mouza + "%'and a.jlno like '%" + jlno + "%'";
            string sql;
            if (string.IsNullOrEmpty(mouza) && string.IsNullOrEmpty(jlno))
            {
            	sql = "select b.district_name,b.ps_name,a.eng_mouname,a.jlno,b.district_code,b.ps_code,a.moucode from (select c.district_code, c.ps_code, d.district_name,c.ps_name from ps c, district d where c.district_code=d.district_code and d.district_code in (select district_code from district e where e.district_name like '%" + discode + "%') and c.ps_name like '%"+ ps +"%') b left join moucode a on (a.district_code=b.district_code) and (a.ps_code=b.ps_code)  and a.eng_mouname like '%"+ mouza +"%' and a.jlno like '%"+ jlno +"%'";
            }
            else
            {
            	sql = "select b.district_name,b.ps_name,a.eng_mouname,a.jlno,b.district_code,b.ps_code,a.moucode  from moucode a, (select c.district_code, c.ps_code, d.district_name,c.ps_name from ps c, district d where c.district_code=d.district_code and d.district_code in (select district_code from district e where e.district_name like '%" + discode + "%') and c.ps_name like '%" + ps + "%') b where (a.district_code=b.district_code) and (a.ps_code=b.ps_code) and a.eng_mouname like '%" + mouza + "%'and a.jlno like '%" + jlno + "%'";
            }
            DataSet ds = new DataSet();
            OdbcCommand cmd = new OdbcCommand(sql, conn, txn);
            OdbcDataAdapter odap = new OdbcDataAdapter(cmd);
            odap.Fill(ds);
            return ds;
        }
        public DataSet GetMappingSugg(string concode, string statecode, string districtcode)
        {
            
            string sql;
            {
                sql = "select c.cou_code,c.cou_name,a.state_code,a.state_name,b.dis_code,b.dis_name from state_master a,district_master b,country_master c where a.state_code = b.state_code and b.dis_name like '%" + districtcode + "%' and state_name like '%" + statecode + "%' and c.cou_code = '111' and a.con_code = c.cou_code";
                
            }
            DataSet ds = new DataSet();
            OdbcCommand cmd = new OdbcCommand(sql, conn, txn);
            OdbcDataAdapter odap = new OdbcDataAdapter(cmd);
            odap.Fill(ds);
            return ds;
        }
        public DataSet GetJLNobyDistrictPS(string pDistcode, string pPSCode)
        {
            string sql = "select jlno from moucode where district_code = '" + pDistcode + "' and ps_code = '" + pPSCode + "'";
            DataSet ds = new DataSet();
            OdbcCommand cmd = new OdbcCommand(sql, conn, txn);
            OdbcDataAdapter odap = new OdbcDataAdapter(cmd);
            odap.Fill(ds);
            return ds;
        }
        public AutoCompleteStringCollection GetSuggestions(string tblName, string fldName)
        {
            AutoCompleteStringCollection x = new AutoCompleteStringCollection();
            string sql = "Select distinct " + fldName + " from " + tblName;
            DataSet ds = new DataSet();
            OdbcCommand cmd = new OdbcCommand(sql, conn, txn);
            OdbcDataAdapter odap = new OdbcDataAdapter(cmd);
            odap.Fill(ds);
            if (ds.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    x.Add(ds.Tables[0].Rows[i][0].ToString().Trim());
                }
            }
            x.Add("Others");
            //x.Add("NA");
            return x;
        }
        public AutoCompleteStringCollection GetFilteredSuggestions(string tblName, string fldName, string fldFilterName, string fldFIlterValue)
        {
            AutoCompleteStringCollection x = new AutoCompleteStringCollection();
            string sql = "Select distinct " + fldName + " from " + tblName + " where " + fldFilterName.Trim() + " = '" + fldFIlterValue.Trim() + "'";
            DataSet ds = new DataSet();
            OdbcCommand cmd = new OdbcCommand(sql, conn, txn);
            OdbcDataAdapter odap = new OdbcDataAdapter(cmd);
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
        public DataSet GetDuplicateEntry(string pDist, string pRo, string pBook, string pDeed_yr, string pDeed_no)
        {
            string sql = "select district_code, ro_code, deed_year, book, deed_no, volume_no from deed_details where District_Code = '" + pDist + "' and ro_code = '" + pRo + "' and book = '" + pBook + "' and deed_year = '" + pDeed_yr + "' and deed_no = '" + pDeed_no + "'";
            DataSet ds = new DataSet();
            OdbcCommand cmd = new OdbcCommand(sql, conn, txn);
            OdbcDataAdapter odap = new OdbcDataAdapter(cmd);
            odap.Fill(ds);
            return ds;
        }
        public int GetMaxVal()
        {
            int count = 0;
            DataSet ds = new DataSet();
            string sql = "Select max(count) from tbl_Export_Count";
            OdbcCommand cmd = new OdbcCommand(sql, conn, txn);
            OdbcDataAdapter odap = new OdbcDataAdapter(cmd);
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
        public DataSet GetROffice_Active(string districtCode)
        {
            string sql = "Select RO_code,RO_name from RO_MASTER where district_code='" + districtCode + "' and active = 'Y'";
            DataSet ds = new DataSet();
            OdbcCommand cmd = new OdbcCommand(sql, conn, txn);
            OdbcDataAdapter odap = new OdbcDataAdapter(cmd);
            odap.Fill(ds);
            return ds;
        }
        public int checkPageSequence(string pagefrom, string pageto, string dis, string ro, string book, string deedno, string vol)
        {
            int fromPg = Convert.ToInt32(pagefrom);
            int toPg = Convert.ToInt32(pageto);
            int number = 0;
            string sql = "SELECT count(*) from deed_details where ((Convert(Page_From,signed integer) >= '" + fromPg + "' and Convert(Page_To,signed integer) <='" + toPg + "') or (Convert(Page_From,signed integer) <= '" + toPg + "' and Convert(Page_To,signed integer) >= '" + fromPg + "')) and District_Code = '" + dis + "' and ro_code = '" + ro + "' and book = '" + book + "' and Deed_year = '" + deedno + "' and volume_no = '" + vol + "'";
            DataSet ds = new DataSet();
            OdbcCommand cmd = new OdbcCommand(sql, conn, txn);
            OdbcDataAdapter odap = new OdbcDataAdapter(cmd);
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
        public DataSet checkPageSequencedetails(string pagefrom, string pageto, string dis, string ro, string book, string deedno, string vol, string deed_yr)
        {
            int fromPg = Convert.ToInt32(pagefrom);
            int toPg = Convert.ToInt32(pageto);
            string sql = "SELECT * from deed_details where ((Page_From >= '" + fromPg + "' and Page_To <='" + toPg + "') or (Page_From <= '" + toPg + "' and Page_To >= '" + fromPg + "')) and District_Code = '" + dis + "' and ro_code = '" + ro + "' and book = '" + book + "' and Deed_year = '" + deed_yr + "' and volume_no = '" + vol + "'";
            DataSet ds = new DataSet();
            OdbcCommand cmd = new OdbcCommand(sql, conn, txn);
            OdbcDataAdapter odap = new OdbcDataAdapter(cmd);
            odap.Fill(ds);
            return ds;
        }
    }

    }

    

