using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Srvtools;
using yb_interfaces;

namespace yb_interfaces
{
    public partial class Frm_dzXN : Form
    {
        public Frm_dzXN()
        {
            InitializeComponent();
        }

        public Frm_dzXN(DateTimePicker t)
        {
            time = t;
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {

            string strSQL = string.Format(@"select xm,jzlsh,jslsh as djh,yllb,ylfze,zbxje-yyfdfy as zbxje from ybfyjsdr where sysdate between '{0}' and '{1}' and cxbz = 1",
                                       time.Value.ToString("yyyy-MM-dd 00:00:00"),time.Value.ToString("yyyy-MM-dd 23:59:59"));

            DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSQL, CliUtils.fLoginDB.ToString(), true, CliUtils.fCurrentProject.ToString());

            //显示数据信息

            by_data.DataSource = ds.Tables[0];
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string strSQL = string.Format(@"select a.xm,jzlsh,djh,yllb,ylfze,zbxje,ybjzlsh,a.kh,a.grbh,a.dqbh,b.gmsfhm sfzh from ybfyjsdr a
                                                left join ybickxx b on a.grbh = b.grbh");

            DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSQL, CliUtils.fLoginDB.ToString(), true, CliUtils.fCurrentProject.ToString());

            //显示数据信息

            yb_data.DataSource = ds.Tables[0];
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string strSQL = string.Format(@"select xm,jzlsh,jslsh as djh,yllb,ylfze,zbxje-yyfdfy as zbxje from ybfyjsdr where sysdate between '{0}' and '{1}' and cxbz = 1
                                            and jslsh not in (select djh from ybdzlsb)",
                                      time.Value.ToString("yyyy-MM-dd 00:00:00"), time.Value.ToString("yyyy-MM-dd 23:59:59"));

            DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSQL, CliUtils.fLoginDB.ToString(), true, CliUtils.fCurrentProject.ToString());

            //显示数据信息

            by_data.DataSource = ds.Tables[0];
        }

        private void button4_Click(object sender, EventArgs e)
        {
            string strSQL = string.Format(@"select a.xm,jzlsh,djh,yllb,ylfze,zbxje,ybjzlsh,a.kh,a.grbh,a.dqbh,b.gmsfhm sfzh from ybdzlsb a
                                left join ybickxx b on a.grbh = b.grbh where djh not in
                (select jslsh from ybfyjsdr where sysdate between '{0}' and '{1}' and cxbz = 1)",
                                      time.Value.ToString("yyyy-MM-dd 00:00:00"), time.Value.ToString("yyyy-MM-dd 23:59:59"));

            DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSQL, CliUtils.fLoginDB.ToString(), true, CliUtils.fCurrentProject.ToString());

            //显示数据信息

            yb_data.DataSource = ds.Tables[0];
        }

        private void ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult dt = MessageBox.Show("确认撤销?","提示",MessageBoxButtons.OKCancel);
            if (dt == DialogResult.OK)
            {
                int a = yb_data.CurrentRow.Index;
                object[] obj = { yb_data.CurrentRow.Cells["yb_ybjzlsh"].Value.ToString(), yb_data.CurrentRow.Cells["yb_grbh"].Value.ToString(), yb_data.CurrentRow.Cells["yb_kh"].Value.ToString(), 
                                   yb_data.CurrentRow.Cells["yb_xm"].Value.ToString(), yb_data.CurrentRow.Cells["yb_ybjzlsh"].Value.ToString(), yb_data.CurrentRow.Cells["yb_djh"].Value.ToString(), 
                                   yb_data.CurrentRow.Cells["yb_dqbh"].Value.ToString() };
                if (yb_data.CurrentRow.Cells["yb_jzlsh"].Value.ToString().Length == 9)
                {
                    obj = yb_interface.ybs_interface("5703", obj);
                }
                else
                {
                    obj = yb_interface.ybs_interface("5702", obj);
                }
                

                MessageBox.Show(obj[2].ToString());
            }
        }


    }
}
