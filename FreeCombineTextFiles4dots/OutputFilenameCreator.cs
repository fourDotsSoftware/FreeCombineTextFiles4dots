using System;
using System.Collections.Generic;

using System.Text;


namespace FreeCombineTextFiles4dots
{
    public class OutputFilenameCreator
    {
        private string Filepath = "";
        private string Dir = "";

        public OutputFilenameCreator(string filepath):this(filepath,"")
        {
            
        }
        public OutputFilenameCreator(string filepath,string dir)
        {
            Filepath = filepath;
            Dir = dir;
        }

        public string Value
        {
            get
            {
                if (Dir == string.Empty)
                {
                    System.IO.FileInfo fi = new System.IO.FileInfo(Filepath);

                    /*
                    cmbVariableFilename.Items.Add("$Filename");
                    cmbVariableFilename.Items.Add("$CreationDate");
                    cmbVariableFilename.Items.Add("$LastModificationDate");
                    cmbVariableFilename.Items.Add("$CurrentDate");
                    */

                    string prefix = Properties.Settings.Default.Prefix.Replace(
                        "$Filename", System.IO.Path.GetFileNameWithoutExtension(Filepath))
                        .Replace("$CreationDate", fi.CreationTime.ToString(Properties.Settings.Default.DatePattern))
                        .Replace("$LastModificationDate", fi.LastWriteTime.ToString(Properties.Settings.Default.DatePattern))
                        .Replace("$CurrentDate", DateTime.Now.ToString(Properties.Settings.Default.DatePattern));


                    string suffix = Properties.Settings.Default.Suffix.Replace(
                        "$Filename", System.IO.Path.GetFileNameWithoutExtension(Filepath))
                        .Replace("$CreationDate", fi.CreationTime.ToString(Properties.Settings.Default.DatePattern))
                        .Replace("$LastModificationDate", fi.LastWriteTime.ToString(Properties.Settings.Default.DatePattern))
                        .Replace("$CurrentDate", DateTime.Now.ToString(Properties.Settings.Default.DatePattern));

                    return prefix + System.IO.Path.GetFileNameWithoutExtension(Filepath) + suffix + System.IO.Path.GetExtension(Filepath);
                }
                else
                {
                    System.IO.FileInfo fi = new System.IO.FileInfo(Filepath);

                    /*
                    cmbVariableFilename.Items.Add("$Filename");
                    cmbVariableFilename.Items.Add("$CreationDate");
                    cmbVariableFilename.Items.Add("$LastModificationDate");
                    cmbVariableFilename.Items.Add("$CurrentDate");
                    */
                    string prefix = Properties.Settings.Default.Prefix.Replace(
                        "$Filename", System.IO.Path.GetFileNameWithoutExtension(Dir))
                        .Replace("$CreationDate", fi.CreationTime.ToString(Properties.Settings.Default.DatePattern))
                        .Replace("$LastModificationDate", fi.LastWriteTime.ToString(Properties.Settings.Default.DatePattern))
                        .Replace("$CurrentDate", DateTime.Now.ToString(Properties.Settings.Default.DatePattern));


                    string suffix = Properties.Settings.Default.Suffix.Replace(
                        "$Filename", System.IO.Path.GetFileNameWithoutExtension(Dir))
                        .Replace("$CreationDate", fi.CreationTime.ToString(Properties.Settings.Default.DatePattern))
                        .Replace("$LastModificationDate", fi.LastWriteTime.ToString(Properties.Settings.Default.DatePattern))
                        .Replace("$CurrentDate", DateTime.Now.ToString(Properties.Settings.Default.DatePattern));

                    return prefix + System.IO.Path.GetFileNameWithoutExtension(Dir) + suffix + System.IO.Path.GetExtension(Filepath);
                }
            }  
        }
    }
}
