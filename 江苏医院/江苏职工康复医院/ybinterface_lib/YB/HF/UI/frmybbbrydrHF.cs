using Srvtools;
using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;

namespace ybinterface_lib
{
    public partial class frmybbbrydrHF : InfoForm
    {
        public frmybbbrydrHF()
        {
            InitializeComponent();
        }

        #region 带出患者信息
        private void txt_zyno_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (!string.IsNullOrWhiteSpace(txt_zyno.Text))
                {
                    txt_zyno.Text = txt_zyno.Text.PadLeft(8, '0');
                }

                string strwhere = string.Format("zy01h.z1zyno = '{0}'", txt_zyno.Text);
                idszy01h.SetWhere(strwhere);

                if (idszy01h.GetRealDataSet().Tables[0].Rows.Count <= 0)
                {
                    MessageBox.Show("住院号错误或患者已入科!", "提示");
                    txt_zyno.Text = "";
                    txt_zyno.Focus();
                    return;
                }
                else
                {
                    DataRow dr = idszy01h.GetRealDataSet().Tables[0].Rows[0];
                    txt_hzxm.Text = dr["z1hznm"].ToString();
                    txt_sfzh.Text = dr["z1idno"].ToString();
                    txt_zyno.Tag = dr["z1ghno"];
                    txt_yllb.Text = "正常入院";
                    txt_yllb.Tag = "001";
                    txt_bz.Text = dr["z1bznm"].ToString();
                    txt_bz.Tag = dr["z1bzno"];
                    dtp_ryrq.Value = Convert.ToDateTime(dr["z1date"]);

                    #region 判断是否已进行医保登记
                    string strSql = string.Format(@"select * from ybmzzydjdr where jzlsh='{0}' and cxbz=1", txt_zyno.Text.Trim());
                    DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        MessageBox.Show("该患者已进行医保登记，请勿重复登记");
                        return;
                    }
                    #endregion
                }

                txt_yllb.Focus();
            }
        }
        #endregion

        private void btn_Close_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Fybbbry_Load(object sender, EventArgs e)
        {
            #region  病种
            alvBZ.SetColumnParam.AddQueryColumn("dmmc");
            alvBZ.SetColumnParam.AddQueryColumn("pym");
            alvBZ.SetColumnParam.AddQueryColumn("dm");
            alvBZ.SetColumnParam.SetValueColumn("dm");
            alvBZ.SetColumnParam.SetTextColumn("dmmc");
            alvBZ.SetColumnParam.AddViewColumn("dm", "病种编码", 80);
            alvBZ.SetColumnParam.AddViewColumn("dmmc", "病种名称", 200);

            alvBZ1.SetColumnParam.AddQueryColumn("dmmc");
            alvBZ1.SetColumnParam.AddQueryColumn("pym");
            alvBZ1.SetColumnParam.AddQueryColumn("dm");
            alvBZ1.SetColumnParam.SetValueColumn("dm");
            alvBZ1.SetColumnParam.SetTextColumn("dmmc");
            alvBZ1.SetColumnParam.AddViewColumn("dm", "病种编码", 80);
            alvBZ1.SetColumnParam.AddViewColumn("dmmc", "病种名称", 200);

            alvBZ2.SetColumnParam.AddQueryColumn("dmmc");
            alvBZ2.SetColumnParam.AddQueryColumn("pym");
            alvBZ2.SetColumnParam.AddQueryColumn("dm");
            alvBZ2.SetColumnParam.SetValueColumn("dm");
            alvBZ2.SetColumnParam.SetTextColumn("dmmc");
            alvBZ2.SetColumnParam.AddViewColumn("dm", "病种编码", 80);
            alvBZ2.SetColumnParam.AddViewColumn("dmmc", "病种名称", 200);

            string strSql = string.Format(@"select dm,dmmc,id,pym,wbm,ybm,bz from ybbzmrdr  order by len(dmmc)");
            DataSet ds = CliUtils.ExecuteSql("sybdjdr", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            alvBZ.SetDataSource(ds.Copy());
            alvBZ1.SetDataSource(ds.Copy());
            alvBZ2.SetDataSource(ds.Copy());
            #endregion

            Dictionary<string, string[]> dict1 = new Dictionary<string, string[]>();
            dict1.Add("code", new string[] { "入院代码", "90" });
            dict1.Add("name", new string[] { "入院名称", "100" });
            txt_yllb.ShowItem = dict1;
            txt_yllb.showTextItem = "name";
            txt_yllb.setWhere = "LBMC='医保入院类型' and NAME like'%{0}%' and PYM like '%{0}%'";
            txt_yllb.GotFocus += new EventHandler(txt_GotFocus);

            Dictionary<string, string[]> dict2 = new Dictionary<string, string[]>();
            dict2.Add("b6lynm", new string[] { "来源机构", "200" });
            txt_lyjg.ShowItem = dict2;
            txt_lyjg.showTextItem = "b6lynm";
            txt_lyjg.setWhere = " b6comp = '" + CliUtils.fSiteCode + "' and (isnull(b6pymx, '') like '%{0}%' or b6lynm like '%{0}%')";
            txt_lyjg.GotFocus += new EventHandler(txt_GotFocus);
          
        }

        private void txt_GotFocus(object sender, EventArgs e)
        {
            UserTextBox utb = sender as UserTextBox;

            switch (utb.Name)
            {
                case "txt_yllb":
                    {
                        if (utb.dgvRow.Index != -1)
                        {
                            txt_yllb.Tag = utb.dgvRow.Cells["code"].Value;
                            txt_yllb.Text = utb.dgvRow.Cells["name"].Value.ToString();
                        }
                        else
                        {
                            if (string.IsNullOrWhiteSpace(txt_yllb.Text))
                            {
                                txt_yllb.Tag = null;
                            }
                        }
                    }
                    break;
                case "txt_lyjg":
                    {
                        if (utb.dgvRow.Index != -1)
                        {
                            txt_lyjg.Tag = txt_lyjg.dgvRow.Cells["b6lyno"].Value.ToString().Trim();
                            txt_lyjg.Text = txt_lyjg.dgvRow.Cells["b6lynm"].Value.ToString().Trim();
                        }
                        else
                        {
                            if (string.IsNullOrWhiteSpace(txt_lyjg.Text))
                            {
                                txt_lyjg.Text = string.Empty;
                                txt_lyjg.Tag = null;
                            }
                        }
                    }
                    break;
            }
        }

        private bool GetYBInfo(ref string backmsg)
        {
            object[] back_mess = yb_interface.ybs_interface("2201", null);

            if (back_mess[1].ToString() == "1")
            {
                string[] strP = back_mess[2].ToString().Split('|');
                txt_grbh.Text = strP[0].ToString(); //个人编号
                txt_sfzh1.Text = strP[2].ToString(); //身份证号
                txt_xm.Text = strP[3].ToString(); //姓名
                string YBxb = strP[4].ToString(); //性别

                if (YBxb == "1")
                {
                    txt_xb.Text = "男";
                }
                else
                {
                    txt_xb.Text = "女";
                }

                txt_csrq.Text = strP[6].ToString(); //出生日期
                txt_szdw.Text = strP[25].ToString();//单位名称
                txt_zhye.Text = strP[14].ToString();//帐户余额
                txt_rylb.Text = strP[8].ToString();
                txt_sfzh.Text = strP[2].ToString();
                if (back_mess == null)
                {
                    return false;
                }
            }
            else
            {
                MessageBox.Show(string.Format("读医保卡失败:{0}", back_mess[2].ToString()), "提示");
                return false;
            }

            backmsg = back_mess[2].ToString();
            return true;
        }

        private void btn_RegisterYB_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txt_zyno.Text) || string.IsNullOrEmpty(txt_hzxm.Text))
            {
                MessageBox.Show("请输入住院号并按回车键!", "提示");
                txt_zyno.Focus();
                return;
            }
            else if ((txt_yllb.Text.Trim() == "") || (txt_yllb.Tag == null) || (txt_yllb.Tag.ToString() == ""))
            {
                MessageBox.Show("医疗类别不能为空!", "提示");
                txt_yllb.Focus();
                return;
            }
            else if ((txt_bz.Text.Trim() == "") || (txt_bz.Tag == null) || (txt_bz.Tag.ToString() == ""))
            {
                MessageBox.Show("病种不能为空!", "提示");
                txt_bz.Focus();
                return;
            }
            else if ((txt_lyjg.Text.Trim() == "") || (txt_lyjg.Tag == null) || (txt_lyjg.Tag.ToString() == ""))
            {
                MessageBox.Show("缴费类型不能为空!", "提示");
                txt_lyjg.Focus();
                return;
            }
            //else if(string.IsNullOrEmpty(txtDGYS.Text.Trim()))
            //{
            //    MessageBox.Show("请选择定岗医生","提示");
            //    txtDGYS.Focus();
            //    return;
            //}

            if (string.IsNullOrEmpty(txt_bz1.Text.Trim()))
            {
                txt_bz1.Tag = "";
                txt_bz1.Text = "";
            }
            if (string.IsNullOrEmpty(txt_bz2.Text.Trim()))
            {
                txt_bz2.Tag = "";
                txt_bz2.Text = "";
            }


            if (string.IsNullOrWhiteSpace(txt_grbh.Text))
            {
                string backmsg = "";

                if (!GetYBInfo(ref backmsg))
                {
                    return;
                }

                txt_grbh.Tag = backmsg;
            }

            //判断办理入院登记的患者姓名是否与读卡的姓名一致  
            if (txt_hzxm.Text.Trim() != txt_xm.Text.Trim())
            {
                MessageBox.Show("该IC卡姓名[" + txt_xm.Text.Trim() + "]与患者姓名不符，不能办理医保入院!", "提示");
                txt_zyno.Focus();
                return;
            }

            object[] back_mess, param = new object[] { txt_zyno.Text, txt_yllb.Tag, txt_bz.Tag, txt_bz.Text, txt_grbh.Tag, txt_lyjg.Tag, txt_lyjg.Text, txt_yllb.Text, "", "", txt_sfzh.Text, txt_bz1.Tag.ToString(), txt_bz1.Text.Trim(), txt_bz2.Tag.ToString(), txt_bz2.Text.Trim(), txt_yllb.Tag };
            btn_RegisterYB.Enabled = false;
            back_mess = yb_interface.ybs_interface("4100", param);

            if (back_mess[1].ToString() == "1")
            {
                MessageBox.Show("医保补办入院成功!", "提示", MessageBoxButtons.OK);
            }
            else
            {
                MessageBox.Show(back_mess[2].ToString(), "提示");
            }

            btn_RegisterYB.Enabled = true;
        }

        private void btn_ReadCard_Click(object sender, EventArgs e)
        {
            btn_ReadCard.Enabled = false;
            string msg = "";
            GetYBInfo(ref msg);
            txt_grbh.Tag = msg;
            btn_ReadCard.Enabled = true;
        }

        private void btn_CancelYB_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txt_zyno.Text) || string.IsNullOrEmpty(txt_hzxm.Text))
            {
                MessageBox.Show("住院号不能为空!", "提示");
                txt_zyno.Focus();
                return;
            }

            btn_CancelYB.Enabled = false;
            object[] obj = { txt_zyno.Text.Trim() };
            obj = yb_interface.ybs_interface("4102", obj);

            if (obj[1].ToString() == "1")
            {
                MessageBox.Show("医保登记撤销成功", "提示");
            }
            else
            {
                MessageBox.Show(obj[2].ToString(), "提示");
            }

            btn_CancelYB.Enabled = true;
        }
    }
}
