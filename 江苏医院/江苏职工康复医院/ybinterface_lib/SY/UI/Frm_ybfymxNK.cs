using Srvtools;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ybinterface_lib.SY.UI
{
    public partial class Frm_ybfymxNK : InfoForm
    {
        public Frm_ybfymxNK()
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
