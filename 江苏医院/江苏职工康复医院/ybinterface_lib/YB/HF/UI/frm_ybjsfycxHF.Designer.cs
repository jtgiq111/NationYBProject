namespace ybinterface_lib
{
    partial class frm_ybjsfycxHF
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            Srvtools.TotalColumn totalColumn1 = new Srvtools.TotalColumn();
            Srvtools.TotalColumn totalColumn2 = new Srvtools.TotalColumn();
            Srvtools.TotalColumn totalColumn3 = new Srvtools.TotalColumn();
            Srvtools.TotalColumn totalColumn4 = new Srvtools.TotalColumn();
            this.idgv_YbInfo = new Srvtools.InfoDataGridView();
            this.seq = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.col_bxrq = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.col_jbrxm = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.col_jbrbh = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.col_jzlsh = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.col_xm = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.col_sbh = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.col_kb = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.col_fyhj = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.col_bxhj = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.col_zhhj = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.col_zfhj = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.col_mmbzbm1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.col_mmbzmc1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.col_qfxje = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.col_mlyfy = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.col_zycs = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.col_dwmc = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btn_JSD = new System.Windows.Forms.Button();
            this.ckQFXJE = new System.Windows.Forms.CheckBox();
            this.dtp_End = new Srvtools.InfoDateTimePicker();
            this.dtp_Start = new Srvtools.InfoDateTimePicker();
            this.txtName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cmbLB = new System.Windows.Forms.ComboBox();
            this.btn_Excel = new System.Windows.Forms.Button();
            this.label9 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.btn_Close = new System.Windows.Forms.Button();
            this.btn_Find = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.idgv_YbInfo)).BeginInit();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dtp_End)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtp_Start)).BeginInit();
            this.SuspendLayout();
            // 
            // idgv_YbInfo
            // 
            this.idgv_YbInfo.AllowUserToAddRows = false;
            this.idgv_YbInfo.AllowUserToDeleteRows = false;
            this.idgv_YbInfo.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.Black;
            this.idgv_YbInfo.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.idgv_YbInfo.ColumnHeadersHeight = 30;
            this.idgv_YbInfo.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.idgv_YbInfo.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.seq,
            this.col_bxrq,
            this.col_jbrxm,
            this.col_jbrbh,
            this.col_jzlsh,
            this.col_xm,
            this.col_sbh,
            this.col_kb,
            this.col_fyhj,
            this.col_bxhj,
            this.col_zhhj,
            this.col_zfhj,
            this.col_mmbzbm1,
            this.col_mmbzmc1,
            this.col_qfxje,
            this.col_mlyfy,
            this.col_zycs,
            this.col_dwmc});
            this.idgv_YbInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.idgv_YbInfo.EnterEnable = true;
            this.idgv_YbInfo.EnterRefValControl = false;
            this.idgv_YbInfo.Location = new System.Drawing.Point(0, 45);
            this.idgv_YbInfo.Name = "idgv_YbInfo";
            this.idgv_YbInfo.ReadOnly = true;
            this.idgv_YbInfo.RowHeadersWidth = 25;
            this.idgv_YbInfo.RowTemplate.Height = 24;
            this.idgv_YbInfo.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.idgv_YbInfo.Size = new System.Drawing.Size(1008, 454);
            this.idgv_YbInfo.SureDelete = false;
            this.idgv_YbInfo.TabIndex = 11;
            this.idgv_YbInfo.TotalActive = true;
            this.idgv_YbInfo.TotalBackColor = System.Drawing.SystemColors.Info;
            this.idgv_YbInfo.TotalCaption = "汇总";
            this.idgv_YbInfo.TotalCaptionFont = new System.Drawing.Font("宋体", 9F);
            totalColumn1.ColumnName = "col_fyhj";
            totalColumn1.ShowTotal = true;
            totalColumn1.TotalAlignment = Srvtools.TotalColumn.TotalAlign.left;
            totalColumn1.TotalMode = Srvtools.totalMode.sum;
            totalColumn2.ColumnName = "col_zfhj";
            totalColumn2.ShowTotal = true;
            totalColumn2.TotalAlignment = Srvtools.TotalColumn.TotalAlign.left;
            totalColumn2.TotalMode = Srvtools.totalMode.sum;
            totalColumn3.ColumnName = "col_bxhj";
            totalColumn3.ShowTotal = true;
            totalColumn3.TotalAlignment = Srvtools.TotalColumn.TotalAlign.left;
            totalColumn3.TotalMode = Srvtools.totalMode.sum;
            totalColumn4.ColumnName = "col_zhhj";
            totalColumn4.ShowTotal = true;
            totalColumn4.TotalAlignment = Srvtools.TotalColumn.TotalAlign.left;
            totalColumn4.TotalMode = Srvtools.totalMode.sum;
            this.idgv_YbInfo.TotalColumns.Add(totalColumn1);
            this.idgv_YbInfo.TotalColumns.Add(totalColumn2);
            this.idgv_YbInfo.TotalColumns.Add(totalColumn3);
            this.idgv_YbInfo.TotalColumns.Add(totalColumn4);
            this.idgv_YbInfo.TotalFont = new System.Drawing.Font("宋体", 9F);
            this.idgv_YbInfo.DataBindingComplete += new System.Windows.Forms.DataGridViewBindingCompleteEventHandler(this.idgv_YbInfo_DataBindingComplete);
            // 
            // seq
            // 
            this.seq.HeaderText = "序号";
            this.seq.Name = "seq";
            this.seq.ReadOnly = true;
            this.seq.Width = 50;
            // 
            // col_bxrq
            // 
            this.col_bxrq.DataPropertyName = "bxrq";
            this.col_bxrq.HeaderText = "报销日期";
            this.col_bxrq.Name = "col_bxrq";
            this.col_bxrq.ReadOnly = true;
            this.col_bxrq.Width = 130;
            // 
            // col_jbrxm
            // 
            this.col_jbrxm.DataPropertyName = "jbr";
            this.col_jbrxm.HeaderText = "经办人";
            this.col_jbrxm.Name = "col_jbrxm";
            this.col_jbrxm.ReadOnly = true;
            this.col_jbrxm.Width = 80;
            // 
            // col_jbrbh
            // 
            this.col_jbrbh.DataPropertyName = "jbrbh";
            this.col_jbrbh.HeaderText = "经办人编号";
            this.col_jbrbh.Name = "col_jbrbh";
            this.col_jbrbh.ReadOnly = true;
            this.col_jbrbh.Visible = false;
            // 
            // col_jzlsh
            // 
            this.col_jzlsh.DataPropertyName = "jzlsh";
            this.col_jzlsh.HeaderText = "就诊流水号";
            this.col_jzlsh.Name = "col_jzlsh";
            this.col_jzlsh.ReadOnly = true;
            this.col_jzlsh.Width = 80;
            // 
            // col_xm
            // 
            this.col_xm.DataPropertyName = "xm";
            this.col_xm.HeaderText = "患者姓名";
            this.col_xm.Name = "col_xm";
            this.col_xm.ReadOnly = true;
            this.col_xm.Width = 80;
            // 
            // col_sbh
            // 
            this.col_sbh.DataPropertyName = "sbh";
            this.col_sbh.HeaderText = "个人编号";
            this.col_sbh.Name = "col_sbh";
            this.col_sbh.ReadOnly = true;
            this.col_sbh.Width = 120;
            // 
            // col_kb
            // 
            this.col_kb.DataPropertyName = "kb";
            this.col_kb.HeaderText = "科别";
            this.col_kb.Name = "col_kb";
            this.col_kb.ReadOnly = true;
            // 
            // col_fyhj
            // 
            this.col_fyhj.DataPropertyName = "fyhj";
            this.col_fyhj.HeaderText = "费用合计";
            this.col_fyhj.Name = "col_fyhj";
            this.col_fyhj.ReadOnly = true;
            this.col_fyhj.Width = 80;
            // 
            // col_bxhj
            // 
            this.col_bxhj.DataPropertyName = "bxhj";
            this.col_bxhj.HeaderText = "统筹支付";
            this.col_bxhj.Name = "col_bxhj";
            this.col_bxhj.ReadOnly = true;
            this.col_bxhj.Width = 80;
            // 
            // col_zhhj
            // 
            this.col_zhhj.DataPropertyName = "zfhj";
            this.col_zhhj.HeaderText = "医保卡支付";
            this.col_zhhj.Name = "col_zhhj";
            this.col_zhhj.ReadOnly = true;
            // 
            // col_zfhj
            // 
            this.col_zfhj.DataPropertyName = "xjzf";
            this.col_zfhj.HeaderText = "现金支付";
            this.col_zfhj.Name = "col_zfhj";
            this.col_zfhj.ReadOnly = true;
            this.col_zfhj.Width = 80;
            // 
            // col_mmbzbm1
            // 
            this.col_mmbzbm1.DataPropertyName = "bzbm";
            this.col_mmbzbm1.HeaderText = "主诊断";
            this.col_mmbzbm1.Name = "col_mmbzbm1";
            this.col_mmbzbm1.ReadOnly = true;
            // 
            // col_mmbzmc1
            // 
            this.col_mmbzmc1.DataPropertyName = "bzmc";
            this.col_mmbzmc1.HeaderText = "主诊断";
            this.col_mmbzmc1.Name = "col_mmbzmc1";
            this.col_mmbzmc1.ReadOnly = true;
            this.col_mmbzmc1.Width = 150;
            // 
            // col_qfxje
            // 
            this.col_qfxje.DataPropertyName = "qfxje";
            this.col_qfxje.HeaderText = "起付线金额";
            this.col_qfxje.Name = "col_qfxje";
            this.col_qfxje.ReadOnly = true;
            this.col_qfxje.Visible = false;
            // 
            // col_mlyfy
            // 
            this.col_mlyfy.DataPropertyName = "mlyfy";
            this.col_mlyfy.HeaderText = "目录外费用";
            this.col_mlyfy.Name = "col_mlyfy";
            this.col_mlyfy.ReadOnly = true;
            this.col_mlyfy.Visible = false;
            // 
            // col_zycs
            // 
            this.col_zycs.DataPropertyName = "zycs";
            this.col_zycs.HeaderText = "累计住院次数(不含当前次数）";
            this.col_zycs.Name = "col_zycs";
            this.col_zycs.ReadOnly = true;
            this.col_zycs.Visible = false;
            this.col_zycs.Width = 150;
            // 
            // col_dwmc
            // 
            this.col_dwmc.DataPropertyName = "dwmc";
            this.col_dwmc.HeaderText = "单位/住址";
            this.col_dwmc.Name = "col_dwmc";
            this.col_dwmc.ReadOnly = true;
            this.col_dwmc.Width = 180;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btn_JSD);
            this.panel1.Controls.Add(this.ckQFXJE);
            this.panel1.Controls.Add(this.dtp_End);
            this.panel1.Controls.Add(this.dtp_Start);
            this.panel1.Controls.Add(this.txtName);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.cmbLB);
            this.panel1.Controls.Add(this.btn_Excel);
            this.panel1.Controls.Add(this.label9);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.btn_Close);
            this.panel1.Controls.Add(this.btn_Find);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1008, 45);
            this.panel1.TabIndex = 10;
            // 
            // btn_JSD
            // 
            this.btn_JSD.Location = new System.Drawing.Point(900, 6);
            this.btn_JSD.Name = "btn_JSD";
            this.btn_JSD.Size = new System.Drawing.Size(94, 28);
            this.btn_JSD.TabIndex = 1000015;
            this.btn_JSD.Text = "查看结算单";
            this.btn_JSD.UseVisualStyleBackColor = true;
            this.btn_JSD.Visible = false;
            this.btn_JSD.Click += new System.EventHandler(this.btn_JSD_Click);
            // 
            // ckQFXJE
            // 
            this.ckQFXJE.AutoSize = true;
            this.ckQFXJE.ForeColor = System.Drawing.Color.Red;
            this.ckQFXJE.Location = new System.Drawing.Point(452, 14);
            this.ckQFXJE.Name = "ckQFXJE";
            this.ckQFXJE.Size = new System.Drawing.Size(84, 16);
            this.ckQFXJE.TabIndex = 1000014;
            this.ckQFXJE.Text = "诊查费统筹";
            this.ckQFXJE.UseVisualStyleBackColor = true;
            // 
            // dtp_End
            // 
            this.dtp_End.CustomFormat = "yyyy-MM-dd";
            this.dtp_End.DateTimeString = null;
            this.dtp_End.DateTimeType = Srvtools.InfoDateTimePicker.dtType.DateTime;
            this.dtp_End.EditOnEnter = true;
            this.dtp_End.EnterEnable = false;
            this.dtp_End.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtp_End.Location = new System.Drawing.Point(211, 11);
            this.dtp_End.Name = "dtp_End";
            this.dtp_End.ShowCheckBox = false;
            this.dtp_End.Size = new System.Drawing.Size(120, 21);
            this.dtp_End.TabIndex = 1000013;
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
            this.dtp_Start.Location = new System.Drawing.Point(60, 11);
            this.dtp_Start.Name = "dtp_Start";
            this.dtp_Start.ShowCheckBox = false;
            this.dtp_Start.Size = new System.Drawing.Size(120, 21);
            this.dtp_Start.TabIndex = 1000012;
            this.dtp_Start.Value = new System.DateTime(2016, 7, 9, 0, 0, 0, 0);
            // 
            // txtName
            // 
            this.txtName.Location = new System.Drawing.Point(383, 11);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(64, 21);
            this.txtName.TabIndex = 1000011;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(348, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 12);
            this.label1.TabIndex = 1000010;
            this.label1.Text = "姓名";
            // 
            // cmbLB
            // 
            this.cmbLB.AutoCompleteCustomSource.AddRange(new string[] {
            "住院",
            "门诊"});
            this.cmbLB.FormattingEnabled = true;
            this.cmbLB.Items.AddRange(new object[] {
            "住院",
            "门诊"});
            this.cmbLB.Location = new System.Drawing.Point(546, 11);
            this.cmbLB.Name = "cmbLB";
            this.cmbLB.Size = new System.Drawing.Size(64, 20);
            this.cmbLB.TabIndex = 1000009;
            this.cmbLB.Text = "住院";
            this.cmbLB.SelectedIndexChanged += new System.EventHandler(this.cmbLB_SelectedIndexChanged);
            // 
            // btn_Excel
            // 
            this.btn_Excel.Location = new System.Drawing.Point(742, 7);
            this.btn_Excel.Name = "btn_Excel";
            this.btn_Excel.Size = new System.Drawing.Size(73, 28);
            this.btn_Excel.TabIndex = 1000008;
            this.btn_Excel.Text = "导出Excel";
            this.btn_Excel.UseVisualStyleBackColor = true;
            this.btn_Excel.Click += new System.EventHandler(this.btn_Excel_Click);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("宋体", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label9.Location = new System.Drawing.Point(186, 15);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(20, 13);
            this.label9.TabIndex = 1000007;
            this.label9.Text = "至";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(4, 15);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(53, 12);
            this.label4.TabIndex = 18;
            this.label4.Text = "结算日期";
            // 
            // btn_Close
            // 
            this.btn_Close.Location = new System.Drawing.Point(821, 7);
            this.btn_Close.Name = "btn_Close";
            this.btn_Close.Size = new System.Drawing.Size(73, 28);
            this.btn_Close.TabIndex = 16;
            this.btn_Close.Text = "&Q退出";
            this.btn_Close.UseVisualStyleBackColor = true;
            this.btn_Close.Click += new System.EventHandler(this.btn_Close_Click);
            // 
            // btn_Find
            // 
            this.btn_Find.Location = new System.Drawing.Point(661, 7);
            this.btn_Find.Name = "btn_Find";
            this.btn_Find.Size = new System.Drawing.Size(73, 28);
            this.btn_Find.TabIndex = 13;
            this.btn_Find.Text = "查询";
            this.btn_Find.UseVisualStyleBackColor = true;
            this.btn_Find.Click += new System.EventHandler(this.btn_Find_Click);
            // 
            // frm_ybjsfycxSR
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1008, 499);
            this.Controls.Add(this.idgv_YbInfo);
            this.Controls.Add(this.panel1);
            this.Name = "frm_ybjsfycxSR";
            this.Text = "医保结算报销查询";
            this.Load += new System.EventHandler(this.frm_ybjsfycx_Load);
            ((System.ComponentModel.ISupportInitialize)(this.idgv_YbInfo)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dtp_End)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtp_Start)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Srvtools.InfoDataGridView idgv_YbInfo;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btn_JSD;
        private System.Windows.Forms.CheckBox ckQFXJE;
        private Srvtools.InfoDateTimePicker dtp_End;
        private Srvtools.InfoDateTimePicker dtp_Start;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmbLB;
        private System.Windows.Forms.Button btn_Excel;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btn_Close;
        private System.Windows.Forms.Button btn_Find;
        private System.Windows.Forms.DataGridViewTextBoxColumn seq;
        private System.Windows.Forms.DataGridViewTextBoxColumn col_bxrq;
        private System.Windows.Forms.DataGridViewTextBoxColumn col_jbrxm;
        private System.Windows.Forms.DataGridViewTextBoxColumn col_jbrbh;
        private System.Windows.Forms.DataGridViewTextBoxColumn col_jzlsh;
        private System.Windows.Forms.DataGridViewTextBoxColumn col_xm;
        private System.Windows.Forms.DataGridViewTextBoxColumn col_sbh;
        private System.Windows.Forms.DataGridViewTextBoxColumn col_kb;
        private System.Windows.Forms.DataGridViewTextBoxColumn col_fyhj;
        private System.Windows.Forms.DataGridViewTextBoxColumn col_bxhj;
        private System.Windows.Forms.DataGridViewTextBoxColumn col_zhhj;
        private System.Windows.Forms.DataGridViewTextBoxColumn col_zfhj;
        private System.Windows.Forms.DataGridViewTextBoxColumn col_mmbzbm1;
        private System.Windows.Forms.DataGridViewTextBoxColumn col_mmbzmc1;
        private System.Windows.Forms.DataGridViewTextBoxColumn col_qfxje;
        private System.Windows.Forms.DataGridViewTextBoxColumn col_mlyfy;
        private System.Windows.Forms.DataGridViewTextBoxColumn col_zycs;
        private System.Windows.Forms.DataGridViewTextBoxColumn col_dwmc;
    }
}