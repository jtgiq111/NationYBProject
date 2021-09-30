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
    public partial class frm_ybjsfycxXG : InfoForm
    {
        private string MZLB = "1"; //住院门诊类别 1-住院 2-门诊

        public frm_ybjsfycxXG()
        {
            InitializeComponent();
        }

        private void btn_Find_Click(object sender, EventArgs e)
        {
            string dtStart = dtp_Start.Value.ToString("yyyy-MM-dd") + " 00:00:00";
            string dtEnd = dtp_End.Value.ToString("yyyy-MM-dd") + " 23:59:59";
            string sNAME = txtName.Text.Trim();
            string strSQL = string.Empty;
            string sQFXJE = "1=1";
            if (ckQFXJE.Checked)
                sQFXJE = "a.tcjjzf='13.00'";

            //经办人信息
            string jbr = txtJBR.Tag.ToString();
            if (jbr.Equals("0"))
                jbr = "";
            else
                jbr = "and a.jbr='" + jbr + "'";

            DataSet ds = null;
            if (MZLB.Equals("1"))
            {
                strSQL = string.Format(@"select a.jbr,'' as jbrbh,a.jzlsh,b.xm,b.grbh as sbh,b.ksmc as kb,a.ylfze as fyhj,
                                        a.zbxje as bxje,a.tcjjzf as bxhj,a.zhzf as zfhj,a.xjzf,a.sysdate as bxrq,qfbzfy as qfxje,
                                        '' as mlyfy,c.DWMC,c.ZYCS,'' AS zcfbz,b.bzbm,b.bzmc,a.dejjzf as dbjjzf,a.mzjzfy as jzje,
                                        a.dwfdfy as dwfdfy,a.lxgbddtczf as lxgbddtczf,a.zdjbfwnbcje as zdjbfwnbcje,a.zdjbfwybcje as zdjbfwybcje,
                                        a.yyfdfy as yyfdfy,a.ecbcje as ecbcje,a.mzdbjzje as mzdbjzje,a.czzcje as zftdje
                                        from ybfyjsdr a
                                        left join ybmzzydjdr b on a.jzlsh=b.jzlsh and a.cxbz=b.cxbz
                                        left join ybickxx c on a.grbh=c.GRBH  
                                        where a.cxbz=1 and len(a.jzlsh)=8 and b.sysdate between '{0}' and '{1}' and a.xm like '%{2}%' and {3} {4}",
                                        dtStart, dtEnd, sNAME, sQFXJE, jbr);

                ds = CliUtils.ExecuteSql("sybdj", "cmd", strSQL, CliUtils.fLoginDB.ToString(), true, CliUtils.fCurrentProject.ToString());
                //显示数据信息
                idgv_YbInfo.AutoGenerateColumns = false;
                idgv_YbInfo.DataSource = ds.Tables[0];
            }
            else
            {
                strSQL = string.Format(@"select a.jbr,'' as jbrbh,a.jzlsh,b.xm,b.grbh as sbh,b.ksmc as kb,a.ylfze as fyhj,
                                        (ylfze-xjzf) as bxje,a.tcjjzf as tczf,a.zhzf as zfhj,a.xjzf,a.sysdate as bxrq,qfbzfy as qfxje,
                                        '' as mlyfy,c.DWMC,c.ZYCS,'' AS zcfbz,b.mmbzbm1 as bzbm,b.mmbzmc1 as bzmc,a.dejjzf as dbjjzf,a.mzjzfy as jzje,
                                        a.dwfdfy as dwfdfy,a.lxgbddtczf as lxgbddtczf,a.zdjbfwnbcje as zdjbfwnbcje,a.zdjbfwybcje as zdjbfwybcje,
                                        a.yyfdfy as yyfdfy,a.ecbcje as ecbcje,a.mzdbjzje as mzdbjzje,a.czzcje as zftdje
                                        from ybfyjsdr a
                                        left join ybmzzydjdr b on a.jzlsh=b.jzlsh and a.cxbz=b.cxbz
                                        left join ybickxx c on a.grbh=c.GRBH 
                                        where a.cxbz=1 and len(a.jzlsh)=9 and b.sysdate between '{0}' and '{1}' and a.xm like '%{2}%' and {3} {4}",
                                        dtStart, dtEnd, sNAME, sQFXJE, jbr);
               
                ds = CliUtils.ExecuteSql("sybdj", "cmd", strSQL, CliUtils.fLoginDB.ToString(), true, CliUtils.fCurrentProject.ToString());

                //显示数据信息
                idgv_YbInfo.AutoGenerateColumns = false;
                idgv_YbInfo.DataSource = ds.Tables[0].DefaultView ;
            }
        }

        private void frm_ybjsfycx_Load(object sender, EventArgs e)
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
            txtJBR.Text="全部";
            txtJBR.Tag=0;
            #endregion
        }

        private void cmbLB_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbLB.Text.Equals("住院"))
            {
                MZLB = "1";
                idgv_YbInfo.Columns["col_kb"].HeaderText = "科别";
                txtName.Text = "";
            }
            else
            {
                MZLB = "0";
                ckQFXJE.Checked = false;
                idgv_YbInfo.Columns["col_kb"].HeaderText = "持卡人姓名";
                txtName.Text = "";
            }
        }

        private void btn_Excel_Click(object sender, EventArgs e)
        {
            #region 导出
            //*******************
            if (idgv_YbInfo.Rows.Count < 1)
            {
                DialogResult dr2 = MessageBox.Show("没有任何数据，无法导出！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            DialogResult dr = MessageBox.Show("确认导出Excle吗?", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (dr != DialogResult.Yes) return;
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Execl files (*.xls)|*.xls";
            saveFileDialog.Filter = "Execl files (*.xls)|*.xls";
            saveFileDialog.FilterIndex = 0;
            saveFileDialog.RestoreDirectory = true;
            saveFileDialog.CreatePrompt = true;
            saveFileDialog.FileName = "医保报销数据查询" + DateTime.Now.ToShortDateString();
            saveFileDialog.Title = "Export Excel File To";
            string filenamexls = "医保报销数据查询" + DateTime.Now;
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {

                //获得文件路径
                filenamexls = saveFileDialog.FileName.ToString();
                Stream myStream;
                myStream = saveFileDialog.OpenFile();
                StreamWriter sw = new StreamWriter(myStream, System.Text.Encoding.GetEncoding(-0));
                string str = "";
                try
                {
                    //写标题
                    for (int i = 0; i < this.idgv_YbInfo.ColumnCount; i++)
                    {
                        if (i > 0)
                        {
                            str += "\t";
                        }
                        str += idgv_YbInfo.Columns[i].HeaderText;
                    }
                    sw.WriteLine(str);
                    //写内容  
                    for (int j = 0; j < idgv_YbInfo.Rows.Count; j++)
                    {
                        string tempStr = "";
                        for (int k = 0; k < idgv_YbInfo.Columns.Count; k++)
                        {
                            if (k > 0)
                            {
                                tempStr += "\t";
                            }
                            tempStr += idgv_YbInfo.Rows[j].Cells[k].Value.ToString();
                        }
                        sw.WriteLine(tempStr);
                    }
                    //sw.Close();
                    //myStream.Close();
                }
                catch
                {
                    MessageBox.Show(e.ToString());
                }
                finally
                {
                    sw.Close();
                    myStream.Close();
                }
            }
            System.Diagnostics.Process.Start("excel.exe", filenamexls);
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
                idgv_YbInfo.Rows[i].Cells["seq"].Value = i + 1;//赋值
            }
        }

        private void txtJBR_KeyDown(object sender, KeyEventArgs e)
        {
            txtJBR.Text = "";
        }

        //打印结算单
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
            object[] objParam = { zyno};
            object[] objReturn=yb_interface.ybs_interface("4403", objParam);
            MessageBox.Show(objReturn[2].ToString());
        }

    }
}
