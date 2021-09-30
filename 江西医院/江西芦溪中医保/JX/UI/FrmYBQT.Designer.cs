namespace yb_interfaces
{
    partial class FrmYBQT
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
            this.btnybtc = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnybtc
            // 
            this.btnybtc.Location = new System.Drawing.Point(74, 95);
            this.btnybtc.Name = "btnybtc";
            this.btnybtc.Size = new System.Drawing.Size(130, 46);
            this.btnybtc.TabIndex = 0;
            this.btnybtc.Text = "医保签退";
            this.btnybtc.UseVisualStyleBackColor = true;
            this.btnybtc.Click += new System.EventHandler(this.btnybtc_Click);
            // 
            // FrmYBQT
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Controls.Add(this.btnybtc);
            this.Name = "FrmYBQT";
            this.Text = "医保签退";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnybtc;
    }
}