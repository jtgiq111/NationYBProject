using Srvtools;
using System;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace ybinterface_lib
{
    public partial class Frm_ybhzcxdrjj : InfoForm
    {
        GocentPara Para = new GocentPara();
        DataTable dt;

        public Frm_ybhzcxdrjj()
        {
            InitializeComponent();
            //dgvbf.AutoGenerateColumns = false;
            this.dgv_ybhzcx_cvs.AutoGenerateColumns = false;
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void btncx_Click(object sender, EventArgs e)
        {
            string sql = @"select d.z1amt2, left(d.z1date, 10) ryrq, a.sysdate djsj, 
            isnull(c.bzname, '') tcq, '已登记' djzt, isnull(d.z1lynm, '') lyjg, '' ch, isnull(d.z1bqnm, '') bqmc, 
            isnull(e.bzname, '') yllbmc, isnull(f.bzname, '') yldylbmc
            , a.jzlsh, a.xm, isnull(a.ksmc, '') ksmc, a.grbh, a.bntczclj, a.ybjzlsh, a.kh, a.dwmc, a.bzbm, a.bzmc from ybmzzydjdr a join bztbd c on c.bzkeyx = a.tcqh and c.bzcodn = 'DQ'
            join zy01h d on d.z1zyno = a.jzlsh left join bztbd e on e.bzkeyx = a.yllb 
            and e.bzcodn = 'YL' left join bztbd f on f.bzkeyx = a.yldylb and f.bzcodn = 'DL'";

            if (chk_cy.Checked)
            {
                sql += " join ybfyjsdr b on a.jzlsh = b.jzlsh where 1 = 1";
            }
            else
            {
                sql += " where not exists (select 1 from ybfyjsdr b where b.jzlsh = a.jzlsh and b.cxbz = 1)";
            }

            sql += " and a.cxbz = 1";

            if (chk_cy.Checked)
            {
                sql += " and b.cxbz = 1";

                //    if (rbtn_decy.Checked)
                //{
                //    sql += " and b.dejjzf > 0";
                //}
            }

            sql += string.Format(" and a.sysdate between '{0}' and '{1}'", dtp_start.Value.ToString("yyyy-MM-dd"), dtp_end.Value.AddDays(1).ToString("yyyy-MM-dd"));

            if (!string.IsNullOrWhiteSpace(txt_lsh.Text))
            {
                sql += string.Format(" and a.jzlsh = '{0}'", txt_lsh.Text.Trim());
            }

            sql += " order by a.sysdate";
            dt = CliUtils.ExecuteSql("sybdj", "cmd", sql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject).Tables[0];
            //dgvbf.DataSource = dt;
            dgv_ybhzcx_cvs.DataSource = dt;
        }

        private void btnDC_Click(object sender, EventArgs e)
        {
            string filename = "九江市中医院医保患者查询报表.xls";
            ColumnMap[] maps = { new ColumnMap("jzlsh", "住院号")
                               , new ColumnMap("xm", "姓名")
                               , new ColumnMap("ch", "床号")
                               , new ColumnMap("ksmc", "科室")
                               , new ColumnMap("bqmc", "病区")
                               , new ColumnMap("ryrq", "入院日期")
                               , new ColumnMap("grbh", "个人编号")
                               , new ColumnMap("djzt", "登记状态")
                               , new ColumnMap("lyjg", "来源机构")
                               , new ColumnMap("z1amt2", "费用总额")
                               , new ColumnMap("bntczclj", "本年统筹累计")
                               , new ColumnMap("ybjzlsh", "医保住院号")
                               , new ColumnMap("kh", "卡号")
                               , new ColumnMap("dwmc", "单位")
                               , new ColumnMap("bzbm", "病种编码")
                               , new ColumnMap("bzmc", "病种名称")
                               , new ColumnMap("yllbmc", "医疗类别")
                               , new ColumnMap("yldylbmc", "待遇类别")
                               , new ColumnMap("tcq", "统筹区")
                               , new ColumnMap("djsj", "登记时间")
                               };

            try
            {
                DataTableToExcel.Dt2File(dt, maps, filename, true);
                Process.Start("excel.exe", Environment.CurrentDirectory + "\\" + filename);
            }
            catch (Exception error)
            {
                MessageBox.Show(error.ToString());
            }
        }

        private void btn_Close_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void txt_lsh_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                txt_lsh.Text = txt_lsh.Text.PadLeft(8, '0');
                btncx_Click(null, null);
            }
        }

        private void btn_dccvs_Click(object sender, EventArgs e)
        {
            //string filename = "九江市中医院医保患者查询报表";

            try
            {
                //Common.Dcexcel(this.dgv_ybhzcx_cvs, filename);
                InfoDataSet ids = new InfoDataSet();
                ids.RealDataSet = Common.GetDataSetFromDataGridView(dgv_ybhzcx_cvs);
                SaveFileDialog dlg = new SaveFileDialog();
                dlg.AutoUpgradeEnabled = false;
                dlg.Filter = "*.xls|*.xls";

                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    if (ids.ToExcel(0, dlg.FileName))
                    {
                        MessageBox.Show("导出成功");
                    }
                    else
                    {
                        MessageBox.Show("导出失败");
                    }
                }
            }
            catch (Exception error)
            {
                MessageBox.Show(error.ToString());
            }
        }
    }
}

