using Srvtools;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace ybinterface_lib
{
    public class yb_interface_sr_sryb
    {
        #region 医保接口
        #region 基本函数
        //异地读卡
        [DllImport("ylbxdyjk.dll", EntryPoint = "ybdsbk2", CallingConvention = CallingConvention.StdCall)]
        static extern int ybdsbk2(ref Int32 id,
                                StringBuilder xm,
                                StringBuilder xb,
                                StringBuilder csrq,
                                StringBuilder gmsfhm,
                                StringBuilder rylb,
                                StringBuilder dwmc,
                                StringBuilder xqmc,
                                StringBuilder rylbdm,
                                StringBuilder xzlxdm,
                                StringBuilder gwybz,
                                StringBuilder dbbz,
                                StringBuilder zyzt,
                                ref double xj,
                                ref double grzhye,
                                ref double ljzyzf,
                                ref Int32 ndzycs);
        //操作员登录
        [DllImport("ylbxdyjk.dll", EntryPoint = "ybdr2", CallingConvention = CallingConvention.StdCall)]
        static extern int ybdr2(Int32 handle, string czyxm, string mm);
        //读卡2
        [DllImport("ylbxdyjk.dll", EntryPoint = "ybdick2", CallingConvention = CallingConvention.StdCall)]
        public static extern int ybdick2(ref Int32 id,
                                         [MarshalAs(UnmanagedType.LPArray)] byte[] xm,
                                         [MarshalAs(UnmanagedType.LPArray)] byte[] xb,
                                         [MarshalAs(UnmanagedType.LPArray)] byte[] csrq,
                                         [MarshalAs(UnmanagedType.LPArray)] byte[] gmsfhm,
                                         [MarshalAs(UnmanagedType.LPArray)] byte[] zglb,
                                         ref double grzhye);
        //读卡211
        [DllImport("ylbxdyjk.dll", EntryPoint = "ybdick211", CallingConvention = CallingConvention.StdCall)]
        public static extern int ybdick211(ref Int32 id,
                                         [MarshalAs(UnmanagedType.LPArray)] byte[] xm,
                                         [MarshalAs(UnmanagedType.LPArray)] byte[] xb,
                                         [MarshalAs(UnmanagedType.LPArray)] byte[] csrq,
                                         [MarshalAs(UnmanagedType.LPArray)] byte[] gmsfhm,
                                         [MarshalAs(UnmanagedType.LPArray)] byte[] zglb,
                                         [MarshalAs(UnmanagedType.LPArray)] byte[] dwmc,
                                         ref double grzhye);
        //读卡215a
        [DllImport("ylbxdyjk.dll", EntryPoint = "ybdick215a", SetLastError = true, CharSet = CharSet.Ansi,
            ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern int ybdick215a(ref Int32 id,
                                         [MarshalAs(UnmanagedType.LPArray)] byte[] xm,
                                         [MarshalAs(UnmanagedType.LPArray)] byte[] xb,
                                         [MarshalAs(UnmanagedType.LPArray)] byte[] csrq,
                                         [MarshalAs(UnmanagedType.LPArray)] byte[] gmsfhm,
                                         [MarshalAs(UnmanagedType.LPArray)] byte[] zglb,
                                         [MarshalAs(UnmanagedType.LPArray)] byte[] dwmc,
                                         ref double grzhye, ref Int32 sfymm);
        //读卡215
        [DllImport("ylbxdyjk.dll", EntryPoint = "ybdick215", SetLastError = true, CharSet = CharSet.Ansi,
            ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern int ybdick215([MarshalAs(UnmanagedType.LPArray)] byte[] ryxx,
                                         ref double grzhye, ref Int32 sfymm);
        //返回错误信息
        [DllImport("ylbxdyjk.dll", EntryPoint = "ybmessage2", SetLastError = true, CharSet = CharSet.Ansi,
            ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr ybmessage2();

        [DllImport("ylbxdyjk.dll", EntryPoint = "ybcwxx2", SetLastError = true, CharSet = CharSet.Ansi,
            ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern int ybcwxx2(string errMsg);
        //返回详细错误信息
        [DllImport("ylbxdyjk.dll", EntryPoint = "ybxxcwxx2", SetLastError = true, CharSet = CharSet.Ansi,
            ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr ybxxcwxx2();
        //断开连接
        [DllImport("ylbxdyjk.dll", EntryPoint = "ybtc", SetLastError = true, CharSet = CharSet.Ansi,
            ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern int ybtc();
        #endregion
        #region 门诊函数
        //门诊处方明细上传
        [DllImport("ylbxdyjk.dll", EntryPoint = "mzcscf2", SetLastError = true, CharSet = CharSet.Ansi,
            ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern int mzcscf2(Int32 id, Int32 xmbm, string Yyxmbm, string Xmmc, string Jx, string gg, Int32 Xmlx, Int32 Ggdm, double Sl, double Dj, double Je);
        //门诊预结算/结算
        [DllImport("ylbxdyjk.dll", EntryPoint = "mzjs212", SetLastError = true, CharSet = CharSet.Ansi,
            ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern int mzjs212(Int32 id, double ylfhj, Int32 jsfs, string icd10dm, string jbzl, string ysxm, string kbrq, string mzh, string dgysbh, ref double xj, ref double grzh, ref double grzhye, ref double tcj);
       //门诊统筹预结算/结算
        [DllImport("ylbxdyjk.dll", EntryPoint = "mztcjs2", SetLastError = true, CharSet = CharSet.Ansi,
            ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern int mztcjs2(Int32 id, double ylfhj, Int32 jsfs, string xm, string sfzh, string icd10dm, string jbzl, string ysxm, string kbrq, string mzh, string dgysbh, string mm, ref double xj, ref double grzh, ref double grzhye, ref double tcj);
        //门诊预结算/结算(含密码)
        [DllImport("ylbxdyjk.dll", EntryPoint = "mzjs215", SetLastError = true, CharSet = CharSet.Ansi,
        ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern int mzjs215(Int32 id, double ylfhj, Int32 jsfs, string icd10dm, string jbzl, string ysxm, string kbrq, string mzh, string dgysbh, string mm, ref double xj, ref double grzh, ref double grzhye, ref double tcj);
        //门诊保存信息提交或回滚
        [DllImport("ylbxdyjk.dll", EntryPoint = "mzbc", SetLastError = true, CharSet = CharSet.Ansi,
        ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern int mzbc(Int32 id, Boolean bz);
        //打印门诊结算单
        [DllImport("ylbxdyjk.dll", EntryPoint = "mzdyjsd2", SetLastError = true, CharSet = CharSet.Ansi,
        ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern int mzdyjsd2(Int32 id, string kbrq, string ylfhj, string grzh, string tcj);
        //打印门诊结算单
        [DllImport("ylbxdyjk.dll", EntryPoint = "mzdyjsd211", SetLastError = true, CharSet = CharSet.Ansi,
        ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern int mzdyjsd211(Int32 id, string mzh);
        //门诊退费
        [DllImport("ylbxdyjk.dll", EntryPoint = "mztf211", SetLastError = true, CharSet = CharSet.Ansi,
        ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern int mztf211(Int32 id, double yylf, double tfje, string mzh, ref double txj, ref double tgrzh, ref double grzhye, ref double ttcj);
        //查询个人门诊刷卡是否开通
        [DllImport("ylbxdyjk.dll", EntryPoint = "mzktzt2", SetLastError = true, CharSet = CharSet.Ansi,
        ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern int mzktzt2(Int32 id, ref Int32 mzkt, ref Int32 mxbkt, ref Int32 mztckt,
                                        [MarshalAs(UnmanagedType.LPArray)] byte[] mzyy,
                                        [MarshalAs(UnmanagedType.LPArray)] byte[] mxbyy,
                                        [MarshalAs(UnmanagedType.LPArray)] byte[] mxbmc,
                                        [MarshalAs(UnmanagedType.LPArray)] byte[] mztcyy);
        #endregion
        #region 住院函数
        //住院登记
        [DllImport("ylbxdyjk.dll", EntryPoint = "zyrydj2", SetLastError = true, CharSet = CharSet.Ansi,
            ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern int zyrydj2(Int32 id, string zyh, string bqmc, string ch, string ryrq, ref double ljzyf, ref double grzhye, ref double qfxbz, ref double fdbl, ref Int32 zycs);
        //住院登记(新接口)
        [DllImport("ylbxdyjk.dll", EntryPoint = "zyrydj216", SetLastError = true, CharSet = CharSet.Ansi,
            ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern int zyrydj216(Int32 id, string xm, string sfzh, string zyh, string bqmc, string ch, string ryrq, string jbzd, string icd10m, ref double ljzyf, ref double grzhye, ref double qfxbz, ref Int32 zycs);
        //住院登记(异地通用老接口)
        [DllImport("ylbxdyjk.dll", EntryPoint = "zyrydj217", SetLastError = true, CharSet = CharSet.Ansi,
            ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern int zyrydj217(Int32 id, string xm, string sfzh, string zyh, string bqmc, string ch, string ryrq, string jbzd, string icd10m, string ksdm, string dgysbm, string ysxm, string blh, string jzbz,string wsbz, string klx, ref double ljzyf, ref double grzhye, ref double qfxbz, ref Int32 zycs);
        //住院病情登记
        [DllImport("ylbxdyjk.dll", EntryPoint = "zybqdj212", SetLastError = true, CharSet = CharSet.Ansi,
            ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern int zybqdj212(Int32 id, string ryrq, string zyh, string bqmc, string ch, string icd10_1, string jbzd_1, string icd10_2, string jbzd_2, string icd10_3, string jbzd_3, string azhl, string ysxm, string dgysbh);
        //病情查询
        [DllImport("ylbxdyjk.dll", EntryPoint = "zybqxx2", SetLastError = true, CharSet = CharSet.Ansi,
            ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern int zybqxx2(Int32 id, string ryrq, string zyh, string bqmc, string ch, string jbzl, string ysxm, string bqjs);
        //住院登记撤销
        [DllImport("ylbxdyjk.dll", EntryPoint = "zycxdj2", SetLastError = true, CharSet = CharSet.Ansi,
            ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern int zycxdj2(Int32 id, string zyh);
        //住院处方明细上报
        [DllImport("ylbxdyjk.dll", EntryPoint = "zycscf211", SetLastError = true, CharSet = CharSet.Ansi,
            ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern int zycscf211(Int32 id, Int32 xmbm, string yyxmbm, string xmmc, string jx, string gg, Int32 xmlx, double sl, double dj, double je, string clfs, string yyrq);
        //住院处方结算
        [DllImport("ylbxdyjk.dll", EntryPoint = "zycsjs2", SetLastError = true, CharSet = CharSet.Ansi,
            ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern int zycsjs2(Int32 id, ref double ylf, ref double tczf, ref double grzhye);
        //住院费用构成
        [DllImport("ylbxdyjk.dll", EntryPoint = "zyfygc2", SetLastError = true, CharSet = CharSet.Ansi,
            ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern int zyfygc2(Int32 id, [MarshalAs(UnmanagedType.LPArray)] byte[] fyxx, ref Int32 zycs, ref int fdhs,
                                                     [MarshalAs(UnmanagedType.LPArray)] byte[] fd1,
                                                     [MarshalAs(UnmanagedType.LPArray)] byte[] fd2,
                                                     [MarshalAs(UnmanagedType.LPArray)] byte[] fd3);
        //住院预结算/结算
        [DllImport("ylbxdyjk.dll", EntryPoint = "zycyjs211", SetLastError = true, CharSet = CharSet.Ansi,
            ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern int zycyjs211(Int32 id, Int32 ygrzhzf, Int32 jsfs, string cyrq, double ylfhj, ref double ylf, ref double grzf,
                                            ref double tcjz, ref double ickjz, ref double zfxj, ref double grzhye);
        //住院保存信息提交或回滚
        [DllImport("ylbxdyjk.dll", EntryPoint = "zybc", SetLastError = true, CharSet = CharSet.Ansi,
            ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public extern static int zybc(Int32 id, Boolean bz);
        //住院处方明细撤销
        [DllImport("ylbxdyjk.dll", EntryPoint = "zysccf2", SetLastError = true, CharSet = CharSet.Ansi,
            ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public extern static int zysccf2(Int32 id);
        //取消出院
        [DllImport("ylbxdyjk.dll", EntryPoint = "zyqxcy", SetLastError = true, CharSet = CharSet.Ansi,
            ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public extern static int zyqxcy(Int32 id);
        //打印出院结算单
        [DllImport("ylbxdyjk.dll", EntryPoint = "zydyjsd2", SetLastError = true, CharSet = CharSet.Ansi,
           ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public extern static int zydyjsd2(Int32 id, string zyh);
        //出院结算费明细
        [DllImport("ylbxdyjk.dll", EntryPoint = "zyjs_fymx", SetLastError = true, CharSet = CharSet.Ansi,
            ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public extern static int zyjs_fymx(Int32 id, string zyh, [MarshalAs(UnmanagedType.LPArray)] byte[] fymx);
        //获取个人信息
        [DllImport("ylbxdyjk.dll", EntryPoint = "ybgrxx2", SetLastError = true, CharSet = CharSet.Ansi,
            ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public extern static int ybgrxx2(Int32 id, string xm, string gmsfhm, string csrq, string xb, string zglb, string dwmc,
                                           ref double ljmzf, ref double ljzyf, ref double ljgrzhzf, ref double ljtczf, ref Int32 zycs, ref Int32 mzbbz);

        //获取个人信息
        [DllImport("ylbxdyjk.dll", EntryPoint = "ybgrxx217", SetLastError = true, CharSet = CharSet.Ansi,
            ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public extern static int ybgrxx217(Int32 id, string xm, string gmsfhm, string csrq, string xb, string zglb, string dwmc,
                                           ref double ljmzf, ref double ljzyf, ref double ljgrzhzf, ref double ljtczf, ref Int32 zycs, ref Int32 mzbbz,
                                            string qxmc, string rybz);
        //查询个人住院即时结算是否开通
        [DllImport("ylbxdyjk.dll", EntryPoint = "zyktzt2", SetLastError = true, CharSet = CharSet.Ansi,
            ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public extern static int zyktzt2(Int32 id, string rq, ref Int32 zykt, [MarshalAs(UnmanagedType.LPArray)] byte[] zyyy);
        #endregion
        #endregion

        #region 变量
        internal static string YWBH = string.Empty; //业务编号
        internal static string CZYBH = string.Empty;//操作员工号
        internal static string YWZQH = string.Empty;//业务周期号
        internal static string JYLSH = string.Empty;//交易流水号
        internal static string ZXBM = "0000";       //中心编码
        internal static string LJBZ = "1";          //联机标志
        internal static string DQJBBZ = "1";        //地区级别标志 1-本市级 2-本省异地  3-其他

        static IWork iWork = new Work();
        static string xmlPath = AppDomain.CurrentDomain.BaseDirectory;
        static List<Item1> lItem = iWork.getXmlConfig1(xmlPath + "EEPNetClient.exe.config");
        internal static string YLGHBH = lItem[0].DDYLJGBH; //医疗机构编号
        internal static string DDYLJGMC = lItem[0].DDYLJGMC;//医院名称
        internal static string YBIP = lItem[0].YBIP;        //医保IP地址

        bool zhzfbz = false;//账户支付标识

        #endregion

        #region 接口方法

        #region 医保错误信息返馈
        // 每次医保接口调用出错后，调用此方法返回错误信息
        internal static string errorMessage()
        {
            string strReturn = string.Empty;
            IntPtr imessage = ybmessage2();
            strReturn = "错误信息:" + Marshal.PtrToStringAnsi(imessage);

            IntPtr ixxcwxx = ybxxcwxx2();
            string strMsg = Marshal.PtrToStringAnsi(ixxcwxx);
            if (!string.IsNullOrEmpty(strMsg.Trim()))
                strReturn += "\r\n详细错误信息:" + Marshal.PtrToStringAnsi(ixxcwxx);
            return strReturn;
        }
        #endregion

        #region 初始化
        public static object[] YBINIT(object[] param)
        {

            //Ping医保网
            Ping ping = new Ping();
            PingReply pingReply = ping.Send(YBIP);
            if (pingReply.Status == IPStatus.Success)
            {
                //当前日期时间
                string sysdate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                string hisUserID = CliUtils.fLoginUser;
                //入参格式 handle|用户名|密码|
                Int32 handle = Int32.Parse("0");
                string czyxm = string.Empty;
                string czymm = string.Empty;

                string strSql = string.Format(@"select b1ybno,b1ybpw from bz01h where b1empn='{0}' ", hisUserID);
                DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                if (ds.Tables[0].Rows.Count == 0)
                    return new object[] { 0, 0, "该用户登记失败｜医保账户及密码错误" };
                else
                {
                    czyxm = ds.Tables[0].Rows[0]["b1ybno"].ToString();
                    czymm = ds.Tables[0].Rows[0]["b1ybpw"].ToString();
                }
                MessageBox.Show("欢迎使用" + DDYLJGMC + "程序");

                WriteLog(sysdate + "  用户" + czyxm + " 进入医保初始化...");
                WriteLog(sysdate + "  入参|" + handle + "|" + czyxm + "|" + czymm);
                int falg = ybdr2(handle, czyxm, czymm);
                if (falg >= 0)
                {
                    WriteLog(sysdate + "  进入医保初始化成功");
                    return new object[] { 0, 1, "初始化及用户登录成功！" };
                }
                else
                {
                    string errMsg = errorMessage();
                    WriteLog(sysdate + "  进入医保初始化失败|" + errMsg);
                    return new object[] { 0, 0, "初始化及用户登录失败|" + errMsg };
                }
            }
            else
                return new object[] { 0, 0, "未连接医保网" };
        }
        #endregion

        #region 医保退出
        public static object[] YBEXIT(object[] objParam)
        {
            //当前日期时间
            string sysdate = GetServerDateTime();
            try
            {
                int falg = ybtc();

                if (0 <= falg)
                {
                    WriteLog(sysdate + "  医保退出成功");
                    return new object[] { 0, 1, "断开连接成功！" };
                }
                else
                {
                    string errMsg = errorMessage();
                    return new object[] { 0, 0, "断开连接出错！\r\n" + errMsg };
                }
            }
            catch (Exception ex)
            {
                WriteLog(sysdate + ex.Message);
                return new object[] { 0, 0, ex.Message };
            }
        }
        #endregion

        #region 门诊读卡
        public static object[] YBMZDK(object[] objParam)
        {
            string sysdate = GetServerDateTime();//当前时间
            try
            {
                #region 定义变量
                Int32 id = 0;           //卡号
                StringBuilder xm = new StringBuilder(16);
                StringBuilder xb = new StringBuilder(16);
                StringBuilder csrq = new StringBuilder(16);
                StringBuilder gmsfhm = new StringBuilder(16);
                StringBuilder rylb = new StringBuilder(16);
                StringBuilder dwmc = new StringBuilder(50);
                StringBuilder xqmc = new StringBuilder(16);
                StringBuilder rylbdm = new StringBuilder(16);
                StringBuilder xzlxdm = new StringBuilder(16);
                StringBuilder gwybz = new StringBuilder(16);
                StringBuilder dbbz = new StringBuilder(16);
                StringBuilder zyzt = new StringBuilder(16);
                double xj = 0.00;
                double grzhye = 0.00;
                double ljzyzf = 0.00;
                Int32 ndzycs = 0;
                #endregion
                string isYD = objParam[0].ToString();
                byte[] ryxx = new byte[102400];        //人员信息
                Int32 sfymm = 0;
                WriteLog(sysdate + isYD+"   进入医保读卡...");
                WriteLog(sysdate + "  入参|");

                int falg = 0;
                if (isYD == "1")
                {
                    falg = ybdsbk2(ref id, xm, xb,csrq,gmsfhm,rylb,dwmc,xqmc,rylbdm,xzlxdm,gwybz,dbbz,zyzt,ref xj,ref grzhye,ref ljzyzf,ref ndzycs);
                }
                else
                {
                    falg = ybdick215(ryxx, ref grzhye, ref sfymm);
                }

                if (falg == 0)
                {

                    
                    outParams_dk op = new outParams_dk();
                    if (isYD == "1")
                    {
                        WriteLog("异地医保读出成功|出参" + id.ToString() + 
                            xm.ToString() + xb.ToString() + csrq.ToString() + 
                            gmsfhm.ToString() + rylb.ToString() + dwmc.ToString() + 
                            xqmc.ToString() + rylbdm.ToString() + xzlxdm.ToString() + 
                            gwybz.ToString() + dbbz.ToString() + zyzt.ToString() + xj.ToString() + 
                            grzhye.ToString() + ljzyzf.ToString() + ndzycs.ToString());
                        op.Grbh = id.ToString(); //个人编号
                        op.Xm = xm.ToString(); //姓名
                        op.Xb = xb.ToString(); ; //性别
                        op.Csrq = csrq.ToString(); //出生日期
                        op.Sfhz = gmsfhm.ToString(); //公民身份号码
                        op.Yldylb = rylb.ToString(); //享受类别
                        op.Dwmc = dwmc.ToString(); //单位名称
                        op.Qxmc = xqmc.ToString(); //参保地名称
                        op.Zhye = grzhye.ToString();    //账户余额
                        op.Ispwd = sfymm.ToString();    //卡是否有密码
                        if (op.Yldylb.Contains("在岗") || op.Yldylb.Contains("在职") || op.Yldylb.Contains("退休") || op.Yldylb.Contains("离休"))
                            op.Jflx = "0202"; //职工医保
                        else
                            op.Jflx = "0203"; //居民医保

                        if (DDYLJGMC == "区医保")
                        {
                            if (op.Qxmc.Contains("信州区") == false)
                            {
                                return new object[] { 0, 0, "该医保卡为市本级，请在市本级医保程序中使用" };
                            }
                        }
                        if (DDYLJGMC == "市医保")
                        {
                            if (op.Qxmc.Contains("信州区"))
                            {
                                return new object[] { 0, 0, "该医保卡为区卡，请在信州区医保程序中使用" };
                            }
                        }


                        #region 获取个人信息
                        WriteLog(sysdate + "   进入获取个人信息217...");
                        WriteLog(sysdate + "  入参|" + id);
                        string zglb = "";//享受类别
                        double ljmzf = 0.00; //年度内累计门诊费
                        double ljzyf = 0.00; //年度内累计住院费
                        double ljgrzhzf = 0.00; //累计个人帐户支付
                        double ljtczf = 0.00;//累计统筹支付
                        Int32 zycs = 0; //住院次数
                        int mzbbz = 0; //是登记了慢性病
                        string rybz = ""; //人员标志，如灵活就业、低保、建档立卡等
                        string qxmc = ""; //县区名称，参保地


                        //falg = ybgrxx2(id, xm, gmsfhm, csrq, xb, zglb, dwmc, ref ljmzf, ref ljzyf, ref ljgrzhzf, ref ljtczf, ref zycs, ref mzbbz);
                        op.Ljmzzf = ljmzf.ToString();
                        op.Ljzyzf = ljzyf.ToString();
                        op.Bnzhzclj = ljgrzhzf.ToString();
                        op.Bntczclj = ljtczf.ToString();
                        op.Bnzycs = zycs.ToString();
                        #endregion



                        #region 查询个人门诊刷卡是否开通
                        string mzkt = string.Empty;       //普通门诊是否开通  (1开通 0未开通)
                        string mzyys = string.Empty;      //门诊没开通的原因
                        string mxbkt = string.Empty;      //慢性病门诊是否开通 (1开通 0未开通)
                        string mxbyys = string.Empty;     //慢性病没有开通的原因
                        string mxbmcs = string.Empty;     //慢性病名称
                        string mztckt = string.Empty;     //门诊统筹是否开通 (1开通 0未开通)
                        string mztcyys = string.Empty;    //门诊统筹没有开通的原因
                        string bz = string.Empty;
                        object[] objParam1 = { id };
                        objParam = YBMZKZT(objParam1);
                        if (objParam[1].ToString().Equals("1"))
                        {
                            /*
                             mzkt普通门诊是否开通、
                             * mxbkt慢性病门诊是否开通、
                             * mztckt门诊统筹是否开通、
                             * mzyy门诊没开通的原因、
                             * mxbyy慢性病没有开通的原因、
                             * mxbmc慢性病名称、
                             * mztcyy门诊统筹没有开通的原因
                             */
                            op.Ptmzbz = objParam[3].ToString();
                            op.Ptmzyy = objParam[4].ToString();
                            op.Mtbz = objParam[5].ToString();
                            op.Mxbmzyy = objParam[6].ToString();
                            op.Mztcbz = objParam[8].ToString();
                            op.Mztcyy = objParam[9].ToString();




                            if (op.Mtbz == "1")
                            {
                                bz = objParam[7].ToString();
                                if (bz.Contains(',')&&bz.Contains('|'))
                                {
                                    string[] mxb = bz.Split(',');
                                    bz = mxb[0].ToString();
                                    string[] mxbz = bz.Split('|');
                                    op.Mtbzbm = mxbz[0].ToString();
                                    op.Mtbzmc = mxbz[1].ToString();
                                }
                                else
                                {
                                    if (bz.Contains('|'))
                                    {
                                        string[] mxbz = bz.Split('|');
                                        op.Mtbzbm = mxbz[0].ToString();
                                        op.Mtbzmc = mxbz[1].ToString();
                                    }                                   
                                }
                                

                            //    bz = objParam[7].ToString();

                            //    if (bz.Contains(','))
                            //    {
                            //        //存在双病种
                            //        string[] mxb = bz.Split(',');
                            //        DialogResult dr = MessageBox.Show("当前病人存在双病种" + mxb[0].ToString() + "和" + mxb[1].ToString() + "是否选择第一病种?", "提示", MessageBoxButtons.YesNoCancel);
                            //        if (dr == DialogResult.Yes)
                            //        {//获取第一病种
                            //            bz = mxb[0].ToString();
                            //            string[] mxbz = bz.Split('|');
                            //            op.Mtbzbm = mxbz[0].ToString();
                            //            op.Mtbzmc = mxbz[1].ToString();

                            //        }
                            //        else if (dr == DialogResult.No)
                            //        {//获取第二病种
                            //            bz = mxb[1].ToString();
                            //            string[] mxbz = bz.Split('|');
                            //            op.Mtbzbm = mxbz[0].ToString();
                            //            op.Mtbzmc = mxbz[1].ToString();
                            //        }
                            //    }
                            //    else
                            //    {
                            //        bz = bz[0].ToString();
                            //        string[] mxbz = bz.Split('|');
                            //        op.Mtbzbm = mxbz[0].ToString();
                            //        op.Mtbzmc = mxbz[1].ToString();
                            //    }
                            }
                        }
                        else
                            return new object[] { 0, 0, objParam[2].ToString() };
                        #endregion


                    }
                    else
                    {

                        #region 出参
                        /* 
                        * 个人标识号|姓名|性别|出生日期|公民身份号码|享受类别|单位名称|参保地名称
                        */
                        string ryxxs = Encoding.Default.GetString(ryxx, 0, ryxx.Length).Replace("\0", ""); //姓名
                        WriteLog(sysdate + "  医保读出成功|出参|" + ryxxs);
                        string[] ry = ryxxs.Split(';');
                        if (ry.Length > 1)
                        {
                            frm_dk dk = new frm_dk(ryxxs);
                            dk.ShowDialog();
                            ryxxs = dk.sValue;
                        }
                        string[] s = ryxxs.Split('|');
                        op.Grbh = s[0]; //个人编号
                        op.Xm = s[1]; //姓名
                        op.Xb = s[2]; //性别
                        op.Csrq = s[3]; //出生日期
                        op.Sfhz = s[4]; //公民身份号码
                        op.Yldylb = s[5]; //享受类别
                        op.Dwmc = s[6]; //单位名称
                        op.Qxmc = s[7]; //参保地名称
                        op.Zhye = grzhye.ToString();    //账户余额
                        op.Ispwd = sfymm.ToString();    //卡是否有密码
                        if (op.Yldylb.Contains("在岗") || op.Yldylb.Contains("在职") || op.Yldylb.Contains("退休") || op.Yldylb.Contains("离休"))
                            op.Jflx = "0202"; //职工医保
                        else
                            op.Jflx = "0203"; //居民医保

                        id = Convert.ToInt32(op.Grbh);

                        if (DDYLJGMC == "区医保")
                        {
                            if (op.Qxmc.Contains("信州区") == false)
                            {
                                return new object[] { 0, 0, "该医保卡为市本级，请在市本级医保程序中使用" };
                            }
                        }
                        if (DDYLJGMC == "市医保")
                        {
                            if (op.Qxmc.Contains("信州区"))
                            {
                                return new object[] { 0, 0, "该医保卡为区卡，请在信州区医保程序中使用" };
                            }
                        }


                        #region 获取个人信息
                        WriteLog(sysdate + "   进入获取个人信息217...");
                        WriteLog(sysdate + "  入参|" + id);
                        string zglb = "";//享受类别
                        double ljmzf = 0.00; //年度内累计门诊费
                        double ljzyf = 0.00; //年度内累计住院费
                        double ljgrzhzf = 0.00; //累计个人帐户支付
                        double ljtczf = 0.00;//累计统筹支付
                        Int32 zycs = 0; //住院次数
                        int mzbbz = 0; //是登记了慢性病
                        string rybz = ""; //人员标志，如灵活就业、低保、建档立卡等
                        string qxmc = ""; //县区名称，参保地


                        //falg = ybgrxx2(id, xm, gmsfhm, csrq, xb, zglb, dwmc, ref ljmzf, ref ljzyf, ref ljgrzhzf, ref ljtczf, ref zycs, ref mzbbz);
                        op.Ljmzzf = ljmzf.ToString();
                        op.Ljzyzf = ljzyf.ToString();
                        op.Bnzhzclj = ljgrzhzf.ToString();
                        op.Bntczclj = ljtczf.ToString();
                        op.Bnzycs = zycs.ToString();
                        #endregion

                        #region 查询个人门诊刷卡是否开通
                        string mzkt = string.Empty;       //普通门诊是否开通  (1开通 0未开通)
                        string mzyys = string.Empty;      //门诊没开通的原因
                        string mxbkt = string.Empty;      //慢性病门诊是否开通 (1开通 0未开通)
                        string mxbyys = string.Empty;     //慢性病没有开通的原因
                        string mxbmcs = string.Empty;     //慢性病名称
                        string mztckt = string.Empty;     //门诊统筹是否开通 (1开通 0未开通)
                        string mztcyys = string.Empty;    //门诊统筹没有开通的原因
                        object[] objParam1 = { id };
                        objParam = YBMZKZT(objParam1);
                        if (objParam[1].ToString().Equals("1"))
                        {
                            /*
                             mzkt普通门诊是否开通、
                             * mxbkt慢性病门诊是否开通、
                             * mztckt门诊统筹是否开通、
                             * mzyy门诊没开通的原因、
                             * mxbyy慢性病没有开通的原因、
                             * mxbmc慢性病名称、
                             * mztcyy门诊统筹没有开通的原因
                             */
                            op.Ptmzbz = objParam[3].ToString();
                            op.Ptmzyy = objParam[4].ToString();
                            op.Mtbz = objParam[5].ToString();
                            op.Mxbmzyy = objParam[6].ToString();
                            op.Mtbzbm = "";
                            op.Mtbzmc = objParam[7].ToString();
                            op.Mztcbz = objParam[8].ToString();
                            op.Mztcyy = objParam[9].ToString();
                        }
                        else
                            return new object[] { 0, 0, objParam[2].ToString() };
                        #endregion



                        #endregion
                    }

                    #region 返回数据处理
                    op.Dwbh = "";
                    op.Kh = "";
                    if (op.Xb.Equals("男"))
                        op.Xb = "1";
                    else
                        op.Xb = "2";
                    op.Nl = (DateTime.Now.Year - Convert.ToDateTime(op.Csrq).Year).ToString();
                    op.Ybklx = "0";
                    #endregion
                    /*
                     * 个人编号|单位编号|身份证号|姓名|性别|
                     * 民族|出生日期|社会保障卡卡号|医疗待遇类别|人员参保状态|
                     * 异地人员标志|统筹区号|年度|在院状态|帐户余额|
                     * 本年医疗费累计|本年帐户支出累计|本年统筹支出累计|本年救助金支出累计|本年公务员补助基金累计|
                     * 本年城镇居民门诊统筹支付累计|进入统筹费用累计|进入救助金费用累计|起付标准累计|本年住院次数|
                     * 单位名称|年龄|参保单位类型|经办机构编码|缴费类型|
                     * 医保门慢、特资质|医保门慢、特病种说明|医疗类别代码|慢、特病编码|慢、特病名称|
                     * 医保卡类型|是否有卡密码|
                     */
                    string outParams = op.Grbh + "|" + op.Dwbh + "|" + op.Sfhz + "|" + op.Xm + "|" + op.Xb + "|" +
                       op.Mz + "|" + op.Csrq + "|" + op.Kh + "|" + op.Yldylb + "|" + op.Rycbzt + "|" +
                       op.Ydrybz + "|" + op.Tcqh + "|" + op.Nd + "|" + op.Zyzt + "|" + op.Zhye + "|" +
                       op.Bnylflj + "|" + op.Bnzhzclj + "|" + op.Bntczclj + "|" + op.Bndbyljjzflj + "|" + op.Bngwybzjjlj + "|" +
                       op.Bnczjmmztczflj + "|" + op.Jrtcfylj + "|" + op.Jrdbfylj + "|" + op.Qfbzlj + "|" + op.Bnzycs + "|" +
                       op.Dwmc + "|" + op.Nl + "|" + op.Cbdwlx + "|" + op.Jbjgbm + "|" + op.Jflx + "|"
                       + op.Mtbz + "|" + op.Mtmsg + "|" + op.Yllb + "|" + op.Mtbzbm + "|" + op.Mtbzmc + "|"
                       + op.Ybklx + "|" + op.Ispwd + "|";

                    //插入表YBICKXX
                    List<string> liSQL = new List<string>();

                    string strSql = string.Format("delete from YBICKXX where grbh='{0}'", op.Grbh);
                    liSQL.Add(strSql);
                    string strSQL = string.Format(@"select id from ybickxx where grbh='{0}'", id);
                    liSQL.Add(strSQL);
                    strSQL = string.Format(@"insert ybickxx(grbh,xm,xb,csrq,gmsfhm,zglb,dwmc,qxmc,grzhye,sfymm,
                                            ljmzf,ljzyf,ljgrzhzf,ljtczf,zycs,mzbbz,MZKT,MZYY,MXBKT,
                                            MXBYY,MXBMC,MXBBM,MZTCKT,MZTCYY,SYSDATE) values(
                                            '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}',
                                            '{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}',
                                              '{19}','{20}','{21}','{22}','{23}','{24}' )",
                                            op.Grbh, op.Xm, op.Xb, op.Csrq, op.Sfhz, op.Yldylb, op.Dwmc, op.Qxmc, op.Zhye, op.Ispwd,
                                            op.Ljmzzf, op.Ljzyzf, op.Bnzhzclj, op.Bntczclj, op.Bnzycs, op.Mtbz, op.Ptmzbz, op.Ptmzbz, op.Mtbz,
                                            op.Mxbmzyy, op.Mtbzmc,op.Mtbzbm, op.Mztcbz, op.Mztcyy, sysdate);
                    liSQL.Add(strSQL);
                    object[] obj = liSQL.ToArray();
                    obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);
                    if (obj[1].ToString() == "1")
                    {
                        WriteLog(sysdate + "   进入医保读卡成功|" + outParams);
                        return new object[] { 0, 1, outParams };
                    }
                    else
                    {
                        WriteLog(sysdate + "   进入医保读卡成功|本地数据操作失败|" + obj[2].ToString());
                        return new object[] { 0, 0, "读卡成功|本地数据操作失败|" + obj[2].ToString() };
                    }
                }
                else
                {
                    string errMsg = errorMessage();
                    WriteLog(sysdate + "   进入医保读卡失败|" + errMsg);
                    return new object[] { 0, 0, errMsg };
                }
            }
            catch (Exception ex)
            {
                WriteLog(sysdate + "   进入门诊医保读卡异常|" + ex.Message);
                return new object[] { 0, 0, ex.Message };
            }
        }
        #endregion

        #region 住院读卡
        public static object[] YBZYDK(object[] objParam)
        {
            string sysdate = GetServerDateTime();//当前时间
            try
            {
                #region 定义变量
                Int32 id = 0;           //卡号
                StringBuilder xm = new StringBuilder(16);
                StringBuilder xb = new StringBuilder(16);
                StringBuilder csrq = new StringBuilder(16);
                StringBuilder gmsfhm = new StringBuilder(16);
                StringBuilder rylb = new StringBuilder(16);
                StringBuilder dwmc = new StringBuilder(50);
                StringBuilder xqmc = new StringBuilder(16);
                StringBuilder rylbdm = new StringBuilder(16);
                StringBuilder xzlxdm = new StringBuilder(16);
                StringBuilder gwybz = new StringBuilder(16);
                StringBuilder dbbz = new StringBuilder(16);
                StringBuilder zyzt = new StringBuilder(16);
                double xj = 0.00;
                double grzhye = 0.00;
                double ljzyzf = 0.00;
                Int32 ndzycs = 0;
                #endregion
                string isYD = objParam[0].ToString();
                byte[] ryxx = new byte[102400];        //人员信息
                Int32 sfymm = 0;
                WriteLog(sysdate + isYD + "   进入医保读卡...");
                WriteLog(sysdate + "  入参|");

                int falg = 0;
                if (isYD == "1")
                {
                    falg = ybdsbk2(ref id, xm, xb, csrq, gmsfhm, rylb, dwmc, xqmc, rylbdm, xzlxdm, gwybz, dbbz, zyzt, ref xj, ref grzhye, ref ljzyzf, ref ndzycs);
                }
                else
                {
                    falg = ybdick215(ryxx, ref grzhye, ref sfymm);
                }

                if (falg == 0)
                {

                    outParams_dk op = new outParams_dk();
                    if (isYD == "1")
                    {
                        WriteLog("异地医保读出成功|出参" + id.ToString() + 
                            xm.ToString() + xb.ToString() + csrq.ToString() + 
                            gmsfhm.ToString() + rylb.ToString() + dwmc.ToString() + 
                            xqmc.ToString() + rylbdm.ToString() + xzlxdm.ToString() + 
                            gwybz.ToString() + dbbz.ToString() + zyzt.ToString() + xj.ToString() + 
                            grzhye.ToString() + ljzyzf.ToString() + ndzycs.ToString());
                        op.Grbh = id.ToString(); //个人编号
                        op.Xm = xm.ToString(); //姓名
                        op.Xb = xb.ToString(); ; //性别
                        op.Csrq = csrq.ToString(); //出生日期
                        op.Sfhz = gmsfhm.ToString(); //公民身份号码
                        op.Yldylb = rylb.ToString(); //享受类别
                        op.Dwmc = dwmc.ToString(); //单位名称
                        op.Qxmc = xqmc.ToString(); //参保地名称
                        op.Zhye = grzhye.ToString();    //账户余额
                        op.Ispwd = sfymm.ToString();    //卡是否有密码
                        if (op.Yldylb.Contains("在岗") || op.Yldylb.Contains("在职") || op.Yldylb.Contains("退休") || op.Yldylb.Contains("离休"))
                            op.Jflx = "0202"; //职工医保
                        else
                            op.Jflx = "0203"; //居民医保
                        if (DDYLJGMC == "区医保")
                        {
                            if (op.Qxmc.Contains("信州区") == false)
                            {
                                return new object[] { 0, 0, "该医保卡为市本级，请在市本级医保程序中使用" };
                            }
                        }
                        if (DDYLJGMC == "市医保")
                        {
                            if (op.Qxmc.Contains("信州区"))
                            {
                                return new object[] { 0, 0, "该医保卡为区卡，请在信州区医保程序中使用" };
                            }
                        }


                        #region 获取个人信息
                        WriteLog(sysdate + "   进入获取个人信息217...");
                        WriteLog(sysdate + "  入参|" + id);
                        string zglb = "";//享受类别
                        double ljmzf = 0.00; //年度内累计门诊费
                        double ljzyf = 0.00; //年度内累计住院费
                        double ljgrzhzf = 0.00; //累计个人帐户支付
                        double ljtczf = 0.00;//累计统筹支付
                        Int32 zycs = 0; //住院次数
                        int mzbbz = 0; //是登记了慢性病
                        string rybz = ""; //人员标志，如灵活就业、低保、建档立卡等
                        string qxmc = ""; //县区名称，参保地


                        //falg = ybgrxx2(id, xm, gmsfhm, csrq, xb, zglb, dwmc, ref ljmzf, ref ljzyf, ref ljgrzhzf, ref ljtczf, ref zycs, ref mzbbz);
                        op.Ljmzzf = ljmzf.ToString();
                        op.Ljzyzf = ljzyf.ToString();
                        op.Bnzhzclj = ljgrzhzf.ToString();
                        op.Bntczclj = ljtczf.ToString();
                        op.Bnzycs = zycs.ToString();
                        #endregion

                        #region 查询个人门诊刷卡是否开通
                        string mzkt = string.Empty;       //普通门诊是否开通  (1开通 0未开通)
                        string mzyys = string.Empty;      //门诊没开通的原因
                        string mxbkt = string.Empty;      //慢性病门诊是否开通 (1开通 0未开通)
                        string mxbyys = string.Empty;     //慢性病没有开通的原因
                        string mxbmcs = string.Empty;     //慢性病名称
                        string mztckt = string.Empty;     //门诊统筹是否开通 (1开通 0未开通)
                        string mztcyys = string.Empty;    //门诊统筹没有开通的原因
                        string bz = string.Empty;
                        object[] objParam1 = { id };
                        objParam = YBMZKZT(objParam1);
                        if (objParam[1].ToString().Equals("1"))
                        {
                            /*
                             mzkt普通门诊是否开通、
                             * mxbkt慢性病门诊是否开通、
                             * mztckt门诊统筹是否开通、
                             * mzyy门诊没开通的原因、
                             * mxbyy慢性病没有开通的原因、
                             * mxbmc慢性病名称、
                             * mztcyy门诊统筹没有开通的原因
                             */
                            op.Ptmzbz = objParam[3].ToString();
                            op.Ptmzyy = objParam[4].ToString();
                            op.Mtbz = objParam[5].ToString();
                            op.Mxbmzyy = objParam[6].ToString();
                            op.Mtbzmc = objParam[7].ToString();
                            op.Mztcbz = objParam[8].ToString();
                            op.Mztcyy = objParam[9].ToString();

                            if (op.Mtbz == "1")
                            {
                                bz = objParam[7].ToString();
                                if (bz.Contains(',') && bz.Contains('|'))
                                {
                                    string[] mxb = bz.Split(',');
                                    bz = mxb[0].ToString();
                                    string[] mxbz = bz.Split('|');
                                    op.Mtbzbm = mxbz[0].ToString();
                                    op.Mtbzmc = mxbz[1].ToString();
                                }
                                else
                                {
                                    if (bz.Contains('|'))
                                    {
                                        string[] mxbz = bz.Split('|');
                                        op.Mtbzbm = mxbz[0].ToString();
                                        op.Mtbzmc = mxbz[1].ToString();
                                    }
                                }

                                //if (bz.Contains(','))
                                //{
                                //    //存在双病种
                                //    string[] mxb = bz.Split(',');
                                //    DialogResult dr = MessageBox.Show("当前病人存在双病种" + mxb[0].ToString() + "和" + mxb[1].ToString() + "是否选择第一病种?", "提示",MessageBoxButtons.YesNoCancel);
                                //    if (dr == DialogResult.Yes)
                                //    {//获取第一病种
                                //        bz = mxb[0].ToString();
                                //        string[] mxbz = bz.Split('|');
                                //        op.Mtbzbm = mxbz[0].ToString();
                                //        op.Mtbzmc = mxbz[1].ToString();

                                //    }
                                //    else if (dr == DialogResult.No)
                                //    {//获取第二病种
                                //        bz = mxb[1].ToString();
                                //        string[] mxbz = bz.Split('|');
                                //        op.Mtbzbm = mxbz[0].ToString();
                                //        op.Mtbzmc = mxbz[1].ToString();
                                //    }
                                //}
                                //else
                                //{
                                //    bz = bz[0].ToString();
                                //    string[] mxbz = bz.Split('|');
                                //    op.Mtbzbm = mxbz[0].ToString();
                                //    op.Mtbzmc = mxbz[1].ToString();
                                //}
                            }
                        }
                        else
                            return new object[] { 0, 0, objParam[2].ToString() };
                        #endregion

                    }
                    else
                    {

                        #region 出参
                        /* 
                        * 个人标识号|姓名|性别|出生日期|公民身份号码|享受类别|单位名称|参保地名称
                        */
                        string ryxxs = Encoding.Default.GetString(ryxx, 0, ryxx.Length).Replace("\0", ""); //姓名
                        WriteLog(sysdate + "  医保读出成功|出参|" + ryxxs);

                        string[] ry = ryxxs.Split(';');
                        if (ry.Length > 1)
                        {
                            frm_dk dk = new frm_dk(ryxxs);
                            dk.ShowDialog();
                            ryxxs = dk.sValue;
                        }
                        string[] s = ryxxs.Split('|');
                        op.Grbh = s[0]; //个人编号
                        op.Xm = s[1]; //姓名
                        op.Xb = s[2]; //性别
                        op.Csrq = s[3]; //出生日期
                        op.Sfhz = s[4]; //公民身份号码
                        op.Yldylb = s[5]; //享受类别
                        op.Dwmc = s[6]; //单位名称
                        op.Qxmc = s[7]; //参保地名称
                        op.Zhye = grzhye.ToString();    //账户余额
                        op.Ispwd = sfymm.ToString();    //卡是否有密码
                        if (op.Yldylb.Contains("在岗") || op.Yldylb.Contains("在职") || op.Yldylb.Contains("退休") || op.Yldylb.Contains("离休"))
                            op.Jflx = "0202"; //职工医保
                        else
                            op.Jflx = "0203"; //居民医保

                        id = Convert.ToInt32(op.Grbh);


                        //MessageBox.Show(op.Qxmc);

                        if (DDYLJGMC == "区医保")
                        {
                            if (op.Qxmc.Contains("信州区") == false)
                            {
                                return new object[] { 0, 0, "该医保卡为市本级，请在市本级医保程序中使用" };
                            }
                        }
                        if (DDYLJGMC == "市医保")
                        {
                            if (op.Qxmc.Contains("信州区"))
                            {
                                return new object[] { 0, 0, "该医保卡为区卡，请在信州区医保程序中使用" };
                            }
                        }

                        #region 获取个人信息
                        WriteLog(sysdate + "   进入获取个人信息217...");
                        WriteLog(sysdate + "  入参|" + id);
                        string zglb = "";//享受类别
                        double ljmzf = 0.00; //年度内累计门诊费
                        double ljzyf = 0.00; //年度内累计住院费
                        double ljgrzhzf = 0.00; //累计个人帐户支付
                        double ljtczf = 0.00;//累计统筹支付
                        Int32 zycs = 0; //住院次数
                        int mzbbz = 0; //是登记了慢性病
                        string rybz = ""; //人员标志，如灵活就业、低保、建档立卡等
                        string qxmc = ""; //县区名称，参保地


                        //falg = ybgrxx2(id, xm, gmsfhm, csrq, xb, zglb, dwmc, ref ljmzf, ref ljzyf, ref ljgrzhzf, ref ljtczf, ref zycs, ref mzbbz);
                        op.Ljmzzf = ljmzf.ToString();
                        op.Ljzyzf = ljzyf.ToString();
                        op.Bnzhzclj = ljgrzhzf.ToString();
                        op.Bntczclj = ljtczf.ToString();
                        op.Bnzycs = zycs.ToString();
                        #endregion

                        #region 查询个人门诊刷卡是否开通
                        string mzkt = string.Empty;       //普通门诊是否开通  (1开通 0未开通)
                        string mzyys = string.Empty;      //门诊没开通的原因
                        string mxbkt = string.Empty;      //慢性病门诊是否开通 (1开通 0未开通)
                        string mxbyys = string.Empty;     //慢性病没有开通的原因
                        string mxbmcs = string.Empty;     //慢性病名称
                        string mztckt = string.Empty;     //门诊统筹是否开通 (1开通 0未开通)
                        string mztcyys = string.Empty;    //门诊统筹没有开通的原因
                        object[] objParam1 = { id };
                        objParam = YBMZKZT(objParam1);
                        if (objParam[1].ToString().Equals("1"))
                        {
                            /*
                             mzkt普通门诊是否开通、
                             * mxbkt慢性病门诊是否开通、
                             * mztckt门诊统筹是否开通、
                             * mzyy门诊没开通的原因、
                             * mxbyy慢性病没有开通的原因、
                             * mxbmc慢性病名称、
                             * mztcyy门诊统筹没有开通的原因
                             */
                            op.Ptmzbz = objParam[3].ToString();
                            op.Ptmzyy = objParam[4].ToString();
                            op.Mtbz = objParam[5].ToString();
                            op.Mxbmzyy = objParam[6].ToString();
                            op.Mtbzbm = "";
                            op.Mtbzmc = objParam[7].ToString();
                            op.Mztcbz = objParam[8].ToString();
                            op.Mztcyy = objParam[9].ToString();
                        }
                        else
                            return new object[] { 0, 0, objParam[2].ToString() };
                        #endregion



                        #endregion
                    }

                    #region 返回数据处理
                    op.Dwbh = "";
                    op.Kh = "";
                    if (op.Xb.Equals("男"))
                        op.Xb = "1";
                    else
                        op.Xb = "2";
                    op.Nl = (DateTime.Now.Year - Convert.ToDateTime(op.Csrq).Year).ToString();
                    op.Ybklx = "0";
                    #endregion

                    /*
                     * 个人编号|单位编号|身份证号|姓名|性别|
                     * 民族|出生日期|社会保障卡卡号|医疗待遇类别|人员参保状态|
                     * 异地人员标志|统筹区号|年度|在院状态|帐户余额|
                     * 本年医疗费累计|本年帐户支出累计|本年统筹支出累计|本年救助金支出累计|本年公务员补助基金累计|
                     * 本年城镇居民门诊统筹支付累计|进入统筹费用累计|进入救助金费用累计|起付标准累计|本年住院次数|
                     * 单位名称|年龄|参保单位类型|经办机构编码|缴费类型|
                     * 医保门慢、特资质|医保门慢、特病种说明|医疗类别代码|慢、特病编码|慢、特病名称|
                     * 医保卡类型
                     */
                    string outParams = op.Grbh + "|" + op.Dwbh + "|" + op.Sfhz + "|" + op.Xm + "|" + op.Xb + "|" +
                       op.Mz + "|" + op.Csrq + "|" + op.Kh + "|" + op.Yldylb + "|" + op.Rycbzt + "|" +
                       op.Ydrybz + "|" + op.Tcqh + "|" + op.Nd + "|" + op.Zyzt + "|" + op.Zhye + "|" +
                       op.Bnylflj + "|" + op.Bnzhzclj + "|" + op.Bntczclj + "|" + op.Bndbyljjzflj + "|" + op.Bngwybzjjlj + "|" +
                       op.Bnczjmmztczflj + "|" + op.Jrtcfylj + "|" + op.Jrdbfylj + "|" + op.Qfbzlj + "|" + op.Bnzycs + "|" +
                       op.Dwmc + "|" + op.Nl + "|" + op.Cbdwlx + "|" + op.Jbjgbm + "|" + op.Jflx + "|" + op.Mtbz + "|" + op.Mtmsg + "|" + op.Yllb + "|" + op.Mtbzbm + "|" + op.Mtbzmc + "|" + op.Ybklx + "|";

                    //插入表YBICKXX
                    List<string> liSQL = new List<string>();


                    string strSql = string.Format("delete from YBICKXX where grbh='{0}'", op.Grbh);
                    liSQL.Add(strSql);
                    string strSQL = string.Format(@"select id from ybickxx where grbh='{0}'", id);
                    liSQL.Add(strSQL);
                    strSQL = string.Format(@"insert ybickxx(grbh,xm,xb,csrq,gmsfhm,zglb,dwmc,qxmc,grzhye,sfymm,
                                            ljmzf,ljzyf,ljgrzhzf,ljtczf,zycs,mzbbz,MZKT,MZYY,MXBKT,
                                            MXBYY,MXBMC,MZTCKT,MZTCYY,SYSDATE) values(
                                            '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}',
                                            '{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}',
                                              '{19}','{20}','{21}','{22}','{23}' )",
                                            op.Grbh, op.Xm, op.Xb, op.Csrq, op.Sfhz, op.Yldylb, op.Dwmc, op.Qxmc, op.Zhye, op.Ispwd,
                                            op.Ljmzzf, op.Ljzyzf, op.Bnzhzclj, op.Bntczclj, op.Bnzycs, op.Mtbz, op.Ptmzbz, op.Ptmzbz, op.Mtbz,
                                            op.Mxbmzyy, op.Mtbzmc, op.Mztcbz, op.Mztcyy, sysdate);
                    liSQL.Add(strSQL);
                    object[] obj = liSQL.ToArray();
                    obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);
                    if (obj[1].ToString() == "1")
                    {
                        WriteLog(sysdate + "   进入医保读卡成功|" + outParams);
                        return new object[] { 0, 1, outParams };
                    }
                    else
                    {
                        WriteLog(sysdate + "   进入医保读卡成功|本地数据操作失败|" + obj[2].ToString());
                        return new object[] { 0, 0, "读卡成功|本地数据操作失败|" + obj[2].ToString() };
                    }
                }
                else
                {
                    string errMsg = errorMessage();
                    WriteLog(sysdate + "   进入医保读卡失败|" + errMsg);
                    return new object[] { 0, 0, errMsg };
                }
            }
            catch (Exception ex)
            {
                WriteLog(sysdate + "   进入门诊医保读卡异常|" + ex.Message);
                return new object[] { 0, 0, ex.Message };
            }
        }
        #endregion

        #region 查询个人门诊刷卡是否开通
        internal static object[] YBMZKZT(object[] objParam)
        {
            string sysdate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");//当前时间
            string grbh = objParam[0].ToString(); //个人标识号
            //参数
            Int32 id = int.Parse(grbh);
            Int32 mzkt = -1;                //普通门诊是否开通
            Int32 mxbkt = -1;               //慢性病门诊是否开通
            Int32 mztckt = -1;              //门诊统筹是否开通
            byte[] mzyy = new byte[1024];        //门诊没开通的原因
            byte[] mxbyy = new byte[1024];        //慢性病没有开通的原因
            byte[] mxbmc = new byte[1024];        //慢性病名称
            byte[] mztcyy = new byte[1024];        //门诊统筹没有开通的原因


            WriteLog(sysdate + "   个人编号:" + grbh + "进入查询个人门诊刷卡是否开通...");
            WriteLog(sysdate + "  入参|" + grbh);

            int i = mzktzt2(id, ref mzkt, ref mxbkt, ref mztckt, mzyy, mxbyy, mxbmc, mztcyy);
            if (i == 0)
            {

                string mzyys = Encoding.Default.GetString(mzyy, 0, mzyy.Length).Replace("\0", "");
                string mxbyys = Encoding.Default.GetString(mxbyy, 0, mxbyy.Length).Replace("\0", "");
                string mxbmcs = Encoding.Default.GetString(mxbmc, 0, mxbmc.Length).Replace("\0", "");
                string mztcyys = Encoding.Default.GetString(mztcyy, 0, mztcyy.Length).Replace("\0", "");

                WriteLog(sysdate + "   个人编号:" + grbh + "进入查询个人门诊刷卡是否开通成功|" + mzkt + "|" + mzyys + "|" + mxbkt + "|" + mxbyys + "|" + mxbmcs + "|" + mztckt + "|" + mztcyys);
                return new object[] { 0, 1, "查询个人门诊刷卡是否开通成功", mzkt, mzyys, mxbkt, mxbyys, mxbmcs, mztckt, mztcyys };
            }
            else
            {
                string errMsg = errorMessage();
                WriteLog(sysdate + "   个人编号:" + grbh + "进入查询个人门诊刷卡是否开通失败|" + errMsg);
                return new object[] { 0, 0, "查询个人门诊刷卡是否开通失败" + errMsg };
            }

        }
        #endregion

        #region 门诊登记
        public static object[] YBMZDJ(object[] objParam)
        {
            string sysdate = GetServerDateTime();
            WriteLog(sysdate + "  进入门诊登记...");

            try
            {
                #region his参数
                string jbr = CliUtils.fLoginUser;   // 经办人姓名 
                string jzlsh = objParam[0].ToString();  // 就诊流水号
                string yllb = objParam[1].ToString();   // 医疗类别代码
                string bzbm = objParam[2].ToString();   // 病种编码（慢性病要传，否则传空字符串）
                string bzmc = objParam[3].ToString();   // 病种名称（慢性病要传，否则传空字符串）
                string ickxx = objParam[4].ToString();  //医保读卡返回信息
                string ghdjsj = objParam[5].ToString();   // 登记时间(格式：DateTime.Now.ToString("yyyyMMddHHmmss"))
                string cfysdm = objParam[6].ToString(); //处方医生代码
                string cfysxm = objParam[7].ToString(); //处方医生姓名
                string isYD = objParam[8].ToString(); //是否异地登记
                string ysdm = "";   // 医师代码
                string ysxm = "";   // 医师姓名
                string ksbh = "";   // 科室编号
                string ksmc = "";   // 科室名称
                string hzxm = "";   //患者姓名
                #endregion

                JYLSH = Convert.ToDateTime(ghdjsj).ToString("yyyyMMddHHmmss") + "-" + YLGHBH + "-" + new Random().Next(100).ToString().PadLeft(4, '0');

                if (string.IsNullOrEmpty(ghdjsj))
                    return new object[] { 0, 0, "医保提示：挂号时间为空" };
                if (string.IsNullOrEmpty(jzlsh))
                    return new object[] { 0, 0, "医保提示：就诊流水号不能为空" };
                if (string.IsNullOrEmpty(yllb))
                    return new object[] { 0, 0, "医保提示：医疗类别不能为空" };
                if (string.IsNullOrEmpty(ickxx))
                    return new object[] { 0, 0, "医保提示：读卡信息不能为空" };

                //读卡信息
                string[] kxx = ickxx.ToString().Split('|');
                string grbh = kxx[0].ToString(); //个人编号
                string dwbm = kxx[1].ToString();  //单位编号
                string dwmc = kxx[25].ToString();//单位名称
                string sfzh = kxx[2].ToString();  //身份证号
                string xm = kxx[3].ToString();  //姓名
                string xb = kxx[4].ToString();  //性别
                string kh = kxx[7].ToString();  //卡号
                string yldylb = kxx[8].ToString();  //医疗待遇类别
                string ydrybz = kxx[10].ToString();  //异地人员标志
                string tcqh = kxx[11].ToString();  //所属区号
                string zhye = kxx[14].ToString();  //帐户余额
                string ybklx = kxx[35].ToString(); //医保卡类型
                string isPwd = kxx[36].ToString();


                string strSql = "";
                DataSet ds = null;

                #region 获取病种信息
                if (isYD == "1")
                {
                    strSql = string.Format("select * from ybickxx where grbh = '{0}'", grbh);

                    ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                    if (ds == null || ds.Tables[0].Rows.Count == 0)
                        return new object[] { 0, 0, "医保信息查无此人" };

                    DataRow dr1 = ds.Tables[0].Rows[0];
                    bzbm = dr1["MXBBM"].ToString(); //病种名称
                    bzmc = dr1["MXBMC"].ToString(); //病种名称
                }
                else
                {
                    strSql = string.Format("select * from ybickxx where grbh = '{0}'", grbh);

                    ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                    if (ds == null || ds.Tables[0].Rows.Count == 0)
                        return new object[] { 0, 0, "医保信息查无此人" };

                    DataRow dr1 = ds.Tables[0].Rows[0];
                    string mc = dr1["MXBMC"].ToString(); //病种名称


                    strSql = string.Format("select * from ybbzmrdr where dmmc = '{0}'", mc);

                    ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                    if (ds == null || ds.Tables[0].Rows.Count == 0)
                    {
                        if (bzmc == "" || bzmc == null)
                            return new object[] { 0, 0, bzmc + "病种目录无记录,请手动填写" };
                    }
                    else
                    {

                        dr1 = ds.Tables[0].Rows[0];
                        bzbm = dr1["dm"].ToString(); //病种编码
                        bzmc = dr1["dmmc"].ToString();
                    }
                }
                

                #endregion

                if (bzbm == "" || bzmc == null)
                {
                    bzmc = "腹痛";
                    bzbm = "R10.400";
                }

                string ybjzlsh = "";
                if (string.IsNullOrEmpty(bzbm))
                {
                    ybjzlsh = "MZ" + jzlsh;
                }
                else
                    ybjzlsh = "MZ" + jzlsh + bzbm;

                #region  是否门诊挂号
                strSql = string.Format(@"select m1ghno,m1name,m1ksno,b2ksnm,m1empn,b1name,m1amnt from mz01t  a
                                                left join bz01h b on a.m1empn=b.b1empn
                                                left join bz02h c on a.m1ksno=c.b2ksno
                                                where m1ghno='{0}'", jzlsh);

                ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                if (ds.Tables[0].Rows.Count == 0)
                {
                    WriteLog(sysdate + "  无挂号信息");
                    return new object[] { 0, 0, "无挂号信息" };
                }

                ysdm = ds.Tables[0].Rows[0]["m1empn"].ToString();
                ysxm = ds.Tables[0].Rows[0]["b1name"].ToString();
                ksbh = ds.Tables[0].Rows[0]["m1ksno"].ToString();
                ksmc = ds.Tables[0].Rows[0]["b2ksnm"].ToString();
                hzxm = ds.Tables[0].Rows[0]["m1name"].ToString();
                #endregion


                #region 查询定岗医生代码
                strSql = string.Format(@"select * from ybdgyszd where ysbm = '{0}'", ysdm);
                ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                if (ds.Tables[0].Rows.Count == 0)
                {
                    WriteLog(sysdate + cfysxm + "该医生无定岗医生配对信息，请进行配对");
                    return new object[] { 0, 0, cfysxm + "该医生无定岗医生配对信息，请进行配对" };
                }

                string dgysbm = ds.Tables[0].Rows[0]["dgysbm"].ToString();
                #endregion

                #region 医保门诊登记
                if (!hzxm.Equals(xm))
                {
                    return new object[] { 0, 0, "患者信息与医保卡不一致" };
                }

                List<string> liSQL = new List<string>();
                strSql = string.Format("select * from ybmzzydjdr a where a.jzlsh = '{0}' and a.cxbz = 1", jzlsh);
                ds.Tables.Clear();
                ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    return new object[] { 0, 0, "医保提示：就诊流水号" + jzlsh + "已登记医保挂号" };
                }

                WriteLog(sysdate + "   " + jzlsh + "进入门诊医保登记....|" + jzlsh + "|" + ybjzlsh);
                strSql = string.Format(@"insert into ybmzzydjdr
                                            (jzlsh,jylsh,yllb,ghdjsj,bzbm,bzmc,ksbh,ksmc,ysdm,ysxm,
                                            jbr,grbh,xm,kh,ghf,ybzl,ybjzlsh,jzbz,tcqh,yldylb,
                                            dgysdm,dgysxm,sfzh,xb,zhye,dwmc,dq) values(
                                            '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}',
                                            '{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}',
                                            '{20}','{21}','{22}','{23}','{24}','{25}','{26}')",
                                            jzlsh, JYLSH, yllb, ghdjsj, bzbm, bzmc, ksbh, ksmc, ysdm, ysxm,
                                            jbr, grbh, xm, kh, "", "", ybjzlsh, "m", tcqh, yldylb,
                                            dgysbm, ysxm, sfzh, xb, zhye, dwmc, DDYLJGMC);
                WriteLog(strSql);
                liSQL.Add(strSql);
                #endregion

                object[] obj_gh = liSQL.ToArray();
                obj_gh = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj_gh);
                if (obj_gh[1].ToString().Equals("1"))
                {
                    WriteLog(sysdate + "  门诊登记(挂号)成功");
                    return new object[] { 0, 1, "门诊登记(挂号)成功" };
                }
                else
                {
                    WriteLog(sysdate + "  门诊登记(挂号)成功");
                    return new object[] { 0, 0, "门诊登记(挂号)成功" + obj_gh[2].ToString() };

                }
            }
            catch (Exception ex)
            {
                WriteLog(sysdate + "   门诊登记接口异常|" + ex.Message);
                return new object[] { 0, 0, ex.Message };
            }
        }
        #endregion

        #region 门诊登记撤销
        public static object[] YBMZDJCX(object[] objParam)
        {
            string sysdate = GetServerDateTime();
            WriteLog(sysdate + "  进入门诊登记撤销...");

            try
            {
                string jbr = CliUtils.fUserName; //操作员姓名
                string jzlsh = objParam[0].ToString();  //就诊流水号
                string yllb = objParam[1].ToString();   //医疗类别

                JYLSH = DateTime.Now.ToString("yyyyMMddHHmmss") + "-" + YLGHBH + "-" + new Random().Next(100).ToString().PadLeft(4, '0');
                #region 是否挂号登记
                string strSql = string.Format(@"select a.* from ybmzzydjdr a where a.jzlsh='{0}' and a.cxbz=1 
                                                and not exists(select 1 from ybfyjsdr b where a.jzlsh=b.jzlsh and b.cxbz=1) ", jzlsh, yllb);
                DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                if (ds.Tables[0].Rows.Count == 0)
                {
                    WriteLog(sysdate + "  该患者无医保门诊挂号登记信息!");
                    return new object[] { 0, 0, "该患者无医保门诊挂号登记信息" };
                }
                #endregion

                List<string> liSQL = new List<string>();
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    string ybjzlsh = dr["ybjzlsh"].ToString();
                    strSql = string.Format(@"insert into ybmzzydjdr
                                        (jzlsh,jylsh,yllb,ghdjsj,bzbm,bzmc,ksbh,ksmc,ysdm,ysxm,
                                        jbr,grbh,xm,kh,ghf,ybzl,ybjzlsh,jzbz,tcqh,yldylb,
                                        dgysdm,dgysxm,cxbz,sysdate,sfzh,xb,zhye,dwmc)
                                        select jzlsh,jylsh,yllb,ghdjsj,bzbm,bzmc,ksbh,ksmc,ysdm,ysxm,
                                        jbr,grbh,xm,kh,ghf,ybzl,ybjzlsh,jzbz,tcqh,yldylb,
                                        dgysdm,dgysxm,cxbz,'{1}',sfzh,xb,zhye,dwmc from ybmzzydjdr where ybjzlsh='{0}' and cxbz=1", ybjzlsh, sysdate);
                    liSQL.Add(strSql);
                    strSql = string.Format(@"update ybmzzydjdr set cxbz=2 where ybjzlsh = '{0}'  and cxbz = 1", ybjzlsh);
                    liSQL.Add(strSql);
                }

                object[] obj = liSQL.ToArray();
                obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);
                if (obj[1].ToString().Equals("1"))
                {
                    WriteLog(sysdate + "  门诊登记(挂号)登记撤销成功");
                    return new object[] { 0, 1, "门诊登记(挂号)登记撤销成功" };
                }
                else
                {
                    WriteLog(sysdate + "  门诊登记(挂号)登记撤销失败" + obj[2].ToString());
                    return new object[] { 0, 1, "门诊登记(挂号)登记撤销失败" + obj[2].ToString() };
                }
            }
            catch (Exception ex)
            {
                WriteLog(sysdate + "   门诊登记接口撤销异常|" + ex.Message);
                return new object[] { 0, 0, ex.Message };
            }
        }
        #endregion

        #region 门诊登记(挂号)收费
        /// <summary>
        /// 门诊登记收费
        /// </summary>
        /// <param>就诊流水号,医疗类别代码,登记时间(格式：yyyy-MM-dd HH:mm:ss),病种编码（慢性病要传，否则传空字符串）,病种名称（慢性病要传，否则传空字符串）,卡信息(读卡返回的一串个人信息)</param>
        /// <returns>1:成功(医疗费总额|总报销金额|统筹基金支付|大额基金支付|账户支付|现金支付|公务员补助基金支付|企业补充医疗保险基金支付|自费费用|单位负担费用|医院负担费用|民政救助费用|超限价费用|乙类自理费用|丙类自理费用|符合基本医疗费用|起付标准费用|转诊转院自付费用|进入统筹费用|统筹分段自付费用|超统筹封顶线费用|进入大额报销费用|大额分段自付费用|超大额封顶线费用|人工器官自付费用|本次结算前帐户余额|本年统筹支付累计(不含本次)|本年大额支付累计(不含本次)|本年城镇居民门诊统筹支付累计(不含本次)|本年公务员补助支付累计(不含本次)|本年账户支付累计(不含本次)|本年住院次数累计(不含本次)|住院次数|姓名|结算时间|医疗类别|医疗待遇类别|经办机构编码|业务周期号|结算流水号|提示信息|发票号|交易类型|医院交易流水号|有效标志|个人编号|医疗机构编码|二次补偿金额|门慢起付累计|接收方交易流水号|个人编号|单病种补差|财政支出金额|二类门慢限额支出|二类门慢限额剩余|居民个人自付二次补偿金额|体检金额|生育基金支付|),0:失败，2:报错</returns>
        public static object[] YBMZDJSF(object[] objParam)
        {
            string sysdate = GetServerDateTime();//当前时间
            try
            {
                #region 入参
                string jbr = CliUtils.fLoginUser;   // 经办人姓名 
                string jzlsh = objParam[0].ToString();  // 就诊流水号
                string yllb = objParam[1].ToString();   // 医疗类别代码
                string ghdjsj = objParam[2].ToString();   // 登记时间(格式：DateTime.Now.ToString("yyyyMMddHHmmss"))
                string bzbm = objParam[3].ToString();   // 病种编码（慢性病要传，否则传空字符串）
                string bzmc = objParam[4].ToString();   // 病种名称（慢性病要传，否则传空字符串）
                string ickxx = objParam[5].ToString();  //医保读卡返回信息
                string cfysdm = objParam[7].ToString(); //处方医生代码
                string cfysxm = objParam[8].ToString(); //处方医生姓名
                string isYD = objParam[9].ToString(); //处方医生姓名
                string ysdm = "";   // 医师代码
                string ysxm = "";   // 医师姓名
                string ksbh = "";   // 科室编号
                string ksmc = "";   // 科室名称
                string hzxm = "";   //患者姓名

                //读卡信息
                string[] kxx = ickxx.ToString().Split('|');
                string grbh = kxx[0].ToString(); //个人编号
                string dwbm = kxx[1].ToString();  //单位编号
                string sfzh = kxx[2].ToString();  //身份证号
                string dwmc = kxx[25].ToString();//单位名称
                string xm = kxx[3].ToString();  //姓名
                string xb = kxx[4].ToString();  //性别
                string kh = kxx[7].ToString();  //卡号
                string yldylb = kxx[8].ToString();  //医疗待遇类别
                string ydrybz = kxx[10].ToString();  //异地人员标志
                string tcqh = kxx[11].ToString();  //所属区号
                string zhye = kxx[14].ToString();  //帐户余额
                string ybklx = kxx[35].ToString(); //医保卡类型
                string isPwd = kxx[36].ToString();


                #endregion
                JYLSH = Convert.ToDateTime(ghdjsj).ToString("yyyyMMddHHmmss") + "-" + YLGHBH + "-" + new Random().Next(100).ToString().PadLeft(4, '0');
                List<string> liSQL = new List<string>();
                if (string.IsNullOrWhiteSpace(jzlsh))
                    return new object[] { 0, 0, "医保提示：就诊流水号为空" };
                else if (string.IsNullOrWhiteSpace(yllb))
                    return new object[] { 0, 0, "医保提示：医疗类别代码为空" };
                else if (string.IsNullOrWhiteSpace(ghdjsj))
                    return new object[] { 0, 0, "医保提示：挂号时间为空" };

                string ybjzlsh = "";



                string strSql = "";
                DataSet ds = null;

                #region 获取病种信息
                if (isYD == "1")
                {
                    strSql = string.Format("select * from ybickxx where grbh = '{0}'", grbh);

                    ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                    if (ds == null || ds.Tables[0].Rows.Count == 0)
                        return new object[] { 0, 0, "医保信息查无此人" };

                    DataRow dr1 = ds.Tables[0].Rows[0];
                    bzbm = dr1["MXBBM"].ToString(); //病种名称
                    bzmc = dr1["MXBMC"].ToString(); //病种名称
                }
                else
                {
                    strSql = string.Format("select * from ybickxx where grbh = '{0}'", grbh);

                    ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                    if (ds == null || ds.Tables[0].Rows.Count == 0)
                        return new object[] { 0, 0, "医保信息查无此人" };

                    DataRow dr1 = ds.Tables[0].Rows[0];
                    string mc = dr1["MXBMC"].ToString(); //病种名称


                    strSql = string.Format("select * from ybbzmrdr where dmmc = '{0}'", mc);

                    ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                    if (ds == null || ds.Tables[0].Rows.Count == 0)
                    {
                        if (bzmc == "" || bzmc == null)
                            return new object[] { 0, 0, bzmc + "病种目录无记录,请手动填写" };
                    }
                    else
                    {

                        dr1 = ds.Tables[0].Rows[0];
                        bzbm = dr1["dm"].ToString(); //病种编码
                        bzmc = dr1["dmmc"].ToString();
                    }
                }


                #endregion


                if (string.IsNullOrEmpty(bzbm))
                {
                    ybjzlsh = "MZ" + jzlsh;
                }
                else
                    ybjzlsh = "MZ" + jzlsh + bzbm;

                #region  是否门诊挂号
                strSql = string.Format(@"select m1ghno,m1name,m1ksno,b2ksnm,m1empn,b1name,m1amnt from mz01t  a
                                                left join bz01h b on a.m1empn=b.b1empn
                                                left join bz02h c on a.m1ksno=c.b2ksno
                                                where m1ghno='{0}'", jzlsh);

                ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                if (ds.Tables[0].Rows.Count == 0)
                {
                    WriteLog(sysdate + "  无挂号费用明细");
                    return new object[] { 0, 0, "无挂号费用明细" };
                }

                ysdm = ds.Tables[0].Rows[0]["m1empn"].ToString();
                ysxm = ds.Tables[0].Rows[0]["b1name"].ToString();
                ksbh = ds.Tables[0].Rows[0]["m1ksno"].ToString();
                ksmc = ds.Tables[0].Rows[0]["b2ksnm"].ToString();
                hzxm = ds.Tables[0].Rows[0]["m1name"].ToString();
                #endregion


                #region 查询定岗医生代码
                strSql = string.Format(@"select * from ybdgyszd where ysbm = '{0}'", ysdm);
                ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                if (ds.Tables[0].Rows.Count == 0)
                {
                    WriteLog(sysdate + ysxm + "该医生无定岗医生配对信息，请进行配对");
                    return new object[] { 0, 0, ysxm + "该医生无定岗医生配对信息，请进行配对" };
                }

                string dgysbm = ds.Tables[0].Rows[0]["dgysbm"].ToString();
                #endregion


                #region 医保门诊登记
                if (!hzxm.Equals(xm))
                {
                    return new object[] { 0, 0, "患者信息与医保卡不一致" };
                }

                strSql = string.Format("select * from ybmzzydjdr a where a.jzlsh = '{0}' and a.cxbz = 1", jzlsh);
                ds.Tables.Clear();
                ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    return new object[] { 0, 0, "医保提示：就诊流水号" + jzlsh + "已登记医保挂号" };
                }

                WriteLog(sysdate + "   " + jzlsh + "进入门诊医保登记....|" + jzlsh + "|" + ybjzlsh);
                strSql = string.Format(@"insert into ybmzzydjdr
                                            (jzlsh,jylsh,yllb,ghdjsj,bzbm,bzmc,ksbh,ksmc,ysdm,ysxm,
                                            jbr,grbh,xm,kh,ghf,ybzl,ybjzlsh,jzbz,tcqh,yldylb,
                                            dgysdm,dgysxm,sfzh,xb,zhye,dwmc,DQ) values(
                                            '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}',
                                            '{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}',
                                            '{20}','{21}','{22}','{23}','{24}','{25}','{26}')",
                                            jzlsh, JYLSH, yllb, ghdjsj, bzbm, bzmc, ksbh, ksmc, ysdm, ysxm,
                                            jbr, grbh, xm, kh, "", "", ybjzlsh, "m", tcqh, yldylb,
                                            dgysbm, ysxm, sfzh, xb, zhye, dwmc, DDYLJGMC);
                liSQL.Add(strSql);
                #endregion

                #region 获取挂号明细信息并上传
                string ybxmbm = string.Empty;
                string ybxmmc = string.Empty;
                int isequ = 1;
                double dj = 0.00;
                double sl = 0.00;
                double je = 0.00;
                string yyxmmc = string.Empty;
                string yyxmbm = string.Empty;
                double blbfy = 0.00; //病历本费用
                double ylkfy = 0.00; //就诊卡费用
                double ghfy = 0.00;   //挂号总费用
                double ghfylj = 0.00; //挂号费用累计
                string djhin = string.Empty; //单据号(发票号)
                string cfrq = DateTime.Now.ToString("yyyyMMdd");
                string strValue = "";
                int i = 0;

                #region 获取诊查费用信息
                strSql = string.Format(@"select  z.ybxmbh ybxmbh, z.ybxmmc ybxmmc, c.bzmem1 as dj,1 as sl,a.m1gham je,c.bzmem2 yyxmbh, 
                                        c.bzname yyxmmc, a.m1invo,a.m1blam,a.m1kham,a.m1amnt,z.jx,z.sfxmdjdm
                                        from mz01t a           
                                        join bztbd c on a.m1zlfb = c.bzkeyx and c.bzcodn = 'A7' 
                                        left join ybhisdzdr z on c.bzmem2 = z.hisxmbh                
                                        where a.m1ghno = '{0}'", jzlsh);
                ds.Tables.Clear();
                ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                if (ds.Tables[0].Rows.Count == 0)
                    return new object[] { 0, 0, "无费用明细" };

                DataRow dr = ds.Tables[0].Rows[0];
                djhin = dr["m1invo"].ToString();
                je = Convert.ToDouble(dr["je"].ToString());
                blbfy = Convert.ToDouble(dr["m1blam"].ToString());
                ylkfy = Convert.ToDouble(dr["m1kham"].ToString());
                ghfy = Convert.ToDouble(dr["m1amnt"].ToString());
                if (je > 0)
                {
                    /*
                     个人标识号， 医保项目编码（0或-1），医院项目编码(江西省20位医保编码)，
                     * 项目名称， 剂型，规格，项目类型，规格代码，数量，单价， 金额
                     */
                    Int32 id = Int32.Parse(grbh);             //个人标识号
                    Int32 falg = Int32.Parse("0");   //医保项目编码（0或－1）
                    ybxmbm = dr["ybxmbh"].ToString();             //医院项目编码（20位省医保编码）
                    ybxmmc = dr["ybxmmc"].ToString();
                    yyxmmc = dr["yyxmmc"].ToString();               //项目名称
                    yyxmmc = dr["yyxmbh"].ToString();               //项目名称
                    string jx = dr["jx"].ToString();                   //
                    string gg = "";
                    Int32 xmlx = Int32.Parse(dr["sfxmdjdm"].ToString());
                    Int32 ggdm = Int32.Parse("0");
                    sl = double.Parse(dr["sl"].ToString());
                    dj = double.Parse(dr["dj"].ToString());
                    je = double.Parse(dr["je"].ToString());

                    strValue = grbh + "|" + "0" + "|" + ybxmbm + "|" + yyxmmc + "|" + jx + "|" + gg + "|" + xmlx + "|" + ggdm + "|" + sl + "|" + dj + "|" + je + "|";
                    WriteLog(sysdate + "   " + jzlsh + "上传处方明细(挂号)->" + strValue);
                    i = mzcscf2(id, falg, ybxmbm, yyxmmc, jx, gg, xmlx, ggdm, sl, dj, je);

                    if (i != 0)
                    {
                        string errMsg = errorMessage();
                        WriteLog(sysdate + "   " + jzlsh + "进入门诊登记收费｜诊查费上传失败|" + errMsg);
                        object[] objDK = { };
                        YBMZDK(objDK);
                        return new object[] { 0, 0, errMsg };
                    }

                    strSql = string.Format(@"insert into ybcfmxscindr(jzlsh,jylsh,xm,kh,ybjzlsh,cfrq,yysfxmbm,yysfxmmc,sfxmzxbm,sfxmzxmc,
                                            dj,sl,je,jbr,sysdate) values(
                                            '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}',
                                            '{10}','{11}','{12}','{13}','{14}')",
                                            jzlsh, JYLSH, xm, kh, ybjzlsh, cfrq, yyxmbm, yyxmmc, ybxmbm, ybxmmc,
                                            dj, sl, je, jbr, sysdate);
                    liSQL.Add(strSql);
                }
                #endregion

                #region 获取病历费用信息
                if (blbfy > 0)
                {
                    strSql = string.Format(@"select c.ybxmbh,c.ybxmmc,b.b5sfam as dj,1 as sl,b.b5sfam as je,a.pamark as yyxmbh,b.b5name as yyxmmc,z.jx,z.sfxmdjdm
                                            from patbh a
                                            left join bz05d b on a.pamark=b.b5item
                                            left join ybhisdzdr c on a.pamark=c.hisxmbh and b.b5item=c.hisxmbh
                                            where pakind='MZ' and pasequ='0001'");
                    ds.Tables.Clear();
                    ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                    dr = ds.Tables[0].Rows[0];
                    /*
                    个人标识号， 医保项目编码（0或-1），医院项目编码(江西省20位医保编码)，
                    * 项目名称， 剂型，规格，项目类型，规格代码，数量，单价， 金额
                    */
                    Int32 id = Int32.Parse(grbh);             //个人标识号
                    Int32 falg = Int32.Parse("0");   //医保项目编码（0或－1）
                    ybxmbm = dr["ybxmbh"].ToString();             //医院项目编码（20位省医保编码）
                    ybxmmc = dr["ybxmmc"].ToString();
                    yyxmmc = dr["yyxmmc"].ToString();               //项目名称
                    yyxmmc = dr["yyxmbh"].ToString();               //项目名称
                    string jx = dr["jx"].ToString();                   //
                    string gg = "";
                    Int32 xmlx = Int32.Parse(dr["sfxmdjdm"].ToString());
                    Int32 ggdm = Int32.Parse("0");
                    sl = double.Parse(dr["sl"].ToString());
                    dj = double.Parse(dr["dj"].ToString());
                    je = double.Parse(dr["dj"].ToString());

                    strValue = grbh + "|" + "0" + "|" + ybxmbm + "|" + yyxmmc + "|" + jx + "|" + gg + "|" + xmlx + "|" + ggdm + "|" + sl + "|" + dj + "|" + je + "|";
                    WriteLog(sysdate + "   " + jzlsh + "上传处方明细(挂号)->" + strValue);
                    i = mzcscf2(id, falg, ybxmbm, yyxmmc, jx, gg, xmlx, ggdm, sl, dj, je);

                    if (i != 0)
                    {
                        string errMsg = errorMessage();
                        WriteLog(sysdate + "   " + jzlsh + "进入门诊登记收费｜诊查费上传失败|" + errMsg);
                        object[] objDK = { };
                        YBMZDK(objDK);
                        return new object[] { 0, 0, errMsg };
                    }

                    strSql = string.Format(@"insert into ybcfmxscindr(jzlsh,jylsh,xm,kh,ybjzlsh,cfrq,yysfxmbm,yysfxmmc,sfxmzxbm,sfxmzxmc,
                                            dj,sl,je,jbr,sysdate) values(
                                            '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}',
                                            '{10}','{11}','{12}','{13}','{14}')",
                                            jzlsh, JYLSH, xm, kh, ybjzlsh, cfrq, yyxmbm, yyxmmc, ybxmbm, ybxmmc,
                                            dj, sl, je, jbr, sysdate);
                    liSQL.Add(strSql);
                }
                #endregion

                #region 获取就诊卡费用信息
                if (ylkfy > 0)
                {
                    strSql = string.Format(@"select c.ybxmbh,c.ybxmmc,b.b5sfam as dj,1 as sl,b.b5sfam as je,a.pamark as yyxmbh,b.b5name as yyxmmc from patbh a
                                            left join bz05d b on a.pamark=b.b5item
                                            left join ybhisdzdr c on a.pamark=c.hisxmbh and b.b5item=c.hisxmbh
                                            where pakind='MZ' and pasequ='0004'");
                    ds.Tables.Clear();
                    ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                    dr = ds.Tables[0].Rows[0];

                    /*
                  个人标识号， 医保项目编码（0或-1），医院项目编码(江西省20位医保编码)，
                  * 项目名称， 剂型，规格，项目类型，规格代码，数量，单价， 金额
                  */
                    Int32 id = Int32.Parse(grbh);             //个人标识号
                    Int32 falg = Int32.Parse("0");   //医保项目编码（0或－1）
                    ybxmbm = dr["ybxmbh"].ToString();             //医院项目编码（20位省医保编码）
                    ybxmmc = dr["ybxmmc"].ToString();
                    yyxmmc = dr["yyxmmc"].ToString();               //项目名称
                    yyxmmc = dr["yyxmbh"].ToString();               //项目名称
                    string jx = dr["jx"].ToString();                   //
                    string gg = "";
                    Int32 xmlx = Int32.Parse(dr["sfxmdjdm"].ToString());
                    Int32 ggdm = Int32.Parse("0");
                    sl = double.Parse(dr["sl"].ToString());
                    dj = double.Parse(dr["dj"].ToString());
                    je = double.Parse(dr["dj"].ToString());

                    strValue = grbh + "|" + "0" + "|" + ybxmbm + "|" + yyxmmc + "|" + jx + "|" + gg + "|" + xmlx + "|" + ggdm + "|" + sl + "|" + dj + "|" + je + "|";
                    WriteLog(sysdate + "   " + jzlsh + "上传处方明细(挂号)->" + strValue);
                    i = mzcscf2(id, falg, ybxmbm, yyxmmc, jx, gg, xmlx, ggdm, sl, dj, je);

                    if (i != 0)
                    {
                        string errMsg = errorMessage();
                        WriteLog(sysdate + "   " + jzlsh + "进入门诊登记收费｜诊查费上传失败|" + errMsg);
                        object[] objDK = { };
                        YBMZDK(objDK);
                        return new object[] { 0, 0, errMsg };
                    }

                    strSql = string.Format(@"insert into ybcfmxscindr(jzlsh,jylsh,xm,kh,ybjzlsh,cfrq,yysfxmbm,yysfxmmc,sfxmzxbm,sfxmzxmc,
                                            dj,sl,je,jbr,sysdate) values(
                                            '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}',
                                            '{10}','{11}','{12}','{13}','{14}')",
                                            jzlsh, JYLSH, xm, kh, ybjzlsh, cfrq, yyxmbm, yyxmmc, ybxmbm, ybxmmc,
                                            dj, sl, je, jbr, sysdate);
                    liSQL.Add(strSql);
                }
                #endregion
                ds.Dispose();
                #endregion

                #region 门诊挂号费结算
                WriteLog(sysdate + "   " + jzlsh + "门诊登记收费结算(挂号).....");
                Int32 id32 = Int32.Parse(grbh);
                double ylfhj = Convert.ToDouble(ghfy);
                Int32 jsfs = 1;
                string icd10 = bzbm;
                string jbzl = bzmc;
                string dgysxm = ysdm;
                string kbrq = ghdjsj; //取日期
                string mzh = ybjzlsh; //门诊号等于就诊流水号
                string dgysbh = dgysbm;
                string mm = ""; //医保卡密码
                string yllb1 = yllb; //医疗类别
                string xm1 = xm;    //医保卡患者姓名
                string grbh1 = grbh;    //个人编号
                string kh1 = kh;    //卡号
                string sfymm = isPwd; //是否有医保密码
                if (sfymm.Equals("1"))
                {
                    frm_inputPW inputPW = new frm_inputPW();
                    inputPW.ShowDialog();
                    mm = inputPW.sValue;
                }

                double xj = 0.00; //现金支付额
                double grzhje = 0.00;//个人帐户支付额
                double grzhye = 0.00; //个人帐户余额
                double tcj = 0.00; //统筹金支付额

                WriteLog(sysdate + "  " + jzlsh + "门诊登记收费结算(挂号)|入参|" + id32 + "|" + ylfhj + "|" + jsfs + "|" + icd10 + "|" + jbzl + "|" + dgysxm + "|" + kbrq + "|" + mzh + "|" + mm + "|" + "|" + dgysbh + "|");
                i = mzjs215(id32, ylfhj, jsfs, icd10, jbzl, dgysxm, kbrq, mzh, dgysbh, mm, ref xj, ref grzhje, ref grzhye, ref tcj);

                WriteLog(sysdate + "  " + jzlsh + "门诊登记收费结算(挂号)|出参|" + xj + "|" + grzhje + "|" + grzhye + "|" + tcj);
                if (i != 0)
                {
                    string errMsg = errorMessage();
                    WriteLog(sysdate + "   " + jzlsh + "门诊登记收费结算(挂号)失败｜" + errMsg);
                    return new object[] { 0, 0, errMsg };
                }
                outParams_js js = new outParams_js();
                //Mzjs212、mzjs215输出参数：现金支付额，个人帐户支付额，个人帐户余额，统筹金支付额。
                js.Ylfze = ylfhj.ToString();
                js.Tcjjzf = tcj.ToString();
                js.Dejjzf = "0.00";
                js.Zhzf = grzhje.ToString();
                js.Xjzf = xj.ToString();
                js.Bcjsqzhye = (grzhye + grzhje).ToString();
                js.Zbxje = (ylfhj - xj).ToString();
                js.Qtybzf = js.Zbxje;


                /*医疗费总额|总报销金额|统筹基金支付|大额基金支付|账户支付|
                * 现金支付|公务员补助基金支付|企业补充医疗保险基金支付|自费费用|单位负担费用|
                * 医院负担费用|民政救助费用|超限价费用|乙类自理费用|丙类自理费用|
                * 符合基本医疗费用|起付标准费用|转诊转院自付费用|进入统筹费用|统筹分段自付费用|
                * 超统筹封顶线费用|进入大额报销费用|大额分段自付费用|超大额封顶线费用|人工器官自付费用|
                * 本次结算前帐户余额|本年统筹支付累计(不含本次)|本年大额支付累计(不含本次)|本年城镇居民门诊统筹支付累计(不含本次)|本年公务员补助支付累计(不含本次)|
                * 本年账户支付累计(不含本次)|本年住院次数累计(不含本次)|住院次数|姓名|结算时间|
                * 医疗类别|医疗待遇类别|经办机构编码|业务周期号|结算流水号|
                * 提示信息|单据号|交易类型|医院交易流水号|有效标志|
                * 个人编号管理|医疗机构编码|二次补偿金额|门慢起付累计|接收方交易流水号|
                * 个人编号|单病种补差|财政支出金额|二类门慢限额支出|二类门慢限额剩余|
                * 居民个人自付二次补偿金额|体检金额|生育基金支付|其他医保支付|
                */
                string sParam = js.Ylfze + "|" + js.Zbxje + "|" + js.Tcjjzf + "|" + js.Dejjzf + "|" + js.Zhzf + "|" +
                                js.Xjzf + "|" + js.Gwybzjjzf + "|" + js.Qybcylbxjjzf + "|" + js.Zffy + "|" + js.Dwfdfy + "|" +
                                js.Yyfdfy + "|" + js.Mzjzfy + "|" + js.Cxjfy + "|" + js.Ylzlfy + "|" + js.Blzlfy + "|" +
                                js.Fhjjylfy + "|" + js.Qfbzfy + "|" + js.Zzzyzffy + "|" + js.Jrtcfy + "|" + js.Tcfdzffy + "|" +
                                js.Ctcfdxfy + "|" + js.Jrdebxfy + "|" + js.Defdzffy + "|" + js.Cdefdxfy + "|" + js.Rgqgzffy + "|" +
                                js.Bcjsqzhye + "|" + js.Bntczflj + "|" + js.Bndezflj + "|" + js.Bnczjmmztczflj + "|" + js.Bngwybzzflj + "|" +
                                js.Bnzhzflj + "|" + js.Bnzycslj + "|" + js.Zycs + "|" + js.Xm + "|" + js.Jsrq + js.Jssj + "|" +
                                js.Yllb + "|" + js.Yldylb + "|" + js.Jbjgbm + "|" + js.Ywzqh + "|" + js.Jslsh + "|" +
                                js.Tsxx + "|" + js.Djh + "|" + js.Jyxl + "|" + js.Yyjylsh + "|" + js.Yxbz + "|" +
                                js.Grbhgl + "|" + js.Yljgbm + "|" + js.Ecbcje + "|" + js.Mmqflj + "|" + js.Jsfjylsh + "|" +
                                js.Grbh + "|" + js.Dbzbc + "|" + js.Czzcje + "|" + js.Elmmxezc + "|" + js.Elmmxesy + "|" +
                                js.Jmgrzfecbcje + "|" + js.Tjje + "|" + js.Syjjzf + "|" + js.Qtybzf + "|";
                #endregion

                //数据操作
                strSql = string.Format(@"insert into ybfyjsdr(
                                        jzlsh, jylsh ,ylfze ,xjzf ,zhzf ,tcjjzf, dejjzf,gwybzjjzf,qybcylbxjjzf,ylzlfy,
                                        fhjbylfy,qfbzfy,jrtcfy,ctcfdxfy,jrdebxfy,defdzffy,zzzyzffy,zffy,rgqgzffy,mzjzfy,
                                        dwfdfy, yllb,xm,grbh,kh,cfmxjylsh,ywzqh,jslsh,jsrq,djh,
                                        bcjsqzhye,jbr,sysdate,ybjzlsh,zcfbz,djhin)
                                        values('{0}','{1}','{2}','{3}','{4}','{5}' ,'{6}','{7}','{8}' ,'{9}' ,
                                        '{10}','{11}' ,'{12}','{13}' ,'{14}','{15}','{16}', '{17}','{18}','{19}',
                                        '{20}' ,'{21}','{22}','{23}','{24}','{25}','{26}', '{27}' ,'{28}','{29}',
                                        '{30}','{31}','{32}','{33}','{34}','{35}')",
                                        jzlsh, JYLSH, js.Ylfze, js.Xjzf, js.Zhzf, js.Tcjjzf, "0.00", "0.00", "0.00", "0.00",
                                        "0.00", "0.00", "0.00", "0.00", "0.00", "0.00", "0.00", "0.00", "0.00", "0.00",
                                        "0.00", yllb, xm, grbh, kh, JYLSH, "", djhin, ghdjsj, djhin,
                                        js.Bcjsqzhye, jbr, sysdate, ybjzlsh, 'g',djhin);
                liSQL.Add(strSql);
                object[] obj_gh = liSQL.ToArray();
                obj_gh = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj_gh);
                if (obj_gh[1].ToString().Equals("1"))
                {
                    WriteLog(sysdate + "  门诊登记(挂号)收费成功|出参(1)|" + sParam);
                    return new object[] { 0, 1, sParam };
                }
                else
                {
                    //撤销费用结算
                    object[] objParam2 = { grbh, js.Ylfze, js.Ylfze, ybjzlsh };
                    object[] objReturn2 = N_YBMZSFJSCX(objParam2);
                    WriteLog(sysdate + "  门诊登记(挂号)收费失败|" + obj_gh[2].ToString());
                    return new object[] { 0, 0, "门诊登记(挂号)收费失败" + obj_gh[2].ToString() };
                }
            }
            catch (Exception error)
            {
                WriteLog(sysdate + "  门诊登记(挂号)收费异常|" + error.Message);
                return new object[] { 0, 2, "Error:" + error.Message };
            }
        }
        #endregion

        #region 门诊登记(挂号)收费撤销
        /// <summary>
        /// 门诊登记收费撤销
        /// </summary>
        /// <param>就诊流水号</param>
        /// <returns>1:成功，0:不成功，2:报错</returns>
        public static object[] YBMZDJSFCX(object[] objParam)
        {
            string sysdate = GetServerDateTime();
            try
            {
                string jbr = CliUtils.fUserName;
                string jzlsh = objParam[0].ToString();
                string djh = objParam[2].ToString();

                #region 是否已结算
                string strSql = string.Format(@"select a.cfmxjylsh,a.jslsh,a.jsrq,b.*,a.*  from ybfyjsdr a join ybmzzydjdr b on a.ybjzlsh = b.ybjzlsh where a.jzlsh = '{0}' and a.djh = '{1}' and a.cxbz = 1  and b.cxbz = 1 ", jzlsh, djh);
                DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);

                if (ds.Tables[0].Rows.Count == 0)
                {
                    WriteLog(sysdate + "  该患者无结算信息|");
                    return new object[] { 0, 0, "该患者无结算信息" };
                }
                #endregion

                #region 进入住院处方明细上报...
                string ybjzlsh = ds.Tables[0].Rows[0]["ybjzlsh"].ToString(); //医保就诊号
                string ylfze = ds.Tables[0].Rows[0]["ylfze"].ToString();
                string tcjjzf = ds.Tables[0].Rows[0]["tcjjzf"].ToString();
                string xjzf = ds.Tables[0].Rows[0]["xjzf"].ToString();
                string zhzf = ds.Tables[0].Rows[0]["zhzf"].ToString();
                string bcjsqzhye = ds.Tables[0].Rows[0]["bcjsqzhye"].ToString();
                string cfmxjylsh = ds.Tables[0].Rows[0]["cfmxjylsh"].ToString();
                #endregion

                #region 医保登记信息
                strSql = string.Format(@"select * from ybmzzydjdr a where a.ybjzlsh='{0}' and a.cxbz = 1", ybjzlsh);
                ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);

                if (ds == null || ds.Tables[0].Rows.Count == 0)
                {
                    return new object[] { 0, 0, "医保提示：医保未办理挂号登记" };
                }
                DataRow dr = ds.Tables[0].Rows[0];
                string grbh = dr["grbh"].ToString();
                string xm = dr["xm"].ToString();
                string kh = dr["kh"].ToString();
                ds.Dispose();
                #endregion

                #region 医保收费撤销
                /*
                 输入参数：个人标识号，原医疗费金额，退费金额
                 输出参数：退还现金额，退回个人帐户金额，个人帐户余额，退回统筹金
                 函数结果：0 或错误代码。
                */
                Int32 id = Int32.Parse(grbh);
                double yylf = Convert.ToDouble(ylfze);
                double tfje = Convert.ToDouble(ylfze);
                string mzh = jzlsh;
                double txj = 0.00;
                double tgrzh = 0.00;
                double grzhye = 0.00;
                double ttcj = 0.00;

                WriteLog(sysdate + "   " + jzlsh + "进入门诊登记(挂号)收费撤销...");
                WriteLog(sysdate + "  门诊登记(挂号)收费撤销|入参|" + id + "|" + yylf + "|" + tfje + "|" + ybjzlsh + "|");
                List<string> liSQL = new List<string>();
                //退还现金额，退回个人帐户金额，个人帐户余额，退回统筹金
                int i = mztf211(id, yylf, tfje, ybjzlsh, ref txj, ref tgrzh, ref grzhye, ref ttcj);

                if (i != 0)
                {
                    string errMsg = errorMessage();
                    WriteLog(sysdate + "   " + jzlsh + "门诊登记(挂号)收费撤销失败|" + errMsg);
                    return new object[] { 0, 0, "门诊结算失败！" + errMsg };
                }

                WriteLog(sysdate + "  门诊登记(挂号)收费撤销|出参|" + txj + "|" + tgrzh + "|" + grzhye + "|" + ttcj + "|");

                strSql = string.Format(@"insert into ybfyjsdr ( jzlsh, jylsh ,ylfze ,xjzf ,zhzf ,tcjjzf, dejjzf,gwybzjjzf,qybcylbxjjzf,ylzlfy,
                                        fhjbylfy,qfbzfy,jrtcfy,ctcfdxfy,jrdebxfy,defdzffy,zzzyzffy,zffy,rgqgzffy,mzjzfy,
                                        dwfdfy, yllb,xm,grbh,kh,cfmxjylsh,ywzqh,jslsh,jsrq,djh,djhin,
                                        bcjsqzhye,jbr,cxbz,sysdate) 
                                        select jzlsh, jylsh ,ylfze ,xjzf ,zhzf ,tcjjzf, dejjzf,gwybzjjzf,qybcylbxjjzf,ylzlfy,
                                        fhjbylfy,qfbzfy,jrtcfy,ctcfdxfy,jrdebxfy,defdzffy,zzzyzffy,zffy,rgqgzffy,mzjzfy,
                                        dwfdfy, yllb,xm,grbh,kh,cfmxjylsh,ywzqh,jslsh,jsrq,djh,djhin,
                                        bcjsqzhye,jbr,0,'{2}' from ybfyjsdr where jzlsh = '{0}' and djh = '{1}' and cxbz = 1", jzlsh, djh, sysdate);

                liSQL.Add(strSql);
                strSql = string.Format(@"update ybfyjsdr set cxbz = 2 where jzlsh = '{0}' and djh = '{1}' and cxbz = 1", jzlsh, djh);
                liSQL.Add(strSql);
                strSql = string.Format(@"insert into ybcfmxscindr(jzlsh,jylsh,je,dj,sl,sflb,sfxmzl,qezfbz,grbh,xm,
                                                kh,ybcfh,yysfxmbm,yysfxmmc,sfxmzxbm,ybxmmc,cxbz,sysdate)
                                                select jzlsh,jylsh,je,dj,sl,sflb,sfxmzl,qezfbz,grbh,xm,
                                                kh,ybcfh,yysfxmbm,yysfxmmc,sfxmzxbm,ybxmmc,0,'{2}' from ybcfmxscindr where jzlsh = '{0}' and jylsh='{1}' and cxbz = 1", jzlsh, cfmxjylsh, sysdate);
                liSQL.Add(strSql);
                strSql = string.Format(@"update ybcfmxscindr set cxbz=2 where jzlsh = '{0}' and jylsh='{1}' and cxbz = 1", jzlsh, cfmxjylsh);
                liSQL.Add(strSql);

                strSql = string.Format(@"insert into ybmzzydjdr
                                        (jzlsh,jylsh,yllb,ghdjsj,bzbm,bzmc,ksbh,ksmc,ysdm,ysxm,
                                        jbr,grbh,xm,kh,ghf,ybzl,ybjzlsh,jzbz,tcqh,yldylb,
                                        dgysdm,dgysxm,cxbz,sysdate,sfzh,xb,zhye,dwmc)
                                        select jzlsh,jylsh,yllb,ghdjsj,bzbm,bzmc,ksbh,ksmc,ysdm,ysxm,
                                        jbr,grbh,xm,kh,ghf,ybzl,ybjzlsh,jzbz,tcqh,yldylb,
                                        dgysdm,dgysxm,cxbz,'{1}',sfzh,xb,zhye,dwmc from ybmzzydjdr where ybjzlsh='{0}' and cxbz=1", ybjzlsh, sysdate);
                liSQL.Add(strSql);
                strSql = string.Format(@"update ybmzzydjdr set cxbz=2 where ybjzlsh = '{0}'  and cxbz = 1", ybjzlsh);
                liSQL.Add(strSql);


                object[] obj = liSQL.ToArray();
                obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);
                if (obj[1].ToString().Equals("1"))
                {
                    WriteLog(sysdate + "  门诊登记(挂号)收费撤销成功");
                    return new object[] { 0, 1, "门诊登记(挂号)收费撤销成功" };
                }
                else
                {
                    WriteLog(sysdate + "  门诊登记(挂号)收费撤销失败|" + obj[2].ToString());
                    return new object[] { 0, 0, "门诊登记(挂号)收费撤销失败" + obj[2].ToString() };
                }
                #endregion
            }
            catch (Exception ex)
            {
                WriteLog(sysdate + "  门诊登记(挂号)收费撤销异常|" + ex.Message);
                return new object[] { 0, 0, "门诊登记(挂号)收费撤销异常|" + ex.Message };
            }
        }
        #endregion

        #region 门诊处方明细上报
        public static object[] YBMZSFDJ(object[] param2)
        {
            string sysdate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");//当前时间
            string jzlsh = param2[0].ToString();  //就诊流水号
            string jbr = CliUtils.fUserName;    //经办人姓名
            string cfhs = param2[1].ToString();   //处方号
            string sfje = param2[2].ToString(); //医疗合计费用 (新增)
            string sfymm = param2[3].ToString(); //医保卡密码 （新增）
            string cfsj = param2[4].ToString(); //处方时间
            string jsfs = param2[5].ToString(); //
            //string ybjzlsh = param2[6].ToString();


            JYLSH = Convert.ToDateTime(cfsj).ToString("yyyyMMddHHmmss") + "-" + YLGHBH + "-" + new Random().Next(100).ToString().PadLeft(4, '0');

            string strMsg = string.Empty; //记录上报处方明细
            string strMsg1 = string.Empty; //插入数据表ybcfmxscfhdr,返回错误信息

            if (string.IsNullOrEmpty(jzlsh))
                return new object[] { 0, 0, "就诊流水号不能为空！" };
            if (string.IsNullOrEmpty(cfhs))
                return new object[] { 0, 0, "处方号不能为空！" };
            if (string.IsNullOrEmpty(sfje))
                return new object[] { 0, 0, "医疗合计费不能为空" };
            if (string.IsNullOrEmpty(cfsj))
                return new object[] { 0, 0, "看病日期不能为空,时间格式为:yyyy-MM-dd HH:mm:ss" };

            string strSql = string.Format("select * from ybmzzydjdr a where a.jzlsh = '{0}' and a.cxbz = 1", jzlsh);

            DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            if (ds == null || ds.Tables[0].Rows.Count == 0)
                return new object[] { 0, 0, "无医保门诊登记信息" };

            DataRow dr1 = ds.Tables[0].Rows[0];
            string grbh = dr1["grbh"].ToString(); //个人编码
            string xm = dr1["xm"].ToString();  //姓名
            string kh = dr1["kh"].ToString();  //卡号
            string icd10_1 = dr1["bzbm"].ToString(); //病种代码
            string jbzl1 = dr1["bzmc"].ToString(); //病种名称
            string dgysxm = dr1["dgysxm"].ToString(); //定岗医生姓名
            string dgysbh = dr1["dgysdm"].ToString(); //定岗医生代码
            string ybjzlsh = dr1["ybjzlsh"].ToString(); //定岗医生代码

            #region 处方明细
            StringBuilder rc = new StringBuilder();
            strSql = string.Format(@"select y.ybxmbh, y.ybxmmc, m.dj, sum(m.sl) sl, sum(m.je) je, m.yyxmbh, m.yyxmmc, y.sfxmzldm, y.sflbdm,y.jxdm, m.cfh,m.gg,m.jxdw,y.jx,m.yf,y.sfxmdjdm  from 
                                    (
                                    --药品
                                    select a.mcypno yyxmbh, a.mcypnm yyxmmc, a.mcpric dj, a.mcquty sl, a.mcamnt je, a.mccfno cfh,a.mcypgg gg,a.mcunt1 as jxdw,a.mcwayx as yf
                                    from mzcfd a 
                                    where a.mcghno = '{0}' and a.mccfno in ({1})
                                    union all
                                    --检查/治疗
                                    select b.mbitem yyxmbh, b.mbname yyxmmc, b.mbpric dj, b.mbquty sl, b.mbsjam je,b.mbzlno cfh,NULL as gg,NULL as jxdw,NULL AS yf           
                                    from mzb2d b 
                                    where b.mbghno = '{0}' and b.mbzlno in ({1})
                                    union all
                                    --检验
                                    select c.mbitem yyxmbh, c.mbname yyxmmc, c.mbpric dj, c.mbquty sl, c.mbsjam je, c.mbzlno cfh,NULL as gg,NULL as jxdw,NULL AS yf 
                                    from mzb4d c 
                                    where c.mbghno = '{0}' and c.mbzlno in ({1})
                                    union all
                                    --注射
                                    select b5item yyxmbh, b5name yyxmmc, b5sfam dj, mddays sl, b5sfam * mddays je, mdzsno cfh,NULL as gg,NULL as jxdw,NULL AS yf 
                                    from mzd3d
                                    left join (select distinct mccfno, mcghno from mzcfd) mzcf on mccfno = mdcfno
                                    left join bz09d on b9mbno = mdtwid 
                                    left join bz05d on b5item = b9item where mdtiwe > 0 and mdzsno in ({1})
                                    union all
                                    select b5item yyxmbh, b5name yyxmmc, b5sfam dj, mdtims sl, b5sfam * mdtims je,mdzsno cfh,NULL as gg,NULL as jxdw,NULL AS yf 
                                    from mzd3d 
                                    left join bz09d on b9mbno = mdwayid left join bz05d on b5item = b9item
                                    left join (select distinct mccfno, mcghno from mzcfd) mzcf on mccfno = mdcfno 
                                    where mdzsno in ({1})
                                    union all
                                    select b5item yyxmbh, b5name yyxmmc, b5sfam dj, mdpqty sl, b5sfam * mdpqty je,mdzsno cfh,NULL as gg,NULL as jxdw,NULL AS yf 
                                    from mzd3d 
                                    left join bz09d on b9mbno = mdpprid 
                                    left join bz05d on b5item = b9item
                                    left join (select distinct mccfno, mcghno from mzcfd) mzcf on mccfno = mdcfno
                                    where mdpqty > 0 and mdzsno in ({1})
                                    ) m 
                                    left join ybhisdzdr y on m.yyxmbh = y.hisxmbh
                                    group by y.ybxmbh, y.ybxmmc, m.dj, m.yyxmbh, m.yyxmmc, y.sfxmzldm, y.sflbdm,y.jxdm, m.cfh,m.gg,m.jxdw,y.jx,m.yf,y.sfxmdjdm", jzlsh, cfhs);
            ds.Tables.Clear();
            ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);

            List<string> li_cfxm = new List<string>(); //添加处方明细
           
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                DataTable dt = ds.Tables[0];
                StringBuilder wdzxms = new StringBuilder();
                for (int k = 0; k < dt.Rows.Count; k++)
                {
                    DataRow dr = dt.Rows[k];
                    if (dr["ybxmbh"] == DBNull.Value)
                        wdzxms.Append("项目代码：[" + dr["yyxmbh"].ToString() + "]，名称：[" + dr["yyxmmc"].ToString() + "]未对照，不能上传！");
                    else
                    {
                        string ybsfxmzldm = dr["sfxmzldm"].ToString();  //收费项目等级代码
                        string ybsflbdm = dr["sflbdm"].ToString();      //收费类别代码
                        string yyxmbh = dr["yyxmbh"].ToString();          //医院项目代码
                        string yyxmmc = dr["yyxmmc"].ToString();          //医院项目名称
                        string ybxmbh = dr["ybxmbh"].ToString();          //医保项目编号
                        string ybxmmc = dr["ybxmmc"].ToString();          //医保项目名称
                        decimal dj = Convert.ToDecimal(dr["dj"]);//单价
                        decimal sl = Convert.ToDecimal(dr["sl"]);//数量
                        decimal je = Convert.ToDecimal(dr["je"]); //金额
                        string jx = dr["jx"].ToString();  //剂型
                        string gg = dr["gg"].ToString(); //规格
                        decimal mcyl = 1;
                        string cfh = dr["cfh"].ToString(); //处方号
                        string xmlx = dr["sfxmdjdm"].ToString(); //项目类型
                        int ggdm = 0; //规格代码
                        string ybcfh = cfsj + k.ToString();
                        /*
                         Mzcscf2输入参数：个人标识号，医保项目编码（0或-1），医院项目编码(江西省20位医保编码)，项目名称，剂型，规格，项目类型，规格代码，数量，单价，金额
                         */
                        string sTmp = grbh + "|" + "0" + "|" + ybxmbh + "|" + yyxmmc + "|" + jx + "|" + gg + "|" + xmlx + "|" + ggdm + "|" + sl + "|" + dj + "|" + je + "|" + yyxmbh + "|" + ybxmmc + "|" + cfh + "|" + ybcfh;
                        li_cfxm.Add(sTmp);
                    }
                }
                if (wdzxms.Length > 0)
                    return new object[] { 0, 0, wdzxms.ToString() };
            }
            else
                return new object[] { 0, 0, "无费用明细" };
            #endregion

            #region 处方明细上传
            int iResult = 0;
            foreach (string strValue in li_cfxm)
            {
                string[] s = strValue.Split('|');
                Int32 id = Int32.Parse(s[0]);             //个人标识号
                Int32 ybxmbm = Int32.Parse(s[1]);   //医保项目编码（0或－1）
                string yyxmbm = s[2];               //医院项目编码（20位省医保编码）
                string yyxmmc = s[3];               //项目名称
                string jx = s[4];                   //
                string gg = s[5];
                Int32 xmlx = Int32.Parse(s[6]);
                Int32 ggdm = Int32.Parse(s[7]);
                double sl = double.Parse(s[8]);
                double dj = double.Parse(s[9]);
                double je = double.Parse(s[10]);

                WriteLog(sysdate + "   " + jzlsh + "上传门诊处方明细->" + strValue);
                iResult += mzcscf2(id, ybxmbm, yyxmbm, yyxmmc, jx, gg, xmlx, ggdm, sl, dj, je);
                strMsg += strValue + ";";
            }
            #endregion

            #region 数据保存
            if (0 == iResult)
            {
                WriteLog(sysdate + "   " + jzlsh + "进入门诊处方上传成功|");

                if (jsfs.Equals("1"))
                {
                    List<string> liSQL = new List<string>();
                    for (int j = 0; j < li_cfxm.Count; j++)
                    {
                        string[] st = li_cfxm[j].Split('|');
                        grbh = st[0];          //个人标识号
                        string ybxmbm = st[2];               //医保项目编码
                        string yyxmmc = st[3];               //医院项目名称
                        string jx = st[4];                   //剂型
                        string gg = st[5];                   //规格
                        Int32 xmlx = Int32.Parse(st[6]);     //项目类型
                        Int32 ggdm = Int32.Parse(st[7]);     //规格代码
                        decimal sl = decimal.Parse(st[8]);     //数量
                        decimal dj = decimal.Parse(st[9]);     //单价
                        decimal je = decimal.Parse(st[10]);    //金额
                        string yyxmbm = st[11];   //医院项目编码
                        string ybxmmc = st[12];   //医保项目名称
                        string cfh = st[13];      //处方号
                        string ybcfh = st[14];    //医保处方号
                        //收费项目等级
                        string sfxmdj = "";
                        //全额自费标志
                        string qezfbz = "";
                        strSql = string.Format(@"insert into ybcfmxscindr(jzlsh,jylsh,je,dj,sl,sflb,sfxmzl,qezfbz,grbh,xm,
                                                kh,ybcfh,yysfxmbm,yysfxmmc,sfxmzxbm,ybxmmc,ybjzlsh)  values(
                                                '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}',
                                                '{10}','{11}','{12}','{13}','{14}','{15}','{16}')",
                                                jzlsh, JYLSH, je, dj, sl, "", sfxmdj, qezfbz, grbh, xm,
                                                kh, cfh, yyxmbm, yyxmmc, ybxmbm, ybxmmc, ybjzlsh);
                        liSQL.Add(strSql);
                    }
                    object[] obj = liSQL.ToArray();
                    obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);
                    if (obj[1].ToString().Equals("1"))
                    {
                        WriteLog(sysdate + "   " + jzlsh + "进入门诊处方上传成功|");
                        return new object[] { 0, 1, JYLSH };
                    }
                    else
                    {
                        WriteLog(sysdate + "   " + jzlsh + "进入门诊处方上传|本地数据操作失败|");
                        return new object[] { 0, 0, "门诊处方上传|数据库操作失败" + obj[2].ToString() };
                    }
                }
                else
                {
                    WriteLog(sysdate + "   " + jzlsh + "进入门诊处方上传成功(预结算)|" + JYLSH);
                    return new object[] { 0, 1, JYLSH };
                }

            }
            else
            {
                string errMsg = errorMessage();
                return new object[] { 0, 0, "门诊处方上传失败|mzcscf2|" + errMsg };
            }
            #endregion
        }
        #endregion

        #region 门诊处方明细上报撤销
        public static object[] YBMZCFMXSCCX(object[] objParam)
        {
            string sysdate = GetServerDateTime();
            WriteLog(sysdate + "   进入门诊费用登记撤销...");
            try
            {
                return null;
            }
            catch (Exception ex)
            {
                WriteLog(sysdate + "  门诊处方明细上报撤销异常|" + ex.Message);
                return new object[] { 0, 0, "门诊处方明细上报撤销异常" + ex.Message };
            }
        }
        #endregion

        #region 门诊费用预结算
        public static object[] YBMZSFYJS(object[] objParam)
        {
            string sysdate = GetServerDateTime(); //获取系统时间
            try
            {
                #region 入参
                string jbr = CliUtils.fUserName;    //经办人姓名
                string jzlsh = objParam[0].ToString();  //就诊流水号
                string zhsybz = objParam[1].ToString(); //账户使用标志
                string jsrq = objParam[2].ToString();   //结算时间
                string bzbh = objParam[3].ToString();   //病种编号
                string bzmc = objParam[4].ToString();   //病种名称
                string cfhs = objParam[5].ToString();   //处方号集
                string yllb = objParam[6].ToString();   //医疗类别
                string sfje = objParam[7].ToString();   //收费金额
                string cfysdm = objParam[8].ToString(); //处方医生代码
                string cfysxm = objParam[9].ToString(); //处方医生姓名
                #endregion

                if (string.IsNullOrEmpty(jzlsh))
                    return new object[] { 0, 0, "就诊流水号不能为空" };
                if (string.IsNullOrEmpty(yllb))
                    return new object[] { 0, 0, "医疗类别不能为空" };
                if (string.IsNullOrEmpty(sfje))
                    return new object[] { 0, 0, "总费用不能为空" };
                if (string.IsNullOrEmpty(cfhs))
                    return new object[] { 0, 0, "处方号集不能为空" };

                string cfmxjylsh = "";
                string mm = ""; //医保卡密码
                string ybjzlsh = "";


                #region 获取登记信息
                string strSql = string.Format(@"select a.*,b.SFYMM,b.LJTCZF,b.LJGRZHZF,b.LJMZF,b.LJZYF,b.zycs from ybmzzydjdr a 
                                    left join YBICKXX b on a.grbh=b.grbh 
                                    where a.jzlsh = '{0}' and a.cxbz = 1", jzlsh);
                DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                if (ds == null || ds.Tables[0].Rows.Count == 0)
                    return new object[] { 0, 0, "就诊流水号" + jzlsh + "未办理医保登记" };
                DataRow dr = ds.Tables[0].Rows[0];
                Int32 id = Int32.Parse(dr["grbh"].ToString());
                double ylfhj = Convert.ToDouble(sfje);
                Int32 jsfs = 3; //jsfs=3试结算，处方不留
                string icd10 = dr["bzbm"].ToString();
                string jbzl = dr["bzmc"].ToString();
                string ysxm = dr["dgysxm"].ToString();
                string kbrq = Convert.ToDateTime(dr["ghdjsj"].ToString()).ToString("yyyy-MM-dd"); //取日期
                string dgysbh = dr["dgysdm"].ToString();
                string sfymm = dr["SFYMM"].ToString(); //是否有医保密码
                yllb = dr["yllb"].ToString(); //医疗类别
                string xm = dr["xm"].ToString();    //医保卡患者姓名
                string grbh = dr["grbh"].ToString();    //个人编号
                string kh = dr["kh"].ToString();    //卡号
                string sfzh = dr["sfzh"].ToString();    //身份证号
                ybjzlsh = dr["ybjzlsh"].ToString();    //身份证号
                string mzh = ybjzlsh;

                if (sfymm.Equals("1"))
                {
                    frm_inputPW inputPW = new frm_inputPW();
                    inputPW.ShowDialog();
                    mm = inputPW.sValue;
                }

                double xj = 0.00; //现金支付额
                double grzhje = 0.00;//个人帐户支付额
                double grzhye = 0.00; //个人帐户余额
                double tcj = 0.00; //统筹金支付额
                #endregion

                #region 上传处方明细信息
                object[] objParam1 = { jzlsh, cfhs, ylfhj, mm, jsrq, jsfs, ybjzlsh };
                object[] objReutrn1 = YBMZSFDJ(objParam1);
                if (!objReutrn1[1].ToString().Equals("1"))
                    return new object[] { 0, 0, objReutrn1[2].ToString() };
                else
                    cfmxjylsh = objReutrn1[2].ToString();
                #endregion

                #region 门诊收费预结算
                WriteLog(sysdate + "   " + jzlsh + "门诊收费预结算...");
                /*
                 * 输入参数：个人标识号，医疗费合计，结算方式，icd10代码，疾病种类，医师姓名，看病日期，门诊号，定岗医师编号。215中加传密码，如果卡设置过密码，则mm不能为空，没有设置密码的，mm传空串，这个函数不会提示输入密码，其他函数会提示输入密码。预结算时不需要密码。
                 */

                WriteLog(sysdate + "  " + jzlsh + "门诊收费预结算|入参|" + id + "|" + ylfhj + "|" + jsfs + "|" + icd10 + "|" + jbzl + "|" + ysxm + "|" + kbrq + "|" + mzh + "|" + mm + "|" + "|" + dgysbh + "|");
                
                List<string> liSQL = new List<string>();
                int i = 0;
                i = mzjs215(id, ylfhj, jsfs, icd10, jbzl, ysxm, kbrq, mzh, dgysbh, mm, ref xj, ref grzhje, ref grzhye, ref tcj);
                if (i != 0)
                {
                    string errMsg = errorMessage();
                    WriteLog(sysdate + "   " + jzlsh + "门诊收费预结算失败｜" + errMsg);
                    return new object[] { 0, 0, errMsg };
                }

                WriteLog(sysdate + "  门诊收费结算|出参|" + xj + "|" + grzhje + "|" + grzhye + "|" + tcj + "|");
                #region 出参
                outParams_js js = new outParams_js();
                //Mzjs212、mzjs215输出参数：现金支付额，个人帐户支付额，个人帐户余额，统筹金支付额。
                js.Ylfze = ylfhj.ToString();
                js.Tcjjzf = tcj.ToString();
                js.Dejjzf = "0.00";
                js.Zhzf = grzhje.ToString();
                js.Xjzf = xj.ToString();
                js.Bcjsqzhye = (grzhye + grzhje).ToString();
                js.Zbxje = (ylfhj - xj).ToString();
                js.Qtybzf = js.Zbxje;


                /*医疗费总额|总报销金额|统筹基金支付|大额基金支付|账户支付|
                * 现金支付|公务员补助基金支付|企业补充医疗保险基金支付|自费费用|单位负担费用|
                * 医院负担费用|民政救助费用|超限价费用|乙类自理费用|丙类自理费用|
                * 符合基本医疗费用|起付标准费用|转诊转院自付费用|进入统筹费用|统筹分段自付费用|
                * 超统筹封顶线费用|进入大额报销费用|大额分段自付费用|超大额封顶线费用|人工器官自付费用|
                * 本次结算前帐户余额|本年统筹支付累计(不含本次)|本年大额支付累计(不含本次)|本年城镇居民门诊统筹支付累计(不含本次)|本年公务员补助支付累计(不含本次)|
                * 本年账户支付累计(不含本次)|本年住院次数累计(不含本次)|住院次数|姓名|结算时间|
                * 医疗类别|医疗待遇类别|经办机构编码|业务周期号|结算流水号|
                * 提示信息|单据号|交易类型|医院交易流水号|有效标志|
                * 个人编号管理|医疗机构编码|二次补偿金额|门慢起付累计|接收方交易流水号|
                * 个人编号|单病种补差|财政支出金额|二类门慢限额支出|二类门慢限额剩余|
                * 居民个人自付二次补偿金额|体检金额|生育基金支付|其他医保支付|
                */
                string sParam = js.Ylfze + "|" + js.Zbxje + "|" + js.Tcjjzf + "|" + js.Dejjzf + "|" + js.Zhzf + "|" +
                                js.Xjzf + "|" + js.Gwybzjjzf + "|" + js.Qybcylbxjjzf + "|" + js.Zffy + "|" + js.Dwfdfy + "|" +
                                js.Yyfdfy + "|" + js.Mzjzfy + "|" + js.Cxjfy + "|" + js.Ylzlfy + "|" + js.Blzlfy + "|" +
                                js.Fhjjylfy + "|" + js.Qfbzfy + "|" + js.Zzzyzffy + "|" + js.Jrtcfy + "|" + js.Tcfdzffy + "|" +
                                js.Ctcfdxfy + "|" + js.Jrdebxfy + "|" + js.Defdzffy + "|" + js.Cdefdxfy + "|" + js.Rgqgzffy + "|" +
                                js.Bcjsqzhye + "|" + js.Bntczflj + "|" + js.Bndezflj + "|" + js.Bnczjmmztczflj + "|" + js.Bngwybzzflj + "|" +
                                js.Bnzhzflj + "|" + js.Bnzycslj + "|" + js.Zycs + "|" + js.Xm + "|" + js.Jsrq + js.Jssj + "|" +
                                js.Yllb + "|" + js.Yldylb + "|" + js.Jbjgbm + "|" + js.Ywzqh + "|" + js.Jslsh + "|" +
                                js.Tsxx + "|" + js.Djh + "|" + js.Jyxl + "|" + js.Yyjylsh + "|" + js.Yxbz + "|" +
                                js.Grbhgl + "|" + js.Yljgbm + "|" + js.Ecbcje + "|" + js.Mmqflj + "|" + js.Jsfjylsh + "|" +
                                js.Grbh + "|" + js.Dbzbc + "|" + js.Czzcje + "|" + js.Elmmxezc + "|" + js.Elmmxesy + "|" +
                                js.Jmgrzfecbcje + "|" + js.Tjje + "|" + js.Syjjzf + "|" + js.Qtybzf + "|";
                #endregion

                WriteLog(sysdate + "  门诊收费预结算成功|出参(1)|" + sParam);
                return new object[] { 0, 1, sParam };

                #endregion
            }
            catch (Exception ex)
            {
                WriteLog(sysdate + "  门诊费用预结算异常|" + ex.Message);
                return new object[] { 0, 0, "门诊费用预结算异常|" + ex.Message };
            }
        }
        #endregion

        #region 门诊费用结算
        public static object[] YBMZSFJS(object[] objParam)
        {
            string sysdate = GetServerDateTime();
            try
            {
                #region 入参
                string jzlsh = objParam[0].ToString();  //就诊流水号
                string djh = objParam[1].ToString();    //单据号（发票号)
                string zhsybz = objParam[2].ToString(); //账户使用标志(；在吉安地区用不到,门诊收费走账户)
                string jssj = objParam[3].ToString();   //结算时间
                string bzbh = objParam[4].ToString();   //病种编号
                string bzmc = objParam[5].ToString();   //病种名称
                string cfhs = objParam[6].ToString();   //处方号集
                string yllb = objParam[7].ToString();   //医疗类别
                string sfje = objParam[8].ToString();   //收费金额
                string cfysdm = objParam[9].ToString(); //处方医生代码
                string cfysxm = objParam[10].ToString();    //处方医生姓名
                #endregion
                string jbr = CliUtils.fLoginUser;
                string mm = "";
                string cfmxjylsh = string.Empty;
                string ybjzlsh = "";
                //if (string.IsNullOrEmpty(bzbh))
                //    ybjzlsh = "MZ" + yllb + jzlsh;
                //else
                //    ybjzlsh = "MZ" + yllb + jzlsh + bzbh;

                if (string.IsNullOrEmpty(jzlsh))
                    return new object[] { 0, 0, "就诊流水号不能为空" };
                if (string.IsNullOrEmpty(yllb))
                    return new object[] { 0, 0, "医疗类别不能为空" };

                JYLSH = DateTime.Now.ToString("yyyyMMddHHmmss") + "-" + YLGHBH + "-" + new Random().Next(100).ToString().PadLeft(4, '0');

                #region 获取登记信息
                string strSql = string.Format(@"select a.*,b.SFYMM,b.LJTCZF,b.LJGRZHZF,b.LJMZF,b.LJZYF,b.zycs from ybmzzydjdr a 
                                                left join YBICKXX b on a.grbh=b.grbh 
                                                where a.jzlsh = '{0}' and a.cxbz = 1", jzlsh);
                DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                if (ds == null || ds.Tables[0].Rows.Count == 0)
                    return new object[] { 0, 0, "就诊流水号" + jzlsh + "未办理医保登记" };
                DataRow dr = ds.Tables[0].Rows[0];
                Int32 id = Int32.Parse(dr["grbh"].ToString());
                double ylfhj = Convert.ToDouble(sfje);
                Int32 jsfs = 1; //jsfs=1结算，处方保留
                string icd10 = dr["bzbm"].ToString();
                string jbzl = dr["bzmc"].ToString();
                string ysxm = dr["dgysxm"].ToString();
                string kbrq = Convert.ToDateTime(dr["ghdjsj"].ToString()).ToString("yyyy-MM-dd"); //取日期
                string dgysbh = dr["dgysdm"].ToString();
                string sfymm = dr["SFYMM"].ToString(); //是否有医保密码
                yllb = dr["yllb"].ToString(); //医疗类别
                string xm = dr["xm"].ToString();    //医保卡患者姓名
                string grbh = dr["grbh"].ToString();    //个人编号
                string kh = dr["kh"].ToString();    //卡号
                ybjzlsh = dr["ybjzlsh"].ToString();    //卡号
                string mzh = ybjzlsh;

                if (sfymm.Equals("1"))
                {
                    frm_inputPW inputPW = new frm_inputPW();
                    inputPW.ShowDialog();
                    mm = inputPW.sValue;
                }

                double xj = 0.00; //现金支付额
                double grzhje = 0.00;//个人帐户支付额
                double grzhye = 0.00; //个人帐户余额
                double tcj = 0.00; //统筹金支付额
                #endregion

                #region 上传处方明细信息
                object[] objParam1 = { jzlsh, cfhs, ylfhj, mm, jssj, jsfs, ybjzlsh };
                object[] objReutrn1 = YBMZSFDJ(objParam1);
                if (!objReutrn1[1].ToString().Equals("1"))
                    return new object[] { 0, 0, objReutrn1[2].ToString() };
                else
                    cfmxjylsh = objReutrn1[2].ToString();
                #endregion

                #region 门诊收费结算
                WriteLog(sysdate + "   " + jzlsh + "门诊收费结算...");
                /*
                 * 输入参数：个人标识号，医疗费合计，结算方式，icd10代码，疾病种类，医师姓名，看病日期，门诊号，定岗医师编号。215中加传密码，如果卡设置过密码，则mm不能为空，没有设置密码的，mm传空串，这个函数不会提示输入密码，其他函数会提示输入密码。预结算时不需要密码。
                 */

                WriteLog(sysdate + "  " + jzlsh + "门诊收费结算|入参|" + id + "|" + ylfhj + "|" + jsfs + "|" + icd10 + "|" + jbzl + "|" + ysxm + "|" + kbrq + "|" + mzh + "|" + mm + "|");
                List<string> liSQL = new List<string>();
                int i = mzjs215(id, ylfhj, jsfs, icd10, jbzl, ysxm, kbrq, mzh, dgysbh, mm, ref xj, ref grzhje, ref grzhye, ref tcj);

                if (i != 0)
                {
                    mzbc(id, true);
                    string errMsg = errorMessage();
                    WriteLog(sysdate + "   " + jzlsh + "门诊收费结算失败｜" + errMsg);
                    return new object[] { 0, 0, errMsg };
                }

                WriteLog(sysdate + "  门诊收费结算|出参|" + xj + "|" + grzhje + "|" + grzhye + "|" + tcj + "|");
                #region 出参
                outParams_js js = new outParams_js();
                //Mzjs212、mzjs215输出参数：现金支付额，个人帐户支付额，个人帐户余额，统筹金支付额。
                js.Ylfze = ylfhj.ToString();
                js.Tcjjzf = tcj.ToString();
                js.Dejjzf = "0.00";
                js.Zhzf = grzhje.ToString();
                js.Xjzf = xj.ToString();
                js.Bcjsqzhye = (grzhje + grzhye).ToString();
                js.Zbxje = (ylfhj - xj).ToString();
                js.Qtybzf = js.Zbxje;


                /*医疗费总额|总报销金额|统筹基金支付|大额基金支付|账户支付|
                * 现金支付|公务员补助基金支付|企业补充医疗保险基金支付|自费费用|单位负担费用|
                * 医院负担费用|民政救助费用|超限价费用|乙类自理费用|丙类自理费用|
                * 符合基本医疗费用|起付标准费用|转诊转院自付费用|进入统筹费用|统筹分段自付费用|
                * 超统筹封顶线费用|进入大额报销费用|大额分段自付费用|超大额封顶线费用|人工器官自付费用|
                * 本次结算前帐户余额|本年统筹支付累计(不含本次)|本年大额支付累计(不含本次)|本年城镇居民门诊统筹支付累计(不含本次)|本年公务员补助支付累计(不含本次)|
                * 本年账户支付累计(不含本次)|本年住院次数累计(不含本次)|住院次数|姓名|结算时间|
                * 医疗类别|医疗待遇类别|经办机构编码|业务周期号|结算流水号|
                * 提示信息|单据号|交易类型|医院交易流水号|有效标志|
                * 个人编号管理|医疗机构编码|二次补偿金额|门慢起付累计|接收方交易流水号|
                * 个人编号|单病种补差|财政支出金额|二类门慢限额支出|二类门慢限额剩余|
                * 居民个人自付二次补偿金额|体检金额|生育基金支付|其他医保支付|
                */
                string sParam = js.Ylfze + "|" + js.Zbxje + "|" + js.Tcjjzf + "|" + js.Dejjzf + "|" + js.Zhzf + "|" +
                                js.Xjzf + "|" + js.Gwybzjjzf + "|" + js.Qybcylbxjjzf + "|" + js.Zffy + "|" + js.Dwfdfy + "|" +
                                js.Yyfdfy + "|" + js.Mzjzfy + "|" + js.Cxjfy + "|" + js.Ylzlfy + "|" + js.Blzlfy + "|" +
                                js.Fhjjylfy + "|" + js.Qfbzfy + "|" + js.Zzzyzffy + "|" + js.Jrtcfy + "|" + js.Tcfdzffy + "|" +
                                js.Ctcfdxfy + "|" + js.Jrdebxfy + "|" + js.Defdzffy + "|" + js.Cdefdxfy + "|" + js.Rgqgzffy + "|" +
                                js.Bcjsqzhye + "|" + js.Bntczflj + "|" + js.Bndezflj + "|" + js.Bnczjmmztczflj + "|" + js.Bngwybzzflj + "|" +
                                js.Bnzhzflj + "|" + js.Bnzycslj + "|" + js.Zycs + "|" + js.Xm + "|" + js.Jsrq + js.Jssj + "|" +
                                js.Yllb + "|" + js.Yldylb + "|" + js.Jbjgbm + "|" + js.Ywzqh + "|" + js.Jslsh + "|" +
                                js.Tsxx + "|" + js.Djh + "|" + js.Jyxl + "|" + js.Yyjylsh + "|" + js.Yxbz + "|" +
                                js.Grbhgl + "|" + js.Yljgbm + "|" + js.Ecbcje + "|" + js.Mmqflj + "|" + js.Jsfjylsh + "|" +
                                js.Grbh + "|" + js.Dbzbc + "|" + js.Czzcje + "|" + js.Elmmxezc + "|" + js.Elmmxesy + "|" +
                                js.Jmgrzfecbcje + "|" + js.Tjje + "|" + js.Syjjzf + "|" + js.Qtybzf + "|";
                #endregion


                string strSql1 = string.Format(@"insert into ybfyjsdr(
                                                jzlsh, jylsh ,ylfze ,xjzf ,zhzf ,tcjjzf, dejjzf,gwybzjjzf,qybcylbxjjzf,ylzlfy,
                                                fhjbylfy,qfbzfy,jrtcfy,ctcfdxfy,jrdebxfy,defdzffy,zzzyzffy,zffy,rgqgzffy,mzjzfy,
                                                dwfdfy, yllb,xm,grbh,kh,cfmxjylsh,ywzqh,jslsh,jsrq,djh,
                                                bcjsqzhye,jbr,sysdate,ybjzlsh,djhin)
                                                values('{0}','{1}','{2}','{3}','{4}','{5}' ,'{6}','{7}','{8}' ,'{9}' ,
                                                '{10}','{11}' ,'{12}','{13}' ,'{14}','{15}','{16}', '{17}','{18}','{19}',
                                                '{20}' ,'{21}','{22}','{23}','{24}','{25}','{26}', '{27}' ,'{28}','{29}',
                                                '{30}','{31}','{32}','{33}','{34}')",
                                                jzlsh, JYLSH, js.Ylfze, js.Xjzf, js.Zhzf, js.Tcjjzf, "0.00", "0.00", "0.00", "0.00",
                                                "0.00", "0.00", "0.00", "0.00", "0.00", "0.00", "0.00", "0.00", "0.00", "0.00",
                                                "0.00", yllb, xm, grbh, kh, cfmxjylsh, "", djh, jssj, djh,
                                                js.Bcjsqzhye, jbr, sysdate, ybjzlsh, djh);
                liSQL.Add(strSql1);
                object[] obj = liSQL.ToArray();
                obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);
                if (obj[1].ToString().Equals("1"))
                {
                    WriteLog(sysdate + "   " + jzlsh + "门诊费用结算成功|" + sParam);
                    ////打印门诊结算单
                    //object[] objParam2 = { jzlsh, grbh };
                    //YBMZJSD(objParam2);
                    return new object[] { 0, 1, sParam };
                }
                else
                {
                    object[] objParam2 = { grbh, js.Ylfze, js.Ylfze, ybjzlsh };
                    object[] objReturn2 = N_YBMZSFJSCX(objParam2);
                    WriteLog(sysdate + "   " + jzlsh + "门诊费用结算失败|本地操作数据失败|" + obj[2].ToString());
                    return new object[] { 0, 0, obj[2].ToString() };
                }
                #endregion

            }
            catch (Exception ex)
            {
                WriteLog(sysdate + "  门诊费用结算异常|" + ex.Message);
                return new object[] { 0, 0, "门诊费用结算异常|" + ex.Message };
            }
        }
        #endregion

        #region 门诊费用结算撤销
        public static object[] YBMZSFJSCX(object[] objParam)
        {
            string sysdate = GetServerDateTime();
            WriteLog(sysdate + "  进入门诊费用结算撤销...");
            try
            {
                #region 入参
                string jzlsh = objParam[0].ToString();   // 就诊流水号
                string djh = objParam[1].ToString();     // 发票号
                #endregion
                if (string.IsNullOrEmpty(jzlsh))
                    return new object[] { 0, 0, "就诊流水号不能为空" };
                if (string.IsNullOrEmpty(djh))
                    return new object[] { 0, 0, "发票号不能为空" };


                string strSql = string.Format(@"select a.* from ybfyjsdr a where a.jzlsh = '{0}' and a.djh = '{1}' and a.cxbz = 1", jzlsh, djh);
                DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    DataRow dr = ds.Tables[0].Rows[0];
                    string ybjzlsh = dr["ybjzlsh"].ToString();
                    string xm = dr["xm"].ToString();
                    string kh = dr["kh"].ToString();
                    string grbh = dr["grbh"].ToString();
                    string jsrq = dr["jsrq"].ToString();
                    string cfmxjylsh = dr["cfmxjylsh"].ToString();
                    string jslsh = dr["jslsh"].ToString();
                    string ylfze = dr["ylfze"].ToString(); //医疗费总额
                    string tfje1 = ylfze;

                    Int32 id = Int32.Parse(grbh);
                    double yylf = Convert.ToDouble(ylfze);
                    double tfje = Convert.ToDouble(tfje1);
                    string mzh = ybjzlsh;
                    double txj = 0.00;
                    double tgrzh = 0.00;
                    double grzhye = 0.00;
                    double ttcj = 0.00;
                    /*
                        输入参数：个人标识号，原医疗费金额，退费金额，门诊号
                        输出参数：退还现金额，退回个人帐户金额，个人帐户余额，退回统筹金
                    */
                    WriteLog(sysdate + "   " + jzlsh + "进入门诊费用结算撤销...");
                    WriteLog(sysdate + "  门诊费用结算撤销|入参|" + id + "|" + yylf + "|" + tfje + "|" + mzh + "|");

                    //退还现金额，退回个人帐户金额，个人帐户余额，退回统筹金
                    int i = mztf211(id, yylf, tfje, mzh, ref txj, ref tgrzh, ref grzhye, ref ttcj);

                    if (i != 0)
                    {
                        string errMsg = errorMessage();
                        WriteLog(sysdate + "   " + jzlsh + "进入门诊费用结算撤销失败|" + errMsg);
                        return new object[] { 0, 0, "门诊结算失败！" + errMsg };
                    }

                    WriteLog(sysdate + "  门诊费用结算撤销|出参|" + txj + "|" + tgrzh + "|" + grzhye + "|" + ttcj + "|");
                    List<string> liSQL = new List<string>();
                    strSql = string.Format(@"insert into ybfyjsdr ( jzlsh, jylsh ,ylfze ,xjzf ,zhzf ,tcjjzf, dejjzf,gwybzjjzf,qybcylbxjjzf,ylzlfy,
                                                    fhjbylfy,qfbzfy,jrtcfy,ctcfdxfy,jrdebxfy,defdzffy,zzzyzffy,zffy,rgqgzffy,mzjzfy,
                                                    dwfdfy, yllb,xm,grbh,kh,cfmxjylsh,ywzqh,jslsh,jsrq,djh,djhin,
                                                    bcjsqzhye,jbr,cxbz,sysdate) 
                                                    select jzlsh, jylsh ,ylfze ,xjzf ,zhzf ,tcjjzf, dejjzf,gwybzjjzf,qybcylbxjjzf,ylzlfy,
                                                    fhjbylfy,qfbzfy,jrtcfy,ctcfdxfy,jrdebxfy,defdzffy,zzzyzffy,zffy,rgqgzffy,mzjzfy,
                                                    dwfdfy, yllb,xm,grbh,kh,cfmxjylsh,ywzqh,jslsh,jsrq,djh,djhin,
                                                    bcjsqzhye,jbr,0,'{2}' from ybfyjsdr where jzlsh = '{0}' and djh = '{1}' and cxbz = 1", jzlsh, djh, sysdate);
                    liSQL.Add(strSql);
                    strSql = string.Format(@"update ybfyjsdr set cxbz = 2 where jzlsh = '{0}' and djh = '{1}' and cxbz = 1", jzlsh, djh);
                    liSQL.Add(strSql);
                    strSql = string.Format(@"insert into ybcfmxscindr(jzlsh,jylsh,je,dj,sl,sflb,sfxmzl,qezfbz,grbh,xm,
                                                kh,ybcfh,yysfxmbm,yysfxmmc,sfxmzxbm,ybxmmc,cxbz,sysdate)
                                                select jzlsh,jylsh,je,dj,sl,sflb,sfxmzl,qezfbz,grbh,xm,
                                                kh,ybcfh,yysfxmbm,yysfxmmc,sfxmzxbm,ybxmmc,0,'{2}' from ybcfmxscindr where jzlsh = '{0}' and jylsh='{1}' and cxbz = 1", jzlsh, cfmxjylsh, sysdate);
                    liSQL.Add(strSql);
                    strSql = string.Format(@"update ybcfmxscindr set cxbz=2 where jzlsh = '{0}' and jylsh='{1}' and cxbz = 1", jzlsh, cfmxjylsh);
                    liSQL.Add(strSql);

                    object[] obj = liSQL.ToArray();
                    obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);
                    if (obj[1].ToString().Equals("1"))
                    {
                        WriteLog(sysdate + "   " + jzlsh + "进入门诊费用结算撤销成功");
                        return new object[] { 0, 1, "门诊费用结算撤销成功！" };
                    }
                    else
                    {
                        WriteLog(sysdate + "   " + jzlsh + "进入门诊费用结算撤销失败|本地数据操作失败|" + obj[2].ToString());
                        return new object[] { 0, 0, "门诊费用结算撤销失败|本地数据操作失败|" + obj[2].ToString() };
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                WriteLog(sysdate + "  进入门诊费用结算撤销异常|" + ex.Message);
                return new object[] { 0, 0, "进入门诊费用结算撤销异常|" + ex.Message };
            }
        }
        #endregion

        #region 打印门诊结算单
        public static object[] YBMZJSD(object[] objParam)
        {
            string sysdate = GetServerDateTime();//当前时间
            string jzlsh = objParam[0].ToString();
            string djh = objParam[1].ToString();     // 单据号
            if (string.IsNullOrEmpty(jzlsh))
                return new object[] { 0, 0, "就诊流水号不能为空" };
            if (string.IsNullOrEmpty(djh))
                return new object[] { 0, 0, "结算单据号不能为空" };

            string strSql = string.Format(@"select grbh,ybjzlsh from ybfyjsdr where jzlsh='{0}' and djh='{1}' and cxbz=1", jzlsh, djh);
            DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            if (ds.Tables[0].Rows.Count == 0)
                return new object[] { 0, 0, "无患者结算信息" };
            string grbh = ds.Tables[0].Rows[0]["grbh"].ToString();
            string ybjzlsh = ds.Tables[0].Rows[0]["ybjzlsh"].ToString();

            //入参
            Int32 id = Int32.Parse(grbh);
            WriteLog(sysdate + "   " + jzlsh + "进入打印门诊结算单...");
            WriteLog(sysdate + "  入参|" + ybjzlsh + "|" + grbh);
            int i = mzdyjsd211(id, ybjzlsh);
            if (i == 0)
            {
                WriteLog(sysdate + "   " + jzlsh + "进入打印门诊结算单成功|");
                return new object[] { 0, 0, "打印门诊结算单成功！" };
            }
            else
            {
                string errMsg = errorMessage();
                WriteLog(sysdate + "   " + jzlsh + "进入打印门诊结算单失败|" + errMsg);
                return new object[] { 0, 0, "打印门诊结算单|失败！" + errMsg };
            }
        }
        #endregion

        #region 住院登记
        public static object[] YBZYDJ(object[] objParam)
        {
            string sysdate = GetServerDateTime();
            WriteLog(sysdate + "  进入医保住院登记...");
            try
            {
                #region his参数
                string jbr = CliUtils.fLoginUser;   // 经办人姓名 
                string jzlsh = objParam[0].ToString(); //就诊流水号
                string yllb = objParam[1].ToString(); //医疗类别代码
                string bzbm = objParam[2].ToString(); //病种编码
                string bzmc = objParam[3].ToString(); //病种名称
                string ickxx = objParam[4].ToString(); //读卡返回信息
                string lyjgdm = objParam[5].ToString();//来源机构代码
                string lyjgmc = objParam[6].ToString();//来源机构名称
                string yllbmc = objParam[7].ToString();//医疗类别名称
                string ysdm = objParam[8].ToString(); //定岗医生代码
                string ysxm = objParam[9].ToString(); //定岗医生姓名

                string isYD = objParam[13].ToString(); //异地标识
                //string zszbm = objParam[10].ToString(); //准生证编号
                //string sylb = objParam[11].ToString();      //生育类别
                //string jhsylb = objParam[12].ToString();    //计划生育类别
                #endregion

                if (string.IsNullOrEmpty(jzlsh))
                    return new object[] { 0, 0, "就诊流水号不能为空" };
                if (string.IsNullOrEmpty(yllb))
                    return new object[] { 0, 0, "医疗类别不能为空" };
                if (string.IsNullOrEmpty(ickxx))
                    return new object[] { 0, 0, "读卡信息不能为空" };
                if (string.IsNullOrEmpty(bzbm))
                    return new object[] { 0, 0, "诊断不能为空" };

                //读卡信息
                string[] kxx = ickxx.ToString().Split('|');
                string grbh = kxx[0].ToString(); //个人编号
                string dwbm = kxx[1].ToString();  //单位编号
                string sfzh = kxx[2].ToString();  //身份证号
                string xm = kxx[3].ToString();  //姓名
                string xb = kxx[4].ToString();  //性别
                string kh = kxx[7].ToString();  //卡号
                string yldylb = kxx[8].ToString();  //医疗待遇类别
                string ydrybz = kxx[10].ToString();  //异地人员标志
                string tcqh = kxx[11].ToString();  //所属区号
                string zhye = kxx[14].ToString();  //帐户余额
                string ybklx = kxx[35].ToString(); //医保卡类型
                string isPwd = kxx[36].ToString();
                string dwmc = kxx[25].ToString(); //单位名称

                string ybjzlsh = jzlsh + DateTime.Now.ToString("HHmmss");
                JYLSH = DateTime.Now.ToString("yyyyMMddHHmmss") + "-" + YLGHBH + "-" + new Random().Next(100).ToString().PadLeft(4, '0');

                #region 是否入院登记
                string strSql = string.Format(@"select z1date as rysj,z1hznm,z1ksno,z1ksnm,'' as z1bedn,z1empn from zy01h where z1zyno = '{0}'", jzlsh);
                DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                if (ds == null || ds.Tables[0].Rows.Count == 0)
                    return new object[] { 0, 0, "无患者信息" };
                //string djsj = Convert.ToDateTime(ds.Tables[0].Rows[0]["rysj"].ToString()).ToString("yyyy-MM-dd HH:mm:ss");
                string hznm = ds.Tables[0].Rows[0]["z1hznm"].ToString();
                string cwh = ds.Tables[0].Rows[0]["z1bedn"].ToString();
                string ksbh = ds.Tables[0].Rows[0]["z1ksno"].ToString();
                string ksmc = ds.Tables[0].Rows[0]["z1ksnm"].ToString();
                //string ysxm = ds.Tables[0].Rows[0]["z1ysnm"].ToString();
                string ryrq = Convert.ToDateTime(ds.Tables[0].Rows[0]["rysj"]).ToString("yyyy-MM-dd HH:mm:ss");
                #endregion

                #region 是否医保登记
                strSql = string.Format(@"select zglb from ybickxx where grbh='{0}'", grbh);
                ds.Tables.Clear();
                ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                string zglb = ds.Tables[0].Rows[0]["zglb"].ToString();
                if (!zglb.Trim().Equals("未成年人"))
                {
                    if (!hznm.Trim().Equals(xm.Trim()))
                        return new object[] { 0, 0, "患者姓名与医保卡不符,请核对" };
                }

                strSql = string.Format(@"select * from ybmzzydjdr where jzlsh='{0}' and cxbz=1", jzlsh);
                ds.Tables.Clear();
                ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                if (ds.Tables[0].Rows.Count > 0)
                    return new object[] { 0, 0, "患者" + jzlsh + "已做医保补办登记" };

                #endregion


                #region 查询定岗医生代码
                strSql = string.Format(@"select * from ybdgyszd where ysbm = '{0}'", ysdm);
                ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                if (ds.Tables[0].Rows.Count == 0)
                {
                    WriteLog(sysdate + ysxm + "该医生无定岗医生配对信息，请进行配对");
                    return new object[] { 0, 0, ysxm + "该医生无定岗医生配对信息，请进行配对" };
                }

                string dgysdm = ds.Tables[0].Rows[0]["dgysbm"].ToString();
                #endregion


                #region 医保住院登记
                /*
                 输入参数：个人标识号，姓名，身份证号，住院号，病区名称，床位号，入院日期，疾病诊断，诊断ICD10码。入院日期请包括时间。
                 输出参数：累计住院费，个人帐户余额，起付线标准，本年度住院次数。
                */
                Int32 id = Int32.Parse(grbh);
                string zyh = ybjzlsh;
                string bqmc = ksmc;
                string ch = cwh;
                //入院日期
                double ljzyf = 0.00; //累计住院费
                double grzhye = 0.00; //个人帐户余额
                double qfxbz = 0.00;  //起付线标准
                double fdbl = 0.00; //分段起付比例
                Int32 zycs = 0; //住院次数
                string ksdm = "";
                string dgysbm = "";
                string blh = "";
                string jzbz = "";
                string wsbz = "";

                WriteLog(sysdate + "  医保住院登记|入参|" + id + "|" + xm + "|" + sfzh + "|" + zyh + "|" + bqmc + "|" + ch + "|" + ryrq + "|" + bzmc + "|" + bzbm + "|" );
                int i = 0;

                if (isYD == "1")
                {
                    i = zyrydj217(id, xm, sfzh, zyh, bqmc, ch, ryrq, bzmc, bzbm,ksdm,dgysbm,ysxm,blh,jzbz,wsbz,"2", ref ljzyf, ref grzhye, ref qfxbz, ref zycs);
                }
                else
                {
                    i = zyrydj216(id, xm, sfzh, zyh, bqmc, ch, ryrq, bzmc, bzbm, ref ljzyf, ref grzhye, ref qfxbz, ref zycs);
                } 
                if (i != 0)
                {
                    string errMsg = errorMessage();
                    WriteLog(sysdate + "   " + jzlsh + "进入医保住院登记失败|" + errMsg);
                    return new object[] { 0, 0, "医保住院登记失败！" + errMsg };
                }

                WriteLog(sysdate + "  医保住院登记|出参|" + ljzyf + "|" + grzhye + "|" + qfxbz + "|" + zycs + "|");
                #endregion

                #region 病情登记
                string icd10_1 = bzbm;
                string jbzd_1 = bzmc;
                string icd10_2 = "";
                string jbzd_2 = "";
                string icd10_3 = "";
                string jbzd_3 = "";
                string azhl = "0";
                WriteLog(sysdate + "   " + jzlsh + "进入住院登记|病情登记(临时)...");
                /*
                输入参数：个人标识号，入院日期，住院号，科室（病区）名称，床位号，cd10代码1，疾病诊断1，icd10代码2，，疾病诊断2，icd10代码3，疾病诊断3，癌症化疗，医师姓名，定岗医师编号。
                输出参数：
                函数结果：0或错误代码。
                癌症化疗，急诊标志，外伤标志：1是0不是。
                科室代码表见附表。
                其他标志暂时没使用，传空串，以后使用再说明。
               */
                WriteLog(sysdate + "  入参|" + id + "|" + ryrq + "|" + zyh + "|" + bqmc + "|" + cwh + "|" + icd10_1 + "|" + jbzd_1 + "|" + icd10_2 + "|" + jbzd_2 + "|"
                        + icd10_3 + "|" + jbzd_3 + "|" + azhl + "|" + ysxm + "|" + dgysdm);
              
                    i = zybqdj212(id, ryrq, zyh, bqmc, cwh, icd10_1, jbzd_1, icd10_2, jbzd_2, icd10_3, jbzd_3, azhl, ysxm, dgysdm);

                    if (i != 0)
                    {
                        string errMsg = errorMessage();
                        WriteLog(sysdate + "   " + jzlsh + "进入医保住院登记|病情登记失败|" + errMsg);

                        object[] objParam1 = { id, jzlsh, zyh };
                        object[] objReturn = N_YBZYDJCX(objParam1);
                        return new object[] { 0, 0, "医保住院登记|病情登记失败|" + errMsg };
                    }
                    
                #endregion

                List<string> liSQL = new List<string>();
                strSql = string.Format(@"insert into ybmzzydjdr(jzlsh,jylsh,yllb,ghdjsj,bzbm,bzmc,ksbh,ksmc,ysdm,ysxm,
                                        jbr,grbh,xm,kh,dwmc,jzbz,ybjzlsh,cwh,tcqh,yldylb,
                                        dgysdm,dgysxm,bnylflj,zhye,qfbzlj,bnzycs,sysdate,mmbzbm1,mmbzmc1,mmbzbm2,
                                        mmbzmc2,mmbzbm3,mmbzmc3,sfzh,xb,dq) values(
                                        '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}',
                                        '{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}',
                                        '{20}','{21}','{22}','{23}','{24}','{25}','{26}','{27}','{28}','{29}',
                                        '{30}','{31}','{32}','{33}','{34}','{35}')",
                                        jzlsh, JYLSH, yllb, ryrq, bzbm, bzmc, ksbh, ksmc, ysdm, "",
                                        jbr, grbh, xm, kh, dwmc, "z", ybjzlsh, cwh, tcqh, yldylb,
                                        dgysdm, ysxm, ljzyf, grzhye, qfxbz, zycs, sysdate, icd10_1, jbzd_1, icd10_2,
                                        jbzd_2, icd10_3, jbzd_3, sfzh, xb,DDYLJGMC);
                liSQL.Add(strSql);
                strSql = string.Format(@"update zy01h set z1lynm='{0}',z1lyjg='{1}',z1ybno='{3}' where z1zyno='{2}'", lyjgmc, lyjgdm, jzlsh, grbh);
                liSQL.Add(strSql);

                object[] obj = liSQL.ToArray();
                obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);
                if (obj[1].ToString().Equals("1"))
                {
                    WriteLog(sysdate + "   " + jzlsh + "进入住院登记成功|");
                    return new object[] { 0, 1, "住院登记成功！" };
                }
                else
                {
                    object[] objParam1 = { id, jzlsh, zyh };
                    object[] objReturn = N_YBZYDJCX(objParam1);
                    WriteLog(sysdate + "   " + jzlsh + "进入住院登记失败|"+obj[2].ToString());
                    return new object[] { 0, 0, "住院登记失败|" + obj[2].ToString() };
                }
            }
            catch (Exception ex)
            {
                WriteLog(sysdate + "  住院登记异常" + ex.Message);
                return new object[] { 0, 0, "住院登记异常" + ex.Message };
            }
        }
        #endregion

        #region 住院登记撤销
        public static object[] YBZYDJCX(object[] objParam)
        {
            string sysdate = GetServerDateTime();
            WriteLog(sysdate + "  进入住院费用登记撤销...");
            try
            {
                string jzlsh = objParam[0].ToString(); // 就诊流水号
                #region 判断是否已结算
                string strSql = string.Format(@"select * from ybfyjsdr a where a.jzlsh = '{0}' and a.cxbz = 1", jzlsh);
                DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                if (ds != null && ds.Tables[0].Rows.Count > 0)
                    return new object[] { 0, 0, "已收费结算或中途结算，不能冲销费用明细" };
                #endregion

                #region 是否已登记
                strSql = string.Format("select * from ybmzzydjdr a where a.jzlsh = '{0}' and jzbz='z' and a.cxbz = 1", jzlsh);
                ds.Tables.Clear();
                WriteLog(strSql);
                ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                if (ds == null || ds.Tables[0].Rows.Count == 0)
                    return new object[] { 0, 0, "无入院登记记录" };
                DataTable dt = ds.Tables[0];
                DataRow dr = dt.Rows[0];
                string grbh = dr["grbh"].ToString();  //个人编号
                string xm = dr["xm"].ToString();      //姓名
                string kh = dr["kh"].ToString();      //卡号
                string ybjzlsh = dr["ybjzlsh"].ToString();  //医保就诊流水号
                string tcqh = dr["tcqh"].ToString();        //地区编号
                #endregion


                #region 登记撤销
                //玉山参数
                Int32 id = Int32.Parse(grbh);
                string zyh = ybjzlsh;
                int i = zycxdj2(id, zyh);
                if (i != 0)
                {
                    string errMsg = errorMessage();
                    WriteLog(sysdate + "   " + zyh + "进入住院登记撤销失败|" + errMsg);
                    return new object[] { 0, 0, "住院登记撤销失败！" + errMsg };
                }
            
                    List<string> liSQL = new List<string>();
                    strSql = string.Format(@"insert into ybmzzydjdr(jzlsh,jylsh,yllb,ghdjsj,bzbm,bzmc,ksbh,ksmc,ysdm,ysxm,
                                        jbr,grbh,xm,kh,dwmc,jzbz,ybjzlsh,cwh,tcqh,yldylb,
                                        dgysdm,dgysxm,bnylflj,zhye,qfbzlj,bnzycs,cxbz,sysdate,sfzh,xb) select 
                                        jzlsh,jylsh,yllb,ghdjsj,bzbm,bzmc,ksbh,ksmc,ysdm,ysxm,
                                        jbr,grbh,xm,kh,dwmc,jzbz,ybjzlsh,cwh,tcqh,yldylb,
                                        dgysdm,dgysxm,bnylflj,zhye,qfbzlj,bnzycs,0,'{1}',sfzh,xb from ybmzzydjdr where jzlsh = '{0}' and cxbz = 1", jzlsh, sysdate);
                    liSQL.Add(strSql);
                    strSql = string.Format("update ybmzzydjdr set cxbz = 2 where jzlsh = '{0}' and cxbz = 1", jzlsh);
                    liSQL.Add(strSql);

                    strSql = string.Format(@"update zy01h set z1lyjg='0201',z1lynm='全费自理' where z1zyno='{0}'", jzlsh);
                    liSQL.Add(strSql);
                    object[] obj = liSQL.ToArray();
                    obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);
                    if (obj[1].ToString().Equals("1"))
                    {
                        WriteLog(sysdate + "   " + jzlsh + "进入住院登记撤销成功|");
                        return new object[] { 0, 1, "住院登记撤销成功!" };
                    }
                    else
                    {
                        WriteLog(sysdate + "   " + jzlsh + "进入住院登记撤销成功|本地数据操作失败|" + obj[2].ToString());
                        return new object[] { 0, 0, "住院登记撤销失败|数据库操作失败!" + obj[2].ToString() };
                    }
                #endregion
            }
            catch (Exception ex)
            {
                WriteLog(sysdate + "  住院费用登记撤销异常" + ex.Message);
                return new object[] { 0, 0, "住院费用登记撤销异常" + ex.Message };
            }
        }
        #endregion

        #region 住院病情登记
        public static object[] YBZYBQDJ(object[] objParam)
        {
            string sysdate = GetServerDateTime();
            try
            {
                string jzlsh = objParam[0].ToString();
                string icd10_1 = objParam[1].ToString();
                string jbzd_1 = objParam[2].ToString();
                string icd10_2 = objParam[3].ToString();
                string jbzd_2 = objParam[4].ToString();
                string icd10_3 = objParam[5].ToString();
                string jbzd_3 = objParam[6].ToString();

                if (string.IsNullOrEmpty(jzlsh))
                    return new object[] { 0, 0, "就诊流水号不能为空" };
                if (string.IsNullOrEmpty(icd10_1))
                    return new object[] { 0, 0, "主诊断不能为空" };

                #region 是否医保登记 
                string strSql = string.Format(@"select jzlsh,grbh,ghdjsj,ksmc,cwh,dgysdm,dgysxm,ybjzlsh from ybmzzydjdr where jzlsh='{0}' and cxbz=1", jzlsh);
                DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                if (ds.Tables[0].Rows.Count == 0)
                    return new object[] { 0, 0, "未办理医保入院登记" };
                Int32 id = Convert.ToInt32(ds.Tables[0].Rows[0]["grbh"].ToString());
                string ryrq = ds.Tables[0].Rows[0]["ghdjsj"].ToString();
                string zyh = ds.Tables[0].Rows[0]["ybjzlsh"].ToString();
                string bqmc = ds.Tables[0].Rows[0]["ksmc"].ToString();
                string cwh = ds.Tables[0].Rows[0]["cwh"].ToString();
                string azhl = "0";
                string dgysxm = ds.Tables[0].Rows[0]["dgysxm"].ToString();
                string dgysbm = ds.Tables[0].Rows[0]["dgysdm"].ToString();
                #endregion
                /*
                 输入参数：个人标识号，入院日期，住院号，科室（病区）名称，床位号，cd10代码1，疾病诊断1，icd10代码2，，疾病诊断2，icd10代码3，疾病诊断3，癌症化疗，医师姓名，定岗医师编号。
                 输出参数：
                 函数结果：0或错误代码。
                 癌症化疗，急诊标志，外伤标志：1是0不是。
                 科室代码表见附表。
                 其他标志暂时没使用，传空串，以后使用再说明
                 */
                WriteLog(sysdate + "  " + jzlsh + "进入住院病情登记...");
                WriteLog(sysdate + "  " + jzlsh + "进入住院病情登记|入参|" + id + "|" + ryrq + "|" + zyh + "|" + bqmc + "|" + cwh + "|" + icd10_1 + "|" + jbzd_1 + "|" + icd10_2 + "|" + jbzd_2 + "|" + icd10_3 + "|" + jbzd_3 + "|" + azhl + "|" + dgysxm + "|" + dgysbm + "|");
                 int i = zybqdj212(id, ryrq, zyh, bqmc, cwh, icd10_1, jbzd_1, icd10_2, jbzd_2, icd10_3, jbzd_3, azhl, dgysxm, dgysbm);
                 if (i != 0)
                 {
                     string errMsg = errorMessage();
                     WriteLog(sysdate + "   " + jzlsh + "修改病情登记失败|" + errMsg);
                     return new object[] { 0, 0, "修改病情登记失败" };
                 }

                strSql = string.Format(@"update ybmzzydjdr set bzbm='{0}',bzmc='{1}', mmbzbm1='{0}',mmbzmc1='{1}',mmbzbm2='{2}',mmbzmc2='{3}',mmbzbm3='{4}',mmbzmc3='{5}' where jzlsh='{6}' and cxbz=1",
                                             icd10_1, jbzd_1, icd10_2, jbzd_2, icd10_3, jbzd_3, jzlsh);

                object[] obj = { strSql };
                object[] objReturn = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);
                if (objReturn[1].ToString().Equals("1"))
                {
                    WriteLog(sysdate + "   " + jzlsh + "修改病情登记成功");
                    return new object[] { 0, 1, "修改病情登记成功" };
                }
                else
                {
                    WriteLog(sysdate + "   " + jzlsh + "修改病情登记失败|本地数据操作失败|"+objReturn[2].ToString());
                    MessageBox.Show("修改病情登记失败|" + objReturn[2].ToString());
                    return new object[] { 0, 0, "修改病情登记失败|" + objReturn[2].ToString() };
                }
            }
            catch (Exception ex)
            {
                WriteLog(sysdate + "  住院病情登记异常|" + ex.Message);
                return new object[] { 0, 0, "住院病情登记异常|" + ex.Message };
            }
        }
        #endregion

        #region 住院费用登记
        public static object[] YBZYSFDJ(object[] objParam)
        {
            string sysdate = GetServerDateTime();
            try
            {
                string jzlsh = objParam[0].ToString();  //就诊流水号
                //string ztjssj = objParam[1].ToString(); //中途结算时间
                string jbr = CliUtils.fUserName;    //经办人姓名
                string cfsj = DateTime.Now.ToString("yyyyMMddHHmmss");
                Int32 id = 0;

                JYLSH = cfsj + "-" + YLGHBH + "-" + new Random().Next(100).ToString().PadLeft(4, '0');

                if (string.IsNullOrEmpty(jzlsh))
                    return new object[] { 0, 0, "就诊流水号不能为空" };

                #region 是否医保登记
                string strSql = string.Format("select * from ybmzzydjdr a where a.jzlsh = '{0}' and a.cxbz = 1", jzlsh);
                DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                if (ds == null || ds.Tables[0].Rows.Count == 0)
                    return new object[] { 0, 0, "该患者未办理医保登记" };

                DataRow dr1 = ds.Tables[0].Rows[0];
                string ybjzlsh = dr1["ybjzlsh"].ToString();
                string grbh = dr1["grbh"].ToString();  //*对应玉山 个人编码
                string xm = dr1["xm"].ToString();
                string kh = dr1["kh"].ToString();
                #endregion

                WriteLog(sysdate + "   " + jzlsh + "进入住院处方明细上报...");

                #region 是否已上传
                strSql = string.Format("select * from ybcfmxscindr a where a.jzlsh = '{0}' and a.cxbz = 1", jzlsh);
                ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    #region 登记前先撤销已上传费用
                    object[] obj1 = new object[] { jzlsh };
                    obj1 = YBZYSFDJCX(obj1);
                    if (obj1[1].ToString() == "0")
                    {
                        return new object[] { 0, 0, obj1[2].ToString() };
                    }
                    #endregion
                }
                #endregion


                #region 获取处方明细
                StringBuilder inputParam = new StringBuilder();
                strSql = string.Format(@"select 
                                    b.ybxmbh,b.ybxmmc,a.z3djxx as dj,sum(case LEFT(a.z3endv,1) when '4' then -a.z3jzcs else 1*a.z3jzcs end) as sl,
                                    sum(case LEFT(a.z3endv,1) when '4' then -1*a.z3jzje else 1*a.z3jzje end) as je,a.z3item as yyxmbh, a.z3name as yyxmmc,
                                    a.z3empn as ysdm, a.z3kdys as ysxm,max(a.z3date) as yysj,
                                    a.z3ksno as ksno, a.z3zxks as zxks, z3sfno as sfno,b.jx,b.gg,
                                    b.sfxmzldm as ybsfxmzldm, b.sflbdm as ybsflbdm,b.sfxmdjdm as xmlx 
                                    from zy03d a 
                                    left join ybhisdzdr b on 
                                    a.z3item=b.hisxmbh 
                                    where a.z3ybup is null and LEFT(a.z3kind,1) in(2,4) and a.z3zyno='{0}'
                                    group by b.ybxmbh,b.ybxmmc,a.z3djxx,a.z3item,a.z3name, a.z3empn,a.z3kdys,a.z3ksno,a.z3zxks,a.z3sfno,
                                    b.sfxmzldm,b.sflbdm,b.sfxmdjdm,b.jx,b.gg
                                    having sum(case LEFT(a.z3endv,1) when '4' then -1*a.z3jzcs else 1*a.z3jzcs end)!=0", jzlsh);
                ds.Tables.Clear();
                ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);

                if (ds.Tables[0].Rows.Count == 0)
                    return new object[] { 0, 0, "无费用明细" };

                StringBuilder wdzxms = new StringBuilder();
                List<string> li_cfxm = new List<string>();
                DataTable dt = ds.Tables[0];
                for (int k = 0; k < dt.Rows.Count; k++)
                {
                    DataRow dr = dt.Rows[k];
                    if (dr["ybxmbh"] == DBNull.Value)
                        wdzxms.Append("项目代码：[" + dr["yyxmbh"].ToString() + "]，名称：[" + dr["yyxmmc"].ToString() + "]未对照，不能上传！");
                    else
                    {
                        string ybsfxmzldm = dr["ybsfxmzldm"].ToString(); // 收费项目种类代码 
                        string ybsflbdm = dr["ybsflbdm"].ToString();     // 收费类别代码
                        string yyxmbh = dr["yyxmbh"].ToString();         // 医院收费项目编号
                        string ybxmbh = dr["ybxmbh"].ToString();         // 医保项目编号 //* 对应玉山医院项目编码
                        string ybxmmc = dr["ybxmmc"].ToString();         // 医保项目名称
                        string yyxmmc = dr["yyxmmc"].ToString();         // 医院收费项目 //*对应玉山医院项目名称
                        decimal dj = Convert.ToDecimal(dr["dj"]);        // 单价  //*
                        decimal sl = Convert.ToDecimal(dr["sl"]);        // 数量  //*
                        decimal je = Convert.ToDecimal(dr["je"]);        // 金额  //*
                        // decimal mcyl = 1;
                        string ysbm = dr["ysdm"].ToString();             // 医生代码
                        string ysxm = dr["ysxm"].ToString();             // 医生姓名
                        string ksdm = dr["ksno"].ToString();             // 科室代码
                        string ksmc = dr["zxks"].ToString();             // 科室名称
                        //string ypjldw = "-";                             // 药品剂量单位
                        string yysj = Convert.ToDateTime(dr["yysj"].ToString()).ToString("yyyy-MM-dd"); //用药时间 //*
                        string xmlx = dr["xmlx"].ToString();     //对应玉山项目类型(新增)
                        string jx = dr["jx"].ToString(); //对应玉山 
                        string gg = dr["gg"].ToString(); //对应玉山
                        string clfs = ""; //对应玉山
                        string sflb = dr["ybsflbdm"].ToString();

                        string ybcfh = cfsj + k.ToString();

                        string sTmp = grbh + "|" + "0" + "|" + ybxmbh + "|" + yyxmmc + "|" + jx + "|" + gg + "|" + xmlx + "|" + sl + "|" + dj + "|" + je + "|"
                                        + "|" + clfs + "|" + "|" + yysj + "|" + yyxmbh + "|" + ybxmmc + "|" + ybcfh + "|" + sflb;
                        li_cfxm.Add(sTmp);
                    }
                }

                if (wdzxms.Length > 0)
                    return new object[] { 0, 0, wdzxms.ToString() };
                #endregion


                //#region 登记前先撤销已上传费用
                //object[] obj1 = new object[] {jzlsh};
                //obj1 = YBZYSFDJCX(obj1);
                //if (obj1[1].ToString() == "0")
                //{
                //    return new object[] { 0, 0, obj1[2].ToString() };
                //}
                //#endregion


                #region 处方明细上传
                List<string> liSQL = new List<string>();
                int iRestult = 0;
                List<string> li_je = new List<string>();
                foreach (string strValue in li_cfxm)
                {
                    //玉山接口参数
                    //zycscf211(Int32 id, Int32 xmbm, string yyxmbm, string xmmc, string jx, string gg, Int32 xmlx, double sl, double dj, double je);
                    string[] s = strValue.Split('|');
                    id = Int32.Parse(s[0]);
                    Int32 xmbm = Int32.Parse(s[1]);
                    string ybxmbm = s[2];
                    string xmmc = s[3];
                    string jx = s[4];
                    string gg = s[5];
                    Int32 xmlx = Int32.Parse(s[6]);
                    double sl = Convert.ToDouble(s[7]);
                    double dj = Convert.ToDouble(s[8]);
                    double je = Convert.ToDouble(s[9]);
                    string clfs = s[10];
                    string yyrq = s[13];
                    string yyxmbm = s[14];
                    string ybxmmc = s[15];
                    string ybcfh = s[16];
                    string sflb = s[17];
                    /*
                     输入参数：个人标识号，医保项目编码（0或－1），医院项目编码（20位省医保编码），项目名称，剂型，规格，项目类型，数量，单价，金额，特殊药品是否按政策内的处理方式，用药日期。
                    特殊药品是否按政策内的处理方式clfs这个标志在以前的版本中没有使用，2016年后开始使用，如果传过来是空的，则由程序自动确定是否为政策内支付，如果所传的药品是限制用药，且传的值是“强制目录内:医师姓名”则此药按目录内处理，如果传的值是“强制目录外:医师姓名”则此药按目录外处理，例如：clfs=’强制目录内:张山’，表示张山医师认定这个限制用药符合目录的限定要求。
                    输出参数：合计医疗费，统筹支付金额，个人帐户余额。
                    函数结果：0或出错代码。
                     */
                    //WriteLog(sysdate + "  处方明细上传|入参|" + id + "|" + xmbm + "|" + ybxmbm + "|" + xmmc + "|" + jx + "|" + gg + "|" + xmlx + "|" + sl + "|" + dj + "|" + je + "|" + clfs + "|" + yyrq + "|");
                    iRestult += zycscf211(id, xmbm, ybxmbm, xmmc, jx, gg, xmlx, sl, dj, je, clfs, yyrq);
                    if (iRestult != 0)
                    {
                        string errMsg = errorMessage();
                        WriteLog(sysdate + "  " + jzlsh + " 住院收费登记(zycscf211)失败|" + errMsg);
                        return new object[] { 0, 0, "住院收费登记失败|" + errMsg };
                    }

                    strSql = string.Format(@"insert into ybcfmxscindr(jzlsh,jylsh,je,xm,kh,yysfxmbm,yysfxmmc,sysdate,ybjzlsh,ybcfh,sfxmzxbm,
                                            ybxmmc,sfxmzl,sflb,dj,sl,yyrq,scsj) values(
                                            '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}',
                                            '{11}','{12}','{13}','{14}','{15}','{16}','{17}')",
                                            jzlsh, jzlsh, je, xm, kh, yyxmbm, xmmc, sysdate, ybjzlsh, ybcfh, ybxmbm,
                                            ybxmmc, xmlx, sflb, dj, sl, yyrq, sysdate);
                    liSQL.Add(strSql);
                    string inParam = iRestult + "|" + id + "|" + xmbm + "|" + yyxmbm + "|" + xmmc + "|" + jx + "|" + gg + "|" + xmlx + "|" + sl + "|" + dj + "|" + je + "|" + clfs + "|" + yyrq;
                    li_je.Add(je.ToString("f4"));
                    WriteLog(sysdate + "   " + jzlsh + "上传处方明细|入参|" + inParam + "\r\n");
                }

                string strMsg = string.Empty; //保存数据返馈错误信息

                double ylf = 0.00; //合计医疗费
                double tczf = 0.00; //统筹支付金额
                double grzhye = 0.00; //个人帐户余额
                //提交接口
                iRestult = zycsjs2(id, ref ylf, ref tczf, ref grzhye);

                if (iRestult != 0)
                {
                    string errMsg = errorMessage();
                    WriteLog(sysdate + "  " + jzlsh + " 进入住院费用明细上传(zycsjs2)失败|" + errMsg);
                    return new object[] { 0, 0, "住院收费登记(zycsjs2)失败|" + errMsg };
                }
                WriteLog(sysdate + "   " + jzlsh + "上传处方明细|出参|" + ylf + "|" + tczf + "|" + grzhye);
                object[] obj = liSQL.ToArray();
                obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);
                if (obj[1].ToString().Equals("1"))
                {
                    WriteLog(sysdate + "   " + jzlsh + "进入住院处方明细上报成功|");
                    return new object[] { 0, 1, "住院处方明细上报成功！"};
                }
                else
                {
                    object[] objParam1 = { jzlsh, grbh };
                    object[] objReturn1 = N_YBZYSFDJCX(objParam1);
                    WriteLog(sysdate + "   " + jzlsh + "进入住院处方明细上报成功|本地数据操作失败|" + obj[2].ToString());
                    return new object[] { 0, 0, "住处方明细上报||数据操作失败！" + obj[2].ToString() };
                }
                #endregion
            }
            catch (Exception ex)
            {
                WriteLog(sysdate + "  住院费用登记(明细上传)异常|" + ex.Message);
                return new object[] { 0, 0, "住院费用登记(明细上传)异常|" + ex.Message };
            }
        }
        #endregion

        #region 住院费用登记撤销
        public static object[] YBZYSFDJCX(object[] objParam)
        {
            string sysdate = GetServerDateTime();
            try
            {
                string jzlsh = objParam[0].ToString(); // 就诊流水号
                string jbr = CliUtils.fUserName;   // 经办人姓名

                if (string.IsNullOrEmpty(jzlsh))
                    return new object[] { 0, 0, "就诊流水号不能为空" };

                #region 是否医保结算
                string strSql = string.Format(@"select * from ybfyjsdr a where a.jzlsh = '{0}'  and a.cxbz = 1", jzlsh);
                DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                if (ds != null && ds.Tables[0].Rows.Count > 0)
                    return new object[] { 0, 0, "已收费结算，不能冲销" };
                #endregion

                #region 是否费用已上传
                strSql = string.Format(@"select * from ybcfmxscindr where jzlsh='{0}' and cxbz=1", jzlsh);
                ds.Tables.Clear();
                ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                if (ds == null || ds.Tables[0].Rows.Count == 0)
                    return new object[] { 0, 0, "无上传费用或已经撤销" };
                #endregion

                #region 是否医保登记
                strSql = string.Format("select * from ybmzzydjdr a where a.jzlsh = '{0}' and a.cxbz = 1", jzlsh);
                ds.Tables.Clear();
                ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                if (ds == null || ds.Tables[0].Rows.Count == 0)
                {
                    return new object[] { 0, 0, "无入院登记记录" };
                }

                DataTable dt = ds.Tables[0];
                DataRow dr = dt.Rows[0];
                string grbh = dr["grbh"].ToString();  // 个人编号
                string xm = dr["xm"].ToString();      // 姓名
                string kh = dr["kh"].ToString();      // 卡号
                string ybjzlsh = dr["ybjzlsh"].ToString();  // 医保就诊流水号
                #endregion

                /*
                 输入参数：个人标识号。
                 函数结果：0或出错代码。
                 将最后一次出院倒回到住院，处方项目已按项目归类合并，如果退过费，退费金额被扣除，但数量和单价不变。
                 */
                WriteLog(sysdate + "   " + jzlsh + "进入住院处方明细撤销...");
                Int32 id = Int32.Parse(grbh);
                WriteLog(sysdate + "  入参|" + id);
                int i = zysccf2(id); //上传处方全部撤销
                List<string> liSQL = new List<string>();

                if (i != 0)
                {
                    string errMsg = errorMessage();
                    WriteLog(sysdate + "   " + jzlsh + "进入住院处方明细撤销失败|" + errMsg);
                    return new object[] { 0, 0, "住院处方明细撤销|失败！" + errMsg };
                }
                strSql = string.Format(@"insert into ybcfmxscindr(jzlsh,jylsh,je,xm,kh,yysfxmbm,yysfxmmc,ybjzlsh,ybcfh,sfxmzxbm,
                                         ybxmmc,sfxmzl,sflb,dj,sl,yyrq,scsj,cxbz,sysdate) select 
                                        jzlsh,jylsh,je,xm,kh,yysfxmbm,yysfxmmc,ybjzlsh,ybcfh,sfxmzxbm,
                                        ybxmmc,sfxmzl,sflb,dj,sl,yyrq,scsj,0,'{1}' from ybcfmxscindr
                                        where jzlsh = '{0}' and cxbz = 1", jzlsh, sysdate);
                liSQL.Add(strSql);
                strSql = string.Format("update ybcfmxscindr set cxbz = 2 where jzlsh = '{0}' and cxbz = 1", jzlsh);
                liSQL.Add(strSql);

                strSql = string.Format("update zy03d set z3ybup = null where z3ybup is not null and z3zyno = '{0}'", jzlsh);
                liSQL.Add(strSql);

                object[] obj = liSQL.ToArray();
                obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);
                if (obj[1].ToString().Equals("1"))
                {
                    WriteLog(sysdate + "   " + jzlsh + "进入住院处方明细撤销成功|");
                    return new object[] { 0, 1, "住院处方明细撤销成功！" };
                }
                else
                {
                    WriteLog(sysdate + "   " + jzlsh + "进入住院处方明细撤销成功|本地数据操作失败|" + obj[2].ToString());
                    return new object[] { 0, 0, "住院处方明细撤销||数据库操作失败!" };
                }
            }
            catch (Exception ex)
            {
                WriteLog(sysdate + "  住院费用登记(明细上传)撤销异常|" + ex.Message);
                return new object[] { 0, 0, "住院费用登记(明细上传)撤销异常|" + ex.Message };
            }
        }
        #endregion

        #region 住院收费预结算
        public static object[] YBZYSFYJS(object[] objParam)
        { 
            string sysdate = GetServerDateTime();
            try
            {
                string jzlsh = objParam[0].ToString();      // 就诊流水号
                string cyyy = objParam[1].ToString();       //出院原因
                string zhsybz = objParam[2].ToString();     //账户使用标志 （0或1）
                string ztjsbz = objParam[3].ToString();     //中途结算标志
                string jsrqsj = objParam[4].ToString();     //结算日期时间
                string cyrqsj = objParam[5].ToString();     //出院日期时间
                string sfje = objParam[6].ToString();       //收费金额
                string djh = "0000";
                if (string.IsNullOrEmpty(jzlsh))
                    return new object[] { 0, 0, "就诊流水号不能为空" };
                if (string.IsNullOrEmpty(cyyy))
                    return new object[] { 0, 0, "出院原因不能为空" };
                if (string.IsNullOrEmpty(sfje))
                    return new object[] { 0, 0, "医疗合计费不能为空" };
                string jbr = CliUtils.fLoginUser;
                //jsrqsj = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                JYLSH = Convert.ToDateTime(jsrqsj).ToString("yyyyMMddHHmmss") + "-" + YLGHBH + "-" + new Random().Next(100).ToString().PadLeft(4, '0');
                WriteLog(sysdate + "  " + jzlsh + " 进入住院收费预结算...");

                #region  是否费用上传
                string strSql = string.Format("select * from ybcfmxscindr a where a.jzlsh = '{0}' and a.cxbz = 1", jzlsh);
                DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                if (ds == null || ds.Tables[0].Rows.Count == 0)
                    return new object[] { 0, 0, "就诊流水号" + jzlsh + "无收费上传记录" };
                string cfmxjylsh = ds.Tables[0].Rows[0]["jylsh"].ToString();
                #endregion

                #region 获取登记信息
                strSql = string.Format(@"select a.*,b.SFYMM,b.LJTCZF,b.LJGRZHZF,b.LJMZF,b.LJZYF,b.zycs from ybmzzydjdr a 
                                    left join YBICKXX b on a.grbh=b.grbh 
                                    where a.jzlsh = '{0}' and a.cxbz = 1", jzlsh);
                ds.Tables.Clear();
                ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                if (ds == null || ds.Tables[0].Rows.Count == 0)
                    return new object[] { 0, 0, "就诊流水号" + jzlsh + "无挂号或入院登记记录" };
                DataRow dr = ds.Tables[0].Rows[0];

                Int32 id = Int32.Parse(dr["grbh"].ToString());
                Int32 zhsybz1 = Int32.Parse(zhsybz);
                Int32 jsfs = 2; //jsfs=2为试结算，即正式结算的程序全部走一遍，但最后回滚而不提交；
                string cyrq1 = cyrqsj;
                double ylfhj = Convert.ToDouble(sfje);
                string yllb = dr["yllb"].ToString();
                string xm = dr["xm"].ToString();
                string kh = dr["kh"].ToString();
                string grbh = dr["grbh"].ToString();
                string zycs = dr["zycs"].ToString();
                //string bntczflj = dr["LJTCZF"].ToString(); //累计统筹支付
                //string bnzhzflj = dr["LJGRZHZF"].ToString();
                //string bndezflj = dr["LJZYF"].ToString();
                #endregion

                double ylf = 0.00; //医疗费合计
                double grzf = 0.00;//个人自费
                double tcjz = 0.00; //统筹记帐
                double ickjz = 0.00; //IC卡划帐
                double zfxj = 0.00; //病人付现金
                double grzhye = 0.00; //个人帐户余额

                WriteLog(sysdate + "   " + jzlsh + "进入住院费用预结算...");
                /*
                 输入参数：个人标识号，是否可用个人帐户支付自费部份，结算方式，出院日期，医疗费合计。
                输出参数：医疗费合计，个人自费，统筹记帐，IC卡划帐，病人付现金，个人帐户余额。
                函数结果：0或出错代码。
                结算时要求IC卡在读写器内。
                结算方式，jsfs=1时出院，jsfs=2为试结算，即正式结算的程序全部走一遍，但最后回滚而不提交；如果处方合计与ylfhj的差别小于1可以出院，否则出错。
                 */
                WriteLog(sysdate + "  入参|" + id + "|" + zhsybz1 + "|" + jsfs + "|" + cyrq1 + "|" + ylfhj + "|");
                int i = zycyjs211(id, zhsybz1, jsfs, cyrq1, ylfhj, ref ylf, ref grzf, ref tcjz, ref ickjz, ref zfxj, ref grzhye);
                if (i != 0)
                {
                    string errMsg = errorMessage();
                    WriteLog(sysdate + "   " + jzlsh + "进入住院费用预结算失败|" + errMsg);
                    return new object[] { 0, 0, "预结算失败" + errMsg };
                }
                WriteLog(sysdate + "  出参|" + ylf + "|" + grzf + "|" + tcjz + "|" + ickjz + "|" + zfxj + "|" + grzhye);
                List<string> liSql = new List<string>();
                
                #region 出参
                outParams_js js = new outParams_js();
                js.Ylfze = ylf.ToString();         //医疗费总费用
                js.Xjzf = zfxj.ToString();         //个人现金支付
                js.Tcjjzf = tcjz.ToString();        //统筹支出
                js.Zhzf = ickjz.ToString();          //本次帐户支付
                js.Zffy = grzf.ToString();      //个人自费
                js.Bcjsqzhye = (grzhye + ickjz).ToString(); //本次结算前帐户余额


                js.Yyfdfy = "0.00";
                js.Cxjfy = "0.00";
                js.Ylzlfy = "0.00";
                js.Jslsh = "0000";
                js.Dejjzf = "0";
                js.Gwybzjjzf = "0";
                js.Qybcylbxjjzf = "0";
                js.Dwfdfy = "0";
                js.Yyfdfy = "0";
                js.Mzjzfy = "0";
                js.Cxjfy = "0";
                js.Ylzlfy = "0";
                js.Blzlfy = "0";
                js.Fhjjylfy = "0";
                js.Qfbzfy = "0";

                /*医疗费总额|总报销金额|统筹基金支付|大额基金支付|账户支付|
                  * 现金支付|公务员补助基金支付|企业补充医疗保险基金支付|自费费用|单位负担费用|
                  * 医院负担费用|民政救助费用|超限价费用|乙类自理费用|丙类自理费用|
                  * 符合基本医疗费用|起付标准费用|转诊转院自付费用|进入统筹费用|统筹分段自付费用|
                  * 超统筹封顶线费用|进入大额报销费用|大额分段自付费用|超大额封顶线费用|人工器官自付费用|
                  * 本次结算前帐户余额|本年统筹支付累计(不含本次)|本年大额支付累计(不含本次)|本年城镇居民门诊统筹支付累计(不含本次)|本年公务员补助支付累计(不含本次)|
                  * 本年账户支付累计(不含本次)|本年住院次数累计(不含本次)|住院次数|姓名|结算时间|
                  * 医疗类别|医疗待遇类别|经办机构编码|业务周期号|结算流水号|
                  * 提示信息|单据号|交易类型|医院交易流水号|有效标志|
                  * 个人编号管理|医疗机构编码|二次补偿金额|门慢起付累计|接收方交易流水号|
                  * 个人编号|单病种补差|财政支出金额|二类门慢限额支出|二类门慢限额剩余|
                  * 居民个人自付二次补偿金额|体检金额|生育基金支付|其他医保支付|
                  */
                //总报销金额=医疗总费用-本次现金支付
                js.Zbxje = (ylf - zfxj).ToString(); //总报销金额
                //现金支付=本次现金支付
                js.Ybxjzf = js.Xjzf;
                //其他医保支付=医疗总费用-本次现金支付-本次统筹支付-本次账户支付
                double je = ylf - zfxj - tcjz - ickjz;
                if((je>=-0.0001)&&(je<=0.00001))
                    js.Qtybzf = "0";
                else
                    js.Qtybzf = (ylf - zfxj - tcjz - ickjz).ToString(); //其他医保支付
                string strValue = js.Ylfze + "|" + js.Zbxje + "|" + js.Tcjjzf + "|" + js.Dejjzf + "|" + js.Zhzf + "|" +
                                   js.Ybxjzf + "|" + js.Gwybzjjzf + "|" + js.Qybcylbxjjzf + "|" + js.Zffy + "|" + js.Dwfdfy + "|" +
                                   js.Yyfdfy + "|" + js.Mzjzfy + "|" + js.Cxjfy + "|" + js.Ylzlfy + "|" + js.Blzlfy + "|" +
                                   js.Fhjjylfy + "|" + js.Qfbzfy + "|" + js.Zzzyzffy + "|" + js.Jrtcfy + "|" + js.Tcfdzffy + "|" +
                                   js.Ctcfdxfy + "|" + js.Jrdebxfy + "|" + js.Defdzffy + "|" + js.Cdefdxfy + "|" + js.Rgqgzffy + "|" +
                                   js.Bcjsqzhye + "|" + js.Bntczflj + "|" + js.Bndezflj + "|" + js.Bnczjmmztczflj + "|" + js.Bngwybzzflj + "|" +
                                   js.Bnzhzflj + "|" + js.Bnzycslj + "|" + js.Zycs + "|" + js.Xm + "|" + js.Jsrq + js.Jssj + "|" +
                                   js.Yllb + "|" + js.Yldylb + "|" + js.Jbjgbm + "|" + js.Ywzqh + "|" + js.Jslsh + "|" +
                                   js.Tsxx + "|" + js.Djh + "|" + js.Jyxl + "|" + js.Yyjylsh + "|" + js.Yxbz + "|" +
                                   js.Grbhgl + "|" + js.Yljgbm + "|" + js.Ecbcje + "|" + js.Mmqflj + "|" + js.Jsfjylsh + "|" +
                                   js.Grbh + "|" + js.Dbzbc + "|" + js.Czzcje + "|" + js.Elmmxezc + "|" + js.Elmmxesy + "|" +
                                   js.Jmgrzfecbcje + "|" + js.Tjje + "|" + js.Syjjzf + "|" + js.Qtybzf + "|";
                #endregion

                strSql = string.Format(@"delete from ybfyyjsdr where jzlsh='{0}' and cxbz=1", jzlsh);
                liSql.Add(strSql);
                strSql = string.Format(@"insert into ybfyyjsdr(
                                        jzlsh, jylsh ,ylfze ,xjzf ,zhzf ,tcjjzf, dejjzf,gwybzjjzf,qybcylbxjjzf,ylzlfy,
                                        fhjbylfy,qfbzfy,jrtcfy,ctcfdxfy,jrdebxfy,defdzffy,zzzyzffy,zffy,rgqgzffy,mzjzfy,
                                        dwfdfy, yllb,xm,grbh,kh,cfmxjylsh,ywzqh,jslsh,jsrq,djh,
                                        bcjsqzhye,jbr,sysdate,qtybfy,zhxjzffy)
                                        values('{0}','{1}','{2}','{3}','{4}','{5}' ,'{6}','{7}','{8}' ,'{9}' ,
                                        '{10}','{11}' ,'{12}','{13}' ,'{14}','{15}','{16}', '{17}','{18}','{19}',
                                        '{20}' ,'{21}','{22}','{23}','{24}','{25}','{26}', '{27}' ,'{28}','{29}',
                                        '{30}','{31}','{32}','{34}','{34}')",
                                        jzlsh, JYLSH, js.Ylfze, js.Xjzf, js.Zhzf, js.Tcjjzf, "0.00", "0.00", "0.00", "0.00",
                                        "0.00", "0.00", "0.00", "0.00", "0.00", "0.00", "0.00", js.Zffy, "0.00", "0.00",
                                        "0.00", yllb, xm, grbh, kh, cfmxjylsh, "", djh, jsrqsj, djh,
                                        grzhye, jbr, sysdate, js.Qtybzf, js.Ybxjzf);
                liSql.Add(strSql);
                object[] obj = liSql.ToArray();
                obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);
                if (obj[1].ToString().Equals("1"))
                {
                    WriteLog(sysdate + "   " + jzlsh + "住院费用预结算成功|" + strValue);
                    return new object[] { 0, 1, strValue };
                }
                else
                {
                    WriteLog(sysdate + "   " + jzlsh + "住院费用预结算失败|" + obj[2].ToString());
                    return new object[] { 0, 0, "住院费用预结算失败|" + obj[2].ToString() };
                }
            }
            catch (Exception ex)
            {
                WriteLog(sysdate + "  住院收费预结算异常|" + ex.Message);
                return new object[] { 0, 0, "住院收费预结算异常|" + ex.Message };
            }
        }
        #endregion

        #region 住院收费结算
        public static object[] YBZYSFJS(object[] objParam)
        {
            string sysdate = GetServerDateTime();
            try
            {
                string jzlsh = objParam[0].ToString();      // 就诊流水号
                string djh = objParam[1].ToString();        //单据号（发票号）
                string cyyy = objParam[2].ToString();       //出院原因
                string zhsybz = objParam[3].ToString();     //账户使用标志（0或1）
                string ztjsbz = objParam[4].ToString();     //中途结算标志
                string jsrqsj = objParam[5].ToString();     //结算日期时间
                string cyrqsj = objParam[6].ToString();     //出院日期时间
                string sfje = objParam[7].ToString();       //收费金额
                string cfmxjylsh = "";
                string jbr = CliUtils.fLoginUser;

                if (string.IsNullOrEmpty(jzlsh))
                    return new object[] { 0, 0, "就诊流水号不能为空" };
                if (string.IsNullOrEmpty(djh))
                    return new object[] { 0, 0, "单据号不能为空" };
                if (string.IsNullOrEmpty(sfje))
                    return new object[] { 0, 0, "医疗合计费不能为空" };

                JYLSH = Convert.ToDateTime(jsrqsj).ToString("yyyyMMddHHmmss") + "-" + YLGHBH + "-" + new Random().Next(100).ToString().PadLeft(4, '0');

                #region 获取登记信息
                string strSql = string.Format(@"select a.*,b.SFYMM,b.LJTCZF,b.LJGRZHZF,b.LJMZF,b.LJZYF,b.zycs from ybmzzydjdr a 
                                    left join YBICKXX b on a.grbh=b.grbh 
                                    where a.jzlsh = '{0}' and a.cxbz = 1", jzlsh);
                DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                if (ds == null || ds.Tables[0].Rows.Count == 0)
                    return new object[] { 0, 0, "就诊流水号" + jzlsh + "无挂号或入院登记记录" };
                DataRow dr = ds.Tables[0].Rows[0];

                Int32 id = Int32.Parse(dr["grbh"].ToString());
                Int32 zhsybz1 = Int32.Parse(zhsybz);
                Int32 jsfs = 1; //jsfs=2为试结算，即正式结算的程序全部走一遍，但最后回滚而不提交；
                string cyrq1 = cyrqsj;
                double ylfhj = Convert.ToDouble(sfje);
                string yllb = dr["yllb"].ToString();
                string xm = dr["xm"].ToString();
                string kh = dr["kh"].ToString();
                string grbh = dr["grbh"].ToString();
                string zycs = dr["zycs"].ToString();
                string ybjzlsh = dr["ybjzlsh"].ToString();
                //string bntczflj = dr["LJTCZF"].ToString(); //累计统筹支付
                //string bnzhzflj = dr["LJGRZHZF"].ToString();
                //string bndezflj = dr["LJZYF"].ToString();
                #endregion

                double ylf = 0.00; //医疗费合计
                double grzf = 0.00;//个人自费
                double tcjz = 0.00; //统筹记帐
                double ickjz = 0.00; //IC卡划帐
                double zfxj = 0.00; //病人付现金
                double grzhye = 0.00; //个人帐户余额

                WriteLog(sysdate + "   " + jzlsh + "进入住院费用结算...");
                /*
                 输入参数：个人标识号，是否可用个人帐户支付自费部份，结算方式，出院日期，医疗费合计。
                输出参数：医疗费合计，个人自费，统筹记帐，IC卡划帐，病人付现金，个人帐户余额。
                函数结果：0或出错代码。
                结算时要求IC卡在读写器内。
                结算方式，jsfs=1时出院，jsfs=2为试结算，即正式结算的程序全部走一遍，但最后回滚而不提交；如果处方合计与ylfhj的差别小于1可以出院，否则出错。
                 */
                WriteLog(sysdate + "  入参|" + id + "|" + zhsybz1 + "|" + jsfs + "|" + cyrq1 + "|" + ylfhj + "|");
                int i = zycyjs211(id, zhsybz1, jsfs, cyrq1, ylfhj, ref ylf, ref grzf, ref tcjz, ref ickjz, ref zfxj, ref grzhye);
                if (i != 0)
                {
                    string errMsg = errorMessage();
                    WriteLog(sysdate + "   " + jzlsh + "进入住院费用预结算失败|" + errMsg);
                    return new object[] { 0, 0, "预结算失败" + errMsg };
                }
                WriteLog(sysdate + "  出参|" + ylf + "|" + grzf + "|" + tcjz + "|" + ickjz + "|" + zfxj + "|" + grzhye);
                

                #region 出参
                outParams_js js = new outParams_js();
                js.Ylfze = ylf.ToString();         //医疗费总费用
                js.Xjzf = zfxj.ToString();         //个人现金支付
                js.Tcjjzf = tcjz.ToString();        //统筹支出
                js.Zhzf = ickjz.ToString();          //本次帐户支付
                js.Zffy = grzf.ToString();      //个人自费
                js.Bcjsqzhye = (grzhye + ickjz).ToString(); //本次结算前帐户余额

                js.Yyfdfy = "0.00";
                js.Cxjfy = "0.00";
                js.Ylzlfy = "0.00";
                js.Jslsh = "0000";
                js.Dejjzf = "0";
                js.Gwybzjjzf = "0";
                js.Qybcylbxjjzf = "0";
                js.Dwfdfy = "0";
                js.Yyfdfy = "0";
                js.Mzjzfy = "0";
                js.Cxjfy = "0";
                js.Ylzlfy = "0";
                js.Blzlfy = "0";
                js.Fhjjylfy = "0";
                js.Qfbzfy = "0";

                /*医疗费总额|总报销金额|统筹基金支付|大额基金支付|账户支付|
                  * 现金支付|公务员补助基金支付|企业补充医疗保险基金支付|自费费用|单位负担费用|
                  * 医院负担费用|民政救助费用|超限价费用|乙类自理费用|丙类自理费用|
                  * 符合基本医疗费用|起付标准费用|转诊转院自付费用|进入统筹费用|统筹分段自付费用|
                  * 超统筹封顶线费用|进入大额报销费用|大额分段自付费用|超大额封顶线费用|人工器官自付费用|
                  * 本次结算前帐户余额|本年统筹支付累计(不含本次)|本年大额支付累计(不含本次)|本年城镇居民门诊统筹支付累计(不含本次)|本年公务员补助支付累计(不含本次)|
                  * 本年账户支付累计(不含本次)|本年住院次数累计(不含本次)|住院次数|姓名|结算时间|
                  * 医疗类别|医疗待遇类别|经办机构编码|业务周期号|结算流水号|
                  * 提示信息|单据号|交易类型|医院交易流水号|有效标志|
                  * 个人编号管理|医疗机构编码|二次补偿金额|门慢起付累计|接收方交易流水号|
                  * 个人编号|单病种补差|财政支出金额|二类门慢限额支出|二类门慢限额剩余|
                  * 居民个人自付二次补偿金额|体检金额|生育基金支付|其他医保支付|
                  */
                //总报销金额=医疗总费用-本次现金支付
                js.Zbxje = (ylf - zfxj).ToString(); //总报销金额
                //现金支付=本次现金支付
                js.Ybxjzf = js.Xjzf;
                //其他医保支付=医疗总费用-本次现金支付-本次统筹支付-本次账户支付
                double je = ylf - zfxj - tcjz - ickjz;
                if ((je >= -0.0001) && (je <= 0.00001))
                    js.Qtybzf = "0";
                else
                    js.Qtybzf = (ylf - zfxj - tcjz - ickjz).ToString(); //其他医保支付
                string strValue = js.Ylfze + "|" + js.Zbxje + "|" + js.Tcjjzf + "|" + js.Dejjzf + "|" + js.Zhzf + "|" +
                                   js.Ybxjzf + "|" + js.Gwybzjjzf + "|" + js.Qybcylbxjjzf + "|" + js.Zffy + "|" + js.Dwfdfy + "|" +
                                   js.Yyfdfy + "|" + js.Mzjzfy + "|" + js.Cxjfy + "|" + js.Ylzlfy + "|" + js.Blzlfy + "|" +
                                   js.Fhjjylfy + "|" + js.Qfbzfy + "|" + js.Zzzyzffy + "|" + js.Jrtcfy + "|" + js.Tcfdzffy + "|" +
                                   js.Ctcfdxfy + "|" + js.Jrdebxfy + "|" + js.Defdzffy + "|" + js.Cdefdxfy + "|" + js.Rgqgzffy + "|" +
                                   js.Bcjsqzhye + "|" + js.Bntczflj + "|" + js.Bndezflj + "|" + js.Bnczjmmztczflj + "|" + js.Bngwybzzflj + "|" +
                                   js.Bnzhzflj + "|" + js.Bnzycslj + "|" + js.Zycs + "|" + js.Xm + "|" + js.Jsrq + js.Jssj + "|" +
                                   js.Yllb + "|" + js.Yldylb + "|" + js.Jbjgbm + "|" + js.Ywzqh + "|" + js.Jslsh + "|" +
                                   js.Tsxx + "|" + js.Djh + "|" + js.Jyxl + "|" + js.Yyjylsh + "|" + js.Yxbz + "|" +
                                   js.Grbhgl + "|" + js.Yljgbm + "|" + js.Ecbcje + "|" + js.Mmqflj + "|" + js.Jsfjylsh + "|" +
                                   js.Grbh + "|" + js.Dbzbc + "|" + js.Czzcje + "|" + js.Elmmxezc + "|" + js.Elmmxesy + "|" +
                                   js.Jmgrzfecbcje + "|" + js.Tjje + "|" + js.Syjjzf + "|" + js.Qtybzf + "|";
                #endregion

                List<string> liSql = new List<string>();
                strSql = string.Format(@"insert into ybfyjsdr(
                                        jzlsh, jylsh ,ylfze ,xjzf ,zhzf ,tcjjzf, dejjzf,gwybzjjzf,qybcylbxjjzf,ylzlfy,
                                        fhjbylfy,qfbzfy,jrtcfy,ctcfdxfy,jrdebxfy,defdzffy,zzzyzffy,zffy,rgqgzffy,mzjzfy,
                                        dwfdfy, yllb,xm,grbh,kh,cfmxjylsh,ywzqh,jslsh,jsrq,djh,
                                        bcjsqzhye,jbr,sysdate,qtybfy,zhxjzffy,ybjzlsh,djhin)
                                        values('{0}','{1}','{2}','{3}','{4}','{5}' ,'{6}','{7}','{8}' ,'{9}' ,
                                        '{10}','{11}' ,'{12}','{13}' ,'{14}','{15}','{16}', '{17}','{18}','{19}',
                                        '{20}' ,'{21}','{22}','{23}','{24}','{25}','{26}', '{27}' ,'{28}','{29}',
                                        '{30}','{31}','{32}','{33}','{34}','{35}','{36}')",
                                        jzlsh, JYLSH, js.Ylfze, js.Xjzf, js.Zhzf, js.Tcjjzf, "0.00", "0.00", "0.00", "0.00",
                                        "0.00", "0.00", "0.00", "0.00", "0.00", "0.00", "0.00", js.Zffy, "0.00", "0.00",
                                        "0.00", yllb, xm, grbh, kh, cfmxjylsh, "", djh, jsrqsj, djh,
                                        js.Bcjsqzhye, jbr, sysdate, js.Qtybzf, js.Ybxjzf, ybjzlsh,djh);
                liSql.Add(strSql);

                object[] obj = liSql.ToArray();
                obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);
                if (obj[1].ToString().Equals("1"))
                {
                    WriteLog(sysdate + "   " + jzlsh + "进入住院费用结算成功|" + strValue);
                    //打印医保结算单
                    //object[] objParam2 = { jzlsh, grbh };
                    //YBZYJSD(objParam2);
                    return new object[] { 0, 1, strValue };
                }
                else
                {
                    object[] objParam2 = { jzlsh, grbh };
                    object[] objReturn2 = N_YBZYSFJSCX(objParam2);
                    WriteLog(sysdate + "   " + jzlsh + "进入住院费用结算成功|本地操作失败|" + obj[2].ToString());
                    return new object[] { 0, 0, "住院费用结算|数据库操作失败！" + obj[2].ToString() };
                }
            }
            catch (Exception ex)
            {
                WriteLog(sysdate + "  住院收费结算异常|" + ex.Message);
                return new object[] { 0, 0, "住院收费结算异常|" + ex.Message };
            }
        }
        #endregion

        #region  住院收费结算撤销
        public static object[] YBZYSFJSCX(object[] objParam)
        {
            string sysdate = GetServerDateTime();
            try
            {
                string jbr = CliUtils.fUserName;  //经办人
                string jzlsh = objParam[0].ToString();   // 就诊流水号
                string djh = objParam[1].ToString();     // 结算单据号

                if (string.IsNullOrEmpty(jzlsh))
                    return new object[] { 0, 0, "就诊流水号不能为空" };
                if (string.IsNullOrEmpty(djh))
                    return new object[] { 0, 0, "发票号不能为空" };
                JYLSH = DateTime.Now.ToString("yyyyMMddHHmmss") + "-" + YLGHBH + "-" + new Random().Next(100).ToString().PadLeft(4, '0');

                #region 是否医保结算
                string strSql = string.Format(@"select a.* from ybfyjsdr a 
                                            where a.jzlsh = '{0}' and a.djh = '{1}' and a.cxbz = 1   ", jzlsh, djh);
                DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                if (ds.Tables[0].Rows.Count == 0)
                    return new object[] { 0, 1, "该患者未进行医保结算或结算已撤销" };

                DataRow dr = ds.Tables[0].Rows[0];
                string ybjzlsh = dr["ybjzlsh"].ToString();    //"1100008927"
                string xm = dr["xm"].ToString();          // // "徐国强""施美荣";
                string kh = dr["kh"].ToString();          // ;"520313262"//"469285189";
                string grbh = dr["grbh"].ToString();      //"360402837288" ;//"360403700513";
                string jsrq = dr["jsrq"].ToString();      //"20160308180140" ;//"20160129000000";
                //string cfmxjylsh = dr["cfmxjylsh"].ToString();// //"处方明细交易流水号";
                string jslsh = dr["jslsh"].ToString();    //结算流水号  曹晓红  20160818
                Int32 id = Int32.Parse(grbh);
                #endregion

                //入参
                WriteLog(sysdate + "   " + jzlsh + "进入住院费用结算撤销...");
                /*
                 输入参数：个人标识号。
                函数结果：0或出错代码。
                将最后一次出院倒回到住院，处方项目已按项目归类合并，如果退过费，退费金额被扣除，但数量和单价不变。
                 */
                WriteLog(sysdate + "  入参|" + id + "|" );
                List<string> liSQL = new List<string>();
                int i = zyqxcy(id);
                if (i != 0)
                {
                    string errMsg = errorMessage();
                    WriteLog(sysdate + "   " + jzlsh + "住院费用结算撤销失败|" + errMsg);
                    return new object[] { 0, 0, "住院费用结算撤销失败|" + errMsg };
                }

                strSql = string.Format(@"insert into ybfyjsdr(
                                                    jzlsh, jylsh ,ylfze ,xjzf ,zhzf ,tcjjzf, dejjzf,gwybzjjzf,qybcylbxjjzf,ylzlfy,
                                                    fhjbylfy,qfbzfy,jrtcfy,ctcfdxfy,jrdebxfy,defdzffy,zzzyzffy,zffy,rgqgzffy,mzjzfy,
                                                    dwfdfy, yllb,xm,grbh,kh,cfmxjylsh,ywzqh,jslsh,jsrq,djh,djhin,
                                                    bcjsqzhye,jbr,cxbz,sysdate) 
                                                    select 
                                                    jzlsh, jylsh ,ylfze ,xjzf ,zhzf ,tcjjzf, dejjzf,gwybzjjzf,qybcylbxjjzf,ylzlfy,
                                                    fhjbylfy,qfbzfy,jrtcfy,ctcfdxfy,jrdebxfy,defdzffy,zzzyzffy,zffy,rgqgzffy,mzjzfy,
                                                    dwfdfy, yllb,xm,grbh,kh,cfmxjylsh,ywzqh,jslsh,jsrq,djh,djhin,
                                                    bcjsqzhye,jbr,0,'{2}' from ybfyjsdr where jzlsh = '{0}' and djh = '{1}' and cxbz = 1", jzlsh, djh, sysdate);
                liSQL.Add(strSql);
                strSql = string.Format(@"update ybfyjsdr set cxbz = 2 where jzlsh = '{0}' and djh = '{1}' and cxbz = 1", jzlsh, djh);
                liSQL.Add(strSql);
                 object[] obj = liSQL.ToArray();
                    obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);
                    if (obj[1].ToString().Equals("1"))
                    {
                        WriteLog(sysdate + "   " + jzlsh + "进入住院费用结算撤销成功|");
                        #region 费用明细撤销
                        YBZYSFDJCX(new object[] { jzlsh });
                        #endregion
                        return new object[] { 0, 1, "住院费用结算撤销成功" };
                    }
                    else
                    {
                        WriteLog(sysdate + "   " + jzlsh + "住院费用结算撤销成功|本地数据操作失败|" + obj[2].ToString());
                        return new object[] { 0, 0, "住院费用结算撤销失败|数据库操作失败|" + obj[2].ToString() };
                    }
            }
            catch (Exception ex)
            {
                WriteLog(sysdate + "  住院收费结算撤销异常|" + ex.Message);
                return new object[] { 0, 0, "住院收费结算撤销异常|" + ex.Message };
            }
        }
        #endregion

        #region 住院收费结算单打印
        public static object[] YBZYJSD(object[] objParam)
        {
            string sysdate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");//当前时间
            try
            {
                string jzlsh = objParam[0].ToString();
                string djh = objParam[1].ToString();

                if (string.IsNullOrEmpty(jzlsh))
                    return new object[] { 0, 0, "就诊流水号不能为空" };
                if (string.IsNullOrEmpty(djh))
                    return new object[] { 0, 0, "结算单据号不能为空" };
                #region 是否医保结算
                string strSql = string.Format(@"select * from ybfyjsdr where jzlsh='{0}' and djh='{1}' and cxbz=1", jzlsh, djh);
                DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                if (ds.Tables[0].Rows.Count == 0)
                    return new object[] { 0, 0, "无患者结算信息" };
                string grbh = ds.Tables[0].Rows[0]["grbh"].ToString();
                string ybjzlsh = ds.Tables[0].Rows[0]["ybjzlsh"].ToString();
                Int32 id = int.Parse(grbh);
                string zyh = ybjzlsh;
                #endregion

                WriteLog(sysdate + "   " + jzlsh + "进入打印出院结算单...");
                WriteLog(sysdate + "  入参|" + grbh + "|" + zyh);

                int i = zydyjsd2(id, zyh);
                if (i == 0)
                {
                    WriteLog(sysdate + "   " + jzlsh + "打印出院结算单成功|");
                    return new object[] { 0, 1, "打印出院结算单成功！" };
                }
                else
                {
                    string errMsg = errorMessage();
                    WriteLog(sysdate + "   " + jzlsh + "打印出院结算单失败|" + errMsg);
                    return new object[] { 0, 0, "打印出院结算单出院|失败！" + errMsg };
                }
            }
            catch (Exception ex)
            {
                WriteLog(sysdate + "  住院结算单打印异常|" + ex.Message);
                return new object[] { 0, 0, "住院结算单打印异常|" + ex.Message };
            }
        }
        #endregion

        #region 门诊费用结算撤销(内部)
        internal static object[] N_YBMZSFJSCX(object[] objParam)
        {
            string sysdate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");//当前时间
            string grbh = objParam[0].ToString();
            string yylf1 = objParam[1].ToString();
            string tfje1 = objParam[2].ToString();
            string jzlsh = objParam[3].ToString();

            Int32 id = Convert.ToInt32(grbh);
            double yylf = Convert.ToDouble(yylf1);
            double tfje = Convert.ToDouble(tfje1);
            string mzh = jzlsh;
            double txj = 0.00;
            double tgrzh = 0.00;
            double grzhye = 0.00;
            double ttcj = 0.00;
            /*
                输入参数：个人标识号，原医疗费金额，退费金额
                输出参数：退还现金额，退回个人帐户金额，个人帐户余额，退回统筹金
             */
            int i = mztf211(id, yylf, tfje, mzh, ref txj, ref tgrzh, ref grzhye, ref ttcj);
            if (i == 0)
            {
                WriteLog(sysdate + "   " + jzlsh + "进入门诊费用结算撤销成功(内部)|" + txj + "|" + tgrzh + "|" + grzhye + "|" + ttcj);
                return new object[] { 0, 1, "门诊费用结算撤销成功（内部）！" };
            }
            else
            {
                string errMsg = errorMessage();
                WriteLog(sysdate + "   " + jzlsh + "进入门诊费用结算撤销失败(内部)|" + errMsg);
                return new object[] { 0, 0, "门诊费用结算撤销失败（内部）！" + errMsg };
            }
        }
        #endregion

        #region 住院登记撤销(内部)
        internal static object[] N_YBZYDJCX(object[] objParam)
        {
            string sysdate = GetServerDateTime();//当前时间
            string grbh = objParam[0].ToString();  // 人个编号
            string jzlsh = objParam[1].ToString();  // 就诊流水号
            string ybjzlsh = objParam[2].ToString();  //医保就诊流水号

            //玉山参数
            WriteLog(sysdate + "   " + jzlsh + "进入住院登记撤销(内部)...");
            Int32 id = Int32.Parse(grbh);
            string zyh = ybjzlsh;

            WriteLog(sysdate + "  入参|" + id + "|" + zyh + "|");
            int i = zycxdj2(id, zyh);
            if (i == 0)
            {
                WriteLog(sysdate + "   " + jzlsh + "进入住院登记撤销(内部)成功|");
                return new object[] { 0, 1, "住院登记撤销(内部)成功!" };
            }
            else
            {
                string errMsg = errorMessage();
                WriteLog(sysdate + "   " + jzlsh + "进入住院登记撤销(内部)失败|" + errMsg);
                return new object[] { 0, 0, "住院登记撤销(内部)失败！" + errMsg };
            }
        }
        #endregion

        #region 住院费用登记撤销(内部)
        internal static object[] N_YBZYSFDJCX(object[] objParam)
        {
            string sysdate = GetServerDateTime();//当前时间
            string jzlsh = objParam[0].ToString();
            string grbh = objParam[1].ToString();

            WriteLog(sysdate + "   " + jzlsh + "进入住院处方明细撤销(内部)...");
            Int32 id = Int32.Parse(grbh);
            WriteLog(sysdate + "  入参|" + id);
            int i = zysccf2(id); //上传处方全部撤销
            if (i == 0)
            {
                WriteLog(sysdate + "   " + jzlsh + "住院处方明细撤销(内部)成功|");
                return new object[] { 0, 1, "住院处方明细撤销(内部)成功!" };
            }
            else
            {
                string errMsg = errorMessage();
                WriteLog(sysdate + "   " + jzlsh + "住院处方明细撤销(内部)失败|" + errMsg);
                return new object[] { 0, 0, "住院处方明细撤销(内部)失败！" + errMsg };
            }
        }
        #endregion

        #region  住院收费结算撤销(内部)
        public static object[] N_YBZYSFJSCX(object[] objParam)
        {
            string sysdate = GetServerDateTime();
            string jzlsh = objParam[0].ToString();   // 就诊流水号
            string grbh = objParam[1].ToString();     // 个人编号

            Int32 id = Int32.Parse(grbh);
            //入参
            WriteLog(sysdate + "   " + jzlsh + "进入住院费用结算撤销(内部)...");
            /*
             输入参数：个人标识号。
            函数结果：0或出错代码。
            将最后一次出院倒回到住院，处方项目已按项目归类合并，如果退过费，退费金额被扣除，但数量和单价不变。
             */
            WriteLog(sysdate + "  入参|" + id + "|");
            int i = zyqxcy(id);
            if (i == 0)
            {
                WriteLog(sysdate + "   " + jzlsh + "住院费用结算撤销(内部)成功|");
                #region 费用明细撤销
                YBZYSFDJCX(new object[] { jzlsh });
                #endregion
                return new object[] { 0, 1, "住院费用结算撤销(内部)成功!" };
            }
            else
            {
                string errMsg = errorMessage();
                WriteLog(sysdate + "   " + jzlsh + "住院费用结算撤销(内部)失败|" + errMsg);
                return new object[] { 0, 0, "住院费用结算撤销(内部)失败！" + errMsg };
            }
        }
        #endregion

        #endregion

        #region 取出服务器日期时间
        private static string GetServerDateTime()
        {
            object[] myDateTime = CliUtils.CallMethod("GLModule", "GetServerTime", new object[] { });
            //return (string)myDateTime[3];    //yyyy-MM-dd  
            //return (string)myDateTime[4];   // hh:mm:ss   
            return (string)myDateTime[5];     //yyyy-MM-dd HH:mm:ss
        }
        #endregion

        #region 日志
        public static void WriteLog(string str)
        {
            if (!Directory.Exists("YBLog"))
            {
                Directory.CreateDirectory("YBLog");
            }
            FileStream stream = new FileStream("YBLog\\YBLog" + DateTime.Now.ToString("yyyyMMdd") + ".txt", FileMode.Append, FileAccess.Write);
            StreamWriter writer = new StreamWriter(stream);
            writer.WriteLine(str);
            writer.Close();
            stream.Close();
        }

        public static void WriteLogFile(string fileName, string data)
        {
            StreamWriter sw = new StreamWriter(fileName, false);
            sw.WriteLine(data);
            sw.Close();
        }
        #endregion
    }
}
