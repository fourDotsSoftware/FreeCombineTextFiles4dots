using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;

namespace FreeCombineTextFiles4dots
{
    public partial class frmMain : FreeCombineTextFiles4dots.CustomForm
    {
        public static frmMain Instance = null;

        public bool SilentAdd = false;
        public string SilentAddErr = "";

        public bool OperationStopped = false;
        public bool OperationPaused = false;

        public string Err = "";

        private string sOutputDir = "";
        private bool bKeepBackup = false;

        public string FirstOutputDocument = "";

        public string FirstOutputFilepath = "";

        public int[] de = new int[5];

        public BackgroundWorker bwWork = new BackgroundWorker();

        private bool Success = false;

        private string OutputDir = "";

        private string FirstFilepath = "";

        private string FilenamePattern = "";

        private string DateFormat = "";

        private string CreationDate = "";

        private string ModDate = "";

        private string OutputFilename = "";

        public string OutputFilepath = "";

        public frmMain()
        {
            InitializeComponent();

            if (!Properties.Settings.Default.Initialized)
            {
                Properties.Settings.Default.TxtHeader= "===============================================================================\r\n"+
"File: $Filename\r\n" +
"Folder : $FolderPath\r\n" +
"Creation Date: $CreationDate\r\n" +
"Last Modification Date : $LastModificationDate\r\n" +
"Size KBytes: $SizeKBytes\r\n" +
"Number of Words : $NumberOfWords\r\n" +
"Number of Lines : $NumberOfLines\r\n" +
"File Number: $FileNumber\r\n" +
"Total Files: $TotalFiles\r\n" +
"===============================================================================\r\n";

                Properties.Settings.Default.TxtFooter = "=============================EOF===============================================";

                Properties.Settings.Default.Initialized = true;

                Properties.Settings.Default.Save();
            }

            bwWork.DoWork += bwWork_DoWork;
            bwWork.WorkerReportsProgress = true;
            bwWork.ProgressChanged += bwWork_ProgressChanged;

            bwWork.WorkerSupportsCancellation = true;

            Instance = this;

            dt.Columns.Add("filename");
            dt.Columns.Add("password");
            dt.Columns.Add("slideranges");
            dt.Columns.Add("sizekb");
            dt.Columns.Add("fullfilepath");
            dt.Columns.Add("filedate");
            dt.Columns.Add("rootfolder");

            dt.Columns.Add("foldersep",typeof(bool));
            dt.Columns.Add("folderfiles");

            dgFiles.AutoGenerateColumns = false;

            dtClipboard = dt.Clone();

            for (int k = 0; k < de.Length; k++)
            {
                de[k] = 0;
            }

            if (Module.IsCommandLine)
            {
                this.Visible = false;                

                frmMain_Load(null, null);

                //ArgsHelper.ExamineArgs(Module.args);

                //ArgsHelper.ExecuteCommandLine();

                //Environment.Exit(0);

                return;
            }
            else if (Module.IsFromWindowsExplorer)
            {
                dt.Rows.Clear();
                //this.Visible = false;                

                frmMain_Load(null, null);

                //ArgsHelper.ExamineArgs(Module.args);

                for (int k = 0; k < Module.args.Length; k++)
                {
                    if (System.IO.File.Exists(Module.args[k]))
                    {
                        AddFile(Module.args[k]);
                    }
                    else if (System.IO.Directory.Exists(Module.args[k]))
                    {
                        AddFolder(Module.args[k]);
                    }
                }

                tsbMergeDocuments_Click(null, null);

                Environment.Exit(0);

                return;
            }            
        }

        void bwWork_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.ProgressPercentage==30)
            {
                int max = (int)e.UserState;

                frmProgress.Instance.progressBar1.Value = 0;
                frmProgress.Instance.progressBar1.Maximum = max;
            }
            else if (e.ProgressPercentage != 10)
            {
                int val = frmProgress.Instance.progressBar1.Value;

                if ((val + 1) <= frmProgress.Instance.progressBar1.Maximum)
                {
                    frmProgress.Instance.progressBar1.Value = frmProgress.Instance.progressBar1.Value + 1;
                }
            }

            frmProgress.Instance.lblOutputFile.Text = e.UserState.ToString();

            frmProgress.Instance.lblJoinNumber.Text =
                frmProgress.Instance.progressBar1.Value.ToString()
                + " / " +
                frmProgress.Instance.progressBar1.Maximum.ToString();
        }

        void bwWork_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {

                if (!Properties.Settings.Default.EachRootFolder && !Properties.Settings.Default.EachSubFolder)
                {
                    string folderfiles = "";

                    for (int k = 0; k < dt.Rows.Count; k++)
                    {
                        if (dt.Rows[k]["foldersep"].ToString() == bool.TrueString)
                        {
                            folderfiles = dt.Rows[k]["folderfiles"].ToString();
                            break;
                        }
                    }

                    string[] folderf = null;

                    if (folderfiles != string.Empty)
                    {
                        folderf = folderfiles.Split(new string[] { "|||" }, StringSplitOptions.RemoveEmptyEntries);

                        bwWork.ReportProgress(30, folderf.Length);

                        //frmProgress.Instance.progressBar1.Value = 0;
                        //frmProgress.Instance.progressBar1.Maximum = folderf.Length;
                    }

                    int folderk = 0;

                    while (true)
                    {
                        string outfilepath = "";

                        string firstFilepath = dt.Rows[0]["fullfilepath"].ToString();

                        if (folderfiles != string.Empty)
                        {
                            firstFilepath = folderf[folderk];
                        }

                        FileInfo fi = new FileInfo(firstFilepath);

                        CreationDate = fi.CreationTime.ToString(txtDatePatternText);

                        ModDate = fi.LastWriteTime.ToString(txtDatePatternText);

                        string curdate = DateTime.Now.ToString(txtDatePatternText);

                        string firstfilename = System.IO.Path.GetFileNameWithoutExtension(firstFilepath);

                        OutputFilenameCreator of = new OutputFilenameCreator(FirstFilepath);

                        OutputFilename = of.Value;

                        /*
                        OutputFilename = txtFilenameText.Replace("[FILENAME]", firstfilename).Replace("[MODDATE]", ModDate)
                            .Replace("[CREATIONDATE]", CreationDate).Replace("[CURDATE]", curdate)
                            + System.IO.Path.GetExtension(FirstFilepath);
                        */
                        if (OutputDir.Trim() == TranslateHelper.Translate("Same Folder of Text Document"))
                        {
                            string dirpath = System.IO.Path.GetDirectoryName(firstFilepath);

                            outfilepath = System.IO.Path.Combine(dirpath, OutputFilename);
                        }
                        else if (OutputDir.StartsWith(TranslateHelper.Translate("Subfolder") + " : "))
                        {
                            int subfolderspos = (TranslateHelper.Translate("Subfolder") + " : ").Length;
                            string subfolder = OutputDir.Substring(subfolderspos);

                            outfilepath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(firstFilepath) + "\\" + subfolder, OutputFilename);

                            if (!System.IO.Directory.Exists(System.IO.Path.GetDirectoryName(outfilepath)))
                            {
                                System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(outfilepath));
                            }
                        }
                        else
                        {
                            outfilepath = System.IO.Path.Combine(OutputDir, OutputFilename);
                        }

                        if (!(Module.IsCommandLine && frmMain.Instance.OutputFilepath != string.Empty))
                        {
                            OutputFilepath = outfilepath;
                        }

                        DataTable dt2 = dt.Clone();

                        for (int m = 0; m < dt.Rows.Count; m++)
                        {
                            if (dt.Rows[m]["foldersep"].ToString() != bool.TrueString)
                            {
                                DataRow dr = dt2.NewRow();

                                for (int n = 0; n < dt.Columns.Count; n++)
                                {
                                    dr[n] = dt.Rows[m][n];
                                }

                                dt2.Rows.Add(dr);
                            }
                            else
                            {
                                DataRow dr = dt2.NewRow();
                                dr["fullfilepath"] = firstFilepath;
                                dr["filename"] = firstfilename;

                                dt2.Rows.Add(dr);
                            }
                        }

                        if (folderfiles != string.Empty)
                        {
                            frmMain.Instance.bwWork.ReportProgress(10, System.IO.Path.GetFileName(firstFilepath));

                            try
                            {
                                bool suc = FreeCombineTextHelper.FreeCombineTextFiles(dt2, outfilepath, true);
                                Success = suc;

                                if (System.IO.File.Exists(outfilepath))
                                {
                                    FileInfo fi2 = new FileInfo(outfilepath);

                                    if (Properties.Settings.Default.KeepCreationDate)
                                    {
                                        fi2.CreationTime = fi.CreationTime;
                                    }

                                    if (Properties.Settings.Default.KeepLastModificationDate)
                                    {
                                        fi2.LastWriteTime = fi.LastWriteTime;
                                    }
                                }
                            }
                            catch (Exception exd)
                            {
                                Err += TranslateHelper.Translate("Error could not combine Files !") + " : " + outfilepath + "\r\n" + exd.Message + "\r\n";
                            }

                            frmMain.Instance.bwWork.ReportProgress(20, System.IO.Path.GetFileName(firstFilepath));
                        }
                        else
                        {
                            if (Properties.Settings.Default.EachFileExtension)
                            {
                                Dictionary<string, DataTable> lstde = new Dictionary<string, DataTable>();

                                for (int m = 0; m < dt.Rows.Count; m++)
                                {
                                    string ext = System.IO.Path.GetExtension(dt.Rows[m]["fullfilepath"].ToString().ToLower());

                                    if (!lstde.ContainsKey(ext))
                                    {
                                        DataTable dte = dt.Clone();

                                        lstde.Add(ext, dte);
                                    }

                                    DataRow dr = lstde[ext].NewRow();

                                    for (int j = 0; j < dt.Columns.Count; j++)
                                    {
                                        dr[j] = dt.Rows[m][j];
                                    }

                                    lstde[ext].Rows.Add(dr);

                                }

                                foreach (KeyValuePair<string, DataTable> dtm in lstde)
                                {
                                    try
                                    {
                                        DataTable dtf = dtm.Value;
                                        string ext = dtm.Key;

                                        OutputFilenameCreator of1 = new OutputFilenameCreator(dtf.Rows[0]["fullfilepath"].ToString());

                                        OutputFilename = of1.Value;

                                        firstFilepath = dtf.Rows[0]["fullfilepath"].ToString();

                                        if (OutputDir.Trim() == TranslateHelper.Translate("Same Folder of Text Document"))
                                        {
                                            string dirpath = System.IO.Path.GetDirectoryName(firstFilepath);

                                            outfilepath = System.IO.Path.Combine(dirpath, OutputFilename);
                                        }
                                        else if (OutputDir.StartsWith(TranslateHelper.Translate("Subfolder") + " : "))
                                        {
                                            int subfolderspos = (TranslateHelper.Translate("Subfolder") + " : ").Length;
                                            string subfolder = OutputDir.Substring(subfolderspos);

                                            outfilepath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(firstFilepath) + "\\" + subfolder, OutputFilename);

                                            if (!System.IO.Directory.Exists(System.IO.Path.GetDirectoryName(outfilepath)))
                                            {
                                                System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(outfilepath));
                                            }
                                        }
                                        else
                                        {
                                            outfilepath = System.IO.Path.Combine(OutputDir, OutputFilename);
                                        }

                                        OutputFilepath = outfilepath;

                                        bool suc1 = FreeCombineTextHelper.FreeCombineTextFiles(dtf, OutputFilepath, true);

                                        Success = suc1;

                                        if (System.IO.File.Exists(OutputFilepath))
                                        {
                                            FileInfo fi1 = new FileInfo(dtf.Rows[0]["fullfilepath"].ToString());

                                            FileInfo fi2 = new FileInfo(OutputFilepath);

                                            if (Properties.Settings.Default.KeepCreationDate)
                                            {
                                                fi2.CreationTime = fi1.CreationTime;
                                            }

                                            if (Properties.Settings.Default.KeepLastModificationDate)
                                            {
                                                fi2.LastWriteTime = fi1.LastWriteTime;
                                            }
                                        }

                                        if (FirstOutputFilepath == string.Empty)
                                        {
                                            FirstOutputFilepath = OutputFilepath;
                                        }
                                    }
                                    catch (Exception exd)
                                    {
                                        Err += TranslateHelper.Translate("Error could not combine Files !") + " : " + OutputFilepath + "\r\n" + exd.Message + "\r\n";
                                    }
                                }
                            }
                            else
                            {
                                try
                                {
                                    bool suc = FreeCombineTextHelper.FreeCombineTextFiles(dt, outfilepath, false);
                                    Success = suc;

                                    if (System.IO.File.Exists(outfilepath))
                                    {
                                        FileInfo fi2 = new FileInfo(outfilepath);

                                        if (Properties.Settings.Default.KeepCreationDate)
                                        {
                                            fi2.CreationTime = fi.CreationTime;
                                        }

                                        if (Properties.Settings.Default.KeepLastModificationDate)
                                        {
                                            fi2.LastWriteTime = fi.LastWriteTime;
                                        }
                                    }
                                }
                                catch (Exception exd)
                                {
                                    Err += TranslateHelper.Translate("Error could not combine Files !") + " : " + outfilepath + "\r\n" + exd.Message + "\r\n";
                                }
                            }

                        }

                        folderk++;

                        if (folderfiles == string.Empty)
                        {
                            break;
                        }
                        else if (folderk >= folderf.Length)
                        {
                            break;
                        }
                    }
                }
                else if (Properties.Settings.Default.EachRootFolder)
                {
                    Dictionary<string, DataTable> lstdt = new Dictionary<string, DataTable>();

                    for (int k = 0; k < dt.Rows.Count; k++)
                    {
                        string dir1 = System.IO.Path.GetDirectoryName(dt.Rows[k]["fullfilepath"].ToString()).ToLower();

                        string dir = "";

                        int minchars = 0;

                        for (int d = 0; d < dt.Rows.Count; d++)
                        {
                            string dir2 = System.IO.Path.GetDirectoryName(dt.Rows[d]["fullfilepath"].ToString()).ToLower();

                            if (dir1.StartsWith(dir2))
                            {
                                if ((minchars == 0) || (dir2.Length < minchars))
                                {
                                    dir = dir2;

                                    minchars = dir2.Length;
                                }
                            }
                        }

                        if (dir == string.Empty)
                        {
                            dir = dir1;
                        }

                        if (!lstdt.ContainsKey(dir))
                        {
                            DataTable dte = dt.Clone();

                            lstdt.Add(dir, dte);
                        }

                        DataRow dr = lstdt[dir].NewRow();

                        for (int j = 0; j < dt.Columns.Count; j++)
                        {
                            dr[j] = dt.Rows[k][j];
                        }

                        lstdt[dir].Rows.Add(dr);
                    }

                    bwWork.ReportProgress(30, lstdt.Count);                                        

                    if (Properties.Settings.Default.EachFileExtension)
                    {
                        foreach (KeyValuePair<string, DataTable> dtl in lstdt)
                        {
                            Dictionary<string, DataTable> lstde = new Dictionary<string, DataTable>();

                            string dirname = System.IO.Path.GetFileName(dtl.Key);

                            DataTable dt2 = dtl.Value;

                            for (int m = 0; m < dt2.Rows.Count; m++)
                            {
                                string ext = System.IO.Path.GetExtension(dt2.Rows[m]["fullfilepath"].ToString().ToLower());

                                if (!lstde.ContainsKey(ext))
                                {
                                    DataTable dte = dt.Clone();

                                    lstde.Add(ext, dte);
                                }

                                DataRow dr = lstde[ext].NewRow();

                                for (int j = 0; j < dt2.Columns.Count; j++)
                                {
                                    dr[j] = dt2.Rows[m][j];
                                }

                                lstde[ext].Rows.Add(dr);

                            }

                            foreach (KeyValuePair<string, DataTable> dtm in lstde)
                            {
                                try
                                {
                                    DataTable dtf = dtm.Value;
                                    string ext = dtm.Key;

                                    string dir = System.IO.Path.GetDirectoryName(dtf.Rows[0]["fullfilepath"].ToString());

                                    string dirpar = System.IO.Path.GetDirectoryName(dir);

                                    OutputFilenameCreator of = new OutputFilenameCreator(dtf.Rows[0]["fullfilepath"].ToString(), dir);

                                    string dir3 = of.Value;

                                    //OutputFilepath = System.IO.Path.Combine(dirpar, System.IO.Path.GetFileName(dir) + ext);

                                    OutputFilepath = System.IO.Path.Combine(dirpar, dir3);

                                    bool suc1 = FreeCombineTextHelper.FreeCombineTextFiles(dtf, OutputFilepath, true);

                                    Success = suc1;

                                    if (System.IO.File.Exists(OutputFilepath))
                                    {
                                        FileInfo fi = new FileInfo(dtf.Rows[0]["fullfilepath"].ToString());

                                        FileInfo fi2 = new FileInfo(OutputFilepath);

                                        if (Properties.Settings.Default.KeepCreationDate)
                                        {
                                            fi2.CreationTime = fi.CreationTime;
                                        }

                                        if (Properties.Settings.Default.KeepLastModificationDate)
                                        {
                                            fi2.LastWriteTime = fi.LastWriteTime;
                                        }
                                    }

                                    if (FirstOutputFilepath == string.Empty)
                                    {
                                        FirstOutputFilepath = OutputFilepath;
                                    }
                                }
                                catch (Exception exd)
                                {
                                    Err += TranslateHelper.Translate("Error could not combine Files !") + " : " + OutputFilepath + "\r\n" + exd.Message + "\r\n";
                                }


                            }

                            frmMain.Instance.bwWork.ReportProgress(0, dirname);
                        }
                    }
                    else
                    {
                        foreach (KeyValuePair<string, DataTable> dtl in lstdt)
                        {
                            string dirname = System.IO.Path.GetFileName(dtl.Key);

                            try
                            {
                                DataTable dtf = dtl.Value;
                                //string ext = dtl.Key;
                                string ext = System.IO.Path.GetExtension(dtf.Rows[0]["fullfilepath"].ToString());

                                string dir = System.IO.Path.GetDirectoryName(dtf.Rows[0]["fullfilepath"].ToString());

                                string dirpar = System.IO.Path.GetDirectoryName(dir);

                                OutputFilenameCreator of = new OutputFilenameCreator(dtf.Rows[0]["fullfilepath"].ToString(), dir);

                                string dir3 = of.Value;

                                //OutputFilepath = System.IO.Path.Combine(dirpar, System.IO.Path.GetFileName(dir) + ext);

                                OutputFilepath = System.IO.Path.Combine(dirpar, dir3);

                                //OutputFilepath = System.IO.Path.Combine(dirpar, System.IO.Path.GetFileName(dir) + ext);

                                bool suc1 = FreeCombineTextHelper.FreeCombineTextFiles(dtf, OutputFilepath, true);

                                Success = suc1;

                                if (System.IO.File.Exists(OutputFilepath))
                                {
                                    FileInfo fi = new FileInfo(dtf.Rows[0]["fullfilepath"].ToString());

                                    FileInfo fi2 = new FileInfo(OutputFilepath);

                                    if (Properties.Settings.Default.KeepCreationDate)
                                    {
                                        fi2.CreationTime = fi.CreationTime;
                                    }

                                    if (Properties.Settings.Default.KeepLastModificationDate)
                                    {
                                        fi2.LastWriteTime = fi.LastWriteTime;
                                    }
                                }

                                if (FirstOutputFilepath == string.Empty)
                                {
                                    FirstOutputFilepath = OutputFilepath;
                                }
                            }
                            catch (Exception exd)
                            {
                                Err += TranslateHelper.Translate("Error could not combine Files !") + " : " + OutputFilepath + "\r\n" + exd.Message + "\r\n";
                            }

                            frmMain.Instance.bwWork.ReportProgress(0, dirname);
                        }
                    }
                }
                else if (Properties.Settings.Default.EachSubFolder)
                {
                    Dictionary<string, DataTable> lstdt = new Dictionary<string, DataTable>();

                    for (int k = 0; k < dt.Rows.Count; k++)
                    {
                        string dir = System.IO.Path.GetDirectoryName(dt.Rows[k]["fullfilepath"].ToString()).ToLower();

                        if (!lstdt.ContainsKey(dir))
                        {
                            DataTable dte = dt.Clone();

                            lstdt.Add(dir, dte);
                        }

                        DataRow dr = lstdt[dir].NewRow();

                        for (int j = 0; j < dt.Columns.Count; j++)
                        {
                            dr[j] = dt.Rows[k][j];
                        }

                        lstdt[dir].Rows.Add(dr);
                    }

                    bwWork.ReportProgress(30, lstdt.Count);
                    
                    if (Properties.Settings.Default.EachFileExtension)
                    {
                        foreach (KeyValuePair<string, DataTable> dtl in lstdt)
                        {
                            string dirname = System.IO.Path.GetFileName(dtl.Key);

                            Dictionary<string, DataTable> lstde = new Dictionary<string, DataTable>();

                            DataTable dt2 = dtl.Value;

                            for (int m = 0; m < dt2.Rows.Count; m++)
                            {
                                string ext = System.IO.Path.GetExtension(dt2.Rows[m]["fullfilepath"].ToString().ToLower());

                                if (!lstde.ContainsKey(ext))
                                {
                                    DataTable dte = dt.Clone();

                                    lstde.Add(ext, dte);
                                }

                                DataRow dr = lstde[ext].NewRow();

                                for (int j = 0; j < dt2.Columns.Count; j++)
                                {
                                    dr[j] = dt2.Rows[m][j];
                                }

                                lstde[ext].Rows.Add(dr);

                            }

                            foreach (KeyValuePair<string, DataTable> dtm in lstde)
                            {
                                try
                                {
                                    DataTable dtf = dtm.Value;
                                    string ext = dtm.Key;

                                    string dir = System.IO.Path.GetDirectoryName(dtf.Rows[0]["fullfilepath"].ToString());

                                    string dirpar = System.IO.Path.GetDirectoryName(dir);

                                    OutputFilenameCreator of = new OutputFilenameCreator(dtf.Rows[0]["fullfilepath"].ToString(), dir);

                                    string dir3 = of.Value;

                                    //OutputFilepath = System.IO.Path.Combine(dirpar, System.IO.Path.GetFileName(dir) + ext);

                                    OutputFilepath = System.IO.Path.Combine(dirpar, dir3);

                                    //OutputFilepath = System.IO.Path.Combine(dirpar, System.IO.Path.GetFileName(dir) + ext);

                                    bool suc1 = FreeCombineTextHelper.FreeCombineTextFiles(dtf, OutputFilepath, true);

                                    Success = suc1;

                                    if (System.IO.File.Exists(OutputFilepath))
                                    {
                                        FileInfo fi = new FileInfo(dtf.Rows[0]["fullfilepath"].ToString());

                                        FileInfo fi2 = new FileInfo(OutputFilepath);

                                        if (Properties.Settings.Default.KeepCreationDate)
                                        {
                                            fi2.CreationTime = fi.CreationTime;
                                        }

                                        if (Properties.Settings.Default.KeepLastModificationDate)
                                        {
                                            fi2.LastWriteTime = fi.LastWriteTime;
                                        }
                                    }

                                    if (FirstOutputFilepath == string.Empty)
                                    {
                                        FirstOutputFilepath = OutputFilepath;
                                    }
                                }
                                catch (Exception exd)
                                {
                                    Err += TranslateHelper.Translate("Error could not combine Files !") + " : " + OutputFilepath + "\r\n" + exd.Message + "\r\n";
                                }


                            }

                            frmMain.Instance.bwWork.ReportProgress(0, dirname);
                        }
                    }
                    else
                    {
                        bwWork.ReportProgress(30, lstdt.Count);                       

                        foreach (KeyValuePair<string, DataTable> dtl in lstdt)
                        {
                            string dirname = System.IO.Path.GetFileName(dtl.Key);

                            try
                            {
                                DataTable dtf = dtl.Value;
                                //string ext = dtl.Key;

                                string ext = System.IO.Path.GetExtension(dtf.Rows[0]["fullfilepath"].ToString());

                                string dir = System.IO.Path.GetDirectoryName(dtf.Rows[0]["fullfilepath"].ToString());

                                string dirpar = System.IO.Path.GetDirectoryName(dir);

                                OutputFilenameCreator of = new OutputFilenameCreator(dtf.Rows[0]["fullfilepath"].ToString(), dir);

                                string dir3 = of.Value;

                                //OutputFilepath = System.IO.Path.Combine(dirpar, System.IO.Path.GetFileName(dir) + ext);

                                OutputFilepath = System.IO.Path.Combine(dirpar, dir3);

                                //OutputFilepath = System.IO.Path.Combine(dirpar, System.IO.Path.GetFileName(dir) + ext);

                                bool suc1 = FreeCombineTextHelper.FreeCombineTextFiles(dtf, OutputFilepath, true);

                                Success = suc1;

                                if (System.IO.File.Exists(OutputFilepath))
                                {
                                    FileInfo fi = new FileInfo(dtf.Rows[0]["fullfilepath"].ToString());

                                    FileInfo fi2 = new FileInfo(OutputFilepath);

                                    if (Properties.Settings.Default.KeepCreationDate)
                                    {
                                        fi2.CreationTime = fi.CreationTime;
                                    }

                                    if (Properties.Settings.Default.KeepLastModificationDate)
                                    {
                                        fi2.LastWriteTime = fi.LastWriteTime;
                                    }
                                }

                                if (FirstOutputFilepath == string.Empty)
                                {
                                    FirstOutputFilepath = OutputFilepath;
                                }
                            }
                            catch (Exception exd)
                            {
                                Err += TranslateHelper.Translate("Error could not combine Files !") + " : " + OutputFilepath + "\r\n" + exd.Message + "\r\n";
                            }

                            frmMain.Instance.bwWork.ReportProgress(0, dirname);
                        }
                    }
                }
            }
            catch (Exception exm)
            {
                Err += TranslateHelper.Translate("Error could not combine Files !") + " : " + exm.Message + "\r\n";
            }
        }

        public DataTable dt = new DataTable("table");
        public DataTable dtClipboard = new DataTable("table");

        private bool _IsDirty = false;

        private bool IsDirty
        {
            get { return _IsDirty; }

            set
            {
                _IsDirty = value;

                lblTotal.Text = TranslateHelper.Translate("Total") + " : " + dt.Rows.Count + " " + TranslateHelper.Translate("Documents");
            }
        }


        private void tsdbAddFile_ButtonClick(object sender, EventArgs e)
        {
            if (!CreateFilePatternSetting()) return;

            openFileDialog1.Filter = "All Files (*.*)|*.*";
            openFileDialog1.Multiselect = true;

            openFileDialog1.FileName = "";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                SilentAddErr = "";

                try
                {
                    this.Cursor = Cursors.WaitCursor;

                    for (int k = 0; k < openFileDialog1.FileNames.Length; k++)
                    {
                        AddFile(openFileDialog1.FileNames[k]);
                        RecentFilesHelper.AddRecentFile(openFileDialog1.FileNames[k]);
                    }
                }
                finally
                {
                    this.Cursor = null;

                    if (SilentAddErr != string.Empty)
                    {
                        frmError f = new frmError(TranslateHelper.Translate("Error"), SilentAddErr);
                        f.ShowDialog(this);
                    }
                }
            }
        }

        private void tsdbAddFile_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (!CreateFilePatternSetting()) return;

            try
            {
                this.Cursor = Cursors.WaitCursor;

                SilentAddErr = "";

                AddFile(e.ClickedItem.Text);
                RecentFilesHelper.AddRecentFile(e.ClickedItem.Text);

            }
            finally
            {
                this.Cursor = null;

                if (SilentAddErr != string.Empty)
                {
                    frmError f = new frmError(TranslateHelper.Translate("Error"), SilentAddErr);
                    f.ShowDialog(this);
                }
            }
        }

        private void tsbRemove_Click(object sender, EventArgs e)
        {
            DataGridViewSelectedCellCollection cells = dgFiles.SelectedCells;
            List<DataGridViewRow> rows = new List<DataGridViewRow>();

            for (int k = 0; k < cells.Count; k++)
            {
                if (rows.IndexOf(dgFiles.Rows[cells[k].RowIndex]) < 0)
                {
                    rows.Add(dgFiles.Rows[cells[k].RowIndex]);
                }
            }

            for (int k = 0; k < rows.Count; k++)
            {
                dgFiles.Rows.Remove(rows[k]);
            }

            IsDirty = true;
        }        

        private void tsbClear_Click(object sender, EventArgs e)
        {
            //LockTest();
            //return;

            DialogResult dres = Module.ShowQuestionDialog(TranslateHelper.Translate("Are you sure that you want clear the added files ?"), TranslateHelper.Translate("Clear Added Files ?"));

            if (dres == DialogResult.Yes)
            {
                dt.Rows.Clear();
            }

            IsDirty = true;
        }

        private void tsdbAddFolder_ButtonClick(object sender, EventArgs e)
        {
            if (!CreateFilePatternSetting()) return;

            folderBrowserDialog1.SelectedPath = "";
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                SilentAddErr = "";

                AddFolder(folderBrowserDialog1.SelectedPath);
                RecentFilesHelper.AddRecentFolder(folderBrowserDialog1.SelectedPath);

                if (SilentAddErr != string.Empty)
                {
                    frmError f = new frmError(TranslateHelper.Translate("Error"), SilentAddErr);
                    f.ShowDialog(this);
                }
            }
        }

        private void tsdbAddFolder_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (!CreateFilePatternSetting()) return;

            SilentAddErr = "";

            AddFolder(e.ClickedItem.Text, "",false);
            RecentFilesHelper.AddRecentFolder(e.ClickedItem.Text);

            if (SilentAddErr != string.Empty)
            {
                frmError f = new frmError(TranslateHelper.Translate("Error"), SilentAddErr);
                f.ShowDialog(this);
            }
        }

        public void ImportList(string listfilepath)
        {
            string curdir = Environment.CurrentDirectory;

            try
            {
                SilentAdd = true;
                using (StreamReader sr = new StreamReader(listfilepath, Encoding.Default, true))
                {
                    string line = null;

                    while ((line = sr.ReadLine()) != null)
                    {
                        if (line.StartsWith("#"))
                        {
                            continue;
                        }

                        string filepath = line;
                        string password = "";

                        try
                        {
                            if (line.StartsWith("\""))
                            {
                                int epos = line.IndexOf("\"", 1);

                                if (epos > 0)
                                {
                                    filepath = line.Substring(1, epos - 1);
                                }
                            }
                            else if (line.StartsWith("'"))
                            {
                                int epos = line.IndexOf("'", 1);

                                if (epos > 0)
                                {
                                    filepath = line.Substring(1, epos - 1);
                                }
                            }

                            int compos = line.IndexOf(",");

                            if (compos > 0)
                            {
                                password = line.Substring(compos + 1);

                                if (!line.StartsWith("\"") && !line.StartsWith("'"))
                                {
                                    filepath = line.Substring(0, compos);
                                }

                                if ((password.StartsWith("\"") && password.EndsWith("\""))
                                    || (password.StartsWith("'") && password.EndsWith("'")))
                                {
                                    if (password.Length == 2)
                                    {
                                        password = "";
                                    }
                                    else
                                    {
                                        password = password.Substring(1, password.Length - 2);
                                    }
                                }

                            }
                        }
                        catch (Exception exq)
                        {
                            SilentAddErr += TranslateHelper.Translate("Error while processing List !") + " " + line + " " + exq.Message + "\r\n";
                        }

                        line = filepath;

                        Environment.CurrentDirectory = System.IO.Path.GetDirectoryName(listfilepath);

                        line = System.IO.Path.GetFullPath(line);

                        if (System.IO.File.Exists(line))
                        {
                            AddFile(line, password);
                            /*
                            else
                            {
                                SilentAddErr += TranslateHelper.Translate("Error wrong file type !") + " " + line + "\r\n";
                            }*/
                        }
                        else if (System.IO.Directory.Exists(line))
                        {
                            AddFolder(line, password,false);
                        }
                        else
                        {
                            SilentAddErr += TranslateHelper.Translate("Error. File or Directory not found !") + " " + line + "\r\n";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                SilentAddErr += TranslateHelper.Translate("Error could not read file !") + " " + ex.Message + "\r\n";
            }
            finally
            {
                Environment.CurrentDirectory = curdir;

                SilentAdd = false;
            }
        }

        private void tsdbImportList_ButtonClick(object sender, EventArgs e)
        {
            if (!CreateFilePatternSetting()) return;

            SilentAddErr = "";

            openFileDialog1.Filter = "Text Files (*.txt)|*.txt|CSV Files (*.csv)|*.csv|All Files (*.*)|*.*";
            openFileDialog1.FilterIndex = 0;
            openFileDialog1.FileName = "";

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                ImportList(openFileDialog1.FileName);
                RecentFilesHelper.ImportListRecent(openFileDialog1.FileName);

                if (SilentAddErr != string.Empty)
                {
                    frmMessage f = new frmMessage();
                    f.txtMsg.Text = SilentAddErr;
                    f.ShowDialog();

                }
            }
        }

        private void tsdbImportList_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (!CreateFilePatternSetting()) return;

            SilentAddErr = "";

            ImportList(e.ClickedItem.Text);
            RecentFilesHelper.ImportListRecent(e.ClickedItem.Text);

            if (SilentAddErr != string.Empty)
            {
                frmMessage f = new frmMessage();
                f.txtMsg.Text = SilentAddErr;
                f.ShowDialog();

            }
        }
        /*
        #region Share

        private void tsiFacebook_Click(object sender, EventArgs e)
        {
            ShareHelper.ShareFacebook();
        }

        private void tsiTwitter_Click(object sender, EventArgs e)
        {
            ShareHelper.ShareTwitter();
        }

        private void tsiGooglePlus_Click(object sender, EventArgs e)
        {
            ShareHelper.ShareGooglePlus();
        }

        private void tsiLinkedIn_Click(object sender, EventArgs e)
        {
            ShareHelper.ShareLinkedIn();
        }

        private void tsiEmail_Click(object sender, EventArgs e)
        {
            ShareHelper.ShareEmail();
        }

        #endregion
        */
        public bool AddFile(string filepath)
        {
            return AddFile(filepath, "", "");
        }

        public bool AddFile(string filepath, string password)
        {
            return AddFile(filepath, password, "");
        }

        public bool AddFile(string filepath, string password, string rootfolder)
        {
            string ext = "*" + System.IO.Path.GetExtension(filepath).ToLower() + ";";

            if (!FilePatternEvaluator.IsFilePattern(filepath)) return false;

            /*
            if (Module.AcceptableMediaInputPattern.IndexOf(ext) < 0)
            {
                SilentAddErr += filepath + "\n\n" + TranslateHelper.Translate("Please add only Word Files !") + "\n\n";

                return false;
            }
            */

            DataRow dr = dt.NewRow();

            FileInfo fi = new FileInfo(filepath);

            long sizekb = fi.Length / 1024;
            dr["filename"] = fi.Name;
            dr["fullfilepath"] = filepath;
            dr["sizekb"] = sizekb.ToString() + "KB";
            dr["filedate"] = fi.LastWriteTime.ToString();
            dr["rootfolder"] = rootfolder;
            dr["foldersep"] = false;
            dr["folderfiles"] = "";

            if (password != string.Empty)
            {
                dr["password"] = password;
            }

            dt.Rows.Add(dr);

            /*
            if (dt.Rows.Count == 1)
            {
                string outfile = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(filepath), "mergedDocument.docx");

                RecentFilesHelper.AddRecentOutputFile(outfile);                
            }
            */

            IsDirty = true;

            return true;
        }

        public void AddFolder(string folder_path)
        {
            AddFolder(folder_path, "",false);
        }

        public void AddFolder(string folder_path, string password,bool folderSeparate)
        {
            string[] filez = null;

            bool foldersep = false;

            bool found = false;

            for (int k=0;k<dt.Rows.Count;k++)
            {
                if (dt.Rows[k]["foldersep"].ToString()==bool.TrueString)
                {
                    found = true;
                    break;
                }
            }

            if (folderSeparate && !found)
            {
                foldersep = true;
            }

            if (!SilentAdd && !found)
            {
                DialogResult dres = Module.ShowQuestionDialog(
                    "Would you like to join each file of the folder separately (intro + file + outro) or do you want to just import the files of the folder ?", 
                    TranslateHelper.Translate("Join each file of the folder separately (intro + file + outro) ?"));

                if (dres == DialogResult.Yes)
                {
                    foldersep = true;
                }
            }

            if (!SilentAdd)
            {
                if (System.IO.Directory.GetDirectories(folder_path).Length > 0)
                {
                    DialogResult dres = Module.ShowQuestionDialog("Would you like to add also Subdirectories ?", TranslateHelper.Translate("Add Subdirectories ?"));

                    if (dres == DialogResult.Yes)
                    {
                        filez = System.IO.Directory.GetFiles(folder_path, "*.*", SearchOption.AllDirectories);
                    }
                    else
                    {
                        filez = System.IO.Directory.GetFiles(folder_path, "*.*", SearchOption.TopDirectoryOnly);
                    }
                }
                else
                {
                    filez = System.IO.Directory.GetFiles(folder_path, "*.*", SearchOption.TopDirectoryOnly);
                }
            }
            else
            {
                // silent add for import list
                filez = System.IO.Directory.GetFiles(folder_path, "*.*", SearchOption.AllDirectories);
            }

            string folderfiles = "";

            try
            {
                this.Cursor = Cursors.WaitCursor;

                for (int k = 0; k < filez.Length; k++)
                {
                    string filepath = filez[k];

                    //if (Module.IsWordDocument(filepath) || Module.IsPPDocument(filepath) || Module.IsExcelDocument(filepath))
                    //if (Module.IsPDFDocument(filepath))
                    if (FilePatternEvaluator.IsFilePattern(filepath))
                    {
                        if (!foldersep)
                        {
                            AddFile(filez[k], password, folder_path);
                        }
                        else
                        {
                            folderfiles += filepath + "|||";
                        }
                    }
                }

                if (foldersep)
                {
                    DataRow dr = dt.NewRow();
                    DirectoryInfo di = new DirectoryInfo(folder_path);

                    dr["filename"] = System.IO.Path.GetFileName(folder_path);
                    dr["fullfilepath"] = folder_path;                    
                    dr["filedate"] = di.LastWriteTime.ToString();
                    dr["rootfolder"] = "";
                    dr["foldersep"] = true;
                    dr["folderfiles"] = folderfiles;                    
                    dr["password"] = password;                    

                    dt.Rows.Add(dr);
                }
            }
            finally
            {
                this.Cursor = null;
            }

        }

        bool FreeForPersonalUse = false;
        bool FreeForPersonalAndCommercialUse = true;

        private void SetTitle()
        {
            string str = "";

            if (!FreeForPersonalUse && !FreeForPersonalAndCommercialUse)
            {
                /*
                if (frmAbout.LDT == string.Empty)
                {
                    str += " - " + TranslateHelper.Translate("Trial Version - Unlicensed - Please Buy !");
                }
                else
                {
                    str += " - " + TranslateHelper.Translate("Licensed Version");
                }*/
            }
            else if (FreeForPersonalUse)
            {
                str += " - " + TranslateHelper.Translate("Free for Personal Use Only - Please Donate !");
            }
            else if (FreeForPersonalAndCommercialUse)
            {
                str += " - " + TranslateHelper.Translate("Free for Personal and Commercial Use - Please Donate !");
            }

            this.Text = Module.ApplicationTitle + str.ToUpper();
        }
        private void SetupOnLoad()
        {
            dgFiles.DataSource = dt;

            //3this.Icon = Properties.Resources.pdf_compress_48;

            this.Text = Module.ApplicationTitle;

            SetTitle();
            //this.Width = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Width;
            //this.Left = 0;
            AddLanguageMenuItems();

            //3DownloadSuggestionsHelper ds = new DownloadSuggestionsHelper();
            //3ds.SetupDownloadMenuItems(downloadToolStripMenuItem);

            AdjustSizeLocation();

            //3SetupOutputFolders();

            //3keepFolderStructureToolStripMenuItem.Checked = Properties.Settings.Default.KeepFolderStructure;

            RecentFilesHelper.FillMenuRecentFile();
            RecentFilesHelper.FillMenuRecentFolder();
            RecentFilesHelper.FillMenuRecentImportList();
            //3RecentFilesHelper.FillRecentOutputFile();                        

            exploreFirstOutputDocumentToolStripMenuItem.Checked = Properties.Settings.Default.ExploreDocumentOnFinish;

            numericStringSortToolStripMenuItem.Checked = Properties.Settings.Default.NumericStringSort;

            sortInDescendingOrderToolStripMenuItem.Checked = Properties.Settings.Default.SortDescendingOrder;

            //retainTimestampToolStripMenuItem.Checked = Properties.Settings.Default.RetainTimestamp;

            cmbOutputDir.Items.Clear();

            cmbOutputDir.Items.Add(TranslateHelper.Translate("Same Folder of Text Document"));            
            cmbOutputDir.Items.Add(TranslateHelper.Translate("Subfolder of Text Document"));
            cmbOutputDir.Items.Add(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments).ToString());
            cmbOutputDir.Items.Add("---------------------------------------------------------------------------------------");

            OutputFolderHelper.LoadOutputFolders();
            cmbOutputDir.SelectedIndex = Properties.Settings.Default.OutputFolderIndex;

            txtOutputFilenamePrefix.Text = Properties.Settings.Default.Prefix;
            txtOutputFilenameSuffix.Text = Properties.Settings.Default.Suffix;            

            txtDatePattern.Text = Properties.Settings.Default.DatePattern;

            checkForNewVersionEachWeekToolStripMenuItem.Checked = Properties.Settings.Default.CheckWeek;

            //=========
            
            keepCreationDateToolStripMenuItem.Checked =
                Properties.Settings.Default.KeepCreationDate;

            keepLastModificationDateToolStripMenuItem.Checked =
                Properties.Settings.Default.KeepLastModificationDate;

            showMessageOnSucessToolStripMenuItem.Checked=
                Properties.Settings.Default.ShowMessageOnSucess;

            chkRootFolder.Checked = Properties.Settings.Default.EachRootFolder;

            chkSubfolder.Checked = Properties.Settings.Default.EachSubFolder;

            //=======================

            chkAddTextHeader.Checked = Properties.Settings.Default.ChkAddTextHeader;
            chkAddTextFooter.Checked = Properties.Settings.Default.ChkAddTextFooter;
            chkAddLinesHeader.Checked = Properties.Settings.Default.ChkAddLinesHeader;
            chkAddLinesFooter.Checked = Properties.Settings.Default.ChkAddLinesFooter;
            chkOnlyForFirstHeader.Checked = Properties.Settings.Default.ChkOnlyFirstHeader;
            chkOnlyForFirstFooter.Checked = Properties.Settings.Default.ChkOnlyFirstFooter;

            nudLinesHeader.Value = Properties.Settings.Default.NumLinesHeader;
            nudLinesFooter.Value = Properties.Settings.Default.NumLinesFooter;

            txtHeader.Text = Properties.Settings.Default.TxtHeader;
            txtFooter.Text = Properties.Settings.Default.TxtFooter;

            rdAutodetect.Checked = Properties.Settings.Default.OptEncoding == 0;
            rdDefault.Checked = Properties.Settings.Default.OptEncoding == 1;
            rdSpecific.Checked = Properties.Settings.Default.OptEncoding == 2;

            txtDateFormatFooter.Text = Properties.Settings.Default.FooterDateFormat;
            txtDateFormatHeader.Text = Properties.Settings.Default.HeaderDateFormat;

            EncodingInfo[] einfo=System.Text.Encoding.GetEncodings();

            for (int m=0;m<einfo.Length;m++)
            {
                cmbEncodings.Items.Add(einfo[m].CodePage + " - " + einfo[m].DisplayName);
            }

            if (Properties.Settings.Default.SpecificEncoding != string.Empty)
            {
                cmbEncodings.Text = Properties.Settings.Default.SpecificEncoding;
            }

            cmbVariableFilenamePrefix.Items.Add("$Filename");
            cmbVariableFilenamePrefix.Items.Add("$CreationDate");
            cmbVariableFilenamePrefix.Items.Add("$LastModificationDate");
            cmbVariableFilenamePrefix.Items.Add("$CurrentDate");

            cmbVariableFilenamePrefix.SelectedIndex = 0;

            cmbVariableFilenameSuffix.Items.Add("$Filename");
            cmbVariableFilenameSuffix.Items.Add("$CreationDate");
            cmbVariableFilenameSuffix.Items.Add("$LastModificationDate");
            cmbVariableFilenameSuffix.Items.Add("$CurrentDate");

            cmbVariableFilenameSuffix.SelectedIndex = 0;

            cmbVariablesHeader.Items.Add("$Filename");
            cmbVariablesHeader.Items.Add("$Filelpath");
            cmbVariablesHeader.Items.Add("$FolderName");
            cmbVariablesHeader.Items.Add("$FolderPath");
            cmbVariablesHeader.Items.Add("$CreationDate");
            cmbVariablesHeader.Items.Add("$LastModificationDate");
            cmbVariablesHeader.Items.Add("$CurrentDate");
            cmbVariablesHeader.Items.Add("$SizeBytes");
            cmbVariablesHeader.Items.Add("$SizeKBytes");
            cmbVariablesHeader.Items.Add("$SizeMBytes");
            cmbVariablesHeader.Items.Add("$NumberOfWords");
            cmbVariablesHeader.Items.Add("$NumberOfLines");
            cmbVariablesHeader.Items.Add("$NonWhitespaceChars");
            cmbVariablesHeader.Items.Add("$DocumentLength");

            cmbVariablesHeader.Items.Add("$FileNumber");
            cmbVariablesHeader.Items.Add("$TotalFiles");

            for (int k=0;k<cmbVariablesHeader.Items.Count;k++)
            {
                cmbVariablesFooter.Items.Add(cmbVariablesHeader.Items[k].ToString());
            }

            cmbVariablesHeader.SelectedIndex = 0;
            cmbVariablesFooter.SelectedIndex = 0;
            cmbVariableFilenamePrefix.SelectedIndex = 0;

            chkFileExtension.Checked = Properties.Settings.Default.EachFileExtension;

            ucFilePattern uc1 = fplFilePatterns.Controls[0] as ucFilePattern;
            uc1.Selected = true;

            if (Properties.Settings.Default.FilePattern.Trim() != string.Empty)
            {
                string[] sz = Properties.Settings.Default.FilePattern.Split(new string[] { "|||" }, StringSplitOptions.RemoveEmptyEntries);

                fplFilePatterns.Controls.Clear();

                for (int k = 0; k < sz.Length; k += 2)
                {
                    ucFilePattern uc = new ucFilePattern();

                    string sinc = sz[k].Substring(0, 1);
                    string sval = sz[k].Substring(2);
                    if (sval == "-") sval = "";

                    if (sinc == "0")
                    {
                        uc.cmbIncludeWildcards.SelectedIndex = 0;
                    }
                    else
                    {
                        uc.cmbIncludeWildcards.SelectedIndex = 1;
                    }

                    uc.txtWildcards.Text = sval;

                    string sinc1 = sz[k + 1].Substring(0, 1);
                    string sval1 = sz[k + 1].Substring(2);
                    if (sval1 == "-") sval1 = "";

                    if (sinc1 == "0")
                    {
                        uc.cmbIncludeRegExp.SelectedIndex = 0;
                    }
                    else
                    {
                        uc.cmbIncludeRegExp.SelectedIndex = 1;
                    }

                    uc.txtRegExp.Text = sval1;

                    fplFilePatterns.Controls.Add(uc);
                }
            }

            ucFilePattern uc2 = fplFilePatterns.Controls[0] as ucFilePattern;
            uc2.Selected = true;

            if (Properties.Settings.Default.SpecificEncoding==string.Empty)
            {
                cmbEncodings.SelectedIndex = 0;
            }
            else
            {
                cmbEncodings.Text = Properties.Settings.Default.SpecificEncoding;
            }

        }

        private void AdjustSizeLocation()
        {
            if (Properties.Settings.Default.Maximized)
            {
                this.WindowState = FormWindowState.Maximized;
            }
            else
            {

                if (Properties.Settings.Default.Width == -1)
                {
                    this.CenterToScreen();
                    return;
                }
                else
                {
                    this.Width = Properties.Settings.Default.Width;
                }
                if (Properties.Settings.Default.Height != -1)
                {
                    this.Height = Properties.Settings.Default.Height;
                }

                if (Properties.Settings.Default.Left != -1)
                {
                    this.Left = Properties.Settings.Default.Left;
                }

                if (Properties.Settings.Default.Top != -1)
                {
                    this.Top = Properties.Settings.Default.Top;
                }

                if (this.Width < 300)
                {
                    this.Width = 300;
                }

                if (this.Height < 300)
                {
                    this.Height = 300;
                }

                if (this.Left < 0)
                {
                    this.Left = 0;
                }

                if (this.Top < 0)
                {
                    this.Top = 0;
                }
            }

        }

        private void SaveSizeLocation()
        {
            Properties.Settings.Default.Maximized = (this.WindowState == FormWindowState.Maximized);

            Properties.Settings.Default.Save();

            if (this.WindowState == System.Windows.Forms.FormWindowState.Minimized) return;

            Properties.Settings.Default.Left = this.Left;
            Properties.Settings.Default.Top = this.Top;
            Properties.Settings.Default.Width = this.Width;
            Properties.Settings.Default.Height = this.Height;
            Properties.Settings.Default.Save();

        }

        #region Localization

        private void AddLanguageMenuItems()
        {
            for (int k = 0; k < frmLanguage.LangCodes.Count; k++)
            {
                ToolStripMenuItem ti = new ToolStripMenuItem();
                ti.Text = frmLanguage.LangDesc[k];
                ti.Tag = frmLanguage.LangCodes[k];
                ti.Image = frmLanguage.LangImg[k];

                if (Properties.Settings.Default.Language == frmLanguage.LangCodes[k])
                {
                    ti.Checked = true;
                }

                ti.Click += new EventHandler(tiLang_Click);

                if (k < 25)
                {
                    languages1ToolStripMenuItem.DropDownItems.Add(ti);
                }
                else
                {
                    languages2ToolStripMenuItem.DropDownItems.Add(ti);
                }

                //languageToolStripMenuItem.DropDownItems.Add(ti);
            }
        }

        void tiLang_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem ti = (ToolStripMenuItem)sender;
            string langcode = ti.Tag.ToString();
            ChangeLanguage(langcode);

            //for (int k = 0; k < languageToolStripMenuItem.DropDownItems.Count; k++)
            for (int k = 0; k < languages1ToolStripMenuItem.DropDownItems.Count; k++)
            {
                ToolStripMenuItem til = (ToolStripMenuItem)languages1ToolStripMenuItem.DropDownItems[k];
                if (til == ti)
                {
                    til.Checked = true;
                }
                else
                {
                    til.Checked = false;
                }
            }

            for (int k = 0; k < languages2ToolStripMenuItem.DropDownItems.Count; k++)
            {
                ToolStripMenuItem til = (ToolStripMenuItem)languages2ToolStripMenuItem.DropDownItems[k];
                if (til == ti)
                {
                    til.Checked = true;
                }
                else
                {
                    til.Checked = false;
                }
            }
        }

        private bool InChangeLanguage = false;

        private void ChangeLanguage(string language_code)
        {
            try
            {
                InChangeLanguage = true;

                Properties.Settings.Default.Language = language_code;
                frmLanguage.SetLanguage();

                Properties.Settings.Default.Save();
                Module.ShowMessage("Please restart the application !");
                Application.Exit();
                return;

                bool maximized = (this.WindowState == FormWindowState.Maximized);
                this.WindowState = FormWindowState.Normal;

                /*
                RegistryKey key = Registry.CurrentUser;
                RegistryKey key2 = Registry.CurrentUser;

                try
                {
                    key = key.OpenSubKey("Software\\softpcapps Software", true);

                    if (key == null)
                    {
                        key = Registry.CurrentUser.CreateSubKey("SOFTWARE\\softpcapps Software");
                    }

                    key2 = key.OpenSubKey(frmLanguage.RegKeyName, true);

                    if (key2 == null)
                    {
                        key2 = key.CreateSubKey(frmLanguage.RegKeyName);
                    }

                    key = key2;

                    //key.SetValue("Language", language_code);
                    key.SetValue("Menu Item Caption", TranslateHelper.Translate("Change PDF Properties"));
                }
                catch (Exception ex)
                {
                    Module.ShowError(ex);
                    return;
                }
                finally
                {
                    key.Close();
                    key2.Close();
                }
                */
                //1SaveSizeLocation();

                //3SavePositionSize();

                this.Controls.Clear();

                InitializeComponent();

                SetupOnLoad();

                if (maximized)
                {
                    this.WindowState = FormWindowState.Maximized;
                }

                this.ResumeLayout(true);
            }
            finally
            {
                InChangeLanguage = false;
            }
        }

        #endregion        

        private void frmMain_Load(object sender, EventArgs e)
        {            
            SetupOnLoad();

            if (!Module.IsFromWindowsExplorer && !Module.IsCommandLine && Properties.Settings.Default.CheckWeek)
            {
                UpdateHelper.InitializeCheckVersionWeek();
            }

            /*
            if (Module.args != null)
            {
                AddVisual(Module.args);
            }
            */

            //CompressZIPPackage();

            ResizeControls();
        }

        private void AddVisual(string[] argsvisual)
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;

                //Module.ShowMessage("Is From Windows Explorer");                                

                for (int k = 0; k < argsvisual.Length; k++)
                {
                    if (System.IO.File.Exists(argsvisual[k]))
                    {
                        AddFile(argsvisual[k]);

                    }
                    else if (System.IO.Directory.Exists(argsvisual[k]))
                    {
                        AddFolder(argsvisual[k]);
                    }
                }
            }
            finally
            {
                this.Cursor = null;
            }
        }


        #region Help

        private void helpGuideToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //System.Diagnostics.Process.Start(Application.StartupPath + "\\Video Cutter Joiner Expert - User's Manual.chm");
            System.Diagnostics.Process.Start(Module.HelpURL);
        }

        private void pleaseDonateToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://softpcapps.com/donate.php");
        }

        private void dotsSoftwarePRODUCTCATALOGToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://softpcapps.com/downloads/4dots-Software-PRODUCT-CATALOG.pdf");
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmAbout f = new frmAbout();
            f.ShowDialog();
        }

        private void tiHelpFeedback_Click(object sender, EventArgs e)
        {
            /*
            frmUninstallQuestionnaire f = new frmUninstallQuestionnaire(false);
            f.ShowDialog();
            */

            System.Diagnostics.Process.Start("https://softpcapps.com/support/bugfeature.php?app=" + System.Web.HttpUtility.UrlEncode(Module.ShortApplicationTitle));
        }

        private void followUsOnTwitterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("http://www.twitter.com/4dotsSoftware");
        }

        private void visit4dotsSoftwareWebsiteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://softpcapps.com");
        }

        private void checkForNewVersionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UpdateHelper.CheckVersion(false);
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Application.Exit();
            }
            catch { }
        }

        #endregion

        private void SaveSettings()
        {
            Properties.Settings.Default.ExploreDocumentOnFinish = exploreFirstOutputDocumentToolStripMenuItem.Checked;

            Properties.Settings.Default.NumericStringSort = numericStringSortToolStripMenuItem.Checked;

            Properties.Settings.Default.SortDescendingOrder = sortInDescendingOrderToolStripMenuItem.Checked;

            //Properties.Settings.Default.RetainTimestamp = retainTimestampToolStripMenuItem.Checked;

            //3Properties.Settings.Default.OutputFolderIndex = cmbOutputDir.SelectedIndex;                        

            Properties.Settings.Default.Prefix = txtOutputFilenamePrefix.Text;
            Properties.Settings.Default.Suffix = txtOutputFilenameSuffix.Text;

            Properties.Settings.Default.DatePattern = txtDatePattern.Text;

            Properties.Settings.Default.CheckWeek = checkForNewVersionEachWeekToolStripMenuItem.Checked;

            //=========

            Properties.Settings.Default.KeepCreationDate = keepCreationDateToolStripMenuItem.Checked;

            Properties.Settings.Default.KeepLastModificationDate = keepLastModificationDateToolStripMenuItem.Checked;

            Properties.Settings.Default.ShowMessageOnSucess = showMessageOnSucessToolStripMenuItem.Checked;

            Properties.Settings.Default.EachRootFolder = chkRootFolder.Checked;

            Properties.Settings.Default.EachSubFolder = chkSubfolder.Checked;

            //==================

            Properties.Settings.Default.ChkAddTextHeader = chkAddTextHeader.Checked;
            Properties.Settings.Default.ChkAddTextFooter = chkAddTextFooter.Checked;
            Properties.Settings.Default.ChkAddLinesHeader = chkAddLinesHeader.Checked;
            Properties.Settings.Default.ChkAddLinesFooter = chkAddLinesFooter.Checked;
            Properties.Settings.Default.ChkOnlyFirstHeader = chkOnlyForFirstHeader.Checked;
            Properties.Settings.Default.ChkOnlyFirstFooter = chkOnlyForFirstFooter.Checked;

            Properties.Settings.Default.NumLinesHeader = (int)nudLinesHeader.Value;
            Properties.Settings.Default.NumLinesFooter = (int)nudLinesFooter.Value;

            Properties.Settings.Default.TxtHeader = txtHeader.Text;
            Properties.Settings.Default.TxtFooter = txtFooter.Text;

            Properties.Settings.Default.FooterDateFormat = txtDateFormatFooter.Text;
            Properties.Settings.Default.HeaderDateFormat = txtDateFormatHeader.Text;

            if (rdAutodetect.Checked) Properties.Settings.Default.OptEncoding = 0;
            if (rdDefault.Checked) Properties.Settings.Default.OptEncoding = 1;
            if (rdSpecific.Checked) Properties.Settings.Default.OptEncoding = 2;

            Properties.Settings.Default.SpecificEncoding = cmbEncodings.Text;

            CreateFilePatternSetting();

            Properties.Settings.Default.EachFileExtension = chkFileExtension.Checked;

            //==================

            Properties.Settings.Default.Save();
        }

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveSizeLocation();

            SaveSettings();
        }

        private void selectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            for (int k = 0; k < dgFiles.Rows.Count; k++)
            {
                dgFiles.Rows[k].Selected = true;
            }
        }

        private void seelctNoneToolStripMenuItem_Click(object sender, EventArgs e)
        {
            for (int k = 0; k < dgFiles.Rows.Count; k++)
            {
                dgFiles.Rows[k].Selected = false;
            }
        }

        private void invertSelectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            for (int k = 0; k < dgFiles.Rows.Count; k++)
            {
                dgFiles.Rows[k].Selected = !dgFiles.Rows[k].Selected;
            }
        }

        #region Grid Context menu

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dgFiles.CurrentRow == null) return;

            DataRowView drv = (DataRowView)dgFiles.CurrentRow.DataBoundItem;

            DataRow dr = drv.Row;

            string filepath = dr["fullfilepath"].ToString();

            System.Diagnostics.Process.Start(filepath);
        }

        private void exploreToolStripMenuItem_Click(object sender, EventArgs e)
        {
            /*
            DataRowView drv = (DataRowView)dgFiles.CurrentRow.DataBoundItem;

            DataRow dr = drv.Row;

            string filepath = dr["fullfilepath"].ToString();

            string args = string.Format("/e, /select, \"{0}\"", filepath);

            ProcessStartInfo info = new ProcessStartInfo();
            info.FileName = "explorer";
            info.UseShellExecute = true;
            info.Arguments = args;
            Process.Start(info);
            */
        }

        private void copyFullFilePathToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DataRowView drv = (DataRowView)dgFiles.CurrentRow.DataBoundItem;

            DataRow dr = drv.Row;

            string filepath = dr["fullfilepath"].ToString();

            Clipboard.Clear();

            Clipboard.SetText(filepath);
        }

        private void cmsFiles_Opening(object sender, CancelEventArgs e)
        {
            Point p = dgFiles.PointToClient(new Point(Control.MousePosition.X, Control.MousePosition.Y));
            DataGridView.HitTestInfo hit = dgFiles.HitTest(p.X, p.Y);

            if (hit.Type == DataGridViewHitTestType.Cell)
            {
                dgFiles.CurrentCell = dgFiles.Rows[hit.RowIndex].Cells[hit.ColumnIndex];
            }

            if (dgFiles.CurrentRow == null)
            {
                e.Cancel = true;
            }
        }
        #endregion

        #region Drag and Drop

        private void dgFiles_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop, false))
            {
                e.Effect = DragDropEffects.All;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private void dgFiles_DragOver(object sender, DragEventArgs e)
        {
            if ((e.AllowedEffect & DragDropEffects.Copy) == DragDropEffects.Copy)
            {
                e.Effect = DragDropEffects.Copy;
            }
        }

        private void dgFiles_DragDrop(object sender, DragEventArgs e)
        {
            if (!CreateFilePatternSetting()) return;

            if (e.Data.GetDataPresent(DataFormats.FileDrop, false))
            {
                string[] filez = (string[])e.Data.GetData(DataFormats.FileDrop);

                for (int k = 0; k < filez.Length; k++)
                {
                    try
                    {
                        this.Cursor = Cursors.WaitCursor;

                        if (System.IO.File.Exists(filez[k]))
                        {
                            AddFile(filez[k]);
                        }
                        else if (System.IO.Directory.Exists(filez[k]))
                        {
                            AddFolder(filez[k]);
                        }
                    }
                    finally
                    {
                        this.Cursor = null;
                    }
                }
            }
        }

        #endregion

        private void EnableDisableForm(bool enable)
        {
            foreach (Control co in this.Controls)
            {
                co.Enabled = enable;
            }
        }
        private void ExploreOnFinish()
        {            
            if (FirstOutputFilepath == string.Empty) return;

            string filepath = FirstOutputFilepath;

            string args = string.Format("/e, /select, \"{0}\"", filepath);

            ProcessStartInfo info = new ProcessStartInfo();
            info.FileName = "explorer";
            info.UseShellExecute = true;
            info.Arguments = args;
            Process.Start(info);
        }

        public string txtFilenameText = "";
        public string txtDatePatternText = "";

        public void tsbMergeDocuments_Click(object sender, EventArgs e)
        {
            if (cmbOutputDir.Text.Trim() == string.Empty)
            {
                Module.ShowMessage("Please specify an output file !");
                return;
            }

            if (dt.Rows.Count < 2)
            {
                Module.ShowMessage("Please specify at least two Documents to merge !");
                return;
            }

            if ((txtOutputFilenamePrefix.Text.Trim()==string.Empty) &&
                (txtOutputFilenameSuffix.Text.Trim()==string.Empty)
                )
            {
                Module.ShowMessage("Please specify prefix or suffix for output filename !");
                return;
            }

            SaveSettings();

            CreateFilePatternSetting();

            try
            {
                FilePatternEvaluator.IsFilePattern(@"c:\a.txt");
            }
            catch
            {
                return;
            }            

            //========================================

            //3Properties.Settings.Default.OutputFolderIndex = cmbOutputDir.SelectedIndex;                        

            FirstFilepath = dt.Rows[0]["fullfilepath"].ToString();

            FileInfo fi = new FileInfo(FirstFilepath);

            CreationDate = fi.CreationTime.ToString(txtDatePattern.Text);

            ModDate = fi.LastWriteTime.ToString(txtDatePattern.Text);

            string curdate = DateTime.Now.ToString(txtDatePattern.Text);

            string firstfilename=System.IO.Path.GetFileNameWithoutExtension(FirstFilepath);

            OutputDir = cmbOutputDir.Text;

            txtFilenameText = txtOutputFilenamePrefix.Text;

            txtDatePatternText = txtDatePattern.Text;

            /*
            OutputFilename = txtOutputFilenamePrefix.Text.Replace("[FILENAME]", firstfilename).Replace("[MODDATE]", ModDate)
                .Replace("[CREATIONDATE]", CreationDate).Replace("[CURDATE]",curdate)+".pdf";
            */

            dgFiles.EndEdit();

            for (int k = 0; k < dt.Rows.Count; k++)
            {
                if (dt.Rows[k]["foldersep"].ToString() == bool.TrueString)
                {
                    if (Properties.Settings.Default.EachRootFolder || Properties.Settings.Default.EachSubFolder
                        || Properties.Settings.Default.EachFileExtension)
                    {
                        Module.ShowMessage("Error ! Cannot join each file of the folder separately(intro + file + outro) together with the option `Create one Text document for each folder and subfolders` or `Create one Text document for each root folder` or `Create one Text document for each file extension`");
                        return;

                    }
                }
            }

            Err = string.Empty;
            OperationStopped = false;
            OperationPaused = false;                     

            try
            {
                EnableDisableForm(false);

                frmProgress fp = new frmProgress(false);
                fp.progressBar1.Value = 0;
                fp.progressBar1.Maximum = dt.Rows.Count;

                bwWork.RunWorkerAsync();

                if (!Module.IsCommandLine)
                {
                    fp.Show(this);
                }

                while (bwWork.IsBusy)
                {
                    Application.DoEvents();
                }

                fp.Close();

                //if (System.IO.File.Exists(OutputFilepath) && retainTimestampToolStripMenuItem.Checked)

                /*
                if (!Properties.Settings.Default.EachRootFolder && !Properties.Settings.Default.EachSubFolder)
                {
                    if (System.IO.File.Exists(OutputFilepath))
                    {
                        FileInfo fi2 = new FileInfo(OutputFilepath);

                        if (Properties.Settings.Default.KeepCreationDate)
                        {
                            fi2.CreationTime = fi.CreationTime;
                        }

                        if (Properties.Settings.Default.KeepLastModificationDate)
                        {
                            fi2.LastWriteTime = fi.LastWriteTime;
                        }
                    }
                }
                */

                EnableDisableForm(true);

                /*
                if (Module.IsFromWindowsExplorer)
                {

                }*/

                if (!Module.IsCommandLine)
                {
                    if (OperationStopped)
                    {
                        Module.ShowMessage("Operation stopped !");
                       return;
                    }


                    if (Err == string.Empty)
                    {
                        if (Properties.Settings.Default.ShowMessageOnSucess)
                        {
                            Module.ShowMessage("Operation completed");
                        }
                    }
                    else
                    {
                        frmError fe = new frmError(TranslateHelper.Translate("Operation completed with errors !"), Err);
                        fe.ShowDialog(this);
                    }
                    /*
                    frmMessage fm2 = new frmMessage();
                    fm2.txtMsg.Text = TranslateHelper.Translate("Operation Completed");
                    fm2.Text = TranslateHelper.Translate("Operation Completed");
                    fm2.chkShow.Visible = false;
                    fm2.TopMost = true;
                    fm2.ShowDialog();
                    */
                }
                else
                {
                    if (Err == string.Empty)
                    {
                        if (Properties.Settings.Default.ShowMessageOnSucess)
                        {
                            Module.ShowMessage("Operation completed");
                        }
                    }
                    else
                    {
                        Module.ShowError(Err);
                    }
                }

                if (FirstOutputFilepath == string.Empty)
                {
                    FirstOutputFilepath = OutputFilepath;
                }

                if (!Module.IsCommandLine && !Module.IsFromWindowsExplorer)
                {
                    //if (System.IO.File.Exists(cmbOutputDir.Text) && exploreFirstOutputDocumentToolStripMenuItem.Checked)

                    if (System.IO.File.Exists(FirstOutputFilepath) && exploreFirstOutputDocumentToolStripMenuItem.Checked)
                    {
                        ExploreOnFinish();
                    }
                }
            }
            catch (Exception ex)
            {
                EnableDisableForm(true);

                Module.ShowError(ex);
            }
        }

        private bool CreateFilePatternSetting()
        {
            string str = "";

            for (int k = 0; k < fplFilePatterns.Controls.Count; k++)
            {
                ucFilePattern uc = fplFilePatterns.Controls[k] as ucFilePattern;

                if (uc.txtWildcards.Text.Trim() == string.Empty)
                {
                    str += uc.cmbIncludeWildcards.SelectedIndex.ToString() + "|-";
                }
                else
                {
                    str += uc.cmbIncludeWildcards.SelectedIndex.ToString() + "|" + uc.txtWildcards.Text;
                }

                str += "|||";

                if (uc.txtRegExp.Text.Trim() == string.Empty)
                {
                    str += uc.cmbIncludeRegExp.SelectedIndex.ToString() + "|-";
                }
                else
                {
                    str += uc.cmbIncludeRegExp.SelectedIndex.ToString() + "|" + uc.txtRegExp.Text;
                }

                str += "|||";
            }

            Properties.Settings.Default.FilePattern = str;

            try
            {
                FilePatternEvaluator.IsFilePattern(@"c:\a.txt");
            }
            catch
            {
                return false;
            }

            return true;
        }

        private void tsbMoveUp_Click(object sender, EventArgs e)
        {
            if (dgFiles.SelectedRows == null) return;
            if (dgFiles.SelectedRows.Count == 0) return;

            List<DataRow> lst = new List<DataRow>();
            List<int> lstind = new List<int>();

            for (int k = 0; k < dgFiles.SelectedRows.Count; k++)
            {
                lstind.Add(dgFiles.SelectedRows[k].Index);
            }

            lstind.Sort();

            for (int k = 0; k < lstind.Count; k++)
            {
                DataRowView drv = (DataRowView)dgFiles.Rows[lstind[k]].DataBoundItem;
                lst.Add(drv.Row);
            }

            dgFiles.ClearSelection();

            for (int k = 0; k < lst.Count; k++)
            {
                int ind = lstind[k];

                if (ind > 0)
                {
                    DataRow dr = dt.NewRow();

                    for (int m = 0; m < dt.Columns.Count; m++)
                    {
                        dr[m] = lst[k][m];
                    }                    

                    dt.Rows.Remove(lst[k]);

                    dt.Rows.InsertAt(dr, ind - 1);
                }
            }            

            dgFiles.ClearSelection();

            int newind = -1;

            for (int k = 0; k < lstind.Count; k++)
            {
                if (lstind[k] > 0)
                {
                    dgFiles.Rows[lstind[k] - 1].Selected = true;

                    if (k == 0)
                    {
                        newind = lstind[k] - 1;
                    }
                }
                else
                {
                    dgFiles.Rows[lstind[k]].Selected = true;

                    if (k == 0)
                    {
                        newind = lstind[k];
                    }
                }
            }

            dgFiles.FirstDisplayedScrollingRowIndex = newind;            
        }

        private void tsbMoveDown_Click(object sender, EventArgs e)
        {
            if (dgFiles.SelectedRows == null) return;
            if (dgFiles.SelectedRows.Count == 0) return;

            List<DataRow> lst = new List<DataRow>();
            List<int> lstind = new List<int>();

            for (int k = 0; k < dgFiles.SelectedRows.Count; k++)
            {
                lstind.Add(dgFiles.SelectedRows[k].Index);
            }

            lstind.Sort();

            for (int k = 0; k < lstind.Count; k++)
            {
                DataRowView drv = (DataRowView)dgFiles.Rows[lstind[k]].DataBoundItem;
                lst.Add(drv.Row);
            }

            dgFiles.ClearSelection();

            for (int k = lst.Count - 1; k >= 0; k--)
            {
                int ind = lstind[k];

                if (ind < dt.Rows.Count - 1)
                {
                    DataRow dr = dt.NewRow();

                    for (int m = 0; m < dt.Columns.Count; m++)
                    {
                        dr[m] = lst[k][m];
                    }
                    /*
                    dr[0] = lst[k][0];
                    dr["durationmsecs"] = lst[k]["durationmsecs"];
                    dr["videoinfo"] = lst[k]["videoinfo"];
                    dr["videoimg"] = lst[k]["videoimg"];
                    dr["fadein"] = lst[k]["fadein"];
                    dr["fadeout"] = lst[k]["fadeout"];
                    dr["crossfade"] = lst[k]["crossfade"];
                    dr["effects"] = lst[k]["effects"];
                    dr["normalize"] = lst[k]["normalize"];
                    dr["effectstype"] = lst[k]["effectstype"];
                    */

                    /*
                    dt.Columns.Add("videoimg", typeof(Image));
                    dt.Columns.Add("ind", typeof(int));
                    dt.Columns.Add("durationmsecs", typeof(int));
                    dt.Columns.Add("videoinfo", typeof(FFMPEGInfo));

                    dt.Columns.Add("fadein", typeof(bool));
                    dt.Columns.Add("fadeout", typeof(bool));
                    dt.Columns.Add("crossfade", typeof(bool));
                    dt.Columns.Add("effects", typeof(bool));
                    dt.Columns.Add("normalize", typeof(bool));
                    dt.Columns.Add("effectstype", typeof(EffectsType));
                    */

                    dt.Rows.Remove(lst[k]);

                    dt.Rows.InsertAt(dr, ind + 1);
                }
            }

            //dgVideo.Refresh();

            dgFiles.ClearSelection();

            int newind = -1;

            for (int k = lstind.Count - 1; k >= 0; k--)
            {
                if (lstind[k] < dgFiles.Rows.Count - 1)
                {
                    dgFiles.Rows[lstind[k] + 1].Selected = true;

                    if (k == 0)
                    {
                        newind = lstind[k] + 1;
                    }
                }
                else
                {
                    dgFiles.Rows[lstind[k]].Selected = true;

                    if (k == 0)
                    {
                        newind = lstind[k];
                    }
                }
            }

            dgFiles.FirstDisplayedScrollingRowIndex = newind;
        }

        private void tsbCopy_Click(object sender, EventArgs e)
        {
            if (dgFiles.SelectedRows != null)
            {
                dtClipboard.Clear();

                for (int k = 0; k < dgFiles.SelectedRows.Count; k++)
                {
                    DataRowView drv = (DataRowView)dgFiles.SelectedRows[k].DataBoundItem;

                    DataRow dr = drv.Row;

                    DataRow dr0 = dtClipboard.NewRow();

                    for (int m = 0; m < dt.Columns.Count; m++)
                    {
                        dr0[m] = dr[m];
                    }

                    dtClipboard.Rows.Add(dr0);
                }
            }
        }

        private void tsbPaste_Click(object sender, EventArgs e)
        {
            int sel = 0;

            if (dgFiles.CurrentRow != null)
            {
                sel = dgFiles.CurrentRow.Index;
            }

            for (int k = 0; k < dtClipboard.Rows.Count; k++)
            {
                DataRow dr = dtClipboard.Rows[k];

                DataRow dr0 = dt.NewRow();

                for (int m = 0; m < dt.Columns.Count; m++)
                {
                    dr0[m] = dr[m];
                }

                dt.Rows.InsertAt(dr0, sel + k);
            }

            IsDirty = true;
        }

        private void filenameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem ti = sender as ToolStripMenuItem;

            if (ti != null)
            {
                string sortmode = ti.Tag.ToString();

                DataTable dt0 = FileSorter.SortAudioDataTable(dt, sortmode, sortInDescendingOrderToolStripMenuItem.Checked,numericStringSortToolStripMenuItem.Checked);

                dt = dt0;

                dgFiles.DataSource = dt;
            }
        }

        private void sortInDescendingOrderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            sortInDescendingOrderToolStripMenuItem.Checked = !sortInDescendingOrderToolStripMenuItem.Checked;

            Properties.Settings.Default.SortDescendingOrder = sortInDescendingOrderToolStripMenuItem.Checked;
        }

        private void numericStringSortToolStripMenuItem_Click(object sender, EventArgs e)
        {
            numericStringSortToolStripMenuItem.Checked = !numericStringSortToolStripMenuItem.Checked;

            Properties.Settings.Default.NumericStringSort = numericStringSortToolStripMenuItem.Checked;
        }

        private void btnOpenFolder_Click(object sender, EventArgs e)
        {
            if (cmbOutputDir.Text.Trim() != string.Empty && System.IO.Directory.Exists(cmbOutputDir.Text))
            {
                System.Diagnostics.Process.Start(cmbOutputDir.Text);
            }
        }

        private void btnChangeFolder_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbr = new FolderBrowserDialog();            
            
            if (fbr.ShowDialog() == DialogResult.OK)
            {
                RecentFilesHelper.AddRecentOutputFile(fbr.SelectedPath);

                cmbOutputDir.SelectedIndex = 4;
            }
        }

        private void saveDocumentsListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog opf = new SaveFileDialog();
            opf.Filter = "Text Files (*.txt)|*.txt";

            if (opf.ShowDialog() == DialogResult.OK)
            {
                using (System.IO.StreamWriter sw = new StreamWriter(opf.FileName))
                {
                    for (int k = 0; k < dt.Rows.Count; k++)
                    {
                        sw.WriteLine("\""+dt.Rows[k]["fullfilepath"].ToString()+"\"");
                    }
                }
            }
        }

        private void retainTimestampToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //retainTimestampToolStripMenuItem.Checked=!retainTimestampToolStripMenuItem.Checked;

            //Properties.Settings.Default.RetainTimestamp = retainTimestampToolStripMenuItem.Checked;
        }

        private void folderWatcherSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmOptionsWatchers f = new frmOptionsWatchers();

            f.ShowDialog(this);
        }

        private void cmbOutputDir_SelectedIndexChanged(object sender, EventArgs e)
        {            
            if (cmbOutputDir.SelectedIndex == 3)
            {
                Module.ShowMessage("Please specify another option as the Output Folder !");
                cmbOutputDir.SelectedIndex = Properties.Settings.Default.OutputFolderIndex;
            }
            else if (cmbOutputDir.SelectedIndex == 1)
            {
                frmOutputSubFolder fob = new frmOutputSubFolder();

                if (fob.ShowDialog() == DialogResult.OK)
                {
                    OutputFolderHelper.SaveOutputFolder(TranslateHelper.Translate("Subfolder") + " : " + fob.txtSubfolder.Text);
                }
                else
                {
                    return;
                }
            }            
        }

        private void dgFiles_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dgFiles.Columns[e.ColumnIndex].Name == "colFolder")
            {
                DataRowView dv = (DataRowView)dgFiles.Rows[e.RowIndex].DataBoundItem;

                if (dv.Row["foldersep"].ToString() == bool.TrueString)
                {
                    e.Value = Properties.Resources.folder;
                }
                else
                {
                    e.Value = new System.Drawing.Bitmap(1, 1);
                }
            }
        }

        private void youtubeChannelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://www.youtube.com/channel/UCovA-lld9Q79l08K-V1QEng");
        }

        private void importListFromExcelFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.FileName = "";
            openFileDialog1.Filter = "Excel Files (*.xls;*.xlsx;*.xlt)|*.xls;*.xlsx;*.xlt";
            openFileDialog1.Multiselect = false;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                ExcelImporter xl = new ExcelImporter();
                xl.ImportListExcel(openFileDialog1.FileName);
            }

        }

        private void enterFileListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string txt = "";

            for (int k = 0; k < dt.Rows.Count; k++)
            {
                txt += dt.Rows[k]["fullfilepath"].ToString() + "\r\n";
            }

            frmMultipleFiles f = new frmMultipleFiles(false, txt);

            if (f.ShowDialog() == DialogResult.OK)
            {
                dt.Rows.Clear();

                for (int k = 0; k < f.txtFiles.Lines.Length; k++)
                {
                    AddFile(f.txtFiles.Lines[k].Trim());
                }
            }
        }

        private void tryOnlineVersionAtOnlinepdfappscomToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://onlinepdfapps.com");
        }

        private void commandLineArgumentsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmMessage fm = new frmMessage(true);
            fm.ShowDialog(this);
        }

        private void proxySettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmProxySettings f = new frmProxySettings();
            f.ShowDialog(this);
        }

        private void btnHeaderClear_Click(object sender, EventArgs e)
        {
            txtHeader.Text = "";
        }

        private void btnFooterClear_Click(object sender, EventArgs e)
        {
            txtFooter.Text = "";
        }

        private void btnAddFilePattern_Click(object sender, EventArgs e)
        {
            ucFilePattern uc = new ucFilePattern();
            fplFilePatterns.Controls.Add(uc);
            uc.Selected = true;
        }

        private void btnFilePatternRemove_Click(object sender, EventArgs e)
        {
            if (fplFilePatterns.Controls.Count>1)
            {
                for (int k=0;k<fplFilePatterns.Controls.Count;k++)
                {
                    ucFilePattern uc = fplFilePatterns.Controls[k] as ucFilePattern;

                    bool found = false;

                    if (uc.Selected)
                    {
                        fplFilePatterns.Controls.RemoveAt(k);

                        found = true;
                    }

                    if (found)
                    {
                        if (k == 0)
                        {
                            ucFilePattern uc1 = fplFilePatterns.Controls[0] as ucFilePattern;
                            uc1.Selected = true;
                        }
                        else if (k < (fplFilePatterns.Controls.Count))
                        {
                            ucFilePattern uc1 = fplFilePatterns.Controls[k] as ucFilePattern;
                            uc1.Selected = true;
                        }
                        else if (k == (fplFilePatterns.Controls.Count))
                        {
                            ucFilePattern uc1 = fplFilePatterns.Controls[k - 1] as ucFilePattern;
                            uc1.Selected = true;
                        }


                        break;
                    }
                }
            }
        }

        private void btnAddVariableHeader_Click(object sender, EventArgs e)
        {
            txtHeader.SelectedText = cmbVariablesHeader.SelectedItem.ToString();
        }

        private void btnAddVariableFooter_Click(object sender, EventArgs e)
        {
            txtFooter.SelectedText = cmbVariablesFooter.SelectedItem.ToString();
        }

        private void btnAddVariableFilename_Click(object sender, EventArgs e)
        {
            txtOutputFilenamePrefix.SelectedText = cmbVariableFilenamePrefix.SelectedItem.ToString();
        }

        private void btnClearFileFilter_Click(object sender, EventArgs e)
        {
            fplFilePatterns.Controls.Clear();

            ucFilePattern uc = new ucFilePattern();

            fplFilePatterns.Controls.Add(uc);

            uc.Selected = true;
        }

        private void btnAddVariableFilenameSuffix_Click(object sender, EventArgs e)
        {
            txtOutputFilenameSuffix.SelectedText = cmbVariableFilenameSuffix.SelectedItem.ToString();
        }

        private void chkSubfolder_CheckedChanged(object sender, EventArgs e)
        {
            if (chkSubfolder.Checked)
            {
                chkRootFolder.Checked = false;
            }
        }

        private void chkRootFolder_CheckedChanged(object sender, EventArgs e)
        {
            if (chkRootFolder.Checked)
            {
                chkSubfolder.Checked = false;
            }
        }
    }
}
