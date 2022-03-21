using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Data.Odbc;

namespace BurnMedia
{
    public class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        /// 
        
        [STAThread]
        static void Main()
        {
            OdbcConnection pCon = null;
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm(pCon));
        }
    }
}
