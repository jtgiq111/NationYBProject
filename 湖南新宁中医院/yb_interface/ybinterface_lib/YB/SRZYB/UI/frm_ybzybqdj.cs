using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Srvtools;

namespace ybinterface_lib
{
    public partial class frm_ybzybqdj : InfoForm
    {
        public frm_ybzybqdj()
        {
            InitializeComponent();
        }

        private void frm_ybzybqdj_Load(object sender, EventArgs e)
        {
            #region 加载病种信息

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
        }


         #region 带出患者信息
        private void txt_zyno_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (!string.IsNullOrWhiteSpace(txtJZLSH_ZY.Text))
                {
                    txtJZLSH_ZY.Text = txtJZLSH_ZY.Text.PadLeft(8, '0');
                }
                string strSql = string.Format(@"select a.jzlsh,a.xm, a.grbh, a.yldylb,a.ksmc,a.mmbzbm1,a.mmbzmc1,a.mmbzbm2,
                                        a.mmbzmc2,a.mmbzbm3,a.mmbzmc3 from ybmzzydjdr a where len( a.jzlsh)=8 and a.cxbz=1 and a.jzlsh='{0}'", txtJZLSH_ZY.Text.Trim());
                DataSet ds= CliUtils.ExecuteSql("sybdjdr", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                if (ds.Tables[0].Rows.Count == 0)
                {
                    MessageBox.Show("住院号错误或未办理医保登记!", "提示");
                    txtJZLSH_ZY.Text = "";
                    txtJZLSH_ZY.Focus();
                    return;
                }
                txtHZXM_ZY.Text = ds.Tables[0].Rows[0]["xm"].ToString();
                txtGRBH_ZY.Text = ds.Tables[0].Rows[0]["grbh"].ToString();
                txtRYLB_ZY.Text = ds.Tables[0].Rows[0]["yldylb"].ToString();
                txtKSMC_ZY.Text = ds.Tables[0].Rows[0]["ksmc"].ToString();
                string bzbm1 = ds.Tables[0].Rows[0]["mmbzbm1"].ToString();
                string bzmc1 = ds.Tables[0].Rows[0]["mmbzmc1"].ToString();
                string bzbm2 = ds.Tables[0].Rows[0]["mmbzbm2"].ToString();
                string bzmc2 = ds.Tables[0].Rows[0]["mmbzmc2"].ToString();
                string bzbm3 = ds.Tables[0].Rows[0]["mmbzbm3"].ToString();
                string bzmc3 = ds.Tables[0].Rows[0]["mmbzmc3"].ToString();
                if (!string.IsNullOrEmpty(bzbm1))
                {
                    txt_bz.Text = bzmc1;
                    txt_bz.Tag = bzbm1;
                }
                if (!string.IsNullOrEmpty(bzbm2))
                {
                    txt_bz1.Text = bzmc2;
                    txt_bz1.Tag = bzbm2;
                }
                if (!string.IsNullOrEmpty(bzbm3))
                {
                    txt_bz2.Text = bzbm3;
                    txt_bz2.Tag = bzmc3;
                }

            }
        }
        #endregion

        private void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtJZLSH_ZY.Text.Trim()))
            {
                MessageBox.Show("无住院号", "提示");
                txtJZLSH_ZY.Focus();
                return;
            }
            if (string.IsNullOrEmpty(txt_bz.Text.Trim()))
            {
                MessageBox.Show("主诊断必需输入", "提示");
                return;
            }
            string jzlsh = txtJZLSH_ZY.Text.Trim();
            string icd10_1 = txt_bz.Tag.ToString();
            string jbzd_1 = txt_bz.Text.Trim();
            string icd10_2 = string.Empty;
                string jbzd_2=string.Empty;
            string icd10_3=string.Empty;
            string jbzd_3=string.Empty;
            if (!string.IsNullOrEmpty(txt_bz1.Text.Trim()))
            {
                icd10_2 = txt_bz1.Tag.ToString();
                jbzd_2 = txt_bz1.Text.Trim();
            }
            if (!string.IsNullOrEmpty(txt_bz2.Text.Trim()))
            {
                icd10_3 = txt_bz2.Tag.ToString();
                jbzd_3 = txt_bz2.Text.Trim();
            }

            object[] objParam = { jzlsh, icd10_1, jbzd_1, icd10_2, jbzd_2, icd10_3, jbzd_3 };
            object[] objReturn = yb_interface.ybs_interface("4200", objParam);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

    }
}
