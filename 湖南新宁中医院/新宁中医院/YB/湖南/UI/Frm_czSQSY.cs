using Srvtools;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms; 

namespace yb_interfaces
{
    public partial class Frm_czXHWY : InfoForm
    {
        public Frm_czXHWY()
        {
            InitializeComponent();
        }

        private void btnCXJY_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(this.txtdqbh.Text))
            {
                MessageBox.Show("参保地区编号不能为空", "提示");
                txtdqbh.Focus();
                return;
            }
            if (string.IsNullOrWhiteSpace(this.txtrybh.Text))
            {
                MessageBox.Show("人员编号不能为空", "提示");
                txtrybh.Focus();
                return;
            }
            if (string.IsNullOrWhiteSpace(this.txtywbm.Text))
            {
                MessageBox.Show("业务交易编码不能为空", "提示");
                txtywbm.Focus();
                return;
            }
            if (string.IsNullOrWhiteSpace(this.txtjylsh.Text))
            {
                MessageBox.Show("需冲正交易流水号不能为空", "提示");
                txtjylsh.Focus();
                return;
            }
            object[] obj = { txtywbm.Text.Trim(), txtjylsh.Text.Trim(), "" ,this.txtrybh.Text ,txtdqbh.Text};
            obj = yb_interface_hn_nkNew.YBCZJY(obj);
            MessageBox.Show(obj[2].ToString());
        }

      

        private void Frm_czXH_Load(object sender, EventArgs e)
        {
         

        }
    }
}
