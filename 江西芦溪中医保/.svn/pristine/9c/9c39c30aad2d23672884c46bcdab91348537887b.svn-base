using Srvtools;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace yb_interfaces
{
    public partial class FrmYBQT : InfoForm
    {
        public FrmYBQT()
        {
            InitializeComponent();
        }

        private void btnybtc_Click(object sender, EventArgs e)
        {
            DialogResult drs = MessageBox.Show("\r\n 是否进行医保签退操作，签退后该工号当天无法进行医保业务操作？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (drs == DialogResult.Yes)
            {
                if (! string.IsNullOrEmpty(yb_interface_jx.YbSignNo))
                yb_interface_jx.YBEXIT(new object[] { CliUtils.fLoginUser});
            }
        }
    }
}
