using Srvtools;
using System;
using System.Data;

namespace ybinterface_lib
{
    public partial class Frm_ybyjscxdr : InfoForm
    {
        public Frm_ybyjscxdr()
        {
            InitializeComponent();
            dgv_yjs.AutoGenerateColumns = false;
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void btncx_Click(object sender, EventArgs e)
        {
            string sql = string.Format("select * from ybfyyjsdr(nolock) a where jzlsh = '{0}' and cxbz = 1", txt_jzlsh.Text.Trim());
            DataTable dt = CliUtils.ExecuteSql("sybdj", "cmd", sql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject).Tables[0];

            if (dt != null && dt.Rows.Count > 0)
            {
                dgv_yjs.DataSource = dt;
            }
            else
            {
                dgv_yjs.DataSource = null;
            }
        }

        private void btn_Close_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}

