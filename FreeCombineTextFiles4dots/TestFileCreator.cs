using System;
using System.Collections.Generic;

using System.Text;


namespace FreeCombineTextFiles4dots
{
    class TestFileCreator
    {
        public static void CreateTestFiles()
        {
            string dir = @"c:\code\File Utilities\Test Documents\";

            if (!System.IO.Directory.Exists(dir+"a\""))
            {
                System.IO.Directory.CreateDirectory(dir + "a\\");
            }

            

            if (!System.IO.Directory.Exists(dir + "a\\b\\"))
            {
                System.IO.Directory.CreateDirectory(dir + "\\a\\b");
            }

            CreateFile(dir + "a\\b\\1.txt");

            CreateFile(dir + "a\\b\\1.ini");

            CreateFile(dir + "a\\b\\2.txt");

            CreateFile(dir + "a\\b\\2.ini");

            CreateFile(dir + "a\\b\\3.txt");

            CreateFile(dir + "a\\b\\1\\b11.txt");

            CreateFile(dir + "a\\b\\1\\b11.ini");

            CreateFile(dir + "a\\b\\1\\b12.txt");

            CreateFile(dir + "a\\b\\1\\b12.ini");

            CreateFile(dir + "a\\b\\1\\b13.txt");

            CreateFile(dir + "a\\b\\2\\b21.txt");
            CreateFile(dir + "a\\b\\2\\b21.txt");
            CreateFile(dir + "a\\b\\2\\b21.txt");

            CreateFile(dir + "a\\b\\3\\b31.txt");

            CreateFile(dir + "a\\b\\3\\b31.ini");

            CreateFile(dir + "a\\b\\3\\b32.txt");

            CreateFile(dir + "a\\b\\3\\b32.ini");

            CreateFile(dir + "a\\b\\3\\b33.txt");

            if (!System.IO.Directory.Exists(dir + "a\\c\\"))
            {
                System.IO.Directory.CreateDirectory(dir + "a\\c\\");
            }

            CreateFile(dir + "a\\c\\4.txt");
            CreateFile(dir + "a\\c\\5.txt");
            CreateFile(dir + "a\\c\\6.txt");
            CreateFile(dir + "a\\c\\7.txt");

            CreateFile(dir + "a\\c\\1\\c11.txt");

            CreateFile(dir + "a\\c\\1\\c11.ini");

            CreateFile(dir + "a\\c\\1\\c12.txt");

            CreateFile(dir + "a\\c\\1\\c12.ini");

            CreateFile(dir + "a\\c\\1\\c13.ini");

            CreateFile(dir + "a\\c\\2\\c21.txt");
            CreateFile(dir + "a\\c\\2\\c22.txt");
            CreateFile(dir + "a\\c\\2\\c23.txt");


            if (!System.IO.Directory.Exists(dir + "a\\d\\"))
            {
                System.IO.Directory.CreateDirectory(dir + "a\\d\\");
            }

            CreateFile(dir + "a\\d\\8.txt");
            CreateFile(dir + "a\\d\\9.txt");

            CreateFile(dir + "a\\d\\1\\d11.txt");
            CreateFile(dir + "a\\d\\1\\d12.txt");

        }

        private static void CreateFile(string filepath)
        {
            using (System.IO.StreamWriter sw = new System.IO.StreamWriter(filepath,false))
            {
                for (int k=0;k<30;k++)
                {
                    sw.WriteLine("File "+System.IO.Path.GetFileNameWithoutExtension(filepath));
                }
            }
        }


    }
}
