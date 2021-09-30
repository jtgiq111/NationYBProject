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
    public partial class Frm_YBDQZD : Form
    {

        string oldDQDM = string.Empty; //地区代码

        public Frm_YBDQZD()
        {
            InitializeComponent();
        }

        private void Frm_YBDQZD_Load(object sender, EventArgs e)
        {
            tssl_czyxm.Text = CliUtils.fUserName;
        }

        private void btnDQSELECT_Click(object sender, EventArgs e)
        {
            string strSql = string.Format(@"SELECT DQDM,DQMC,LMC,QYBZ,case when QYBZ=1 then '启用' else '未启用' end QYBZMC FROM YBDQZD WHERE DQMC LIKE '%{0}%' OR PYM LIKE '%{0}%' OR WBM LIKE '%{0}%' ", txtINDEX.Text.Trim());
            DataSet dsDQ = CliUtils.ExecuteSql("sybdjdr", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            dgvDQZD.AutoGenerateColumns = false;
            dgvDQZD.DataSource = dsDQ.Tables[0].DefaultView;
        }

        private void txtINDEX_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnDQSELECT_Click(sender, e);
            }
        }

        private void btnDQSAVE_Click(object sender, EventArgs e)
        {
            string newDQDM = txtDQDM.Text.Trim();
            string newDQMC = txtDQMC.Text.Trim();
            string newLMC = txtLMC.Text.Trim();
            if (string.IsNullOrEmpty(newDQDM) || string.IsNullOrEmpty(newDQMC) || string.IsNullOrEmpty(newLMC))
            {
                MessageBox.Show("地区代码或地区名称或类名称不能为空");
                return;
            }

            if (string.IsNullOrEmpty(oldDQDM))
            {
                MessageBox.Show("请选择需要修改的地区信息");
                return;
            }

            DialogResult dr = MessageBox.Show("是否保存当前地区信息？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (dr == DialogResult.Yes)
            {
                //保存地区信息
                string strSql = string.Format(@"update ybdqzd set dqdm='{0}',dqmc='{1}',lmc='{3}',pym=dbo.f_get_PY('{1}',10),wbm=dbo.f_get_WB('{1}',10) where dqdm='{2}'", newDQDM, newDQMC, oldDQDM, newLMC);
                object[] objSql = { strSql };
                objSql = CliUtils.CallMethod("sybdjdr", "BatExecuteSql", objSql);
                if (objSql[1].ToString().Equals("1"))
                    MessageBox.Show("保存信息成功");
                else
                    MessageBox.Show("保存信息失败|" + objSql[2].ToString());
                btnDQSELECT_Click(sender, e);
            }
        }

        private void btnDQADD_Click(object sender, EventArgs e)
        {
            string newDQDM = txtDQDM.Text.Trim();
            string newDQMC = txtDQMC.Text.Trim();
            string newLMC = txtLMC.Text.Trim();
            if (string.IsNullOrEmpty(newDQDM) || string.IsNullOrEmpty(newDQMC) || string.IsNullOrEmpty(newLMC))
            {
                MessageBox.Show("地区代码或地区名称或类名称不能为空");
                return;
            }
            //判断是否已存在地区代码
            string strSql = string.Format("select * from ybdqzd where dqdm='{0}'", newDQDM);
            DataSet dsFalg = CliUtils.ExecuteSql("sybdjdr", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            if (dsFalg.Tables[0].Rows.Count > 0)
            {
                MessageBox.Show("当前地区代码已存在，请重新确认地区代码");
                return;
            }

            DialogResult dr = MessageBox.Show("是否新增地区信息？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (dr == DialogResult.Yes)
            {
                //保存新增地区信息
                strSql = string.Format(@"insert into ybdqzd(dqdm,dqmc,lmc,pym,wbm) values(
                                    '{0}','{1}','{2}',dbo.f_get_PY('{1}',10),dbo.f_get_WB('{1}',10))",
                                        newDQDM, newDQMC, newLMC);
                object[] objParam = { strSql };
                objParam = CliUtils.CallMethod("sybdjdr", "BatExecuteSql", objParam);
                if (objParam[1].ToString().Equals("1"))
                    MessageBox.Show("新增信息成功");
                else
                    MessageBox.Show("新增信息失败|" + objParam[2].ToString());
                btnDQSELECT_Click(sender, e);
            }
        }
    }
}
