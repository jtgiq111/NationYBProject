using FastReport;
using Srvtools;
using System;
using System.Data;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;

namespace ybinterface_lib
{
    public partial class Frm_ybbxhzdrjj : InfoForm
    {
        GocentPara Para = new GocentPara();
        string yldylb = "";
        DataSet ds;
        DataSet dsmx;

        public Frm_ybbxhzdrjj()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void btnprint_Click(object sender, EventArgs e)
        {

            string s_path = Para.sys_parameter("BZ", "BZ0007");
            s_path = string.IsNullOrEmpty(s_path) ? @"C:\Program Files (x86)\Infolight\EEP2012\EEPNetClient\FastReport" : s_path;//空值取默认
            string c_file = Application.StartupPath + @"\FastReport\ZY\医疗社会保险结算拨付汇总表jj.frx"; //client
            string s_file = s_path + @"\ZY\医疗社会保险结算拨付汇总表jj.frx";    //server             
            CliUtils.DownLoad(s_file, c_file);   //下载远端AP SERVER报表文件
            
            //检查报表文件是否存
            if (!File.Exists(c_file))
            {
                MessageBox.Show(c_file + "不存在!");
                return;
            }


            DataTable table = ds.Tables[0].Copy();
            DataRow row = table.Rows[table.Rows.Count-1];
            string tcjjzf = row["tcjjzf"].ToString();
            string zhzf = row["zhzf"].ToString();
            string dejjzf = row["dbzf"].ToString();
            decimal je = 0;

            if(rbtnjm.Checked)
                je = Convert.ToDecimal(tcjjzf) + Convert.ToDecimal(zhzf);
            else
                je = Convert.ToDecimal(tcjjzf) + Convert.ToDecimal(zhzf) + Convert.ToDecimal(dejjzf);
            

            Report report = new Report();
            report.Load(c_file);
            report.RegisterData(ds);
            report.SetParameterValue("start", dateTimeStar.Value.ToString("yyyy-MM-dd"));
            report.SetParameterValue("end", dateTimeEnd.Value.ToString("yyyy-MM-dd"));
            report.SetParameterValue("zbsj", DateTime.Now.ToString("yyyy-MM-dd"));
            report.SetParameterValue("yldylb", yldylb);
            report.SetParameterValue("je", je.ToString());
            report.SetParameterValue("rmbdx", Common.MoneyToUpper(je.ToString()));
            report.Show();
            report.Dispose();

            
        }

        private void btncx_Click(object sender, EventArgs e)
        {
            string yldylbs = "";
            string location = "";
            string where = "";

            if (this.rbtnjm.Checked)
            {
                yldylbs = " and left(a.yldylb, 1) in ('4', '5', '6', '7', '8', '9') and a.yldylb != '99' and a.yldylb not in ('57','58','59')";
                yldylb = rbtnjm.Text;
            }
            else if (this.rbtnlx.Checked)
            {
                yldylbs = " and a.yldylb in ('25', '31', '34')";
                yldylb = rbtnlx.Text;
            }
            else if (this.rbtnzg.Checked)
            {
                yldylbs =" and (left(a.yldylb, 1) in ('1', '2') or a.yldylb = '99') and a.yldylb not in ('23', '25')";
                yldylb = rbtnzg.Text;
            }
            else if (this.rbtnKMYC.Checked)
            {
                yldylbs = " and a.yldylb in ('23')";
                yldylb = rbtnKMYC.Text;
            }
            else if (this.rbtnSCJR.Checked)
            {
                yldylbs = " and a.yldylb in ('33','35','36','37','38','57','58','59')"; 
                yldylb = rbtnSCJR.Text;
            }

            if (this.rbtn_Local.Checked)
            {
                location = "and d.tcqh = 360430";
            }
            else if (this.rbtn_Country.Checked)
            {
                location = "and d.tcqh !=360430";
            }

            if (this.rbtn_mz.Checked)
            {
                where = "and d.jzbz = 'm'";
            }
            else if (this.rbtn_zy.Checked)
            {
                where = "and d.jzbz = 'z'";
            }
            else if (this.rbtn_all.Checked)
            {
                where = "";
            }

            if (this.rbtn_TC.Checked)
            {
                where += "and a.tcjjzf != 0";
            }
            else if (this.rbtn_ZH.Checked)
            {
                where += "and a.zhzf != 0";
            }
            

            string sql = string.Format(@"select b.bzname yllb 
                , c.bzname yldylb 
                , count(a.jzlsh) rc
                , isnull(sum(a.tcjjzf), 0) tcjjzf
                , isnull(sum(a.zhzf), 0) zhzf
                , isnull(sum(a.xjzf), 0) xjzf
                , isnull(sum(a.dejjzf), 0) dbzf
                , isnull(sum(a.yyfdfy), 0) yyfd
                , isnull(sum(a.mzjzfy), 0) mzjz
                , isnull(sum(a.ecbcje), 0) ecbc
                , isnull(sum(a.gwybzjjzf), 0) gwybz
                , isnull(sum(a.qybcylbxjjzf), 0) qybc
                , isnull(sum(a.zfddjjfy), 0) zfdd
                , isnull(sum(a.tcjjzf), 0) 
                + isnull(sum(a.zhzf), 0) 
                + isnull(sum(a.xjzf), 0) 
                + isnull(sum(a.dejjzf), 0) 
                + isnull(sum(a.yyfdfy), 0) 
                + isnull(sum(a.mzjzfy), 0) 
                + isnull(sum(a.ecbcje), 0) 
                + isnull(sum(a.qybcylbxjjzf), 0) 
                + isnull(sum(a.zfddjjfy), 0) 
                + isnull(sum(a.gwybzjjzf), 0) hj
                from ybfyjsdr a
                join bztbd b on a.yllb = b.bzmem1 and b.bzcodn = 'YL'
                join bztbd c on a.yldylb = c.bzmem1 and c.bzcodn = 'DL'
                join ybmzzydjdr d on a.jzlsh = d.jzlsh and a.cxbz = d.cxbz
                where a.sysdate >= '{0}' and a.sysdate < '{1}' 
                {2} and a.cxbz = 1 {3} {4}
                group by
                b.bzname, c.bzname
                union all 
                select '合计'
                , null       
                , count(a.jzlsh) as rc
                , isnull(sum(a.tcjjzf), 0) tcjjzf
                , isnull(sum(a.zhzf), 0) zhzf
                , isnull(sum(a.xjzf), 0) xjzf
                , isnull(sum(a.dejjzf), 0) dbzf
                , isnull(sum(a.yyfdfy), 0) yyfd
                , isnull(sum(a.mzjzfy), 0) mzjz
                , isnull(sum(a.ecbcje), 0) ecbc
                , isnull(sum(a.gwybzjjzf), 0) gwybz
                , isnull(sum(a.qybcylbxjjzf), 0) qybc
                , isnull(sum(a.zfddjjfy), 0) zfdd
                , isnull(sum(a.tcjjzf), 0) 
                + isnull(sum(a.zhzf), 0) 
                + isnull(sum(a.xjzf), 0) 
                + isnull(sum(a.dejjzf), 0) 
                + isnull(sum(a.yyfdfy), 0) 
                + isnull(sum(a.mzjzfy), 0) 
                + isnull(sum(a.ecbcje), 0) 
                + isnull(sum(a.qybcylbxjjzf), 0) 
                + isnull(sum(a.zfddjjfy), 0) 
                + isnull(sum(a.gwybzjjzf), 0) hj
                from ybfyjsdr a
                join bztbd b on a.yllb = b.bzmem1 and b.bzcodn = 'YL'
                join bztbd c on a.yldylb = c.bzmem1 and c.bzcodn = 'DL'
                join ybmzzydjdr d on a.jzlsh = d.jzlsh and a.cxbz = d.cxbz
                where a.sysdate >= '{0}' and a.sysdate < '{1}'
                {2} and a.cxbz = 1 {3} {4}"
                , dateTimeStar.Value.ToString("yyyy-MM-dd"), dateTimeEnd.Value.AddDays(1).ToString("yyyy-MM-dd"), yldylbs,location,where);
            ds = CliUtils.ExecuteSql("sybdj", "cmd", sql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);


            DataTable dt = ds.Tables[0];
            dgvbf.DataSource = dt;

            sql = string.Format(@"select a.jzlsh,a.xm
                , count(a.jzlsh) hc
                , d.bzmc bzmc
                , d.ksmc ksmc
                , isnull(sum(a.tcjjzf), 0) tcjjzf
                , isnull(sum(a.zhzf), 0) zhzf
                , isnull(sum(a.xjzf), 0) xjzf
                , isnull(sum(a.dejjzf), 0) dbzf
                , isnull(sum(a.yyfdfy), 0) yyfd
                , isnull(sum(a.mzjzfy), 0) mzjz
                , isnull(sum(a.ecbcje), 0) ecbc
                , isnull(sum(a.gwybzjjzf), 0) gwybz
                , isnull(sum(a.tcjjzf), 0) 
                + isnull(sum(a.zhzf), 0) 
                + isnull(sum(a.xjzf), 0) 
                + isnull(sum(a.dejjzf), 0) 
                + isnull(sum(a.yyfdfy), 0) 
                + isnull(sum(a.mzjzfy), 0) 
                + isnull(sum(a.ecbcje), 0) 
                + isnull(sum(a.gwybzjjzf), 0) hj
                 from ybfyjsdr a
                 join ybmzzydjdr d on d.jzlsh = a.jzlsh  and a.cxbz = d.cxbz
                 where  a.sysdate >= '{0}' and a.sysdate < '{1}'
                {2} and a.cxbz = 1 {3} {4}
                 group by a.jzlsh,a.xm,d.bzmc,d.ksmc
                "
               , dateTimeStar.Value.ToString("yyyy-MM-dd"), dateTimeEnd.Value.AddDays(1).ToString("yyyy-MM-dd"), yldylbs, location,where);
            dsmx = CliUtils.ExecuteSql("sybdj", "cmd", sql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void btn_PrintMX_Click(object sender, EventArgs e)
        {
            string smxpath = Para.sys_parameter("BZ", "BZ0007");
            smxpath = string.IsNullOrEmpty(smxpath) ? @"C:\Program Files (x86)\Infolight\EEP2012\EEPNetClient\FastReport" : smxpath;//空值取默认
            string cmxfile = Application.StartupPath + @"\FastReport\ZY\医疗社会保险结算拨付汇总人员明细表.frx"; //client
            string smxfile = smxpath + @"\ZY\医疗社会保险结算拨付汇总人员明细表.frx";    //server             
            CliUtils.DownLoad(smxfile, cmxfile);   //下载远端AP SERVER报表文件

            //检查报表文件是否存
            if (!File.Exists(cmxfile))
            {
                MessageBox.Show(cmxfile + "不存在!");
                return;
            }


            Report reportmx = new Report();
            reportmx.Load(cmxfile);
            reportmx.RegisterData(dsmx);
            reportmx.SetParameterValue("start", dateTimeStar.Value.ToString("yyyy-MM-dd"));
            reportmx.SetParameterValue("end", dateTimeEnd.Value.ToString("yyyy-MM-dd"));
            reportmx.SetParameterValue("zbsj", DateTime.Now.ToString("yyyy-MM-dd"));
            reportmx.SetParameterValue("yldylb", yldylb);
            reportmx.Show();
            reportmx.Dispose();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string filename = "彭泽县中医院人员明细.xls";
            ColumnMap[] maps = { new ColumnMap("jzlsh", "就诊流水号")
                               , new ColumnMap("xm", "姓名")
                               , new ColumnMap("hc", "号次")
                               , new ColumnMap("bzmc", "病种")
                               , new ColumnMap("ksmc", "科室")
                               , new ColumnMap("hj", "合计")
                               , new ColumnMap("tcjjzf", "统筹基金支付")
                               , new ColumnMap("zhzf", "账户支付")
                               , new ColumnMap("xjzf", "现金支付")
                               , new ColumnMap("dbzf", "大病支付")
                               , new ColumnMap("yyfd", "医院负担")
                               , new ColumnMap("mzjz", "民政救助")
                               , new ColumnMap("ecbc", "二次补偿")
                               , new ColumnMap("gwybz", "公务员补助")
                               };

            try
            {
                DataTable dt = dsmx.Tables[0];
                DataTableToExcel.Dt2File(dt, maps, filename, true);
                Process.Start(filename, Environment.CurrentDirectory + "\\" + filename);
            }
            catch (Exception error)
            {
                MessageBox.Show(error.ToString());
            }
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            string filename = "彭泽县中医院汇总明细.xls";
            ColumnMap[] maps = { new ColumnMap("yllb", "医疗类别")
                               , new ColumnMap("yldylb", "医疗待遇类别")
                               , new ColumnMap("rc", "人次")
                               , new ColumnMap("hj", "合计")
                               , new ColumnMap("tcjjzf", "统筹基金支付")
                               , new ColumnMap("zhzf", "账户支付")
                               , new ColumnMap("xjzf", "现金支付")
                               , new ColumnMap("dbzf", "大病支付")
                               , new ColumnMap("yyfd", "医院负担")
                               , new ColumnMap("mzjz", "民政救助")
                               , new ColumnMap("ecbc", "二次补偿")
                               , new ColumnMap("gwybz", "公务员补助")
                               };

            try
            {
                DataTable dt = ds.Tables[0];
                DataTableToExcel.Dt2File(dt, maps, filename, true);
                Process.Start(filename, Environment.CurrentDirectory + "\\" + filename);
            }
            catch (Exception error)
            {
                MessageBox.Show(error.ToString());
            }
        }

        private void dgvbf_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}

