using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Windows.Forms;
using IMAPI2.Interop;
using IMAPI2.MediaItem;
using System.Data;
using System.Data.Odbc;
using Excel = Microsoft.Office.Interop.Excel;
using System.Data.SqlClient;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using Ionic.Zip;
using NovaNet.Utils;
using LItems;


namespace BurnMedia
{

    public partial class MainForm : Form
    {
        private const string ClientName = "BurnMedia";

        Int64 _totalDiscSize;
        private OdbcConnection con;
        private bool _isBurning;
        private bool _isFormatting;
        private IMAPI_BURN_VERIFICATION_LEVEL _verificationLevel = 
            IMAPI_BURN_VERIFICATION_LEVEL.IMAPI_BURN_VERIFICATION_NONE;
        private bool _closeMedia;
        private bool _ejectMedia;
        DataTable dtDd = new DataTable();
        DataTable dtIn1 = new DataTable();
        DataTable dtIn2 = new DataTable();
        DataTable dtOp = new DataTable();
        DataTable dtOk = new DataTable();
        string appPath = System.IO.Path.GetDirectoryName(Application.ExecutablePath);
        string sqlIp = string.Empty;
        private BurnData _burnData = new BurnData();

        public MainForm(OdbcConnection pCon)
        {
            InitializeComponent();
            con = pCon;
            ReadINI();
        }

        /// <summary>
        /// Initialize the form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainForm_Load(object sender, EventArgs e)
        {
            //
            // Determine the current recording devices
            //
            //MsftDiscMaster2 discMaster = null;
            try
            {
                //discMaster = new MsftDiscMaster2();

                //if (!discMaster.IsSupportedEnvironment)
                //    return;
                //foreach (string uniqueRecorderId in discMaster)
                //{
                //    var discRecorder2 = new MsftDiscRecorder2();
                //    discRecorder2.InitializeDiscRecorder(uniqueRecorderId);

                //    devicesComboBox.Items.Add(discRecorder2);
                //}
                //if (devicesComboBox.Items.Count > 0)
                //{
                //    //devicesComboBox.SelectedIndex = 0;
                //}
                populateRunNum();
                
            }
            catch (COMException ex)
            {
                MessageBox.Show(ex.Message,
                    string.Format("Error:{0} - Please install IMAPI2", ex.ErrorCode),
                    MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }
            finally
            {
                //if (discMaster != null)
                //{
                //    Marshal.ReleaseComObject(discMaster);
                //}
            }


            //
            // Create the volume label based on the current date
            //
            

            labelStatusText.Text = string.Empty;
            labelFormatStatusText.Text = string.Empty;

            //
            // Select no verification, by default
            //
            comboBoxVerification.SelectedIndex = 0;

            UpdateCapacity();
        }
        private void populateRunNum()
        {
            DataSet ds = new DataSet();
            ds = GetRunnumBurn();
            if (ds.Tables[0].Rows.Count > 0)
            {
                cmbVol.DataSource = ds.Tables[0];
                cmbVol.DisplayMember = "run_no";
                cmbVol.ValueMember = "run_no";
            }
        }
        public DataSet GetRunnumBurn()
        {
            string sql = "select distinct run_no,status from batch_master where run_no is not null and status ='7'";
            DataSet ds = new DataSet();
            OdbcDataAdapter odap = new OdbcDataAdapter(sql, con);
            odap.Fill(ds);
            return ds;
        }
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            //
            // Release the disc recorder items
            //
            foreach (MsftDiscRecorder2 discRecorder2 in devicesComboBox.Items)
            {
                if (discRecorder2 != null)
                {
                    Marshal.ReleaseComObject(discRecorder2);
                }
            }
        }



        #region Device ComboBox
        /// <summary>
        /// Selected a new device
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void devicesComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (devicesComboBox.SelectedIndex == -1)
            {
                return;
            }

            var discRecorder =
                (IDiscRecorder2)devicesComboBox.Items[devicesComboBox.SelectedIndex];

            supportedMediaLabel.Text = string.Empty;

            //
            // Verify recorder is supported
            //
            IDiscFormat2Data discFormatData = null;
            try
            {
                discFormatData = new MsftDiscFormat2Data();
                if (!discFormatData.IsRecorderSupported(discRecorder))
                {
                    MessageBox.Show("Recorder not supported", ClientName,
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                StringBuilder supportedMediaTypes = new StringBuilder();
                foreach (IMAPI_PROFILE_TYPE profileType in discRecorder.SupportedProfiles)
                {
                    string profileName = GetProfileTypeString(profileType);

                    if (string.IsNullOrEmpty(profileName))
                        continue;

                    if (supportedMediaTypes.Length > 0)
                        supportedMediaTypes.Append(", ");
                    supportedMediaTypes.Append(profileName);
                }

                supportedMediaLabel.Text = supportedMediaTypes.ToString();
            }
            catch (COMException)
            {
                supportedMediaLabel.Text = "Error getting supported types";
            }
            finally
            {
                if (discFormatData != null)
                {
                    Marshal.ReleaseComObject(discFormatData);
                }
            }
        }

        /// <summary>
        /// converts an IMAPI_MEDIA_PHYSICAL_TYPE to it's string
        /// </summary>
        /// <param name="mediaType"></param>
        /// <returns></returns>
        private static string GetMediaTypeString(IMAPI_MEDIA_PHYSICAL_TYPE mediaType)
        {
            switch (mediaType)
            {
                case IMAPI_MEDIA_PHYSICAL_TYPE.IMAPI_MEDIA_TYPE_UNKNOWN:
                default:
                    return "Unknown Media Type";

                case IMAPI_MEDIA_PHYSICAL_TYPE.IMAPI_MEDIA_TYPE_CDROM:
                    return "CD-ROM";

                case IMAPI_MEDIA_PHYSICAL_TYPE.IMAPI_MEDIA_TYPE_CDR:
                    return "CD-R";

                case IMAPI_MEDIA_PHYSICAL_TYPE.IMAPI_MEDIA_TYPE_CDRW:
                    return "CD-RW";

                case IMAPI_MEDIA_PHYSICAL_TYPE.IMAPI_MEDIA_TYPE_DVDROM:
                    return "DVD ROM";

                case IMAPI_MEDIA_PHYSICAL_TYPE.IMAPI_MEDIA_TYPE_DVDRAM:
                    return "DVD-RAM";

                case IMAPI_MEDIA_PHYSICAL_TYPE.IMAPI_MEDIA_TYPE_DVDPLUSR:
                    return "DVD+R";

                case IMAPI_MEDIA_PHYSICAL_TYPE.IMAPI_MEDIA_TYPE_DVDPLUSRW:
                    return "DVD+RW";

                case IMAPI_MEDIA_PHYSICAL_TYPE.IMAPI_MEDIA_TYPE_DVDPLUSR_DUALLAYER:
                    return "DVD+R Dual Layer";

                case IMAPI_MEDIA_PHYSICAL_TYPE.IMAPI_MEDIA_TYPE_DVDDASHR:
                    return "DVD-R";

                case IMAPI_MEDIA_PHYSICAL_TYPE.IMAPI_MEDIA_TYPE_DVDDASHRW:
                    return "DVD-RW";

                case IMAPI_MEDIA_PHYSICAL_TYPE.IMAPI_MEDIA_TYPE_DVDDASHR_DUALLAYER:
                    return "DVD-R Dual Layer";

                case IMAPI_MEDIA_PHYSICAL_TYPE.IMAPI_MEDIA_TYPE_DISK:
                    return "random-access writes";

                case IMAPI_MEDIA_PHYSICAL_TYPE.IMAPI_MEDIA_TYPE_DVDPLUSRW_DUALLAYER:
                    return "DVD+RW DL";

                case IMAPI_MEDIA_PHYSICAL_TYPE.IMAPI_MEDIA_TYPE_HDDVDROM:
                    return "HD DVD-ROM";

                case IMAPI_MEDIA_PHYSICAL_TYPE.IMAPI_MEDIA_TYPE_HDDVDR:
                    return "HD DVD-R";

                case IMAPI_MEDIA_PHYSICAL_TYPE.IMAPI_MEDIA_TYPE_HDDVDRAM:
                    return "HD DVD-RAM";

                case IMAPI_MEDIA_PHYSICAL_TYPE.IMAPI_MEDIA_TYPE_BDROM:
                    return "Blu-ray DVD (BD-ROM)";

                case IMAPI_MEDIA_PHYSICAL_TYPE.IMAPI_MEDIA_TYPE_BDR:
                    return "Blu-ray media";

                case IMAPI_MEDIA_PHYSICAL_TYPE.IMAPI_MEDIA_TYPE_BDRE:
                    return "Blu-ray Rewritable media";
            }
        }

        /// <summary>
        /// converts an IMAPI_PROFILE_TYPE to it's string
        /// </summary>
        /// <param name="profileType"></param>
        /// <returns></returns>
        static string GetProfileTypeString(IMAPI_PROFILE_TYPE profileType)
        {
            switch (profileType)
            {
                default:
                    return string.Empty;

                case IMAPI_PROFILE_TYPE.IMAPI_PROFILE_TYPE_CD_RECORDABLE:
                    return "CD-R";

                case IMAPI_PROFILE_TYPE.IMAPI_PROFILE_TYPE_CD_REWRITABLE:
                    return "CD-RW";

                case IMAPI_PROFILE_TYPE.IMAPI_PROFILE_TYPE_DVDROM:
                    return "DVD ROM";

                case IMAPI_PROFILE_TYPE.IMAPI_PROFILE_TYPE_DVD_DASH_RECORDABLE:
                    return "DVD-R";

                case IMAPI_PROFILE_TYPE.IMAPI_PROFILE_TYPE_DVD_RAM:
                    return "DVD-RAM";

                case IMAPI_PROFILE_TYPE.IMAPI_PROFILE_TYPE_DVD_PLUS_R:
                    return "DVD+R";

                case IMAPI_PROFILE_TYPE.IMAPI_PROFILE_TYPE_DVD_PLUS_RW:
                    return "DVD+RW";

                case IMAPI_PROFILE_TYPE.IMAPI_PROFILE_TYPE_DVD_PLUS_R_DUAL:
                    return "DVD+R Dual Layer";

                case IMAPI_PROFILE_TYPE.IMAPI_PROFILE_TYPE_DVD_DASH_REWRITABLE:
                    return "DVD-RW";

                case IMAPI_PROFILE_TYPE.IMAPI_PROFILE_TYPE_DVD_DASH_RW_SEQUENTIAL:
                    return "DVD-RW Sequential";

                case IMAPI_PROFILE_TYPE.IMAPI_PROFILE_TYPE_DVD_DASH_R_DUAL_SEQUENTIAL:
                    return "DVD-R DL Sequential";

                case IMAPI_PROFILE_TYPE.IMAPI_PROFILE_TYPE_DVD_DASH_R_DUAL_LAYER_JUMP:
                    return "DVD-R Dual Layer";

                case IMAPI_PROFILE_TYPE.IMAPI_PROFILE_TYPE_DVD_PLUS_RW_DUAL:
                    return "DVD+RW DL";

                case IMAPI_PROFILE_TYPE.IMAPI_PROFILE_TYPE_HD_DVD_ROM:
                    return "HD DVD-ROM";

                case IMAPI_PROFILE_TYPE.IMAPI_PROFILE_TYPE_HD_DVD_RECORDABLE:
                    return "HD DVD-R";

                case IMAPI_PROFILE_TYPE.IMAPI_PROFILE_TYPE_HD_DVD_RAM:
                    return "HD DVD-RAM";

                case IMAPI_PROFILE_TYPE.IMAPI_PROFILE_TYPE_BD_ROM:
                    return "Blu-ray DVD (BD-ROM)";

                case IMAPI_PROFILE_TYPE.IMAPI_PROFILE_TYPE_BD_R_SEQUENTIAL:
                    return "Blu-ray media Sequential";

                case IMAPI_PROFILE_TYPE.IMAPI_PROFILE_TYPE_BD_R_RANDOM_RECORDING:
                    return "Blu-ray media";

                case IMAPI_PROFILE_TYPE.IMAPI_PROFILE_TYPE_BD_REWRITABLE:
                    return "Blu-ray Rewritable media";
            }
        }

        /// <summary>
        /// Provides the display string for an IDiscRecorder2 object in the combobox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void devicesComboBox_Format(object sender, ListControlConvertEventArgs e)
        {
            IDiscRecorder2 discRecorder2 = (IDiscRecorder2)e.ListItem;
            string devicePaths = string.Empty;
            string volumePath = (string)discRecorder2.VolumePathNames.GetValue(0);
            foreach (string volPath in discRecorder2.VolumePathNames)
            {
                if (!string.IsNullOrEmpty(devicePaths))
                {
                    devicePaths += ",";
                }
                devicePaths += volumePath;
            }

            e.Value = string.Format("{0} [{1}]", devicePaths, discRecorder2.ProductId);
        }
        #endregion


        #region Media Size

        private void buttonDetectMedia_Click(object sender, EventArgs e)
        {
            if (devicesComboBox.SelectedIndex == -1)
            {
                return;
            }

            var discRecorder =
                (IDiscRecorder2)devicesComboBox.Items[devicesComboBox.SelectedIndex];

            MsftFileSystemImage fileSystemImage = null;
            MsftDiscFormat2Data discFormatData = null;

            try
            {
                //
                // Create and initialize the IDiscFormat2Data
                //
                discFormatData = new MsftDiscFormat2Data();
                if (!discFormatData.IsCurrentMediaSupported(discRecorder))
                {
                    labelMediaType.Text = "Media not supported!";
                    _totalDiscSize = 0;
                    return;
                }
                else
                {
                    //
                    // Get the media type in the recorder
                    //
                    discFormatData.Recorder = discRecorder;
                    IMAPI_MEDIA_PHYSICAL_TYPE mediaType = discFormatData.CurrentPhysicalMediaType;
                    labelMediaType.Text = GetMediaTypeString(mediaType);

                    //
                    // Create a file system and select the media type
                    //
                    fileSystemImage = new MsftFileSystemImage();
                    fileSystemImage.ChooseImageDefaultsForMediaType(mediaType);

                    //
                    // See if there are other recorded sessions on the disc
                    //
                    if (!discFormatData.MediaHeuristicallyBlank)
                    {
                        fileSystemImage.MultisessionInterfaces = discFormatData.MultisessionInterfaces;
                        fileSystemImage.ImportFileSystem();
                    }

                    Int64 freeMediaBlocks = fileSystemImage.FreeMediaBlocks;
                    _totalDiscSize = 2048 * freeMediaBlocks;
                }
            }
            catch (COMException exception)
            {
                MessageBox.Show(this, exception.Message, "Detect Media Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (discFormatData != null)
                {
                    Marshal.ReleaseComObject(discFormatData);
                }

                if (fileSystemImage != null)
                {
                    Marshal.ReleaseComObject(fileSystemImage);
                }
            }


            UpdateCapacity();
        }

        /// <summary>
        /// Updates the capacity progressbar
        /// </summary>
        private void UpdateCapacity()
        {
            //
            // Get the text for the Max Size
            //
            if (_totalDiscSize == 0)
            {
                labelTotalSize.Text = "0MB";
                return;
            }
            
            labelTotalSize.Text = _totalDiscSize < 1000000000 ?
                string.Format("{0}MB", _totalDiscSize / 1000000) :
                string.Format("{0:F2}GB", (float)_totalDiscSize / 1000000000.0);

            //
            // Calculate the size of the files
            //
            Int64 totalMediaSize = 0;
            foreach (IMediaItem mediaItem in listBoxFiles.Items)
            {
                totalMediaSize += mediaItem.SizeOnDisc;
            }

            if (totalMediaSize == 0)
            {
                progressBarCapacity.Value = 0;
                progressBarCapacity.ForeColor = SystemColors.Highlight;
            }
            else
            {
                var percent = (int)((totalMediaSize * 100) / _totalDiscSize);
                if (percent > 100)
                {
                    progressBarCapacity.Value = 100;
                    progressBarCapacity.ForeColor = Color.Red;
                }
                else
                {
                    progressBarCapacity.Value = percent;
                    progressBarCapacity.ForeColor = SystemColors.Highlight;
                }
            }
        }

        #endregion


        #region Burn Media Process

        /// <summary>
        /// User clicked the "Burn" button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonBurn_Click(object sender, EventArgs e)
        {
            
            if (devicesComboBox.SelectedIndex == -1)
            {
                MessageBox.Show("No Recorder Selected...");
                devicesComboBox.Focus();
                return;
            }
            
            if (_isBurning)
            {
                buttonBurn.Enabled = false;
                backgroundBurnWorker.CancelAsync();
            }
            else
            {
                if (excelExport() == true)
                {
                    _isBurning = true;
                    _closeMedia = checkBoxCloseMedia.Checked;
                    _ejectMedia = checkBoxEject.Checked;
                    int cdNo = Convert.ToInt32(getCDNo().Tables[0].Rows[0][0].ToString());
                    EnableBurnUI(false);

                    var discRecorder =
                        (IDiscRecorder2)devicesComboBox.Items[devicesComboBox.SelectedIndex];
                    _burnData.uniqueRecorderId = discRecorder.ActiveDiscRecorder;

                    backgroundBurnWorker.RunWorkerAsync(_burnData);
                    UpdateCDCVD(cdNo.ToString());
                }
                else
                {
                    MessageBox.Show("There Is Some Error,Please try Again After Some Time");
                    return;
                }
            }
        }

        /// <summary>
        /// The thread that does the burning of the media
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void backgroundBurnWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            MsftDiscRecorder2 discRecorder = null;
            MsftDiscFormat2Data discFormatData = null;

            try
            {
                //
                // Create and initialize the IDiscRecorder2 object
                //
                discRecorder = new MsftDiscRecorder2();
                var burnData = (BurnData)e.Argument;
                discRecorder.InitializeDiscRecorder(burnData.uniqueRecorderId);

                //
                // Create and initialize the IDiscFormat2Data
                //
                discFormatData = new MsftDiscFormat2Data
                    {
                        Recorder = discRecorder,
                        ClientName = ClientName,
                        ForceMediaToBeClosed = _closeMedia
                    };
                
                //
                // Set the verification level
                //
                //////var burnVerification = (IBurnVerification)discFormatData;
                //////burnVerification.BurnVerificationLevel = _verificationLevel;

                //
                // Check if media is blank, (for RW media)
                //
                object[] multisessionInterfaces = null;
                if (!discFormatData.MediaHeuristicallyBlank)
                {
                    multisessionInterfaces = discFormatData.MultisessionInterfaces;
                }

                //
                // Create the file system
                //
                IStream fileSystem;
                if (!CreateMediaFileSystem(discRecorder, multisessionInterfaces, out fileSystem))
                {
                    e.Result = -1;
                    return;
                }

                //
                // add the Update event handler
                //
                discFormatData.Update += discFormatData_Update;

                //
                // Write the data here
                //
                try
                {
                    discFormatData.Write(fileSystem);
                    e.Result = 0;
                }
                catch (COMException ex)
                {
                    e.Result = ex.ErrorCode;
                    MessageBox.Show(ex.Message, "IDiscFormat2Data.Write failed",
                        MessageBoxButtons.OK, MessageBoxIcon.Stop);
                }
                finally
                {
                    if (fileSystem != null)
                    {
                        Marshal.FinalReleaseComObject(fileSystem);
                    }
                }

                //
                // remove the Update event handler
                //
                discFormatData.Update -= discFormatData_Update;

                if (_ejectMedia)
                {
                    discRecorder.EjectMedia();
                }
            }
            catch (COMException exception)
            {
                //
                // If anything happens during the format, show the message
                //
                MessageBox.Show(exception.Message);
                e.Result = exception.ErrorCode;
            }
            finally
            {
                if (discRecorder != null)
                {
                    Marshal.ReleaseComObject(discRecorder);
                }

                if (discFormatData != null)
                {
                    Marshal.ReleaseComObject(discFormatData);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="progress"></param>
        void discFormatData_Update([In, MarshalAs(UnmanagedType.IDispatch)] object sender, [In, MarshalAs(UnmanagedType.IDispatch)] object progress)
        {
            //
            // Check if we've cancelled
            //
            if (backgroundBurnWorker.CancellationPending)
            {
                var format2Data = (IDiscFormat2Data)sender;
                format2Data.CancelWrite();
                return;
            }

            var eventArgs = (IDiscFormat2DataEventArgs)progress;

            _burnData.task = BURN_MEDIA_TASK.BURN_MEDIA_TASK_WRITING;

            // IDiscFormat2DataEventArgs Interface
            _burnData.elapsedTime = eventArgs.ElapsedTime;
            _burnData.remainingTime = eventArgs.RemainingTime;
            _burnData.totalTime = eventArgs.TotalTime;

            // IWriteEngine2EventArgs Interface
            _burnData.currentAction = eventArgs.CurrentAction;
            _burnData.startLba = eventArgs.StartLba;
            _burnData.sectorCount = eventArgs.SectorCount;
            _burnData.lastReadLba = eventArgs.LastReadLba;
            _burnData.lastWrittenLba = eventArgs.LastWrittenLba;
            _burnData.totalSystemBuffer = eventArgs.TotalSystemBuffer;
            _burnData.usedSystemBuffer = eventArgs.UsedSystemBuffer;
            _burnData.freeSystemBuffer = eventArgs.FreeSystemBuffer;

            //
            // Report back to the UI
            //
            backgroundBurnWorker.ReportProgress(0, _burnData);
        }

        /// <summary>
        /// Completed the "Burn" thread
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void backgroundBurnWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            labelStatusText.Text = (int)e.Result == 0 ? "Finished Burning Disc!" : "Error Burning Disc!";
            statusProgressBar.Value = 0;

            _isBurning = false;
            EnableBurnUI(true);
            buttonBurn.Enabled = true;
        }

        /// <summary>
        /// Enables/Disables the "Burn" User Interface
        /// </summary>
        /// <param name="enable"></param>
        void EnableBurnUI(bool enable)
        {
            buttonBurn.Text = enable ? "&Burn" : "&Cancel";
            buttonDetectMedia.Enabled = enable;

            devicesComboBox.Enabled = enable;
            listBoxFiles.Enabled = enable;

            buttonAddFiles.Enabled = enable;
            buttonAddFolders.Enabled = enable;
            buttonRemoveFiles.Enabled = enable;
            checkBoxEject.Enabled = enable;
            checkBoxCloseMedia.Enabled = enable;
            textBoxLabel.Enabled = enable;
            comboBoxVerification.Enabled = enable;
        }

        /// <summary>
        /// Event receives notification from the Burn thread of an event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void backgroundBurnWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            //int percent = e.ProgressPercentage;
            var burnData = (BurnData)e.UserState;

            if (burnData.task == BURN_MEDIA_TASK.BURN_MEDIA_TASK_FILE_SYSTEM)
            {
                labelStatusText.Text = burnData.statusMessage;
            }
            else if (burnData.task == BURN_MEDIA_TASK.BURN_MEDIA_TASK_WRITING)
            {
                switch (burnData.currentAction)
                {
                    case IMAPI_FORMAT2_DATA_WRITE_ACTION.IMAPI_FORMAT2_DATA_WRITE_ACTION_VALIDATING_MEDIA:
                        labelStatusText.Text = "Validating current media...";
                        break;

                    case IMAPI_FORMAT2_DATA_WRITE_ACTION.IMAPI_FORMAT2_DATA_WRITE_ACTION_FORMATTING_MEDIA:
                        labelStatusText.Text = "Formatting media...";
                        break;

                    case IMAPI_FORMAT2_DATA_WRITE_ACTION.IMAPI_FORMAT2_DATA_WRITE_ACTION_INITIALIZING_HARDWARE:
                        labelStatusText.Text = "Initializing hardware...";
                        break;

                    case IMAPI_FORMAT2_DATA_WRITE_ACTION.IMAPI_FORMAT2_DATA_WRITE_ACTION_CALIBRATING_POWER:
                        labelStatusText.Text = "Optimizing laser intensity...";
                        break;

                    case IMAPI_FORMAT2_DATA_WRITE_ACTION.IMAPI_FORMAT2_DATA_WRITE_ACTION_WRITING_DATA:
                        long writtenSectors = burnData.lastWrittenLba - burnData.startLba;

                        if (writtenSectors > 0 && burnData.sectorCount > 0)
                        {
                            var percent = (int)((100 * writtenSectors) / burnData.sectorCount);
                            labelStatusText.Text = string.Format("Progress: {0}%", percent);
                            statusProgressBar.Value = percent;
                        }
                        else
                        {
                            labelStatusText.Text = "Progress 0%";
                            statusProgressBar.Value = 0;
                        }
                        break;

                    case IMAPI_FORMAT2_DATA_WRITE_ACTION.IMAPI_FORMAT2_DATA_WRITE_ACTION_FINALIZATION:
                        labelStatusText.Text = "Finalizing writing...";
                        break;

                    case IMAPI_FORMAT2_DATA_WRITE_ACTION.IMAPI_FORMAT2_DATA_WRITE_ACTION_COMPLETED:
                        labelStatusText.Text = "Completed!";
                        break;

                    case IMAPI_FORMAT2_DATA_WRITE_ACTION.IMAPI_FORMAT2_DATA_WRITE_ACTION_VERIFYING:
                        labelStatusText.Text = "Verifying";
                        break;
                }
            }
        }

        /// <summary>
        /// Enable the Burn Button if items in the file listbox
        /// </summary>
        private void EnableBurnButton()
        {
            buttonBurn.Enabled = (listBoxFiles.Items.Count > 0);
        }


        #endregion


        #region File System Process
        private bool CreateMediaFileSystem(IDiscRecorder2 discRecorder, object[] multisessionInterfaces, out IStream dataStream)
        {
            MsftFileSystemImage fileSystemImage = null;
            try
            {
                fileSystemImage = new MsftFileSystemImage();
                fileSystemImage.ChooseImageDefaults(discRecorder);
                fileSystemImage.FileSystemsToCreate =
                    FsiFileSystems.FsiFileSystemJoliet | FsiFileSystems.FsiFileSystemISO9660;
                
                fileSystemImage.VolumeName = textBoxLabel.Text;

                fileSystemImage.Update += fileSystemImage_Update;

                //
                // If multisessions, then import previous sessions
                //
                if (multisessionInterfaces != null)
                {
                    fileSystemImage.MultisessionInterfaces = multisessionInterfaces;
                    fileSystemImage.ImportFileSystem();
                }

                //
                // Get the image root
                //
                IFsiDirectoryItem rootItem = fileSystemImage.Root;
                //rootItem.AddDirectory("Export1");
                 
                //
                // Add Files and Directories to File System Image
                //
                foreach (IMediaItem mediaItem in listBoxFiles.Items)
                {
                    //
                    // Check if we've cancelled
                    //
                    if (backgroundBurnWorker.CancellationPending)
                    {
                        break;
                    }

                    //
                    // Add to File System
                    //
                    mediaItem.AddToFileSystem(rootItem);
                }

                fileSystemImage.Update -= fileSystemImage_Update;

                //
                // did we cancel?
                //
                if (backgroundBurnWorker.CancellationPending)
                {
                    dataStream = null;
                    return false;
                }

                dataStream = fileSystemImage.CreateResultImage().ImageStream;
            }
            catch (COMException exception)
            {
                MessageBox.Show(this, exception.Message, "Create File System Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                dataStream = null;
                return false;
            }
            finally
            {
                if (fileSystemImage != null)
                {
                    Marshal.ReleaseComObject(fileSystemImage);
                }
            }

	        return true;
        }

        /// <summary>
        /// Event Handler for File System Progress Updates
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="currentFile"></param>
        /// <param name="copiedSectors"></param>
        /// <param name="totalSectors"></param>
        void fileSystemImage_Update([In, MarshalAs(UnmanagedType.IDispatch)] object sender,
            [In, MarshalAs(UnmanagedType.BStr)]string currentFile, [In] int copiedSectors, [In] int totalSectors)
        {
            var percentProgress = 0;
            if (copiedSectors > 0 && totalSectors > 0)
            {
                percentProgress = (copiedSectors * 100) / totalSectors;
            }

            if (!string.IsNullOrEmpty(currentFile))
            {
                var fileInfo = new FileInfo(currentFile);
                _burnData.statusMessage = "Adding \"" + fileInfo.Name + "\" to image...";

                //
                // report back to the ui
                //
                _burnData.task = BURN_MEDIA_TASK.BURN_MEDIA_TASK_FILE_SYSTEM;
                backgroundBurnWorker.ReportProgress(percentProgress, _burnData);
            }

        }
        #endregion


        #region Add/Remove File(s)/Folder(s)

        /// <summary>
        /// Adds a file to the burn list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonAddFiles_Click(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                var fileItem = new FileItem(openFileDialog.FileName);
                listBoxFiles.Items.Add(fileItem);

                UpdateCapacity();
                EnableBurnButton();
            }
        }

        /// <summary>
        /// Adds a folder to the burn list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonAddFolders_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog.ShowDialog(this) == DialogResult.OK)
            {
                var directoryItem = new DirectoryItem(folderBrowserDialog.SelectedPath);
                string folderPath = folderBrowserDialog.SelectedPath;
                string fileName = Path.GetFileName(folderPath).ToString();
                int index1 = fileName.IndexOf('_');
                if (index1 == -1)
                {
                    MessageBox.Show(this, "You Have Selected Wrong Folder...", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (index1 != -1 && fileName.Substring(0, index1).Length < 10)
                {
                    MessageBox.Show(this, "You Have Selected Wrong Folder...", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                int index = listBoxFiles.FindString(fileName.Substring(0, index1));
                if (index == -1)
                {
                    listBoxFiles.Items.Add(directoryItem);
                    UpdateCapacity();
                    EnableBurnButton();
                }
                else
                {
                    
                    MessageBox.Show(this,"You Have Selected Duplicate Volume","Warning",MessageBoxButtons.OK,MessageBoxIcon.Warning);
                    listBoxFiles.SetSelected(index, true);
                    return;
                }
            }
        }

        /// <summary>
        /// User wants to remove a file or folder
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonRemoveFiles_Click(object sender, EventArgs e)
        {
            var mediaItem = (IMediaItem)listBoxFiles.SelectedItem;
            if (mediaItem == null)
                return;

            if (MessageBox.Show("Are you sure you want to remove \"" + mediaItem + "\"?",
                "Remove item", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                listBoxFiles.Items.Remove(mediaItem);

                EnableBurnButton();
                UpdateCapacity();
            }
        }

        #endregion


        #region File ListBox Events
        /// <summary>
        /// The user has selected a file or folder
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listBoxFiles_SelectedIndexChanged(object sender, EventArgs e)
        {
            //buttonRemoveFiles.Enabled = (listBoxFiles.SelectedIndex != -1);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listBoxFiles_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (listBoxFiles.Items.Count > 0)
            {
                var mediaItem = (IMediaItem)listBoxFiles.Items[e.Index];

                if (mediaItem == null)
                {
                    return;
                }

                e.DrawBackground();

                if ((e.State & DrawItemState.Focus) != 0)
                {
                    e.DrawFocusRectangle();
                }

                if (mediaItem.FileIconImage != null)
                {
                    e.Graphics.DrawImage(mediaItem.FileIconImage, new Rectangle(4, e.Bounds.Y + 4, 16, 16));
                }

                var rectF = new RectangleF(e.Bounds.X + 24, e.Bounds.Y,
                    e.Bounds.Width - 24, e.Bounds.Height);

                var font = new Font(FontFamily.GenericSansSerif, 11);

                var stringFormat = new StringFormat
                    {
                        LineAlignment = StringAlignment.Center,
                        Alignment = StringAlignment.Near,
                        Trimming = StringTrimming.EllipsisCharacter
                    };

                e.Graphics.DrawString(mediaItem.ToString(), font, new SolidBrush(e.ForeColor),
                    rectF, stringFormat);
            }
        }
        #endregion
        public DataSet getDistrict(string dis)
        {
            string sql = "select trim(district_name) as district_name from district where district_code = '"+dis+"'";
            DataSet ds = new DataSet();
            OdbcDataAdapter odap = new OdbcDataAdapter(sql, con);
            odap.Fill(ds);
            return ds;
        }
        public DataSet getRO(string dis, string ro)
        {
            string sql = "select trim(ro_name) as ro_name from ro_master where district_code = '"+dis+"' and ro_code = '"+ro+"'";
            DataSet ds = new DataSet();
            OdbcDataAdapter odap = new OdbcDataAdapter(sql, con);
            odap.Fill(ds);
            return ds;
        }
        public DataSet getCDNo()
        {
            string sql = "select SYSVALUES from SYSCONFIG  where SYSKEYS='CD_NO'";
            DataSet ds = new DataSet();
            OdbcDataAdapter odap = new OdbcDataAdapter(sql, con);
            odap.Fill(ds);
            return ds;
        }
        public bool UpdateCDCVD(string cdno)
        {
            string sqlStr = null;
            bool commitBol = true;
            OdbcCommand sqlCmd = new OdbcCommand();
            int cdnumber = Convert.ToInt32(cdno)+1;
            try
            {
                sqlStr = @"update SYSCONFIG set SYSVALUES = '" + cdnumber + "' where SYSKEYS='CD_NO'";
                sqlCmd.CommandText = sqlStr;
                sqlCmd.Connection = con;
                sqlCmd.ExecuteNonQuery();
                commitBol = true;
            }
            catch (Exception ex)
            {
                commitBol = true;
            }
            return commitBol;
        }
        public void tabTextFile(DataGridView dg, string filename)
        {

            DataSet ds = new DataSet();
            DataTable dtSource = null;
            DataTable dt = new DataTable();
            DataRow dr;
            if (dg.DataSource != null)
            {
                if (dg.DataSource.GetType() == typeof(DataSet))
                {
                    DataSet dsSource = (DataSet)dg.DataSource;
                    if (dsSource.Tables.Count > 0)
                    {
                        string strTables = string.Empty;
                        foreach (DataTable dt1 in dsSource.Tables)
                        {
                            strTables += TableToString(dt1);
                            strTables += "\r\n\r\n";
                        }
                        if (strTables != string.Empty)
                            SaveDataGridData(strTables, filename);
                    }
                }
                else
                {
                    if (dg.DataSource.GetType() == typeof(DataTable))
                        dtSource = (DataTable)dg.DataSource;
                    if (dtSource != null)

                        SaveDataGridData(TableToString(dtSource), filename);
                }
            }

        }
        private void SaveDataGridData(string strData, string strFileName)
        {
            FileStream fs;
            TextWriter tw = null;
            try
            {
                if (File.Exists(strFileName))
                {
                    fs = new FileStream(strFileName, FileMode.Open);
                }
                else
                {
                    fs = new FileStream(strFileName, FileMode.Create);
                }
                tw = new StreamWriter(fs);
                tw.Write(strData);
            }
            finally
            {
                if (tw != null)
                {
                    tw.Flush();
                    tw.Close();
                }
            }
        }
        private string TableToString(DataTable dt)
        {
            string strData = string.Empty;
            string sep = string.Empty;
            if (dt.Rows.Count > 0)
            {
                foreach (DataColumn c in dt.Columns)
                {
                    if (c.DataType != typeof(System.Guid) &&
                    c.DataType != typeof(System.Byte[]))
                    {
                        strData += sep + c.ColumnName;
                        sep = ",";
                    }
                }
                strData += "\r\n";
                foreach (DataRow r in dt.Rows)
                {
                    sep = string.Empty;
                    foreach (DataColumn c in dt.Columns)
                    {
                        if (c.DataType != typeof(System.Guid) &&
                        c.DataType != typeof(System.Byte[]))
                        {
                            if (!Convert.IsDBNull(r[c.ColumnName]))

                                strData += sep +
                                '"' + r[c.ColumnName].ToString().Replace("\n", " ").Replace(",", "-") + '"';

                            else

                                strData += sep + "";
                            sep = ",";

                        }
                    }
                    strData += "\r\n";

                }
            }
            else
            {
                //strData += "\r\n---> Table was empty!";
                foreach (DataColumn c in dt.Columns)
                {
                    if (c.DataType != typeof(System.Guid) &&
                    c.DataType != typeof(System.Byte[]))
                    {
                        strData += sep + c.ColumnName;
                        sep = ",";
                    }
                }
                strData += "\r\n";
            }
            return strData;
        }

        public bool excelExport()
        {
            DateTime now = DateTime.Now;
            textBoxLabel.Text = "L" + cmbVol.Text.Substring(0, 4) + now.Month.ToString().PadLeft(2, '0') + now.Year.ToString().Substring(2, 2) + cmbVol.Text.Substring(4, 6);
            if (Directory.Exists(appPath + "\\NIC\\" + textBoxLabel.Text))
            {
                Directory.Delete(appPath + "\\NIC\\" + textBoxLabel.Text, true);
                Directory.CreateDirectory(appPath + "\\NIC\\" + textBoxLabel.Text);
                Directory.CreateDirectory(appPath + "\\NIC\\" + textBoxLabel.Text + "\\csv");
                Directory.CreateDirectory(appPath + "\\NIC\\" + textBoxLabel.Text + "\\Images");
                Directory.CreateDirectory(appPath + "\\NIC\\" + textBoxLabel.Text + "\\DB");
            }
            else
            {
                Directory.CreateDirectory(appPath + "\\NIC\\" + textBoxLabel.Text);
                Directory.CreateDirectory(appPath + "\\NIC\\" + textBoxLabel.Text + "\\csv");
                Directory.CreateDirectory(appPath + "\\NIC\\" + textBoxLabel.Text + "\\Images");
                Directory.CreateDirectory(appPath + "\\NIC\\" + textBoxLabel.Text + "\\DB");
            }
            Excel.Application xlApp = new Excel.Application();
            int deedCount = 0;
            int index1Count = 0;
            int index2Count = 0;
            int totalscanPagesCount = 0;
            int totalHold = 0;
            int imageonHold = 0;
            int deedAccepted = 0;
            int imageAccepted = 0;
            int addRecord = 0;
            string district = string.Empty;
            string ro = string.Empty;
            string book = string.Empty;
            string year = string.Empty;
            string csv_file_path = string.Empty;
            string cdDvdNo = string.Empty;
            System.Data.DataTable csvData = null;
            System.Data.DataTable csvDeedCount = null;
            System.Data.DataTable dtDeed_Details = new DataTable();
            System.Data.DataTable dtIndex1 = new DataTable();
            System.Data.DataTable dtindex2 = new DataTable();
            System.Data.DataTable dtotherPlot = new DataTable();
            System.Data.DataTable dtotherKhatian = new DataTable();
            try
            {
                int rowNum = 9;
                district = listBoxFiles.Items[0].ToString().Substring(0, 2);
                ro = listBoxFiles.Items[0].ToString().Substring(2, 2);
                book = listBoxFiles.Items[0].ToString().Substring(4, 1);
                year = listBoxFiles.Items[0].ToString().Substring(5, 4);
                for (int i = 0; i < listBoxFiles.Items.Count; i++)
                {
                    if(district !=listBoxFiles.Items[i].ToString().Substring(0, 2))
                    {
                        MessageBox.Show("District for All Records must me unique,Remove Folder: " + listBoxFiles.Items[i].ToString(), "Warning", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }
                    if (ro != listBoxFiles.Items[i].ToString().Substring(2, 2))
                    {
                        MessageBox.Show("RO for All Records must me unique,Remove Folder: " + listBoxFiles.Items[i].ToString(), "Warning", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }
                    if (book != listBoxFiles.Items[i].ToString().Substring(4, 1))
                    {
                        MessageBox.Show("Book Type for All Records must me unique,Remove Folder: " + listBoxFiles.Items[i].ToString(), "Warning", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }
                    if (year != listBoxFiles.Items[i].ToString().Substring(5,4))
                    {
                        MessageBox.Show("Year must me unique,Remove Folder: " + listBoxFiles.Items[i].ToString(), "Warning", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }
                }
                cdDvdNo = getCDNo().Tables[0].Rows[0][0].ToString().PadLeft(6,'0');
                //DateTime now = DateTime.Now;
                
                //textBoxLabel.Text = "L" + cmbVol.Text.Substring(0, 4) + now.Month.ToString().PadLeft(2, '0') + now.Year.ToString().Substring(2, 2) + cmbVol.Text.Substring(4, 6);
                string UATTemplatePath = System.IO.Path.GetDirectoryName(Application.ExecutablePath) + "\\UAT.xls";
                string UATDestPath = System.IO.Path.GetDirectoryName(Application.ExecutablePath) + "\\NIC\\" + textBoxLabel.Text + "\\" + textBoxLabel.Text + ".xls";
                File.Copy(UATTemplatePath, UATDestPath, true);
                Excel.Workbook xlWorkBook = xlApp.Workbooks.Open(UATDestPath, 0, false, 5, "", "", true, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "\t", false, false, 0, true, 1, 0);
                Excel.Worksheet xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets[1];
                for (int i = 0; i < listBoxFiles.Items.Count; i++)
                {
                    Excel.Range r = xlWorkSheet.get_Range("A" + rowNum.ToString(), "J" + rowNum.ToString());
                    r.Insert(Excel.XlInsertShiftDirection.xlShiftDown,Type.Missing);
                }
                bgwFileCopy.RunWorkerAsync();
                for (int i = 0; i < listBoxFiles.Items.Count; i++)
                {
                    var mediaItem = (IMediaItem)listBoxFiles.Items[i];
                    string folderPath = mediaItem.Path.ToString();
                    if (Directory.Exists(folderPath))
                    {
                        foreach (string s in Directory.GetDirectories(folderPath))
                        {
                            
                            string districtFolder = s.ToString();
                            foreach (string r in Directory.GetDirectories(districtFolder))
                            {
                                string roFolder = r.ToString();
                                foreach (string image in Directory.GetFiles(r))
                                {
                                    string filename = Path.GetFileName(image);
                                    File.Copy(image, appPath + "\\NIC\\" + textBoxLabel.Text + "\\Images\\" + filename);
                                }
                                if (Directory.Exists(roFolder + "\\csv"))
                                {
                                    if (File.Exists(roFolder + "\\csv" + "\\summary.csv"))
                                    {
                                        if(File.Exists(roFolder+"\\csv"+"\\deedCount.csv"))
                                        {
                                            csv_file_path = roFolder + "\\csv" + "\\deedCount.csv";
                                            csvDeedCount = GetDataTabletFromCSVFile(csv_file_path);
                                        }
                                        csv_file_path = roFolder + "\\csv" + "\\summary.csv";
                                        csvData = GetDataTabletFromCSVFile(csv_file_path);
                                        xlWorkSheet.get_Range("A" + rowNum, "A" + rowNum);
                                        xlWorkSheet.Cells[4, 1] = "Year - " + listBoxFiles.Items[i].ToString().Substring(5, 4);
                                        xlWorkSheet.Cells[4, 4] = "Location: " + getRO(district, ro).Tables[0].Rows[0][0].ToString();
                                        //xlWorkSheet.Cells[8, 6] = getDistrict(district).Tables[0].Rows[0][0].ToString();
                                        xlWorkSheet.Cells[5, 1] = "Media Number:" + textBoxLabel.Text;
                                        //xlWorkBook.Save();
                                        //xlApp.Quit();
                                        int index2 = listBoxFiles.Items[i].ToString().IndexOf('_');
                                        int colNum = 0;
                                        
                                        xlWorkSheet.Cells[rowNum, 1] = (i+1).ToString();
                                        xlWorkSheet.Cells[rowNum, 2] = listBoxFiles.Items[i].ToString().Substring(5, 4) + "-" + listBoxFiles.Items[i].ToString().Substring(9, index2 - 9);
                                        xlWorkSheet.Cells[rowNum, 3] = csvDeedCount.Rows[0][0].ToString();
                                        xlWorkSheet.Cells[rowNum, 4] = csvData.Rows[0][0].ToString();
                                        xlWorkSheet.Cells[rowNum, 5] = csvData.Rows[0][1].ToString();
                                        xlWorkSheet.Cells[rowNum, 6] = csvData.Rows[0][2].ToString();
                                        xlWorkSheet.Cells[rowNum, 7] = csvData.Rows[0][3].ToString();
                                        xlWorkSheet.Cells[rowNum, 8] = Convert.ToInt32(csvData.Rows[0][3].ToString()) / Convert.ToInt32(csvData.Rows[0][0].ToString());
                                        xlWorkSheet.Cells[rowNum, 9] = (Convert.ToInt32(csvData.Rows[0][1].ToString()) + Convert.ToInt32(csvData.Rows[0][2].ToString())) - Convert.ToInt32(csvData.Rows[0][0].ToString()) * 3;
                                        xlWorkSheet.Cells[rowNum, 10] = getUATDate();
                                    }
                                }
                                
                                if (File.Exists(roFolder + "\\csv" + "\\deed_details.csv"))
                                {
                                    csv_file_path = roFolder + "\\csv" + "\\deed_details.csv";
                                    dtDeed_Details  = GetDataTabletFromCSVFile(csv_file_path);
                                }
                                if (File.Exists(roFolder + "\\csv" + "\\index_of_name.csv"))
                                {
                                    csv_file_path = roFolder + "\\csv" + "\\index_of_name.csv";
                                    dtIndex1  = GetDataTabletFromCSVFile(csv_file_path);
                                }
                                if (File.Exists(roFolder + "\\csv" + "\\index_of_property.csv"))
                                {
                                    csv_file_path = roFolder + "\\csv" + "\\index_of_property.csv";
                                    dtindex2 = GetDataTabletFromCSVFile(csv_file_path);
                                }
                                if (File.Exists(roFolder + "\\csv" + "\\other_plots.csv"))
                                {
                                    csv_file_path = roFolder + "\\csv" + "\\other_plots.csv";
                                    dtotherPlot = GetDataTabletFromCSVFile(csv_file_path);
                                }
                                if (File.Exists(roFolder + "\\csv" + "\\other_khatian.csv"))
                                {
                                    csv_file_path = roFolder + "\\csv" + "\\other_khatian.csv";
                                    dtotherKhatian = GetDataTabletFromCSVFile(csv_file_path);
                                }
                            }
                            deedCount = deedCount + Convert.ToInt32(csvData.Rows[0][0].ToString());
                            index1Count = index1Count + Convert.ToInt32(csvData.Rows[0][1].ToString());
                            index2Count = index2Count + Convert.ToInt32(csvData.Rows[0][2].ToString());
                            totalscanPagesCount = totalscanPagesCount + Convert.ToInt32(csvData.Rows[0][3].ToString());
                            addRecord = addRecord + (Convert.ToInt32(csvData.Rows[0][1].ToString()) + Convert.ToInt32(csvData.Rows[0][2].ToString()))- Convert.ToInt32(csvData.Rows[0][0].ToString()) * 3;
                            rowNum = rowNum + 1;
                            dtDd.Merge(dtDeed_Details);
                            dtIn1.Merge(dtIndex1);
                            dtIn2.Merge(dtindex2);
                            dtOp.Merge(dtotherPlot);
                            dtOk.Merge(dtotherKhatian);
                        }
                    }
                    
                }
                xlWorkSheet.Cells[rowNum+1, 4] = deedCount.ToString();
                xlWorkSheet.Cells[rowNum+1, 5] = index1Count.ToString();
                xlWorkSheet.Cells[rowNum+1, 6] = index2Count.ToString();
                xlWorkSheet.Cells[rowNum+1, 7] = totalscanPagesCount.ToString();
                xlWorkSheet.Cells[rowNum+1, 8] = Convert.ToInt32(totalscanPagesCount / deedCount).ToString();
                if (addRecord < 0)
                {
                    xlWorkSheet.Cells[rowNum+1, 9] = "0";
                }
                else
                {
                    xlWorkSheet.Cells[rowNum+1, 9] = addRecord.ToString();
                }
                xlWorkBook.Save();
                xlApp.Quit();
                
                
                dgvdeedDetails.DataSource = dtDd;
                dgvIndex1.DataSource = dtIn1;
                dgvindex2.DataSource = dtIn2;
                dgvOtherKhatian.DataSource = dtOk;
                dgvotherPlot.DataSource = dtOp;
                tabTextFile(dgvdeedDetails, appPath + "\\NIC\\" + textBoxLabel.Text + "\\csv\\" + textBoxLabel.Text + "_Deed_Details.csv");
                tabTextFile(dgvIndex1, appPath + "\\NIC\\" + textBoxLabel.Text + "\\csv\\" + textBoxLabel.Text + "_index_of_name.csv");
                tabTextFile(dgvindex2, appPath + "\\NIC\\" + textBoxLabel.Text + "\\csv\\" + textBoxLabel.Text + "_index_of_property.csv");
                tabTextFile(dgvOtherKhatian, appPath + "\\NIC\\" + textBoxLabel.Text + "\\csv\\" + textBoxLabel.Text + "_other_khatian.csv");
                tabTextFile(dgvotherPlot, appPath + "\\NIC\\" + textBoxLabel.Text + "\\csv\\" + textBoxLabel.Text + "_other_plots.csv");

                listBoxFiles.Items.Clear();
                using (ZipFile zip = new ZipFile())
                {
                    zip.AddDirectory(appPath + "\\NIC\\" + textBoxLabel.Text + "\\csv"); // recurses subdirectories
                    zip.Save(appPath + "\\NIC\\" + textBoxLabel.Text + "\\" + textBoxLabel.Text + "_CSV.zip");

                }

                var directoryItem1 = new DirectoryItem(appPath + "\\NIC\\" + textBoxLabel.Text + "\\Images");
                listBoxFiles.Items.Add(directoryItem1);
                UpdateCapacity();
                var directoryItem2 = new DirectoryItem(appPath + "\\NIC\\" + textBoxLabel.Text + "\\csv");
                listBoxFiles.Items.Add(directoryItem2);
                UpdateCapacity();
                var directoryItem3 = new DirectoryItem(appPath + "\\NIC\\" + textBoxLabel.Text + "\\DB");
                listBoxFiles.Items.Add(directoryItem3);
                UpdateCapacity();
                var fileItem = new FileItem(UATDestPath);
                listBoxFiles.Items.Add(fileItem);
                UpdateCapacity();
                var fileItem1 = new FileItem(appPath + "\\NIC\\" + textBoxLabel.Text + "\\" + textBoxLabel.Text + "_CSV.zip");
                listBoxFiles.Items.Add(fileItem1);
                UpdateCapacity();
                return true;
                
            }
            catch (Exception ex)
            {
                xlApp.Quit();
                MessageBox.Show(ex.Message.ToString());
                return false;
                
            }
        }

        public string getUATDate()
        {
            string date = string.Empty;
            DataSet ds = new DataSet();
            string qry = "select DATE_FORMAT(created_dttm, '%d-%m-%y') from tbl_uat_info where run_no = '"+cmbVol.Text+"'";
            OdbcDataAdapter odap = new OdbcDataAdapter(qry, con);
            odap.Fill(ds);
            if (ds.Tables[0].Rows.Count > 0)
            {
                date = ds.Tables[0].Rows[0][0].ToString();
            }

            return date;
        }
        public bool ExportWorkbookToPdf(string workbookPath, string outputPath)
        {
            // If either required string is null or empty, stop and bail out
            if (string.IsNullOrEmpty(workbookPath) || string.IsNullOrEmpty(outputPath))
            {
                return false;
            }

            // Create COM Objects
            Microsoft.Office.Interop.Excel.Application excelApplication;
            Microsoft.Office.Interop.Excel.Workbook excelWorkbook;

            // Create new instance of Excel
            excelApplication = new Microsoft.Office.Interop.Excel.Application();

            // Make the process invisible to the user
            //excelApplication.ScreenUpdating = false;

            // Make the process silent
            excelApplication.DisplayAlerts = false;

            // Open the workbook that you wish to export to PDF
            excelWorkbook = excelApplication.Workbooks.Open(workbookPath, 0, false, 5, "", "", true, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "\t", false, false, 0, true, 1, 0);
            
            // If the workbook failed to open, stop, clean up, and bail out
            if (excelWorkbook == null)
            {
                excelApplication.Quit();

                excelApplication = null;
                excelWorkbook = null;

                return false;
            }

            var exportSuccessful = true;
            try
            {
                // Call Excel's native export function (valid in Office 2007 and Office 2010, AFAIK)
                excelWorkbook.ExportAsFixedFormat(Microsoft.Office.Interop.Excel.XlFixedFormatType.xlTypePDF, outputPath, Microsoft.Office.Interop.Excel.XlFixedFormatQuality.xlQualityStandard, Type.Missing, false, Type.Missing, Type.Missing, false, Type.Missing);
                //excelWorkbook.ExportAsFixedFormat(Microsoft.Office.Interop.Excel.XlFixedFormatType.xlTypePDF, Server.MapPath("MCS.pdf"), Microsoft.Office.Interop.Excel.XlFixedFormatQuality.xlQualityStandard, missing, False, missing, missing, False, missing)
            }
            catch (System.Exception ex)
            {
                // Mark the export as failed for the return value...
                exportSuccessful = false;
                MessageBox.Show("If you are using MS Office 2003 then Install Service Pack 3 for MS Office 2003");
                // Do something with any exceptions here, if you wish...
                // MessageBox.Show...        
            }
            finally
            {
                // Close the workbook, quit the Excel, and clean up regardless of the results...
                excelWorkbook.Close(false, Type.Missing, Type.Missing);
                excelApplication.Quit();

                excelApplication = null;
                excelWorkbook = null;
            }

            // You can use the following method to automatically open the PDF after export if you wish
            // Make sure that the file actually exists first...
            if (System.IO.File.Exists(outputPath))
            {
                System.Diagnostics.Process.Start(outputPath);
            }

            return exportSuccessful;
        }

        private static DataTable GetDataTabletFromCSVFile(string csv_file_path)
        {
            System.Data.DataTable csvData = new System.Data.DataTable();
            try
            {
                using (Microsoft.VisualBasic.FileIO.TextFieldParser csvReader = new Microsoft.VisualBasic.FileIO.TextFieldParser(csv_file_path))
                {
                    csvReader.SetDelimiters(new string[] { "," });
                    csvReader.HasFieldsEnclosedInQuotes = true;
                    //read column names
                    string[] colFields = csvReader.ReadFields();
                    foreach (string column in colFields)
                    {
                        System.Data.DataColumn datecolumn = new System.Data.DataColumn(column);
                        datecolumn.AllowDBNull = true;
                        csvData.Columns.Add(datecolumn);
                    }
                    while (!csvReader.EndOfData)
                    {
                        string[] fieldData = csvReader.ReadFields();
                        //Making empty value as null
                        for (int i = 0; i < fieldData.Length; i++)
                        {
                            if (fieldData[i] == "")
                            {
                                fieldData[i] = null;
                            }
                        }
                        csvData.Rows.Add(fieldData);
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return csvData;
        }
        #region Format/Erase the Disc
        /// <summary>
        /// The user has clicked the "Format" button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonFormat_Click(object sender, EventArgs e)
        {
            if (devicesComboBox.SelectedIndex == -1)
            {
                return;
            }

            _isFormatting = true;
            EnableFormatUI(false);

            var discRecorder =
                (IDiscRecorder2)devicesComboBox.Items[devicesComboBox.SelectedIndex];
            backgroundFormatWorker.RunWorkerAsync(discRecorder.ActiveDiscRecorder);
        }

        /// <summary>
        /// Enables/Disables the "Burn" User Interface
        /// </summary>
        /// <param name="enable"></param>
        void EnableFormatUI(bool enable)
        {
            buttonFormat.Enabled = enable;
            checkBoxEjectFormat.Enabled = enable;
            checkBoxQuickFormat.Enabled = enable;
        }

        /// <summary>
        /// Worker thread that Formats the Disc
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void backgroundFormatWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            MsftDiscRecorder2 discRecorder = null;
            MsftDiscFormat2Erase discFormatErase = null;

            try
            {
                //
                // Create and initialize the IDiscRecorder2
                //
                discRecorder = new MsftDiscRecorder2();
                var activeDiscRecorder = (string)e.Argument;
                discRecorder.InitializeDiscRecorder(activeDiscRecorder);

                //
                // Create the IDiscFormat2Erase and set properties
                //
                discFormatErase = new MsftDiscFormat2Erase
                    {
                        Recorder = discRecorder,
                        ClientName = ClientName,
                        FullErase = !checkBoxQuickFormat.Checked
                    };

                //
                // Setup the Update progress event handler
                //
                discFormatErase.Update += discFormatErase_Update;

                //
                // Erase the media here
                //
                try
                {
                    discFormatErase.EraseMedia();
                    e.Result = 0;
                }
                catch (COMException ex)
                {
                    e.Result = ex.ErrorCode;
                    MessageBox.Show(ex.Message, "IDiscFormat2.EraseMedia failed",
                        MessageBoxButtons.OK, MessageBoxIcon.Stop);
                }

                //
                // Remove the Update progress event handler
                //
                discFormatErase.Update -= discFormatErase_Update;

                //
                // Eject the media 
                //
                if (checkBoxEjectFormat.Checked)
                {
                    discRecorder.EjectMedia();
                }

            }
            catch (COMException exception)
            {
                //
                // If anything happens during the format, show the message
                //
                MessageBox.Show(exception.Message);
            }
            finally
            {
                if (discRecorder != null)
                {
                    Marshal.ReleaseComObject(discRecorder);
                }

                if (discFormatErase != null)
                {
                    Marshal.ReleaseComObject(discFormatErase);
                }
            }
        }

        /// <summary>
        /// Event Handler for the Erase Progress Updates
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="elapsedSeconds"></param>
        /// <param name="estimatedTotalSeconds"></param>
        void discFormatErase_Update([In, MarshalAs(UnmanagedType.IDispatch)] object sender, int elapsedSeconds, int estimatedTotalSeconds)
        {
            var percent = elapsedSeconds * 100 / estimatedTotalSeconds;
            //
            // Report back to the UI
            //
            backgroundFormatWorker.ReportProgress(percent);
        }

        private void backgroundFormatWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            labelFormatStatusText.Text = string.Format("Formatting {0}%...", e.ProgressPercentage);
            formatProgressBar.Value = e.ProgressPercentage;
        }

        private void backgroundFormatWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            labelFormatStatusText.Text = (int)e.Result == 0 ?
                "Finished Formatting Disc!" : "Error Formatting Disc!";

            formatProgressBar.Value = 0;

            _isFormatting = false;
            EnableFormatUI(true);
        }
        #endregion

        /// <summary>
        /// Called when user selects a new tab
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabControl1_Selecting(object sender, TabControlCancelEventArgs e)
        {
            //
            // Prevent page from changing if we're burning or formatting.
            //
            if (_isBurning || _isFormatting)
            {
                e.Cancel = true;
            }
        }

        /// <summary>
        /// Get the burn verification level when the user changes the selection
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBoxVerification_SelectedIndexChanged(object sender, EventArgs e)
        {
            _verificationLevel = (IMAPI_BURN_VERIFICATION_LEVEL)comboBoxVerification.SelectedIndex;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Application.DoEvents();
            prgsStatus.Visible = true;
            Application.DoEvents();
            excelExport();
            prgsStatus.Visible = false;
        }

        private void supportedMediaLabel_Click(object sender, EventArgs e)
        {

        }
        private void ReadINI()
        {
            string iniPath = System.IO.Path.GetDirectoryName(Application.ExecutablePath) + "\\Configuration.ini";
            if (File.Exists(iniPath))
            {
                NovaNet.Utils.INIFile ini = new NovaNet.Utils.INIFile();
                sqlIp = ini.ReadINI("SQLSERVERIP", "IP", "", iniPath);
                sqlIp = sqlIp.Replace("\0", "").Trim();
            }
        }
        private bool ExportSqlServer(DataTable DeedSQL, DataTable NameSQL, DataTable PropertySQL,DataTable plotSQL, DataTable KhatianSQL)
        {
            SqlConnection CN = null;
            try
            {
                string tempfilename = textBoxLabel.Text + "_IGR.bak";
                string dbTemplatePath = System.IO.Path.GetDirectoryName(Application.ExecutablePath) + "\\db_Template" + "\\IGR.mdf";
                string destDbFilePath = appPath + "\\SQLSERVEREXPORT";

                if (!File.Exists(dbTemplatePath))
                {
                    MessageBox.Show("Template mdf not present");
                    return false;
                }

                if (Directory.Exists(destDbFilePath))
                {
                    Directory.Delete(destDbFilePath, true);

                }
                Directory.CreateDirectory(destDbFilePath);
                File.Copy(dbTemplatePath, destDbFilePath + "\\" + tempfilename, true);
                string sql = @"Server=" + sqlIp + @"\SQLEXPRESS;AttachDbFilename=" + destDbFilePath + "\\" + tempfilename + "; Database=IGR;Trusted_Connection=Yes;";
                CN = new SqlConnection(sql);
                bool result = false;
                CN.Open();
                SqlTransaction sqltrans = null;
                try
                {
                    sqltrans = CN.BeginTransaction();
                }
                catch (Exception ex)
                {
                    CN.Open();
                    sqltrans = CN.BeginTransaction();
                }
                if (SaveDeedDetails(CN, DeedSQL, sqltrans) == true)
                {
                        if (SaveIndex1(CN, NameSQL, sqltrans) == true)
                        {
                            if (SaveIndex2(CN, PropertySQL, sqltrans) == true)
                            {
                                if (SaveOtherPlot(CN, plotSQL, sqltrans) == true)
                                {
                                    if (SaveOtherKhatian(CN, KhatianSQL, sqltrans) == true)
                                    {
                                        sqltrans.Commit();
                                        TakeBackup(destDbFilePath);
                                        DropDb(CN);
                                        if (CN.State == ConnectionState.Open)
                                        {
                                            CN.Close();
                                            SqlConnection.ClearPool(CN);
                                        }
                                        string[] filenames = Directory.GetFiles(destDbFilePath);
                                        foreach (string s in filenames)
                                        {
                                            string fileName = System.IO.Path.GetFileName(s);

                                            string destFile = System.IO.Path.Combine(appPath + "\\NIC\\DB",tempfilename);
                                            System.IO.File.Copy(s, destFile, true);
                                        }
                                        result = true;
                                    }
                                }
                            }
                    }

                }
                return result;
            }
            catch (Exception ex)
            {
                DropDb(CN);
                CN.Close();
                MessageBox.Show(this, "Error while exporting....Please Restart your Application..." + ex.Message.ToString(), "IGR....");
                return false;
            }
            finally
            {
                if (CN.State == ConnectionState.Open)
                {
                    CN.Close();
                    SqlConnection.ClearPool(CN);
                } 
            }
        }

        private bool SaveDeedDetails(SqlConnection CN, DataTable dt, SqlTransaction sqltrans)
        {
            try
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    string qry = "insert into Deed_details(District_Code,RO_Code,Book,Deed_year,Deed_no,Serial_No,Serial_Year,tran_maj_code,tran_min_code,Volume_No,Page_From,Page_To,Date_of_Completion,Date_of_Delivery,Deed_Remarks,scan_doc_type) values ('" + dt.Rows[i][0].ToString() + "','" + dt.Rows[i][1].ToString() + "','" + dt.Rows[i][2].ToString() + "','" + dt.Rows[i][3].ToString() + "','" + dt.Rows[i][4].ToString() + "','" + dt.Rows[i][5].ToString() + "','" + dt.Rows[i][6].ToString() + "','" + dt.Rows[i][7].ToString() + "','" + dt.Rows[i][8].ToString() + "','" + dt.Rows[i][9].ToString() + "','" + dt.Rows[i][10].ToString() + "','" + dt.Rows[i][11].ToString() + "','" + dt.Rows[i][12].ToString() + "','" + dt.Rows[i][13].ToString() + "','" + dt.Rows[i][14].ToString() + "','" + dt.Rows[i][15].ToString() + "')";
                    //Initialize SqlCommand object for insert.
                    SqlCommand SqlCom = new SqlCommand(qry, CN);
                    SqlCom.Transaction = sqltrans;
                    SqlCom.ExecuteNonQuery();
                }
                return true;
            }
            catch (Exception ex)
            { return false; }
        }
        private bool SaveIndex1(SqlConnection CN, DataTable dt, SqlTransaction sqltrans)
        {
            try
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    string qry = "insert into index_of_name(District_Code,RO_Code,Book,Deed_year,Deed_no,Item_no,initial_name,First_name,Last_name,Party_code,Admit_code,Address,Address_district_code,Address_district_name,Address_ps_code,Address_ps_name,Father_mother,Rel_code,Relation,occupation_code,religion_code,more,pin,city,other_party_code,linked_to) values('" + dt.Rows[i][0].ToString() + "','" + dt.Rows[i][1].ToString() + "','" + dt.Rows[i][2].ToString() + "','" + dt.Rows[i][3].ToString() + "','" + dt.Rows[i][4].ToString() + "','" + dt.Rows[i][5].ToString() + "','" + dt.Rows[i][6].ToString() + "','" + dt.Rows[i][7].ToString() + "','" + dt.Rows[i][8].ToString() + "','" + dt.Rows[i][9].ToString() + "','" + dt.Rows[i][10].ToString() + "','" + dt.Rows[i][11].ToString() + "','" + dt.Rows[i][12].ToString() + "','" + dt.Rows[i][13].ToString() + "','" + dt.Rows[i][14].ToString() + "','" + dt.Rows[i][15].ToString() + "','" + dt.Rows[i][16].ToString() + "','" + dt.Rows[i][17].ToString() + "','" + dt.Rows[i][18].ToString() + "','" + dt.Rows[i][19].ToString() + "','" + dt.Rows[i][20].ToString() + "','" + dt.Rows[i][21].ToString() + "','" + dt.Rows[i][22].ToString() + "','" + dt.Rows[i][23].ToString() + "','" + dt.Rows[i][24].ToString() + "','" + dt.Rows[i][25].ToString() + "')";
                    SqlCommand SqlCom = new SqlCommand(qry, CN);
                    SqlCom.Transaction = sqltrans;
                    SqlCom.ExecuteNonQuery();
                }
                return true;
            }
            catch (Exception ex)
            { return false; }
        }
        private bool SaveIndex2(SqlConnection CN, DataTable dt, SqlTransaction sqltrans)
        {
            try
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    //string qry = "insert into index_of_property(District_Code,RO_Code,Book,Deed_year,Deed_no,Item_no,Property_district_code,Property_ro_code,ps_code,moucode,Area_type,GP_Muni_Corp_Code,Ward,Holding,Premises,road_code,Plot_code_type,Road,Plot_No,Bata_No,Khatian_type,khatian_No,bata_khatian_no,property_type,Land_Area_acre,Land_Area_bigha,Land_Area_decimal,Land_Area_katha,Land_Area_chatak,Land_Area_sqfeet,Structure_area_in_sqFeet,ref_ps,ref_mouza,jl_no,other_plots,other_khatian) values ('" + dt.Rows[i][0].ToString() + "','" + dt.Rows[i][1].ToString() + "','" + dt.Rows[i][2].ToString() + "','" + dt.Rows[i][3].ToString() + "','" + dt.Rows[i][4].ToString() + "','" + dt.Rows[i][5].ToString() + "','" + dt.Rows[i][6].ToString() + "','" + dt.Rows[i][7].ToString() + "','" + dt.Rows[i][8].ToString() + "','" + dt.Rows[i][9].ToString() + "','" + dt.Rows[i][10].ToString() + "','" + dt.Rows[i][11].ToString() + "','" + dt.Rows[i][12].ToString() + "','" + dt.Rows[i][13].ToString() + "','" + dt.Rows[i][14].ToString() + "','" + dt.Rows[i][15].ToString() + "','" + dt.Rows[i][16].ToString() + "','" + dt.Rows[i][17].ToString() + "','" + dt.Rows[i][18].ToString() + "','" + dt.Rows[i][19].ToString() + "','" + dt.Rows[i][20].ToString() + "','" + dt.Rows[i][21].ToString() + "','" + dt.Rows[i][22].ToString() + "','" + dt.Rows[i][23].ToString() + "','" + dt.Rows[i][24].ToString() + "','" + dt.Rows[i][25].ToString() + "','" + dt.Rows[i][26].ToString() + "','" + dt.Rows[i][27].ToString() + "','" + dt.Rows[i][28].ToString() + "','" + dt.Rows[i][29].ToString() + "','" + dt.Rows[i][30].ToString() + "','" + dt.Rows[i][31].ToString() + "','" + dt.Rows[i][32].ToString() + "','" + dt.Rows[i][33].ToString() + "','" + dt.Rows[i][34].ToString() + "','" + dt.Rows[i][35].ToString() + "')";
                    string qry = "insert into index_of_property(District_Code,RO_Code,Book,Deed_year,Deed_no,Item_no,Property_district_code,Property_district_name,Property_ro_code,ps_code,ps_name,moucode,mouja,Area_type,GP_Muni_Corp_Code,GP_Muni_name,Ward,Holding,Premises,road_code,Plot_code_type,Road,Plot_No,Bata_No,Khatian_type,khatian_No,bata_khatian_no,property_type,Land_Area_acre,Land_Area_bigha,Land_Area_decimal,Land_Area_katha,Land_Area_chatak,Land_Area_sqfeet,Structure_area_in_sqFeet,ref_ps,ref_mouza,jl_no,other_plots,other_khatian,land_type,refjl_no) values ('" + dt.Rows[i][0].ToString() + "','" + dt.Rows[i][1].ToString() + "','" + dt.Rows[i][2].ToString() + "','" + dt.Rows[i][3].ToString() + "','" + dt.Rows[i][4].ToString() + "','" + dt.Rows[i][5].ToString() + "','" + dt.Rows[i][6].ToString() + "','" + dt.Rows[i][7].ToString() + "','" + dt.Rows[i][8].ToString() + "','" + dt.Rows[i][9].ToString() + "','" + dt.Rows[i][10].ToString() + "','" + dt.Rows[i][11].ToString() + "','" + dt.Rows[i][12].ToString() + "','" + dt.Rows[i][13].ToString() + "','" + dt.Rows[i][14].ToString() + "','" + dt.Rows[i][15].ToString() + "','" + dt.Rows[i][16].ToString() + "','" + dt.Rows[i][17].ToString() + "','" + dt.Rows[i][18].ToString() + "','" + dt.Rows[i][19].ToString() + "','" + dt.Rows[i][20].ToString() + "','" + dt.Rows[i][21].ToString() + "','" + dt.Rows[i][22].ToString() + "','" + dt.Rows[i][23].ToString() + "','" + dt.Rows[i][24].ToString() + "','" + dt.Rows[i][25].ToString() + "','" + dt.Rows[i][26].ToString() + "','" + dt.Rows[i][27].ToString() + "','" + dt.Rows[i][28].ToString() + "','" + dt.Rows[i][29].ToString() + "','" + dt.Rows[i][30].ToString() + "','" + dt.Rows[i][31].ToString() + "','" + dt.Rows[i][32].ToString() + "','" + dt.Rows[i][33].ToString() + "','" + dt.Rows[i][34].ToString() + "','" + dt.Rows[i][35].ToString() + "','" + dt.Rows[i][36].ToString() + "','" + dt.Rows[i][37].ToString() + "','" + dt.Rows[i][38].ToString() + "','" + dt.Rows[i][39].ToString() + "','" + dt.Rows[i][40].ToString() + "','" + dt.Rows[i][41].ToString() + "')";
                    SqlCommand SqlCom = new SqlCommand(qry, CN);
                    SqlCom.Transaction = sqltrans;
                    SqlCom.ExecuteNonQuery();
                }
                return true;
            }
            catch (Exception ex)
            { return false; }
        }
        private bool SaveOtherPlot(SqlConnection CN, DataTable dt, SqlTransaction sqltrans)
        {
            try
            {
                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        string qry = "insert into other_plots(District_Code,RO_Code,Book,Deed_year,Deed_no,Item_no,other_plots) values ('" + dt.Rows[i][0].ToString() + "','" + dt.Rows[i][1].ToString() + "','" + dt.Rows[i][2].ToString() + "','" + dt.Rows[i][3].ToString() + "','" + dt.Rows[i][4].ToString() + "','" + dt.Rows[i][5].ToString() + "','" + dt.Rows[i][6].ToString() + "')";
                        //Initialize SqlCommand object for insert.
                        SqlCommand SqlCom = new SqlCommand(qry, CN);
                        SqlCom.Transaction = sqltrans;
                        SqlCom.ExecuteNonQuery();
                    }
                }
                return true;
            }
            catch (Exception ex)
            { return false; }
        }
        private bool SaveOtherKhatian(SqlConnection CN, DataTable dt, SqlTransaction sqltrans)
        {
            try
            {
                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        string qry = "insert into other_khatian(District_Code,RO_Code,Book,Deed_year,Deed_no,Item_no,other_khatian) values ('" + dt.Rows[i][0].ToString() + "','" + dt.Rows[i][1].ToString() + "','" + dt.Rows[i][2].ToString() + "','" + dt.Rows[i][3].ToString() + "','" + dt.Rows[i][4].ToString() + "','" + dt.Rows[i][5].ToString() + "','" + dt.Rows[i][6].ToString() + "')";
                        //Initialize SqlCommand object for insert.
                        SqlCommand SqlCom = new SqlCommand(qry, CN);
                        SqlCom.Transaction = sqltrans;
                        SqlCom.ExecuteNonQuery();
                    }
                }
                return true;
            }
            catch (Exception ex)
            { return false; }
        }
        private Boolean TakeBackup(string path)
        {
            string sqlConnectionString = "Server=localhost\\SQLEXPRESS;Initial Catalog=IGR;Integrated Security=True;Trusted_Connection=Yes;";
            string script = "BACKUP DATABASE [IGR] TO  DISK = '" + path + "\\IGR.bak" + "' WITH  INIT ,  NOUNLOAD ,  NAME = N'IGR backup',  NOSKIP ,  STATS = 10,  NOFORMAT";
            SqlConnection conn = new SqlConnection(sqlConnectionString);
            Server server = new Server(new ServerConnection(conn));
            server.ConnectionContext.ExecuteNonQuery(script);
            return true;
        }
        private bool DropDb(SqlConnection pSqlCon)
        {
            try
            {
                pSqlCon.Close();
                string qry = "ALTER DATABASE IGR SET SINGLE_USER WITH ROLLBACK IMMEDIATE;";
                string sql = @"Data Source=" + sqlIp + @"\SQLEXPRESS;Initial Catalog=master;Integrated Security=SSPI;";
                SqlConnection CN = new SqlConnection(sql);
                CN.Open();
                SqlCommand sqlCom1 = new SqlCommand();
                sqlCom1.Connection = CN;
                sqlCom1.CommandText = qry;
                sqlCom1.ExecuteNonQuery();
                SqlCommand sqlCom = new SqlCommand();
                sqlCom.Connection = CN;
                //IF  EXISTS (SELECT name FROM master.dbo.sysdatabases WHERE name = N'SQLServerPlanet')
                // DROP DATABASE [SQLServerPlanet]
                sqlCom.CommandText = "IF  EXISTS (SELECT name FROM master.dbo.sysdatabases WHERE name = N'IGR') DROP DATABASE [IGR]";
                //sqlCom.CommandText = "drop database IGR";
                sqlCom.ExecuteNonQuery();
                CN.Close();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private void bgwFileCopy_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

        }

        private void bgwFileCopy_DoWork(object sender, DoWorkEventArgs e)
        {
            ////for (int i = 0; i < listBoxFiles.Items.Count; i++)
            ////{
            ////    var mediaItem = (IMediaItem)listBoxFiles.Items[i];
            ////    string folderPath = mediaItem.Path.ToString();
            ////    if (Directory.Exists(folderPath))
            ////    {
            ////        //File Copy
            ////        //foreach (string dirPath in Directory.GetDirectories(folderPath, "*", SearchOption.AllDirectories))
            ////        //{
            ////        //    Directory.CreateDirectory(dirPath.Replace(folderPath, appPath + "\\NIC\\System\\" + listBoxFiles.Items[i].ToString()));
            ////        //}

            ////        //Copy all the files & Replaces any files with the same name
            ////        //foreach (string newPath in Directory.GetFiles(folderPath, "*.*", SearchOption.AllDirectories))
            ////        //{
            ////        //    File.Copy(newPath, newPath.Replace(folderPath, appPath + "\\NIC\\System\\" + listBoxFiles.Items[i].ToString()), true);
            ////        //}
            ////    }
            ////}
        }

        private void button2_Click(object sender, EventArgs e)
        {

            try
            {
                DataSet ds = new DataSet();
                ds = getBatch();
                if (ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        string folderPath = ds.Tables[0].Rows[i][1].ToString().Replace('\\', '\\');
                        var directoryItem = new DirectoryItem(folderPath);
                        listBoxFiles.Items.Add(directoryItem);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }
        private DataSet getBatch()
        {
            DataSet ds = new DataSet();
            string sql = "select batch_code,batch_path from batch_master where run_no = '"+cmbVol.SelectedValue.ToString()+"'";
            OdbcDataAdapter odap = new OdbcDataAdapter(sql, con);
            odap.Fill(ds);
            return ds;
        }

        private void cmdSreachRunno_Click(object sender, EventArgs e)
        {
            if (folderdialogforRunno.ShowDialog(this) == DialogResult.OK)
            {
                txtRunnoPath.Text = folderdialogforRunno.SelectedPath.ToString();
                DirectoryItem dItem = new DirectoryItem(folderdialogforRunno.SelectedPath);
                string[] subdirs = Directory.GetDirectories(folderdialogforRunno.SelectedPath);
                if (subdirs.Length > 0)
                {
                    foreach (string directory in subdirs)
                    {
                        listBoxFiles.Items.Add(new DirectoryItem(directory));
                        UpdateCapacity();
                    }
                }
            }
        }

    }
}
