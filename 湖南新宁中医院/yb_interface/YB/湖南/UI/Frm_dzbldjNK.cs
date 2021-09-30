using Srvtools;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace yb_interfaces.YB.湖南.UI
{
    public partial class Frm_dzbldjNK : InfoForm
    {
        public Frm_dzbldjNK()
        {
            InitializeComponent();
        }

        private void btnQUIT_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnDJ_Click(object sender, EventArgs e)
        {
                    //object[] back_mess, param = new object[] { txtZyno.Text,"0" };
                    //back_mess = yb_interface.ybs_interface("4600", param);
                    object[] objParam1 = { txtZyno.Text, "0" };
                    object[] obdj =yb_interface_hn_nkNew.YBZYDJ(objParam1);
                    if (obdj[1].ToString() == "1")
                    {
                        MessageBox.Show("[" + txtZyno.Text + "]病案登记成功", "提示");
                        labXM.Text = "";//姓名
                        labKH.Text = "";//卡号
                        labLSH.Text = "";//流水号
                        labBXH.Text = "";//保险号

                    }
                    else 
                    {
                        MessageBox.Show("[" + txtZyno.Text + "]病案登记失败" + obdj[2].ToString(), "提示");
                    }
        }

        private void txtZyno_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                string strSQL = string.Format(@"select grbh,xm,kh,tcqh,jzlsh,z1idno,z1bahx,z1mobi,z1bzno,z1bznm,jylsh from ybmzzydjdr 
            left join zy01h on z1zyno=jzlsh where cxbz=1 and jzlsh='{0}'",txtZyno.Text);
            DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSQL, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            if (ds.Tables[0].Rows.Count > 0)
            {
                labXM.Text = ds.Tables[0].Rows[0]["xm"].ToString();//姓名
                labKH.Text = ds.Tables[0].Rows[0]["kh"].ToString();//卡号
                labLSH.Text = ds.Tables[0].Rows[0]["jylsh"].ToString();//流水号
                labBXH.Text = ds.Tables[0].Rows[0]["grbh"].ToString();//保险号
            }
            else 
            {
                MessageBox.Show("该病人未进行医保登记");
                return;
            }
            
            }
        }

        private void btnCX_Click(object sender, EventArgs e)
        {
            //object[] back_mess, param = new object[] { txtZyno.Text, "2" };
            //back_mess = yb_interface.ybs_interface("4600", param);
            object[] objParam1 = { txtZyno.Text, "2" };
            object[] obdj = yb_interface_hn_nkNew.YBDZBLDJ(objParam1);
            if (obdj[1].ToString() == "1")
            {
                MessageBox.Show("[" + txtZyno.Text + "]病案撤销成功", "提示");
                labXM.Text = "";//姓名
                labKH.Text = "";//卡号
                labLSH.Text = "";//流水号
                labBXH.Text = "";//保险号

            }
            else
            {
                MessageBox.Show("[" + txtZyno.Text + "]病案撤销失败" + obdj[2].ToString(), "提示");
            }
        }
    }
}
