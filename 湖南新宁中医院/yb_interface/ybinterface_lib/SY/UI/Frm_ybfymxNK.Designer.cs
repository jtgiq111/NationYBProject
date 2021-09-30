namespace ybinterface_lib.SY.UI
{
    public partial class Frm_ybfymxNK
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
            this.dataGridView = new System.Windows.Forms.DataGridView();
            this.yyxmbh = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.yyxmmc = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ybxmbh = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ybxmmc = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.gg = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dj = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.sl = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.je = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.sfxmdj = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridView
            // 
            this.dataGridView.BackgroundColor = System.Drawing.SystemColors.Control;
            this.dataGridView.ColumnHeadersHeight = 25;
            this.dataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.yyxmbh,
            this.yyxmmc,
            this.ybxmbh,
            this.ybxmmc,
            this.gg,
            this.dj,
            this.sl,
            this.je,
            this.sfxmdj});
            this.dataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView.Location = new System.Drawing.Point(0, 0);
            this.dataGridView.Name = "dataGridView";
            this.dataGridView.RowHeadersWidth = 12;
            this.dataGridView.RowTemplate.Height = 23;
            this.dataGridView.Size = new System.Drawing.Size(802, 462);
            this.dataGridView.TabIndex = 0;
            // 
            // yyxmbh
            // 
            this.yyxmbh.DataPropertyName = "yyxmbh";
            this.yyxmbh.HeaderText = "本院代码";
            this.yyxmbh.Name = "yyxmbh";
            this.yyxmbh.Width = 80;
            // 
            // yyxmmc
            // 
            this.yyxmmc.DataPropertyName = "yyxmmc";
            this.yyxmmc.HeaderText = "本院名称";
            this.yyxmmc.Name = "yyxmmc";
            this.yyxmmc.Width = 150;
            // 
            // ybxmbh
            // 
            this.ybxmbh.DataPropertyName = "ybxmbh";
            this.ybxmbh.HeaderText = "医保代码";
            this.ybxmbh.Name = "ybxmbh";
            this.ybxmbh.Width = 85;
            // 
            // ybxmmc
            // 
            this.ybxmmc.DataPropertyName = "ybxmmc";
            this.ybxmmc.HeaderText = "医保名称";
            this.ybxmmc.Name = "ybxmmc";
            this.ybxmmc.Width = 150;
            // 
            // gg
            // 
            this.gg.DataPropertyName = "gg";
            this.gg.HeaderText = "规格";
            this.gg.Name = "gg";
            this.gg.Width = 90;
            // 
            // dj
            // 
            this.dj.DataPropertyName = "dj";
            this.dj.HeaderText = "单价";
            this.dj.Name = "dj";
            this.dj.Width = 60;
            // 
            // sl
            // 
            this.sl.DataPropertyName = "sl";
            this.sl.HeaderText = "数量";
            this.sl.Name = "sl";
            this.sl.Width = 45;
            // 
            // je
            // 
            this.je.DataPropertyName = "je";
            this.je.HeaderText = "金额";
            this.je.Name = "je";
            this.je.Width = 60;
            // 
            // sfxmdj
            // 
            this.sfxmdj.DataPropertyName = "sfxmdj";
            this.sfxmdj.HeaderText = "标志";
            this.sfxmdj.Name = "sfxmdj";
            this.sfxmdj.Width = 70;
            // 
            // Frm_ybfymx
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(802, 462);
            this.Controls.Add(this.dataGridView);
            this.KeyPreview = true;
            this.Name = "Frm_ybfymx";
            this.Text = "医保上传费用信息";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Frm_ybfymx_KeyDown);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.DataGridView dataGridView;
        private System.Windows.Forms.DataGridViewTextBoxColumn yyxmbh;
        private System.Windows.Forms.DataGridViewTextBoxColumn yyxmmc;
        private System.Windows.Forms.DataGridViewTextBoxColumn ybxmbh;
        private System.Windows.Forms.DataGridViewTextBoxColumn ybxmmc;
        private System.Windows.Forms.DataGridViewTextBoxColumn gg;
        private System.Windows.Forms.DataGridViewTextBoxColumn dj;
        private System.Windows.Forms.DataGridViewTextBoxColumn sl;
        private System.Windows.Forms.DataGridViewTextBoxColumn je;
        private System.Windows.Forms.DataGridViewTextBoxColumn sfxmdj;
    }
}