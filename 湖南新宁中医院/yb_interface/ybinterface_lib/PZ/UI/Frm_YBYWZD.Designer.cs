namespace ybinterface_lib
{
    partial class Frm_YBYWZD
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Frm_YBYWZD));
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel2 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel3 = new System.Windows.Forms.ToolStripStatusLabel();
            this.tssl_czyxm = new System.Windows.Forms.ToolStripStatusLabel();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.dgvYWZD = new System.Windows.Forms.DataGridView();
            this.col_ywdm = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.col_ywmc = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.col_qybz = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.col_qybzmc = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.col_ffmc = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnYWSELECT = new System.Windows.Forms.Button();
            this.txtINDEX = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.txtFFMC = new System.Windows.Forms.TextBox();
            this.label25 = new System.Windows.Forms.Label();
            this.rabtnN = new System.Windows.Forms.RadioButton();
            this.rabtnY = new System.Windows.Forms.RadioButton();
            this.btnYWADD = new System.Windows.Forms.Button();
            this.btnYWSAVE = new System.Windows.Forms.Button();
            this.label8 = new System.Windows.Forms.Label();
            this.txtYWMC = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtYWDM = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.alvCS = new AutoListView.AutoListView();
            this.alvDQ = new AutoListView.AutoListView();
            this.statusStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvYWZD)).BeginInit();
            this.panel1.SuspendLayout();
            this.groupBox1.SuspendLayout();
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
            this.statusStrip1.Size = new System.Drawing.Size(821, 25);
            this.statusStrip1.TabIndex = 1;
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
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.BackColor = System.Drawing.Color.White;
            this.splitContainer1.Panel1.Controls.Add(this.dgvYWZD);
            this.splitContainer1.Panel1.Controls.Add(this.panel1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.BackColor = System.Drawing.Color.White;
            this.splitContainer1.Panel2.Controls.Add(this.groupBox1);
            this.splitContainer1.Size = new System.Drawing.Size(821, 537);
            this.splitContainer1.SplitterDistance = 360;
            this.splitContainer1.SplitterWidth = 5;
            this.splitContainer1.TabIndex = 2;
            // 
            // dgvYWZD
            // 
            this.dgvYWZD.AllowUserToAddRows = false;
            this.dgvYWZD.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvYWZD.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.col_ywdm,
            this.col_ywmc,
            this.col_qybz,
            this.col_qybzmc,
            this.col_ffmc});
            this.dgvYWZD.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvYWZD.Location = new System.Drawing.Point(0, 36);
            this.dgvYWZD.Name = "dgvYWZD";
            this.dgvYWZD.RowTemplate.Height = 23;
            this.dgvYWZD.Size = new System.Drawing.Size(821, 324);
            this.dgvYWZD.TabIndex = 1;
            // 
            // col_ywdm
            // 
            this.col_ywdm.DataPropertyName = "YWDM";
            this.col_ywdm.HeaderText = "业务代码";
            this.col_ywdm.Name = "col_ywdm";
            this.col_ywdm.ReadOnly = true;
            // 
            // col_ywmc
            // 
            this.col_ywmc.DataPropertyName = "YWMC";
            this.col_ywmc.HeaderText = "业务名称";
            this.col_ywmc.Name = "col_ywmc";
            this.col_ywmc.ReadOnly = true;
            this.col_ywmc.Width = 150;
            // 
            // col_qybz
            // 
            this.col_qybz.DataPropertyName = "QYBZ";
            this.col_qybz.HeaderText = "QYBZ";
            this.col_qybz.Name = "col_qybz";
            this.col_qybz.ReadOnly = true;
            this.col_qybz.Visible = false;
            // 
            // col_qybzmc
            // 
            this.col_qybzmc.DataPropertyName = "QYBZMC";
            this.col_qybzmc.HeaderText = "是否启用";
            this.col_qybzmc.Name = "col_qybzmc";
            this.col_qybzmc.ReadOnly = true;
            // 
            // col_ffmc
            // 
            this.col_ffmc.DataPropertyName = "ffmc";
            this.col_ffmc.HeaderText = "方法名称";
            this.col_ffmc.Name = "col_ffmc";
            this.col_ffmc.ReadOnly = true;
            this.col_ffmc.Width = 130;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.btnYWSELECT);
            this.panel1.Controls.Add(this.txtINDEX);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(821, 36);
            this.panel1.TabIndex = 0;
            // 
            // btnYWSELECT
            // 
            this.btnYWSELECT.ForeColor = System.Drawing.Color.Blue;
            this.btnYWSELECT.Location = new System.Drawing.Point(330, 4);
            this.btnYWSELECT.Name = "btnYWSELECT";
            this.btnYWSELECT.Size = new System.Drawing.Size(87, 27);
            this.btnYWSELECT.TabIndex = 31;
            this.btnYWSELECT.Text = "查  询";
            this.btnYWSELECT.UseVisualStyleBackColor = true;
            this.btnYWSELECT.Click += new System.EventHandler(this.btnYWSELECT_Click);
            // 
            // txtINDEX
            // 
            this.txtINDEX.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtINDEX.Location = new System.Drawing.Point(128, 6);
            this.txtINDEX.Name = "txtINDEX";
            this.txtINDEX.Size = new System.Drawing.Size(187, 23);
            this.txtINDEX.TabIndex = 1;
            this.txtINDEX.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtINDEX_KeyDown);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.Color.Blue;
            this.label1.Location = new System.Drawing.Point(14, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(112, 14);
            this.label1.TabIndex = 0;
            this.label1.Text = "业务代码/名称：";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.txtFFMC);
            this.groupBox1.Controls.Add(this.label25);
            this.groupBox1.Controls.Add(this.rabtnN);
            this.groupBox1.Controls.Add(this.rabtnY);
            this.groupBox1.Controls.Add(this.btnYWADD);
            this.groupBox1.Controls.Add(this.btnYWSAVE);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.txtYWMC);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.txtYWDM);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Location = new System.Drawing.Point(6, 7);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(798, 161);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "业务配置";
            // 
            // txtFFMC
            // 
            this.txtFFMC.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtFFMC.Location = new System.Drawing.Point(370, 79);
            this.txtFFMC.Name = "txtFFMC";
            this.txtFFMC.Size = new System.Drawing.Size(189, 23);
            this.txtFFMC.TabIndex = 21;
            // 
            // label25
            // 
            this.label25.AutoSize = true;
            this.label25.Location = new System.Drawing.Point(289, 82);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(77, 14);
            this.label25.TabIndex = 22;
            this.label25.Text = "方法名称：";
            // 
            // rabtnN
            // 
            this.rabtnN.AutoSize = true;
            this.rabtnN.Location = new System.Drawing.Point(154, 79);
            this.rabtnN.Name = "rabtnN";
            this.rabtnN.Size = new System.Drawing.Size(39, 18);
            this.rabtnN.TabIndex = 20;
            this.rabtnN.TabStop = true;
            this.rabtnN.Text = "否";
            this.rabtnN.UseVisualStyleBackColor = true;
            // 
            // rabtnY
            // 
            this.rabtnY.AutoSize = true;
            this.rabtnY.Location = new System.Drawing.Point(91, 79);
            this.rabtnY.Name = "rabtnY";
            this.rabtnY.Size = new System.Drawing.Size(39, 18);
            this.rabtnY.TabIndex = 19;
            this.rabtnY.TabStop = true;
            this.rabtnY.Text = "是";
            this.rabtnY.UseVisualStyleBackColor = true;
            // 
            // btnYWADD
            // 
            this.btnYWADD.ForeColor = System.Drawing.Color.Blue;
            this.btnYWADD.Location = new System.Drawing.Point(479, 119);
            this.btnYWADD.Name = "btnYWADD";
            this.btnYWADD.Size = new System.Drawing.Size(80, 27);
            this.btnYWADD.TabIndex = 16;
            this.btnYWADD.Text = "新  增";
            this.btnYWADD.UseVisualStyleBackColor = true;
            this.btnYWADD.Click += new System.EventHandler(this.btnYWADD_Click);
            // 
            // btnYWSAVE
            // 
            this.btnYWSAVE.ForeColor = System.Drawing.Color.Blue;
            this.btnYWSAVE.Location = new System.Drawing.Point(375, 119);
            this.btnYWSAVE.Name = "btnYWSAVE";
            this.btnYWSAVE.Size = new System.Drawing.Size(80, 27);
            this.btnYWSAVE.TabIndex = 3;
            this.btnYWSAVE.Text = "修  改";
            this.btnYWSAVE.UseVisualStyleBackColor = true;
            this.btnYWSAVE.Click += new System.EventHandler(this.btnYWSAVE_Click);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(8, 82);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(77, 14);
            this.label8.TabIndex = 13;
            this.label8.Text = "是否启用：";
            // 
            // txtYWMC
            // 
            this.txtYWMC.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtYWMC.Location = new System.Drawing.Point(370, 34);
            this.txtYWMC.Name = "txtYWMC";
            this.txtYWMC.Size = new System.Drawing.Size(189, 23);
            this.txtYWMC.TabIndex = 4;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(289, 38);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(77, 14);
            this.label4.TabIndex = 5;
            this.label4.Text = "业务名称：";
            // 
            // txtYWDM
            // 
            this.txtYWDM.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtYWDM.Location = new System.Drawing.Point(90, 34);
            this.txtYWDM.Name = "txtYWDM";
            this.txtYWDM.Size = new System.Drawing.Size(189, 23);
            this.txtYWDM.TabIndex = 3;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(9, 38);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(77, 14);
            this.label3.TabIndex = 3;
            this.label3.Text = "业务代码：";
            // 
            // alvCS
            // 
            this.alvCS.ChooseInfoDataSet = null;
            this.alvCS.DataViewFont = null;
            this.alvCS.DataViewFontColor = System.Drawing.Color.Navy;
            this.alvCS.DataViewMaxSize = new System.Drawing.Size(240, 150);
            this.alvCS.DeleteTextWhenNull = true;
            this.alvCS.Enable = true;
            this.alvCS.FilterString = "";
            this.alvCS.IsGetAllRecord = true;
            this.alvCS.ParentControl = null;
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
            // 
            // alvDQ
            // 
            this.alvDQ.ChooseInfoDataSet = null;
            this.alvDQ.DataViewFont = null;
            this.alvDQ.DataViewFontColor = System.Drawing.Color.Navy;
            this.alvDQ.DataViewMaxSize = new System.Drawing.Size(240, 150);
            this.alvDQ.DeleteTextWhenNull = true;
            this.alvDQ.Enable = true;
            this.alvDQ.FilterString = "";
            this.alvDQ.IsGetAllRecord = true;
            this.alvDQ.ParentControl = null;
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
            // 
            // Frm_YBYWZD
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(821, 562);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.statusStrip1);
            this.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Frm_YBYWZD";
            this.Tag = "ybinterface_lib.Frm_YBYWZD";
            this.Text = "医保业务字典设置";
            this.Load += new System.EventHandler(this.Frm_YBYWZD_Load);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvYWZD)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
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
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TextBox txtINDEX;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnYWSELECT;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox txtFFMC;
        private System.Windows.Forms.Label label25;
        private System.Windows.Forms.RadioButton rabtnN;
        private System.Windows.Forms.RadioButton rabtnY;
        private System.Windows.Forms.Button btnYWADD;
        private System.Windows.Forms.Button btnYWSAVE;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox txtYWMC;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtYWDM;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.DataGridView dgvYWZD;
        private System.Windows.Forms.DataGridViewTextBoxColumn col_ywdm;
        private System.Windows.Forms.DataGridViewTextBoxColumn col_ywmc;
        private System.Windows.Forms.DataGridViewTextBoxColumn col_qybz;
        private System.Windows.Forms.DataGridViewTextBoxColumn col_qybzmc;
        private System.Windows.Forms.DataGridViewTextBoxColumn col_ffmc;
        private AutoListView.AutoListView alvCS;
        private AutoListView.AutoListView alvDQ;

    }
}