namespace ybinterface_lib
{
    partial class Frm_ybjmzybcmxjj
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Frm_ybjmzybcmxjj));
            this.dateTimeEnd = new System.Windows.Forms.DateTimePicker();
            this.dateTimeStar = new System.Windows.Forms.DateTimePicker();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.btncx = new System.Windows.Forms.Button();
            this.dgvbf = new System.Windows.Forms.DataGridView();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column12 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column6 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column7 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column8 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column10 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column11 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column9 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column13 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btn_PrintMX = new System.Windows.Forms.Button();
            this.infoDataSet1 = new Srvtools.InfoDataSet(this.components);
            this.button1 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvbf)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.infoDataSet1)).BeginInit();
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
            // btncx
            // 
            this.btncx.Location = new System.Drawing.Point(82, 69);
            this.btncx.Name = "btncx";
            this.btncx.Size = new System.Drawing.Size(75, 23);
            this.btncx.TabIndex = 215;
            this.btncx.Text = "查询";
            this.btncx.UseVisualStyleBackColor = true;
            this.btncx.Click += new System.EventHandler(this.btncx_Click);
            // 
            // dgvbf
            // 
            this.dgvbf.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvbf.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column1,
            this.Column12,
            this.Column2,
            this.Column3,
            this.Column4,
            this.Column5,
            this.Column6,
            this.Column7,
            this.Column8,
            this.Column10,
            this.Column11,
            this.Column9,
            this.Column13});
            this.dgvbf.Location = new System.Drawing.Point(0, 120);
            this.dgvbf.Name = "dgvbf";
            this.dgvbf.RowTemplate.Height = 23;
            this.dgvbf.Size = new System.Drawing.Size(1147, 317);
            this.dgvbf.TabIndex = 217;
            this.dgvbf.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvbf_CellContentClick);
            // 
            // Column1
            // 
            this.Column1.DataPropertyName = "jzlsh";
            this.Column1.HeaderText = "就诊流水号";
            this.Column1.Name = "Column1";
            this.Column1.ReadOnly = true;
            // 
            // Column12
            // 
            this.Column12.DataPropertyName = "kh";
            this.Column12.HeaderText = "社会保障卡号";
            this.Column12.Name = "Column12";
            this.Column12.ReadOnly = true;
            // 
            // Column2
            // 
            this.Column2.DataPropertyName = "xm";
            this.Column2.HeaderText = "姓名";
            this.Column2.Name = "Column2";
            this.Column2.ReadOnly = true;
            // 
            // Column3
            // 
            this.Column3.DataPropertyName = "bzmc";
            this.Column3.HeaderText = "诊断";
            this.Column3.Name = "Column3";
            this.Column3.ReadOnly = true;
            // 
            // Column4
            // 
            this.Column4.DataPropertyName = "dwmc";
            this.Column4.HeaderText = "地址";
            this.Column4.Name = "Column4";
            this.Column4.ReadOnly = true;
            // 
            // Column5
            // 
            this.Column5.DataPropertyName = "ryrq";
            this.Column5.HeaderText = "入院日期";
            this.Column5.Name = "Column5";
            this.Column5.ReadOnly = true;
            // 
            // Column6
            // 
            this.Column6.DataPropertyName = "cyrq";
            this.Column6.HeaderText = "出院日期";
            this.Column6.Name = "Column6";
            this.Column6.ReadOnly = true;
            // 
            // Column7
            // 
            this.Column7.DataPropertyName = "ts";
            this.Column7.HeaderText = "天数";
            this.Column7.Name = "Column7";
            this.Column7.ReadOnly = true;
            // 
            // Column8
            // 
            this.Column8.DataPropertyName = "fpje";
            this.Column8.HeaderText = "发票金额";
            this.Column8.Name = "Column8";
            this.Column8.ReadOnly = true;
            // 
            // Column10
            // 
            this.Column10.DataPropertyName = "zyf";
            this.Column10.HeaderText = "总药费";
            this.Column10.Name = "Column10";
            this.Column10.ReadOnly = true;
            // 
            // Column11
            // 
            this.Column11.DataPropertyName = "kbje";
            this.Column11.HeaderText = "可报金额";
            this.Column11.Name = "Column11";
            this.Column11.ReadOnly = true;
            // 
            // Column9
            // 
            this.Column9.DataPropertyName = "sbje";
            this.Column9.HeaderText = "实补金额";
            this.Column9.Name = "Column9";
            this.Column9.ReadOnly = true;
            // 
            // Column13
            // 
            this.Column13.DataPropertyName = "qfx";
            this.Column13.HeaderText = "起付线";
            this.Column13.Name = "Column13";
            // 
            // btn_PrintMX
            // 
            this.btn_PrintMX.Location = new System.Drawing.Point(203, 69);
            this.btn_PrintMX.Name = "btn_PrintMX";
            this.btn_PrintMX.Size = new System.Drawing.Size(93, 23);
            this.btn_PrintMX.TabIndex = 221;
            this.btn_PrintMX.Text = "打印人员明细";
            this.btn_PrintMX.UseVisualStyleBackColor = true;
            this.btn_PrintMX.Click += new System.EventHandler(this.btn_PrintMX_Click);
            // 
            // infoDataSet1
            // 
            this.infoDataSet1.Active = false;
            this.infoDataSet1.AlwaysClose = false;
            this.infoDataSet1.DataCompressed = false;
            this.infoDataSet1.DeleteIncomplete = true;
            this.infoDataSet1.LastKeyValues = null;
            this.infoDataSet1.Locale = new System.Globalization.CultureInfo("zh-CN");
            this.infoDataSet1.PacketRecords = 100;
            this.infoDataSet1.Position = -1;
            this.infoDataSet1.RemoteName = null;
            this.infoDataSet1.ServerModify = false;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(361, 69);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(93, 23);
            this.button1.TabIndex = 223;
            this.button1.Text = "导出人员明细";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click_1);
            // 
            // Frm_ybjmzybcmxjj
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1159, 440);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.btn_PrintMX);
            this.Controls.Add(this.dgvbf);
            this.Controls.Add(this.btncx);
            this.Controls.Add(this.dateTimeEnd);
            this.Controls.Add(this.dateTimeStar);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "Frm_ybjmzybcmxjj";
            this.Text = "医保拨付查询";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvbf)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.infoDataSet1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DateTimePicker dateTimeEnd;
        private System.Windows.Forms.DateTimePicker dateTimeStar;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btncx;
        private System.Windows.Forms.DataGridView dgvbf;
        private System.Windows.Forms.Button btn_PrintMX;
        private Srvtools.InfoDataSet infoDataSet1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column12;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column2;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column3;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column4;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column5;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column6;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column7;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column8;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column10;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column11;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column9;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column13;
        private System.Windows.Forms.Button button1;
    }
}

