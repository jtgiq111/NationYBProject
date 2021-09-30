using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Srvtools;
using System.Runtime.InteropServices;
using System.Net.NetworkInformation;
using System.IO;
using System.Data;
using System.Windows.Forms;

//合肥地区医保接口文件
namespace ybinterface_lib
{
    public class yb_interface_hf
    {
        #region 接口DLL 加载
        //初始化
        [DllImport("sieaf.dll", EntryPoint = "init", CallingConvention = CallingConvention.StdCall)]
        static extern IntPtr init(string addr, int port, string servletEntry);
        //以"name=value"的格式装入参数数据
        [DllImport("sieaf.dll", EntryPoint = "putPara", CallingConvention = CallingConvention.StdCall)]
        static extern bool putPara(IntPtr whandler, string name, string value);
        //指定后续装入数据所在的记录集名
        [DllImport("sieaf.dll", EntryPoint = "startResultSetName", CallingConvention = CallingConvention.StdCall)]
        static extern bool startResultSetName(IntPtr whandler, string name);
        //结束当前的记录集
        [DllImport("sieaf.dll", EntryPoint = "endCurResultSet", CallingConvention = CallingConvention.StdCall)]
        static extern bool endCurResultSet(IntPtr whandler);
        //结束当前的行数据
        [DllImport("sieaf.dll", EntryPoint = "endcurRow", CallingConvention = CallingConvention.StdCall)]
        static extern bool endcurRow(IntPtr whandler);
        //在当前记录集的当前行，以"name=value"的格式装入数据
        [DllImport("sieaf.dll", EntryPoint = "putColData", CallingConvention = CallingConvention.StdCall)]
        static extern bool putColData(IntPtr whandler, string name, string value);
        //调用应用服务器端处理，参数funcID为业务功能标识
        [DllImport("sieaf.dll", EntryPoint = "process", CallingConvention = CallingConvention.StdCall)]
        static extern bool process(IntPtr whandler, string funcID);
        //按名字取得返回的参数数据
        [DllImport("sieaf.dll", EntryPoint = "getParaByName", CallingConvention = CallingConvention.StdCall)]
        static extern bool getParaByName(IntPtr whandler, string name, byte[] value);
        //按名字取得返回的参数数据
        [DllImport("sieaf.dll", EntryPoint = "getParaByNameBlob", CallingConvention = CallingConvention.StdCall)]
        static extern bool getParaByNameBlob(IntPtr whandler, string name, [MarshalAs(UnmanagedType.LPArray)] byte[] value);
        //返回服务器端的信息,如果是OK,表示操作成功！
        [DllImport("sieaf.dll", EntryPoint = "getErrMsg", CallingConvention = CallingConvention.StdCall)]
        static extern bool getErrMsg(IntPtr whandler, [MarshalAs(UnmanagedType.LPArray)] byte[] value);
        //按名字得到记录集，并返回行的数目;如果失败，可以用getFailReason获取失败原因
        [DllImport("sieaf.dll", EntryPoint = "toResultSetName", CallingConvention = CallingConvention.StdCall)]
        static extern int toResultSetName(IntPtr whandler, string name);
        //将光标移到下一行
        [DllImport("sieaf.dll", EntryPoint = "nextRow", CallingConvention = CallingConvention.StdCall)]
        static extern int nextRow(IntPtr whandler);
        //按名字得到当前光标指定行的数据;如果失败，可用getFailReason获取失败原因
        [DllImport("sieaf.dll", EntryPoint = "getColData", CallingConvention = CallingConvention.StdCall)]
        static extern bool getColData(IntPtr whandler, string name, [MarshalAs(UnmanagedType.LPArray)] byte[] value);
        //释放接口
        [DllImport("sieaf.dll", EntryPoint = "destroyInterface", CallingConvention = CallingConvention.StdCall)]
        static extern bool destroyInterface(IntPtr whandler);
        //得到具体的错误信息
        [DllImport("sieaf.dll", EntryPoint = "getFailReason", CallingConvention = CallingConvention.StdCall)]
        static extern bool getFailReason(IntPtr whandler, [MarshalAs(UnmanagedType.LPArray)] byte[] value);
        //
        [DllImport("sieaf.dll", EntryPoint = "base64Encode", CallingConvention = CallingConvention.StdCall)]
        static extern bool base64Encode(StringBuilder sData, long sLen, ref StringBuilder rData, ref long rLen);
        //
        [DllImport("sieaf.dll", EntryPoint = "base64Decode", CallingConvention = CallingConvention.StdCall)]
        static extern bool base64Decode(StringBuilder sData, long sLen, ref StringBuilder rData, ref long rLen);
        #endregion

        #region 变量
        static IWork iWork = new Work();
        static string xmlPath = AppDomain.CurrentDomain.BaseDirectory;
        static List<Item1> lItem = iWork.getXmlConfig1(xmlPath + "EEPNetClient.exe.config");
        internal static string YBIP = lItem[0].YBIP;        //医保IP地址
        internal static string YBPORT = "8001";        //医保IP端口
        internal static string YBURL = "/steaf/MainServlet";
        internal static IntPtr whandler;
        internal static string ybuserid = string.Empty; //医保帐户
        internal static string ybpasswd = string.Empty; //医保密码
        #endregion

        #region 接口业务调用方法
        #region 初始化
        public static object[] YBINIT(object[] objParam)
        {
            //Ping医保网
            Ping ping = new Ping();
            PingReply pingReply = ping.Send(YBIP);
            if (pingReply.Status != IPStatus.Success)
                return new object[] { 0, 0, "未连接医保网" };

            if (string.IsNullOrEmpty(YBPORT))
                return new object[] { 0, 0, "参数配置错误｜医保IP端口不能为空" };

            string sysdate = GetServerDateTime();
            string userID = CliUtils.fLoginUser;
            string addr = YBIP;
            int port = int.Parse(YBPORT);
            string servletEntry = YBURL;
            string xtdh00 = string.Empty;
            bool bfalg = false;
            // 初始化成功下，进行系统登记
            string strSql = string.Format(@"select b1ybno,b1ybpw from bz01h where b1empn='{0}' ", userID);
            DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            if (ds.Tables[0].Rows.Count == 0)
                return new object[] { 0, 0, "该用户登记失败｜医保账户及密码错误" };
            else
            {
                ybuserid = ds.Tables[0].Rows[0]["b1ybno"].ToString();
                ybpasswd = ds.Tables[0].Rows[0]["b1ybpw"].ToString();
                xtdh00 = "YY";
            }

            WriteLog(sysdate + "  用户" + userID + " 进入医保初始化...");
            WriteLog(sysdate + "  入参|" + addr + "|" + port + "|" + servletEntry);


            byte[] sMsg = new byte[1024]; //返回标识
            byte[] outParam = new byte[1024];
            string sMsg1 = string.Empty;
            //初始化接口，返回句柄
            whandler = init(addr, port, servletEntry);

            putPara(whandler, "usr", ybuserid);
            putPara(whandler, "pwd", ybpasswd);
            bfalg = process(whandler, "F00.00.00.00");

            if (bfalg)
            {
                //初始化成功
                getErrMsg(whandler, sMsg);
                sMsg1 = Encoding.Default.GetString(sMsg, 0, sMsg.Length).Replace("\0", "");
                if (sMsg1.Trim().Equals("OK"))
                    WriteLog(sysdate + "  进入医保初始化成功" + sMsg);

                //入参
                putPara(whandler, "usr", ybuserid);
                putPara(whandler, "pwd", ybpasswd);
                putPara(whandler, "userid", ybuserid);
                putPara(whandler, "passwd", ybpasswd);
                putPara(whandler, "xtdh00", xtdh00);
                WriteLog(sysdate + "  用户" + userID + " 进入系统登录...");
                WriteLog(sysdate + "  入参|" + ybuserid + "|" + ybpasswd + "|" + xtdh00);
                bfalg = process(whandler, "F09.01.01.05");

                if (bfalg)
                {
                    //获取返回参数
                    string yfmc = string.Empty; //用户名称 
                    string wdbh = string.Empty; //网点编号
                    string wdmc = string.Empty; //网点名称
                    string yydj = string.Empty; //医院等级
                    string fzxbm = string.Empty; //分中心编码
                    string fzxmc = string.Empty; //分中心名称

                    getErrMsg(whandler, sMsg);
                    sMsg1 = Encoding.Default.GetString(sMsg, 0, sMsg.Length).Replace("\0", "");
                    if (sMsg1.Trim().Equals("OK"))
                    {
                        getParaByName(whandler, "usernm", outParam);
                        yfmc = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                        getParaByName(whandler, "akb020", outParam);
                        wdbh = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                        getParaByName(whandler, "akb021", outParam);
                        wdmc = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                        getParaByName(whandler, "aka101", outParam);
                        yydj = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                        getParaByName(whandler, "aab034", outParam);
                        fzxbm = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                        getParaByName(whandler, "aab300", outParam);
                        fzxmc = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                        string strValue = yfmc + "|" + wdbh + "|" + wdmc + "|" + yydj + "|" + fzxbm + "|" + fzxmc;
                        WriteLog(sysdate + "  进入系统登记成功|" + strValue);
                        return new object[] { 0, 1, "医保初始化成功" };
                    }
                    else
                    {
                        WriteLog(sysdate + "  返回服务器端的信息失败");
                        return new object[] { 0, 0, "返回服务器端的信息失败" };
                    }
                }
                else
                {
                    getFailReason(whandler, sMsg);
                    sMsg1 = Encoding.Default.GetString(sMsg, 0, sMsg.Length).Replace("\0", "");
                    WriteLog(sysdate + "  进入系统登录失败" + sMsg);
                    return new object[] { 0, 0, sMsg };
                }
            }
            else
            {
                getFailReason(whandler, sMsg);
                sMsg1 = Encoding.Default.GetString(sMsg, 0, sMsg.Length).Replace("\0", "");
                WriteLog(sysdate + "  进入医保初始化失败|" + sMsg);
                return new object[] { 0, 0, "初始化及用户登录失败|" + sMsg };
            }
        }
        #endregion

        #region 退出医保
        public static object[] YBEXIT(object[] objParam)
        {
            string sysdate = GetServerDateTime();
            bool bfalg = destroyInterface(whandler);
            if (bfalg)
            {
                WriteLog(sysdate + "  医保退出成功");
                return new object[] { 0, 1, "断开连接成功！" };
            }
            else
            {
                byte[] sMsg = new byte[1024]; //返回标识
                string sMsg1 = string.Empty;
                getErrMsg(whandler, sMsg);
                sMsg1 = Encoding.Default.GetString(sMsg, 0, sMsg.Length).Replace("\0", "");
                WriteLog(sysdate + "  医保退出失败" + sMsg);
                return new object[] { 0, 1, "断开连接失败！" + sMsg };
            }
        }
        #endregion


        #region 医保读卡(门诊/住院)
        public static object[] YBMZZYDK(object[] objParam)
        {
            string ickh = string.Empty; //IC卡号
            string cbdq = string.Empty; //参保地行政区划
            string yzdm = string.Empty; //险种代码

            byte[] sMsg = new byte[1024];
            byte[] outParam = new byte[1024];
            string sMsg1 = string.Empty; //返回结算信息
            string sValue = string.Empty;

            frm_dkHF dk = new frm_dkHF();
            dk.ShowDialog();
            sValue = dk.sValue;
            string[] s = sValue.Split('|');
            if (s[0].Equals("0"))
                return new object[] { 0, 0, "读卡失败|" };

            //获取读卡入参
            ickh = s[1];
            cbdq = s[2];
            yzdm = s[3];

            int iCount = 0; //返回行数
            bool bfalg = false;
            string sysdate = GetServerDateTime();

            WriteLog(sysdate + "  进入读卡...");

            //入参
            bfalg = putPara(whandler, "usr", ybuserid);
            bfalg = putPara(whandler, "pwd", ybpasswd);

            bfalg = putPara(whandler, "akc020", "");
            bfalg = putPara(whandler, "aab301", cbdq);
            bfalg = putPara(whandler, "aae140", yzdm);

            WriteLog("入参|" + ickh + "|" + cbdq + "|" + yzdm);

            bfalg = process(whandler, "F04.02.01.01");
            if (bfalg)
            {
                getErrMsg(whandler, sMsg);
                sMsg1 = Encoding.Default.GetString(sMsg, 0, sMsg.Length).Replace("\0", "");
                if (!sMsg1.ToUpper().Trim().Equals("OK"))
                {
                    WriteLog(sysdate + "  获取读卡返回信息失败|" + sMsg1);
                    return new object[] { 0, 0, sMsg1 };
                }

                List<string> liSQL = new List<string>();
                //出参
                #region 定义出参变量
                string ybbh = string.Empty; //医保编号
                string r_ickh = string.Empty; //IC卡号
                string sfzh = string.Empty; //身份证号
                string ylrylb = string.Empty;//医疗人员类别
                string ylrylbmc = string.Empty;//医疗人员类别名称
                string dwbh = string.Empty;//单位编号
                string dwmc = string.Empty;//单位名称
                string xm = string.Empty;//姓名
                string xbbm = string.Empty;//性别编码
                string xbmc = string.Empty;//性别名称
                string nl = string.Empty;//年龄
                string jbjgbm = string.Empty;//经办机构编码(作废)
                string jbjgmc = string.Empty;//经办机构名称(作废)
                string xzqhdm = string.Empty;//行政区划代码(参保人员所属)
                string xzqhmc = string.Empty;//行政区划名称(参保人员所属)
                string grzhye = string.Empty;//个人帐户余额
                string ickzt = string.Empty;//IC卡状态
                string yfxsdbjj = string.Empty;//职工医保是否享受大病基金(000不享受 001享受)
                string ybxslxbm = string.Empty;//医保享受类型编码（00--职工医保 01--居民医保 99--不享受医保）
                string ybxslxmc = string.Empty;//医保享受类型名称
                //特殊病定点信息
                string ddyybh = string.Empty;//定点医院编号
                string ddyymc = string.Empty;//定点医院名称
                string tsbzbm = string.Empty;//特殊病种编码
                string tsbzmc = string.Empty;//特殊病种名称

                //生育信息
                string ysxslxbm = string.Empty; //生育享受类型编码（11--享受职工生育 99--不享受生育保险）
                string ysxslxmc = string.Empty; //生育享受类型名称
                string bxsysbxyysm = string.Empty; //不享受生育保险原因说明 
                string cqjcbayy = string.Empty; //产前检查备案医院 (返回此职工产前检查备案的定点医院的四位编码， '0000'表示未备案产前检查医院)
                string zgscbayy = string.Empty; //职工生产备案医院 (返回此职工生产备案的定点医院的四位编码， '0000'表示未备案生产医院)

                //工伤信息
                string gsxslxbm = string.Empty;//工伤享受类型编码
                string gsxslxmc = string.Empty;//工伤享受类型名称
                string bxsgsbxyysm = string.Empty;//不享受工伤保险原因说明 
                string gsmzbayy = string.Empty; //工伤门诊备案医院(预留) (返回此职工工伤门诊备案的定点医院的四位编码， '0000'表示未备案工伤门诊医院)
                string zgzybayy = string.Empty; //职工住院备案医院 (返回此职工工伤住院备案的定点医院的四位编码， '0000'表示未备案工伤住院医院)
                string gsfssj = string.Empty;   //工伤发生时间
                string scdj = string.Empty;     //伤残等级
                string zllx = string.Empty;     //治疗类型
                string ssbwbm1 = string.Empty;  //受伤部位1编码
                string ssbwbm2 = string.Empty;  //受伤部位2编码
                string ssbwbm3 = string.Empty;  //受伤部位3编码
                string ssbwbm4 = string.Empty;  //受伤部位4编码
                string ssbwbm5 = string.Empty;  //受伤部位5编码
                string ssbwbm6 = string.Empty;  //受伤部位6编码
                string ssbwbmxxys1 = string.Empty;//受伤部位1详细描述
                string ssbwbmxxys2 = string.Empty;//受伤部位2详细描述
                string ssbwbmxxys3 = string.Empty;//受伤部位3详细描述
                string ssbwbmxxys4 = string.Empty;//受伤部位4详细描述
                string ssbwbmxxys5 = string.Empty;//受伤部位5详细描述
                string ssbwbmxxys6 = string.Empty;//受伤部位6详细描述
                #endregion

                #region 获取出参信息
                getParaByName(whandler, "aac001", outParam);
                ybbh = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                outParam = new byte[1024];
                getParaByName(whandler, "akc020", outParam);
                ickh = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                outParam = new byte[1024];
                getParaByName(whandler, "aac002", outParam);
                sfzh = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                outParam = new byte[1024];
                getParaByName(whandler, "akc021", outParam);
                ylrylb = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                outParam = new byte[1024];
                getParaByName(whandler, "vvv001", outParam);
                ylrylbmc = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                outParam = new byte[1024];
                getParaByName(whandler, "aab001", outParam);
                dwbh = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                outParam = new byte[1024];
                getParaByName(whandler, "aab004", outParam);
                dwmc = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                outParam = new byte[1024];
                getParaByName(whandler, "aac003", outParam);
                xm = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                outParam = new byte[1024];
                getParaByName(whandler, "aac004", outParam);
                xbbm = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                outParam = new byte[1024];
                getParaByName(whandler, "vvv002", outParam);
                xbmc = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                outParam = new byte[1024];
                getParaByName(whandler, "akc023", outParam);
                nl = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                outParam = new byte[1024];
                getParaByName(whandler, "aab034", outParam);
                jbjgbm = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                outParam = new byte[1024];
                getParaByName(whandler, "aab300", outParam);
                jbjgmc = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                outParam = new byte[1024];
                getParaByName(whandler, "aab301", outParam);
                xzqhdm = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                outParam = new byte[1024];
                getParaByName(whandler, "aaa146", outParam);
                xzqhmc = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                outParam = new byte[1024];
                getParaByName(whandler, "akc087", outParam);
                grzhye = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                outParam = new byte[1024];
                getParaByName(whandler, "akc141", outParam);
                ickzt = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                outParam = new byte[1024];
                getParaByName(whandler, "ska702", outParam);
                yfxsdbjj = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                outParam = new byte[1024];
                getParaByName(whandler, "skc111", outParam);
                ybxslxbm = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                outParam = new byte[1024];
                getParaByName(whandler, "skc112", outParam);
                ybxslxmc = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");

                /*310医疗保险 410工伤保险 510生育保险 */

                if (yzdm.Equals("410"))
                {
                    outParam = new byte[1024];
                    getParaByName(whandler, "mkc111", outParam);
                    ysxslxbm = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "mkc112", outParam);
                    ysxslxmc = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "smc053", outParam);
                    bxsysbxyysm = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "smc055", outParam);
                    cqjcbayy = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "smc056", outParam);
                    zgscbayy = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                }
                else if (yzdm.Equals("510"))
                {
                    outParam = new byte[1024];
                    getParaByName(whandler, "mkc111", outParam);
                    gsxslxbm = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "lkc112", outParam);
                    gsxslxmc = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "lkc053", outParam);
                    bxsgsbxyysm = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "lkc055", outParam);
                    gsmzbayy = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "lkc056", outParam);
                    zgzybayy = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "alc020", outParam);
                    gsfssj = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "ala040", outParam);
                    scdj = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "lkc058", outParam);
                    zllx = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "slc011", outParam);
                    ssbwbm1 = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "slc012", outParam);
                    ssbwbm2 = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "slc013", outParam);
                    ssbwbm3 = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "slc014", outParam);
                    ssbwbm4 = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "slc015", outParam);
                    ssbwbm5 = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "slc016", outParam);
                    ssbwbm6 = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "slc021", outParam);
                    ssbwbmxxys1 = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "slc022", outParam);
                    ssbwbmxxys2 = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "slc023", outParam);
                    ssbwbmxxys3 = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "slc024", outParam);
                    ssbwbmxxys4 = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "slc025", outParam);
                    ssbwbmxxys5 = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "slc026", outParam);
                    ssbwbmxxys6 = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                }

                //特殊病定点信息
                iCount = toResultSetName(whandler, "list01");
                for (int i = 0; i < iCount; i++)
                {
                    outParam = new byte[1024];
                    getParaByName(whandler, "akb020", outParam);
                    ddyybh = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "akb021", outParam);
                    ddyymc = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "skc516", outParam);
                    tsbzbm = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "skc519", outParam);
                    tsbzmc = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");

                }
                #endregion


                //个人编号|单位编号|身份证号|姓名|性别|民族|出生日期|社会保障卡卡号|
                //医疗待遇类别|人员参保状态|异地人员标志|统筹区号|年度|在院状态|帐户余额|本年医疗费累计|
                //本年帐户支出累计|本年统筹支出累计|本年救助金支出累计|本年公务员补助基金累计|本年城镇居民门诊统筹支付累计|
                //进入统筹费用累计|进入救助金费用累计|起付标准累计|本年住院次数|单位名称|年龄|参保单位类型|经办机构编码
               
                //依身份证号取出生日期　
                string csrq = sfzh.Substring(6, 4) + "-" + sfzh.Substring(10, 2) + "-" + sfzh.Substring(12, 2);
                //年龄取整
                string nl1 = "";
                int index = nl.IndexOf('.');
                if (index > 0)
                    nl1 = nl.Substring(0, nl.IndexOf('.'));
                else
                    nl1 = nl;
                //ick状态
                string strSql = string.Format(@"select NAME from YBXMLBZD where LBMC='IC卡状态' and CODE='{0}'", ickzt);
                DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                if (ds.Tables[0].Rows.Count > 0)
                    ickzt = ds.Tables[0].Rows[0]["NAME"].ToString();

                sValue = ybbh + "|" + dwbh + "|" + sfzh + "|" + xm + "|" + xbbm + "||" + csrq + "|" + ickh
                    + "|" + ybxslxmc + "|" + ickzt + "|0|" + xzqhdm + "|0|0|" + grzhye + "||"
                    + "|||||||||" + dwmc + "|" + nl1 + "|"+yzdm+"|" + jbjgbm;

                WriteLog(sysdate + "  读卡信息成功|" + sValue);
               strSql = string.Format(@"delete from ybickxx where grbh='{0}'", ybbh);
                liSQL.Add(strSql);
                //插入到表数据中
                strSql = string.Format(@"insert into ybickxx(GRBH,KH,GMSFHM,RYLB,RYLBMC,DWBH,DWMC,XM,XB,XBMC,
                                        SJNL,JBJGBM,JBJGMC,DQBH,QXMC,GRZHYE,KZT,SFXSDBJJ,YBXSLXBM,ZGLB,
                                        ysxslxbm,ysxslxmc,bxsysbxyysm,cqjcbayy,zgscbayy,gsxslxbm,gsxslxmc,bxsgsbxyysm,gsmzbayy,zgzybayy,
                                        gsfssj,scdj,zllx,xzdm,ssbwbm2,ssbwbm3,ssbwbm4,ssbwbm5,ssbwbm6,ssbwbmxxys1,
                                        ssbwbmxxys2,ssbwbmxxys3,ssbwbmxxys4,ssbwbmxxys5,ssbwbmxxys6) values(
                                        '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}',
                                        '{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}',
                                        '{20}','{21}','{22}','{23}','{24}','{25}','{26}','{27}','{28}','{29}',
                                        '{30}','{31}','{32}','{33}','{34}','{35}','{36}','{37}','{38}','{39}',
                                        '{40}','{41}','{42}','{43}','{44}')",
                                        ybbh, r_ickh, sfzh, ylrylb, ylrylbmc, dwbh, dwmc, xm, xbbm, xbmc,
                                        nl, jbjgbm, jbjgmc, xzqhdm, xzqhmc, grzhye, ickzt, yfxsdbjj, ybxslxbm, ybxslxmc,
                                        ysxslxbm, ysxslxmc, bxsysbxyysm, cqjcbayy, zgscbayy, gsxslxbm, gsxslxmc, bxsgsbxyysm, gsmzbayy, zgzybayy,
                                        gsfssj, scdj, zllx, yzdm, ssbwbm1, ssbwbm2, ssbwbm3, ssbwbm4, ssbwbm5, ssbwbm6,ssbwbmxxys1, 
                                        ssbwbmxxys2, ssbwbmxxys3, ssbwbmxxys4, ssbwbmxxys5, ssbwbmxxys6);
                liSQL.Add(strSql);
                object[] obj = liSQL.ToArray();
                obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);
                if (obj[1].ToString().Equals("1"))
                {
                    WriteLog(sysdate + "  读卡信息成功|" + sValue);
                    return new object[] { 0, 1, sValue };
                }
                else
                {
                    WriteLog(sysdate + "  保存读卡信息失败|" + obj[2].ToString());
                    return new object[] { 0, 0, obj[2].ToString() };
                }
            }
            else
            {
                getFailReason(whandler, sMsg);
                sMsg1 = Encoding.Default.GetString(sMsg, 0, sMsg.Length).Replace("\0", "");
                WriteLog(sysdate + "  医保门诊/住院读卡失败|" + sMsg1);
                return new object[] { 0, 0, sMsg1 };
            }
        }
        #endregion

        #region 公告查询
        public static object[] YBGGCX(object[] objParam)
        {
            string sysdate = GetServerDateTime();
            byte[] sMsg = new byte[1024];
            byte[] outParam = new byte[1024];
            string sMsg1 = string.Empty; //返回结算信息
            string sValue = string.Empty; //返回参数信息
            bool bfalg = false;

            WriteLog(sysdate + "  进行公告查询...");

            //入参
            putPara(whandler, "usr", ybuserid);
            putPara(whandler, "pwd", ybpasswd);
            bfalg = process(whandler, "F04.12.01.02");
            if (bfalg)
            {
                //获取公告信息
                getErrMsg(whandler, sMsg);
                sMsg1 = Encoding.Default.GetString(sMsg, 0, sMsg.Length).Replace("\0", "");
                if (sMsg1.ToUpper().Trim().Equals("OK"))
                {
                    string ggbt = string.Empty;
                    string ggnr = string.Empty;
                    string ggsj = string.Empty;

                    getParaByName(whandler, "skb611", outParam);
                    ggbt = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    getParaByName(whandler, "skb612", outParam);
                    ggnr = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    getParaByName(whandler, "skb613", outParam);
                    ggsj = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    sValue = ggbt + "|" + ggnr + "|" + ggsj;
                    WriteLog(sysdate + "  获取公告信息成功｜" + sValue);
                    return new object[] { 0, 1, sValue };
                }
                else
                {
                    WriteLog(sysdate + "  获取公告信息失败|" + sMsg1);
                    return new object[] { 0, 0, sMsg1 };
                }
            }
            else
            {
                getFailReason(whandler, sMsg);
                sMsg1 = Encoding.Default.GetString(sMsg, 0, sMsg.Length).Replace("\0", "");
                WriteLog(sysdate + "  进入公告查询失败|" + sMsg1);
                return new object[] { 0, 0, sMsg1 };
            }
        }
        #endregion

        #region 门诊登记 (作废)
//        public static object[] YBMZDJ(object[] objParam)
//        {
//            string sysdate = GetServerDateTime();
//            string jzlsh = objParam[0].ToString(); //就诊流水号
//            string yllb = "";//objParam[1].ToString();   // 医疗类别代码
//            string bzbm = objParam[2].ToString();   // 病种编码(icd10)
//            string bzmc = objParam[3].ToString();   // 病种名称
//            string ghsj = objParam[5].ToString();   // 挂号时间(格式：DateTime.Now.ToString("yyyyMMddHHmmss"))
//            string dgysdm = "";//objParam[6].ToString(); //定岗医生代码(新增)
//            string dgysxm = "";//objParam[7].ToString()
//            string bzbm1 = objParam[8].ToString();
//            string bzmc1 = objParam[9].ToString();
//            string bzbm2 = objParam[10].ToString();
//            string bzmc2 = objParam[11].ToString();

//            string tsmzbm = objParam[12].ToString();
//            string tsmzmc = objParam[13].ToString();

//            string jylsh = DateTime.Now.ToString("yyyyMMddHHmmss") + "-102002-" + new Random().Next(100).ToString().PadLeft(4, '0');

//            #region 获取读卡信息
//            string[] kxx = objParam[4].ToString().Split('|'); //读卡返回信息
//            string grbh = kxx[0].ToString(); //个人编号
//            string dwbh = kxx[1].ToString(); //单位编号
//            string sfzh = kxx[2].ToString(); //身份证号
//            string xm = kxx[3].ToString(); //姓名
//            string xb = kxx[4].ToString(); //性别
//            string mz = kxx[5].ToString(); //民族
//            string csrq = kxx[6].ToString(); //出生日期
//            string kh = kxx[7].ToString(); //卡号
//            string yldylb = kxx[8].ToString(); //医疗待遇类别
//            string rycbzt = kxx[9].ToString(); //人员参保状态
//            string ydrybz = kxx[10].ToString(); //异地人员标志
//            string tcqh = kxx[11].ToString(); //统筹区号
//            string nd = kxx[12].ToString(); //年度
//            string zyzt = kxx[13].ToString(); //在院状态
//            string zhye = kxx[14].ToString(); //帐户余额
//            string bnylflj = kxx[15].ToString(); //本年医疗费累计
//            string bnzhzclj = kxx[16].ToString(); //本年帐户支出累计
//            string bntczclj = kxx[17].ToString(); //本年统筹支出累计
//            string bnjzjzclj = kxx[18].ToString(); //本年救助金支出累计
//            string bngwybzjjlj = kxx[19].ToString(); //本年公务员补助基金累计
//            string bnczjmmztczflj = kxx[20].ToString(); //本年城镇居民门诊统筹支付累计
//            string jrtcfylj = kxx[21].ToString(); //进入统筹费用累计
//            string jrjzjfylj = kxx[22].ToString(); //进入救助金费用累计
//            string qfbzlj = kxx[23].ToString(); //起付标准累计
//            string bnzycs = kxx[24].ToString(); //本年住院次数
//            string Dwmc = kxx[25].ToString(); //单位名称
//            string nl = kxx[26].ToString(); //年龄
//            string cbdwlx = kxx[27].ToString(); //参保单位类型
//            string jbjgbm = kxx[28].ToString(); //经办机构编码
//            #endregion

//            string sMsg1 = string.Empty;
//            byte[] sMsg = new byte[1024];
//            byte[] outParam = new byte[1024];
//            string sValue = string.Empty;
//            bool bfalg = false;
//            int iCount = 0;

//            string ybbh = string.Empty; //医咻编号
//            string ksbm = string.Empty; //科室编码
//            string ksmc = string.Empty; //科室名称
//            string ghfy = string.Empty; //挂号费用
//            string lxdh = string.Empty; //联系电话
//            string jtdz = string.Empty; //家族地址

//            if (string.IsNullOrEmpty(jzlsh))
//                return new object[] { 0, 0, "就诊流水号不能为空" };

//            string strSql = string.Format(@"select a.m1name as name, a.m1telp,a.m1addr,a.m1gham,a.m1ksno,
//                                            (select b2ejnm from bz02d  where b2ksno=a.m1ksno) as ksmc 
//                                            from mz01h  a where a.m1ghno = '{0}' ", jzlsh);
//            DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
//            if (ds.Tables[0].Rows.Count == 0)
//                return new object[] { 0, 0, "该患者未挂号" };
//            else
//            {
//                ybbh = grbh;
//                ksbm = ds.Tables[0].Rows[0]["m1ksno"].ToString();
//                ksmc = ds.Tables[0].Rows[0]["ksmc"].ToString();
//                ghfy = ds.Tables[0].Rows[0]["m1gham"].ToString(); ;
//                lxdh = ds.Tables[0].Rows[0]["m1telp"].ToString();
//                jtdz = ds.Tables[0].Rows[0]["m1addr"].ToString();
//            }

//            strSql = string.Format(@"select * from ybmzzydjdr where jzlsh='{0}' and cxbz=1", jzlsh);
//            ds.Tables.Clear();
//            ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
//            if (ds.Tables[0].Rows.Count > 0)
//                return new object[] { 0, 0, "患者已进行医保门诊登记，清匆再进行重复操作" };

//            WriteLog(sysdate + "  进入医保门诊登记...");
//            //入参
//            putPara(whandler, "usr", ybuserid);
//            putPara(whandler, "pwd", ybpasswd);
//            putPara(whandler, "aac001", ybbh);
//            putPara(whandler, "skb010", ksbm);
//            putPara(whandler, "skc031", ksmc);
//            putPara(whandler, "skc032", ghfy);
//            putPara(whandler, "aae005", lxdh);
//            putPara(whandler, "aae006", jtdz);

//            WriteLog("入参:" + ybuserid + "|" + ybpasswd + "|" + ybbh + "|" + ksbm + "|" + ghfy + "|" + lxdh + "|" + jtdz);

//            bfalg = process(whandler, "F04.05.02.01");
//            if (bfalg)
//            {
//                List<string> liSQL = new List<string>();
//                //获取返回数据
//                getErrMsg(whandler, sMsg);
//                sMsg1 = Encoding.Default.GetString(sMsg, 0, sMsg.Length).Replace("\0", "");
//                if (sMsg1.ToUpper().Trim().Equals("OK"))
//                {
//                    #region 出参数据
//                    string mzh = string.Empty; //门诊号
//                    string ybbh_r = string.Empty; //医保编号
//                    string ickh = string.Empty; //IC卡号
//                    string ickzt = string.Empty; //IC卡状态
//                    string wdbh = string.Empty; //网点编号
//                    string yydj = string.Empty; //医院等级
//                    string ylrylb = string.Empty; //医疗人员类型
//                    string fzxbm = string.Empty; //分中心编码
//                    string fzxmc = string.Empty; //分中心名称
//                    string xzqhdm = string.Empty; //行政区划代码
//                    string xzqhmc = string.Empty; //行政区划名称
//                    string dwbh_r = string.Empty; //单位编号
//                    string xm_r = string.Empty;//姓名
//                    string xb_r = string.Empty;//性别
//                    string nl_r = string.Empty;//年龄
//                    string kbcs = string.Empty;//看病次数
//                    string zydjh = string.Empty;//转院登记号
//                    string ghksbm = string.Empty;//挂号科室编码（必填）
//                    string ghksmc = string.Empty;//挂号科室名称
//                    string ghfy_r = string.Empty;//挂号费用
//                    string cxbz = string.Empty;//冲销标志（Z 正常；+ 被冲销；- 冲销）
//                    string bcxlsh = string.Empty;//被冲销流水号
//                    string jbr = string.Empty;//经办人(医院办理挂号工作人员姓名)
//                    string jbrq = string.Empty;//经办日期(挂号日期)
//                    string jsr = string.Empty;//结算日期
//                    string grzhye = string.Empty;
//                    string ylrylbmc = string.Empty;
//                    string jznd = string.Empty;
//                    string xbmc = string.Empty;
//                    string icztmc = string.Empty;
//                    string yydjmc = string.Empty;
//                    string jbjgmc = string.Empty;
//                    string dwmc_r = string.Empty;
//                    string wdmc = string.Empty;
//                    //收费单据
//                    string djh = string.Empty;
//                    string ybjjzfje = string.Empty;
//                    string grxjzfje = string.Empty;
//                    string grzhzfje = string.Empty;
//                    string zje = string.Empty;
//                    string cfxms = string.Empty;
//                    string jsrq = string.Empty;
//                    string jsr_r = string.Empty;
//                    #endregion

//                    #region 获取门诊挂号数据
//                    getParaByName(whandler, "akc190", outParam);
//                    mzh = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
//                    outParam = new byte[1024];
//                    getParaByName(whandler, "aac001", outParam);
//                    ybbh_r = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
//                    outParam = new byte[1024];
//                    getParaByName(whandler, "akc020", outParam);
//                    ickh = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
//                    outParam = new byte[1024];
//                    getParaByName(whandler, "akc141", outParam);
//                    ickzt = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
//                    outParam = new byte[1024];
//                    getParaByName(whandler, "akb020", outParam);
//                    wdbh = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
//                    outParam = new byte[1024];
//                    getParaByName(whandler, "aka101", outParam);
//                    yydj = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
//                    outParam = new byte[1024];
//                    getParaByName(whandler, "akc021", outParam);
//                    ylrylb = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
//                    outParam = new byte[1024];
//                    getParaByName(whandler, "aab034", outParam);
//                    fzxbm = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
//                    outParam = new byte[1024];
//                    getParaByName(whandler, "aab300", outParam);
//                    fzxmc = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
//                    outParam = new byte[1024];
//                    getParaByName(whandler, "aab301", outParam);
//                    xzqhdm = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
//                    outParam = new byte[1024];
//                    getParaByName(whandler, "aaa146", outParam);
//                    xzqhmc = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
//                    outParam = new byte[1024];
//                    getParaByName(whandler, "aab001", outParam);
//                    dwbh_r = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
//                    outParam = new byte[1024];
//                    getParaByName(whandler, "aac003", outParam);
//                    xm_r = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
//                    outParam = new byte[1024];
//                    getParaByName(whandler, "aac004", outParam);
//                    xb_r = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
//                    outParam = new byte[1024];
//                    getParaByName(whandler, "akc023", outParam);
//                    nl_r = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
//                    outParam = new byte[1024];
//                    getParaByName(whandler, "aka150", outParam);
//                    kbcs = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
//                    outParam = new byte[1024];
//                    getParaByName(whandler, "skc030", outParam);
//                    zydjh = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
//                    outParam = new byte[1024];
//                    getParaByName(whandler, "skb010", outParam);
//                    ghksbm = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
//                    outParam = new byte[1024];
//                    getParaByName(whandler, "skc031", outParam);
//                    ghksmc = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
//                    outParam = new byte[1024];
//                    getParaByName(whandler, "skc032", outParam);
//                    ghfy_r = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
//                    outParam = new byte[1024];
//                    getParaByName(whandler, "skc033", outParam);
//                    cxbz = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
//                    outParam = new byte[1024];
//                    getParaByName(whandler, "skc034", outParam);
//                    bcxlsh = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
//                    outParam = new byte[1024];
//                    getParaByName(whandler, "aae011", outParam);
//                    jbr = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
//                    outParam = new byte[1024];
//                    getParaByName(whandler, "aae036", outParam);
//                    jbrq = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
//                    outParam = new byte[1024];
//                    getParaByName(whandler, "aae040", outParam);
//                    jsrq = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
//                    outParam = new byte[1024];
//                    getParaByName(whandler, "skc035", outParam);
//                    jsr = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
//                    outParam = new byte[1024];
//                    getParaByName(whandler, "akc087", outParam);
//                    grzhye = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
//                    outParam = new byte[1024];
//                    getParaByName(whandler, "aae001", outParam);
//                    jznd = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
//                    outParam = new byte[1024];
//                    getParaByName(whandler, "vvv001", outParam);
//                    ylrylbmc = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
//                    outParam = new byte[1024];
//                    getParaByName(whandler, "vvv002", outParam);
//                    xbmc = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
//                    outParam = new byte[1024];
//                    getParaByName(whandler, "vvv003", outParam);
//                    icztmc = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
//                    outParam = new byte[1024];
//                    getParaByName(whandler, "vvv004", outParam);
//                    yydjmc = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
//                    outParam = new byte[1024];
//                    getParaByName(whandler, "aab300", outParam);
//                    jbjgmc = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
//                    outParam = new byte[1024];
//                    getParaByName(whandler, "aab004", outParam);
//                    Dwmc = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
//                    outParam = new byte[1024];
//                    getParaByName(whandler, "akb021", outParam);
//                    wdmc = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
//                    //收费单据<list01>开始
//                    iCount = toResultSetName(whandler, "list01");
//                    for (int i = 0; i < iCount; i++)
//                    {
//                        outParam = new byte[1024];
//                        getParaByName(whandler, "aae072", outParam);
//                        djh = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
//                        outParam = new byte[1024];
//                        getParaByName(whandler, "akc260", outParam);
//                        ybjjzfje = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
//                        outParam = new byte[1024];
//                        getParaByName(whandler, "akc261", outParam);
//                        grxjzfje = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
//                        outParam = new byte[1024];
//                        getParaByName(whandler, "akc262", outParam);
//                        grzhzfje = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
//                        outParam = new byte[1024];
//                        getParaByName(whandler, "akc264", outParam);
//                        zje = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
//                        outParam = new byte[1024];
//                        getParaByName(whandler, "skc099", outParam);
//                        cfxms = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
//                        outParam = new byte[1024];
//                        getParaByName(whandler, "aae040", outParam);
//                        jsrq = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
//                        outParam = new byte[1024];
//                        getParaByName(whandler, "skc035", outParam);
//                        jsr_r = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
//                        sValue = djh + "|" + ybjjzfje + "|" + grxjzfje + "|" + grzhzfje + "|" + zje + "|" + cfxms + "|" + jsrq + "|" + jsr_r;
//                        WriteLog(sysdate + "  门诊医保登记|收费单据|" + sValue);
//                    }

//                    #endregion

//                    //插入数据
//                    strSql = string.Format(@"insert into ybmzzydjdr(jzlsh,ybjzlsh,grbh,kh,ickzt,wdbh,yydj,yllb,fzxbm,fzxmc,
//                                            xzqhdm,xzqhmc,dwbh,Dwmc,xm,xb,nl,bckbcs,zydjh,ksbh,
//                                            ksmc,ghf,cxbz1,bcxlsh,jbr,ghdjsj,jsrq,jsr,zhye,jznd,
//                                            yldyxz,xbmc,ickztmc,yydjmc,jbjgmc,wdmc,bzbm,bzmc,mmbzbm1,mmbzmc1,
//                                            mmbzbm2, mmbzmc2,mmbzbm3,mmbzmc3,jylsh,sysdate,xzbm) values(
//                                            '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}',
//                                            '{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}',
//                                            '{20}','{21}','{22}','{23}','{24}','{25}','{26}','{27}','{28}','{29}',
//                                            '{30}','{31}','{32}','{33}','{34}','{35}','{36}','{37}','{38}','{39}',
//                                            '{40}','{41}','{42}','{43}','{44}','{45}','{46}')",
//                                            jzlsh, mzh, ybbh_r, ickh, ickzt, wdbh, yydj, ylrylb, fzxbm, fzxmc,
//                                            xzqhdm, xzqhmc, dwbh_r, dwmc_r, xm_r, xb_r, nl_r, kbcs, zydjh, ksbm,
//                                            ksmc, ghfy_r, cxbz, bcxlsh, jbr, jbrq, jsrq, jsr, grzhye, jznd,
//                                            ylrylbmc, xbmc, icztmc, yydjmc, jbjgmc, wdmc, bzbm, bzmc, bzbm1, bzmc1,
//                                            bzbm2, bzmc2, tsmzbm, tsmzmc, jylsh, sysdate, cbdwlx);
//                    liSQL.Add(strSql);
//                    object[] obj = liSQL.ToArray();
//                    obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);
//                    if (obj[1].ToString().Equals("1"))
//                    {
//                        WriteLog(sysdate + "  医保门诊登记成功|" + jzlsh + "|" + mzh + "|" + ghfy + "|" + jsrq);
//                        return new object[] { 0, 1, "医保门诊登记成功|" };
//                    }
//                    else
//                    {
//                        WriteLog(sysdate + "  医保门诊登记数据操作失败|" + obj[2].ToString());
//                        //撤销登记信息
//                        object[] obj1 = { ybbh_r, mzh };
//                        N_YBMZDJCX(obj1);
//                        return new object[] { 0, 0, "医保门诊登记数据操作失败|" };
//                    }
//                }
//                else
//                {
//                    WriteLog(sysdate + "  获取门诊挂号返回信息失败|" + sMsg1);
//                    return new object[] { 0, 0, "获取门诊挂号返回信息失败" };
//                }
//            }
//            else
//            {
//                getFailReason(whandler, sMsg);
//                sMsg1 = Encoding.Default.GetString(sMsg, 0, sMsg.Length).Replace("\0", "");
//                WriteLog(sysdate + "  门诊挂号失败|" + sMsg1);
//                return new object[] { 0, 0, sMsg1 };
//            }
//        }
        #endregion

        #region 查询有效门诊号列表
        public static object[] YBCXYXMZHLB(object[] objParam)
        {
            string sysdate = GetServerDateTime();
            string ybbh = objParam[0].ToString();

            byte[] sMsg = new byte[1024];
            byte[] outParam = new byte[1024];
            string sMsg1 = string.Empty;
            string sValue = string.Empty;
            bool bfalg = false;

            if (string.IsNullOrEmpty(ybbh))
                return new object[] { 0, 0, "医保编号不能为空" };

            //入参
            putPara(whandler, "usr", ybuserid);
            putPara(whandler, "pwd", ybpasswd);
            putPara(whandler, "aac001", ybbh);

            WriteLog("入参|" + ybuserid + "|" + ybpasswd + "|" + ybbh);

            bfalg = process(whandler, "F04.05.01.01");
            if (bfalg)
            {
                getErrMsg(whandler, sMsg);
                sMsg1 = Encoding.Default.GetString(sMsg, 0, sMsg.Length).Replace("\0", "");
                if (sMsg1.Trim().Equals("OK"))
                {
                    #region 出参
                    string ghsl = string.Empty;
                    string fpsl = string.Empty;
                    string mzh = string.Empty;
                    string ghksbm = string.Empty;
                    string ghksmc = string.Empty;
                    string ghfy = string.Empty;
                    string jbr = string.Empty;
                    string ghrq = string.Empty;
                    #endregion
                    #region 获取出参数据
                    getParaByName(whandler, "vvv001", outParam);
                    ghsl = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    getParaByName(whandler, "vvv002", outParam);
                    fpsl = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    getParaByName(whandler, "akc190", outParam);
                    mzh = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    getParaByName(whandler, "skb010", outParam);
                    ghksbm = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    getParaByName(whandler, "skc031", outParam);
                    ghksmc = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    getParaByName(whandler, "skc032", outParam);
                    ghfy = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    getParaByName(whandler, "aae011", outParam);
                    jbr = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    getParaByName(whandler, "aae036", outParam);
                    ghrq = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    sValue = ghsl + "|" + fpsl + "|" + mzh + "|" + ghksbm + "|" + ghksmc + "|" + ghfy + "|" + jbr + "|" + ghrq;
                    #endregion
                    return new object[] { 0, 1, sValue };
                }
                else
                {
                    return new object[] { 0, 0, "获取公告信息失败|" + sMsg1 };
                }
            }
            else
            {
                getFailReason(whandler, sMsg);
                sMsg1 = Encoding.Default.GetString(sMsg, 0, sMsg.Length).Replace("\0", "");
                WriteLog(sysdate + "  查询有效门诊号列表失败|" + sMsg1);
                return new object[] { 0, 0, sMsg1 };
            }

        }
        #endregion

        #region 门诊挂号查询
        public static object[] YBMZGHCX(object[] objParam)
        {
            string sysdate = GetServerDateTime();
            string jzlsh = objParam[0].ToString();

            if (string.IsNullOrEmpty(jzlsh))
                return new object[] { 0, 0, "就诊流水号不能为空" };

            string mzh = string.Empty;
            string sMsg = string.Empty;
            string sValue = string.Empty;

            string strSql = string.Format(@"select * from ybmzzydjdr where jzlsh='{0}' and cxbz=1", jzlsh);
            DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            if (ds.Tables[0].Rows.Count == 0)
                return new object[] { 0, 0, "患者未做医保门诊登记" };
            else
                mzh = ds.Tables[0].Rows[0]["ybjzlsh"].ToString();
            return null;


        }
        #endregion

        #region 门诊处方明细上报
        public static object[] YBMZSFDJ(object[] objParam)
        {
            return new object[] { 0, 1, "门诊处方明细上报成功" };
        }
        #endregion


        #region 门诊登记_医疗/生育/工伤
        public static object[] YBMZDJ(object[] objParam)
        {
            string sysdate = GetServerDateTime();
            string jzlsh = objParam[0].ToString(); //就诊流水号
            string yllb = "";//objParam[1].ToString();   // 医疗类别代码
            string bzbm = objParam[2].ToString();   // 病种编码(icd10)
            string bzmc = objParam[3].ToString();   // 病种名称
            string ghsj = objParam[5].ToString();   // 挂号时间(格式：DateTime.Now.ToString("yyyyMMddHHmmss"))
            string dgysdm = "";//objParam[6].ToString(); //定岗医生代码(新增)
            string dgysxm = "";//objParam[7].ToString()
            string bzbm1 = objParam[8].ToString();
            string bzmc1 = objParam[9].ToString();
            string bzbm2 = objParam[10].ToString();
            string bzmc2 = objParam[11].ToString();

            string tsmzbm = objParam[12].ToString();
            string tsmzmc = objParam[13].ToString();

            string jylsh = DateTime.Now.ToString("yyyyMMddHHmmss") + "-102002-" + new Random().Next(100).ToString().PadLeft(4, '0');

            #region 获取读卡信息
            string[] kxx = objParam[4].ToString().Split('|'); //读卡返回信息
            string grbh = kxx[0].ToString(); //个人编号
            string dwbh = kxx[1].ToString(); //单位编号
            string sfzh = kxx[2].ToString(); //身份证号
            string xm = kxx[3].ToString(); //姓名
            string xb = kxx[4].ToString(); //性别
            string mz = kxx[5].ToString(); //民族
            string csrq = kxx[6].ToString(); //出生日期
            string kh = kxx[7].ToString(); //卡号
            string yldylb = kxx[8].ToString(); //医疗待遇类别
            string rycbzt = kxx[9].ToString(); //人员参保状态
            string ydrybz = kxx[10].ToString(); //异地人员标志
            string tcqh = kxx[11].ToString(); //统筹区号
            string nd = kxx[12].ToString(); //年度
            string zyzt = kxx[13].ToString(); //在院状态
            string zhye = kxx[14].ToString(); //帐户余额
            string bnylflj = kxx[15].ToString(); //本年医疗费累计
            string bnzhzclj = kxx[16].ToString(); //本年帐户支出累计
            string bntczclj = kxx[17].ToString(); //本年统筹支出累计
            string bnjzjzclj = kxx[18].ToString(); //本年救助金支出累计
            string bngwybzjjlj = kxx[19].ToString(); //本年公务员补助基金累计
            string bnczjmmztczflj = kxx[20].ToString(); //本年城镇居民门诊统筹支付累计
            string jrtcfylj = kxx[21].ToString(); //进入统筹费用累计
            string jrjzjfylj = kxx[22].ToString(); //进入救助金费用累计
            string qfbzlj = kxx[23].ToString(); //起付标准累计
            string bnzycs = kxx[24].ToString(); //本年住院次数
            string dwmc = kxx[25].ToString(); //单位名称
            string nl = kxx[26].ToString(); //年龄
            string cbdwlx = kxx[27].ToString(); //参保单位类型 --> 对应险种编码
            string jbjgbm = kxx[28].ToString(); //经办机构编码
            #endregion

            string sMsg1 = string.Empty;
            byte[] sMsg = new byte[1024];
            byte[] outParam = new byte[1024];
            string sValue = string.Empty;
            bool bfalg = false;
            int iCount = 0;

            string ybbh = string.Empty; //医咻编号
            string ksbm = string.Empty; //科室编码
            string ksmc = string.Empty; //科室名称
            string ghfy = string.Empty; //挂号费用
            string lxdh = string.Empty; //联系电话
            string jtdz = string.Empty; //家族地址

            if (string.IsNullOrEmpty(jzlsh))
                return new object[] { 0, 0, "就诊流水号不能为空" };

            string strSql = string.Format(@"select a.m1name as name, a.m1telp,a.m1addr,a.m1gham,a.m1ksno,
                                            (select b2ejnm from bz02d  where b2ksno=a.m1ksno) as ksmc 
                                            from mz01h  a where a.m1ghno = '{0}' ", jzlsh);
            DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            if (ds.Tables[0].Rows.Count == 0)
                return new object[] { 0, 0, "该患者未挂号" };
            else
            {
                ybbh = grbh;
                ksbm = ds.Tables[0].Rows[0]["m1ksno"].ToString();
                ksmc = ds.Tables[0].Rows[0]["ksmc"].ToString();
                ghfy = ds.Tables[0].Rows[0]["m1gham"].ToString(); ;
                lxdh = ds.Tables[0].Rows[0]["m1telp"].ToString();
                jtdz = ds.Tables[0].Rows[0]["m1addr"].ToString();
            }

            strSql = string.Format(@"select * from ybmzzydjdr where jzlsh='{0}' and cxbz=1", jzlsh);
            ds.Tables.Clear();
            ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            if (ds.Tables[0].Rows.Count > 0)
                return new object[] { 0, 0, "患者已进行医保门诊登记，清匆再进行重复操作" };


            WriteLog(sysdate + "  进入医保门诊登记...");
            if (cbdwlx.Equals("310")) //医疗保险
            {
                //入参
                putPara(whandler, "usr", ybuserid);
                putPara(whandler, "pwd", ybpasswd);
                putPara(whandler, "aac001", ybbh);
                putPara(whandler, "skb010", ksbm);
                putPara(whandler, "skc031", ksmc);
                putPara(whandler, "skc032", ghfy);
                putPara(whandler, "aae005", lxdh);
                putPara(whandler, "aae006", jtdz);

                WriteLog("入参:" + ybuserid + "|" + ybpasswd + "|" + ybbh + "|" + ksbm + "|" + ghfy + "|" + lxdh + "|" + jtdz);
                bfalg = process(whandler, "F04.05.02.01");
            }
            else if (cbdwlx.Equals("510")) //生育保险
            {
                //入参
                putPara(whandler, "usr", ybuserid);
                putPara(whandler, "pwd", ybpasswd);
                putPara(whandler, "aac001", ybbh);
                putPara(whandler, "skc031", ksmc);
                putPara(whandler, "skc032", ghfy);
                putPara(whandler, "skc113", "S");

                WriteLog("入参:" + ybuserid + "|" + ybpasswd + "|" + ybbh + "|" + ksmc + "|" + ghfy + "|S");
                bfalg = process(whandler, "F04.05.02.01");
            }
            else if (cbdwlx.Equals("410")) //工伤保险
            {
                putPara(whandler, "usr", ybuserid);
                putPara(whandler, "pwd", ybpasswd);
                putPara(whandler, "aac001", ybbh);
                putPara(whandler, "skc031", ksmc);
                putPara(whandler, "skc032", ghfy);

                WriteLog("入参:" + ybuserid + "|" + ybpasswd + "|" + ybbh + "|" + ksbm + "|" + ghfy);
                bfalg = process(whandler, "F07.11.05.01");
            }


            if (bfalg)
            {
                List<string> liSQL = new List<string>();
                //获取返回数据
                getErrMsg(whandler, sMsg);
                sMsg1 = Encoding.Default.GetString(sMsg, 0, sMsg.Length).Replace("\0", "");
                if (sMsg1.ToUpper().Trim().Equals("OK"))
                {
                    #region 出参数据
                    string mzh = string.Empty; //门诊号
                    string ybbh_r = string.Empty; //医保编号
                    string ickh = string.Empty; //IC卡号
                    string ickzt = string.Empty; //IC卡状态
                    string wdbh = string.Empty; //网点编号
                    string yydj = string.Empty; //医院等级
                    string ylrylb = string.Empty; //医疗人员类型
                    string fzxbm = string.Empty; //分中心编码
                    string fzxmc = string.Empty; //分中心名称
                    string xzqhdm = string.Empty; //行政区划代码
                    string xzqhmc = string.Empty; //行政区划名称
                    string dwbh_r = string.Empty; //单位编号
                    string xm_r = string.Empty;//姓名
                    string xb_r = string.Empty;//性别
                    string nl_r = string.Empty;//年龄
                    string kbcs = string.Empty;//看病次数
                    string zydjh = string.Empty;//转院登记号
                    string ghksbm = string.Empty;//挂号科室编码（必填）
                    string ghksmc = string.Empty;//挂号科室名称
                    string ghfy_r = string.Empty;//挂号费用
                    string cxbz = string.Empty;//冲销标志（Z 正常；+ 被冲销；- 冲销）
                    string bcxlsh = string.Empty;//被冲销流水号
                    string jbr = string.Empty;//经办人(医院办理挂号工作人员姓名)
                    string jbrq = string.Empty;//经办日期(挂号日期)
                    string jsr = string.Empty;//结算日期
                    string grzhye = string.Empty;
                    string ylrylbmc = string.Empty;
                    string jznd = string.Empty;
                    string xbmc = string.Empty;
                    string icztmc = string.Empty;
                    string yydjmc = string.Empty;
                    string jbjgmc = string.Empty;
                    string dwmc_r = string.Empty;
                    string wdmc = string.Empty;
                    //收费单据
                    string djh = string.Empty;
                    string ybjjzfje = string.Empty;
                    string grxjzfje = string.Empty;
                    string grzhzfje = string.Empty;
                    string zje = string.Empty;
                    string cfxms = string.Empty;
                    string jsrq = string.Empty;
                    string jsr_r = string.Empty;
                    #endregion

                    #region 获取门诊挂号数据
                    getParaByName(whandler, "akc190", outParam);
                    mzh = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "aac001", outParam);
                    ybbh_r = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "akc020", outParam);
                    ickh = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "akc141", outParam);
                    ickzt = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "akb020", outParam);
                    wdbh = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "aka101", outParam);
                    yydj = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "akc021", outParam);
                    ylrylb = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "aab034", outParam);
                    fzxbm = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "aab300", outParam);
                    fzxmc = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "aab301", outParam);
                    xzqhdm = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    //getParaByName(whandler, "aaa146", outParam);
                    //xzqhmc = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    //outParam = new byte[1024];
                    getParaByName(whandler, "aab001", outParam);
                    dwbh_r = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "aac003", outParam);
                    xm_r = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "aac004", outParam);
                    xb_r = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "akc023", outParam);
                    nl_r = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "aka150", outParam);
                    kbcs = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "skc030", outParam);
                    zydjh = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    //outParam = new byte[1024];
                    //getParaByName(whandler, "skb010", outParam);
                    //ghksbm = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "skc031", outParam);
                    ghksmc = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "skc032", outParam);
                    ghfy_r = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "skc033", outParam);
                    cxbz = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "skc034", outParam);
                    bcxlsh = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "aae011", outParam);
                    jbr = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "aae036", outParam);
                    jbrq = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "aae040", outParam);
                    jsrq = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "skc035", outParam);
                    jsr = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "akc087", outParam);
                    grzhye = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "aae001", outParam);
                    jznd = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "vvv001", outParam);
                    ylrylbmc = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "vvv002", outParam);
                    xbmc = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "vvv003", outParam);
                    icztmc = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "vvv004", outParam);
                    yydjmc = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "aab300", outParam);
                    jbjgmc = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "aab004", outParam);
                    dwmc = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "akb021", outParam);
                    wdmc = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    //收费单据<list01>开始
                    iCount = toResultSetName(whandler, "list01");
                    for (int i = 0; i < iCount; i++)
                    {
                        outParam = new byte[1024];
                        getParaByName(whandler, "aae072", outParam);
                        djh = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                        outParam = new byte[1024];
                        getParaByName(whandler, "akc260", outParam);
                        ybjjzfje = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                        outParam = new byte[1024];
                        getParaByName(whandler, "akc261", outParam);
                        grxjzfje = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                        outParam = new byte[1024];
                        getParaByName(whandler, "akc262", outParam);
                        grzhzfje = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                        outParam = new byte[1024];
                        getParaByName(whandler, "akc264", outParam);
                        zje = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                        outParam = new byte[1024];
                        getParaByName(whandler, "skc099", outParam);
                        cfxms = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                        outParam = new byte[1024];
                        getParaByName(whandler, "aae040", outParam);
                        jsrq = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                        outParam = new byte[1024];
                        getParaByName(whandler, "skc035", outParam);
                        jsr_r = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                        sValue = djh + "|" + ybjjzfje + "|" + grxjzfje + "|" + grzhzfje + "|" + zje + "|" + cfxms + "|" + jsrq + "|" + jsr_r;
                        WriteLog(sysdate + "  门诊医保登记|收费单据|" + sValue);
                    }

                    #endregion

                    //插入数据
                    strSql = string.Format(@"insert into ybmzzydjdr(jzlsh,ybjzlsh,grbh,kh,ickzt,wdbh,yydj,yllb,fzxbm,fzxmc,
                                            xzqhdm,xzqhmc,dwbh,dwmc,xm,xb,nl,bckbcs,zydjh,ksbh,
                                            ksmc,ghf,cxbz1,bcxlsh,jbr,ghdjsj,jsrq,jsr,zhye,jznd,
                                            yldyxz,xbmc,ickztmc,yydjmc,jbjgmc,wdmc,bzbm,bzmc,mmbzbm1,mmbzmc1,
                                            mmbzbm2, mmbzmc2,mmbzbm3,mmbzmc3,jylsh,sysdate,xzbm) values(
                                            '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}',
                                            '{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}',
                                            '{20}','{21}','{22}','{23}','{24}','{25}','{26}','{27}','{28}','{29}',
                                            '{30}','{31}','{32}','{33}','{34}','{35}','{36}','{37}','{38}','{39}',
                                            '{40}','{41}','{42}','{43}','{44}','{45}','{46}')",
                                            jzlsh, mzh, ybbh_r, ickh, ickzt, wdbh, yydj, ylrylb, fzxbm, fzxmc,
                                            xzqhdm, xzqhmc, dwbh_r, dwmc_r, xm_r, xb_r, nl_r, kbcs, zydjh, ksbm,
                                            ksmc, ghfy_r, cxbz, bcxlsh, jbr, jbrq, jsrq, jsr, grzhye, jznd,
                                            ylrylbmc, xbmc, icztmc, yydjmc, jbjgmc, wdmc, bzbm, bzmc, bzbm1, bzmc1,
                                            bzbm2, bzmc2, tsmzbm, tsmzmc, jylsh, sysdate, cbdwlx);
                    liSQL.Add(strSql);
                    object[] obj = liSQL.ToArray();
                    obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);
                    if (obj[1].ToString().Equals("1"))
                    {
                        WriteLog(sysdate + "  医保门诊登记成功|" + jzlsh + "|" + mzh + "|" + ghfy + "|" + jsrq);
                        return new object[] { 0, 1, "医保门诊登记成功|" };
                    }
                    else
                    {
                        WriteLog(sysdate + "  医保门诊登记数据操作失败|" + obj[2].ToString());
                        //撤销登记信息
                        object[] obj1 = { ybbh_r, mzh, cbdwlx };
                        N_YBMZDJCX(obj1);
                        return new object[] { 0, 0, "医保门诊登记数据操作失败|" };
                    }
                }
                else
                {
                    WriteLog(sysdate + "  获取门诊挂号返回信息失败|" + sMsg1);
                    return new object[] { 0, 0, "获取门诊挂号返回信息失败" };
                }
            }
            else
            {
                getFailReason(whandler, sMsg);
                sMsg1 = Encoding.Default.GetString(sMsg, 0, sMsg.Length).Replace("\0", "");
                WriteLog(sysdate + "  门诊挂号失败|" + sMsg1);
                return new object[] { 0, 0, sMsg1 };
            }
 
        }
        #endregion

        #region 门诊登记撤销_医疗/生育/工伤
        public static object[] YBMZDJCX(object[] objParam)
        {
            string sysdate = GetServerDateTime();
            string jzlsh = objParam[0].ToString();

            string ybbh = string.Empty; //医保编号
            string mzh = string.Empty; //门诊号
            string xzbm = string.Empty; // 险种编码
            byte[] sMsg = new byte[1024];
            byte[] outParam = new byte[1024];
            string sMsg1 = string.Empty;
            string sValue = string.Empty;
            bool bfalg = false;
            int iCount = 0;

            if (string.IsNullOrEmpty(jzlsh))
                return new object[] { 0, 0, "就诊流水号不能为空" };

            string strSql = string.Format(@"select * from ybmzzydjdr where jzlsh='{0}' and cxbz=1", jzlsh);
            DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            if (ds.Tables[0].Rows.Count == 0)
                return new object[] { 0, 0, "患者未做医保门诊登记" };

            ybbh = ds.Tables[0].Rows[0]["grbh"].ToString();
            mzh = ds.Tables[0].Rows[0]["ybjzlsh"].ToString();
            xzbm = ds.Tables[0].Rows[0]["xzbm"].ToString();

            //入参
            if (xzbm.Equals("310"))
            {
                WriteLog(sysdate + "  进入门诊登记撤销(医疗保险)...");
                putPara(whandler, "usr", ybuserid);
                putPara(whandler, "pwd", ybpasswd);
                putPara(whandler, "aac001", ybbh);
                putPara(whandler, "akc190", mzh);
                WriteLog("入参|" + ybuserid + "|" + ybpasswd + "|" + ybbh + "|" + mzh);
                bfalg = process(whandler, "F04.05.03.01");
            }
            else if (xzbm.Equals("510")) //生育保险
            {
                WriteLog(sysdate + "  进入门诊登记撤销(生育保险)...");
                putPara(whandler, "usr", ybuserid);
                putPara(whandler, "pwd", ybpasswd);
                putPara(whandler, "aac001", ybbh);
                putPara(whandler, "akc190", mzh);
                putPara(whandler, "skc113", "S");
                WriteLog("入参|" + ybuserid + "|" + ybpasswd + "|" + ybbh + "|" + mzh + "|S");
                bfalg = process(whandler, "F04.05.03.01");
            }
            else if (xzbm.Equals("410")) //工伤保险
            {
                WriteLog(sysdate + "  进入门诊登记撤销(工伤保险)...");
                putPara(whandler, "usr", ybuserid);
                putPara(whandler, "pwd", ybpasswd);
                putPara(whandler, "aac001", ybbh);
                putPara(whandler, "akc190", mzh);
                WriteLog("入参|" + ybuserid + "|" + ybpasswd + "|" + ybbh + "|" + mzh);
                bfalg = process(whandler, "F07.11.05.02");
            }
            
            if (bfalg)
            {
                List<string> liSQL = new List<string>();
                getErrMsg(whandler, sMsg);
                sMsg1 = Encoding.Default.GetString(sMsg, 0, sMsg.Length).Replace("\0", "");
                if (sMsg1.ToUpper().Trim().Equals("OK"))
                {
                    #region 出参数据
                    string cxbz = string.Empty;//冲销标志（Z 正常；+ 被冲销；- 冲销）
                    string bcxlsh = string.Empty;//被冲销流水号
                    #endregion

                    #region 获取门诊挂号数据
                    getParaByName(whandler, "skc033", outParam);
                    cxbz = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    getParaByName(whandler, "skc034", outParam);
                    bcxlsh = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    #endregion

                    //修改表数据
                    strSql = string.Format(@"insert into ybmzzydjdr(jzlsh,jylsh,ybjzlsh,grbh,kh,ickzt,wdbh,yydj,yllb,fzxbm,fzxmc,
                                                    xzqhdm,xzqhmc,dwbh,dwmc,xm,xb,nl,bckbcs,zydjh,ksbh,
                                                    ksmc,ghf,cxbz1,bcxlsh,jbr,ghdjsj,jsrq,jsr,zhye,jznd,
                                                    yldyxz,xbmc,ickztmc,yydjmc,jbjgmc,wdmc,cxbz,sysdate) select
                                                    jzlsh,jylsh,ybjzlsh,grbh,kh,ickzt,wdbh,yydj,yllb,fzxbm,fzxmc,
                                                    xzqhdm,xzqhmc,dwbh,dwmc,xm,xb,nl,bckbcs,zydjh,ksbh,
                                                    ksmc,ghf,'{1}','{2}',jbr,ghdjsj,jsrq,jsr,zhye,jznd,
                                                    yldyxz,xbmc,ickztmc,yydjmc,jbjgmc,wdmc,0,'{3}' from ybmzzydjdr where jzlsh='{0}' and cxbz=1",
                                                    jzlsh, cxbz, bcxlsh, sysdate);
                    liSQL.Add(strSql);
                    strSql = string.Format(@"update ybmzzydjdr set cxbz=2 where jzlsh='{0}' and cxbz=1", jzlsh, sysdate);
                    liSQL.Add(strSql);
                    object[] obj = liSQL.ToArray();
                    obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);
                    if (obj[1].ToString().Equals("1"))
                    {
                        WriteLog(sysdate + "  医保门诊登记撤销成功");
                        return new object[] { 0, 1, "医保门诊登记撤销成功" };
                    }
                    else
                    {
                        WriteLog(sysdate + "  医保门诊登记撤销失败|数据操作失败|" + obj[2].ToString());
                        return new object[] { 0, 0, "医保门诊登记撤销失败|数据操作失败|" };
                    }
                }
                else
                    return new object[] { 0, 0, "获取医保门诊登记撤销信息失败" + sValue };
            }
            else
            {
                getFailReason(whandler, sMsg);
                sMsg1 = Encoding.Default.GetString(sMsg, 0, sMsg.Length).Replace("\0", "");
                WriteLog(sysdate + "  医保门诊登记失败|" + sMsg1);
                return new object[] { 0, 0, "医保门诊登记失败|" + sMsg1 };
            }
        }
        #endregion


        #region 门诊收费预结算
        public static object[] YBMZSFYJS(object[] objParam)
        {
            string jzlsh = objParam[0].ToString();
            string sxzbm=string.Empty;
            if (string.IsNullOrEmpty(jzlsh))
                return new object[] { 0, 0, "就诊流水号不能为空" };

            string strSql = string.Format(@"select xzbm from ybmzzydjdr where jzlsh='{0}' and cxbz=1", objParam[0].ToString());
            DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            if (ds == null || ds.Tables[0].Rows.Count == 0)
                return new object[] { 0, 0, "未做医保门诊登记信息" };
            sxzbm = ds.Tables[0].Rows[0]["xzbm"].ToString();
            object[] objReturn=null;
            switch (sxzbm)
            {
                case "310":
                    objReturn= YBMZSFYJS_YLBX(objParam);
                    break;
                case "410":
                    objReturn = YBMZSFYJS_GSBX(objParam);
                    break;
                case "510":
                    objReturn = YBMZSFYJS_SYBX(objParam);
                    break;
                default:
                    objReturn = new object[] { 0, 0, "门诊费用预结算失败|未找到结算体" };
                break;
            }
            return objReturn;

        }
        #endregion

        #region 门诊收费预结算_医疗保险
        public static object[] YBMZSFYJS_YLBX(object[] objParam)
        {
            string sysdate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");//当前时间
            string czygh = CliUtils.fLoginUser;      // 操作员工号
            string jzlsh = objParam[0].ToString();      // 就诊流水号
            string djh = "0000";        // 处方单据号
            string zhsybz = "1";// objParam[1].ToString();     // 账户使用标志（0或1）
            string jbr = CliUtils.fUserName;        // 经办人姓名
            string cfmxjylsh = string.Empty;  // 交易流水号（门诊传处方对应的交易流水号，住院传空字符串）
            string dqrq = objParam[2].ToString();  // 当前日期
            string cyrq = Convert.ToDateTime(dqrq).ToString("yyyy-MM-dd HH:mm:ss");                                     //出院日期

            string cfhs = objParam[5].ToString();   //处方号集
            string ylfhj1 = objParam[7].ToString(); //医疗费合计 (新增)
            string sfymm = string.Empty; //是否有医保卡密码(0,1)
            string jylsh = DateTime.Now.ToString("yyyyMMddHHmmss") + "-102002-" + new Random().Next(100).ToString().PadLeft(4, '0');


            if (string.IsNullOrEmpty(jzlsh))
                return new object[] { 0, 0, "就诊流水号不能为空" };
            if (string.IsNullOrEmpty(jzlsh))
                return new object[] { 0, 0, "就诊流水号不能为空" };
            if (string.IsNullOrEmpty(ylfhj1))
                return new object[] { 0, 0, "医疗合计费不能为空" };
            if (string.IsNullOrEmpty(cfhs))
                return new object[] { 0, 0, "处方号集不能为空" };

            double sfje2 = 0.0000; //金额 
            #region 金额有效性
            try
            {
                sfje2 = Convert.ToDouble(ylfhj1);
            }
            catch
            {
                return new object[] { 0, 0, "收费金额格式错误" };
            }
            #endregion

            string jsdjh = Convert.ToDateTime(dqrq).ToString("yyyyMMddHHmmss") + jzlsh.Substring(jzlsh.Length - 5, 5);
            byte[] sMsg = new byte[1024];
            byte[] outParam = new byte[1024];
            string sMsg1 = string.Empty;
            string sValue = string.Empty;
            string sfzssf = "000"; //是否正式收费(000 收费预览 001 正式收费)
            string stscpch = string.Empty; //系统上传批次号
            string cfxmsl = "0";
            bool bfalg = false;

            #region 获取患者登记信息
            string strSql = string.Format("select * from ybmzzydjdr a where a.jzlsh = '{0}' and a.cxbz = 1", jzlsh);

            DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            if (ds == null || ds.Tables[0].Rows.Count == 0)
                return new object[] { 0, 0, "未做医保门诊登记信息" };

            DataRow dr1 = ds.Tables[0].Rows[0];
            string ybbh = dr1["grbh"].ToString();  //医保编号
            string mzh = dr1["ybjzlsh"].ToString();
            string bzbm1 = dr1["bzbm"].ToString(); //病情编码
            string bzbm2 = dr1["mmbzbm1"].ToString(); //病情编码1
            string bzbm3 = dr1["mmbzbm2"].ToString(); //病情编码2
            string tsbzbm = dr1["mmbzbm3"].ToString(); //特殊病种编码
            string xm = dr1["xm"].ToString();
            string kh = dr1["kh"].ToString();
            string chxmsl = "0";
            #endregion

            #region 获取处方明细上传数据
            StringBuilder rc = new StringBuilder();
            strSql = string.Format(@"select y.ybxmbh, y.ybxmmc, m.dj, sum(m.sl) sl, sum(m.je) je, m.yyxmbh, m.yyxmmc, m.ysdm ysdm, n.b1name ysxm, m.ksno ksno,
                                    o.b2ejnm zxks, m.sfno, y.sfxmzldm ybsfxmzldm, y.sflbdm ybsflbdm, m.cfh,y.sfxmdjdm as xmlx,y.yfyb from (
                                    --药品
                                    select a.mcypno yyxmbh, a.mcypnm yyxmmc, a.mcpric dj, a.mcquty sl, a.mcamnt je, a.mcksno ksno, a.mcuser ysdm, a.mcsflb sfno,
                                    a.mccfno cfh from mzcfd a where a.mcghno = '{0}' and a.mccfno in ({1})
                                    union all
                                    --检查/治疗 
                                    select b.mbitem yyxmbh, b.mbname yyxmmc, b.mbpric dj, b.mbquty sl, b.mbsjam je, b.mbksno ksno, b.mbuser ysdm, b.mbsfno sfno,
                                    b.mbzlno cfh from mzb2d b where b.mbghno = '{0}' and b.mbzlno in ({1})
                                    union all
                                    --检验
                                    select c.mbitem yyxmbh, c.mbname yyxmmc, c.mbpric dj, c.mbquty sl, c.mbsjam je, c.mbksno ksno, c.mbuser ysdm, c.mbsfno sfno,
                                    c.mbzlno cfh from mzb4d c where c.mbghno = '{0}' and c.mbzlno in ({1})
                                    union all
                                    --注射
                                    select d.mdwayid yyxmbh, d.mdwayx yyxmmc, d.mdamnt dj, 1 sl, d.mdamnt je, d.mdzsks ksno, d.mdempn ysdm, d.mdsflb sfno,
                                    d.mdcfno cfh from mzd3d d where d.mdzsno in ({1})
                                    union all
                                    --处方划价
                                    select a.ygypno yyxmbh, a.ygypnm yyxmmc, ((a.ygamnt + 0.0) / a.ygslxx) dj, a.ygslxx sl, a.ygamnt je, b.ygksno ksno, 
                                    b.ygysno ysdm, c.y1sflb, a.ygshno cfh from yp17d a join yp17h b on a.ygcomp = b.ygcomp and a.ygshno = b.ygshno
                                    join yp01h c on c.y1ypno = a.ygypno
                                    where b.ygghno = '{0}' and a.ygshno in ({1}) and a.ygslxx > 0
                                    ) m left join bz01h n on m.ysdm = n.b1empn 
                                    left join bz02d o on m.ksno = o.b2ejks
                                    left join ybhisdzdr y on m.yyxmbh = y.hisxmbh
                                    group by y.ybxmbh, y.ybxmmc, m.dj, m.yyxmbh, m.yyxmmc, m.ysdm, n.b1name, m.ksno, o.b2ejnm, m.sfno,y.sfxmzldm,y.sflbdm, m.cfh,y.sfxmdjdm,y.yfyb", jzlsh, cfhs);
            ds.Tables.Clear();
            ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            List<string> li_cfxm = new List<string>(); //添加处方明细
            double ylfze = 0.00;
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                cfxmsl = ds.Tables[0].Rows.Count.ToString();
                DataTable dt = ds.Tables[0];
                StringBuilder wdzxms = new StringBuilder();
                for (int k = 0; k < dt.Rows.Count; k++)
                {
                    DataRow dr = dt.Rows[k];
                    if (dr["ybxmbh"] == DBNull.Value)
                        wdzxms.Append("项目代码：[" + dr["yyxmbh"].ToString() + "]，名称：[" + dr["yyxmmc"].ToString() + "]未对照，不能上传！");
                    else
                    {

                        string ybxmbh = dr["ybxmbh"].ToString();          //医保项目编号
                        string ybxmmc = dr["ybxmmc"].ToString();          //医保项目名称
                        string ybsflbdm = dr["ybsflbdm"].ToString();      //收费类别代码
                        string ybyfyb = dr["yfyb"].ToString();            //是否医保  
                        string ysxm = dr["ysxm"].ToString();            //医生姓名
                        string jx = "";//dr["jx"].ToString();  //剂型
                        string gg = "";//dr["gg"].ToString(); //规格  
                        decimal dj = Convert.ToDecimal(dr["dj"]);//单价
                        decimal sl = Convert.ToDecimal(dr["sl"]);//数量
                        decimal je = Convert.ToDecimal(dr["je"]); //金额  
                        ylfze += Convert.ToDouble(je);
                        decimal mcyl = 1;   //每次用量
                        string zxpc = "1";  //使用频次
                        string fy = "1";    //用法
                        string zxts = "1";  //执行天数
                        string ybsfxmzldm = dr["ybsfxmzldm"].ToString();  //收费项目等级代码
                        string yyxmbh = dr["yyxmbh"].ToString();          //医院项目代码
                        string yyxmmc = dr["yyxmmc"].ToString();          //医院项目名称

                        string ysbm = dr["ysdm"].ToString(); //医生编码
                        string ksdm = dr["ksno"].ToString(); //科室代码
                        string ksmc = dr["zxks"].ToString();  //执行科室
                        string cfh = dr["cfh"].ToString(); //处方号
                        string xmlx = dr["xmlx"].ToString(); //项目类型
                        string ybcfh = cfh + k.ToString();
                        string ypjldw = "-";

                        if (dr["sfno"].ToString() == "01" || dr["sfno"].ToString() == "02" || dr["sfno"].ToString() == "03")
                        {
                            ypjldw = "粒";
                        }
                        string sTmp = ybxmbh + "|" + ybxmmc + "|" + ybsflbdm + "|" + ybyfyb + "|" + ysxm + "|" + jx + "|" + gg + "|" + dj + "|" + sl + "|" + je + "|" + mcyl + "|" + zxpc + "|" + fy + "|" + zxts + "|" + yyxmmc + "|" + yyxmbh + "|" + cfh + "|" + ysbm + "|" + ksdm;
                        li_cfxm.Add(sTmp);
                    }
                }
                if (wdzxms.Length > 0)
                    return new object[] { 0, 0, wdzxms.ToString() };
                if (Math.Abs(sfje2 - ylfze) > 1.0)
                    return new object[] { 0, 0, "收费金额与医保结算金额相差" + Math.Abs(sfje2 - ylfze) + ",无法结算，请核实!" };
            }
            else
                return new object[] { 0, 0, "无费用明细" };
            #endregion

            #region 获取批次号
            strSql = string.Format("select * from yb_pcxx where bz=1 and jzbz='m'");
            ds.Tables.Clear();
            ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            string pch = (ds.Tables[0].Rows.Count + 1).ToString();
            stscpch = "MZ" + pch.PadLeft(8, '0');
            #endregion

            WriteLog(sysdate + "  进行门诊收费预结算(医疗保险)...");
            //入参
            putPara(whandler, "usr", ybuserid);
            putPara(whandler, "pwd", ybpasswd);

            putPara(whandler, "aac001", ybbh);
            putPara(whandler, "akc190", mzh);
            putPara(whandler, "skc600", stscpch);
            putPara(whandler, "aka120", bzbm1);
            putPara(whandler, "skc057", bzbm2);
            putPara(whandler, "skc058", bzbm3);
            putPara(whandler, "skc099", cfxmsl);
            putPara(whandler, "skc516", tsbzbm);
            putPara(whandler, "skc098", sfzssf);
            WriteLog("入参|" + ybbh + "|" + mzh + "|" + stscpch + "|" + bzbm1 + "|" + bzbm2 + "|" + bzbm3 + "|" + tsbzbm + "|" + sfzssf);

            #region 上传明细

            string ybsfxmbh = string.Empty; //收费项目编码  <非空>（医保收费目录编码）  
            string ybsfxmmc = string.Empty; //收费项目名称（医院内部收费项目或者药品名称）
            string ybsflb = string.Empty;   //收费类别
            string sfybxm = string.Empty;   //是否医保项目 (000否，001是)
            string ysxm1 = string.Empty;    //医生姓名
            string jxdw = string.Empty;       //剂型（单位）
            string gg1 = string.Empty;      //规格
            string dj1 = string.Empty;       //单价  <非空>
            string sl1 = string.Empty;      //数量<非空>
            string je1 = string.Empty;      //金额<非空>
            string mcyl1 = string.Empty;    //每次用量
            string sypc = string.Empty;     //使用频次
            string fy1 = string.Empty;      //用法
            string zxts1 = string.Empty;    //执行天数
            string yysfxmbh = string.Empty; //医院项目编码
            string yysfxmmc = string.Empty; //医院项目名称
            string cfh1 = string.Empty;     //处方号
            string ysdm1 = string.Empty;   //医生代码
            string ksno1 = string.Empty; //科室代码

            bfalg = startResultSetName(whandler, "list01");

            foreach (string ss in li_cfxm)
            {
                string[] s = ss.Split('|');
                ybsfxmbh = s[0];
                ybsfxmmc = s[1];
                ybsflb = s[2];
                sfybxm = s[3];
                ysxm1 = s[4];
                jxdw = s[5];
                gg1 = s[6];
                dj1 = s[7];
                sl1 = s[8];
                je1 = s[9];
                mcyl1 = s[10];
                sypc = s[11];
                fy1 = s[12];
                zxts1 = s[13];
                yysfxmbh = s[14];
                yysfxmmc = s[15];
                cfh1 = s[16];
                ysdm1 = s[17];
                ksno1 = s[18];


                WriteLog("上传费用明细-->" + ss);
                bfalg = putColData(whandler, "aka060", ybsfxmbh);
                putColData(whandler, "aka061", ybsfxmmc);
                putColData(whandler, "aka063", ybsflb);
                putColData(whandler, "ska003", sfybxm);
                putColData(whandler, "skc049", ysxm1);
                putColData(whandler, "aka070", jxdw);
                putColData(whandler, "aka074", gg1);
                putColData(whandler, "aka068", dj1);
                putColData(whandler, "akc226", sl1);
                putColData(whandler, "akc227", je1);
                putColData(whandler, "aka071", mcyl1);
                putColData(whandler, "aka072", sypc);
                putColData(whandler, "aka073", fy1);
                putColData(whandler, "akc229", zxts1);
                endcurRow(whandler);
            }
            endCurResultSet(whandler);
            #endregion

            bfalg = process(whandler, "F04.05.10.02");
            if (bfalg)
            {
                getErrMsg(whandler, sMsg);
                sMsg1 = Encoding.Default.GetString(sMsg, 0, sMsg.Length).Replace("\0", "");
                if (sMsg1.ToUpper().Trim().Equals("OK"))
                {
                    #region 出参变量
                    string djh_r = string.Empty;    //单据号
                    string mzh_r = string.Empty;    //门诊号
                    string ybbh_r = string.Empty;   //医保编号
                    string ylrylb_r = string.Empty; //医疗人员类别
                    string kh_r = string.Empty;     //iC卡号
                    string xm_r = string.Empty;       //姓名
                    string bcjsqzhye = string.Empty;//个人帐户余额(不含本次收费)
                    string tcjjzf = string.Empty;   //本次医保基金应支付金额
                    string xjzf = string.Empty;     //本次个人现金应支付金额
                    string zhzf = string.Empty;     //本次个人帐户应支付金额
                    string ylzje = string.Empty;    //本次医疗费总额
                    string bcxdjh = string.Empty;   //被冲销单据号
                    string cxbz_r = string.Empty;   //冲销标志（Z 正常；+ 被冲销；- 冲销）
                    string jbr_r = string.Empty;    //经办人(收费人员)
                    string jbrq = string.Empty;     //经办日期(收费日期)
                    string djdycs = string.Empty;   //单据打印次数
                    string cfxms = string.Empty;    //处方项目数
                    string jsrq = string.Empty;     //结算日期
                    string jsr = string.Empty;      //结算人
                    string grzhye = string.Empty;   //个人帐户余额

                    //收费项目<list01>开始 同入参变量
                    string yftsmzyy = string.Empty; //是否特殊门诊用药（000否，001是）
                    #endregion

                    #region 获取门诊收费返回数据
                    getParaByName(whandler, "aae072", outParam);
                    djh_r = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "akc190", outParam);
                    mzh_r = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "aac001", outParam);
                    ybbh_r = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "akc020", outParam);
                    kh_r = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "aac003", outParam);
                    xm_r = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "akc252", outParam);
                    bcjsqzhye = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "akc260", outParam);
                    tcjjzf = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "akc261", outParam);
                    xjzf = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "akc262", outParam);
                    zhzf = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "akc264", outParam);
                    ylzje = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "skc093", outParam);
                    bcxdjh = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "skc033", outParam);
                    cxbz_r = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "aae011", outParam);
                    jbr_r = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "aae036", outParam);
                    jbrq = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "skc095", outParam);
                    djdycs = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "skc099", outParam);
                    cfxms = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "aae040", outParam);
                    jsrq = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "skc035", outParam);
                    jsr = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "akc087", outParam);
                    grzhye = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    #endregion

                    sValue = ylzje + "|0.00|" + tcjjzf + "|0.00|" + zhzf + "|" + xjzf + "|0.00|0.00|0.00|0.00|" +
                           "0.00|0.00|0.00|0.00|0.00|0.00|0.00|0.00|0.00|0.00|" +
                           "0.00|0.00|0.00|0.00|0.00|" + bcjsqzhye + "|0.00|0.00|0.00|0.00|" +
                            "|0.00||0.00||0.00|" + jsrq + "|||||" + djh_r + "||" +
                           djh + "|" + cxbz_r + "|||||||||||||||";

                    WriteLog(sysdate + "  门诊费用预结算(医疗保险)成功|" + sValue);
                    return new object[] { 0, 1, sValue };
                }
                else
                {
                    WriteLog(sysdate + "  获取门诊收费预结算(医疗保险)返回信息失败|" + sMsg1);
                    return new object[] { 0, 0, "获取门诊收费预结算(医疗保险)返回信息失败|" + sMsg1 };
                }
            }
            else
            {
                sMsg = new byte[1024];
                getFailReason(whandler, sMsg);
                sMsg1 = Encoding.Default.GetString(sMsg, 0, sMsg.Length).Replace("\0", "");
                WriteLog(sysdate + "  门诊收费预结算(医疗保险)失败|" + sMsg1);
                return new object[] { 0, 0, "门诊收费预结算(医疗保险)失败|" + sMsg1 };
            }
        }
        #endregion

        #region 门诊收费预结算_生育保险
        public static object[] YBMZSFYJS_SYBX(object[] objParam)
        {
            string sysdate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");//当前时间
            string czygh = CliUtils.fLoginUser;      // 操作员工号
            string jzlsh = objParam[0].ToString();      // 就诊流水号
            string djh = DateTime.Now.ToString("yyyyHHmmss");        // 处方单据号
            string zhsybz = "1";// objParam[1].ToString();     // 账户使用标志（0或1）
            string jbr = CliUtils.fUserName;        // 经办人姓名
            string cfmxjylsh = string.Empty;  // 交易流水号（门诊传处方对应的交易流水号，住院传空字符串）
            string dqrq = objParam[2].ToString();  // 当前日期
            string cyrq = Convert.ToDateTime(dqrq).ToString("yyyy-MM-dd HH:mm:ss");                                     //出院日期

            string cfhs = objParam[5].ToString();   //处方号集
            string ylfhj1 = objParam[7].ToString(); //医疗费合计 (新增)
            string sfymm = string.Empty; //是否有医保卡密码(0,1)
            string jylsh = DateTime.Now.ToString("yyyyMMddHHmmss") + "-102002-" + new Random().Next(100).ToString().PadLeft(4, '0');


            if (string.IsNullOrEmpty(jzlsh))
                return new object[] { 0, 0, "就诊流水号不能为空" };
            if (string.IsNullOrEmpty(jzlsh))
                return new object[] { 0, 0, "就诊流水号不能为空" };
            if (string.IsNullOrEmpty(ylfhj1))
                return new object[] { 0, 0, "医疗合计费不能为空" };
            if (string.IsNullOrEmpty(cfhs))
                return new object[] { 0, 0, "处方号集不能为空" };

            double sfje2 = 0.0000; //金额 
            #region 金额有效性
            try
            {
                sfje2 = Convert.ToDouble(ylfhj1);
            }
            catch
            {
                return new object[] { 0, 0, "收费金额格式错误" };
            }
            #endregion

            string jsdjh = Convert.ToDateTime(dqrq).ToString("yyyyMMddHHmmss") + jzlsh.Substring(jzlsh.Length - 5, 5);
            byte[] sMsg = new byte[1024];
            byte[] outParam = new byte[1024];
            string sMsg1 = string.Empty;
            string sValue = string.Empty;
            string sfzssf = "000"; //是否正式收费(000 收费预览 001 正式收费)
            string stscpch = string.Empty; //系统上传批次号
            string cfxmsl = "0";
            string sylx = ""; //生育类型
            string sfybfz = ""; //是否有并发症
            bool bfalg = false;

            #region 获取患者登记信息
            string strSql = string.Format("select * from ybmzzydjdr a where a.jzlsh = '{0}' and a.cxbz = 1", jzlsh);

            DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            if (ds == null || ds.Tables[0].Rows.Count == 0)
                return new object[] { 0, 0, "未做医保门诊登记信息" };

            DataRow dr1 = ds.Tables[0].Rows[0];
            string ybbh = dr1["grbh"].ToString();  //医保编号
            string mzh = dr1["ybjzlsh"].ToString();
            string bzbm1 = dr1["bzbm"].ToString(); //病情编码
            string bzbm2 = dr1["mmbzbm1"].ToString(); //病情编码1
            string bzbm3 = dr1["mmbzbm2"].ToString(); //病情编码2
            string tsbzbm = dr1["mmbzbm3"].ToString(); //特殊病种编码
            string xm = dr1["xm"].ToString();
            string kh = dr1["kh"].ToString();
            string chxmsl = "0";
            #endregion

            #region 获取处方明细上传数据
            StringBuilder rc = new StringBuilder();
            strSql = string.Format(@"select y.ybxmbh, y.ybxmmc, m.dj, sum(m.sl) sl, sum(m.je) je, m.yyxmbh, m.yyxmmc, m.ysdm ysdm, n.b1name ysxm, m.ksno ksno,
                                    o.b2ejnm zxks, m.sfno, y.sfxmzldm ybsfxmzldm, y.sflbdm ybsflbdm, m.cfh,y.sfxmdjdm as xmlx,y.yfyb from (
                                    --药品
                                    select a.mcypno yyxmbh, a.mcypnm yyxmmc, a.mcpric dj, a.mcquty sl, a.mcamnt je, a.mcksno ksno, a.mcuser ysdm, a.mcsflb sfno,
                                    a.mccfno cfh from mzcfd a where a.mcghno = '{0}' and a.mccfno in ({1})
                                    union all
                                    --检查/治疗 
                                    select b.mbitem yyxmbh, b.mbname yyxmmc, b.mbpric dj, b.mbquty sl, b.mbsjam je, b.mbksno ksno, b.mbuser ysdm, b.mbsfno sfno,
                                    b.mbzlno cfh from mzb2d b where b.mbghno = '{0}' and b.mbzlno in ({1})
                                    union all
                                    --检验
                                    select c.mbitem yyxmbh, c.mbname yyxmmc, c.mbpric dj, c.mbquty sl, c.mbsjam je, c.mbksno ksno, c.mbuser ysdm, c.mbsfno sfno,
                                    c.mbzlno cfh from mzb4d c where c.mbghno = '{0}' and c.mbzlno in ({1})
                                    union all
                                    --注射
                                    select d.mdwayid yyxmbh, d.mdwayx yyxmmc, d.mdamnt dj, 1 sl, d.mdamnt je, d.mdzsks ksno, d.mdempn ysdm, d.mdsflb sfno,
                                    d.mdcfno cfh from mzd3d d where d.mdcfno in ({1})
                                    union all
                                    --处方划价
                                    select a.ygypno yyxmbh, a.ygypnm yyxmmc, ((a.ygamnt + 0.0) / a.ygslxx) dj, a.ygslxx sl, a.ygamnt je, b.ygksno ksno, 
                                    b.ygysno ysdm, c.y1sflb, a.ygshno cfh from yp17d a join yp17h b on a.ygcomp = b.ygcomp and a.ygshno = b.ygshno
                                    join yp01h c on c.y1ypno = a.ygypno
                                    where b.ygghno = '{0}' and a.ygshno in ({1}) and a.ygslxx > 0
                                    ) m left join bz01h n on m.ysdm = n.b1empn 
                                    left join bz02d o on m.ksno = o.b2ejks
                                    left join ybhisdzdr y on m.yyxmbh = y.hisxmbh
                                    group by y.ybxmbh, y.ybxmmc, m.dj, m.yyxmbh, m.yyxmmc, m.ysdm, n.b1name, m.ksno, o.b2ejnm, m.sfno,y.sfxmzldm,y.sflbdm, m.cfh,y.sfxmdjdm,y.yfyb", jzlsh, cfhs);
            ds.Tables.Clear();
            ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            List<string> li_cfxm = new List<string>(); //添加处方明细
            double ylfze = 0.00;
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                cfxmsl = ds.Tables[0].Rows.Count.ToString();
                DataTable dt = ds.Tables[0];
                StringBuilder wdzxms = new StringBuilder();
                for (int k = 0; k < dt.Rows.Count; k++)
                {
                    DataRow dr = dt.Rows[k];
                    if (dr["ybxmbh"] == DBNull.Value)
                        wdzxms.Append("项目代码：[" + dr["yyxmbh"].ToString() + "]，名称：[" + dr["yyxmmc"].ToString() + "]未对照，不能上传！");
                    else
                    {

                        string ybxmbh = dr["ybxmbh"].ToString();          //医保项目编号
                        string ybxmmc = dr["ybxmmc"].ToString();          //医保项目名称
                        string ybsflbdm = dr["ybsflbdm"].ToString();      //收费类别代码
                        string ybyfyb = dr["yfyb"].ToString();            //是否医保  
                        string ysxm = dr["ysxm"].ToString();            //医生姓名
                        string jx = "";//dr["jx"].ToString();  //剂型
                        string gg = "";//dr["gg"].ToString(); //规格  
                        decimal dj = Convert.ToDecimal(dr["dj"]);//单价
                        decimal sl = Convert.ToDecimal(dr["sl"]);//数量
                        decimal je = Convert.ToDecimal(dr["je"]); //金额  
                        ylfze += Convert.ToDouble(je);
                        decimal mcyl = 1;   //每次用量
                        string zxpc = "1";  //使用频次
                        string fy = "1";    //用法
                        string zxts = "1";  //执行天数
                        string ybsfxmzldm = dr["ybsfxmzldm"].ToString();  //收费项目等级代码
                        string yyxmbh = dr["yyxmbh"].ToString();          //医院项目代码
                        string yyxmmc = dr["yyxmmc"].ToString();          //医院项目名称

                        string ysbm = dr["ysdm"].ToString(); //医生编码
                        string ksdm = dr["ksno"].ToString(); //科室代码
                        string ksmc = dr["zxks"].ToString();  //执行科室
                        string cfh = dr["cfh"].ToString(); //处方号
                        string xmlx = dr["xmlx"].ToString(); //项目类型
                        string ybcfh = cfh + k.ToString();
                        string ypjldw = "-";

                        if (dr["sfno"].ToString() == "01" || dr["sfno"].ToString() == "02" || dr["sfno"].ToString() == "03")
                        {
                            ypjldw = "粒";
                        }
                        string sTmp = ybxmbh + "|" + ybxmmc + "|" + ybsflbdm + "|" + ybyfyb + "|" + ysxm + "|" + jx + "|" + gg + "|" + dj + "|" + sl + "|" + je + "|" + mcyl + "|" + zxpc + "|" + fy + "|" + zxts + "|" + yyxmmc + "|" + yyxmbh + "|" + cfh + "|" + ysbm + "|" + ksdm;
                        li_cfxm.Add(sTmp);
                    }
                }
                if (wdzxms.Length > 0)
                    return new object[] { 0, 0, wdzxms.ToString() };
                if (Math.Abs(sfje2 - ylfze) > 1.0)
                    return new object[] { 0, 0, "收费金额与医保结算金额相差" + Math.Abs(sfje2 - ylfze) + ",无法结算，请核实!" };
            }
            else
                return new object[] { 0, 0, "无费用明细" };
            #endregion

            WriteLog(sysdate + "  进行门诊收费预结算(生育保险)...");
            //入参
            putPara(whandler, "usr", ybuserid);
            putPara(whandler, "pwd", ybpasswd);

            putPara(whandler, "aac001", ybbh);
            putPara(whandler, "akc190", mzh);
           
            putPara(whandler, "aka120", bzbm1);
            putPara(whandler, "skc057", bzbm2);
            putPara(whandler, "skc058", bzbm3);
            putPara(whandler, "skc099", cfxmsl);
            putPara(whandler, "skc516", tsbzbm);
            putPara(whandler, "skc113", "S");
            putPara(whandler, "mkc191", sylx);
            putPara(whandler, "smc052", sfybfz);
            WriteLog("入参|" + ybbh + "|" + mzh + "|" + stscpch + "|" + bzbm1 + "|" + bzbm2 + "|" + bzbm3 + "|" + tsbzbm + "|S|" + sylx + "|" + sfybfz);

            #region 上传明细

            string ybsfxmbh = string.Empty; //收费项目编码  <非空>（医保收费目录编码）  
            string ybsfxmmc = string.Empty; //收费项目名称（医院内部收费项目或者药品名称）
            string ybsflb = string.Empty;   //收费类别
            string sfybxm = string.Empty;   //是否医保项目 (000否，001是)
            string ysxm1 = string.Empty;    //医生姓名
            string jxdw = string.Empty;       //剂型（单位）
            string gg1 = string.Empty;      //规格
            string dj1 = string.Empty;       //单价  <非空>
            string sl1 = string.Empty;      //数量<非空>
            string je1 = string.Empty;      //金额<非空>
            string mcyl1 = string.Empty;    //每次用量
            string sypc = string.Empty;     //使用频次
            string fy1 = string.Empty;      //用法
            string zxts1 = string.Empty;    //执行天数
            string yysfxmbh = string.Empty; //医院项目编码
            string yysfxmmc = string.Empty; //医院项目名称
            string cfh1 = string.Empty;     //处方号
            string ysdm1 = string.Empty;   //医生代码
            string ksno1 = string.Empty; //科室代码

            bfalg = startResultSetName(whandler, "list01");

            foreach (string ss in li_cfxm)
            {
                string[] s = ss.Split('|');
                ybsfxmbh = s[0];
                ybsfxmmc = s[1];
                ybsflb = s[2];
                sfybxm = s[3];
                ysxm1 = s[4];
                jxdw = s[5];
                gg1 = s[6];
                dj1 = s[7];
                sl1 = s[8];
                je1 = s[9];
                mcyl1 = s[10];
                sypc = s[11];
                fy1 = s[12];
                zxts1 = s[13];
                yysfxmbh = s[14];
                yysfxmmc = s[15];
                cfh1 = s[16];
                ysdm1 = s[17];
                ksno1 = s[18];


                WriteLog("上传费用明细-->" + ss);
                putColData(whandler, "aka060", ybsfxmbh);
                putColData(whandler, "ska100", "");
                putColData(whandler, "aka061", ybsfxmmc);
                putColData(whandler, "aka063", ybsflb);
                putColData(whandler, "ska003", sfybxm);
                putColData(whandler, "skc049", ysxm1);
                putColData(whandler, "aka070", jxdw);
                putColData(whandler, "aka074", gg1);
                putColData(whandler, "aka068", dj1);
                putColData(whandler, "akc226", sl1);
                putColData(whandler, "akc227", je1);
                putColData(whandler, "aka071", mcyl1);
                putColData(whandler, "aka072", sypc);
                putColData(whandler, "aka073", fy1);
                putColData(whandler, "akc229", zxts1);
                endcurRow(whandler);
            }
            endCurResultSet(whandler);
            #endregion

            bfalg = process(whandler, "F04.05.10.01");
            if (bfalg)
            {
                getErrMsg(whandler, sMsg);
                sMsg1 = Encoding.Default.GetString(sMsg, 0, sMsg.Length).Replace("\0", "");
                if (sMsg1.ToUpper().Trim().Equals("OK"))
                {
                    #region 出参变量
                    string djh_r = string.Empty;    //单据号
                    string mzh_r = string.Empty;    //门诊号
                    string ybbh_r = string.Empty;   //医保编号
                    string ylrylb_r = string.Empty; //医疗人员类别
                    string kh_r = string.Empty;     //iC卡号
                    string xm_r = string.Empty;       //姓名
                    string bcjsqzhye = string.Empty;//个人帐户余额(不含本次收费)
                    string tcjjzf = string.Empty;   //本次医保基金应支付金额
                    string xjzf = string.Empty;     //本次个人现金应支付金额
                    string zhzf = string.Empty;     //本次个人帐户应支付金额
                    string ylzje = string.Empty;    //本次医疗费总额
                    string bcxdjh = string.Empty;   //被冲销单据号
                    string cxbz_r = string.Empty;   //冲销标志（Z 正常；+ 被冲销；- 冲销）
                    string jbr_r = string.Empty;    //经办人(收费人员)
                    string jbrq = string.Empty;     //经办日期(收费日期)
                    string djdycs = string.Empty;   //单据打印次数
                    string cfxms = string.Empty;    //处方项目数
                    string jsrq = string.Empty;     //结算日期
                    string jsr = string.Empty;      //结算人
                    string grzhye = string.Empty;   //个人帐户余额

                    //收费项目<list01>开始 同入参变量
                    string yftsmzyy = string.Empty; //是否特殊门诊用药（000否，001是）
                    #endregion

                    #region 获取门诊收费返回数据
                    getParaByName(whandler, "aae072", outParam);
                    djh_r = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "akc190", outParam);
                    mzh_r = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "aac001", outParam);
                    ybbh_r = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "akc020", outParam);
                    kh_r = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "aac003", outParam);
                    xm_r = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "akc252", outParam);
                    bcjsqzhye = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "akc260", outParam);
                    tcjjzf = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "akc261", outParam);
                    xjzf = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "akc262", outParam);
                    zhzf = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "akc264", outParam);
                    ylzje = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "skc093", outParam);
                    bcxdjh = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "skc033", outParam);
                    cxbz_r = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "aae011", outParam);
                    jbr_r = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "aae036", outParam);
                    jbrq = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "skc095", outParam);
                    djdycs = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "skc099", outParam);
                    cfxms = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "aae040", outParam);
                    jsrq = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "skc035", outParam);
                    jsr = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "akc087", outParam);
                    grzhye = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    #endregion

                    sValue = ylzje + "|0.00|" + tcjjzf + "|0.00|" + zhzf + "|" + xjzf + "|0.00|0.00|0.00|0.00|" +
                           "0.00|0.00|0.00|0.00|0.00|0.00|0.00|0.00|0.00|0.00|" +
                           "0.00|0.00|0.00|0.00|0.00|" + bcjsqzhye + "|0.00|0.00|0.00|0.00|" +
                            "|0.00||0.00||0.00|" + jsrq + "|||||" + djh_r + "||" +
                           djh + "|" + cxbz_r + "|||||||||||||||";

                    WriteLog(sysdate + "  门诊费用预结算(生育保险)成功|" + sValue);
                    //撤销结算
                    object[] obj = { ybbh_r, djh_r };
                    N_YBMZSFJSCX_SYBX(obj);
                    return new object[] { 0, 1, sValue };
                }
                else
                {
                    WriteLog(sysdate + "  获取门诊收费预结算(生育保险)返回信息失败|" + sMsg1);
                    return new object[] { 0, 0, "获取门诊收费预结算(生育保险)返回信息失败|" + sMsg1 };
                }
            }
            else
            {
                sMsg = new byte[1024];
                getFailReason(whandler, sMsg);
                sMsg1 = Encoding.Default.GetString(sMsg, 0, sMsg.Length).Replace("\0", "");
                WriteLog(sysdate + "  门诊收费预结算(生育保险)失败|" + sMsg1);
                return new object[] { 0, 0, "门诊收费预结算(生育保险)失败|" + sMsg1 };
            }
        }
        #endregion

        #region 门诊收费预结算_ 工伤保险
        public static object[] YBMZSFYJS_GSBX(object[] objParam)
        {
            string sysdate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");//当前时间
            string czygh = CliUtils.fLoginUser;      // 操作员工号
            string jzlsh = objParam[0].ToString();      // 就诊流水号
            string djh = DateTime.Now.ToString("yyyyHHmmss");        // 处方单据号
            string zhsybz = "1";// objParam[1].ToString();     // 账户使用标志（0或1）
            string jbr = CliUtils.fUserName;        // 经办人姓名
            string cfmxjylsh = string.Empty;  // 交易流水号（门诊传处方对应的交易流水号，住院传空字符串）
            string dqrq = objParam[2].ToString();  // 当前日期
            string cyrq = Convert.ToDateTime(dqrq).ToString("yyyy-MM-dd HH:mm:ss");                                     //出院日期

            string cfhs = objParam[5].ToString();   //处方号集
            string ylfhj1 = objParam[7].ToString(); //医疗费合计 (新增)
            string sfymm = string.Empty; //是否有医保卡密码(0,1)
            string jylsh = DateTime.Now.ToString("yyyyMMddHHmmss") + "-102002-" + new Random().Next(100).ToString().PadLeft(4, '0');


            if (string.IsNullOrEmpty(jzlsh))
                return new object[] { 0, 0, "就诊流水号不能为空" };
            if (string.IsNullOrEmpty(jzlsh))
                return new object[] { 0, 0, "就诊流水号不能为空" };
            if (string.IsNullOrEmpty(ylfhj1))
                return new object[] { 0, 0, "医疗合计费不能为空" };
            if (string.IsNullOrEmpty(cfhs))
                return new object[] { 0, 0, "处方号集不能为空" };

            double sfje2 = 0.0000; //金额 
            #region 金额有效性
            try
            {
                sfje2 = Convert.ToDouble(ylfhj1);
            }
            catch
            {
                return new object[] { 0, 0, "收费金额格式错误" };
            }
            #endregion

            string jsdjh = Convert.ToDateTime(dqrq).ToString("yyyyMMddHHmmss") + jzlsh.Substring(jzlsh.Length - 5, 5);
            byte[] sMsg = new byte[1024];
            byte[] outParam = new byte[1024];
            string sMsg1 = string.Empty;
            string sValue = string.Empty;
            string sfzssf = "000"; //是否正式收费(000 收费预览 001 正式收费)
            string stscpch = string.Empty; //系统上传批次号
            string cfxmsl = "0";
            bool bfalg = false;

            #region 获取患者登记信息
            string strSql = string.Format("select * from ybmzzydjdr a where a.jzlsh = '{0}' and a.cxbz = 1", jzlsh);

            DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            if (ds == null || ds.Tables[0].Rows.Count == 0)
                return new object[] { 0, 0, "未做医保门诊登记信息" };

            DataRow dr1 = ds.Tables[0].Rows[0];
            string ybbh = dr1["grbh"].ToString();  //医保编号
            string mzh = dr1["ybjzlsh"].ToString();
            string bzbm1 = dr1["bzbm"].ToString(); //病情编码
            string bzbm2 = dr1["mmbzbm1"].ToString(); //病情编码1
            string bzbm3 = dr1["mmbzbm2"].ToString(); //病情编码2
            string tsbzbm = dr1["mmbzbm3"].ToString(); //特殊病种编码
            string xm = dr1["xm"].ToString();
            string kh = dr1["kh"].ToString();
            string chxmsl = "0";
            #endregion

            #region 获取处方明细上传数据
            StringBuilder rc = new StringBuilder();
            strSql = string.Format(@"select y.ybxmbh, y.ybxmmc, m.dj, sum(m.sl) sl, sum(m.je) je, m.yyxmbh, m.yyxmmc, m.ysdm ysdm, n.b1name ysxm, m.ksno ksno,
                                    o.b2ejnm zxks, m.sfno, y.sfxmzldm ybsfxmzldm, y.sflbdm ybsflbdm, m.cfh,y.sfxmdjdm as xmlx,y.yfyb from (
                                    --药品
                                    select a.mcypno yyxmbh, a.mcypnm yyxmmc, a.mcpric dj, a.mcquty sl, a.mcamnt je, a.mcksno ksno, a.mcuser ysdm, a.mcsflb sfno,
                                    a.mccfno cfh from mzcfd a where a.mcghno = '{0}' and a.mccfno in ({1})
                                    union all
                                    --检查/治疗 
                                    select b.mbitem yyxmbh, b.mbname yyxmmc, b.mbpric dj, b.mbquty sl, b.mbsjam je, b.mbksno ksno, b.mbuser ysdm, b.mbsfno sfno,
                                    b.mbzlno cfh from mzb2d b where b.mbghno = '{0}' and b.mbzlno in ({1})
                                    union all
                                    --检验
                                    select c.mbitem yyxmbh, c.mbname yyxmmc, c.mbpric dj, c.mbquty sl, c.mbsjam je, c.mbksno ksno, c.mbuser ysdm, c.mbsfno sfno,
                                    c.mbzlno cfh from mzb4d c where c.mbghno = '{0}' and c.mbzlno in ({1})
                                    union all
                                    --注射
                                    select d.mdwayid yyxmbh, d.mdwayx yyxmmc, d.mdamnt dj, 1 sl, d.mdamnt je, d.mdzsks ksno, d.mdempn ysdm, d.mdsflb sfno,
                                    d.mdcfno cfh from mzd3d d where d.mdcfno in ({1})
                                    union all
                                    --处方划价
                                    select a.ygypno yyxmbh, a.ygypnm yyxmmc, ((a.ygamnt + 0.0) / a.ygslxx) dj, a.ygslxx sl, a.ygamnt je, b.ygksno ksno, 
                                    b.ygysno ysdm, c.y1sflb, a.ygshno cfh from yp17d a join yp17h b on a.ygcomp = b.ygcomp and a.ygshno = b.ygshno
                                    join yp01h c on c.y1ypno = a.ygypno
                                    where b.ygghno = '{0}' and a.ygshno in ({1}) and a.ygslxx > 0
                                    ) m left join bz01h n on m.ysdm = n.b1empn 
                                    left join bz02d o on m.ksno = o.b2ejks
                                    left join ybhisdzdr y on m.yyxmbh = y.hisxmbh
                                    group by y.ybxmbh, y.ybxmmc, m.dj, m.yyxmbh, m.yyxmmc, m.ysdm, n.b1name, m.ksno, o.b2ejnm, m.sfno,y.sfxmzldm,y.sflbdm, m.cfh,y.sfxmdjdm,y.yfyb", jzlsh, cfhs);
            ds.Tables.Clear();
            ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            List<string> li_cfxm = new List<string>(); //添加处方明细
            double ylfze = 0.00;
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                cfxmsl = ds.Tables[0].Rows.Count.ToString();
                DataTable dt = ds.Tables[0];
                StringBuilder wdzxms = new StringBuilder();
                for (int k = 0; k < dt.Rows.Count; k++)
                {
                    DataRow dr = dt.Rows[k];
                    if (dr["ybxmbh"] == DBNull.Value)
                        wdzxms.Append("项目代码：[" + dr["yyxmbh"].ToString() + "]，名称：[" + dr["yyxmmc"].ToString() + "]未对照，不能上传！");
                    else
                    {

                        string ybxmbh = dr["ybxmbh"].ToString();          //医保项目编号
                        string ybxmmc = dr["ybxmmc"].ToString();          //医保项目名称
                        string ybsflbdm = dr["ybsflbdm"].ToString();      //收费类别代码
                        string ybyfyb = dr["yfyb"].ToString();            //是否医保  
                        string ysxm = dr["ysxm"].ToString();            //医生姓名
                        string jx = "";//dr["jx"].ToString();  //剂型
                        string gg = "";//dr["gg"].ToString(); //规格  
                        decimal dj = Convert.ToDecimal(dr["dj"]);//单价
                        decimal sl = Convert.ToDecimal(dr["sl"]);//数量
                        decimal je = Convert.ToDecimal(dr["je"]); //金额  
                        ylfze += Convert.ToDouble(je);
                        decimal mcyl = 1;   //每次用量
                        string zxpc = "1";  //使用频次
                        string fy = "1";    //用法
                        string zxts = "1";  //执行天数
                        string ybsfxmzldm = dr["ybsfxmzldm"].ToString();  //收费项目等级代码
                        string yyxmbh = dr["yyxmbh"].ToString();          //医院项目代码
                        string yyxmmc = dr["yyxmmc"].ToString();          //医院项目名称

                        string ysbm = dr["ysdm"].ToString(); //医生编码
                        string ksdm = dr["ksno"].ToString(); //科室代码
                        string ksmc = dr["zxks"].ToString();  //执行科室
                        string cfh = dr["cfh"].ToString(); //处方号
                        string xmlx = dr["xmlx"].ToString(); //项目类型
                        string ybcfh = cfh + k.ToString();
                        string ypjldw = "-";

                        if (dr["sfno"].ToString() == "01" || dr["sfno"].ToString() == "02" || dr["sfno"].ToString() == "03")
                        {
                            ypjldw = "粒";
                        }
                        string sTmp = ybxmbh + "|" + ybxmmc + "|" + ybsflbdm + "|" + ybyfyb + "|" + ysxm + "|" + jx + "|" + gg + "|" + dj + "|" + sl + "|" + je + "|" + mcyl + "|" + zxpc + "|" + fy + "|" + zxts + "|" + yyxmmc + "|" + yyxmbh + "|" + cfh + "|" + ysbm + "|" + ksdm;
                        li_cfxm.Add(sTmp);
                    }
                }
                if (wdzxms.Length > 0)
                    return new object[] { 0, 0, wdzxms.ToString() };
                if (Math.Abs(sfje2 - ylfze) > 1.0)
                    return new object[] { 0, 0, "收费金额与医保结算金额相差" + Math.Abs(sfje2 - ylfze) + ",无法结算，请核实!" };
            }
            else
                return new object[] { 0, 0, "无费用明细" };
            #endregion

            WriteLog(sysdate + "  进行门诊收费预结算(工伤保险)...");
            //入参
            putPara(whandler, "usr", ybuserid);
            putPara(whandler, "pwd", ybpasswd);

            putPara(whandler, "aac001", ybbh);
            putPara(whandler, "akc190", mzh);
            putPara(whandler, "aka120", bzbm1);
            putPara(whandler, "skc057", bzbm2);
            putPara(whandler, "skc058", bzbm3);
            putPara(whandler, "skc099", cfxmsl);
            putPara(whandler, "skc516", tsbzbm);
            WriteLog("入参|" + ybbh + "|" + mzh + "|" + stscpch + "|" + bzbm1 + "|" + bzbm2 + "|" + bzbm3 + "|" + tsbzbm );

            #region 上传明细

            string ybsfxmbh = string.Empty; //收费项目编码  <非空>（医保收费目录编码）  
            string ybsfxmmc = string.Empty; //收费项目名称（医院内部收费项目或者药品名称）
            string ybsflb = string.Empty;   //收费类别
            string sfybxm = string.Empty;   //是否医保项目 (000否，001是)
            string ysxm1 = string.Empty;    //医生姓名
            string jxdw = string.Empty;       //剂型（单位）
            string gg1 = string.Empty;      //规格
            string dj1 = string.Empty;       //单价  <非空>
            string sl1 = string.Empty;      //数量<非空>
            string je1 = string.Empty;      //金额<非空>
            string mcyl1 = string.Empty;    //每次用量
            string sypc = string.Empty;     //使用频次
            string fy1 = string.Empty;      //用法
            string zxts1 = string.Empty;    //执行天数
            string yysfxmbh = string.Empty; //医院项目编码
            string yysfxmmc = string.Empty; //医院项目名称
            string cfh1 = string.Empty;     //处方号
            string ysdm1 = string.Empty;   //医生代码
            string ksno1 = string.Empty; //科室代码

            bfalg = startResultSetName(whandler, "list01");

            foreach (string ss in li_cfxm)
            {
                string[] s = ss.Split('|');
                ybsfxmbh = s[0];
                ybsfxmmc = s[1];
                ybsflb = s[2];
                sfybxm = s[3];
                ysxm1 = s[4];
                jxdw = s[5];
                gg1 = s[6];
                dj1 = s[7];
                sl1 = s[8];
                je1 = s[9];
                mcyl1 = s[10];
                sypc = s[11];
                fy1 = s[12];
                zxts1 = s[13];
                yysfxmbh = s[14];
                yysfxmmc = s[15];
                cfh1 = s[16];
                ysdm1 = s[17];
                ksno1 = s[18];


                WriteLog("上传费用明细-->" + ss);
                putColData(whandler, "aka060", ybsfxmbh);
                putColData(whandler, "aka061", ybsfxmmc);
                putColData(whandler, "aka063", ybsflb);
                putColData(whandler, "ska003", sfybxm);
                putColData(whandler, "sla003", "000");
                putColData(whandler, "skc049", ysxm1);
                putColData(whandler, "aka070", jxdw);
                putColData(whandler, "aka074", gg1);
                putColData(whandler, "aka068", dj1);
                putColData(whandler, "akc226", sl1);
                putColData(whandler, "akc227", je1);
                putColData(whandler, "aka071", mcyl1);
                putColData(whandler, "aka072", sypc);
                putColData(whandler, "aka073", fy1);
                putColData(whandler, "akc229", zxts1);
                endcurRow(whandler);
            }
            endCurResultSet(whandler);
            #endregion

            bfalg = process(whandler, "F07.11.06.01");
            if (bfalg)
            {
                getErrMsg(whandler, sMsg);
                sMsg1 = Encoding.Default.GetString(sMsg, 0, sMsg.Length).Replace("\0", "");
                if (sMsg1.ToUpper().Trim().Equals("OK"))
                {
                    #region 出参变量
                    string djh_r = string.Empty;    //单据号
                    string mzh_r = string.Empty;    //门诊号
                    string ybbh_r = string.Empty;   //医保编号
                    string ylrylb_r = string.Empty; //医疗人员类别
                    string kh_r = string.Empty;     //iC卡号
                    string xm_r = string.Empty;       //姓名
                    string bcjsqzhye = string.Empty;//个人帐户余额(不含本次收费)
                    string tcjjzf = string.Empty;   //本次医保基金应支付金额
                    string xjzf = string.Empty;     //本次个人现金应支付金额
                    string zhzf = string.Empty;     //本次个人帐户应支付金额
                    string ylzje = string.Empty;    //本次医疗费总额
                    string bcxdjh = string.Empty;   //被冲销单据号
                    string cxbz_r = string.Empty;   //冲销标志（Z 正常；+ 被冲销；- 冲销）
                    string jbr_r = string.Empty;    //经办人(收费人员)
                    string jbrq = string.Empty;     //经办日期(收费日期)
                    string djdycs = string.Empty;   //单据打印次数
                    string cfxms = string.Empty;    //处方项目数
                    string jsrq = string.Empty;     //结算日期
                    string jsr = string.Empty;      //结算人
                    string grzhye = string.Empty;   //个人帐户余额

                    //收费项目<list01>开始 同入参变量
                    string yftsmzyy = string.Empty; //是否特殊门诊用药（000否，001是）
                    #endregion

                    #region 获取门诊收费返回数据
                    getParaByName(whandler, "aae072", outParam);
                    djh_r = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "akc190", outParam);
                    mzh_r = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "aac001", outParam);
                    ybbh_r = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "akc020", outParam);
                    kh_r = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "aac003", outParam);
                    xm_r = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    //outParam = new byte[1024];
                    //getParaByName(whandler, "akc252", outParam);
                    //bcjsqzhye = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "akc260", outParam);
                    tcjjzf = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "akc261", outParam);
                    xjzf = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "akc262", outParam);
                    zhzf = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "akc264", outParam);
                    ylzje = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "skc093", outParam);
                    bcxdjh = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "skc033", outParam);
                    cxbz_r = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "aae011", outParam);
                    jbr_r = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "aae036", outParam);
                    jbrq = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "skc095", outParam);
                    djdycs = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "skc099", outParam);
                    cfxms = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "aae040", outParam);
                    jsrq = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "skc035", outParam);
                    jsr = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "akc087", outParam);
                    grzhye = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    #endregion

                    sValue = ylzje + "|0.00|" + tcjjzf + "|0.00|" + zhzf + "|" + xjzf + "|0.00|0.00|0.00|0.00|" +
                           "0.00|0.00|0.00|0.00|0.00|0.00|0.00|0.00|0.00|0.00|" +
                           "0.00|0.00|0.00|0.00|0.00|" + bcjsqzhye + "|0.00|0.00|0.00|0.00|" +
                            "|0.00||0.00||0.00|" + jsrq + "|||||" + djh_r + "||" +
                           djh + "|" + cxbz_r + "|||||||||||||||";

                    WriteLog(sysdate + "  门诊费用预结算(工伤保险)成功|" + sValue);
                    //撤销结算
                    object[] obj = { ybbh, djh_r };
                    N_YBMZSFJSCX_GSBX(obj);
                    return new object[] { 0, 1, sValue };
                }
                else
                {
                    WriteLog(sysdate + "  获取门诊收费预结算(工伤保险)返回信息失败|" + sMsg1);
                    return new object[] { 0, 0, "获取门诊收费预结算(工伤保险)返回信息失败|" + sMsg1 };
                }
            }
            else
            {
                sMsg = new byte[1024];
                getFailReason(whandler, sMsg);
                sMsg1 = Encoding.Default.GetString(sMsg, 0, sMsg.Length).Replace("\0", "");
                WriteLog(sysdate + "  门诊收费预结算(工伤保险)失败|" + sMsg1);
                return new object[] { 0, 0, "门诊收费预结算(工伤保险)失败|" + sMsg1 };
            }
        }
        #endregion


        #region 门诊收费结算
        public static object[] YBMZSFJS(object[] objParam)
        {
            string jzlsh = objParam[0].ToString();
            string sxzbm = string.Empty;
            if (string.IsNullOrEmpty(jzlsh))
                return new object[] { 0, 0, "就诊流水号不能为空" };

            string strSql = string.Format(@"select xzbm from ybmzzydjdr where jzlsh='{0}' and cxbz=1", objParam[0].ToString());
            DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            if (ds == null || ds.Tables[0].Rows.Count == 0)
                return new object[] { 0, 0, "未做医保门诊登记信息" };
            sxzbm = ds.Tables[0].Rows[0]["xzbm"].ToString();
            object[] objReturn = null;
            switch (sxzbm)
            {
                case "310":
                    objReturn = YBMZSFJS_YLBX(objParam);
                    break;
                case "410":
                    objReturn = YBMZSFJS_GSBX(objParam);
                    break;
                case "510":
                    objReturn = YBMZSFJS_SYBX(objParam);
                    break;
                default:
                    objReturn = new object[] { 0, 0, "门诊费用结算失败|未找到结算体" };
                    break;
            }
            return objReturn;

        }
        #endregion

        #region 门诊收费结算_医疗保险
        public static object[] YBMZSFJS_YLBX(object[] objParam)
        {
            string sysdate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");//当前时间
            string czygh = CliUtils.fLoginUser;      // 操作员工号
            string jzlsh = objParam[0].ToString();      // 就诊流水号
            string djh = objParam[1].ToString();        // 处方单据号
            string cyyy = "";       // 出院原因代码 
            string zhsybz = "1";// objParam[2].ToString();     // 账户使用标志（0或1）
            string jbr = CliUtils.fUserName;        // 经办人姓名
            string cfmxjylsh = string.Empty;  // 交易流水号（门诊传处方对应的交易流水号，住院传空字符串）
            string dqrq = objParam[3].ToString();  // 当前日期
            string cyrq = Convert.ToDateTime(dqrq).ToString("yyyyMMddHHmmss");                                     //出院日期
            string jsdjh = cyrq + jzlsh.Substring(jzlsh.Length - 5, 5);    //结算单据号 门诊以12位时间+流水号后5位作为单据号
            string cfhs = objParam[6].ToString();   //处方号集
            string ylfhj1 = objParam[8].ToString(); //医疗费合计 (新增)
            string sfymm = string.Empty; //是否有医保卡密码(0,1)
            string jylsh = DateTime.Now.ToString("yyyyMMddHHmmss") + "-102002-" + new Random().Next(100).ToString().PadLeft(4, '0');
            string ybjzlsh = jzlsh + DateTime.Now.ToString("HHmmss");



            if (string.IsNullOrEmpty(jzlsh))
                return new object[] { 0, 0, "就诊流水号不能为空" };
            if (string.IsNullOrEmpty(jzlsh))
                return new object[] { 0, 0, "就诊流水号不能为空" };
            if (string.IsNullOrEmpty(ylfhj1))
                return new object[] { 0, 0, "医疗合计费不能为空" };
            if (string.IsNullOrEmpty(cfhs))
                return new object[] { 0, 0, "处方号集不能为空" };

            byte[] sMsg = new byte[1024];
            byte[] outParam = new byte[1024];
            string sMsg1 = string.Empty;
            string sValue = string.Empty;
            string sfzssf = "001"; //是否正式收费(000 收费预览 001 正式收费)
            string stscpch = string.Empty; //系统上传批次号
            string cfxmsl = "0";
            bool bfalg = false;


            double sfje2 = 0.0000; //金额 
            #region 金额有效性
            try
            {
                sfje2 = Convert.ToDouble(ylfhj1);
            }
            catch
            {
                return new object[] { 0, 0, "收费金额格式错误" };
            }
            #endregion

            #region 获取患者登记信息
            string strSql = string.Format("select * from ybmzzydjdr a where a.jzlsh = '{0}' and a.cxbz = 1", jzlsh);

            DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            if (ds == null || ds.Tables[0].Rows.Count == 0)
                return new object[] { 0, 0, "未做医保门诊登记信息" };

            DataRow dr1 = ds.Tables[0].Rows[0];
            string ybbh = dr1["grbh"].ToString();  //医保编号
            string mzh = dr1["ybjzlsh"].ToString();
            string bzbm1 = dr1["bzbm"].ToString(); //病情编码
            string bzbm2 = dr1["mmbzbm1"].ToString(); //病情编码1
            string bzbm3 = dr1["mmbzbm2"].ToString(); //病情编码2
            string tsbzbm = dr1["mmbzbm3"].ToString(); //特殊病种编码
            string xm = dr1["xm"].ToString();
            string kh = dr1["kh"].ToString();
            string chxmsl = "0";
            #endregion

            #region 获取处方明细上传数据
            StringBuilder rc = new StringBuilder();
            strSql = string.Format(@"select y.ybxmbh, y.ybxmmc, m.dj, sum(m.sl) sl, sum(m.je) je, m.yyxmbh, m.yyxmmc, m.ysdm ysdm, n.b1name ysxm, m.ksno ksno,
                                    o.b2ejnm zxks, m.sfno, y.sfxmzldm ybsfxmzldm, y.sflbdm ybsflbdm, m.cfh,y.sfxmdjdm as xmlx,y.yfyb from (
                                    --药品
                                    select a.mcypno yyxmbh, a.mcypnm yyxmmc, a.mcpric dj, a.mcquty sl, a.mcamnt je, a.mcksno ksno, a.mcuser ysdm, a.mcsflb sfno,
                                    a.mccfno cfh from mzcfd a where a.mcghno = '{0}' and a.mccfno in ({1})
                                    union all
                                    --检查/治疗 
                                    select b.mbitem yyxmbh, b.mbname yyxmmc, b.mbpric dj, b.mbquty sl, b.mbsjam je, b.mbksno ksno, b.mbuser ysdm, b.mbsfno sfno,
                                    b.mbzlno cfh from mzb2d b where b.mbghno = '{0}' and b.mbzlno in ({1})
                                    union all
                                    --检验
                                    select c.mbitem yyxmbh, c.mbname yyxmmc, c.mbpric dj, c.mbquty sl, c.mbsjam je, c.mbksno ksno, c.mbuser ysdm, c.mbsfno sfno,
                                    c.mbzlno cfh from mzb4d c where c.mbghno = '{0}' and c.mbzlno in ({1})
                                    union all
                                    --注射
                                    select d.mdwayid yyxmbh, d.mdwayx yyxmmc, d.mdamnt dj, 1 sl, d.mdamnt je, d.mdzsks ksno, d.mdempn ysdm, d.mdsflb sfno,
                                    d.mdcfno cfh from mzd3d d where d.mdcfno in ({1})
                                    union all
                                    --处方划价
                                    select a.ygypno yyxmbh, a.ygypnm yyxmmc, ((a.ygamnt + 0.0) / a.ygslxx) dj, a.ygslxx sl, a.ygamnt je, b.ygksno ksno, 
                                    b.ygysno ysdm, c.y1sflb, a.ygshno cfh from yp17d a join yp17h b on a.ygcomp = b.ygcomp and a.ygshno = b.ygshno
                                    join yp01h c on c.y1ypno = a.ygypno
                                    where b.ygghno = '{0}' and a.ygshno in ({1}) and a.ygslxx > 0
                                    ) m left join bz01h n on m.ysdm = n.b1empn 
                                    left join bz02d o on m.ksno = o.b2ejks
                                    left join ybhisdzdr y on m.yyxmbh = y.hisxmbh
                                    group by y.ybxmbh, y.ybxmmc, m.dj, m.yyxmbh, m.yyxmmc, m.ysdm, n.b1name, m.ksno, o.b2ejnm, m.sfno,y.sfxmzldm,y.sflbdm, m.cfh,y.sfxmdjdm,y.yfyb", jzlsh, cfhs);
            ds.Tables.Clear();
            ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            List<string> li_cfxm = new List<string>(); //添加处方明细
            double ylfze = 0.00;
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                cfxmsl = ds.Tables[0].Rows.Count.ToString();
                DataTable dt = ds.Tables[0];
                StringBuilder wdzxms = new StringBuilder();
                for (int k = 0; k < dt.Rows.Count; k++)
                {
                    DataRow dr = dt.Rows[k];
                    if (dr["ybxmbh"] == DBNull.Value)
                        wdzxms.Append("项目代码：[" + dr["yyxmbh"].ToString() + "]，名称：[" + dr["yyxmmc"].ToString() + "]未对照，不能上传！");
                    else
                    {
                        string ybxmbh = dr["ybxmbh"].ToString();          //医保项目编号
                        string ybxmmc = dr["ybxmmc"].ToString();          //医保项目名称
                        string ybsflbdm = dr["ybsflbdm"].ToString();      //收费类别代码
                        string ybyfyb = dr["yfyb"].ToString();            //是否医保  
                        string ysxm = dr["ysxm"].ToString();            //医生姓名
                        string jx = "";//dr["jx"].ToString();  //剂型
                        string gg = "";//dr["gg"].ToString(); //规格  
                        decimal dj = Convert.ToDecimal(dr["dj"]);//单价
                        decimal sl = Convert.ToDecimal(dr["sl"]);//数量
                        decimal je = Convert.ToDecimal(dr["je"]); //金额  
                        ylfze += Convert.ToDouble(je);
                        decimal mcyl = 1;   //每次用量
                        string zxpc = "1";  //使用频次
                        string fy = "1";    //用法
                        string zxts = "1";  //执行天数
                        string ybsfxmzldm = dr["ybsfxmzldm"].ToString();  //收费项目等级代码
                        string yyxmbh = dr["yyxmbh"].ToString();          //医院项目代码
                        string yyxmmc = dr["yyxmmc"].ToString();          //医院项目名称

                        string ysbm = dr["ysdm"].ToString(); //医生编码
                        string ksdm = dr["ksno"].ToString(); //科室代码
                        string ksmc = dr["zxks"].ToString();  //执行科室
                        string cfh = dr["cfh"].ToString(); //处方号
                        string xmlx = dr["xmlx"].ToString(); //项目类型
                        string ybcfh = cfh + k.ToString();
                        string ypjldw = "-";

                        if (dr["sfno"].ToString() == "01" || dr["sfno"].ToString() == "02" || dr["sfno"].ToString() == "03")
                        {
                            ypjldw = "粒";
                        }
                        string sTmp = ybxmbh + "|" + ybxmmc + "|" + ybsflbdm + "|" + ybyfyb + "|" + ysxm + "|" + jx + "|" + gg + "|" + dj + "|" + sl + "|" + je + "|" + mcyl + "|" + zxpc + "|" + fy + "|" + zxts + "|" + yyxmmc + "|" + yyxmbh + "|" + cfh + "|" + ysbm + "|" + ksdm;
                        li_cfxm.Add(sTmp);
                    }
                }
                if (wdzxms.Length > 0)
                    return new object[] { 0, 0, wdzxms.ToString() };
                if (Math.Abs(sfje2 - ylfze) > 1.0)
                    return new object[] { 0, 0, "收费金额与医保结算金额相差" + Math.Abs(sfje2 - ylfze) + ",无法结算，请核实!" };
            }
            else
                return new object[] { 0, 0, "无费用明细" };
            #endregion

            #region 获取批次号
            strSql = string.Format("select * from yb_pcxx where bz=1 and jzbz='m'");
            ds.Tables.Clear();
            ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            string pch = (ds.Tables[0].Rows.Count + 1).ToString();
            stscpch = "MZ" + pch.PadLeft(8, '0');
            #endregion

            WriteLog(sysdate + "  进行门诊收费预结算(医疗保险)...");
            List<string> liSQL = new List<string>();
            //入参
            putPara(whandler, "usr", ybuserid);
            putPara(whandler, "pwd", ybpasswd);

            putPara(whandler, "aac001", ybbh);
            putPara(whandler, "akc190", mzh);
            putPara(whandler, "skc600", stscpch);
            putPara(whandler, "aka120", bzbm1);
            putPara(whandler, "skc057", bzbm2);
            putPara(whandler, "skc058", bzbm3);
            putPara(whandler, "skc099", cfxmsl);
            putPara(whandler, "skc516", tsbzbm);
            putPara(whandler, "skc098", sfzssf);
            WriteLog("入参|" + ybbh + "|" + mzh + "|" + stscpch + "|" + bzbm1 + "|" + bzbm2 + "|" + bzbm3 + "|" + tsbzbm + "|" + sfzssf);
            //上传明细

            string ybsfxmbh = string.Empty; //收费项目编码  <非空>（医保收费目录编码）  
            string ybsfxmmc = string.Empty; //收费项目名称（医院内部收费项目或者药品名称）
            string ybsflb = string.Empty;   //收费类别
            string sfybxm = string.Empty;   //是否医保项目 (000否，001是)
            string ysxm1 = string.Empty;    //医生姓名
            string jxdw = string.Empty;       //剂型（单位）
            string gg1 = string.Empty;      //规格
            string dj1 = string.Empty;       //单价  <非空>
            string sl1 = string.Empty;      //数量<非空>
            string je1 = string.Empty;      //金额<非空>
            string mcyl1 = string.Empty;    //每次用量
            string sypc = string.Empty;     //使用频次
            string fy1 = string.Empty;      //用法
            string zxts1 = string.Empty;    //执行天数
            string yysfxmbh = string.Empty; //医院项目编码
            string yysfxmmc = string.Empty; //医院项目名称
            string cfh1 = string.Empty;     //处方号
            string ysdm1 = string.Empty;   //医生代码
            string ksno1 = string.Empty; //科室代码

            startResultSetName(whandler, "list01");

            foreach (string ss in li_cfxm)
            {
                string[] s = ss.Split('|');
                ybsfxmbh = s[0];
                ybsfxmmc = s[1];
                ybsflb = s[2];
                sfybxm = s[3];
                ysxm1 = s[4];
                jxdw = s[5];
                gg1 = s[6];
                dj1 = s[7];
                sl1 = s[8];
                je1 = s[9];
                mcyl1 = s[10];
                sypc = s[11];
                fy1 = s[12];
                zxts1 = s[13];
                yysfxmbh = s[14];
                yysfxmmc = s[15];
                cfh1 = s[16];
                ysdm1 = s[17];
                ksno1 = s[18];


                WriteLog("上传费用明细-->" + s);
                putColData(whandler, "aka060", ybsfxmbh);
                putColData(whandler, "aka061", ybsfxmmc);
                putColData(whandler, "aka063", ybsflb);
                putColData(whandler, "ska003", sfybxm);
                putColData(whandler, "skc049", ysxm1);
                putColData(whandler, "aka070", jxdw);
                putColData(whandler, "aka074", gg1);
                putColData(whandler, "aka068", dj1);
                putColData(whandler, "akc226", sl1);
                putColData(whandler, "akc227", je1);
                putColData(whandler, "aka071", mcyl1);
                putColData(whandler, "aka072", sypc);
                putColData(whandler, "aka073", fy1);
                putColData(whandler, "akc229", zxts1);
                endcurRow(whandler);

                strSql = string.Format(@"insert into ybcfmxscindr( jzlsh,jylsh,ybcfh,yysfxmbm,yysfxmmc,sfxmzxbm,ybxmmc,dj,sl,je,
                                        mcyl,sypc,ysxm,ysbm,ksbh,yf,jbr,xm,kh,ybjzlsh,sysdate,scpch) values(
                                        '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}',
                                        '{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}','{20}','{21}')",
                                        jzlsh, jylsh, cfh1, yysfxmbh, yysfxmmc, ybsfxmbh, ybsfxmmc, dj1, sl1, je1,
                                        mcyl1, sypc, ysxm1, ysdm1, ksno1, fy1, jbr, xm, kh, mzh, sysdate, stscpch);
                liSQL.Add(strSql);

            }
            endCurResultSet(whandler);

            //插入批次号
            strSql = string.Format(@"insert into yb_pcxx(pch,bz,djh,jzbz) values('{0}','{1}','{2}','{3}')", pch, "1", djh, "m");
            liSQL.Add(strSql);

            bfalg = process(whandler, "F04.05.10.01");
            if (bfalg)
            {
                getErrMsg(whandler, sMsg);
                sMsg1 = Encoding.Default.GetString(sMsg, 0, sMsg.Length).Replace("\0", "");
                if (sMsg1.Trim().Equals("OK"))
                {
                    #region 出参变量
                    string djh_r = string.Empty;    //单据号
                    string mzh_r = string.Empty;    //门诊号
                    string ybbh_r = string.Empty;   //医保编号
                    string ylrylb_r = string.Empty; //医疗人员类别
                    string kh_r = string.Empty;     //iC卡号
                    string xm_r = string.Empty;       //姓名
                    string bcjsqzhye = string.Empty;//个人帐户余额(不含本次收费)
                    string tcjjzf = string.Empty;   //本次医保基金应支付金额
                    string xjzf = string.Empty;     //本次个人现金应支付金额
                    string zhzf = string.Empty;     //本次个人帐户应支付金额
                    string ylzje = string.Empty;    //本次医疗费总额
                    string bcxdjh = string.Empty;   //被冲销单据号
                    string cxbz_r = string.Empty;   //冲销标志（Z 正常；+ 被冲销；- 冲销）
                    string jbr_r = string.Empty;    //经办人(收费人员)
                    string jbrq = string.Empty;     //经办日期(收费日期)
                    string djdycs = string.Empty;   //单据打印次数
                    string cfxms = string.Empty;    //处方项目数
                    string jsrq = string.Empty;     //结算日期
                    string jsr = string.Empty;      //结算人
                    string grzhye = string.Empty;   //个人帐户余额

                    //收费项目<list01>开始 同入参变量
                    string yftsmzyy = string.Empty; //是否特殊门诊用药（000否，001是）
                    #endregion

                    #region 获取门诊收费返回数据
                    getParaByName(whandler, "aae072", outParam);
                    djh_r = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1204];
                    getParaByName(whandler, "akc190", outParam);
                    mzh_r = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1204];
                    getParaByName(whandler, "aac001", outParam);
                    ybbh_r = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1204];
                    getParaByName(whandler, "akc020", outParam);
                    kh_r = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1204];
                    getParaByName(whandler, "aac003", outParam);
                    xm_r = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1204];
                    getParaByName(whandler, "akc252", outParam);
                    bcjsqzhye = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1204];
                    getParaByName(whandler, "akc260", outParam);
                    tcjjzf = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1204];
                    getParaByName(whandler, "akc261", outParam);
                    xjzf = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1204];
                    getParaByName(whandler, "akc262", outParam);
                    zhzf = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1204];
                    getParaByName(whandler, "akc264", outParam);
                    ylzje = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1204];
                    getParaByName(whandler, "skc093", outParam);
                    bcxdjh = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1204];
                    getParaByName(whandler, "skc033", outParam);
                    cxbz_r = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1204];
                    getParaByName(whandler, "aae011", outParam);
                    jbr_r = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1204];
                    getParaByName(whandler, "aae036", outParam);
                    jbrq = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1204];
                    getParaByName(whandler, "skc095", outParam);
                    djdycs = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1204];
                    getParaByName(whandler, "skc099", outParam);
                    cfxms = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1204];
                    getParaByName(whandler, "aae040", outParam);
                    jsrq = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1204];
                    getParaByName(whandler, "skc035", outParam);
                    jsr = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1204];
                    getParaByName(whandler, "akc087", outParam);
                    grzhye = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    #endregion

                    sValue = ylzje + "|0.00|" + tcjjzf + "|0.00|" + zhzf + "|" + xjzf + "|0.00|0.00|0.00|0.00|" +
                           "0.00|0.00|0.00|0.00|0.00|0.00|0.00|0.00|0.00|0.00|" +
                           "0.00|0.00|0.00|0.00|0.00|" + bcjsqzhye + "|0.00|0.00|0.00|0.00|" +
                            "|0.00||0.00||0.00|" + jsrq + "|||||" + djh_r + "||" +
                           djh + "|" + cxbz_r + "|||||||||||||||" + bcxdjh;
                    strSql = string.Format(@"insert into ybfyjsdr( jzlsh,jylsh,jbr,ylfze,tcjjzf,zhzf,xjzf,djh,djhin,grbh,
                                            kh,xm,bcjsqzhye,cxbz1,jbrq,jsrq,jsr,grzfye,cfmxs,ybdjh,sysdate,ybjzlsh) values(
                                            '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}',
                                            '{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}','{20}','{21}')",
                                            jzlsh, jylsh, jbr_r, ylzje, tcjjzf, zhzf, xjzf, djh, bcxdjh, ybbh_r,
                                            kh_r, xm_r, bcjsqzhye, cxbz_r, jbrq, jsrq, jsr, grzhye, cfxms, djh_r, sysdate, mzh_r);
                    liSQL.Add(strSql);
                    object[] obj = liSQL.ToArray();
                    obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);
                    if (obj[1].ToString().Equals("1"))
                    {
                        WriteLog(sysdate + "  门诊费用预结算(医疗保险)成功|" + sValue);
                        return new object[] { 0, 1, sValue };
                    }
                    else
                    {
                        WriteLog(sysdate + "  门诊费用预结算(医疗保险)失败｜操作数据失败|" + obj[2].ToString());
                        //撤销结算
                        object[] obj1 = { ybbh_r, djh_r };
                        N_YBMZSFJSCX_YLBX(obj1);
                        return new object[] { 0, 0, "  门诊费用预结算(医疗保险)失败｜操作数据失败|" + obj[2].ToString() };
                    }
                }
                else
                {
                    WriteLog(sysdate + "  获取门诊收费预结算(医疗保险)返回信息失败|" + sMsg1);
                    return new object[] { 0, 0, "获取门诊收费预结算(医疗保险)返回信息失败|" + sMsg1 };
                }
            }
            else
            {
                sMsg = new byte[1024];
                getFailReason(whandler, sMsg);
                sMsg1 = Encoding.Default.GetString(sMsg, 0, sMsg.Length).Replace("\0", "");
                WriteLog(sysdate + "  门诊收费结算(医疗保险)失败|" + sMsg1);
                return new object[] { 0, 0, "门诊收费结算(医疗保险)失败|" + sMsg1 };
            }
        }
        #endregion

        #region 门诊收费结算_生育保险
        public static object[] YBMZSFJS_SYBX(object[] objParam)
        {
            string sysdate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");//当前时间
            string czygh = CliUtils.fLoginUser;      // 操作员工号
            string jzlsh = objParam[0].ToString();      // 就诊流水号
            string djh = objParam[1].ToString();        // 处方单据号
            string cyyy = "";       // 出院原因代码 
            string zhsybz = "1";// objParam[2].ToString();     // 账户使用标志（0或1）
            string jbr = CliUtils.fUserName;        // 经办人姓名
            string cfmxjylsh = string.Empty;  // 交易流水号（门诊传处方对应的交易流水号，住院传空字符串）
            string dqrq = objParam[3].ToString();  // 当前日期
            string cyrq = Convert.ToDateTime(dqrq).ToString("yyyyMMddHHmmss");                                     //出院日期
            string jsdjh = cyrq + jzlsh.Substring(jzlsh.Length - 5, 5);    //结算单据号 门诊以12位时间+流水号后5位作为单据号
            string cfhs = objParam[6].ToString();   //处方号集
            string ylfhj1 = objParam[8].ToString(); //医疗费合计 (新增)
            string sfymm = string.Empty; //是否有医保卡密码(0,1)
            string jylsh = DateTime.Now.ToString("yyyyMMddHHmmss") + "-102002-" + new Random().Next(100).ToString().PadLeft(4, '0');
            string ybjzlsh = jzlsh + DateTime.Now.ToString("HHmmss");



            if (string.IsNullOrEmpty(jzlsh))
                return new object[] { 0, 0, "就诊流水号不能为空" };
            if (string.IsNullOrEmpty(jzlsh))
                return new object[] { 0, 0, "就诊流水号不能为空" };
            if (string.IsNullOrEmpty(ylfhj1))
                return new object[] { 0, 0, "医疗合计费不能为空" };
            if (string.IsNullOrEmpty(cfhs))
                return new object[] { 0, 0, "处方号集不能为空" };

            byte[] sMsg = new byte[1024];
            byte[] outParam = new byte[1024];
            string sMsg1 = string.Empty;
            string sValue = string.Empty;
            string sfzssf = "001"; //是否正式收费(000 收费预览 001 正式收费)
            string stscpch = string.Empty; //系统上传批次号
            string cfxmsl = "0";
            string sylx = ""; //生育类型
            string sfybfz = ""; //是否有并发症
            bool bfalg = false;


            double sfje2 = 0.0000; //金额 
            #region 金额有效性
            try
            {
                sfje2 = Convert.ToDouble(ylfhj1);
            }
            catch
            {
                return new object[] { 0, 0, "收费金额格式错误" };
            }
            #endregion

            #region 获取患者登记信息
            string strSql = string.Format("select * from ybmzzydjdr a where a.jzlsh = '{0}' and a.cxbz = 1", jzlsh);

            DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            if (ds == null || ds.Tables[0].Rows.Count == 0)
                return new object[] { 0, 0, "未做医保门诊登记信息" };

            DataRow dr1 = ds.Tables[0].Rows[0];
            string ybbh = dr1["grbh"].ToString();  //医保编号
            string mzh = dr1["ybjzlsh"].ToString();
            string bzbm1 = dr1["bzbm"].ToString(); //病情编码
            string bzbm2 = dr1["mmbzbm1"].ToString(); //病情编码1
            string bzbm3 = dr1["mmbzbm2"].ToString(); //病情编码2
            string tsbzbm = dr1["mmbzbm3"].ToString(); //特殊病种编码
            string xm = dr1["xm"].ToString();
            string kh = dr1["kh"].ToString();
            string chxmsl = "0";
            #endregion

            #region 获取处方明细上传数据
            StringBuilder rc = new StringBuilder();
            strSql = string.Format(@"select y.ybxmbh, y.ybxmmc, m.dj, sum(m.sl) sl, sum(m.je) je, m.yyxmbh, m.yyxmmc, m.ysdm ysdm, n.b1name ysxm, m.ksno ksno,
                                    o.b2ejnm zxks, m.sfno, y.sfxmzldm ybsfxmzldm, y.sflbdm ybsflbdm, m.cfh,y.sfxmdjdm as xmlx,y.yfyb from (
                                    --药品
                                    select a.mcypno yyxmbh, a.mcypnm yyxmmc, a.mcpric dj, a.mcquty sl, a.mcamnt je, a.mcksno ksno, a.mcuser ysdm, a.mcsflb sfno,
                                    a.mccfno cfh from mzcfd a where a.mcghno = '{0}' and a.mccfno in ({1})
                                    union all
                                    --检查/治疗 
                                    select b.mbitem yyxmbh, b.mbname yyxmmc, b.mbpric dj, b.mbquty sl, b.mbsjam je, b.mbksno ksno, b.mbuser ysdm, b.mbsfno sfno,
                                    b.mbzlno cfh from mzb2d b where b.mbghno = '{0}' and b.mbzlno in ({1})
                                    union all
                                    --检验
                                    select c.mbitem yyxmbh, c.mbname yyxmmc, c.mbpric dj, c.mbquty sl, c.mbsjam je, c.mbksno ksno, c.mbuser ysdm, c.mbsfno sfno,
                                    c.mbzlno cfh from mzb4d c where c.mbghno = '{0}' and c.mbzlno in ({1})
                                    union all
                                    --注射
                                    select d.mdwayid yyxmbh, d.mdwayx yyxmmc, d.mdamnt dj, 1 sl, d.mdamnt je, d.mdzsks ksno, d.mdempn ysdm, d.mdsflb sfno,
                                    d.mdcfno cfh from mzd3d d where d.mdcfno in ({1})
                                    union all
                                    --处方划价
                                    select a.ygypno yyxmbh, a.ygypnm yyxmmc, ((a.ygamnt + 0.0) / a.ygslxx) dj, a.ygslxx sl, a.ygamnt je, b.ygksno ksno, 
                                    b.ygysno ysdm, c.y1sflb, a.ygshno cfh from yp17d a join yp17h b on a.ygcomp = b.ygcomp and a.ygshno = b.ygshno
                                    join yp01h c on c.y1ypno = a.ygypno
                                    where b.ygghno = '{0}' and a.ygshno in ({1}) and a.ygslxx > 0
                                    ) m left join bz01h n on m.ysdm = n.b1empn 
                                    left join bz02d o on m.ksno = o.b2ejks
                                    left join ybhisdzdr y on m.yyxmbh = y.hisxmbh
                                    group by y.ybxmbh, y.ybxmmc, m.dj, m.yyxmbh, m.yyxmmc, m.ysdm, n.b1name, m.ksno, o.b2ejnm, m.sfno,y.sfxmzldm,y.sflbdm, m.cfh,y.sfxmdjdm,y.yfyb", jzlsh, cfhs);
            ds.Tables.Clear();
            ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            List<string> li_cfxm = new List<string>(); //添加处方明细
            double ylfze = 0.00;
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                cfxmsl = ds.Tables[0].Rows.Count.ToString();
                DataTable dt = ds.Tables[0];
                StringBuilder wdzxms = new StringBuilder();
                for (int k = 0; k < dt.Rows.Count; k++)
                {
                    DataRow dr = dt.Rows[k];
                    if (dr["ybxmbh"] == DBNull.Value)
                        wdzxms.Append("项目代码：[" + dr["yyxmbh"].ToString() + "]，名称：[" + dr["yyxmmc"].ToString() + "]未对照，不能上传！");
                    else
                    {
                        string ybxmbh = dr["ybxmbh"].ToString();          //医保项目编号
                        string ybxmmc = dr["ybxmmc"].ToString();          //医保项目名称
                        string ybsflbdm = dr["ybsflbdm"].ToString();      //收费类别代码
                        string ybyfyb = dr["yfyb"].ToString();            //是否医保  
                        string ysxm = dr["ysxm"].ToString();            //医生姓名
                        string jx = "";//dr["jx"].ToString();  //剂型
                        string gg = "";//dr["gg"].ToString(); //规格  
                        decimal dj = Convert.ToDecimal(dr["dj"]);//单价
                        decimal sl = Convert.ToDecimal(dr["sl"]);//数量
                        decimal je = Convert.ToDecimal(dr["je"]); //金额  
                        ylfze += Convert.ToDouble(je);
                        decimal mcyl = 1;   //每次用量
                        string zxpc = "1";  //使用频次
                        string fy = "1";    //用法
                        string zxts = "1";  //执行天数
                        string ybsfxmzldm = dr["ybsfxmzldm"].ToString();  //收费项目等级代码
                        string yyxmbh = dr["yyxmbh"].ToString();          //医院项目代码
                        string yyxmmc = dr["yyxmmc"].ToString();          //医院项目名称

                        string ysbm = dr["ysdm"].ToString(); //医生编码
                        string ksdm = dr["ksno"].ToString(); //科室代码
                        string ksmc = dr["zxks"].ToString();  //执行科室
                        string cfh = dr["cfh"].ToString(); //处方号
                        string xmlx = dr["xmlx"].ToString(); //项目类型
                        string ybcfh = cfh + k.ToString();
                        string ypjldw = "-";

                        if (dr["sfno"].ToString() == "01" || dr["sfno"].ToString() == "02" || dr["sfno"].ToString() == "03")
                        {
                            ypjldw = "粒";
                        }
                        string sTmp = ybxmbh + "|" + ybxmmc + "|" + ybsflbdm + "|" + ybyfyb + "|" + ysxm + "|" + jx + "|" + gg + "|" + dj + "|" + sl + "|" + je + "|" + mcyl + "|" + zxpc + "|" + fy + "|" + zxts + "|" + yyxmmc + "|" + yyxmbh + "|" + cfh + "|" + ysbm + "|" + ksdm;
                        li_cfxm.Add(sTmp);
                    }
                }
                if (wdzxms.Length > 0)
                    return new object[] { 0, 0, wdzxms.ToString() };
                if (Math.Abs(sfje2 - ylfze) > 1.0)
                    return new object[] { 0, 0, "收费金额与医保结算金额相差" + Math.Abs(sfje2 - ylfze) + ",无法结算，请核实!" };
            }
            else
                return new object[] { 0, 0, "无费用明细" };
            #endregion

            WriteLog(sysdate + "  进行门诊收费预结算(生育保险)...");
            List<string> liSQL = new List<string>();
            //入参
            putPara(whandler, "usr", ybuserid);
            putPara(whandler, "pwd", ybpasswd);

            putPara(whandler, "aac001", ybbh);
            putPara(whandler, "akc190", mzh);
            putPara(whandler, "skc600", stscpch);
            putPara(whandler, "aka120", bzbm1);
            putPara(whandler, "skc057", bzbm2);
            putPara(whandler, "skc058", bzbm3);
            putPara(whandler, "skc099", cfxmsl);
            putPara(whandler, "skc516", tsbzbm);
            putPara(whandler, "skc113", "S");
            putPara(whandler, "mkc191", sylx);
            putPara(whandler, "smc052", sfybfz);
            WriteLog("入参|" + ybbh + "|" + mzh + "|" + stscpch + "|" + bzbm1 + "|" + bzbm2 + "|" + bzbm3 + "|" + tsbzbm + "|S|" + sylx + "|" + sfybfz);
            //上传明细

            string ybsfxmbh = string.Empty; //收费项目编码  <非空>（医保收费目录编码）  
            string ybsfxmmc = string.Empty; //收费项目名称（医院内部收费项目或者药品名称）
            string ybsflb = string.Empty;   //收费类别
            string sfybxm = string.Empty;   //是否医保项目 (000否，001是)
            string ysxm1 = string.Empty;    //医生姓名
            string jxdw = string.Empty;       //剂型（单位）
            string gg1 = string.Empty;      //规格
            string dj1 = string.Empty;       //单价  <非空>
            string sl1 = string.Empty;      //数量<非空>
            string je1 = string.Empty;      //金额<非空>
            string mcyl1 = string.Empty;    //每次用量
            string sypc = string.Empty;     //使用频次
            string fy1 = string.Empty;      //用法
            string zxts1 = string.Empty;    //执行天数
            string yysfxmbh = string.Empty; //医院项目编码
            string yysfxmmc = string.Empty; //医院项目名称
            string cfh1 = string.Empty;     //处方号
            string ysdm1 = string.Empty;   //医生代码
            string ksno1 = string.Empty; //科室代码

            startResultSetName(whandler, "list01");

            foreach (string ss in li_cfxm)
            {
                string[] s = ss.Split('|');
                ybsfxmbh = s[0];
                ybsfxmmc = s[1];
                ybsflb = s[2];
                sfybxm = s[3];
                ysxm1 = s[4];
                jxdw = s[5];
                gg1 = s[6];
                dj1 = s[7];
                sl1 = s[8];
                je1 = s[9];
                mcyl1 = s[10];
                sypc = s[11];
                fy1 = s[12];
                zxts1 = s[13];
                yysfxmbh = s[14];
                yysfxmmc = s[15];
                cfh1 = s[16];
                ysdm1 = s[17];
                ksno1 = s[18];


                WriteLog("上传费用明细-->" + s);
                putColData(whandler, "aka060", ybsfxmbh);
                putColData(whandler, "aka061", ybsfxmmc);
                putColData(whandler, "ska100", "");
                putColData(whandler, "aka063", ybsflb);
                putColData(whandler, "ska003", sfybxm);
                putColData(whandler, "skc049", ysxm1);
                putColData(whandler, "aka070", jxdw);
                putColData(whandler, "aka074", gg1);
                putColData(whandler, "aka068", dj1);
                putColData(whandler, "akc226", sl1);
                putColData(whandler, "akc227", je1);
                putColData(whandler, "aka071", mcyl1);
                putColData(whandler, "aka072", sypc);
                putColData(whandler, "aka073", fy1);
                putColData(whandler, "akc229", zxts1);
                endcurRow(whandler);

                strSql = string.Format(@"insert into ybcfmxscindr( jzlsh,jylsh,ybcfh,yysfxmbm,yysfxmmc,sfxmzxbm,ybxmmc,dj,sl,je,
                                        mcyl,sypc,ysxm,ysbm,ksbh,yf,jbr,xm,kh,ybjzlsh,sysdate,scpch) values(
                                        '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}',
                                        '{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}','{20}','{21}')",
                                        jzlsh, jylsh, cfh1, yysfxmbh, yysfxmmc, ybsfxmbh, ybsfxmmc, dj1, sl1, je1,
                                        mcyl1, sypc, ysxm1, ysdm1, ksno1, fy1, jbr, xm, kh, mzh, sysdate, stscpch);
                liSQL.Add(strSql);

            }
            endCurResultSet(whandler);

            bfalg = process(whandler, "F04.05.10.01");
            if (bfalg)
            {
                getErrMsg(whandler, sMsg);
                sMsg1 = Encoding.Default.GetString(sMsg, 0, sMsg.Length).Replace("\0", "");
                if (sMsg1.Trim().Equals("OK"))
                {
                    #region 出参变量
                    string djh_r = string.Empty;    //单据号
                    string mzh_r = string.Empty;    //门诊号
                    string ybbh_r = string.Empty;   //医保编号
                    string ylrylb_r = string.Empty; //医疗人员类别
                    string kh_r = string.Empty;     //iC卡号
                    string xm_r = string.Empty;       //姓名
                    string bcjsqzhye = string.Empty;//个人帐户余额(不含本次收费)
                    string tcjjzf = string.Empty;   //本次医保基金应支付金额
                    string xjzf = string.Empty;     //本次个人现金应支付金额
                    string zhzf = string.Empty;     //本次个人帐户应支付金额
                    string ylzje = string.Empty;    //本次医疗费总额
                    string bcxdjh = string.Empty;   //被冲销单据号
                    string cxbz_r = string.Empty;   //冲销标志（Z 正常；+ 被冲销；- 冲销）
                    string jbr_r = string.Empty;    //经办人(收费人员)
                    string jbrq = string.Empty;     //经办日期(收费日期)
                    string djdycs = string.Empty;   //单据打印次数
                    string cfxms = string.Empty;    //处方项目数
                    string jsrq = string.Empty;     //结算日期
                    string jsr = string.Empty;      //结算人
                    string grzhye = string.Empty;   //个人帐户余额

                    //收费项目<list01>开始 同入参变量
                    string yftsmzyy = string.Empty; //是否特殊门诊用药（000否，001是）
                    #endregion

                    #region 获取门诊收费返回数据
                    getParaByName(whandler, "aae072", outParam);
                    djh_r = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1204];
                    getParaByName(whandler, "akc190", outParam);
                    mzh_r = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1204];
                    getParaByName(whandler, "aac001", outParam);
                    ybbh_r = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1204];
                    getParaByName(whandler, "akc020", outParam);
                    kh_r = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1204];
                    getParaByName(whandler, "aac003", outParam);
                    xm_r = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1204];
                    getParaByName(whandler, "akc252", outParam);
                    bcjsqzhye = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1204];
                    getParaByName(whandler, "akc260", outParam);
                    tcjjzf = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1204];
                    getParaByName(whandler, "akc261", outParam);
                    xjzf = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1204];
                    getParaByName(whandler, "akc262", outParam);
                    zhzf = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1204];
                    getParaByName(whandler, "akc264", outParam);
                    ylzje = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1204];
                    getParaByName(whandler, "skc093", outParam);
                    bcxdjh = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1204];
                    getParaByName(whandler, "skc033", outParam);
                    cxbz_r = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1204];
                    getParaByName(whandler, "aae011", outParam);
                    jbr_r = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1204];
                    getParaByName(whandler, "aae036", outParam);
                    jbrq = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1204];
                    getParaByName(whandler, "skc095", outParam);
                    djdycs = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1204];
                    getParaByName(whandler, "skc099", outParam);
                    cfxms = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1204];
                    getParaByName(whandler, "aae040", outParam);
                    jsrq = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1204];
                    getParaByName(whandler, "skc035", outParam);
                    jsr = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1204];
                    getParaByName(whandler, "akc087", outParam);
                    grzhye = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    #endregion

                    sValue = ylzje + "|0.00|" + tcjjzf + "|0.00|" + zhzf + "|" + xjzf + "|0.00|0.00|0.00|0.00|" +
                           "0.00|0.00|0.00|0.00|0.00|0.00|0.00|0.00|0.00|0.00|" +
                           "0.00|0.00|0.00|0.00|0.00|" + bcjsqzhye + "|0.00|0.00|0.00|0.00|" +
                            "|0.00||0.00||0.00|" + jsrq + "|||||" + djh_r + "||" +
                           djh + "|" + cxbz_r + "|||||||||||||||" + bcxdjh;
                    strSql = string.Format(@"insert into ybfyjsdr( jzlsh,jylsh,jbr,ylfze,tcjjzf,zhzf,xjzf,djh,djhin,grbh,
                                            kh,xm,bcjsqzhye,cxbz1,jbrq,jsrq,jsr,grzfye,cfmxs,ybdjh,sysdate,ybjzlsh) values(
                                            '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}',
                                            '{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}','{20}','{21}')",
                                            jzlsh, jylsh, jbr_r, ylzje, tcjjzf, zhzf, xjzf, djh, bcxdjh, ybbh_r,
                                            kh_r, xm_r, bcjsqzhye, cxbz_r, jbrq, jsrq, jsr, grzhye, cfxms, djh_r, sysdate, mzh_r);
                    liSQL.Add(strSql);
                    object[] obj = liSQL.ToArray();
                    obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);
                    if (obj[1].ToString().Equals("1"))
                    {
                        WriteLog(sysdate + "  门诊费用预结算(生育保险)成功|" + sValue);
                        return new object[] { 0, 1, sValue };
                    }
                    else
                    {
                        WriteLog(sysdate + "  门诊费用预结算(生育保险)失败｜操作数据失败|" + obj[2].ToString());
                        //撤销结算
                        object[] obj1 = { ybbh_r, djh_r };
                        N_YBMZSFJSCX_SYBX(obj1);
                        return new object[] { 0, 0, "  门诊费用预结算(生育保险)失败｜操作数据失败|" + obj[2].ToString() };
                    }
                }
                else
                {
                    WriteLog(sysdate + "  获取门诊收费预结算(生育保险)返回信息失败|" + sMsg1);
                    return new object[] { 0, 0, "获取门诊收费预结算(生育保险)返回信息失败|" + sMsg1 };
                }
            }
            else
            {
                sMsg = new byte[1024];
                getFailReason(whandler, sMsg);
                sMsg1 = Encoding.Default.GetString(sMsg, 0, sMsg.Length).Replace("\0", "");
                WriteLog(sysdate + "  门诊收费结算(生育保险)失败|" + sMsg1);
                return new object[] { 0, 0, "门诊收费结算(生育保险)失败|" + sMsg1 };
            }
        }
        #endregion

        #region 门诊收费结算_工伤保险
        public static object[] YBMZSFJS_GSBX(object[] objParam)
        {
            string sysdate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");//当前时间
            string czygh = CliUtils.fLoginUser;      // 操作员工号
            string jzlsh = objParam[0].ToString();      // 就诊流水号
            string djh = objParam[1].ToString();        // 处方单据号
            string cyyy = "";       // 出院原因代码 
            string zhsybz = "1";// objParam[2].ToString();     // 账户使用标志（0或1）
            string jbr = CliUtils.fUserName;        // 经办人姓名
            string cfmxjylsh = string.Empty;  // 交易流水号（门诊传处方对应的交易流水号，住院传空字符串）
            string dqrq = objParam[3].ToString();  // 当前日期
            string cyrq = Convert.ToDateTime(dqrq).ToString("yyyyMMddHHmmss");                                     //出院日期
            string jsdjh = cyrq + jzlsh.Substring(jzlsh.Length - 5, 5);    //结算单据号 门诊以12位时间+流水号后5位作为单据号
            string cfhs = objParam[6].ToString();   //处方号集
            string ylfhj1 = objParam[8].ToString(); //医疗费合计 (新增)
            string sfymm = string.Empty; //是否有医保卡密码(0,1)
            string jylsh = DateTime.Now.ToString("yyyyMMddHHmmss") + "-102002-" + new Random().Next(100).ToString().PadLeft(4, '0');
            string ybjzlsh = jzlsh + DateTime.Now.ToString("HHmmss");



            if (string.IsNullOrEmpty(jzlsh))
                return new object[] { 0, 0, "就诊流水号不能为空" };
            if (string.IsNullOrEmpty(jzlsh))
                return new object[] { 0, 0, "就诊流水号不能为空" };
            if (string.IsNullOrEmpty(ylfhj1))
                return new object[] { 0, 0, "医疗合计费不能为空" };
            if (string.IsNullOrEmpty(cfhs))
                return new object[] { 0, 0, "处方号集不能为空" };

            byte[] sMsg = new byte[1024];
            byte[] outParam = new byte[1024];
            string sMsg1 = string.Empty;
            string sValue = string.Empty;
            string sfzssf = "001"; //是否正式收费(000 收费预览 001 正式收费)
            string stscpch = string.Empty; //系统上传批次号
            string cfxmsl = "0";
            bool bfalg = false;


            double sfje2 = 0.0000; //金额 
            #region 金额有效性
            try
            {
                sfje2 = Convert.ToDouble(ylfhj1);
            }
            catch
            {
                return new object[] { 0, 0, "收费金额格式错误" };
            }
            #endregion

            #region 获取患者登记信息
            string strSql = string.Format("select * from ybmzzydjdr a where a.jzlsh = '{0}' and a.cxbz = 1", jzlsh);

            DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            if (ds == null || ds.Tables[0].Rows.Count == 0)
                return new object[] { 0, 0, "未做医保门诊登记信息" };

            DataRow dr1 = ds.Tables[0].Rows[0];
            string ybbh = dr1["grbh"].ToString();  //医保编号
            string mzh = dr1["ybjzlsh"].ToString();
            string bzbm1 = dr1["bzbm"].ToString(); //病情编码
            string bzbm2 = dr1["mmbzbm1"].ToString(); //病情编码1
            string bzbm3 = dr1["mmbzbm2"].ToString(); //病情编码2
            string tsbzbm = dr1["mmbzbm3"].ToString(); //特殊病种编码
            string xm = dr1["xm"].ToString();
            string kh = dr1["kh"].ToString();
            string chxmsl = "0";
            #endregion

            #region 获取处方明细上传数据
            StringBuilder rc = new StringBuilder();
            strSql = string.Format(@"select y.ybxmbh, y.ybxmmc, m.dj, sum(m.sl) sl, sum(m.je) je, m.yyxmbh, m.yyxmmc, m.ysdm ysdm, n.b1name ysxm, m.ksno ksno,
                                    o.b2ejnm zxks, m.sfno, y.sfxmzldm ybsfxmzldm, y.sflbdm ybsflbdm, m.cfh,y.sfxmdjdm as xmlx,y.yfyb from (
                                    --药品
                                    select a.mcypno yyxmbh, a.mcypnm yyxmmc, a.mcpric dj, a.mcquty sl, a.mcamnt je, a.mcksno ksno, a.mcuser ysdm, a.mcsflb sfno,
                                    a.mccfno cfh from mzcfd a where a.mcghno = '{0}' and a.mccfno in ({1})
                                    union all
                                    --检查/治疗 
                                    select b.mbitem yyxmbh, b.mbname yyxmmc, b.mbpric dj, b.mbquty sl, b.mbsjam je, b.mbksno ksno, b.mbuser ysdm, b.mbsfno sfno,
                                    b.mbzlno cfh from mzb2d b where b.mbghno = '{0}' and b.mbzlno in ({1})
                                    union all
                                    --检验
                                    select c.mbitem yyxmbh, c.mbname yyxmmc, c.mbpric dj, c.mbquty sl, c.mbsjam je, c.mbksno ksno, c.mbuser ysdm, c.mbsfno sfno,
                                    c.mbzlno cfh from mzb4d c where c.mbghno = '{0}' and c.mbzlno in ({1})
                                    union all
                                    --注射
                                    select d.mdwayid yyxmbh, d.mdwayx yyxmmc, d.mdamnt dj, 1 sl, d.mdamnt je, d.mdzsks ksno, d.mdempn ysdm, d.mdsflb sfno,
                                    d.mdcfno cfh from mzd3d d where d.mdcfno in ({1})
                                    union all
                                    --处方划价
                                    select a.ygypno yyxmbh, a.ygypnm yyxmmc, ((a.ygamnt + 0.0) / a.ygslxx) dj, a.ygslxx sl, a.ygamnt je, b.ygksno ksno, 
                                    b.ygysno ysdm, c.y1sflb, a.ygshno cfh from yp17d a join yp17h b on a.ygcomp = b.ygcomp and a.ygshno = b.ygshno
                                    join yp01h c on c.y1ypno = a.ygypno
                                    where b.ygghno = '{0}' and a.ygshno in ({1}) and a.ygslxx > 0
                                    ) m left join bz01h n on m.ysdm = n.b1empn 
                                    left join bz02d o on m.ksno = o.b2ejks
                                    left join ybhisdzdr y on m.yyxmbh = y.hisxmbh
                                    group by y.ybxmbh, y.ybxmmc, m.dj, m.yyxmbh, m.yyxmmc, m.ysdm, n.b1name, m.ksno, o.b2ejnm, m.sfno,y.sfxmzldm,y.sflbdm, m.cfh,y.sfxmdjdm,y.yfyb", jzlsh, cfhs);
            ds.Tables.Clear();
            ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            List<string> li_cfxm = new List<string>(); //添加处方明细
            double ylfze = 0.00;
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                cfxmsl = ds.Tables[0].Rows.Count.ToString();
                DataTable dt = ds.Tables[0];
                StringBuilder wdzxms = new StringBuilder();
                for (int k = 0; k < dt.Rows.Count; k++)
                {
                    DataRow dr = dt.Rows[k];
                    if (dr["ybxmbh"] == DBNull.Value)
                        wdzxms.Append("项目代码：[" + dr["yyxmbh"].ToString() + "]，名称：[" + dr["yyxmmc"].ToString() + "]未对照，不能上传！");
                    else
                    {
                        string ybxmbh = dr["ybxmbh"].ToString();          //医保项目编号
                        string ybxmmc = dr["ybxmmc"].ToString();          //医保项目名称
                        string ybsflbdm = dr["ybsflbdm"].ToString();      //收费类别代码
                        string ybyfyb = dr["yfyb"].ToString();            //是否医保  
                        string ysxm = dr["ysxm"].ToString();            //医生姓名
                        string jx = "";//dr["jx"].ToString();  //剂型
                        string gg = "";//dr["gg"].ToString(); //规格  
                        decimal dj = Convert.ToDecimal(dr["dj"]);//单价
                        decimal sl = Convert.ToDecimal(dr["sl"]);//数量
                        decimal je = Convert.ToDecimal(dr["je"]); //金额  
                        ylfze += Convert.ToDouble(je);
                        decimal mcyl = 1;   //每次用量
                        string zxpc = "1";  //使用频次
                        string fy = "1";    //用法
                        string zxts = "1";  //执行天数
                        string ybsfxmzldm = dr["ybsfxmzldm"].ToString();  //收费项目等级代码
                        string yyxmbh = dr["yyxmbh"].ToString();          //医院项目代码
                        string yyxmmc = dr["yyxmmc"].ToString();          //医院项目名称

                        string ysbm = dr["ysdm"].ToString(); //医生编码
                        string ksdm = dr["ksno"].ToString(); //科室代码
                        string ksmc = dr["zxks"].ToString();  //执行科室
                        string cfh = dr["cfh"].ToString(); //处方号
                        string xmlx = dr["xmlx"].ToString(); //项目类型
                        string ybcfh = cfh + k.ToString();
                        string ypjldw = "-";

                        if (dr["sfno"].ToString() == "01" || dr["sfno"].ToString() == "02" || dr["sfno"].ToString() == "03")
                        {
                            ypjldw = "粒";
                        }
                        string sTmp = ybxmbh + "|" + ybxmmc + "|" + ybsflbdm + "|" + ybyfyb + "|" + ysxm + "|" + jx + "|" + gg + "|" + dj + "|" + sl + "|" + je + "|" + mcyl + "|" + zxpc + "|" + fy + "|" + zxts + "|" + yyxmmc + "|" + yyxmbh + "|" + cfh + "|" + ysbm + "|" + ksdm;
                        li_cfxm.Add(sTmp);
                    }
                }
                if (wdzxms.Length > 0)
                    return new object[] { 0, 0, wdzxms.ToString() };
                if (Math.Abs(sfje2 - ylfze) > 1.0)
                    return new object[] { 0, 0, "收费金额与医保结算金额相差" + Math.Abs(sfje2 - ylfze) + ",无法结算，请核实!" };
            }
            else
                return new object[] { 0, 0, "无费用明细" };
            #endregion

            WriteLog(sysdate + "  进行门诊收费预结算...");
            List<string> liSQL = new List<string>();
            //入参
            putPara(whandler, "usr", ybuserid);
            putPara(whandler, "pwd", ybpasswd);

            putPara(whandler, "aac001", ybbh);
            putPara(whandler, "akc190", mzh);
            putPara(whandler, "skc600", stscpch);
            putPara(whandler, "aka120", bzbm1);
            putPara(whandler, "skc057", bzbm2);
            putPara(whandler, "skc058", bzbm3);
            putPara(whandler, "skc099", cfxmsl);
            putPara(whandler, "skc516", tsbzbm);
            WriteLog("入参|" + ybbh + "|" + mzh + "|" + stscpch + "|" + bzbm1 + "|" + bzbm2 + "|" + bzbm3 + "|" + tsbzbm);
            //上传明细

            string ybsfxmbh = string.Empty; //收费项目编码  <非空>（医保收费目录编码）  
            string ybsfxmmc = string.Empty; //收费项目名称（医院内部收费项目或者药品名称）
            string ybsflb = string.Empty;   //收费类别
            string sfybxm = string.Empty;   //是否医保项目 (000否，001是)
            string ysxm1 = string.Empty;    //医生姓名
            string jxdw = string.Empty;       //剂型（单位）
            string gg1 = string.Empty;      //规格
            string dj1 = string.Empty;       //单价  <非空>
            string sl1 = string.Empty;      //数量<非空>
            string je1 = string.Empty;      //金额<非空>
            string mcyl1 = string.Empty;    //每次用量
            string sypc = string.Empty;     //使用频次
            string fy1 = string.Empty;      //用法
            string zxts1 = string.Empty;    //执行天数
            string yysfxmbh = string.Empty; //医院项目编码
            string yysfxmmc = string.Empty; //医院项目名称
            string cfh1 = string.Empty;     //处方号
            string ysdm1 = string.Empty;   //医生代码
            string ksno1 = string.Empty; //科室代码

            startResultSetName(whandler, "list01");

            foreach (string ss in li_cfxm)
            {
                string[] s = ss.Split('|');
                ybsfxmbh = s[0];
                ybsfxmmc = s[1];
                ybsflb = s[2];
                sfybxm = s[3];
                ysxm1 = s[4];
                jxdw = s[5];
                gg1 = s[6];
                dj1 = s[7];
                sl1 = s[8];
                je1 = s[9];
                mcyl1 = s[10];
                sypc = s[11];
                fy1 = s[12];
                zxts1 = s[13];
                yysfxmbh = s[14];
                yysfxmmc = s[15];
                cfh1 = s[16];
                ysdm1 = s[17];
                ksno1 = s[18];


                WriteLog("上传费用明细-->" + s);
                putColData(whandler, "aka060", ybsfxmbh);
                putColData(whandler, "aka061", ybsfxmmc);
                putColData(whandler, "aka063", ybsflb);
                putColData(whandler, "ska003", sfybxm);
                putColData(whandler, "sla003", "000");
                putColData(whandler, "skc049", ysxm1);
                putColData(whandler, "aka070", jxdw);
                putColData(whandler, "aka074", gg1);
                putColData(whandler, "aka068", dj1);
                putColData(whandler, "akc226", sl1);
                putColData(whandler, "akc227", je1);
                putColData(whandler, "aka071", mcyl1);
                putColData(whandler, "aka072", sypc);
                putColData(whandler, "aka073", fy1);
                putColData(whandler, "akc229", zxts1);
                endcurRow(whandler);

                strSql = string.Format(@"insert into ybcfmxscindr( jzlsh,jylsh,ybcfh,yysfxmbm,yysfxmmc,sfxmzxbm,ybxmmc,dj,sl,je,
                                        mcyl,sypc,ysxm,ysbm,ksbh,yf,jbr,xm,kh,ybjzlsh,sysdate,scpch) values(
                                        '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}',
                                        '{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}','{20}','{21}')",
                                        jzlsh, jylsh, cfh1, yysfxmbh, yysfxmmc, ybsfxmbh, ybsfxmmc, dj1, sl1, je1,
                                        mcyl1, sypc, ysxm1, ysdm1, ksno1, fy1, jbr, xm, kh, mzh, sysdate, stscpch);
                liSQL.Add(strSql);

            }
            endCurResultSet(whandler);

            bfalg = process(whandler, "F07.11.06.01");
            if (bfalg)
            {
                getErrMsg(whandler, sMsg);
                sMsg1 = Encoding.Default.GetString(sMsg, 0, sMsg.Length).Replace("\0", "");
                if (sMsg1.Trim().Equals("OK"))
                {
                    #region 出参变量
                    string djh_r = string.Empty;    //单据号
                    string mzh_r = string.Empty;    //门诊号
                    string ybbh_r = string.Empty;   //医保编号
                    string ylrylb_r = string.Empty; //医疗人员类别
                    string kh_r = string.Empty;     //iC卡号
                    string xm_r = string.Empty;       //姓名
                    string bcjsqzhye = string.Empty;//个人帐户余额(不含本次收费)
                    string tcjjzf = string.Empty;   //本次医保基金应支付金额
                    string xjzf = string.Empty;     //本次个人现金应支付金额
                    string zhzf = string.Empty;     //本次个人帐户应支付金额
                    string ylzje = string.Empty;    //本次医疗费总额
                    string bcxdjh = string.Empty;   //被冲销单据号
                    string cxbz_r = string.Empty;   //冲销标志（Z 正常；+ 被冲销；- 冲销）
                    string jbr_r = string.Empty;    //经办人(收费人员)
                    string jbrq = string.Empty;     //经办日期(收费日期)
                    string djdycs = string.Empty;   //单据打印次数
                    string cfxms = string.Empty;    //处方项目数
                    string jsrq = string.Empty;     //结算日期
                    string jsr = string.Empty;      //结算人
                    string grzhye = string.Empty;   //个人帐户余额

                    //收费项目<list01>开始 同入参变量
                    string yftsmzyy = string.Empty; //是否特殊门诊用药（000否，001是）
                    #endregion

                    #region 获取门诊收费返回数据
                    getParaByName(whandler, "aae072", outParam);
                    djh_r = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1204];
                    getParaByName(whandler, "akc190", outParam);
                    mzh_r = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1204];
                    getParaByName(whandler, "aac001", outParam);
                    ybbh_r = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1204];
                    getParaByName(whandler, "akc020", outParam);
                    kh_r = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1204];
                    getParaByName(whandler, "aac003", outParam);
                    xm_r = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1204];
                    getParaByName(whandler, "akc252", outParam);
                    bcjsqzhye = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1204];
                    getParaByName(whandler, "akc260", outParam);
                    tcjjzf = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1204];
                    getParaByName(whandler, "akc261", outParam);
                    xjzf = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1204];
                    getParaByName(whandler, "akc262", outParam);
                    zhzf = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1204];
                    getParaByName(whandler, "akc264", outParam);
                    ylzje = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1204];
                    getParaByName(whandler, "skc093", outParam);
                    bcxdjh = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1204];
                    getParaByName(whandler, "skc033", outParam);
                    cxbz_r = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1204];
                    getParaByName(whandler, "aae011", outParam);
                    jbr_r = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1204];
                    getParaByName(whandler, "aae036", outParam);
                    jbrq = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1204];
                    getParaByName(whandler, "skc095", outParam);
                    djdycs = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1204];
                    getParaByName(whandler, "skc099", outParam);
                    cfxms = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1204];
                    getParaByName(whandler, "aae040", outParam);
                    jsrq = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1204];
                    getParaByName(whandler, "skc035", outParam);
                    jsr = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1204];
                    getParaByName(whandler, "akc087", outParam);
                    grzhye = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    #endregion

                    sValue = ylzje + "|0.00|" + tcjjzf + "|0.00|" + zhzf + "|" + xjzf + "|0.00|0.00|0.00|0.00|" +
                           "0.00|0.00|0.00|0.00|0.00|0.00|0.00|0.00|0.00|0.00|" +
                           "0.00|0.00|0.00|0.00|0.00|" + bcjsqzhye + "|0.00|0.00|0.00|0.00|" +
                            "|0.00||0.00||0.00|" + jsrq + "|||||" + djh_r + "||" +
                           djh + "|" + cxbz_r + "|||||||||||||||" + bcxdjh;
                    strSql = string.Format(@"insert into ybfyjsdr( jzlsh,jylsh,jbr,ylfze,tcjjzf,zhzf,xjzf,djh,djhin,grbh,
                                            kh,xm,bcjsqzhye,cxbz1,jbrq,jsrq,jsr,grzfye,cfmxs,ybdjh,sysdate,ybjzlsh) values(
                                            '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}',
                                            '{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}','{20}','{21}')",
                                            jzlsh, jylsh, jbr_r, ylzje, tcjjzf, zhzf, xjzf, djh, bcxdjh, ybbh_r,
                                            kh_r, xm_r, bcjsqzhye, cxbz_r, jbrq, jsrq, jsr, grzhye, cfxms, djh_r, sysdate, mzh_r);
                    liSQL.Add(strSql);
                    object[] obj = liSQL.ToArray();
                    obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);
                    if (obj[1].ToString().Equals("1"))
                    {
                        WriteLog(sysdate + "  门诊费用预结算成功|" + sValue);
                        return new object[] { 0, 1, sValue };
                    }
                    else
                    {
                        WriteLog(sysdate + "  门诊费用预结算失败｜操作数据失败|" + obj[2].ToString());
                        //撤销结算
                        object[] obj1 = { ybbh_r, djh_r };
                        N_YBMZSFJSCX_GSBX(obj1);
                        return new object[] { 0, 0, "  门诊费用预结算失败｜操作数据失败|" + obj[2].ToString() };
                    }
                }
                else
                {
                    WriteLog(sysdate + "  获取门诊收费预结算返回信息失败|" + sMsg1);
                    return new object[] { 0, 0, "获取门诊收费预结算返回信息失败|" + sMsg1 };
                }
            }
            else
            {
                sMsg = new byte[1024];
                getFailReason(whandler, sMsg);
                sMsg1 = Encoding.Default.GetString(sMsg, 0, sMsg.Length).Replace("\0", "");
                WriteLog(sysdate + "  门诊收费结算失败|" + sMsg1);
                return new object[] { 0, 0, "门诊收费结算失败|" + sMsg1 };
            }
        }
        #endregion


        #region 门诊费用结算撤销
        public static object[] YBMZSFJSCX(object[] objParam)
        {
            string jzlsh = objParam[0].ToString();
            string sxzbm = string.Empty;
            if (string.IsNullOrEmpty(jzlsh))
                return new object[] { 0, 0, "就诊流水号不能为空" };

            string strSql = string.Format(@"select xzbm from ybmzzydjdr where jzlsh='{0}' and cxbz=1", objParam[0].ToString());
            DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            if (ds == null || ds.Tables[0].Rows.Count == 0)
                return new object[] { 0, 0, "未做医保门诊登记信息" };
            sxzbm = ds.Tables[0].Rows[0]["xzbm"].ToString();
            object[] objReturn = null;
            switch (sxzbm)
            {
                case "310":
                    objReturn = YBMZSFJSCX_YLBX(objParam);
                    break;
                case "410":
                    objReturn = YBMZSFJSCX_GSBX(objParam);
                    break;
                case "510":
                    objReturn = YBMZSFJSCX_SYBX(objParam);
                    break;
                default:
                    objReturn = new object[] { 0, 0, "门诊费用结算失败|未找到结算体" };
                    break;
            }
            return objReturn;

        }
        #endregion

        #region 门诊费用结算撤销_医疗保险
        public static object[] YBMZSFJSCX_YLBX(object[] objParam)
        {
            string sysdate = GetServerDateTime();//当前时间
            string czygh = CliUtils.fLoginUser;   // 操作员工号 
            string ywzqh = "";   // 业务周期号
            string jzlsh = objParam[0].ToString();   // 就诊流水号
            string jbr = CliUtils.fUserName;     // 经办人姓名
            string djh = objParam[1].ToString();     // 单据号(发票号)
            string yylf1 = ""; //原医疗费金额(新增)
            string tfje1 = ""; //退费金额(新增)
            string ybbh = string.Empty; //医保编号
            string ybdjh = string.Empty;

            bool bfalg = false;
            byte[] sMsg = new byte[1024];
            byte[] outParam = new byte[1024];
            string sMsg1 = string.Empty;


            if (string.IsNullOrEmpty(jzlsh))
                return new object[] { 0, 0, "就诊流水号不能为空" };
            if (string.IsNullOrEmpty(djh))
                return new object[] { 0, 0, "发票号不能为空" };

            string strSql = string.Format(@"select a.* from ybfyjsdr a where a.jzlsh = '{0}' and a.djh = '{1}' and a.cxbz = 1", jzlsh, djh);
            DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                ybbh = ds.Tables[0].Rows[0]["grbh"].ToString();
                ybdjh = ds.Tables[0].Rows[0]["ybdjh"].ToString();
            }
            else
                return new object[] { 0, 0, "未进行医保门诊收费结算" };

            WriteLog(sysdate + "  进入门诊费用结算撤销(医疗保险)...");
            //入参
            putPara(whandler, "usr", ybuserid);
            putPara(whandler, "pwd", ybpasswd);

            putPara(whandler, "aac001", ybbh);
            putPara(whandler, "aae072", ybdjh);

            WriteLog("入参|" + djh + "|" + jzlsh + "|" + ybbh + "|" + ybdjh);
            List<string> liSQL = new List<string>();
            bfalg = process(whandler, "F04.05.12.01");
            if (bfalg)
            {
                getErrMsg(whandler, sMsg);
                sMsg1 = Encoding.Default.GetString(sMsg, 0, sMsg.Length).Replace("\0", "");
                if (sMsg1.ToUpper().Trim().Equals("OK"))
                {
                    //出参
                    string bcxdjh = string.Empty; //被冲销单据号
                    string cxbz1 = string.Empty;  //冲销标志（Z 正常；+ 被冲销；- 冲销）
                    string jbr_r = string.Empty;
                    string jbrq = string.Empty;

                    //获取出参数据
                    getParaByName(whandler, "skc093", outParam);
                    bcxdjh = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "skc033", outParam);
                    cxbz1 = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "aae011", outParam);
                    jbr_r = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "aae036", outParam);
                    jbrq = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");

                    strSql = string.Format(@"insert into ybfyjsdr( jzlsh,jylsh,jbr,ylfze,tcjjzf,zhzf,xjzf,djh,djhin,grbh,
                                            kh,xm,bcjsqzhye,cxbz1,jbrq,jsrq,jsr,grzfye,cfmxs,ybdjh,cxbz,sysdate) select 
                                            jzlsh,jylsh,'{5}',ylfze,tcjjzf,zhzf,xjzf,djh,'{3}',grbh,
                                            kh,xm,bcjsqzhye,'{4}','{2}',jsrq,jsr,grzfye,cfmxs,ybdjh,0,'{6}' from ybfyjsdr 
                                            where jzlsh='{0}' and djh='{1}' and cxbz=1 ", jzlsh, djh, jbrq, bcxdjh, cxbz1, jbr_r, sysdate);
                    liSQL.Add(strSql);
                    strSql = string.Format(@"update ybfyjsdr set cxbz=2 where jzlsh='{0}' and djh='{1}' and cxbz=1", jzlsh, djh);
                    liSQL.Add(strSql);
                    strSql = string.Format(@"insert into ybcfmxscindr( jzlsh,jylsh,ybcfh,yysfxmbm,yysfxmmc,sfxmzxbm,ybxmmc,dj,sl,je,
                                            mcyl,sypc,ysxm,ysbm,ksbh,yf,jbr,xm,kh,ybjzlsh,cxbz,sysdate) select
                                            jzlsh,jylsh,ybcfh,yysfxmbm,yysfxmmc,sfxmzxbm,ybxmmc,dj,sl,je,
                                            mcyl,sypc,ysxm,ysbm,ksbh,yf,jbr,xm,kh,ybjzlsh,0,'{1}' from ybcfmxscindr where jzlsh='{0}' and cxbz=1", jzlsh, sysdate);
                    liSQL.Add(strSql);
                    strSql = string.Format(@"update ybcfmxscindr set cxbz=2 where jzlsh='{0}' and cxbz=1", jzlsh);
                    liSQL.Add(strSql);
                    object[] obj = liSQL.ToArray();
                    obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);
                    if (obj[1].ToString().Equals("1"))
                    {
                        WriteLog(sysdate + "   门诊费用结算撤销(医疗保险)成功|");
                        return new object[] { 0, 1, "门诊费用结算撤销(医疗保险)成功" };
                    }
                    else
                    {
                        WriteLog(sysdate + "   门诊费用结算撤销(医疗保险)失败|数据操作失败|" + obj[2].ToString());
                        return new object[] { 0, 0, "门诊费用结算撤销(医疗保险)失败|数据操作失败|" + obj[2].ToString() };
                    }
                }
                else
                {
                    WriteLog(sysdate + "  获取门诊费用结算撤销(医疗保险)返回信息失败" + sMsg1);
                    return new object[] { 0, 0, "获取门诊费用结算撤销(医疗保险)返回信息失败|" + sMsg1 };
                }
            }
            else
            {
                sMsg = new byte[1024];
                getFailReason(whandler, sMsg);
                sMsg1 = Encoding.Default.GetString(sMsg, 0, sMsg.Length).Replace("\0", "");
                WriteLog(sysdate + "  门诊费用结算撤销(医疗保险)失败|" + sMsg1);
                return new object[] { 0, 0, "门诊费用结算撤销(医疗保险)失败|" + sMsg1 };
            }
        }
        #endregion

        #region 门诊费用结算撤销_生育保险
        public static object[] YBMZSFJSCX_SYBX(object[] objParam)
        {
            string sysdate = GetServerDateTime();//当前时间
            string czygh = CliUtils.fLoginUser;   // 操作员工号 
            string ywzqh = "";   // 业务周期号
            string jzlsh = objParam[0].ToString();   // 就诊流水号
            string jbr = CliUtils.fUserName;     // 经办人姓名
            string djh = objParam[1].ToString();     // 单据号(发票号)
            string yylf1 = ""; //原医疗费金额(新增)
            string tfje1 = ""; //退费金额(新增)
            string ybbh = string.Empty; //医保编号
            string ybdjh = string.Empty;

            bool bfalg = false;
            byte[] sMsg = new byte[1024];
            byte[] outParam = new byte[1024];
            string sMsg1 = string.Empty;


            if (string.IsNullOrEmpty(jzlsh))
                return new object[] { 0, 0, "就诊流水号不能为空" };
            if (string.IsNullOrEmpty(djh))
                return new object[] { 0, 0, "发票号不能为空" };

            string strSql = string.Format(@"select a.* from ybfyjsdr a where a.jzlsh = '{0}' and a.djh = '{1}' and a.cxbz = 1", jzlsh, djh);
            DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                ybbh = ds.Tables[0].Rows[0]["grbh"].ToString();
                ybdjh = ds.Tables[0].Rows[0]["ybdjh"].ToString();
            }
            else
                return new object[] { 0, 0, "未进行医保门诊收费结算" };

            WriteLog(sysdate + "  进入门诊费用结算撤销(生育保险)...");
            //入参
            putPara(whandler, "usr", ybuserid);
            putPara(whandler, "pwd", ybpasswd);

            putPara(whandler, "aac001", ybbh);
            putPara(whandler, "aae072", ybdjh);
            putPara(whandler, "skc113", "S");

            WriteLog("入参|" + djh + "|" + jzlsh + "|" + ybbh + "|" + ybdjh);
            List<string> liSQL = new List<string>();
            bfalg = process(whandler, "F04.05.12.01");
            if (bfalg)
            {
                getErrMsg(whandler, sMsg);
                sMsg1 = Encoding.Default.GetString(sMsg, 0, sMsg.Length).Replace("\0", "");
                if (sMsg1.ToUpper().Trim().Equals("OK"))
                {
                    //出参
                    string bcxdjh = string.Empty; //被冲销单据号
                    string cxbz1 = string.Empty;  //冲销标志（Z 正常；+ 被冲销；- 冲销）
                    string jbr_r = string.Empty;
                    string jbrq = string.Empty;

                    //获取出参数据
                    getParaByName(whandler, "skc093", outParam);
                    bcxdjh = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "skc033", outParam);
                    cxbz1 = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "aae011", outParam);
                    jbr_r = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "aae036", outParam);
                    jbrq = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");

                    strSql = string.Format(@"insert into ybfyjsdr( jzlsh,jylsh,jbr,ylfze,tcjjzf,zhzf,xjzf,djh,djhin,grbh,
                                            kh,xm,bcjsqzhye,cxbz1,jbrq,jsrq,jsr,grzfye,cfmxs,ybdjh,cxbz,sysdate) select 
                                            jzlsh,jylsh,'{5}',ylfze,tcjjzf,zhzf,xjzf,djh,'{3}',grbh,
                                            kh,xm,bcjsqzhye,'{4}','{2}',jsrq,jsr,grzfye,cfmxs,ybdjh,0,'{6}' from ybfyjsdr 
                                            where jzlsh='{0}' and djh='{1}' and cxbz=1 ", jzlsh, djh, jbrq, bcxdjh, cxbz1, jbr_r, sysdate);
                    liSQL.Add(strSql);
                    strSql = string.Format(@"update ybfyjsdr set cxbz=2 where jzlsh='{0}' and djh='{1}' and cxbz=1", jzlsh, djh);
                    liSQL.Add(strSql);
                    strSql = string.Format(@"insert into ybcfmxscindr( jzlsh,jylsh,ybcfh,yysfxmbm,yysfxmmc,sfxmzxbm,ybxmmc,dj,sl,je,
                                            mcyl,sypc,ysxm,ysbm,ksbh,yf,jbr,xm,kh,ybjzlsh,cxbz,sysdate) select
                                            jzlsh,jylsh,ybcfh,yysfxmbm,yysfxmmc,sfxmzxbm,ybxmmc,dj,sl,je,
                                            mcyl,sypc,ysxm,ysbm,ksbh,yf,jbr,xm,kh,ybjzlsh,0,'{1}' from ybcfmxscindr where jzlsh='{0}' and cxbz=1", jzlsh, sysdate);
                    liSQL.Add(strSql);
                    strSql = string.Format(@"update ybcfmxscindr set cxbz=2 where jzlsh='{0}' and cxbz=1", jzlsh);
                    liSQL.Add(strSql);
                    object[] obj = liSQL.ToArray();
                    obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);
                    if (obj[1].ToString().Equals("1"))
                    {
                        WriteLog(sysdate + "   门诊费用结算撤销(生育保险)成功|");
                        return new object[] { 0, 1, "门诊费用结算撤销(生育保险)成功" };
                    }
                    else
                    {
                        WriteLog(sysdate + "   门诊费用结算撤销(生育保险)失败|数据操作失败|" + obj[2].ToString());
                        return new object[] { 0, 0, "门诊费用结算撤销(生育保险)失败|数据操作失败|" + obj[2].ToString() };
                    }
                }
                else
                {
                    WriteLog(sysdate + "  获取门诊费用结算撤销(生育保险)返回信息失败" + sMsg1);
                    return new object[] { 0, 0, "获取门诊费用结算撤销(生育保险)返回信息失败|" + sMsg1 };
                }
            }
            else
            {
                sMsg = new byte[1024];
                getFailReason(whandler, sMsg);
                sMsg1 = Encoding.Default.GetString(sMsg, 0, sMsg.Length).Replace("\0", "");
                WriteLog(sysdate + "  门诊费用结算撤销(医疗保险)失败|" + sMsg1);
                return new object[] { 0, 0, "门诊费用结算撤销(医疗保险)失败|" + sMsg1 };
            }
        }
        #endregion

        #region 门诊费用结算撤销_工伤保险
        public static object[] YBMZSFJSCX_GSBX(object[] objParam)
        {
            string sysdate = GetServerDateTime();//当前时间
            string czygh = CliUtils.fLoginUser;   // 操作员工号 
            string ywzqh = "";   // 业务周期号
            string jzlsh = objParam[0].ToString();   // 就诊流水号
            string jbr = CliUtils.fUserName;     // 经办人姓名
            string djh = objParam[1].ToString();     // 单据号(发票号)
            string yylf1 = ""; //原医疗费金额(新增)
            string tfje1 = ""; //退费金额(新增)
            string ybbh = string.Empty; //医保编号
            string ybdjh = string.Empty;

            bool bfalg = false;
            byte[] sMsg = new byte[1024];
            byte[] outParam = new byte[1024];
            string sMsg1 = string.Empty;


            if (string.IsNullOrEmpty(jzlsh))
                return new object[] { 0, 0, "就诊流水号不能为空" };
            if (string.IsNullOrEmpty(djh))
                return new object[] { 0, 0, "发票号不能为空" };

            string strSql = string.Format(@"select a.* from ybfyjsdr a where a.jzlsh = '{0}' and a.djh = '{1}' and a.cxbz = 1", jzlsh, djh);
            DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                ybbh = ds.Tables[0].Rows[0]["grbh"].ToString();
                ybdjh = ds.Tables[0].Rows[0]["ybdjh"].ToString();
            }
            else
                return new object[] { 0, 0, "未进行医保门诊收费结算" };

            WriteLog(sysdate + "  进入门诊费用结算撤销(工伤保险)...");
            //入参
            putPara(whandler, "usr", ybuserid);
            putPara(whandler, "pwd", ybpasswd);

            putPara(whandler, "aac001", ybbh);
            putPara(whandler, "aae072", ybdjh);

            WriteLog("入参|" + djh + "|" + jzlsh + "|" + ybbh + "|" + ybdjh);
            List<string> liSQL = new List<string>();
            bfalg = process(whandler, "F07.11.06.02");
            if (bfalg)
            {
                getErrMsg(whandler, sMsg);
                sMsg1 = Encoding.Default.GetString(sMsg, 0, sMsg.Length).Replace("\0", "");
                if (sMsg1.ToUpper().Trim().Equals("OK"))
                {
                    //出参
                    string bcxdjh = string.Empty; //被冲销单据号
                    string cxbz1 = string.Empty;  //冲销标志（Z 正常；+ 被冲销；- 冲销）
                    string jbr_r = string.Empty;
                    string jbrq = string.Empty;

                    //获取出参数据
                    getParaByName(whandler, "skc093", outParam);
                    bcxdjh = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "skc033", outParam);
                    cxbz1 = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "aae011", outParam);
                    jbr_r = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "aae036", outParam);
                    jbrq = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");

                    strSql = string.Format(@"insert into ybfyjsdr( jzlsh,jylsh,jbr,ylfze,tcjjzf,zhzf,xjzf,djh,djhin,grbh,
                                            kh,xm,bcjsqzhye,cxbz1,jbrq,jsrq,jsr,grzfye,cfmxs,ybdjh,cxbz,sysdate) select 
                                            jzlsh,jylsh,'{5}',ylfze,tcjjzf,zhzf,xjzf,djh,'{3}',grbh,
                                            kh,xm,bcjsqzhye,'{4}','{2}',jsrq,jsr,grzfye,cfmxs,ybdjh,0,'{6}' from ybfyjsdr 
                                            where jzlsh='{0}' and djh='{1}' and cxbz=1 ", jzlsh, djh, jbrq, bcxdjh, cxbz1, jbr_r, sysdate);
                    liSQL.Add(strSql);
                    strSql = string.Format(@"update ybfyjsdr set cxbz=2 where jzlsh='{0}' and djh='{1}' and cxbz=1", jzlsh, djh);
                    liSQL.Add(strSql);
                    strSql = string.Format(@"insert into ybcfmxscindr( jzlsh,jylsh,ybcfh,yysfxmbm,yysfxmmc,sfxmzxbm,ybxmmc,dj,sl,je,
                                            mcyl,sypc,ysxm,ysbm,ksbh,yf,jbr,xm,kh,ybjzlsh,cxbz,sysdate) select
                                            jzlsh,jylsh,ybcfh,yysfxmbm,yysfxmmc,sfxmzxbm,ybxmmc,dj,sl,je,
                                            mcyl,sypc,ysxm,ysbm,ksbh,yf,jbr,xm,kh,ybjzlsh,0,'{1}' from ybcfmxscindr where jzlsh='{0}' and cxbz=1", jzlsh, sysdate);
                    liSQL.Add(strSql);
                    strSql = string.Format(@"update ybcfmxscindr set cxbz=2 where jzlsh='{0}' and cxbz=1", jzlsh);
                    liSQL.Add(strSql);
                    object[] obj = liSQL.ToArray();
                    obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);
                    if (obj[1].ToString().Equals("1"))
                    {
                        WriteLog(sysdate + "   门诊费用结算撤销(工伤保险)成功|");
                        return new object[] { 0, 1, "门诊费用结算撤销(工伤保险)成功" };
                    }
                    else
                    {
                        WriteLog(sysdate + "   门诊费用结算撤销(工伤保险)失败|数据操作失败|" + obj[2].ToString());
                        return new object[] { 0, 0, "门诊费用结算撤销(工伤保险)失败|数据操作失败|" + obj[2].ToString() };
                    }
                }
                else
                {
                    WriteLog(sysdate + "  获取门诊费用结算撤销(工伤保险)返回信息失败" + sMsg1);
                    return new object[] { 0, 0, "获取门诊费用结算撤销(工伤保险)返回信息失败|" + sMsg1 };
                }
            }
            else
            {
                sMsg = new byte[1024];
                getFailReason(whandler, sMsg);
                sMsg1 = Encoding.Default.GetString(sMsg, 0, sMsg.Length).Replace("\0", "");
                WriteLog(sysdate + "  门诊费用结算撤销(医疗保险)失败|" + sMsg1);
                return new object[] { 0, 0, "门诊费用结算撤销(医疗保险)失败|" + sMsg1 };
            }
        }
        #endregion


        #region 住院登记
        public static object[] YBZYDJ(object[] objParam)
        {
            string[] kxx = objParam[4].ToString().Split('|');//读卡返回信息
            if (kxx.Length < 2)
                return new object[] { 0, 0, "无读卡信息反馈" };
            string sxzbm = kxx[27].ToString(); //参保单位类型
            object[] objReturn = null;
            switch (sxzbm)
            {
                case "310": //医疗保险
                    objReturn = YBZYDJ_YLBX(objParam);
                    break;
                case "410": //工伤保险
                    objReturn = YBZYDJ_GSBX(objParam);
                    break;
                case "510": //生育保险
                    objReturn = YBZYDJ_SYBX(objParam);
                    break;
                default:
                    objReturn = new object[] { 0, 0, "住院登记失败|未找到文法体" };
                    break;
            }
            return objReturn;
        }
        #endregion

        #region 住院登记_医疗保险
        public static object[] YBZYDJ_YLBX(object[] objParam)
        {
            string sysdate = GetServerDateTime();//当前时间
            string jzlsh = objParam[0].ToString(); //就诊流水号
            string yllb = objParam[1].ToString(); //医疗类别代码
            string bzbm = objParam[2].ToString(); //病种编码
            string bzmc = objParam[3].ToString(); //病种名称
            string czygh = CliUtils.fLoginUser;  // 操作员工号
            string ywzqh = "";  // 业务周期号 
            string djsj = "";   // 登记时间(格式：DateTime.Now.ToString("yyyyMMdd HHmmss"))  //对应入院日期时间

            #region 读卡信息 
            string[] kxx = objParam[4].ToString().Split('|');//读卡返回信息
            if (kxx.Length < 2)
                return new object[] { 0, 0, "无读卡信息反馈" };
            string grbh = kxx[0].ToString(); //个人编号  *//对应玉山 个人标识号
            string dwbh = kxx[1].ToString(); //单位编号
            string sfzh = kxx[2].ToString(); //身份证号
            string xm = kxx[3].ToString(); //姓名
            string xb = kxx[4].ToString(); //性别
            string mz = kxx[5].ToString(); //民族
            string csrq = kxx[6].ToString(); //出生日期
            string kh = kxx[7].ToString(); //卡号
            string yldylb = kxx[8].ToString(); //医疗待遇类别
            string rycbzt = kxx[9].ToString(); //人员参保状态
            string ydrybz = kxx[10].ToString(); //异地人员标志
            string tcqh = kxx[11].ToString(); //统筹区号
            string nd = kxx[12].ToString(); //年度
            string zyzt = kxx[13].ToString(); //在院状态
            string zhye = kxx[14].ToString(); //帐户余额
            string bnylflj = kxx[15].ToString(); //本年医疗费累计
            string bnzhzclj = kxx[16].ToString(); //本年帐户支出累计
            string bntczclj = kxx[17].ToString(); //本年统筹支出累计
            string bnjzjzclj = kxx[18].ToString(); //本年救助金支出累计
            string bngwybzjjlj = kxx[19].ToString(); //本年公务员补助基金累计
            string bnczjmmztczflj = kxx[20].ToString(); //本年城镇居民门诊统筹支付累计
            string jrtcfylj = kxx[21].ToString(); //进入统筹费用累计
            string jrjzjfylj = kxx[22].ToString(); //进入救助金费用累计
            string qfbzlj = kxx[23].ToString(); //起付标准累计
            string bnzycs = kxx[24].ToString(); //本年住院次数
            string dwmc = kxx[25].ToString(); //单位名称
            string nl = kxx[26].ToString(); //年龄
            string cbdwlx = kxx[27].ToString(); //参保单位类型
            string jbjgbm = kxx[28].ToString(); //经办机构编码
            #endregion

            string hznm = ""; //患者姓名
            string ksbh = "";   // 科室编号 
            string ksmc = "";   // 科室名称  //*对应 病区名称
            string ysdm = "";   // 医师代码
            string ysxm = "";  // 医师姓名
            string jbr = CliUtils.fUserName;   // 经办人姓名
            string cwh = "";    //床号  //*对应玉山 床位号
            string dqbh = tcqh;    //所属区号(地区编号)     
            string lyjgdm = objParam[5].ToString();//来源机构代码
            string lyjgmc = objParam[6].ToString();//来源机构名称
            string yllbmc = objParam[7].ToString();//医疗类别名称
            string dgysbm = "";// objParam[8].ToString(); //定岗医生代码
            string dgysxm = "";//objParam[9].ToString(); //定岗医生姓名
            string sfzh1 = objParam[10].ToString(); //身份证号
            string bzbm1 = objParam[11].ToString(); //病种编码1
            string bzmc1 = objParam[12].ToString(); //病种名称1
            string bzbm2 = objParam[13].ToString(); //病种编码2
            string bzmc2 = objParam[14].ToString(); //病种编码2
            string rylx = objParam[15].ToString();  //入院类型
            string lxdh = string.Empty;
            string jtdz = string.Empty;
            byte[] sMsg = new byte[1024];
            byte[] outParam = new byte[1024];
            string sMsg1 = string.Empty;
            string sValue = string.Empty;
            bool bfalg = false;

            if (string.IsNullOrEmpty(jzlsh))
                return new object[] { 0, 0, "就诊流水号不能为空" };
            if (string.IsNullOrEmpty(grbh))
                return new object[] { 0, 0, "个人编号不能为空" };
            if (string.IsNullOrEmpty(bzbm))
                return new object[] { 0, 0, "病种编码不能为空" };
            if (string.IsNullOrEmpty(bzmc))
                return new object[] { 0, 0, "病种名称不能为空" };

            string ybjzlsh = jzlsh + DateTime.Now.ToString("HHmmss");
            string strSql = string.Format(@"select a.z1zyno,a.z1hznm,a.z1date,b.z1ksno, b.z1ksnm,b.z1ysno,b.z1ysnm,b.z1bedn,a.z1mobi,a.z1yzbm from zy01h a
                                            left join zy01d b on a.z1zyno=b.z1zyno and a.z1ghno=b.z1ghno 
                                            where a.z1zyno='{0}'", jzlsh);
            DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            if (ds == null || ds.Tables[0].Rows.Count == 0)
                return new object[] { 0, 0, "无患者信息" };
            djsj = Convert.ToDateTime(ds.Tables[0].Rows[0]["z1date"].ToString()).ToString("yyyy-MM-dd HH:mm:ss");
            hznm = ds.Tables[0].Rows[0]["z1hznm"].ToString();
            cwh = ds.Tables[0].Rows[0]["z1bedn"].ToString();
            ksbh = ds.Tables[0].Rows[0]["z1ksno"].ToString();
            ksmc = ds.Tables[0].Rows[0]["z1ksnm"].ToString();
            ysdm = ds.Tables[0].Rows[0]["z1ysno"].ToString();
            ysxm = ds.Tables[0].Rows[0]["z1ysnm"].ToString();
            lxdh = ds.Tables[0].Rows[0]["z1mobi"].ToString();
            jtdz = ds.Tables[0].Rows[0]["z1yzbm"].ToString();

            string ryrq = Convert.ToDateTime(ds.Tables[0].Rows[0]["z1date"]).ToString("yyyy-MM-dd");
            string jylsh = DateTime.Now.ToString("yyyyMMddHHmmss") + "-102002-" + new Random().Next(100).ToString().PadLeft(4, '0');

            strSql = string.Format(@"select * from ybmzzydjdr where jzlsh='{0}' and cxbz=1", jzlsh);
            ds.Tables.Clear();
            ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            if (ds.Tables[0].Rows.Count > 0)
                return new object[] { 0, 0, "患者" + jzlsh + "已做医保补办登记" };

            WriteLog(sysdate + "  进入医保住院登记(医疗保险)...");
            //入参
            putPara(whandler, "usr", ybuserid);
            putPara(whandler, "pwd", ybpasswd);

            putPara(whandler, "aac001", grbh);
            putPara(whandler, "skb010", ksbh);
            putPara(whandler, "skc037", ksmc);
            putPara(whandler, "skc615", rylx);
            putPara(whandler, "akc192", ryrq);
            putPara(whandler, "akc193", bzbm);
            putPara(whandler, "skc038", bzbm1);
            putPara(whandler, "skc039", bzbm2);
            putPara(whandler, "skc616", bzmc);
            putPara(whandler, "skc617", bzmc1);
            putPara(whandler, "skc618", bzmc2);
            putPara(whandler, "aae005", lxdh);
            putPara(whandler, "aae006", jtdz);

            WriteLog("入参|" + ybuserid + "|" + ybpasswd + "|" + grbh + "|" + ksbh + "|" + ksmc + "|" + rylx + "|" + ryrq + "|" + bzbm + "|"
                    + bzbm1 + "|" + bzbm2 + "|" + bzmc + "|" + bzmc1 + "|" + bzmc2 + "|" + lxdh + "|" + jtdz);

            bfalg = process(whandler, "F04.06.01.01");
            if (bfalg)
            {
                List<string> liSQL = new List<string>();
                getErrMsg(whandler, sMsg);
                sMsg1 = Encoding.Default.GetString(sMsg, 0, sMsg.Length).Replace("\0", "");
                if (sMsg1.ToUpper().Trim().Equals("OK"))
                {
                    #region 出参变量
                    string zylsh = string.Empty;    //住院流水号（医保中心系统生成）
                    string ybbh_r = string.Empty;   //医保编号
                    string ickh_r = string.Empty;   //IC卡号
                    string ickzt_r = string.Empty;  //IC卡状态
                    string wdbh = string.Empty;     //网点编号（医院、药店、门诊编号）
                    string yydj = string.Empty;     //医院等级         
                    string ylrylb = string.Empty;   //医疗人员类别
                    string fzxbh = string.Empty;    //分中心编码（网点所属分中心）
                    string fzxmc = string.Empty;    //分中心名称（网点所属分中心）
                    string xzqhdm = string.Empty;   //行政区划代码(参保人员所属)
                    string xzqhmc = string.Empty;   //行政区划名称(参保人员所属)
                    string dwbh_r = string.Empty;   //单位编号
                    string xm_r = string.Empty;//姓名
                    string xb_r = string.Empty;//性别
                    string nl_r = string.Empty;//年龄
                    string kbcs = string.Empty;//看病次数
                    string zydjh = string.Empty;//转院登记号
                    string ksbm_r = string.Empty;//科室编码（必填）
                    string ksmc_r = string.Empty;//挂号科室名称
                    string ryzdjbbm1 = string.Empty;    //入院诊断疾病编码（主）  <非空>
                    string ryzdjbbm2 = string.Empty;    //入院诊断疾病编码（次）
                    string ryzdjbbm3 = string.Empty;    //入院诊断疾病编码（第三）
                    string ryzdjbmc1 = string.Empty;    //入院诊断疾病中文名称（主）  <非空>
                    string ryzdjbmc2 = string.Empty;    //入院诊断疾病中文名称（次）
                    string ryzdjbmc3 = string.Empty;    //入院诊断疾病中文名称（第三）
                    string zyrq = string.Empty;         //住院日期
                    string cyzdjbbm1 = string.Empty;    //出院疾病诊断编码       <非空
                    string cyzdjbbm2 = string.Empty;    //出院诊断疾病编码（次）
                    string cyzdjbbm3 = string.Empty;    //出院诊断疾病编码（第三）
                    string cyzdjbmc1 = string.Empty;    //出院诊断疾病中文名称（主）  <非空>
                    string cyzdjbmc2 = string.Empty;    //出院诊断疾病中文名称（次）
                    string cyzdjbmc3 = string.Empty;    //出院诊断疾病中文名称（第三）
                    string cyrq = string.Empty;         //出院日期
                    string blcyrq = string.Empty;       //办理出院日期（可能比出院日期晚）
                    string jbr_r = string.Empty;//经办人(医院办理挂号工作人员姓名)
                    string jbrq = string.Empty;//经办日期(挂号日期)
                    string cydjr = string.Empty;    //出院登记人
                    string cxbz = string.Empty;//冲销标志（Z 正常；+ 被冲销；- 冲销）
                    string bcxlsh = string.Empty;//被冲销流水号
                    string jsrq = string.Empty; //结算日期
                    string jsr = string.Empty;//结算人
                    string grzhye = string.Empty;   //个人帐户余额
                    string ylrylbmc = string.Empty; //医疗人员类别名称
                    string jznd = string.Empty; //结转年度
                    string xbmc = string.Empty; //性别名称
                    string icztmc = string.Empty; //IC卡状态名称
                    string yydjmc = string.Empty;   //医院等级名称
                    string jbjgmc = string.Empty;   //经办机构名称
                    string dwmc_r = string.Empty;   //单位名称
                    string wdmc = string.Empty;     //网点名称
                    string zbqmc_ry = string.Empty; //主病情名称(入院)
                    string cbqmc_ry = string.Empty; //次病情名称(入院)
                    string sbqmc_ry = string.Empty; //三病情名称(入院)
                    string zbqmc_cy = string.Empty; //主病情名称（出院）
                    string cbqmc_cy = string.Empty; //次病情名称（出院）  
                    string sbqmc_cy = string.Empty; //三病情名称（出院）
                    #endregion

                    #region 获取出参数据
                    getParaByName(whandler, "akc190", outParam);
                    zylsh = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "aac001", outParam);
                    ybbh_r = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "akc020", outParam);
                    ickh_r = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "akc141", outParam);
                    ickzt_r = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "akb020", outParam);
                    wdbh = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "aka101", outParam);
                    yydj = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "akc021", outParam);
                    ylrylb = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "aab034", outParam);
                    fzxbh = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "aab300", outParam);
                    fzxmc = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "aab301", outParam);
                    xzqhdm = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "aaa146", outParam);
                    xzqhmc = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "aab001", outParam);
                    dwmc_r = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "aac003", outParam);
                    xm_r = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "aac004", outParam);
                    xb_r = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "akc023", outParam);
                    nl_r = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "aka150", outParam);
                    kbcs = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "skc054", outParam);
                    zydjh = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "skb010", outParam);
                    ksbm_r = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "skc037", outParam);
                    ksmc_r = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "akc193", outParam);
                    ryzdjbbm1 = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "skc038", outParam);
                    ryzdjbbm2 = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "skc039", outParam);
                    ryzdjbbm3 = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "skc616", outParam);
                    ryzdjbmc1 = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "skc617", outParam);
                    ryzdjbmc2 = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "skc618", outParam);
                    ryzdjbmc3 = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "akc192", outParam);
                    zyrq = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "akc196", outParam);
                    cyzdjbbm1 = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "skc040", outParam);
                    cyzdjbbm2 = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "skc041", outParam);
                    cyzdjbbm3 = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "skc619", outParam);
                    cyzdjbmc1 = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "skc620", outParam);
                    cyzdjbmc2 = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "skc621", outParam);
                    cyzdjbmc3 = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "akc194", outParam);
                    cyrq = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "skc042", outParam);
                    blcyrq = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "aae011", outParam);
                    jbr_r = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "aae036", outParam);
                    jbrq = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "skc043", outParam);
                    cydjr = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "skc033", outParam);
                    cxbz = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "skc034", outParam);
                    bcxlsh = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "aae040", outParam);
                    jsrq = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "skc035", outParam);
                    jsr = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "akc087", outParam);
                    grzhye = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "aae001", outParam);
                    jznd = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "vvv001", outParam);
                    ylrylbmc = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "vvv002", outParam);
                    xbmc = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "vvv003", outParam);
                    ickzt_r = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "vvv004", outParam);
                    yydjmc = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "aab300", outParam);
                    jbjgmc = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "aab004", outParam);
                    dwmc = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "akb021", outParam);
                    wdmc = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "vvv005", outParam);
                    zbqmc_ry = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "vvv006", outParam);
                    cbqmc_ry = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "vvv007", outParam);
                    sbqmc_ry = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "vvv008", outParam);
                    zbqmc_cy = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "vvv009", outParam);
                    cbqmc_cy = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "vvv010", outParam);
                    sbqmc_cy = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    #endregion

                    strSql = string.Format(@"insert into ybmzzydjdr(jzlsh,jylsh,ybjzlsh,grbh,kh,ickzt,wdbh,yydj,yllb,fzxbm,fzxmc,
                                            xzqhdm,xzqhmc,dwbh,dwmc,xm,xb,nl,bckbcs,zydjh,ksbh,
                                            ksmc,ghf,cxbz1,bcxlsh,jbr,ghdjsj,jsrq,jsr,zhye,jznd,
                                            yldyxz,xbmc,ickztmc,yydjmc,jbjgmc,wdmc,bzbm,bzmc,mmbzbm1,mmbzmc1,
                                            mmbzbm2,mmbzmc2,cyzdjbbm1,cyzdjbmc1,cyzdjbbm2,cyzdjbmc2,cyzdjbbm3,cyzdjbmc3,cyrq,zbqmc_ry,
                                            cbqmc_ry,sbqmc_ry,zbqmc_cy,cbqmc_cy,sbqmc_cy,sysdate,rylxdm,xzbm) values(
                                            '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}',
                                            '{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}','{20}',
                                            '{21}','{22}','{23}','{24}','{25}','{26}','{27}','{28}','{29}','{30}',
                                            '{31}','{32}','{33}','{34}','{35}','{36}','{37}','{38}','{39}','{40}',
                                            '{41}','{42}','{43}','{44}','{45}','{46}','{47}','{48}','{49}','{50}',
                                            '{51}','{52}','{53}','{54}','{55}','{56}','{57}','{58}')",
                                            jzlsh, jylsh, zylsh, ybbh_r, ickh_r, ickzt_r, wdbh, yydj, yllb, fzxbh, fzxmc,
                                            xzqhdm, xzqhmc, dwbh_r, dwmc_r, xm_r, xb_r, nl_r, kbcs, zydjh, ksbh,
                                            ksmc_r, "0.00", cxbz, bcxlsh, jbr_r, zyrq, jsrq, jsr, zhye, jznd,
                                            yllbmc, xbmc, icztmc, yydjmc, jbjgmc, wdmc, ryzdjbbm1, ryzdjbmc1, ryzdjbbm2, ryzdjbmc2,
                                            ryzdjbbm3, ryzdjbmc3, cyzdjbbm1, cyzdjbmc1, cyzdjbbm2, cyzdjbmc2, cyzdjbbm3, cyzdjbmc3, cyrq, zbqmc_ry,
                                            cbqmc_ry, sbqmc_ry, zbqmc_cy, cbqmc_cy, sbqmc_cy, sysdate, rylx, cbdwlx);
                    liSQL.Add(strSql);
                    strSql = string.Format(@"select z1zyno from zy01h where z1comp='1' and z1zyno='{0}' and left(z1endv,1)='1'", jzlsh);
                    ds.Tables.Clear();
                    ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                    if (ds.Tables[0].Rows.Count == 0)
                    {
                        strSql = string.Format(@"update zy01h set z1lynm='{0}',z1lyjg='{1}',z1ybno='{3}' where z1zyno='{2}'", lyjgmc, lyjgdm, jzlsh, grbh);
                        liSQL.Add(strSql);
                    }
                    object[] obj = liSQL.ToArray();
                    obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);
                    if (obj[1].ToString().Equals("1"))
                    {
                        WriteLog(sysdate + "  医保住院登记(医疗保险)成功|");
                        return new object[] { 0, 1, "医保住院登记(医疗保险)成功|" };
                    }
                    else
                    {
                        WriteLog(sysdate + "  医保住院登记(医疗保险)失败|数据操作失败|" + obj[2].ToString());
                        //撤销住院登记
                        object[] obj1 = { zylsh, ybbh_r };
                        N_YBZYDJCX_YLBX(obj1);
                        return new object[] { 0, 0, "医保住院登记(医疗保险)失败|数据操作失败|" + obj[2].ToString() };
                    }

                }
                else
                {
                    WriteLog(sysdate + "  获取住院登记(医疗保险)返回信息失败|" + sMsg1);
                    return new object[] { 0, 0, "获取住院登记(医疗保险)返回信息失败|" + sMsg1 };
                }
            }
            else
            {
                getFailReason(whandler, sMsg);
                sMsg1 = Encoding.Default.GetString(sMsg, 0, sMsg.Length).Replace("\0", "");
                WriteLog(sysdate + "  医保住院登记(医疗保险)失败|" + sMsg1);
                return new object[] { 0, 0, "医保住院登记(医疗保险)失败|" + sMsg1 };
            }

        }
        #endregion

        #region 住院登记_生育保险
        public static object[] YBZYDJ_SYBX(object[] objParam)
        {
            string sysdate = GetServerDateTime();//当前时间
            string jzlsh = objParam[0].ToString(); //就诊流水号
            string yllb = objParam[1].ToString(); //医疗类别代码
            string bzbm = objParam[2].ToString(); //病种编码
            string bzmc = objParam[3].ToString(); //病种名称
            string czygh = CliUtils.fLoginUser;  // 操作员工号
            string ywzqh = "";  // 业务周期号 
            string djsj = "";   // 登记时间(格式：DateTime.Now.ToString("yyyyMMdd HHmmss"))  //对应入院日期时间

            #region 读卡信息
            string[] kxx = objParam[4].ToString().Split('|');//读卡返回信息
            if (kxx.Length < 2)
                return new object[] { 0, 0, "无读卡信息反馈" };
            string grbh = kxx[0].ToString(); //个人编号  *//对应玉山 个人标识号
            string dwbh = kxx[1].ToString(); //单位编号
            string sfzh = kxx[2].ToString(); //身份证号
            string xm = kxx[3].ToString(); //姓名
            string xb = kxx[4].ToString(); //性别
            string mz = kxx[5].ToString(); //民族
            string csrq = kxx[6].ToString(); //出生日期
            string kh = kxx[7].ToString(); //卡号
            string yldylb = kxx[8].ToString(); //医疗待遇类别
            string rycbzt = kxx[9].ToString(); //人员参保状态
            string ydrybz = kxx[10].ToString(); //异地人员标志
            string tcqh = kxx[11].ToString(); //统筹区号
            string nd = kxx[12].ToString(); //年度
            string zyzt = kxx[13].ToString(); //在院状态
            string zhye = kxx[14].ToString(); //帐户余额
            string bnylflj = kxx[15].ToString(); //本年医疗费累计
            string bnzhzclj = kxx[16].ToString(); //本年帐户支出累计
            string bntczclj = kxx[17].ToString(); //本年统筹支出累计
            string bnjzjzclj = kxx[18].ToString(); //本年救助金支出累计
            string bngwybzjjlj = kxx[19].ToString(); //本年公务员补助基金累计
            string bnczjmmztczflj = kxx[20].ToString(); //本年城镇居民门诊统筹支付累计
            string jrtcfylj = kxx[21].ToString(); //进入统筹费用累计
            string jrjzjfylj = kxx[22].ToString(); //进入救助金费用累计
            string qfbzlj = kxx[23].ToString(); //起付标准累计
            string bnzycs = kxx[24].ToString(); //本年住院次数
            string dwmc = kxx[25].ToString(); //单位名称
            string nl = kxx[26].ToString(); //年龄
            string cbdwlx = kxx[27].ToString(); //参保单位类型
            string jbjgbm = kxx[28].ToString(); //经办机构编码
            #endregion

            string hznm = ""; //患者姓名
            string ksbh = "";   // 科室编号 
            string ksmc = "";   // 科室名称  //*对应 病区名称
            string ysdm = "";   // 医师代码
            string ysxm = "";  // 医师姓名
            string jbr = CliUtils.fUserName;   // 经办人姓名
            string cwh = "";    //床号  //*对应玉山 床位号
            string dqbh = tcqh;    //所属区号(地区编号)     
            string lyjgdm = objParam[5].ToString();//来源机构代码
            string lyjgmc = objParam[6].ToString();//来源机构名称
            string yllbmc = objParam[7].ToString();//医疗类别名称
            string dgysbm = "";// objParam[8].ToString(); //定岗医生代码
            string dgysxm = "";//objParam[9].ToString(); //定岗医生姓名
            string sfzh1 = objParam[10].ToString(); //身份证号
            string bzbm1 = objParam[11].ToString(); //病种编码1
            string bzmc1 = objParam[12].ToString(); //病种名称1
            string bzbm2 = objParam[13].ToString(); //病种编码2
            string bzmc2 = objParam[14].ToString(); //病种编码2
            string rylx = objParam[15].ToString();  //入院类型
            string lxdh = string.Empty;
            string jtdz = string.Empty;
            byte[] sMsg = new byte[1024];
            byte[] outParam = new byte[1024];
            string sMsg1 = string.Empty;
            string sValue = string.Empty;
            bool bfalg = false;

            if (string.IsNullOrEmpty(jzlsh))
                return new object[] { 0, 0, "就诊流水号不能为空" };
            if (string.IsNullOrEmpty(grbh))
                return new object[] { 0, 0, "个人编号不能为空" };
            if (string.IsNullOrEmpty(bzbm))
                return new object[] { 0, 0, "病种编码不能为空" };
            if (string.IsNullOrEmpty(bzmc))
                return new object[] { 0, 0, "病种名称不能为空" };

            string ybjzlsh = jzlsh + DateTime.Now.ToString("HHmmss");
            string strSql = string.Format(@"select a.z1zyno,a.z1hznm,a.z1date,b.z1ksno, b.z1ksnm,b.z1ysno,b.z1ysnm,b.z1bedn,a.z1mobi,a.z1yzbm from zy01h a
                                            left join zy01d b on a.z1zyno=b.z1zyno and a.z1ghno=b.z1ghno 
                                            where a.z1zyno='{0}'", jzlsh);
            DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            if (ds == null || ds.Tables[0].Rows.Count == 0)
                return new object[] { 0, 0, "无患者信息" };
            djsj = Convert.ToDateTime(ds.Tables[0].Rows[0]["z1date"].ToString()).ToString("yyyy-MM-dd HH:mm:ss");
            hznm = ds.Tables[0].Rows[0]["z1hznm"].ToString();
            cwh = ds.Tables[0].Rows[0]["z1bedn"].ToString();
            ksbh = ds.Tables[0].Rows[0]["z1ksno"].ToString();
            ksmc = ds.Tables[0].Rows[0]["z1ksnm"].ToString();
            ysdm = ds.Tables[0].Rows[0]["z1ysno"].ToString();
            ysxm = ds.Tables[0].Rows[0]["z1ysnm"].ToString();
            lxdh = ds.Tables[0].Rows[0]["z1mobi"].ToString();
            jtdz = ds.Tables[0].Rows[0]["z1yzbm"].ToString();

            string ryrq = Convert.ToDateTime(ds.Tables[0].Rows[0]["z1date"]).ToString("yyyy-MM-dd");
            string jylsh = DateTime.Now.ToString("yyyyMMddHHmmss") + "-102002-" + new Random().Next(100).ToString().PadLeft(4, '0');

            strSql = string.Format(@"select * from ybmzzydjdr where jzlsh='{0}' and cxbz=1", jzlsh);
            ds.Tables.Clear();
            ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            if (ds.Tables[0].Rows.Count > 0)
                return new object[] { 0, 0, "患者" + jzlsh + "已做医保补办登记" };

            WriteLog(sysdate + "  进入医保住院登记(生育保险)...");
            //入参
            putPara(whandler, "usr", ybuserid);
            putPara(whandler, "pwd", ybpasswd);

            putPara(whandler, "aac001", grbh);
            putPara(whandler, "skc037", ksmc);
            putPara(whandler, "akc192", ryrq);
            putPara(whandler, "akc193", bzbm);
            putPara(whandler, "skc038", bzbm1);
            putPara(whandler, "skc039", bzbm2);
            putPara(whandler, "skc113", "S");

            WriteLog("入参|" + ybuserid + "|" + ybpasswd + "|" + grbh + "|" + ksmc + "|" + ryrq + "|" + bzbm + "|"
                    + bzbm1 + "|" + bzbm2 + "|S" );

            bfalg = process(whandler, "F04.06.01.01");
            if (bfalg)
            {
                List<string> liSQL = new List<string>();
                getErrMsg(whandler, sMsg);
                sMsg1 = Encoding.Default.GetString(sMsg, 0, sMsg.Length).Replace("\0", "");
                if (sMsg1.ToUpper().Trim().Equals("OK"))
                {
                    #region 出参变量
                    string zylsh = string.Empty;    //住院流水号（医保中心系统生成）
                    string ybbh_r = string.Empty;   //医保编号
                    string ickh_r = string.Empty;   //IC卡号
                    string ickzt_r = string.Empty;  //IC卡状态
                    string wdbh = string.Empty;     //网点编号（医院、药店、门诊编号）
                    string yydj = string.Empty;     //医院等级         
                    string ylrylb = string.Empty;   //医疗人员类别
                    string fzxbh = string.Empty;    //分中心编码（网点所属分中心）
                    string fzxmc = string.Empty;    //分中心名称（网点所属分中心）
                    string xzqhdm = string.Empty;   //行政区划代码(参保人员所属)
                    string xzqhmc = string.Empty;   //行政区划名称(参保人员所属)
                    string dwbh_r = string.Empty;   //单位编号
                    string xm_r = string.Empty;//姓名
                    string xb_r = string.Empty;//性别
                    string nl_r = string.Empty;//年龄
                    string kbcs = string.Empty;//看病次数
                    string zydjh = string.Empty;//转院登记号
                    string ksbm_r = string.Empty;//科室编码（必填）
                    string ksmc_r = string.Empty;//挂号科室名称
                    string ryzdjbbm1 = string.Empty;    //入院诊断疾病编码（主）  <非空>
                    string ryzdjbbm2 = string.Empty;    //入院诊断疾病编码（次）
                    string ryzdjbbm3 = string.Empty;    //入院诊断疾病编码（第三）
                    string ryzdjbmc1 = string.Empty;    //入院诊断疾病中文名称（主）  <非空>
                    string ryzdjbmc2 = string.Empty;    //入院诊断疾病中文名称（次）
                    string ryzdjbmc3 = string.Empty;    //入院诊断疾病中文名称（第三）
                    string zyrq = string.Empty;         //住院日期
                    string cyzdjbbm1 = string.Empty;    //出院疾病诊断编码       <非空
                    string cyzdjbbm2 = string.Empty;    //出院诊断疾病编码（次）
                    string cyzdjbbm3 = string.Empty;    //出院诊断疾病编码（第三）
                    string cyzdjbmc1 = string.Empty;    //出院诊断疾病中文名称（主）  <非空>
                    string cyzdjbmc2 = string.Empty;    //出院诊断疾病中文名称（次）
                    string cyzdjbmc3 = string.Empty;    //出院诊断疾病中文名称（第三）
                    string cyrq = string.Empty;         //出院日期
                    string blcyrq = string.Empty;       //办理出院日期（可能比出院日期晚）
                    string jbr_r = string.Empty;//经办人(医院办理挂号工作人员姓名)
                    string jbrq = string.Empty;//经办日期(挂号日期)
                    string cydjr = string.Empty;    //出院登记人
                    string cxbz = string.Empty;//冲销标志（Z 正常；+ 被冲销；- 冲销）
                    string bcxlsh = string.Empty;//被冲销流水号
                    string jsrq = string.Empty; //结算日期
                    string jsr = string.Empty;//结算人
                    string grzhye = string.Empty;   //个人帐户余额
                    string ylrylbmc = string.Empty; //医疗人员类别名称
                    string jznd = string.Empty; //结转年度
                    string xbmc = string.Empty; //性别名称
                    string icztmc = string.Empty; //IC卡状态名称
                    string yydjmc = string.Empty;   //医院等级名称
                    string jbjgmc = string.Empty;   //经办机构名称
                    string dwmc_r = string.Empty;   //单位名称
                    string wdmc = string.Empty;     //网点名称
                    string zbqmc_ry = string.Empty; //主病情名称(入院)
                    string cbqmc_ry = string.Empty; //次病情名称(入院)
                    string sbqmc_ry = string.Empty; //三病情名称(入院)
                    string zbqmc_cy = string.Empty; //主病情名称（出院）
                    string cbqmc_cy = string.Empty; //次病情名称（出院）  
                    string sbqmc_cy = string.Empty; //三病情名称（出院）
                    #endregion

                    #region 获取出参数据
                    getParaByName(whandler, "akc190", outParam);
                    zylsh = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "aac001", outParam);
                    ybbh_r = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "akc020", outParam);
                    ickh_r = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "akc141", outParam);
                    ickzt_r = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "akb020", outParam);
                    wdbh = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "aka101", outParam);
                    yydj = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "akc021", outParam);
                    ylrylb = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "aab034", outParam);
                    fzxbh = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "aab300", outParam);
                    fzxmc = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "aab301", outParam);
                    xzqhdm = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "aaa146", outParam);
                    xzqhmc = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "aab001", outParam);
                    dwmc_r = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "aac003", outParam);
                    xm_r = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "aac004", outParam);
                    xb_r = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "akc023", outParam);
                    nl_r = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "aka150", outParam);
                    kbcs = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "skc054", outParam);
                    zydjh = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "skb010", outParam);
                    ksbm_r = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "skc037", outParam);
                    ksmc_r = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "akc193", outParam);
                    ryzdjbbm1 = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "skc038", outParam);
                    ryzdjbbm2 = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "skc039", outParam);
                    ryzdjbbm3 = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "skc616", outParam);
                    ryzdjbmc1 = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "skc617", outParam);
                    ryzdjbmc2 = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "skc618", outParam);
                    ryzdjbmc3 = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "akc192", outParam);
                    zyrq = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "akc196", outParam);
                    cyzdjbbm1 = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "skc040", outParam);
                    cyzdjbbm2 = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "skc041", outParam);
                    cyzdjbbm3 = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "skc619", outParam);
                    cyzdjbmc1 = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "skc620", outParam);
                    cyzdjbmc2 = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "skc621", outParam);
                    cyzdjbmc3 = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "akc194", outParam);
                    cyrq = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "skc042", outParam);
                    blcyrq = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "aae011", outParam);
                    jbr_r = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "aae036", outParam);
                    jbrq = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "skc043", outParam);
                    cydjr = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "skc033", outParam);
                    cxbz = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "skc034", outParam);
                    bcxlsh = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "aae040", outParam);
                    jsrq = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "skc035", outParam);
                    jsr = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "akc087", outParam);
                    grzhye = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "aae001", outParam);
                    jznd = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "vvv001", outParam);
                    ylrylbmc = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "vvv002", outParam);
                    xbmc = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "vvv003", outParam);
                    ickzt_r = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "vvv004", outParam);
                    yydjmc = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "aab300", outParam);
                    jbjgmc = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "aab004", outParam);
                    dwmc = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "akb021", outParam);
                    wdmc = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "vvv005", outParam);
                    zbqmc_ry = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "vvv006", outParam);
                    cbqmc_ry = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "vvv007", outParam);
                    sbqmc_ry = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "vvv008", outParam);
                    zbqmc_cy = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "vvv009", outParam);
                    cbqmc_cy = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "vvv010", outParam);
                    sbqmc_cy = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    #endregion

                    strSql = string.Format(@"insert into ybmzzydjdr(jzlsh,jylsh,ybjzlsh,grbh,kh,ickzt,wdbh,yydj,yllb,fzxbm,fzxmc,
                                            xzqhdm,xzqhmc,dwbh,dwmc,xm,xb,nl,bckbcs,zydjh,ksbh,
                                            ksmc,ghf,cxbz1,bcxlsh,jbr,ghdjsj,jsrq,jsr,zhye,jznd,
                                            yldyxz,xbmc,ickztmc,yydjmc,jbjgmc,wdmc,bzbm,bzmc,mmbzbm1,mmbzmc1,
                                            mmbzbm2,mmbzmc2,cyzdjbbm1,cyzdjbmc1,cyzdjbbm2,cyzdjbmc2,cyzdjbbm3,cyzdjbmc3,cyrq,zbqmc_ry,
                                            cbqmc_ry,sbqmc_ry,zbqmc_cy,cbqmc_cy,sbqmc_cy,sysdate,rylxdm,xzbm) values(
                                            '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}',
                                            '{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}','{20}',
                                            '{21}','{22}','{23}','{24}','{25}','{26}','{27}','{28}','{29}','{30}',
                                            '{31}','{32}','{33}','{34}','{35}','{36}','{37}','{38}','{39}','{40}',
                                            '{41}','{42}','{43}','{44}','{45}','{46}','{47}','{48}','{49}','{50}',
                                            '{51}','{52}','{53}','{54}','{55}','{56}','{57}','{58}')",
                                            jzlsh, jylsh, zylsh, ybbh_r, ickh_r, ickzt_r, wdbh, yydj, yllb, fzxbh, fzxmc,
                                            xzqhdm, xzqhmc, dwbh_r, dwmc_r, xm_r, xb_r, nl_r, kbcs, zydjh, ksbh,
                                            ksmc_r, "0.00", cxbz, bcxlsh, jbr_r, zyrq, jsrq, jsr, zhye, jznd,
                                            yllbmc, xbmc, icztmc, yydjmc, jbjgmc, wdmc, ryzdjbbm1, ryzdjbmc1, ryzdjbbm2, ryzdjbmc2,
                                            ryzdjbbm3, ryzdjbmc3, cyzdjbbm1, cyzdjbmc1, cyzdjbbm2, cyzdjbmc2, cyzdjbbm3, cyzdjbmc3, cyrq, zbqmc_ry,
                                            cbqmc_ry, sbqmc_ry, zbqmc_cy, cbqmc_cy, sbqmc_cy, sysdate, rylx, cbdwlx);
                    liSQL.Add(strSql);
                    strSql = string.Format(@"select z1zyno from zy01h where z1comp='1' and z1zyno='{0}' and left(z1endv,1)='1'", jzlsh);
                    ds.Tables.Clear();
                    ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                    if (ds.Tables[0].Rows.Count == 0)
                    {
                        strSql = string.Format(@"update zy01h set z1lynm='{0}',z1lyjg='{1}',z1ybno='{3}' where z1zyno='{2}'", lyjgmc, lyjgdm, jzlsh, grbh);
                        liSQL.Add(strSql);
                    }
                    object[] obj = liSQL.ToArray();
                    obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);
                    if (obj[1].ToString().Equals("1"))
                    {
                        WriteLog(sysdate + "  医保住院登记(生育保险)成功|");
                        return new object[] { 0, 1, "医保住院登记(生育保险)成功|" };
                    }
                    else
                    {
                        WriteLog(sysdate + "  医保住院登记(生育保险)失败|数据操作失败|" + obj[2].ToString());
                        //撤销住院登记
                        object[] obj1 = { zylsh, ybbh_r };
                        N_YBZYDJCX_SYBX(obj1);
                        return new object[] { 0, 0, "医保住院登记(生育保险)失败|数据操作失败|" + obj[2].ToString() };
                    }
                }
                else
                {
                    WriteLog(sysdate + "  获取住院登记(生育保险)返回信息失败|" + sMsg1);
                    return new object[] { 0, 0, "获取住院登记(生育保险)返回信息失败|" + sMsg1 };
                }
            }
            else
            {
                getFailReason(whandler, sMsg);
                sMsg1 = Encoding.Default.GetString(sMsg, 0, sMsg.Length).Replace("\0", "");
                WriteLog(sysdate + "  医保住院登记(医疗保险)失败|" + sMsg1);
                return new object[] { 0, 0, "医保住院登记(医疗保险)失败|" + sMsg1 };
            }

        }
        #endregion

        #region 住院登记_工伤保险
        public static object[] YBZYDJ_GSBX(object[] objParam)
        {
            string sysdate = GetServerDateTime();//当前时间
            string jzlsh = objParam[0].ToString(); //就诊流水号
            string yllb = objParam[1].ToString(); //医疗类别代码
            string bzbm = objParam[2].ToString(); //病种编码
            string bzmc = objParam[3].ToString(); //病种名称
            string czygh = CliUtils.fLoginUser;  // 操作员工号
            string ywzqh = "";  // 业务周期号 
            string djsj = "";   // 登记时间(格式：DateTime.Now.ToString("yyyyMMdd HHmmss"))  //对应入院日期时间

            #region 读卡信息
            string[] kxx = objParam[4].ToString().Split('|');//读卡返回信息
            if (kxx.Length < 2)
                return new object[] { 0, 0, "无读卡信息反馈" };
            string grbh = kxx[0].ToString(); //个人编号  *//对应玉山 个人标识号
            string dwbh = kxx[1].ToString(); //单位编号
            string sfzh = kxx[2].ToString(); //身份证号
            string xm = kxx[3].ToString(); //姓名
            string xb = kxx[4].ToString(); //性别
            string mz = kxx[5].ToString(); //民族
            string csrq = kxx[6].ToString(); //出生日期
            string kh = kxx[7].ToString(); //卡号
            string yldylb = kxx[8].ToString(); //医疗待遇类别
            string rycbzt = kxx[9].ToString(); //人员参保状态
            string ydrybz = kxx[10].ToString(); //异地人员标志
            string tcqh = kxx[11].ToString(); //统筹区号
            string nd = kxx[12].ToString(); //年度
            string zyzt = kxx[13].ToString(); //在院状态
            string zhye = kxx[14].ToString(); //帐户余额
            string bnylflj = kxx[15].ToString(); //本年医疗费累计
            string bnzhzclj = kxx[16].ToString(); //本年帐户支出累计
            string bntczclj = kxx[17].ToString(); //本年统筹支出累计
            string bnjzjzclj = kxx[18].ToString(); //本年救助金支出累计
            string bngwybzjjlj = kxx[19].ToString(); //本年公务员补助基金累计
            string bnczjmmztczflj = kxx[20].ToString(); //本年城镇居民门诊统筹支付累计
            string jrtcfylj = kxx[21].ToString(); //进入统筹费用累计
            string jrjzjfylj = kxx[22].ToString(); //进入救助金费用累计
            string qfbzlj = kxx[23].ToString(); //起付标准累计
            string bnzycs = kxx[24].ToString(); //本年住院次数
            string dwmc = kxx[25].ToString(); //单位名称
            string nl = kxx[26].ToString(); //年龄
            string cbdwlx = kxx[27].ToString(); //参保单位类型
            string jbjgbm = kxx[28].ToString(); //经办机构编码
            #endregion

            string hznm = ""; //患者姓名
            string ksbh = "";   // 科室编号 
            string ksmc = "";   // 科室名称  //*对应 病区名称
            string ysdm = "";   // 医师代码
            string ysxm = "";  // 医师姓名
            string jbr = CliUtils.fUserName;   // 经办人姓名
            string cwh = "";    //床号  //*对应玉山 床位号
            string dqbh = tcqh;    //所属区号(地区编号)     
            string lyjgdm = objParam[5].ToString();//来源机构代码
            string lyjgmc = objParam[6].ToString();//来源机构名称
            string yllbmc = objParam[7].ToString();//医疗类别名称
            string dgysbm = "";// objParam[8].ToString(); //定岗医生代码
            string dgysxm = "";//objParam[9].ToString(); //定岗医生姓名
            string sfzh1 = objParam[10].ToString(); //身份证号
            string bzbm1 = objParam[11].ToString(); //病种编码1
            string bzmc1 = objParam[12].ToString(); //病种名称1
            string bzbm2 = objParam[13].ToString(); //病种编码2
            string bzmc2 = objParam[14].ToString(); //病种编码2
            string rylx = objParam[15].ToString();  //入院类型
            string lxdh = string.Empty;
            string jtdz = string.Empty;
            byte[] sMsg = new byte[1024];
            byte[] outParam = new byte[1024];
            string sMsg1 = string.Empty;
            string sValue = string.Empty;
            bool bfalg = false;

            if (string.IsNullOrEmpty(jzlsh))
                return new object[] { 0, 0, "就诊流水号不能为空" };
            if (string.IsNullOrEmpty(grbh))
                return new object[] { 0, 0, "个人编号不能为空" };
            if (string.IsNullOrEmpty(bzbm))
                return new object[] { 0, 0, "病种编码不能为空" };
            if (string.IsNullOrEmpty(bzmc))
                return new object[] { 0, 0, "病种名称不能为空" };

            string ybjzlsh = jzlsh + DateTime.Now.ToString("HHmmss");
            string strSql = string.Format(@"select a.z1zyno,a.z1hznm,a.z1date,b.z1ksno, b.z1ksnm,b.z1ysno,b.z1ysnm,b.z1bedn,a.z1mobi,a.z1yzbm from zy01h a
                                            left join zy01d b on a.z1zyno=b.z1zyno and a.z1ghno=b.z1ghno 
                                            where a.z1zyno='{0}'", jzlsh);
            DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            if (ds == null || ds.Tables[0].Rows.Count == 0)
                return new object[] { 0, 0, "无患者信息" };
            djsj = Convert.ToDateTime(ds.Tables[0].Rows[0]["z1date"].ToString()).ToString("yyyy-MM-dd HH:mm:ss");
            hznm = ds.Tables[0].Rows[0]["z1hznm"].ToString();
            cwh = ds.Tables[0].Rows[0]["z1bedn"].ToString();
            ksbh = ds.Tables[0].Rows[0]["z1ksno"].ToString();
            ksmc = ds.Tables[0].Rows[0]["z1ksnm"].ToString();
            ysdm = ds.Tables[0].Rows[0]["z1ysno"].ToString();
            ysxm = ds.Tables[0].Rows[0]["z1ysnm"].ToString();
            lxdh = ds.Tables[0].Rows[0]["z1mobi"].ToString();
            jtdz = ds.Tables[0].Rows[0]["z1yzbm"].ToString();

            string ryrq = Convert.ToDateTime(ds.Tables[0].Rows[0]["z1date"]).ToString("yyyy-MM-dd");
            string jylsh = DateTime.Now.ToString("yyyyMMddHHmmss") + "-102002-" + new Random().Next(100).ToString().PadLeft(4, '0');

            strSql = string.Format(@"select * from ybmzzydjdr where jzlsh='{0}' and cxbz=1", jzlsh);
            ds.Tables.Clear();
            ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            if (ds.Tables[0].Rows.Count > 0)
                return new object[] { 0, 0, "患者" + jzlsh + "已做医保补办登记" };

            WriteLog(sysdate + "  进入医保住院登记(工伤保险)...");
            //入参
            putPara(whandler, "usr", ybuserid);
            putPara(whandler, "pwd", ybpasswd);

            putPara(whandler, "aac001", grbh);
            putPara(whandler, "skc037", ksmc);
            putPara(whandler, "akc192", ryrq);
            putPara(whandler, "akc193", bzbm);
            putPara(whandler, "skc038", bzbm1);
            putPara(whandler, "skc039", bzbm2);

            WriteLog("入参|" + ybuserid + "|" + ybpasswd + "|" + grbh + "|" + ksbh + "|" + ksmc + "|" + rylx + "|" + ryrq + "|" + bzbm + "|"
                    + bzbm1 + "|" + bzbm2 + "|" + bzmc + "|" + bzmc1 + "|" + bzmc2 + "|" + lxdh + "|" + jtdz);

            bfalg = process(whandler, "F07.11.01.01");
            if (bfalg)
            {
                List<string> liSQL = new List<string>();
                getErrMsg(whandler, sMsg);
                sMsg1 = Encoding.Default.GetString(sMsg, 0, sMsg.Length).Replace("\0", "");
                if (sMsg1.ToUpper().Trim().Equals("OK"))
                {
                    #region 出参变量
                    string zylsh = string.Empty;    //住院流水号（医保中心系统生成）
                    string ybbh_r = string.Empty;   //医保编号
                    string ickh_r = string.Empty;   //IC卡号
                    string ickzt_r = string.Empty;  //IC卡状态
                    string wdbh = string.Empty;     //网点编号（医院、药店、门诊编号）
                    string yydj = string.Empty;     //医院等级         
                    string ylrylb = string.Empty;   //医疗人员类别
                    string fzxbh = string.Empty;    //分中心编码（网点所属分中心）
                    string fzxmc = string.Empty;    //分中心名称（网点所属分中心）
                    string xzqhdm = string.Empty;   //行政区划代码(参保人员所属)
                    string xzqhmc = string.Empty;   //行政区划名称(参保人员所属)
                    string dwbh_r = string.Empty;   //单位编号
                    string xm_r = string.Empty;//姓名
                    string xb_r = string.Empty;//性别
                    string nl_r = string.Empty;//年龄
                    string kbcs = string.Empty;//看病次数
                    string zydjh = string.Empty;//转院登记号
                    string ksbm_r = string.Empty;//科室编码（必填）
                    string ksmc_r = string.Empty;//挂号科室名称
                    string ryzdjbbm1 = string.Empty;    //入院诊断疾病编码（主）  <非空>
                    string ryzdjbbm2 = string.Empty;    //入院诊断疾病编码（次）
                    string ryzdjbbm3 = string.Empty;    //入院诊断疾病编码（第三）
                    string ryzdjbmc1 = string.Empty;    //入院诊断疾病中文名称（主）  <非空>
                    string ryzdjbmc2 = string.Empty;    //入院诊断疾病中文名称（次）
                    string ryzdjbmc3 = string.Empty;    //入院诊断疾病中文名称（第三）
                    string zyrq = string.Empty;         //住院日期
                    string cyzdjbbm1 = string.Empty;    //出院疾病诊断编码       <非空
                    string cyzdjbbm2 = string.Empty;    //出院诊断疾病编码（次）
                    string cyzdjbbm3 = string.Empty;    //出院诊断疾病编码（第三）
                    string cyzdjbmc1 = string.Empty;    //出院诊断疾病中文名称（主）  <非空>
                    string cyzdjbmc2 = string.Empty;    //出院诊断疾病中文名称（次）
                    string cyzdjbmc3 = string.Empty;    //出院诊断疾病中文名称（第三）
                    string cyrq = string.Empty;         //出院日期
                    string blcyrq = string.Empty;       //办理出院日期（可能比出院日期晚）
                    string jbr_r = string.Empty;//经办人(医院办理挂号工作人员姓名)
                    string jbrq = string.Empty;//经办日期(挂号日期)
                    string cydjr = string.Empty;    //出院登记人
                    string cxbz = string.Empty;//冲销标志（Z 正常；+ 被冲销；- 冲销）
                    string bcxlsh = string.Empty;//被冲销流水号
                    string jsrq = string.Empty; //结算日期
                    string jsr = string.Empty;//结算人
                    string grzhye = string.Empty;   //个人帐户余额
                    string ylrylbmc = string.Empty; //医疗人员类别名称
                    string jznd = string.Empty; //结转年度
                    string xbmc = string.Empty; //性别名称
                    string icztmc = string.Empty; //IC卡状态名称
                    string yydjmc = string.Empty;   //医院等级名称
                    string jbjgmc = string.Empty;   //经办机构名称
                    string dwmc_r = string.Empty;   //单位名称
                    string wdmc = string.Empty;     //网点名称
                    string zbqmc_ry = string.Empty; //主病情名称(入院)
                    string cbqmc_ry = string.Empty; //次病情名称(入院)
                    string sbqmc_ry = string.Empty; //三病情名称(入院)
                    string zbqmc_cy = string.Empty; //主病情名称（出院）
                    string cbqmc_cy = string.Empty; //次病情名称（出院）  
                    string sbqmc_cy = string.Empty; //三病情名称（出院）
                    #endregion

                    #region 获取出参数据
                    getParaByName(whandler, "akc190", outParam);
                    zylsh = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "aac001", outParam);
                    ybbh_r = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "akc020", outParam);
                    ickh_r = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "akc141", outParam);
                    ickzt_r = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    //outParam = new byte[1024];
                    //getParaByName(whandler, "akb020", outParam);
                    //wdbh = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "aka101", outParam);
                    yydj = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "akc021", outParam);
                    ylrylb = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "aab034", outParam);
                    fzxbh = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    //outParam = new byte[1024];
                    //getParaByName(whandler, "aab300", outParam);
                    //fzxmc = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    //outParam = new byte[1024];
                    //getParaByName(whandler, "aab301", outParam);
                    //xzqhdm = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "aaa146", outParam);
                    xzqhmc = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "aab001", outParam);
                    dwmc_r = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "aac003", outParam);
                    xm_r = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "aac004", outParam);
                    xb_r = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "akc023", outParam);
                    nl_r = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "aka150", outParam);
                    kbcs = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "skc054", outParam);
                    zydjh = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "skb010", outParam);
                    ksbm_r = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "skc037", outParam);
                    ksmc_r = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "akc193", outParam);
                    ryzdjbbm1 = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "skc038", outParam);
                    ryzdjbbm2 = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "skc039", outParam);
                    ryzdjbbm3 = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    //outParam = new byte[1024];
                    //getParaByName(whandler, "skc616", outParam);
                    //ryzdjbmc1 = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    //outParam = new byte[1024];
                    //getParaByName(whandler, "skc617", outParam);
                    //ryzdjbmc2 = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    //outParam = new byte[1024];
                    //getParaByName(whandler, "skc618", outParam);
                    //ryzdjbmc3 = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "akc192", outParam);
                    zyrq = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "akc196", outParam);
                    cyzdjbbm1 = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "skc040", outParam);
                    cyzdjbbm2 = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "skc041", outParam);
                    cyzdjbbm3 = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    //outParam = new byte[1024];
                    //getParaByName(whandler, "skc619", outParam);
                    //cyzdjbmc1 = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    //outParam = new byte[1024];
                    //getParaByName(whandler, "skc620", outParam);
                    //cyzdjbmc2 = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    //outParam = new byte[1024];
                    //getParaByName(whandler, "skc621", outParam);
                    //cyzdjbmc3 = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "akc194", outParam);
                    cyrq = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "skc042", outParam);
                    blcyrq = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "aae011", outParam);
                    jbr_r = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "aae036", outParam);
                    jbrq = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "skc043", outParam);
                    cydjr = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "skc033", outParam);
                    cxbz = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "skc034", outParam);
                    bcxlsh = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "aae040", outParam);
                    jsrq = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "skc035", outParam);
                    jsr = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "akc087", outParam);
                    grzhye = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "aae001", outParam);
                    jznd = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "vvv001", outParam);
                    ylrylbmc = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "vvv002", outParam);
                    xbmc = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "vvv003", outParam);
                    ickzt_r = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "vvv004", outParam);
                    yydjmc = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "aab300", outParam);
                    jbjgmc = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "aab004", outParam);
                    dwmc = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "akb021", outParam);
                    wdmc = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "vvv005", outParam);
                    zbqmc_ry = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "vvv006", outParam);
                    cbqmc_ry = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "vvv007", outParam);
                    sbqmc_ry = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "vvv008", outParam);
                    zbqmc_cy = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "vvv009", outParam);
                    cbqmc_cy = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "vvv010", outParam);
                    sbqmc_cy = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    #endregion

                    strSql = string.Format(@"insert into ybmzzydjdr(jzlsh,jylsh,ybjzlsh,grbh,kh,ickzt,wdbh,yydj,yllb,fzxbm,fzxmc,
                                            xzqhdm,xzqhmc,dwbh,dwmc,xm,xb,nl,bckbcs,zydjh,ksbh,
                                            ksmc,ghf,cxbz1,bcxlsh,jbr,ghdjsj,jsrq,jsr,zhye,jznd,
                                            yldyxz,xbmc,ickztmc,yydjmc,jbjgmc,wdmc,bzbm,bzmc,mmbzbm1,mmbzmc1,
                                            mmbzbm2,mmbzmc2,cyzdjbbm1,cyzdjbmc1,cyzdjbbm2,cyzdjbmc2,cyzdjbbm3,cyzdjbmc3,cyrq,zbqmc_ry,
                                            cbqmc_ry,sbqmc_ry,zbqmc_cy,cbqmc_cy,sbqmc_cy,sysdate,rylxdm,xzbm) values(
                                            '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}',
                                            '{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}','{20}',
                                            '{21}','{22}','{23}','{24}','{25}','{26}','{27}','{28}','{29}','{30}',
                                            '{31}','{32}','{33}','{34}','{35}','{36}','{37}','{38}','{39}','{40}',
                                            '{41}','{42}','{43}','{44}','{45}','{46}','{47}','{48}','{49}','{50}',
                                            '{51}','{52}','{53}','{54}','{55}','{56}','{57}','{58}')",
                                            jzlsh, jylsh, zylsh, ybbh_r, ickh_r, ickzt_r, wdbh, yydj, yllb, fzxbh, fzxmc,
                                            xzqhdm, xzqhmc, dwbh_r, dwmc_r, xm_r, xb_r, nl_r, kbcs, zydjh, ksbh,
                                            ksmc_r, "0.00", cxbz, bcxlsh, jbr_r, zyrq, jsrq, jsr, zhye, jznd,
                                            yllbmc, xbmc, icztmc, yydjmc, jbjgmc, wdmc, ryzdjbbm1, ryzdjbmc1, ryzdjbbm2, ryzdjbmc2,
                                            ryzdjbbm3, ryzdjbmc3, cyzdjbbm1, cyzdjbmc1, cyzdjbbm2, cyzdjbmc2, cyzdjbbm3, cyzdjbmc3, cyrq, zbqmc_ry,
                                            cbqmc_ry, sbqmc_ry, zbqmc_cy, cbqmc_cy, sbqmc_cy, sysdate, rylx, cbdwlx);
                    liSQL.Add(strSql);
                    strSql = string.Format(@"select z1zyno from zy01h where z1comp='1' and z1zyno='{0}' and left(z1endv,1)='1'", jzlsh);
                    ds.Tables.Clear();
                    ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                    if (ds.Tables[0].Rows.Count == 0)
                    {
                        strSql = string.Format(@"update zy01h set z1lynm='{0}',z1lyjg='{1}',z1ybno='{3}' where z1zyno='{2}'", lyjgmc, lyjgdm, jzlsh, grbh);
                        liSQL.Add(strSql);
                    }
                    object[] obj = liSQL.ToArray();
                    obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);
                    if (obj[1].ToString().Equals("1"))
                    {
                        WriteLog(sysdate + "  医保住院登记(医疗保险)成功|");
                        return new object[] { 0, 1, "医保住院登记(医疗保险)成功|" };
                    }
                    else
                    {
                        WriteLog(sysdate + "  医保住院登记(医疗保险)失败|数据操作失败|" + obj[2].ToString());
                        //撤销住院登记
                        object[] obj1 = { zylsh, ybbh_r };
                        N_YBZYDJCX_GSBX(obj1);
                        return new object[] { 0, 0, "医保住院登记(医疗保险)失败|数据操作失败|" + obj[2].ToString() };
                    }

                }
                else
                {
                    WriteLog(sysdate + "  获取住院登记(医疗保险)返回信息失败|" + sMsg1);
                    return new object[] { 0, 0, "获取住院登记(医疗保险)返回信息失败|" + sMsg1 };
                }
            }
            else
            {
                getFailReason(whandler, sMsg);
                sMsg1 = Encoding.Default.GetString(sMsg, 0, sMsg.Length).Replace("\0", "");
                WriteLog(sysdate + "  医保住院登记(医疗保险)失败|" + sMsg1);
                return new object[] { 0, 0, "医保住院登记(医疗保险)失败|" + sMsg1 };
            }

        }
        #endregion



        #region 住院登记撤销
        public static object[] YBZYDJCX(object[] objParam)
        {
            string jzlsh = objParam[0].ToString();  // 就诊流水号 *//对应玉山 住院号
            if (string.IsNullOrEmpty(jzlsh))
                return new object[] { 0, 0, "就诊流水号不能为空" };
            string strSql = string.Format(@"select xzbm from ybmzzydjdr where jzlsh='{0}' and cxbz=1", objParam[0].ToString());
            DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            if (ds == null || ds.Tables[0].Rows.Count == 0)
                return new object[] { 0, 0, "未做医保住院登记信息" };
            string sxzbm = ds.Tables[0].Rows[0]["xzbm"].ToString();
            object[] objReturn = null;
            switch (sxzbm)
            {
                case "310":
                    objReturn = YBZYDJCX_YLBX(objParam);
                    break;
                case "410":
                    objReturn = YBZYDJCX_GSBX(objParam);
                    break;
                case "510":
                    objReturn = YBZYDJCX_SYBX(objParam);
                    break;
                default:
                    objReturn = new object[] { 0, 0, "未找到方法体" };
                    break;
            }
            return objReturn;
        }
        #endregion

        #region 住院登记撤销_医疗保险
        public static object[] YBZYDJCX_YLBX(object[] objParam)
        {
            string sysdate = GetServerDateTime();//当前时间
            string czygh = CliUtils.fLoginUser;  // 操作员工号
            string ywzqh = "";  // 业务周期号
            string jzlsh = objParam[0].ToString();  // 就诊流水号 *//对应玉山 住院号
            string jbr = CliUtils.fUserName;    // 经办人姓名
            string yllb = "";   // 医疗类别代码
            //交易流水号
            if (string.IsNullOrEmpty(jzlsh))
                return new object[] { 0, 0, "就诊流水号不能为空" };
            string strSql = string.Format("select * from ybcfmxscindr a where a.jzlsh = '{0}' and a.cxbz = 1", jzlsh);
            DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);

            if (ds != null && ds.Tables[0].Rows.Count > 0)
                return new object[] { 0, 0, "已上传费用明细不能冲销入院登记" };

            strSql = string.Format("select * from ybmzzydjdr a where a.jzlsh = '{0}' and a.cxbz = 1", jzlsh);
            ds.Tables.Clear();
            ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            if (ds == null || ds.Tables[0].Rows.Count == 0)
                return new object[] { 0, 0, "未办理入院登记" };

            DataRow dr = ds.Tables[0].Rows[0];
            string ybjzlsh = dr["ybjzlsh"].ToString();   // 医保就诊流水号
            string ybbh = dr["grbh"].ToString();

            bool bfalg = false;
            byte[] sMsg = new byte[1024];
            byte[] outParam = new byte[1024];
            string sMsg1 = string.Empty;

            strSql = string.Format(@"select b6lyno,b6lynm from bz06h  where left(b6lyjg,1)=2 ");
            ds.Tables.Clear();
            ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            if (ds.Tables[0].Rows.Count == 0)
            {
                return new object[] { 0, 0, "获取bz06h缴费类型出错" };
            }
            string lyjg = ds.Tables[0].Rows[0]["b6lyno"].ToString();
            string lynm = ds.Tables[0].Rows[0]["b6lynm"].ToString();

            WriteLog(sysdate + "  进入住院登记撤销(医疗保险)...");

            //入参
            putPara(whandler, "usr", ybuserid);
            putPara(whandler, "pwd", ybpasswd);

            putPara(whandler, "aac001", ybbh);
            putPara(whandler, "akc190", ybjzlsh);
            WriteLog("入参|" + ybuserid + "|" + ybpasswd + "|" + ybbh + "|" + ybjzlsh);

            bfalg = process(whandler, "F04.06.04.01");
            if (bfalg)
            {
                List<string> liSQL = new List<string>();
                getErrMsg(whandler, sMsg);
                sMsg1 = Encoding.Default.GetString(sMsg, 0, sMsg.Length).Replace("\0", "");
                if (sMsg1.ToUpper().Trim().Equals("OK"))
                {
                    //出参
                    string cxbz = string.Empty;
                    string bcxlsh = string.Empty;
                    getParaByName(whandler, "skc033", outParam);
                    cxbz = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "skc034", outParam);
                    bcxlsh = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");

                    strSql = string.Format(@"insert into ybmzzydjdr( jzlsh,jylsh,ybjzlsh,grbh,kh,ickzt,wdbh,yydj,yllb,fzxbm,fzxmc,
                                            xzqhdm,xzqhmc,dwbh,dwmc,xm,xb,nl,bckbcs,zydjh,ksbh,
                                            ksmc,ghf,cxbz1,bcxlsh,jbr,ghdjsj,jsrq,jsr,zhye,jznd,
                                            yldyxz,xbmc,ickztmc,yydjmc,jbjgmc,wdmc,bzbm,bzmc,mmbzbm1,mmbzmc1,
                                            mmbzbm2,mmbzmc2,cyzdjbbm1,cyzdjbmc1,cyzdjbbm2,cyzdjbmc2,cyzdjbbm3,cyzdjbmc3,cyrq,zbqmc_ry,
                                            cbqmc_ry,sbqmc_ry,zbqmc_cy,cbqmc_cy,sbqmc_cy,cxbz,sysdate)
                                            select jzlsh,jylsh,ybjzlsh,grbh,kh,ickzt,wdbh,yydj,yllb,fzxbm,fzxmc,
                                            xzqhdm,xzqhmc,dwbh,dwmc,xm,xb,nl,bckbcs,zydjh,ksbh,
                                            ksmc,ghf,'{1}','{2}',jbr,ghdjsj,jsrq,jsr,zhye,jznd,
                                            yldyxz,xbmc,ickztmc,yydjmc,jbjgmc,wdmc,bzbm,bzmc,mmbzbm1,mmbzmc1,
                                            mmbzbm2,mmbzmc2,cyzdjbbm1,cyzdjbmc1,cyzdjbbm2,cyzdjbmc2,cyzdjbbm3,cyzdjbmc3,cyrq,zbqmc_ry,
                                            cbqmc_ry,sbqmc_ry,zbqmc_cy,cbqmc_cy,sbqmc_cy,0,'{3}' from ybmzzydjdr where jzlsh='{0}' and cxbz=1"
                                            , jzlsh, cxbz, bcxlsh, sysdate);
                    liSQL.Add(strSql);
                    strSql = string.Format(@"update ybmzzydjdr set cxbz=2 where jzlsh='{0}' and cxbz=1", jzlsh);
                    liSQL.Add(strSql);

                    strSql = string.Format(@"update zy01h set z1lynm='{0}',z1lyjg='{1}',z1ybno='' where z1zyno='{2}'", lynm, lyjg, jzlsh);
                    liSQL.Add(strSql);

                    object[] obj = liSQL.ToArray();
                    obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);
                    if (obj[1].ToString().Equals("1"))
                    {
                        WriteLog(sysdate + "  住院登记撤销(医疗保险)成功|");
                        return new object[] { 0, 1, "住院登记撤销(医疗保险)成功|" };
                    }
                    else
                    {
                        WriteLog(sysdate + "  住院登记撤销(医疗保险)失败|操作数据失败|" + obj[2].ToString());
                        return new object[] { 0, 0, "住院登记撤销(医疗保险)失败|操作数据失败|" + obj[2].ToString() };
                    }
                }
                else
                {
                    WriteLog(sysdate + "  获取住院登记撤销(医疗保险)返回信息失败|" + sMsg1);
                    return new object[] { 0, 0, "获取住院登记撤销(医疗保险)返回信息失败|" + sMsg1 };
                }
            }
            else
            {
                getFailReason(whandler, sMsg);
                sMsg1 = Encoding.Default.GetString(sMsg, 0, sMsg.Length).Replace("\0", "");
                WriteLog(sysdate + "  住院登记撤销(医疗保险)失败|" + sMsg1);
                return new object[] { 0, 0, "住院登记撤销(医疗保险)失败|" + sMsg1 };
            }
        }
        #endregion

        #region 住院登记撤销_生育保险
        public static object[] YBZYDJCX_SYBX(object[] objParam)
        {
            string sysdate = GetServerDateTime();//当前时间
            string czygh = CliUtils.fLoginUser;  // 操作员工号
            string ywzqh = "";  // 业务周期号
            string jzlsh = objParam[0].ToString();  // 就诊流水号 *//对应玉山 住院号
            string jbr = CliUtils.fUserName;    // 经办人姓名
            string yllb = "";   // 医疗类别代码
            //交易流水号
            if (string.IsNullOrEmpty(jzlsh))
                return new object[] { 0, 0, "就诊流水号不能为空" };
            string strSql = string.Format("select * from ybcfmxscindr a where a.jzlsh = '{0}' and a.cxbz = 1", jzlsh);
            DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);

            if (ds != null && ds.Tables[0].Rows.Count > 0)
                return new object[] { 0, 0, "已上传费用明细不能冲销入院登记" };

            strSql = string.Format("select * from ybmzzydjdr a where a.jzlsh = '{0}' and a.cxbz = 1", jzlsh);
            ds.Tables.Clear();
            ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            if (ds == null || ds.Tables[0].Rows.Count == 0)
                return new object[] { 0, 0, "未办理入院登记" };

            DataRow dr = ds.Tables[0].Rows[0];
            string ybjzlsh = dr["ybjzlsh"].ToString();   // 医保就诊流水号
            string ybbh = dr["grbh"].ToString();

            bool bfalg = false;
            byte[] sMsg = new byte[1024];
            byte[] outParam = new byte[1024];
            string sMsg1 = string.Empty;

            strSql = string.Format(@"select b6lyno,b6lynm from bz06h  where left(b6lyjg,1)=2 ");
            ds.Tables.Clear();
            ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            if (ds.Tables[0].Rows.Count == 0)
            {
                return new object[] { 0, 0, "获取bz06h缴费类型出错" };
            }
            string lyjg = ds.Tables[0].Rows[0]["b6lyno"].ToString();
            string lynm = ds.Tables[0].Rows[0]["b6lynm"].ToString();

            WriteLog(sysdate + "  进入住院登记撤销(生育保险)...");

            //入参
            putPara(whandler, "usr", ybuserid);
            putPara(whandler, "pwd", ybpasswd);

            putPara(whandler, "aac001", ybbh);
            putPara(whandler, "akc190", ybjzlsh);
            putPara(whandler, "skc113", "S");
            WriteLog("入参|" + ybuserid + "|" + ybpasswd + "|" + ybbh + "|" + ybjzlsh + "|S");

            bfalg = process(whandler, "F04.06.04.01");
            if (bfalg)
            {
                List<string> liSQL = new List<string>();
                getErrMsg(whandler, sMsg);
                sMsg1 = Encoding.Default.GetString(sMsg, 0, sMsg.Length).Replace("\0", "");
                if (sMsg1.ToUpper().Trim().Equals("OK"))
                {
                    //出参
                    string cxbz = string.Empty;
                    string bcxlsh = string.Empty;
                    getParaByName(whandler, "skc033", outParam);
                    cxbz = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "skc034", outParam);
                    bcxlsh = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");

                    strSql = string.Format(@"insert into ybmzzydjdr( jzlsh,jylsh,ybjzlsh,grbh,kh,ickzt,wdbh,yydj,yllb,fzxbm,fzxmc,
                                            xzqhdm,xzqhmc,dwbh,dwmc,xm,xb,nl,bckbcs,zydjh,ksbh,
                                            ksmc,ghf,cxbz1,bcxlsh,jbr,ghdjsj,jsrq,jsr,zhye,jznd,
                                            yldyxz,xbmc,ickztmc,yydjmc,jbjgmc,wdmc,bzbm,bzmc,mmbzbm1,mmbzmc1,
                                            mmbzbm2,mmbzmc2,cyzdjbbm1,cyzdjbmc1,cyzdjbbm2,cyzdjbmc2,cyzdjbbm3,cyzdjbmc3,cyrq,zbqmc_ry,
                                            cbqmc_ry,sbqmc_ry,zbqmc_cy,cbqmc_cy,sbqmc_cy,cxbz,sysdate)
                                            select jzlsh,jylsh,ybjzlsh,grbh,kh,ickzt,wdbh,yydj,yllb,fzxbm,fzxmc,
                                            xzqhdm,xzqhmc,dwbh,dwmc,xm,xb,nl,bckbcs,zydjh,ksbh,
                                            ksmc,ghf,'{1}','{2}',jbr,ghdjsj,jsrq,jsr,zhye,jznd,
                                            yldyxz,xbmc,ickztmc,yydjmc,jbjgmc,wdmc,bzbm,bzmc,mmbzbm1,mmbzmc1,
                                            mmbzbm2,mmbzmc2,cyzdjbbm1,cyzdjbmc1,cyzdjbbm2,cyzdjbmc2,cyzdjbbm3,cyzdjbmc3,cyrq,zbqmc_ry,
                                            cbqmc_ry,sbqmc_ry,zbqmc_cy,cbqmc_cy,sbqmc_cy,0,'{3}' from ybmzzydjdr where jzlsh='{0}' and cxbz=1"
                                            , jzlsh, cxbz, bcxlsh, sysdate);
                    liSQL.Add(strSql);
                    strSql = string.Format(@"update ybmzzydjdr set cxbz=2 where jzlsh='{0}' and cxbz=1", jzlsh);
                    liSQL.Add(strSql);

                    strSql = string.Format(@"update zy01h set z1lynm='{0}',z1lyjg='{1}',z1ybno='' where z1zyno='{2}'", lynm, lyjg, jzlsh);
                    liSQL.Add(strSql);

                    object[] obj = liSQL.ToArray();
                    obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);
                    if (obj[1].ToString().Equals("1"))
                    {
                        WriteLog(sysdate + "  住院登记撤销(生育保险)成功|");
                        return new object[] { 0, 1, "住院登记撤销(生育保险)成功|" };
                    }
                    else
                    {
                        WriteLog(sysdate + "  住院登记撤销(生育保险)失败|操作数据失败|" + obj[2].ToString());
                        return new object[] { 0, 0, "住院登记撤销(生育保险)失败|操作数据失败|" + obj[2].ToString() };
                    }
                }
                else
                {
                    WriteLog(sysdate + "  获取住院登记撤销(生育保险)返回信息失败|" + sMsg1);
                    return new object[] { 0, 0, "获取住院登记撤销(生育保险)返回信息失败|" + sMsg1 };
                }
            }
            else
            {
                getFailReason(whandler, sMsg);
                sMsg1 = Encoding.Default.GetString(sMsg, 0, sMsg.Length).Replace("\0", "");
                WriteLog(sysdate + "  住院登记撤销(生育保险)失败|" + sMsg1);
                return new object[] { 0, 0, "住院登记撤销(生育保险)失败|" + sMsg1 };
            }
        }
        #endregion

        #region 住院登记撤销_工伤保险
        public static object[] YBZYDJCX_GSBX(object[] objParam)
        {
            string sysdate = GetServerDateTime();//当前时间
            string czygh = CliUtils.fLoginUser;  // 操作员工号
            string ywzqh = "";  // 业务周期号
            string jzlsh = objParam[0].ToString();  // 就诊流水号 *//对应玉山 住院号
            string jbr = CliUtils.fUserName;    // 经办人姓名
            string yllb = "";   // 医疗类别代码
            //交易流水号
            if (string.IsNullOrEmpty(jzlsh))
                return new object[] { 0, 0, "就诊流水号不能为空" };
            string strSql = string.Format("select * from ybcfmxscindr a where a.jzlsh = '{0}' and a.cxbz = 1", jzlsh);
            DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);

            if (ds != null && ds.Tables[0].Rows.Count > 0)
                return new object[] { 0, 0, "已上传费用明细不能冲销入院登记" };

            strSql = string.Format("select * from ybmzzydjdr a where a.jzlsh = '{0}' and a.cxbz = 1", jzlsh);
            ds.Tables.Clear();
            ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            if (ds == null || ds.Tables[0].Rows.Count == 0)
                return new object[] { 0, 0, "未办理入院登记" };

            DataRow dr = ds.Tables[0].Rows[0];
            string ybjzlsh = dr["ybjzlsh"].ToString();   // 医保就诊流水号
            string ybbh = dr["grbh"].ToString();

            bool bfalg = false;
            byte[] sMsg = new byte[1024];
            byte[] outParam = new byte[1024];
            string sMsg1 = string.Empty;

            strSql = string.Format(@"select b6lyno,b6lynm from bz06h  where left(b6lyjg,1)=2 ");
            ds.Tables.Clear();
            ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            if (ds.Tables[0].Rows.Count == 0)
            {
                return new object[] { 0, 0, "获取bz06h缴费类型出错" };
            }
            string lyjg = ds.Tables[0].Rows[0]["b6lyno"].ToString();
            string lynm = ds.Tables[0].Rows[0]["b6lynm"].ToString();

            WriteLog(sysdate + "  进入住院登记撤销(工伤保险)...");

            //入参
            putPara(whandler, "usr", ybuserid);
            putPara(whandler, "pwd", ybpasswd);

            putPara(whandler, "aac001", ybbh);
            putPara(whandler, "akc190", ybjzlsh);
            WriteLog("入参|" + ybuserid + "|" + ybpasswd + "|" + ybbh + "|" + ybjzlsh );

            bfalg = process(whandler, "F07.11.01.02");
            if (bfalg)
            {
                List<string> liSQL = new List<string>();
                getErrMsg(whandler, sMsg);
                sMsg1 = Encoding.Default.GetString(sMsg, 0, sMsg.Length).Replace("\0", "");
                if (sMsg1.ToUpper().Trim().Equals("OK"))
                {
                    //出参
                    string cxbz = string.Empty;
                    string bcxlsh = string.Empty;
                    getParaByName(whandler, "skc033", outParam);
                    cxbz = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "skc034", outParam);
                    bcxlsh = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");

                    strSql = string.Format(@"insert into ybmzzydjdr( jzlsh,jylsh,ybjzlsh,grbh,kh,ickzt,wdbh,yydj,yllb,fzxbm,fzxmc,
                                            xzqhdm,xzqhmc,dwbh,dwmc,xm,xb,nl,bckbcs,zydjh,ksbh,
                                            ksmc,ghf,cxbz1,bcxlsh,jbr,ghdjsj,jsrq,jsr,zhye,jznd,
                                            yldyxz,xbmc,ickztmc,yydjmc,jbjgmc,wdmc,bzbm,bzmc,mmbzbm1,mmbzmc1,
                                            mmbzbm2,mmbzmc2,cyzdjbbm1,cyzdjbmc1,cyzdjbbm2,cyzdjbmc2,cyzdjbbm3,cyzdjbmc3,cyrq,zbqmc_ry,
                                            cbqmc_ry,sbqmc_ry,zbqmc_cy,cbqmc_cy,sbqmc_cy,cxbz,sysdate)
                                            select jzlsh,jylsh,ybjzlsh,grbh,kh,ickzt,wdbh,yydj,yllb,fzxbm,fzxmc,
                                            xzqhdm,xzqhmc,dwbh,dwmc,xm,xb,nl,bckbcs,zydjh,ksbh,
                                            ksmc,ghf,'{1}','{2}',jbr,ghdjsj,jsrq,jsr,zhye,jznd,
                                            yldyxz,xbmc,ickztmc,yydjmc,jbjgmc,wdmc,bzbm,bzmc,mmbzbm1,mmbzmc1,
                                            mmbzbm2,mmbzmc2,cyzdjbbm1,cyzdjbmc1,cyzdjbbm2,cyzdjbmc2,cyzdjbbm3,cyzdjbmc3,cyrq,zbqmc_ry,
                                            cbqmc_ry,sbqmc_ry,zbqmc_cy,cbqmc_cy,sbqmc_cy,0,'{3}' from ybmzzydjdr where jzlsh='{0}' and cxbz=1"
                                            , jzlsh, cxbz, bcxlsh, sysdate);
                    liSQL.Add(strSql);
                    strSql = string.Format(@"update ybmzzydjdr set cxbz=2 where jzlsh='{0}' and cxbz=1", jzlsh);
                    liSQL.Add(strSql);

                    strSql = string.Format(@"update zy01h set z1lynm='{0}',z1lyjg='{1}',z1ybno='' where z1zyno='{2}'", lynm, lyjg, jzlsh);
                    liSQL.Add(strSql);

                    object[] obj = liSQL.ToArray();
                    obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);
                    if (obj[1].ToString().Equals("1"))
                    {
                        WriteLog(sysdate + "  住院登记撤销(工伤保险)成功|");
                        return new object[] { 0, 1, "住院登记撤销(工伤保险)成功|" };
                    }
                    else
                    {
                        WriteLog(sysdate + "  住院登记撤销(工伤保险)失败|操作数据失败|" + obj[2].ToString());
                        return new object[] { 0, 0, "住院登记撤销(工伤保险)失败|操作数据失败|" + obj[2].ToString() };
                    }
                }
                else
                {
                    WriteLog(sysdate + "  获取住院登记撤销(工伤保险)返回信息失败|" + sMsg1);
                    return new object[] { 0, 0, "获取住院登记撤销(工伤保险)返回信息失败|" + sMsg1 };
                }
            }
            else
            {
                getFailReason(whandler, sMsg);
                sMsg1 = Encoding.Default.GetString(sMsg, 0, sMsg.Length).Replace("\0", "");
                WriteLog(sysdate + "  住院登记撤销(工伤保险)失败|" + sMsg1);
                return new object[] { 0, 0, "住院登记撤销(工伤保险)失败|" + sMsg1 };
            }
        }
        #endregion


        #region 住院处方明细上报
        public static object[] YBZYSFDJ(object[] objParam)
        {
            string jzlsh = objParam[0].ToString();  // 就诊流水号 *//对应玉山 住院号
            if (string.IsNullOrEmpty(jzlsh))
                return new object[] { 0, 0, "就诊流水号不能为空" };
            string strSql = string.Format(@"select xzbm from ybmzzydjdr where jzlsh='{0}' and cxbz=1", objParam[0].ToString());
            DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            if (ds == null || ds.Tables[0].Rows.Count == 0)
                return new object[] { 0, 0, "未做医保住院登记信息" };
            string sxzbm = ds.Tables[0].Rows[0]["xzbm"].ToString();
            object[] objReturn = null;
            switch (sxzbm)
            {
                case "310":
                    objReturn = YBZYSFDJ_YLBX(objParam);
                    break;
                case "410":
                    objReturn = YBZYSFDJ_GSBX(objParam);
                    break;
                case "510":
                    objReturn = YBZYSFDJ_SYBX(objParam);
                    break;
                default:
                    objReturn = new object[] { 0, 0, "未找到方法体" };
                    break;
            }
            return objReturn;
        }
        #endregion

        #region 住院处方明细上报_医疗保险
        public static object[] YBZYSFDJ_YLBX(object[] objParam)
        {
            string sysdate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");//当前时间
            string czygh = CliUtils.fLoginUser;  //操作员工号
            string ywzqh = "";  //业务周期号
            string jzlsh = objParam[0].ToString();  //就诊流水号
            string jbr = CliUtils.fUserName;    //经办人姓名
            string cfsj = DateTime.Now.ToString("yyyyMMddHHmmss");
            string jylsh = cfsj + "-102002-" + new Random().Next(100).ToString().PadLeft(4, '0');

            bool bfalg = false;
            byte[] sMsg = new byte[1024];
            byte[] outParam = new byte[1024];
            string sMsg1 = string.Empty;

            if (string.IsNullOrEmpty(jzlsh))
                return new object[] { 0, 0, "就诊流水号不能为空" };

            string strSql = string.Format("select * from ybmzzydjdr a where a.jzlsh = '{0}' and a.cxbz = 1", jzlsh);
            DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            if (ds == null || ds.Tables[0].Rows.Count == 0)
                return new object[] { 0, 0, "该患者未办理医保登记" };

            DataRow dr1 = ds.Tables[0].Rows[0];
            string ybjzlsh = dr1["ybjzlsh"].ToString(); //住院号（在医保中心系统编号）
            string ybbh = dr1["grbh"].ToString();       //  医保编号
            string bzbm1 = dr1["bzbm"].ToString();      //病情编码
            string bzbm2 = dr1["mmbzbm1"].ToString();   //病情编码(2)
            string bzbm3 = dr1["mmbzbm2"].ToString();   //病情编码(3)
            string xm = dr1["xm"].ToString();
            string kh = dr1["kh"].ToString();

            #region 处方明细
            StringBuilder rc = new StringBuilder();

            strSql = string.Format(@"select 
                                    b.ybxmbh,b.ybxmmc,a.z3djxx as dj,sum(case LEFT(a.z3endv,1) when '4' then -a.z3jzcs else 1*a.z3jzcs end) as sl,
                                    sum(case LEFT(a.z3endv,1) when '4' then -1*a.z3jzje else 1*a.z3jzje end) as je,a.z3item as yyxmbh, a.z3name as yyxmmc,
                                    a.z3empn as ysdm, a.z3kdys as ysxm,max(a.z3date) as yysj,
                                    a.z3ksno as ksno, a.z3zxks as zxks, z3sfno as sfno, 
                                    b.sfxmzldm as ybsfxmzldm, b.sflbdm as ybsflbdm,b.sfxmdjdm as xmlx ,b.yfyb
                                    from zy03d a 
                                    left join ybhisdzdr b on 
                                    a.z3item=b.hisxmbh 
                                    where a.z3ybup is null and LEFT(a.z3kind,1) in(2,4) and a.z3zyno='{0}'
                                    group by b.ybxmbh,b.ybxmmc,a.z3djxx,a.z3item,a.z3name, a.z3empn,a.z3kdys,a.z3ksno,a.z3zxks,a.z3sfno,
                                    b.sfxmzldm,b.sflbdm,b.sfxmdjdm,b.yfyb
                                    having sum(case LEFT(a.z3endv,1) when '4' then -1*a.z3jzcs else 1*a.z3jzcs end)!=0", jzlsh);

            ds.Tables.Clear();
            ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);

            List<string> li_cfxm = new List<string>();

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
                        string ybxmbh = dr["ybxmbh"].ToString();         // 医保项目编号 
                        string yyxmmc = dr["yyxmmc"].ToString();         // 医院收费项目 
                        string ybsflbdm = dr["ybsflbdm"].ToString();     // 收费类别代码
                        string sfyb = dr["yfyb"].ToString();             //是否医保项目（000否，001 是）
                        string ysxm = dr["ysxm"].ToString();             // 医生姓名
                        string jx = ""; //对应玉山 
                        string gg = ""; //对应玉山
                        decimal dj = Convert.ToDecimal(dr["dj"]);        // 单价  //*
                        decimal sl = Convert.ToDecimal(dr["sl"]);        // 数量  //*
                        decimal je = Convert.ToDecimal(dr["je"]);        // 金额  //*
                        decimal mcyl = 1;
                        string sypc = "";   //使用频次
                        string fy = "";     //用法
                        string zxts = "1";    //执行天数
                        string ybsfxmzldm = dr["ybsfxmzldm"].ToString(); // 收费项目种类代码 
                        string yyxmbh = dr["yyxmbh"].ToString();         // 医院收费项目编号
                        string ybxmmc = dr["ybxmmc"].ToString();         // 医保项目名称
                        string ysbm = dr["ysdm"].ToString();             // 医生代码
                        string ksdm = dr["ksno"].ToString();             // 科室代码
                        string ksmc = dr["zxks"].ToString();             // 科室名称
                        string ypjldw = "-";                             // 药品剂量单位
                        string yysj = Convert.ToDateTime(dr["yysj"].ToString()).ToString("yyyy-MM-dd"); //用药时间 //*
                        string xmlx = dr["xmlx"].ToString();     //对应玉山项目类型(新增)
                        string clfs = ""; //对应玉山
                        //string sflb = dr["ybsflbdm"].ToString();

                        string ybcfh = cfsj + k.ToString();
                        //liybcfh.Add(ybcfh);
                        //liyyxmdm.Add(yyxmbh);
                        //liyyxmmc.Add(yyxmmc);

                        if (dr["sfno"].ToString() == "01" || dr["sfno"].ToString() == "02" || dr["sfno"].ToString() == "03")
                        {
                            ypjldw = "粒";
                        }

                        //string sTmp = grbh + "|" + "0" + "|" + ybxmbh + "|" + yyxmmc + "|" + jx + "|" + gg + "|" + xmlx + "|" + sl + "|" + dj + "|" + je + "|"
                        //                + "|" + clfs + "|" + "|" + yysj + "|" + yyxmbh + "|" + ybxmmc + "|" + ybcfh + "|" + sflb;
                        string sTmp = ybxmbh + "|" + yyxmmc + "|" + ybsflbdm + "|" + sfyb + "|" + ysxm + "|" + jx + "|" + gg + "|" + dj + "|" + sl + "|" + je + "|" +
                                        mcyl + "|" + sypc + "|" + fy + "|" + zxts + "|" + yyxmbh + "|" + ybxmmc + "|" + clfs + "|" + ksdm + "|" + ksmc;
                        li_cfxm.Add(sTmp);
                    }
                }

                if (wdzxms.Length > 0)
                    return new object[] { 0, 0, wdzxms.ToString() };
            }
            else
                return new object[] { 0, 0, "无费用明细" };
            #endregion

            #region 获取批次号
            strSql = string.Format("select * from yb_pcxx where bz=1 and jzbz='z'");
            ds.Tables.Clear();
            ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            string pch = (ds.Tables[0].Rows.Count + 1).ToString();
            string stscpch = "ZY" + pch.PadLeft(8, '0');
            #endregion

            List<string> liSQL = new List<string>();
            WriteLog(sysdate + "  进行住院费用上传(医疗保险)...");
            //入参
            putPara(whandler, "usr", ybuserid);
            putPara(whandler, "pwd", ybpasswd);

            putPara(whandler, "aac001", ybbh);
            putPara(whandler, "akc190", ybjzlsh);
            putPara(whandler, "skc600", stscpch);
            putPara(whandler, "aka120", bzbm1);
            putPara(whandler, "skc057", bzbm2);
            putPara(whandler, "skc058", bzbm3);
            WriteLog("入参|" + ybbh + "|" + ybjzlsh + "|" + stscpch + "|" + bzbm1 + "|" + bzbm2 + "|" + bzbm3);

            #region 上传明细

            string ybsfxmbh = string.Empty; //收费项目编码  <非空>（医保收费目录编码）  
            string yysfxmmc = string.Empty; //收费项目名称（医院内部收费项目或者药品名称）
            string ybsflb = string.Empty;   //收费类别
            string sfybxm = string.Empty;   //是否医保项目 (000否，001是)
            string ysxm1 = string.Empty;    //医生姓名
            string jxdw = string.Empty;       //剂型（单位）
            string gg1 = string.Empty;      //规格
            string dj1 = string.Empty;       //单价  <非空>
            string sl1 = string.Empty;      //数量<非空>
            string je1 = string.Empty;      //金额<非空>
            string mcyl1 = string.Empty;    //每次用量
            string sypc1 = string.Empty;     //使用频次
            string fy1 = string.Empty;      //用法
            string zxts1 = string.Empty;    //执行天数
            string yysfxmbh = string.Empty; //医院项目编码
            string ybsfxmmc = string.Empty; //医保项目名称
            string cfh1 = string.Empty;     //处方号
            string ysdm1 = string.Empty;   //医生代码
            string ksno1 = string.Empty; //科室代码

            bfalg = startResultSetName(whandler, "list01");

            foreach (string ss in li_cfxm)
            {
                string[] s = ss.Split('|');
                ybsfxmbh = s[0];
                yysfxmmc = s[1];
                ybsflb = s[2];
                sfybxm = s[3];
                ysxm1 = s[4];
                jxdw = s[5];
                gg1 = s[6];
                dj1 = s[7];
                sl1 = s[8];
                je1 = s[9];
                mcyl1 = s[10];
                sypc1 = s[11];
                fy1 = s[12];
                zxts1 = s[13];
                yysfxmbh = s[14];
                ybsfxmmc = s[15];
                cfh1 = s[16];
                ysdm1 = s[17];
                ksno1 = s[18];


                WriteLog("上传费用明细-->" + s);
                bfalg = putColData(whandler, "aka060", ybsfxmbh);
                putColData(whandler, "aka061", yysfxmmc);
                putColData(whandler, "aka063", ybsflb);
                putColData(whandler, "ska003", sfybxm);
                putColData(whandler, "skc049", ysxm1);
                putColData(whandler, "aka070", jxdw);
                putColData(whandler, "aka074", gg1);
                putColData(whandler, "aka068", dj1);
                putColData(whandler, "akc226", sl1);
                putColData(whandler, "akc227", je1);
                putColData(whandler, "aka071", mcyl1);
                putColData(whandler, "aka072", sypc1);
                putColData(whandler, "aka073", fy1);
                putColData(whandler, "akc229", zxts1);
                endcurRow(whandler);
                strSql = string.Format(@"insert into ybcfmxscindr( jzlsh,jylsh,ybcfh,yysfxmbm,yysfxmmc,sfxmzxbm,ybxmmc,dj,sl,je,
                                        mcyl,sypc,ysxm,ysbm,ksbh,yf,jbr,xm,kh,ybjzlsh,sysdate,scpch) values(
                                        '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}',
                                        '{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}','{20}','{21}')",
                                        jzlsh, jylsh, cfh1, yysfxmbh, yysfxmmc, ybsfxmbh, ybsfxmmc, dj1, sl1, je1,
                                        mcyl1, sypc1, ysxm1, ysdm1, ksno1, fy1, jbr, xm, kh, ybjzlsh, sysdate, stscpch);
                liSQL.Add(strSql);
            }
            endCurResultSet(whandler);
            #endregion

            bfalg = process(whandler, "F04.06.09.01");
            if (bfalg)
            {
                getErrMsg(whandler, sMsg);
                sMsg1 = Encoding.Default.GetString(sMsg, 0, sMsg.Length).Replace("\0", "");
                if (sMsg1.ToUpper().Equals("OK"))
                {
                    #region 出参
                    string sfalg = string.Empty; //操作成功标志（0-未成功 1-已成功）
                    getParaByName(whandler, "flag", outParam);
                    sfalg = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    #endregion
                    if (sfalg.Equals("1"))
                    {
                        //修改zy03d上传标识
                        strSql = string.Format(@"update zy03d set z3ybup = '{0}' where z3ybup is null and z3zyno = '{1}' ", jylsh, jzlsh);
                        liSQL.Add(strSql);
                        object[] obj = liSQL.ToArray();
                        obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);
                        if (obj[1].ToString().Equals("1"))
                        {
                            WriteLog(sysdate + "  住院费用明细上传(医疗保险)成功|");
                            return new object[] { 0, 1, "住院费用明细上传(医疗保险)成功|" };
                        }
                        else
                        {
                            WriteLog(sysdate + "  住院费用明细上传(医疗保险)失败|数据操作失败|" + obj[2].ToString());
                            return new object[] { 0, 0, "住院费用明细上传(医疗保险)失败|数据操作失败|" + obj[2].ToString() };
                        }
                    }
                    else
                    {
                        WriteLog(sysdate + "  住院费用上传(医疗保险)失败｜" + sfalg);
                        return new object[] { 0, 0, "住院费用上传(医疗保险)失败｜" + sfalg };
                    }
                }
                else
                {
                    WriteLog(sysdate + "  获取住院费用明细上报(医疗保险)返回信息失败|" + sMsg1);
                    return new object[] { 0, 0, "获取住院费用明细上报(医疗保险)返回信息失败|" + sMsg1 };
                }
            }
            else
            {
                sMsg = new byte[1024];
                getFailReason(whandler, sMsg);
                sMsg1 = Encoding.Default.GetString(sMsg, 0, sMsg.Length).Replace("\0", "");
                WriteLog(sysdate + "  住院费用明细上报上传(医疗保险)失败|" + sMsg1);
                return new object[] { 0, 0, "住院费用明细上报上传(医疗保险)失败|" + sMsg1 };
            }
        }
        #endregion

        #region 住院处方明细上报_生育保险
        public static object[] YBZYSFDJ_SYBX(object[] objParam)
        {
            string sysdate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");//当前时间
            string czygh = CliUtils.fLoginUser;  //操作员工号
            string ywzqh = "";  //业务周期号
            string jzlsh = objParam[0].ToString();  //就诊流水号
            string jbr = CliUtils.fUserName;    //经办人姓名
            string cfsj = DateTime.Now.ToString("yyyyMMddHHmmss");
            string jylsh = cfsj + "-102002-" + new Random().Next(100).ToString().PadLeft(4, '0');

            bool bfalg = false;
            byte[] sMsg = new byte[1024];
            byte[] outParam = new byte[1024];
            string sMsg1 = string.Empty;

            if (string.IsNullOrEmpty(jzlsh))
                return new object[] { 0, 0, "就诊流水号不能为空" };

            string strSql = string.Format("select * from ybmzzydjdr a where a.jzlsh = '{0}' and a.cxbz = 1", jzlsh);
            DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            if (ds == null || ds.Tables[0].Rows.Count == 0)
                return new object[] { 0, 0, "该患者未办理医保登记" };

            DataRow dr1 = ds.Tables[0].Rows[0];
            string ybjzlsh = dr1["ybjzlsh"].ToString(); //住院号（在医保中心系统编号）
            string ybbh = dr1["grbh"].ToString();       //  医保编号
            string bzbm1 = dr1["bzbm"].ToString();      //病情编码
            string bzbm2 = dr1["mmbzbm1"].ToString();   //病情编码(2)
            string bzbm3 = dr1["mmbzbm2"].ToString();   //病情编码(3)
            string xm = dr1["xm"].ToString();
            string kh = dr1["kh"].ToString();

            #region 处方明细
            StringBuilder rc = new StringBuilder();

            strSql = string.Format(@"select 
                                    b.ybxmbh,b.ybxmmc,a.z3djxx as dj,sum(case LEFT(a.z3endv,1) when '4' then -a.z3jzcs else 1*a.z3jzcs end) as sl,
                                    sum(case LEFT(a.z3endv,1) when '4' then -1*a.z3jzje else 1*a.z3jzje end) as je,a.z3item as yyxmbh, a.z3name as yyxmmc,
                                    a.z3empn as ysdm, a.z3kdys as ysxm,max(a.z3date) as yysj,
                                    a.z3ksno as ksno, a.z3zxks as zxks, z3sfno as sfno, 
                                    b.sfxmzldm as ybsfxmzldm, b.sflbdm as ybsflbdm,b.sfxmdjdm as xmlx ,b.yfyb
                                    from zy03d a 
                                    left join ybhisdzdr b on 
                                    a.z3item=b.hisxmbh 
                                    where a.z3ybup is null and LEFT(a.z3kind,1) in(2,4) and a.z3zyno='{0}'
                                    group by b.ybxmbh,b.ybxmmc,a.z3djxx,a.z3item,a.z3name, a.z3empn,a.z3kdys,a.z3ksno,a.z3zxks,a.z3sfno,
                                    b.sfxmzldm,b.sflbdm,b.sfxmdjdm,b.yfyb
                                    having sum(case LEFT(a.z3endv,1) when '4' then -1*a.z3jzcs else 1*a.z3jzcs end)!=0", jzlsh);

            ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);

            List<string> li_cfxm = new List<string>();

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
                        string ybxmbh = dr["ybxmbh"].ToString();         // 医保项目编号 
                        string yyxmmc = dr["yyxmmc"].ToString();         // 医院收费项目 
                        string ybsflbdm = dr["ybsflbdm"].ToString();     // 收费类别代码
                        string sfyb = dr["yfyb"].ToString();             //是否医保项目（000否，001 是）
                        string ysxm = dr["ysxm"].ToString();             // 医生姓名
                        string jx = ""; //对应玉山 
                        string gg = ""; //对应玉山
                        decimal dj = Convert.ToDecimal(dr["dj"]);        // 单价  //*
                        decimal sl = Convert.ToDecimal(dr["sl"]);        // 数量  //*
                        decimal je = Convert.ToDecimal(dr["je"]);        // 金额  //*
                        decimal mcyl = 1;
                        string sypc = "";   //使用频次
                        string fy = "";     //用法
                        string zxts = "1";    //执行天数
                        string ybsfxmzldm = dr["ybsfxmzldm"].ToString(); // 收费项目种类代码 
                        string yyxmbh = dr["yyxmbh"].ToString();         // 医院收费项目编号
                        string ybxmmc = dr["ybxmmc"].ToString();         // 医保项目名称
                        string ysbm = dr["ysdm"].ToString();             // 医生代码
                        string ksdm = dr["ksno"].ToString();             // 科室代码
                        string ksmc = dr["zxks"].ToString();             // 科室名称
                        string ypjldw = "-";                             // 药品剂量单位
                        string yysj = Convert.ToDateTime(dr["yysj"].ToString()).ToString("yyyy-MM-dd"); //用药时间 //*
                        string xmlx = dr["xmlx"].ToString();     //对应玉山项目类型(新增)
                        string clfs = ""; //对应玉山
                        //string sflb = dr["ybsflbdm"].ToString();

                        string ybcfh = cfsj + k.ToString();
                        //liybcfh.Add(ybcfh);
                        //liyyxmdm.Add(yyxmbh);
                        //liyyxmmc.Add(yyxmmc);

                        if (dr["sfno"].ToString() == "01" || dr["sfno"].ToString() == "02" || dr["sfno"].ToString() == "03")
                        {
                            ypjldw = "粒";
                        }

                        //string sTmp = grbh + "|" + "0" + "|" + ybxmbh + "|" + yyxmmc + "|" + jx + "|" + gg + "|" + xmlx + "|" + sl + "|" + dj + "|" + je + "|"
                        //                + "|" + clfs + "|" + "|" + yysj + "|" + yyxmbh + "|" + ybxmmc + "|" + ybcfh + "|" + sflb;
                        string sTmp = ybxmbh + "|" + yyxmmc + "|" + ybsflbdm + "|" + sfyb + "|" + ysxm + "|" + jx + "|" + gg + "|" + dj + "|" + sl + "|" + je + "|" +
                                        mcyl + "|" + sypc + "|" + fy + "|" + zxts + "|" + yyxmbh + "|" + ybxmmc + "|" + clfs + "|" + ksdm + "|" + ksmc;
                        li_cfxm.Add(sTmp);
                    }
                }

                if (wdzxms.Length > 0)
                    return new object[] { 0, 0, wdzxms.ToString() };
            }
            else
                return new object[] { 0, 0, "无费用明细" };
            #endregion

            List<string> liSQL = new List<string>();
            WriteLog(sysdate + "  进行住院费用上传(生育保险)...");
            //入参
            putPara(whandler, "usr", ybuserid);
            putPara(whandler, "pwd", ybpasswd);

            putPara(whandler, "aac001", ybbh);
            putPara(whandler, "akc190", ybjzlsh);
            putPara(whandler, "aka120", bzbm1);
            putPara(whandler, "skc057", bzbm2);
            putPara(whandler, "skc058", bzbm3);
            putPara(whandler, "skc113", "S");

            WriteLog("入参|" + ybbh + "|" + ybjzlsh + "|" + bzbm1 + "|" + bzbm2 + "|" + bzbm3 + "|S");

            #region 上传明细

            string ybsfxmbh = string.Empty; //收费项目编码  <非空>（医保收费目录编码）  
            string yysfxmmc = string.Empty; //收费项目名称（医院内部收费项目或者药品名称）
            string ybsflb = string.Empty;   //收费类别
            string sfybxm = string.Empty;   //是否医保项目 (000否，001是)
            string ysxm1 = string.Empty;    //医生姓名
            string jxdw = string.Empty;       //剂型（单位）
            string gg1 = string.Empty;      //规格
            string dj1 = string.Empty;       //单价  <非空>
            string sl1 = string.Empty;      //数量<非空>
            string je1 = string.Empty;      //金额<非空>
            string mcyl1 = string.Empty;    //每次用量
            string sypc1 = string.Empty;     //使用频次
            string fy1 = string.Empty;      //用法
            string zxts1 = string.Empty;    //执行天数
            string yysfxmbh = string.Empty; //医院项目编码
            string ybsfxmmc = string.Empty; //医保项目名称
            string cfh1 = string.Empty;     //处方号
            string ysdm1 = string.Empty;   //医生代码
            string ksno1 = string.Empty; //科室代码

            bfalg = startResultSetName(whandler, "list01");

            foreach (string ss in li_cfxm)
            {
                string[] s = ss.Split('|');
                ybsfxmbh = s[0];
                yysfxmmc = s[1];
                ybsflb = s[2];
                sfybxm = s[3];
                ysxm1 = s[4];
                jxdw = s[5];
                gg1 = s[6];
                dj1 = s[7];
                sl1 = s[8];
                je1 = s[9];
                mcyl1 = s[10];
                sypc1 = s[11];
                fy1 = s[12];
                zxts1 = s[13];
                yysfxmbh = s[14];
                ybsfxmmc = s[15];
                cfh1 = s[16];
                ysdm1 = s[17];
                ksno1 = s[18];


                WriteLog("上传费用明细-->" + s);
                bfalg = putColData(whandler, "aka060", ybsfxmbh);
                bfalg = putColData(whandler, "ska100", "");
                putColData(whandler, "aka061", yysfxmmc);
                putColData(whandler, "aka063", ybsflb);
                putColData(whandler, "ska003", sfybxm);
                putColData(whandler, "skc049", ysxm1);
                putColData(whandler, "aka070", jxdw);
                putColData(whandler, "aka074", gg1);
                putColData(whandler, "aka068", dj1);
                putColData(whandler, "akc226", sl1);
                putColData(whandler, "akc227", je1);
                putColData(whandler, "aka071", mcyl1);
                putColData(whandler, "aka072", sypc1);
                putColData(whandler, "aka073", fy1);
                putColData(whandler, "akc229", zxts1);
                endcurRow(whandler);
                strSql = string.Format(@"insert into ybcfmxscindr( jzlsh,jylsh,ybcfh,yysfxmbm,yysfxmmc,sfxmzxbm,ybxmmc,dj,sl,je,
                                        mcyl,sypc,ysxm,ysbm,ksbh,yf,jbr,xm,kh,ybjzlsh,sysdate,scpch) values(
                                        '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}',
                                        '{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}','{20}','{21}')",
                                        jzlsh, jylsh, cfh1, yysfxmbh, yysfxmmc, ybsfxmbh, ybsfxmmc, dj1, sl1, je1,
                                        mcyl1, sypc1, ysxm1, ysdm1, ksno1, fy1, jbr, xm, kh, ybjzlsh, sysdate, "");
                liSQL.Add(strSql);
            }
            endCurResultSet(whandler);
            #endregion

            bfalg = process(whandler, "F04.06.09.01");
            if (bfalg)
            {
                getErrMsg(whandler, sMsg);
                sMsg1 = Encoding.Default.GetString(sMsg, 0, sMsg.Length).Replace("\0", "");
                if (sMsg1.ToUpper().Equals("OK"))
                {
                    #region 出参
                    string sfalg = string.Empty; //操作成功标志（0-未成功 1-已成功）
                    getParaByName(whandler, "flag", outParam);
                    sfalg = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    #endregion
                    if (sfalg.Equals("1"))
                    {
                        //修改zy03d上传标识
                        strSql = string.Format(@"update zy03d set z3ybup = '{0}' where z3ybup is null and z3zyno = '{1}' ", jylsh, jzlsh);
                        liSQL.Add(strSql);
                        object[] obj = liSQL.ToArray();
                        obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);
                        if (obj[1].ToString().Equals("1"))
                        {
                            WriteLog(sysdate + "  住院费用明细上传(生育保险)成功|");
                            return new object[] { 0, 1, "住院费用明细上传(生育保险)成功|" };
                        }
                        else
                        {
                            WriteLog(sysdate + "  住院费用明细上传(生育保险)失败|数据操作失败|" + obj[2].ToString());
                            return new object[] { 0, 0, "住院费用明细上传(生育保险)失败|数据操作失败|" + obj[2].ToString() };
                        }
                    }
                    else
                    {
                        WriteLog(sysdate + "  住院费用上传(生育保险)失败｜" + sfalg);
                        return new object[] { 0, 0, "住院费用上传(生育保险)失败｜" + sfalg };
                    }
                }
                else
                {
                    WriteLog(sysdate + "  获取住院费用明细上报(生育保险)返回信息失败|" + sMsg1);
                    return new object[] { 0, 0, "获取住院费用明细上报(生育保险)返回信息失败|" + sMsg1 };
                }
            }
            else
            {
                sMsg = new byte[1024];
                getFailReason(whandler, sMsg);
                sMsg1 = Encoding.Default.GetString(sMsg, 0, sMsg.Length).Replace("\0", "");
                WriteLog(sysdate + "  住院费用明细上报上传(生育保险)失败|" + sMsg1);
                return new object[] { 0, 0, "住院费用明细上报上传(生育保险)失败|" + sMsg1 };
            }
        }
        #endregion

        #region 住院处方明细上报_工伤保险
        public static object[] YBZYSFDJ_GSBX(object[] objParam)
        {
            string sysdate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");//当前时间
            string czygh = CliUtils.fLoginUser;  //操作员工号
            string ywzqh = "";  //业务周期号
            string jzlsh = objParam[0].ToString();  //就诊流水号
            string jbr = CliUtils.fUserName;    //经办人姓名
            string cfsj = DateTime.Now.ToString("yyyyMMddHHmmss");
            string jylsh = cfsj + "-102002-" + new Random().Next(100).ToString().PadLeft(4, '0');

            bool bfalg = false;
            byte[] sMsg = new byte[1024];
            byte[] outParam = new byte[1024];
            string sMsg1 = string.Empty;

            if (string.IsNullOrEmpty(jzlsh))
                return new object[] { 0, 0, "就诊流水号不能为空" };

            string strSql = string.Format("select * from ybmzzydjdr a where a.jzlsh = '{0}' and a.cxbz = 1", jzlsh);
            DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            if (ds == null || ds.Tables[0].Rows.Count == 0)
                return new object[] { 0, 0, "该患者未办理医保登记" };

            DataRow dr1 = ds.Tables[0].Rows[0];
            string ybjzlsh = dr1["ybjzlsh"].ToString(); //住院号（在医保中心系统编号）
            string ybbh = dr1["grbh"].ToString();       //  医保编号
            string bzbm1 = dr1["bzbm"].ToString();      //病情编码
            string bzbm2 = dr1["mmbzbm1"].ToString();   //病情编码(2)
            string bzbm3 = dr1["mmbzbm2"].ToString();   //病情编码(3)
            string xm = dr1["xm"].ToString();
            string kh = dr1["kh"].ToString();

            #region 处方明细
            StringBuilder rc = new StringBuilder();

            strSql = string.Format(@"select 
                                    b.ybxmbh,b.ybxmmc,a.z3djxx as dj,sum(case LEFT(a.z3endv,1) when '4' then -a.z3jzcs else 1*a.z3jzcs end) as sl,
                                    sum(case LEFT(a.z3endv,1) when '4' then -1*a.z3jzje else 1*a.z3jzje end) as je,a.z3item as yyxmbh, a.z3name as yyxmmc,
                                    a.z3empn as ysdm, a.z3kdys as ysxm,max(a.z3date) as yysj,
                                    a.z3ksno as ksno, a.z3zxks as zxks, z3sfno as sfno, 
                                    b.sfxmzldm as ybsfxmzldm, b.sflbdm as ybsflbdm,b.sfxmdjdm as xmlx ,b.yfyb
                                    from zy03d a 
                                    left join ybhisdzdr b on 
                                    a.z3item=b.hisxmbh 
                                    where a.z3ybup is null and LEFT(a.z3kind,1) in(2,4) and a.z3zyno='{0}'
                                    group by b.ybxmbh,b.ybxmmc,a.z3djxx,a.z3item,a.z3name, a.z3empn,a.z3kdys,a.z3ksno,a.z3zxks,a.z3sfno,
                                    b.sfxmzldm,b.sflbdm,b.sfxmdjdm,b.yfyb
                                    having sum(case LEFT(a.z3endv,1) when '4' then -1*a.z3jzcs else 1*a.z3jzcs end)!=0", jzlsh);

            ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);

            List<string> li_cfxm = new List<string>();

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
                        string ybxmbh = dr["ybxmbh"].ToString();         // 医保项目编号 
                        string yyxmmc = dr["yyxmmc"].ToString();         // 医院收费项目 
                        string ybsflbdm = dr["ybsflbdm"].ToString();     // 收费类别代码
                        string sfyb = dr["yfyb"].ToString();             //是否医保项目（000否，001 是）
                        string ysxm = dr["ysxm"].ToString();             // 医生姓名
                        string jx = ""; //对应玉山 
                        string gg = ""; //对应玉山
                        decimal dj = Convert.ToDecimal(dr["dj"]);        // 单价  //*
                        decimal sl = Convert.ToDecimal(dr["sl"]);        // 数量  //*
                        decimal je = Convert.ToDecimal(dr["je"]);        // 金额  //*
                        decimal mcyl = 1;
                        string sypc = "";   //使用频次
                        string fy = "";     //用法
                        string zxts = "1";    //执行天数
                        string ybsfxmzldm = dr["ybsfxmzldm"].ToString(); // 收费项目种类代码 
                        string yyxmbh = dr["yyxmbh"].ToString();         // 医院收费项目编号
                        string ybxmmc = dr["ybxmmc"].ToString();         // 医保项目名称
                        string ysbm = dr["ysdm"].ToString();             // 医生代码
                        string ksdm = dr["ksno"].ToString();             // 科室代码
                        string ksmc = dr["zxks"].ToString();             // 科室名称
                        string ypjldw = "-";                             // 药品剂量单位
                        string yysj = Convert.ToDateTime(dr["yysj"].ToString()).ToString("yyyy-MM-dd"); //用药时间 //*
                        string xmlx = dr["xmlx"].ToString();     //对应玉山项目类型(新增)
                        string clfs = ""; //对应玉山
                        //string sflb = dr["ybsflbdm"].ToString();

                        string ybcfh = cfsj + k.ToString();
                        //liybcfh.Add(ybcfh);
                        //liyyxmdm.Add(yyxmbh);
                        //liyyxmmc.Add(yyxmmc);

                        if (dr["sfno"].ToString() == "01" || dr["sfno"].ToString() == "02" || dr["sfno"].ToString() == "03")
                        {
                            ypjldw = "粒";
                        }

                        //string sTmp = grbh + "|" + "0" + "|" + ybxmbh + "|" + yyxmmc + "|" + jx + "|" + gg + "|" + xmlx + "|" + sl + "|" + dj + "|" + je + "|"
                        //                + "|" + clfs + "|" + "|" + yysj + "|" + yyxmbh + "|" + ybxmmc + "|" + ybcfh + "|" + sflb;
                        string sTmp = ybxmbh + "|" + yyxmmc + "|" + ybsflbdm + "|" + sfyb + "|" + ysxm + "|" + jx + "|" + gg + "|" + dj + "|" + sl + "|" + je + "|" +
                                        mcyl + "|" + sypc + "|" + fy + "|" + zxts + "|" + yyxmbh + "|" + ybxmmc + "|" + clfs + "|" + ksdm + "|" + ksmc;
                        li_cfxm.Add(sTmp);
                    }
                }

                if (wdzxms.Length > 0)
                    return new object[] { 0, 0, wdzxms.ToString() };
            }
            else
                return new object[] { 0, 0, "无费用明细" };
            #endregion

            List<string> liSQL = new List<string>();
            WriteLog(sysdate + "  进行住院费用上传(工伤保险)...");
            //入参
            putPara(whandler, "usr", ybuserid);
            putPara(whandler, "pwd", ybpasswd);

            putPara(whandler, "aac001", ybbh);
            putPara(whandler, "akc190", ybjzlsh);
            //putPara(whandler, "skc600", stscpch);
            //putPara(whandler, "aka120", bzbm1);
            //putPara(whandler, "skc057", bzbm2);
            //putPara(whandler, "skc058", bzbm3);
            WriteLog("入参|" + ybbh + "|" + ybjzlsh );

            #region 上传明细

            string ybsfxmbh = string.Empty; //收费项目编码  <非空>（医保收费目录编码）  
            string yysfxmmc = string.Empty; //收费项目名称（医院内部收费项目或者药品名称）
            string ybsflb = string.Empty;   //收费类别
            string sfybxm = string.Empty;   //是否医保项目 (000否，001是)
            string ysxm1 = string.Empty;    //医生姓名
            string jxdw = string.Empty;       //剂型（单位）
            string gg1 = string.Empty;      //规格
            string dj1 = string.Empty;       //单价  <非空>
            string sl1 = string.Empty;      //数量<非空>
            string je1 = string.Empty;      //金额<非空>
            string mcyl1 = string.Empty;    //每次用量
            string sypc1 = string.Empty;     //使用频次
            string fy1 = string.Empty;      //用法
            string zxts1 = string.Empty;    //执行天数
            string yysfxmbh = string.Empty; //医院项目编码
            string ybsfxmmc = string.Empty; //医保项目名称
            string cfh1 = string.Empty;     //处方号
            string ysdm1 = string.Empty;   //医生代码
            string ksno1 = string.Empty; //科室代码

            bfalg = startResultSetName(whandler, "list01");

            foreach (string ss in li_cfxm)
            {
                string[] s = ss.Split('|');
                ybsfxmbh = s[0];
                yysfxmmc = s[1];
                ybsflb = s[2];
                sfybxm = s[3];
                ysxm1 = s[4];
                jxdw = s[5];
                gg1 = s[6];
                dj1 = s[7];
                sl1 = s[8];
                je1 = s[9];
                mcyl1 = s[10];
                sypc1 = s[11];
                fy1 = s[12];
                zxts1 = s[13];
                yysfxmbh = s[14];
                ybsfxmmc = s[15];
                cfh1 = s[16];
                ysdm1 = s[17];
                ksno1 = s[18];


                WriteLog("上传费用明细-->" + s);
                bfalg = putColData(whandler, "aka060", ybsfxmbh);
                putColData(whandler, "aka061", yysfxmmc);
                putColData(whandler, "aka063", ybsflb);
                putColData(whandler, "ska003", sfybxm);
                putColData(whandler, "sla003", "000");
                putColData(whandler, "skc049", ysxm1);
                putColData(whandler, "aka070", jxdw);
                putColData(whandler, "aka074", gg1);
                putColData(whandler, "aka068", dj1);
                putColData(whandler, "akc226", sl1);
                putColData(whandler, "akc227", je1);
                putColData(whandler, "aka071", mcyl1);
                putColData(whandler, "aka072", sypc1);
                putColData(whandler, "aka073", fy1);
                putColData(whandler, "akc229", zxts1);
                endcurRow(whandler);
                strSql = string.Format(@"insert into ybcfmxscindr( jzlsh,jylsh,ybcfh,yysfxmbm,yysfxmmc,sfxmzxbm,ybxmmc,dj,sl,je,
                                        mcyl,sypc,ysxm,ysbm,ksbh,yf,jbr,xm,kh,ybjzlsh,sysdate,scpch) values(
                                        '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}',
                                        '{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}','{20}','{21}')",
                                        jzlsh, jylsh, cfh1, yysfxmbh, yysfxmmc, ybsfxmbh, ybsfxmmc, dj1, sl1, je1,
                                        mcyl1, sypc1, ysxm1, ysdm1, ksno1, fy1, jbr, xm, kh, ybjzlsh, sysdate, "");
                liSQL.Add(strSql);
            }
            endCurResultSet(whandler);
            #endregion

            bfalg = process(whandler, "F07.11.02.01");
            if (bfalg)
            {
                getErrMsg(whandler, sMsg);
                sMsg1 = Encoding.Default.GetString(sMsg, 0, sMsg.Length).Replace("\0", "");
                if (sMsg1.ToUpper().Equals("OK"))
                {
                    #region 出参
                    string sfalg = string.Empty; //操作成功标志（0-未成功 1-已成功）
                    getParaByName(whandler, "flag", outParam);
                    sfalg = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    #endregion
                    if (sfalg.Equals("1"))
                    {
                        //修改zy03d上传标识
                        strSql = string.Format(@"update zy03d set z3ybup = '{0}' where z3ybup is null and z3zyno = '{1}' ", jylsh, jzlsh);
                        liSQL.Add(strSql);
                        object[] obj = liSQL.ToArray();
                        obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);
                        if (obj[1].ToString().Equals("1"))
                        {
                            WriteLog(sysdate + "  住院费用明细上传(工伤保险)成功|");
                            return new object[] { 0, 1, "住院费用明细上传(工伤保险)成功|" };
                        }
                        else
                        {
                            WriteLog(sysdate + "  住院费用明细上传(工伤保险)失败|数据操作失败|" + obj[2].ToString());
                            return new object[] { 0, 0, "住院费用明细上传(工伤保险)失败|数据操作失败|" + obj[2].ToString() };
                        }
                    }
                    else
                    {
                        WriteLog(sysdate + "  住院费用上传(工伤保险)失败｜" + sfalg);
                        return new object[] { 0, 0, "住院费用上传(工伤保险)失败｜" + sfalg };
                    }
                }
                else
                {
                    WriteLog(sysdate + "  获取住院费用明细上报(工伤保险)返回信息失败|" + sMsg1);
                    return new object[] { 0, 0, "获取住院费用明细上报(工伤保险)返回信息失败|" + sMsg1 };
                }
            }
            else
            {
                sMsg = new byte[1024];
                getFailReason(whandler, sMsg);
                sMsg1 = Encoding.Default.GetString(sMsg, 0, sMsg.Length).Replace("\0", "");
                WriteLog(sysdate + "  住院费用明细上报上传(工伤保险)失败|" + sMsg1);
                return new object[] { 0, 0, "住院费用明细上报上传(工伤保险)失败|" + sMsg1 };
            }
        }
        #endregion

        #region 住院处方明细撤销
        public static object[] YBZYSFDJCX(object[] objParam)
        {
            string jzlsh = objParam[0].ToString();  // 就诊流水号 *//对应玉山 住院号
            if (string.IsNullOrEmpty(jzlsh))
                return new object[] { 0, 0, "就诊流水号不能为空" };
            string strSql = string.Format(@"select xzbm from ybmzzydjdr where jzlsh='{0}' and cxbz=1", objParam[0].ToString());
            DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            if (ds == null || ds.Tables[0].Rows.Count == 0)
                return new object[] { 0, 0, "未做医保住院登记信息" };
            string sxzbm = ds.Tables[0].Rows[0]["xzbm"].ToString();
            object[] objReturn = null;
            switch (sxzbm)
            {
                case "310":
                    objReturn = YBZYSFDJCX_YLBX(objParam);
                    break;
                case "410":
                    objReturn = YBZYSFDJCX_GSBX(objParam);
                    break;
                case "510":
                    objReturn = YBZYSFDJCX_SYBX(objParam);
                    break;
                default:
                    objReturn = new object[] { 0, 0, "未找到方法体" };
                    break;
            }
            return objReturn;
        }
        #endregion

        #region 住院处方明细撤销_医疗保险
        public static object[] YBZYSFDJCX_YLBX(object[] objParam)
        {
            string sysdate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");//当前时间
            string czygh = CliUtils.fLoginUser; // 操作员工号 
            string ywzqh = ""; // 业务周期号
            string jzlsh = objParam[0].ToString(); // 就诊流水号
            string jbr = CliUtils.fUserName;   // 经办人姓名
            if (string.IsNullOrEmpty(jzlsh))
                return new object[] { 0, 0, "就诊流水号不能为空" };

            string strSql = string.Format(@"select * from ybfyjsdr a where a.jzlsh = '{0}'  and a.cxbz = 1", jzlsh);
            DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
                return new object[] { 0, 0, "已收费结算，不能冲销" };

            strSql = string.Format(@"select * from ybcfmxscindr where jzlsh='{0}' and cxbz=1", jzlsh);
            ds.Tables.Clear();
            ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            if (ds == null || ds.Tables[0].Rows.Count == 0)
                return new object[] { 0, 0, "无上传费用或已经撤销" };
            string scpch = ds.Tables[0].Rows[0]["scpch"].ToString();


            strSql = string.Format("select * from ybmzzydjdr a where a.jzlsh = '{0}' and a.cxbz = 1", jzlsh);
            ds.Tables.Clear();
            ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            if (ds == null || ds.Tables[0].Rows.Count == 0)
                return new object[] { 0, 0, "该患者未办理医保登记" };


            DataRow dr1 = ds.Tables[0].Rows[0];
            string ybjzlsh = dr1["ybjzlsh"].ToString(); //住院号（在医保中心系统编号）
            string ybbh = dr1["grbh"].ToString();       //  医保编号
            string bzbm1 = dr1["bzbm"].ToString();      //病情编码
            string bzbm2 = dr1["mmbzbm1"].ToString();   //病情编码(2)
            string bzbm3 = dr1["mmbzbm2"].ToString();   //病情编码(3)


            WriteLog(sysdate + "   " + jzlsh + "进入住院处方明细撤销(医疗保险)...");

            //先结算后撤销
            object[] objParam1 = { ybjzlsh, ybbh, bzbm1, bzbm2, bzbm3, scpch };
            object[] outParam = N_YBZYFYJS_YLBX(objParam1);
            if (outParam[1].ToString().Equals("1"))
            {

                List<string> liSQL = new List<string>();
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
                    WriteLog(sysdate + "   " + jzlsh + "进入住院处方明细撤销(医疗保险)成功|");
                    return new object[] { 0, 1, "住院处方明细撤销(医疗保险)成功！" };
                }
                else
                {
                    WriteLog(sysdate + "   " + jzlsh + "进入住院处方明细撤销(医疗保险)成功|数据操作失败|" + obj[2].ToString());
                    return new object[] { 0, 0, "住院处方明细撤销(医疗保险)|数据库操作失败!" };
                }
            }
            else
            {
                WriteLog(sysdate + "  住院费用明细上传撤销(医疗保险)失败|" + outParam[2].ToString());
                return new object[] { 0, 0, "住院费用明细上传撤销(医疗保险)失败|" + outParam[2].ToString() };
            }
        }

        #endregion

        #region 住院处方明细撤销_生育保险
        public static object[] YBZYSFDJCX_SYBX(object[] objParam)
        {
            string sysdate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");//当前时间
            string czygh = CliUtils.fLoginUser; // 操作员工号 
            string ywzqh = ""; // 业务周期号
            string jzlsh = objParam[0].ToString(); // 就诊流水号
            string jbr = CliUtils.fUserName;   // 经办人姓名
            if (string.IsNullOrEmpty(jzlsh))
                return new object[] { 0, 0, "就诊流水号不能为空" };

            string strSql = string.Format(@"select * from ybfyjsdr a where a.jzlsh = '{0}'  and a.cxbz = 1", jzlsh);
            DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
                return new object[] { 0, 0, "已收费结算，不能冲销" };

            strSql = string.Format(@"select * from ybcfmxscindr where jzlsh='{0}' and cxbz=1", jzlsh);
            ds.Tables.Clear();
            ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            if (ds == null || ds.Tables[0].Rows.Count == 0)
                return new object[] { 0, 0, "无上传费用或已经撤销" };
            string scpch = ds.Tables[0].Rows[0]["scpch"].ToString();


            strSql = string.Format("select * from ybmzzydjdr a where a.jzlsh = '{0}' and a.cxbz = 1", jzlsh);
            ds.Tables.Clear();
            ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            if (ds == null || ds.Tables[0].Rows.Count == 0)
                return new object[] { 0, 0, "该患者未办理医保登记" };


            DataRow dr1 = ds.Tables[0].Rows[0];
            string ybjzlsh = dr1["ybjzlsh"].ToString(); //住院号（在医保中心系统编号）
            string ybbh = dr1["grbh"].ToString();       //  医保编号
            string bzbm1 = dr1["bzbm"].ToString();      //病情编码
            string bzbm2 = dr1["mmbzbm1"].ToString();   //病情编码(2)
            string bzbm3 = dr1["mmbzbm2"].ToString();   //病情编码(3)


            WriteLog(sysdate + "   " + jzlsh + "进入住院处方明细撤销(生育保险)...");

            //先结算后撤销
            object[] objParam1 = { ybjzlsh, ybbh, bzbm1, bzbm2, bzbm3, scpch };
            object[] outParam = N_YBZYFYJS_SYBX(objParam1);
            if (outParam[1].ToString().Equals("1"))
            {

                List<string> liSQL = new List<string>();
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
                    WriteLog(sysdate + "   " + jzlsh + "进入住院处方明细撤销(医疗保险)成功|");
                    return new object[] { 0, 1, "住院处方明细撤销(医疗保险)成功！" };
                }
                else
                {
                    WriteLog(sysdate + "   " + jzlsh + "进入住院处方明细撤销(医疗保险)成功|数据操作失败|" + obj[2].ToString());
                    return new object[] { 0, 0, "住院处方明细撤销(医疗保险)|数据库操作失败!" };
                }
            }
            else
            {
                WriteLog(sysdate + "  住院费用明细上传撤销(医疗保险)失败|" + outParam[2].ToString());
                return new object[] { 0, 0, "住院费用明细上传撤销(医疗保险)失败|" + outParam[2].ToString() };
            }
        }

        #endregion

        #region 住院处方明细撤销_工伤保险
        public static object[] YBZYSFDJCX_GSBX(object[] objParam)
        {
            string sysdate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");//当前时间
            string czygh = CliUtils.fLoginUser; // 操作员工号 
            string ywzqh = ""; // 业务周期号
            string jzlsh = objParam[0].ToString(); // 就诊流水号
            string jbr = CliUtils.fUserName;   // 经办人姓名
            if (string.IsNullOrEmpty(jzlsh))
                return new object[] { 0, 0, "就诊流水号不能为空" };

            string strSql = string.Format(@"select * from ybfyjsdr a where a.jzlsh = '{0}'  and a.cxbz = 1", jzlsh);
            DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
                return new object[] { 0, 0, "已收费结算，不能冲销" };

            strSql = string.Format(@"select * from ybcfmxscindr where jzlsh='{0}' and cxbz=1", jzlsh);
            ds.Tables.Clear();
            ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            if (ds == null || ds.Tables[0].Rows.Count == 0)
                return new object[] { 0, 0, "无上传费用或已经撤销" };
            string scpch = ds.Tables[0].Rows[0]["scpch"].ToString();


            strSql = string.Format("select * from ybmzzydjdr a where a.jzlsh = '{0}' and a.cxbz = 1", jzlsh);
            ds.Tables.Clear();
            ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            if (ds == null || ds.Tables[0].Rows.Count == 0)
                return new object[] { 0, 0, "该患者未办理医保登记" };


            DataRow dr1 = ds.Tables[0].Rows[0];
            string ybjzlsh = dr1["ybjzlsh"].ToString(); //住院号（在医保中心系统编号）
            string ybbh = dr1["grbh"].ToString();       //  医保编号
            string bzbm1 = dr1["bzbm"].ToString();      //病情编码
            string bzbm2 = dr1["mmbzbm1"].ToString();   //病情编码(2)
            string bzbm3 = dr1["mmbzbm2"].ToString();   //病情编码(3)


            WriteLog(sysdate + "   " + jzlsh + "进入住院处方明细撤销(工伤保险)...");

            //先结算后撤销
            object[] objParam1 = { ybjzlsh, ybbh, bzbm1, bzbm2, bzbm3, scpch };
            object[] outParam = N_YBZYFYJS_GSBX(objParam1);
            if (outParam[1].ToString().Equals("1"))
            {

                List<string> liSQL = new List<string>();
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
                    WriteLog(sysdate + "   " + jzlsh + "进入住院处方明细撤销(工伤保险)成功|");
                    return new object[] { 0, 1, "住院处方明细撤销(工伤保险)成功！" };
                }
                else
                {
                    WriteLog(sysdate + "   " + jzlsh + "进入住院处方明细撤销(工伤保险)成功|数据操作失败|" + obj[2].ToString());
                    return new object[] { 0, 0, "住院处方明细撤销(工伤保险)|数据库操作失败!" };
                }
            }
            else
            {
                WriteLog(sysdate + "  住院费用明细上传撤销(工伤保险)失败|" + outParam[2].ToString());
                return new object[] { 0, 0, "住院费用明细上传撤销(工伤保险)失败|" + outParam[2].ToString() };
            }
        }

        #endregion


        #region 住院费用预结算
        public static object[] YBZYSFYJS(object[] objParam)
        {
            string jzlsh = objParam[0].ToString();  // 就诊流水号 *//对应玉山 住院号
            if (string.IsNullOrEmpty(jzlsh))
                return new object[] { 0, 0, "就诊流水号不能为空" };
            string strSql = string.Format(@"select xzbm from ybmzzydjdr where jzlsh='{0}' and cxbz=1", objParam[0].ToString());
            DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            if (ds == null || ds.Tables[0].Rows.Count == 0)
                return new object[] { 0, 0, "未做医保住院登记信息" };
            string sxzbm = ds.Tables[0].Rows[0]["xzbm"].ToString();
            object[] objReturn = null;
            switch (sxzbm)
            {
                case "310":
                    objReturn = YBZYSFYJS_YLBX(objParam);
                    break;
                case "410":
                    objReturn = YBZYSFYJS_GSBX(objParam);
                    break;
                case "510":
                    objReturn = YBZYSFYJS_SYBX(objParam);
                    break;
                default:
                    objReturn = new object[] { 0, 0, "未找到方法体" };
                    break;
            }
            return objReturn;
        }
        #endregion

        #region 住院费用预结算_医疗保险
        public static object[] YBZYSFYJS_YLBX(object[] objParam)
        {
            string sysdate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");//当前时间
            string czygh = CliUtils.fLoginUser;   // 操作员工号
            string ywzqh = "";   // 业务周期号
            string jzlsh = objParam[0].ToString();   // 就诊流水号
            string cyyy = objParam[1].ToString();    // 出院原因代码（门诊传9）
            string zhsybz = objParam[2].ToString();  // 账户使用标志（0或1）
            string jbr = CliUtils.fUserName;     // 经办人姓名
            string dqrq = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");  // 当前日期
            string cyrq = objParam[4].ToString();//出院日期
            string jsdjh = "0000";    //结算单据号
            string ylfhj1 = objParam[5].ToString(); //医疗费合计 (新增)
            string ybmm = ""; //医保卡密码 (新增)

            bool bfalg = false;
            byte[] sMsg = new byte[1024];
            byte[] outParam = new byte[1024];
            string sMsg1 = string.Empty;
            string sValue = string.Empty;

            string strSql;
            DataSet ds = null;

            if (string.IsNullOrEmpty(jzlsh))
                return new object[] { 0, 0, "就诊流水号不能为空!" };

            if (string.IsNullOrEmpty(ylfhj1))
                return new object[] { 0, 0, "医疗合计费不能为空" };

            if (string.IsNullOrEmpty(zhsybz))
                return new object[] { 0, 0, "是否可用个人帐户支付自费部份需选择" };

            strSql = string.Format("select a.z1outd as cyrq from zy01d a where left(a.z1endv, 1) = '8' and a.z1zyno = '{0}'", jzlsh);
            ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            if (ds == null || ds.Tables[0].Rows.Count == 0)
                return new object[] { 0, 0, "就诊流水号" + jzlsh + "未拖出床位" };
            cyrq = Convert.ToDateTime(ds.Tables[0].Rows[0]["cyrq"]).ToString("yyyy-MM-dd"); //出院日期


            strSql = string.Format("select * from ybcfmxscindr a where a.jzlsh = '{0}' and a.cxbz = 1", jzlsh);
            ds.Tables.Clear();
            ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            if (ds == null || ds.Tables[0].Rows.Count == 0)
                return new object[] { 0, 0, "就诊流水号" + jzlsh + "无收费上传记录" };
            string cfmxjylsh = ds.Tables[0].Rows[0]["jylsh"].ToString();

            strSql = string.Format("select sum(je) as je from ybcfmxscindr a where a.jzlsh = '{0}' and a.cxbz = 1", jzlsh);
            ds.Tables.Clear();
            ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            double scfyze = Convert.ToDouble(ds.Tables[0].Rows[0]["je"].ToString());
            double inpufyze = Convert.ToDouble(ylfhj1);
            if (Math.Abs(inpufyze - scfyze) > 1.0)
                return new object[] { 0, 0, "医疗费用总额与上传费用总额相差" + Math.Abs(inpufyze - scfyze) + ",无法结算，请核实!" };

            strSql = string.Format("select * from ybmzzydjdr where jzlsh='{0}' and cxbz=1", jzlsh);
            ds.Tables.Clear();
            ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            if (ds == null || ds.Tables[0].Rows.Count == 0)
                return new object[] { 0, 0, "就诊流水号" + jzlsh + "无挂号或入院登记记录" };
            DataRow dr = ds.Tables[0].Rows[0];

            string ybjzlsh = dr["ybjzlsh"].ToString(); //住院号（在医保中心系统编号）
            string ybbh = dr["grbh"].ToString();       //  医保编号
            string bzbm1 = dr["bzbm"].ToString();      //病情编码
            string bzbm2 = dr["mmbzbm1"].ToString();   //病情编码(2)
            string bzbm3 = dr["mmbzbm2"].ToString();   //病情编码(3)
            string sfzssy = "000"; //是否正式收费(000 收费预览)

            #region 医保出院登记
            frm_cydjHF cydj = new frm_cydjHF(jzlsh);
            cydj.ShowDialog();
            string outParam_cydj = cydj.strValue;
            string[] s = outParam_cydj.Split('|');
            MessageBox.Show(outParam_cydj);
            if (s[0].Equals("0"))
                return new object[] { 0, 0, "医保出院登记失败" };

            //strValue ="1|" +jzlsh + "|" + cylxdm + "|" + cylxmc + "|" + cyzdbm1 + "|" + cyzdmc1 + "|" + cyzdbm2 + "|" + cyzdmc2 + "|" + cyzdbm3 + "|" + cyzdmc3;
            string cylxdm = s[2];
            string cyzdbm1 = s[4];
            string cyzdmc1 = s[5];
            string cyzdbm2 = s[6];
            string cyzdmc2 = s[7];
            string cyzdbm3 = s[8];
            string cyzdmc3 = s[9];
            string cyrq1 = s[10];

            strSql = string.Format(@"update ybmzzydjdr set cyzdjbbm1='{1}',cyzdjbmc1='{2}',cyzdjbbm2='{3}',cyzdjbmc2='{4}',cyzdjbbm3='{5}',cyzdjbmc3='{6}',cyrq='{7}',cylxdm='{8}' 
                                    where jzlsh='{0}' and cxbz=1",
                                    jzlsh, cyzdbm1, cyzdmc1, cyzdbm2, cyzdmc2, cyzdbm3, cyzdmc3, cyrq1, cylxdm);
            object[] obj = { strSql };
            obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);
            if (!obj[1].ToString().Equals("1"))
                return new object[] { 0, 0, "记录医保出院登记信息失败"+obj[2].ToString() };
            #endregion

            WriteLog(sysdate + "  进入住院费用预结算...");

            //入参
            putPara(whandler, "usr", ybuserid);
            putPara(whandler, "pwd", ybpasswd);

            putPara(whandler, "aac001", ybbh);
            putPara(whandler, "akc190", ybjzlsh);
            putPara(whandler, "aka120", bzbm1);
            putPara(whandler, "skc057", bzbm2);
            putPara(whandler, "skc058", bzbm3);
            putPara(whandler, "skc098", sfzssy);

            bfalg = process(whandler, "F04.06.10.02");
            if (bfalg)
            {
                getErrMsg(whandler, sMsg);
                sMsg1 = Encoding.Default.GetString(sMsg, 0, sMsg.Length).Replace("\0", "");
                if (sMsg1.Equals("OK"))
                {
                    #region 出参变量
                    string djh_r = string.Empty;    //单据号
                    string mzh_r = string.Empty;    //住院号
                    string ybbh_r = string.Empty;   //个人编号
                    string ylrylb_r = string.Empty; //医疗人员类别
                    string kh_r = string.Empty;     //iC卡号
                    string xm_r = string.Empty;       //姓名
                    string bcjsqzhye = string.Empty;//个人帐户余额(不含本次收费)
                    string tcjjzf = string.Empty;   //本次医保基金应支付金额
                    string xjzf = string.Empty;     //本次个人现金应支付金额
                    string zhzf = string.Empty;     //本次个人帐户应支付金额
                    string ylzje = string.Empty;    //本次医疗费总额
                    string bcxdjh = string.Empty;   //被冲销单据号
                    string cxbz_r = string.Empty;   //冲销标志（Z 正常；+ 被冲销；- 冲销）
                    string jbr_r = string.Empty;    //经办人(收费人员)
                    string jbrq = string.Empty;     //经办日期(收费日期)
                    string djdycs = string.Empty;   //单据打印次数
                    string cfxms = string.Empty;    //处方项目数
                    string jsrq = string.Empty;     //结算日期
                    string jsr = string.Empty;      //结算人
                    string grzhye = string.Empty;   //个人帐户余额
                    #endregion

                    #region 获取出参信息
                    getParaByName(whandler, "aae072", outParam);
                    djh_r = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "akc190", outParam);
                    mzh_r = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "aac001", outParam);
                    ybbh_r = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "akc020", outParam);
                    kh_r = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "aac003", outParam);
                    xm_r = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "akc252", outParam);
                    bcjsqzhye = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "akc260", outParam);
                    tcjjzf = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "akc261", outParam);
                    xjzf = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "akc262", outParam);
                    zhzf = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "akc264", outParam);
                    ylzje = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "skc093", outParam);
                    bcxdjh = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "skc033", outParam);
                    cxbz_r = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "aae011", outParam);
                    jbr_r = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "aae036", outParam);
                    jbrq = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "skc095", outParam);
                    djdycs = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "skc099", outParam);
                    cfxms = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "aae040", outParam);
                    jsrq = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "skc035", outParam);
                    jsr = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "akc087", outParam);
                    grzhye = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    decimal zbxje = Convert.ToDecimal(ylzje) - Convert.ToDecimal(xjzf);
                    #endregion
                    sValue = ylzje + zbxje + tcjjzf + "|0.00|" + zhzf + "|" + xjzf + "|0.00|0.00|0.00|0.00|" +
                          "0.00|0.00|0.00|0.00|0.00|0.00|0.00|0.00|0.00|0.00|" +
                          "0.00|0.00|0.00|0.00|0.00|" + bcjsqzhye + "|0.00|0.00|0.00|0.00|" +
                           "|0.00||0.00||0.00|" + jsrq + "|||||" + djh_r + "||" +
                          jsdjh + "|" + cxbz_r + "|||||||||||||||";

                    WriteLog(sysdate + "  住院费用预结算成功|" + sValue);
                    return new object[] { 0, 1, sValue };
                }
                else
                {
                    WriteLog(sysdate + "  获取住院费用预结算返回信息失败|" + sMsg1);
                    return new object[] { 0, 0, "获取住院费用预结算返回信息失败|" + sMsg1 };
                }
            }
            else
            {
                sMsg = new byte[1024];
                getFailReason(whandler, sMsg);
                sMsg1 = Encoding.Default.GetString(sMsg, 0, sMsg.Length).Replace("\0", "");
                WriteLog(sysdate + "  住院费用预结算失败" + sMsg1);
                return new object[] { 0, 0, "住院费用预结算失败" + sMsg1 };
            }

        }
        #endregion

        #region 住院费用预结算_生育保险
        public static object[] YBZYSFYJS_SYBX(object[] objParam)
        {
            string sysdate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");//当前时间
            string czygh = CliUtils.fLoginUser;   // 操作员工号
            string ywzqh = "";   // 业务周期号
            string jzlsh = objParam[0].ToString();   // 就诊流水号
            string cyyy = objParam[1].ToString();    // 出院原因代码（门诊传9）
            string zhsybz = objParam[2].ToString();  // 账户使用标志（0或1）
            string jbr = CliUtils.fUserName;     // 经办人姓名
            string dqrq = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");  // 当前日期
            string cyrq = objParam[4].ToString();//出院日期
            string jsdjh = "0000";    //结算单据号
            string ylfhj1 = objParam[5].ToString(); //医疗费合计 (新增)
            string ybmm = ""; //医保卡密码 (新增)

            bool bfalg = false;
            byte[] sMsg = new byte[1024];
            byte[] outParam = new byte[1024];
            string sMsg1 = string.Empty;
            string sValue = string.Empty;

            string strSql;
            DataSet ds = null;

            if (string.IsNullOrEmpty(jzlsh))
                return new object[] { 0, 0, "就诊流水号不能为空!" };

            if (string.IsNullOrEmpty(ylfhj1))
                return new object[] { 0, 0, "医疗合计费不能为空" };

            if (string.IsNullOrEmpty(zhsybz))
                return new object[] { 0, 0, "是否可用个人帐户支付自费部份需选择" };

            strSql = string.Format("select a.z1outd as cyrq from zy01d a where left(a.z1endv, 1) = '8' and a.z1zyno = '{0}'", jzlsh);
            ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            if (ds == null || ds.Tables[0].Rows.Count == 0)
                return new object[] { 0, 0, "就诊流水号" + jzlsh + "未拖出床位" };
            cyrq = Convert.ToDateTime(ds.Tables[0].Rows[0]["cyrq"]).ToString("yyyy-MM-dd"); //出院日期


            strSql = string.Format("select * from ybcfmxscindr a where a.jzlsh = '{0}' and a.cxbz = 1", jzlsh);
            ds.Tables.Clear();
            ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            if (ds == null || ds.Tables[0].Rows.Count == 0)
                return new object[] { 0, 0, "就诊流水号" + jzlsh + "无收费上传记录" };
            string cfmxjylsh = ds.Tables[0].Rows[0]["jylsh"].ToString();

            strSql = string.Format("select sum(je) as je from ybcfmxscindr a where a.jzlsh = '{0}' and a.cxbz = 1", jzlsh);
            ds.Tables.Clear();
            ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            double scfyze = Convert.ToDouble(ds.Tables[0].Rows[0]["je"].ToString());
            double inpufyze = Convert.ToDouble(ylfhj1);
            if (Math.Abs(inpufyze - scfyze) > 1.0)
                return new object[] { 0, 0, "医疗费用总额与上传费用总额相差" + Math.Abs(inpufyze - scfyze) + ",无法结算，请核实!" };

            strSql = string.Format("select * from ybmzzydjdr where jzlsh='{0}' and cxbz=1", jzlsh);
            ds.Tables.Clear();
            ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            if (ds == null || ds.Tables[0].Rows.Count == 0)
                return new object[] { 0, 0, "就诊流水号" + jzlsh + "无挂号或入院登记记录" };
            DataRow dr = ds.Tables[0].Rows[0];

            string ybjzlsh = dr["ybjzlsh"].ToString(); //住院号（在医保中心系统编号）
            string ybbh = dr["grbh"].ToString();       //  医保编号
            string bzbm1 = dr["bzbm"].ToString();      //病情编码
            string bzbm2 = dr["mmbzbm1"].ToString();   //病情编码(2)
            string bzbm3 = dr["mmbzbm2"].ToString();   //病情编码(3)
            string sfzssy = "000"; //是否正式收费(000 收费预览)

            #region 医保出院登记
            frm_cydjHF cydj = new frm_cydjHF(jzlsh);
            cydj.ShowDialog();
            string outParam_cydj = cydj.strValue;
            string[] s = outParam_cydj.Split('|');
            if (s[0].Equals("0"))
                return new object[] { 0, 0, "医保出院登记失败" };

            //strValue ="1|" +jzlsh + "|" + cylxdm + "|" + cylxmc + "|" + cyzdbm1 + "|" + cyzdmc1 + "|" + cyzdbm2 + "|" + cyzdmc2 + "|" + cyzdbm3 + "|" + cyzdmc3;
            string cylxdm = s[2];
            string cyzdbm1 = s[4];
            string cyzdmc1 = s[5];
            string cyzdbm2 = s[6];
            string cyzdmc2 = s[7];
            string cyzdbm3 = s[8];
            string cyzdmc3 = s[9];
            string cyrq1 = s[10];

            strSql = string.Format(@"update ybmzzydj set cyzdjbbm1='{1}',cyzdjbmc1='{2}',cyzdjbbm2='{3}',cyzdjbmc2='{4}',cyzdjbbm3='{5}',cyzdjbmc3='{6}',cyrq='{7}',cylxdm='{8}' 
                                    where jzlsh='{0}' and cxbz=1",
                                    jzlsh, cyzdbm1, cyzdmc1, cyzdbm2, cyzdmc2, cyzdbm3, cyzdmc3, cyrq1, cylxdm);
            object[] obj = { strSql };
            obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);
            if (!obj[1].ToString().Equals("1"))
                return new object[] { 0, 0, "记录医保出院登记信息失败" };
            #endregion

            WriteLog(sysdate + "  进入住院费用预结算...");

            //入参
            putPara(whandler, "usr", ybuserid);
            putPara(whandler, "pwd", ybpasswd);

            putPara(whandler, "aac001", ybbh);
            putPara(whandler, "akc190", ybjzlsh);
            putPara(whandler, "aka120", bzbm1);
            putPara(whandler, "skc057", bzbm2);
            putPara(whandler, "skc058", bzbm3);
            putPara(whandler, "skc098", sfzssy);
            putPara(whandler, "skc113", "S");
            putPara(whandler, "mkc191", "");
            putPara(whandler, "smc052", "000");
            putPara(whandler, "amc028", "1");

            bfalg = process(whandler, "F04.06.10.01");
            if (bfalg)
            {
                getErrMsg(whandler, sMsg);
                sMsg1 = Encoding.Default.GetString(sMsg, 0, sMsg.Length).Replace("\0", "");
                if (sMsg1.Equals("OK"))
                {
                    #region 出参变量
                    string djh_r = string.Empty;    //单据号
                    string mzh_r = string.Empty;    //住院号
                    string ybbh_r = string.Empty;   //个人编号
                    string ylrylb_r = string.Empty; //医疗人员类别
                    string kh_r = string.Empty;     //iC卡号
                    string xm_r = string.Empty;       //姓名
                    string bcjsqzhye = string.Empty;//个人帐户余额(不含本次收费)
                    string tcjjzf = string.Empty;   //本次医保基金应支付金额
                    string xjzf = string.Empty;     //本次个人现金应支付金额
                    string zhzf = string.Empty;     //本次个人帐户应支付金额
                    string ylzje = string.Empty;    //本次医疗费总额
                    string bcxdjh = string.Empty;   //被冲销单据号
                    string cxbz_r = string.Empty;   //冲销标志（Z 正常；+ 被冲销；- 冲销）
                    string jbr_r = string.Empty;    //经办人(收费人员)
                    string jbrq = string.Empty;     //经办日期(收费日期)
                    string djdycs = string.Empty;   //单据打印次数
                    string cfxms = string.Empty;    //处方项目数
                    string jsrq = string.Empty;     //结算日期
                    string jsr = string.Empty;      //结算人
                    string grzhye = string.Empty;   //个人帐户余额
                    #endregion

                    #region 获取出参信息
                    getParaByName(whandler, "aae072", outParam);
                    djh_r = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "akc190", outParam);
                    mzh_r = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "aac001", outParam);
                    ybbh_r = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "akc020", outParam);
                    kh_r = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "aac003", outParam);
                    xm_r = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "akc252", outParam);
                    bcjsqzhye = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "akc260", outParam);
                    tcjjzf = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "akc261", outParam);
                    xjzf = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "akc262", outParam);
                    zhzf = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "akc264", outParam);
                    ylzje = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "skc093", outParam);
                    bcxdjh = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "skc033", outParam);
                    cxbz_r = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "aae011", outParam);
                    jbr_r = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "aae036", outParam);
                    jbrq = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "skc095", outParam);
                    djdycs = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "skc099", outParam);
                    cfxms = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "aae040", outParam);
                    jsrq = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "skc035", outParam);
                    jsr = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "akc087", outParam);
                    grzhye = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    decimal zbxje = Convert.ToDecimal(ylzje) - Convert.ToDecimal(xjzf);
                    #endregion
                    sValue = ylzje + zbxje + tcjjzf + "|0.00|" + zhzf + "|" + xjzf + "|0.00|0.00|0.00|0.00|" +
                          "0.00|0.00|0.00|0.00|0.00|0.00|0.00|0.00|0.00|0.00|" +
                          "0.00|0.00|0.00|0.00|0.00|" + bcjsqzhye + "|0.00|0.00|0.00|0.00|" +
                           "|0.00||0.00||0.00|" + jsrq + "|||||" + djh_r + "||" +
                          jsdjh + "|" + cxbz_r + "|||||||||||||||";

                    WriteLog(sysdate + "  住院费用预结算成功|" + sValue);
                    //撤销费用登记
                    object[] obj2 = { ybbh_r, djh_r };
                    N_YBZYSFJSCX_SYBX(obj2);
                    return new object[] { 0, 1, sValue };
                }
                else
                {
                    WriteLog(sysdate + "  获取住院费用预结算返回信息失败|" + sMsg1);
                    return new object[] { 0, 0, "获取住院费用预结算返回信息失败|" + sMsg1 };
                }
            }
            else
            {
                sMsg = new byte[1024];
                getFailReason(whandler, sMsg);
                sMsg1 = Encoding.Default.GetString(sMsg, 0, sMsg.Length).Replace("\0", "");
                WriteLog(sysdate + "  住院费用预结算失败" + sMsg1);
                return new object[] { 0, 0, "住院费用预结算失败" + sMsg1 };
            }

        }
        #endregion

        #region 住院费用预结算_工伤保险
        public static object[] YBZYSFYJS_GSBX(object[] objParam)
        {
            string sysdate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");//当前时间
            string czygh = CliUtils.fLoginUser;   // 操作员工号
            string ywzqh = "";   // 业务周期号
            string jzlsh = objParam[0].ToString();   // 就诊流水号
            string cyyy = objParam[1].ToString();    // 出院原因代码（门诊传9）
            string zhsybz = objParam[2].ToString();  // 账户使用标志（0或1）
            string jbr = CliUtils.fUserName;     // 经办人姓名
            string dqrq = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");  // 当前日期
            string cyrq = objParam[4].ToString();//出院日期
            string jsdjh = "0000";    //结算单据号
            string ylfhj1 = objParam[5].ToString(); //医疗费合计 (新增)
            string ybmm = ""; //医保卡密码 (新增)

            bool bfalg = false;
            byte[] sMsg = new byte[1024];
            byte[] outParam = new byte[1024];
            string sMsg1 = string.Empty;
            string sValue = string.Empty;

            string strSql;
            DataSet ds = null;

            if (string.IsNullOrEmpty(jzlsh))
                return new object[] { 0, 0, "就诊流水号不能为空!" };

            if (string.IsNullOrEmpty(ylfhj1))
                return new object[] { 0, 0, "医疗合计费不能为空" };

            if (string.IsNullOrEmpty(zhsybz))
                return new object[] { 0, 0, "是否可用个人帐户支付自费部份需选择" };

            strSql = string.Format("select a.z1outd as cyrq from zy01d a where left(a.z1endv, 1) = '8' and a.z1zyno = '{0}'", jzlsh);
            ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            if (ds == null || ds.Tables[0].Rows.Count == 0)
                return new object[] { 0, 0, "就诊流水号" + jzlsh + "未拖出床位" };
            cyrq = Convert.ToDateTime(ds.Tables[0].Rows[0]["cyrq"]).ToString("yyyy-MM-dd"); //出院日期


            strSql = string.Format("select * from ybcfmxscindr a where a.jzlsh = '{0}' and a.cxbz = 1", jzlsh);
            ds.Tables.Clear();
            ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            if (ds == null || ds.Tables[0].Rows.Count == 0)
                return new object[] { 0, 0, "就诊流水号" + jzlsh + "无收费上传记录" };
            string cfmxjylsh = ds.Tables[0].Rows[0]["jylsh"].ToString();

            strSql = string.Format("select sum(je) as je from ybcfmxscindr a where a.jzlsh = '{0}' and a.cxbz = 1", jzlsh);
            ds.Tables.Clear();
            ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            double scfyze = Convert.ToDouble(ds.Tables[0].Rows[0]["je"].ToString());
            double inpufyze = Convert.ToDouble(ylfhj1);
            if (Math.Abs(inpufyze - scfyze) > 1.0)
                return new object[] { 0, 0, "医疗费用总额与上传费用总额相差" + Math.Abs(inpufyze - scfyze) + ",无法结算，请核实!" };

            strSql = string.Format("select * from ybmzzydjdr where jzlsh='{0}' and cxbz=1", jzlsh);
            ds.Tables.Clear();
            ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            if (ds == null || ds.Tables[0].Rows.Count == 0)
                return new object[] { 0, 0, "就诊流水号" + jzlsh + "无挂号或入院登记记录" };
            DataRow dr = ds.Tables[0].Rows[0];

            string ybjzlsh = dr["ybjzlsh"].ToString(); //住院号（在医保中心系统编号）
            string ybbh = dr["grbh"].ToString();       //  医保编号
            string bzbm1 = dr["bzbm"].ToString();      //病情编码
            string bzbm2 = dr["mmbzbm1"].ToString();   //病情编码(2)
            string bzbm3 = dr["mmbzbm2"].ToString();   //病情编码(3)
            string sfzssy = "000"; //是否正式收费(000 收费预览)

            #region 医保出院登记
            frm_cydjHF cydj = new frm_cydjHF(jzlsh);
            cydj.ShowDialog();
            string outParam_cydj = cydj.strValue;
            string[] s = outParam_cydj.Split('|');
            if (s[0].Equals("0"))
                return new object[] { 0, 0, "医保出院登记失败" };

            //strValue ="1|" +jzlsh + "|" + cylxdm + "|" + cylxmc + "|" + cyzdbm1 + "|" + cyzdmc1 + "|" + cyzdbm2 + "|" + cyzdmc2 + "|" + cyzdbm3 + "|" + cyzdmc3;
            string cylxdm = s[2];
            string cyzdbm1 = s[4];
            string cyzdmc1 = s[5];
            string cyzdbm2 = s[6];
            string cyzdmc2 = s[7];
            string cyzdbm3 = s[8];
            string cyzdmc3 = s[9];
            string cyrq1 = s[10];

            strSql = string.Format(@"update ybmzzydj set cyzdjbbm1='{1}',cyzdjbmc1='{2}',cyzdjbbm2='{3}',cyzdjbmc2='{4}',cyzdjbbm3='{5}',cyzdjbmc3='{6}',cyrq='{7}',cylxdm='{8}' 
                                    where jzlsh='{0}' and cxbz=1",
                                    jzlsh, cyzdbm1, cyzdmc1, cyzdbm2, cyzdmc2, cyzdbm3, cyzdmc3, cyrq1, cylxdm);
            object[] obj = { strSql };
            obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);
            if (!obj[1].ToString().Equals("1"))
                return new object[] { 0, 0, "记录医保出院登记信息失败" };
            #endregion

            WriteLog(sysdate + "  进入住院费用预结算...");

            //入参
            putPara(whandler, "usr", ybuserid);
            putPara(whandler, "pwd", ybpasswd);

            putPara(whandler, "aac001", ybbh);
            putPara(whandler, "akc190", ybjzlsh);
            putPara(whandler, "aka120", bzbm1);
            putPara(whandler, "skc057", bzbm2);
            putPara(whandler, "skc058", bzbm3);
            putPara(whandler, "skc098", sfzssy);
            putPara(whandler, "smc052", "000");

            bfalg = process(whandler, "F07.11.03.01");
            if (bfalg)
            {
                getErrMsg(whandler, sMsg);
                sMsg1 = Encoding.Default.GetString(sMsg, 0, sMsg.Length).Replace("\0", "");
                if (sMsg1.Equals("OK"))
                {
                    #region 出参变量
                    string djh_r = string.Empty;    //单据号
                    string mzh_r = string.Empty;    //住院号
                    string ybbh_r = string.Empty;   //个人编号
                    string ylrylb_r = string.Empty; //医疗人员类别
                    string kh_r = string.Empty;     //iC卡号
                    string xm_r = string.Empty;       //姓名
                    string bcjsqzhye = string.Empty;//个人帐户余额(不含本次收费)
                    string tcjjzf = string.Empty;   //本次医保基金应支付金额
                    string xjzf = string.Empty;     //本次个人现金应支付金额
                    string zhzf = string.Empty;     //本次个人帐户应支付金额
                    string ylzje = string.Empty;    //本次医疗费总额
                    string bcxdjh = string.Empty;   //被冲销单据号
                    string cxbz_r = string.Empty;   //冲销标志（Z 正常；+ 被冲销；- 冲销）
                    string jbr_r = string.Empty;    //经办人(收费人员)
                    string jbrq = string.Empty;     //经办日期(收费日期)
                    string djdycs = string.Empty;   //单据打印次数
                    string cfxms = string.Empty;    //处方项目数
                    string jsrq = string.Empty;     //结算日期
                    string jsr = string.Empty;      //结算人
                    string grzhye = string.Empty;   //个人帐户余额
                    #endregion

                    #region 获取出参信息
                    getParaByName(whandler, "aae072", outParam);
                    djh_r = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "akc190", outParam);
                    mzh_r = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "aac001", outParam);
                    ybbh_r = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "akc020", outParam);
                    kh_r = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "aac003", outParam);
                    xm_r = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    //outParam = new byte[1024];
                    //getParaByName(whandler, "akc252", outParam);
                    //bcjsqzhye = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "akc260", outParam);
                    tcjjzf = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "akc261", outParam);
                    xjzf = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    //outParam = new byte[1024];
                    //getParaByName(whandler, "akc262", outParam);
                    //zhzf = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "akc264", outParam);
                    ylzje = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "skc093", outParam);
                    bcxdjh = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "skc033", outParam);
                    cxbz_r = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "aae011", outParam);
                    jbr_r = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "aae036", outParam);
                    jbrq = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "skc095", outParam);
                    djdycs = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "skc099", outParam);
                    cfxms = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "aae040", outParam);
                    jsrq = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "skc035", outParam);
                    jsr = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    //outParam = new byte[1024];
                    //getParaByName(whandler, "akc087", outParam);
                    //grzhye = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");

                    decimal zbxje = Convert.ToDecimal(ylzje) - Convert.ToDecimal(xjzf);
                    #endregion
                    sValue = ylzje + zbxje + tcjjzf + "|0.00|" + zhzf + "|" + xjzf + "|0.00|0.00|0.00|0.00|" +
                          "0.00|0.00|0.00|0.00|0.00|0.00|0.00|0.00|0.00|0.00|" +
                          "0.00|0.00|0.00|0.00|0.00|" + bcjsqzhye + "|0.00|0.00|0.00|0.00|" +
                           "|0.00||0.00||0.00|" + jsrq + "|||||" + djh_r + "||" +
                          jsdjh + "|" + cxbz_r + "|||||||||||||||";

                    WriteLog(sysdate + "  住院费用预结算成功|" + sValue);
                    //撤销结算信息
                    object[] obj2 = { ybbh_r, djh_r };
                    N_YBZYSFJSCX_GSBX(obj2);
                    return new object[] { 0, 1, sValue };
                }
                else
                {
                    WriteLog(sysdate + "  获取住院费用预结算返回信息失败|" + sMsg1);
                    return new object[] { 0, 0, "获取住院费用预结算返回信息失败|" + sMsg1 };
                }
            }
            else
            {
                sMsg = new byte[1024];
                getFailReason(whandler, sMsg);
                sMsg1 = Encoding.Default.GetString(sMsg, 0, sMsg.Length).Replace("\0", "");
                WriteLog(sysdate + "  住院费用预结算失败" + sMsg1);
                return new object[] { 0, 0, "住院费用预结算失败" + sMsg1 };
            }

        }
        #endregion


        #region 住院费用结算
        public static object[] YBZYSFJS(object[] objParam)
        {
            string jzlsh = objParam[0].ToString();  // 就诊流水号 *//对应玉山 住院号
            if (string.IsNullOrEmpty(jzlsh))
                return new object[] { 0, 0, "就诊流水号不能为空" };
            string strSql = string.Format(@"select xzbm from ybmzzydjdr where jzlsh='{0}' and cxbz=1", objParam[0].ToString());
            DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            if (ds == null || ds.Tables[0].Rows.Count == 0)
                return new object[] { 0, 0, "未做医保住院登记信息" };
            string sxzbm = ds.Tables[0].Rows[0]["xzbm"].ToString();
            object[] objReturn = null;
            switch (sxzbm)
            {
                case "310":
                    objReturn = YBZYSFJS_YLBX(objParam);
                    break;
                case "410":
                    objReturn = YBZYSFJS_GSBX(objParam);
                    break;
                case "510":
                    objReturn = YBZYSFJS_SYBX(objParam);
                    break;
                default:
                    objReturn = new object[] { 0, 0, "未找到方法体" };
                    break;
            }
            return objReturn;
        }
        #endregion

        #region 住院费用结算_医疗保险
        public static object[] YBZYSFJS_YLBX(object[] objParam)
        {
            string sysdate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");//当前时间
            string czygh = CliUtils.fLoginUser;      // 操作员工号
            string ywzqh = "";      // 业务周期号
            string jzlsh = objParam[0].ToString();      // 就诊流水号
            string djh = objParam[1].ToString();        // 单据号
            string cyyy = objParam[2].ToString();       // 出院原因代码 
            string zhsybz = objParam[3].ToString();     // 账户使用标志（0或1）
            string jbr = CliUtils.fUserName;        // 经办人姓名
            string dqrq = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");  // 当前日期
            string jsdjh = djh;    //结算单据号 门诊以12位时间+流水号后5位作为单据号
            string cyrq = objParam[5].ToString(); ;
            string ylfhj1 = objParam[6].ToString(); //医疗费合计 (新增)

            string jylsh = DateTime.Now.ToString("yyyyMMddHHmmss") + "-102002-" + new Random().Next(100).ToString().PadLeft(4, '0');
            string strSql;
            DataSet ds;

            bool bfalg = false;
            byte[] sMsg = new byte[1024];
            byte[] outParam = new byte[1024];
            string sMsg1 = string.Empty;
            string sValue = string.Empty;

            if (string.IsNullOrEmpty(jzlsh))
                return new object[] { 0, 0, "就诊流水号不能为空!" };

            if (string.IsNullOrEmpty(ylfhj1))
                return new object[] { 0, 0, "医疗合计费不能为空" };

            if (string.IsNullOrEmpty(zhsybz))
                return new object[] { 0, 0, "是否可用个人帐户支付自费部份需选择" };

            strSql = string.Format("select a.z1outd as cyrq from zy01d a where left(a.z1endv, 1) = '8' and a.z1zyno = '{0}'", jzlsh);
            ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            if (ds == null || ds.Tables[0].Rows.Count == 0)
                return new object[] { 0, 0, "就诊流水号" + jzlsh + "未拖出床位" };
            cyrq = Convert.ToDateTime(ds.Tables[0].Rows[0]["cyrq"]).ToString("yyyy-MM-dd"); //出院日期


            strSql = string.Format("select * from ybcfmxscindr a where a.jzlsh = '{0}' and a.cxbz = 1", jzlsh);
            ds.Tables.Clear();
            ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            if (ds == null || ds.Tables[0].Rows.Count == 0)
                return new object[] { 0, 0, "就诊流水号" + jzlsh + "无收费上传记录" };
            string cfmxjylsh = ds.Tables[0].Rows[0]["jylsh"].ToString();
            string scpch = ds.Tables[0].Rows[0]["jylsh"].ToString(); ;

            strSql = string.Format("select sum(je) as je from ybcfmxscindr a where a.jzlsh = '{0}' and a.cxbz = 1", jzlsh);
            ds.Tables.Clear();
            ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            double scfyze = Convert.ToDouble(ds.Tables[0].Rows[0]["je"].ToString());
            double inpufyze = Convert.ToDouble(ylfhj1);
            if (Math.Abs(inpufyze - scfyze) > 1.0)
                return new object[] { 0, 0, "医疗费用总额与上传费用总额相差" + Math.Abs(inpufyze - scfyze) + ",无法结算，请核实!" };


            strSql = string.Format("select * from ybmzzydjdr where jzlsh='{0}' and cxbz=1", jzlsh);
            ds.Tables.Clear();
            ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            if (ds == null || ds.Tables[0].Rows.Count == 0)
                return new object[] { 0, 0, "就诊流水号" + jzlsh + "无挂号或入院登记记录" };
            DataRow dr = ds.Tables[0].Rows[0];

            string ybjzlsh = dr["ybjzlsh"].ToString(); //住院号（在医保中心系统编号）
            string ybbh = dr["grbh"].ToString();       //  医保编号
            string bzbm1 = dr["bzbm"].ToString();      //病情编码
            string bzbm2 = dr["mmbzbm1"].ToString();   //病情编码(2)
            string bzbm3 = dr["mmbzbm2"].ToString();   //病情编码(3)
            string sfzssy = "001"; //是否正式收费(000 收费预览)

            WriteLog(sysdate + "  进入住院费用结算...");

            //入参
            putPara(whandler, "usr", ybuserid);
            putPara(whandler, "pwd", ybpasswd);

            putPara(whandler, "aac001", ybbh);
            putPara(whandler, "akc190", ybjzlsh);
            putPara(whandler, "aka120", bzbm1);
            putPara(whandler, "skc057", bzbm2);
            putPara(whandler, "skc058", bzbm3);
            putPara(whandler, "skc098", sfzssy);

            bfalg = process(whandler, "F04.06.10.01");
            if (bfalg)
            {
                getErrMsg(whandler, sMsg);
                sMsg1 = Encoding.Default.GetString(sMsg, 0, sMsg.Length).Replace("\0", "");
                if (sMsg1.Equals("OK"))
                {
                    List<string> liSQL = new List<string>();
                    #region 出参变量
                    string djh_r = string.Empty;    //单据号
                    string zyh_r = string.Empty;    //住院号
                    string ybbh_r = string.Empty;   //个人编号
                    string ylrylb_r = string.Empty; //医疗人员类别
                    string kh_r = string.Empty;     //iC卡号
                    string xm_r = string.Empty;       //姓名
                    string bcjsqzhye = string.Empty;//个人帐户余额(不含本次收费)
                    string tcjjzf = string.Empty;   //本次医保基金应支付金额
                    string xjzf = string.Empty;     //本次个人现金应支付金额
                    string zhzf = string.Empty;     //本次个人帐户应支付金额
                    string ylzje = string.Empty;    //本次医疗费总额
                    string bcxdjh = string.Empty;   //被冲销单据号
                    string cxbz_r = string.Empty;   //冲销标志（Z 正常；+ 被冲销；- 冲销）
                    string jbr_r = string.Empty;    //经办人(收费人员)
                    string jbrq = string.Empty;     //经办日期(收费日期)
                    string djdycs = string.Empty;   //单据打印次数
                    string cfxms = string.Empty;    //处方项目数
                    string jsrq = string.Empty;     //结算日期
                    string jsr = string.Empty;      //结算人
                    string grzhye = string.Empty;   //个人帐户余额
                    #endregion

                    #region 获取出参信息
                    getParaByName(whandler, "aae072", outParam);
                    djh_r = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "akc190", outParam);
                    zyh_r = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "aac001", outParam);
                    ybbh_r = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "akc020", outParam);
                    kh_r = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "aac003", outParam);
                    xm_r = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "akc252", outParam);
                    bcjsqzhye = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "akc260", outParam);
                    tcjjzf = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "akc261", outParam);
                    xjzf = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "akc262", outParam);
                    zhzf = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "akc264", outParam);
                    ylzje = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "skc093", outParam);
                    bcxdjh = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "skc033", outParam);
                    cxbz_r = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "aae011", outParam);
                    jbr_r = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "aae036", outParam);
                    jbrq = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "skc095", outParam);
                    djdycs = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "skc099", outParam);
                    cfxms = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "aae040", outParam);
                    jsrq = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "skc035", outParam);
                    jsr = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "akc087", outParam);
                    grzhye = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");

                    decimal zbxje = Convert.ToDecimal(ylzje) - Convert.ToDecimal(xjzf);
                    #endregion
                    sValue = ylzje + zbxje + tcjjzf + "|0.00|" + zhzf + "|" + xjzf + "|0.00|0.00|0.00|0.00|" +
                          "0.00|0.00|0.00|0.00|0.00|0.00|0.00|0.00|0.00|0.00|" +
                          "0.00|0.00|0.00|0.00|0.00|" + bcjsqzhye + "|0.00|0.00|0.00|0.00|" +
                           "|0.00||0.00||0.00|" + jsrq + "|||||" + djh_r + "||" +
                          jsdjh + "|" + cxbz_r + "|||||||||||||||";
                    strSql = string.Format(@"insert into ybfyjsdr( jzlsh,jylsh,jbr,ylfze,tcjjzf,zhzf,xjzf,djh,djhin,grbh,
                                            kh,xm,bcjsqzhye,cxbz1,jbrq,jsrq,jsr,grzfye,cfmxs,ybdjh,sysdate,ybjzlsh) values(
                                            '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}',
                                            '{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}','{20}','{21}')",
                                            jzlsh, jylsh, jbr_r, ylzje, tcjjzf, zhzf, xjzf, djh, bcxdjh, ybbh_r,
                                            kh_r, xm_r, bcjsqzhye, cxbz_r, jbrq, jsrq, jsr, grzhye, cfxms, djh_r, sysdate, zyh_r);

                    liSQL.Add(strSql);

                    //插入批次号
                    strSql = string.Format(@"insert into yb_pcxx(pch,bz,djh,jzbz) values('{0}','{1}','{2}','{3}')", scpch, "1", djh, "z");
                    liSQL.Add(strSql);
                    object[] obj = liSQL.ToArray();
                    obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);
                    if (obj[1].ToString().Equals("1"))
                    {
                        WriteLog(sysdate + "  住院费用结算成功|" + sValue);
                        //进入出院登记
                        object[] obj2 = { jzlsh };
                        obj2 = YBCYDJ(obj2);
                        if (obj2[1].ToString().Equals("1"))
                        {
                            WriteLog(sysdate + "  住院费用结算成功|出院登记成功|");
                            return new object[] { 0, 1, sValue };
                        }
                        else
                        {
                            WriteLog(sysdate + "  住院费用结算成功|出院登记失败|");
                            //撤销医保登记
                            object[] obj3 = { ybbh_r, djh_r };
                            N_YBZYSFJSCX_YLBX(obj3);
                            return new object[] { 0, 0, "住院费用结算成功|出院登记失败" };
                        }
                    }
                    else
                    {
                        //撤销医保结算
                        object[] obj1 = { ybbh_r, djh_r };
                        N_YBZYSFJSCX_YLBX(obj1);

                        WriteLog(sysdate + "  住院费用结算失败|数据操作失败|" + obj[2].ToString());
                        return new object[] { 0, 1, "住院费用结算失败|数据操作失败|" + obj[2].ToString() };
                    }
                }
                else
                {
                    WriteLog(sysdate + "  获取住院费用结算返回信息失败|" + sMsg1);
                    return new object[] { 0, 0, "获取住院费用结算返回信息失败|" + sMsg1 };
                }
            }
            else
            {
                sMsg = new byte[1024];
                getFailReason(whandler, sMsg);
                sMsg1 = Encoding.Default.GetString(sMsg, 0, sMsg.Length).Replace("\0", "");
                WriteLog(sysdate + "  住院费用结算失败" + sMsg1);
                return new object[] { 0, 0, "住院费用结算失败" + sMsg1 };
            }
        }
        #endregion

        #region 住院费用结算_生育保险
        public static object[] YBZYSFJS_SYBX(object[] objParam)
        {
            string sysdate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");//当前时间
            string czygh = CliUtils.fLoginUser;      // 操作员工号
            string ywzqh = "";      // 业务周期号
            string jzlsh = objParam[0].ToString();      // 就诊流水号
            string djh = objParam[1].ToString();        // 单据号
            string cyyy = objParam[2].ToString();       // 出院原因代码 
            string zhsybz = objParam[3].ToString();     // 账户使用标志（0或1）
            string jbr = CliUtils.fUserName;        // 经办人姓名
            string dqrq = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");  // 当前日期
            string jsdjh = djh;    //结算单据号 门诊以12位时间+流水号后5位作为单据号
            string cyrq = objParam[5].ToString(); ;
            string ylfhj1 = objParam[6].ToString(); //医疗费合计 (新增)

            string jylsh = DateTime.Now.ToString("yyyyMMddHHmmss") + "-102002-" + new Random().Next(100).ToString().PadLeft(4, '0');
            string strSql;
            DataSet ds;

            bool bfalg = false;
            byte[] sMsg = new byte[1024];
            byte[] outParam = new byte[1024];
            string sMsg1 = string.Empty;
            string sValue = string.Empty;

            if (string.IsNullOrEmpty(jzlsh))
                return new object[] { 0, 0, "就诊流水号不能为空!" };

            if (string.IsNullOrEmpty(ylfhj1))
                return new object[] { 0, 0, "医疗合计费不能为空" };

            if (string.IsNullOrEmpty(zhsybz))
                return new object[] { 0, 0, "是否可用个人帐户支付自费部份需选择" };

            strSql = string.Format("select a.z1outd as cyrq from zy01d a where left(a.z1endv, 1) = '8' and a.z1zyno = '{0}'", jzlsh);
            ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            if (ds == null || ds.Tables[0].Rows.Count == 0)
                return new object[] { 0, 0, "就诊流水号" + jzlsh + "未拖出床位" };
            cyrq = Convert.ToDateTime(ds.Tables[0].Rows[0]["cyrq"]).ToString("yyyy-MM-dd"); //出院日期


            strSql = string.Format("select * from ybcfmxscindr a where a.jzlsh = '{0}' and a.cxbz = 1", jzlsh);
            ds.Tables.Clear();
            ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            if (ds == null || ds.Tables[0].Rows.Count == 0)
                return new object[] { 0, 0, "就诊流水号" + jzlsh + "无收费上传记录" };
            string cfmxjylsh = ds.Tables[0].Rows[0]["jylsh"].ToString();
            string scpch = ds.Tables[0].Rows[0]["jylsh"].ToString(); ;

            strSql = string.Format("select sum(je) as je from ybcfmxscindr a where a.jzlsh = '{0}' and a.cxbz = 1", jzlsh);
            ds.Tables.Clear();
            ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            double scfyze = Convert.ToDouble(ds.Tables[0].Rows[0]["je"].ToString());
            double inpufyze = Convert.ToDouble(ylfhj1);
            if (Math.Abs(inpufyze - scfyze) > 1.0)
                return new object[] { 0, 0, "医疗费用总额与上传费用总额相差" + Math.Abs(inpufyze - scfyze) + ",无法结算，请核实!" };


            strSql = string.Format("select * from ybmzzydjdr where jzlsh='{0}' and cxbz=1", jzlsh);
            ds.Tables.Clear();
            ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            if (ds == null || ds.Tables[0].Rows.Count == 0)
                return new object[] { 0, 0, "就诊流水号" + jzlsh + "无挂号或入院登记记录" };
            DataRow dr = ds.Tables[0].Rows[0];

            string ybjzlsh = dr["ybjzlsh"].ToString(); //住院号（在医保中心系统编号）
            string ybbh = dr["grbh"].ToString();       //  医保编号
            string bzbm1 = dr["bzbm"].ToString();      //病情编码
            string bzbm2 = dr["mmbzbm1"].ToString();   //病情编码(2)
            string bzbm3 = dr["mmbzbm2"].ToString();   //病情编码(3)
            string sfzssy = "001"; //是否正式收费(000 收费预览)

            WriteLog(sysdate + "  进入住院费用结算...");

            //入参
            putPara(whandler, "usr", ybuserid);
            putPara(whandler, "pwd", ybpasswd);

            putPara(whandler, "aac001", ybbh);
            putPara(whandler, "akc190", ybjzlsh);
            putPara(whandler, "aka120", bzbm1);
            putPara(whandler, "skc057", bzbm2);
            putPara(whandler, "skc058", bzbm3);
            putPara(whandler, "skc098", sfzssy);
            putPara(whandler, "skc113", "S");
            putPara(whandler, "mkc191", "");
            putPara(whandler, "smc052", "000");
            putPara(whandler, "amc028", "1");

            bfalg = process(whandler, "F04.06.10.01");
            if (bfalg)
            {
                getErrMsg(whandler, sMsg);
                sMsg1 = Encoding.Default.GetString(sMsg, 0, sMsg.Length).Replace("\0", "");
                if (sMsg1.Equals("OK"))
                {
                    List<string> liSQL = new List<string>();
                    #region 出参变量
                    string djh_r = string.Empty;    //单据号
                    string zyh_r = string.Empty;    //住院号
                    string ybbh_r = string.Empty;   //个人编号
                    string ylrylb_r = string.Empty; //医疗人员类别
                    string kh_r = string.Empty;     //iC卡号
                    string xm_r = string.Empty;       //姓名
                    string bcjsqzhye = string.Empty;//个人帐户余额(不含本次收费)
                    string tcjjzf = string.Empty;   //本次医保基金应支付金额
                    string xjzf = string.Empty;     //本次个人现金应支付金额
                    string zhzf = string.Empty;     //本次个人帐户应支付金额
                    string ylzje = string.Empty;    //本次医疗费总额
                    string bcxdjh = string.Empty;   //被冲销单据号
                    string cxbz_r = string.Empty;   //冲销标志（Z 正常；+ 被冲销；- 冲销）
                    string jbr_r = string.Empty;    //经办人(收费人员)
                    string jbrq = string.Empty;     //经办日期(收费日期)
                    string djdycs = string.Empty;   //单据打印次数
                    string cfxms = string.Empty;    //处方项目数
                    string jsrq = string.Empty;     //结算日期
                    string jsr = string.Empty;      //结算人
                    string grzhye = string.Empty;   //个人帐户余额
                    #endregion

                    #region 获取出参信息
                    getParaByName(whandler, "aae072", outParam);
                    djh_r = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "akc190", outParam);
                    zyh_r = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "aac001", outParam);
                    ybbh_r = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "akc020", outParam);
                    kh_r = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "aac003", outParam);
                    xm_r = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "akc252", outParam);
                    bcjsqzhye = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "akc260", outParam);
                    tcjjzf = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "akc261", outParam);
                    xjzf = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "akc262", outParam);
                    zhzf = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "akc264", outParam);
                    ylzje = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "skc093", outParam);
                    bcxdjh = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "skc033", outParam);
                    cxbz_r = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "aae011", outParam);
                    jbr_r = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "aae036", outParam);
                    jbrq = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "skc095", outParam);
                    djdycs = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "skc099", outParam);
                    cfxms = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "aae040", outParam);
                    jsrq = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "skc035", outParam);
                    jsr = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "akc087", outParam);
                    grzhye = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    decimal zbxje = Convert.ToDecimal(ylzje) - Convert.ToDecimal(xjzf);
                    #endregion
                    sValue = ylzje + zbxje + tcjjzf + "|0.00|" + zhzf + "|" + xjzf + "|0.00|0.00|0.00|0.00|" +
                          "0.00|0.00|0.00|0.00|0.00|0.00|0.00|0.00|0.00|0.00|" +
                          "0.00|0.00|0.00|0.00|0.00|" + bcjsqzhye + "|0.00|0.00|0.00|0.00|" +
                           "|0.00||0.00||0.00|" + jsrq + "|||||" + djh_r + "||" +
                          jsdjh + "|" + cxbz_r + "|||||||||||||||";
                    strSql = string.Format(@"insert into ybfyjsdr( jzlsh,jylsh,jbr,ylfze,tcjjzf,zhzf,xjzf,djh,djhin,grbh,
                                            kh,xm,bcjsqzhye,cxbz1,jbrq,jsrq,jsr,grzfye,cfmxs,ybdjh,sysdate,ybjzlsh) values(
                                            '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}',
                                            '{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}','{20}','{21}')",
                                            jzlsh, jylsh, jbr_r, ylzje, tcjjzf, zhzf, xjzf, djh, bcxdjh, ybbh_r,
                                            kh_r, xm_r, bcjsqzhye, cxbz_r, jbrq, jsrq, jsr, grzhye, cfxms, djh_r, sysdate, zyh_r);

                    liSQL.Add(strSql);

                    object[] obj = liSQL.ToArray();
                    obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);
                    if (obj[1].ToString().Equals("1"))
                    {
                        WriteLog(sysdate + "  住院费用结算成功|" + sValue);
                        //进入出院登记
                        object[] obj2 = { jzlsh };
                        obj2 = YBCYDJ(obj2);
                        if (obj2[1].ToString().Equals("1"))
                        {
                            WriteLog(sysdate + "  住院费用结算成功|出院登记成功|");
                            return new object[] { 0, 1, sValue };
                        }
                        else
                        {
                            WriteLog(sysdate + "  住院费用结算成功|出院登记失败|");
                            //撤销医保登记
                            object[] obj3 = { ybbh_r, djh_r };
                            N_YBZYSFJSCX_SYBX(obj3);
                            return new object[] { 0, 0, "住院费用结算成功|出院登记失败" };
                        }
                    }
                    else
                    {
                        //撤销医保结算
                        object[] obj1 = { ybbh_r, djh_r };
                        N_YBZYSFJSCX_SYBX(obj1);

                        WriteLog(sysdate + "  住院费用结算失败|数据操作失败|" + obj[2].ToString());
                        return new object[] { 0, 1, "住院费用结算失败|数据操作失败|" + obj[2].ToString() };
                    }
                }
                else
                {
                    WriteLog(sysdate + "  获取住院费用结算返回信息失败|" + sMsg1);
                    return new object[] { 0, 0, "获取住院费用结算返回信息失败|" + sMsg1 };
                }
            }
            else
            {
                sMsg = new byte[1024];
                getFailReason(whandler, sMsg);
                sMsg1 = Encoding.Default.GetString(sMsg, 0, sMsg.Length).Replace("\0", "");
                WriteLog(sysdate + "  住院费用结算失败" + sMsg1);
                return new object[] { 0, 0, "住院费用结算失败" + sMsg1 };
            }
        }
        #endregion

        #region 住院费用结算_工伤保险
        public static object[] YBZYSFJS_GSBX(object[] objParam)
        {
            string sysdate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");//当前时间
            string czygh = CliUtils.fLoginUser;      // 操作员工号
            string ywzqh = "";      // 业务周期号
            string jzlsh = objParam[0].ToString();      // 就诊流水号
            string djh = objParam[1].ToString();        // 单据号
            string cyyy = objParam[2].ToString();       // 出院原因代码 
            string zhsybz = objParam[3].ToString();     // 账户使用标志（0或1）
            string jbr = CliUtils.fUserName;        // 经办人姓名
            string dqrq = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");  // 当前日期
            string jsdjh = djh;    //结算单据号 门诊以12位时间+流水号后5位作为单据号
            string cyrq = objParam[5].ToString(); ;
            string ylfhj1 = objParam[6].ToString(); //医疗费合计 (新增)

            string jylsh = DateTime.Now.ToString("yyyyMMddHHmmss") + "-102002-" + new Random().Next(100).ToString().PadLeft(4, '0');
            string strSql;
            DataSet ds;

            bool bfalg = false;
            byte[] sMsg = new byte[1024];
            byte[] outParam = new byte[1024];
            string sMsg1 = string.Empty;
            string sValue = string.Empty;

            if (string.IsNullOrEmpty(jzlsh))
                return new object[] { 0, 0, "就诊流水号不能为空!" };

            if (string.IsNullOrEmpty(ylfhj1))
                return new object[] { 0, 0, "医疗合计费不能为空" };

            if (string.IsNullOrEmpty(zhsybz))
                return new object[] { 0, 0, "是否可用个人帐户支付自费部份需选择" };

            strSql = string.Format("select a.z1outd as cyrq from zy01d a where left(a.z1endv, 1) = '8' and a.z1zyno = '{0}'", jzlsh);
            ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            if (ds == null || ds.Tables[0].Rows.Count == 0)
                return new object[] { 0, 0, "就诊流水号" + jzlsh + "未拖出床位" };
            cyrq = Convert.ToDateTime(ds.Tables[0].Rows[0]["cyrq"]).ToString("yyyy-MM-dd"); //出院日期


            strSql = string.Format("select * from ybcfmxscindr a where a.jzlsh = '{0}' and a.cxbz = 1", jzlsh);
            ds.Tables.Clear();
            ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            if (ds == null || ds.Tables[0].Rows.Count == 0)
                return new object[] { 0, 0, "就诊流水号" + jzlsh + "无收费上传记录" };
            string cfmxjylsh = ds.Tables[0].Rows[0]["jylsh"].ToString();
            string scpch = ds.Tables[0].Rows[0]["jylsh"].ToString(); ;

            strSql = string.Format("select sum(je) as je from ybcfmxscindr a where a.jzlsh = '{0}' and a.cxbz = 1", jzlsh);
            ds.Tables.Clear();
            ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            double scfyze = Convert.ToDouble(ds.Tables[0].Rows[0]["je"].ToString());
            double inpufyze = Convert.ToDouble(ylfhj1);
            if (Math.Abs(inpufyze - scfyze) > 1.0)
                return new object[] { 0, 0, "医疗费用总额与上传费用总额相差" + Math.Abs(inpufyze - scfyze) + ",无法结算，请核实!" };


            strSql = string.Format("select * from ybmzzydjdr where jzlsh='{0}' and cxbz=1", jzlsh);
            ds.Tables.Clear();
            ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            if (ds == null || ds.Tables[0].Rows.Count == 0)
                return new object[] { 0, 0, "就诊流水号" + jzlsh + "无挂号或入院登记记录" };
            DataRow dr = ds.Tables[0].Rows[0];

            string ybjzlsh = dr["ybjzlsh"].ToString(); //住院号（在医保中心系统编号）
            string ybbh = dr["grbh"].ToString();       //  医保编号
            string bzbm1 = dr["bzbm"].ToString();      //病情编码
            string bzbm2 = dr["mmbzbm1"].ToString();   //病情编码(2)
            string bzbm3 = dr["mmbzbm2"].ToString();   //病情编码(3)
            string sfzssy = "001"; //是否正式收费(000 收费预览)

            WriteLog(sysdate + "  进入住院费用结算...");

            //入参
            putPara(whandler, "usr", ybuserid);
            putPara(whandler, "pwd", ybpasswd);

            putPara(whandler, "aac001", ybbh);
            putPara(whandler, "akc190", ybjzlsh);
            putPara(whandler, "aka120", bzbm1);
            putPara(whandler, "skc057", bzbm2);
            putPara(whandler, "skc058", bzbm3);
            putPara(whandler, "skc098", sfzssy);
            putPara(whandler, "smc052", "000");

            bfalg = process(whandler, "F07.11.03.01");
            if (bfalg)
            {
                getErrMsg(whandler, sMsg);
                sMsg1 = Encoding.Default.GetString(sMsg, 0, sMsg.Length).Replace("\0", "");
                if (sMsg1.Equals("OK"))
                {
                    List<string> liSQL = new List<string>();
                    #region 出参变量
                    string djh_r = string.Empty;    //单据号
                    string zyh_r = string.Empty;    //住院号
                    string ybbh_r = string.Empty;   //个人编号
                    string ylrylb_r = string.Empty; //医疗人员类别
                    string kh_r = string.Empty;     //iC卡号
                    string xm_r = string.Empty;       //姓名
                    string bcjsqzhye = string.Empty;//个人帐户余额(不含本次收费)
                    string tcjjzf = string.Empty;   //本次医保基金应支付金额
                    string xjzf = string.Empty;     //本次个人现金应支付金额
                    string zhzf = string.Empty;     //本次个人帐户应支付金额
                    string ylzje = string.Empty;    //本次医疗费总额
                    string bcxdjh = string.Empty;   //被冲销单据号
                    string cxbz_r = string.Empty;   //冲销标志（Z 正常；+ 被冲销；- 冲销）
                    string jbr_r = string.Empty;    //经办人(收费人员)
                    string jbrq = string.Empty;     //经办日期(收费日期)
                    string djdycs = string.Empty;   //单据打印次数
                    string cfxms = string.Empty;    //处方项目数
                    string jsrq = string.Empty;     //结算日期
                    string jsr = string.Empty;      //结算人
                    string grzhye = string.Empty;   //个人帐户余额
                    #endregion

                    #region 获取出参信息
                    getParaByName(whandler, "aae072", outParam);
                    djh_r = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "akc190", outParam);
                    zyh_r = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "aac001", outParam);
                    ybbh_r = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "akc020", outParam);
                    kh_r = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "aac003", outParam);
                    xm_r = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    //outParam = new byte[1024];
                    //getParaByName(whandler, "akc252", outParam);
                    //bcjsqzhye = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "akc260", outParam);
                    tcjjzf = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "akc261", outParam);
                    xjzf = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    //outParam = new byte[1024];
                    //getParaByName(whandler, "akc262", outParam);
                    //zhzf = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "akc264", outParam);
                    ylzje = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "skc093", outParam);
                    bcxdjh = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "skc033", outParam);
                    cxbz_r = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "aae011", outParam);
                    jbr_r = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "aae036", outParam);
                    jbrq = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "skc095", outParam);
                    djdycs = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "skc099", outParam);
                    cfxms = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "aae040", outParam);
                    jsrq = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "skc035", outParam);
                    jsr = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    //outParam = new byte[1024];
                    //getParaByName(whandler, "akc087", outParam);
                    //grzhye = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    decimal zbxje = Convert.ToDecimal(ylzje) - Convert.ToDecimal(xjzf);
                    #endregion
                    sValue = ylzje + zbxje + tcjjzf + "|0.00|" + zhzf + "|" + xjzf + "|0.00|0.00|0.00|0.00|" +
                          "0.00|0.00|0.00|0.00|0.00|0.00|0.00|0.00|0.00|0.00|" +
                          "0.00|0.00|0.00|0.00|0.00|" + bcjsqzhye + "|0.00|0.00|0.00|0.00|" +
                           "|0.00||0.00||0.00|" + jsrq + "|||||" + djh_r + "||" +
                          jsdjh + "|" + cxbz_r + "|||||||||||||||";
                    strSql = string.Format(@"insert into ybfyjsdr( jzlsh,jylsh,jbr,ylfze,tcjjzf,zhzf,xjzf,djh,djhin,grbh,
                                            kh,xm,bcjsqzhye,cxbz1,jbrq,jsrq,jsr,grzfye,cfmxs,ybdjh,sysdate,ybjzlsh) values(
                                            '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}',
                                            '{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}','{20}','{21}')",
                                            jzlsh, jylsh, jbr_r, ylzje, tcjjzf, zhzf, xjzf, djh, bcxdjh, ybbh_r,
                                            kh_r, xm_r, bcjsqzhye, cxbz_r, jbrq, jsrq, jsr, grzhye, cfxms, djh_r, sysdate, zyh_r);

                    liSQL.Add(strSql);

                    object[] obj = liSQL.ToArray();
                    obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);
                    if (obj[1].ToString().Equals("1"))
                    {
                        WriteLog(sysdate + "  住院费用结算成功|" + sValue);
                        //进入出院登记
                        object[] obj2 = { jzlsh };
                        obj2 = YBCYDJ(obj2);
                        if (obj2[1].ToString().Equals("1"))
                        {
                            WriteLog(sysdate + "  住院费用结算成功|出院登记成功|");
                            return new object[] { 0, 1, sValue };
                        }
                        else
                        {
                            WriteLog(sysdate + "  住院费用结算成功|出院登记失败|");
                            //撤销医保登记
                            object[] obj3 = { ybbh_r, djh_r };
                            N_YBZYSFJSCX_GSBX(obj3);
                            return new object[] { 0, 0, "住院费用结算成功|出院登记失败" };
                        }
                    }
                    else
                    {
                        //撤销医保结算
                        object[] obj1 = { ybbh_r, djh_r };
                        N_YBZYSFJSCX_GSBX(obj1);

                        WriteLog(sysdate + "  住院费用结算失败|数据操作失败|" + obj[2].ToString());
                        return new object[] { 0, 1, "住院费用结算失败|数据操作失败|" + obj[2].ToString() };
                    }
                }
                else
                {
                    WriteLog(sysdate + "  获取住院费用结算返回信息失败|" + sMsg1);
                    return new object[] { 0, 0, "获取住院费用结算返回信息失败|" + sMsg1 };
                }
            }
            else
            {
                sMsg = new byte[1024];
                getFailReason(whandler, sMsg);
                sMsg1 = Encoding.Default.GetString(sMsg, 0, sMsg.Length).Replace("\0", "");
                WriteLog(sysdate + "  住院费用结算失败" + sMsg1);
                return new object[] { 0, 0, "住院费用结算失败" + sMsg1 };
            }
        }
        #endregion


        #region 住院费用结算撤销
        public static object[] YBZYSFJSCX(object[] objParam)
        {
            string jzlsh = objParam[0].ToString();  // 就诊流水号 *//对应玉山 住院号
            if (string.IsNullOrEmpty(jzlsh))
                return new object[] { 0, 0, "就诊流水号不能为空" };
            string strSql = string.Format(@"select xzbm from ybmzzydjdr where jzlsh='{0}' and cxbz=1", objParam[0].ToString());
            DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            if (ds == null || ds.Tables[0].Rows.Count == 0)
                return new object[] { 0, 0, "未做医保住院登记信息" };
            string sxzbm = ds.Tables[0].Rows[0]["xzbm"].ToString();
            object[] objReturn = null;
            switch (sxzbm)
            {
                case "310":
                    objReturn = YBZYSFJSCX_YLBX(objParam);
                    break;
                case "410":
                    objReturn = YBZYSFJSCX_GSBX(objParam);
                    break;
                case "510":
                    objReturn = YBZYSFJSCX_SYBX(objParam);
                    break;
                default:
                    objReturn = new object[] { 0, 0, "未找到方法体" };
                    break;
            }
            return objReturn;
        }
        #endregion

        #region 住院费用结算撤销_医疗保险
        public static object[] YBZYSFJSCX_YLBX(object[] objParam)
        {
            string sysdate = GetServerDateTime();//当前时间
            string czygh = CliUtils.fLoginUser;   // 操作员工号 
            string ywzqh = "";   // 业务周期号
            string jzlsh = objParam[0].ToString();   // 就诊流水号
            string jbr = CliUtils.fUserName;     // 经办人姓名
            string djh = objParam[1].ToString();     // 发票号

            //交易流水号
            string jylsh = "";
            bool bfalg = false;
            byte[] sMsg = new byte[1024];
            byte[] outParam = new byte[1024];
            string sMsg1 = string.Empty;

            if (string.IsNullOrEmpty(jzlsh))
                return new object[] { 0, 0, "就诊流水号不能为空" };

            if (string.IsNullOrEmpty(djh))
                return new object[] { 0, 0, "发票号不能为空" };

            string strSql = string.Format(@"select a.* from ybfyjsdr a 
                                            where a.jzlsh = '{0}' and a.djh = '{1}' and a.cxbz = 1   ", jzlsh, djh);
            DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            if (ds.Tables[0].Rows.Count == 0)
                return new object[] { 0, 1, "该患者未进行医保结算或结算已撤销" };

            string ybbh = ds.Tables[0].Rows[0]["grbh"].ToString();  //医保编号
            string ybdjh = ds.Tables[0].Rows[0]["ybdjh"].ToString(); //医保单据号
            string ybjzlsh = ds.Tables[0].Rows[0]["ybjzlsh"].ToString(); //医保就诊流水号

            WriteLog(sysdate + "  进入住院费用结算撤销...");

            //先撤销出院登记
            object[] obj2 = { jzlsh };
            obj2 = YBCYDJCX(obj2);
            if (!obj2[1].ToString().Equals("1"))
                return new object[] { 0,0,"出院登记撤销失败"};

            //入参
            putPara(whandler, "usr", ybuserid);
            putPara(whandler, "pwd", ybpasswd);

            putPara(whandler, "aac001", ybbh);
            putPara(whandler, "aae072", ybdjh);

            bfalg = process(whandler, "F04.06.12.01");
            if (bfalg)
            {
                List<string> liSQL = new List<string>();
                sMsg = new byte[1024];
                getErrMsg(whandler, sMsg);
                sMsg1 = Encoding.Default.GetString(sMsg, 0, sMsg.Length).Replace("\0", "");
                if (sMsg1.ToUpper().Equals("OK"))
                {
                    //出参
                    string bcxdjh = string.Empty; //被冲销单据号
                    string cxbz1 = string.Empty;  //冲销标志（Z 正常；+ 被冲销；- 冲销）
                    string jbr_r = string.Empty;
                    string jbrq = string.Empty;

                    //获取出参数据
                    getParaByName(whandler, "skc093", outParam);
                    bcxdjh = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "skc033", outParam);
                    cxbz1 = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "aae011", outParam);
                    jbr_r = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "aae036", outParam);
                    jbrq = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");

                    strSql = string.Format(@"insert into ybfyjsdr( jzlsh,jylsh,jbr,ylfze,tcjjzf,zhzf,xjzf,djh,djhin,grbh,
                                            kh,xm,bcjsqzhye,cxbz1,jbrq,jsrq,jsr,grzfye,cfmxs,ybdjh,cxbz,sysdate) select 
                                            jzlsh,jylsh,'{5}',ylfze,tcjjzf,zhzf,xjzf,djh,'{3}',grbh,
                                            kh,xm,bcjsqzhye,'{4}','{2}',jsrq,jsr,grzfye,cfmxs,ybdjh,0,'{6}' from ybfyjsdr 
                                            where jzlsh='{0}' and djh='{1}' and cxbz=1 ", jzlsh, djh, jbrq, bcxdjh, cxbz1, jbr_r, sysdate);
                    liSQL.Add(strSql);
                    strSql = string.Format(@"update ybfyjsdr set cxbz=2 where jzlsh='{0}' and djh='{1}' and cxbz=1", jzlsh, djh);
                    liSQL.Add(strSql);
                    strSql = string.Format(@"insert into ybcfmxscindr( jzlsh,jylsh,ybcfh,yysfxmbm,yysfxmmc,sfxmzxbm,ybxmmc,dj,sl,je,
                                            mcyl,sypc,ysxm,ysbm,ksbh,yf,jbr,xm,kh,ybjzlsh,cxbz,sysdate) select
                                            jzlsh,jylsh,ybcfh,yysfxmbm,yysfxmmc,sfxmzxbm,ybxmmc,dj,sl,je,
                                            mcyl,sypc,ysxm,ysbm,ksbh,yf,jbr,xm,kh,ybjzlsh,0,'{1}' from ybcfmxscindr where jzlsh='{0}' and cxbz=1", jzlsh, sysdate);
                    liSQL.Add(strSql);
                    strSql = string.Format(@"update ybcfmxscindr set cxbz=2 where jzlsh='{0}' and cxbz=1", jzlsh);
                    liSQL.Add(strSql);
                    object[] obj = liSQL.ToArray();
                    obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);
                    if (obj[1].ToString().Equals("1"))
                    {
                        WriteLog(sysdate + "   住院费用结算撤销成功|");
                        return new object[] { 0, 1, "住院费用结算撤销成功" };
                    }
                    else
                    {
                        WriteLog(sysdate + "   住院费用结算撤销失败|数据操作失败|" + obj[2].ToString());
                        return new object[] { 0, 0, "住院费用结算撤销失败|数据操作失败|" + obj[2].ToString() };
                    }
                }
                else
                {
                    WriteLog(sysdate + "  获取费用结算撤销返回信息失败|" + sMsg1);
                    return new object[] { 0, 0, "获取费用结算撤销返回信息失败|" + sMsg1 };
                }
            }
            else
            {
                sMsg = new byte[1024];
                getFailReason(whandler, sMsg);
                sMsg1 = Encoding.Default.GetString(sMsg, 0, sMsg.Length).Replace("\0", "");
                WriteLog(sysdate + "  住院费用结算撤销失败|" + sMsg1);
                return new object[] { 0, 0, "住院费用结算撤销失败|" + sMsg1 };
            }

        }
        #endregion

        #region 住院费用结算撤销_生育保险
        public static object[] YBZYSFJSCX_SYBX(object[] objParam)
        {
            string sysdate = GetServerDateTime();//当前时间
            string czygh = CliUtils.fLoginUser;   // 操作员工号 
            string ywzqh = "";   // 业务周期号
            string jzlsh = objParam[0].ToString();   // 就诊流水号
            string jbr = CliUtils.fUserName;     // 经办人姓名
            string djh = objParam[1].ToString();     // 发票号

            //交易流水号
            string jylsh = "";
            bool bfalg = false;
            byte[] sMsg = new byte[1024];
            byte[] outParam = new byte[1024];
            string sMsg1 = string.Empty;

            if (string.IsNullOrEmpty(jzlsh))
                return new object[] { 0, 0, "就诊流水号不能为空" };

            if (string.IsNullOrEmpty(djh))
                return new object[] { 0, 0, "发票号不能为空" };

            string strSql = string.Format(@"select a.* from ybfyjsdr a 
                                            where a.jzlsh = '{0}' and a.djh = '{1}' and a.cxbz = 1   ", jzlsh, djh);
            DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            if (ds.Tables[0].Rows.Count == 0)
                return new object[] { 0, 1, "该患者未进行医保结算或结算已撤销" };

            string ybbh = ds.Tables[0].Rows[0]["grbh"].ToString();  //医保编号
            string ybdjh = ds.Tables[0].Rows[0]["ybdjh"].ToString(); //医保单据号
            string ybjzlsh = ds.Tables[0].Rows[0]["ybjzlsh"].ToString(); //医保就诊流水号

            WriteLog(sysdate + "  进入住院费用结算撤销...");

            //先撤销出院登记
            object[] obj2 = { jzlsh };
            obj2 = YBCYDJCX(obj2);
            if (!obj2[1].ToString().Equals("1"))
                return new object[] { 0, 0, "出院登记撤销失败" };

            //入参
            putPara(whandler, "usr", ybuserid);
            putPara(whandler, "pwd", ybpasswd);

            putPara(whandler, "aac001", ybbh);
            putPara(whandler, "aae072", ybdjh);
            putPara(whandler, "skc113", "S");

            bfalg = process(whandler, "F04.06.12.01");
            if (bfalg)
            {
                List<string> liSQL = new List<string>();
                sMsg = new byte[1024];
                getErrMsg(whandler, sMsg);
                sMsg1 = Encoding.Default.GetString(sMsg, 0, sMsg.Length).Replace("\0", "");
                if (sMsg1.ToUpper().Equals("OK"))
                {
                    //出参
                    string bcxdjh = string.Empty; //被冲销单据号
                    string cxbz1 = string.Empty;  //冲销标志（Z 正常；+ 被冲销；- 冲销）
                    string jbr_r = string.Empty;
                    string jbrq = string.Empty;

                    //获取出参数据
                    getParaByName(whandler, "skc093", outParam);
                    bcxdjh = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "skc033", outParam);
                    cxbz1 = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "aae011", outParam);
                    jbr_r = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "aae036", outParam);
                    jbrq = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");

                    strSql = string.Format(@"insert into ybfyjsdr( jzlsh,jylsh,jbr,ylfze,tcjjzf,zhzf,xjzf,djh,djhin,grbh,
                                            kh,xm,bcjsqzhye,cxbz1,jbrq,jsrq,jsr,grzfye,cfmxs,ybdjh,cxbz,sysdate) select 
                                            jzlsh,jylsh,'{5}',ylfze,tcjjzf,zhzf,xjzf,djh,'{3}',grbh,
                                            kh,xm,bcjsqzhye,'{4}','{2}',jsrq,jsr,grzfye,cfmxs,ybdjh,0,'{6}' from ybfyjsdr 
                                            where jzlsh='{0}' and djh='{1}' and cxbz=1 ", jzlsh, djh, jbrq, bcxdjh, cxbz1, jbr_r, sysdate);
                    liSQL.Add(strSql);
                    strSql = string.Format(@"update ybfyjsdr set cxbz=2 where jzlsh='{0}' and djh='{1}' and cxbz=1", jzlsh, djh);
                    liSQL.Add(strSql);
                    strSql = string.Format(@"insert into ybcfmxscindr( jzlsh,jylsh,ybcfh,yysfxmbm,yysfxmmc,sfxmzxbm,ybxmmc,dj,sl,je,
                                            mcyl,sypc,ysxm,ysbm,ksbh,yf,jbr,xm,kh,ybjzlsh,cxbz,sysdate) select
                                            jzlsh,jylsh,ybcfh,yysfxmbm,yysfxmmc,sfxmzxbm,ybxmmc,dj,sl,je,
                                            mcyl,sypc,ysxm,ysbm,ksbh,yf,jbr,xm,kh,ybjzlsh,0,'{1}' from ybcfmxscindr where jzlsh='{0}' and cxbz=1", jzlsh, sysdate);
                    liSQL.Add(strSql);
                    strSql = string.Format(@"update ybcfmxscindr set cxbz=2 where jzlsh='{0}' and cxbz=1", jzlsh);
                    liSQL.Add(strSql);
                    object[] obj = liSQL.ToArray();
                    obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);
                    if (obj[1].ToString().Equals("1"))
                    {
                        WriteLog(sysdate + "   住院费用结算撤销成功|");
                        return new object[] { 0, 1, "住院费用结算撤销成功" };
                    }
                    else
                    {
                        WriteLog(sysdate + "   住院费用结算撤销失败|数据操作失败|" + obj[2].ToString());
                        return new object[] { 0, 0, "住院费用结算撤销失败|数据操作失败|" + obj[2].ToString() };
                    }
                }
                else
                {
                    WriteLog(sysdate + "  获取费用结算撤销返回信息失败|" + sMsg1);
                    return new object[] { 0, 0, "获取费用结算撤销返回信息失败|" + sMsg1 };
                }
            }
            else
            {
                sMsg = new byte[1024];
                getFailReason(whandler, sMsg);
                sMsg1 = Encoding.Default.GetString(sMsg, 0, sMsg.Length).Replace("\0", "");
                WriteLog(sysdate + "  住院费用结算撤销失败|" + sMsg1);
                return new object[] { 0, 0, "住院费用结算撤销失败|" + sMsg1 };
            }

        }
        #endregion

        #region 住院费用结算撤销_工伤保险
        public static object[] YBZYSFJSCX_GSBX(object[] objParam)
        {
            string sysdate = GetServerDateTime();//当前时间
            string czygh = CliUtils.fLoginUser;   // 操作员工号 
            string ywzqh = "";   // 业务周期号
            string jzlsh = objParam[0].ToString();   // 就诊流水号
            string jbr = CliUtils.fUserName;     // 经办人姓名
            string djh = objParam[1].ToString();     // 发票号

            //交易流水号
            string jylsh = "";
            bool bfalg = false;
            byte[] sMsg = new byte[1024];
            byte[] outParam = new byte[1024];
            string sMsg1 = string.Empty;

            if (string.IsNullOrEmpty(jzlsh))
                return new object[] { 0, 0, "就诊流水号不能为空" };

            if (string.IsNullOrEmpty(djh))
                return new object[] { 0, 0, "发票号不能为空" };

            string strSql = string.Format(@"select a.* from ybfyjsdr a 
                                            where a.jzlsh = '{0}' and a.djh = '{1}' and a.cxbz = 1   ", jzlsh, djh);
            DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            if (ds.Tables[0].Rows.Count == 0)
                return new object[] { 0, 1, "该患者未进行医保结算或结算已撤销" };

            string ybbh = ds.Tables[0].Rows[0]["grbh"].ToString();  //医保编号
            string ybdjh = ds.Tables[0].Rows[0]["ybdjh"].ToString(); //医保单据号
            string ybjzlsh = ds.Tables[0].Rows[0]["ybjzlsh"].ToString(); //医保就诊流水号

            WriteLog(sysdate + "  进入住院费用结算撤销...");

            //先撤销出院登记
            object[] obj2 = { jzlsh };
            obj2 = YBCYDJCX(obj2);
            if (!obj2[1].ToString().Equals("1"))
                return new object[] { 0, 0, "出院登记撤销失败" };

            //入参
            putPara(whandler, "usr", ybuserid);
            putPara(whandler, "pwd", ybpasswd);

            putPara(whandler, "aac001", ybbh);
            putPara(whandler, "aae072", ybdjh);

            bfalg = process(whandler, "F07.11.03.02");
            if (bfalg)
            {
                List<string> liSQL = new List<string>();
                sMsg = new byte[1024];
                getErrMsg(whandler, sMsg);
                sMsg1 = Encoding.Default.GetString(sMsg, 0, sMsg.Length).Replace("\0", "");
                if (sMsg1.ToUpper().Equals("OK"))
                {
                    //出参
                    string bcxdjh = string.Empty; //被冲销单据号
                    string cxbz1 = string.Empty;  //冲销标志（Z 正常；+ 被冲销；- 冲销）
                    string jbr_r = string.Empty;
                    string jbrq = string.Empty;

                    //获取出参数据
                    getParaByName(whandler, "skc093", outParam);
                    bcxdjh = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "skc033", outParam);
                    cxbz1 = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "aae011", outParam);
                    jbr_r = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "aae036", outParam);
                    jbrq = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");

                    strSql = string.Format(@"insert into ybfyjsdr( jzlsh,jylsh,jbr,ylfze,tcjjzf,zhzf,xjzf,djh,djhin,grbh,
                                            kh,xm,bcjsqzhye,cxbz1,jbrq,jsrq,jsr,grzfye,cfmxs,ybdjh,cxbz,sysdate) select 
                                            jzlsh,jylsh,'{5}',ylfze,tcjjzf,zhzf,xjzf,djh,'{3}',grbh,
                                            kh,xm,bcjsqzhye,'{4}','{2}',jsrq,jsr,grzfye,cfmxs,ybdjh,0,'{6}' from ybfyjsdr 
                                            where jzlsh='{0}' and djh='{1}' and cxbz=1 ", jzlsh, djh, jbrq, bcxdjh, cxbz1, jbr_r, sysdate);
                    liSQL.Add(strSql);
                    strSql = string.Format(@"update ybfyjsdr set cxbz=2 where jzlsh='{0}' and djh='{1}' and cxbz=1", jzlsh, djh);
                    liSQL.Add(strSql);
                    strSql = string.Format(@"insert into ybcfmxscindr( jzlsh,jylsh,ybcfh,yysfxmbm,yysfxmmc,sfxmzxbm,ybxmmc,dj,sl,je,
                                            mcyl,sypc,ysxm,ysbm,ksbh,yf,jbr,xm,kh,ybjzlsh,cxbz,sysdate) select
                                            jzlsh,jylsh,ybcfh,yysfxmbm,yysfxmmc,sfxmzxbm,ybxmmc,dj,sl,je,
                                            mcyl,sypc,ysxm,ysbm,ksbh,yf,jbr,xm,kh,ybjzlsh,0,'{1}' from ybcfmxscindr where jzlsh='{0}' and cxbz=1", jzlsh, sysdate);
                    liSQL.Add(strSql);
                    strSql = string.Format(@"update ybcfmxscindr set cxbz=2 where jzlsh='{0}' and cxbz=1", jzlsh);
                    liSQL.Add(strSql);
                    object[] obj = liSQL.ToArray();
                    obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);
                    if (obj[1].ToString().Equals("1"))
                    {
                        WriteLog(sysdate + "   住院费用结算撤销成功|");
                        return new object[] { 0, 1, "住院费用结算撤销成功" };
                    }
                    else
                    {
                        WriteLog(sysdate + "   住院费用结算撤销失败|数据操作失败|" + obj[2].ToString());
                        return new object[] { 0, 0, "住院费用结算撤销失败|数据操作失败|" + obj[2].ToString() };
                    }
                }
                else
                {
                    WriteLog(sysdate + "  获取费用结算撤销返回信息失败|" + sMsg1);
                    return new object[] { 0, 0, "获取费用结算撤销返回信息失败|" + sMsg1 };
                }
            }
            else
            {
                sMsg = new byte[1024];
                getFailReason(whandler, sMsg);
                sMsg1 = Encoding.Default.GetString(sMsg, 0, sMsg.Length).Replace("\0", "");
                WriteLog(sysdate + "  住院费用结算撤销失败|" + sMsg1);
                return new object[] { 0, 0, "住院费用结算撤销失败|" + sMsg1 };
            }

        }
        #endregion


        #region  出院登记
        public static object[] YBCYDJ(object[] objParam)
        {
            string jzlsh = objParam[0].ToString();  // 就诊流水号 *//对应玉山 住院号
            if (string.IsNullOrEmpty(jzlsh))
                return new object[] { 0, 0, "就诊流水号不能为空" };
            string strSql = string.Format(@"select xzbm from ybmzzydjdr where jzlsh='{0}' and cxbz=1", objParam[0].ToString());
            DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            if (ds == null || ds.Tables[0].Rows.Count == 0)
                return new object[] { 0, 0, "未做医保住院登记信息" };
            string sxzbm = ds.Tables[0].Rows[0]["xzbm"].ToString();
            object[] objReturn = null;
            switch (sxzbm)
            {
                case "310":
                    objReturn = YBCYDJ_YLBX(objParam);
                    break;
                case "410":
                    objReturn = YBCYDJ_GSBX(objParam);
                    break;
                case "510":
                    objReturn = YBCYDJ_SYBX(objParam);
                    break;
                default:
                    objReturn = new object[] { 0, 0, "未找到方法体" };
                    break;
            }
            return objReturn;
        }
        #endregion

        #region  出院登记_医疗保险
        public static object[] YBCYDJ_YLBX(object[] objParam)
        {
            string sysdate = GetServerDateTime();
            string jzlsh = objParam[0].ToString(); //就诊流水号

            string ybbh = string.Empty;     //医保编号  <非空>
            string ybzyh = string.Empty;    //住院号  <非空>
            string cyrq = string.Empty;       //出院日期
            string cylxdm = string.Empty;   //出院类型
            string cyzddm1 = string.Empty;  //出院诊断疾病编码（主）  <非空>
            string cyzdmc1 = string.Empty;  //出院诊断疾病中文名称（主）  <非空>
            string cyzddm2 = string.Empty;  //出院诊断疾病编码（次）
            string cyzdmc2 = string.Empty;  //出院诊断疾病中文名称（次）
            string cyzddm3 = string.Empty;  //出院诊断疾病编码（第三）
            string cyzdmc3 = string.Empty;  //出院诊断疾病中文名称（第三）
            string rylxdm = string.Empty;   //入院类型 

            bool bfalg = false;
            byte[] sMsg = new byte[1024];
            byte[] outParam = new byte[1024];
            string sMsg1 = string.Empty;


            string strSql = string.Format(@"select grbh,ybjzlsh,cyrq,cylxdm,cyzdjbbm1,cyzdjbmc1,cyzdjbbm2,cyzdjbmc2,cyzdjbbm3,cyzdjbmc3,rylxdm from ybmzzydjdr where jzlsh='{0}' and cxbz=1", jzlsh);
            DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            if (ds.Tables[0].Rows.Count == 0)
            {
                WriteLog(sysdate + "  获取出院登记信息失败");
                return new object[] { 0, 0, "获取出院登记信息失败" };
            }
            ybbh = ds.Tables[0].Rows[0]["grbh"].ToString();
            ybzyh = ds.Tables[0].Rows[0]["ybjzlsh"].ToString();
            cyrq = ds.Tables[0].Rows[0]["cyrq"].ToString();
            cylxdm = ds.Tables[0].Rows[0]["cylxdm"].ToString();
            cyzddm1 = ds.Tables[0].Rows[0]["cyzdjbbm1"].ToString();
            cyzdmc1 = ds.Tables[0].Rows[0]["cyzdjbmc1"].ToString();
            cyzddm2 = ds.Tables[0].Rows[0]["cyzdjbbm2"].ToString();
            cyzdmc2 = ds.Tables[0].Rows[0]["cyzdjbmc2"].ToString();
            cyzddm3 = ds.Tables[0].Rows[0]["cyzdjbbm3"].ToString();
            cyzdmc3 = ds.Tables[0].Rows[0]["cyzdjbmc3"].ToString();
            rylxdm = ds.Tables[0].Rows[0]["rylxdm"].ToString();

            WriteLog(sysdate + "  进入医保出院登记...");
            //入参
            putPara(whandler, "usr", ybuserid);
            putPara(whandler, "pwd", ybpasswd);

            putPara(whandler, "aac001", ybbh);
            putPara(whandler, "akc190", ybzyh);
            putPara(whandler, "akc194", cyrq);
            putPara(whandler, "skc622", cylxdm);
            putPara(whandler, "akc196", cyzddm1);
            putPara(whandler, "skc040", cyzdmc1);
            putPara(whandler, "skc041", cyzddm2);
            putPara(whandler, "skc619", cyzdmc2);
            putPara(whandler, "skc620", cyzddm3);
            putPara(whandler, "skc621", cyzdmc3);
            putPara(whandler, "skc515 ", rylxdm);

            bfalg = process(whandler, "F04.06.20.01");
            if (bfalg)
            {
                getErrMsg(whandler, sMsg);
                sMsg1 = Encoding.Default.GetString(sMsg, 0, sMsg.Length).Replace("\0", "");
                if (sMsg1.ToUpper().Equals("OK"))
                {
                    WriteLog(sysdate + "  医保出院登记成功");
                    return new object[] { 0, 1, "医保出院登记成功" };
                }
                else
                {
                    WriteLog(sysdate + "  获取医保出院登记返回信息失败");
                    return new object[] { 0, 0, "获取医保出院登记返回信息失败" };
                }
            }
            else
            {
                getFailReason(whandler, sMsg);
                sMsg1 = Encoding.Default.GetString(sMsg, 0, sMsg.Length).Replace("\0", "");
                WriteLog(sysdate + "  医保出院登记失败|" + sMsg1);
                return new object[] { 0, 0, "医保出院登记失败|" + sMsg1 };
            }
        }
        #endregion

        #region  出院登记_生育保险
        public static object[] YBCYDJ_SYBX(object[] objParam)
        {
            string sysdate = GetServerDateTime();
            string jzlsh = objParam[0].ToString(); //就诊流水号

            string ybbh = string.Empty;     //医保编号  <非空>
            string ybzyh = string.Empty;    //住院号  <非空>
            string cyrq = string.Empty;       //出院日期
            string cylxdm = string.Empty;   //出院类型
            string cyzddm1 = string.Empty;  //出院诊断疾病编码（主）  <非空>
            string cyzdmc1 = string.Empty;  //出院诊断疾病中文名称（主）  <非空>
            string cyzddm2 = string.Empty;  //出院诊断疾病编码（次）
            string cyzdmc2 = string.Empty;  //出院诊断疾病中文名称（次）
            string cyzddm3 = string.Empty;  //出院诊断疾病编码（第三）
            string cyzdmc3 = string.Empty;  //出院诊断疾病中文名称（第三）
            string rylxdm = string.Empty;   //入院类型 
            string fmsj = string.Empty;  //分娩时间

            bool bfalg = false;
            byte[] sMsg = new byte[1024];
            byte[] outParam = new byte[1024];
            string sMsg1 = string.Empty;


            string strSql = string.Format(@"select grbh,ybjzlsh,cyrq,cylxdm,cyzdjbbm1,cyzdjbmc1,cyzdjbbm2,cyzdjbmc2,cyzdjbbm3,cyzdjbmc3,rylxdm from ybmzzydjdr where jzlsh='{0}' and cxbz=1", jzlsh);
            DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            if (ds.Tables[0].Rows.Count == 0)
            {
                WriteLog(sysdate + "  获取出院登记信息失败");
                return new object[] { 0, 0, "获取出院登记信息失败" };
            }
            ybbh = ds.Tables[0].Rows[0]["grbh"].ToString();
            ybzyh = ds.Tables[0].Rows[0]["ybjzlsh"].ToString();
            cyrq = ds.Tables[0].Rows[0]["cyrq"].ToString();
            cylxdm = ds.Tables[0].Rows[0]["cylxdm"].ToString();
            cyzddm1 = ds.Tables[0].Rows[0]["cyzdjbbm1"].ToString();
            cyzdmc1 = ds.Tables[0].Rows[0]["cyzdjbmc1"].ToString();
            cyzddm2 = ds.Tables[0].Rows[0]["cyzdjbbm2"].ToString();
            cyzdmc2 = ds.Tables[0].Rows[0]["cyzdjbmc2"].ToString();
            cyzddm3 = ds.Tables[0].Rows[0]["cyzdjbbm3"].ToString();
            cyzdmc3 = ds.Tables[0].Rows[0]["cyzdjbmc3"].ToString();
            rylxdm = ds.Tables[0].Rows[0]["rylxdm"].ToString();
            fmsj = ds.Tables[0].Rows[0]["ghdjsj"].ToString();

            WriteLog(sysdate + "  进入医保出院登记...");
            //入参
            putPara(whandler, "usr", ybuserid);
            putPara(whandler, "pwd", ybpasswd);

            putPara(whandler, "aac001", ybbh);
            putPara(whandler, "akc190", ybzyh);
            putPara(whandler, "akc194", cyrq);
            putPara(whandler, "akc196", cyzddm1);
            putPara(whandler, "skc040", cyzdmc1);
            putPara(whandler, "skc041", cyzddm2);
            putPara(whandler, "skc515 ", rylxdm);
            putPara(whandler, "skc113 ", "S");
            putPara(whandler, "amc020 ", fmsj);

            bfalg = process(whandler, "F04.06.20.01");
            if (bfalg)
            {
                getErrMsg(whandler, sMsg);
                sMsg1 = Encoding.Default.GetString(sMsg, 0, sMsg.Length).Replace("\0", "");
                if (sMsg1.ToUpper().Equals("OK"))
                {
                    WriteLog(sysdate + "  医保出院登记成功");
                    return new object[] { 0, 1, "医保出院登记成功" };
                }
                else
                {
                    WriteLog(sysdate + "  获取医保出院登记返回信息失败");
                    return new object[] { 0, 0, "获取医保出院登记返回信息失败" };
                }
            }
            else
            {
                getFailReason(whandler, sMsg);
                sMsg1 = Encoding.Default.GetString(sMsg, 0, sMsg.Length).Replace("\0", "");
                WriteLog(sysdate + "  医保出院登记失败|" + sMsg1);
                return new object[] { 0, 0, "医保出院登记失败|" + sMsg1 };
            }
        }
        #endregion

        #region  出院登记_工伤保险
        public static object[] YBCYDJ_GSBX(object[] objParam)
        {
            string sysdate = GetServerDateTime();
            string jzlsh = objParam[0].ToString(); //就诊流水号

            string ybbh = string.Empty;     //医保编号  <非空>
            string ybzyh = string.Empty;    //住院号  <非空>
            string cyrq = string.Empty;       //出院日期
            string cylxdm = string.Empty;   //出院类型
            string cyzddm1 = string.Empty;  //出院诊断疾病编码（主）  <非空>
            string cyzdmc1 = string.Empty;  //出院诊断疾病中文名称（主）  <非空>
            string cyzddm2 = string.Empty;  //出院诊断疾病编码（次）
            string cyzdmc2 = string.Empty;  //出院诊断疾病中文名称（次）
            string cyzddm3 = string.Empty;  //出院诊断疾病编码（第三）
            string cyzdmc3 = string.Empty;  //出院诊断疾病中文名称（第三）
            string rylxdm = string.Empty;   //入院类型 
            string fmsj = string.Empty;  //分娩时间

            bool bfalg = false;
            byte[] sMsg = new byte[1024];
            byte[] outParam = new byte[1024];
            string sMsg1 = string.Empty;


            string strSql = string.Format(@"select grbh,ybjzlsh,cyrq,cylxdm,cyzdjbbm1,cyzdjbmc1,cyzdjbbm2,cyzdjbmc2,cyzdjbbm3,cyzdjbmc3,rylxdm from ybmzzydjdr where jzlsh='{0}' and cxbz=1", jzlsh);
            DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            if (ds.Tables[0].Rows.Count == 0)
            {
                WriteLog(sysdate + "  获取出院登记信息失败");
                return new object[] { 0, 0, "获取出院登记信息失败" };
            }
            ybbh = ds.Tables[0].Rows[0]["grbh"].ToString();
            ybzyh = ds.Tables[0].Rows[0]["ybjzlsh"].ToString();
            cyrq = ds.Tables[0].Rows[0]["cyrq"].ToString();
            cylxdm = ds.Tables[0].Rows[0]["cylxdm"].ToString();
            cyzddm1 = ds.Tables[0].Rows[0]["cyzdjbbm1"].ToString();
            cyzdmc1 = ds.Tables[0].Rows[0]["cyzdjbmc1"].ToString();
            cyzddm2 = ds.Tables[0].Rows[0]["cyzdjbbm2"].ToString();
            cyzdmc2 = ds.Tables[0].Rows[0]["cyzdjbmc2"].ToString();
            cyzddm3 = ds.Tables[0].Rows[0]["cyzdjbbm3"].ToString();
            cyzdmc3 = ds.Tables[0].Rows[0]["cyzdjbmc3"].ToString();
            rylxdm = ds.Tables[0].Rows[0]["rylxdm"].ToString();
            fmsj = ds.Tables[0].Rows[0]["ghdjsj"].ToString();

            WriteLog(sysdate + "  进入医保出院登记...");
            //入参
            putPara(whandler, "usr", ybuserid);
            putPara(whandler, "pwd", ybpasswd);

            putPara(whandler, "aac001", ybbh);
            putPara(whandler, "akc190", ybzyh);
            putPara(whandler, "akc194", cyrq);
            putPara(whandler, "akc196", cyzddm1);
            putPara(whandler, "skc040", cyzdmc1);
            putPara(whandler, "skc041", cyzddm2);

            bfalg = process(whandler, "F07.11.04.01");
            if (bfalg)
            {
                getErrMsg(whandler, sMsg);
                sMsg1 = Encoding.Default.GetString(sMsg, 0, sMsg.Length).Replace("\0", "");
                if (sMsg1.ToUpper().Equals("OK"))
                {
                    WriteLog(sysdate + "  医保出院登记成功");
                    return new object[] { 0, 1, "医保出院登记成功" };
                }
                else
                {
                    WriteLog(sysdate + "  获取医保出院登记返回信息失败");
                    return new object[] { 0, 0, "获取医保出院登记返回信息失败" };
                }
            }
            else
            {
                getFailReason(whandler, sMsg);
                sMsg1 = Encoding.Default.GetString(sMsg, 0, sMsg.Length).Replace("\0", "");
                WriteLog(sysdate + "  医保出院登记失败|" + sMsg1);
                return new object[] { 0, 0, "医保出院登记失败|" + sMsg1 };
            }
        }
        #endregion


        #region  出院登记撤销
        public static object[] YBCYDJCX(object[] objParam)
        {
            string jzlsh = objParam[0].ToString();  // 就诊流水号 *//对应玉山 住院号
            if (string.IsNullOrEmpty(jzlsh))
                return new object[] { 0, 0, "就诊流水号不能为空" };
            string strSql = string.Format(@"select xzbm from ybmzzydjdr where jzlsh='{0}' and cxbz=1", objParam[0].ToString());
            DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            if (ds == null || ds.Tables[0].Rows.Count == 0)
                return new object[] { 0, 0, "未做医保住院登记信息" };
            string sxzbm = ds.Tables[0].Rows[0]["xzbm"].ToString();
            object[] objReturn = null;
            switch (sxzbm)
            {
                case "310":
                    objReturn = YBCYDJCX_YLBX(objParam);
                    break;
                case "410":
                    objReturn = YBCYDJCX_GSBX(objParam);
                    break;
                case "510":
                    objReturn = YBCYDJCX_SYBX(objParam);
                    break;
                default:
                    objReturn = new object[] { 0, 0, "未找到方法体" };
                    break;
            }
            return objReturn;
        }
        #endregion

        #region  出院登记撤销_医疗保险
        public static object[] YBCYDJCX_YLBX(object[] objParam)
        {
            string sysdate = GetServerDateTime();
            string ybbh = objParam[0].ToString();
            string ybzyh = objParam[1].ToString();

            bool bfalg = false;
            byte[] sMsg = new byte[1024];
            string sMsg1 = string.Empty;

            WriteLog(sysdate + "  进入出院登记撤销...");
            //入参
            putPara(whandler, "usr", ybuserid);
            putPara(whandler, "pwd", ybpasswd);

            putPara(whandler, "aac001", ybbh);
            putPara(whandler, "akc190", ybzyh);

            WriteLog("入参|" + ybuserid + "|" + ybpasswd + "|" + ybbh + "|" + ybzyh);

            bfalg = process(whandler, "F04.06.21.01");
            if (bfalg)
            {
                getErrMsg(whandler, sMsg);
                sMsg1 = Encoding.Default.GetString(sMsg, 0, sMsg.Length).Replace("\0", "");
                WriteLog(sysdate + "出院登记撤销成功|" + sMsg1);
                return new object[] { 0, 0, "出院登记撤销成功|" + sMsg1 };
            }
            else
            {
                getFailReason(whandler, sMsg);
                sMsg1 = Encoding.Default.GetString(sMsg, 0, sMsg.Length).Replace("\0", "");
                WriteLog(sysdate + "   出院登记撤销失败|" + sMsg1);
                return new object[] { 0, 0, "出院登记撤销失败|" + sMsg1 };
            }
        }
        #endregion

        #region  出院登记撤销_生育保险
        public static object[] YBCYDJCX_SYBX(object[] objParam)
        {
            string sysdate = GetServerDateTime();
            string ybbh = objParam[0].ToString();
            string ybzyh = objParam[1].ToString();

            bool bfalg = false;
            byte[] sMsg = new byte[1024];
            string sMsg1 = string.Empty;

            WriteLog(sysdate + "  进入出院登记撤销...");
            //入参
            putPara(whandler, "usr", ybuserid);
            putPara(whandler, "pwd", ybpasswd);

            putPara(whandler, "aac001", ybbh);
            putPara(whandler, "akc190", ybzyh);
            putPara(whandler, "skc113", "S");

            WriteLog("入参|" + ybuserid + "|" + ybpasswd + "|" + ybbh + "|" + ybzyh);

            bfalg = process(whandler, "F04.06.21.01");
            if (bfalg)
            {
                getErrMsg(whandler, sMsg);
                sMsg1 = Encoding.Default.GetString(sMsg, 0, sMsg.Length).Replace("\0", "");
                WriteLog(sysdate + "出院登记撤销成功|" + sMsg1);
                return new object[] { 0, 0, "出院登记撤销成功|" + sMsg1 };
            }
            else
            {
                getFailReason(whandler, sMsg);
                sMsg1 = Encoding.Default.GetString(sMsg, 0, sMsg.Length).Replace("\0", "");
                WriteLog(sysdate + "   出院登记撤销失败|" + sMsg1);
                return new object[] { 0, 0, "出院登记撤销失败|" + sMsg1 };
            }
        }
        #endregion

        #region  出院登记撤销_工伤保险
        public static object[] YBCYDJCX_GSBX(object[] objParam)
        {
            string sysdate = GetServerDateTime();
            string ybbh = objParam[0].ToString();
            string ybzyh = objParam[1].ToString();

            bool bfalg = false;
            byte[] sMsg = new byte[1024];
            string sMsg1 = string.Empty;

            WriteLog(sysdate + "  进入出院登记撤销...");
            //入参
            putPara(whandler, "usr", ybuserid);
            putPara(whandler, "pwd", ybpasswd);

            putPara(whandler, "aac001", ybbh);
            putPara(whandler, "akc190", ybzyh);

            WriteLog("入参|" + ybuserid + "|" + ybpasswd + "|" + ybbh + "|" + ybzyh);

            bfalg = process(whandler, "F07.11.04.02");
            if (bfalg)
            {
                getErrMsg(whandler, sMsg);
                sMsg1 = Encoding.Default.GetString(sMsg, 0, sMsg.Length).Replace("\0", "");
                WriteLog(sysdate + "出院登记撤销成功|" + sMsg1);
                return new object[] { 0, 0, "出院登记撤销成功|" + sMsg1 };
            }
            else
            {
                getFailReason(whandler, sMsg);
                sMsg1 = Encoding.Default.GetString(sMsg, 0, sMsg.Length).Replace("\0", "");
                WriteLog(sysdate + "   出院登记撤销失败|" + sMsg1);
                return new object[] { 0, 0, "出院登记撤销失败|" + sMsg1 };
            }
        }
        #endregion


        #region 医保目录查询
        public static object[] YBMRCX(object[] objParam)
        {
            string sysdate=GetServerDateTime();
            string cxlx = "000";
            string cxkssj = "20110101";
            string cxjssj = DateTime.Now.ToString("yyyyMMdd");
            string first = "first";
            //string offset="1";
            bool bfalg = false;
            byte[] sMsg = new byte[1024];
            byte[] outParam = new byte[1024];
            string sMsg1 = string.Empty;
            
            WriteLog(sysdate+"  进行医保目录查询...");
            
            //入参
            putPara(whandler, "usr", ybuserid);
            putPara(whandler, "pwd", ybpasswd);

            putPara(whandler, "skc583", cxlx);
            putPara(whandler, "aae030", cxkssj);
            putPara(whandler, "aae031", cxjssj);
            putPara(whandler, "first", first);

            List<string> liSQL = new List<string>();
            
            bfalg = process(whandler, "F04.11.01.01");
            if (bfalg)
            {
                //保存数据前，先删除之煎数据
                string strSql = string.Format("truncate table yb_ypmr_hf");
                liSQL.Add(strSql);
                #region 出参
                int Totalcount = 0; //一共要返回多少条
                int Rowcount = 0;   //本次返回了多少条
                string ypbm = string.Empty; //药品编码(项目编号)
                string ypmc = string.Empty; //中文名称
                string ypmc1 = string.Empty;//英文名称
                string sflb = string.Empty; //收费类别（发票项目）
                string cfybz = string.Empty;    //处方药标志
                string sfxmdj = string.Empty;   //收费项目等级
                string zjm = string.Empty;      //助记码
                string dw = string.Empty;   //单位
                string bzjg = string.Empty; //标准价格
                string jx = string.Empty;   //剂型
                string mcyl = string.Empty; //每次用量
                string sypc = string.Empty; //使用频次
                string fy = string.Empty;   //用法
                string gg = string.Empty;   //规格
                string yfybxm = string.Empty;   //是否医保项目
                string yplb = string.Empty; //药品类别
                string jhjd = string.Empty; //进货渠道
                string pybm = string.Empty; //拼音编码
                string wbbm = string.Empty; //五笔编码
                string bbh = string.Empty;  //版本号
                string ybmrjxsj = string.Empty; //医保目录更新时间
                #endregion

                getParaByName(whandler, "totalcount", outParam);
                Totalcount = int.Parse(Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", ""));
                outParam = new byte[1024];
                getParaByName(whandler, "rowcount", outParam);
                Rowcount = int.Parse(Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", ""));

                while (toResultSetName(whandler, "resultset") > 0)
                {
                    for (int i = 0; i < Rowcount; i++)
                    {
                        #region 获取出参信息
                        outParam = new byte[1024];
                        getColData(whandler, "aka060", outParam);
                        ypbm = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                        outParam = new byte[1024];
                        getColData(whandler, "aka061", outParam);
                        ypmc = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                        outParam = new byte[1024];
                        getColData(whandler, "aka062", outParam);
                        ypmc1 = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                        outParam = new byte[1024];
                        getColData(whandler, "aka063", outParam);
                        sflb = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                        outParam = new byte[1024];
                        getColData(whandler, "aka064", outParam);
                        cfybz = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                        outParam = new byte[1024];
                        getColData(whandler, "aka065", outParam);
                        sfxmdj = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                        outParam = new byte[1024];
                        getColData(whandler, "aka066", outParam);
                        zjm = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                        outParam = new byte[1024];
                        getColData(whandler, "aka067", outParam);
                        dw = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                        outParam = new byte[1024];
                        getColData(whandler, "aka068", outParam);
                        bzjg = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                        outParam = new byte[1024];
                        getColData(whandler, "aka070", outParam);
                        jx = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                        outParam = new byte[1024];
                        getColData(whandler, "aka071", outParam);
                        mcyl = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                        outParam = new byte[1024];
                        getColData(whandler, "aka072", outParam);
                        sypc = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                        outParam = new byte[1024];
                        getColData(whandler, "aka073", outParam);
                        fy = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                        outParam = new byte[1024];
                        getColData(whandler, "aka074", outParam);
                        gg = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                        outParam = new byte[1024];
                        getColData(whandler, "ska003", outParam);
                        yfybxm = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                        outParam = new byte[1024];
                        getColData(whandler, "ska001", outParam);
                        yplb = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                        outParam = new byte[1024];
                        getColData(whandler, "ska004", outParam);
                        jhjd = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                        outParam = new byte[1024];
                        getColData(whandler, "sae040", outParam);
                        pybm = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                        outParam = new byte[1024];
                        getColData(whandler, "sae041", outParam);
                        wbbm = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                        outParam = new byte[1024];
                        getColData(whandler, "sae009", outParam);
                        bbh = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                        outParam = new byte[1024];
                        getParaByName(whandler, "aae036", outParam);
                        ybmrjxsj = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                        strSql = string.Format(@"insert into yb_ypmr_hf (ypbm,ypmc,ypmc1,sflb,cfybz,sfxmdj,zjm,dw,bzjg,jx,
                                            mcyl,sypc,fy,gg,yfybxm,yplb,jhjd,pybm,wbbm,bbh,ybmrjxsj)values(
                                            '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}',
                                            '{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}','{20}')",
                                                ypbm, ypmc, ypmc1, sflb, cfybz, sfxmdj, zjm, dw, bzjg, jx,
                                                mcyl, sypc, fy, gg, yfybxm, yplb, jhjd, pybm, wbbm, bbh, ybmrjxsj);
                        liSQL.Add(strSql);
                        #endregion
                    }

                    #region 进入下一次获取
                    putPara(whandler, "usr", ybuserid);
                    putPara(whandler, "pwd", ybpasswd);

                    putPara(whandler, "skc583", cxlx);
                    putPara(whandler, "aae030", cxkssj);
                    putPara(whandler, "aae031", cxjssj);
                    putPara(whandler, "first", "");
                    bfalg = process(whandler, "F04.11.01.01");
                    if (!bfalg)
                    {
                        sMsg = new byte[1024];
                        getFailReason(whandler, sMsg);
                        Encoding.Default.GetString(sMsg, 0, sMsg.Length).Replace("\0", "");
                        WriteLog(sysdate + "  医保目录查询失败|" + sMsg1);
                        return new object[] { 0, 0, "医保目录查询失败|" + sMsg1 };
                    }
                    #endregion
                }

                object[] obj = liSQL.ToArray();
                obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);
                if (obj[1].ToString().Equals("1"))
                {
                    WriteLog(sysdate + "  医保目录查询成功");
                    return new object[] { 0, 1, "医保目录查询成功" };
                }
                else
                {
                    WriteLog(sysdate + "  医保目录查询失败");
                    return new object[] { 0, 0, "医保目录查询失败|" + obj[2] };
                }
            }
            else
            {
                getFailReason(whandler, sMsg);
                sMsg1 = Encoding.Default.GetString(sMsg, 0, sMsg.Length).Replace("\0", "");
                WriteLog(sysdate + "  医保目录查询失败|" + sMsg1);
                return new object[] { 0, 0, "医保目录查询失败|" + sMsg1 };
            }
             
        }
        #endregion

        #region 医保病种编码查询
        public static object[] YBBZBMCX(object[] objParam)
        {
            string sysdate = GetServerDateTime();
            string cxlx = "000";
            string cxkssj = "20110101";
            string cxjssj = DateTime.Now.ToString("yyyyMMdd");
            string first = "first";

            bool bfalg = false;
            byte[] sMsg = new byte[1024];
            byte[] outParam = new byte[1024];
            string sMsg1 = string.Empty;
            object[] obj = null;

            WriteLog(sysdate+"  进入医保病种编码查询...");

            //入参
            putPara(whandler, "usr", ybuserid);
            putPara(whandler, "pwd", ybpasswd);

            putPara(whandler, "skc583", cxlx);
            putPara(whandler, "aae030", cxkssj);
            putPara(whandler, "aae031", cxjssj);
            putPara(whandler, "first", first);

            List<string> liSQL = new List<string>();

            bfalg = process(whandler, "F04.11.01.04");
            if (bfalg)
            {
                string strSql = string.Format(@"truncate table yb_bzmr_hf");
                liSQL.Add(strSql);

                #region 出参
                int Totalcount = 0; //一共要返回多少条
                int Rowcount = 0;   //本次返回了多少条
                string bzbm = string.Empty; //病种编码
                string bzmc = string.Empty; //病种名称
                string bzlb = string.Empty; //病种分类
                string pybm = string.Empty; //拼音编码
                string wbbm = string.Empty; //五笔编码
                string bbh = string.Empty;  //版本号
                string jxsj = string.Empty; //更新时间
                #endregion

                getParaByName(whandler, "totalcount", outParam);
                Totalcount = int.Parse(Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", ""));
                outParam = new byte[1024];
                getParaByName(whandler, "rowcount", outParam);
                Rowcount = int.Parse(Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", ""));

                while (toResultSetName(whandler, "resultset") > 0)
                {
                    for (int i = 0; i < Rowcount; i++)
                    {
                        
                        #region 获取出参数据
                        outParam = new byte[1024];
                        getColData(whandler, "aka120", outParam);
                        bzbm = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                        outParam = new byte[1024];
                        getColData(whandler, "aka121", outParam);
                        bzmc = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                        outParam = new byte[1024];
                        getColData(whandler, "aka122", outParam);
                        bzlb = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                        outParam = new byte[1024];
                        getColData(whandler, "sae040", outParam);
                        pybm = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                        outParam = new byte[1024];
                        getColData(whandler, "sae041", outParam);
                        wbbm = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                        outParam = new byte[1024];
                        getColData(whandler, "sae009", outParam);
                        bbh = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                        outParam = new byte[1024];
                        getColData(whandler, "aae036", outParam);
                        jxsj = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                        strSql = string.Format(@"insert into YB_BZMR_HF( bzbm,bzmc,bzlb,pybm,wbbm,bbh,jxsj) values(
                                            '{0}','{1}','{2}','{3}','{4}','{5}','{6}')", bzbm, bzmc, bzlb, pybm, wbbm, bbh, jxsj);
                        liSQL.Add(strSql);
                        nextRow(whandler);
                        #endregion
                    }

                    #region 进入下一次获取
                    putPara(whandler, "usr", ybuserid);
                    putPara(whandler, "pwd", ybpasswd);

                    putPara(whandler, "skc583", cxlx);
                    putPara(whandler, "aae030", cxkssj);
                    putPara(whandler, "aae031", cxjssj);
                    putPara(whandler, "first", "");
                    bfalg = process(whandler, "F04.11.01.04");
                    if (!bfalg)
                    {
                        sMsg = new byte[1024];
                        getFailReason(whandler, sMsg);
                        Encoding.Default.GetString(sMsg, 0, sMsg.Length).Replace("\0", "");
                        WriteLog(sysdate + "  医保目录查询失败|" + sMsg1);
                        return new object[] { 0, 0, "医保目录查询失败|" + sMsg1 };
                    }
                    #endregion

                    obj = liSQL.ToArray();
                    liSQL.Clear();
                    obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);
                    //if (obj[1].ToString().Equals("1"))
                    //{
                    //    WriteLog(sysdate + "  医保病种编码查询成功");
                    //    return new object[] { 0, 1, "医保病种编码查询成功" };
                    //}
                    //else
                    //{
                    //    WriteLog(sysdate + "  医保病种编码查询失败");
                    //    return new object[] { 0, 0, "医保病种编码查询失败|" + obj[2] };
                    //}
                }
                return obj;
               
            }
            else
            {
                getFailReason(whandler, sMsg);
                sMsg1 = Encoding.Default.GetString(sMsg, 0, sMsg.Length).Replace("\0", "");
                WriteLog(sysdate + "  医保病种编码查询失败|" + sMsg1);
                return new object[] { 0, 0, "医保病种编码查询失败|" + sMsg1 };

            }
        }
        #endregion

        #region 医疗待遇封锁信息查询
        public static object[] YBYLDYFSXXCX(object[] objParam)
        {
            return new object[] { 0, 1, "住院即时结算开通" };
        }
        #endregion


        #region 门诊登记撤销(内部)_医疗/生育/工伤
        public static object[] N_YBMZDJCX(object[] objParam)
        {
            string sysdate = GetServerDateTime();
            string ybbh = objParam[0].ToString(); //医保编号
            string mzh = objParam[1].ToString();    //门诊号
            string xzbm = objParam[2].ToString(); //险种编码
            bool bfalg = false;
            byte[] sMsg = new byte[1024];
            string sMsg1 = string.Empty;

            WriteLog(sysdate + "  进入门诊登记撤销(内部)...");
            if (xzbm.Equals("310"))
            {
                //入参
                putPara(whandler, "usr", ybuserid);
                putPara(whandler, "pwd", ybpasswd);
                putPara(whandler, "aac001", ybbh);
                putPara(whandler, "akc190", mzh);
                WriteLog("入参|" + ybuserid + "|" + ybpasswd + "|" + ybbh + "|" + mzh);
                bfalg = process(whandler, "F04.05.03.01");
            }
             if (xzbm.Equals("510"))
            {
                //入参
                putPara(whandler, "usr", ybuserid);
                putPara(whandler, "pwd", ybpasswd);
                putPara(whandler, "aac001", ybbh);
                putPara(whandler, "akc190", mzh);
                putPara(whandler, "skc113", "S");
                WriteLog("入参|" + ybuserid + "|" + ybpasswd + "|" + ybbh + "|" + mzh + "|S");
                bfalg = process(whandler, "F04.05.03.01");
            }
            if (xzbm.Equals("410"))
            {
                //入参
                putPara(whandler, "usr", ybuserid);
                putPara(whandler, "pwd", ybpasswd);
                putPara(whandler, "aac001", ybbh);
                putPara(whandler, "akc190", mzh);
                WriteLog("入参|" + ybuserid + "|" + ybpasswd + "|" + ybbh + "|" + mzh );
                bfalg = process(whandler, "F07.11.05.02");
 
            }

            if (bfalg)
            {
                WriteLog(sysdate + "  医保门诊登记撤销成功");
                return new object[] { 0, 0, "医保门诊登记撤销成功" };
            }
            else
            {
                getFailReason(whandler, sMsg);
                sMsg1 = Encoding.Default.GetString(sMsg, 0, sMsg.Length).Replace("\0", "");
                WriteLog(sysdate + "  医保门诊登记撤销失败" + sMsg1);
                return new object[] { 0, 0, "医保门诊登记撤销失败" + sMsg1 };
            }
        }
        #endregion


        #region 门诊费用结算撤销_医疗保险(内部)
        public static object[] N_YBMZSFJSCX_YLBX(object[] objParam)
        {
            string sysdate = GetServerDateTime();
            string ybbh = objParam[0].ToString();
            string ybdjh = objParam[1].ToString();
            bool bfalg = false;
            byte[] sMsg = new byte[1024];
            string sMsg1 = string.Empty;

            WriteLog(sysdate + "  进入门诊费用结算_医疗保险撤销(内部)...");
            //入参
            putPara(whandler, "usr", ybuserid);
            putPara(whandler, "pwd", ybpasswd);

            putPara(whandler, "aac001", ybbh);
            putPara(whandler, "aae072", ybdjh);
            bfalg = process(whandler, "F04.05.12.01");
            if (bfalg)
            {
                WriteLog(sysdate + " 门诊费用结算_医疗保险撤销(内部)成功|");
                return new object[] { 0, 1, "门诊费用结算_医疗保险撤销(内部)成功|" };
            }
            else
            {
                getFailReason(whandler, sMsg);
                sMsg1 = Encoding.Default.GetString(sMsg, 0, sMsg.Length).Replace("\0", "");
                WriteLog(sysdate + " 门诊费用结算_医疗保险撤销(内部)失败|" + sMsg1);
                return new object[] { 0, 0, "门诊费用结算_医疗保险撤销(内部)失败|" + sMsg1 };
            }
        }
        #endregion

        #region 门诊费用结算撤销_生育保险(内部)
        public static object[] N_YBMZSFJSCX_SYBX(object[] objParam)
        {
            string sysdate = GetServerDateTime();
            string ybbh = objParam[0].ToString();
            string ybdjh = objParam[1].ToString();
            bool bfalg = false;
            byte[] sMsg = new byte[1024];
            string sMsg1 = string.Empty;

            WriteLog(sysdate + "  进入门诊费用结算撤销_生育保险(内部)...");
            //入参
            putPara(whandler, "usr", ybuserid);
            putPara(whandler, "pwd", ybpasswd);

            putPara(whandler, "aac001", ybbh);
            putPara(whandler, "aae072", ybdjh);
            putPara(whandler, "skc113", "S");
            bfalg = process(whandler, "F04.05.12.01");
            if (bfalg)
            {
                WriteLog(sysdate + " 门诊费用结算撤销_生育保险(内部)成功|");
                return new object[] { 0, 1, "门诊费用结算撤销_生育保险(内部)成功|" };
            }
            else
            {
                getFailReason(whandler, sMsg);
                sMsg1 = Encoding.Default.GetString(sMsg, 0, sMsg.Length).Replace("\0", "");
                WriteLog(sysdate + " 门诊费用结算撤销_生育保险(内部)失败|" + sMsg1);
                return new object[] { 0, 0, "门诊费用结算撤销_生育保险(内部)失败|" + sMsg1 };
            }
        }
        #endregion

        #region 门诊费用结算撤销_工伤保险(内部)
        public static object[] N_YBMZSFJSCX_GSBX(object[] objParam)
        {
            string sysdate = GetServerDateTime();
            string ybbh = objParam[0].ToString();
            string ybdjh = objParam[1].ToString();
            bool bfalg = false;
            byte[] sMsg = new byte[1024];
            string sMsg1 = string.Empty;

            WriteLog(sysdate + "  进入门诊费用结算撤销_工伤保险(内部)...");
            //入参
            putPara(whandler, "usr", ybuserid);
            putPara(whandler, "pwd", ybpasswd);

            putPara(whandler, "aac001", ybbh);
            putPara(whandler, "aae072", ybdjh);
            bfalg = process(whandler, "F07.11.06.02");
            if (bfalg)
            {
                WriteLog(sysdate + " 门诊费用结算撤销_工伤保险(内部)成功|");
                return new object[] { 0, 1, "门诊费用结算撤销_工伤保险(内部)成功|" };
            }
            else
            {
                getFailReason(whandler, sMsg);
                sMsg1 = Encoding.Default.GetString(sMsg, 0, sMsg.Length).Replace("\0", "");
                WriteLog(sysdate + " 门诊费用结算撤销_工伤保险(内部)失败|" + sMsg1);
                return new object[] { 0, 0, "门诊费用结算撤销_工伤保险(内部)失败|" + sMsg1 };
            }
        }
        #endregion


        #region 住院登记撤销_医疗保险(内部)
        public static object[] N_YBZYDJCX_YLBX(object[] objParam)
        {
            string sysdate = GetServerDateTime();
            string ybjzlsh = objParam[0].ToString();   // 医保就诊流水号
            string ybbh = objParam[1].ToString();
            byte[] sMsg = new byte[1024];
            string sMsg1 = string.Empty;
            bool bfalg = false;

            WriteLog(sysdate + "  进入住院登记撤销(内部)(医疗保险)...");

            //入参
            putPara(whandler, "usr", ybuserid);
            putPara(whandler, "pwd", ybpasswd);

            putPara(whandler, "aac001", ybbh);
            putPara(whandler, "akc190", ybjzlsh);
            WriteLog("入参|" + ybuserid + "|" + ybpasswd + "|" + ybbh + "|" + ybjzlsh);

            bfalg = process(whandler, "F04.06.04.01");
            if (bfalg)
            {

                WriteLog(sysdate + "  住院登记撤销成功(内部)(医疗保险)|");
                return new object[] { 0, 1, "住院登记撤销成功(内部)(医疗保险)|" };
            }
            else
            {
                getFailReason(whandler, sMsg);
                sMsg1 = Encoding.Default.GetString(sMsg, 0, sMsg.Length).Replace("\0", "");
                WriteLog(sysdate + "  住院登记撤销失败(内部)(医疗保险)|" + sMsg1);
                return new object[] { 0, 0, "住院登记撤销失败(内部)(医疗保险)|" + sMsg1 };
            }
        }
        #endregion

        #region 住院登记撤销_生育保险(内部)
        public static object[] N_YBZYDJCX_SYBX(object[] objParam)
        {
            string sysdate = GetServerDateTime();
            string ybjzlsh = objParam[0].ToString();   // 医保就诊流水号
            string ybbh = objParam[1].ToString();
            byte[] sMsg = new byte[1024];
            string sMsg1 = string.Empty;
            bool bfalg = false;

            WriteLog(sysdate + "  进入住院登记撤销(内部)(生育保险)...");

            //入参
            putPara(whandler, "usr", ybuserid);
            putPara(whandler, "pwd", ybpasswd);

            putPara(whandler, "aac001", ybbh);
            putPara(whandler, "akc190", ybjzlsh);
            putPara(whandler, "skc113", "S");
            WriteLog("入参|" + ybuserid + "|" + ybpasswd + "|" + ybbh + "|" + ybjzlsh + "|S");

            bfalg = process(whandler, "F04.06.04.01");
            if (bfalg)
            {

                WriteLog(sysdate + "  住院登记撤销成功(内部)(生育保险)|");
                return new object[] { 0, 1, "住院登记撤销成功(内部)(生育保险)|" };
            }
            else
            {
                getFailReason(whandler, sMsg);
                sMsg1 = Encoding.Default.GetString(sMsg, 0, sMsg.Length).Replace("\0", "");
                WriteLog(sysdate + "  住院登记撤销失败(内部)(生育保险)|" + sMsg1);
                return new object[] { 0, 0, "住院登记撤销失败(内部)(生育保险)|" + sMsg1 };
            }
        }
        #endregion

        #region 住院登记撤销_工伤保险(内部)
        public static object[] N_YBZYDJCX_GSBX(object[] objParam)
        {
            string sysdate = GetServerDateTime();
            string ybjzlsh = objParam[0].ToString();   // 医保就诊流水号
            string ybbh = objParam[1].ToString();
            byte[] sMsg = new byte[1024];
            string sMsg1 = string.Empty;
            bool bfalg = false;

            WriteLog(sysdate + "  进入住院登记撤销(内部)(工伤保险)...");

            //入参
            putPara(whandler, "usr", ybuserid);
            putPara(whandler, "pwd", ybpasswd);

            putPara(whandler, "aac001", ybbh);
            putPara(whandler, "akc190", ybjzlsh);
            WriteLog("入参|" + ybuserid + "|" + ybpasswd + "|" + ybbh + "|" + ybjzlsh);

            bfalg = process(whandler, "F07.11.01.02");
            if (bfalg)
            {

                WriteLog(sysdate + "  住院登记撤销成功(内部)(工伤保险)|");
                return new object[] { 0, 1, "住院登记撤销成功(内部)(工伤保险)|" };
            }
            else
            {
                getFailReason(whandler, sMsg);
                sMsg1 = Encoding.Default.GetString(sMsg, 0, sMsg.Length).Replace("\0", "");
                WriteLog(sysdate + "  住院登记撤销失败(内部)(工伤保险)|" + sMsg1);
                return new object[] { 0, 0, "住院登记撤销失败(内部)(工伤保险)|" + sMsg1 };
            }
        }
        #endregion


        #region 住院费用结算_医疗保险(内部)
        public static object[] N_YBZYFYJS_YLBX(object[] objParam)
        {
            string sysdate = GetServerDateTime();
            string ybjzlsh = objParam[0].ToString(); //住院号（在医保中心系统编号）
            string ybbh = objParam[1].ToString();       //  医保编号
            string bzbm1 = objParam[2].ToString();      //病情编码
            string bzbm2 = objParam[3].ToString();   //病情编码(2)
            string bzbm3 = objParam[4].ToString();   //病情编码(3)
            string scpch = objParam[5].ToString();   //上传批次号
            string sfzssy = "001"; //是否正式收费(000 收费预览)


            bool bfalg = false;
            byte[] sMsg = new byte[1024];
            byte[] outParam = new byte[1024];
            string sMsg1 = string.Empty;

            WriteLog(sysdate + "  进入住院费用结算(内部)(医疗保险)...");

            //入参
            putPara(whandler, "usr", ybuserid);
            putPara(whandler, "pwd", ybpasswd);

            putPara(whandler, "aac001", ybbh);
            putPara(whandler, "akc190", ybjzlsh);
            putPara(whandler, "aka120", bzbm1);
            putPara(whandler, "skc057", bzbm2);
            putPara(whandler, "skc058", bzbm3);
            putPara(whandler, "skc098", sfzssy);

            bfalg = process(whandler, "F04.06.10.01");
            if (bfalg)
            {
                List<string> liSQL = new List<string>();
                getErrMsg(whandler, sMsg);
                sMsg1 = Encoding.Default.GetString(sMsg, 0, sMsg.Length).Replace("\0", "");
                if (sMsg1.ToUpper().Equals("OK"))
                {
                    #region 出参
                    string djh_r = string.Empty;    //单据号
                    string zyh_r = string.Empty;    //住院号
                    string ybbh_r = string.Empty;   //个人编号

                    getParaByName(whandler, "aae072", outParam);
                    djh_r = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "akc190", outParam);
                    zyh_r = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "aac001", outParam);
                    ybbh_r = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    #endregion

                    //插入批次号
                    string strSql = string.Format(@"insert into yb_pcxx(pch,bz,djh,jzbz) values('{0}','{1}','{2}','{3}')", scpch, "1", djh_r, "z");
                    liSQL.Add(strSql);

                    object[] obj = liSQL.ToArray();
                    obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);
                    WriteLog(sysdate + "  住院费用结算(内部)(医疗保险)成功|单据号:" + djh_r + "|个人编号:" + ybbh_r + "|住院号:" + zyh_r);
                    object[] obj1 = { ybbh_r, djh_r };
                    return N_YBZYSFJSCX_YLBX(obj1);
                }
                else
                {
                    WriteLog(sysdate + "  住院费用结算(内部)(医疗保险)失败" + sMsg1);
                    return new object[] { 0, 0, "住院费用结算(内部)(医疗保险)失败" + sMsg1 };
                }
            }
            else
            {
                getFailReason(whandler, sMsg);
                sMsg1 = Encoding.Default.GetString(sMsg, 0, sMsg.Length).Replace("\0", "");
                WriteLog(sysdate + "  住院费用结算(内部)(医疗保险)失败|" + sMsg1);
                return new object[] { 0, 0, "住院费用结算(内部)(医疗保险)失败|" + sMsg1 };
            }
        }
        #endregion

        #region 住院费用结算_生育保险(内部)
        public static object[] N_YBZYFYJS_SYBX(object[] objParam)
        {
            string sysdate = GetServerDateTime();
            string ybjzlsh = objParam[0].ToString(); //住院号（在医保中心系统编号）
            string ybbh = objParam[1].ToString();       //  医保编号
            string bzbm1 = objParam[2].ToString();      //病情编码
            string bzbm2 = objParam[3].ToString();   //病情编码(2)
            string bzbm3 = objParam[4].ToString();   //病情编码(3)
            string scpch = objParam[5].ToString();   //上传批次号
            string sfzssy = "001"; //是否正式收费(000 收费预览)
            string sybxbz = "S";
            string sylx = "";
            string sfybfz = "000";
            string tes = "1";

            bool bfalg = false;
            byte[] sMsg = new byte[1024];
            byte[] outParam = new byte[1024];
            string sMsg1 = string.Empty;

            WriteLog(sysdate + "  进入住院费用结算(内部)(生育保险)...");

            //入参
            putPara(whandler, "usr", ybuserid);
            putPara(whandler, "pwd", ybpasswd);

            putPara(whandler, "aac001", ybbh);
            putPara(whandler, "akc190", ybjzlsh);
            putPara(whandler, "aka120", bzbm1);
            putPara(whandler, "skc057", bzbm2);
            putPara(whandler, "skc058", bzbm3);
            putPara(whandler, "skc098", sfzssy);
            putPara(whandler, "skc113", sybxbz);
            putPara(whandler, "mkc191", sylx);
            putPara(whandler, "smc052", sfybfz);
            putPara(whandler, "amc028", tes);

            bfalg = process(whandler, "F04.06.10.02");
            if (bfalg)
            {
                List<string> liSQL = new List<string>();
                getErrMsg(whandler, sMsg);
                sMsg1 = Encoding.Default.GetString(sMsg, 0, sMsg.Length).Replace("\0", "");
                if (sMsg1.ToUpper().Equals("OK"))
                {
                    #region 出参
                    string djh_r = string.Empty;    //单据号
                    string zyh_r = string.Empty;    //住院号
                    string ybbh_r = string.Empty;   //个人编号

                    getParaByName(whandler, "aae072", outParam);
                    djh_r = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "akc190", outParam);
                    zyh_r = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "aac001", outParam);
                    ybbh_r = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    #endregion

                    WriteLog(sysdate + "  住院费用结算(内部)(生育保险)成功|单据号:" + djh_r + "|个人编号:" + ybbh_r + "|住院号:" + zyh_r);
                    object[] obj1 = { ybbh_r, djh_r };
                    return N_YBZYSFJSCX_SYBX(obj1);
                }
                else
                {
                    WriteLog(sysdate + "  住院费用结算(内部)(生育保险)失败" + sMsg1);
                    return new object[] { 0, 0, "住院费用结算(内部)(生育保险)失败" + sMsg1 };
                }
            }
            else
            {
                getFailReason(whandler, sMsg);
                sMsg1 = Encoding.Default.GetString(sMsg, 0, sMsg.Length).Replace("\0", "");
                WriteLog(sysdate + "  住院费用结算(内部)(生育保险)失败|" + sMsg1);
                return new object[] { 0, 0, "住院费用结算(内部)(生育保险)失败|" + sMsg1 };
            }
        }
        #endregion

        #region 住院费用结算_工伤保险(内部)
        public static object[] N_YBZYFYJS_GSBX(object[] objParam)
        {
            string sysdate = GetServerDateTime();
            string ybjzlsh = objParam[0].ToString(); //住院号（在医保中心系统编号）
            string ybbh = objParam[1].ToString();       //  医保编号
            string bzbm1 = objParam[2].ToString();      //病情编码
            string bzbm2 = objParam[3].ToString();   //病情编码(2)
            string bzbm3 = objParam[4].ToString();   //病情编码(3)
            string scpch = objParam[5].ToString();   //上传批次号
            string sfzssy = "001"; //是否正式收费(000 收费预览)
            string sfybfz = "000";

            bool bfalg = false;
            byte[] sMsg = new byte[1024];
            byte[] outParam = new byte[1024];
            string sMsg1 = string.Empty;

            WriteLog(sysdate + "  进入住院费用结算(内部)(工伤保险)...");

            //入参
            putPara(whandler, "usr", ybuserid);
            putPara(whandler, "pwd", ybpasswd);

            putPara(whandler, "aac001", ybbh);
            putPara(whandler, "akc190", ybjzlsh);
            putPara(whandler, "aka120", bzbm1);
            putPara(whandler, "skc057", bzbm2);
            putPara(whandler, "skc058", bzbm3);
            putPara(whandler, "skc098", sfzssy);
            putPara(whandler, "smc052", sfybfz);

            bfalg = process(whandler, "F07.11.03.01");
            if (bfalg)
            {
                List<string> liSQL = new List<string>();
                getErrMsg(whandler, sMsg);
                sMsg1 = Encoding.Default.GetString(sMsg, 0, sMsg.Length).Replace("\0", "");
                if (sMsg1.ToUpper().Equals("OK"))
                {
                    #region 出参
                    string djh_r = string.Empty;    //单据号
                    string zyh_r = string.Empty;    //住院号
                    string ybbh_r = string.Empty;   //个人编号

                    getParaByName(whandler, "aae072", outParam);
                    djh_r = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "akc190", outParam);
                    zyh_r = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    outParam = new byte[1024];
                    getParaByName(whandler, "aac001", outParam);
                    ybbh_r = Encoding.Default.GetString(outParam, 0, outParam.Length).Replace("\0", "");
                    #endregion

                    WriteLog(sysdate + "  住院费用结算(内部)(工伤保险)成功|单据号:" + djh_r + "|个人编号:" + ybbh_r + "|住院号:" + zyh_r);
                    object[] obj1 = { ybbh_r, djh_r };
                    return N_YBZYSFJSCX_GSBX(obj1);
                }
                else
                {
                    WriteLog(sysdate + "  住院费用结算(内部)(工伤保险)失败" + sMsg1);
                    return new object[] { 0, 0, "住院费用结算(内部)(工伤保险)失败" + sMsg1 };
                }
            }
            else
            {
                getFailReason(whandler, sMsg);
                sMsg1 = Encoding.Default.GetString(sMsg, 0, sMsg.Length).Replace("\0", "");
                WriteLog(sysdate + "  住院费用结算(内部)(工伤保险)失败|" + sMsg1);
                return new object[] { 0, 0, "住院费用结算(内部)(工伤保险)失败|" + sMsg1 };
            }
        }
        #endregion


        #region 住院费用结算撤销_医疗保险(内部)
        public static object[] N_YBZYSFJSCX_YLBX(object[] objParam)
        {
            string sysdate = GetServerDateTime();//当前时间
            string ybbh = objParam[0].ToString();  //医保编号
            string ybdjh = objParam[1].ToString(); //医保单据号

            bool bfalg = false;
            byte[] sMsg = new byte[1024];
            string sMsg1 = string.Empty;
            WriteLog(sysdate + "  进入住院费用结算撤销(内部)...");

            //入参
            putPara(whandler, "usr", ybuserid);
            putPara(whandler, "pwd", ybpasswd);

            putPara(whandler, "aac001", ybbh);
            putPara(whandler, "aae072", ybdjh);

            bfalg = process(whandler, "F04.06.12.01");
            if (bfalg)
            {
                WriteLog(sysdate + "  住院费用结算撤销(内部)成功|");
                return new object[] { 0, 1, "住院费用结算撤销(内部)成功|" };
            }
            else
            {
                getFailReason(whandler, sMsg);
                sMsg1 = Encoding.Default.GetString(sMsg, 0, sMsg.Length).Replace("\0", "");
                WriteLog(sysdate + "  住院费用结算撤销(内部)失败|" + sMsg1);
                return new object[] { 0, 0, "住院费用结算撤销(内部)失败|" + sMsg1 };
            }
        }
        #endregion

        #region 住院费用结算撤销_生育保险(内部)
        public static object[] N_YBZYSFJSCX_SYBX(object[] objParam)
        {
            string sysdate = GetServerDateTime();//当前时间
            string ybbh = objParam[0].ToString();  //医保编号
            string ybdjh = objParam[1].ToString(); //医保单据号

            bool bfalg = false;
            byte[] sMsg = new byte[1024];
            string sMsg1 = string.Empty;
            WriteLog(sysdate + "  进入住院费用结算撤销(内部)...");

            //入参
            putPara(whandler, "usr", ybuserid);
            putPara(whandler, "pwd", ybpasswd);

            putPara(whandler, "aac001", ybbh);
            putPara(whandler, "aae072", ybdjh);
            putPara(whandler, "skc113", "S");

            bfalg = process(whandler, "F04.06.12.01");
            if (bfalg)
            {
                WriteLog(sysdate + "  住院费用结算撤销(内部)成功|");
                return new object[] { 0, 1, "住院费用结算撤销(内部)成功|" };
            }
            else
            {
                getFailReason(whandler, sMsg);
                sMsg1 = Encoding.Default.GetString(sMsg, 0, sMsg.Length).Replace("\0", "");
                WriteLog(sysdate + "  住院费用结算撤销(内部)失败|" + sMsg1);
                return new object[] { 0, 0, "住院费用结算撤销(内部)失败|" + sMsg1 };
            }
        }
        #endregion

        #region 住院费用结算撤销_工伤保险(内部)
        public static object[] N_YBZYSFJSCX_GSBX(object[] objParam)
        {
            string sysdate = GetServerDateTime();//当前时间
            string ybbh = objParam[0].ToString();  //医保编号
            string ybdjh = objParam[1].ToString(); //医保单据号

            bool bfalg = false;
            byte[] sMsg = new byte[1024];
            string sMsg1 = string.Empty;
            WriteLog(sysdate + "  进入住院费用结算撤销(内部)...");

            //入参
            putPara(whandler, "usr", ybuserid);
            putPara(whandler, "pwd", ybpasswd);

            putPara(whandler, "aac001", ybbh);
            putPara(whandler, "aae072", ybdjh);

            bfalg = process(whandler, "F07.11.03.02");
            if (bfalg)
            {
                WriteLog(sysdate + "  住院费用结算撤销(内部)成功|");
                return new object[] { 0, 1, "住院费用结算撤销(内部)成功|" };
            }
            else
            {
                getFailReason(whandler, sMsg);
                sMsg1 = Encoding.Default.GetString(sMsg, 0, sMsg.Length).Replace("\0", "");
                WriteLog(sysdate + "  住院费用结算撤销(内部)失败|" + sMsg1);
                return new object[] { 0, 0, "住院费用结算撤销(内部)失败|" + sMsg1 };
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
        #endregion
    }
}
