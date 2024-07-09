namespace FreeCombineTextFiles4dots
{
    partial class frmOptionsWatchers
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmOptionsWatchers));
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.lstDirs = new System.Windows.Forms.ListBox();
            this.btnAdd = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.txtArguments = new System.Windows.Forms.TextBox();
            this.chkRunWindowsStartup = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.chkLocalAccount = new System.Windows.Forms.CheckBox();
            this.chkSystemAccount = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            resources.ApplyResources(this.btnOK, "btnOK");
            this.btnOK.Image = global::FreeCombineTextFiles4dots.Properties.Resources.check;
            this.btnOK.Name = "btnOK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            resources.ApplyResources(this.btnCancel, "btnCancel");
            this.btnCancel.Image = global::FreeCombineTextFiles4dots.Properties.Resources.exit;
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // lstDirs
            // 
            this.lstDirs.FormattingEnabled = true;
            resources.ApplyResources(this.lstDirs, "lstDirs");
            this.lstDirs.Name = "lstDirs";
            // 
            // btnAdd
            // 
            resources.ApplyResources(this.btnAdd, "btnAdd");
            this.btnAdd.Image = global::FreeCombineTextFiles4dots.Properties.Resources.add;
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // button1
            // 
            resources.ApplyResources(this.button1, "button1");
            this.button1.Image = global::FreeCombineTextFiles4dots.Properties.Resources.delete;
            this.button1.Name = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.ForeColor = System.Drawing.Color.DarkBlue;
            this.label2.Name = "label2";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.ForeColor = System.Drawing.Color.DarkBlue;
            this.label1.Name = "label1";
            // 
            // txtArguments
            // 
            resources.ApplyResources(this.txtArguments, "txtArguments");
            this.txtArguments.Name = "txtArguments";
            // 
            // chkRunWindowsStartup
            // 
            resources.ApplyResources(this.chkRunWindowsStartup, "chkRunWindowsStartup");
            this.chkRunWindowsStartup.BackColor = System.Drawing.Color.Transparent;
            this.chkRunWindowsStartup.ForeColor = System.Drawing.Color.DarkBlue;
            this.chkRunWindowsStartup.Name = "chkRunWindowsStartup";
            this.chkRunWindowsStartup.UseVisualStyleBackColor = false;
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.BackColor = System.Drawing.Color.Transparent;
            this.label3.ForeColor = System.Drawing.Color.DarkBlue;
            this.label3.Name = "label3";
            // 
            // chkLocalAccount
            // 
            resources.ApplyResources(this.chkLocalAccount, "chkLocalAccount");
            this.chkLocalAccount.BackColor = System.Drawing.Color.Transparent;
            this.chkLocalAccount.ForeColor = System.Drawing.Color.DarkBlue;
            this.chkLocalAccount.Name = "chkLocalAccount";
            this.chkLocalAccount.UseVisualStyleBackColor = false;
            this.chkLocalAccount.CheckedChanged += new System.EventHandler(this.chkLocalAccount_CheckedChanged);
            // 
            // chkSystemAccount
            // 
            resources.ApplyResources(this.chkSystemAccount, "chkSystemAccount");
            this.chkSystemAccount.BackColor = System.Drawing.Color.Transparent;
            this.chkSystemAccount.ForeColor = System.Drawing.Color.DarkBlue;
            this.chkSystemAccount.Name = "chkSystemAccount";
            this.chkSystemAccount.UseVisualStyleBackColor = false;
            this.chkSystemAccount.CheckedChanged += new System.EventHandler(this.chkSystemAccount_CheckedChanged);
            // 
            // frmOptionsWatchers
            // 
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.chkSystemAccount);
            this.Controls.Add(this.chkLocalAccount);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.chkRunWindowsStartup);
            this.Controls.Add(this.txtArguments);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this.lstDirs);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnCancel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmOptionsWatchers";
            this.Load += new System.EventHandler(this.frmOptionsWatchers_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.ListBox lstDirs;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        public System.Windows.Forms.TextBox txtArguments;
        private System.Windows.Forms.CheckBox chkRunWindowsStartup;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox chkLocalAccount;
        private System.Windows.Forms.CheckBox chkSystemAccount;
    }
}
