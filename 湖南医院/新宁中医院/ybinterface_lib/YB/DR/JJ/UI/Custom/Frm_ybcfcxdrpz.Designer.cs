namespace ybinterface_lib
{
    partial class Frm_ybcfcxdrpz
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
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.btn_Close = new System.Windows.Forms.Button();
            this.btn_qk = new System.Windows.Forms.Button();
            this.btprint = new System.Windows.Forms.Button();
            this.lbfphx = new System.Windows.Forms.Label();
            this.txtfphx = new Srvtools.InfoTextBox();
            this.btquty = new System.Windows.Forms.Button();
            this.lbagex = new System.Windows.Forms.Label();
            this.txtagex = new Srvtools.InfoTextBox();
            this.lbsex = new System.Windows.Forms.Label();
            this.txtsex = new Srvtools.InfoTextBox();
            this.lbname = new System.Windows.Forms.Label();
            this.txtname = new Srvtools.InfoTextBox();
            this.lbmzno = new System.Windows.Forms.Label();
            this.txtmzno = new Srvtools.InfoTextBox();
            this.lbghno = new System.Windows.Forms.Label();
            this.txtghno = new Srvtools.InfoTextBox();
            this.infoDataGridView1 = new Srvtools.InfoDataGridView();
            this.m3invo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.m3cfno = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.yyxmmc = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dj = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.sl = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.je = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.m3mzno = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.m3ghno = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.m1name = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.m1sexx = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.m1agex = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.infoDataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.btn_Close);
            this.splitContainer1.Panel1.Controls.Add(this.btn_qk);
            this.splitContainer1.Panel1.Controls.Add(this.btprint);
            this.splitContainer1.Panel1.Controls.Add(this.lbfphx);
            this.splitContainer1.Panel1.Controls.Add(this.txtfphx);
            this.splitContainer1.Panel1.Controls.Add(this.btquty);
            this.splitContainer1.Panel1.Controls.Add(this.lbagex);
            this.splitContainer1.Panel1.Controls.Add(this.txtagex);
            this.splitContainer1.Panel1.Controls.Add(this.lbsex);
            this.splitContainer1.Panel1.Controls.Add(this.txtsex);
            this.splitContainer1.Panel1.Controls.Add(this.lbname);
            this.splitContainer1.Panel1.Controls.Add(this.txtname);
            this.splitContainer1.Panel1.Controls.Add(this.lbmzno);
            this.splitContainer1.Panel1.Controls.Add(this.txtmzno);
            this.splitContainer1.Panel1.Controls.Add(this.lbghno);
            this.splitContainer1.Panel1.Controls.Add(this.txtghno);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.infoDataGridView1);
            this.splitContainer1.Size = new System.Drawing.Size(724, 612);
            this.splitContainer1.SplitterDistance = 77;
            this.splitContainer1.SplitterWidth = 5;
            this.splitContainer1.TabIndex = 0;
            // 
            // btn_Close
            // 
            this.btn_Close.Location = new System.Drawing.Point(636, 42);
            this.btn_Close.Name = "btn_Close";
            this.btn_Close.Size = new System.Drawing.Size(83, 31);
            this.btn_Close.TabIndex = 199;
            this.btn_Close.Text = "关闭";
            this.btn_Close.UseVisualStyleBackColor = true;
            this.btn_Close.Click += new System.EventHandler(this.btn_Close_Click);
            // 
            // btn_qk
            // 
            this.btn_qk.Location = new System.Drawing.Point(429, 40);
            this.btn_qk.Name = "btn_qk";
            this.btn_qk.Size = new System.Drawing.Size(80, 33);
            this.btn_qk.TabIndex = 14;
            this.btn_qk.Text = "清空";
            this.btn_qk.UseVisualStyleBackColor = true;
            this.btn_qk.Click += new System.EventHandler(this.btn_qk_Click);
            // 
            // btprint
            // 
            this.btprint.Location = new System.Drawing.Point(531, 40);
            this.btprint.Name = "btprint";
            this.btprint.Size = new System.Drawing.Size(80, 33);
            this.btprint.TabIndex = 13;
            this.btprint.Text = "打印";
            this.btprint.UseVisualStyleBackColor = true;
            this.btprint.Click += new System.EventHandler(this.btprint_Click);
            // 
            // lbfphx
            // 
            this.lbfphx.AutoSize = true;
            this.lbfphx.Location = new System.Drawing.Point(8, 17);
            this.lbfphx.Name = "lbfphx";
            this.lbfphx.Size = new System.Drawing.Size(49, 14);
            this.lbfphx.TabIndex = 12;
            this.lbfphx.Text = "发票号";
            // 
            // txtfphx
            // 
            this.txtfphx.EnterEnable = true;
            this.txtfphx.LeaveText = null;
            this.txtfphx.Location = new System.Drawing.Point(63, 12);
            this.txtfphx.MaxLength = 21;
            this.txtfphx.Name = "txtfphx";
            this.txtfphx.RefVal = null;
            this.txtfphx.Size = new System.Drawing.Size(160, 23);
            this.txtfphx.TabIndex = 1;
            this.txtfphx.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtfphx_KeyDown);
            // 
            // btquty
            // 
            this.btquty.Location = new System.Drawing.Point(639, 6);
            this.btquty.Name = "btquty";
            this.btquty.Size = new System.Drawing.Size(80, 33);
            this.btquty.TabIndex = 10;
            this.btquty.Text = "查询";
            this.btquty.UseVisualStyleBackColor = true;
            this.btquty.Click += new System.EventHandler(this.btquty_Click);
            // 
            // lbagex
            // 
            this.lbagex.AutoSize = true;
            this.lbagex.Location = new System.Drawing.Point(278, 50);
            this.lbagex.Name = "lbagex";
            this.lbagex.Size = new System.Drawing.Size(35, 14);
            this.lbagex.TabIndex = 9;
            this.lbagex.Text = "年龄";
            // 
            // txtagex
            // 
            this.txtagex.Enabled = false;
            this.txtagex.EnterEnable = true;
            this.txtagex.LeaveText = null;
            this.txtagex.Location = new System.Drawing.Point(317, 45);
            this.txtagex.MaxLength = 3;
            this.txtagex.Name = "txtagex";
            this.txtagex.RefVal = null;
            this.txtagex.Size = new System.Drawing.Size(56, 23);
            this.txtagex.TabIndex = 8;
            // 
            // lbsex
            // 
            this.lbsex.AutoSize = true;
            this.lbsex.Location = new System.Drawing.Point(167, 50);
            this.lbsex.Name = "lbsex";
            this.lbsex.Size = new System.Drawing.Size(35, 14);
            this.lbsex.TabIndex = 7;
            this.lbsex.Text = "性别";
            // 
            // txtsex
            // 
            this.txtsex.Enabled = false;
            this.txtsex.EnterEnable = true;
            this.txtsex.LeaveText = null;
            this.txtsex.Location = new System.Drawing.Point(206, 45);
            this.txtsex.MaxLength = 1;
            this.txtsex.Name = "txtsex";
            this.txtsex.RefVal = null;
            this.txtsex.Size = new System.Drawing.Size(56, 23);
            this.txtsex.TabIndex = 6;
            // 
            // lbname
            // 
            this.lbname.AutoSize = true;
            this.lbname.Location = new System.Drawing.Point(11, 48);
            this.lbname.Name = "lbname";
            this.lbname.Size = new System.Drawing.Size(35, 14);
            this.lbname.TabIndex = 5;
            this.lbname.Text = "姓名";
            // 
            // txtname
            // 
            this.txtname.Enabled = false;
            this.txtname.EnterEnable = true;
            this.txtname.LeaveText = null;
            this.txtname.Location = new System.Drawing.Point(50, 43);
            this.txtname.MaxLength = 10;
            this.txtname.Name = "txtname";
            this.txtname.RefVal = null;
            this.txtname.Size = new System.Drawing.Size(108, 23);
            this.txtname.TabIndex = 4;
            // 
            // lbmzno
            // 
            this.lbmzno.AutoSize = true;
            this.lbmzno.Location = new System.Drawing.Point(229, 19);
            this.lbmzno.Name = "lbmzno";
            this.lbmzno.Size = new System.Drawing.Size(49, 14);
            this.lbmzno.TabIndex = 3;
            this.lbmzno.Text = "门诊号";
            // 
            // txtmzno
            // 
            this.txtmzno.Enabled = false;
            this.txtmzno.EnterEnable = true;
            this.txtmzno.LeaveText = null;
            this.txtmzno.Location = new System.Drawing.Point(279, 14);
            this.txtmzno.MaxLength = 21;
            this.txtmzno.Name = "txtmzno";
            this.txtmzno.RefVal = null;
            this.txtmzno.Size = new System.Drawing.Size(118, 23);
            this.txtmzno.TabIndex = 2;
            // 
            // lbghno
            // 
            this.lbghno.AutoSize = true;
            this.lbghno.Location = new System.Drawing.Point(407, 19);
            this.lbghno.Name = "lbghno";
            this.lbghno.Size = new System.Drawing.Size(77, 14);
            this.lbghno.TabIndex = 1;
            this.lbghno.Text = "挂号流水号";
            // 
            // txtghno
            // 
            this.txtghno.Enabled = false;
            this.txtghno.EnterEnable = true;
            this.txtghno.LeaveText = null;
            this.txtghno.Location = new System.Drawing.Point(490, 14);
            this.txtghno.MaxLength = 10;
            this.txtghno.Name = "txtghno";
            this.txtghno.RefVal = null;
            this.txtghno.Size = new System.Drawing.Size(146, 23);
            this.txtghno.TabIndex = 3;
            this.txtghno.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtghno_KeyDown);
            // 
            // infoDataGridView1
            // 
            this.infoDataGridView1.AllowUserToAddRows = false;
            this.infoDataGridView1.AllowUserToDeleteRows = false;
            this.infoDataGridView1.ColumnHeadersHeight = 30;
            this.infoDataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.m3invo,
            this.m3cfno,
            this.yyxmmc,
            this.dj,
            this.sl,
            this.je,
            this.m3mzno,
            this.m3ghno,
            this.m1name,
            this.m1sexx,
            this.m1agex});
            this.infoDataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.infoDataGridView1.EnterEnable = true;
            this.infoDataGridView1.EnterRefValControl = false;
            this.infoDataGridView1.Location = new System.Drawing.Point(0, 0);
            this.infoDataGridView1.Name = "infoDataGridView1";
            this.infoDataGridView1.ReadOnly = true;
            this.infoDataGridView1.RowHeadersWidth = 15;
            this.infoDataGridView1.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.infoDataGridView1.RowTemplate.Height = 23;
            this.infoDataGridView1.Size = new System.Drawing.Size(724, 530);
            this.infoDataGridView1.SureDelete = false;
            this.infoDataGridView1.TabIndex = 0;
            this.infoDataGridView1.TotalActive = true;
            this.infoDataGridView1.TotalBackColor = System.Drawing.SystemColors.Info;
            this.infoDataGridView1.TotalCaption = null;
            this.infoDataGridView1.TotalCaptionFont = new System.Drawing.Font("宋体", 9F);
            this.infoDataGridView1.TotalFont = new System.Drawing.Font("宋体", 9F);
            // 
            // m3invo
            // 
            this.m3invo.DataPropertyName = "m3invo";
            this.m3invo.HeaderText = "发票号";
            this.m3invo.Name = "m3invo";
            this.m3invo.ReadOnly = true;
            // 
            // m3cfno
            // 
            this.m3cfno.DataPropertyName = "m3cfno";
            this.m3cfno.HeaderText = "处方号";
            this.m3cfno.Name = "m3cfno";
            this.m3cfno.ReadOnly = true;
            // 
            // yyxmmc
            // 
            this.yyxmmc.DataPropertyName = "yyxmmc";
            this.yyxmmc.HeaderText = "名称";
            this.yyxmmc.Name = "yyxmmc";
            this.yyxmmc.ReadOnly = true;
            // 
            // dj
            // 
            this.dj.DataPropertyName = "dj";
            this.dj.HeaderText = "单价";
            this.dj.Name = "dj";
            this.dj.ReadOnly = true;
            // 
            // sl
            // 
            this.sl.DataPropertyName = "sl";
            this.sl.HeaderText = "数量";
            this.sl.Name = "sl";
            this.sl.ReadOnly = true;
            // 
            // je
            // 
            this.je.DataPropertyName = "je";
            this.je.HeaderText = "金额";
            this.je.Name = "je";
            this.je.ReadOnly = true;
            // 
            // m3mzno
            // 
            this.m3mzno.DataPropertyName = "m3mzno";
            this.m3mzno.HeaderText = "住院号";
            this.m3mzno.Name = "m3mzno";
            this.m3mzno.ReadOnly = true;
            // 
            // m3ghno
            // 
            this.m3ghno.DataPropertyName = "m3ghno";
            this.m3ghno.HeaderText = "流水号";
            this.m3ghno.Name = "m3ghno";
            this.m3ghno.ReadOnly = true;
            // 
            // m1name
            // 
            this.m1name.DataPropertyName = "m1name";
            this.m1name.HeaderText = "姓名";
            this.m1name.Name = "m1name";
            this.m1name.ReadOnly = true;
            // 
            // m1sexx
            // 
            this.m1sexx.DataPropertyName = "m1sexx";
            this.m1sexx.HeaderText = "性别";
            this.m1sexx.Name = "m1sexx";
            this.m1sexx.ReadOnly = true;
            // 
            // m1agex
            // 
            this.m1agex.DataPropertyName = "m1agex";
            this.m1agex.HeaderText = "年纪";
            this.m1agex.Name = "m1agex";
            this.m1agex.ReadOnly = true;
            // 
            // Frm_ybcfcxdrpz
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.ClientSize = new System.Drawing.Size(724, 612);
            this.Controls.Add(this.splitContainer1);
            this.Font = new System.Drawing.Font("宋体", 10.5F);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Frm_ybcfcxdrpz";
            this.Text = "医保处方查询(东软)";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.infoDataGridView1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Button btquty;
        private System.Windows.Forms.Label lbagex;
        private Srvtools.InfoTextBox txtagex;
        private System.Windows.Forms.Label lbsex;
        private Srvtools.InfoTextBox txtsex;
        private System.Windows.Forms.Label lbname;
        private Srvtools.InfoTextBox txtname;
        private System.Windows.Forms.Label lbmzno;
        private Srvtools.InfoTextBox txtmzno;
        private System.Windows.Forms.Label lbghno;
        private Srvtools.InfoTextBox txtghno;
        private System.Windows.Forms.Button btprint;
        private System.Windows.Forms.Label lbfphx;
        private Srvtools.InfoTextBox txtfphx;
        private Srvtools.InfoDataGridView infoDataGridView1;
        private System.Windows.Forms.Button btn_qk;
        private System.Windows.Forms.DataGridViewTextBoxColumn m3invo;
        private System.Windows.Forms.DataGridViewTextBoxColumn m3cfno;
        private System.Windows.Forms.DataGridViewTextBoxColumn yyxmmc;
        private System.Windows.Forms.DataGridViewTextBoxColumn dj;
        private System.Windows.Forms.DataGridViewTextBoxColumn sl;
        private System.Windows.Forms.DataGridViewTextBoxColumn je;
        private System.Windows.Forms.DataGridViewTextBoxColumn m3mzno;
        private System.Windows.Forms.DataGridViewTextBoxColumn m3ghno;
        private System.Windows.Forms.DataGridViewTextBoxColumn m1name;
        private System.Windows.Forms.DataGridViewTextBoxColumn m1sexx;
        private System.Windows.Forms.DataGridViewTextBoxColumn m1agex;
        private System.Windows.Forms.Button btn_Close;

    }
}
