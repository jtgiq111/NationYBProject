namespace ybinterface_lib
{
    partial class frm_ybdzcxTS
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
            this.dtp_End = new Srvtools.InfoDateTimePicker();
            this.dtp_Start = new Srvtools.InfoDateTimePicker();
            this.label4 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.btnZEDZ = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.txtYWZQH = new System.Windows.Forms.TextBox();
            this.btnFYMXXZ = new System.Windows.Forms.Button();
            this.btnCFMXXZ = new System.Windows.Forms.Button();
            this.txtJZLSH = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.btnMXDZ = new System.Windows.Forms.Button();
            this.txtDJH = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dtp_End)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtp_Start)).BeginInit();
            this.SuspendLayout();
            // 
            // dtp_End
            // 
            this.dtp_End.CustomFormat = "yyyy-MM-dd";
            this.dtp_End.DateTimeString = null;
            this.dtp_End.DateTimeType = Srvtools.InfoDateTimePicker.dtType.DateTime;
            this.dtp_End.EditOnEnter = true;
            this.dtp_End.EnterEnable = false;
            this.dtp_End.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtp_End.Location = new System.Drawing.Point(280, 5);
            this.dtp_End.Name = "dtp_End";
            this.dtp_End.ShowCheckBox = false;
            this.dtp_End.Size = new System.Drawing.Size(120, 21);
            this.dtp_End.TabIndex = 1000015;
            this.dtp_End.Value = new System.DateTime(2016, 7, 9, 0, 0, 0, 0);
            // 
            // dtp_Start
            // 
            this.dtp_Start.CustomFormat = "yyyy-MM-dd";
            this.dtp_Start.DateTimeString = null;
            this.dtp_Start.DateTimeType = Srvtools.InfoDateTimePicker.dtType.DateTime;
            this.dtp_Start.EditOnEnter = true;
            this.dtp_Start.EnterEnable = false;
            this.dtp_Start.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtp_Start.Location = new System.Drawing.Point(87, 5);
            this.dtp_Start.Name = "dtp_Start";
            this.dtp_Start.ShowCheckBox = false;
            this.dtp_Start.Size = new System.Drawing.Size(120, 21);
            this.dtp_Start.TabIndex = 1000014;
            this.dtp_Start.Value = new System.DateTime(2016, 7, 9, 0, 0, 0, 0);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(28, 9);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(53, 12);
            this.label4.TabIndex = 1000016;
            this.label4.Text = "开始日期";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(221, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 12);
            this.label1.TabIndex = 1000017;
            this.label1.Text = "结束日期";
            // 
            // btnZEDZ
            // 
            this.btnZEDZ.Location = new System.Drawing.Point(34, 76);
            this.btnZEDZ.Name = "btnZEDZ";
            this.btnZEDZ.Size = new System.Drawing.Size(100, 23);
            this.btnZEDZ.TabIndex = 1000018;
            this.btnZEDZ.Text = "总额对帐";
            this.btnZEDZ.UseVisualStyleBackColor = true;
            this.btnZEDZ.Click += new System.EventHandler(this.btnZEDZ_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(16, 43);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 12);
            this.label2.TabIndex = 1000019;
            this.label2.Text = "业务周期号";
            // 
            // txtYWZQH
            // 
            this.txtYWZQH.Location = new System.Drawing.Point(88, 39);
            this.txtYWZQH.Name = "txtYWZQH";
            this.txtYWZQH.Size = new System.Drawing.Size(119, 21);
            this.txtYWZQH.TabIndex = 1000020;
            // 
            // btnFYMXXZ
            // 
            this.btnFYMXXZ.Location = new System.Drawing.Point(158, 75);
            this.btnFYMXXZ.Name = "btnFYMXXZ";
            this.btnFYMXXZ.Size = new System.Drawing.Size(100, 23);
            this.btnFYMXXZ.TabIndex = 1000021;
            this.btnFYMXXZ.Text = "费用明细下载";
            this.btnFYMXXZ.UseVisualStyleBackColor = true;
            this.btnFYMXXZ.Click += new System.EventHandler(this.btnFYMXXZ_Click);
            // 
            // btnCFMXXZ
            // 
            this.btnCFMXXZ.Location = new System.Drawing.Point(158, 168);
            this.btnCFMXXZ.Name = "btnCFMXXZ";
            this.btnCFMXXZ.Size = new System.Drawing.Size(100, 23);
            this.btnCFMXXZ.TabIndex = 1000029;
            this.btnCFMXXZ.Text = "处方明细下载";
            this.btnCFMXXZ.UseVisualStyleBackColor = true;
            // 
            // txtJZLSH
            // 
            this.txtJZLSH.Location = new System.Drawing.Point(110, 132);
            this.txtJZLSH.Name = "txtJZLSH";
            this.txtJZLSH.Size = new System.Drawing.Size(97, 21);
            this.txtJZLSH.TabIndex = 1000028;
            this.txtJZLSH.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtJZLSH_KeyPress);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(9, 136);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(95, 12);
            this.label3.TabIndex = 1000027;
            this.label3.Text = "门诊/住院流水号";
            // 
            // btnMXDZ
            // 
            this.btnMXDZ.Location = new System.Drawing.Point(34, 169);
            this.btnMXDZ.Name = "btnMXDZ";
            this.btnMXDZ.Size = new System.Drawing.Size(100, 23);
            this.btnMXDZ.TabIndex = 1000026;
            this.btnMXDZ.Text = "明细对帐";
            this.btnMXDZ.UseVisualStyleBackColor = true;
            this.btnMXDZ.Click += new System.EventHandler(this.btnMXDZ_Click);
            // 
            // txtDJH
            // 
            this.txtDJH.Location = new System.Drawing.Point(282, 132);
            this.txtDJH.Name = "txtDJH";
            this.txtDJH.Size = new System.Drawing.Size(118, 21);
            this.txtDJH.TabIndex = 1000031;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(211, 135);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(65, 12);
            this.label5.TabIndex = 1000030;
            this.label5.Text = "交易流水号";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(280, 76);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(120, 23);
            this.button1.TabIndex = 1000032;
            this.button1.Text = "费用明细详细信息";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(300, 169);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(100, 23);
            this.button2.TabIndex = 1000033;
            this.button2.Text = "冲正交易";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // frm_ybdzcxTS
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(536, 210);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.txtDJH);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.btnCFMXXZ);
            this.Controls.Add(this.txtJZLSH);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.btnMXDZ);
            this.Controls.Add(this.btnFYMXXZ);
            this.Controls.Add(this.txtYWZQH);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnZEDZ);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.dtp_End);
            this.Controls.Add(this.dtp_Start);
            this.Name = "frm_ybdzcxTS";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "医保对账查询";
            this.Load += new System.EventHandler(this.frm_ybdzcxTS_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dtp_End)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtp_Start)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Srvtools.InfoDateTimePicker dtp_End;
        private Srvtools.InfoDateTimePicker dtp_Start;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnZEDZ;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtYWZQH;
        private System.Windows.Forms.Button btnFYMXXZ;
        private System.Windows.Forms.Button btnCFMXXZ;
        private System.Windows.Forms.TextBox txtJZLSH;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnMXDZ;
        private System.Windows.Forms.TextBox txtDJH;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
    }
}