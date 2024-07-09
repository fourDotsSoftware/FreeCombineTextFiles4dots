using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace FreeCombineTextFiles4dots
{ 
    class ArgsHelper
    {        
        public static bool ExamineArgs(string[] args)
        {
            if (args.Length == 0) return true;
                        
            Module.args = args;

            try
            {
                if (args[0].ToLower().Trim().StartsWith("-tempfile:"))
                {                                       
                    string tempfile = GetParameter(args[0]);

                    //MessageBox.Show(tempfile);

                    using (StreamReader sr = new StreamReader(tempfile, Encoding.Unicode))
                    {
                        string scont = sr.ReadToEnd();

                        //args = scont.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                        args = SplitArguments(scont);
                        Module.args = args;

                        // MessageBox.Show(scont);
                    }

                    Module.IsFromWindowsExplorer = true;
                }
                else if (args.Length>0 && (Module.args.Length==1 && (System.IO.File.Exists(Module.args[0]) || System.IO.Directory.Exists(Module.args[0]))))
                {

                }
                else
                {
                    Module.IsCommandLine = true;

                    //System.Windows.Forms.MessageBox.Show("0");

                    frmMain f=new frmMain();

                    string password = "";

                    frmMain.Instance.dt.Rows.Clear();

                    bool folderSep = false;

                    for (int k=0;k<Module.args.Length;k++)
                    {
                        if (Module.args[k].ToLower()=="/foldersep"
                            || Module.args[k].ToLower() == "-foldersep")
                        {
                            folderSep = true;
                        }
                    }

                    for (int k = 0; k < Module.args.Length; k++)
                    {
                        if (System.IO.File.Exists(Module.args[k]))
                        {
                            frmMain.Instance.AddFile(Module.args[k],password);

                            password = "";
                        }
                        else if (System.IO.Directory.Exists(Module.args[k]))
                        {
                            frmMain.Instance.SilentAdd = true;

                            frmMain.Instance.AddFolder(Module.args[k],password,folderSep);

                            password = "";
                        }                        
                        else if (Module.args[k].ToLower().Trim() == "-keepcreationdate"
                            || Module.args[k].ToLower().Trim() == "/keepcreationdate")
                        {
                            //frmMain.Instance.retainTimestampToolStripMenuItem.Checked = true;

                            Properties.Settings.Default.KeepCreationDate = true;                            

                            password = "";
                        }
                        else if (Module.args[k].ToLower().Trim() == "-keeplastmoddate"
                            || Module.args[k].ToLower().Trim() == "/keeplastmoddate")
                        {
                            //frmMain.Instance.retainTimestampToolStripMenuItem.Checked = true;
                                                        
                            Properties.Settings.Default.KeepLastModificationDate = true;

                            password = "";
                        }
                        else if (Module.args[k].ToLower().StartsWith("/outputfile:") ||
    Module.args[k].ToLower().StartsWith("-outputfile:"))
                        {
                            string outfile = GetParameter(Module.args[k]);

                            //3RecentFilesHelper.AddRecentOutputFile(outfile);

                            //3frmMain.Instance.cmbOutputDir.SelectedIndex = 0;

                            //Module.OutputFilepath = outfile;

                            frmMain.Instance.OutputFilepath = outfile;

                            password = "";
                        }
                        else if (Module.args[k].ToLower().StartsWith("/outputfolder:") ||
        Module.args[k].ToLower().StartsWith("-outputfolder:"))
                        {
                            string outfolder = GetParameter(Module.args[k]);

                            //3frmMain.Instance.cmbOutputDir.Text = outfolder;

                            password = "";
                        }
                        else
                        {
                            password = RemoveQuotes(Module.args[k]);
                        }
                    }                                      
                }
            }
            catch (Exception ex)
            {
                Module.ShowError("Error could not parse Arguments !", ex.ToString());
                return false;
            }

            return true;
        }

        private static string RemoveQuotes(string str)
        {
            if ((str.StartsWith("\"") && str.EndsWith("\"")) ||
                    (str.StartsWith("'") && str.EndsWith("'")))
            {
                if (str.Length > 2)
                {
                    str = str.Substring(1, str.Length - 2);
                }
                else
                {
                    str = "";
                }
            }

            return str;
        }

        private static string GetParameter(string arg)
        {
            int spos = arg.IndexOf(":");
            if (spos == arg.Length - 1) return "";
            else
            {
                string str=arg.Substring(spos + 1);

                if ((str.StartsWith("\"") && str.EndsWith("\"")) ||
                    (str.StartsWith("'") && str.EndsWith("'")))
                {
                    if (str.Length > 2)
                    {
                        str = str.Substring(1, str.Length - 2);
                    }
                    else
                    {
                        str = "";
                    }
                }

                return str;
            }
        }

        public static string[] SplitArguments(string commandLine)
        {
            char[] parmChars = commandLine.ToCharArray();
            bool inSingleQuote = false;
            bool inDoubleQuote = false;
            for (int index = 0; index < parmChars.Length; index++)
            {
                if (parmChars[index] == '"' && !inSingleQuote)
                {
                    inDoubleQuote = !inDoubleQuote;
                    parmChars[index] = '\n';
                }
                if (parmChars[index] == '\'' && !inDoubleQuote)
                {
                    inSingleQuote = !inSingleQuote;
                    parmChars[index] = '\n';
                }
                if (!inSingleQuote && !inDoubleQuote && parmChars[index] == ' ')
                    parmChars[index] = '\n';
            }
            return (new string(parmChars)).Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
        }

        public static void ShowCommandUsage()
        {
            string msg = GetCommandUsage();

            Module.ShowMessage(msg);

            Environment.Exit(0);
        }
        public static string GetCommandUsage()
        {
            string msg = "Combines multiple Text documents into a single Text document.\n\n" +
            "FreeTextFileJoiner4dots.exe [[passsword] [file|directory]]\n" +
            "[/cmd]\n" +
            "[/foldersep]"+
            "[/keeptimestamp]\n"+                                    
            "[/outputfile:OUTPUT_FILEPATH]\n"+            
            "[/?]\n\n\n" +
            "cmd : use the command line\n" +
            "file : one or more Text documents to be processed.\n" +
            "directory : one or more directories containing files to be processed.\n" +                                                           
            "outputfile  : (optional) the combined Word document output filepath.\n"+   
            "foldersep : join each file of the folder separately (intro + file + outro).\n"+
            "keepcreationdate : (optional) keep file creation date.\n" +
            "keeplastmoddate : (optional) keep file last modification date.\n" +
            "/? : show help\n\n\n" +
            "Example :\n" +
            "FreeTextFileJoiner4dots.exe \"c:\\documents\\invoice.txt\" \"c:\\documents\\invoice2.txt\" " +
            " \"c:\\documents\\invoice3.txt\" /outputfile:\"c:\\documents\\merged.txt\" ";

            return msg;
        }

        public static bool IsFromFolderWatcher
        {
            get
            {                
                // new
                if (Module.args.Length > 0 && Module.args[0].ToLower().Trim() == "/cmdfw")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public static bool IsFromWindowsExplorer
        {
            get
            {
                if (Module.IsFromWindowsExplorer) return true;

                // new
                if (Module.args.Length > 0 && (Module.args[0].ToLower().Trim().Contains("-tempfile:")
                    || (Module.args.Length==1 && (System.IO.File.Exists(Module.args[0]) || System.IO.Directory.Exists(Module.args[0])))))
                {
                    Module.IsFromWindowsExplorer = true;
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public static bool IsFromCommandLine
        {
            get
            {
                if (Module.args == null || Module.args.Length == 0)
                {
                    return false;
                }

                if (ArgsHelper.IsFromWindowsExplorer)
                {
                    Module.IsCommandLine = false;
                    return false;
                }
                else
                {
                    Module.IsCommandLine = true;
                    return true;
                }
            }
        }

        /*
        public static bool IsFromWindowsExplorer()
        {
            if (Module.args == null || Module.args.Length == 0)
            {
                return false;
            }

            for (int k = 0; k < Module.args.Length; k++)
            {
                if (Module.args[k] == "-visual")
                {
                    Module.IsFromWindowsExplorer = true;
                    return true;
                }
            }

            Module.IsFromWindowsExplorer = false;
            return false;
        }
        */

        public static void ExecuteCommandLine()
        {
            string err = "";
            bool finished = false;

            try
            {
                /*
                if (Module.CmdLogFile != string.Empty)
                {
                    try
                    {
                        Module.CmdLogFileWriter = new StreamWriter(Module.CmdLogFile, true);
                        Module.CmdLogFileWriter.AutoFlush = true;
                        Module.CmdLogFileWriter.WriteLine("[" + DateTime.Now.ToString() + "] Started compressing PDF files !");
                    }
                    catch (Exception exl)
                    {
                        Module.ShowMessage("Error could not start log writer !");
                        ShowCommandUsage();
                        Environment.Exit(0);
                        return;
                    }
                }                

                if (Module.CmdImportListFile != string.Empty)
                {
                    frmMain.Instance.ImportList(Module.CmdImportListFile);

                    err += frmMain.Instance.SilentAddErr;

                }
                */

                if (Module.args[0].ToLower() == "/h" ||
                        Module.args[0].ToLower() == "-h" ||
                        Module.args[0].ToLower() == "-?" ||
                        Module.args[0].ToLower() == "/?")
                {
                    ShowCommandUsage();
                    Environment.Exit(1);
                    return;
                }

                if (frmMain.Instance.dt.Rows.Count == 0)
                {
                    Module.ShowMessage("Please specify PDF Files to combine !");
                    ShowCommandUsage();
                    Environment.Exit(0);
                    return;
                }
                
                frmMain.Instance.tsbMergeDocuments_Click(null,null);
                
            }
            finally
            {
                
            }
            Environment.Exit(0);
        }

        
                
                
    }

    public class ReadListsResult
    {
        public bool Success = true;
        public string err = "";
    }
}
