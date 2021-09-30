using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Srvtools; 

namespace yb_interfaces
{
    public partial class Frm_ybfyscXN : InfoForm
    {
        public Frm_ybfyscXN()
        {
            InitializeComponent();
        }

        private void btnHZXX_Click(object sender, EventArgs e)
        {
            infoDataGridView1.DataSource = null;
            infoDataGridView2.DataSource = null;

            string sql = string.Format(@"with tmp as
                                        (
                                        select a.z3zyno,a.z3ghno,a.z3comp,sum(a.z3amnt * (case left(a.z3endv, 1) when '4' then -1 else 1 end)) amt2,
                                        max(a.z3kdys) z3kdys
                                        from zy03d  a
                                        group by a.z3zyno,a.z3ghno,a.z3comp
                                        union all
                                        select a.z3zyno,a.z3ghno,a.z3comp,sum(a.z3amnt * (case left(a.z3endv, 1) when '4' then -1 else 1 end)) amt2,
                                        max(a.z3kdys) z3kdys
                                        from zy03dz  a
                                        group by a.z3zyno,a.z3ghno,a.z3comp
                                        )
                                        select z1zyno, grbh bxh, z1hznm, z1ksno, z1ksnm, 
                                        sum(b.amt2) as z1amt2, 0.00 as amt2,
                                        z1amt1, z1amt3, xm, jylsh, kh, max(b.z3kdys) z3kdys,bzbm,bzmc,dgysdm,dgysxm 
                                        from zy01h join ybmzzydjdr on z1zyno = jzlsh and cxbz = 1
                                        left join tmp b on z1zyno = b.z3zyno and z1ghno = b.z3ghno and z1comp = b.z3comp 
                                        group by z1zyno, z1hznm, z1ksno, z1ksnm, z1amt1, z1amt2, z1amt3, xm, grbh, jylsh, kh,bzbm,bzmc,dgysdm,dgysxm ");
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
            string sql = string.Format(@"select yysfxmbm,yysfxmmc,a.sfxmzxmc,sfxmzxbm,a.dj,sl,je,b.sflb,b.sfxmdj from ybcfmxscindr  a
                                        left join ybhisdzdrnew b on sfxmzxbm=ybxmbh and yysfxmbm=hisxmbh
                                        where jzlsh = '{0}' and cxbz=1", zyno);
            DataSet ds = CliUtils.ExecuteSql("shs01", "cmd", sql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);

            if (ds.Tables[0].Rows.Count == 0)
            {
//                sql = string.Format(@"select '' sfxmzxbm,'' sfxmzxmc,a.z3djxx as dj,case LEFT(a.z3endv,1) when '4' then -a.z3jzcs else a.z3jzcs end as sl,
//                                    case LEFT(a.z3endv,1) when '4' then -a.z3jzje else a.z3jzje end as je,a.z3item as yysfxmbm, a.z3name as yysfxmmc,
//                                    b.sflb,b.sfxmdj
//                                    from zy03d a left join ybhisdzdrnew b on 
//                                    a.z3item=b.hisxmbh where a.z3ybup is null and LEFT(a.z3kind,1)in(2,4)  and a.z3zyno='{0}' 
//                                    union all
//                                    select '' sfxmzxbm,'' sfxmzxmc,a.z3djxx as dj,case LEFT(a.z3endv,1) when '4' then -a.z3jzcs else a.z3jzcs end as sl,
//                                    case LEFT(a.z3endv,1) when '4' then -a.z3jzje else a.z3jzje end as je,a.z3item as yysfxmbm, a.z3name as yysfxmmc,
//                                    b.sflb,b.sfxmdj
//                                    from zy03dz a left join ybhisdzdrnew b on 
//                                    a.z3item=b.hisxmbh where a.z3ybup is null and LEFT(a.z3kind,1)in(2,4)  and a.z3zyno='{0}'", zyno);

                sql = string.Format(@"select ybxmbh sfxmzxbm,ybxmmc sfxmzxmc,a.z3djxx as dj,case LEFT(a.z3endv,1) when '4' then -a.z3jzcs else a.z3jzcs end as sl,
                                    case LEFT(a.z3endv,1) when '4' then -a.z3jzje else a.z3jzje end as je,a.z3item as yysfxmbm, a.z3name as yysfxmmc,
                                    b.sflb,b.sfxmdj
                                    from zy03d a left join ybhisdzdrnew b on 
                                    a.z3item=b.hisxmbh where a.z3ybup is null and LEFT(a.z3kind,1)in(2,4)  and a.z3zyno='{0}' 
                                    union all
                                    select ybxmbh sfxmzxbm,ybxmmc sfxmzxmc,a.z3djxx as dj,case LEFT(a.z3endv,1) when '4' then -a.z3jzcs else a.z3jzcs end as sl,
                                    case LEFT(a.z3endv,1) when '4' then -a.z3jzje else a.z3jzje end as je,a.z3item as yysfxmbm, a.z3name as yysfxmmc,
                                    b.sflb,b.sfxmdj
                                    from zy03dz a left join ybhisdzdrnew b on 
                                    a.z3item=b.hisxmbh where a.z3ybup is null and LEFT(a.z3kind,1)in(2,4)  and a.z3zyno='{0}'", zyno);
            }
            ds.Tables.Clear();
            ds = CliUtils.ExecuteSql("shs01", "cmd", sql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);

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
            if (infoDataGridView1.SelectedRows.Count !=0 )
            {
                string zyno = infoDataGridView1.CurrentRow.Cells["zyno"].Value.ToString();
               // string sfje = infoDataGridView1.CurrentRow.Cells["Tfee"].Value.ToString();
                
                DialogResult dr = MessageBox.Show("确认上传病人[" + zyno + "]费用？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                //string strSql = string.Format(@"select z1outd from zy01d where z1endv like '8%' and z1zyno='{0}'",zyno);
                //DataSet ds = CliUtils.ExecuteSql("shs01", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                //string cyrq = string.IsNullOrEmpty(ds.Tables[0].Rows[0]["z1outd"].ToString()) ? DateTime.Now.ToString("yyyyMMddHHmm") : DateTime.Parse(ds.Tables[0].Rows[0]["z1outd"].ToString()).ToString("yyyyMMddHHmm");
               
                if (dr == DialogResult.Yes)
                {
                    string jsdate = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    object[] back_mess, param = new object[] { zyno,jsdate,txtSCSL.Text };
                    back_mess = yb_interface_hn_nkNew.YBZYSFDJ(param);

                    if (Convert.ToInt32(back_mess[1].ToString()) == 1)
                    {
                        MessageBox.Show("[" + zyno + "]所有费用上传成功", "提示");
                        //object[] back_mess1, param1 = new object[] 
                        //{ zyno, 9, 1, 0, DateTime.Now.ToString("yyyyMMdd"),cyrq,sfje };
                        //back_mess1 = yb_interface.ybs_interface("4400", param1);
                        //if (Convert.ToInt32(back_mess[1].ToString()) == 1)
                        //{ 
                           
                        //}
                       // btnHZXX_Click(null, null);
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
                        back_mess = Functions.ybs_interface("4300", param);

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
                  string strSQL = string.Format(@" select ybjzlsh,grbh,xm,kh,z1tcdq,jzlsh,z1idno,z1bahx,z1mobi,z1bzno,z1bznm from ybfyjsdr 
            left join zy01h on z1zyno=jzlsh where cxbz=1 and jzlsh='{0}' and bascbz='1'", zyno);
            DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSQL, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            if (ds.Tables[0].Rows.Count > 0)
            { 
                //医保电子病历登记撤销
                object[] back_mess, param = new object[] { zyno, "2" };
                back_mess = yb_interface_hn_nkNew.YBZYSFDJCX(param); ;
                if (back_mess[1].ToString() != "1")
                {
                    MessageBox.Show("医保电子病历登记撤销失败,请先手动撤销，再进行费用明细撤销！");
                    return;
                }

            }

              
                //费用明细撤销
                object[] obj = { zyno };
                obj = yb_interface_hn_nkNew.YBZYSFDJCX(obj);

                if (obj[1].ToString() == "1")
                {
                    MessageBox.Show("撤销费用明细成功", "提示");
                   
                    //btnHZXX_Click(null, null);
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
                    string sqlck = string.Format(@"SELECT 1
                    FROM dbo.zy01h h
                       INNER JOIN dbo.zy01d d ON(d.z1comp=h.z1comp AND d.z1zyno = h.z1zyno AND d.z1ghno = h.z1ghno)
                    WHERE h.z1zyno='{0}' AND h.z1endv LIKE '0%' AND d.z1endv LIKE '8%'",txtZYNO.Text);
                    DataSet dsck = CliUtils.ExecuteSql("shs01", "cmd", sqlck, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                    if (dsck.Tables[0].Rows.Count <= 0) 
                    {
                        MessageBox.Show("该患者未出科或已结算");
                        return;
                    }
                    
                    string sql = string.Format(@"select z1zyno, grbh bxh, z1hznm, z1ksno, z1ksnm, z1amt2
                                        , sum(z3amnt * (case left(z3endv, 1) when '4' then -1 else 1 end)) amt2
                                        , z1amt1, z1amt3, xm, jylsh, kh, max(z3kdys) z3kdys,bzbm,bzmc,dgysdm,dgysxm 
                                        from zy01h join ybmzzydjdr on z1zyno = jzlsh and cxbz = 1
                                        left join zy03d on z1zyno = z3zyno and z1ghno = z3ghno and z1comp = z3comp 
                                        where left(z3kind, 1) in ('2', '4') and jzlsh not in(select jzlsh from ybfyjsdr where cxbz=1) and z1zyno='{0}'
                                        group by z1zyno, z1hznm, z1ksno, z1ksnm, z1amt1, z1amt2, z1amt3, xm, grbh, jylsh, kh,bzbm,bzmc,dgysdm,dgysxm 
                                        having sum(z3amnt * (case left(z3endv, 1) when '4' then -1 else 1 end)) > 0", txtZYNO.Text.Trim());
                    DataSet ds = CliUtils.ExecuteSql("shs01", "cmd", sql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                    if (ds.Tables[0].Rows.Count == 0)
                    {
                        MessageBox.Show("无索引到该患者信息");
                    }
                    infoDataGridView1.AutoGenerateColumns = false;
                    infoDataGridView1.DataSource = ds.Tables[0];
                    infoDataGridView1.ClearSelection();
                }
            }
        }

        private void txtSCTS_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(Char.IsNumber(e.KeyChar)) && e.KeyChar != (char)13 && e.KeyChar != (char)8)
            {
                e.Handled = true;
            }
        }

        private void Frm_ybfyscXG_Load(object sender, EventArgs e)
        {
            txtSCSL.Text = "50";
        }


       



    }
}
