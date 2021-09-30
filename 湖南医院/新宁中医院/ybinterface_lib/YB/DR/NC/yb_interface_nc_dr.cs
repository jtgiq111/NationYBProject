using Srvtools;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

//南昌地区医保接口
namespace ybinterface_lib
{
    public class yb_interface_nc_dr
    {
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
        #endregion

        #region 医保dll函数声明
        [DllImport("SiInterface.dll", EntryPoint = "INIT", CharSet = CharSet.Ansi)]
        static extern int INIT(StringBuilder pErrMsg);

        [DllImport("SiInterface.dll", EntryPoint = "BUSINESS_HANDLE", CharSet = CharSet.Ansi)]
        static extern int BUSINESS_HANDLE(StringBuilder inputData, StringBuilder outputData);
        #endregion

        #region 查询类

        #region 医疗待遇审批信息查询
        public static object[] YBYLDYSPXXCX(object[] objParam)
        {
            string sysdate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            WriteLog(sysdate + "  进入医疗待遇审批信息查询...");
            string grbh = objParam[0].ToString();
            string splx = objParam[1].ToString(); //16-门诊慢性病 17-门诊特病

            if (string.IsNullOrEmpty(grbh))
                return new object[] { 0, 0, "个人编码不能为空" };
            if (string.IsNullOrEmpty(splx))
                return new object[] { 0, 0, "审批类别不能为空" };
            try
            {
                #region 异地医保无审批功能
                string strSql = string.Format(@"select DQJBBZ from ybickxx where grbh='{0}'", grbh);
                DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                DQJBBZ = ds.Tables[0].Rows[0]["DQJBBZ"].ToString();
                if (DQJBBZ.Equals("2"))
                    return new object[] { 0, 0, "异地无医疗待遇审批信息查询功能" };
                #endregion
                CZYBH = CliUtils.fLoginUser; //操作员工号
                YWZQH = CliUtils.fLoginYbNo.Split('|')[0].ToString();    //业务周期号
                JYLSH = DateTime.Now.ToString("yyyyMMddHHmmss") + "-" + YLGHBH + "-" + new Random().Next(100).ToString().PadLeft(4, '0');

                #region 参数
                /*
                 * 1	个人编号	VARCHAR2(14)	NOT NULL	
                 * 2	审批类别	VARCHAR2(3)	NOT NULL	
                 * 3	医院编号	VARCHAR2(14)		当为慢性病、特检特治特药时不能为空
                 * 4	就诊号	VARCHAR2(18)		审批类别为特检特治时:如果为空,当作查询挂号前,对应到某个人的审批查询,否则当作具体到某个项目的审批
                 * 5	项目编号	VARCHAR2(20)		当为特检特治特药时并且就诊号不为空时不能为空
                 * 6	病种编码	VARCHAR2(20)		
                 * 7	判断时间	VARCHAR2(8)		YYYYMMDD
                */
                YWBH = "1600";
                StringBuilder inputParam = new StringBuilder();
                //本市级
                inputParam.Append(grbh + "|");   //个人编号
                inputParam.Append(splx + "|");      //审批类别
                inputParam.Append(YLGHBH + "|");      //医院编号
                inputParam.Append("|");      //就诊号
                inputParam.Append("|");      //项目编号
                inputParam.Append("|");      //病种编码
                inputParam.Append("|");             //判断时间
                #endregion
                StringBuilder inputData = new StringBuilder();
                StringBuilder outputData = new StringBuilder(10240);

                inputData.Append(YWBH + "^");
                inputData.Append(YLGHBH + "^");
                inputData.Append(CZYBH + "^");
                inputData.Append(YWZQH + "^");
                inputData.Append(JYLSH + "^");
                inputData.Append(ZXBM + "^");
                inputData.Append(inputParam + "^");
                inputData.Append(LJBZ + "^");

                WriteLog(sysdate + "  入参|" + inputData.ToString());

                string strMsg = string.Empty;
                int i = BUSINESS_HANDLE(inputData, outputData);
                if (i == 0)
                {

                    WriteLog(sysdate + "   医疗待遇审批信息查询成功|" + outputData.ToString().Split('^')[2].ToString());
                    /*
                     * 1	审批状态	VARCHAR2(18)		0未审批、1审批通过、2审批未通过
                     * 2	审批编号	NUMBER(16)		
                     * 3	定点医疗机构编号	VARCHAR2(20)		
                     * 4	定点医疗机构名称	VARCHAR2(100)		
                     * 5	病种编码	VARCHAR2(50)		
                     * 6	病种名称	VARCHAR2(300)		
                     * 7	开始日期	DATE		
                     * 8	终止日期	DATE	
                     * 1|4729071|0201|安义县中医院|TSB0004|高血压|20170101||$1|4729072|0201|安义县中医院|TSB0005|糖尿病|20170101||
                     */
                    string[] sval = outputData.ToString().Split('^')[2].ToString().Split('$');
                    foreach (string val in sval)
                    {
                        string[] sval1 = val.Split('|');
                        if (sval1[0].ToString().Equals("1"))
                        {
                            string svalue = sval1[4] + "|" + sval1[5];
                            strMsg += svalue + "$";
                        }
                    }
                }
                else
                {
                    WriteLog(sysdate + "   医疗待遇审批信息查询出错|" + outputData.ToString().Split('^')[2].ToString());
                    return new object[] { 0, 0, outputData.ToString().Split('^')[2].ToString() };
                }

                if (!string.IsNullOrEmpty(strMsg))
                {
                    WriteLog(sysdate + "  " + strMsg.TrimEnd('$'));
                    return new object[] { 0, 1, strMsg.TrimEnd('$') };
                }
                else
                    return new object[] { 0, 0, "无慢病信息" };
            }
            catch (Exception ex)
            {
                WriteLog(sysdate + "  医疗待遇审批信息查询|系统异常|" + ex.Message);
                return new object[] { 0, 0, "Error:" + ex.Message };
            }
        }
        #endregion

        #region 医疗待遇封锁信息查询
        /// <summary>
        /// 医疗待遇封锁信息查询
        /// </summary>
        /// <param>个人编号,社会保障卡卡号</param>
        /// <returns>返回0为未封锁，其他为封锁</returns>
        public static object[] YBYLDYFSXXCX(object[] objParam)
        {
            string sysdate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string grbh = objParam[0].ToString();
            string yllb = objParam[1].ToString(); //医疗类别
            string fssj = Convert.ToDateTime(sysdate).ToString("yyyyMMddHHmmss");


            if (string.IsNullOrEmpty(grbh))
                return new object[] { 0, 0, "个人编码不能为空" };
            if (string.IsNullOrEmpty(yllb))
                return new object[] { 0, 0, "医疗类别不能为空" };
            try
            {
                CZYBH = CliUtils.fLoginUser; //操作员工号
                YWZQH = CliUtils.fLoginYbNo.Split('|')[0].ToString();    //业务周期号
                JYLSH = DateTime.Now.ToString("yyyyMMddHHmmss") + "-" + YLGHBH + "-" + new Random().Next(100).ToString().PadLeft(4, '0');

                string strSql = string.Format(@"select DWBH from ybickxx where grbh='{0}'",grbh);
                 DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                 if (ds.Tables[0].Rows.Count == 0)
                     return new object[] { 0, 0, "未获取单位编号" };
                 string dwbh = ds.Tables[0].Rows[0]["DWBH"].ToString();

                 #region 参数
                 /*
                 1		单位编号	VARCHAR2(14)	NOT NULL	
                2		个人编号	VARCHAR2(14)	NOT NULL	
                3		个人卡号	VARCHAR2(20)		南昌不用
                4		医疗类别	VARCHAR2(3)	NOT NULL	代码
                5		判断时间	VARCHAR2(8)	NOT NULL	判断有效期，YYYYMMDD
                */
                 YWBH = "1500";
                 StringBuilder inputParam = new StringBuilder();
                 //本市级
                 inputParam.Append(dwbh + "|");   //单位编号
                 inputParam.Append(grbh + "|");   //个人编号
                 inputParam.Append("|");      //个人卡号
                 inputParam.Append(yllb + "|");      //医疗类别
                 inputParam.Append(fssj + "|");      //判断时间
                 #endregion
                 StringBuilder inputData = new StringBuilder();
                 StringBuilder outputData = new StringBuilder(10240);

                 inputData.Append(YWBH + "^");
                 inputData.Append(YLGHBH + "^");
                 inputData.Append(CZYBH + "^");
                 inputData.Append(YWZQH + "^");
                 inputData.Append(JYLSH + "^");
                 inputData.Append(ZXBM + "^");
                 inputData.Append(inputParam + "^");
                 inputData.Append(LJBZ + "^");

                 WriteLog(sysdate + "  入参|" + inputData.ToString());

                 int i = BUSINESS_HANDLE(inputData, outputData);
                 if (i > 0)
                 {
                     WriteLog(sysdate + " 医疗待遇封锁信息查询|出参|" + outputData.ToString());
                     string tsxx = outputData.ToString().Split('^')[2];
                     return new object[] { 0, tsxx.Split('|')[0], tsxx };
                 }
                 else
                 {
                     WriteLog(sysdate + " 医疗待遇封锁信息查询失败|" + outputData.ToString().Split('^')[2]);
                     return new object[] { 0,0, outputData.ToString().Split('^')[2] };
                 }
            }
            catch (Exception ex)
            {
                WriteLog(sysdate + "  医疗待遇封锁信息查询|异常|"+ex.Message);
                return new object[] { 0, 0, "医疗待遇封锁信息查询|异常|" + ex.Message };
            }
        }
        #endregion

        #region 批量数据查询下载
        public static object[] YBPLSJCXXZ(object[] objParam)
        {
            string sysdate = GetServerDateTime();
            WriteLog(sysdate + "  进入批量数据查询下载...");
            try
            {
                string sjlb = objParam[0].ToString(); //数据类别
                string startDate = objParam[1].ToString();    //开始时间
                string endData = objParam[2].ToString();


                if (string.IsNullOrEmpty(startDate))
                    return new object[] { 0, 0, "开始时间不能为空" };
                if (string.IsNullOrEmpty(sjlb))
                    return new object[] { 0, 0, "数据类别不能为空" };

                string sDate = Convert.ToDateTime(startDate).ToString("yyyyMMddHHmmss");



                CZYBH = CliUtils.fLoginUser; //操作员工号
                YWZQH = CliUtils.fLoginYbNo.Split('|')[0].ToString();    //业务周期号
                JYLSH = DateTime.Now.ToString("yyyyMMddHHmmss") + "-" + YLGHBH + "-" + new Random().Next(100).ToString().PadLeft(4, '0');
                YWBH = "1300";  //业务编号

                StringBuilder inParam = new StringBuilder();
                inParam.Append(sjlb + "|");
                inParam.Append(sDate + "|");

                //入参
                StringBuilder inputData = new StringBuilder();
                inputData.Append(YWBH + "^");
                inputData.Append(YLGHBH + "^");
                inputData.Append(CZYBH + "^");
                inputData.Append(YWZQH + "^");
                inputData.Append(JYLSH + "^");
                inputData.Append(ZXBM + "^");
                inputData.Append(inParam.ToString() + "^");
                inputData.Append(LJBZ + "^");

                StringBuilder outputData = new StringBuilder(10240);
                WriteLog(sysdate + "  入参|" + inputData.ToString());
                int i = BUSINESS_HANDLE(inputData, outputData);
                if (i == 0)
                {
                    WriteLog(sysdate + "  批量数据查询下载成功|出参|" + outputData.ToString());
                    return new object[] { 0, 1, "批量数据查询下载成功|" + outputData.ToString() };
                }
                else
                {
                    WriteLog(sysdate + "  批量数据查询下载失败|" + outputData.ToString());
                    return new object[] { 0, 0, "批量数据查询下载失败|" + outputData.ToString() };
                }
            }
            catch (Exception ex)
            {
                WriteLog(sysdate + "  批量数据查询下载异常|" + ex.Message);
                return new object[] { 0, 0, "批量数据查询下载异常|" + ex.Message };
            }
        }
        #endregion

        #endregion

        #region 功能类
        #region 医保对照信息上传
        public static object[] YBDZXXPLSC(object[] objParam)
        {
            string sysdate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            
            try
            {
                YWBH = "3130";
                string strSql = string.Format(@"select * from ybhisdzdr where scbz=0 ");
                DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                if (ds.Tables[0].Rows.Count == 0)
                    return new object[] { 0, 0, "无上传对照信息" };
                List<string> liSql = new List<string>();
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    string sfxmzldm = dr["sfxmzldm"].ToString();
                    string ybxmbh = dr["ybxmbh"].ToString();
                    string ybxmmc = dr["ybxmmc"].ToString();
                    string hisxmbh = dr["hisxmbh"].ToString();
                    string hisxmmc = dr["hisxmmc"].ToString();
                    string jx = "";
                    string dw = "";
                    string gg = "";
                    string dj = "";
                    string cd = ""; //产地
                    string jbr = "";
                    string ksrq = "";
                    string zzrq = "";

                    /*
                     * 1		项目类别	VARCHAR2(3)	NOT NULL	1药品、2诊疗项目、3服务设施
                        2		三大目录中心编码	VARCHAR2(20)	NOT NULL	项目中心编码
                        3		医院项目内码	VARCHAR2(20)	NOT NULL	
                        4		医院项目名称	VARCHAR2(50)	NOT NULL	
                        5		定点医疗机构药品剂型	VARCHAR2(20)		
                        6		单位	VARCHAR2(10)		
                        7		规格	VARCHAR2(14)		
                        8		医院端价格	VARCHAR2(12)		4位小数
                        9		医院端产地	VARCHAR2(50)		
                        10		对照经办人	VARCHAR2(20)		
                        11		对照操作时间	VARCHAR2(14)		YYYYMMDDHH24MISS
                        12		开始日期	VARCHAR2(14)		YYYYMMDDHH24MISS
                        13		终止日期	VARCHAR2(14)		YYYYMMDDHH24MISS
                     */
                    StringBuilder inputParam = new StringBuilder();
                    inputParam.Append(sfxmzldm + "|");
                    inputParam.Append(ybxmbh + "|");
                    inputParam.Append(hisxmbh + "|");
                    inputParam.Append(hisxmmc + "|");
                    inputParam.Append(jx + "|");
                    inputParam.Append(dw + "|");
                    inputParam.Append(gg + "|");
                    inputParam.Append(dj + "|");
                    inputParam.Append(cd + "|");
                    inputParam.Append(jbr + "|");
                    inputParam.Append(ksrq + "|");
                    inputParam.Append(ksrq + "|");
                    inputParam.Append(zzrq + "|");


                    StringBuilder inputData = new StringBuilder();
                    inputData.Append(YWBH + "^");
                    inputData.Append(YLGHBH + "^");
                    inputData.Append(CZYBH + "^");
                    inputData.Append(YWZQH + "^");
                    inputData.Append(JYLSH + "^");
                    inputData.Append(ZXBM + "^");
                    inputData.Append(inputParam.ToString() + "^");
                    inputData.Append(LJBZ + "^");

                    WriteLog(sysdate + "  对照信息上传|入参|" + inputData.ToString());

                    StringBuilder outputData = new StringBuilder(10240);
                    int i = BUSINESS_HANDLE(inputData, outputData);
                    WriteLog(sysdate + "  对照信息上传|出参|" + outputData.ToString());
                    if (i < 0)
                        return new object[] { 0,0,"对照信息上传失败|"+outputData.ToString()};
                    if (i == 0)
                    {
                      
                        strSql = string.Format(@"update ybhisdzdr set scbz=1 where hisxmbh='{0}'",hisxmbh);
                        liSql.Add(strSql);
                    }
                    inputParam.Clear();
                    inputData.Clear();
                }
                ds.Dispose();
                object[] obj = liSql.ToArray();
                liSql.Clear();
                obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);

                if (obj[1].ToString() == "1")
                {
                    WriteLog(sysdate + "    对照信息上传成功" );
                    return new object[] { 0, 1, "对照信息上传成功" };
                }
                else
                {
                    WriteLog(sysdate + "    对照信息上传失败|" + obj[2].ToString());
                    return new object[] { 0, 0, "对照信息上传失败|" };
                }
            }
            catch (Exception ex)
            {
                WriteLog(sysdate + "  医保对照信息上传异常|" + ex.Message);
                return new object[] { 0, 0, "医保对照信息上传异常|" + ex.Message };
            }
        }
        #endregion

        #region 获取病种信息查询
        public static object[] YBBZCX(object[] objParam)
        {
            string yllb = objParam[0].ToString(); // 医疗类别
            string grbh = objParam[1].ToString(); //个人编号
            string jzbz = objParam[2].ToString();   //门诊住院标志 m-门诊 z-住院
            string splb = objParam[3].ToString();   //审批类别
            DataSet ds = null;
            string[] syl_mz = { "13","14" };
            WriteLog("  获取病种信息查询|入参|" + yllb + "|" + grbh + "|" + jzbz + "|" + splb);

            string strSql = string.Empty;
            if (jzbz.ToUpper().Equals("M"))
            {
                if (syl_mz.Contains(yllb))
                {
                    //DataTable dt = new DataTable();
                    //dt.Columns.Add("dm", typeof(String));
                    //dt.Columns.Add("dmmc", typeof(String));
                    ////慢病审批查询
                    //object[] objParam1 = { grbh, "16" };
                    //objParam1 = YBYLDYSPXXCX(objParam1);
                    //if (objParam1[1].ToString().Equals("1"))
                    //{
                    //    string[] sV = objParam1[2].ToString().Split('|');
                    //    dt.Rows.Add(sV[0], sV[1]);
                    //}

                    //object[] objParam2 = { grbh, "17" };
                    //objParam2 = YBYLDYSPXXCX(objParam2);
                    //if (objParam2[1].ToString().Equals("1"))
                    //{
                    //    string[] sV = objParam2[2].ToString().Split('|');
                    //    dt.Rows.Add(sV[0], sV[1]);
                    //}
                    //return new object[] { 0, 1, dt };
                    strSql = string.Format(@"select dm,dmmc,pym,wbm from ybbzmrdr where ybm=2");
                    ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                    WriteLog("获取病种信息查询成功|");
                    return new object[] { 0, 1, ds.Tables[0] };
                }
                else
                {
                    strSql = string.Format(@"select dm,dmmc,pym,wbm from ybbzmrdr where ybm=1");
                    ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                    WriteLog("获取病种信息查询成功|");
                    return new object[] { 0, 1, ds.Tables[0] };
                }
            }
            else
            {
                strSql = string.Format(@"select dm,dmmc,pym,wbm from ybbzmrdr where ybm=1");
                ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                WriteLog("获取病种信息查询成功|");
                return new object[] { 0, 1, ds.Tables[0] };
            }
        }
        #endregion
        #endregion

        #region 接口业务调用方法

        #region 初始化
        public static object[] YBINIT(object[] objParam)
        {
            CZYBH = CliUtils.fLoginUser;  //用户工号
            string sysdate = GetServerDateTime(); //获取系统时间

            if (string.IsNullOrEmpty(CZYBH))
                return new object[] { 0, 0, "用户工号不能为空" };

            //Ping医保网
            Ping ping = new Ping();
            PingReply pingReply = ping.Send(YBIP);
            if (pingReply.Status != IPStatus.Success)
            {
                return new object[] { 0, 0, "未连接医保网" };
            }

            YWBH = "9100";
            JYLSH = DateTime.Now.ToString("yyyyMMddHHmmss") + "-" + YLGHBH + "-" + new Random().Next(100).ToString().PadLeft(4, '0');

            //入参
            StringBuilder inputData = new StringBuilder();
            inputData.Append(YWBH + "^");
            inputData.Append(YLGHBH + "^");
            inputData.Append(CZYBH + "^");
            inputData.Append(YWZQH + "^");
            inputData.Append(JYLSH + "^");
            inputData.Append(ZXBM + "^");
            inputData.Append("^");
            inputData.Append(LJBZ + "^");

            WriteLog(sysdate + "  用户" + CZYBH + " 进入医保初始化...");
            StringBuilder outputData = new StringBuilder(1024);

            int i = INIT(outputData);
            if (i == 0)
                WriteLog(sysdate + "  用户" + CZYBH + " 进入医保初始化成功|" + outputData.ToString());
            else
            {
                WriteLog(sysdate + "  用户" + CZYBH + " 进入医保初始化失败|" + outputData.ToString());
                return new object[] { 0, 0, outputData.ToString().Split('^')[2].ToString() };
            }
            //签入
            WriteLog(sysdate + "  用户" + CZYBH + " 进入医保签到...");
            WriteLog(sysdate + "  入参|" + inputData.ToString());
            i = BUSINESS_HANDLE(inputData, outputData);
            if (i == 0)
            {
                YWZQH = outputData.ToString().Split('^')[2].TrimEnd('|');
                WriteLog(sysdate + "  用户" + CZYBH + " 进入医保签到成功|" + outputData.ToString());
                CliUtils.fLoginYbNo = YWZQH;
                return new object[] { 0, 1, YWZQH };
            }
            else
            {
                WriteLog(sysdate + "  用户" + CZYBH + " 进入医保签到失败|" + outputData.ToString());
                return new object[] { 0, 0, outputData.ToString().Split('^')[2].ToString() };
            }
        }
        #endregion

        #region 医保退出
        public static object[] YBEXIT(object[] objParam)
        {
            string sysdate = GetServerDateTime();
            CZYBH = CliUtils.fLoginUser; //操作员工号
            YWZQH = CliUtils.fLoginYbNo.Split('|')[0].ToString();    //业务周期号

            JYLSH = DateTime.Now.ToString("yyyyMMddHHmmss") + "-" + YLGHBH + "-" + new Random().Next(100).ToString().PadLeft(4, '0');
            YWBH = "9110";  //业务编号

            //入参
            StringBuilder inputData = new StringBuilder();
            inputData.Append(YWBH + "^");
            inputData.Append(YLGHBH + "^");
            inputData.Append(CZYBH + "^");
            inputData.Append(YWZQH + "^");
            inputData.Append(JYLSH + "^");
            inputData.Append(ZXBM + "^");
            inputData.Append("^");
            inputData.Append(LJBZ + "^");

            StringBuilder outputData = new StringBuilder(1024);

            WriteLog(sysdate + "  用户" + CZYBH + " 进入医保退出...");
            WriteLog(sysdate + "  入参|" + inputData.ToString());
            int i = BUSINESS_HANDLE(inputData, outputData);
            if (i == 0)
            {
                WriteLog(sysdate + "  用户" + CZYBH + " 进入医保退出成功|");
                return new object[] { 0, 1, "医保退出成功|", "" };
            }
            else
            {
                WriteLog(sysdate + "  用户" + CZYBH + " 进入医保退出失败|" + outputData.ToString());
                return new object[] { 0, 0, "医保退出失败|", outputData.ToString().Split('^')[2].ToString() };
            }
        }
        #endregion

        #region 医保门诊读卡
        public static object[] YBMZDK(object[] objParam)
        {
            string sysdate = GetServerDateTime();
            string YBKLX = objParam[0].ToString();  //医保卡类型    0:正式卡，1：临时卡
            DQJBBZ = objParam[1].ToString();  //地区级别标志 1-本市级 2-本省异地 

            if (string.IsNullOrEmpty(DQJBBZ))
                return new object[] { 0,0,"地区级别标志不能为空"};

            if (DQJBBZ.Equals("1"))
            {
                return YBMZZYDK_SBJ(objParam);
            }
            else
            {
                return YBMZZYDK_SNYD(objParam);
            }
        }
        #endregion

        #region 医保门诊读卡1
        public static object[] YBMZDK1(object[] objParam)
        {
            string sysdate = GetServerDateTime();
            frm_YBDKChoose ybdk = new frm_YBDKChoose();
            ybdk.ShowDialog();
            string[] inParam = ybdk.strReutrn.Split(new char[] { '|' });

            DQJBBZ = inParam[1];
            object[] objParam1 = { inParam[0], inParam[1] };
            if (string.IsNullOrEmpty(DQJBBZ))
                return new object[] { 0, 0, "地区级别标志不能为空" };

            if (DQJBBZ.Equals("1"))
            {
                return YBMZZYDK_SBJ(objParam1);
            }
            else
            {
                return YBMZZYDK_SNYD(objParam1);
            }
        }
        #endregion

        #region 医保住院读卡
        public static object[] YBZYDK(object[] objParam)
        {
            string sysdate = GetServerDateTime();
            string YBKLX = objParam[0].ToString();  //医保卡类型    0:正式卡，1：临时卡
            DQJBBZ = objParam[1].ToString();  //地区级别标志 1-本市级 2-本省异地  3-其他

            if (string.IsNullOrEmpty(DQJBBZ))
                return new object[] { 0, 0, "地区级别标志不能为空" };

            if (DQJBBZ.Equals("1"))
            {
                return YBMZZYDK_SBJ(objParam);
            }
            else
            {
                return YBMZZYDK_SNYD(objParam);
            }
        }
        #endregion

        #region 医保门诊/住院读卡(市本级)
        public static object[] YBMZZYDK_SBJ(object[] objParam)
        {
            string sysdate = GetServerDateTime();
            WriteLog(sysdate + "  进入门诊/住院读卡(市本级)...");
            try
            {
                string YBKLX = objParam[0].ToString();  //医保卡类型    0:正式卡，1：临时卡
                DQJBBZ = objParam[1].ToString(); //异地标志  1 市本级 2 异地医保
                CZYBH = CliUtils.fLoginUser; //操作员工号
                YWZQH = CliUtils.fLoginYbNo.Split('|')[0].ToString();    //业务周期号
                JYLSH = DateTime.Now.ToString("yyyyMMddHHmmss") + "-" + YLGHBH + "-" + new Random().Next(100).ToString().PadLeft(4, '0');
                List<string> liSQL = new List<string>();
                //入参
                StringBuilder inputData = new StringBuilder();
                StringBuilder outputData = new StringBuilder(1024);
                //业务代码
                if (YBKLX.Equals("1"))
                    YWBH = "2102";
                else
                    YWBH = "2100";
                inputData.Append(YWBH + "^");
                inputData.Append(YLGHBH + "^");
                inputData.Append(CZYBH + "^");
                inputData.Append(YWZQH + "^");
                inputData.Append(JYLSH + "^");
                inputData.Append(ZXBM + "^");
                inputData.Append("^");
                inputData.Append(LJBZ + "^");

                WriteLog(sysdate + "  入参|" + inputData.ToString());
                //进入本市级读卡
                int i = BUSINESS_HANDLE(inputData, outputData);
                if (i == 0)
                {
                    DQJBBZ = "1"; //市本级
                    WriteLog(sysdate + "  出参|市本级读卡成功|" + outputData.ToString());
                    #region 出参
                    string[] sValue = outputData.ToString().Split('^')[2].Split('|');
                    /*返回参数格式:
                     * 个人编号|单位编号|身份证号|姓名|性别|民族|出生日期|行政职务|社会保障卡卡号|个人身份|
                     *干部标志|公务员标识|医疗人员类别|居民类别|医疗待遇类别|人员状态|单位名称|所属区号|单位类型|基金类型|
                     *业务年度|帐户余额|在院状态|本年医疗费累计|本年现金支出累计|本年帐户支出累计|本年统筹支出累计|本年大病支出累计|本年公补支出累计|本年企补支出累计|
                     *进入统筹费用累计|进入大病费用累计|本年自费累计|本年自理累计|起付标准累计|本年住院次数|民政救助人员标志|军转干部标志|医疗账户余额|公补账户余额|
                     *庭门诊统筹账户余额|建档立卡人员标志
                     */

                    outParams_dk OP = new outParams_dk();
                    OP.Grbh = sValue[0];
                    OP.Dwbh = sValue[1];
                    OP.Sfhz = sValue[2];
                    OP.Xm = sValue[3];
                    OP.Xb = sValue[4];
                    OP.Mz = sValue[5];
                    OP.Csrq = sValue[6];
                    OP.Xzzwjb = sValue[7];
                    OP.Kh = sValue[8];
                    OP.Grsf = sValue[9];
                    OP.Gbbz = sValue[10];
                    OP.Gwybcylbxcbbz = sValue[11];
                    OP.Ylrylb = sValue[12];
                    OP.Jmlx = sValue[13];
                    OP.Yldylb = sValue[14];
                    OP.Rycbzt = sValue[15];
                    OP.Dwmc = sValue[16];
                    OP.Tcqh = sValue[17];
                    OP.Cbdwlx = sValue[18];
                    OP.Jjlx = sValue[19];
                    OP.Nd = sValue[20];
                    OP.Zhye = sValue[21];
                    OP.Zyzt = sValue[22];
                    OP.Bnylflj = sValue[23];
                    OP.Bnxjzclj = sValue[24];
                    OP.Bnzhzclj = sValue[25];
                    OP.Bntczclj = sValue[26];
                    OP.Bndbyljjzflj = sValue[27];
                    OP.Bngwybzjjlj = sValue[28];
                    OP.Bnqybczclj = sValue[29];
                    OP.Jrtcfylj = sValue[30];
                    OP.Jrdbfylj = sValue[31];
                    OP.Jrzffylj = sValue[32];
                    OP.Jrzlfylj = sValue[33];
                    OP.Qfbzlj = sValue[34];
                    OP.Bnzycs = sValue[35];
                    OP.Mzjzrybz = sValue[36];
                    OP.Jzgbbz = sValue[37];
                    OP.Jbylndsykbje = sValue[38];
                    OP.Gwybcylndsykbje = sValue[39];
                    OP.Jtmztczhye = sValue[40];
                    OP.Jdlkrybz = sValue[41];
                    OP.Ydrybz = "0";
                    OP.Bnjzjzclj = "0.00";
                    OP.Bnczjmmztczflj = "0.00";

                    OP.Nl = (DateTime.Now.Year - Convert.ToDateTime(OP.Csrq).Year).ToString();
                    OP.Jbjgbm = YLGHBH;
                    string[] sV1 = { "1", "2", "3" };
                    if (sV1.Contains(OP.Yldylb.Substring(0, 1)))
                        OP.Jflx = "0202"; //职工医保
                    else
                        OP.Jflx = "0203"; //居民医保

                    string strSql = string.Format("delete from YBICKXX where grbh='{0}'", OP.Grbh);
                    liSQL.Add(strSql);
                    #endregion

                    #region 获取慢、特病信息
                    OP.Mtbz = "0";
                    OP.Yllb = "11";
                    //慢病审批查询
                    object[] objParam1 = { OP.Grbh, "16" };
                    objParam1 = YBYLDYSPXXCX(objParam1);
                    if (objParam1[1].ToString().Equals("1"))
                    {
                        OP.Mtbz = "1";
                        OP.Yllb = "13";
                        strSql = string.Format("delete from ybmxbdj where bxh='{0}'", OP.Grbh);
                        liSQL.Add(strSql);
                        string[] sV = objParam1[2].ToString().Split('$');
                        foreach (string s in sV)
                        {
                            string[] sV_temp = s.Split('|');
                            OP.Mtbzbm = sV_temp[0];  //门特病种编码
                            OP.Mtbzmc = sV_temp[1];  //门特病种名称
                            strSql = string.Format(@"select * from ybbzmrdr where dm='{0}'", OP.Mtbzbm);
                            DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                            if (ds.Tables[0].Rows.Count > 0)
                            {
                                OP.Bzlb = ds.Tables[0].Rows[0]["bzlb"].ToString();
                                OP.Yllb = ds.Tables[0].Rows[0]["yllb"].ToString();
                            }
                            //OP.Mtmsg += OP.Bzlb + "\t";
                            OP.Mtmsg += OP.Mtbzbm + "\t" + OP.Mtbzmc + "\r\n";

                            strSql = string.Format(@"insert into  ybmxbdj(BXH,KH,XM,MMBZBM,MMBZMC,YLLB,BZLB) values('{0}','{1}','{2}','{3}','{4}','{5}','{6}') ",
                                                    OP.Grbh, OP.Kh, OP.Xm, OP.Mtbzbm, OP.Mtbzmc, OP.Yllb, OP.Bzlb);
                            liSQL.Add(strSql);
                        }
                    }
                    //特病审批查询
                    object[] objParam2 = { OP.Grbh, "17" };
                    objParam2 = YBYLDYSPXXCX(objParam2);
                    if (objParam2[1].ToString().Equals("1"))
                    {
                        OP.Mtbz = "1";
                        OP.Yllb = "13";
                        strSql = string.Format("delete from ybmxbdj where bxh='{0}'", OP.Grbh);
                        liSQL.Add(strSql);
                        string[] sV = objParam2[2].ToString().Split('$');
                        foreach (string s in sV)
                        {
                            string[] sV_temp = s.Split('|');
                            OP.Mtbzbm = sV_temp[0];  //门特病种编码
                            OP.Mtbzmc = sV_temp[1];  //门特病种名称
                            strSql = string.Format(@"select * from ybbzmrdr where dm='{0}'", OP.Mtbzbm);
                            DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                            if (ds.Tables[0].Rows.Count > 0)
                            {
                                OP.Bzlb = ds.Tables[0].Rows[0]["bzlb"].ToString();
                                OP.Yllb = ds.Tables[0].Rows[0]["yllb"].ToString();
                            }
                            //OP.Mtmsg += OP.Bzlb + "\t";
                            OP.Mtmsg += OP.Mtbzbm + "\t" + OP.Mtbzmc + "\r\n";

                            strSql = string.Format(@"insert into  ybmxbdj(BXH,KH,XM,MMBZBM,MMBZMC,YLLB,BZLB) values('{0}','{1}','{2}','{3}','{4}','{5}','{6}') ",
                                                    OP.Grbh, OP.Kh, OP.Xm, OP.Mtbzbm, OP.Mtbzmc, OP.Yllb, OP.Bzlb);
                            liSQL.Add(strSql);
                        }
                    }

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
                    string outParams = OP.Grbh + "|" + OP.Dwbh + "|" + OP.Sfhz + "|" + OP.Xm + "|" + OP.Xb + "|" +
                       OP.Mz + "|" + OP.Csrq + "|" + OP.Kh + "|" + OP.Yldylb + "|" + OP.Rycbzt + "|" +
                       OP.Ydrybz + "|" + OP.Tcqh + "|" + OP.Nd + "|" + OP.Zyzt + "|" + OP.Zhye + "|" +
                       OP.Bnylflj + "|" + OP.Bnzhzclj + "|" + OP.Bntczclj + "|" + OP.Bndbyljjzflj + "|" + OP.Bngwybzjjlj + "|" +
                       OP.Bnczjmmztczflj + "|" + OP.Jrtcfylj + "|" + OP.Jrdbfylj + "|" + OP.Qfbzlj + "|" + OP.Bnzycs + "|" +
                       OP.Dwmc + "|" + OP.Nl + "|" + OP.Cbdwlx + "|" + OP.Jbjgbm + "|" + OP.Jflx + "|" + OP.Mtbz + "|" + OP.Mtmsg + "|" + OP.Yllb + "|" + OP.Mtbzbm + "|" + OP.Mtbzmc + "|" + YBKLX + "|";

                    strSql = string.Format(@"insert into YBICKXX(
                                            GRBH,DWBH,GMSFHM,XM,XB,MZ,CSRQ,XZZWJB,KH,GRSF,
                                            GBBZ,GWYYLCBBZ,YLRYLB,JMLX,YLDYLB,KZT,DWMC,DQBH,DWLX,JJLX,
                                            GRZHYE,BNYLFLJ,BNXJZCLJ,BNZHZCLJ,BNTCZCLJ,BNDBYLJJZFLJ,BNGWYJJZFLJ,BNQYBCZCLJ,JRTCFYLJ,JRDBFYJLJ,
                                            JRZFFYLJ,JRZLFYLJ,QFBZLJ,ZYCS,MZJZBZ,JZGBBZ,JBYLSYKBXJE,GWYYLSYKBXJE,JTMZTCZHYE,JDLKRYBZ,
                                            SJNL,SYSDATE,DQJBBZ)
                                            VALUES(
                                            '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}',
                                            '{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}',
                                            '{20}','{21}','{22}','{23}','{24}','{25}','{26}','{27}','{28}','{29}',
                                            '{30}','{31}','{32}','{33}','{34}','{35}','{36}','{37}','{38}','{39}',
                                            '{40}','{41}','{42}')",
                                            OP.Grbh, OP.Dwbh, OP.Sfhz, OP.Xm, OP.Xb, OP.Mz, OP.Csrq, OP.Xzzwjb, OP.Kh, OP.Grsf,
                                            OP.Gbbz, OP.Gwybcylbxcbbz, OP.Ylrylb, OP.Jmlx, OP.Yldylb, OP.Rycbzt, OP.Dwmc, OP.Tcqh, OP.Cbdwlx, OP.Jjlx,
                                            OP.Zhye, OP.Bnylflj, OP.Bnxjzclj, OP.Bnzhzclj, OP.Bntczclj, OP.Bndbyljjzflj, OP.Bngwybzjjlj, OP.Bnqybczclj, OP.Jrtcfylj, OP.Jrdbfylj,
                                            OP.Jrzffylj, OP.Jrzlfylj, OP.Qfbzlj, OP.Bnzycs, OP.Mzjzrybz, OP.Jzgbbz, OP.Jbylndsykbje, OP.Gwybcylndsykbje, OP.Jtmztczhye, OP.Jdlkrybz,
                                            OP.Nl, sysdate, DQJBBZ);
                    liSQL.Add(strSql);
                    object[] obj = liSQL.ToArray();
                    obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);
                    if (obj[1].ToString().Equals("1"))
                    {
                        WriteLog(sysdate + "  门诊/住院读卡(市本级)成功|" + outParams);
                        return new object[] { 0, 1, outParams };
                    }
                    else
                    {
                        WriteLog(sysdate + "  门诊/住院读卡(市本级)成功|保存本地数据失败|" + obj[2].ToString());
                        return new object[] { 0, 2, "门诊读卡成功|" + obj[2].ToString() };
                    }
                }
                else
                {
                    WriteLog(sysdate + "  出参|门诊/住院读卡(市本级)失败|" + outputData.ToString());
                    return new object[] { 0, 0, outputData.ToString() };
                }
            }
            catch (Exception ex)
            {
                WriteLog(sysdate + "  系统异常|" + ex.Message);
                return new object[] { 0, 0, "Error:" + ex.Message };
            }
        }
        #endregion

        #region 医保门诊/住院读卡(省内异地)
        public static object[] YBMZZYDK_SNYD(object[] objParam)
        {
            string sysdate = GetServerDateTime();
            try
            {
                List<string> liSQL = new List<string>();
                WriteLog(sysdate + "  进入门诊/住院读卡(省内异地)...");
                CZYBH = CliUtils.fLoginUser; //操作员工号
                YWZQH = CliUtils.fLoginYbNo.Split('|')[0].ToString();    //业务周期号
                string YBKLX = objParam[0].ToString();  //医保卡类型    0:正式卡，1：临时卡
                DQJBBZ = objParam[1].ToString(); //异地标志  1 市本级 2 异地医保
                JYLSH = DateTime.Now.ToString("yyyyMMddHHmmss") + "-" + YLGHBH + "-" + new Random().Next(100).ToString().PadLeft(4, '0');

                //入参
                StringBuilder inputData = new StringBuilder();
                StringBuilder outputData = new StringBuilder(1024);
                //进入本省异地读卡
                YWBH = "7100";
                inputData.Append(YWBH + "^");
                inputData.Append(YLGHBH + "^");
                inputData.Append(CZYBH + "^");
                inputData.Append(YWZQH + "^");
                inputData.Append(JYLSH + "^");
                inputData.Append(ZXBM + "^");
                inputData.Append("^");
                inputData.Append(LJBZ + "^");

                WriteLog(sysdate + "  入参|" + inputData.ToString());

                int i = BUSINESS_HANDLE(inputData, outputData);
                if (i == 0)
                {
                    DQJBBZ = "2";
                    WriteLog(sysdate + "  出参|省内异地读卡成功|" + outputData.ToString().Split('^')[2]);

                    #region 返回参数
                    /*
                     * 2000000003388051|李良淼|401846365|360921|宜春市奉新县|
                     * 1959-09-19|01090032-李良淼|1|71|0.0|
                     * 7265.79|0.0|7265.79|362226195909193336|71|
                     * 390|0|0|1|0|
                     * NULL|NULL|NULL;31^
                     * 1	个人编号	VARHCAR2(20)		
                    2	姓名	VARCHAR2(20)		
                    3	卡号	VARCHAR2(18)		
                    4	地区编号	VARCHAR2(6)		
                    5	地区名称	VARCHAR2(50)		
                    6	出生日期	VARCHAR2(8)		YYYY-MM-DD
                    7	单位名称	VARCHAR2(100)		
                    8	性别	VARCHAR2(10)		名称
                    9	医疗人员类别	VARCHAR2(50)		名称
                    10	账户余额	VARHCAR2(16)		2位小数
                    11	累计住院支付	VARHCAR2(16)		2位小数
                    12	累计门诊支付	VARHCAR2(16)		2位小数
                    13	累计特殊门诊	VARHCAR2(16)		2位小数
                    14	身份号码	VARCHAR2(18)		
                    15	医疗人员类别代码	VARCHAR2(3)		跨省异地返回显示(下面相同)
                    16	险种类型	VARCHAR2(3)		
                    17	参加公务员医疗补助标识	VARCHAR2(3)		
                    18	低保对象标识	VARCHAR2(3)		
                    19	本年度住院次数	VARCHAR2(3)		
                    20	在院状态	VARCHAR2(3)		
                    21	输出附加信息1	VARCHAR2(500)		
                    22	输出附加信息2	VARCHAR2(500)		
                    23	输出附加信息3	VARCHAR2(500)		以下为个人门诊慢性病信息可以为多条以$分割此病种信息作为异地就医门诊慢性病、门诊特殊病上传使用
                    1	医疗类别	VARCHAR2(3)		
                    2	病种编码	VARCHAR2(20)		
                    3	病种名称	VARCHAR2(50)		
                    */
                    string[] sValue = outputData.ToString().Split('^')[2].Split(';');
                    string[] sParam = sValue[0].Split('|');
                    outParams_dk OP = new outParams_dk();
                    OP.Grbh = sParam[0]; //个人编号
                    OP.Xm = sParam[1]; //姓名
                    OP.Kh = sParam[2]; //卡号
                    OP.Tcqh = sParam[3];  //姓名
                    OP.Dwbh = sParam[4];
                    OP.Csrq = sParam[5];
                    OP.Dwmc = sParam[6];
                    OP.Xb = sParam[7];
                    OP.Ylrylb = sParam[8];
                    OP.Zhye = sParam[9];
                    OP.Ljzyzf = sParam[10];
                    OP.Ljmzzf = sParam[11];
                    OP.Ljtsmzzf = sParam[12];
                    OP.Sfhz = sParam[13];
                    OP.Yldylb = sParam[14];
                    OP.Jjlx = sParam[15];
                    OP.Gwybcylbxcbbz = sParam[16];
                    OP.Dbdxbz = sParam[17];
                    OP.Bnzycs = sParam[18];
                    OP.Zyzt = sParam[19];
                    OP.Mz = "01";
                    OP.Rycbzt = "";
                    OP.Bnylflj = "0.00";
                    OP.Bnzhzclj = "0.00";
                    OP.Bntczclj = "0.00";
                    OP.Bndbyljjzflj = "0.00";
                    OP.Bngwybzjjlj = "0.00";
                    OP.Bnczjmmztczflj = "0.00";
                    OP.Jrtcfylj = "0.00";
                    OP.Jrdbfylj = "0.00";
                    OP.Qfbzlj = "0.00";
                    OP.Xzzwjb = "";
                    OP.Grsf = "";
                    OP.Ydrybz = "1";

                    OP.Nl = (DateTime.Now.Year - Convert.ToDateTime(OP.Csrq).Year).ToString();
                    OP.Jbjgbm = YLGHBH;
                    string[] sV1 = { "1", "2", "3" };
                    if (sV1.Contains(OP.Yldylb.Substring(0, 1)))
                        OP.Jflx = "0202"; //职工医保
                    else
                        OP.Jflx = "0203"; //居民医保

                    string strSql = string.Format("delete from YBICKXX where grbh='{0}'", OP.Grbh);
                    liSQL.Add(strSql);

                    #endregion

                    #region 门诊慢、特病信息
                    OP.Mtbz = "0";
                    if (sValue.Length > 1)
                    {
                        string[] sParam1 = sValue[1].Split('|');
                        if (sParam1.Length > 1)
                        {
                            OP.Yllb = "13";
                            OP.Bzlb = "门诊慢性病";
                            OP.Mtbz = "1";
                            strSql = string.Format("delete from ybmxbdj where bxh='{0}'", OP.Grbh);
                            liSQL.Add(strSql);
                            OP.Yllb = sParam1[0].Split('$')[0];
                            OP.Mtbzbm = sParam1[1];
                            OP.Mtbzmc = sParam1[2];
                            strSql = string.Format(@"select * from ybbzmrdr where dm='{0}'", OP.Mtbzbm);
                            DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                            if (ds.Tables[0].Rows.Count > 0)
                            {
                                OP.Bzlb = ds.Tables[0].Rows[0]["bzlb"].ToString();
                                OP.Yllb = ds.Tables[0].Rows[0]["yllb"].ToString();
                            }
                            strSql = string.Format(@"insert into  ybmxbdj(BXH,KH,XM,MMBZBM,MMBZMC,YLLB,BZLB) values('{0}','{1}','{2}','{3}','{4}','{5}','{6}') ",
                                                  OP.Grbh, OP.Kh, OP.Xm, OP.Mtbzbm, OP.Mtbzmc, OP.Yllb, OP.Bzlb);
                            liSQL.Add(strSql);
                            //OP.Mtmsg += OP.Bzlb + "\t";
                            OP.Mtmsg += OP.Mtbzbm + "\t" + OP.Mtbzmc + "\r\n";
                        }
                    }

                    #endregion
                    /*
                     * 个人编号|单位编号|身份证号|姓名|性别|
                     * 民族|出生日期|社会保障卡卡号|医疗待遇类别|人员参保状态|
                     * 异地人员标志|统筹区号|年度|在院状态|帐户余额|
                     * 本年医疗费累计|本年帐户支出累计|本年统筹支出累计|本年救助金支出累计|本年公务员补助基金累计|
                     * 本年城镇居民门诊统筹支付累计|进入统筹费用累计|进入救助金费用累计|起付标准累计|本年住院次数|
                     * 单位名称|年龄|参保单位类型|经办机构编码|缴费类型|
                     * 医保门慢、特资质|医保门慢、特病种说明|医疗类别代码|慢、特病编码|慢、特病名称|医保卡类型
                     */
                    string outParams = OP.Grbh + "|" + OP.Dwbh + "|" + OP.Sfhz + "|" + OP.Xm + "|" + OP.Xb + "|" +
                                       OP.Mz + "|" + OP.Csrq + "|" + OP.Kh + "|" + OP.Yldylb + "|" + OP.Rycbzt + "|" +
                                       OP.Ydrybz + "|" + OP.Tcqh + "|" + OP.Nd + "|" + OP.Zyzt + "|" + OP.Zhye + "|" +
                                       OP.Bnylflj + "|" + OP.Bnzhzclj + "|" + OP.Bntczclj + "|" + OP.Bndbyljjzflj + "|" + OP.Bngwybzjjlj + "|" +
                                       OP.Bnczjmmztczflj + "|" + OP.Jrtcfylj + "|" + OP.Jrdbfylj + "|" + OP.Qfbzlj + "|" + OP.Bnzycs + "|" +
                                       OP.Dwmc + "|" + OP.Nl + "|" + OP.Cbdwlx + "|" + OP.Jbjgbm + "|" + OP.Jflx + "|" + OP.Mtbz + "|" + 
                                       OP.Mtmsg + "|" + OP.Yllb + "|" + OP.Mtbzbm + "|" + OP.Mtbzmc+"|"+YBKLX+"|";

                    WriteLog(sysdate + "  出参|整合后返回|" + outParams);

                    strSql = string.Format(@"insert into YBICKXX(
                                            GRBH,DWBH,GMSFHM,XM,XB,MZ,CSRQ,XZZWJB,KH,GRSF,
                                            GWYYLCBBZ,YLRYLB,YLDYLB,DWMC,DQBH,JJLX,GRZHYE,LJZYF,LJMZF,LJTSMZF,
                                            DBDXBZ,QXMC,SJNL,SYSDATE,DQJBBZ)
                                            VALUES(
                                            '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}',
                                            '{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}',
                                            '{20}','{21}','{22}','{23}','{24}')",
                                            OP.Grbh, OP.Dwbh, OP.Sfhz, OP.Xm, OP.Xb, OP.Mz, OP.Csrq, OP.Xzzwjb, OP.Kh, OP.Grsf,
                                            OP.Gwybcylbxcbbz, OP.Ylrylb,  OP.Yldylb, OP.Dwmc, OP.Tcqh, OP.Jjlx,OP.Zhye, OP.Ljzyzf, OP.Ljmzzf, OP.Ljtsmzzf,
                                            OP.Dbdxbz, OP.Qxmc, OP.Nl, sysdate,DQJBBZ);
                    liSQL.Add(strSql);
                    object[] obj = liSQL.ToArray();
                    obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);
                    if (obj[1].ToString().Equals("1"))
                    {
                        WriteLog(sysdate + "  门诊/住院读卡(省内异地)成功|" + outParams);
                        return new object[] { 0, 1, outParams };
                    }
                    else
                    {
                        WriteLog(sysdate + "  门诊/住院读卡(省内异地)成功|保存本地数据失败|" + obj[2].ToString());
                        return new object[] { 0, 2, "门诊/住院读卡(省内异地)成功|" + obj[2].ToString() };
                    }
                }
                else
                {
                    WriteLog(sysdate + "  门诊/住院读卡(省内异地)失败|" + outputData.ToString());
                    return new object[] { 0, 0, outputData.ToString() };
                }
            }
            catch (Exception ex)
            {
                WriteLog(sysdate + "  省内异地读卡|系统异常|" + ex.Message);
                return new object[] { 0, 0, "Error:" + ex.Message };
            }
        }
        #endregion

        #region 门诊登记
        public static object[] YBMZDJ(object[] objParam)
        {
            //string jzlsh = objParam[0].ToString();  // 就诊流水号
            //string yllb = objParam[1].ToString();   // 医疗类别代码
            //string bzbm = objParam[2].ToString();   // 病种编码（慢性病要传，否则传空字符串）
            //string bzmc = objParam[3].ToString();   // 病种名称（慢性病要传，否则传空字符串）
            string ickxx = objParam[4].ToString();  //医保读卡返回信息
            //string ghsj = objParam[5].ToString();   // 登记时间(格式：DateTime.Now.ToString("yyyyMMddHHmmss"))
            //string cfysdm = objParam[6].ToString(); //处方医生代码
            //string cfysxm = objParam[7].ToString(); //处方医生姓名
            if (string.IsNullOrEmpty(ickxx))
                return new object[] { 0,0,"读卡信息不能为空"};
            DQJBBZ = ickxx.Split('|')[10];

            //判断市本级或省内异地
            if (DQJBBZ.Equals("0"))
                return YBMZDJ_SBJ(objParam);
            else
                return YBMZDJ_SNYD(objParam);

        }
        #endregion

        #region 门诊登记(本市级)
        public static object[] YBMZDJ_SBJ(object[] objParam)
        {
            string sysdate = GetServerDateTime();
            WriteLog(sysdate + "  进入门诊登记(市本级)...");

            try
            {
                #region his参数
                CZYBH = CliUtils.fLoginUser;    //操作员工号
                YWZQH = CliUtils.fLoginYbNo.Split('|')[0].ToString();    //业务周期号
                string jbr = CliUtils.fUserName;   // 经办人姓名 
                string jzlsh = objParam[0].ToString();  // 就诊流水号
                string yllb = objParam[1].ToString();   // 医疗类别代码
                string bzbm = objParam[2].ToString();   // 病种编码（慢性病要传，否则传空字符串）
                string bzmc = objParam[3].ToString();   // 病种名称（慢性病要传，否则传空字符串）
                string ickxx = objParam[4].ToString();  //医保读卡返回信息
                string ghsj = objParam[5].ToString();   // 登记时间(格式：DateTime.Now.ToString("yyyyMMddHHmmss"))
                //string cfysdm = objParam[6].ToString(); //处方医生代码
                //string cfysxm = objParam[7].ToString(); //处方医生姓名
                //DQJBBZ = objParam[8].ToString();  //地区级别标志 1-本市级 2-本省异地  3-其他
                string ysdm = "";   // 医师代码
                string ysxm = "";     // 医师姓名
                string ksbh = "";     // 科室编号
                string ksmc = "";     // 科室名称
                string hzxm = "";   //患者姓名
                string ghdjsj = "";
                #endregion

                if (string.IsNullOrEmpty(jzlsh))
                    return new object[] { 0, 0, "就诊流水号不能为空" };
                if (string.IsNullOrEmpty(yllb))
                    return new object[] { 0, 0, "医疗类别不能为空" };
                if (string.IsNullOrEmpty(ickxx))
                    return new object[] { 0, 0, "读卡信息不能为空" };
                if (string.IsNullOrEmpty(ghsj))
                    return new object[] { 0, 0, "登记时间不能为空" };

                if (yllb == "13")
                {
                    if (string.IsNullOrEmpty(bzbm) || string.IsNullOrEmpty(bzmc))
                        return new object[] { 0, 0, "选择门诊慢性病时，病种必须输入" };
                }

                ghdjsj = Convert.ToDateTime(ghsj).ToString("yyyyMMddHHmmss"); 

                #region 获取医保读卡信息
                string[] ickParam = ickxx.Split('|');
                string grbh = ickParam[0].ToString(); //个人编号
                string dwbm = ickParam[1].ToString();  //单位编号
                string sfzh = ickParam[2].ToString();  //身份证号
                string xm = ickParam[3].ToString();  //姓名
                string xb = ickParam[4].ToString();  //性别
                string kh = ickParam[7].ToString();  //卡号
                string yldylb = ickParam[8].ToString();  //医疗待遇类别
                string ydrybz = ickParam[10].ToString();  //异地人员标志
                string ssqh = ickParam[11].ToString();  //所属区号
                string zhye = ickParam[14].ToString();  //帐户余额
                string ybklx = ickParam[35].ToString(); //医保卡类型
                #endregion

                JYLSH = ghdjsj + "-" + YLGHBH + "-" + new Random().Next(100).ToString().PadLeft(4, '0');
                string strSql = string.Format(@"select m1ghno,m1name,m1ksno,b2ksnm,m1empn,b1name,m1amnt from mz01t  a
                                                left join bz01h b on a.m1empn=b.b1empn
                                                left join bz02h c on a.m1ksno=c.b2ksno
                                                where m1ghno='{0}'", jzlsh);

                DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                if (ds.Tables[0].Rows.Count == 0)
                {
                    WriteLog(sysdate + "  无挂号费用明细");
                    return new object[] { 0, 0, "无挂号费用明细" };
                }

                decimal jegh = 0;
                decimal jezc = 0;

                ysdm = ds.Tables[0].Rows[0]["m1empn"].ToString();
                ysxm = ds.Tables[0].Rows[0]["b1name"].ToString();
                ksbh = ds.Tables[0].Rows[0]["m1ksno"].ToString();
                ksmc = ds.Tables[0].Rows[0]["b2ksnm"].ToString();
                hzxm = ds.Tables[0].Rows[0]["m1name"].ToString();
                DQJBBZ = "1";
                string ybjzlsh = "";
                if (string.IsNullOrEmpty(bzbm))
                {
                    ybjzlsh = "MZ" + yllb + jzlsh;
                }else
                    ybjzlsh = "MZ" + yllb + jzlsh + bzbm;

                if (!hzxm.Equals(xm))
                    return new object[] { 0, 0, "医保卡与患者挂号姓名不一致" };

                //判断是否重复挂号
                strSql = string.Format(@"select * from ybmzzydjdr where ybjzlsh='{0}' and cxbz=1", ybjzlsh);
                ds.Tables.Clear();
                ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    WriteLog(sysdate + "  该患者已经进行医保挂号，请勿重复挂号|");
                    return new object[] { 0, 0, "该患者已经进行医保挂号，请勿重复挂号|" };
                }

                #region 参数
                YWBH = "2210";
                StringBuilder inputParam = new StringBuilder();
                //本市级
                inputParam.Append(ybjzlsh + "|");   //门诊/住院流水号
                inputParam.Append(yllb + "|");      //医疗类别
                inputParam.Append(ghdjsj + "|");      //挂号日期
                inputParam.Append(bzbm + "|");      //诊断疾病编码
                inputParam.Append(bzmc + "|");      //诊断疾病名称
                inputParam.Append("|");             //病历信息
                inputParam.Append(ksmc + "|");             //科室名称
                inputParam.Append("|");             //床位号
                inputParam.Append(ysdm + "|");             //医生代码
                inputParam.Append(ysxm + "|");             //医生姓名
                inputParam.Append(jegh + "|");      //挂号费
                inputParam.Append(jezc + "|");      //检查费
                inputParam.Append(jbr + "|");       //经办人
                inputParam.Append(kh + "|");        //卡号
                inputParam.Append(CliUtils.fComputerName.Trim() + "|");      //个人电脑编号
                #endregion

                //入参
                StringBuilder inputData = new StringBuilder();
                inputData.Append(YWBH + "^");
                inputData.Append(YLGHBH + "^");
                inputData.Append(CZYBH + "^");
                inputData.Append(YWZQH + "^");
                inputData.Append(JYLSH + "^");
                inputData.Append(ZXBM + "^");
                inputData.Append(inputParam.ToString() + "^");
                inputData.Append(LJBZ + "^");

                WriteLog(sysdate + "  门诊登记(市本级)|入参|" + inputData.ToString());
                StringBuilder outputData = new StringBuilder(10240);
                int i = BUSINESS_HANDLE(inputData, outputData);
                if (i == 0)
                {
                    WriteLog(sysdate + "   门诊登记(市本级)|出参|" + outputData.ToString());
                    string strSql1 = string.Format(@"insert into ybmzzydjdr(
                                                    jzlsh,jylsh,ybjzlsh,yllb,ghdjsj,bzbm,bzmc,ksbh,ksmc,ysdm,
                                                    ysxm,ghf,jbr,xm,grbh,kh,yldylb,xb,tcqh,zhye,
                                                    jzbz,ydrybz,dqjbbz,sysdate,ybklx) 
                                                    values(
                                                    '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}',
                                                    '{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}',
                                                    '{20}','{21}','{22}','{23}','{24}')"
                                                    , jzlsh, JYLSH, ybjzlsh, yllb, ghsj, bzbm, bzmc, ksbh, ksmc, ysdm,
                                                    ysxm, jegh, jbr, xm, grbh, kh, yldylb, xb, ssqh, zhye,
                                                    "m", ydrybz, DQJBBZ, sysdate, ybklx);

                    object[] obj = { strSql1 };

                    obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);
                    if (obj[1].ToString() == "1")
                    {
                        WriteLog(sysdate + "   门诊登记(市本级)成功|本地数据操作成功|" + outputData.ToString());
                        return new object[] { 0, 1, "门诊登记挂号成功" };
                    }
                    else
                    {
                        WriteLog(sysdate + "   门诊登记(市本级)成功|本地数据操作失败|" + obj[2].ToString());
                        //门诊登记撤销
                        object[] objParam1 = { jzlsh, jbr, yllb, grbh, xm, kh, ssqh, "", DQJBBZ };
                        NYBMZDJCX(objParam1);
                        return new object[] { 0, 0, "门诊登记(市本级)成功|本地数据操作失败|" + obj[2].ToString() };
                    }

                }
                else
                {
                    WriteLog(sysdate + "   门诊登记(市本级)失败|出参|" + outputData.ToString());
                    return new object[] { 0, 0, outputData.ToString() };
                }
            }
            catch (Exception error)
            {
                WriteLog(sysdate + "   门诊登记(市本级)|接口异常|" + error.Message);
                return new object[] { 0, 2, error.Message };
            }
        }
        #endregion

        #region 门诊登记(省内异地)
        public static object[] YBMZDJ_SNYD(object[] objParam)
        {
            string sysdate = GetServerDateTime();
            WriteLog(sysdate + "  进入门诊登记(省内异地)...");
            try
            {
                #region his参数
                CZYBH = CliUtils.fLoginUser;    //操作员工号
                YWZQH = CliUtils.fLoginYbNo.Split('|')[0].ToString();    //业务周期号
                string jbr = CliUtils.fUserName;    //经办人
                CZYBH = CliUtils.fLoginUser;    //操作员工号
                YWZQH = CliUtils.fLoginYbNo.Split('|')[0].ToString();    //业务周期号

                string jzlsh = objParam[0].ToString();  // 就诊流水号
                string yllb = objParam[1].ToString();   // 医疗类别代码
                string bzbm = objParam[2].ToString();   // 病种编码（慢性病要传，否则传空字符串）
                string bzmc = objParam[3].ToString();   // 病种名称（慢性病要传，否则传空字符串）
                string ickxx = objParam[4].ToString();  //医保读卡返回信息
                string ghsj = objParam[5].ToString();   // 登记时间(格式：DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"))
                //DQJBBZ = objParam[8].ToString();  //地区级别标志 1-本市级 2-本省异地  3-其他
                string ysdm = "";   // 医师代码
                string ysxm = "";     // 医师姓名
                string ksbh = "";     // 科室编号
                string ksmc = "";     // 科室名称
                string hzxm = "";   //患者姓名
                string ghdjsj = "";
                #endregion

                if (string.IsNullOrEmpty(jzlsh))
                    return new object[] { 0, 0, "就诊流水号不能为空" };
                if (string.IsNullOrEmpty(yllb))
                    return new object[] { 0, 0, "医疗类别不能为空" };
                if (string.IsNullOrEmpty(ickxx))
                    return new object[] { 0, 0, "读卡信息不能为空" };
                if (string.IsNullOrEmpty(ghsj))
                    return new object[] { 0, 0, "登记时间不能为空" };

                ghdjsj = Convert.ToDateTime(ghsj).ToString("yyyyMMddHHmmss"); 
                #region 获取医保读卡信息

                string[] ickParam = ickxx.Split('|');
                string grbh = ickParam[0].ToString(); //个人编号
                string dwbm = ickParam[1].ToString();  //单位编号
                string sfzh = ickParam[2].ToString();  //身份证号
                string xm = ickParam[3].ToString();  //姓名
                string xb = ickParam[4].ToString();  //性别
                string kh = ickParam[7].ToString();  //卡号
                string yldylb = ickParam[8].ToString();  //医疗待遇类别
                string ydrybz = ickParam[10].ToString();  //异地人员标志
                string ssqh = ickParam[11].ToString();  //所属区号
                string zhye = ickParam[14].ToString();  //帐户余额
                string ybklx = ickParam[35].ToString();
                #endregion

                JYLSH = ghdjsj + "-" + YLGHBH + "-" + new Random().Next(100).ToString().PadLeft(4, '0');
                string strSql = string.Format(@"select m1ghno,m1name,m1ksno,b2ksnm,m1empn,b1name,m1amnt from mz01t  a
                                                left join bz01h b on a.m1empn=b.b1empn
                                                left join bz02h c on a.m1ksno=c.b2ksno
                                                where m1ghno='{0}'", jzlsh);

                DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                if (ds.Tables[0].Rows.Count == 0)
                {
                    WriteLog(sysdate + "  无挂号费用明细");
                    return new object[] { 0, 0, "无挂号费用明细" };
                }

                decimal jegh = 0;

                ysdm = ds.Tables[0].Rows[0]["m1empn"].ToString();
                ysxm = ds.Tables[0].Rows[0]["b1name"].ToString();
                ksbh = ds.Tables[0].Rows[0]["m1ksno"].ToString();
                ksmc = ds.Tables[0].Rows[0]["b2ksnm"].ToString();
                hzxm = ds.Tables[0].Rows[0]["m1name"].ToString();
                DQJBBZ = "2";

                string ybjzlsh = "";
                if (string.IsNullOrEmpty(bzbm))
                    ybjzlsh = "MZ" + yllb + jzlsh;
                else
                    ybjzlsh = "MZ" + yllb + jzlsh + bzbm;

                    //判断是否重复挂号
                    strSql = string.Format(@"select * from ybmzzydjdr where ybjzlsh='{0}' and cxbz=1", ybjzlsh);
                ds.Tables.Clear();
                ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    WriteLog(sysdate + "  该患者已经进行医保挂号，请勿重复挂号|");
                    return new object[] { 0, 0, "该患者已经进行医保挂号，请勿重复挂号|" };
                }

                #region 参数
                StringBuilder inputParam = new StringBuilder();

                //省内异地
                inputParam.Append(grbh + "|");      //个人编号
                inputParam.Append(xm + "|");        //姓名
                inputParam.Append(kh + "|");        //卡号
                inputParam.Append(ssqh + "|");      //地区编号
                inputParam.Append(yllb + "|");      //医疗类别
                inputParam.Append(ksmc + "|");      //科室名称
                inputParam.Append(jegh + "|");      //挂号费
                inputParam.Append(ghdjsj + "|");      //挂号日期
                inputParam.Append(ybjzlsh + "|");   //门诊/住院流水号
                YWBH = "7110";
                #endregion

                //入参
                StringBuilder inputData = new StringBuilder();
                inputData.Append(YWBH + "^");
                inputData.Append(YLGHBH + "^");
                inputData.Append(CZYBH + "^");
                inputData.Append(YWZQH + "^");
                inputData.Append(JYLSH + "^");
                inputData.Append(ZXBM + "^");
                inputData.Append(inputParam.ToString() + "^");
                inputData.Append(LJBZ + "^");

                WriteLog(sysdate + "  门诊登记(省内异地)|入参|" + inputData.ToString());
                StringBuilder outputData = new StringBuilder(102400);
                int i = BUSINESS_HANDLE(inputData, outputData);

                if (i == 0)
                {
                    WriteLog(sysdate + "   门诊登记(省内异地)|出参|" + outputData.ToString());
                    string[] sValue = outputData.ToString().Split('^')[2].Split('|');
                    string ybjzlsh_snyd = sValue[0];

                    string strSql1 = string.Format(@"insert into ybmzzydjdr(
                                                    jzlsh,jylsh,ybjzlsh,yllb,ghdjsj,bzbm,bzmc,ksbh,ksmc,ysdm,
                                                    ysxm,ghf,jbr,xm,grbh,kh,yldylb,xb,tcqh,zhye,
                                                    jzbz,ydrybz,dqjbbz,ybjzlsh_snyd,sysdate,ybklx) 
                                                    values(
                                                    '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}',
                                                    '{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}',
                                                    '{20}','{21}','{22}','{23}','{24}','{25}')"
                                                    , jzlsh, JYLSH, ybjzlsh, yllb, ghsj, bzbm, bzmc, ksbh, ksmc, ysdm,
                                                    ysxm, jegh, jbr, xm, grbh, kh, yldylb, xb, ssqh, zhye,
                                                    "m", ydrybz, DQJBBZ, ybjzlsh_snyd, sysdate, ybklx);
                    object[] obj = { strSql1 };

                    obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);
                    if (obj[1].ToString() == "1")
                    {
                        WriteLog(sysdate + "   门诊登记(省内异地)成功|本地数据操作成功|" + outputData.ToString());
                        return new object[] { 0, 1, "门诊登记(省内异地)成功" };
                    }
                    else
                    {
                        WriteLog(sysdate + "   门诊登记(省内异地)成功|本地数据操作失败|" + obj[2].ToString());
                        //门诊登记撤销
                        object[] objParam1 = { jzlsh, jbr, yllb, grbh, xm, kh, ssqh, ybjzlsh_snyd, DQJBBZ };
                        NYBMZDJCX(objParam1);
                        return new object[] { 0, 0, "门诊登记失败|本地数据操作失败|" + obj[2].ToString() };
                    }
                }
                else
                {
                    WriteLog(sysdate + "   门诊登记失败|出参|" + outputData.ToString());
                    return new object[] { 0, 0, outputData.ToString() };
                }
            }
            catch (Exception error)
            {
                WriteLog(sysdate + "   门诊登记异常|" + error.Message);
                return new object[] { 0, 2, "门诊登记异常|" + error.Message };
            }
        }
        #endregion

        #region 门诊登记撤销 (本市级/省内异地)
        public static object[] YBMZDJCX(object[] objParam)
        {
            string sysdate = GetServerDateTime();
            WriteLog(sysdate + "  进入门诊登记撤销...");

            CZYBH = CliUtils.fLoginUser;    //操作员工号
            YWZQH = CliUtils.fLoginYbNo.Split('|')[0].ToString();    //业务周期号
            string jbr = CliUtils.fUserName; //操作员姓名
            string jzlsh = objParam[0].ToString();  //就诊流水号
            string yllb = objParam[1].ToString();   //医疗类别

            JYLSH = DateTime.Now.ToString("yyyyMMddHHmmss") + "-" + YLGHBH + "-" + new Random().Next(100).ToString().PadLeft(4, '0');
            try
            {
                string strSql = string.Format(@"select a.* from ybmzzydjdr a where a.jzlsh='{0}' and a.cxbz=1 
                                                and not exists(select 1 from ybfyjsdr b where a.jzlsh=b.jzlsh and b.cxbz=1) ", jzlsh, yllb);
                DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                if (ds.Tables[0].Rows.Count == 0)
                {
                    WriteLog(sysdate + "  该患者无医保门诊挂号登记信息!");
                    return new object[] { 0, 0, "该患者无医保门诊挂号登记信息" };
                }

                List<string> liSQL = new List<string>();

                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    //获取
                   // DataRow dr = ds.Tables[0].Rows[0];
                    string grbh = dr["grbh"].ToString();
                    string xm = dr["xm"].ToString();
                    string kh = dr["kh"].ToString();
                    string dqbh = dr["tcqh"].ToString();
                    string ybjzlsh = dr["ybjzlsh"].ToString();
                    string ybjzlsh_snyd = dr["ybjzlsh_snyd"].ToString();
                    DQJBBZ = ds.Tables[0].Rows[0]["DQJBBZ"].ToString();

                    StringBuilder inputParam = new StringBuilder();

                    if (DQJBBZ.Equals("1")) //本市级
                    {
                        inputParam.Append(ybjzlsh + "|");
                        inputParam.Append(jbr + "|");
                        inputParam.Append(yllb + "|");
                        YWBH = "2240";
                    }
                    else //省内异地
                    {
                        inputParam.Append(grbh + "|");
                        inputParam.Append(xm + "|");
                        inputParam.Append(kh + "|");
                        inputParam.Append(dqbh + "|");
                        inputParam.Append(ybjzlsh_snyd + "|");
                        YWBH = "7150";
                    }

                    StringBuilder inputData = new StringBuilder();
                    inputData.Append(YWBH + "^");
                    inputData.Append(YLGHBH + "^");
                    inputData.Append(CZYBH + "^");
                    inputData.Append(YWZQH + "^");
                    inputData.Append(JYLSH + "^");
                    inputData.Append(ZXBM + "^");
                    inputData.Append(inputParam.ToString() + "^");
                    inputData.Append(LJBZ + "^");
                    WriteLog(sysdate + "  门诊登记撤销|入参|" + inputData.ToString());

                    StringBuilder outputData = new StringBuilder(1024);
                    int i = BUSINESS_HANDLE(inputData, outputData);
                    if (i == 0)
                    {
                        WriteLog(sysdate + "  门诊登记撤销|出参|" + outputData.ToString());

                        strSql = string.Format(@"insert into ybmzzydjdr(jzlsh,jylsh,ybjzlsh,yllb,ghdjsj,bzbm,bzmc,ksbh,ksmc,ysdm,
                                            ysxm,ghf,jbr,xm,grbh,kh,yldylb,xb,tcqh,zhye,
                                            jzbz,ydrybz,dqjbbz,ybjzlsh_snyd,sysdate,cxbz) 
                                            select jzlsh,jylsh,ybjzlsh,yllb,ghdjsj,bzbm,bzmc,ksbh,ksmc,ysdm,
                                            ysxm,ghf,jbr,xm,grbh,kh,yldylb,xb,tcqh,zhye,
                                            jzbz,ydrybz,dqjbbz,ybjzlsh_snyd,'{1}',0 from ybmzzydjdr where ybjzlsh='{0}' and cxbz=1", ybjzlsh, sysdate);
                        liSQL.Add(strSql);
                        strSql = string.Format("update ybmzzydjdr set cxbz = 2 where ybjzlsh = '{0}'  and cxbz=1", ybjzlsh);
                        liSQL.Add(strSql);
                    }
                    else
                    {
                        WriteLog(sysdate + "  门诊登记撤销失败|出参|" + outputData.ToString());
                        return new object[] { 0, 0, outputData.ToString() };
                    }
                }

                object[] obj = liSQL.ToArray();

                obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);

                if (obj[1].ToString() == "1")
                {
                    WriteLog(sysdate + "  门诊登记撤销成功|本地数据操作成功|");
                    return new object[] { 0, 1, "门诊登记撤销成功" };
                }
                else
                {
                    WriteLog(sysdate + "  门诊登记撤销成功|本地数据操作失败|" + obj[2].ToString());
                    return new object[] { 0, 0, "进入门诊登记撤销成功|本地数据操作失败|" + obj[2].ToString() };
                }
            }
            catch (Exception error)
            {
                WriteLog(sysdate + "  门诊登记撤销|接口异常|" + error.Message);
                return new object[] { 0, 2, error.Message };
            }
        }
        #endregion

        #region 门诊登记收费
        public static object[] YBMZDJSF(object[] objParam)
        {
            //string jzlsh = objParam[0].ToString();
            //string yllb = objParam[1].ToString();
            //string jssj = objParam[2].ToString();
            //string bzbm = objParam[3].ToString();
            //string bzmc = objParam[4].ToString();
            string kxx = objParam[5].ToString();
            //string dkbz = objParam[6].ToString();
            //string dgysdm = objParam[7].ToString();     //开方医生代码
            //string dgysxm = objParam[8].ToString();     //开方医生姓名
            //DQJBBZ = objParam[9].ToString();  //地区级别标志 1-本市级 2-本省异地  3-其他

            if (string.IsNullOrEmpty(kxx))
                return new object[] { 0, 0, "读卡信息不能为空" };
            DQJBBZ = kxx.Split('|')[10];

            if (DQJBBZ.Equals("0"))
                return YBMZDJSF_SBJ(objParam);
            else
                return YBMZDJSF_SNYD(objParam);
        }
        #endregion

        #region 门诊登记收费(市本级)
        public static object[] YBMZDJSF_SBJ(object[] objParam)
        {
            string sysdate = GetServerDateTime();
            WriteLog(sysdate + "  进入门诊登记收费(市本级)...");
            try
            {
                #region his参数
                CZYBH = CliUtils.fLoginUser;    //操作员工号
                YWZQH = CliUtils.fLoginYbNo.Split('|')[0].ToString();    //业务周期号
                string jbr = CliUtils.fUserName;   // 经办人姓名 

                string jzlsh = objParam[0].ToString();
                string yllb = objParam[1].ToString();
                string jsrq = objParam[2].ToString();
                string bzbm = objParam[3].ToString();
                string bzmc = objParam[4].ToString();
                string kxx = objParam[5].ToString();
                string dkbz = objParam[6].ToString();
                //string dgysdm = objParam[7].ToString();     //开方医生代码
                //string dgysxm = objParam[8].ToString();     //开方医生姓名
                //DQJBBZ = objParam[9].ToString();  //地区级别标志 1-本市级 2-本省异地  3-其他

                string zhye = kxx.Split('|')[14].ToString(); //帐户余额
                string ghdjsj = "";
                string djh = ""; //发票号
                string sslxdm = "";                         //手术类别
                string grbh = "";        // 个人编号   
                string xm = "";            // 姓名
                string kh = "";            // 卡号
                #endregion

                if (string.IsNullOrEmpty(jzlsh))
                    return new object[] { 0, 0, "就诊流水号不能为空" };
                if (string.IsNullOrEmpty(yllb))
                    return new object[] { 0, 0, "医疗类别不能为空" };


                string ybjzlsh = "";
                if (string.IsNullOrEmpty(bzbm))
                    ybjzlsh = "MZ" + yllb + jzlsh;
                else
                    ybjzlsh = "MZ" + yllb + jzlsh + bzbm;

                string cfmxjylsh = string.Empty;

                ghdjsj = Convert.ToDateTime(jsrq).ToString("yyyyMMddHHmmss");

                //医院交易流水号
                JYLSH = ghdjsj + "-" + YLGHBH + "-" + new Random().Next(100).ToString().PadLeft(4, '0');
                List<string> liSQL = new List<string>();

                #region 医保登记(挂号)
                string strSql = string.Format(@"select * from ybmzzydjdr where ybjzlsh='{0}' and cxbz=1", ybjzlsh);
                DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                if (ds.Tables[0].Rows.Count == 0)
                {
                    object[] objParam1 = { jzlsh, yllb, bzbm, bzmc, kxx, jsrq, "", "" };
                    objParam1 = YBMZDJ_SBJ(objParam1);
                    if (!objParam1[1].ToString().Equals("1"))
                        return objParam1;
                }
                //获取挂号信息
                strSql = string.Format(@"select * from ybmzzydjdr where ybjzlsh='{0}' and cxbz=1", ybjzlsh);
                ds.Tables.Clear();
                ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                DataRow dr = ds.Tables[0].Rows[0];
                grbh = dr["grbh"].ToString();        // 个人编号   
                xm = dr["xm"].ToString();            // 姓名
                kh = dr["kh"].ToString();            // 卡号

                ds.Dispose();
                #endregion

                #region 获取诊查费用信息
                strSql = string.Format(@"select a.m1ghno, z.ybxmbh ybxmbh, z.ybxmmc ybxmmc, c.bzmem1 as dj,1 as sl,a.m1gham je,c.bzmem2 yyxmbh, 
                                                c.bzname yyxmmc, a.m1invo,a.m1blam,a.m1kham,a.m1amnt,z.sfxmzldm,z.sflbdm
                                                from mz01t a           
                                                join bztbd c on a.m1zlfb = c.bzkeyx and c.bzcodn = 'A7' 
                                                left join ybhisdzdr z on c.bzmem2 = z.hisxmbh                
                                                where a.m1ghno = '{0}'", jzlsh);
                ds.Tables.Clear();
                ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                if (ds.Tables[0].Rows.Count == 0)
                    return new object[] { 0, 0, "无费用明细" };

                #region 入参
                /*
                 * 1		住院流水号(门诊流水号)	VARCHAR2(18)	NOT NULL	同登记时的住院流水号（门诊流水号）
                    2		项目类别	VARCHAR2(3)	NOT NULL	代码
                    3		费用类别	VARCHAR2(3)	NOT NULL	代码
                    4		处方号	VARCHAR2(20)	NOT NULL	
                    5		处方日期	VARCHAR2(14)	NOT NULL	YYYYMMDDHH24MISS
                    6		医院收费项目内码	VARCHAR2(20)	NOT NULL	
                    7		收费项目中心编码	VARCHAR2(20)	NOT NULL	中心编号
                    8		医院收费项目名称	VARCHAR2(50)	NOT NULL	
                    9		单价	VARCHAR2(12)	NOT NULL	4位小数
                    10		数量	VARCHAR2(12)	NOT NULL	2位小数
                    11		剂型	VARCHAR2(3)		二级代码
                    12		规格	VARCHAR2(100)		
                    13		每次用量	VARCHAR2(12)		2位小数
                    14		使用频次	VARCHAR2(20)		
                    15		医生姓名	VARCHAR2(20)		传处方医师姓名
                    16		处方医师	VARCHAR2(20)		传处方医师编码
                    17		用法	VARCHAR2(100)		
                    18		单位	VARCHAR2(20)		
                    19		科别名称	VARCHAR2(20)		
                    20		执行天数	VARCHAR2(4)		
                    21		草药单复方标志	VARCHAR2(3)		代码
                    22		经办人	VARCHAR2(20)		医疗机构操作员姓名
                    23		药品剂量单位	VARCHAR2(20)		
                 */
                string xmlb = ds.Tables[0].Rows[0]["sfxmzldm"].ToString();
                string sflb = ds.Tables[0].Rows[0]["sflbdm"].ToString();
                string chf = ghdjsj + "A701";
                string cfrq = ghdjsj;
                string yyxmbh = ds.Tables[0].Rows[0]["yyxmbh"].ToString();
                string ybxmbh = ds.Tables[0].Rows[0]["ybxmbh"].ToString();
                string yyxmmc = ds.Tables[0].Rows[0]["yyxmmc"].ToString();
                string ybxmmc = ds.Tables[0].Rows[0]["ybxmmc"].ToString();
                string dj = ds.Tables[0].Rows[0]["m1amnt"].ToString();
                string sl = ds.Tables[0].Rows[0]["sl"].ToString();
                string je = ds.Tables[0].Rows[0]["m1amnt"].ToString();
                string jx = "";
                string gg = "";
                string ycyl = "";
                string pd = "";
                string ysxm = "";
                string ysdm = "";
                string yf = "";
                string dw = "";
                string ksmc = "";
                string txts = "";
                string zcffbz = "";
                string ypjldw = "";
                djh = ds.Tables[0].Rows[0]["m1invo"].ToString();
                ds.Dispose();
                StringBuilder inputParam = new StringBuilder();
                inputParam.Append(ybjzlsh + "|");
                inputParam.Append(xmlb + "|");
                inputParam.Append(sflb + "|");
                inputParam.Append(chf + "|");
                inputParam.Append(cfrq + "|");
                inputParam.Append(yyxmbh + "|");
                inputParam.Append(ybxmbh + "|");
                inputParam.Append(yyxmmc + "|");
                inputParam.Append(dj + "|");
                inputParam.Append(sl + "|");
                inputParam.Append(jx + "|");
                inputParam.Append(gg + "|");
                inputParam.Append(ycyl + "|");
                inputParam.Append(pd + "|");
                inputParam.Append(ysxm + "|");
                inputParam.Append(ysdm + "|");
                inputParam.Append(yf + "|");
                inputParam.Append(dw + "|");
                inputParam.Append(ksmc + "|");
                inputParam.Append(txts + "|");
                inputParam.Append(zcffbz + "|");
                inputParam.Append(ypjldw + "|");

                StringBuilder inputData = new StringBuilder();
                YWBH = "2310";
                inputData.Append(YWBH + "^");
                inputData.Append(YLGHBH + "^");
                inputData.Append(CZYBH + "^");
                inputData.Append(YWZQH + "^");
                inputData.Append(JYLSH + "^");
                inputData.Append(ZXBM + "^");
                inputData.Append(inputParam.ToString() + "^");
                inputData.Append(LJBZ + "^");

                WriteLog(sysdate + "  门诊登记收费|费用上传|入参|" + inputData.ToString());

                StringBuilder outputData = new StringBuilder(10240);
                int i = BUSINESS_HANDLE(inputData, outputData);
                WriteLog(sysdate + "  门诊登记收费|费用上传|出参|" + outputData.ToString());
                if (i < 0)
                    return new object[] { 0, 0, outputData.ToString().Split('^')[2].ToString() };
                #endregion
                strSql = string.Format(@"insert into ybcfmxscindr(jzlsh,jylsh,xm,kh,ybjzlsh,cfrq,yysfxmbm,yysfxmmc,sfxmzxbm,sfxmzxmc,
                                            dj,sl,je,jbr,sysdate,sflb) values(
                                            '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}',
                                            '{10}','{11}','{12}','{13}','{14}','{15}')",
                                              jzlsh, JYLSH, xm, kh, ybjzlsh, cfrq, yyxmbh, yyxmmc, ybxmbh, ybxmmc,
                                              dj, sl, je, jbr, sysdate, sflb);
                liSQL.Add(strSql);
                string[] cfmx = outputData.ToString().Split('^')[2].Split('|');

                /*
                 金额	VARCHAR2(10)		2位小数
                自理金额	VARCHAR2(10)		2位小数
                自费金额	VARCHAR2(10)		2位小数
                收费项目等级	VARCHAR2(3)		代码
                全额自费标志	VARCHAR2(3)		代码
                限制使用范围标志	VARCHAR2(3)		0—否；1—是，标识此处方使用的药品或诊疗项目是否有限制使用范围
                限制使用范围	VARCHAR2(200)		药品或诊疗项目限制使用范围说明
                 */
                outParams_fymx op = new outParams_fymx();

                op.Je = cfmx[0];
                op.Zlje = cfmx[1];
                op.Zfje = cfmx[2];
                op.Sfxmdj = cfmx[3];
                op.Zfbz = cfmx[4];
                op.Xzsybz = cfmx[5];
                op.Bz = cfmx[6];

                strSql = string.Format(@"insert into ybcfmxscfhdr(jzlsh,jylsh,je,zlje,zfje,sfxmdj,zfbz,xzsybz, bz) 
                                            values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}')",
                                        jzlsh, JYLSH, op.Je, op.Zlje, op.Zfje, op.Sfxmdj, op.Zfbz, op.Xzsybz, op.Bz);
                liSQL.Add(strSql);
                #endregion

                #region 医保结算
                inputParam.Clear();
                /*
                 1		住院流水号(门诊流水号)	VARCHAR2(18)	NOT NULL	同登记时的住院流水号（门诊流水号）
                2		单据号	VARCHAR2(18)	NOT NULL	唯一
                3		医疗类别	VARCHAR2(3)	NOT NULL	代码
                4		结算日期	VARCHAR2(14)	NOT NULL	YYYYMMDDHH24MISS，医院上传数据不能为空
                5		出院日期	VARCHAR2(14)		YYYYMMDDHH24MISS，出院结算不能为空
                6		出院原因	VARCHAR2(3)		代码
                7		出院诊断疾病编码	VARCHAR2(20)		必须为中心病种编码
                8		出院诊断疾病名称	VARCHAR2(50)		
                9		账户使用标志	VARCHAR2(3)	NOT NULL	代码
                10		中途结算标志	VARCHAR2(3)		代码
                11		经办人	VARCHAR2(20)	NOT NULL	医疗机构操作员姓名
                12		手术类型	VARCHAR2(3)		住院医疗类别时不允许为空

                 */
                inputParam.Append(ybjzlsh + "|");
                inputParam.Append(djh + "|");
                inputParam.Append(yllb + "|");
                inputParam.Append(ghdjsj + "|");
                inputParam.Append("|");
                inputParam.Append("|");
                inputParam.Append(bzbm + "|");
                inputParam.Append(bzmc + "|");
                inputParam.Append("0|");
                inputParam.Append("0|");
                inputParam.Append(jbr + "|");
                inputParam.Append(sslxdm + "|");

                inputData.Clear();
                YWBH = "2410";
                inputData.Append(YWBH + "^");
                inputData.Append(YLGHBH + "^");
                inputData.Append(CZYBH + "^");
                inputData.Append(YWZQH + "^");
                inputData.Append(JYLSH + "^");
                inputData.Append(ZXBM + "^");
                inputData.Append(inputParam.ToString() + "^");
                inputData.Append(LJBZ + "^");
                WriteLog(sysdate + "  门诊登记收费|入参|" + inputData.ToString());

                outputData.Clear();
                i = BUSINESS_HANDLE(inputData, outputData);

                if (i == 0)
                {
                    WriteLog(sysdate + "  门诊登记收费成功|出参|" + outputData.ToString());
                    string[] sfjsfh = outputData.ToString().Split('^')[2].Split('|');
                    outParams_js js = new outParams_js();
                    #region 出参
                    /*
                     * 1		医疗费总费用	VARCHAR2(10)		2位小数
                        2		自费费用	VARCHAR2(10)		2位小数
                        3		本次帐户支付	VARCHAR2(10)		2位小数
                        4		统筹支出	VARCHAR2(10)		2位小数
                        5		大病支出	VARCHAR2(10)		2位小数
                        6		公务员补助支付金额	VARCHAR2(10)		2位小数, 如果参保人参加了单位补充医疗保险，此处输出单位补充医疗保险支付金额
                        7		企业补充支付金额	VARCHAR2(10)		2位小数，如果是居民门诊统筹结算，则为本次门诊统筹定额打包费用。
                        8		乙类自理费用	VARCHAR2(10)		2位小数
                        9		特检自理费用	VARCHAR2(10)		2位小数
                        10		特治自理费用	VARCHAR2(10)		2位小数
                        11		符合基本医疗费用	VARCHAR2(10)		2位小数
                        12		起付标准自付	VARCHAR2(10)		2位小数
                        13		进入统筹费用	VARCHAR2(10)		2位小数
                        14		统筹分段自付	VARCHAR2(10)		2位小数
                        15		进入大病费用	VARCHAR2(10)		2位小数
                        16		大病分段自付	VARCHAR2(10)		2位小数
                        17		转诊先自付	VARCHAR2(10)		2位小数
                        18		个人现金支付	VARCHAR2(10)		2位小数
                        19		药品费用	VARCHAR2(10)		2位小数
                        20		诊疗项目费用	VARCHAR2(10)		2位小数
                        21		安装器官费用	VARCHAR2(10)		2位小数
                        22		商业补充保险支付费用	VARCHAR2(10)		2位小数，城居是民政三次救助
                        23		专项救助费用	VARCHAR2(10)		2位小数，城居是民政救助
                        24		单位分担金额	VARCHAR2(10)		2位小数
                        25		门诊诊查费	VARCHAR2(10)		2位小数
                        26		军转干部补助支付	VARCHAR2(10)		2位小数
                        27		基本医疗账户支付	NUMBER(16)		2位小数
                        28		公费医疗账户支付	NUMBER(16)		2位小数
                        29		家庭门诊统筹支付	NUMBER(16)		2位小数
                        30		大病救助支付	NUMBER(16)		2位小数
                        31		生育基金支付	NUMBER(16)		2 位小数
                     */

                    js.Ylfze = sfjsfh[0];         //医疗费总费用
                    js.Zffy = sfjsfh[1];          //自费费用
                    js.Zhzf = sfjsfh[2];          //本次帐户支付
                    js.Tcjjzf = sfjsfh[3];        //统筹支出
                    js.Dejjzf = sfjsfh[4];        //大病支出
                    js.Gwybzjjzf = sfjsfh[5];     //公务员补助支付金额
                    js.Qybcylbxjjzf = sfjsfh[6];  //企业补充支付金额
                    js.Ylzlfy = sfjsfh[7];        //乙类自理费用
                    js.Tjzlfy = sfjsfh[8];        //特检自理费用
                    js.Tzzlfy = sfjsfh[9];        //特治自理费用
                    js.Fhjjylfy = sfjsfh[10];     //符合基本医疗费用
                    js.Qfbzfy = sfjsfh[11];       //起付标准自付
                    js.Jrtcfy = sfjsfh[12];       //进入统筹费用
                    js.Tcfdzffy = sfjsfh[13];     //统筹分段自付
                    js.Jrdebxfy = sfjsfh[14];     //进入大病费用
                    js.Defdzffy = sfjsfh[15];     //大病分段自付
                    js.Zzzyzffy = sfjsfh[16];     //转诊先自付
                    js.Xjzf = sfjsfh[17];         //个人现金支付
                    js.Ypfy = sfjsfh[18];         //药品费用
                    js.Zlxmfy = sfjsfh[19];       //诊疗项目费用
                    js.Rgqgzffy = sfjsfh[20];     //安装器官费用
                    js.Sybxfy = sfjsfh[21];   //商业补充保险支付费用
                    js.Mzjzfy = sfjsfh[22];       //专项救助费用
                    js.Dwfdfy = sfjsfh[23];       //单位分担金额
                    js.Mzzcf = sfjsfh[24];        //门诊诊查费
                    js.Jzgbbzzf = sfjsfh[25];     //军转干部补助支付
                    js.Jbylzhzf = sfjsfh[26];     //基本医疗账户支付
                    js.Gfylzhzf = sfjsfh[27];     //公费医疗账户支付
                    js.Jtmztczf = sfjsfh[28];     //家庭门诊统筹支付
                    js.Dbjzfy = sfjsfh[29];     //大病救助支付
                    js.Yyfdfy = "0.00";
                    js.Cxjfy = "0.00";
                    js.Ylzlfy = "0.00";
                    js.Bcjsqzhye = zhye;
                    js.Jslsh = djh;/*医疗费总额|总报销金额|统筹基金支付|大额基金支付|账户支付|
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
                    * 居民个人自付二次补偿金额|体检金额|生育基金支付|
                    */
                    string zcfbz = "1";
                    //现金支付=本次现金支付+本次大病支出
                    js.Ybxjzf = (Convert.ToDecimal(js.Xjzf) + Convert.ToDecimal(js.Dejjzf)).ToString();
                    //总报销金额=总费用-医保现金支付（包括大病支出）
                    js.Zbxje = (Convert.ToDecimal(js.Ylfze) - Convert.ToDecimal(js.Ybxjzf)).ToString();
                    //其他医保支付=医疗总费用-本次现金支付-本次大病支出-本次统筹支付-本次账户支付
                    js.Qtybzf = (Convert.ToDecimal(js.Ylfze) - Convert.ToDecimal(js.Xjzf) - Convert.ToDecimal(js.Dejjzf) - Convert.ToDecimal(js.Tcjjzf) - Convert.ToDecimal(js.Zhzf)).ToString();
                    
                    
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
                                    js.Jmgrzfecbcje + "|" + js.Tjje + "|" + js.Syjjzf + "|";
                    #endregion
                    strSql = string.Format(@"insert into ybfyjsdr(jzlsh, jylsh ,ylfze ,xjzf ,zhzf ,tcjjzf, dejjzf,gwybzjjzf,qybcylbxjjzf,ylzlfy,
                                                    tjzlfy,tzzlfy,fhjbylfy,qfbzfy,jrtcfy,ctcfdxfy,jrdebxfy,defdzffy,zzzyzffy,zffy,
                                                    ypfy,zlxmfy,rgqgzffy,sybcbxzffy,mzjzfy,dwfdfy,mzzcf,yllb,xm,grbh,
                                                    kh,cfmxjylsh,ywzqh,jslsh,jsrq,djh,ybjzlsh,jzgbbzzf,jbylzhzf,gfylzhzf,
                                                    jtmztczf,dbjzzf,sysdate,jbr,zbxje,bcjsqzhye,zcfbz,qtybfy,zhxjzffy)
                                                    values('{0}','{1}','{2}','{3}','{4}','{5}' ,'{6}','{7}','{8}' ,'{9}' ,
                                                    '{10}','{11}' ,'{12}','{13}' ,'{14}','{15}','{16}' ,'{17}' ,'{18}','{19}',
                                                    '{20}' ,'{21}','{22}','{23}','{24}','{25}','{26}','{27}' ,'{28}','{29}',
                                                    '{30}','{31}','{32}','{33}','{34}','{35}','{36}','{37}','{38}','{39}',
                                                    '{40}','{41}','{42}','{43}','{44}','{45}','{46}','{47}','{48}')",
                                                     jzlsh, JYLSH, js.Ylfze, js.Xjzf, js.Zhzf, js.Tcjjzf, js.Dejjzf, js.Gwybzjjzf, js.Qybcylbxjjzf, js.Ylzlfy,
                                                     js.Tjzlfy, js.Tzzlfy, js.Fhjjylfy, js.Qfbzfy, js.Jrtcfy, js.Tcfdzffy, js.Jrdebxfy, js.Defdzffy, js.Zzzyzffy, js.Zffy,
                                                     js.Ypfy, js.Zlxmfy, js.Rgqgzffy, js.Sybxfy, js.Mzjzfy, js.Dwfdfy, js.Mzzcf, yllb, xm, grbh,
                                                     kh, JYLSH, YWZQH, djh, jsrq, djh, ybjzlsh, js.Jzgbbzzf, js.Jbylzhzf, js.Gfylzhzf,
                                                     js.Jtmztczf, js.Dbjzfy, sysdate, jbr, js.Zbxje, js.Bcjsqzhye, zcfbz, js.Qtybzf, js.Ybxjzf);

                    //string strSql2 = string.Format(" update ybcfmxscfhdr set djh='{0}' where isnull(djh,'')='' and jzlsh='{1}' and grbh='{2}'", djh, jzlsh, grbh);
                    liSQL.Add(strSql);
                    object[] obj = liSQL.ToArray();
                    obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);
                    if (obj[1].ToString() == "1")
                    {
                        WriteLog(sysdate + "  门诊登记收费(市本级)成功|本地数据操作成功|" + outputData.ToString().Split('^')[2]);
                        return new object[] { 0, 1, strValue };
                    }
                    else
                    {
                        WriteLog(sysdate + "  门诊登记收费(市本级)成功|本地数据操作失败|" + obj[2].ToString());
                        //费用结算撤销信息、处方明细上传撤销
                        object[] objFYJSCX = { jzlsh, djh, ybjzlsh, ghdjsj, grbh, xm, kh, "", "", JYLSH, 1 };
                        NYBFYJSCX(objFYJSCX);
                        return new object[] { 0, 0, "数据库操作失败" + obj[2].ToString() };
                    }
                }
                else
                {
                    WriteLog(sysdate + "  门诊登记收费(市本级)失败|" + outputData.ToString());
                    return new object[] { 0, 0, outputData.ToString() };
                }
                #endregion
            }
            catch (Exception error)
            {
                WriteLog(sysdate + "  系统异常:" + error.Message);
                return new object[] { 0, 2, "Error:" + error.Message };
            }
        }
        #endregion

        #region 门诊登记收费(省内异地)
        public static object[] YBMZDJSF_SNYD(object[] objParam)
        {
            string sysdate = GetServerDateTime();
            GocentPara para = new GocentPara();
            WriteLog(sysdate + "  进入门诊登记收费(省内异地)...");
            try
            {
                string mzfy = para.sys_parameter("MZ", "MZ0021"); //门诊挂号费上传，如果是0则传实际值，如果是其他则传其他设定的值

                CZYBH = CliUtils.fLoginUser;      // 操作员工号
                YWZQH = CliUtils.fLoginYbNo.Split('|')[0].ToString();      // 业务周期号

                string jzlsh = objParam[0].ToString();
                string yllb = objParam[1].ToString();
                string jssj = objParam[2].ToString();
                string bzbm = objParam[3].ToString();
                string bzmc = objParam[4].ToString();
                string kxx = objParam[5].ToString();
                string dkbz = objParam[6].ToString();
                //string dgysdm = objParam[7].ToString();     //开方医生代码
                //string dgysxm = objParam[8].ToString();     //开方医生姓名
                //DQJBBZ = objParam[9].ToString();  //地区级别标志 1-本市级 2-本省异地  3-其他
                string jbr = CliUtils.fUserName;        // 经办人姓名
                string djh = "";        // 单据号
                string cyyy = "";       // 出院原因代码 
                string zhsybz = "1";     // 账户使用标志（0或1）
                string zhye = kxx.Split('|')[14].ToString(); //帐户余额
                string strSql;
                string sslxdm = string.Empty;    //手术类型代码  

                string dqrq = DateTime.Now.ToString("yyyyMMddHHmmss");  // 当前日期
                string cyrq = dqrq;

                string ybjzlsh = "";
                if (string.IsNullOrEmpty(bzbm))
                    ybjzlsh = "MZ" + yllb + jzlsh;
                else
                    ybjzlsh = "MZ" + yllb + jzlsh + bzbm;
                string cfmxjylsh = string.Empty;

                JYLSH = dqrq + "-" + YLGHBH + "-" + new Random().Next(100).ToString().PadLeft(4, '0');
                List<string> liSQL = new List<string>();

                #region 判断是否进行登记
                strSql = string.Format("select * from ybmzzydjdr a where a.ybjzlsh = '{0}' and a.jzbz='m'  and a.cxbz = 1", ybjzlsh);
                DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                if (ds == null || ds.Tables[0].Rows.Count == 0)
                {
                    object[] objParam1 = { jzlsh, yllb, bzbm, bzmc, kxx, jssj, "", "" };
                    objParam1 = YBMZDJ_SNYD(objParam1);
                    if (!objParam1[1].ToString().Equals("1"))
                        return objParam1;
                }

                strSql = string.Format("select * from ybmzzydjdr a where a.ybjzlsh = '{0}' and a.jzbz='m'  and a.cxbz = 1", ybjzlsh);
                ds.Tables[0].Clear();
                ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                DataRow dr1 = ds.Tables[0].Rows[0];
                //string bzbm = dr["bzbm"].ToString();        // 病种编码
                //string bzmc = dr["bzmc"].ToString();        // 病种名称
                string grbh = dr1["grbh"].ToString();        // 个人编号   
                string xm = dr1["xm"].ToString();            // 姓名
                string kh = dr1["kh"].ToString();            // 卡号
                string dqbh = dr1["tcqh"].ToString();
                string ghsj = dr1["ghdjsj"].ToString();
                string ysxm = dr1["ysxm"].ToString();
                string ybjzlsh_snyd = dr1["ybjzlsh_snyd"].ToString();
                #endregion

                #region 获取挂号信息
                strSql = string.Format(@"select a.m1ghno, z.ybxmbh ybxmbh, z.ybxmmc ybxmmc, c.bzmem1 as dj,1 as sl,a.m1gham je,c.bzmem2 yyxmbh, 
                                        c.bzname yyxmmc, a.m1invo,a.m1blam,a.m1kham,a.m1amnt,z.sfxmzldm,z.sflbdm
                                        from mz01t a           
                                        join bztbd c on a.m1zlfb = c.bzkeyx and c.bzcodn = 'A7' 
                                        left join ybhisdzdr z on c.bzmem2 = z.hisxmbh                
                                        where a.m1ghno = '{0}'", jzlsh);
                ds.Tables.Clear();
                ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                if (ds.Tables[0].Rows.Count == 0)
                {
                    WriteLog(sysdate + "  门诊登记预收费|无挂号信息");
                    return new object[] { 0, 0, "无挂号费用明细" };
                }

                DataRow dr = ds.Tables[0].Rows[0];

                if (dr["ybxmbh"] == DBNull.Value)
                    return new object[] { 0, 0, "项目代码：[" + dr["yyxmbh"].ToString() + "]，名称：[" + dr["yyxmmc"].ToString() + "]未对照，不能上传！" };
                /*
                1	收费项目中心编码	VARCHAR2(20)	NOT NULL	代码，中心系统规定的收费项目
                2	项目名称	VARCHAR2(100)	NOT NULL	
                3	单价	NUMBER(8,2)	NOT NULL	
                4	数量	NUMBER(8,2)	NOT NULL	
                5	金额	NUMBER(8,2)	NOT NULL	
                6	处方日期	VARCHAR2(14)	NOT NULL	YYYYMMDDHH24MISS
                7	医院收费项目内码	VARCHAR2(20)	NOT NULL	
                8	医院收费项目名称	VARCHAR2(50)	NOT NULL	
                 */
                string sflb = dr["sflbdm"].ToString();
                string ybxmbh = dr["ybxmbh"].ToString();
                string ybxmmc = dr["ybxmmc"].ToString();
                string dj = dr["m1amnt"].ToString();
                string sl = dr["sl"].ToString();
                decimal je = Convert.ToDecimal(dr["m1amnt"].ToString());
                string cfrq = dqrq;
                string yyxmbh = dr["yyxmbh"].ToString();
                string yyxmmc = dr["yyxmmc"].ToString();
                djh = dr["m1invo"].ToString();
                if (je <= 0)//jegh == 0 && 
                {
                    return new object[] { 0, 0, "挂号诊查费不能为0" };
                }

                #endregion

                #region 门诊收费结算

                #region 参数

                /*
                 1	保险号	VARCHAR2(10)	NOT NULL	
                2	姓名	VARCHAR2(20)	NOT NULL	
                3	卡号	VARCHAR2(18)	NOT NULL	
                4	地区编号	VARCHAR2(6)	NOT NULL	
                5	门诊号	VARCHAR2(18)	NOT NULL	
                6	病种编号	VARCHAR2(20)		为门诊挂号时返回的个人慢性病审批数据包，如果没有病种信息,则用“”代替。
                7	病种名称	VARCHAR2(100)		
                8	开方医生	VARCHAR2(20)	NOT NULL	
                9	是否打印票据	VARCHAR2(8)		值为1或0，做为预留，暂不使用。以下为费用明细，这两个参数之间以$符分割
                1	收费项目中心编码	VARCHAR2(20)	NOT NULL	代码，中心系统规定的收费项目
                2	项目名称	VARCHAR2(100)	NOT NULL	
                3	单价	NUMBER(8,2)	NOT NULL	
                4	数量	NUMBER(8,2)	NOT NULL	
                5	金额	NUMBER(8,2)	NOT NULL	
                6	处方日期	VARCHAR2(14)	NOT NULL	YYYYMMDDHH24MISS
                7	医院收费项目内码	VARCHAR2(20)	NOT NULL	
                8	医院收费项目名称	VARCHAR2(50)	NOT NULL	
                 */

                StringBuilder inputParam = new StringBuilder();
                inputParam.Append(grbh + "|");
                inputParam.Append(xm + "|");
                inputParam.Append(kh + "|");
                inputParam.Append(dqbh + "|");
                inputParam.Append(ybjzlsh_snyd + "|");
                inputParam.Append(bzbm + "|");
                inputParam.Append(bzmc + "|");
                inputParam.Append(ysxm + "|");
                inputParam.Append("0" + "|$");

                inputParam.Append(ybxmbh + "|");
                inputParam.Append(ybxmmc + "|");
                inputParam.Append(dj + "|");
                inputParam.Append(sl + "|");
                inputParam.Append(je + "|");
                inputParam.Append(cfrq + "|");
                inputParam.Append(yyxmbh + "|");
                inputParam.Append(yyxmmc + "|");
                
                #endregion

                YWBH = "7130";
                StringBuilder inputData = new StringBuilder();
                inputData.Append(YWBH + "^");
                inputData.Append(YLGHBH + "^");
                inputData.Append(CZYBH + "^");
                inputData.Append(YWZQH + "^");
                inputData.Append(JYLSH + "^");
                inputData.Append(ZXBM + "^");
                inputData.Append(inputParam.ToString() + "^");
                inputData.Append(LJBZ + "^");

                WriteLog(sysdate + "  门诊登记收费结算(省内异地)|入参|" + inputData.ToString());
                StringBuilder outputData = new StringBuilder(102400);
                int i = BUSINESS_HANDLE(inputData, outputData);

                if (i == 0)//处方上传成功
                {
                    WriteLog(sysdate + "  门诊登记收费结算(省内异地)|出参|" + outputData.ToString());
                    string[] zysfdjfh = outputData.ToString().Split('^')[2].Split('|');
                    #region 出参
                    /*
                     1	医疗总费用	VARCHAR2(10)		2位小数
                    2	本次账户支付	VARCHAR2(10)		2位小数
                    3	本次现金支付	VARCHAR2(10)		2位小数
                    4	本次基金支付	VARCHAR2(10)		2位小数
                    5	大病基金支付	VARCHAR2(10)		2位小数
                    6	救助金额	VARCHAR2(10)		2位小数
                    7	公务员补助支付	VARCHAR2(10)		2位小数
                    8	甲类费用	VARCHAR2(10)		2位小数
                    9	乙类费用	VARCHAR2(10)		2位小数
                    10	丙类费用	VARCHAR2(10)		2位小数
                    11	自费费用	VARCHAR2(10)		2位小数
                    12	起付标准自付	VARCHAR2(10)		2位小数
                    13	非医保自付	VARCHAR2(10)		2位小数
                    14	乙类药品自付	VARCHAR2(10)		2位小数
                    15	特检特治自付	VARCHAR2(10)		2位小数
                    16	进入统筹自付	VARCHAR2(10)		2位小数
                    17	进入大病自付	VARCHAR2(10)		2位小数
                    18	门诊流水号	VARCHAR2(18)	NOT NULL	
                    19	单据流水号	VARCHAR2(18)	NOT NULL	
                     */
                    //出参
                    outParams_js js=new outParams_js();
                    js.Ylfze = zysfdjfh[0];         //医疗总费用
                    js.Zhzf = zysfdjfh[1];          //本次帐户支付
                    js.Xjzf = zysfdjfh[2];          //本次现金支付
                    js.Tcjjzf = zysfdjfh[3];        //本次统筹支付
                    js.Dejjzf = zysfdjfh[4];        //大病基金支付
                    js.Mzjzfy = zysfdjfh[5];        //救助金额
                    js.Gwybzjjzf = zysfdjfh[6];     //公务员补助支付
                    js.Jlfy = zysfdjfh[7];          //甲类费用
                    js.Ylfy = zysfdjfh[8];          //乙类费用
                    js.Blfy = zysfdjfh[9];          //丙类费用

                    js.Zffy = zysfdjfh[10];         //自费费用
                    js.Qfbzfy = zysfdjfh[11];       //起付标准自付
                    js.Fybzf = zysfdjfh[12];        //非医保自付
                    js.Ylzlfy = zysfdjfh[13];       //乙类药品自付
                    js.Tjzlfy = zysfdjfh[14];       //特检特治自付

                    js.Tcfdzffy = zysfdjfh[15];     //进入统筹自付(统筹分段自付)
                    js.Defdzffy = zysfdjfh[16];     //进入大病自付(大病分段自付)
                    js.Ybmzlsh = zysfdjfh[17];        //门诊流水号
                    js.Jslsh = zysfdjfh[18];        //返回的单据号
                    js.Zbxje = (Convert.ToDecimal(js.Ylfze) - Convert.ToDecimal(js.Xjzf)).ToString();

                    js.Bcjsqzhye = zhye;

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
                       * 居民个人自付二次补偿金额|体检金额|生育基金支付|
                       */

                    //现金支付=本次现金支付+本次大病支出
                    js.Ybxjzf = (Convert.ToDecimal(js.Xjzf) + Convert.ToDecimal(js.Dejjzf)).ToString();
                    //总报销金额=总费用-医保现金支付（包括大病支出）
                    js.Zbxje = (Convert.ToDecimal(js.Ylfze) - Convert.ToDecimal(js.Ybxjzf)).ToString();
                    //其他医保支付=医疗总费用-本次现金支付-本次大病支出-本次统筹支付-本次账户支付
                    js.Qtybzf = (Convert.ToDecimal(js.Ylfze) - Convert.ToDecimal(js.Xjzf) - Convert.ToDecimal(js.Dejjzf) - Convert.ToDecimal(js.Tcjjzf) - Convert.ToDecimal(js.Zhzf)).ToString();
                    
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
                                    js.Jmgrzfecbcje + "|" + js.Tjje + "|" + js.Syjjzf + "|";

                    WriteLog(sysdate + "  门诊登记收费结算(省内异地)|出参1|" + outputData.ToString());
                    #endregion
                    string zcfbz = "1";
                    strSql = string.Format(@"insert into ybfyjsdr(
                                             jzlsh, jylsh ,ylfze,zhzf,xjzf,tcjjzf,dejjzf, mzjzfy,gwybzjjzf,jlfy,
                                             ylfy,blfy,zffy,qfbzfy,fybzf,ylzlfy,tjzlfy,tcfdzffy,defdzffy,yllb,
                                             xm,grbh,kh,cfmxjylsh,ywzqh,djh,ybjzlsh,jslsh,ybmzlsh,jbr,
                                             zbxje,bcjsqzhye,jsrq,zcfbz,qtybfy,zhxjzffy)
                                             values('{0}','{1}','{2}','{3}','{4}','{5}' ,'{6}','{7}','{8}' ,'{9}' ,
                                             '{10}','{11}' ,'{12}','{13}' ,'{14}','{15}','{16}' ,'{17}' ,'{18}','{19}',
                                             '{20}' ,'{21}', '{22}','{23}','{24}','{25}','{26}','{27}','{28}','{29}',
                                             '{30}' ,'{31}','{32}','{33}','{34}','{35}')",
                                             jzlsh, JYLSH, js.Ylfze, js.Zhzf, js.Xjzf, js.Tcjjzf, js.Dejjzf, js.Mzjzfy, js.Gwybzjjzf, js.Jlfy,
                                             js.Ylfy, js.Blfy, js.Zffy, js.Qfbzfy, js.Fybzf, js.Ylzlfy, js.Tjzlfy, js.Tcfdzffy, js.Defdzffy, yllb,
                                             xm, grbh, kh, JYLSH, YWZQH, djh, ybjzlsh, js.Jslsh, js.Ybmzlsh, jbr,
                                             js.Zbxje, js.Bcjsqzhye, jssj, zcfbz, js.Qtybzf, js.Ybxjzf);
                    liSQL.Add(strSql);
                    strSql = string.Format(@"insert into ybcfmxscindr(jzlsh,jylsh,xm,kh,ybjzlsh,cfrq,yysfxmbm,yysfxmmc,sfxmzxbm,sfxmzxmc,
                                            dj,sl,je,jbr,grbh,sflb) values(
                                            '{0}','{1}','{2}','{3}','{4}','{5}' ,'{6}','{7}','{8}' ,'{9}' ,
                                            '{10}','{11}' ,'{12}','{13}' ,'{14}','{15}' )",
                                            jzlsh, JYLSH, xm, kh, ybjzlsh_snyd, cfrq, yyxmbh, yyxmmc, ybxmbh, ybxmmc,
                                            dj, sl, je, jbr, grbh, sflb);
                    liSQL.Add(strSql);
                   
                    object[] obj_GHSF = liSQL.ToArray();
                    obj_GHSF = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj_GHSF);

                    if (obj_GHSF[1].ToString() == "1")
                    {
                        WriteLog(sysdate + "  门诊登记收费结算成功|本地数据操作成功|" + strValue);
                        return new object[] { 0, 1, strValue };
                    }
                    else
                    {
                        WriteLog(sysdate + "  门诊登记收费结算成功|本地数据操作失败|" + obj_GHSF[2].ToString());
                        //撤销操作
                        //费用结算撤销信息、处方明细上传撤销
                        object[] objFYJSCX = { jzlsh, djh, ybjzlsh, dqrq, grbh, xm, kh, dqbh, ybjzlsh_snyd, JYLSH, 2 };
                        NYBFYJSCX(objFYJSCX);
                        return new object[] { 0, 0, obj_GHSF[2].ToString() };
                    }
                }
                else
                {
                    WriteLog(sysdate + "  门诊收费预结算失败|" + outputData.ToString());
                    return new object[] { 0, 0, outputData.ToString() };
                }

                #endregion
            }
            catch (Exception ex)
            {
                WriteLog(sysdate + "  门诊登记收费结算|系统异常|" + ex.Message);
                return new object[] { 0, 0, "系统异常|" + ex.Message };
            }
        }
        #endregion

        #region 门诊登记收费撤销
        public static object YBMZDJSFCX(object[] objParam)
        {
            string sysdate = GetServerDateTime();
            WriteLog(sysdate + "  进入门诊登记收费撤销...");
            try
            {
                string jzlsh = objParam[0].ToString();
                string dkbz = objParam[1].ToString(); //读卡标志
                string djh = objParam[2].ToString();
                string jbr = CliUtils.fUserName;
                string yllb = "";
                string strSql = string.Format(@"select b.yllb from ybfyjsdr a 
                                                inner join ybmzzydjdr b on a.jzlsh=b.jzlsh and a.ybjzlsh=b.ybjzlsh and a.cxbz=b.cxbz 
                                                where a.jzlsh='{0}' and a.djh='{1}' and a.cxbz=1",jzlsh,djh);
                DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                if (ds.Tables[0].Rows.Count == 0)
                {
                    strSql = string.Format(@"select b.yllb from ybmzzydjdr b where b.jzlsh='{0}' and b.cxbz=1", jzlsh);
                    ds.Tables.Clear();
                    ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                    yllb = ds.Tables[0].Rows[0]["yllb"].ToString();
                    object[] objDJCX = { jzlsh, yllb };
                    objDJCX = YBMZDJCX(objDJCX);
                    if (objDJCX[1].ToString().Equals("1"))
                        return new object[] { 0, 1, "门诊登记收费撤销成功" };
                    else
                        return objDJCX;
                }
                else
                {
                    yllb = ds.Tables[0].Rows[0]["yllb"].ToString();
                    object[] objJSCX = { jzlsh, djh };

                    objJSCX = YBMZSFJSCX(objJSCX);
                    if (objJSCX[1].ToString().Equals("1"))
                    {
                        object[] objDJCX = { jzlsh, yllb };
                        objDJCX = YBMZDJCX(objDJCX);
                        if (objDJCX[1].ToString().Equals("1"))
                            return new object[] { 0, 1, "门诊登记收费撤销成功" };
                        else
                            return objDJCX;
                    }
                    else
                    {
                        return objJSCX;
                    }
                }
            }
            catch (Exception ex)
            {
                WriteLog(sysdate + "  门诊登记收费撤销|接口异常|" + ex.Message);
                return new object[] { 0, 0, ex.Message };
            }
        }
        #endregion

        #region 门诊处方明细上传(市本级)
        public static object[] YBMZCFMXSC(object[] objParam)
        {
            string sysdate = GetServerDateTime();
            WriteLog(sysdate + "  进入门诊入方明细上传(市本级)...");

            try
            {
                #region 入参
                string jzlsh = objParam[0].ToString();  //就诊流水号
                string cfhs = objParam[1].ToString();   //处方号
                string ybjzlsh = objParam[2].ToString(); //医保就诊流水号
                string ylfze = objParam[3].ToString();  //总费用
                string cfysdm = objParam[4].ToString(); //处方医生代码
                string cfysxm = objParam[5].ToString(); //处方医生姓名
                #endregion

                CZYBH = CliUtils.fLoginUser;  //操作员工号
                YWZQH = CliUtils.fLoginYbNo.Split('|')[0].ToString();  //业务周期号
                string jbr = CliUtils.fUserName;    //经办人姓名
                string cfsj = DateTime.Now.ToString("yyyyMMddHHmmss");  //处方时间
                JYLSH = cfsj + "-" + YLGHBH + "-" + new Random().Next(100).ToString().PadLeft(4, '0');

                decimal sfje = Convert.ToDecimal(ylfze);

                //判断是否医保登记
                string strSql = string.Format("select * from ybmzzydjdr a where a.ybjzlsh = '{0}' and a.cxbz = 1", ybjzlsh);
                DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                if (ds == null || ds.Tables[0].Rows.Count == 0)
                {
                    WriteLog(sysdate + "  无医保门诊挂号信息");
                    return new object[] { 0, 0, "无医保门诊挂号信息" };
                }

                DataRow dr1 = ds.Tables[0].Rows[0];
                string grbh = dr1["grbh"].ToString();
                string xm = dr1["xm"].ToString();  //姓名
                string kh = dr1["kh"].ToString();  //卡号

                StringBuilder inputParam = new StringBuilder();

                #region 获取处方明细信息
                strSql = string.Format(@"select y.ybxmbh, y.ybxmmc, m.dj, sum(m.sl) sl, sum(m.je) je, m.yyxmbh, m.yyxmmc, y.sfxmzldm, y.sflbdm,y.jxdm, m.cfh,m.gg,m.jxdw,y.jx,m.yf from 
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
                                        group by y.ybxmbh, y.ybxmmc, m.dj, m.yyxmbh, m.yyxmmc, y.sfxmzldm, y.sflbdm,y.jxdm, m.cfh,m.gg,m.jxdw,y.jx,m.yf", jzlsh, cfhs);
                ds.Tables.Clear();
                ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                if (ds.Tables[0].Rows.Count == 0)
                {
                    WriteLog(sysdate + "  无费用明细");
                    return new object[] { 0, 0, "无费用明细" };
                }

                DataTable dt = ds.Tables[0];
                StringBuilder wdzxms = new StringBuilder();
                List<string> liSQL = new List<string>();
                List<string> liyyxmbh = new List<string>();
                List<string> liyyxmmc = new List<string>();
                List<string> liybxmbm = new List<string>();
                List<string> liybxmmc = new List<string>();
                decimal sfje_total = 0;
                for (int k = 0; k < dt.Rows.Count; k++)
                {
                    DataRow dr = dt.Rows[k];

                    if (dr["ybxmbh"] == DBNull.Value)
                    {
                        wdzxms.Append("项目代码：[" + dr["yyxmbh"].ToString() + "]，名称：[" + dr["yyxmmc"].ToString() + "]未对照，不能上传！");
                    }
                    else
                    {
                        /*1		住院流水号(门诊流水号)	VARCHAR2(18)	NOT NULL	同登记时的住院流水号（门诊流水号）
                        2		项目类别	VARCHAR2(3)	NOT NULL	代码
                        3		费用类别	VARCHAR2(3)	NOT NULL	代码
                        4		处方号	VARCHAR2(20)	NOT NULL	
                        5		处方日期	VARCHAR2(14)	NOT NULL	YYYYMMDDHH24MISS
                        6		医院收费项目内码	VARCHAR2(20)	NOT NULL	
                        7		收费项目中心编码	VARCHAR2(20)	NOT NULL	中心编号
                        8		医院收费项目名称	VARCHAR2(50)	NOT NULL	
                        9		单价	VARCHAR2(12)	NOT NULL	4位小数
                        10		数量	VARCHAR2(12)	NOT NULL	2位小数
                        11		剂型	VARCHAR2(3)		二级代码
                        12		规格	VARCHAR2(100)		
                        13		每次用量	VARCHAR2(12)		2位小数
                        14		使用频次	VARCHAR2(20)		
                        15		医生姓名	VARCHAR2(20)		传处方医师姓名
                        16		处方医师	VARCHAR2(20)		传处方医师编码
                        17		用法	VARCHAR2(100)		
                        18		单位	VARCHAR2(20)		
                        19		科别名称	VARCHAR2(20)		
                        20		执行天数	VARCHAR2(4)		
                        21		草药单复方标志	VARCHAR2(3)		代码
                        22		经办人	VARCHAR2(20)		医疗机构操作员姓名
                        23		药品剂量单位	VARCHAR2(20)		
                         */
                        string ybsfxmzldm = dr["sfxmzldm"].ToString();  //收费项目等级代码
                        string ybsflbdm = dr["sflbdm"].ToString();      //收费类别代码
                        string yyxmbh = dr["yyxmbh"].ToString();          //检查项目代码
                        string ybxmbh = dr["ybxmbh"].ToString();          //医保项目编号
                        string ybxmmc = dr["ybxmmc"].ToString();          //医保项目名称
                        string yyxmmc = dr["yyxmmc"].ToString();          //项目名称
                        decimal dj = Convert.ToDecimal(dr["dj"]);
                        decimal sl = Convert.ToDecimal(dr["sl"]);
                        decimal je = Convert.ToDecimal(dr["je"]);
                        sfje_total += je;
                        string jx = dr["jxdm"].ToString(); 
                        string gg = "";//dr["gg"].ToString();
                        decimal mcyl = 1;
                        //string ysbm = dr["ysdm"].ToString();
                        //string ysxm = dr["ysxm"].ToString();
                        string ksdm = "";
                        string ksmc = "";
                        string cfh = dr["cfh"].ToString();
                        string ybcfh = cfsj + k.ToString();
                        liyyxmbh.Add(yyxmbh);
                        liyyxmmc.Add(yyxmmc);
                        liybxmbm.Add(ybxmbh);
                        liybxmmc.Add(ybxmmc);


                        string ypjldw = "";

                        //if (dr["sfno"].ToString() == "01" || dr["sfno"].ToString() == "02" || dr["sfno"].ToString() == "03")
                        //{
                        //    ypjldw = "粒";
                        //}

                        // 1、住院流水号(门诊流水号) | 2、项目类别 | 3、费用类别 | 4、处方号 | 5、处方日期
                        //6、医院收费项目内码|7、收费项目中心编码|8、医院收费项目名称|9、单价|10、数量
                        //11、剂型|12、规格|13、每次用量|14、使用频次|15、医生姓名
                        //16、处方医师|17、用法|18、单位|19、科别名称|20、执行天数
                        //21、草药单复方标志|22、经办人|23、药品剂量单位
                        inputParam.Append(ybjzlsh + "|");
                        inputParam.Append(ybsfxmzldm + "|");
                        inputParam.Append(ybsflbdm + "|");
                        inputParam.Append(ybcfh + "|");
                        inputParam.Append(cfsj + "|");
                        inputParam.Append(yyxmbh + "|");
                        inputParam.Append(ybxmbh + "|");
                        inputParam.Append(yyxmmc + "|");
                        inputParam.Append(dj + "|");
                        inputParam.Append(sl + "|");
                        inputParam.Append(jx + "|");
                        inputParam.Append(gg + "|");
                        inputParam.Append("|");
                        inputParam.Append("" + "|");    //使用频次
                        inputParam.Append(cfysxm + "|");
                        inputParam.Append(cfysdm + "|");
                        inputParam.Append("" + "|");    //用法
                        inputParam.Append("" + "|");    //单位
                        inputParam.Append(ksmc + "|");
                        inputParam.Append("" + "|");
                        inputParam.Append("1" + "|");
                        inputParam.Append(jbr + "|");
                        inputParam.Append(ypjldw + "|$");

                        strSql = string.Format(@"insert into ybcfmxscindr(jzlsh,jylsh,xm,kh,ybjzlsh,cfrq,yysfxmbm,yysfxmmc,sfxmzxbm,sfxmzxmc,
                                            dj,sl,je,jbr,sysdate,sflb,ybcfh) values(
                                            '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}',
                                            '{10}','{11}','{12}','{13}','{14}','{15}','{16}')",
                                            jzlsh, JYLSH, xm, kh, ybjzlsh, cfsj, yyxmbh, yyxmmc, ybxmbh, ybxmmc,
                                            dj, sl, je, jbr, sysdate, ybsflbdm, cfh);
                        liSQL.Add(strSql);
                    }
                }

                if (wdzxms.Length > 0)
                {
                    return new object[] { 0, 0, wdzxms.ToString() };
                }

                if (Math.Abs(sfje_total - sfje) > 1)
                    return new object[] { 0, 0, "收费金额与医保结算金额相差" + Math.Abs(sfje_total - sfje) + ",无法结算，请核实!" };

                #endregion

                YWBH = "2310";
                StringBuilder inputData = new StringBuilder();
                inputData.Append(YWBH + "^");
                inputData.Append(YLGHBH + "^");
                inputData.Append(CZYBH + "^");
                inputData.Append(YWZQH + "^");
                inputData.Append(JYLSH + "^");
                inputData.Append(ZXBM + "^");
                inputData.Append(inputParam.ToString().TrimEnd('$') + "^");
                inputData.Append(LJBZ + "^");

                WriteLog(sysdate + "  门诊入方明细上传(市本级)|入参|" + inputData.ToString());
                StringBuilder outputData = new StringBuilder(1024000);
                int i = BUSINESS_HANDLE(inputData, outputData);

                if (i == 0)
                {

                    WriteLog(sysdate + "  门诊入方明细上传(市本级)|出参|" + outputData.ToString());
                    string[] zysfdjfhs = outputData.ToString().Split('^')[2].TrimEnd('$').Split('$');
                    List<string> lizysfdjfh = new List<string>();

                    for (int j = 0; j < zysfdjfhs.Length; j++)
                    {
                        //金额 、 自理金额 、 自费金额 、 收费项目等级 、 全额自费标志 、 限制使用范围标志 、 限制使用范围
                        string[] zysfdjfh = zysfdjfhs[j].Split('|');
                        outParams_fymx op = new outParams_fymx();
                        op.Je = zysfdjfh[0];
                        op.Zlje = zysfdjfh[1];
                        op.Zfje = zysfdjfh[2];
                        op.Sfxmdj = zysfdjfh[3];
                        op.Zfbz = zysfdjfh[4];
                        op.Xzsybz = zysfdjfh[5];
                        op.Bz = zysfdjfh[6];

                        strSql = string.Format(@"insert into ybcfmxscfhdr(jzlsh,jylsh,je,zlje,zfje,sfxmdj,zfbz,xzsybz, bz,ybjzlsh,
                                                yyxmdm,yyxmmc,yybxmbh,ybxmmc) 
                                                values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}',
                                                '{10}','{11}','{12}','{13}')",
                                                jzlsh, JYLSH, op.Je, op.Zlje, op.Zfje, op.Sfxmdj, op.Zfbz, op.Xzsybz, op.Bz, ybjzlsh,
                                                liyyxmbh[j], liyyxmmc[j], liybxmbm[j], liybxmmc[j]);
                        liSQL.Add(strSql);
                    }

                    object[] obj = liSQL.ToArray();
                    obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);

                    if (obj[1].ToString() == "1")
                    {
                        WriteLog(sysdate + "    门诊入方明细上传(市本级)成功|本地数据操作成功|" + outputData.ToString());
                        return new object[] { 0, 1, JYLSH };
                    }
                    else
                    {
                        WriteLog(sysdate + "    门诊入方明细上传(市本级)成功|本地数据操作失败|" + obj[2].ToString());
                        object[] objFYMXCX = { jzlsh, JYLSH, ybjzlsh };
                        NYBMZCFMXCX(objFYMXCX);
                        return new object[] { 0, 0, "上传处方明细失败|" + obj[2].ToString() };
                    }
                }
                else
                {
                    WriteLog(sysdate + "  门诊入方明细上传(市本级)失败" + outputData.ToString());
                    return new object[] { 0, 0, outputData.ToString() };
                }
            }
            catch (Exception error)
            {
                WriteLog(sysdate + "  门诊入方明细上传(市本级)成功|系统异常|" + error.Message);
                return new object[] { 0, 2, "Error:" + error.Message };
            }
        }
        #endregion

        #region 门诊处方明细撤销(市本级)
        public static object[] YBMZCFMXSCCX(object[] objParam)
        {
            string sysdate = GetServerDateTime();
            WriteLog(sysdate + "   进入门诊费用登记撤销...");
            try
            {
                CZYBH = CliUtils.fLoginUser;  // 操作员工号
                YWZQH = CliUtils.fLoginYbNo.Split('|')[0].ToString();  // 业务周期号
                string jbr = CliUtils.fUserName;    // 经办人姓名
                string ybjzlsh = objParam[0].ToString();  // 医保就诊流水号
                string cxjylsh = objParam[1].ToString();  // 撤销交易流水号(全部撤销则传空字符串)

                JYLSH = DateTime.Now.ToString("yyyyMMddHHmmss") + "-" + YLGHBH + "-" + new Random().Next(100).ToString().PadLeft(4, '0');

                // 1、就诊流水号 | 2、被撤销交易流水号（如果只撤销一部分，则此值不为空） | 3、经办人
                YWBH = "2320";
                StringBuilder inputParam = new StringBuilder();
                inputParam.Append(ybjzlsh + "|");
                inputParam.Append( "|");
                inputParam.Append(jbr + "|");

                StringBuilder inputData = new StringBuilder();
                inputData.Append(YWBH + "^");
                inputData.Append(YLGHBH + "^");
                inputData.Append(CZYBH + "^");
                inputData.Append(YWZQH + "^");
                inputData.Append(JYLSH + "^");
                inputData.Append(ZXBM + "^");
                inputData.Append(inputParam.ToString() + "^");
                inputData.Append(LJBZ + "^");

                List<string> liSQL = new List<string>();
                WriteLog(sysdate + "  门诊费用登记撤销|入参|" + inputData.ToString());
                StringBuilder outputData = new StringBuilder(10240);
                int i = BUSINESS_HANDLE(inputData, outputData);
                if (i == 0)
                {
                    WriteLog(sysdate + "  门诊费用登记撤销|出参|" + outputData.ToString());
                    string strSql = string.Format(@"insert into ybcfmxscindr(jzlsh,jylsh,xm,kh,ybjzlsh,cfrq,yysfxmbm,yysfxmmc,sfxmzxbm,sfxmzxmc,
                                            dj,sl,je,jbr,cxbz,sysdate) select 
                                            jzlsh,jylsh,xm,kh,ybjzlsh,cfrq,yysfxmbm,yysfxmmc,sfxmzxbm,sfxmzxmc,
                                            dj,sl,je,jbr,0,'{1}' from ybcfmxscindr 
                                            where ybjzlsh = '{0}' and cxbz = 1", ybjzlsh, sysdate);
                        strSql += string.Format(" and jylsh = '{0}'", cxjylsh);

                    liSQL.Add(strSql);

                    strSql = string.Format("update ybcfmxscindr set cxbz = 2 where ybjzlsh = '{0}' and cxbz = 1", ybjzlsh);
                    
                        strSql += string.Format(" and jylsh = '{0}'", cxjylsh);
                    liSQL.Add(strSql);

                    strSql = string.Format(@"insert into ybcfmxscfhdr(jzlsh,jylsh,je,zlje,zfje,sfxmdj,zfbz,xzsybz,bz,cxbz,sysdate) 
                                            select jzlsh,jylsh,je,zlje,zfje,sfxmdj,zfbz,xzsybz,bz,0,'{1}' from ybcfmxscfhdr 
                                            where ybjzlsh = '{0}' and cxbz = 1", ybjzlsh, sysdate);
                        strSql += string.Format(" and jylsh = '{0}'", cxjylsh);

                    liSQL.Add(strSql);

                    strSql = string.Format("update ybcfmxscfhdr set cxbz = 2 where ybjzlsh = '{0}' and cxbz = 1", ybjzlsh);
                        strSql += string.Format(" and jylsh = '{0}'", cxjylsh);
                    liSQL.Add(strSql);

                    object[] obj = liSQL.ToArray();

                    obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);

                    if (obj[1].ToString() == "1")
                    {
                        WriteLog(sysdate + "  门诊费用登记撤销成功|本地数据操作成功|" + outputData.ToString());
                        return new object[] { 0, 1, outputData.ToString() };
                    }
                    else
                    {
                        WriteLog(sysdate + "  门诊费用登记撤销成功|本地数据操作失败|" + obj[2].ToString());
                        return new object[] { 0, 0, "数据库操作失败" + obj[2].ToString() };
                    }
                }
                else
                {
                    WriteLog(sysdate + "  门诊费用登记撤销失败|" + outputData.ToString());
                    return new object[] { 0, 0, outputData.ToString() };
                }

            }
            catch (Exception ex)
            {
                WriteLog(sysdate + "  系统异常|" + ex.Message);
                return new object[] { 0, 2, "Error:" + ex.Message };
            }
        }
        #endregion

        #region 门诊上传费用查询
        public static object[] YBMZCFMXFHCX(object[] objParam)
        {
            string sysdate = GetServerDateTime();
            WriteLog(sysdate + "   进入门诊上传费用查询...");
            try
            {
                string jzlsh = objParam[0].ToString(); //就诊流水号
                string yllb = objParam[1].ToString();   //医疗类别
                string bzbh = objParam[2].ToString();   //病种编号
                if (string.IsNullOrEmpty(jzlsh))
                    return new object[] { 0, 0, "就诊流水号不能为空" };
                if (string.IsNullOrEmpty(yllb))
                    return new object[] { 0, 0, "医疗类别不能为空" };

                string ybjzlsh = "";
                if (string.IsNullOrEmpty(bzbh))
                    ybjzlsh = "MZ" + yllb + jzlsh;
                else
                    ybjzlsh = "MZ" + yllb + jzlsh + bzbh;

                Frm_YBMZCFFHCX frm = new Frm_YBMZCFFHCX(ybjzlsh);
                frm.ShowDialog();
                return new object[] { 0, 1, "门诊上传费用查询" };
            }
            catch (Exception ex)
            {
                WriteLog(sysdate + "  系统异常|" + ex.Message);
                return new object[] { 0, 2, "Error:" + ex.Message };
            }
        }
        #endregion

        #region 门诊费用预结算
        public static object[] YBMZSFYJS(object[] objParam)
        {
            #region 入参
            CZYBH = CliUtils.fLoginUser;  //操作员工号
            YWZQH = CliUtils.fLoginYbNo.Split('|')[0].ToString();  //业务周期号
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

            string ybjzlsh = "";
            if (string.IsNullOrEmpty(bzbh))
                ybjzlsh = "MZ" + yllb + jzlsh;
            else
                ybjzlsh = "MZ" + yllb + jzlsh + bzbh;
            //判断是否进行登记
            string strSql = string.Format("select * from ybmzzydjdr a where a.ybjzlsh = '{0}' and a.jzbz='m'  and a.cxbz = 1", ybjzlsh);
            WriteLog(strSql);
            DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            if (ds == null || ds.Tables[0].Rows.Count == 0)
                return new object[] { 0, 0, "就诊流水号：" + jzlsh + "无挂号或入院登记记录" };
            DataRow dr = ds.Tables[0].Rows[0];
            DQJBBZ = dr["dqjbbz"].ToString();           //市本级或省内异地标志
            ds.Dispose();

            //判断市本级或省内异地
            if (DQJBBZ.Equals("1"))
                return YBMZFYYJS_BSJ(objParam);
            else
                return YBMZFYYJS_SNYD(objParam);
        }
        #endregion

        #region 门诊费用预结算(市本级)
        public static object[] YBMZFYYJS_BSJ(object[] objParam)
        {
            string sysdate = GetServerDateTime();
            WriteLog(sysdate + "  进入门诊费用预结算(市本级)...");

            try
            {
                CZYBH = CliUtils.fLoginUser;  //操作员工号
                YWZQH = CliUtils.fLoginYbNo.Split('|')[0].ToString();  //业务周期号
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
                string strSql;
                string sslxdm = "0";    //手术类型代码  
                string cfmxjylsh = "";
                string djh = "0000";    //单据号    //结算单据号 门诊以12位时间+流水号后5位作为单据号
                DataSet ds = null;

                if (string.IsNullOrEmpty(jzlsh))
                    return new object[] { 0, 0, "就诊流水号不能为空" };
                if (string.IsNullOrEmpty(yllb))
                    return new object[] { 0, 0, "医疗类别不能为空" };

                string ybjzlsh = "";
                if (string.IsNullOrEmpty(bzbh))
                    ybjzlsh = "MZ" + yllb + jzlsh;
                else
                    ybjzlsh = "MZ" + yllb + jzlsh + bzbh;
                string jsrq1 = Convert.ToDateTime(jsrq).ToString("yyyyMMddHHmmss");
                JYLSH = jsrq + "-" + YLGHBH + "-" + new Random().Next(100).ToString().PadLeft(4, '0');
                
                //是否进行医保登记
                strSql = string.Format("select * from ybmzzydjdr a where a.ybjzlsh = '{0}'  and a.jzbz='m' and a.cxbz = 1", ybjzlsh);
                ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);

                if (ds == null || ds.Tables[0].Rows.Count == 0)
                    return new object[] { 0, 0, "就诊流水号" + jzlsh + "无挂号或入院登记记录" };
                DataRow dr = ds.Tables[0].Rows[0];
                bzbh = dr["bzbm"].ToString();
                bzmc = dr["bzmc"].ToString();
                string grbh = dr["grbh"].ToString();
                string kh = dr["kh"].ToString();
                string xm = dr["xm"].ToString();

                strSql = string.Format("select GRZHYE from ybickxx where grbh='{0}'", grbh);
                ds.Tables.Clear();
                ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                if (ds.Tables[0].Rows.Count == 0)
                    return new object[] { 0, 0, "获取医保信息出错" };
                string zhye = ds.Tables[0].Rows[0]["GRZHYE"].ToString();

                strSql = string.Format(@"select a.jylsh from ybcfmxscindr a  
                                        where jylsh not in(select cfmxjylsh from ybfyjsdr where ybjzlsh=a.ybjzlsh and cxbz=1) 
                                        and a.ybjzlsh='{0}' and a.cxbz=1", ybjzlsh);
                ds.Tables.Clear();
                ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                if (ds.Tables[0].Rows.Count > 0)
                    cfmxjylsh = ds.Tables[0].Rows[0]["jylsh"].ToString();
                ds.Dispose();


                #region 费用上传
                //撤销处方
                object[] objFYMXCX = { ybjzlsh, cfmxjylsh };
                YBMZCFMXSCCX(objFYMXCX);
                //上传处方
                object[] objFYMXSC = { jzlsh, cfhs, ybjzlsh, sfje, cfysdm, cfysxm };
                objFYMXSC = YBMZCFMXSC(objFYMXSC);
                if (objFYMXSC[1].ToString().Equals("1"))
                    cfmxjylsh = objFYMXSC[2].ToString();
                else
                    return objFYMXSC;
                #endregion

                #region 入参
                YWBH = "2420";
                /*
                 1		住院流水号(门诊流水号)	VARCHAR2(18)	NOT NULL	同登记时的住院流水号（门诊流水号）
                2		单据号	VARCHAR2(18)		医院结算的单据号不能为空
                3		医疗类别	VARCHAR2(3)		代码
                4		结算日期	VARCHAR2(14)	NOT NULL	YYYYMMDDHH24MISS，医院上传数据不能为空
                5		出院日期	VARCHAR2(14)		NULL
                6		出院原因	VARCHAR2(3)		NULL
                7		出院诊断疾病编码	VARCHAR2(20)		NULL
                8		出院诊断疾病名称	VARCHAR2(50)		NULL
                9		账户使用标志	VARCHAR2(3)	NOT NULL	代码
                10		中途结算标志	VARCHAR2(3)		代码
                11		经办人	VARCHAR2(20)	NOT NULL	医疗机构操作员姓名
                12		手术类型	VARCHAR2(3)		住院医疗类别时不允许为空
                13		胎儿数	VARCHAR2(3)		生育刷卡直补传入
                 */
                StringBuilder inputParam = new StringBuilder();
                inputParam.Append(ybjzlsh + "|");
                inputParam.Append(djh + "|");
                inputParam.Append(yllb + "|");
                inputParam.Append(jsrq1 + "|");
                inputParam.Append("|");
                inputParam.Append("|");
                inputParam.Append(bzbh + "|");
                inputParam.Append(bzmc + "|");
                inputParam.Append(zhsybz + "|");
                inputParam.Append("0" + "|");
                inputParam.Append(jbr + "|"); //经办人
                inputParam.Append(sslxdm + "|"); //手术类型
                inputParam.Append("" + "|");    //胎儿数

                StringBuilder inputData = new StringBuilder();
                inputData.Append(YWBH + "^");
                inputData.Append(YLGHBH + "^");
                inputData.Append(CZYBH + "^");
                inputData.Append(YWZQH + "^");
                inputData.Append(JYLSH + "^");
                inputData.Append(ZXBM + "^");
                inputData.Append(inputParam.ToString() + "^");
                inputData.Append(LJBZ + "^");

                WriteLog(sysdate + "  门诊费用预结算(市本级)|" + inputData.ToString());
                #endregion

                StringBuilder outputData = new StringBuilder(10240);
                int i = BUSINESS_HANDLE(inputData, outputData);
                List<string> lisql = new List<string>();
                if (i == 0)
                {
                    WriteLog(sysdate + "  费用预结算(市本级)成功|出参|" + outputData.ToString());
                    string[] sfjsfh = outputData.ToString().Split('^')[2].Split('|');
                    #region 出参
                    outParams_js js = new outParams_js();

                    /*
                     * 1		医疗费总费用	VARCHAR2(10)		2位小数
                        2		自费费用	VARCHAR2(10)		2位小数
                        3		本次帐户支付	VARCHAR2(10)		2位小数
                        4		统筹支出	VARCHAR2(10)		2位小数
                        5		大病支出	VARCHAR2(10)		2位小数
                        6		公务员补助支付金额	VARCHAR2(10)		2位小数, 如果参保人参加了单位补充医疗保险，此处输出单位补充医疗保险支付金额
                        7		企业补充支付金额	VARCHAR2(10)		2位小数，如果是居民门诊统筹结算，则为本次门诊统筹定额打包费用。
                        8		乙类自理费用	VARCHAR2(10)		2位小数
                        9		特检自理费用	VARCHAR2(10)		2位小数
                        10		特治自理费用	VARCHAR2(10)		2位小数
                        11		符合基本医疗费用	VARCHAR2(10)		2位小数
                        12		起付标准自付	VARCHAR2(10)		2位小数
                        13		进入统筹费用	VARCHAR2(10)		2位小数
                        14		统筹分段自付	VARCHAR2(10)		2位小数
                        15		进入大病费用	VARCHAR2(10)		2位小数
                        16		大病分段自付	VARCHAR2(10)		2位小数
                        17		转诊先自付	VARCHAR2(10)		2位小数
                        18		个人现金支付	VARCHAR2(10)		2位小数
                        19		药品费用	VARCHAR2(10)		2位小数
                        20		诊疗项目费用	VARCHAR2(10)		2位小数
                        21		安装器官费用	VARCHAR2(10)		2位小数
                        22		商业补充保险支付费用	VARCHAR2(10)		2位小数，城居是民政三次救助
                        23		专项救助费用	VARCHAR2(10)		2位小数，城居是民政救助
                        24		单位分担金额	VARCHAR2(10)		2位小数
                        25		门诊诊查费	VARCHAR2(10)		2位小数
                        26		军转干部补助支付	VARCHAR2(10)		2位小数
                        27		基本医疗账户支付	NUMBER(16)		2位小数
                        28		公费医疗账户支付	NUMBER(16)		2位小数
                        29		家庭门诊统筹支付	NUMBER(16)		2位小数
                        30		大病救助支付	NUMBER(16)		2位小数
                        31		生育基金支付	NUMBER(16)		2 位小数
                     */

                    js.Ylfze = sfjsfh[0];         //医疗费总费用
                    js.Zffy = sfjsfh[1];          //自费费用
                    js.Zhzf = sfjsfh[2];          //本次帐户支付
                    js.Tcjjzf = sfjsfh[3];        //统筹支出
                    js.Dejjzf = sfjsfh[4];        //大病支出
                    js.Gwybzjjzf = sfjsfh[5];     //公务员补助支付金额
                    js.Qybcylbxjjzf = sfjsfh[6];  //企业补充支付金额
                    js.Ylzlfy = sfjsfh[7];        //乙类自理费用
                    js.Tjzlfy = sfjsfh[8];        //特检自理费用
                    js.Tzzlfy = sfjsfh[9];        //特治自理费用
                    js.Fhjjylfy = sfjsfh[10];     //符合基本医疗费用
                    js.Qfbzfy = sfjsfh[11];       //起付标准自付
                    js.Jrtcfy = sfjsfh[12];       //进入统筹费用
                    js.Tcfdzffy = sfjsfh[13];     //统筹分段自付
                    js.Jrdebxfy = sfjsfh[14];     //进入大病费用
                    js.Defdzffy = sfjsfh[15];     //大病分段自付
                    js.Zzzyzffy = sfjsfh[16];     //转诊先自付
                    js.Xjzf = sfjsfh[17];         //个人现金支付
                    js.Ypfy = sfjsfh[18];         //药品费用
                    js.Zlxmfy = sfjsfh[19];       //诊疗项目费用
                    js.Rgqgzffy = sfjsfh[20];     //安装器官费用
                    js.Sybxfy = sfjsfh[21];   //商业补充保险支付费用
                    js.Mzjzfy = sfjsfh[22];       //专项救助费用
                    js.Dwfdfy = sfjsfh[23];       //单位分担金额
                    js.Mzzcf = sfjsfh[24];        //门诊诊查费
                    js.Jzgbbzzf = sfjsfh[25];     //军转干部补助支付
                    js.Jbylzhzf = sfjsfh[26];     //基本医疗账户支付
                    js.Gfylzhzf = sfjsfh[27];     //公费医疗账户支付
                    js.Jtmztczf = sfjsfh[28];     //家庭门诊统筹支付
                    js.Dbjzfy = sfjsfh[29];     //大病救助支付
                    js.Yyfdfy = "0.00";
                    js.Cxjfy = "0.00";
                    js.Ylzlfy = "0.00";
                    js.Bcjsqzhye = zhye;
                    js.Jslsh = djh;
                    //js.Bcjshzhye = (Convert.ToDecimal(zhye) - Convert.ToDecimal(js.Zhzf)).ToString();

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
                    //总报销金额=医疗总费用-本次现金支付-本次大病支出
                    js.Zbxje = (Convert.ToDecimal(js.Ylfze) - Convert.ToDecimal(js.Xjzf) - Convert.ToDecimal(js.Dejjzf)).ToString();
                    //其他医保支付=医疗总费用-本次现金支付-本次大病支出-本次统筹支付-本次账户支付
                    js.Qtybzf = (Convert.ToDecimal(js.Ylfze) - Convert.ToDecimal(js.Xjzf) - Convert.ToDecimal(js.Dejjzf) - Convert.ToDecimal(js.Tcjjzf) - Convert.ToDecimal(js.Zhzf)).ToString();
                    //现金支付=本次现金支付+本次大病支出
                    js.Ybxjzf = (Convert.ToDecimal(js.Xjzf) + Convert.ToDecimal(js.Dejjzf)).ToString();


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
                    lisql.Add(strSql);
                    strSql = string.Format(@"insert into ybfyyjsdr(jzlsh, jylsh ,ylfze ,xjzf ,zhzf ,tcjjzf, dejjzf,gwybzjjzf,qybcylbxjjzf,ylzlfy,
                                            tjzlfy,tzzlfy,fhjbylfy,qfbzfy,jrtcfy,ctcfdxfy,jrdebxfy,defdzffy,zzzyzffy,zffy,
                                            ypfy,zlxmfy,rgqgzffy,sybcbxzffy,mzjzfy,dwfdfy,mzzcf,yllb,xm,grbh,
                                            kh,cfmxjylsh,ywzqh,jslsh,jsrq,djh,ybjzlsh,jzgbbzzf,jbylzhzf,gfylzhzf,
                                            jtmztczf,dbjzzf,sysdate,jbr,zbxje,bcjsqzhye,qtybfy,zhxjzffy)
                                            values('{0}','{1}','{2}','{3}','{4}','{5}' ,'{6}','{7}','{8}' ,'{9}' ,
                                            '{10}','{11}' ,'{12}','{13}' ,'{14}','{15}','{16}' ,'{17}' ,'{18}','{19}',
                                            '{20}' ,'{21}','{22}','{23}','{24}','{25}','{26}','{27}' ,'{28}','{29}',
                                            '{30}','{31}','{32}','{33}','{34}','{35}','{36}','{37}','{38}','{39}',
                                            '{40}','{41}','{42}','{43}','{44}','{45}','{46}','{47}')",
                                            jzlsh, JYLSH, js.Ylfze, js.Xjzf, js.Zhzf, js.Tcjjzf, js.Dejjzf, js.Gwybzjjzf, js.Qybcylbxjjzf, js.Ylzlfy,
                                            js.Tjzlfy, js.Tzzlfy, js.Fhjjylfy, js.Qfbzfy, js.Jrtcfy, js.Tcfdzffy, js.Jrdebxfy, js.Defdzffy, js.Zzzyzffy, js.Zffy,
                                            js.Ypfy, js.Zlxmfy, js.Rgqgzffy, js.Sybxfy, js.Mzjzfy, js.Dwfdfy, js.Mzzcf, yllb, xm, grbh,
                                            kh, cfmxjylsh, YWZQH, djh, jsrq1, djh, ybjzlsh, js.Jzgbbzzf, js.Jbylzhzf, js.Gfylzhzf,
                                            js.Jtmztczf, js.Dbjzfy, sysdate, jbr, js.Zbxje, js.Bcjsqzhye,js.Qtybzf,js.Ybxjzf);
                    lisql.Add(strSql);
                    object[] obj = lisql.ToArray();
                    obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);
                    if (obj[1].ToString() == "1")
                    {
                        WriteLog(sysdate + "  费用预结算(市本级)|本地操作成功|" + strValue);
                        return new object[] { 0, 1, strValue };
                    }
                    else
                    {
                        WriteLog(sysdate + "  费用预结算(市本级)|本地操作失败|" + obj[2].ToString());
                        return new object[] { 0, 0, "费用预结算(市本级)失败" };
                    }
                }
                else
                {
                    WriteLog(sysdate + "  费用预结算(市本级)失败|" + outputData.ToString());
                    return new object[] { 0, 0, outputData.ToString() };
                }
            }
            catch (Exception error)
            {
                WriteLog(sysdate + "  系统异常|" + error.Message);
                return new object[] { 0, 2, error.Message };
            }
        }
        #endregion

        #region 门诊费用预结算(省内异地)
        public static object[] YBMZFYYJS_SNYD(object[] objParam)
        {
            string sysdate = GetServerDateTime();
            WriteLog(sysdate + "  进入门诊费用预结算...");
            try
            {
                CZYBH = CliUtils.fLoginUser;  //操作员工号
                YWZQH = CliUtils.fLoginYbNo.Split('|')[0].ToString();  //业务周期号
                string jbr = CliUtils.fUserName;    //经办人姓名

                string jzlsh = objParam[0].ToString();  //就诊流水号
                string zhsybz = objParam[1].ToString(); //账户使用标志
                string jsrq = objParam[2].ToString();   //结算时间
                string bzbm = objParam[3].ToString();   //病种编号
                string bzmc = objParam[4].ToString();   //病种名称
                string cfhs = objParam[5].ToString();   //处方号集
                string yllb = objParam[6].ToString();   //医疗类别
                string sfje1 = objParam[7].ToString();   //收费金额
                string cfysdm = objParam[8].ToString(); //处方医生代码
                string cfysxm = objParam[9].ToString(); //处方医生姓名

                decimal sfje = Convert.ToDecimal(sfje1);
                string ybjzlsh = "";
                if (string.IsNullOrEmpty(bzbm))
                    ybjzlsh = "MZ" + yllb + jzlsh;
                else
                    ybjzlsh = "MZ" + yllb + jzlsh + bzbm;
                string cfsj = Convert.ToDateTime(jsrq).ToString("yyyyMMddHHmmss");  //处方时间
                string cyrq = cfsj;                                     //出院日期
                string djh = "0000";
                JYLSH = cfsj + "-" + YLGHBH + "-" + new Random().Next(100).ToString().PadLeft(4, '0');

                StringBuilder inputParam = new StringBuilder();

                #region 获取医保门诊登记信息
                string strSql = string.Format("select * from ybmzzydjdr a where a.ybjzlsh = '{0}' and a.cxbz = 1", ybjzlsh);
                DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);

                if (ds == null || ds.Tables[0].Rows.Count == 0)
                    return new object[] { 0, 0, "无医保门诊挂号信息" };

                DataRow dr1 = ds.Tables[0].Rows[0];
                string grbh = dr1["grbh"].ToString(); //个人编号
                string xm = dr1["xm"].ToString();  //姓名
                string kh = dr1["kh"].ToString();  //卡号
                string dqbh = dr1["tcqh"].ToString();  //地区编号
                bzbm = dr1["bzbm"].ToString();  //病种编码
                bzmc = dr1["bzmc"].ToString();  //病种名称
                string ybjzlsh_snyd = dr1["ybjzlsh_snyd"].ToString();

                //账户余额
                strSql = string.Format("select GRZHYE from ybickxx where grbh='{0}'", grbh);
                ds.Tables.Clear();
                ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                if (ds.Tables[0].Rows.Count == 0)
                    return new object[] { 0, 0, "获取医保信息出错" };
                string zhye = ds.Tables[0].Rows[0]["GRZHYE"].ToString();

                /*
                 1	保险号	VARCHAR2(10)	NOT NULL	
                2	姓名	VARCHAR2(20)	NOT NULL	
                3	卡号	VARCHAR2(18)	NOT NULL	
                4	地区编号	VARCHAR2(6)	NOT NULL	
                5	门诊号	VARCHAR2(18)	NOT NULL	
                6	病种编号	VARCHAR2(20)		为门诊挂号时返回的个人慢性病审批数据包，如果没有病种信息,则用“”代替。
                7	病种名称	VARCHAR2(100)		
                8	开方医生	VARCHAR2(20)	NOT NULL	
                9	是否打印票据	VARCHAR2(8)		值为1或0，做为预留，暂不使用。以下为费用明细，这两个参数之间以$符分割
                 */
                inputParam.Append(grbh + "|");
                inputParam.Append(xm + "|");
                inputParam.Append(kh + "|");
                inputParam.Append(dqbh + "|");
                inputParam.Append(ybjzlsh_snyd + "|");
                inputParam.Append(bzbm + "|");
                inputParam.Append(bzmc + "|");
                inputParam.Append(cfysxm + "|");
                inputParam.Append("0" + "|$");
                #endregion

                #region 获取处方明细信息
                strSql = string.Format(@"select y.ybxmbh, y.ybxmmc, m.dj, sum(m.sl) as sl, sum(m.je) as je,m.jx, m.yyxmbh, m.yyxmmc, m.ysdm, n.b1name as ysxm, m.ksno, o.b2ejnm as zxks, m.sfno,y.sfxmzldm as ybsfxmzldm,y.sflbdm as ybsflbdm, m.cfh from 
                                        (
                                            --药品
                                            select a.mcypno as yyxmbh, a.mcypnm as yyxmmc, a.mcpric as dj, a.mcquty as sl, a.mcamnt as je,'' jx,'' gg, a.mcksno as ksno, a.mcuser as ysdm, a.mcsflb as sfno, a.mccfno as cfh
                                            from mzcfd a where a.mcghno = '{0}' and a.mccfno in ({1})
                                            union all  
                                            --检查/治疗 
                                            select b.mbitem as yyxmbh, b.mbname as yyxmmc, b.mbpric as dj, b.mbquty as sl, b.mbsjam as je,'' jx,'' gg, b.mbksno as ksno, b.mbuser as ysdm, b.mbsfno as sfno , b.mbzlno as cfh          
                                            from mzb2d b where b.mbghno = '{0}' and b.mbzlno in ({1})
                                            union all
                                            --检验
                                            select c.mbitem as yyxmbh, c.mbname as yyxmmc, c.mbpric as dj, c.mbquty as sl, c.mbsjam as je,'' jx,'' gg, c.mbksno as ksno, c.mbuser as ysdm, c.mbsfno as sfno, c.mbzlno as cfh
                                            from mzb4d c where c.mbghno = '{0}' and c.mbzlno in ({1})
                                            --注射
                                            union all
                                            select d.mdwayid as yyxmbh, d.mdwayx as yyxmmc,d.mdpric as dj, mdtims  as sl, d.mdpric*d.mdtims as je,'' jx,'' gg, d.mdzsks as ksno, d.mdempn as ysdm, d.mdsflb as sfno, d.mdzsno as cfh
                                            from mzd3d d where d.mdzsno in ({1})
                                            union all
                                            select tw.b4item as yyxmbh, tw.b4name as yyxmmc, tw.b4sfam as dj,mddays as sl, tw.b4sfam* mddays as je,'' jx,'' gg, tw.mdzsks as ksno, tw.mdempn as ysdm, tw.mdsflb as sfno, tw.mdzsno as cfh
                                            from(select mzd3d.*,b4item,b4name,b4sfam from mzd3d left join bz04d on mdtwid=b4yzno where mdzsno in ({1}) and mdtiwe>0) tw
                                            union all
                                            select d.mdpprid as yyxmbh, '皮试' as yyxmmc,d.mdppri as dj, mdpqty as sl, d.mdppri*mdpqty as je,'' jx,'' gg, d.mdzsks as ksno, d.mdempn as ysdm, d.mdsflb as sfno, d.mdzsno as cfh
                                            from mzd3d d where d.mdzsno in ({1}) and d.mdpqty>0

                                            union all
                                            --处方划价
                                            select a.ygypno as yyxmbh, a.ygypnm as yyxmmc, ((a.ygamnt + 0.0) / a.ygslxx) as dj, a.ygslxx as sl, a.ygamnt as je,'' jx,'' gg, b.ygksno as ksno, b.ygysno as ysdm, c.y1sflb, a.ygshno as cfh
                                            from yp17d a 
                                            join yp17h b on a.ygcomp = b.ygcomp and a.ygshno = b.ygshno
                                            join yp01h c on c.y1ypno = a.ygypno
                                            where b.ygghno = '{0}' and a.ygshno in ({1}) and a.ygslxx > 0
                                        ) m join bz01h n on m.ysdm = n.b1empn join bz02d o on m.ksno = o.b2ejks
                                        left join ybhisdzdr y on m.yyxmbh = y.hisxmbh
                                        group by y.ybxmbh, y.ybxmmc, m.dj, m.jx, m.yyxmbh, m.yyxmmc, m.ysdm, n.b1name, m.ksno, o.b2ejnm, m.sfno,y.sfxmzldm,y.sflbdm, m.cfh"
                                        , jzlsh, cfhs);
                ds.Tables.Clear();
                ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                List<string> licfh = new List<string>();
                List<string> liybcfh = new List<string>();
                List<string> liyyxmdm = new List<string>();
                List<string> liyyxmmc = new List<string>();

                if (ds.Tables[0].Rows.Count == 0)
                    return new object[] { 0, 0, "无费用明细" };

                DataTable dt = ds.Tables[0];
                StringBuilder wdzxms = new StringBuilder();
                List<string> liSQL = new List<string>();
                decimal sfje_total = 0;
                for (int k = 0; k < dt.Rows.Count; k++)
                {
                    DataRow dr = dt.Rows[k];

                    if (dr["ybxmbh"] == DBNull.Value)
                    {
                        wdzxms.Append("项目代码：[" + dr["yyxmbh"].ToString() + "]，名称：[" + dr["yyxmmc"].ToString() + "]未对照，不能上传！");
                    }
                    else
                    {
                        string ybsfxmzldm = dr["ybsfxmzldm"].ToString();  //收费项目等级代码
                        string ybsflbdm = dr["ybsflbdm"].ToString();      //收费类别代码
                        string yyxmbh = dr["yyxmbh"].ToString();          //检查项目代码
                        string ybxmbh = dr["ybxmbh"].ToString();          //医保项目编号
                        string ybxmmc = dr["ybxmmc"].ToString();          //医保项目名称
                        string yyxmmc = dr["yyxmmc"].ToString();          //项目名称
                        decimal dj = Convert.ToDecimal(dr["dj"]);
                        decimal sl = Convert.ToDecimal(dr["sl"]);
                        decimal je = Convert.ToDecimal(dr["je"]);
                        sfje_total += je;
                        string jx = "";//dr["jx"].ToString(); 
                        string gg = "";//dr["gg"].ToString();
                        decimal mcyl = 1;
                        //string ysbm = dr["ysdm"].ToString();
                        //ysxm = dr["ysxm"].ToString();
                        //string ksdm = dr["ksno"].ToString();
                        //string ksmc = dr["zxks"].ToString();
                        string cfh = dr["cfh"].ToString();
                        string ybcfh = cfsj + k.ToString();


                        /*1	收费项目中心编码	VARCHAR2(20)	NOT NULL	代码，中心系统规定的收费项目
                            2	项目名称	VARCHAR2(100)	NOT NULL	
                            3	单价	NUMBER(8,2)	NOT NULL	
                            4	数量	NUMBER(8,2)	NOT NULL	
                            5	金额	NUMBER(8,2)	NOT NULL	
                            6	处方日期	VARCHAR2(14)	NOT NULL	YYYYMMDDHH24MISS
                            7	医院收费项目内码	VARCHAR2(20)	NOT NULL	
                            8	医院收费项目名称	VARCHAR2(50)	NOT NULL	
                         */

                        inputParam.Append(ybxmbh + "|");
                        inputParam.Append(ybxmmc + "|");
                        inputParam.Append(dj + "|");
                        inputParam.Append(sl + "|");
                        inputParam.Append(je + "|");
                        inputParam.Append(cfsj + "|");
                        inputParam.Append(yyxmbh + "|");
                        inputParam.Append(yyxmmc + "|$");
                    }
                }

                if (wdzxms.Length > 0)
                {
                    return new object[] { 0, 0, wdzxms.ToString() };
                }

                if (Math.Abs(sfje_total - sfje) > 1)
                    return new object[] { 0, 0, "收费金额与医保结算金额相差" + Math.Abs(sfje_total - sfje) + ",无法结算，请核实!" };

                #endregion

                #region 门诊收费预结算
                StringBuilder inputData = new StringBuilder();
                YWBH = "7120";
                inputData.Append(YWBH + "^");
                inputData.Append(YLGHBH + "^");
                inputData.Append(CZYBH + "^");
                inputData.Append(YWZQH + "^");
                inputData.Append(JYLSH + "^");
                inputData.Append(ZXBM + "^");
                inputData.Append(inputParam.ToString().TrimEnd('$') + "^");
                inputData.Append(LJBZ + "^");

                WriteLog(sysdate + "  门诊收费预结算|入参|" + inputData.ToString());

                StringBuilder outputData = new StringBuilder(10240);
                int i = BUSINESS_HANDLE(inputData, outputData);

                if (i == 0)//处方上传成功
                {
                    WriteLog(sysdate + "  门诊收费预结算|出参|" + outputData.ToString());
                    string[] zysfdjfh = outputData.ToString().Split('^')[2].Split('|');
                    #region 出参
                    /*
                     1	医疗总费用	VARCHAR2(10)		2位小数
                    2	本次账户支付	VARCHAR2(10)		2位小数
                    3	本次现金支付	VARCHAR2(10)		2位小数
                    4	本次基金支付	VARCHAR2(10)		2位小数
                    5	大病基金支付	VARCHAR2(10)		2位小数
                    6	救助金额	VARCHAR2(10)		2位小数
                    7	公务员补助支付	VARCHAR2(10)		2位小数
                    8	甲类费用	VARCHAR2(10)		2位小数
                    9	乙类费用	VARCHAR2(10)		2位小数
                    10	丙类费用	VARCHAR2(10)		2位小数
                    11	自费费用	VARCHAR2(10)		2位小数
                    12	起付标准自付	VARCHAR2(10)		2位小数
                    13	非医保自付	VARCHAR2(10)		2位小数
                    14	乙类药品自付	VARCHAR2(10)		2位小数
                    15	特检特治自付	VARCHAR2(10)		2位小数
                    16	进入统筹自付	VARCHAR2(10)		2位小数
                    17	进入大病自付	VARCHAR2(10)		2位小数
                    18	门诊流水号	VARCHAR2(18)	NOT NULL	
                    19	单据流水号	VARCHAR2(18)	NOT NULL	
                     */
                    //出参
                    outParams_js js = new outParams_js();
                    js.Ylfze = zysfdjfh[0];         //医疗总费用
                    js.Zhzf = zysfdjfh[1];          //本次帐户支付
                    js.Xjzf = zysfdjfh[2];          //本次现金支付
                    js.Tcjjzf = zysfdjfh[3];        //本次统筹支付
                    js.Dejjzf = zysfdjfh[4];        //大病基金支付
                    js.Mzjzfy = zysfdjfh[5];        //救助金额
                    js.Gwybzjjzf = zysfdjfh[6];     //公务员补助支付
                    js.Jlfy = zysfdjfh[7];          //甲类费用
                    js.Ylfy = zysfdjfh[8];          //乙类费用
                    js.Blfy = zysfdjfh[9];          //丙类费用

                    js.Zffy = zysfdjfh[10];         //自费费用
                    js.Qfbzfy = zysfdjfh[11];       //起付标准自付
                    js.Fybzf = zysfdjfh[12];        //非医保自付
                    js.Ylzlfy = zysfdjfh[13];       //乙类药品自付
                    js.Tjzlfy = zysfdjfh[14];       //特检特治自付

                    js.Tcfdzffy = zysfdjfh[15];     //进入统筹自付(统筹分段自付)
                    js.Defdzffy = zysfdjfh[16];     //进入大病自付(大病分段自付)
                    js.Ybmzlsh = zysfdjfh[17];        //门诊流水号
                    js.Jslsh = zysfdjfh[18];        //返回的单据号
                    js.Zbxje = (Convert.ToDecimal(js.Ylfze) - Convert.ToDecimal(js.Xjzf)).ToString();

                    js.Bcjsqzhye = zhye;

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
                       * 居民个人自付二次补偿金额|体检金额|生育基金支付|
                       */
                    //总报销金额=医疗总费用-本次现金支付-本次大病支出
                    js.Zbxje = (Convert.ToDecimal(js.Ylfze) - Convert.ToDecimal(js.Xjzf) - Convert.ToDecimal(js.Dejjzf)).ToString();
                    //其他医保支付=医疗总费用-本次现金支付-本次大病支出-本次统筹支付-本次账户支付
                    js.Qtybzf = (Convert.ToDecimal(js.Ylfze) - Convert.ToDecimal(js.Xjzf) - Convert.ToDecimal(js.Dejjzf) - Convert.ToDecimal(js.Tcjjzf) - Convert.ToDecimal(js.Zhzf)).ToString();
                    //现金支付=本次现金支付+本次大病支出
                    js.Ybxjzf = (Convert.ToDecimal(js.Xjzf) + Convert.ToDecimal(js.Dejjzf)).ToString();

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

                    strSql = string.Format(@"delete from ybfyyjsdr where ybjzlsh='{0}'", ybjzlsh);
                    liSQL.Add(strSql);
                    strSql = string.Format(@"insert into ybfyyjsdr(
                                             jzlsh, jylsh ,ylfze,zhzf,xjzf,tcjjzf,dejjzf, mzjzfy,gwybzjjzf,jlfy,
                                             ylfy,blfy,zffy,qfbzfy,fybzf,ylzlfy,tjzlfy,tcfdzffy,defdzffy,yllb,
                                             xm,grbh,kh,cfmxjylsh,ywzqh,djh,ybjzlsh,jslsh,ybmzlsh,jbr,
                                             zbxje,bcjsqzhye,jsrq,qtybfy,zhxjzffy)
                                             values('{0}','{1}','{2}','{3}','{4}','{5}' ,'{6}','{7}','{8}' ,'{9}' ,
                                             '{10}','{11}' ,'{12}','{13}' ,'{14}','{15}','{16}' ,'{17}' ,'{18}','{19}',
                                             '{20}' ,'{21}', '{22}','{23}','{24}','{25}','{26}','{27}','{28}','{29}',
                                            '{30}' ,'{31}','{32}','{33}','{34}')",
                                            jzlsh, JYLSH, js.Ylfze, js.Zhzf, js.Xjzf, js.Tcjjzf, js.Dejjzf, js.Mzjzfy, js.Gwybzjjzf, js.Jlfy,
                                            js.Ylfy, js.Blfy, js.Zffy, js.Qfbzfy, js.Fybzf, js.Ylzlfy, js.Tjzlfy, js.Tcfdzffy, js.Defdzffy, yllb,
                                            xm, grbh, kh, JYLSH, YWZQH, djh, ybjzlsh, js.Jslsh, js.Ybmzlsh, jbr,
                                            js.Zbxje, js.Bcjsqzhye, cfsj, js.Qtybzf, js.Ybxjzf);
                    liSQL.Add(strSql);

                    object[] obj_MZSF = liSQL.ToArray();
                    obj_MZSF = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj_MZSF);

                    if (obj_MZSF[1].ToString() == "1")
                    {

                        WriteLog(sysdate + "  费用预结算(省内异地)|本地数据操作成功|" + strValue);
                        return new object[] { 0, 1, strValue };
                    }
                    else
                    {
                        WriteLog(sysdate + "  费用预结算(省内异地)|本地数据操作失败|" + obj_MZSF[2].ToString());
                        return new object[] { 0, 0, "费用预结算(省内异地)失败" };
                    }
                }
                else
                {
                    WriteLog(sysdate + "  费用预结算(省内异地)失败|" + outputData.ToString());
                    return new object[] { 0, 0, outputData.ToString() };
                }
                #endregion
            }
            catch (Exception ex)
            {
                WriteLog(sysdate + "  门诊收费预结算(省内异地)失败|系统异常|" + ex.Message);
                return new object[] { 0, 2, "系统异常|" + ex.Message };
            }
        }

        #endregion

        #region 门诊费用结算
        public static object[] YBMZSFJS(object[] objParam)
        {
            #region 入参
            CZYBH = CliUtils.fLoginUser;  //操作员工号
            YWZQH = CliUtils.fLoginYbNo.Split('|')[0].ToString();  //业务周期号
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

            if (string.IsNullOrEmpty(jzlsh))
                return new object[] { 0, 0, "就诊流水号不能为空" };
            if (string.IsNullOrEmpty(yllb))
                return new object[] { 0, 0, "医疗类别不能为空" };

            string ybjzlsh = "";
            if (string.IsNullOrEmpty(bzbh))
                ybjzlsh = "MZ" + yllb + jzlsh;
            else
                ybjzlsh = "MZ" + yllb + jzlsh + bzbh;

            //判断是否进行登记
            string strSql = string.Format("select * from ybmzzydjdr a where a.ybjzlsh = '{0}' and a.jzbz='m'  and a.cxbz = 1", ybjzlsh);
            DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            if (ds == null || ds.Tables[0].Rows.Count == 0)
                return new object[] { 0, 0, "就诊流水号：" + jzlsh + "无挂号或入院登记记录" };
            DataRow dr = ds.Tables[0].Rows[0];
            string grbh = dr["grbh"].ToString();        // 个人编号  
            DQJBBZ = dr["dqjbbz"].ToString();           //市本级或省内异地标志

            //判断市本级或省内异地

            if (DQJBBZ.Equals("1"))
                return YBMZFYJS_BSJ(objParam);
            else
                return YBMZFYJS_SNYD(objParam);
        }
        #endregion

        #region 门诊费用结算(本市级)
        public static object[] YBMZFYJS_BSJ(object[] objParam)
        {
            string sysdate = GetServerDateTime();
            WriteLog(sysdate + "  进入门诊费用结算(市本级)...");

            try
            {
                CZYBH = CliUtils.fLoginUser;  //操作员工号
                YWZQH = CliUtils.fLoginYbNo.Split('|')[0].ToString();  //业务周期号
                string jbr = CliUtils.fUserName;    //经办人姓名

                string jzlsh = objParam[0].ToString();  //就诊流水号
                string djh = objParam[1].ToString();    //单据号（发票号)
                string zhsybz = objParam[2].ToString(); //账户使用标志(；在吉安地区用不到,门诊收费走账户)
                string jssj = objParam[3].ToString();   //结算时间
                string bzbm = objParam[4].ToString();   //病种编号
                string bzmc = objParam[5].ToString();   //病种名称
                string cfhs = objParam[6].ToString();   //处方号集
                string yllb = objParam[7].ToString();   //医疗类别
                string sfje = objParam[8].ToString();   //收费金额
                string cfysdm = objParam[9].ToString(); //处方医生代码
                string cfysxm = objParam[10].ToString();    //处方医生姓名
                string cfmxjylsh="";
                string jsrq = Convert.ToDateTime(jssj).ToString("yyyyMMddHHmmss"); //结算日期
                //string jsdjh = jsrq + jzlsh.Substring(jzlsh.Length - 5, 5);    //结算单据号 门诊以12位时间+流水号后5位作为单据号
                string strSql;
                string sslxdm = "";    //手术类型代码  

                JYLSH = jsrq + "-" + YLGHBH + "-" + new Random().Next(100).ToString().PadLeft(4, '0');
                DataSet ds = null;

                if (string.IsNullOrEmpty(jzlsh))
                    return new object[] { 0, 0, "就诊流水号不能为空" };
                if (string.IsNullOrEmpty(djh))
                    return new object[] { 0, 0, "单据号不能为空" };
                if (string.IsNullOrEmpty(yllb))
                    return new object[] { 0, 0, "医疗类别不能为空" };

                string ybjzlsh = "";
                if (string.IsNullOrEmpty(bzbm))
                    ybjzlsh = "MZ" + yllb + jzlsh;
                else
                    ybjzlsh = "MZ" + yllb + jzlsh + bzbm;

                //是否进行医保登记
                strSql = string.Format("select * from ybmzzydjdr a where a.ybjzlsh = '{0}'  and a.jzbz='m' and a.cxbz = 1", ybjzlsh);
                ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);

                if (ds == null || ds.Tables[0].Rows.Count == 0)
                    return new object[] { 0, 0, "无挂号或入院登记记录" };

                DataRow dr = ds.Tables[0].Rows[0];
                yllb = dr["yllb"].ToString();
                bzbm = dr["bzbm"].ToString();
                bzmc = dr["bzmc"].ToString();
                string xm = dr["xm"].ToString();
                string grbh = dr["grbh"].ToString();
                string kh = dr["kh"].ToString();

                strSql = string.Format("select GRZHYE from ybickxx where grbh='{0}'", grbh);
                ds.Tables.Clear();
                ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                if (ds.Tables[0].Rows.Count == 0)
                    return new object[] { 0, 0, "获取医保信息出错" };
                string zhye = ds.Tables[0].Rows[0]["GRZHYE"].ToString();

                //是否进行费用上传
                strSql = string.Format(@"select a.jylsh from ybcfmxscindr a  
                                        where jylsh not in(select cfmxjylsh from ybfyjsdr where ybjzlsh=a.ybjzlsh and cxbz=1) 
                                        and a.ybjzlsh='{0}' and a.cxbz=1", ybjzlsh);
                ds.Tables.Clear();
                ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                if (ds.Tables[0].Rows.Count > 0)
                    cfmxjylsh = ds.Tables[0].Rows[0]["jylsh"].ToString();
                else
                {
                    WriteLog(sysdate + "  无上传费用明细");
                    return new object[] { 0, 0, "无上传费用明细" };
                }
                ds.Dispose();
               

                #region 入参
                YWBH = "2410";
                StringBuilder inputParam = new StringBuilder();
                inputParam.Append(ybjzlsh + "|");
                inputParam.Append(djh + "|");
                inputParam.Append(yllb + "|");
                inputParam.Append(jsrq + "|");
                inputParam.Append("|");
                inputParam.Append("|");
                inputParam.Append(bzbm + "|");
                inputParam.Append(bzmc + "|");
                inputParam.Append(zhsybz + "|");
                inputParam.Append("0" + "|");
                inputParam.Append(jbr + "|"); //经办人
                inputParam.Append(sslxdm + "|"); //手术类型
                //inputParam.Append("" + "|");    //胎儿数

                StringBuilder inputData = new StringBuilder();
                inputData.Append(YWBH + "^");
                inputData.Append(YLGHBH + "^");
                inputData.Append(CZYBH + "^");
                inputData.Append(YWZQH + "^");
                inputData.Append(JYLSH + "^");
                inputData.Append(ZXBM + "^");
                inputData.Append(inputParam.ToString() + "^");
                inputData.Append(LJBZ + "^");

                WriteLog(sysdate + "  门诊费用结算(市本级)|入参|" + inputData.ToString());
                #endregion

                StringBuilder outputData = new StringBuilder(10240);
                int i = BUSINESS_HANDLE(inputData, outputData);

                if (i == 0)
                {
                    List<string> lisql = new List<string>();
                    WriteLog(sysdate + "  门诊费用结算(市本级)成功|出参|" + outputData.ToString());
                    string[] sfjsfh = outputData.ToString().Split('^')[2].Split('|');
                    outParams_js js = new outParams_js();
                    #region 出参
                    /*
                     * 1		医疗费总费用	VARCHAR2(10)		2位小数
                        2		自费费用	VARCHAR2(10)		2位小数
                        3		本次帐户支付	VARCHAR2(10)		2位小数
                        4		统筹支出	VARCHAR2(10)		2位小数
                        5		大病支出	VARCHAR2(10)		2位小数
                        6		公务员补助支付金额	VARCHAR2(10)		2位小数, 如果参保人参加了单位补充医疗保险，此处输出单位补充医疗保险支付金额
                        7		企业补充支付金额	VARCHAR2(10)		2位小数，如果是居民门诊统筹结算，则为本次门诊统筹定额打包费用。
                        8		乙类自理费用	VARCHAR2(10)		2位小数
                        9		特检自理费用	VARCHAR2(10)		2位小数
                        10		特治自理费用	VARCHAR2(10)		2位小数
                        11		符合基本医疗费用	VARCHAR2(10)		2位小数
                        12		起付标准自付	VARCHAR2(10)		2位小数
                        13		进入统筹费用	VARCHAR2(10)		2位小数
                        14		统筹分段自付	VARCHAR2(10)		2位小数
                        15		进入大病费用	VARCHAR2(10)		2位小数
                        16		大病分段自付	VARCHAR2(10)		2位小数
                        17		转诊先自付	VARCHAR2(10)		2位小数
                        18		个人现金支付	VARCHAR2(10)		2位小数
                        19		药品费用	VARCHAR2(10)		2位小数
                        20		诊疗项目费用	VARCHAR2(10)		2位小数
                        21		安装器官费用	VARCHAR2(10)		2位小数
                        22		商业补充保险支付费用	VARCHAR2(10)		2位小数，城居是民政三次救助
                        23		专项救助费用	VARCHAR2(10)		2位小数，城居是民政救助
                        24		单位分担金额	VARCHAR2(10)		2位小数
                        25		门诊诊查费	VARCHAR2(10)		2位小数
                        26		军转干部补助支付	VARCHAR2(10)		2位小数
                        27		基本医疗账户支付	NUMBER(16)		2位小数
                        28		公费医疗账户支付	NUMBER(16)		2位小数
                        29		家庭门诊统筹支付	NUMBER(16)		2位小数
                        30		大病救助支付	NUMBER(16)		2位小数
                        31		生育基金支付	NUMBER(16)		2 位小数
                     */

                    js.Ylfze = sfjsfh[0];         //医疗费总费用
                    js.Zffy = sfjsfh[1];          //自费费用
                    js.Zhzf = sfjsfh[2];          //本次帐户支付
                    js.Tcjjzf = sfjsfh[3];        //统筹支出
                    js.Dejjzf = sfjsfh[4];        //大病支出
                    js.Gwybzjjzf = sfjsfh[5];     //公务员补助支付金额
                    js.Qybcylbxjjzf = sfjsfh[6];  //企业补充支付金额
                    js.Ylzlfy = sfjsfh[7];        //乙类自理费用
                    js.Tjzlfy = sfjsfh[8];        //特检自理费用
                    js.Tzzlfy = sfjsfh[9];        //特治自理费用
                    js.Fhjjylfy = sfjsfh[10];     //符合基本医疗费用
                    js.Qfbzfy = sfjsfh[11];       //起付标准自付
                    js.Jrtcfy = sfjsfh[12];       //进入统筹费用
                    js.Tcfdzffy = sfjsfh[13];     //统筹分段自付
                    js.Jrdebxfy = sfjsfh[14];     //进入大病费用
                    js.Defdzffy = sfjsfh[15];     //大病分段自付
                    js.Zzzyzffy = sfjsfh[16];     //转诊先自付
                    js.Xjzf = sfjsfh[17];         //个人现金支付
                    js.Ypfy = sfjsfh[18];         //药品费用
                    js.Zlxmfy = sfjsfh[19];       //诊疗项目费用
                    js.Rgqgzffy = sfjsfh[20];     //安装器官费用
                    js.Sybxfy = sfjsfh[21];   //商业补充保险支付费用
                    js.Mzjzfy = sfjsfh[22];       //专项救助费用
                    js.Dwfdfy = sfjsfh[23];       //单位分担金额
                    js.Mzzcf = sfjsfh[24];        //门诊诊查费
                    js.Jzgbbzzf = sfjsfh[25];     //军转干部补助支付
                    js.Jbylzhzf = sfjsfh[26];     //基本医疗账户支付
                    js.Gfylzhzf = sfjsfh[27];     //公费医疗账户支付
                    js.Jtmztczf = sfjsfh[28];     //家庭门诊统筹支付
                    js.Dbjzfy = sfjsfh[29];     //大病救助支付
                    js.Yyfdfy = "0.00";
                    js.Cxjfy = "0.00";
                    js.Ylzlfy = "0.00";
                    js.Bcjsqzhye = zhye;
                    js.Jslsh = djh;
                    //js.Bcjshzhye = (Convert.ToDecimal(zhye) - Convert.ToDecimal(js.Zhzf)).ToString();

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
                    js.Zbxje = (Convert.ToDecimal(js.Ylfze) - Convert.ToDecimal(js.Xjzf) - Convert.ToDecimal(js.Dejjzf)).ToString();
                    //其他医保支付=医疗总费用-本次现金支付-本次大病支出-本次统筹支付-本次账户支付
                    js.Qtybzf = (Convert.ToDecimal(js.Ylfze) - Convert.ToDecimal(js.Xjzf) - Convert.ToDecimal(js.Dejjzf) - Convert.ToDecimal(js.Tcjjzf) - Convert.ToDecimal(js.Zhzf)).ToString();
                    //现金支付=本次现金支付+本次大病支出
                    js.Ybxjzf = (Convert.ToDecimal(js.Xjzf) + Convert.ToDecimal(js.Dejjzf)).ToString();

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

                    //WriteLog(sysdate + "  门诊费用结算(市本级)成功|出参1|" + strValue);
                    #endregion
                    strSql = string.Format(@"insert into ybfyjsdr(jzlsh, jylsh ,ylfze ,xjzf ,zhzf ,tcjjzf, dejjzf,gwybzjjzf,qybcylbxjjzf,ylzlfy,
                                                    tjzlfy,tzzlfy,fhjbylfy,qfbzfy,jrtcfy,ctcfdxfy,jrdebxfy,defdzffy,zzzyzffy,zffy,
                                                    ypfy,zlxmfy,rgqgzffy,sybcbxzffy,mzjzfy,dwfdfy,mzzcf,yllb,xm,grbh,
                                                    kh,cfmxjylsh,ywzqh,jslsh,jsrq,djh,ybjzlsh,jzgbbzzf,jbylzhzf,gfylzhzf,
                                                    jtmztczf,dbjzzf,sysdate,jbr,zbxje,bcjsqzhye,qtybfy,zhxjzffy,bzbm,bzmc)
                                                    values('{0}','{1}','{2}','{3}','{4}','{5}' ,'{6}','{7}','{8}' ,'{9}' ,
                                                    '{10}','{11}' ,'{12}','{13}' ,'{14}','{15}','{16}' ,'{17}' ,'{18}','{19}',
                                                    '{20}' ,'{21}','{22}','{23}','{24}','{25}','{26}','{27}' ,'{28}','{29}',
                                                    '{30}','{31}','{32}','{33}','{34}','{35}','{36}','{37}','{38}','{39}',
                                                    '{40}','{41}','{42}','{43}','{44}','{45}','{46}','{47}','{48}','{49}')",
                                                     jzlsh, JYLSH, js.Ylfze, js.Xjzf, js.Zhzf, js.Tcjjzf, js.Dejjzf, js.Gwybzjjzf, js.Qybcylbxjjzf, js.Ylzlfy,
                                                     js.Tjzlfy, js.Tzzlfy, js.Fhjjylfy, js.Qfbzfy, js.Jrtcfy, js.Tcfdzffy, js.Jrdebxfy, js.Defdzffy, js.Zzzyzffy, js.Zffy,
                                                     js.Ypfy, js.Zlxmfy, js.Rgqgzffy, js.Sybxfy, js.Mzjzfy, js.Dwfdfy, js.Mzzcf, yllb, xm, grbh,
                                                     kh, cfmxjylsh, YWZQH, djh, jssj, djh, ybjzlsh, js.Jzgbbzzf, js.Jbylzhzf, js.Gfylzhzf,
                                                     js.Jtmztczf, js.Dbjzfy, sysdate, jbr, js.Zbxje, js.Bcjsqzhye, js.Qtybzf, js.Ybxjzf, bzbm, bzmc);

                    lisql.Add(strSql);

                    object[] obj = lisql.ToArray();
                    obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);
                    if (obj[1].ToString() == "1")
                    {
                        WriteLog(sysdate + "  门诊费用结算(市本级)成功|本地数据操作成功|" + strValue);
                        return new object[] { 0, 1, strValue };
                    }
                    else
                    {
                        WriteLog(sysdate + "  门诊费用结算(市本级)成功|本地数据操作失败|" + obj[2].ToString());
                        object[] objFYJSCX = { jzlsh, djh, ybjzlsh, jsrq, grbh, xm, kh, "", "", cfmxjylsh, 1 };
                        NYBFYJSCX(objFYJSCX);
                        return new object[] { 0, 0, "门诊费用结算(市本级)成功|本地数据操作失败|" + obj[2].ToString() };
                    }
                }
                else
                {
                    WriteLog(sysdate + "  费用结算(市本级)失败|" + outputData.ToString());
                    return new object[] { 0, 0, outputData.ToString() };
                }
            }
            catch (Exception error)
            {
                WriteLog(sysdate + "  系统异常|" + error.Message);
                return new object[] { 0, 2, error.Message };
            }
        }
        #endregion

        #region 门诊费用结算(省内异地)
        public static object[] YBMZFYJS_SNYD(object[] objParam)
        {
            string sysdate = GetServerDateTime();
            WriteLog(sysdate + "  进入门诊费用结算(省内异地)...");
            try
            {
                CZYBH = CliUtils.fLoginUser;  //操作员工号
                YWZQH = CliUtils.fLoginYbNo.Split('|')[0].ToString();  //业务周期号
                string jbr = CliUtils.fUserName;    //经办人姓名

                string jzlsh = objParam[0].ToString();  //就诊流水号
                string djh = objParam[1].ToString();    //单据号（发票号)
                string zhsybz = objParam[2].ToString(); //账户使用标志(；在吉安地区用不到,门诊收费走账户)
                string jssj = objParam[3].ToString();   //结算时间
                string bzbm = objParam[4].ToString();   //病种编号
                string bzmc = objParam[5].ToString();   //病种名称
                string cfhs = objParam[6].ToString();   //处方号集
                string yllb = objParam[7].ToString();   //医疗类别
                string sfje1 = objParam[8].ToString();   //收费金额
                string cfysdm = objParam[9].ToString(); //处方医生代码
                string cfysxm = objParam[10].ToString();    //处方医生姓名

                if (string.IsNullOrEmpty(jzlsh))
                    return new object[] { 0, 0, "就诊流水号不能为空" };
                if (string.IsNullOrEmpty(djh))
                    return new object[] { 0, 0, "单据号不能为空" };
                if (string.IsNullOrEmpty(yllb))
                    return new object[] { 0, 0, "医疗类别不能为空" };

                string ybjzlsh = "";
                if (string.IsNullOrEmpty(bzbm))
                    ybjzlsh = "MZ" + yllb + jzlsh;
                else
                    ybjzlsh = "MZ" + yllb + jzlsh + bzbm;

                decimal sfje = Convert.ToDecimal(sfje1);

                string cfsj = DateTime.Now.ToString("yyyyMMddHHmmss");  //处方时间
                string cyrq = cfsj;                                     //出院日期
                JYLSH = cfsj + "-" + YLGHBH + "-" + new Random().Next(100).ToString().PadLeft(4, '0');


                #region 获取医保门诊登记信息
                string strSql = string.Format("select * from ybmzzydjdr a where a.ybjzlsh = '{0}' and a.cxbz = 1", ybjzlsh);
                DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);

                if (ds == null || ds.Tables[0].Rows.Count == 0)
                    return new object[] { 0, 0, "无医保门诊挂号信息" };

                DataRow dr1 = ds.Tables[0].Rows[0];
                string grbh = dr1["grbh"].ToString(); //个人编号
                string xm = dr1["xm"].ToString();  //姓名
                string kh = dr1["kh"].ToString();  //卡号
                string dqbh = dr1["tcqh"].ToString();  //地区编号
                 bzbm = dr1["bzbm"].ToString();  //病种编码
                 bzmc = dr1["bzmc"].ToString();  //病种名称
                string ybjzlsh_snyd = dr1["ybjzlsh_snyd"].ToString();

                /* 
                 1	保险号	VARCHAR2(10)	NOT NULL	
                2	姓名	VARCHAR2(20)	NOT NULL	
                3	卡号	VARCHAR2(18)	NOT NULL	
                4	地区编号	VARCHAR2(6)	NOT NULL	
                5	门诊号	VARCHAR2(18)	NOT NULL	
                6	病种编号	VARCHAR2(20)		为门诊挂号时返回的个人慢性病审批数据包，如果没有病种信息,则用“”代替。
                7	病种名称	VARCHAR2(100)		
                8	开方医生	VARCHAR2(20)	NOT NULL	
                9	是否打印票据	VARCHAR2(8)		值为1或0，做为预留，暂不使用。以下为费用明细，这两个参数之间以$符分割
                 */
                StringBuilder inputParam = new StringBuilder();
                inputParam.Append(grbh + "|");
                inputParam.Append(xm + "|");
                inputParam.Append(kh + "|");
                inputParam.Append(dqbh + "|");
                inputParam.Append(ybjzlsh_snyd + "|");
                inputParam.Append(bzbm + "|");
                inputParam.Append(bzmc + "|");
                inputParam.Append(cfysxm + "|");
                inputParam.Append("0" + "|$");
                #endregion

                strSql = string.Format("select GRZHYE from ybickxx where grbh='{0}'", grbh);
                ds.Tables.Clear();
                ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                if (ds.Tables[0].Rows.Count == 0)
                    return new object[] { 0, 0, "获取医保信息出错" };
                string zhye = ds.Tables[0].Rows[0]["GRZHYE"].ToString();


                #region 获取处方明细信息
                strSql = string.Format(@"select y.ybxmbh, y.ybxmmc, m.dj, sum(m.sl) as sl, sum(m.je) as je,m.jx, m.yyxmbh, m.yyxmmc, m.ysdm, n.b1name as ysxm, m.ksno, o.b2ejnm as zxks, m.sfno,y.sfxmzldm as ybsfxmzldm,y.sflbdm as ybsflbdm, m.cfh from 
                                        (
                                            --药品
                                            select a.mcypno as yyxmbh, a.mcypnm as yyxmmc, a.mcpric as dj, a.mcquty as sl, a.mcamnt as je,'' jx,'' gg, a.mcksno as ksno, a.mcuser as ysdm, a.mcsflb as sfno, a.mccfno as cfh
                                            from mzcfd a where a.mcghno = '{0}' and a.mccfno in ({1})
                                            union all  
                                            --检查/治疗 
                                            select b.mbitem as yyxmbh, b.mbname as yyxmmc, b.mbpric as dj, b.mbquty as sl, b.mbsjam as je,'' jx,'' gg, b.mbksno as ksno, b.mbuser as ysdm, b.mbsfno as sfno , b.mbzlno as cfh          
                                            from mzb2d b where b.mbghno = '{0}' and b.mbzlno in ({1})
                                            union all
                                            --检验
                                            select c.mbitem as yyxmbh, c.mbname as yyxmmc, c.mbpric as dj, c.mbquty as sl, c.mbsjam as je,'' jx,'' gg, c.mbksno as ksno, c.mbuser as ysdm, c.mbsfno as sfno, c.mbzlno as cfh
                                            from mzb4d c where c.mbghno = '{0}' and c.mbzlno in ({1})
                                            --注射
                                            union all
                                            select d.mdwayid as yyxmbh, d.mdwayx as yyxmmc,d.mdpric as dj, mdtims  as sl, d.mdpric*d.mdtims as je,'' jx,'' gg, d.mdzsks as ksno, d.mdempn as ysdm, d.mdsflb as sfno, d.mdzsno as cfh
                                            from mzd3d d where d.mdzsno in ({1})
                                            union all
                                            select tw.b4item as yyxmbh, tw.b4name as yyxmmc, tw.b4sfam as dj,mddays as sl, tw.b4sfam* mddays as je,'' jx,'' gg, tw.mdzsks as ksno, tw.mdempn as ysdm, tw.mdsflb as sfno, tw.mdzsno as cfh
                                            from(select mzd3d.*,b4item,b4name,b4sfam from mzd3d left join bz04d on mdtwid=b4yzno where mdzsno in ({1}) and mdtiwe>0) tw
                                            union all
                                            select d.mdpprid as yyxmbh, '皮试' as yyxmmc,d.mdppri as dj, mdpqty as sl, d.mdppri*mdpqty as je,'' jx,'' gg, d.mdzsks as ksno, d.mdempn as ysdm, d.mdsflb as sfno, d.mdzsno as cfh
                                            from mzd3d d where d.mdzsno in ({1}) and d.mdpqty>0

                                            union all
                                            --处方划价
                                            select a.ygypno as yyxmbh, a.ygypnm as yyxmmc, ((a.ygamnt + 0.0) / a.ygslxx) as dj, a.ygslxx as sl, a.ygamnt as je,'' jx,'' gg, b.ygksno as ksno, b.ygysno as ysdm, c.y1sflb, a.ygshno as cfh
                                            from yp17d a 
                                            join yp17h b on a.ygcomp = b.ygcomp and a.ygshno = b.ygshno
                                            join yp01h c on c.y1ypno = a.ygypno
                                            where b.ygghno = '{0}' and a.ygshno in ({1}) and a.ygslxx > 0
                                        ) m join bz01h n on m.ysdm = n.b1empn join bz02d o on m.ksno = o.b2ejks
                                        left join ybhisdzdr y on m.yyxmbh = y.hisxmbh
                                        group by y.ybxmbh, y.ybxmmc, m.dj, m.jx, m.yyxmbh, m.yyxmmc, m.ysdm, n.b1name, m.ksno, o.b2ejnm, m.sfno,y.sfxmzldm,y.sflbdm, m.cfh"
                                        , jzlsh, cfhs);
                ds.Tables.Clear();
                ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);

                if (ds.Tables[0].Rows.Count == 0)
                    return new object[] { 0, 0, "无费用明细" };

                DataTable dt = ds.Tables[0];
                StringBuilder wdzxms = new StringBuilder();
                List<string> liSQL = new List<string>();
                decimal sfje_total = 0;
                for (int k = 0; k < dt.Rows.Count; k++)
                {
                    DataRow dr = dt.Rows[k];

                    if (dr["ybxmbh"] == DBNull.Value)
                    {
                        wdzxms.Append("项目代码：[" + dr["yyxmbh"].ToString() + "]，名称：[" + dr["yyxmmc"].ToString() + "]未对照，不能上传！");
                    }
                    else
                    {
                        string ybsfxmzldm = dr["ybsfxmzldm"].ToString();  //收费项目等级代码
                        string ybsflbdm = dr["ybsflbdm"].ToString();      //收费类别代码
                        string yyxmbh = dr["yyxmbh"].ToString();          //检查项目代码
                        string ybxmbh = dr["ybxmbh"].ToString();          //医保项目编号
                        string ybxmmc = dr["ybxmmc"].ToString();          //医保项目名称
                        string yyxmmc = dr["yyxmmc"].ToString();          //项目名称
                        decimal dj = Convert.ToDecimal(dr["dj"]);
                        decimal sl = Convert.ToDecimal(dr["sl"]);
                        decimal je = Convert.ToDecimal(dr["je"]);
                        sfje_total += je;
                        string jx = "";//dr["jx"].ToString(); 
                        string gg = "";//dr["gg"].ToString();
                        decimal mcyl = 1;
                        //string ysbm = dr["ysdm"].ToString();
                        //ysxm = dr["ysxm"].ToString();
                        //string ksdm = dr["ksno"].ToString();
                        //string ksmc = dr["zxks"].ToString();
                        string cfh = dr["cfh"].ToString();
                        string ybcfh = cfsj + k.ToString();




                        /*1	收费项目中心编码	VARCHAR2(20)	NOT NULL	代码，中心系统规定的收费项目
                            2	项目名称	VARCHAR2(100)	NOT NULL	
                            3	单价	NUMBER(8,2)	NOT NULL	
                            4	数量	NUMBER(8,2)	NOT NULL	
                            5	金额	NUMBER(8,2)	NOT NULL	
                            6	处方日期	VARCHAR2(14)	NOT NULL	YYYYMMDDHH24MISS
                            7	医院收费项目内码	VARCHAR2(20)	NOT NULL	
                            8	医院收费项目名称	VARCHAR2(50)	NOT NULL	
                         */

                        inputParam.Append(ybxmbh + "|");
                        inputParam.Append(ybxmmc + "|");
                        inputParam.Append(dj + "|");
                        inputParam.Append(sl + "|");
                        inputParam.Append(je + "|");
                        inputParam.Append(cfsj + "|");
                        inputParam.Append(yyxmbh + "|");
                        inputParam.Append(yyxmmc + "|$");


                        strSql = string.Format(@"insert into ybcfmxscindr(jzlsh,jylsh,xm,kh,ybjzlsh,cfrq,yysfxmbm,yysfxmmc,sfxmzxbm,sfxmzxmc,
                                                dj,sl,je,jbr,grbh,sflb) values(
                                                '{0}','{1}','{2}','{3}','{4}','{5}' ,'{6}','{7}','{8}' ,'{9}' ,
                                                '{10}','{11}' ,'{12}','{13}' ,'{14}','{15}' )",
                                                jzlsh, JYLSH, xm, kh, ybjzlsh_snyd, jssj, yyxmbh, yyxmmc, ybxmbh, ybxmmc,
                                                dj, sl, je, jbr, grbh, ybsflbdm);
                        liSQL.Add(strSql);
                    }
                }
                ds.Dispose();

                if (wdzxms.Length > 0)
                {
                    return new object[] { 0, 0, wdzxms.ToString() };
                }

                if (Math.Abs(sfje_total - sfje) > 1)
                    return new object[] { 0, 0, "收费金额与医保结算金额相差" + Math.Abs(sfje_total - sfje) + ",无法结算，请核实!" };

                #endregion

                #region 门诊收费结算
                StringBuilder inputData = new StringBuilder();
                YWBH = "7130";
                inputData.Append(YWBH + "^");
                inputData.Append(YLGHBH + "^");
                inputData.Append(CZYBH + "^");
                inputData.Append(YWZQH + "^");
                inputData.Append(JYLSH + "^");
                inputData.Append(ZXBM + "^");
                inputData.Append(inputParam.ToString().TrimEnd('$') + "^");
                inputData.Append(LJBZ + "^");

                WriteLog(sysdate + "  门诊收费结算|入参|" + inputData.ToString());

                StringBuilder outputData = new StringBuilder(10240);
                int i = BUSINESS_HANDLE(inputData, outputData);

                if (i == 0)//处方上传成功
                {
                    WriteLog(sysdate + "  门诊收费结算|出参|" + outputData.ToString());
                    string[] zysfdjfh = outputData.ToString().Split('^')[2].Split('|');
                    #region 出参
                    /*
                     1	医疗总费用	VARCHAR2(10)		2位小数
                    2	本次账户支付	VARCHAR2(10)		2位小数
                    3	本次现金支付	VARCHAR2(10)		2位小数
                    4	本次基金支付	VARCHAR2(10)		2位小数
                    5	大病基金支付	VARCHAR2(10)		2位小数
                    6	救助金额	VARCHAR2(10)		2位小数
                    7	公务员补助支付	VARCHAR2(10)		2位小数
                    8	甲类费用	VARCHAR2(10)		2位小数
                    9	乙类费用	VARCHAR2(10)		2位小数
                    10	丙类费用	VARCHAR2(10)		2位小数
                    11	自费费用	VARCHAR2(10)		2位小数
                    12	起付标准自付	VARCHAR2(10)		2位小数
                    13	非医保自付	VARCHAR2(10)		2位小数
                    14	乙类药品自付	VARCHAR2(10)		2位小数
                    15	特检特治自付	VARCHAR2(10)		2位小数
                    16	进入统筹自付	VARCHAR2(10)		2位小数
                    17	进入大病自付	VARCHAR2(10)		2位小数
                    18	门诊流水号	VARCHAR2(18)	NOT NULL	
                    19	单据流水号	VARCHAR2(18)	NOT NULL	
                     */
                    //出参
                    outParams_js js = new outParams_js();
                    js.Ylfze = zysfdjfh[0];         //医疗总费用
                    js.Zhzf = zysfdjfh[1];          //本次帐户支付
                    js.Xjzf = zysfdjfh[2];          //本次现金支付
                    js.Tcjjzf = zysfdjfh[3];        //本次统筹支付
                    js.Dejjzf = zysfdjfh[4];        //大病基金支付
                    js.Mzjzfy = zysfdjfh[5];        //救助金额
                    js.Gwybzjjzf = zysfdjfh[6];     //公务员补助支付
                    js.Jlfy = zysfdjfh[7];          //甲类费用
                    js.Ylfy = zysfdjfh[8];          //乙类费用
                    js.Blfy = zysfdjfh[9];          //丙类费用

                    js.Zffy = zysfdjfh[10];         //自费费用
                    js.Qfbzfy = zysfdjfh[11];       //起付标准自付
                    js.Fybzf = zysfdjfh[12];        //非医保自付
                    js.Ylzlfy = zysfdjfh[13];       //乙类药品自付
                    js.Tjzlfy = zysfdjfh[14];       //特检特治自付

                    js.Tcfdzffy = zysfdjfh[15];     //进入统筹自付(统筹分段自付)
                    js.Defdzffy = zysfdjfh[16];     //进入大病自付(大病分段自付)
                    js.Ybmzlsh = zysfdjfh[17];        //门诊流水号
                    js.Jslsh = zysfdjfh[18];        //返回的单据号
                    js.Zbxje = (Convert.ToDecimal(js.Ylfze) - Convert.ToDecimal(js.Xjzf)).ToString();

                    js.Bcjsqzhye = zhye;

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
                       * 居民个人自付二次补偿金额|体检金额|生育基金支付|
                       */
                    js.Zbxje = (Convert.ToDecimal(js.Ylfze) - Convert.ToDecimal(js.Xjzf) - Convert.ToDecimal(js.Dejjzf)).ToString();
                    //其他医保支付=医疗总费用-本次现金支付-本次大病支出-本次统筹支付-本次账户支付
                    js.Qtybzf = (Convert.ToDecimal(js.Ylfze) - Convert.ToDecimal(js.Xjzf) - Convert.ToDecimal(js.Dejjzf) - Convert.ToDecimal(js.Tcjjzf) - Convert.ToDecimal(js.Zhzf)).ToString();
                    //现金支付=本次现金支付+本次大病支出
                    js.Ybxjzf = (Convert.ToDecimal(js.Xjzf) + Convert.ToDecimal(js.Dejjzf)).ToString();

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
                                    js.Jmgrzfecbcje + "|" + js.Tjje + "|" + js.Syjjzf + "|";

                    WriteLog(sysdate + "  门诊收费结算(省内异地)|出参1|" + outputData.ToString());
                    #endregion

                    strSql = string.Format(@"insert into ybfyjsdr(
                                             jzlsh, jylsh ,ylfze,zhzf,xjzf,tcjjzf,dejjzf, mzjzfy,gwybzjjzf,jlfy,
                                             ylfy,blfy,zffy,qfbzfy,fybzf,ylzlfy,tjzlfy,tcfdzffy,defdzffy,yllb,
                                             xm,grbh,kh,cfmxjylsh,ywzqh,djh,ybjzlsh,jslsh,ybmzlsh,jbr,
                                             zbxje,bcjsqzhye,jsrq,qtybfy,zhxjzffy)
                                             values('{0}','{1}','{2}','{3}','{4}','{5}' ,'{6}','{7}','{8}' ,'{9}' ,
                                             '{10}','{11}' ,'{12}','{13}' ,'{14}','{15}','{16}' ,'{17}' ,'{18}','{19}',
                                             '{20}' ,'{21}', '{22}','{23}','{24}','{25}','{26}','{27}','{28}','{29}',
                                            '{30}' ,'{31}','{32}','{33}','{34}')",
                                             jzlsh, JYLSH, js.Ylfze, js.Zhzf, js.Xjzf, js.Tcjjzf, js.Dejjzf, js.Mzjzfy, js.Gwybzjjzf, js.Jlfy,
                                             js.Ylfy, js.Blfy, js.Zffy, js.Qfbzfy, js.Fybzf, js.Ylzlfy, js.Tjzlfy, js.Tcfdzffy, js.Defdzffy, yllb,
                                             xm, grbh, kh, JYLSH, YWZQH, djh, ybjzlsh, js.Jslsh, js.Ybmzlsh, jbr,
                                             js.Zbxje, js.Bcjsqzhye, jssj, js.Qtybzf, js.Ybxjzf);
                    liSQL.Add(strSql);

                    object[] obj_MZSF = liSQL.ToArray();
                    obj_MZSF = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj_MZSF);

                    if (obj_MZSF[1].ToString() == "1")
                    {
                        WriteLog(sysdate + "  门诊收费结算成功|本地数据操作成功|" + strValue);
                        return new object[] { 0, 1, strValue };
                    }
                    else
                    {
                        WriteLog(sysdate + "  门诊登记收费结算成功|本地数据操作失败|" + obj_MZSF[2].ToString());
                        //撤销操作
                        //费用结算撤销信息、处方明细上传撤销
                        object[] objFYJSCX = { jzlsh, js.Jslsh, ybjzlsh, cfsj, grbh, xm, kh, dqbh, ybjzlsh_snyd, JYLSH, 2 };
                        NYBFYJSCX(objFYJSCX);
                        return new object[] { 0, 0, "门诊收费结算成功|本地数据操作失败" + obj_MZSF[2].ToString() };
                    }
                }
                else
                {
                    WriteLog(sysdate + "  门诊收费结算失败|" + outputData.ToString());
                    return new object[] { 0, 0, outputData.ToString() };
                }
                #endregion
            }
            catch (Exception ex)
            {
                WriteLog(sysdate + "  门诊收费结算失败|系统异常|" + ex.Message);
                return new object[] { 0, 2, "系统异常|" + ex.Message };
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

                CZYBH = CliUtils.fLoginUser;   // 操作员工号 
                YWZQH = CliUtils.fLoginYbNo.Split('|')[0].ToString();   // 业务周期号
                string jbr = CliUtils.fUserName;    //经办人
                #endregion

                //交易流水号
                JYLSH = DateTime.Now.ToString("yyyyMMddHHmmss") + "-" + YLGHBH + "-" + new Random().Next(100).ToString().PadLeft(4, '0');

                //判断是否已结算
                string strSql = string.Format(@"select a.cfmxjylsh,a.jslsh,a.jsrq,b.*  from ybfyjsdr a join ybmzzydjdr b on a.ybjzlsh = b.ybjzlsh where a.jzlsh = '{0}' and a.djh = '{1}' and a.cxbz = 1  and b.cxbz = 1 ", jzlsh, djh);
                DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);

                if (ds.Tables[0].Rows.Count == 0)
                {
                    WriteLog(sysdate + "  该患者无结算信息|");
                    return new object[] { 0, 0, "该患者无结算信息" };
                }

                DataRow dr = ds.Tables[0].Rows[0];
                string ybjzlsh = dr["ybjzlsh"].ToString();
                string cfmxjylsh = dr["cfmxjylsh"].ToString();
                string jsrq = dr["jsrq"].ToString();
                string jslsh = dr["jslsh"].ToString();    //结算流水号  
                string grbh = dr["grbh"].ToString();
                string xm = dr["xm"].ToString();
                string kh = dr["kh"].ToString();
                string dqbh = dr["tcqh"].ToString();
                string ybjzlsh_snyd = dr["ybjzlsh_snyd"].ToString();
                DQJBBZ = ds.Tables[0].Rows[0]["DQJBBZ"].ToString();


                #region 入参
                StringBuilder inputParam = new StringBuilder();
                if (DQJBBZ.Equals("1"))
                {
                    /*
                     1		住院流水号(门诊流水号)	VARCHAR2(18)	NOT NULL	同登记时的住院流水号（门诊流水号）
                    2		单据号	VARCHAR2(18)	NOT NULL	唯一
                    3		经办人	VARCHAR2(20)	NOT NULL	医疗机构操作员姓名
                    4		是否保存处方标志	VARCHAR2(3)		0不保存1保存,默认不保存
                    5		结算日期	VARCHAR2(14)		
                     */
                    inputParam.Append(ybjzlsh + "|");
                    inputParam.Append(djh + "|");
                    inputParam.Append(jbr + "|");
                    inputParam.Append("0" + "|");
                    inputParam.Append(jsrq + "|");
                    YWBH = "2430";
                }
                else
                {
                    /*
                     * 1	个人编号	VARCHAR2(10)	NOT NULL	
                    2	姓名	VARCHAR2(20)	NOT NULL	
                    3	卡号	VARCHAR2(18)	NOT NULL	
                    4	地区编号	VARCHAR2(6)	NOT NULL	
                    5	门诊号	VARCHAR2(18)	NOT NULL	
                    6	单据流水号	VARCHAR2(18)	NOT NULL	
                     */
                    YWBH = "7140";
                    inputParam.Append(grbh + "|");
                    inputParam.Append(xm + "|");
                    inputParam.Append(kh + "|");
                    inputParam.Append(dqbh + "|");
                    inputParam.Append(ybjzlsh_snyd + "|");
                    inputParam.Append(jslsh + "|");
                }

                StringBuilder inputData = new StringBuilder();
                inputData.Append(YWBH + "^");
                inputData.Append(YLGHBH + "^");
                inputData.Append(CZYBH + "^");
                inputData.Append(YWZQH + "^");
                inputData.Append(JYLSH + "^");
                inputData.Append(ZXBM + "^");
                inputData.Append(inputParam.ToString() + "^");
                inputData.Append(LJBZ + "^");
                WriteLog(sysdate + "  门诊费用结算撤销|入参|" + inputData.ToString());
                #endregion

                StringBuilder outputData = new StringBuilder(1024);

                int i = BUSINESS_HANDLE(inputData, outputData);

                if (i == 0)
                {
                    WriteLog(sysdate + "  门诊费用结算撤销|出参|" + outputData.ToString());
                    List<string> lijsmxcx = new List<string>();
                    strSql = string.Format(@"insert into ybfyjsdr(jzlsh,jylsh,ylfze,zbxje,tcjjzf,dejjzf,zhzf,xjzf,
                                            gwybzjjzf,qybcylbxjjzf,zffy,dwfdfy,yyfdfy,mzjzfy,cxjfy,ylzlfy,blzlfy,fhjbylfy,
                                            qfbzfy,zzzyzffy,jrtcfy,tcfdzffy,ctcfdxfy,jrdebxfy,defdzffy,cdefdxfy,rgqgzffy,
                                            bcjsqzhye,bntczflj,bndezflj,bnczjmmztczflj,bngwybzzflj,bnzhzflj,bnzycslj,zycs,
                                            xm,jsrq,yllb,yldylb,jbjgbm,ywzqh,jslsh,tsxx,djh,jylx,yyjylsh,yxbz,grbhgl,yljgbm,
                                            ecbcje,mmqflj,jsfjylsh,grbh,dbzbc,czzcje,elmmxezc,elmmxesy,jmgrzfecbcje,tjje,syjjzf,
                                            kh,cfmxjylsh,ybjzlsh,qtybfy,zhxjzffy,cxbz) 
                                            select jzlsh,jylsh,ylfze,zbxje,tcjjzf,dejjzf,zhzf,xjzf,
                                            gwybzjjzf,qybcylbxjjzf,zffy,dwfdfy,yyfdfy,mzjzfy,cxjfy,ylzlfy,blzlfy,fhjbylfy,
                                            qfbzfy,zzzyzffy,jrtcfy,tcfdzffy,ctcfdxfy,jrdebxfy,defdzffy,cdefdxfy,rgqgzffy,
                                            bcjsqzhye,bntczflj,bndezflj,bnczjmmztczflj,bngwybzzflj,bnzhzflj,bnzycslj,zycs,
                                            xm,jsrq,yllb,yldylb,jbjgbm,ywzqh,jslsh,tsxx,djh,jylx,yyjylsh,yxbz,grbhgl,yljgbm,
                                            ecbcje,mmqflj,jsfjylsh,grbh,dbzbc,czzcje,elmmxezc,elmmxesy,jmgrzfecbcje,tjje,syjjzf,
                                            kh,cfmxjylsh,ybjzlsh,qtybfy,zhxjzffy,0 
                                            from ybfyjsdr where jzlsh = '{0}' and djh = '{1}' and cxbz = 1", jzlsh, djh);
                    lijsmxcx.Add(strSql);
                    strSql = string.Format(@"update ybfyjsdr set cxbz = 2 where jzlsh = '{0}' and djh = '{1}' and cxbz = 1", jzlsh, djh);
                    lijsmxcx.Add(strSql);

                    strSql = string.Format(@"insert into ybcfmxscfhdr(jzlsh,jylsh,je,zlje,zfje,cxjzfje,sflb,sfxmdj,qezfbz,zlbl,xj,bz,grbh,xm,kh,cfh,ybcfh,yyxmdm,yyxmmc,cxbz) select jzlsh,jylsh,je,zlje,zfje,cxjzfje,sflb,sfxmdj,qezfbz,zlbl,xj,bz,grbh,xm,kh,cfh,ybcfh,yyxmdm,yyxmmc, 0 from ybcfmxscfhdr where jzlsh = '{0}' and jylsh = '{1}' and cxbz = 1", jzlsh, cfmxjylsh);
                    lijsmxcx.Add(strSql);
                    strSql = string.Format(@"update ybcfmxscfhdr set cxbz = 2 where jzlsh = '{0}' and jylsh = '{1}' and cxbz = 1", jzlsh, cfmxjylsh);
                    lijsmxcx.Add(strSql);

                    strSql = string.Format(@"insert into ybcfmxscindr(jzlsh,jylsh,xm,kh,ybjzlsh,cfrq,yysfxmbm,yysfxmmc,sfxmzxbm,sfxmzxmc,
                                            dj,sl,je,jbr,cxbz,sysdate) 
                                            select jzlsh,jylsh,xm,kh,ybjzlsh,cfrq,yysfxmbm,yysfxmmc,sfxmzxbm,sfxmzxmc,
                                            dj,sl,je,jbr, 0,'{2}' from ybcfmxscindr where jzlsh = '{0}' and jylsh = '{1}' and cxbz = 1", jzlsh, cfmxjylsh, sysdate);
                    lijsmxcx.Add(strSql);
                    strSql = string.Format(@"update ybcfmxscindr set cxbz = 2 where jzlsh = '{0}' and jylsh = '{1}' and cxbz = 1", jzlsh, cfmxjylsh);
                    lijsmxcx.Add(strSql);

                    object[] obj = lijsmxcx.ToArray();
                    obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);
                    if (obj[1].ToString() == "1")
                    {
                        WriteLog(sysdate + "  门诊费用结算撤销成功|本地数据操作成功|" + outputData.ToString());
                        return new object[] { 0, 1, "门诊费用结算撤销成功" };
                    }
                    else
                    {
                        WriteLog(sysdate + "  门诊费用结算撤销成功|本地数据操作成功|" + obj[2].ToString());
                        return new object[] { 0, 0, "数据库操作失败" + obj[2].ToString() };
                    }
                }
                else
                {
                    WriteLog(sysdate + "  门诊费用结算撤销失败|" + outputData.ToString());
                    return new object[] { 0, 0, outputData.ToString() };
                }
            }
            catch (Exception error)
            {
                WriteLog(sysdate + "   门诊费用结算撤销|接口异常|" + error.Message);
                return new object[] { 0, 2, "Error:" + error.Message };
            }
        }
        #endregion

        #region 住院登记
        public static object[] YBZYDJ(object[] objParam)
        {
            #region 入参
            string ickxx = objParam[4].ToString();  //医保读卡返回信息
            DQJBBZ = ickxx.Split('|')[10];
            #endregion
            if (DQJBBZ.Equals("0"))
                return YBZYDJ_SBJ(objParam);
            else
                return YBZYDJ_SNYD(objParam);

        }
        #endregion

        #region 住院登记_市本级
        public static object[] YBZYDJ_SBJ(object[] objParam)
        {
            string sysdate = GetServerDateTime();
            WriteLog(sysdate + "  进入住院登记(市本级)...");
            try
            {
                CZYBH = CliUtils.fLoginUser;    //操作员工号
                YWZQH = CliUtils.fLoginYbNo.Split('|')[0].ToString();    //业务周期号
                #region his参数
                string jbr = CliUtils.fUserName;   // 经办人姓名 
                string jzlsh = objParam[0].ToString(); //就诊流水号
                string yllb = objParam[1].ToString(); //医疗类别代码
                string bzbm = objParam[2].ToString(); //病种编码
                string bzmc = objParam[3].ToString(); //病种名称
                string[] kxx = objParam[4].ToString().Split('|'); //读卡返回信息
                string lyjgdm = objParam[5].ToString();//来源机构代码
                string lyjgmc = objParam[6].ToString();//来源机构名称
                string yllbmc = objParam[7].ToString();//医疗类别名称
                string dgysdm = objParam[8].ToString(); //定岗医生代码
                string dgysxm = objParam[9].ToString(); //定岗医生姓名
                //string zszbm = objParam[10].ToString(); //准生证编号
                //string sylb = objParam[11].ToString();      //生育类别
                //string jhsylb = objParam[12].ToString();    //计划生育类别
                #endregion
                string djsj = "";
                string ksmc = "";
                string ksbh = "";

                #region 读卡信息
                if (kxx.Length < 2)
                    return new object[] { 0, 0, "无读卡信息反馈" };

                string grbh = kxx[0].ToString(); //个人编号
                string dwbm = kxx[1].ToString();  //单位编号
                string sfzh = kxx[2].ToString();  //身份证号
                string xm = kxx[3].ToString();  //姓名
                string xb = kxx[4].ToString();  //性别
                string kh = kxx[7].ToString();  //卡号
                string yldylb = kxx[8].ToString();  //医疗待遇类别
                string ydrybz = kxx[10].ToString();  //异地人员标志
                string ssqh = kxx[11].ToString();  //所属区号
                string zhye = kxx[14].ToString();  //帐户余额
                #endregion

                if (string.IsNullOrEmpty(jzlsh))
                    return new object[] { 0, 0, "就诊流水号不能为空" };
                if (string.IsNullOrEmpty(yllb))
                    return new object[] { 0, 0, "医疗类别不能为空" };
                if (string.IsNullOrEmpty(xm))
                    return new object[] { 0, 0, "读卡信息不能为空" };


                #region 判断是否已入院
                string strSql = string.Format("select z1date as rysj,z1hznm,z1ksno,z1ksnm,'' as z1bedn from zy01h where z1zyno = '{0}'", jzlsh);
                DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                if (ds.Tables[0].Rows.Count == 0)
                {
                    WriteLog(sysdate + "  就诊流水号" + jzlsh + "未登记入院");
                    return new object[] { 0, 0, "就诊流水号" + jzlsh + "未登记入院" };
                }
                djsj = Convert.ToDateTime(ds.Tables[0].Rows[0]["rysj"]).ToString("yyyyMMddHHmmss");
                ksmc = ds.Tables[0].Rows[0]["z1ksnm"].ToString();
                ksmc = ds.Tables[0].Rows[0]["z1ksno"].ToString();
                #endregion
                string ybjzlsh = jzlsh + DateTime.Now.ToString("HHmmss");
                JYLSH = djsj + "-" + YLGHBH + "-" + new Random().Next(100).ToString().PadLeft(4, '0');

                #region 判断是否已登记
                strSql = string.Format(@"select * from ybmzzydjdr where jzlsh='{0}' and cxbz=1", jzlsh);
                ds.Tables.Clear();
                ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    WriteLog(sysdate + "  就诊流水号" + jzlsh + "已存在住院登记");
                    return new object[] { 0, 0, "就诊流水号" + jzlsh + "已经存在住院登记" };
                }
                #endregion

                #region 参数
                /*
                 1		门诊/住院流水号	VARCHAR2(18)	NOT NULL	唯一
                2		医疗类别	VARCHAR2(3)	NOT NULL	代码
                3		挂号日期	VARCHAR2(14)	NOT NULL 	YYYYMMDDHH24MISS
                4		诊断疾病编码	VARCHAR2(20)		必须为中心病种编码
                5		诊断疾病名称	VARCHAR2(50)		
                6		病历信息	VARCHAR2(100)		
                7		科室名称	VARCHAR2(50)		
                8		床位号	VARCHAR2(20)		住院类需要传入床位号，如果入院时没有分配床位号，可以通过调用住院信息修改交易录入床位号
                9		医生代码	VARCHAR2(20)		
                10		医生姓名	VARCHAR2(20)		
                11		挂号费	VARCHAR2(8)		2位小数 只对门诊有用，如要的话传入，否则传0
                12		检查费	VARCHAR2(8)		2位小数 只对门诊有用，如要的话传入，否则传0
                13		经办人	VARCHAR2(20)	NOT NULL	医疗机构操作员姓名
                14		卡号	VARCHAR2(20)	Not  Null	为防止中途换卡，增加入参，标识在本交易前，已经读卡的卡号
                15		个人电脑编号	VARCHAR2(16)	NOT NULL	
                 */
                StringBuilder inputParam = new StringBuilder();
                inputParam.Append(ybjzlsh + "|");
                inputParam.Append(yllb + "|");
                inputParam.Append(djsj + "|");
                inputParam.Append(bzbm + "|");
                inputParam.Append(bzmc + "|");
                inputParam.Append("" + "|");
                inputParam.Append(ksmc + "|");
                inputParam.Append("|");
                inputParam.Append(dgysdm + "|");
                inputParam.Append(dgysxm + "|");
                inputParam.Append("0|");
                inputParam.Append("0|");
                inputParam.Append(jbr + "|");
                inputParam.Append(kh + "|");
                inputParam.Append(CliUtils.fComputerName.Trim() + "|");

                StringBuilder inputData = new StringBuilder();
                YWBH = "2210";
                inputData.Append(YWBH + "^");
                inputData.Append(YLGHBH + "^");
                inputData.Append(CZYBH + "^");
                inputData.Append(YWZQH + "^");
                inputData.Append(JYLSH + "^");
                inputData.Append(ZXBM + "^");
                inputData.Append(inputParam.ToString() + "^");
                inputData.Append(LJBZ + "^");
                WriteLog(sysdate + "  住院登记(市本级)|入参|" + inputData.ToString());
                #endregion

                StringBuilder outputData = new StringBuilder(10240);
                int i = BUSINESS_HANDLE(inputData, outputData);
                if (i == 0)
                {
                    DQJBBZ = "1";
                    List<string> liSQL = new List<string>();
                    WriteLog(sysdate + "  住院登记(市本级)|出参|" + inputData.ToString());

                    strSql = string.Format(@"insert into ybmzzydjdr(
                                            jzlsh,jylsh,ybjzlsh,yllb,ghdjsj,bzbm,bzmc,ksbh,ksmc,ysdm,
                                            ysxm,ghf,jbr,xm,grbh,kh,yldylb,xb,tcqh,zhye,
                                            jzbz,ydrybz,dqjbbz,sysdate) 
                                            values(
                                            '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}',
                                            '{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}',
                                            '{20}','{21}','{22}','{23}')"
                                           , jzlsh, JYLSH, ybjzlsh, yllb, djsj, bzbm, bzmc, ksbh, ksmc, dgysdm,
                                           dgysxm, 0, jbr, xm, grbh, kh, yldylb, xb, ssqh, zhye,
                                           "z", ydrybz, DQJBBZ, sysdate);
                     liSQL.Add(strSql);
                     strSql = string.Format(@"update zy01h set z1rylb = '{0}', z1tcdq = '{1}', z1lyjg = '{2}', z1lynm = '{3}', z1ylno = '{4}'
                                            , z1ylnm = '{5}', z1bzno = '{6}', z1bznm = '{7}', z1ybno = '{8}' where z1comp = '{9}' and z1zyno = '{10}'"
                                             , yldylb, ssqh, lyjgdm, lyjgmc, yllb, yllbmc, bzbm, bzmc, grbh, CliUtils.fSiteCode, jzlsh, kh);
                    liSQL.Add(strSql);

                    object[] obj = liSQL.ToArray();
                    obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);
                    if (obj[1].ToString() == "1")
                    {
                        WriteLog(sysdate + "  住院登记(市本级)成功|本地数据操作成功|" + outputData.ToString());
                        return new object[] { 0, 1, "住院登记成功" };
                    }
                    else
                    {
                        WriteLog(sysdate + "  住院登记(市本级)成功|本地数据操作失败|" + obj[2].ToString());
                        //登记撤销
                        object[] objZYDJCX = { jzlsh, ybjzlsh, yllb, grbh, xm, kh, ssqh, "", DQJBBZ };
                        NYBZYDJCX(objZYDJCX);
                        return new object[] { 0, 0, "住院登记(市本级)成功|本地数据操作失败|" + obj[2].ToString() };
                    }
                }
                else
                {
                    WriteLog(sysdate + "  住院登记(市本级)失败|" + outputData.ToString());
                    return new object[] { 0, 0, outputData.ToString() };
                }
            }
            catch (Exception error)
            {
                WriteLog(sysdate + "  住院登记(市本级)|系统异常|" + error.Message);
                return new object[] { 0, 2, "Error:" + error.Message };
            }
        }
        #endregion

        #region 住院登记_省内异地
        public static object[] YBZYDJ_SNYD(object[] objParam)
        {
            string sysdate = GetServerDateTime();
            WriteLog(sysdate + "  进入住院登记(省内异地)...");
            try
            {
                #region his参数
                CZYBH = CliUtils.fLoginUser;    //操作员工号
                YWZQH = CliUtils.fLoginYbNo.Split('|')[0].ToString();    //业务周期号
                string jbr = CliUtils.fUserName;   // 经办人姓名 
                string jzlsh = objParam[0].ToString();  // 就诊流水号
                string yllb = objParam[1].ToString();   // 医疗类别代码
                string bzbm = objParam[2].ToString();   // 病种编码（慢性病要传，否则传空字符串）
                string bzmc = objParam[3].ToString();   // 病种名称（慢性病要传，否则传空字符串）
                string ickxx = objParam[4].ToString();  //医保读卡返回信息
                string lyjgdm = objParam[5].ToString();//来源机构代码
                string lyjgmc = objParam[6].ToString();//来源机构名称
                string yllbmc = objParam[7].ToString();//医疗类别名称
                string dgysdm = objParam[8].ToString(); //定岗医生代码
                string dgysxm = objParam[9].ToString(); //定岗医生姓名
                string zszbm = objParam[10].ToString(); //准生证编号
                string sylb = objParam[11].ToString();      //生育类别
                //string jhsylb = objParam[12].ToString();    //计划生育类别
                #endregion
                string djsj = "";
                string ksmc = "";
                string ksbh = "";

                if (string.IsNullOrEmpty(jzlsh))
                    return new object[] { 0, 0, "就诊流水号不能为空" };
                if (string.IsNullOrEmpty(yllb))
                    return new object[] { 0, 0, "医疗类别不能为空" };
                if (string.IsNullOrEmpty(ickxx))
                    return new object[] { 0, 0, "读卡信息不能为空" };

                #region 获取医保读卡信息
                string[] ickParam = ickxx.Split('|');

                string grbh = ickParam[0].ToString(); //个人编号
                string dwbm = ickParam[1].ToString();  //单位编号
                string sfzh = ickParam[2].ToString();  //身份证号
                string xm = ickParam[3].ToString();  //姓名
                string xb = ickParam[4].ToString();  //性别
                string kh = ickParam[7].ToString();  //卡号
                string yldylb = ickParam[8].ToString();  //医疗待遇类别
                string ydrybz = ickParam[10].ToString();  //异地人员标志
                string ssqh = ickParam[11].ToString();  //所属区号
                string zhye = ickParam[14].ToString();  //帐户余额
                #endregion

                #region 判断是否已入院
                string strSql = string.Format("select z1date as rysj,z1hznm,z1ksno,z1ksnm,'' as z1bedn from zy01h where z1zyno = '{0}'", jzlsh);
                DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                if (ds == null || ds.Tables[0].Rows.Count == 0)
                {
                    WriteLog(sysdate + "  就诊流水号" + jzlsh + "未登记入院");
                    return new object[] { 0, 0, "省内异地 就诊流水号" + jzlsh + "未登记入院" };
                }
                djsj = Convert.ToDateTime(ds.Tables[0].Rows[0]["rysj"]).ToString("yyyyMMddHHmmss");
                ksmc = ds.Tables[0].Rows[0]["z1ksnm"].ToString();
                ksbh = ds.Tables[0].Rows[0]["z1ksno"].ToString();
                #endregion


                string ybjzlsh = jzlsh + DateTime.Now.ToString("HHmmss");
                JYLSH = djsj + "-" + YLGHBH + "-" + new Random().Next(100).ToString().PadLeft(4, '0');
               
                //判断是否已登记
                strSql = string.Format(@"select * from ybmzzydjdr where jzlsh='{0}' and cxbz=1", jzlsh);
                ds.Tables.Clear();
                ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    WriteLog(sysdate + "  就诊流水号" + jzlsh + "已存在住院登记");
                    return new object[] { 0, 0, "就诊流水号" + jzlsh + "已经存在住院登记" };
                }

                #region 接口参数
                /*
               1	个人编号	VARCHAR2(10)	NOT NULL	
                2	姓名	VARCHAR2(20)	NOT NULL	
                3	卡号	VARCHAR2(18)	NOT NULL	
                4	地区编号	VARCHAR2(6)	NOT NULL	
                5	医疗类别	VARCHAR2(3)	NOT NULL	代码
                6	科室名称	VARCHAR2(20)		
                7	挂号日期	VARCHAR2(14)	NOT NULL	YYYYMMDDHH24MISS
                8	住院床号	VARCHAR2(20)	NOT NULL	
                9	诊断疾病编码	VARCHAR2(20)	NOT NULL	必须为中心病种编码
                10	诊断疾病名称	VARCHAR2(50)		
                11	住院流水号	VARCHAR2(18)	NOT NULL	
                12	主要病情描述	VARCHAR2(1000)		
                13	科室编号	VARCHAR2(6)	NOT NULL	
                14	医生编号	VARCHAR2(50)		
                15	就诊医生	VARCHAR2(50)		
                16	病例号	VARCHAR2(50)		
                17	急诊标志	VARCHAR2(3)		
                18	外伤标识	VARCHAR2(3)		
                19	经办人	VARCHAR2(50)	NOT NULL	
                20	次要疾病诊断编码1	VARCHAR2(20)		
                21	次要疾病诊断编码2	VARCHAR2(20)		
                22	输入附加信息1	VARCHAR2(500)		
                23	输入附加信息2	VARCHAR2(500)		
                24	输入附加信息3	VARCHAR2(500)		
                25	住院类型	VARCHAR2(3)		二级代码
                */

                StringBuilder inputParam = new StringBuilder();
                inputParam.Append(grbh + "|");
                inputParam.Append(xm + "|");
                inputParam.Append(kh + "|");
                inputParam.Append(ssqh + "|");
                inputParam.Append(yllb + "|");
                inputParam.Append(ksmc + "|");
                inputParam.Append(djsj + "|");
                inputParam.Append("01" + "|");
                inputParam.Append(bzbm + "|");
                inputParam.Append(bzmc + "|");
                inputParam.Append(ybjzlsh + "|");
                inputParam.Append("" + "|");
                inputParam.Append(ksbh + "|");
                inputParam.Append(dgysdm + "|");
                inputParam.Append(dgysxm + "|");
                inputParam.Append("" + "|");
                inputParam.Append("0" + "|");
                inputParam.Append("0" + "|");
                inputParam.Append(jbr + "|");
                inputParam.Append("|");
                inputParam.Append("|");
                inputParam.Append("" + "|");
                inputParam.Append("" + "|");
                inputParam.Append("" + "|");
                inputParam.Append("1" + "|");

                StringBuilder inputData = new StringBuilder();
                YWBH = "7210";
                inputData.Append(YWBH + "^");
                inputData.Append(YLGHBH + "^");
                inputData.Append(CZYBH + "^");
                inputData.Append(YWZQH + "^");
                inputData.Append(JYLSH + "^");
                inputData.Append(ZXBM + "^");
                inputData.Append(inputParam.ToString().TrimEnd('$') + "^");
                inputData.Append(LJBZ + "^");

                WriteLog(sysdate + "  住院登记(省内异地)|入参|" + inputData.ToString());
                #endregion
                List<string> liSQL=new List<string>();
                StringBuilder outputData = new StringBuilder(102400);
                int i = BUSINESS_HANDLE(inputData, outputData);
                if (i == 0)
                {
                    DQJBBZ = "2";
                    WriteLog(sysdate + "  住院登记(省内异地)|出参|" + outputData.ToString());
                    string[] sReturn = outputData.ToString().Split('^')[2].Split('|');
                    string ybjzlsh_snyd = sReturn[0]; //省内异地返回特定流水号

                 

                    strSql = string.Format(@"insert into ybmzzydjdr(
                                                    jzlsh,jylsh,ybjzlsh,yllb,ghdjsj,bzbm,bzmc,ksbh,ksmc,ysdm,
                                                    ysxm,ghf,jbr,xm,grbh,kh,yldylb,xb,tcqh,zhye,
                                                    jzbz,ydrybz,dqjbbz,ybjzlsh_snyd,sysdate) 
                                                    values(
                                                    '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}',
                                                    '{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}',
                                                    '{20}','{21}','{22}','{23}','{24}')"
                                                   , jzlsh, JYLSH, ybjzlsh, yllb, djsj, bzbm, bzmc, ksbh, ksmc, dgysdm,
                                                   dgysxm, 0, jbr, xm, grbh, kh, yldylb, xb, ssqh, zhye,
                                                   "z", ydrybz, DQJBBZ,ybjzlsh_snyd,sysdate);
                    liSQL.Add(strSql);
                    strSql = string.Format(@"update zy01h set z1rylb = '{0}', z1tcdq = '{1}', z1lyjg = '{2}', z1lynm = '{3}', z1ylno = '{4}'
                                            , z1ylnm = '{5}', z1bzno = '{6}', z1bznm = '{7}', z1ybno = '{8}' where z1comp = '{9}' and z1zyno = '{10}'"
                                            , yldylb, ssqh, lyjgdm, lyjgmc, yllb, yllbmc, bzbm, bzmc, grbh, CliUtils.fSiteCode, jzlsh, kh);
                    liSQL.Add(strSql);

                    object[] obj = liSQL.ToArray();
                    obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);

                    if (obj[1].ToString() == "1")
                    {
                        WriteLog(sysdate + "  住院登记(省内异地)成功|本地数据操作成功|" + outputData.ToString());
                        return new object[] { 0, 1, "住院登记成功" };
                    }
                    else
                    {
                        WriteLog(sysdate + "  住院登记(省内异地)成功|本地数据操作失败|" + outputData.ToString());
                        //登记撤销
                        object[] objZYDJCX = { jzlsh, ybjzlsh, yllb, grbh, xm, kh, ssqh, ybjzlsh_snyd, 2 };
                        NYBZYDJCX(objZYDJCX);
                        return new object[] { 0, 0, "住院登记(省内异地)成功|本地数据操作失败|" + obj[1].ToString() };
                    }
                }
                else
                {
                    WriteLog(sysdate + "  住院登记(省内异地)失败|" + outputData.ToString());
                    return new object[] { 0, 0, outputData.ToString() };
                }
            }
            catch (Exception error)
            {
                WriteLog(sysdate + "  住院登记(省内异地)|系统异常|" + error.Message);
                return new object[] { 0, 2, "Error:" + error.Message };
            }
        }
        #endregion

        #region 住院登记撤销
        public static object[] YBZYDJCX(object[] objParam)
        {
            string sysdate = GetServerDateTime();
            WriteLog(sysdate + "  进入住院登记撤销...");
            try
            {
                string jzlsh = objParam[0].ToString();  // 就诊流水号
                CZYBH = CliUtils.fLoginUser;  // 操作员工号
                YWZQH = CliUtils.fLoginYbNo.Split('|')[0].ToString();  // 业务周期号
                string jbr = CliUtils.fLoginUser;


                //交易流水号
                JYLSH = DateTime.Now.ToString("yyyyMMddHHmmss") + "-" + YLGHBH + "-" + new Random().Next(100).ToString().PadLeft(4, '0');

                string strSql = string.Format("select jzlsh from ybcfmxscfhdr a where a.jzlsh = '{0}' and a.cxbz = 1", jzlsh);
                DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);

                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    WriteLog(sysdate + "    已上传费用明细不能冲销入院登记");
                    return new object[] { 0, 0, "已上传费用明细不能冲销入院登记" };
                }

                strSql = string.Format("select grbh,xm,kh,tcqh,ybjzlsh,ybjzlsh_snyd,yllb,DQJBBZ from ybmzzydjdr a where a.jzlsh = '{0}' and a.cxbz = 1", jzlsh);
                ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);

                if (ds == null || ds.Tables[0].Rows.Count == 0)
                {
                    WriteLog(sysdate + "  未办理入院登记|");
                    return new object[] { 0, 0, "未办理入院登记" };
                }

                DataRow dr = ds.Tables[0].Rows[0];
                string ybjzlsh = dr["ybjzlsh"].ToString();   // 医保就诊流水号
                string grbh = dr["grbh"].ToString();         // 个人编号
                string xm = dr["xm"].ToString();             // 姓名
                string kh = dr["kh"].ToString();             // 卡号
                string dqbh = dr["tcqh"].ToString();         // 地区编号
                string ybjzlsh_snyd = dr["ybjzlsh_snyd"].ToString();
                string yllb = dr["yllb"].ToString();
                DQJBBZ = dr["DQJBBZ"].ToString();

                StringBuilder inputParam = new StringBuilder();
                if (DQJBBZ.Equals("1"))
                {
                    YWBH = "2240";
                    inputParam.Append(ybjzlsh + "|");
                    inputParam.Append(jbr + "|");
                    inputParam.Append(yllb + "|");
                }
                else
                {
                    YWBH = "7220";
                    inputParam.Append(grbh + "|");
                    inputParam.Append(xm + "|");
                    inputParam.Append(kh + "|");
                    inputParam.Append(dqbh + "|");
                    inputParam.Append(ybjzlsh_snyd + "|");
                    inputParam.Append(ybjzlsh + "|");
                    inputParam.Append(jbr + "|");
                }

                StringBuilder inputData = new StringBuilder();
                inputData.Append(YWBH + "^");
                inputData.Append(YLGHBH + "^");
                inputData.Append(CZYBH + "^");
                inputData.Append(YWZQH + "^");
                inputData.Append(JYLSH + "^");
                inputData.Append(ZXBM + "^");
                inputData.Append(inputParam.ToString().TrimEnd('$') + "^");
                inputData.Append(LJBZ + "^");

                WriteLog(sysdate + "  住院登记撤销|入参|" + inputData.ToString());
                StringBuilder outputData = new StringBuilder(102400);
                int i = BUSINESS_HANDLE(inputData, outputData);
                if (i == 0)
                {
                    List<string> liSql = new List<string>();
                    WriteLog(sysdate + "住院登记撤销|出参|" + inputData.ToString());

                    strSql = string.Format(@"insert into ybmzzydjdr(jzlsh,jylsh,ybjzlsh,yllb,ghdjsj,bzbm,bzmc,ksbh,ksmc,ysdm,
                                            ysxm,ghf,jbr,xm,grbh,kh,yldylb,xb,tcqh,zhye,
                                            jzbz,ydrybz,dqjbbz,ybjzlsh_snyd,cxbz,sysdate)
                                            select jzlsh,jylsh,ybjzlsh,yllb,ghdjsj,bzbm,bzmc,ksbh,ksmc,ysdm,
                                            ysxm,ghf,jbr,xm,grbh,kh,yldylb,xb,tcqh,zhye,
                                            jzbz,ydrybz,dqjbbz,ybjzlsh_snyd,0,'{1}' from ybmzzydjdr where jzlsh = '{0}' and cxbz = 1", jzlsh, sysdate, jbr);
                    liSql.Add(strSql);
                    strSql = string.Format("update ybmzzydjdr set cxbz = 2 where jzlsh = '{0}' and cxbz = 1", jzlsh);
                    liSql.Add(strSql);

                    object[] obj = liSql.ToArray();

                    obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);

                    if (obj[1].ToString() == "1")
                    {
                        WriteLog(sysdate + "  住院登记撤销成功|本地数据操作成功|" + outputData.ToString());
                        return new object[] { 0, 1, "住院登记撤销成功" };
                    }
                    else
                    {
                        WriteLog(sysdate + "  住院登记撤销成功|本地数据操作失败|" + obj[2].ToString());
                        return new object[] { 0, 0, "数据库操作失败" + obj[2].ToString() };
                    }
                }
                else
                {
                    WriteLog(sysdate + "  住院登记撤销失败|" + outputData.ToString());
                    return new object[] { 0, 0, outputData.ToString() };
                }
            }
            catch (Exception error)
            {
                WriteLog(sysdate + "  住院登记撤销|系统异常|" + error.Message);
                return new object[] { 0, 2, "Error:" + error.Message };
            }
        }
        #endregion

        #region 住院费用登记
        public static object[] YBZYSFDJ(object[] objParam)
        {
            string jzlsh = objParam[0].ToString();  //就诊流水号
            string ztjssj = objParam[1].ToString(); //中途结算时间
            //判断是否医保登记
            string strSql = string.Format("select * from ybmzzydjdr a where a.jzlsh = '{0}' and jzbz='z' and a.cxbz = 1", jzlsh);
            DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            if (ds == null || ds.Tables[0].Rows.Count == 0)
                return new object[] { 0, 0, "无医保住院登记信息" };
            string grbh = ds.Tables[0].Rows[0]["grbh"].ToString();
            DQJBBZ = ds.Tables[0].Rows[0]["DQJBBZ"].ToString();

            if (DQJBBZ.Equals("1"))
                return YBZYCFMXSC_SBJ(objParam);
            else
                return YBZYCFMXSC_SNYD(objParam);
        }
        #endregion

        #region 住院费用登记_市本级
        public static object[] YBZYCFMXSC_SBJ(object[] objParam)
        {
            string sysdate = GetServerDateTime();
            WriteLog(sysdate + "  进入住院费用登记（市本级）...");

            try
            {//经办人姓名
                string jzlsh = objParam[0].ToString();  //就诊流水号
                string ztjssj = objParam[1].ToString(); //中途结算时间

                CZYBH = CliUtils.fLoginUser;  //操作员工号
                YWZQH = CliUtils.fLoginYbNo.Split('|')[0].ToString();  //业务周期号
                string jbr = CliUtils.fUserName;
                string cfsj = Convert.ToDateTime(sysdate).ToString("yyyyMMddHHmmss");
                string ztjssj1 = "";
                JYLSH = cfsj + "-" + YLGHBH + "-" + new Random().Next(100).ToString().PadLeft(4, '0');
                YWBH = "2310";

                if (!string.IsNullOrEmpty(ztjssj))
                {
                    ztjssj1 = Convert.ToDateTime(ztjssj).AddDays(1).ToString("yyyy-MM-dd");
                }
                #region 判断是否医保登记
                string strSql = string.Format("select * from ybmzzydjdr a where a.jzlsh = '{0}' and jzbz='z' and a.cxbz = 1", jzlsh);
                DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                if (ds == null || ds.Tables[0].Rows.Count == 0)
                    return new object[] { 0, 0, "无医保住院登记信息" };

                DataRow dr1 = ds.Tables[0].Rows[0];
                string ybjzlsh = dr1["ybjzlsh"].ToString();  //医保就诊流水号
                string grbh = dr1["grbh"].ToString();        //个人编号
                string xm = dr1["xm"].ToString();            //姓名    
                string kh = dr1["kh"].ToString();            //卡号
                string ysdm = dr1["ysdm"].ToString();
                string ysxm = dr1["ysxm"].ToString();
                string ksmc = dr1["ksmc"].ToString();
                string tcqh = dr1["tcqh"].ToString();
                DQJBBZ = dr1["dqjbbz"].ToString();
                #endregion

                List<string> liSQL = new List<string>();
                #region 获取费用明细信息
                strSql = string.Format(@"select y.ybxmbh, y.ybxmmc, a.z3djxx as dj
                                        , sum(case left(a.z3endv, 1) when '4' then -a.z3jzcs else a.z3jzcs end) as sl
                                        , sum(case left(a.z3endv, 1) when '4' then -a.z3jzje else a.z3jzje end) as je
                                        , a.z3item as yyxmbh, a.z3name as yyxmmc, min(a.z3empn) as ysdm, min(a.z3kdys) as ysxm
                                        , z3sfno as sfno, y.sfxmzldm as ybsfxmzldm, y.sflbdm as ybsflbdm,max(a.z3date) as yysj,y.sfxmdjdm
                                        from zy03d a 
                                        left join ybhisdzdr y on a.z3item = y.hisxmbh 
                                        where a.z3ybup is null and left(a.z3kind, 1) in ('2', '4') and isnull(a.z3jshx,'')=''  and a.z3zyno = '{0}' ", jzlsh);
                if (!string.IsNullOrEmpty(ztjssj))
                    strSql += string.Format(@"and Convert(datetime,z3date)<'{0}' ", ztjssj1);
                strSql += string.Format(@"group by y.ybxmbh, y.ybxmmc, a.z3djxx, a.z3name, a.z3item
                                        ,z3sfno,y.sfxmzldm, y.sflbdm,y.sfxmdjdm 
                                        having sum(case left(a.z3endv, 1) when '4' then -a.z3jzje else a.z3jzje end) > 0");
                ds.Tables.Clear();
                ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                List<string> li_inputData = new List<string>();


                if (ds.Tables[0].Rows.Count > 0)
                {
                    StringBuilder strMsg = new StringBuilder();
                    int index = 1;
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        if (dr["ybxmbh"] == DBNull.Value)
                        {
                            strMsg.Append("项目代码：[" + dr["yyxmbh"].ToString() + "]，名称：[" + dr["yyxmmc"].ToString() + "]未对照，不能上传！\r\n");
                        }
                        else
                        {
                            string ybsfxmzldm = dr["ybsfxmzldm"].ToString(); // 收费项目种类代码 
                            string ybsflbdm = dr["ybsflbdm"].ToString();     // 收费类别代码
                            string ybxmbh = dr["ybxmbh"].ToString();
                            string ybxmmc = dr["ybxmmc"].ToString();
                            string yyxmbh = dr["yyxmbh"].ToString();
                            string yyxmmc = dr["yyxmmc"].ToString();
                            string ybxmdj = dr["sfxmdjdm"].ToString();
                            double dj = Convert.ToDouble(dr["dj"]);
                            double sl = Convert.ToDouble(dr["sl"]);
                            double je = Convert.ToDouble(dr["je"]);
                            string fysj = Convert.ToDateTime(dr["yysj"].ToString()).ToString("yyyyMMddHHmmss");
                            string gg = "";
                            string jx = "";
                            string jldw = "";
                            string yfyl = "";
                            string ybcfh = cfsj + index.ToString();
                            string ypjldw = "";
                            index++;
                            #region 入参
                            /*
                               1		住院流水号(门诊流水号)	VARCHAR2(18)	NOT NULL	同登记时的住院流水号（门诊流水号）
                                2		项目类别	VARCHAR2(3)	NOT NULL	代码
                                3		费用类别	VARCHAR2(3)	NOT NULL	代码
                                4		处方号	VARCHAR2(20)	NOT NULL	
                                5		处方日期	VARCHAR2(14)	NOT NULL	YYYYMMDDHH24MISS
                                6		医院收费项目内码	VARCHAR2(20)	NOT NULL	
                                7		收费项目中心编码	VARCHAR2(20)	NOT NULL	中心编号
                                8		医院收费项目名称	VARCHAR2(50)	NOT NULL	
                                9		单价	VARCHAR2(12)	NOT NULL	4位小数
                                10		数量	VARCHAR2(12)	NOT NULL	2位小数
                                11		剂型	VARCHAR2(3)		二级代码
                                12		规格	VARCHAR2(100)		
                                13		每次用量	VARCHAR2(12)		2位小数
                                14		使用频次	VARCHAR2(20)		
                                15		医生姓名	VARCHAR2(20)		传处方医师姓名
                                16		处方医师	VARCHAR2(20)		传处方医师编码
                                17		用法	VARCHAR2(100)		
                                18		单位	VARCHAR2(20)		
                                19		科别名称	VARCHAR2(20)		
                                20		执行天数	VARCHAR2(4)		
                                21		草药单复方标志	VARCHAR2(3)		代码
                                22		经办人	VARCHAR2(20)		医疗机构操作员姓名
                                23		药品剂量单位	VARCHAR2(20)		
                                */
                            StringBuilder inputParam = new StringBuilder();

                            inputParam.Append(ybjzlsh + "|");
                            inputParam.Append(ybsfxmzldm + "|");
                            inputParam.Append(ybsflbdm + "|");
                            inputParam.Append(ybcfh + "|");
                            inputParam.Append(fysj + "|");
                            inputParam.Append(yyxmbh + "|");
                            inputParam.Append(ybxmbh + "|");
                            inputParam.Append(yyxmmc + "|");
                            inputParam.Append(dj + "|");
                            inputParam.Append(sl + "|");
                            inputParam.Append(jx + "|");
                            inputParam.Append(gg + "|");
                            inputParam.Append("|");
                            inputParam.Append("" + "|");    //使用频次
                            inputParam.Append(ysdm + "|");
                            inputParam.Append(ysxm + "|");
                            inputParam.Append("" + "|");    //用法
                            inputParam.Append("" + "|");    //单位
                            inputParam.Append(ksmc + "|");
                            inputParam.Append("" + "|");
                            inputParam.Append("|");
                            inputParam.Append(jbr + "|");
                            inputParam.Append(ypjldw + "|$");
                            li_inputData.Add(inputParam.ToString());

                            strSql = string.Format(@"insert into ybcfmxscindr(jzlsh,jylsh,xm,kh,ybjzlsh,cfrq,yysfxmbm,yysfxmmc,sfxmzxbm,sfxmzxmc,
                                            dj,sl,je,jbr,sysdate,sflb,sfxmdj) values(
                                            '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}',
                                            '{10}','{11}','{12}','{13}','{14}','{15}','{16}')",
                                                jzlsh, JYLSH, xm, kh, ybjzlsh, fysj, yyxmbh, yyxmmc, ybxmbh, ybxmmc,
                                                dj, sl, je, jbr, sysdate, ybsflbdm, ybxmdj);
                            liSQL.Add(strSql);
                            #endregion
                        }
                    }
                    if (!string.IsNullOrEmpty(strMsg.ToString()))
                        return new object[] { 0, 0, strMsg.ToString() };
                }
                else
                    return new object[] { 0, 0, "无费用明细" };
                ds.Dispose();
                #endregion

                #region 费用上传
                StringBuilder inputData = new StringBuilder();
                StringBuilder outputData = new StringBuilder(102400);
                int iTemp = 0;
                #region 分段上传 每次50条
                foreach (string inputData3 in li_inputData)
                {
                    if (iTemp < 50)
                    {
                        inputData.Append(inputData3.ToString());
                        iTemp++;
                    }
                    else
                    {
                        StringBuilder outData = new StringBuilder(102400);
                        StringBuilder inputData1 = new StringBuilder();
                        WriteLog(sysdate + "  " + jzlsh + " 进入住院费用明细上传(分段)...");

                        inputData1.Append(YWBH + "^");
                        inputData1.Append(YLGHBH + "^");
                        inputData1.Append(CZYBH + "^");
                        inputData1.Append(YWZQH + "^");
                        inputData1.Append(JYLSH + "^");
                        inputData1.Append(ZXBM + "^");
                        inputData1.Append(inputData.ToString().TrimEnd('$') + "^");
                        inputData1.Append(LJBZ + "^");

                        WriteLog(sysdate + "  进入住院费用明细上传(市本级)(分段)|入参|" + inputData1.ToString());
                        int i = BUSINESS_HANDLE(inputData1, outputData);
                      
                        if (i != 0)
                        {
                            WriteLog(sysdate + "  " + jzlsh + " 进入住院费用明细上传(分段)失败|" + outputData.ToString());
                            return new object[] { 0, 0, "住院收费登记失败|" + outputData.ToString().Split('^')[2] };
                        }
                        WriteLog(sysdate + "  住院费用明细上传(市本级)(分段)|出参|" + outData.ToString());

                        string[] zysfdjfhs = outputData.ToString().Split('^')[2].TrimEnd('$').Split('$');
                        List<string> lizysfdjfh = new List<string>();

                        for (int j = 0; j < zysfdjfhs.Length; j++)
                        {
                            //金额 、 自理金额 、 自费金额 、 收费项目等级 、 全额自费标志 、 限制使用范围标志 、 限制使用范围
                            string[] zysfdjfh = zysfdjfhs[j].Split('|');
                            outParams_fymx op = new outParams_fymx();
                            op.Je = zysfdjfh[0];
                            op.Zlje = zysfdjfh[1];
                            op.Zfje = zysfdjfh[2];
                            op.Sfxmdj = zysfdjfh[3];
                            op.Zfbz = zysfdjfh[4];
                            op.Xzsybz = zysfdjfh[5];
                            op.Bz = zysfdjfh[6];

                            strSql = string.Format(@"insert into ybcfmxscfhdr(jzlsh,jylsh,je,zlje,zfje,sfxmdj,zfbz,xzsybz, bz,ybjzlsh) 
                                            values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}')",
                                                    jzlsh, JYLSH, op.Je, op.Zlje, op.Zfje, op.Sfxmdj, op.Zfbz, op.Xzsybz, op.Bz, ybjzlsh);
                            liSQL.Add(strSql);
                        }

                        iTemp = 1;
                        inputData.Remove(0, inputData.Length);
                        inputData.Append(inputData3.ToString());
                    }
                }
                #endregion
                #region 明细不足50条时，一次性上传
                if (iTemp > 0)
                {
                    StringBuilder outData = new StringBuilder(102400);
                    StringBuilder retMsg = new StringBuilder(102400);
                    WriteLog(sysdate + "  " + jzlsh + " 进入住院费用明细上传(补传、一次性上传)...");
                    StringBuilder inputData1 = new StringBuilder();

                    inputData1.Append(YWBH + "^");
                    inputData1.Append(YLGHBH + "^");
                    inputData1.Append(CZYBH + "^");
                    inputData1.Append(YWZQH + "^");
                    inputData1.Append(JYLSH + "^");
                    inputData1.Append(ZXBM + "^");
                    inputData1.Append(inputData.ToString().TrimEnd('$') + "^");
                    inputData1.Append(LJBZ + "^");

                    WriteLog(sysdate + " 进入住院费用明细上传(补传、一次性上传)|入参|" + inputData1.ToString());
                    int i = BUSINESS_HANDLE(inputData1, outputData);
                    if (i != 0)
                    {
                        WriteLog(sysdate + "  " + jzlsh + " 进入住院费用明细上传(补传、一次性上传)失败|" + outputData.ToString());
                        return new object[] { 0, 0, "住院收费登记失败|" + outputData.ToString().Split('^')[2] };

                    }
                    WriteLog(sysdate + " 进入住院费用明细上传(补传、一次性上传)|出参|" + outputData.ToString().Split('^')[2]);
                    string[] zysfdjfhs = outputData.ToString().Split('^')[2].TrimEnd('$').Split('$');
                    List<string> lizysfdjfh = new List<string>();

                    for (int j = 0; j < zysfdjfhs.Length; j++)
                    {
                        //金额 、 自理金额 、 自费金额 、 收费项目等级 、 全额自费标志 、 限制使用范围标志 、 限制使用范围
                        string[] zysfdjfh = zysfdjfhs[j].Split('|');
                        outParams_fymx op = new outParams_fymx();
                        op.Je = zysfdjfh[0];
                        op.Zlje = zysfdjfh[1];
                        op.Zfje = zysfdjfh[2];
                        op.Sfxmdj = zysfdjfh[3];
                        op.Zfbz = zysfdjfh[4];
                        op.Xzsybz = zysfdjfh[5];
                        op.Bz = zysfdjfh[6];

                        strSql = string.Format(@"insert into ybcfmxscfhdr(jzlsh,jylsh,je,zlje,zfje,sfxmdj,zfbz,xzsybz, bz,ybjzlsh) 
                                            values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}')",
                                                jzlsh, JYLSH, op.Je, op.Zlje, op.Zfje, op.Sfxmdj, op.Zfbz, op.Xzsybz, op.Bz, ybjzlsh);
                        liSQL.Add(strSql);
                    }
                }
                #endregion
               
                #endregion
                strSql = string.Format(@"update zy03d set z3ybup = '{0}' where z3ybup is null and LEFT(z3kind,1)=2 and z3zyno = '{1}' ", JYLSH, jzlsh);
                if (!string.IsNullOrEmpty(ztjssj))
                    strSql += string.Format(@"and Convert(datetime,z3date)<'{0}' ", ztjssj1);
                liSQL.Add(strSql);
                object[] obj = liSQL.ToArray();
                obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);

                if (obj[1].ToString() == "1")
                {
                    WriteLog(sysdate + "    住院处方明细上传(市本级)成功|本地数据操作成功|" + outputData.ToString());
                    return new object[] { 0, 1, JYLSH };
                }
                else
                {
                    WriteLog(sysdate + "    院处明细上传(市本级)成功|本地数据操作失败|" + obj[2].ToString());
                    object[] objFYMXCX = { ybjzlsh, JYLSH, jbr, grbh, xm, kh, tcqh, "",DQJBBZ };
                    NYBZYCFMXSCCX(objFYMXCX);
                    return new object[] { 0, 0, "上传住院处方明细失败|" + obj[2].ToString() };
                }
            }
            catch (Exception error)
            {
                WriteLog(sysdate + "  住院费用登记|接口异常|" + error.ToString());
                return new object[] { 0, 2, "Error:" + error.Message };
            }
        }
        #endregion

        #region 住院费用登记_省内异地
        public static object[] YBZYCFMXSC_SNYD(object[] objParam)
        {
            string sysdate = GetServerDateTime();
            WriteLog(sysdate + "  进入住院收费登记(省内异地)...");

            try
            {
                string jzlsh = objParam[0].ToString();  //就诊流水号
                string ztjssj = objParam[1].ToString(); //中途结算时间

                CZYBH = CliUtils.fLoginUser;  //操作员工号
                YWZQH = CliUtils.fLoginYbNo.Split('|')[0].ToString();  //业务周期号
                string jbr = CliUtils.fUserName;    //经办人姓名
                string cfsj = Convert.ToDateTime(sysdate).ToString("yyyyMMddHHmmss");
                JYLSH = cfsj + "-" + YLGHBH + "-" + new Random().Next(100).ToString().PadLeft(4, '0');
                string ztjssj1 = "";
                YWBH = "7310";


                if (!string.IsNullOrEmpty(ztjssj))
                {
                    ztjssj1 = Convert.ToDateTime(ztjssj).AddDays(1).ToString("yyyy-MM-dd");
                }

                #region 判断是否医保登记
                string strSql = string.Format("select * from ybmzzydjdr a where a.jzlsh = '{0}' and jzbz='z' and a.cxbz = 1", jzlsh);
                DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                if (ds == null || ds.Tables[0].Rows.Count == 0)
                    return new object[] { 0, 0, "无医保住院登记信息" };

                DataRow dr1 = ds.Tables[0].Rows[0];
                string ybjzlsh = dr1["ybjzlsh"].ToString();  //医保就诊流水号
                string grbh = dr1["grbh"].ToString();        //个人编号
                string xm = dr1["xm"].ToString();            //姓名    
                string kh = dr1["kh"].ToString();            //卡号
                string ysdm = dr1["ysdm"].ToString();
                string ysxm = dr1["ysxm"].ToString();
                string ksmc = dr1["ksmc"].ToString();
                string dqbh = dr1["tcqh"].ToString().Trim();        //地区编号
                string ybjzlsh_sndy = dr1["ybjzlsh_snyd"].ToString().Trim();  //医保就诊流水号
                DQJBBZ = dr1["dqjbbz"].ToString();
                #endregion

                List<string> liSQL = new List<string>();
                #region 获取费用明细信息
                strSql = string.Format(@"select y.ybxmbh, y.ybxmmc, a.z3djxx as dj
                                        , sum(case left(a.z3endv, 1) when '4' then -a.z3jzcs else a.z3jzcs end) as sl
                                        , sum(case left(a.z3endv, 1) when '4' then -a.z3jzje else a.z3jzje end) as je
                                        , a.z3item as yyxmbh, a.z3name as yyxmmc, min(a.z3empn) as ysdm, min(a.z3kdys) as ysxm
                                        , z3sfno as sfno, y.sfxmzldm as ybsfxmzldm, y.sflbdm as ybsflbdm,max(a.z3date) as yysj,y.sfxmdjdm
                                        from zy03d a 
                                        left join ybhisdzdr y on a.z3item = y.hisxmbh 
                                        where a.z3ybup is null and left(a.z3kind, 1) in ('2', '4') and isnull(a.z3jshx,'')=''  and a.z3zyno = '{0}' ", jzlsh);
                if (!string.IsNullOrEmpty(ztjssj))
                    strSql += string.Format(@"and Convert(datetime,z3date)<'{0}' ", ztjssj1);
                strSql += string.Format(@"group by y.ybxmbh, y.ybxmmc, a.z3djxx, a.z3name, a.z3item
                                        ,z3sfno,y.sfxmzldm, y.sflbdm ,y.sfxmdjdm
                                        having sum(case left(a.z3endv, 1) when '4' then -a.z3jzje else a.z3jzje end) > 0");

                ds.Tables.Clear();
                ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                List<string> li_inputData = new List<string>();


                if (ds.Tables[0].Rows.Count > 0)
                {
                    StringBuilder strMsg = new StringBuilder();
                    int index = 1;
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        if (dr["ybxmbh"] == DBNull.Value)
                        {
                            strMsg.Append("项目代码：[" + dr["yyxmbh"].ToString() + "]，名称：[" + dr["yyxmmc"].ToString() + "]未对照，不能上传！\r\n");
                        }
                        else
                        {
                            string ybsfxmzldm = dr["ybsfxmzldm"].ToString(); // 收费项目种类代码 
                            string ybsflbdm = dr["ybsflbdm"].ToString();     // 收费类别代码
                            string ybxmbh = dr["ybxmbh"].ToString();
                            string ybxmmc = dr["ybxmmc"].ToString();
                            string yyxmbh = dr["yyxmbh"].ToString();
                            string yyxmmc = dr["yyxmmc"].ToString();
                            string ybsfxmdj = dr["sfxmdjdm"].ToString();
                            double dj = Convert.ToDouble(dr["dj"]);
                            double sl = Convert.ToDouble(dr["sl"]);
                            double je = Convert.ToDouble(dr["je"]);
                            string fysj = Convert.ToDateTime(dr["yysj"].ToString()).ToString("yyyyMMddHHmmss");
                            string gg = "";
                            string jx = "";
                            string jxdw = "";
                            string jldw = "";
                            string yfyl = "";
                            string ybcfh = cfsj + index.ToString();
                            string ypjldw = "";
                            index++;
                            #region 入参
                            /*
                                1	收费项目中心编码	VARCHAR2(20)	NOT NULL	中心编码
                                2	单价	VARCHAR2(12)	NOT NULL	4位小数
                                3	数量	VARCHAR2(12)	NOT NULL	2位小数
                                4	金额	VARCHAR2(12)	NOT NULL	
                                5	处方日期	VARCHAR2(14)	NOT NULL	YYYYMMDDHH24MISS
                                6	医院收费项目内码	VARCHAR2(20)	NOT NULL	
                                7	医院收费项目名称	VARCHAR2(50)	NOT NULL	
                                8	剂型	VARCHAR2(20)		
                                9	剂型单位	VARCHAR2(20)		
                                10	单次用量	NUMBER(8,2)		
                                11	规格	VARCHAR2(50)		
                                12	产地	VARCHAR2(100)		
                                13	用法	VARCHAR2(50)		
                                14	科室	VARCHAR2(50)		
                                15	开方科室	VARCHAR2(50)		
                                16	经办人	VARCHAR2(50)	NOT NULL	
                                17	经办时间	VARCHAR2(14)	NOT NULL	YYYYMMDDHH24MISS
                                */
                            StringBuilder inputParam1 = new StringBuilder();

                            //inputParam1.Append(ybsfxmzldm + "|");
                            //inputParam1.Append(ybsflbdm + "|");
                            //inputParam1.Append(ybcfh + "|");
                            inputParam1.Append(ybxmbh + "|");
                            inputParam1.Append(dj + "|");
                            inputParam1.Append(sl + "|");
                            inputParam1.Append(je + "|");
                            inputParam1.Append(fysj + "|");
                            inputParam1.Append(yyxmbh + "|");
                            inputParam1.Append(yyxmmc + "|");
                            inputParam1.Append(jx + "|");
                            inputParam1.Append(jxdw + "|");
                            inputParam1.Append("|");
                            inputParam1.Append(gg + "|");
                            inputParam1.Append("|");
                            inputParam1.Append("|");
                            inputParam1.Append(ksmc + "|");
                            inputParam1.Append(jbr + "|");
                            inputParam1.Append(cfsj + "|$");
                            li_inputData.Add(inputParam1.ToString());


                            strSql = string.Format(@"insert into ybcfmxscindr(jzlsh,jylsh,xm,kh,ybjzlsh,cfrq,yysfxmbm,yysfxmmc,sfxmzxbm,sfxmzxmc,
                                            dj,sl,je,jbr,sysdate,sflb,sfxmdj) values(
                                            '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}',
                                            '{10}','{11}','{12}','{13}','{14}','{15}','{16}')",
                                                jzlsh, JYLSH, xm, kh, ybjzlsh, fysj, yyxmbh, yyxmmc, ybxmbh, ybxmmc,
                                                dj, sl, je, jbr, sysdate, ybsflbdm, ybsfxmdj);
                            liSQL.Add(strSql);
                            #endregion
                        }
                    }
                    if (!string.IsNullOrEmpty(strMsg.ToString()))
                        return new object[] { 0, 0, strMsg.ToString() };
                }
                else
                    return new object[] { 0, 0, "无费用明细" };
                ds.Dispose();
                #endregion

                #region 费用上传
                StringBuilder outputData = new StringBuilder(102400);
                int iTemp = 0;
                //主表入参  个人编号、姓名、卡号、地区编号、住院流水号、开方医生
                StringBuilder inputParam = new StringBuilder();
                inputParam.Append(grbh + "|");
                inputParam.Append(xm + "|");
                inputParam.Append(kh + "|");
                inputParam.Append(dqbh + "|");
                inputParam.Append(ybjzlsh_sndy + "|");
                inputParam.Append(ysxm + "|$");
                #region 分段上传 每次50条
                foreach (string inputData3 in li_inputData)
                {
                    if (iTemp < 50)
                    {
                        inputParam.Append(inputData3.ToString());
                        iTemp++;
                    }
                    else
                    {
                        StringBuilder inputData1 = new StringBuilder();
                        StringBuilder outData = new StringBuilder(102400);
                        WriteLog(sysdate + "  " + jzlsh + " 进入住院费用明细上传(分段)...");

                        inputData1.Append(YWBH + "^");
                        inputData1.Append(YLGHBH + "^");
                        inputData1.Append(CZYBH + "^");
                        inputData1.Append(YWZQH + "^");
                        inputData1.Append(JYLSH + "^");
                        inputData1.Append(ZXBM + "^");
                        inputData1.Append(inputParam.ToString().TrimEnd('$') + "^");
                        inputData1.Append(LJBZ + "^");

                        WriteLog(sysdate + "  进入住院费用明细上传(省内异地)(分段)|入参|" + inputData1.ToString());
                        int i = BUSINESS_HANDLE(inputData1, outputData);

                        if (i != 0)
                        {
                            WriteLog(sysdate + "  " + jzlsh + " 进入住院费用明细上传(分段)失败|" + outputData.ToString());
                            return new object[] { 0, 0, "住院收费登记失败|" + outputData.ToString().Split('^')[2] };
                        }
                        WriteLog(sysdate + "  住院费用明细上传(省内异地)(分段)|出参|" + outData.ToString().Replace("&quot;", ""));

                        string[] zysfdjfhs = outputData.ToString().Split('^')[2].Replace("&quot;","").TrimEnd(';').Split(';');
                        List<string> lizysfdjfh = new List<string>();

                        for (int j = 0; j < zysfdjfhs.Length; j++)
                        {
                            /*1	住院流水号	VARCHAR2(18)		
                            2	处方号	VARCHAR2(20)		
                            3	项目编号	VARCHAR2(20)		
                            4	项目名称	VARCHAR2(100)		
                            5	项目等级	VARCHAR2(50)		二级代码
                            6	收费类别	VARCHAR2(50)		二级代码
                            7	单价	VARCHAR2(12)		
                            8	数量	VARCHAR2(12)		
                            9	金额	VARCHAR2(12)		
                             */
                            string[] zysfdjfh = zysfdjfhs[j].Split('|');
                            outParams_fymx op = new outParams_fymx();
                            op.Ybjzlsh = zysfdjfh[0];
                            op.Zxsequ = zysfdjfh[1];
                            op.Ybxmbm = zysfdjfh[2];
                            op.Ybxmmc = zysfdjfh[3];
                            op.Sfxmdj = zysfdjfh[4];
                            op.Sfxmlb = zysfdjfh[5];
                            op.Dj = zysfdjfh[6];
                            op.Sl = zysfdjfh[7];
                            op.Je = zysfdjfh[8];

                            strSql = string.Format(@"insert into ybcfmxscfhdr(jzlsh,jylsh,ybjzlsh,ybcfh,yybxmbh,ybxmmc,sfxmdj,sflb,dj,sl,
                                                    je,sysdate) 
                                                    values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}',
                                                    '{10}','{11}')",
                                                    jzlsh, JYLSH, op.Ybjzlsh, op.Zxsequ, op.Ybxmbm, op.Ybxmmc, op.Sfxmdj, op.Sfxmlb, op.Dj, op.Sl, op.Je,
                                                    sysdate);
                            liSQL.Add(strSql);
                        }

                        iTemp = 1;
                        inputParam.Remove(0, inputParam.Length);
                        inputParam.Append(grbh + "|");
                        inputParam.Append(xm + "|");
                        inputParam.Append(kh + "|");
                        inputParam.Append(dqbh + "|");
                        inputParam.Append(ybjzlsh_sndy + "|");
                        inputParam.Append(ysxm + "|$");
                        inputParam.Append(inputData3.ToString());
                    }
                }
                #endregion
                #region 明细不足50条时，一次性上传
                if (iTemp > 0)
                {
                    StringBuilder outData = new StringBuilder(102400);
                    StringBuilder retMsg = new StringBuilder(102400);
                    WriteLog(sysdate + "  " + jzlsh + " 进入住院费用明细上传(补传、一次性上传)...");
                    StringBuilder inputData1 = new StringBuilder();

                    inputData1.Append(YWBH + "^");
                    inputData1.Append(YLGHBH + "^");
                    inputData1.Append(CZYBH + "^");
                    inputData1.Append(YWZQH + "^");
                    inputData1.Append(JYLSH + "^");
                    inputData1.Append(ZXBM + "^");
                    inputData1.Append(inputParam.ToString().TrimEnd('$') + "^");
                    inputData1.Append(LJBZ + "^");

                    WriteLog(sysdate + " 进入住院费用明细上传(补传、一次性上传)|入参|" + inputData1.ToString());
                    int i = BUSINESS_HANDLE(inputData1, outputData);
                    if (i != 0)
                    {
                        WriteLog(sysdate + "  " + jzlsh + " 进入住院费用明细上传(补传、一次性上传)失败|" + outputData.ToString());
                        return new object[] { 0, 0, "住院收费登记失败|" + outputData.ToString().Split('^')[2] };
                    }
                    WriteLog(sysdate + "  住院费用明细上传(省内异地)(分段)|出参|" + outData.ToString().Replace("&quot;", ""));
                    string[] zysfdjfhs = outputData.ToString().Split('^')[2].Replace("&quot;", "").TrimEnd(';').Split(';');
                    List<string> lizysfdjfh = new List<string>();

                    for (int j = 0; j < zysfdjfhs.Length; j++)
                    {
                        /*1	住院流水号	VARCHAR2(18)		
                        2	处方号	VARCHAR2(20)		
                        3	项目编号	VARCHAR2(20)		
                        4	项目名称	VARCHAR2(100)		
                        5	项目等级	VARCHAR2(50)		二级代码
                        6	收费类别	VARCHAR2(50)		二级代码
                        7	单价	VARCHAR2(12)		
                        8	数量	VARCHAR2(12)		
                        9	金额	VARCHAR2(12)		
                         */
                        string[] zysfdjfh = zysfdjfhs[j].Split('|');
                        outParams_fymx op = new outParams_fymx();
                        op.Ybjzlsh = zysfdjfh[0];
                        op.Zxsequ = zysfdjfh[1];
                        op.Ybxmbm = zysfdjfh[2];
                        op.Ybxmmc = zysfdjfh[3];
                        op.Sfxmdj = zysfdjfh[4];
                        op.Sfxmlb = zysfdjfh[5];
                        op.Dj = zysfdjfh[6];
                        op.Sl = zysfdjfh[7];
                        op.Je = zysfdjfh[8];

                        strSql = string.Format(@"insert into ybcfmxscfhdr(jzlsh,jylsh,ybjzlsh,ybcfh,yybxmbh,ybxmmc,sfxmdj,sflb,dj,sl,
                                                    je,sysdate) 
                                                    values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}',
                                                    '{10}','{11}')",
                                                jzlsh, JYLSH, op.Ybjzlsh, op.Zxsequ, op.Ybxmbm, op.Ybxmmc, op.Sfxmdj, op.Sfxmlb, op.Dj, op.Sl, op.Je,
                                                sysdate);
                        liSQL.Add(strSql);
                    }
                }
                #endregion
                #endregion

                strSql = string.Format(@"update zy03d set z3ybup = '{0}' where z3ybup is null and LEFT(z3kind,1)=2 and z3zyno = '{1}' ", JYLSH, jzlsh);
                if (!string.IsNullOrEmpty(ztjssj))
                    strSql += string.Format(@"and Convert(datetime,z3date)<'{0}' ", ztjssj1);
                liSQL.Add(strSql);

                object[] obj = liSQL.ToArray();
                obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);

                if (obj[1].ToString() == "1")
                {
                    WriteLog(sysdate + "    住院处方明细上传(省内异地)成功|本地数据操作成功|" + outputData.ToString());
                    return new object[] { 0, 1, JYLSH };
                }
                else
                {
                    WriteLog(sysdate + "    院处明细上传(省内异地)成功|本地数据操作失败|" + obj[2].ToString());
                    object[] objFYMXCX = { ybjzlsh, JYLSH, jbr, grbh, xm, kh, dqbh, ybjzlsh_sndy, DQJBBZ };
                    NYBZYCFMXSCCX(objFYMXCX);
                    return new object[] { 0, 0, "上传住院处方明细失败|" + obj[2].ToString() };
                }
            }
            catch (Exception error)
            {
                WriteLog(sysdate + "  住院费用登记(省内异地)|接口异常|" + error.Message);
                return new object[] { 0, 2, "Error:" + error.Message };
            }
        }
        #endregion

        #region 住院费用登记撤销
        public static object[] YBZYSFDJCX(object[] objParam)
        {
            string sysdate = GetServerDateTime();
            WriteLog(sysdate + "  进入住院费用登记撤销...");
            try
            {
                string jzlsh = objParam[0].ToString(); // 就诊流水号
                CZYBH = CliUtils.fLoginUser; // 操作员工号 
                YWZQH = CliUtils.fLoginYbNo.Split('|')[0].ToString(); // 业务周期号
                string jbr = CliUtils.fUserName;   // 经办人姓名
                JYLSH = DateTime.Now.ToString("yyyyMMddHHmmss") + "-" + YLGHBH + "-" + new Random().Next(100).ToString().PadLeft(4, '0');

                //判断是否已结算
                string strSql = string.Format(@"select * from ybfyjsdr a where a.jzlsh = '{0}' and a.cxbz = 1", jzlsh);
                DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                if (ds != null && ds.Tables[0].Rows.Count > 0)
                    return new object[] { 0, 0, "已收费结算或中途结算，不能冲销费用明细" };

                //判断是否住院登记
                strSql = string.Format("select * from ybmzzydjdr a where a.jzlsh = '{0}' and jzbz='z' and a.cxbz = 1", jzlsh);
                ds.Tables.Clear();
                ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                if (ds == null || ds.Tables[0].Rows.Count == 0)
                    return new object[] { 0, 0, "无入院登记记录" };
                DataTable dt = ds.Tables[0];
                DataRow dr = dt.Rows[0];
                string grbh = dr["grbh"].ToString();  //个人编号
                string xm = dr["xm"].ToString();      //姓名
                string kh = dr["kh"].ToString();      //卡号
                string ybjzlsh = dr["ybjzlsh"].ToString();  //医保就诊流水号
                string dqbh = dr["tcqh"].ToString();        //地区编号
                string ybjzlsh_sndy = dr["ybjzlsh_snyd"].ToString(); //医保就诊流水号
                DQJBBZ = dr["dqjbbz"].ToString();   

                StringBuilder inputParam = new StringBuilder();
                if (DQJBBZ.Equals("1"))
                {
                    //1、就诊流水号 | 2、被撤销交易流水号（如果只撤销一部分，则此值不为空） | 3、经办人
                    YWBH = "2320";
                    inputParam.Append(ybjzlsh + "|");
                    inputParam.Append("|");
                    inputParam.Append(jbr + "|");
                }
                else
                {
                    // // 0个人编号 | 1姓名 | 2卡号 | 3地区编号 | 4住院流水号
                    YWBH = "7321";
                    inputParam.Append(grbh + "|");
                    inputParam.Append(xm + "|");
                    inputParam.Append(kh + "|");
                    inputParam.Append(dqbh + "|");
                    inputParam.Append(ybjzlsh_sndy + "|");
                }

                StringBuilder inputData = new StringBuilder();
                inputData.Append(YWBH + "^");
                inputData.Append(YLGHBH + "^");
                inputData.Append(CZYBH + "^");
                inputData.Append(YWZQH + "^");
                inputData.Append(JYLSH + "^");
                inputData.Append(ZXBM + "^");
                inputData.Append(inputParam.ToString() + "^");
                inputData.Append(LJBZ + "^");
                WriteLog(sysdate + "  住院费用登记撤销|入参|" + inputData.ToString());

                StringBuilder outputData = new StringBuilder(1024);
                int i = BUSINESS_HANDLE(inputData, outputData);
                if (i == 0)
                {
                    List<string> liSql = new List<string>();
                    WriteLog(sysdate + "  住院费用登记撤销|出参|" + outputData.ToString());
                    strSql = string.Format(@"insert into ybcfmxscindr(jzlsh,jylsh,xm,kh,ybjzlsh,cfrq,yysfxmbm,yysfxmmc,sfxmzxbm,sfxmzxmc,
                                            dj,sl,je,jbr,sflb,sfxmdj,cxbz,sysdate) 
                                            select jzlsh,jylsh,xm,kh,ybjzlsh,cfrq,yysfxmbm,yysfxmmc,sfxmzxbm,sfxmzxmc,
                                            dj,sl,je,jbr,sflb,sfxmdj,0,'{1}' from ybcfmxscindr 
                                            where jzlsh = '{0}' and isnull(ybdjh,'')='' and cxbz = 1", jzlsh, sysdate);
                    liSql.Add(strSql);
                    strSql = string.Format("update ybcfmxscindr set cxbz = 2 where jzlsh = '{0}' and isnull(ybdjh,'')='' and cxbz = 1", jzlsh);
                    liSql.Add(strSql);
                    strSql = string.Format(@"insert into ybcfmxscfhdr(jzlsh,jylsh,je,zlje,zfje,sfxmdj,zfbz,xzsybz, bz,ybjzlsh,yybxmbh,ybxmmc,sflb,ybcfh,cxbz,sysdate) 
                                            select jzlsh,jylsh,je,zlje,zfje,sfxmdj,zfbz,xzsybz, bz,ybjzlsh,yybxmbh,ybxmmc,sflb,ybcfh,0,'{1}' from ybcfmxscfhdr 
                                            where jzlsh = '{0}' and isnull(ybdjh,'')='' and cxbz = 1", jzlsh, sysdate);
                    liSql.Add(strSql);
                    strSql = string.Format("update ybcfmxscfhdr set cxbz = 2 where jzlsh = '{0}' and isnull(ybdjh,'')='' and cxbz = 1", jzlsh);
                    liSql.Add(strSql);

                    strSql = string.Format("update zy03d set z3ybup = null where z3ybup is not null and isnull(z3fphx,'')='' and z3zyno = '{0}'", jzlsh);
                    liSql.Add(strSql);

                    object[] obj = liSql.ToArray();
                    obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);
                    if (obj[1].ToString().Equals("1"))
                    {
                        WriteLog(sysdate + "  住院费用登记撤销成功|本地数据操作成功|" + outputData.ToString());
                        return new object[] { 0, 1, "住院费用登记撤销成功" };
                    }
                    else
                    {
                        WriteLog(sysdate + "  住院费用登记撤销成功|本地数据操作失败|" + obj[2].ToString());
                        return new object[] { 0, 0, "住院费用登记撤销失败" };
                    }
                }
                else
                {
                    WriteLog(sysdate + "  住院费用登记撤销失败|" + outputData.ToString());
                    return new object[] { 0, 0, outputData.ToString() };
                }
            }
            catch (Exception ex)
            {
                WriteLog(sysdate + "  住院费用登记撤销|接口异常" + ex.Message);
                return new object[] { 0, 0, "接口异常" + ex.Message };
            }
        }
        #endregion

        #region 住院收费预结算
        public static object[] YBZYSFYJS(object[] objParam)
        {
            string jzlsh = objParam[0].ToString();      //就诊流水号
            //判断是否医保登记
            string strSql = string.Format("select * from ybmzzydjdr a where a.jzlsh = '{0}' and jzbz='z' and a.cxbz = 1", jzlsh);
            DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            if (ds == null || ds.Tables[0].Rows.Count == 0)
                return new object[] { 0, 0, "无医保住院登记信息" };
            string grbh = ds.Tables[0].Rows[0]["grbh"].ToString();
            DQJBBZ = ds.Tables[0].Rows[0]["DQJBBZ"].ToString();

            if (DQJBBZ.Equals("1"))
                return YBZYFYYJS_SBJ(objParam);
            else
                return YBZYFYYJS_SNYD(objParam);
        }
        #endregion

        #region 住院收费预结算_市本级
        public static object[] YBZYFYYJS_SBJ(object[] objParam)
        {
            string sysdate = GetServerDateTime();
            WriteLog(sysdate + "  进入住院收费预结算(市本级)...");
            try
            {
                string jzlsh = objParam[0].ToString();      // 就诊流水号
                string cyyy = objParam[1].ToString();       //出院原因
                string zhsybz = objParam[2].ToString();     //账户使用标志 （0或1）
                string ztjsbz = objParam[3].ToString();     //中途结算标志
                string jsrqsj = objParam[4].ToString();     //结算日期时间
                string cyrqsj = objParam[5].ToString();     //出院日期时间
                string sfje = objParam[6].ToString();       //收费金额
                string grbh = "";
                string djh = "0000";
                if (string.IsNullOrEmpty(jzlsh))
                    return new object[] { 0, 0, "就诊流水号不能为空" };
                if (string.IsNullOrEmpty(cyyy))
                    return new object[] { 0, 0, "出院原因不能为空" };

                jsrqsj = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                WriteLog(sysdate + "  " + jzlsh + " 进入住院收费预结算...");

                CZYBH = CliUtils.fLoginUser;   // 操作员工号
                YWZQH = CliUtils.fLoginYbNo.Split('|')[0].ToString();   // 业务周期号
                string jbr = CliUtils.fUserName;     // 经办人姓名
                 string cyrq="";
                string jsrq = Convert.ToDateTime(jsrqsj).ToString("yyyyMMddHHmmss");
                string dqrq = Convert.ToDateTime(sysdate).ToString("yyyyMMddHHmmss");  // 当前日期
                if(ztjsbz.Equals("1"))
                    cyrq = "";  //出院日期
                else
                    cyrq = Convert.ToDateTime(cyrqsj).ToString("yyyyMMddHHmmss");
                string sslxdm ="0";    //手术类型代码  

                JYLSH = dqrq + "-" + YLGHBH + "-" + new Random().Next(100).ToString().PadLeft(4, '0');

                #region 中途结算，先撤销后结算
                if (ztjsbz.Equals("1"))
                {
                    //医保费用撤销
                    object[] objParam1 = { jzlsh, jsrqsj };
                    YBZYSFDJCX(objParam1);
                    //处方重新上传
                    YBZYCFMXSC_SBJ(objParam1);
                }
                #endregion

                #region 是否未办理医保登记
                string strSql = string.Format("select * from ybmzzydjdr where jzlsh='{0}' and cxbz=1", jzlsh);
                DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                if (ds.Tables[0].Rows.Count == 0)
                    return new object[] { 0, 0, "该患者未办理医保登记" };

                DataRow dr = ds.Tables[0].Rows[0];
                //sslxdm = dr["sslxdm"].ToString().Trim();
                string ybjzlsh = dr["ybjzlsh"].ToString();
                string yllb = dr["yllb"].ToString();
                string bzbm = dr["bzbm"].ToString();
                string bzmc = dr["bzmc"].ToString();
                grbh = dr["grbh"].ToString();
                string kh = dr["kh"].ToString();
                string xm = dr["xm"].ToString();
                #endregion

                #region 是否已经医保结算
                strSql = string.Format(@"select * from ybfyjsdr where jzlsh='{0}' and ztjsbz=0 and cxbz=1", jzlsh);
                ds.Tables.Clear();
                ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                if (ds.Tables[0].Rows.Count > 0)
                    return new object[] { 0, 0, "该患者已办理医保结算" };
                #endregion

                #region 获取账户余额
                strSql = string.Format("select GRZHYE from ybickxx where grbh='{0}'", grbh);
                ds.Tables.Clear();
                ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                if (ds.Tables[0].Rows.Count == 0)
                    return new object[] { 0, 0, "获取医保信息出错" };
                string zhye = ds.Tables[0].Rows[0]["GRZHYE"].ToString();
                #endregion

                #region 入参
                /*
                 * 1		住院流水号(门诊流水号)	VARCHAR2(18)	NOT NULL	同登记时的住院流水号（门诊流水号）
                    2		单据号	VARCHAR2(18)		医院结算的单据号不能为空
                    3		医疗类别	VARCHAR2(3)		代码
                    4		结算日期	VARCHAR2(14)	NOT NULL	YYYYMMDDHH24MISS，医院上传数据不能为空
                    5		出院日期	VARCHAR2(14)		NULL
                    6		出院原因	VARCHAR2(3)		NULL
                    7		出院诊断疾病编码	VARCHAR2(20)		NULL
                    8		出院诊断疾病名称	VARCHAR2(50)		NULL
                    9		账户使用标志	VARCHAR2(3)	NOT NULL	代码
                    10		中途结算标志	VARCHAR2(3)		代码
                    11		经办人	VARCHAR2(20)	NOT NULL	医疗机构操作员姓名
                    12		手术类型	VARCHAR2(3)		住院医疗类别时不允许为空
                    13		胎儿数	VARCHAR2(3)		生育刷卡直补传入
                 */
                StringBuilder inputParam = new StringBuilder();
                inputParam.Append(ybjzlsh + "|");
                inputParam.Append("0000|");
                inputParam.Append(yllb + "|");
                inputParam.Append(jsrq + "|");
                inputParam.Append(cyrq + "|");
                inputParam.Append("0" + "|");   //出院原因
                inputParam.Append(bzbm + "|");
                inputParam.Append(bzmc + "|");
                inputParam.Append(zhsybz + "|");
                inputParam.Append(ztjsbz + "|");
                inputParam.Append(jbr + "|");
                inputParam.Append(sslxdm + "|");    //手术类型  
                inputParam.Append("|"); //胎儿数

                StringBuilder inputData = new StringBuilder();
                YWBH = "2420";
                inputData.Append(YWBH + "^");
                inputData.Append(YLGHBH + "^");
                inputData.Append(CZYBH + "^");
                inputData.Append(YWZQH + "^");
                inputData.Append(JYLSH + "^");
                inputData.Append(ZXBM + "^");
                inputData.Append(inputParam.ToString() + "^");
                inputData.Append(LJBZ + "^");
                WriteLog(sysdate + "  住院收费预结算(市本级)|入参|" + inputData.ToString());

                #endregion

                #region  预结算
                StringBuilder outputData = new StringBuilder(102400);
                int i = BUSINESS_HANDLE(inputData, outputData);
                List<string> liSQL = new List<string>();
                if (i == 0)
                {
                    string[] sfjsfh = outputData.ToString().Split('^')[2].Split('|');
                    #region 出参
                    outParams_js js = new outParams_js();
                    /*
                     * 1		医疗费总费用	VARCHAR2(10)		2位小数
                        2		自费费用	VARCHAR2(10)		2位小数
                        3		本次帐户支付	VARCHAR2(10)		2位小数
                        4		统筹支出	VARCHAR2(10)		2位小数
                        5		大病支出	VARCHAR2(10)		2位小数
                        6		公务员补助支付金额	VARCHAR2(10)		2位小数, 如果参保人参加了单位补充医疗保险，此处输出单位补充医疗保险支付金额
                        7		企业补充支付金额	VARCHAR2(10)		2位小数，如果是居民门诊统筹结算，则为本次门诊统筹定额打包费用。
                        8		乙类自理费用	VARCHAR2(10)		2位小数
                        9		特检自理费用	VARCHAR2(10)		2位小数
                        10		特治自理费用	VARCHAR2(10)		2位小数
                        11		符合基本医疗费用	VARCHAR2(10)		2位小数
                        12		起付标准自付	VARCHAR2(10)		2位小数
                        13		进入统筹费用	VARCHAR2(10)		2位小数
                        14		统筹分段自付	VARCHAR2(10)		2位小数
                        15		进入大病费用	VARCHAR2(10)		2位小数
                        16		大病分段自付	VARCHAR2(10)		2位小数
                        17		转诊先自付	VARCHAR2(10)		2位小数
                        18		个人现金支付	VARCHAR2(10)		2位小数
                        19		药品费用	VARCHAR2(10)		2位小数
                        20		诊疗项目费用	VARCHAR2(10)		2位小数
                        21		安装器官费用	VARCHAR2(10)		2位小数
                        22		商业补充保险支付费用	VARCHAR2(10)		2位小数，城居是民政三次救助
                        23		专项救助费用	VARCHAR2(10)		2位小数，城居是民政救助
                        24		单位分担金额	VARCHAR2(10)		2位小数
                        25		门诊诊查费	VARCHAR2(10)		2位小数
                        26		军转干部补助支付	VARCHAR2(10)		2位小数
                        27		基本医疗账户支付	NUMBER(16)		2位小数
                        28		公费医疗账户支付	NUMBER(16)		2位小数
                        29		家庭门诊统筹支付	NUMBER(16)		2位小数
                        30		大病救助支付	NUMBER(16)		2位小数
                        31		生育基金支付	NUMBER(16)		2 位小数
                     */

                    js.Ylfze = sfjsfh[0];         //医疗费总费用
                    js.Zffy = sfjsfh[1];          //自费费用
                    js.Zhzf = sfjsfh[2];          //本次帐户支付
                    js.Tcjjzf = sfjsfh[3];        //统筹支出
                    js.Dejjzf = sfjsfh[4];        //大病支出
                    js.Gwybzjjzf = sfjsfh[5];     //公务员补助支付金额
                    js.Qybcylbxjjzf = sfjsfh[6];  //企业补充支付金额
                    js.Ylzlfy = sfjsfh[7];        //乙类自理费用
                    js.Tjzlfy = sfjsfh[8];        //特检自理费用
                    js.Tzzlfy = sfjsfh[9];        //特治自理费用
                    js.Fhjjylfy = sfjsfh[10];     //符合基本医疗费用
                    js.Qfbzfy = sfjsfh[11];       //起付标准自付
                    js.Jrtcfy = sfjsfh[12];       //进入统筹费用
                    js.Tcfdzffy = sfjsfh[13];     //统筹分段自付
                    js.Jrdebxfy = sfjsfh[14];     //进入大病费用
                    js.Defdzffy = sfjsfh[15];     //大病分段自付
                    js.Zzzyzffy = sfjsfh[16];     //转诊先自付
                    js.Xjzf = sfjsfh[17];         //个人现金支付
                    js.Ypfy = sfjsfh[18];         //药品费用
                    js.Zlxmfy = sfjsfh[19];       //诊疗项目费用
                    js.Rgqgzffy = sfjsfh[20];     //安装器官费用
                    js.Sybxfy = sfjsfh[21];   //商业补充保险支付费用
                    js.Mzjzfy = sfjsfh[22];       //专项救助费用
                    js.Dwfdfy = sfjsfh[23];       //单位分担金额
                    js.Mzzcf = sfjsfh[24];        //门诊诊查费
                    js.Jzgbbzzf = sfjsfh[25];     //军转干部补助支付
                    js.Jbylzhzf = sfjsfh[26];     //基本医疗账户支付
                    js.Gfylzhzf = sfjsfh[27];     //公费医疗账户支付
                    js.Jtmztczf = sfjsfh[28];     //家庭门诊统筹支付
                    js.Dbjzfy = sfjsfh[29];     //大病救助支付
                    js.Yyfdfy = "0.00";
                    js.Cxjfy = "0.00";
                    js.Ylzlfy = "0.00";
                    js.Bcjsqzhye = zhye;
                    js.Jslsh = "0000";
                    

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
                    //总报销金额=医疗总费用-本次现金支付-本次大病支出
                    js.Zbxje = (Convert.ToDecimal(js.Ylfze) - Convert.ToDecimal(js.Xjzf) - Convert.ToDecimal(js.Dejjzf)).ToString();

                    //其他医保支付=医疗总费用-本次现金支付-本次大病支出-本次统筹支付-本次账户支付
                    js.Qtybzf = (Convert.ToDecimal(js.Ylfze) - Convert.ToDecimal(js.Xjzf) - Convert.ToDecimal(js.Dejjzf) - Convert.ToDecimal(js.Tcjjzf) - Convert.ToDecimal(js.Zhzf)).ToString();
                    //现金支付=本次现金支付+本次大病支出
                    js.Ybxjzf = (Convert.ToDecimal(js.Xjzf) + Convert.ToDecimal(js.Dejjzf)).ToString();


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
                                    js.Jmgrzfecbcje + "|" + js.Tjje + "|" + js.Syjjzf + "|" + js.Qtybzf+"|";
                    #endregion

                    WriteLog(sysdate + "  住院收费预结算(市本级)｜整合出参|" + strValue);

                    strSql = string.Format(@"delete from ybfyyjsdr where jzlsh='{0}'", jzlsh);
                    liSQL.Add(strSql);

                    strSql = string.Format(@"insert into ybfyyjsdr(jzlsh, jylsh ,ylfze ,xjzf ,zhzf ,tcjjzf, dejjzf,gwybzjjzf,qybcylbxjjzf,ylzlfy,
                                            tjzlfy,tzzlfy,fhjbylfy,qfbzfy,jrtcfy,ctcfdxfy,jrdebxfy,defdzffy,zzzyzffy,zffy,
                                            ypfy,zlxmfy,rgqgzffy,sybcbxzffy,mzjzfy,dwfdfy,mzzcf,yllb,xm,grbh,
                                            kh,cfmxjylsh,ywzqh,jslsh,jsrq,djh,ybjzlsh,jzgbbzzf,jbylzhzf,gfylzhzf,
                                            jtmztczf,dbjzzf,sysdate,jbr,ztjsbz,zbxje,bcjsqzhye,cyyy,cyrq,qtybfy,zhxjzffy)
                                            values('{0}','{1}','{2}','{3}','{4}','{5}' ,'{6}','{7}','{8}' ,'{9}' ,
                                            '{10}','{11}' ,'{12}','{13}' ,'{14}','{15}','{16}' ,'{17}' ,'{18}','{19}',
                                            '{20}' ,'{21}','{22}','{23}','{24}','{25}','{26}','{27}' ,'{28}','{29}',
                                            '{30}','{31}','{32}','{33}','{34}','{35}','{36}','{37}','{38}','{39}',
                                            '{40}','{41}','{42}','{43}','{44}','{45}','{46}','{47}','{48}','{49}','{50}')",
                                            jzlsh, JYLSH, js.Ylfze, js.Xjzf, js.Zhzf, js.Tcjjzf, js.Dejjzf, js.Gwybzjjzf, js.Qybcylbxjjzf, js.Ylzlfy,
                                            js.Tjzlfy, js.Tzzlfy, js.Fhjjylfy, js.Qfbzfy, js.Jrtcfy, js.Tcfdzffy, js.Jrdebxfy, js.Defdzffy, js.Zzzyzffy, js.Zffy,
                                            js.Ypfy, js.Zlxmfy, js.Rgqgzffy, js.Sybxfy, js.Mzjzfy, js.Dwfdfy, js.Mzzcf, yllb, xm, grbh,
                                            kh, "", YWZQH, djh, jsrq, djh, ybjzlsh, js.Jzgbbzzf, js.Jbylzhzf, js.Gfylzhzf,
                                            js.Jtmztczf, js.Dbjzfy, sysdate, jbr, ztjsbz, js.Zbxje, js.Bcjsqzhye, cyyy, cyrq,js.Qtybzf,js.Ybxjzf);

                    liSQL.Add(strSql);
                     object[] obj = liSQL.ToArray();
                    obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);
                    if (obj[1].ToString() == "1")
                    {
                        WriteLog(sysdate + "  住院收费预结算(市本级)成功|本地数据操作成功|" + outputData.ToString());
                        return new object[] { 0, 1, strValue };
                    }
                    else
                    {
                        WriteLog(sysdate + "  住院收费预结算(市本级)成功|本地数据操作失败|" + obj[2].ToString());
                        return new object[] { 0, 0, obj[2].ToString() };
                    }
                }
                else
                {
                    WriteLog(sysdate + "  住院收费预结算(市本级)失败|" + outputData.ToString());
                    return new object[] { 0, 0, outputData.ToString() };
                }
                #endregion
            }
            catch (Exception error)
            {
                WriteLog(sysdate + "  住院收费预结算(市本级)|系统异常|" + error.Message);
                return new object[] { 0, 2, error.Message };
            }
        }
        #endregion

        #region 住院收费预结算_省内异地
        public static object[] YBZYFYYJS_SNYD(object[] objParam)
        {
            string sysdate = GetServerDateTime();
            WriteLog(sysdate + "  进入住院收费预结算(省内异地)...");
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
                string yllb = "";

                if (string.IsNullOrEmpty(jzlsh))
                    return new object[] { 0, 0, "就诊流水号不能为空" };
                jsrqsj = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                CZYBH = CliUtils.fLoginUser;   // 操作员工号
                YWZQH = CliUtils.fLoginYbNo.Split('|')[0].ToString();   // 业务周期号
                string jbr = CliUtils.fUserName;     // 经办人姓名
                string cyrq = Convert.ToDateTime(cyrqsj).ToString("yyyyMMddHHmmss");
                string jsrq = Convert.ToDateTime(jsrqsj).ToString("yyyyMMddHHmmss");
                string dqrq = Convert.ToDateTime(sysdate).ToString("yyyyMMddHHmmss");  // 当前日期
                string strSql;
                DataSet ds = null;

                JYLSH = dqrq + "-" + YLGHBH + "-" + new Random().Next(100).ToString().PadLeft(4, '0');

                #region 中途结算，先撤销后结算
                if (ztjsbz.Equals("1"))
                {
                    //医保费用撤销
                    object[] objParam1 = { jzlsh, jsrqsj };
                    YBZYSFDJCX(objParam1);
                    //处方重新上传
                    YBZYCFMXSC_SNYD(objParam1);
                }
                #endregion

                #region 判断是否医保登记 
                strSql = string.Format("select * from ybmzzydjdr a where a.jzlsh = '{0}'  and a.jzbz='z' and a.cxbz = 1  and a.dqjbbz=2", jzlsh);
                ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);

                if (ds == null || ds.Tables[0].Rows.Count == 0)
                    return new object[] { 0, 0, "就诊流水号" + jzlsh + "无挂号或入院登记记录" };

                DataRow dr = ds.Tables[0].Rows[0];
                string grbh = dr["grbh"].ToString().Trim();
                string xm = dr["xm"].ToString().Trim();
                string kh = dr["kh"].ToString().Trim();
                string dqbh = dr["tcqh"].ToString().Trim();
                string ybjzlsh = dr["ybjzlsh"].ToString().Trim();
                string ybjzlsh_snyd = dr["ybjzlsh_snyd"].ToString().Trim();
                string bzbm = dr["bzbm"].ToString().Trim();
                string bzmc = dr["bzmc"].ToString().Trim();
                yllb = dr["yllb"].ToString();
                #endregion

                #region 获取账户余额
                strSql = string.Format("select GRZHYE from ybickxx where grbh='{0}'", grbh);
                ds.Tables.Clear();
                ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                if (ds.Tables[0].Rows.Count == 0)
                    return new object[] { 0, 0, "获取医保信息出错" };
                string zhye = ds.Tables[0].Rows[0]["GRZHYE"].ToString();
                #endregion

                #region 入参
                /*
                 1	个人编号	VARCHAR2(10)	NOT NULL	
                2	姓名	VARCHAR2(50)	NOT NULL	跨省异地长度修改
                3	卡号	VARCHAR2(18)	NOT NULL	
                4	地区编号	VARCHAR2(6)	NOT NULL	
                5	住院号	VARCHAR2(20)	NOT NULL	跨省异地长度修改
                6	出院原因	VARCHAR2(3)	NOT NULL	代码
                7	出院日期	VARCHAR2(14)	NOT NULL	YYYYMMDDHH24MISS
                8	是否打印票据	VARCHAR2(8)		值为1或0，做为预留，暂不使用
                9	出院诊断疾病编码	VARCHAR2(100)		必须为中心编码
                10	出院诊断疾病名称			
                11	医生编号	VARCHAR2(50)		
                12	就诊医生	VARCHAR2(50)		
                13	出院描述	VARCHAR2(1000)		
                14	离院方式	VARCHAR2(3)	NOT NULL	二级代码
                15	中途结算标志	VARCHAR2(3)	NOT NULL	二级代码 0：否 1：是
                16	账户使用标志	VARCHAR2(3)	NOT NULL	二级代码 0：否 1：是
                17	TAC	VARCHAR2(3)		
                18	经办人	VARCHAR2(50)	NOT NULL	
                19	次要出院疾病诊断编码1	VARCHAR2(20)		
                20	次要出院疾病诊断编码2	VARCHAR2(20)		
                21	次要出院疾病诊断编码3	VARCHAR2(20)		
                22	次要出院疾病诊断编码4	VARCHAR2(20)		
                23	次要出院疾病诊断编码5	VARCHAR2(20)		
                24	次要出院疾病诊断编码6	VARCHAR2(20)		
                25	次要出院疾病诊断编码7	VARCHAR2(20)		
                26	次要出院疾病诊断编码8	VARCHAR2(20)		
                27	次要出院疾病诊断编码9	VARCHAR2(20)		
                28	次要出院疾病诊断编码10	VARCHAR2(20)		
                29	输入附加信息1	VARCHAR2(500)		
                30	输入附加信息2	VARCHAR2(500)		
                31	输入附加信息3	VARCHAR2(500)		
                32	住院流水号	VARCHAR2(20)	NOT NULL 	就医地定点机构自定义的唯一就诊序列号，一个病人在医院一次就诊的标志。需与住院登记交易上传的医院住院流水号一致
                33	单据号	VARCHAR2(20)	NOT NULL	预结算固定传“0000”。正式结算时就医地定点机构根据实际发票号码传入。如果采集不到的填“1111”
                 */
                StringBuilder inputParam = new StringBuilder();
                inputParam.Append(grbh + "|");
                inputParam.Append(xm + "|");
                inputParam.Append(kh + "|");
                inputParam.Append(dqbh + "|");
                inputParam.Append(ybjzlsh_snyd + "|");
                inputParam.Append("0|");
                inputParam.Append(cyrq + "|");
                inputParam.Append(1 + "|");
                inputParam.Append(bzbm + "|");
                inputParam.Append(bzmc + "|");
                inputParam.Append("|");
                inputParam.Append("|");
                inputParam.Append("|");
                inputParam.Append(9 + "|");
                inputParam.Append(ztjsbz + "|");    //中途结算标志 二级代码 0：否 1：是
                inputParam.Append(zhsybz + "|");
                inputParam.Append("|");
                inputParam.Append(jbr + "|");
                inputParam.Append(bzbm + "|");   //次要出院疾病诊断编码1
                inputParam.Append("|");   //次要出院疾病诊断编码2
                inputParam.Append("|");     //次要出院疾病诊断编码3
                inputParam.Append("|");     //次要出院疾病诊断编码4
                inputParam.Append("|");
                inputParam.Append("|");
                inputParam.Append("|");
                inputParam.Append("|");
                inputParam.Append("|");
                inputParam.Append("|");     //次要出院疾病诊断编码10
                inputParam.Append("|");     //输入附加信息1
                inputParam.Append("|");     //输入附加信息2
                inputParam.Append("|");     //输入附加信息3
                inputParam.Append(ybjzlsh + "|");     //住院流水号
                inputParam.Append("0000" + "|");     //单据号
                #endregion

                StringBuilder inputData = new StringBuilder();
                StringBuilder outputData = new StringBuilder(10240);

                #region 预结算前先刷卡
                YWBH = "7100";
                inputData.Append(YWBH + "^");
                inputData.Append(YLGHBH + "^");
                inputData.Append(CZYBH + "^");
                inputData.Append(YWZQH + "^");
                inputData.Append(JYLSH + "^");
                inputData.Append(ZXBM + "^");
                inputData.Append("^");
                inputData.Append(LJBZ + "^");

                int i = BUSINESS_HANDLE(inputData, outputData);
                WriteLog(sysdate + "  预结算前刷卡|" + outputData.ToString());
                #endregion

                #region 预结算
                inputData.Remove(0, inputData.Length);
                YWBH = "7410";
                inputData.Append(YWBH + "^");
                inputData.Append(YLGHBH + "^");
                inputData.Append(CZYBH + "^");
                inputData.Append(YWZQH + "^");
                inputData.Append(JYLSH + "^");
                inputData.Append(ZXBM + "^");
                inputData.Append(inputParam.ToString() + "^");
                inputData.Append(LJBZ + "^");

                WriteLog(sysdate + "  住院收费预结算(省内异地)|入参|" + inputData.ToString());

                outputData.Remove(0, outputData.Length);
                i = BUSINESS_HANDLE(inputData, outputData);
                if (i == 0)
                {
                    WriteLog(sysdate + "  住院收费预结算(省内异地)|出参|" + outputData.ToString());
                    string[] sfjsfh = outputData.ToString().Split('^')[2].Split('|');
                    #region 出参
                    outParams_js js = new outParams_js();
                    /*
                     * 1	单据号	VARCHAR2(18)		
                    2	医疗总费用	VARCHAR2(10)		2位小数
                    3	本次账户支付	VARCHAR2(10)		2位小数
                    4	本次现金支付	VARCHAR2(10)		2位小数
                    5	本次统筹支付	VARCHAR2(10)		2位小数
                    6	大病基金支付	VARCHAR2(10)		2位小数
                    7	救助金额	VARCHAR2(10)		2位小数
                    8	公务员补助支付	VARCHAR2(10)		2位小数
                    9	甲类费用	VARCHAR2(10)		2位小数
                    10	乙类费用	VARCHAR2(10)		2位小数
                    11	丙类费用	VARCHAR2(10)		2位小数
                    12	自费费用	VARCHAR2(10)		2位小数
                    13	起付标准自付	VARCHAR2(10)		2位小数
                    14	非医保自付	VARCHAR2(10)		2位小数
                    15	乙类药品自付	VARCHAR2(10)		2位小数
                    16	特检特治自付	VARCHAR2(10)		2位小数
                    17	进入统筹自付	VARCHAR2(10)		2位小数
                    18	进入大病自付	VARCHAR2(10)		2位小数
                    19	输出附加信息1	VARCHAR2(500)		
                    20	输出附加信息2	VARCHAR2(500)		
                    21	输出附加信息3	VARCHAR2(500)		
                    22	其他基金支付	VARCHAR2(16)		2位小数
                     */

                    js.Jslsh = sfjsfh[0];         //返回的单据号
                    js.Ylfze = sfjsfh[1];         //医疗总费用
                    js.Zhzf = sfjsfh[2];          //本次帐户支付
                    js.Xjzf = sfjsfh[3];          //本次现金支付
                    js.Tcjjzf = sfjsfh[4];        //本次统筹支付
                    js.Dejjzf = sfjsfh[5];        //大病基金支付

                    js.Mzjzfy = sfjsfh[6];        //救助金额
                    js.Gwybzjjzf = sfjsfh[7];     //公务员补助支付
                    js.Jlfy = sfjsfh[8];          //甲类费用
                    js.Ylfy = sfjsfh[9];          //乙类费用
                    js.Blfy = sfjsfh[10];         //丙类费用
                    js.Zffy = sfjsfh[11];         //自费费用
                    js.Qfbzfy = sfjsfh[12];       //起付标准自付
                    js.Fybzf = sfjsfh[13];        //非医保自付
                    js.Ylzlfy = sfjsfh[14];       //乙类药品自付
                    js.Tjzlfy = sfjsfh[15];       //特检特治自付

                    js.Tcfdzffy = sfjsfh[16];     //进入统筹自付(统筹分段自付)
                    js.Defdzffy = sfjsfh[17];     //进入大病自付(大病分段自付)
                    string scfjxxo = sfjsfh[18];      //输出附加信息1
                    string scfjxxt = sfjsfh[19];      //输出附加信息2
                    string scfjxxs = sfjsfh[20];      //输出附加信息3
                    js.Qtjjzf = sfjsfh[21];       //其他基金支付
                    js.Yyfdfy = "0.00";
                    js.Cxjfy = "0.00";
                    js.Ylzlfy = "0.00";
                    js.Bcjsqzhye = zhye;
                    js.Jslsh = "0000";
                    js.Ecbcje = "0.00";
                    js.Qybcylbxjjzf = "0.00";


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
                    //总报销金额=医疗总费用-本次现金支付-本次大病支出
                    js.Zbxje = (Convert.ToDecimal(js.Ylfze) - Convert.ToDecimal(js.Xjzf) - Convert.ToDecimal(js.Dejjzf)).ToString();
                    //其他医保支付=医疗总费用-本次现金支付-本次大病支出-本次统筹支付-本次账户支付
                    js.Qtybzf = (Convert.ToDecimal(js.Ylfze) - Convert.ToDecimal(js.Xjzf) - Convert.ToDecimal(js.Dejjzf) - Convert.ToDecimal(js.Tcjjzf) - Convert.ToDecimal(js.Zhzf)).ToString();
                    //现金支付=本次现金支付+本次大病支出
                    js.Ybxjzf = (Convert.ToDecimal(js.Xjzf) + Convert.ToDecimal(js.Dejjzf)).ToString();

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

                    WriteLog(sysdate + "  住院收费预结算(省内异地)｜整合出参|" + strValue);

                    List<string> liSQL = new List<string>();
                    strSql = string.Format(@"delete from ybfyyjsdr where jzlsh='{0}'", jzlsh);
                    liSQL.Add(strSql);

                    strSql = string.Format(@"insert into ybfyyjsdr(
                                             jzlsh, jylsh ,ylfze,zhzf,xjzf,tcjjzf,dejjzf, mzjzfy,gwybzjjzf,jlfy,
                                             ylfy,blfy,zffy,qfbzfy,fybzf,ylzlfy,tjzlfy,tcfdzffy,defdzffy,yllb,
                                             xm,grbh,kh,cfmxjylsh,ywzqh,djh,ybjzlsh,jslsh,ybmzlsh,jbr,
                                             zbxje,bcjsqzhye,ztjsbz,jsrq,qtybfy,zhxjzffy)
                                             values('{0}','{1}','{2}','{3}','{4}','{5}' ,'{6}','{7}','{8}' ,'{9}' ,
                                             '{10}','{11}' ,'{12}','{13}' ,'{14}','{15}','{16}' ,'{17}' ,'{18}','{19}',
                                             '{20}' ,'{21}', '{22}','{23}','{24}','{25}','{26}','{27}','{28}','{29}',
                                             '{30}' ,'{31}','{32}','{33}','{34}','{35}')",
                                              jzlsh, JYLSH, js.Ylfze, js.Zhzf, js.Xjzf, js.Tcjjzf, js.Dejjzf, js.Mzjzfy, js.Gwybzjjzf, js.Jlfy,
                                              js.Ylfy, js.Blfy, js.Zffy, js.Qfbzfy, js.Fybzf, js.Ylzlfy, js.Tjzlfy, js.Tcfdzffy, js.Defdzffy, yllb,
                                              xm, grbh, kh, JYLSH, YWZQH, djh, ybjzlsh, js.Jslsh, js.Ybmzlsh, jbr,
                                              js.Zbxje, js.Bcjsqzhye, ztjsbz, jsrq, js.Qtybzf, js.Ybxjzf);
                    liSQL.Add(strSql);

                    object[] obj = liSQL.ToArray();
                    obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);
                    if (obj[1].ToString() == "1")
                    {
                        WriteLog(sysdate + "  住院收费预结算(省内异地)成功|本地数据操作成功|" + outputData.ToString());
                        return new object[] { 0, 1, strValue };
                    }
                    else
                    {
                        WriteLog(sysdate + "  住院收费预结算(省内异地)成功|本地数据操作失败|" + obj[2].ToString());
                        return new object[] { 0, 0, obj[2].ToString() };
                    }
                }
                else
                {
                    WriteLog(sysdate + "  住院收费预结算(省内异地)失败|" + outputData.ToString());
                    return new object[] { 0, 0, outputData.ToString() };
                }
                #endregion

            }
            catch (Exception error)
            {
                WriteLog(sysdate + "  住院收费预结算(省内异地)|系统异常|" + error.Message);
                return new object[] { 0, 2, error.Message };
            }
        }
        #endregion

        #region 住院收费结算
        public static object[] YBZYSFJS(object[] objParam)
        {
            #region 入参
            string jzlsh = objParam[0].ToString();      // 就诊流水号
            string djh = objParam[1].ToString();        //单据号（发票号）
            string cyyy = objParam[2].ToString();       //出院原因
            string zhsybz = objParam[3].ToString();     //账户使用标志（0或1）
            string ztjsbz = objParam[4].ToString();     //中途结算标志
            string jsrqsj = objParam[5].ToString();     //结算日期时间
            string cyrqsj = objParam[6].ToString();     //出院日期时间
            string sfje = objParam[7].ToString();       //收费金额

            if (string.IsNullOrEmpty(jzlsh))
                return new object[] { 0, 0, "就诊流水号不能为空" };
            if (string.IsNullOrEmpty(djh))
                return new object[] { 0, 0, "单据号不能为空" };

            #endregion

            //判断是否医保登记
            string strSql = string.Format("select * from ybmzzydjdr a where a.jzlsh = '{0}' and jzbz='z' and a.cxbz = 1", jzlsh);
            DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            if (ds == null || ds.Tables[0].Rows.Count == 0)
                return new object[] { 0, 0, "无医保住院登记信息" };
            string grbh = ds.Tables[0].Rows[0]["grbh"].ToString();
            DQJBBZ = ds.Tables[0].Rows[0]["DQJBBZ"].ToString();


            if (DQJBBZ.Equals("1"))
                return YBZYFYJS_SBJ(objParam);
            else
                return YBZYFYJS_SNYD(objParam);
        }
        #endregion

        #region 住院收费结算_市本级
        public static object[] YBZYFYJS_SBJ(object[] objParam)
        {
            string sysdate = GetServerDateTime();
            WriteLog(sysdate + "  进入住院收费结算(市本级)...");
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

                if (string.IsNullOrEmpty(jzlsh))
                    return new object[] { 0, 0, "就诊流水号不能为空" };
                if (string.IsNullOrEmpty(djh))
                    return new object[] { 0, 0, "单据号不能为空" };

                CZYBH = CliUtils.fLoginUser;   // 操作员工号
                YWZQH = CliUtils.fLoginYbNo.Split('|')[0].ToString();   // 业务周期号
                string jbr = CliUtils.fUserName;     // 经办人姓名
                string cyrq = "";
                string sslxdm = "0";
                string jsrq = Convert.ToDateTime(jsrqsj).ToString("yyyyMMddHHmmss");
                string dqrq = Convert.ToDateTime(sysdate).ToString("yyyyMMddHHmmss");  // 当前日期
                if (ztjsbz.Equals("1"))
                    cyrq = "";  //出院日期
                else
                    cyrq = Convert.ToDateTime(cyrqsj).ToString("yyyyMMddHHmmss");
                //医院交易流水号
                JYLSH = dqrq + "-" + YLGHBH + "-" + new Random().Next(100).ToString().PadLeft(4, '0');

                #region 是否未办理医保登记
                string strSql = string.Format("select * from ybmzzydjdr where jzlsh='{0}' and cxbz=1", jzlsh);
                DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                if (ds.Tables[0].Rows.Count == 0)
                    return new object[] { 0, 0, "该患者未办理医保登记" };

                DataRow dr = ds.Tables[0].Rows[0];
                string ybjzlsh = dr["ybjzlsh"].ToString();
                string yllb = dr["yllb"].ToString();
                string bzbm = dr["bzbm"].ToString();
                string bzmc = dr["bzmc"].ToString();
                string grbh = dr["grbh"].ToString();
                string kh = dr["kh"].ToString();
                string xm = dr["xm"].ToString();
                #endregion

                #region 是否已经医保结算
                strSql = string.Format(@"select * from ybfyjsdr where jzlsh='{0}' and cxbz=1", jzlsh);
                ds.Tables.Clear();
                ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                if (ds.Tables[0].Rows.Count > 0)
                    return new object[] { 0, 0, "该患者已办理医保结算" };
                #endregion

                #region 获取账户余额
                strSql = string.Format("select GRZHYE from ybickxx where grbh='{0}'", grbh);
                ds.Tables.Clear();
                ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                if (ds.Tables[0].Rows.Count == 0)
                    return new object[] { 0, 0, "获取医保信息出错" };
                string zhye = ds.Tables[0].Rows[0]["GRZHYE"].ToString();
                #endregion

                #region 入参
                /*
                 1		住院流水号(门诊流水号)	VARCHAR2(18)	NOT NULL	同登记时的住院流水号（门诊流水号）
                2		单据号	VARCHAR2(18)	NOT NULL	唯一
                3		医疗类别	VARCHAR2(3)	NOT NULL	代码
                4		结算日期	VARCHAR2(14)	NOT NULL	YYYYMMDDHH24MISS，医院上传数据不能为空
                5		出院日期	VARCHAR2(14)		YYYYMMDDHH24MISS，出院结算不能为空
                6		出院原因	VARCHAR2(3)		代码
                7		出院诊断疾病编码	VARCHAR2(20)		必须为中心病种编码
                8		出院诊断疾病名称	VARCHAR2(50)		
                9		账户使用标志	VARCHAR2(3)	NOT NULL	代码
                10		中途结算标志	VARCHAR2(3)		代码
                11		经办人	VARCHAR2(20)	NOT NULL	医疗机构操作员姓名
                12		手术类型	VARCHAR2(3)		住院医疗类别时不允许为空
                 */
                StringBuilder inputParam = new StringBuilder();
                inputParam.Append(ybjzlsh + "|");
                inputParam.Append(djh + "|");
                inputParam.Append(yllb + "|");
                inputParam.Append(jsrq + "|");
                inputParam.Append(cyrq + "|");
                inputParam.Append("0|");
                inputParam.Append(bzbm + "|");
                inputParam.Append(bzmc + "|");
                inputParam.Append(zhsybz + "|");
                inputParam.Append(ztjsbz + "|");
                inputParam.Append(jbr + "|");
                inputParam.Append(sslxdm + "|");    //手术类型  

                StringBuilder inputData = new StringBuilder();
                YWBH = "2410";
                inputData.Append(YWBH + "^");
                inputData.Append(YLGHBH + "^");
                inputData.Append(CZYBH + "^");
                inputData.Append(YWZQH + "^");
                inputData.Append(JYLSH + "^");
                inputData.Append(ZXBM + "^");
                inputData.Append(inputParam.ToString() + "^");
                inputData.Append(LJBZ + "^");

                WriteLog(sysdate + "  住院收费结算(市本级)|入参|" + inputData.ToString());
                #endregion

                #region 收费结算
                StringBuilder outputData = new StringBuilder(10240);
                List<string> liSQL = new List<string>();
                int i = BUSINESS_HANDLE(inputData, outputData); ;

                if (i == 0)
                {
                    WriteLog(sysdate + "  住院收费结算(市本级)|出参|" + inputData.ToString());
                    string[] sfjsfh = outputData.ToString().Split('^')[2].Split('|');
                    outParams_js js = new outParams_js();
                    #region 出参
                    /*
                     * 1		医疗费总费用	VARCHAR2(10)		2位小数
                        2		自费费用	VARCHAR2(10)		2位小数
                        3		本次帐户支付	VARCHAR2(10)		2位小数
                        4		统筹支出	VARCHAR2(10)		2位小数
                        5		大病支出	VARCHAR2(10)		2位小数
                        6		公务员补助支付金额	VARCHAR2(10)		2位小数, 如果参保人参加了单位补充医疗保险，此处输出单位补充医疗保险支付金额
                        7		企业补充支付金额	VARCHAR2(10)		2位小数，如果是居民门诊统筹结算，则为本次门诊统筹定额打包费用。
                        8		乙类自理费用	VARCHAR2(10)		2位小数
                        9		特检自理费用	VARCHAR2(10)		2位小数
                        10		特治自理费用	VARCHAR2(10)		2位小数
                        11		符合基本医疗费用	VARCHAR2(10)		2位小数
                        12		起付标准自付	VARCHAR2(10)		2位小数
                        13		进入统筹费用	VARCHAR2(10)		2位小数
                        14		统筹分段自付	VARCHAR2(10)		2位小数
                        15		进入大病费用	VARCHAR2(10)		2位小数
                        16		大病分段自付	VARCHAR2(10)		2位小数
                        17		转诊先自付	VARCHAR2(10)		2位小数
                        18		个人现金支付	VARCHAR2(10)		2位小数
                        19		药品费用	VARCHAR2(10)		2位小数
                        20		诊疗项目费用	VARCHAR2(10)		2位小数
                        21		安装器官费用	VARCHAR2(10)		2位小数
                        22		商业补充保险支付费用	VARCHAR2(10)		2位小数，城居是民政三次救助
                        23		专项救助费用	VARCHAR2(10)		2位小数，城居是民政救助
                        24		单位分担金额	VARCHAR2(10)		2位小数
                        25		门诊诊查费	VARCHAR2(10)		2位小数
                        26		军转干部补助支付	VARCHAR2(10)		2位小数
                        27		基本医疗账户支付	NUMBER(16)		2位小数
                        28		公费医疗账户支付	NUMBER(16)		2位小数
                        29		家庭门诊统筹支付	NUMBER(16)		2位小数
                        30		大病救助支付	NUMBER(16)		2位小数
                        31		生育基金支付	NUMBER(16)		2 位小数
                     */

                    js.Ylfze = sfjsfh[0];         //医疗费总费用
                    js.Zffy = sfjsfh[1];          //自费费用
                    js.Zhzf = sfjsfh[2];          //本次帐户支付
                    js.Tcjjzf = sfjsfh[3];        //统筹支出
                    js.Dejjzf = sfjsfh[4];        //大病支出
                    js.Gwybzjjzf = sfjsfh[5];     //公务员补助支付金额
                    js.Qybcylbxjjzf = sfjsfh[6];  //企业补充支付金额
                    js.Ylzlfy = sfjsfh[7];        //乙类自理费用
                    js.Tjzlfy = sfjsfh[8];        //特检自理费用
                    js.Tzzlfy = sfjsfh[9];        //特治自理费用
                    js.Fhjjylfy = sfjsfh[10];     //符合基本医疗费用
                    js.Qfbzfy = sfjsfh[11];       //起付标准自付
                    js.Jrtcfy = sfjsfh[12];       //进入统筹费用
                    js.Tcfdzffy = sfjsfh[13];     //统筹分段自付
                    js.Jrdebxfy = sfjsfh[14];     //进入大病费用
                    js.Defdzffy = sfjsfh[15];     //大病分段自付
                    js.Zzzyzffy = sfjsfh[16];     //转诊先自付
                    js.Xjzf = sfjsfh[17];         //个人现金支付
                    js.Ypfy = sfjsfh[18];         //药品费用
                    js.Zlxmfy = sfjsfh[19];       //诊疗项目费用
                    js.Rgqgzffy = sfjsfh[20];     //安装器官费用
                    js.Sybxfy = sfjsfh[21];   //商业补充保险支付费用
                    js.Mzjzfy = sfjsfh[22];       //专项救助费用
                    js.Dwfdfy = sfjsfh[23];       //单位分担金额
                    js.Mzzcf = sfjsfh[24];        //门诊诊查费
                    js.Jzgbbzzf = sfjsfh[25];     //军转干部补助支付
                    js.Jbylzhzf = sfjsfh[26];     //基本医疗账户支付
                    js.Gfylzhzf = sfjsfh[27];     //公费医疗账户支付
                    js.Jtmztczf = sfjsfh[28];     //家庭门诊统筹支付
                    js.Dbjzfy = sfjsfh[29];     //大病救助支付
                    js.Syjjzf = sfjsfh[30];     //生育基金支付
                    js.Yyfdfy = "0.00";
                    js.Cxjfy = "0.00";
                    //js.Ylzlfy = "0.00";
                    js.Bcjsqzhye = zhye;
                    js.Jslsh = djh;

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
                    //总报销金额=医疗总费用-本次现金支付-本次大病支出
                    js.Zbxje = (Convert.ToDecimal(js.Ylfze) - Convert.ToDecimal(js.Xjzf) - Convert.ToDecimal(js.Dejjzf)).ToString();
                    //其他医保支付=医疗总费用-本次现金支付-本次大病支出-本次统筹支付-本次账户支付
                    js.Qtybzf = (Convert.ToDecimal(js.Ylfze) - Convert.ToDecimal(js.Xjzf) - Convert.ToDecimal(js.Dejjzf) - Convert.ToDecimal(js.Tcjjzf) - Convert.ToDecimal(js.Zhzf)).ToString();
                    //现金支付=本次现金支付+本次大病支出
                    js.Ybxjzf = (Convert.ToDecimal(js.Xjzf) + Convert.ToDecimal(js.Dejjzf)).ToString();
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
                    WriteLog(sysdate + "  住院收费结算(市本级)｜整合出参|" + strValue);

                    strSql = string.Format(@"insert into ybfyjsdr(jzlsh, jylsh ,ylfze ,xjzf ,zhzf ,tcjjzf, dejjzf,gwybzjjzf,qybcylbxjjzf,ylzlfy,
                                            tjzlfy,tzzlfy,fhjbylfy,qfbzfy,jrtcfy,ctcfdxfy,jrdebxfy,defdzffy,zzzyzffy,zffy,
                                            ypfy,zlxmfy,rgqgzffy,sybcbxzffy,mzjzfy,dwfdfy,mzzcf,yllb,xm,grbh,
                                            kh,cfmxjylsh,ywzqh,jslsh,jsrq,djh,ybjzlsh,jzgbbzzf,jbylzhzf,gfylzhzf,
                                            jtmztczf,dbjzzf,sysdate,jbr,ztjsbz,zbxje,bcjsqzhye,cyyy,cyrq,qtybfy,
                                            zhxjzffy,syjjzf)
                                            values('{0}','{1}','{2}','{3}','{4}','{5}' ,'{6}','{7}','{8}' ,'{9}' ,
                                            '{10}','{11}' ,'{12}','{13}' ,'{14}','{15}','{16}' ,'{17}' ,'{18}','{19}',
                                            '{20}' ,'{21}','{22}','{23}','{24}','{25}','{26}','{27}' ,'{28}','{29}',
                                            '{30}','{31}','{32}','{33}','{34}','{35}','{36}','{37}','{38}','{39}',
                                            '{40}','{41}','{42}','{43}','{44}','{45}','{46}','{47}','{48}','{49}'
                                            ,'{50}','{51}')",
                                            jzlsh, JYLSH, js.Ylfze, js.Xjzf, js.Zhzf, js.Tcjjzf, js.Dejjzf, js.Gwybzjjzf, js.Qybcylbxjjzf, js.Ylzlfy,
                                            js.Tjzlfy, js.Tzzlfy, js.Fhjjylfy, js.Qfbzfy, js.Jrtcfy, js.Tcfdzffy, js.Jrdebxfy, js.Defdzffy, js.Zzzyzffy, js.Zffy,
                                            js.Ypfy, js.Zlxmfy, js.Rgqgzffy, js.Sybxfy, js.Mzjzfy, js.Dwfdfy, js.Mzzcf, yllb, xm, grbh,
                                            kh, "", YWZQH, djh, jsrqsj, djh, ybjzlsh, js.Jzgbbzzf, js.Jbylzhzf, js.Gfylzhzf,
                                            js.Jtmztczf, js.Dbjzfy, sysdate, jbr, ztjsbz, js.Zbxje, js.Bcjsqzhye, cyyy, cyrq, js.Qtybzf,
                                            js.Ybxjzf, js.Syjjzf);

                    liSQL.Add(strSql);
                    strSql = string.Format("update ybcfmxscfhdr set ybdjh='{0}' where isnull(ybdjh,'')='' and jzlsh='{1}' and cxbz=1", djh, jzlsh);
                    liSQL.Add(strSql);
                    strSql = string.Format("update ybcfmxscindr set ybdjh='{0}' where isnull(ybdjh,'')='' and jzlsh='{1}' and cxbz=1", djh, jzlsh);
                    liSQL.Add(strSql);
                    //strSql = string.Format("update zy03d set z3fphx='{0}' where isnull(z3fphx,'')='' and z3zyno='{1}' ", djh, jzlsh);
                    //liSQL.Add(strSql);

                    object[] obj = liSQL.ToArray();
                    obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);
                    if (obj[1].ToString() == "1")
                    {
                        WriteLog(sysdate + "  住院收费结算(市本级)成功|本地数据操作成功|" + outputData.ToString());
                        return new object[] { 0, 1, strValue };
                    }
                    else
                    {
                        WriteLog(sysdate + "  住院收费结算(市本级)成功|本地数据操作失败|" + obj[2].ToString());
                        //撤销收费结算
                        object[] objJSCX = { jzlsh, djh, ybjzlsh, xm, kh, grbh, dqrq, "", "", "", 1 };
                        NYBZYFYJSCX(objJSCX);
                        return new object[] { 0, 0, "住院收费结算(市本级)成功|本地数据操作失败|" + obj[2].ToString() };
                    }
                }
                else
                {
                    WriteLog(sysdate + "  住院收费结算(市本级)失败|" + outputData.ToString());
                    return new object[] { 0, 0, outputData.ToString() };
                }
                #endregion
            }
            catch (Exception error)
            {
                WriteLog(sysdate + "  住院收费结算(市本级)|异常|" + error.Message);
                return new object[] { 0, 2, "Error:" + error.Message };
            }
        }
        #endregion

        #region 住院收费结算_省内异地
        public static object[] YBZYFYJS_SNYD(object[] objParam)
        {
            string sysdate = GetServerDateTime();
            WriteLog(sysdate + "  进入住院收费结算(省内异地)...");
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

                if (string.IsNullOrEmpty(jzlsh))
                    return new object[] { 0, 0, "就诊流水号不能为空" };
                if (string.IsNullOrEmpty(djh))
                    return new object[] { 0, 0, "单据号不能为空" };

                CZYBH = CliUtils.fLoginUser;   // 操作员工号
                YWZQH = CliUtils.fLoginYbNo.Split('|')[0].ToString();   // 业务周期号
                string jbr = CliUtils.fUserName;     // 经办人姓名

                string cyrq = Convert.ToDateTime(cyrqsj).ToString("yyyyMMddHHmmss");
                string jsrq = Convert.ToDateTime(jsrqsj).ToString("yyyyMMddHHmmss");
                string dqrq = Convert.ToDateTime(sysdate).ToString("yyyyMMddHHmmss");  // 当前日期

                //医院交易流水号
                JYLSH = dqrq + "-" + YLGHBH + "-" + new Random().Next(100).ToString().PadLeft(4, '0');

                #region 判断是否医保登记
                string strSql = string.Format("select * from ybmzzydjdr a where a.jzlsh = '{0}'  and a.jzbz='z' and a.cxbz = 1  and a.dqjbbz=2", jzlsh);
                
               DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);

                if (ds == null || ds.Tables[0].Rows.Count == 0)
                    return new object[] { 0, 0, "就诊流水号" + jzlsh + "无挂号或入院登记记录" };

                DataRow dr = ds.Tables[0].Rows[0];
                string grbh = dr["grbh"].ToString().Trim();
                string xm = dr["xm"].ToString().Trim();
                string kh = dr["kh"].ToString().Trim();
                string dqbh = dr["tcqh"].ToString().Trim();
                string ybjzlsh = dr["ybjzlsh"].ToString().Trim();
                string ybjzlsh_snyd = dr["ybjzlsh_snyd"].ToString().Trim();
                string bzbm = dr["bzbm"].ToString().Trim();
                string bzmc = dr["bzmc"].ToString().Trim();
                string yllb = dr["yllb"].ToString().Trim();
                #endregion

                #region 获取账户余额
                strSql = string.Format("select GRZHYE from ybickxx where grbh='{0}'", grbh);
                ds.Tables.Clear();
                ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                if (ds.Tables[0].Rows.Count == 0)
                    return new object[] { 0, 0, "获取医保信息出错" };
                string zhye = ds.Tables[0].Rows[0]["GRZHYE"].ToString();
                ds.Dispose();
                #endregion

                #region 入参
                /*
                 1	个人编号	VARCHAR2(10)	NOT NULL	
                2	姓名	VARCHAR2(50)	NOT NULL	跨省异地长度修改
                3	卡号	VARCHAR2(18)	NOT NULL	
                4	地区编号	VARCHAR2(6)	NOT NULL	
                5	住院号	VARCHAR2(20)	NOT NULL	跨省异地长度修改
                6	出院原因	VARCHAR2(3)	NOT NULL	代码
                7	出院日期	VARCHAR2(14)	NOT NULL	YYYYMMDDHH24MISS
                8	是否打印票据	VARCHAR2(8)		值为1或0，做为预留，暂不使用
                9	出院诊断疾病编码	VARCHAR2(100)		必须为中心编码
                10	出院诊断疾病名称			
                11	医生编号	VARCHAR2(50)		
                12	就诊医生	VARCHAR2(50)		
                13	出院描述	VARCHAR2(1000)		
                14	离院方式	VARCHAR2(3)	NOT NULL	二级代码
                15	中途结算标志	VARCHAR2(3)	NOT NULL	二级代码 0：否 1：是
                16	账户使用标志	VARCHAR2(3)	NOT NULL	二级代码 0：否 1：是
                17	TAC	VARCHAR2(3)		
                18	经办人	VARCHAR2(50)	NOT NULL	
                19	次要出院疾病诊断编码1	VARCHAR2(20)		
                20	次要出院疾病诊断编码2	VARCHAR2(20)		
                21	次要出院疾病诊断编码3	VARCHAR2(20)		
                22	次要出院疾病诊断编码4	VARCHAR2(20)		
                23	次要出院疾病诊断编码5	VARCHAR2(20)		
                24	次要出院疾病诊断编码6	VARCHAR2(20)		
                25	次要出院疾病诊断编码7	VARCHAR2(20)		
                26	次要出院疾病诊断编码8	VARCHAR2(20)		
                27	次要出院疾病诊断编码9	VARCHAR2(20)		
                28	次要出院疾病诊断编码10	VARCHAR2(20)		
                29	输入附加信息1	VARCHAR2(500)		
                30	输入附加信息2	VARCHAR2(500)		
                31	输入附加信息3	VARCHAR2(500)		
                32	住院流水号	VARCHAR2(20)	NOT NULL 	就医地定点机构自定义的唯一就诊序列号，一个病人在医院一次就诊的标志。需与住院登记交易上传的医院住院流水号一致
                33	单据号	VARCHAR2(20)	NOT NULL	预结算固定传“0000”。正式结算时就医地定点机构根据实际发票号码传入。如果采集不到的填“1111”
                 */
                StringBuilder inputParam = new StringBuilder();
                inputParam.Append(grbh + "|");
                inputParam.Append(xm + "|");
                inputParam.Append(kh + "|");
                inputParam.Append(dqbh + "|");
                inputParam.Append(ybjzlsh_snyd + "|");
                inputParam.Append("0|");
                inputParam.Append(cyrq + "|");
                inputParam.Append(1 + "|");
                inputParam.Append(bzbm + "|");
                inputParam.Append(bzmc + "|");
                inputParam.Append("|");
                inputParam.Append("|");
                inputParam.Append("|");
                inputParam.Append(9 + "|");
                inputParam.Append(ztjsbz + "|");    //中途结算标志 二级代码 0：否 1：是
                inputParam.Append(zhsybz + "|");
                inputParam.Append("|");
                inputParam.Append(jbr + "|");
                inputParam.Append(bzbm + "|");   //次要出院疾病诊断编码1
                inputParam.Append("|");   //次要出院疾病诊断编码2
                inputParam.Append("|");     //次要出院疾病诊断编码3
                inputParam.Append("|");     //次要出院疾病诊断编码4
                inputParam.Append("|");
                inputParam.Append("|");
                inputParam.Append("|");
                inputParam.Append("|");
                inputParam.Append("|");
                inputParam.Append("|");     //次要出院疾病诊断编码10
                inputParam.Append("|");     //输入附加信息1
                inputParam.Append("|");     //输入附加信息2
                inputParam.Append("|");     //输入附加信息3
                inputParam.Append(ybjzlsh + "|");     //住院流水号
                inputParam.Append(djh+ "|");     //单据号
                #endregion

                StringBuilder inputData = new StringBuilder();
                StringBuilder outputData = new StringBuilder(10240);

                #region 结算前先刷卡
                YWBH = "7100";
                inputData.Append(YWBH + "^");
                inputData.Append(YLGHBH + "^");
                inputData.Append(CZYBH + "^");
                inputData.Append(YWZQH + "^");
                inputData.Append(JYLSH + "^");
                inputData.Append(ZXBM + "^");
                inputData.Append("^");
                inputData.Append(LJBZ + "^");

                int i = BUSINESS_HANDLE(inputData, outputData);
                WriteLog(sysdate + "  结算前刷卡|" + outputData.ToString());
                #endregion

                #region 收费结算
                inputData.Remove(0, inputData.Length);
                YWBH = "7420";
                inputData.Append(YWBH + "^");
                inputData.Append(YLGHBH + "^");
                inputData.Append(CZYBH + "^");
                inputData.Append(YWZQH + "^");
                inputData.Append(JYLSH + "^");
                inputData.Append(ZXBM + "^");
                inputData.Append(inputParam.ToString() + "^");
                inputData.Append(LJBZ + "^");

                WriteLog(sysdate + "  住院收费结算(省内异地)|入参|" + inputData.ToString());
                List<string> liSQL = new List<string>();
                outputData.Remove(0, outputData.Length);
                i = BUSINESS_HANDLE(inputData, outputData);
                if (i == 0)
                {
                    WriteLog(sysdate + "  住院收费结算(省内异地)成功|出参|" + outputData.ToString());
                    string[] sfjsfh = outputData.ToString().Split('^')[2].Split('|');
                    #region 出参

                    outParams_js js = new outParams_js();
                    /*
                     * 1	单据号	VARCHAR2(18)		
                    2	医疗总费用	VARCHAR2(10)		2位小数
                    3	本次账户支付	VARCHAR2(10)		2位小数
                    4	本次现金支付	VARCHAR2(10)		2位小数
                    5	本次统筹支付	VARCHAR2(10)		2位小数
                    6	大病基金支付	VARCHAR2(10)		2位小数
                    7	救助金额	VARCHAR2(10)		2位小数
                    8	公务员补助支付	VARCHAR2(10)		2位小数
                    9	甲类费用	VARCHAR2(10)		2位小数
                    10	乙类费用	VARCHAR2(10)		2位小数
                    11	丙类费用	VARCHAR2(10)		2位小数
                    12	自费费用	VARCHAR2(10)		2位小数
                    13	起付标准自付	VARCHAR2(10)		2位小数
                    14	非医保自付	VARCHAR2(10)		2位小数
                    15	乙类药品自付	VARCHAR2(10)		2位小数
                    16	特检特治自付	VARCHAR2(10)		2位小数
                    17	进入统筹自付	VARCHAR2(10)		2位小数
                    18	进入大病自付	VARCHAR2(10)		2位小数
                    19	输出附加信息1	VARCHAR2(500)		
                    20	输出附加信息2	VARCHAR2(500)		
                    21	输出附加信息3	VARCHAR2(500)		
                    22	其他基金支付	VARCHAR2(16)		2位小数
                     */
                    js.Jslsh = sfjsfh[0];         //返回的单据号
                    js.Ylfze = sfjsfh[1];         //医疗总费用
                    js.Zhzf = sfjsfh[2];          //本次帐户支付
                    js.Xjzf = sfjsfh[3];          //本次现金支付
                    js.Tcjjzf = sfjsfh[4];        //本次统筹支付
                    js.Dejjzf = sfjsfh[5];        //大病基金支付
                    js.Mzjzfy = sfjsfh[6];        //救助金额
                    js.Gwybzjjzf = sfjsfh[7];     //公务员补助支付
                    js.Jlfy = sfjsfh[8];          //甲类费用
                    js.Ylfy = sfjsfh[9];          //乙类费用
                    js.Blfy = sfjsfh[10];         //丙类费用
                    js.Zffy = sfjsfh[11];         //自费费用
                    js.Qfbzfy = sfjsfh[12];       //起付标准自付
                    js.Fybzf = sfjsfh[13];        //非医保自付
                    js.Ylzlfy = sfjsfh[14];       //乙类药品自付
                    js.Tjzlfy = sfjsfh[15];       //特检特治自付
                    js.Tcfdzffy = sfjsfh[16];     //进入统筹自付(统筹分段自付)
                    js.Defdzffy = sfjsfh[17];     //进入大病自付(大病分段自付)
                    string scfjxxo = sfjsfh[18];      //输出附加信息1
                    string scfjxxt = sfjsfh[19];      //输出附加信息2
                    string scfjxxs = sfjsfh[20];      //输出附加信息3
                    js.Qtjjzf = sfjsfh[21];       //其他基金支付
                    js.Yyfdfy = "0.00";
                    js.Cxjfy = "0.00";
                    js.Bcjsqzhye = zhye;
                    js.Ecbcje = "0.00";
                    js.Qybcylbxjjzf = "0.00";

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
                    //总报销金额=医疗总费用-本次现金支付-本次大病支出
                    js.Zbxje = (Convert.ToDecimal(js.Ylfze) - Convert.ToDecimal(js.Xjzf) - Convert.ToDecimal(js.Dejjzf)).ToString();
                    //其他医保支付=医疗总费用-本次现金支付-本次大病支出-本次统筹支付-本次账户支付
                    js.Qtybzf = (Convert.ToDecimal(js.Ylfze) - Convert.ToDecimal(js.Xjzf) - Convert.ToDecimal(js.Dejjzf) - Convert.ToDecimal(js.Tcjjzf) - Convert.ToDecimal(js.Zhzf)).ToString();
                    //现金支付=本次现金支付+本次大病支出
                    js.Ybxjzf = (Convert.ToDecimal(js.Xjzf) + Convert.ToDecimal(js.Dejjzf)).ToString();
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

                    WriteLog(sysdate + "  住院收费结算(省内异地)｜整合出参|" + strValue);
                    #endregion

                    strSql = string.Format(@"insert into ybfyjsdr(
                                             jzlsh, jylsh ,ylfze,zhzf,xjzf,tcjjzf,dejjzf, mzjzfy,gwybzjjzf,jlfy,
                                             ylfy,blfy,zffy,qfbzfy,fybzf,ylypzf,tjzlfy,tcfdzffy,defdzffy,yllb,
                                             xm,grbh,kh,cfmxjylsh,ywzqh,djh,ybjzlsh,jslsh,ybmzlsh,jbr,
                                             zbxje,bcjsqzhye,ztjsbz,jsrq,qtybfy,zhxjzffy)
                                             values('{0}','{1}','{2}','{3}','{4}','{5}' ,'{6}','{7}','{8}' ,'{9}' ,
                                             '{10}','{11}' ,'{12}','{13}' ,'{14}','{15}','{16}' ,'{17}' ,'{18}','{19}',
                                             '{20}' ,'{21}', '{22}','{23}','{24}','{25}','{26}','{27}','{28}','{29}',
                                             '{30}' ,'{31}','{32}','{33}','{34}','{35}')",
                                             jzlsh, JYLSH, js.Ylfze, js.Zhzf, js.Xjzf, js.Tcjjzf, js.Dejjzf, js.Mzjzfy, js.Gwybzjjzf, js.Jlfy,
                                             js.Ylfy, js.Blfy, js.Zffy, js.Qfbzfy, js.Fybzf, js.Ylzlfy, js.Tjzlfy, js.Tcfdzffy, js.Defdzffy, yllb,
                                             xm, grbh, kh, JYLSH, YWZQH, djh, ybjzlsh, js.Jslsh, js.Ybmzlsh, jbr,
                                             js.Zbxje, js.Bcjsqzhye, ztjsbz, jsrqsj, js.Qtybzf, js.Ybxjzf);
                    liSQL.Add(strSql);
                    strSql = string.Format("update ybcfmxscfhdr set ybdjh='{0}' where isnull(ybdjh,'')='' and jzlsh='{1}' and cxbz=1", djh, jzlsh);
                    liSQL.Add(strSql);
                    strSql = string.Format("update ybcfmxscindr set ybdjh='{0}' where isnull(ybdjh,'')='' and jzlsh='{1}' and cxbz=1", djh, jzlsh);
                    liSQL.Add(strSql);
                    //strSql = string.Format("update zy03d set z3fphx='{0}' where isnull(z3fphx,'')='' and z3zyno='{1}' ", djh, jzlsh, grbh);
                    //liSQL.Add(strSql);

                    object[] obj = liSQL.ToArray();
                    obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);
                    if (obj[1].ToString() == "1")
                    {
                        WriteLog(sysdate + "  住院收费结算(省内异地)成功|本地数据操作成功|" + outputData.ToString());
                        return new object[] { 0, 1, strValue };
                    }
                    else
                    {
                        WriteLog(sysdate + "  住院收费结算(省内异地)成功|本地数据操作失败|" + obj[2].ToString());
                        //撤销结算信息
                        object[] objJSCX = { jzlsh, djh, "", xm, kh, grbh, "", djh, ybjzlsh_snyd, dqbh, 2 };
                        NYBZYFYJSCX(objJSCX);

                        return new object[] { 0, 0, "住院收费结算(省内异地)成功|本地数据操作失败|" + obj[2].ToString() };
                    }
                }
                else
                {
                    WriteLog(sysdate + "  住院收费结算(省内异地)失败|" + outputData.ToString());
                    return new object[] { 0, 0, outputData.ToString() };
                }
                #endregion
            }
            catch (Exception error)
            {
                WriteLog(sysdate + "  住院收费结算(省内异地)|系统异常|" + error.Message);
                return new object[] { 0, 2, error.Message };
            }
        }
        #endregion

        #region  住院收费结算撤销
        public static object[] YBZYSFJSCX(object[] objParam)
        {
            string sysdate = GetServerDateTime();
            WriteLog(sysdate + "  进入住院收费结算撤销...");
            try
            {
                CZYBH = CliUtils.fLoginUser;   // 操作员工号 
                YWZQH = CliUtils.fLoginYbNo.Split('|')[0].ToString();   // 业务周期号
                string jbr = CliUtils.fUserName;  //经办人
                string jzlsh = objParam[0].ToString();   // 就诊流水号
                string djh = objParam[1].ToString();     // 结算单据号
                string fphx = "";
                JYLSH = DateTime.Now.ToString("yyyyMMddHHmmss") + "-" + YLGHBH + "-" + new Random().Next(100).ToString().PadLeft(4, '0');

                //获取医保结算信息
                string strSql = string.Format(@"select a.jslsh,a.cfmxjylsh,c.z3fphx,a.jsrq, b.* from ybfyjsdr a 
                                                left join ybmzzydjdr b on a.jzlsh = b.jzlsh and a.cxbz=b.cxbz
                                                left join zy03dw c on a.jzlsh=c.z3zyno and a.djh=c.z3jshx and c.z3endv like '1%'
                                                where a.jzlsh = '{0}'  
                                                and a.djh = '{1}' and a.cxbz = 1  ", jzlsh, djh);
                DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                if (ds.Tables[0].Rows.Count == 0)
                    return new object[] { 0, 0, "该患者无医保结算信息" };

                DataRow dr = ds.Tables[0].Rows[0];
                string ybjzlsh = dr["ybjzlsh"].ToString();    //"1100008927"
                string xm = dr["xm"].ToString();          // // "徐国强""施美荣";
                string kh = dr["kh"].ToString();          // ;"520313262"//"469285189";
                string grbh = dr["grbh"].ToString();      //"360402837288" ;//"360403700513";
                string jsrq = dr["jsrq"].ToString();      //"20160308180140" ;//"20160129000000";
                //string cfmxjylsh = dr["cfmxjylsh"].ToString();// //"处方明细交易流水号";
                string jslsh = dr["jslsh"].ToString();    //结算流水号  曹晓红  20160818
                string ybjzlsh_snyd = dr["ybjzlsh_snyd"].ToString();
                string dqbh = dr["tcqh"].ToString();
                DQJBBZ = dr["dqjbbz"].ToString();    //地区级别标志
                fphx = dr["z3fphx"].ToString();

                #region 入参
                StringBuilder inputParam = new StringBuilder();
                if (DQJBBZ.Equals("1"))
                {
                    //1、住院流水号(门诊流水号) | 2、单据号 | 3、经办人 | 4、是否保存处方标志 | 5、结算日期
                    inputParam.Append(ybjzlsh + "|");
                    inputParam.Append(jslsh + "|");
                    inputParam.Append(jbr + "|");
                    inputParam.Append("1|");
                    inputParam.Append(jsrq + "|");
                    YWBH = "2430";
                }
                else
                {
                    //0个人编号 | 1姓名 | 2卡号 | 3地区编号 | 4住院流水号 | 5单据号 | 6是否保存处方标志 | 6经办人
                    inputParam.Append(grbh + "|");
                    inputParam.Append(xm + "|");
                    inputParam.Append(kh + "|");
                    inputParam.Append(dqbh + "|");
                    inputParam.Append(ybjzlsh_snyd + "|");
                    inputParam.Append(djh + "|");
                    inputParam.Append("1|");
                    inputParam.Append(jbr + "|");
                    YWBH = "7430";
                }

                StringBuilder inputData = new StringBuilder();
                inputData.Append(YWBH + "^");
                inputData.Append(YLGHBH + "^");
                inputData.Append(CZYBH + "^");
                inputData.Append(YWZQH + "^");
                inputData.Append(JYLSH + "^");
                inputData.Append(ZXBM + "^");
                inputData.Append(inputParam.ToString() + "^");
                inputData.Append(LJBZ + "^");

                WriteLog(sysdate + "  住院收费结算撤销|入参|" + inputData.ToString());
                StringBuilder outputData = new StringBuilder(10240);
                #endregion

                int i = BUSINESS_HANDLE(inputData, outputData);
                if (i == 0)
                {
                    WriteLog(sysdate + "  住院收费结算撤销|出参|" + outputData.ToString());

                    List<string> liSql = new List<string>();
                    //撤销结算信息
                    strSql = string.Format(@"insert into ybfyjsdr(jzlsh,jylsh,ylfze,zbxje,tcjjzf,dejjzf,zhzf,xjzf,
                                              gwybzjjzf,qybcylbxjjzf,zffy,dwfdfy,yyfdfy,mzjzfy,cxjfy,ylzlfy,blzlfy,fhjbylfy,
                                              qfbzfy,zzzyzffy,jrtcfy,tcfdzffy,ctcfdxfy,jrdebxfy,defdzffy,cdefdxfy,rgqgzffy,
                                              bcjsqzhye,bntczflj,bndezflj,bnczjmmztczflj,bngwybzzflj,bnzhzflj,bnzycslj,zycs,
                                              xm,jsrq,yllb,yldylb,jbjgbm,ywzqh,jslsh,tsxx,djh,jylx,yyjylsh,yxbz,grbhgl,yljgbm,
                                              ecbcje,mmqflj,jsfjylsh,grbh,dbzbc,czzcje,elmmxezc,elmmxesy,jmgrzfecbcje,tjje,syjjzf,
                                              kh,cfmxjylsh,cxbz,ztjsbz,ybjzlsh,qtybfy,zhxjzffy,sysdate) 
                                              select jzlsh,jylsh,ylfze,zbxje,tcjjzf,dejjzf,zhzf,xjzf,
                                              gwybzjjzf,qybcylbxjjzf,zffy,dwfdfy,yyfdfy,mzjzfy,cxjfy,ylzlfy,blzlfy,fhjbylfy,
                                              qfbzfy,zzzyzffy,jrtcfy,tcfdzffy,ctcfdxfy,jrdebxfy,defdzffy,cdefdxfy,rgqgzffy,
                                              bcjsqzhye,bntczflj,bndezflj,bnczjmmztczflj,bngwybzzflj,bnzhzflj,bnzycslj,zycs,
                                              xm,jsrq,yllb,yldylb,jbjgbm,ywzqh,jslsh,tsxx,djh,jylx,yyjylsh,yxbz,grbhgl,yljgbm,
                                              ecbcje,mmqflj,jsfjylsh,grbh,dbzbc,czzcje,elmmxezc,elmmxesy,jmgrzfecbcje,tjje,syjjzf,
                                              kh,cfmxjylsh,0,ztjsbz,ybjzlsh,qtybfy,zhxjzffy,'{2}'
                                              from ybfyjsdr where jzlsh = '{0}' and djh = '{1}' and cxbz = 1", jzlsh, djh, sysdate);
                    liSql.Add(strSql);

                    strSql = string.Format(@"update ybfyjsdr set cxbz = 2 where jzlsh = '{0}' and djh = '{1}' and cxbz = 1", jzlsh, djh);
                    liSql.Add(strSql);

                    //撤销费用明细
                    strSql = string.Format(@"insert into ybcfmxscfhdr(jzlsh,jylsh,je,zlje,zfje,sfxmdj,zfbz,xzsybz,bz,ybjzlsh,cxbz,sysdate) 
                                            select jzlsh,jylsh,je,zlje,zfje,sfxmdj,zfbz,xzsybz,bz,ybjzlsh, 0,'{2}' 
                                           from ybcfmxscfhdr where jzlsh = '{0}' and ybdjh='{1}' and cxbz = 1", jzlsh, djh, sysdate);
                    liSql.Add(strSql);
                    strSql = string.Format(@"update ybcfmxscfhdr set cxbz = 2 where jzlsh = '{0}' and ybdjh='{1}' and cxbz = 1", jzlsh, djh);
                    liSql.Add(strSql);
                    strSql = string.Format(@"insert into ybcfmxscindr(
                                                jzlsh,jylsh,xm,kh,ybjzlsh,cfrq,yysfxmbm,yysfxmmc,sfxmzxbm,sfxmzxmc,
                                                dj,sl,je,jbr,cxbz,sysdate) 
                                                select jzlsh,jylsh,xm,kh,ybjzlsh,cfrq,yysfxmbm,yysfxmmc,sfxmzxbm,sfxmzxmc,
                                                dj,sl,je,jbr,0,'{2}' from ybcfmxscindr 
                                                where jzlsh = '{0}' and ybdjh='{1}' and cxbz = 1", jzlsh, djh, sysdate);
                    liSql.Add(strSql);
                    strSql = string.Format("update ybcfmxscindr set cxbz = 2 where jzlsh = '{0}' and ybdjh='{1}' and cxbz = 1", jzlsh, djh);
                    liSql.Add(strSql);

                    //取消上传标志
                    strSql = string.Format(@"update zy03d set z3ybup = null where z3zyno = '{0}' and z3fphx='{1}'", jzlsh, fphx);
                    liSql.Add(strSql);
                    object[] obj = liSql.ToArray();
                    obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);
                    if (obj[1].ToString() == "1")
                    {
                        WriteLog(sysdate + " 住院收费结算撤销成功|本地数据操作成功|" + outputData.ToString());
                        return new object[] { 0, 1, outputData.ToString() };
                    }
                    else
                    {
                        WriteLog(sysdate + "   住院收费结算撤销成功|本地数据操作失败|" + obj[2].ToString());
                        return new object[] { 0, 0, obj[2].ToString() };
                    }
                }
                else
                {
                    WriteLog(sysdate + "   住院收费结算撤销失败|" + outputData.ToString());
                    return new object[] { 0, 0, outputData.ToString() };
                }
            }
            catch (Exception ex)
            {
                WriteLog(sysdate + "   住院收费结算撤销|系统异常|" + ex.Message);
                return new object[] { 0, 0, ex.Message };
            }
        }
        #endregion

        #region 住院收费结算单打印
        public static object[] YBZYJSD(object[] objParam)
        {
            WriteLog("结算单不启用.....");
            return new object[] { 0,1,"结算单不启用"};
        }

        #endregion

        #region 门诊登记撤销_内部
        public static object[] NYBMZDJCX(object[] objParam)
        {
            string sysdate = GetServerDateTime();
            WriteLog(sysdate + "  进入门诊登记撤销(内部)...");

            CZYBH = CliUtils.fLoginUser;    //操作员工号
            YWZQH = CliUtils.fLoginYbNo.Split('|')[0].ToString();    //业务周期号

            string jzlsh = objParam[0].ToString();
            string jbr = objParam[1].ToString();
            string yllb = objParam[2].ToString();
            string grbh = objParam[3].ToString();
            string xm = objParam[4].ToString();
            string kh = objParam[5].ToString();
            string dqbh = objParam[6].ToString();
            string ybjzlsh_snyd = objParam[7].ToString();
            DQJBBZ = objParam[8].ToString();

            JYLSH = DateTime.Now.ToString("yyyyMMddHHmmss") + "-" + YLGHBH + "-" + new Random().Next(100).ToString().PadLeft(4, '0');
            try
            {
                string ybjzlsh = "MZ" + yllb + jzlsh;

                StringBuilder inputParam = new StringBuilder();

                if (DQJBBZ.Equals("1"))
                {
                    inputParam.Append(ybjzlsh + "|");
                    inputParam.Append(jbr + "|");
                    inputParam.Append(yllb + "|");
                    YWBH = "2240";
                }
                else
                {
                    inputParam.Append(grbh + "|");
                    inputParam.Append(xm + "|");
                    inputParam.Append(kh + "|");
                    inputParam.Append(dqbh + "|");
                    inputParam.Append(ybjzlsh_snyd + "|");
                    YWBH = "7150";
                }

                StringBuilder inputData = new StringBuilder();
                inputData.Append(YWBH + "^");
                inputData.Append(YLGHBH + "^");
                inputData.Append(CZYBH + "^");
                inputData.Append(YWZQH + "^");
                inputData.Append(JYLSH + "^");
                inputData.Append(ZXBM + "^");
                inputData.Append(inputParam.ToString() + "^");
                inputData.Append(LJBZ + "^");
                WriteLog(sysdate + "  门诊登记撤销|入参|" + inputData.ToString());

                StringBuilder outputData = new StringBuilder(1024);
                int i = BUSINESS_HANDLE(inputData, outputData);

                List<string> liSQL = new List<string>();
                if (i == 0)
                {
                    WriteLog(sysdate + "  门诊登记撤销成功|出参|" + outputData.ToString());
                    return new object[] { 0, 1, outputData.ToString() };
                }
                else
                {
                    WriteLog(sysdate + "  门诊登记撤销失败|出参|" + outputData.ToString());
                    return new object[] { 0, 0, outputData.ToString() };
                }
            }
            catch (Exception error)
            {
                WriteLog(sysdate + "  门诊登记撤销|接口异常|" + error.Message);
                return new object[] { 0, 2, error.Message };
            }
        }
        #endregion

        #region 门诊费用登记撤销_内部
        public static object[] NYBMZCFMXCX(object[] objParam)
        {
            string sysdate = GetServerDateTime();
            WriteLog(sysdate + "   进入门诊费用登记撤销(内部)...");
            try
            {
                CZYBH = CliUtils.fLoginUser;  // 操作员工号
                YWZQH = CliUtils.fLoginYbNo.Split('|')[0].ToString();  // 业务周期号
                string jbr = CliUtils.fUserName;
                string jzlsh = objParam[0].ToString();  // 就诊流水号
                string cxjylsh = objParam[1].ToString();  // 撤销交易流水号(全部撤销则传空字符串)
                string ybjzlsh = objParam[2].ToString();  //医保就诊流水号

                JYLSH = DateTime.Now.ToString("yyyyMMddHHmmss") + "-" + YLGHBH + "-" + new Random().Next(100).ToString().PadLeft(4, '0');


                // 1、就诊流水号 | 2、被撤销交易流水号（如果只撤销一部分，则此值不为空） | 3、经办人
                YWBH = "2320";
                StringBuilder inputParam = new StringBuilder();
                inputParam.Append(ybjzlsh + "|");
                inputParam.Append(cxjylsh + "|");
                inputParam.Append(jbr + "|");

                StringBuilder inputData = new StringBuilder();
                inputData.Append(YWBH + "^");
                inputData.Append(YLGHBH + "^");
                inputData.Append(CZYBH + "^");
                inputData.Append(YWZQH + "^");
                inputData.Append(JYLSH + "^");
                inputData.Append(ZXBM + "^");
                inputData.Append(inputParam.ToString() + "^");
                inputData.Append(LJBZ + "^");

                List<string> liSQL = new List<string>();
                WriteLog(sysdate + "  门诊费用登记撤销(内部)|入参|" + inputData.ToString());
                StringBuilder outputData = new StringBuilder(10240);
                int i = BUSINESS_HANDLE(inputData, outputData);
                if (i == 0)
                {
                    WriteLog(sysdate + "  门诊费用登记撤销成功(内部)|" + outputData.ToString());
                    return new object[] { 0, 1, outputData.ToString() };
                }
                else
                {
                    WriteLog(sysdate + "  门诊费用登记撤销失败(内部)|" + outputData.ToString());
                    return new object[] { 0, 0, outputData.ToString() };
                }
            }
            catch (Exception ex)
            {
                WriteLog(sysdate + "  系统异常|" + ex.Message);
                return new object[] { 0, 2, "Error:" + ex.Message };
            }
        }
        #endregion

        #region 门诊费用结算撤销(内部)
        public static object[] NYBFYJSCX(object[] objParam)
        {
            string sysdate = GetServerDateTime();
            WriteLog(sysdate + "  进入门诊费用结算撤销(内部)...");
            try
            {
                CZYBH = CliUtils.fLoginUser;            //操作员工号
                YWZQH = CliUtils.fLoginYbNo.Split('|')[0].ToString();            //业务周期号
                string jbr = CliUtils.fUserName;        // 经办人姓名
                string jzlsh = objParam[0].ToString();  // 就诊流水号
                string djh = objParam[1].ToString();    // 发票号
                string ybjzlsh = objParam[2].ToString();
                string jsrq = objParam[3].ToString();
                string grbh = objParam[4].ToString();
                string xm = objParam[5].ToString();
                string kh = objParam[6].ToString();
                string dqbh = objParam[7].ToString();
                string ybjzlsh_snyd = objParam[8].ToString();
                string cfmxjylsh = objParam[9].ToString();
                DQJBBZ = objParam[10].ToString();

                #region 入参
                StringBuilder inputParam = new StringBuilder();
                if (DQJBBZ.Equals("1"))
                {
                    /*住院流水号(门诊流水号)	VARCHAR2(18)	NOT NULL	同登记时的住院流水号（门诊流水号）
                    单据号	VARCHAR2(18)	NOT NULL	唯一
                    经办人	VARCHAR2(20)	NOT NULL	医疗机构操作员姓名
                    是否保存处方标志	VARCHAR2(3)		0不保存1保存,默认不保存
                    结算日期	VARCHAR2(14)		
                     */
                    YWBH = "2430";
                    inputParam.Append(ybjzlsh + "|");
                    inputParam.Append(djh + "|");
                    inputParam.Append(jbr + "|");
                    inputParam.Append("0" + "|");
                    inputParam.Append(jsrq + "|");
                }
                else if (DQJBBZ.Equals("2"))
                {
                    YWBH = "7140";
                    inputParam.Append(grbh + "|");
                    inputParam.Append(xm + "|");
                    inputParam.Append(kh + "|");
                    inputParam.Append(dqbh + "|");
                    inputParam.Append(ybjzlsh_snyd + "|");
                    inputParam.Append(djh + "|");
                }

                StringBuilder inputData = new StringBuilder();
                inputData.Append(YWBH + "^");
                inputData.Append(YLGHBH + "^");
                inputData.Append(CZYBH + "^");
                inputData.Append(YWZQH + "^");
                inputData.Append(JYLSH + "^");
                inputData.Append(ZXBM + "^");
                inputData.Append(inputParam.ToString() + "^");
                inputData.Append(LJBZ + "^");
                WriteLog(sysdate + "  门诊费用结算撤销(内部)|入参|" + inputData.ToString());
                #endregion

                StringBuilder outputData = new StringBuilder(1024);
                int i = BUSINESS_HANDLE(inputData, outputData);
                if (i == 0)
                {
                    List<string> liSql = new List<string>();
                    //本市级在做费用结算撤销时，需撤销门诊费用上传信息
                    string strSql = string.Format(@"insert into ybcfmxscfhdr(jzlsh,jylsh,je,zlje,zfje,sfxmdj,zfbz,xzsybz,bz,cxbz,sysdate)
                                                     select jzlsh,jylsh,je,zlje,zfje,sfxmdj,zfbz,xzsybz,bz,0,'{2}' from ybcfmxscfhdr
                                                    where ybjzlsh='{0}' and jylsh='{1}' and cxbz=1", ybjzlsh, cfmxjylsh, sysdate);
                    liSql.Add(strSql);
                    strSql = string.Format(@"update ybcfmxscfhdr set cxbz=2 where ybjzlsh='{0}' and jylsh='{1}' and cxbz=1", ybjzlsh, cfmxjylsh);
                    liSql.Add(strSql);
                    strSql = string.Format(@"insert into ybcfmxscindr(jzlsh,jylsh,xm,kh,ybjzlsh,cfrq,yysfxmbm,yysfxmmc,sfxmzxbm,sfxmzxmc,
                                            dj,sl,je,jbr,cxbz,sysdate)
                                            select jzlsh,jylsh,xm,kh,ybjzlsh,cfrq,yysfxmbm,yysfxmmc,sfxmzxbm,sfxmzxmc,
                                            dj,sl,je,jbr,0,'{2}' from ybcfmxscindr
                                            where ybjzlsh='{0}' and jylsh='{1}' and cxbz=1", ybjzlsh, cfmxjylsh, sysdate);
                    liSql.Add(strSql);
                    strSql = string.Format(@"update ybcfmxscindr set cxbz=2 where ybjzlsh='{0}' and jylsh='{1}' and cxbz=1", ybjzlsh, cfmxjylsh, sysdate);
                    liSql.Add(strSql);
                    object[] obj = liSql.ToArray();
                    obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);
                    if (obj[1].ToString() == "1")
                    {
                        WriteLog(sysdate + "  门诊费用结算撤销(内部)成功|本地数据操作成功|" + outputData.ToString());
                        return new object[] { 0, 1, outputData.ToString() };
                    }
                    else
                    {
                        WriteLog(sysdate + "  门诊费用结算撤销(内部)成功|本地数据操作失败|" + obj[2].ToString());
                        return new object[] { 0, 0, obj[2].ToString() };
                    }
                }
                else
                {
                    WriteLog(sysdate + "  门诊费用结算撤销(内部)失败|" + outputData.ToString());
                    return new object[] { 0, 0, outputData.ToString() };
                }
            }
            catch (Exception ex)
            {
                WriteLog(sysdate + "  门诊费用结算撤销(内部)|系统异常|" + ex.Message);
                return new object[] { 0, 2, ex.Message };
            }
        }
        #endregion

        #region 住院登记撤销_内部
        public static object[] NYBZYDJCX(object[] objParam)
        {
            string sysdate = GetServerDateTime();
            WriteLog(sysdate + "  进入住院登记撤销(内部)...");

            try
            {
                string jzlsh = objParam[0].ToString();      // 就诊流水号
                string ybjzlsh = objParam[1].ToString();    // 医保就诊流水号
                string yllb = objParam[2].ToString();
                string grbh = objParam[3].ToString();       // 个人编号
                string xm = objParam[4].ToString();         // 姓名
                string kh = objParam[5].ToString();         // 卡号
                string dqbh = objParam[6].ToString();       // 地区编号
                string ybjzlsh_snyd = objParam[7].ToString();
                DQJBBZ = objParam[8].ToString();
                CZYBH = CliUtils.fLoginUser;  // 操作员工号
                YWZQH = CliUtils.fLoginYbNo.Split('|')[0].ToString();  // 业务周期号
                string jbr = CliUtils.fLoginUser;

                //交易流水号
                JYLSH = DateTime.Now.ToString("yyyyMMddHHmmss") + "-" + YLGHBH + "-" + new Random().Next(100).ToString().PadLeft(4, '0');

                StringBuilder inputParam = new StringBuilder();
                if (DQJBBZ.Equals("1"))
                {
                    YWBH = "2240";
                    inputParam.Append(ybjzlsh + "|");
                    inputParam.Append(jbr + "|");
                    inputParam.Append(yllb + "|");
                }
                else
                {
                    YWBH = "7220";
                    inputParam.Append(grbh + "|");
                    inputParam.Append(xm + "|");
                    inputParam.Append(kh + "|");
                    inputParam.Append(dqbh + "|");
                    inputParam.Append(ybjzlsh_snyd + "|");
                }

                StringBuilder inputData = new StringBuilder();
                inputData.Append(YWBH + "^");
                inputData.Append(YLGHBH + "^");
                inputData.Append(CZYBH + "^");
                inputData.Append(YWZQH + "^");
                inputData.Append(JYLSH + "^");
                inputData.Append(ZXBM + "^");
                inputData.Append(inputParam.ToString() + "^");
                inputData.Append(LJBZ + "^");

                WriteLog(sysdate + "  住院登记撤销|入参|" + inputData.ToString());
                StringBuilder outputData = new StringBuilder(102400);
                int i = BUSINESS_HANDLE(inputData, outputData);
                if (i == 0)
                {
                    WriteLog(sysdate + "  住院登记撤销成功|" + outputData.ToString());
                    return new object[] { 0, 1, outputData.ToString() };
                }
                else
                {
                    WriteLog(sysdate + "  住院登记撤销失败|" + outputData.ToString());
                    return new object[] { 0, 0, outputData.ToString() };
                }
            }
            catch (Exception error)
            {
                WriteLog(sysdate + "  住院登记撤销|系统异常|" + error.Message);
                return new object[] { 0, 2, "Error:" + error.Message };
            }

        }
        #endregion

        #region 住院费用登记撤销(内部)
        public static object[] NYBZYCFMXSCCX(object[] objParam)
        {
            string sysdate = GetServerDateTime();
            WriteLog(sysdate + "  进入住院费用登记撤销(内部)...");
            try
            {
                string ybjzlsh = objParam[0].ToString(); // 就诊流水号
                string cfmxjylsh = objParam[1].ToString();  //处方交易流水号
                string jbr = objParam[2].ToString();   // 经办人姓名
                string grbh = objParam[3].ToString();
                string xm = objParam[4].ToString();
                string kh = objParam[5].ToString();
                string dqbh = objParam[6].ToString();
                string ybjzlsh_sndy = objParam[7].ToString();
                DQJBBZ = objParam[8].ToString();


                JYLSH = DateTime.Now.ToString("yyyyMMddHHmmss") + "-" + YLGHBH + "-" + new Random().Next(100).ToString().PadLeft(4, '0');

                StringBuilder inputParam = new StringBuilder();
                if (DQJBBZ.Equals("1"))
                {
                    //1、就诊流水号 | 2、被撤销交易流水号（如果只撤销一部分，则此值不为空） | 3、经办人
                    YWBH = "2320";
                    inputParam.Append(ybjzlsh + "|");
                    inputParam.Append(cfmxjylsh + "|");
                    inputParam.Append(jbr + "|");
                }
                else
                {
                    // // 0个人编号 | 1姓名 | 2卡号 | 3地区编号 | 4住院流水号
                    YWBH = "7321";
                    inputParam.Append(grbh + "|");
                    inputParam.Append(xm + "|");
                    inputParam.Append(kh + "|");
                    inputParam.Append(dqbh + "|");
                    inputParam.Append(ybjzlsh_sndy + "|");
                }

                StringBuilder inputData = new StringBuilder();
                inputData.Append(YWBH + "^");
                inputData.Append(YLGHBH + "^");
                inputData.Append(CZYBH + "^");
                inputData.Append(YWZQH + "^");
                inputData.Append(JYLSH + "^");
                inputData.Append(ZXBM + "^");
                inputData.Append(inputParam.ToString() + "^");
                inputData.Append(LJBZ + "^");
                WriteLog(sysdate + "  住院费用登记撤销(内部)|入参|" + inputData.ToString());

                StringBuilder outputData = new StringBuilder(1024);
                int i = BUSINESS_HANDLE(inputData, outputData);
                if (i == 0)
                {
                    WriteLog(sysdate + "  住院费用登记撤销(内部)成功|" + outputData.ToString());
                    return new object[] { 0, 1, "住院费用登记撤销成功" };
                }
                else
                {
                    WriteLog(sysdate + "  住院费用登记撤销(内部)失败|" + outputData.ToString());
                    return new object[] { 0, 0, outputData.ToString() };
                }
            }
            catch (Exception ex)
            {
                WriteLog(sysdate + "  住院费用登记撤销(内部)|系统异常" + ex.Message);
                return new object[] { 0, 0, "系统异常" + ex.Message };
            }
        }
        #endregion

        #region  住院收费结算撤销(内部)
        public static object[] NYBZYFYJSCX(object[] objParam)
        {
            string sysdate = GetServerDateTime();
            WriteLog(sysdate + "  进入住院收费结算撤销(内部)...");
            try
            {
                CZYBH = CliUtils.fLoginUser;   // 操作员工号 
                YWZQH = CliUtils.fLoginYbNo.Split('|')[0].ToString();   // 业务周期号
                string jbr = CliUtils.fUserName;  //经办人
                string jzlsh = objParam[0].ToString();   // 就诊流水号
                string djh = objParam[1].ToString();     // 结算单据号
                string ybjzlsh = objParam[2].ToString();   //"1100008927"
                string xm = objParam[3].ToString();         // // "徐国强""施美荣";
                string kh = objParam[4].ToString();          // ;"520313262"//"469285189";
                string grbh = objParam[5].ToString();     //"360402837288" ;//"360403700513";
                string jsrq = objParam[6].ToString();      //"20160308180140" ;//"20160129000000";
                string jslsh = objParam[7].ToString();    //结算流水号  曹晓红  20160818
                string ybjzlsh_snyd = objParam[8].ToString();
                string dqbh = objParam[9].ToString();
                DQJBBZ = objParam[10].ToString();    //地区级别标志
                JYLSH = DateTime.Now.ToString("yyyyMMddHHmmss") + "-" + YLGHBH + "-" + new Random().Next(100).ToString().PadLeft(4, '0');

                #region 入参
                StringBuilder inputParam = new StringBuilder();
                if (DQJBBZ.Equals("1"))
                {
                    /*
                     1		住院流水号(门诊流水号)	VARCHAR2(18)	NOT NULL	同登记时的住院流水号（门诊流水号）
                    2		单据号	VARCHAR2(18)	NOT NULL	唯一
                    3		经办人	VARCHAR2(20)	NOT NULL	医疗机构操作员姓名
                    4		是否保存处方标志	VARCHAR2(3)		0不保存1保存,默认不保存
                    5		结算日期	VARCHAR2(14)		
                    */
                    inputParam.Append(ybjzlsh + "|");
                    inputParam.Append(djh + "|");
                    inputParam.Append(jbr + "|");
                    inputParam.Append("1|");
                    inputParam.Append(jsrq + "|");
                    YWBH = "2430";
                }
                else
                {
                    /*
                     1	个人编号	VARCHAR2(10)	NOT NULL	
                    2	姓名	VARCHAR2(20)	NOT NULL	
                    3	卡号	VARCHAR2(18)	NOT NULL	
                    4	地区编号	VARCHAR2(6)	NOT NULL	
                    5	住院号	VARCHAR2(18)	NOT NULL	
                    6	单据号	VARCHAR2(18)	NOT NULL	
                    7	是否保存处方标志	VARCHAR2(8)		0不保存1保存,默认不保存
                    8	住院流水号	VARCHAR2(2)	NOT NULL	就医地定点机构自定义的唯一就诊序列号，一个病人在医院一次就诊的标志。需与住院登记交易上传的医院住院流水号一致
                    9	经办人	VARCHAR2(50)	NOT NULL	
                    */
                    inputParam.Append(grbh + "|");
                    inputParam.Append(xm + "|");
                    inputParam.Append(kh + "|");
                    inputParam.Append(dqbh + "|");
                    inputParam.Append(ybjzlsh_snyd + "|");
                    inputParam.Append(jslsh + "|");
                    inputParam.Append("1|");
                    inputParam.Append(jbr + "|");
                    YWBH = "7430";
                }

                StringBuilder inputData = new StringBuilder();
                inputData.Append(YWBH + "^");
                inputData.Append(YLGHBH + "^");
                inputData.Append(CZYBH + "^");
                inputData.Append(YWZQH + "^");
                inputData.Append(JYLSH + "^");
                inputData.Append(ZXBM + "^");
                inputData.Append(inputParam.ToString() + "^");
                inputData.Append(LJBZ + "^");

                WriteLog(sysdate + "  住院收费结算撤销(内部)|入参|" + inputData.ToString());
                StringBuilder outputData = new StringBuilder(10240);
                #endregion

                int i = BUSINESS_HANDLE(inputData, outputData);
                if (i == 0)
                {
                    WriteLog(sysdate + "  住院收费结算撤销(内部)|出参|" + inputData.ToString());

//                    List<string> liSql = new List<string>();

//                    string strSql = string.Format(@"insert into ybcfmxscindr(jzlsh,jylsh,xm,kh,ybjzlsh,cfrq,yysfxmbm,yysfxmmc,sfxmzxbm,sfxmzxmc,
//                                            dj,sl,je,jbr,sflb,sfxmdj,cxbz,sysdate) 
//                                            select jzlsh,jylsh,xm,kh,ybjzlsh,cfrq,yysfxmbm,yysfxmmc,sfxmzxbm,sfxmzxmc,
//                                            dj,sl,je,jbr,sflb,sfxmdj,0,'{1}' from ybcfmxscindr 
//                                            where jzlsh = '{0}' and isnull(ybdjh,'')='' and cxbz = 1", jzlsh, sysdate);
//                    liSql.Add(strSql);
//                    strSql = string.Format("update ybcfmxscindr set cxbz = 2 where jzlsh = '{0}' and isnull(ybdjh,'')='' and cxbz = 1", jzlsh);
//                    liSql.Add(strSql);
//                    strSql = string.Format(@"insert into ybcfmxscfhdr(jzlsh,jylsh,je,zlje,zfje,sfxmdj,zfbz,xzsybz, bz,ybjzlsh,yybxmbh,ybxmmc,sflb,ybcfh,cxbz,sysdate) 
//                                            select jzlsh,jylsh,je,zlje,zfje,sfxmdj,zfbz,xzsybz, bz,ybjzlsh,yybxmbh,ybxmmc,sflb,ybcfh,0,'{1}' from ybcfmxscfhdr 
//                                            where jzlsh = '{0}' and isnull(ybdjh,'')='' and cxbz = 1", jzlsh, sysdate);
//                    liSql.Add(strSql);
//                    strSql = string.Format("update ybcfmxscfhdr set cxbz = 2 where jzlsh = '{0}' and isnull(ybdjh,'')='' and cxbz = 1", jzlsh);
//                    liSql.Add(strSql);

//                    strSql = string.Format("update zy03d set z3ybup = null where z3ybup is not null and isnull(z3fphx,'')='' and z3zyno = '{0}'", jzlsh);
//                    liSql.Add(strSql);

//                    object[] obj = liSql.ToArray();
//                    obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);
//                    if (obj[1].ToString() == "1")
//                    {
                        WriteLog(sysdate + " 住院收费结算撤销(内部)成功|本地数据操作成功|" + outputData.ToString());
                        return new object[] { 0, 1, "住院收费结算撤销(内部)成功" };
                    //}
                    //else
                    //{
                    //    WriteLog(sysdate + "   住院收费结算撤销(内部)成功|本地数据操作失败|" + obj[2].ToString());
                    //    return new object[] { 0, 0, obj[2].ToString() };
                    //}
                }
                else
                {
                    WriteLog(sysdate + "   住院收费结算撤销(内部)失败|" + outputData.ToString());
                    return new object[] { 0, 0, outputData.ToString() };
                }
            }
            catch (Exception ex)
            {
                WriteLog(sysdate + "   住院收费结算撤销(内部)|系统异常|" + ex.Message);
                return new object[] { 0, 0, ex.Message };
            }
        }
        #endregion

        #region 住院费用上传方法(本市级)
        private static bool insertYBcfmxscfh(StringBuilder outputData, List<string> liybcfh, List<string> liyyxmdm, List<string> liyyxmmc,
           string jzlsh, string jylsh, string grbh, string xm, string kh, string ybjzlsh)
        {
            try
            {
                string[] zysfdjfhs = outputData.ToString().Split('^')[2].TrimEnd('$').Split('$');
                List<string> lizysfdjfh = new List<string>();
                object[] obj_ins = null;
                string strSql = string.Empty;

                for (int j = 0; j < zysfdjfhs.Length; j++)
                {
                    //金额 、 自理金额 、 自费金额 、 收费项目等级 、 全额自费标志 、 限制使用范围标志 、 限制使用范围
                    string[] zysfdjfh = zysfdjfhs[j].Split('|');
                    decimal je;     //金额
                    bool isConvert = decimal.TryParse(zysfdjfh[0], out je);
                    decimal zlje;   //自理金额
                    isConvert = decimal.TryParse(zysfdjfh[1], out zlje);
                    decimal zfje;   //自费金额
                    isConvert = decimal.TryParse(zysfdjfh[2], out zfje);
                    //收费项目等级
                    string sfxmdj = zysfdjfh[3].Trim();
                    //全额自费标志
                    string qezfbz = zysfdjfh[4].Trim();
                    //限制使用范围标志
                    string xzsyfwbz = zysfdjfh[5].Trim();
                    //限制使用范围
                    string xzsyfw = zysfdjfh[6].Trim();

                    strSql = string.Format(@"insert into ybcfmxscfhdr(jzlsh,jylsh,je,zlje,zfje,cxjzfje,sflb,sfxmdj,qezfbz,zlbl,
                                            xj,bz,grbh,xm,kh,cfh,ybcfh,yyxmdm,yyxmmc,xzsyfwbz,xzsyfw,ybjzlsh) values
                                            ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}',
                                            '{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}','{20}','{21}')"
                                            , jzlsh, jylsh, je, zlje, zfje, 0, "", sfxmdj, qezfbz, 0,
                                            0, "", grbh, xm, kh, jzlsh, liybcfh[j], liyyxmdm[j], liyyxmmc[j], xzsyfwbz, xzsyfw, ybjzlsh);
                    lizysfdjfh.Add(strSql);
                }

                strSql = string.Format("update zy03d set z3ybup = '{0}' where z3ybup is null and z3zyno = '{1}'", jylsh, jzlsh);
                lizysfdjfh.Add(strSql);
                strSql = string.Format("update zy03dz set z3ybup = '{0}' where z3ybup is null and z3zyno = '{1}'", jylsh, jzlsh);
                lizysfdjfh.Add(strSql);

                obj_ins = lizysfdjfh.ToArray();

                obj_ins = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj_ins);

                if (obj_ins[1].ToString() == "1")
                {
                    WriteLog("费用上传成功|本地数据操作成功|");
                    return true;
                }
                else
                {
                    WriteLog("费用上传成功|本地数据操作失败|" + obj_ins[2].ToString());
                    return false;
                }
            }
            catch (Exception err)
            {
                WriteLog("费用上传成功|系统异常|" + err.ToString());
                return false;
            }
        }
        #endregion

        #region 住院费用上传方法(省内异地)
        private static bool insertYBcfmxscfhSNYD(StringBuilder outputData, List<string> liybcfh, List<string> liyyxmdm, List<string> liyyxmmc,
           string jzlsh, string jylsh, string grbh, string xm, string kh)
        {
            try
            {
                //string[] zysfdjfhs = outputData.ToString().Split('^')[2].TrimEnd('$').Split('$');
                string[] zysfdjfhs = outputData.ToString().TrimEnd('$').Split('$');
                List<string> lizysfdjfh = new List<string>();
                object[] obj_ins = null;
                string strSql = string.Empty;
                for (int j = 0; j < zysfdjfhs.Length; j++)
                {
                    //金额 、 自理金额 、 自费金额 、 收费项目等级 、 全额自费标志 、 限制使用范围标志 、 限制使用范围
                    //住院流水号  处方号  项目编号 项目名称 项目等级  收费类别 单价 数量 金额

                    string[] zysfdjfh = zysfdjfhs[j].Split('|');
                    string ybjzlsh = zysfdjfh[0].Trim();     //住院流水号
                    string ybcfh = zysfdjfh[1].Trim();   //医保处方号
                    string yyxmdm = zysfdjfh[2].Trim();  //项目编号
                    string yyxmmc = zysfdjfh[3].Trim();  //项目名称
                    string sfxmdj = zysfdjfh[4].Trim();    //项目等级
                    string sflb = zysfdjfh[5].Trim();      //收费类别
                    decimal dj;   //单价
                    bool isConvert = decimal.TryParse(zysfdjfh[6], out dj);
                    decimal sl;   //数量
                    isConvert = decimal.TryParse(zysfdjfh[7], out sl);
                    decimal je;   //金额
                    isConvert = decimal.TryParse(zysfdjfh[8], out je);

                    strSql = string.Format(@"insert into ybcfmxscfhdr(jzlsh,jylsh,je,zlje,zfje,cxjzfje,sflb,sfxmdj,qezfbz,zlbl,
                                                     xj,bz,grbh,xm,kh,cfh,ybcfh,yyxmdm,yyxmmc,ybjzlsh,
                                                     dqjbbz,dj,sl,ybjzlsh) values(
                                                    '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}',
                                                    '{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}',
                                                    '{20}','{21}','{22}','{23}')"
                                                   , jzlsh, jylsh, je, 0, 0, 0, sflb, sfxmdj, "", 0,
                                                    0, "", grbh, xm, kh, jzlsh, ybcfh, yyxmdm, yyxmmc, ybjzlsh,
                                                    2, dj, sl, ybjzlsh);

                    lizysfdjfh.Add(strSql);
                }


                strSql = string.Format("update zy03d set z3ybup = '{0}' where z3ybup is null and z3zyno = '{1}'", jylsh, jzlsh);
                lizysfdjfh.Add(strSql);
                strSql = string.Format("update zy03dz set z3ybup = '{0}' where z3ybup is null and z3zyno = '{1}'", jylsh, jzlsh);
                lizysfdjfh.Add(strSql);

                obj_ins = lizysfdjfh.ToArray();

                obj_ins = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj_ins);

                if (obj_ins[1].ToString() == "1")
                {
                    WriteLog("费用上传成功|本地数据操作成功|");
                    return true;
                }
                else
                {
                    WriteLog("费用上传成功|本地数据操作失败|" + obj_ins[2].ToString());
                    return false;
                }

                return true;
            }
            catch (Exception err)
            {
                WriteLog("费用上传成功|系统异常|" + err.ToString());
                return false;
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
