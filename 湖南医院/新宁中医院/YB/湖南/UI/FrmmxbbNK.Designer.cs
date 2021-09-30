namespace yb_interfaces.YB.湖南.UI
{
    partial class FrmmxbbNK
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
            this.DataGridView = new Srvtools.InfoDataGridView();
            this.bzbm = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.bzmc = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.DataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // DataGridView
            // 
            this.DataGridView.AllowUserToAddRows = false;
            this.DataGridView.AllowUserToDeleteRows = false;
            this.DataGridView.BackgroundColor = System.Drawing.SystemColors.Control;
            this.DataGridView.ColumnHeadersHeight = 25;
            this.DataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.bzbm,
            this.bzmc});
            this.DataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.DataGridView.EnterEnable = true;
            this.DataGridView.EnterRefValControl = false;
            this.DataGridView.Location = new System.Drawing.Point(0, 0);
            this.DataGridView.Name = "DataGridView";
            this.DataGridView.ReadOnly = true;
            this.DataGridView.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            this.DataGridView.RowHeadersWidth = 15;
            this.DataGridView.RowTemplate.Height = 23;
            this.DataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.DataGridView.Size = new System.Drawing.Size(290, 128);
            this.DataGridView.SureDelete = false;
            this.DataGridView.TabIndex = 0;
            this.DataGridView.TotalActive = true;
            this.DataGridView.TotalBackColor = System.Drawing.SystemColors.Info;
            this.DataGridView.TotalCaption = null;
            this.DataGridView.TotalCaptionFont = new System.Drawing.Font("宋体", 9F);
            this.DataGridView.TotalFont = new System.Drawing.Font("宋体", 9F);
            this.DataGridView.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.DataGridView_CellDoubleClick);
            this.DataGridView.KeyDown += new System.Windows.Forms.KeyEventHandler(this.DataGridView_KeyDown);
            // 
            // bzbm
            // 
            this.bzbm.DataPropertyName = "bzbm";
            this.bzbm.HeaderText = "病种编码";
            this.bzbm.Name = "bzbm";
            this.bzbm.ReadOnly = true;
            this.bzbm.Width = 90;
            // 
            // bzmc
            // 
            this.bzmc.DataPropertyName = "bzmc";
            this.bzmc.HeaderText = "病种名称";
            this.bzmc.Name = "bzmc";
            this.bzmc.ReadOnly = true;
            this.bzmc.Width = 170;
            // 
            // FrmmxbbXG
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(290, 128);
            this.Controls.Add(this.DataGridView);
            this.Name = "FrmmxbbXG";
            this.Text = "多病种选择";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Frmmxbbz_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.DataGridView)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        public Srvtools.InfoDataGridView DataGridView;
        private System.Windows.Forms.DataGridViewTextBoxColumn bzbm;
        private System.Windows.Forms.DataGridViewTextBoxColumn bzmc;
    }
}