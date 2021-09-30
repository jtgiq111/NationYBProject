namespace ybinterface_lib
{
    partial class Frm_ybmzsfrbdrjj
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
            this.dt_start = new System.Windows.Forms.DateTimePicker();
            this.btncx = new System.Windows.Forms.Button();
            this.dgv_ybmzsfrb = new System.Windows.Forms.DataGridView();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column11 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column6 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column7 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btn_Close = new System.Windows.Forms.Button();
            this.btn_Print = new System.Windows.Forms.Button();
            this.rbtn_ny = new System.Windows.Forms.RadioButton();
            this.rbtn_by = new System.Windows.Forms.RadioButton();
            this.rbtn_pp = new System.Windows.Forms.RadioButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rbtn_qy = new System.Windows.Forms.RadioButton();
            this.label1 = new System.Windows.Forms.Label();
            this.dt_end = new System.Windows.Forms.DateTimePicker();
            ((System.ComponentModel.ISupportInitialize)(this.dgv_ybmzsfrb)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // dt_start
            // 
            this.dt_start.Location = new System.Drawing.Point(5, 22);
            this.dt_start.Name = "dt_start";
            this.dt_start.Size = new System.Drawing.Size(106, 21);
            this.dt_start.TabIndex = 199;
            this.dt_start.ValueChanged += new System.EventHandler(this.dt_start_ValueChanged);
            // 
            // btncx
            // 
            this.btncx.Location = new System.Drawing.Point(487, 15);
            this.btncx.Name = "btncx";
            this.btncx.Size = new System.Drawing.Size(75, 36);
            this.btncx.TabIndex = 215;
            this.btncx.Text = "查询";
            this.btncx.UseVisualStyleBackColor = true;
            this.btncx.Click += new System.EventHandler(this.btncx_Click);
            // 
            // dgv_ybmzsfrb
            // 
            this.dgv_ybmzsfrb.AllowUserToAddRows = false;
            this.dgv_ybmzsfrb.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv_ybmzsfrb.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column1,
            this.Column2,
            this.Column4,
            this.Column11,
            this.Column5,
            this.Column6,
            this.Column7});
            this.dgv_ybmzsfrb.Location = new System.Drawing.Point(5, 71);
            this.dgv_ybmzsfrb.Name = "dgv_ybmzsfrb";
            this.dgv_ybmzsfrb.RowTemplate.Height = 23;
            this.dgv_ybmzsfrb.Size = new System.Drawing.Size(812, 357);
            this.dgv_ybmzsfrb.TabIndex = 217;
            // 
            // Column1
            // 
            this.Column1.DataPropertyName = "jbr";
            this.Column1.HeaderText = "收费部门";
            this.Column1.Name = "Column1";
            this.Column1.ReadOnly = true;
            // 
            // Column2
            // 
            this.Column2.DataPropertyName = "fyze";
            this.Column2.HeaderText = "费用总额";
            this.Column2.Name = "Column2";
            this.Column2.ReadOnly = true;
            // 
            // Column4
            // 
            this.Column4.DataPropertyName = "zhbf";
            this.Column4.HeaderText = "账户拨付";
            this.Column4.Name = "Column4";
            this.Column4.ReadOnly = true;
            // 
            // Column11
            // 
            this.Column11.DataPropertyName = "jbtczf";
            this.Column11.HeaderText = "基本统筹支付";
            this.Column11.Name = "Column11";
            this.Column11.ReadOnly = true;
            this.Column11.Width = 120;
            // 
            // Column5
            // 
            this.Column5.DataPropertyName = "dbtczf";
            this.Column5.HeaderText = "大病统筹支付";
            this.Column5.Name = "Column5";
            this.Column5.ReadOnly = true;
            this.Column5.Width = 120;
            // 
            // Column6
            // 
            this.Column6.DataPropertyName = "shjzzf";
            this.Column6.HeaderText = "社会救助支付";
            this.Column6.Name = "Column6";
            this.Column6.ReadOnly = true;
            this.Column6.Width = 120;
            // 
            // Column7
            // 
            this.Column7.DataPropertyName = "sjxj";
            this.Column7.HeaderText = "实交现金";
            this.Column7.Name = "Column7";
            this.Column7.ReadOnly = true;
            // 
            // btn_Close
            // 
            this.btn_Close.Location = new System.Drawing.Point(707, 12);
            this.btn_Close.Name = "btn_Close";
            this.btn_Close.Size = new System.Drawing.Size(100, 40);
            this.btn_Close.TabIndex = 220;
            this.btn_Close.Text = "关闭";
            this.btn_Close.UseVisualStyleBackColor = true;
            this.btn_Close.Click += new System.EventHandler(this.btn_Close_Click);
            // 
            // btn_Print
            // 
            this.btn_Print.Enabled = false;
            this.btn_Print.Location = new System.Drawing.Point(587, 13);
            this.btn_Print.Name = "btn_Print";
            this.btn_Print.Size = new System.Drawing.Size(100, 40);
            this.btn_Print.TabIndex = 221;
            this.btn_Print.Text = "打印";
            this.btn_Print.UseVisualStyleBackColor = true;
            this.btn_Print.Click += new System.EventHandler(this.btn_Print_Click);
            // 
            // rbtn_ny
            // 
            this.rbtn_ny.AutoSize = true;
            this.rbtn_ny.Location = new System.Drawing.Point(6, 16);
            this.rbtn_ny.Name = "rbtn_ny";
            this.rbtn_ny.Size = new System.Drawing.Size(47, 16);
            this.rbtn_ny.TabIndex = 222;
            this.rbtn_ny.Tag = "1";
            this.rbtn_ny.Text = "南院";
            this.rbtn_ny.UseVisualStyleBackColor = true;
            this.rbtn_ny.Click += new System.EventHandler(this.rbtn_ny_Click);
            // 
            // rbtn_by
            // 
            this.rbtn_by.AutoSize = true;
            this.rbtn_by.Location = new System.Drawing.Point(59, 15);
            this.rbtn_by.Name = "rbtn_by";
            this.rbtn_by.Size = new System.Drawing.Size(47, 16);
            this.rbtn_by.TabIndex = 223;
            this.rbtn_by.Tag = "2";
            this.rbtn_by.Text = "北院";
            this.rbtn_by.UseVisualStyleBackColor = true;
            this.rbtn_by.Click += new System.EventHandler(this.rbtn_by_Click);
            // 
            // rbtn_pp
            // 
            this.rbtn_pp.AutoSize = true;
            this.rbtn_pp.Location = new System.Drawing.Point(112, 17);
            this.rbtn_pp.Name = "rbtn_pp";
            this.rbtn_pp.Size = new System.Drawing.Size(47, 16);
            this.rbtn_pp.TabIndex = 224;
            this.rbtn_pp.Tag = "3";
            this.rbtn_pp.Text = "湓浦";
            this.rbtn_pp.UseVisualStyleBackColor = true;
            this.rbtn_pp.Click += new System.EventHandler(this.rbtn_pp_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rbtn_qy);
            this.groupBox1.Controls.Add(this.rbtn_ny);
            this.groupBox1.Controls.Add(this.rbtn_pp);
            this.groupBox1.Controls.Add(this.rbtn_by);
            this.groupBox1.Location = new System.Drawing.Point(259, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(222, 38);
            this.groupBox1.TabIndex = 225;
            this.groupBox1.TabStop = false;
            // 
            // rbtn_qy
            // 
            this.rbtn_qy.AutoSize = true;
            this.rbtn_qy.Checked = true;
            this.rbtn_qy.Location = new System.Drawing.Point(169, 17);
            this.rbtn_qy.Name = "rbtn_qy";
            this.rbtn_qy.Size = new System.Drawing.Size(47, 16);
            this.rbtn_qy.TabIndex = 225;
            this.rbtn_qy.TabStop = true;
            this.rbtn_qy.Tag = "3";
            this.rbtn_qy.Text = "全院";
            this.rbtn_qy.UseVisualStyleBackColor = true;
            this.rbtn_qy.Click += new System.EventHandler(this.rbtn_qy_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(117, 27);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(17, 12);
            this.label1.TabIndex = 226;
            this.label1.Text = "至";
            // 
            // dt_end
            // 
            this.dt_end.Location = new System.Drawing.Point(140, 22);
            this.dt_end.Name = "dt_end";
            this.dt_end.Size = new System.Drawing.Size(106, 21);
            this.dt_end.TabIndex = 227;
            this.dt_end.ValueChanged += new System.EventHandler(this.dt_end_ValueChanged);
            // 
            // Frm_ybmzsfrbdrjj
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(819, 440);
            this.Controls.Add(this.dt_end);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btn_Print);
            this.Controls.Add(this.btn_Close);
            this.Controls.Add(this.dgv_ybmzsfrb);
            this.Controls.Add(this.btncx);
            this.Controls.Add(this.dt_start);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Frm_ybmzsfrbdrjj";
            this.Text = "医保门诊收费日报查询(东软)";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgv_ybmzsfrb)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DateTimePicker dt_start;
        private System.Windows.Forms.Button btncx;
        private System.Windows.Forms.DataGridView dgv_ybmzsfrb;
        private System.Windows.Forms.Button btn_Close;
        private System.Windows.Forms.Button btn_Print;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column2;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column4;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column11;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column5;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column6;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column7;
        private System.Windows.Forms.RadioButton rbtn_ny;
        private System.Windows.Forms.RadioButton rbtn_by;
        private System.Windows.Forms.RadioButton rbtn_pp;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton rbtn_qy;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DateTimePicker dt_end;
    }
}

