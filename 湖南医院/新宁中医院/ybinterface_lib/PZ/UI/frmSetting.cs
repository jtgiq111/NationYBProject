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
    public partial class frmSetting : InfoForm
    {
        public frmSetting()
        {
            InitializeComponent();
        }

        private void frmSetting_Load(object sender, EventArgs e)
        {
            tabPage5.Parent = null;
            #region 绑定数据 
            string strSql = string.Format(@"select CSDM,CSMC,CSJM,PYM,WBM from YBCSZD order by CSDM");
            DataSet dsCS = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            alvCS.SetDataSource(dsCS.Copy());
            strSql = string.Format(@"select DQDM,DQMC,LMC,PYM,WBM from YBDQZD  order by DQDM");
            DataSet dsDQ = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
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

        #region 地区字典维护
        string oldDQDM = string.Empty; //地区代码
        //查询地区信息
        private void txtDQMC1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnDQSELECT_Click(sender, e);
            }
        }
        //查询地区信息
        private void btnDQSELECT_Click(object sender, EventArgs e)
        {
            string strSql = string.Format(@"SELECT DQDM,DQMC,LMC,QYBZ,case when QYBZ=1 then '启用' else '未启用' end QYBZMC FROM YBDQZD WHERE DQMC LIKE '%{0}%' OR PYM LIKE '%{0}%' OR WBM LIKE '%{0}%' ", txtDQMC1.Text.Trim());
            DataSet dsDQ = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            dgvDQZD.AutoGenerateColumns = false;
            dgvDQZD.DataSource = dsDQ.Tables[0].DefaultView;
        }
        //获取指定地区数据
        private void dgvDQZD_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex != -1)
            {
                oldDQDM = dgvDQZD.Rows[e.RowIndex].Cells["col_DQDM1"].Value.ToString(); //地区代码
                txtDQDM.Text = oldDQDM;
                txtDQMC.Text = dgvDQZD.Rows[e.RowIndex].Cells["col_dqmc1"].Value.ToString(); //地区名称
                txtLMC.Text = dgvDQZD.Rows[e.RowIndex].Cells["col_dqlmc"].Value.ToString(); //类名称
            }
        }
        //修改地区信息
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
                objSql = CliUtils.CallMethod("sybdj", "BatExecuteSql", objSql);
                if (objSql[1].ToString().Equals("1"))
                    MessageBox.Show("保存信息成功");
                else
                    MessageBox.Show("保存信息失败|" + objSql[2].ToString());
                txtDQMC1.Text = newDQMC;
                btnDQSELECT_Click(sender, e);
            }
        }
        //新增地区信息
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
            DataSet dsFalg = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
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
                objParam = CliUtils.CallMethod("sybdj", "BatExecuteSql", objParam);
                if (objParam[1].ToString().Equals("1"))
                    MessageBox.Show("新增信息成功");
                else
                    MessageBox.Show("新增信息失败|" + objParam[2].ToString());
                txtDQMC1.Text = newDQMC;
                btnDQSELECT_Click(sender, e);
            }

        }
        #endregion

        #region 厂商字典维护
        string oldSCDM = string.Empty;
        //查询地区信息
        private void txtSCMC1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnSCSELECT_Click(sender, e);
            }
        }
        //查询厂商信息
        private void btnSCSELECT_Click(object sender, EventArgs e)
        {
            string scmc=txtSCMC1.Text.Trim();
            string strSql = string.Format(@"select CSDM,CSMC,CSJM,QYBZ,case when QYBZ=1 then '启用' else '未启用' end QYBZMC from YBCSZD where CSMC like '%{0}%' OR PYM LIKE '%{0}%' OR WBM LIKE '%{0}%'", scmc);
            DataSet dsSC = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            dgvSCZD.AutoGenerateColumns = false;
            dgvSCZD.DataSource = dsSC.Tables[0].DefaultView;
        }
        //获取指定厂商信息
        private void dgvSCZD_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex != -1)
            {
                oldSCDM = dgvSCZD.Rows[e.RowIndex].Cells["col_csdm"].Value.ToString(); //地区代码
                txtSCDM.Text = oldSCDM;
                txtSCMC.Text = dgvSCZD.Rows[e.RowIndex].Cells["col_csmc"].Value.ToString(); //地区名称
                txtSCJM.Text = dgvSCZD.Rows[e.RowIndex].Cells["col_csjm"].Value.ToString(); //地区名称
            }
        }
        //保存配置信息
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
                objSql = CliUtils.CallMethod("sybdj", "BatExecuteSql", objSql);
                if (objSql[1].ToString().Equals("1"))
                    MessageBox.Show("保存信息成功");
                else
                    MessageBox.Show("保存信息失败|" + objSql[2].ToString());
                txtSCMC1.Text = newCSMC;
                btnSCSELECT_Click(sender, e);
            }
        }
        //新增厂商信息
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
            DataSet dsFalg = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
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
                objParam = CliUtils.CallMethod("sybdj", "BatExecuteSql", objParam);
                if (objParam[1].ToString().Equals("1"))
                    MessageBox.Show("新增信息成功");
                else
                    MessageBox.Show("新增信息失败|" + objParam[2].ToString());
                txtSCMC1.Text = newCSMC;
                btnSCSELECT_Click(sender, e);
            }
        }
        #endregion

        #region 适用医院字典维护
        string oldYYDM = string.Empty; //医院代码
        //获取适用医院信息
        private void txtYYMC1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnYYSELECT_Click(sender, e);
            }
        }
        //获取适用医院信息
        private void btnYYSELECT_Click(object sender, EventArgs e)
        {
            string yymc = txtYYMC1.Text.Trim();
            string strSql = string.Format(@"select YYDM,YYMC,JKMM,QYBZ,case when QYBZ=1 then '启用' else '未启用' end QYBZMC from YBYYZD where YYMC like '%{0}%' OR PYM LIKE '%{0}%' OR WBM LIKE '%{0}%'", yymc);
            DataSet dsYY = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            dgvYYZD.AutoGenerateColumns = false;
            dgvYYZD.DataSource = dsYY.Tables[0].DefaultView;
        }
        //获取指定医院信息
        private void dgvYYZD_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex != -1)
            {
                oldYYDM = dgvYYZD.Rows[e.RowIndex].Cells["col_yydm1"].Value.ToString(); //地区代码
                txtYYDM.Text = oldYYDM;
                txtYYMC.Text = dgvYYZD.Rows[e.RowIndex].Cells["col_yymc1"].Value.ToString(); //地区名称
                txtYYJKMM.Text = dgvYYZD.Rows[e.RowIndex].Cells["col_JKMM"].Value.ToString();

            }
        }
        //保存医院配置信息
        private void btnYYSAVE_Click(object sender, EventArgs e)
        {
            string newYYDM = oldYYDM;
            string newYYMC = txtYYMC.Text.Trim();
            string newYYKJMM = txtYYJKMM.Text.Trim();
            if (string.IsNullOrEmpty(newYYDM) || string.IsNullOrEmpty(newYYMC) || string.IsNullOrEmpty(newYYKJMM))
            {
                MessageBox.Show("医院编码或名称或接口命名不能为空");
                return;
            }
            DialogResult dr = MessageBox.Show("是否保存当前医院信息？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (dr == DialogResult.Yes)
            {
                //保存厂商信息
                string strSql = string.Format(@"update YBYYZD set YYDM='{0}',YYMC='{1}',JKMM='{2}',pym=dbo.f_get_PY('{1}',10),wbm=dbo.f_get_WB('{1}',10) where YYDM='{2}'", newYYDM, newYYMC, newYYKJMM);
                object[] objSql = { strSql };
                objSql = CliUtils.CallMethod("sybdj", "BatExecuteSql", objSql);
                if (objSql[1].ToString().Equals("1"))
                    MessageBox.Show("保存信息成功");
                else
                    MessageBox.Show("保存信息失败|" + objSql[2].ToString());
                txtYYMC1.Text = newYYMC;
                btnYYSELECT_Click(sender, e);
            }

        }
        //新增医院信息
        private void btnYYADD_Click(object sender, EventArgs e)
        {
            string newYYDM = txtYYDM.Text.Trim();
            string newYYMC = txtYYMC.Text.Trim();
            string newYYKJMM = txtYYJKMM.Text.Trim();
            if (string.IsNullOrEmpty(newYYDM) || string.IsNullOrEmpty(newYYMC) || string.IsNullOrEmpty(newYYKJMM))
            {
                MessageBox.Show("医院编码或名称或接口命名不能为空");
                return;
            }

            //判断是否已存在厂商代码
            string strSql = string.Format("select * from YBYYZD where YYDM='{0}'", newYYDM);
            DataSet dsFalg = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            if (dsFalg.Tables[0].Rows.Count > 0)
            {
                MessageBox.Show("当前医院代码已存在，请重新确认！");
                return;
            }

            
            DialogResult dr = MessageBox.Show("是否新增医院信息？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (dr == DialogResult.Yes)
            {

                strSql = string.Format(@"insert into YBYYZD(yydm,yymc,jkmm,pym,wbm) values(
                                    '{0}','{1}','{2}',dbo.f_get_PY('{1}',10),dbo.f_get_WB('{1}',10))",
                                     newYYDM, newYYMC, newYYKJMM);
                object[] objParam = { strSql };
                objParam = CliUtils.CallMethod("sybdj", "BatExecuteSql", objParam);
                if (objParam[1].ToString().Equals("1"))
                    MessageBox.Show("新增信息成功");
                else
                    MessageBox.Show("新增信息失败|" + objParam[2].ToString());
                txtYYMC1.Text = newYYMC;
                btnYYSELECT_Click(sender, e);
            }
        }
        #endregion

        #region 医保业务设置
        string oldYWDM = string.Empty;
        string QYBZ = string.Empty;
        string YYJKMM = string.Empty;
        //获取业务信息
        private void txtYWMC1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnYWSELECT_Click(sender, e);
            }
        }
        //查询业务数据
        private void btnYWSELECT_Click(object sender, EventArgs e)
        {
            string ywmc = txtYWMC1.Text.Trim();
            string strSql = string.Format(@"select a.YWDM,a.YWMC,a.QYBZ,case when a.QYBZ=1 then '启用' else '未启用' end QYBZMC,
                                            A.DQDM,B.DQMC,A.SCDM,C.CSMC,A.YYDM,D.YYMC,A.DLLMC,A.MMKJ,A.LMC,A.FFMC from YBYWDMZD a
                                            left join YBDQZD b on a.DQDM=b.DQDM 
                                            left join YBCSZD c on a.SCDM=c.CSDM
                                            left join YBYYZD d on a.YYDM=d.YYDM
                                            WHERE YWDM LIKE '%{0}%' OR YWMC LIKE '%{0}%' ORDER BY a.YWDM", ywmc);
            DataSet dsYW = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            dgvYWZD.AutoGenerateColumns = false;
            dgvYWZD.DataSource = dsYW.Tables[0].DefaultView;
        }
        //指定业务数据
        private void dgvYWZD_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex != -1)
            {
                oldYWDM = dgvYWZD.Rows[e.RowIndex].Cells["col_ywdm"].Value.ToString();
                txtYWDM.Text = oldYWDM;
                txtYWMC.Text = dgvYWZD.Rows[e.RowIndex].Cells["col_ywmc"].Value.ToString();
                txtFFMC.Text = dgvYWZD.Rows[e.RowIndex].Cells["col_ffmc"].Value.ToString();
                QYBZ = dgvYWZD.Rows[e.RowIndex].Cells["col_qybz"].Value.ToString();
                if (QYBZ.Equals("1"))
                    rabtnY.Checked = true;
                else
                    rabtnN.Checked = true;
            }

        }
        //保存业务数据
        private void btnYWSAVE_Click(object sender, EventArgs e)
        {
            string newYWDM = txtYWDM.Text.Trim();
            string newYWMC = txtYWMC.Text.Trim();
            string newFFMC = txtFFMC.Text.Trim();
            string newQYBZ="0";
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
                objSql = CliUtils.CallMethod("sybdj", "BatExecuteSql", objSql);
                if (objSql[1].ToString().Equals("1"))
                    MessageBox.Show("保存信息成功");
                else
                    MessageBox.Show("保存信息失败|" + objSql[2].ToString());
                txtYWMC1.Text = newYWMC;
                btnYWSELECT_Click(sender, e);
            }
        }
        //新增配置参数
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
            DataSet dsFalg = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            if (dsFalg.Tables[0].Rows.Count > 0)
            {
                MessageBox.Show("当前业务代码已存在，请重新确认！");
                return;
            }

            DialogResult dr = MessageBox.Show("是否新增业务配置？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (dr == DialogResult.Yes)
            {
                strSql = string.Format(@"insert into YBYWDMZD(YWDM,YWMC,QYBZ,FFMC) values('{0}','{1}','{2}','{3}')",
                                        newYWDM, newYWMC, QYBZ,newFFMC);
                object[] objSql = { strSql };
                objSql = CliUtils.CallMethod("sybdj", "BatExecuteSql", objSql);
                if (objSql[1].ToString().Equals("1"))
                    MessageBox.Show("新增信息成功");
                else
                    MessageBox.Show("新增信息失败|" + objSql[2].ToString());
                txtYWMC1.Text = newYWMC;
                btnYWSELECT_Click(sender, e);
            }
        }
        //重置当前配置
        private void btnYWRESET_Click(object sender, EventArgs e)
        {
            string newYWDM = txtYWDM.Text.Trim();
            string newYWMC = txtYWMC.Text.Trim();
            if (string.IsNullOrEmpty(newYWDM))
            {
                MessageBox.Show("参数数据不能为空!");
                return;
            }

            DialogResult dr = MessageBox.Show("是否重置当前业务配置？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (dr == DialogResult.Yes)
            {
                string strSql = string.Format(@"update YBYWDMZD set QYBZ='',DQDM='',SCDM='',YYDM='',LMC='' where YWDM='{0}'", newYWDM);
                object[] objSql = { strSql };
                objSql = CliUtils.CallMethod("sybdj", "BatExecuteSql", objSql);
                if (objSql[1].ToString().Equals("1"))
                    MessageBox.Show("保存信息成功");
                else
                    MessageBox.Show("保存信息失败|" + objSql[2].ToString());
                txtYWMC1.Text = newYWMC;
                btnYWSELECT_Click(sender, e);
            }
            ;
        }
        //重置所有配置
        private void btnRESETALL_Click(object sender, EventArgs e)
        {
            string newYWDM = txtYWDM.Text.Trim();
            string newYWMC = txtYWMC.Text.Trim();
            if (string.IsNullOrEmpty(newYWDM))
            {
                MessageBox.Show("参数数据不能为空!");
                return;
            }
            DialogResult dr = MessageBox.Show("是否重置所有业务配置？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (dr == DialogResult.Yes)
            {
                string strSql = string.Format(@"update YBYWDMZD set QYBZ='',DQDM='',SCDM='',YYDM='',LMC=''");
                object[] objSql = { strSql };
                objSql = CliUtils.CallMethod("sybdj", "BatExecuteSql", objSql);
                if (objSql[1].ToString().Equals("1"))
                    MessageBox.Show("保存信息成功");
                else
                    MessageBox.Show("保存信息失败|" + objSql[2].ToString());
                txtYWMC1.Text = newYWMC;
                btnYWSELECT_Click(sender, e);
            }
        }
        #endregion

        #region 接口测试
        private void btnJKTEST_Click(object sender, EventArgs e)
        {
            frmTest test = new frmTest();
            test.Show();
        }
        #endregion

        #region 医保配置
        string DQLMC = string.Empty;
        string CSJM = string.Empty;
        string oldCSDM = string.Empty;
        string oldCSMC = string.Empty;
        string oldDQDM_PZ = string.Empty;
        string oldDQMC_PZ = string.Empty;
        string oldDYBZ = string.Empty;
        string oldJKLB = string.Empty;


        private void alvCS_AfterConfirm(object sender, EventArgs e)
        {
            CSJM = alvCS.currentDataRow["csjm"].ToString();
        }

        private void alvDQ_AfterConfirm(object sender, EventArgs e)
        {
            DQLMC = alvDQ.currentDataRow["lmc"].ToString();
        }
        //医保配置查询
        private void btnPZSELECT_Click(object sender, EventArgs e)
        {
            string dqmc = txtPZNAME1.Text.Trim();
            string strSql = string.Format(@"select a.CSDM,b.CSMC,a.DQDM,c.DQMC,a.DLLMC,a.LMC,a.QYBZ,
                                            case when a.QYBZ=1 then '启用' else '未启用' end QYBZMC,
                                            a.SYSDATE,a.JKLB from YBPZZD a
                                            left join YBCSZD b on a.CSDM=b.CSDM
                                            left join YBDQZD c on a.DQDM=c.DQDM
                                            where c.DQMC like '%%'", dqmc);
            DataSet dsYW = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            dgvPZZD.AutoGenerateColumns = false;
            dgvPZZD.DataSource = dsYW.Tables[0].DefaultView;
        }
        //医保配置查询
        private void txtPZNAME1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnPZSELECT_Click(sender, e);
            }
        }
        //指定参数
        private void dgvPZZD_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
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
        //修改医保配置
        private void btnPZUPDATE_Click(object sender, EventArgs e)
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
                DataSet dsPZ = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
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
                objSql = CliUtils.CallMethod("sybdj", "BatExecuteSql", objSql);
                if (objSql[1].ToString().Equals("1"))
                    MessageBox.Show("保存信息成功");
                else
                    MessageBox.Show("保存信息失败|" + objSql[2].ToString());
                txtPZNAME1.Text = newDQMC;
                btnPZSELECT_Click(sender, e);
            }
        }
        //新增配置 
        private void btnPZADD_Click(object sender, EventArgs e)
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
            DataSet dsPZ = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            if (dsPZ.Tables[0].Rows.Count > 0)
            {
                MessageBox.Show("已存在相同配置参数，不能重复配置!");
                return;
            }
            //if (newQYBZ.Equals("1"))
            //{
            //    strSql = string.Format(@"select * from ybpzzd where qybz=1");
            //    DataSet dsPZQY = CliUtils.ExecuteSql("sybdjdr", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            //    if (dsPZQY.Tables[0].Rows.Count > 0)
            //    {
            //        MessageBox.Show("配置字典中存在启用配置,请查实!");
            //        return;
            //    }
            //}
            DQLMC = DQLMC + "_" + CSJM;
            strSql = string.Format(@"insert into YBPZZD(CSDM,DQDM,JKLB,LMC,QYBZ) 
                                    values('{0}','{1}','{2}','{3}','{4}')",
                                    newCSDM, newDQDM, newJKLB, DQLMC, newQYBZ);
            object[] objSql = { strSql };
            objSql = CliUtils.CallMethod("sybdj", "BatExecuteSql", objSql);
            if (objSql[1].ToString().Equals("1"))
                MessageBox.Show("新增配置成功");
            else
                MessageBox.Show("新增配置失败|" + objSql[2].ToString());
            txtPZNAME1.Text = newDQMC;
            btnPZSELECT_Click(sender, e);
        }
        #endregion

        private void button1_Click(object sender, EventArgs e)
        {
            FrmTest1 test1 = new FrmTest1();
            test1.Show();
        }

       


    }
}
