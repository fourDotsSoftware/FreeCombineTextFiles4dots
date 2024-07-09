using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Win32;
using System.Windows.Forms;

namespace FreeCombinePDFInstaller
{
    class Program
    {
        static int Main(string[] args)
        {            
            if (args[0] == "-i")
            {
                System.Diagnostics.Process.Start("\"" + args[1] + "\"", "\"" + args[2] + "\"");

                //System.Diagnostics.Process.Start( args[1],  args[2]);
                /*
                System.Windows.Forms.MessageBox.Show("i","i", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information, System.Windows.Forms.MessageBoxDefaultButton.Button1, System.Windows.Forms.MessageBoxOptions.ServiceNotification);
                return ServiceHandler.Install();*/
            }
            else if (args[0] == "-u")
            {
                System.Diagnostics.Process.Start("\"" + args[1] + "\"", "\"" + args[2] + "\"");
                //return ServiceHandler.Uninstall();
            }
            else if (args[0] == "-settings")
            {
                //System.Windows.Forms.MessageBox.Show("set", "set", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information, System.Windows.Forms.MessageBoxDefaultButton.Button1, System.Windows.Forms.MessageBoxOptions.ServiceNotification);

                bool cu = true;
                
                if (args[1].Trim() == "-lm")
                {
                    cu = false;
                }

                bool startupWindows = false;

                if (args[2].Trim() == "-startupTrue")
                {
                    startupWindows = true;
                }
                
                RegistryKey key = Registry.CurrentUser;

                try
                {
                    key = key.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true);

                    if (key != null)
                    {
                        if (startupWindows && cu)
                        {
                            if (key.GetValue("Free Combine Text Files 4dots") != null)
                            {
                                key.DeleteValue("Free Combine Text Files 4dots");
                            }

                            if (key.GetValue("Free Combine Text Files 4dots") == null)
                            {
                                key.SetValue("Free Combine Text Files 4dots", "\"" + Application.StartupPath + "\\FreeCombinePDFFolderWatcher.exe\" -cu");
                            }
                        }
                        else
                        {
                            if (key.GetValue("Free Combine Text Files 4dots") != null)
                            {
                                key.DeleteValue("Free Combine Text Files 4dots");
                            }
                        }
                    }
                }
                catch (Exception ex1)
                {
                }
                finally
                {
                    if (key != null)
                    {
                        key.Close();
                    }
                }

                key = Registry.LocalMachine;                

                try
                {
                    key = key.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true);

                    if (key != null)
                    {
                        if (startupWindows && cu)
                        {
                            if (key.GetValue("Free Combine Text Files 4dots") != null)
                            {
                                key.DeleteValue("Free Combine Text Files 4dots");
                            }

                            if (key.GetValue("Free Combine Text Files 4dots") == null)
                            {
                                key.SetValue("Free Combine Text Files 4dots", "\"" + Application.StartupPath + "\\FreeCombinePDFFolderWatcher.exe\" -lm");
                            }
                        }
                        else
                        {
                            if (key.GetValue("Free Combine Text Files 4dots") != null)
                            {
                                key.DeleteValue("Free Combine Text Files 4dots");
                            }
                        }
                    }
                }
                catch (Exception ex1)
                {
                }
                finally
                {
                    if (key != null)
                    {
                        key.Close();
                    }
                }

                if (cu)
                {
                    RegistryHelper2.SetKeyValue("Free Combine Text Files 4dots", "WatchFolders", args[3]);
                    RegistryHelper2.SetKeyValue("Free Combine Text Files 4dots", "AppFilepath", args[4]);
                    RegistryHelper2.SetKeyValue("Free Combine Text Files 4dots", "ConvertArgs", args[5]);
                }
                else
                {
                    RegistryHelper2.SetKeyValueLM("Free Combine Text Files 4dots", "WatchFolders", args[3]);
                    RegistryHelper2.SetKeyValueLM("Free Combine Text Files 4dots", "AppFilepath", args[4]);
                    RegistryHelper2.SetKeyValueLM("Free Combine Text Files 4dots", "ConvertArgs", args[5]);
                }
                return 0;
            }
            else
            {
                return -1;
            }

            return 0;
        }
    }
}
