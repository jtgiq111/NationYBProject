namespace ybinterface_lib
{
    partial class Frm_ybyjscxdr
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
            this.btncx = new System.Windows.Forms.Button();
            this.dgv_yjs = new System.Windows.Forms.DataGridView();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column11 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column6 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column9 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column7 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column12 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column8 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column10 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column13 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column14 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column15 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column16 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column17 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column18 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column19 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column20 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column21 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column22 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column23 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.txt_jzlsh = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btn_Close = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgv_yjs)).BeginInit();
            this.SuspendLayout();
            // 
            // dateTimeEnd
            // 
            this.dateTimeEnd.Location = new System.Drawing.Point(185, 50);
            this.dateTimeEnd.Name = "dateTimeEnd";
            this.dateTimeEnd.Size = new System.Drawing.Size(109, 21);
            this.dateTimeEnd.TabIndex = 200;
            this.dateTimeEnd.Visible = false;
            // 
            // dateTimeStar
            // 
            this.dateTimeStar.Location = new System.Drawing.Point(47, 51);
            this.dateTimeStar.Name = "dateTimeStar";
            this.dateTimeStar.Size = new System.Drawing.Size(109, 21);
            this.dateTimeStar.TabIndex = 199;
            this.dateTimeStar.Visible = false;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(162, 54);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(17, 12);
            this.label4.TabIndex = 198;
            this.label4.Text = "至";
            this.label4.Visible = false;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 57);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(29, 12);
            this.label3.TabIndex = 197;
            this.label3.Text = "日期";
            this.label3.Visible = false;
            // 
            // btncx
            // 
            this.btncx.Location = new System.Drawing.Point(382, 12);
            this.btncx.Name = "btncx";
            this.btncx.Size = new System.Drawing.Size(75, 23);
            this.btncx.TabIndex = 215;
            this.btncx.Text = "查询";
            this.btncx.UseVisualStyleBackColor = true;
            this.btncx.Click += new System.EventHandler(this.btncx_Click);
            // 
            // dgv_yjs
            // 
            this.dgv_yjs.AllowUserToAddRows = false;
            this.dgv_yjs.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv_yjs.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column1,
            this.Column2,
            this.Column3,
            this.Column4,
            this.Column11,
            this.Column5,
            this.Column6,
            this.Column9,
            this.Column7,
            this.Column12,
            this.Column8,
            this.Column10,
            this.Column13,
            this.Column14,
            this.Column15,
            this.Column16,
            this.Column17,
            this.Column18,
            this.Column19,
            this.Column20,
            this.Column21,
            this.Column22,
            this.Column23});
            this.dgv_yjs.Location = new System.Drawing.Point(5, 111);
            this.dgv_yjs.Name = "dgv_yjs";
            this.dgv_yjs.RowTemplate.Height = 23;
            this.dgv_yjs.Size = new System.Drawing.Size(1151, 317);
            this.dgv_yjs.TabIndex = 217;
            // 
            // Column1
            // 
            this.Column1.DataPropertyName = "jzlsh";
            this.Column1.HeaderText = "流水号";
            this.Column1.Name = "Column1";
            this.Column1.ReadOnly = true;
            // 
            // Column2
            // 
            this.Column2.DataPropertyName = "ylfze";
            this.Column2.HeaderText = "总医疗费";
            this.Column2.Name = "Column2";
            this.Column2.ReadOnly = true;
            // 
            // Column3
            // 
            this.Column3.DataPropertyName = "zbxje";
            this.Column3.HeaderText = "总报销";
            this.Column3.Name = "Column3";
            this.Column3.ReadOnly = true;
            // 
            // Column4
            // 
            this.Column4.DataPropertyName = "tcjjzf";
            this.Column4.HeaderText = "统筹基金";
            this.Column4.Name = "Column4";
            this.Column4.ReadOnly = true;
            // 
            // Column11
            // 
            this.Column11.DataPropertyName = "dejjzf";
            this.Column11.HeaderText = "大额基金";
            this.Column11.Name = "Column11";
            this.Column11.ReadOnly = true;
            // 
            // Column5
            // 
            this.Column5.DataPropertyName = "zhzf";
            this.Column5.HeaderText = "账户支付";
            this.Column5.Name = "Column5";
            this.Column5.ReadOnly = true;
            // 
            // Column6
            // 
            this.Column6.DataPropertyName = "xjzf";
            this.Column6.HeaderText = "现金支付";
            this.Column6.Name = "Column6";
            this.Column6.ReadOnly = true;
            // 
            // Column9
            // 
            this.Column9.DataPropertyName = "gwybzjjzf";
            this.Column9.HeaderText = "公务员补助";
            this.Column9.Name = "Column9";
            this.Column9.ReadOnly = true;
            // 
            // Column7
            // 
            this.Column7.DataPropertyName = "zffy";
            this.Column7.HeaderText = "自费费用";
            this.Column7.Name = "Column7";
            this.Column7.ReadOnly = true;
            // 
            // Column12
            // 
            this.Column12.DataPropertyName = "dwfdfy";
            this.Column12.HeaderText = "单位负担";
            this.Column12.Name = "Column12";
            this.Column12.ReadOnly = true;
            // 
            // Column8
            // 
            this.Column8.DataPropertyName = "yyfdfy";
            this.Column8.HeaderText = "医院负担";
            this.Column8.Name = "Column8";
            this.Column8.ReadOnly = true;
            // 
            // Column10
            // 
            this.Column10.DataPropertyName = "mzjzfy";
            this.Column10.HeaderText = "民政救助";
            this.Column10.Name = "Column10";
            this.Column10.ReadOnly = true;
            // 
            // Column13
            // 
            this.Column13.DataPropertyName = "cxjfy";
            this.Column13.HeaderText = "超限价";
            this.Column13.Name = "Column13";
            this.Column13.ReadOnly = true;
            // 
            // Column14
            // 
            this.Column14.DataPropertyName = "qfbzfy";
            this.Column14.HeaderText = "起付标准";
            this.Column14.Name = "Column14";
            this.Column14.ReadOnly = true;
            // 
            // Column15
            // 
            this.Column15.DataPropertyName = "jrtcfy";
            this.Column15.HeaderText = "进入统筹";
            this.Column15.Name = "Column15";
            this.Column15.ReadOnly = true;
            // 
            // Column16
            // 
            this.Column16.DataPropertyName = "bcjsqzhye";
            this.Column16.HeaderText = "账户余额";
            this.Column16.Name = "Column16";
            this.Column16.ReadOnly = true;
            // 
            // Column17
            // 
            this.Column17.DataPropertyName = "bntczflj";
            this.Column17.HeaderText = "统筹累计";
            this.Column17.Name = "Column17";
            this.Column17.ReadOnly = true;
            // 
            // Column18
            // 
            this.Column18.DataPropertyName = "bndezflj";
            this.Column18.HeaderText = "大额统筹累计";
            this.Column18.Name = "Column18";
            this.Column18.ReadOnly = true;
            // 
            // Column19
            // 
            this.Column19.DataPropertyName = "bnczjmmztczflj";
            this.Column19.HeaderText = "居民门诊统筹累计";
            this.Column19.Name = "Column19";
            this.Column19.ReadOnly = true;
            // 
            // Column20
            // 
            this.Column20.DataPropertyName = "bngwybzzflj";
            this.Column20.HeaderText = "公务员补助累计";
            this.Column20.Name = "Column20";
            this.Column20.ReadOnly = true;
            // 
            // Column21
            // 
            this.Column21.DataPropertyName = "bnzhzflj";
            this.Column21.HeaderText = "账户支付累计";
            this.Column21.Name = "Column21";
            this.Column21.ReadOnly = true;
            // 
            // Column22
            // 
            this.Column22.DataPropertyName = "bnzycslj";
            this.Column22.HeaderText = "住院次数累计";
            this.Column22.Name = "Column22";
            this.Column22.ReadOnly = true;
            // 
            // Column23
            // 
            this.Column23.DataPropertyName = "mmqflj";
            this.Column23.HeaderText = "门慢起付累计";
            this.Column23.Name = "Column23";
            this.Column23.ReadOnly = true;
            // 
            // txt_jzlsh
            // 
            this.txt_jzlsh.Location = new System.Drawing.Point(83, 14);
            this.txt_jzlsh.MaxLength = 10;
            this.txt_jzlsh.Name = "txt_jzlsh";
            this.txt_jzlsh.Size = new System.Drawing.Size(243, 21);
            this.txt_jzlsh.TabIndex = 219;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 21);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 12);
            this.label2.TabIndex = 218;
            this.label2.Text = "就诊流水号";
            // 
            // btn_Close
            // 
            this.btn_Close.Location = new System.Drawing.Point(1047, 12);
            this.btn_Close.Name = "btn_Close";
            this.btn_Close.Size = new System.Drawing.Size(100, 40);
            this.btn_Close.TabIndex = 220;
            this.btn_Close.Text = "关闭";
            this.btn_Close.UseVisualStyleBackColor = true;
            this.btn_Close.Click += new System.EventHandler(this.btn_Close_Click);
            // 
            // Frm_ybyjscxdr
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1159, 440);
            this.Controls.Add(this.btn_Close);
            this.Controls.Add(this.txt_jzlsh);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.dgv_yjs);
            this.Controls.Add(this.btncx);
            this.Controls.Add(this.dateTimeEnd);
            this.Controls.Add(this.dateTimeStar);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Frm_ybyjscxdr";
            this.Text = "医保预结算查询(东软)";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgv_yjs)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DateTimePicker dateTimeEnd;
        private System.Windows.Forms.DateTimePicker dateTimeStar;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btncx;
        private System.Windows.Forms.DataGridView dgv_yjs;
        private System.Windows.Forms.TextBox txt_jzlsh;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column2;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column3;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column4;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column11;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column5;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column6;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column9;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column7;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column12;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column8;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column10;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column13;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column14;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column15;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column16;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column17;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column18;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column19;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column20;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column21;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column22;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column23;
        private System.Windows.Forms.Button btn_Close;
    }
}

