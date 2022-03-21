using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using LandAreaConversion;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        double acreForeach = 0;
        double sqFeetStructure = 0;
        double BighaForeach = 0;
        double KathaForeach = 0;
        double ChatakForeach = 0;
        double decimalforeach = 0;
        double sqfeetland = 0;
        int no_of_split = 0;
        private void button1_Click(object sender, EventArgs e)
        {
            LandAreaConversion.frmConverter frm = new frmConverter(GetResultsFromAnadi);
            frm.ShowDialog();
            frm.Activate();
        }
        public void GetResultsFromAnadi(int pNoSplits, double pAcre, double pBigha, double pKatha, double pChhatak,bool pLand, double pSqFeet,double pDecimal)
        {
            acreForeach = pAcre;
            BighaForeach = pBigha;
            KathaForeach = pKatha;
            ChatakForeach = pChhatak;
            no_of_split = pNoSplits;
            sqFeetStructure = pSqFeet;
            decimalforeach = pDecimal;
            //sqfeetland = pSqftland;
            bool l = pLand;
            //PopulateSplitVal();
        }
    }
}
