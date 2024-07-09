using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Data;

namespace FreeCombineTextFiles4dots
{
    public class FileSorter
    {
        public static DataTable SortAudioDataTable(DataTable dt, string sortmode,bool sortdesc,bool numericStringSort)
        {
            List<FileSortRow> lst = new List<FileSortRow>();

            for (int k = 0; k < dt.Rows.Count; k++)
            {
                FileSortRow ar = new FileSortRow(dt.Rows[k], k, sortmode,sortdesc,numericStringSort);

                lst.Add(ar);
            }

            lst.Sort();

            DataTable dt0 = dt.Clone();

            for (int k = 0; k < lst.Count; k++)
            {
                DataRow dri = lst[k].DataRow;

                DataRow dro = CopyDataRow(dt0, dri);

                dt0.Rows.Add(dro);
            }

            return dt0;
        }

        private static DataRow CopyDataRow(DataTable dt0, DataRow dri)
        {
            DataRow dr = dt0.NewRow();

            for (int k = 0; k < dt0.Columns.Count; k++)
            {
                dr[k] = dri[k];
            }

            return dr;
        }
    }

    public class FileSortRow : IComparable<FileSortRow>
    {
        public string SortMode = "";

        public int RowIndex = -1;
        public string Filepath = "";
        public string Filename = "";

        public long FileSize = -1;        

        public DateTime CreationDate = DateTime.Now;
        public DateTime LastModificationDate = DateTime.Now;
                
        public DataRow DataRow = null;               

        public bool SortDescending = false;

        public bool NumericStringSort = true;

        public FileSortRow(DataRow dr, int rindex, string sortmode, bool sortdesc, bool numericStringSort)
        {
            RowIndex = rindex;

            DataRow = dr;

            SortMode = sortmode;

            SortDescending = sortdesc;

            NumericStringSort = numericStringSort;

            Filepath = dr["fullfilepath"].ToString();

            Filename = System.IO.Path.GetFileName(Filepath);

            System.IO.FileInfo fi = new System.IO.FileInfo(Filepath);

            FileSize = fi.Length;

            CreationDate = fi.CreationTime;

            LastModificationDate = fi.LastWriteTime;            
            
        }

        public int CompareTo(FileSortRow a2)
        {
            FileSortRow a1 = this;

            int si = 1;

            if (SortDescending)
            {
                si = -1;
            }

            if (SortMode == "f:filename")
            {
                return si* CompareNumericString(a1.Filename, a2.Filename);
            }
            else if (SortMode == "f:filepath")
            {
                return si * CompareNumericString(a1.Filepath, a2.Filepath);
            }            
            else if (SortMode == "f:size")
            {
                return si * a1.FileSize.CompareTo(a2.FileSize);
            }
            else if (SortMode == "f:cdate")
            {
                return si * a1.CreationDate.CompareTo(a2.CreationDate);
            }
            else if (SortMode == "f:lmdate")
            {
                return si * a1.LastModificationDate.CompareTo(a2.LastModificationDate);
            }                        

            return 0;
        }

        public int CompareNumericString(string x, string y)
        {
            if (!NumericStringSort)
            {
                return x.CompareTo(y);
            }

            var regex = new Regex(@"^(\d+)");

            //regex = new Regex("^(d+?)");

            var regex2 = new Regex(@"^(\d+)\.(\d+)");

            var regex3 = new Regex(@"^(\d+)\.(\d+)\.(\d+)");

            var regex4 = new Regex(@"^(\d+)\.(\d+)\.(\d+)\.(\d+)");            

            // run the regex on both strings
            var xRegexResult = regex.Match(x);
            var yRegexResult = regex.Match(y);

            // check if they are both numbers
            if (xRegexResult.Success && yRegexResult.Success)
            {
                int comp1 = int.Parse(xRegexResult.Groups[1].Value).CompareTo(int.Parse(yRegexResult.Groups[1].Value));

                if (comp1 == 0)
                {
                    xRegexResult = regex2.Match(x);
                    yRegexResult = regex2.Match(y);

                    if (xRegexResult.Success && yRegexResult.Success)
                    {
                        int comp2 = int.Parse(xRegexResult.Groups[2].Value).CompareTo(int.Parse(yRegexResult.Groups[2].Value));

                        if (comp2 == 0)
                        {
                            xRegexResult = regex3.Match(x);
                            yRegexResult = regex3.Match(y);

                            if (xRegexResult.Success && yRegexResult.Success)
                            {
                                int comp3 = int.Parse(xRegexResult.Groups[3].Value).CompareTo(int.Parse(yRegexResult.Groups[3].Value));

                                if (comp3 == 0)
                                {
                                    xRegexResult = regex2.Match(x);
                                    yRegexResult = regex2.Match(y);

                                    if (xRegexResult.Success && yRegexResult.Success)
                                    {
                                        int comp4 = int.Parse(xRegexResult.Groups[4].Value).CompareTo(int.Parse(yRegexResult.Groups[4].Value));

                                        return comp4;
                                    }
                                    else return comp3;
                                }
                                else return comp3;
                            }
                            else return comp2;
                        }
                        else return comp2;
                    }
                }
                else return comp1;
            }
            // otherwise return as string comparison
            return x.CompareTo(y);
        }
    }
}