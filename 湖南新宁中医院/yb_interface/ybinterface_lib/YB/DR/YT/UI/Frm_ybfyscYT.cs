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
    public partial class Frm_ybfyscYT : InfoForm
    {
        public Frm_ybfyscYT()
        {
            InitializeComponent();
        }

        private void btnHZXX_Click(object sender, EventArgs e)
        {
            infoDataGridView1.DataSource = null;
            infoDataGridView2.DataSource = null;

            string sql = string.Format(@"select a.z1zyno, grbh bxh, z1hznm, a.z1ksno, a.z1ksnm, z1amt2
                                                ,sum(z3amnt * (case left(z3endv, 1) when '4' then -1 else 1 end)) amt2
                                                ,z1amt1, z1amt3, xm, jylsh, kh, max(z3kdys) z3kdys,bzbm,bzmc,dgysdm,dgysxm 
                                                from zy01h a join ybmzzydjdr on z1zyno = jzlsh and cxbz = 1
                                                left join zy03d on z1zyno = z3zyno and z1ghno = z3ghno and z1comp = z3comp 
                                                inner join zy01d e on e.z1zyno=a.z1zyno and left(e.z1endv,1)=8
                                                where left(z3kind, 1) in ('2', '4') and jzlsh not in(select jzlsh from ybfyjsdr where cxbz=1) 
                                                group by a.z1zyno, z1hznm, a.z1ksno, a.z1ksnm, z1amt1, z1amt2, z1amt3, xm, grbh, jylsh, kh,bzbm,bzmc,dgysdm,dgysxm 
                                                having sum(z3amnt * (case left(z3endv, 1) when '4' then -1 else 1 end)) > 0");
            DataSet ds = CliUtils.ExecuteSql("shs01", "cmd", sql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            infoDataGridView1.AutoGenerateColumns = false;
            infoDataGridView1.DataSource = ds.Tables[0];
            infoDataGridView1.ClearSelection();
        }

        private void infoDataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex != -1)
            {
                string jzlsh = infoDataGridView1.Rows[e.RowIndex].Cells["zyno"].Value.ToString();
                txtHZXM.Text = infoDataGridView1.Rows[e.RowIndex].Cells["z1hznm"].Value.ToString();
                txtKSMC.Text = infoDataGridView1.Rows[e.RowIndex].Cells["ksnm"].Value.ToString();
                txtYLFY.Text = infoDataGridView1.Rows[e.RowIndex].Cells["Tfee"].Value.ToString();
                GetPatFee(jzlsh);
            }
        }

        internal void GetPatFee(string zyno)
        {
            string sql = string.Format(@"select b.yysfxmbm as yysfxmbm,b.yysfxmmc as yysfxmmc,b.sfxmzxbm as sfxmzxbm,b.sfxmzxmc as ybxmmc,b.dj,b.sl,b.je,d.name as sflb,c.name as sfxmdj 
                                        from ybcfmxscindr b
										left join YBXMLBZD c on c.LBMC='收费项目等级' and c.QYBZ=1 and c.CODE=b.sfxmdj
										left join YBXMLBZD d on d.LBMC='收费类别' and d.QYBZ=1 and d.CODE=b.sflb
                                        where b.jzlsh = '{0}' and b.cxbz=1", zyno);
            DataSet ds = CliUtils.ExecuteSql("shs01", "cmd", sql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);

             sql = string.Format(@"select SUM(je) as ybfy from ybcfmxscindr
                                        where jzlsh = '{0}' and cxbz=1", zyno);
            DataSet ds1 = CliUtils.ExecuteSql("shs01", "cmd", sql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            txtYBFY.Text = ds1.Tables[0].Rows[0]["ybfy"].ToString();
            infoDataGridView2.AutoGenerateColumns = false;
            infoDataGridView2.DataSource = ds.Tables[0];
            infoDataGridView2.ClearSelection();
          
        }

        private void btnCLOSE_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        //单个患者费用上传
        private void btnFYSC_Click(object sender, EventArgs e)
        {
            string scRow = cmbMCROW.Text;
            string sCfrqbz = "0";
            if (!chkCSRQBZ.Checked) //不选中 处方日期及出院日期代替
                sCfrqbz = "1";

            if (infoDataGridView1.SelectedRows.Count !=0 )
            {
                string zyno = infoDataGridView1.CurrentRow.Cells["zyno"].Value.ToString();
                DialogResult dr = MessageBox.Show("确认上传病人[" + zyno + "]费用？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (dr == DialogResult.Yes)
                {
                    object[] back_mess, param = new object[] { zyno, "", scRow, sCfrqbz };
                    back_mess = yb_interface.ybs_interface("4300", param);

                    if (Convert.ToInt32(back_mess[1].ToString()) == 1)
                    {
                        MessageBox.Show("[" + zyno + "]所有费用上传成功", "提示");
                        btnHZXX_Click(null, null);
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

        private void btnSFSCALL_Click(object sender, EventArgs e)
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
                        back_mess = yb_interface.ybs_interface("4300", param);

                        if (back_mess[1].ToString() != "1")
                        {
                            errorCount++;
                            errorMess += infoDataGridView1.Rows[i].Cells["zyno"].Value + ":" + back_mess[2].ToString() + ",";
                        }
                    }

                    if (errorCount == 0)
                    {
                        MessageBox.Show("所有病人费用上传成功", "提示");
                        btnHZXX_Click(null, null);
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

        private void btnSFCX_Click(object sender, EventArgs e)
        {
            if (infoDataGridView1.SelectedRows.Count != 0)
            {
                string zyno = infoDataGridView1.CurrentRow.Cells["zyno"].Value.ToString();
                DialogResult dr = MessageBox.Show("是否撤销已上传费用明细？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (dr == DialogResult.No)
                    return;

                object[] obj = { zyno };
                obj = yb_interface.ybs_interface("4301", obj);

                if (obj[1].ToString() == "1")
                {
                    MessageBox.Show("撤销费用明细成功", "提示");
                    btnHZXX_Click(null, null);
                    GetPatFee(zyno);
                }
                else
                    MessageBox.Show(obj[2].ToString(), "提示");
            }
            else
            {
                MessageBox.Show("请选中一个病人", "提示");
            }
        }

        private void txtZYNO_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (!string.IsNullOrWhiteSpace(txtZYNO.Text))
                {
                    txtZYNO.Text = txtZYNO.Text.PadLeft(8, '0');

                    string sql = string.Format(@"select a.z1zyno, grbh bxh, z1hznm, a.z1ksno, a.z1ksnm, z1amt2
                                                ,sum(z3amnt * (case left(z3endv, 1) when '4' then -1 else 1 end)) amt2
                                                ,z1amt1, z1amt3, xm, jylsh, kh, max(z3kdys) z3kdys,bzbm,bzmc,dgysdm,dgysxm 
                                                from zy01h a join ybmzzydjdr on z1zyno = jzlsh and cxbz = 1
                                                left join zy03d on z1zyno = z3zyno and z1ghno = z3ghno and z1comp = z3comp 
                                                inner join zy01d e on e.z1zyno=a.z1zyno and left(e.z1endv,1)=8
                                                where left(z3kind, 1) in ('2', '4') and jzlsh not in(select jzlsh from ybfyjsdr where cxbz=1) and a.z1zyno='{0}'
                                                group by a.z1zyno, z1hznm, a.z1ksno, a.z1ksnm, z1amt1, z1amt2, z1amt3, xm, grbh, jylsh, kh,bzbm,bzmc,dgysdm,dgysxm 
                                                having sum(z3amnt * (case left(z3endv, 1) when '4' then -1 else 1 end)) > 0", txtZYNO.Text.Trim());
                    DataSet ds = CliUtils.ExecuteSql("shs01", "cmd", sql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                    if (ds.Tables[0].Rows.Count == 0)
                    {
                        MessageBox.Show("该患者信息未做医保登记或未拖出科室");
                    }
                    infoDataGridView1.AutoGenerateColumns = false;
                    infoDataGridView1.DataSource = ds.Tables[0];
                    infoDataGridView1.ClearSelection();
                }
            }
        }

        private void Frm_ybfyscYT_Load(object sender, EventArgs e)
        {

        }






    }
}
