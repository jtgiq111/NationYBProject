namespace ybinterface_lib
{
    partial class Frm_wjcx
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
            this.dateTimeEnd = new System.Windows.Forms.DateTimePicker();
            this.dateTimeStar = new System.Windows.Forms.DateTimePicker();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.button2 = new System.Windows.Forms.Button();
            this.btncx = new System.Windows.Forms.Button();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.dgvbf = new Srvtools.InfoDataGridView();
            this.grbh = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.xm = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cyzd = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ryrq = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cyrq = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.bcfy = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.jrtcfy = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.bcsbje = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.button1 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvbf)).BeginInit();
            this.SuspendLayout();
            // 
            // dateTimeEnd
            // 
            this.dateTimeEnd.Location = new System.Drawing.Point(280, 43);
            this.dateTimeEnd.Name = "dateTimeEnd";
            this.dateTimeEnd.Size = new System.Drawing.Size(109, 21);
            this.dateTimeEnd.TabIndex = 204;
            // 
            // dateTimeStar
            // 
            this.dateTimeStar.Location = new System.Drawing.Point(119, 43);
            this.dateTimeStar.Name = "dateTimeStar";
            this.dateTimeStar.Size = new System.Drawing.Size(109, 21);
            this.dateTimeStar.TabIndex = 203;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(247, 48);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(17, 12);
            this.label4.TabIndex = 202;
            this.label4.Text = "至";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(68, 48);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(29, 12);
            this.label3.TabIndex = 201;
            this.label3.Text = "日期";
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(594, 40);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(93, 23);
            this.button2.TabIndex = 225;
            this.button2.Text = "打印明细";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // btncx
            // 
            this.btncx.Location = new System.Drawing.Point(497, 40);
            this.btncx.Name = "btncx";
            this.btncx.Size = new System.Drawing.Size(75, 23);
            this.btncx.TabIndex = 224;
            this.btncx.Text = "查询";
            this.btncx.UseVisualStyleBackColor = true;
            this.btncx.Click += new System.EventHandler(this.btncx_Click);
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
            this.splitContainer1.Panel1.Controls.Add(this.button1);
            this.splitContainer1.Panel1.Controls.Add(this.dateTimeEnd);
            this.splitContainer1.Panel1.Controls.Add(this.button2);
            this.splitContainer1.Panel1.Controls.Add(this.label3);
            this.splitContainer1.Panel1.Controls.Add(this.btncx);
            this.splitContainer1.Panel1.Controls.Add(this.label4);
            this.splitContainer1.Panel1.Controls.Add(this.dateTimeStar);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.dgvbf);
            this.splitContainer1.Size = new System.Drawing.Size(810, 468);
            this.splitContainer1.SplitterDistance = 100;
            this.splitContainer1.TabIndex = 226;
            // 
            // dgvbf
            // 
            this.dgvbf.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvbf.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.grbh,
            this.xm,
            this.cyzd,
            this.ryrq,
            this.cyrq,
            this.bcfy,
            this.jrtcfy,
            this.bcsbje});
            this.dgvbf.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvbf.EnterEnable = true;
            this.dgvbf.EnterRefValControl = false;
            this.dgvbf.Location = new System.Drawing.Point(0, 0);
            this.dgvbf.Name = "dgvbf";
            this.dgvbf.RowTemplate.Height = 23;
            this.dgvbf.Size = new System.Drawing.Size(810, 364);
            this.dgvbf.SureDelete = false;
            this.dgvbf.TabIndex = 1;
            this.dgvbf.TotalActive = true;
            this.dgvbf.TotalBackColor = System.Drawing.SystemColors.Info;
            this.dgvbf.TotalCaption = null;
            this.dgvbf.TotalCaptionFont = new System.Drawing.Font("宋体", 9F);
            this.dgvbf.TotalFont = new System.Drawing.Font("宋体", 9F);
            // 
            // grbh
            // 
            this.grbh.DataPropertyName = "grbh";
            this.grbh.HeaderText = "个人编号";
            this.grbh.Name = "grbh";
            // 
            // xm
            // 
            this.xm.DataPropertyName = "xm";
            this.xm.HeaderText = "姓名";
            this.xm.Name = "xm";
            // 
            // cyzd
            // 
            this.cyzd.DataPropertyName = "cyzd";
            this.cyzd.HeaderText = "出院诊断";
            this.cyzd.Name = "cyzd";
            // 
            // ryrq
            // 
            this.ryrq.DataPropertyName = "ryrq";
            this.ryrq.HeaderText = "入院日期";
            this.ryrq.Name = "ryrq";
            // 
            // cyrq
            // 
            this.cyrq.DataPropertyName = "cyrq";
            this.cyrq.HeaderText = "出院日期";
            this.cyrq.Name = "cyrq";
            // 
            // bcfy
            // 
            this.bcfy.DataPropertyName = "bcfy";
            this.bcfy.HeaderText = "本次费用";
            this.bcfy.Name = "bcfy";
            // 
            // jrtcfy
            // 
            this.jrtcfy.DataPropertyName = "jrtcfy";
            this.jrtcfy.HeaderText = "进入统筹费用";
            this.jrtcfy.Name = "jrtcfy";
            // 
            // bcsbje
            // 
            this.bcsbje.DataPropertyName = "bcsbje";
            this.bcsbje.HeaderText = "本次实报金额";
            this.bcsbje.Name = "bcsbje";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(705, 40);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(93, 23);
            this.button1.TabIndex = 226;
            this.button1.Text = "导出";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // Frm_wjcx
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(810, 468);
            this.Controls.Add(this.splitContainer1);
            this.Name = "Frm_wjcx";
            this.Text = "外检查询";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvbf)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DateTimePicker dateTimeEnd;
        private System.Windows.Forms.DateTimePicker dateTimeStar;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button btncx;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private Srvtools.InfoDataGridView dgvbf;
        private System.Windows.Forms.DataGridViewTextBoxColumn grbh;
        private System.Windows.Forms.DataGridViewTextBoxColumn xm;
        private System.Windows.Forms.DataGridViewTextBoxColumn cyzd;
        private System.Windows.Forms.DataGridViewTextBoxColumn ryrq;
        private System.Windows.Forms.DataGridViewTextBoxColumn cyrq;
        private System.Windows.Forms.DataGridViewTextBoxColumn bcfy;
        private System.Windows.Forms.DataGridViewTextBoxColumn jrtcfy;
        private System.Windows.Forms.DataGridViewTextBoxColumn bcsbje;
        private System.Windows.Forms.Button button1;
    }
}