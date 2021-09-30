using Srvtools;
using System;
using System.Windows.Forms;

namespace ybinterface_lib
{
    public partial class frmybcxdrSR : InfoForm
    {
        public frmybcxdrSR()
        {
            InitializeComponent();
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

            if (string.IsNullOrWhiteSpace(this.txt_fphjscxmz.Text))
            {
                MessageBox.Show("无发票号", "提示");
                txt_fphjscxmz.Focus();
                return;
            }

            object[] obj = { txt_ghlshjscxmz.Text.Trim(), txt_fphjscxmz.Text.Trim() };
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

        private void btn_cfmxsbcxzy_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(this.txt_zyhsbcxzy.Text))
            {
                MessageBox.Show("无住院号", "提示");
                txt_zyhsbcxzy.Focus();
                return;
            }

            object[] obj = { txt_zyhsbcxzy.Text.Trim() };
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

        private void btn_jscxzy_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(this.txt_zyhjscxzy.Text))
            {
                MessageBox.Show("无住院号", "提示");
                txt_zyhjscxzy.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(this.txt_fphcxzy.Text))
            {
                MessageBox.Show("无发票号", "提示");
                txt_fphcxzy.Focus();
                return;
            }

            object[] obj = { txt_zyhjscxzy.Text.Trim(), txt_fphcxzy.Text.Trim() };
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

        private void btn_ghdjcxmz_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(this.txt_ghlshcxmz.Text))
            {
                MessageBox.Show("无挂号流水号", "提示");
                txt_ghlshcxmz.Focus();
                return;
            }

            object[] obj = { txt_ghlshcxmz.Text.Trim() };
            obj = yb_interface.ybs_interface("2240", obj);

            if (obj[1].ToString() == "1")
            {
                MessageBox.Show("撤销挂号登记成功", "提示");
            }
            else
            {
                MessageBox.Show(obj[2].ToString(), "提示");
            }
        }
    }
}
