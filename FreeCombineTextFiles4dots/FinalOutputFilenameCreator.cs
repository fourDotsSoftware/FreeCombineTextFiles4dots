using System;
using System.Collections.Generic;
using System.Text;

namespace FreeCombineTextFiles4dots
{
    public class FinalOutputFilenameCreator
    {
        private string Filepath;
        public FinalOutputFilenameCreator(string filepath)
        {
            Filepath = filepath;
        }

        public string Value
        {
            get
            {
                int k = 1;

                string newfp = Filepath;

                while (System.IO.File.Exists(newfp))
                {
                    newfp = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Filepath),
                        System.IO.Path.GetFileNameWithoutExtension(Filepath) + " - " + k.ToString()
                        + System.IO.Path.GetExtension(Filepath));

                    k++;
                }

                return newfp;
            }
        }
    }
}
