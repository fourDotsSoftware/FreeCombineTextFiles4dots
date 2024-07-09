﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace FreeCombineTextFiles4dots
{
    public partial class CustomForm : Form
    {
        private bool LoadComplete=false;

        private Image BackGroundImg = null;

        public bool GradientBackground = true;

        public CustomForm()
        {
            
            InitializeComponent();
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            this.ResizeRedraw = true;            

            this.Icon = FreeCombineTextFiles4dots.Properties.Resources.free_pdf_combine_48;            
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            if (!GradientBackground)
            {
                return;
            }

            if (BackGroundImg != null)
            {
                BackGroundImg.Dispose();
            }

            if (this.IsDisposed) return;

            BackGroundImg = new Bitmap(this.Width, this.Height);

            using (Graphics g = Graphics.FromImage(BackGroundImg))
            {
                int x = this.Width;
                int y = this.Height;

                System.Drawing.Drawing2D.LinearGradientBrush
                    lgBrush = new System.Drawing.Drawing2D.LinearGradientBrush
                    (new Rectangle(0, 0, x, y),
                    Color.White, Color.FromArgb(190, 190, 190), System.Drawing.Drawing2D.LinearGradientMode.ForwardDiagonal);
                lgBrush.GammaCorrection = true;
                g.FillRectangle(lgBrush, 0, 0, x, y);
            }

            this.BackgroundImage = BackGroundImg;

            this.Invalidate();
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            base.OnPaintBackground(e);

            return;

            if (!LoadComplete) return;

            try
            {
                
                

            }
            catch
            {
                base.OnPaintBackground(e);
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            try
            {
                this.Icon = FreeCombineTextFiles4dots.Properties.Resources.free_pdf_combine_48;
                
                base.Cursor = Cursors.WaitCursor;
                base.OnLoad(e);
                base.Cursor = null;

                foreach (Control co in this.Controls)
                {
                    if (co is Button)
                    {
                        if (co.Name == "btnOK")
                        {
                            this.AcceptButton = (Button)co;
                        }
                        else if (co.Name == "btnCancel")
                        {
                            this.CancelButton = (Button)co;
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                //3Module.ShowError(ex);
            }
            finally
            {
                Cursor.Current = null;
                
                LoadComplete = true;
                
                this.Invalidate();
            }
        }

        private void CustomForm_Activated(object sender, EventArgs e)
        {
            this.Invalidate();
        }
    }
}