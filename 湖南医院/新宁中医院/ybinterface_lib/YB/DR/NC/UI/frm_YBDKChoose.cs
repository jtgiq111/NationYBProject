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
    public partial class frm_YBDKChoose : Form
    {
        public string strReutrn = "0|1|";
        public frm_YBDKChoose()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            strReutrn="";
            if (ck_ybklx1.Checked)
                strReutrn += "1|";
            else
                strReutrn += "0|";
            if (ck_ydybk1.Checked)
                strReutrn += "2|";
            else
                strReutrn += "1|";
            this.Close();
        }
    }
}
