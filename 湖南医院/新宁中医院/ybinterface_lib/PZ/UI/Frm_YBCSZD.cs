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
    public partial class Frm_YBCSZD : Form
    {
        string oldSCDM = string.Empty;

        public Frm_YBCSZD()
        {
            InitializeComponent();
        }

        private void Frm_YBCSZD_Load(object sender, EventArgs e)
        {
            tssl_czyxm.Text = CliUtils.fUserName;
        }

        private void txtINDEX_KeyDown(object sender, KeyEventArgs e)
        {

            if (e.KeyCode == Keys.Enter)
            {
                btnCSSELECT_Click(sender, e);
            }
        }

        private void btnCSSELECT_Click(object sender, EventArgs e)
        {
            string scmc = txtINDEX.Text.Trim();
            string strSql = string.Format(@"select CSDM,CSMC,CSJM,QYBZ,case when QYBZ=1 then '启用' else '未启用' end QYBZMC from YBCSZD where CSMC like '%{0}%' OR PYM LIKE '%{0}%' OR WBM LIKE '%{0}%'", scmc);
            DataSet dsSC = CliUtils.ExecuteSql("sybdjdr", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            dgvSCZD.AutoGenerateColumns = false;
            dgvSCZD.DataSource = dsSC.Tables[0].DefaultView;
        }

        private void btnSCADD_Click(object sender, EventArgs e)
        {
            string newCSDM = txtSCDM.Text.Trim();
            string newCSMC = txtSCMC.Text.Trim();
            string newCSJM = txtSCJM.Text.Trim();
            if (string.IsNullOrEmpty(newCSDM) || string.IsNullOrEmpty(newCSMC) || string.IsNullOrEmpty(newCSJM))
            {
                MessageBox.Show("厂商代码、名称、简码不能为空");
                return;
            }
            //判断是否已存在地区代码
            string strSql = string.Format("select * from ybdqzd where dqdm='{0}'", newCSDM);
            DataSet dsFalg = CliUtils.ExecuteSql("sybdjdr", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            if (dsFalg.Tables[0].Rows.Count > 0)
            {
                MessageBox.Show("当前厂商代码已存在，请重新确认厂商代码");
                return;
            }
            DialogResult dr = MessageBox.Show("是否新增厂商信息？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (dr == DialogResult.Yes)
            {
                //保存新增地区信息
                strSql = string.Format(@"insert into ybcszd(csdm,csmc,csjm,pym,wbm) values(
                                    '{0}','{1}','{2}',dbo.f_get_PY('{1}',10),dbo.f_get_WB('{1}',10))",
                                        newCSDM, newCSMC, newCSJM);
                object[] objParam = { strSql };
                objParam = CliUtils.CallMethod("sybdjdr", "BatExecuteSql", objParam);
                if (objParam[1].ToString().Equals("1"))
                    MessageBox.Show("新增信息成功");
                else
                    MessageBox.Show("新增信息失败|" + objParam[2].ToString());
                btnCSSELECT_Click(sender, e);
            }
        }

        private void btnSCSAVE_Click(object sender, EventArgs e)
        {
            string newCSDM = txtSCDM.Text.Trim();
            string newCSMC = txtSCMC.Text.Trim();
            string newCSJM = txtSCJM.Text.Trim();
            if (string.IsNullOrEmpty(newCSDM) || string.IsNullOrEmpty(newCSMC) || string.IsNullOrEmpty(newCSJM))
            {
                MessageBox.Show("厂商代码、名称、简码不能为空");
                return;
            }

            if (string.IsNullOrEmpty(oldSCDM))
            {
                MessageBox.Show("请选择需要修改的厂商信息");
                return;
            }

            DialogResult dr = MessageBox.Show("是否保存厂商信息？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (dr == DialogResult.Yes)
            {
                //保存厂商信息
                string strSql = string.Format(@"update ybcszd set csdm='{0}',csmc='{1}',csjm='{3}',pym=dbo.f_get_PY('{1}',10),wbm=dbo.f_get_WB('{1}',10) where csdm='{2}'", newCSDM, newCSMC, oldSCDM, newCSJM);
                object[] objSql = { strSql };
                objSql = CliUtils.CallMethod("sybdjdr", "BatExecuteSql", objSql);
                if (objSql[1].ToString().Equals("1"))
                    MessageBox.Show("保存信息成功");
                else
                    MessageBox.Show("保存信息失败|" + objSql[2].ToString());

                btnCSSELECT_Click(sender, e);
            }
        }

        private void dgvSCZD_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex != -1)
            {
                oldSCDM = dgvSCZD.Rows[e.RowIndex].Cells["col_csdm"].Value.ToString(); //地区代码
                txtSCDM.Text = oldSCDM;
                txtSCMC.Text = dgvSCZD.Rows[e.RowIndex].Cells["col_csmc"].Value.ToString(); //地区名称
                txtSCJM.Text = dgvSCZD.Rows[e.RowIndex].Cells["col_csjm"].Value.ToString(); //地区名称
            }
        }
    }
}
