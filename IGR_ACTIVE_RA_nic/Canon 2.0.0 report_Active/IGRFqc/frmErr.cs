using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace IGRFqc
{
    public partial class frmErr : Form
    {
        List<string> mList = new List<string>();
        public frmErr(List<string> plist)
        {
            InitializeComponent();
            mList = plist;
            init_err();
        }
        private void init_err()
        {
            for (int i = 0; i < mList.Count; i++)
            {
                lstErr.Items.Add(mList[i].ToString());
            }
        }
        private void frmErr_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
        }
    }
}
