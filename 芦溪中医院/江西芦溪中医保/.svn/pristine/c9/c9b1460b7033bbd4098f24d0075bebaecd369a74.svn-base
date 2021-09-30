using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Srvtools;
using yb_interfaces;

namespace yb_interfaces
{
    public partial class frm_ybdzcxXN : InfoForm
    {
        private string ywCode = string.Empty;

        public frm_ybdzcxXN()
        {
            InitializeComponent();
        }

        private void frm_ybdzcxTS_Load(object sender, EventArgs e)
        {
            dtp_Start.Text = DateTime.Now.ToString("yyyy-MM-dd");
            dtp_End.Text = DateTime.Now.ToString("yyyy-MM-dd");

        }

        private void btnZEDZ_Click(object sender, EventArgs e)
        {
            string dtStart = dtp_Start.Text;
            string dtEnd = dtp_End.Text;
            string ywzqh = txtYWZQH.Text.Trim();
            ywCode="";
            string qslb = cmbqslb.SelectedItem.ToString().Substring(0, 2);//清算类别
            string xzlx=cmbxzlb.SelectedItem.ToString().Substring(0, 3);//险种类型
            string ylfze = "0";//医疗总金额
            string pubCost = "0";//基金金额
            string payCost = "0";//账户金额
            string ylfbs = "0";//医疗费用笔数

            object[] objParam = { xzlx,qslb,dtStart, dtEnd,ylfze,pubCost,payCost,ylfbs};
            object[] objres = yb_interface_jx.YBDZZZ(objParam);
            if (objres[1].ToString() == "1")
            {
                object[] outparams = objres[3] as object[];
                MessageBox.Show($"医保对账结果是{outparams[1].ToString()}!\r\n 对账结果说明{outparams[2].ToString()}");
            }
            else
            {
                MessageBox.Show("医保对账失败！"+objres[2].ToString());
            }
            //object[] objReturn=yb_interface.ybs_interface(
        }

        private void btnMXDZ_Click(object sender, EventArgs e)
        {

        }

        private void btnFYMXXZ_Click(object sender, EventArgs e)
        {

        }
    }
}
