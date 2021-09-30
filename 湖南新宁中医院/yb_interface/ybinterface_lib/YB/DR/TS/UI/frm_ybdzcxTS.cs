using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Srvtools;
using ybinterface_lib;

namespace ybinterface_lib
{
    public partial class frm_ybdzcxTS : InfoForm
    {
        private string ywCode = string.Empty;

        public frm_ybdzcxTS()
        {
            InitializeComponent();
        }

        private void frm_ybdzcxTS_Load(object sender, EventArgs e)
        {
            dtp_Start.Text = DateTime.Now.ToString("yyyy-MM-dd");
            dtp_End.Text = DateTime.Now.ToString("yyyy-MM-dd");

        }

        private void btnZEDZ_Click(object sender, EventArgs e)
        {
            string dtStart = dtp_Start.Text;
            string dtEnd = dtp_End.Text;
            string ywzqh = txtYWZQH.Text.Trim();
            ywCode="";

            object[] objParam = { dtStart, dtEnd, ywzqh };
            //object[] objReturn=yb_interface.ybs_interface(
        }

        private void btnFYMXXZ_Click(object sender, EventArgs e)
        {

        }

        private void btnMXDZ_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            int days = (dtp_End.Value - dtp_Start.Value).Days;
            if (days > 31)
            {
                AutoListView.MyTip.ShowMsg("查询结束日期与查询开始日期之间的差超过31天!");
                return;
            }
            string dtStart = dtp_Start.Text;
            string dtEnd = dtp_End.Text;
            object[] objParam = { dtStart, dtEnd };
            object[] objReturn = yb_interface.ybs_interface("5104", objParam);
            if (objReturn[1].ToString().Trim().Equals("1"))
            {
                AutoListView.MyTip.ShowMsg(objReturn[2].ToString());
                return;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtJZLSH.Text.Trim()))
            {
                AutoListView.MyTip.ShowMsg("请输入要冲正的住院号或门诊流水号!");
                return;
            }
            object[] objParam = { txtJZLSH.Text.Trim(), txtDJH.Text.Trim() };
            object[] objReturn = yb_interface.ybs_interface("4500", objParam);
            if (objReturn[1].ToString().Trim().Equals("1"))
            {
                AutoListView.MyTip.ShowMsg(objReturn[2].ToString());
                return;
            }
        }

        private void txtJZLSH_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
            {
                string strSql = string.Format(@"select jylsh from ybmzzydjdr where jzlsh='{0}' and cxbz='1' ", txtJZLSH.Text);
                DataSet ds = CliUtils.ExecuteSql("sybdjdr", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                if (ds.Tables[0].Rows.Count == 0)
                {
                    AutoListView.MyTip.ShowMsg("该患者不存在结算信息!");
                    return;
                }
                txtJZLSH.Text = ds.Tables[0].Rows[0]["jylsh"].ToString();
            }
        }
    }
}
