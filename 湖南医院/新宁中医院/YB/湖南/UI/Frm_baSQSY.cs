using Newtonsoft.Json.Linq;
using Srvtools;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace yb_interfaces
{
    public partial class Frm_baSQSY : InfoForm
    {
        private string hzxm = string.Empty;
        public Frm_baSQSY()
        {
            InitializeComponent();

            #region 外伤取信息
            string strSql1 = string.Format(@"select bzkeyx,bzname from bztbd where bzcodn='adm_way'");
            DataSet ds1 = CliUtils.ExecuteSql("sybdj", "cmd", strSql1, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            cmbwsryfs.ValueMember = "bzkeyx";
            cmbwsryfs.DisplayMember = "bzname";
            cmbwsryfs.DataSource = ds1.Tables[0].DefaultView;
            cmbwsywsybz.SelectedIndex = 1;
            #endregion

            #region 生育取信息 
            strSql1 = string.Format(@"select bzkeyx,bzname from bztbd where bzcodn='matn_trt_dclaer_type'");
            ds1 = CliUtils.ExecuteSql("sybdj", "cmd", strSql1, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            cmbsysydysbrlb.ValueMember = "bzkeyx";
            cmbsysydysbrlb.DisplayMember = "bzname";
            cmbsysydysbrlb.DataSource = ds1.Tables[0].DefaultView;
            cmbsysyzgdjzt.SelectedIndex = 1;

            strSql1 = string.Format(@"select bzkeyx,bzname from bztbd where bzcodn='agnter_rlts'");
            ds1 = CliUtils.ExecuteSql("sybdj", "cmd", strSql1, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            cmbsydbrgx.ValueMember = "bzkeyx";
            cmbsydbrgx.DisplayMember = "bzname";
            cmbsydbrgx.DataSource = ds1.Tables[0].DefaultView;

            strSql1 = string.Format(@"select bzkeyx,bzname from bztbd where bzcodn='matn_type'");
            ds1 = CliUtils.ExecuteSql("sybdj", "cmd", strSql1, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            cmbsysylb.ValueMember = "bzkeyx";
            cmbsysylb.DisplayMember = "bzname";
            cmbsysylb.DataSource = ds1.Tables[0].DefaultView;
            #endregion


            #region 初始化datagridview
            this.dataGridView4.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            this.dataGridView4.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
            this.dataGridView4.AllowUserToAddRows = false;
            this.dataGridView4.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            #endregion
        }

        private void btnDk_Click(object sender, EventArgs e)
        {
            object[] back_mess = yb_interface_hn_nkNew.YBZYDK(null);
            if (back_mess[1].ToString() == "1")
            {
                string[] strP = back_mess[2].ToString().Split('|');
                string grbh = strP[0].ToString(); //个人编号
                string xzlx = "";//险种类型
                string tcqh = "";//联系电话
                string strSql = string.Format(@"select * from ybickxx where grbh='{0}'", grbh);
                DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    xzlx = ds.Tables[0].Rows[0]["xzlx"].ToString();
                    tcqh = ds.Tables[0].Rows[0]["tcqh"].ToString();
                    txt_xzlx.Text = xzlx;
                    txt_rybh.Text = grbh;
                    txt_dhbh.Text = tcqh;
                }


            }
            else
            {
                MessageBox.Show("读卡失败|" + back_mess[2].ToString());
            }
        }

        private void btnZy_Click(object sender, EventArgs e)
        {
            if ((bool)isData()[1] == false)
            {
                MessageBox.Show(isData()[2].ToString());
                return;
            }
            object[] param = new object[] { txt_rybh.Text,txt_xzlx.Text,txt_lxdh.Text,txt_lxdz.Text,txt_dhbh.Text,txt_zddm.Text,txt_zdmc.Text,txt_bqms.Text,txt_zwjgbh.Text,txt_zwyymc.Text, txt_jydqbh.Text, txt_zybz.Text,txt_zylx.Text.ToString(),dtpZyrq.Value.ToString("yyyy-MM-dd"),txt_zyyy.Text,txt_zyyj.Text,dtpZykssj.Value.ToString("yyyy-MM-dd"),dtpZyjssj.Value.ToString("yyyy-MM-dd"),"1"};
            object[] back_mess = yb_interface_hn_nkNew.YBZYBA(param);
            if (back_mess[1].ToString() == "1")
            {
                MessageBox.Show("转院备案成功！");
            }
            else
            {
                MessageBox.Show(back_mess[2].ToString());
            }
        }

        private object[] isData()
        {
            bool bl = true;
            string errMessage = "";
            if (string.IsNullOrEmpty(txt_rybh.Text))
            {
                bl = false;
                errMessage += "人员编号不能为空，请先读卡！" + "\r\n";
            }
            if (string.IsNullOrEmpty(txt_jydqbh.Text))
            {
                bl = false;
                // errMessage += "就医地行政区划（转往医院）不能为空！" + "\r\n";
                txt_jydqbh.Text = "431103";
            }
            if (string.IsNullOrEmpty(txt_zddm.Text))
            {
                bl = false;
                errMessage += "诊断代码不能为空！" + "\r\n";
            }
            if (string.IsNullOrEmpty(txt_zdmc.Text))
            {
                bl = false;
                errMessage += "诊断名称不能为空！" + "\r\n";
            }
            if (string.IsNullOrEmpty(txt_zwjgbh.Text))
            {
                bl = false;
                //errMessage += "转往机构编号不能为空！" + "\r\n";
                txt_zwjgbh.Text = "H43110300029";
            }
            if (string.IsNullOrEmpty(txt_zwyymc.Text))
            {
                bl = false;
                //errMessage += "转往医院名称不能为空！" + "\r\n";
                txt_zwyymc.Text = "永州市第三人民医院";
            }
            if (string.IsNullOrEmpty(txt_zylx.Text))
            {
                bl = false;
                errMessage += "转院类型不能为空！" + "\r\n";
            }
            if (string.IsNullOrEmpty(dtpZyrq.Value.ToString()))
            {
                bl = false;
                errMessage += "转院日期不能为空！" + "\r\n";
            }
            if (string.IsNullOrEmpty(txt_zyyy.Text))
            {
                bl = false;
                errMessage += "转院原因不能为空！" + "\r\n";
            }
            if (string.IsNullOrEmpty(txt_zyyj.Text))
            {
                bl = false;
                errMessage += "转院意见不能为空！" + "\r\n";
            }
            return new object[] { 0, true, errMessage };
        }

        private object[] isMbData()
        {
            bool bl = true;
            string errMessage = "";
            if (string.IsNullOrEmpty(txtmx_rybh.Text))
            {
                bl = false;
                errMessage += "人员编号不能为空，请先读卡！" + "\r\n";
            }
            if (string.IsNullOrEmpty(txtmx_zddm.Text))
            {
                bl = false;
                //errMessage += "门慢门特病种目录代码不能为空！" + "\r\n";
            }
            if (string.IsNullOrEmpty(txtmx_zdmc.Text))
            {
                bl = false;
                errMessage += "门慢门特病种名称不能为空！" + "\r\n";
            }
            if (string.IsNullOrEmpty(txtmx_tcqh.Text))
            {
                bl = false;
                errMessage += "参保机构医保区划不能为空！" + "\r\n";
            }
            if (string.IsNullOrEmpty(txtmx_ddyljgbm.Text))
            {
                bl = false;
                errMessage += "鉴定定点医药机构编号不能为空！" + "\r\n";
            }
            if (string.IsNullOrEmpty(txtmx_ddyljgmc.Text))
            {
                bl = false;
                errMessage += "鉴定定点医药机构名称不能为空！" + "\r\n";
            }
            if (string.IsNullOrEmpty(txtmx_zdysbm.Text))
            {
                bl = false;
                errMessage += "诊断医师编码不能为空！" + "\r\n";
            }
            if (string.IsNullOrEmpty(txtmx_zdysxm.Text))
            {
                bl = false;
                errMessage += "诊断医师姓名不能为空！" + "\r\n";
            }
            if (string.IsNullOrEmpty(dtmbjdsj.Value.ToString()))
            {
                bl = false;
                errMessage += "医院鉴定日期不能为空！" + "\r\n";
            }
            if (string.IsNullOrEmpty(dtmbbegin.Value.ToString()))
            {
                bl = false;
                errMessage += "开始日期不能为空！" + "\r\n";
            }

            return new object[] { 0, bl, errMessage };

        }

        private object[] isDdData()
        {
            bool bl = true;
            string errMessage = "";
            if (string.IsNullOrEmpty(txtdd_rybh.Text))
            {
                bl = false;
                errMessage += "人员编号不能为空，请先读卡！" + "\r\n";
            }
            if (string.IsNullOrEmpty(txtdd_ddyljgmc.Text))
            {
                bl = false;
                errMessage += "定点医药机构名称不能为空！" + "\r\n";
            }
            if (string.IsNullOrEmpty(txtdd_ywsqlx.Text))
            {
                bl = false;
                errMessage += "业务申请类型不能为空！" + "\r\n";
            }
            if (string.IsNullOrEmpty(txtdd_ddpxh.Text))
            {
                bl = false;
                errMessage += "定点排序号不能为空！" + "\r\n";
            }
            if (string.IsNullOrEmpty(txtdd_ddyljgbm.Text))
            {
                bl = false;
                errMessage += "定点医药机构编号不能为空！" + "\r\n";
            }
            if (string.IsNullOrEmpty(dtLDStart.Text))
            {
                bl = false;
                errMessage += "开始日期不能为空！" + "\r\n";
            }
            return new object[] { 0, bl, errMessage };

        }

        public void GetDate()
        {

            string tb = txt_zyrybh.Text.Trim();
            StringBuilder Strsql = new StringBuilder();
            Strsql.Append("SELECT top 1000 dysblsh,jbr,grbh,isnull(xm,'') as xm,isnull(sfzh,'') as sfzh,xzlx,lxdh,lxdz,tcqh,zddm,zdmc,jbbqms,zwddyljgbh,zwddyljgmc,");
            Strsql.Append("jytcqh,yytyzybz,zylx,zyrq,zyyy,zyyj,ksrq,jsrq FROM ybzyba where");
            Strsql.Append(" balx='zy' and cxbz=1");
            if (!string.IsNullOrEmpty(tb))
            {
                Strsql.Append(" and grbh like  '%" + tb + "%'");
            }
            DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", Strsql.ToString(), CliUtils.fLoginDB.ToString(), true, CliUtils.fCurrentProject.ToString());
            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.DataSource = ds.Tables[0];

        }

        private void btn_Query_Click(object sender, EventArgs e)
        {
            GetDate();
        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            txt_dysblsh.Text = this.dataGridView1.CurrentRow.Cells["dysblsh"].Value.ToString();
            txt_rybh.Text = this.dataGridView1.CurrentRow.Cells["grbh"].Value.ToString();
            txt_xzlx.Text = this.dataGridView1.CurrentRow.Cells["xzlx"].Value.ToString();
            txt_lxdh.Text = this.dataGridView1.CurrentRow.Cells["lxdh"].Value.ToString();
            txt_lxdz.Text = this.dataGridView1.CurrentRow.Cells["lxdz"].Value.ToString();
            txt_dhbh.Text = this.dataGridView1.CurrentRow.Cells["tcqh"].Value.ToString();
            txt_zddm.Text = this.dataGridView1.CurrentRow.Cells["zddm"].Value.ToString();
            txt_zdmc.Text = this.dataGridView1.CurrentRow.Cells["zdmc"].Value.ToString();
            txt_bqms.Text = this.dataGridView1.CurrentRow.Cells["jbbqms"].Value.ToString();
            txt_zwjgbh.Text = this.dataGridView1.CurrentRow.Cells["zwddyljgbh"].Value.ToString();
            txt_zwyymc.Text = this.dataGridView1.CurrentRow.Cells["zwddyljgmc"].Value.ToString();
            txt_jydqbh.Text = this.dataGridView1.CurrentRow.Cells["jytcqh"].Value.ToString();
            txt_zybz.Text = this.dataGridView1.CurrentRow.Cells["yytyzybz"].Value.ToString();
            txt_zylx.Text = this.dataGridView1.CurrentRow.Cells["zylx"].Value.ToString();
            dtpZyrq.Value  =Convert.ToDateTime(this.dataGridView1.CurrentRow.Cells["zyrq"].Value.ToString());
            txt_zyyy.Text = this.dataGridView1.CurrentRow.Cells["zyyy"].Value.ToString();
            txt_zyyj.Text = this.dataGridView1.CurrentRow.Cells["zyyj"].Value.ToString();
            dtpZykssj.Value = DateTime.Parse(this.dataGridView1.CurrentRow.Cells["ksrq"].Value.ToString());
            dtpZyjssj.Value = DateTime.Parse(this.dataGridView1.CurrentRow.Cells["jsrq"].Value.ToString());
        }

        private void Frm_baSQSY_Load(object sender, EventArgs e)
        {
            GetDate();

        }

        private void btn_qk_Click(object sender, EventArgs e)
        {
            txt_rybh.Text = "";
            txt_xzlx.Text = "";
            txt_lxdh.Text = "";
            txt_lxdz.Text = "";
            txt_dhbh.Text = "";
            txt_zddm.Text = "";
            txt_zdmc.Text = "";
            txt_bqms.Text = "";
            txt_zwjgbh.Text = "";
            txt_zwyymc.Text = "";
            txt_jydqbh.Text = "";
            txt_zybz.Text = "";
            txt_zylx.Text = ""; 
            txt_zyyy.Text = "";
            txt_zyyj.Text = "";
            txt_dysblsh.Text = "";

        }

        private void btn_Dk_Click(object sender, EventArgs e)
        {
            string iniPath = AppDomain.CurrentDomain.BaseDirectory + "GocentYB.ini";
            //读取配置
            string YLGHBH = yb_interface_hn_nkNew.YBJGBH;//医疗机构编号
            string DDYLJGMC = yb_interface_hn_nkNew.YBJGMC;//医院名称
            object[] back_mess = yb_interface_hn_nkNew.YBZYDK(null);
            if (back_mess[1].ToString() == "1")
            {
                string[] strP = back_mess[2].ToString().Split('|');
                string grbh = strP[0].ToString(); //个人编号
                string xzlx = "";//险种类型
                string tcqh = "";//联系电话
                string strSql = string.Format(@"select * from ybickxx where grbh='{0}'", grbh);
                DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    xzlx = ds.Tables[0].Rows[0]["xzlx"].ToString();
                    tcqh = ds.Tables[0].Rows[0]["tcqh"].ToString();
                    txtmx_xzlx.Text = xzlx;
                    txtmx_tcqh.Text = tcqh;
                    txtmx_rybh.Text = grbh;
                    txtmx_ddyljgbm.Text = YLGHBH;
                    txtmx_ddyljgmc.Text = DDYLJGMC;
                }


            }
            else
            {
                MessageBox.Show("读卡失败|" + back_mess[2].ToString());
            }
        }

        private void btnMx_Qk_Click(object sender, EventArgs e)
        {
            txtmx_rybh.Text = "";
            txtmx_xzlx.Text = "";
            txtmx_zddm.Text = "";
            txtmx_zdmc.Text = "";
            txtmx_lxdh.Text = "";
            txtmx_lxdz.Text = "";
            txtmx_tcqh.Text = "";
            txtmx_ddyljgbm.Text = "";
            txtmx_ddyljgmc.Text = ""; 
            txtmx_zdysbm.Text = "";
            txtmx_zdysxm.Text = ""; 
            txt_mbdysblsh.Text = "";

        }

        public void Getmxdate()
        {
            string tb1 = txb_bh.Text.Trim();
            StringBuilder Strsql = new StringBuilder();
            Strsql.Append("SELECT TOP 1000 dysblsh,jbr,grbh,isnull(xm,'') as xm,isnull(sfzh,'') as sfzh,xzlx,zddm,zdmc,lxdh,lxdz,tcqh,ddyljgbm,ddyljgmc,");
            Strsql.Append("yyjdrq,zdysbm,zdysxm,ksrq,jsrq FROM ybzyba where");
            Strsql.Append(" balx='mb' and cxbz=1");
            if (!string.IsNullOrEmpty(tb1))
            {
                Strsql.Append(" and grbh like  '%" + tb1 + "%'");
            }
            DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", Strsql.ToString(), CliUtils.fLoginDB.ToString(), true, CliUtils.fCurrentProject.ToString());
            dataGridView2.AutoGenerateColumns = false;
            dataGridView2.DataSource = ds.Tables[0];

        }


        private void btnmx_Query_Click(object sender, EventArgs e)
        {
            Getmxdate();
        }

        private void btnZybacx_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txt_rybh.Text))
            {
                MessageBox.Show("人员编号不能为空！请先选择左侧备案数据双击！");
                return;
            }
            if (string.IsNullOrEmpty(txt_dysblsh.Text))
            {
                MessageBox.Show("待遇申报流水号不能为空！请先选择左侧备案数据双击！");
                return;
            }
            object[] param = new object[] { txt_dysblsh.Text, txt_rybh.Text };
            object[] back_mess = yb_interface_hn_nkNew.YBZYBACX(param);
            if (back_mess[1].ToString() == "1")
            {
                MessageBox.Show("转院备案撤销成功！");
            }
            else
            {
                MessageBox.Show(back_mess[2].ToString());
            }
        }

        public void Getdddate()
        {
            string tb2 = txtdd_bh.Text.Trim();
            StringBuilder Strsql = new StringBuilder();
            Strsql.Append("SELECT TOP 1000 dysblsh,jbr,grbh,lxdh,lxdz,ywsqlx,ksrq,jsrq,dbrxm,dbrzjlx,dbrzjhm,");
            Strsql.Append("dbrlxdh,dbrlxdz,dbrgx,ddpxh,ddyljgbm,ddyljgmc,bz FROM ybzyba where");
            Strsql.Append(" balx='dd' and cxbz=1");
            if (!string.IsNullOrEmpty(tb2))
            {
                Strsql.Append(" and grbh like  '%" + tb2 + "%'");
            }
            DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", Strsql.ToString(), CliUtils.fLoginDB.ToString(), true, CliUtils.fCurrentProject.ToString());
            dataGridView3.AutoGenerateColumns = false;
            dataGridView3.DataSource = ds.Tables[0];

        }
        private void btndd_Query_Click(object sender, EventArgs e)
        {
            Getdddate();
        }

        private void btndd_qk_Click(object sender, EventArgs e)
        {
            txtdd_dysblsh.Text = "";
            txtdd_rybh.Text = "";
            txtdd_lxdh.Text = "";
            txtdd_ddyljgmc.Text = "";
            txtdd_lxdz.Text = "";
            txtdd_ywsqlx.Text = ""; 
            txtdd_dbrxm.Text = "";
            txtdd_dbrzjlx.Text = "";
            txtdd_dbrzjhm.Text = "";
            txtdd_dbrlxdh.Text = "";
            txtdd_dbrlxdz.Text = "";
            txtdd_dbrgx.Text = "";
            txtdd_ddpxh.Text = "";
            txtdd_ddyljgbm.Text = "";
            txtdd_bz.Text = "";
        }

        private void dataGridView2_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            txt_mbdysblsh.Text = this.dataGridView2.CurrentRow.Cells["mbdysblsh"].Value.ToString();
            txtmx_rybh.Text = this.dataGridView2.CurrentRow.Cells["mxgrbh"].Value.ToString();
            txtmx_xzlx.Text = this.dataGridView2.CurrentRow.Cells["mxxzlx"].Value.ToString();
            txtmx_zddm.Text = this.dataGridView2.CurrentRow.Cells["mxzddm"].Value.ToString();
            txtmx_zdmc.Text = this.dataGridView2.CurrentRow.Cells["mxzdmc"].Value.ToString();
            txtmx_lxdh.Text = this.dataGridView2.CurrentRow.Cells["mxlxdh"].Value.ToString();
            txtmx_lxdz.Text = this.dataGridView2.CurrentRow.Cells["mxlxdz"].Value.ToString();
            txtmx_tcqh.Text = this.dataGridView2.CurrentRow.Cells["mxtcqh"].Value.ToString();
            txtmx_ddyljgbm.Text = this.dataGridView2.CurrentRow.Cells["mxddyljgbm"].Value.ToString();
            txtmx_ddyljgmc.Text = this.dataGridView2.CurrentRow.Cells["mxddyljgmc"].Value.ToString();
            dtmbjdsj.Value= DateTime.Parse(this.dataGridView2.CurrentRow.Cells["mxyyjdrq"].Value.ToString());
            txtmx_zdysbm.Text = this.dataGridView2.CurrentRow.Cells["mxzdysbm"].Value.ToString();
            txtmx_zdysxm.Text = this.dataGridView2.CurrentRow.Cells["mxzdysxm"].Value.ToString();
            dtmbbegin.Value = DateTime.Parse(this.dataGridView2.CurrentRow.Cells["mxksrq"].Value.ToString());
            dtmbend.Value = DateTime.Parse(this.dataGridView2.CurrentRow.Cells["mxjsrq"].Value.ToString());

        }

        private void dataGridView3_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            txtdd_dysblsh.Text = this.dataGridView3.CurrentRow.Cells["dddysblsh"].Value.ToString();
            txtdd_rybh.Text = this.dataGridView3.CurrentRow.Cells["ddgrbh"].Value.ToString();
            txtdd_lxdh.Text = this.dataGridView3.CurrentRow.Cells["ddlxdh"].Value.ToString();
            txtdd_ddyljgmc.Text = this.dataGridView3.CurrentRow.Cells["ddddyljgmc"].Value.ToString();
            txtdd_lxdz.Text = this.dataGridView3.CurrentRow.Cells["ddlxdz"].Value.ToString();
            txtdd_ywsqlx.Text = this.dataGridView3.CurrentRow.Cells["ddywsqlx"].Value.ToString();
            dtLDStart.Value = DateTime.Parse(this.dataGridView3.CurrentRow.Cells["ddksrq"].Value.ToString());
            dtLDStart.Value = DateTime.Parse(this.dataGridView3.CurrentRow.Cells["ddjsrq"].Value.ToString());
            txtdd_dbrxm.Text = this.dataGridView3.CurrentRow.Cells["dddbrxm"].Value.ToString();
            txtdd_dbrzjlx.Text = this.dataGridView3.CurrentRow.Cells["dddbrzjlx"].Value.ToString();
            txtdd_dbrzjhm.Text = this.dataGridView3.CurrentRow.Cells["dddbrzjhm"].Value.ToString();
            txtdd_dbrlxdh.Text = this.dataGridView3.CurrentRow.Cells["dddbrlxdh"].Value.ToString();
            txtdd_dbrlxdz.Text = this.dataGridView3.CurrentRow.Cells["dddbrlxdz"].Value.ToString();
            txtdd_dbrgx.Text = this.dataGridView3.CurrentRow.Cells["dddbrgx"].Value.ToString();
            txtdd_ddpxh.Text = this.dataGridView3.CurrentRow.Cells["ddddpxh"].Value.ToString();
            txtdd_ddyljgbm.Text = this.dataGridView3.CurrentRow.Cells["ddddyljgbm"].Value.ToString();
            txtdd_bz.Text = this.dataGridView3.CurrentRow.Cells["ddbz"].Value.ToString();
        }

        private void btnmx_mxba_Click(object sender, EventArgs e)
        {
            if ((bool)isMbData()[1] == false)
            {
                MessageBox.Show(isMbData()[2].ToString());
                return;
            }
            object[] param = new object[] { txtmx_rybh.Text,txtmx_xzlx.Text,txtmx_zddm.Text,txtmx_zdmc.Text,txtmx_lxdh.Text,txtmx_lxdz.Text,
                                           txtmx_tcqh.Text,txtmx_ddyljgbm.Text,txtmx_ddyljgmc.Text,dtmbjdsj.Value.ToString("yyyy-MM-dd"),txtmx_zdysbm.Text,txtmx_zdysxm.Text,dtmbbegin.Value.ToString("yyyy-MM-dd"),dtmbend.Value.ToString("yyyy-MM-dd")};
            object[] back_mess = yb_interface_hn_nkNew.YBRYMBBA(param);
            if (back_mess[1].ToString() == "1")
            {
                MessageBox.Show("人员慢病备案成功！");
            }
            else
            {
                MessageBox.Show(back_mess[2].ToString());
            }
        }

        private void btn_mbcx_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtmx_rybh.Text))
            {
                MessageBox.Show("人员编号不能为空！请先选择左侧备案数据双击！");
                return;
            }
            if (string.IsNullOrEmpty(txt_mbdysblsh.Text))
            {
                MessageBox.Show("待遇申报流水号不能为空！请先选择左侧备案数据双击！");
                return;
            }
            object[] param = new object[] { txt_mbdysblsh.Text, txtmx_rybh.Text };
            object[] back_mess = yb_interface_hn_nkNew.YBRYMBBACX(param);
            if (back_mess[1].ToString() == "1")
            {
                MessageBox.Show("人员慢病备案撤销成功！");
            }
            else
            {
                MessageBox.Show(back_mess[2].ToString());
            }
        }

        private void btn_ddba_Click(object sender, EventArgs e)
        {
            if ((bool)isDdData()[1] == false)
            {
                MessageBox.Show(isDdData()[2].ToString());
                return;
            }
            object[] param = new object[] { txtdd_rybh.Text,txtdd_lxdh.Text,txtdd_lxdz.Text,txtdd_ywsqlx.Tag.ToString(),dtLDStart.Value.ToString("yyyy-MM-dd 00:00:00"),dtLDEnd.Value.ToString("yyyy-MM-dd 23:59:59"),txtdd_dbrxm.Text,
                                           string.IsNullOrEmpty(txtdd_dbrzjlx.Text)?txtdd_dbrzjlx.Text:txtdd_dbrzjlx.Tag.ToString(),txtdd_dbrzjhm.Text,txtdd_dbrlxdh.Text,txtdd_dbrlxdz.Text,string.IsNullOrEmpty(txtdd_dbrgx.Text)?txtdd_dbrgx.Text:txtdd_dbrgx.Tag.ToString(),txtdd_ddpxh.Text,txtdd_ddyljgbm.Text,txtdd_ddyljgmc.Text,txtdd_bz.Text};
            object[] back_mess = yb_interface_hn_nkNew.YBRYDDBA(param);
            if (back_mess[1].ToString() == "1")
            {
                MessageBox.Show("人员定点备案成功！");
            }
            else
            {
                MessageBox.Show(back_mess[2].ToString());
            }
        }

        private void btn_ddbacx_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtdd_rybh.Text))
            {
                MessageBox.Show("人员编号不能为空！请先选择左侧备案数据双击！");
                return;
            }
            if (string.IsNullOrEmpty(txtdd_dysblsh.Text))
            {
                MessageBox.Show("待遇申报流水号不能为空！请先选择左侧备案数据双击！");
                return;
            }
            object[] param = new object[] { txtdd_dysblsh.Text, txtdd_rybh.Text };
            object[] back_mess = yb_interface_hn_nkNew.YBRYDDBACX(param);
            if (back_mess[1].ToString() == "1")
            {
                MessageBox.Show("人员定点备案撤销成功！");
            }
            else
            {
                MessageBox.Show(back_mess[2].ToString());
            }
        }

        private void btndd_DK_Click(object sender, EventArgs e)
        {
            string iniPath = AppDomain.CurrentDomain.BaseDirectory + "GocentYB.ini";
            //读取配置
            string YLGHBH = yb_interface_hn_nkNew.YBJGBH;//医疗机构编号
            string DDYLJGMC = yb_interface_hn_nkNew.YBJGMC;//医院名称
            object[] back_mess = yb_interface_hn_nkNew.YBZYDK(null);
            if (back_mess[1].ToString() == "1")
            {
                string[] strP = back_mess[2].ToString().Split('|');
                string grbh = strP[0].ToString(); //个人编号
                string xzlx = "";//险种类型
                string tcqh = "";//联系电话
                string strSql = string.Format(@"select * from ybickxx where grbh='{0}'", grbh);
                DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    xzlx = ds.Tables[0].Rows[0]["xzlx"].ToString();
                    tcqh = ds.Tables[0].Rows[0]["tcqh"].ToString();
                    txtdd_ddpxh.Text = "1";
                    txtdd_rybh.Text = grbh;
                    txtdd_ddyljgbm.Text = YLGHBH;
                    txtdd_ddyljgmc.Text = DDYLJGMC;
                }


            }
            else
            {
                MessageBox.Show("读卡失败|" + back_mess[2].ToString());
            }
        }

        private void splitContainer1_Panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            object[] back_mess = yb_interface_hn_nkNew.YBZYDK(null);
            if (back_mess[1].ToString() == "1")
            {
                string[] strP = back_mess[2].ToString().Split('|');
                string grbh = strP[0].ToString(); //个人编号
                string strSql = string.Format(@"select xzlx,tcqh,zjlx,gmsfhm,dwmc,xm from ybickxx where grbh='{0}'", grbh);
                DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    txtwsrybh.Text = grbh;
                    txtwsxzlx.Text = ds.Tables[0].Rows[0]["xzlx"].ToString();
                    txtwsybqh.Text = ds.Tables[0].Rows[0]["tcqh"].ToString();
                    txtwsryzjlx.Text = ds.Tables[0].Rows[0]["zjlx"].ToString();
                    txtwszjhm.Text = ds.Tables[0].Rows[0]["gmsfhm"].ToString();
                    txtwsdwmc.Text = ds.Tables[0].Rows[0]["dwmc"].ToString();
                    txtwscxtj.Tag = ds.Tables[0].Rows[0]["xm"].ToString();
                }
            }
            else
            {
                MessageBox.Show("读卡失败|" + back_mess[2].ToString());
            }
        }

        private void btnwsba_Click(object sender, EventArgs e)
        {
            string wssssj = "";
            string wsrysj = "";
            if (string.IsNullOrEmpty(txtwsrybh.Text.ToString()))
            {
                MessageBox.Show("请先读卡再进行外伤备案！");
                return;
            }
            if (!string.IsNullOrEmpty(txtwssssj.Text.ToString()))
            {
                try
                {
                    wssssj = DateTime.Parse(txtwssssj.Text.ToString()).ToString("yyyy-MM-dd HH:mm:ss");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                    return;
                }
            }
            if (!string.IsNullOrEmpty(txtwsrysj.Text.ToString()))
            {
                try
                {
                    wsrysj = DateTime.Parse(txtwsrysj.Text.ToString()).ToString("yyyy-MM-dd HH:mm:ss");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                    return;
                }
            }
            object[] param = new object[] { txtwsrybh.Text.ToString(), txtwsxzlx.Text.ToString(), dtpwsksrq.Value.ToString("yyyy-MM-dd HH:mm:ss"), dtpwsbajsrq.Value.ToString("yyyy-MM-dd HH:mm:ss"), txtwsryzjlx.Text.ToString(), txtwszjhm.Text.ToString(),
                txtwslxdh.Text.ToString(), txtwslxdz.Text.ToString(), txtwsybqh.Text.ToString(), txtwsdwbh.Text.ToString(), txtwsdwmc.Text.ToString(), txtwsjzid.Text.ToString(), txtwsjsid.Text.ToString(),
                txtwsshbw.Text.ToString(), wssssj, txtwsssdd.Text.ToString(), txtwszsyy.Text.ToString(), cmbwsryfs.SelectedValue==null?"":cmbwsryfs.SelectedValue.ToString(), wsrysj, txtwsryzdms.Text.ToString(), txtwsdbrxm.Text.ToString(),
                txtwsdbrzjlx.Text.ToString(), txtwsdbrzjhm.Text.ToString(), txtwsdbrlxfs.Text.ToString(), txtwsdbrlxdz.Text.ToString(), txtwsdbrgx.Text.ToString(), cmbwsywsybz.Text.ToString().Substring(0,1),
                txtwsbz.Text.ToString(), txtwsdsfpfbl.Text.ToString(), txtwscxtj.Tag.ToString()};
            object[] back_mess = yb_interface_hn_nkNew.YBWSDJBA(param);
            if (back_mess[1].ToString() == "1")
            {
                MessageBox.Show("人员外伤登记备案成功！");
                btnwsqk_Click(sender, e);
            }
            else
            {
                MessageBox.Show(back_mess[2].ToString());
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (dgvwsba.Rows.Count == 0)
            {
                MessageBox.Show("请先查询要撤销的备案信息，点击某行进行撤销！");
                return;
            }
            int aRow = dgvwsba.CurrentRow.Index;
            string mxlsh = dgvwsba.Rows[aRow].Cells["wsmxlsh"].Value.ToString();
            string rybh = dgvwsba.Rows[aRow].Cells["wsrybh"].Value.ToString();
            object[] param = new object[] { mxlsh, rybh };
            object[] back_mess = yb_interface_hn_nkNew.YBWSDJBACX(param);
            if (back_mess[1].ToString() == "1")
            {
                MessageBox.Show("人员外伤登记备案撤销成功！");
                btnwsqk_Click(sender, e);
            }
            else
            {
                MessageBox.Show(back_mess[2].ToString());
            }
        }

        private void btnwsqk_Click(object sender, EventArgs e)
        {
            foreach (Control ctl in this.plws.Controls)
            {
                if (ctl is TextBox)
                    ctl.Text = "";
            }
            txtwscxtj.Tag = "";
        }

        private void btnsydk_Click(object sender, EventArgs e)
        {
            object[] back_mess = yb_interface_hn_nkNew.YBZYDK(null);
            if (back_mess[1].ToString() == "1")
            {
                string[] strP = back_mess[2].ToString().Split('|');
                string grbh = strP[0].ToString(); //个人编号
                string strSql = string.Format(@"select xzlx,tcqh,zjlx,gmsfhm,dwmc,xm from ybickxx where grbh='{0}'", grbh);
                DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    txtsyrybh.Text = grbh;
                    txtsyxzlx.Text = ds.Tables[0].Rows[0]["xzlx"].ToString();
                    txtsyybqh.Text = ds.Tables[0].Rows[0]["tcqh"].ToString();
                    txtsyryzjlx.Text = ds.Tables[0].Rows[0]["zjlx"].ToString();
                    txtsyzjhm.Text = ds.Tables[0].Rows[0]["gmsfhm"].ToString();
                    txtsydwmc.Text = ds.Tables[0].Rows[0]["dwmc"].ToString();
                    txtsyxm.Text = ds.Tables[0].Rows[0]["xm"].ToString();
                }
            }
            else
            {
                MessageBox.Show("读卡失败|" + back_mess[2].ToString());
            }
        }

        private void btnsyqk_Click(object sender, EventArgs e)
        {
            foreach (Control ctl in this.plsy.Controls)
            {
                if (ctl is TextBox)
                    ctl.Text = "";
            }
        }

        private void btnsyba_Click(object sender, EventArgs e)
        {
            string symcyjrq = "";
            string syyjsyrq = "";
            string sysbrq = "";
            string syjsrq = "";
            if (string.IsNullOrEmpty(txtsyrybh.Text.ToString()))
            {
                MessageBox.Show("请先读卡再进行生育登记备案！");
                return;
            }
            if (!string.IsNullOrEmpty(txtsymcyjrq.Text.ToString()))
            {
                try
                {
                    symcyjrq = DateTime.Parse(txtsymcyjrq.Text.ToString()).ToString("yyyy-MM-dd HH:mm:ss");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                    return;
                }
            }
            if (!string.IsNullOrEmpty(txtsyyjsyrq.Text.ToString()))
            {
                try
                {
                    syyjsyrq = DateTime.Parse(txtsyyjsyrq.Text.ToString()).ToString("yyyy-MM-dd HH:mm:ss");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                    return;
                }
            }
            if (!string.IsNullOrEmpty(txtsysbrq.Text.ToString()))
            {
                try
                {
                    sysbrq = DateTime.Parse(txtsysbrq.Text.ToString()).ToString("yyyy-MM-dd HH:mm:ss");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                    return;
                }
            }
            if (!string.IsNullOrEmpty(txtsyjsrq.Text.ToString()))
            {
                try
                {
                    syjsrq = DateTime.Parse(txtsyjsrq.Text.ToString()).ToString("yyyy-MM-dd HH:mm:ss");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                    return;
                }
            }
            object[] param = new object[] { txtsyrybh.Text.ToString(), txtsyxzlx.Text.ToString(), txtsyryzjlx.Text.ToString(), txtsyzjhm.Text.ToString(), txtsyxm.Text.ToString(), txtsycsrq.Text.ToString(),
                txtsylxdh.Text.ToString(), txtsylxdz.Text.ToString(), txtsyybqh.Text.ToString(), txtsydwbh.Text.ToString(), txtsydwmc.Text.ToString(), txtsyyzs.Text.ToString(), cmbsysydysbrlb.SelectedValue.ToString(),
                txtsyjhsyfwzh.Text.ToString(), symcyjrq, syyjsyrq, sysbrq, txtsypoxm.Text.ToString(), txtsypozjlx.Text.ToString(), txtsypozjhm.Text.ToString(), cmbsysyzgdjzt.Text.ToString().Substring(0,1),
                dtpsyksrq.Value.ToString("yyyy-MM-dd HH:mm:ss"), syjsrq, txtsydbrxm.Text.ToString(),txtsydbrzjlx.Text.ToString(), txtsydbrzjhm.Text.ToString(), txtsydbrlxfs.Text.ToString(), txtsydbrlxdz.Text.ToString(),
                cmbsydbrgx.SelectedValue.ToString(), txtsybz.Text.ToString(), txtsytc.Text.ToString(), cmbsysylb.SelectedValue.ToString(), txtsymzjcbxbz.Text.ToString() };
            object[] back_mess = yb_interface_hn_nkNew.YBSYDYBA(param);
            if (back_mess[1].ToString() == "1")
            {
                MessageBox.Show("生育登记备案成功！");
                btnsyqk_Click(sender, e);
            }
            else
            {
                MessageBox.Show(back_mess[2].ToString());
            }
        }

        private void btncxsyba_Click(object sender, EventArgs e)
        {
            if (dgvsy.Rows.Count == 0)
            {
                MessageBox.Show("请先查询要撤销的备案信息，点击某行进行撤销！");
                return;
            }
            int aRow = dgvsy.CurrentRow.Index;
            string mxlsh = dgvsy.Rows[aRow].Cells["symxlsh"].Value.ToString();
            string rybh = dgvsy.Rows[aRow].Cells["syrybh"].Value.ToString();
            object[] param = new object[] { mxlsh, rybh };
            object[] back_mess = yb_interface_hn_nkNew.YBSYDYBACX(param);
            if (back_mess[1].ToString() == "1")
            {
                MessageBox.Show("生育登记备案撤销成功！");
                btnwsqk_Click(sender, e);
            }
            else
            {
                MessageBox.Show(back_mess[2].ToString());
            }
        }

        private void btnwscx_Click(object sender, EventArgs e)
        {
            string cxtj = txtwscxtj.Text.Trim();
            StringBuilder Strsql = new StringBuilder();
            Strsql.Append("SELECT dysblsh,jbr,grbh,lxdh,lxdz,ywsqlx,ksrq,jsrq,dbrxm,dbrzjlx,dbrzjhm,");
            Strsql.Append("dbrlxdh,dbrlxdz,dbrgx,ddpxh,ddyljgbm,ddyljgmc,bz,xm FROM ybzyba where");
            Strsql.Append(" balx='WS' and cxbz=1");
            if (!string.IsNullOrEmpty(cxtj))
            {
                Strsql.Append(" and (grbh like  '%" + cxtj + "%' or xm like '%" + cxtj + "%') ");
            }
            DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", Strsql.ToString(), CliUtils.fLoginDB.ToString(), true, CliUtils.fCurrentProject.ToString());
            dgvwsba.AutoGenerateColumns = false;
            dgvwsba.DataSource = ds.Tables[0];
        }

        private void btnsycx_Click(object sender, EventArgs e)
        {
            string cxtj = txtsycxtj.Text.Trim();
            StringBuilder Strsql = new StringBuilder();
            Strsql.Append("SELECT dysblsh,jbr,grbh,lxdh,lxdz,ywsqlx,ksrq,jsrq,dbrxm,dbrzjlx,dbrzjhm,");
            Strsql.Append("dbrlxdh,dbrlxdz,dbrgx,ddpxh,ddyljgbm,ddyljgmc,bz,xm FROM ybzyba where");
            Strsql.Append(" balx='SY' and cxbz=1");
            if (!string.IsNullOrEmpty(cxtj))
            {
                Strsql.Append(" and (grbh like  '%" + cxtj + "%' or xm like '%" + cxtj + "%') ");
            }
            DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", Strsql.ToString(), CliUtils.fLoginDB.ToString(), true, CliUtils.fCurrentProject.ToString());
            dgvsy.AutoGenerateColumns = false;
            dgvsy.DataSource = ds.Tables[0];
        }
        internal DataSet ds = new DataSet();
        private void btnjzxxcx_Click(object sender, EventArgs e)
        {
            object[] param = new object[] { txtjsxxcxrybh.Text.ToString(), txtjsxxcxjsid.Text.ToString(), txtjsxxcxjzid.Text.ToString() };
            object[] back_mess = yb_interface_hn_nkNew.YBJSXXCX(param);
            if (back_mess[1].ToString() == "1")
            {
                List<JObject> jlsit = back_mess[2] as List<JObject>;
                if (jlsit.Count > 0)
                {
                    this.dataGridView4.DataSource = jlsit;
                }
                else
                {
                    dataGridView4.DataSource = null;
                }
                MessageBox.Show("结算信息查询成功！");
            }
            else
            {
                MessageBox.Show(back_mess[2].ToString());
            }
        }

        private void btnzyxxcx_Click(object sender, EventArgs e)
        {
            object[] param = new object[] { txtzyxxcxrybh.Text.ToString(), dtpzyxxcxkssj.Value.ToString("yyyy-MM-dd 00:00:00"), dtpzyxxcxjssj.Value.ToString("yyyy-MM-dd 23:59:59") };
            object[] back_mess = yb_interface_hn_nkNew.YBZYXXCX(param);
            if (back_mess[1].ToString() == "1")
            {
                List<JObject> jlist = back_mess[2] as List<JObject>;
                if (jlist.Count > 0)
                {
                    this.dataGridView4.DataSource = jlist;
                }
                else
                {
                    dataGridView4.DataSource = null;
                }
                MessageBox.Show("在院信息查询成功！");
            }
            else
            {
                MessageBox.Show(back_mess[2].ToString());
            }
        }

        private void btnjzxxcx_Click_1(object sender, EventArgs e)
        {
            object[] param = new object[] { txtjzxxcxrybh.Text.ToString(), dtpjzxxcxkssj.Value.ToString("yyyy-MM-dd 00:00:00"), dtpjzxxcxjssj.Value.ToString("yyyy-MM-dd 23:59:59"),
                txtjzxxcxyllb.Text.ToString(), txtjzxxcxjzid.Text.ToString()};
            object[] back_mess = yb_interface_hn_nkNew.YBJZXXCX(param);
            if (back_mess[1].ToString() == "1")
            {
                List < JObject > jlist = back_mess[2] as List<JObject>;
                if (jlist.Count>0)
                {
                    this.dataGridView4.DataSource = jlist;
                }
                else
                {
                    dataGridView4.DataSource = null;
                }
                MessageBox.Show("就诊信息查询成功！");
            }
            else
            {
                MessageBox.Show(back_mess[2].ToString());
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            object[] param = new object[] { txtfymxpsnNO.Text.ToString(), txtfymxJSid.Text.ToString(), txtfymxJZid.Text.ToString() };
            object[] back_mess = yb_interface_hn_nkNew.YBFYMXCX(param);
            if (back_mess[1].ToString() == "1")
            {
                List < JObject > jlist = back_mess[2] as List<JObject>;
                if (jlist.Count>0)
                {
                    this.dataGridView4.DataSource = jlist;
                }
                else
                {
                    dataGridView4.DataSource = null;
                }
                MessageBox.Show("费用明细查询成功！");
            }
            else
            {
                MessageBox.Show(back_mess[2].ToString());
            }
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            object[] param = new object[] { txtmtyypsnNO.Text.ToString(), dtmtyybegin.Value.ToString("yyyy-MM-dd 00:00:00"), dtmtyyend.Value.ToString("yyyy-MM-dd 23:59:59") };
            object[] back_mess = yb_interface_hn_nkNew.YBRYMTBYYJLCX(param);
            if (back_mess[1].ToString() == "1")
            {
                List<JObject> jlist = back_mess[2] as List<JObject>;
                if (jlist.Count > 0)
                {
                    this.dataGridView4.DataSource = jlist;
                }
                else
                {
                    dataGridView4.DataSource = null;
                }
                MessageBox.Show("人员慢特用药记录查询成功！");
            }
            else
            {
                MessageBox.Show(back_mess[2].ToString());
            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            object[] param = new object[] { txtzyxxpsnNO.Text.ToString(), dtzyxxbegin.Value.ToString("yyyy-MM-dd 00:00:00"), dtzyxxend.Value.ToString("yyyy-MM-dd 23:59:59") };
            object[] back_mess = yb_interface_hn_nkNew.YBZHYXXCX(param);
            if (back_mess[1].ToString() == "1")
            {
                List<JObject> jlist = back_mess[2] as List<JObject>;
                if (jlist.Count > 0)
                {
                    this.dataGridView4.DataSource = jlist;
                }
                else
                {
                    dataGridView4.DataSource = null;
                }
                MessageBox.Show("转院信息查询成功！");
            }
            else
            {
                MessageBox.Show(back_mess[2].ToString());
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            object[] param = new object[] { txtzdxxjzid.Text.ToString(), txtzdxxpsnNo.Text.ToString() };
            object[] back_mess = yb_interface_hn_nkNew.YBZDXXCX(param);
            if (back_mess[1].ToString() == "1")
            {
                List<JObject> jlist = back_mess[2] as List<JObject>;
                if (jlist.Count > 0)
                {
                    this.dataGridView4.DataSource = jlist;
                }
                else
                {
                    dataGridView4.DataSource = null;
                }
                MessageBox.Show("诊断信息查询成功！");
            }
            else
            {
                MessageBox.Show(back_mess[2].ToString());
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            object[] param = new object[] { txtmtbbaPsnNo.Text.ToString() };
            object[] back_mess = yb_interface_hn_nkNew.YBRYMTBBACX(param);
            if (back_mess[1].ToString() == "1")
            {
                List<JObject> jlist = back_mess[2] as List<JObject>;
                if (jlist.Count > 0)
                {
                    this.dataGridView4.DataSource = jlist;
                }
                else
                {
                    dataGridView4.DataSource = null;
                }
                MessageBox.Show("人员慢特病备案查询成功！");
            }
            else
            {
                MessageBox.Show(back_mess[2].ToString());
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            string word = this.cmbryddywlx.SelectedItem.ToString().Substring(0, 2);
            string grbh = this.txtryddpsnNo.Text;
            object[] param = new object[] { grbh, word };
            object[] back_mess = yb_interface_hn_nkNew.YBRYLJXJCX(param);
            if (back_mess[1].ToString() == "1")
            {
                List<JObject> jlist = back_mess[2] as List<JObject>;
                if (jlist.Count > 0)
                {
                    this.dataGridView4.DataSource = jlist;
                }
                else
                {
                    dataGridView4.DataSource = null;
                }
                MessageBox.Show("人员累计信息查询成功！");

            }
            else
            {
                MessageBox.Show(back_mess[2].ToString());
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            string psnNo = this.txtljxxcxPsnNo.Text.Trim();
            string ljny = this.dtryljcxdate.Value.ToString("yyyyMM");
            object[] param = new object[] { psnNo, ljny };
            object[] back_mess = yb_interface_hn_nkNew.YBRYLJXJCX(param);
            if (back_mess[1].ToString() == "1")
            {
                List<JObject> jlist = back_mess[2] as List<JObject>;
                if (jlist.Count > 0)
                {
                    this.dataGridView4.DataSource = jlist;
                }
                else
                {
                    dataGridView4.DataSource = null;
                }
                MessageBox.Show("人员累计信息查询成功！");

            }
            else
            {
                MessageBox.Show(back_mess[2].ToString());
            }
        }

        private void button11_Click(object sender, EventArgs e)
        {
            object[] back_mess = yb_interface_hn_nkNew.YBZYDK(null);
            if (back_mess[1].ToString() == "1")
            {
                string[] strP = back_mess[2].ToString().Split('|');
                string grbh = strP[0].ToString(); //个人编号
                string strSql = string.Format(@"select xzlx,tcqh,zjlx,gmsfhm,dwmc,xm from ybickxx where grbh='{0}'", grbh);
                DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    txtdbzgrbh.Text = grbh;
                    txtdbzxzlx.Text = ds.Tables[0].Rows[0]["xzlx"].ToString();
                    txtdbzybqh.Text = ds.Tables[0].Rows[0]["tcqh"].ToString();
                    txtdbzzjlx.Text = ds.Tables[0].Rows[0]["zjlx"].ToString();
                    txtdbzzjhm.Text = ds.Tables[0].Rows[0]["gmsfhm"].ToString();
                    txtdbzdwmc.Text = ds.Tables[0].Rows[0]["dwmc"].ToString();
                    txtdbzxm.Text = ds.Tables[0].Rows[0]["xm"].ToString();
                }
            }
            else
            {
                MessageBox.Show("读卡失败|" + back_mess[2].ToString());
            }
        }

        private void button12_Click(object sender, EventArgs e)
        {
            string dbzmcyjrq = "";
            string dbzyjsyrq = "";
            string dbzsbrq = "";
            string dbzksrq = "";
            string dbzjsrq = "";
            string dbzsjlx = this.cmbdbzsjlx.SelectedItem.ToString().Substring(0, 1);
            string dcla_souc = this.cmbdbzsbyl.SelectedItem.ToString().Substring(0, 1);
            string bzbm = this.txtdbzzddm.Text;
            string bzmc = this.txtdbzzdmc.Text;
            string ssczbm = "";
            string ssczmc = "";
            string sqly = this.txtdbzsqly.Text;
            string dbrxm = this.txtdbzdbrxm.Text;
            string dbrzjlx = this.txtdbzdbrzjlx.Text;
            string dbrzjhm = this.txtdbrzjhm.Text;
            string dbrlxfs = this.txtdbzdbrlxdz.Text;
            string dbrlxdz = this.txtdbzdbrlxdz.Text;
            string dbrgx = this.txtdbzdbrgx.Text;

            if (string.IsNullOrEmpty(txtsyrybh.Text.ToString()))
            {
                MessageBox.Show("请先读卡再进行备案！");
                return;
            }

            if (!string.IsNullOrEmpty(dtdbzsbrq.Value.ToString()))
            {
                try
                {
                    dbzsbrq = dtdbzsbrq.Value.ToString("yyyy-MM-dd HH:mm:ss");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                    return;
                }
            }
            if (!string.IsNullOrEmpty(dtdbzstart.Value.ToString()))
            {
                try
                {
                    dbzksrq = dtdbzstart.Value.ToString("yyyy-MM-dd 00:00:00");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                    return;
                }
            }
            if (!string.IsNullOrEmpty(dtdbzjsrq.Value.ToString()))
            {
                try
                {
                    dbzjsrq = dtdbzjsrq.Value.ToString("yyyy-MM-dd 23:59:59");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                    return;
                }
            }
            object[] param = new object[] { dbzsjlx, dcla_souc, bzbm, bzmc, ssczbm, ssczmc, dbzsbrq, sqly, dbrxm, dbrzjlx, dbrzjhm, dbrlxfs, dbrlxdz, dbrgx, dbzksrq, dbzjsrq, "" };
            object[] back_mess = yb_interface_hn_nkNew.YBZDJBBADJ(param);
            if (back_mess[1].ToString() == "1")
            {
                MessageBox.Show("单病种登记备案成功！");
                btnsyqk_Click(sender, e);
            }
            else
            {
                MessageBox.Show(back_mess[2].ToString());
            }
        }

        private void button10_Click(object sender, EventArgs e)
        {
            foreach (Control ctl in this.plsy.Controls)
            {
                if (ctl is TextBox)
                    ctl.Text = "";
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            if (dgvsy.Rows.Count == 0)
            {
                MessageBox.Show("请先查询要撤销的备案信息，点击某行进行撤销！");
                return;
            }
            int aRow = dgvsy.CurrentRow.Index;
            string mxlsh = dgvsy.Rows[aRow].Cells["dbzdysblsh"].Value.ToString();
            string rybh = dgvsy.Rows[aRow].Cells["dbzrybh"].Value.ToString();
            object[] param = new object[] { mxlsh, rybh, "" };
            object[] back_mess = yb_interface_hn_nkNew.YBTDJBBACX(param);
            if (back_mess[1].ToString() == "1")
            {
                MessageBox.Show("单病种登记备案撤销成功！");
                btnwsqk_Click(sender, e);
            }
            else
            {
                MessageBox.Show(back_mess[2].ToString());
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            string cxtj = txtsycxtj.Text.Trim();

            StringBuilder Strsql = new StringBuilder();
            Strsql.Append("SELECT dysblsh,jbr,grbh,lxdh,lxdz,ywsqlx,ksrq,jsrq,dbrxm,dbrzjlx,dbrzjhm,");
            Strsql.Append("dbrlxdh,dbrlxdz,dbrgx,ddpxh,ddyljgbm,ddyljgmc,bz,xm FROM ybzyba where");
            Strsql.Append(" balx='DBZ' and cxbz=1");
            if (!string.IsNullOrEmpty(cxtj))
            {
                Strsql.Append(" and (grbh like  '%" + cxtj + "%' or xm like '%" + cxtj + "%') ");
            }
            DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", Strsql.ToString(), CliUtils.fLoginDB.ToString(), true, CliUtils.fCurrentProject.ToString());
            dgvsy.AutoGenerateColumns = false;
            dgvsy.DataSource = ds.Tables[0];
        }
    }
}
