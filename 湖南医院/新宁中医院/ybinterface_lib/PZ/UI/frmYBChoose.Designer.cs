namespace ybinterface_lib
{
    partial class frmYBChoose
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
            this.btnStart = new System.Windows.Forms.Button();
            this.btnCANCEL = new System.Windows.Forms.Button();
            this.txtYBCS = new System.Windows.Forms.TextBox();
            this.alvCS = new AutoListView.AutoListView();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.ForeColor = System.Drawing.Color.Red;
            this.label1.Location = new System.Drawing.Point(66, 26);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(129, 19);
            this.label1.TabIndex = 0;
            this.label1.Text = "启用医保切换";
            // 
            // btnStart
            // 
            this.btnStart.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnStart.Location = new System.Drawing.Point(44, 115);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(75, 32);
            this.btnStart.TabIndex = 3;
            this.btnStart.Text = "确  定";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // btnCANCEL
            // 
            this.btnCANCEL.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnCANCEL.Location = new System.Drawing.Point(137, 115);
            this.btnCANCEL.Name = "btnCANCEL";
            this.btnCANCEL.Size = new System.Drawing.Size(75, 32);
            this.btnCANCEL.TabIndex = 4;
            this.btnCANCEL.Text = "取  消";
            this.btnCANCEL.UseVisualStyleBackColor = true;
            this.btnCANCEL.Click += new System.EventHandler(this.btnCANCEL_Click);
            // 
            // txtYBCS
            // 
            this.txtYBCS.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtYBCS.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txtYBCS.Location = new System.Drawing.Point(12, 67);
            this.txtYBCS.Name = "txtYBCS";
            this.txtYBCS.Size = new System.Drawing.Size(230, 23);
            this.txtYBCS.TabIndex = 5;
            // 
            // alvCS
            // 
            this.alvCS.AttachTextBox = this.txtYBCS;
            this.alvCS.ChooseInfoDataSet = null;
            this.alvCS.DataViewFont = null;
            this.alvCS.DataViewFontColor = System.Drawing.Color.Navy;
            this.alvCS.DataViewMaxSize = new System.Drawing.Size(240, 150);
            this.alvCS.DeleteTextWhenNull = true;
            this.alvCS.Enable = true;
            this.alvCS.FilterString = "";
            this.alvCS.IsGetAllRecord = true;
            this.alvCS.ParentControl = this;
            this.alvCS.RelationEnable = true;
            this.alvCS.Relations = null;
            this.alvCS.Remark = null;
            this.alvCS.SetColumnParam.QueryColumn = "";
            this.alvCS.SetColumnParam.TextColumn = "";
            this.alvCS.SetColumnParam.ValueColumn = "";
            this.alvCS.SetColumnParam.ViewColumn = "";
            this.alvCS.TargetColumn = 0;
            this.alvCS.UpdateSourceWhenEnter = false;
            this.alvCS.UseMulThread = false;
            this.alvCS.WhereStr = null;
            this.alvCS.AfterConfirm += new AutoListView.AutoListView.AfterConfirmHandle(this.alvCS_AfterConfirm);
            // 
            // frmYBChoose
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(254, 170);
            this.Controls.Add(this.txtYBCS);
            this.Controls.Add(this.btnCANCEL);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "frmYBChoose";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "启用医保切换";
            this.Load += new System.EventHandler(this.frmYBChoose_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Button btnCANCEL;
        private System.Windows.Forms.TextBox txtYBCS;
        private AutoListView.AutoListView alvCS;
    }
}