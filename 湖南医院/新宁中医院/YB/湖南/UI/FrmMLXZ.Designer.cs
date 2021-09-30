namespace yb_interfaces
{
    partial class FrmMLXZ
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
            this.label1 = new System.Windows.Forms.Label();
            this.cbxXZZL = new System.Windows.Forms.ComboBox();
            this.btnxz = new System.Windows.Forms.Button();
            this.gridView = new Srvtools.InfoDataGridView();
            this.btnxz2 = new System.Windows.Forms.Button();
            this.txtbbh = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.chbxzyw = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.dtqueryDate = new System.Windows.Forms.DateTimePicker();
            this.txtZDkey = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.txtzdType = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.gridView)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(16, 11);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(142, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "请选择下载目录类型";
            // 
            // cbxXZZL
            // 
            this.cbxXZZL.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxXZZL.FormattingEnabled = true;
            this.cbxXZZL.Items.AddRange(new object[] {
            "1301西药中成药目录下载",
            "1302中药饮片目录下载",
            "1303医疗机构制剂目录下载",
            "1305医疗服务项目目录下载",
            "1306医用耗材目录下载",
            "1307疾病与诊断目录下载",
            "1308手术操作目录下载",
            "1309门诊慢特病种目录下载",
            "1310按病种付费病种目录下载",
            "1311日间手术治疗病种目录下载",
            "1313肿瘤形态学目录下载",
            "1314中医疾病目录下载",
            "1315中医证候目录下载",
            "-------以下请选下载2---------------",
            "1304民族药品目录查询",
            "1312医保目录信息下载",
            "1316医疗目录与医保目录匹配信息下载",
            "1317医药机构目录匹配信息下载",
            "1318医保目录限价信息下载",
            "1319医保目录先自付比例信息下载 "});
            this.cbxXZZL.Location = new System.Drawing.Point(19, 32);
            this.cbxXZZL.Margin = new System.Windows.Forms.Padding(4);
            this.cbxXZZL.Name = "cbxXZZL";
            this.cbxXZZL.Size = new System.Drawing.Size(248, 23);
            this.cbxXZZL.TabIndex = 1;
            // 
            // btnxz
            // 
            this.btnxz.Location = new System.Drawing.Point(19, 65);
            this.btnxz.Margin = new System.Windows.Forms.Padding(4);
            this.btnxz.Name = "btnxz";
            this.btnxz.Size = new System.Drawing.Size(100, 29);
            this.btnxz.TabIndex = 2;
            this.btnxz.Text = "下载";
            this.btnxz.UseVisualStyleBackColor = true;
            this.btnxz.Click += new System.EventHandler(this.button1_Click);
            // 
            // gridView
            // 
            this.gridView.AllowUserToAddRows = false;
            this.gridView.AllowUserToDeleteRows = false;
            this.gridView.BackgroundColor = System.Drawing.SystemColors.ControlLightLight;
            this.gridView.ColumnHeadersHeight = 24;
            this.gridView.Dock = System.Windows.Forms.DockStyle.Right;
            this.gridView.EnterEnable = true;
            this.gridView.EnterRefValControl = false;
            this.gridView.Location = new System.Drawing.Point(320, 0);
            this.gridView.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.gridView.Name = "gridView";
            this.gridView.ReadOnly = true;
            this.gridView.RowHeadersWidth = 51;
            this.gridView.RowTemplate.Height = 23;
            this.gridView.Size = new System.Drawing.Size(981, 642);
            this.gridView.SureDelete = false;
            this.gridView.TabIndex = 4;
            this.gridView.TotalActive = true;
            this.gridView.TotalBackColor = System.Drawing.SystemColors.Info;
            this.gridView.TotalCaption = null;
            this.gridView.TotalCaptionFont = new System.Drawing.Font("宋体", 9F);
            this.gridView.TotalFont = new System.Drawing.Font("宋体", 9F);
            // 
            // btnxz2
            // 
            this.btnxz2.Location = new System.Drawing.Point(19, 101);
            this.btnxz2.Margin = new System.Windows.Forms.Padding(4);
            this.btnxz2.Name = "btnxz2";
            this.btnxz2.Size = new System.Drawing.Size(100, 29);
            this.btnxz2.TabIndex = 10;
            this.btnxz2.Text = "下载2";
            this.btnxz2.UseVisualStyleBackColor = true;
            this.btnxz2.Click += new System.EventHandler(this.btnxz2_Click);
            // 
            // txtbbh
            // 
            this.txtbbh.Location = new System.Drawing.Point(16, 192);
            this.txtbbh.Margin = new System.Windows.Forms.Padding(4);
            this.txtbbh.Name = "txtbbh";
            this.txtbbh.Size = new System.Drawing.Size(248, 25);
            this.txtbbh.TabIndex = 11;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(23, 162);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(112, 15);
            this.label4.TabIndex = 12;
            this.label4.Text = "指定版本号下载";
            // 
            // chbxzyw
            // 
            this.chbxzyw.AutoSize = true;
            this.chbxzyw.Location = new System.Drawing.Point(149, 162);
            this.chbxzyw.Margin = new System.Windows.Forms.Padding(4);
            this.chbxzyw.Name = "chbxzyw";
            this.chbxzyw.Size = new System.Drawing.Size(89, 19);
            this.chbxzyw.TabIndex = 13;
            this.chbxzyw.Text = "不写入表";
            this.chbxzyw.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(23, 354);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(67, 15);
            this.label2.TabIndex = 15;
            this.label2.Text = "查询时间";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(23, 253);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(67, 15);
            this.label3.TabIndex = 17;
            this.label3.Text = "字典类型";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(23, 300);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(82, 15);
            this.label5.TabIndex = 19;
            this.label5.Text = "父字典键值";
            // 
            // dtqueryDate
            // 
            this.dtqueryDate.Location = new System.Drawing.Point(99, 349);
            this.dtqueryDate.Name = "dtqueryDate";
            this.dtqueryDate.Size = new System.Drawing.Size(139, 25);
            this.dtqueryDate.TabIndex = 22;
            // 
            // txtZDkey
            // 
            this.txtZDkey.Location = new System.Drawing.Point(115, 296);
            this.txtZDkey.Name = "txtZDkey";
            this.txtZDkey.Size = new System.Drawing.Size(100, 25);
            this.txtZDkey.TabIndex = 23;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(99, 402);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(116, 31);
            this.button1.TabIndex = 24;
            this.button1.Text = "字典查询";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click_1);
            // 
            // txtzdType
            // 
            this.txtzdType.Location = new System.Drawing.Point(115, 250);
            this.txtzdType.Name = "txtzdType";
            this.txtzdType.Size = new System.Drawing.Size(100, 25);
            this.txtzdType.TabIndex = 25;
            // 
            // FrmMLXZ
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1301, 642);
            this.Controls.Add(this.txtzdType);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.txtZDkey);
            this.Controls.Add(this.dtqueryDate);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.chbxzyw);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtbbh);
            this.Controls.Add(this.btnxz2);
            this.Controls.Add(this.gridView);
            this.Controls.Add(this.btnxz);
            this.Controls.Add(this.cbxXZZL);
            this.Controls.Add(this.label1);
            this.Margin = new System.Windows.Forms.Padding(5);
            this.Name = "FrmMLXZ";
            this.Text = "医保目录下载";
            ((System.ComponentModel.ISupportInitialize)(this.gridView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cbxXZZL;
        private System.Windows.Forms.Button btnxz;
        private Srvtools.InfoDataGridView gridView;
        private System.Windows.Forms.Button btnxz2;
        private System.Windows.Forms.TextBox txtbbh;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.CheckBox chbxzyw;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.DateTimePicker dtqueryDate;
        private System.Windows.Forms.TextBox txtZDkey;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox txtzdType;
    }
}