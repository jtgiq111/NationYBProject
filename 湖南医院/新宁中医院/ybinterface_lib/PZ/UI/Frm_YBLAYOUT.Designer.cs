namespace ybinterface_lib
{
    partial class Frm_YBLAYOUT
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
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel2 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel3 = new System.Windows.Forms.ToolStripStatusLabel();
            this.tssl_czyxm = new System.Windows.Forms.ToolStripStatusLabel();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.panel1 = new System.Windows.Forms.Panel();
            this.cmbCTLB = new System.Windows.Forms.ComboBox();
            this.btnPZSELECT = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.btnUPDATE = new System.Windows.Forms.Button();
            this.btnDEL = new System.Windows.Forms.Button();
            this.btnADD = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.rabtnQY = new System.Windows.Forms.RadioButton();
            this.rabtnBQY = new System.Windows.Forms.RadioButton();
            this.label6 = new System.Windows.Forms.Label();
            this.txtSEQU = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txtCTTITLE = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtCTNAME = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtCTLB = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.dgvLayout = new System.Windows.Forms.DataGridView();
            this.col_ctlb = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.col_ctmc = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.col_ctdm = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.col_ctseq = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.statusStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvLayout)).BeginInit();
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
            this.statusStrip1.TabIndex = 4;
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
            this.splitContainer1.Panel1.Controls.Add(this.dgvLayout);
            this.splitContainer1.Panel1.Controls.Add(this.panel1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.btnUPDATE);
            this.splitContainer1.Panel2.Controls.Add(this.btnDEL);
            this.splitContainer1.Panel2.Controls.Add(this.btnADD);
            this.splitContainer1.Panel2.Controls.Add(this.groupBox2);
            this.splitContainer1.Panel2.Controls.Add(this.label6);
            this.splitContainer1.Panel2.Controls.Add(this.txtSEQU);
            this.splitContainer1.Panel2.Controls.Add(this.label5);
            this.splitContainer1.Panel2.Controls.Add(this.txtCTTITLE);
            this.splitContainer1.Panel2.Controls.Add(this.label2);
            this.splitContainer1.Panel2.Controls.Add(this.txtCTNAME);
            this.splitContainer1.Panel2.Controls.Add(this.label4);
            this.splitContainer1.Panel2.Controls.Add(this.txtCTLB);
            this.splitContainer1.Panel2.Controls.Add(this.label3);
            this.splitContainer1.Size = new System.Drawing.Size(821, 537);
            this.splitContainer1.SplitterDistance = 383;
            this.splitContainer1.TabIndex = 5;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.cmbCTLB);
            this.panel1.Controls.Add(this.btnPZSELECT);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(821, 36);
            this.panel1.TabIndex = 3;
            // 
            // cmbCTLB
            // 
            this.cmbCTLB.FormattingEnabled = true;
            this.cmbCTLB.Location = new System.Drawing.Point(84, 6);
            this.cmbCTLB.Name = "cmbCTLB";
            this.cmbCTLB.Size = new System.Drawing.Size(177, 22);
            this.cmbCTLB.TabIndex = 34;
            // 
            // btnPZSELECT
            // 
            this.btnPZSELECT.ForeColor = System.Drawing.Color.Blue;
            this.btnPZSELECT.Location = new System.Drawing.Point(277, 4);
            this.btnPZSELECT.Name = "btnPZSELECT";
            this.btnPZSELECT.Size = new System.Drawing.Size(87, 27);
            this.btnPZSELECT.TabIndex = 31;
            this.btnPZSELECT.Text = "查  询";
            this.btnPZSELECT.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.Color.Blue;
            this.label1.Location = new System.Drawing.Point(14, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(77, 14);
            this.label1.TabIndex = 0;
            this.label1.Text = "窗体类别：";
            // 
            // btnUPDATE
            // 
            this.btnUPDATE.ForeColor = System.Drawing.Color.Blue;
            this.btnUPDATE.Location = new System.Drawing.Point(431, 94);
            this.btnUPDATE.Name = "btnUPDATE";
            this.btnUPDATE.Size = new System.Drawing.Size(75, 23);
            this.btnUPDATE.TabIndex = 43;
            this.btnUPDATE.Text = "修　改";
            this.btnUPDATE.UseVisualStyleBackColor = true;
            this.btnUPDATE.Visible = false;
            // 
            // btnDEL
            // 
            this.btnDEL.ForeColor = System.Drawing.Color.Blue;
            this.btnDEL.Location = new System.Drawing.Point(521, 94);
            this.btnDEL.Name = "btnDEL";
            this.btnDEL.Size = new System.Drawing.Size(75, 23);
            this.btnDEL.TabIndex = 42;
            this.btnDEL.Text = "删 除";
            this.btnDEL.UseVisualStyleBackColor = true;
            // 
            // btnADD
            // 
            this.btnADD.ForeColor = System.Drawing.Color.Blue;
            this.btnADD.Location = new System.Drawing.Point(341, 94);
            this.btnADD.Name = "btnADD";
            this.btnADD.Size = new System.Drawing.Size(75, 23);
            this.btnADD.TabIndex = 41;
            this.btnADD.Text = "新 增";
            this.btnADD.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.rabtnQY);
            this.groupBox2.Controls.Add(this.rabtnBQY);
            this.groupBox2.Location = new System.Drawing.Point(411, 45);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(157, 44);
            this.groupBox2.TabIndex = 40;
            this.groupBox2.TabStop = false;
            // 
            // rabtnQY
            // 
            this.rabtnQY.AutoSize = true;
            this.rabtnQY.Checked = true;
            this.rabtnQY.Location = new System.Drawing.Point(13, 16);
            this.rabtnQY.Name = "rabtnQY";
            this.rabtnQY.Size = new System.Drawing.Size(53, 18);
            this.rabtnQY.TabIndex = 11;
            this.rabtnQY.TabStop = true;
            this.rabtnQY.Text = "启用";
            this.rabtnQY.UseVisualStyleBackColor = true;
            // 
            // rabtnBQY
            // 
            this.rabtnBQY.AutoSize = true;
            this.rabtnBQY.Location = new System.Drawing.Point(81, 16);
            this.rabtnBQY.Name = "rabtnBQY";
            this.rabtnBQY.Size = new System.Drawing.Size(67, 18);
            this.rabtnBQY.TabIndex = 12;
            this.rabtnBQY.Text = "不启用";
            this.rabtnBQY.UseVisualStyleBackColor = true;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label6.Location = new System.Drawing.Point(340, 64);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(70, 14);
            this.label6.TabIndex = 39;
            this.label6.Text = "是否启用:";
            // 
            // txtSEQU
            // 
            this.txtSEQU.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtSEQU.Location = new System.Drawing.Point(411, 15);
            this.txtSEQU.Name = "txtSEQU";
            this.txtSEQU.Size = new System.Drawing.Size(95, 23);
            this.txtSEQU.TabIndex = 15;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label5.ForeColor = System.Drawing.Color.Red;
            this.label5.Location = new System.Drawing.Point(340, 19);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(70, 14);
            this.label5.TabIndex = 14;
            this.label5.Text = "窗体排序:";
            // 
            // txtCTTITLE
            // 
            this.txtCTTITLE.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtCTTITLE.Location = new System.Drawing.Point(85, 94);
            this.txtCTTITLE.Name = "txtCTTITLE";
            this.txtCTTITLE.Size = new System.Drawing.Size(232, 23);
            this.txtCTTITLE.TabIndex = 13;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.ForeColor = System.Drawing.Color.Red;
            this.label2.Location = new System.Drawing.Point(14, 98);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(70, 14);
            this.label2.TabIndex = 12;
            this.label2.Text = "窗体TEXT:";
            // 
            // txtCTNAME
            // 
            this.txtCTNAME.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtCTNAME.Location = new System.Drawing.Point(85, 55);
            this.txtCTNAME.Name = "txtCTNAME";
            this.txtCTNAME.Size = new System.Drawing.Size(232, 23);
            this.txtCTNAME.TabIndex = 11;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label4.ForeColor = System.Drawing.Color.Red;
            this.label4.Location = new System.Drawing.Point(14, 59);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(70, 14);
            this.label4.TabIndex = 10;
            this.label4.Text = "窗体NAME:";
            // 
            // txtCTLB
            // 
            this.txtCTLB.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtCTLB.Location = new System.Drawing.Point(85, 13);
            this.txtCTLB.Name = "txtCTLB";
            this.txtCTLB.Size = new System.Drawing.Size(232, 23);
            this.txtCTLB.TabIndex = 9;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label3.ForeColor = System.Drawing.Color.Red;
            this.label3.Location = new System.Drawing.Point(14, 17);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(70, 14);
            this.label3.TabIndex = 8;
            this.label3.Text = "窗体类别:";
            // 
            // dgvLayout
            // 
            this.dgvLayout.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvLayout.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.col_ctlb,
            this.col_ctmc,
            this.col_ctdm,
            this.col_ctseq});
            this.dgvLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvLayout.Location = new System.Drawing.Point(0, 36);
            this.dgvLayout.Name = "dgvLayout";
            this.dgvLayout.RowTemplate.Height = 23;
            this.dgvLayout.Size = new System.Drawing.Size(821, 347);
            this.dgvLayout.TabIndex = 4;
            // 
            // col_ctlb
            // 
            this.col_ctlb.HeaderText = "窗体类别";
            this.col_ctlb.Name = "col_ctlb";
            this.col_ctlb.ReadOnly = true;
            // 
            // col_ctmc
            // 
            this.col_ctmc.HeaderText = "窗体标题";
            this.col_ctmc.Name = "col_ctmc";
            this.col_ctmc.ReadOnly = true;
            // 
            // col_ctdm
            // 
            this.col_ctdm.HeaderText = "窗体映射名";
            this.col_ctdm.Name = "col_ctdm";
            this.col_ctdm.ReadOnly = true;
            // 
            // col_ctseq
            // 
            this.col_ctseq.HeaderText = "窗体排序";
            this.col_ctseq.Name = "col_ctseq";
            this.col_ctseq.ReadOnly = true;
            // 
            // Frm_YBLAYOUT
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(821, 562);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.statusStrip1);
            this.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Name = "Frm_YBLAYOUT";
            this.Text = "医保窗体排序";
            this.Load += new System.EventHandler(this.Frm_YBLAYOUT_Load);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvLayout)).EndInit();
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
        private System.Windows.Forms.Button btnPZSELECT;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmbCTLB;
        private System.Windows.Forms.TextBox txtSEQU;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtCTTITLE;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtCTNAME;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtCTLB;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RadioButton rabtnQY;
        private System.Windows.Forms.RadioButton rabtnBQY;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button btnUPDATE;
        private System.Windows.Forms.Button btnDEL;
        private System.Windows.Forms.Button btnADD;
        private System.Windows.Forms.DataGridView dgvLayout;
        private System.Windows.Forms.DataGridViewTextBoxColumn col_ctlb;
        private System.Windows.Forms.DataGridViewTextBoxColumn col_ctmc;
        private System.Windows.Forms.DataGridViewTextBoxColumn col_ctdm;
        private System.Windows.Forms.DataGridViewTextBoxColumn col_ctseq;
    }
}