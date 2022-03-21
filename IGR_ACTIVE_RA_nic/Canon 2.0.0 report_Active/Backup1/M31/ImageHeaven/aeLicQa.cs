/*
 * Created by SharpDevelop.
 * User: SubhajitB
 * Date: 4/4/2009
 * Time: 5:57 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Data;
using System.Data.Odbc;
using NovaNet.Utils;
using NovaNet.wfe;
using LItems;
using System.Collections;
using System.Drawing.Imaging;
using System.IO;
using igr_base;
using IGRFqc;
using DataLayerDefs;
using System.Drawing.Drawing2D;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace ImageHeaven
{
	/// <summary>
	/// Description of aeLicQa.
	/// </summary>
	public partial class aeLicQa : Form
	{
		OdbcConnection sqlCon=null;
		NovaNet.Utils.dbCon dbcon=null;
		CtrlPolicy pPolicy=null;
		private CtrlImage pImage=null;
		wfePolicy wPolicy=null;
		wfeImage wImage=null;
		private string boxNo=null;
		private string policyNumber=null;
        private string projCode = null;
        private string batchCode = null;
        private string picPath = null;
		private udtPolicy policyData=null;
		string policyPath=null;
		private int policyStatus=0;
		private int clickedIndexValue;
		private CtrlBox pBox=null;
		private int selBoxNo;
        string[] imageName;
        int policyRowIndex;
        int total_Count = 0;
        DataSet dsimage11 = new DataSet();
        //private CtrlBatch pBatch = null;

		//private MagickNet.Image imgQc;
		string imagePath=null;
		string photoPath=null;
		//private CtrlBox pBox=null;
		private Imagery img;
		private Imagery imgAll;
        private Credentials crd = new Credentials();
        public static NovaNet.Utils.exLog.Logger exMailLog = new NovaNet.Utils.exLog.emailLogger("./errLog.log", NovaNet.Utils.exLog.LogLevel.Dev,Constants._MAIL_TO, Constants._MAIL_FROM, Constants._SMTP);
        public static NovaNet.Utils.exLog.Logger exTxtLog = new NovaNet.Utils.exLog.txtLogger("./errLog.log", NovaNet.Utils.exLog.LogLevel.Dev);
        private string imgFileName = string.Empty;
        private int zoomWidth;
        private int zoomHeight;
        private Size zoomSize = new Size();
        private int keyPressed = 1;
        private DataTable gTable;
        ihwQuery wQ;
        private string selDocType = string.Empty;
        private int currntPg = 0;
        private bool firstDoc = true;
        private string prevDoc;
        private int policyLen = 0;
		public aeLicQa(OdbcConnection prmCon,Credentials prmCrd)
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			this.Name = "IGR quality control" ;
			InitializeComponent();
            sqlCon = prmCon;
			img = IgrFactory.GetImagery(Constants.IGR_CLEARIMAGE);
			//img = IgrFactory.GetImagery(Constants.IGR_GDPICTURE);
			imgAll= IgrFactory.GetImagery(Constants.IGR_CLEARIMAGE);
            crd = prmCrd;
            exMailLog.SetNextLogger(exTxtLog);
            dgvProperty.BringToFront();
            
			//img = IgrFactory.GetImagery(Constants.IGR_GDPICTURE);			
			//
			// TODO: Add constructor code after the InitializeComponent() call.
			//
		}
		private void PopulateBatchCombo()
		{
			string projKey=null;
			DataSet ds=new DataSet();
			
			dbcon=new NovaNet.Utils.dbCon();
            NovaNet.wfe.eSTATES[] bState = new NovaNet.wfe.eSTATES[2];
			wfeBatch tmpBatch=new wfeBatch(sqlCon);
			if(cmbProject.SelectedValue != null)
			{
				projKey=cmbProject.SelectedValue.ToString();
                projCode = projKey;
                wQ = new ihwQuery(sqlCon);
                if (wQ.GetSysConfigValue(ihConstants.CENT_PERCENT_FQC_KEY) == ihConstants.CENT_PERCENT_FQC_VALUE)
                {
                    ds = tmpBatch.GetAllValues(Convert.ToInt32(projKey));
                }
                else
                {
                    bState[0] = eSTATES.BATCH_FQC;
                    bState[1] = eSTATES.BATCH_READY_FOR_UAT;
                    ds = tmpBatch.GetAllValues(Convert.ToInt32(projKey),bState);
                }
                if (ds.Tables[0].Rows.Count > 0)
                {
                    comboBox1.DataSource = ds.Tables[0];
                    comboBox1.DisplayMember = ds.Tables[0].Columns[1].ToString();
                    comboBox1.ValueMember = ds.Tables[0].Columns[0].ToString();
                }
                else
                {
                    comboBox1.DataSource = ds.Tables[0];
                }
			}
		}
		private void PopulateProjectCombo()
		{
			DataSet ds=new DataSet();
			
			dbcon=new NovaNet.Utils.dbCon();
			
			wfeProject tmpProj=new wfeProject(sqlCon);
			//cmbProject.Items.Add("Select");
			ds=tmpProj.GetAllValues();
            if (ds.Tables[0].Rows.Count > 0)
            {
                cmbProject.DataSource = ds.Tables[0];
                cmbProject.DisplayMember = ds.Tables[0].Columns[1].ToString();
                cmbProject.ValueMember = ds.Tables[0].Columns[0].ToString();
            }
		}

		private void PopulateBoxDetails()
		{
			string batchKey=null;
			DataSet ds=new DataSet();
            CtrlBox cBox = new CtrlBox((int)cmbProject.SelectedValue,(int)cmbBatch.SelectedValue,"0");
			dbcon=new NovaNet.Utils.dbCon();
			
			wfeBox tmpBox=new wfeBox(sqlCon,cBox);
			DataTable dt=new DataTable();
			DataSet imageCount = new DataSet();
        	DataRow dr;
        	int indexPolicyCont=0;
        	double avgSize;
        	string totSize;
        	string totPage;
        	NovaNet.wfe.eSTATES[] state=new NovaNet.wfe.eSTATES[5];
            NovaNet.wfe.eSTATES[] policyState = new NovaNet.wfe.eSTATES[5];
        	
            dt.Columns.Add("BoxNo");
            dt.Columns.Add("Policies");
            dt.Columns.Add("Ready");
			dt.Columns.Add("ScannedPages");
			dt.Columns.Add("Avg_Size");
			dt.Columns.Add("TotalSize");
			
			if(cmbBatch.SelectedValue != null)
			{
				batchKey=cmbBatch.SelectedValue.ToString();
                batchCode = batchKey;
				ds=tmpBox.GetAllBox(Convert.ToInt32(batchKey));
				if(ds.Tables[0].Rows.Count>0)
				{
					for(int i=0;i< ds.Tables[0].Rows.Count;i++)
					{
						 dr = dt.NewRow();
						 dr["BoxNo"] = ds.Tables[0].Rows[i]["box_number"];
	            		 dr["Policies"] = ds.Tables[0].Rows[i]["policy_number"].ToString();
	            		 
	            		 pPolicy = new CtrlPolicy(Convert.ToInt32(cmbProject.SelectedValue.ToString()),Convert.ToInt32(cmbBatch.SelectedValue.ToString()),ds.Tables[0].Rows[i]["box_number"].ToString(),"0");
		    			 wPolicy=new wfePolicy(sqlCon,pPolicy);
		    			 
            			 policyState[0]=NovaNet.wfe.eSTATES.POLICY_INDEXED;
                         policyState[1] = NovaNet.wfe.eSTATES.POLICY_FQC;
                         policyState[2] = NovaNet.wfe.eSTATES.POLICY_CHECKED;
                         policyState[3] = NovaNet.wfe.eSTATES.POLICY_EXCEPTION;
                         policyState[4] = NovaNet.wfe.eSTATES.POLICY_EXPORTED;
		    			 indexPolicyCont = wPolicy.GetPolicyCount(policyState);
		    			 
		    			 dr["Ready"] = indexPolicyCont;
		    			 
		    			 pImage = new CtrlImage(Convert.ToInt32(cmbProject.SelectedValue.ToString()),Convert.ToInt32(cmbBatch.SelectedValue.ToString()),ds.Tables[0].Rows[i]["box_number"].ToString(),"0",string.Empty,string.Empty);
		    			 wImage  = new wfeImage(sqlCon, pImage);
		    			 
	    			      state[0]=eSTATES.PAGE_INDEXED;
			              state[1]=eSTATES.PAGE_FQC;
			              state[2]=eSTATES.PAGE_CHECKED;
			              state[3]=eSTATES.PAGE_EXCEPTION;
                          state[4] = eSTATES.PAGE_EXPORTED;
                          //state[5] = eSTATES.PAGE_ON_HOLD;
				         imageCount = wImage.GetReadyImageCount(state,policyState);
		    			 totPage =imageCount .Tables[0].Rows[0]["page_count"].ToString();
		    			 dr["ScannedPages"] = totPage;
		    			 totSize=imageCount.Tables[0].Rows[0]["index_size"].ToString();
                         if (totSize != string.Empty)
                         {
                             dr["TotalSize"] = Math.Round(Convert.ToDouble(totSize), 2);
                         }
                         else
                         {
                             dr["TotalSize"] = string.Empty;
                         }
						 
		    			 if((totSize != string.Empty) && (totPage != "0"))
		    			 {
		    			 	avgSize =Math.Round(Convert.ToDouble(totSize)  / Convert.ToDouble(totPage),2);
		    			 	dr["Avg_Size"] = avgSize.ToString();
		    			 }
		    			
		    			 dt.Rows.Add(dr);
					}
					grdBox.DataSource = dt;
                    grdBox.ForeColor = Color.Black;
				}
			}
		}
		void CmbProjectLeave(object sender, EventArgs e)
		{
			PopulateBatchCombo();
		}
		
		void AeLicQaLoad(object sender, EventArgs e)
		{
            System.Diagnostics.Process.GetCurrentProcess().PriorityClass = System.Diagnostics.ProcessPriorityClass.RealTime;
            System.Windows.Forms.ToolTip bttnToolTip = new System.Windows.Forms.ToolTip();
            System.Windows.Forms.ToolTip otherToolTip = new System.Windows.Forms.ToolTip();
            this.WindowState = FormWindowState.Maximized;
			PopulateProjectCombo();
			rdoShowAll.Checked = true;
            cmdZoomIn.ForeColor = Color.Black;
            cmdZoomOut.ForeColor = Color.Black;
            chkRejectBatch.Visible = false;
            bttnToolTip.SetToolTip(cmdZoomIn,"Shortcut Key- (+)") ;
            bttnToolTip.SetToolTip(cmdZoomOut,"Shortcut Key- (-)") ;
            label6.ForeColor = Color.Black;
            label7.ForeColor = Color.Black;
            label8.ForeColor = Color.Black;
            label9.ForeColor = Color.Black;
            txtPolicyNumber.ForeColor = Color.DarkRed;
            txtName.ForeColor = Color.DarkRed;
            populateDistrict();
            populateRunNum();
            
            //cmdRefine.Enabled = false;
		}
        void populateRO()
        {
            if (cmbDistrict.DataSource != null)
            {
                wfeBox dly = new wfeBox(sqlCon);
                string districtCode = cmbDistrict.SelectedValue.ToString();
                cmbWhereReg.DataSource = dly.GetROffice(districtCode).Tables[0];
                cmbWhereReg.DisplayMember = "RO_name";
                cmbWhereReg.ValueMember = "RO_code";
            }
        }
        private void populateRunNum()
        {
            DataSet ds = new DataSet();
            wfeBox dly = new wfeBox(sqlCon);
            ds = dly.GetRunnum();
            if (ds.Tables[0].Rows.Count > 0)
            {
                cmbRunnum.DataSource = ds.Tables[0];
                cmbRunnum.DisplayMember = "run_no";
                cmbRunnum.ValueMember = "run_no";
            }

        }
        void populateYear()
        {
            if (cmbDistrict.DataSource != null && cmbWhereReg.DataSource != null)
            {
                wfeBox dly = new wfeBox(sqlCon);
                string districtCode = cmbDistrict.SelectedValue.ToString();
                string ro_code = cmbWhereReg.SelectedValue.ToString();
                cmbyear.DataSource = dly.GetYear(districtCode, ro_code).Tables[0];
                cmbyear.DisplayMember = "deed_year";


            }
        }
        void populateDistrict()
        {
            wfeBox dly = new wfeBox(sqlCon);
            cmbDistrict.DataSource = dly.GetDistrict().Tables[0];
            cmbDistrict.DisplayMember = "district_name";
            cmbDistrict.ValueMember = "district_code";
            cmbDistrict.SelectedIndex = 0;

            if (cmbDistrict.DataSource != null)
            {
                string districtCode = cmbDistrict.SelectedValue.ToString();
                cmbWhereReg.DataSource = dly.GetROffice(districtCode).Tables[0];
                cmbWhereReg.DisplayMember = "RO_name";
                cmbWhereReg.ValueMember = "RO_code";
            }
            if (cmbDistrict.DataSource != null && cmbWhereReg.DataSource != null)
            {
                string districtCode = cmbDistrict.SelectedValue.ToString();
                string ro_code = cmbWhereReg.SelectedValue.ToString();
                cmbyear.DataSource = dly.GetYear(districtCode, ro_code).Tables[0];
                cmbyear.DisplayMember = "deed_year";


            }
            cmbBook.DataSource = dly.GetBook().Tables[0];
            cmbBook.DisplayMember = "key_book";
            cmbBook.ValueMember = "value_book";

            //cmbBook.SelectedIndex = 0;
        }
        
		void CmbBatchLeave(object sender, EventArgs e)
		{
            //try
            //{
            //    if ((cmbProject.SelectedValue != null) && (cmbBatch.SelectedValue != null))
            //    {
            //        wfeBox wBox;
            //        PopulateBoxDetails();
            //        eSTATES state = new eSTATES();

            //        eSTATES[] tempState = new eSTATES[5];
            //        eSTATES[] policyState = new eSTATES[5];
            //        pBox = new CtrlBox(Convert.ToInt32(cmbProject.SelectedValue.ToString()), Convert.ToInt32(cmbBatch.SelectedValue.ToString()), "0");
            //        wBox = new wfeBox(sqlCon, pBox);
            //        lblTotPolicies.Text = wBox.GetTotalPolicies(state).ToString();
            //        lblPolRcvd.Text = Convert.ToString((Convert.ToInt32(lblTotPolicies.Text) - Convert.ToInt32(wBox.GetTotalPolicies(eSTATES.POLICY_MISSING))));
            //        lblPolHold.Text = wBox.GetTotalPolicies(eSTATES.POLICY_ON_HOLD).ToString();

            //        policyState[0] = NovaNet.wfe.eSTATES.POLICY_INDEXED;
            //        policyState[1] = NovaNet.wfe.eSTATES.POLICY_FQC;
            //        policyState[2] = NovaNet.wfe.eSTATES.POLICY_CHECKED;
            //        policyState[3] = NovaNet.wfe.eSTATES.POLICY_EXCEPTION;
            //        policyState[4] = NovaNet.wfe.eSTATES.POLICY_EXPORTED;
            //        lblScannedPol.Text = wBox.GetTotalPolicies(policyState).ToString();
            //        lblBatchSz.Text = wBox.GetTotalBatchSize().ToString();
            //        tempState[0] = eSTATES.PAGE_INDEXED;
            //        tempState[1] = eSTATES.PAGE_FQC;
            //        tempState[2] = eSTATES.PAGE_CHECKED;
            //        tempState[3] = eSTATES.PAGE_EXCEPTION;
            //        tempState[4] = eSTATES.PAGE_EXPORTED;
            //        int scannedPol = Convert.ToInt32(lblScannedPol.Text);
            //        lblAvgDocketSz.Text = Convert.ToString(Math.Round(Convert.ToDouble(Convert.ToDouble(lblBatchSz.Text) / scannedPol), 2));
            //        lblTotImages.Text = wBox.GetTotalImageCount(tempState, false, policyState).ToString();
            //        lblSigCount.Text = wBox.GetTotalImageCount(tempState, true, policyState).ToString();
            //        lblNetImageCount.Text = Convert.ToString(wBox.GetTotalImageCount(tempState, false, policyState) - wBox.GetTotalImageCount(tempState, true, policyState));
            //        double bSize = Convert.ToInt32(lblBatchSz.Text) * 1024;
            //        double tImage = Convert.ToInt32(lblTotImages.Text);
            //        double aImageSize = bSize / tImage;
            //        lblAvgImageSize.Text = Math.Round(aImageSize,1).ToString() + " KB";
            //        wfeBatch wBatch = new wfeBatch(sqlCon);
            //        if (wBatch.GetBatchStatus(Convert.ToInt32(cmbBatch.SelectedValue.ToString())) == (int)eSTATES.BATCH_READY_FOR_UAT)
            //        {
            //            chkReadyUat.Enabled = false;
            //            chkReadyUat.Checked = true;
            //            cmdAccepted.Enabled = false;
            //            cmdRejected.Enabled = false;
            //        }
            //        else
            //        {
            //            chkReadyUat.Enabled = true;
            //            chkReadyUat.Checked = false;
            //            cmdAccepted.Enabled = true;
            //            cmdRejected.Enabled = true;
            //        }
            //        CheckBatchRejection(cmbBatch.SelectedValue.ToString());
            //        lblTotPol.Text = wBox.GetLICCheckedCount().ToString();
            //    }
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show("Error while generating batch information........" + "  " + ex.Message);
            //}
		}
		void PolicyDetails(string prmBoxNo)
		{
			DataTable dt=new DataTable();
			DataRow dr;
			DataSet ds = new DataSet();
			DataSet dsPolicy = new DataSet();
			DataSet dsImage = new DataSet();
			eSTATES[] filterState = new eSTATES[1];
        	double avgSize;
        	string totSize = string.Empty;
        	string totPage;
        	string yr;
        	string mm;
        	string dd;
            NovaNet.wfe.eSTATES[] state = new NovaNet.wfe.eSTATES[6];

			dt.Columns.Add("SrlNo");
            dt.Columns.Add("Policy");
            dt.Columns.Add("Name");
            dt.Columns.Add("CommencementDt");
			dt.Columns.Add("DOB");
			dt.Columns.Add("ScannedPages");
			dt.Columns.Add("TotalSize");
			dt.Columns.Add("Avg_Size");
			dt.Columns.Add("STATUS");
            dt.Columns.Add("POLICYSTATUS");
			dt.Columns.Add("PROPOSALFORM");
            dt.Columns.Add("PHOTOADDENDUM");
			dt.Columns.Add("PROPOSALENCLOSERS");
            dt.Columns.Add("SIGNATUREPAGE");
            dt.Columns.Add("MEDICALREPORT");
			dt.Columns.Add("PROPOSALREVIEWSLIP");
			dt.Columns.Add("POLICYBOND");
			dt.Columns.Add("NOMINATION");
			dt.Columns.Add("ASSIGNMENT");
            dt.Columns.Add("ALTERATION");
            dt.Columns.Add("REVIVALS");
			dt.Columns.Add("POLICYLOANS");
			dt.Columns.Add("SURRENDER");
			dt.Columns.Add("CLAIMS");
			dt.Columns.Add("CORRESPONDENCE");
			dt.Columns.Add("OTHERS");
            dt.Columns.Add("KYCDOCUMENT");
            
            
			if((prmBoxNo != string.Empty) && (prmBoxNo != null) && (cmbProject.SelectedValue.ToString() != string.Empty) && (cmbProject.SelectedValue.ToString() != null) && (cmbBatch.SelectedValue.ToString() != string.Empty) && ((cmbBatch.SelectedValue.ToString() != null)))
			{
				boxNo = prmBoxNo;
				pPolicy = new CtrlPolicy(Convert.ToInt32(cmbProject.SelectedValue.ToString()),Convert.ToInt32(cmbBatch.SelectedValue.ToString()),prmBoxNo.ToString(),"0");
				wPolicy=new wfePolicy(sqlCon,pPolicy);
				if(rdoShowAll.Checked == true)
				{
					eSTATES[] allState = new eSTATES[0];
					dsPolicy = wPolicy.GetPolicyList(allState);
				}
				if(rdoChecked.Checked == true)
				{
					filterState[0]=eSTATES.POLICY_CHECKED;
					dsPolicy = wPolicy.GetPolicyList(filterState);
				}
				if(rdoExceptions.Checked == true)
				{
					filterState[0]=eSTATES.POLICY_EXCEPTION;
					dsPolicy = wPolicy.GetPolicyList(filterState);
				}
				
				if(rdoOnHold.Checked == true)
				{
					filterState[0]=eSTATES.POLICY_ON_HOLD;
					dsPolicy = wPolicy.GetPolicyList(filterState);
				}
                if (rdoMissing.Checked == true)
                {
                    filterState[0] = eSTATES.POLICY_MISSING;
                    dsPolicy = wPolicy.GetPolicyList(filterState);
                }

                if (rdo150.Checked == true)
                {
                    eSTATES[] allState = new eSTATES[0];
                    dsPolicy = wPolicy.GetPolicyList(allState);
                }

                for (int i = 0; i < dsPolicy.Tables[0].Rows.Count; i++)
                {
                    pImage = new CtrlImage(Convert.ToInt32(cmbProject.SelectedValue.ToString()), Convert.ToInt32(cmbBatch.SelectedValue.ToString()), prmBoxNo, dsPolicy.Tables[0].Rows[i]["policy_no"].ToString(), string.Empty, string.Empty);
                    wImage = new wfeImage(sqlCon, pImage);

                    //NovaNet.wfe.eSTATES[] state = new NovaNet.wfe.eSTATES[4];
                    state[0] = NovaNet.wfe.eSTATES.PAGE_EXCEPTION;
                    state[1] = NovaNet.wfe.eSTATES.PAGE_INDEXED;
                    state[2] = NovaNet.wfe.eSTATES.PAGE_CHECKED;
                    state[3] = NovaNet.wfe.eSTATES.PAGE_FQC;
                    state[4] = NovaNet.wfe.eSTATES.PAGE_EXPORTED;
                    state[5] = NovaNet.wfe.eSTATES.PAGE_ON_HOLD;
                    dsImage = wImage.GetPolicyWiseImageInfo(state);
                    if (rdo150.Checked == true)
                    {
                        
                        totSize = dsImage.Tables[0].Rows[0]["qc_size"].ToString();
                        if (totSize != String.Empty)
                        {
                            double totFileSize = Convert.ToDouble(totSize) / 1024;
                            if (Convert.ToDouble(totFileSize) > ihConstants._DOCKET_MAX_SIZE)
                            {
                                if ((Convert.ToInt32(dsPolicy.Tables[0].Rows[i]["status"].ToString()) != (int)eSTATES.POLICY_SCANNED) && (Convert.ToInt32(dsPolicy.Tables[0].Rows[i]["status"].ToString()) != (int)eSTATES.POLICY_QC) && (Convert.ToInt32(dsPolicy.Tables[0].Rows[i]["status"].ToString()) != (int)eSTATES.POLICY_ON_HOLD))
                                {
                                    dr = dt.NewRow();
                                    dr["SrlNo"] = i + 1;
                                    dr["Policy"] = dsPolicy.Tables[0].Rows[i]["policy_no"].ToString();
                                    dr["Name"] = dsPolicy.Tables[0].Rows[i]["name_of_policyholder"].ToString();
                                    yr = dsPolicy.Tables[0].Rows[i]["date_of_birth"].ToString().Substring(0, 4);


                                    mm = dsPolicy.Tables[0].Rows[i]["date_of_birth"].ToString().Substring(4, 2);
                                    dd = dsPolicy.Tables[0].Rows[i]["date_of_birth"].ToString().Substring(6, 2);
                                    dr["DOB"] = yr + "/" + mm + "/" + dd;

                                    yr = dsPolicy.Tables[0].Rows[i]["date_of_commencement"].ToString().Substring(0, 4);

                                    mm = dsPolicy.Tables[0].Rows[i]["date_of_commencement"].ToString().Substring(4, 2);
                                    dd = dsPolicy.Tables[0].Rows[i]["date_of_commencement"].ToString().Substring(6, 2);
                                    dr["CommencementDt"] = yr + "/" + mm + "/" + dd;

                                    pImage = new CtrlImage(Convert.ToInt32(cmbProject.SelectedValue.ToString()), Convert.ToInt32(cmbBatch.SelectedValue.ToString()), prmBoxNo.ToString(), dsPolicy.Tables[0].Rows[i]["policy_no"].ToString(), string.Empty, string.Empty);
                                    wImage = new wfeImage(sqlCon, pImage);

                                    if ((Convert.ToInt32(dsPolicy.Tables[0].Rows[i]["status"].ToString()) != (int)eSTATES.POLICY_MISSING))
                                    {
                                        totPage = dsImage.Tables[0].Rows[0]["page_count"].ToString();
                                    }
                                    else
                                    {
                                        totPage = "0";
                                    }
                                    dr["ScannedPages"] = totPage;
                                    if ((Convert.ToInt32(dsPolicy.Tables[0].Rows[i]["status"].ToString()) != (int)eSTATES.POLICY_MISSING))
                                    {
                                        totSize = dsImage.Tables[0].Rows[0]["qc_size"].ToString();
                                    }
                                    else
                                    {
                                        totSize = string.Empty;
                                    }
                                    if (totSize != string.Empty)
                                    {
                                        totSize = Convert.ToString(Math.Round(Convert.ToDouble(totSize), 2));
                                    }
                                    dr["TotalSize"] = totSize;

                                    dr["STATUS"] = dsPolicy.Tables[0].Rows[i]["status"];

                                    if ((Convert.ToInt32(dsPolicy.Tables[0].Rows[i]["status"].ToString()) == (int)eSTATES.POLICY_INDEXED) || (Convert.ToInt32(dsPolicy.Tables[0].Rows[i]["status"].ToString()) == (int)eSTATES.POLICY_FQC) || (Convert.ToInt32(dsPolicy.Tables[0].Rows[i]["status"].ToString()) == (int)eSTATES.POLICY_NOT_INDEXED))
                                    {
                                        dr["POLICYSTATUS"] = "Indexed";
                                    }
                                    if ((Convert.ToInt32(dsPolicy.Tables[0].Rows[i]["status"].ToString()) == (int)eSTATES.POLICY_ON_HOLD))
                                    {
                                        dr["POLICYSTATUS"] = "On hold";
                                    }
                                    if ((Convert.ToInt32(dsPolicy.Tables[0].Rows[i]["status"].ToString()) == (int)eSTATES.POLICY_MISSING))
                                    {
                                        dr["POLICYSTATUS"] = "Missing";
                                    }
                                    if ((Convert.ToInt32(dsPolicy.Tables[0].Rows[i]["status"].ToString()) == (int)eSTATES.POLICY_EXCEPTION))
                                    {
                                        dr["POLICYSTATUS"] = "In exception";
                                    }
                                    if ((Convert.ToInt32(dsPolicy.Tables[0].Rows[i]["status"].ToString()) == (int)eSTATES.POLICY_CHECKED))
                                    {
                                        dr["POLICYSTATUS"] = "Checked";
                                    }
                                    if ((Convert.ToInt32(dsPolicy.Tables[0].Rows[i]["status"].ToString()) == (int)eSTATES.POLICY_EXPORTED))
                                    {
                                        dr["POLICYSTATUS"] = "Exported";
                                    }
                                    if ((Convert.ToInt32(dsPolicy.Tables[0].Rows[i]["status"].ToString()) == (int)eSTATES.POLICY_SCANNED) || (Convert.ToInt32(dsPolicy.Tables[0].Rows[i]["status"].ToString()) == (int)eSTATES.POLICY_QC) || (Convert.ToInt32(dsPolicy.Tables[0].Rows[i]["status"].ToString()) == (int)eSTATES.POLICY_ON_HOLD))
                                    {
                                        dr["ScannedPages"] = "0";
                                        dr["TotalSize"] = string.Empty;
                                        totPage = "0";
                                        totSize = string.Empty;
                                    }
                                    if ((totSize != string.Empty) && (totPage != "0"))
                                    {
                                        avgSize = Convert.ToDouble(totSize) / Convert.ToDouble(totPage);
                                        dr["Avg_Size"] =Convert.ToString(Math.Round(avgSize,2));
                                    }
                                    if ((Convert.ToInt32(dsPolicy.Tables[0].Rows[i]["status"].ToString()) == (int)eSTATES.POLICY_SCANNED) || (Convert.ToInt32(dsPolicy.Tables[0].Rows[i]["status"].ToString()) == (int)eSTATES.POLICY_QC) || (Convert.ToInt32(dsPolicy.Tables[0].Rows[i]["status"].ToString()) == (int)eSTATES.POLICY_ON_HOLD) || (Convert.ToInt32(dsPolicy.Tables[0].Rows[i]["status"].ToString()) == (int)eSTATES.POLICY_MISSING))
                                    {
                                        dr["PROPOSALFORM"] = "0";
                                        dr["PHOTOADDENDUM"] = "0";
                                        dr["PROPOSALENCLOSERS"] = "0";
                                        dr["SIGNATUREPAGE"] = "0";
                                        dr["MEDICALREPORT"] = "0";
                                        dr["PROPOSALREVIEWSLIP"] = "0";
                                        dr["POLICYBOND"] = "0";
                                        dr["NOMINATION"] = "0";
                                        dr["ASSIGNMENT"] = "0";
                                        dr["ALTERATION"] = "0";
                                        dr["REVIVALS"] = "0";
                                        dr["POLICYLOANS"] = "0";
                                        dr["SURRENDER"] = "0";
                                        dr["CLAIMS"] = "0";
                                        dr["CORRESPONDENCE"] = "0";
                                        dr["OTHERS"] = "0";
                                        dr["KYCDOCUMENT"] = "0";
                                    }
                                    else
                                    {
                                        dr["PROPOSALFORM"] = wImage.GetDocTypeCount(ihConstants.PROPOSALFORM_FILE);
                                        dr["PHOTOADDENDUM"] = wImage.GetDocTypeCount(ihConstants.PHOTOADDENDUM_FILE);
                                        dr["PROPOSALENCLOSERS"] = wImage.GetDocTypeCount(ihConstants.PROPOSALENCLOSERS_FILE);
                                        dr["SIGNATUREPAGE"] = wImage.GetDocTypeCount(ihConstants.SIGNATUREPAGE_FILE);
                                        dr["MEDICALREPORT"] = wImage.GetDocTypeCount(ihConstants.MEDICALREPORT_FILE);
                                        dr["PROPOSALREVIEWSLIP"] = wImage.GetDocTypeCount(ihConstants.PROPOSALREVIEWSLIP_FILE);
                                        dr["POLICYBOND"] = wImage.GetDocTypeCount(ihConstants.POLICYBOND_FILE);
                                        dr["NOMINATION"] = wImage.GetDocTypeCount(ihConstants.NOMINATION_FILE);
                                        dr["ASSIGNMENT"] = wImage.GetDocTypeCount(ihConstants.ASSIGNMENT_FILE);
                                        dr["ALTERATION"] = wImage.GetDocTypeCount(ihConstants.ALTERATION_FILE);
                                        dr["REVIVALS"] = wImage.GetDocTypeCount(ihConstants.REVIVALS_FILE);
                                        dr["POLICYLOANS"] = wImage.GetDocTypeCount(ihConstants.POLICYLOANS_FILE);
                                        dr["SURRENDER"] = wImage.GetDocTypeCount(ihConstants.SURRENDER_FILE);
                                        dr["CLAIMS"] = wImage.GetDocTypeCount(ihConstants.CLAIMS_FILE);
                                        dr["CORRESPONDENCE"] = wImage.GetDocTypeCount(ihConstants.CORRESPONDENCE_FILE);
                                        dr["OTHERS"] = wImage.GetDocTypeCount(ihConstants.OTHERS_FILE);
                                        dr["KYCDOCUMENT"] = wImage.GetDocTypeCount(ihConstants.KYCDOCUMENT_FILE);
                                    }
                                    dt.Rows.Add(dr);
                                }
                            }
                        }
                    }
                    else
                    {
                        dr = dt.NewRow();
                        dr["SrlNo"] = i + 1;
                        dr["Policy"] = dsPolicy.Tables[0].Rows[i]["policy_no"].ToString();
                        dr["Name"] = dsPolicy.Tables[0].Rows[i]["name_of_policyholder"].ToString();
                        yr = dsPolicy.Tables[0].Rows[i]["date_of_birth"].ToString().Substring(0, 4);


                        mm = dsPolicy.Tables[0].Rows[i]["date_of_birth"].ToString().Substring(4, 2);
                        dd = dsPolicy.Tables[0].Rows[i]["date_of_birth"].ToString().Substring(6, 2);
                        dr["DOB"] = yr + "/" + mm + "/" + dd;

                        yr = dsPolicy.Tables[0].Rows[i]["date_of_commencement"].ToString().Substring(0, 4);

                        mm = dsPolicy.Tables[0].Rows[i]["date_of_commencement"].ToString().Substring(4, 2);
                        dd = dsPolicy.Tables[0].Rows[i]["date_of_commencement"].ToString().Substring(6, 2);
                        dr["CommencementDt"] = yr + "/" + mm + "/" + dd;


                        if ((Convert.ToInt32(dsPolicy.Tables[0].Rows[i]["status"].ToString()) != (int)eSTATES.POLICY_MISSING))
                        {
                            totPage = dsImage.Tables[0].Rows[0]["page_count"].ToString();
                        }
                        else
                        {
                            totPage = "0";
                        }
                        dr["ScannedPages"] = totPage;
                        if ((Convert.ToInt32(dsPolicy.Tables[0].Rows[i]["status"].ToString()) != (int)eSTATES.POLICY_MISSING))
                        {
                            totSize = dsImage.Tables[0].Rows[0]["qc_size"].ToString();
                        }
                        else
                        {
                            totSize = string.Empty;
                        }
                        if (totSize != string.Empty)
                        {
                            totSize = Convert.ToString(Math.Round(Convert.ToDouble(totSize), 2));
                        }
                        dr["TotalSize"] = totSize;
                        dr["STATUS"] = dsPolicy.Tables[0].Rows[i]["status"];
                        if ((Convert.ToInt32(dsPolicy.Tables[0].Rows[i]["status"].ToString()) == (int)eSTATES.POLICY_INDEXED) || (Convert.ToInt32(dsPolicy.Tables[0].Rows[i]["status"].ToString()) == (int)eSTATES.POLICY_FQC) || (Convert.ToInt32(dsPolicy.Tables[0].Rows[i]["status"].ToString()) == (int)eSTATES.POLICY_NOT_INDEXED))
                        {
                            dr["POLICYSTATUS"] = "Indexed";
                        }
                        if ((Convert.ToInt32(dsPolicy.Tables[0].Rows[i]["status"].ToString()) == (int)eSTATES.POLICY_ON_HOLD))
                        {
                            dr["POLICYSTATUS"] = "On hold";
                        }
                        if ((Convert.ToInt32(dsPolicy.Tables[0].Rows[i]["status"].ToString()) == (int)eSTATES.POLICY_MISSING))
                        {
                            dr["POLICYSTATUS"] = "Missing";
                        }
                        if ((Convert.ToInt32(dsPolicy.Tables[0].Rows[i]["status"].ToString()) == (int)eSTATES.POLICY_EXCEPTION))
                        {
                            dr["POLICYSTATUS"] = "In exception";
                        }
                        if ((Convert.ToInt32(dsPolicy.Tables[0].Rows[i]["status"].ToString()) == (int)eSTATES.POLICY_CHECKED))
                        {
                            dr["POLICYSTATUS"] = "Checked";
                        }
                        if ((Convert.ToInt32(dsPolicy.Tables[0].Rows[i]["status"].ToString()) == (int)eSTATES.POLICY_EXPORTED))
                        {
                            dr["POLICYSTATUS"] = "Exported";
                        }
                        if ((Convert.ToInt32(dsPolicy.Tables[0].Rows[i]["status"].ToString()) == (int)eSTATES.POLICY_SCANNED) || (Convert.ToInt32(dsPolicy.Tables[0].Rows[i]["status"].ToString()) == (int)eSTATES.POLICY_QC) || (Convert.ToInt32(dsPolicy.Tables[0].Rows[i]["status"].ToString()) == (int)eSTATES.POLICY_ON_HOLD) || (Convert.ToInt32(dsPolicy.Tables[0].Rows[i]["status"].ToString()) == (int)eSTATES.POLICY_NOT_INDEXED))
                        {
                            dr["ScannedPages"] = "0";
                            dr["TotalSize"] = string.Empty;
                            totPage = "0";
                            totSize = string.Empty;
                        }
                        if ((totSize != string.Empty) && (totPage != "0"))
                        {
                            avgSize = Convert.ToDouble(totSize) / Convert.ToDouble(totPage);
                            dr["Avg_Size"] = Convert.ToString(Math.Round(avgSize, 2));
                        }
                        if ((Convert.ToInt32(dsPolicy.Tables[0].Rows[i]["status"].ToString()) == (int)eSTATES.POLICY_SCANNED) || (Convert.ToInt32(dsPolicy.Tables[0].Rows[i]["status"].ToString()) == (int)eSTATES.POLICY_QC) || (Convert.ToInt32(dsPolicy.Tables[0].Rows[i]["status"].ToString()) == (int)eSTATES.POLICY_ON_HOLD) || (Convert.ToInt32(dsPolicy.Tables[0].Rows[i]["status"].ToString()) == (int)eSTATES.POLICY_MISSING))
                        {
                            dr["PROPOSALFORM"] = "0";
                            dr["PHOTOADDENDUM"] = "0";
                            dr["PROPOSALENCLOSERS"] = "0";
                            dr["SIGNATUREPAGE"] = "0";
                            dr["MEDICALREPORT"] = "0";
                            dr["PROPOSALREVIEWSLIP"] = "0";
                            dr["POLICYBOND"] = "0";
                            dr["NOMINATION"] = "0";
                            dr["ASSIGNMENT"] = "0";
                            dr["ALTERATION"] = "0";
                            dr["REVIVALS"] = "0";
                            dr["POLICYLOANS"] = "0";
                            dr["SURRENDER"] = "0";
                            dr["CLAIMS"] = "0";
                            dr["CORRESPONDENCE"] = "0";
                            dr["OTHERS"] = "0";
                            dr["KYCDOCUMENT"] = "0";
                        }
                        else
                        {
                            dr["PROPOSALFORM"] = wImage.GetDocTypeCount(ihConstants.PROPOSALFORM_FILE);
                            dr["PHOTOADDENDUM"] = wImage.GetDocTypeCount(ihConstants.PHOTOADDENDUM_FILE);
                            dr["PROPOSALENCLOSERS"] = wImage.GetDocTypeCount(ihConstants.PROPOSALENCLOSERS_FILE);
                            dr["SIGNATUREPAGE"] = wImage.GetDocTypeCount(ihConstants.SIGNATUREPAGE_FILE);
                            dr["MEDICALREPORT"] = wImage.GetDocTypeCount(ihConstants.MEDICALREPORT_FILE);
                            dr["PROPOSALREVIEWSLIP"] = wImage.GetDocTypeCount(ihConstants.PROPOSALREVIEWSLIP_FILE);
                            dr["POLICYBOND"] = wImage.GetDocTypeCount(ihConstants.POLICYBOND_FILE);
                            dr["NOMINATION"] = wImage.GetDocTypeCount(ihConstants.NOMINATION_FILE);
                            dr["ASSIGNMENT"] = wImage.GetDocTypeCount(ihConstants.ASSIGNMENT_FILE);
                            dr["ALTERATION"] = wImage.GetDocTypeCount(ihConstants.ALTERATION_FILE);
                            dr["REVIVALS"] = wImage.GetDocTypeCount(ihConstants.REVIVALS_FILE);
                            dr["POLICYLOANS"] = wImage.GetDocTypeCount(ihConstants.POLICYLOANS_FILE);
                            dr["SURRENDER"] = wImage.GetDocTypeCount(ihConstants.SURRENDER_FILE);
                            dr["CLAIMS"] = wImage.GetDocTypeCount(ihConstants.CLAIMS_FILE);
                            dr["CORRESPONDENCE"] = wImage.GetDocTypeCount(ihConstants.CORRESPONDENCE_FILE);
                            dr["OTHERS"] = wImage.GetDocTypeCount(ihConstants.OTHERS_FILE);
                            dr["KYCDOCUMENT"] = wImage.GetDocTypeCount(ihConstants.KYCDOCUMENT_FILE);
                        }

                        dt.Rows.Add(dr);
                    }
                }
				if(dt.Rows.Count > 0)
				{
					grdPolicy.DataSource = ds;
					grdPolicy.DataSource = dt;
				}
				else
				{
					grdPolicy.DataSource = ds;
				}

                if ((grdPolicy.Rows.Count > 0))
                {
                    for (int l = 0; l < grdPolicy.Rows.Count; l++)
                    {
                        if (Convert.ToInt32(grdPolicy.Rows[l].Cells[8].Value.ToString()) == (int)eSTATES.POLICY_CHECKED)
                        {
                            grdPolicy.Rows[l].DefaultCellStyle.ForeColor = Color.Black;
                            grdPolicy.Rows[l].DefaultCellStyle.BackColor = Color.Green;
                        }
                        if ((Convert.ToInt32(grdPolicy.Rows[l].Cells[8].Value.ToString()) == (int)eSTATES.POLICY_EXCEPTION) || (Convert.ToInt32(grdPolicy.Rows[l].Cells[8].Value.ToString()) == (int)eSTATES.POLICY_EXCEPTION))
                        {
                            grdPolicy.Rows[l].DefaultCellStyle.ForeColor = Color.Black;
                            grdPolicy.Rows[l].DefaultCellStyle.BackColor = Color.Red;
                        }
                        if ((Convert.ToInt32(grdPolicy.Rows[l].Cells[8].Value.ToString()) == (int)eSTATES.POLICY_ON_HOLD))
                        {
                            grdPolicy.Rows[l].DefaultCellStyle.ForeColor = Color.Black;
                            grdPolicy.Rows[l].DefaultCellStyle.BackColor = Color.Turquoise;
                        }
                        if ((Convert.ToInt32(grdPolicy.Rows[l].Cells[8].Value.ToString()) == (int)eSTATES.POLICY_MISSING))
                        {
                            grdPolicy.Rows[l].DefaultCellStyle.ForeColor = Color.Black;
                            grdPolicy.Rows[l].DefaultCellStyle.BackColor = Color.Magenta;
                        }
                    }

                }
                if (dt.Rows.Count > 0)
                {
                    grdPolicy.Columns[8].Visible = false;
                    grdPolicy.Columns[0].Width = 40;
                    grdPolicy.Columns[1].Width = 70;
                    grdPolicy.Columns[2].Width = 120;
                    grdPolicy.Columns[3].Width = 70;
                    grdPolicy.Columns[4].Width = 70;
                    grdPolicy.Columns[5].Width = 40;
                    grdPolicy.Columns[6].Width = 50;
                    grdPolicy.Columns[7].Width = 60;
                    grdPolicy.Columns[8].Width = 60;
                    grdPolicy.Columns[9].Width = 60;
                    grdPolicy.Columns[10].Width = 30;
                    grdPolicy.Columns[11].Width = 30;
                    grdPolicy.Columns[12].Width = 30;
                    grdPolicy.Columns[13].Width = 30;
                    grdPolicy.Columns[14].Width = 30;
                    grdPolicy.Columns[15].Width = 30;
                    grdPolicy.Columns[16].Width = 30;
                    grdPolicy.Columns[17].Width = 30;
                    grdPolicy.Columns[18].Width = 30;
                    grdPolicy.Columns[19].Width = 30;
                    grdPolicy.Columns[20].Width = 30;
                    grdPolicy.Columns[21].Width = 30;
                    grdPolicy.Columns[22].Width = 30;
                    grdPolicy.Columns[23].Width = 30;
                    grdPolicy.Columns[24].Width = 30;
                    grdPolicy.Columns[25].Width = 30;
                    grdPolicy.Columns[26].Width = 30;
                }
                
			}
		}
        private string policy_number = string.Empty;
        int selectedIndex = 0;
		void GrdBoxCellDoubleClick(object sender, DataGridViewCellEventArgs e)
		{
            wfeImage wfeimg = new wfeImage(sqlCon);
            selectedIndex = e.RowIndex;
            string deedno = grdBox.Rows[e.RowIndex].Cells[0].Value.ToString() + grdBox.Rows[e.RowIndex].Cells[1].Value.ToString() + grdBox.Rows[e.RowIndex].Cells[2].Value.ToString() + grdBox.Rows[e.RowIndex].Cells[3].Value.ToString() + "[" + grdBox.Rows[e.RowIndex].Cells[4].Value.ToString() + "]";
            policy_number = deedno;
            string deed_no = grdBox.Rows[e.RowIndex].Cells[4].Value.ToString();
            DataTable dt1 = wfeimg.GetAllNameEX(grdBox.Rows[e.RowIndex].Cells[0].Value.ToString(),grdBox.Rows[e.RowIndex].Cells[1].Value.ToString(),grdBox.Rows[e.RowIndex].Cells[2].Value.ToString(),grdBox.Rows[e.RowIndex].Cells[3].Value.ToString(),grdBox.Rows[e.RowIndex].Cells[4].Value.ToString());
            dgvname.DataSource = dt1;
            dgvname.Columns[0].Width = dgvname.Width / 2;
            dgvname.Columns[1].Width = dgvname.Width / 2;
            dgvname.Columns[2].Visible = false;
            for (int i = 0; i < dgvname.Rows.Count; i++)
            {
                dgvname.Rows[i].Height = 100;
                dgvname.Rows[i].Selected = false;
            }

            DataTable dt2 = wfeimg.GetAllPropEX(grdBox.Rows[e.RowIndex].Cells[0].Value.ToString(), grdBox.Rows[e.RowIndex].Cells[1].Value.ToString(), grdBox.Rows[e.RowIndex].Cells[2].Value.ToString(), grdBox.Rows[e.RowIndex].Cells[3].Value.ToString(), grdBox.Rows[e.RowIndex].Cells[4].Value.ToString());
            dgvProperty.DataSource = dt2;
            for (int i = 0; i < dgvProperty.Rows.Count; i++)
            {
                dgvProperty.Rows[i].Height = 100;
                dgvProperty.Rows[i].Selected = false;
            }
            dgvProperty.Columns[3].Visible = false;
            dgvProperty.Columns[4].Visible = false;
            dgvProperty.Columns[5].Visible = false;
            dgvProperty.Columns[6].Visible = false;
            dgvProperty.Columns[7].Visible = false;
            dgvProperty.Columns[8].Visible = false;
            dgvProperty.Columns[9].Visible = false;
            dgvProperty.Columns[10].Visible = false;
            dgvProperty.Columns[11].Visible = false;
            dgvProperty.Columns[12].Visible = false;
            dgvProperty.Columns[13].Visible = false;
            dgvProperty.Columns[14].Visible = false;
            dgvProperty.Columns[15].Visible = false;
            dgvProperty.Columns[16].Visible = false;
            dgvProperty.Columns[17].Visible = false;
            dgvProperty.Columns[18].Visible = false;
            dgvProperty.Columns[19].Visible = false;
            dgvProperty.Columns[20].Visible = false;
            dgvProperty.Columns[21].Visible = false;
            dgvProperty.Columns[22].Visible = true;
            dgvProperty.Columns[0].Width = dgvProperty.Width / 4;
            dgvProperty.Columns[1].Width = dgvProperty.Width / 4;
            dgvProperty.Columns[2].Width = dgvProperty.Width / 4;
            dgvProperty.Columns[22].Width = dgvProperty.Width / 4;

            ShowImages(grdBox.Rows[e.RowIndex].Cells[0].Value.ToString(), grdBox.Rows[e.RowIndex].Cells[1].Value.ToString(), grdBox.Rows[e.RowIndex].Cells[2].Value.ToString(), grdBox.Rows[e.RowIndex].Cells[3].Value.ToString(), grdBox.Rows[e.RowIndex].Cells[4].Value.ToString());
            
		}
        private string GetPolicyPath(int policyNo)
        {
            //policyLst = (ListBox)BoxDtls.Controls["lstPolicy"];
            wfeBatch wBatch = new wfeBatch(sqlCon);
            string batchPath = wBatch.GetPath(Convert.ToInt32(cmbProject.SelectedValue.ToString()),Convert.ToInt32(cmbBatch.SelectedValue.ToString()));
            return batchPath + "\\" + boxNo + "\\" + policyNo;
        }
        private void GetAuditException()
        {
            try
            {
                ClearPicBox();

                //firstDoc = true;
                DataSet expDs = new DataSet();
                //////clickedIndexValue = e.RowIndex;
                //picControl.Image = null;
                //lstImage.Items.Clear();
                //DisplayDockType();
                //policyNumber = grdPolicy.Rows[e.RowIndex].Cells[1].Value.ToString();
                //policyLen = policyNumber.Length;
                //txtPolicyNumber.Text = policyNumber;
                //txtName.Text = grdPolicy.Rows[e.RowIndex].Cells[2].Value.ToString();
                //txtDOB.Text = grdPolicy.Rows[e.RowIndex].Cells[4].Value.ToString();
                //txtCommDt.Text = grdPolicy.Rows[e.RowIndex].Cells[3].Value.ToString();
                //policyRowIndex = e.RowIndex;
                //if (Convert.ToDouble(grdPolicy.Rows[e.RowIndex].Cells[5].Value.ToString()) > 0)
                //{
                    //for (int i = 0; i < grdPolicy.Columns.Count - 10; i++)
                    //{
                    //    lvwDockTypes.Items[i].SubItems[1].Text = grdPolicy.Rows[e.RowIndex].Cells[i + 10].Value.ToString();
                    //}
                    //lblTotFiles.Text = Convert.ToString(Math.Round(Convert.ToDouble(grdPolicy.Rows[e.RowIndex].Cells[5].Value.ToString()), 2));
                    //lblAvgSize.Text = Convert.ToString(Math.Round(Convert.ToDouble(grdPolicy.Rows[e.RowIndex].Cells[7].Value.ToString()), 2)) + " KB";
                    //lblDock.Text = Convert.ToString(Math.Round(Convert.ToDouble(grdPolicy.Rows[e.RowIndex].Cells[6].Value.ToString()), 2)) + " KB";
                    //policyStatus = Convert.ToInt32(grdPolicy.Rows[e.RowIndex].Cells[8].Value.ToString());
                    //if (policyStatus == (int)eSTATES.POLICY_EXPORTED)
                    //{
                    //    cmdAccepted.Enabled = false;
                    //    cmdRejected.Enabled = false;
                    //}
                    //else
                    //{
                    //    cmdAccepted.Enabled = true;
                    //    cmdRejected.Enabled = true;
                    //}
                    //lstImage.Items.Clear();
                pPolicy = new CtrlPolicy(Convert.ToInt32(dsimage11.Tables[0].Rows[0][0].ToString()), Convert.ToInt32(dsimage11.Tables[0].Rows[0][1].ToString()), "1",policy_number);    
                //pPolicy = new CtrlPolicy(Convert.ToInt32(cmbProject.SelectedValue.ToString()), Convert.ToInt32(cmbBatch.SelectedValue.ToString()), boxNo.ToString(), grdPolicy.Rows[e.RowIndex].Cells[1].Value.ToString());
                    wfePolicy policy = new wfePolicy(sqlCon, pPolicy);
                    //policyData = (udtPolicy)policy.LoadValuesFromDB();
                    //policyPath = GetPolicyPath(Convert.ToInt32(policyNumber)); //policyData.policy_path;
                    expDs = policy.GetAllException();
                    if (expDs.Tables[0].Rows.Count > 0)
                    {
                        if (Convert.ToInt32(expDs.Tables[0].Rows[0]["missing_img_exp"].ToString()) == 1)
                        {
                            chkMissingImg.Checked = true;
                        }
                        else
                        {
                            chkMissingImg.Checked = false;
                        }

                        if (Convert.ToInt32(expDs.Tables[0].Rows[0]["crop_clean_exp"].ToString()) == 1)
                        {
                            chkCropClean.Checked = true;
                        }
                        else
                        {
                            chkCropClean.Checked = false;
                        }

                        if (Convert.ToInt32(expDs.Tables[0].Rows[0]["poor_scan_exp"].ToString()) == 1)
                        {
                            chkPoorScan.Checked = true;
                        }
                        else
                        {
                            chkPoorScan.Checked = false;
                        }
                        if (Convert.ToInt32(expDs.Tables[0].Rows[0]["wrong_indexing_exp"].ToString()) == 1)
                        {
                            chkIndexing.Checked = true;
                        }
                        else
                        {
                            chkIndexing.Checked = false;
                        }
                        if (Convert.ToInt32(expDs.Tables[0].Rows[0]["linked_policy_exp"].ToString()) == 1)
                        {
                            chkWrongProperty.Checked = true;
                        }
                        else
                        {
                            chkWrongProperty.Checked = false;
                        }
                        if (Convert.ToInt32(expDs.Tables[0].Rows[0]["decision_misd_exp"].ToString()) == 1)
                        {
                            chkSpelmis.Checked = true;
                        }
                        else
                        {
                            chkSpelmis.Checked = false;
                        }
                        if (Convert.ToInt32(expDs.Tables[0].Rows[0]["extra_page_exp"].ToString()) == 1)
                        {
                            chkExtraPage.Checked = true;
                        }
                        else
                        {
                            chkExtraPage.Checked = false;
                        }
                        if (Convert.ToInt32(expDs.Tables[0].Rows[0]["decision_misd_exp"].ToString()) == 1)
                        {
                            chkDesicion.Checked = true;
                        }
                        else
                        {
                            chkDesicion.Checked = false;
                        }
                        if (Convert.ToInt32(expDs.Tables[0].Rows[0]["rearrange_exp"].ToString()) == 1)
                        {
                            chkRearrange.Checked = true;
                        }
                        else
                        {
                            chkRearrange.Checked = false;
                        }
                        if (Convert.ToInt32(expDs.Tables[0].Rows[0]["other_exp"].ToString()) == 1)
                        {
                            chkOther.Checked = true;
                        }
                        else
                        {
                            chkOther.Checked = false;
                        }
                        if (Convert.ToInt32(expDs.Tables[0].Rows[0]["move_to_respective_policy_exp"].ToString()) == 1)
                        {
                            chkMove.Checked = true;
                        }
                        else
                        {
                            chkMove.Checked = false;
                        }
                        txtComments.Text = expDs.Tables[0].Rows[0]["comments"].ToString() + "\r\n";
                        txtComments.SelectionStart = txtComments.Text.Length;
                        txtComments.ScrollToCaret();
                        txtComments.Refresh();
                    }
                    else
                    {
                        chkMissingImg.Checked = false;
                        chkCropClean.Checked = false;
                        chkPoorScan.Checked = false;
                        chkIndexing.Checked = false;
                        chkLinkedPolicy.Checked = false;
                        chkDesicion.Checked = false;
                        chkExtraPage.Checked = false;
                        chkDesicion.Checked = false;
                        chkRearrange.Checked = false;
                        chkOther.Checked = false;
                        chkMove.Checked = false;
                        txtComments.Text = string.Empty;
                    }

                    ArrayList arrImage = new ArrayList();
                    wQuery pQuery = new ihwQuery(sqlCon);
                    eSTATES[] state = new eSTATES[5];
                    state[0] = eSTATES.POLICY_CHECKED;
                    state[1] = eSTATES.POLICY_FQC;
                    state[2] = eSTATES.POLICY_INDEXED;
                    state[3] = eSTATES.POLICY_EXCEPTION;
                    state[4] = eSTATES.POLICY_EXPORTED;
                    CtrlImage ctrlImage;
                    arrImage = pQuery.GetItems(eITEMS.LIC_QA_PAGE, state, policy);
                    for (int i = 0; i < arrImage.Count; i++)
                    {
                        ctrlImage = (CtrlImage)arrImage[i];
                        if (ctrlImage.DocType != string.Empty)
                        {
                            lstImage.Items.Add(ctrlImage.ImageName + "-" + ctrlImage.DocType);
                        }
                        else
                            lstImage.Items.Add(ctrlImage.ImageName);
                    }
                    tabControl1.SelectedIndex = 1;
                    if (lstImage.Items.Count > 0)
                    {
                        lstImage.SelectedIndex = 0;
                        if (checkBox1.Checked == true)
                        {
                            cmdAccepted.Enabled = false;
                            cmdRejected.Enabled = false;
                        }
                        else
                        {
                            cmdAccepted.Enabled = true;
                            cmdRejected.Enabled = true;
                        }
                    }

                //}
                //else
                //{
                //    cmdAccepted.Enabled = false;
                //    cmdRejected.Enabled = false;
                //}

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error while getting the information of the selected policy.....");
                exMailLog.Log(ex);
            }
        }
		void GrdPolicyCellDoubleClick(object sender, DataGridViewCellEventArgs e)
		{
            try
            {
                ClearPicBox();
                
                firstDoc = true;
                DataSet expDs = new DataSet();
                clickedIndexValue = e.RowIndex;
                picControl.Image = null;
                lstImage.Items.Clear();
                DisplayDockType();
                policyNumber = grdPolicy.Rows[e.RowIndex].Cells[1].Value.ToString();
                policyLen = policyNumber.Length;
                txtPolicyNumber.Text = policyNumber;
                txtName.Text = grdPolicy.Rows[e.RowIndex].Cells[2].Value.ToString();
                txtDOB.Text = grdPolicy.Rows[e.RowIndex].Cells[4].Value.ToString();
                txtCommDt.Text = grdPolicy.Rows[e.RowIndex].Cells[3].Value.ToString();
                policyRowIndex = e.RowIndex;
                if (Convert.ToDouble(grdPolicy.Rows[e.RowIndex].Cells[5].Value.ToString()) > 0)
                {
                    for (int i = 0; i < grdPolicy.Columns.Count - 10; i++)
                    {
                        lvwDockTypes.Items[i].SubItems[1].Text = grdPolicy.Rows[e.RowIndex].Cells[i + 10].Value.ToString();
                    }
                    lblTotFiles.Text = Convert.ToString(Math.Round(Convert.ToDouble(grdPolicy.Rows[e.RowIndex].Cells[5].Value.ToString()), 2));
                    lblAvgSize.Text = Convert.ToString(Math.Round(Convert.ToDouble(grdPolicy.Rows[e.RowIndex].Cells[7].Value.ToString()), 2)) + " KB";
                    lblDock.Text = Convert.ToString(Math.Round(Convert.ToDouble(grdPolicy.Rows[e.RowIndex].Cells[6].Value.ToString()), 2)) + " KB";
                    policyStatus = Convert.ToInt32(grdPolicy.Rows[e.RowIndex].Cells[8].Value.ToString());
                    if (policyStatus == (int)eSTATES.POLICY_EXPORTED)
                    {
                        cmdAccepted.Enabled = false;
                        cmdRejected.Enabled = false;
                    }
                    else
                    {
                        cmdAccepted.Enabled = true;
                        cmdRejected.Enabled = true;
                    }
                    //lstImage.Items.Clear();
                    pPolicy = new CtrlPolicy(Convert.ToInt32(cmbProject.SelectedValue.ToString()), Convert.ToInt32(cmbBatch.SelectedValue.ToString()), boxNo.ToString(), grdPolicy.Rows[e.RowIndex].Cells[1].Value.ToString());
                    wfePolicy policy = new wfePolicy(sqlCon, pPolicy);
                    //policyData = (udtPolicy)policy.LoadValuesFromDB();
                    policyPath = GetPolicyPath(Convert.ToInt32(policyNumber)); //policyData.policy_path;
                    expDs = policy.GetAllException();
                    if (expDs.Tables[0].Rows.Count > 0)
                    {
                        if (Convert.ToInt32(expDs.Tables[0].Rows[0]["missing_img_exp"].ToString()) == 1)
                        {
                            chkMissingImg.Checked = true;
                        }
                        else
                        {
                            chkMissingImg.Checked = false;
                        }

                        if (Convert.ToInt32(expDs.Tables[0].Rows[0]["crop_clean_exp"].ToString()) == 1)
                        {
                            chkCropClean.Checked = true;
                        }
                        else
                        {
                            chkCropClean.Checked = false;
                        }

                        if (Convert.ToInt32(expDs.Tables[0].Rows[0]["poor_scan_exp"].ToString()) == 1)
                        {
                            chkPoorScan.Checked = true;
                        }
                        else
                        {
                            chkPoorScan.Checked = false;
                        }
                        if (Convert.ToInt32(expDs.Tables[0].Rows[0]["wrong_indexing_exp"].ToString()) == 1)
                        {
                            chkIndexing.Checked = true;
                        }
                        else
                        {
                            chkIndexing.Checked = false;
                        }
                        if (Convert.ToInt32(expDs.Tables[0].Rows[0]["linked_policy_exp"].ToString()) == 1)
                        {
                            chkLinkedPolicy.Checked = true;
                        }
                        else
                        {
                            chkLinkedPolicy.Checked = false;
                        }
                        if (Convert.ToInt32(expDs.Tables[0].Rows[0]["decision_misd_exp"].ToString()) == 1)
                        {
                            chkDesicion.Checked = true;
                        }
                        else
                        {
                            chkDesicion.Checked = false;
                        }
                        if (Convert.ToInt32(expDs.Tables[0].Rows[0]["extra_page_exp"].ToString()) == 1)
                        {
                            chkExtraPage.Checked = true;
                        }
                        else
                        {
                            chkExtraPage.Checked = false;
                        }
                        if (Convert.ToInt32(expDs.Tables[0].Rows[0]["decision_misd_exp"].ToString()) == 1)
                        {
                            chkDesicion.Checked = true;
                        }
                        else
                        {
                            chkDesicion.Checked = false;
                        }
                        if (Convert.ToInt32(expDs.Tables[0].Rows[0]["rearrange_exp"].ToString()) == 1)
                        {
                            chkRearrange.Checked = true;
                        }
                        else
                        {
                            chkRearrange.Checked = false;
                        }
                        if (Convert.ToInt32(expDs.Tables[0].Rows[0]["other_exp"].ToString()) == 1)
                        {
                            chkOther.Checked = true;
                        }
                        else
                        {
                            chkOther.Checked = false;
                        }
                        if (Convert.ToInt32(expDs.Tables[0].Rows[0]["move_to_respective_policy_exp"].ToString()) == 1)
                        {
                            chkMove.Checked = true;
                        }
                        else
                        {
                            chkMove.Checked = false;
                        }
                        txtComments.Text = expDs.Tables[0].Rows[0]["comments"].ToString() + "\r\n";
                        txtComments.SelectionStart = txtComments.Text.Length;
                        txtComments.ScrollToCaret();
                        txtComments.Refresh();
                    }
                    else
                    {
                        chkMissingImg.Checked = false;
                        chkCropClean.Checked = false;
                        chkPoorScan.Checked = false;
                        chkIndexing.Checked = false;
                        chkLinkedPolicy.Checked = false;
                        chkDesicion.Checked = false;
                        chkExtraPage.Checked = false;
                        chkDesicion.Checked = false;
                        chkRearrange.Checked = false;
                        chkOther.Checked = false;
                        chkMove.Checked = false;
                        txtComments.Text = string.Empty;
                    }

                    ArrayList arrImage = new ArrayList();
                    wQuery pQuery = new ihwQuery(sqlCon);
                    eSTATES[] state = new eSTATES[5];
                    state[0] = eSTATES.POLICY_CHECKED;
                    state[1] = eSTATES.POLICY_FQC;
                    state[2] = eSTATES.POLICY_INDEXED;
                    state[3] = eSTATES.POLICY_EXCEPTION;
                    state[4] = eSTATES.POLICY_EXPORTED;
                    CtrlImage ctrlImage;
                    arrImage = pQuery.GetItems(eITEMS.LIC_QA_PAGE, state, policy);
                    for (int i = 0; i < arrImage.Count; i++)
                    {
                        ctrlImage = (CtrlImage)arrImage[i];
                        if (ctrlImage.DocType != string.Empty)
                        {
                            lstImage.Items.Add(ctrlImage.ImageName + "-" + ctrlImage.DocType);
                        }
                        else
                            lstImage.Items.Add(ctrlImage.ImageName);
                    }
                    tabControl1.SelectedIndex = 1;
                    if (lstImage.Items.Count > 0)
                    {
                        lstImage.SelectedIndex = 0;
                        cmdAccepted.Enabled = true;
                        cmdRejected.Enabled = true;
                    }
                    
                }
                else
                {
                    cmdAccepted.Enabled = false;
                    cmdRejected.Enabled = false;
                }
                
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error while getting the information of the selected policy.....");
                exMailLog.Log(ex);
            }
		}
		void DisplayDockType()
		{
			lvwDockTypes.Items.Clear();
			ListViewItem lvwItem = lvwDockTypes.Items.Add(ihConstants.PROPOSALFORM_FILE);
			lvwItem.SubItems.Add("0");

            lvwItem = lvwDockTypes.Items.Add(ihConstants.PHOTOADDENDUM_FILE);
            lvwItem.SubItems.Add("0");

			lvwItem=lvwDockTypes.Items.Add(ihConstants.PROPOSALENCLOSERS_FILE);
			lvwItem.SubItems.Add("0");

			lvwItem=lvwDockTypes.Items.Add(ihConstants.SIGNATUREPAGE_FILE);
			lvwItem.SubItems.Add("0");
			
			lvwItem=lvwDockTypes.Items.Add(ihConstants.MEDICALREPORT_FILE);
			lvwItem.SubItems.Add("0");
			
			lvwItem=lvwDockTypes.Items.Add(ihConstants.PROPOSALREVIEWSLIP_FILE);
			lvwItem.SubItems.Add("0");
			
			lvwItem=lvwDockTypes.Items.Add(ihConstants.POLICYBOND_FILE);
			lvwItem.SubItems.Add("0");
			
			lvwItem=lvwDockTypes.Items.Add(ihConstants.NOMINATION_FILE);
			lvwItem.SubItems.Add("0");
			
			lvwItem=lvwDockTypes.Items.Add(ihConstants.ASSIGNMENT_FILE);
			lvwItem.SubItems.Add("0");
			
			lvwItem=lvwDockTypes.Items.Add(ihConstants.ALTERATION_FILE);
			lvwItem.SubItems.Add("0");
			
			lvwItem=lvwDockTypes.Items.Add(ihConstants.REVIVALS_FILE);
			lvwItem.SubItems.Add("0");
			
			lvwItem=lvwDockTypes.Items.Add(ihConstants.POLICYLOANS_FILE);
			lvwItem.SubItems.Add("0");
			
			lvwItem=lvwDockTypes.Items.Add(ihConstants.SURRENDER_FILE);
			lvwItem.SubItems.Add("0");
			
			lvwItem=lvwDockTypes.Items.Add(ihConstants.CLAIMS_FILE);
			lvwItem.SubItems.Add("0");
			
			lvwItem=lvwDockTypes.Items.Add(ihConstants.CORRESPONDENCE_FILE);
			lvwItem.SubItems.Add("0");
			
			lvwItem=lvwDockTypes.Items.Add(ihConstants.OTHERS_FILE);
			lvwItem.SubItems.Add("0");

            lvwItem = lvwDockTypes.Items.Add(ihConstants.KYCDOCUMENT_FILE);
            lvwItem.SubItems.Add("0");
		}
		
		void LstImageSelectedIndexChanged(object sender, EventArgs e)
		{
            try
            {

                string aa = policyPath;
                imgFileName = policyPath + @"\" + lstImage.SelectedItem.ToString();
                img.LoadBitmapFromFile(imgFileName);
                picControl.Image = img.GetBitmap();
                //picControl.SizeMode = PictureBoxSizeMode.StretchImage;
                ChangeSize();
                //ZoomIn();
                //picControl.Refresh();
                //ZoomIn();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Document is not Scanned Yet, Please Scan the Document First..."+ ex.Message.ToString());
            }
            //int pos;
            //string changedImage=null;
            //double fileSize;
            //string currntDoc ;
            //wfeImage wImage = null;
            ////string photoImageName=null;

            //try
            //{
            //    pos = lstImage.SelectedItem.ToString().IndexOf("-");
            //    changedImage = lstImage.SelectedItem.ToString().Substring(0, pos);
            //    //changedImage=lstImage.SelectedItem.ToString().Substring(0,pos);
            //    pImage = new CtrlImage(Convert.ToInt32(cmbProject.SelectedValue.ToString()), Convert.ToInt32(cmbBatch.SelectedValue.ToString()), boxNo.ToString(), policyNumber, changedImage, string.Empty);
            //    wImage = new wfeImage(sqlCon, pImage);
            //    changedImage = wImage.GetIndexedImageName();

            //    if ((policyStatus == (int)eSTATES.POLICY_INDEXED) || (policyStatus == (int)eSTATES.POLICY_CHECKED) || (policyStatus == (int)eSTATES.POLICY_EXCEPTION) || (policyStatus == (int)eSTATES.POLICY_EXPORTED))
            //    {
            //        if (Directory.Exists(policyPath + "\\" + ihConstants._FQC_FOLDER))
            //        {
            //            picPath = policyPath + "\\" + ihConstants._FQC_FOLDER;
            //            imagePath = policyPath + "\\" + ihConstants._FQC_FOLDER + "\\" + changedImage;
            //            if (changedImage.Substring(policyLen, 6) == "_000_A")
            //            {
            //                imgFileName = policyPath + "\\" + ihConstants._FQC_FOLDER + "\\" + changedImage;
            //                if (File.Exists(imgFileName) == false)
            //                {
            //                    imgFileName = policyPath + "\\" + ihConstants._INDEXING_FOLDER + "\\" + changedImage;
            //                    picPath = policyPath + "\\" + ihConstants._INDEXING_FOLDER;
            //                }
            //                //img.SaveAsTiff(policyPath + "\\" + ihConstants._FQC_FOLDER + "\\" + changedImage, IGRComressionTIFF.JPEG);
            //                photoPath = imagePath;
            //            }
            //            else
            //            {
            //                imgFileName = policyPath + "\\" + ihConstants._FQC_FOLDER + "\\" + changedImage;
            //                if (File.Exists(imgFileName) == false)
            //                {
            //                    imgFileName = policyPath + "\\" + ihConstants._INDEXING_FOLDER + "\\" + changedImage;
            //                    picPath = policyPath + "\\" + ihConstants._INDEXING_FOLDER;
            //                }
            //            }
            //        }
            //        else
            //        {
            //            imagePath = policyPath + "\\" + ihConstants._INDEXING_FOLDER + "\\" + changedImage;
            //            picPath = policyPath + "\\" + ihConstants._INDEXING_FOLDER;
            //            if (changedImage.Substring(policyLen, 6) == "_000_A")
            //            {
            //                imgFileName = policyPath + "\\" + ihConstants._INDEXING_FOLDER + "\\" + changedImage;
            //                img.LoadBitmapFromFile(imgFileName);
            //                //img.SaveAsTiff(policyPath + "\\" + ihConstants._INDEXING_FOLDER + "\\" + changedImage, IGRComressionTIFF.JPEG);
            //                photoPath = imagePath;
            //            }
            //            else
            //            {
            //                imgFileName = policyPath + "\\" + ihConstants._INDEXING_FOLDER + "\\" + changedImage;
                            
            //            }
            //        }

            //    }
            //    else
            //    {
            //        picPath = policyPath + "\\" + ihConstants._FQC_FOLDER;
            //        imagePath = policyPath + "\\" + ihConstants._FQC_FOLDER + "\\" + changedImage;
            //        if (changedImage.Substring(policyLen, 6) == "_000_A")
            //        {
            //            imgFileName = policyPath + "\\" + ihConstants._FQC_FOLDER + "\\" + changedImage;
            //            if (File.Exists(imgFileName) == false)
            //            {
            //                imgFileName = policyPath + "\\" + ihConstants._INDEXING_FOLDER + "\\" + changedImage;
            //                picPath = policyPath + "\\" + ihConstants._INDEXING_FOLDER;
            //            }
            //            //img.SaveAsTiff(policyPath + "\\" + ihConstants._FQC_FOLDER + "\\" + changedImage, IGRComressionTIFF.JPEG);
            //            photoPath = imagePath;
            //        }
            //        else
            //        {
            //            imgFileName = policyPath + "\\" + ihConstants._FQC_FOLDER + "\\" + changedImage;
            //            if (File.Exists(imgFileName) == false)
            //            {
            //                imgFileName = policyPath + "\\" + ihConstants._INDEXING_FOLDER + "\\" + changedImage;
            //                picPath = policyPath + "\\" + ihConstants._INDEXING_FOLDER;
            //            }
            //        }

            //    }
            //    System.IO.FileInfo info = new System.IO.FileInfo(imgFileName);

            //    fileSize = info.Length;
            //    fileSize = fileSize / 1024;
            //    lblImageSize.Text = Convert.ToString(Math.Round(fileSize, 2)) + " KB";
            //    img.LoadBitmapFromFile(imgFileName);
            //    int dashPos = lstImage.SelectedItem.ToString().IndexOf("-") + 1;
            //    //currntDoc = lstImage.Items[lstImage.SelectedIndex].ToString().Substring(dashPos);
                
            //    //if ((prevDoc != currntDoc))
            //    //{
            //    //    ListViewItem lvwItem = lvwDockTypes.FindItemWithText(currntDoc);
            //    //    lvwDockTypes.Items[lvwItem.Index].Selected = true;
            //    //}
            //    //firstDoc = false;
            //    if (imgFileName != string.Empty)
            //    {
            //        ChangeSize();
            //    }
            //    //prevDoc = currntDoc;
            //    //ChangeSize();
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show("Error while generating the preview....");
            //    exMailLog.Log(ex);
            //}
		}
        public System.Drawing.Image CreateThumbnail(System.Drawing.Image pImage, int lnWidth, int lnHeight)
        {

            Bitmap bmp = new Bitmap(lnWidth, lnHeight);
            try
            {

                DateTime stdt = DateTime.Now;

                //create a new Bitmap the size of the new image

                //create a new graphic from the Bitmap
                Graphics graphic = Graphics.FromImage((System.Drawing.Image)bmp);
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
            return (System.Drawing.Image)bmp;
        }
		private void ChangeSize()
		{
            System.Drawing.Image imgTot = null;
            try
            {
                if (img.IsValid() == true)
                {
                    System.Drawing.Image newImage = img.GetBitmap();
                	if(newImage.PixelFormat== PixelFormat.Format1bppIndexed)
            		{
                        picControl.Height = tabControl2.Height;
                        picControl.Width = tabControl2.Width;
	                    //if (!System.IO.File.Exists(imgFileName)) return;
                        
                        //imgAll.LoadBitmapFromFile(imgFileName);
                        //if (imgAll.GetBitmap().PixelFormat == PixelFormat.Format24bppRgb)
                        //{
                        //    imgAll.GetLZW("tmp1.TIF");
                        //    imgTot = Image.FromFile("tmp1.TIF");
                        //    newImage = imgTot;
                        //    //File.Delete("tmp1.TIF");
                        //}
                        //else
                        //{
                        //    newImage = System.Drawing.Image.FromFile(imgFileName);
                        //}

	                    double scaleX = (double)picControl.Width / (double)newImage.Width;
	                    double scaleY = (double)picControl.Height / (double)newImage.Height;
	                    double Scale = Math.Min(scaleX, scaleY);
	                    int w = (int)(newImage.Width * Scale)*4;
	                    int h = (int)(newImage.Height * Scale)*2;
	                    picControl.Width = w;
	                    picControl.Height = h;
                        picControl.Image = CreateThumbnail(newImage, w, h); //newImage.GetThumbnailImage(w, h, new System.Drawing.Image.GetThumbnailImageAbort(GetThumbnailImageAbort), IntPtr.Zero);
	                    newImage.Dispose();
	                    picControl.Refresh();
                        if (imgTot != null)
                        {
                            imgTot.Dispose();
                            imgTot = null;
                            if (File.Exists("tmp1.tif"))
                                File.Delete("tmp1.TIF");
                        }
                	}
                	else
                	{
                        picControl.Height = tabControl1.Height - 75;
                        picControl.Width = tabControl2.Width - 100;
                		//img.LoadBitmapFromFile(imgFileName);
	                	picControl.Image=img.GetBitmap();
	                	picControl.SizeMode= PictureBoxSizeMode.StretchImage;
	                	picControl.Refresh();
                	}
                }
            }
            catch (Exception ex)
            {
                exMailLog.Log(ex);
                MessageBox.Show("Error ..." + ex.Message, "Error");
            }
		}
		void CmdNextClick(object sender, EventArgs e)
		{
            ListViewItem lvwItem;
            if (tabControl2.SelectedIndex == 0)
            {
                if (lstImage.Items.Count > 0)
                {
                    if ((lstImage.Items.Count - 1) != lstImage.SelectedIndex)
                    {
                        lstImage.SelectedIndex = lstImage.SelectedIndex + 1;
                    }
                }
                if (tabControl2.SelectedIndex == 1)
                {
                    if (lstImage.SelectedIndex != 0)
                    {
                        int dashPos = lstImage.SelectedItem.ToString().IndexOf("-") + 1;
                        string currntDoc = lstImage.Items[lstImage.SelectedIndex - 1].ToString().Substring(dashPos);
                        string prevDoc = lstImage.Items[lstImage.SelectedIndex].ToString().Substring(dashPos);
                        if (currntDoc != prevDoc)
                        {
                            lvwItem = lvwDockTypes.FindItemWithText(prevDoc);
                            lvwDockTypes.Items[lvwItem.Index].Selected = true;
                            lvwDockTypes.Focus();
                            //lstImage.Focus();
                        }
                    }
                }
            }
		}
		
		void CmdPreviousClick(object sender, EventArgs e)
		{
            ListViewItem lvwItem;
            if (tabControl2.SelectedIndex == 0)
            {
                if (lstImage.SelectedIndex != 0)
                {
                    lstImage.SelectedIndex = lstImage.SelectedIndex - 1;
                }
                if (tabControl2.SelectedIndex == 1)
                {
                    if (lstImage.SelectedIndex != 0)
                    {
                        int dashPos = lstImage.SelectedItem.ToString().IndexOf("-") + 1;
                        string currntDoc = lstImage.Items[lstImage.SelectedIndex].ToString().Substring(dashPos);
                        string prevDoc = lstImage.Items[lstImage.SelectedIndex + 1].ToString().Substring(dashPos);
                        if (currntDoc != prevDoc)
                        {
                            lvwItem = lvwDockTypes.FindItemWithText(currntDoc);
                            lvwDockTypes.Items[lvwItem.Index].Selected = true;
                            lvwDockTypes.Focus();
                        }
                    }
                }
            }
		}
		
		void CmdAcceptedClick(object sender, EventArgs e)
		{
			string pageName;
            try
            {
                if (crd.role == ihConstants._LIC_ROLE)
                {
                    if (chkReadyUat.Checked == false)
                    {
                        if (lstImage.Items.Count > 0)
                        {
                            pPolicy = new CtrlPolicy(Convert.ToInt32(dsimage11.Tables[0].Rows[0][0].ToString()), Convert.ToInt32(dsimage11.Tables[0].Rows[0][1].ToString()), "1", dsimage11.Tables[0].Rows[0][7].ToString());
                            wfePolicy wPolicy = new wfePolicy(sqlCon, pPolicy);
                            wPolicy.UpdateStatus(eSTATES.POLICY_CHECKED, crd);

                            //for (int i = 0; i < lstImage.Items.Count; i++)
                            //{
                            //    pageName = lstImage.Items[i].ToString().Substring(0, lstImage.Items[i].ToString().IndexOf("-"));
                            //    pImage = new CtrlImage(Convert.ToInt32(cmbProject.SelectedValue.ToString()), Convert.ToInt32(cmbBatch.SelectedValue.ToString()), boxNo.ToString(), Convert.ToInt32(policyNumber), pageName, string.Empty);
                            //    wfeImage wImage = new wfeImage(sqlCon, pImage);
                            //    wImage.UpdateStatus(eSTATES.PAGE_CHECKED, crd);
                            //}
                            CtrlImage exppImage = new CtrlImage(Convert.ToInt32(dsimage11.Tables[0].Rows[0][0].ToString()), Convert.ToInt32(dsimage11.Tables[0].Rows[0][1].ToString()), "1", dsimage11.Tables[0].Rows[0][7].ToString(), string.Empty, string.Empty);
                            wfeImage expwImage = new wfeImage(sqlCon, exppImage);
                            expwImage.UpdateAllImageStatus(eSTATES.PAGE_CHECKED, crd);

                            wPolicy.QaExceptionStatus(ihConstants._POLICY_EXCEPTION_SOLVED, ihConstants._LIC_QA_POLICY_CHECKED);
                            grdBox.Rows[selectedIndex].DefaultCellStyle.BackColor = Color.Green;
                            //if ((wPolicy.GetPolicyStatus() == (int)eSTATES.POLICY_INDEXED) || (wPolicy.GetPolicyStatus() == (int)eSTATES.POLICY_FQC) || (wPolicy.GetPolicyStatus() == (int)eSTATES.POLICY_NOT_INDEXED))
                            //{
                            //    grdPolicy.Rows[policyRowIndex].Cells[9].Value = "Indexed";
                            //}
                            //if ((wPolicy.GetPolicyStatus() == (int)eSTATES.POLICY_ON_HOLD))
                            //{
                            //    grdPolicy.Rows[policyRowIndex].Cells[9].Value = "On hold";
                            //}
                            //if (wPolicy.GetPolicyStatus() == (int)eSTATES.POLICY_MISSING)
                            //{
                            //    grdPolicy.Rows[policyRowIndex].Cells[9].Value = "Missing";
                            //}
                            //if (wPolicy.GetPolicyStatus() == (int)eSTATES.POLICY_EXCEPTION)
                            //{
                            //    grdPolicy.Rows[policyRowIndex].Cells[9].Value = "In exception";
                            //}
                            //if (wPolicy.GetPolicyStatus() == (int)eSTATES.POLICY_CHECKED)
                            //{
                            //    grdPolicy.Rows[policyRowIndex].Cells[9].Value = "Checked";
                            //}
                            tabControl1.SelectedIndex = 0;
                            //tabControl2.SelectedIndex = 0;
                            CheckBatchRejection(dsimage11.Tables[0].Rows[0][1].ToString());
                        }
                    }
                    else
                    {
                        MessageBox.Show("This batch is already marked as ready for UAT.....");
                    }
                }
                else
                {
                    MessageBox.Show("You are not authorized to do this.....");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
		}

        private void ThumbnailChangeSize(string fName)
        {
            System.Drawing.Image imgTot = null;
            try
            {
                //picBig.Height = tabControl1.Height - 75;
                //picBig.Width = tabControl2.Width - 30;
                //if (!System.IO.File.Exists(fName)) return;
                //Image newImage;
                //imgAll.LoadBitmapFromFile(fName);
                //if (imgAll.GetBitmap().PixelFormat == PixelFormat.Format24bppRgb)
                //{
                //    imgAll.GetLZW("tmp1.TIF");
                //    imgTot = Image.FromFile("tmp1.TIF");
                //    newImage = imgTot;
                //}
                //else
                //{
                //    newImage = System.Drawing.Image.FromFile(fName);
                //}
                //double scaleX = (double)picBig.Width / (double)newImage.Width;
                //double scaleY = (double)picBig.Height / (double)newImage.Height;
                //double Scale = Math.Min(scaleX, scaleY);
                //int w = (int)(newImage.Width * Scale);
                //int h = (int)(newImage.Height * Scale);
                //picBig.Width = w;
                //picBig.Height = h;
                //picBig.Image = newImage.GetThumbnailImage(w, h, new System.Drawing.Image.GetThumbnailImageAbort(GetThumbnailImageAbort), IntPtr.Zero);
                //newImage.Dispose();
                //picBig.Refresh();
                //if (imgTot != null)
                //{
                //    imgTot.Dispose();
                //    imgTot = null;
                //    if (File.Exists("tmp1.tif"))
                //        File.Delete("tmp1.TIF");
                //}
            }
            catch (Exception ex)
            {
                exMailLog.Log(ex);
                MessageBox.Show("Error ..." + ex.Message, "Error");
            }
        }


		void CmdRejectedClick(object sender, EventArgs e)
		{
			bool expBol=false;
			policyException udtExp = new policyException();
			NovaNet.Utils.dbCon dbcon=new NovaNet.Utils.dbCon();
			string pageName=null;
            if (crd.role == ihConstants._LIC_ROLE)
            {
                if (chkReadyUat.Checked == false)
                {
                    if (lstImage.Items.Count > 0)
                    {
//                        pPolicy = new CtrlPolicy(Convert.ToInt32(cmbProject.SelectedValue.ToString()), Convert.ToInt32(cmbBatch.SelectedValue.ToString()), boxNo.ToString(), policyNumber.ToString());
                        pPolicy = new CtrlPolicy(Convert.ToInt32(dsimage11.Tables[0].Rows[0][0].ToString()), Convert.ToInt32(dsimage11.Tables[0].Rows[0][1].ToString()), "1", dsimage11.Tables[0].Rows[0][7].ToString());
                        wfePolicy policy = new wfePolicy(sqlCon, pPolicy);
                        if (chkCropClean.Checked == true)
                        {
                            udtExp.crop_clean_exp = 1;
                            expBol = true;
                        }
                        else
                        {
                            udtExp.crop_clean_exp = 0;
                        }

                        if (chkSpelmis.Checked == true)
                        {
                            udtExp.decision_misd_exp = 1;
                            expBol = true;
                        }
                        else
                        {
                            udtExp.decision_misd_exp = 0;
                        }

                        if (chkExtraPage.Checked == true)
                        {
                            udtExp.extra_page_exp = 1;
                            expBol = true;
                        }
                        else
                        {
                            udtExp.extra_page_exp = 0;
                        }

                        if (chkWrongProperty.Checked == true)
                        {
                            udtExp.linked_policy_exp = 1;
                            expBol = true;
                        }
                        else
                        {
                            udtExp.linked_policy_exp = 0;
                        }

                        if (chkMissingImg.Checked == true)
                        {
                            udtExp.missing_img_exp = 1;
                            expBol = true;
                        }
                        else
                        {
                            udtExp.missing_img_exp = 0;
                        }
                        if (chkMove.Checked == true)
                        {
                            udtExp.move_to_respective_policy_exp = 1;
                            expBol = true;
                        }
                        else
                        {
                            udtExp.move_to_respective_policy_exp = 0;
                        }
                        if (chkOther.Checked == true)
                        {
                            udtExp.other_exp = 1;
                            expBol = true;
                        }
                        else
                        {
                            udtExp.other_exp = 0;
                        }

                        if (chkPoorScan.Checked == true)
                        {
                            udtExp.poor_scan_exp = 1;
                            expBol = true;
                        }
                        else
                        {
                            udtExp.poor_scan_exp = 0;
                        }
                        if (chkRearrange.Checked == true)
                        {
                            udtExp.rearrange_exp = 1;
                            expBol = true;
                        }
                        else
                        {
                            udtExp.rearrange_exp = 0;
                        }
                        if (chkIndexing.Checked == true)
                        {
                            udtExp.wrong_indexing_exp = 1;
                            expBol = true;
                        }
                        else
                        {
                            udtExp.wrong_indexing_exp = 0;
                        }
                        udtExp.comments = txtComments.Text;
                        //udtExp.status = ihConstants._LIC_QA_POLICY_EXCEPTION;
                        if (expBol == true)
                        {
                            udtExp.solved = ihConstants._POLICY_EXCEPTION_NOT_SOLVED;
                            if (policy.UpdateQaPolicyException(crd, udtExp) == true)
                            {
                                if (policy.QaExceptionStatus(ihConstants._POLICY_EXCEPTION_NOT_SOLVED, ihConstants._LIC_QA_POLICY_EXCEPTION) == true)
                                {
                                    policy.UpdateStatus(eSTATES.POLICY_EXCEPTION, crd);

                                    CtrlImage exppImage = new CtrlImage(Convert.ToInt32(dsimage11.Tables[0].Rows[0][0].ToString()), Convert.ToInt32(dsimage11.Tables[0].Rows[0][1].ToString()), "1", dsimage11.Tables[0].Rows[0][7].ToString(), string.Empty, string.Empty);
                                    wfeImage expwImage = new wfeImage(sqlCon, exppImage);
                                    expwImage.UpdateAllImageStatus(eSTATES.PAGE_EXCEPTION, crd);
                                    grdBox.Rows[selectedIndex].DefaultCellStyle.BackColor = Color.Red;
                                }
                            }
                            tabControl1.SelectedIndex = 0;
                            CheckBatchRejection(dsimage11.Tables[0].Rows[0][1].ToString());
                        }
                        else
                        {
                            MessageBox.Show("Provide atleast one exception type", "B'Zer", MessageBoxButtons.OK);
                        }

                    }
                }
                else
                {
                    MessageBox.Show("This batch is already marked as ready for UAT.....");
                }
            }
            else
            {
                MessageBox.Show("You are not authorized to do this.....");
            }
			
		}
        private void CheckBatchRejection(string pBatchKey)
        {
            wfeBatch wBatch = new wfeBatch(sqlCon);
            wQ = new ihwQuery(sqlCon);
            if (chkReadyUat.Checked == false)
            {
                if (wQ.GetSysConfigValue(ihConstants.BATCH_REJECTION_KEY) != ihConstants.BATCH_REJECTION_VALUE)
                {
                    if (wBatch.PolicyWithLICException(Convert.ToInt32(dsimage11.Tables[0].Rows[0][0].ToString()), Convert.ToInt32(dsimage11.Tables[0].Rows[0][1].ToString())) == true)
                    {
                        chkRejectBatch.Visible = true;
                    }
                    else
                    {
                        chkRejectBatch.Visible = false;
                    }
                }
            }
            else
            {
                chkRejectBatch.Visible = false;
            }
        }
		void GrdPolicyCellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
		{
            //if((grdPolicy.Rows.Count > 0) && (e.RowIndex != null))
            //{
            //    if(Convert.ToInt32(grdPolicy.Rows[e.RowIndex].Cells[8].Value.ToString()) == (int) eSTATES.POLICY_CHECKED)
            //    {
            //        e.CellStyle.ForeColor = Color.Green; 
            //    }
            //    if((Convert.ToInt32(grdPolicy.Rows[e.RowIndex].Cells[8].Value.ToString()) == (int) eSTATES.POLICY_EXCEPTION) || (Convert.ToInt32(grdPolicy.Rows[e.RowIndex].Cells[8].Value.ToString()) == (int) eSTATES.POLICY_EXCEPTION))
            //    {
            //        e.CellStyle.ForeColor = Color.Red; 
            //    }
               
            //}
		}
		
		void TabControl1SelectedIndexChanged(object sender, EventArgs e)
		{
            //picBig.Visible = false;
            //panelBig.Visible = false;
            //picBig.Image = null;
            //pgOne.Visible = false;
            //pgThree.Visible = false;
            //pgTwo.Visible = false;
            if ((grdBox.Rows.Count > 0) && (dgvname.Rows.Count > 0) || (grdBox.Rows.Count > 0) && (dgvProperty.Rows.Count > 0))
            {
                if (tabControl1.SelectedIndex == 0)
                {
                    if (grdBox.Rows.Count > 0)
                    {
                        if (selectedIndex < (grdBox.Rows.Count - 1))
                        {
                            if (grdBox.Rows[selectedIndex].Cells[1] != null)
                            {
                                if (grdBox.Rows[selectedIndex + 1].Displayed == false)
                                {
                                    grdBox.FirstDisplayedScrollingRowIndex = selectedIndex;
                                }
                                grdBox.Rows[selectedIndex + 1].Selected = true;
                                //grdBox.CurrentCell = grdBox.Rows[selectedIndex + 1].Cells[1];
                                //policyRowIndex = policyRowIndex + 1;
                            }
                        }
                    }
                }
                if (tabControl1.SelectedIndex == 1)
                {
                    GetAuditException();

                    if (lstImage.Items.Count > 0)
                    {
                        pPolicy = new CtrlPolicy(Convert.ToInt32(dsimage11.Tables[0].Rows[0][0].ToString()), Convert.ToInt32(dsimage11.Tables[0].Rows[0][1].ToString()), "1", policy_number);
                        wfePolicy policy = new wfePolicy(sqlCon, pPolicy);
                        if (policy.GetLicExpCount() == 0)
                        {
                            if (policy.InitiateQaPolicyException(crd) == true)
                            {
                                policy.QaExceptionStatus(ihConstants._POLICY_EXCEPTION_INITIALIZED, ihConstants._LIC_QA_POLICY_VIEWED);
                            }
                        }
                    }
                }
                CtrlBox pBox = new CtrlBox(Convert.ToInt32(cmbProject.SelectedValue), Convert.ToInt32(cmbBatch.SelectedValue), "0");
                wfeBox wBox = new wfeBox(sqlCon, pBox);
                lblTotPol.Text =  wBox.GetLICCheckedCount().ToString();
            }
		}
		
		void GroupBox1Enter(object sender, EventArgs e)
		{
			
		}
		
		void AeLicQaFormClosing(object sender, FormClosingEventArgs e)
		{
			//sqlCon.Close();
		}

        
		
		void RdoShowAllClick(object sender, EventArgs e)
		{
			if((selBoxNo.ToString() != string.Empty) && (selBoxNo != 0))
				PolicyDetails(selBoxNo.ToString());
		}
		
		void RdoCheckedClick(object sender, EventArgs e)
		{	if((selBoxNo.ToString() != string.Empty) && (selBoxNo != 0))
				PolicyDetails(selBoxNo.ToString());
		}
		
		void RdoExceptionsClick(object sender, EventArgs e)
		{
			if((selBoxNo.ToString() != string.Empty) && (selBoxNo != 0))
				PolicyDetails(selBoxNo.ToString());
		}
		
		void RdoOnHoldClick(object sender, EventArgs e)
		{
			if((selBoxNo.ToString() != string.Empty) && (selBoxNo != 0))
				PolicyDetails(selBoxNo.ToString());
		}
		
		void RdoOnHoldCheckedChanged(object sender, EventArgs e)
		{
			
		}
		
		void RdoShowAllCheckedChanged(object sender, EventArgs e)
		{
			
		}
        private bool GetThumbnailImageAbort()
        {
            return false;
        }
        private void lvwDockTypes_SelectedIndexChanged(object sender, EventArgs e)
        {
            //pgOne.Visible = false;
            //pgTwo.Visible = false;
            //pgThree.Visible = false;
            currntPg = 0;
            //if (tabControl2.SelectedIndex == 1)
            //{
                for (int i = 0; i < lvwDockTypes.Items.Count; i++)
                {
                    if (lvwDockTypes.Items[i].Selected == true)
                    {
                        selDocType = lvwDockTypes.Items[i].SubItems[0].Text;
                        ShowThumbImage(selDocType);
                        for (int j = 0; j < lstImage.Items.Count; j++)
                        {
                            string srchStr = lstImage.Items[j].ToString();
                            if (srchStr.IndexOf(selDocType) > 0)
                            {
                                lstImage.SelectedIndex = j;
                                break;
                            }
                        }
                        lstImage.Focus();
                    }
                }
            //}
        }
        private void ShowThumbImage(string pDocType)
        {
            DataSet ds = new DataSet();
            string imageFileName;
            System.Drawing.Image imgNew = null;
            IContainerControl icc = tabControl2.GetContainerControl();
            
            //tabControl2.SelectedIndex = 1;
            //picBig.Visible = false;
            //panelBig.Visible = false;
            //picBig.Image = null;
            System.Drawing.Image imgThumbNail = null;

            pImage = new CtrlImage(Convert.ToInt32(projCode), Convert.ToInt32(batchCode), boxNo.ToString(), policyNumber, string.Empty, pDocType);
            wfeImage wImage = new wfeImage(sqlCon, pImage);
            ds = wImage.GetAllIndexedImageName();
            ClearPicBox();
            if (ds.Tables[0].Rows.Count > 0)
            {
                imageName = new string[ds.Tables[0].Rows.Count];
                if (ds.Tables[0].Rows.Count <= 6)
                {
                    //pgOne.Visible = true;
                    //pgTwo.Visible = false;
                    //pgThree.Visible = false;
                }
                if ((ds.Tables[0].Rows.Count > 6) && (ds.Tables[0].Rows.Count <= 12))
                {
                    //pgOne.Visible = true;
                    //pgTwo.Visible = true;
                    //pgThree.Visible = false;
                }
                if ((ds.Tables[0].Rows.Count > 12) && (ds.Tables[0].Rows.Count <= 14))
                {
                    //pgOne.Visible = true;
                    //pgTwo.Visible = true;
                    //pgThree.Visible = true;
                }
                for (int j = 0; j < ds.Tables[0].Rows.Count; j++)
                {
                    imageFileName = picPath + "\\" + ds.Tables[0].Rows[j][0].ToString();
                    imgAll.LoadBitmapFromFile(imageFileName);

                    if (imgAll.GetBitmap().PixelFormat == PixelFormat.Format24bppRgb)
                    {
                        try
                        {
                            imgAll.GetLZW("tmp.TIF");
                            imgNew = System.Drawing.Image.FromFile("tmp.TIF");
                            imgThumbNail = imgNew;
                        }
                        catch (Exception ex)
                        {
                            string err = ex.Message;
                        }
                    }
                    else
                    {
                        imgThumbNail = System.Drawing.Image.FromFile(imageFileName);
                    }
                    imageName[j] = imageFileName;
                    if (!System.IO.File.Exists(imageFileName)) return;
                    //imgThumbNail = Image.FromFile(imageFileName);
                    double scaleX = 0;//(double)pictureBox1.Width / (double)imgThumbNail.Width;
                    double scaleY = 0;//(double)pictureBox1.Height / (double)imgThumbNail.Height;
                    double Scale = Math.Min(scaleX, scaleY);
                    int w = (int)(imgThumbNail.Width * Scale);
                    int h = (int)(imgThumbNail.Height * Scale);
                    w = w - 5;
                    imgThumbNail = imgThumbNail.GetThumbnailImage(w, h, new System.Drawing.Image.GetThumbnailImageAbort(GetThumbnailImageAbort), IntPtr.Zero);

                    if (j == 0)
                    {
                        //pictureBox1.Image = imgThumbNail;
                        //pictureBox1.Tag = imageFileName;
                    }
                    if (j == 1)
                    {
                        //pictureBox2.Image = imgThumbNail;
                        //pictureBox2.Tag = imageFileName;
                    }
                    if (j == 2)
                    {
                        //pictureBox3.Image = imgThumbNail;
                        //pictureBox3.Tag = imageFileName;
                    }
                    if (j == 3)
                    {
                        //pictureBox4.Image = imgThumbNail;
                        //pictureBox4.Tag = imageFileName;
                    }
                    if (j == 4)
                    {
                        //pictureBox5.Image = imgThumbNail;
                        //pictureBox5.Tag = imageFileName;
                    }
                    if (j == 5)
                    {
                        //pictureBox6.Image = imgThumbNail;
                        //pictureBox6.Tag = imageFileName;
                    }
                    if (imgNew != null)
                    {
                        imgNew.Dispose();
                        imgNew = null;
                        if (File.Exists("tmp.tif"))
                            File.Delete("tmp.TIF");
                    }
                }
            }
            else
            {
                ClearPicBox();
                imageName = null;
            }
             
        }
        void ClearPicBox()
        {
            //pictureBox1.Image = null;
            //pictureBox2.Image = null;
            //pictureBox3.Image = null;
            //pictureBox4.Image = null;
            //pictureBox5.Image = null;
            //pictureBox6.Image = null;
        }

        private void rdo150_CheckedChanged(object sender, EventArgs e)
        {
            //if ((grdPolicy.Rows.Count > 0))
            //{
            //    for (int l = 0; l < grdPolicy.Rows.Count; l++)
            //    {
            //        if (Convert.ToDouble(grdPolicy.Rows[l].Cells[7].Value.ToString()) == ihConstants._DOCKET_MAX_SIZE)
            //        {
            //            grdPolicy.Rows[l] DefaultCellStyle.ForeColor = Color.Green;
            //        }
            //    }

            //}
        }

        private void rdo150_Click(object sender, EventArgs e)
        {
            if ((selBoxNo.ToString() != string.Empty) && (selBoxNo != 0))
                PolicyDetails(selBoxNo.ToString());
        }

        private int GetDocTypePos()
        {
            string currntDoc;
            int index = 0;
            string srchStr;
               for (int i = 0; i < lvwDockTypes.Items.Count; i++)
                {
                    if (lvwDockTypes.Items[i].Selected == true)
                    {
                        currntDoc = lvwDockTypes.Items[i].SubItems[0].Text;
                        for (int j = 0; j < lstImage.Items.Count; j++)
                        {
                            srchStr = lstImage.Items[j].ToString();
                            if (srchStr.IndexOf(currntDoc) > 0)
                            {
                                index = j;
                                break;
                            }
                        }
                        break;
                    }
                }
            return index;
        }
        private void pictureBox1_DoubleClick(object sender, EventArgs e)
        {
            
            //Bitmap bmp;
            //picBig.Image = null;
            if (imageName != null)
            {
                if (imageName.Length >= 1)
                {
                    //ThumbnailChangeSize(pictureBox1.Tag.ToString());
                    
                    int lstIndex;
                    lstIndex = (currntPg * 6) + 0 + GetDocTypePos();

                    if (lstIndex < lstImage.Items.Count)
                    {
                        lstImage.SelectedIndex = lstIndex;
                    }
                    tabControl2.SelectedIndex = 0;
                    lvwDockTypes.Focus();
                    //picBig.Visible = true;
                    //panelBig.Visible = true;
                }
            }
        }

        private void pictureBox2_DoubleClick(object sender, EventArgs e)
        {
            
			//Bitmap bmp;
            //picBig.Image = null;
            if (imageName != null)
            {
                if (imageName.Length >= 2)
                {
                    
                    //ThumbnailChangeSize(pictureBox2.Tag.ToString());
                    int lstIndex;
                    lstIndex = (currntPg * 6) + 1 + GetDocTypePos();
                    if (lstIndex < lstImage.Items.Count)
                    {
                        lstImage.SelectedIndex = lstIndex;
                    }
                    tabControl2.SelectedIndex = 0;
                    lvwDockTypes.Focus();
                    //picBig.Visible = true;
                    //panelBig.Visible = true;
                }
            }
        }

        private void pictureBox3_DoubleClick(object sender, EventArgs e)
        {
            //picBig.Image = null;
            if (imageName != null)
            {
                if (imageName.Length >= 3)
                {
                    
                    //ThumbnailChangeSize(pictureBox3.Tag.ToString());
                    int lstIndex;
                    lstIndex = (currntPg * 6) + 2 + GetDocTypePos();
                    if (lstIndex < lstImage.Items.Count)
                    {
                        lstImage.SelectedIndex = lstIndex;
                    }
                    tabControl2.SelectedIndex = 0;
                    lvwDockTypes.Focus();
                    //picBig.Visible = true;
                    //panelBig.Visible = true;
                }
            }
        }

        private void pictureBox4_DoubleClick(object sender, EventArgs e)
        {
            //Bitmap bmp;
            //picBig.Image = null;
            if (imageName != null)
            {
                if (imageName.Length >= 4)
                {
                    
                    //ThumbnailChangeSize(pictureBox4.Tag.ToString());
                    int lstIndex;
                    lstIndex = (currntPg * 6) + 3 + GetDocTypePos();
                    if (lstIndex < lstImage.Items.Count)
                    {
                        lstImage.SelectedIndex = lstIndex;
                    }
                    tabControl2.SelectedIndex = 0;
                    lvwDockTypes.Focus();
                    //picBig.Visible = true;
                    //panelBig.Visible = true;
                }
            }
        }

        private void pictureBox5_DoubleClick(object sender, EventArgs e)
        {
            //Bitmap bmp;
            //picBig.Image = null;
            if (imageName != null)
            {
                if (imageName.Length >= 5)
                {
                    
                    //ThumbnailChangeSize(pictureBox5.Tag.ToString());
                    int lstIndex;
                    lstIndex = (currntPg * 6) + 4 + GetDocTypePos();
                    if (lstIndex < lstImage.Items.Count)
                    {
                        lstImage.SelectedIndex = lstIndex;
                    }
                    tabControl2.SelectedIndex = 0;
                    lvwDockTypes.Focus();
                    //picBig.Visible = true;
                    //panelBig.Visible = true;
                }
            }
        }

        private void pictureBox6_DoubleClick(object sender, EventArgs e)
        {
            //Bitmap bmp;
            //picBig.Image = null;
            if (imageName != null)
            {
                if (imageName.Length >= 6)
                {
                    
                    //ThumbnailChangeSize(pictureBox6.Tag.ToString());
                    int lstIndex;
                    lstIndex = (currntPg * 6) + 5 + GetDocTypePos();
                    if (lstIndex < lstImage.Items.Count)
                    {
                        lstImage.SelectedIndex = lstIndex;
                    }
                    tabControl2.SelectedIndex = 0;
                    lvwDockTypes.Focus();
                    //picBig.Visible = true;
                    //panelBig.Visible = true;
                }
            }
        }

        private void aeLicQa_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                //if (picBig.Visible == true)
                //{
                //    picBig.Visible = false;
                //    panelBig.Visible = false;
                //    picBig.Image = null;
                //}
            }
        }

        private void aeLicQa_Resize(object sender, EventArgs e)
        {
            if (this.WindowState != FormWindowState.Minimized)
            {
                //groupBox2.Height = (this.ClientSize.Height - groupBox1.Height) - 40;
                //groupBox2.Width = (this.ClientSize.Width - 10);
                tabControl2.Width = tabControl1.Width - (pictureBox.Width+50);
                //tabControl2.Height = pictureBox.Height;
                //MessageBox.Show("Height - " + pictureBox1.Height + " Width - " + pictureBox1.Width);
                //panel3.Dock = DockStyle.None;
                //panel3.Width = tabControl2.Width;
                //panel3.Height = tabControl2.Height;
                //picControl.Height = tabControl2.Height - 100;
                //picControl.Width = tabControl2.Width - 80;
                //MessageBox.Show("Height - " + picControl.Height + " Width - " + picControl.Width);
                //MessageBox.Show("Height - " + tabControl2.Height + " Width - " + tabControl2.Width);
            }
        }

        private void tabControl1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                //if (picBig.Visible == true)
                //{
                //    picBig.Visible = false;
                //    panelBig.Visible = false;
                //    picBig.Image = null;
                //}
            }
            if (e.KeyCode == Keys.Subtract)
            {
                ZoomOut();
            }
            if (e.KeyCode == Keys.Add)
            {
                ZoomIn();
            }
        }

		
		void GrdPolicyCellContentClick(object sender, DataGridViewCellEventArgs e)
		{
			
		}
		
		void ChkMissingImgCheckedChanged(object sender, EventArgs e)
		{
            int tifPos;
            string origDoctype = string.Empty;
            if (lstImage.SelectedIndex >= 0)
            {
                tifPos = lstImage.SelectedItem.ToString().IndexOf("-") + 1;
                string imgNumber;
                imgNumber = lstImage.SelectedItem.ToString().Substring((lstImage.SelectedItem.ToString().IndexOf("_") + 1), 5);
                if (tifPos > 0)
                {
                    origDoctype = lstImage.SelectedItem.ToString().Substring(tifPos);
                }
                if (chkMissingImg.Checked)
                {
                    txtComments.Text = txtComments.Text + imgNumber + "-" + origDoctype + " Missing image \r\n";
                    txtComments.SelectionStart = txtComments.Text.Length;
                    txtComments.ScrollToCaret();
                    txtComments.Refresh();
                }
                else
                {
                    string strToReplace;
                    strToReplace =imgNumber + "-" + origDoctype + " Missing image \r\n";
                    txtComments.Text = txtComments.Text.Replace(strToReplace, "");
                }
            }
		}
		
		void ChkExtraPageCheckedChanged(object sender, EventArgs e)
		{
            int tifPos;
            string origDoctype = string.Empty;
            if (lstImage.SelectedIndex >= 0)
            {
                tifPos = lstImage.SelectedItem.ToString().IndexOf("-") + 1;
                string imgNumber;
                imgNumber = lstImage.SelectedItem.ToString().Substring((lstImage.SelectedItem.ToString().IndexOf("_") + 1), 5);
                if (tifPos > 0)
                {
                    origDoctype = lstImage.SelectedItem.ToString().Substring(tifPos);
                }
                if (chkExtraPage.Checked)
                {
                    txtComments.Text = txtComments.Text + imgNumber + "-" + origDoctype + " Extra page \r\n";
                    txtComments.SelectionStart = txtComments.Text.Length;
                    txtComments.ScrollToCaret();
                    txtComments.Refresh();
                }
                else
                {
                    string strToReplace;
                    strToReplace =imgNumber + "-" + origDoctype + " Extra page \r\n";
                    txtComments.Text = txtComments.Text.Replace(strToReplace, "");
                }
            }
		}
		
		void ChkIndexingCheckedChanged(object sender, EventArgs e)
		{
            int tifPos;
            string origDoctype = string.Empty;
            if (lstImage.SelectedIndex >= 0)
            {
                tifPos = lstImage.SelectedItem.ToString().IndexOf("-") + 1;
                string imgNumber;
                imgNumber = lstImage.SelectedItem.ToString().Substring((lstImage.SelectedItem.ToString().IndexOf("_") + 1), 5);
                if (tifPos > 0)
                {
                    origDoctype = lstImage.SelectedItem.ToString().Substring(tifPos);
                }
                if (chkIndexing.Checked)
                {
                    txtComments.Text = txtComments.Text + imgNumber + "-" + origDoctype + " Wrong indexing \r\n";
                    txtComments.SelectionStart = txtComments.Text.Length;
                    txtComments.ScrollToCaret();
                    txtComments.Refresh();
                }
                else
                {
                    string strToReplace;
                    strToReplace = imgNumber + "-" + origDoctype + " Wrong indexing \r\n";
                    txtComments.Text = txtComments.Text.Replace(strToReplace, "");
                }
            }
		}
		
				
		void TxtCommentsKeyDown(object sender, KeyEventArgs e)
		{
			if(e.KeyCode== Keys.Enter)
			{
				
			}
		}
		
		void ChkMoveCheckedChanged(object sender, EventArgs e)
		{
            int tifPos;
            string origDoctype = string.Empty;
            if (lstImage.SelectedIndex >= 0)
            {
                tifPos = lstImage.SelectedItem.ToString().IndexOf("-") + 1;
                string imgNumber;
                imgNumber = lstImage.SelectedItem.ToString().Substring((lstImage.SelectedItem.ToString().IndexOf("_") + 1), 5);
                if (tifPos > 0)
                {
                    origDoctype = lstImage.SelectedItem.ToString().Substring(tifPos);
                }
                if (chkMove.Checked)
                {
                    txtComments.Text = txtComments.Text + imgNumber + "-" + origDoctype + " Move to respective policy \r\n";
                    txtComments.SelectionStart = txtComments.Text.Length;
                    txtComments.ScrollToCaret();
                    txtComments.Refresh();
                }
                else
                {
                    string strToReplace;
                    strToReplace = imgNumber + "-" + origDoctype + " Move to respective policy \r\n";
                    txtComments.Text = txtComments.Text.Replace(strToReplace, "");
                }
            }
		}
		
		
		void ChkRearrangeCheckedChanged(object sender, EventArgs e)
		{
            int tifPos;
            string origDoctype = string.Empty;
            if (lstImage.SelectedIndex >= 0)
            {
                tifPos = lstImage.SelectedItem.ToString().IndexOf("-") + 1;
                string imgNumber;
                imgNumber = lstImage.SelectedItem.ToString().Substring((lstImage.SelectedItem.ToString().IndexOf("_") + 1), 5);
                if (tifPos > 0)
                {
                    origDoctype = lstImage.SelectedItem.ToString().Substring(tifPos);
                }
                if (chkRearrange.Checked)
                {
                    txtComments.Text = txtComments.Text + imgNumber + "-" + origDoctype + " Rearrange error \r\n";
                    txtComments.SelectionStart = txtComments.Text.Length;
                    txtComments.ScrollToCaret();
                    txtComments.Refresh();
                }
                else
                {
                    string strToReplace;
                    strToReplace = imgNumber + "-" + origDoctype + " Rearrange error \r\n";
                    txtComments.Text = txtComments.Text.Replace(strToReplace, "");
                }
            }
		}
		
		void ChkLinkedPolicyCheckedChanged(object sender, EventArgs e)
		{
            int tifPos;
            string origDoctype = string.Empty;

            if (lstImage.SelectedIndex >= 0)
            {
                tifPos = lstImage.SelectedItem.ToString().IndexOf("-") + 1;
                string imgNumber;
                imgNumber = lstImage.SelectedItem.ToString().Substring((lstImage.SelectedItem.ToString().IndexOf("_") + 1), 5);
                if (tifPos > 0)
                {
                    origDoctype = lstImage.SelectedItem.ToString().Substring(tifPos);
                }
                if (chkLinkedPolicy.Checked)
                {
                    txtComments.Text = txtComments.Text + imgNumber + "-" + origDoctype + " Linked policy problem \r\n";
                    txtComments.SelectionStart = txtComments.Text.Length;
                    txtComments.ScrollToCaret();
                    txtComments.Refresh();
                }
                else
                {
                    string strToReplace;
                    strToReplace = imgNumber + "-" + origDoctype + " Linked policy problem \r\n";
                    txtComments.Text = txtComments.Text.Replace(strToReplace, "");
                }
            }
		}
		
		void ChkCropCleanCheckedChanged(object sender, EventArgs e)
		{
            int tifPos;
            string origDoctype = string.Empty;
            string imgNumber;
            if (lstImage.SelectedIndex >= 0)
            {
                tifPos = lstImage.SelectedItem.ToString().IndexOf("-") + 1;
                imgNumber = lstImage.SelectedItem.ToString().Substring((lstImage.SelectedItem.ToString().IndexOf("_")+1), 5);
                if (tifPos > 0)
                {
                    origDoctype = lstImage.SelectedItem.ToString().Substring(tifPos);
                }
                if (chkCropClean.Checked)
                {
                    txtComments.Text = txtComments.Text + imgNumber + "-" + origDoctype + " Crop clean problem \r\n" ;
                    txtComments.SelectionStart = txtComments.Text.Length;
                    txtComments.ScrollToCaret();
                    txtComments.Refresh();
                }
                else
                {
                    string strToReplace;
                    strToReplace = imgNumber + "-" + origDoctype + " Crop clean problem \r\n";
                    txtComments.Text = txtComments.Text.Replace(strToReplace, "");
                }
            }
		}
		
		void ChkPoorScanCheckedChanged(object sender, EventArgs e)
		{
            int tifPos;
            string origDoctype = string.Empty;
            string imgNumber;
            if (lstImage.SelectedIndex >= 0)
            {
                tifPos = lstImage.SelectedItem.ToString().IndexOf("-") + 1;
                imgNumber = lstImage.SelectedItem.ToString().Substring((lstImage.SelectedItem.ToString().IndexOf("_") + 1), 5);
                if (tifPos > 0)
                {
                    origDoctype = lstImage.SelectedItem.ToString().Substring(tifPos);
                }
                if (chkPoorScan.Checked)
                {
                    txtComments.Text = txtComments.Text + imgNumber + "-" + origDoctype + " Poor scan quality \r\n";
                    txtComments.SelectionStart = txtComments.Text.Length;
                    txtComments.ScrollToCaret();
                    txtComments.Refresh();
                }
                else
                {
                    string strToReplace;
                    strToReplace =imgNumber + "-" + origDoctype + " Poor scan quality \r\n";
                    txtComments.Text = txtComments.Text.Replace(strToReplace, "");
                }
            }
		}
		
		void ChkDesicionCheckedChanged(object sender, EventArgs e)
		{
            int tifPos;
            string origDoctype = string.Empty;
            string imgNumber;
            if (lstImage.SelectedIndex >= 0)
            {
                tifPos = lstImage.SelectedItem.ToString().IndexOf("-") + 1;
                imgNumber = lstImage.SelectedItem.ToString().Substring((lstImage.SelectedItem.ToString().IndexOf("_") + 1), 5);
                if (tifPos > 0)
                {
                    origDoctype = lstImage.SelectedItem.ToString().Substring(tifPos);
                }
                if (chkDesicion.Checked)
                {
                    txtComments.Text = txtComments.Text + imgNumber + "-" + origDoctype + " Desicion misd \r\n";
                    txtComments.SelectionStart = txtComments.Text.Length;
                    txtComments.ScrollToCaret();
                    txtComments.Refresh();
                }
                else
                {
                    string strToReplace;
                    strToReplace = imgNumber + "-" + origDoctype + " Desicion misd \r\n";
                    txtComments.Text = txtComments.Text.Replace(strToReplace, "");
                }
            }
		}
		
		void ChkOtherCheckedChanged(object sender, EventArgs e)
		{
            int tifPos;
            string origDoctype = string.Empty;
            string imgNumber;
            if (lstImage.SelectedIndex >= 0)
            {
                tifPos = lstImage.SelectedItem.ToString().IndexOf("-") + 1;
                imgNumber = lstImage.SelectedItem.ToString().Substring((lstImage.SelectedItem.ToString().IndexOf("_") + 1), 5);
                if (tifPos > 0)
                {
                    origDoctype = lstImage.SelectedItem.ToString().Substring(tifPos);
                }
                if (chkOther.Checked)
                {
                    txtComments.Text = txtComments.Text + imgNumber + "-" + origDoctype + " Other \r\n";
                    txtComments.SelectionStart = txtComments.Text.Length;
                    txtComments.ScrollToCaret();
                    txtComments.Refresh();
                }
                else
                {
                    string strToReplace;
                    strToReplace =imgNumber + "-" + origDoctype + " Other \r\n";
                    txtComments.Text = txtComments.Text.Replace(strToReplace, "");
                }
            }
        }
        /// <summary>
        /// addedd in version 1.0.0.1
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chkReadyUat_Click(object sender, EventArgs e)
        {
            DialogResult dlg;
            wfeBatch wBatch = new wfeBatch(sqlCon);
            ///changed in version 1.0.2
            if (crd.role == ihConstants._LIC_ROLE)
            {
                if ((cmbProject.SelectedValue != null) && (cmbBatch.SelectedValue != null))
                {
                    if (GetMissingPoliyLst() == false)
                    {
                        if ((grdBox.Rows.Count > 0) && (grdPolicy.Rows.Count > 0))
                        {
                            if (wBatch.PolicyWithLICException(Convert.ToInt32(cmbProject.SelectedValue.ToString()), Convert.ToInt32(cmbBatch.SelectedValue.ToString())) == false)
                            {
                                if (chkReadyUat.Checked == true)
                                {
                                    dlg = MessageBox.Show(this, "Are you sure, this batch is ready for UAT?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                                    if (dlg == DialogResult.Yes)
                                    {
                                        wBatch.UpdateStatus(eSTATES.BATCH_READY_FOR_UAT, Convert.ToInt32(cmbBatch.SelectedValue.ToString()));
                                        chkReadyUat.Checked = true;
                                        chkReadyUat.Enabled = false;
                                    }
                                    else
                                    {
                                        chkReadyUat.Checked = false;
                                        chkReadyUat.Enabled = true;
                                    }
                                }
                            }
                            else
                            {
                                MessageBox.Show("One or more policies is in exception stage, clear the exceptions before proceeding....");
                                chkReadyUat.Checked = false;
                                chkReadyUat.Enabled = true;
                            }
                        }
                        else
                        {
                            MessageBox.Show("Populate the box and policy details.....");
                        }
                    }
                    else
                    {
                        DialogResult rslt = MessageBox.Show(this, "Mandatory document missing in one or more policies, do you want to check the list.....", "Missing mandatory doc types", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                        if (rslt == DialogResult.Yes)
                        {
                            frmMandPolicyList frmMiss = new frmMandPolicyList(gTable, cmbBatch.Text);
                            frmMiss.ShowDialog(this);
                            gTable.Clear();
                            gTable.Dispose();
                            chkReadyUat.Checked = false;
                        }
                        else
                        {
                            if ((grdBox.Rows.Count > 0) && (grdPolicy.Rows.Count > 0))
                            {
                                if (wBatch.PolicyWithLICException(Convert.ToInt32(cmbProject.SelectedValue.ToString()), Convert.ToInt32(cmbBatch.SelectedValue.ToString())) == false)
                                {
                                    if (chkReadyUat.Checked == true)
                                    {
                                        dlg = MessageBox.Show(this, "Are you sure, this batch is ready for UAT?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                                        if (dlg == DialogResult.Yes)
                                        {
                                            wBatch.UpdateStatus(eSTATES.BATCH_READY_FOR_UAT, Convert.ToInt32(cmbBatch.SelectedValue.ToString()));
                                            chkReadyUat.Checked = true;
                                            chkReadyUat.Enabled = false;
                                        }
                                        else
                                        {
                                            chkReadyUat.Checked = false;
                                            chkReadyUat.Enabled = true;
                                        }
                                    }
                                }
                                else
                                {
                                    MessageBox.Show("One or more policies is in exception stage, clear the exceptions before proceeding....");
                                    chkReadyUat.Checked = false;
                                    chkReadyUat.Enabled = true;
                                }
                            }
                            else
                            {
                                MessageBox.Show("Populate the box and policy details.....");
                            }
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("You are not authorized to do this.....");
                chkReadyUat.Checked = false;
            }
        }
        private bool GetMissingPoliyLst()
        {
            CtrlPolicy ctrlPolicy;
            wfePolicy wPolicy;
            bool missingDoc = false;
            ctrlPolicy = new CtrlPolicy(Convert.ToInt32(cmbProject.SelectedValue.ToString()), Convert.ToInt32(cmbBatch.SelectedValue.ToString()),"0","0");
            wPolicy = new wfePolicy(sqlCon, ctrlPolicy);
            eSTATES[] pState = new eSTATES[4];
            DataSet pDs = new DataSet();
            DataSet iDs = new DataSet();
            DataTable dt = new DataTable();
            DataRow dr;
            
            dt.Columns.Add("Boxnumber");
            dt.Columns.Add("Policy");
            dt.Columns.Add("ProposalForm");
            dt.Columns.Add("ProposalReviewSlip");
            dt.Columns.Add("PolicyBond");
            dt.Columns.Add("SignaturePage");

            pState[0] = eSTATES.POLICY_CHECKED;
            pState[1] = eSTATES.POLICY_FQC;
            pState[2] = eSTATES.POLICY_EXCEPTION;
            pState[3] = eSTATES.POLICY_INDEXED;
            pDs = wPolicy.GetPolicyList(pState);
            if (pDs.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < pDs.Tables[0].Rows.Count; i++)
                {
                    ctrlPolicy = new CtrlPolicy(0, Convert.ToInt32(cmbBatch.SelectedValue.ToString()),"0", pDs.Tables[0].Rows[i][0].ToString());
                    wPolicy = new wfePolicy(sqlCon, ctrlPolicy);
                    iDs = wPolicy.GetMissingDocumentPolicyLst();
                    if (iDs.Tables[0].Rows.Count < 4)
                    {
                        dr = dt.NewRow();
                        dr["Boxnumber"] = pDs.Tables[0].Rows[i]["box_number"].ToString();
                        dr["Policy"] = pDs.Tables[0].Rows[i]["policy_no"].ToString();
                        for (int j = 0; j < iDs.Tables[0].Rows.Count; j++)
                        {
                            if (iDs.Tables[0].Rows[j][0].ToString() == ihConstants.PROPOSALFORM_FILE)
                            {
                                dr["ProposalForm"] = iDs.Tables[0].Rows[j][1].ToString();
                            }
                            
                            if (iDs.Tables[0].Rows[j][0].ToString() == ihConstants.PROPOSALREVIEWSLIP_FILE)
                            {
                                dr["ProposalReviewSlip"] = iDs.Tables[0].Rows[j][1].ToString();
                            }
                            
                            if (iDs.Tables[0].Rows[j][0].ToString() == ihConstants.POLICYBOND_FILE)
                            {
                                dr["PolicyBond"] = iDs.Tables[0].Rows[j][1].ToString();
                            }
                            
                            if (iDs.Tables[0].Rows[j][0].ToString() == ihConstants.SIGNATUREPAGE_FILE)
                            {
                                dr["SignaturePage"] = iDs.Tables[0].Rows[j][1].ToString();
                            }
                        }
                        missingDoc = true;
                        dt.Rows.Add(dr);
                    }
                }
            }
            if (missingDoc == true)
            {
                gTable = dt;
                return true;
            }
            else
                return false;
        }
        private void chkReadyUat_CheckedChanged(object sender, EventArgs e)
        {

        }

        int ZoomIn()
        {
            try
            {
                if (img.IsValid() == true)
                {
                    //picControl.Dock = DockStyle.None;
                    //OperationInProgress = ihConstants._OTHER_OPERATION;
                    keyPressed = keyPressed + 1;
                    zoomHeight = Convert.ToInt32(img.GetBitmap().Height * (1.2));
                    zoomWidth = Convert.ToInt32(img.GetBitmap().Width * (1.2));
                    zoomSize.Height = zoomHeight;
                    zoomSize.Width = zoomWidth;

                    picControl.Width = Convert.ToInt32(Convert.ToDouble(picControl.Width) * 1.2);
                    picControl.Height = Convert.ToInt32(Convert.ToDouble(picControl.Height) * 1.2);
                    //picControl.Refresh();
                    ChangeZoomSize();

                    //delinsrtBol = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error while zooming the image " + ex.Message, "Zoom Error");
                exMailLog.Log(ex);
            }
            return 0;
        }
        private void ChangeZoomSize()
        {
            if (!System.IO.File.Exists(imgFileName)) return;
            System.Drawing.Image newImage = System.Drawing.Image.FromFile(imgFileName);
            double scaleX = (double)picControl.Width / (double)newImage.Width;
            double scaleY = (double)picControl.Height / (double)newImage.Height;
            double Scale = Math.Min(scaleX, scaleY);
            int w = (int)(newImage.Width * Scale);
            int h = (int)(newImage.Height * Scale);
            picControl.Width = w;
            picControl.Height = h;
            picControl.Image = newImage.GetThumbnailImage(w, h, new System.Drawing.Image.GetThumbnailImageAbort(GetThumbnailImageAbort), IntPtr.Zero);
            //picControl.Invalidate();
            newImage.Dispose();
        }
        int ZoomOut()
        {
            try
            {
                if (keyPressed > 0)
                {
                    picControl.Dock = DockStyle.None;
                    //OperationInProgress = ihConstants._OTHER_OPERATION;
                    keyPressed = keyPressed + 1;
                    zoomHeight = Convert.ToInt32(img.GetBitmap().Height / (1.2));
                    zoomWidth = Convert.ToInt32(img.GetBitmap().Width / (1.2));
                    zoomSize.Height = zoomHeight;
                    zoomSize.Width = zoomWidth;

                    picControl.Width = Convert.ToInt32(Convert.ToDouble(picControl.Width) / 1.2);
                    picControl.Height = Convert.ToInt32(Convert.ToDouble(picControl.Height) / 1.2);
                    picControl.Refresh();
                    ChangeZoomSize();
                    //delinsrtBol = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error while zooming the image " + ex.Message, "Zoom Error");
            }
            return 0;
        }

        private void cmdZoomIn_Click(object sender, EventArgs e)
        {
            ZoomIn();
        }

        private void cmdZoomOut_Click(object sender, EventArgs e)
        {
            ZoomOut();
        }

        private void rdoMissing_Click(object sender, EventArgs e)
        {
            if ((selBoxNo.ToString() != string.Empty) && (selBoxNo != 0))
                PolicyDetails(selBoxNo.ToString());
        }

        private void chkRejectBatch_Click(object sender, EventArgs e)
        {
            DialogResult rslt;
            wfeBatch wBatch = new wfeBatch(sqlCon);
            DataSet blankDs = new DataSet();
            if (crd.role == ihConstants._LIC_ROLE)
            {
                if (chkReadyUat.Checked == false)
                {
                    if (chkRejectBatch.Checked == true)
                    {
                        if ((cmbBatch.SelectedValue != null) && (cmbProject.SelectedValue != null))
                        {
                            rslt = MessageBox.Show(this, "Are you sure you want to reject this batch?", "Batch rejection", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                            if (rslt == DialogResult.Yes)
                            {
                                if (wBatch.RejectBatch(Convert.ToInt32(cmbBatch.SelectedValue), eSTATES.BATCH_CREATED,eSTATES.BOX_INDEXED, eSTATES.POLICY_INDEXED, eSTATES.PAGE_INDEXED))
                                {
                                    MessageBox.Show("Batch rejected successfully.....");
                                    PopulateProjectCombo();
                                    PopulateBatchCombo();
                                    lblTotPolicies.Text = string.Empty;
                                    lblPolRcvd.Text = string.Empty;
                                    lblPolHold.Text = string.Empty;
                                    lblScannedPol.Text = string.Empty;
                                    lblBatchSz.Text = string.Empty;
                                    lblAvgDocketSz.Text = string.Empty;
                                    lblTotImages.Text = string.Empty;
                                    lblSigCount.Text = string.Empty;
                                    lblNetImageCount.Text = string.Empty;
                                    grdBox.DataSource = blankDs;
                                    grdPolicy.DataSource = blankDs;
                                    ClearPicBox();
                                    chkRejectBatch.Visible = false;
                                    chkReadyUat.Checked = false;
                                }
                                else
                                {
                                    MessageBox.Show("Error while updating the result, aborting.....");
                                    chkRejectBatch.Checked = false;
                                }
                            }
                            else
                            {
                                chkRejectBatch.Checked = false;
                            }
                        }
                    }
                }
                else
                {
                    MessageBox.Show("This batch already gone for UAT, can not be rejected......");
                    chkRejectBatch.Checked = false;
                }
            }
            else
            {
                MessageBox.Show("You are not authorized to do this.....");
            }
        }

        private void pgTwo_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string imageFileName;
            System.Drawing.Image imgNew = null;
            tabControl2.SelectedIndex = 1;
            
            System.Drawing.Image imgThumbNail = null;
            ClearPicBox();
            for (int i = 6; i < imageName.Length; i++)
            {
                imageFileName = imageName[i];
                if (!System.IO.File.Exists(imageFileName)) return;
                    imgAll.LoadBitmapFromFile(imageFileName);
                    currntPg = 1;
                    if (imgAll.GetBitmap().PixelFormat == PixelFormat.Format24bppRgb)
                    {
                        try
                        {
                            imgAll.GetLZW("tmp.TIF");
                            imgNew = System.Drawing.Image.FromFile("tmp.TIF");
                            imgThumbNail = imgNew;
                        }
                        catch (Exception ex)
                        {
                            string err = ex.Message;
                        }
                    }
                    else
                    {
                        imgThumbNail = System.Drawing.Image.FromFile(imageFileName);
                    }
                    double scaleX = 0; //(double)pictureBox1.Width / (double)imgThumbNail.Width;
                    double scaleY = 0;// (double)pictureBox1.Height / (double)imgThumbNail.Height;
                    double Scale = Math.Min(scaleX, scaleY);
                    int w = (int)(imgThumbNail.Width * Scale);
                    int h = (int)(imgThumbNail.Height * Scale);
                    w = w - 5;
                    imgThumbNail = imgThumbNail.GetThumbnailImage(w, h, new System.Drawing.Image.GetThumbnailImageAbort(GetThumbnailImageAbort), IntPtr.Zero);

                    if (i == 6)
                    {
                        //pictureBox1.Image = imgThumbNail;
                        //pictureBox1.Tag = imageFileName;
                    }
                    if (i == 7)
                    {
                        //pictureBox2.Image = imgThumbNail;
                        //pictureBox2.Tag = imageFileName;
                    }
                    if (i == 8)
                    {
                        //pictureBox3.Image = imgThumbNail;
                        //pictureBox3.Tag = imageFileName;
                    }
                    if (i == 9)
                    {
                        //pictureBox4.Image = imgThumbNail;
                        //pictureBox4.Tag = imageFileName;
                    }
                    if (i == 10)
                    {
                        //pictureBox5.Image = imgThumbNail;
                        //pictureBox5.Tag = imageFileName;
                    }
                    if (i == 11)
                    {
                        //pictureBox6.Image = imgThumbNail;
                        //pictureBox6.Tag = imageFileName;
                    }
                    if (imgNew != null)
                    {
                        imgNew.Dispose();
                        imgNew = null;
                        if (File.Exists("tmp.tif"))
                            File.Delete("tmp.TIF");
                    }
            }
        }

        private void pgOne_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string imageFileName;
            System.Drawing.Image imgNew = null;
            tabControl2.SelectedIndex = 1;
            
            System.Drawing.Image imgThumbNail = null;
            ClearPicBox();
            for (int i = 0; i < imageName.Length; i++)
            {
                imageFileName = imageName[i];
                if (!System.IO.File.Exists(imageFileName)) return;
                imgAll.LoadBitmapFromFile(imageFileName);

                if (imgAll.GetBitmap().PixelFormat == PixelFormat.Format24bppRgb)
                {
                    try
                    {
                        imgAll.GetLZW("tmp.TIF");
                        imgNew = System.Drawing.Image.FromFile("tmp.TIF");
                        imgThumbNail = imgNew;
                    }
                    catch (Exception ex)
                    {
                        string err = ex.Message;
                    }
                }
                else
                {
                    imgThumbNail = System.Drawing.Image.FromFile(imageFileName);
                }
                double scaleX = 0;// (double)pictureBox1.Width / (double)imgThumbNail.Width;
                double scaleY = 0;// (double)pictureBox1.Height / (double)imgThumbNail.Height;
                double Scale = Math.Min(scaleX, scaleY);
                int w = (int)(imgThumbNail.Width * Scale);
                int h = (int)(imgThumbNail.Height * Scale);
                w = w - 5;
                imgThumbNail = imgThumbNail.GetThumbnailImage(w, h, new System.Drawing.Image.GetThumbnailImageAbort(GetThumbnailImageAbort), IntPtr.Zero);
                currntPg = 0;
                if (i == 0)
                {
                    //pictureBox1.Image = imgThumbNail;
                    //pictureBox1.Tag = imageFileName;
                }
                if (i == 1)
                {
                    //pictureBox2.Image = imgThumbNail;
                    //pictureBox2.Tag = imageFileName;
                }
                if (i == 2)
                {
                    //pictureBox3.Image = imgThumbNail;
                    //pictureBox3.Tag = imageFileName;
                }
                if (i == 3)
                {
                    //pictureBox4.Image = imgThumbNail;
                    //pictureBox4.Tag = imageFileName;
                }
                if (i == 4)
                {
                    //pictureBox5.Image = imgThumbNail;
                    //pictureBox5.Tag = imageFileName;
                }
                if (i == 5)
                {
                    //pictureBox6.Image = imgThumbNail;
                    //pictureBox6.Tag = imageFileName;
                }
                if (imgNew != null)
                {
                    imgNew.Dispose();
                    imgNew = null;
                    if (File.Exists("tmp.tif"))
                        File.Delete("tmp.TIF");
                }
            }
        }

        private void pgThree_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string imageFileName;
            System.Drawing.Image imgNew = null;
            tabControl2.SelectedIndex = 1;
            
            System.Drawing.Image imgThumbNail = null;
            ClearPicBox();
            for (int i = 0; i < imageName.Length; i++)
            {
                imageFileName = imageName[i];
                if (!System.IO.File.Exists(imageFileName)) return;
                imgAll.LoadBitmapFromFile(imageFileName);
                currntPg = 2;
                if (imgAll.GetBitmap().PixelFormat == PixelFormat.Format24bppRgb)
                {
                    try
                    {
                        imgAll.GetLZW("tmp.TIF");
                        imgNew = System.Drawing.Image.FromFile("tmp.TIF");
                        imgThumbNail = imgNew;
                    }
                    catch (Exception ex)
                    {
                        string err = ex.Message;
                    }
                }
                else
                {
                    imgThumbNail = System.Drawing.Image.FromFile(imageFileName);
                }
                double scaleX = 0;// (double)pictureBox1.Width / (double)imgThumbNail.Width;
                double scaleY = 0;// (double)pictureBox1.Height / (double)imgThumbNail.Height;
                double Scale = Math.Min(scaleX, scaleY);
                int w = (int)(imgThumbNail.Width * Scale);
                int h = (int)(imgThumbNail.Height * Scale);
                w = w - 5;
                imgThumbNail = imgThumbNail.GetThumbnailImage(w, h, new System.Drawing.Image.GetThumbnailImageAbort(GetThumbnailImageAbort), IntPtr.Zero);

                if (i == 12)
                {
                    //pictureBox1.Image = imgThumbNail;
                    //pictureBox1.Tag = imageFileName;
                }
                if (i == 13)
                {
                    //pictureBox2.Image = imgThumbNail;
                    //pictureBox2.Tag = imageFileName;
                }
                if (i == 14)
                {
                    //pictureBox3.Image = imgThumbNail;
                    //pictureBox3.Tag = imageFileName;
                }
                if (imgNew != null)
                {
                    imgNew.Dispose();
                    imgNew = null;
                    if (File.Exists("tmp.tif"))
                        File.Delete("tmp.TIF");
                }
            }
        }

        private void tabControl2_TabIndexChanged(object sender, EventArgs e)
        {
            if (imgFileName != string.Empty)
            {
                if(tabControl2.SelectedIndex == 0)
                    ChangeSize();
                ThumbnailChangeSize(imgFileName);
            }
        }

        private void tabControl2_SelectedIndexChanged(object sender, EventArgs e)
        {
            //ListViewItem lvwItem;
            //string currntDoc = string.Empty;
            //if (tabControl2.SelectedIndex == 1)
            //{
            //    firstDoc = false;
            //    for (int i = 0; i < 1; i++)
            //    {
            //        //if (lvwDockTypes.Items[i].Selected == true)
            //        //{
            //        //    currntDoc = lvwDockTypes.Items[i].SubItems[0].Text;
            //        //    break;
            //        //}
            //        string aa = policyPath;
            //        string imgFileName = policyPath + @"\" + lstImage.Items[i].ToString();
            //        img.LoadBitmapFromFile(imgFileName);
            //        //picControl.Image = img.GetBitmap();
                    
            //        pictureBox1.Image = img.GetBitmap();
            //        ChangeSize();                    
            //        ThumbnailChangeSize(imgFileName);
            //        //picControl.SizeMode = PictureBoxSizeMode.StretchImage;
            //        //ChangeSize();
            //        //picControl.Refresh();
            //    }
            //    //if (currntDoc != string.Empty)
            //    //{
            //    //    lvwItem = lvwDockTypes.FindItemWithText(currntDoc);
            //    //    lvwDockTypes.Items[lvwItem.Index].Selected = true;
            //    //}
            //}
            //else
            //{
            //    ChangeSize();
            //}
            //lvwDockTypes.Focus();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            wfeImage img = new wfeImage(sqlCon);
//            DataTable dt = img.GetAllDeedEXQA(cmbDistrict.SelectedValue.ToString(), cmbWhereReg.SelectedValue.ToString(), cmbBook.SelectedValue.ToString(), cmbyear.Text, cmbVol.Text);
               dt = img.GetAllDeedEXQA(cmbRunnum.SelectedValue.ToString());
               wfeBox wfeb = new wfeBox(sqlCon);
               if (wfeb.GetRunnumStatus(cmbRunnum.SelectedValue.ToString()).Tables[0].Rows[0][0].ToString() == "7")
                {
                    checkBox1.Checked = true;
                    checkBox1.Enabled = false;
                    cmdAccepted.Enabled = false;
                    cmdRejected.Enabled = false;
                }
               if (wfeb.GetRunnumStatus(cmbRunnum.SelectedValue.ToString()).Tables[0].Rows[0][0].ToString() == "66")
               {
                   checkBox1.Checked = false;
                   checkBox1.Enabled = true;
                   cmdAccepted.Enabled = true;
                   cmdRejected.Enabled = true;
               }
            if (dt.Rows.Count > 0)
            {
                //cmdRefine.Enabled = true;
                grdBox.DataSource = null;
                grdBox.DataSource = dt;
                total_Count = grdBox.Rows.Count;
                dgvname.DataSource = null;
                dgvProperty.DataSource = null;
                grdBox.Columns[0].Visible = false;
                grdBox.Columns[1].Visible = false;
                grdBox.Columns[5].Visible = false;
                grdBox.Columns[6].Visible = false;
                grdBox.Columns[7].Visible = false;
                grdBox.Columns[8].Visible = false;
                //grdBox.Columns[5].Width = 150;
                //grdBox.Columns[6].Width = 250;
                for (int i = 0; i < grdBox.Rows.Count; i++)
                {
                    string deedno = grdBox.Rows[i].Cells[0].Value.ToString() + grdBox.Rows[i].Cells[1].Value.ToString() + grdBox.Rows[i].Cells[2].Value.ToString() + grdBox.Rows[i].Cells[3].Value.ToString() + "[" + grdBox.Rows[i].Cells[4].Value.ToString() + "]";
                    wfePolicy wfepol = new wfePolicy(sqlCon);

                    if (wfepol.deedCheck(deedno) == "30")
                    {
                        grdBox.Rows[i].DefaultCellStyle.BackColor = Color.Red;
                    }
                    if (wfepol.deedCheck(deedno) == "31")
                    {
                        grdBox.Rows[i].DefaultCellStyle.BackColor = Color.Green;
                    }

                }
                
            }
            else { MessageBox.Show("No Record Found ....."); }
        }

        private void grdBox_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void grdPolicy_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dgvname_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            ShowImages(grdBox.CurrentRow.Cells[0].Value.ToString(), grdBox.CurrentRow.Cells[1].Value.ToString(), grdBox.CurrentRow.Cells[2].Value.ToString(), grdBox.CurrentRow.Cells[3].Value.ToString(), grdBox.CurrentRow.Cells[4].Value.ToString());
        }

        private void ShowImages()
        {
            try
            {
                tabControl1.TabPages[1].Enabled = true;
                lstImage.Items.Clear();
                picControl.Refresh();
                string deed_no = grdBox.Rows[selectedIndex].Cells[4].Value.ToString();
                //grdBox.Rows[e.RowIndex].Cells[0].Value.ToString(),grdBox.Rows[e.RowIndex].Cells[1].Value.ToString(),grdBox.Rows[e.RowIndex].Cells[2].Value.ToString(),grdBox.Rows[e.RowIndex].Cells[3].Value.ToString(),grdBox.Rows[e.RowIndex].Cells[4].Value.ToString()
                string policyname = cmbDistrict.SelectedValue.ToString() + cmbWhereReg.SelectedValue.ToString() + cmbBook.SelectedValue.ToString() + cmbyear.Text + "[" + deed_no + "]";
                wfeImage wfeimg = new wfeImage(sqlCon);
                dsimage11 = wfeimg.GetAllExportedImagewithProj(policyname);

                wfePolicy wPol = new wfePolicy(sqlCon);
                int polStatus = wPol.GetPolicyStatus(policyname);
                if ((polStatus == (int)eSTATES.POLICY_INITIALIZED) || (polStatus == (int)eSTATES.POLICY_CREATED) || (polStatus == (int)eSTATES.POLICY_SCANNED))
                {
                    MessageBox.Show("Image information not available/ not processed, try again...");
                    tabControl1.TabPages[1].Enabled = false;
                    picControl.Image = null;
                    return;
                }
                wfePolicy wfepol = new wfePolicy(sqlCon);
                policyPath = wfepol.GetPolicyPath(policyname);
                policyPath = policyPath + "\\QC";
                
                string[] files = Directory.GetFiles(policyPath);
                //for (int i = 0; i < files.Length; ++i)
                //{
                //    string filename = Path.GetFileName(files[i]);
                //    lstImage.Items.Add(filename);
                //    filename = null;
                //}
                if (dsimage11.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsimage11.Tables[0].Rows.Count; i++)
                    {
                        lstImage.Items.Add(dsimage11.Tables[0].Rows[i][3].ToString());
                    }
                    tabControl1.SelectedTab = tbImages;
                    lstImage.SelectedIndex = 0;
                    txtVol.Text = grdBox.Rows[grdBox.CurrentRow.Index].Cells[11].Value.ToString();
                    txtVol.Enabled = false;
                    txtPgFrom.Text = grdBox.Rows[grdBox.CurrentRow.Index].Cells[12].Value.ToString();
                    txtPgFrom.Enabled = false;
                    txtPgTo.Text = grdBox.Rows[grdBox.CurrentRow.Index].Cells[13].Value.ToString();
                    txtPgTo.Enabled = false;
                }
                else
                {
                    MessageBox.Show("Document is not Scanned Yet, Please Scan the Document First...");
                    tabControl1.TabPages[1].Enabled = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
                tabControl1.TabPages[1].Enabled = false;
                picControl.Image = null;
            }
        }


        private void ShowImages(string dist,string ro,string book,string deed_year,string deedno)
        {
            try
            {
                tabControl1.TabPages[1].Enabled = true;
                lstImage.Items.Clear();
                picControl.Refresh();
                string deed_no = deedno;
                string policyname = dist + ro + book + deed_year + "[" + deed_no + "]";
                wfeImage wfeimg = new wfeImage(sqlCon);
                dsimage11 = wfeimg.GetAllExportedImagewithProj(policyname);

                wfePolicy wPol = new wfePolicy(sqlCon);
                int polStatus = wPol.GetPolicyStatus(policyname);
                if ((polStatus == (int)eSTATES.POLICY_INITIALIZED) || (polStatus == (int)eSTATES.POLICY_CREATED) || (polStatus == (int)eSTATES.POLICY_SCANNED))
                {
                    MessageBox.Show("Image information not available/ not processed, try again...");
                    tabControl1.TabPages[1].Enabled = false;
                    picControl.Image = null;
                    return;
                }
                wfePolicy wfepol = new wfePolicy(sqlCon);
                policyPath = wfepol.GetPolicyPath(policyname);
                policyPath = policyPath + "\\QC";

                string[] files = Directory.GetFiles(policyPath);
                if (dsimage11.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsimage11.Tables[0].Rows.Count; i++)
                    {
                        lstImage.Items.Add(dsimage11.Tables[0].Rows[i][3].ToString());
                    }
                    tabControl1.SelectedTab = tbImages;
                    lstImage.SelectedIndex = 0;
                    txtVol.Text = grdBox.Rows[grdBox.CurrentRow.Index].Cells[11].Value.ToString();
                    txtVol.Enabled = false;
                    txtPgFrom.Text = grdBox.Rows[grdBox.CurrentRow.Index].Cells[12].Value.ToString();
                    txtPgFrom.Enabled = false;
                    txtPgTo.Text = grdBox.Rows[grdBox.CurrentRow.Index].Cells[13].Value.ToString();
                    txtPgTo.Enabled = false;
                }
                else
                {
                    MessageBox.Show("Document is not Scanned Yet, Please Scan the Document First...");
                    tabControl1.TabPages[1].Enabled = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
                tabControl1.TabPages[1].Enabled = false;
                picControl.Image = null;
            }
        }

        private void ShowImages(string dist, string ro, string book, string deed_year, string deedno,string aa)
        {
            try
            {
                tabControl1.TabPages[1].Enabled = true;
                lstImage.Items.Clear();
                picControl.Refresh();
                string deed_no = deedno;
                string policyname = dist + ro + book + deed_year + "[" + deed_no + "]";
                wfeImage wfeimg = new wfeImage(sqlCon);
                dsimage11 = wfeimg.GetAllExportedImagewithProj(policyname);

                wfePolicy wPol = new wfePolicy(sqlCon);
                int polStatus = wPol.GetPolicyStatus(policyname);
                if ((polStatus == (int)eSTATES.POLICY_INITIALIZED) || (polStatus == (int)eSTATES.POLICY_CREATED) || (polStatus == (int)eSTATES.POLICY_SCANNED))
                {
                    MessageBox.Show("Image information not available/ not processed, try again...");
                    tabControl1.TabPages[1].Enabled = false;
                    picControl.Image = null;
                    return;
                }
                wfePolicy wfepol = new wfePolicy(sqlCon);
                policyPath = wfepol.GetPolicyPath(policyname);
                policyPath = policyPath + "\\QC";

                string[] files = Directory.GetFiles(policyPath);
                if (dsimage11.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsimage11.Tables[0].Rows.Count; i++)
                    {
                        lstImage.Items.Add(dsimage11.Tables[0].Rows[i][3].ToString());
                    }
                    //tabControl1.SelectedTab = tbSelection;
                    //lstImage.SelectedIndex = 0;
                    txtVol.Text = grdBox.Rows[grdBox.CurrentRow.Index].Cells[11].Value.ToString();
                    txtVol.Enabled = false;
                    txtPgFrom.Text = grdBox.Rows[grdBox.CurrentRow.Index].Cells[12].Value.ToString();
                    txtPgFrom.Enabled = false;
                    txtPgTo.Text = grdBox.Rows[grdBox.CurrentRow.Index].Cells[13].Value.ToString();
                    txtPgTo.Enabled = false;
                }
                else
                {
                    MessageBox.Show("Document is not Scanned Yet, Please Scan the Document First...");
                    tabControl1.TabPages[1].Enabled = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
                tabControl1.TabPages[1].Enabled = false;
                picControl.Image = null;
            }
        }


        private void splitContainer3_SplitterMoved(object sender, SplitterEventArgs e)
        {

        }

        private void dgvProperty_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            //ShowImages();
            ShowImages(grdBox.CurrentRow.Cells[0].Value.ToString(), grdBox.CurrentRow.Cells[1].Value.ToString(), grdBox.CurrentRow.Cells[2].Value.ToString(), grdBox.CurrentRow.Cells[3].Value.ToString(), grdBox.CurrentRow.Cells[4].Value.ToString());
        }

        private void grdBox_CellMouseEnter(object sender, DataGridViewCellEventArgs e)
        {
            
            //if ((e.ColumnIndex == 0) && (e.RowIndex >=0))
            //{
            //    string doCode = grdBox.Rows[e.RowIndex].Cells[0].Value.ToString();
            //    wfeProject pro = new wfeProject(sqlCon);
            //    string dis_code = pro.GetDistrictByCode(doCode).Trim();
            //    grdBox.Cursor = Cursors.Hand;
            //    grdBox.Rows[e.RowIndex].Cells[0].ToolTipText= "    "+dis_code;
            //}
            //if ((e.ColumnIndex == 1) && (e.RowIndex >= 0))
            //{
            //    string doCode = grdBox.Rows[e.RowIndex].Cells[1].Value.ToString();
            //    wfeProject pro = new wfeProject(sqlCon);
            //    string dis_code = pro.GetROByCode(doCode).Trim();
            //    grdBox.Cursor = Cursors.Hand;
            //    grdBox.Rows[e.RowIndex].Cells[1].ToolTipText = "    " + dis_code;
            //}
            //if ((e.ColumnIndex == 7) && (e.RowIndex >= 0))
            //{
            //    string doCode = grdBox.Rows[e.RowIndex].Cells[7].Value.ToString();
            //    wfeProject pro = new wfeProject(sqlCon);
            //    string dis_code = pro.GetTranMajorByCode(doCode).Trim();
            //    grdBox.Cursor = Cursors.Hand;
            //    grdBox.Rows[e.RowIndex].Cells[7].ToolTipText = "    " + dis_code;
            //}
            //if ((e.ColumnIndex == 8) && (e.RowIndex >= 0))
            //{
            //    string doCode = grdBox.Rows[e.RowIndex].Cells[8].Value.ToString();
            //    string doCode1 = grdBox.Rows[e.RowIndex].Cells[7].Value.ToString();
            //    wfeProject pro = new wfeProject(sqlCon);
            //    string dis_code = pro.GetTranMinorByCode(doCode, doCode1).Trim();
            //    grdBox.Cursor = Cursors.Hand;
            //    grdBox.Rows[e.RowIndex].Cells[8].ToolTipText = "    " + dis_code;
            //}
        }

        private void grdBox_CellMouseLeave(object sender, DataGridViewCellEventArgs e)
        {
            //grdBox.Cursor = Cursors.Default;
        }

        private void dgvname_CellMouseEnter(object sender, DataGridViewCellEventArgs e)
        {
            //if ((e.ColumnIndex == 0) && (e.RowIndex >= 0))
            //{
            //    string doCode = dgvname.Rows[e.RowIndex].Cells[0].Value.ToString();
            //    wfeProject pro = new wfeProject(sqlCon);
            //    string dis_code = pro.GetDistrictByCode(doCode).Trim();
            //    dgvname.Cursor = Cursors.Hand;
            //    dgvname.Rows[e.RowIndex].Cells[0].ToolTipText = "    " + dis_code;
            //}
            //if ((e.ColumnIndex == 1) && (e.RowIndex >= 0))
            //{
            //    string doCode = dgvname.Rows[e.RowIndex].Cells[1].Value.ToString();
            //    wfeProject pro = new wfeProject(sqlCon);
            //    string dis_code = pro.GetROByCode(doCode).Trim();
            //    dgvname.Cursor = Cursors.Hand;
            //    dgvname.Rows[e.RowIndex].Cells[1].ToolTipText = "    " + dis_code;
            //}
            //if ((e.ColumnIndex == 19) && (e.RowIndex >= 0))
            //{
            //    string doCode = dgvname.Rows[e.RowIndex].Cells[19].Value.ToString();
            //    wfeProject pro = new wfeProject(sqlCon);
            //    string dis_code = pro.GetProfByCode(doCode).Trim();
            //    dgvname.Cursor = Cursors.Hand;
            //    dgvname.Rows[e.RowIndex].Cells[19].ToolTipText = "    " + dis_code;
            //}
            //if ((e.ColumnIndex == 20) && (e.RowIndex >= 0))
            //{
            //    string doCode = dgvname.Rows[e.RowIndex].Cells[20].Value.ToString();
            //    wfeProject pro = new wfeProject(sqlCon);
            //    string dis_code = pro.GetReligionByCode(doCode).Trim();
            //    dgvname.Cursor = Cursors.Hand;
            //    dgvname.Rows[e.RowIndex].Cells[20].ToolTipText = "    " + dis_code;
            //}
            //if ((e.ColumnIndex == 9) && (e.RowIndex >= 0))
            //{
            //    string doCode = dgvname.Rows[e.RowIndex].Cells[9].Value.ToString();
            //    wfeProject pro = new wfeProject(sqlCon);
            //    string dis_code = pro.GetPartyCode(doCode).Trim();
            //    dgvname.Cursor = Cursors.Hand;
            //    dgvname.Rows[e.RowIndex].Cells[9].ToolTipText = "    " + dis_code;
            //}
        }

        private void dgvname_CellMouseLeave(object sender, DataGridViewCellEventArgs e)
        {
            dgvname.Cursor = Cursors.Default;
        }

        private void dgvProperty_CellMouseEnter(object sender, DataGridViewCellEventArgs e)
        {
            //if ((e.ColumnIndex == 0) && (e.RowIndex >= 0))
            //{
            //    string doCode = dgvProperty.Rows[e.RowIndex].Cells[0].Value.ToString();
            //    wfeProject pro = new wfeProject(sqlCon);
            //    string dis_code = pro.GetDistrictByCode(doCode).Trim();
            //    dgvProperty.Cursor = Cursors.Hand;
            //    dgvProperty.Rows[e.RowIndex].Cells[0].ToolTipText = "    " + dis_code;
            //}
            //if ((e.ColumnIndex == 1) && (e.RowIndex >= 0))
            //{
            //    string doCode = dgvProperty.Rows[e.RowIndex].Cells[1].Value.ToString();
            //    wfeProject pro = new wfeProject(sqlCon);
            //    string dis_code = pro.GetROByCode(doCode).Trim();
            //    dgvProperty.Cursor = Cursors.Hand;
            //    dgvProperty.Rows[e.RowIndex].Cells[1].ToolTipText = "    " + dis_code;
            //}
            //if ((e.ColumnIndex == 6) && (e.RowIndex >= 0))
            //{
            //    string doCode = dgvProperty.Rows[e.RowIndex].Cells[6].Value.ToString();
            //    wfeProject pro = new wfeProject(sqlCon);
            //    string dis_code = pro.GetDistrictByCode(doCode).Trim();
            //    dgvProperty.Cursor = Cursors.Hand;
            //    dgvProperty.Rows[e.RowIndex].Cells[6].ToolTipText = "    " + dis_code;
            //}
            //if ((e.ColumnIndex == 7) && (e.RowIndex >= 0))
            //{
            //    string doCode = dgvProperty.Rows[e.RowIndex].Cells[7].Value.ToString();
            //    wfeProject pro = new wfeProject(sqlCon);
            //    string dis_code = pro.GetROByCode(doCode).Trim();
            //    dgvProperty.Cursor = Cursors.Hand;
            //    dgvProperty.Rows[e.RowIndex].Cells[7].ToolTipText = "    " + dis_code;
            //}
            //if ((e.ColumnIndex == 8) && (e.RowIndex >= 0))
            //{
            //    string doCode = dgvProperty.Rows[e.RowIndex].Cells[8].Value.ToString();
            //    string doCode1 = dgvProperty.Rows[e.RowIndex].Cells[0].Value.ToString();
            //    wfeProject pro = new wfeProject(sqlCon);
            //    string dis_code = pro.GetPsCode(doCode,doCode1).Trim();
            //    dgvProperty.Cursor = Cursors.Hand;
            //    dgvProperty.Rows[e.RowIndex].Cells[8].ToolTipText = "    " + dis_code;
            //}
        }

        private void dgvProperty_CellMouseLeave(object sender, DataGridViewCellEventArgs e)
        {
            //dgvProperty.Cursor = Cursors.Default;
        }

        private void cmbyear_Leave(object sender, EventArgs e)
        {
            wfePolicy wfepo = new wfePolicy(sqlCon);
            if (cmbDistrict.Text != "" && cmbWhereReg.Text != "" && cmbBook.Text != "" && cmbyear.Text != "")
            {
                if (wfepo.GetVol(cmbDistrict.SelectedValue.ToString(), cmbWhereReg.SelectedValue.ToString(), cmbBook.SelectedValue.ToString(), cmbyear.Text).Tables[0].Rows.Count > 0)
                {
                    cmbVol.DataSource = wfepo.GetVol(cmbDistrict.SelectedValue.ToString(), cmbWhereReg.SelectedValue.ToString(), cmbBook.SelectedValue.ToString(), cmbyear.Text).Tables[0];
                    cmbVol.DisplayMember = "Volume";
                    cmbVol.ValueMember = "Volume_No";
                    cmbVol.SelectedIndex = 0;
                }
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            
        }
        private bool saveUATinfo()
        {
            Boolean flag = false;
            wfePolicy wpol = new wfePolicy(sqlCon);
            if (wpol.SaveUatInfo(cmbRunnum.Text, txtPercent.Text.Trim(), crd.created_by, crd.created_dttm) == true)
            {
                flag = true;
            }

            return flag;
        }
        private void checkBox1_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(this, "Do You Want to Mark this Batch as Ready for UAT...?", "IGR...", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (grdBox.Rows.Count > 0)
            {
                if (result == DialogResult.Yes)
                {
                    DataSet ds = new DataSet();

                    //if (cmbDistrict.SelectedValue.ToString() != null && cmbWhereReg.SelectedValue.ToString() != null && cmbBook.SelectedValue.ToString() != null && cmbyear.Text != "" && cmbVol.Text != "")
                    //{
                        if (checkBox1.Checked == true)
                        {
                            //wfePolicy wfe = new wfePolicy(sqlCon);
                            //ds = wfe.GetProject(cmbDistrict.SelectedValue.ToString(), cmbWhereReg.SelectedValue.ToString(), cmbBook.SelectedValue.ToString(), cmbyear.Text, cmbVol.Text);
                            //string proj_key = ds.Tables[0].Rows[0]["proj_key"].ToString();
                            //string batch_key = ds.Tables[0].Rows[0]["Batch_key"].ToString();
                            
                            //groupBox5.Enabled = false;
                            for (int i = 0; i < grdBox.Rows.Count; i++)
                            {
                                string policyname = grdBox.Rows[i].Cells[0].Value.ToString() + grdBox.Rows[i].Cells[1].Value.ToString() + grdBox.Rows[i].Cells[2].Value.ToString() + grdBox.Rows[i].Cells[3].Value.ToString() + "[" + grdBox.Rows[i].Cells[4].Value.ToString() + "]";
                                if (deed_check(policyname) == false)
                                {
                                    MessageBox.Show(this,"There is Exception in Deed Number " + policyname,"Warning",MessageBoxButtons.OK,MessageBoxIcon.Error);
                                    return;
                                }
                            }
                            wfePolicy wfe = new wfePolicy(sqlCon);
                            if (wfe.ReadyForUAT(cmbRunnum.SelectedValue.ToString()) == true)
                            {
                                if (saveUATinfo() == true)
                                {
                                    MessageBox.Show(this, "Volumes Successfully Marked For UAT...", "IGR...", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                }
                            }
                            else
                            {
                                MessageBox.Show(this, "Error...", "IGR...", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    //}
                    //else
                    //{
                    //    MessageBox.Show(this, "Please select at least one Deed and try agin...", "IGR...", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    //}
                }
            }
        }
        private bool deed_check(string deedNo)
        {
            Boolean flag = false;
           
            wfePolicy wfe = new wfePolicy(sqlCon);
            try
            {
                int status = wfe.GetPolicyStatus(deedNo);
                if (status != 30)
                {
                    flag = true;
                }
                else
                {
                    flag = false;
                }
            }
            catch (Exception ex)
            {
                flag = false;
            }
            return flag;
        }
        private void cmbDistrict_Leave(object sender, EventArgs e)
        {
            populateRO();
        }

        private void cmbWhereReg_Leave(object sender, EventArgs e)
        {
            populateYear();
        }
        private void mImages_Click(object sender, EventArgs e)
        {
            ShowImages(grdBox.CurrentRow.Cells[0].Value.ToString(), grdBox.CurrentRow.Cells[1].Value.ToString(), grdBox.CurrentRow.Cells[2].Value.ToString(), grdBox.CurrentRow.Cells[3].Value.ToString(), grdBox.CurrentRow.Cells[4].Value.ToString());
        }
        private void mCheck_Click(object sender, EventArgs e)
        {
            try
            {
                DataLayerDefs.DeedControl pdc = new DataLayerDefs.DeedControl();
                //string policy_no = policyLst.SelectedItem.ToString();
                string district_code = grdBox.Rows[selectedIndex].Cells[0].Value.ToString();
                string ro_code = grdBox.Rows[selectedIndex].Cells[1].Value.ToString();
                string book = grdBox.Rows[selectedIndex].Cells[2].Value.ToString();
                string deed_year = grdBox.Rows[selectedIndex].Cells[3].Value.ToString();
                string deed_no = grdBox.Rows[selectedIndex].Cells[4].Value.ToString();
                if (district_code != null && ro_code != null && book != null && deed_year != null && deed_no != null)
                {
                    OdbcTransaction trans = sqlCon.BeginTransaction();
                    pdc.District_code = district_code;
                    pdc.RO_code = ro_code;
                    pdc.Book = book;
                    pdc.Deed_year = deed_year;
                    pdc.Deed_no = deed_no;
                    igr_deed igr = new igr_deed(sqlCon, trans, crd, pdc);
                    frmDeedsummery ds = new frmDeedsummery(sqlCon, trans, crd, igr, Mode._View);
                    ds.ShowDialog();
                }
                else
                {
                    MessageBox.Show(this, "Error ....", "Deed Switboard", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error while fetching the details... " + ex.Message);
            }
        }

        private void grdBox_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                ContextMenu mCheck = new ContextMenu();
                mCheck.MenuItems.Add(new MenuItem("Deed details"));
                mCheck.MenuItems.Add(new MenuItem("Images"));
                mCheck.MenuItems[0].Click += new EventHandler(mCheck_Click);
                mCheck.MenuItems[1].Click += new EventHandler(mImages_Click);
                int currentMouseOverRow = grdBox.HitTest(e.X, e.Y).RowIndex;
                selectedIndex = currentMouseOverRow;
                //if (currentMouseOverRow >= 0)
                //{
                //    mCheck.MenuItems.Add(new MenuItem(string.Format("", currentMouseOverRow.ToString())));
                //}
                mCheck.Show(grdBox, new Point(e.X, e.Y));
            }
        }

        private void dgvname_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dgvProperty_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void cmdRefine_Click(object sender, EventArgs e)
        {
            DataTable dt = new DataTable();
            wfeImage img = new wfeImage(sqlCon);
            if (img.GetAllDeedEXQA(cmbRunnum.SelectedValue.ToString()).Rows.Count > 0)
            {
                total_Count = img.GetAllDeedEXQA(cmbRunnum.SelectedValue.ToString()).Rows.Count;
                
            }
            try
            {
                if (Convert.ToInt32(txtPercent.Text.Trim()) <= 100)
                {
                    int percent = (total_Count * Convert.ToInt32(txtPercent.Text.Trim())/100);
                    dt = img.GetAllDeedEXQAPercent(cmbRunnum.SelectedValue.ToString(), percent.ToString());
                    lblperCount.Text = "Showing " + dt.Rows.Count.ToString() + " out of " + total_Count + " Records";
                    wfeBox wfeb = new wfeBox(sqlCon);
                    if (wfeb.GetRunnumStatus(cmbRunnum.SelectedValue.ToString()).Tables[0].Rows[0][0].ToString() == "7")
                    {
                        checkBox1.Checked = true;
                        checkBox1.Enabled = false;
                        cmdAccepted.Enabled = false;
                        cmdRejected.Enabled = false;
                    }
                    if (wfeb.GetRunnumStatus(cmbRunnum.SelectedValue.ToString()).Tables[0].Rows[0][0].ToString() == "66")
                    {
                        checkBox1.Checked = false;
                        checkBox1.Enabled = true;
                        cmdAccepted.Enabled = true;
                        cmdRejected.Enabled = true;
                    }
                    if (dt.Rows.Count > 0)
                    {
                        grdBox.DataSource = null;
                        dgvname.DataSource = null;
                        dgvProperty.DataSource = null;
                        grdBox.DataSource = dt;
                        grdBox.Columns[0].Visible = false;
                        grdBox.Columns[1].Visible = false;
                        grdBox.Columns[5].Visible = false;
                        grdBox.Columns[6].Visible = false;
                        grdBox.Columns[7].Visible = false;
                        grdBox.Columns[8].Visible = false;

                        grdBox.Columns[10].Width = 250;
                        //grdBox.Columns[6].Width = 250;
                        for (int i = 0; i < grdBox.Rows.Count; i++)
                        {
                            string deedno = grdBox.Rows[i].Cells[0].Value.ToString() + grdBox.Rows[i].Cells[1].Value.ToString() + grdBox.Rows[i].Cells[2].Value.ToString() + grdBox.Rows[i].Cells[3].Value.ToString() + "[" + grdBox.Rows[i].Cells[4].Value.ToString() + "]";
                            wfePolicy wfepol = new wfePolicy(sqlCon);
                            if (wfepol.deedCheck(deedno) == "30")
                            {
                                grdBox.Rows[i].DefaultCellStyle.BackColor = Color.Red;
                            }
                            if (wfepol.deedCheck(deedno) == "31")
                            {
                                grdBox.Rows[i].DefaultCellStyle.BackColor = Color.Green;
                            }
                            if (grdBox.Rows[i].Cells[19].Value.ToString() == "Y")
                            {
                                grdBox.Rows[i].DefaultCellStyle.BackColor = Color.Yellow;
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("No Record Found .....");
                    }
                }
                else
                {

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            ZoomIn();
        }

        private void cmdout_Click(object sender, EventArgs e)
        {
            ZoomOut();
        }

        private void chkSpelmis_CheckedChanged(object sender, EventArgs e)
        {
            int tifPos;
            string origDoctype = string.Empty;
            if (lstImage.SelectedIndex >= 0)
            {
                tifPos = lstImage.SelectedItem.ToString().IndexOf("-") + 1;
                string imgNumber;
                imgNumber = lstImage.SelectedItem.ToString().Substring((lstImage.SelectedItem.ToString().IndexOf("_") + 1), 5);
                if (tifPos > 0)
                {
                    origDoctype = lstImage.SelectedItem.ToString().Substring(tifPos);
                }
                if (chkSpelmis.Checked)
                {
                    txtComments.Text = txtComments.Text + imgNumber + "-" + origDoctype + " Spelling Mistake \r\n";
                    txtComments.SelectionStart = txtComments.Text.Length;
                    txtComments.ScrollToCaret();
                    txtComments.Refresh();
                }
                else
                {
                    string strToReplace;
                    strToReplace = imgNumber + "-" + origDoctype + " Spelling Mistake \r\n";
                    txtComments.Text = txtComments.Text.Replace(strToReplace, "");
                }
            }
        }

        private void chkWrongProperty_CheckedChanged(object sender, EventArgs e)
        {
            int tifPos;
            string origDoctype = string.Empty;
            if (lstImage.SelectedIndex >= 0)
            {
                tifPos = lstImage.SelectedItem.ToString().IndexOf("-") + 1;
                string imgNumber;
                imgNumber = lstImage.SelectedItem.ToString().Substring((lstImage.SelectedItem.ToString().IndexOf("_") + 1), 5);
                if (tifPos > 0)
                {
                    origDoctype = lstImage.SelectedItem.ToString().Substring(tifPos);
                }
                if (chkWrongProperty.Checked)
                {
                    txtComments.Text = txtComments.Text + imgNumber + "-" + origDoctype + " Wrong Property \r\n";
                    txtComments.SelectionStart = txtComments.Text.Length;
                    txtComments.ScrollToCaret();
                    txtComments.Refresh();
                }
                else
                {
                    string strToReplace;
                    strToReplace = imgNumber + "-" + origDoctype + " Wrong Property \r\n";
                    txtComments.Text = txtComments.Text.Replace(strToReplace, "");
                }
            }
        }

        private void splitContainer4_Panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void tbImages_Click(object sender, EventArgs e)
        {

        }

        private void cmdGeneratereport_Click(object sender, EventArgs e)
        {
            DataSet dsVol = new DataSet();
            OdbcDataAdapter sqlAdap = null;
            DataSet dsDeed = new DataSet();
            OdbcDataAdapter sqlTemp = null;
            string sql = "select a.run_no as 'Submission No.', c.district_name as District,d.ro_name as RO, a.deed_year as Year, count(*) as 'Total' from policy_master a, district c, ro_master d where a.do_code=c.district_code and a.run_no='"+cmbRunnum.Text.ToString().Trim()+"' and d.district_code=a.do_code and d.ro_code=a.br_code";
            sqlAdap = new OdbcDataAdapter(sql, sqlCon);
            sqlAdap.Fill(dsVol);
            string qry = "select distinct a.deed_vol as Volume, a.deed_no as Deed, b.qa_status as Status from policy_master a, lic_qa_log b, district c, ro_master d  where a.do_code=c.district_code and a.run_no='" + cmbRunnum.Text.ToString().Trim() + "'and a.policy_number=b.policy_number and b.qa_status=0 and d.district_code=a.do_code and d.ro_code=a.br_code";
            sqlTemp = new OdbcDataAdapter(qry, sqlCon);
            sqlTemp.Fill(dsDeed);
            FileStream fs = new FileStream("UAT_Report.pdf", FileMode.Create, FileAccess.Write, FileShare.None);
            Document doc = new Document();
            PdfWriter writer = PdfWriter.GetInstance(doc, fs);
            doc.Open();
            doc.Add(new Paragraph("Submission No. "+ dsVol.Tables[0].Rows[0][0].ToString()));
            doc.Add(new Paragraph("District. " + dsVol.Tables[0].Rows[0][1].ToString()));
            doc.Add(new Paragraph("RO. " + dsVol.Tables[0].Rows[0][2].ToString()));
            doc.Add(new Paragraph("Year. " + dsVol.Tables[0].Rows[0][3].ToString()));
            doc.Add(new Paragraph("Total. " + dsVol.Tables[0].Rows[0][4].ToString()));
            doc.Add(new Paragraph("\n"));
            doc.Add(new Paragraph("\n"));
            PdfPTable table1 = new PdfPTable(3);
            table1.WidthPercentage = 90;
            PdfPTable table2 = new PdfPTable(3);
            table2.WidthPercentage = 90;
            PdfPCell cell1 = new PdfPCell();
            PdfPCell cell2 = new PdfPCell();
            PdfPCell cell3 = new PdfPCell();
            cell1.AddElement(new Paragraph("Volume No " ));
            cell2.AddElement(new Paragraph("Deed No  "));
            cell3.AddElement(new Paragraph("Status  "));
            table2.AddCell(cell1);
            table2.AddCell(cell2);
            table2.AddCell(cell3);
            cell1.VerticalAlignment = Element.ALIGN_CENTER;
            cell2.VerticalAlignment = Element.ALIGN_CENTER;
            cell3.VerticalAlignment = Element.ALIGN_CENTER;
            doc.Add(table2);
            for (int i = 0; i < dsDeed.Tables[0].Rows.Count; i++)
            {
                PdfPCell cell11 = new PdfPCell();
                PdfPCell cell12 = new PdfPCell();
                PdfPCell cell13 = new PdfPCell();

                cell11.AddElement(new Paragraph(dsDeed.Tables[0].Rows[i][0].ToString()));

                cell12.AddElement(new Paragraph( dsDeed.Tables[0].Rows[i][1].ToString()));
                if (dsDeed.Tables[0].Rows[i][2].ToString() == "0")
                {
                    cell13.AddElement(new Paragraph("Checked"));
                }
                else
                {
                    cell13.AddElement(new Paragraph("Not Checked"));
                }

                cell11.VerticalAlignment = Element.ALIGN_CENTER;
                cell12.VerticalAlignment = Element.ALIGN_CENTER;
                cell13.VerticalAlignment = Element.ALIGN_CENTER;
                table1.AddCell(cell11);
                table1.AddCell(cell12);
                table1.AddCell(cell13);
            }
            doc.Add(table1);
            doc.Close();
        }
        
	}
}
