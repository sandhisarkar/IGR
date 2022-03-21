/*
 * Created by SharpDevelop.
 * User: SubhajitB
 * Date: 18/2/2009
 * Time: 11:43 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Drawing;
using System.Windows.Forms;
using NovaNet.wfe;
using NovaNet.Utils;
using System.Data.Odbc;
using System.Data;
using LItems;
using System.Reflection;
using LoadPlugin;
using NP;
using System.Collections.Generic;
using System.IO;
using igr_base;
using IGRFqc;
using DataLayerDefs;
using TestComponents;
using BurnMedia;
using System.Data.OleDb;
using System.Globalization;


namespace ImageHeaven
{
	/// <summary>
	/// Description of frmMain.
	/// </summary>
    public partial class frmMain : Form
    {
        static wItem wi;
        //NovaNet.Utils.dbCon dbcon;
        frmMain mainForm;
        OdbcConnection sqlCon = null;
        private Credentials crd = new Credentials();
        static int colorMode;
        dbCon dbcon;
        //
        NovaNet.Utils.GetProfile pData;
        NovaNet.Utils.ChangePassword pCPwd;
        NovaNet.Utils.Profile p;
        public static NovaNet.Utils.IntrRBAC rbc;
        private short logincounter;
        //

        public static string projectName = null;
        public static string batchName = null;
        public static string boxNumber = null;
        public static string projectVal = null;
        public static string batchVal = null;
        List<NIPlugin> np = new List<NIPlugin>();

        public frmMain(OdbcConnection pCon)
        {
            //
            // The InitializeComponent() call is required for Windows Forms designer support.
            //


            InitializeComponent();
            sqlCon = pCon;

            logincounter = 0;
            //
            // TODO: Add constructor code after the InitializeComponent() call.
            //
            ImageHeaven.Program.Logout = false;
        }
        public frmMain()
        {
            //
            // The InitializeComponent() call is required for Windows Forms designer support.
            //
            AssemblyName assemName = Assembly.GetExecutingAssembly().GetName();
            this.Text = "B'Zer" + "           Version: " + assemName.ToString();
            InitializeComponent();

            logincounter = 0;
            //
            // TODO: Add constructor code after the InitializeComponent() call.
            //
        }
        //		public frmMain(string prmProject)
        //		{
        //			projectVal=prmProject;
        //		}

        public void SetValues(wItem pBox)
        {
            wi = (wfeBox)pBox;

        }
        public void SetValues(wItem pBox, int prmMode)
        {
            wi = (wfeBox)pBox;
            colorMode = prmMode;
        }
        void ProjectToolStripMenuItemClick(object sender, EventArgs e)
        {
            try
            {
                aeProject dispProject;
                wi = new wfeProject(sqlCon);
                dispProject = new aeProject(wi, sqlCon);
                dispProject.ShowDialog(this);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        void ProjectToolStripMenuItem1Click(object sender, EventArgs e)
        {
            aePageCount pagecount = new aePageCount(sqlCon, crd);
            pagecount.ShowDialog(this);
        }
        private List<string> GetFiles(string Path, string ext)
        {
            List<string> fls = new List<string>();
            if (Directory.Exists(Path))
            {
                foreach (string f in Directory.GetFiles(Path, "*." + ext))
                {
                    fls.Add(new FileInfo(f).FullName);
                }
            }
            else
            {
                throw new System.Exception("Folder doesn't exist");
            }
            return fls;
        }
        void FrmMainLoad(object sender, EventArgs e)
        {
            OdbcCommand sqlCmd = new OdbcCommand();
            string sql = "update ac_role set role_description = 'IGR Audit' where role_description = 'LIC'";
            sqlCmd.Connection = sqlCon;
            sqlCmd.CommandText = sql;
            sqlCmd.ExecuteNonQuery();


            int k;
            dbcon = new NovaNet.Utils.dbCon();
            //System.Collections.Generic.List<NIPlugin> plgList = new System.Collections.Generic.List<NIPlugin>();
            try
            {
                
                string dllPaths = string.Empty;

                if (sqlCon.State == ConnectionState.Open)
                {
                    pData = getData;
                    pCPwd = getCPwd;
                    rbc = new NovaNet.Utils.RBAC(sqlCon, dbcon, pData, pCPwd);
                    //string test = sqlCon.Database;
                    GetChallenge gc = new GetChallenge(getData);
                    gc.ShowDialog(this);
                    ///get credential for the logged user
                    crd = rbc.getCredentials(p);
                    NPlugin nPlug = null;
                    dllPaths = Path.GetDirectoryName(Application.ExecutablePath) + @"\Reports";
                    List<string> fls = GetFiles(dllPaths, "DLL");
                    IEnumerator<string> itrFname = fls.GetEnumerator();
                    ToolStripMenuItem mnu = (ToolStripMenuItem)menuStrip1.Items[4];
                    ToolStripMenuItem tlc = null;
                    tlc = (ToolStripMenuItem)mnu.DropDownItems.Add("Plugin Reports");
                    while (itrFname.MoveNext())
                    {
                        nPlug = new NPlugin(itrFname.Current);
                        np = nPlug.GetPlugin;
                        IEnumerator<NIPlugin> itr = np.GetEnumerator();

                        while (itr.MoveNext())
                        {
                            NIPluginReport nr = (NIPluginReport)itr.Current;
                            nr.Init(sqlCon);
                            tlc.DropDownItems.Add(nr.EntryPoint);
                            if (nr.GetParams == true)
                                tlc.DropDownItems[tlc.DropDownItems.Count - 1].Click += delegate { nr.Show(this, p); };
                        }
                    }
                    //if (databaseImport())
                    //{
                        InsertDatainDb();
                    //}
                    ///changed in version 1.0.2
                    stsName.Text = "User name - " + p.UserName;
                    stsRole.Text = "Role - " + p.Role_des;
                    if (p.Role_des == ihConstants._LIC_ROLE)
                    {
                        boxSummaryToolStripMenuItem.Visible = false;
                        batchSummeryToolStripMenuItem.Visible = false;
                        genericReportsToolStripMenuItem.Visible = false;
                        uATReportToolStripMenuItem.Visible = false;
                        
                        newUserToolStripMenuItem.Visible = false;
                        toolOnlineUser.Visible = false;
                        deedWiseToolStripMenuItem.Visible = true;
                        
                    }
                    if (p.Role_des != ihConstants._ADMINISTRATOR_ROLE && p.Role_des != ihConstants._LIC_ROLE)
                    {
                        boxSummaryToolStripMenuItem.Visible = false;
                        batchSummeryToolStripMenuItem.Visible = true;
                        genericReportsToolStripMenuItem.Visible = true;
                        uATReportToolStripMenuItem.Visible = true;
                        genericReportsBookIVToolStripMenuItem.Visible = true;
                        boxSummaryToolStripMenuItem.Enabled = true;
                        batchSummeryToolStripMenuItem.Enabled = true;
                        genericReportsToolStripMenuItem.Enabled = true;
                        uATReportToolStripMenuItem.Enabled = true;
                        genericReportsBookIVToolStripMenuItem.Enabled = true;
                        deedWiseToolStripMenuItem.Visible = true;
                        
                    }
                    if (p.Role_des != ihConstants._ADMINISTRATOR_ROLE)
                    {
                        toolsToolStripMenuItem.Visible = true;
                        toolsToolStripMenuItem.Enabled = true;
                        changePasswordToolStripMenuItem.Visible = true;
                        changePasswordToolStripMenuItem.Enabled = true;
                        systemConfigToolStripMenuItem.Visible = false;
                        systemConfigToolStripMenuItem.Enabled = false;
                        newUserToolStripMenuItem.Visible = false;
                        toolOnlineUser.Visible = false;
                        boxSummaryToolStripMenuItem.Visible = false;
                       

                        DataSet ds = rbc.getResource(p.UserId);
                        if (ds.Tables[0].Rows.Count > 0)
                        {

                            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                            {
                                for (int j = 0; j < menuStrip1.Items.Count; j++)
                                {
                                    ///For parent menus
                                    if ((ds.Tables[0].Rows[i]["resource_id"].ToString()) == menuStrip1.Items[j].Name.ToString())
                                    {
                                        menuStrip1.Items[j].Visible = true;
                                        menuStrip1.Items[j].Enabled = true;
                                    }
                                }
                                ///For New menus
                                for (k = 0; k < itemsToolStripMenuItem.DropDownItems.Count; k++)
                                {
                                    if ((ds.Tables[0].Rows[i]["resource_id"].ToString()) == itemsToolStripMenuItem.DropDownItems[k].Name.ToString())
                                    {
                                        itemsToolStripMenuItem.DropDownItems[k].Visible = true;
                                        itemsToolStripMenuItem.DropDownItems[k].Enabled = true;
                                    }
                                }

                                ///For transaction menus
                                for (k = 0; k < transactinToolStripMenuItem.DropDownItems.Count; k++)
                                {
                                    if ((ds.Tables[0].Rows[i]["resource_id"].ToString()) == transactinToolStripMenuItem.DropDownItems[k].Name.ToString())
                                    {
                                        transactinToolStripMenuItem.DropDownItems[k].Visible = true;
                                        transactinToolStripMenuItem.DropDownItems[k].Enabled = true;
                                    }
                                }

                                ///For LIC menus
                                for (k = 0; k < toolStripMenuItem2.DropDownItems.Count; k++)
                                {
                                    if ((ds.Tables[0].Rows[i]["resource_id"].ToString()) == toolStripMenuItem2.DropDownItems[k].Name.ToString())
                                    {
                                        toolStripMenuItem2.DropDownItems[k].Visible = true;
                                        toolStripMenuItem2.DropDownItems[k].Enabled = true;
                                    }
                                }

                                ///For Tools menus
                                for (k = 0; k < toolsToolStripMenuItem.DropDownItems.Count; k++)
                                {
                                    if ((ds.Tables[0].Rows[i]["resource_id"].ToString()) == toolsToolStripMenuItem.DropDownItems[k].Name.ToString())
                                    {
                                        toolsToolStripMenuItem.DropDownItems[k].Visible = true;
                                        toolsToolStripMenuItem.DropDownItems[k].Enabled = true;

                                    }

                                }

                                ///For Tools menus
                                reportsToolStripMenuItem.Visible = true;
                                for (k = 0; k < reportsToolStripMenuItem.DropDownItems.Count; k++)
                                {
                                    if ((ds.Tables[0].Rows[i]["resource_id"].ToString()) == reportsToolStripMenuItem.DropDownItems[k].Name.ToString())
                                    {
                                        reportsToolStripMenuItem.DropDownItems[k].Visible = true;
                                        reportsToolStripMenuItem.DropDownItems[k].Enabled = true;
                                    }
                                }
                                boxSummaryToolStripMenuItem.Visible = false;
                                deedWiseToolStripMenuItem.Visible = true;
                                
                                ///For enable/disable toolstrip button
                                for (k = 0; k < toolStrip1.Items.Count; k++)
                                {
                                    if ((ds.Tables[0].Rows[i]["resource_id"].ToString()) == toolStrip1.Items[k].Name.ToString())
                                    {
                                        toolStrip1.Items[k].Visible = true;
                                        toolStrip1.Items[k].Enabled = true;
                                    }
                                }
                            }
                        }
                    }
                    else if (p.Role_des == ihConstants._LIC_ROLE)
                    {
                        boxSummaryToolStripMenuItem.Visible = false;
                        batchSummeryToolStripMenuItem.Visible = false;
                        genericReportsToolStripMenuItem.Visible = false;
                        uATReportToolStripMenuItem.Visible = false;
                        
                        newUserToolStripMenuItem.Visible = false;
                        toolOnlineUser.Visible = false;
                        deedWiseToolStripMenuItem.Visible = true;
                        
                    }
                    else
                    {
                        for (int j = 0; j < menuStrip1.Items.Count; j++)
                        {
                            ///For parent menus
                            menuStrip1.Items[j].Visible = true;
                            menuStrip1.Items[j].Enabled = true;
                        }
                        ///For New menus
                        for (k = 0; k < itemsToolStripMenuItem.DropDownItems.Count; k++)
                        {
                            itemsToolStripMenuItem.DropDownItems[k].Visible = true;
                            itemsToolStripMenuItem.DropDownItems[k].Enabled = true;
                        }

                        ///For transaction menus
                        for (k = 0; k < transactinToolStripMenuItem.DropDownItems.Count; k++)
                        {
                            transactinToolStripMenuItem.DropDownItems[k].Visible = true;
                            transactinToolStripMenuItem.DropDownItems[k].Enabled = true;
                        }

                        ///For LIC menus
                        for (k = 0; k < toolStripMenuItem2.DropDownItems.Count; k++)
                        {
                            toolStripMenuItem2.DropDownItems[k].Visible = true;
                            toolStripMenuItem2.DropDownItems[k].Enabled = true;
                        }

                        ///For Tools menus
                        for (k = 0; k < toolsToolStripMenuItem.DropDownItems.Count; k++)
                        {
                            toolsToolStripMenuItem.DropDownItems[k].Visible = true;
                            toolsToolStripMenuItem.DropDownItems[k].Enabled = true;
                        }

                        ///For Tools menus
                        for (k = 0; k < reportsToolStripMenuItem.DropDownItems.Count; k++)
                        {
                            reportsToolStripMenuItem.DropDownItems[k].Visible = true;
                            reportsToolStripMenuItem.DropDownItems[k].Enabled = true;
                        }
                        boxSummaryToolStripMenuItem.Visible = false;
                        deedWiseToolStripMenuItem.Visible = true;
                       
                        ///For enable/disable toolstrip button
                        for (k = 0; k < toolStrip1.Items.Count; k++)
                        {
                            toolStrip1.Items[k].Visible = true;
                            toolStrip1.Items[k].Enabled = true;
                        }
                    }
                    mnuJobCreation.Visible = false;
                    projectToolStripMenuItem1.Visible = false;
                    indexingToolStripMenuItem.Visible = false;
                    expertQualityControlCentreToolStripMenuItem.Visible = true;
                    reexportToolStripMenuItem.Visible = false;
                    
                    DataSet dsactive = new DataSet();
                    wfeProject wfep = new wfeProject(sqlCon);
                    dsactive = wfep.getActive();
                    if (dsactive.Tables[0].Rows.Count < 1)
                    {
                        frmsysCon frmsys = new frmsysCon(sqlCon, crd);
                        frmsys.ShowDialog(this);
                    }

                    batchSummeryToolStripMenuItem.Visible = false;
                    renameBatchToolStripMenuItem.Visible = false;
                    renameBatchToolStripMenuItem.Visible = false;
                    //transferredDataToolStripMenuItem.Visible = false;

                }
                else
                {
                    dbcon = new NovaNet.Utils.dbCon();
                    sqlCon.Open();


                    if (sqlCon.State == ConnectionState.Open)
                    {
                        pData = getData;
                        pCPwd = getCPwd;
                        rbc = new NovaNet.Utils.RBAC(sqlCon, dbcon, pData, pCPwd);
                        //string test = sqlCon.Database;
                        GetChallenge gc = new GetChallenge(getData);
                        gc.ShowDialog(this);
                        ///get credential for the logged user
                        crd = rbc.getCredentials(p);
                        NPlugin nPlug = null;
                        dllPaths = Path.GetDirectoryName(Application.ExecutablePath) + @"\Reports";
                        List<string> fls = GetFiles(dllPaths, "DLL");
                        IEnumerator<string> itrFname = fls.GetEnumerator();
                        ToolStripMenuItem mnu = (ToolStripMenuItem)menuStrip1.Items[4];
                        ToolStripMenuItem tlc = null;
                        tlc = (ToolStripMenuItem)mnu.DropDownItems.Add("Plugin Reports");
                        while (itrFname.MoveNext())
                        {
                            nPlug = new NPlugin(itrFname.Current);
                            np = nPlug.GetPlugin;
                            IEnumerator<NIPlugin> itr = np.GetEnumerator();

                            while (itr.MoveNext())
                            {
                                NIPluginReport nr = (NIPluginReport)itr.Current;
                                nr.Init(sqlCon);
                                tlc.DropDownItems.Add(nr.EntryPoint);
                                if (nr.GetParams == true)
                                    tlc.DropDownItems[tlc.DropDownItems.Count - 1].Click += delegate { nr.Show(this, p); };
                            }
                        }
                        //if (databaseImport())
                        //{
                            InsertDatainDb();
                        //}
                        ///changed in version 1.0.2
                        stsName.Text = "User name - " + p.UserName;
                        stsRole.Text = "Role - " + p.Role_des;
                        if (p.Role_des != ihConstants._ADMINISTRATOR_ROLE)
                        {
                            toolsToolStripMenuItem.Visible = true;
                            toolsToolStripMenuItem.Enabled = true;
                            changePasswordToolStripMenuItem.Visible = true;
                            changePasswordToolStripMenuItem.Enabled = true;
                            
                            DataSet ds = rbc.getResource(p.UserId);
                            if (ds.Tables[0].Rows.Count > 0)
                            {

                                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                                {
                                    for (int j = 0; j < menuStrip1.Items.Count; j++)
                                    {
                                        ///For parent menus
                                        if ((ds.Tables[0].Rows[i]["resource_id"].ToString()) == menuStrip1.Items[j].Name.ToString())
                                        {
                                            menuStrip1.Items[j].Visible = true;
                                            menuStrip1.Items[j].Enabled = true;
                                        }
                                    }
                                    ///For New menus
                                    for (k = 0; k < itemsToolStripMenuItem.DropDownItems.Count; k++)
                                    {
                                        if ((ds.Tables[0].Rows[i]["resource_id"].ToString()) == itemsToolStripMenuItem.DropDownItems[k].Name.ToString())
                                        {
                                            itemsToolStripMenuItem.DropDownItems[k].Visible = true;
                                            itemsToolStripMenuItem.DropDownItems[k].Enabled = true;
                                        }
                                    }

                                    ///For transaction menus
                                    for (k = 0; k < transactinToolStripMenuItem.DropDownItems.Count; k++)
                                    {
                                        if ((ds.Tables[0].Rows[i]["resource_id"].ToString()) == transactinToolStripMenuItem.DropDownItems[k].Name.ToString())
                                        {
                                            transactinToolStripMenuItem.DropDownItems[k].Visible = true;
                                            transactinToolStripMenuItem.DropDownItems[k].Enabled = true;
                                        }
                                    }

                                    ///For LIC menus
                                    for (k = 0; k < toolStripMenuItem2.DropDownItems.Count; k++)
                                    {
                                        if ((ds.Tables[0].Rows[i]["resource_id"].ToString()) == toolStripMenuItem2.DropDownItems[k].Name.ToString())
                                        {
                                            toolStripMenuItem2.DropDownItems[k].Visible = true;
                                            toolStripMenuItem2.DropDownItems[k].Enabled = true;
                                        }
                                    }

                                    ///For Tools menus
                                    for (k = 0; k < toolsToolStripMenuItem.DropDownItems.Count; k++)
                                    {
                                        if ((ds.Tables[0].Rows[i]["resource_id"].ToString()) == toolsToolStripMenuItem.DropDownItems[k].Name.ToString())
                                        {
                                            toolsToolStripMenuItem.DropDownItems[k].Visible = true;
                                            toolsToolStripMenuItem.DropDownItems[k].Enabled = true;

                                        }

                                    }

                                    ///For Tools menus
                                    reportsToolStripMenuItem.Visible = true;
                                    for (k = 0; k < reportsToolStripMenuItem.DropDownItems.Count; k++)
                                    {
                                        if ((ds.Tables[0].Rows[i]["resource_id"].ToString()) == reportsToolStripMenuItem.DropDownItems[k].Name.ToString())
                                        {
                                            reportsToolStripMenuItem.DropDownItems[k].Visible = true;
                                            reportsToolStripMenuItem.DropDownItems[k].Enabled = true;
                                            genericReportsBookIVToolStripMenuItem.Visible = false;
                                        }
                                    }
                                    boxSummaryToolStripMenuItem.Visible = false;
                                    deedWiseToolStripMenuItem.Visible = true;
                                    

                                    ///For enable/disable toolstrip button
                                    for (k = 0; k < toolStrip1.Items.Count; k++)
                                    {
                                        if ((ds.Tables[0].Rows[i]["resource_id"].ToString()) == toolStrip1.Items[k].Name.ToString())
                                        {
                                            toolStrip1.Items[k].Visible = true;
                                            toolStrip1.Items[k].Enabled = true;
                                        }
                                    }
                                }
                            }
                        }
                        else if (p.Role_des == ihConstants._LIC_ROLE)
                        {
                            boxSummaryToolStripMenuItem.Visible = false;
                            batchSummeryToolStripMenuItem.Visible = false;
                            genericReportsToolStripMenuItem.Visible = false;
                            uATReportToolStripMenuItem.Visible = false;
                            genericReportsBookIVToolStripMenuItem.Visible = false;
                            newUserToolStripMenuItem.Visible = false;
                            toolOnlineUser.Visible = false;
                            deedWiseToolStripMenuItem.Visible = true;
                           
                        }
                        else
                        {
                            for (int j = 0; j < menuStrip1.Items.Count; j++)
                            {
                                ///For parent menus
                                menuStrip1.Items[j].Visible = true;
                                menuStrip1.Items[j].Enabled = true;
                            }
                            ///For New menus
                            for (k = 0; k < itemsToolStripMenuItem.DropDownItems.Count; k++)
                            {
                                itemsToolStripMenuItem.DropDownItems[k].Visible = true;
                                itemsToolStripMenuItem.DropDownItems[k].Enabled = true;
                            }

                            ///For transaction menus
                            for (k = 0; k < transactinToolStripMenuItem.DropDownItems.Count; k++)
                            {
                                transactinToolStripMenuItem.DropDownItems[k].Visible = true;
                                transactinToolStripMenuItem.DropDownItems[k].Enabled = true;
                            }

                            ///For LIC menus
                            for (k = 0; k < toolStripMenuItem2.DropDownItems.Count; k++)
                            {
                                toolStripMenuItem2.DropDownItems[k].Visible = true;
                                toolStripMenuItem2.DropDownItems[k].Enabled = true;
                            }

                            ///For Tools menus
                            for (k = 0; k < toolsToolStripMenuItem.DropDownItems.Count; k++)
                            {
                                toolsToolStripMenuItem.DropDownItems[k].Visible = true;
                                toolsToolStripMenuItem.DropDownItems[k].Enabled = true;
                            }

                            ///For Tools menus
                            for (k = 0; k < reportsToolStripMenuItem.DropDownItems.Count; k++)
                            {
                                reportsToolStripMenuItem.DropDownItems[k].Visible = true;
                                reportsToolStripMenuItem.DropDownItems[k].Enabled = true;
                                genericReportsBookIVToolStripMenuItem.Visible = false;
                            }
                            boxSummaryToolStripMenuItem.Visible = false;
                            deedWiseToolStripMenuItem.Visible = true;
                            
                            ///For enable/disable toolstrip button
                            for (k = 0; k < toolStrip1.Items.Count; k++)
                            {
                                toolStrip1.Items[k].Visible = true;
                                toolStrip1.Items[k].Enabled = true;
                            }
                        }
                        mnuJobCreation.Visible = false;
                        projectToolStripMenuItem1.Visible = false;
                        indexingToolStripMenuItem.Visible = false;
                        expertQualityControlCentreToolStripMenuItem.Visible = true;
                        reexportToolStripMenuItem.Visible = false;
                        
                        DataSet dsactive = new DataSet();
                        wfeProject wfep = new wfeProject(sqlCon);
                        dsactive = wfep.getActive();
                        if (dsactive.Tables[0].Rows.Count < 1)
                        {
                            frmsysCon frmsys = new frmsysCon(sqlCon, crd);
                            frmsys.ShowDialog(this);
                        }

                        batchSummeryToolStripMenuItem.Visible = false;
                        renameBatchToolStripMenuItem.Visible = false;
                        renameBatchToolStripMenuItem.Visible = false;
                       
                        //transferredDataToolStripMenuItem.Visible = false;
                        if (p.Role_des == ihConstants._LIC_ROLE)
                        {
                            boxSummaryToolStripMenuItem.Visible = false;
                            batchSummeryToolStripMenuItem.Visible = false;
                            genericReportsToolStripMenuItem.Visible = false;
                            uATReportToolStripMenuItem.Visible = false;
                            
                            newUserToolStripMenuItem.Visible = false;
                            toolOnlineUser.Visible = false;
                            deedWiseToolStripMenuItem.Visible = true;
                            
                        }
                    }
                }
                AssemblyName assemName = Assembly.GetExecutingAssembly().GetName();
                this.Text = "B'Zer" + "           Version: " + assemName.Version.ToString() + "    Database name: " + sqlCon.Database.ToString() + "    Logged in user: " + crd.userName;
                transferredDataToolStripMenuItem.Visible = false;

            }
            catch (DBConnectionException dbex)
            {
                //MessageBox.Show(dbex.Message, "Image Heaven", MessageBoxButtons.OK, MessageBoxIcon.Error);
                string err = dbex.Message;
                this.Close();
            }
            //catch (Exception ex)
            //{

            //}

        }

        // Used for Login
        void getData(ref NovaNet.Utils.Profile prmp)
        {
            int i;
            p = prmp;
            for (i = 1; i <= 2; i++)
            {
                if (rbc.authenticate(p.UserId, p.Password) == false)
                {
                    if (logincounter == 2)
                    {
                        Application.Exit();
                    }
                    else
                    {
                        logincounter++;
                        GetChallenge ogc = new GetChallenge(getData);
                        ogc.ShowDialog(this);
                    }
                }
                else
                {
                    if (rbc.CheckUserIsLogged(p.UserId))
                    {

                        p = rbc.getProfile();
                        crd = rbc.getCredentials(p);
                        if (crd.role != ihConstants._ADMINISTRATOR_ROLE)
                        {
                            rbc.LockedUser(p.UserId, crd.created_dttm);
                        }
                        break;
                    }
                    else
                    {
                        p.UserId = null;
                        p.UserName = null;
                        GetChallenge ogc = new GetChallenge(getData);
                        AssemblyName assemName = Assembly.GetExecutingAssembly().GetName();
                        this.Text = "B'Zer" + "           Version: " + assemName.Version.ToString() + "    Database name: " + sqlCon.Database.ToString() + "    Logged in user: " + crd.userName;
                        ogc.ShowDialog(this);
                    }
                }
            }
        }

        void BatchToolStripMenuItemClick(object sender, EventArgs e)
        {
            frmAddEdit dispBatch;
            mainForm = new frmMain();
            wi = new wfeBatch(sqlCon);
            dispBatch = new aeBatch(wi, sqlCon);
            dispBatch.ShowDialog(mainForm);
        }

        void UploadCSVToolStripMenuItemClick(object sender, EventArgs e)
        {
            //aeCSV csvUploader=new aeCSV(sqlCon,crd);
            //mainForm=new frmMain();
            //csvUploader.ShowDialog(mainForm);
            frmDataUploader upload = new frmDataUploader(sqlCon, crd);
            mainForm = new frmMain();
            upload.ShowDialog(mainForm);

        }


        void ConfigurationToolStripMenuItemClick(object sender, EventArgs e)
        {
            aeConfiguration csvUploader = new aeConfiguration();
            mainForm = new frmMain();
            csvUploader.ShowDialog(mainForm);
        }


        void QualityControlCentreToolStripMenuItemClick(object sender, EventArgs e)
        {
            eSTATES[] state = new eSTATES[1];
            state[0] = eSTATES.POLICY_SCANNED;

            aeBoxSelection box = new aeBoxSelection(state, sqlCon);
            //mainForm=new frmMain();
            wi = null;
            box.ShowDialog(this);
            wfeBox tmpBox = (wfeBox)wi;
            if (tmpBox != null)
            {
                if ((tmpBox.ctrlBox.ProjectCode.ToString() != null) && (tmpBox.ctrlBox.BatchKey.ToString() != null) && (tmpBox.ctrlBox.BoxNumber.ToString() != null))
                {
                    Form activeChild = this.ActiveMdiChild;
                    if (activeChild == null)
                    {
                        aeImageQC frmQc = new aeImageQC(tmpBox, sqlCon, crd);
                        frmQc.MdiParent = this;
                        frmQc.Height = this.ClientRectangle.Height;
                        frmQc.Width = this.ClientRectangle.Width;
                        frmQc.Show();
                    }
                }
            }
        }

        void IndexingToolStripMenuItemClick(object sender, EventArgs e)
        {
            eSTATES[] state = new eSTATES[2];
            state[0] = eSTATES.POLICY_QC;
            state[1] = eSTATES.POLICY_ON_HOLD; // should be removed after demo

            aeBoxSelection box = new aeBoxSelection(state, sqlCon);
            wfeBox tmpBox = null;
            wi = null;
            box.ShowDialog(this);
            tmpBox = (wfeBox)wi;
            if (tmpBox != null)
            {
                if ((tmpBox.ctrlBox.ProjectCode.ToString() != null) && (tmpBox.ctrlBox.BatchKey.ToString() != null) && (tmpBox.ctrlBox.BoxNumber.ToString() != null))
                {
                    //wfeBox tmpBox = (wfeBox)wi;
                    Form activeChild = this.ActiveMdiChild;
                    if (activeChild == null)
                    {
                        aeIndexing frmIndex = new aeIndexing(tmpBox, sqlCon, crd);
                        frmIndex.MdiParent = this;
                        frmIndex.Height = this.ClientRectangle.Height;
                        frmIndex.Width = this.ClientRectangle.Width;
                        frmIndex.Show();
                    }
                }
            }
        }

        void ToolStripMenuItem1Click(object sender, EventArgs e)
        {
            eSTATES[] state = new eSTATES[1];
            state[0] = eSTATES.POLICY_CREATED;
            aeBoxSelection box = new aeBoxSelection(state, sqlCon);
            box.chkPhotoScan.Visible = true;
            wfeBox tmpBox = null;
            wi = null;
            box.ShowDialog(this);
            tmpBox = (wfeBox)wi;
            if (tmpBox != null)
            {
                if ((tmpBox.ctrlBox.ProjectCode.ToString() != null) && (tmpBox.ctrlBox.BatchKey.ToString() != null) && (tmpBox.ctrlBox.BoxNumber.ToString() != null))
                {
                    //wfeBox tmpBox = (wfeBox)wi;
                    Form activeChild = this.ActiveMdiChild;
                    if (activeChild == null)
                    {
                        //aePolicyScan frmScan = new aePolicyScan(tmpBox, sqlCon, crd, colorMode);
                        //frmScan.MdiParent = this;
                        //frmScan.Height = this.ClientRectangle.Height;
                        //frmScan.Width = this.ClientRectangle.Width;
                        //frmScan.Show();
                        frmBookScanning bkScan = new frmBookScanning(tmpBox, sqlCon, crd);
                        bkScan.MdiParent = this;
                        bkScan.Height = this.ClientRectangle.Height;
                        bkScan.Width = this.ClientRectangle.Width;
                        bkScan.Show();
                    }
                }
            }
        }

        void LICToolStripMenuItemClick(object sender, EventArgs e)
        {
            //Form activeChild=this.ActiveMdiChild;
            //if(activeChild==null)
            //{
            aeLicQa frmLicQA = new aeLicQa(sqlCon, crd);
            //frmLicQA.MdiParent=this;
            frmLicQA.Height = this.ClientRectangle.Height;
            frmLicQA.Width = this.ClientRectangle.Width;
            frmLicQA.ShowDialog(this);
            //}
        }

        void ExpertQualityControlCentreToolStripMenuItemClick(object sender, EventArgs e)
        {
            NovaNet.wfe.eSTATES[] state = new NovaNet.wfe.eSTATES[9];
            state[0] = NovaNet.wfe.eSTATES.POLICY_EXCEPTION;
            state[1] = NovaNet.wfe.eSTATES.POLICY_INDEXED;
            state[2] = NovaNet.wfe.eSTATES.POLICY_FQC;
            state[3] = NovaNet.wfe.eSTATES.POLICY_ON_HOLD;
            state[4] = NovaNet.wfe.eSTATES.POLICY_CHECKED;
            state[5] = NovaNet.wfe.eSTATES.POLICY_NOT_INDEXED;
            state[6] = NovaNet.wfe.eSTATES.POLICY_EXPORTED;
            state[7] = NovaNet.wfe.eSTATES.POLICY_QC;
            state[8] = NovaNet.wfe.eSTATES.POLICY_SUBMITTED;
            aeBoxSelection box = new aeBoxSelection(state, sqlCon);
            wfeBox tmpBox = null;
            wi = null;
            box.ShowDialog(this);
            tmpBox = (wfeBox)wi;
            if (tmpBox != null)
            {
                if ((tmpBox.ctrlBox.ProjectCode.ToString() != null) && (tmpBox.ctrlBox.BatchKey.ToString() != null) && (tmpBox.ctrlBox.BoxNumber.ToString() != null))
                {
                    //wfeBox tmpBox = (wfeBox)wi;
                    Form activeChild = this.ActiveMdiChild;
                    if (activeChild == null)
                    {
                        aeFQC frmFQC = new aeFQC(tmpBox, sqlCon, crd);
                        //frmFQC.MdiParent=this;
                        frmFQC.Height = this.ClientRectangle.Height;
                        frmFQC.Width = this.ClientRectangle.Width;
                        frmFQC.ShowDialog(this);
                    }
                }
            }
        }

        void ExportToolStripMenuItemClick(object sender, EventArgs e)
        {
            aeExport export = new aeExport(sqlCon, crd);
            export.ShowDialog(this);
        }
        void reExportToolStripMenuItemClick(object sender, EventArgs e)
        {
            frmSingleExp exp = new frmSingleExp(sqlCon, crd);
            exp.ShowDialog(this);
        }
        void ToolStripButton1Click(object sender, EventArgs e)
        {
            eSTATES[] state = new eSTATES[1];
            state[0] = eSTATES.POLICY_CREATED;
            aeBoxSelection box = new aeBoxSelection(state, sqlCon);
            box.chkPhotoScan.Visible = true;
            wfeBox tmpBox = null;
            wi = null;
            box.ShowDialog(this);
            tmpBox = (wfeBox)wi;
            if (tmpBox != null)
            {
                if ((tmpBox.ctrlBox.ProjectCode.ToString() != null) && (tmpBox.ctrlBox.BatchKey.ToString() != null) && (tmpBox.ctrlBox.BoxNumber.ToString() == null))
                {
                    //wfeBox tmpBox = (wfeBox)wi;
                    Form activeChild = this.ActiveMdiChild;
                    if (activeChild == null)
                    {
                        aePolicyScan frmScan = new aePolicyScan(tmpBox, sqlCon, crd, colorMode);
                        //frmScan.MdiParent = this;
                        //frmScan.Height = this.ClientRectangle.Height;
                        //frmScan.Width = this.ClientRectangle.Width;
                        frmScan.Show();
                    }
                }
            }
        }

        void ToolStripButton3Click(object sender, EventArgs e)
        {
            eSTATES[] state = new eSTATES[1];
            state[0] = eSTATES.POLICY_SCANNED;

            aeBoxSelection box = new aeBoxSelection(state, sqlCon);
            //mainForm=new frmMain();
            wi = null;
            box.ShowDialog(this);
            wfeBox tmpBox = (wfeBox)wi;
            if (tmpBox != null)
            {
                if ((tmpBox.ctrlBox.ProjectCode.ToString() != null) && (tmpBox.ctrlBox.BatchKey.ToString() != null) && (tmpBox.ctrlBox.BoxNumber.ToString() != null))
                {
                    Form activeChild = this.ActiveMdiChild;
                    if (activeChild == null)
                    {
                        aeImageQC frmQc = new aeImageQC(tmpBox, sqlCon, crd);
                        frmQc.MdiParent = this;
                        frmQc.Height = this.ClientRectangle.Height;
                        frmQc.Width = this.ClientRectangle.Width;
                        frmQc.Show();
                    }
                }
            }
        }

        void ToolStripButton2Click(object sender, EventArgs e)
        {
            eSTATES[] state = new eSTATES[1];
            state[0] = eSTATES.POLICY_QC;

            aeBoxSelection box = new aeBoxSelection(state, sqlCon);
            wfeBox tmpBox = null;
            wi = null;
            box.ShowDialog(this);
            tmpBox = (wfeBox)wi;
            if (tmpBox != null)
            {
                if ((tmpBox.ctrlBox.ProjectCode.ToString() != null) && (tmpBox.ctrlBox.BatchKey.ToString() != null) && (tmpBox.ctrlBox.BoxNumber.ToString() != null))
                {
                    //wfeBox tmpBox = (wfeBox)wi;
                    Form activeChild = this.ActiveMdiChild;
                    if (activeChild == null)
                    {
                        aeIndexing frmIndex = new aeIndexing(tmpBox, sqlCon, crd);
                        frmIndex.MdiParent = this;
                        frmIndex.Height = this.ClientRectangle.Height;
                        frmIndex.Width = this.ClientRectangle.Width;
                        frmIndex.Show();
                    }
                }
            }
        }

        void ToolStripButton4Click(object sender, EventArgs e)
        {
            NovaNet.wfe.eSTATES[] state = new NovaNet.wfe.eSTATES[7];
            state[0] = NovaNet.wfe.eSTATES.POLICY_EXCEPTION;
            state[1] = NovaNet.wfe.eSTATES.POLICY_INDEXED;
            state[2] = NovaNet.wfe.eSTATES.POLICY_FQC;
            state[3] = NovaNet.wfe.eSTATES.POLICY_ON_HOLD;
            state[4] = NovaNet.wfe.eSTATES.POLICY_CHECKED;
            state[5] = NovaNet.wfe.eSTATES.POLICY_NOT_INDEXED;
            state[6] = NovaNet.wfe.eSTATES.POLICY_EXPORTED;
            aeBoxSelection box = new aeBoxSelection(state, sqlCon);
            wfeBox tmpBox = null;
            wi = null;
            box.ShowDialog(this);
            tmpBox = (wfeBox)wi;
            if (tmpBox != null)
            {
                if ((tmpBox.ctrlBox.ProjectCode.ToString() != null) && (tmpBox.ctrlBox.BatchKey.ToString() != null) && (tmpBox.ctrlBox.BoxNumber.ToString() != null))
                {
                    //wfeBox tmpBox = (wfeBox)wi;
                    Form activeChild = this.ActiveMdiChild;
                    if (activeChild == null)
                    {
                        aeFQC frmFQC = new aeFQC(tmpBox, sqlCon, crd);
                        //frmFQC.MdiParent=this;
                        frmFQC.Height = this.ClientRectangle.Height;
                        frmFQC.Width = this.ClientRectangle.Width;
                        frmFQC.Visible = false;
                        frmFQC.ShowDialog(this);
                    }
                }
            }
        }

        private void mnuJobCreation_Click(object sender, EventArgs e)
        {
            aeJobCreation jobCrt = new aeJobCreation(sqlCon, crd);
            jobCrt.ShowDialog(this);
        }

        private void changePasswordToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PwdChange pwdCh = new PwdChange(ref p, getCPwd);
            pwdCh.ShowDialog(this);
        }

        private void newUserToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddNewUser nwUsr = new AddNewUser(getnwusrData, sqlCon);
            nwUsr.ShowDialog(this);
        }
        // Used for password change
        void getCPwd(ref NovaNet.Utils.Profile prmpwd)
        {
            p = prmpwd;
            rbc.changePassword(p.UserId, p.UserName, p.Password);
        }
        // Used for add new user
        void getnwusrData(ref NovaNet.Utils.Profile prmp)
        {
            p = prmp;
            if (rbc.addUser(p.UserId, p.UserName, p.Role_des, p.Password) == false)
            {
                AddNewUser nwUsr = new AddNewUser(getnwusrData, sqlCon);
                nwUsr.ShowDialog(this);
            }
        }

        private void uATToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmSelection frmSel = new frmSelection(sqlCon, "UATAnxA");
            frmSel.ShowDialog(this);
        }

        private void batchSummeryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmSelection frmSel = new frmSelection(sqlCon, "BatchSummary");
            frmSel.ShowDialog(this);
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            About frmSel = new About();
            frmSel.ShowDialog(this);
        }

        private void boxSummaryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmJobDistribution frmJob = new frmJobDistribution(sqlCon);
            frmJob.Show(this);
        }

        private void userManualToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start("B'Zer.pdf");
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "Error While Opening User Manual (PDF Format)\n" + ex.Message, "User Manual", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

            }

        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void frmMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (p.UserId != string.Empty)
            {
                rbc.UnLockedUser(p.UserId);
                Application.Exit();
            }
        }

        private void toolOnlineUser_Click(object sender, EventArgs e)
        {
            frmLoggedUser loged = new frmLoggedUser(rbc, crd);
            loged.ShowDialog(this);
        }

        private void batchScanADFToolStripMenuItem_Click(object sender, EventArgs e)
        {
            eSTATES[] state = new eSTATES[1];
            state[0] = eSTATES.POLICY_CREATED;
            aeBoxSelection box = new aeBoxSelection(state, sqlCon);
            box.chkPhotoScan.Visible = true;
            wfeBox tmpBox = null;
            wi = null;
            box.ShowDialog(this);
            tmpBox = (wfeBox)wi;
            if (tmpBox != null)
            {
                if ((tmpBox.ctrlBox.ProjectCode.ToString() != null) && (tmpBox.ctrlBox.BatchKey.ToString() != null) && (tmpBox.ctrlBox.BoxNumber.ToString() != null))
                {
                    //wfeBox tmpBox = (wfeBox)wi;
                    Form activeChild = this.ActiveMdiChild;
                    if (activeChild == null)
                    {
                        aePolicyScan frmScan = new aePolicyScan(tmpBox, sqlCon, crd, colorMode);
                        frmScan.MdiParent = this;
                        frmScan.Height = this.ClientRectangle.Height;
                        frmScan.Width = this.ClientRectangle.Width;
                        frmScan.Show();
                        //frmBookScanning bkScan = new frmBookScanning(tmpBox, sqlCon, crd);
                        //bkScan.MdiParent = this;
                        //bkScan.Height = this.ClientRectangle.Height;
                        //bkScan.Width = this.ClientRectangle.Width;
                        //bkScan.Show();
                    }
                }
            }
        }

        private void transferredDataToolStripMenuItem_Click(object sender, EventArgs e)
        {

            //dt.ShowDialog(this);
            Form activeChild = this.ActiveMdiChild;
            if (activeChild == null)
            {
                frmTransferredData dt = new frmTransferredData(sqlCon);
                dt.MdiParent = this;
                dt.Height = this.ClientRectangle.Height;
                dt.Width = this.ClientRectangle.Width;
                dt.Show();
                //frmBookScanning bkScan = new frmBookScanning(tmpBox, sqlCon, crd);
                //bkScan.MdiParent = this;
                //bkScan.Height = this.ClientRectangle.Height;
                //bkScan.Width = this.ClientRectangle.Width;
                //bkScan.Show();
            }
        }

        private void genericReportsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                TestReportViewer tstViewer = new TestReportViewer(sqlCon);
                //tstViewer.MdiParent = this;
                //tstViewer.Height = this.ClientRectangle.Height;
                //tstViewer.Width = this.ClientRectangle.Width;
                tstViewer.ShowDialog(this);
            }
            catch
            {
                MessageBox.Show("You need to install ODBC driver 5.1 to see this reports.....");
            }
        }

        private void uATReportToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void batchWiseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmUatReport uat = new frmUatReport(sqlCon, crd);
            uat.Show(this);
        }

        private void projectWiseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmUatReportProject uat = new frmUatReportProject(sqlCon, crd);
            uat.Show(this);
        }

        private void burnCDToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MainForm main = new MainForm(sqlCon);
            main.Show(this);
        }

        private void volumeSubmitedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmSubmit frms = new frmSubmit(sqlCon, crd);
            frms.Show(this);
        }

        private void dataEntryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            IGRFqc.frmVolume frmchild = new frmVolume(sqlCon, (NovaNet.Utils.Credentials)crd);
            frmchild.Show(this);
        }

        private void importImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmDataImport data = new frmDataImport(sqlCon, crd);
            data.ShowDialog(this);
        }

        private void systemConfigToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmsysCon fsc = new frmsysCon(sqlCon, crd);
            fsc.ShowDialog(this);
        }

        private void renameBatchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmBatchRename batRem = new frmBatchRename(sqlCon, crd);
            batRem.ShowDialog(this);
        }
        private Boolean databaseImport()
        {
            bool flag = false;
            OdbcCommand cmd = new OdbcCommand();
            OdbcDataAdapter sqlAdap = new OdbcDataAdapter();
            OdbcTransaction trans = null;
            DataSet ds = new DataSet();
            string sql = string.Empty;
            try
            {

                sql = "SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = '" + sqlCon.Database + "' AND TABLE_NAME = 'tblexception'";
                sqlAdap = new OdbcDataAdapter(sql, sqlCon);
                sqlAdap.Fill(ds);
                if (ds.Tables[0].Rows.Count == 0)
                {
                    sql = "CREATE TABLE `tblexception` (`ExcepTion_Code` varchar(10) NOT NULL,`ExcepTion_Name` varchar(50) DEFAULT NULL,PRIMARY KEY (`ExcepTion_Code`))  ";
                    cmd.CommandText = sql;
                    cmd.Connection = sqlCon;
                    cmd.ExecuteNonQuery();
                }
                ds.Clear();
                sql = "SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = '" + sqlCon.Database + "' AND TABLE_NAME = 'country_master'";
                sqlAdap = new OdbcDataAdapter(sql, sqlCon);
                sqlAdap.Fill(ds);
                if (ds.Tables[0].Rows.Count == 0)
                {
                    sql = "CREATE TABLE `country_master`(`Cou_Code` varchar(10) NOT NULL,`Cou_Name` varchar(50) DEFAULT NULL,`Office_code` varchar(50) DEFAULT NULL,`isManualEntry` varchar(50) DEFAULT NULL,PRIMARY KEY (`Cou_Code`))";
                    cmd.CommandText = sql;
                    cmd.Connection = sqlCon;
                    cmd.ExecuteNonQuery();
                }
                ds.Clear();
                sql = "SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = '" + sqlCon.Database + "' AND TABLE_NAME = 'deed_details_exception'";
                sqlAdap = new OdbcDataAdapter(sql, sqlCon);
                sqlAdap.Fill(ds);
                if (ds.Tables[0].Rows.Count == 0)
                {
                    sql = "CREATE TABLE `deed_details_exception` (`District_code` varchar(10) NOT NULL,`RO_Code` varchar(10) NOT NULL,`Book` varchar(10) NOT NULL,`Deed_year` varchar(10) NOT NULL,`Deed_no` varchar(10) NOT NULL,`srl_no` int(10) NOT NULL,`Exception` varchar(25) DEFAULT NULL,`Details` varchar(50) DEFAULT NULL,PRIMARY KEY (`District_code`,`RO_Code`,`Book`,`Deed_year`,`Deed_no`,`srl_no`))";
                    cmd.CommandText = sql;
                    cmd.Connection = sqlCon;
                    cmd.ExecuteNonQuery();
                }
                ds.Clear();
                sql = "SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = '" + sqlCon.Database + "' AND TABLE_NAME = 'district_master'";
                sqlAdap = new OdbcDataAdapter(sql, sqlCon);
                sqlAdap.Fill(ds);
                if (ds.Tables[0].Rows.Count == 0)
                {
                    sql = "CREATE TABLE `district_master` (`Con_code` varchar(10) NOT NULL,`State_code` varchar(10) NOT NULL,`dis_code` varchar(10) NOT NULL,`Dis_name` varchar(50) DEFAULT NULL,`is_active` varchar(10) DEFAULT NULL,`isManualEntry` varchar(10) DEFAULT NULL,PRIMARY KEY (`Con_code`,`State_code`,`dis_code`))";
                    cmd.CommandText = sql;
                    cmd.Connection = sqlCon;
                    cmd.ExecuteNonQuery();
                }
                ds.Clear();
                sql = "SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = '" + sqlCon.Database + "' AND TABLE_NAME = 'index_of_name_exception'";
                sqlAdap = new OdbcDataAdapter(sql, sqlCon);
                sqlAdap.Fill(ds);
                if (ds.Tables[0].Rows.Count == 0)
                {
                    sql = "CREATE TABLE `index_of_name_exception` (`District_code` varchar(10) NOT NULL,`RO_Code` varchar(10) NOT NULL,`book` varchar(10) NOT NULL,`Deed_year` varchar(10) NOT NULL,`Deed_no` varchar(10) NOT NULL,`item_no` int(10) NOT NULL,`srl_no` int(10) DEFAULT NULL,`Exception` varchar(25) DEFAULT NULL,`Details` varchar(50) DEFAULT NULL,PRIMARY KEY (`District_code`,`RO_Code`,`book`,`Deed_year`,`Deed_no`,`item_no`))";
                    cmd.CommandText = sql;
                    cmd.Connection = sqlCon;
                    cmd.ExecuteNonQuery();
                }
                ds.Clear();
                sql = "SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = '" + sqlCon.Database + "' AND TABLE_NAME = 'index_of_property_exception'";
                sqlAdap = new OdbcDataAdapter(sql, sqlCon);
                sqlAdap.Fill(ds);
                if (ds.Tables[0].Rows.Count == 0)
                {
                    sql = "CREATE TABLE `index_of_property_exception` ( `District_code` varchar(10) NOT NULL,`RO_Code` varchar(10) NOT NULL,`book` varchar(10) NOT NULL,`Deed_year` varchar(10) NOT NULL,`Deed_no` varchar(10) NOT NULL,`item_no` int(10) NOT NULL,`srl_no` int(10) NOT NULL,`Exception` varchar(25) DEFAULT NULL,`Details` varchar(50) DEFAULT NULL,PRIMARY KEY (`District_code`,`RO_Code`,`book`,`Deed_year`,`Deed_no`,`item_no`,`srl_no`))";
                    cmd.CommandText = sql;
                    cmd.Connection = sqlCon;
                    cmd.ExecuteNonQuery();
                }
                ds.Clear();
                sql = "SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = '" + sqlCon.Database + "' AND TABLE_NAME = 'index_of_property_out_wb'";
                sqlAdap = new OdbcDataAdapter(sql, sqlCon);
                sqlAdap.Fill(ds);
                if (ds.Tables[0].Rows.Count == 0)
                {
                    sql = "CREATE TABLE `index_of_property_out_wb` (`District_Code` varchar(50) NOT NULL,`RO_Code` varchar(50) NOT NULL,`Book` varchar(50) NOT NULL,`Deed_year` varchar(50) NOT NULL,`Deed_no` varchar(50) NOT NULL,`Item_no` varchar(50) NOT NULL,`property_country_code` varchar(10) DEFAULT NULL,`property_state_code` varchar(10) DEFAULT NULL,`Property_district_code` varchar(50) DEFAULT NULL,`thana` varchar(50) DEFAULT NULL,`moucode` varchar(50) DEFAULT NULL,`Plot_code_type` varchar(50) DEFAULT NULL,`Plot_No` varchar(50) DEFAULT NULL,`Khatian_type` varchar(50) DEFAULT NULL,`khatian_No` varchar(50) DEFAULT NULL,`land_use` varchar(50) DEFAULT NULL,`property_type` varchar(50) DEFAULT NULL,`Created_DTTM` varchar(50) DEFAULT NULL,`Created_by` varchar(25) DEFAULT NULL,`local_body_type` varchar(10) DEFAULT NULL,`other_details` varchar(255) DEFAULT NULL,`area_bigha` varchar(50) DEFAULT NULL,`area_decimal` varchar(50) DEFAULT NULL,`area_katha` varchar(50) DEFAULT NULL,`area_chatak` varchar(50) DEFAULT NULL,`area_sqf` varchar(50) DEFAULT NULL,`area_sqfeet` varchar(50) DEFAULT NULL,`total_area_decimal` varchar(50) DEFAULT NULL,`struct_sqfeet` varchar(50) DEFAULT NULL,`area_acre` varchar(50) DEFAULT NULL,PRIMARY KEY (`District_Code`,`RO_Code`,`Book`,`Deed_year`,`Deed_no`,`Item_no`))  ";
                    // sql = "CREATE TABLE `index_of_property_out_wb` (`District_Code` varchar(50) NOT NULL,`RO_Code` varchar(50) NOT NULL,`Book` varchar(50) NOT NULL,`Deed_year` varchar(50) NOT NULL,`Deed_no` varchar(50) NOT NULL,`Item_no` varchar(50) NOT NULL,`property_country_code` varchar(10) DEFAULT NULL,`property_state_code` varchar(10) DEFAULT NULL,`Property_district_code` varchar(50) DEFAULT NULL,`thana` varchar(50) DEFAULT NULL,`moucode` varchar(50) DEFAULT NULL,`Plot_code_type` varchar(50) DEFAULT NULL,`Plot_No` varchar(50) DEFAULT NULL,`Khatian_type` varchar(50) DEFAULT NULL,`khatian_No` varchar(50) DEFAULT NULL,`land_use` varchar(50) DEFAULT NULL,`property_type` varchar(50) DEFAULT NULL,`Area` varchar(50) NOT NULL,`Created_DTTM` varchar(50) DEFAULT NULL,`Created_by` varchar(25) DEFAULT NULL,`local_body_type` varchar(10) DEFAULT NULL,`other_details` varchar(255) DEFAULT NULL,PRIMARY KEY (`District_Code`,`RO_Code`,`Book`,`Deed_year`,`Deed_no`,`Item_no`))";
                    cmd.CommandText = sql;
                    cmd.Connection = sqlCon;
                    cmd.ExecuteNonQuery();
                }
                ds.Clear();
                sql = "SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = '" + sqlCon.Database + "' AND TABLE_NAME = 'state_master'";
                sqlAdap = new OdbcDataAdapter(sql, sqlCon);
                sqlAdap.Fill(ds);
                if (ds.Tables[0].Rows.Count == 0)
                {
                    sql = "CREATE TABLE `state_master` (`Con_code` varchar(10) NOT NULL,`State_code` varchar(10) NOT NULL,`State_name` varchar(50) DEFAULT NULL,`isManualEntry` varchar(50) DEFAULT NULL,PRIMARY KEY (`Con_code`,`State_code`))";
                    cmd.CommandText = sql;
                    cmd.Connection = sqlCon;
                    cmd.ExecuteNonQuery();
                }
                ds.Clear();
                sql = "SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = '" + sqlCon.Database + "' AND TABLE_NAME = 'tbloutsidewblist'";
                sqlAdap = new OdbcDataAdapter(sql, sqlCon);
                sqlAdap.Fill(ds);
                if (ds.Tables[0].Rows.Count == 0)
                {
                    sql = "CREATE TABLE `tbloutsidewblist` (`District_Code` varchar(10) DEFAULT NULL,`RO_Code` varchar(10) DEFAULT NULL,`Book` varchar(10) DEFAULT NULL,`deed_year` varchar(10) DEFAULT NULL,`Deed_no` varchar(10) DEFAULT NULL,`item_no` varchar(10) DEFAULT NULL,`isOutsideWB` varchar(10) DEFAULT NULL)";
                    cmd.CommandText = sql;
                    cmd.Connection = sqlCon;
                    cmd.ExecuteNonQuery();
                }
                ds.Clear();
                sql = "SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = '" + sqlCon.Database + "' AND TABLE_NAME = 'tbllot_media_mapping'";
                sqlAdap = new OdbcDataAdapter(sql, sqlCon);
                sqlAdap.Fill(ds);
                if (ds.Tables[0].Rows.Count == 0)
                {
                    sql = "CREATE TABLE `tbllot_media_mapping` (`id` int(11) NOT NULL AUTO_INCREMENT,`lot_no` varchar(25) DEFAULT NULL,`media_no` varchar(50) DEFAULT NULL,PRIMARY KEY (`id`))";
                    cmd.CommandText = sql;
                    cmd.Connection = sqlCon;
                    cmd.ExecuteNonQuery();
                }

                sql = "alter table ps modify ps_code varchar(3)";
                cmd.CommandText = sql;
                cmd.Connection = sqlCon;
                cmd.ExecuteNonQuery();
                flag = true;
            }
            catch (Exception ex)
            {
                sql = "SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = '" + sqlCon.Database + "' AND TABLE_NAME = 'tblexception'";
                sqlAdap = new OdbcDataAdapter(sql, sqlCon);
                sqlAdap.Fill(ds);
                if (ds.Tables[0].Rows.Count == 0)
                {
                    sql = "DROP TABLE `tblexception`";
                    cmd.CommandText = sql;
                    cmd.Connection = sqlCon;
                    cmd.ExecuteNonQuery();
                }
                ds.Clear();
                sql = "SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = '" + sqlCon.Database + "' AND TABLE_NAME = 'country_master'";
                sqlAdap = new OdbcDataAdapter(sql, sqlCon);
                sqlAdap.Fill(ds);
                if (ds.Tables[0].Rows.Count == 0)
                {
                    sql = "DROP TABLE `country_master`";
                    cmd.CommandText = sql;
                    cmd.Connection = sqlCon;
                    cmd.ExecuteNonQuery();
                }
                ds.Clear();
                sql = "SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = '" + sqlCon.Database + "' AND TABLE_NAME = 'deed_details_exception'";
                sqlAdap = new OdbcDataAdapter(sql, sqlCon);
                sqlAdap.Fill(ds);
                if (ds.Tables[0].Rows.Count == 0)
                {
                    sql = "DROP TABLE `deed_details_exception`";
                    cmd.CommandText = sql;
                    cmd.Connection = sqlCon;
                    cmd.ExecuteNonQuery();
                }
                ds.Clear();
                sql = "SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = '" + sqlCon.Database + "' AND TABLE_NAME = 'district_master'";
                sqlAdap = new OdbcDataAdapter(sql, sqlCon);
                sqlAdap.Fill(ds);
                if (ds.Tables[0].Rows.Count == 0)
                {
                    sql = "DROP TABLE `district_master`";
                    cmd.CommandText = sql;
                    cmd.Connection = sqlCon;
                    cmd.ExecuteNonQuery();
                }
                ds.Clear();
                sql = "SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = '" + sqlCon.Database + "' AND TABLE_NAME = 'index_of_name_exception'";
                sqlAdap = new OdbcDataAdapter(sql, sqlCon);
                sqlAdap.Fill(ds);
                if (ds.Tables[0].Rows.Count == 0)
                {
                    sql = "DROP TABLE `index_of_name_exception`";
                    cmd.CommandText = sql;
                    cmd.Connection = sqlCon;
                    cmd.ExecuteNonQuery();
                }
                ds.Clear();
                sql = "SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = '" + sqlCon.Database + "' AND TABLE_NAME = 'index_of_property_exception'";
                sqlAdap = new OdbcDataAdapter(sql, sqlCon);
                sqlAdap.Fill(ds);
                if (ds.Tables[0].Rows.Count == 0)
                {
                    sql = "DROP TABLE `index_of_property_exception`";
                    cmd.CommandText = sql;
                    cmd.Connection = sqlCon;
                    cmd.ExecuteNonQuery();
                }
                ds.Clear();
                sql = "SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = '" + sqlCon.Database + "' AND TABLE_NAME = 'index_of_property_out_wb'";
                sqlAdap = new OdbcDataAdapter(sql, sqlCon);
                sqlAdap.Fill(ds);
                if (ds.Tables[0].Rows.Count == 0)
                {
                    sql = "DROP TABLE `index_of_property_out_wb`";
                    cmd.CommandText = sql;
                    cmd.Connection = sqlCon;
                    cmd.ExecuteNonQuery();
                }
                ds.Clear();
                sql = "SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = '" + sqlCon.Database + "' AND TABLE_NAME = 'state_master'";
                sqlAdap = new OdbcDataAdapter(sql, sqlCon);
                sqlAdap.Fill(ds);
                if (ds.Tables[0].Rows.Count == 0)
                {
                    sql = "DROP TABLE `state_master`";
                    cmd.CommandText = sql;
                    cmd.Connection = sqlCon;
                    cmd.ExecuteNonQuery();
                }
                ds.Clear();
                sql = "SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = '" + sqlCon.Database + "' AND TABLE_NAME = 'tbloutsidewblist'";
                sqlAdap = new OdbcDataAdapter(sql, sqlCon);
                sqlAdap.Fill(ds);
                if (ds.Tables[0].Rows.Count == 0)
                {
                    sql = "DROP TABLE `tbloutsidewblist`";
                    cmd.CommandText = sql;
                    cmd.Connection = sqlCon;
                    cmd.ExecuteNonQuery();
                }
            }
            return flag;
        }
        private Boolean InsertDatainDb()
        {
            bool flag = false;
            DataSet ds = new DataSet();

            OdbcDataAdapter oAdptr = new OdbcDataAdapter();
            OdbcTransaction trans = null;
            string sqlStr = string.Empty;
            string dbVersion = string.Empty;
            string[] csvName;
            try
            {

                sqlStr = "select sysvalues from sysconfig where syskeys = 'DB_VERSION'";
                oAdptr = new OdbcDataAdapter(sqlStr, sqlCon);
                oAdptr.Fill(ds);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    dbVersion = ds.Tables[0].Rows[0][0].ToString();
                }
                if (dbVersion == "4")
                {
                    trans = sqlCon.BeginTransaction();
                    if (csvFileCheck())
                    {
                        csvName = Directory.GetFiles(Application.StartupPath + "\\csv\\", "*");
                        for (int i = 0; i < csvName.Length; i++)
                        {
                            DataTable dt = new DataTable();
                            string fileName = Path.GetFileName(csvName[i]).ToString();
                            int index = fileName.IndexOf('.');
                            string tableName = fileName.Substring(0, index);
                            if (index > 0)
                            {
                                dt = GetDataTableFromCsv(csvName[i], false);
                                insertData(dt, tableName, trans);
                            }
                        }
                    }
                    updateDBversion(trans);
                    trans.Commit();
                }

            }
            catch (Exception ex)
            {
                flag = false;
                trans.Rollback();
            }
            return flag;
        }
        private Boolean csvFileCheck()
        {
            bool flag = true;
            try
            {
                if (!File.Exists(Application.StartupPath + "\\csv\\district.csv"))
                {
                    flag = false;
                }
                if (!File.Exists(Application.StartupPath + "\\csv\\ro_master.csv"))
                {
                    flag = false;
                }
                if (!File.Exists(Application.StartupPath + "\\csv\\country_master.csv"))
                {
                    flag = false;
                }
                if (!File.Exists(Application.StartupPath + "\\csv\\district_master.csv"))
                {
                    flag = false;
                }
                if (!File.Exists(Application.StartupPath + "\\csv\\state_master.csv"))
                {
                    flag = false;
                }
                if (!File.Exists(Application.StartupPath + "\\csv\\tblexception.csv"))
                {
                    flag = false;
                }
                if (!File.Exists(Application.StartupPath + "\\csv\\ps.csv"))
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
        private void updateDBversion(OdbcTransaction trans)
        {
            OdbcCommand sqlCmd = new OdbcCommand();
            string sql = "update sysconfig set sysvalues = '11' where syskeys = 'DB_VERSION'";
            sqlCmd.Connection = sqlCon;
            sqlCmd.CommandText = sql;
            sqlCmd.Transaction = trans;
            sqlCmd.ExecuteNonQuery();
        }
        static DataTable GetDataTableFromCsv(string path, bool isFirstRowHeader)
        {
            string header = isFirstRowHeader ? "Yes" : "No";
            //string header = "No";    
            string pathOnly = Path.GetDirectoryName(path);
            string fileName = Path.GetFileName(path);

            string sql = @"SELECT * FROM [" + fileName + "]";

            using (OleDbConnection connection = new OleDbConnection(
                  @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + pathOnly +
                  ";Extended Properties=\"Text;HDR=" + header + "\""))
            using (OleDbCommand command = new OleDbCommand(sql, connection))
            using (OleDbDataAdapter adapter = new OleDbDataAdapter(command))
            {
                DataTable dataTable = new DataTable();
                //dataTable.Locale = CultureInfo.CurrentCulture;
                adapter.Fill(dataTable);
                return dataTable;
            }
        }
        private Boolean insertData(DataTable dt, string tableName, OdbcTransaction trans)
        {
            string sql = null;
            bool flag = false;
            try
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {

                    string sqlpre = null;
                    sqlpre = "insert into " + tableName + " values(";
                    for (int j = 0; j < dt.Columns.Count; j++)
                    {

                        //sql += "'+dt.Rows[" + i + "][ " + j + "]".ToString()+"',";
                        sql = sql + "'" + dt.Rows[i][j].ToString() + "'" + ",";
                    }
                    sqlpre += sql.TrimEnd(',') + ")";

                    sql = null;
                    OdbcCommand SqlCom = new OdbcCommand(sqlpre, sqlCon, trans);
                    //SqlCom.Transaction = trans;
                    SqlCom.ExecuteNonQuery();
                }
                flag = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(sql + ex.ToString());
            }
            return flag;

        }

        private void reportsToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void genericReportsBookIVToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                TestReportViewer4 tstViewer = new TestReportViewer4(sqlCon);
                //tstViewer.MdiParent = this;
                //tstViewer.Height = this.ClientRectangle.Height;
                //tstViewer.Width = this.ClientRectangle.Width;
                tstViewer.ShowDialog(this);
            }
            catch
            {
                MessageBox.Show("You need to install ODBC driver 5.1 to see this reports.....");
            }
        }

        private void logoutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Text = null;
            sqlCon.Close();


            sqlCon.Open();
            logoutToolStripMenuItem.Visible = true;
            logoutToolStripMenuItem.Enabled = true;
            itemsToolStripMenuItem.Visible = false;
            transactinToolStripMenuItem.Visible = false;
            toolStripMenuItem2.Visible = false;
            toolsToolStripMenuItem.Visible = false;
            reportsToolStripMenuItem.Visible = false;
            helpToolStripMenuItem.Visible = false;


            FrmMainLoad(sender, e);


        }

        private void deedWiseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmNewReport frmn = new frmNewReport(sqlCon);
            frmn.ShowDialog();
        }

        private void l1DeedReportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            L1DeedReport frm = new L1DeedReport(sqlCon);
            frm.ShowDialog();
        }

        private void toolStripMenuItem3_Click(object sender, EventArgs e)
        {
            try
            {
                TestReportViewer3 tstViewer = new TestReportViewer3(sqlCon);
                //tstViewer.MdiParent = this;
                //tstViewer.Height = this.ClientRectangle.Height;
                //tstViewer.Width = this.ClientRectangle.Width;
                tstViewer.ShowDialog(this);
            }
            catch
            {
                MessageBox.Show("You need to install ODBC driver 5.1 to see this reports.....");
            }
        }
    }
	
}