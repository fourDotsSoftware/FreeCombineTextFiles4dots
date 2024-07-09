using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace FreeCombinePDFFolderWatcher
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            ExceptionHandlersHelper.AddUnhandledExceptionHandlers();

            bool forcu = true;

            if (args != null && args.Length > 0)
            {
                if (args[0].ToLower().Trim() == "-lm")
                {
                    forcu = false;
                }
            }

            Application.Run(new frmMain(forcu));
        }
    }
}
