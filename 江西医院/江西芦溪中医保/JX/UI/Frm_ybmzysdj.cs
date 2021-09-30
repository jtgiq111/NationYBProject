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
    public partial class Frm_ybmzysdj : Form
    {
        public string sslb = "";
        public string ssrq = "";
        public string sylb = "";
        public string yzs = "";
        public string wybz = "0";   //0	否	1	是
        public string zcbz = "0";   //0	否	1	是

        public Frm_ybmzysdj()
        {
            InitializeComponent();
        }

        private void Frm_ybmzysdj_Load(object sender, EventArgs e)
        {
            dtSYRQ.Text = DateTime.Now.ToString("yyyy-MM-dd");

            string strSql = string.Format(@"select * from bztbd where bzcodn='birctrl_type'");
            DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            cmbSSLB.ValueMember = "bzkeyx";
            cmbSSLB.DisplayMember = "bzname";
            cmbSSLB.DataSource = ds.Tables[0].DefaultView;

            string strSql1 = string.Format(@"select * from bztbd where bzcodn='matn_type'");
            DataSet ds1 = CliUtils.ExecuteSql("sybdj", "cmd", strSql1, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            comboBox1.ValueMember = "bzkeyx";
            comboBox1.DisplayMember = "bzname";
            comboBox1.DataSource = ds1.Tables[0].DefaultView;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            sslb = cmbSSLB.SelectedValue.ToString();
            ssrq = dtSYRQ.Text.Trim();
            sylb = comboBox1.SelectedValue.ToString();
            try
            {
                if(string.IsNullOrEmpty(textBox1.Text.Trim()))
                {
                    int days = int.Parse(textBox1.Text.Trim());
                }
                yzs = textBox1.Text.Trim();
                if (chbwybz.Checked)
                    wybz = "1";
                else
                    wybz = "0";
                if (chbzcbz.Checked)
                    zcbz = "1";
                else
                    zcbz = "0";
            }
            catch (Exception ex)
            {
                MessageBox.Show("孕周数格式不正确，请确认!");
                return;
            }
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }


    }
}
