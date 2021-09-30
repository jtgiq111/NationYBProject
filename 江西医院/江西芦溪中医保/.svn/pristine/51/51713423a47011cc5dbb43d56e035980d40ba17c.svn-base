using FastReport;
using Srvtools;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace yb_interfaces.JX.UI
{
    public partial class Frm_ybjshz1XN : InfoForm
    {
        private string MZLB = "1"; //1-医保结算 2-自费结算
        private string strWhere = string.Empty; //条件
        private string dtStart = string.Empty;
        private string dtEnd = string.Empty;
        DataSet ds = null;
        DataSet ds1 = null;
        private string brxz = string.Empty;
        public Frm_ybjshz1XN()
        {
            InitializeComponent();
        }

        private void Frm_ybjshz1_Load(object sender, EventArgs e)
        {
            dtp_Start.Value = DateTime.Now;
            dtp_End.Value = DateTime.Now;
            #region 绑定操作员信息
            alv_JBR.SetColumnParam.AddQueryColumn("b1pymx");
            alv_JBR.SetColumnParam.AddQueryColumn("b1name");
            alv_JBR.SetColumnParam.SetValueColumn("b1empn");
            alv_JBR.SetColumnParam.SetTextColumn("b1name");
            alv_JBR.SetColumnParam.AddViewColumn("b1empn", "人员编号", 80);
            alv_JBR.SetColumnParam.AddViewColumn("b1name", "人员姓名", 100);
            string strSQL = string.Format(@"SELECT b1empn,b1name,b1pymx FROM bz01h where ISNULL(b1ybno,'')!=''
                            union all 
                            select '0','全部','QB' ");
            DataSet ds = CliUtils.ExecuteSql("szy01", "cmd", strSQL, CliUtils.fLoginDB.ToString(), true, CliUtils.fCurrentProject.ToString());
            alv_JBR.SetDataSource(ds.Copy());
            txtJBR.Tag = CliUtils.fLoginUser;
            txtJBR.Text = CliUtils.fUserName;
            #endregion
        }

        internal void getInfo()
        {
            dtStart = dtp_Start.Value.ToString("yyyy-MM-dd") + " 00:00:00";
            dtEnd = dtp_End.Value.ToString("yyyy-MM-dd") + " 23:59:59";
            if (cmbLB.Text.Equals("医保结算"))
            {
                strWhere = "and z3amtj!=0";
                brxz = "医保";
            }
            else if (cmbLB.Text.Equals("自费结算"))
            {
                strWhere = "and z3amtj=0";
                brxz = "自费";
            }
            else
            {
                strWhere = "";
                brxz = "医保+自费";
            }

            if (!txtJBR.Tag.ToString().Equals("0"))
            {
                strWhere += " and z3user='"+txtJBR.Tag+"' ";
            }

            string strSql = string.Format(@"select z3zyno,z1hznm,z3amt1,z3amtz,(z3amtz-z3amtj) xjzf,z3amtj,
                                            case when left(z3stbz,1)=1 then -z3amnt else z3amnt end as sfje,
                                            isnull(zhzf,0) as zhzf,0 as qfje,0 as jdcs,convert(date,z1date) as z1date,convert(date,z1ldat) as z1ldat,z3zxrq,CONVERT(date,jsrq) as jsrq
                                            from zy03dw 
                                            left join zy01h on z3zyno=z1zyno 
                                            left join ybfyjsdr on z3zyno=jzlsh and z1zyno=jzlsh and cxbz=1
                                            where z3endv like '1%' 
                                            and CONVERT(datetime,z3dat1) between  '{0}' and  '{1}'
                                            {2} order by z3dat1 desc", dtStart, dtEnd, strWhere);
            ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            //MessageBox.Show(strSql);
            idgv_zyjssf.AutoGenerateColumns = false;
            idgv_zyjssf.ClearSelection();
            idgv_zyjssf.DataSource = ds.Tables[0].DefaultView;

            strSql = string.Format(@"with jsinfo as
                                    ( 
                                    select z3zyno,z1hznm,z3amt1,z3amtz,(z3amtz-z3amtj) xjzf,z3amtj,
                                    case when left(z3stbz,1)=1 then -z3amnt else z3amnt end as sfje,
                                    isnull(zhzf,0) as zhzf,0 as qfje,0 as jdcs,z1date,z1ldat,z3zxrq,CONVERT(date,jsrq) as jsrq
                                    from zy03dw 
                                    left join zy01h on z3zyno=z1zyno 
                                    left join ybfyjsdr on z3zyno=jzlsh and z1zyno=jzlsh and cxbz=1
                                    where z3endv like '1%' 
                                    and CONVERT(datetime,z3dat1) between  '{0}' and  '{1}'
                                    {2}
                                    )
                                    select SUM(z3amt1) z3amt1,SUM(z3amtz)z3amtz,SUM(xjzf)xjzf,SUM(ABS(sfje))sfje,SUM(zhzf)zhzf
                                    ,SUM(z3amtj)z3amtj,SUM(qfje)qfje from jsinfo", dtStart, dtEnd, strWhere);
            ds1 = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);

            if (ds1.Tables[0].Rows.Count > 0)
            {
                lbl_ylzfy.Text = string.Format("{0:N2}",ds1.Tables[0].Rows[0]["z3amtz"].ToString());
                lbl_yjjhj.Text = string.Format("{0:N2}",ds1.Tables[0].Rows[0]["z3amt1"].ToString());
                lbl_sfjehj.Text = string.Format("{0:N2}",ds1.Tables[0].Rows[0]["sfje"].ToString());
                lbl_zffyhj.Text = string.Format("{0:N2}",ds1.Tables[0].Rows[0]["xjzf"].ToString());
                lbl_zhhj.Text = string.Format("{0:N2}",ds1.Tables[0].Rows[0]["zhzf"].ToString());
                lbl_tczfhj.Text = string.Format("{0:N2}",ds1.Tables[0].Rows[0]["z3amtj"].ToString());
                lbl_qffyhj.Text = string.Format("{0:N2}", ds1.Tables[0].Rows[0]["qfje"].ToString());
            }
        }

        private void btn_Find_Click(object sender, EventArgs e)
        {
            getInfo();
        }

        private void idgv_zyjssf_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            //添加序号
            for (int i = 0; i < idgv_zyjssf.Rows.Count; i++)
            {
                idgv_zyjssf.Rows[i].Cells["col_seq"].Value = i + 1;//赋值
            }
        }

        private void btn_Print_Click(object sender, EventArgs e)
        {
            if (ds.Tables[0].Rows.Count == 0)
            {
                MessageBox.Show("没有打印数据!");
                return;
            }
            ds.Tables[0].TableName = "jsinfo";
            ds.WriteXmlSchema(@"D:\mm.xml");
            string s_path = string.Empty;
            s_path = string.IsNullOrEmpty(s_path) ? @"C:\Program Files\Infolight\EEP2012\EEPNetClient\FastReport" : s_path;//空值取默认

            string c_file = Application.StartupPath + @"\FastReport\YB\住院结算收付报表_新干.frx"; //client
            string s_file = s_path + @"\YB\住院结算收付报表_新干.frx";    //server  
            CliUtils.DownLoad(s_file, c_file);
            //检查报表文件是否存
            if (!File.Exists(c_file))
            {
                ds.Dispose();
            }
            else
            {
                Report report = new Report();
                report.PrintSettings.ShowDialog = false;
                report.Load(c_file);
                report.RegisterData(ds);
                report.SetParameterValue("start", dtStart);//开始时间
                report.SetParameterValue("end", dtEnd);//结束时间
                report.SetParameterValue("brxz", brxz);//病人性质

                report.SetParameterValue("ylzfy", string.Format("{0:N2}",lbl_ylzfy.Text));//医疗总费用
                report.SetParameterValue("yjxjhj", string.Format("{0:N2}", lbl_yjjhj.Text));//预交金合计
                report.SetParameterValue("sfjehj", string.Format("{0:N2}", lbl_sfjehj.Text));//收付金额合计
                report.SetParameterValue("grzffyhj", string.Format("{0:N2}", lbl_zffyhj.Text));//个人自付费用合计:[grzffyhj]
                report.SetParameterValue("zhhj", string.Format("{0:N2}", lbl_zhhj.Text));//账户合计：[zhhj]
                report.SetParameterValue("tczfzfy", string.Format("{0:N2}", lbl_tczfhj.Text));//统筹支付总费用:[tczfzfy]
                report.SetParameterValue("cyqffyhj", string.Format("{0:N2}", lbl_qffyhj.Text));//出院欠费费用合计:[cyqffyhj]

                report.Show();
                report.Dispose();
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void alv_JBR_AfterConfirm(object sender, EventArgs e)
        {

        }

        private void btnExcel_Click(object sender, EventArgs e)
        {
            DataGridView dgv = idgv_zyjssf;
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
        }


    }
}
