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
using FastReport;
using System.Diagnostics;

namespace ybinterface_lib
{
    public partial class Frm_ybmzjzmxcxjj : InfoForm
    {

        GocentPara Para = new GocentPara();
        DataSet ds;

        public Frm_ybmzjzmxcxjj()
        {
            InitializeComponent();
            Frm_Load();
        }


        private void Frm_Load()
        {
            string sql = "select a.USERID as USERID,a.USERNAME as USERNAME from USERS a join USERGROUPS b on a.USERID = b.USERID where b.GROUPID = 18";

            DataSet ds;
            ds = CliUtils.ExecuteSql("sybdj", "cmd", sql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            DataTable dt = ds.Tables[0];
            dt.Rows.Add("-1", "全部用户");

            cmb_jbr.DataSource = dt;
            cmb_jbr.DisplayMember = "USERNAME";
            cmb_jbr.ValueMember = "USERID";
            cmb_jbr.SelectedIndex = 0;

        }

        private void btn_Search_Click(object sender, EventArgs e)
        {
            string where = "";
            if (cmb_jbr.Text != "全部用户")
                where = string.Format("and a.jbr = '{0}'", cmb_jbr.Text);

            where += string.Format("and a.sysdate >= '{0}' and a.sysdate < '{1}'", dt_stime.Value.ToString("yyyy-MM-dd"), dt_end.Value.AddDays(1).ToString("yyyy-MM-dd"));

            string sql = string.Format(@"select row_number() over (order by a.sysdate) as rowNum,
            a.jzlsh as jzlsh,
            a.xm as xm,           
            case when c.xb = 1 then '男' else '女'  end as xb,
            d.bzname as dxlb,
            c.sfzh as sfzh,
            b.dwmc as dwmc,
            c.ybkh as ybkh,
            convert(varchar(20),a.sysdate,111) as sysdate,
            b.bzmc as bzmc,
            a.ylfze as ylfze,
            a.zbxje as zbxje,
            a.tcjjzf as tcjjzf,
            a.dejjzf as dejjzf,
            a.qybcylbxjjzf as qybcylbxjjzf,
            a.mzjzfy as mzjzfy
                from ybfyjsdr a join ybmzzydjdr b on a.jzlsh = b.jzlsh and a.cxbz = b.cxbz
                left join YBICKXX c on a.grbh = c.grbh 
                left join bztbd d on a.yldylb = d.bzmem1 and d.bzcodn = 'DL'
                where a.cxbz = 1  and b.jzbz = 'z' and a.yldylb in ('50','51','52','53','61','62','63','64','65','66','72','73','74','75','76','77','78','80','83','84','85','86','87') {0}
                group by    a.sysdate,
                            a.jzlsh,
                            a.xm,
                            c.xb, 
                            d.bzname,
                            c.sfzh,
                            b.dwmc,
                            c.ybkh,
                            a.sysdate, 
                            b.bzmc,
                            a.ylfze,
                            a.zbxje ,
                            a.tcjjzf ,
                            a.dejjzf ,
                            a.qybcylbxjjzf ,
                            a.mzjzfy
 ", where);

            ds = CliUtils.ExecuteSql("sybdj", "cmd", sql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            DataTable dt = ds.Tables[0];
            if (dt.Rows.Count > 0)
            {
                dgvbf.DataSource = dt;
            }

        }

        private void btn_Print_Click(object sender, EventArgs e)
        {
            string s_path = Para.sys_parameter("BZ", "BZ0007");
            s_path = string.IsNullOrEmpty(s_path) ? @"C:\Program Files (x86)\Infolight\EEP2012\EEPNetClient\FastReport" : s_path;//空值取默认
            string c_file = Application.StartupPath + @"\FastReport\YB\彭泽县民政救助对象医疗救助同步结算单.frx"; //client
            string s_file = s_path + @"\YB\彭泽县民政救助对象医疗救助同步结算单.frx";    //server             
            CliUtils.DownLoad(s_file, c_file);   //下载远端AP SERVER报表文件

            //检查报表文件是否存
            if (!File.Exists(c_file))
            {
                MessageBox.Show(c_file + "不存在!");
                return;
            }

            Report report = new Report();
            report.Load(c_file);
            report.RegisterData(ds);
            report.SetParameterValue("user", cmb_jbr.Text);
            report.SetParameterValue("time", Convert.ToDateTime((Common.GetServerTime())).ToString("yyyy-MM-dd"));
            report.Show();
            report.Dispose();
        }

        private void btn_Excel_Click(object sender, EventArgs e)
        {
            string filename = "彭泽县中医院民政救助人员明细.xls";
            ColumnMap[] maps = { new ColumnMap("rowNum", "序号")
                               , new ColumnMap("jzlsh", "就诊流水号")
                               , new ColumnMap("xm", "姓名")
                               , new ColumnMap("xb", "性别")
                               , new ColumnMap("sfzh", "身份证号")
                               , new ColumnMap("dwmc", "单位地址")
                               , new ColumnMap("dxlb", "对象类别")
                               , new ColumnMap("ybkh", "医保卡号")
                               , new ColumnMap("sysdate", "出入院日期")
                               , new ColumnMap("bzmc", "病种")
                               , new ColumnMap("ylfze", "医疗总费用")
                               , new ColumnMap("zbxje", "可报费用")
                               , new ColumnMap("tcjjzf", "基本医疗费用")
                               , new ColumnMap("dejjzf", "大病补偿保险")
                               , new ColumnMap("qybcylbxjjzf", "商业补充保险补偿")
                               , new ColumnMap("mzjzfy", "民政医疗实际救助")
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
