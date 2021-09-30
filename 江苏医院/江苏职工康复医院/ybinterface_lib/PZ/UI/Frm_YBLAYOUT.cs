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
    public partial class Frm_YBLAYOUT : Form
    {
        public Frm_YBLAYOUT()
        {
            InitializeComponent();
        }

        private void Frm_YBLAYOUT_Load(object sender, EventArgs e)
        {
            tssl_czyxm.Text = CliUtils.fUserName;

        }
    }
}
