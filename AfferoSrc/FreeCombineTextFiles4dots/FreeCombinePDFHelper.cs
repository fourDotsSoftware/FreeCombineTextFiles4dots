using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iTextSharp;
using iTextSharp.text.pdf;
using iTextSharp.text;
using System.IO;

namespace FreeCombineTextFiles4dots
{
    class FreeCombinePDFHelper
    {
        public static bool FreeCombinePDF(System.Data.DataTable dt,string outputFile)
        {
            Document document = new Document();
            PdfCopy copy = new PdfSmartCopy(document, new FileStream(outputFile,FileMode.OpenOrCreate));
            document.Open();

            for (int k = 0; k < dt.Rows.Count; k++)
            {
                if (frmMain.Instance.bwWork.CancellationPending)
                {
                    return true;
                }

                PdfReader reader;                
                // loop over readers
                // add the PDF to PdfCopy

                string password = dt.Rows[k]["password"].ToString();

                //3reader = new PdfReader(dt.Rows[k]["fullfilepath"].ToString());

                if (password == string.Empty)
                {
                    reader = new PdfReader(dt.Rows[k]["fullfilepath"].ToString());
                }
                else
                {
                    reader = new PdfReader(dt.Rows[k]["fullfilepath"].ToString(),Encoding.ASCII.GetBytes(password));
                }

                copy.AddDocument(reader);
                reader.Close();

                frmMain.Instance.bwWork.ReportProgress(0,System.IO.Path.GetFileName(dt.Rows[k]["fullfilepath"].ToString()));
            }
            // end loop
            document.Close();

            return true;
        }
    }
}
