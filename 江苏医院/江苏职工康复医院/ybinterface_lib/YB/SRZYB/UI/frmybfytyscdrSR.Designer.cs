namespace ybinterface_lib
{
    partial class frmybfytyscdrSR
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
            this.infoDataGridView1 = new Srvtools.InfoDataGridView();
            this.zyno = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.bxh = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.z1hznm = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ksno = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ksnm = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Tfee = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Nfee = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.amnt1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.amnt3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.xm = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.zyh = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.kh = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.z3kdys = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.btn_uploadonefee = new System.Windows.Forms.Button();
            this.btn_close = new System.Windows.Forms.Button();
            this.btn_uploadallfee = new System.Windows.Forms.Button();
            this.btn_sjhz = new System.Windows.Forms.Button();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.infoDataGridView2 = new Srvtools.InfoDataGridView();
            this.yyxmdm = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.yyxmmc = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.je = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.zlje = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.zfje = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cxjzfje = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.sflb = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.sfxmdj = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.qezfbz = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.zlbl = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.xj = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.bz = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.infoDataGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.infoDataGridView2)).BeginInit();
            this.SuspendLayout();
            // 
            // infoDataGridView1
            // 
            this.infoDataGridView1.AllowUserToAddRows = false;
            this.infoDataGridView1.AllowUserToDeleteRows = false;
            this.infoDataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.infoDataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.zyno,
            this.bxh,
            this.z1hznm,
            this.ksno,
            this.ksnm,
            this.Tfee,
            this.Nfee,
            this.amnt1,
            this.amnt3,
            this.xm,
            this.zyh,
            this.kh,
            this.z3kdys});
            this.infoDataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.infoDataGridView1.EnterEnable = true;
            this.infoDataGridView1.EnterRefValControl = false;
            this.infoDataGridView1.Location = new System.Drawing.Point(3, 20);
            this.infoDataGridView1.Name = "infoDataGridView1";
            this.infoDataGridView1.ReadOnly = true;
            this.infoDataGridView1.RowTemplate.Height = 23;
            this.infoDataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.infoDataGridView1.Size = new System.Drawing.Size(556, 421);
            this.infoDataGridView1.SureDelete = false;
            this.infoDataGridView1.TabIndex = 0;
            this.infoDataGridView1.TotalActive = true;
            this.infoDataGridView1.TotalBackColor = System.Drawing.SystemColors.Info;
            this.infoDataGridView1.TotalCaption = null;
            this.infoDataGridView1.TotalCaptionFont = new System.Drawing.Font("宋体", 9F);
            this.infoDataGridView1.TotalFont = new System.Drawing.Font("宋体", 9F);
            this.infoDataGridView1.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.infoDataGridView1_CellClick);
            // 
            // zyno
            // 
            this.zyno.DataPropertyName = "z1zyno";
            this.zyno.HeaderText = "住院号";
            this.zyno.Name = "zyno";
            this.zyno.ReadOnly = true;
            // 
            // bxh
            // 
            this.bxh.DataPropertyName = "bxh";
            this.bxh.HeaderText = "医疗保险号";
            this.bxh.Name = "bxh";
            this.bxh.ReadOnly = true;
            this.bxh.Width = 110;
            // 
            // z1hznm
            // 
            this.z1hznm.DataPropertyName = "z1hznm";
            this.z1hznm.HeaderText = "姓名";
            this.z1hznm.Name = "z1hznm";
            this.z1hznm.ReadOnly = true;
            // 
            // ksno
            // 
            this.ksno.DataPropertyName = "z1ksno";
            this.ksno.HeaderText = "科室编号";
            this.ksno.Name = "ksno";
            this.ksno.ReadOnly = true;
            this.ksno.Visible = false;
            // 
            // ksnm
            // 
            this.ksnm.DataPropertyName = "z1ksnm";
            this.ksnm.HeaderText = "科室名称";
            this.ksnm.Name = "ksnm";
            this.ksnm.ReadOnly = true;
            // 
            // Tfee
            // 
            this.Tfee.DataPropertyName = "z1amt2";
            this.Tfee.HeaderText = "费用总额";
            this.Tfee.Name = "Tfee";
            this.Tfee.ReadOnly = true;
            // 
            // Nfee
            // 
            this.Nfee.DataPropertyName = "amt2";
            this.Nfee.HeaderText = "未传费用";
            this.Nfee.Name = "Nfee";
            this.Nfee.ReadOnly = true;
            // 
            // amnt1
            // 
            this.amnt1.DataPropertyName = "z1amt1";
            this.amnt1.HeaderText = "预交款额";
            this.amnt1.Name = "amnt1";
            this.amnt1.ReadOnly = true;
            // 
            // amnt3
            // 
            this.amnt3.DataPropertyName = "z1amt3";
            this.amnt3.HeaderText = "担保额";
            this.amnt3.Name = "amnt3";
            this.amnt3.ReadOnly = true;
            // 
            // xm
            // 
            this.xm.DataPropertyName = "xm";
            this.xm.HeaderText = "姓名";
            this.xm.Name = "xm";
            this.xm.ReadOnly = true;
            this.xm.Visible = false;
            // 
            // zyh
            // 
            this.zyh.DataPropertyName = "jylsh";
            this.zyh.HeaderText = "医保流水号";
            this.zyh.Name = "zyh";
            this.zyh.ReadOnly = true;
            this.zyh.Width = 110;
            // 
            // kh
            // 
            this.kh.DataPropertyName = "kh";
            this.kh.HeaderText = "IC卡号";
            this.kh.Name = "kh";
            this.kh.ReadOnly = true;
            // 
            // z3kdys
            // 
            this.z3kdys.DataPropertyName = "z3kdys";
            this.z3kdys.HeaderText = "开单医生";
            this.z3kdys.Name = "z3kdys";
            this.z3kdys.ReadOnly = true;
            this.z3kdys.Visible = false;
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
            this.splitContainer1.Panel1.Controls.Add(this.btn_uploadonefee);
            this.splitContainer1.Panel1.Controls.Add(this.btn_close);
            this.splitContainer1.Panel1.Controls.Add(this.btn_uploadallfee);
            this.splitContainer1.Panel1.Controls.Add(this.btn_sjhz);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
            this.splitContainer1.Size = new System.Drawing.Size(970, 495);
            this.splitContainer1.SplitterDistance = 47;
            this.splitContainer1.TabIndex = 1;
            // 
            // btn_uploadonefee
            // 
            this.btn_uploadonefee.Location = new System.Drawing.Point(194, 9);
            this.btn_uploadonefee.Name = "btn_uploadonefee";
            this.btn_uploadonefee.Size = new System.Drawing.Size(189, 27);
            this.btn_uploadonefee.TabIndex = 1;
            this.btn_uploadonefee.Text = "上传单个病人费用(2310)";
            this.btn_uploadonefee.UseVisualStyleBackColor = true;
            this.btn_uploadonefee.Click += new System.EventHandler(this.btn_uploadonefee_Click);
            // 
            // btn_close
            // 
            this.btn_close.Location = new System.Drawing.Point(684, 9);
            this.btn_close.Name = "btn_close";
            this.btn_close.Size = new System.Drawing.Size(75, 27);
            this.btn_close.TabIndex = 0;
            this.btn_close.Text = "关闭";
            this.btn_close.UseVisualStyleBackColor = true;
            this.btn_close.Click += new System.EventHandler(this.btn_close_Click);
            // 
            // btn_uploadallfee
            // 
            this.btn_uploadallfee.Location = new System.Drawing.Point(447, 9);
            this.btn_uploadallfee.Name = "btn_uploadallfee";
            this.btn_uploadallfee.Size = new System.Drawing.Size(196, 27);
            this.btn_uploadallfee.TabIndex = 0;
            this.btn_uploadallfee.Text = "上传所有病人费用(2310)";
            this.btn_uploadallfee.UseVisualStyleBackColor = true;
            this.btn_uploadallfee.Click += new System.EventHandler(this.btn_uploadallfee_Click);
            // 
            // btn_sjhz
            // 
            this.btn_sjhz.Location = new System.Drawing.Point(27, 9);
            this.btn_sjhz.Name = "btn_sjhz";
            this.btn_sjhz.Size = new System.Drawing.Size(83, 27);
            this.btn_sjhz.TabIndex = 0;
            this.btn_sjhz.Text = "收集患者";
            this.btn_sjhz.UseVisualStyleBackColor = true;
            this.btn_sjhz.Click += new System.EventHandler(this.btn_sjhz_Click);
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.groupBox2);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.groupBox1);
            this.splitContainer2.Size = new System.Drawing.Size(970, 444);
            this.splitContainer2.SplitterDistance = 562;
            this.splitContainer2.TabIndex = 1;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.infoDataGridView1);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox2.Location = new System.Drawing.Point(0, 0);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(562, 444);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "待上传患者";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.infoDataGridView2);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(404, 444);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "已上传费用";
            // 
            // infoDataGridView2
            // 
            this.infoDataGridView2.AllowUserToAddRows = false;
            this.infoDataGridView2.AllowUserToDeleteRows = false;
            this.infoDataGridView2.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.infoDataGridView2.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.yyxmdm,
            this.yyxmmc,
            this.je,
            this.zlje,
            this.zfje,
            this.cxjzfje,
            this.sflb,
            this.sfxmdj,
            this.qezfbz,
            this.zlbl,
            this.xj,
            this.bz});
            this.infoDataGridView2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.infoDataGridView2.EnterEnable = true;
            this.infoDataGridView2.EnterRefValControl = false;
            this.infoDataGridView2.Location = new System.Drawing.Point(3, 20);
            this.infoDataGridView2.Name = "infoDataGridView2";
            this.infoDataGridView2.ReadOnly = true;
            this.infoDataGridView2.RowTemplate.Height = 23;
            this.infoDataGridView2.Size = new System.Drawing.Size(398, 421);
            this.infoDataGridView2.SureDelete = false;
            this.infoDataGridView2.TabIndex = 0;
            this.infoDataGridView2.TotalActive = true;
            this.infoDataGridView2.TotalBackColor = System.Drawing.SystemColors.Info;
            this.infoDataGridView2.TotalCaption = null;
            this.infoDataGridView2.TotalCaptionFont = new System.Drawing.Font("宋体", 9F);
            this.infoDataGridView2.TotalFont = new System.Drawing.Font("宋体", 9F);
            // 
            // yyxmdm
            // 
            this.yyxmdm.DataPropertyName = "yyxmdm";
            this.yyxmdm.HeaderText = "医院项目代码";
            this.yyxmdm.Name = "yyxmdm";
            this.yyxmdm.ReadOnly = true;
            this.yyxmdm.Width = 130;
            // 
            // yyxmmc
            // 
            this.yyxmmc.DataPropertyName = "yyxmmc";
            this.yyxmmc.HeaderText = "医院项目名称";
            this.yyxmmc.Name = "yyxmmc";
            this.yyxmmc.ReadOnly = true;
            this.yyxmmc.Width = 130;
            // 
            // je
            // 
            this.je.DataPropertyName = "je";
            this.je.HeaderText = "金额";
            this.je.Name = "je";
            this.je.ReadOnly = true;
            // 
            // zlje
            // 
            this.zlje.DataPropertyName = "zlje";
            this.zlje.HeaderText = "处理金额";
            this.zlje.Name = "zlje";
            this.zlje.ReadOnly = true;
            // 
            // zfje
            // 
            this.zfje.DataPropertyName = "zfje";
            this.zfje.HeaderText = "自费金额";
            this.zfje.Name = "zfje";
            this.zfje.ReadOnly = true;
            // 
            // cxjzfje
            // 
            this.cxjzfje.DataPropertyName = "cxjzfje";
            this.cxjzfje.HeaderText = "超限价自付金额";
            this.cxjzfje.Name = "cxjzfje";
            this.cxjzfje.ReadOnly = true;
            // 
            // sflb
            // 
            this.sflb.DataPropertyName = "sflb";
            this.sflb.HeaderText = "收费类别";
            this.sflb.Name = "sflb";
            this.sflb.ReadOnly = true;
            // 
            // sfxmdj
            // 
            this.sfxmdj.DataPropertyName = "sfxmdj";
            this.sfxmdj.HeaderText = "收费项目等级";
            this.sfxmdj.Name = "sfxmdj";
            this.sfxmdj.ReadOnly = true;
            // 
            // qezfbz
            // 
            this.qezfbz.DataPropertyName = "qezfbz";
            this.qezfbz.HeaderText = "全额自费标志";
            this.qezfbz.Name = "qezfbz";
            this.qezfbz.ReadOnly = true;
            // 
            // zlbl
            // 
            this.zlbl.DataPropertyName = "zlbl";
            this.zlbl.HeaderText = "自理比例";
            this.zlbl.Name = "zlbl";
            this.zlbl.ReadOnly = true;
            // 
            // xj
            // 
            this.xj.DataPropertyName = "xj";
            this.xj.HeaderText = "限价";
            this.xj.Name = "xj";
            this.xj.ReadOnly = true;
            // 
            // bz
            // 
            this.bz.DataPropertyName = "bz";
            this.bz.HeaderText = "备注";
            this.bz.Name = "bz";
            this.bz.ReadOnly = true;
            // 
            // frmybfytyscdrSR
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(970, 495);
            this.Controls.Add(this.splitContainer1);
            this.Font = new System.Drawing.Font("宋体", 11F);
            this.Name = "frmybfytyscdrSR";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "医保费用统一上传（今天以前所有费用）";
            ((System.ComponentModel.ISupportInitialize)(this.infoDataGridView1)).EndInit();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.infoDataGridView2)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Srvtools.InfoDataGridView infoDataGridView1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Button btn_sjhz;
        private System.Windows.Forms.Button btn_close;
        private System.Windows.Forms.Button btn_uploadallfee;
        private System.Windows.Forms.DataGridViewTextBoxColumn zyno;
        private System.Windows.Forms.DataGridViewTextBoxColumn bxh;
        private System.Windows.Forms.DataGridViewTextBoxColumn z1hznm;
        private System.Windows.Forms.DataGridViewTextBoxColumn ksno;
        private System.Windows.Forms.DataGridViewTextBoxColumn ksnm;
        private System.Windows.Forms.DataGridViewTextBoxColumn Tfee;
        private System.Windows.Forms.DataGridViewTextBoxColumn Nfee;
        private System.Windows.Forms.DataGridViewTextBoxColumn amnt1;
        private System.Windows.Forms.DataGridViewTextBoxColumn amnt3;
        private System.Windows.Forms.DataGridViewTextBoxColumn xm;
        private System.Windows.Forms.DataGridViewTextBoxColumn zyh;
        private System.Windows.Forms.DataGridViewTextBoxColumn kh;
        private System.Windows.Forms.DataGridViewTextBoxColumn z3kdys;
        private System.Windows.Forms.Button btn_uploadonefee;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private Srvtools.InfoDataGridView infoDataGridView2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.DataGridViewTextBoxColumn yyxmdm;
        private System.Windows.Forms.DataGridViewTextBoxColumn yyxmmc;
        private System.Windows.Forms.DataGridViewTextBoxColumn je;
        private System.Windows.Forms.DataGridViewTextBoxColumn zlje;
        private System.Windows.Forms.DataGridViewTextBoxColumn zfje;
        private System.Windows.Forms.DataGridViewTextBoxColumn cxjzfje;
        private System.Windows.Forms.DataGridViewTextBoxColumn sflb;
        private System.Windows.Forms.DataGridViewTextBoxColumn sfxmdj;
        private System.Windows.Forms.DataGridViewTextBoxColumn qezfbz;
        private System.Windows.Forms.DataGridViewTextBoxColumn zlbl;
        private System.Windows.Forms.DataGridViewTextBoxColumn xj;
        private System.Windows.Forms.DataGridViewTextBoxColumn bz;
    }
}