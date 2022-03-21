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
    public partial class frmBookScanning : Form
    {
        private IntPtr camList1;
        private int fCount;
        static wItem wi;
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
        public frmBookScanning(wfeBox prmBox, OdbcConnection prmCon, Credentials prmCrd)
        {
            InitializeComponent();
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frmBookScanning_KeyDown);
            cmdAccept.Enabled = false;
            cmdDisconnect.Enabled = false;
            cmdFar1.Enabled = false;
            cmdFar2.Enabled = false;
            cmdLiveView.Enabled = false;
            cmdNear1.Enabled = false;
            cmdNear2.Enabled = false;
            cmdTakePicture.Enabled = false;
            grpCameraControl.Enabled = false;
            sqlCon = prmCon;
            wBox = prmBox;
            this.Text = "Batch Scanning";
            tmpImg = (ci)IgrFactory.GetImagery(Constants.IGR_CLEARIMAGE);
            crd = prmCrd;
            exMailLog.SetNextLogger(exTxtLog);
        }
        private void ChangeZoomSize(string pFileName,PictureBox pPictureBox,ci pImageRef)
        {
            if (!System.IO.File.Exists(pFileName)) return;
            Image newImage = pImageRef.GetBitmap();
            double scaleX = (double)pPictureBox.Width / (double)newImage.Width;
            double scaleY = (double)pPictureBox.Height / (double)newImage.Height;
            double Scale = Math.Min(scaleX, scaleY);
            int w = (int)(newImage.Width * Scale);
            int h = (int)(newImage.Height * Scale);
            //pictureControl.Width = w;
            //pictureControl.Height = h;
            pPictureBox.Image = CreateThumbnail(newImage, w, h); //newImage.GetThumbnailImage(w, h, new System.Drawing.Image.GetThumbnailImageAbort(GetThumbnailImageAbort), IntPtr.Zero);
            newImage.Dispose();
        }
        private Image CreateThumbnail(Image pImage, int lnWidth, int lnHeight)
        {

            Bitmap bmp = new Bitmap(lnWidth, lnHeight);
            try
            {

                DateTime stdt = DateTime.Now;

                //create a new Bitmap the size of the new image

                //create a new graphic from the Bitmap
                Graphics graphic = Graphics.FromImage((Image)bmp);
                graphic.InterpolationMode = InterpolationMode.HighQualityBicubic;
                //draw the newly resized image
                graphic.DrawImage(pImage, 0, 0, lnWidth, lnHeight);
                //dispose and free up the resources
                graphic.Dispose();
                DateTime dt = DateTime.Now;
                TimeSpan tp = dt - stdt;
                //MessageBox.Show(tp.Milliseconds.ToString());
                //return the image

            }
            catch
            {
                return null;
            }
            return (Image)bmp;
        }
        void DisplayValues()
        {
            pBatch = new wfeBatch(sqlCon);
            pProject = new wfeProject(sqlCon);
            lblProjectName.Text = pProject.GetProjectName(wBox.ctrlBox.ProjectCode);
            lblBatch.Text = pBatch.GetBatchName(wBox.ctrlBox.ProjectCode, wBox.ctrlBox.BatchKey);
            lblBox.Text = wBox.ctrlBox.BoxNumber.ToString().ToString();
        }
        void PrevImages()
        {
            eSTATES[] prmPolicyState;

            ArrayList arrImage = new ArrayList();
            wQuery pQuery = new ihwQuery(sqlCon);
            CtrlPolicy ctrlPolicy = new CtrlPolicy(wBox.ctrlBox.ProjectCode, wBox.ctrlBox.BatchKey, wBox.ctrlBox.BoxNumber.ToString(), lblCurrentPolicy.Text.ToString());
            wItem policy = new wfePolicy(sqlCon, ctrlPolicy);
            wBatch = new wfeBatch(sqlCon);
            batchPath = wBatch.GetPath(wBox.ctrlBox.ProjectCode, wBox.ctrlBox.BatchKey);
            CtrlImage ctrlImage;

            prmPolicyState = new eSTATES[1];
            prmPolicyState[0] = eSTATES.POLICY_CREATED;
            arrImage = pQuery.GetItems(eITEMS.PAGE, prmPolicyState, policy);

            lstImageName.Items.Clear();
            if (arrImage.Count > 0)
            {
                for (int l = 0; l < arrImage.Count; l++)
                {
                    ctrlImage = (CtrlImage)arrImage[l];
                    lstImageName.Items.Add(ctrlImage.ImageName);
                }

                scanFolder = batchPath + "\\" + lblBox.Text + "\\" + lblCurrentPolicy.Text + "\\" + ihConstants._SCAN_FOLDER;
            }
            lstImageName.Refresh();
        }
        private bool ShowPolicy()
        {
            CtrlPolicy ctrPolCurrent = null;
            CtrlPolicy ctrPolNext = null;

            policyList = GetPolicyList();
            if (policyList.Count > 0)
            {
                if (policyList.Count > 1)
                {
                    ctrPolCurrent = (CtrlPolicy)policyList[0];
                    ctrPolNext = (CtrlPolicy)policyList[1];
                }
                else
                {
                    ctrPolCurrent = (CtrlPolicy)policyList[0];
                    ctrPolNext = (CtrlPolicy)policyList[0];
                }
                lblCurrentPolicy.Text = ctrPolCurrent.PolicyNumber.ToString();
                lblNextPolicy.Text = ctrPolNext.PolicyNumber.ToString();
                this.Text = "Book Scanning            " + " Project- " + lblProjectName.Text + "  Batch- " + lblBatch.Text + "  Box- " + lblBox.Text + " Current Policy- " + lblCurrentPolicy.Text + " Next Policy- " + lblNextPolicy.Text;
                return true;
            }
            else
            {
                MessageBox.Show("No more policies remain to be scanned....");
                return false;
            }
        }
        private ArrayList GetPolicyList()
        {
            ArrayList arrPolicy = new ArrayList();
            wQuery pQuery = new ihwQuery(sqlCon,1);
            eSTATES[] state = new eSTATES[1];
            state[0] = eSTATES.POLICY_CREATED;
            arrPolicy = pQuery.GetItems(eITEMS.POLICY, state, wBox);
            return arrPolicy;
        }
        private void cmdConnect_Click(object sender, EventArgs e)
        {
            try
            {
                int i = 0;
                numCams = 0;
                EDSDK.EdsInitializeSDK();
                EDSDK.EdsGetCameraList(ref camList1);
                EDSDK.EdsGetChildCount(camList1, ref numCams);
                this.Text = "Book Scanning            " + " Project- " + lblProjectName.Text + "  Batch- " + lblBatch.Text + "  Box- " + lblBox.Text + " Current Policy- " + lblCurrentPolicy.Text + " Next Policy- " + lblNextPolicy.Text;
                this.Text = "     Connecting.........";
                if (numCams > 0)
                {
                    cmbCameraCount.Items.Clear();
                    for (i = 0; i <= numCams - 1; i++)
                    {
                        //EDSDK.EdsRelease(camList1);
                        //EDSDK.EdsGetCameraList(ref camList1);
                        //EDSDK.EdsGetChildCount(camList1, ref numCams);
                        //get the only camera
                        if (i == 0)
                        {
                            EDSDK.EdsGetChildAtIndex(camList1, i, ref m_cam1);
                            camera.EstablishSession(camList1, m_cam1);
                        }
                        else
                        {
                            EDSDK.EdsGetChildAtIndex(camList1, i, ref m_cam2);
                            camera1.EstablishSession(camList1, m_cam2);
                        }

                        cmbCameraCount.Items.Add(i + 1);
                        
                        this.Text = "Book Scanning            " + " Project- " + lblProjectName.Text + "  Batch- " + lblBatch.Text + "  Box- " + lblBox.Text + " Current Policy- " + lblCurrentPolicy.Text + " Next Policy- " + lblNextPolicy.Text;
                        this.Text = this.Text + "    Camera Found: " + (i + 1);
                    }

                    if (cmbCameraCount.Items.Count != 0)
                    {
                        cmbCameraCount.SelectedIndex = 0;
                    }
                    cmdDisconnect.Enabled = true;
                    cmdConnect.Enabled = false;
                    cmdLiveView.Enabled = true;
                    cmbCameraCount.Enabled = true;
                    grpCameraControl.Enabled = true;
                }
                else
                {
                    this.Text = "Book Scanning            " + " Project- " + lblProjectName.Text + "  Batch- " + lblBatch.Text + "  Box- " + lblBox.Text + " Current Policy- " + lblCurrentPolicy.Text + " Next Policy- " + lblNextPolicy.Text;
                    this.Text = this.Text + "     Camera Found: " + "0";
                    cmdDisconnect.Enabled = false;
                    cmdConnect.Enabled = true;
                    cmdLiveView.Enabled = false;
                    cmbCameraCount.Enabled = false;
                    grpCameraControl.Enabled = false;
                    cmdFar1.Enabled = false;
                    cmdFar2.Enabled = false;
                    cmdNear1.Enabled = false;
                    cmdNear2.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error while getting information from the camera, try again later..........");
            }
        }

        private void cmbCameraCount_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (cmbCameraCount.Items.Count != 0)
            {
                InitializeCombo();
                if (cmbCameraCount.Text == "1")
                {
                    PopulateCombo(EDSDKTypes.kEdsPropID_AEMode, m_cam1);
                    PopulateCombo(EDSDKTypes.kEdsPropID_AFMode, m_cam1);
                    PopulateCombo(EDSDKTypes.kEdsPropID_ISOSpeed, m_cam1);
                    PopulateCombo(EDSDKTypes.kEdsPropID_Av, m_cam1);
                    PopulateCombo(EDSDKTypes.kEdsPropID_Tv, m_cam1);
                    PopulateCombo(EDSDKTypes.kEdsPropID_MeteringMode, m_cam1);
                    PopulateCombo(EDSDKTypes.kEdsPropID_ExposureCompensation, m_cam1);
                    PopulateCombo(EDSDKTypes.kEdsPropID_ImageQuality, m_cam1);
                }
                else
                {
                    PopulateCombo(EDSDKTypes.kEdsPropID_AEMode, m_cam2);
                    PopulateCombo(EDSDKTypes.kEdsPropID_AFMode, m_cam2);
                    PopulateCombo(EDSDKTypes.kEdsPropID_ISOSpeed, m_cam2);
                    PopulateCombo(EDSDKTypes.kEdsPropID_Av, m_cam2);
                    PopulateCombo(EDSDKTypes.kEdsPropID_Tv, m_cam2);
                    PopulateCombo(EDSDKTypes.kEdsPropID_MeteringMode, m_cam2);
                    PopulateCombo(EDSDKTypes.kEdsPropID_ExposureCompensation, m_cam2);
                    PopulateCombo(EDSDKTypes.kEdsPropID_ImageQuality, m_cam2);
                }
            }

        }
        private void InitializeCombo()
        {
            m_cmbTbl.Clear();
            m_cmbTbl.Add(EDSDKTypes.kEdsPropID_AEMode, this.AEModeCmb);
            m_cmbTbl.Add(EDSDKTypes.kEdsPropID_ISOSpeed, this.ISOSpeedCmb);
            m_cmbTbl.Add(EDSDKTypes.kEdsPropID_Av, this.AvCmb);
            m_cmbTbl.Add(EDSDKTypes.kEdsPropID_Tv, this.TvCmb);
            m_cmbTbl.Add(EDSDKTypes.kEdsPropID_MeteringMode, this.MeteringModeCmb);
            m_cmbTbl.Add(EDSDKTypes.kEdsPropID_ExposureCompensation, this.ExposureCompCmb);
            m_cmbTbl.Add(EDSDKTypes.kEdsPropID_ImageQuality, this.ImageQualityCmb);
        }
        private void PopulateCombo(int propertyID, IntPtr pm_Cam)
        {
            int err = 0;
            int iCnt = 0;
            int k =0;
            ComboBox cmb =(ComboBox) m_cmbTbl[propertyID];
            Hashtable propList =(Hashtable) CameraProperty.g_PropList[propertyID];
            string propStr = null;
            EDSDKTypes.EdsPropertyDesc desc = new EDSDKTypes.EdsPropertyDesc();
            ArrayList propValueList = new ArrayList();

            err = EDSDK.EdsGetPropertyDesc(pm_Cam, propertyID, ref desc);

            if (cmb == null)
            {
                return;
            }

            cmb.BeginUpdate();
            cmb.Items.Clear();
            for (iCnt = 0; iCnt <= desc.numElements - 1; iCnt++)
            {
                k = desc.propDesc[iCnt];
                if(propList.ContainsKey(desc.propDesc[iCnt]))
                {
                    propStr = propList[desc.propDesc[iCnt]].ToString();
                }
                if (propStr != null)
                {
                    err = cmb.Items.Add(propStr);
                    propValueList.Add(desc.propDesc[iCnt]);
                }
            }
            cmb.Tag = propValueList;
            getProperty(propertyID, pm_Cam);
            cmb.EndUpdate();
            if (cmb.Items.Count == 0)
            {
                cmb.Enabled = false;
                //// No available item.
            }
            else
            {
                cmb.Enabled = true;
            }
        }
        private int getProperty(int id, IntPtr pm_Cam)
        {
            int err = EDSDKErrors.EDS_ERR_OK;
            EDSDKTypes.EdsDataType dataType = EDSDKTypes.EdsDataType.kEdsDataType_Unknown;
            int dataSize = 0;
            IntPtr m_Cam = pm_Cam;

            if (id == EDSDKTypes.kEdsPropID_Unknown)
            {
                //// If the propertyID is invalidID,
                //// you should retry to get properties.
                //// InvalidID is able to be published for the models elder than EOS30D.

                if (err == EDSDKErrors.EDS_ERR_OK)
                {
                    err = getProperty(EDSDKTypes.kEdsPropID_AEMode, m_Cam);
                }
                if (err == EDSDKErrors.EDS_ERR_OK)
                {
                    err = getProperty(EDSDKTypes.kEdsPropID_Tv, m_Cam);
                }
                if (err == EDSDKErrors.EDS_ERR_OK)
                {
                    err = getProperty(EDSDKTypes.kEdsPropID_Av, m_Cam);
                }
                if (err == EDSDKErrors.EDS_ERR_OK)
                {
                    err = getProperty(EDSDKTypes.kEdsPropID_ISOSpeed, m_Cam);
                }
                if (err == EDSDKErrors.EDS_ERR_OK)
                {
                    err = getProperty(EDSDKTypes.kEdsPropID_ImageQuality, m_Cam);
                }

                return err;
            }

            //// Get propertysize.

            if (err == EDSDKErrors.EDS_ERR_OK)
            {
                err = EDSDK.EdsGetPropertySize(m_Cam, id, 0,ref dataType, ref dataSize);

            }


            if (err == EDSDKErrors.EDS_ERR_OK)
            {
                int data = 0;
                if (dataType == EDSDKTypes.EdsDataType.kEdsDataType_UInt32)
                {
                    //// Get a property.
                    IntPtr ptr = Marshal.AllocHGlobal(dataSize);

                    err = EDSDK.EdsGetPropertyData(m_Cam, id, 0, dataSize, ptr);

                    data =(int) Marshal.PtrToStructure(ptr, typeof(int));
                    Marshal.FreeHGlobal(ptr);
                    UpdateProperty(id, data);

                    if (err == EDSDKErrors.EDS_ERR_OK)
                    {
                        //MyBase.model.setPropertyUInt32(id, data)

                    }
                }


                if (dataType ==EDSDKTypes.EdsDataType.kEdsDataType_String)
                {
                    string str = null;
                    //char[EDS_MAX_NAME]
                    IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf(EDSDKTypes.EDS_MAX_NAME));

                    //// Get a property.
                    err =EDSDK.EdsGetPropertyData(m_Cam, id, 0, dataSize, ptr);

                    str = Marshal.PtrToStringAnsi(ptr);
                    Marshal.FreeHGlobal(ptr);
                    //
                    //// Stock the property .

                    if (err == EDSDKErrors.EDS_ERR_OK)
                    {


                    }
                }

            }


            //// Notify updating.

            if (err ==EDSDKErrors.EDS_ERR_OK)
            {


            }

            return err;

        }
        public void UpdateProperty(int propertyID, int data)
        {
            try
            {
                Hashtable propList = (Hashtable)CameraProperty.g_PropList[propertyID];
                switch (propertyID)
                {
                    case EDSDKTypes.kEdsPropID_AEMode:
                        AEModeCmb.Text = propList[data].ToString();
                        break;
                    case EDSDKTypes.kEdsPropID_ISOSpeed:
                        ISOSpeedCmb.Text = propList[data].ToString();
                        break;
                    case EDSDKTypes.kEdsPropID_MeteringMode:
                        MeteringModeCmb.Text = propList[data].ToString();
                        break;
                    case EDSDKTypes.kEdsPropID_Av:
                        AvCmb.Text = propList[data].ToString();
                        break;
                    case EDSDKTypes.kEdsPropID_Tv:
                        TvCmb.Text = propList[data].ToString();
                        break;
                    case EDSDKTypes.kEdsPropID_ExposureCompensation:
                        ExposureCompCmb.Text = propList[data].ToString();
                        break;
                    case EDSDKTypes.kEdsPropID_ImageQuality:
                        ImageQualityCmb.Text = propList[data].ToString();
                        break;
                }
            }
            catch (Exception ex)
            {
                string err = ex.Message;
            }
        }
        public bool SetProperty(IntPtr pm_cam, int propertyID, int data)
        {
            int err =EDSDKErrors.EDS_ERR_OK;
            bool locked = false;
            //// You should do UILock when you send a command to camera models elder than EOS30D.
            err = EDSDK.EdsSendStatusCommand(pm_cam,(int) EDSDKTypes.EdsCameraStatusCommand.kEdsCameraStatusCommand_UILock, 0);
            if (err ==EDSDKErrors.EDS_ERR_OK)
            {
                locked = true;

            }
            if (err == EDSDKErrors.EDS_ERR_OK)
            {
                err =EDSDK.EdsSetPropertyData(pm_cam, propertyID, 0, Marshal.SizeOf(data), data);

            }
            if (locked)
            {
                err =EDSDK.EdsSendStatusCommand(pm_cam,(int)EDSDKTypes.EdsCameraStatusCommand.kEdsCameraStatusCommand_UIUnLock, 0);

            }
            return true;
        }

        private void AEModeCmb_SelectionChangeCommitted(object sender, EventArgs e)
        {
            ComboBox cmb = (ComboBox)sender;
            // "sender" is the combobox
            ArrayList propValueList = (ArrayList)cmb.Tag;
            int data =(int) propValueList[cmb.SelectedIndex];

            if (cmbCameraCount.Text == "1")
            {
                SetProperty(m_cam1,EDSDKTypes.kEdsPropID_Av, data);
            }
            else
            {
                SetProperty(m_cam2, EDSDKTypes.kEdsPropID_Av, data);
            }
        }

        private void ISOSpeedCmb_SelectionChangeCommitted(object sender, EventArgs e)
        {
            ComboBox cmb = (ComboBox)sender;
            // "sender" is the combobox
            ArrayList propValueList = (ArrayList)cmb.Tag;
            int data = (int)propValueList[cmb.SelectedIndex];

            if (cmbCameraCount.Text == "1")
            {
                SetProperty(m_cam1, EDSDKTypes.kEdsPropID_ISOSpeed, data);
            }
            else
            {
                SetProperty(m_cam2, EDSDKTypes.kEdsPropID_ISOSpeed, data);
            }
        }

        private void MeteringModeCmb_SelectionChangeCommitted(object sender, EventArgs e)
        {
            ComboBox cmb = (ComboBox)sender;
            // "sender" is the combobox
            ArrayList propValueList = (ArrayList)cmb.Tag;
            int data = (int)propValueList[cmb.SelectedIndex];

            if (cmbCameraCount.Text == "1")
            {
                SetProperty(m_cam1, EDSDKTypes.kEdsPropID_MeteringMode, data);
            }
            else
            {
                SetProperty(m_cam2, EDSDKTypes.kEdsPropID_MeteringMode, data);
            }
        }

        private void ExposureCompCmb_SelectionChangeCommitted(object sender, EventArgs e)
        {
            ComboBox cmb = (ComboBox)sender;
            // "sender" is the combobox
            ArrayList propValueList = (ArrayList)cmb.Tag;
            int data =(int) propValueList[cmb.SelectedIndex];

            if (cmbCameraCount.Text == "1")
            {
                SetProperty(m_cam1, EDSDKTypes.kEdsPropID_ExposureCompensation, data);
            }
            else
            {
                SetProperty(m_cam2, EDSDKTypes.kEdsPropID_ExposureCompensation, data);
            }
        }

        private void ImageQualityCmb_SelectionChangeCommitted(object sender, EventArgs e)
        {
            ComboBox cmb = (ComboBox)sender;
            // "sender" is the combobox
            ArrayList propValueList = (ArrayList)cmb.Tag;
            int data =(int) propValueList[cmb.SelectedIndex];

            if (cmbCameraCount.Text == "1")
            {
                SetProperty(m_cam1, EDSDKTypes.kEdsPropID_ImageQuality, data);
            }
            else
            {
                SetProperty(m_cam2, EDSDKTypes.kEdsPropID_ImageQuality, data);
            }
        }

        private void cmdLiveView_Click(object sender, EventArgs e)
        {
            try
            {
                int i = 0;
                for (i = 0; i < numCams; i++)
                {
                    //get the only camera
                    if (i == 0)
                    {
                        camera.StartLiveView(this.picLivCamera1);
                    }
                    else
                    {
                        camera1.StartLiveView(this.picLivCamera2);
                    }
                }
                //System.Threading.Thread.Sleep(500);
                //if((picLivCamera1.Image != null) && (picLivCamera2.Image != null))
                //{
                    cmdFar1.Enabled = true;
                    cmdNear1.Enabled = true;
                    cmdFar2.Enabled = true;
                    cmdNear2.Enabled = true;
                    cmdTakePicture.Enabled = true;
                //}
            }
            catch(Exception ex)
            {
                MessageBox.Show("Error while starting the live view.......");
            }
        }

        private void frmBookScanning_Load(object sender, EventArgs e)
        {
            if (this.WindowState != FormWindowState.Minimized)
            {
                //panel2.Width = this.ClientSize.Width;
                panel2.Height = this.ClientSize.Height - 100;

                //pictureBox1.Width = this.ClientSize.Width;
                pictureBox1.Height = this.ClientSize.Height;

                Resolution objFormResizer = new Resolution();
                objFormResizer.ResizeForm(this, 864, 1152);
            }
            //this.Location = new Point(0, 0);
            //this.Size = Screen.PrimaryScreen.WorkingArea.Size;
            //this.WindowState = FormWindowState.Maximized;
            //FormResizer objFormResizer = new FormResizer();
            //objFormResizer.ResizeForm(this, System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Height, System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Width);
            
            //this.Height = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Height;
            //this.Width = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Width;
            DisplayValues();
            ShowPolicy();
            PrevImages();
            lblBatch.ForeColor = Color.RoyalBlue;
            lblBox.ForeColor = Color.RoyalBlue;
            lblCurrentPolicy.ForeColor = Color.RoyalBlue;
            lblNextPolicy.ForeColor = Color.RoyalBlue;
            lblPicSize.ForeColor = Color.RoyalBlue;
            picLarge.Visible = false;
            lblProjectName.ForeColor = Color.RoyalBlue;
        }

        protected override void OnHandleCreated(EventArgs e)
        {

            base.OnHandleCreated(e);

            this.Bounds = Screen.PrimaryScreen.WorkingArea;

        }

        private void cmdTakePicture_Click(object sender, EventArgs e)
        {
            int i = 0;
            try
            {
                //cmdTakePicture.Enabled = false;
                cmdAccept.Enabled = true;
                string tempPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase).Remove(0, 6) + "\\";
                string Camera1FileName = string.Empty;
                string Camera2FileName = string.Empty;
                string fileName = string.Empty;
                tempPath = tempPath + "\\" + "TempImg";
                if (!Directory.Exists(tempPath))
                {
                    Directory.CreateDirectory(tempPath);
                }
                fCount = System.IO.Directory.GetFiles(tempPath).Length;
                if (numCams > 1)
                {
                    fCount = fCount + 1;
                    Camera1FileName = tempPath + "\\" + "Img_" + fCount + ".jpg";
                    camera.TakePicture(Camera1FileName, picLivCamera1, m_cam1);
                    Camera2FileName = tempPath + "\\" + "Img_" + (fCount+1) + ".jpg";
                    camera1.TakePicture(Camera2FileName, picLivCamera2, m_cam2);

                    //camera.FlushTransferQueue();
                    //camera1.FlushTransferQueue();

                    //Thread.Sleep(1000);
                    i++;
                    camera.StartLiveView(picLivCamera1);
                    //Thread.Sleep(1000);
                    i++;
                    camera1.StartLiveView(picLivCamera2);

                    i++;
                    tmpImg.LoadBitmapFromFile(Camera1FileName);
                    tmpImg.SaveFile(tempPath + "\\" + fCount + "_Img.tiff");
                    picStillCamera1.Image = tmpImg.GetBitmap();
                    File.Delete(Camera1FileName);
                    picStillCamera1.Tag = tempPath + "\\" + (fCount) + "_Img.tiff";

                    i++;
                    tmpImg.LoadBitmapFromFile(Camera2FileName);
                    tmpImg.SaveFile(tempPath + "\\" + (fCount + 1) + "_Img.tiff");
                    picStillCamera2.Image = tmpImg.GetBitmap();
                    File.Delete(Camera2FileName);
                    picStillCamera2.Tag = tempPath + "\\" + (fCount+1) + "_Img.tiff";
                }
                else
                {
                    fCount = fCount + 1;
                    fileName = tempPath + "\\" + "Img_" + fCount + ".jpg";
                    camera.TakePicture(fileName, picLivCamera1, m_cam1);
                    camera.FlushTransferQueue();
                    camera.StartLiveView(picLivCamera1);
                    tmpImg.LoadBitmapFromFile(fileName);
                    tmpImg.SaveAsTiff(tempPath + "\\" + fCount + "_Img.tiff", IGRComressionTIFF.JPEG);
                    picStillCamera1.Image = tmpImg.GetBitmap();
                    File.Delete(fileName);
                    picStillCamera1.Tag = tempPath + "\\" + fCount + "_Img.tiff";
                }
            }
            catch
            {
                MessageBox.Show("Error while taking picture........" + i.ToString());
                cmdAccept.Enabled = false;
                cmdTakePicture.Enabled = true;
                picStillCamera1.Image = null;
                picStillCamera2.Image = null;
            }
        }

        private void cmdAccept_Click(object sender, EventArgs e)
        {
            char leftPad = Convert.ToChar("0");
            //For showing the next and current policy
            
            
            scanFolder = batchPath + "\\" + wBox.ctrlBox.BoxNumber.ToString() + "\\" + lblCurrentPolicy.Text + "\\" + ihConstants._SCAN_FOLDER;
            if (FileorFolder.CreateFolder(scanFolder) == true)
            {
                CtrlImage pImage = new CtrlImage(Convert.ToInt32(wBox.ctrlBox.ProjectCode), Convert.ToInt32(wBox.ctrlBox.BatchKey), wBox.ctrlBox.BoxNumber.ToString(), lblCurrentPolicy.Text.ToString(), string.Empty, string.Empty);
                wfeImage wImage = new wfeImage(sqlCon, pImage);

                for (int camCount = 0; camCount < 2; camCount++)
                {
                    int imageCount = wImage.GetImageCount() + 1;
                    string tifFileName = scanFolder + "\\" + lblCurrentPolicy.Text + "_" + imageCount.ToString().PadLeft(3, leftPad) + "_" + "A" + ".TIF";
                    if (camCount == 0)
                    {
                        File.Move(picStillCamera1.Tag.ToString(), tifFileName);
                    }
                    else
                    {
                        File.Move(picStillCamera2.Tag.ToString(), tifFileName);
                    }
                    SetListboxValue(lblCurrentPolicy.Text + "_" + imageCount.ToString().PadLeft(3, leftPad) + "_" + "A" + ".TIF", imageCount);
                }
            }
            cmdTakePicture.Enabled = true;
            cmdAccept.Enabled = false;
            picStillCamera1.Image = null;
            picStillCamera2.Image = null;
        }
        void SetListboxValue(string prmIamgeName, int prmSrlNo)
        {
            CtrlImage ctrlImg;
            //Credentials crd=new Credentials();
            long fileSize;
            System.IO.FileInfo info = new System.IO.FileInfo(scanFolder + "\\" + prmIamgeName);

            fileSize = info.Length;
            fileSize = fileSize / 1024;
            
            wfeImage img;
            lstImageName.Items.Add(prmIamgeName);
            lstImageName.Refresh();
            ctrlImg = new CtrlImage(wBox.ctrlBox.ProjectCode, wBox.ctrlBox.BatchKey, wBox.ctrlBox.BoxNumber.ToString(), lblCurrentPolicy.Text.ToString(), prmIamgeName, string.Empty);
            img = new wfeImage(sqlCon, ctrlImg);
            img.Save(crd, eSTATES.PAGE_SCANNED, fileSize, ihConstants._NORMAL_PAGE, prmSrlNo, prmIamgeName);
        }

        private void frmBookScanning_FormClosing(object sender, FormClosingEventArgs e)
        {
            DisConnectCameras();
        }

        private void DisConnectCameras()
        {
            if (cmbCameraCount.Items.Count > 0)
            {
                if (camera != null)
                {
                    camera.IDispose();
                }
                if (camera1 != null)
                {
                    camera1.IDispose();
                }
                EDSDK.EdsTerminateSDK();
            }
        }

        private void cmdDisconnect_Click(object sender, EventArgs e)
        {
            DisConnectCameras();
            cmdConnect.Enabled = true;
            cmdDisconnect.Enabled = false;
            cmdLiveView.Enabled = false;
            cmdTakePicture.Enabled = false;
            cmdAccept.Enabled = false;
            cmdFar1.Enabled = false;
            cmdFar2.Enabled = false;
            cmdNear1.Enabled = false;
            cmdNear2.Enabled = false;
            grpCameraControl.Enabled = false;
        }

        private void cmdNear1_Click(object sender, EventArgs e)
        {
            int p;
            if (camera1Near == 1)
            {
                p = EDSDK.EdsSendCommand(m_cam1,(int)EDSDKTypes.EdsCameraCommand.kEdsCameraCommand_DriveLensEvf, EDSDKTypes.EvfDriveLens_Near1);
            }
            else if (camera1Near == 2)
            {
                p = EDSDK.EdsSendCommand(m_cam1, (int)EDSDKTypes.EdsCameraCommand.kEdsCameraCommand_DriveLensEvf, EDSDKTypes.EvfDriveLens_Near2);
            }
            else
            {
                p = EDSDK.EdsSendCommand(m_cam1, (int)EDSDKTypes.EdsCameraCommand.kEdsCameraCommand_DriveLensEvf, EDSDKTypes.EvfDriveLens_Near3);
            }
            camera1Near = camera1Near + 1;
        }

        private void cmdFar1_Click(object sender, EventArgs e)
        {
            int p;
            if (camera1Far == 1)
            {
                p = EDSDK.EdsSendCommand(m_cam1, (int)EDSDKTypes.EdsCameraCommand.kEdsCameraCommand_DriveLensEvf, EDSDKTypes.EvfDriveLens_Far1);
            }
            else if (camera1Far == 2)
            {
                p = EDSDK.EdsSendCommand(m_cam1, (int)EDSDKTypes.EdsCameraCommand.kEdsCameraCommand_DriveLensEvf, EDSDKTypes.EvfDriveLens_Far2);
            }
            else
            {
                p = EDSDK.EdsSendCommand(m_cam1, (int)EDSDKTypes.EdsCameraCommand.kEdsCameraCommand_DriveLensEvf, EDSDKTypes.EvfDriveLens_Far3);
            }

            camera1Far = camera1Far + 1;
        }

        private void cmdNear2_Click(object sender, EventArgs e)
        {
            int p;
            if (camera2Near == 1)
            {
                p = EDSDK.EdsSendCommand(m_cam2, (int)EDSDKTypes.EdsCameraCommand.kEdsCameraCommand_DriveLensEvf, EDSDKTypes.EvfDriveLens_Near1);
            }
            else if (camera2Near == 2)
            {
                p = EDSDK.EdsSendCommand(m_cam2, (int)EDSDKTypes.EdsCameraCommand.kEdsCameraCommand_DriveLensEvf, EDSDKTypes.EvfDriveLens_Near2);
            }
            else
            {
                p = EDSDK.EdsSendCommand(m_cam2, (int)EDSDKTypes.EdsCameraCommand.kEdsCameraCommand_DriveLensEvf, EDSDKTypes.EvfDriveLens_Near3);
            }

            camera2Near = camera2Near + 1;
        }

        private void cmdFar2_Click(object sender, EventArgs e)
        {
            int p;
            if (camera2Far == 1)
            {
                p = EDSDK.EdsSendCommand(m_cam2, (int)EDSDKTypes.EdsCameraCommand.kEdsCameraCommand_DriveLensEvf, EDSDKTypes.EvfDriveLens_Far1);
            }
            else if (camera2Far == 2)
            {
                p = EDSDK.EdsSendCommand(m_cam2, (int)EDSDKTypes.EdsCameraCommand.kEdsCameraCommand_DriveLensEvf, EDSDKTypes.EvfDriveLens_Far2);
            }
            else
            {
                p = EDSDK.EdsSendCommand(m_cam2, (int)EDSDKTypes.EdsCameraCommand.kEdsCameraCommand_DriveLensEvf, EDSDKTypes.EvfDriveLens_Far3);
            }
            camera2Far = camera2Far + 1;
        }

        
        private void frmBookScanning_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                picStillCamera1.Location = new Point(12, 450);
                picStillCamera1.Height = 404;
                picStillCamera1.Width = 343;
                picStillCamera1.SendToBack();
            }
        }
        private void picStillCamera1_DoubleClick(object sender, EventArgs e)
        {
            if ((picStillCamera1.Image != null) && (picStillCamera1.Tag.ToString() != string.Empty))
            {
                picLarge.Visible = true;
                picLarge.Dock = DockStyle.Fill;
                tmpImg.LoadBitmapFromFile(picStillCamera1.Tag.ToString());
                ChangeZoomSize(picStillCamera1.Tag.ToString(), picLarge, tmpImg);
            }
        }

        private void picStillCamera2_DoubleClick(object sender, EventArgs e)
        {
            if ((picStillCamera2.Image != null) && (picStillCamera1.Tag.ToString() != string.Empty))
            {
                picLarge.Visible = true;
                picLarge.Dock = DockStyle.Fill;
                tmpImg.LoadBitmapFromFile(picStillCamera2.Tag.ToString());
                ChangeZoomSize(picStillCamera2.Tag.ToString(), picLarge, tmpImg);
            }
        }

        private void picLarge_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                picLarge.Image = null;
                picLarge.Dock = DockStyle.None;
                picLarge.Visible = false;
            }
        }

        private void picLarge_MouseClick(object sender, MouseEventArgs e)
        {
            
            picLarge.Image = null;
            picLarge.Dock = DockStyle.None;
            picLarge.Visible = false;
        
        }

        private void cmdFinished_Click(object sender, EventArgs e)
        {
            CtrlPolicy ctrPolCurrent = null;
            CtrlPolicy ctrPolNext = null;
            updatedPolCount = updatedPolCount + 1;
            scanDate = dbcon.GetCurrenctDTTM(1, sqlCon);
            pPolicy = new CtrlPolicy(wBox.ctrlBox.ProjectCode, wBox.ctrlBox.BatchKey, wBox.ctrlBox.BoxNumber.ToString(), lblCurrentPolicy.Text.ToString());
            wfePolicy wPolicy = new wfePolicy(sqlCon, pPolicy);
            crd.created_dttm = scanDate;
            wPolicy.UpdateStatus(eSTATES.POLICY_SCANNED, crd);

            ///insert into transaction log
            wPolicy.UpdateTransactionLog(eSTATES.POLICY_SCANNED, crd);

            wPolicy.UpdateScanDetails(scanDate, ihConstants.SCAN_SUCCESS_FLAG);
            pBox = new CtrlBox(wBox.ctrlBox.ProjectCode, wBox.ctrlBox.BatchKey, wBox.ctrlBox.BoxNumber.ToString());
            wfeBox box = new wfeBox(sqlCon, pBox);
            NovaNet.wfe.eSTATES[] state = new NovaNet.wfe.eSTATES[1];
            state[0] = NovaNet.wfe.eSTATES.POLICY_CREATED;
            if (wPolicy.GetPolicyCount(state) == 0)
            {
                box.UpdateStatus(eSTATES.BOX_SCANNED);
            }
            if ((policyList.Count > (updatedPolCount+1)))
            {
                ctrPolCurrent = (CtrlPolicy)policyList[updatedPolCount];
                ctrPolNext = (CtrlPolicy)policyList[updatedPolCount+1];
            }
            else if (policyList.Count == (updatedPolCount+1))
            {
                ctrPolCurrent = (CtrlPolicy)policyList[updatedPolCount];
                ctrPolNext = (CtrlPolicy)policyList[updatedPolCount];
            }
            else
            {
                MessageBox.Show("No more policies are ready to be scanned....");
                cmdAccept.Enabled = false;
                cmdTakePicture.Enabled = false;
                cmdLiveView.Enabled = false;
                cmdConnect.Enabled = false;
                cmdDisconnect.Enabled = false;
                cmdFar1.Enabled = false;
                cmdNear1.Enabled = false;
                cmdFar2.Enabled = false;
                cmdNear2.Enabled = false;
                grpCameraControl.Enabled = false;
                picLivCamera1.Image = null;
                picLivCamera2.Image = null;
                picStillCamera1.Image = null;
                picStillCamera2.Image = null;
                return;
            }
            lblCurrentPolicy.Text = ctrPolCurrent.PolicyNumber.ToString();
            lblNextPolicy.Text = ctrPolNext.PolicyNumber.ToString();
            lblCurrentPolicy.Refresh();
            lblNextPolicy.Refresh();
            this.Text = "Batch Scanning            " + " Project- " + lblProjectName.Text + "  Batch- " + lblBatch.Text + "  Box- " + lblBox.Text + " Current Policy- " + lblCurrentPolicy.Text + " Next Policy- " + lblNextPolicy.Text;
            cmdAccept.Enabled = false;
            cmdTakePicture.Enabled = false;
            cmdLiveView.Enabled = false;
            cmdConnect.Enabled = true;
            cmdDisconnect.Enabled = false;
            cmdFar1.Enabled = false;
            cmdNear1.Enabled = false;
            cmdFar2.Enabled = false;
            cmdNear2.Enabled = false;
            grpCameraControl.Enabled = false;
            picStillCamera1.Image = null;
            picStillCamera2.Image = null;
            lstImageName.Items.Clear();
            //DisConnectCameras();
        }

        private void picLivCamera1_Click(object sender, EventArgs e)
        {

            //picLarge.Visible = true;
            //camera.StopLiveView();
            //camera.StartLiveView(picLarge);
            picLivCamera1.Location = new Point(0,0);
            picLivCamera1.Height = 950;
            picLivCamera1.Width = 863;
            picLivCamera1.BringToFront();

        }

        private void picLivCamera1_DoubleClick(object sender, EventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void picLarge_Click(object sender, EventArgs e)
        {
            //picLarge.Visible = false;
            //camera.StopLiveView();
            //camera.StartLiveView(this.picLivCamera1);
        }

        private void picLivCamera2_Click(object sender, EventArgs e)
        {
            picLivCamera2.Location = new Point(0, 0);
            picLivCamera2.Height = 950;
            picLivCamera2.Width = 863;
            picLivCamera2.BringToFront();
        }

        private void picLivCamera1_MouseLeave(object sender, EventArgs e)
        {
            picLivCamera1.Location = new Point(12, 6);
            picLivCamera1.Height = 406;
            picLivCamera1.Width = 397;
            picLivCamera1.SendToBack();
        }

        private void picLivCamera2_MouseLeave(object sender, EventArgs e)
        {
            picLivCamera2.Location = new Point(424, 6);
            picLivCamera2.Height = 406;
            picLivCamera2.Width = 397;
            picLivCamera2.SendToBack();
        }

        private void picStillCamera1_Click(object sender, EventArgs e)
        {
            picStillCamera1.Location = new Point(0, 0);
            picStillCamera1.Height = 950;
            picStillCamera1.Width = 863;
            picStillCamera1.BringToFront();
        }

        private void picStillCamera1_MouseLeave(object sender, EventArgs e)
        {
            picStillCamera1.Location = new Point(12, 450);
            picStillCamera1.Height = 404;
            picStillCamera1.Width = 343;
            picStillCamera1.SendToBack();
        }

        private void picStillCamera2_Click(object sender, EventArgs e)
        {
            picStillCamera2.Location = new Point(0, 0);
            picStillCamera2.Height = 950;
            picStillCamera2.Width = 863;
            picStillCamera2.BringToFront();
        }

        private void picStillCamera2_MouseLeave(object sender, EventArgs e)
        {
            picStillCamera2.Location = new Point(424, 450);
            picStillCamera2.Height = 404;
            picStillCamera2.Width = 343;
            picStillCamera2.SendToBack();
        }

        private void frmBookScanning_Resize(object sender, EventArgs e)
        {
            
        }

        private void frmBookScanning_KeyUp(object sender, KeyEventArgs e)
        {

        }

        private void frmBookScanning_KeyPress(object sender, KeyPressEventArgs e)
        {

        }


    }
}
