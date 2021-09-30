using Srvtools;
using System;
using System.Data;
using System.Windows.Forms;

namespace ybinterface_lib
{
    public partial class Frm_ybtestdr : InfoForm
    {
        public Frm_ybtestdr()
        {
            InitializeComponent();
        }

        private void btn_cshqd_Click(object sender, EventArgs e)
        {
            object[] obj = yb_interface.ybs_interface("9100", null);

            if (obj[1].ToString() == "1")
            {
                MessageBox.Show("签到成功", "提示");
            }
        }

        private void btn_dk_Click(object sender, EventArgs e)
        {
            lbl_dkfh.Text = "";
            lbl_fsxx.Text = "";
            lbl_ydry.Text = "";
            lbl_zhye.Text = "";
            lbl_zyzt.Text = "";
            object[] obj = yb_interface.ybs_interface("2101", null);

            if (obj[1].ToString() == "1")
            {
                MessageBox.Show("读卡成功", "提示");
                lbl_dkfh.Text = obj[2].ToString();
                lbl_zhye.Text = obj[2].ToString().Split('|')[14];
                lbl_zyzt.Text = obj[2].ToString().Split('|')[13] == "0" ? "不在院" : "在院";

                if (obj[2].ToString().Split('|')[10] == "0")
                {
                    lbl_ydry.Text = "否";
                    btn_fsxx.Enabled = true;
                    //groupBox_zycfmxsb.Visible = true;
                    //groupBox_zycfmxsbcx.Visible = true;
                }
                else
                {
                    lbl_ydry.Text = "是";
                    btn_fsxx.Enabled = false;
                    //groupBox_zycfmxsb.Visible = false;
                    //groupBox_zycfmxsbcx.Visible = false;
                }
            }
            else
            {
                MessageBox.Show("读卡失败：" + obj[2].ToString(), "提示");
            }
        }

        private void btn_qt_Click(object sender, EventArgs e)
        {
            object[] obj = yb_interface.ybs_interface("9100", null);

            if (obj[1].ToString() == "1")
            {
                MessageBox.Show("签退成功", "提示");
                MessageBox.Show(obj[2].ToString(), "提示");
            }
        }

        private void btn_gh_Click(object sender, EventArgs e)
        {
            if (!lbl_dkfh.Text.Contains("|"))
            {
                MessageBox.Show("请先读卡", "提示");
                return;
            }

            if (string.IsNullOrWhiteSpace(this.txt_ghlshmz.Text))
            {
                MessageBox.Show("无挂号流水号", "提示");
                txt_ghlshmz.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(this.cmb_yllbmz.SelectedValue.ToString()))
            {
                MessageBox.Show("无医疗类别", "提示");
                return;
            }

            if (string.IsNullOrWhiteSpace(this.txt_jssjghmz.Text))
            {
                MessageBox.Show("无结算时间", "提示");
                txt_jssjghmz.Focus();
                return;
            }

            string bzbm = "";
            string bzmc = "";

            if (cmb_mxbmz.Text.Contains("|"))
            {
                bzbm = cmb_mxbmz.Text.Split('|')[0];
                bzmc = cmb_mxbmz.Text.Split('|')[1];
            }
            else
            {
                cmb_yllbmz.SelectedIndex = 0;
            }

            object[] obj = { txt_ghlshmz.Text.Trim(), cmb_yllbmz.SelectedValue, txt_jssjghmz.Text.Trim(), bzbm, bzmc, lbl_dkfh.Text };
            obj = yb_interface.ybs_interface("3210", obj);

            if (obj[1].ToString() == "1")
            {
                MessageBox.Show("挂号成功", "提示");
            }
            else
            {
                MessageBox.Show(obj[2].ToString(), "提示");
            }
        }

        private void frmybtest_Load(object sender, EventArgs e)
        {
            txt_jssjghmz.Text = DateTime.Now.ToString();
            txt_jssjjsmz.Text = DateTime.Now.ToString();
            txt_jssjjszy.Text = DateTime.Now.ToString();
            string strSql = "select * from bztbd where bzcodn = 'YL' and bzmem1 in ('11', '12', '13', '35')";
            DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);

            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                cmb_yllbmz.DataSource = ds.Tables[0];
                cmb_yllbmz.DisplayMember = "bzname";
                cmb_yllbmz.ValueMember = "bzmem1";
                cmb_yllbmzjs.DataSource = ds.Tables[0].Copy();
                cmb_yllbmzjs.DisplayMember = "bzname";
                cmb_yllbmzjs.ValueMember = "bzmem1";
            }
        }

        private void btn_rydjzy_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(lbl_dkfh.Text) || !lbl_dkfh.Text.Contains("|"))
            {
                MessageBox.Show("请先读卡", "提示");
                return;
            }

            if (string.IsNullOrWhiteSpace(this.txt_yllbdmzy.Text))
            {
                MessageBox.Show("无医疗类别代码", "提示");
                txt_yllbdmzy.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(this.txt_yllbmczy.Text))
            {
                MessageBox.Show("无医疗类别名称", "提示");
                txt_yllbmczy.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(this.txt_zyhzy.Text))
            {
                MessageBox.Show("无住院号", "提示");
                txt_zyhzy.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(this.txt_bzbmzy.Text))
            {
                MessageBox.Show("无病种编码", "提示");
                txt_bzbmzy.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(this.txt_bzmczy.Text))
            {
                MessageBox.Show("无病种名称", "提示");
                txt_bzmczy.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(this.txt_lydmzy.Text))
            {
                MessageBox.Show("无来源代码", "提示");
                txt_lydmzy.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(this.txt_lymczy.Text))
            {
                MessageBox.Show("无来源名称", "提示");
                txt_lymczy.Focus();
                return;
            }

            object[] obj = { txt_zyhzy.Text.Trim(), txt_yllbdmzy.Text.Trim(), txt_bzbmzy.Text.Trim(), txt_bzmczy.Text.Trim(), lbl_dkfh.Text, txt_lydmzy.Text.Trim(), txt_lymczy.Text.Trim(), txt_yllbmczy.Text.Trim() };
            obj = yb_interface.ybs_interface("4100", obj);

            if (obj[1].ToString() == "1")
            {
                MessageBox.Show("入院登记成功", "提示");
            }
            else
            {
                MessageBox.Show(obj[2].ToString(), "提示");
            }
        }

        private void btn_cfmxsbcxmz_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(this.txt_ghlshsbcxmz.Text))
            {
                MessageBox.Show("无挂号流水号", "提示");
                txt_ghlshsbcxmz.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(this.txt_jylshmz.Text))
            {
                MessageBox.Show("无交易流水号", "提示");
                txt_jylshmz.Focus();
                return;
            }

            object[] obj = { txt_ghlshsbcxmz.Text.Trim(), txt_jylshmz.Text.Trim() };
            obj = yb_interface.ybs_interface("3201", obj);

            if (obj[1].ToString() == "1")
            {
                MessageBox.Show("撤销处方明细成功", "提示");
            }
            else
            {
                MessageBox.Show(obj[2].ToString(), "提示");
            }
        }

        private void btn_ghcxmz_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(this.txt_ghlshcxmz.Text))
            {
                MessageBox.Show("无挂号流水号", "提示");
                txt_ghlshcxmz.Focus();
                return;
            }

            object[] obj = { txt_ghlshcxmz.Text.Trim() };
            obj = yb_interface.ybs_interface("3101", obj);

            if (obj[1].ToString() == "1")
            {
                MessageBox.Show("撤销挂号成功", "提示");
            }
            else
            {
                MessageBox.Show(obj[2].ToString(), "提示");
            }
        }

        private void btn_jscxmz_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(this.txt_ghlshjscxmz.Text))
            {
                MessageBox.Show("无挂号流水号", "提示");
                txt_ghlshjscxmz.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(this.txt_jshjscxmz.Text))
            {
                MessageBox.Show("无结算号", "提示");
                txt_jshjscxmz.Focus();
                return;
            }

            object[] obj = { txt_ghlshjscxmz.Text.Trim(), txt_jshjscxmz.Text.Trim() };
            obj = yb_interface.ybs_interface("3302", obj);

            if (obj[1].ToString() == "1")
            {
                MessageBox.Show("撤销结算成功", "提示");
            }
            else
            {
                MessageBox.Show(obj[2].ToString(), "提示");
            }
        }

        private void btn_cfmxsbmz_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(this.txt_ghlshsbmz.Text))
            {
                MessageBox.Show("无挂号流水号", "提示");
                txt_ghlshsbmz.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(this.txt_cfhsmz.Text))
            {
                MessageBox.Show("无处方号", "提示");
                txt_cfhsmz.Focus();
                return;
            }

            object[] obj = { txt_ghlshsbmz.Text.Trim(), txt_cfhsmz.Text.Trim() };
            obj = yb_interface.ybs_interface("3200", obj);

            if (obj[1].ToString() == "1")
            {
                MessageBox.Show("处方明细上报成功", "提示");
            }
            else
            {
                MessageBox.Show(obj[2].ToString(), "提示");
            }
        }

        private void btn_yjsmz_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(this.txt_ghlshjsmz.Text))
            {
                MessageBox.Show("无挂号流水号", "提示");
                txt_ghlshjsmz.Focus();
                return;
            }
            else if (string.IsNullOrWhiteSpace(this.txt_zhsybzmz.Text))
            {
                MessageBox.Show("无账户使用标志", "提示");
                txt_zhsybzmz.Focus();
                return;
            }
            else if (string.IsNullOrWhiteSpace(this.txt_jssjjsmz.Text))
            {
                MessageBox.Show("无结算时间", "提示");
                txt_jssjjsmz.Focus();
                return;
            }
            else if (string.IsNullOrWhiteSpace(this.cmb_yllbmzjs.SelectedValue.ToString()))
            {
                MessageBox.Show("无医疗类别", "提示");
                return;
            }
            else if (string.IsNullOrWhiteSpace(this.txt_cfhsmzjs.Text.Trim()))
            {
                MessageBox.Show("无处方号集合", "提示");
                return;
            }

            string bzbm = "";
            string bzmc = "";

            if (cmb_mxbmzjs.Text.Contains("|"))
            {
                bzbm = cmb_mxbmzjs.Text.Split('|')[0];
                bzmc = cmb_mxbmzjs.Text.Split('|')[1];
            }
            else
            {
                cmb_yllbmzjs.SelectedIndex = 0;
            }

            object[] obj = { txt_ghlshjsmz.Text.Trim(), txt_zhsybzmz.Text.Trim(), txt_jssjjsmz.Text.Trim(), bzbm, bzmc, txt_cfhsmzjs.Text.Trim(), cmb_yllbmzjs.SelectedValue };
            obj = yb_interface.ybs_interface("3300", obj);

            if (obj[1].ToString() == "1")
            {
                MessageBox.Show("预结算成功", "提示");
            }
            else
            {
                MessageBox.Show(obj[2].ToString(), "提示");
            }
        }

        private void btn_jsmz_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(this.txt_ghlshjsmz.Text))
            {
                MessageBox.Show("无挂号流水号", "提示");
                txt_ghlshjsmz.Focus();
                return;
            }
            else if (string.IsNullOrWhiteSpace(this.txt_jshjsmz.Text))
            {
                MessageBox.Show("无结算号", "提示");
                txt_jshjsmz.Focus();
                return;
            }
            else if (string.IsNullOrWhiteSpace(this.txt_zhsybzmz.Text))
            {
                MessageBox.Show("无账户使用标志", "提示");
                txt_zhsybzmz.Focus();
                return;
            }
            else if (string.IsNullOrWhiteSpace(this.txt_jssjjsmz.Text))
            {
                MessageBox.Show("无结算时间", "提示");
                txt_jssjjsmz.Focus();
                return;
            }
            else if (string.IsNullOrWhiteSpace(this.cmb_yllbmzjs.SelectedValue.ToString()))
            {
                MessageBox.Show("无医疗类别", "提示");
                return;
            }
            else if (string.IsNullOrWhiteSpace(this.txt_cfhsmzjs.Text))
            {
                MessageBox.Show("无处方号集合", "提示");
                return;
            }

            string bzbm = "";
            string bzmc = "";

            if (cmb_mxbmzjs.Text.Contains("|"))
            {
                bzbm = cmb_mxbmzjs.Text.Split('|')[0];
                bzmc = cmb_mxbmzjs.Text.Split('|')[1];
            }

            object[] obj = { txt_ghlshjsmz.Text.Trim(), txt_jshjsmz.Text.Trim(), txt_zhsybzmz.Text.Trim(), txt_jssjjsmz.Text.Trim(), bzbm, bzmc, txt_cfhsmzjs.Text.Trim(), cmb_yllbmzjs.SelectedValue };
            obj = yb_interface.ybs_interface("3301", obj);

            if (obj[1].ToString() == "1")
            {
                MessageBox.Show("结算成功", "提示");
            }
            else
            {
                MessageBox.Show(obj[2].ToString(), "提示");
            }
        }

        private void btn_rydjcxzy_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(this.txt_zyhrycxzy.Text))
            {
                MessageBox.Show("无住院号", "提示");
                txt_zyhrycxzy.Focus();
                return;
            }

            object[] obj = { txt_zyhrycxzy.Text.Trim() };
            obj = yb_interface.ybs_interface("4102", obj);

            if (obj[1].ToString() == "1")
            {
                MessageBox.Show("撤销入院登记成功", "提示");
            }
            else
            {
                MessageBox.Show(obj[2].ToString(), "提示");
            }
        }

        private void btn_cfmxsbzy_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(this.txt_zyhsbzy.Text))
            {
                MessageBox.Show("无住院号", "提示");
                txt_zyhsbzy.Focus();
                return;
            }

            object[] obj = { txt_zyhsbzy.Text.Trim() };
            obj = yb_interface.ybs_interface("4300", obj);

            if (obj[1].ToString() == "1")
            {
                MessageBox.Show("入院处方明细上报成功", "提示");
            }
            else
            {
                MessageBox.Show(obj[2].ToString(), "提示");
            }
        }

        private void btn_cfmxsbcxzy_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(this.txt_zyhsbcxzy.Text))
            {
                MessageBox.Show("无住院号", "提示");
                txt_zyhsbcxzy.Focus();
                return;
            }

            object[] obj = { txt_zyhsbcxzy.Text.Trim(), txt_jylshzy.Text.Trim() };
            obj = yb_interface.ybs_interface("4301", obj);

            if (obj[1].ToString() == "1")
            {
                MessageBox.Show("撤销处方明细成功", "提示");
            }
            else
            {
                MessageBox.Show(obj[2].ToString(), "提示");
            }
        }

        private void btn_yjszy_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(this.txt_zyhjszy.Text))
            {
                MessageBox.Show("无住院号", "提示");
                txt_zyhjszy.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(this.txt_cyyydmzy.Text))
            {
                MessageBox.Show("无出院原因代码", "提示");
                txt_cyyydmzy.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(this.txt_zhsybzzy.Text))
            {
                MessageBox.Show("无账户使用标志", "提示");
                txt_zhsybzzy.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(this.txt_ztjsbzzy.Text))
            {
                MessageBox.Show("无中途结算标志", "提示");
                txt_ztjsbzzy.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(this.txt_jssjjszy.Text))
            {
                MessageBox.Show("无结算时间", "提示");
                txt_jssjjszy.Focus();
                return;
            }

            object[] obj = { txt_zyhjszy.Text.Trim(), txt_cyyydmzy.Text.Trim(), txt_zhsybzzy.Text.Trim(), txt_ztjsbzzy.Text.Trim(), txt_jssjjszy.Text.Trim() };
            obj = yb_interface.ybs_interface("4400", obj);

            if (obj[1].ToString() == "1")
            {
                MessageBox.Show("预结算成功", "提示");
            }
            else
            {
                MessageBox.Show(obj[2].ToString(), "提示");
            }
        }

        private void btn_jszy_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(this.txt_zyhjszy.Text))
            {
                MessageBox.Show("无住院号", "提示");
                txt_zyhjszy.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(this.txt_jshjszy.Text))
            {
                MessageBox.Show("无结算号", "提示");
                txt_jshjszy.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(this.txt_cyyydmzy.Text))
            {
                MessageBox.Show("无出院原因代码", "提示");
                txt_cyyydmzy.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(this.txt_zhsybzzy.Text))
            {
                MessageBox.Show("无账户使用标志", "提示");
                txt_zhsybzzy.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(this.txt_ztjsbzzy.Text))
            {
                MessageBox.Show("无中途结算标志", "提示");
                txt_ztjsbzzy.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(this.txt_jssjjszy.Text))
            {
                MessageBox.Show("无结算时间", "提示");
                txt_jssjjszy.Focus();
                return;
            }

            object[] obj = { txt_zyhjszy.Text.Trim(), txt_jshjszy.Text.Trim(), txt_cyyydmzy.Text.Trim(), txt_zhsybzzy.Text.Trim(), txt_ztjsbzzy.Text.Trim(), txt_jssjjszy.Text.Trim() };
            obj = yb_interface.ybs_interface("4401", obj);

            if (obj[1].ToString() == "1")
            {
                MessageBox.Show("结算成功", "提示");
            }
            else
            {
                MessageBox.Show(obj[2].ToString(), "提示");
            }
        }

        private void btn_jscxzy_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(this.txt_zyhjscxzy.Text))
            {
                MessageBox.Show("无住院号", "提示");
                txt_zyhjscxzy.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(this.txt_jshcxzy.Text))
            {
                MessageBox.Show("无结算号", "提示");
                txt_jshcxzy.Focus();
                return;
            }

            object[] obj = { txt_zyhjscxzy.Text.Trim(), txt_jshcxzy.Text.Trim() };
            obj = yb_interface.ybs_interface("4402", obj);

            if (obj[1].ToString() == "1")
            {
                MessageBox.Show("撤销结算成功", "提示");
            }
            else
            {
                MessageBox.Show(obj[2].ToString(), "提示");
            }
        }

        private void cmb_yllbmz_SelectedValueChanged(object sender, EventArgs e)
        {
            if (!lbl_dkfh.Text.Contains("|"))
            {
                MessageBox.Show("请先读卡", "提示");
                return;
            }

            cmb_mxbmz.Items.Clear();
            string strSql = string.Format(@"select bzmem1, bzmem2 from bztbd where bzcodn = 'YL' and bzmem1 = '{0}'", cmb_yllbmz.SelectedValue);
            DataSet ds = CliUtils.ExecuteSql("scomm", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);

            if (ds.Tables[0].Rows.Count > 0)
            {
                DataRow dr = ds.Tables[0].Rows[0];

                if (!string.IsNullOrEmpty(dr["bzmem2"].ToString()))
                {
                    string splbdm = dr["bzmem2"].ToString();
                    object[] obj = { lbl_dkfh.Text.Split('|')[0], splbdm };
                    obj = yb_interface.ybs_interface("5111", obj);

                    if (obj[1].ToString() == "1")
                    {
                        string bzs = obj[2].ToString();

                        if (bzs.Split('|')[0] == "1")
                        {
                            string[] bz = bzs.Split('$');

                            for (int i = 0; i < bz.Length; i++)
                            {
                                cmb_mxbmz.Items.Add(bz[i].ToString().Split('|')[6] + "|" + bz[i].ToString().Split('|')[7]);
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("无慢性病", "提示");
                    }
                }
            }
        }

        private void btn_fsxx_Click(object sender, EventArgs e)
        {
            if (!lbl_dkfh.Text.Contains("|"))
            {
                MessageBox.Show("请先读卡", "提示");
                return;
            }

            object[] obj = { lbl_dkfh.Text.Split('|')[0], lbl_dkfh.Text.Split('|')[7] };
            obj = yb_interface.ybs_interface("5110", obj);
            lbl_fsxx.Text = obj[1].ToString() + "," + obj[2].ToString();
        }

        private void btn_czjy_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(this.txt_jybm.Text))
            {
                MessageBox.Show("无交易编码", "提示");
                txt_jybm.Focus();
                return;
            }
            else if (string.IsNullOrWhiteSpace(this.txt_jylsh.Text))
            {
                MessageBox.Show("无交易流水号", "提示");
                txt_jylsh.Focus();
                return;
            }

            object[] obj = { txt_jybm.Text.Trim(), txt_jylsh.Text.Trim() };
            obj = yb_interface.ybs_interface("4500", obj);

            if (obj[1].ToString() == "1")
            {
                MessageBox.Show("冲正交易成功", "提示");
            }
            else
            {
                MessageBox.Show(obj[2].ToString(), "提示");
            }
        }

        private void btn_ghdjcxmz_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(this.txt_ghlshcxmz.Text))
            {
                MessageBox.Show("无挂号流水号", "提示");
                txt_ghlshcxmz.Focus();
                return;
            }

            object[] obj = { txt_ghlshcxmz.Text.Trim() };
            obj = yb_interface.ybs_interface("3101", obj);

            if (obj[1].ToString() == "1")
            {
                MessageBox.Show("撤销挂号登记成功", "提示");
            }
            else
            {
                MessageBox.Show(obj[2].ToString(), "提示");
            }
        }

        private void cmb_yllbmzjs_SelectedValueChanged(object sender, EventArgs e)
        {
            if (!lbl_dkfh.Text.Contains("|"))
            {
                MessageBox.Show("请先读卡", "提示");
                return;
            }

            this.cmb_mxbmzjs.Items.Clear();
            string strSql = string.Format(@"select bzmem1, bzmem2 from bztbd where bzcodn = 'YL' and bzmem1 = '{0}'", cmb_yllbmzjs.SelectedValue);
            DataSet ds = CliUtils.ExecuteSql("scomm", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);

            if (ds.Tables[0].Rows.Count > 0)
            {
                DataRow dr = ds.Tables[0].Rows[0];

                if (!string.IsNullOrEmpty(dr["bzmem2"].ToString()))
                {
                    string splbdm = dr["bzmem2"].ToString();
                    object[] obj = { lbl_dkfh.Text.Split('|')[0], splbdm };
                    obj = yb_interface.ybs_interface("5111", obj);

                    if (obj[1].ToString() == "1")
                    {
                        string bzs = obj[2].ToString();

                        if (bzs.Split('|')[0] == "1")
                        {
                            string[] bz = bzs.Split('$');

                            for (int i = 0; i < bz.Length; i++)
                            {
                                cmb_mxbmzjs.Items.Add(bz[i].ToString().Split('|')[6] + "|" + bz[i].ToString().Split('|')[7]);
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("无慢性病", "提示");
                    }
                }
            }
        }

        private void btn_ghdj_Click(object sender, EventArgs e)
        {
            if (!lbl_dkfh.Text.Contains("|"))
            {
                MessageBox.Show("请先读卡", "提示");
                return;
            }

            if (string.IsNullOrWhiteSpace(this.txt_ghlshmz.Text))
            {
                MessageBox.Show("无挂号流水号", "提示");
                txt_ghlshmz.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(this.cmb_yllbmz.SelectedValue.ToString()))
            {
                MessageBox.Show("无医疗类别", "提示");
                return;
            }

            if (string.IsNullOrWhiteSpace(this.txt_jssjghmz.Text))
            {
                MessageBox.Show("无结算时间", "提示");
                txt_jssjghmz.Focus();
                return;
            }

            string bzbm = "";
            string bzmc = "";

            if (cmb_mxbmz.Text.Contains("|"))
            {
                bzbm = cmb_mxbmz.Text.Split('|')[0];
                bzmc = cmb_mxbmz.Text.Split('|')[1];
            }
            else
            {
                cmb_yllbmz.SelectedIndex = 0;
            }

            object[] obj = { txt_ghlshmz.Text.Trim(), cmb_yllbmz.SelectedValue, bzbm, bzmc, lbl_dkfh.Text, txt_jssjghmz.Text.Trim() };
            obj = yb_interface.ybs_interface("3100", obj);

            if (obj[1].ToString() == "1")
            {
                MessageBox.Show("挂号登记成功", "提示");
            }
            else
            {
                MessageBox.Show(obj[2].ToString(), "提示");
            }
        }

        private void btn_wkcx_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txt_sfzh.Text))
            {
                MessageBox.Show("无身份证号", "提示");
                txt_sfzh.Focus();
                return;
            }

            lbl_dkfh.Text = "";
            lbl_fsxx.Text = "";
            lbl_ydry.Text = "";
            lbl_zhye.Text = "";
            lbl_zyzt.Text = "";
            object[] obj = { txt_sfzh.Text.Trim() };
            obj = yb_interface.ybs_interface("1400",obj);

            if (obj[1].ToString() == "1")
            {
                MessageBox.Show("查询成功", "提示");
                lbl_dkfh.Text = obj[2].ToString();
                lbl_zhye.Text = obj[2].ToString().Split('|')[14];
                lbl_zyzt.Text = obj[2].ToString().Split('|')[13] == "0" ? "不在院" : "在院";

                if (obj[2].ToString().Split('|')[10] == "0")
                {
                    lbl_ydry.Text = "否";
                    btn_fsxx.Enabled = true;
                    //groupBox_zycfmxsb.Visible = true;
                    //groupBox_zycfmxsbcx.Visible = true;
                }
                else
                {
                    lbl_ydry.Text = "是";
                    btn_fsxx.Enabled = false;
                    //groupBox_zycfmxsb.Visible = false;
                    //groupBox_zycfmxsbcx.Visible = false;
                }
            }
            else
            {
                MessageBox.Show("查询失败：" + obj[2].ToString(), "提示");
            }
        }

        private void btn_cfmxzdsbzy_Click(object sender, EventArgs e)
        {
            yb_interface.ybs_interface("4302", null);
        }

        private void btn_Close_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btn_jzdjxg_Click(object sender, EventArgs e)
        {
            object[] obj = { txt_ybjzlsh.Text, txt_grbh.Text, this.txt_xm.Text.Trim(), this.txt_kh.Text.Trim(), this.txt_yllb.Text};
            obj = yb_interface.ybs_interface("4101", obj);

            if (obj[1].ToString() == "1")
            {
                MessageBox.Show("入院修改成功", "提示");
            }
            else
            {
                MessageBox.Show(obj[2].ToString(), "提示");
            }
        }
    }
}
