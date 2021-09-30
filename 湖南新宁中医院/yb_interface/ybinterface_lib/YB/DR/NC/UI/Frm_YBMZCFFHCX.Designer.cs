namespace ybinterface_lib
{
    partial class Frm_YBMZCFFHCX
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
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.panel1 = new System.Windows.Forms.Panel();
            this.button1 = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.col_yysfxmmc = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.col_sfxmzxmc = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.col_sfxmdj = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.col_je = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.col_zfje = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.col_zfbz = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.col_bz = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.col_yysfxmmc,
            this.col_sfxmzxmc,
            this.col_sfxmdj,
            this.col_je,
            this.col_zfje,
            this.col_zfbz,
            this.col_bz});
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.Location = new System.Drawing.Point(0, 0);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowTemplate.Height = 23;
            this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView1.Size = new System.Drawing.Size(786, 359);
            this.dataGridView1.TabIndex = 0;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.button1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 359);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(786, 49);
            this.panel1.TabIndex = 1;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(666, 7);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(106, 37);
            this.button1.TabIndex = 0;
            this.button1.Text = "关  闭";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.dataGridView1);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(786, 359);
            this.panel2.TabIndex = 2;
            // 
            // col_yysfxmmc
            // 
            this.col_yysfxmmc.DataPropertyName = "yysfxmmc";
            this.col_yysfxmmc.HeaderText = "医院项目名称";
            this.col_yysfxmmc.Name = "col_yysfxmmc";
            this.col_yysfxmmc.ReadOnly = true;
            this.col_yysfxmmc.Width = 120;
            // 
            // col_sfxmzxmc
            // 
            this.col_sfxmzxmc.DataPropertyName = "sfxmzxmc";
            this.col_sfxmzxmc.HeaderText = "医保项目名称";
            this.col_sfxmzxmc.Name = "col_sfxmzxmc";
            this.col_sfxmzxmc.ReadOnly = true;
            this.col_sfxmzxmc.Width = 120;
            // 
            // col_sfxmdj
            // 
            this.col_sfxmdj.DataPropertyName = "sfxmdj";
            this.col_sfxmdj.HeaderText = "等级";
            this.col_sfxmdj.Name = "col_sfxmdj";
            this.col_sfxmdj.ReadOnly = true;
            this.col_sfxmdj.Width = 80;
            // 
            // col_je
            // 
            this.col_je.DataPropertyName = "je";
            this.col_je.HeaderText = "总额";
            this.col_je.Name = "col_je";
            this.col_je.ReadOnly = true;
            this.col_je.Width = 80;
            // 
            // col_zfje
            // 
            this.col_zfje.DataPropertyName = "zfje";
            this.col_zfje.HeaderText = "自费金额";
            this.col_zfje.Name = "col_zfje";
            this.col_zfje.ReadOnly = true;
            // 
            // col_zfbz
            // 
            this.col_zfbz.DataPropertyName = "zfbz";
            this.col_zfbz.HeaderText = "自费标志";
            this.col_zfbz.Name = "col_zfbz";
            this.col_zfbz.ReadOnly = true;
            // 
            // col_bz
            // 
            this.col_bz.DataPropertyName = "bz";
            this.col_bz.HeaderText = "说明";
            this.col_bz.Name = "col_bz";
            this.col_bz.ReadOnly = true;
            this.col_bz.Width = 200;
            // 
            // Frm_YBMZCFFHCX
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(786, 408);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Name = "Frm_YBMZCFFHCX";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "上传处方明细查询";
            this.Load += new System.EventHandler(this.Frm_YBMZCFFHCX_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.DataGridViewTextBoxColumn col_yysfxmmc;
        private System.Windows.Forms.DataGridViewTextBoxColumn col_sfxmzxmc;
        private System.Windows.Forms.DataGridViewTextBoxColumn col_sfxmdj;
        private System.Windows.Forms.DataGridViewTextBoxColumn col_je;
        private System.Windows.Forms.DataGridViewTextBoxColumn col_zfje;
        private System.Windows.Forms.DataGridViewTextBoxColumn col_zfbz;
        private System.Windows.Forms.DataGridViewTextBoxColumn col_bz;
    }
}