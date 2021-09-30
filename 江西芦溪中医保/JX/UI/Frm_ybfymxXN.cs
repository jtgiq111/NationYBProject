using Srvtools;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace yb_interfaces.JX.UI
{
    public partial class Frm_ybfymxXN : InfoForm
    {
        public Frm_ybfymxXN()
        {
            InitializeComponent();
        }

        private void Frm_ybfymx_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) 
            {
                this.Close();
            }
        }
    }
}
