using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Microsoft.Win32;

namespace FreeCombineTextFiles4dots
{
    public partial class frmOptionsWatchers : FreeCombineTextFiles4dots.CustomForm
    {
        public frmOptionsWatchers()
        {
            InitializeComponent();
        }

        private string GetInstallUtilPath()
        {
            string sysdir = Environment.GetFolderPath(Environment.SpecialFolder.System);

            sysdir = System.IO.Path.GetDirectoryName(sysdir) + "\\Microsoft.NET\\Framework\\";

            string msbfile = sysdir + @"v2.0.50727\installutil.exe";

            if (!System.IO.File.Exists(msbfile))
            {
                string[] dirz = System.IO.Directory.GetDirectories(sysdir);

                for (int k = 0; k < dirz.Length; k++)
                {
                    if (System.IO.Path.GetFileName(dirz[k]).ToLower().StartsWith("v") && System.IO.Path.GetFileName(dirz[k]).IndexOf(".") >= 0)
                    {
                        if (System.IO.Path.GetFileName(dirz[k]).ToLower().StartsWith("v3") || System.IO.Path.GetFileName(dirz[k]).ToLower().StartsWith("v4"))
                        {
                            msbfile = System.IO.Path.Combine(dirz[k], "installutil.exe");

                            if (System.IO.File.Exists(msbfile))
                            {
                                break;
                            }
                        }
                    }
                }
            }

            return msbfile;

        }

        private void btnInstallService_Click(object sender, EventArgs e)
        {
            SetRegistry();

            /*
            string file = "\"" + GetInstallUtilPath() + "\"";
            string args="\"" + Application.StartupPath + "\\ConvertPowerpointToVideoService.exe\"";
            System.Diagnostics.Process.Start(file, args);

            System.Diagnostics.Process.Start("\"" + Application.StartupPath + "\\ConvertPowerpointToVideoServiceInstaller.exe\"", "-i "+file+" "+args);

            //System.Diagnostics.Process.Start("\"" + Application.StartupPath + "\\ConvertPowerpointToVideoServiceInstaller.exe\"", "-i");
            */

        }

        private void btnUninstallService_Click(object sender, EventArgs e)
        {   
            /*      
            string file = "\"" + GetInstallUtilPath() + "\"";
            string args = "\"" + Application.StartupPath + "\\ConvertPowerpointToVideoService.exe\"";

            //System.Diagnostics.Process.Start(file, args);

            System.Diagnostics.Process.Start("\"" + Application.StartupPath + "\\ConvertPowerpointToVideoServiceInstaller.exe\"", "-u "+file+" "+args);
           */
        }

        private void SetRegistry()
        {
            string dirs = "";

            for (int k = 0; k < lstDirs.Items.Count; k++)
            {
                dirs += lstDirs.Items[k].ToString() + "|||";
            }

            /*
            RegistryHelper2.SetKeyValue("Convert Powerpoint to Video 4dots", "WatchFolders", dirs);

            RegistryHelper2.SetKeyValue("Convert Powerpoint to Video 4dots", "AppFilepath",
            "\"" + System.Reflection.Assembly.GetEntryAssembly().Location + "\"");

            RegistryHelper2.SetKeyValue("Convert Powerpoint to Video 4dots", "ConvertArgs",
            txtArguments.Text);
            */

            string args=" \""+dirs+"\" \""+ System.Reflection.Assembly.GetEntryAssembly().Location + "\" "
            +"\""+txtArguments.Text+"\"";

            string lmcu = chkSystemAccount.Checked ? " -lm " : " -cu " ;

            string windowsStartup=chkRunWindowsStartup.Checked?" -startupTrue ":" -startupFalse ";

            System.Diagnostics.Process.Start("\"" + Application.StartupPath + "\\FreeCombinePDFInstaller.exe\"", "-settings"+lmcu+windowsStartup+args);

            System.Diagnostics.Process.Start("\"" + Application.StartupPath + "\\FreeCombinePDFFolderWatcher.exe\"");

        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            SetRegistry();

            this.DialogResult = DialogResult.OK;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fb = new FolderBrowserDialog();
            if (fb.ShowDialog() == DialogResult.OK)
            {
                lstDirs.Items.Add(fb.SelectedPath);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (lstDirs.SelectedIndex != -1)
            {
                lstDirs.Items.RemoveAt(lstDirs.SelectedIndex);
            }
        }

        private void frmOptionsWatchers_Load(object sender, EventArgs e)
        {
            string watchfolders = RegistryHelper2.GetKeyValue("Free Combine Text 4dots", "WatchFolders");

            txtArguments.Text = RegistryHelper2.GetKeyValue("Free Combine Text 4dots", "ConvertArgs");

            if (watchfolders != string.Empty)
            {                               
                string[] dirz = watchfolders.Split(new string[] { "|||" }, StringSplitOptions.None);

                for (int k = 0; k < dirz.Length; k++)
                {
                    lstDirs.Items.Add(dirz[k]);
                }
            }

            chkRunWindowsStartup.Checked = false;

            chkLocalAccount.Checked = false;

            chkSystemAccount.Checked = false;

            RegistryKey key = Registry.CurrentUser;

            try
            {
                key = key.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true);

                if (key == null)
                {
                    Module.ShowMessage("Error. Could not specify if Application will start automatically with Windows");
                }

                if (key.GetValue("Free Combine Text 4dots") == null)
                {

                }
                else
                {
                    chkRunWindowsStartup.Checked = true;

                    chkLocalAccount.Checked = true;
                }
            }
            catch
            {
            }
            finally
            {
                if (key != null)
                {
                    key.Close();
                }
            }

            // ----------------------------          

            key = Registry.LocalMachine;

            try
            {
                key = key.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true);

                if (key == null)
                {
                    Module.ShowMessage("Error. Could not specify if Application will start automatically with Windows");
                }

                if (key.GetValue("Free Combine Text 4dots") == null)
                {                                                         
                }
                else
                {
                    chkRunWindowsStartup.Checked = true;

                    chkSystemAccount.Checked = true;
                }
            }
            catch
            {
            }
            finally
            {
                if (key != null)
                {
                    key.Close();
                }
            }
            // -------------------------------------


        }

        private void chkLocalAccount_CheckedChanged(object sender, EventArgs e)
        {
            if (chkLocalAccount.Checked)
            {
                chkSystemAccount.Checked = false;
            }
        }

        private void chkSystemAccount_CheckedChanged(object sender, EventArgs e)
        {
            if (chkSystemAccount.Checked)
            {
                chkLocalAccount.Checked = false;
            }
        }
    }
}
