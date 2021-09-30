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

namespace ybinterface_lib
{
    public partial class Frm_JZFP : InfoForm
    {
        DataSet ds;

        public Frm_JZFP()
        {
            InitializeComponent();
        }

        private void btncx_Click(object sender, EventArgs e)
        {
            string sql = string.Format(@"select b.xm,b.sfzh,d.bzname,b.dwmc,a.bzmc,
                SUBSTRING(ghdjsj,1,4)+'-'+SUBSTRING(ghdjsj,5,2)+'-'+SUBSTRING(ghdjsj,7,2)+' '+SUBSTRING(ghdjsj,9,2)+':'+SUBSTRING(ghdjsj,11,2)+':'+SUBSTRING(ghdjsj,13,2) rysj,
                SUBSTRING(cyrq,1,4)+'-'+SUBSTRING(cyrq,5,2)+'-'+SUBSTRING(cyrq,7,2)+' '+SUBSTRING(cyrq,9,2)+':'+SUBSTRING(cyrq,11,2)+':'+SUBSTRING(cyrq,13,2) cysj,
                a.ylfze,a.jrtcfy,a.qfbzfy,a.cxjfy,a.zffy,a.tcjjzf,a.dejjzf,a.qybcylbxjjzf,a.mzjzfy,a.zfddjjfy,null as zfufy,a.yyfdfy from ybfyjsdr a 
                left join ybmzzydjdr b on a.jzlsh = b.jzlsh and a.cxbz = b.cxbz 
                left join bztbd d on b.yldylb = d.bzkeyx and bzcodn = 'DL'
                 where a.sysdate between '{0}' and '{1}' and a.cxbz = 1 and b.yldylb >= 50 and jzbz = 'z'"
               , dateTimeStar.Value.ToString("yyyy-MM-dd"), dateTimeEnd.Value.AddDays(1).ToString("yyyy-MM-dd"));
            ds = CliUtils.ExecuteSql("sybdj", "cmd", sql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);


            DataTable dt = ds.Tables[0];
            dgvbf.DataSource = dt;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string filename = "精准扶贫一站式补偿明细.xls";
            ColumnMap[] maps = { new ColumnMap("xm", "姓名")
                               , new ColumnMap("sfzh", "身份证号")
                               , new ColumnMap("bzname", "身份类别")
                               , new ColumnMap("dwmc", "单位名称")
                               , new ColumnMap("bzmc", "疾病诊断")
                               , new ColumnMap("rysj", "入院日期")
                               , new ColumnMap("cysj", "出院日期")
                               , new ColumnMap("ylfze", "总金额")
                               , new ColumnMap("jrtcfy", "进入统筹费用")
                               , new ColumnMap("qfbzfy", "起付线")
                               , new ColumnMap("cxjfy", "超限价自费")
                               , new ColumnMap("zffy", "政策范围外自费(自费费用)")
                               , new ColumnMap("tcjjzf", "统筹支付")
                               , new ColumnMap("dejjzf", "大病支付")
                               , new ColumnMap("qybcylbxjjzf", "补充保险")
                               , new ColumnMap("mzjzfy", "民政救助")
                               , new ColumnMap("zfddjjfy", "政府兜底")
                               , new ColumnMap("zfufy", "自付费用")
                               , new ColumnMap("yyfdfy", "医院垫付(目录外超10%费用)")
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
