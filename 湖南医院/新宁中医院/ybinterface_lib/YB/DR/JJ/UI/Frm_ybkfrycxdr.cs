using Srvtools;
using System;
using System.Data;
using System.Windows.Forms;

namespace ybinterface_lib
{
    public partial class Frm_ybkfrycxdr : InfoForm
    {
        string jzbz = "";

        public Frm_ybkfrycxdr()
        {
            InitializeComponent();
        }

        private void btnjscx_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(this.txtybjzlsh.Text))
            {
                MessageBox.Show("请输入医保就诊流水号", "提示");
                txtybjzlsh.Focus();
                return;
            }
            else if (string.IsNullOrWhiteSpace(this.txtgrbh.Text))
            {
                MessageBox.Show("请输入个人编号", "提示");
                txtgrbh.Focus();
                return;
            }
            else if (string.IsNullOrWhiteSpace(this.txtxm.Text))
            {
                MessageBox.Show("请输入姓名", "提示");
                txtxm.Focus();
                return;
            }
            else if (string.IsNullOrWhiteSpace(this.txtkh.Text))
            {
                MessageBox.Show("请输入卡号", "提示");
                txtkh.Focus();
                return;
            }
            else if (string.IsNullOrWhiteSpace(this.txtfph.Text))
            {
                MessageBox.Show("请输入发票号", "提示");
                txtfph.Focus();
                return;
            }
            else if (string.IsNullOrWhiteSpace(this.txtjsrq.Text))
            {
                MessageBox.Show("请输入结算日期", "提示");
                txtjsrq.Focus();
                return;
            }
            else if (string.IsNullOrWhiteSpace(this.txt_zxbm.Text))
            {
                MessageBox.Show("请输入中心编码", "提示");
                txt_zxbm.Focus();
                return;
            }

            object[] re = yb_interface.ybs_interface("5007",new object[]{ txtybjzlsh.Text, txtfph.Text, txtjsrq.Text, txtgrbh.Text, txtxm.Text, txtkh.Text, txt_zxbm.Text});
            int i = Convert.ToInt32(re[0].ToString());
            if (i == 1)
            {
                MessageBox.Show("撤销成功", "提示");
            }
            else
            {
                MessageBox.Show("撤销失败", "提示");
            }
        }

        private void btndjcx_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(this.txtybjzlsh.Text))
            {
                MessageBox.Show("请输入医保就诊流水号", "提示");
                txtybjzlsh.Focus();
                return;
            }
            else if (string.IsNullOrWhiteSpace(this.txtgrbh.Text))
            {
                MessageBox.Show("请输入个人编号", "提示");
                txtgrbh.Focus();
                return;
            }
            else if (string.IsNullOrWhiteSpace(this.txtxm.Text))
            {
                MessageBox.Show("请输入姓名", "提示");
                txtxm.Focus();
                return;
            }
            else if (string.IsNullOrWhiteSpace(this.txtkh.Text))
            {
                MessageBox.Show("请输入卡号", "提示");
                txtkh.Focus();
                return;
            }
            else if (string.IsNullOrWhiteSpace(this.txt_zxbm.Text))
            {
                MessageBox.Show("请输入中心编码", "提示");
                txt_zxbm.Focus();
                return;
            }
            object[] obj = yb_interface.ybs_interface("5006",new object[]{txtybjzlsh.Text, txtgrbh.Text, txtxm.Text, txtkh.Text, txt_zxbm.Text});
            int i = Convert.ToInt32(obj[0].ToString());
            
            if (i == 1)
            {
                MessageBox.Show("撤销成功", "提示");
            }
            else
            {
                MessageBox.Show("撤销失败", "提示");
            }
        }

        private void btnmxcx_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(this.txtybjzlsh.Text))
            {
                MessageBox.Show("请输入医保就诊流水号", "提示");
                txtybjzlsh.Focus();
                return;
            }
            else if (string.IsNullOrWhiteSpace(this.txtgrbh.Text))
            {
                MessageBox.Show("请输入个人编号", "提示");
                txtgrbh.Focus();
                return;
            }
            else if (string.IsNullOrWhiteSpace(this.txtxm.Text))
            {
                MessageBox.Show("请输入姓名", "提示");
                txtxm.Focus();
                return;
            }
            else if (string.IsNullOrWhiteSpace(this.txtkh.Text))
            {
                MessageBox.Show("请输入卡号", "提示");
                txtkh.Focus();
                return;
            }
            else if (string.IsNullOrWhiteSpace(this.txt_zxbm.Text))
            {
                MessageBox.Show("请输入中心编码", "提示");
                txt_zxbm.Focus();
                return;
            }

            object[] r = yb_interface.ybs_interface("5505", new object[] { txtybjzlsh.Text, txtgrbh.Text, txtxm.Text, txtkh.Text, txt_zxbm.Text });
            int i = Convert.ToInt32(r[0].ToString());

            if (i == 1)
            {
                MessageBox.Show("撤销成功", "提示");
            }
            else
            {
                MessageBox.Show("撤销失败", "提示");
            }
        }

        private void btn_Close_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btn_sjkmxcx_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txt_ghzylsh.Text))
            {
                MessageBox.Show("请输入挂号/住院流水号并回车", "提示");
                return;
            }

            object[] re = yb_interface.ybs_interface("5008", new object[]{txt_ghzylsh.Text.Trim(), jzbz});
            

            if (re[0].ToString() == "1")
            {
                MessageBox.Show("撤销成功", "提示");
                return;
            }
            else
            {
                MessageBox.Show("撤销成失败", "提示");
                return;
            }

        }

        private void txt_ghzylsh_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (string.IsNullOrWhiteSpace(txt_ghzylsh.Text))
                {
                    MessageBox.Show("请输入挂号/住院流水号并回车", "提示");
                    return;
                }

                string ghzylsh = txt_ghzylsh.Text.Trim();
                string sql = string.Format(@"select jzlsh, grbh, xm from ybmzzydjdr a where a.jzlsh = '{0}' and cxbz = 1", ghzylsh);
                DataTable dt = CliUtils.ExecuteSql("sybdj", "cmd", sql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject).Tables[0];

                if (dt != null && dt.Rows.Count > 0)
                {
                    DataRow dr = dt.Rows[0];
                    lbl_xm.Text = dr["xm"].ToString();
                    lbl_grbh.Text = dr["grbh"].ToString();
                    lbl_jzlsh.Text = dr["jzlsh"].ToString();
                }
                else
                {
                    MessageBox.Show("医保提示：就诊流水号" + ghzylsh + "未办理医保挂号", "提示");
                    return;
                }
            }
        }

        private void btn_cz_Click(object sender, EventArgs e)
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

            object[] obj = { txt_jybm.Text.Trim(), txt_jylsh.Text.Trim(), txt_zxbmcz.Text.Trim() };
            obj = yb_interface.ybs_interface("2421", obj);

            if (obj[1].ToString() == "1")
            {
                MessageBox.Show("冲正交易成功", "提示");
            }
            else
            {
                MessageBox.Show(obj[2].ToString(), "提示");
            }
        }
    }
}
