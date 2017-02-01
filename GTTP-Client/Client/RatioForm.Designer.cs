namespace ClientRaw
{
    partial class RatioForm
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
            this.autoRdBtn = new System.Windows.Forms.RadioButton();
            this.rdManualBtn = new System.Windows.Forms.RadioButton();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label6 = new System.Windows.Forms.Label();
            this.lbSum = new System.Windows.Forms.Label();
            this.txtA = new ClientRaw.NumericTextBox();
            this.txtC = new ClientRaw.NumericTextBox();
            this.txtT = new ClientRaw.NumericTextBox();
            this.txtG = new ClientRaw.NumericTextBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // autoRdBtn
            // 
            this.autoRdBtn.AutoSize = true;
            this.autoRdBtn.Location = new System.Drawing.Point(12, 59);
            this.autoRdBtn.Name = "autoRdBtn";
            this.autoRdBtn.Size = new System.Drawing.Size(73, 17);
            this.autoRdBtn.TabIndex = 0;
            this.autoRdBtn.TabStop = true;
            this.autoRdBtn.Text = "Automatic";
            this.autoRdBtn.UseVisualStyleBackColor = true;
            this.autoRdBtn.CheckedChanged += new System.EventHandler(this.autoRdBtn_CheckedChanged);
            // 
            // rdManualBtn
            // 
            this.rdManualBtn.AutoSize = true;
            this.rdManualBtn.Location = new System.Drawing.Point(197, 59);
            this.rdManualBtn.Name = "rdManualBtn";
            this.rdManualBtn.Size = new System.Drawing.Size(65, 17);
            this.rdManualBtn.TabIndex = 1;
            this.rdManualBtn.TabStop = true;
            this.rdManualBtn.Text = "Manaual";
            this.rdManualBtn.UseVisualStyleBackColor = true;
            this.rdManualBtn.CheckedChanged += new System.EventHandler(this.rdManualBtn_CheckedChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 25);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(14, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "A";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(7, 66);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(14, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "C";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(7, 142);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(13, 13);
            this.label3.TabIndex = 9;
            this.label3.Text = "T";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 101);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(14, 13);
            this.label4.TabIndex = 8;
            this.label4.Text = "G";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(13, 25);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(91, 13);
            this.label5.TabIndex = 12;
            this.label5.Text = "Select Ratio Type";
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(44, 327);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 13;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(182, 327);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 14;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lbSum);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.txtA);
            this.groupBox1.Controls.Add(this.txtC);
            this.groupBox1.Controls.Add(this.txtT);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.txtG);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Location = new System.Drawing.Point(16, 97);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(200, 209);
            this.groupBox1.TabIndex = 15;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Ratios (Each between 0.0 and 1.0)";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 181);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(38, 13);
            this.label6.TabIndex = 12;
            this.label6.Text = "Sum =";
            // 
            // lbSum
            // 
            this.lbSum.AutoSize = true;
            this.lbSum.Location = new System.Drawing.Point(51, 181);
            this.lbSum.Name = "lbSum";
            this.lbSum.Size = new System.Drawing.Size(0, 13);
            this.lbSum.TabIndex = 13;
            // 
            // txtA
            // 
            this.txtA.AllowSpace = false;
            this.txtA.Location = new System.Drawing.Point(38, 22);
            this.txtA.Name = "txtA";
            this.txtA.Size = new System.Drawing.Size(100, 20);
            this.txtA.TabIndex = 6;
            this.txtA.TextChanged += new System.EventHandler(this.texBox_TextChanged);
            // 
            // txtC
            // 
            this.txtC.AllowSpace = false;
            this.txtC.Location = new System.Drawing.Point(38, 63);
            this.txtC.Name = "txtC";
            this.txtC.Size = new System.Drawing.Size(100, 20);
            this.txtC.TabIndex = 7;
            this.txtC.TextChanged += new System.EventHandler(this.texBox_TextChanged);
            // 
            // txtT
            // 
            this.txtT.AllowSpace = false;
            this.txtT.Location = new System.Drawing.Point(38, 139);
            this.txtT.Name = "txtT";
            this.txtT.Size = new System.Drawing.Size(100, 20);
            this.txtT.TabIndex = 11;
            this.txtT.TextChanged += new System.EventHandler(this.texBox_TextChanged);
            // 
            // txtG
            // 
            this.txtG.AllowSpace = false;
            this.txtG.Location = new System.Drawing.Point(38, 98);
            this.txtG.Name = "txtG";
            this.txtG.Size = new System.Drawing.Size(100, 20);
            this.txtG.TabIndex = 10;
            this.txtG.TextChanged += new System.EventHandler(this.texBox_TextChanged);
            // 
            // RatioForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(319, 390);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.rdManualBtn);
            this.Controls.Add(this.autoRdBtn);
            this.Name = "RatioForm";
            this.Text = "RatioForm";
            this.Load += new System.EventHandler(this.RatioForm_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RadioButton autoRdBtn;
        private System.Windows.Forms.RadioButton rdManualBtn;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private ClientRaw.NumericTextBox txtA;
        private ClientRaw.NumericTextBox txtC;
        private ClientRaw.NumericTextBox txtT;
        private ClientRaw.NumericTextBox txtG;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label lbSum;
        private System.Windows.Forms.Label label6;

    }
}