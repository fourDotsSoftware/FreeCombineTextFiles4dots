using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace FreeCombinePDFFolderWatcher
{
    class ExceptionHandlersHelper
    {
        public static void AddUnhandledExceptionHandlers()
        {            
            // Define a handler for unhandled exceptions.
            AppDomain.CurrentDomain.UnhandledException +=
                new System.UnhandledExceptionEventHandler(myExceptionHandler);
            Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(
                myThreadExceptionHandler);

        }

        private static void myExceptionHandler(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = (Exception)e.ExceptionObject;
            Module.ShowError(TranslateHelper.Translate("Unspecified Error"), ex.ToString());
            
            /*
            Dim EX As Exception 
            EX = e.ExceptionObject 
            Console.WriteLine(EX.StackTrace) 
            End Sub 
            */
        }
        private static void myThreadExceptionHandler(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            Module.ShowError(TranslateHelper.Translate("Unspecified Error"), e.Exception.ToString());
            /*
            Private Sub MYThreadHandler(ByVal sender As Object, ByVal e As Threading.ThreadExceptionEventArgs) 
            Console.WriteLine(e.Exception.StackTrace) 
            End Sub
            */
        }

    }
}

public class Module
{
    public static void ShowError(string msg, string caption)
    {
        System.Windows.Forms.MessageBox.Show(msg,caption, System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information, System.Windows.Forms.MessageBoxDefaultButton.Button1, System.Windows.Forms.MessageBoxOptions.ServiceNotification);

    }
}

public class TranslateHelper
{
    public static string Translate(string str)
    {
        return str;
    }
}
