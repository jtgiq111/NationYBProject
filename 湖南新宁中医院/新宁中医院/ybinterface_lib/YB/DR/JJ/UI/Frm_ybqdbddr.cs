using FastReport;
using Srvtools;
using System;
using System.Data;
using System.IO;
using System.Windows.Forms;

namespace ybinterface_lib
{
    public partial class Frm_ybqdbddr : InfoForm
    {
        GocentPara Para = new GocentPara();

        public Frm_ybqdbddr()
        {
            InitializeComponent();
        }

        private void btnbd_Click(object sender, EventArgs e)
        {
            string zybah = txtzybah.Text.Trim();
            string jsh = txtjsh.Text.Trim();

            if (string.IsNullOrWhiteSpace(zybah) || string.IsNullOrWhiteSpace(jsh) || lbl_jsh.Text != jsh)
            {
                MessageBox.Show("请输入住院号/发票号和结算号并回车", "提示");
                return;
            }

            else if (string.IsNullOrWhiteSpace(lbljsrq.Text))
            {
                MessageBox.Show("结算日期为空", "提示");
                return;
            }

            object[] obj = yb_interface.ybs_interface("4403",new object[]{lbl_zyh.Text, lbl_jsh.Text, lbljsrq.Text, "(重打)"});
        }

        private void txtfph_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (string.IsNullOrWhiteSpace(txtzybah.Text))
                    {
                        MessageBox.Show("请输入住院号/发票号并回车", "提示");
                        return;
                    }
                    else if (string.IsNullOrWhiteSpace(txtjsh.Text))
                    {
                        MessageBox.Show("请输入结算号并回车", "提示");
                        return;
                    }

                    string zybah = txtzybah.Text.Trim();
                    string jsh = txtjsh.Text.Trim();
                    string sql = string.Format("select jzlsh, djhin, sysdate, xm, ylfze, zbxje, tcjjzf, zhzf from ybfyjsdr(nolock) a where a.jzlsh = '{0}' and a.cxbz = 1 and a.djhin = '{1}'", zybah, jsh);
                    DataTable dt = CliUtils.ExecuteSql("sybdj", "cmd", sql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject).Tables[0];

                    if (dt != null && dt.Rows.Count > 0)
                    {
                        DataRow dr = dt.Rows[0];
                        lbl_zyh.Text = dr["jzlsh"].ToString();
                        lbl_jsh.Text = dr["djhin"].ToString();
                        lbljsrq.Text = Convert.ToDateTime(dr["sysdate"]).ToString("yyyy-MM-dd");
                        lblxm.Text = dr["xm"].ToString();
                        lblylfze.Text = dr["ylfze"].ToString();
                        lblzbxje.Text = dr["zbxje"].ToString();
                        lbltczf.Text = dr["tcjjzf"].ToString();
                        lblzhzf.Text = dr["zhzf"].ToString();
                    }
                    else
                    {
                        MessageBox.Show("该住院号/发票号不存在或没有医保结算", "提示");
                        return;
                    }
                }
            }
            catch (Exception error)
            {
                MessageBox.Show(error.ToString(), "提示");
            }
        }

        private void btn_Close_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void txtzybah_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (string.IsNullOrWhiteSpace(txtzybah.Text))
                {
                    MessageBox.Show("请输入住院号/发票号并回车", "提示");
                    return;
                }

                string zyh = txtzybah.Text.Trim();
                string sql = string.Format("select top 1 jzlsh, djhin, sysdate, xm, ylfze, zbxje, tcjjzf, zhzf from ybfyjsdr(nolock) a where (a.jzlsh = '{0}' or a.djhin = '{0}' or a.djhin = (select top 1 m3shno from mz03d(nolock) where m3invo = '{0}' and left(m3endv, 1) not in ('1', '4'))) and a.cxbz = 1 order by a.sysdate desc", zyh);
                DataTable dt = CliUtils.ExecuteSql("sybdj", "cmd", sql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject).Tables[0];

                if (dt != null && dt.Rows.Count > 0)
                {
                    DataRow dr = dt.Rows[0];
                    lbl_zyh.Text = dr["jzlsh"].ToString();
                    lbl_jsh.Text = dr["djhin"].ToString();
                    txtjsh.Text = dr["djhin"].ToString();
                    lbljsrq.Text = Convert.ToDateTime(dr["sysdate"]).ToString("yyyy-MM-dd");
                    lblxm.Text = dr["xm"].ToString();
                    lblylfze.Text = dr["ylfze"].ToString();
                    lblzbxje.Text = dr["zbxje"].ToString();
                    lbltczf.Text = dr["tcjjzf"].ToString();
                    lblzhzf.Text = dr["zhzf"].ToString();
                }
                else
                {
                    MessageBox.Show("该住院号/发票号不存在或没有医保结算", "提示");
                    return;
                }
            }
        }

        private void Frm_ybqdbddr_Load(object sender, EventArgs e)
        {
            txtzybah.Focus();
        }

        private void btn_bdqb_Click(object sender, EventArgs e)
        {
            string zybah = txtzybah.Text.Trim();
            string jsh = txtjsh.Text.Trim();

            if (string.IsNullOrWhiteSpace(zybah) || string.IsNullOrWhiteSpace(jsh) || lbl_jsh.Text != jsh)
            {
                MessageBox.Show("请输入住院号/发票号和结算号并回车", "提示");
                return;
            }

            else if (string.IsNullOrWhiteSpace(lbljsrq.Text))
            {
                MessageBox.Show("结算日期为空", "提示");
                return;
            }

            object[] obj = yb_interface.ybs_interface("5003",new object[]{lbl_zyh.Text, lbl_jsh.Text, lbljsrq.Text, "(重打)"});
        }
    }
}
