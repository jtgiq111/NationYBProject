using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Srvtools;
using System.Diagnostics;
using FastReport;
using System.IO;

namespace ybinterface_lib
{
    public partial class Frm_wjcx : InfoForm
    {
        GocentPara Para = new GocentPara();
        DataSet ds;
        string fp = "";
        string jrtc = "";
        string bc = "";

        public Frm_wjcx()
        {
            InitializeComponent();
        }

        private void btncx_Click(object sender, EventArgs e)
        {
            string sql = string.Format(@"select * from ybwjjs where sysdate between '{0}' and '{1}' "
               , dateTimeStar.Value.ToString("yyyy-MM-dd"), dateTimeEnd.Value.AddDays(1).ToString("yyyy-MM-dd"));
            ds = CliUtils.ExecuteSql("sybdj", "cmd", sql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);


            DataTable dt = ds.Tables[0];
            dgvbf.DataSource = dt;

            sql = string.Format(@"select sum(cast(isnull(bcfy,'0') as decimal(18,2))) fp,sum(cast(isnull(jrtcfy,'0') as decimal(18,2))) jrtc,
                sum(cast(isnull(bcsbje,'0') as decimal(18,2))) bc from ybwjjs
                 where sysdate between '{0}' and '{1}' "
               , dateTimeStar.Value.ToString("yyyy-MM-dd"), dateTimeEnd.Value.AddDays(1).ToString("yyyy-MM-dd"));
            DataSet ds1 = CliUtils.ExecuteSql("sybdj", "cmd", sql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            dt = ds1.Tables[0];

            DataRow dr = dt.Rows[0];
            fp = dr["fp"].ToString();
            jrtc = dr["jrtc"].ToString();
            bc = dr["bc"].ToString();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string smxpath = Para.sys_parameter("BZ", "BZ0007");
            smxpath = string.IsNullOrEmpty(smxpath) ? @"C:\Program Files (x86)\Infolight\EEP2012\EEPNetClient\FastReport" : smxpath;//空值取默认
            string cmxfile = Application.StartupPath + @"\FastReport\YB\外检费用明细.frx"; //client
            string smxfile = smxpath + @"\YB\外检费用明细.frx";    //server             
            CliUtils.DownLoad(smxfile, cmxfile);   //下载远端AP SERVER报表文件

            //检查报表文件是否存
            if (!File.Exists(cmxfile))
            {
                MessageBox.Show(cmxfile + "不存在!");
                return;
            }


            Report reportmx = new Report();
            reportmx.Load(cmxfile);
            reportmx.RegisterData(ds);
            //reportmx.SetParameterValue("start", dateTimeStar.Value.ToString("yyyy-MM-dd"));
            //reportmx.SetParameterValue("end", dateTimeEnd.Value.ToString("yyyy-MM-dd"));
            //reportmx.SetParameterValue("zbsj", DateTime.Now.ToString("yyyy-MM-dd"));
            //reportmx.SetParameterValue("yldylb", yldylb);
            reportmx.SetParameterValue("fp", fp);
            reportmx.SetParameterValue("jrtc", jrtc);
            reportmx.SetParameterValue("bc", bc);
            reportmx.SetParameterValue("comp", CliUtils.fLoginComp);
            reportmx.SetParameterValue("time", DateTime.Now.ToString("yyyy-MM-dd"));
            reportmx.Show();
            reportmx.Dispose();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string filename = "彭泽县中医院外检结算报表.xls";
            ColumnMap[] maps = { new ColumnMap("grbh", "个人编号")
                               , new ColumnMap("xm", "姓名")
                               , new ColumnMap("cyzd", "出院诊断")
                               , new ColumnMap("ryrq", "入院日期")
                               , new ColumnMap("cyrq", "出院日期")
                               , new ColumnMap("bcfy", "发票金额")
                               , new ColumnMap("jrtcfy", "可报金额")
                               , new ColumnMap("bcsbje", "补偿金额")
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
    }
}
