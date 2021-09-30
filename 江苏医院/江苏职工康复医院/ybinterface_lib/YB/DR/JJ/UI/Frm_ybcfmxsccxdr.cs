using Srvtools;
using System;
using System.Data;
using System.Windows.Forms;

namespace ybinterface_lib
{
    public partial class Frm_ybcfmxsccxdr : InfoForm
    {
        public Frm_ybcfmxsccxdr()
        {
            InitializeComponent();
            dgvcfmxscfh.AutoGenerateColumns = false;
            dgv_cfmxscin.AutoGenerateColumns = false;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
           
        }

        private void btntc_Click(object sender, EventArgs e)
        {
            this.Close();         
        }

        private void txtjzlsh_KeyPress(object sender, KeyPressEventArgs e)
        {
            //if ((int)e.KeyChar >= 65296 && e.KeyChar <= 65305)
            //{
            //    e.KeyChar = ybdjdr.QuanBianBan(e.KeyChar);
            //}

            //if (!(Char.IsNumber(e.KeyChar)) && e.KeyChar != (char)13 && e.KeyChar != (char)8)
            //{

            //    e.Handled = true;
            //}
        }

        private void txtjzlsh_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                string jzlsh = this.txtjzlsh.Text = txtjzlsh.Text.PadLeft(8, '0');
                string sql = string.Format(@"select xm, je, zlje, zfje, cxjzfje, case qezfbz when '0' then '否' else '是' end qezfbz
                , zlbl, xj, bz, yyxmdm, yyxmmc, case sfxmdj when '1' then '甲类' when '2' then '乙类' when '3' then '丙' else '自费' end sfxmdj
                from ybcfmxscfhdr a where a.cxbz = 1 and a.jzlsh = '{0}'", jzlsh);
                DataTable dt = CliUtils.ExecuteSql("sybdj", "cmd", sql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject).Tables[0];
                dgvcfmxscfh.DataSource = dt;
                sql = string.Format(@"select * from ybcfmxscindr a where a.cxbz = 1 and a.jzlsh = '{0}'", jzlsh);
                dt = CliUtils.ExecuteSql("sybdj", "cmd", sql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject).Tables[0];
                dgv_cfmxscin.DataSource = dt;
            }
        }
    }
}

