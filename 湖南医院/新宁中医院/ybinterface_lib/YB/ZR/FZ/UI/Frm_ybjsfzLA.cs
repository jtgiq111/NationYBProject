using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Srvtools;
using System.IO;

namespace ybinterface_lib
{
    public partial class Frm_ybjsfzLA : InfoForm
    {
        private string MZLB = "1"; //住院门诊类别 1-住院 2-门诊
        private string CBSF=""; //参保身份

        public Frm_ybjsfzLA()
        {
            InitializeComponent();
        }


        internal void getInfo()
        {
            string dtStart = dtp_Start.Value.ToString("yyyy-MM-dd") + " 00:00:00";
            string dtEnd = dtp_End.Value.ToString("yyyy-MM-dd") + " 23:59:59";
            string sNAME = txtName.Text.Trim();
            string strSQL = string.Empty;
            string strSQL1 = string.Empty;
            string sQFXJE = "1=1";
            //if (ckQFXJE.Checked)
            //    sQFXJE = "a.tcjjzf='13.00'";

            //经办人信息
            string jbr = txtJBR.Tag.ToString();
            if (jbr.Equals("0"))
                jbr = "";
            else
                jbr = "and a.jbr='" + jbr + "'";

            string strCBSF = "";
            if (!string.IsNullOrEmpty(CBSF))
                strCBSF = "and a.cbsf='" + CBSF + "'";
            else
                strCBSF = "";

            DataSet ds = null;
            DataSet ds1 = null;
            if (MZLB.Equals("1"))
            {
                strSQL1 = string.Format(@"select a.jbr,b.b1name as jbrxm, count(jbr) as bxrs,
                                        sum(ylfze)ylfze,sum(zbxje)zbxje,sum(tcjjzf)tcjjzf,sum(zhzf)zhzf,sum(xjzf)xjzf,
                                        sum(Convert(decimal(8,2),zgjbyljjzf))zgjbyljjzf,
                                        sum(Convert(decimal(8,2),czjmjbyljjzf))czjmjbyljjzf,
                                        sum(Convert(decimal(8,2),cxjmjjyljjzf))cxjmjjyljjzf,
                                        sum(Convert(decimal(8,2),dejjzf))dejjzf,
                                        sum(Convert(decimal(8,2),gwybzjjzf))gwybzjjzf,
                                        sum(Convert(decimal(8,2),qybcylbxjjzf))qybcylbxjjzf,
                                        sum(Convert(decimal(8,2),eyzxyljjzf))eyzxyljjzf,
                                        sum(Convert(decimal(8,2),lhjzxyljjzf))lhjzxyljjzf,
                                        sum(Convert(decimal(8,2),lxgbddtczf))lxgbddtczf,
                                        sum(Convert(decimal(8,2),ylbjzf))ylbjzf,
                                        sum(Convert(decimal(8,2),qtjjzf))qtjjzf,
                                        sum(Convert(decimal(8,2),gsjjzf))gsjjzf,
                                        sum(Convert(decimal(8,2),syjjzf))syjjzf,
                                        sum(Convert(decimal(8,2),mzjzfy))mzjzfy,
                                        sum(Convert(decimal(8,2),yyfdfy))yyfdfy 
                                        from ybfyjsdr a
                                        left join bz01h b on a.jbr=b.b1empn
                                        where cxbz=1 and len(a.jzlsh)=8
                                        and convert(datetime,SUBSTRING(jsrq,0,9)) between '{0}' and '{1}'  and (a.xm like '%{2}%' or a.jzlsh like '%{2}%') and {3} {4} {5}
                                        group by a.jbr,b.b1name
                                        order by a.jbr,b.b1name", dtStart, dtEnd, sNAME, sQFXJE, jbr, strCBSF);
                ds1 = CliUtils.ExecuteSql("sybdj", "cmd", strSQL1, CliUtils.fLoginDB.ToString(), true, CliUtils.fCurrentProject.ToString());
                //显示数据信息
                idgv_ybfyhz.AutoGenerateColumns = false;
                idgv_ybfyhz.ClearSelection();
                idgv_ybfyhz.DataSource = ds1.Tables[0];

            }
            else
            {
                strSQL1 = string.Format(@"select a.jbr,b.b1name as jbrxm, count(jbr) as bxrs,
                                        sum(ylfze)ylfze,sum(zbxje)zbxje,sum(tcjjzf)tcjjzf,sum(zhzf)zhzf,sum(xjzf)xjzf,
                                        sum(Convert(decimal(8,2),zgjbyljjzf))zgjbyljjzf,
                                        sum(Convert(decimal(8,2),czjmjbyljjzf))czjmjbyljjzf,
                                        sum(Convert(decimal(8,2),cxjmjjyljjzf))cxjmjjyljjzf,
                                        sum(Convert(decimal(8,2),dejjzf))dejjzf,
                                        sum(Convert(decimal(8,2),gwybzjjzf))gwybzjjzf,
                                        sum(Convert(decimal(8,2),qybcylbxjjzf))qybcylbxjjzf,
                                        sum(Convert(decimal(8,2),eyzxyljjzf))eyzxyljjzf,
                                        sum(Convert(decimal(8,2),lhjzxyljjzf))lhjzxyljjzf,
                                        sum(Convert(decimal(8,2),lxgbddtczf))lxgbddtczf,
                                        sum(Convert(decimal(8,2),ylbjzf))ylbjzf,
                                        sum(Convert(decimal(8,2),qtjjzf))qtjjzf,
                                        sum(Convert(decimal(8,2),gsjjzf))gsjjzf,
                                        sum(Convert(decimal(8,2),syjjzf))syjjzf,
                                        sum(Convert(decimal(8,2),mzjzfy))mzjzfy,
                                        sum(Convert(decimal(8,2),yyfdfy))yyfdfy 
                                        from ybfyjsdr a
                                        left join bz01h b on a.jbr=b.b1empn
                                        where cxbz=1 and len(a.jzlsh)=9
                                        and convert(datetime,SUBSTRING(jsrq,0,9)) between '{0}' and '{1}'  and  (a.xm like '%{2}%' or a.jzlsh like '%{2}%') and {3} {4} {5}
                                        group by a.jbr,b.b1name
                                        order by a.jbr,b.b1name", dtStart, dtEnd, sNAME, sQFXJE, jbr, strCBSF);

                ds1 = CliUtils.ExecuteSql("sybdj", "cmd", strSQL1, CliUtils.fLoginDB.ToString(), true, CliUtils.fCurrentProject.ToString());

                //显示数据信息
                idgv_ybfyhz.AutoGenerateColumns = false;
                idgv_ybfyhz.ClearSelection();
                idgv_ybfyhz.DataSource = ds1.Tables[0].DefaultView;
            }
        }
        internal void getInfo1()
        {
            string dtStart = dtp_Start.Value.ToString("yyyy-MM-dd") + " 00:00:00";
            string dtEnd = dtp_End.Value.ToString("yyyy-MM-dd") + " 23:59:59";
            string sNAME = txtName.Text.Trim();
            string strSQL = string.Empty;
            string strSQL1 = string.Empty;
            string sQFXJE = "1=1";
            //if (ckQFXJE.Checked)
            //    sQFXJE = "a.tcjjzf='13.00'";

            //经办人信息
            string jbr = txtJBR.Tag.ToString();
            if (jbr.Equals("0"))
                jbr = "";
            else
                jbr = "and a.jbr='" + jbr + "'";

            string strCBSF = "";
            if (!string.IsNullOrEmpty(CBSF))
                strCBSF = "and a.cbsf='" + CBSF + "'";
            else
                strCBSF = "";


            DataSet ds = null;
            if (MZLB.Equals("1"))
            {
                strSQL = string.Format(@"select jzlsh,grbh,xm,convert(date,SUBSTRING(jsrq,0,9)) as jsrq,yllb,cbsf,dqmc,jbr,b.b1name as jbrxm,
                                        ylfze,zbxje,tcjjzf,zhzf,xjzf,zgjbyljjzf,czjmjbyljjzf,cxjmjjyljjzf,
                                        dejjzf,gwybzjjzf,qybcylbxjjzf,eyzxyljjzf,lhjzxyljjzf,lxgbddtczf,
                                        ylbjzf,qtjjzf,gsjjzf,syjjzf,mzjzfy,yyfdfy from ybfyjsdr a
                                        left join bz01h b on a.jbr=b.b1empn
                                        where cxbz=1 and len(jzlsh)=8
                                        and convert(datetime,SUBSTRING(jsrq,0,9)) between '{0}' and '{1}' and (a.xm like '%{2}%' or a.jzlsh like '%{2}%') and {3} {4} {5}
                                        order by jsrq,xm",
                                        dtStart, dtEnd, sNAME, sQFXJE, jbr, strCBSF);

                ds = CliUtils.ExecuteSql("sybdj", "cmd", strSQL, CliUtils.fLoginDB.ToString(), true, CliUtils.fCurrentProject.ToString());
                //显示数据信息
                idgv_YbInfo.AutoGenerateColumns = false;
                idgv_YbInfo.ClearSelection();
                idgv_YbInfo.DataSource = ds.Tables[0];
            }
            else
            {
                strSQL = string.Format(@"select jzlsh,grbh,xm,convert(date,SUBSTRING(jsrq,0,9)) as jsrq,yllb,cbsf,dqmc,jbr,b.b1name as jbrxm,
                                        ylfze,zbxje,tcjjzf,zhzf,xjzf,zgjbyljjzf,czjmjbyljjzf,cxjmjjyljjzf,
                                        dejjzf,gwybzjjzf,qybcylbxjjzf,eyzxyljjzf,lhjzxyljjzf,lxgbddtczf,
                                        ylbjzf,qtjjzf,gsjjzf,syjjzf,mzjzfy,yyfdfy from ybfyjsdr a
                                        left join bz01h b on a.jbr=b.b1empn
                                        where cxbz=1 and len(jzlsh)=9
                                        and convert(datetime,SUBSTRING(jsrq,0,9)) between '{0}' and '{1}' and (a.xm like '%{2}%' or a.jzlsh like '%{2}%') and {3} {4} {5}
                                        order by jsrq,xm",
                                        dtStart, dtEnd, sNAME, sQFXJE, jbr, strCBSF);

                ds = CliUtils.ExecuteSql("sybdj", "cmd", strSQL, CliUtils.fLoginDB.ToString(), true, CliUtils.fCurrentProject.ToString());

                //显示数据信息
                idgv_YbInfo.AutoGenerateColumns = false;
                idgv_YbInfo.DataSource = ds.Tables[0].DefaultView;
                idgv_YbInfo.ClearSelection();

            }
        }
        internal void getInfo2()
        {
            string dtStart = dtp_Start.Value.ToString("yyyy-MM-dd") + " 00:00:00";
            string dtEnd = dtp_End.Value.ToString("yyyy-MM-dd") + " 23:59:59";
            string sNAME = txtName.Text.Trim();
            string strSQL = string.Empty;
            string strSQL1 = string.Empty;
            string sQFXJE = "1=1";
            //if (ckQFXJE.Checked)
            //    sQFXJE = "a.tcjjzf='13.00'";

            //经办人信息
            string jbr = txtJBR.Tag.ToString();
            if (jbr.Equals("0"))
                jbr = "";
            else
                jbr = "and a.jbr='" + jbr + "'";

            string strCBSF = "";
            if (!string.IsNullOrEmpty(CBSF))
                strCBSF = "and a.cbsf='" + CBSF + "'";
            else
                strCBSF = "";

            DataSet ds = null;
            DataSet ds1 = null;
            if (MZLB.Equals("1"))
            {
                strSQL1 = string.Format(@"select jbr,b.b1name as jbrxm,cbsf,
                                        count(jbr) as bxrs,
                                        sum(ylfze)ylfze,sum(zbxje)zbxje,sum(tcjjzf)tcjjzf,sum(zhzf)zhzf,sum(xjzf)xjzf,
                                        sum(Convert(decimal(8,2),zgjbyljjzf))zgjbyljjzf,
                                        sum(Convert(decimal(8,2),czjmjbyljjzf))czjmjbyljjzf,
                                        sum(Convert(decimal(8,2),cxjmjjyljjzf))cxjmjjyljjzf,
                                        sum(Convert(decimal(8,2),dejjzf))dejjzf,
                                        sum(Convert(decimal(8,2),gwybzjjzf))gwybzjjzf,
                                        sum(Convert(decimal(8,2),qybcylbxjjzf))qybcylbxjjzf,
                                        sum(Convert(decimal(8,2),eyzxyljjzf))eyzxyljjzf,
                                        sum(Convert(decimal(8,2),lhjzxyljjzf))lhjzxyljjzf,
                                        sum(Convert(decimal(8,2),lxgbddtczf))lxgbddtczf,
                                        sum(Convert(decimal(8,2),ylbjzf))ylbjzf,
                                        sum(Convert(decimal(8,2),qtjjzf))qtjjzf,
                                        sum(Convert(decimal(8,2),gsjjzf))gsjjzf,
                                        sum(Convert(decimal(8,2),syjjzf))syjjzf,
                                        sum(Convert(decimal(8,2),mzjzfy))mzjzfy,
                                        sum(Convert(decimal(8,2),yyfdfy))yyfdfy 
                                        from ybfyjsdr a
                                        left join bz01h b on a.jbr=b.b1empn
                                        where cxbz=1 and len(jzlsh)=8
                                        and convert(datetime,SUBSTRING(jsrq,0,9)) between '{0}' and '{1}'  and (a.xm like '%{2}%' or a.jzlsh like '%{2}%') and {3} {4} {5}
                                        group by jbr,b1name,cbsf
                                        order by jbr,b1name", dtStart, dtEnd, sNAME, sQFXJE, jbr, strCBSF);
                ds1 = CliUtils.ExecuteSql("sybdj", "cmd", strSQL1, CliUtils.fLoginDB.ToString(), true, CliUtils.fCurrentProject.ToString());
                //显示数据信息
                idgv_cbsf.AutoGenerateColumns = false;
                idgv_cbsf.ClearSelection();
                idgv_cbsf.DataSource = ds1.Tables[0];

            }
            else
            {
                strSQL1 = string.Format(@"select jbr,b.b1name as jbrxm,cbsf,
                                        count(jbr) as bxrs,
                                        sum(ylfze)ylfze,sum(zbxje)zbxje,sum(tcjjzf)tcjjzf,sum(zhzf)zhzf,sum(xjzf)xjzf,
                                        sum(Convert(decimal(8,2),zgjbyljjzf))zgjbyljjzf,
                                        sum(Convert(decimal(8,2),czjmjbyljjzf))czjmjbyljjzf,
                                        sum(Convert(decimal(8,2),cxjmjjyljjzf))cxjmjjyljjzf,
                                        sum(Convert(decimal(8,2),dejjzf))dejjzf,
                                        sum(Convert(decimal(8,2),gwybzjjzf))gwybzjjzf,
                                        sum(Convert(decimal(8,2),qybcylbxjjzf))qybcylbxjjzf,
                                        sum(Convert(decimal(8,2),eyzxyljjzf))eyzxyljjzf,
                                        sum(Convert(decimal(8,2),lhjzxyljjzf))lhjzxyljjzf,
                                        sum(Convert(decimal(8,2),lxgbddtczf))lxgbddtczf,
                                        sum(Convert(decimal(8,2),ylbjzf))ylbjzf,
                                        sum(Convert(decimal(8,2),qtjjzf))qtjjzf,
                                        sum(Convert(decimal(8,2),gsjjzf))gsjjzf,
                                        sum(Convert(decimal(8,2),syjjzf))syjjzf,
                                        sum(Convert(decimal(8,2),mzjzfy))mzjzfy,
                                        sum(Convert(decimal(8,2),yyfdfy))yyfdfy 
                                        from ybfyjsdr a
                                        left join bz01h b on a.jbr=b.b1empn
                                        where cxbz=1 and len(jzlsh)=9
                                        and convert(datetime,SUBSTRING(jsrq,0,9)) between '{0}' and '{1}'  and (a.xm like '%{2}%' or a.jzlsh like '%{2}%') and {3} {4} {5}
                                        group by jbr,b1name,cbsf
                                        order by jbr,b1name", dtStart, dtEnd, sNAME, sQFXJE, jbr, strCBSF);

                ds1 = CliUtils.ExecuteSql("sybdj", "cmd", strSQL1, CliUtils.fLoginDB.ToString(), true, CliUtils.fCurrentProject.ToString());

                //显示数据信息
                idgv_cbsf.AutoGenerateColumns = false;
                idgv_cbsf.ClearSelection();
                idgv_cbsf.DataSource = ds1.Tables[0].DefaultView;
            }
        }


        private void btn_Find_Click(object sender, EventArgs e)
        {
            CBSF = "";
            getInfo();
            getInfo1();
            getInfo2();
        }

        private void cmbLB_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbLB.Text.Equals("住院"))
            {
                MZLB = "1";
                txtName.Text = "";
                CBSF = "";
            }
            else
            {
                MZLB = "0";
                txtName.Text = "";
                CBSF = "";
            }
        }

        private void btn_Excel_Click(object sender, EventArgs e)
        {
            #region 导出

            DataGridView dgv = idgv_YbInfo;
            if (dgv.ColumnCount == 0)
                return;
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "*.xls|*.xls";
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                InfoDataSet ids = new InfoDataSet();
                DataSet ds_copy = new DataSet();
                ids.RealDataSet = ds_copy;
                DataView dv = null;
                if (dgv.DataSource is DataTable)
                {
                    dv = (dgv.DataSource as DataTable).DefaultView;
                }
                else if (dgv.DataSource is DataView)
                    dv = dgv.DataSource as DataView;
                else if (dgv.DataSource is DataSet)
                {
                    dv = (dgv.DataSource as DataSet).Tables[0].DefaultView;
                }
                List<string> columns = new List<string>();
                List<string> titles = new List<string>();
                for (int i = 1; i < dgv.ColumnCount; i++)
                {
                    if (dgv.Columns[i].Visible == true)
                    {
                        columns.Add(dgv.Columns[i].DataPropertyName);
                        titles.Add(dgv.Columns[i].HeaderText);
                    }
                }
                ds_copy.Tables.Add(dv.ToTable(false, columns.ToArray()));
                for (int i = 0; i < columns.Count; i++)
                {
                    ds_copy.Tables[0].Columns[i].ColumnName = titles[i];
                }
                if (ids.ToExcel(0, dlg.FileName) == true)
                    MessageBox.Show("导出成功");
                ids.Dispose();
            }

            #endregion
        }

        private void btn_Close_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void idgv_YbInfo_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            //添加序号
            for (int i = 0; i < idgv_YbInfo.Rows.Count; i++)
            {
                idgv_YbInfo.Rows[i].Cells["col_seq"].Value = i + 1;//赋值
            }
        }

        private void txtJBR_KeyDown(object sender, KeyEventArgs e)
        {
            txtJBR.Text = "";
        }

        //打印结算费用单
        private void btn_JSD_Click(object sender, EventArgs e)
        {
            if (idgv_YbInfo.SelectedRows.Count == 0)
            {
                MessageBox.Show("请选中一个病人", "提示");
                return;
            }
            string zyno = idgv_YbInfo.CurrentRow.Cells["col_jzlsh"].Value.ToString();

            if (zyno.Length != 8)
            {
                MessageBox.Show("请选择住院患者");
                return;
            }
            object[] objParam = { zyno };
            object[] objReturn = yb_interface.ybs_interface("4404", objParam);
            MessageBox.Show(objReturn[2].ToString());
        }

        private void Frm_ybjsfz_Load(object sender, EventArgs e)
        {
            dtp_Start.Value = DateTime.Now;
            dtp_End.Value = DateTime.Now;
            txtJBR.Text = CliUtils.fUserName;

            #region 设置收费人员
            string strSql = string.Format(@"select '0' AS b1empn,'全部' as b1name union all
                                            select b1empn,b1name from bz01h where ISNULL(b1ybno,'')!='' ");
            DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            alvJBR.SetDataSource(ds.Copy());
            alvJBR.SetColumnParam.AddQueryColumn("b1name");
            alvJBR.SetColumnParam.SetValueColumn("b1empn");
            alvJBR.SetColumnParam.SetTextColumn("b1name");
            alvJBR.SetColumnParam.AddViewColumn("b1name", "姓 名", 100);
            txtJBR.Text = "全部";
            txtJBR.Tag = 0;
            #endregion
        }

        private void idgv_ybfyhz_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            //添加序号
            for (int i = 0; i < idgv_ybfyhz.Rows.Count; i++)
            {
                idgv_ybfyhz.Rows[i].Cells["col_seq1"].Value = i + 1;//赋值
            }
        }

        private void idgv_ybfyhz_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            txtJBR.Tag = this.idgv_ybfyhz.CurrentRow.Cells["col_jbrbh"].Value.ToString();
            txtJBR.Text = this.idgv_ybfyhz.CurrentRow.Cells["col_jbr1"].Value.ToString();
            CBSF = "";
            tabControl1.SelectedTab=tabPage3;
            getInfo1();
            getInfo2();
        }

        private void idgv_cbsf_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            //添加序号
            for (int i = 0; i < idgv_cbsf.Rows.Count; i++)
            {
                idgv_cbsf.Rows[i].Cells["col_seq2"].Value = i + 1;//赋值
            }

        }

        private void idgv_cbsf_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            CBSF = this.idgv_cbsf.CurrentRow.Cells["col_cbsf2"].Value.ToString();
            tabControl1.SelectedTab = tabPage1;
            getInfo1();
        }
    }
}
