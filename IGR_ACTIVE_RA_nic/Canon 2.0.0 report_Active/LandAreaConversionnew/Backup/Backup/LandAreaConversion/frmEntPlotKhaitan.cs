using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace LandAreaConversion
{
    public partial class frmEntPlotKhaitan : Form
    {
        List<string> _item = new List<string>();
        public delegate void MyCallback(List<string> PlotKhaitan); 
        bool _isEditing;
        MyCallback m_callback;
        public frmEntPlotKhaitan(MyCallback pCallBack)
        {
            InitializeComponent();
            init();
            AppendRow();
            m_callback = pCallBack;
        }
        public frmEntPlotKhaitan(MyCallback pCallBack,List<string> item)
        {
            InitializeComponent();                       
            _item = item;
            m_callback = pCallBack;
            init(); 
            
        }

        private void init()
        {
            _isEditing = false;
        }
        private void frmEntPlotKhaitan_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                this.Close();
            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Space)
            {
                if (this.ActiveControl == lstVwPlKh)
                {
                    if (lstVwPlKh.SelectedItems.Count > 0 && _isEditing == false)
                    {
                        _isEditing = true;
                        lstVwPlKh.SelectedItems[0].BeginEdit();
                    }
                }
                else
                {
                    //SendKeys.Send("{Tab}");
                }
            }
        }
        private void AppendRow()
        {
           // string strText = "Test";
           // ListViewItem x = new ListViewItem(strText,0);
            ListViewItem x = new ListViewItem();
            lstVwPlKh.Items.Add(x);
            x.BeginEdit();
        }

        private void frmEntPlotKhaitan_Load(object sender, EventArgs e)
        {
            
            if (_item.Count > 0)
            {
                for (int i = 0; i < _item.Count; i++)
                {
                    //lstVwPlKh.Items.Add((i + 1).ToString());
                    //lstVwPlKh.Items[i].SubItems.Add(_item[i]);
                    lstVwPlKh.Items.Add(_item[i]);
                }
            }
            AppendRow();
            lstVwPlKh.Focus();
            lstVwPlKh.LabelEdit = true;
        }

        private void lstVwPlKh_AfterLabelEdit(object sender, LabelEditEventArgs e)
        {
            ListView tmp = (ListView)sender;
            if (e.Label != null)
            {
                //if (tmp.SelectedItems[0] <= tmp.Items.Count)
                if (tmp.SelectedIndices[0] < tmp.Items.Count-1)
                {
                    tmp.Items[tmp.SelectedIndices[0]+1].BeginEdit();
                }
                else
                {
                    AppendRow();
                }
            }
            else
            {
                //lstVwPlKh.Items[lstVwPlKh.Items.Count - 1].Remove();
                _isEditing = false;
                this.cmdDone.Focus();
            }
           
            /*
            if (e.Label == null)
            {
                if (lstVwPlKh.Items[lstVwPlKh.Items.Count - 1].Text.Trim().Length <= 0)
                {
                    lstVwPlKh.Items[lstVwPlKh.Items.Count - 1].Remove();
                }
                _isEditing = false;
                
            }
            else
            {
                
            }
             * */
        }

        private void lstVwPlKh_BeforeLabelEdit(object sender, LabelEditEventArgs e)
        {
            _isEditing = true;
        }

        private void frmEntPlotKhaitan_FormClosing(object sender, FormClosingEventArgs e)
        {
           
        }

        private void cmdDone_Click(object sender, EventArgs e)
        {
            List<string> retItem = new List<string>();
            if (lstVwPlKh.Items.Count > 0)
            {
                for (int i = 0; i < lstVwPlKh.Items.Count; i++)
                {
                    string g = lstVwPlKh.Items[i].Text;
                    if (lstVwPlKh.Items[i].Text.Trim().Length > 0)
                    {
                       // retItem.Add(lstVwPlKh.Items[i].ToString().Trim());
                        retItem.Add(lstVwPlKh.Items[i].Text.ToString().Trim());
                        
                    }
                }
            }
            m_callback.Invoke(retItem);
            
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void cmdDelete_Click(object sender, EventArgs e)
        {

        }

        private void lstVwPlKh_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete && e.Modifiers == Keys.Control)
            {
                if (lstVwPlKh.SelectedItems.Count > 0)
                {
                    lstVwPlKh.SelectedItems[0].Remove();
                   // AppendRow();
                }
            }
        }

        private void lstVwPlKh_KeyPress(object sender, KeyPressEventArgs e)
        {

        }

        private void cmdClr_Click(object sender, EventArgs e)
        {
            lstVwPlKh.Items.Clear();
        }
    }
}
