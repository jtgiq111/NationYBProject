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
    public partial class frm_cydjHF : Form
    {
        #region 变量
        private string strJZLSH = string.Empty;//就诊号
        public string strValue = string.Empty; //出参
        #endregion
        public frm_cydjHF()
        {
            InitializeComponent();
        }

        public frm_cydjHF(string Param)
        {
            InitializeComponent();
            strJZLSH = Param;
        }

        private void frm_cydjHF_Load(object sender, EventArgs e)
        {
            #region 获取患者信息
            string strSql = string.Format(@"select a.grbh,a.jzlsh,a.xm,a.ybjzlsh,b.z1outd from ybmzzydjdr a
                                            left join zy01d b on a.jzlsh=b.z1zyno and left(b.z1endv, 1) = '8'
                                            where a.jzlsh='{0}' and a.cxbz=1", strJZLSH);
            DataSet ds = CliUtils.ExecuteSql("sybdjdr", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            if (ds.Tables[0].Rows.Count == 0)
            {
                MessageBox.Show("无法获取患者信息|");
                btnDJ.Enabled = false;
                return;
            }
            txtJZLSH.Text = ds.Tables[0].Rows[0]["jzlsh"].ToString();
            txtXM.Text = ds.Tables[0].Rows[0]["xm"].ToString();
            txtGRBH.Text = ds.Tables[0].Rows[0]["grbh"].ToString();
            txtCYRQ.Text = ds.Tables[0].Rows[0]["z1outd"].ToString();
            #endregion

            #region  病种
            alvCYZD1.SetColumnParam.AddQueryColumn("dmmc");
            alvCYZD1.SetColumnParam.AddQueryColumn("pym");
            alvCYZD1.SetColumnParam.AddQueryColumn("dm");
            alvCYZD1.SetColumnParam.SetValueColumn("dm");
            alvCYZD1.SetColumnParam.SetTextColumn("dmmc");
            alvCYZD1.SetColumnParam.AddViewColumn("dm", "病种编码", 80);
            alvCYZD1.SetColumnParam.AddViewColumn("dmmc", "病种名称", 200);

            alvCYZD2.SetColumnParam.AddQueryColumn("dmmc");
            alvCYZD2.SetColumnParam.AddQueryColumn("pym");
            alvCYZD2.SetColumnParam.AddQueryColumn("dm");
            alvCYZD2.SetColumnParam.SetValueColumn("dm");
            alvCYZD2.SetColumnParam.SetTextColumn("dmmc");
            alvCYZD2.SetColumnParam.AddViewColumn("dm", "病种编码", 80);
            alvCYZD2.SetColumnParam.AddViewColumn("dmmc", "病种名称", 200);

            alvCYZD3.SetColumnParam.AddQueryColumn("dmmc");
            alvCYZD3.SetColumnParam.AddQueryColumn("pym");
            alvCYZD3.SetColumnParam.AddQueryColumn("dm");
            alvCYZD3.SetColumnParam.SetValueColumn("dm");
            alvCYZD3.SetColumnParam.SetTextColumn("dmmc");
            alvCYZD3.SetColumnParam.AddViewColumn("dm", "病种编码", 80);
            alvCYZD3.SetColumnParam.AddViewColumn("dmmc", "病种名称", 200);

             strSql = string.Format(@"select dm,dmmc,id,pym,wbm,ybm,bz from ybbzmrdr  order by len(dmmc)");
             ds.Tables.Clear();
             ds = CliUtils.ExecuteSql("sybdjdr", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            alvCYZD1.SetDataSource(ds.Copy());
            alvCYZD2.SetDataSource(ds.Copy());
            alvCYZD3.SetDataSource(ds.Copy());
            #endregion

            #region 出院类型

            alvCYZD3.SetColumnParam.AddQueryColumn("CODE");
            alvCYZD3.SetColumnParam.AddQueryColumn("PYM");
            alvCYZD3.SetColumnParam.AddQueryColumn("NAME");
            alvCYZD3.SetColumnParam.SetValueColumn("CODE");
            alvCYZD3.SetColumnParam.SetTextColumn("NAME");
            alvCYZD3.SetColumnParam.AddViewColumn("CODE", "病种编码", 80);
            alvCYZD3.SetColumnParam.AddViewColumn("NAME", "病种名称", 800);
            strSql = string.Format(@"SELECT CODE,NAME,PYM FROM YBXMLBZD WHERE LBMC='医保出院类型'");
            DataSet ds1 = CliUtils.ExecuteSql("sybdjdr", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            alvCYLX.SetDataSource(ds1.Copy());

            txtCYLX.Tag = "001";
            txtCYLX.Text = "正常出院";
            #endregion

            
        }

        private void btnDJ_Click(object sender, EventArgs e)
        {
            string jzlsh = txtJZLSH.Text;
            string grbh = txtGRBH.Text;
            string cyrq = txtCYRQ.Text;

            if (string.IsNullOrEmpty(txtCYLX.Text.Trim()))
            {
                MessageBox.Show("请选择出院类型");
                return;
            }
            if (string.IsNullOrEmpty(txtCYZD1.Text.Trim()))
            {
                MessageBox.Show("出院诊断（主） 不能为空");
                return;
            }

            string cylxmc = txtCYLX.Text;
            string cylxdm = txtCYLX.Tag.ToString();
            string cyzdbm1 = txtCYZD1.Tag.ToString();
            string cyzdmc1 = txtCYZD1.Text.Trim();
            string cyzdbm2 = "";
            string cyzdmc2 = "";
            string cyzdbm3 = "";
            string cyzdmc3 = "";

            if (!string.IsNullOrEmpty(txtCYZD2.Text.Trim()))
            {
                 cyzdbm2 = txtCYZD2.Tag.ToString();
                 cyzdmc2 = txtCYZD2.Text.Trim();
            }

            if (!string.IsNullOrEmpty(txtCYZD3.Text.Trim()))
            {
                cyzdbm3 = txtCYZD3.Tag.ToString();
                cyzdmc3 = txtCYZD3.Text.Trim();
            }

            strValue = "1|" + jzlsh + "|" + cylxdm + "|" + cylxmc + "|" + cyzdbm1 + "|" + cyzdmc1 + "|" + cyzdbm2 + "|" + cyzdmc2 + "|" + cyzdbm3 + "|" + cyzdmc3 + "|" + cyrq;
            this.Close();
        }

        private void btnCANCEL_Click(object sender, EventArgs e)
        {
            strValue = "0";
            this.Close();
        }


    }
}
