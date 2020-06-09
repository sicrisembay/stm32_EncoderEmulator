namespace encoderEmuTool {
    partial class ui {
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
            this.gbUsbComm = new System.Windows.Forms.GroupBox();
            this.lblSerialNumber = new System.Windows.Forms.Label();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.cboDongleSN = new System.Windows.Forms.ComboBox();
            this.tbxRPM = new System.Windows.Forms.TextBox();
            this.btnUpdate = new System.Windows.Forms.Button();
            this.btnConnect = new System.Windows.Forms.Button();
            this.gbSpeed = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.tbxCPR = new System.Windows.Forms.TextBox();
            this.gbSineTest = new System.Windows.Forms.GroupBox();
            this.btnTestStart = new System.Windows.Forms.Button();
            this.label8 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.tbxUpdateInterval = new System.Windows.Forms.TextBox();
            this.tbxFreq = new System.Windows.Forms.TextBox();
            this.tbxAmplitude = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.tmrUpdate = new System.Windows.Forms.Timer(this.components);
            this.gbUsbComm.SuspendLayout();
            this.gbSpeed.SuspendLayout();
            this.gbSineTest.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbUsbComm
            // 
            this.gbUsbComm.Controls.Add(this.lblSerialNumber);
            this.gbUsbComm.Controls.Add(this.btnRefresh);
            this.gbUsbComm.Controls.Add(this.cboDongleSN);
            this.gbUsbComm.Location = new System.Drawing.Point(12, 12);
            this.gbUsbComm.Name = "gbUsbComm";
            this.gbUsbComm.Size = new System.Drawing.Size(258, 68);
            this.gbUsbComm.TabIndex = 5;
            this.gbUsbComm.TabStop = false;
            this.gbUsbComm.Text = "USB Dongle";
            // 
            // lblSerialNumber
            // 
            this.lblSerialNumber.AutoSize = true;
            this.lblSerialNumber.Location = new System.Drawing.Point(18, 30);
            this.lblSerialNumber.Name = "lblSerialNumber";
            this.lblSerialNumber.Size = new System.Drawing.Size(25, 13);
            this.lblSerialNumber.TabIndex = 3;
            this.lblSerialNumber.Text = "SN:";
            // 
            // btnRefresh
            // 
            this.btnRefresh.Location = new System.Drawing.Point(176, 27);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(73, 21);
            this.btnRefresh.TabIndex = 1;
            this.btnRefresh.Text = "Refresh";
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // cboDongleSN
            // 
            this.cboDongleSN.FormattingEnabled = true;
            this.cboDongleSN.Location = new System.Drawing.Point(49, 27);
            this.cboDongleSN.Name = "cboDongleSN";
            this.cboDongleSN.Size = new System.Drawing.Size(121, 21);
            this.cboDongleSN.TabIndex = 2;
            // 
            // tbxRPM
            // 
            this.tbxRPM.Location = new System.Drawing.Point(70, 31);
            this.tbxRPM.Name = "tbxRPM";
            this.tbxRPM.Size = new System.Drawing.Size(64, 20);
            this.tbxRPM.TabIndex = 6;
            this.tbxRPM.Text = "1000";
            // 
            // btnUpdate
            // 
            this.btnUpdate.Location = new System.Drawing.Point(49, 94);
            this.btnUpdate.Name = "btnUpdate";
            this.btnUpdate.Size = new System.Drawing.Size(142, 50);
            this.btnUpdate.TabIndex = 7;
            this.btnUpdate.Text = "Update";
            this.btnUpdate.UseVisualStyleBackColor = true;
            this.btnUpdate.Click += new System.EventHandler(this.btnUpdate_Click);
            // 
            // btnConnect
            // 
            this.btnConnect.Location = new System.Drawing.Point(12, 86);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(258, 38);
            this.btnConnect.TabIndex = 8;
            this.btnConnect.Text = "Connect";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // gbSpeed
            // 
            this.gbSpeed.Controls.Add(this.label2);
            this.gbSpeed.Controls.Add(this.label3);
            this.gbSpeed.Controls.Add(this.label1);
            this.gbSpeed.Controls.Add(this.tbxCPR);
            this.gbSpeed.Controls.Add(this.tbxRPM);
            this.gbSpeed.Controls.Add(this.btnUpdate);
            this.gbSpeed.Enabled = false;
            this.gbSpeed.Location = new System.Drawing.Point(12, 146);
            this.gbSpeed.Name = "gbSpeed";
            this.gbSpeed.Size = new System.Drawing.Size(258, 159);
            this.gbSpeed.TabIndex = 9;
            this.gbSpeed.TabStop = false;
            this.gbSpeed.Text = "Manual Speed Configuration";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(140, 34);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(31, 13);
            this.label2.TabIndex = 9;
            this.label2.Text = "RPM";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(35, 60);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(29, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "CPR";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(26, 34);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(38, 13);
            this.label1.TabIndex = 8;
            this.label1.Text = "Speed";
            // 
            // tbxCPR
            // 
            this.tbxCPR.Location = new System.Drawing.Point(70, 57);
            this.tbxCPR.Name = "tbxCPR";
            this.tbxCPR.Size = new System.Drawing.Size(64, 20);
            this.tbxCPR.TabIndex = 6;
            this.tbxCPR.Text = "100";
            // 
            // gbSineTest
            // 
            this.gbSineTest.Controls.Add(this.btnTestStart);
            this.gbSineTest.Controls.Add(this.label8);
            this.gbSineTest.Controls.Add(this.label6);
            this.gbSineTest.Controls.Add(this.label5);
            this.gbSineTest.Controls.Add(this.tbxUpdateInterval);
            this.gbSineTest.Controls.Add(this.tbxFreq);
            this.gbSineTest.Controls.Add(this.tbxAmplitude);
            this.gbSineTest.Controls.Add(this.label9);
            this.gbSineTest.Controls.Add(this.label7);
            this.gbSineTest.Controls.Add(this.label4);
            this.gbSineTest.Enabled = false;
            this.gbSineTest.Location = new System.Drawing.Point(12, 311);
            this.gbSineTest.Name = "gbSineTest";
            this.gbSineTest.Size = new System.Drawing.Size(257, 166);
            this.gbSineTest.TabIndex = 10;
            this.gbSineTest.TabStop = false;
            this.gbSineTest.Text = "Sinusoidal Speed Test";
            // 
            // btnTestStart
            // 
            this.btnTestStart.Location = new System.Drawing.Point(46, 114);
            this.btnTestStart.Name = "btnTestStart";
            this.btnTestStart.Size = new System.Drawing.Size(147, 35);
            this.btnTestStart.TabIndex = 10;
            this.btnTestStart.Text = "Start";
            this.btnTestStart.UseVisualStyleBackColor = true;
            this.btnTestStart.Click += new System.EventHandler(this.btnTestStart_Click);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(163, 83);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(20, 13);
            this.label8.TabIndex = 9;
            this.label8.Text = "ms";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(163, 57);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(28, 13);
            this.label6.TabIndex = 9;
            this.label6.Text = "mHz";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(163, 31);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(31, 13);
            this.label5.TabIndex = 9;
            this.label5.Text = "RPM";
            // 
            // tbxUpdateInterval
            // 
            this.tbxUpdateInterval.Location = new System.Drawing.Point(93, 80);
            this.tbxUpdateInterval.Name = "tbxUpdateInterval";
            this.tbxUpdateInterval.Size = new System.Drawing.Size(64, 20);
            this.tbxUpdateInterval.TabIndex = 6;
            this.tbxUpdateInterval.Text = "10";
            // 
            // tbxFreq
            // 
            this.tbxFreq.Location = new System.Drawing.Point(93, 54);
            this.tbxFreq.Name = "tbxFreq";
            this.tbxFreq.Size = new System.Drawing.Size(64, 20);
            this.tbxFreq.TabIndex = 6;
            this.tbxFreq.Text = "100";
            // 
            // tbxAmplitude
            // 
            this.tbxAmplitude.Location = new System.Drawing.Point(93, 28);
            this.tbxAmplitude.Name = "tbxAmplitude";
            this.tbxAmplitude.Size = new System.Drawing.Size(64, 20);
            this.tbxAmplitude.TabIndex = 6;
            this.tbxAmplitude.Text = "8000";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(4, 83);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(83, 13);
            this.label9.TabIndex = 8;
            this.label9.Text = "Update Interval:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(27, 57);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(60, 13);
            this.label7.TabIndex = 8;
            this.label7.Text = "Frequency:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(31, 31);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(56, 13);
            this.label4.TabIndex = 8;
            this.label4.Text = "Amplitude:";
            // 
            // tmrUpdate
            // 
            this.tmrUpdate.Interval = 10;
            this.tmrUpdate.Tick += new System.EventHandler(this.tmrUpdate_Tick);
            // 
            // ui
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(285, 489);
            this.Controls.Add(this.gbSineTest);
            this.Controls.Add(this.gbSpeed);
            this.Controls.Add(this.btnConnect);
            this.Controls.Add(this.gbUsbComm);
            this.Name = "ui";
            this.Text = "Encoder Emulator";
            this.gbUsbComm.ResumeLayout(false);
            this.gbUsbComm.PerformLayout();
            this.gbSpeed.ResumeLayout(false);
            this.gbSpeed.PerformLayout();
            this.gbSineTest.ResumeLayout(false);
            this.gbSineTest.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox gbUsbComm;
        private System.Windows.Forms.Label lblSerialNumber;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.ComboBox cboDongleSN;
        private System.Windows.Forms.TextBox tbxRPM;
        private System.Windows.Forms.Button btnUpdate;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.GroupBox gbSpeed;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbxCPR;
        private System.Windows.Forms.GroupBox gbSineTest;
        private System.Windows.Forms.Button btnTestStart;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox tbxUpdateInterval;
        private System.Windows.Forms.TextBox tbxFreq;
        private System.Windows.Forms.TextBox tbxAmplitude;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Timer tmrUpdate;
    }
}

