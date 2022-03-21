using System;
using System.Drawing;
using System.Windows.Forms;
using NovaNet.Utils;
using NovaNet.wfe;
using LItems;
using System.Data;
using System.Data.Odbc;
using System.Collections;
using System.IO;
using System.Collections.Generic;

namespace ImageHeaven
{
    public partial class frmDataImport : Form
    {
        MemoryStream stateLog;
        byte[] tmpWrite;
        NovaNet.Utils.dbCon dbcon;
        int pos = 0;
        int posAdd = 0;
        OdbcConnection sqlCon = null;
        eSTATES[] state;
        wfeProject tmpProj = null;
        DataSet ds = null;
        private double ZOOMFACTOR = 1.10;   // = 25% smaller or larger
        private int MINMAX = 5;
        Point mouseDown = new Point();
        private Size ImageSize = new Size(100, 200);
        Credentials crd = new Credentials();
        //OdbcTransaction sqlTrans = null;
        private Dictionary<string, ListViewItem> ListViewItems = new Dictionary<string, ListViewItem>();
        private Dictionary<string, ListViewItem> ListViewItems1 = new Dictionary<string, ListViewItem>();
        public frmDataImport(OdbcConnection prmCon, Credentials prmCrd)
        {
            dbcon = new NovaNet.Utils.dbCon();
            sqlCon = prmCon;
            crd = prmCrd;
            InitializeComponent();
            tabControl1.TabPages.Remove(tabPage3);
            
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }
        private void KeyEvent(object sender, KeyEventArgs e) //Keyup Event 
        {
            if (e.KeyCode == Keys.Add)
            {
                cmdAdd_Click(this, e);
            }
            if (e.KeyCode == Keys.Subtract)
            {
                cmdRemove_Click(this, e);
            }
            if (e.KeyCode == Keys.F5)
            {
                cmdImport_Click(this, e);
            }

        }
        private void frmDataImport_Load(object sender, EventArgs e)
        {
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(KeyEvent);
            PopulateProjectCombo();
        }
        private void PopulateProjectCombo()
        {
            DataSet ds = new DataSet();

            tmpProj = new wfeProject(sqlCon);
            //cmbProject.Items.Add("Select");
            ds = tmpProj.GetAllValues();
            cmbProject.DataSource = ds.Tables[0];
            cmbProject.DisplayMember = ds.Tables[0].Columns[1].ToString();
            cmbProject.ValueMember = ds.Tables[0].Columns[0].ToString();
        }

        private void cmbProject_Leave(object sender, EventArgs e)
        {
            PopulateBatchCombo();
        }
        private void PopulateBatchCombo()
        {
            string projKey = null;
            DataSet ds = new DataSet();

            dbcon = new NovaNet.Utils.dbCon();

            wfeBatch tmpBatch = new wfeBatch(sqlCon);
            if (cmbProject.SelectedValue != null)
            {
                projKey = cmbProject.SelectedValue.ToString();
                ds = tmpBatch.GetAllValues(Convert.ToInt32(projKey));
                //batchCount = ds.Tables[0].Rows.Count;
                cmbBatch.DataSource = ds.Tables[0];
                cmbBatch.DisplayMember = ds.Tables[0].Columns[1].ToString();
                cmbBatch.ValueMember = ds.Tables[0].Columns[0].ToString();
            }
        }

        private void cmbBatch_Leave(object sender, EventArgs e)
        {
            PopulateBox();
        }
        private void PopulateBox()
        {
            string batchKey = null;
            DataSet ds = new DataSet();
            

            dbcon = new NovaNet.Utils.dbCon();

            int policyCount;

            if (cmbBatch.SelectedValue != null)
            {
                batchKey = cmbBatch.SelectedValue.ToString();
                //CtrlPolicy pPolicy = new CtrlPolicy(Convert.ToInt32(cmbProject.SelectedValue.ToString()), Convert.ToInt32(cmbBatch.SelectedValue.ToString()), 1, "0");
                CtrlPolicy pPolicy = new CtrlPolicy(Convert.ToInt32(cmbProject.SelectedValue.ToString()), Convert.ToInt32(cmbBatch.SelectedValue.ToString()), "1", "0");
                wfePolicy wPolicy = new wfePolicy(sqlCon, pPolicy);

                //eSTATES[] state = new eSTATES[1];
                //state[0] = eSTATES.POLICY_CREATED;
                ds = wPolicy.GetPolicyListImport(cmbProject.SelectedValue.ToString(), cmbBatch.SelectedValue.ToString());
                
                if (ds.Tables.Count > 0)
                {
                    lstPolicy.Items.Clear();
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        cmbFile.DataSource = ds.Tables[0];
                        cmbFile.DisplayMember = ds.Tables[0].Columns[0].ToString();
                        cmbFile.ValueMember = ds.Tables[0].Columns[0].ToString();
                    }
                    //lstPolicy.Columns.Add("policy");
                    for (int z = 0; z < ds.Tables[0].Rows.Count; z++)
                    {
                        lstPolicy.Items.Add(ds.Tables[0].Rows[z][0].ToString());
                        lstCheckDeed.Items.Add(ds.Tables[0].Rows[z][0].ToString());
                        lstAddlPages.Items.Add(ds.Tables[0].Rows[z][0].ToString());
                    }
                    if (lstPolicy.Items.Count > 0)
                    {
                        lstPolicy.Items[0].Selected = true;
                        GetIndexDetails(lstPolicy.Items[0].Text);
                        GetDeedVolume(lstPolicy.Items[0].Text);
                    }
                    else
                    {

                        lblinfo.Text = "No More Deed...";
                        lblfirstparty.Text = "";
                        lblsecondParty.Text = "";
                        txtDeedno.Text = "";
                        txtDeedYear.Text = "";
                        Txtfirstparty.Text = "";
                        txtsecondparty.Text = "";
                        txtVol.Text = "";
                        txtPgFrom.Text = "";
                        txtPgTo.Text = "";
                    }
                }
            }
        }
        private void GetDeedVolume(string deed_no)
        {
            try
            {
                wQuery pQuery = new ihwQuery(sqlCon);
                DataSet dsVol = new DataSet();
                dsVol = pQuery.GetDeedVolume(cmbProject.SelectedValue.ToString(), cmbBatch.SelectedValue.ToString(),"1",deed_no );
                txtVol.Text = dsVol.Tables[0].Rows[0][0].ToString();
                txtvol1.Text = dsVol.Tables[0].Rows[0][0].ToString();
                txtPgFrom.Text = dsVol.Tables[0].Rows[0][1].ToString();
                txtPGfrom1.Text = dsVol.Tables[0].Rows[0][1].ToString();
                txtPgTo.Text = dsVol.Tables[0].Rows[0][2].ToString();
                txtPGto2.Text = dsVol.Tables[0].Rows[0][2].ToString();
                //if (txtPgFrom.Text != "" && txtPgTo.Text != "")
                //{
                //    txtTotalpages.Text = (Convert.ToInt32(txtPgTo.Text) - Convert.ToInt32(txtPgFrom.Text)).ToString();
                //}
            }
            catch (Exception ex)
            {

            }
        }
        private void GetIndexDetails(string deed_no)
        {
            wQuery pQuery = new ihwQuery(sqlCon);
            DataSet dsVol = new DataSet();
            string IMGName = deed_no;
            IMGName = IMGName.Split(new char[] { '[', ']' })[1];
            string deed_year = deed_no.Substring(deed_no.IndexOf("[") - 4, 4);
            dsVol = pQuery.GetIndexDetails(IMGName, deed_year);
            if (dsVol.Tables[0].Rows.Count >= 1)
            {
                txtDeedYear.Text = dsVol.Tables[0].Rows[0][0].ToString();
                txtDeedno.Text = dsVol.Tables[0].Rows[0][1].ToString();
                lblfirstparty.Text = dsVol.Tables[0].Rows[0][2].ToString();
                Txtfirstparty.Text = dsVol.Tables[0].Rows[0][3].ToString();

                txtDeedYear1.Text = dsVol.Tables[0].Rows[0][0].ToString();
                txtDeedno1.Text = dsVol.Tables[0].Rows[0][1].ToString();
                lblfirstparty1.Text = dsVol.Tables[0].Rows[0][2].ToString();
                Txtfirstparty1.Text = dsVol.Tables[0].Rows[0][3].ToString();
            }
            if (dsVol.Tables[0].Rows.Count >= 2)
            {
                lblsecondParty.Text = dsVol.Tables[0].Rows[1][2].ToString();
                txtsecondparty.Text = dsVol.Tables[0].Rows[1][3].ToString();

                lblsecondParty1.Text = dsVol.Tables[0].Rows[1][2].ToString();
                txtsecondparty1.Text = dsVol.Tables[0].Rows[1][3].ToString();
            }
            if (dsVol.Tables[0].Rows.Count > 1)
            {

            }
        }
        private void cmdBrowse_Click(object sender, EventArgs e)
        {
            try
            {
                lblinfo.Text = "";
                List<string> fileNames = new List<string>();
                List<string> tempPath = new System.Collections.Generic.List<string>(1000);
               
                fbdPath.ShowDialog();
                txtPath.Text = fbdPath.SelectedPath;
                DirectoryInfo selectedPath = new DirectoryInfo(txtPath.Text);
                if (Directory.Exists(txtPath.Text + "\\Backup"))
                {

                }
                else
                {
                    Directory.CreateDirectory(txtPath.Text + "\\Backup");
                    DirectoryInfo selectedPath1 = new DirectoryInfo(txtPath.Text);
                    foreach (FileInfo file in selectedPath.GetFiles())
                    {
                        if (file.Extension == ".TIF" || file.Extension == ".tif")
                        {
                            file.CopyTo(txtPath.Text + "\\Backup\\" + file.Name);
                        }
                    }
                }

                if (selectedPath.GetFiles().Length > 0)

                    foreach (FileInfo file in selectedPath.GetFiles())
                    {
                        if (file.Extension.Equals(".tif") || file.Extension.Equals(".TIF"))
                        {
                            fileNames.Add(file.FullName);
                            tempPath.Add(txtPath.Text + "\\" + file.ToString());
                        }
                    }
                //lvwItem.SubItems.Add(CLAIMS.ToString());
                //lvwItem.SubItems.Add("0");
                ListViewItems.Clear();
                ListViewItems1.Clear();
                lstImage.Items.Clear();
                //lstTotalImage.Items.Clear();
                lstImage.BeginUpdate();
                //lstTotalImage.BeginUpdate();
                
                foreach (string fileName in fileNames)
                {
                    
                    ListViewItem lvi = lstImage.Items.Add(System.IO.Path.GetFileNameWithoutExtension(fileName));
                    //ListViewItem lvi1 = lstTotalImage.Items.Add(System.IO.Path.GetFileNameWithoutExtension(fileName));
                    lvi.Tag = fileName;
                    //lvi1.Tag = fileName;
                    ListViewItems.Add(fileName, lvi);
                    lstTotalImage.Rows.Add();
                    lstTotalImage.Rows[pos].Cells[0].Value = System.IO.Path.GetFileNameWithoutExtension(fileName);
                    
                    pos = pos + 1;
                    //ListViewItems1.Add(fileName, lvi1);
                }
                //foreach (string fileName in fileNames)
                //{
                //    ListViewItem lvi1 = lstTotalImage.Items.Add(System.IO.Path.GetFileNameWithoutExtension(fileName));
                //    lvi1.Tag = fileName;
                //    ListViewItems.Add(fileName, lvi1);
                //}
                lstImage.EndUpdate();
               // lstTotalImage.EndUpdate();
                if (lstPolicy.Items.Count > 0)
                {
                    lstPolicy.Items[0].Selected = true;
                }
                groupBox2.Enabled = false;
                
            }
                
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }

            //LblCount.Text = lstImage.Items.Count.ToString();
        }

        private void cmdImport_Click(object sender, EventArgs e)
        {
            OdbcTransaction sqlTrans = null;


            try
            {
                if (lstSelImg.Items.Count == 0)
                {
                    DialogResult dr = MessageBox.Show(this, "No Images selected for this deed.., You sure to continue?", "Selected no Image", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);
                    if (dr == DialogResult.Yes)
                    {
                        if (lstPolicy.Items.Count > 0)
                        {
                            lstPolicy.Items[0].Selected = true;

                            if (lstPolicy.SelectedItems.Count > 0)
                            {
                                for (int i = 0; i < lstSelImg.Items.Count; i++)
                                {
                                    //lstSelImg.Items[i].Text
                                    for (int j = 0; j < lstTotalImage.Rows.Count; j++)
                                    {
                                        if (lstTotalImage.Rows[j].Cells[0].Value != null)
                                        {
                                            if (lstSelImg.Items[i].Text == lstTotalImage.Rows[j].Cells[0].Value.ToString())
                                            {
                                                lstTotalImage.Rows[j].Cells[1].Value = lstPolicy.Items[0].Text;
                                            }
                                        }
                                    }

                                }

                            }
                            lstSelImg.Items.Clear();
                            lstPolicy.SelectedItems[0].Remove();
                            if (lstPolicy.Items.Count > 0)
                            {
                                lstPolicy.Items[0].Selected = true;

                            }
                        }
                    }

                    else
                    {
                        return;
                    }
                }
                ////
                else
                {
                    if (lstPolicy.Items.Count > 0)
                    {
                        lstPolicy.Items[0].Selected = true;

                        if (lstPolicy.SelectedItems.Count > 0)
                        {
                            for (int i = 0; i < lstSelImg.Items.Count; i++)
                            {
                                for (int j = 0; j < lstTotalImage.Rows.Count; j++)
                                {
                                    if (lstTotalImage.Rows[j].Cells[0].Value != null)
                                    {
                                        if (lstSelImg.Items[i].Text == lstTotalImage.Rows[j].Cells[0].Value.ToString())
                                        {
                                            lstTotalImage.Rows[j].Cells[1].Value = lstPolicy.Items[0].Text;
                                        }
                                    }
                                }

                            }

                        }
                        lstSelImg.Items.Clear();
                        lstPolicy.SelectedItems[0].Remove();
                        if (lstPolicy.Items.Count > 0)
                        {
                            lstPolicy.Items[0].Selected = true;

                        }
                    }
                }

         
                if (lstPolicy.Items.Count > 0)
                {
                    lstPolicy.Items[0].Selected = true;
                    GetIndexDetails(lstPolicy.SelectedItems[0].Text);
                    GetDeedVolume(lstPolicy.SelectedItems[0].Text);
                }
                
            }
        
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
                lblinfo.Text = "Error...";
                sqlTrans.Rollback();
            }
        }
        private bool insertIntoDB(string image, int imageNo, OdbcTransaction trans,string PolicyNo)
        {
            bool commitBol = true;
            if (lstCheckDeed.Items.Count > 0)
            {
                string sqlStr = string.Empty;
                
                OdbcCommand sqlCmd = new OdbcCommand();
                sqlStr = @"insert into image_master(proj_key,batch_key,box_number, policy_number,created_by,Page_name,status,photo,serial_no,page_index_name) values('" +
                            cmbProject.SelectedValue.ToString() + "','" + cmbBatch.SelectedValue.ToString() + "','1','" + PolicyNo +
                            "','IMPORT','" + image + "','24','0','" + imageNo + "','" + image + "')";
                sqlCmd.Connection = sqlCon;
                sqlCmd.Transaction = trans;
                sqlCmd.CommandText = sqlStr;
                int i = sqlCmd.ExecuteNonQuery();
                if (i > 0)
                {
                    commitBol = true;
                }
                else
                {
                    commitBol = false;
                }
            }

            return commitBol;

        }
        private bool UpdatePolicy(OdbcTransaction trans,string policyNo)
        {
            OdbcCommand sqlCmd = new OdbcCommand();
            bool commitBol = true;
            if (lstCheckDeed.Items.Count > 0)
            {
                string sqlStr = "update policy_master set status = '17' where policy_number = '" + policyNo  + "' and proj_key = '" + cmbProject.SelectedValue.ToString() + "' and batch_key = '" + cmbBatch.SelectedValue.ToString() + "'";
                sqlCmd.Connection = sqlCon;
                sqlCmd.Transaction = trans;
                sqlCmd.CommandText = sqlStr;
                int i = sqlCmd.ExecuteNonQuery();
                if (i > 0)
                {
                    commitBol = true;
                }
                else
                {
                    commitBol = false;
                }
            }
            return commitBol;
        }

        private void cmbProject_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void cmdAdd_Click(object sender, EventArgs e)
        {
            picMain.Image = null;

            try
            {
                for (int i = 0; i < lstImage.Items.Count; i++)
                {
                    if (lstImage.Items[i].Selected == true)
                    {
                        lstSelImg.Items.Add(lstImage.Items[i].Text.ToString());
                    }
                }
                foreach (ListViewItem eachItem in lstImage.SelectedItems)
                {
                    lstImage.Items.Remove(eachItem);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }


       }

        private void cmdRemove_Click(object sender, EventArgs e)
        {
            try
            {
                for (int i = 0; i < lstSelImg.Items.Count; i++)
                {
                    if (lstSelImg.Items[i].Selected == true)
                    {
                        lstImage.Items.Add(lstSelImg.Items[i].Text.ToString());
                    }
                }
                foreach (ListViewItem eachItem in lstSelImg.SelectedItems)
                {
                    lstSelImg.Items.Remove(eachItem);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        private void lstImage_MouseClick(object sender, MouseEventArgs e)
        {
            try
            {
                if (lstImage.Items.Count > 0)
                {
                    if (lstImage.SelectedItems.Count > 0)
                    {


                        picMain.Height = 647;
                        picMain.Width = 625;
                        picMain.Refresh();
                        picMain.ImageLocation = null;
                        string imgPath = txtPath.Text + "\\" + lstImage.SelectedItems[0].Text + ".TIF";
                        picMain.ImageLocation = imgPath;
                        
                        
                        Image newImage = Image.FromFile(imgPath);
                        picMain.Height = Convert.ToInt32(picMain.Height * 1.1);
                        picMain.Width = Convert.ToInt32(picMain.Height * newImage.Width / newImage.Height);
                        
                        picMain.SizeMode = PictureBoxSizeMode.StretchImage;
                        //picMain.Width = Convert.ToInt32(picMain.Width * 1);
                        //picMain.Height = Convert.ToInt32(picMain.Height * 1);
                        picMain.Refresh();
                        newImage.Dispose();
                       // GC.Collect();
                        picMain.MouseWheel += new MouseEventHandler(picMain_MouseWheel);
                        picMain.MouseHover += new EventHandler(picMain_MouseHover);
                    }
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message.ToString());
                GC.Collect();
            }
        }
        void picMain_MouseWheel(object sender, MouseEventArgs e)
        {
            //if (e.Delta < 0)
            //{
            //    ZoomIn();
            //}
            //else
            //{
            //    ZoomOut();
            //}

        }
        protected override void OnMouseWheel(MouseEventArgs mea)
        {

            //if (picMain.Image != null)
            //{
            //    if (mea.Delta > 0)
            //    {
            //        if ((picMain.Width < (15 * this.Width)) && (picMain.Height < (15 * this.Height)))
            //        {
            //            picMain.Width = (int)(picMain.Width * 1.25);
            //            picMain.Height = (int)(picMain.Height * 1.25);
            //        }
            //    }

            //    else
            //    {
            //        // Check if the pictureBox dimensions are in range (15 is the minimum and maximum zoom level)
            //        if ((picMain.Width > (this.Width / 15)) && (picMain.Height > (this.Height / 15)))
            //        {
            //            // Change the size of the picturebox, divide it by the ZOOM FACTOR
            //            picMain.Width = (int)(picMain.Width / 1.25);
            //            picMain.Height = (int)(picMain.Height / 1.25);
            //        }
            //    }
            //}
            //picMain.Refresh();
        }
        private void ZoomOut()
        {
            if(picMain.Height > (panel1.Height))
            {
                panel1.Width = picMain.Width;
                picMain.SizeMode = PictureBoxSizeMode.StretchImage;
                picMain.Width = Convert.ToInt32(picMain.Width / ZOOMFACTOR);
                picMain.Height = Convert.ToInt32(picMain.Height / ZOOMFACTOR);
                picMain.Refresh();
            }
        }
        private void ZoomIn()
        {
            if ((picMain.Width < (MINMAX * panel1.Width)) &&
                (picMain.Height < (MINMAX * panel1.Height)))
            {
                picMain.Width = Convert.ToInt32(picMain.Width * ZOOMFACTOR);
                picMain.Height = Convert.ToInt32(picMain.Height * ZOOMFACTOR);
                picMain.SizeMode = PictureBoxSizeMode.StretchImage;
                picMain.Refresh();
            }
        }
        private void picMain_MouseEnter(object sender, EventArgs e)
        {
            if (picMain.Focused == false)
            {
                picMain.Focus();
            }
        }

        void picMain_MouseHover(object sender, EventArgs e)
        {
            picMain.Focus();
        }
        private void picMain_MouseMove(object sender, MouseEventArgs e)
        {
            MouseEventArgs mouse = e as MouseEventArgs;

            if (mouse.Button == MouseButtons.Left)
            {
                Point mousePosNow = mouse.Location;

                int deltaX = mousePosNow.X - mouseDown.X;
                int deltaY = mousePosNow.Y - mouseDown.Y;

                int newX = picMain.Location.X + deltaX;
                int newY = picMain.Location.Y + deltaY;

                picMain.Location = new Point(newX, newY);
            }
        }

        private void lstSelImg_MouseClick(object sender, MouseEventArgs e)
        {
            if (lstSelImg.Items.Count > 0)
            {
                if (lstSelImg.SelectedItems.Count > 0)
                {



                    string imgPath = txtPath.Text + "\\" + lstSelImg.SelectedItems[0].Text + ".TIF";
                    picMain.Height = 647;
                    picMain.Width = 625;
                    picMain.Refresh();
                    picMain.ImageLocation = null;
                   
                    picMain.ImageLocation = imgPath;


                    Image newImage = Image.FromFile(imgPath);
                    picMain.Height = Convert.ToInt32(picMain.Height * 1.1);
                    picMain.Width = Convert.ToInt32(picMain.Height * newImage.Width / newImage.Height);

                    picMain.SizeMode = PictureBoxSizeMode.StretchImage;
                    //picMain.Width = Convert.ToInt32(picMain.Width * 1);
                    //picMain.Height = Convert.ToInt32(picMain.Height * 1);
                    picMain.Refresh();
                    newImage.Dispose();
                    // GC.Collect();
                }
            }
        }

        private void cmbBatch_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }

        private void cmbBatch_SelectedValueChanged(object sender, EventArgs e)
        {
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //sqlTrans.Rollback();
        }

        private void cmdRejectBatch_Click(object sender, EventArgs e)
        {
            //sqlTrans.Commit();
        }

        private void cmbBatch_SelectedValueChanged_1(object sender, EventArgs e)
        {
        }

        private void cmbBatch_SelectedIndexChanged_1(object sender, EventArgs e)
        {

        }

        private void cmbBatch_Validated(object sender, EventArgs e)
        {
            string batchKey = null;
            if (cmbBatch.SelectedIndex >= 0 && cmbBatch.SelectedValue != null)
            {
                batchKey = cmbBatch.SelectedValue.ToString();
                //CtrlPolicy pPolicy = new CtrlPolicy(Convert.ToInt32(cmbProject.SelectedValue.ToString()), Convert.ToInt32(cmbBatch.SelectedValue.ToString()), 1, "0");
                CtrlPolicy pPolicy = new CtrlPolicy(Convert.ToInt32(cmbProject.SelectedValue.ToString()), Convert.ToInt32(cmbBatch.SelectedValue.ToString()), "1", "0");
                wfePolicy wPolicy = new wfePolicy(sqlCon, pPolicy);

                //eSTATES[] state = new eSTATES[1];
                //state[0] = eSTATES.POLICY_CREATED;
                ds = wPolicy.GetPolicyListImport(cmbProject.SelectedValue.ToString(), cmbBatch.SelectedValue.ToString());

                if (ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count <= 0)
                    {
                        DialogResult dr = MessageBox.Show(this, "There's no deed in this volume, You sure to continue?", "Nothing Left", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);
                        if (dr == DialogResult.No)
                        {
                            cmbBatch.Focus();
                            return;
                        }
                    }
                }
            }

        }

        private void lstImage_SelectedIndexChanged(object sender, EventArgs e)
        {
            lstImage_MouseClick(sender, new MouseEventArgs(MouseButtons.Left, 1, 0, 0, 0));
        }

        private void lstSelImg_SelectedIndexChanged(object sender, EventArgs e)
        {
            lstSelImg_MouseClick(sender, new MouseEventArgs(MouseButtons.Left, 1, 0, 0, 0));
        }

        private void textBox7_TextChanged(object sender, EventArgs e)
        {

        }

        private void listView7_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        
        private void tabPage1_Click(object sender, EventArgs e)
        {

        }

        private void txtDeedno1_TextChanged(object sender, EventArgs e)
        {

        }

        private void lstTotalImage_Click(object sender, EventArgs e)
        {

        }

        private void lstImage_Click(object sender, EventArgs e)
        {

        }

        private void lstTotalImage_MouseClick(object sender, MouseEventArgs e)
        {
            try
            {
                if (lstTotalImage.Rows.Count > 0)
                {
                    
                string imgPath = txtPath.Text + "\\" + lstTotalImage.CurrentRow.Cells[0].Value.ToString() + ".TIF";
                picMain.Height = 647;
                picMain.Width = 625;
                picMain.Refresh();
                picMain.ImageLocation = null;

                picMain.ImageLocation = imgPath;


                Image newImage = Image.FromFile(imgPath);
                picMain.Height = Convert.ToInt32(picMain.Height * 1.1);
                picMain.Width = Convert.ToInt32(picMain.Height * newImage.Width / newImage.Height);

                picMain.SizeMode = PictureBoxSizeMode.StretchImage;
                //picMain.Width = Convert.ToInt32(picMain.Width * 1);
                //picMain.Height = Convert.ToInt32(picMain.Height * 1);
                picMain.Refresh();
                newImage.Dispose();
                    }
                }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message.ToString());
                GC.Collect();
            }
        }

        private void lstTotalImage_SelectedIndexChanged(object sender, EventArgs e)
        {
            lstTotalImage_MouseClick(sender, new MouseEventArgs(MouseButtons.Left, 1, 0, 0, 0));
        }

        private void lstSelectedImg_MouseClick(object sender, MouseEventArgs e)
        {
            if (lstSelectedImg.Items.Count > 0)
            {
                if (lstSelectedImg.SelectedItems.Count > 0)
                {



                    string imgPath = txtPath.Text + "\\" + lstSelectedImg.SelectedItems[0].Text + ".TIF";
                    picMain.Height = 647;
                    picMain.Width = 625;
                    picMain.Refresh();
                    picMain.ImageLocation = null;
                    
                    picMain.ImageLocation = imgPath;


                    Image newImage = Image.FromFile(imgPath);
                    picMain.Height = Convert.ToInt32(picMain.Height * 1.1);
                    picMain.Width = Convert.ToInt32(picMain.Height * newImage.Width / newImage.Height);

                    picMain.SizeMode = PictureBoxSizeMode.StretchImage;
                    //picMain.Width = Convert.ToInt32(picMain.Width * 1);
                    //picMain.Height = Convert.ToInt32(picMain.Height * 1);
                    picMain.Refresh();
                    newImage.Dispose();
                }
            }
        }

        
        private void lstCheckDeed_MouseClick(object sender, MouseEventArgs e)
        {
            lstSelectedImg.Items.Clear(); 
            for(int i =0;i<lstTotalImage.Rows.Count-1;i++)
               {
                   if (lstTotalImage.Rows[i].Cells[1].Value != null)
                   {
                   if(lstTotalImage.Rows[i].Cells[1].Value.ToString() == lstCheckDeed.SelectedItems[0].Text)
                   {
                       lstSelectedImg.Items.Add(lstTotalImage.Rows[i].Cells[0].Value.ToString());
                   }
                   }
               }
        }

        private void lstCheckDeed_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            picMain.Image = null;
            picMain.Refresh();
        }

        private void lstTotalImage_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void cmdremove1_Click(object sender, EventArgs e)
        {
            try
            {
                for (int i = 0; i < lstSelectedImg.Items.Count; i++)
                {
                    if (lstSelectedImg.Items[i].Selected == true)
                    {
                        for (int j = 0; j < lstTotalImage.Rows.Count - 1; j++)
                        {
                            if (lstTotalImage.Rows[j].Cells[0].Value != null)
                            {
                                if (lstTotalImage.Rows[j].Cells[0].Value.ToString() == lstSelectedImg.Items[i].Text)
                                {
                                    lstTotalImage.Rows[j].Cells[1].Value = null;
                                    lstTotalImage.Rows[j].DefaultCellStyle.BackColor = Color.Yellow;
                                }
                            }
                        }
                    }
                }
                foreach (ListViewItem eachItem in lstSelectedImg.SelectedItems)
                {
                    lstSelectedImg.Items.Remove(eachItem);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        private void CmdFinalSave_Click(object sender, EventArgs e)
        {
            OdbcTransaction sqlTrans = null;
            string path = string.Empty;
            string oldFilename = string.Empty;
            string newFilename = string.Empty;
            wfeProject wfep = new wfeProject(sqlCon);
            DirectoryInfo selectedPath = new DirectoryInfo(txtPath.Text);
            try
            {
                DialogResult dr = MessageBox.Show(this, "Ready to move Images??,Transaction cannot be rolled back, You sure to continue?", "Importing Images", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);
                if (dr == DialogResult.Yes)
                {
                    lstTotalImage.Enabled = false;
                    lstSelectedImg.Enabled = false;
                    lstCheckDeed.Enabled = false;
                    cmdadd1.Enabled = false;
                    cmdremove1.Enabled = false;
                    CmdFinalSave.Enabled = false;
                    //path = wfep.GetPath(Convert.ToInt32(cmbProject.SelectedValue.ToString()), sqlTrans).Tables[0].Rows[0][0].ToString();
                    path = wfep.GetPath(Convert.ToInt32(cmbProject.SelectedValue.ToString())).Tables[0].Rows[0][0].ToString();
                    sqlTrans = sqlCon.BeginTransaction();
                    if (lstCheckDeed.Items.Count > 0)
                    {
                        

                        for (int i = 0; i < lstCheckDeed.Items.Count; i++)
                        {

                            
                            string scanFolder = path + "\\" + cmbBatch.Text + "\\1\\" + lstCheckDeed.Items[i].Text + "\\Scan";
                            if (!Directory.Exists(scanFolder))
                            {
                                Directory.CreateDirectory(scanFolder);
                            }

                            List<string> fileNames = new List<string>();
                            int sequence = 1;
                            
                            for (int j = 0; j < lstTotalImage.Rows.Count - 1; j++)
                            {
                                if (lstTotalImage.Rows[j].Cells[1].Value != null)
                                {
                                    if (lstCheckDeed.Items[i].Text == lstTotalImage.Rows[j].Cells[1].Value.ToString())
                                    {
                                        fileNames.Add(lstTotalImage.Rows[j].Cells[0].Value.ToString() + ".TIF");
                                        newFilename = scanFolder + "\\" + lstCheckDeed.Items[i].Text + "_" + sequence.ToString().PadLeft(3, '0') + "_A.TIF";
                                        string imageName = lstCheckDeed.Items[i].Text + "_" + sequence.ToString().PadLeft(3, '0') + "_A.TIF";
                                        File.Copy(txtPath.Text + "\\" + lstTotalImage.Rows[j].Cells[0].Value.ToString() + ".TIF", newFilename, true);
                                        insertIntoDB(imageName, sequence, sqlTrans, lstCheckDeed.Items[i].Text);
                                        sequence = sequence + 1;

                                    }
                                }

                            }
                            if (UpdatePolicy(sqlTrans, lstCheckDeed.Items[i].Text))
                            {

                            }
                            wfePolicy wfe = new wfePolicy(sqlCon, new CtrlPolicy(Convert.ToInt32(cmbProject.SelectedValue.ToString()), Convert.ToInt32(cmbBatch.SelectedValue.ToString()), "1", lstCheckDeed.Items[i].Text));
                            wfe.UpdateTransactionLog(eSTATES.POLICY_SCANNED, crd,sqlTrans);
                        }
                    }

                    sqlTrans.Commit();
                    MessageBox.Show(this,"Images Successfully Imported","Import Images",MessageBoxButtons.OK,MessageBoxIcon.Information);
                }
                else
                {
                    return;
                }
            }
            catch (Exception ex)
            {
                sqlTrans.Rollback();
                MessageBox.Show(ex.Message.ToString());
            }
        }
        

        private void cmdadd1_Click(object sender, EventArgs e)
        {
            int indexRow = 0;
            try
            {
                        for (int j = 0; j < lstTotalImage.Rows.Count - 1; j++)
                        {
                                if (lstCheckDeed.SelectedItems[0].Selected == true)
                                {
                                    lstTotalImage.CurrentRow.Cells[1].Value = lstCheckDeed.SelectedItems[0].Text;
                                    
                                }
                        }
                        lstSelectedImg.Items.Add(lstTotalImage.CurrentRow.Cells[0].Value.ToString());

                

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        private void tabPage3_Click(object sender, EventArgs e)
        {

        }

        private void groupBox10_Enter(object sender, EventArgs e)
        {

        }

        private void groupBox11_Enter(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            int indexRow = 0;
            try
            {
                //for (int j = 0; j < lstTotalImage.Rows.Count - 1; j++)
                //{
                //    if (lstAddlPages.SelectedItems[0].Selected == true)
                //    {
                //        lstAddlallpages.CurrentRow.Cells[1].Value = lstAddlPages.SelectedItems[0].Text;

                //    }
                //}
                lstSelectedAddl.Items.Add(lstAddlallpages.CurrentRow.Cells[0].Value.ToString());
                lstAddlallpages.CurrentRow.DefaultCellStyle.BackColor = Color.Yellow;



            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                for (int i = 0; i < lstSelectedAddl.Items.Count; i++)
                {
                    if (lstSelectedAddl.Items[i].Selected == true)
                    {
                        for (int j = 0; j < lstAddlallpages.Rows.Count - 1; j++)
                        {
                            if (lstAddlallpages.Rows[j].Cells[0].Value != null)
                            {
                                if (lstAddlallpages.Rows[j].Cells[0].Value.ToString() == lstSelectedAddl.Items[i].Text)
                                {
                                    lstAddlallpages.Rows[j].Cells[1].Value = null;
                                    lstAddlallpages.Rows[j].DefaultCellStyle.BackColor = Color.White;
                                }
                            }
                        }
                    }
                }
                foreach (ListViewItem eachItem in lstSelectedAddl.SelectedItems)
                {
                    lstSelectedAddl.Items.Remove(eachItem);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void cmdAddl_Click(object sender, EventArgs e)
        {
            try
            {
                lblinfo.Text = "";
                List<string> fileNames = new List<string>();
                List<string> tempPath = new System.Collections.Generic.List<string>(1000);
                fbdPathAddl.ShowDialog();
                txtAddlPages.Text = fbdPathAddl.SelectedPath;
                DirectoryInfo selectedPath = new DirectoryInfo(txtAddlPages.Text);
                if (Directory.Exists(txtAddlPages.Text + "\\Backup"))
                {

                }
                else
                {
                    Directory.CreateDirectory(txtAddlPages.Text + "\\Backup");
                    DirectoryInfo selectedPath1 = new DirectoryInfo(txtAddlPages.Text);
                    foreach (FileInfo file in selectedPath.GetFiles())
                    {
                        if (file.Extension == ".TIF" || file.Extension == ".tif")
                        {
                            file.CopyTo(txtAddlPages.Text + "\\Backup\\" + file.Name);
                        }
                    }
                }

                if (selectedPath.GetFiles().Length > 0)

                    foreach (FileInfo file in selectedPath.GetFiles())
                    {
                        if (file.Extension.Equals(".tif") || file.Extension.Equals(".TIF"))
                        {
                            fileNames.Add(file.FullName);
                            tempPath.Add(txtPath.Text + "\\" + file.ToString());
                        }
                    }


                foreach (string fileName in fileNames)
                {

                    lstAddlallpages.Rows.Add();
                    lstAddlallpages.Rows[posAdd].Cells[0].Value = System.IO.Path.GetFileNameWithoutExtension(fileName);

                    posAdd = posAdd + 1;

                }
                if (lstAddlPages.Items.Count > 0)
                {
                    lstAddlPages.Items[0].Selected = true;
                }
                groupBox2.Enabled = false;

            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }

        }

        private void lstPolicy_Click(object sender, EventArgs e)
        {
            if (lstPolicy.Items.Count > 0)
            {
                //lstPolicy.Items[0].Selected = true;
                GetIndexDetails(lstPolicy.SelectedItems[0].Text);
                GetDeedVolume(lstPolicy.SelectedItems[0].Text);
            }
        }

        private void lstCheckDeed_Click(object sender, EventArgs e)
        {
            if (lstCheckDeed.Items.Count > 0)
            {
                //lstPolicy.Items[0].Selected = true;
                GetIndexDetails(lstCheckDeed.SelectedItems[0].Text);
                GetDeedVolume(lstCheckDeed.SelectedItems[0].Text);
            }
        }

        private void cmdImportAddladd_Click(object sender, EventArgs e)
        {
            try
            {
                for (int j = 0; j < lstTotalImage.Rows.Count - 1; j++)
                {
                    if (lstAddlPages.SelectedItems[0].Selected == true)
                    {
                        lstAddlallpages.CurrentRow.Cells[1].Value = lstAddlPages.SelectedItems[0].Text;
                    }
                }

                lstSelectedAddl.Items.Clear();


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        private void lstAddlPages_MouseClick(object sender, MouseEventArgs e)
        {
            lstSelectedAddl.Items.Clear();
            for (int i = 0; i < lstAddlallpages.Rows.Count - 1; i++)
            {
                if (lstAddlallpages.Rows[i].Cells[1].Value != null)
                {
                    if (lstAddlallpages.Rows[i].Cells[1].Value.ToString() == lstAddlPages.SelectedItems[0].Text)
                    {
                        lstSelectedAddl.Items.Add(lstAddlallpages.Rows[i].Cells[0].Value.ToString());
                    }
                }
            }
        }

        private void cmdaddlFinalImp_Click(object sender, EventArgs e)
        {
            OdbcTransaction sqlTrans = null;
            string path = string.Empty;
            string oldFilename = string.Empty;
            string newFilename = string.Empty;
            string qcFilename = string.Empty;
            wfeProject wfep = new wfeProject(sqlCon);
            DirectoryInfo selectedPath = new DirectoryInfo(txtAddlPages.Text);
            try
            {
                DialogResult dr = MessageBox.Show(this, "Ready to move Additional Images??,Transaction cannot be rolled back, You sure to continue?", "Importing Images", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);
                if (dr == DialogResult.Yes)
                {
                    //lstTotalImage.Enabled = false;
                    //lstSelectedImg.Enabled = false;
                    //lstCheckDeed.Enabled = false;
                    //cmdadd1.Enabled = false;
                    //cmdremove1.Enabled = false;
                    //CmdFinalSave.Enabled = false;
                    path = wfep.GetPath(Convert.ToInt32(cmbProject.SelectedValue.ToString()), sqlTrans).Tables[0].Rows[0][0].ToString();
                    sqlTrans = sqlCon.BeginTransaction();
                    if (lstAddlPages.Items.Count > 0)
                    {

                        path = wfep.GetPath(Convert.ToInt32(cmbProject.SelectedValue.ToString())).Tables[0].Rows[0][0].ToString();
                        //string scanFolder = path + "\\" + cmbBatch.Text + "\\1\\" + lstCheckDeed.SelectedItems[0].Text + "\\Scan";
                        //if (Directory.Exists(path))
                        //{
                        //    Directory.CreateDirectory(scanFolder);
                        //}
                        //else
                        //{
                        //    Directory.CreateDirectory(scanFolder);
                        //}


                        for (int i = 0; i < lstAddlPages.Items.Count; i++)
                        {


                            string scanFolder = path + "\\" + cmbBatch.Text + "\\1\\" + lstAddlPages.Items[i].Text + "\\Scan";
                            string qcFolder = path + "\\" + cmbBatch.Text + "\\1\\" + lstAddlPages.Items[i].Text + "\\QC";
                            //if (!Directory.Exists(scanFolder))
                            //{
                            //    Directory.CreateDirectory(scanFolder);
                            //}

                            List<string> fileNames = new List<string>();
                            int sequence = Convert.ToInt32(wfep.GetMaxImageSerial(lstAddlPages.Items[i].Text.ToString()).Tables[0].Rows[0][0].ToString());
                            sequence = sequence + 1;
                            for (int j = 0; j < lstAddlallpages.Rows.Count - 1; j++)
                            {
                                if (lstAddlPages.Items[i].Text == lstAddlallpages.Rows[j].Cells[1].Value.ToString())
                                {
                                    fileNames.Add(lstTotalImage.Rows[j].Cells[0].Value.ToString() + ".TIF");
                                    newFilename = scanFolder + "\\" + lstAddlPages.Items[i].Text + "_" + sequence.ToString().PadLeft(3, '0') + "_A.TIF";
                                    string imageName = lstAddlPages.Items[i].Text + "_" + sequence.ToString().PadLeft(3, '0') + "_A.TIF";
                                    File.Copy(txtPath.Text + "\\" + lstAddlallpages.Rows[j].Cells[0].Value.ToString() + ".TIF", newFilename, true);
                                    insertIntoDB(imageName, sequence, sqlTrans, lstAddlPages.Items[i].Text);
                                    sequence = sequence + 1;

                                }

                            }
                        }
                    }

                    sqlTrans.Commit();
                    MessageBox.Show(this, "Images Successfully Imported", "Import Images", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    return;
                }
            }
            catch (Exception ex)
            {
                sqlTrans.Rollback();
                MessageBox.Show(ex.Message.ToString());
            }
        }

        private void picMain_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                ZoomIn();
            if (e.Button == MouseButtons.Right)
                ZoomOut();
        }

        private void lstPolicy_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }

        
    }
}
