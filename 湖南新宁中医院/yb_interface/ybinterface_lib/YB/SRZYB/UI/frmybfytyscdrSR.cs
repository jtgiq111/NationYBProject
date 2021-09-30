using Srvtools;
using System;
using System.Data;
using System.Windows.Forms;

namespace ybinterface_lib
{
    public partial class frmybfytyscdrSR : InfoForm
    {
        public frmybfytyscdrSR()
        {
            InitializeComponent();
        }

        private void btn_sjhz_Click(object sender, EventArgs e)
        {
//            string sql = string.Format(@"select z1zyno, grbh bxh, z1hznm, z1ksno, z1ksnm, z1amt2
//            , sum(z3amnt * (case left(z3endv, 1) when '4' then -1 else 1 end)) amt2
//            , z1amt1, z1amt3, xm, jylsh, kh, max(z3kdys) z3kdys 
//            from zy01h join ybmzzydjdr on z1zyno = jzlsh and cxbz = 1
//            left join zy03d on z1zyno = z3zyno and z1ghno = z3ghno and z1comp = z3comp 
//            where left(z1endv ,1) = '0' and jzlsh is not null and left(z3kind, 1) in ('2', '4') 
//            and isnull(z3ybup, '') = '' and isnull(z3fphx, '') = ''
//            group by z1zyno, z1hznm, z1ksno, z1ksnm, z1amt1, z1amt2, z1amt3, xm, grbh, jylsh, kh 
//            having sum(z3amnt * (case left(z3endv, 1) when '4' then -1 else 1 end)) > 0");
            string sql = string.Format(@"select z1zyno, grbh bxh, z1hznm, z1ksno, z1ksnm, z1amt2
            , sum(z3amnt * (case left(z3endv, 1) when '4' then -1 else 1 end)) amt2
            , z1amt1, z1amt3, xm, jylsh, kh, max(z3kdys) z3kdys 
            from zy01h join ybmzzydjdr on z1zyno = jzlsh and cxbz = 1
            left join zy03d on z1zyno = z3zyno and z1ghno = z3ghno and z1comp = z3comp 
            where jzlsh is not null and left(z3kind, 1) in ('2', '4') 
            and isnull(z3ybup, '') = '' 
            group by z1zyno, z1hznm, z1ksno, z1ksnm, z1amt1, z1amt2, z1amt3, xm, grbh, jylsh, kh 
            having sum(z3amnt * (case left(z3endv, 1) when '4' then -1 else 1 end)) > 0");
            DataSet ds = CliUtils.ExecuteSql("shs01", "cmd", sql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            infoDataGridView1.DataSource = ds.Tables[0];
        }

        private void btn_uploadallfee_Click(object sender, EventArgs e)
        {
            if (infoDataGridView1.Rows.Count > 0)
            {
                DialogResult dr = MessageBox.Show("确认上传所有病人费用？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (dr == DialogResult.Yes)
                {
                    string errorMess = "";
                    int errorCount = 0;

                    for (int i = 0; i < infoDataGridView1.Rows.Count; i++)
                    {
                        object[] back_mess, param = new object[] { infoDataGridView1.Rows[i].Cells["zyno"].Value };
                        back_mess = yb_interface.ybs_interface("4300",param);

                        if (back_mess[1].ToString() != "1")
                        {
                            errorCount++;
                            errorMess += infoDataGridView1.Rows[i].Cells["zyno"].Value + ":" + back_mess[2].ToString() + ",";
                        }
                    }

                    if (errorCount == 0)
                    {
                        MessageBox.Show("所有病人费用上传成功", "提示");
                        btn_sjhz_Click(null, null);
                    }
                    else
                    {
                        MessageBox.Show("有" + errorCount.ToString() + "个病人的费用上传失败:" + errorMess.TrimEnd(','), "提示");
                    }
                }
            }
            else
            {
                MessageBox.Show("没有需要上传的费用信息", "提示");
            }
        }

        private void btn_close_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void GetPatFee(string zyno)
        {
            string sql = string.Format(@"SELECT je, zlje, zfje, cxjzfje, sflb, sfxmdj, qezfbz, zlbl, xj, bz, yyxmdm, yyxmmc
                                        FROM ybcfmxscfhdr where jzlsh = '{0}' and isnull(jsdjh, '') = '' and cxbz = 1", zyno);
            DataSet ds = CliUtils.ExecuteSql("shs01", "cmd", sql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);

            if (ds.Tables[0].Rows.Count > 0)
            {
                infoDataGridView2.DataSource = ds.Tables[0];
            }
            else
            {
                infoDataGridView2.DataSource = null;
            }
        }

        private void btn_uploadonefee_Click(object sender, EventArgs e)
        {
            if (infoDataGridView1.CurrentRow != null && infoDataGridView1.CurrentRow.Index != -1)
            {
                string zyno = infoDataGridView1.CurrentRow.Cells["zyno"].Value.ToString();
                DialogResult dr = MessageBox.Show("确认上传病人[" + zyno + "]费用？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (dr == DialogResult.Yes)
                {
                    object[] back_mess, param = new object[] { zyno };
                    back_mess = yb_interface.ybs_interface("4300", param);

                    if (Convert.ToInt32(back_mess[1].ToString()) == 1)
                    {
                        MessageBox.Show("[" + zyno + "]所有费用上传成功", "提示");
                        btn_sjhz_Click(null, null);
                        GetPatFee(zyno);
                    }
                    else
                    {
                        MessageBox.Show("[" + zyno + "]费用上传失败:" + back_mess[2].ToString(), "提示");
                    }
                }
            }
            else
            {
                MessageBox.Show("请选中一个病人", "提示");
            }
        }

        private void infoDataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex != -1)
            {
                GetPatFee(infoDataGridView1.Rows[e.RowIndex].Cells["zyno"].Value.ToString());
            }
        }
    }
}
