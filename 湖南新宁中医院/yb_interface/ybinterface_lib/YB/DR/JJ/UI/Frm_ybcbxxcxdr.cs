using Srvtools;
using System;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace ybinterface_lib
{
    public partial class Frm_ybcbxxcxdr : InfoForm
    {
        public Frm_ybcbxxcxdr()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            lblgrbh.Text = "";
            lblshkh.Text = "";
            string sql = "select bzkeyx, bzname from bztbd(nolock) where bzcodn = 'SP'";
            DataTable dt = CliUtils.ExecuteSql("sybdj", "cmd", sql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject).Tables[0];
            DataRow dr = dt.NewRow();
            dr["bzname"] = "----请选择审批类别----";
            dr["bzkeyx"] = "";
            dt.Rows.InsertAt(dr, 0);
            this.cmbyldysplb.DataSource = dt;
            cmbyldysplb.DisplayMember = "bzname";
            cmbyldysplb.ValueMember = "bzkeyx";

            sql = "select bzkeyx, bzname from bztbd(nolock) where bzcodn = 'DQ' and bzusex = 1";
            dt = CliUtils.ExecuteSql("sybdj", "cmd", sql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject).Tables[0];
            dr = dt.NewRow();
            dr["bzname"] = "--统筹区--";
            dr["bzkeyx"] = "--统筹区--";
            dt.Rows.InsertAt(dr, 0);
            cmb_tcq.DataSource = dt;
            cmb_tcq.DisplayMember = "bzname";
            cmb_tcq.ValueMember = "bzkeyx";
        }

        private void btnDK_Click(object sender, EventArgs e)
        {
            try
            {
                object[] obj = yb_interface.ybs_interface("2101", null);

                if (obj[1].ToString() != "1")
                {
                    MessageBox.Show("读卡失败：" + obj[2].ToString(), "提示");
                    return;
                }

                txtRemark.Text = obj[2].ToString();
                string[] ccmx = obj[2].ToString().Split('|');
                lblgrbh.Text = ccmx[0];
                this.lbldwbh.Text = ccmx[1];
                this.lblsfzh.Text = ccmx[2];
                lblxm.Text = ccmx[3];
                lblxb.Text = ccmx[4];
                lblmz.Text = ccmx[5];
                lblcsrq.Text = ccmx[6];
                lblshkh.Text = ccmx[7];
                lblyldylb.Text = ccmx[8];
                string sql = string.Format("select bzname from bztbd(nolock) where bzcodn = 'DL' and bzmem1 = '{0}'", ccmx[8]);
                DataTable dt = CliUtils.ExecuteSql("sybdj", "cmd", sql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject).Tables[0];

                if (dt != null && dt.Rows.Count > 0)
                {
                    DataRow dr = dt.Rows[0];
                    lblyldylb.Text += dr["bzname"].ToString();
                }

                lblrycbzt.Text = ccmx[9];
                lblydrybz.Text = ccmx[10];
                lbltcqh.Text = ccmx[11];
                sql = string.Format("select bzname from bztbd(nolock) where bzcodn = 'DQ' and bzkeyx = '{0}' and bzusex = 1", ccmx[11]);
                dt = CliUtils.ExecuteSql("sybdj", "cmd", sql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject).Tables[0];

                if (dt != null && dt.Rows.Count > 0)
                {
                    DataRow dr = dt.Rows[0];
                    lbltcqh.Text += dr["bzname"].ToString();
                }

                lblnd.Text = ccmx[12];
                lblzyzt.Text = ccmx[13];
                lblzhye.Text = ccmx[14];
                lblbnylflj.Text = ccmx[15];
                lblbnzhzclj.Text = ccmx[16];
                lblbntczclj.Text = ccmx[17];
                lblbnjzjzclj.Text = ccmx[18];
                lblbngwybzjjlj.Text = ccmx[19];
                lblbnczjmmztczfj.Text = ccmx[20];
                lbljrtcfylj.Text = ccmx[21];
                lbljrjzjfylj.Text = ccmx[22];
                lblqfbzlj.Text = ccmx[23];
                lblbnzycs.Text = ccmx[24];
                lbldwmc.Text = ccmx[25];
                lblnl.Text = ccmx[26];
                lblcbdwlx.Text = ccmx[27];
                lbljbjgbm.Text = ccmx[28];
            }
            catch (Exception error)
            {
                MessageBox.Show(error.ToString());
            }
        }

        private void btnfscxynn_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(lblgrbh.Text) && (string.IsNullOrWhiteSpace(txt_grbh_fs.Text) || string.IsNullOrWhiteSpace(txt_kh_fs.Text)))
            {
                MessageBox.Show("请先读卡或输入个人编号和卡号", "提示");
                return;
            }

            string grbh = lblgrbh.Text == "" ? txt_grbh_fs.Text.Trim() : lblgrbh.Text;
            string kh = lblshkh.Text == "" ? txt_kh_fs.Text.Trim() : lblshkh.Text;
            object[] obj = yb_interface.ybs_interface("5010", new object[]{grbh, kh});
            txtRemark.Text = obj[2].ToString();
        }

        private void btnjzdjcx_Click(object sender, EventArgs e)
        {
            string kssj = dpkssj.Value.ToString("yyyyMMdd") + "000000";
            string zzsj = dpzzsj.Value.AddDays(1).ToString("yyyyMMdd") + "000000";
            object[] obj = { kssj, zzsj, txt_grbh.Text };
            obj = yb_interface.ybs_interface("1103",obj);
            txtRemark.Text = obj[2].ToString();
        }

        private void btnyldyspxxcx_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txt_grbh.Text))
            {
                MessageBox.Show("请输入个人编号", "提示");
                return;
            }
            if (cmbyldysplb.SelectedValue.ToString() == "")
            {
                MessageBox.Show("请选择审批类别", "提示");
                return;
            }

            object[] obj = { txt_grbh.Text, cmbyldysplb.SelectedValue.ToString() };
            obj = yb_interface.ybs_interface("1600", obj);
            txtRemark.Text = obj[2].ToString();
        }

        private void btnspxxsc_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txt_grbh.Text))
            {
                MessageBox.Show("请先读卡", "提示");
                return;
            }
            else if (cmbyldysplb.SelectedValue.ToString() == "")
            {
                MessageBox.Show("请选择审批类别", "提示");
                return;
            }

            object[] objlxzy = yb_interface.ybs_interface("3110",new object[]{txt_grbh.Text, cmbyldysplb.SelectedValue.ToString()});

            if (objlxzy[1].ToString() == "1")
            {
                MessageBox.Show("审批信息上报成功", "提示");
            }
            else
            {
                MessageBox.Show("审批信息上报失败：" + objlxzy[1].ToString() + objlxzy[2].ToString(), "提示");
            }
        }

        private void btnspxxcx_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(lblgrbh.Text))
            {
                MessageBox.Show("请先读卡", "提示");
                return;
            }
            else if (cmbyldysplb.SelectedValue.ToString() == "")
            {
                MessageBox.Show("请选择审批类别", "提示");
                return;
            }

            object[] re = yb_interface.ybs_interface("3120",new object[] {cmbyldysplb.SelectedValue.ToString(), lblgrbh.Text});

            if (re[0].ToString() == "1")
            {
                MessageBox.Show("审批信息撤销成功", "提示");
            }
            else
            {
                MessageBox.Show("审批信息撤销失败", "提示");
            }
        }

        private void btnCX_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtSFZH.Text))
            {
                MessageBox.Show("请输入身份证号", "提示");
                return;
            }

            object[] obj = { txtSFZH.Text.Trim() };
            obj = yb_interface.ybs_interface("1400",obj);
            txtRemark.Text = obj[2].ToString();

            if (obj[1].ToString() == "1")
            {
                string[] ccmx = obj[2].ToString().Split('|');
                lblgrbh.Text = ccmx[0];
                this.lbldwbh.Text = ccmx[1];
                this.lblsfzh.Text = ccmx[2];
                lblxm.Text = ccmx[3];
                lblxb.Text = ccmx[4];
                lblmz.Text = ccmx[5];
                lblcsrq.Text = ccmx[6];
                lblshkh.Text = ccmx[7];
                lblyldylb.Text = ccmx[8];
                lblrycbzt.Text = ccmx[9];
                lblydrybz.Text = ccmx[10];
                lbltcqh.Text = ccmx[11];
                string sql = string.Format("select bzname from bztbd(nolock) where bzcodn = 'DQ' and bzkeyx = '{0}' and bzusex = 1", ccmx[11]);
                DataTable dt = CliUtils.ExecuteSql("sybdj", "cmd", sql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject).Tables[0];

                if (dt != null && dt.Rows.Count > 0)
                {
                    DataRow dr = dt.Rows[0];
                    lbltcqh.Text += dr["bzname"].ToString();
                }

                lblnd.Text = ccmx[12];
                lblzyzt.Text = ccmx[13];
                lblzhye.Text = ccmx[14];
                lblbnylflj.Text = ccmx[15];
                lblbnzhzclj.Text = ccmx[16];
                lblbntczclj.Text = ccmx[17];
                lblbnjzjzclj.Text = ccmx[18];
                lblbngwybzjjlj.Text = ccmx[19];
                lblbnczjmmztczfj.Text = ccmx[20];
                lbljrtcfylj.Text = ccmx[21];
                lbljrjzjfylj.Text = ccmx[22];
                lblqfbzlj.Text = ccmx[23];
                lblbnzycs.Text = ccmx[24];
                lbldwmc.Text = ccmx[25];
                lblnl.Text = ccmx[26];
                lblcbdwlx.Text = ccmx[27];
                lbljbjgbm.Text = ccmx[28];
            }
        }

        private void btnjsxxcx_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(this.txtjzlsh.Text))
            {
                MessageBox.Show("请输入就诊流水号", "提示");
                return;
            }

            //if (string.IsNullOrWhiteSpace(txtfph.Text))
            //{
            //    MessageBox.Show("请输入发票号", "提示");
            //    return;
            //}

            string jzlsh = this.txtjzlsh.Text.Trim();
            string fph = txtfph.Text.Trim();
            object[] obj = { jzlsh, fph };
            obj = yb_interface.ybs_interface("1101",obj);
            txtRemark.Text = obj[2].ToString();
        }

        private void txtjzlsh_KeyPress(object sender, KeyPressEventArgs e)
        {
            //if ((int)e.KeyChar >= 65296 && e.KeyChar <= 65305)
            //{
            //    e.KeyChar = Common.QuanBianBan(e.KeyChar);
            //}

            if (!(Char.IsNumber(e.KeyChar)) && e.KeyChar != (char)13 && e.KeyChar != (char)8)
            {
                e.Handled = true;
            }
        }

        private void btn_Close_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btn_grylfcx_Click(object sender, EventArgs e)
        {
            string kssj = dpkssj.Value.ToString("yyyyMMdd") + "000000";
            string zzsj = dpzzsj.Value.AddDays(1).ToString("yyyyMMdd") + "000000";
            object[] obj = { kssj, zzsj, txt_grbh.Text };
            obj = yb_interface.ybs_interface("1102",obj);
            txtRemark.Text = obj[2].ToString();
        }

        private void btn_fymxxxcx_Click(object sender, EventArgs e)
        {
            object[] obj = { txtjzlsh.Text };
            obj = yb_interface.ybs_interface("1200",obj);
            txtRemark.Text = obj[2].ToString();
        }

        private void btn_fyxxcx_Click(object sender, EventArgs e)
        {
            string kssj = dpkssj.Value.ToString("yyyyMMdd") + "000000";
            string zzsj = dpzzsj.Value.AddDays(1).ToString("yyyyMMdd") + "000000";
            object[] obj = { kssj, zzsj };
            obj = yb_interface.ybs_interface("1100",obj);
            txtRemark.Text = obj[2].ToString();
        }

        private void btn_yjsdz_Click(object sender, EventArgs e)
        {
            if (cmb_tcq.SelectedValue.ToString() == "--统筹区--")
            {
                MessageBox.Show("请选择统筹区", "提示");
                return;
            }

            string kssj = dpkssj.Value.ToString("yyyyMMdd") + "000000";
            string zzsj = dpzzsj.Value.AddDays(1).ToString("yyyyMMdd") + "000000";
            object[] obj = { kssj, zzsj, cmb_tcq.SelectedValue };
            obj = yb_interface.ybs_interface("5009",obj);
            txtRemark.Text = obj[2].ToString();
        }

        private void btn_fscxsjk_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(lblgrbh.Text) && (string.IsNullOrWhiteSpace(txt_grbh_fs.Text) || string.IsNullOrWhiteSpace(txt_kh_fs.Text)))
            {
                MessageBox.Show("请先读卡或输入个人编号和卡号", "提示");
                return;
            }

            string grbh = lblgrbh.Text == "" ? txt_grbh_fs.Text.Trim() : lblgrbh.Text;
            string kh = lblshkh.Text == "" ? txt_kh_fs.Text.Trim() : lblshkh.Text;
            object[] obj = yb_interface.ybs_interface("5010", new object[] {grbh, kh});
            txtRemark.Text = obj[2].ToString();
        }

        private void btn_fscx_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(lblgrbh.Text) && (string.IsNullOrWhiteSpace(txt_grbh_fs.Text) || string.IsNullOrWhiteSpace(txt_kh_fs.Text)))
            {
                MessageBox.Show("请先读卡或输入个人编号和卡号", "提示");
                return;
            }

            string grbh = lblgrbh.Text == "" ? txt_grbh_fs.Text.Trim() : lblgrbh.Text;
            string kh = lblshkh.Text == "" ? txt_kh_fs.Text.Trim() : lblshkh.Text;
            object[] obj = { grbh, kh };
            obj = yb_interface.ybs_interface("1500",obj);
            txtRemark.Text = obj[2].ToString();
        }
    }
}

