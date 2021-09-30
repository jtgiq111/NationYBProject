using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ybinterface_lib
{
    public partial class Frm_ybmzfyjscxYT : Form
    {
        public Frm_ybmzfyjscxYT()
        {
            InitializeComponent();
        }


        private void btnCLOSE_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (!string.IsNullOrWhiteSpace(txtJZLSH.Text))
                {
                    txtJZLSH.Text = txtJZLSH.Text.PadLeft(8, '0');
                }
            }
        }
    }
}
