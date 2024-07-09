using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace FreeCombineTextFiles4dots
{
    public partial class frmPleaseBuy : FreeCombineTextFiles4dots.CustomForm
    {
        public frmPleaseBuy()
        {
            InitializeComponent();
        }

        private void btnBuy_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(Module.BuyURL);
        }

        private void frmPleaseBuy_Load(object sender, EventArgs e)
        {
            label1.Text = TranslateHelper.Translate("The Trial Version of Free Combine Text 4dots allows only maximum 5 documents to be merged.\n\nPlease buy the Application for Unlimited Number of Documents and Unlimited Functionality.\n");
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }
    }
}
