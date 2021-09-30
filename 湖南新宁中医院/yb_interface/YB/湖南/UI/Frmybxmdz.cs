using Srvtools;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace yb_interfaces
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

            #region 加载医保字典
            string Sql = @"select dm as 国家项目编码,dmmc as 国家项目名称,pym 拼音码,wbm as 五笔码,isnull(sfxmzldm,'') as 收费项目类别代码,
 isnull(sfxmzl,'') as 收费项目类别名称,isnull(jbr,'') as 经办人,isnull(pzwh,'') as 批准文号
 ,isnull(jxqy,'') as 经销企业,isnull(scqybh,'') as 生产企业名称,
  isnull(remark,'') as 备注,  isnull(yczly,'')  as 药材种来源,isnull(gnzz,'') as 功能主治, 
  isnull(fbzcj,'') as 分包装厂家,isnull(bbmc,'') as 版本名称
  from ybmrdr";
            DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", Sql.ToString(), CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            if (ds.Tables[0].Rows.Count>0)
            {
                dstotal = ds;
            }
            else
            {
                MessageBox.Show("医保字典表没有查到数据！");
            }
            #endregion

            databindData();
        }
        public DataSet dstotal = new DataSet();
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
                    strSql = string.Format(@"select distinct a.y1ypno as hisxmbm,a.y1ypnm as hisxmmc,a.y1ggxx as hisgg,a.y1jxxx as hisjx,a.y1pymx as pym,a.y1sflb as sflbdm,
(select b5sfnm from bz05h where b5sfno=a.y1sflb) as sflb,a.y1ycnm as sccj
,a.y1zydw as yydw,a.y1blfs as sl,b.y8sunp as je,
                    '' as bz,c.ybxmbh,c.ybxmmc,c.xmnh,c.cwnn,c.dw,c.sm from yp01h a
                    left join yp08d b on  a.y1ypno=b.y8ypno and a.y1ypnm=b.y8ypnm and a.y1ggxx=b.y8ggxx
                    left join ybhisdzdrNew c on a.y1ypno=c.hisxmbh
                    where y1yplx in ('X','C','Z') ");
                    if (!string.IsNullOrEmpty(xmmc))
                    {
                        strSql += string.Format(@"and (a.y1ypnm like '%" + xmmc + "%' OR a.y1pymx like '%" + xmmc + "%' ");
                        strSql += string.Format(@"or c.ybxmbh like '%" + xmmc + "%' or a.y1ypno like '%" + xmmc + "%' )");
                    }

                    break;
                case "2": //材料
                    strSql = string.Format(@"select distinct a.y1ypno as hisxmbm,a.y1ypnm as hisxmmc,a.y1ycnm as sccj,a.y1sflb as sflbdm,(select h.b5sfnm from bz05h h where h.b5sfno=y1sflb) as sflb ,a.y1ggxx as hisgg,a.y1jxxx as hisjx,a.y1pymx as pym,
a.y1zydw as yydw,a.y1blfs as sl,b.y8sunp as je,'' as bz,c.ybxmbh,c.xmnh,c.ybxmmc,c.cwnn,c.dw,c.sm
 from yp01h a 
 left join yp08d b on  a.y1ypno=b.y8ypno and a.y1ggxx=b.y8ggxx 
                                            left join ybhisdzdrNew c on a.y1ypno=c.hisxmbh
                                            where y1yplx in ('W') ");
                    if (!string.IsNullOrEmpty(xmmc))
                    {
                        strSql += string.Format(@"and (a.y1ypnm like '%" + xmmc + "%' OR a.y1pymx like '%" + xmmc + "%' ");
                        strSql += string.Format(@"or c.ybxmbh like '%" + xmmc + "%' or a.y1ypno like '%" + xmmc + "%' )");
                    }
                    break;

                case "3": //诊疗项目
                    strSql = string.Format(@"with zlxm as(select a.b5item as hisxmbm,a.b5name as hisxmmc,'' as hisgg,'' as hisjx,a.b5pymx as pym,'' as sccj,a.b5sfno as sflbdm,(select b5sfnm from bz05h where b5sfno=a.b5sfno) as sflb,a.b5unit as dw,1 as sl,a.b5sfam as je,
                                            '' as bz from bz05d a ) , zlxm1 as(select a.hisxmbm,a.hisxmmc,hisgg,hisjx,a.pym,a.sccj,a.sflbdm,a.sflb,a.dw as yydw,sl,a.je,a.bz,  c.ybxmbh,c.ybxmmc,c.xmnh,c.cwnn,c.dw,c.sm   from zlxm a 
                                            left join ybhisdzdrNew c on a.hisxmbm=c.hisxmbh) select * from zlxm1 a  where 1=1  ");//where a.b5chk1=1
                    if (!string.IsNullOrEmpty(xmmc))
                    {
                        strSql += string.Format(@"and (a.hisxmbm like '%" + xmmc + "%' or a.pym like '%" + xmmc + "%' ");
                        strSql += string.Format(@"or a.hisxmmc like '%" + xmmc + "%' or a.ybxmbh like '%" + xmmc + "%') ");
                    }
                    break;
                case "4": //其他
                    strSql = string.Format(@"with zlxm as(select a.b5item as hisxmbm,a.b5name as hisxmmc,'' as hisgg,'' as hisjx,a.b5pymx as pym,'' as sccj,a.b5sfno as sflbdm,(select b5sfnm from bz05h where b5sfno=a.b5sfno) as sflb,a.b5unit as dw,1 as sl,a.b5sfam as je,
                                            '' as bz from bz05d a ) , zlxm1 as(select a.hisxmbm,a.hisxmmc,hisgg,hisjx,a.pym,a.sccj,a.sflbdm,a.sflb,a.dw as yydw,sl,a.je,a.bz,  c.ybxmbh,c.ybxmmc,c.xmnh,c.cwnn,c.dw,c.sm   from zlxm a 
                                            left join ybhisdzdrNew c on a.hisxmbm=c.hisxmbh) select * from zlxm1 a where 1!=1 ");
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
                    left join ybhisdzdrNew c on a.y1ypno=c.hisxmbh 
                    where y1yplx in ('X','C','Z') ");
                    break;
                case "2"://材料
                    strSql = string.Format(@"select COUNT(a.y1ypno) AS ypgs, SUM( case when ISNULL(c.ybxmbh,'')='' then 0 else 1 end) yzdgs,SUM( case when ISNULL(c.ybxmbh,'')='' then 1 else 0 end) wzdgs from yp01h a 
                    left join ybhisdzdrNew c on a.y1ypno=c.hisxmbh 
                    where y1yplx in ('W') ");
                    break;
                case "3": //诊疗项目    
                    strSql = string.Format(@"select COUNT(a.b5item) as ypgs, SUM( case when ISNULL(c.ybxmbh,'')='' then 0 else 1 end) yzdgs,SUM( case when ISNULL(c.ybxmbh,'')='' then 1 else 0 end) wzdgs  from bz05d a 
                    left join ybhisdzdrNew c on a.b5item=c.hisxmbh");
                    break;
                case "4": //其他
                    strSql = string.Format(@"select COUNT(a.b5item) as ypgs, isnull(SUM( case when ISNULL(c.ybxmbh,'')='' then 0 else 1 end),0) yzdgs,isnull(SUM( case when ISNULL(c.ybxmbh,'')='' then 1 else 0 end),0) wzdgs  from bz05d a 
                    left join ybhisdzdrNew c on a.b5item=c.hisxmbh where 1!=1");
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
            string sysdate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string yyxmbh = txtYYXMBM.Text;
            string yyxmmc = txtYYXMMC.Text;
            string ybxmbm = TXTYBXMBM.Text.Trim();
            string ybxmmc = TXTYBXMMC.Text.Trim();
            string xmnh = TXTXMNH.Text.Trim();
            string cwnn = TXTCWNN.Text.Trim();
            string dw = TXTDW.Text.Trim();
            string sm = TXTSM.Text.Trim();
            string begindate = DateTime.Now.ToString("yyyy-MM-dd 00:00:00"); 
            string endDate = DateTime.Now.ToString("yyyy-MM-dd 23:59:59");

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
            string xmlbCode = this.lblsfxmzldm.Text;
            string xmlbName = this.lblsfxmzlmc.Text;
            //医保对照上传
            object[] scres = yb_interface_hn_nkNew.YBDZXXPLSC(new object[] { "2", yyxmbh, ybxmbm });
            if (scres[1].ToString()!="1")
            {
                MessageBox.Show("医保对照上传成功！" + scres[2].ToString()) ;
            }

            List<string> liSql = new List<string>();
            string strSql = string.Format(@"delete from ybhisdzdrNew where hisxmbh='{0}'", yyxmbh);
            liSql.Add(strSql);
            strSql = string.Format(@"insert into ybhisdzdrNew(hisxmbh,hisxmmc,ybxmbh,ybxmmc,xmnh,cwnn,dw,sm,sfxmzldm,sfxmzl,ksrq,jsrq,sflbdm,sflb) values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}')",
                                    yyxmbh, yyxmmc, ybxmbm, ybxmmc, xmnh, cwnn, dw, sm, xmlbCode, xmlbName, begindate, endDate,sflbdm,sflb);
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
                WriteLog(sysdate+"|项目配对失败Sql语句：" + strSql);
                MessageBox.Show("项目配对失败！");
            }
        }
        public void WriteLog(string str)
        {
            if (!Directory.Exists("YBLog\\YBSql"))
            {
                Directory.CreateDirectory("YBLog\\YBSql");
            }
            FileStream stream = new FileStream("YBLog\\YBSql\\YBSql" + DateTime.Now.ToString("yyyyMMdd") + ".txt", FileMode.Append, FileAccess.Write);
            StreamWriter writer = new StreamWriter(stream);
            writer.WriteLine(str);
            writer.Close();
            stream.Close();
        }
        /// <summary>
        /// 收费类别
        /// </summary>
        private string sflbdm = "";
        /// <summary>
        /// 收费类别名称
        /// </summary>
        private string sflb = "";
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
            sflb = dgv_hisInfo.CurrentRow.Cells["col_sflb"].Value.ToString();
            sflbdm= dgv_hisInfo.CurrentRow.Cells["col_sflbdm"].Value.ToString();
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
            //医保配对撤销
            object[] inparams = new object[] { yyxmbh };
            object[] res = yb_interface_hn_nkNew.YBDZXXSCCX(inparams);
            if (res[1].ToString()!="1")
            {
                MessageBox.Show(res[2].ToString());
                return;
            }
            List<string> liSql = new List<string>();
            string strSql = string.Format(@"delete from ybhisdzdrNew where hisxmbh='{0}'", yyxmbh);
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
         
        private void dgv_hisInfo_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {

            string xmmc = dgv_hisInfo.Rows[e.RowIndex].Cells["col_hisxmmc"].Value.ToString();
            sflb = dgv_hisInfo.CurrentRow.Cells["col_sflb"].Value.ToString();
            sflbdm = dgv_hisInfo.CurrentRow.Cells["col_sflbdm"].Value.ToString();
            Frmzd zd = new Frmzd(xmmc,dstotal);
            zd.ShowDialog();
            if (zd.dr!=null)
            {
                this.TXTYBXMBM.Text = zd.dr.Cells["国家项目编码"].Value.ToString();
                this.TXTYBXMMC.Text = zd.dr.Cells["国家项目名称"].Value.ToString();
                this.lblsfxmzldm.Text = zd.dr.Cells["收费项目类别代码"].Value.ToString();
                this.lblsfxmzlmc.Text = zd.dr.Cells["收费项目类别名称"].Value.ToString();
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            string hisxmbh = this.txtYYXMBM.Text;
            string ybxmbh = this.TXTYBXMBM.Text;

            object[] inparams = new object[] { "1" };
            if (!string.IsNullOrEmpty(hisxmbh))
            {
                inparams = new object[] { "2", hisxmbh, ybxmbh };
            }
            object[] objres = yb_interface_hn_nkNew.YBDZXXPLSC(inparams);
            MessageBox.Show(objres[2].ToString());
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string yyxmbh = txtYYXMBM.Text;
            string yyxmmc = txtYYXMMC.Text;

            if (string.IsNullOrEmpty(yyxmbh) || string.IsNullOrEmpty(yyxmmc))
            {
                MessageBox.Show("请选择医院项目");
                return;
            }
            //医保配对撤销
            object[] inparams = new object[] {yyxmbh };
            object[] res = yb_interface_hn_nkNew.YBDZXXSCCX(inparams);
            if (res[1].ToString() != "1")
            {
                MessageBox.Show(res[2].ToString());
                return;
            }
        }
    }
    public class Info
    {
        public string Id { get; set; }
        public string Name { get; set; }

    }
}
