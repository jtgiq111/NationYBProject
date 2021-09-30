using Srvtools;
using System;
using System.Data;
using System.Windows.Forms;

namespace ybinterface_lib
{
    public partial class Frm_ybzyfyscdr : InfoForm
    {
        string xmbhs = "";

        public Frm_ybzyfyscdr()
        {
            InitializeComponent();
        }

        private void btn_sjhz_Click(object sender, EventArgs e)
        {
            xmbhs = "";
            string sql = @"select z1zyno, grbh bxh, z1hznm, z1ksno, z1ksnm, z1amt2
            , sum(z3amnt * (case left(z3endv, 1) when '4' then -1 else 1 end)) amt2
            , z1amt1, z1amt3, xm, jylsh, kh, min(z3kdys) z3kdys 
            from zy01h join ybmzzydjdr on z1zyno = jzlsh and cxbz = 1
            join zy03d on z1zyno = z3zyno and z1ghno = z3ghno and z1comp = z3comp 
            where left(z1endv ,1) = '0' and left(z3kind, 1) in ('2')  
            and isnull(z3ybup, '') = '' and isnull(z3jshx, '') = ''";

            if (!string.IsNullOrWhiteSpace(txt_zyh.Text))
            {
                string zyh = txt_zyh.Text = txt_zyh.Text.Trim().PadLeft(8, '0');
                sql += string.Format(" and z3zyno = '{0}'", zyh);
            }

            sql += @" group by z1zyno, z1hznm, z1ksno, z1ksnm, z1amt1, z1amt2, z1amt3, xm, grbh, jylsh, kh 
            having sum(case left(z3endv, 1) when '4' then -z3jzcs else z3jzcs end) > 0 and sum(case left(z3endv, 1) when '4' then -z3jzje else z3jzje end) > 0";
            DataSet ds = CliUtils.ExecuteSql("shs01", "cmd", sql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            infoDataGridView1.DataSource = ds.Tables[0];
            btn_cltf.Visible = false;
            btn_cltf2018.Visible = false;
            chk_jd.Checked = false;
        }

        private void btn_uploadallfee_Click(object sender, EventArgs e)
        {
            xmbhs = "";
            btn_cltf.Visible = false;
            btn_cltf2018.Visible = false;

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
                        back_mess = yb_interface.ybs_interface("5013",param);

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

            chk_jd.Checked = false;
        }

        private void GetPatFee(string zyno)
        {
            string sql = string.Format(@"SELECT je, zlje, zfje, cxjzfje, sflb, sfxmdj, qezfbz, zlbl, xj, bz, yyxmdm, yyxmmc
                                        FROM ybcfmxscfhdr where jzlsh = '{0}' and isnull(jsdjh, '') = '' and cxbz = 1
                                        group by je, zlje, zfje, cxjzfje, sflb, sfxmdj, qezfbz, zlbl, xj, bz, yyxmdm, yyxmmc", zyno);
            DataSet ds = CliUtils.ExecuteSql("shs01", "cmd", sql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);

            if (ds.Tables[0].Rows.Count > 0)
            {
                infoDataGridView2.DataSource = ds.Tables[0];
                sql = string.Format(@"select b.xm, sum(je) zfy
                                        FROM ybcfmxscfhdr a join ybmzzydjdr b on a.jzlsh = b.jzlsh where a.jzlsh = '{0}' and isnull(a.jsdjh, '') = '' and a.cxbz = 1 and b.cxbz = 1
                                        group by b.xm", zyno);
                ds = CliUtils.ExecuteSql("shs01", "cmd", sql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    DataRow dr = ds.Tables[0].Rows[0];
                    lbl_zfy.Text = dr["zfy"].ToString();
                }
            }
            else
            {
                infoDataGridView2.DataSource = null;
            }
        }

        private void btn_uploadonefee_Click(object sender, EventArgs e)
        {
            xmbhs = "";

            if (infoDataGridView1.CurrentRow != null && infoDataGridView1.CurrentRow.Index != -1)
            {
                string zyno = infoDataGridView1.CurrentRow.Cells["zyno"].Value.ToString();
                DialogResult dr = MessageBox.Show("确认上传病人[" + zyno + "]费用？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (dr == DialogResult.Yes)
                {
                    object[] back_mess, param = new object[] { zyno };

                    if (chk_jd.Checked)
                    {
                        param = new object[] { zyno, "", dtp_jdrq.Text };
                    }

                    back_mess = yb_interface.ybs_interface("4300",param);

                    if (Convert.ToInt32(back_mess[1].ToString()) == 1)
                    {
                        MessageBox.Show("[" + zyno + "]所有费用上传成功", "提示");
                        btn_sjhz_Click(null, null);
                        GetPatFee(zyno);
                    }
                    else
                    {
                        string mes = "";

                        if (back_mess[2].ToString().Contains("12月30号"))
                        {
                            MessageBox.Show(back_mess[2].ToString());
                            xmbhs = back_mess[3].ToString();
                            //xmbhs = back_mess[2].ToString().Substring(back_mess[2].ToString().IndexOf("(##") + 3, back_mess[2].ToString().IndexOf("##)") - back_mess[2].ToString().IndexOf("(##") - 3);
                            mes = "；请执行右上角的处理退费按钮";
                            btn_cltf.Visible = true;
                            btn_cltf2018.Visible = false;
                        }

                        MessageBox.Show("[" + zyno + "]费用上传失败:" + back_mess[2].ToString() + mes + ";" + xmbhs, "提示");
                    }
                }
            }
            else
            {
                MessageBox.Show("请选中一个病人", "提示");
            }

            chk_jd.Checked = false;
        }

        private void infoDataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex != -1)
            {
                GetPatFee(infoDataGridView1.Rows[e.RowIndex].Cells["zyno"].Value.ToString());
                txt_zyh.Text = infoDataGridView1.Rows[e.RowIndex].Cells["zyno"].Value.ToString();
            }
        }

        private void txt_zyh_KeyDown(object sender, KeyEventArgs e)
        {
            xmbhs = "";
            btn_cltf.Visible = false;
            btn_cltf2018.Visible = false;

            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (string.IsNullOrWhiteSpace(txt_zyh.Text))
                    {
                        MessageBox.Show("请输入住院号并回车", "提示");
                        return;
                    }

                    string zyh = txt_zyh.Text = txt_zyh.Text.Trim().PadLeft(8, '0');
                    string sql = string.Format(@"select z1zyno, grbh bxh, z1hznm, z1ksno, z1ksnm, z1amt2
                    , sum(z3amnt * (case left(z3endv, 1) when '4' then -1 else 1 end)) amt2
                    , z1amt1, z1amt3, xm, jylsh, kh, min(z3kdys) z3kdys 
                    from zy01h join ybmzzydjdr on z1zyno = jzlsh and cxbz = 1
                                        left join zy03d on z1zyno = z3zyno and z1ghno = z3ghno and z1comp = z3comp 
                                        where left(z3kind, 1) in ('2', '4') and jzlsh not in(select jzlsh from ybfyjsdr where cxbz=1) and z1zyno='{0}'
                                        group by z1zyno, z1hznm, z1ksno, z1ksnm, z1amt1, z1amt2, z1amt3, xm, grbh, jylsh, kh,bzbm,bzmc,dgysdm,dgysxm 
                                        having sum(z3amnt * (case left(z3endv, 1) when '4' then -1 else 1 end)) > 0", zyh);
                    DataSet ds = CliUtils.ExecuteSql("shs01", "cmd", sql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                    infoDataGridView1.DataSource = ds.Tables[0];
                }
            }
            catch (Exception error)
            {
                MessageBox.Show(error.ToString(), "提示");
            }
        }

        private void btn_sx_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txt_zyh.Text))
            {
                MessageBox.Show("请输入住院号并回车", "提示");
                return;
            }

            string zyh = txt_zyh.Text = txt_zyh.Text.Trim().PadLeft(8, '0');
            string sql = string.Format(@"with tm2 as(
	        select z3ghno,SUM(z3amnt*(case substring(z3endv,1,1) when '4' then -1 else 1 end)) z3amt2 
	        from zy03d 
	        where substring(z3kind,1,1) in ('2','4') and isnull(z3jshx,'')='' and z3comp='{0}'  and z3zyno='{1}'
	        group by z3ghno
	        )
	        , tm1 as(
	        select z3ghno,SUM(z3amnt*(case substring(z3endv,1,1) when '4' then -1 else 1 end)) z3amt1 
	        from zy03d 
	        where  substring(z3kind,1,1)='1'  and isnull(z3jshx,'')='' and  z3comp='{0}'  and z3zyno='{1}' 
	        group by z3ghno
	        )
	        , tm3 as(
	        select z3ghno,SUM(z3amnt*(case substring(z3endv,1,1) when '4' then -1 else 1 end)) z3amt3 
	        from zy03d 
	        where  substring(z3kind,1,1)='3'  and isnull(z3jshx,'')='' and  z3comp='{0}'  and z3zyno='{1}' 
	        group by z3ghno
	        )
	        ,tot AS (
	        select amt2=ISNULL(z3amt2,0),amtz=ISNULL(z3amt1,0)+ISNULL(z3amt3,0),amt1=ISNULL(z3amt1,0),
	        amt3=ISNULL(z3amt3,0),z1ghno from zy01h 
	        left join tm2 on z1ghno=tm2.z3ghno 
	        left join tm1 on z1ghno=tm1.z3ghno
	        left join tm3 on z1ghno=tm3.z3ghno
	        )
	        UPDATE a SET a.z1amt1=b.amt1,a.z1amt2=b.amt2,a.z1amt3=b.amt3,a.z1amtz=b.amtz,a.z1amty=b.amtz-b.amt2
	        FROM zy01h a 
	        LEFT JOIN tot b ON a.z1ghno=b.z1ghno where a.z1comp='{0}'  and a.z1zyno='{1}'", CliUtils.fSiteCode, zyh); //and a.z1ghno='{2}'
            object[] obj = { sql };
            obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);

            if (obj[1].ToString() == "1")
            {
                MessageBox.Show("刷新成功", "提示");
                btn_sjhz_Click(null, null);
            }
            else
            {
                MessageBox.Show("刷新失败", "提示");
            }

            chk_jd.Checked = false;
        }

        private void btn_cltf_Click(object sender, EventArgs e)
        {
            if (xmbhs == "")
            {
                MessageBox.Show("医院项目编码为空,请重新上传费用", "医保提示");
                return;
            }

            string jzlsh = txt_zyh.Text.Trim();
            object[] obj = { jzlsh };
            obj = yb_interface.ybs_interface("4320",obj);
            obj = new object[] { jzlsh, xmbhs };
            obj = yb_interface.ybs_interface("5011",obj);

            if (obj[1].ToString() == "1")
            {
                MessageBox.Show("处理成功，请重新上传费用", "医保提示");
            }
            else
            {
                MessageBox.Show("处理失败", "医保提示");
            }

            xmbhs = "";
            btn_cltf.Visible = false;
            chk_jd.Checked = false;
        }

        private void btn_cltf2018_Click(object sender, EventArgs e)
        {
            if (xmbhs == "")
            {
                MessageBox.Show("医院项目编码为空,请重新上传费用", "医保提示");
                return;
            }

            string jzlsh = txt_zyh.Text.Trim();
            object[] obj = { jzlsh };
            obj = yb_interface.ybs_interface("4320",obj);
            obj = new object[] { jzlsh, xmbhs };
            obj = yb_interface.ybs_interface("5014",obj);

            if (obj[1].ToString() == "1")
            {
                MessageBox.Show("处理成功，请重新上传费用", "医保提示");
            }
            else
            {
                MessageBox.Show("处理失败", "医保提示");
            }

            xmbhs = "";
            btn_cltf2018.Visible = false;
            chk_jd.Checked = false;
        }

        private void btn_2018sc_Click(object sender, EventArgs e)
        {
            xmbhs = "";

            if (infoDataGridView1.CurrentRow != null && infoDataGridView1.CurrentRow.Index != -1)
            {
                string zyno = infoDataGridView1.CurrentRow.Cells["zyno"].Value.ToString();
                DialogResult dr = MessageBox.Show("确认上传病人[" + zyno + "]费用？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (dr == DialogResult.Yes)
                {
                    object[] back_mess, param = new object[] { zyno };
                    back_mess = yb_interface.ybs_interface("5014",param);

                    if (Convert.ToInt32(back_mess[1].ToString()) == 1)
                    {
                        MessageBox.Show("[" + zyno + "]所有费用上传成功", "提示");
                        btn_sjhz_Click(null, null);
                        GetPatFee(zyno);
                    }
                    else
                    {
                        string mes = "";

                        if (back_mess[2].ToString().Contains("1月1号"))
                        {
                            MessageBox.Show(back_mess[2].ToString());
                            xmbhs = back_mess[3].ToString();
                            //xmbhs = back_mess[2].ToString().Substring(back_mess[2].ToString().IndexOf("(##") + 3, back_mess[2].ToString().IndexOf("##)") - back_mess[2].ToString().IndexOf("(##") - 3);
                            mes = "；请执行右上角的处理退费按钮";
                            btn_cltf.Visible = false;
                            btn_cltf2018.Visible = true;
                        }

                        MessageBox.Show("[" + zyno + "]费用上传失败:" + back_mess[2].ToString() + mes + ";" + xmbhs, "提示");
                    }
                }
            }
            else
            {
                MessageBox.Show("请选中一个病人", "提示");
            }

            chk_jd.Checked = false;
        }

        private void chk_jd_Click(object sender, EventArgs e)
        {
            if (chk_jd.Checked)
            {
                dtp_jdrq.Visible = true;
            }
            else
            {
                dtp_jdrq.Visible = false;
            }
        }
    }
}
