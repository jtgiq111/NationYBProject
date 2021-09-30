using Srvtools;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ybinterface_lib
{
    public partial class Frmybxmdz : InfoForm
    {
        #region 变量
        public static string hislx = "1"; //分类
        public static string dzzt = "0"; //状态
        public static string xmmc = ""; //项目名称
        #endregion

        public Frmybxmdz()
        {
            InitializeComponent();
        }

        private void Form3_Load(object sender, EventArgs e)
        {
            #region 加载配对状态信息
            IList<Info> infoList = new List<Info>();
            Info info1 = new Info() { Id = "0", Name = "全部" };
            Info info2 = new Info() { Id = "1", Name = "已配对" };
            Info info3 = new Info() { Id = "-1", Name = "未配对" };
            infoList.Add(info1);
            infoList.Add(info2);
            infoList.Add(info3);
            cmb_dzzt.DataSource = infoList;
            cmb_dzzt.ValueMember = "Id";
            cmb_dzzt.DisplayMember = "Name";
            cmb_dzzt.SelectedValue = "0";
            #endregion

            #region 加载配对项目分类
            IList<Info> infoList1 = new List<Info>();
            Info info11 = new Info() { Id = "1", Name = "药品(西药、中成药、中草药)" };
            Info info22 = new Info() { Id = "2", Name = "材料(科室用材料、一次性材料)" };
            Info info33 = new Info() { Id = "3", Name = "诊疗项目" };
            Info info44 = new Info() { Id = "4", Name = "其他" };
            infoList1.Add(info11);
            infoList1.Add(info22);
            infoList1.Add(info33);
            infoList1.Add(info44);
            cmb_hislx.DataSource = infoList1;
            cmb_hislx.ValueMember = "Id";
            cmb_hislx.DisplayMember = "Name";
            cmb_hislx.SelectedValue = "1";
            #endregion

            databindData();
        }

        /// <summary>
        /// 依据查询条件，获取数据并绑定
        /// </summary>
        /// <param name="param1"></param>
        /// <param name="param2"></param>
        /// <param name="param3"></param>
        internal void databindData()
        {
            #region  依据查询条件，获取数据并绑定
            xmmc = txt_index.Text.Trim();
            string strSql="";
            switch (hislx)
            {
                case "1": //药品
                    strSql=string.Format(@"select distinct a.y1ypno as hisxmbm,a.y1ypnm as hisxmmc,a.y1ggxx as hisgg,a.y1jxxx as hisjx,a.y1pymx as pym,a.y1zydw as yydw,a.y1blfs as sl,b.y8sunp as je,
                    '' as bz,c.ybxmbh,c.ybxmmc,c.xmnh,c.cwnn,c.dw,c.sm from yp01h a
                    left join yp08d b on  a.y1ypno=b.y8ypno and a.y1ypnm=b.y8ypnm and a.y1ggxx=b.y8ggxx
                    left join ybhisdzgmdrNew c on a.y1ypno=c.hisxmbh
                    where y1yplx in ('X','C','Z') ");
                    if (!string.IsNullOrEmpty(xmmc))
                    {
                        strSql+=string.Format(@"and (a.y1ypnm like '%" + xmmc + "%' OR a.y1pymx like '%" + xmmc + "%' ");
                        strSql += string.Format(@"or c.ybxmbh like '%" + xmmc + "%' or a.y1ypno like '%" + xmmc + "%' )");
                    }

                    break;
                case "2": //材料
                    strSql = string.Format(@"select distinct a.y1ypno as hisxmbm,a.y1ypnm as hisxmmc,a.y1ggxx as hisgg,a.y1jxxx as hisjx,a.y1pymx as pym,a.y1zydw as yydw,a.y1blfs as sl,b.y8sunp as je,
                                            '' as bz,c.ybxmbh,c.xmnh,c.ybxmmc,c.cwnn,c.dw,c.sm from yp01h a 
                                            left join yp08d b on  a.y1ypno=b.y8ypno and a.y1ggxx=b.y8ggxx 
                                            left join ybhisdzgmdrNew c on a.y1ypno=c.hisxmbh
                                            where y1yplx in ('W') ");
                    if (!string.IsNullOrEmpty(xmmc))
                    {
                        strSql += string.Format(@"and (a.y1ypnm like '%" + xmmc + "%' OR a.y1pymx like '%" + xmmc + "%' ");
                        strSql += string.Format(@"or c.ybxmbh like '%" + xmmc + "%' or a.y1ypno like '%" + xmmc + "%' )");
                    }
                    break;

                case "3": //诊疗项目
                    strSql = string.Format(@"with zlxm as(select a.b5item as hisxmbm,a.b5name as hisxmmc,'' as hisgg,'' as hisjx,a.b5pymx as pym,a.b5unit as dw,1 as sl,a.b5sfam as je,
                                            '' as bz from bz05d a where a.b5chk1=1) , zlxm1 as(select a.hisxmbm,a.hisxmmc,hisgg,hisjx,a.pym,a.dw as yydw,sl,a.je,a.bz,  c.ybxmbh,c.ybxmmc,c.xmnh,c.cwnn,c.dw,c.sm   from zlxm a 
                                            left join ybhisdzgmdrNew c on a.hisxmbm=c.hisxmbh) select * from zlxm1 a  where 1=1 ");
                    if (!string.IsNullOrEmpty(xmmc))
                    {
                        strSql += string.Format(@"and (a.hisxmbm like '%" + xmmc + "%' or a.pym like '%" + xmmc + "%' ");
                        strSql += string.Format(@"or a.hisxmmc like '%" + xmmc + "%' or a.ybxmbh like '%" + xmmc + "%') ");
                    }
                    break;
                case "4": //其他
                    strSql = string.Format(@"with zlxm as(select a.b5item as hisxmbm,a.b5name as hisxmmc,'' as hisgg,'' as hisjx,a.b5pymx as pym,a.b5unit as dw,1 as sl,a.b5sfam as je,
                                            '' as bz from bz05d a where a.b5chk1=1) , zlxm1 as(select a.hisxmbm,a.hisxmmc,hisgg,hisjx,a.pym,a.dw as yydw,sl,a.je,a.bz,  c.ybxmbh,c.ybxmmc,c.xmnh,c.cwnn,c.dw,c.sm   from zlxm a 
                                            left join ybhisdzgmdrNew c on a.hisxmbm=c.hisxmbh) select * from zlxm1 a  where 1!=1 ");
                    break;
            }
            switch (dzzt)
            {
                case "0": //全部
                    strSql += " ";
                    break;
                case "1": //已配对
                    if (hislx.Equals("1") || hislx.Equals("2"))
                       strSql += "and a.y1ypno=c.hisxmbh ";
                    else if (hislx.Equals("3"))
                         strSql += "and isnull(a.ybxmbh,'')!='' ";
                    break;
                case "-1": //未配对
                    if (hislx.Equals("1") || hislx.Equals("2"))
                        strSql += "and c.hisxmbh is null";
                    else if (hislx.Equals("3"))
                        strSql += "and isnull(a.ybxmbh,'')='' ";
                    break;
            }

            DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            dgv_hisInfo.DataSource = ds.Tables[0].DefaultView;
            dgv_hisInfo.AutoGenerateColumns = false;
            dgv_hisInfo.ClearSelection();
            #endregion

            狗屁需求();
        }

        internal void 狗屁需求()
        {
            string strSql = "";
            switch (hislx)
            {
                case "1": //药品

                    strSql = string.Format(@" select COUNT(a.y1ypno) AS ypgs, SUM( case when ISNULL(c.ybxmbh,'')='' then 0 else 1 end) yzdgs,SUM( case when ISNULL(c.ybxmbh,'')='' then 1 else 0 end) wzdgs from yp01h a 
                    left join ybhisdzgmdrNew c on a.y1ypno=c.hisxmbh 
                    where y1yplx in ('X','C','Z') ");
                    break;
                case "2"://材料
                    strSql = string.Format(@"select COUNT(a.y1ypno) AS ypgs, SUM( case when ISNULL(c.ybxmbh,'')='' then 0 else 1 end) yzdgs,SUM( case when ISNULL(c.ybxmbh,'')='' then 1 else 0 end) wzdgs from yp01h a 
                    left join ybhisdzgmdrNew c on a.y1ypno=c.hisxmbh 
                    where y1yplx in ('W') ");
                    break;
                case "3": //诊疗项目    
                    strSql = string.Format(@"select COUNT(a.b5item) as ypgs, SUM( case when ISNULL(c.ybxmbh,'')='' then 0 else 1 end) yzdgs,SUM( case when ISNULL(c.ybxmbh,'')='' then 1 else 0 end) wzdgs  from bz05d a 
                    left join ybhisdzgmdrNew c on a.b5item=c.hisxmbh");
                    break;
                case "4": //其他
                    strSql = string.Format(@"select COUNT(a.b5item) as ypgs, isnull(SUM( case when ISNULL(c.ybxmbh,'')='' then 0 else 1 end),0) yzdgs,isnull(SUM( case when ISNULL(c.ybxmbh,'')='' then 1 else 0 end),0) wzdgs  from bz05d a 
                    left join ybhisdzgmdrNew c on a.b5item=c.hisxmbh where 1!=1");
                    break;
            }

            DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql.ToString(), CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            label12.Text = "总计数:" + ds.Tables[0].Rows[0]["ypgs"].ToString() + "; 已对照数:" + ds.Tables[0].Rows[0]["yzdgs"].ToString() + ";未对照数:" + ds.Tables[0].Rows[0]["wzdgs"].ToString();
        }


        private void cmb_hislx_SelectedIndexChanged(object sender, EventArgs e)
        {
            hislx = cmb_hislx.SelectedValue.ToString();
            if (hislx.Length != 1)
                return;
            databindData();
        }

        private void cmb_dzzt_SelectedIndexChanged(object sender, EventArgs e)
        {
            dzzt = cmb_dzzt.SelectedValue.ToString();
            databindData();
        }

        private void txt_index_TextChanged(object sender, EventArgs e)
        {
            xmmc = txt_index.Text.Trim();
            databindData();
        }


        private void button1_Click(object sender, EventArgs e)
        {
            string yyxmbh = txtYYXMBM.Text;
            string yyxmmc = txtYYXMMC.Text;
            string ybxmbm = TXTYBXMBM.Text.Trim();
            string ybxmmc = TXTYBXMMC.Text.Trim();
            string xmnh = TXTXMNH.Text.Trim();
            string cwnn = TXTCWNN.Text.Trim();
            string dw = TXTDW.Text.Trim();
            string sm = TXTSM.Text.Trim();

            if (string.IsNullOrEmpty(yyxmbh) || string.IsNullOrEmpty(yyxmmc))
            {
                MessageBox.Show("请选择医院项目");
                return;
            }
            if (string.IsNullOrEmpty(ybxmbm) || string.IsNullOrEmpty(ybxmmc))
            {
                MessageBox.Show("请输入国家项目代码或名称");
                TXTYBXMBM.Focus();
                return;
            }
            string xmlbCode = "";
            string xmlbName = "";
            if (ybxmbm.Contains("XM"))
            {
                xmlbCode = "2";
                xmlbName = "诊疗项目";
            }
            else if (ybxmbm.Contains("CL"))
            {
                xmlbCode = "3";
                xmlbName = "材料项目";
            }
            else
            {
                xmlbCode = "1";
                xmlbName = "药品项目";
            }
            List<string> liSql = new List<string>();
            string strSql = string.Format(@"delete from ybhisdzgmdrNew where hisxmbh='{0}'", yyxmbh);
            liSql.Add(strSql);
            strSql = string.Format(@"insert into ybhisdzgmdrNew(hisxmbh,hisxmmc,ybxmbh,ybxmmc,xmnh,cwnn,dw,sm,sfxmzldm,sfxmzl) values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}')",
                                    yyxmbh, yyxmmc, ybxmbm, ybxmmc, xmnh, cwnn, dw, sm,xmlbCode,xmlbName);
            liSql.Add(strSql);
            object[] obj = liSql.ToArray();
            obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);
            if (obj[1].ToString().Equals("1"))
            {
                dgv_hisInfo.CurrentRow.DefaultCellStyle.BackColor = Color.Yellow;
                this.dgv_hisInfo.CurrentRow.Cells["col_ybxmbh"].Value = ybxmbm;
                this.dgv_hisInfo.CurrentRow.Cells["col_ybxmmc"].Value = ybxmmc;
                this.dgv_hisInfo.CurrentRow.Cells["col_xmnh"].Value = xmnh;
                this.dgv_hisInfo.CurrentRow.Cells["col_cwnn"].Value = cwnn;
                this.dgv_hisInfo.CurrentRow.Cells["col_dw"].Value = dw;
                this.dgv_hisInfo.CurrentRow.Cells["col_sm"].Value = sm;
                MessageBox.Show("项目配对成功！");
            }
            else
            {
                MessageBox.Show("项目配对失败！");
            }
        }

        private void dgv_hisInfo_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            dgv_hisInfo.EndEdit();
            txtYYXMBM.Text = dgv_hisInfo.CurrentRow.Cells["col_hisxmbm"].Value.ToString();
            txtYYXMMC.Text = dgv_hisInfo.CurrentRow.Cells["col_hisxmmc"].Value.ToString();
            TXTYBXMBM.Text = dgv_hisInfo.CurrentRow.Cells["col_ybxmbh"].Value.ToString();
            TXTYBXMMC.Text = dgv_hisInfo.CurrentRow.Cells["col_ybxmmc"].Value.ToString();
            TXTXMNH.Text = dgv_hisInfo.CurrentRow.Cells["col_xmnh"].Value.ToString();
            TXTCWNN.Text = dgv_hisInfo.CurrentRow.Cells["col_cwnn"].Value.ToString();
            TXTDW.Text = dgv_hisInfo.CurrentRow.Cells["col_dw"].Value.ToString();
            TXTSM.Text = dgv_hisInfo.CurrentRow.Cells["col_sm"].Value.ToString();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string yyxmbh = txtYYXMBM.Text;
            string yyxmmc = txtYYXMMC.Text;

            if (string.IsNullOrEmpty(yyxmbh) || string.IsNullOrEmpty(yyxmmc))
            {
                MessageBox.Show("请选择医院项目");
                return;
            }
            List<string> liSql = new List<string>();
            string strSql = string.Format(@"delete from ybhisdzgmdrNew where hisxmbh='{0}'", yyxmbh);
            liSql.Add(strSql);
            object[] obj = liSql.ToArray();
            obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);

            if (obj[1].ToString().Equals("1"))
            {
                dgv_hisInfo.CurrentRow.DefaultCellStyle.BackColor = Color.White;
                this.dgv_hisInfo.CurrentRow.Cells["col_ybxmbh"].Value = "";
                this.dgv_hisInfo.CurrentRow.Cells["col_ybxmmc"].Value = "";
                this.dgv_hisInfo.CurrentRow.Cells["col_xmnh"].Value = "";
                this.dgv_hisInfo.CurrentRow.Cells["col_cwnn"].Value = "";
                this.dgv_hisInfo.CurrentRow.Cells["col_dw"].Value = "";
                this.dgv_hisInfo.CurrentRow.Cells["col_sm"].Value = "";
                MessageBox.Show("项目配对撤销成功！");
            }
            else
            {
                MessageBox.Show("项目配对撤销失败！");
            }
        }


    }
}
