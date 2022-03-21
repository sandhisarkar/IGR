/*
 * Created by SharpDevelop.
 * User: USER
 * Date: 28/03/2017
 * Time: 8:49 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Linq;
using System.Collections.Generic;
using System.Globalization;
using System.Diagnostics;

namespace valUtils
{
	/// <summary>
	/// Description of DeedValidation.
	/// </summary>
	public class DeedValidation
	{
		public const string OK = "OK";
		public const string INERROR = "ERROR";
		public const string NOCOMMENTS = "";
		public static List<string> lstRelationships = new List<string>{"F", "M", "W", "H", "N", "S", "D", "R"};
		public static List<string> lstPlotKhatian = new List<string>{"LR", "RS", "CS"};
		public static List<string> lstPropertyTypes = new List<string>();
		public static readonly char[] SpecialChars = "!@#$%^&*()".ToCharArray();
		public DeedValidation()
		{
		}
		public static bool LoadMasters() {
			lstPropertyTypes = FileUtils.ReturnColumn(FileUtils.ReadLines(AppDomain.CurrentDomain.BaseDirectory + @"Masters\property_type.csv"), 0);
			return true;
		}
		public static Dictionary<string, string> DeedControlComponents(string full, bool include_item=false) {
			Dictionary<string, string> retVal = new Dictionary<string, string>();
			retVal["district_code"] = full.Substring(0, 2);
			retVal["ro_code"] = full.Substring(2, 2);
			retVal["book"] = full.Substring(4, 1);
			retVal["deed_year"] = full.Substring(5, 4);
			if (full.Length < 14) {
				retVal["deed_no"] = full.Substring(9, (full.Length - 9));
				if (include_item) {
					retVal["item_no"] = full.Substring(full.Length - 1);
				}
			}
			else {
				if (include_item) {
//					retVal["deed_no"] = full.Substring(9, 5);
//					retVal["item_no"] = full.Substring(14, 1);

					string cut = full.Substring(9);
					retVal["deed_no"] = cut.Substring(0, (cut.Length - 1));
					retVal["item_no"] = cut.Substring(cut.Length - 1);
					
				}
				else {
					retVal["deed_no"] = full.Substring(9, 5);
				}
			}
			return retVal;
		}
		public static string CreateDeedNoFromDictionary(Dictionary<string, string> l){
			string retVal = l["district_code"] + l["ro_code"] + l["book"] + l["deed_year"] + l["deed_no"];;
			return retVal;
		}
		public static string CreateDeedNoFromDictionaryWithItem(Dictionary<string, string> l) {
			string retVal = l["district_code"] + l["ro_code"] + l["book"] + l["deed_year"] + l["deed_no"] + l["item_no"];
			return retVal;
		}
		public static string LocateDeed(Dictionary<string, string> l) {
			if (l["item_no"] == null) {
				
			}
			return l["district_code"] + l["ro_code"] + l["book"] + l["deed_year"] + l["deed_no"];
		}
		public static string AggreGateData(Dictionary<string, string> data) {
			Dictionary <string, Dictionary<string, string>> ret = ValidateRow(data);
			string strControl = data["district_code"] + data["ro_code"] + data["book"] + data["deed_year"] + data["deed_no"];
			string retVal = string.Empty;
			foreach (KeyValuePair <string, Dictionary<string, string>> kv in ret) {
				foreach (KeyValuePair<string, string> kvL1 in kv.Value) {
					if (kvL1.Value != "") {
						retVal += kv.Key;
						retVal += ":" + kvL1.Key + "=" + kvL1.Value + ",";
					}
					
				}
			}
			if (retVal.Length > 0) {
				return retVal;
			}
			else {
				return OK;
			}
		}
		public static Dictionary<string, Dictionary<string, string>> ValidateRow(Dictionary<string, string> data) {
			Dictionary<string, Dictionary<string, string>> retDic = new Dictionary<string, Dictionary<string, string>>();
			foreach (KeyValuePair<string, string> kv in data) {
				string ret = Convert.ToString(GetFunc(kv.Key)(kv.Value));
				Dictionary<string, string> tmpStore = new Dictionary<string, string>();
				tmpStore.Add(kv.Value, ret);
				retDic.Add(kv.Key, tmpStore);
			}
			return retDic;
		}


		public static Func<string, string> GetFunc(string Field) {
			switch (Field.ToLower())
			{
					//General cases for all tables
				case "district_code":
					return ChkDistCode;
				case "ro_code":
					return ChkDistCode;
				case "book":
					return ChkBook;
				case "deed_year":
					return ChkDeedYear;
				case "deed_no":
					return ChkDeedNo;
					//End: General cases for all tables
					//Cases for Deed Details
				case "serial_no":
					return ChkDeedNo;
				case "serial_year":
					return ChkSerialYear;
				case "tran_maj_code":
					return ChkTranMajCode;
				case "tran_min_code":
					return ChkTranMinCode;
				case "volume_no":
					return ChkVolumeNo;
				case "page_from":
					return ChkPageFrom;
				case "page_to":
					return ChkPageFrom;
				case "date_of_completion":
					return ChkDateOrBlank;
				case "date_of_delivery":
					return ChkDateOrBlank;
				case "deed_remarks":
					return ChkDeedRemarks;
				case "scan_doc_type":
					return ListValidate(new List<string>{"C", "H", "X"});
					//End: Cases for Deed Details
					//Cases for Index of Name
				case "item_no":
					return ChkNo;
				case "initial_name":
					return ListValidateEmptyAllowed(new List<string>{"Mr.","Mrs.", "Ms."});
				case "first_name":
					return ChkName;
				case "last_name":
					return ChkEmptyOrMax50Digits;
				case "party_code":
					return ChkNo;
				case "admit_code":
					return ChkEmptyOrNumber;
				case "address":
					return ChkAddress;
				case "address_district_code":
					return ChkEmptyOrMax50Digits;
				case "address_district_name":
					return ChkAddressDistrictName;
				case "address_ps_code":
					return ChkNumeric2DigitOrEmpty;
				case "address_ps_name":
					return ChkAddressPSName;
				case "father_mother":
					return ListValidate(lstRelationships);
				case "rel_code":
					return ListValidate(lstRelationships);
				case "occupation_code":
					return ChkNo;
				case "religion_code":
					return ChkEmptyOrNumber;
				case "more":
					return ListValidate(new List<string>{"Y", "N"});
				case "pin":
					return ChkNumeric6DigitOrEmpty;
				case "city":
					return ChkEmptyOrMax50Digits;
				case "other_party_code":
					return ChkEmptyOrNumber;
					//End: Cases for Index of Name
					//Cases for Index of Property
				case "property_district_code":
					return ChkNumeric2Digit;
				case "property_district_name":
					return ChkEmptyOrMax50Digits;
				case "property_ro_code":
					return ChkNumeric2Digit;
				case "ps_code":
					return ChkEmptyOrNumber;
				case "ps_name":
					return ChkEmptyOrMax50Digits;
				case "moucode":
					return ChkEmptyOrMax3DigitsNOspclChar;
				case "mouja":
					return ChkEmptyOrMax50Digits;
				case "area_type":
					return ListValidateEmptyAllowed(new List<string>{"M", "G", "C"});
				case "gp_muni_corp_code":
					return ChkEmptyOrMax3DigitsNOspclChar;
				case "gp_muni_name":
					return ChkEmptyOrMax50Digits;
				case "ward":
					return ChkEmptyOrNumber;
				case "holding":
					return ChkEmptyOrMax50Digits;
				case "premises":
					return ChkEmptyOrMax50Digits;
				case "road_code":
					return ChkEmptyOrNumber;
				case "plot_code_type":
					return ListValidateEmptyAllowed(lstPlotKhatian);
				case "road":
					return ChkEmptyOrMax50Digits;
				case "plot_no":
					return ChkPlotNumber;
				case "bata_no":
					return ChkNumberWithSymbols;
				case "khatian_type":
					return ListValidateEmptyAllowed(lstPlotKhatian);
				case "khatian_no":
					return ChkEmptyOrNumber;
				case "bata_khatian_no":
					return ChkEmptyOrNumber;
				case "property_type":
					return ListValidateEmptyAllowed(lstPropertyTypes);
				case "land_area_acre":
					return ChkEmptyOrDecimal;
				case "land_area_bigha":
					return ChkEmptyOrDecimal;
				case "land_area_decimal":
					return ChkEmptyOrDecimal;
				case "land_area_katha":
					return ChkEmptyOrDecimal;
				case "land_area_chatak":
					return ChkEmptyOrDecimal;
				case "land_area_sqfeet":
					return ChkEmptyOrDecimal;
				case "structure_area_in_sqfeet":
					return ChkEmptyOrDecimal;
				case "ref_ps":
					return ChkEmptyOrMax50Digits;
				case "ref_mouza":
					return ChkEmptyOrMax50Digits;
				case "jl_no":
					return ChkEmptyOrNumberOrAlphaNumeric;
				case "other_plots":
					return ChkNumberWithSymbols;
				case "other_khatian":
					return ChkNumberWithSymbols;
				case "land_type":
					return ChkEmptyOrMax50Digits;
				case "refjl_no":
					return ChkEmptyOrNumber;
					//End: Cases for Index of Property
					//Cases for Other Plots
					
					//End: Cases for Other Plots
					//Cases for Other Khatians
					
					//End: Cases for Other Khatians
					
				default:
					return BlankOK;
			}
			
		}
		public static string BlankOK(string s) {
			return NOCOMMENTS;
		}
		public static Func<string, string> ListValidate(List<string> initVal) {
			List<string> iVal = initVal;
			Func<string, string> ValidateMethod = (string fld) =>
			{
				return (Within(fld, iVal));
			};
			return ValidateMethod;
		}
		public static Func<string, string> ListValidateEmptyAllowed(List<string> initVal) {
			List<string> iVal = initVal;
			Func<string, string> ValidateMethod = (string fld) =>
			{
				if (fld.Trim() == string.Empty) {
					return NOCOMMENTS;
				}
				else {
					return (Within(fld, iVal));
				}
			};
			return ValidateMethod;
		}
		public static string ChkPlotNumber(string fld)
		{
//			if(!IsNumericReturnBOOL(dic.Values["plot_no"].Trim()))
//			{
//				retVal += "~plot_no contains characters~";
//			}
//			if(fld.Equals("RS"))
//			{
//				int r = 9;
//			}
			return (ChkEmptyOrNumber(fld));
		}
		public static string ChkEmptyOrNumber(string fld) {
			if (fld.Trim() == string.Empty) {
				return NOCOMMENTS;
			}
			else {
				return IsNumeric(fld);
			}
		}
		public static string ChkEmptyOrNumberOrAlphaNumeric(string fld) {
			if (fld.Trim() == string.Empty) {
				return NOCOMMENTS;
			}
			else {
//				return IsNumeric(fld);
				int cnt = fld.ToList()
					.Select(IsEnglishOrNumber)
					.ToList()
					.Where(b => b==false)
					.Count();
				if (cnt > 0) {
					return "~Not a number or character~";
				}
				else{
					return NOCOMMENTS;
				}
			}
		}
		public static string ChkEmptyOrDecimal(string fld) {
			if (fld.Trim() == string.Empty) {
				return NOCOMMENTS;
			}
			else {
				return IsDecimal(fld);
			}
		}
		
		public static string ChkEmptyOrMax50Digits(string fld) {
			if (fld.Trim() == string.Empty) {
				return NOCOMMENTS;
			}
			else {
				return ChkMaxLen(fld, 50);
			}
		}

		public static string ChkEmptyOrMax2Digits(string fld) {
			if (fld.Trim() == string.Empty) {
				return NOCOMMENTS;
			}
			else {
				return ChkMaxLen(fld, 2);
			}
		}
		
		public static string ChkEmptyOrMax3DigitsNOspclChar(string fld) {
//			int indexOf = fld.IndexOfAny(SpecialChars);
			if (fld.Trim() == string.Empty) {
				return NOCOMMENTS;
			}
//			else if(indexOf != -1){
//				return "~Special charecter is not allowed in MOUZA code~";
//			}
			else {
				
				int cnt = fld.ToList()
					.Select(IsEnglishOrNumber)
					.ToList()
					.Where(b => b==false)
					.Count();
				if (cnt > 0) {
					return "~Not a number or character~";
				}
				else{
					return ChkMaxLen(fld, 3);
				}
			}
		}
		
		public static bool ChkAlphaNumericBOOL(string fld)   //added by arpan
		{
			fld = fld.ToUpper();
			int cnt = fld.ToList()
				.Select(IsEnglishInUpper)
				.ToList()
				.Where(b => b==true)
				.Count();
			if (cnt > 0) {
				return true;
			}
			else{
				return false;
			}
		}
		
		public static string ChkAddressPSName(string fld) {
			if (fld.Trim() == string.Empty) {
				return NOCOMMENTS;
			}
			else {
				return ChkMaxLen(fld, 50);
			}
		}
		public static string ChkAddressDistrictName(string fld) {
			if (fld.Trim() == string.Empty) {
				return NOCOMMENTS;
			}
			else {
				return ChkMaxLen(fld, 50);
			}
		}
		public static string ChkNumeric2Digit(string fld) {
			return (CheckLen(fld, 2) + IsNumeric(fld));
		}
		public static string ChkNumeric2DigitOrEmpty(string fld) {
			if (fld.Trim() == string.Empty) {
				return NOCOMMENTS;
			}
			else {
				return (CheckLen(fld, 2) + IsNumeric(fld));
			}
		}
		public static string ChkNumeric6DigitOrEmpty(string fld) {
			string retVal = string.Empty;
			if (fld.Trim() == string.Empty) {
				retVal = NOCOMMENTS;
			}
			else {
				retVal = (CheckLenForPIN(fld) + IsNumericForPIN(fld));
			}
			
			if (retVal.Length > 0) {
				System.Diagnostics.Debug.Print("Testing PIN");
			}
			return retVal;
		}

		public static string ChkName(string fld) {
			return (NotBlank(fld));
		}
		public static string ChkNo(string fld) {
			return (IsNumeric(fld));
		}
		public static string ChkAddress(string fld) {
			return (ChkMaxLen(fld, 255));
		}
		public static string ChkDeedRemarks(string fld) {
			return (ChkMaxLen(fld, 200));
		}
		public static string ChkDateOrBlank(string fld) {
			if (fld.Trim() == string.Empty) {
				return NOCOMMENTS;
			}
			else {
				return ChkDate(fld);
			}
		}
		public static string ChkDate(string fld) {
			DateTime parsed;
			if (DateTime.TryParseExact(fld, "yyyy/MM/dd",
			                           CultureInfo.InvariantCulture,
			                           DateTimeStyles.None,
			                           out parsed)) {
				return NOCOMMENTS;
			}
			else {
				return "Error in date or date format";
			}
		}
		public static string ChkPageFrom(string fld) {
			return (IsNumeric(fld));
		}
		public static string ChkPageTo(string fld) {
			return (IsNumeric(fld));
		}
		
		public static string ChkTranMajCode(string fld) {
			return (CheckLen(fld, 2) + IsNumeric(fld));
		}
		public static string ChkTranMinCode(string fld) {
			return (CheckLen(fld, 2) + IsNumeric(fld));
		}
		public static string ChkSerialNo(string fld) {
			return (CheckLen(fld, 5) + IsNumeric(fld));
		}
		public static string ChkSerialYear(string fld) {
			return (CheckLen(fld, 4) + IsNumeric(fld) + Within(fld,
			                                                   Enumerable.Range(1985, 2010)
			                                                   .ToList()
			                                                   .Select(l => Convert.ToString(l))
			                                                   .ToList()));
		}
		public static string ChkDistCode(string fld) {
			return (CheckLen(fld, 2) + IsNumeric(fld));
		}
//		public static string ChkDeedNo(string fld) {
//			return (CheckLen(fld, 5) + IsNumeric(fld));
//		}
		public static string ChkBook(string fld) {
			return (CheckLen(fld, 1) + IsNumeric(fld) + Within(fld, new List<string>(new string[]{"1", "2", "3", "4", "5"})));
		}
		public static string ChkDeedYear(string fld) {
			return (CheckLen(fld, 4) + IsNumeric(fld) + Within(fld,
			                                                   Enumerable.Range(1985, 2010)
			                                                   .ToList()
			                                                   .Select(l => Convert.ToString(l))
			                                                   .ToList()));
		}
		public static string CheckLen(string fld, int ln) {
			if (fld.Trim().Length == ln) {
				return NOCOMMENTS;
			}
			else {
				return "~Length not of " + Convert.ToString(ln) + " digits~";
			}
		}
		public static string CheckLenForPIN(string fld) {
			if(fld.Equals("7000[8"))
			{
				int e = 0;
			}
			if(fld.Trim().Contains(" "))
			{
				return "~Pin code contains space~";
			}
			if (fld.Trim().Length == 0) {
				return NOCOMMENTS;
			}
			else if (fld.Trim().Length == 2) {
				return NOCOMMENTS;
			}
//			else if (fld.Trim().Length == 3) {
//				return NOCOMMENTS;
//			}
			else if (fld.Trim().Length == 6) {
				return NOCOMMENTS;
			}
			else {
				return "~Length not of 2 or 6"  + " digits~";
			}
		}
		public static string ChkMaxLen(string fld, int ln) {
			if (fld.Trim().Length <= ln) {
				return NOCOMMENTS;
			}
			else {
				return "~Length greater than " + Convert.ToString(ln) + " digits~";
			}
		}
		public static string IsNumeric(string fld) {
			int n;
			if (int.TryParse(fld, out n)) {
				return NOCOMMENTS;
			}
			else {
				return "~Not a number~";
			}
		}
		public static string IsNumericorBlank(string fld) {
			int n;
			if (int.TryParse(fld, out n)) {
				return NOCOMMENTS;
			}
			if(fld == String.Empty)
			{
				return NOCOMMENTS;
			}
			else {
				return "~Not a number~";
			}
		}
		public static bool IsNumericReturnBOOL(string fld) {   //created by arpan
			int n;
			if (int.TryParse(fld, out n)) {
				return true;
			}
			else {
				return false;
			}
		}
		public static string IsNumericForPIN(string fld) {
			int n;
			if (int.TryParse(fld, out n)) {
				if(n > 0 && !fld.Substring(0,1).Equals("0"))
				{
					return NOCOMMENTS;
				}
				if(fld.Trim().Equals("000000"))
				{
					return NOCOMMENTS;
				}
				else
				{
					return "~Not a valid pin number~";
				}
			}
			else {
				return "~Not a number~";
			}
		}
		public static string IsDecimal(string fld) {
			decimal n;
			if (decimal.TryParse(fld, out n)) {
				return NOCOMMENTS;
			}
			else {
				return "~Not a decimal~";
			}
		}
		public static string ChkRange(string fld, int min, int max)
		{
			if(fld.Trim().Length >= min && fld.Trim().Length <= max)
			{
				return NOCOMMENTS;
			}
			else
			{
				return "~Length Error~";
			}
		}
		
		public static bool ChkRangeBool(string fld, int min, int max)
		{
			if(fld.Trim().Length >= min && fld.Trim().Length <= max)
			{
				return true;
			}
			else
			{
				return false;
			}
		}
		
		public static string ChkDeedNo(string c)
		{
//			string length_msg = ChkRange(c,5,6);
//			if(length_msg.Equals(NOCOMMENTS))
//			{
			int cnt = c.ToList()
				.Select(IsEnglishOrNumber)
				.ToList()
				.Where(b => b==false)
				.Count();
			if (cnt > 0) {
				return "~Not a number or character~";
			}
			else {
				int cntofChars = c.ToList()
					.Select(IsEnglishInUpper)
					.ToList()
					.Where(b => b==true)
					.Count();
				if (cntofChars > 1) {
					return "~More than 1 Alphabet Present~";
				}
				//#if BzerUse
				if (cntofChars == 1) {
					return "~1 Alphabet Present~";
				}
				//#endif
				return NOCOMMENTS;
			}
//			}
//			else
//			{
//				return length_msg;
//			}
		}
		public static string ChkVolumeNo(string fld) {
			int cnt = fld.ToList()
				.Select(IsEnglishOrNumber)
				.ToList()
				.Where(b => b==false)
				.Count();
			if (cnt > 0) {
				return "~Not a number or character~";
			}
			else {
				int cntofChars = fld.ToList()
					.Select(IsEnglishInUpper)
					.ToList()
					.Where(b => b==true)
					.Count();
				if (cntofChars > 1) {
					return "~More than 1 Alphabet Present~";
				}
				return NOCOMMENTS;
			}
		}
		public static bool IsEnglishOrNumber(char c)
		{
			return (c>='A' && c<='Z') || (c>='a' && c<='z') || (c>='0' && c<='9');
		}
		public static bool IsEnglishInUpper(char c)
		{
			return (c>='A' && c<='Z');
		}
		public static string ChkNumberWithSymbols(string c)
		{
			int cnt = c.ToList()
				.Select(IsNumberWithSlash)
				.ToList()
				.Where(b => b==false)
				.Count();
			if (cnt > 0) {
				return "~Not a number and symbol~";
			}
			else {
				return NOCOMMENTS;
			}
		}
		
		public static bool IsNumberWithSlash(char c)
		{
			return (c>='A' && c<='Z') || (c>='0' && c<='9') || (c>='/');
		}
		public static string NotBlank(string fld) {
			if (fld.Trim().Length > 0) {
				return NOCOMMENTS;
			}
			else {
				return "~Cannot be blank~";
			}
		}
		public static string Within(string fld, List<string> master) {
			if (master.Contains(fld)) {
				return NOCOMMENTS;
			}
			else {
				return "~Not in master~";
			}
		}
		
		
		public static string InvalidIndexOfProperty(FileRecordMap dic) {
			string retVal = NOCOMMENTS;
//			if (dic.Values["ps_code"].Trim()=="00" || dic.Values["property_district_code"].Trim()=="01") {
//				//Debug.Print(CreateDeedNoFromDictionaryWithItem(dic) + ", " + dic["ps_code"]);
//				retVal += "~property_district_code/ps_code missing~";
//			}
			if(IsNumericReturnBOOL(dic.Values["premises"].Trim()))
			{
				if(Convert.ToInt32(dic.Values["premises"].Trim()) == 0)
				{
					if(dic.Values["plot_no"].Trim().Length > 0)
					{
						if(IsNumericReturnBOOL(dic.Values["plot_no"].Trim()))
						{
							if(Convert.ToInt32(dic.Values["plot_no"].Trim()) == 0)
							{
								retVal += "~Plot number is zero~";
							}
						}
					}
					if(dic.Values["plot_no"].Trim().Length == 0)
					{
						retVal += "~Plot number missing~";
					}
				}
			}
			if(IsNumericReturnBOOL(dic.Values["plot_no"].Trim()))
			{
				if(Convert.ToInt32(dic.Values["plot_no"].Trim()) == 0)
				{
					if(dic.Values["premises"].Trim().Length == 0)
					{
						retVal += "~Premises number missing~";
					}
				}
			}
			if(IsNumericReturnBOOL(dic.Values["land_area_acre"].Trim()) && IsNumericReturnBOOL(dic.Values["land_area_bigha"].Trim()) && IsNumericReturnBOOL(dic.Values["land_area_decimal"].Trim()) && IsNumericReturnBOOL(dic.Values["land_area_katha"].Trim()) && IsNumericReturnBOOL(dic.Values["land_area_chatak"].Trim()) && IsNumericReturnBOOL(dic.Values["land_area_sqfeet"].Trim()) && IsNumericReturnBOOL(dic.Values["structure_area_in_sqfeet"].Trim()))
			{
				if(dic.Values["land_area_acre"].Trim().Length > 0 && dic.Values["land_area_bigha"].Trim().Length > 0 && dic.Values["land_area_decimal"].Trim().Length > 0 && dic.Values["land_area_katha"].Trim().Length > 0 && dic.Values["land_area_chatak"].Trim().Length > 0 && dic.Values["land_area_sqfeet"].Trim().Length > 0 && dic.Values["structure_area_in_sqfeet"].Trim().Length > 0)
				{
					if(Convert.ToInt32(dic.Values["land_area_acre"].Trim()) == 0 && Convert.ToInt32(dic.Values["land_area_bigha"].Trim()) == 0 && Convert.ToInt32(dic.Values["land_area_decimal"].Trim()) == 0 && Convert.ToInt32(dic.Values["land_area_katha"].Trim()) == 0 && Convert.ToInt32(dic.Values["land_area_chatak"].Trim()) == 0 && Convert.ToInt32(dic.Values["land_area_sqfeet"].Trim()) == 0 && Convert.ToInt32(dic.Values["structure_area_in_sqfeet"].Trim()) == 0)
					{
						retVal += "~Mesurement missing~";
					}
				}
			}
			
//			if(IsNumericReturnBOOL(dic.Values["ps_code"].Trim()))
//			{
//				if(Convert.ToInt32(dic.Values["ps_code"].Trim()) == 0)
//				{
//					retVal += "~property_district_code/ps_code missing~";
//				}
//			}
//			if(dic.Values["ps_code"].Trim().Length == 0)
//			{
//				retVal += "~property_district_code/ps_code missing~";
//			}
			
			
			if(dic.Values["property_district_code"].Trim().Length == 0)
			{
				retVal += "~property_district_code/ps_code missing~";
			}
			
			if(dic.Values["property_district_code"].Trim().Length > 0)
			{
				if(dic.Values["ps_code"].Trim().Length == 0)
				{
					retVal += "~property_district_code/ps_code missing~";
				}
				if(IsNumericReturnBOOL(dic.Values["ps_code"].Trim()))
				{
					if(Convert.ToInt32(dic.Values["ps_code"].Trim()) == 0)
					{
						retVal += "~property_district_code/ps_code missing~";
					}
				}
			}
			
			if(IsNumericReturnBOOL(dic.Values["property_district_code"].Trim()))
			{
				if(IsNumericReturnBOOL(dic.Values["ps_code"].Trim()))
				{
					if((Convert.ToInt32(dic.Values["property_district_code"].Trim()) + Convert.ToInt32(dic.Values["ps_code"].Trim())) == 0)
					{
						retVal += "~property_district_code/ps_code missing~";
					}
				}
			}
			
//			else {
//				retVal = NOCOMMENTS;
//			}
			return retVal;
		}
		
		public static string InvalidDeedLength(FileRecordMap dic)
		{
			string retVal = NOCOMMENTS;
			
			if(dic.flName.ToLower().Contains("deed_details"))
			{
				if(!ChkRangeBool(dic.Values["deed_no"],5,6))
				{
					retVal += "~Length error in Deed_No~";
				}
				
				if(!ChkRangeBool(dic.Values["serial_no"],5,6))
				{
					retVal += "~Length error in Serial_No";
				}
			}
			else
			{
				if(!ChkRangeBool(dic.Values["deed_no"],5,6))
				{
					retVal += "~Length error in Deed_No~";
				}
			}
			return retVal;
		}
		
		
		public static string InvalidDeedPage(FileRecordMap dic)
		{
			string retVal = NOCOMMENTS;
			
			if(IsNumericReturnBOOL(dic.Values["page_from"]) && IsNumericReturnBOOL(dic.Values["page_to"]))
			{
				if(Convert.ToInt32(dic.Values["page_from"]) > Convert.ToInt32(dic.Values["page_to"]))
				{
					retVal += "~Page_From is greater than Page_To~";
				}
			}
			return retVal;
		}
		
		
		public static string InvalidDeedDetails(FileRecordMap dic)
		{
			
			if(ChkAlphaNumericBOOL(dic.Values["deed_no"]))
			{
				return "~Alphanumeric value in Deed_No";
			}
			else
			{
				return NOCOMMENTS;
			}
		}
		
		
		public static string InvalidIndex_of_Name(FileRecordMap dic)
		{
			string flag = NOCOMMENTS;
			if(DeedValidation.IsNumericReturnBOOL(dic.Values["pin"].Trim()))
			{
				if(Convert.ToInt32(dic.Values["pin"].Trim()) == 0 && dic.Values["address_district_code"].Trim().Length == 0 && dic.Values["address_district_name"].Trim().Length == 0 && dic.Values["address_ps_code"].Trim().Length == 0 && dic.Values["address_ps_name"].Trim().Length == 0 && dic.Values["city"].Trim().Length == 0)
				{
					flag = "~Address missing~";
				}
			}
			if(dic.Values["address_district_code"].Trim().Length == 0 && dic.Values["address_district_name"].Trim().Length == 0 && dic.Values["address_ps_code"].Trim().Length == 0 && dic.Values["address_ps_name"].Trim().Length == 0 && dic.Values["pin"].Trim().Length == 0 && dic.Values["city"].Trim().Length == 0)
			{
				flag = "~Address missing~";
			}
			
			return flag;
		}
		
		
	}
}

