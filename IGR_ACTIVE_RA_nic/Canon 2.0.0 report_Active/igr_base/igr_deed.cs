using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NovaNet.Utils;
using NvUtils;
using DataLayerDefs;
using IGR;
using System.Data;
using System.Data.Odbc;

namespace igr_base
{
    public class DeedEventArgs : EventArgs
    {
        private Deed _deed;

        public Deed _Deed
        {
            get { return _deed; }
        }
        public DeedEventArgs(Deed pDeed)
        {
            this._deed = pDeed;
        }
    }
    public class igr_deed
    {
        public static int nxt;
        //The connection object. Expected to be open. We will use this one to query
        //the database
        private OdbcConnection con=null;
        //Transaction
        OdbcTransaction txn;
        //The credentials with which the user has logged on...
        Credentials crd = new Credentials();
		//The control object which will act as the key to retrieving records
        DeedControl ctrlDeed = null;
        //The mode with which a deed is being requested (New, Old)
        private Mode _mode = Mode._View;
        ///The deed which will hold all it's subsequent properties
        Deed deed = null;
        ///Holds the object which will keep the connection open with underlying data layer 
       	DataLayer dl = null;
        ///Holds list of errors
        List<string> err = null;
        //public delegate void CommitHandler(object _formObject, DeedEventArgs _deedArgs);
        public event EventHandler<DeedEventArgs> OnCommit;
        public event EventHandler<DeedEventArgs> OnAbort;

        #region deed.statics
        /// <summary><c>GetAllDeeds</c> is a static method in the <c>igr_deed</c> class to return all deeds.
    	/// </summary> 
        public static List<Deed> GetAllDeeds(OdbcConnection pCon,OdbcTransaction pTxn ,Credentials prmCrd)
        {
        	List<Deed> retVal = new List<Deed>();
        	DataLayer dl = new DataLayer(pCon,pTxn,prmCrd);
        	DataTable dt = dl._GetAllDeeds();
        	return retVal;
        }
        public static List<Volumes> GetAllVolumes(OdbcConnection pCon,OdbcTransaction pTxn, Credentials prmCrd)
        {
        	List<Volumes> retVal = new List<Volumes>();
        	DataLayer dl = new DataLayer(pCon,pTxn,prmCrd);
        	DataTable dt = dl._GetAllVolumes();
        	for (int i=0; i<dt.Rows.Count; i++)
        	{
        		Volumes tmp = new Volumes();
        		tmp.District_Code = dt.Rows[i]["District_code"].ToString();
        		tmp.District_name  = dt.Rows[i]["District_name"].ToString();
        		tmp.Ro_Code = dt.Rows[i]["RO_code"].ToString();
        		tmp.Ro_name = dt.Rows[i]["RO_name"].ToString();
        		tmp.Year = dt.Rows[i]["Year"].ToString();
        		tmp.Book = dt.Rows[i]["Book"].ToString();
        		tmp.Volume = dt.Rows[i]["Volume"].ToString();
        		tmp.Nos = dt.Rows[i]["Nos"].ToString();
        		retVal.Add(tmp);
        	}
        	return retVal;
        }
        public static List<DeedControl> GetDeedsByVolume(OdbcConnection pCon,OdbcTransaction pTxn, Credentials prmCrd, Volumes pVol)
        {
        	List<DeedControl> retVal = new List<DeedControl>();
        	DataLayer dl = new DataLayer(pCon,pTxn,prmCrd);
        	DataTable dt = dl._GetDeeds_Volume(pVol);
        	for (int i=0; i<dt.Rows.Count; i++)
        	{
        		DeedControl tmp = new DeedControl();
        		tmp.District_code = dt.Rows[i]["District_code"].ToString();
        		tmp.RO_code = dt.Rows[i]["RO_code"].ToString();
        		tmp.Deed_year = dt.Rows[i]["Year"].ToString();
        		tmp.Book = dt.Rows[i]["Book"].ToString();
        		tmp.Deed_no = dt.Rows[i]["Deed_no"].ToString();
        		retVal.Add(tmp);
        	}
        	return retVal;
        }       
        #endregion
        public igr_deed(OdbcConnection pCon, OdbcTransaction pTxn,Credentials prmCrd)
        {
            con = pCon;
            crd = prmCrd;
            _mode = Mode._Add;
            txn = pTxn;
            init_deed();
        }
        public igr_deed(OdbcConnection pCon, OdbcTransaction pTxn,Credentials prmCrd, DeedControl pDeedControl)
        {
            con = pCon;
            crd = prmCrd;
            _mode = Mode._Edit;
            ctrlDeed = pDeedControl;
            txn = pTxn;
            init_deed();
        }
        public Mode _Mode {
			get { return _mode; }
		}
        public Deed _GetDeed {
			get { return deed; }
		}        
        private bool init_deed()
        {
        	//The connection to data layer
        	dl = new DataLayer(con,txn,crd);
        	//The semi constant row no to retrieve values from
        	int rowNo = 0;
        	//The data table to hold retrieved values from datalayer
        	DataTable dt = null;
        	//The return value
        	bool retVal = false;
        	//Initialize the deed
        	deed = new Deed();
            //Initialize the list of errors that has been encountered during validation
            err = new List<string>();
        	//Retrieve values if it's in editing mode
            if (_mode == Mode._Edit)
            {
                dt = dl._GetDeed(ctrlDeed);


                //Populate values
                if (dt.Rows.Count > 0 && _mode == Mode._Edit)
                {
                    //Load deed header into the structure
                    deed.DeedHeader.Deed_control = ctrlDeed;
                    deed.DeedHeader.Serial_no = dt.Rows[rowNo]["Serial_No"].ToString();
                    deed.DeedHeader.Serial_year = dt.Rows[rowNo]["Serial_Year"].ToString();
                    deed.DeedHeader.tran_maj_code = dt.Rows[rowNo]["tran_maj_code"].ToString();
                    deed.DeedHeader.tran_min_code = dt.Rows[rowNo]["tran_min_code"].ToString();
                    deed.DeedHeader.volume_no = dt.Rows[rowNo]["Volume_No"].ToString();
                    deed.DeedHeader.page_from = dt.Rows[rowNo]["Page_From"].ToString();
                    deed.DeedHeader.page_to = dt.Rows[rowNo]["Page_To"].ToString();
                    deed.DeedHeader.date_of_completion = dt.Rows[rowNo]["Date_of_Completion"].ToString();
                    deed.DeedHeader.date_of_delivery = dt.Rows[rowNo]["Date_of_Delivery"].ToString();
                    deed.DeedHeader.deed_remarks = dt.Rows[rowNo]["Deed_Remarks"].ToString();
                    deed.DeedHeader.created_system = dt.Rows[rowNo]["created_system"].ToString();
                    deed.DeedHeader.Exported = dt.Rows[rowNo]["Exported"].ToString();
                    deed.DeedHeader.addl_pages = dt.Rows[rowNo]["addl_pages"].ToString();
                    deed.DeedHeader.scan_doc_type = dt.Rows[rowNo]["Scan_doc_type"].ToString();
                    deed.DeedHeader.hold  = dt.Rows[rowNo]["hold"].ToString();
                    deed.DeedHeader.hold_reason = dt.Rows[rowNo]["hold_reason"].ToString();
                    deed.DeedHeader.status = dt.Rows[rowNo]["status"].ToString();
                    deed.DeedHeader.version = dt.Rows[rowNo]["version"].ToString();
                    deed.DeedHeader.Mismatch = dt.Rows[rowNo]["mismatch"].ToString();
                    deed.DeedHeader.Created_By = dt.Rows[rowNo]["created_by"].ToString();
                    deed.DeedHeader.Created_Dttm = dt.Rows[rowNo]["created_dttm"].ToString();
                    deed.DeedHeader.Modified_By = dt.Rows[rowNo]["modified_by"].ToString();
                    deed.DeedHeader.Modified_Dttm = dt.Rows[rowNo]["modified_dttm"].ToString();
                    //deed.DeedHeader.exception = dt.Rows[rowNo]["exception"].ToString();
                    //Fetch the persons details
                    dt = dl._GetPersons(ctrlDeed);
                    for (rowNo = 0; rowNo < dt.Rows.Count; rowNo++)
                    {
                        PersonDetails p = new PersonDetails();
                        p.district_code = dt.Rows[rowNo]["District_Code"].ToString();
                        p.RO_code = dt.Rows[rowNo]["RO_Code"].ToString();
                        p.Book = dt.Rows[rowNo]["Book"].ToString();
                        p.Deed_year = dt.Rows[rowNo]["Deed_year"].ToString();
                        p.Deed_no = dt.Rows[rowNo]["Deed_no"].ToString();
                        p.Serial = dt.Rows[rowNo]["Item_no"].ToString();
                        p.initial_name = dt.Rows[rowNo]["initial_name"].ToString();
                        p.First_name = dt.Rows[rowNo]["First_name"].ToString();
                        p.Last_Name = dt.Rows[rowNo]["Last_name"].ToString();
                        p.Status_code = dt.Rows[rowNo]["Party_code"].ToString();
                        p.Admit_code = dt.Rows[rowNo]["Admit_code"].ToString();
                        p.Address = dt.Rows[rowNo]["Address"].ToString();
                        p.Address_district_code = dt.Rows[rowNo]["Address_district_code"].ToString();
                        p.Address_district_name = dt.Rows[rowNo]["Address_district_name"].ToString();
                        p.Address_ps_code = dt.Rows[rowNo]["Address_ps_code"].ToString();
                        p.Address_ps_Name = dt.Rows[rowNo]["Address_ps_name"].ToString();
                        p.Father_mother = dt.Rows[rowNo]["Father_mother"].ToString();
                        p.Rel_code = dt.Rows[rowNo]["Rel_code"].ToString();
                        p.Relation = dt.Rows[rowNo]["Relation"].ToString();
                        p.Proffession = dt.Rows[rowNo]["occupation_code"].ToString();
                        p.Cast = dt.Rows[rowNo]["religion_code"].ToString();
                        p.more = dt.Rows[rowNo]["more"].ToString();
                        p.PIN = dt.Rows[rowNo]["pin"].ToString();
                        p.City_Name = dt.Rows[rowNo]["city"].ToString();
                        p.other_party_code = dt.Rows[rowNo]["other_party_code"].ToString();
                        p.linked_to = dt.Rows[rowNo]["linked_to"].ToString();
                        p.Created_By = dt.Rows[rowNo]["created_by"].ToString();
                        p.Created_Dttm = dt.Rows[rowNo]["created_dttm"].ToString();
                        //p.exception = dt.Rows[rowNo]["exception"].ToString();
                        deed.Persons.Add(p);
                    }
                    //Fetch the Deed Exception details
                    dt = dl._GetDeedExp(ctrlDeed);
                    for (rowNo = 0; rowNo < dt.Rows.Count; rowNo++)
                    {
                        deedDetailsException p = new deedDetailsException();
                        p.district_code = dt.Rows[rowNo]["District_Code"].ToString();
                        p.RO_code = dt.Rows[rowNo]["RO_Code"].ToString();
                        p.Book = dt.Rows[rowNo]["Book"].ToString();
                        p.Deed_year = dt.Rows[rowNo]["Deed_year"].ToString();
                        p.Deed_no = dt.Rows[rowNo]["Deed_no"].ToString();
                        p.serial = dt.Rows[rowNo]["srl_no"].ToString();
                        p.exception  = dt.Rows[rowNo]["Exception"].ToString();
                        p.excDetails = dt.Rows[rowNo]["Details"].ToString();
                        deed.D_Excp.Add(p);
                    }
                    //Fetch the Person Exception details
                    dt = dl._GetPersonsExcp(ctrlDeed);
                    for (rowNo = 0; rowNo < dt.Rows.Count; rowNo++)
                    {
                        PersonDetailsException p = new PersonDetailsException();
                        p.district_code = dt.Rows[rowNo]["District_Code"].ToString();
                        p.RO_code = dt.Rows[rowNo]["RO_Code"].ToString();
                        p.Book = dt.Rows[rowNo]["Book"].ToString();
                        p.Deed_year = dt.Rows[rowNo]["Deed_year"].ToString();
                        p.Deed_no = dt.Rows[rowNo]["Deed_no"].ToString();
                        p.serial = dt.Rows[rowNo]["srl_no"].ToString();
                        p.item_no = dt.Rows[rowNo]["item_no"].ToString();
                        p.exception = dt.Rows[rowNo]["Exception"].ToString();
                        p.excDetails = dt.Rows[rowNo]["Details"].ToString();
                        deed.P_Excp.Add(p);
                    }
                    //Fetch the Person Exception details
                    dt = dl._GetPropertyExcp(ctrlDeed);
                    for (rowNo = 0; rowNo < dt.Rows.Count; rowNo++)
                    {
                        PropertyDetailsException p = new PropertyDetailsException();
                        p.district_code = dt.Rows[rowNo]["District_Code"].ToString();
                        p.RO_code = dt.Rows[rowNo]["RO_Code"].ToString();
                        p.Book = dt.Rows[rowNo]["Book"].ToString();
                        p.Deed_year = dt.Rows[rowNo]["Deed_year"].ToString();
                        p.Deed_no = dt.Rows[rowNo]["Deed_no"].ToString();
                        p.item_no = dt.Rows[rowNo]["item_no"].ToString();
                        p.serial = dt.Rows[rowNo]["srl_no"].ToString();
                        p.exception = dt.Rows[rowNo]["Exception"].ToString();
                        p.excDetails = dt.Rows[rowNo]["Details"].ToString();
                        deed.Pro_Excp.Add(p);
                    }
                    dt = dl._GetoutWBList(ctrlDeed);
                    for (rowNo = 0; rowNo < dt.Rows.Count; rowNo++)
                    {
                        outSideWBList p = new outSideWBList();
                        p.district_code = dt.Rows[rowNo]["District_Code"].ToString();
                        p.RO_code = dt.Rows[rowNo]["RO_Code"].ToString();
                        p.Book = dt.Rows[rowNo]["Book"].ToString();
                        p.Deed_year = dt.Rows[rowNo]["Deed_year"].ToString();
                        p.Deed_no = dt.Rows[rowNo]["Deed_no"].ToString();
                        p.serial = dt.Rows[rowNo]["item_no"].ToString();
                        p.exception = dt.Rows[rowNo]["isOutsideWB"].ToString();

                        deed.Pro_ExcpWb.Add(p);
                    }
                    //Fetch the properties details
                    dt = dl._GetProperties(ctrlDeed);
                    for (rowNo = 0; rowNo < dt.Rows.Count; rowNo++)
                    {
                        PropertyDetails p = new PropertyDetails();
                        p.district_code = dt.Rows[rowNo]["District_Code"].ToString();
                        p.RO_code = dt.Rows[rowNo]["RO_Code"].ToString();
                        p.Book = dt.Rows[rowNo]["Book"].ToString();
                        p.Deed_year = dt.Rows[rowNo]["Deed_year"].ToString();
                        p.Deed_no = dt.Rows[rowNo]["Deed_no"].ToString();
                        p.Serial = dt.Rows[rowNo]["Item_no"].ToString();
                        p.Property_district_code = dt.Rows[rowNo]["Property_district_code"].ToString();
                        p.Property_ro_code = dt.Rows[rowNo]["Property_ro_code"].ToString();
                        p.ps_code = dt.Rows[rowNo]["ps_code"].ToString();
                        p.moucode = dt.Rows[rowNo]["moucode"].ToString();
                        p.Area_type = dt.Rows[rowNo]["Area_type"].ToString();
                        p.GP_Muni_Corp_Code = dt.Rows[rowNo]["GP_Muni_Corp_Code"].ToString();
                        p.Ward = dt.Rows[rowNo]["Ward"].ToString();
                        p.Holding = dt.Rows[rowNo]["Holding"].ToString();
                        p.Premises = dt.Rows[rowNo]["Premises"].ToString();
                        p.road_code = dt.Rows[rowNo]["road_code"].ToString();
                        p.Plot_code_type = dt.Rows[rowNo]["Plot_code_type"].ToString();
                        p.Road = dt.Rows[rowNo]["Road"].ToString();
                        p.Plot_No = dt.Rows[rowNo]["Plot_No"].ToString();
                        p.Bata_No = dt.Rows[rowNo]["Bata_No"].ToString();
                        p.Khatian_type = dt.Rows[rowNo]["Khatian_type"].ToString();
                        p.khatian_No = dt.Rows[rowNo]["khatian_No"].ToString();
                        p.bata_khatian_no = dt.Rows[rowNo]["bata_khatian_no"].ToString();
                        p.property_type = dt.Rows[rowNo]["property_type"].ToString();
                        p.Land_Area_acre = dt.Rows[rowNo]["Land_Area_acre"].ToString();
                        p.Land_Area_bigha = dt.Rows[rowNo]["Land_Area_bigha"].ToString();
                        p.Land_Area_decimal = dt.Rows[rowNo]["Land_Area_decimal"].ToString();
                        p.Land_Area_katha = dt.Rows[rowNo]["Land_Area_katha"].ToString();
                        p.Land_Area_chatak = dt.Rows[rowNo]["Land_Area_chatak"].ToString();
                        p.Land_Area_sqfeet = dt.Rows[rowNo]["Land_Area_sqfeet"].ToString();
                        p.Structure_area_in_sqFeet = dt.Rows[rowNo]["Structure_area_in_sqFeet"].ToString();
                        p.Ref_ps = dt.Rows[rowNo]["ref_ps"].ToString();
                        p.Ref_mou = dt.Rows[rowNo]["ref_mouza"].ToString();
                        p.JL_NO = dt.Rows[rowNo]["JL_no"].ToString();
                        p.other_plots = dt.Rows[rowNo]["Other_plots"].ToString();
                        p.other_Khatian = dt.Rows[rowNo]["Other_Khatian"].ToString();
                        p.land_type = dt.Rows[rowNo]["land_type"].ToString();
                        p.Ref_JL_Number = dt.Rows[rowNo]["RefJL_no"].ToString();
                        p.Created_By = dt.Rows[rowNo]["created_by"].ToString();
                        p.Created_Dttm = dt.Rows[rowNo]["created_dttm"].ToString();
                        //p.exception = dt.Rows[rowNo]["exception"].ToString();
                        //Add the properties list to the deed
                        deed.Properties.Add(p);
                    }
                    //Fetch the outside WB property details
                    dt = dl._GetPropertiesWB(ctrlDeed);
                    for (rowNo = 0; rowNo < dt.Rows.Count; rowNo++)
                    {
                        PropertyDetailsWB p = new PropertyDetailsWB();
                        p.district_code = dt.Rows[rowNo]["District_Code"].ToString();
                        p.RO_code = dt.Rows[rowNo]["RO_Code"].ToString();
                        p.Book = dt.Rows[rowNo]["Book"].ToString();
                        p.Deed_year = dt.Rows[rowNo]["Deed_year"].ToString();
                        p.Deed_no = dt.Rows[rowNo]["Deed_no"].ToString();
                        p.Serial = dt.Rows[rowNo]["Item_no"].ToString();
                        p.Property_country_code = dt.Rows[rowNo]["Property_country_code"].ToString();
                        p.Property_state_code = dt.Rows[rowNo]["Property_state_code"].ToString();
                        p.Property_district_code = dt.Rows[rowNo]["Property_district_code"].ToString();
                        p.thana = dt.Rows[rowNo]["thana"].ToString();
                        p.mouza  = dt.Rows[rowNo]["moucode"].ToString();
                        p.Plot_code_type = dt.Rows[rowNo]["Plot_code_type"].ToString();
                        p.Plot_No = dt.Rows[rowNo]["Plot_No"].ToString();
                        p.Khatian_type = dt.Rows[rowNo]["Khatian_type"].ToString();
                        p.khatian_No = dt.Rows[rowNo]["khatian_No"].ToString();
                        p.land_use = dt.Rows[rowNo]["land_use"].ToString();
                        p.property_type = dt.Rows[rowNo]["property_type"].ToString();

                        p.Area_acre = dt.Rows[rowNo]["area_acre"].ToString();
                        p.Area_Bigha = dt.Rows[rowNo]["area_bigha"].ToString();
                        p.Area_Chatak = dt.Rows[rowNo]["area_chatak"].ToString();
                        p.Area_Decimal = dt.Rows[rowNo]["area_decimal"].ToString();
                        p.Area_Katha = dt.Rows[rowNo]["area_katha"].ToString();
                        p.Area_SqtL = dt.Rows[rowNo]["area_sqf"].ToString();
                        p.Total_decimal = dt.Rows[rowNo]["total_area_decimal"].ToString();
                        p.structure_sqt = dt.Rows[rowNo]["struct_sqfeet"].ToString();

                        //p.Area = dt.Rows[rowNo]["Area"].ToString();
                        p.local_body_type = dt.Rows[rowNo]["local_body_type"].ToString();
                        p.other_details = dt.Rows[rowNo]["other_details"].ToString();
                        p.Created_By = dt.Rows[rowNo]["created_by"].ToString();
                        p.Created_Dttm = dt.Rows[rowNo]["created_dttm"].ToString();

                        //Add the properties list to the deed
                        deed.PropertiesoutWB.Add(p);
                    }
                    //Fetch the other plot details
                    DataTable dtp = dl._GetOtherPlots(ctrlDeed);
                    for (int rowNoChilds = 0; rowNoChilds < dtp.Rows.Count; rowNoChilds++)
                    {
                        PropertyDetails_other_plot op = new PropertyDetails_other_plot();
                        op.district_code = dtp.Rows[rowNoChilds]["district_code"].ToString();
                        op.RO_code = dtp.Rows[rowNoChilds]["ro_code"].ToString();
                        op.Book = dtp.Rows[rowNoChilds]["book"].ToString();
                        op.Deed_year = dtp.Rows[rowNoChilds]["deed_year"].ToString();
                        op.Deed_no = dtp.Rows[rowNoChilds]["deed_no"].ToString();
                        op.item_no = dtp.Rows[rowNoChilds]["item_no"].ToString();
                        op.other_plot_no = dtp.Rows[rowNoChilds]["other_plots"].ToString();
                        deed.Lst_other_plots.Add(op);
                    }
                    //Fetch the other khatian details
                    dtp = dl._GetOtherKhatians(ctrlDeed);
                    for (int rowNoChilds = 0; rowNoChilds < dtp.Rows.Count; rowNoChilds++)
                    {
                        PropertyDetails_other_khatian op = new PropertyDetails_other_khatian();
                        op.district_code = dtp.Rows[rowNoChilds]["district_code"].ToString();
                        op.RO_code = dtp.Rows[rowNoChilds]["ro_code"].ToString(); // Changed on 19/02/2014, due to wrong ro code entry
                        op.Book = dtp.Rows[rowNoChilds]["book"].ToString();
                        op.Deed_year = dtp.Rows[rowNoChilds]["deed_year"].ToString();
                        op.Deed_no = dtp.Rows[rowNoChilds]["deed_no"].ToString();
                        op.item_no = dtp.Rows[rowNoChilds]["item_no"].ToString();
                        op.other_Khatian_no = dtp.Rows[rowNoChilds]["other_khatian"].ToString();
                        deed.Lst_other_khatians.Add(op);
                    }
                    
                }
                
                
            }
            return retVal;
        }
        //Saves the deed in db
        public bool SaveDeed()
        {
            

            bool retVal = false;
        	if (_mode==Mode._Edit && Validate())
        	{
        		dl._UpdateDeed(deed);
                dl._DeleteIndex1(deed.DeedHeader.Deed_control);
                dl._DeleteIndex2(deed.DeedHeader.Deed_control);
                dl._DeleteIndex2WB(deed.DeedHeader.Deed_control);
                dl._DeleteOtherPlot(deed.DeedHeader.Deed_control);
                dl._DeleteOtherKhatian(deed.DeedHeader.Deed_control);
                dl._DeletedeedExp(deed.DeedHeader.Deed_control);
                dl._DeletepersonExp(deed.DeedHeader.Deed_control);
                dl._DeletePropertyExp(deed.DeedHeader.Deed_control);
                dl._DeleteIndex2WBList(deed.DeedHeader.Deed_control);
                dl._InsertDeedException(deed.D_Excp);
                dl._InsertPersonDetails(deed.Persons);
                dl._InsertPersonException(deed.P_Excp);
                dl._InsertPropertyDetails(deed.Properties);
                dl._InsertPropertyDetailsWB(deed.PropertiesoutWB);
                dl._InsertPropertyException(deed.Pro_Excp);
                dl._InsertOtherPlots(deed.Lst_other_plots);
                dl._InsertOtherkhatian(deed.Lst_other_khatians);
                dl._InsertWBList(deed.Pro_ExcpWb);
                retVal = true;
        	}
            if (_mode == Mode._Add && Validate())
            {
                dl._InsertDeed(deed);
                dl._InsertDeedException(deed.D_Excp);
                dl._InsertPersonDetails(deed.Persons);
                dl._InsertPersonException(deed.P_Excp);
                dl._InsertPropertyDetails(deed.Properties);
                dl._InsertPropertyDetailsWB(deed.PropertiesoutWB);
                dl._InsertPropertyException(deed.Pro_Excp);
                dl._InsertWBList(deed.Pro_ExcpWb);
                dl._InsertOtherPlots(deed.Lst_other_plots);
                dl._InsertOtherkhatian(deed.Lst_other_khatians);
                retVal = true;
            }
            EventHandler<DeedEventArgs> CommitHandler = OnCommit;
            if (CommitHandler != null && retVal)
            {
                DeedEventArgs tmpDeedEventArgs = new DeedEventArgs(this._GetDeed);
                OnCommit(this, tmpDeedEventArgs);
            }
            else
            {
                EventHandler<DeedEventArgs> AbortHandler = OnAbort;
                if (AbortHandler != null && retVal)
                {
                    DeedEventArgs tmpDeedEventArgs = new DeedEventArgs(this._GetDeed);
                    OnAbort(this, tmpDeedEventArgs);
                }
            }
        	return retVal;
        }

        public DataTable DeedExists(DeedControl pdc)
        {
            DataTable dt = new DataTable();
            dt = dl._GetDeedExists(pdc);
            return dt;
        }
        public bool RenameDeed(DeedControl ndc,string vol,string NewVol)
        {
            bool retVal = false;
            dl._RenameDeedNo(deed.DeedHeader.Deed_control,ndc, NewVol);
            dl._RenameIndex1(deed.DeedHeader.Deed_control,ndc);
            dl._RenameIndex2(deed.DeedHeader.Deed_control,ndc);
            dl._RenameOtherplots(deed.DeedHeader.Deed_control,ndc);
            dl._RenameOtherKhatian(deed.DeedHeader.Deed_control,ndc);
            retVal = true;
            return retVal;
        }
        public bool RenameVolume(DeedControl ndc, string NewVol)
        {
            bool retVal = false;
            dl._RenameDeedVolume(deed.DeedHeader.Deed_control,NewVol);
            retVal = true;
            return retVal;
        }        
        public bool DeleteDeed()
        {
            bool retVal = false;
            dl._DeleteDeed(deed.DeedHeader.Deed_control);
            dl._DeletedeedExp(deed.DeedHeader.Deed_control);
            dl._DeleteIndex1(deed.DeedHeader.Deed_control);
            dl._DeletepersonExp(deed.DeedHeader.Deed_control);
            dl._DeleteIndex2(deed.DeedHeader.Deed_control);
            dl._DeletePropertyExp(deed.DeedHeader.Deed_control);
            dl._DeleteOtherPlot(deed.DeedHeader.Deed_control);
            dl._DeleteOtherKhatian(deed.DeedHeader.Deed_control);
            retVal = true;

            return retVal;
        }
        public List<string> GetErrors()
        {
            return err;
        }
        private bool Validate()
        {
            err.Clear();
        	bool retVal = false;
        	//The mode checking (Insert, Update)
        	if (_mode==Mode._Edit)
        	{
                if (deed.DeedHeader.hold == "Y")
                {

                    retVal = true;
                }
                else
                {
                    retVal = ValidateFields();
                }
        	}
            else
            {
                retVal = ValidateFields();
            }
        	//End: The mode checking (Insert, Update)
        	return retVal;
        }
        private bool ValidateFields()
        {
            bool retVal = true;
            if (Convert.ToInt32(deed.DeedHeader.page_from) <= 0)
            {
                retVal = false;
                err.Add("How can you leave Page From Blank!!!");
            }
            if (Convert.ToInt32(deed.DeedHeader.page_to) <= 0)
            {
                retVal = false;
                err.Add("How can you leave Page To Blank!!!");
            }
            if(string.IsNullOrEmpty (deed.DeedHeader.scan_doc_type))
            {
                retVal = false;
                err.Add("How can you leave Document Type Blank!!!");
            }
            //if (string.IsNullOrEmpty(deed.DeedHeader.volume_no))
            //{
            //    retVal = false;
            //    err.Add("How can you leave Volume No Type Blank!!!");
            //}
            if (string.IsNullOrEmpty(deed.DeedHeader.tran_maj_code))
            {
                retVal = false;
                err.Add("How can you leave Transaction Type Blank!!!");
            }
            if (string.IsNullOrEmpty(deed.DeedHeader.tran_min_code))
            {
                retVal = false;
                err.Add("How can you leave Transaction Sub Type Blank!!!");
            }
            if ((deed.DeedHeader.Deed_control.Book  == "1") && (deed.Properties.Count == 0))
            { err.Add("Property List Can not be Left Empty for Book-I ....."); }
            
            
            return retVal;
        }
    }
}
