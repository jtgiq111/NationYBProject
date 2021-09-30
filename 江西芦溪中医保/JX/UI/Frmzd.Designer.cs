
namespace yb_interfaces
{
    partial class Frmzd
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
            this.dgvzdinfo = new System.Windows.Forms.DataGridView();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.txtFilterword = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dgvzdinfo)).BeginInit();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // dgvzdinfo
            // 
            this.dgvzdinfo.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvzdinfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvzdinfo.Location = new System.Drawing.Point(0, 0);
            this.dgvzdinfo.Name = "dgvzdinfo";
            this.dgvzdinfo.RowHeadersWidth = 51;
            this.dgvzdinfo.RowTemplate.Height = 27;
            this.dgvzdinfo.Size = new System.Drawing.Size(800, 405);
            this.dgvzdinfo.TabIndex = 0;
            this.dgvzdinfo.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvzdinfo_CellDoubleClick);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.txtFilterword);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(800, 45);
            this.panel1.TabIndex = 1;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.dgvzdinfo);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 45);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(800, 405);
            this.panel2.TabIndex = 2;
            // 
            // txtFilterword
            // 
            this.txtFilterword.Location = new System.Drawing.Point(155, 10);
            this.txtFilterword.Name = "txtFilterword";
            this.txtFilterword.Size = new System.Drawing.Size(224, 25);
            this.txtFilterword.TabIndex = 0;
            this.txtFilterword.TextChanged += new System.EventHandler(this.txtFilterword_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(16, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(136, 15);
            this.label1.TabIndex = 1;
            this.label1.Text = "检索框(名称/编码)";
            // 
            // Frmzd
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Name = "Frmzd";
            this.Text = "医保项目字典";
            ((System.ComponentModel.ISupportInitialize)(this.dgvzdinfo)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvzdinfo;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtFilterword;
        private System.Windows.Forms.Panel panel2;
    }
}