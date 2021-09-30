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
    public partial class Frm_YBPZZD : Form
    {

        string DQLMC = string.Empty;
        string CSJM = string.Empty;
        string oldCSDM = string.Empty;
        string oldCSMC = string.Empty;
        string oldDQDM_PZ = string.Empty;
        string oldDQMC_PZ = string.Empty;
        string oldDYBZ = string.Empty;
        string oldJKLB = string.Empty;

        public Frm_YBPZZD()
        {
            InitializeComponent();
        }

        private void Frm_YBPZZD_Load(object sender, EventArgs e)
        {
            tssl_czyxm.Text = CliUtils.fUserName;


            #region 绑定数据
            string strSql = string.Format(@"select CSDM,CSMC,CSJM,PYM,WBM from YBCSZD order by CSDM");
            DataSet dsCS = CliUtils.ExecuteSql("sybdjdr", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            alvCS.SetDataSource(dsCS.Copy());
            strSql = string.Format(@"select DQDM,DQMC,LMC,PYM,WBM from YBDQZD  order by DQDM");
            DataSet dsDQ = CliUtils.ExecuteSql("sybdjdr", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            alvDQ.SetDataSource(dsDQ.Copy());
            #endregion

            #region 设置AutoListView
            alvCS.SetColumnParam.AddQueryColumn("CSDM");
            alvCS.SetColumnParam.AddQueryColumn("PYM");
            alvCS.SetColumnParam.AddQueryColumn("WBM");
            alvCS.SetColumnParam.SetValueColumn("CSDM");
            alvCS.SetColumnParam.SetTextColumn("CSMC");
            alvCS.SetColumnParam.AddViewColumn("CSDM", "编码", 80);
            alvCS.SetColumnParam.AddViewColumn("CSMC", "名称", 200);
            alvCS.SetColumnParam.AddViewColumn("CSJM", "简码", 200);

            alvDQ.SetColumnParam.AddQueryColumn("dqmc");
            alvDQ.SetColumnParam.AddQueryColumn("pym");
            alvDQ.SetColumnParam.AddQueryColumn("wbm");
            alvDQ.SetColumnParam.SetValueColumn("dqdm");
            alvDQ.SetColumnParam.SetTextColumn("dqmc");
            alvDQ.SetColumnParam.AddViewColumn("dqdm", "编码", 80);
            alvDQ.SetColumnParam.AddViewColumn("dqmc", "名称", 200);
            alvDQ.SetColumnParam.AddViewColumn("lmc", "类名称", 60);
            #endregion

        }

        private void btnPZSELECT_Click(object sender, EventArgs e)
        {
            string strSql = string.Format(@"select a.CSDM,b.CSMC,a.DQDM,c.DQMC,a.DLLMC,a.LMC,a.QYBZ,
                                            case when a.QYBZ=1 then '启用' else '未启用' end QYBZMC,
                                            a.SYSDATE,a.JKLB from YBPZZD a
                                            left join YBCSZD b on a.CSDM=b.CSDM
                                            left join YBDQZD c on a.DQDM=c.DQDM");
            DataSet dsYW = CliUtils.ExecuteSql("sybdjdr", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            dgvPZZD.AutoGenerateColumns = false;
            dgvPZZD.DataSource = dsYW.Tables[0].DefaultView;

        }

        private void button1_Click(object sender, EventArgs e)
        {
            string newCSDM = txtYBCS_PZ.Tag.ToString();
            string newDQDM = txtYBDQ_PZ.Tag.ToString();
            string newDQMC = txtYBDQ_PZ.Text.Trim();
            string newQYBZ = "0";
            string newJKLB = cmbJKLB.Text;
            if (string.IsNullOrEmpty(newCSDM) || string.IsNullOrEmpty(newDQDM) || string.IsNullOrEmpty(newJKLB))
            {
                MessageBox.Show("请选择医保厂商或医保地区或接口类别！");
                return;
            }

            if (rabtnY_pz.Checked)
                newQYBZ = "1";
            string strSql = string.Empty;
            //判断是否存在重复配置
            strSql = string.Format(@"select * from YBPZZD where CSDM='{0}' and DQDM='{1}'", newCSDM, newDQDM);
            DataSet dsPZ = CliUtils.ExecuteSql("sybdjdr", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            if (dsPZ.Tables[0].Rows.Count > 0)
            {
                MessageBox.Show("已存在相同配置参数，不能重复配置!");
                return;
            }
            DQLMC = DQLMC + "_" + CSJM;
            strSql = string.Format(@"insert into YBPZZD(CSDM,DQDM,JKLB,LMC,QYBZ) 
                                    values('{0}','{1}','{2}','{3}','{4}')",
                                    newCSDM, newDQDM, newJKLB, DQLMC, newQYBZ);
            object[] objSql = { strSql };
            objSql = CliUtils.CallMethod("sybdjdr", "BatExecuteSql", objSql);
            if (objSql[1].ToString().Equals("1"))
                MessageBox.Show("新增配置成功");
            else
                MessageBox.Show("新增配置失败|" + objSql[2].ToString());

            btnPZSELECT_Click(sender, e);
        }

        private void btnUPDATE_Click(object sender, EventArgs e)
        {
            string newCSDM = txtYBCS_PZ.Tag.ToString();
            string newDQDM = txtYBDQ_PZ.Tag.ToString();
            string newDQMC = txtYBDQ_PZ.Text.Trim();
            string newQYBZ = "0";
            string newJKLB = cmbJKLB.Text;

            if (string.IsNullOrEmpty(newCSDM) || string.IsNullOrEmpty(newDQDM) || string.IsNullOrEmpty(newJKLB))
            {
                MessageBox.Show("请选择医保厂商或医保地区或接口类别！");
                return;
            }

            if (rabtnY_pz.Checked)
                newQYBZ = "1";
            string strSql = string.Empty;
            //判断是否存在重复配置
            if ((!newCSDM.Equals(oldCSDM)) || (!newDQDM.Equals(oldDQDM_PZ)))
            {
                strSql = string.Format(@"select * from YBPZZD where CSDM='{0}' and DQDM='{1}'", newCSDM, newDQDM);
                DataSet dsPZ = CliUtils.ExecuteSql("sybdjdr", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                if (dsPZ.Tables[0].Rows.Count > 0)
                {
                    MessageBox.Show("已存在相同配置参数，不能重复配置!");
                    return;
                }
            }

            DQLMC = DQLMC + "_" + CSJM;
            DialogResult dr = MessageBox.Show("是否保存当前配置？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (dr == DialogResult.Yes)
            {
                //修改配置
                strSql = string.Format(@"update YBPZZD set CSDM='{2}',DQDM='{3}',JKLB='{4}',LMC='{5}',QYBZ='{6}'
                                    WHERE CSDM='{0}' AND DQDM='{1}'", oldCSDM, oldDQDM_PZ, newCSDM, newDQDM, newJKLB, DQLMC, newQYBZ);
                object[] objSql = { strSql };
                objSql = CliUtils.CallMethod("sybdjdr", "BatExecuteSql", objSql);
                if (objSql[1].ToString().Equals("1"))
                    MessageBox.Show("保存信息成功");
                else
                    MessageBox.Show("保存信息失败|" + objSql[2].ToString());
                btnPZSELECT_Click(sender, e);
            }

        }

        private void dgvPZZD_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex != -1)
            {
                oldCSDM = dgvPZZD.Rows[e.RowIndex].Cells["col_PZCSDM"].Value.ToString();
                oldCSMC = dgvPZZD.Rows[e.RowIndex].Cells["col_PZCSMC"].Value.ToString();
                txtYBCS_PZ.Text = oldCSMC;
                txtYBCS_PZ.Tag = oldCSDM;
                oldDQDM_PZ = dgvPZZD.Rows[e.RowIndex].Cells["col_PZDQDM"].Value.ToString();
                oldDQMC_PZ = dgvPZZD.Rows[e.RowIndex].Cells["col_PZDQMC"].Value.ToString();
                txtYBDQ_PZ.Text = oldDQMC_PZ;
                txtYBDQ_PZ.Tag = oldDQDM_PZ;
                oldJKLB = dgvPZZD.Rows[e.RowIndex].Cells["col_PZJKLB"].Value.ToString();
                cmbJKLB.Text = oldJKLB;
                oldDYBZ = dgvPZZD.Rows[e.RowIndex].Cells["col_PZDYBZ"].Value.ToString();
                if (oldDYBZ.Equals("1"))
                    rabtnY_pz.Checked = true;
                else
                    rabtnN_pz.Checked = true;
            }
        }

        private void alvCS_AfterConfirm(object sender, EventArgs e)
        {
            CSJM = alvCS.currentDataRow["csjm"].ToString();
        }

        private void alvDQ_AfterConfirm(object sender, EventArgs e)
        {
            DQLMC = alvDQ.currentDataRow["lmc"].ToString();
        }
    }
}
