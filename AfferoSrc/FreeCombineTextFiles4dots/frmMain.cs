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
            int val = frmProgress.Instance.progressBar1.Value;

            if ((val + 1) <= frmProgress.Instance.progressBar1.Maximum)
            {
                frmProgress.Instance.progressBar1.Value = frmProgress.Instance.progressBar1.Value + 1;
            }

            frmProgress.Instance.lblOutputFile.Text = e.UserState.ToString();

            frmProgress.Instance.lblJoinNumber.Text =
                frmProgress.Instance.progressBar1.Value.ToString()
                + " / " +
                frmProgress.Instance.progressBar1.Maximum.ToString();
        }

        void bwWork_DoWork(object sender, DoWorkEventArgs e)
        {
            string outfilepath = "";

            if (OutputDir.Trim() == TranslateHelper.Translate("Same Folder of PDF Document"))
            {
                string dirpath = System.IO.Path.GetDirectoryName(FirstFilepath);

                outfilepath = System.IO.Path.Combine(dirpath, OutputFilename);                
            }
            else if (OutputDir.StartsWith(TranslateHelper.Translate("Subfolder") + " : "))
            {
                int subfolderspos = (TranslateHelper.Translate("Subfolder") + " : ").Length;
                string subfolder = OutputDir.Substring(subfolderspos);

                outfilepath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(FirstFilepath) + "\\" + subfolder, OutputFilename);

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

            bool suc = FreeCombinePDFHelper.FreeCombinePDF(dt, outfilepath);

            Success = suc;
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
            openFileDialog1.Filter = Module.OpenFilesFilter;
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
            SilentAddErr = "";

            AddFolder(e.ClickedItem.Text, "");
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
                            AddFolder(line, password);
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
            AddFolder(folder_path, "");
        }

        public void AddFolder(string folder_path, string password)
        {
            string[] filez = null;

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

            try
            {
                this.Cursor = Cursors.WaitCursor;

                for (int k = 0; k < filez.Length; k++)
                {
                    string filepath = filez[k];

                    //if (Module.IsWordDocument(filepath) || Module.IsPPDocument(filepath) || Module.IsExcelDocument(filepath))
                    if (Module.IsPDFDocument(filepath))
                    {
                        AddFile(filez[k], password, folder_path);
                    }
                }
            }
            finally
            {
                this.Cursor = null;
            }

        }

        private void SetupOnLoad()
        {
            dgFiles.DataSource = dt;

            //3this.Icon = Properties.Resources.pdf_compress_48;

            this.Text = Module.ApplicationTitle;
            //this.Width = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Width;
            //this.Left = 0;
            
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

            retainTimestampToolStripMenuItem.Checked = Properties.Settings.Default.RetainTimestamp;

            cmbOutputDir.Items.Clear();

            cmbOutputDir.Items.Add(TranslateHelper.Translate("Same Folder of PDF Document"));            
            cmbOutputDir.Items.Add(TranslateHelper.Translate("Subfolder of PDF Document"));
            cmbOutputDir.Items.Add(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments).ToString());
            cmbOutputDir.Items.Add("---------------------------------------------------------------------------------------");

            OutputFolderHelper.LoadOutputFolders();
            cmbOutputDir.SelectedIndex = Properties.Settings.Default.OutputFolderIndex;

            txtFilename.Text = Properties.Settings.Default.FilenamePattern;

            txtDatePattern.Text = Properties.Settings.Default.DatePattern;
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
            Properties.Settings.Default.Left = this.Left;
            Properties.Settings.Default.Top = this.Top;
            Properties.Settings.Default.Width = this.Width;
            Properties.Settings.Default.Height = this.Height;
            Properties.Settings.Default.Save();

        }        

        private void frmMain_Load(object sender, EventArgs e)
        {            
            SetupOnLoad();            

            /*
            if (Module.args != null)
            {
                AddVisual(Module.args);
            }
            */

            //CompressZIPPackage();
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

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.ExploreDocumentOnFinish = exploreFirstOutputDocumentToolStripMenuItem.Checked;

            Properties.Settings.Default.NumericStringSort = numericStringSortToolStripMenuItem.Checked;

            Properties.Settings.Default.SortDescendingOrder = sortInDescendingOrderToolStripMenuItem.Checked;

            Properties.Settings.Default.RetainTimestamp = retainTimestampToolStripMenuItem.Checked;

            //3Properties.Settings.Default.OutputFolderIndex = cmbOutputDir.SelectedIndex;                        

            Properties.Settings.Default.FilenamePattern = txtFilename.Text;

            Properties.Settings.Default.DatePattern = txtDatePattern.Text;

            Properties.Settings.Default.Save();
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
            DataRowView drv = (DataRowView)dgFiles.CurrentRow.DataBoundItem;

            DataRow dr = drv.Row;

            string filepath = dr["fullfilepath"].ToString();

            string args = string.Format("/e, /select, \"{0}\"", filepath);

            ProcessStartInfo info = new ProcessStartInfo();
            info.FileName = "explorer";
            info.UseShellExecute = true;
            info.Arguments = args;
            Process.Start(info);
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

        private void ExploreOnFinish()
        {            
            if (OutputFilepath == string.Empty) return;

            string filepath = OutputFilepath;

            string args = string.Format("/e, /select, \"{0}\"", filepath);

            ProcessStartInfo info = new ProcessStartInfo();
            info.FileName = "explorer";
            info.UseShellExecute = true;
            info.Arguments = args;
            Process.Start(info);
        }

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

            Properties.Settings.Default.ExploreDocumentOnFinish = exploreFirstOutputDocumentToolStripMenuItem.Checked;

            Properties.Settings.Default.NumericStringSort = numericStringSortToolStripMenuItem.Checked;

            Properties.Settings.Default.SortDescendingOrder = sortInDescendingOrderToolStripMenuItem.Checked;

            //3Properties.Settings.Default.OutputFolderIndex = cmbOutputDir.SelectedIndex;                        

            FirstFilepath = dt.Rows[0]["fullfilepath"].ToString();

            FileInfo fi = new FileInfo(FirstFilepath);

            CreationDate = fi.CreationTime.ToString(txtDatePattern.Text);

            ModDate = fi.LastWriteTime.ToString(txtDatePattern.Text);

            string curdate = DateTime.Now.ToString(txtDatePattern.Text);

            string firstfilename=System.IO.Path.GetFileNameWithoutExtension(FirstFilepath);

            OutputDir = cmbOutputDir.Text;

            OutputFilename = txtFilename.Text.Replace("[FILENAME]", firstfilename).Replace("[MODDATE]", ModDate)
                .Replace("[CREATIONDATE]", CreationDate).Replace("[CURDATE]",curdate)+".pdf";

            dgFiles.EndEdit();

            try
            {
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

                if (System.IO.File.Exists(OutputFilepath) && retainTimestampToolStripMenuItem.Checked)
                {
                    FileInfo fi2 = new FileInfo(OutputFilepath);

                    fi2.CreationTime = fi.CreationTime;

                    fi2.LastWriteTime = fi.LastWriteTime;
                }

                if (!Module.IsCommandLine)
                {
                    Module.ShowMessage("Operation completed");

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
                    Module.ShowMessage("Operation completed");
                }

                if (!Module.IsCommandLine && !Module.IsFromWindowsExplorer)
                {
                    //if (System.IO.File.Exists(cmbOutputDir.Text) && exploreFirstOutputDocumentToolStripMenuItem.Checked)

                    if (System.IO.File.Exists(OutputFilepath) && exploreFirstOutputDocumentToolStripMenuItem.Checked)
                    {
                        ExploreOnFinish();
                    }
                }
            }
            catch (Exception ex)
            {
                Module.ShowError(ex);
            }
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
            retainTimestampToolStripMenuItem.Checked=!retainTimestampToolStripMenuItem.Checked;

            Properties.Settings.Default.RetainTimestamp = retainTimestampToolStripMenuItem.Checked;
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

    }
}
