using Srvtools;
using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;

namespace ybinterface_lib
{
    public partial class Frm_ybcxdr : InfoForm
    {
        List<string> li_jzlshs;

        public Frm_ybcxdr()
        {
            InitializeComponent();
        }

        private void btn_ghcxmz_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txt_ghlshcxmz.Text))
            {
                MessageBox.Show("请输入挂号流水号并回车", "提示");
                txt_ghlshcxmz.Focus();
                return;
            }

            object[] obj = { txt_ghlshcxmz.Text.Trim() };

            if (!chk_dk.Checked)
            { 
            obj = new object[]{ txt_ghlshcxmz.Text.Trim(), 1 };
            }

            obj = yb_interface.ybs_interface("3101", obj);

            if (obj[1].ToString() == "1")
            {
                MessageBox.Show("撤销医保挂号成功", "提示");
            }
            else
            {
                MessageBox.Show(obj[2].ToString(), "提示");
            }
        }

        private void btn_jscxmz_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txt_ghlshjscxmz.Text))
            {
                MessageBox.Show("请输入挂号流水号", "提示");
                txt_ghlshjscxmz.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txt_jshcxmz.Text))
            {
                MessageBox.Show("请输入结算号并回车", "提示");
                txt_jshcxmz.Focus();
                return;
            }

            object[] obj = { txt_ghlshjscxmz.Text.Trim(), txt_jshcxmz.Text.Trim() };

            if (!chk_dk.Checked)
            {
                obj = new object[] { txt_ghlshjscxmz.Text.Trim(), txt_jshcxmz.Text.Trim(), 1 };
            }

            obj = yb_interface.ybs_interface("3302", obj);

            if (obj[1].ToString() == "1")
            {
                MessageBox.Show("撤销医保结算成功", "提示");
            }
            else
            {
                MessageBox.Show(obj[2].ToString(), "提示");
            }
        }

        private void btn_rydjcxzy_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txt_zyhrycxzy.Text))
            {
                MessageBox.Show("请输入住院号并回车", "提示");
                txt_zyhrycxzy.Focus();
                return;
            }

            object[] obj = { txt_zyhrycxzy.Text.Trim() };
            obj = yb_interface.ybs_interface("4102", obj);

            if (obj[1].ToString() == "1")
            {
                MessageBox.Show("撤销医保入院登记成功", "提示");
            }
            else
            {
                MessageBox.Show(obj[2].ToString(), "提示");
            }
        }

        private void btn_cfmxsbcxzy_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txt_zyhsbcxzy.Text))
            {
                MessageBox.Show("请输入住院号并回车", "提示");
                txt_zyhsbcxzy.Focus();
                return;
            }

            object[] obj = { txt_zyhsbcxzy.Text.Trim() };
            obj = yb_interface.ybs_interface("4301", obj);

            if (obj[1].ToString() == "1")
            {
                MessageBox.Show("撤销医保处方明细成功", "提示");
            }
            else
            {
                MessageBox.Show(obj[2].ToString(), "提示");
            }
        }

        private void btn_jscxzy_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txt_zyhjscxzy.Text))
            {
                MessageBox.Show("请输入住院号", "提示");
                txt_zyhjscxzy.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txt_jshcxzy.Text))
            {
                MessageBox.Show("请输入结算号并回车", "提示");
                txt_jshcxzy.Focus();
                return;
            }

            object[] obj = { txt_zyhjscxzy.Text.Trim(), txt_jshcxzy.Text.Trim() };

            if (!chk_dk.Checked)
            {
                obj = new object[] { txt_zyhjscxzy.Text.Trim(), txt_jshcxzy.Text.Trim(), 1 };
            }

            obj = yb_interface.ybs_interface("4402", obj);

            if (obj[1].ToString() == "1")
            {
                MessageBox.Show("撤销医保结算成功", "提示");
            }
            else
            {
                MessageBox.Show(obj[2].ToString(), "提示");
            }
        }

        private void btn_ghdjcxmz_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txt_ghlshcxmz.Text))
            {
                MessageBox.Show("请输入挂号流水号并回车", "提示");
                txt_ghlshcxmz.Focus();
                return;
            }

            object[] obj = { txt_ghlshcxmz.Text.Trim() };
            obj = yb_interface.ybs_interface("3101", obj);

            if (obj[1].ToString() == "1")
            {
                MessageBox.Show("撤销医保挂号登记成功", "提示");
            }
            else
            {
                MessageBox.Show(obj[2].ToString(), "提示");
            }
        }

        private void txt_ghlshcxmz_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (string.IsNullOrWhiteSpace(txt_ghlshcxmz.Text))
                    {
                        MessageBox.Show("请输入挂号流水号并回车", "提示");
                        return;
                    }

                    string ghlsh = txt_ghlshcxmz.Text = txt_ghlshcxmz.Text.Trim().PadLeft(9, '0');
                    string sql = string.Format(@"select jzlsh, grbh, xm from ybmzzydjdr a where a.jzlsh = '{0}' and cxbz = 1", ghlsh); //and right(jylsh, 1) = 'g'
                    DataTable dt = CliUtils.ExecuteSql("sybdj", "cmd", sql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject).Tables[0];

                    if (dt != null && dt.Rows.Count > 0)
                    {
                        DataRow dr = dt.Rows[0];
                        lblxm.Text = dr["xm"].ToString();
                        lblgrbh.Text = dr["grbh"].ToString();
                        lbljzlsh.Text = dr["jzlsh"].ToString();
                    }
                    else
                    {
                        MessageBox.Show("医保提示：就诊流水号" + ghlsh + "未办理医保挂号", "提示");
                        return;
                    }
                }
            }
            catch (Exception error)
            {
                MessageBox.Show(error.ToString(), "提示");
            }
        }

        private void txt_jshcxmz_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (string.IsNullOrWhiteSpace(txt_ghlshjscxmz.Text))
                    {
                        MessageBox.Show("请输入挂号流水号并回车", "提示");
                        return;
                    }
                    else if (string.IsNullOrWhiteSpace(txt_jshcxmz.Text))
                    {
                        MessageBox.Show("请输入发票号并回车", "提示");
                        return;
                    }

                    string ghlsh = txt_ghlshjscxmz.Text.Trim();
                    string jsh = txt_jshcxmz.Text.Trim();
                    string sql = string.Format(@"select jzlsh, grbh, xm, djhin, ylfze from ybfyjsdr a where a.jzlsh = '{0}' and a.djhin = '{1}' and a.cxbz = 1", ghlsh, jsh);
                    DataTable dt = CliUtils.ExecuteSql("sybdj", "cmd", sql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject).Tables[0];

                    if (dt != null && dt.Rows.Count > 0)
                    {
                        DataRow dr = dt.Rows[0];
                        lblxm.Text = dr["xm"].ToString();
                        lblgrbh.Text = dr["grbh"].ToString();
                        lbljzlsh.Text = dr["jzlsh"].ToString();
                        lbljsh.Text = dr["djhin"].ToString();
                        lbl_je.Text = dr["ylfze"].ToString();
                    }
                    else
                    {
                        MessageBox.Show("医保提示：就诊流水号" + ghlsh + "或结算号" + jsh + "未办理医保结算", "提示");
                        return;
                    }
                }
            }
            catch (Exception error)
            {
                MessageBox.Show(error.ToString(), "提示");
            }
        }

        private void txt_zyhrycxzy_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (string.IsNullOrWhiteSpace(txt_zyhrycxzy.Text))
                    {
                        MessageBox.Show("请输入住院号并回车", "提示");
                        return;
                    }

                    string zyh = txt_zyhrycxzy.Text = txt_zyhrycxzy.Text.Trim().PadLeft(8, '0');
                    string sql = string.Format(@"select jzlsh, grbh, xm from ybmzzydjdr a where a.jzlsh = '{0}' and cxbz = 1", zyh);
                    DataTable dt = CliUtils.ExecuteSql("sybdj", "cmd", sql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject).Tables[0];

                    if (dt != null && dt.Rows.Count > 0)
                    {
                        DataRow dr = dt.Rows[0];
                        lblxm.Text = dr["xm"].ToString();
                        lblgrbh.Text = dr["grbh"].ToString();
                        lbljzlsh.Text = dr["jzlsh"].ToString();
                    }
                    else
                    {
                        MessageBox.Show("医保提示：住院号" + zyh + "未办理医保入院", "提示");
                        return;
                    }
                }
            }
            catch (Exception error)
            {
                MessageBox.Show(error.ToString(), "提示");
            }
        }

        private void txt_zyhsbcxzy_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (string.IsNullOrWhiteSpace(txt_zyhsbcxzy.Text))
                    {
                        MessageBox.Show("请输入住院号并回车", "提示");
                        return;
                    }

                    string zyh = txt_zyhsbcxzy.Text = txt_zyhsbcxzy.Text.Trim().PadLeft(8, '0');
                    string sql = string.Format(@"select jzlsh, grbh, xm from ybmzzydjdr a where a.jzlsh = '{0}' and cxbz = 1", zyh);
                    DataTable dt = CliUtils.ExecuteSql("sybdj", "cmd", sql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject).Tables[0];

                    if (dt != null && dt.Rows.Count > 0)
                    {
                        DataRow dr = dt.Rows[0];
                        lblxm.Text = dr["xm"].ToString();
                        lblgrbh.Text = dr["grbh"].ToString();
                        lbljzlsh.Text = dr["jzlsh"].ToString();
                    }
                    else
                    {
                        MessageBox.Show("医保提示：住院号" + zyh + "未办理医保入院", "提示");
                        return;
                    }
                }
            }
            catch (Exception error)
            {
                MessageBox.Show(error.ToString(), "提示");
            }
        }

        private void txt_jshcxzy_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (string.IsNullOrWhiteSpace(txt_zyhjscxzy.Text))
                    {
                        MessageBox.Show("请输入住院号并回车", "提示");
                        return;
                    }
                    else if (string.IsNullOrWhiteSpace(txt_jshcxzy.Text))
                    {
                        MessageBox.Show("请输入结算号并回车", "提示");
                        return;
                    }

                    string zyh = txt_zyhjscxzy.Text.Trim();
                    string jsh = txt_jshcxzy.Text.Trim();
                    string sql = string.Format(@"select jzlsh, grbh, xm, djhin, ylfze from ybfyjsdr a where a.jzlsh = '{0}' and a.djhin = '{1}' and a.cxbz = 1", zyh, jsh);
                    DataTable dt = CliUtils.ExecuteSql("sybdj", "cmd", sql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject).Tables[0];

                    if (dt != null && dt.Rows.Count > 0)
                    {
                        DataRow dr = dt.Rows[0];
                        lblxm.Text = dr["xm"].ToString();
                        lblgrbh.Text = dr["grbh"].ToString();
                        lbljzlsh.Text = dr["jzlsh"].ToString();
                        lbljsh.Text = dr["djhin"].ToString();
                        lbl_je.Text = dr["ylfze"].ToString();
                    }
                    else
                    {
                        MessageBox.Show("医保提示：就诊流水号" + zyh + "或结算号" + jsh + "未办理医保结算", "提示");
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

        private void txt_ghlshjscxmz_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                string ghlsh = txt_ghlshjscxmz.Text = txt_ghlshjscxmz.Text.Trim().PadLeft(9, '0');
                string sql = string.Format("select djhin, xm, grbh, jzlsh, ylfze from ybfyjsdr a where a.jzlsh = '{0}' and a.cxbz = 1 order by sysdate desc", ghlsh);
                DataTable dt = CliUtils.ExecuteSql("sybdj", "cmd", sql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject).Tables[0];

                if (dt != null && dt.Rows.Count > 0)
                {
                    DataRow dr = dt.Rows[0];
                    txt_jshcxmz.Text = dr["djhin"].ToString();
                    lblxm.Text = dr["xm"].ToString();
                    lblgrbh.Text = dr["grbh"].ToString();
                    lbljzlsh.Text = dr["jzlsh"].ToString();
                    lbljsh.Text = dr["djhin"].ToString();
                    lbl_je.Text = dr["ylfze"].ToString();
                }
                else
                {
                    MessageBox.Show("医保提示：就诊流水号" + ghlsh + "未办理医保结算", "提示");
                    return;
                }
            }
        }

        private void txt_zyhjscxzy_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (string.IsNullOrWhiteSpace(txt_zyhjscxzy.Text))
                {
                    MessageBox.Show("请输入住院号并回车", "提示");
                    return;
                }

                string zyh = txt_zyhjscxzy.Text = txt_zyhjscxzy.Text.Trim().PadLeft(8, '0');
                string sql = string.Format("select djhin, xm, grbh, jzlsh, ylfze from ybfyjsdr a where a.jzlsh = '{0}' and a.cxbz = 1 order by sysdate desc", zyh);
                DataTable dt = CliUtils.ExecuteSql("sybdj", "cmd", sql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject).Tables[0];

                if (dt != null && dt.Rows.Count > 0)
                {
                    DataRow dr = dt.Rows[0];
                    txt_jshcxzy.Text = dr["djhin"].ToString();
                    lblxm.Text = dr["xm"].ToString();
                    lblgrbh.Text = dr["grbh"].ToString();
                    lbljzlsh.Text = dr["jzlsh"].ToString();
                    lbljsh.Text = dr["djhin"].ToString();
                    lbl_je.Text = dr["ylfze"].ToString();
                }
                else
                {
                    MessageBox.Show("医保提示：就诊流水号" + zyh + "未办理医保结算", "提示");
                    return;
                }
            }
        }

        private void btn_cfmxsbcxmz_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txt_grbhcfcxmz.Text))
            {
                MessageBox.Show("请输入个人编号并回车", "提示");
                txt_grbhcfcxmz.Focus();
                return;
            }

            if (li_jzlshs.Count == 0)
            {
                MessageBox.Show("该个人编号无可撤销处方", "提示");
                return;
            }

            bool iscx = true;

            foreach (string jzlsh in li_jzlshs)
            {
                object[] obj = { jzlsh };
                obj = yb_interface.ybs_interface("2320",obj);

                if (obj[1].ToString() == "1")
                {

                }
                else
                {
                    iscx = false;
                    MessageBox.Show("就诊流水号：" + jzlsh + "冲销处方明细失败" + obj[2].ToString(), "提示");
                }
            }

            if (iscx)
            {
                MessageBox.Show("冲销处方明细成功", "提示");
            }
        }

        private void txt_grbhcfcxmz_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (string.IsNullOrWhiteSpace(txt_grbhcfcxmz.Text))
                    {
                        MessageBox.Show("请输入个人编号并回车", "提示");
                        return;
                    }

                    string grbh = txt_grbhcfcxmz.Text.Trim();
                    string sql = string.Format(@"select distinct a.jzlsh, a.grbh, a.xm from ybcfmxscfhdr a where a.grbh = '{0}' and a.cxbz = 1 and isnull(a.jsdjh, '') = ''", grbh);
                    DataTable dt = CliUtils.ExecuteSql("sybdj", "cmd", sql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject).Tables[0];
                    li_jzlshs = new List<string>();

                    if (dt != null && dt.Rows.Count > 0)
                    {
                        foreach (DataRow dr in dt.Rows)
                        {
                            lblxm.Text = dr["xm"].ToString();
                            lblgrbh.Text = dr["grbh"].ToString();
                            li_jzlshs.Add(dr["jzlsh"].ToString());
                        }
                    }
                    else
                    {
                        MessageBox.Show("医保提示：个人编号" + grbh + "无需要冲销的处方", "提示");
                        return;
                    }
                }
            }
            catch (Exception error)
            {
                MessageBox.Show(error.ToString(), "提示");
            }
        }
    }
}
