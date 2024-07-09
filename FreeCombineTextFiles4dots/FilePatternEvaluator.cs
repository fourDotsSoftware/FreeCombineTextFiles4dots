using System;
using System.Collections.Generic;

using System.Text;


namespace FreeCombineTextFiles4dots
{
    class FilePatternEvaluator
    {
        public static bool IsFilePattern(string filepath)
        {
            try
            {
                string fn = System.IO.Path.GetFileName(filepath);

                bool ismatch = false;

                bool notempty = false;

                if (Properties.Settings.Default.FilePattern.Trim() != string.Empty)
                {
                    string[] sz = Properties.Settings.Default.FilePattern.Split(new string[] { "|||" }, StringSplitOptions.RemoveEmptyEntries);

                    for (int k = 0; k < sz.Length; k += 2)
                    {
                        string sinc = sz[k].Substring(0, 1);
                        string sval = sz[k].Substring(2);
                        if (sval == "-") sval = "";

                        if (sval != string.Empty)
                        {
                            notempty = true;

                            string[] swi = sval.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);

                            for (int m = 0; m < swi.Length; m++)
                            {
                                string sval2 = RegexEscape(swi[m]);
                                sval2 = sval2.Replace(@"\?", ".{1}").Replace(@"\*", ".*");

                                System.Text.RegularExpressions.Regex reg1 = new System.Text.RegularExpressions.Regex(sval2);

                                if (sinc == "1")
                                {
                                    if (reg1.IsMatch(fn))
                                    {
                                        return false;
                                    }
                                }
                                else if (sinc == "0")
                                {
                                    if (reg1.IsMatch(fn))
                                    {
                                        ismatch = true;
                                    }
                                }
                            }
                        }

                        string sinc1 = sz[k + 1].Substring(0, 1);
                        string sval1 = sz[k + 1].Substring(2);
                        if (sval1 == "-") sval1 = "";

                        if (sval1 != string.Empty)
                        {
                            notempty = true;

                            System.Text.RegularExpressions.Regex reg1 = new System.Text.RegularExpressions.Regex(sval1);

                            if (sinc1 == "1")
                            {
                                if (reg1.IsMatch(fn))
                                {
                                    return false;
                                }
                            }
                            else if (sinc1 == "0")
                            {
                                if (reg1.IsMatch(fn))
                                {
                                    ismatch = true;
                                }
                            }

                        }
                    }

                    if (notempty)
                    {
                        return ismatch;
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                Module.ShowMessage("Invalid Wildcards or Regular Expression for File Filter !");
                throw;
            }
        }

        public static string RegexEscape(string searchpattern)
        {
            return System.Text.RegularExpressions.Regex.Escape(searchpattern).Replace(@"\\\ ", " ");
        }

    }
}