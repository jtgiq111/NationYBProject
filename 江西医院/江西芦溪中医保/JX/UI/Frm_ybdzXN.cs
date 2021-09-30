using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Srvtools;
using System.Diagnostics;
using yb_interfaces;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;
using static yb_interfaces.FrmPXXZ;
using Cassandra.Mapping;

namespace yb_interfaces
{
    public partial class Frm_ybdzXN : InfoForm
    {
        Frm_dzXN dz;
        public Frm_ybdzXN()
        {
            InitializeComponent();
            Init();
        }

        public void Init()
        {
            this.cmbqslb.SelectedIndex = 0;
            this.cmbxzlb.SelectedIndex = 0;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            string xzlx = string.IsNullOrEmpty(cmbxzlb.SelectedItem.ToString()) ? "ALL" : cmbxzlb.SelectedItem.ToString().Substring(0, 3);
            string qslx = string.IsNullOrEmpty(cmbqslb.SelectedItem.ToString()) ? "ALL" : cmbqslb.SelectedItem.ToString().Substring(0, 2);
            string strSql = string.Empty;
            strSql = string.Format(@"select count(jzlsh) rc,isnull(sum(ylfze),'0.00') ylfze,isnull(sum(zbxje)-sum(zhzf),'0.00') as zbxje,isnull(sum(zhzf),'0.00') zhzf,isnull(sum(xjzf),'0.00') as xjzf  from ybfyjsdr
            where cxbz = 1 and sysdate between '{0}' and '{1}' and (xzlx='{2}' or 'ALL'='{2}') and (jylx='{3}' or 'ALL'='{3}')",
                zzDate.Value.ToString("yyyy-MM-dd 00:00:00"), zzDate.Value.ToString("yyyy-MM-dd 23:59:59"), xzlx, qslx);
            DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            if (ds.Tables[0].Rows.Count == 0)
                MessageBox.Show("无结算信息");
            else
            {
                txt_byrc.Text = ds.Tables[0].Rows[0]["rc"].ToString();
                txt_byylfze.Text = ds.Tables[0].Rows[0]["ylfze"].ToString();
                txt_byzbxje.Text = ds.Tables[0].Rows[0]["zbxje"].ToString();
                txt_byzhzf.Text = ds.Tables[0].Rows[0]["zhzf"].ToString();
                this.txt_byxjzf.Text = ds.Tables[0].Rows[0]["xjzf"].ToString();
            }
        }
        public static string txtfilePath = "";
        private void button4_Click(object sender, EventArgs e)
        {
            string xzlx = string.IsNullOrEmpty(cmbxzlb.SelectedItem.ToString()) ? "ALL" : cmbxzlb.SelectedItem.ToString().Substring(0, 3);
            string qslx = string.IsNullOrEmpty(cmbqslb.SelectedItem.ToString()) ? "ALL" : cmbqslb.SelectedItem.ToString().Substring(0, 2);
            
            string StrSql = string.Format(@"select distinct jbjgbm from ybfyjsdr
where cxbz = 1 and sysdate between '{0}'
and '{1}'
and(xzlx = '{2}' or 'ALL' = '{2}')
and(jylx = '{3}' or 'ALL' = '{3}' and isnull(jbjgbm,'')<>'')", zzDate.Value.ToString("yyyy-MM-dd 00:00:00"), zzDate.Value.ToString("yyyy-MM-dd 23:59:59"), xzlx, qslx);
            DataSet ds1 = CliUtils.ExecuteSql("sybdj", "cmd", StrSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            if (ds1.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds1.Tables[0].Rows.Count; i++)
                {
                    string qsyljg = ds1.Tables[0].Rows[i]["jbjgbm"].ToString();
                    string StrdzSql = string.Format(@"select  jslsh as setl_id,ybjzlsh as mdtrt_id,grbh as psn_no,ylfze as medfee_sumamt,tcjjzf as fund_pay_sumamt,zhzf as acct_pay,(case cxbz when '1' then '0' when '2' then '1' else '2' end) as refd_setl_flag
from ybfyjsdr
where cxbz = 1 and sysdate between '{0}'
and '{1}'
and(xzlx = '{2}' or 'ALL' = '{2}')
and(jylx = '{3}' or 'ALL' = '{3}'
and jbjgbm='{4}')", zzDate.Value.ToString("yyyy-MM-dd 00:00:00"), zzDate.Value.ToString("yyyy-MM-dd 23:59:59"), xzlx, qslx, qsyljg);
                    DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", StrdzSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                    if (ds.Tables[0].Rows.Count == 0)
                        MessageBox.Show("无结算明细");
                    WriteTable(ds.Tables[0], $"对账明细_{DateTime.Now.ToString("yyyyMMddHHmmss")}.txt");
                    MessageBox.Show("导出txt文本成功");
                    string zipFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"YBDZ\\SRZIP\\对账明细_{DateTime.Now.ToString("yyyyMMddHHmmss")}.zip");
                    CreateZipFile(zipFilePath);
                    byte[] wjlbyt = FileToByte(zipFilePath);

                    //MessageBox.Show("压缩成功！");
                    object[] obj = { wjlbyt, zzDate.Value.ToString("yyyy-MM-dd 00:00:00"), zzDate.Value.ToString("yyyy-MM-dd 23:59:59"), txt_byylfze.Text, txt_byzbxje.Text, txt_byzhzf.Text, txt_byxjzf.Text, txt_byrc.Text, qsyljg, qslx,"" };
                    obj = yb_interface_jx.YBDZMX(obj);
                    if (obj[1].ToString() != "1")
                    {
                        MessageBox.Show(obj[2].ToString(), "提示");
                    }
                    else
                    {
                        string path = obj[3].ToString();
                        path = "YBLog\\result.txt";
                        string sPath = Path.Combine(Environment.CurrentDirectory, path);
                        if (!File.Exists(sPath))
                        {
                            MessageBox.Show("文件不存在！");
                            return;
                        }
                        string[] strCloumns = { "人员编号", "就诊ID", "结算ID", "发送方报文ID", "对账结果", "退费结算标志", "对账说明", "基金支付总额", "个人账户支出","备注" };
                        DataTable dt = ExcelFunc.GetDataTableFromFile(path, strCloumns, "制表符", "gb2312");
                        this.dataGridView1.DataSource = dt;
                    }
                }
            }

            //if (obj[1].ToString() == "1")
            //{
            //    MessageBox.Show("所有明细下载成功", "提示");
            //}
            //else
            //{
            //    MessageBox.Show(obj[2].ToString(), "提示");
            //}
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //string sql = string.Format(@"select * from ybfyjsdr");
            //DataSet dsmx = CliUtils.ExecuteSql("sybdj", "cmd", sql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            //string filename = "新宁县中医院医保明细对照.xls";
            //ColumnMap[] maps = { new ColumnMap("ybjzlsh", "医保就诊流水号")
            //                   , new ColumnMap("djh", "单据号")
            //                   , new ColumnMap("grbh", "个人编号")
            //                   , new ColumnMap("kh", "卡号")
            //                   , new ColumnMap("xm", "姓名")
            //                   , new ColumnMap("yllb", "医疗类别")
            //                   , new ColumnMap("ylfze", "医疗总费用")
            //                   , new ColumnMap("zbxje", "总报销金额")
            //                   , new ColumnMap("jsrq", "结算日期")
            //                   , new ColumnMap("jssj", "结算时间")
            //                   , new ColumnMap("jzlsh", "就诊流水号")
            //                   , new ColumnMap("dqbh", "地区编号")
            //                   , new ColumnMap("zhzf", "医院负担")
            //                   , new ColumnMap("xjzf", "现金支付")
            //                   };

            //try
            //{
            //    DataTable dt = dsmx.Tables[0];
            //    DataTableToExcel.Dt2File(dt, maps, filename, true);
            //    Process.Start(filename, Environment.CurrentDirectory + "\\" + filename);
            //}
            //catch (Exception error)
            //{
            //    MessageBox.Show(error.ToString());
            //}
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(cmbqslb.SelectedItem.ToString()))
            {
                MessageBox.Show("清算类别不得为空！");
                return;
            }
            if (string.IsNullOrEmpty(cmbxzlb.SelectedItem.ToString()))
            {
                MessageBox.Show("险种类型不得为空！");
                return;
            }
            string qslb = cmbqslb.SelectedItem.ToString().Substring(0, 2);//清算类别
            string xzlx = cmbxzlb.SelectedItem.ToString().Substring(0, 3);//险种类型
            object[] obj = { xzlx, qslb, zzDate.Value.ToString("yyyy-MM-dd 00:00:00"), zzDate.Value.ToString("yyyy-MM-dd 23:59:59"), txt_byylfze.Text, txt_byzbxje.Text, txt_byzhzf.Text, txt_byrc.Text };
            obj = yb_interface_jx.YBDZZZ(obj);

            if (obj[1].ToString() == "1")
            {
                string result = "对账结果：" + obj[1].ToString() + "\r\n对账结果详情：" + obj[2].ToString();
                this.txtresult.Text = result;
                this.txtresult.ForeColor = Color.Green;

                //if (obj[2].ToString() == "B")
                //    MessageBox.Show("本院数据与医保信息不符");
                //else
                //    MessageBox.Show("本院数据与医保信息完全一致");
                MessageBox.Show("对账成功！结果已显示在下面");

            }
            else
            {
                MessageBox.Show(obj[2].ToString(), "提示");
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            dz = new Frm_dzXN(zzDate);
            dz.ShowDialog();
        }

        public void WriteTable(DataTable dt, string FileName)
        {
            string path = System.AppDomain.CurrentDomain.BaseDirectory + @"YBDZ";
            string FullPath = Path.Combine(path, FileName);
            txtfilePath = FullPath;
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            FileStream stream = new FileStream(FullPath, FileMode.Append, FileAccess.Write);
            StreamWriter writer = new StreamWriter(stream);
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    writer.Write(dt.Rows[i][j].ToString() + "\r\t");
                }
                writer.WriteLine();
            }
            writer.Close();
            stream.Close();
        }


        internal string fileNAME = "";
        #region 压缩方法
        private bool CreateZipFile(string zipFilePath)
        {
            bool result = true;
            ZipOutputStream zipStream = null;
            FileStream fs = null;
            ZipEntry ent = null;

            if (!File.Exists(txtfilePath))
                return false;

            try
            {
                fs = File.OpenRead(txtfilePath);
                byte[] buffer = new byte[fs.Length];
                fs.Read(buffer, 0, buffer.Length);
                fs.Close();

                fs = File.Create(zipFilePath);
                zipStream = new ZipOutputStream(fs);
                ent = new ZipEntry(Path.GetFileName(txtfilePath));
                zipStream.PutNextEntry(ent);
                zipStream.SetLevel(6);

                zipStream.Write(buffer, 0, buffer.Length);

            }
            catch
            {
                result = false;
            }
            finally
            {
                if (zipStream != null)
                {
                    zipStream.Finish();
                    zipStream.Close();
                }
                if (ent != null)
                {
                    ent = null;
                }
                if (fs != null)
                {
                    fs.Close();
                    fs.Dispose();
                }
            }
            GC.Collect();
            GC.Collect(1);

            return result;
        }
        #endregion


        #region 提取byte 数组
        public byte[] FileToByte(string filename)
        {
            if (!File.Exists(filename))
            {
                return new byte[0];
            }
            FileInfo info = new FileInfo(filename);
            byte[] barr = new byte[info.Length];
            FileStream fi = info.OpenRead();
            fi.Read(barr, 0, Convert.ToInt32(fi.Length));
            fi.Close();
            return barr;
        }
        #endregion
    }
}
