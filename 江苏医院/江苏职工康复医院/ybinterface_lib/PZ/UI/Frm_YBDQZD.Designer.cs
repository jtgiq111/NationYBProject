namespace ybinterface_lib
{
    partial class Frm_YBDQZD
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Frm_YBDQZD));
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel2 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel3 = new System.Windows.Forms.ToolStripStatusLabel();
            this.tssl_czyxm = new System.Windows.Forms.ToolStripStatusLabel();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.dgvDQZD = new System.Windows.Forms.DataGridView();
            this.col_DQDM1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.col_DQMC1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.col_dqqybz = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.col_dqqybzmc = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.col_DQLMC = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnDQSELECT = new System.Windows.Forms.Button();
            this.txtINDEX = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.txtLMC = new System.Windows.Forms.TextBox();
            this.label18 = new System.Windows.Forms.Label();
            this.btnDQADD = new System.Windows.Forms.Button();
            this.btnDQSAVE = new System.Windows.Forms.Button();
            this.txtDQMC = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.txtDQDM = new System.Windows.Forms.TextBox();
            this.label16 = new System.Windows.Forms.Label();
            this.statusStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDQZD)).BeginInit();
            this.panel1.SuspendLayout();
            this.groupBox2.SuspendLayout();
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
            this.statusStrip1.TabIndex = 3;
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
            this.splitContainer1.Panel1.Controls.Add(this.dgvDQZD);
            this.splitContainer1.Panel1.Controls.Add(this.panel1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.BackColor = System.Drawing.Color.White;
            this.splitContainer1.Panel2.Controls.Add(this.groupBox2);
            this.splitContainer1.Size = new System.Drawing.Size(821, 537);
            this.splitContainer1.SplitterDistance = 348;
            this.splitContainer1.SplitterWidth = 5;
            this.splitContainer1.TabIndex = 4;
            // 
            // dgvDQZD
            // 
            this.dgvDQZD.AllowUserToAddRows = false;
            this.dgvDQZD.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvDQZD.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.col_DQDM1,
            this.col_DQMC1,
            this.col_dqqybz,
            this.col_dqqybzmc,
            this.col_DQLMC});
            this.dgvDQZD.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvDQZD.Location = new System.Drawing.Point(0, 36);
            this.dgvDQZD.Name = "dgvDQZD";
            this.dgvDQZD.RowTemplate.Height = 23;
            this.dgvDQZD.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvDQZD.Size = new System.Drawing.Size(821, 312);
            this.dgvDQZD.TabIndex = 3;
            // 
            // col_DQDM1
            // 
            this.col_DQDM1.DataPropertyName = "DQDM";
            this.col_DQDM1.HeaderText = "地区代码";
            this.col_DQDM1.Name = "col_DQDM1";
            this.col_DQDM1.ReadOnly = true;
            // 
            // col_DQMC1
            // 
            this.col_DQMC1.DataPropertyName = "DQMC";
            this.col_DQMC1.HeaderText = "地区名称";
            this.col_DQMC1.Name = "col_DQMC1";
            this.col_DQMC1.ReadOnly = true;
            this.col_DQMC1.Width = 150;
            // 
            // col_dqqybz
            // 
            this.col_dqqybz.DataPropertyName = "QYBZ";
            this.col_dqqybz.HeaderText = "启用标志";
            this.col_dqqybz.Name = "col_dqqybz";
            this.col_dqqybz.ReadOnly = true;
            this.col_dqqybz.Visible = false;
            // 
            // col_dqqybzmc
            // 
            this.col_dqqybzmc.DataPropertyName = "QYBZMC";
            this.col_dqqybzmc.HeaderText = "是否启用";
            this.col_dqqybzmc.Name = "col_dqqybzmc";
            this.col_dqqybzmc.ReadOnly = true;
            this.col_dqqybzmc.Visible = false;
            // 
            // col_DQLMC
            // 
            this.col_DQLMC.DataPropertyName = "LMC";
            this.col_DQLMC.HeaderText = "类名称";
            this.col_DQLMC.Name = "col_DQLMC";
            this.col_DQLMC.ReadOnly = true;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.btnDQSELECT);
            this.panel1.Controls.Add(this.txtINDEX);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(821, 36);
            this.panel1.TabIndex = 2;
            // 
            // btnDQSELECT
            // 
            this.btnDQSELECT.ForeColor = System.Drawing.Color.Blue;
            this.btnDQSELECT.Location = new System.Drawing.Point(363, 4);
            this.btnDQSELECT.Name = "btnDQSELECT";
            this.btnDQSELECT.Size = new System.Drawing.Size(87, 27);
            this.btnDQSELECT.TabIndex = 31;
            this.btnDQSELECT.Text = "查  询";
            this.btnDQSELECT.UseVisualStyleBackColor = true;
            this.btnDQSELECT.Click += new System.EventHandler(this.btnDQSELECT_Click);
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
            this.label1.Text = "地区代码/名称：";
            // 
            // groupBox2
            // 
            this.groupBox2.BackColor = System.Drawing.Color.White;
            this.groupBox2.Controls.Add(this.txtLMC);
            this.groupBox2.Controls.Add(this.label18);
            this.groupBox2.Controls.Add(this.btnDQADD);
            this.groupBox2.Controls.Add(this.btnDQSAVE);
            this.groupBox2.Controls.Add(this.txtDQMC);
            this.groupBox2.Controls.Add(this.label13);
            this.groupBox2.Controls.Add(this.txtDQDM);
            this.groupBox2.Controls.Add(this.label16);
            this.groupBox2.Location = new System.Drawing.Point(3, 3);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(814, 164);
            this.groupBox2.TabIndex = 21;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "地区维护";
            // 
            // txtLMC
            // 
            this.txtLMC.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtLMC.Location = new System.Drawing.Point(78, 79);
            this.txtLMC.Name = "txtLMC";
            this.txtLMC.Size = new System.Drawing.Size(189, 23);
            this.txtLMC.TabIndex = 17;
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(9, 84);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(63, 14);
            this.label18.TabIndex = 18;
            this.label18.Text = "接口名称";
            // 
            // btnDQADD
            // 
            this.btnDQADD.ForeColor = System.Drawing.Color.Blue;
            this.btnDQADD.Location = new System.Drawing.Point(75, 121);
            this.btnDQADD.Name = "btnDQADD";
            this.btnDQADD.Size = new System.Drawing.Size(87, 27);
            this.btnDQADD.TabIndex = 16;
            this.btnDQADD.Text = "新  增";
            this.btnDQADD.UseVisualStyleBackColor = true;
            this.btnDQADD.Click += new System.EventHandler(this.btnDQADD_Click);
            // 
            // btnDQSAVE
            // 
            this.btnDQSAVE.ForeColor = System.Drawing.Color.Blue;
            this.btnDQSAVE.Location = new System.Drawing.Point(180, 121);
            this.btnDQSAVE.Name = "btnDQSAVE";
            this.btnDQSAVE.Size = new System.Drawing.Size(87, 27);
            this.btnDQSAVE.TabIndex = 3;
            this.btnDQSAVE.Text = "修  改";
            this.btnDQSAVE.UseVisualStyleBackColor = true;
            this.btnDQSAVE.Click += new System.EventHandler(this.btnDQSAVE_Click);
            // 
            // txtDQMC
            // 
            this.txtDQMC.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtDQMC.Location = new System.Drawing.Point(351, 36);
            this.txtDQMC.Name = "txtDQMC";
            this.txtDQMC.Size = new System.Drawing.Size(189, 23);
            this.txtDQMC.TabIndex = 4;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(282, 41);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(63, 14);
            this.label13.TabIndex = 5;
            this.label13.Text = "地区名称";
            // 
            // txtDQDM
            // 
            this.txtDQDM.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtDQDM.Location = new System.Drawing.Point(78, 34);
            this.txtDQDM.Name = "txtDQDM";
            this.txtDQDM.Size = new System.Drawing.Size(189, 23);
            this.txtDQDM.TabIndex = 3;
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(9, 38);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(63, 14);
            this.label16.TabIndex = 3;
            this.label16.Text = "地区代码";
            // 
            // Frm_YBDQZD
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(821, 562);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.statusStrip1);
            this.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Frm_YBDQZD";
            this.Tag = "ybinterface_lib.Frm_YBDQZD";
            this.Text = "医保地区字典设置";
            this.Load += new System.EventHandler(this.Frm_YBDQZD_Load);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvDQZD)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
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
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox txtLMC;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Button btnDQADD;
        private System.Windows.Forms.Button btnDQSAVE;
        private System.Windows.Forms.TextBox txtDQMC;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.TextBox txtDQDM;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnDQSELECT;
        private System.Windows.Forms.TextBox txtINDEX;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DataGridView dgvDQZD;
        private System.Windows.Forms.DataGridViewTextBoxColumn col_DQDM1;
        private System.Windows.Forms.DataGridViewTextBoxColumn col_DQMC1;
        private System.Windows.Forms.DataGridViewTextBoxColumn col_dqqybz;
        private System.Windows.Forms.DataGridViewTextBoxColumn col_dqqybzmc;
        private System.Windows.Forms.DataGridViewTextBoxColumn col_DQLMC;
    }
}