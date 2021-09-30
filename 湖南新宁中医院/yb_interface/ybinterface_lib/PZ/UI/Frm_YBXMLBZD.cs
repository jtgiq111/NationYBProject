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
    public partial class Frm_YBXMLBZD : Form
    {
        string strWhere = string.Empty;
        string oldXMLB = string.Empty;
        string oldXMDM = string.Empty;
        string oldXMMC = string.Empty;
        string oldSC = string.Empty;
        string oldDQ = string.Empty;
        string newXMLB = string.Empty;
        string newXMDM = string.Empty;
        string newXMMC = string.Empty;
        string newCS = string.Empty;
        string newDQ = string.Empty;
        string newQYBZ = string.Empty;
        string newBZ = string.Empty;

        public Frm_YBXMLBZD()
        {
            InitializeComponent();
        }

        private void Frm_YBXMLBZD_Load(object sender, EventArgs e)
        {
            tssl_czyxm.Text = CliUtils.fUserName;

            #region 数据绑定
            //项目类别
            string strSql = string.Format(@"SELECT 0 as ID,'全部' as LBMC UNION ALL
                                            SELECT ROW_NUMBER() OVER(ORDER BY LBMC) AS ID,  LBMC  from YBXMLBZD group by LBMC ");
            DataSet dsXMLB = CliUtils.ExecuteSql("sybdjdr", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            cmbXMLB.DataSource = dsXMLB.Tables[0];
            cmbXMLB.DisplayMember = "LBMC";
            cmbXMLB.ValueMember = "ID";

            //适合医保厂商
            strSql = string.Format(@"SELECT CSDM,CSMC from YBCSZD ");
            DataSet dsCSZD = CliUtils.ExecuteSql("sybdjdr", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            cmbYBCS.DataSource = dsCSZD.Tables[0];
            cmbYBCS.DisplayMember = "CSMC";
            cmbYBCS.ValueMember = "CSDM";

            //适用地区
            strSql = string.Format(@"SELECT DQDM,DQMC FROM YBDQZD ");
            DataSet dsDQZD = CliUtils.ExecuteSql("sybdjdr", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            cmbDQ.DataSource = dsDQZD.Tables[0];
            cmbDQ.DisplayMember = "DQMC";
            cmbDQ.ValueMember = "DQDM";
            #endregion
        }

        private void cmbXMLB_SelectedIndexChanged(object sender, EventArgs e)
        {
            oldXMLB = cmbXMLB.Text;
            ControlsClear();
            btnLBSELECT_Click(sender, e);
        }

        internal void ControlsClear()
        {
            txtXMLB.Text = "";
            txtXMDM.Text = "";
            txtXMMC.Text = "";

        }

        private void btnLBSELECT_Click(object sender, EventArgs e)
        {
            if (!cmbXMLB.SelectedValue.ToString().Equals("0"))
                strWhere = " and a.LBMC='" + cmbXMLB.Text + "'";

            if (!string.IsNullOrEmpty(txtXMJS.Text.Trim()))
                strWhere += string.Format(@" and (a.NAME LIKE '%{0}%' OR a.PYM LIKE '%{0}%' OR a.WB LIKE'%{0}%')", txtXMJS.Text.Trim());

            string strSql = string.Format(@" SELECT a.ID,a.LBMC,a.CODE,a.NAME,a.QYBZ,a.CSDM,b.CSMC,a.DQDM,c.DQMC,a.PYM,a.WB, a.BZ FROM YBXMLBZD a
                                             LEFT JOIN YBCSZD b on a.csdm=b.CSDM
                                             LEFT JOIN YBDQZD c on a.DQDM=c.DQDM WHERE 1=1 {0}", strWhere);
            DataSet dsXM = CliUtils.ExecuteSql("sybdjdr", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);

            dgvXMLBZD.AutoGenerateColumns = false;
            dgvXMLBZD.DataSource = dsXM.Tables[0].DefaultView;
        }

        private void btnADD_Click(object sender, EventArgs e)
        {
            newXMLB = txtXMLB.Text.Trim();
            newXMDM = txtXMDM.Text.Trim();
            newXMMC = txtXMMC.Text.Trim();
            newCS = cmbYBCS.SelectedValue.ToString();
            newDQ = cmbDQ.SelectedValue.ToString();
            if (rabtnBQY.Checked)
                newQYBZ = "0";
            if (rabtnQY.Checked)
                newQYBZ = "1";
            newBZ = txtBZ.Text.Trim();

            if (string.IsNullOrEmpty(newXMLB))
            {
                MessageBox.Show("项目类别不能为空");
                return;
            }
            if (string.IsNullOrEmpty(newXMDM))
            {
                MessageBox.Show("项目代码不能为空");
                return;
            }
            if (string.IsNullOrEmpty(newXMMC))
            {
                MessageBox.Show("项目名称不能为空");
                return;
            }

            //判断是否已存在同相记录
            string strSql = string.Format(@"select * from YBXMLBZD where LBMC='{0}' and CODE='{1}' and CSDM='{2}' and DQDM='{3}'",
                                            newXMLB, newXMDM, newCS, newDQ);
            DataSet ds = CliUtils.ExecuteSql("sybdjdr", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            if (ds.Tables[0].Rows.Count > 0)
            {
                MessageBox.Show("已存在相同属性的项目，不能重复添加！");
                return;
            }
            //插入数据
            strSql = string.Format(@"insert into YBXMLBZD(LBMC,CODE,NAME,PYM,WB,QYBZ,CSDM,DQDM,BZ)
                                    VALUES('{0}','{1}','{2}',DBO.GetPYM('{2}'),DBO.GetWBM('{2}'),'{3}','{4}','{5}','{6}')",
                                    newXMLB, newXMDM, newXMMC, newQYBZ, newCS, newDQ, newBZ);
            object[] objSql = { strSql };
            objSql = CliUtils.CallMethod("sybdjdr", "BatExecuteSql", objSql);
            if (objSql[1].ToString().Equals("1"))
                MessageBox.Show("添加成功");
            else
                MessageBox.Show("添加失败|" + objSql[2].ToString());
        }

        private void txtXMJS_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnLBSELECT_Click(sender, e);
            }
        }

        private void btnUPDATE_Click(object sender, EventArgs e)
        {
            newXMLB = txtXMLB.Text.Trim();
            newXMDM = txtXMDM.Text.Trim();
            newXMMC = txtXMMC.Text.Trim();
            newCS = cmbYBCS.SelectedValue.ToString();
            newDQ = cmbDQ.SelectedValue.ToString();
            if (rabtnBQY.Checked)
                newQYBZ = "0";
            if (rabtnQY.Checked)
                newQYBZ = "1";
            newBZ = txtBZ.Text.Trim();

            if (string.IsNullOrEmpty(newXMMC))
            {
                MessageBox.Show("项目名称不能为空");
                return;
            }
            if (!oldXMLB.Equals(newXMLB) || !oldXMDM.Equals(newXMDM))
            {
                MessageBox.Show("修改功能不针对项目类别、项目代码的修改操作");
                return;
            }
            string strSql = string.Empty;
            if (!oldSC.Equals(newCS) || !oldDQ.Equals(newDQ))
            {
                //判断是否已存在记录
                strSql = string.Format(@"select * from YBXMLBZD where LBMC='{0}' and CODE='{1}' and CSDM='{2}' and DQDM='{3}'",
                                                oldXMLB, oldXMMC, newCS, newDQ);
                DataSet ds = CliUtils.ExecuteSql("sybdjdr", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                if (ds.Tables[0].Rows.Count == 0)
                {
                    MessageBox.Show("不存在当前属性的项目，删除项目失败");
                    return;
                }
            }

            //修改记录
            strSql = string.Format(@"update YBXMLBZD set NAME='{0}',QYBZ='{1}',CSDM='{2}',DQDM='{3}',BZ='{4}',PYM=dbo.GetPYM('{0}'),WB=dbo.GetWBM('{0}')
                                    where LBMC='{5}' and CODE='{6}'", newXMMC, newQYBZ, newCS, newDQ, newBZ, oldXMLB, oldXMMC);
            object[] objSql = { strSql };
            objSql = CliUtils.CallMethod("sybdjdr", "BatExecuteSql", objSql);
            if (objSql[1].ToString().Equals("1"))
                MessageBox.Show("修改成功");
            else
                MessageBox.Show("修改失败|" + objSql[2].ToString());
        }

        private void btnDEL_Click(object sender, EventArgs e)
        {
            newXMLB = txtXMLB.Text.Trim();
            newXMDM = txtXMDM.Text.Trim();
            newCS = cmbYBCS.SelectedValue.ToString();
            newDQ = cmbDQ.SelectedValue.ToString();

            if (string.IsNullOrEmpty(newXMLB))
            {
                MessageBox.Show("项目类别不能为空");
                return;
            }
            if (string.IsNullOrEmpty(newXMDM))
            {
                MessageBox.Show("项目代码不能为空");
                return;
            }

            //判断是否已存在同相记录
            string strSql = string.Format(@"select * from YBXMLBZD where LBMC='{0}' and CODE='{1}' and CSDM='{2}' and DQDM='{3}'",
                                            newXMLB, newXMDM, newCS, newDQ);
            DataSet ds = CliUtils.ExecuteSql("sybdjdr", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            if (ds.Tables[0].Rows.Count == 0)
            {
                MessageBox.Show("不存在当前属性的项目，删除项目失败");
                return;
            }

            if (MessageBox.Show("确定删除当前项目", "医保配置提示", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
            {
                //删除记录
                strSql = string.Format(@"delete from YBXMLBZD where LBMC='{0}' and CODE='{1}' and CSDM='{2}' and DQDM='{3}'",
                                       newXMLB, newXMDM, newCS, newDQ);
                object[] objSql = { strSql };
                objSql = CliUtils.CallMethod("sybdjdr", "BatExecuteSql", objSql);
                if (objSql[1].ToString().Equals("1"))
                    MessageBox.Show("删除成功");
                else
                    MessageBox.Show("删除失败|" + objSql[2].ToString());
            }
        }

        private void dgvXMLBZD_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex != -1)
            {
                oldXMLB = dgvXMLBZD.Rows[e.RowIndex].Cells["col_lbmc"].Value.ToString();
                oldXMDM = dgvXMLBZD.Rows[e.RowIndex].Cells["col_code"].Value.ToString();
                oldDQ = dgvXMLBZD.Rows[e.RowIndex].Cells["col_dq"].Value.ToString();
                oldSC = dgvXMLBZD.Rows[e.RowIndex].Cells["col_cs"].Value.ToString();

                txtXMDM.Text = dgvXMLBZD.Rows[e.RowIndex].Cells["col_code"].Value.ToString();
                txtXMMC.Text = dgvXMLBZD.Rows[e.RowIndex].Cells["col_name"].Value.ToString();
                txtXMLB.Text = dgvXMLBZD.Rows[e.RowIndex].Cells["col_lbmc"].Value.ToString();
                txtXMDM.Text = dgvXMLBZD.Rows[e.RowIndex].Cells["col_code"].Value.ToString();
                txtXMMC.Text = dgvXMLBZD.Rows[e.RowIndex].Cells["col_name"].Value.ToString();
                cmbYBCS.SelectedValue = dgvXMLBZD.Rows[e.RowIndex].Cells["col_cs"].Value.ToString();
                cmbDQ.SelectedValue = dgvXMLBZD.Rows[e.RowIndex].Cells["col_dq"].Value.ToString();
                if (dgvXMLBZD.Rows[e.RowIndex].Cells["col_qybz"].Value.ToString().Equals("1"))
                    rabtnQY.Checked = true;
                else
                    rabtnBQY.Checked = false;
                txtBZ.Text = dgvXMLBZD.Rows[e.RowIndex].Cells["col_bz"].Value.ToString();
            }
        }
    }
}
