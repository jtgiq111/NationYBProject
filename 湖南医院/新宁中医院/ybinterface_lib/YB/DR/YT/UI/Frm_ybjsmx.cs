using Srvtools;
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
    public partial class Frm_ybjsmx : Form
    {
        private string jzlsh = string.Empty;
        public Frm_ybjsmx()
        {
            InitializeComponent();
        }

        public Frm_ybjsmx(string param)
        {
            InitializeComponent();
            jzlsh = param;
        }

        private void Frm_ybjsmx_Load(object sender, EventArgs e)
        {
            string strSql = string.Format(@"select * from ybfyyjsdr where jzlsh='{0}'", jzlsh);
            DataSet ds = CliUtils.ExecuteSql("shs01", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            if (ds.Tables[0].Rows.Count > 0)
            {
                DataRow dr = ds.Tables[0].Rows[0];
                txtylfze.Text = dr["ylfze"].ToString();
                txtzbxje.Text = dr["zbxje"].ToString();
                txttcjjzf.Text = dr["tcjjzf"].ToString();
                txtdejjzf.Text = dr["dejjzf"].ToString();
                txtzhzf.Text = dr["zhzf"].ToString();
                txtxjzf.Text = dr["xjzf"].ToString();
                txtgwybz.Text = dr["gwybzjjzf"].ToString();
                txtqybc.Text = dr["qybcylbxjjzf"].ToString();
                txtzffy.Text = dr["zffy"].ToString();
                txtdwfd.Text = dr["dwfdfy"].ToString();
                txtyyfd.Text = dr["yyfdfy"].ToString();
                txtmzjz.Text = dr["mzjzfy"].ToString();
                txtcxjfy.Text = dr["cxjfy"].ToString();
                txtfhjbyl.Text = dr["fhjbylfy"].ToString();
                txtqfbz.Text = dr["qfbzfy"].ToString();
                txtjrtc.Text = dr["jrtcfy"].ToString();
                txtjrde.Text = dr["jrdebxfy"].ToString();
                txttclj.Text = dr["bntczflj"].ToString();
                txtdelj.Text = dr["bndezflj"].ToString();
                txtecbc.Text = dr["ecbcje"].ToString();
                txtdbzbc.Text = dr["dbzbc"].ToString();
                txtjmecbc.Text = dr["jmecbc"].ToString();
                txtrgqg.Text = dr["rgqgzffy"].ToString();
                txtsyjj.Text = dr["syjjzf"].ToString();
                txtjmdbyd.Text = dr["jjjmdbydje"].ToString();
                txtjmdbed.Text = dr["jjjmdbedje"].ToString();
                txtjbbcfwn.Text = dr["jjjmjbbcfwnje"].ToString();
                txtjbbcfwy.Text = dr["jjjmjbbcfwwje"].ToString();
                txtmgwxlc.Text = dr["mgwxlcjjzf"].ToString();
                txtzftd.Text = dr["zftdjjzf"].ToString();
                txtjddbyd.Text = dr["jddbbcbcydzfje"].ToString();
                txtjddbed.Text = dr["jddbbcbcedzfje"].ToString();
                txtjddbsd.Text = dr["jddbbcbcsdzfje"].ToString();
                txtjdbcyd.Text = dr["jdecbcbcydzfje"].ToString();
                txtjdbced.Text = dr["jdecbcbcedzfje"].ToString();
                txtjdbcsd.Text = dr["jdecbcbcsdzfje"].ToString();
                txtfwnyd.Text = dr["jbbcbxbczcfwnfyydzfje"].ToString();
                txtfwned.Text = dr["jbbcbxbczcfwnfyedzfje"].ToString();
                txtjzgb.Text = dr["jzgbjjzf"].ToString();
            }
        }

        private void btnCLOSE_Click(object sender, EventArgs e)
        {
            this.Close();
        }

    }
}
