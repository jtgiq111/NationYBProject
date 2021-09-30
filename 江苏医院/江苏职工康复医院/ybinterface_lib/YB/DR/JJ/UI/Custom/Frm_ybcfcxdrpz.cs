using FastReport;
using Srvtools;
using System;
using System.Data;
using System.IO;
using System.Windows.Forms;

namespace ybinterface_lib
{
    public partial class Frm_ybcfcxdrpz : InfoForm
    {
        DataSet ds;

        public Frm_ybcfcxdrpz()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void btquty_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtfphx.Text))
            {
                MessageBox.Show("请输入发票号并回车", "提示");
                return;
            }

            if (string.IsNullOrEmpty(txtghno.Text))
            {
                MessageBox.Show("挂号流水号为空，请输入发票号并回车", "提示");
                return;
            }

            string strSql = string.Format(@"select * from mz03d(nolock) m join  
                           (
                           --药品
                           select a.mcypno yyxmbh, a.mcypnm yyxmmc, a.mcpric dj, a.mcquty sl, a.mcamnt je, a.mcksno ksno, a.mcuser ysdm, a.mcsflb sfno, a.mccfno cfh
                            from mzcfd(nolock) a  
                            union all
                            --检查/治疗 
                            select  b.mbitem yyxmbh, b.mbname yyxmmc, b.mbpric dj, b.mbquty sl, b.mbsjam je, b.mbksno ksno, b.mbuser ysdm, b.mbsfno sfno, b.mbzlno cfh          
                            from mzb2d(nolock) b 
                            union all
                            --检验
                            select c.mbitem yyxmbh, c.mbname yyxmmc, c.mbpric dj, c.mbquty sl, c.mbsjam je, c.mbksno ksno, c.mbuser ysdm, c.mbsfno sfno, c.mbzlno cfh
                            from mzb4d(nolock) c 
                            union all
                            --注射
                            select d.mdwayid yyxmbh, d.mdwayx yyxmmc, d.mdamnt dj, 1 sl, d.mdamnt je, d.mdzsks ksno, d.mdempn ysdm, d.mdsflb sfno, d.mdcfno cfh
                            from mzd3d(nolock) d
                            ) t on t.cfh = m.m3cfno 
                            join mz01h(nolock) on m3ghno = m1ghno and m3mzno = m1mzno
                            join ybmzzydjdr(nolock) on jzlsh = m.m3ghno
                            join bz01h(nolock) e on m.m3empn = e.b1empn
                            join bz02d(nolock) f on m.m3cfks = f.b2ejks
                            where m.m3invo = '{0}' and isnull(m.m3invo, '') <> ''", txtfphx.Text);//2秒
            ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            infoDataGridView1.AutoGenerateColumns = false;
            infoDataGridView1.DataSource = ds.Tables[0];
        }

        private void btprint_Click(object sender, EventArgs e)
        {
            if (ds.Tables[0].Rows.Count <= 0)
            {
                MessageBox.Show("没有任何数据", "提示");
                return;
            }

            GocentPara Para = new GocentPara();
            string s_path = Para.sys_parameter("BZ", "BZ0007");
            s_path = string.IsNullOrEmpty(s_path) ? @"C:\Program Files (x86)\Infolight\EEP2012\EEPNetClient\FastReport" : s_path;
            string c_file = Application.StartupPath + @"\FastReport\YB\彭泽县中医院医保处方报表.frx";
            string s_file = s_path + @"\YB\彭泽县中医院医保处方报表.frx";
            CliUtils.DownLoad(s_file, c_file);

            //检查报表文件是否存
            if (!File.Exists(c_file))
            {
                MessageBox.Show(c_file + "不存在!", "提示");
                return;
            }

            Report report = new Report();
            report.Load(c_file);
            report.RegisterData(ds);
            report.SetParameterValue("comp", CliUtils.fLoginComp);
            report.SetParameterValue("kh", ds.Tables[0].Rows[0]["grbh"]);
            report.SetParameterValue("data", ds.Tables[0].Rows[0]["m3sfdt"]);
            report.SetParameterValue("mcdw", ds.Tables[0].Rows[0]["m1dwmc"]);
            report.SetParameterValue("name", ds.Tables[0].Rows[0]["m1name"]);
            report.SetParameterValue("sexx", ds.Tables[0].Rows[0]["m1sexx"]);
            report.SetParameterValue("agex", ds.Tables[0].Rows[0]["m1agex"]);
            report.SetParameterValue("ksmc", ds.Tables[0].Rows[0]["b2ejnm"]);
            report.SetParameterValue("ysxm", ds.Tables[0].Rows[0]["b1name"]);
            string strSQL1 = string.Format(@"select top 1 m1xynm from mza1dd(nolock) where m1ghno = '{0}' order by m1date desc", txtghno.Text.Trim());//2秒
            DataSet ds1 = CliUtils.ExecuteSql("sybdj", "cmd", strSQL1, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            report.SetParameterValue("mzzd", ds1.Tables[0].Rows.Count == 0 ? "" : ds1.Tables[0].Rows[0]["m1xynm"]);

            if (CliUtils.fPrintPreview == "True")
            {
                report.Show();
            }
            else
            {
                report.Print();
            }

            report.Dispose();
        }

        private void txtfphx_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                string strSQL = string.Format(@"select m3mzno, m3ghno, m1name, m1sexx, m1agex from mz01h(nolock) join mz03d(nolock) on m3ghno = m1ghno and m3mzno = m1mzno where m3invo = '{0}'", txtfphx.Text);//0秒
                DataSet ds0 = CliUtils.ExecuteSql("sybdj", "cmd", strSQL, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);

                if (ds0.Tables[0].Rows.Count > 0)
                {
                    txtmzno.Text = ds0.Tables[0].Rows[0]["m3mzno"].ToString();
                    txtghno.Text = ds0.Tables[0].Rows[0]["m3ghno"].ToString();
                    txtname.Text = ds0.Tables[0].Rows[0]["m1name"].ToString();
                    txtsex.Text = ds0.Tables[0].Rows[0]["m1sexx"].ToString();
                    txtagex.Text = ds0.Tables[0].Rows[0]["m1agex"].ToString();
                }
                else
                {
                    return;
                }
            }
        }

        private void btn_qk_Click(object sender, EventArgs e)
        {
            txtmzno.Text = "";
            txtghno.Text = "";
            txtname.Text = "";
            txtsex.Text = "";
            txtagex.Text = "";
        }

        private void btn_Close_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void txtghno_KeyDown(object sender, KeyEventArgs e)
        {
            txtghno.Text = txtghno.Text.PadLeft(9, '0');
        }
    }
}

