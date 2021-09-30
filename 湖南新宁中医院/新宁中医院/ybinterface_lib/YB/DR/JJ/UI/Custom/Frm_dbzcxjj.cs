using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Srvtools;

namespace ybinterface_lib
{
    public partial class Frm_dbzcxjj : InfoForm
    {
        public Frm_dbzcxjj()
        {
            InitializeComponent();
        }

        private void Frm_dbzcxJNA_Load(object sender, EventArgs e)
        {

        }

        private void btn_cx_Click(object sender, EventArgs e)
        {

            object[] obj = { ybjzlsh.Text.Trim(), djh.Text.Trim(), cysj.Text.Trim(), grbh.Text.Trim(), xm.Text.Trim(), kh.Text.Trim(), zxbm.Text.Trim() };
            obj = yb_interface.ybs_interface("5007", obj);

            MessageBox.Show(obj[0].ToString());
        }
    }
}
