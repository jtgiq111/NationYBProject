
namespace yb_interfaces 
{
    partial class FrmxzJDT
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
            this.pbarjd = new System.Windows.Forms.ProgressBar();
            this.label1 = new System.Windows.Forms.Label();
            this.lblJD = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // pbarjd
            // 
            this.pbarjd.Location = new System.Drawing.Point(36, 75);
            this.pbarjd.Name = "pbarjd";
            this.pbarjd.Size = new System.Drawing.Size(708, 51);
            this.pbarjd.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("楷体", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(33, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(93, 20);
            this.label1.TabIndex = 1;
            this.label1.Text = "当前进度";
            // 
            // lblJD
            // 
            this.lblJD.AutoSize = true;
            this.lblJD.Font = new System.Drawing.Font("楷体", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblJD.Location = new System.Drawing.Point(146, 20);
            this.lblJD.Name = "lblJD";
            this.lblJD.Size = new System.Drawing.Size(0, 20);
            this.lblJD.TabIndex = 2;
            // 
            // FrmxzJDT
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(777, 169);
            this.Controls.Add(this.lblJD);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pbarjd);
            this.Name = "FrmxzJDT";
            this.Text = "下载进度";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.ProgressBar pbarjd;
        private System.Windows.Forms.Label label1;
        public System.Windows.Forms.Label lblJD;
    }
}