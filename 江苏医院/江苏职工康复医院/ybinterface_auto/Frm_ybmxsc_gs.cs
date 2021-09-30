using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ybinterface_auto.Forms;
using ybinterface_lib;

namespace ybinterface_auto
{
    public partial class Frm_ybmxsc_gs : Form
    {
        IWork iWork = new Work();
        SqlHelper helper = new SqlHelper();
        string CZYBH = "";
        string CZYPWD = "";
        string YWZQH = "";
        int i_TIMEPOINT = 0;
        int i_TIMEPOINT1 = 0;
        bool bFalg = false;

        public Frm_ybmxsc_gs()
        {
            InitializeComponent();
            yb_interface_sy_zrNew.isSqlHelper = true;
        }

        private void Frm_ybmxsc_th_Load(object sender, EventArgs e)
        {
            Init();

        }

        internal void Init()
        {
            string xmlPath = AppDomain.CurrentDomain.BaseDirectory;
            string exePath = Application.StartupPath;
            Item it = iWork.getXmlConfig(xmlPath + "ybConfig.xml");
            CZYBH = it.YBUSER;
            CZYPWD = it.YBPWD;
            i_TIMEPOINT = Convert.ToInt32(it.POINT1);
            i_TIMEPOINT1 = Convert.ToInt32(it.POINT2);
            //医保初如化
            object[] objParam = { CZYBH };
            objParam = yb_interface_sy_zrNew.YBINIT(objParam);
            if (objParam[1].ToString().Equals("1"))
            {
                YWZQH = objParam[2].ToString();
                lbMsg.Text = "设定自动上传时间：【" + i_TIMEPOINT + "】时,上传状态：【开启】";
                bFalg = true;
                timer2.Enabled = true;
                getPatientInfo("");
            }
            else
            {
                lbMsg.Text = "设定自动上传时间：【" + i_TIMEPOINT + "】时,上传状态：【未开启】";
                MessageBox.Show("初始化失败|" + objParam[2].ToString());
            }
        }

        internal void getPatientInfo(string param1)
        {

            #region 备份老版本
            //string strSql = string.Format(@"with hisfy as
            //                                (
            //                                select z3zyno,
            //                                sum(case LEFT(a.z3endv,1) when '4' then -1*a.z3jzje else 1*a.z3jzje end) as zje
            //                                from zy03d a 
            //                                where  LEFT(a.z3kind,1) in(2,4) 
            //                                group by z3zyno
            //                                having sum(case LEFT(a.z3endv,1) when '4' then -1*a.z3jzcs else 1*a.z3jzcs end)!=0
            //                                ),
            //                                ybscfy as
            //                                (
            //                                select jzlsh,isnull(SUM(je),0.00) as scje from ybcfmxscfhdr where LEN(jzlsh)=8 and cxbz=1 group by jzlsh
            //                                )
            //                                select  distinct b.z1ghno as bah, b.z1zyno as zyh,b.z1hznm as xm,b.z1ksno as ksno,b.z1ksnm as ksmc,
            //                                a.XB,
            //                                b.z1empn as ysno,b.z1mzys as ysxm,a.grbh,
            //                                '' as zglb,
            //                                a.dwmc, 
            //                                CONVERT(varchar(10),b.z1date,120) as ryrq,d.zje,isnull(e.scje,0.00) as scje
            //                                from ybmzzydjdr  a 
            //                                inner join zy01h b on a.jzlsh=b.z1zyno and left(b.z1endv,1)='0'
            //                                inner join hisfy d on a.jzlsh=d.z3zyno
            //                                left join ybscfy e on a.jzlsh=e.jzlsh
            //                                where LEN(a.jzlsh)=8 and a.cxbz=1 and not exists(select 1 from ybfyjsdr where a.jzlsh=jzlsh and cxbz=1) 
            //                                and (a.jzlsh like '%{0}%' or a.xm like '%{0}%')  and isgs='1'", param1); 
            #endregion
            string strSql = string.Format(@"with hisfy as
                                            (
                                            select z3zyno,
                                            sum(case LEFT(a.z3endv,1) when '4' then -1*a.z3jzje else 1*a.z3jzje end) as zje
                                            from zy03d a 
                                            where  LEFT(a.z3kind,1) in(2,4) 
                                            group by z3zyno
                                            having sum(case LEFT(a.z3endv,1) when '4' then -1*a.z3jzcs else 1*a.z3jzcs end)!=0
                                            ),
                                            ybscfy as
                                            (
                                            select jzlsh,isnull(SUM(je),0.00) as scje from ybcfmxscindr where LEN(jzlsh)=8 and cxbz=1 group by jzlsh
                                            )
                                            select  distinct b.z1ghno as bah, b.z1zyno as zyh,b.z1hznm as xm,b.z1ksno as ksno,b.z1ksnm as ksmc,
                                            a.XB,
                                            b.z1empn as ysno,b.z1mzys as ysxm,a.grbh,
                                            '' as zglb,
                                            a.dwmc, 
                                            CONVERT(varchar(10),b.z1date,120) as ryrq,d.zje,isnull(e.scje,0.00) as scje
                                            from ybmzzydjdr  a 
                                            inner join zy01h b on a.jzlsh=b.z1zyno and left(b.z1endv,1)='0'
                                            inner join hisfy d on a.jzlsh=d.z3zyno
                                            left join ybscfy e on a.jzlsh=e.jzlsh
                                            where LEN(a.jzlsh)=8 and a.cxbz=1 and not exists(select 1 from ybfyjsdr where a.jzlsh=jzlsh and cxbz=1) 
                                            and (a.jzlsh like '%{0}%' or a.xm like '%{0}%')  and isgs='1'", param1);
            DataSet ds = helper.ExecuteDataSet(strSql);
            dgv_PatInfo.AutoGenerateColumns = false;
            dgv_PatInfo.DataSource = ds.Tables[0].DefaultView;
        }

        internal void getOrdersInfo(string Param1)
        {
            string strsql = string.Format(@"select a.z3zyno,a.z3djxx as dj,case LEFT(a.z3endv,1) when '4' then -a.z3jzcs else a.z3jzcs end as sl,
                                        case LEFT(a.z3endv,1) when '4' then -a.z3jzje else a.z3jzje end as je,a.z3mbno as yyxmbh, a.z3item as yyxmmc,
                                        b.ybxmbh,b.ybxmmc,b.sfxmdj, a.z3empn as ysdm, 
                                        a.z3kdys as ysxm,a.z3date as yysj,a.z3ksno as ksno, a.z3zxks as zxks, z3sfno as sfno,
                                        case when a.z3ybup is null then '未上传' else '已上传' end as ybup from zy03d a 
                                        left join ybhisdzgmdrNew b on a.z3mbno=b.hisxmbh
                                        where LEFT(a.z3kind,1)=2 and a.z3zyno='{0}'
                                        order by a.z3ybup", Param1);
            DataSet ds = helper.ExecuteDataSet(strsql);
            dgv_Detail.AutoGenerateColumns = false;
            dgv_Detail.DataSource = ds.Tables[0].DefaultView;

        }

        private int pubMint = 30;
        internal  void autoOrdersSC()
        {
            string sysdate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            int i_nowHour = DateTime.Now.Hour;//i_TIMEPOINT == i_nowHour 
            StringBuilder sb = new StringBuilder(2048);
            sb.Append("费用上传成功的患者：");
            if (dgv_PatInfo.Rows.Count >= 1)
            {
                List<Task> tasklist = new List<Task>();
                foreach (DataGridViewRow row in dgv_PatInfo.Rows)
                {
                    int index = dgv_PatInfo.Rows.IndexOf(row);
                    //Task task =new Task(() =>
                    //{
                    //    lock ("lock")
                    //    {
                            timer1.Enabled = false;
                            //string jzlsh = dgv_PatInfo.Rows[i].Cells["col_zyh"].Value.ToString();

                            string jzlsh = row.Cells["col_zyh"].Value.ToString();
                            string xm = row.Cells["col_xm"].Value.ToString();

                            //object[] objParam = { jzlsh, "", CZYBH, YWZQH, "自动上传" };
                            yb_interface_sy_zrNew.isSqlHelper = true;
                            object[] objParam = { jzlsh, true };
                            objParam = yb_interface_sy_zrNew.YBZYSFDJ(objParam);
                            if (objParam[1].ToString() == "1")
                            {
                                sb.Append(xm + (index == dgv_PatInfo.Rows.Count ? "" : ","));
                                MessageBox.Show($"总共上传{dgv_PatInfo.Rows.Count}个 患者费用\r\n当前第{index + 1}个患者{xm}费用上传成功！(*^▽^*)");
                            }
                            WriteLog(sysdate + "  " + jzlsh + "|" + xm + "    " + objParam[2].ToString()); 
                    //    }
                    //});
                    //tasklist.Add(task);

                }

                //Task.WaitAll(tasklist.ToArray());

                this.lblSCQK.ForeColor = Color.Green;
                this.lblSCQK.Text = sb.ToString();
                getPatientInfo("");
                getOrdersInfo("");
                IsExecute = false;
            }
        }

        internal void WriteLog(string str)
        {
            if (!Directory.Exists("YBLogA"))
            {
                Directory.CreateDirectory("YBLogA");
            }
            FileStream stream = new FileStream("YBLogA\\Log" + DateTime.Now.ToString("yyyyMMdd") + ".txt", FileMode.Append, FileAccess.Write);
            StreamWriter writer = new StreamWriter(stream);
            writer.WriteLine(str);
            writer.Close();
            stream.Close();
        }

        private void tsmi_exit_Click(object sender, EventArgs e)
        {
            //医保退出
            object[] objParam = { CZYBH, YWZQH };
            objParam = yb_interface_sy_zrNew.YBEXIT(objParam);
            Application.Exit();
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.Visible = true;
            this.WindowState = FormWindowState.Normal;
            this.notifyIcon1.Visible = false;
        }

        protected override void WndProc(ref Message m)
        {
            //Console.WriteLine(m.Msg);
            const int WM_SYSCOMMAND = 0x0112;
            const int SC_CLOSE = 0xF060;
            if (m.Msg == WM_SYSCOMMAND && (int)m.WParam == SC_CLOSE)
            {
                //捕捉关闭窗体消息 
                //用户点击关闭窗体控制按钮   注释为最小化窗体   
                //this.WindowState = FormWindowState.Minimized;

                this.WindowState = FormWindowState.Minimized;
                //窗体隐藏
                this.Hide();
                this.notifyIcon1.Visible = true;
                return;
            }
            base.WndProc(ref m);
        }

        private void btn_SELECT_Click(object sender, EventArgs e)
        {
            string strParam = textBox1.Text.Trim();
            getPatientInfo(strParam);
        }

        private void dgv_PatInfo_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            string jzlsh = dgv_PatInfo.CurrentRow.Cells["col_zyh"].Value.ToString();
            getOrdersInfo(jzlsh);
        }

        private void tsmi_start_Click(object sender, EventArgs e)
        {
            if (!bFalg)
            {
                MessageBox.Show("医保初始化失败，不能开启自动上传成能!");
                return;
            }
            //Init();
            string sysdate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            bool IsOk = false;
            if (dgv_PatInfo.SelectedRows.Count != 0)
            {
                string Quaction = "";
                foreach (DataGridViewRow row in dgv_PatInfo.SelectedRows)
                {
                    string zyno1 = row.Cells["col_zyh"].Value.ToString();
                    string xm1 = row.Cells["col_xm"].Value.ToString();
                    Quaction += "确认上传病人[" + zyno1 + "]|[" + xm1 + "]的费用？\r\n";
                }
                //string zyno = dgv_PatInfo.CurrentRow.Cells["col_zyh"].Value.ToString();//col_bah

                DialogResult dr = MessageBox.Show(Quaction, "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                foreach (DataGridViewRow row in dgv_PatInfo.SelectedRows)
                {
                    string zyno = row.Cells["col_zyh"].Value.ToString();
                    string xm = row.Cells["col_xm"].Value.ToString();
                    if (dr == DialogResult.Yes)
                    {
                        //object[] objParam = new object[] { zyno, "", CZYBH, YWZQH, "自动上传" };
                        object[] objParam = new object[] { zyno, true };
                        objParam = yb_interface_sy_zrNew.YBZYSFDJ(objParam);

                        if (Convert.ToInt32(objParam[1].ToString()) == 1)
                        {

                            WriteLog(sysdate + "  " + zyno + "|" + xm + "    " + objParam[2].ToString());
                            MessageBox.Show("[" + zyno + "]|[" + xm + "]自动费用上传完成!请查看日志", "提示");
                            getOrdersInfo(zyno);
                        }
                        else
                        {
                            WriteLog(sysdate + "  " + zyno + "|" + xm + "    " + objParam[2].ToString());
                            MessageBox.Show("[" + zyno + "]|[" + xm + "]自动费用上传失败:" + objParam[2].ToString(), "提示");
                        }
                    }
                }

            }
            else
            {
                MessageBox.Show("请选中一个病人", "提示");
            }
            getPatientInfo("");
            getOrdersInfo("");

        }




        private void tsmi_log_Click(object sender, EventArgs e)
        {
            string logPath = AppDomain.CurrentDomain.BaseDirectory + "YBLogA";
            System.Diagnostics.Process.Start(logPath);
        }

        private void tsmi_ceshi_Click(object sender, EventArgs e)
        {
            string sysdate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            if (dgv_PatInfo.SelectedRows.Count != 0)
            {
                string zyno = dgv_PatInfo.CurrentRow.Cells["col_zyh"].Value.ToString();
                string xm = dgv_PatInfo.CurrentRow.Cells["col_xm"].Value.ToString();
                DialogResult dr = MessageBox.Show("确认上传病人[" + zyno + "]|[" + xm + "]的费用？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (dr == DialogResult.Yes)
                {
                    object[] objParam = new object[] { zyno, "", CZYBH, YWZQH, "自动上传" };
                    objParam = yb_interface_sy_zrNew.YBZYSFDJ(objParam);

                    if (Convert.ToInt32(objParam[1].ToString()) == 1)
                    {

                        WriteLog(sysdate + "  " + zyno + "|" + xm + "    " + objParam[2].ToString());
                        //MessageBox.Show("[" + zyno + "]|[" + xm + "]自动费用上传完成!请查看日志", "提示");
                        getOrdersInfo(zyno);
                    }
                    else
                    {
                        WriteLog(sysdate + "  " + zyno + "|" + xm + "    " + objParam[2].ToString());
                        //MessageBox.Show("[" + zyno + "]|[" + xm + "]自动费用上传失败:" + objParam[2].ToString(), "提示");
                    }
                }
            }
            else
            {
                MessageBox.Show("请选中一个病人", "提示");
            }
        }

        private void tsmi_ref_Click(object sender, EventArgs e)
        {
            getPatientInfo("");
        }

        private void tsmi_ybinit_Click(object sender, EventArgs e)
        {
            object[] objParam = { CZYBH };
            objParam = yb_interface_sy_zrNew.YBINIT(objParam);
            if (objParam[1].ToString().Equals("1"))
            {
                MessageBox.Show("初始化成功");
            }
            else
            {
                MessageBox.Show("初始化失败|" + objParam[2].ToString());
            }
        }

        private void 退出ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //医保退出
            object[] objParam = { CZYBH, YWZQH };
            objParam = yb_interface_sy_zrNew.YBEXIT(objParam);
            Application.Exit();
        }

        private void 手动撤销上传费用ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string sysdate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            if (dgv_PatInfo.SelectedRows.Count != 0)
            {
                string zyno = dgv_PatInfo.CurrentRow.Cells["col_zyh"].Value.ToString();
                string xm = dgv_PatInfo.CurrentRow.Cells["col_xm"].Value.ToString();
                DialogResult dr = MessageBox.Show("确认撤销病人[" + zyno + "]|[" + xm + "]的已上传费用？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (dr == DialogResult.Yes)
                {
                    object[] objParam = new object[] { zyno, CZYBH, YWZQH, "自动上传撤销" };
                    objParam = yb_interface_sy_zrNew.YBZYSFDJCX(objParam);

                    if (Convert.ToInt32(objParam[1].ToString()) == 1)
                    {

                        WriteLog(sysdate + "  " + zyno + "|" + xm + "    " + objParam[2].ToString());
                        MessageBox.Show("[" + zyno + "]|[" + xm + "]费用上传撤销成功", "提示");
                        getOrdersInfo(zyno);
                    }
                    else
                    {
                        WriteLog(sysdate + "  " + zyno + "|" + xm + "    " + objParam[2].ToString());
                        MessageBox.Show("[" + zyno + "]|[" + xm + "]费用上传撤销失败:" + objParam[2].ToString(), "提示");
                    }
                }
            }
            else
            {
                MessageBox.Show("请选中一个病人", "提示");
            }
        }

        private void 刷新患者列表ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            getPatientInfo("");
        }

        private void 查看项目明细ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string jzlsh = this.dgv_PatInfo.SelectedRows[0].Cells["col_zyh"].Value.ToString();
            string Sql = string.Format(@"select  case   when  a.z3ybup IS NULL then '未上传' else '已上传' end as 医保上传状态,b.ybxmbh as 医保项目编号,b.ybxmmc as 医保项目名称,a.z3djxx as 单价,
                                        sum(case LEFT(a.z3endv,1) when '4' then -a.z3jzcs else a.z3jzcs end) as 数量,
                                        sum(case LEFT(a.z3endv,1) when '4' then -a.z3jzje else a.z3jzje end) as 总金额,
                                        a.z3item as 医院项目编号, a.z3name as 医院项目名称,max(a.z3date) as 计费时间,  b.sfxmzldm as 项目诊疗编码, b.sflbdm as 项目类别,b.sfxmdjdm as 项目类型,z3yzxh as 处方号,z3ghno+CONVERT(varchar(20),z3idxx) as 处方流水号,z3empn as 医生,z3ksno as 科室
                                        from zy03d a left join ybhisdzgmdrNew b on 
                                        a.z3item=b.hisxmbh where  LEFT(a.z3kind,1)in(2,4,5)  and a.z3zyno='{0}' 
                                        group by a.z3ybup, b.ybxmbh,b.ybxmmc,a.z3djxx,a.z3item,a.z3name,b.sfxmzldm,b.sflbdm,b.sfxmdjdm,z3yzxh,z3ghno+CONVERT(varchar(20),z3idxx),z3empn,z3ksno", jzlsh);//having sum(case LEFT(a.z3endv,1) when '4' then - a.z3jzcs else a.z3jzcs end)> 0
            string ErrStr = string.Empty;
            DataSet ds = helper.ExecuteDataSet(Sql, ref ErrStr);
            if (ds == null)
            {
                MessageBox.Show(ErrStr);
                return;
            }
            Frmfeedetail feedetail = new Frmfeedetail(ds);

            feedetail.ShowDialog();

        }

        private bool IsExecute = true;
        /// <summary>
        /// 自动监听，并上传费用
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer2_Tick_1(object sender, EventArgs e)
        {
            int i_nowHour = DateTime.Now.Hour;
            int i_nowmintus = DateTime.Now.Minute;
            if (i_nowHour == i_TIMEPOINT1 && i_nowmintus < pubMint)
            {

                autoOrdersSC();
            }
            else if (i_nowHour == i_TIMEPOINT && i_nowmintus < pubMint)
            {
                autoOrdersSC();
            }
        }

        private void timer1_Tick_1(object sender, EventArgs e)
        {

        }
    }
}
