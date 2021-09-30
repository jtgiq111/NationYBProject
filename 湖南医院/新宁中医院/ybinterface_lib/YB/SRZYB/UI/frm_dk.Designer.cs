namespace ybinterface_lib
{
    partial class frm_dk
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
            this.dgv_dk = new System.Windows.Forms.DataGridView();
            this.col_grbh = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.col_xm = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.col_xb = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.col_csrq = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.col_gmsfhm = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.col_xslb = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.col_dwmc = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.col_cbdmc = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dgv_dk)).BeginInit();
            this.SuspendLayout();
            // 
            // dgv_dk
            // 
            this.dgv_dk.AllowUserToAddRows = false;
            this.dgv_dk.AllowUserToDeleteRows = false;
            this.dgv_dk.BackgroundColor = System.Drawing.Color.White;
            this.dgv_dk.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv_dk.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.col_grbh,
            this.col_xm,
            this.col_xb,
            this.col_csrq,
            this.col_gmsfhm,
            this.col_xslb,
            this.col_dwmc,
            this.col_cbdmc});
            this.dgv_dk.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgv_dk.Location = new System.Drawing.Point(0, 0);
            this.dgv_dk.Name = "dgv_dk";
            this.dgv_dk.RowHeadersVisible = false;
            this.dgv_dk.RowTemplate.Height = 23;
            this.dgv_dk.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgv_dk.Size = new System.Drawing.Size(434, 164);
            this.dgv_dk.TabIndex = 1;
            this.dgv_dk.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgv_dk_CellDoubleClick);
            // 
            // col_grbh
            // 
            this.col_grbh.DataPropertyName = "grbh";
            this.col_grbh.HeaderText = "个人标识号";
            this.col_grbh.Name = "col_grbh";
            this.col_grbh.ReadOnly = true;
            // 
            // col_xm
            // 
            this.col_xm.DataPropertyName = "xm";
            this.col_xm.HeaderText = "姓名";
            this.col_xm.Name = "col_xm";
            this.col_xm.ReadOnly = true;
            // 
            // col_xb
            // 
            this.col_xb.DataPropertyName = "xb";
            this.col_xb.HeaderText = "性别";
            this.col_xb.Name = "col_xb";
            this.col_xb.ReadOnly = true;
            // 
            // col_csrq
            // 
            this.col_csrq.DataPropertyName = "csrq";
            this.col_csrq.HeaderText = "出生日期";
            this.col_csrq.Name = "col_csrq";
            this.col_csrq.ReadOnly = true;
            // 
            // col_gmsfhm
            // 
            this.col_gmsfhm.DataPropertyName = "gmsfhm";
            this.col_gmsfhm.HeaderText = "公民身份号码";
            this.col_gmsfhm.Name = "col_gmsfhm";
            this.col_gmsfhm.ReadOnly = true;
            // 
            // col_xslb
            // 
            this.col_xslb.DataPropertyName = "xslb";
            this.col_xslb.HeaderText = "享受类别";
            this.col_xslb.Name = "col_xslb";
            this.col_xslb.ReadOnly = true;
            // 
            // col_dwmc
            // 
            this.col_dwmc.DataPropertyName = "dwmc";
            this.col_dwmc.HeaderText = "单位名称";
            this.col_dwmc.Name = "col_dwmc";
            this.col_dwmc.ReadOnly = true;
            // 
            // col_cbdmc
            // 
            this.col_cbdmc.DataPropertyName = "cbdmc";
            this.col_cbdmc.HeaderText = "参保地名称";
            this.col_cbdmc.Name = "col_cbdmc";
            this.col_cbdmc.ReadOnly = true;
            // 
            // frm_dk
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(434, 164);
            this.Controls.Add(this.dgv_dk);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "frm_dk";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "医保卡信息";
            this.Load += new System.EventHandler(this.frm_dk_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgv_dk)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dgv_dk;
        private System.Windows.Forms.DataGridViewTextBoxColumn col_grbh;
        private System.Windows.Forms.DataGridViewTextBoxColumn col_xm;
        private System.Windows.Forms.DataGridViewTextBoxColumn col_xb;
        private System.Windows.Forms.DataGridViewTextBoxColumn col_csrq;
        private System.Windows.Forms.DataGridViewTextBoxColumn col_gmsfhm;
        private System.Windows.Forms.DataGridViewTextBoxColumn col_xslb;
        private System.Windows.Forms.DataGridViewTextBoxColumn col_dwmc;
        private System.Windows.Forms.DataGridViewTextBoxColumn col_cbdmc;
    }
}