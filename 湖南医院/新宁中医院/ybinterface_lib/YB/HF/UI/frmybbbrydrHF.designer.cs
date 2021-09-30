namespace ybinterface_lib
{
    partial class frmybbbrydrHF
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmybbbrydrHF));
            this.idsybbz = new Srvtools.InfoDataSet(this.components);
            this.dtp_ryrq = new System.Windows.Forms.DateTimePicker();
            this.label4 = new System.Windows.Forms.Label();
            this.txt_yllb = new Srvtools.UserTextBox();
            this.idsbztbd_YL = new Srvtools.InfoDataSet(this.components);
            this.txt_lyjg = new Srvtools.UserTextBox();
            this.idsbz06h = new Srvtools.InfoDataSet(this.components);
            this.txt_bz = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.btn_ReadCard = new System.Windows.Forms.Button();
            this.btn_Close = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.txt_hzxm = new System.Windows.Forms.TextBox();
            this.label25 = new System.Windows.Forms.Label();
            this.txt_zyno = new System.Windows.Forms.TextBox();
            this.idszy01h = new Srvtools.InfoDataSet(this.components);
            this.label7 = new System.Windows.Forms.Label();
            this.txt_szdw = new System.Windows.Forms.TextBox();
            this.txt_grbh = new System.Windows.Forms.TextBox();
            this.txt_csrq = new System.Windows.Forms.TextBox();
            this.txt_zhye = new System.Windows.Forms.TextBox();
            this.txt_xm = new System.Windows.Forms.TextBox();
            this.txt_xb = new System.Windows.Forms.TextBox();
            this.txt_sfzh1 = new System.Windows.Forms.TextBox();
            this.txt_rylb = new System.Windows.Forms.TextBox();
            this.lab6 = new System.Windows.Forms.Label();
            this.lab4 = new System.Windows.Forms.Label();
            this.lab21 = new System.Windows.Forms.Label();
            this.lab3 = new System.Windows.Forms.Label();
            this.lab2 = new System.Windows.Forms.Label();
            this.lab9 = new System.Windows.Forms.Label();
            this.lab8 = new System.Windows.Forms.Label();
            this.lab0 = new System.Windows.Forms.Label();
            this.gybControl = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.txt_bz2 = new System.Windows.Forms.TextBox();
            this.txt_bz1 = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.txtDGYS = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.txt_sfzh = new System.Windows.Forms.TextBox();
            this.btn_RegisterYB = new System.Windows.Forms.Button();
            this.btn_CancelYB = new System.Windows.Forms.Button();
            this.alvDGYS = new AutoListView.AutoListView();
            this.alvBZ = new AutoListView.AutoListView();
            this.alvBZ1 = new AutoListView.AutoListView();
            this.alvBZ2 = new AutoListView.AutoListView();
            ((System.ComponentModel.ISupportInitialize)(this.idsybbz)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.idsbztbd_YL)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.idsbz06h)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.idszy01h)).BeginInit();
            this.gybControl.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // idsybbz
            // 
            this.idsybbz.Active = false;
            this.idsybbz.AlwaysClose = false;
            this.idsybbz.DataCompressed = false;
            this.idsybbz.DeleteIncomplete = true;
            this.idsybbz.LastKeyValues = null;
            this.idsybbz.Locale = new System.Globalization.CultureInfo("zh-CN");
            this.idsybbz.PacketRecords = 50;
            this.idsybbz.Position = -1;
            this.idsybbz.RemoteName = "sybdj.yb_ybbz";
            this.idsybbz.ServerModify = false;
            // 
            // dtp_ryrq
            // 
            this.dtp_ryrq.CalendarFont = new System.Drawing.Font("宋体", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.dtp_ryrq.CustomFormat = "yyyy-MM-dd HH:mm:ss";
            this.dtp_ryrq.Font = new System.Drawing.Font("宋体", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.dtp_ryrq.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtp_ryrq.Location = new System.Drawing.Point(314, 108);
            this.dtp_ryrq.Name = "dtp_ryrq";
            this.dtp_ryrq.Size = new System.Drawing.Size(174, 22);
            this.dtp_ryrq.TabIndex = 189;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("宋体", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label4.ForeColor = System.Drawing.Color.Black;
            this.label4.Location = new System.Drawing.Point(19, 111);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(46, 13);
            this.label4.TabIndex = 194;
            this.label4.Text = "主诊断";
            // 
            // txt_yllb
            // 
            this.txt_yllb.ctrPanel = this;
            this.txt_yllb.EnterEnable = true;
            this.txt_yllb.Font = new System.Drawing.Font("宋体", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txt_yllb.LeaveText = null;
            this.txt_yllb.Location = new System.Drawing.Point(69, 67);
            this.txt_yllb.MyDataSource = this.idsbztbd_YL;
            this.txt_yllb.Name = "txt_yllb";
            this.txt_yllb.nextFocus = this.txt_lyjg;
            this.txt_yllb.RefVal = null;
            this.txt_yllb.setWhere = "";
            this.txt_yllb.ShowItem = null;
            this.txt_yllb.showTextItem = "";
            this.txt_yllb.Size = new System.Drawing.Size(174, 22);
            this.txt_yllb.TabIndex = 1;
            // 
            // idsbztbd_YL
            // 
            this.idsbztbd_YL.Active = false;
            this.idsbztbd_YL.AlwaysClose = false;
            this.idsbztbd_YL.DataCompressed = false;
            this.idsbztbd_YL.DeleteIncomplete = true;
            this.idsbztbd_YL.LastKeyValues = null;
            this.idsbztbd_YL.Locale = new System.Globalization.CultureInfo("zh-CN");
            this.idsbztbd_YL.PacketRecords = 20;
            this.idsbztbd_YL.Position = -1;
            this.idsbztbd_YL.RemoteName = "sybdj.yb_bztbd";
            this.idsbztbd_YL.ServerModify = false;
            // 
            // txt_lyjg
            // 
            this.txt_lyjg.ctrPanel = this;
            this.txt_lyjg.EnterEnable = true;
            this.txt_lyjg.Font = new System.Drawing.Font("宋体", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txt_lyjg.LeaveText = null;
            this.txt_lyjg.Location = new System.Drawing.Point(314, 67);
            this.txt_lyjg.MyDataSource = this.idsbz06h;
            this.txt_lyjg.Name = "txt_lyjg";
            this.txt_lyjg.nextFocus = this.txt_bz;
            this.txt_lyjg.RefVal = null;
            this.txt_lyjg.setWhere = "";
            this.txt_lyjg.ShowItem = null;
            this.txt_lyjg.showTextItem = "";
            this.txt_lyjg.Size = new System.Drawing.Size(174, 22);
            this.txt_lyjg.TabIndex = 2;
            // 
            // idsbz06h
            // 
            this.idsbz06h.Active = false;
            this.idsbz06h.AlwaysClose = false;
            this.idsbz06h.DataCompressed = false;
            this.idsbz06h.DeleteIncomplete = true;
            this.idsbz06h.LastKeyValues = null;
            this.idsbz06h.Locale = new System.Globalization.CultureInfo("zh-CN");
            this.idsbz06h.PacketRecords = 100;
            this.idsbz06h.Position = -1;
            this.idsbz06h.RemoteName = "sybdj.yb_bz06h";
            this.idsbz06h.ServerModify = false;
            // 
            // txt_bz
            // 
            this.txt_bz.Location = new System.Drawing.Point(69, 108);
            this.txt_bz.Name = "txt_bz";
            this.txt_bz.Size = new System.Drawing.Size(174, 22);
            this.txt_bz.TabIndex = 221;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("宋体", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label3.ForeColor = System.Drawing.Color.Black;
            this.label3.Location = new System.Drawing.Point(6, 72);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(59, 13);
            this.label3.TabIndex = 192;
            this.label3.Text = "入院类别";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("宋体", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label2.Location = new System.Drawing.Point(249, 113);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(59, 13);
            this.label2.TabIndex = 190;
            this.label2.Text = "入院日期";
            // 
            // btn_ReadCard
            // 
            this.btn_ReadCard.Location = new System.Drawing.Point(9, 447);
            this.btn_ReadCard.Name = "btn_ReadCard";
            this.btn_ReadCard.Size = new System.Drawing.Size(134, 30);
            this.btn_ReadCard.TabIndex = 4;
            this.btn_ReadCard.Text = "读医保卡(2100)";
            this.btn_ReadCard.UseVisualStyleBackColor = true;
            this.btn_ReadCard.Click += new System.EventHandler(this.btn_ReadCard_Click);
            // 
            // btn_Close
            // 
            this.btn_Close.Font = new System.Drawing.Font("宋体", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btn_Close.Location = new System.Drawing.Point(423, 447);
            this.btn_Close.Name = "btn_Close";
            this.btn_Close.Size = new System.Drawing.Size(84, 30);
            this.btn_Close.TabIndex = 5;
            this.btn_Close.Text = "关闭";
            this.btn_Close.UseVisualStyleBackColor = true;
            this.btn_Close.Click += new System.EventHandler(this.btn_Close_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("宋体", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(146, 33);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(59, 13);
            this.label1.TabIndex = 186;
            this.label1.Text = "患者姓名";
            // 
            // txt_hzxm
            // 
            this.txt_hzxm.Enabled = false;
            this.txt_hzxm.Font = new System.Drawing.Font("宋体", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txt_hzxm.Location = new System.Drawing.Point(211, 28);
            this.txt_hzxm.Name = "txt_hzxm";
            this.txt_hzxm.ReadOnly = true;
            this.txt_hzxm.Size = new System.Drawing.Size(80, 22);
            this.txt_hzxm.TabIndex = 185;
            // 
            // label25
            // 
            this.label25.AutoSize = true;
            this.label25.Font = new System.Drawing.Font("宋体", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label25.ForeColor = System.Drawing.Color.Red;
            this.label25.Location = new System.Drawing.Point(6, 33);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(46, 13);
            this.label25.TabIndex = 184;
            this.label25.Text = "住院号";
            // 
            // txt_zyno
            // 
            this.txt_zyno.Font = new System.Drawing.Font("宋体", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txt_zyno.Location = new System.Drawing.Point(58, 30);
            this.txt_zyno.Name = "txt_zyno";
            this.txt_zyno.Size = new System.Drawing.Size(74, 22);
            this.txt_zyno.TabIndex = 0;
            this.txt_zyno.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txt_zyno_KeyDown);
            // 
            // idszy01h
            // 
            this.idszy01h.Active = false;
            this.idszy01h.AlwaysClose = false;
            this.idszy01h.DataCompressed = false;
            this.idszy01h.DeleteIncomplete = true;
            this.idszy01h.LastKeyValues = null;
            this.idszy01h.Locale = new System.Globalization.CultureInfo("zh-CN");
            this.idszy01h.PacketRecords = 15;
            this.idszy01h.Position = -1;
            this.idszy01h.RemoteName = "sybdj.yb_zy01h";
            this.idszy01h.ServerModify = false;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("宋体", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label7.ForeColor = System.Drawing.Color.Black;
            this.label7.Location = new System.Drawing.Point(249, 72);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(59, 13);
            this.label7.TabIndex = 196;
            this.label7.Text = "缴费类型";
            // 
            // txt_szdw
            // 
            this.txt_szdw.Font = new System.Drawing.Font("宋体", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txt_szdw.Location = new System.Drawing.Point(68, 138);
            this.txt_szdw.Name = "txt_szdw";
            this.txt_szdw.ReadOnly = true;
            this.txt_szdw.Size = new System.Drawing.Size(173, 22);
            this.txt_szdw.TabIndex = 214;
            // 
            // txt_grbh
            // 
            this.txt_grbh.Font = new System.Drawing.Font("宋体", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txt_grbh.Location = new System.Drawing.Point(68, 27);
            this.txt_grbh.Name = "txt_grbh";
            this.txt_grbh.ReadOnly = true;
            this.txt_grbh.Size = new System.Drawing.Size(173, 22);
            this.txt_grbh.TabIndex = 212;
            // 
            // txt_csrq
            // 
            this.txt_csrq.Font = new System.Drawing.Font("宋体", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txt_csrq.Location = new System.Drawing.Point(314, 28);
            this.txt_csrq.Name = "txt_csrq";
            this.txt_csrq.ReadOnly = true;
            this.txt_csrq.Size = new System.Drawing.Size(173, 22);
            this.txt_csrq.TabIndex = 210;
            // 
            // txt_zhye
            // 
            this.txt_zhye.BackColor = System.Drawing.SystemColors.Window;
            this.txt_zhye.Font = new System.Drawing.Font("宋体", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txt_zhye.ForeColor = System.Drawing.Color.Red;
            this.txt_zhye.Location = new System.Drawing.Point(312, 141);
            this.txt_zhye.Name = "txt_zhye";
            this.txt_zhye.ReadOnly = true;
            this.txt_zhye.Size = new System.Drawing.Size(173, 22);
            this.txt_zhye.TabIndex = 209;
            // 
            // txt_xm
            // 
            this.txt_xm.Font = new System.Drawing.Font("宋体", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txt_xm.Location = new System.Drawing.Point(68, 64);
            this.txt_xm.Name = "txt_xm";
            this.txt_xm.ReadOnly = true;
            this.txt_xm.Size = new System.Drawing.Size(173, 22);
            this.txt_xm.TabIndex = 208;
            // 
            // txt_xb
            // 
            this.txt_xb.Font = new System.Drawing.Font("宋体", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txt_xb.Location = new System.Drawing.Point(68, 101);
            this.txt_xb.Name = "txt_xb";
            this.txt_xb.ReadOnly = true;
            this.txt_xb.Size = new System.Drawing.Size(173, 22);
            this.txt_xb.TabIndex = 207;
            // 
            // txt_sfzh1
            // 
            this.txt_sfzh1.Font = new System.Drawing.Font("宋体", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txt_sfzh1.Location = new System.Drawing.Point(314, 64);
            this.txt_sfzh1.Name = "txt_sfzh1";
            this.txt_sfzh1.ReadOnly = true;
            this.txt_sfzh1.Size = new System.Drawing.Size(173, 22);
            this.txt_sfzh1.TabIndex = 213;
            // 
            // txt_rylb
            // 
            this.txt_rylb.Font = new System.Drawing.Font("宋体", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txt_rylb.Location = new System.Drawing.Point(314, 101);
            this.txt_rylb.Name = "txt_rylb";
            this.txt_rylb.ReadOnly = true;
            this.txt_rylb.Size = new System.Drawing.Size(173, 22);
            this.txt_rylb.TabIndex = 206;
            // 
            // lab6
            // 
            this.lab6.AutoSize = true;
            this.lab6.Font = new System.Drawing.Font("宋体", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lab6.Location = new System.Drawing.Point(249, 31);
            this.lab6.Name = "lab6";
            this.lab6.Size = new System.Drawing.Size(59, 13);
            this.lab6.TabIndex = 205;
            this.lab6.Text = "出生日期";
            // 
            // lab4
            // 
            this.lab4.AutoSize = true;
            this.lab4.Font = new System.Drawing.Font("宋体", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lab4.Location = new System.Drawing.Point(6, 104);
            this.lab4.Name = "lab4";
            this.lab4.Size = new System.Drawing.Size(33, 13);
            this.lab4.TabIndex = 204;
            this.lab4.Text = "性别";
            // 
            // lab21
            // 
            this.lab21.AutoSize = true;
            this.lab21.Font = new System.Drawing.Font("宋体", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lab21.Location = new System.Drawing.Point(247, 147);
            this.lab21.Name = "lab21";
            this.lab21.Size = new System.Drawing.Size(59, 13);
            this.lab21.TabIndex = 203;
            this.lab21.Text = "帐户余额";
            // 
            // lab3
            // 
            this.lab3.AutoSize = true;
            this.lab3.Font = new System.Drawing.Font("宋体", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lab3.Location = new System.Drawing.Point(6, 67);
            this.lab3.Name = "lab3";
            this.lab3.Size = new System.Drawing.Size(33, 13);
            this.lab3.TabIndex = 202;
            this.lab3.Text = "姓名";
            // 
            // lab2
            // 
            this.lab2.AutoSize = true;
            this.lab2.Font = new System.Drawing.Font("宋体", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lab2.Location = new System.Drawing.Point(249, 67);
            this.lab2.Name = "lab2";
            this.lab2.Size = new System.Drawing.Size(59, 13);
            this.lab2.TabIndex = 200;
            this.lab2.Text = "身份证号";
            // 
            // lab9
            // 
            this.lab9.AutoSize = true;
            this.lab9.Font = new System.Drawing.Font("宋体", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lab9.Location = new System.Drawing.Point(6, 141);
            this.lab9.Name = "lab9";
            this.lab9.Size = new System.Drawing.Size(59, 13);
            this.lab9.TabIndex = 199;
            this.lab9.Text = "所在单位";
            // 
            // lab8
            // 
            this.lab8.AutoSize = true;
            this.lab8.Font = new System.Drawing.Font("宋体", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lab8.Location = new System.Drawing.Point(249, 104);
            this.lab8.Name = "lab8";
            this.lab8.Size = new System.Drawing.Size(59, 13);
            this.lab8.TabIndex = 198;
            this.lab8.Text = "人员类别";
            // 
            // lab0
            // 
            this.lab0.AutoSize = true;
            this.lab0.Font = new System.Drawing.Font("宋体", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lab0.Location = new System.Drawing.Point(6, 31);
            this.lab0.Name = "lab0";
            this.lab0.Size = new System.Drawing.Size(59, 13);
            this.lab0.TabIndex = 197;
            this.lab0.Text = "个人编号";
            // 
            // gybControl
            // 
            this.gybControl.Controls.Add(this.lab3);
            this.gybControl.Controls.Add(this.txt_szdw);
            this.gybControl.Controls.Add(this.lab0);
            this.gybControl.Controls.Add(this.txt_grbh);
            this.gybControl.Controls.Add(this.lab8);
            this.gybControl.Controls.Add(this.lab9);
            this.gybControl.Controls.Add(this.txt_csrq);
            this.gybControl.Controls.Add(this.lab2);
            this.gybControl.Controls.Add(this.txt_zhye);
            this.gybControl.Controls.Add(this.txt_xm);
            this.gybControl.Controls.Add(this.lab21);
            this.gybControl.Controls.Add(this.txt_xb);
            this.gybControl.Controls.Add(this.lab4);
            this.gybControl.Controls.Add(this.txt_sfzh1);
            this.gybControl.Controls.Add(this.lab6);
            this.gybControl.Controls.Add(this.txt_rylb);
            this.gybControl.Font = new System.Drawing.Font("宋体", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.gybControl.Location = new System.Drawing.Point(9, 242);
            this.gybControl.Name = "gybControl";
            this.gybControl.Size = new System.Drawing.Size(498, 187);
            this.gybControl.TabIndex = 215;
            this.gybControl.TabStop = false;
            this.gybControl.Text = "医保卡信息";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.txt_bz2);
            this.groupBox2.Controls.Add(this.txt_bz1);
            this.groupBox2.Controls.Add(this.txt_bz);
            this.groupBox2.Controls.Add(this.label9);
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Controls.Add(this.txtDGYS);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.txt_zyno);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.label25);
            this.groupBox2.Controls.Add(this.txt_lyjg);
            this.groupBox2.Controls.Add(this.txt_hzxm);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.dtp_ryrq);
            this.groupBox2.Controls.Add(this.txt_yllb);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.txt_sfzh);
            this.groupBox2.Font = new System.Drawing.Font("宋体", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.groupBox2.Location = new System.Drawing.Point(9, 5);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(498, 220);
            this.groupBox2.TabIndex = 0;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "患者信息";
            // 
            // txt_bz2
            // 
            this.txt_bz2.Location = new System.Drawing.Point(69, 188);
            this.txt_bz2.Name = "txt_bz2";
            this.txt_bz2.Size = new System.Drawing.Size(174, 22);
            this.txt_bz2.TabIndex = 223;
            // 
            // txt_bz1
            // 
            this.txt_bz1.Location = new System.Drawing.Point(69, 147);
            this.txt_bz1.Name = "txt_bz1";
            this.txt_bz1.Size = new System.Drawing.Size(174, 22);
            this.txt_bz1.TabIndex = 222;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("宋体", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label9.ForeColor = System.Drawing.Color.Black;
            this.label9.Location = new System.Drawing.Point(12, 191);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(53, 13);
            this.label9.TabIndex = 220;
            this.label9.Text = "次诊断2";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("宋体", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label8.ForeColor = System.Drawing.Color.Black;
            this.label8.Location = new System.Drawing.Point(12, 150);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(53, 13);
            this.label8.TabIndex = 218;
            this.label8.Text = "次诊断1";
            // 
            // txtDGYS
            // 
            this.txtDGYS.Location = new System.Drawing.Point(314, 188);
            this.txtDGYS.Name = "txtDGYS";
            this.txtDGYS.Size = new System.Drawing.Size(174, 22);
            this.txtDGYS.TabIndex = 216;
            this.txtDGYS.Visible = false;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("宋体", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label6.ForeColor = System.Drawing.Color.Black;
            this.label6.Location = new System.Drawing.Point(249, 191);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(59, 13);
            this.label6.TabIndex = 215;
            this.label6.Text = "定岗医生";
            this.label6.Visible = false;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("宋体", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label5.ForeColor = System.Drawing.Color.Red;
            this.label5.Location = new System.Drawing.Point(290, 33);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(59, 13);
            this.label5.TabIndex = 200;
            this.label5.Text = "身份证号";
            // 
            // txt_sfzh
            // 
            this.txt_sfzh.Font = new System.Drawing.Font("宋体", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txt_sfzh.Location = new System.Drawing.Point(350, 28);
            this.txt_sfzh.Name = "txt_sfzh";
            this.txt_sfzh.ReadOnly = true;
            this.txt_sfzh.Size = new System.Drawing.Size(136, 22);
            this.txt_sfzh.TabIndex = 213;
            // 
            // btn_RegisterYB
            // 
            this.btn_RegisterYB.Font = new System.Drawing.Font("宋体", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btn_RegisterYB.Location = new System.Drawing.Point(158, 447);
            this.btn_RegisterYB.Name = "btn_RegisterYB";
            this.btn_RegisterYB.Size = new System.Drawing.Size(112, 30);
            this.btn_RegisterYB.TabIndex = 5;
            this.btn_RegisterYB.Text = "医保登记(2210)";
            this.btn_RegisterYB.UseVisualStyleBackColor = true;
            this.btn_RegisterYB.Click += new System.EventHandler(this.btn_RegisterYB_Click);
            // 
            // btn_CancelYB
            // 
            this.btn_CancelYB.Font = new System.Drawing.Font("宋体", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btn_CancelYB.Location = new System.Drawing.Point(286, 447);
            this.btn_CancelYB.Name = "btn_CancelYB";
            this.btn_CancelYB.Size = new System.Drawing.Size(115, 30);
            this.btn_CancelYB.TabIndex = 5;
            this.btn_CancelYB.Text = "医保撤销(2240)";
            this.btn_CancelYB.UseVisualStyleBackColor = true;
            this.btn_CancelYB.Click += new System.EventHandler(this.btn_CancelYB_Click);
            // 
            // alvDGYS
            // 
            this.alvDGYS.AttachTextBox = this.txtDGYS;
            this.alvDGYS.ChooseInfoDataSet = null;
            this.alvDGYS.DataViewFont = null;
            this.alvDGYS.DataViewFontColor = System.Drawing.Color.Navy;
            this.alvDGYS.DataViewMaxSize = new System.Drawing.Size(300, 150);
            this.alvDGYS.DeleteTextWhenNull = true;
            this.alvDGYS.Enable = true;
            this.alvDGYS.FilterString = "";
            this.alvDGYS.IsGetAllRecord = true;
            this.alvDGYS.NextFocusControl = this.btn_ReadCard;
            this.alvDGYS.ParentControl = this;
            this.alvDGYS.RelationEnable = true;
            this.alvDGYS.Relations = null;
            this.alvDGYS.Remark = null;
            this.alvDGYS.SetColumnParam.QueryColumn = "";
            this.alvDGYS.SetColumnParam.TextColumn = "";
            this.alvDGYS.SetColumnParam.ValueColumn = "";
            this.alvDGYS.SetColumnParam.ViewColumn = "";
            this.alvDGYS.TargetColumn = 0;
            this.alvDGYS.UpdateSourceWhenEnter = false;
            this.alvDGYS.UseMulThread = false;
            this.alvDGYS.WhereStr = null;
            // 
            // alvBZ
            // 
            this.alvBZ.AttachTextBox = this.txt_bz;
            this.alvBZ.ChooseInfoDataSet = null;
            this.alvBZ.DataViewFont = null;
            this.alvBZ.DataViewFontColor = System.Drawing.Color.Navy;
            this.alvBZ.DataViewMaxSize = new System.Drawing.Size(300, 150);
            this.alvBZ.DeleteTextWhenNull = true;
            this.alvBZ.Enable = true;
            this.alvBZ.FilterString = "";
            this.alvBZ.IsGetAllRecord = true;
            this.alvBZ.NextFocusControl = this.txt_bz1;
            this.alvBZ.ParentControl = this;
            this.alvBZ.RelationEnable = true;
            this.alvBZ.Relations = null;
            this.alvBZ.Remark = null;
            this.alvBZ.SetColumnParam.QueryColumn = "";
            this.alvBZ.SetColumnParam.TextColumn = "";
            this.alvBZ.SetColumnParam.ValueColumn = "";
            this.alvBZ.SetColumnParam.ViewColumn = "";
            this.alvBZ.TargetColumn = 0;
            this.alvBZ.UpdateSourceWhenEnter = false;
            this.alvBZ.UseMulThread = false;
            this.alvBZ.WhereStr = null;
            // 
            // alvBZ1
            // 
            this.alvBZ1.AttachTextBox = this.txt_bz1;
            this.alvBZ1.ChooseInfoDataSet = null;
            this.alvBZ1.DataViewFont = null;
            this.alvBZ1.DataViewFontColor = System.Drawing.Color.Navy;
            this.alvBZ1.DataViewMaxSize = new System.Drawing.Size(300, 150);
            this.alvBZ1.DeleteTextWhenNull = true;
            this.alvBZ1.Enable = true;
            this.alvBZ1.FilterString = "";
            this.alvBZ1.IsGetAllRecord = true;
            this.alvBZ1.NextFocusControl = this.txt_bz2;
            this.alvBZ1.ParentControl = this;
            this.alvBZ1.RelationEnable = true;
            this.alvBZ1.Relations = null;
            this.alvBZ1.Remark = null;
            this.alvBZ1.SetColumnParam.QueryColumn = "";
            this.alvBZ1.SetColumnParam.TextColumn = "";
            this.alvBZ1.SetColumnParam.ValueColumn = "";
            this.alvBZ1.SetColumnParam.ViewColumn = "";
            this.alvBZ1.TargetColumn = 0;
            this.alvBZ1.UpdateSourceWhenEnter = false;
            this.alvBZ1.UseMulThread = false;
            this.alvBZ1.WhereStr = null;
            // 
            // alvBZ2
            // 
            this.alvBZ2.AttachTextBox = this.txt_bz2;
            this.alvBZ2.ChooseInfoDataSet = null;
            this.alvBZ2.DataViewFont = null;
            this.alvBZ2.DataViewFontColor = System.Drawing.Color.Navy;
            this.alvBZ2.DataViewMaxSize = new System.Drawing.Size(300, 150);
            this.alvBZ2.DeleteTextWhenNull = true;
            this.alvBZ2.Enable = true;
            this.alvBZ2.FilterString = "";
            this.alvBZ2.IsGetAllRecord = true;
            this.alvBZ2.NextFocusControl = this.txtDGYS;
            this.alvBZ2.ParentControl = this;
            this.alvBZ2.RelationEnable = true;
            this.alvBZ2.Relations = null;
            this.alvBZ2.Remark = null;
            this.alvBZ2.SetColumnParam.QueryColumn = "";
            this.alvBZ2.SetColumnParam.TextColumn = "";
            this.alvBZ2.SetColumnParam.ValueColumn = "";
            this.alvBZ2.SetColumnParam.ViewColumn = "";
            this.alvBZ2.TargetColumn = 0;
            this.alvBZ2.UpdateSourceWhenEnter = false;
            this.alvBZ2.UseMulThread = false;
            this.alvBZ2.WhereStr = null;
            // 
            // frmybbbrydrSR
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(520, 487);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.gybControl);
            this.Controls.Add(this.btn_CancelYB);
            this.Controls.Add(this.btn_RegisterYB);
            this.Controls.Add(this.btn_Close);
            this.Controls.Add(this.btn_ReadCard);
            this.Font = new System.Drawing.Font("宋体", 11F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmybbbrydrSR";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "医保补办入院";
            this.Load += new System.EventHandler(this.Fybbbry_Load);
            ((System.ComponentModel.ISupportInitialize)(this.idsybbz)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.idsbztbd_YL)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.idsbz06h)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.idszy01h)).EndInit();
            this.gybControl.ResumeLayout(false);
            this.gybControl.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label4;
        private Srvtools.UserTextBox txt_yllb;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.DateTimePicker dtp_ryrq;
        private System.Windows.Forms.Button btn_ReadCard;
        private System.Windows.Forms.Button btn_Close;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txt_hzxm;
        private System.Windows.Forms.Label label25;
        private System.Windows.Forms.TextBox txt_zyno;
        private Srvtools.InfoDataSet idsbztbd_YL;
        private Srvtools.InfoDataSet idsybbz;
        private Srvtools.InfoDataSet idszy01h;
        private System.Windows.Forms.Label label7;
        private Srvtools.UserTextBox txt_lyjg;
        private Srvtools.InfoDataSet idsbz06h;
        private System.Windows.Forms.TextBox txt_szdw;
        private System.Windows.Forms.TextBox txt_grbh;
        private System.Windows.Forms.TextBox txt_csrq;
        private System.Windows.Forms.TextBox txt_zhye;
        private System.Windows.Forms.TextBox txt_xm;
        private System.Windows.Forms.TextBox txt_xb;
        private System.Windows.Forms.TextBox txt_sfzh1;
        private System.Windows.Forms.TextBox txt_rylb;
        private System.Windows.Forms.Label lab6;
        private System.Windows.Forms.Label lab4;
        private System.Windows.Forms.Label lab21;
        private System.Windows.Forms.Label lab3;
        private System.Windows.Forms.Label lab2;
        private System.Windows.Forms.Label lab9;
        private System.Windows.Forms.Label lab8;
        private System.Windows.Forms.Label lab0;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox gybControl;
        private System.Windows.Forms.Button btn_CancelYB;
        private System.Windows.Forms.Button btn_RegisterYB;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txt_sfzh;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtDGYS;
        private AutoListView.AutoListView alvDGYS;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox txt_bz2;
        private System.Windows.Forms.TextBox txt_bz1;
        private System.Windows.Forms.TextBox txt_bz;
        private AutoListView.AutoListView alvBZ;
        private AutoListView.AutoListView alvBZ1;
        private AutoListView.AutoListView alvBZ2;
    }
}