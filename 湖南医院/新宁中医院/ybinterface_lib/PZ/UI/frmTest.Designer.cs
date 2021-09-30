namespace ybinterface_lib
{
    partial class frmTest
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
            this.txtYW = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtINPARAM = new System.Windows.Forms.TextBox();
            this.txtOUTPARAM = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.alvYW = new AutoListView.AutoListView();
            this.btnTEST = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.Color.Red;
            this.label1.Location = new System.Drawing.Point(12, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "业务名称";
            // 
            // txtYW
            // 
            this.txtYW.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtYW.Location = new System.Drawing.Point(72, 8);
            this.txtYW.Name = "txtYW";
            this.txtYW.Size = new System.Drawing.Size(269, 21);
            this.txtYW.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.ForeColor = System.Drawing.Color.Red;
            this.label2.Location = new System.Drawing.Point(36, 60);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(29, 12);
            this.label2.TabIndex = 2;
            this.label2.Text = "入参";
            // 
            // txtINPARAM
            // 
            this.txtINPARAM.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtINPARAM.Location = new System.Drawing.Point(72, 58);
            this.txtINPARAM.Multiline = true;
            this.txtINPARAM.Name = "txtINPARAM";
            this.txtINPARAM.Size = new System.Drawing.Size(749, 175);
            this.txtINPARAM.TabIndex = 3;
            // 
            // txtOUTPARAM
            // 
            this.txtOUTPARAM.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtOUTPARAM.Location = new System.Drawing.Point(72, 251);
            this.txtOUTPARAM.Multiline = true;
            this.txtOUTPARAM.Name = "txtOUTPARAM";
            this.txtOUTPARAM.Size = new System.Drawing.Size(749, 175);
            this.txtOUTPARAM.TabIndex = 5;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.ForeColor = System.Drawing.Color.Red;
            this.label3.Location = new System.Drawing.Point(36, 253);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(29, 12);
            this.label3.TabIndex = 4;
            this.label3.Text = "出参";
            // 
            // alvYW
            // 
            this.alvYW.AttachTextBox = this.txtYW;
            this.alvYW.ChooseInfoDataSet = null;
            this.alvYW.DataViewFont = null;
            this.alvYW.DataViewFontColor = System.Drawing.Color.Navy;
            this.alvYW.DataViewMaxSize = new System.Drawing.Size(240, 150);
            this.alvYW.DeleteTextWhenNull = true;
            this.alvYW.Enable = true;
            this.alvYW.FilterString = "";
            this.alvYW.IsGetAllRecord = true;
            this.alvYW.ParentControl = this;
            this.alvYW.RelationEnable = true;
            this.alvYW.Relations = null;
            this.alvYW.Remark = null;
            this.alvYW.SetColumnParam.QueryColumn = "";
            this.alvYW.SetColumnParam.TextColumn = "";
            this.alvYW.SetColumnParam.ValueColumn = "";
            this.alvYW.SetColumnParam.ViewColumn = "";
            this.alvYW.TargetColumn = 0;
            this.alvYW.UpdateSourceWhenEnter = false;
            this.alvYW.UseMulThread = false;
            this.alvYW.WhereStr = null;
            // 
            // btnTEST
            // 
            this.btnTEST.ForeColor = System.Drawing.Color.Red;
            this.btnTEST.Location = new System.Drawing.Point(348, 7);
            this.btnTEST.Name = "btnTEST";
            this.btnTEST.Size = new System.Drawing.Size(117, 23);
            this.btnTEST.TabIndex = 6;
            this.btnTEST.Text = "业务测试";
            this.btnTEST.UseVisualStyleBackColor = true;
            this.btnTEST.Click += new System.EventHandler(this.btnTEST_Click);
            // 
            // frmTest
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(833, 439);
            this.Controls.Add(this.btnTEST);
            this.Controls.Add(this.txtOUTPARAM);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtINPARAM);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtYW);
            this.Controls.Add(this.label1);
            this.Name = "frmTest";
            this.Text = "医保接口测试";
            this.Load += new System.EventHandler(this.frmTest_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtYW;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtINPARAM;
        private System.Windows.Forms.TextBox txtOUTPARAM;
        private System.Windows.Forms.Label label3;
        private AutoListView.AutoListView alvYW;
        private System.Windows.Forms.Button btnTEST;
    }
}