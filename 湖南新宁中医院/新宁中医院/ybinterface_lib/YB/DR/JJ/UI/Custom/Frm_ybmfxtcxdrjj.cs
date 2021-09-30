using Srvtools;
using System;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace ybinterface_lib
{
    public partial class Frm_ybmfxtcxdrjj : InfoForm
    {
        GocentPara Para = new GocentPara();
        DataTable dt;

        public Frm_ybmfxtcxdrjj()
        {
            InitializeComponent();
            dgvbf.AutoGenerateColumns = false;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            cmb_tj.Items.Add("");
            cmb_tj.Items.Add("流水号");
            cmb_tj.Items.Add("发票号");
            cmb_tj.Items.Add("姓名");
            cmb_tj.Items.Add("身份证号");
            cmb_tj.Items.Add("民政救助>0");
            string sql = "select bzkeyx, bzname from bztbd(nolock) where bzcodn = 'DQ' and bzusex = 1";//0秒
            DataTable dt = CliUtils.ExecuteSql("sybdj", "cmd", sql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject).Tables[0];
            DataRow dr = dt.NewRow();
            dr["bzname"] = "";
            dr["bzkeyx"] = "";
            dt.Rows.InsertAt(dr, 0);
            cmb_tcq.DataSource = dt;
            cmb_tcq.ValueMember = "bzkeyx";
            cmb_tcq.DisplayMember = "bzname";
        }

        private void btncx_Click(object sender, EventArgs e)
        {
            //            string sql = string.Format(@"select g.bzname tcq, b.m4ghno lsh, b.m4invo fph, a.xm, c.b5sfnm sflb, b.m4name xmmc, b.m4name mbmc, b.m4pric dj
            //                , d.b1name kdys, e.b1name sfy, sum(case when a.cxbz in (1, 2) then b.m4quty else -b.m4quty end) sl
            //                , sum(case when a.cxbz in (1, 2) then b.m4amnt else -b.m4amnt end) je
            //                , max(a.sysdate) sfsj
            //                , case when a.cxbz in (1, 2) then a.tcjjzf else -a.tcjjzf end tcjjzf
            //                , case when a.cxbz in (1, 2) then a.dejjzf else -a.dejjzf end dejjzf
            //                , case when a.cxbz in (1, 2) then a.xjzf else -a.xjzf end xjzf
            //                , case when a.cxbz in (1, 2) then a.yyfdfy else -a.yyfdfy end yyfdfy
            //                , case when a.cxbz in (1, 2) then a.mzjzfy else -a.mzjzfy end mzjzfy
            //                , case when a.cxbz in (1, 2) then a.jrtcfy else -a.jrtcfy end jrtcfy
            //                from ybmzzydjdr f 
            //                join ybfyjsdr a on f.jzlsh = a.jzlsh
            //                join mz04d b on a.jzlsh = b.m4ghno and a.djhin = b.m4shno
            //                join bz05h c on b.m4sfno = c.b5sfno
            //                join bz01h d on b.m4empn = d.b1empn
            //                join bz01h e on b.m4user = e.b1empn
            //                join bztbd g on f.tcqh = g.bzkeyx
            //                where g.bzcodn = 'DQ' and a.yllb = '35' and a.sysdate between '{0}' and '{1}'
            //                and left(b.m4endv, 1) = '3' and f.cxbz = 1"
            //                , dateTimeStar.Value.ToString("yyyy-MM-dd"), dateTimeEnd.Value.AddDays(1).ToString("yyyy-MM-dd"));
            string sql = string.Format(@"select g.bzname tcq, b.m4ghno lsh, b.m4invo fph, a.xm, f.sfzh, b.m4name mbmc, b.m4pric dj
                , d.b1name kdys, e.b1name sfy
                , sum(b.m4quty) sl
                , sum(b.m4amnt) je
                , max(a.sysdate) sfsj
                , a.tcjjzf tcjjzf
                , a.dejjzf dejjzf
                , a.xjzf xjzf
                , a.yyfdfy yyfdfy
                , a.mzjzfy mzjzfy
                , a.jrtcfy jrtcfy
                from ybmzzydjdr(nolock) f 
                join ybfyjsdr(nolock) a on f.jzlsh = a.jzlsh
                join mz04d(nolock) b on a.jzlsh = b.m4ghno and a.djhin = b.m4shno
                join bz05h(nolock) c on b.m4sfno = c.b5sfno
                join bz01h(nolock) d on b.m4empn = d.b1empn
                join bz01h(nolock) e on b.m4user = e.b1empn
                join bztbd(nolock) g on f.tcqh = g.bzkeyx
                where left(a.jsrq, 8) between '{0}' and '{1}' and g.bzcodn = 'DQ' and g.bzusex = 1 and a.yllb = '35'
                and left(b.m4endv, 1) = '3' and f.cxbz = 1 and a.cxbz = 1"
                , dateTimeStar.Value.ToString("yyyyMMdd"), dateTimeEnd.Value.ToString("yyyyMMdd"));

            if (cmb_tj.Text == "流水号")
            {
                sql += string.Format(" and b.m4ghno = '{0}'", txt_nr.Text.Trim());
            }
            else if (cmb_tj.Text == "发票号")
            {
                sql += string.Format(" and b.m4invo = '{0}'", txt_nr.Text.Trim());
            }
            else if (cmb_tj.Text == "姓名")
            {
                sql += string.Format(" and a.xm = '{0}'", txt_nr.Text.Trim());
            }
            else if (cmb_tj.Text == "身份证号")
            {
                sql += string.Format(" and f.sfzh = '{0}'", txt_nr.Text.Trim());
            }
            else if (cmb_tj.Text == "民政救助>0")
            {
                sql += " and a.mzjzfy > 0";
            }

            if (cmb_tcq.Text != "")
            {
                sql += string.Format(" and f.tcqh = '{0}'", cmb_tcq.SelectedValue);
            }

            //            sql += @" group by g.bzname, b.m4ghno, b.m4invo, a.xm, c.b5sfnm, b.m4name, b.m4pric, d.b1name, e.b1name
            //            , case when a.cxbz in (1, 2) then a.tcjjzf else -a.tcjjzf end
            //            , case when a.cxbz in (1, 2) then a.dejjzf else -a.dejjzf end
            //            , case when a.cxbz in (1, 2) then a.xjzf else -a.xjzf end
            //            , case when a.cxbz in (1, 2) then a.yyfdfy else -a.yyfdfy end
            //            , case when a.cxbz in (1, 2) then a.mzjzfy else -a.mzjzfy end
            //            , case when a.cxbz in (1, 2) then a.jrtcfy else -a.jrtcfy end
            //            , a.cxbz";
            sql += @" group by g.bzname, b.m4ghno, b.m4invo, a.xm, f.sfzh, b.m4name, b.m4pric, d.b1name, e.b1name
                        , a.tcjjzf
                        , a.dejjzf
                        , a.xjzf
                        , a.yyfdfy
                        , a.mzjzfy
                        , a.jrtcfy";//7秒
            Common.WriteYBLog(sql);
            dt = CliUtils.ExecuteSql("sybdj", "cmd", sql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject).Tables[0];
            dgvbf.DataSource = dt;

            if (dt != null && dt.Rows.Count > 0)
            {
                btnDC.Enabled = true;
            }
            else
            {
                btnDC.Enabled = false;
            }

            lbl_rc.Text = "人次：" + dt.Rows.Count.ToString();
        }

        private void btnDC_Click(object sender, EventArgs e)
        {
            string filename = "九江市中医医院门诊免费血透患者信息明细表(按自然日期统计).xls";
            ColumnMap[] maps = { new ColumnMap("tcq", "统筹区")
                               , new ColumnMap("lsh", "流水号")
                               , new ColumnMap("fph", "发票号")
                               , new ColumnMap("xm", "姓名")
                               , new ColumnMap("sfzh", "身份证号")
                               , new ColumnMap("mbmc", "模板名称")
                               , new ColumnMap("dj", "单价")
                               , new ColumnMap("kdys", "开单医生")
                               , new ColumnMap("sfy", "收费员")
                               , new ColumnMap("sl", "数量")
                               , new ColumnMap("je", "金额")
                               , new ColumnMap("sfsj", "收费时间")
                               , new ColumnMap("tcjjzf", "统筹支付")
                               , new ColumnMap("dejjzf", "大额支付")
                               , new ColumnMap("xjzf", "现金支付")
                               , new ColumnMap("yyfdfy", "医院负担")
                               , new ColumnMap("mzjzfy", "民政救助")
                               , new ColumnMap("jrtcfy", "进入统筹")
                               };

            DataTableToExcel.Dt2File(dt, maps, filename, true);
            //MessageBox.Show(Environment.CurrentDirectory + "\\" + filename );
            Process.Start("excel.exe", Environment.CurrentDirectory + "\\" + filename);
        }

        private void btn_Close_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btn_dccvs_Click(object sender, EventArgs e)
        {
            //string filename = "九江市中医医院门诊免费血透患者信息明细表(按自然日期统计).xls";

            try
            {
                //Common.Dcexcel(dgvbf, filename);

                InfoDataSet ids = new InfoDataSet();
                ids.RealDataSet = Common.GetDataSetFromDataGridView(dgvbf);
                SaveFileDialog dlg = new SaveFileDialog();
                dlg.AutoUpgradeEnabled = false;
                dlg.Filter = "*.xls|*.xls";

                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    if (ids.ToExcel(0, dlg.FileName))
                    {
                        MessageBox.Show("导出成功");
                    }
                    else
                    {
                        MessageBox.Show("导出失败");
                    }
                }
            }
            catch (Exception error)
            {
                MessageBox.Show(error.ToString());
            }
        }
    }
}

