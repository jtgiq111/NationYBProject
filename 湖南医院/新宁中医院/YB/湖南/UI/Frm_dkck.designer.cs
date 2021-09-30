namespace yb_interfaces
{
    partial class Frm_dkck
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.btncx = new System.Windows.Forms.Button();
            this.rbsfz = new System.Windows.Forms.RadioButton();
            this.rbtndzk = new System.Windows.Forms.RadioButton();
            this.txtCode = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.rbsbk = new System.Windows.Forms.RadioButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.txtidcard = new System.Windows.Forms.TextBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btncx
            // 
            this.btncx.Location = new System.Drawing.Point(171, 271);
            this.btncx.Name = "btncx";
            this.btncx.Size = new System.Drawing.Size(75, 34);
            this.btncx.TabIndex = 215;
            this.btncx.Text = "确认";
            this.btncx.UseVisualStyleBackColor = true;
            this.btncx.Click += new System.EventHandler(this.btncx_Click);
            // 
            // rbsfz
            // 
            this.rbsfz.AutoSize = true;
            this.rbsfz.Checked = true;
            this.rbsfz.Location = new System.Drawing.Point(5, 32);
            this.rbsfz.Name = "rbsfz";
            this.rbsfz.Size = new System.Drawing.Size(59, 16);
            this.rbsfz.TabIndex = 202;
            this.rbsfz.TabStop = true;
            this.rbsfz.Tag = "";
            this.rbsfz.Text = "身份证";
            this.rbsfz.UseVisualStyleBackColor = true;
            // 
            // rbtndzk
            // 
            this.rbtndzk.AutoSize = true;
            this.rbtndzk.Location = new System.Drawing.Point(5, 86);
            this.rbtndzk.Name = "rbtndzk";
            this.rbtndzk.Size = new System.Drawing.Size(83, 16);
            this.rbtndzk.TabIndex = 204;
            this.rbtndzk.Tag = "";
            this.rbtndzk.Text = "电子医保卡";
            this.rbtndzk.UseVisualStyleBackColor = true;
            this.rbtndzk.CheckedChanged += new System.EventHandler(this.rbtndzk_CheckedChanged);
            // 
            // txtCode
            // 
            this.txtCode.Location = new System.Drawing.Point(108, 83);
            this.txtCode.Name = "txtCode";
            this.txtCode.Size = new System.Drawing.Size(232, 21);
            this.txtCode.TabIndex = 216;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("宋体", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.ForeColor = System.Drawing.Color.Red;
            this.label1.Location = new System.Drawing.Point(143, 89);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(185, 24);
            this.label1.TabIndex = 217;
            this.label1.Text = "请选择读卡方式";
            // 
            // rbsbk
            // 
            this.rbsbk.AutoSize = true;
            this.rbsbk.Location = new System.Drawing.Point(266, 32);
            this.rbsbk.Name = "rbsbk";
            this.rbsbk.Size = new System.Drawing.Size(59, 16);
            this.rbsbk.TabIndex = 218;
            this.rbsbk.Tag = "";
            this.rbsbk.Text = "社保卡";
            this.rbsbk.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.txtidcard);
            this.groupBox1.Controls.Add(this.rbsfz);
            this.groupBox1.Controls.Add(this.rbsbk);
            this.groupBox1.Controls.Add(this.rbtndzk);
            this.groupBox1.Controls.Add(this.txtCode);
            this.groupBox1.Location = new System.Drawing.Point(56, 122);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.groupBox1.Size = new System.Drawing.Size(350, 126);
            this.groupBox1.TabIndex = 219;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "读卡类型选择";
            // 
            // txtidcard
            // 
            this.txtidcard.Location = new System.Drawing.Point(71, 32);
            this.txtidcard.Name = "txtidcard";
            this.txtidcard.Size = new System.Drawing.Size(164, 21);
            this.txtidcard.TabIndex = 219;
            // 
            // Frm_dkck
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(465, 391);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btncx);
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Frm_dkck";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "读卡选择";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Frm_dkck_FormClosed);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btncx;
        private System.Windows.Forms.RadioButton rbsfz;
        private System.Windows.Forms.RadioButton rbtndzk;
        private System.Windows.Forms.TextBox txtCode;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RadioButton rbsbk;
        private System.Windows.Forms.GroupBox groupBox1;
        public System.Windows.Forms.TextBox txtidcard;
    }
}

