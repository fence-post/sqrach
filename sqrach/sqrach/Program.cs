using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using fp.lib.sqlite;
//using fp.lib.mssql;

namespace fp.sqratch
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
            SqlServerTypes.Utilities.LoadNativeAssemblies(AppDomain.CurrentDomain.BaseDirectory);

            // Application.Run(new OptionsDialog()); return;
            // Application.Run(new NewQueryDialog()); return;
            //Application.Run(new Try()); return;

            Application.Run(new main());
        }
    }
}
