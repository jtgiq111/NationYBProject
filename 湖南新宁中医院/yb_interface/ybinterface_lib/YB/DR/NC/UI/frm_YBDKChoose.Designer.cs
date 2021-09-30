namespace ybinterface_lib
{
    partial class frm_YBDKChoose
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
            this.ck_ybklx1 = new System.Windows.Forms.CheckBox();
            this.ck_ydybk1 = new System.Windows.Forms.CheckBox();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // ck_ybklx1
            // 
            this.ck_ybklx1.AutoSize = true;
            this.ck_ybklx1.ForeColor = System.Drawing.Color.Red;
            this.ck_ybklx1.Location = new System.Drawing.Point(12, 12);
            this.ck_ybklx1.Name = "ck_ybklx1";
            this.ck_ybklx1.Size = new System.Drawing.Size(96, 18);
            this.ck_ybklx1.TabIndex = 0;
            this.ck_ybklx1.Text = "临时医保卡";
            this.ck_ybklx1.UseVisualStyleBackColor = true;
            // 
            // ck_ydybk1
            // 
            this.ck_ydybk1.AutoSize = true;
            this.ck_ydybk1.ForeColor = System.Drawing.Color.Red;
            this.ck_ydybk1.Location = new System.Drawing.Point(130, 12);
            this.ck_ydybk1.Name = "ck_ydybk1";
            this.ck_ydybk1.Size = new System.Drawing.Size(96, 18);
            this.ck_ydybk1.TabIndex = 1;
            this.ck_ydybk1.Text = "异地医保卡";
            this.ck_ydybk1.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(86, 41);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(83, 29);
            this.button1.TabIndex = 2;
            this.button1.Text = "确定";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // frm_YBDKChoose
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(244, 72);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.ck_ydybk1);
            this.Controls.Add(this.ck_ybklx1);
            this.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "frm_YBDKChoose";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "医保读卡";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox ck_ybklx1;
        private System.Windows.Forms.CheckBox ck_ydybk1;
        private System.Windows.Forms.Button button1;
    }
}