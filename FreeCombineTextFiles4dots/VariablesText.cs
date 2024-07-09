using System;
using System.Collections.Generic;

using System.Text;

using System.IO;

namespace FreeCombineTextFiles4dots
{
    public class VariablesText
    {
        private string Filepath = "";

        private string VariablesTxt = "";

        private string DateFormat = "";

        private string Contents = "";

        private int FileNumber = 0;

        private int FilesTotal = 0;

        private Encoding Encoding = Encoding.Default;

        public VariablesText(string txt, string filepath, string dateFormat, Encoding enc,int fileNumber, int filesTotal)
        {
            Filepath = filepath;

            VariablesTxt = txt;

            DateFormat = dateFormat;

            //Contents = contents;

            Encoding = enc;

            FileNumber = fileNumber;

            FilesTotal = filesTotal;
        }

        public string GetText()
        {
            if (!System.IO.File.Exists(Filepath))
            {
                return TranslateHelper.Translate("Error - File not found");
            }

            string txt = VariablesTxt;

            string filename = System.IO.Path.GetFileName(Filepath);
            string filepath = Filepath;
            string foldername = System.IO.Path.GetFileName(System.IO.Path.GetDirectoryName(Filepath));
            string folderpath = System.IO.Path.GetDirectoryName(Filepath);

            txt = txt.Replace("$Filename", filename).Replace("$Filelpath", filepath)
                .Replace("$FolderName", foldername).Replace("$FolderPath", folderpath)
                .Replace("$FileNumber", FileNumber.ToString())
                .Replace("$TotalFiles", FilesTotal.ToString());

            FileInfo fi = new FileInfo(Filepath);

            string creationDate = "";

            try
            {
                creationDate = fi.CreationTime.ToString(DateFormat);
            }
            catch {

                creationDate = TranslateHelper.Translate("Error");
            }

            string lastModDate = "";

            try
            {
                lastModDate = fi.LastWriteTime.ToString(DateFormat);
            }
            catch {

                lastModDate = TranslateHelper.Translate("Error");
            }

            string curDate = "";

            try
            {
                curDate = DateTime.Now.ToString(DateFormat);
            }
            catch
            {

                curDate = TranslateHelper.Translate("Error");
            }

            txt=txt.Replace("$CreationDate", creationDate).Replace("$LastModificationDate", lastModDate)
                .Replace("$CurrentDate", curDate);

            if (VariablesTxt.Contains("$SizeBytes") || VariablesTxt.Contains("$SizeKBytes")
                || VariablesTxt.Contains("$SizeMBytes"))
            {

                string sizebytes = fi.Length.ToString() + " Bytes";

                decimal dec1 = (decimal)fi.Length;
                decimal dec2 = (decimal)1024;
                decimal dec3 = (decimal)1024 * 1024;

                decimal deckb = dec1 / dec2;
                decimal decmb = dec1 / dec3;

                string sizekbytes = deckb.ToString() + " KB";

                string sizembytes = decmb.ToString() + " MB";

                txt = txt.Replace("$SizeBytes", sizebytes).Replace("$SizeKBytes", sizekbytes)
                    .Replace("$SizeMBytes", sizembytes);


            }

            if (VariablesTxt.Contains("$NumberOfWords") || VariablesTxt.Contains("$NumberOfLines")
                || VariablesTxt.Contains("$DocumentLength") || VariablesTxt.Contains("$NonWhitespaceChars"))
            {
                Contents = System.IO.File.ReadAllText(filepath, Encoding);

                if (VariablesTxt.Contains("$NumberOfWords"))
                {
                    System.Text.RegularExpressions.Regex regWords = new System.Text.RegularExpressions.Regex(@"\w");

                    int numwords = regWords.Matches(Contents).Count;

                    txt = txt.Replace("$NumberOfWords", numwords.ToString());
                }

                if (VariablesTxt.Contains("$NumberOfLines"))
                {
                    System.Text.RegularExpressions.Regex regLines = new System.Text.RegularExpressions.Regex(@"^[\s\S]*?$", System.Text.RegularExpressions.RegexOptions.Multiline);

                    int numlines = regLines.Matches(Contents).Count;

                    txt = txt.Replace("$NumberOfLines", numlines.ToString());
                }

                if (VariablesTxt.Contains("$NonWhitespaceChars"))
                {
                    System.Text.RegularExpressions.Regex regWords = new System.Text.RegularExpressions.Regex(@"\S", System.Text.RegularExpressions.RegexOptions.Multiline);

                    int numchars = regWords.Matches(Contents).Count;

                    txt = txt.Replace("$NonWhitespaceChars", numchars.ToString());
                }

                if (VariablesTxt.Contains("$DocumentLength"))
                {
                    txt = txt.Replace("$DocumentLength", Contents.Length.ToString());
                }
            }

            return txt;
        }
    }
}