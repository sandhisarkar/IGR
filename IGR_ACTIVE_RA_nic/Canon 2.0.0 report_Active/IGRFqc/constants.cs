using System.Text;
using System.Windows.Forms;
using DataLayerDefs;
using nControls;
using System.Data;
using System.Data.Odbc;
using NovaNet.Utils;
using NvUtils;
using igr_base;

namespace IGRFqc
{
    class constants
    {
        //Suggesion
        public const bool _SUGGEST = true;

        //Status
        public const string Data_entered = "10";
        public const string Data_edited = "11";
        public const string Data_hold = "12";
        public const string Data_unhold = "13";
        public const string Data_exported = "14";
        public const string Data_reexported = "15";
        public const string Property_Missing = "02";
        public const string Duplicate_Deed_No = "03";
        public const string Address_Missing = "04";
        public const string Plot_and_Premisses_Missing = "05";
        public const string Measurment_Missing = "06";
        public const string OutSide_WB_Property = "07";

        public const string deed_details = "01";
        public const string Index_of_name = "02";
        public const string Index_of_property = "03";

        public const string outsideWBDistrict = "00";
        public const string outsideWBRO = "00";
        public const string outsideWBPS = "999";
    }
    

    }

