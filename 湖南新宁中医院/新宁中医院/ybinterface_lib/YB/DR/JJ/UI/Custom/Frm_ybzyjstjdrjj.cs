using Srvtools;
using System;
using System.Data;

namespace ybinterface_lib
{
    public partial class Frm_ybzyjstjdrjj : InfoForm
    {
        public Frm_ybzyjstjdrjj()
        {
            InitializeComponent();
            dgv_ybzyjstj.AutoGenerateColumns = false;
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void btncx_Click(object sender, EventArgs e)
        {
//            string sql = string.Format(@"select CONVERT(varchar(10), a.sysdate, 23) rq
//                , sum(case when a.cxbz in (1, 2) then 1 else -1 end) rc
//                from ybfyjsdr a join zy01h b on a.jzlsh = b.z1zyno
//                where a.sysdate between '{0}' and '{1}'"
//                , dt_start.Value.ToString("yyyy-MM-dd"), dt_end.Value.AddDays(1).ToString("yyyy-MM-dd"));
            string sql = string.Format(@"select CONVERT(varchar(10), a.sysdate, 23) rq
                , count(1) rc
                from ybfyjsdr(nolock) a join zy01h(nolock) b on a.jzlsh = b.z1zyno
                where a.sysdate between '{0}' and '{1}' and a.cxbz = 1"
                , dt_start.Value.ToString("yyyy-MM-dd"), dt_end.Value.AddDays(1).ToString("yyyy-MM-dd"));
            object area = null;

            if (rbtn_ny.Checked)
            {
                area = rbtn_ny.Tag;
            }
            else if (rbtn_by.Checked)
            {
                area = rbtn_by.Tag;
            }
            else if (rbtn_pp.Checked)
            {
                area = rbtn_pp.Tag;
            }

            if (area != null)
            {
                sql += string.Format(" and left(b.z1area, 1) = '{0}'", area);
            }

            sql += " group by CONVERT(varchar(10), a.sysdate, 23)";
            DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", sql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            DataTable dt = ds.Tables[0];
            dgv_ybzyjstj.DataSource = dt;
        }

        private void btn_Close_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btn_Print_Click(object sender, EventArgs e)
        {

        }

        private void rbtn_ny_Click(object sender, EventArgs e)
        {
            dgv_ybzyjstj.DataSource = null;
        }

        private void rbtn_pp_Click(object sender, EventArgs e)
        {
            dgv_ybzyjstj.DataSource = null;
        }

        private void rbtn_by_Click(object sender, EventArgs e)
        {
            dgv_ybzyjstj.DataSource = null;
        }

        private void rbtn_qy_Click(object sender, EventArgs e)
        {
            dgv_ybzyjstj.DataSource = null;
        }

        private void dt_end_ValueChanged(object sender, EventArgs e)
        {
            dgv_ybzyjstj.DataSource = null;
        }

        private void dt_start_ValueChanged(object sender, EventArgs e)
        {
            dgv_ybzyjstj.DataSource = null;
        }
    }
}

