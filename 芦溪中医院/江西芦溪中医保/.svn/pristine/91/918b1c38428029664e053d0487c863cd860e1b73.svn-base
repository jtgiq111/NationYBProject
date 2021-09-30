using FastReport;
using Srvtools;
using System;
using System.Data;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace yb_interfaces
{
    public partial class Frm_dkck : InfoForm
    {
        public string dzybm = string.Empty;
        public DKType dzybFlag = default(DKType);

        public Frm_dkck()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
        }

        private void btnprint_Click(object sender, EventArgs e)
        {
            
        }

        private void btncx_Click(object sender, EventArgs e)
        {
            if (rbsfz.Checked)
            {
                dzybFlag = DKType.身份证;
            }
            else if (rbsbk.Checked)
            {
                dzybFlag = DKType.社保卡;
            }
            else if(rbtndzk.Checked)
            {
                if (string.IsNullOrWhiteSpace(txtCode.Text.ToString()))
                {
                    AutoListView.MyTip.ShowMsg("请先扫码！", 5);
                    txtCode.Focus();
                    return;
                }
                dzybm = txtCode.Text.ToString();
                dzybFlag = DKType.电子凭证;
            }
            this.Close();
        }

        private void btn_Close_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Frm_dkck_FormClosed(object sender, FormClosedEventArgs e)
        {

        }

        private void rbtndzk_CheckedChanged(object sender, EventArgs e)
        {
            if (rbtndzk.Checked)
            {
                txtCode.Focus();
            }
        }
    }
    public enum DKType { 
        身份证=1,
        社保卡=2,
        电子凭证=3
    }

}