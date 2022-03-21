using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
//using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Data.SqlClient;
using System.Data.Odbc;
using System.Drawing.Drawing2D;
using NovaNet.Utils;
using NovaNet.wfe;
using LItems;

/* Multipage tiff viewer
 * Created by Matjaž Grahek - 25.11.2008
 * 
 * Hi. This is my first article for CodeProject.
 * What you're seeing is an application for opening multipage tiffs and viewing the selected page.
 * 
 * This example is just a part of what might come out from one of you, but i think its good for a start.
 * 
 * Some comments are added next to the code. If you have some more questions, email me at matjaz.grahek@gmail.com
 * or post something on the article page.
 * 
 * PS: This code and the article was written by me and i dont know that much code. Intelisense helps a lot :)
 * I'm trying to say, that the code and application can be written MUCH better but its ment just as something to start from.
 */

namespace ImageHeaven
{
    public partial class frmTransferredData : Form
    {
        private OdbcConnection con;
        public Imagery img;
        public frmTransferredData(OdbcConnection pCon)
        {
            InitializeComponent();
            con = pCon;
            ReadINI();
        }
        
        private int intCurrPage = 0; // defining the current page (its some sort of a counter)
        bool opened = false; // if an image was opened

        public void RefreshImage()
        {
            try
            {
                Image myImg; // setting the selected tiff
                Image myBmp; // a new occurance of Image for viewing

                myImg = System.Drawing.Image.FromFile(@lblFile.Text); // setting the image from a file

                int intPages = myImg.GetFrameCount(System.Drawing.Imaging.FrameDimension.Page); // getting the number of pages of this tiff
                intPages--; // the first page is 0 so we must correct the number of pages to -1
                lblNumPages.Text = Convert.ToString(intPages); // showing the number of pages
                lblCurrPage.Text = Convert.ToString(intCurrPage); // showing the number of page on which we're on

                myImg.SelectActiveFrame(System.Drawing.Imaging.FrameDimension.Page, intCurrPage); // going to the selected page

                //myBmp = new Bitmap(myImg, pictureControl.Width, pictureControl.Height); // setting the new page as an image
                // Description on Bitmap(SOURCE, X,Y)
                pictureControl.Image = null;
                pictureControl.Width = panel1.Width - 2;
                pictureControl.Height = panel1.Height - 2;
                
                double scaleX = (double)pictureControl.Width / (double)myImg.Width;
                double scaleY = (double)pictureControl.Height / (double)myImg.Height;
                double Scale = Math.Max(scaleX, scaleY);
                int w = (int)(myImg.Width * Scale);
                int h = (int)(myImg.Height * Scale);
                pictureControl.Width = w;
                pictureControl.Height = h;
                pictureControl.Image = CreateThumbnail(myImg, w, h); //newImage.GetThumbnailImage(w, h, new System.Drawing.Image.GetThumbnailImageAbort(GetThumbnailImageAbort), IntPtr.Zero);
                myImg.Dispose();
                //pictureControl.Image = myBmp; // showing the page in the pictureBox1
            }
            catch (Exception ex)
            {
                string e = ex.Message;
            }

        }
        public Image CreateThumbnail(Image pImage, int lnWidth, int lnHeight)
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
//                graphic.DrawImage(pImage, 0, 0, lnWidth, lnHeight);
                graphic.DrawImage(pImage, 0, 0, pictureControl.Width, pictureControl.Height);
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
        private void button1_Click(object sender, EventArgs e)
        {
            previous();
        }
        private void previous()
        {
            if (opened) // the button works if the file is opened. you could go with button.enabled
            {
                if (intCurrPage == 0) // it stops here if you reached the bottom, the first page of the tiff
                { intCurrPage = 0; }
                else
                {
                    intCurrPage--; // if its not the first page, then go to the previous page
                    RefreshImage(); // refresh the image on the selected page
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Next();
        }
        private void Next()
        {
            if (opened) // the button works if the file is opened. you could go with button.enabled
            {
                if (intCurrPage == Convert.ToInt32(lblNumPages.Text)) // if you have reached the last page it ends here
                // the "-1" should be there for normalizing the number of pages
                { intCurrPage = Convert.ToInt32(lblNumPages.Text); }
                else
                {
                    intCurrPage++;
                    RefreshImage();
                }
            }
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                intCurrPage = 0; // reseting the counter
                lblFile.Text = openFileDialog1.FileName; // showing the file name in the lblFile
                RefreshImage(); // refreshing and showing the new file
                opened = true; // the files was opened.
            }
        }
        SqlConnection sqlCon = null;
        private void GenerateGrid()
        {
            DataSet ds = GetAllAccount(sqlCon);
            dgvExport.DataSource = ds.Tables[0];
            dgvExport.Columns[1].Width = 150;
            //dgvExport.Columns[2].Width = 120;
            //dgvExport.Columns[3].Width = 80;
            //dgvExport.Columns[4].Width = 120;
            
            dgvExport.Columns[1].ReadOnly = true;
            //dgvExport.Columns[2].ReadOnly = true;
            //dgvExport.Columns[3].ReadOnly = true;
            //dgvExport.Columns[4].ReadOnly = true;
            
            //UpdateFont();
            label4.Text = ds.Tables[0].Rows.Count.ToString() + " Records found";
        }
        public static DataSet GetAllAccount(SqlConnection pCon)
        {
            string sql = "Select id_no from deed_image order by id_no";
            DataSet ds = new DataSet();
            SqlDataAdapter odap = new SqlDataAdapter(sql, pCon);
            odap.Fill(ds);

            return ds;
        }
        private void cmdPath_Click(object sender, EventArgs e)
        {
            openFileDialog.Filter = "Sql files (*.MDF)|*.MDF";
            openFileDialog.Title = "Select sql server file";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                if (sqlCon != null)
                {
                    if (sqlCon.State == ConnectionState.Open) { SqlConnection.ClearPool(sqlCon); DetachDb(sqlCon); }
                }
                txtFolderPath.Text = openFileDialog.FileName;
                string dbTempPath = System.IO.Path.GetDirectoryName(Application.ExecutablePath) + "\\Temp";
                if (!Directory.Exists(dbTempPath)) { Directory.CreateDirectory(dbTempPath); }
                else { Directory.Delete(dbTempPath,true); Directory.CreateDirectory(dbTempPath); }
                dbTempPath = dbTempPath + "\\IGR.mdf";
                File.Copy(txtFolderPath.Text, dbTempPath);
                string sql = @"Server="+ sqlIp + @"\SUMITEXPRESS;AttachDbFilename=" + dbTempPath + "; Database=IGR;Trusted_Connection=Yes;";
                sqlCon = new SqlConnection(sql);
                sqlCon.Open();
                if (sqlCon.State == ConnectionState.Open) { GenerateGrid(); } //cmdPath.Enabled = false; }
            }
        }
        public static DataSet GetImages(string id_no, SqlConnection pSqlCon)
        {
            string sql = "Select deed_image from deed_image where id_no = '" + id_no + "'";
            DataSet ds = new DataSet();
            SqlDataAdapter odap = new SqlDataAdapter(sql, pSqlCon);
            odap.Fill(ds);

            return ds;
        }
        private void dgvExport_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            //string district = dgvExport.Rows[e.RowIndex].Cells[1].Value.ToString();
            //string ro =   dgvExport.Rows[e.RowIndex].Cells[2].Value.ToString();
            //string book =   dgvExport.Rows[e.RowIndex].Cells[3].Value.ToString();
            //string deed_year =  dgvExport.Rows[e.RowIndex].Cells[4].Value.ToString();
            string id_no =  dgvExport.Rows[e.RowIndex].Cells[1].Value.ToString();
            DataSet ds = GetImages(id_no, sqlCon);
            string imageName = dgvExport.Rows[e.RowIndex].Cells[1].Value.ToString();
            if (ds.Tables[0].Rows.Count == 1)
            {
                Byte[] data = new Byte[0];
                data = (Byte[])(ds.Tables[0].Rows[0][0]);
                //MemoryStream mem = new MemoryStream();
                //mem.Write(data, 0, data.Length);
                
                
                string imgTempPath = System.IO.Path.GetDirectoryName(Application.ExecutablePath) + "\\TempMultiTiff";
                
                if (!Directory.Exists(imgTempPath)) { Directory.CreateDirectory(imgTempPath); }
                else { Directory.Delete(imgTempPath, true); Directory.CreateDirectory(imgTempPath); }
                imgTempPath = imgTempPath + "\\" + imageName + ".TIF";
                FileStream fls = new FileStream(imgTempPath, FileMode.CreateNew);
                fls.Write(data, 0, data.Length);
                fls.Dispose();
                //img.SaveAsTiff(imgTempPath, IGRComressionTIFF.LZW);
                                //img.Save(imgTempPath + "\\Temp.TIFF");
                if (File.Exists(imgTempPath))
                {
                    intCurrPage = 0; // reseting the counter
                    lblFile.Text = imgTempPath; // showing the file name in the lblFile
                    RefreshImage(); // refreshing and showing the new file
                    opened = true; // the files was opened.
                }
                //picPhoto.Image.Save("D:\\1.jpg");
            } 
        }

        private void lblFile_Click(object sender, EventArgs e)
        {

        }

        private void frmTransferredData_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (sqlCon.State == ConnectionState.Open) { SqlConnection.ClearPool(sqlCon); DetachDb(sqlCon); }
        }
        string sqlIp = string.Empty;
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
        private bool DetachDb(SqlConnection pSqlCon)
        {
            try
            {
                pSqlCon.Close();
                string sql = @"Data Source=" + sqlIp + @"\SQLExpress;Initial Catalog=master;Integrated Security=SSPI;";
                SqlConnection CN = new SqlConnection(sql);
                CN.Open();

                SqlCommand sqlCom = new SqlCommand();
                sqlCom.Connection = CN;
                sqlCom.CommandText = "sys.sp_detach_db IGR";
                sqlCom.ExecuteNonQuery();
                CN.Close();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private void pictureControl_LoadCompleted(object sender, AsyncCompletedEventArgs e)
        {
            panel1.Focus();
        }

        private void pictureControl_Paint(object sender, PaintEventArgs e)
        {
            panel1.Focus();
        }

        private void frmTransferredData_Load(object sender, EventArgs e)
        {

        }

        private void frmTransferredData_KeyDown(object sender, KeyEventArgs e)
        {
            //if (e.KeyData == Keys.LButton)
            //{
            //    previous();
            //}
            //if (e.KeyData == Keys.RButton)
            //{
            //    Next();
            //}
        }

        private void frmTransferredData_KeyPress(object sender, KeyPressEventArgs e)
        {
            //if (e.KeyChar == 12)
            //{
            //    Next();
            //}
            //if (e.KeyChar == 10)
            //{
            //    previous();
            //}
        }
    }
}
