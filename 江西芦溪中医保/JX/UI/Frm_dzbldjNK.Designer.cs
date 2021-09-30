namespace yb_interfaces.JX.UI
{
    partial class Frm_dzbldjNK
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
            this.labZYNO = new System.Windows.Forms.Label();
            this.txtZyno = new System.Windows.Forms.TextBox();
            this.lab1 = new System.Windows.Forms.Label();
            this.lab2 = new System.Windows.Forms.Label();
            this.lab3 = new System.Windows.Forms.Label();
            this.lab4 = new System.Windows.Forms.Label();
            this.labXM = new System.Windows.Forms.Label();
            this.labKH = new System.Windows.Forms.Label();
            this.labLSH = new System.Windows.Forms.Label();
            this.labBXH = new System.Windows.Forms.Label();
            this.btnDJ = new System.Windows.Forms.Button();
            this.btnQUIT = new System.Windows.Forms.Button();
            this.btnCX = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // labZYNO
            // 
            this.labZYNO.AutoSize = true;
            this.labZYNO.Location = new System.Drawing.Point(28, 17);
            this.labZYNO.Name = "labZYNO";
            this.labZYNO.Size = new System.Drawing.Size(65, 12);
            this.labZYNO.TabIndex = 0;
            this.labZYNO.Text = "住院病案号";
            // 
            // txtZyno
            // 
            this.txtZyno.Location = new System.Drawing.Point(99, 13);
            this.txtZyno.Name = "txtZyno";
            this.txtZyno.Size = new System.Drawing.Size(197, 21);
            this.txtZyno.TabIndex = 1;
            this.txtZyno.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtZyno_KeyDown);
            // 
            // lab1
            // 
            this.lab1.AutoSize = true;
            this.lab1.Location = new System.Drawing.Point(50, 66);
            this.lab1.Name = "lab1";
            this.lab1.Size = new System.Drawing.Size(29, 12);
            this.lab1.TabIndex = 2;
            this.lab1.Text = "姓名";
            // 
            // lab2
            // 
            this.lab2.AutoSize = true;
            this.lab2.Location = new System.Drawing.Point(50, 104);
            this.lab2.Name = "lab2";
            this.lab2.Size = new System.Drawing.Size(53, 12);
            this.lab2.TabIndex = 3;
            this.lab2.Text = "医保卡号";
            // 
            // lab3
            // 
            this.lab3.AutoSize = true;
            this.lab3.Location = new System.Drawing.Point(230, 66);
            this.lab3.Name = "lab3";
            this.lab3.Size = new System.Drawing.Size(65, 12);
            this.lab3.TabIndex = 4;
            this.lab3.Text = "医保流水号";
            // 
            // lab4
            // 
            this.lab4.AutoSize = true;
            this.lab4.Location = new System.Drawing.Point(230, 104);
            this.lab4.Name = "lab4";
            this.lab4.Size = new System.Drawing.Size(65, 12);
            this.lab4.TabIndex = 5;
            this.lab4.Text = "医保保险号";
            // 
            // labXM
            // 
            this.labXM.AutoSize = true;
            this.labXM.ForeColor = System.Drawing.Color.Red;
            this.labXM.Location = new System.Drawing.Point(113, 66);
            this.labXM.Name = "labXM";
            this.labXM.Size = new System.Drawing.Size(0, 12);
            this.labXM.TabIndex = 6;
            // 
            // labKH
            // 
            this.labKH.AutoSize = true;
            this.labKH.ForeColor = System.Drawing.Color.Red;
            this.labKH.Location = new System.Drawing.Point(113, 104);
            this.labKH.Name = "labKH";
            this.labKH.Size = new System.Drawing.Size(0, 12);
            this.labKH.TabIndex = 7;
            // 
            // labLSH
            // 
            this.labLSH.AutoSize = true;
            this.labLSH.ForeColor = System.Drawing.Color.Red;
            this.labLSH.Location = new System.Drawing.Point(311, 66);
            this.labLSH.Name = "labLSH";
            this.labLSH.Size = new System.Drawing.Size(0, 12);
            this.labLSH.TabIndex = 8;
            // 
            // labBXH
            // 
            this.labBXH.AutoSize = true;
            this.labBXH.ForeColor = System.Drawing.Color.Red;
            this.labBXH.Location = new System.Drawing.Point(311, 104);
            this.labBXH.Name = "labBXH";
            this.labBXH.Size = new System.Drawing.Size(0, 12);
            this.labBXH.TabIndex = 9;
            // 
            // btnDJ
            // 
            this.btnDJ.Location = new System.Drawing.Point(29, 148);
            this.btnDJ.Name = "btnDJ";
            this.btnDJ.Size = new System.Drawing.Size(126, 30);
            this.btnDJ.TabIndex = 10;
            this.btnDJ.Text = "医保刷卡登记病案";
            this.btnDJ.UseVisualStyleBackColor = true;
            this.btnDJ.Click += new System.EventHandler(this.btnDJ_Click);
            // 
            // btnQUIT
            // 
            this.btnQUIT.Location = new System.Drawing.Point(290, 148);
            this.btnQUIT.Name = "btnQUIT";
            this.btnQUIT.Size = new System.Drawing.Size(75, 30);
            this.btnQUIT.TabIndex = 11;
            this.btnQUIT.Text = "退出";
            this.btnQUIT.UseVisualStyleBackColor = true;
            this.btnQUIT.Click += new System.EventHandler(this.btnQUIT_Click);
            // 
            // btnCX
            // 
            this.btnCX.Location = new System.Drawing.Point(158, 148);
            this.btnCX.Name = "btnCX";
            this.btnCX.Size = new System.Drawing.Size(126, 30);
            this.btnCX.TabIndex = 12;
            this.btnCX.Text = "医保刷卡撤销病案";
            this.btnCX.UseVisualStyleBackColor = true;
            this.btnCX.Click += new System.EventHandler(this.btnCX_Click);
            // 
            // Frm_dzbldjXG
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(421, 209);
            this.Controls.Add(this.btnCX);
            this.Controls.Add(this.btnQUIT);
            this.Controls.Add(this.btnDJ);
            this.Controls.Add(this.labBXH);
            this.Controls.Add(this.labLSH);
            this.Controls.Add(this.labKH);
            this.Controls.Add(this.labXM);
            this.Controls.Add(this.lab4);
            this.Controls.Add(this.lab3);
            this.Controls.Add(this.lab2);
            this.Controls.Add(this.lab1);
            this.Controls.Add(this.txtZyno);
            this.Controls.Add(this.labZYNO);
            this.Name = "Frm_dzbldjXG";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "电子病历登记";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labZYNO;
        private System.Windows.Forms.TextBox txtZyno;
        private System.Windows.Forms.Label lab1;
        private System.Windows.Forms.Label lab2;
        private System.Windows.Forms.Label lab3;
        private System.Windows.Forms.Label lab4;
        private System.Windows.Forms.Label labXM;
        private System.Windows.Forms.Label labKH;
        private System.Windows.Forms.Label labLSH;
        private System.Windows.Forms.Label labBXH;
        private System.Windows.Forms.Button btnDJ;
        private System.Windows.Forms.Button btnQUIT;
        private System.Windows.Forms.Button btnCX;
    }
}