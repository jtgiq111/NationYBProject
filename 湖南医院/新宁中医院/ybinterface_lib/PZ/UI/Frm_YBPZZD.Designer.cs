namespace ybinterface_lib
{
    partial class Frm_YBPZZD
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Frm_YBPZZD));
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel2 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel3 = new System.Windows.Forms.ToolStripStatusLabel();
            this.tssl_czyxm = new System.Windows.Forms.ToolStripStatusLabel();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.dgvPZZD = new System.Windows.Forms.DataGridView();
            this.col_PZCSDM = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.col_PZCSMC = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.col_PZDQDM = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.col_PZDQMC = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.col_PZJKLB = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.col_PZDYBZ = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.col_PZDYMC = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.rabtnY_pz = new System.Windows.Forms.RadioButton();
            this.rabtnN_pz = new System.Windows.Forms.RadioButton();
            this.btnUPDATE = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.txtYBCS_PZ = new System.Windows.Forms.TextBox();
            this.btnPZSELECT = new System.Windows.Forms.Button();
            this.label21 = new System.Windows.Forms.Label();
            this.txtYBDQ_PZ = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.cmbJKLB = new System.Windows.Forms.ComboBox();
            this.alvCS = new AutoListView.AutoListView();
            this.alvDQ = new AutoListView.AutoListView();
            this.statusStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPZZD)).BeginInit();
            this.SuspendLayout();
            // 
            // statusStrip1
            // 
            this.statusStrip1.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1,
            this.toolStripStatusLabel2,
            this.toolStripStatusLabel3,
            this.tssl_czyxm});
            this.statusStrip1.Location = new System.Drawing.Point(0, 537);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Padding = new System.Windows.Forms.Padding(1, 0, 16, 0);
            this.statusStrip1.Size = new System.Drawing.Size(795, 25);
            this.statusStrip1.TabIndex = 0;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.ForeColor = System.Drawing.Color.Blue;
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(101, 20);
            this.toolStripStatusLabel1.Text = "  医保接口系统";
            // 
            // toolStripStatusLabel2
            // 
            this.toolStripStatusLabel2.Name = "toolStripStatusLabel2";
            this.toolStripStatusLabel2.Size = new System.Drawing.Size(13, 20);
            this.toolStripStatusLabel2.Text = "|";
            // 
            // toolStripStatusLabel3
            // 
            this.toolStripStatusLabel3.ForeColor = System.Drawing.Color.Blue;
            this.toolStripStatusLabel3.Name = "toolStripStatusLabel3";
            this.toolStripStatusLabel3.Size = new System.Drawing.Size(54, 20);
            this.toolStripStatusLabel3.Text = "操作员:";
            // 
            // tssl_czyxm
            // 
            this.tssl_czyxm.Name = "tssl_czyxm";
            this.tssl_czyxm.Size = new System.Drawing.Size(0, 20);
            // 
            // splitContainer1
            // 
            this.splitContainer1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.BackColor = System.Drawing.Color.White;
            this.splitContainer1.Panel1.Controls.Add(this.dgvPZZD);
            this.splitContainer1.Panel1MinSize = 100;
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.BackColor = System.Drawing.Color.White;
            this.splitContainer1.Panel2.Controls.Add(this.rabtnY_pz);
            this.splitContainer1.Panel2.Controls.Add(this.rabtnN_pz);
            this.splitContainer1.Panel2.Controls.Add(this.btnUPDATE);
            this.splitContainer1.Panel2.Controls.Add(this.label6);
            this.splitContainer1.Panel2.Controls.Add(this.button1);
            this.splitContainer1.Panel2.Controls.Add(this.txtYBCS_PZ);
            this.splitContainer1.Panel2.Controls.Add(this.btnPZSELECT);
            this.splitContainer1.Panel2.Controls.Add(this.label21);
            this.splitContainer1.Panel2.Controls.Add(this.txtYBDQ_PZ);
            this.splitContainer1.Panel2.Controls.Add(this.label7);
            this.splitContainer1.Panel2.Controls.Add(this.label5);
            this.splitContainer1.Panel2.Controls.Add(this.cmbJKLB);
            this.splitContainer1.Panel2MinSize = 110;
            this.splitContainer1.Size = new System.Drawing.Size(795, 537);
            this.splitContainer1.SplitterDistance = 361;
            this.splitContainer1.SplitterWidth = 5;
            this.splitContainer1.TabIndex = 1;
            // 
            // dgvPZZD
            // 
            this.dgvPZZD.AllowUserToAddRows = false;
            this.dgvPZZD.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvPZZD.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.col_PZCSDM,
            this.col_PZCSMC,
            this.col_PZDQDM,
            this.col_PZDQMC,
            this.col_PZJKLB,
            this.col_PZDYBZ,
            this.col_PZDYMC});
            this.dgvPZZD.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvPZZD.Location = new System.Drawing.Point(0, 0);
            this.dgvPZZD.Name = "dgvPZZD";
            this.dgvPZZD.RowTemplate.Height = 23;
            this.dgvPZZD.Size = new System.Drawing.Size(793, 359);
            this.dgvPZZD.TabIndex = 5;
            this.dgvPZZD.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvPZZD_CellDoubleClick);
            // 
            // col_PZCSDM
            // 
            this.col_PZCSDM.DataPropertyName = "CSDM";
            this.col_PZCSDM.HeaderText = "厂商代码";
            this.col_PZCSDM.Name = "col_PZCSDM";
            this.col_PZCSDM.ReadOnly = true;
            // 
            // col_PZCSMC
            // 
            this.col_PZCSMC.DataPropertyName = "CSMC";
            this.col_PZCSMC.HeaderText = "厂商名称";
            this.col_PZCSMC.Name = "col_PZCSMC";
            this.col_PZCSMC.ReadOnly = true;
            this.col_PZCSMC.Width = 120;
            // 
            // col_PZDQDM
            // 
            this.col_PZDQDM.DataPropertyName = "DQDM";
            this.col_PZDQDM.HeaderText = "地区代码";
            this.col_PZDQDM.Name = "col_PZDQDM";
            this.col_PZDQDM.ReadOnly = true;
            this.col_PZDQDM.Width = 120;
            // 
            // col_PZDQMC
            // 
            this.col_PZDQMC.DataPropertyName = "DQMC";
            this.col_PZDQMC.HeaderText = "地区名称";
            this.col_PZDQMC.Name = "col_PZDQMC";
            this.col_PZDQMC.ReadOnly = true;
            this.col_PZDQMC.Width = 150;
            // 
            // col_PZJKLB
            // 
            this.col_PZJKLB.DataPropertyName = "JKLB";
            this.col_PZJKLB.HeaderText = "接口类别";
            this.col_PZJKLB.Name = "col_PZJKLB";
            this.col_PZJKLB.ReadOnly = true;
            // 
            // col_PZDYBZ
            // 
            this.col_PZDYBZ.DataPropertyName = "QYBZ";
            this.col_PZDYBZ.HeaderText = "QYBZ";
            this.col_PZDYBZ.Name = "col_PZDYBZ";
            this.col_PZDYBZ.ReadOnly = true;
            this.col_PZDYBZ.Visible = false;
            // 
            // col_PZDYMC
            // 
            this.col_PZDYMC.DataPropertyName = "QYBZMC";
            this.col_PZDYMC.HeaderText = "是否启用";
            this.col_PZDYMC.Name = "col_PZDYMC";
            this.col_PZDYMC.ReadOnly = true;
            // 
            // rabtnY_pz
            // 
            this.rabtnY_pz.AutoSize = true;
            this.rabtnY_pz.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.rabtnY_pz.Location = new System.Drawing.Point(427, 62);
            this.rabtnY_pz.Name = "rabtnY_pz";
            this.rabtnY_pz.Size = new System.Drawing.Size(39, 18);
            this.rabtnY_pz.TabIndex = 35;
            this.rabtnY_pz.TabStop = true;
            this.rabtnY_pz.Text = "是";
            this.rabtnY_pz.UseVisualStyleBackColor = true;
            // 
            // rabtnN_pz
            // 
            this.rabtnN_pz.AutoSize = true;
            this.rabtnN_pz.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.rabtnN_pz.Location = new System.Drawing.Point(498, 62);
            this.rabtnN_pz.Name = "rabtnN_pz";
            this.rabtnN_pz.Size = new System.Drawing.Size(39, 18);
            this.rabtnN_pz.TabIndex = 36;
            this.rabtnN_pz.TabStop = true;
            this.rabtnN_pz.Text = "否";
            this.rabtnN_pz.UseVisualStyleBackColor = true;
            // 
            // btnUPDATE
            // 
            this.btnUPDATE.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnUPDATE.ForeColor = System.Drawing.Color.Blue;
            this.btnUPDATE.Location = new System.Drawing.Point(331, 103);
            this.btnUPDATE.Name = "btnUPDATE";
            this.btnUPDATE.Size = new System.Drawing.Size(87, 31);
            this.btnUPDATE.TabIndex = 32;
            this.btnUPDATE.Text = "修  改";
            this.btnUPDATE.UseVisualStyleBackColor = true;
            this.btnUPDATE.Click += new System.EventHandler(this.btnUPDATE_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label6.Location = new System.Drawing.Point(16, 20);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(77, 14);
            this.label6.TabIndex = 7;
            this.label6.Text = "医保厂商：";
            // 
            // button1
            // 
            this.button1.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button1.ForeColor = System.Drawing.Color.Blue;
            this.button1.Location = new System.Drawing.Point(221, 103);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(87, 31);
            this.button1.TabIndex = 31;
            this.button1.Text = "保  存";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // txtYBCS_PZ
            // 
            this.txtYBCS_PZ.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtYBCS_PZ.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txtYBCS_PZ.Location = new System.Drawing.Point(107, 15);
            this.txtYBCS_PZ.Name = "txtYBCS_PZ";
            this.txtYBCS_PZ.Size = new System.Drawing.Size(189, 23);
            this.txtYBCS_PZ.TabIndex = 6;
            // 
            // btnPZSELECT
            // 
            this.btnPZSELECT.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnPZSELECT.ForeColor = System.Drawing.Color.Blue;
            this.btnPZSELECT.Location = new System.Drawing.Point(109, 103);
            this.btnPZSELECT.Name = "btnPZSELECT";
            this.btnPZSELECT.Size = new System.Drawing.Size(87, 31);
            this.btnPZSELECT.TabIndex = 30;
            this.btnPZSELECT.Text = "查  询";
            this.btnPZSELECT.UseVisualStyleBackColor = true;
            this.btnPZSELECT.Click += new System.EventHandler(this.btnPZSELECT_Click);
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label21.Location = new System.Drawing.Point(328, 20);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(77, 14);
            this.label21.TabIndex = 24;
            this.label21.Text = "医保地区：";
            // 
            // txtYBDQ_PZ
            // 
            this.txtYBDQ_PZ.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtYBDQ_PZ.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txtYBDQ_PZ.Location = new System.Drawing.Point(418, 15);
            this.txtYBDQ_PZ.Name = "txtYBDQ_PZ";
            this.txtYBDQ_PZ.Size = new System.Drawing.Size(190, 23);
            this.txtYBDQ_PZ.TabIndex = 23;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label7.Location = new System.Drawing.Point(16, 64);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(77, 14);
            this.label7.TabIndex = 25;
            this.label7.Text = "接口类别：";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label5.Location = new System.Drawing.Point(323, 64);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(77, 14);
            this.label5.TabIndex = 27;
            this.label5.Text = "是否启用：";
            // 
            // cmbJKLB
            // 
            this.cmbJKLB.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbJKLB.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cmbJKLB.FormattingEnabled = true;
            this.cmbJKLB.Items.AddRange(new object[] {
            "医保",
            "农合",
            "扶贫"});
            this.cmbJKLB.Location = new System.Drawing.Point(107, 59);
            this.cmbJKLB.Name = "cmbJKLB";
            this.cmbJKLB.Size = new System.Drawing.Size(188, 22);
            this.cmbJKLB.TabIndex = 26;
            // 
            // alvCS
            // 
            this.alvCS.AttachTextBox = this.txtYBCS_PZ;
            this.alvCS.ChooseInfoDataSet = null;
            this.alvCS.DataViewFont = null;
            this.alvCS.DataViewFontColor = System.Drawing.Color.Navy;
            this.alvCS.DataViewMaxSize = new System.Drawing.Size(240, 150);
            this.alvCS.DeleteTextWhenNull = true;
            this.alvCS.Enable = true;
            this.alvCS.FilterString = "";
            this.alvCS.IsGetAllRecord = true;
            this.alvCS.NextFocusControl = this.txtYBDQ_PZ;
            this.alvCS.ParentControl = this;
            this.alvCS.RelationEnable = true;
            this.alvCS.Relations = null;
            this.alvCS.Remark = null;
            this.alvCS.SetColumnParam.QueryColumn = "";
            this.alvCS.SetColumnParam.TextColumn = "";
            this.alvCS.SetColumnParam.ValueColumn = "";
            this.alvCS.SetColumnParam.ViewColumn = "";
            this.alvCS.TargetColumn = 0;
            this.alvCS.UpdateSourceWhenEnter = false;
            this.alvCS.UseMulThread = false;
            this.alvCS.WhereStr = null;
            this.alvCS.AfterConfirm += new AutoListView.AutoListView.AfterConfirmHandle(this.alvCS_AfterConfirm);
            // 
            // alvDQ
            // 
            this.alvDQ.AttachTextBox = this.txtYBDQ_PZ;
            this.alvDQ.ChooseInfoDataSet = null;
            this.alvDQ.DataViewFont = null;
            this.alvDQ.DataViewFontColor = System.Drawing.Color.Navy;
            this.alvDQ.DataViewMaxSize = new System.Drawing.Size(240, 150);
            this.alvDQ.DeleteTextWhenNull = true;
            this.alvDQ.Enable = true;
            this.alvDQ.FilterString = "";
            this.alvDQ.IsGetAllRecord = true;
            this.alvDQ.ParentControl = this;
            this.alvDQ.RelationEnable = true;
            this.alvDQ.Relations = null;
            this.alvDQ.Remark = null;
            this.alvDQ.SetColumnParam.QueryColumn = "";
            this.alvDQ.SetColumnParam.TextColumn = "";
            this.alvDQ.SetColumnParam.ValueColumn = "";
            this.alvDQ.SetColumnParam.ViewColumn = "";
            this.alvDQ.TargetColumn = 0;
            this.alvDQ.UpdateSourceWhenEnter = false;
            this.alvDQ.UseMulThread = false;
            this.alvDQ.WhereStr = null;
            this.alvDQ.AfterConfirm += new AutoListView.AutoListView.AfterConfirmHandle(this.alvDQ_AfterConfirm);
            // 
            // Frm_YBPZZD
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(795, 562);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.statusStrip1);
            this.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Frm_YBPZZD";
            this.Tag = "ybinterface_lib.Frm_YBPZZD";
            this.Text = "医保启用设置";
            this.Load += new System.EventHandler(this.Frm_YBPZZD_Load);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvPZZD)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel2;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel3;
        private System.Windows.Forms.ToolStripStatusLabel tssl_czyxm;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TextBox txtYBCS_PZ;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtYBDQ_PZ;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.ComboBox cmbJKLB;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button btnUPDATE;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button btnPZSELECT;
        private AutoListView.AutoListView alvCS;
        private AutoListView.AutoListView alvDQ;
        private System.Windows.Forms.DataGridView dgvPZZD;
        private System.Windows.Forms.DataGridViewTextBoxColumn col_PZCSDM;
        private System.Windows.Forms.DataGridViewTextBoxColumn col_PZCSMC;
        private System.Windows.Forms.DataGridViewTextBoxColumn col_PZDQDM;
        private System.Windows.Forms.DataGridViewTextBoxColumn col_PZDQMC;
        private System.Windows.Forms.DataGridViewTextBoxColumn col_PZJKLB;
        private System.Windows.Forms.DataGridViewTextBoxColumn col_PZDYBZ;
        private System.Windows.Forms.DataGridViewTextBoxColumn col_PZDYMC;
        private System.Windows.Forms.RadioButton rabtnY_pz;
        private System.Windows.Forms.RadioButton rabtnN_pz;


    }
}