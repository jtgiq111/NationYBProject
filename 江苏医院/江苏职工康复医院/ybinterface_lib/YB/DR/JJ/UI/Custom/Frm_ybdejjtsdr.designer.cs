namespace ybinterface_lib
{
    partial class Frm_ybdejjtsdr
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
            this.lblMess = new System.Windows.Forms.Label();
            this.btn_Close = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblMess
            // 
            this.lblMess.AutoSize = true;
            this.lblMess.Font = new System.Drawing.Font("宋体", 15F);
            this.lblMess.Location = new System.Drawing.Point(12, 78);
            this.lblMess.Name = "lblMess";
            this.lblMess.Size = new System.Drawing.Size(129, 20);
            this.lblMess.TabIndex = 16;
            this.lblMess.Text = "大额基金提示";
            // 
            // btn_Close
            // 
            this.btn_Close.Location = new System.Drawing.Point(746, 12);
            this.btn_Close.Name = "btn_Close";
            this.btn_Close.Size = new System.Drawing.Size(100, 40);
            this.btn_Close.TabIndex = 199;
            this.btn_Close.Text = "关闭";
            this.btn_Close.UseVisualStyleBackColor = true;
            this.btn_Close.Click += new System.EventHandler(this.btn_Close_Click);
            // 
            // Frm_ybdejjtsdr
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(858, 176);
            this.Controls.Add(this.btn_Close);
            this.Controls.Add(this.lblMess);
            this.Font = new System.Drawing.Font("宋体", 11F);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Frm_ybdejjtsdr";
            this.Text = "医保大额基金提示(东软)";
            this.Load += new System.EventHandler(this.frmybzdscdrjj_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblMess;
        private System.Windows.Forms.Button btn_Close;


    }
}