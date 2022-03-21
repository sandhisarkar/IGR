using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows;

namespace LandAreaConversion
{
    
    public partial class frmConverter : Form
    {
        string Defaultvalue = "0";
        public delegate void MyCallback(int pNoSplits, double pAcre, double pBigha, double pKatha, double pChhatak,bool pLand,double pSqFeet,double decimal1); 
        MyCallback m_CallBack;
        public frmConverter()
        {
            InitializeComponent();
        }

        public frmConverter(MyCallback pCallBack)
        {
            
            InitializeComponent();            
            m_CallBack = pCallBack;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Reset();
            txtPlotCount.Focus();
            txtAcrP.Text = "0";
            txtBighaP.Text = "0";
            txtKathaP.Text = "0";
            txtChatP.Text = "0";
            txtSqFeet.Text = "0";
            //txtsqfeetLand.Text = "0";
            txtDecimal.Text = "0";

           
        }

        private void Reset()
        {
            txtAcre.Text = Defaultvalue;
            txtBigha.Text = Defaultvalue;
            txtKatha.Text = Defaultvalue;
            txtChattak.Text = Defaultvalue;
            txtsqMtr.Text = Defaultvalue;
            txtsqfeetLand.Text = Defaultvalue;
            txtDecimal.Text = Defaultvalue;
            txtPlotCount.Text = "1";
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            Reset();
            txtPlotCount.Focus();
        }

        private void btnConvert_Click(object sender, EventArgs e)
        {
           // int DecimlC = 0;
            double TotalArean_Chattak = 0;
            double Acre = Convert.ToDouble(txtAcre.Text);
            double Bigha = Convert.ToDouble(txtBigha.Text);
            double Katha = Convert.ToDouble(txtKatha.Text);
           // double Deciml = Convert.ToDouble(txtDec.Text);
            double Chattak = Convert.ToDouble(txtChattak.Text);
           // double SqFt = Convert.ToDouble(txtFeet.Text);
            TotalArean_Chattak = (Acre * 969.6 + Bigha * 320 + Katha * 16+Chattak);
            conversion(TotalArean_Chattak);
            if(Convert.ToInt32(txtPlotCount.Text)>0)
            conversion(TotalArean_Chattak,Convert.ToInt32(txtPlotCount.Text));

            //double AcreC1 = TotalArean_Feet / 872640;
           
        }

        private void conversion(double AreaInchattak)
        {
            /*
            double  m_c = AreaInchattak;
            double  o_c = m_c % convConstants._KATHA_TO_CHHATAK;
            double  m_k = m_c / convConstants._KATHA_TO_CHHATAK;
            double  m_b = (int)(m_k >= convConstants._KATHA_TO_BIGHA ? m_k / convConstants._KATHA_TO_BIGHA : 0);
                    m_k = (int)(m_b >= 1 ? m_k % convConstants._KATHA_TO_BIGHA : 0);
            double m_a = (int)(m_b >= convConstants._BIGHA_TO_ACRE ? m_b / convConstants._BIGHA_TO_ACRE : 0);
                   m_b = (int)(m_a >= 1 ? m_a % convConstants._BIGHA_TO_ACRE : 0);
                   MessageBox.Show("acre: " + m_a + "  bigha:" + m_b + "  katha:" + o_c);
            //double  o_k = (int)(m_k >= convConstants._KATHA_TO_BIGHA ? m_k / convConstants._KATHA_TO_BIGHA : 0);
            */
            //double m_c = AreaInchattak;
            //double o_c = m_c % convConstants._KATHA_TO_CHHATAK;
            //m_c = m_c - o_c;
            //m_c = m_c / convConstants._KATHA_TO_CHHATAK;
            //double m_k = m_c % convConstants._KATHA_TO_BIGHA;
            //m_c = m_c - m_k;
            //m_c = m_c / convConstants._KATHA_TO_BIGHA;
            //double m_b = m_c % convConstants._BIGHA_TO_ACRE;
            //m_c = m_c - m_b;
            //double m_a = m_c / convConstants._BIGHA_TO_ACRE;

            double m_c = AreaInchattak;
            m_c = m_c / (convConstants._BIGHA_TO_ACRE * convConstants._KATHA_TO_BIGHA * convConstants._KATHA_TO_CHHATAK);
            double m_a = (int)m_c;
            m_c = (m_c - m_a) * (convConstants._BIGHA_TO_ACRE * convConstants._KATHA_TO_BIGHA * convConstants._KATHA_TO_CHHATAK);

             m_c =Math.Round(m_c / (convConstants._KATHA_TO_BIGHA * convConstants._KATHA_TO_CHHATAK),convConstants._ROUND_PRECISSION);
            double m_b = (int)m_c;
            m_c = (m_c - m_b) * (convConstants._KATHA_TO_BIGHA * convConstants._KATHA_TO_CHHATAK);

            m_c =Math.Round(m_c / (convConstants._KATHA_TO_CHHATAK),convConstants._ROUND_PRECISSION);
            double m_k = (int)m_c;
            m_c = (m_c - m_k) * (convConstants._KATHA_TO_CHHATAK);





            double TotalArean_Chattak = AreaInchattak;
            double AcreC1 = m_a;
            
            
            double BighaC1 = m_b;
            
            double KathaC1 = m_k;
            
            double ChattakC1 =Math.Round(m_c,convConstants._ROUND_PRECISSION);
            
           

            txtAcrC.Text = Convert.ToString(AcreC1);
            txtBighaC.Text = Convert.ToString(BighaC1);
            txtKathaC.Text = Convert.ToString(KathaC1);
            txtChatC.Text = Convert.ToString(ChattakC1);
           
        }
        private void conversion(double AreaInchattak, int No_of_Plot)
        {
            /*
            double  m_c = AreaInchattak;
            double  o_c = m_c % convConstants._KATHA_TO_CHHATAK;
            double  m_k = m_c / convConstants._KATHA_TO_CHHATAK;
            double  m_b = (int)(m_k >= convConstants._KATHA_TO_BIGHA ? m_k / convConstants._KATHA_TO_BIGHA : 0);
                    m_k = (int)(m_b >= 1 ? m_k % convConstants._KATHA_TO_BIGHA : 0);
            double m_a = (int)(m_b >= convConstants._BIGHA_TO_ACRE ? m_b / convConstants._BIGHA_TO_ACRE : 0);
                   m_b = (int)(m_a >= 1 ? m_a % convConstants._BIGHA_TO_ACRE : 0);
                   MessageBox.Show("acre: " + m_a + "  bigha:" + m_b + "  katha:" + o_c);
            //double  o_k = (int)(m_k >= convConstants._KATHA_TO_BIGHA ? m_k / convConstants._KATHA_TO_BIGHA : 0);
            */
            //double m_c = AreaInchattak;
            //double o_c = m_c % convConstants._KATHA_TO_CHHATAK;
            //m_c = m_c - o_c;
            //m_c = m_c / convConstants._KATHA_TO_CHHATAK;
            //double m_k = m_c % convConstants._KATHA_TO_BIGHA;
            //m_c = m_c - m_k;
            //m_c = m_c / convConstants._KATHA_TO_BIGHA;
            //double m_b = m_c % convConstants._BIGHA_TO_ACRE;
            //m_c = m_c - m_b;
            //double m_a = m_c / convConstants._BIGHA_TO_ACRE;

            double m_c = AreaInchattak / No_of_Plot;
            m_c = m_c / (convConstants._BIGHA_TO_ACRE * convConstants._KATHA_TO_BIGHA * convConstants._KATHA_TO_CHHATAK);
            double m_a = (int)m_c;
            m_c = (m_c - m_a) * (convConstants._BIGHA_TO_ACRE * convConstants._KATHA_TO_BIGHA * convConstants._KATHA_TO_CHHATAK);

            m_c = Math.Round(m_c / (convConstants._KATHA_TO_BIGHA * convConstants._KATHA_TO_CHHATAK), convConstants._ROUND_PRECISSION);
            double m_b = (int)m_c;
            m_c = (m_c - m_b) * (convConstants._KATHA_TO_BIGHA * convConstants._KATHA_TO_CHHATAK);

            m_c = Math.Round(m_c / (convConstants._KATHA_TO_CHHATAK), convConstants._ROUND_PRECISSION);
            double m_k = (int)m_c;
            m_c = (m_c - m_k) * (convConstants._KATHA_TO_CHHATAK);





            double TotalArean_Chattak = AreaInchattak;
            double AcreC1 = m_a;

           
            double BighaC1 = m_b;

            double KathaC1 = m_k;

            double ChattakC1 = m_c;



            txtAcrP.Text = Convert.ToString(AcreC1);
            txtBighaP.Text = Convert.ToString(BighaC1);
            txtKathaP.Text = Convert.ToString(KathaC1);
            txtChatP.Text = Convert.ToString(ChattakC1);

        }
     

        private void txtPlotCount_Enter(object sender, EventArgs e)
        {
            txtPlotCount.SelectAll();
        }
        private void txtAcre_Enter(object sender, EventArgs e)
        {
            txtAcre.SelectAll();
        }
        private void txtBigha_Enter(object sender, EventArgs e)
        {
            this.txtBigha.SelectAll();
        }

        private void txtKatha_Enter(object sender, EventArgs e)
        {
            this.txtKatha.SelectAll();
        }

        private void txtChattak_Enter(object sender, EventArgs e)
        {
            this.txtChattak.SelectAll();
        }

        private void txtPlotCount_TextChanged(object sender, EventArgs e)
        {

        }

        private void frmConverter_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                SendKeys.Send("{TAB}");
            if (e.KeyCode == Keys.Escape)
                this.Close();
            if (e.KeyCode == Keys.F1)
                ConvtabControl.SelectedTab = tabPage1;
            if (e.KeyCode == Keys.F2)
                ConvtabControl.SelectedTab = tabPage2;
        }

        private void cmdClose_Click(object sender, EventArgs e)
        {
            bool land = false;
            if (rdoLand.Checked) { land = true; }
            m_CallBack.Invoke(Convert.ToInt32(txtPlotCount.Text), Convert.ToDouble(txtAcrP.Text), Convert.ToDouble(txtBighaP.Text), Convert.ToDouble(txtKathaP.Text), Convert.ToDouble(txtChatP.Text),land,Convert.ToDouble(txtSqFeet.Text),Convert.ToDouble(txtDecimal.Text));
            this.Close();
        }

        private void frmConverter_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.' || (e.KeyChar == (char)(Keys.Back)))
            {
                e.Handled = true;
            }
        }
       
        private void txtAcre_Leave(object sender, EventArgs e)
        {
            SetValues();
        }

        private void txtBigha_Leave(object sender, EventArgs e)
        {
            SetValues();
        }

        private void txtKatha_Leave(object sender, EventArgs e)
        {
            SetValues();
        }

        private void txtChattak_Leave(object sender, EventArgs e)
        {
            SetValues();
        }

        private void frmConverter_FormClosed(object sender, FormClosedEventArgs e)
        {
           
        }

        private void SetValues()
        {
            double TotalArean_Chattak = 0;            
            double Acre = Convert.ToDouble(txtAcre.Text);
            double Bigha = Convert.ToDouble(txtBigha.Text);
            double Katha = Convert.ToDouble(txtKatha.Text);
            double Chattak = Convert.ToDouble(txtChattak.Text);
            //TotalArean_Chattak = (Acre * 969.6 + Bigha * 320 + Katha * 16 + Chattak);
            TotalArean_Chattak = (Acre * convConstants._BIGHA_TO_ACRE*convConstants._KATHA_TO_BIGHA*convConstants._KATHA_TO_CHHATAK
                + Bigha * convConstants._KATHA_TO_BIGHA*convConstants._KATHA_TO_CHHATAK + Katha * convConstants._KATHA_TO_CHHATAK + Chattak);
            conversion(TotalArean_Chattak);
            if (Convert.ToInt32(txtPlotCount.Text) > 0)
            conversion(TotalArean_Chattak, Convert.ToInt32(txtPlotCount.Text));
        }

        private void ConvSmtrToSfeet()
        {
            double sqrFeet = 0;
            double SqrMtr = Convert.ToDouble(txtsqMtr.Text);
            sqrFeet =Math.Round(SqrMtr * convConstants._SQMETER_TO_SQFEET,convConstants._ROUND_PRECISSION);
            txtSqFeet.Text = Convert.ToString(sqrFeet);
        }

        private void txtAcre_KeyUp(object sender, KeyEventArgs e)
        {
            SetValues();
        }

        private void txtBigha_KeyUp(object sender, KeyEventArgs e)
        {
            SetValues();
        }

        private void txtKatha_KeyUp(object sender, KeyEventArgs e)
        {
            SetValues();
        }

        private void txtChattak_KeyUp(object sender, KeyEventArgs e)
        {
            SetValues();
        }

        private void txtsqMtr_KeyUp(object sender, KeyEventArgs e)
        {
            ConvSmtrToSfeet();
        }

        private void txtsqMtr_Leave(object sender, EventArgs e)
        {
            txtChattak_KeyUp(this, null);
        }

        private void ConvtabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ConvtabControl.SelectedIndex == 1)
            {
                rdoLand.Checked = true;
            }
        }

        private void rdoLand_Leave(object sender, EventArgs e)
        {
            txtSqFeet.Focus();
        }

        private void rdoStructure_Leave(object sender, EventArgs e)
        {
            txtSqFeet.Focus();
        }

        private void txtDecimal_Enter(object sender, EventArgs e)
        {
            txtDecimal.SelectAll();
        }

        private void txtsqfeetLand_Enter(object sender, EventArgs e)
        {
            txtsqfeetLand.SelectAll();
        }
    }
    public static class convConstants
    {
        public static double _KATHA_TO_CHHATAK = 16;
        public static double _KATHA_TO_BIGHA = 20;
        public static double _BIGHA_TO_ACRE = 3.03;
        public static double _SQMETER_TO_SQFEET = 10.7639;
        public static int _ROUND_PRECISSION = 5;
    }
   
}
