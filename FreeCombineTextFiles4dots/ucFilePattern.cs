using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;

using System.Text;

using System.Windows.Forms;

namespace FreeCombineTextFiles4dots
{
    public partial class ucFilePattern : UserControl
    {
        private bool _Selected = false;
        public bool Selected
        {
            get
            {
                return _Selected;
            }
            set
            {
                _Selected = value;

                if (_Selected)
                {
                    this.BackColor = Color.Orange;

                    for (int k=0;k<this.Parent.Controls.Count;k++)
                    {
                        ucFilePattern uc = Parent.Controls[k] as ucFilePattern;

                        if (uc!=this)
                        {
                            uc.Selected = false;
                        }
                    }
                }
                else
                {
                    this.BackColor = SystemColors.Control;
                }
            }
        }
        public ucFilePattern()
        {
            InitializeComponent();

            cmbIncludeRegExp.Items.Add(TranslateHelper.Translate("Include"));
            cmbIncludeRegExp.Items.Add(TranslateHelper.Translate("Exclude"));

            cmbIncludeWildcards.Items.Add(TranslateHelper.Translate("Include"));
            cmbIncludeWildcards.Items.Add(TranslateHelper.Translate("Exclude"));

            cmbIncludeRegExp.SelectedIndex = 0;
            cmbIncludeWildcards.SelectedIndex = 0;

            
        }

        private void ucFilePattern_Click(object sender, EventArgs e)
        {
            for (int k=0;k<Parent.Controls.Count;k++)
            {
                ucFilePattern uc = Parent.Controls[k] as ucFilePattern;
                uc.Selected = false;
            }

            Selected = true;
        }
    }
}
