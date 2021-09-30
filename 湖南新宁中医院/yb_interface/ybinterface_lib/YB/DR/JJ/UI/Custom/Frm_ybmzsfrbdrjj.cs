using FastReport;
using Srvtools;
using System;
using System.Data;
using System.IO;
using System.Windows.Forms;

namespace ybinterface_lib
{
    public partial class Frm_ybmzsfrbdrjj : InfoForm
    {
        DataSet ds;
        DataSet dshj;
        GocentPara Para = new GocentPara();
        string hosarea = "";

        public Frm_ybmzsfrbdrjj()
        {
            InitializeComponent();
            dgv_ybmzsfrb.AutoGenerateColumns = false;
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void btncx_Click(object sender, EventArgs e)
        {
            object area = null;

            if (rbtn_ny.Checked)
            {
                hosarea = rbtn_ny.Text;
                area = rbtn_ny.Tag;
            }
            else if (rbtn_by.Checked)
            {
                hosarea = rbtn_by.Text;
                area = rbtn_by.Tag;
            }
            else if (rbtn_pp.Checked)
            {
                hosarea = rbtn_pp.Text;
                area = rbtn_pp.Tag;
            }
            else
            {
                hosarea = rbtn_qy.Text;
            }

//            string sql = string.Format(@"select a.jbr
//                , sum(case when a.cxbz in (1, 2) then a.ylfze else -a.ylfze end) fyze
//                , sum(case when a.cxbz in (1, 2) then a.zhzf else -a.zhzf end) zhbf
//                , sum(case when a.cxbz in (1, 2) then a.zbxje else -a.zbxje end) jbtczf
//                , sum(case when a.cxbz in (1, 2) then a.dejjzf else -a.dejjzf end) dbtczf
//                , sum(case when a.cxbz in (1, 2) then a.mzjzfy else -a.mzjzfy end) shjzzf
//                , sum(case when a.cxbz in (1, 2) then a.xjzf else -a.xjzf end) sjxj
//                from ybfyjsdr a join mz01h b on a.jzlsh = b.m1ghno 
//                where a.sysdate between '{0}' and '{1}'"
//                , dt_start.Value.ToString("yyyy-MM-dd"), dt_end.Value.AddDays(1).ToString("yyyy-MM-dd"));
//            string sqlhj = string.Format(@"select sum(case when a.cxbz in (1, 2) then a.ylfze else -a.ylfze end) hjfyze
//                , sum(case when a.cxbz in (1, 2) then a.zhzf else -a.zhzf end) hjzhbf
//                , sum(case when a.cxbz in (1, 2) then a.zbxje else -a.zbxje end) hjjbtczf
//                , sum(case when a.cxbz in (1, 2) then a.dejjzf else -a.dejjzf end) hjdbtczf
//                , sum(case when a.cxbz in (1, 2) then a.mzjzfy else -a.mzjzfy end) hjshjzzf
//                , sum(case when a.cxbz in (1, 2) then a.xjzf else -a.xjzf end) hjsjxj 
//                from ybfyjsdr a join mz01h b on a.jzlsh = b.m1ghno 
//                where a.sysdate between '{0}' and '{1}'"
//                , dt_start.Value.ToString("yyyy-MM-dd"), dt_end.Value.AddDays(1).ToString("yyyy-MM-dd"));
            string sql = string.Format(@"select a.jbr
                , sum(a.ylfze) fyze
                , sum(a.zhzf) zhbf
                , sum(a.zbxje) jbtczf
                , sum(a.dejjzf) dbtczf
                , sum(a.mzjzfy) shjzzf
                , sum(a.xjzf) sjxj
                from ybfyjsdr(nolock) a join mz01h(nolock) b on a.jzlsh = b.m1ghno 
                where a.sysdate between '{0}' and '{1}' and a.cxbz = 1"
                , dt_start.Value.ToString("yyyy-MM-dd"), dt_end.Value.AddDays(1).ToString("yyyy-MM-dd"));
            string sqlhj = string.Format(@"select sum(a.ylfze) hjfyze
                , sum(a.zhzf) hjzhbf
                , sum(a.zbxje) hjjbtczf
                , sum(a.dejjzf) hjdbtczf
                , sum(a.mzjzfy) hjshjzzf
                , sum(a.xjzf) hjsjxj 
                from ybfyjsdr(nolock) a join mz01h(nolock) b on a.jzlsh = b.m1ghno 
                where a.sysdate between '{0}' and '{1}' and a.cxbz = 1"
                , dt_start.Value.ToString("yyyy-MM-dd"), dt_end.Value.AddDays(1).ToString("yyyy-MM-dd"));
            if (area != null)
            {
                sql += string.Format(" and left(b.m1area, 1) = '{0}'", area);
                sqlhj += string.Format(" and left(b.m1area, 1) = '{0}'", area);
            }

            sql += " group by a.jbr";
            ds = CliUtils.ExecuteSql("sybdj", "cmd", sql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            DataTable dt = ds.Tables[0];
            dshj = CliUtils.ExecuteSql("szy02", "zy01h", sqlhj, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);

            if (dt != null && dt.Rows.Count > 0)
            {
                dgv_ybmzsfrb.DataSource = dt;
                ds.Tables.Add(dshj.Tables[0].Copy());
                btn_Print.Enabled = true;
            }
            else
            {
                dgv_ybmzsfrb.DataSource = null;
                btn_Print.Enabled = false;
            }
        }

        private void btn_Close_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btn_Print_Click(object sender, EventArgs e)
        {
            string spath = Para.sys_parameter("BZ", "BZ0007");
            spath = string.IsNullOrEmpty(spath) ? @"C:\Program Files (x86)\Infolight\EEP2012\EEPNetClient\FastReport" : spath;
            string cfile = Application.StartupPath + @"\FastReport\YB\九江市中医院门诊收费室医社保收费日报表.frx";
            string sfile = spath + @"\YB\九江市中医院门诊收费室医社保收费日报表.frx";
            CliUtils.DownLoad(sfile, cfile);

            if (!File.Exists(cfile))
            {
                MessageBox.Show("医保提示：" + cfile + "不存在!", "提示");
                return;
            }

            Report report = new Report();
            report.PrintSettings.ShowDialog = false;
            report.Load(cfile);
            report.RegisterData(ds);
            report.SetParameterValue("hosarea", hosarea);
            report.SetParameterValue("ksrq", dt_start.Value.ToString("yyyy-MM-dd"));
            report.SetParameterValue("jsrq", dt_end.Value.ToString("yyyy-MM-dd"));

            if (CliUtils.fPrintPreview == "True")
            {
                report.Show();
            }
            else
            {
                report.Print();
            }

            cfile = Application.StartupPath + @"\FastReport\YB\九江市中医院门诊收费室医社保收费汇总表.frx";
            sfile = spath + @"\YB\九江市中医院门诊收费室医社保收费汇总表.frx";
            CliUtils.DownLoad(sfile, cfile);
            report = new Report();
            report.PrintSettings.ShowDialog = false;
            report.Load(cfile);
            report.RegisterData(ds);
            report.SetParameterValue("hosarea", hosarea);
            report.SetParameterValue("ksrq", dt_start.Value.ToString("yyyy-MM-dd"));
            report.SetParameterValue("jsrq", dt_end.Value.ToString("yyyy-MM-dd"));

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

        private void rbtn_ny_Click(object sender, EventArgs e)
        {
            dgv_ybmzsfrb.DataSource = null;
            btn_Print.Enabled = false;
        }

        private void rbtn_pp_Click(object sender, EventArgs e)
        {
            dgv_ybmzsfrb.DataSource = null;
            btn_Print.Enabled = false;
        }

        private void rbtn_by_Click(object sender, EventArgs e)
        {
            dgv_ybmzsfrb.DataSource = null;
            btn_Print.Enabled = false;
        }

        private void rbtn_qy_Click(object sender, EventArgs e)
        {
            dgv_ybmzsfrb.DataSource = null;
            btn_Print.Enabled = false;
        }

        private void dt_end_ValueChanged(object sender, EventArgs e)
        {
            dgv_ybmzsfrb.DataSource = null;
            btn_Print.Enabled = false;
        }

        private void dt_start_ValueChanged(object sender, EventArgs e)
        {
            dgv_ybmzsfrb.DataSource = null;
            btn_Print.Enabled = false;
        }
    }
}

