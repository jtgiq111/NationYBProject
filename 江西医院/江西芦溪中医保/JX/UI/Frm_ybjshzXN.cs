using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Srvtools;

namespace yb_interfaces.JX.UI
{
    public partial class Frm_ybjshzXN : InfoForm
    {
        private string mzlb = "1"; //住院门诊类别 1-住院 2-门诊
        private string yblx = "0"; //0-全部 1-居民 2-职工 3-离休 4-一卡通
        private string tklx = "0";

        public Frm_ybjshzXN()
        {
            InitializeComponent();
        }

        private void Frm_ybjshzJA_Load(object sender, EventArgs e)
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

            #region 设置科室信息
            strSql = string.Format(@"select '0' b2ejks,'全部' as b2ejnm,'QB' as b2pymx,'WU' as b2wbmx union all
                                    select b2ejks,b2ejnm,b2pymx,b2wbmx from bz02d  where b2mark='Z'");
            ds.Tables[0].Clear();
            ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            alvKS.SetDataSource(ds.Copy());
            alvKS.SetColumnParam.AddQueryColumn("b2ejnm");
            alvKS.SetColumnParam.AddQueryColumn("b2pymx");
            alvKS.SetColumnParam.AddQueryColumn("b2wbmx");
            alvKS.SetColumnParam.SetValueColumn("b2ejks");
            alvKS.SetColumnParam.SetTextColumn("b2ejnm");
            alvKS.SetColumnParam.AddViewColumn("b2ejnm", "科  室", 100);
            txtKS.Text = "全部";
            txtKS.Tag = 0;
            #endregion
        }

        private void btn_Find_Click(object sender, EventArgs e)
        {
            getInfo();

        }

        internal void getInfo()
        {
            #region  门诊/住院
            string sMZLX = "";
            if (mzlb.Equals("1"))
                sMZLX += " and len(a.jzlsh)=8 ";
            else
                sMZLX += " and len(a.jzlsh)=9 ";
            #endregion

            #region 患者信息
            string sHZXM = "";
            if (!string.IsNullOrEmpty(txtName.Text.Trim()))
                sHZXM = " and a.xm like '" + txtName.Text.Trim() + "%' ";
            #endregion

            #region 经办人
            string sJBR = "";
            if (!txtJBR.Text.Equals("全部") && !string.IsNullOrEmpty(txtJBR.Text.Trim()))
                sJBR = " and a.jbr='" + txtJBR.Tag + "' ";
            #endregion

            #region 科室
            string sKSNO = "";
            if (!txtKS.Text.Equals("全部") && !string.IsNullOrEmpty(txtKS.Text.Trim()))
                sKSNO = " and b.ksbh='" + txtKS.Tag + "' ";
            #endregion

            #region 医保类型
            string sYBLX = "";
            switch (yblx)
            {
                case "0":
                    sYBLX = "";
                    break;
                case "1":
                    sYBLX = " and (b.yldylb like '居民%' and b.tcqh='360821') ";
                    break;
                case "2":
                    sYBLX = " and (b.yldylb like '职工-在职%' and b.tcqh='360821') ";
                    break;
                case "5":
                    sYBLX = " and (b.yldylb like '职工-退休%' and b.tcqh='360821') ";
                    break;
                case "3":
                    sYBLX = " and (b.yldylb like '离休%' and b.tcqh='360821') ";
                    break;
                case "4":
                    sYBLX = " and b.tcqh!='360821' ";
                    break;
                case "6":
                    sYBLX = " and ";
                    break;
                default:
                    sYBLX = "";
                    break;
            }
            #endregion

            #region 退款类型
            string sTSLX = "";
            switch (tklx)
            {
                case "0":
                    sTSLX = "";
                    break;
                case "1":
                    sTSLX = " and left(i.z3stbz,1)=1 and i.z3ysje!=0 ";
                    break;
                case "2":
                    sTSLX = " and left(i.z3stbz,1)=0 ";
                    break;
                case "3":
                    sTSLX = " and i.z3ysje=0 ";
                    break;
                default:
                    sTSLX = "";
                    break;
            }
            #endregion

            #region 获取汇总数据
            string strSql = string.Format(@"select
                                            convert(date,a.jsrq) as bxrq,
                                            f.b1name as jbr,
                                            a.jzlsh, 
                                            a.xm,
                                            a.grbh,
                                            e.SJNL,
                                            e.GMSFHM,
                                            case when len(a.jzlsh)=8 then convert(date,substring(left(a.ryrq,8)+' ' + substring(a.ryrq,9,2)+':' + substring(a.ryrq,11,2),1,120)) else a.jsrq end as ryrqsj,
                                            case when len(a.jzlsh)=8 then convert(date,substring(left(a.cyrq,8)+' ' + substring(a.cyrq,9,2)+':' + substring(a.cyrq,11,2),1,120)) else a.jsrq end  as cyrqsj,
                                            case when len(a.jzlsh)=8 then a.yllb
                                            else h.NAME end as yllb,
                                            b.bzmc,
                                            DATEDIFF(day,c.z1date,c.z1ldat) as zyts,
                                            b.yldylb,
                                            b.ksmc,
                                            b.dgysxm as ysxm,
                                            g.name as tcqx,
                                            convert(decimal(8,2),i.z3amt1) as yjfy,
                                            i.z3stbz,
                                            convert(decimal(8,2),i.z3ysje) as z3ysje,
                                            case when DATEDIFF(day,c.z1date,c.z1ldat) in (select code  from ybbzfyzd) then (select value1 from ybbzfyzd where code=DATEDIFF(day,c.z1date,c.z1ldat))
                                            else ((select value1 from ybbzfyzd where code=10)+(DATEDIFF(day,c.z1date,c.z1ldat)-10)*180) end  bzfy,
                                            case when DATEDIFF(day,c.z1date,c.z1ldat) in (select code  from ybbzfyzd) and b.yldylb like '居民%'  then ((select value1 from ybbzfyzd where code=DATEDIFF(day,c.z1date,c.z1ldat))-a.qfbzfy)*0.8
                                            when DATEDIFF(day,c.z1date,c.z1ldat) not in (select code  from ybbzfyzd) and b.yldylb like '居民%' then (((select value1 from ybbzfyzd where code=10)+(DATEDIFF(day,c.z1date,c.z1ldat)-10)*180)-a.qfbzfy)*0.8
                                            when DATEDIFF(day,c.z1date,c.z1ldat) in (select code  from ybbzfyzd) and b.yldylb like '职工%'  then ((select value1 from ybbzfyzd where code=DATEDIFF(day,c.z1date,c.z1ldat))-a.qfbzfy)*0.9
                                            when DATEDIFF(day,c.z1date,c.z1ldat) not in (select code  from ybbzfyzd) and b.yldylb like '职工%' then (((select value1 from ybbzfyzd where code=10)+(DATEDIFF(day,c.z1date,c.z1ldat)-10)*180)-a.qfbzfy)*0.9
                                            else (((select value1 from ybbzfyzd where code=10)+(DATEDIFF(day,c.z1date,c.z1ldat)-10)*180)-a.qfbzfy)*0.8 end  bcfy,
                                            isnull(a.ylfze,0.00) as ylfze, 
                                            isnull(a.tcjjzf,0.00) as tcjj,
                                            isnull(a.zhzf,0.00) as zfzf,
                                            isnull(a.xjzf,0.00) as xjzf,
                                            isnull(a.dejjzf,0.00) as dejj,
                                            isnull(a.mzjzfy,0.00) as mzjz,
                                            isnull(a.dwfdfy,0.00) as dwfd,
                                            isnull(a.lxgbddtczf,0.00) as lxgbtczf,
                                            isnull(a.zdjbfwnbcje,0.00) as zdjbnb,
                                            isnull(a.zdjbfwybcje,0.00) as zdjbyb,
                                            isnull(a.yyfdfy,0.00) as yyfd,
                                            isnull(a.ecbcje,0.00) as ecbcje,
                                            isnull(a.mzdbjzje,0.00) as mzdbjz,
                                            isnull(a.czzcje,0.00) as zftd,
                                            isnull(a.gwybzjjzf,0.00) as gwybzjjzf
                                            from ybfyjsdr a
                                            left join ybmzzydjdr b on a.jzlsh=b.jzlsh and a.cxbz=b.cxbz
                                            left join zy01h c on a.jzlsh=c.z1zyno
                                            left join zy03dw d on a.jzlsh=d.z3zyno and left(z3endv,1)=1
                                            left join ybickxx e on a.grbh=e.GRBH
                                            left join bz01h  f on a.jbr=f.b1empn
                                            left join zy03dw i on a.jzlsh=i.z3zyno and a.djh=i.z3jshx and left(i.z3endv,1)=1
                                            left join YBXMLBZD g on b.tcqh=g.CODE and g.LBMC='地区编码'
                                            left join YBXMLBZD h on b.yllb=h.CODE and h.LBMC='医疗类别'
                                            where a.cxbz=1 and convert(date,jsrq) between '{0}' and '{1}' {2} {3} {4} {5} {6} {7}
                                            order by convert(date,a.jsrq) ",
                                            dtp_Start.Text, dtp_End.Text, sMZLX, sJBR, sHZXM, sKSNO, sYBLX, sTSLX);
            #endregion
            DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            idgv_YbInfo.AutoGenerateColumns = false;
            idgv_YbInfo.ClearSelection();
            idgv_YbInfo.DataSource = ds.Tables[0].DefaultView;

        }

        private void alvJBR_AfterConfirm(object sender, EventArgs e)
        {
            if (alvJBR.currentDataRow != null)
            {
                txtJBR.Tag = alvJBR.currentDataRow["b1empn"].ToString();
                txtJBR.Text = alvJBR.currentDataRow["b1name"].ToString();
            }
        }

        private void alvKS_AfterConfirm(object sender, EventArgs e)
        {
            if (alvKS.currentDataRow != null)
            {
                txtKS.Tag = alvKS.currentDataRow["b2ejks"].ToString();
                txtKS.Text = alvKS.currentDataRow["b2ejnm"].ToString();
            }
        }

        private void cmbLB_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbLB.Text.Equals("住院"))
            {
                mzlb = "1";
                txtName.Text = "";
            }
            else
            {
                mzlb = "0";
                txtName.Text = "";
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.Text.Equals("全部"))
            {
                yblx = "0";
                txtName.Text = "";
            }
            else if (comboBox1.Text.Equals("居民"))
            {
                yblx = "1";
                txtName.Text = "";
            }
            else if (comboBox1.Text.Equals("职工-在职"))
            {
                yblx = "2";
                txtName.Text = "";
            }
            else if (comboBox1.Text.Equals("离休"))
            {
                yblx = "3";
                txtName.Text = "";
            }
            else if (comboBox1.Text.Equals("一卡通"))
            {
                yblx = "4";
                txtName.Text = "";
            }
            else if (comboBox1.Text.Equals("职工-退休"))
            {
                yblx = "5";
                txtName.Text = "";
            }
            else if (comboBox1.Text.Equals("单病种住院"))
            {
                yblx = "6";
                txtName.Text = "";
            }
            else
            {
                yblx = "-1";
                txtName.Text = "";
            }
        }

        private void idgv_YbInfo_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            //添加序号
            for (int i = 0; i < idgv_YbInfo.Rows.Count; i++)
            {
                idgv_YbInfo.Rows[i].Cells["col_seq"].Value = i + 1;//赋值
            }

        }

        private void btn_Excel_Click(object sender, EventArgs e)
        {
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
        }

        private void btn_Close_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox2.Text.Equals("0.全部"))
            {
                tklx = "0";
                txtName.Text = "";
            }
            else if (comboBox2.Text.Equals("1.退回款"))
            {
                tklx = "1";
                txtName.Text = "";
            }
            else if (comboBox2.Text.Equals("2.补交款"))
            {
                tklx = "2";
                txtName.Text = "";
            }
            else if (comboBox2.Text.Equals("3.无退补款"))
            {
                tklx = "3";
                txtName.Text = "";
            }
            else
            {
                tklx = "-1";
                txtName.Text = "";
            }
        }
        public void Showthis()
        {
            this.ShowDialog();
        }
    }
}
