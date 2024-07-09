namespace FreeCombineTextFiles4dots
{
    partial class ucFilePattern
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.cmbIncludeWildcards = new System.Windows.Forms.ComboBox();
            this.txtWildcards = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.txtRegExp = new System.Windows.Forms.TextBox();
            this.cmbIncludeRegExp = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // cmbIncludeWildcards
            // 
            this.cmbIncludeWildcards.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbIncludeWildcards.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.cmbIncludeWildcards.FormattingEnabled = true;
            this.cmbIncludeWildcards.Location = new System.Drawing.Point(4, 4);
            this.cmbIncludeWildcards.Margin = new System.Windows.Forms.Padding(4);
            this.cmbIncludeWildcards.Name = "cmbIncludeWildcards";
            this.cmbIncludeWildcards.Size = new System.Drawing.Size(159, 24);
            this.cmbIncludeWildcards.TabIndex = 0;
            // 
            // txtWildcards
            // 
            this.txtWildcards.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.txtWildcards.Location = new System.Drawing.Point(171, 4);
            this.txtWildcards.Margin = new System.Windows.Forms.Padding(4);
            this.txtWildcards.Name = "txtWildcards";
            this.txtWildcards.Size = new System.Drawing.Size(202, 22);
            this.txtWildcards.TabIndex = 1;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.BackColor = System.Drawing.Color.Transparent;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold);
            this.label5.ForeColor = System.Drawing.Color.DarkBlue;
            this.label5.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label5.Location = new System.Drawing.Point(381, 7);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(369, 16);
            this.label5.TabIndex = 75;
            this.label5.Text = "Wildcards (semi-colon-separated) e.g. *.txt;*.ini;*.log";
            this.label5.Click += new System.EventHandler(this.ucFilePattern_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold);
            this.label1.ForeColor = System.Drawing.Color.DarkBlue;
            this.label1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label1.Location = new System.Drawing.Point(381, 39);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(144, 16);
            this.label1.TabIndex = 78;
            this.label1.Text = "Regular Expression";
            this.label1.Click += new System.EventHandler(this.ucFilePattern_Click);
            // 
            // txtRegExp
            // 
            this.txtRegExp.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.txtRegExp.Location = new System.Drawing.Point(171, 36);
            this.txtRegExp.Margin = new System.Windows.Forms.Padding(4);
            this.txtRegExp.Name = "txtRegExp";
            this.txtRegExp.Size = new System.Drawing.Size(202, 22);
            this.txtRegExp.TabIndex = 3;
            // 
            // cmbIncludeRegExp
            // 
            this.cmbIncludeRegExp.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbIncludeRegExp.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.cmbIncludeRegExp.FormattingEnabled = true;
            this.cmbIncludeRegExp.Location = new System.Drawing.Point(4, 36);
            this.cmbIncludeRegExp.Margin = new System.Windows.Forms.Padding(4);
            this.cmbIncludeRegExp.Name = "cmbIncludeRegExp";
            this.cmbIncludeRegExp.Size = new System.Drawing.Size(159, 24);
            this.cmbIncludeRegExp.TabIndex = 2;
            // 
            // ucFilePattern
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtRegExp);
            this.Controls.Add(this.cmbIncludeRegExp);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.txtWildcards);
            this.Controls.Add(this.cmbIncludeWildcards);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(161)));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximumSize = new System.Drawing.Size(752, 68);
            this.MinimumSize = new System.Drawing.Size(752, 68);
            this.Name = "ucFilePattern";
            this.Size = new System.Drawing.Size(750, 66);
            this.Click += new System.EventHandler(this.ucFilePattern_Click);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label1;
        public System.Windows.Forms.ComboBox cmbIncludeWildcards;
        public System.Windows.Forms.TextBox txtWildcards;
        public System.Windows.Forms.TextBox txtRegExp;
        public System.Windows.Forms.ComboBox cmbIncludeRegExp;
    }
}
