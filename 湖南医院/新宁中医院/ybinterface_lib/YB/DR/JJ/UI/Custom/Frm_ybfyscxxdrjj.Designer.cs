namespace ybinterface_lib
{
    partial class Frm_ybfyscxxdrjj
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dgv_fymx = new Srvtools.InfoDataGridView();
            this.yyxmmc = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ybxmmc = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.gg = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.je = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dj = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.sl = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.sflb = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.sfxmdj = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.label3 = new System.Windows.Forms.Label();
            this.lbl_xmfyzj = new System.Windows.Forms.Label();
            this.txt_byxm = new System.Windows.Forms.TextBox();
            this.btncx = new System.Windows.Forms.Button();
            this.rbtn_bypym = new System.Windows.Forms.RadioButton();
            this.rbtn_bymc = new System.Windows.Forms.RadioButton();
            this.label4 = new System.Windows.Forms.Label();
            this.btn_close = new System.Windows.Forms.Button();
            this.lbl_xm = new System.Windows.Forms.Label();
            this.lbl_lsh = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dgv_fymx)).BeginInit();
            this.SuspendLayout();
            // 
            // dgv_fymx
            // 
            this.dgv_fymx.AllowUserToAddRows = false;
            this.dgv_fymx.AllowUserToDeleteRows = false;
            this.dgv_fymx.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv_fymx.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.yyxmmc,
            this.ybxmmc,
            this.gg,
            this.je,
            this.dj,
            this.sl,
            this.sflb,
            this.sfxmdj});
            this.dgv_fymx.EnterEnable = true;
            this.dgv_fymx.EnterRefValControl = false;
            this.dgv_fymx.Location = new System.Drawing.Point(12, 66);
            this.dgv_fymx.Name = "dgv_fymx";
            this.dgv_fymx.ReadOnly = true;
            this.dgv_fymx.RowTemplate.Height = 23;
            this.dgv_fymx.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgv_fymx.Size = new System.Drawing.Size(972, 417);
            this.dgv_fymx.SureDelete = false;
            this.dgv_fymx.TabIndex = 1;
            this.dgv_fymx.TotalActive = true;
            this.dgv_fymx.TotalBackColor = System.Drawing.SystemColors.Info;
            this.dgv_fymx.TotalCaption = null;
            this.dgv_fymx.TotalCaptionFont = new System.Drawing.Font("宋体", 9F);
            this.dgv_fymx.TotalFont = new System.Drawing.Font("宋体", 9F);
            // 
            // yyxmmc
            // 
            this.yyxmmc.DataPropertyName = "yyxmmc";
            this.yyxmmc.HeaderText = "本院项目";
            this.yyxmmc.Name = "yyxmmc";
            this.yyxmmc.ReadOnly = true;
            this.yyxmmc.Width = 120;
            // 
            // ybxmmc
            // 
            this.ybxmmc.DataPropertyName = "ybxmmc";
            this.ybxmmc.HeaderText = "医保项目";
            this.ybxmmc.Name = "ybxmmc";
            this.ybxmmc.ReadOnly = true;
            this.ybxmmc.Width = 120;
            // 
            // gg
            // 
            this.gg.DataPropertyName = "gg";
            this.gg.HeaderText = "规格";
            this.gg.Name = "gg";
            this.gg.ReadOnly = true;
            // 
            // je
            // 
            this.je.DataPropertyName = "je";
            dataGridViewCellStyle1.Format = "N2";
            dataGridViewCellStyle1.NullValue = null;
            this.je.DefaultCellStyle = dataGridViewCellStyle1;
            this.je.HeaderText = "金额";
            this.je.Name = "je";
            this.je.ReadOnly = true;
            // 
            // dj
            // 
            this.dj.DataPropertyName = "dj";
            dataGridViewCellStyle2.Format = "N4";
            dataGridViewCellStyle2.NullValue = null;
            this.dj.DefaultCellStyle = dataGridViewCellStyle2;
            this.dj.HeaderText = "单价";
            this.dj.Name = "dj";
            this.dj.ReadOnly = true;
            // 
            // sl
            // 
            this.sl.DataPropertyName = "sl";
            dataGridViewCellStyle3.Format = "N2";
            dataGridViewCellStyle3.NullValue = null;
            this.sl.DefaultCellStyle = dataGridViewCellStyle3;
            this.sl.HeaderText = "数量";
            this.sl.Name = "sl";
            this.sl.ReadOnly = true;
            this.sl.Width = 80;
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
            this.sfxmdj.HeaderText = "可报";
            this.sfxmdj.Name = "sfxmdj";
            this.sfxmdj.ReadOnly = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(231, 22);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(112, 15);
            this.label3.TabIndex = 54;
            this.label3.Text = "项目费用总计：";
            // 
            // lbl_xmfyzj
            // 
            this.lbl_xmfyzj.AutoSize = true;
            this.lbl_xmfyzj.Location = new System.Drawing.Point(349, 22);
            this.lbl_xmfyzj.Name = "lbl_xmfyzj";
            this.lbl_xmfyzj.Size = new System.Drawing.Size(97, 15);
            this.lbl_xmfyzj.TabIndex = 57;
            this.lbl_xmfyzj.Text = "项目费用总计";
            // 
            // txt_byxm
            // 
            this.txt_byxm.Location = new System.Drawing.Point(551, 17);
            this.txt_byxm.MaxLength = 20;
            this.txt_byxm.Name = "txt_byxm";
            this.txt_byxm.Size = new System.Drawing.Size(117, 24);
            this.txt_byxm.TabIndex = 58;
            this.txt_byxm.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txt_byxm_KeyDown);
            // 
            // btncx
            // 
            this.btncx.Location = new System.Drawing.Point(813, 12);
            this.btncx.Name = "btncx";
            this.btncx.Size = new System.Drawing.Size(75, 34);
            this.btncx.TabIndex = 59;
            this.btncx.Text = "查找";
            this.btncx.UseVisualStyleBackColor = true;
            this.btncx.Click += new System.EventHandler(this.btncx_Click);
            // 
            // rbtn_bypym
            // 
            this.rbtn_bypym.AutoSize = true;
            this.rbtn_bypym.Location = new System.Drawing.Point(681, 18);
            this.rbtn_bypym.Name = "rbtn_bypym";
            this.rbtn_bypym.Size = new System.Drawing.Size(55, 19);
            this.rbtn_bypym.TabIndex = 61;
            this.rbtn_bypym.Text = "拼音";
            this.rbtn_bypym.UseVisualStyleBackColor = true;
            this.rbtn_bypym.Visible = false;
            // 
            // rbtn_bymc
            // 
            this.rbtn_bymc.AutoSize = true;
            this.rbtn_bymc.Checked = true;
            this.rbtn_bymc.Location = new System.Drawing.Point(742, 18);
            this.rbtn_bymc.Name = "rbtn_bymc";
            this.rbtn_bymc.Size = new System.Drawing.Size(55, 19);
            this.rbtn_bymc.TabIndex = 60;
            this.rbtn_bymc.TabStop = true;
            this.rbtn_bymc.Text = "名称";
            this.rbtn_bymc.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(463, 21);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(82, 15);
            this.label4.TabIndex = 64;
            this.label4.Text = "本院项目：";
            // 
            // btn_close
            // 
            this.btn_close.Location = new System.Drawing.Point(909, 10);
            this.btn_close.Name = "btn_close";
            this.btn_close.Size = new System.Drawing.Size(75, 34);
            this.btn_close.TabIndex = 1;
            this.btn_close.Text = "关闭";
            this.btn_close.UseVisualStyleBackColor = true;
            this.btn_close.Click += new System.EventHandler(this.btn_close_Click);
            // 
            // lbl_xm
            // 
            this.lbl_xm.AutoSize = true;
            this.lbl_xm.Location = new System.Drawing.Point(87, 21);
            this.lbl_xm.Name = "lbl_xm";
            this.lbl_xm.Size = new System.Drawing.Size(37, 15);
            this.lbl_xm.TabIndex = 67;
            this.lbl_xm.Text = "姓名";
            // 
            // lbl_lsh
            // 
            this.lbl_lsh.AutoSize = true;
            this.lbl_lsh.Location = new System.Drawing.Point(12, 21);
            this.lbl_lsh.Name = "lbl_lsh";
            this.lbl_lsh.Size = new System.Drawing.Size(52, 15);
            this.lbl_lsh.TabIndex = 66;
            this.lbl_lsh.Text = "流水号";
            // 
            // Frm_ybfyscxxdrjj
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(993, 495);
            this.Controls.Add(this.lbl_xm);
            this.Controls.Add(this.lbl_lsh);
            this.Controls.Add(this.btn_close);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.rbtn_bypym);
            this.Controls.Add(this.rbtn_bymc);
            this.Controls.Add(this.btncx);
            this.Controls.Add(this.txt_byxm);
            this.Controls.Add(this.lbl_xmfyzj);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.dgv_fymx);
            this.Font = new System.Drawing.Font("宋体", 11F);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Frm_ybfyscxxdrjj";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "医保费用上传信息(东软)";
            this.Load += new System.EventHandler(this.Frm_ybfyscxxdrjj_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgv_fymx)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Srvtools.InfoDataGridView dgv_fymx;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lbl_xmfyzj;
        private System.Windows.Forms.TextBox txt_byxm;
        private System.Windows.Forms.Button btncx;
        private System.Windows.Forms.RadioButton rbtn_bypym;
        private System.Windows.Forms.RadioButton rbtn_bymc;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btn_close;
        private System.Windows.Forms.Label lbl_xm;
        private System.Windows.Forms.Label lbl_lsh;
        private System.Windows.Forms.DataGridViewTextBoxColumn yyxmmc;
        private System.Windows.Forms.DataGridViewTextBoxColumn ybxmmc;
        private System.Windows.Forms.DataGridViewTextBoxColumn gg;
        private System.Windows.Forms.DataGridViewTextBoxColumn je;
        private System.Windows.Forms.DataGridViewTextBoxColumn dj;
        private System.Windows.Forms.DataGridViewTextBoxColumn sl;
        private System.Windows.Forms.DataGridViewTextBoxColumn sflb;
        private System.Windows.Forms.DataGridViewTextBoxColumn sfxmdj;

    }
}