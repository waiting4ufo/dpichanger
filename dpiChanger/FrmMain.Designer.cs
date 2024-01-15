namespace dpiChanger
{
    partial class FrmMain
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmMain));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.button1 = new System.Windows.Forms.Button();
            this.directroyBtn = new System.Windows.Forms.Button();
            this.directroyBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.dpiValue = new System.Windows.Forms.NumericUpDown();
            this.listBox = new System.Windows.Forms.ListBox();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.folderBrowserDialog2 = new System.Windows.Forms.FolderBrowserDialog();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dpiValue)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.groupBox1.Controls.Add(this.button1);
            this.groupBox1.Controls.Add(this.directroyBtn);
            this.groupBox1.Controls.Add(this.directroyBox);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.dpiValue);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            this.groupBox1.Enter += new System.EventHandler(this.groupBox1_Enter);
            // 
            // button1
            // 
            this.button1.ForeColor = System.Drawing.SystemColors.ControlText;
            resources.ApplyResources(this.button1, "button1");
            this.button1.Name = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // directroyBtn
            // 
            this.directroyBtn.ForeColor = System.Drawing.SystemColors.ControlText;
            resources.ApplyResources(this.directroyBtn, "directroyBtn");
            this.directroyBtn.Name = "directroyBtn";
            this.directroyBtn.UseVisualStyleBackColor = true;
            this.directroyBtn.Click += new System.EventHandler(this.directroyBtn_Click);
            // 
            // directroyBox
            // 
            this.directroyBox.AllowDrop = true;
            this.directroyBox.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.directroyBox.ForeColor = System.Drawing.SystemColors.Control;
            resources.ApplyResources(this.directroyBox, "directroyBox");
            this.directroyBox.Name = "directroyBox";
            this.directroyBox.ReadOnly = true;
            this.directroyBox.DragDrop += new System.Windows.Forms.DragEventHandler(this.directroyBox_DragDrop);
            this.directroyBox.DragEnter += new System.Windows.Forms.DragEventHandler(this.directroyBox_DragEnter);
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.ForeColor = System.Drawing.SystemColors.Control;
            this.label2.Name = "label2";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.ForeColor = System.Drawing.SystemColors.Control;
            this.label1.Name = "label1";
            // 
            // dpiValue
            // 
            this.dpiValue.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.dpiValue.ForeColor = System.Drawing.SystemColors.Control;
            resources.ApplyResources(this.dpiValue, "dpiValue");
            this.dpiValue.Maximum = new decimal(new int[] {
            9999,
            0,
            0,
            0});
            this.dpiValue.Name = "dpiValue";
            this.dpiValue.Value = new decimal(new int[] {
            96,
            0,
            0,
            0});
            // 
            // listBox
            // 
            this.listBox.AllowDrop = true;
            this.listBox.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.listBox.ForeColor = System.Drawing.SystemColors.Control;
            this.listBox.FormattingEnabled = true;
            resources.ApplyResources(this.listBox, "listBox");
            this.listBox.Name = "listBox";
            this.listBox.TabStop = false;
            this.listBox.DragDrop += new System.Windows.Forms.DragEventHandler(this.listBox_DragDrop);
            this.listBox.DragEnter += new System.Windows.Forms.DragEventHandler(this.listBox_DragEnter);
            // 
            // FrmMain
            // 
            this.AllowDrop = true;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.Controls.Add(this.listBox);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "FrmMain";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.FrmMain_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.FrmMain_DragEnter);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dpiValue)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button directroyBtn;
        private System.Windows.Forms.TextBox directroyBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown dpiValue;
        private System.Windows.Forms.ListBox listBox;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog2;
    }
}

