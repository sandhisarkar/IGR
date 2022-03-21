using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using NvUtils;
using NovaNet.Utils;
using System.Data.Odbc;

namespace IGRFqc
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            OdbcConnection conn = new dbCon().Connect();
            Credentials crd = new Credentials();
            Application.Run(new frmVolume(conn,crd));
        }
    }
}
