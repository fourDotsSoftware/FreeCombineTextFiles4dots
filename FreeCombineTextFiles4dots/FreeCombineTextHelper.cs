using System;
using System.Collections.Generic;

using System.Text;

using iTextSharp;
using iTextSharp.text.pdf;
using iTextSharp.text;
using System.IO;

namespace FreeCombineTextFiles4dots
{
    class FreeCombineTextHelper
    {
        public static bool FreeCombineTextFiles(System.Data.DataTable dt, string outputFile, bool foldersep)
        {
            if (System.IO.File.Exists(outputFile))
            {
                /*
                FileInfo fi = new FileInfo(outputFile);
                fi.Attributes = FileAttributes.Normal;
                fi.Delete();*/

                FinalOutputFilenameCreator fn = new FinalOutputFilenameCreator(outputFile);

                outputFile = fn.Value;
            }

            Encoding enc = Encoding.Default;

            string enctxt = Properties.Settings.Default.SpecificEncoding;

            int epos = enctxt.IndexOf(" - ");

            int encnum = int.Parse(enctxt.Substring(0, epos));

            Encoding enc2 = Encoding.GetEncoding(encnum);

            Encoding enc3 = Encoding.Default;

            using (StreamReader sr = new StreamReader(dt.Rows[0]["fullfilepath"].ToString(), true))
            {
                string line = sr.ReadLine();

                enc3 = sr.CurrentEncoding;
            }

            if (Properties.Settings.Default.OptEncoding == 1)
            {
                enc = enc3;
            }
            else if (Properties.Settings.Default.OptEncoding == 2)
            {
                enc = enc2;
            }

            using (StreamWriter sw = new StreamWriter(outputFile, false, enc))
            {
                for (int k = 0; k < dt.Rows.Count; k++)
                {
                    if (frmMain.Instance.bwWork.CancellationPending)
                    {
                        return true;
                    }

                    using (StreamReader sr = new StreamReader(dt.Rows[k]["fullfilepath"].ToString()))
                    {
                        if
                            (
                            (Properties.Settings.Default.ChkAddTextHeader && Properties.Settings.Default.ChkOnlyFirstHeader
                            && k == 0) || (Properties.Settings.Default.ChkAddTextHeader && !Properties.Settings.Default.ChkOnlyFirstHeader)
                            )
                        {
                            VariablesText var = new VariablesText(Properties.Settings.Default.TxtHeader,
                                dt.Rows[k]["fullfilepath"].ToString(), Properties.Settings.Default.HeaderDateFormat,
                                enc, (k + 1), dt.Rows.Count);

                            sw.WriteLine(var.GetText());

                            if (Properties.Settings.Default.ChkAddLinesHeader)
                            {
                                for (int m = 0; m < Properties.Settings.Default.NumLinesHeader; m++)
                                {
                                    sw.WriteLine();
                                }
                            }
                        }


                        string line = null;

                        while ((line = sr.ReadLine()) != null)
                        {
                            if (frmMain.Instance.bwWork.CancellationPending)
                            {
                                return true;
                            }

                            sw.WriteLine(line);
                        }

                        if
                            (
                            (Properties.Settings.Default.ChkAddTextFooter && Properties.Settings.Default.ChkOnlyFirstFooter
                            && k == 0) || (Properties.Settings.Default.ChkAddTextFooter && !Properties.Settings.Default.ChkOnlyFirstFooter)
                            )
                        {
                            VariablesText var = new VariablesText(Properties.Settings.Default.TxtFooter,
                                dt.Rows[k]["fullfilepath"].ToString(), Properties.Settings.Default.FooterDateFormat,
                                enc, (k + 1), dt.Rows.Count);

                            sw.WriteLine(var.GetText());

                            if (Properties.Settings.Default.ChkAddLinesFooter)
                            {
                                for (int m = 0; m < Properties.Settings.Default.NumLinesFooter; m++)
                                {
                                    sw.WriteLine();
                                }
                            }
                        }
                    }


                    if (!foldersep)
                    {
                        frmMain.Instance.bwWork.ReportProgress(0, System.IO.Path.GetFileName(dt.Rows[k]["fullfilepath"].ToString()));
                    }
                }
                // end loop

                return true;
            }
        }
    }

}
