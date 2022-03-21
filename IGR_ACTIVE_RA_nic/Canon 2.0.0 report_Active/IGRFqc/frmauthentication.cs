using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DataLayerDefs;
using nControls;
using System.Data.Odbc;
using NovaNet.Utils;
using NvUtils;
using igr_base;

namespace IGRFqc
{
    public partial class frmauthentication : Form
    {
        NovaNet.Utils.dbCon dbcon = new NovaNet.Utils.dbCon();
        Credentials crd = new Credentials();
        private OdbcConnection sqlCon;
        OdbcTransaction trans;
        public delegate void OnAccept(bool result);
        OnAccept m_OnAccept;
        //The method to be invoked when the user aborts all operations
        public delegate void OnAbort();
        OnAbort m_OnAbort;
        PopulateCombo pCom;
        bool authenticated = false;
        public frmauthentication(OdbcConnection prmCon, Credentials prmCrd, OnAccept pOnAccpet, OnAbort pOnAbort,OdbcTransaction pTrans)
        {
            InitializeComponent();
            sqlCon = prmCon;
            crd = prmCrd;
            m_OnAbort = pOnAbort;
            m_OnAccept = pOnAccpet;
            trans = pTrans;
            pCom = new PopulateCombo(sqlCon, trans, crd);
        }

        private void frmauthentication_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                this.Close();
        }

        private void cmdAuth_Click(object sender, EventArgs e)
        {
            if (pCom.Validate_user(txtUserpass.Text.Trim()) == true)
            {
                authenticated = true;
                m_OnAccept.Invoke(authenticated);
                this.Close();
            }
            else
            {
                authenticated = false;
                m_OnAccept.Invoke(authenticated);
                this.Close();
            }
        }

        void FrmauthenticationLoad(object sender, EventArgs e)
        {

        }
    }
}
