namespace ybinterface_lib
{
    partial class Frm_ybmfxtcxdrjj
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dateTimeEnd = new System.Windows.Forms.DateTimePicker();
            this.dateTimeStar = new System.Windows.Forms.DateTimePicker();
            this.label4 = new System.Windows.Forms.Label();
            this.btncx = new System.Windows.Forms.Button();
            this.dgvbf = new System.Windows.Forms.DataGridView();
            this.Column18 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column11 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column28 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column12 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column19 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column7 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column8 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column9 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column10 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column13 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column14 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column15 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column16 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column17 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnDC = new System.Windows.Forms.Button();
            this.btn_Close = new System.Windows.Forms.Button();
            this.cmb_tj = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txt_nr = new System.Windows.Forms.TextBox();
            this.cmb_tcq = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.lbl_rc = new System.Windows.Forms.Label();
            this.btn_dccvs = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvbf)).BeginInit();
            this.SuspendLayout();
            // 
            // dateTimeEnd
            // 
            this.dateTimeEnd.Location = new System.Drawing.Point(150, 16);
            this.dateTimeEnd.Name = "dateTimeEnd";
            this.dateTimeEnd.Size = new System.Drawing.Size(109, 21);
            this.dateTimeEnd.TabIndex = 200;
            // 
            // dateTimeStar
            // 
            this.dateTimeStar.Location = new System.Drawing.Point(12, 17);
            this.dateTimeStar.Name = "dateTimeStar";
            this.dateTimeStar.Size = new System.Drawing.Size(109, 21);
            this.dateTimeStar.TabIndex = 199;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(127, 20);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(17, 12);
            this.label4.TabIndex = 198;
            this.label4.Text = "至";
            // 
            // btncx
            // 
            this.btncx.Location = new System.Drawing.Point(695, 14);
            this.btncx.Name = "btncx";
            this.btncx.Size = new System.Drawing.Size(75, 31);
            this.btncx.TabIndex = 215;
            this.btncx.Text = "查询";
            this.btncx.UseVisualStyleBackColor = true;
            this.btncx.Click += new System.EventHandler(this.btncx_Click);
            // 
            // dgvbf
            // 
            this.dgvbf.AllowUserToAddRows = false;
            this.dgvbf.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvbf.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column18,
            this.Column11,
            this.Column28,
            this.Column12,
            this.Column19,
            this.Column7,
            this.Column8,
            this.Column4,
            this.Column2,
            this.Column9,
            this.Column10,
            this.Column3,
            this.Column5,
            this.Column13,
            this.Column14,
            this.Column15,
            this.Column16,
            this.Column17});
            this.dgvbf.Location = new System.Drawing.Point(12, 64);
            this.dgvbf.Name = "dgvbf";
            this.dgvbf.RowTemplate.Height = 23;
            this.dgvbf.Size = new System.Drawing.Size(1066, 325);
            this.dgvbf.TabIndex = 217;
            // 
            // Column18
            // 
            this.Column18.DataPropertyName = "tcq";
            this.Column18.HeaderText = "统筹区";
            this.Column18.Name = "Column18";
            this.Column18.ReadOnly = true;
            // 
            // Column11
            // 
            this.Column11.DataPropertyName = "lsh";
            this.Column11.HeaderText = "流水号";
            this.Column11.Name = "Column11";
            this.Column11.ReadOnly = true;
            // 
            // Column28
            // 
            this.Column28.DataPropertyName = "fph";
            this.Column28.HeaderText = "发票号";
            this.Column28.Name = "Column28";
            this.Column28.ReadOnly = true;
            // 
            // Column12
            // 
            this.Column12.DataPropertyName = "xm";
            this.Column12.HeaderText = "姓名";
            this.Column12.Name = "Column12";
            this.Column12.ReadOnly = true;
            this.Column12.Width = 80;
            // 
            // Column19
            // 
            this.Column19.DataPropertyName = "sfzh";
            this.Column19.HeaderText = "身份证号";
            this.Column19.Name = "Column19";
            this.Column19.ReadOnly = true;
            this.Column19.Width = 120;
            // 
            // Column7
            // 
            this.Column7.DataPropertyName = "mbmc";
            this.Column7.HeaderText = "模板名称";
            this.Column7.Name = "Column7";
            this.Column7.ReadOnly = true;
            // 
            // Column8
            // 
            this.Column8.DataPropertyName = "dj";
            this.Column8.HeaderText = "单价";
            this.Column8.Name = "Column8";
            this.Column8.ReadOnly = true;
            this.Column8.Width = 80;
            // 
            // Column4
            // 
            this.Column4.DataPropertyName = "sl";
            this.Column4.HeaderText = "数量";
            this.Column4.Name = "Column4";
            this.Column4.ReadOnly = true;
            this.Column4.Width = 80;
            // 
            // Column2
            // 
            this.Column2.DataPropertyName = "je";
            dataGridViewCellStyle2.Format = "N2";
            dataGridViewCellStyle2.NullValue = null;
            this.Column2.DefaultCellStyle = dataGridViewCellStyle2;
            this.Column2.HeaderText = "金额";
            this.Column2.Name = "Column2";
            this.Column2.ReadOnly = true;
            // 
            // Column9
            // 
            this.Column9.DataPropertyName = "kdys";
            this.Column9.HeaderText = "开单医生";
            this.Column9.Name = "Column9";
            this.Column9.ReadOnly = true;
            this.Column9.Width = 80;
            // 
            // Column10
            // 
            this.Column10.DataPropertyName = "sfy";
            this.Column10.HeaderText = "收费员";
            this.Column10.Name = "Column10";
            this.Column10.ReadOnly = true;
            this.Column10.Width = 80;
            // 
            // Column3
            // 
            this.Column3.DataPropertyName = "sfsj";
            this.Column3.HeaderText = "收费时间";
            this.Column3.Name = "Column3";
            this.Column3.ReadOnly = true;
            // 
            // Column5
            // 
            this.Column5.DataPropertyName = "tcjjzf";
            this.Column5.HeaderText = "统筹支付";
            this.Column5.Name = "Column5";
            this.Column5.ReadOnly = true;
            // 
            // Column13
            // 
            this.Column13.DataPropertyName = "dejjzf";
            this.Column13.HeaderText = "大病支付";
            this.Column13.Name = "Column13";
            this.Column13.ReadOnly = true;
            // 
            // Column14
            // 
            this.Column14.DataPropertyName = "xjzf";
            this.Column14.HeaderText = "个人支付";
            this.Column14.Name = "Column14";
            this.Column14.ReadOnly = true;
            // 
            // Column15
            // 
            this.Column15.DataPropertyName = "yyfdfy";
            this.Column15.HeaderText = "医院负担";
            this.Column15.Name = "Column15";
            this.Column15.ReadOnly = true;
            // 
            // Column16
            // 
            this.Column16.DataPropertyName = "mzjzfy";
            this.Column16.HeaderText = "民政救助";
            this.Column16.Name = "Column16";
            this.Column16.ReadOnly = true;
            // 
            // Column17
            // 
            this.Column17.DataPropertyName = "jrtcfy";
            this.Column17.HeaderText = "进入统筹费用";
            this.Column17.Name = "Column17";
            this.Column17.ReadOnly = true;
            // 
            // btnDC
            // 
            this.btnDC.Enabled = false;
            this.btnDC.Location = new System.Drawing.Point(800, 169);
            this.btnDC.Name = "btnDC";
            this.btnDC.Size = new System.Drawing.Size(75, 31);
            this.btnDC.TabIndex = 221;
            this.btnDC.Text = "导出";
            this.btnDC.UseVisualStyleBackColor = true;
            this.btnDC.Visible = false;
            this.btnDC.Click += new System.EventHandler(this.btnDC_Click);
            // 
            // btn_Close
            // 
            this.btn_Close.Location = new System.Drawing.Point(992, 8);
            this.btn_Close.Name = "btn_Close";
            this.btn_Close.Size = new System.Drawing.Size(86, 40);
            this.btn_Close.TabIndex = 222;
            this.btn_Close.Text = "关闭";
            this.btn_Close.UseVisualStyleBackColor = true;
            this.btn_Close.Click += new System.EventHandler(this.btn_Close_Click);
            // 
            // cmb_tj
            // 
            this.cmb_tj.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmb_tj.FormattingEnabled = true;
            this.cmb_tj.Location = new System.Drawing.Point(309, 17);
            this.cmb_tj.Name = "cmb_tj";
            this.cmb_tj.Size = new System.Drawing.Size(98, 20);
            this.cmb_tj.TabIndex = 224;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(274, 22);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(29, 12);
            this.label5.TabIndex = 223;
            this.label5.Text = "条件";
            // 
            // txt_nr
            // 
            this.txt_nr.Location = new System.Drawing.Point(413, 17);
            this.txt_nr.Name = "txt_nr";
            this.txt_nr.Size = new System.Drawing.Size(83, 21);
            this.txt_nr.TabIndex = 225;
            // 
            // cmb_tcq
            // 
            this.cmb_tcq.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmb_tcq.FormattingEnabled = true;
            this.cmb_tcq.Location = new System.Drawing.Point(556, 19);
            this.cmb_tcq.Name = "cmb_tcq";
            this.cmb_tcq.Size = new System.Drawing.Size(120, 20);
            this.cmb_tcq.TabIndex = 227;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(509, 23);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(41, 12);
            this.label7.TabIndex = 226;
            this.label7.Text = "统筹区";
            // 
            // lbl_rc
            // 
            this.lbl_rc.AutoSize = true;
            this.lbl_rc.Font = new System.Drawing.Font("宋体", 12F);
            this.lbl_rc.ForeColor = System.Drawing.Color.Red;
            this.lbl_rc.Location = new System.Drawing.Point(876, 21);
            this.lbl_rc.Name = "lbl_rc";
            this.lbl_rc.Size = new System.Drawing.Size(56, 16);
            this.lbl_rc.TabIndex = 228;
            this.lbl_rc.Text = "人次：";
            // 
            // btn_dccvs
            // 
            this.btn_dccvs.Location = new System.Drawing.Point(795, 13);
            this.btn_dccvs.Name = "btn_dccvs";
            this.btn_dccvs.Size = new System.Drawing.Size(75, 32);
            this.btn_dccvs.TabIndex = 248;
            this.btn_dccvs.Text = "导出cvs";
            this.btn_dccvs.UseVisualStyleBackColor = true;
            this.btn_dccvs.Click += new System.EventHandler(this.btn_dccvs_Click);
            // 
            // Frm_ybmfxtcxdrjj
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1090, 389);
            this.Controls.Add(this.btn_dccvs);
            this.Controls.Add(this.lbl_rc);
            this.Controls.Add(this.cmb_tcq);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.txt_nr);
            this.Controls.Add(this.cmb_tj);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.btn_Close);
            this.Controls.Add(this.btnDC);
            this.Controls.Add(this.dgvbf);
            this.Controls.Add(this.btncx);
            this.Controls.Add(this.dateTimeEnd);
            this.Controls.Add(this.dateTimeStar);
            this.Controls.Add(this.label4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Frm_ybmfxtcxdrjj";
            this.Text = "免费血透患者查询(东软)";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvbf)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DateTimePicker dateTimeEnd;
        private System.Windows.Forms.DateTimePicker dateTimeStar;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btncx;
        private System.Windows.Forms.DataGridView dgvbf;
        private System.Windows.Forms.Button btnDC;
        private System.Windows.Forms.Button btn_Close;
        private System.Windows.Forms.ComboBox cmb_tj;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txt_nr;
        private System.Windows.Forms.ComboBox cmb_tcq;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label lbl_rc;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column18;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column11;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column28;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column12;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column19;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column7;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column8;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column4;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column2;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column9;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column10;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column3;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column5;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column13;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column14;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column15;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column16;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column17;
        private System.Windows.Forms.Button btn_dccvs;
    }
}

