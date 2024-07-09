using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace FreeCombinePDFFolderWatcher
{
    public partial class frmMain : Form
    {       
        public string AppFilepath = "";
        public string ConvertArgs = "";

        public List<System.IO.FileSystemWatcher> fws = new List<System.IO.FileSystemWatcher>();

        public List<string> Filepaths = new List<string>();

        public bool ForCurrentUser = false;

        public frmMain(bool forCurrentUser)
        {
            InitializeComponent();

            ForCurrentUser = forCurrentUser;

            this.Visible = false;

            this.Hide();

            if (ForCurrentUser)
            {
                string watchfolders = RegistryHelper2.GetKeyValue("Free Combine Text Files 4dots", "WatchFolders");

                if (watchfolders != string.Empty)
                {
                    AppFilepath = RegistryHelper2.GetKeyValue("Free Combine Text Files 4dots", "AppFilepath");

                    ConvertArgs = RegistryHelper2.GetKeyValue("Free Combine Text Files 4dots", "ConvertArgs");

                    string[] dirz = watchfolders.Split(new string[] { "|||" }, StringSplitOptions.RemoveEmptyEntries);

                    for (int k = 0; k < dirz.Length; k++)
                    {
                        System.IO.FileSystemWatcher fw = new System.IO.FileSystemWatcher(dirz[k]);
                        fw.Created += fw_Created;
                        fw.IncludeSubdirectories = true;
                        fw.EnableRaisingEvents = true;

                        fws.Add(fw);
                    }
                }
            }
            else
            {
                string watchfolders = RegistryHelper2.GetKeyValueLMLowPriv("Free Combine Text Files 4dots", "WatchFolders");

                if (watchfolders != string.Empty)
                {
                    AppFilepath = RegistryHelper2.GetKeyValueLMLowPriv("Free Combine Text Files 4dots", "AppFilepath");

                    ConvertArgs = RegistryHelper2.GetKeyValueLMLowPriv("Free Combine Text Files 4dots", "ConvertArgs");

                    string[] dirz = watchfolders.Split(new string[] { "|||" }, StringSplitOptions.RemoveEmptyEntries);

                    for (int k = 0; k < dirz.Length; k++)
                    {
                        System.IO.FileSystemWatcher fw = new System.IO.FileSystemWatcher(dirz[k]);
                        fw.Created += fw_Created;
                        fw.IncludeSubdirectories = true;
                        fw.EnableRaisingEvents = true;

                        fws.Add(fw);
                    }
                }
            }

            this.Hide();
            this.Visible = false;
        }

        private void frmMain_Activated(object sender, EventArgs e)
        {
            this.Hide();
            this.Visible = false;
        }

        private int FileCreated = 0;

        void fw_Created(object sender, System.IO.FileSystemEventArgs e)
        {
            if (IsDocument(e.FullPath))
            {
                FileCreated++;

                int filecreated = FileCreated;

                int interval = 120 * 1000;

                Stopwatch sw = new Stopwatch();
                sw.Start();

                while (sw.ElapsedMilliseconds <= interval)
                {
                    Application.DoEvents();
                }


                sw.Stop();

                if (filecreated != FileCreated)
                {
                    return;
                }
                else
                {
                    try
                    {
                        string[] pdfs = System.IO.Directory.GetFiles(System.IO.Path.GetDirectoryName(e.FullPath), "*.pdf",System.IO.SearchOption.TopDirectoryOnly);

                        string spdf = "";

                        List<string> lstpdf = new List<string>();

                        for (int k = 0; k < pdfs.Length; k++)
                        {
                            if (!pdfs[k].ToLower().EndsWith("_merged.pdf"))
                            {
                                lstpdf.Add(pdfs[k]);
                            }
                        }

                        lstpdf.Sort();

                        for (int k = 0; k < lstpdf.Count; k++)
                        {
                            spdf += " \"" + lstpdf[k] + "\"";
                        }

                        Process proc = new Process();

                        if (!AppFilepath.StartsWith("\""))
                        {
                            AppFilepath = "\"" + AppFilepath + "\"";
                        }

                        proc.StartInfo.FileName = AppFilepath;

                        proc.StartInfo.Arguments = "/cmd " + spdf + " " +
                        ConvertArgs + " /outputfile:\"" + System.IO.Path.GetFileNameWithoutExtension(lstpdf[0]) + "_merged.pdf" + "\"";

                        proc.StartInfo.UseShellExecute = false;
                        proc.StartInfo.CreateNoWindow = true;

                        proc.Start();
                        proc.WaitForExit();
                    }
                    catch (Exception ex)
                    {
                    }
                }
            }
        }

        public static string AcceptableMediaInputPattern = "*.pdf;";

        public static bool IsDocument(string filepath)
        {
            if (filepath.ToLower().Trim().EndsWith(".pdf"))
            {
                return true;
            }
            else
            {
                return false;
            }            
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            this.Visible = false;
            this.Hide();
        }

    }
}
