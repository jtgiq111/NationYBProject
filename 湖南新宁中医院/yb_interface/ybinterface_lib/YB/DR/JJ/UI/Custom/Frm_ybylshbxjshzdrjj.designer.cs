namespace ybinterface_lib
{
    partial class Frm_ybylshbxjshzdrjj
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
            this.dateTimeEnd = new System.Windows.Forms.DateTimePicker();
            this.dateTimeStar = new System.Windows.Forms.DateTimePicker();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.rbtnflx = new System.Windows.Forms.RadioButton();
            this.rbtnlx = new System.Windows.Forms.RadioButton();
            this.rbtnjm = new System.Windows.Forms.RadioButton();
            this.label2 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.rbtnzy = new System.Windows.Forms.RadioButton();
            this.rbtnmz = new System.Windows.Forms.RadioButton();
            this.cmbrylx = new System.Windows.Forms.ComboBox();
            this.cmbyllb = new System.Windows.Forms.ComboBox();
            this.btncx = new System.Windows.Forms.Button();
            this.btnprint = new System.Windows.Forms.Button();
            this.dgvbf = new System.Windows.Forms.DataGridView();
            this.rbtnqy = new System.Windows.Forms.RadioButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.cmb_tcq = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.btn_Close = new System.Windows.Forms.Button();
            this.Column6 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column7 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column8 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column9 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column12 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column10 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column11 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dgvbf)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // dateTimeEnd
            // 
            this.dateTimeEnd.Location = new System.Drawing.Point(203, 16);
            this.dateTimeEnd.Name = "dateTimeEnd";
            this.dateTimeEnd.Size = new System.Drawing.Size(109, 21);
            this.dateTimeEnd.TabIndex = 200;
            // 
            // dateTimeStar
            // 
            this.dateTimeStar.Location = new System.Drawing.Point(65, 17);
            this.dateTimeStar.Name = "dateTimeStar";
            this.dateTimeStar.Size = new System.Drawing.Size(109, 21);
            this.dateTimeStar.TabIndex = 199;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(180, 20);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(17, 12);
            this.label4.TabIndex = 198;
            this.label4.Text = "至";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(30, 23);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(29, 12);
            this.label3.TabIndex = 197;
            this.label3.Text = "日期";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(30, 63);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 12);
            this.label1.TabIndex = 201;
            this.label1.Text = "人员大类";
            // 
            // rbtnflx
            // 
            this.rbtnflx.AutoSize = true;
            this.rbtnflx.Checked = true;
            this.rbtnflx.Location = new System.Drawing.Point(6, 12);
            this.rbtnflx.Name = "rbtnflx";
            this.rbtnflx.Size = new System.Drawing.Size(59, 16);
            this.rbtnflx.TabIndex = 202;
            this.rbtnflx.TabStop = true;
            this.rbtnflx.Tag = "";
            this.rbtnflx.Text = "非离休";
            this.rbtnflx.UseVisualStyleBackColor = true;
            this.rbtnflx.Click += new System.EventHandler(this.rbtnrylx_Click);
            // 
            // rbtnlx
            // 
            this.rbtnlx.AutoSize = true;
            this.rbtnlx.Location = new System.Drawing.Point(91, 14);
            this.rbtnlx.Name = "rbtnlx";
            this.rbtnlx.Size = new System.Drawing.Size(47, 16);
            this.rbtnlx.TabIndex = 203;
            this.rbtnlx.Tag = "";
            this.rbtnlx.Text = "离休";
            this.rbtnlx.UseVisualStyleBackColor = true;
            this.rbtnlx.Click += new System.EventHandler(this.rbtnrylx_Click);
            // 
            // rbtnjm
            // 
            this.rbtnjm.AutoSize = true;
            this.rbtnjm.Location = new System.Drawing.Point(177, 12);
            this.rbtnjm.Name = "rbtnjm";
            this.rbtnjm.Size = new System.Drawing.Size(47, 16);
            this.rbtnjm.TabIndex = 204;
            this.rbtnjm.Tag = "";
            this.rbtnjm.Text = "居民";
            this.rbtnjm.UseVisualStyleBackColor = true;
            this.rbtnjm.Click += new System.EventHandler(this.rbtnrylx_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(339, 63);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 12);
            this.label2.TabIndex = 205;
            this.label2.Text = "人员类型";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(339, 23);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(53, 12);
            this.label5.TabIndex = 206;
            this.label5.Text = "医疗类别";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(30, 102);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(53, 12);
            this.label6.TabIndex = 207;
            this.label6.Text = "部门分类";
            // 
            // rbtnzy
            // 
            this.rbtnzy.AutoSize = true;
            this.rbtnzy.Location = new System.Drawing.Point(91, 9);
            this.rbtnzy.Name = "rbtnzy";
            this.rbtnzy.Size = new System.Drawing.Size(47, 16);
            this.rbtnzy.TabIndex = 209;
            this.rbtnzy.Text = "住院";
            this.rbtnzy.UseVisualStyleBackColor = true;
            // 
            // rbtnmz
            // 
            this.rbtnmz.AutoSize = true;
            this.rbtnmz.Location = new System.Drawing.Point(6, 9);
            this.rbtnmz.Name = "rbtnmz";
            this.rbtnmz.Size = new System.Drawing.Size(47, 16);
            this.rbtnmz.TabIndex = 208;
            this.rbtnmz.Text = "门诊";
            this.rbtnmz.UseVisualStyleBackColor = true;
            // 
            // cmbrylx
            // 
            this.cmbrylx.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbrylx.FormattingEnabled = true;
            this.cmbrylx.Location = new System.Drawing.Point(398, 57);
            this.cmbrylx.Name = "cmbrylx";
            this.cmbrylx.Size = new System.Drawing.Size(238, 20);
            this.cmbrylx.TabIndex = 211;
            // 
            // cmbyllb
            // 
            this.cmbyllb.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbyllb.FormattingEnabled = true;
            this.cmbyllb.Location = new System.Drawing.Point(398, 19);
            this.cmbyllb.Name = "cmbyllb";
            this.cmbyllb.Size = new System.Drawing.Size(238, 20);
            this.cmbyllb.TabIndex = 212;
            // 
            // btncx
            // 
            this.btncx.Location = new System.Drawing.Point(341, 128);
            this.btncx.Name = "btncx";
            this.btncx.Size = new System.Drawing.Size(75, 34);
            this.btncx.TabIndex = 215;
            this.btncx.Text = "查询";
            this.btncx.UseVisualStyleBackColor = true;
            this.btncx.Click += new System.EventHandler(this.btncx_Click);
            // 
            // btnprint
            // 
            this.btnprint.Enabled = false;
            this.btnprint.Location = new System.Drawing.Point(561, 128);
            this.btnprint.Name = "btnprint";
            this.btnprint.Size = new System.Drawing.Size(75, 34);
            this.btnprint.TabIndex = 216;
            this.btnprint.Text = "打印";
            this.btnprint.UseVisualStyleBackColor = true;
            this.btnprint.Click += new System.EventHandler(this.btnprint_Click);
            // 
            // dgvbf
            // 
            this.dgvbf.AllowUserToAddRows = false;
            this.dgvbf.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvbf.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column6,
            this.Column1,
            this.Column2,
            this.Column3,
            this.Column4,
            this.Column5,
            this.Column7,
            this.Column8,
            this.Column9,
            this.Column12,
            this.Column10,
            this.Column11});
            this.dgvbf.Location = new System.Drawing.Point(1, 178);
            this.dgvbf.Name = "dgvbf";
            this.dgvbf.RowTemplate.Height = 23;
            this.dgvbf.Size = new System.Drawing.Size(1014, 141);
            this.dgvbf.TabIndex = 217;
            // 
            // rbtnqy
            // 
            this.rbtnqy.AutoSize = true;
            this.rbtnqy.Checked = true;
            this.rbtnqy.Location = new System.Drawing.Point(177, 9);
            this.rbtnqy.Name = "rbtnqy";
            this.rbtnqy.Size = new System.Drawing.Size(47, 16);
            this.rbtnqy.TabIndex = 218;
            this.rbtnqy.TabStop = true;
            this.rbtnqy.Text = "全院";
            this.rbtnqy.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rbtnflx);
            this.groupBox1.Controls.Add(this.rbtnlx);
            this.groupBox1.Controls.Add(this.rbtnjm);
            this.groupBox1.Location = new System.Drawing.Point(82, 49);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(230, 34);
            this.groupBox1.TabIndex = 219;
            this.groupBox1.TabStop = false;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.rbtnmz);
            this.groupBox2.Controls.Add(this.rbtnqy);
            this.groupBox2.Controls.Add(this.rbtnzy);
            this.groupBox2.Location = new System.Drawing.Point(82, 89);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(230, 31);
            this.groupBox2.TabIndex = 220;
            this.groupBox2.TabStop = false;
            // 
            // cmb_tcq
            // 
            this.cmb_tcq.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmb_tcq.FormattingEnabled = true;
            this.cmb_tcq.Location = new System.Drawing.Point(398, 98);
            this.cmb_tcq.Name = "cmb_tcq";
            this.cmb_tcq.Size = new System.Drawing.Size(238, 20);
            this.cmb_tcq.TabIndex = 222;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(351, 102);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(41, 12);
            this.label7.TabIndex = 221;
            this.label7.Text = "统筹区";
            // 
            // btn_Close
            // 
            this.btn_Close.Location = new System.Drawing.Point(787, 17);
            this.btn_Close.Name = "btn_Close";
            this.btn_Close.Size = new System.Drawing.Size(100, 40);
            this.btn_Close.TabIndex = 223;
            this.btn_Close.Text = "关闭";
            this.btn_Close.UseVisualStyleBackColor = true;
            this.btn_Close.Click += new System.EventHandler(this.btn_Close_Click);
            // 
            // Column6
            // 
            this.Column6.DataPropertyName = "rc";
            this.Column6.HeaderText = "人次";
            this.Column6.Name = "Column6";
            this.Column6.ReadOnly = true;
            this.Column6.Width = 80;
            // 
            // Column1
            // 
            this.Column1.DataPropertyName = "ylfze";
            this.Column1.HeaderText = "费用总额";
            this.Column1.Name = "Column1";
            this.Column1.ReadOnly = true;
            this.Column1.Width = 80;
            // 
            // Column2
            // 
            this.Column2.DataPropertyName = "xjzf";
            this.Column2.HeaderText = "个人支付";
            this.Column2.Name = "Column2";
            this.Column2.ReadOnly = true;
            this.Column2.Width = 80;
            // 
            // Column3
            // 
            this.Column3.DataPropertyName = "zffy";
            this.Column3.HeaderText = "自费费用";
            this.Column3.Name = "Column3";
            this.Column3.ReadOnly = true;
            this.Column3.Width = 80;
            // 
            // Column4
            // 
            this.Column4.DataPropertyName = "zhzf";
            this.Column4.HeaderText = "账户支付";
            this.Column4.Name = "Column4";
            this.Column4.ReadOnly = true;
            this.Column4.Width = 80;
            // 
            // Column5
            // 
            this.Column5.DataPropertyName = "qfbzfy";
            this.Column5.HeaderText = "起付标准";
            this.Column5.Name = "Column5";
            this.Column5.ReadOnly = true;
            this.Column5.Width = 80;
            // 
            // Column7
            // 
            this.Column7.DataPropertyName = "tcjjzf";
            this.Column7.HeaderText = "基金支付";
            this.Column7.Name = "Column7";
            this.Column7.ReadOnly = true;
            this.Column7.Width = 80;
            // 
            // Column8
            // 
            this.Column8.DataPropertyName = "dejjzf";
            this.Column8.HeaderText = "大额基金";
            this.Column8.Name = "Column8";
            this.Column8.ReadOnly = true;
            this.Column8.Width = 80;
            // 
            // Column9
            // 
            this.Column9.DataPropertyName = "mzjzfy";
            this.Column9.HeaderText = "民政救助";
            this.Column9.Name = "Column9";
            this.Column9.ReadOnly = true;
            this.Column9.Width = 80;
            // 
            // Column12
            // 
            this.Column12.DataPropertyName = "xtmzjzfy";
            this.Column12.HeaderText = "血透民政";
            this.Column12.Name = "Column12";
            this.Column12.ReadOnly = true;
            this.Column12.Width = 80;
            // 
            // Column10
            // 
            this.Column10.DataPropertyName = "ylzlfy";
            this.Column10.HeaderText = "乙类自费";
            this.Column10.Name = "Column10";
            this.Column10.ReadOnly = true;
            this.Column10.Width = 80;
            // 
            // Column11
            // 
            this.Column11.DataPropertyName = "blzlfy";
            this.Column11.HeaderText = "丙类自费";
            this.Column11.Name = "Column11";
            this.Column11.ReadOnly = true;
            this.Column11.Width = 80;
            // 
            // Frm_ybylshbxjshzdrjj
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1020, 331);
            this.Controls.Add(this.btn_Close);
            this.Controls.Add(this.cmb_tcq);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.dgvbf);
            this.Controls.Add(this.btnprint);
            this.Controls.Add(this.btncx);
            this.Controls.Add(this.cmbyllb);
            this.Controls.Add(this.cmbrylx);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.dateTimeEnd);
            this.Controls.Add(this.dateTimeStar);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Frm_ybylshbxjshzdrjj";
            this.Text = "医疗社会保险结算汇总(东软)";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvbf)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DateTimePicker dateTimeEnd;
        private System.Windows.Forms.DateTimePicker dateTimeStar;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RadioButton rbtnflx;
        private System.Windows.Forms.RadioButton rbtnlx;
        private System.Windows.Forms.RadioButton rbtnjm;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.RadioButton rbtnzy;
        private System.Windows.Forms.RadioButton rbtnmz;
        private System.Windows.Forms.ComboBox cmbrylx;
        private System.Windows.Forms.ComboBox cmbyllb;
        private System.Windows.Forms.Button btncx;
        private System.Windows.Forms.Button btnprint;
        private System.Windows.Forms.DataGridView dgvbf;
        private System.Windows.Forms.RadioButton rbtnqy;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ComboBox cmb_tcq;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button btn_Close;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column6;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column2;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column3;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column4;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column5;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column7;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column8;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column9;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column12;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column10;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column11;
    }
}

