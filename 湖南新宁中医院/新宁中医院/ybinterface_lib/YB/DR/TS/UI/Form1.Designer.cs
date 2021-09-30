namespace ybinterface_lib
{
    partial class Form1
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle10 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle11 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle12 = new System.Windows.Forms.DataGridViewCellStyle();
            this.btn_dgselect = new System.Windows.Forms.Button();
            this.txtDGYS = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.txtYSBM = new System.Windows.Forms.TextBox();
            this.btn_dgdelete = new System.Windows.Forms.Button();
            this.label9 = new System.Windows.Forms.Label();
            this.btn_dginsert = new System.Windows.Forms.Button();
            this.label8 = new System.Windows.Forms.Label();
            this.txtDGYSBM = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.txtYSXM = new System.Windows.Forms.TextBox();
            this.dgv_dgys = new System.Windows.Forms.DataGridView();
            this.col_ysbm = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.col_ysxm = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.col_ybdgysbm = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgv_dgys)).BeginInit();
            this.SuspendLayout();
            // 
            // btn_dgselect
            // 
            this.btn_dgselect.Location = new System.Drawing.Point(196, 5);
            this.btn_dgselect.Name = "btn_dgselect";
            this.btn_dgselect.Size = new System.Drawing.Size(75, 22);
            this.btn_dgselect.TabIndex = 38;
            this.btn_dgselect.Text = "查询";
            this.btn_dgselect.UseVisualStyleBackColor = true;
            this.btn_dgselect.Click += new System.EventHandler(this.btn_dgselect_Click);
            // 
            // txtDGYS
            // 
            this.txtDGYS.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtDGYS.Location = new System.Drawing.Point(24, 6);
            this.txtDGYS.Name = "txtDGYS";
            this.txtDGYS.Size = new System.Drawing.Size(166, 21);
            this.txtDGYS.TabIndex = 37;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(-42, -12);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(59, 12);
            this.label4.TabIndex = 36;
            this.label4.Text = "医生姓名:";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.txtYSBM);
            this.groupBox3.Controls.Add(this.btn_dgdelete);
            this.groupBox3.Controls.Add(this.label9);
            this.groupBox3.Controls.Add(this.btn_dginsert);
            this.groupBox3.Controls.Add(this.label8);
            this.groupBox3.Controls.Add(this.txtDGYSBM);
            this.groupBox3.Controls.Add(this.label7);
            this.groupBox3.Controls.Add(this.txtYSXM);
            this.groupBox3.Location = new System.Drawing.Point(345, 33);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(373, 190);
            this.groupBox3.TabIndex = 35;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "配对";
            // 
            // txtYSBM
            // 
            this.txtYSBM.Location = new System.Drawing.Point(109, 20);
            this.txtYSBM.Name = "txtYSBM";
            this.txtYSBM.ReadOnly = true;
            this.txtYSBM.Size = new System.Drawing.Size(258, 21);
            this.txtYSBM.TabIndex = 25;
            // 
            // btn_dgdelete
            // 
            this.btn_dgdelete.Location = new System.Drawing.Point(219, 140);
            this.btn_dgdelete.Name = "btn_dgdelete";
            this.btn_dgdelete.Size = new System.Drawing.Size(75, 36);
            this.btn_dgdelete.TabIndex = 29;
            this.btn_dgdelete.Text = "配对撤销";
            this.btn_dgdelete.UseVisualStyleBackColor = true;
            this.btn_dgdelete.Click += new System.EventHandler(this.btn_dgdelete_Click);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(40, 23);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(53, 12);
            this.label9.TabIndex = 22;
            this.label9.Text = "医生编码";
            // 
            // btn_dginsert
            // 
            this.btn_dginsert.Location = new System.Drawing.Point(133, 140);
            this.btn_dginsert.Name = "btn_dginsert";
            this.btn_dginsert.Size = new System.Drawing.Size(75, 36);
            this.btn_dginsert.TabIndex = 28;
            this.btn_dginsert.Text = "配对";
            this.btn_dginsert.UseVisualStyleBackColor = true;
            this.btn_dginsert.Click += new System.EventHandler(this.btn_dginsert_Click);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(40, 62);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(53, 12);
            this.label8.TabIndex = 23;
            this.label8.Text = "医生姓名";
            // 
            // txtDGYSBM
            // 
            this.txtDGYSBM.Location = new System.Drawing.Point(109, 99);
            this.txtDGYSBM.Name = "txtDGYSBM";
            this.txtDGYSBM.Size = new System.Drawing.Size(258, 21);
            this.txtDGYSBM.TabIndex = 27;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(12, 102);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(77, 12);
            this.label7.TabIndex = 24;
            this.label7.Text = "定岗医生编码";
            // 
            // txtYSXM
            // 
            this.txtYSXM.Location = new System.Drawing.Point(109, 59);
            this.txtYSXM.Name = "txtYSXM";
            this.txtYSXM.ReadOnly = true;
            this.txtYSXM.Size = new System.Drawing.Size(258, 21);
            this.txtYSXM.TabIndex = 26;
            // 
            // dgv_dgys
            // 
            dataGridViewCellStyle10.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle10.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle10.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle10.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle10.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle10.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle10.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgv_dgys.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle10;
            this.dgv_dgys.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv_dgys.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.col_ysbm,
            this.col_ysxm,
            this.col_ybdgysbm});
            dataGridViewCellStyle11.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle11.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle11.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle11.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle11.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle11.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle11.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgv_dgys.DefaultCellStyle = dataGridViewCellStyle11;
            this.dgv_dgys.Location = new System.Drawing.Point(12, 33);
            this.dgv_dgys.Name = "dgv_dgys";
            dataGridViewCellStyle12.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle12.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle12.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle12.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle12.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle12.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle12.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgv_dgys.RowHeadersDefaultCellStyle = dataGridViewCellStyle12;
            this.dgv_dgys.RowTemplate.Height = 23;
            this.dgv_dgys.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgv_dgys.Size = new System.Drawing.Size(327, 315);
            this.dgv_dgys.TabIndex = 34;
            this.dgv_dgys.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgv_dgys_CellContentClick);
            // 
            // col_ysbm
            // 
            this.col_ysbm.DataPropertyName = "b1empn";
            this.col_ysbm.HeaderText = "人员编码";
            this.col_ysbm.Name = "col_ysbm";
            this.col_ysbm.ReadOnly = true;
            this.col_ysbm.Width = 80;
            // 
            // col_ysxm
            // 
            this.col_ysxm.DataPropertyName = "b1name";
            this.col_ysxm.HeaderText = "人员姓名";
            this.col_ysxm.Name = "col_ysxm";
            this.col_ysxm.ReadOnly = true;
            this.col_ysxm.Width = 80;
            // 
            // col_ybdgysbm
            // 
            this.col_ybdgysbm.DataPropertyName = "dgysbm";
            this.col_ybdgysbm.HeaderText = "国家码";
            this.col_ybdgysbm.Name = "col_ybdgysbm";
            this.col_ybdgysbm.ReadOnly = true;
            this.col_ybdgysbm.Width = 160;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(300, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(55, 14);
            this.label1.TabIndex = 39;
            this.label1.Text = "label1";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(731, 360);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btn_dgselect);
            this.Controls.Add(this.txtDGYS);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.dgv_dgys);
            this.Name = "Form1";
            this.Text = "医生护士国家码对照";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgv_dgys)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btn_dgselect;
        private System.Windows.Forms.TextBox txtDGYS;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TextBox txtYSBM;
        private System.Windows.Forms.Button btn_dgdelete;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Button btn_dginsert;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox txtDGYSBM;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txtYSXM;
        private System.Windows.Forms.DataGridView dgv_dgys;
        private System.Windows.Forms.DataGridViewTextBoxColumn col_ysbm;
        private System.Windows.Forms.DataGridViewTextBoxColumn col_ysxm;
        private System.Windows.Forms.DataGridViewTextBoxColumn col_ybdgysbm;
        private System.Windows.Forms.Label label1;
    }
}