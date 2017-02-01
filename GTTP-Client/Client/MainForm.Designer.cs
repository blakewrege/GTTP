namespace ClientRaw
{
    partial class MainForm
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
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataVisualization.Charting.Series series16 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.DataPoint dataPoint21 = new System.Windows.Forms.DataVisualization.Charting.DataPoint(0D, 8D);
            System.Windows.Forms.DataVisualization.Charting.DataPoint dataPoint22 = new System.Windows.Forms.DataVisualization.Charting.DataPoint(0D, 5D);
            System.Windows.Forms.DataVisualization.Charting.Series series17 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series18 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.DataPoint dataPoint23 = new System.Windows.Forms.DataVisualization.Charting.DataPoint(0D, 10D);
            System.Windows.Forms.DataVisualization.Charting.DataPoint dataPoint24 = new System.Windows.Forms.DataVisualization.Charting.DataPoint(0D, 12D);
            System.Windows.Forms.DataVisualization.Charting.Title title6 = new System.Windows.Forms.DataVisualization.Charting.Title();
            this.chart1 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.btnGetFile = new System.Windows.Forms.Button();
            this.txtServerAddress = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtFileName = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.rdTransferTime = new System.Windows.Forms.RadioButton();
            this.rdTotalTime = new System.Windows.Forms.RadioButton();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.ckboxAutoGenerate = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.comBoxDataSize = new System.Windows.Forms.ComboBox();
            this.lbProgress = new System.Windows.Forms.Label();
            this.lbDownloading = new System.Windows.Forms.Label();
            this.btnRatio = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.lbTCount = new System.Windows.Forms.Label();
            this.lbGCount = new System.Windows.Forms.Label();
            this.lbCCount = new System.Windows.Forms.Label();
            this.lbACount = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnCompressed = new System.Windows.Forms.RadioButton();
            this.btnNotCompressed = new System.Windows.Forms.RadioButton();
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // chart1
            // 
            this.chart1.Location = new System.Drawing.Point(23, 12);
            this.chart1.Name = "chart1";
            series16.Name = "Series1";
            dataPoint21.AxisLabel = "Original Data";
            dataPoint21.IsValueShownAsLabel = true;
            dataPoint21.Label = "";
            dataPoint22.AxisLabel = "";
            dataPoint22.Label = "5";
            series16.Points.Add(dataPoint21);
            series16.Points.Add(dataPoint22);
            series17.IsValueShownAsLabel = true;
            series17.LabelAngle = 90;
            series17.Name = "Series3";
            series18.Name = "Series4";
            dataPoint23.AxisLabel = "Coded";
            dataPoint23.Label = "10";
            dataPoint24.AxisLabel = "Encoded Data";
            dataPoint24.Label = "Compressed";
            series18.Points.Add(dataPoint23);
            series18.Points.Add(dataPoint24);
            this.chart1.Series.Add(series16);
            this.chart1.Series.Add(series17);
            this.chart1.Series.Add(series18);
            this.chart1.Size = new System.Drawing.Size(517, 411);
            this.chart1.TabIndex = 1;
            this.chart1.Text = "chart1";
            title6.Docking = System.Windows.Forms.DataVisualization.Charting.Docking.Bottom;
            title6.Name = "Title1";
            title6.Text = "Diagram to show file before and after coding and compression";
            this.chart1.Titles.Add(title6);
            // 
            // btnGetFile
            // 
            this.btnGetFile.Location = new System.Drawing.Point(374, 610);
            this.btnGetFile.Name = "btnGetFile";
            this.btnGetFile.Size = new System.Drawing.Size(166, 44);
            this.btnGetFile.TabIndex = 2;
            this.btnGetFile.Text = "Get Data";
            this.btnGetFile.UseVisualStyleBackColor = true;
            this.btnGetFile.Click += new System.EventHandler(this.btnGetData_Click);
            // 
            // txtServerAddress
            // 
            this.txtServerAddress.Location = new System.Drawing.Point(327, 456);
            this.txtServerAddress.Name = "txtServerAddress";
            this.txtServerAddress.Size = new System.Drawing.Size(442, 20);
            this.txtServerAddress.TabIndex = 4;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(208, 456);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(81, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Server Address";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(209, 497);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(91, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "Text File Location";
            // 
            // txtFileName
            // 
            this.txtFileName.Location = new System.Drawing.Point(327, 497);
            this.txtFileName.Name = "txtFileName";
            this.txtFileName.Size = new System.Drawing.Size(442, 20);
            this.txtFileName.TabIndex = 6;
            this.toolTip1.SetToolTip(this.txtFileName, "Enter text file name like mydata.txt");
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.rdTransferTime);
            this.groupBox2.Controls.Add(this.rdTotalTime);
            this.groupBox2.Location = new System.Drawing.Point(23, 456);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(130, 77);
            this.groupBox2.TabIndex = 4;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Timing";
            // 
            // rdTransferTime
            // 
            this.rdTransferTime.AutoSize = true;
            this.rdTransferTime.Location = new System.Drawing.Point(6, 48);
            this.rdTransferTime.Name = "rdTransferTime";
            this.rdTransferTime.Size = new System.Drawing.Size(112, 17);
            this.rdTransferTime.TabIndex = 1;
            this.rdTransferTime.Text = "Transfer time only";
            this.rdTransferTime.UseVisualStyleBackColor = true;
            this.rdTransferTime.CheckedChanged += new System.EventHandler(this.rdTransferTime_CheckedChanged);
            // 
            // rdTotalTime
            // 
            this.rdTotalTime.AutoSize = true;
            this.rdTotalTime.Checked = true;
            this.rdTotalTime.Location = new System.Drawing.Point(6, 21);
            this.rdTotalTime.Name = "rdTotalTime";
            this.rdTotalTime.Size = new System.Drawing.Size(72, 17);
            this.rdTotalTime.TabIndex = 0;
            this.rdTotalTime.TabStop = true;
            this.rdTotalTime.Text = "Total time";
            this.rdTotalTime.UseVisualStyleBackColor = true;
            this.rdTotalTime.CheckedChanged += new System.EventHandler(this.rdTotalTime_CheckedChanged);
            // 
            // toolTip1
            // 
            this.toolTip1.ToolTipTitle = "ToolTip";
            // 
            // ckboxAutoGenerate
            // 
            this.ckboxAutoGenerate.AutoSize = true;
            this.ckboxAutoGenerate.Location = new System.Drawing.Point(212, 538);
            this.ckboxAutoGenerate.Name = "ckboxAutoGenerate";
            this.ckboxAutoGenerate.Size = new System.Drawing.Size(214, 17);
            this.ckboxAutoGenerate.TabIndex = 8;
            this.ckboxAutoGenerate.Text = "Get Auto Generated Data  From Server";
            this.ckboxAutoGenerate.UseVisualStyleBackColor = true;
            this.ckboxAutoGenerate.CheckedChanged += new System.EventHandler(this.ckboxAutoGenerate_CheckedChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(209, 564);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(52, 13);
            this.label3.TabIndex = 10;
            this.label3.Text = "Data Size";
            // 
            // comBoxDataSize
            // 
            this.comBoxDataSize.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.comBoxDataSize.FormattingEnabled = true;
            this.comBoxDataSize.Items.AddRange(new object[] {
            "MB",
            "GB"});
            this.comBoxDataSize.Location = new System.Drawing.Point(262, 561);
            this.comBoxDataSize.Name = "comBoxDataSize";
            this.comBoxDataSize.Size = new System.Drawing.Size(38, 21);
            this.comBoxDataSize.TabIndex = 12;
            // 
            // lbProgress
            // 
            this.lbProgress.AutoSize = true;
            this.lbProgress.Location = new System.Drawing.Point(577, 43);
            this.lbProgress.Name = "lbProgress";
            this.lbProgress.Size = new System.Drawing.Size(0, 13);
            this.lbProgress.TabIndex = 13;
            // 
            // lbDownloading
            // 
            this.lbDownloading.AutoSize = true;
            this.lbDownloading.Location = new System.Drawing.Point(577, 87);
            this.lbDownloading.Name = "lbDownloading";
            this.lbDownloading.Size = new System.Drawing.Size(0, 13);
            this.lbDownloading.TabIndex = 14;
            // 
            // btnRatio
            // 
            this.btnRatio.Enabled = false;
            this.btnRatio.Location = new System.Drawing.Point(596, 561);
            this.btnRatio.Name = "btnRatio";
            this.btnRatio.Size = new System.Drawing.Size(97, 29);
            this.btnRatio.TabIndex = 15;
            this.btnRatio.Text = "Ratios";
            this.btnRatio.UseVisualStyleBackColor = true;
            this.btnRatio.Click += new System.EventHandler(this.btnRatio_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.lbTCount);
            this.groupBox3.Controls.Add(this.lbGCount);
            this.groupBox3.Controls.Add(this.lbCCount);
            this.groupBox3.Controls.Add(this.lbACount);
            this.groupBox3.Controls.Add(this.label7);
            this.groupBox3.Controls.Add(this.label6);
            this.groupBox3.Controls.Add(this.label5);
            this.groupBox3.Controls.Add(this.label4);
            this.groupBox3.Location = new System.Drawing.Point(580, 194);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(200, 200);
            this.groupBox3.TabIndex = 18;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Statistic";
            // 
            // lbTCount
            // 
            this.lbTCount.AutoSize = true;
            this.lbTCount.Location = new System.Drawing.Point(33, 143);
            this.lbTCount.Name = "lbTCount";
            this.lbTCount.Size = new System.Drawing.Size(13, 13);
            this.lbTCount.TabIndex = 7;
            this.lbTCount.Text = "T";
            // 
            // lbGCount
            // 
            this.lbGCount.AutoSize = true;
            this.lbGCount.Location = new System.Drawing.Point(33, 106);
            this.lbGCount.Name = "lbGCount";
            this.lbGCount.Size = new System.Drawing.Size(14, 13);
            this.lbGCount.TabIndex = 6;
            this.lbGCount.Text = "G";
            // 
            // lbCCount
            // 
            this.lbCCount.AutoSize = true;
            this.lbCCount.Location = new System.Drawing.Point(33, 72);
            this.lbCCount.Name = "lbCCount";
            this.lbCCount.Size = new System.Drawing.Size(14, 13);
            this.lbCCount.TabIndex = 5;
            this.lbCCount.Text = "C";
            // 
            // lbACount
            // 
            this.lbACount.AutoSize = true;
            this.lbACount.Location = new System.Drawing.Point(33, 36);
            this.lbACount.Name = "lbACount";
            this.lbACount.Size = new System.Drawing.Size(14, 13);
            this.lbACount.TabIndex = 4;
            this.lbACount.Text = "A";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(6, 143);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(20, 13);
            this.label7.TabIndex = 3;
            this.label7.Text = "T :";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 106);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(21, 13);
            this.label6.TabIndex = 2;
            this.label6.Text = "G :";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 72);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(21, 13);
            this.label5.TabIndex = 1;
            this.label5.Text = "C :";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 36);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(21, 13);
            this.label4.TabIndex = 0;
            this.label4.Text = "A :";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnCompressed);
            this.groupBox1.Controls.Add(this.btnNotCompressed);
            this.groupBox1.Location = new System.Drawing.Point(23, 561);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(130, 81);
            this.groupBox1.TabIndex = 19;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Data Compression";
            // 
            // btnCompressed
            // 
            this.btnCompressed.AutoSize = true;
            this.btnCompressed.Location = new System.Drawing.Point(6, 44);
            this.btnCompressed.Name = "btnCompressed";
            this.btnCompressed.Size = new System.Drawing.Size(84, 17);
            this.btnCompressed.TabIndex = 1;
            this.btnCompressed.Text = "Compressed";
            this.btnCompressed.UseVisualStyleBackColor = true;
            this.btnCompressed.CheckedChanged += new System.EventHandler(this.btnCompressed_CheckedChanged);
            // 
            // btnNotCompressed
            // 
            this.btnNotCompressed.AutoSize = true;
            this.btnNotCompressed.Checked = true;
            this.btnNotCompressed.Location = new System.Drawing.Point(6, 22);
            this.btnNotCompressed.Name = "btnNotCompressed";
            this.btnNotCompressed.Size = new System.Drawing.Size(102, 17);
            this.btnNotCompressed.TabIndex = 0;
            this.btnNotCompressed.TabStop = true;
            this.btnNotCompressed.Text = "Not compressed";
            this.btnNotCompressed.UseVisualStyleBackColor = true;
            this.btnNotCompressed.CheckedChanged += new System.EventHandler(this.btnNotCompressed_CheckedChanged);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(798, 666);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.btnRatio);
            this.Controls.Add(this.lbDownloading);
            this.Controls.Add(this.lbProgress);
            this.Controls.Add(this.comBoxDataSize);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.ckboxAutoGenerate);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtFileName);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtServerAddress);
            this.Controls.Add(this.btnGetFile);
            this.Controls.Add(this.chart1);
            this.Name = "MainForm";
            this.Text = "HTTP Client (Raw)";
            this.Load += new System.EventHandler(this.MainForm_Load_1);
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataVisualization.Charting.Chart chart1;
        private System.Windows.Forms.Button btnGetFile;
        private System.Windows.Forms.TextBox txtServerAddress;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtFileName;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RadioButton rdTransferTime;
        private System.Windows.Forms.RadioButton rdTotalTime;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.CheckBox ckboxAutoGenerate;

        private NumericTextBox txtDataSize;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox comBoxDataSize;
        private System.Windows.Forms.Label lbProgress;
        private System.Windows.Forms.Label lbDownloading;
        private System.Windows.Forms.Button btnRatio;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label lbTCount;
        private System.Windows.Forms.Label lbGCount;
        private System.Windows.Forms.Label lbCCount;
        private System.Windows.Forms.Label lbACount;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton btnCompressed;
        private System.Windows.Forms.RadioButton btnNotCompressed;
    }
}

