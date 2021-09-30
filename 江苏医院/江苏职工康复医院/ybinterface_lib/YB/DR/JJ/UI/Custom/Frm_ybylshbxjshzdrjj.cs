using FastReport;
using Srvtools;
using System;
using System.Data;
using System.IO;
using System.Windows.Forms;

namespace ybinterface_lib
{
    public partial class Frm_ybylshbxjshzdrjj : InfoForm
    {
        GocentPara Para = new GocentPara();
        string bbtjlx;
        DataSet ds;

        public Frm_ybylshbxjshzdrjj()
        {
            InitializeComponent();
            dgvbf.AutoGenerateColumns = false;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            string sql = "select bzmem1, bzname from bztbd(nolock) where bzcodn = 'YL'";
            DataTable dt = CliUtils.ExecuteSql("sybdj", "cmd", sql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject).Tables[0];
            DataRow dr = dt.NewRow();
            dr["bzname"] = "";
            dr["bzmem1"] = "";
            dt.Rows.InsertAt(dr, 0);
            cmbyllb.DataSource = dt;
            cmbyllb.DisplayMember = "bzname";
            cmbyllb.ValueMember = "bzmem1";
            sql = "select bzkeyx, bzname from bztbd(nolock) where bzcodn = 'DQ' and bzusex = 1";
            dt = CliUtils.ExecuteSql("sybdj", "cmd", sql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject).Tables[0];
            dr = dt.NewRow();
            dr["bzname"] = "";
            dr["bzkeyx"] = "";
            dt.Rows.InsertAt(dr, 0);
            cmb_tcq.DataSource = dt;
            cmb_tcq.DisplayMember = "bzname";
            cmb_tcq.ValueMember = "bzkeyx";
            rbtnrylx_Click(null, null);
        }

        private void btnprint_Click(object sender, EventArgs e)
        {
            Report report = new Report();
            string s_path = Para.sys_parameter("BZ", "BZ0007");
            s_path = string.IsNullOrEmpty(s_path) ? @"C:\Program Files (x86)\Infolight\EEP2012\EEPNetClient\FastReport" : s_path;
            string c_file = Application.StartupPath + @"\FastReport\YB\九江市医疗社会保险结算拨付汇总表.frx";
            string s_file = s_path + @"\YB\九江市医疗社会保险结算拨付汇总表.frx";
            CliUtils.DownLoad(s_file, c_file);

            if (!File.Exists(c_file))
            {
                MessageBox.Show(c_file + "不存在!");
                return;
            }

            report.Load(c_file);
            report.RegisterData(ds);
            report.SetParameterValue("comp", "九江市中医医院");
            report.SetParameterValue("bbbt", "九江市医疗社会保险结算拨付汇总表");
            report.SetParameterValue("yydj", "三级医院");
            report.SetParameterValue("ksrq", dateTimeStar.Value.ToString("yyyy-MM-dd"));
            report.SetParameterValue("jsrq", dateTimeEnd.Value.ToString("yyyy-MM-dd"));
            report.SetParameterValue("bbtjlx", bbtjlx);
            report.SetParameterValue("printDate", DateTime.Now.ToString("yyyy-MM-dd"));
            report.SetParameterValue("tcq", cmb_tcq.Text);

            if (CliUtils.fPrintPreview == "True")
            {
                report.Show();
            }
            else
            {
                report.Print();
            }

            report.Dispose();
        }

        private void btncx_Click(object sender, EventArgs e)
        {
            //            string sql = string.Format(@"select sum(case when b.cxbz in (1, 2) then b.je else -b.je end) z3amt2
            //                , c.sflb 
            //                from ybmzzydjdr a
            //                join ybcfmxscindr b on a.jzlsh = b.jzlsh
            //                left join ybhisdzdr c on b.yysfxmbm = c.hisxmbh 
            //                where b.jsdjh is not null and b.sysdate between '{0}' and '{1}'"
            //                , dateTimeStar.Value.ToString("yyyy-MM-dd"), dateTimeEnd.Value.AddDays(1).ToString("yyyy-MM-dd"));
            string sql = string.Format(@"select sum(b.je) z3amt2
                , c.sflb 
                from ybmzzydjdr(nolock) a
                join ybcfmxscindr(nolock) b on a.jzlsh = b.jzlsh
                left join ybhisdzdr(nolock) c on b.yysfxmbm = c.hisxmbh 
                where b.jsdjh is not null and left(b.cfrq, 8) between '{0}' and '{1}' and a.cxbz = 1 and b.cxbz = 1"
                , dateTimeStar.Value.ToString("yyyyMMdd"), dateTimeEnd.Value.ToString("yyyyMMdd"));
            //            string sql1 = string.Format(@"select isnull(sum(case when a.cxbz in (1, 2) then a.ylfze else -a.ylfze end), 0) ylfze
            //            , isnull(sum(case when a.cxbz in (1, 2) then a.xjzf else -a.xjzf end), 0) xjzf
            //            , isnull(sum(case when a.cxbz in (1, 2) then a.tcjjzf else -a.tcjjzf end), 0) tcjjzf
            //            , isnull(sum(case when a.cxbz in (1, 2) then a.zhzf else -a.zhzf end), 0) zhzf
            //            , isnull(sum(case when a.cxbz in (1, 2) then a.dejjzf else -a.dejjzf end), 0) dejjzf
            //            , isnull(sum(case when a.cxbz in (1, 2) then a.ecbcje else -a.ecbcje end), 0) ecbcje
            //            , isnull(sum(case when a.cxbz in (1, 2) then a.mzjzfy else -a.mzjzfy end), 0) mzjzfy
            //            , isnull(sum(case when a.cxbz in (1, 2) then a.jrdebxfy else -a.jrdebxfy end), 0) jrdebxfy
            //            , isnull(sum(case when a.cxbz in (1, 2) then a.cxjfy else -a.cxjfy end), 0) cxjfy
            //            , isnull(sum(case when a.cxbz in (1, 2) then a.qfbzfy else -a.qfbzfy end), 0) qfbzfy
            //            , isnull(sum(case when a.cxbz in (1, 2) then a.bnzhzflj else -a.bnzhzflj end), 0) bnzhzflj
            //            , isnull(sum(case when a.cxbz in (1, 2) then a.jrtcfy else -a.jrtcfy end), 0) jrtcfy
            //            , isnull(sum(case when a.cxbz in (1, 2) then a.zffy else -a.zffy end), 0) zffy
            //            , isnull(sum(case when a.cxbz in (1, 2) then a.ylzlfy else -a.ylzlfy end), 0) ylzlfy
            //            , isnull(sum(case when a.cxbz in (1, 2) then a.blzlfy else -a.blzlfy end), 0) blzlfy
            //            , isnull(sum(case when a.cxbz in (1, 2) then a.tcfdzffy else -a.tcfdzffy end), 0) tcfdzffy
            //            , isnull(sum(case when a.cxbz in (1, 2) then a.dwfdfy else -a.dwfdfy end), 0) dwfdfy
            //            , sum(case when a.cxbz in (1, 2) then 1 else -1 end) rc
            //            from ybfyjsdr a
            //            join ybmzzydjdr b on a.jzlsh = b.jzlsh
            //            where a.sysdate between '{0}' and '{1}'", dateTimeStar.Value.ToString("yyyy-MM-dd"), dateTimeEnd.Value.AddDays(1).ToString("yyyy-MM-dd"));
            
            string sql11 = "";
            bbtjlx = "";

            if (rbtnflx.Checked)
            {
                bbtjlx += rbtnflx.Text;
                sql += " and (left(a.yldylb, 1) in ('1', '2', '3') or a.yldylb = '99') and a.yldylb not in ('25', '31', '34')";
                sql11 += " and (left(b.yldylb, 1) in ('1', '2', '3') or b.yldylb = '99') and b.yldylb not in ('25', '31', '34')";
            }
            else if (rbtnlx.Checked)
            {
                bbtjlx += rbtnlx.Text;
                sql += " and a.yldylb in ('25', '31', '34')";
                sql11 += " and b.yldylb in ('25', '31', '34')";
            }
            else
            {
                bbtjlx += rbtnjm.Text;
                sql += " and left(a.yldylb, 1) in ('4', '5', '6', '7', '8', '9') and a.yldylb != '99'";
                sql11 += " and left(b.yldylb, 1) in ('4', '5', '6', '7', '8', '9') and b.yldylb != '99'";
            }

            if (rbtnmz.Checked)
            {
                bbtjlx += rbtnmz.Text;
                sql += " and a.jzbz = 'm'";
                sql11 += " and b.jzbz = 'm'";
            }
            else if (rbtnzy.Checked)
            {
                bbtjlx += rbtnzy.Text;
                sql += " and a.jzbz = 'z'";
                sql11 += " and b.jzbz = 'z'";
            }
            else
            {
                bbtjlx += rbtnqy.Text;
            }

            if (cmbyllb.Text != "")
            {
                bbtjlx += cmbyllb.Text;
                sql += string.Format(" and a.yllb = '{0}'", cmbyllb.SelectedValue);
                sql11 += string.Format(" and b.yllb = '{0}'", cmbyllb.SelectedValue);
            }

            if (cmbrylx.Text != "")
            {
                bbtjlx += cmbrylx.Text;
                sql += string.Format(" and a.yldylb = '{0}'", cmbrylx.SelectedValue);
                sql11 += string.Format(" and b.yldylb = '{0}'", cmbrylx.SelectedValue);
            }

            if (cmb_tcq.Text != "")
            {
                sql += string.Format(" and a.tcqh = '{0}'", cmb_tcq.SelectedValue);
                sql11 += string.Format(" and b.tcqh = '{0}'", cmb_tcq.SelectedValue);
            }

            sql += " group by c.sflb";
            string sql1 = string.Format(@"select isnull(sum(a.ylfze), 0) ylfze
            , isnull(sum(a.xjzf), 0) xjzf
            , isnull(sum(a.tcjjzf), 0) tcjjzf
            , isnull(sum(a.zhzf), 0) zhzf
            , isnull(sum(a.dejjzf), 0) dejjzf
            , isnull(sum(a.xtmzjzfy), 0) xtmzjzfy
            , isnull(sum(a.mzjzfy), 0) mzjzfy
            , isnull(sum(a.qfbzfy), 0) qfbzfy
            , isnull(sum(a.zffy), 0) zffy
            , isnull(sum(a.ylzlfy), 0) ylzlfy
            , isnull(sum(a.blzlfy), 0) blzlfy
            , isnull(sum(a.ecbcje), 0) ecbcje
            , isnull(sum(a.jrdebxfy), 0) jrdebxfy
            , isnull(sum(a.cxjfy), 0) cxjfy
            , isnull(sum(a.jrtcfy), 0) jrtcfy
            , isnull(sum(a.tcfdzffy), 0) tcfdzffy
            , isnull(sum(a.qybcylbxjjzf), 0) qybcylbxjjzf
            , isnull(sum(a.defdzffy), 0) defdzffy
            , isnull(sum(a.yyfdfy), 0) yyfdfy
            , isnull(sum(case a.jzbz when 'z' then datediff(day, cast(left(c.z1date, 10) as datetime), cast(left(a.jsrq, 8) as datetime)) else 0 end), 0) zyts
            , count(1) rc
            from (select a.jzlsh, a.cxbz, a.ylfze
            , a.xjzf
            , a.tcjjzf
            , a.zhzf
            , a.dejjzf
            , case a.yllb when '35' then a.mzjzfy else 0 end xtmzjzfy
            , case a.yllb when '35' then 0 else a.mzjzfy end mzjzfy
            , a.qfbzfy
            , a.zffy
            , a.ylzlfy
            , a.blzlfy
            , a.jsrq
            , a.ecbcje
            , a.jrdebxfy
            , a.cxjfy
            , a.jrtcfy
            , a.tcfdzffy
            , a.qybcylbxjjzf
            , a.defdzffy
            , b.jzbz
            , b.ghdjsj
            , a.yyfdfy
            from ybfyjsdr(nolock) a
            join ybmzzydjdr(nolock) b on a.jzlsh = b.jzlsh
            where left(a.jsrq, 8) between '{0}' and '{1}' and a.cxbz = 1 and b.cxbz = 1
            {2}
            union all
            select a.zybah jzlsh, case a.stbz when '0' then 1 when '1' then 0 else 2 end cxbz
            , a.fyzje ylfze
            , a.grzf xjzf
            , a.jjzfe tcjjzf
            , a.zhzf zhzf
            , a.dbtczfje dejjzf
            , a.mzjz xtmzjzfy
            , 0 mzjzfy
            , a.qfbzfy qfbzfy
            , a.zffy zffy
            , a.ylzlfy ylzlfy
            , a.blzlfy blzlfy
            , convert(char, a.ybxtrq, 112) jsrq
            , a.ecbcfy ecbcje
            , a.jrdebxfy
            , a.cxjfy
            , a.jrtcfy
            , a.tcfdzffy
            , a.qybcjjzf qybcylbxjjzf
            , a.defdzffy
            , b.jzbz
            , b.ghdjsj
            , a.yyfd
            from [192.168.42.222].iinhis.dbo.ybjzxx a --join [192.168.42.222].iinhis.dbo.ybzyxxb b on (a.zybah = b.zybah) 
            join ybmzzydjdr(nolock) b on a.zybah = b.jzlsh
            where left(a.zybah, 2) != '99' and left(convert(char, a.ybxtrq, 112), 8) between '{0}' and '{1}' and case a.stbz when '0' then 1 when '1' then 0 else 2 end = 1 and b.cxbz = 1
            {2}) a left join zy01h c on c.z1zyno = a.jzlsh", dateTimeStar.Value.ToString("yyyyMMdd"), dateTimeEnd.Value.ToString("yyyyMMdd"), sql11);
            Common.WriteYBLog(sql + ";" + sql1);
            ds = CliUtils.ExecuteSql("sybdj", "cmd", sql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            DataSet ds1 = CliUtils.ExecuteSql("szy02", "zy01h", sql1, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            ds.Tables.Add(ds1.Tables[0].Copy());
            dgvbf.DataSource = ds1.Tables[0];

            if (ds1.Tables[0] != null && ds1.Tables[0].Rows.Count > 0)
            {
                btnprint.Enabled = true;
            }
            else
            {
                btnprint.Enabled = false;
            }
        }

        private void btn_Close_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void rbtnrylx_Click(object sender, EventArgs e)
        {
            string sql = "select bzmem1, bzname from bztbd(nolock) where bzcodn = 'DL'";

            if (rbtnflx.Checked)
            {
                sql += " and (left(bzmem1, 1) in ('1', '2', '3') or bzmem1 = '99') and bzmem1 not in ('25', '31', '34')";
            }
            else if (this.rbtnlx.Checked)
            {
                sql += " and bzmem1 in ('25', '31', '34')";
            }
            else
            {
                sql += " and left(bzmem1, 1) in ('4', '5', '6', '7', '8', '9') and bzmem1 != '99'";
            }

            DataTable dt = CliUtils.ExecuteSql("sybdj", "cmd", sql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject).Tables[0];
            DataRow dr = dt.NewRow();
            dr["bzname"] = "";
            dr["bzmem1"] = "";
            dt.Rows.InsertAt(dr, 0);
            cmbrylx.DataSource = dt;
            cmbrylx.DisplayMember = "bzname";
            cmbrylx.ValueMember = "bzmem1";
        }
    }
}