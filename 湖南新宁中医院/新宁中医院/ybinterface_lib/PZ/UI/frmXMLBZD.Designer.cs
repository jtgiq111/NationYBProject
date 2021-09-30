namespace ybinterface_lib
{
    partial class frmXMLBZD
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
            this.panel2 = new System.Windows.Forms.Panel();
            this.label10 = new System.Windows.Forms.Label();
            this.btnUPDATE = new System.Windows.Forms.Button();
            this.cmbDQ = new System.Windows.Forms.ComboBox();
            this.label9 = new System.Windows.Forms.Label();
            this.cmbYBCS = new System.Windows.Forms.ComboBox();
            this.label8 = new System.Windows.Forms.Label();
            this.txtXMMC = new System.Windows.Forms.TextBox();
            this.btnDEL = new System.Windows.Forms.Button();
            this.btnADD = new System.Windows.Forms.Button();
            this.txtBZ = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.rabtnQY = new System.Windows.Forms.RadioButton();
            this.rabtnBQY = new System.Windows.Forms.RadioButton();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.txtXMDM = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtXMLB = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label7 = new System.Windows.Forms.Label();
            this.cmbXMLB = new System.Windows.Forms.ComboBox();
            this.btnSelect = new System.Windows.Forms.Button();
            this.txtXMJS = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.dgvXMLBZD = new System.Windows.Forms.DataGridView();
            this.col_sequ = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.col_ID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.col_lbmc = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.col_code = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.col_name = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.col_cs1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.col_cs = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.col_dq1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.col_dq = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.col_qybz = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.col_pym = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.col_wbm = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.col_bz = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.panel2.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvXMLBZD)).BeginInit();
            this.SuspendLayout();
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.label10);
            this.panel2.Controls.Add(this.btnUPDATE);
            this.panel2.Controls.Add(this.cmbDQ);
            this.panel2.Controls.Add(this.label9);
            this.panel2.Controls.Add(this.cmbYBCS);
            this.panel2.Controls.Add(this.label8);
            this.panel2.Controls.Add(this.txtXMMC);
            this.panel2.Controls.Add(this.btnDEL);
            this.panel2.Controls.Add(this.btnADD);
            this.panel2.Controls.Add(this.txtBZ);
            this.panel2.Controls.Add(this.label6);
            this.panel2.Controls.Add(this.groupBox2);
            this.panel2.Controls.Add(this.label5);
            this.panel2.Controls.Add(this.label4);
            this.panel2.Controls.Add(this.txtXMDM);
            this.panel2.Controls.Add(this.label3);
            this.panel2.Controls.Add(this.txtXMLB);
            this.panel2.Controls.Add(this.label2);
            this.panel2.Controls.Add(this.linkLabel1);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel2.Location = new System.Drawing.Point(799, 39);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(320, 467);
            this.panel2.TabIndex = 3;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label10.ForeColor = System.Drawing.Color.Red;
            this.label10.Location = new System.Drawing.Point(8, 400);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(305, 12);
            this.label10.TabIndex = 26;
            this.label10.Text = "注:修改功能不针对项目类别、项目代码的修改操作!";
            // 
            // btnUPDATE
            // 
            this.btnUPDATE.Location = new System.Drawing.Point(129, 336);
            this.btnUPDATE.Name = "btnUPDATE";
            this.btnUPDATE.Size = new System.Drawing.Size(75, 23);
            this.btnUPDATE.TabIndex = 25;
            this.btnUPDATE.Text = "修　改";
            this.btnUPDATE.UseVisualStyleBackColor = true;
            this.btnUPDATE.Visible = false;
            this.btnUPDATE.Click += new System.EventHandler(this.btnUPDATE_Click);
            // 
            // cmbDQ
            // 
            this.cmbDQ.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbDQ.FormattingEnabled = true;
            this.cmbDQ.Location = new System.Drawing.Point(76, 164);
            this.cmbDQ.Name = "cmbDQ";
            this.cmbDQ.Size = new System.Drawing.Size(232, 20);
            this.cmbDQ.TabIndex = 24;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label9.Location = new System.Drawing.Point(7, 166);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(70, 14);
            this.label9.TabIndex = 23;
            this.label9.Text = "适用地区:";
            // 
            // cmbYBCS
            // 
            this.cmbYBCS.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbYBCS.FormattingEnabled = true;
            this.cmbYBCS.Location = new System.Drawing.Point(76, 130);
            this.cmbYBCS.Name = "cmbYBCS";
            this.cmbYBCS.Size = new System.Drawing.Size(232, 20);
            this.cmbYBCS.TabIndex = 22;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label8.Location = new System.Drawing.Point(7, 132);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(70, 14);
            this.label8.TabIndex = 21;
            this.label8.Text = "适用医保:";
            // 
            // txtXMMC
            // 
            this.txtXMMC.Location = new System.Drawing.Point(76, 101);
            this.txtXMMC.Name = "txtXMMC";
            this.txtXMMC.Size = new System.Drawing.Size(232, 21);
            this.txtXMMC.TabIndex = 20;
            // 
            // btnDEL
            // 
            this.btnDEL.Location = new System.Drawing.Point(219, 336);
            this.btnDEL.Name = "btnDEL";
            this.btnDEL.Size = new System.Drawing.Size(75, 23);
            this.btnDEL.TabIndex = 19;
            this.btnDEL.Text = "删 除";
            this.btnDEL.UseVisualStyleBackColor = true;
            this.btnDEL.Click += new System.EventHandler(this.btnDEL_Click);
            // 
            // btnADD
            // 
            this.btnADD.Location = new System.Drawing.Point(39, 336);
            this.btnADD.Name = "btnADD";
            this.btnADD.Size = new System.Drawing.Size(75, 23);
            this.btnADD.TabIndex = 3;
            this.btnADD.Text = "新 增";
            this.btnADD.UseVisualStyleBackColor = true;
            this.btnADD.Click += new System.EventHandler(this.btnADD_Click);
            // 
            // txtBZ
            // 
            this.txtBZ.Location = new System.Drawing.Point(77, 240);
            this.txtBZ.Multiline = true;
            this.txtBZ.Name = "txtBZ";
            this.txtBZ.Size = new System.Drawing.Size(232, 90);
            this.txtBZ.TabIndex = 17;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label6.Location = new System.Drawing.Point(8, 291);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(70, 14);
            this.label6.TabIndex = 16;
            this.label6.Text = "备注说明:";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.rabtnQY);
            this.groupBox2.Controls.Add(this.rabtnBQY);
            this.groupBox2.Location = new System.Drawing.Point(76, 190);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(232, 44);
            this.groupBox2.TabIndex = 14;
            this.groupBox2.TabStop = false;
            // 
            // rabtnQY
            // 
            this.rabtnQY.AutoSize = true;
            this.rabtnQY.Checked = true;
            this.rabtnQY.Location = new System.Drawing.Point(13, 16);
            this.rabtnQY.Name = "rabtnQY";
            this.rabtnQY.Size = new System.Drawing.Size(47, 16);
            this.rabtnQY.TabIndex = 11;
            this.rabtnQY.TabStop = true;
            this.rabtnQY.Text = "启用";
            this.rabtnQY.UseVisualStyleBackColor = true;
            // 
            // rabtnBQY
            // 
            this.rabtnBQY.AutoSize = true;
            this.rabtnBQY.Location = new System.Drawing.Point(81, 16);
            this.rabtnBQY.Name = "rabtnBQY";
            this.rabtnBQY.Size = new System.Drawing.Size(59, 16);
            this.rabtnBQY.TabIndex = 12;
            this.rabtnBQY.Text = "不启用";
            this.rabtnBQY.UseVisualStyleBackColor = true;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label5.Location = new System.Drawing.Point(7, 208);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(70, 14);
            this.label5.TabIndex = 10;
            this.label5.Text = "是否启用:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label4.Location = new System.Drawing.Point(7, 102);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(70, 14);
            this.label4.TabIndex = 6;
            this.label4.Text = "项目名称:";
            // 
            // txtXMDM
            // 
            this.txtXMDM.Location = new System.Drawing.Point(76, 70);
            this.txtXMDM.Name = "txtXMDM";
            this.txtXMDM.Size = new System.Drawing.Size(232, 21);
            this.txtXMDM.TabIndex = 5;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label3.ForeColor = System.Drawing.Color.Red;
            this.label3.Location = new System.Drawing.Point(7, 75);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(70, 14);
            this.label3.TabIndex = 4;
            this.label3.Text = "项目代码:";
            // 
            // txtXMLB
            // 
            this.txtXMLB.Location = new System.Drawing.Point(76, 41);
            this.txtXMLB.Name = "txtXMLB";
            this.txtXMLB.Size = new System.Drawing.Size(232, 21);
            this.txtXMLB.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.ForeColor = System.Drawing.Color.Red;
            this.label2.Location = new System.Drawing.Point(7, 44);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(70, 14);
            this.label2.TabIndex = 1;
            this.label2.Text = "项目类别:";
            // 
            // linkLabel1
            // 
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.Location = new System.Drawing.Point(7, 7);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(107, 12);
            this.linkLabel1.TabIndex = 0;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = " 医保项目类别信息";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.label7);
            this.panel1.Controls.Add(this.cmbXMLB);
            this.panel1.Controls.Add(this.btnSelect);
            this.panel1.Controls.Add(this.txtXMJS);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1119, 39);
            this.panel1.TabIndex = 2;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label7.Location = new System.Drawing.Point(253, 11);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(63, 14);
            this.label7.TabIndex = 4;
            this.label7.Text = "项目检索";
            // 
            // cmbXMLB
            // 
            this.cmbXMLB.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbXMLB.FormattingEnabled = true;
            this.cmbXMLB.Location = new System.Drawing.Point(84, 8);
            this.cmbXMLB.Name = "cmbXMLB";
            this.cmbXMLB.Size = new System.Drawing.Size(149, 20);
            this.cmbXMLB.TabIndex = 3;
            this.cmbXMLB.SelectedIndexChanged += new System.EventHandler(this.cmbXMLB_SelectedIndexChanged);
            // 
            // btnSelect
            // 
            this.btnSelect.Location = new System.Drawing.Point(517, 8);
            this.btnSelect.Name = "btnSelect";
            this.btnSelect.Size = new System.Drawing.Size(75, 23);
            this.btnSelect.TabIndex = 2;
            this.btnSelect.Text = "查 询";
            this.btnSelect.UseVisualStyleBackColor = true;
            this.btnSelect.Click += new System.EventHandler(this.btnSelect_Click);
            // 
            // txtXMJS
            // 
            this.txtXMJS.ForeColor = System.Drawing.SystemColors.ControlLight;
            this.txtXMJS.Location = new System.Drawing.Point(322, 8);
            this.txtXMJS.Name = "txtXMJS";
            this.txtXMJS.Size = new System.Drawing.Size(189, 21);
            this.txtXMJS.TabIndex = 1;
            this.txtXMJS.Click += new System.EventHandler(this.txtXMJS_Click);
            this.txtXMJS.TextChanged += new System.EventHandler(this.txtXMJS_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(14, 11);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(63, 14);
            this.label1.TabIndex = 0;
            this.label1.Text = "项目类别";
            // 
            // dgvXMLBZD
            // 
            this.dgvXMLBZD.AllowUserToAddRows = false;
            this.dgvXMLBZD.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvXMLBZD.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.col_sequ,
            this.col_ID,
            this.col_lbmc,
            this.col_code,
            this.col_name,
            this.col_cs1,
            this.col_cs,
            this.col_dq1,
            this.col_dq,
            this.col_qybz,
            this.col_pym,
            this.col_wbm,
            this.col_bz});
            this.dgvXMLBZD.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvXMLBZD.Location = new System.Drawing.Point(0, 39);
            this.dgvXMLBZD.Name = "dgvXMLBZD";
            this.dgvXMLBZD.RowHeadersVisible = false;
            this.dgvXMLBZD.RowTemplate.Height = 23;
            this.dgvXMLBZD.Size = new System.Drawing.Size(799, 467);
            this.dgvXMLBZD.TabIndex = 5;
            this.dgvXMLBZD.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvXMLBZD_CellContentClick);
            // 
            // col_sequ
            // 
            this.col_sequ.HeaderText = "序号";
            this.col_sequ.Name = "col_sequ";
            this.col_sequ.ReadOnly = true;
            this.col_sequ.Width = 60;
            // 
            // col_ID
            // 
            this.col_ID.DataPropertyName = "ID";
            this.col_ID.HeaderText = "ID";
            this.col_ID.Name = "col_ID";
            this.col_ID.ReadOnly = true;
            this.col_ID.Visible = false;
            this.col_ID.Width = 60;
            // 
            // col_lbmc
            // 
            this.col_lbmc.DataPropertyName = "LBMC";
            this.col_lbmc.HeaderText = "项目类别";
            this.col_lbmc.Name = "col_lbmc";
            this.col_lbmc.ReadOnly = true;
            // 
            // col_code
            // 
            this.col_code.DataPropertyName = "CODE";
            this.col_code.HeaderText = "代码";
            this.col_code.Name = "col_code";
            this.col_code.ReadOnly = true;
            this.col_code.Width = 60;
            // 
            // col_name
            // 
            this.col_name.DataPropertyName = "NAME";
            this.col_name.HeaderText = "名称";
            this.col_name.Name = "col_name";
            this.col_name.ReadOnly = true;
            this.col_name.Width = 120;
            // 
            // col_cs1
            // 
            this.col_cs1.DataPropertyName = "CSMC";
            this.col_cs1.HeaderText = "医保接口";
            this.col_cs1.Name = "col_cs1";
            this.col_cs1.ReadOnly = true;
            this.col_cs1.Width = 120;
            // 
            // col_cs
            // 
            this.col_cs.DataPropertyName = "CSDM";
            this.col_cs.HeaderText = "厂商代码";
            this.col_cs.Name = "col_cs";
            this.col_cs.ReadOnly = true;
            this.col_cs.Visible = false;
            // 
            // col_dq1
            // 
            this.col_dq1.DataPropertyName = "DQMC";
            this.col_dq1.HeaderText = "适用地区";
            this.col_dq1.Name = "col_dq1";
            this.col_dq1.ReadOnly = true;
            this.col_dq1.Width = 120;
            // 
            // col_dq
            // 
            this.col_dq.DataPropertyName = "DQDM";
            this.col_dq.HeaderText = "地区编码";
            this.col_dq.Name = "col_dq";
            this.col_dq.ReadOnly = true;
            this.col_dq.Visible = false;
            // 
            // col_qybz
            // 
            this.col_qybz.DataPropertyName = "QYBZ";
            this.col_qybz.FalseValue = "0";
            this.col_qybz.HeaderText = "启用标志";
            this.col_qybz.Name = "col_qybz";
            this.col_qybz.ReadOnly = true;
            this.col_qybz.TrueValue = "1";
            this.col_qybz.Width = 80;
            // 
            // col_pym
            // 
            this.col_pym.DataPropertyName = "PYM";
            this.col_pym.HeaderText = "拼音码";
            this.col_pym.Name = "col_pym";
            this.col_pym.ReadOnly = true;
            this.col_pym.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.col_pym.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // col_wbm
            // 
            this.col_wbm.DataPropertyName = "WB";
            this.col_wbm.HeaderText = "五笔码";
            this.col_wbm.Name = "col_wbm";
            this.col_wbm.ReadOnly = true;
            this.col_wbm.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.col_wbm.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // col_bz
            // 
            this.col_bz.DataPropertyName = "BZ";
            this.col_bz.HeaderText = "备注信息";
            this.col_bz.Name = "col_bz";
            this.col_bz.ReadOnly = true;
            // 
            // frmXMLBZD
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1119, 506);
            this.Controls.Add(this.dgvXMLBZD);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Name = "frmXMLBZD";
            this.Text = "医保项目类别字典";
            this.Load += new System.EventHandler(this.frmXMLBZD_Load);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvXMLBZD)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button btnDEL;
        private System.Windows.Forms.Button btnADD;
        private System.Windows.Forms.TextBox txtBZ;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RadioButton rabtnQY;
        private System.Windows.Forms.RadioButton rabtnBQY;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtXMDM;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtXMLB;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.LinkLabel linkLabel1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ComboBox cmbXMLB;
        private System.Windows.Forms.Button btnSelect;
        private System.Windows.Forms.TextBox txtXMJS;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox txtXMMC;
        private System.Windows.Forms.ComboBox cmbYBCS;
        private System.Windows.Forms.ComboBox cmbDQ;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.DataGridView dgvXMLBZD;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Button btnUPDATE;
        private System.Windows.Forms.DataGridViewTextBoxColumn col_sequ;
        private System.Windows.Forms.DataGridViewTextBoxColumn col_ID;
        private System.Windows.Forms.DataGridViewTextBoxColumn col_lbmc;
        private System.Windows.Forms.DataGridViewTextBoxColumn col_code;
        private System.Windows.Forms.DataGridViewTextBoxColumn col_name;
        private System.Windows.Forms.DataGridViewTextBoxColumn col_cs1;
        private System.Windows.Forms.DataGridViewTextBoxColumn col_cs;
        private System.Windows.Forms.DataGridViewTextBoxColumn col_dq1;
        private System.Windows.Forms.DataGridViewTextBoxColumn col_dq;
        private System.Windows.Forms.DataGridViewCheckBoxColumn col_qybz;
        private System.Windows.Forms.DataGridViewTextBoxColumn col_pym;
        private System.Windows.Forms.DataGridViewTextBoxColumn col_wbm;
        private System.Windows.Forms.DataGridViewTextBoxColumn col_bz;
    }
}