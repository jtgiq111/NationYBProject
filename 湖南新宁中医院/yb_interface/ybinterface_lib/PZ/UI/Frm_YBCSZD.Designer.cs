namespace ybinterface_lib
{
    partial class Frm_YBCSZD
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Frm_YBCSZD));
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel2 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel3 = new System.Windows.Forms.ToolStripStatusLabel();
            this.tssl_czyxm = new System.Windows.Forms.ToolStripStatusLabel();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.dgvSCZD = new System.Windows.Forms.DataGridView();
            this.col_csdm = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.col_csmc = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.col_csjm = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.col_csqybz = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.col_csqybzmc = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnCSSELECT = new System.Windows.Forms.Button();
            this.txtINDEX = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.txtSCJM = new System.Windows.Forms.TextBox();
            this.label26 = new System.Windows.Forms.Label();
            this.btnSCADD = new System.Windows.Forms.Button();
            this.btnSCSAVE = new System.Windows.Forms.Button();
            this.txtSCMC = new System.Windows.Forms.TextBox();
            this.label14 = new System.Windows.Forms.Label();
            this.txtSCDM = new System.Windows.Forms.TextBox();
            this.label15 = new System.Windows.Forms.Label();
            this.statusStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSCZD)).BeginInit();
            this.panel1.SuspendLayout();
            this.groupBox3.SuspendLayout();
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
            this.statusStrip1.TabIndex = 2;
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
            this.splitContainer1.Panel1.Controls.Add(this.dgvSCZD);
            this.splitContainer1.Panel1.Controls.Add(this.panel1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.BackColor = System.Drawing.Color.White;
            this.splitContainer1.Panel2.Controls.Add(this.groupBox3);
            this.splitContainer1.Size = new System.Drawing.Size(821, 537);
            this.splitContainer1.SplitterDistance = 381;
            this.splitContainer1.SplitterWidth = 5;
            this.splitContainer1.TabIndex = 3;
            // 
            // dgvSCZD
            // 
            this.dgvSCZD.AllowUserToAddRows = false;
            this.dgvSCZD.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvSCZD.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.col_csdm,
            this.col_csmc,
            this.col_csjm,
            this.col_csqybz,
            this.col_csqybzmc});
            this.dgvSCZD.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvSCZD.Location = new System.Drawing.Point(0, 36);
            this.dgvSCZD.Name = "dgvSCZD";
            this.dgvSCZD.RowTemplate.Height = 23;
            this.dgvSCZD.Size = new System.Drawing.Size(821, 345);
            this.dgvSCZD.TabIndex = 2;
            this.dgvSCZD.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvSCZD_CellDoubleClick);
            // 
            // col_csdm
            // 
            this.col_csdm.DataPropertyName = "CSDM";
            this.col_csdm.HeaderText = "厂商代码";
            this.col_csdm.Name = "col_csdm";
            this.col_csdm.ReadOnly = true;
            // 
            // col_csmc
            // 
            this.col_csmc.DataPropertyName = "CSMC";
            this.col_csmc.HeaderText = "厂商名称";
            this.col_csmc.Name = "col_csmc";
            this.col_csmc.ReadOnly = true;
            this.col_csmc.Width = 150;
            // 
            // col_csjm
            // 
            this.col_csjm.DataPropertyName = "CSJM";
            this.col_csjm.HeaderText = "厂商简码";
            this.col_csjm.Name = "col_csjm";
            this.col_csjm.ReadOnly = true;
            // 
            // col_csqybz
            // 
            this.col_csqybz.DataPropertyName = "QYBZ";
            this.col_csqybz.HeaderText = "启用标志";
            this.col_csqybz.Name = "col_csqybz";
            this.col_csqybz.ReadOnly = true;
            this.col_csqybz.Visible = false;
            // 
            // col_csqybzmc
            // 
            this.col_csqybzmc.DataPropertyName = "QYBZMC";
            this.col_csqybzmc.HeaderText = "是否启用";
            this.col_csqybzmc.Name = "col_csqybzmc";
            this.col_csqybzmc.ReadOnly = true;
            this.col_csqybzmc.Visible = false;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.btnCSSELECT);
            this.panel1.Controls.Add(this.txtINDEX);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(821, 36);
            this.panel1.TabIndex = 1;
            // 
            // btnCSSELECT
            // 
            this.btnCSSELECT.ForeColor = System.Drawing.Color.Blue;
            this.btnCSSELECT.Location = new System.Drawing.Point(363, 4);
            this.btnCSSELECT.Name = "btnCSSELECT";
            this.btnCSSELECT.Size = new System.Drawing.Size(87, 27);
            this.btnCSSELECT.TabIndex = 31;
            this.btnCSSELECT.Text = "查  询";
            this.btnCSSELECT.UseVisualStyleBackColor = true;
            this.btnCSSELECT.Click += new System.EventHandler(this.btnCSSELECT_Click);
            // 
            // txtINDEX
            // 
            this.txtINDEX.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtINDEX.Location = new System.Drawing.Point(128, 6);
            this.txtINDEX.Name = "txtINDEX";
            this.txtINDEX.Size = new System.Drawing.Size(225, 23);
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
            this.label1.Text = "厂商代码/名称：";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.txtSCJM);
            this.groupBox3.Controls.Add(this.label26);
            this.groupBox3.Controls.Add(this.btnSCADD);
            this.groupBox3.Controls.Add(this.btnSCSAVE);
            this.groupBox3.Controls.Add(this.txtSCMC);
            this.groupBox3.Controls.Add(this.label14);
            this.groupBox3.Controls.Add(this.txtSCDM);
            this.groupBox3.Controls.Add(this.label15);
            this.groupBox3.Location = new System.Drawing.Point(3, 3);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(783, 138);
            this.groupBox3.TabIndex = 5;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "厂商维护";
            // 
            // txtSCJM
            // 
            this.txtSCJM.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtSCJM.Location = new System.Drawing.Point(78, 77);
            this.txtSCJM.Name = "txtSCJM";
            this.txtSCJM.Size = new System.Drawing.Size(135, 23);
            this.txtSCJM.TabIndex = 17;
            // 
            // label26
            // 
            this.label26.AutoSize = true;
            this.label26.Location = new System.Drawing.Point(9, 82);
            this.label26.Name = "label26";
            this.label26.Size = new System.Drawing.Size(63, 14);
            this.label26.TabIndex = 18;
            this.label26.Text = "厂商简码";
            // 
            // btnSCADD
            // 
            this.btnSCADD.ForeColor = System.Drawing.Color.Blue;
            this.btnSCADD.Location = new System.Drawing.Point(301, 73);
            this.btnSCADD.Name = "btnSCADD";
            this.btnSCADD.Size = new System.Drawing.Size(87, 27);
            this.btnSCADD.TabIndex = 16;
            this.btnSCADD.Text = "新  增";
            this.btnSCADD.UseVisualStyleBackColor = true;
            this.btnSCADD.Click += new System.EventHandler(this.btnSCADD_Click);
            // 
            // btnSCSAVE
            // 
            this.btnSCSAVE.ForeColor = System.Drawing.Color.Blue;
            this.btnSCSAVE.Location = new System.Drawing.Point(402, 73);
            this.btnSCSAVE.Name = "btnSCSAVE";
            this.btnSCSAVE.Size = new System.Drawing.Size(87, 27);
            this.btnSCSAVE.TabIndex = 3;
            this.btnSCSAVE.Text = "修  改";
            this.btnSCSAVE.UseVisualStyleBackColor = true;
            this.btnSCSAVE.Click += new System.EventHandler(this.btnSCSAVE_Click);
            // 
            // txtSCMC
            // 
            this.txtSCMC.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtSCMC.Location = new System.Drawing.Point(301, 34);
            this.txtSCMC.Name = "txtSCMC";
            this.txtSCMC.Size = new System.Drawing.Size(189, 23);
            this.txtSCMC.TabIndex = 4;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(232, 38);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(63, 14);
            this.label14.TabIndex = 5;
            this.label14.Text = "厂商名称";
            // 
            // txtSCDM
            // 
            this.txtSCDM.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtSCDM.Location = new System.Drawing.Point(78, 34);
            this.txtSCDM.Name = "txtSCDM";
            this.txtSCDM.Size = new System.Drawing.Size(135, 23);
            this.txtSCDM.TabIndex = 3;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(9, 38);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(63, 14);
            this.label15.TabIndex = 3;
            this.label15.Text = "厂商代码";
            // 
            // Frm_YBCSZD
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(821, 562);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.statusStrip1);
            this.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Frm_YBCSZD";
            this.Tag = "ybinterface_lib.Frm_YBCSZD";
            this.Text = "医保厂商字典设置";
            this.Load += new System.EventHandler(this.Frm_YBCSZD_Load);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvSCZD)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
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
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TextBox txtSCJM;
        private System.Windows.Forms.Label label26;
        private System.Windows.Forms.Button btnSCADD;
        private System.Windows.Forms.Button btnSCSAVE;
        private System.Windows.Forms.TextBox txtSCMC;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.TextBox txtSCDM;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnCSSELECT;
        private System.Windows.Forms.TextBox txtINDEX;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DataGridView dgvSCZD;
        private System.Windows.Forms.DataGridViewTextBoxColumn col_csdm;
        private System.Windows.Forms.DataGridViewTextBoxColumn col_csmc;
        private System.Windows.Forms.DataGridViewTextBoxColumn col_csjm;
        private System.Windows.Forms.DataGridViewTextBoxColumn col_csqybz;
        private System.Windows.Forms.DataGridViewTextBoxColumn col_csqybzmc;
    }
}