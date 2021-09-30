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
    public partial class Frm_YBYWZD : Form
    {

        string oldYWDM = string.Empty;
        string QYBZ = string.Empty;
        string YYJKMM = string.Empty;

        public Frm_YBYWZD()
        {
            InitializeComponent();
        }

        private void Frm_YBYWZD_Load(object sender, EventArgs e)
        {
            tssl_czyxm.Text = CliUtils.fUserName;
        }

        private void txtINDEX_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnYWSELECT_Click(sender, e);
            }
        }

        private void btnYWADD_Click(object sender, EventArgs e)
        {
            string newYWDM = txtYWDM.Text.Trim();
            string newYWMC = txtYWMC.Text.Trim();
            string newFFMC = txtFFMC.Text.Trim();
            if (rabtnY.Checked)
                QYBZ = "1";
            else
                QYBZ = "0";

            if (string.IsNullOrEmpty(newYWDM) || string.IsNullOrEmpty(newYWMC))
            {
                MessageBox.Show("参数数据不能为空!");
                return;
            }

            string strSql = string.Format(@"select * from YBYWDMZD where YWDM='{0}'", newYWDM);
            DataSet dsFalg = CliUtils.ExecuteSql("sybdjdr", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            if (dsFalg.Tables[0].Rows.Count > 0)
            {
                MessageBox.Show("当前业务代码已存在，请重新确认！");
                return;
            }

            DialogResult dr = MessageBox.Show("是否新增业务配置？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (dr == DialogResult.Yes)
            {
                strSql = string.Format(@"insert into YBYWDMZD(YWDM,YWMC,QYBZ,FFMC) values('{0}','{1}','{2}','{3}')",
                                        newYWDM, newYWMC, QYBZ, newFFMC);
                object[] objSql = { strSql };
                objSql = CliUtils.CallMethod("sybdjdr", "BatExecuteSql", objSql);
                if (objSql[1].ToString().Equals("1"))
                    MessageBox.Show("新增信息成功");
                else
                    MessageBox.Show("新增信息失败|" + objSql[2].ToString());
                btnYWSELECT_Click(sender, e);
            }
        }

        private void btnYWSELECT_Click(object sender, EventArgs e)
        {
            string ywmc = txtINDEX.Text.Trim();
            string strSql = string.Format(@"select a.YWDM,a.YWMC,a.QYBZ,case when a.QYBZ=1 then '启用' else '未启用' end QYBZMC,
                                            A.DQDM,B.DQMC,A.SCDM,C.CSMC,A.YYDM,D.YYMC,A.DLLMC,A.MMKJ,A.LMC,A.FFMC from YBYWDMZD a
                                            left join YBDQZD b on a.DQDM=b.DQDM 
                                            left join YBCSZD c on a.SCDM=c.CSDM
                                            left join YBYYZD d on a.YYDM=d.YYDM
                                            WHERE YWDM LIKE '%{0}%' OR YWMC LIKE '%{0}%' ORDER BY a.YWDM", ywmc);
            DataSet dsYW = CliUtils.ExecuteSql("sybdjdr", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            dgvYWZD.AutoGenerateColumns = false;
            dgvYWZD.DataSource = dsYW.Tables[0].DefaultView;
        }

        private void btnYWSAVE_Click(object sender, EventArgs e)
        {
            string newYWDM = txtYWDM.Text.Trim();
            string newYWMC = txtYWMC.Text.Trim();
            string newFFMC = txtFFMC.Text.Trim();
            string newQYBZ = "0";
            if (rabtnY.Checked)
                newQYBZ = "1";
            else
                newQYBZ = "0";

            if (string.IsNullOrEmpty(newYWDM) || string.IsNullOrEmpty(newYWMC) || string.IsNullOrEmpty(newFFMC))
            {
                MessageBox.Show("参数数据不能为空!");
                return;
            }

            DialogResult dr = MessageBox.Show("是否保存当前业务配置？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (dr == DialogResult.Yes)
            {
                string strSql = string.Format(@"update YBYWDMZD set YWDM='{1}',YWMC='{2}',QYBZ='{3}',FFMC='{4}' where YWDM='{0}'",
                                                oldYWDM, newYWDM, newYWMC, newQYBZ, newFFMC);
                object[] objSql = { strSql };
                objSql = CliUtils.CallMethod("sybdjdr", "BatExecuteSql", objSql);
                if (objSql[1].ToString().Equals("1"))
                    MessageBox.Show("保存信息成功");
                else
                    MessageBox.Show("保存信息失败|" + objSql[2].ToString());
                btnYWSELECT_Click(sender, e);
            }
        }
    }
}
