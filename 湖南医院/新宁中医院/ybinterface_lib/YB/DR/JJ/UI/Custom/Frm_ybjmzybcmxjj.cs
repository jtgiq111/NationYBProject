using FastReport;
using Srvtools;
using System;
using System.Data;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;

namespace ybinterface_lib
{
    public partial class Frm_ybjmzybcmxjj : InfoForm
    {
        GocentPara Para = new GocentPara();
        string yldylb = "";
        DataSet ds;
        DataSet dsmx;

        public Frm_ybjmzybcmxjj()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void btncx_Click(object sender, EventArgs e)
        {
            string yldylbs = "";
            string location = "";
            string where = "";

            yldylbs = " and left(a.yldylb, 1) in ('4', '5', '6', '7', '8', '9') and a.yldylb != '99' and a.yldylb not in ('57','58','59')";
            yldylb = "居民";
            

            location = "and d.tcqh = 360430";
            
            where = "and d.jzbz = 'z'";

            string sql = string.Format(@"	with tmp as	 
			(
				select sum(je) je,jzlsh from ybcfmxscfhdr where left(yyxmdm, 7) in ('1000000','2000000','3000000') and cxbz = 1 group by jzlsh
			)  

select a.jzlsh,d.kh,a.xm
                , d.bzmc bzmc
				, b.dwmc dwmc
				, c.z1date ryrq
				, c.z1ldat cyrq
				, datediff(DD,z1date,z1ldat) ts
				, isnull(sum(a.tcjjzf), 0) 
                + isnull(sum(a.zhzf), 0) 
                + isnull(sum(a.xjzf), 0) 
                + isnull(sum(a.dejjzf), 0) 
                + isnull(sum(a.yyfdfy), 0) 
                + isnull(sum(a.mzjzfy), 0) 
                + isnull(sum(a.ecbcje), 0) 
                + isnull(sum(a.qybcylbxjjzf), 0) 
                + isnull(sum(a.zfddjjfy), 0) 
                + isnull(sum(a.gwybzjjzf), 0) fpje
                , isnull(sum(a.jrtcfy), 0) kbje
				, isnull(sum(e.je), 0) zyf
				 , isnull(sum(a.tcjjzf), 0) sbje
				 , a.qfbzfy qfx
                 from ybfyjsdr a
                 join ybmzzydjdr d on d.jzlsh = a.jzlsh  and a.cxbz = d.cxbz
				  left join YBICKXX b on a.xm = b.xm and a.grbh = b.grbh
				  left join zy01h c on a.jzlsh = c.z1zyno
				  left join tmp e on a.jzlsh = e.jzlsh 
                 where  a.sysdate >= '{0}' and a.sysdate < '{1}' and a.cxbz = 1
                {2} {3} {4}
                 group by a.jzlsh,a.xm,d.bzmc,d.ksmc,d.kh,b.dwmc,a.qfbzfy,c.z1date, c.z1ldat"
                , dateTimeStar.Value.ToString("yyyy-MM-dd"), dateTimeEnd.Value.AddDays(1).ToString("yyyy-MM-dd"), yldylbs,location,where);
            ds = CliUtils.ExecuteSql("sybdj", "cmd", sql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);


            DataTable dt = ds.Tables[0];
            dgvbf.DataSource = dt;
           
        }


        private void btn_PrintMX_Click(object sender, EventArgs e)
        {
            string smxpath = Para.sys_parameter("BZ", "BZ0007");
            smxpath = string.IsNullOrEmpty(smxpath) ? @"C:\Program Files (x86)\Infolight\EEP2012\EEPNetClient\FastReport" : smxpath;//空值取默认
            string cmxfile = Application.StartupPath + @"\FastReport\ZY\参合居民住院补偿明细表.frx"; //client
            string smxfile = smxpath + @"\ZY\参合居民住院补偿明细表.frx";    //server             
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
            reportmx.SetParameterValue("date", DateTime.Now.ToString("yyyy-MM-dd"));
            reportmx.Show();
            reportmx.Dispose();
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            string filename = "彭泽县中医院居民住院补偿明细.xls";
            ColumnMap[] maps = { new ColumnMap("jzlsh", "就诊流水号")
                               , new ColumnMap("kh", "社会保障卡号")
                               , new ColumnMap("xm", "姓名")
                               , new ColumnMap("bzmc", "诊断")
                               , new ColumnMap("dwmc", "地址")
                               , new ColumnMap("ryrq", "入院日期")
                               , new ColumnMap("cyrq", "出院日期")
                               , new ColumnMap("ts", "天数")
                               , new ColumnMap("fpje", "发票金额")
                               , new ColumnMap("zyf", "总药费")
                               , new ColumnMap("kbje", "可报金额")
                               , new ColumnMap("sbje", "实补金额")
                               , new ColumnMap("qfx", "起付线")
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

