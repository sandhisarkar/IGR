using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Camera.Camerafunctions;
using System.Collections;
using NovaNet.Utils;
using Camera.CameraFunctions;
using System.Runtime.InteropServices;
using LItems;
using System.Data.Odbc;
using NovaNet.wfe;
using System.IO;
using System.Drawing.Drawing2D;
using System.Threading;

namespace ImageHeaven
{
    public partial class frmlivebig : Form
    {
        private IntPtr camList1;
        private int fCount;
        private IntPtr camList2;
        private int numCams;
        private IntPtr m_cam1;
        private IntPtr m_cam2;
        private int camera1Far = 1;
        private int camera1Near = 1;
        private int camera2Far = 1;
        private int camera2Near = 1;
        private Camera.Camerafunctions.Camera camera = new Camera.Camerafunctions.Camera();
        private Camera.Camerafunctions.Camera camera1 = new Camera.Camerafunctions.Camera();
        private int count;
        public static Hashtable m_cmbTbl = new Hashtable();
        private Camera.CameraFunctions.CameraProperty propObj = new CameraProperty();
        private ci tmpImg;
        private wfeBox wBox = null;
        private OdbcConnection sqlCon;
        private wfeBatch pBatch = null;
        private wfeProject pProject = null;
        //private ADFScanUtils scanUtil=null;
        private wfeBatch wBatch = null;
        NovaNet.Utils.dbCon dbcon = new NovaNet.Utils.dbCon();
        private CtrlPolicy pPolicy = null;
        private CtrlBox pBox = null;
        //private TImgDisp timg = new TImgDisp();
        string scanFolder = null;
        //private IContainer components;
        private bool msgfilter;
        //private int		picnumber = 0;
        ArrayList policyList;
        private CtrlImage pImage = null;
        public static NovaNet.Utils.exLog.Logger exMailLog = new NovaNet.Utils.exLog.emailLogger("./errLog.log", NovaNet.Utils.exLog.LogLevel.Dev, Constants._MAIL_TO, Constants._MAIL_FROM, Constants._SMTP);
        public static NovaNet.Utils.exLog.Logger exTxtLog = new NovaNet.Utils.exLog.txtLogger("./errLog.log", NovaNet.Utils.exLog.LogLevel.Dev);
        string batchPath = null;
        string scanDate;
        int pageCount;
        bool hasphoto;
        bool policyChanged = true;
        int updatedPolCount;
        Credentials crd = new Credentials();
        bool hasImage = false;
        public frmlivebig(wfeBox prmBox, OdbcConnection prmCon, Credentials prmCrd)
        {
            InitializeComponent();
            sqlCon = prmCon;
            wBox = prmBox;
            tmpImg = (ci)IgrFactory.GetImagery(Constants.IGR_CLEARIMAGE);
            crd = prmCrd;
            exMailLog.SetNextLogger(exTxtLog);
        }

        private void frmlivebig_Load(object sender, EventArgs e)
        {
            camera.StartLiveView(this.picCamera);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
