
namespace ybinterface_lib
{
    partial class FrmZZ
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
            this.label1 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.kssj = new System.Windows.Forms.DateTimePicker();
            this.jssj = new System.Windows.Forms.DateTimePicker();
            this.label5 = new System.Windows.Forms.Label();
            this.hosname = new System.Windows.Forms.TextBox();
            this.quhua = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.jtgj = new System.Windows.Forms.TextBox();
            this.ssqx = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.sqly = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(61, 115);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(77, 12);
            this.label1.TabIndex = 4;
            this.label1.Text = "转院开始时间";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(414, 115);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(77, 12);
            this.label4.TabIndex = 5;
            this.label4.Text = "转院终止时间";
            // 
            // kssj
            // 
            this.kssj.CustomFormat = "yyyy-MM-dd HH:mm:ss";
            this.kssj.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.kssj.Location = new System.Drawing.Point(160, 109);
            this.kssj.Name = "kssj";
            this.kssj.Size = new System.Drawing.Size(159, 21);
            this.kssj.TabIndex = 6;
            // 
            // jssj
            // 
            this.jssj.CustomFormat = "yyyy-MM-dd HH:mm:ss";
            this.jssj.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.jssj.Location = new System.Drawing.Point(507, 108);
            this.jssj.Name = "jssj";
            this.jssj.Size = new System.Drawing.Size(159, 21);
            this.jssj.TabIndex = 7;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(61, 180);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(77, 12);
            this.label5.TabIndex = 8;
            this.label5.Text = "转外医院名称";
            // 
            // hosname
            // 
            this.hosname.Location = new System.Drawing.Point(160, 177);
            this.hosname.Name = "hosname";
            this.hosname.Size = new System.Drawing.Size(100, 21);
            this.hosname.TabIndex = 9;
            // 
            // quhua
            // 
            this.quhua.Location = new System.Drawing.Point(507, 175);
            this.quhua.Name = "quhua";
            this.quhua.Size = new System.Drawing.Size(100, 21);
            this.quhua.TabIndex = 10;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(414, 180);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(53, 12);
            this.label6.TabIndex = 11;
            this.label6.Text = "转外区划";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(414, 236);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(53, 12);
            this.label7.TabIndex = 15;
            this.label7.Text = "交通工具";
            // 
            // jtgj
            // 
            this.jtgj.Location = new System.Drawing.Point(507, 231);
            this.jtgj.Name = "jtgj";
            this.jtgj.Size = new System.Drawing.Size(100, 21);
            this.jtgj.TabIndex = 14;
            // 
            // ssqx
            // 
            this.ssqx.Location = new System.Drawing.Point(160, 233);
            this.ssqx.Name = "ssqx";
            this.ssqx.Size = new System.Drawing.Size(100, 21);
            this.ssqx.TabIndex = 17;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(61, 236);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(53, 12);
            this.label10.TabIndex = 16;
            this.label10.Text = "所属区县";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(313, 410);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 20;
            this.button1.Text = "转诊上传";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // sqly
            // 
            this.sqly.Location = new System.Drawing.Point(145, 285);
            this.sqly.Multiline = true;
            this.sqly.Name = "sqly";
            this.sqly.Size = new System.Drawing.Size(232, 74);
            this.sqly.TabIndex = 22;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(62, 288);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(53, 12);
            this.label8.TabIndex = 21;
            this.label8.Text = "申请理由";
            // 
            // FrmZZ
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 523);
            this.Controls.Add(this.sqly);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.ssqx);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.jtgj);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.quhua);
            this.Controls.Add(this.hosname);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.jssj);
            this.Controls.Add(this.kssj);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label1);
            this.Name = "FrmZZ";
            this.Text = "cccc";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.DateTimePicker kssj;
        private System.Windows.Forms.DateTimePicker jssj;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox hosname;
        private System.Windows.Forms.TextBox quhua;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox jtgj;
        private System.Windows.Forms.TextBox ssqx;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox sqly;
        private System.Windows.Forms.Label label8;
    }
}