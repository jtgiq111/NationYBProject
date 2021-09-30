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
    public partial class Frm_ybbrdjbgLA : InfoForm
    {
        public Frm_ybbrdjbgLA()
        {
            InitializeComponent();
        }

        private void txt_zyno_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (!string.IsNullOrWhiteSpace(txt_zyno.Text))
                {
                    txt_zyno.Text = txt_zyno.Text.PadLeft(8, '0');
                }

                string strSql = string.Format(@"select  sfzh,xm,ghdjsj,yllb,bzbm,bzmc,b.NAME,a.zszbm,a.sylb,a.jhsylb,c.name as sylbmc,d.NAME as jhsylbmc from ybmzzydjdr a
                                                left join YBXMLBZD b on a.yllb=b.code and b.LBMC='医疗类别'
                                                left join YBXMLBZD c on a.sylb=c.code and c.LBMC='生育类别'
                                                left join YBXMLBZD d on a.jhsylb=d.code and d.LBMC='计划生育类别'
                                                where jzlsh='{0}' and cxbz=1", txt_zyno.Text);
                DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    txt_hzxm.Text = ds.Tables[0].Rows[0]["xm"].ToString();
                    txt_sfzh.Text = ds.Tables[0].Rows[0]["sfzh"].ToString();
                    txt_yllb.Tag = ds.Tables[0].Rows[0]["yllb"].ToString();
                    txt_yllb.Text = ds.Tables[0].Rows[0]["NAME"].ToString();
                    txt_bz.Tag = ds.Tables[0].Rows[0]["bzbm"].ToString();
                    txt_bz.Text = ds.Tables[0].Rows[0]["bzmc"].ToString();
                    txtZSZH.Text = ds.Tables[0].Rows[0]["zszbm"].ToString();
                    txtSYLB.Tag = ds.Tables[0].Rows[0]["sylb"].ToString();
                    txtSYLB.Text = ds.Tables[0].Rows[0]["sylbmc"].ToString();
                    txtJHSYLB.Tag = ds.Tables[0].Rows[0]["jhsylb"].ToString();
                    txtJHSYLB.Text = ds.Tables[0].Rows[0]["jhsylbmc"].ToString();
                    dtp_ryrq.Value = Convert.ToDateTime(ds.Tables[0].Rows[0]["ghdjsj"].ToString());
                }
                else
                {
                    MessageBox.Show("该患者未做医保登记");
                }
            }

        }

        private void Frm_ybbrdjbgLA_Load(object sender, EventArgs e)
        {
            #region 医疗类别
            alv_yllb.SetColumnParam.AddQueryColumn("bzname");
            alv_yllb.SetColumnParam.AddQueryColumn("bzpymx");
            alv_yllb.SetColumnParam.AddQueryColumn("bzwbmx");
            alv_yllb.SetColumnParam.SetValueColumn("bzkeyx");
            alv_yllb.SetColumnParam.SetTextColumn("bzname");
            alv_yllb.SetColumnParam.AddViewColumn("bzkeyx", "类别代码", 100);
            alv_yllb.SetColumnParam.AddViewColumn("bzname", "类别名称", 150);
            string strSQL = string.Format(@"select bzkeyx,bzname,bzpymx,bzwbmx from bztbd where bzcodn='YL' AND bzmem3='z' and bzusex=1");
            DataSet ds_yllb = CliUtils.ExecuteSql("sybdj", "cmd", strSQL, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            alv_yllb.SetDataSource(ds_yllb.Copy());
            txt_yllb.Tag = "";
            #endregion

            #region 病种
            alv_bz.SetColumnParam.AddQueryColumn("dmmc");
            alv_bz.SetColumnParam.AddQueryColumn("pym");
            alv_bz.SetColumnParam.AddQueryColumn("wbm");
            alv_bz.SetColumnParam.SetValueColumn("dm");
            alv_bz.SetColumnParam.SetTextColumn("dmmc");
            alv_bz.SetColumnParam.AddViewColumn("dm", "病种编码", 100);
            alv_bz.SetColumnParam.AddViewColumn("dmmc", "病种名称", 150);
            alv_bz.SetColumnParam.AddViewColumn("bzlb", "病种类别", 100);
            strSQL = string.Format(@"select dm,dmmc,pym,wbm,bzlb from ybbzmrdr  order by len(dmmc) asc");
            
            DataSet ds_bz = CliUtils.ExecuteSql("sybdj", "cmd", strSQL, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            alv_bz.SetDataSource(ds_bz.Copy());
            txt_bz.Tag = "";
            #endregion

            #region 生育类别
            alv_sylb.SetColumnParam.AddQueryColumn("NAME");
            alv_sylb.SetColumnParam.AddQueryColumn("PYM");
            alv_sylb.SetColumnParam.SetValueColumn("CODE");
            alv_sylb.SetColumnParam.SetTextColumn("NAME");
            alv_sylb.SetColumnParam.AddViewColumn("CODE", "编码", 50);
            alv_sylb.SetColumnParam.AddViewColumn("NAME", "名称", 100);
            strSQL = string.Format(@"select CODE,NAME,PYM,WB from YBXMLBZD where lbmc='生育类别'");
            DataSet ds_sylb = CliUtils.ExecuteSql("sybdj", "cmd", strSQL, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            alv_sylb.SetDataSource(ds_sylb.Copy());
            txtSYLB.Tag = "";
            #endregion

            #region 计划生育类别
            alv_jhsylb.SetColumnParam.AddQueryColumn("NAME");
            alv_jhsylb.SetColumnParam.AddQueryColumn("PYM");
            alv_jhsylb.SetColumnParam.SetValueColumn("CODE");
            alv_jhsylb.SetColumnParam.SetTextColumn("NAME");
            alv_jhsylb.SetColumnParam.AddViewColumn("CODE", "编码", 50);
            alv_jhsylb.SetColumnParam.AddViewColumn("NAME", "名称", 100);
            strSQL = string.Format(@"select CODE,NAME,PYM,WB from YBXMLBZD where lbmc='计划生育类别'");
            DataSet ds_jhsylb = CliUtils.ExecuteSql("sybdj", "cmd", strSQL, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            alv_jhsylb.SetDataSource(ds_jhsylb.Copy());
            txtJHSYLB.Tag = "";
            #endregion
        }

        private void alv_yllb_AfterConfirm(object sender, EventArgs e)
        {
            if (alv_yllb.currentDataRow != null)
            {
                txt_yllb.Tag = alv_yllb.currentDataRow["bzkeyx"].ToString();
                txt_yllb.Text = alv_yllb.currentDataRow["bzname"].ToString();
            }
        }

        private void alv_bz_AfterConfirm(object sender, EventArgs e)
        {
            if (alv_bz.currentDataRow != null)
            {
                txt_bz.Tag = alv_bz.currentDataRow["dm"].ToString();
                txt_bz.Text = alv_bz.currentDataRow["dmmc"].ToString();
            }

        }

        private void alv_sylb_AfterConfirm(object sender, EventArgs e)
        {
            if (alv_sylb.currentDataRow != null)
            {
                txtSYLB.Tag = alv_sylb.currentDataRow["CODE"].ToString();
                txtSYLB.Text = alv_sylb.currentDataRow["NAME"].ToString();
            }
        }

        private void alv_jhsylb_AfterConfirm(object sender, EventArgs e)
        {

            if (alv_jhsylb.currentDataRow != null)
            {
                txtJHSYLB.Tag = alv_jhsylb.currentDataRow["CODE"].ToString();
                txtJHSYLB.Text = alv_jhsylb.currentDataRow["NAME"].ToString();
            }
        }

        private void btnBG_Click(object sender, EventArgs e)
        {
            if ((txt_yllb.Text.Trim() == "") || (txt_yllb.Tag == null) || (txt_yllb.Tag.ToString() == ""))
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
            else if (dtp_ryrq.Value == null || dtp_ryrq.Value.ToString() == "")
            {
                MessageBox.Show("入院时间不能为空!", "提示");
                txt_bz.Focus();
                return;
            }

             object[] back_mess, param =new object[] { txt_zyno.Text, txt_yllb.Tag, txt_bz.Tag, txt_bz.Text, dtp_ryrq.Value.ToString(),txtZSZH.Text.Trim(), txtSYLB.Tag, txtJHSYLB.Tag };
             back_mess = yb_interface.ybs_interface("4101", param);

             if (back_mess[1].ToString() == "1")
             {
                 MessageBox.Show("医保住院登记变更成功!", "提示", MessageBoxButtons.OK);
             }
             else
             {
                 MessageBox.Show(back_mess[2].ToString(), "提示");
             }

        }

        private void btnCLOSE_Click(object sender, EventArgs e)
        {
            this.Close();
        }


    }
}
