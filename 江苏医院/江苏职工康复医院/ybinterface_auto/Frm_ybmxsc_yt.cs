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


namespace ybinterface_auto
{
    public partial class Frm_ybmxsc_yt : Form
    {
        IWork iWork = new Work();
        SqlHelper helper = new SqlHelper();
        string CZYBH = "";
        string CZYPWD = "";
        string YWZQH = "";
        int i_TIMEPOINT = 0;
        int i_TIMEPOINT1 = 0;
        int i_LisenceTime = 0;
        bool bFalg = false;

        public Frm_ybmxsc_yt()
        {
            InitializeComponent();
        }

        private void Frm_ybmxsc_ts_Load(object sender, EventArgs e)
        {
            bFalg = true;
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
            objParam=ybinterface_lib_yt.YBINIT(objParam);
            if (objParam[1].ToString().Equals("1"))
            {
                YWZQH = objParam[2].ToString();
                lbMsg.Text = "设定自动上传时间：【" + i_TIMEPOINT + "】时、【" + i_TIMEPOINT1 + "】时,上传状态：【开启】";
                timer2.Enabled = true;
                getPatientInfo("");
            }
            else
            {
                lbMsg.Text = "设定自动上传时间：【" + i_TIMEPOINT + "】时、【" + i_TIMEPOINT1 + "】时,上传状态：【未开启】";
                MessageBox.Show("初始化失败|" + objParam[2].ToString());
            }
        }

        internal void getPatientInfo(string param1)
        {
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
                                            select jzlsh,isnull(SUM(je),0.00) as scje from ybcfmxscfhdr where LEN(jzlsh)=8 and cxbz=1 group by jzlsh
                                            )
                                            select  distinct b.z1ghno as bah, b.z1zyno as zyh,b.z1hznm as xm,b.z1ksno as ksno,b.z1ksnm as ksmc,
                                            case when c.xb='2' then '女'
                                            when c.XB='1' then '男'
                                            else '未知' end xb,
                                            case when a.dqjbbz='2' then '1' else '0'end as ydrybz,
                                            a.ysdm as ysno,a.ysxm as ysxm,a.grbh,
                                            (select name from YBXMLBZD where CODE=a.yldylb and LBMC='医疗待遇类别') zglb,
                                            c.dwmc, 
                                            CONVERT(varchar(10),b.z1date,120) as ryrq,d.zje,isnull(e.scje,0.00) as scje,
                                            isnull(g.zhzbxje,0.00) as tcdbfy,
                                            isnull(g.tcjjzf,0.00) as tcjjzf,
                                            isnull(g.zhzf,0.00) as zhzf,
                                            isnull(g.dejjzf,0.00) as dejjzf,
                                            isnull(g.xjzf,0.00) as xjzf,
                                            isnull(h.errmsg,'') as errmsg
                                            from ybmzzydjdr  a 
                                            inner join zy01h b on a.jzlsh=b.z1zyno and left(b.z1endv,1)='0'
                                            inner join hisfy d on a.jzlsh=d.z3zyno
                                            left join ybickxx c on a.grbh=c.GRBH 
                                            left join ybscfy e on a.jzlsh=e.jzlsh
                                            left join zy03da f on a.jzlsh=f.z3zyno and z3kind=3 and left(z3payw,1)=6
                                            left join ybfyyjsdr g on a.jzlsh=g.jzlsh 
                                            left join ybzdscrz h on a.jzlsh=h.jzlsh
                                            where LEN(a.jzlsh)=8 and a.cxbz=1 and not exists(select 1 from ybfyjsdr where a.jzlsh=jzlsh and cxbz=1)  
                                            and (a.jzlsh like '%{0}%' or a.xm like '%{0}%') ", param1);
            DataSet ds = helper.ExecuteDataSet(strSql);
            dgv_PatInfo.AutoGenerateColumns = false;
            dgv_PatInfo.DataSource = ds.Tables[0].DefaultView;
        }

        internal void getOrdersInfo(string Param1)
        {
            string strsql = string.Format(@"select a.z3zyno,a.z3djxx as dj,case LEFT(a.z3endv,1) when '4' then -a.z3jzcs else a.z3jzcs end as sl,
                                            case LEFT(a.z3endv,1) when '4' then -a.z3jzje else a.z3jzje end as je,a.z3item as yyxmbh, a.z3name as yyxmmc, a.z3empn as ysdm, 
                                            a.z3kdys as ysxm,a.z3date as yysj,a.z3ksno as ksno, a.z3zxks as zxks, z3sfno as sfno,
                                            case when a.z3ybup is null then '未上传' else '已上传' end as ybup from zy03d a 
                                            where LEFT(a.z3kind,1)=2 and a.z3zyno='{0}'
                                            order by a.z3ybup", Param1);
            DataSet ds = helper.ExecuteDataSet(strsql);
            dgv_Detail.AutoGenerateColumns = false;
            dgv_Detail.DataSource = ds.Tables[0].DefaultView;

        }

        internal void autoOrdersSC()
        {
            int i_nowHour = DateTime.Now.Hour;
            if (i_TIMEPOINT == i_nowHour||i_TIMEPOINT1==i_nowHour)
            {
                i_LisenceTime = i_nowHour + 2;
                timer1.Enabled = false;
                Init();
                if (dgv_PatInfo.Rows.Count >= 1)
                {
                    List<string> liSQL = new List<string>();

                    for (int i = 0; i < dgv_PatInfo.Rows.Count; i++)
                    {
                        string sysdate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                        timer1.Enabled = false;
                        dgv_PatInfo.Rows[i].Selected = true;
                        string jzlsh = dgv_PatInfo.Rows[i].Cells["col_zyh"].Value.ToString();
                        string xm=dgv_PatInfo.Rows[i].Cells["col_xm"].Value.ToString();
                        string ydybbz = dgv_PatInfo.Rows[i].Cells["col_ydybbz"].Value.ToString();
                        string strSQL1 = string.Format(@"delete from ybzdscrz where jzlsh='{0}'", jzlsh);
                        liSQL.Add(strSQL1);
                        string errmsg = "";
                        helper.ExecuteSqlTran(liSQL, ref errmsg);
                        object[] objParam=null;
                        if (ydybbz.Equals("1"))
                            objParam = new object[] { jzlsh, "", 10, 0, CZYBH, YWZQH, "管理员" };
                        else
                            objParam = new object[] { jzlsh, "", 30, 0, CZYBH, YWZQH, "管理员" };
                        objParam = ybinterface_lib_yt.YBZYSFDJ(objParam);
                        if (Convert.ToInt32(objParam[1].ToString()) == 1)
                        {
                            dgv_PatInfo.Rows[i].Cells["col_scje"].Value = objParam[3].ToString();
                            WriteLog(sysdate + "  " + jzlsh + "|" + xm + "    " + objParam[2].ToString() +"上传金额:"+ objParam[3].ToString());
                            object[] objParam1 = { jzlsh, "9", "1", "0", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "0.00", CZYBH, YWZQH, "管理员" };

                            objParam1 = ybinterface_lib_yt.YBZYSFYJS(objParam1);
                            WriteLog(sysdate + "  " + jzlsh + "|" + xm + "    " + objParam1[2].ToString());
                            if (objParam1[1].ToString().Equals("1"))
                            {
                                dgv_PatInfo.Rows[i].Cells["col_tcdbfy"].Value = objParam1[3].ToString();
                                dgv_PatInfo.Rows[i].Cells["col_tcjjzf"].Value = objParam1[4].ToString();
                                dgv_PatInfo.Rows[i].Cells["col_zhzf"].Value = objParam1[5].ToString();
                                dgv_PatInfo.Rows[i].Cells["col_dejjzf"].Value = objParam1[6].ToString();
                                dgv_PatInfo.Rows[i].Cells["col_xjzf"].Value = objParam1[7].ToString();
                            }
                            else
                            {
                                dgv_PatInfo.Rows[i].Cells["col_errmsg"].Value = objParam1[2].ToString();
                            }
                        }
                        else
                        {
                            dgv_PatInfo.Rows[i].Cells["col_errmsg"].Value = objParam[2].ToString();
                            WriteLog(sysdate + "  " + jzlsh + "|" + xm + "    " + objParam[2].ToString());
                        }
                    }
                }
                getPatientInfo("");
               
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
            objParam = ybinterface_lib_yt.YBEXIT(objParam);
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
            i_LisenceTime = DateTime.Now.Hour;
            Init();

        }

        /// <summary>
        /// 自动监听，并上传费用
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer1_Tick(object sender, EventArgs e)
        {
            autoOrdersSC();
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            int i_nowHour = DateTime.Now.Hour;
            if (i_nowHour == i_LisenceTime)
            {
                if (!timer1.Enabled)
                {
                    timer1.Enabled = true;
                }
            }
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
                string ydybbz = dgv_PatInfo.CurrentRow.Cells["col_ydybbz"].Value.ToString();
                DialogResult dr = MessageBox.Show("确认上传病人[" + zyno + "]|["+xm+"]的费用？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (dr == DialogResult.Yes)
                {
                    List<string> liSQL = new List<string>();
                    string strSQL1 = string.Format(@"delete from ybzdscrz where jzlsh='{0}'", zyno);
                    liSQL.Add(strSQL1);
                    string errmsg = "";
                    helper.ExecuteSqlTran(liSQL, ref errmsg);

                    object[] objParam = null;
                    if (ydybbz.Equals("1"))
                        objParam = new object[] { zyno, "", 10, 0, CZYBH, YWZQH, "管理员" };
                    else
                        objParam = new object[] { zyno, "", 30, 0, CZYBH, YWZQH, "管理员" };
                    objParam = ybinterface_lib_yt.YBZYSFDJ(objParam);
                    if (Convert.ToInt32(objParam[1].ToString()) == 1)
                    {
                        dgv_PatInfo.CurrentRow.Cells["col_scje"].Value = objParam[3].ToString();
                        WriteLog(sysdate + "  " + zyno + "|" + xm + "    " + objParam[2].ToString() + "上传金额:" + objParam[3].ToString());

                        object[] objParam1 = { zyno, "9", "1", "0", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "0.00", CZYBH, YWZQH, "管理员" };
                        objParam1 = ybinterface_lib_yt.YBZYSFYJS(objParam1);
                        WriteLog(sysdate + "  " + zyno + "|" + xm + "    " + objParam1[2].ToString());
                        if (objParam1[1].ToString().Equals("1"))
                        {
                            dgv_PatInfo.CurrentRow.Cells["col_tcdbfy"].Value = objParam1[3].ToString();
                            dgv_PatInfo.CurrentRow.Cells["col_tcjjzf"].Value = objParam1[4].ToString();
                            dgv_PatInfo.CurrentRow.Cells["col_zhzf"].Value = objParam1[5].ToString();
                            dgv_PatInfo.CurrentRow.Cells["col_dejjzf"].Value = objParam1[6].ToString();
                            dgv_PatInfo.CurrentRow.Cells["col_xjzf"].Value = objParam1[7].ToString();
                        }
                        else
                        {
                            dgv_PatInfo.CurrentRow.Cells["col_errmsg"].Value = objParam1[2].ToString();
                        }
                    }
                    else
                    {
                        dgv_PatInfo.CurrentRow.Cells["col_errmsg"].Value = objParam[2].ToString();
                        WriteLog(sysdate + "  " + zyno + "|" + xm + "    " + objParam[2].ToString());
                        //MessageBox.Show("[" + zyno + "]|[" + xm + "]自动费用上传失败:" + objParam[2].ToString(), "提示");
                    }

                    //getPatientInfo("");
                    //getOrdersInfo("");
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
             object[] objParam = {CZYBH };
            objParam=ybinterface_lib_yt.YBINIT(objParam);
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
            objParam = ybinterface_lib_yt.YBEXIT(objParam);
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
                    object[] objParam = new object[] { zyno, CZYBH, YWZQH, "管理员" };
                    objParam = ybinterface_lib_yt.YBZYSFDJCX(objParam);

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
    }
}
