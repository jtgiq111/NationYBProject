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
    public partial class frm_inputPW : Form
    {
        public string sValue = string.Empty;

        public frm_inputPW()
        {
            InitializeComponent();
        }

        private void frm_inputPW_Load(object sender, EventArgs e)
        {
            txtPWD.Focus();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            sValue = txtPWD.Text.Trim();
            this.Close();
        }

    }
}
