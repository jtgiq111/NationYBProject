using Srvtools;
using System;

namespace ybinterface_lib
{
    public partial class Frm_ybdejjtsdr : InfoForm
    {
        decimal dejjzf, bndezflj;
        string tcq, dwmc;

        public Frm_ybdejjtsdr(decimal dejjzf, decimal bndezflj,string tcq, string dwmc)
        {
            InitializeComponent();
            this.dejjzf = dejjzf;
            this.bndezflj = bndezflj;
            this.tcq = tcq;
            this.dwmc = dwmc;
        }

        private void frmybzdscdrjj_Load(object sender, EventArgs e)
        {
            lblMess.Text="";

            if (dejjzf > 0)
            {
                lblMess.Text += "该病人大额基金支付为：" + dejjzf + "，统筹区：" + tcq + "，单位：" + dwmc + "，请保留发票！\n";
            }

            if(bndezflj > 0)
            {
                lblMess.Text += "该病人本年大额支付累计(不含本次)为：" + bndezflj + "，统筹区：" + tcq + "，单位：" + dwmc + "，请保留发票！";
            }
        }

        private void btn_Close_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
