using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Srvtools;
using FastReport;
using System.IO;

namespace ybinterface_lib
{
    public partial class Frm_wjjsdjj : InfoForm
    {
        static GocentPara Para = new GocentPara();
        DataSet ds;
        public Frm_wjjsdjj()
        {
            InitializeComponent();
            Load();
        }

        private void Load()
        {
            Getwjjsd();

            string str = string.Format(@"select * from bztbd where bzcodn = 'DL'");
            ds = CliUtils.ExecuteSql("sybdj", "cmd", str, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            
            txt_dylb.DataSource = ds.Tables[0];
            txt_dylb.DisplayMember = "bzname";
            txt_dylb.ValueMember = "bzmem1";
        }

        private void Getwjjsd()
        {

            string str = string.Format(@"select * from ybwjjs where scbz = 1");
            ds = CliUtils.ExecuteSql("sybdj", "cmd", str, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);

            dgvWJ.DataSource = ds;
            dgvWJ.AutoGenerateColumns = false;
            dgvWJ.DataSource = ds.Tables[0].DefaultView;
        }


        private void txt_jzlsh_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)//如果输入的是回车键  
            {
                txt_jzlsh.Text = txt_jzlsh.Text.PadLeft(8, '0');
                if (string.IsNullOrEmpty(txt_jzlsh.Text))
                {
                    MessageBox.Show( "流水号不能为空","提示");
                    return;
                }
                string str = string.Format(@"select distinct a.z1hznm xm,a.z1ybno grbh,a.z1bird csrq,a.z1rylb dylb,a.z1dwmc dwmc,
                 a.z1mobi lxdh,b.z1date ryrq,b.z1outd cyrq,a.z1bznm cyzd,c.z3jshx djh
                  from zy01h a 
                  join zy01d b on a.z1zyno = b.z1zyno
                  join zy03d c on a.z1zyno = c.z3zyno
					 where a.z1zyno = '{0}' and SUBSTRING(b.z1endv,1,1) = '8'", txt_jzlsh.Text);
                ds = CliUtils.ExecuteSql("sybdj", "cmd", str, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                DataTable dt = ds.Tables[0];
                if (dt.Rows.Count > 0)
                {
                    DataRow dr = dt.Rows[0];
                    string dylb = dr["dylb"].ToString();
                    txt_dylb.SelectedValue = dylb;
                    txt_xm.Text = dr["xm"].ToString();
                    txt_grbh.Text = dr["grbh"].ToString();
                    txt_csrq.Text = dr["csrq"].ToString();
                    txt_dylb.Text = dr["dylb"].ToString();
                    txt_dwmc.Text = dr["dwmc"].ToString();
                    txt_lxdh.Text = dr["lxdh"].ToString();
                    txt_ryrq.Text = dr["ryrq"].ToString();
                    txt_cyrq.Text = dr["cyrq"].ToString();
                    lab_djh.Text = dr["djh"].ToString();
                   
                }
                else
                {
                    MessageBox.Show("该病人无出院记录");
                }
            }
        }

        private void txt_bcsbje_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)//如果输入的是回车键  
            {
                if (string.IsNullOrEmpty(txt_bcsbje.Text))
                {
                    MessageBox.Show("请输入金额");
                    return;
                }
                string dxje = Common.MoneyToUpper(txt_bcsbje.Text);
                txt_dx.Text = dxje;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {

            string spath = Para.sys_parameter("BZ", "BZ0007");
            spath = string.IsNullOrEmpty(spath) ? @"C:\Program Files (x86)\Infolight\EEP2012\EEPNetClient\FastReport" : spath;
            string cfile = Application.StartupPath + @"\FastReport\YB\九江市城乡居民医保门诊(外检)结算单.frx";
            string sfile = spath + @"\YB\九江市城乡居民医保门诊(外检)结算单.frx";
            CliUtils.DownLoad(sfile, cfile);

            if (!File.Exists(cfile))
            {
                MessageBox.Show( cfile + "不存在!", "提示");
            }

            Report report = new Report();
            report.Load(cfile);
            report.PrintSettings.ShowDialog = false;
            report.RegisterData(ds);
            

            #region 报表赋值
            report.SetParameterValue("djh", lab_djh.Text);
            report.SetParameterValue("jzlsh", txt_jzlsh.Text);
            report.SetParameterValue("grbh", txt_grbh.Text);
            report.SetParameterValue("jsrq", Common.GetServerTime());
            report.SetParameterValue("xm", txt_xm.Text);
            report.SetParameterValue("csrq", txt_csrq.Text);
            report.SetParameterValue("dylb", txt_dylb.Text);
            report.SetParameterValue("dwmc", txt_dwmc.Text);
            report.SetParameterValue("dwbh", txt_dwbh.Text);
            report.SetParameterValue("lxdh", txt_lxdh.Text);
            report.SetParameterValue("jcyy", txt_jcyy.Text);
            report.SetParameterValue("yydj", txt_yydj.Text);
            report.SetParameterValue("ryrq", txt_ryrq.Text);
            report.SetParameterValue("cyrq", txt_cyrq.Text);
            report.SetParameterValue("cyzd", txt_cyzd.Text);
            report.SetParameterValue("ylzf", txt_ylzf.Text);
            report.SetParameterValue("blzf", txt_blzf.Text);
            report.SetParameterValue("cxjfy", txt_cxj.Text);
            report.SetParameterValue("zffy", txt_zffy.Text);
            report.SetParameterValue("bcfy", txt_bcfy.Text);
            report.SetParameterValue("jrtcfy", txt_jrtcfy.Text);
            report.SetParameterValue("tczf", txt_tczf.Text);
            report.SetParameterValue("grzf", txt_grzf.Text);
            report.SetParameterValue("bcsbje", txt_bcsbje.Text);
            report.SetParameterValue("dx", txt_dx.Text);
            #endregion

            if (CliUtils.fPrintPreview == "True")
            {
                report.Show();
            }
            else
            {
                report.Print();
            }

        }

        private void btn_add_Click(object sender, EventArgs e)
        {
            DialogResult dr = MessageBox.Show("是否保存该病人外检信息？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (dr == DialogResult.No)
            {
                return;
            }

            #region 外检数据
            string jzlsh = txt_jzlsh.Text;
            string djh = lab_djh.Text;
            string xm = txt_xm.Text;
            string grbh = txt_grbh.Text;
            string csrq =  txt_csrq.Text;
            string dylb = txt_dylb.Text;
            string dwmc = txt_dwmc.Text;
            string dwbh = txt_dwbh.Text;
            string lxdh = txt_lxdh.Text;
            string jcyy = txt_jcyy.Text;
            string yydj = txt_yydj.Text;
            string ryrq = txt_ryrq.Text;
            string cyrq = txt_cyrq.Text;
            string cyzd = txt_cyzd.Text;
            string ylzf = txt_ylzf.Text;
            string blzf = txt_blzf.Text;
            string cxj = txt_cxj.Text;
            string zffy = txt_zffy.Text;
            string bcfy = txt_bcfy.Text;
            string jrtcfy = txt_jrtcfy.Text;
            string tczf = txt_tczf.Text;
            string grzf = txt_grzf.Text;
            string bcsbje = txt_bcsbje.Text;
            #endregion

            try
            {
                string str = string.Format(@"insert into ybwjjs (jzlsh, djh, xm, grbh, csrq, dylb, dwmc, dwbh, lxdh, jcyy, yydj, ryrq, cyrq, cyzd, ylzf, blzf, cxj, zffy, bcfy, jrtcfy, tczf, grzf, bcsbje,jbr,sysdate,scbz)
                values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}','{20}','{21}','{22}','{23}','{24}',1)",
                           jzlsh, djh, xm, grbh, csrq, dylb, dwmc, dwbh, lxdh, jcyy, yydj, ryrq, cyrq, cyzd, ylzf, blzf, cxj, zffy, bcfy, jrtcfy, tczf, grzf, bcsbje, CliUtils.fLoginUser, Common.GetServerTime());
                object[] obj = new object[] { str };
                obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);
                if (obj[1].ToString() == "1")
                {
                    MessageBox.Show("插入数据库成功");
                    Getwjjsd();
                }
                else
                {
                    MessageBox.Show("插入数据库失败");
                }

            }
            catch (SyntaxErrorException err)
            {
                MessageBox.Show(err.ToString());
            }
            
        }

        private void dgvWJ_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex != -1)
            {
                txt_jzlsh.Text = dgvWJ.Rows[e.RowIndex].Cells["jzlsh"].Value.ToString();
                lab_djh.Text = dgvWJ.Rows[e.RowIndex].Cells["djh"].Value.ToString();
                txt_xm.Text = dgvWJ.Rows[e.RowIndex].Cells["xm"].Value.ToString();
                txt_grbh.Text = dgvWJ.Rows[e.RowIndex].Cells["grbh"].Value.ToString();
                txt_csrq.Text = dgvWJ.Rows[e.RowIndex].Cells["csrq"].Value.ToString();
                txt_dylb.Text = dgvWJ.Rows[e.RowIndex].Cells["dylb"].Value.ToString();
                txt_dwmc.Text = dgvWJ.Rows[e.RowIndex].Cells["dwmc"].Value.ToString();
                txt_dwbh.Text = dgvWJ.Rows[e.RowIndex].Cells["dwbh"].Value.ToString();
                txt_lxdh.Text = dgvWJ.Rows[e.RowIndex].Cells["lxdh"].Value.ToString();
                txt_jcyy.Text = dgvWJ.Rows[e.RowIndex].Cells["jcyy"].Value.ToString();
                txt_yydj.Text = dgvWJ.Rows[e.RowIndex].Cells["yydj"].Value.ToString();
                txt_ryrq.Text = dgvWJ.Rows[e.RowIndex].Cells["ryrq"].Value.ToString();
                txt_cyrq.Text = dgvWJ.Rows[e.RowIndex].Cells["cyrq"].Value.ToString();
                txt_cyzd.Text = dgvWJ.Rows[e.RowIndex].Cells["cyzd"].Value.ToString();
                txt_ylzf.Text = dgvWJ.Rows[e.RowIndex].Cells["ylzf"].Value.ToString();
                txt_blzf.Text = dgvWJ.Rows[e.RowIndex].Cells["blzf"].Value.ToString();
                txt_cxj.Text = dgvWJ.Rows[e.RowIndex].Cells["cxj"].Value.ToString();
                txt_zffy.Text = dgvWJ.Rows[e.RowIndex].Cells["zffy"].Value.ToString();
                txt_bcfy.Text = dgvWJ.Rows[e.RowIndex].Cells["bcfy"].Value.ToString();
                txt_jrtcfy.Text = dgvWJ.Rows[e.RowIndex].Cells["jrtcfy"].Value.ToString();
                txt_tczf.Text = dgvWJ.Rows[e.RowIndex].Cells["tczf"].Value.ToString();
                txt_grzf.Text = dgvWJ.Rows[e.RowIndex].Cells["grzf"].Value.ToString();
                txt_bcsbje.Text = dgvWJ.Rows[e.RowIndex].Cells["bcsbje"].Value.ToString();
                if (!string.IsNullOrEmpty(txt_bcsbje.Text))
                {
                    txt_dx.Text = Common.MoneyToUpper(txt_bcsbje.Text);
                }

            }
        }

        private void btn_delete_Click(object sender, EventArgs e)
        {

            DialogResult dr = MessageBox.Show("是否删除该病人外检信息？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (dr == DialogResult.No)
            {
                return;
            }
            if (string.IsNullOrEmpty(txt_jzlsh.Text))
            {
                MessageBox.Show("就诊流水号不能为空");
                return;

            }
            if (string.IsNullOrEmpty(lab_djh.Text))
            {
                MessageBox.Show("单据号不能为空");
                return;
            }

            string jzlsh = txt_jzlsh.Text;
            string djh = lab_djh.Text;

            try
            {
                string str = string.Format(@"update ybwjjs set scbz = 0 where jzlsh = '{0}' and djh = '{1}'",jzlsh,djh);
                object[] obj = new object[] { str };
                obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);
                if (obj[1].ToString() == "1")
                {
                    MessageBox.Show("删除信息成功");
                    Getwjjsd();
                }
                else
                {
                    MessageBox.Show("删除信息失败");
                }
            }
            catch (SyntaxErrorException err)
            {
                MessageBox.Show(err.ToString());
            }
        }
    }
}
