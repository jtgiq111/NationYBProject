using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Srvtools;
using System.Net.NetworkInformation;
using System.Data;
using System.Windows.Forms;

//南京汤山地区医保接口文件
namespace ybinterface_lib
{
    public class yb_interface_ts_dr
    {
        #region 加载DLL
        [DllImport("SiInterface.dll", EntryPoint = "INIT", CharSet = CharSet.Ansi)]
        static extern int INIT(StringBuilder pErrMsg);

        [DllImport("SiInterface.dll", EntryPoint = "BUSINESS_HANDLE", CharSet = CharSet.Ansi)]
        static extern int BUSINESS_HANDLE(StringBuilder inputData, StringBuilder outputData);
        #endregion

        #region 变量
        internal static string YWBH = string.Empty; //业务编号
        internal static string CZYBH = string.Empty;//操作员工号
        internal static string YWZQH = "1";//业务周期号
        internal static string JYLSH = string.Empty;//交易流水号
        internal static string JRFS = "0"; //接入方式
        internal static string DKLX = "0"; //读卡类型 


        static IWork iWork = new Work();
        static string xmlPath = AppDomain.CurrentDomain.BaseDirectory;
        static List<Item1> lItem = iWork.getXmlConfig1(xmlPath + "EEPNetClient.exe.config");
        internal static string YBIP = lItem[0].YBIP;        //医保IP地址
        internal static string YLGHBH = lItem[0].DDYLJGBH;  //医疗机构编号
        internal static string DDYLJGBH = lItem[0].DDYLJGBH;//医疗机构编号
        internal static string DDYLJGMC = lItem[0].DDYLJGMC;//医院名称
        internal static string YLJGGJM = lItem[0].YLJGGJM;  //医疗机构国家码
        #endregion

        #region 初始化
        public static object[] YBINIT(object[] objParam)
        {
            CZYBH = CliUtils.fLoginUser;  //用户工号
            string sysdate = GetServerDateTime();

            if (string.IsNullOrEmpty(CZYBH))
                return new object[] { 0, 0, "用户工号不能为空" };

            //Ping医保网
            Ping ping = new Ping();
            PingReply pingReply = ping.Send(YBIP);
            if (pingReply.Status != IPStatus.Success)
                return new object[] { 0, 0, "未连接医保网" };

            YWBH = "9100";
            JYLSH = DateTime.Now.ToString("yyyyMMddHHmmss") + "-" + YLGHBH + "-" + new Random().Next(100).ToString().PadLeft(4, '0');

            //入参
            StringBuilder inputData = new StringBuilder();
            inputData.Append(YWBH + "^");
            inputData.Append(YLGHBH + "^");
            inputData.Append(CZYBH + "^");
            inputData.Append(YWZQH + "^");
            inputData.Append(JYLSH + "^");
            inputData.Append(JRFS + "^");
            inputData.Append(DKLX + "^");
            inputData.Append("^");

            WriteLog(sysdate + "  用户" + CZYBH + " 进入医保初始化...");
            WriteLog(sysdate+"  入参" + inputData.ToString());
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
                YWZQH = outputData.ToString().Split('^')[1].TrimEnd('|');
                WriteLog(sysdate + "  用户" + CZYBH + " 医保签到成功|" + outputData.ToString());
              
                CliUtils.fLoginYbNo.Split('|')[0] = YWZQH;
                return new object[] { 0, 1, YWZQH };
            }
            else
            {
                WriteLog(sysdate + "  用户" + CZYBH + " 医保签到失败|" + outputData.ToString());
                return new object[] { 0, 0, outputData.ToString().Split('^')[2].ToString() };
            }
        }
        #endregion

        #region 退出
        public static object[] YBEXIT(object[] objParam)
        {
            string sysdate = GetServerDateTime();
            CZYBH = CliUtils.fLoginUser; //操作员工号
            YWZQH = CliUtils.fLoginYbNo.Split('|')[0];    //业务周期号

            JYLSH = DateTime.Now.ToString("yyyyMMddHHmmss") + "-" + YLGHBH + "-" + new Random().Next(100).ToString().PadLeft(4, '0');
            YWBH = "9110";  //业务编号

            //入参
            StringBuilder inputData = new StringBuilder();
            inputData.Append(YWBH + "^");
            inputData.Append(YLGHBH + "^");
            inputData.Append(CZYBH + "^");
            inputData.Append(YWZQH + "^");
            inputData.Append(JYLSH + "^");
            inputData.Append(JRFS + "^");
            inputData.Append(DKLX + "^");
            inputData.Append("^");

            StringBuilder outputData = new StringBuilder(1024);

            WriteLog(sysdate + "  用户" + CZYBH + " 进入医保退出...");
            WriteLog(sysdate + "  入参|" + inputData.ToString());
            int i = BUSINESS_HANDLE(inputData, outputData);
            if (i == 0)
            {
                WriteLog(sysdate + "  用户" + CZYBH + " 进入医保退出成功|");
                return new object[] { 0, 1, "医保退出成功|" };
            }
            else
            {
                WriteLog(sysdate + "  用户" + CZYBH + " 进入医保退出失败|" + outputData.ToString());
                return new object[] { 0, 0, "医保退出失败|" + outputData.ToString().Split('^')[2].ToString() };
            }
        }
        #endregion

        #region 医保门诊读卡
        public static object[] YBMZDK(object[] objParam)
        {
            try
            {
                string sysdate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                CZYBH = CliUtils.fLoginUser;
                YWZQH = CliUtils.fLoginYbNo.Split('|')[0];
                JYLSH = DateTime.Now.ToString("yyyyMMddHHmmss") + "-" + YLGHBH + "-" + new Random().Next(100).ToString().PadLeft(4, '0');
                YWBH = "2100";  //业务编号
                

                //入参
                StringBuilder inputData = new StringBuilder();
                inputData.Append(YWBH + "^");
                inputData.Append(YLGHBH + "^");
                inputData.Append(CZYBH + "^");
                inputData.Append(YWZQH + "^");
                inputData.Append(JYLSH + "^");
                inputData.Append(JRFS + "^");
                inputData.Append(DKLX + "^");
                inputData.Append("^");

                StringBuilder outputData = new StringBuilder(10240);

                WriteLog(sysdate + "  进入门诊读卡...");
                WriteLog(sysdate + "  入参|" + inputData.ToString());
                int i = BUSINESS_HANDLE(inputData, outputData);
                if (i == 0)
                {
                    List<string> liSQL = new List<string>();
                    WriteLog(sysdate + "  门诊读卡返回参数|" + outputData.ToString());
                    //1000677037|10094171|南京陆拾度温泉酒店有限公司|320121197509114128|孙志花|2|11|0|320115|1252.7|0|0|1|||0||0||0||0||0||0||0|||0||0||0|||0||0|||1|||0|0|0|||0|
                    #region 出参赋值
                    string[] sValue = outputData.ToString().Split('^')[1].Split('|');
                    if (sValue.Length < 20)
                        return new object[] { 0, 0, "读卡返回参数有误" };
                    string ybkh = sValue[0]; //社会保障卡号
                    string dwbh = sValue[1]; //单位编号
                    string dwmc = sValue[2]; //单位名称
                    string sfzh = sValue[3]; //身份证号
                    string xm = sValue[4];   //姓名
                    string xb = sValue[5];   //性别
                    string ylrylb = sValue[6];   //医疗人员类别
                    string ydrybz = sValue[7];   //异地人员标志
                    string tcqh = sValue[8];     //统筹区号
                    string dqzhye = sValue[9];   //当前帐户余额
                    string zyzt = sValue[10];     //在院状态
                    string bnzycs = sValue[11];   //本年住院次数
                    string dyssbz = sValue[12];    //待遇享受标志
                    string dybssyy = sValue[13];   //待遇不享受原因
                    string bzdjqk = sValue[14];    //病种登记情况
                    string ybmmzg = sValue[15];    //医保门慢资格
                    string ybmmbz = sValue[16];    //医保门慢病种
                    string ybmjzg = sValue[17];    //医保门精资格
                    string ybmjbz = sValue[18];    //医保门精病种
                    string ybmizg = sValue[19];    //医保门艾资格
                    string ybmibz = sValue[20];    //医保门艾病种
                    string ybjgglszg = sValue[21]; //医保丙肝干扰素资格
                    string ybjgglsbz = sValue[22]; //医保丙肝干扰素病种
                    string ybmzxybzg = sValue[23];  //医保门诊血友病资格
                    string ybmzxybbz = sValue[24];  //医保门诊血友病病种
                    string ybmtzg = sValue[25];    //医保门特资格
                    string ybmtbz = sValue[26];    //医保门特病种
                    string ybtyzg = sValue[27];    //医保特药资格
                    string ybtybz = sValue[28];    //医保特药病种
                    string ybtyypmcbm = sValue[29];    //医保特药名称编码
                    string jmmdzg = sValue[30];    //居民门大资格
                    string jmmdbz = sValue[31];         //居民门大病种
                    string jmmzxybzg = sValue[32];     //居民门诊血友病资格
                    string jmmzxybbz = sValue[33];     //居民门诊血友病病种
                    string jmtyzg = sValue[34];    //居民特药资格
                    string jmtybz = sValue[35];    //居民特药病种
                    string jmtymcbm = sValue[36];  //居民特药名称编码
                    string nmgmdzg = sValue[37];   //农民工门大资格
                    string nmgmdbz = sValue[38];   //农民工门大病种
                    string nmgtyzg = sValue[39];   //农民工特药资格
                    string nmgtybz = sValue[40];   //农民工特药病种
                    string nmgtymcbm = sValue[41];     //农民工特药名称编码
                    string nfsszgmztc = sValue[42];    //能否享受门诊统筹
                    string sysplx = sValue[43];        //生育审批类型
                    string fsyy = sValue[44];          //封锁原因
                    string mmsykbje = sValue[45];      //门慢剩余可报金额
                    string mtpzzlsykbje = sValue[46];  //门特辅助治疗剩余可报金额
                    string gsdyzg = sValue[47];        //工伤待遇资格
                    string gsdybz = sValue[48];        //工伤待遇病种
                    string gszdjr = sValue[49];        //工伤诊断结论
                    string dksykbje = sValue[50];      //大卡剩余可报金额
                    string mtsykbje = sValue[51];      //门统剩余可报金额
                    string ybjczg = sValue[52];        //医保家床资格
                    #endregion

                    #region 出参返回

                    string brithday = "";
                    string age = "";
                    if (sfzh.Length == 18)
                    {
                        brithday = sfzh.Substring(6, 8).Insert(6, "-").Insert(4, "-");
                        age = (DateTime.Now.Year - Convert.ToDateTime(brithday).Year).ToString();
                    }
                    else if (sfzh.Length == 15)
                    {
                        brithday = "19"+sfzh.Substring(6, 6).Insert(4, "-").Insert(2, "-");
                        age = (DateTime.Now.Year - Convert.ToDateTime(brithday).Year).ToString();
                    }

                    string jflx=string.Empty;
                    string[] sV={"1","2","3"};
                    if (sV.Contains(ylrylb.Substring(0, 1)))
                        jflx="0002"; //职工医保
                    else
                        jflx="0001"; //居民医保

                    string mtbbz = "0"; //门慢特病资质

                    string mtbMsg = "";

                    //医保门慢资质
                    if (ybmmzg.Equals("1"))
                    {
                        mtbbz = "1";
                        mtbMsg+="享受医保门慢资格\r\n";
                        string[] sBZBM = bzdjqk.Split(',');
                        foreach (string sbz in sBZBM)
                        {
                            string strSql1 = string.Format("select dmmc from ybbzmrdr where dm='{0}'", sbz);
                            DataSet ds1 = CliUtils.ExecuteSql("sybdj", "cmd", strSql1, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                            if (ds1.Tables[0].Rows.Count > 0)
                            {
                                mtbMsg += "病种编码:" + sbz + " 病种名称:" + ds1.Tables[0].Rows[0]["dmmc"].ToString()+"&";
                            }
                        }
                        mtbMsg += "\r\n";
                    }

                    //医保门特资格
                    if (ybmtzg.Equals("1"))
                    {
                        mtbbz = "1";
                        mtbMsg += "享受医保门特资格\r\n";
                        string[] sBZBM = ybmmbz.Split(',');
                        foreach (string sbz in sBZBM)
                        {
                            string strSql1 = string.Format("select dmmc from ybbzmrdr where dm='{0}'", sbz);
                            DataSet ds1 = CliUtils.ExecuteSql("sybdj", "cmd", strSql1, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                            if (ds1.Tables[0].Rows.Count > 0)
                            {
                                mtbMsg += "病种编码:" + sbz + " 病种名称:" + ds1.Tables[0].Rows[0]["dmmc"].ToString() + "|";
                            }
                        }
                        mtbMsg += "\r\n";
                    }


                    /*个人编号|单位编号|身份证号|姓名|性别|
                     * 民族|出生日期|社会保障卡卡号|医疗待遇类别|人员参保状态|
                     * 异地人员标志|统筹区号|年度|在院状态|帐户余额|
                     * 本年医疗费累计|本年帐户支出累计|本年统筹支出累计|本年救助金支出累计|本年公务员补助基金累计|
                     * 本年城镇居民门诊统筹支付累计|进入统筹费用累计|进入救助金费用累计|起付标准累计|本年住院次数|
                     * 单位名称|年龄|参保单位类型|经办机构编码|缴费类型|
                     * 医保门慢、特资质|医保门慢、特病种说明
                     */
                    string strParam = ybkh + "|" + dwbh + "|" + sfzh + "|" + xm + "|" + xb +
                                      "|01|" + brithday + "|" + ybkh + "|" + ylrylb + "|" + dyssbz + "|" +
                                      ydrybz + "|" + tcqh + "||" + zyzt + "|" + dqzhye +
                                      "||" + "||||||||" + bnzycs + "|" +
                                      dwmc + "|" + age + "||" + YLGHBH + "|" + jflx + "|" +
                                      mtbbz + "|" + mtbMsg;
                    #endregion

                    string strSql = string.Format(@"delete from ybickxx where grbh='{0}'", ybkh);
                    liSQL.Add(strSql);
                    strSql = string.Format(@"insert into YBICKXX(
                                            GRBH,DWBH,DWMC,GMSFHM,XM,XB,YLRYLB,YDRYBZ,DQBH,GRZHYE,
                                            ZYKT,ZYCS,DYSSBZ,DYBSSYY,BQDJQK,YBMMZG,YBMMBZ,YBMJZG,YBMJBZ,YBMIZG,
                                            YBMIBZ,YBJGGLSZG,YBJGGLSBZ,YBMZXYBZG,YBMZXYBBZ,YBMTZG,YBMTBZ,YBTYZG,YBTYBZ,YBTYMCBM,
                                            JMMDZG,JMMDBZ,JMMZXYBZG,JMMZXYBBZ,JMTYZG,JMTYBZ,JMTYMCBM,NMGMDZG,NMGMDBZ,NMGTYZG,
                                            NMGTYBZ,NMGTYMCBM,NFSSZGMZTC,SYSPLX,MMSYKBJE,MTPZZLSYKBJE,FSYY,MTSYKBJE,SYSDATE) VALUES(
                                            '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}',
                                            '{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}',
                                            '{20}','{21}','{22}','{23}','{24}','{25}','{26}','{27}','{28}','{29}',
                                            '{30}','{31}','{32}','{33}','{34}','{35}','{36}','{37}','{38}','{39}',
                                            '{40}','{41}','{42}','{43}','{44}','{45}','{46}','{47}','{48}')",
                                             ybkh, dwbh, dwmc, sfzh, xm, xb, ylrylb, ydrybz, tcqh, dqzhye,
                                             zyzt, bnzycs, dyssbz, dybssyy, bzdjqk, ybmmzg, ybmmbz, ybmjzg, ybmjbz, ybmizg,
                                             ybmibz, ybjgglszg, ybjgglsbz, ybmzxybzg, ybmzxybbz, ybmtzg, ybmtbz, ybtyzg, ybtybz, ybtyypmcbm,
                                             jmmdzg, jmmdbz, jmmzxybzg, jmmzxybbz, jmtyzg, jmtybz, jmtymcbm, nmgmdzg, nmgmdbz, nmgtyzg,
                                             nmgtybz, nmgtymcbm, nfsszgmztc, sysplx, mmsykbje, mtpzzlsykbje, fsyy, mtsykbje, sysdate);
                    liSQL.Add(strSql);

                    object[] obj = liSQL.ToArray();
                    obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);
                    if (obj[1].ToString().Equals("1"))
                    {
                        WriteLog(sysdate + "  读卡信息成功|" + strParam);
                        return new object[] { 0, 1, strParam };
                    }
                    else
                    {
                        WriteLog(sysdate + "  保存读卡信息失败|" + obj[2].ToString());
                        return new object[] { 0, 0, obj[2].ToString() };
                    }
                }
                else
                {
                    WriteLog(sysdate + "  进入门诊读卡失败|" + outputData.ToString());
                    return new object[] { 0, 0, outputData.ToString().Split('^')[2].ToString() };
                }
            }
            catch
            {
                return new object[] { 0, 0, "请插入医保卡或医保读卡出错｜" };
            }
        }
        #endregion

        #region 医保住院读卡
        public static object[] YBZYDK(object[] objParam)
        {
            try
            {
                string sysdate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                CZYBH = CliUtils.fLoginUser;
                YWZQH = CliUtils.fLoginYbNo.Split('|')[0];
                JYLSH = DateTime.Now.ToString("yyyyMMddHHmmss") + "-" + YLGHBH + "-" + new Random().Next(100).ToString().PadLeft(4, '0');
                YWBH = "2100";  //业务编号

                //入参
                StringBuilder inputData = new StringBuilder();
                inputData.Append(YWBH + "^");
                inputData.Append(YLGHBH + "^");
                inputData.Append(CZYBH + "^");
                inputData.Append(YWZQH + "^");
                inputData.Append(JYLSH + "^");
                inputData.Append(JRFS + "^");
                inputData.Append(DKLX + "^");
                inputData.Append("^");

                StringBuilder outputData = new StringBuilder(10240);

                WriteLog(sysdate + "  进入门诊读卡...");
                WriteLog(sysdate + "  入参|" + inputData.ToString());
                int i = BUSINESS_HANDLE(inputData, outputData);
                if (i == 0)
                {
                    List<string> liSQL = new List<string>();
                    WriteLog(sysdate + "  门诊读卡返回参数|" + outputData.ToString());
                    //1000677037|10094171|南京陆拾度温泉酒店有限公司|320121197509114128|孙志花|2|11|0|320115|1252.7|0|0|1|||0||0||0||0||0||0||0|||0||0||0|||0||0|||1|||0|0|0|||0|
                    #region 出参赋值
                    string[] sValue = outputData.ToString().Split('^')[1].Split('|');
                    if (sValue.Length < 20)
                        return new object[] { 0, 0, "读卡返回参数有误" };
                    string ybkh = sValue[0]; //社会保障卡号
                    string dwbh = sValue[1]; //单位编号
                    string dwmc = sValue[2]; //单位名称
                    string sfzh = sValue[3]; //身份证号
                    string xm = sValue[4];   //姓名
                    string xb = sValue[5];   //性别
                    string ylrylb = sValue[6];   //医疗人员类别
                    string ydrybz = sValue[7];   //异地人员标志
                    string tcqh = sValue[8];     //统筹区号
                    string dqzhye = sValue[9];   //当前帐户余额
                    string zyzt = sValue[10];     //在院状态
                    string bnzycs = sValue[11];   //本年住院次数
                    string dyssbz = sValue[12];    //待遇享受标志
                    string dybssyy = sValue[13];   //待遇不享受原因
                    string bzdjqk = sValue[14];    //病种登记情况
                    string ybmmzg = sValue[15];    //医保门慢资格
                    string ybmmbz = sValue[16];    //医保门慢病种
                    string ybmjzg = sValue[17];    //医保门精资格
                    string ybmjbz = sValue[18];    //医保门精病种
                    string ybmizg = sValue[19];    //医保门艾资格
                    string ybmibz = sValue[20];    //医保门艾病种
                    string ybjgglszg = sValue[21]; //医保丙肝干扰素资格
                    string ybjgglsbz = sValue[22]; //医保丙肝干扰素病种
                    string ybmzxybzg = sValue[23];  //医保门诊血友病资格
                    string ybmzxybbz = sValue[24];  //医保门诊血友病病种
                    string ybmtzg = sValue[25];    //医保门特资格
                    string ybmtbz = sValue[26];    //医保门特病种
                    string ybtyzg = sValue[27];    //医保特药资格
                    string ybtybz = sValue[28];    //医保特药病种
                    string ybtyypmcbm = sValue[29];    //医保特药名称编码
                    string jmmdzg = sValue[30];    //居民门大资格
                    string jmmdbz = sValue[31];    //居民门大病种
                    string jmmzxybzg = sValue[32];     //居民门诊血友病资格
                    string jmmzxybbz = sValue[33];     //居民门诊血友病病种
                    string jmtyzg = sValue[34];    //居民特药资格
                    string jmtybz = sValue[35];    //居民特药病种
                    string jmtymcbm = sValue[36];  //居民特药名称编码
                    string nmgmdzg = sValue[37];   //农民工门大资格
                    string nmgmdbz = sValue[38];   //农民工门大病种
                    string nmgtyzg = sValue[39];   //农民工特药资格
                    string nmgtybz = sValue[40];   //农民工特药病种
                    string nmgtymcbm = sValue[41];     //农民工特药名称编码
                    string nfsszgmztc = sValue[42];    //能否享受门诊统筹
                    string sysplx = sValue[43];    //生育审批类型
                    string fsyy = sValue[44];       //封锁原因
                    string mmsykbje = sValue[45];  //门慢剩余可报金额
                    string mtpzzlsykbje = sValue[46];  //门特辅助治疗剩余可报金额
                    string gsdyzg = sValue[47];  //工伤待遇资格
                    string gsdybz = sValue[48];  //工伤待遇病种
                    string gszdjr = sValue[49];  //工伤诊断结论
                    string dksykbje = sValue[50];  //大卡剩余可报金额
                    string mtsykbje = sValue[51];   //门统剩余可报金额
                    string ybjczg = sValue[52];  //医保家床资格
                    #endregion

                    #region 出参返回

                    string brithday = "";
                    string age = "";
                    if (sfzh.Length == 18)
                    {
                        brithday = sfzh.Substring(6, 8).Insert(6, "-").Insert(4, "-");
                        age = (DateTime.Now.Year - Convert.ToDateTime(brithday).Year).ToString();
                    }
                    else if (sfzh.Length == 15)
                    {
                        brithday = "19"+sfzh.Substring(6, 6).Insert(4, "-").Insert(2, "-");
                        age = (DateTime.Now.Year - Convert.ToDateTime(brithday).Year).ToString();
                    }

                    /*个人编号|单位编号|身份证号|姓名|性别|
                     * 民族|出生日期|社会保障卡卡号|医疗待遇类别|人员参保状态|
                     * 异地人员标志|统筹区号|年度|在院状态|帐户余额|
                     * 本年医疗费累计|本年帐户支出累计|本年统筹支出累计|本年救助金支出累计|本年公务员补助基金累计|
                     * 本年城镇居民门诊统筹支付累计|进入统筹费用累计|进入救助金费用累计|起付标准累计|本年住院次数|
                     * 单位名称|年龄|参保单位类型|经办机构编码|缴费类型|
                     * 医保门慢、特资质|医保门慢、特病种说明｜
                     */

                    string jflx = string.Empty;
                    string[] sV = { "1", "2", "3" };
                    if (sValue.Contains(ylrylb.Substring(0, 1)))
                        jflx = "0002"; //职工医保
                    else
                        jflx = "0001"; //居民医保


                    string mtbbz = "0"; //门慢特病资质

                    string mtbMsg = "";

                    //医保门慢资质
                    if (ybmmzg.Equals("1"))
                    {
                        mtbbz = "1";
                        mtbMsg += "享受医保门慢资格\r\n";
                        string[] sBZBM = ybmmbz.Split(',');
                        foreach (string sbz in sBZBM)
                        {
                            string strSql1 = string.Format("select dmmc from ybbzmrdr where dm='{0}'", sbz);
                            DataSet ds1 = CliUtils.ExecuteSql("sybdj", "cmd", strSql1, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                            if (ds1.Tables[0].Rows.Count > 0)
                            {
                                mtbMsg += "病种编码:" + sbz + " 病种名称:" + ds1.Tables[0].Rows[0]["dmmc"].ToString() + "|";
                            }
                        }
                        mtbMsg += "\r\n";
                    }

                    //医保门特资格
                    if (ybmtzg.Equals("1"))
                    {
                        mtbbz = "1";
                        mtbMsg += "享受医保门特资格\r\n";
                        string[] sBZBM = ybmmbz.Split(',');
                        foreach (string sbz in sBZBM)
                        {
                            string strSql1 = string.Format("select dmmc from ybbzmrdr where dm='{0}'", sbz);
                            DataSet ds1 = CliUtils.ExecuteSql("sybdj", "cmd", strSql1, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                            if (ds1.Tables[0].Rows.Count > 0)
                            {
                                mtbMsg += "病种编码:" + sbz + " 病种名称:" + ds1.Tables[0].Rows[0]["dmmc"].ToString() + "|";
                            }
                        }
                        mtbMsg += "\r\n";
                    }

                    string strParam = ybkh + "|" + dwbh + "|" + sfzh + "|" + xm + "|" + xb +
                                      "|01|" + brithday + "|" + ybkh + "|" + ylrylb + "|" + dyssbz + "|" +
                                      ydrybz + "|" + tcqh + "||" + zyzt + "|" + dqzhye +
                                      "||" + "||||||||" + bnzycs + "|" +
                                      dwmc + "|" + age + "||" + YLGHBH + "|" + jflx + "|" + 
                                      mtbbz + "|" + mtbMsg;
                    #endregion

                    string strSql = string.Format(@"delete from ybickxx where grbh='{0}'", ybkh);
                    liSQL.Add(strSql);
                    strSql = string.Format(@"insert into YBICKXX(
                                            GRBH,DWBH,DWMC,GMSFHM,XM,XB,YLRYLB,YDRYBZ,DQBH,GRZHYE,
                                            ZYKT,ZYCS,DYSSBZ,DYBSSYY,BQDJQK,YBMMZG,YBMMBZ,YBMJZG,YBMJBZ,YBMIZG,
                                            YBMIBZ,YBJGGLSZG,YBJGGLSBZ,YBMZXYBZG,YBMZXYBBZ,YBMTZG,YBMTBZ,YBTYZG,YBTYBZ,YBTYMCBM,
                                            JMMDZG,JMMDBZ,JMMZXYBZG,JMMZXYBBZ,JMTYZG,JMTYBZ,JMTYMCBM,NMGMDZG,NMGMDBZ,NMGTYZG,
                                            NMGTYBZ,NMGTYMCBM,NFSSZGMZTC,SYSPLX,MMSYKBJE,MTPZZLSYKBJE,FSYY,MTSYKBJE,SYSDATE) VALUES(
                                            '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}',
                                            '{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}',
                                            '{20}','{21}','{22}','{23}','{24}','{25}','{26}','{27}','{28}','{29}',
                                            '{30}','{31}','{32}','{33}','{34}','{35}','{36}','{37}','{38}','{39}',
                                            '{40}','{41}','{42}','{43}','{44}','{45}','{46}','{47}','{48}')",
                                             ybkh, dwbh, dwmc, sfzh, xm, xb, ylrylb, ydrybz, tcqh, dqzhye,
                                             zyzt, bnzycs, dyssbz, dybssyy, bzdjqk, ybmmzg, ybmmbz, ybmjzg, ybmjbz, ybmizg,
                                             ybmibz, ybjgglszg, ybjgglsbz, ybmzxybzg, ybmzxybbz, ybmtzg, ybmtbz, ybtyzg, ybtybz, ybtyypmcbm,
                                             jmmdzg, jmmdbz, jmmzxybzg, jmmzxybbz, jmtyzg, jmtybz, jmtymcbm, nmgmdzg, nmgmdbz, nmgtyzg,
                                             nmgtybz, nmgtymcbm, nfsszgmztc, sysplx, mmsykbje, mtpzzlsykbje, fsyy, mtsykbje, sysdate);
                    liSQL.Add(strSql);

                    object[] obj = liSQL.ToArray();
                    obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);
                    if (obj[1].ToString().Equals("1"))
                    {
                        WriteLog(sysdate + "  读卡信息成功|" + strParam);
                        return new object[] { 0, 1, strParam };
                    }
                    else
                    {
                        WriteLog(sysdate + "  保存读卡信息失败|" + obj[2].ToString());
                        return new object[] { 0, 0, obj[2].ToString() };
                    }
                }
                else
                {
                    WriteLog(sysdate + "  进入门诊读卡失败|" + outputData.ToString());
                    return new object[] { 0, 0, outputData.ToString().Split('^')[2].ToString() };
                }
            }
            catch
            {
                return new object[] { 0, 0, "请插入医保卡或医保读卡出错｜" };
            }
        }
        #endregion

        #region 门诊登记(挂号)收费
        public static object[] YBMZDJSF(object[] objParam)
        {
            string sysdate = GetServerDateTime();
            string jzlsh = objParam[0].ToString();//就诊流水号
            string yllb = objParam[1].ToString();//医疗类别代码
            string ghsj = objParam[2].ToString();//挂号时间
            string bzbm = objParam[3].ToString();//病种编码
            string bzmc = objParam[4].ToString();//病种名称
            string dkxx = objParam[5].ToString(); //读卡信息

            string ghdjsj = Convert.ToDateTime(objParam[2]).ToString("yyyyMMddHHmmss");//挂号时间
            
            WriteLog(sysdate + "进入门诊登记收费...");
            WriteLog(sysdate + "  入参|" + jzlsh + "|" + yllb + "|" + ghsj + "|" + bzbm + "|" + bzmc + "|" + dkxx + "|");
            //医保门诊登记
            object[] objDJ = { jzlsh, yllb, bzbm, bzmc, dkxx, ghsj };
            objDJ = YBMZDJ(objDJ);
            if (objDJ[1].ToString().Equals("1"))
            {
                object[] objGHFYDJ = { jzlsh };
                //医保挂号费用登记
                WriteLog(sysdate + "进入门诊费用登记...");
                objGHFYDJ = YBMZGHFDJ(objGHFYDJ);
                if (objGHFYDJ[1].ToString().Equals("1"))
                {
                    string djh = string.Empty;
                    string cfjylsh = objGHFYDJ[2].ToString(); //处方交易流水号 
                    #region 如果是挂号登记,则获取发票号
                    string strSql = string.Format(@"select m1invo,* from mz01h where m1ghno='{0}'", jzlsh);
                    DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                    if (ds.Tables[0].Rows.Count == 0)
                        return new object[] { 0, 0, "该患者未挂号" };
                    djh = ds.Tables[0].Rows[0]["m1invo"].ToString();
                    #endregion

                    //医保门诊费用结算
                    object[] objMZFYJS = { jzlsh, djh, "", ghsj, "", "", "", yllb, "", 1, cfjylsh };
                    objMZFYJS = YBMZFYJS_GH(objMZFYJS);

                    if (objMZFYJS[1].ToString().Equals("1"))
                        return new object[] { 0, 1, objMZFYJS[2].ToString() };
                    else
                        return new object[] { 0, 0, objMZFYJS[2].ToString() };
                }
                else
                {
                    object[] objMZDJCX = { jzlsh};
                    YBMZDJSFCX(objMZDJCX);
                    return new object[] { 0, 0, objGHFYDJ[2].ToString() };
                }
            }
            else
                return new object[] { 0, 0, objDJ[2].ToString() };
            
        }
        #endregion

        #region 门诊登记(挂号)费用结算
        public static object[] YBMZFYJS_GH(object[] objParam)
        {
            string sysdate = GetServerDateTime();//系统时间
            string jzlsh = objParam[0].ToString();      // 就诊流水号
            string djh = objParam[1].ToString();        // 单据号
            string zhsybz = objParam[2].ToString();     // 账户使用标志（0或1） 
            string dqrq = objParam[3].ToString();  // 结算时间
            string bzbm = objParam[4].ToString(); //病种编码
            string bzmc = objParam[5].ToString(); //病种名称
            string cfhs = objParam[6].ToString();   //处方号集
            string yllb = objParam[7].ToString(); //医疗类别
            string ylfhj1 = objParam[8].ToString(); //医疗费合计 (新增)
            string sfghjs = objParam[9].ToString(); //是否挂号费结算
            string cfjylsh = objParam[10].ToString(); //处方交易流水号
            if (string.IsNullOrEmpty(jzlsh))
                return new object[] { 0, 0, "就诊流水号不能为空" };
            if (string.IsNullOrEmpty(djh))
                return new object[] { 0, 0, "单据号不能为空" };
            //if (string.IsNullOrEmpty(bzbm))
            //    return new object[] { 0, 0, "病种不能为空" };

            YWBH = "2410";
            JYLSH = DateTime.Now.ToString("yyyyMMddHHmmss") + "-" + YLGHBH + "-" + new Random().Next(100).ToString().PadLeft(4, '0');
            CZYBH = CliUtils.fLoginUser;
            YWZQH = CliUtils.fLoginYbNo.Split('|')[0];

            #region 获取变量信息
            string strSql = string.Format(@"select * from ybmzzydjdr where jzlsh='{0}' and cxbz=1", jzlsh);
            DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            if (ds.Tables[0].Rows.Count == 0)
                return new object[] { 0, 0, "该患者未医保门诊登记" };
            yllb = ds.Tables[0].Rows[0]["yllb"].ToString(); //医疗类别
            string kh = ds.Tables[0].Rows[0]["kh"].ToString();  //社会保障卡号
            string ksbm = ds.Tables[0].Rows[0]["ksbh"].ToString();  //科室编码
            string xm = ds.Tables[0].Rows[0]["xm"].ToString();  //姓名
            string grbh = ds.Tables[0].Rows[0]["grbh"].ToString();  //医保编号
            string ybjzlsh = ds.Tables[0].Rows[0]["ybjzlsh"].ToString(); //医保就诊流水号

            string[] syllb = { "11", "13", "86" };
            if (!syllb.Contains(yllb))
            {
                bzbm = ds.Tables[0].Rows[0]["bzbm"].ToString();  //病种编码
                bzmc = ds.Tables[0].Rows[0]["bzmc"].ToString();     //病种名称
            }

            string jsrq = Convert.ToDateTime(sysdate).ToString("yyyyMMddHHmmss");   //结算日期
            string cyrq = Convert.ToDateTime(dqrq).ToString("yyyyMMddHHmmss");  //出院日期
            string cyyy = "";       //出院原因
            string cyzdbm = bzbm;     //出院诊断疾病编码
            string yjslb = "";      //月结算类别
            string ztjsbz = "0";     //中途结算标志
            string jbr = CliUtils.fUserName;    //经办人
            string jbrbh = CliUtils.fLoginUser; //经办人编码
            string fmrq = "";   //分娩日期
            string cs = "";     //产次
            string tes = "";    //胎儿数
            string zybh = "";   //转院医院编号
            string zsekh = "";  //准生儿社会保障卡号
            string ssbz = "";   //手术是否成功标志
            decimal xjzf_gh = 0;
            string zcfbz = "";
            string ysbm = "";
            #endregion

            //获取处方医生
            strSql = string.Format(@"select distinct m3empn from mz03d where left(m3endv,1)=1 and m3ghno='{0}'", jzlsh);
            ds.Tables.Clear();
            ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            if (sfghjs.Equals("1"))
                ysbm = "";
            else
                ysbm = ds.Tables[0].Rows[0]["m3empn"].ToString();  //医生编码

            #region 获取病历本费、卡费
            //if (sfghjs.Equals("1"))
            //{
            //    zcfbz = "g";
            //    strSql = string.Format(@"select m1blam,m1jzam from mz01h where m1ghno='{0}'", jzlsh);
            //    ds.Tables.Clear();
            //    ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            //    if (ds.Tables[0].Rows.Count == 0)
            //        return new object[] { 0, 0, "无挂号信息" };
            //    decimal blbfy = Convert.ToDecimal(ds.Tables[0].Rows[0]["m1blam"].ToString()); //病历本费
            //    decimal jzkfy = Convert.ToDecimal(ds.Tables[0].Rows[0]["m1jzam"].ToString()); //就诊卡费
            //    xjzf_gh = blbfy + jzkfy;

            //}
            #endregion

            #region 入参赋值
            //入参数据
            StringBuilder inputParam = new StringBuilder();
            inputParam.Append(ybjzlsh + "|");
            inputParam.Append(djh + "|");
            inputParam.Append(yllb + "|");
            inputParam.Append(jsrq + "|");
            inputParam.Append(cyrq + "|");
            inputParam.Append(cyyy + "|");
            inputParam.Append(cyzdbm + "|");
            inputParam.Append(yjslb + "|");
            inputParam.Append(ztjsbz + "|");
            inputParam.Append(jbr + "|");
            inputParam.Append(fmrq + "|");
            inputParam.Append(cs + "|");
            inputParam.Append(tes + "|");
            inputParam.Append(kh + "|");
            inputParam.Append(zybh + "|");
            inputParam.Append(ksbm + "|");
            inputParam.Append(ysbm + "|");
            inputParam.Append(sfghjs + "|");
            inputParam.Append(zsekh + "|");
            inputParam.Append(ssbz + "|");

            //入参
            StringBuilder inputData = new StringBuilder();
            inputData.Append(YWBH + "^");
            inputData.Append(YLGHBH + "^");
            inputData.Append(CZYBH + "^");
            inputData.Append(YWZQH + "^");
            inputData.Append(JYLSH + "^");
            inputData.Append(JRFS + "^");
            inputData.Append(DKLX + "^");
            inputData.Append(inputParam + "^");
            #endregion

            StringBuilder outputData = new StringBuilder(10240);
            WriteLog(sysdate + "  进入门诊登记(挂号)费用结算...");
            WriteLog(sysdate + "  入参|" + inputData.ToString());

            int i = BUSINESS_HANDLE(inputData, outputData);
            if (i == 0)
            {
                WriteLog(sysdate + "  门诊登记(挂号)费用结算成功|" + outputData.ToString());
                #region 出参赋值
                string[] str = outputData.ToString().Split('^')[1].Split('|');
                decimal ylzfy = Convert.ToDecimal(str[0]);         //医疗总费用
                //decimal ylzfy1 = ylzfy + xjzf_gh;
                decimal ylzfy1 = ylzfy;
                decimal zbxje = Convert.ToDecimal(0.00);         //总报销金额
                decimal tcjjzf = Convert.ToDecimal(str[1]);         //统筹基金支付
                decimal dejjzf = Convert.ToDecimal(str[2]);         //本次大病救助支付
                decimal dbbyzf = Convert.ToDecimal(str[3]);         //本次大病保险支付  
                decimal mzfdfy = Convert.ToDecimal(str[4]);         //本次民政补助支付
                decimal zhzf = Convert.ToDecimal(str[5]);          //本次帐户支付总额
                decimal xjzf = Convert.ToDecimal(str[6]);//本次现金支付总额
                //decimal xjzf1 = xjzf + xjzf_gh;
                decimal xjzf1 = xjzf;
                decimal zhzfzf = Convert.ToDecimal(str[7]);          //本次帐户支付自付
                decimal zhzfzl = Convert.ToDecimal(str[8]);          //本次帐户支付自理
                decimal xjzfzf = Convert.ToDecimal(str[9]);          //本次帐户支付自付
                decimal xjzfzl = Convert.ToDecimal(str[10]);          //本次帐户支付自理
                decimal ybfwnfy = Convert.ToDecimal(str[11]);         //医保范围内费用
                decimal bcjsqzhye = Convert.ToDecimal(str[12]) + zhzf;//本次结算前帐户余额
                string dbzbm = str[13];        //单病种病种编码
                string smxx = str[14];          //说明信息
                decimal yfhj = Convert.ToDecimal(str[15]);       //药费合计
                decimal zlxmfhj = Convert.ToDecimal(str[16]);       //诊疗项目费合计
                decimal bbzf = Convert.ToDecimal(str[17]);       //补保支付
                string yllb_r = str[18];//医疗类别
                string by6 = str[19];       //备用6

                decimal gwybzjjzf = Convert.ToDecimal(0.00);     //公务员补助基金支付
                decimal qybcylbxjjzf = Convert.ToDecimal(0.00);  //企业补充医疗保险基金支付
                decimal dwfdfy = Convert.ToDecimal(0.00);        //单位负担费用    
                decimal yyfdfy = Convert.ToDecimal(0.00);       //医院负担费用
                //decimal mzfdfy = Convert.ToDecimal("0.00");       //民政负担费用
                decimal cxjfy = Convert.ToDecimal(0.00);        //超限价费用单病种病种编码
                decimal ylzlfy = Convert.ToDecimal(0.00);       //乙类自理费用
                decimal blzlfy = Convert.ToDecimal(0.00);       //丙类自理费用
                decimal fhjbylfy = Convert.ToDecimal(0.00);     //符合基本医疗费用
                decimal qfbzfy = Convert.ToDecimal(0.00);       //起付标准费用
                decimal zzzyzffy = Convert.ToDecimal(0.00);     //转诊转院自付费用
                decimal jrtcfy = ybfwnfy;       //进入统筹费用
                decimal tcfdzffy = Convert.ToDecimal(0.00);     //统筹分段自付费用
                decimal ctcfdxfy = Convert.ToDecimal(0.00);       //超统筹封顶线费用
                decimal jrdebsfy = Convert.ToDecimal(0.00);       //进入大额报销费用
                decimal defdzffy = Convert.ToDecimal(0.00);       //大额分段自付费用
                decimal cdefdxfy = Convert.ToDecimal(0.00);       //超大额封顶线费用
                decimal rgqgzffy = Convert.ToDecimal(0.00);       //人工器管自付费用
                //decimal bcjsqzfye = Convert.ToDecimal(0.00);       //本次结算前帐户余额
                decimal bntczflj = Convert.ToDecimal(0.00);       //本年统筹支付累计
                decimal bndezflj = Convert.ToDecimal(0.00);       //本年大额支付累计
                decimal bnczjmmztczflj = Convert.ToDecimal(0.00);       //本年城镇居民门诊统筹支付累计
                decimal bngwybzzflj = Convert.ToDecimal(0.00);  //本年公务员补助支付累计(不含本次)
                decimal bnzhzflj = Convert.ToDecimal(0.00);  //本年账户支付累计(不含本次)
                string bnzycslj = "1";  //本年住院次数累计(不含本次)
                string zycs = "1";        //住院次数
                string yldylb = ""; //医疗待遇类别
                string jbjgbm = ""; //经办机构编码
                #endregion

                zbxje = ylzfy - xjzf;

                //本次医疗费总额=本次统筹支付金额+本次大病救助支付+本次大病保险支付+本次民政补助支付+本次帐户支付总额+本次现金支付总额+本次补保支付总额

                /*
                     *医疗费总额|总报销金额|统筹基金支付|大额基金支付|账户支付|
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
                     * 居民个人自付二次补偿金额|体检金额|生育基金支付|本次民政补助支付|医保范围内费用|
                     */

                string strValue = ylzfy1 + "|" + zbxje + "|" + tcjjzf + "|" + dejjzf + "|" + zhzf + "|" + 
                    xjzf1 + "|" + gwybzjjzf + "|" + qybcylbxjjzf + "|" + "0.00" + "|" + dwfdfy + "|" +
                  yyfdfy + "|" + mzfdfy + "|" + cxjfy + "|" + ylzlfy + "|" + blzlfy + "|" + 
                  fhjbylfy + "|" + qfbzfy + "|" + zzzyzffy + "|" + jrtcfy + "|" + tcfdzffy + "|" +
                  ctcfdxfy + "|" + "0.00" + "|" + defdzffy + "|" + cdefdxfy + "|" + rgqgzffy + "|" +
                  bcjsqzhye + "|" + bntczflj + "|" + bndezflj + "|" + bnczjmmztczflj + "|" + bngwybzzflj + "|" +
                                  bnzhzflj + "|" + bnzycslj + "|" + zycs + "|" + xm + "|" + jsrq + "|" +
                                  yllb_r + "||" + YLGHBH + "|" + YWZQH + "|" + djh + "|" +
                                  "|" + djh + "||" + JYLSH + "|1|" +
                                  "|" + YLGHBH + "|0.00|||" +
                                  grbh + "|0.00|0.00|0.00|0.00|" +
                                  "0.00|0.00|0.00|" + dbbyzf + "|" + ybfwnfy + "|";
                WriteLog(sysdate + "  门诊登记(挂号)费用结算返回参数|" + strValue);
                strSql = string.Format(@"insert into ybfyjsdr (jzlsh,jylsh,djhin,cyrq,cyyy,bzbm,bzmc,yllb,xm,kh,
                                        grbh,ylfze,zbxje,tcjjzf,dejjzf,dbbyzf,mzjzfy,zhzf,xjzf,zhzfzf,
                                        zhzfzl,xjzhzf,xjzhzl,ybfwnfy,bcjsqzhye,dbzbc,smxx,yfhj,zlxmfhj,bbzf,
                                        bz6,jbr,zcfbz,djh,cfmxjylsh,zffy,ybjzlsh,cxbz,sysdate,jsrq) values(
                                        '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}',
                                        '{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}',
                                        '{20}','{21}','{22}','{23}','{24}','{25}','{26}','{27}','{28}','{29}',
                                        '{30}','{31}','{32}','{33}','{34}','{35}','{36}','{37}','{38}','{39}')",
                                        jzlsh, JYLSH, djh, cyrq, cyyy, bzbm, bzmc, yllb_r, xm, kh,
                                        grbh, ylzfy1, zbxje, tcjjzf, dejjzf, dbbyzf, mzfdfy, zhzf, xjzf1, zhzfzf,
                                        zhzfzl, xjzfzf, xjzfzl, ybfwnfy, bcjsqzhye, dbzbm, smxx, yfhj, zlxmfhj,
                                        bbzf, by6, jbrbh, zcfbz, djh, cfjylsh, xjzf_gh, ybjzlsh, 1, sysdate, jsrq);
                object[] obj = { strSql };
                obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);
                if (obj[1].ToString().Equals("1"))
                {
                    WriteLog(sysdate + " 门诊登记(挂号)费用结算成功|" + strValue);
                    return new object[] { 0, 1, strValue };
                }
                else
                {
                    WriteLog(sysdate + " 门诊登记(挂号)费用结算失败|数据操作失败|" + obj[2].ToString());
                    //撤销结算信息
                    object[] objFYDJCX = { jzlsh, djh, "0", ybjzlsh };
                    objFYDJCX = N_YBMZZYFYJSCX(objFYDJCX);
                    return new object[] { 0, 0, "门诊登记(挂号)费用结算失败|数据操作失败|" + obj[2].ToString() };
                }
            }
            else
            {
                WriteLog(sysdate + "  门诊登记(挂号)费用结算失败" + outputData.ToString());
                return new object[] { 0, 0, outputData.ToString() };
            }
        }
        #endregion

        #region 门诊登记(挂号)收费撤销
        public static object[] YBMZDJSFCX(object[] objParam)
        {
            string jzlsh = objParam[0].ToString();
            string djh = string.Empty;

            if (string.IsNullOrEmpty(jzlsh))
                return new object[] { 0, 0, "就诊流水号不能为空" };

            string strSql = string.Format(@"select * from ybmzzydjdr where jzlsh='{0}' and cxbz=1 and jzbz='m'", jzlsh);
            DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            if (ds.Tables[0].Rows.Count == 0)
                return new object[] { 0, 0, "该患者未做医保登记" };

            strSql = string.Format(@"select * from ybfyjsdr where jzlsh='{0}' and cxbz=1", jzlsh);
            ds.Tables[0].Clear();
            ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);

            if (ds.Tables[0].Rows.Count == 0)
            {
                //只做门诊登记（挂号）撤销
                object[] objMZDJCX = new object[] { jzlsh };
                return YBMZDJCX(objMZDJCX);
            }
            else
            {
                //进行门诊登记（挂号）收费，需撤销费用后再撤销登记
                djh = ds.Tables[0].Rows[0]["djhin"].ToString();
                object[] objParam1 = new object[] { jzlsh, djh };
                object[] objReturn = YBMZSFJSCX(objParam1);
                if (objReturn[1].ToString().Equals("1"))
                    //撤销门诊登记（挂号）
                    return YBMZDJCX(objParam1);
                else
                    return objReturn;
            }
        }
        #endregion

        #region 门诊登记
        public static object[] YBMZDJ(object[] objParam)
        {
            string sysdate = GetServerDateTime();
            try
            {
                string jzlsh = objParam[0].ToString(); //就诊流水号
                string yllb = objParam[1].ToString();   // 医疗类别代码
                string bzbm = objParam[2].ToString();   // 病种编码(icd10)
                string bzmc = objParam[3].ToString();   // 病种名称
                string[] kxx = objParam[4].ToString().Split('|'); //读卡返回信息
                string ghsj = Convert.ToDateTime(objParam[5].ToString()).ToString("yyyyMMddHHmmss");   // 挂号时间(格式：DateTime.Now.ToString("yyyyMMddHHmmss"))
                //string dgysdm=objParam[6].ToString();   // 定岗医师代码(汤山不用)
                //string dgysxm = objParam[7].ToString();   //定岗医师名称(汤山不用)

                string ybjzlsh = jzlsh + DateTime.Now.ToString("HHmmss");
                #region 获取读卡信息
                if (kxx.Length < 10)
                    return new object[] { 0, 0, "入参|读卡信息错误" };
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
                string zhye = kxx[14].ToString(); //帐户余额
                string dwmc = kxx[25].ToString(); //单位名称
                string nl = kxx[26].ToString(); //年龄
                #endregion

                YWBH = "2210";
                JYLSH = DateTime.Now.ToString("yyyyMMddHHmmss") + "-" + YLGHBH + "-" + new Random().Next(100).ToString().PadLeft(4, '0');
                CZYBH = CliUtils.fLoginUser;
                YWZQH = CliUtils.fLoginYbNo.Split('|')[0];

                string ksmc = string.Empty; //科室名称
                string ksbm = string.Empty; //科室编码
                string ybbh = string.Empty; //医保卡号
                string lxdh = string.Empty; //联系电话
                string cwh = "";    //床位号
                string ysbm = string.Empty;   //医生编码
                string jbr = CliUtils.fUserName; //经办人
                string hzxm = string.Empty; //患者姓名

                if (string.IsNullOrEmpty(jzlsh))
                    return new object[] { 0, 0, "就诊流水号不能为空" };
                if (string.IsNullOrEmpty(yllb))
                    return new object[] { 0, 0, "医疗类别代码不能为空|" };
                if (string.IsNullOrEmpty(ghsj))
                    return new object[] { 0, 0, "挂号时间不能为空|" };

                string[] syllb={"11","13","86"};
                if (string.IsNullOrEmpty(bzbm) && !syllb.Contains(yllb))
                    return new object[] { 0, 0, "病种不能为空" };

                string strSql = string.Format(@"select a.m1name as name, a.m1telp,a.m1addr,a.m1gham,a.m1ksno,a.m1empn,
                                            (select b2ksnm from bz02h  where b2ksno=a.m1ksno) as ksmc 
                                            from mz01h  a where a.m1ghno = '{0}' ", jzlsh);
                DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                if (ds.Tables[0].Rows.Count == 0)
                    return new object[] { 0, 0, "该患者未挂号" };
                ybbh = grbh;
                ksbm = ds.Tables[0].Rows[0]["m1ksno"].ToString();
                ksmc = ds.Tables[0].Rows[0]["ksmc"].ToString();
                lxdh = ds.Tables[0].Rows[0]["m1telp"].ToString();
                ysbm = ds.Tables[0].Rows[0]["m1empn"].ToString();
                hzxm = ds.Tables[0].Rows[0]["name"].ToString();

                if (!string.Equals(hzxm, xm))
                    return new object[] { 0, 0, "挂号登记姓名和医保卡姓名不相符" };

                strSql = string.Format(@"select * from ybmzzydjdr where jzlsh='{0}' and jzbz='m' and cxbz=1", jzlsh);
                ds.Tables.Clear();
                ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                if (ds.Tables[0].Rows.Count > 0)
                    return new object[] { 0, 0, "患者已进行医保门诊登记，清匆再进行重复操作" };
                #region 获取定岗医生信息
                string ysgjm = string.Empty;
                strSql = string.Format(@"select * from ybdgyszd where ysbm='{0}'", ysbm);
                ds.Tables.Clear();
                ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    ysgjm = ds.Tables[0].Rows[0]["dgysbm"].ToString();
                }
                #endregion
                //上传参数
                StringBuilder inputParam = new StringBuilder();
                inputParam.Append(ybjzlsh + "|");
                inputParam.Append(yllb + "|");
                inputParam.Append(ghsj + "|");
                inputParam.Append(bzbm + "|");
                inputParam.Append(ksmc + "|");
                inputParam.Append(ksbm + "|");
                inputParam.Append(cwh + "|");
                inputParam.Append(ysbm + "|");
                inputParam.Append(jbr + "|");
                inputParam.Append(lxdh + "|");
                inputParam.Append(ybbh + "|");
                //inputParam.Append("||||||");
                inputParam.Append("|");
                inputParam.Append("|"); //准生儿社会保障卡号
                inputParam.Append(YLJGGJM+"|"); //医疗机构国家码
                inputParam.Append("|"); //护士国家码
                inputParam.Append(""+"|"); //药师国家码
                //inputParam.Append(ysgjm + "|"); //药师国家码
                inputParam.Append("|"); //备用6
                //入参
                StringBuilder inputData = new StringBuilder();
                inputData.Append(YWBH + "^");
                inputData.Append(YLGHBH + "^");
                inputData.Append(CZYBH + "^");
                inputData.Append(YWZQH + "^");
                inputData.Append(JYLSH + "^");
                inputData.Append(JRFS + "^");
                inputData.Append(DKLX + "^");
                inputData.Append(inputParam + "^");

                StringBuilder outputData = new StringBuilder(102400);
                List<string> liSQL = new List<string>();

                WriteLog(sysdate + "  进入门诊登记...");
                WriteLog(sysdate + "  入参|" + inputData.ToString());
                int i = BUSINESS_HANDLE(inputData, outputData);
                if (i == 0)
                {
                    strSql = string.Format(@"insert into ybmzzydjdr(
                                        jzlsh,jylsh,yldylb,ghdjsj,bzbm,bzmc,bq,ksbh,ksmc,cwh,
                                        ysdm,jbr,dwmc,grbh,xm,xb,ybjzlsh,kh,yllb,cxbz,
                                        sysdate,tcqh,ydrybz,nl,csrq,zhye,jzbz)
                                        values(
                                        '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}',
                                        '{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}',
                                        '{20}','{21}','{22}','{23}','{24}','{25}','{26}')",
                                        jzlsh, JYLSH, yldylb, ghsj, bzbm, bzmc, "", ksbm, ksmc, cwh,
                                        ysbm, jbr, dwmc, ybbh, xm, xb, ybjzlsh, kh, yllb, 1,
                                        sysdate, tcqh, ydrybz, nl, csrq, zhye, "m");
                    object[] obj_dj = { strSql };
                    obj_dj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj_dj);
                    if (obj_dj[1].ToString().Equals("1"))
                    {
                        WriteLog(sysdate + "  门诊登记成功|" + outputData.ToString());
                        return new object[] { 0, 1, "门诊登记成功" };
                    }
                    else
                    {
                        WriteLog(sysdate + "  门诊登记失败|数据操作失败|" + obj_dj[2].ToString());
                        //门诊登记撤销
                        object[] objParam1 = { ybjzlsh };
                        objParam1 = N_YBMZZYDJCX(objParam1);
                        return new object[] { 0, 0, "门诊登记失败|数据操作失败|" + obj_dj[2].ToString() };
                    }
                }
                else
                {
                    WriteLog(sysdate + "  门诊登记失败|" + outputData.ToString());
                    return new object[] { 0, 0, outputData.ToString().Split('^')[2].ToString() };
                }
            }
            catch (Exception ex)
            {
                WriteLog(sysdate + "  门诊登记接口异常|" + ex.Message);
                return new object[] { 0, 0, "门诊登记接口异常|" + ex.Message };
            }
        }
        #endregion

        #region 门诊登记撤销
        public static object[] YBMZDJCX(object[] objParam)
        {
            string sysdate=GetServerDateTime();
            string jzlsh = objParam[0].ToString();

            YWBH = "2240";
            JYLSH = DateTime.Now.ToString("yyyyMMddHHmmss") + "-" + YLGHBH + "-" + new Random().Next(100).ToString().PadLeft(4, '0');
            CZYBH = CliUtils.fLoginUser;
            YWZQH = CliUtils.fLoginYbNo.Split('|')[0];
            string jbr = CliUtils.fUserName;
            string ybjzlsh = string.Empty;

            if (string.IsNullOrEmpty(jzlsh))
                return new object[] { 0, 0, "就诊流水号不能为空" };

            StringBuilder inputData = new StringBuilder();
            StringBuilder inputParam = new StringBuilder();
            StringBuilder outputData = new StringBuilder(1024);

            #region 判断是否进行门诊登记
            string strSql = string.Format(@"select jzlsh,ybjzlsh,cxbz from ybmzzydjdr where cxbz=1 and jzlsh='{0}' and jzbz='m'", jzlsh);
            DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            if (ds.Tables[0].Rows.Count == 0)
                return new object[] { 0, 0, "该患者未医保门诊登记" };
            ybjzlsh = ds.Tables[0].Rows[0]["ybjzlsh"].ToString();
            #endregion


            #region 判断是否已经结算
            strSql = string.Format(@"select jzlsh,cxbz from ybfyjsdr where cxbz=1 and jzlsh='{0}' ", jzlsh);
            ds.Tables.Clear();
            ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            if (ds.Tables[0].Rows.Count > 0)
                return new object[] { 0, 0, "该患者已经产生医保结算，请进行结算撤销" };
            #endregion

            //入参数据
            inputParam.Append(ybjzlsh + "|");
            inputParam.Append(jbr + "|");

            //入参
            inputData.Append(YWBH + "^");
            inputData.Append(YLGHBH + "^");
            inputData.Append(CZYBH + "^");
            inputData.Append(YWZQH + "^");
            inputData.Append(JYLSH + "^");
            inputData.Append(JRFS + "^");
            inputData.Append(DKLX + "^");
            inputData.Append(inputParam + "^");

            WriteLog(sysdate+"  进入医保门诊登记撤销...");
            WriteLog(sysdate + "  入参|" + inputData.ToString());
            int i = BUSINESS_HANDLE(inputData, outputData);
            if (i == 0)
            {
                List<string> liSQL = new List<string>();
                strSql = string.Format(@"insert into ybmzzydjdr(
                                        jzlsh,jylsh,yldylb,ghdjsj,bzbm,bzmc,bq,ksbh,ksmc,cwh,
                                        ysdm,jbr,dwmc,grbh,xm,xb,ybjzlsh,kh,yllb,tcqh,
                                        ydrybz,nl,csrq,zhye,jzbz,cxbz,sysdate)
                                        select jzlsh,jylsh,yldylb,ghdjsj,bzbm,bzmc,bq,ksbh,ksmc,cwh,
                                        ysdm,'{1}',dwmc,grbh,xm,xb,ybjzlsh,kh,yllb,tcqh,
                                        ydrybz,nl,csrq,zhye,jzbz,0, '{2}' 
                                        from ybmzzydjdr where jzlsh='{0}' and cxbz=1", jzlsh, jbr, sysdate);
                liSQL.Add(strSql);
                strSql = string.Format(@"update ybmzzydjdr set cxbz=2 where jzlsh='{0}' and cxbz=1", jzlsh);
                liSQL.Add(strSql);

                object[] obj = liSQL.ToArray();
                obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);
                if (obj[1].ToString().Equals("1"))
                {
                    WriteLog(sysdate + "  医保门诊登记撤销成功|");
                    return new object[] { 0, 1, "医保门诊登记撤销成功" };
                }
                else
                {
                    WriteLog(sysdate + "  医保门诊登记撤销失败|本地操作失败|" + obj[2].ToString());
                    return new object[] { 0, 0, obj[2].ToString() };
                }
            }
            else
            {
                WriteLog(sysdate + "  医保门诊登记撤销失败|" + outputData.ToString());
                return new object[] { 0, 0, outputData.ToString() };
            }
        }
        #endregion

        #region 门诊挂号费用登记
        public static object[] YBMZGHFDJ(object[] objParam)
        {
            string sysdate = GetServerDateTime();
            try
            {
                string jzlsh = objParam[0].ToString(); //流水号不能为空
                if (string.IsNullOrWhiteSpace(jzlsh))
                    return new object[] { 0, 0, "医保提示：就诊流水号为空" };

                JYLSH = DateTime.Now.ToString("yyyyMMddHHmmss") + "-" + YLGHBH + "-" + new Random().Next(100).ToString().PadLeft(4, '0');
                CZYBH = CliUtils.fLoginUser;
                YWZQH = CliUtils.fLoginYbNo.Split('|')[0];
                string jbr = CliUtils.fUserName;

                string strSql = string.Format(@"select bzbm,bzmc,xm,grbh,kh,ybjzlsh from ybmzzydjdr where jzlsh='{0}' and cxbz=1 ", jzlsh);
                DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                if (ds.Tables[0].Rows.Count == 0)
                    return new object[] { 0, 0, "该患者未医保门诊登记" };
                string bzbm = ds.Tables[0].Rows[0]["bzbm"].ToString();  //病种编码
                string xm = ds.Tables[0].Rows[0]["xm"].ToString();
                string grbh = ds.Tables[0].Rows[0]["grbh"].ToString();
                string kh = ds.Tables[0].Rows[0]["kh"].ToString();
                string ybjzlsh = ds.Tables[0].Rows[0]["ybjzlsh"].ToString();

                #region 费用上传前，撤销所有未结算费用
                object[] objGHFYDJ = { jzlsh };
                //费用登记前，撤销费用登记
                objGHFYDJ = YBMZFYDJCX(objGHFYDJ);
                #endregion

                #region 获取费用明细
                string sfxmzl = string.Empty;  //收费项目种类
                string cfh = string.Empty;    //处方号
                string cflsh = string.Empty;    //处方流水号
                string cfrq = string.Empty;
                string cfrq1 = string.Empty;  //处方日期
                string yyxmbm = string.Empty;    //医院收费项目自编码
                string yyxmmc = string.Empty; //医院收费项目名称
                string dj = string.Empty;  //单价
                string sl = string.Empty;  //数量
                double je = 0.00;  //金额
                string fs = string.Empty;  //中药饮片副数
                string ysbm = string.Empty;    //医生编码
                string ksbm = string.Empty;    //医生编码
                string sfbz = string.Empty;      //按最小计价单位收费标志
                string by3 = "";    //事务处理类别
                string by4 = "";    //处方医院编号
                string by5 = "";    //外配处方号
                string by6 = "";    //药品电子监管码

                string fph = string.Empty; //发票号
                double blbfy = 0.00; //病历本费用
                double ylkfy = 0.00; //医疗卡费用
                double ghfy = 0.00;    //挂号总费用
                double ghfylj = 0.00;
                double ghfyvv = 0.00;

                int index = 1;
                List<string> liSQL = new List<string>();
                StringBuilder inputParam = new StringBuilder();

                #region 获取诊查费
                strSql = string.Format(@"select '2' as sfxmzl,a.m1ghno as cfh, a.m1date as cfrq,c.bzmem2 yysfxmbm,
                                    c.bzname yysfxmmc,c.bzmem1 as dj,1 as sl,a.m1gham je, '' as fs, 
                                    a.m1empn as ysbm,a.m1ksno as ksbm,0 as sfbz,'' as bzbm,a.m1invo,a.m1gham1,a.m1blam,a.m1kham,a.m1amnt,ybxmbh
                                    from mz01h a           
                                    join bztbd c on a.m1zlfb = c.bzkeyx and c.bzcodn = 'A7' 
                                    left join ybhisdzdr z on c.bzmem2 = z.hisxmbh                
                                    where a.m1ghno = '{0}'", jzlsh);
                ds.Tables.Clear();
                ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                DataRow dr = ds.Tables[0].Rows[0];

                sfxmzl = dr["sfxmzl"].ToString();  //收费项目种类
                cfh = dr["sfxmzl"].ToString() + DateTime.Now.ToString("HHmmss");    //处方号
                cflsh = (index++).ToString();    //处方流水号
                cfrq = dr["cfrq"].ToString();
                cfrq1 = Convert.ToDateTime(dr["cfrq"].ToString()).ToString("yyyyMMddHHmmss");  //处方日期
                yyxmbm = dr["yysfxmbm"].ToString();    //医院收费项目自编码
                yyxmmc = dr["yysfxmmc"].ToString(); //医院收费项目名称
                dj = dr["dj"].ToString();  //单价
                sl = dr["sl"].ToString();  //数量
                je = Convert.ToDouble(dr["je"].ToString());  //金额
                ghfylj += je;
                fs = dr["fs"].ToString();  //中药饮片副数
                ysbm = dr["ysbm"].ToString();    //医生编码
                ksbm = dr["ksbm"].ToString();    //科室编码
                sfbz = dr["sfbz"].ToString();      //按最小计价单位收费标志
                by3 = "";    //事务处理类别
                by4 = "";    //处方医院编号
                by5 = "";    //外配处方号
                by6 = "";    //药品电子监管码
                string gjbm = dr["ybxmbh"].ToString();    //国家编码
                #region 获取定岗医生信息
                string ysgjm = string.Empty;
                strSql = string.Format(@"select * from ybdgyszd where ysbm='{0}'", ysbm);
                ds.Tables.Clear();
                ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    ysgjm = ds.Tables[0].Rows[0]["dgysbm"].ToString();
                }
                #endregion

                blbfy = Convert.ToDouble(dr["m1blam"].ToString());
                ylkfy = Convert.ToDouble(dr["m1kham"].ToString());
                ghfy = Convert.ToDouble(dr["m1amnt"].ToString());
                ghfyvv = Convert.ToDouble(dr["m1gham1"].ToString());



                //入参数据
                inputParam.Append(ybjzlsh + "|");
                inputParam.Append(sfxmzl + "|");
                inputParam.Append(cfh + "|");
                inputParam.Append(cflsh + "|");
                inputParam.Append(cfrq1 + "|");
                inputParam.Append(yyxmbm + "|");
                inputParam.Append(dj + "|");
                inputParam.Append(sl + "|");
                inputParam.Append(je + "|");
                inputParam.Append(fs + "|");
                inputParam.Append(ysbm + "|");
                inputParam.Append(ksbm + "|");
                inputParam.Append(jbr + "|");
                inputParam.Append(sfbz + "|");
                inputParam.Append(bzbm + "|");
                inputParam.Append(by3 + "|");
                inputParam.Append(by4 + "|");
                inputParam.Append(by5 + "|");
                inputParam.Append(by6 + "|");
                inputParam.Append(YLJGGJM + "|"); //20200501增加
                inputParam.Append(gjbm + "|");
                inputParam.Append(by5 + "|");
                inputParam.Append("" + "|$");
                //inputParam.Append(ysgjm + "|$");
                strSql = string.Format(@"insert into ybcfmxscindr(jzlsh,jylsh,sfxmzl,ybcfh,djlsh,cfrq,yysfxmbm,yysfxmmc,dj,sl,
						                   je,ysbm,ksbh,jbr,sflb,grbh,xm,kh,cxbz,sysdate,ybjzlsh) values(
                                           '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}',
                                           '{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}','{20}')",
                                          jzlsh, JYLSH, sfxmzl, cfh, cflsh, cfrq, yyxmbm, yyxmmc, dj, sl,
                                          je, ysbm, ksbm, jbr, sfbz, grbh, xm, kh, 1, sysdate, ybjzlsh);
                liSQL.Add(strSql);
                #endregion

                #region 获取挂号费
                if (ghfyvv > 0)
                {
                    strSql = string.Format(@"select '2' as sfxmzl,a.m1ghno as cfh, a.m1date as cfrq,c.b5item yysfxmbm,
                                            c.b5name yysfxmmc,c.b5amt2 as dj,1 as sl,a.m1gham1 je, '' as fs, 
                                            a.m1empn as ysbm,a.m1ksno as ksbm,0 as sfbz,'' as bzbm,a.m1invo,a.m1blam,a.m1kham,a.m1amnt,ybxmbh
                                            from mz01h a  
                                            left join bz05d  c on c.b5item='800000000160' 
                                            left join ybhisdzdr d on d.hisxmbh=c.b5item    
                                            where a.m1ghno = '{0}'", jzlsh);
                    ds.Tables.Clear();
                    ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                    dr = ds.Tables[0].Rows[0];

                    sfxmzl = dr["sfxmzl"].ToString();  //收费项目种类
                    //cfh = dr["sfxmzl"].ToString() + DateTime.Now.ToString("HHmmss");    //处方号
                    cflsh = (index++).ToString();    //处方流水号
                    //cfrq = dr["cfrq"].ToString();
                    //cfrq1 = Convert.ToDateTime(dr["cfrq"].ToString()).ToString("yyyyMMddHHmmss");  //处方日期
                    yyxmbm = dr["yysfxmbm"].ToString();    //医院收费项目自编码
                    yyxmmc = dr["yysfxmmc"].ToString(); //医院收费项目名称
                    dj = dr["dj"].ToString();  //单价
                    sl = dr["sl"].ToString();  //数量
                    je = Convert.ToDouble(dr["je"].ToString());  //金额
                    ghfylj += je;
                    fs = dr["fs"].ToString();  //中药饮片副数
                    //ysbm = dr["m1empn"].ToString();    //医生编码
                    //ksbm = dr["m1ksno"].ToString();    //科室编码
                    sfbz = dr["sfbz"].ToString();      //按最小计价单位收费标志
                    by3 = "";    //事务处理类别
                    by4 = "";    //处方医院编号
                    by5 = "";    //外配处方号
                    by6 = "";    //药品电子监管码
                    gjbm = dr["ybxmbh"].ToString();    //国家编码


                    //入参数据
                    inputParam.Append(ybjzlsh + "|");
                    inputParam.Append(sfxmzl + "|");
                    inputParam.Append(cfh + "|");
                    inputParam.Append(cflsh + "|");
                    inputParam.Append(cfrq1 + "|");
                    inputParam.Append(yyxmbm + "|");
                    inputParam.Append(dj + "|");
                    inputParam.Append(sl + "|");
                    inputParam.Append(je + "|");
                    inputParam.Append(fs + "|");
                    inputParam.Append(ysbm + "|");
                    inputParam.Append(ksbm + "|");
                    inputParam.Append(jbr + "|");
                    inputParam.Append(sfbz + "|");
                    inputParam.Append(bzbm + "|");
                    inputParam.Append(by3 + "|");
                    inputParam.Append(by4 + "|");
                    inputParam.Append(by5 + "|");
                    inputParam.Append(by6 + "|");
                    inputParam.Append(YLJGGJM + "|"); //20200501增加
                    inputParam.Append(gjbm + "|");
                    inputParam.Append(by5 + "|");
                    inputParam.Append("" + "|$");
                    //inputParam.Append(ysgjm + "|$");
                    strSql = string.Format(@"insert into ybcfmxscindr(jzlsh,jylsh,sfxmzl,ybcfh,djlsh,cfrq,yysfxmbm,yysfxmmc,dj,sl,
						                   je,ysbm,ksbh,jbr,sflb,grbh,xm,kh,cxbz,sysdate,ybjzlsh) values(
                                           '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}',
                                           '{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}','{20}')",
                                              jzlsh, JYLSH, sfxmzl, cfh, cflsh, cfrq, yyxmbm, yyxmmc, dj, sl,
                                              je, ysbm, ksbm, jbr, sfbz, grbh, xm, kh, 1, sysdate, ybjzlsh);
                    liSQL.Add(strSql);
                }

                #endregion

                #region 获取病历本费
                if (blbfy > 0)
                {
                    strSql = string.Format(@"select '2' as sfxmzl,'' as cfh, '' as cfrq,
                                    a.pamark as yysfxmbm,b.b5name as yysfxmmc,b.b5sfam as dj,1 as sl,b.b5sfam as je ,'' as fs,
                                    '' as ysbm,'' as m1ksno, 0 as sfbz,'' as bzbm,ybxmbh
                                    from patbh a
                                    left join bz05d b on a.pamark=b.b5item
                                    left join ybhisdzdr c on c.hisxmbh=b.b5item
                                    where pakind='MZ' and pasequ='0001'");

                    ds.Tables.Clear();
                    ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                    dr = ds.Tables[0].Rows[0];

                    sfxmzl = dr["sfxmzl"].ToString();  //收费项目种类
                    //cfh = dr["sfxmzl"].ToString() + DateTime.Now.ToString("HHmmss");    //处方号
                    cflsh = (index++).ToString();    //处方流水号
                    //cfrq = dr["cfrq"].ToString();
                    //cfrq1 = Convert.ToDateTime(dr["cfrq"].ToString()).ToString("yyyyMMddHHmmss");  //处方日期
                    yyxmbm = dr["yysfxmbm"].ToString();    //医院收费项目自编码
                    yyxmmc = dr["yysfxmmc"].ToString(); //医院收费项目名称
                    dj = dr["dj"].ToString();  //单价
                    sl = dr["sl"].ToString();  //数量
                    je = Convert.ToDouble(dr["je"].ToString());  //金额
                    ghfylj += je;
                    fs = dr["fs"].ToString();  //中药饮片副数
                    //ysbm = dr["m1empn"].ToString();    //医生编码
                    //ksbm = dr["m1ksno"].ToString();    //科室编码
                    sfbz = dr["sfbz"].ToString();      //按最小计价单位收费标志
                    by3 = "";    //事务处理类别
                    by4 = "";    //处方医院编号
                    by5 = "";    //外配处方号
                    by6 = "";    //药品电子监管码
                    gjbm = dr["ybxmbh"].ToString();    //国家编码


                    //入参数据
                    inputParam.Append(ybjzlsh + "|");
                    inputParam.Append(sfxmzl + "|");
                    inputParam.Append(cfh + "|");
                    inputParam.Append(cflsh + "|");
                    inputParam.Append(cfrq1 + "|");
                    inputParam.Append(yyxmbm + "|");
                    inputParam.Append(dj + "|");
                    inputParam.Append(sl + "|");
                    inputParam.Append(je + "|");
                    inputParam.Append(fs + "|");
                    inputParam.Append(ysbm + "|");
                    inputParam.Append(ksbm + "|");
                    inputParam.Append(jbr + "|");
                    inputParam.Append(sfbz + "|");
                    inputParam.Append(bzbm + "|");
                    inputParam.Append(by3 + "|");
                    inputParam.Append(by4 + "|");
                    inputParam.Append(by5 + "|");
                    inputParam.Append(by6 + "|");
                    inputParam.Append(YLJGGJM + "|"); //20200501增加
                    inputParam.Append(gjbm + "|");
                    inputParam.Append(by5 + "|");
                    inputParam.Append("" + "|$");
                    //inputParam.Append(ysgjm + "|$");
                    strSql = string.Format(@"insert into ybcfmxscindr(jzlsh,jylsh,sfxmzl,ybcfh,djlsh,cfrq,yysfxmbm,yysfxmmc,dj,sl,
						                   je,ysbm,ksbh,jbr,sflb,grbh,xm,kh,cxbz,sysdate,ybjzlsh) values(
                                           '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}',
                                           '{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}','{20}')",
                                              jzlsh, JYLSH, sfxmzl, cfh, cflsh, cfrq, yyxmbm, yyxmmc, dj, sl,
                                              je, ysbm, ksbm, jbr, sfbz, grbh, xm, kh, 1, sysdate, ybjzlsh);
                    liSQL.Add(strSql);
                }
                #endregion

                #region 获取医疗卡费
                if (ylkfy > 0)
                {
                    strSql = string.Format(@"select '2' as sfxmzl,'' as cfh, '' as cfrq,
                                    a.pamark as yysfxmbm,b.b5name as yysfxmmc,b.b5sfam as dj,1 as sl,b.b5sfam as je ,'' as fs,
                                    '' as ysbm,'' as m1ksno, 0 as sfbz,'' as bzbm,ybxmbh
                                    from patbh a
                                    left join bz05d b on a.pamark=b.b5item
                                    left join ybhisdzdr c on c.hisxmbh=b.b5item
                                    where pakind='MZ' and pasequ='0004'");

                    ds.Tables.Clear();
                    ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                    dr = ds.Tables[0].Rows[0];

                    sfxmzl = dr["sfxmzl"].ToString();  //收费项目种类
                    //cfh = dr["sfxmzl"].ToString() + DateTime.Now.ToString("HHmmss");    //处方号
                    cflsh = (index++).ToString();    //处方流水号
                    //cfrq = dr["cfrq"].ToString();
                    //cfrq1 = Convert.ToDateTime(dr["cfrq"].ToString()).ToString("yyyyMMddHHmmss");  //处方日期
                    yyxmbm = dr["yysfxmbm"].ToString();    //医院收费项目自编码
                    yyxmmc = dr["yysfxmmc"].ToString(); //医院收费项目名称
                    dj = dr["dj"].ToString();  //单价
                    sl = dr["sl"].ToString();  //数量
                    je = Convert.ToDouble(dr["je"].ToString());  //金额
                    ghfylj += je;
                    fs = dr["fs"].ToString();  //中药饮片副数
                    //ysbm = dr["m1empn"].ToString();    //医生编码
                    //ksbm = dr["m1ksno"].ToString();    //科室编码
                    sfbz = dr["sfbz"].ToString();      //按最小计价单位收费标志
                    by3 = "";    //事务处理类别
                    by4 = "";    //处方医院编号
                    by5 = "";    //外配处方号
                    by6 = "";    //药品电子监管码
                    gjbm = dr["ybxmbh"].ToString();    //国家编码

                    //入参数据
                    inputParam.Append(ybjzlsh + "|");
                    inputParam.Append(sfxmzl + "|");
                    inputParam.Append(cfh + "|");
                    inputParam.Append(cflsh + "|");
                    inputParam.Append(cfrq1 + "|");
                    inputParam.Append(yyxmbm + "|");
                    inputParam.Append(dj + "|");
                    inputParam.Append(sl + "|");
                    inputParam.Append(je + "|");
                    inputParam.Append(fs + "|");
                    inputParam.Append(ysbm + "|");
                    inputParam.Append(ksbm + "|");
                    inputParam.Append(jbr + "|");
                    inputParam.Append(sfbz + "|");
                    inputParam.Append(bzbm + "|");
                    inputParam.Append(by3 + "|");
                    inputParam.Append(by4 + "|");
                    inputParam.Append(by5 + "|");
                    inputParam.Append(by6 + "|");
                    inputParam.Append(YLJGGJM + "|"); //20200501增加
                    inputParam.Append(gjbm + "|");
                    inputParam.Append(by5 + "|");
                    inputParam.Append("" + "|$");
                    //inputParam.Append(ysgjm + "|$");
                    strSql = string.Format(@"insert into ybcfmxscindr(jzlsh,jylsh,sfxmzl,ybcfh,djlsh,cfrq,yysfxmbm,yysfxmmc,dj,sl,
						                   je,ysbm,ksbh,jbr,sflb,grbh,xm,kh,cxbz,sysdate,ybjzlsh) values(
                                           '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}',
                                           '{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}','{20}')",
                                              jzlsh, JYLSH, sfxmzl, cfh, cflsh, cfrq, yyxmbm, yyxmmc, dj, sl,
                                              je, ysbm, ksbm, jbr, sfbz, grbh, xm, kh, 1, sysdate, ybjzlsh);
                    liSQL.Add(strSql);
                }
                #endregion


                //判断总费用与累计费用是否相等
                if (Math.Abs(ghfy - ghfylj) > 1.0)
                    return new object[] { 0, 0, "收费金额与医保结算金额相差" + Math.Abs(ghfy - ghfylj) + ",无法结算，请核实!" };
                #endregion

                YWBH = "2310";
                //入参
                StringBuilder inputData = new StringBuilder();
                inputData.Append(YWBH + "^");
                inputData.Append(YLGHBH + "^");
                inputData.Append(CZYBH + "^");
                inputData.Append(YWZQH + "^");
                inputData.Append(JYLSH + "^");
                inputData.Append(JRFS + "^");
                inputData.Append(DKLX + "^");
                inputData.Append(inputParam.ToString().TrimEnd('$') + "^");

                WriteLog(sysdate + "   " + jzlsh + "|进入门诊挂号费上传...");
                WriteLog("入参|" + inputData.ToString());
                StringBuilder outputData = new StringBuilder(10240);
                int i = BUSINESS_HANDLE(inputData, outputData);

                WriteLog(sysdate + "  回参|" + outputData.ToString());
                if (i == 0)
                {
                    //记录返回数据
                    string[] sValue = outputData.ToString().Split('^')[1].TrimEnd('$').Split('$');
                    foreach (string sRow in sValue)
                    {
                        string[] s = sRow.Split('|');
                        string cfh_r = s[0];    //处方号
                        string cflsh_r = s[1];  //处方流水号
                        string cfrq_r = s[2];   //处方日期
                        string yyxmbm_r = s[3]; //医院收费项目自编码
                        string je_r = s[4];     //金额
                        string zfje_r = s[5];   //自付金额
                        string zlje_r = s[6];   //自理金额
                        string zfbl_r = s[7];   //自付比例
                        string zfsx_r = s[8];   //支付上限
                        string sfxmdj_r = s[9]; //收费项目等级
                        string smxx_r = s[10];  //说明信息
                        string by2_r = s[11];   //备用2
                        string by3_r = s[12];   //备用3
                        string by4_r = s[13];   //备用4
                        string by5_r = s[14];   //备用5
                        string by6_r = s[15];   //备用6

                        strSql = string.Format(@"insert into ybcfmxscfhdr(jzlsh,jylsh,yyxmdm,ybcfh,cfh,je,zfje,zlje,zlbl,qezfbz,
                                            sfxmdj,yyxmmc,grbh,xm,kh,cxbz,sysdate,ybjzlsh) values(
                                            '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}',
                                            '{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}')",
                                                jzlsh, JYLSH, yyxmbm_r, cfh_r, cflsh_r, je_r, zfje_r, zlje_r, zfbl_r, zfsx_r,
                                                sfxmdj_r, smxx_r, grbh, xm, kh, 1, sysdate, ybjzlsh);
                        liSQL.Add(strSql);
                    }

                    object[] obj = liSQL.ToArray();
                    obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);
                    if (obj[1].ToString().Equals("1"))
                    {
                        WriteLog(sysdate + "  门诊挂号费登记成功|" + outputData.ToString());
                        return new object[] { 0, 1, JYLSH };
                    }
                    else
                    {
                        WriteLog(sysdate + "  门诊挂号费登记失败|数据操作失败|" + obj[2].ToString());
                        //门诊费用上传撤销
                        object[] objParam1 = { jzlsh, "", "", jbr, ybjzlsh };
                        objParam1 = N_YBMZZYSFDJCX(objParam1);
                        return new object[] { 0, 0, "门诊挂号费登记失败|数据操作失败|" + obj[2].ToString() };
                    }
                }
                else
                {
                    WriteLog(sysdate + "  " + jzlsh + "|进入门诊挂号费登记失败|" + outputData.ToString());
                    return new object[] { 0, 0, outputData.ToString() };
                }
            }
            catch (Exception ex)
            {
                WriteLog(sysdate + "  进入门诊挂号费登记异常|" + ex.Message);
                return new object[] { 0, 0, ex.Message };
            }

        }
        #endregion

        #region 门诊费用登记(门诊处方明细上传)
        public static object[] YBMZFYDJ(object[] objParam)
        {
            string sysdate = GetServerDateTime();
            string jzlsh = objParam[0].ToString(); //流水号不能为空
            string cfhs = objParam[1].ToString();   //处方号
            string ylhjfy = objParam[2].ToString(); //医疗合计费用 (新增)
            string sfymm = "";//objParam[3].ToString(); //医保卡密码 （新增）
            string kbrq1 = objParam[4].ToString(); //看病日期 
            string jsfs = "";// param2[5].ToString();

            if (string.IsNullOrEmpty(jzlsh))
                return new object[] { 0, 0, "就诊流水号不能为空！" };
            if (string.IsNullOrEmpty(cfhs))
                return new object[] { 0, 0, "处方号不能为空！" };
            if (string.IsNullOrEmpty(ylhjfy))
                return new object[] { 0, 0, "医疗合计费不能为空" };
            if (string.IsNullOrEmpty(kbrq1))
                return new object[] { 0, 0, "看病日期不能为空,时间格式为:yyyy-MM-dd HH:mm:ss" };

            decimal sfje = 0; //收费金额
            #region 金额有效性
            try
            {
                sfje = Convert.ToDecimal(ylhjfy);
            }
            catch
            {
                return new object[] { 0, 0, "收费金额格式错误" };
            }
            #endregion

            JYLSH = DateTime.Now.ToString("yyyyMMddHHmmss") + "-" + YLGHBH + "-" + new Random().Next(100).ToString().PadLeft(4, '0');
            CZYBH = CliUtils.fLoginUser;
            YWZQH = CliUtils.fLoginYbNo.Split('|')[0];
            string jbr = CliUtils.fUserName;

            string strSql = string.Format("select * from ybmzzydjdr a where a.jzlsh = '{0}' and a.cxbz = 1", jzlsh);
            DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            if (ds == null || ds.Tables[0].Rows.Count == 0)
                return new object[] { 0, 0, "该患者未医保门诊登记" };
            string grbh = ds.Tables[0].Rows[0]["grbh"].ToString(); //个人编码
            string xm = ds.Tables[0].Rows[0]["xm"].ToString();  //姓名
            string kh = ds.Tables[0].Rows[0]["kh"].ToString();  //卡号
            string bzbm = ds.Tables[0].Rows[0]["bzbm"].ToString(); //病种编码
            string ybjzlsh = ds.Tables[0].Rows[0]["ybjzlsh"].ToString();//医保就诊流水号

            #region 费用上传前，撤销所有未结算费用
            object[] objGHFYDJ = { jzlsh };
            //费用登记前，撤销费用登记
            YBMZFYDJCX(objGHFYDJ);
            #endregion

            List<string> liSQL = new List<string>();
            #region 获取处方信息
            strSql = string.Format(@"select m.cfh,m.cflsh,m.sfxmzl,m.yyxmbh, m.yyxmmc,m.dj, sum(m.sl) sl, sum(m.je) je, m.fs,m.ysdm ysdm, n.b1name ysxm, m.ksno ksno,
                                    o.b2ejnm zxks, m.sfbz,p.ybxmbh from (
                                    --药品
                                    select a.mccfno cfh,a.mcseq2 cflsh,a.mcypno yyxmbh, a.mcypnm yyxmmc,
                                    convert(decimal(8,3),round(c.dj,3,1)) as dj, 
                                    convert(decimal(8,2),round((a.mcquty/b.y1blfs),3,1)) as sl, 
                                    convert(decimal(8,4),convert(decimal(8,3),round(c.dj,3,1))*convert(decimal(8,2),round((a.mcquty/b.y1blfs),3,1))) as je, 
                                    a.mcksno ksno, a.mcuser ysdm, 
                                    case when mcsflb='03' then cast((a.mcquty/a.mcusex) as int) else 0 end as fs,'0' as sfbz,
                                    case when left(mccfno,1)='W' then '3' else '1' end as sfxmzl
                                    from mzcfd a
                                    left join yp01h b on a.mcypno=b.y1ypno
                                    left join (select y8pchx,y8ypno,y8sequ,y8sunp as dj,y8blfs,y8stoc  from yp08d ) c 
                                    on c.y8ypno=a.mcypno and c.y8pchx=a.mcpchx and mcsequ=c.y8sequ and mcstoc=c.y8stoc
                                    where a.mcghno = '{0}' and a.mccfno in ({1})
                                    union all
                                    --检查/治疗 
                                    select b.mbzlno cfh,b.mbsequ cflsh, b.mbitem yyxmbh, b.mbname yyxmmc, b.mbpric dj, b.mbquty sl, b.mbsjam je, b.mbksno ksno, b.mbuser ysdm, 0 as fs,'0' as sfbz,
                                    case  when b.mbsfno in(select b5sfno from bz05h where b5mzno='19') then '3' else '2' end as sfxmzl
                                    from mzb2d b where b.mbghno = '{0}' and b.mbzlno in ({1})
                                    union all
                                    --检验
                                    select c.mbzlno cfh,c.mbsequ cflsh, c.mbitem yyxmbh, c.mbname yyxmmc, c.mbpric dj, c.mbquty sl, c.mbsjam je, c.mbksno ksno, c.mbuser ysdm, 0 as fs,'0' as sfbz,'2' as sfxmzl
                                    from mzb4d c where c.mbghno = '{0}' and c.mbzlno in ({1})
                                    union all
                                    --注射
                                    select d.mdzsno as cfh,'' cflsh,y.b5item yyxmbh, y.b5name yyxmmc, d.mdtiwe dj, d.mddays sl, (d.mdtiwe*d.mddays) je, d.mdzsks ksno, d.mdempn ysdm, 0 as fs,'0' as sfbz,'2' as sfxmzl
                                    from mzd3d d 
                                    left join (select distinct mccfno, mcghno from mzcfd) mzcf on mccfno = mdcfno
                                    left join bz09d on b9mbno = mdtwid 
                                    left join bz05d y on b5item = b9item 
                                    where mdtiwe > 0 and d.mdzsno in ({1})
                                    union all
                                    select d.mdzsno as cfh,'' cflsh,y.b5item yyxmbh, y.b5name yyxmmc, d.mdpric dj, d.mdtims sl, (d.mdpric*d.mdtims) je, d.mdzsks ksno, d.mdempn ysdm, 0 as fs,'0' as sfbz,'2' as sfxmzl
                                    from mzd3d d 
                                    left join bz09d on b9mbno = mdwayid 
                                    left join bz05d y on b5item = b9item
                                    left join (select distinct mccfno, mcghno from mzcfd) mzcf on mccfno = mdcfno 
                                    where mdzsno in ({1})
                                    union all
                                    select d.mdzsno as cfh,'' cflsh,y.b5item yyxmbh, y.b5name yyxmmc, d.mdppri dj, d.mdpqty sl, (d.mdppri*d.mdpqty) je, d.mdzsks ksno, d.mdempn ysdm, 0 as fs,'0' as sfbz,'2' as sfxmzl
                                    from mzd3d d 
                                    left join bz09d on b9mbno = mdpprid 
                                    left join bz05d y on b5item = b9item
                                    left join (select distinct mccfno, mcghno from mzcfd) mzcf on mccfno = mdcfno
                                    where mdpqty > 0 and d.mdzsno in ({1})
                                    ) m left join bz01h n on m.ysdm = n.b1empn 
                                    left join bz02d o on m.ksno = o.b2ejks
                                    left join ybhisdzdr p on p.hisxmbh=m.yyxmbh
                                    group by  m.dj, m.yyxmbh, m.yyxmmc, m.ysdm, n.b1name, m.ksno, o.b2ejnm, m.cfh,m.cflsh,m.sfxmzl,m.fs,m.sfbz,p.ybxmbh", jzlsh, cfhs);
            ds.Tables.Clear();
            ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            if (ds.Tables[0].Rows.Count == 0)
                return new object[] { 0, 0, "无处方信息" };
            decimal scfy=0;
            int index = 0;
            List<string> li_Param = new List<string>();
            StringBuilder inputParam = new StringBuilder();
            string ysbm = "";
            string ysxm = ""; 
            string ksbm = "";
            string ksmc = "";
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                string cfh = dr["cfh"].ToString();      //处方号
                string cflsh = dr["cflsh"].ToString();  //处方流水号
                if (string.IsNullOrEmpty(cflsh))
                    cflsh = index++.ToString();
                string sfxmzl = dr["sfxmzl"].ToString(); //收费项目种类
                string yyxmbm = dr["yyxmbh"].ToString();    //医院收费项目自编码
                string yyxmmc = dr["yyxmmc"].ToString();    //医院收费项目名称
                string dj = dr["dj"].ToString();    //单价
                string sl = dr["sl"].ToString();    //数量
                string je = dr["je"].ToString();    //金额
                string fs ="";    //中药饮片副数
                if (!dr["fs"].ToString().Equals("0"))
                    fs = dr["fs"].ToString();
                ysbm = dr["ysdm"].ToString();    //医生编码
                ysxm = dr["ysxm"].ToString();    //医生姓名
                ksbm = dr["ksno"].ToString();    //科室编码
                ksmc = dr["zxks"].ToString();    //科室名称
                string sfbz = dr["sfbz"].ToString();    //按最小计价单位收费标志
                bzbm = "";//病种编码
                string by3 = "";    //事务处理类别
                string by4 = "";    //处方医院编号
                string by5 = "";    //外配处方号
                string by6 = "";    //备用6
                string gjbm = dr["ybxmbh"].ToString();    //国家编码
                #region 获取定岗医生信息
                string ysgjm = string.Empty;
                strSql = string.Format(@"select * from ybdgyszd where ysbm='{0}'", ysbm);
                ds.Tables.Clear();
                ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    ysgjm = ds.Tables[0].Rows[0]["dgysbm"].ToString();
                }
                #endregion

                scfy+=Convert.ToDecimal(je);

                #region 参数
                inputParam.Clear();
                inputParam.Append(ybjzlsh + "|");
                inputParam.Append(sfxmzl + "|");
                inputParam.Append(cfh + "|");
                inputParam.Append(cflsh + "|");
                inputParam.Append(Convert.ToDateTime(kbrq1).ToString("yyyyMMddHHmmss") + "|");
                inputParam.Append(yyxmbm + "|");
                inputParam.Append(dj + "|");
                inputParam.Append(sl + "|");
                inputParam.Append(je + "|");
                inputParam.Append(fs + "|");
                inputParam.Append(ysbm + "|");
                inputParam.Append(ksbm + "|");
                inputParam.Append(jbr + "|");
                inputParam.Append(sfbz + "|");
                inputParam.Append(bzbm + "|");
                inputParam.Append(by3 + "|");
                inputParam.Append(by4 + "|");
                inputParam.Append(by5 + "|");
                inputParam.Append(by6 + "|");
                inputParam.Append(YLJGGJM + "|"); //20200501增加
                inputParam.Append(gjbm + "|");
                inputParam.Append(by5 + "|");
                inputParam.Append("" + "|$");
                //inputParam.Append(ysgjm + "|$");
                li_Param.Add(inputParam.ToString());
                #endregion

                strSql = string.Format(@"insert into ybcfmxscindr(jzlsh,jylsh,sfxmzl,ybcfh,djlsh,cfrq,yysfxmbm,yysfxmmc,dj,sl,
						                 je,ysbm,ksbh,jbr,sflb,grbh,xm,kh,cxbz,sysdate,ybjzlsh) values(
                                         '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}',
                                         '{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}','{20}')",
                                         jzlsh, JYLSH, sfxmzl, cfh, cflsh, kbrq1, yyxmbm, yyxmmc, dj, sl,
                                         je, ysbm, ksbm, jbr, sfbz, grbh, xm, kh, 1, sysdate, ybjzlsh);
                liSQL.Add(strSql);
            }
            #endregion

            #region 补差操作
            //            WriteLog(sysdate + "医保补差  |" + sfje + "|" + scfy + "|" + Math.Round((scfy - sfje)) + "|" + Math.Abs(Math.Round((scfy - sfje), 4)));
//            if (Math.Abs(Math.Round((scfy - sfje), 4)) > 0)
//            {
//                string cfh = "Z1" + DateTime.Now.ToString("yyyyMMddHHmmss");
//                string cflsh = (ds.Tables[0].Rows.Count + 1).ToString();
//                inputParam.Clear();
//                inputParam.Append(ybjzlsh + "|");
//                inputParam.Append("2|");
//                inputParam.Append(cfh + "|");
//                inputParam.Append(cflsh + "|");
//                inputParam.Append(DateTime.Now.ToString("yyyyMMddHHmmss") + "|");
//                inputParam.Append("800000000197|");
//                inputParam.Append(Math.Round((sfje - scfy), 4) + "|");
//                inputParam.Append(1 + "|");
//                inputParam.Append(Math.Round((sfje - scfy), 4) + "|");
//                inputParam.Append("|");
//                inputParam.Append(ysbm + "|");
//                inputParam.Append(ksbm + "|");
//                inputParam.Append(jbr + "|");
//                inputParam.Append("|");
//                inputParam.Append(bzbm + "|");
//                inputParam.Append("|");
//                inputParam.Append("|");
//                inputParam.Append("|");
//                inputParam.Append("|$");
//                li_Param.Add(inputParam.ToString());
//                strSql = string.Format(@"insert into ybcfmxscindr(jzlsh,jylsh,sfxmzl,ybcfh,djlsh,cfrq,yysfxmbm,yysfxmmc,dj,sl,
//						                   je,ysbm,ksbh,jbr,sflb,grbh,xm,kh,cxbz,sysdate,ybjzlsh) values(
//                                           '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}',
//                                           '{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}','{20}')",
//                                        jzlsh, JYLSH, "2", cflsh, cflsh, kbrq1, "800000000197", "疾病健康教育", Math.Round((sfje - scfy), 4), 1,
//                                        Math.Round((sfje - scfy), 4), ysbm, ksbm, jbr, "", grbh, xm, kh, 1, sysdate, ybjzlsh);
//                liSQL.Add(strSql);
            //            }
            #endregion

            if (Math.Abs(scfy - sfje) > 1)
                return new object[] { 0, 0, "收费金额与医保结算金额相差" + Math.Abs(scfy - sfje) + ",无法结算，请核实!" };

            //入参
            YWBH = "2310";
            int index1 = 0;
            StringBuilder inputParam1 = new StringBuilder();
            foreach (string param in li_Param)
            {
                if (index1 < 30)
                {
                    inputParam1.Append(param);
                    index1++;
                }
                else
                {
                    StringBuilder inputData = new StringBuilder();
                    inputData.Append(YWBH + "^");
                    inputData.Append(YLGHBH + "^");
                    inputData.Append(CZYBH + "^");
                    inputData.Append(YWZQH + "^");
                    inputData.Append(JYLSH + "^");
                    inputData.Append(JRFS + "^");
                    inputData.Append(DKLX + "^");
                    inputData.Append(inputParam1.ToString().TrimEnd('$') + "^");

                    StringBuilder outputData = new StringBuilder(102400);
                    WriteLog(sysdate + "  进入门诊费用登记(分段上传)...");
                    WriteLog(sysdate + "  入参|" + inputParam1.ToString());
                    int i = BUSINESS_HANDLE(inputData, outputData);
                    if (i == 0)
                    {
                        //记录返回数据
                        WriteLog(sysdate + "  回参|" + outputData.ToString());
                        string[] sValue = outputData.ToString().Split('^')[1].TrimEnd('$').Split('$');
                        foreach (string sRow in sValue)
                        {
                            string[] s = sRow.Split('|');
                            string cfh_r = s[0];    //处方号
                            string cflsh_r = s[1];  //处方流水号
                            string cfrq_r = s[2];   //处方日期
                            string yyxmbm_r = s[3]; //医院收费项目自编码
                            string je_r = s[4];     //金额
                            string zfje_r = s[5];   //自付金额
                            string zlje_r = s[6];   //自理金额
                            string zfbl_r = s[7];   //自付比例
                            string zfsx_r = s[8];   //支付上限
                            string sfxmdj_r = s[9]; //收费项目等级
                            string smxx_r = s[10];  //说明信息
                            string by2_r = s[11];   //备用2
                            string by3_r = s[12];   //备用3
                            string by4_r = s[13];   //备用4
                            string by5_r = s[14];   //备用5
                            string by6_r = s[15];   //备用6

                            strSql = string.Format(@"insert into ybcfmxscfhdr(jzlsh,jylsh,yyxmdm,ybcfh,cfh,je,zfje,zlje,zlbl,qezfbz,
                                                sfxmdj,yyxmmc,grbh,xm,kh,cxbz,sysdate,ybjzlsh ) values(
                                                '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}',
                                                '{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}')",
                                                jzlsh, JYLSH, yyxmbm_r, cfh_r, cflsh_r, je_r, zfje_r, zlje_r, zfbl_r, zfsx_r,
                                                sfxmdj_r, smxx_r, grbh, xm, kh, 1, sysdate, ybjzlsh);
                            liSQL.Add(strSql);
                        }

                        index1 = 1;
                        inputParam1.Remove(0, inputParam1.Length);
                        inputParam1.Append(param);
                    }
                    else
                    {
                        WriteLog(sysdate + "  门诊费用登记失败|" + outputData.ToString());
                        return new object[] { 0, 0, "门诊费用登记失败" + outputData.ToString() };
                    }

                }
            }

            if (index1 > 0)
            {
                StringBuilder inputData = new StringBuilder();
                inputData.Append(YWBH + "^");
                inputData.Append(YLGHBH + "^");
                inputData.Append(CZYBH + "^");
                inputData.Append(YWZQH + "^");
                inputData.Append(JYLSH + "^");
                inputData.Append(JRFS + "^");
                inputData.Append(DKLX + "^");
                inputData.Append(inputParam1.ToString().TrimEnd('$') + "^");

                StringBuilder outputData = new StringBuilder(102400);
                WriteLog(sysdate + "  进入门诊费用登记(一次性上传、补传)...");
                WriteLog(sysdate + "  入参|" + inputParam1.ToString());
                int i = BUSINESS_HANDLE(inputData, outputData);
                if (i == 0)
                {

                    //记录返回数据
                    WriteLog(sysdate + "  回参|" + outputData.ToString());
                    string[] sValue = outputData.ToString().Split('^')[1].TrimEnd('$').Split('$');
                    foreach (string sRow in sValue)
                    {
                        string[] s = sRow.Split('|');
                        string cfh_r = s[0];    //处方号
                        string cflsh_r = s[1];  //处方流水号
                        string cfrq_r = s[2];   //处方日期
                        string yyxmbm_r = s[3]; //医院收费项目自编码
                        string je_r = s[4];     //金额
                        string zfje_r = s[5];   //自付金额
                        string zlje_r = s[6];   //自理金额
                        string zfbl_r = s[7];   //自付比例
                        string zfsx_r = s[8];   //支付上限
                        string sfxmdj_r = s[9]; //收费项目等级
                        string smxx_r = s[10];  //说明信息
                        string by2_r = s[11];   //备用2
                        string by3_r = s[12];   //备用3
                        string by4_r = s[13];   //备用4
                        string by5_r = s[14];   //备用5
                        string by6_r = s[15];   //备用6

                        strSql = string.Format(@"insert into ybcfmxscfhdr(jzlsh,jylsh,yyxmdm,ybcfh,cfh,je,zfje,zlje,zlbl,qezfbz,
                                                sfxmdj,yyxmmc,grbh,xm,kh,cxbz,sysdate,ybjzlsh ) values(
                                                '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}',
                                                '{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}')",
                                                jzlsh, JYLSH, yyxmbm_r, cfh_r, cflsh_r, je_r, zfje_r, zlje_r, zfbl_r, zfsx_r,
                                                sfxmdj_r, smxx_r, grbh, xm, kh, 1, sysdate, ybjzlsh);
                        liSQL.Add(strSql);
                    }

                }
                else
                {
                    WriteLog(sysdate + "  门诊费用登记失败|" + outputData.ToString());
                    return new object[] { 0, 0, "门诊费用登记失败" + outputData.ToString() };
                }
            }

            object[] obj = liSQL.ToArray();
            obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);
            if (obj[1].ToString().Equals("1"))
            {
                WriteLog(sysdate + "  门诊费用登记成功|");
                return new object[] { 0, 1, JYLSH };
            }
            else
            {
                WriteLog(sysdate + "  门诊费用登记失败|数据操作失败|" + obj[2].ToString());
                //门诊费用上传撤销
                object[] objParam1 = { jzlsh, "", "", jbr, ybjzlsh };
                objParam1 = N_YBMZZYSFDJCX(objParam1);
                return new object[] { 0, 0, "门诊费用登记失败|数据操作失败|" + obj[2].ToString() };
            }

        }
        #endregion

        #region 门诊费用登记撤销(门诊处方明细上传撤销)
        public static object[] YBMZFYDJCX(object[] objParam)
        {
            string sysdate = GetServerDateTime();
            string jzlsh = objParam[0].ToString();

            if(string.IsNullOrEmpty(jzlsh))
                return new object[] { 0, 0, "就诊流水号不能为空" };

            YWBH = "2320";
            JYLSH = DateTime.Now.ToString("yyyyMMddHHmmss") + "-" + YLGHBH + "-" + new Random().Next(100).ToString().PadLeft(4, '0');
            CZYBH = CliUtils.fLoginUser;
            YWZQH = CliUtils.fLoginYbNo.Split('|')[0];
            string jbr = CliUtils.fUserName;
            try
            {
                //获取上传处方信息
                string strSql = string.Format(@"select distinct jylsh,ybjzlsh from ybcfmxscindr where jzlsh='{0}' and cxbz=1 
                                            and jylsh not in(select isnull(cfmxjylsh,'') as cfjylsh from ybfyjsdr where cxbz=1 and jzlsh='{0}')"
                                                , jzlsh);
                DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                if (ds.Tables[0].Rows.Count == 0)
                {
                    WriteLog(sysdate + "  该患者未上传费用信息或已完成结算");
                    return new object[] { 0, 0, "该患者未上传费用信息或已完成结算" };
                }
                string cfjylsh = ds.Tables[0].Rows[0]["jylsh"].ToString();
                string ybjzlsh = ds.Tables[0].Rows[0]["ybjzlsh"].ToString();
                string cfh = "";
                string cflsh = "";

                //入参数据
                StringBuilder inputParam = new StringBuilder();
                inputParam.Append(ybjzlsh + "|");
                inputParam.Append(cfh + "|");
                inputParam.Append(cflsh + "|");
                inputParam.Append(jbr + "|");

                //入参
                StringBuilder inputData = new StringBuilder();
                inputData.Append(YWBH + "^");
                inputData.Append(YLGHBH + "^");
                inputData.Append(CZYBH + "^");
                inputData.Append(YWZQH + "^");
                inputData.Append(JYLSH + "^");
                inputData.Append(JRFS + "^");
                inputData.Append(DKLX + "^");
                inputData.Append(inputParam + "^");

                WriteLog(sysdate + "  进入门诊费用登记撤销...");
                WriteLog(sysdate + " 入参|" + inputData.ToString());

                StringBuilder outputData = new StringBuilder(1024);
                int i = BUSINESS_HANDLE(inputData, outputData);

                List<string> liSQL = new List<string>();
                if (i == 0)
                {
                    strSql = string.Format(@"insert into ybcfmxscindr(jzlsh,jylsh,sfxmzl,ybcfh,djlsh,cfrq,yysfxmbm,yysfxmmc,dj,sl,
						                 je,ysbm,ksbh,jbr,sflb,grbh,xm,kh,ybjzlsh,cxbz,sysdate) select 
                                         jzlsh,jylsh,sfxmzl,ybcfh,djlsh,cfrq,yysfxmbm,yysfxmmc,dj,sl,
						                 je,ysbm,ksbh,'{2}',sflb,grbh,xm,kh,ybjzlsh,0,'{3}' from ybcfmxscindr where jzlsh='{0}' and jylsh='{1}' and cxbz=1",
                                             jzlsh, cfjylsh, jbr, sysdate);
                    liSQL.Add(strSql);
                    strSql = string.Format(@"update ybcfmxscindr set cxbz=2 where jzlsh='{0}' and jylsh='{1}' and cxbz=1", jzlsh, cfjylsh);
                    liSQL.Add(strSql);

                    strSql = string.Format(@"insert into ybcfmxscfhdr(jzlsh,jylsh,yyxmdm,ybcfh,cfh,je,zfje,zlje,zlbl,qezfbz,
                                         sfxmdj,yyxmmc,grbh,xm,kh,ybjzlsh,cxbz,sysdate ) select
                                         jzlsh,jylsh,yyxmdm,ybcfh,cfh,je,zfje,zlje,zlbl,qezfbz,
                                         sfxmdj,yyxmmc,grbh,xm,kh,ybjzlsh,0,'{2}' from ybcfmxscfhdr where jzlsh='{0}' and jylsh='{1}' and cxbz=1",
                                             jzlsh, cfjylsh, sysdate);
                    liSQL.Add(strSql);
                    strSql = string.Format(@"update ybcfmxscfhdr set cxbz=2 where jzlsh='{0}' and jylsh='{1}' and cxbz=1", jzlsh, cfjylsh);
                    liSQL.Add(strSql);

                    object[] obj = liSQL.ToArray();
                    obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);
                    if (obj[1].ToString().Equals("1"))
                    {
                        WriteLog(sysdate + "  门诊费用登记撤销成功|" + outputData.ToString());
                        return new object[] { 0, 1, "门诊费用登记撤销成功" };
                    }
                    else
                    {
                        WriteLog(sysdate + "  门诊费用登记撤销失败|数据操作失败|" + obj[2].ToString());
                        return new object[] { 0, 0, "门诊费用登记撤销失败|数据操作失败|" + obj[2].ToString() };
                    }
                }
                else
                {
                    WriteLog(sysdate + "  门诊费用登记撤销失败|" + outputData.ToString());
                    return new object[] { 0, 0, "门诊费用登记撤销失败|数据操作失败|" + outputData.ToString() };
                }
            }
            catch (Exception ex)
            {
                WriteLog(sysdate + "  门诊费用登记撤销异常" + ex.Message);
                return new object[] { 0, 0, "门诊费用登记撤销异常" + ex.Message };
            }
        }
        #endregion

        #region 门诊费用预结算
        public static object[] YBMZSFYJS(object[] objParam)
        {
            string sysdate = GetServerDateTime();//系统时间
            string jzlsh = objParam[0].ToString();      // 就诊流水号
            string djh = "0000";        // 单据号
            string zhsybz = objParam[1].ToString();     // 账户使用标志（0或1） 此处为是否为挂号费结算
            string dqrq = objParam[2].ToString();  // 结算时间
            string bzbm = objParam[3].ToString(); //病种编码
            string bzmc = objParam[4].ToString(); //病种名称
            string cfhs = objParam[5].ToString();   //处方号集
            string yllb = objParam[6].ToString(); //医疗类别
            string ylfhj1 = objParam[7].ToString(); //医疗费合计 (新增)

            string jbr = CliUtils.fUserName;    //经办人
            string jbrbh = CliUtils.fLoginUser; //经办人编码

            try
            {
                if (string.IsNullOrEmpty(jzlsh))
                    return new object[] { 0, 0, "就诊流水号不能为空" };
                if (string.IsNullOrEmpty(bzbm))
                    return new object[] { 0, 0, "病种编码不能为空" };
                if (string.IsNullOrEmpty(yllb))
                    return new object[] { 0, 0, "医疗类别不能为空" };

                WriteLog(zhsybz + "|" + zhsybz + "|" + dqrq + "|" + bzbm + "|" + bzmc + "|" + cfhs + "|" + yllb + "|" + ylfhj1);
                string strSql = string.Format(@"select * from ybmzzydjdr where jzlsh='{0}' and cxbz=1", jzlsh);
                DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                if (ds.Tables[0].Rows.Count == 0)
                    return new object[] { 0, 0, "该患者未医保门诊登记" };
                yllb = ds.Tables[0].Rows[0]["yllb"].ToString(); //医疗类别
                string xm = ds.Tables[0].Rows[0]["xm"].ToString(); //姓名
                string kh = ds.Tables[0].Rows[0]["kh"].ToString();  //社会保障卡号
                string ksbm = ds.Tables[0].Rows[0]["ksbh"].ToString();  //科室编码
                string grbh = ds.Tables[0].Rows[0]["grbh"].ToString(); //个人编号
                string ysbm = ds.Tables[0].Rows[0]["ysdm"].ToString(); //医生代码
                string[] syllb = { "11", "13", "86" };
                if (!syllb.Contains(yllb))
                    bzbm = ds.Tables[0].Rows[0]["bzbm"].ToString();  //病种编码
                string ybjzlsh = ds.Tables[0].Rows[0]["ybjzlsh"].ToString();

                //获取处方医生
                strSql = string.Format(@"select distinct m3empn from mz03d where m3ghno='{0}' and m3cfno in ({1})", jzlsh, cfhs);
                ds.Tables.Clear();
                ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);

                if (ds.Tables[0].Rows.Count == 0)
                    return new object[] { 0, 0, "未获取到处方医生信息,不能进行预结算操作" };
                ysbm = ds.Tables[0].Rows[0]["m3empn"].ToString();  //医生编码

                #region 费用上传
                //门诊费用登记
                object[] objMZFYDJ = { jzlsh, cfhs, ylfhj1, "", dqrq };
                objMZFYDJ = YBMZFYDJ(objMZFYDJ);
                if (!objMZFYDJ[1].ToString().Equals("1"))
                    return new object[] { 0, 0, objMZFYDJ[2].ToString() };
                string cfjylsh = objMZFYDJ[2].ToString();
                #endregion

                YWBH = "2420";
                JYLSH = DateTime.Now.ToString("yyyyMMddHHmmss") + "-" + YLGHBH + "-" + new Random().Next(100).ToString().PadLeft(4, '0');
                CZYBH = CliUtils.fLoginUser;
                YWZQH = CliUtils.fLoginYbNo.Split('|')[0];

                #region 获取病历本费、卡费
                decimal xjzf_gh = 0;
                //if (sfghjs.Equals("1"))
                //{
                //    //string zcfbz = "g";
                //    strSql = string.Format(@"select m1blam,m1jzam from mz01h where m1ghno='{0}'", jzlsh);
                //    ds.Tables.Clear();
                //    ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                //    if (ds.Tables[0].Rows.Count == 0)
                //        return new object[] { 0, 0, "无挂号信息" };
                //    decimal blbfy = Convert.ToDecimal(ds.Tables[0].Rows[0]["m1blam"].ToString()); //病历本费
                //    decimal jzkfy = Convert.ToDecimal(ds.Tables[0].Rows[0]["m1jzam"].ToString()); //就诊卡费
                //    xjzf_gh = blbfy + jzkfy;

                //}
                #endregion

                #region 入参赋值
                string jsrq = Convert.ToDateTime(sysdate).ToString("yyyyMMddHHmmss");   //结算日期
                string cyrq = Convert.ToDateTime(dqrq).ToString("yyyyMMddHHmmss");  //出院日期
                string cyyy = "";       //出院原因
                string cyzdbm = bzbm;     //出院诊断疾病编码
                string yjslb = "";      //月结算类别
                string ztjsbz = "0";     //中途结算标志
                string fmrq = "";   //分娩日期
                string cs = "";     //产次
                string tes = "";    //胎儿数
                string zybh = "";   //转院医院编号
                string zsekh = "";  //准生儿社会保障卡号
                string ssbz = "";   //手术是否成功标志
                string sfghjs = "0";    //是否为挂号费结算
                #region 获取定岗医生信息
                string ysgjm = string.Empty;
                strSql = string.Format(@"select * from ybdgyszd where ysbm='{0}'", ysbm);
                ds.Tables.Clear();
                ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    ysgjm = ds.Tables[0].Rows[0]["dgysbm"].ToString();
                }
                #endregion
                //入参数据
                StringBuilder inputParam = new StringBuilder();
                inputParam.Append(ybjzlsh + "|");
                inputParam.Append(djh + "|");
                inputParam.Append(yllb + "|");
                inputParam.Append(jsrq + "|");
                inputParam.Append(cyrq + "|");
                inputParam.Append(cyyy + "|");
                inputParam.Append(cyzdbm + "|");
                inputParam.Append(yjslb + "|");
                inputParam.Append(ztjsbz + "|");
                inputParam.Append(jbr + "|");
                inputParam.Append(fmrq + "|");
                inputParam.Append(cs + "|");
                inputParam.Append(tes + "|");
                inputParam.Append(kh + "|");
                inputParam.Append(zybh + "|");
                inputParam.Append(ksbm + "|");
                inputParam.Append(ysbm + "|");
                inputParam.Append(sfghjs + "|");
                inputParam.Append(zsekh + "|");
                inputParam.Append("" + "|");//二维码
                inputParam.Append(YLJGGJM + "|");//医疗机构国家码
                inputParam.Append("" + "|");//出院手术编码
                inputParam.Append("" + "|");//护士国家码
                //inputParam.Append(ysgjm + "|");//药师国家码
                inputParam.Append("" + "|");//药师国家码
                inputParam.Append("" + "|");//中医疾病分类代码
                inputParam.Append("" + "|");//中医证候分类代码

                //入参
                StringBuilder inputData = new StringBuilder();
                inputData.Append(YWBH + "^");
                inputData.Append(YLGHBH + "^");
                inputData.Append(CZYBH + "^");
                inputData.Append(YWZQH + "^");
                inputData.Append(JYLSH + "^");
                inputData.Append(JRFS + "^");
                inputData.Append(DKLX + "^");
                inputData.Append(inputParam + "^");
                #endregion

                StringBuilder outputData = new StringBuilder(102400);
                WriteLog(sysdate + "  进入门诊费用预结算...");
                WriteLog(sysdate + "  入参|" + inputData.ToString());

                int i = BUSINESS_HANDLE(inputData, outputData);
                if (i == 0)
                {
                    WriteLog(sysdate + "  门诊费用预结算成功|" + outputData.ToString());
                    #region 出参赋值
                    string[] str = outputData.ToString().Split('^')[1].Split('|');
                    decimal ylzfy = Convert.ToDecimal(str[0]);         //医疗总费用
                    decimal zbxje = Convert.ToDecimal(0.00);         //总报销金额
                    decimal tcjjzf = Convert.ToDecimal(str[1]);        //统筹基金支付
                    decimal dejjzf = Convert.ToDecimal(str[2]);        //本次大病救助支付
                    decimal dbbyzf = Convert.ToDecimal(str[3]);         //本次大病保险支付  
                    decimal mzfdfy = Convert.ToDecimal(str[4]);       //本次民政补助支付
                    decimal zhzf = Convert.ToDecimal(str[5]);          //本次帐户支付总额
                    decimal xjzf = Convert.ToDecimal(str[6]);          //本次现金支付总额

                    decimal zhzfzf = Convert.ToDecimal(str[7]);          //本次帐户支付自付
                    decimal zhzfzl = Convert.ToDecimal(str[8]);          //本次帐户支付自理
                    decimal xjzfzf = Convert.ToDecimal(str[9]);          //本次帐户支付自付
                    decimal xjzfzl = Convert.ToDecimal(str[10]);          //本次帐户支付自理
                    decimal ybfwnfy = Convert.ToDecimal(str[11]);       //医保范围内费用
                    decimal bcjsqzhye = Convert.ToDecimal(str[12]);       //本次结算前帐户余额
                    string dbzbm = str[13];        //单病种病种编码
                    string smxx = str[14];          //说明信息
                    decimal yfhj = Convert.ToDecimal(str[15]);       //药费合计
                    decimal zlxmfhj = Convert.ToDecimal(str[16]);       //诊疗项目费合计
                    decimal bbzf = Convert.ToDecimal(str[17]);       //补保支付
                    string yllb_r = str[18];    //医疗类别
                    string by6 = str[19];       //备用6

                    decimal gwybzjjzf = Convert.ToDecimal(0.00);     //公务员补助基金支付
                    decimal qybcylbxjjzf = Convert.ToDecimal(0.00);  //企业补充医疗保险基金支付
                    decimal dwfdfy = Convert.ToDecimal(0.00);        //单位负担费用    
                    decimal yyfdfy = Convert.ToDecimal(0.00);       //医院负担费用
                    //decimal mzfdfy = Convert.ToDecimal("0.00");       //民政负担费用
                    decimal cxjfy = Convert.ToDecimal(0.00);        //超限价费用单病种病种编码
                    decimal ylzlfy = Convert.ToDecimal(0.00);       //乙类自理费用
                    decimal blzlfy = Convert.ToDecimal(0.00);       //丙类自理费用
                    decimal fhjbylfy = Convert.ToDecimal(0.00);     //符合基本医疗费用
                    decimal qfbzfy = Convert.ToDecimal(0.00);       //起付标准费用
                    decimal zzzyzffy = Convert.ToDecimal(0.00);     //转诊转院自付费用
                    decimal jrtcfy = Convert.ToDecimal(0.00);       //进入统筹费用
                    decimal tcfdzffy = Convert.ToDecimal(0.00);     //统筹分段自付费用
                    decimal ctcfdxfy = Convert.ToDecimal(0.00);       //超统筹封顶线费用
                    decimal jrdebsfy = Convert.ToDecimal(0.00);       //进入大额报销费用
                    decimal defdzffy = Convert.ToDecimal(0.00);       //大额分段自付费用
                    decimal cdefdxfy = Convert.ToDecimal(0.00);       //超大额封顶线费用
                    decimal rgqgzffy = Convert.ToDecimal(0.00);       //人工器管自付费用
                    //decimal bcjsqzfye = Convert.ToDecimal(0.00)       //本次结算前帐户余额
                    decimal bntczflj = Convert.ToDecimal(0.00);       //本年统筹支付累计
                    decimal bndezflj = Convert.ToDecimal(0.00);       //本年大额支付累计
                    decimal bnczjmmztczflj = Convert.ToDecimal(0.00);       //本年城镇居民门诊统筹支付累计
                    decimal bngwybzzflj = Convert.ToDecimal(0.00);  //本年公务员补助支付累计(不含本次)
                    decimal bnzhzflj = Convert.ToDecimal(0.00);  //本年账户支付累计(不含本次)
                    string bnzycslj = "1";  //本年住院次数累计(不含本次)
                    string zycs = "1";        //住院次数
                    string yldylb = ""; //医疗待遇类别
                    string jbjgbm = ""; //经办机构编码
                    #endregion

                    zbxje = ylzfy - xjzf;
                    //医保补差金额
                    decimal ybbcfy = Convert.ToDecimal(ylzfy) - Convert.ToDecimal(ylfhj1);
                    

                    /*
                         *医疗费总额|总报销金额|统筹基金支付|大额基金支付|账户支付|
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
                         * 居民个人自付二次补偿金额|体检金额|生育基金支付|本次民政补助支付|医保范围内费用|
                         * 医保类型|医保补差|
                         */

                    #region 医保类型
                    string yblx = string.Empty;
                    strSql = string.Format(@"select NAME from YBXMLBZD where LBMC='医疗类别' and CODE='{0}'", yllb_r);
                    ds.Tables.Clear();
                    ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                    if (ds.Tables[0].Rows.Count != 0)
                    {
                        yblx = ds.Tables[0].Rows[0]["NAME"].ToString();
                    }

                    strSql = string.Format(@"select LEFT(ylrylb,1) as ylrylb from ybickxx where grbh='{0}'", grbh);
                    ds.Tables.Clear();
                    ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                    if (ds.Tables[0].Rows.Count != 0)
                    {
                        string[] sValue = { "1", "2", "3" };
                        if (sValue.Contains(ds.Tables[0].Rows[0]["ylrylb"].ToString()))
                            yblx += "(职工)";
                        else
                            yblx += "(居民)";
                    }
                    #endregion

                    string strValue = ylzfy + "|" + zbxje + "|" + tcjjzf + "|" + dejjzf + "|" + zhzf + "|" +
                                      xjzf + "|" + gwybzjjzf + "|" + qybcylbxjjzf + "|" + "0.00" + "|" + dwfdfy + "|" +
                                      yyfdfy + "|" + mzfdfy + "|" + cxjfy + "|" + ylzlfy + "|" + blzlfy + "|" +
                                      fhjbylfy + "|" + qfbzfy + "|" + zzzyzffy + "|" + jrtcfy + "|" + tcfdzffy + "|" +
                                      ctcfdxfy + "|" + "0.00" + "|" + defdzffy + "|" + cdefdxfy + "|" + rgqgzffy + "|" +
                                      bcjsqzhye + "|" + bntczflj + "|" + bndezflj + "|" + bnczjmmztczflj + "|" + bngwybzzflj + "|" +
                                      bnzhzflj + "|" + bnzycslj + "|" + zycs + "|" + xm + "|" + jsrq + "|" +
                                      yllb_r + "||" + YLGHBH + "|" + YWZQH + "|" + djh + "|" +
                                      "|" + djh + "||" + JYLSH + "|1|" +
                                      "|" + YLGHBH + "|0.00|||" +
                                      grbh + "|0.00|0.00|0.00|0.00|" +
                                      "0.00|0.00|0.00|" + dbbyzf + "|" + ybfwnfy + "|" +
                                      yblx + "|" + ybbcfy;
                    WriteLog(sysdate + "  门诊费用预结算返回参数|" + strValue);
                    return new object[] { 0, 1, strValue };
                }
                else
                {
                    WriteLog(sysdate + "  门诊费用预结算失败" + outputData.ToString());
                    //撤销上传费用
                    object[] objMZFYDJCX = { jzlsh };
                    YBMZFYDJCX(objMZFYDJCX);
                    return new object[] { 0, 0, outputData.ToString() };
                }
            }
            catch (Exception ex)
            {
                WriteLog(sysdate + "  门诊费用预结算异常|"+ex.Message);
                return new object[] { 0, 0, "门诊费用预结算异常|" + ex.Message };
            }
        }
        #endregion

        #region 门诊费用结算
        public static object[] YBMZSFJS(object[] objParam)
        {
            string sysdate = GetServerDateTime();//系统时间
            string jzlsh = objParam[0].ToString();      // 就诊流水号
            string djh = objParam[1].ToString();        // 单据号
            string zhsybz = objParam[2].ToString();     // 账户使用标志（0或1） 
            string dqrq = objParam[3].ToString();  // 结算时间
            string bzbm = objParam[4].ToString(); //病种编码
            string bzmc = objParam[5].ToString(); //病种名称
            string cfhs = objParam[6].ToString();   //处方号集
            string yllb = objParam[7].ToString(); //医疗类别
            string ylfhj1 = objParam[8].ToString(); //医疗费合计 (新增)
            string sfghjs = "0"; //是否挂号费结算
            string cfjylsh = "";// objParam[10].ToString(); //处方交易流水号

            if (string.IsNullOrEmpty(jzlsh))
                return new object[] { 0, 0, "就诊流水号不能为空" };
            if (string.IsNullOrEmpty(djh))
                return new object[] { 0, 0, "单据号不能为空" };
            if (string.IsNullOrEmpty(bzbm))
                return new object[] { 0, 0, "病种编码不能为空" };
            if (string.IsNullOrEmpty(yllb))
                return new object[] { 0, 0, "医疗类别不能为空" };
            try
            {
                YWBH = "2410";
                JYLSH = DateTime.Now.ToString("yyyyMMddHHmmss") + "-" + YLGHBH + "-" + new Random().Next(100).ToString().PadLeft(4, '0');
                CZYBH = CliUtils.fLoginUser;
                YWZQH = CliUtils.fLoginYbNo.Split('|')[0];

                #region 获取变量信息
                string strSql = string.Format(@"select * from ybmzzydjdr where jzlsh='{0}' and cxbz=1", jzlsh);
                DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                if (ds.Tables[0].Rows.Count == 0)
                    return new object[] { 0, 0, "该患者未医保门诊登记" };
                yllb = ds.Tables[0].Rows[0]["yllb"].ToString(); //医疗类别
                string kh = ds.Tables[0].Rows[0]["kh"].ToString();  //社会保障卡号
                string ksbm = ds.Tables[0].Rows[0]["ksbh"].ToString();  //科室编码
                string xm = ds.Tables[0].Rows[0]["xm"].ToString();  //姓名
                string grbh = ds.Tables[0].Rows[0]["grbh"].ToString();  //医保编号
                string ysbm = ds.Tables[0].Rows[0]["ysdm"].ToString(); //医生代码

                string[] syllb = { "11", "13", "86" };
                if (!syllb.Contains(yllb))
                {
                    bzbm = ds.Tables[0].Rows[0]["bzbm"].ToString();  //病种编码
                    bzmc = ds.Tables[0].Rows[0]["bzmc"].ToString();     //病种名称
                }
                string ybjzlsh = ds.Tables[0].Rows[0]["ybjzlsh"].ToString(); //医保就诊流水号

                string jsrq = Convert.ToDateTime(sysdate).ToString("yyyyMMddHHmmss");   //结算日期
                string cyrq = Convert.ToDateTime(dqrq).ToString("yyyyMMddHHmmss");  //出院日期
                string cyyy = "";       //出院原因
                string cyzdbm = bzbm;     //出院诊断疾病编码
                string yjslb = "";      //月结算类别
                string ztjsbz = "0";     //中途结算标志
                string jbr = CliUtils.fUserName;    //经办人
                string jbrbh = CliUtils.fLoginUser; //经办人编码
                string fmrq = "";   //分娩日期
                string cs = "";     //产次
                string tes = "";    //胎儿数
                string zybh = "";   //转院医院编号
                string zsekh = "";  //准生儿社会保障卡号
                string ssbz = "";   //手术是否成功标志
                decimal xjzf_gh = 0;
                string zcfbz = "";
                #endregion

                //获取处方医生
                strSql = string.Format(@"select distinct m3empn from mz03d where m3ghno='{0}' and m3cfno in ({1})", jzlsh, cfhs);
                ds.Tables.Clear();
                ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);

                if (ds.Tables[0].Rows.Count == 0)
                    return new object[] { 0, 0, "未获取到处方医生信息,不能进行结算操作" };
                ysbm = ds.Tables[0].Rows[0]["m3empn"].ToString();  //医生编码
                
                if (sfghjs.Equals("1"))
                    ysbm = "";
                

                #region 获取病历本费、卡费
                //if (sfghjs.Equals("1"))
                //{
                //    zcfbz = "g";
                //    strSql = string.Format(@"select m1blam,m1jzam from mz01h where m1ghno='{0}'", jzlsh);
                //    ds.Tables.Clear();
                //    ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                //    if (ds.Tables[0].Rows.Count == 0)
                //        return new object[] { 0, 0, "无挂号信息" };
                //    decimal blbfy = Convert.ToDecimal(ds.Tables[0].Rows[0]["m1blam"].ToString()); //病历本费
                //    decimal jzkfy = Convert.ToDecimal(ds.Tables[0].Rows[0]["m1jzam"].ToString()); //就诊卡费
                //    xjzf_gh = blbfy + jzkfy;

                //}
                #endregion

                #region 获取定岗医生信息
                string ysgjm = string.Empty;
                strSql = string.Format(@"select * from ybdgyszd where ysbm='{0}'", ysbm);
                ds.Tables.Clear();
                ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    ysgjm = ds.Tables[0].Rows[0]["dgysbm"].ToString();
                }
                #endregion

                #region 入参赋值
                //入参数据
                StringBuilder inputParam = new StringBuilder();
                inputParam.Append(ybjzlsh + "|");
                inputParam.Append(djh + "|");
                inputParam.Append(yllb + "|");
                inputParam.Append(jsrq + "|");
                inputParam.Append(cyrq + "|");
                inputParam.Append(cyyy + "|");
                inputParam.Append(cyzdbm + "|");
                inputParam.Append(yjslb + "|");
                inputParam.Append(ztjsbz + "|");
                inputParam.Append(jbr + "|");
                inputParam.Append(fmrq + "|");
                inputParam.Append(cs + "|");
                inputParam.Append(tes + "|");
                inputParam.Append(kh + "|");
                inputParam.Append(zybh + "|");
                inputParam.Append(ksbm + "|");
                inputParam.Append(ysbm + "|");
                inputParam.Append(sfghjs + "|");
                inputParam.Append(zsekh + "|");
                inputParam.Append("" + "|");//二维码
                inputParam.Append(YLJGGJM + "|");//医疗机构国家码
                inputParam.Append("" + "|");//出院手术编码
                inputParam.Append("" + "|");//护士国家码
                //inputParam.Append(ysgjm + "|");//药师国家码
                inputParam.Append("" + "|");//药师国家码
                inputParam.Append("" + "|");//中医疾病分类代码
                inputParam.Append("" + "|");//中医证候分类代码

                //入参
                StringBuilder inputData = new StringBuilder();
                inputData.Append(YWBH + "^");
                inputData.Append(YLGHBH + "^");
                inputData.Append(CZYBH + "^");
                inputData.Append(YWZQH + "^");
                inputData.Append(JYLSH + "^");
                inputData.Append(JRFS + "^");
                inputData.Append(DKLX + "^");
                inputData.Append(inputParam + "^");
                #endregion

                StringBuilder outputData = new StringBuilder(10240);
                WriteLog(sysdate + "  进入门诊费用结算...");
                WriteLog(sysdate + "  入参|" + inputData.ToString());

                int i = BUSINESS_HANDLE(inputData, outputData);
                if (i == 0)
                {
                    WriteLog(sysdate + "  门诊费用结算成功|" + outputData.ToString());
                    #region 出参赋值
                    string[] str = outputData.ToString().Split('^')[1].Split('|');
                    decimal ylzfy = Convert.ToDecimal(str[0]);         //医疗总费用
                    decimal ylzfy1 = ylzfy + xjzf_gh;
                    decimal zbxje = Convert.ToDecimal(0.00);         //总报销金额
                    decimal tcjjzf = Convert.ToDecimal(str[1]);         //统筹基金支付
                    decimal dejjzf = Convert.ToDecimal(str[2]);         //本次大病救助支付
                    decimal dbbyzf = Convert.ToDecimal(str[3]);         //本次大病保险支付  
                    decimal mzfdfy = Convert.ToDecimal(str[4]);         //本次民政补助支付
                    decimal zhzf = Convert.ToDecimal(str[5]);          //本次帐户支付总额
                    decimal xjzf = Convert.ToDecimal(str[6]);//本次现金支付总额
                    decimal xjzf1 = xjzf + xjzf_gh;
                    decimal zhzfzf = Convert.ToDecimal(str[7]);          //本次帐户支付自付
                    decimal zhzfzl = Convert.ToDecimal(str[8]);          //本次帐户支付自理
                    decimal xjzfzf = Convert.ToDecimal(str[9]);          //本次帐户支付自付
                    decimal xjzfzl = Convert.ToDecimal(str[10]);          //本次帐户支付自理
                    decimal ybfwnfy = Convert.ToDecimal(str[11]);         //医保范围内费用
                    decimal bcjsqzhye = Convert.ToDecimal(str[12]) + zhzf;//本次结算前帐户余额
                    string dbzbm = str[13];        //单病种病种编码
                    string smxx = str[14];          //说明信息
                    decimal yfhj = Convert.ToDecimal(str[15]);       //药费合计
                    decimal zlxmfhj = Convert.ToDecimal(str[16]);       //诊疗项目费合计
                    decimal bbzf = Convert.ToDecimal(str[17]);       //补保支付
                    string yllb_r = str[18];    // 医疗类别
                    string by6 = str[19];       //备用6

                    decimal gwybzjjzf = Convert.ToDecimal(0.00);     //公务员补助基金支付
                    decimal qybcylbxjjzf = Convert.ToDecimal(0.00);  //企业补充医疗保险基金支付
                    decimal dwfdfy = Convert.ToDecimal(0.00);        //单位负担费用    
                    decimal yyfdfy = Convert.ToDecimal(0.00);       //医院负担费用
                    //decimal mzfdfy = Convert.ToDecimal("0.00");       //民政负担费用
                    decimal cxjfy = Convert.ToDecimal(0.00);        //超限价费用单病种病种编码
                    decimal ylzlfy = Convert.ToDecimal(0.00);       //乙类自理费用
                    decimal blzlfy = Convert.ToDecimal(0.00);       //丙类自理费用
                    decimal fhjbylfy = Convert.ToDecimal(0.00);     //符合基本医疗费用
                    decimal qfbzfy = Convert.ToDecimal(0.00);       //起付标准费用
                    decimal zzzyzffy = Convert.ToDecimal(0.00);     //转诊转院自付费用
                    decimal jrtcfy = Convert.ToDecimal(0.00);       //进入统筹费用
                    decimal tcfdzffy = Convert.ToDecimal(0.00);     //统筹分段自付费用
                    decimal ctcfdxfy = Convert.ToDecimal(0.00);       //超统筹封顶线费用
                    decimal jrdebsfy = Convert.ToDecimal(0.00);       //进入大额报销费用
                    decimal defdzffy = Convert.ToDecimal(0.00);       //大额分段自付费用
                    decimal cdefdxfy = Convert.ToDecimal(0.00);       //超大额封顶线费用
                    decimal rgqgzffy = Convert.ToDecimal(0.00);       //人工器管自付费用
                    //decimal bcjsqzfye = Convert.ToDecimal(0.00);       //本次结算前帐户余额
                    decimal bntczflj = Convert.ToDecimal(0.00);       //本年统筹支付累计
                    decimal bndezflj = Convert.ToDecimal(0.00);       //本年大额支付累计
                    decimal bnczjmmztczflj = Convert.ToDecimal(0.00);       //本年城镇居民门诊统筹支付累计
                    decimal bngwybzzflj = Convert.ToDecimal(0.00);  //本年公务员补助支付累计(不含本次)
                    decimal bnzhzflj = Convert.ToDecimal(0.00);  //本年账户支付累计(不含本次)
                    string bnzycslj = "1";  //本年住院次数累计(不含本次)
                    string zycs = "1";        //住院次数
                    string yldylb = ""; //医疗待遇类别
                    string jbjgbm = ""; //经办机构编码

                    #endregion

                    zbxje = ylzfy1 - xjzf;
                    //医保补差金额
                    decimal ybbcfy = Convert.ToDecimal(ylzfy) - Convert.ToDecimal(ylfhj1);

                    /*
                     *医疗费总额|总报销金额|统筹基金支付|大额基金支付|账户支付|
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
                     * 居民个人自付二次补偿金额|体检金额|生育基金支付|本次民政补助支付|医保范围内费用|
                     * 医保类型|医保补差|
                     */

                    #region 医保类型
                    string yblx = string.Empty;
                    strSql = string.Format(@"select NAME from YBXMLBZD where LBMC='医疗类别' and CODE='{0}'",yllb_r);
                    ds.Tables.Clear();
                    ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                    if (ds.Tables[0].Rows.Count != 0)
                    {
                        yblx = ds.Tables[0].Rows[0]["NAME"].ToString();
                    }

                    strSql = string.Format(@"select LEFT(ylrylb,1) as ylrylb from ybickxx where grbh='{0}'", grbh);
                    ds.Tables.Clear();
                    ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                    if (ds.Tables[0].Rows.Count != 0)
                    {
                        string[] sValue = { "1", "2", "3" };
                        if (sValue.Contains(ds.Tables[0].Rows[0]["ylrylb"].ToString()))
                            yblx += "(职工)";
                        else
                            yblx += "(居民)";
                    }
                    #endregion

                    //本次医疗费总额=本次统筹支付金额+本次大病救助支付+本次大病保险支付+本次民政补助支付+本次帐户支付总额+本次现金支付总额+本次补保支付总额
                    string strValue = ylzfy1 + "|" + zbxje + "|" + tcjjzf + "|" + dejjzf + "|" + zhzf + "|" +
                      xjzf1 + "|" + gwybzjjzf + "|" + qybcylbxjjzf + "|" + "0.00" + "|" + dwfdfy + "|" +
                      yyfdfy + "|" + mzfdfy + "|" + cxjfy + "|" + ylzlfy + "|" + blzlfy + "|" +
                      fhjbylfy + "|" + qfbzfy + "|" + zzzyzffy + "|" + jrtcfy + "|" + tcfdzffy + "|" +
                      ctcfdxfy + "|" + "0.00" + "|" + defdzffy + "|" + cdefdxfy + "|" + rgqgzffy + "|" +
                      bcjsqzhye + "|" + bntczflj + "|" + bndezflj + "|" + bnczjmmztczflj + "|" + bngwybzzflj + "|" +
                      bnzhzflj + "|" + bnzycslj + "|" + zycs + "|" + xm + "|" + jsrq + "|" +
                      yllb_r + "||" + YLGHBH + "|" + YWZQH + "|" + djh + "|" +
                      "|" + djh + "||" + JYLSH + "|1|" +
                      "|" + YLGHBH + "|0.00|||" +
                      grbh + "|0.00|0.00|0.00|0.00|" +
                      "0.00|0.00|0.00|" + dbbyzf + "|" + ybfwnfy + "|" +
                      yblx + "|" + ybbcfy;

                    WriteLog(sysdate + "  门诊费用预结算返回参数|" + strValue);
                    //return new object[] { 0, 1, strValue };
                    strSql = string.Format(@"insert into ybfyjsdr (jzlsh,jylsh,djhin,cyrq,cyyy,bzbm,bzmc,yllb,xm,kh,
                                        grbh,ylfze,zbxje,tcjjzf,dejjzf,dbbyzf,mzjzfy,zhzf,xjzf,zhzfzf,
                                        zhzfzl,xjzhzf,xjzhzl,ybfwnfy,bcjsqzhye,dbzbc,smxx,yfhj,zlxmfhj,bbzf,
                                        bz6,jbr,zcfbz,djh,cfmxjylsh,zffy,ybjzlsh,cxbz,sysdate,jsrq,
                                        yblx) values(
                                        '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}',
                                        '{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}',
                                        '{20}','{21}','{22}','{23}','{24}','{25}','{26}','{27}','{28}','{29}',
                                        '{30}','{31}','{32}','{33}','{34}','{35}','{36}','{37}','{38}','{39}',
                                        '{40}')",
                                        jzlsh, JYLSH, djh, cyrq, cyyy, bzbm, bzmc, yllb_r, xm, kh,
                                        grbh, ylzfy1, zbxje, tcjjzf, dejjzf, dbbyzf, mzfdfy, zhzf, xjzf1, zhzfzf,
                                        zhzfzl, xjzfzf, xjzfzl, ybfwnfy, bcjsqzhye, dbzbm, smxx, yfhj, zlxmfhj,
                                        bbzf, by6, jbrbh, zcfbz, djh, cfjylsh, xjzf_gh, ybjzlsh, 1, sysdate, jsrq,
                                        yblx);
                    object[] obj = { strSql };
                    obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);
                    if (obj[1].ToString().Equals("1"))
                    {
                        WriteLog(sysdate + " 门诊费用结算成功|" + strValue);
                        return new object[] { 0, 1, strValue };
                    }
                    else
                    {
                        WriteLog(sysdate + " 门诊费用结算失败|数据操作失败|" + obj[2].ToString());
                        //撤销结算信息
                        object[] objFYDJCX = { jzlsh, djh, "0", ybjzlsh };
                        objFYDJCX = N_YBMZZYFYJSCX(objFYDJCX);
                        return new object[] { 0, 0, "门诊费用结算失败|数据操作失败|" + obj[2].ToString() };
                    }
                }
                else
                {
                    WriteLog(sysdate + "  门诊费用结算失败" + outputData.ToString());
                    return new object[] { 0, 0, outputData.ToString() };
                }
            }
            catch (Exception ex)
            {
                WriteLog(sysdate + "  门诊费用结算异常" + ex.Message);
                return new object[] { 0, 0, "门诊费用结算异常" + ex.Message };
            }

        }
        #endregion

        #region 门诊费用结算撤销
        public static object[] YBMZSFJSCX(object[] objParam)
        {
            string sysdate = GetServerDateTime();//当前时间
            string jzlsh = objParam[0].ToString();   // 就诊流水号
            string djh = objParam[1].ToString();     // 单据号(发票号)

            YWBH = "2430";
            JYLSH = DateTime.Now.ToString("yyyyMMddHHmmss") + "-" + YLGHBH + "-" + new Random().Next(100).ToString().PadLeft(4, '0');
            CZYBH = CliUtils.fLoginUser;
            YWZQH = CliUtils.fLoginYbNo.Split('|')[0];


            if (string.IsNullOrEmpty(jzlsh))
                return new object[] { 0, 0, "就诊流水号不能为空" };
            if (string.IsNullOrEmpty(djh))
                return new object[] { 0, 0, "单据号不能为空" };
            try
            {
                //判断是否已结算
                string strSql = string.Format(@"select * from ybfyjsdr where jzlsh='{0}' and djhin='{1}' and cxbz=1", jzlsh, djh);
                DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                if (ds.Tables[0].Rows.Count == 0)
                    return new object[] { 0, 0, "该患者无医保结算信息" };
                string cfjylsh = ds.Tables[0].Rows[0]["cfmxjylsh"].ToString();
                string ybjzlsh = ds.Tables[0].Rows[0]["ybjzlsh"].ToString();

                string jscxrq = Convert.ToDateTime(sysdate).ToString("yyyyMMddHHmmss");//结算撤销日期
                string jbr = CliUtils.fUserName;//经办人
                string sfblcfbz = "1"; //是否保留处方标志
                string by1 = "";    //备用1
                string by2 = "";    //备用2
                string by3 = "";    //备用3
                string by4 = "";    //备用4
                string by5 = "";    //备用5
                string by6 = "";    //备用6

                //入参数据
                StringBuilder inputParam = new StringBuilder();
                inputParam.Append(ybjzlsh + "|");
                inputParam.Append(djh + "|");
                inputParam.Append(jscxrq + "|");
                inputParam.Append(jbr + "|");
                inputParam.Append(sfblcfbz + "|");
                inputParam.Append(by1 + "|");
                inputParam.Append(YLJGGJM + "|"); //20200501修改
                inputParam.Append(by3 + "|");
                inputParam.Append(by4 + "|");
                inputParam.Append(by5 + "|");
                inputParam.Append(by6 + "|");

                //入参
                StringBuilder inputData = new StringBuilder();
                inputData.Append(YWBH + "^");
                inputData.Append(YLGHBH + "^");
                inputData.Append(CZYBH + "^");
                inputData.Append(YWZQH + "^");
                inputData.Append(JYLSH + "^");
                inputData.Append(JRFS + "^");
                inputData.Append(DKLX + "^");
                inputData.Append(inputParam + "^");

                WriteLog(sysdate + "  进入门诊费用结算撤销...");
                WriteLog(sysdate + "  入参|" + inputData.ToString());
                StringBuilder outputData = new StringBuilder(102400);
                List<string> liSQL = new List<string>();

                int i = BUSINESS_HANDLE(inputData, outputData);
                if (i == 0)
                {
                    WriteLog(sysdate + "  门诊费用结算撤销成功|出参|" + outputData.ToString());
                    strSql = string.Format(@"insert into ybfyjsdr (jzlsh,jylsh,djhin,cyrq,cyyy,bzbm,bzmc,yllb,xm,kh,
                                        grbh,ylfze,zbxje,tcjjzf,dejjzf,dbbyzf,mzjzfy,zhzf,xjzf,zhzfzf,
                                        zhzfzl,xjzhzf,xjzhzl,ybfwnfy,bcjsqzhye,dbzbc,smxx,yfhj,zlxmfhj,bbzf,
                                        bz6,cfmxjylsh,zcfbz,jbr,djh,ybjzlsh,jsrq,yblx,cxbz,sysdate) select jzlsh,jylsh,djhin,cyrq,cyyy,bzbm,bzmc,yllb,xm,kh,
                                        grbh,ylfze,zbxje,tcjjzf,dejjzf,dbbyzf,mzjzfy,zhzf,xjzf,zhzfzf,
                                        zhzfzl,xjzhzf,xjzhzl,ybfwnfy,bcjsqzhye,dbzbc,smxx,yfhj,zlxmfhj,bbzf,
                                        bz6,cfmxjylsh,zcfbz,'{3}',djh,ybjzlsh,jsrq,yblx,0,'{2}' from ybfyjsdr where jzlsh='{0}' and djhin='{1}' and cxbz=1", jzlsh, djh, sysdate, jbr);
                    liSQL.Add(strSql);
                    strSql = string.Format(@"update ybfyjsdr set cxbz=2 where jzlsh='{0}' and djhin='{1}' and cxbz=1", jzlsh, djh);
                    liSQL.Add(strSql);
                    object[] obj = liSQL.ToArray();
                    obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);
                    if (obj[1].ToString().Equals("1"))
                    {
                        WriteLog(sysdate + "  门诊费用结算撤销成功|数据操作成功|");
                        //门诊费用登记撤销
                        object[] objFYDJCX = { jzlsh };
                        objFYDJCX = YBMZFYDJCX(objFYDJCX);
                        if (objFYDJCX[1].ToString().Equals("1"))
                            return new object[] { 0, 1, cfjylsh };
                        else
                            return new object[] { 0, 0, "门诊费用结算撤销成功|费用登记撤销失败" };
                    }
                    else
                    {
                        WriteLog(sysdate + "  门诊费用结算撤销失败|数据操作失败|" + obj[2].ToString());
                        return new object[] { 0, 0, "门诊费用结算撤销失败|数据操作失败|" + obj[2].ToString() };
                    }
                }
                else
                {
                    WriteLog(sysdate + "  门诊费用结算撤销失败|" + outputData.ToString());
                    return new object[] { 0, 0, "门诊费用结算撤销失败|" + outputData.ToString() };
                }
            }
            catch (Exception ex)
            {
                WriteLog(sysdate + "  门诊费用结算撤销" + ex.Message);
                return new object[] { 0, 0, "门诊费用结算撤销" + ex.Message };
            }
        }
        #endregion

        #region 住院登记
        public static object[] YBZYDJ(object[] objParam)
        {
            string sysdate = GetServerDateTime();//当前时间
            string jzlsh = objParam[0].ToString(); //就诊流水号
            string yllb = objParam[1].ToString(); //医疗类别代码
            string bzbm = objParam[2].ToString(); //病种编码
            string bzmc = objParam[3].ToString(); //病种名称
            string lyjgdm = objParam[5].ToString();//来源机构代码
            string lyjgmc = objParam[6].ToString();//来源机构名称
            string yllbmc = objParam[7].ToString();//医疗类别名称
            try
            {

                #region 读卡信息
                string[] kxx = objParam[4].ToString().Split('|');//读卡返回信息
                if (kxx.Length < 10)
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

                YWBH = "2210";
                JYLSH = DateTime.Now.ToString("yyyyMMddHHmmss") + "-" + YLGHBH + "-" + new Random().Next(100).ToString().PadLeft(4, '0');
                CZYBH = CliUtils.fLoginUser;
                YWZQH = CliUtils.fLoginYbNo.Split('|')[0];
                string ybjzlsh = jzlsh + DateTime.Now.ToString("HHmmss");

                string ksmc = string.Empty; //科室名称
                string ksbm = string.Empty; //科室编码
                string ybbh = grbh; //医保卡号
                string lxdh = string.Empty; //联系电话
                string rysj = string.Empty;    //门诊/住院入院时间
                string cwh = "";    //床位号
                string ysbm = string.Empty;   //医生编码
                string ysxm = string.Empty;     //医生姓名
                string jbr = CliUtils.fUserName; //经办人
                string hzxm = string.Empty; //患者姓名

                if (string.IsNullOrEmpty(jzlsh))
                    return new object[] { 0, 0, "就诊流水号不能为空" };
                if (string.IsNullOrEmpty(yllb))
                    return new object[] { 0, 0, "医疗类别代码不能为空|" };

                string strSql = string.Format(@"select a.z1zyno,a.z1hznm,a.z1date, a.z1ksno, a.z1ksnm,a.z1empn as z1ysno,a.z1mzys as z1ysnm,'' as z1bedn,a.z1mobi,a.z1yzbm from zy01h a
                                            where a.z1zyno='{0}'", jzlsh);
                DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                if (ds == null || ds.Tables[0].Rows.Count == 0)
                    return new object[] { 0, 0, "无患者信息" };
                rysj = Convert.ToDateTime(ds.Tables[0].Rows[0]["z1date"].ToString()).ToString("yyyyMMddHHmmss");
                ksmc = ds.Tables[0].Rows[0]["z1ksnm"].ToString();
                ksbm = ds.Tables[0].Rows[0]["z1ksno"].ToString();
                cwh = ds.Tables[0].Rows[0]["z1bedn"].ToString();
                ysbm = ds.Tables[0].Rows[0]["z1ysno"].ToString();
                ysxm = ds.Tables[0].Rows[0]["z1ysnm"].ToString();
                lxdh = ds.Tables[0].Rows[0]["z1mobi"].ToString();
                hzxm = ds.Tables[0].Rows[0]["z1hznm"].ToString();
                if (!string.Equals(hzxm, xm))
                    return new object[] { 0, 0, "入院登记姓名和医保卡姓名不相符" };

                strSql = string.Format(@"select * from ybmzzydjdr where jzlsh='{0}' and cxbz=1", jzlsh);
                ds.Tables.Clear();
                ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                if (ds.Tables[0].Rows.Count > 0)
                    return new object[] { 0, 0, "患者已进行医保住院登记，清匆再进行重复操作" };
                #region 获取定岗医生信息
                string ysgjm = string.Empty;
                strSql = string.Format(@"select * from ybdgyszd where ysbm='{0}'", ysbm);
                ds.Tables.Clear();
                ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    ysgjm = ds.Tables[0].Rows[0]["dgysbm"].ToString();
                }
                #endregion
                #region 入参
                //上传参数
                StringBuilder inputParam = new StringBuilder();
                inputParam.Append(ybjzlsh + "|");
                inputParam.Append(yllb + "|");
                inputParam.Append(rysj + "|");
                inputParam.Append(bzbm + "|");
                inputParam.Append(ksmc + "|");
                inputParam.Append(ksbm + "|");
                inputParam.Append(cwh + "|");
                inputParam.Append(ysbm + "|");
                inputParam.Append(jbr + "|");
                inputParam.Append(lxdh + "|");
                inputParam.Append(ybbh + "|");
                inputParam.Append(jzlsh + "|");
                //inputParam.Append("|||||");
                inputParam.Append("|"); //准生儿社会保障卡号
                inputParam.Append(YLJGGJM + "|"); //医疗机构国家码
                inputParam.Append("|"); //护士国家码
                //inputParam.Append(ysgjm + "|"); //药师国家码
                inputParam.Append("" + "|"); //药师国家码
                inputParam.Append("|"); //备用6

                //入参
                StringBuilder inputData = new StringBuilder();
                inputData.Append(YWBH + "^");
                inputData.Append(YLGHBH + "^");
                inputData.Append(CZYBH + "^");
                inputData.Append(YWZQH + "^");
                inputData.Append(JYLSH + "^");
                inputData.Append(JRFS + "^");
                inputData.Append(DKLX + "^");
                inputData.Append(inputParam + "^");
                #endregion

                StringBuilder outputData = new StringBuilder(1024);
                List<string> liSQL = new List<string>();

                WriteLog(sysdate + "  进入住院登记...");
                WriteLog(sysdate + "  入参|" + inputData.ToString());
                int i = BUSINESS_HANDLE(inputData, outputData);
                if (i == 0)
                {
                    strSql = string.Format(@"insert into ybmzzydjdr(
                                        jzlsh,jylsh,yldylb,ghdjsj,bzbm,bzmc,bq,ksbh,ksmc,cwh,
                                        ysdm,jbr,dwmc,grbh,xm,xb,ybjzlsh,kh,yllb,cxbz,
                                        sysdate,tcqh,ydrybz,nl,csrq,zhye,jzbz)
                                        values(
                                        '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}',
                                        '{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}',
                                        '{20}','{21}','{22}','{23}','{24}','{25}','{26}')",
                                            jzlsh, JYLSH, yldylb, rysj, bzbm, bzmc, "", ksbm, ksmc, cwh,
                                            ysbm, jbr, dwmc, ybbh, xm, xb, ybjzlsh, kh, yllb, 1,
                                            sysdate, tcqh, ydrybz, nl, csrq, zhye, "z");
                    liSQL.Add(strSql);


                    strSql = string.Format(@"update zy01h set z1rylb = '{0}', z1tcdq = '{1}', z1lyjg = '{2}', z1lynm = '{3}', z1ylno = '{4}'
                                            , z1ylnm = '{5}', z1bzno = '{6}', z1bznm = '{7}', z1ybno = '{8}' where z1comp = '{9}' and z1zyno = '{10}'"
                                            , yldylb, tcqh, lyjgdm, lyjgmc, yllb, yllbmc, bzbm, bzmc, grbh, CliUtils.fSiteCode, jzlsh);
                    liSQL.Add(strSql);

                    object[] obj_dj = liSQL.ToArray();
                    obj_dj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj_dj);
                    if (obj_dj[1].ToString().Equals("1"))
                    {
                        WriteLog(sysdate + "  住院登记成功|" + outputData.ToString());
                        return new object[] { 0, 1, "住院登记成功" };
                    }
                    else
                    {
                        WriteLog(sysdate + "  住院登记失败|数据操作失败|" + obj_dj[2].ToString());
                        //门诊登记撤销
                        object[] objParam1 = { ybjzlsh };
                        objParam1 = N_YBMZZYDJCX(objParam1);
                        return new object[] { 0, 0, "住院登记失败|数据操作失败|" + obj_dj[2].ToString() };
                    }
                }
                else
                {
                    WriteLog(sysdate + "  住院登记失败|" + outputData.ToString());
                    return new object[] { 0, 0, outputData.ToString().Split('^')[2].ToString() };
                }
            }
            catch (Exception ex)
            {
                WriteLog(sysdate + " 住院登记异常|" + ex.Message);
                return new object[] { 0, 0, "住院登记异常|" + ex.Message };
            }
        }
        #endregion

        #region 住院登记撤销
        public static object[] YBZYDJCX(object[] objParam)
        {
            string sysdate = GetServerDateTime();   //系统时间
            string jzlsh = objParam[0].ToString();  // 就诊流水号

            YWBH = "2240";
            JYLSH = DateTime.Now.ToString("yyyyMMddHHmmss") + "-" + YLGHBH + "-" + new Random().Next(100).ToString().PadLeft(4, '0');
            CZYBH = CliUtils.fLoginUser;
            YWZQH = CliUtils.fLoginYbNo.Split('|')[0];
            string jbr = CliUtils.fUserName;
            if (string.IsNullOrEmpty(jzlsh))
                return new object[] { 0, 0, "就诊流水号不能为空" };

            StringBuilder inputData = new StringBuilder();
            StringBuilder inputParam = new StringBuilder();
            StringBuilder outputData = new StringBuilder(1024);

            #region 判断是否进行住院登记
            string strSql = string.Format(@"select jzlsh,ybjzlsh,cxbz from ybmzzydjdr where cxbz=1 and jzlsh='{0}'", jzlsh);
            DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            if (ds.Tables[0].Rows.Count == 0)
                return new object[] { 0, 0, "该患者未医保住院登记" };
            string ybjzlsh = ds.Tables[0].Rows[0]["ybjzlsh"].ToString();
            #endregion

            #region 判断是否有费用上传
            strSql = string.Format(@"select jzlsh,cxbz from ybcfmxscindr where cxbz=1 and jzlsh='{0}' and ybjzlsh='{1}'", jzlsh, ybjzlsh);
            ds.Tables.Clear();
            ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            if (ds.Tables[0].Rows.Count > 0)
                return new object[] { 0, 0, "该患者已上传费用明细，请先撤销上传费用" };
            #endregion

            //入参数据
            inputParam.Append(ybjzlsh + "|");
            inputParam.Append(jbr + "|");
            inputParam.Append(YLJGGJM + "|");   //20210增加

            //入参
            inputData.Append(YWBH + "^");
            inputData.Append(YLGHBH + "^");
            inputData.Append(CZYBH + "^");
            inputData.Append(YWZQH + "^");
            inputData.Append(JYLSH + "^");
            inputData.Append(JRFS + "^");
            inputData.Append(DKLX + "^");
            inputData.Append(inputParam + "^");

            WriteLog(sysdate + "  进入医保住院登记撤销...");
            WriteLog("入参|" + inputData.ToString());
            int i = BUSINESS_HANDLE(inputData, outputData);
            if (i == 0)
            {
                List<string> liSQL = new List<string>();
                strSql = string.Format(@"insert into ybmzzydjdr(
                                        jzlsh,jylsh,yldylb,ghdjsj,bzbm,bzmc,bq,ksbh,ksmc,cwh,
                                        ysdm,jbr,dwmc,grbh,xm,xb,ybjzlsh,kh,cxbz,sysdate,
                                        tcqh,ydrybz,nl,csrq,zhye,jzbz)
                                        select jzlsh,jylsh,yldylb,ghdjsj,bzbm,bzmc,bq,ksbh,ksmc,cwh,
                                        ysdm,'{1}',dwmc,grbh,xm,xb,ybjzlsh,kh,0,'{2}', 
                                        tcqh,ydrybz,nl,csrq,zhye,jzbz 
                                        from ybmzzydjdr where jzlsh='{0}' and cxbz=1", jzlsh, jbr, sysdate);
                liSQL.Add(strSql);
                strSql = string.Format(@"update ybmzzydjdr set cxbz=2 where jzlsh='{0}' and cxbz=1", jzlsh);
                liSQL.Add(strSql);

                strSql = string.Format(@"update zy01h set z1lyjg='0000',z1lynm='全费自理' where z1zyno='{0}'", jzlsh);
                liSQL.Add(strSql);
                object[] obj = liSQL.ToArray();
                obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);
                if (obj[1].ToString().Equals("1"))
                {
                    WriteLog(sysdate + "  医保住院登记撤销成功|");
                    return new object[] { 0, 1, "医保住院登记撤销成功" };
                }
                else
                {
                    WriteLog(sysdate + "  医保住院登记撤销失败|" + obj[2].ToString());
                    return new object[] { 0, 0, obj[2].ToString() };
                }
            }
            else
            {
                WriteLog(sysdate + "  医保门诊登记撤销失败|" + outputData.ToString());
                return new object[] { 0, 0, outputData.ToString() };
            }
        }
        #endregion

        #region 住院费用登记
        public static object[] YBZYSFDJ(object[] objParam)
        {
            string sysdate = GetServerDateTime();   //系统时间
            string jzlsh = objParam[0].ToString();  //就诊流水号
            string jzsj = objParam[1].ToString();   //截止时间 中途结算时，截止时间不能为空
            string sWhere = "";

            if (string.IsNullOrEmpty(jzlsh))
                return new object[] { 0, 0, "就诊流水号不能为空" };

            if (!string.IsNullOrEmpty(jzsj))
            {
                jzsj = Convert.ToDateTime(jzsj).ToString("yyyy-MM-dd") + " 23:59:59";
                sWhere = string.Format(@"a.z3date <=CONVERT(datetime,'{0}')", jzsj);
            }

            try
            {
                YWBH = "2310";
                JYLSH = DateTime.Now.ToString("yyyyMMddHHmmss") + "-" + YLGHBH + "-" + new Random().Next(100).ToString().PadLeft(4, '0');
                CZYBH = CliUtils.fLoginUser;
                YWZQH = CliUtils.fLoginYbNo.Split('|')[0];
                string jbr = CliUtils.fUserName;

                string strSql = string.Format("select * from ybmzzydjdr a where a.jzlsh = '{0}' and a.cxbz = 1", jzlsh);
                DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                if (ds == null || ds.Tables[0].Rows.Count == 0)
                    return new object[] { 0, 0, "该患者未办理医保登记" };
                string grbh = ds.Tables[0].Rows[0]["grbh"].ToString(); //个人编码
                string xm = ds.Tables[0].Rows[0]["xm"].ToString();  //姓名
                string kh = ds.Tables[0].Rows[0]["kh"].ToString();  //卡号
                string bzbm = ds.Tables[0].Rows[0]["bzbm"].ToString(); //病种编码
                string ybjzlsh = ds.Tables[0].Rows[0]["ybjzlsh"].ToString();

                #region 获取医嘱信息
                strSql = string.Format(@"select 
                                        case when a.z3sfno IN(select b5sfno from bz05h where b5zyno in('01','02','03')) then 1 
                                        when a.z3sfno IN(select b5sfno from bz05h where b5zyno in('19')) then 3
                                        else 2 end as sfxmzl,MAX(z3date) as zxsj, a.z3item as yyxmbh, a.z3name as yyxmmc,
                                        a.z3djxx as dj,sum(case LEFT(a.z3endv,1) when '4' then -a.z3jzcs else 1*a.z3jzcs end) as sl,
                                        sum(case LEFT(a.z3endv,1) when '4' then -1*convert(decimal(8,4),z3djxx*z3jzcs) else 1*convert(decimal(8,4),z3djxx*z3jzcs) end) as je, 1 as fs,
                                        --sum(case LEFT(a.z3endv,1) when '4' then -1*convert(decimal(8,2),z3djxx*z3jzcs) else 1*convert(decimal(8,2),z3djxx*z3jzcs) end) as je, 1 as fs,
                                        a.z3empn as ysdm, a.z3kdys as ysxm,a.z3ksno as ksno, a.z3zxks as zxks, '0' as sfbz,sum(case LEFT(a.z3endv,1) when '4' then -z3jzje else z3jzje end) jzje,ybxmbh
                                        from zy03d a left join ybhisdzdr b on a.z3item=b.hisxmbh
                                        where a.z3ybup is null and LEFT(a.z3kind,1) in(2,4)  and a.z3zyno='{0}' {1}
                                        group by a.z3djxx,a.z3item,a.z3name, a.z3empn,a.z3kdys,a.z3ksno,a.z3zxks,a.z3sfno,ybxmbh
                                        having sum(case LEFT(a.z3endv,1) when '4' then -1*a.z3jzcs else 1*a.z3jzcs end)!=0", jzlsh, sWhere);
                ds.Tables.Clear();
                ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                if (ds.Tables[0].Rows.Count == 0)
                    return new object[] { 0, 0, "无医嘱信息" };

                List<string> liYZXX = new List<string>(); //医嘱信息
                string sMsg = string.Empty;
                int index_cfh = 1;
                int m = 0;
                List<string> liSQL = new List<string>();
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    #region 赋值
                    string cfh = (index_cfh++).ToString();      //处方号
                    string cflsh = "010" + (index_cfh++).ToString();  //处方流水号
                    string zxsj = Convert.ToDateTime(dr["zxsj"].ToString()).ToString("yyyyMMddHHmmss");    //执行时间
                    string sfxmzl = dr["sfxmzl"].ToString(); //收费项目种类
                    string yyxmbm = dr["yyxmbh"].ToString();    //医院收费项目自编码
                    string yyxmmc = dr["yyxmmc"].ToString();    //医院收费项目名称
                    string dj = dr["dj"].ToString();    //单价
                    string sl = dr["sl"].ToString();    //数量
                    string je = dr["je"].ToString();    //金额
                    string fs = dr["fs"].ToString();    //中药饮片副数
                    string ysbm = dr["ysdm"].ToString();    //医生编码
                    string ysxm = dr["ysxm"].ToString();    //医生姓名
                    string ksbm = dr["ksno"].ToString();    //科室编码
                    string ksmc = dr["zxks"].ToString();    //科室名称
                    string sfbz = dr["sfbz"].ToString();    //按最小计价单位收费标志
                    string by3 = "";    //备用3
                    string by4 = "";    //备用4
                    string by5 = "";    //备用5
                    string by6 = "";    //备用6
                    string gjbm = dr["ybxmbh"].ToString();    //国家编码  20210506暂时不上传
                    //string gjbm = "";    //国家编码
                    #endregion
                    #region 获取定岗医生信息
                    string ysgjm = string.Empty;
                    strSql = string.Format(@"select * from ybdgyszd where ysbm='{0}'", ysbm);
                    DataSet dsYS  = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                    if (dsYS.Tables[0].Rows.Count > 0)
                    {
                        ysgjm = dsYS.Tables[0].Rows[0]["dgysbm"].ToString();
                    }
                    #endregion
                    if (string.IsNullOrEmpty(sfxmzl) || string.IsNullOrEmpty(yyxmbm))
                        sMsg = "存在空数据或未配对数据";

                    #region 参数
                    StringBuilder inputParam = new StringBuilder();
                    #region 补差操作
                    string byzje = ds.Tables[0].Compute("sum(jzje)", "true").ToString();
                    string ybzje = ds.Tables[0].Compute("sum(je)", "true").ToString();
                    decimal sfje = Math.Round(decimal.Parse(byzje),4);
                    decimal scfy = Math.Round(decimal.Parse(ybzje), 4);
                    decimal xcje = Math.Round(sfje - scfy, 4);
                    if ((Math.Abs(Math.Round((sfje - scfy), 2)) > 0) && (m != 9))
                    {
                        m = 9;
                        WriteLog(sysdate + "医保补差  |" + sfje + "|" + scfy + "|" + Math.Round((scfy - sfje)) + "|" + Math.Abs(Math.Round((scfy - sfje), 4)) + "m: "+m.ToString());
                        inputParam.Append(ybjzlsh + "|");
                        inputParam.Append("2" + "|");
                        inputParam.Append(cfh + "|");
                        inputParam.Append(cflsh + "|");
                        inputParam.Append(zxsj + "|");
                        inputParam.Append("800000000197" + "|");
                        inputParam.Append(xcje + "|");
                        inputParam.Append(1 + "|");
                        inputParam.Append(xcje + "|");
                        inputParam.Append("|");
                        inputParam.Append(ysbm + "|");
                        inputParam.Append(ksbm + "|");
                        inputParam.Append(jbr + "|");
                        inputParam.Append("|");
                        inputParam.Append(bzbm + "|");
                        inputParam.Append(by3 + "|");
                        inputParam.Append(by4 + "|");
                        inputParam.Append(by5 + "|");
                        inputParam.Append(by6 + "|");
                        inputParam.Append(YLJGGJM + "|"); //20200501增加
                        inputParam.Append("001309000020000" + "|");
                        inputParam.Append(by5 + "|");
                        //inputParam.Append(ysgjm + "|$");
                        inputParam.Append("" + "|$");
                        #region 记录上传费用信息
                        strSql = string.Format(@"insert into ybcfmxscindr(jzlsh,jylsh,sfxmzl,ybcfh,djlsh,cfrq,yysfxmbm,yysfxmmc,dj,sl,
						                   je,ysbm,ksbh,jbr,sflb,grbh,xm,kh,cxbz,sysdate,ybjzlsh) values(
                                           '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}',
                                           '{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}','{20}')",
                                        jzlsh, JYLSH, "2", cflsh, cflsh, zxsj, "800000000197", "疾病健康教育", Math.Round((sfje - scfy), 4), 1,
                                        Math.Round((sfje - scfy), 4), ysbm, ksbm, jbr, "", grbh, xm, kh, 1, sysdate, ybjzlsh);
                        liSQL.Add(strSql);
                        #endregion
                    }
                    #endregion

                    inputParam.Append(ybjzlsh + "|");
                    inputParam.Append(sfxmzl + "|");
                    inputParam.Append(cfh + "|");
                    inputParam.Append(cflsh + "|");
                    inputParam.Append(zxsj + "|");
                    inputParam.Append(yyxmbm + "|");
                    inputParam.Append(dj + "|");
                    inputParam.Append(sl + "|");
                    inputParam.Append(je + "|");
                    inputParam.Append(fs + "|");
                    inputParam.Append(ysbm + "|");
                    inputParam.Append(ksbm + "|");
                    inputParam.Append(jbr + "|");
                    inputParam.Append(sfbz + "|");
                    inputParam.Append(bzbm + "|");
                    inputParam.Append(by3 + "|");
                    inputParam.Append(by4 + "|");
                    inputParam.Append(by5 + "|");
                    inputParam.Append(by6 + "|");
                    inputParam.Append(YLJGGJM + "|"); //20200501增加
                    inputParam.Append(gjbm + "|");
                    inputParam.Append(by5 + "|");
                    //inputParam.Append(ysgjm + "|$");
                    inputParam.Append("" + "|$");

                    #endregion

                    #region 记录上传费用信息
                    strSql = string.Format(@"insert into ybcfmxscindr(jzlsh,jylsh,sfxmzl,ybcfh,djlsh,cfrq,yysfxmbm,yysfxmmc,dj,sl,
						                   je,ysbm,ksbh,jbr,sflb,grbh,xm,kh,ybjzlsh,cxbz,sysdate) values(
                                           '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}',
                                           '{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}','{20}')",
                                             jzlsh, JYLSH, sfxmzl, cfh, cflsh, zxsj, yyxmbm, yyxmmc, dj, sl,
                                             je, ysbm, ksbm, jbr, sfbz, grbh, xm, kh, ybjzlsh, 1, sysdate);
                    liSQL.Add(strSql);
                    #endregion

                    liYZXX.Add(inputParam.ToString());
                }

                if (!string.IsNullOrEmpty(sMsg))
                    return new object[] { 0, 0, sMsg };
                #endregion

                int iTemp = 1;
                StringBuilder inputParam1 = new StringBuilder();
                #region 分段上传 每次50条
                WriteLog(liYZXX.Count.ToString());
                foreach (string inputData3 in liYZXX)
                {
                    if (iTemp <=30)
                    {
                        inputParam1.Append(inputData3);
                        //WriteLog(inputParam1.ToString());
                        iTemp++;
                    }
                    else
                    {
                        StringBuilder outputData = new StringBuilder(102400);

                        WriteLog(sysdate + "  " + jzlsh + " 进入住院费用登记(分段)...");
                        WriteLog(sysdate + "  入参|" + inputParam1.ToString().TrimEnd('$'));

                        //入参
                        StringBuilder inputData = new StringBuilder();
                        inputData.Append(YWBH + "^");
                        inputData.Append(YLGHBH + "^");
                        inputData.Append(CZYBH + "^");
                        inputData.Append(YWZQH + "^");
                        inputData.Append(JYLSH + "^");
                        inputData.Append(JRFS + "^");
                        inputData.Append(DKLX + "^");
                        inputData.Append(inputParam1.ToString().TrimEnd('$') + "^");
                        int i = BUSINESS_HANDLE(inputData, outputData);
                        if (i != 0)
                        {
                            WriteLog(sysdate + "  " + jzlsh + " 住院收费登记(分段)失败|" + outputData.ToString());
                            return new object[] { 0, 0, "住院收费登记(分段)失败|" + outputData.ToString() };
                        }

                        strSql = string.Format(@"update zy03d set z3ybup='{1}' where z3zyno='{0}' {2} and isnull(z3ybup,'')='' ", jzlsh, JYLSH, sWhere);
                        liSQL.Add(strSql);
                        string[] str = outputData.ToString().Split('^')[1].TrimEnd('$').Split('$');
                        string[] array = str;
                        for (int k = 0; k < array.Length; k++)
                        {
                            string s = array[k];
                            string[] str2 = s.Split('|');
                            //出参:处方号|处方流水号|处方日期|医院收费项目自编码|金额|自付金额|自理金额|自付比例|支付上限|收费项目等级|说明信息|备用2|备用3|备用4|备用5|备用6
                            string cfh_r = str2[0];    //处方号
                            string cflsh_r = str2[1];  //处方流水号
                            string cfrq_r = str2[2];   //处方日期
                            string yyxmbm_r = str2[3]; //医院收费项目自编码
                            string je_r = str2[4];     //金额
                            string zfje_r = str2[5];   //自付金额
                            string zlje_r = str2[6];   //自理金额
                            string zfbl_r = str2[7];   //自付比例
                            string zfsx_r = str2[8];   //支付上限
                            string sfxmdj_r = str2[9]; //收费项目等级
                            string smxx_r = str2[10];  //说明信息
                            string by2_r = str2[11];   //备用2
                            string by3_r = str2[12];   //备用3
                            string by4_r = str2[13];   //备用4
                            string by5_r = str2[14];   //备用5
                            string by6_r = str2[15];   //备用6

                            strSql = string.Format(@"insert into ybcfmxscfhdr(jzlsh,jylsh,yyxmdm,ybcfh,cfh,je,zfje,zlje,zlbl,qezfbz,
                                                sfxmdj,yyxmmc,grbh,xm,kh,cxbz,sysdate ) values(
                                                '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}',
                                                '{10}','{11}','{12}','{13}','{14}','{15}','{16}')",
                                               jzlsh, JYLSH, yyxmbm_r, cfh_r, cflsh_r, je_r, zfje_r, zlje_r, zfbl_r, zfsx_r,
                                               sfxmdj_r, smxx_r, grbh, xm, kh, 1, sysdate);
                            liSQL.Add(strSql);
                        }
                        inputParam1.Remove(0, inputParam1.Length);
                        iTemp = 0;
                        inputParam1.Append(inputData3);

                    }
                }
                #endregion
                #region 明细不足50条时，一次性上传
                if (iTemp >= 0)
                {
                    StringBuilder inputData = new StringBuilder();
                    inputData.Append(YWBH + "^");
                    inputData.Append(YLGHBH + "^");
                    inputData.Append(CZYBH + "^");
                    inputData.Append(YWZQH + "^");
                    inputData.Append(JYLSH + "^");
                    inputData.Append(JRFS + "^");
                    inputData.Append(DKLX + "^");
                    inputData.Append(inputParam1.ToString().TrimEnd('$') + "^");

                    WriteLog(sysdate + "  " + jzlsh + " 进入住院费用登记(补传、一次性上传)...");
                    WriteLog(sysdate + "  入参|" + inputData.ToString());

                    StringBuilder outputData = new StringBuilder(102400);
                    int i = BUSINESS_HANDLE(inputData, outputData);
                    if (i != 0)
                    {
                        WriteLog(sysdate + "  " + jzlsh + " 住院费用登记(补传、一次性上传)失败|" + outputData.ToString());
                        return new object[] { 0, 0, "住院费用登记(补传、一次性上传)失败|" + outputData.ToString() };
                    }

                    strSql = string.Format(@"update zy03d set z3ybup='{1}' where z3zyno='{0}' {2} and isnull(z3ybup,'')='' ", jzlsh, JYLSH, sWhere);
                    liSQL.Add(strSql);

                    string[] str = outputData.ToString().Split('^')[1].TrimEnd('$').Split('$');
                    string[] array = str;

                    WriteLog("c4分断返回行数 " + array.Length.ToString());
                    for (int k = 0; k < array.Length; k++)
                    {
                        string s = array[k];
                        string[] str2 = s.Split('|');
                        //出参:处方号|处方流水号|处方日期|医院收费项目自编码|金额|自付金额|自理金额|自付比例|支付上限|收费项目等级|说明信息|备用2|备用3|备用4|备用5|备用6
                        string cfh_r = str2[0];    //处方号
                        string cflsh_r = str2[1];  //处方流水号
                        string cfrq_r = str2[2];   //处方日期
                        string yyxmbm_r = str2[3]; //医院收费项目自编码
                        string je_r = str2[4];     //金额
                        string zfje_r = str2[5];   //自付金额
                        string zlje_r = str2[6];   //自理金额
                        string zfbl_r = str2[7];   //自付比例
                        string zfsx_r = str2[8];   //支付上限
                        string sfxmdj_r = str2[9]; //收费项目等级
                        string smxx_r = str2[10];  //说明信息
                        string by2_r = str2[11];   //备用2
                        string by3_r = str2[12];   //备用3
                        string by4_r = str2[13];   //备用4
                        string by5_r = str2[14];   //备用5
                        string by6_r = str2[15];   //备用6

                        strSql = string.Format(@"insert into ybcfmxscfhdr(jzlsh,jylsh,yyxmdm,ybcfh,cfh,je,zfje,zlje,zlbl,qezfbz,
                                            sfxmdj,yyxmmc,grbh,xm,kh,cxbz,sysdate ) values(
                                            '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}',
                                            '{10}','{11}','{12}','{13}','{14}','{15}','{16}')",
                                                jzlsh, JYLSH, yyxmbm_r, cfh_r, cflsh_r, je_r, zfje_r, zlje_r, zfbl_r, zfsx_r,
                                                sfxmdj_r, smxx_r, grbh, xm, kh, 1, sysdate);
                        liSQL.Add(strSql);
                    }
                    inputParam1.Remove(0, inputParam1.Length);
                }
                #endregion

                object[] obj = liSQL.ToArray();
                obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);
                if (obj[1].ToString().Equals("1"))
                {
                    WriteLog(sysdate + "  住院费用登记成功|");
                    return new object[] { 0, 1, JYLSH };
                }
                else
                {
                    WriteLog(sysdate + "  住院费用登记失败|数据操作失败|" + obj[2].ToString());
                    //门诊费用上传撤销
                    object[] objParam1 = { ybjzlsh, "", "", jbr };
                    objParam1 = N_YBMZZYSFDJCX(objParam1);
                    return new object[] { 0, 0, "住院费用登记失败|数据操作失败|" + obj[2].ToString() };
                }
            }
            catch (Exception ex)
            {
                WriteLog(sysdate + "  住院费用登记异常|" + ex.Message);
                return new object[] { 0, 0, "住院费用登记异常|" + ex.Message };
            }
        }
        #endregion

        #region 住院费用登记撤销
        public static object[] YBZYSFDJCX(object[] objParam)
        {
            string sysdate = GetServerDateTime();//当前时间
            string jzlsh = objParam[0].ToString(); //就诊流水号
            string cfmxjylsh = ""; //处方交易流水号
            string sWhere = ""; //查询条件

            if (string.IsNullOrEmpty(jzlsh))
                return new object[] { 0, 0, "就诊流水号不能为空" };

            if (!string.IsNullOrEmpty(cfmxjylsh))
            {
                sWhere = string.Format(@"and jylsh='{0}'", cfmxjylsh);
            }

            YWBH = "2320";
            JYLSH = DateTime.Now.ToString("yyyyMMddHHmmss") + "-" + YLGHBH + "-" + new Random().Next(100).ToString().PadLeft(4, '0');
            CZYBH = CliUtils.fLoginUser;
            YWZQH = CliUtils.fLoginYbNo.Split('|')[0];
            string jbr = CliUtils.fUserName;

            //获取上传处方信息
            string strSql = string.Format(@"select distinct jylsh,ybjzlsh from ybcfmxscindr where jzlsh='{0}' and cxbz=1 
                                            and jylsh not in(select isnull(cfmxjylsh,'') as cfjylsh from ybfyjsdr where cxbz=1 and jzlsh='{0}')", jzlsh);
            DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            if (ds.Tables[0].Rows.Count == 0)
            {
                WriteLog(sysdate + "  该患者未上传费用信息或已完成结算");
                return new object[] { 0, 0, "该患者未上传费用信息或已完成结算" };
            }

            cfmxjylsh = ds.Tables[0].Rows[0]["jylsh"].ToString();
            string ybjzlsh = ds.Tables[0].Rows[0]["ybjzlsh"].ToString();

            string cfh = "";
            string cflsh = "";

            //入参数据
            StringBuilder inputParam = new StringBuilder();
            inputParam.Append(ybjzlsh + "|");
            inputParam.Append(cfh + "|");
            inputParam.Append(cflsh + "|");
            inputParam.Append(jbr + "|");

            //入参
            StringBuilder inputData = new StringBuilder();
            inputData.Append(YWBH + "^");
            inputData.Append(YLGHBH + "^");
            inputData.Append(CZYBH + "^");
            inputData.Append(YWZQH + "^");
            inputData.Append(JYLSH + "^");
            inputData.Append(JRFS + "^");
            inputData.Append(DKLX + "^");
            inputData.Append(inputParam + "^");

            WriteLog(sysdate + "  进入住院费用登记撤销...");

            StringBuilder outputData = new StringBuilder(1024);
            int i = BUSINESS_HANDLE(inputData, outputData);
            List<string> liSQL = new List<string>();
            if (i == 0)
            {
                strSql = string.Format(@"insert into ybcfmxscindr(jzlsh,jylsh,sfxmzl,ybcfh,djlsh,cfrq,yysfxmbm,yysfxmmc,dj,sl,
						                 je,ysbm,ksbh,jbr,sflb,grbh,xm,kh,cxbz,sysdate) select 
                                         jzlsh,jylsh,sfxmzl,ybcfh,djlsh,cfrq,yysfxmbm,yysfxmmc,dj,sl,
						                 je,ysbm,ksbh,'{2}',sflb,grbh,xm,kh,0,'{3}' from ybcfmxscindr where jzlsh='{0}' and jylsh='{1}' and cxbz=1",
                                         jzlsh, cfmxjylsh, jbr, sysdate);
                liSQL.Add(strSql);
                //strSql = string.Format(@"update ybcfmxscindr set cxbz=2 where jzlsh='{0}' and jylsh='{1}' and cxbz=1", jzlsh, cfmxjylsh);
                strSql = string.Format(@"update ybcfmxscindr set cxbz=2 where jzlsh='{0}' and cxbz=1", jzlsh);
                liSQL.Add(strSql);

                strSql = string.Format(@"insert into ybcfmxscfhdr(jzlsh,jylsh,yyxmdm,ybcfh,cfh,je,zfje,zlje,zlbl,qezfbz,
                                         sfxmdj,yyxmmc,grbh,xm,kh,cxbz,sysdate ) select
                                         jzlsh,jylsh,yyxmdm,ybcfh,cfh,je,zfje,zlje,zlbl,qezfbz,
                                         sfxmdj,yyxmmc,grbh,xm,kh,0,'{2}' from ybcfmxscfhdr where jzlsh='{0}' and jylsh='{1}' and cxbz=1",
                                         jzlsh, cfmxjylsh, sysdate);
                liSQL.Add(strSql);
                //strSql = string.Format(@"update ybcfmxscfhdr set cxbz=2 where jzlsh='{0}' and jylsh='{1}' and cxbz=1", jzlsh, cfmxjylsh);
                strSql = string.Format(@"update ybcfmxscfhdr set cxbz=2 where jzlsh='{0}' and cxbz=1", jzlsh);
                liSQL.Add(strSql);

                //strSql = string.Format("update zy03d set z3ybup = null where z3zyno = '{0}' and z3ybup='{1}'", jzlsh, cfmxjylsh);
                strSql = string.Format("update zy03d set z3ybup = null where z3zyno = '{0}' ", jzlsh);
                liSQL.Add(strSql);

                object[] obj = liSQL.ToArray();
                obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);
                if (obj[1].ToString().Equals("1"))
                {
                    WriteLog(sysdate + "  住院费用登记撤销成功|" + outputData.ToString());
                    return new object[] { 0, 1, "住院费用登记撤销成功" };
                }
                else
                {
                    WriteLog(sysdate + "  住院费用登记撤销失败|数据操作失败|" + obj[2].ToString());
                    return new object[] { 0, 0, "住院费用登记撤销失败|数据操作失败|" + obj[2].ToString() };
                }
            }
            else
            {
                WriteLog(sysdate + "  住院费用登记撤销失败|" + outputData.ToString());
                return new object[] { 0, 0, "住院费用登记撤销失败|数据操作失败|" + outputData.ToString() };
            }

        }
        #endregion

        #region 住院费用预结算
        public static object[] YBZYSFYJS(object[] objParam)
        {
            string sysdate = GetServerDateTime();//系统时间
            string jzlsh = objParam[0].ToString();   // 就诊流水号
            string cyyy = objParam[1].ToString();    // 出院原因代码
            string zhsybz = ""; //objParam[2].ToString();  // 账户使用标志（0或1）
            string ztjsbz = objParam[3].ToString(); //中途结算标志
            string cyrq = objParam[4].ToString();//出院日期
            string ylfhj1 = objParam[6].ToString(); //医疗费合计 

            string djh = "";    //单据号
            string cfmxjylsh = ""; //处方交易流水号


            string strSql = string.Format(@"select * from ybmzzydjdr where jzlsh='{0}' and cxbz=1", jzlsh);
            DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            if (ds.Tables[0].Rows.Count == 0)
                return new object[] { 0, 0, "该患者未医保门诊登记" };
            string yllb = ds.Tables[0].Rows[0]["yllb"].ToString(); //医疗类别
            string kh = ds.Tables[0].Rows[0]["kh"].ToString();  //社会保障卡号
            string ksbm = ds.Tables[0].Rows[0]["ksbh"].ToString();  //科室编码
            string ysbm = ds.Tables[0].Rows[0]["ysdm"].ToString();  //医生编码
            string ybjzlsh = ds.Tables[0].Rows[0]["ybjzlsh"].ToString();  //医保就诊流水号
            string cyzdbm = ds.Tables[0].Rows[0]["bzbm"].ToString();     //出院诊断疾病编码
            string cyzdmc = ds.Tables[0].Rows[0]["bzmc"].ToString();     //出院诊断疾病名称
            string xm = ds.Tables[0].Rows[0]["xm"].ToString();  //姓名
            string grbh = ds.Tables[0].Rows[0]["grbh"].ToString();  //医保编号

            strSql = string.Format("select a.z1outd as cyrq from zy01d a where left(a.z1endv, 1) = '8' and a.z1zyno = '{0}'", jzlsh);
            ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            if (ds == null || ds.Tables[0].Rows.Count == 0)
                return new object[] { 0, 0, "就诊流水号" + jzlsh + "未拖出床位" };
            //cyrq = Convert.ToDateTime(ds.Tables[0].Rows[0]["cyrq"]).ToString("yyyyMMddHHmmss"); //出院日期
            cyrq = Convert.ToDateTime(cyrq).ToString("yyyyMMddHHmmss");//出院日期
            #region 如果是中途结算，则需要对已上传费用进行撤销再上传操作
            if (ztjsbz.Equals("1"))
            {
                object[] objCFSCCX = { jzlsh, "" };
                objCFSCCX = YBZYSFDJCX(objCFSCCX);
                if (!objCFSCCX[1].ToString().Equals("1"))
                    return new object[] { 0, 0, "住院费用预结算失败|" + objCFSCCX[2].ToString() };
                object[] objCFSC = { jzlsh, cyrq };
                objCFSC = YBZYSFDJ(objCFSC);
                if (!objCFSC[1].ToString().Equals("1"))
                    return new object[] { 0, 0, "住院费用预结算失败|" + objCFSC[2].ToString() };
                cfmxjylsh = objCFSC[2].ToString();
            }

            #endregion

            YWBH = "2420";
            JYLSH = DateTime.Now.ToString("yyyyMMddHHmmss") + "-" + YLGHBH + "-" + new Random().Next(100).ToString().PadLeft(4, '0');
            CZYBH = CliUtils.fLoginUser;
            YWZQH = CliUtils.fLoginYbNo.Split('|')[0];
            string jbr = CliUtils.fUserName;    //经办人
            string jbrbh = CliUtils.fLoginUser;

            #region 入参赋值
            string jsrq = Convert.ToDateTime(sysdate).ToString("yyyyMMddHHmmss");   //结算日期
            string yjslb = "";      //月结算类别
            string fmrq = "";   //分娩日期
            string cs = "";     //产次
            string tes = "";    //胎儿数
            string zybh = "";   //转院医院编号
            string zsekh = "";  //准生儿社会保障卡号
            string ssbz = "";   //手术是否成功标志
            string sfghjs = ""; //是否为挂号费结算

            //入参数据
            StringBuilder inputParam = new StringBuilder();
            inputParam.Append(ybjzlsh + "|");
            inputParam.Append(djh + "|");
            inputParam.Append(yllb + "|");
            inputParam.Append(jsrq + "|");
            inputParam.Append(cyrq + "|");
            inputParam.Append(cyyy + "|");
            inputParam.Append(cyzdbm + "|");
            inputParam.Append(yjslb + "|");
            inputParam.Append(ztjsbz + "|");
            inputParam.Append(jbr + "|");
            inputParam.Append(fmrq + "|");
            inputParam.Append(cs + "|");
            inputParam.Append(tes + "|");
            inputParam.Append(kh + "|");
            inputParam.Append(zybh + "|");
            inputParam.Append(ksbm + "|");
            inputParam.Append(ysbm + "|");
            inputParam.Append(sfghjs + "|");
            inputParam.Append(zsekh + "|");
            inputParam.Append(ssbz + "|");

            //入参
            StringBuilder inputData = new StringBuilder();
            inputData.Append(YWBH + "^");
            inputData.Append(YLGHBH + "^");
            inputData.Append(CZYBH + "^");
            inputData.Append(YWZQH + "^");
            inputData.Append(JYLSH + "^");
            inputData.Append(JRFS + "^");
            inputData.Append(DKLX + "^");
            inputData.Append(inputParam + "^");
            #endregion

            StringBuilder outputData = new StringBuilder(102400);
            WriteLog(sysdate + "  进入住院费用预结算...");
            WriteLog(sysdate + "  入参|" + inputData.ToString());

            int i = BUSINESS_HANDLE(inputData, outputData);
            if (i == 0)
            {
                WriteLog(sysdate + "  住院费用预结算成功|" + outputData.ToString());
                #region 出参赋值
                string[] str = outputData.ToString().Split('^')[1].Split('|');
                decimal ylzfy = Convert.ToDecimal(str[0]);         //医疗总费用
                
                decimal tcjjzf = Convert.ToDecimal(str[1]);        //统筹基金支付
                decimal dejjzf = Convert.ToDecimal(str[2]);        //本次大病救助支付
                decimal dbbyzf = Convert.ToDecimal(str[3]);         //本次大病保险支付  
                decimal mzfdfy = Convert.ToDecimal(str[4]);       //本次民政补助支付
                decimal zhzf = Convert.ToDecimal(str[5]);          //本次帐户支付总额
                decimal xjzf = Convert.ToDecimal(str[6]);          //本次现金支付总额
                decimal zbxje = ylzfy - xjzf;         //总报销金额

                decimal zhzfzf = Convert.ToDecimal(str[7]);          //本次帐户支付自付
                decimal zhzfzl = Convert.ToDecimal(str[8]);          //本次帐户支付自理
                decimal xjzfzf = Convert.ToDecimal(str[9]);          //本次帐户支付自付
                decimal xjzfzl = Convert.ToDecimal(str[10]);          //本次帐户支付自理
                decimal ybfwnfy = Convert.ToDecimal(str[11]);       //医保范围内费用
                decimal bcjsqzhye = Convert.ToDecimal(str[12]) + zhzf;       //本次结算前帐户余额
                string dbzbm = str[13];        //单病种病种编码
                string smxx = str[14];          //说明信息
                decimal yfhj = Convert.ToDecimal(str[15]);       //药费合计
                decimal zlxmfhj = Convert.ToDecimal(str[16]);       //诊疗项目费合计
                decimal bbzf = Convert.ToDecimal(str[17]);       //补保支付
                string yllb_r = str[18];    //医疗类别
                string by6 = str[19];       //备用6

                decimal gwybzjjzf = Convert.ToDecimal(0.00);     //公务员补助基金支付
                decimal qybcylbxjjzf = Convert.ToDecimal(0.00);  //企业补充医疗保险基金支付
                decimal dwfdfy = Convert.ToDecimal(0.00);        //单位负担费用    
                decimal yyfdfy = Convert.ToDecimal(0.00);       //医院负担费用
                //decimal mzfdfy = Convert.ToDecimal("0.00");       //民政负担费用
                decimal cxjfy = Convert.ToDecimal(0.00);        //超限价费用单病种病种编码
                decimal ylzlfy = Convert.ToDecimal(0.00);       //乙类自理费用
                decimal blzlfy = Convert.ToDecimal(0.00);       //丙类自理费用
                decimal fhjbylfy = Convert.ToDecimal(0.00);     //符合基本医疗费用
                decimal qfbzfy = Convert.ToDecimal(0.00);       //起付标准费用
                decimal zzzyzffy = Convert.ToDecimal(0.00);     //转诊转院自付费用
                decimal jrtcfy = Convert.ToDecimal(0.00);       //进入统筹费用
                decimal tcfdzffy = Convert.ToDecimal(0.00);     //统筹分段自付费用
                decimal ctcfdxfy = Convert.ToDecimal(0.00);       //超统筹封顶线费用
                decimal jrdebsfy = Convert.ToDecimal(0.00);       //进入大额报销费用
                decimal defdzffy = Convert.ToDecimal(0.00);       //大额分段自付费用
                decimal cdefdxfy = Convert.ToDecimal(0.00);       //超大额封顶线费用
                decimal rgqgzffy = Convert.ToDecimal(0.00);       //人工器管自付费用
                //decimal bcjsqzfye = Convert.ToDecimal(0.00);       //本次结算前帐户余额
                decimal bntczflj = Convert.ToDecimal(0.00);       //本年统筹支付累计
                decimal bndezflj = Convert.ToDecimal(0.00);       //本年大额支付累计
                decimal bnczjmmztczflj = Convert.ToDecimal(0.00);       //本年城镇居民门诊统筹支付累计
                decimal bngwybzzflj = Convert.ToDecimal(0.00);  //本年公务员补助支付累计(不含本次)
                decimal bnzhzflj = Convert.ToDecimal(0.00);  //本年账户支付累计(不含本次)
                string bnzycslj = "1";  //本年住院次数累计(不含本次)
                string zycs = "1";        //住院次数
                string yldylb = ""; //医疗待遇类别
                string jbjgbm = ""; //经办机构编码
                #endregion

                /*
                    *医疗费总额|总报销金额|统筹基金支付|大额基金支付|账户支付|
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
                    * 居民个人自付二次补偿金额|体检金额|生育基金支付|本次民政补助支付|医保范围内费用|
                    * 医疗类别|医保补差|   
                    */

                string strValue = ylzfy + "|" + zbxje + "|" + tcjjzf + "|" + dejjzf + "|" + zhzf + "|" +
                    xjzf + "|" + gwybzjjzf + "|" + qybcylbxjjzf + "|" + "0.00" + "|" + dwfdfy + "|" +
                  yyfdfy + "|" + mzfdfy + "|" + cxjfy + "|" + ylzlfy + "|" + blzlfy + "|" +
                  fhjbylfy + "|" + qfbzfy + "|" + zzzyzffy + "|" + jrtcfy + "|" + tcfdzffy + "|" +
                  ctcfdxfy + "|" + "0.00" + "|" + defdzffy + "|" + cdefdxfy + "|" + rgqgzffy + "|" +
                  bcjsqzhye + "|" + bntczflj + "|" + bndezflj + "|" + bnczjmmztczflj + "|" + bngwybzzflj + "|" +
                      bnzhzflj + "|" + bnzycslj + "|" + zycs + "|" + xm + "|" + jsrq + "|" +
                      yllb_r + "||" + YLGHBH + "|" + YWZQH + "|" + djh + "|" +
                      "|" + djh + "||" + JYLSH + "|1|" +
                      "|" + YLGHBH + "|0.00|||" +
                      grbh + "|0.00|0.00|0.00|0.00|" +
                      "0.00|0.00|0.00|" + dbbyzf + "|" + ybfwnfy + "||";
                WriteLog(sysdate + "  住院费用预结算返回参数|" + strValue);
                strSql = string.Format(@"insert into ybfyyjsdr (jzlsh,jylsh,djhin,cyrq,cyyy,bzbm,bzmc,yllb,xm,kh,
                                        grbh,ylfze,zbxje,tcjjzf,dejjzf,dbbyzf,mzjzfy,zhzf,xjzf,zhzfzf,
                                        zhzfzl,xjzhzf,xjzhzl,ybfwnfy,bcjsqzhye,dbzbc,smxx,yfhj,zlxmfhj,bbzf,
                                        bz6,jbr,ybjzlsh,cfmxjylsh,cxbz,sysdate,jsrq,djh) values(
                                        '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}',
                                        '{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}',
                                        '{20}','{21}','{22}','{23}','{24}','{25}','{26}','{27}','{28}','{29}',
                                        '{30}','{31}','{32}','{33}','{34}','{35}','{36}','{37}')",
                                     jzlsh, JYLSH, djh, cyrq, cyyy, cyzdbm, cyzdmc, yllb, xm, kh,
                                     grbh, ylzfy, zbxje, tcjjzf, dejjzf, dbbyzf, mzfdfy, zhzf, xjzf, zhzfzf,
                                     zhzfzl, xjzfzf, xjzfzl, ybfwnfy, bcjsqzhye, dbzbm, smxx, yfhj, zlxmfhj, bbzf,
                                     by6, jbrbh, ybjzlsh, cfmxjylsh, 1, sysdate, jsrq, djh);
                object[] obj = { strSql };
                obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);
                if (obj[1].ToString().Equals("1"))
                {
                    WriteLog(sysdate + " 住院费用结算成功|" + strValue);
                    return new object[] { 0, 1, strValue };
                }
                else
                {
                    WriteLog(sysdate + " 住院费用结算失败|数据操作失败|" + obj[2].ToString());
                    //撤销结算信息
                    object[] objFYDJCX = { ybjzlsh, djh, "0" };
                    objFYDJCX = N_YBMZZYFYJSCX(objFYDJCX);
                    return new object[] { 0, 0, "住院费用结算失败|数据操作失败|" + obj[2].ToString() };

                }
                return new object[] { 0, 1, strValue };
            }
            else
            {
                WriteLog(sysdate + "  住院费用预结算失败" + outputData.ToString());
                return new object[] { 0, 0, outputData.ToString() };
            }

        }
        #endregion

        #region 住院费用结算
        public static object[] YBZYSFJS(object[] objParam)
        {
            string sysdate = GetServerDateTime();//系统时间
            string jzlsh = objParam[0].ToString();   // 就诊流水号
            string djh = objParam[1].ToString();   //单据号
            string cyyy = objParam[2].ToString();    // 出院原因代码
            string zhsybz = ""; //objParam[3].ToString();  // 账户使用标志（0或1）
            string ztjsbz = objParam[4].ToString(); //中途结算标志
            string cyrq = objParam[5].ToString();//出院日期
            //string ylfhj1 = objParam[6].ToString(); //医疗费合计 
            string cfmxjylsh = "";

            string strSql = string.Format(@"select * from ybmzzydjdr where jzlsh='{0}' and cxbz=1", jzlsh);
            DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            if (ds.Tables[0].Rows.Count == 0)
                return new object[] { 0, 0, "该患者未医保门诊登记" };
            string yllb = ds.Tables[0].Rows[0]["yllb"].ToString(); //医疗类别
            string kh = ds.Tables[0].Rows[0]["kh"].ToString();  //社会保障卡号
            string ksbm = ds.Tables[0].Rows[0]["ksbh"].ToString();  //科室编码
            string ysbm = ds.Tables[0].Rows[0]["ysdm"].ToString();  //医生编码
            string cyzdbm = ds.Tables[0].Rows[0]["bzbm"].ToString();     //出院诊断疾病编码
            string cyzdmc = ds.Tables[0].Rows[0]["bzmc"].ToString();     //出院诊断疾病名称
            string xm = ds.Tables[0].Rows[0]["xm"].ToString();  //姓名
            string grbh = ds.Tables[0].Rows[0]["grbh"].ToString();  //医保编号
            string ybjzlsh = ds.Tables[0].Rows[0]["ybjzlsh"].ToString();    //医保就诊流水号

            strSql = string.Format("select a.z1outd as cyrq from zy01d a where left(a.z1endv, 1) = '8' and a.z1zyno = '{0}'", jzlsh);
            ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            if (ds == null || ds.Tables[0].Rows.Count == 0)
                return new object[] { 0, 0, "就诊流水号" + jzlsh + "未拖出床位" };
            //cyrq = Convert.ToDateTime(ds.Tables[0].Rows[0]["cyrq"]).ToString("yyyyMMddHHmmss"); //出院日期
            cyrq = Convert.ToDateTime(cyrq).ToString("yyyyMMddHHmmss"); //出院日期
            #region 如果是中途结算，则需要对已上传费用进行撤销再上传操作
            if (ztjsbz.Equals("1"))
            {
                object[] objCFSCCX = { jzlsh, "" };
                objCFSCCX = YBZYSFDJCX(objCFSCCX);
                if (!objCFSCCX[1].ToString().Equals("1"))
                    return new object[] { 0, 0, "住院费用结算失败|" + objCFSCCX[2].ToString() };
                object[] objCFSC = { jzlsh, cyrq };
                objCFSC = YBZYSFDJ(objCFSC);
                if (!objCFSC[1].ToString().Equals("1"))
                    return new object[] { 0, 0, "住院费用结算失败|" + objCFSC[2].ToString() };
                cfmxjylsh = objCFSC[2].ToString();
            }
            #endregion

            YWBH = "2410";
            JYLSH = DateTime.Now.ToString("yyyyMMddHHmmss") + "-" + YLGHBH + "-" + new Random().Next(100).ToString().PadLeft(4, '0');
            CZYBH = CliUtils.fLoginUser;
            YWZQH = CliUtils.fLoginYbNo.Split('|')[0];
            string jbr = CliUtils.fUserName;    //经办人
            string jbrbh = CliUtils.fLoginUser;

            #region 入参赋值
            string jsrq = Convert.ToDateTime(sysdate).ToString("yyyyMMddHHmmss");   //结算日期
           
            string yjslb = "";      //月结算类别
            string fmrq = "";   //分娩日期
            string cs = "";     //产次
            string tes = "";    //胎儿数
            string zybh = "";   //转院医院编号
            string zsekh = "";  //准生儿社会保障卡号
            string ssbz = "";   //手术是否成功标志
            string sfghjs = "";

            //入参数据
            StringBuilder inputParam = new StringBuilder();
            inputParam.Append(ybjzlsh + "|");
            inputParam.Append(djh + "|");
            inputParam.Append(yllb + "|");
            inputParam.Append(jsrq + "|");
            inputParam.Append(cyrq + "|");
            inputParam.Append(cyyy + "|");
            inputParam.Append(cyzdbm + "|");
            inputParam.Append(yjslb + "|");
            inputParam.Append(ztjsbz + "|");
            inputParam.Append(jbr + "|");
            inputParam.Append(fmrq + "|");
            inputParam.Append(cs + "|");
            inputParam.Append(tes + "|");
            inputParam.Append(kh + "|");
            inputParam.Append(zybh + "|");
            inputParam.Append(ksbm + "|");
            inputParam.Append(ysbm + "|");
            inputParam.Append(sfghjs + "|");
            inputParam.Append(zsekh + "|");

            //入参
            StringBuilder inputData = new StringBuilder();
            inputData.Append(YWBH + "^");
            inputData.Append(YLGHBH + "^");
            inputData.Append(CZYBH + "^");
            inputData.Append(YWZQH + "^");
            inputData.Append(JYLSH + "^");
            inputData.Append(JRFS + "^");
            inputData.Append(DKLX + "^");
            inputData.Append(inputParam + "^");
            #endregion

            StringBuilder outputData = new StringBuilder(102400);
            WriteLog(sysdate + "  进入住院费用结算...");
            WriteLog(sysdate + "  入参|" + inputData.ToString());

            int i = BUSINESS_HANDLE(inputData, outputData);
            if (i == 0)
            {
                WriteLog(sysdate + "  住院费用结算成功|" + outputData.ToString());
                #region 出参赋值
                string[] str = outputData.ToString().Split('^')[1].Split('|');
                decimal ylzfy = Convert.ToDecimal(str[0]);         //医疗总费用
                decimal zbxje = Convert.ToDecimal(0.00);         //总报销金额
                decimal tcjjzf = Convert.ToDecimal(str[1]);        //统筹基金支付
                decimal dejjzf = Convert.ToDecimal(str[2]);        //本次大病救助支付
                decimal dbbyzf = Convert.ToDecimal(str[3]);         //本次大病保险支付  
                decimal mzfdfy = Convert.ToDecimal(str[4]);       //本次民政补助支付
                decimal zhzf = Convert.ToDecimal(str[5]);          //本次帐户支付总额
                decimal xjzf = Convert.ToDecimal(str[6]);          //本次现金支付总额

                decimal zhzfzf = Convert.ToDecimal(str[7]);          //本次帐户支付自付
                decimal zhzfzl = Convert.ToDecimal(str[8]);          //本次帐户支付自理
                decimal xjzfzf = Convert.ToDecimal(str[9]);          //本次帐户支付自付
                decimal xjzfzl = Convert.ToDecimal(str[10]);          //本次帐户支付自理
                decimal ybfwnfy = Convert.ToDecimal(str[11]);       //医保范围内费用
                decimal bcjsqzhye = Convert.ToDecimal(str[12]) + zhzf;       //本次结算前帐户余额
                string dbzbm = str[13];        //单病种病种编码
                string smxx = str[14];          //说明信息
                decimal yfhj = Convert.ToDecimal(str[15]);       //药费合计
                decimal zlxmfhj = Convert.ToDecimal(str[16]);       //诊疗项目费合计
                decimal bbzf = Convert.ToDecimal(str[17]);       //补保支付
                string yllb_r = str[18];    //医疗类别
                string by6 = str[19];       //备用6

                decimal gwybzjjzf = Convert.ToDecimal(0.00);     //公务员补助基金支付
                decimal qybcylbxjjzf = Convert.ToDecimal(0.00);  //企业补充医疗保险基金支付
                decimal dwfdfy = Convert.ToDecimal(0.00);        //单位负担费用    
                decimal yyfdfy = Convert.ToDecimal(0.00);       //医院负担费用
                //decimal mzfdfy = Convert.ToDecimal("0.00");       //民政负担费用
                decimal cxjfy = Convert.ToDecimal(0.00);        //超限价费用单病种病种编码
                decimal ylzlfy = Convert.ToDecimal(0.00);       //乙类自理费用
                decimal blzlfy = Convert.ToDecimal(0.00);       //丙类自理费用
                decimal fhjbylfy = Convert.ToDecimal(0.00);     //符合基本医疗费用
                decimal qfbzfy = Convert.ToDecimal(0.00);       //起付标准费用
                decimal zzzyzffy = Convert.ToDecimal(0.00);     //转诊转院自付费用
                decimal jrtcfy = Convert.ToDecimal(0.00);       //进入统筹费用
                decimal tcfdzffy = Convert.ToDecimal(0.00);     //统筹分段自付费用
                decimal ctcfdxfy = Convert.ToDecimal(0.00);       //超统筹封顶线费用
                decimal jrdebsfy = Convert.ToDecimal(0.00);       //进入大额报销费用
                decimal defdzffy = Convert.ToDecimal(0.00);       //大额分段自付费用
                decimal cdefdxfy = Convert.ToDecimal(0.00);       //超大额封顶线费用
                decimal rgqgzffy = Convert.ToDecimal(0.00);       //人工器管自付费用
                decimal bcjsqzfye = Convert.ToDecimal(0.00);       //本次结算前帐户余额
                decimal bntczflj = Convert.ToDecimal(0.00);       //本年统筹支付累计
                decimal bndezflj = Convert.ToDecimal(0.00);       //本年大额支付累计
                decimal bnczjmmztczflj = Convert.ToDecimal(0.00);       //本年城镇居民门诊统筹支付累计
                decimal bngwybzzflj = Convert.ToDecimal(0.00);  //本年公务员补助支付累计(不含本次)
                decimal bnzhzflj = Convert.ToDecimal(0.00);  //本年账户支付累计(不含本次)
                string bnzycslj = "1";  //本年住院次数累计(不含本次)
                string zycs = "1";        //住院次数
                string yldylb = ""; //医疗待遇类别
                string jbjgbm = ""; //经办机构编码
                #endregion


                zbxje = ylzfy - xjzf;
                /*
                    *医疗费总额|总报销金额|统筹基金支付|大额基金支付|账户支付|
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
                    * 居民个人自付二次补偿金额|体检金额|生育基金支付|本次民政补助支付|医保范围内费用|
                    */

                string strValue = ylzfy + "|" + zbxje + "|" + tcjjzf + "|" + dejjzf + "|" + zhzf + "|" + 
                    xjzf + "|" + gwybzjjzf + "|" + qybcylbxjjzf + "|" + "0.00" + "|" + dwfdfy + "|" +
                  yyfdfy + "|" + mzfdfy + "|" + cxjfy + "|" + ylzlfy + "|" + blzlfy + "|" + 
                  fhjbylfy + "|" + qfbzfy + "|" + zzzyzffy + "|" + jrtcfy + "|" + tcfdzffy + "|" +
                  ctcfdxfy + "|" + "0.00" + "|" + defdzffy + "|" + cdefdxfy + "|" + rgqgzffy + "|" +
                  bcjsqzhye + "|" + bntczflj + "|" + bndezflj + "|" + bnczjmmztczflj + "|" + bngwybzzflj + "|" +
                      bnzhzflj + "|" + bnzycslj + "|" + zycs + "|" + xm + "|" + jsrq + "|" +
                      yllb_r + "||" + YLGHBH + "|" + YWZQH + "|" + djh + "|" +
                      "|" + djh + "||" + JYLSH + "|1|" +
                      "|" + YLGHBH + "|0.00|||" +
                      grbh + "|0.00|0.00|0.00|0.00|" +
                      "0.00|0.00|0.00|" + dbbyzf + "|" + ybfwnfy + "|";
                WriteLog(sysdate + "  住院费用预结算返回参数|" + strValue);
                strSql = string.Format(@"insert into ybfyjsdr (jzlsh,jylsh,djhin,cyrq,cyyy,bzbm,bzmc,yllb,xm,kh,
                                        grbh,ylfze,zbxje,tcjjzf,dejjzf,dbbyzf,mzjzfy,zhzf,xjzf,zhzfzf,
                                        zhzfzl,xjzhzf,xjzhzl,ybfwnfy,bcjsqzhye,dbzbc,smxx,yfhj,zlxmfhj,bbzf,
                                        bz6,jbr,ybjzlsh,cfmxjylsh,cxbz,sysdate,jsrq,djh) values(
                                        '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}',
                                        '{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}',
                                        '{20}','{21}','{22}','{23}','{24}','{25}','{26}','{27}','{28}','{29}',
                                        '{30}','{31}','{32}','{33}','{34}','{35}','{36}','{37}')",
                                      jzlsh, JYLSH, djh, cyrq, cyyy, cyzdbm, cyzdmc, yllb, xm, kh,
                                      grbh, ylzfy, zbxje, tcjjzf, dejjzf, dbbyzf, mzfdfy, zhzf, xjzf, zhzfzf,
                                      zhzfzl, xjzfzf, xjzfzl, ybfwnfy, bcjsqzhye, dbzbm, smxx, yfhj, zlxmfhj, bbzf,
                                      by6, jbrbh, ybjzlsh, cfmxjylsh, 1, sysdate, jsrq, djh);
                object[] obj = { strSql };
                obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);
                if (obj[1].ToString().Equals("1"))
                {
                    WriteLog(sysdate + " 住院费用结算成功|" + strValue);
                    return new object[] { 0, 1, strValue };
                }
                else
                {
                    WriteLog(sysdate + " 住院费用结算失败|数据操作失败|" + obj[2].ToString());
                    //撤销结算信息
                    object[] objFYDJCX = { ybjzlsh, djh, "0" };
                    objFYDJCX = N_YBMZZYFYJSCX(objFYDJCX);
                    return new object[] { 0, 0, "住院费用结算失败|数据操作失败|" + obj[2].ToString() };

                }
            }
            else
            {
                WriteLog(sysdate + "  住院费用预结算失败" + outputData.ToString());
                return new object[] { 0, 0, outputData.ToString() };
            }
        }
        #endregion

        #region 住院费用结算撤销
        public static object[] YBZYSFJSCX(object[] objParam)
        {
            string sysdate = GetServerDateTime();//当前时间
            string jzlsh = objParam[0].ToString();   // 就诊流水号
            string djh = objParam[1].ToString();     // 单据号(发票号)

            YWBH = "2430";
            JYLSH = DateTime.Now.ToString("yyyyMMddHHmmss") + "-" + YLGHBH + "-" + new Random().Next(100).ToString().PadLeft(4, '0');
            CZYBH = CliUtils.fLoginUser;
            YWZQH = CliUtils.fLoginYbNo.Split('|')[0];


            if (string.IsNullOrEmpty(jzlsh))
                return new object[] { 0, 0, "就诊流水号不能为空" };
            if (string.IsNullOrEmpty(djh))
                return new object[] { 0, 0, "单据号不能为空" };

            //判断是否已结算
            string strSql = string.Format(@"select * from ybfyjsdr where jzlsh='{0}' and djhin='{1}' and cxbz=1", jzlsh, djh);
            DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            if (ds.Tables[0].Rows.Count == 0)
                return new object[] { 0, 0, "该患者无医保结算信息" };
            string ybjzlsh = ds.Tables[0].Rows[0]["ybjzlsh"].ToString();
            string cfmxjylsh = ds.Tables[0].Rows[0]["cfmxjylsh"].ToString();

            string jscxrq = Convert.ToDateTime(sysdate).ToString("yyyyMMddHHmmss");//结算撤销日期
            string jbr = CliUtils.fUserName;//经办人
            string sfblcfbz = "1"; //是否保留处方标志
            string by1 = "";    //备用1
            string by2 = "";    //备用2
            string by3 = "";    //备用3
            string by4 = "";    //备用4
            string by5 = "";    //备用5
            string by6 = "";    //备用6

            //入参数据
            StringBuilder inputParam = new StringBuilder();
            inputParam.Append(ybjzlsh + "|");
            inputParam.Append(djh + "|");
            inputParam.Append(jscxrq + "|");
            inputParam.Append(jbr + "|");
            inputParam.Append(sfblcfbz + "|");
            inputParam.Append(by1 + "|");
            inputParam.Append(YLJGGJM + "|"); //20200501修改
            inputParam.Append(by3 + "|");
            inputParam.Append(by4 + "|");
            inputParam.Append(by5 + "|");
            inputParam.Append(by6 + "|");

            //入参
            StringBuilder inputData = new StringBuilder();
            inputData.Append(YWBH + "^");
            inputData.Append(YLGHBH + "^");
            inputData.Append(CZYBH + "^");
            inputData.Append(YWZQH + "^");
            inputData.Append(JYLSH + "^");
            inputData.Append(JRFS + "^");
            inputData.Append(DKLX + "^");
            inputData.Append(inputParam + "^");

            WriteLog(sysdate + "  进入住院费用结算撤销...");
            WriteLog(sysdate + "  入参|" + inputData.ToString());
            StringBuilder outputData = new StringBuilder(102400);
            List<string> liSQL = new List<string>();

            int i = BUSINESS_HANDLE(inputData, outputData);
            if (i == 0)
            {
                WriteLog(sysdate + "  住院费用结算撤销成功|出参|" + outputData.ToString());
                strSql = string.Format(@"insert into ybfyjsdr (jzlsh,jylsh,djhin,cyrq,cyyy,bzbm,bzmc,yllb,xm,kh,
                                        grbh,ylfze,zbxje,tcjjzf,dejjzf,dbbyzf,mzjzfy,zhzf,xjzf,zhzfzf,
                                        zhzfzl,xjzhzf,xjzhzl,ybfwnfy,bcjsqzhye,dbzbc,smxx,yfhj,zlxmfhj,bbzf,
                                        bz6,jsrq,jbr,djh,cxbz,sysdate) select jzlsh,jylsh,djhin,cyrq,cyyy,bzbm,bzmc,yllb,xm,kh,
                                        grbh,ylfze,zbxje,tcjjzf,dejjzf,dbbyzf,mzjzfy,zhzf,xjzf,zhzfzf,
                                        zhzfzl,xjzhzf,xjzhzl,ybfwnfy,bcjsqzhye,dbzbc,smxx,yfhj,zlxmfhj,bbzf,
                                        bz6,jsrq,djh,'{3}',0,'{2}' from ybfyjsdr where jzlsh='{0}' and djhin='{1}' and cxbz=1", jzlsh, djh, sysdate, jbr);
                liSQL.Add(strSql);
                strSql = string.Format(@"update ybfyjsdr set cxbz=2 where jzlsh='{0}' and djhin='{1}' and cxbz=1", jzlsh, djh);
                liSQL.Add(strSql);
                object[] obj = liSQL.ToArray();
                obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);
                if (obj[1].ToString().Equals("1"))
                {
                    WriteLog(sysdate + "  住院费用结算撤销成功|数据操作成功|");
                    //住字费用登记撤销
                    object[] objFYDJCX = { jzlsh, cfmxjylsh };
                    objFYDJCX = YBZYSFDJCX(objFYDJCX);
                    if (objFYDJCX[1].ToString().Equals("1"))
                        return new object[] { 0, 1, "住院费用结算撤销成功|费用登记撤销成功" };
                    else
                        return new object[] { 0, 0, "住院费用结算撤销成功|费用登记撤销失败" };
                }
                else
                {
                    WriteLog(sysdate + "  住院费用结算撤销失败|数据操作失败|" + obj[2].ToString());
                    return new object[] { 0, 0, "住院费用结算撤销失败|数据操作失败|" + obj[2].ToString() };
                }
            }
            else
            {
                WriteLog(sysdate + "  住院费用结算撤销失败|" + outputData.ToString());
                return new object[] { 0, 0, "住院费用结算撤销失败|" + outputData.ToString() };
            }
        }
        #endregion

        #region 医疗待遇审批信息查询
        public static object[] YBYLDYSPXXCX(object[] ojbParam)
        {
            string sysdate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            WriteLog(sysdate + "  进入医疗待遇审批信息查询...");
            return new object[] { 0, 0, "无医疗待遇审批信息查询接口" };
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
            WriteLog(sysdate + "  医疗待遇封锁信息未启用...");
            return new object[] { 0, 1, "医疗待遇封锁信息未启用" };
        }
        #endregion

        #region 医保对账
        public static object[] YBDZ(object[] objParam)
        {
            string dtStart = Convert.ToDateTime(objParam[0].ToString()).ToString("yyyyMMdd");
            string dtEnd = Convert.ToDateTime(objParam[1].ToString()).ToString("yyyyMMdd");


            YWBH = "1120";
            JYLSH = DateTime.Now.ToString("yyyyMMddHHmmss") + "-" + YLGHBH + "-" + new Random().Next(100).ToString().PadLeft(4, '0');
            CZYBH = CliUtils.fLoginUser;
            YWZQH = CliUtils.fLoginYbNo.Split('|')[0];

            StringBuilder inputParam = new StringBuilder();
            inputParam.Append(dtStart + "|");
            inputParam.Append(dtEnd + "|");

            //入参
            StringBuilder inputData = new StringBuilder();
            inputData.Append(YWBH + "^");
            inputData.Append(YLGHBH + "^");
            inputData.Append(CZYBH + "^");
            inputData.Append(YWZQH + "^");
            inputData.Append(JYLSH + "^");
            inputData.Append(JRFS + "^");
            inputData.Append(DKLX + "^");
            inputData.Append(inputParam + "^");

            StringBuilder outputData = new StringBuilder(1024000);

            int i = BUSINESS_HANDLE(inputData, outputData);
            if (i == 0)
            {
                WriteLog("总额对帐成功|" + outputData.ToString());
                return new object[] { 0, 1, outputData.ToString() };
            }
            else
            {
                WriteLog("总额对帐失败|" + outputData.ToString());
                return new object[] { 0, 0, outputData.ToString() };
            }
        }
        #endregion

        #region 费用明细下载
        public static object[] YBFYMXXZ(object[] objParam)
        {
            string dtStart = Convert.ToDateTime(objParam[0].ToString()).ToString("yyyyMMdd");
            string dtEnd = Convert.ToDateTime(objParam[1].ToString()).ToString("yyyyMMdd");


            YWBH = "1100";
            JYLSH = DateTime.Now.ToString("yyyyMMddHHmmss") + "-" + YLGHBH + "-" + new Random().Next(100).ToString().PadLeft(4, '0');
            CZYBH = CliUtils.fLoginUser;
            YWZQH = CliUtils.fLoginYbNo.Split('|')[0];

            StringBuilder inputParam = new StringBuilder();
            inputParam.Append(dtStart + "|");
            inputParam.Append(dtEnd + "|");

            //入参
            StringBuilder inputData = new StringBuilder();
            inputData.Append(YWBH + "^");
            inputData.Append(YLGHBH + "^");
            inputData.Append(CZYBH + "^");
            inputData.Append(YWZQH + "^");
            inputData.Append(JYLSH + "^");
            inputData.Append(JRFS + "^");
            inputData.Append(DKLX + "^");
            inputData.Append(inputParam + "^");

            StringBuilder outputData = new StringBuilder(1024000);

            int i = BUSINESS_HANDLE(inputData, outputData);
            if (i == 0)
            {
                WriteLog("费用明细下载成功|" + outputData.ToString());
                return new object[] { 0, 1, outputData.ToString() };
            }
            else
            {
                WriteLog("费用明细下载失败|" + outputData.ToString());
                return new object[] { 0, 0, outputData.ToString() };
            }
        }
        #endregion

        #region 费用明细详细信息
        public static object[] YBFYMXXXXX(object[] objParam)
        {
            string dtStart = Convert.ToDateTime(objParam[0].ToString()).ToString("yyyyMMdd");
            string dtEnd = Convert.ToDateTime(objParam[1].ToString()).ToString("yyyyMMdd");


            YWBH = "1102";
            JYLSH = DateTime.Now.ToString("yyyyMMddHHmmss") + "-" + YLGHBH + "-" + new Random().Next(100).ToString().PadLeft(4, '0');
            CZYBH = CliUtils.fLoginUser;
            YWZQH = CliUtils.fLoginYbNo.Split('|')[0];

            StringBuilder inputParam = new StringBuilder();
            inputParam.Append(dtStart + "|");
            inputParam.Append(dtEnd + "|");
            inputParam.Append(YWZQH + "|");

            //入参
            StringBuilder inputData = new StringBuilder();
            inputData.Append(YWBH + "^");
            inputData.Append(YLGHBH + "^");
            inputData.Append(CZYBH + "^");
            inputData.Append(YWZQH + "^");
            inputData.Append(JYLSH + "^");
            inputData.Append(JRFS + "^");
            inputData.Append(DKLX + "^");
            inputData.Append(inputParam + "^");

            StringBuilder outputData = new StringBuilder(1024000);

            int i = BUSINESS_HANDLE(inputData, outputData);
            if (i == 0)
            {
                WriteLog("费用明细详细信息下载成功|" + outputData.ToString());
                return new object[] { 0, 1, outputData.ToString() };
            }
            else
            {
                WriteLog("费用明细详细信息下载失败|" + outputData.ToString());
                return new object[] { 0, 0, outputData.ToString() };
            }
        }
        #endregion

        #region 明细对帐
        public static object[] YBMXDZ(object[] objParam)
        {
            string dtStart = Convert.ToDateTime(objParam[0].ToString()).ToString("yyyyMMdd");
            string dtEnd = Convert.ToDateTime(objParam[1].ToString()).ToString("yyyyMMdd");


            YWBH = "1210";
            JYLSH = DateTime.Now.ToString("yyyyMMddHHmmss") + "-" + YLGHBH + "-" + new Random().Next(100).ToString().PadLeft(4, '0');
            CZYBH = CliUtils.fLoginUser;
            YWZQH = CliUtils.fLoginYbNo.Split('|')[0];

            StringBuilder inputParam = new StringBuilder();
            inputParam.Append(dtStart + "|");
            inputParam.Append(dtEnd + "|");

            //入参
            StringBuilder inputData = new StringBuilder();
            inputData.Append(YWBH + "^");
            inputData.Append(YLGHBH + "^");
            inputData.Append(CZYBH + "^");
            inputData.Append(YWZQH + "^");
            inputData.Append(JYLSH + "^");
            inputData.Append(JRFS + "^");
            inputData.Append(DKLX + "^");
            inputData.Append(inputParam + "^");

            StringBuilder outputData = new StringBuilder(1024000);

            int i = BUSINESS_HANDLE(inputData, outputData);
            if (i == 0)
            {
                WriteLog("明细对帐成功|" + outputData.ToString());
                return new object[] { 0, 1, outputData.ToString() };
            }
            else
            {
                WriteLog("明细对帐失败|" + outputData.ToString());
                return new object[] { 0, 0, outputData.ToString() };
            }
        }
        #endregion

        #region 冲正交易
        public static object[] YBCZJY(object[] objParam)
        {
            string czywbm = objParam[0].ToString();
            string czjylsh = objParam[1].ToString();


            YWBH = "2421";
            JYLSH = DateTime.Now.ToString("yyyyMMddHHmmss") + "-" + YLGHBH + "-" + new Random().Next(100).ToString().PadLeft(4, '0');
            CZYBH = CliUtils.fLoginUser;
            YWZQH = CliUtils.fLoginYbNo.Split('|')[0];

            StringBuilder inputParam = new StringBuilder();
            inputParam.Append(czywbm + "|");
            inputParam.Append(czjylsh + "|");

            //入参
            StringBuilder inputData = new StringBuilder();
            inputData.Append(YWBH + "^");
            inputData.Append(YLGHBH + "^");
            inputData.Append(CZYBH + "^");
            inputData.Append(YWZQH + "^");
            inputData.Append(JYLSH + "^");
            inputData.Append(JRFS + "^");
            inputData.Append(DKLX + "^");
            inputData.Append(inputParam + "^");

            StringBuilder outputData = new StringBuilder(1024000);

            int i = BUSINESS_HANDLE(inputData, outputData);
            if (i == 0)
            {
                WriteLog("冲正交易成功|" + outputData.ToString());
                return new object[] { 0, 1, outputData.ToString() };
            }
            else
            {
                WriteLog("冲正交易失败|" + outputData.ToString());
                return new object[] { 0, 0, outputData.ToString() };
            }

        }
        #endregion

        #region 门诊/住院登记撤销(内部)
        public static object[] N_YBMZZYDJCX(object[] objParam)
        {
            string sysdate=GetServerDateTime();
            string ybjzlsh = objParam[0].ToString();
            YWBH = "2240";
            JYLSH = DateTime.Now.ToString("yyyyMMddHHmmss") + "-" + YLGHBH + "-" + new Random().Next(100).ToString().PadLeft(4, '0');
            CZYBH = CliUtils.fLoginUser;
            YWZQH = CliUtils.fLoginYbNo.Split('|')[0];
            string jbr = CliUtils.fUserName;
            if (string.IsNullOrEmpty(ybjzlsh))
                return new object[] { 0, 0, "就诊流水号不能为空" };

            StringBuilder inputData = new StringBuilder();
            StringBuilder inputParam = new StringBuilder();
            StringBuilder outputData = new StringBuilder(1024);

            //入参数据
            inputParam.Append(ybjzlsh + "|");
            inputParam.Append(jbr + "|");

            //入参
            inputData.Append(YWBH + "^");
            inputData.Append(YLGHBH + "^");
            inputData.Append(CZYBH + "^");
            inputData.Append(YWZQH + "^");
            inputData.Append(JYLSH + "^");
            inputData.Append(JRFS + "^");
            inputData.Append(DKLX + "^");
            inputData.Append(inputParam + "^");

            WriteLog(sysdate + "  进入医保门诊/住院登记撤销(内部)...");
            WriteLog(sysdate + "  入参|" + inputData.ToString());
            int i = BUSINESS_HANDLE(inputData, outputData);
            if (i == 0)
            {
                WriteLog(sysdate + "  医保门诊/住院登记撤销(内部)成功|");
                    return new object[] { 0, 1, "医保门诊登记撤销(内部)成功" };
            }
            else
            {
                WriteLog(sysdate + "  医保门诊/住院登记撤销(内部)失败|" + outputData.ToString());
                return new object[] { 0, 0, outputData.ToString() };
            }
        }
        #endregion

        #region 门诊/住院费用登记撤销(内部)
        public static object[] N_YBMZZYSFDJCX(object[] objParam)
        {
            string sysdate = GetServerDateTime();
            string jzlsh = objParam[0].ToString();
            string cfh = objParam[1].ToString();
            string cflsh = objParam[2].ToString();
            string jbr = objParam[3].ToString();
            string ybjzlsh = objParam[4].ToString();

         
            YWBH = "2320";
            JYLSH = DateTime.Now.ToString("yyyyMMddHHmmss") + "-" + YLGHBH + "-" + new Random().Next(100).ToString().PadLeft(4, '0');
            CZYBH = CliUtils.fLoginUser;
            YWZQH = CliUtils.fLoginYbNo.Split('|')[0];

            //入参数据
            StringBuilder inputParam = new StringBuilder();
            inputParam.Append(ybjzlsh + "|");
            inputParam.Append(cfh + "|");
            inputParam.Append(cflsh + "|");
            inputParam.Append(jbr + "|");

            //入参
            StringBuilder inputData = new StringBuilder();
            inputData.Append(YWBH + "^");
            inputData.Append(YLGHBH + "^");
            inputData.Append(CZYBH + "^");
            inputData.Append(YWZQH + "^");
            inputData.Append(JYLSH + "^");
            inputData.Append(JRFS + "^");
            inputData.Append(DKLX + "^");
            inputData.Append(inputParam + "^");

            WriteLog(sysdate + "  进入门诊/住院费用登记撤销(内部)...");

            StringBuilder outputData = new StringBuilder(1024);
            int i = BUSINESS_HANDLE(inputData, outputData);
            List<string> liSQL = new List<string>();
            if (i == 0)
            {
                WriteLog(sysdate + "  门诊/住院费用登记撤销(内部)成功|" + outputData.ToString());
                return new object[] { 0, 1, "门诊费用登记撤销(内部)成功" };
            }
            else
            {
                WriteLog(sysdate + "  门诊/住院费用登记撤销(内部)失败|" + outputData.ToString());
                return new object[] { 0, 0, "门诊/住院费用登记撤销(内部)失败|数据操作失败|" + outputData.ToString() };
            }
        }
        #endregion

        #region 门诊/住院费用结算撤销(内部)
        public static object[] N_YBMZZYFYJSCX(object[] objParam)
        {
            string sysdate = GetServerDateTime();//当前时间
            string jzlsh = objParam[0].ToString();   // 就诊流水号
            string djh = objParam[1].ToString();     // 单据号(发票号)
            string sfblcfbz = objParam[2].ToString();   //是否保留处方标志 0-不保留 1-保留
            string ybjzlsh = objParam[3].ToString();

            YWBH = "2430";
            JYLSH = DateTime.Now.ToString("yyyyMMddHHmmss") + "-" + YLGHBH + "-" + new Random().Next(100).ToString().PadLeft(4, '0');
            CZYBH = CliUtils.fLoginUser;
            YWZQH = CliUtils.fLoginYbNo.Split('|')[0];


            if (string.IsNullOrEmpty(jzlsh))
                return new object[] { 0, 0, "就诊流水号不能为空" };
            if (string.IsNullOrEmpty(djh))
                return new object[] { 0, 0, "单据号不能为空" };

         
            string jscxrq = Convert.ToDateTime(sysdate).ToString("yyyyMMddHHmmss");//结算撤销日期
            string jbr = CliUtils.fUserName;//经办人
            string by1 = "";    //备用1
            string by2 = "";    //备用2
            string by3 = "";    //备用3
            string by4 = "";    //备用4
            string by5 = "";    //备用5
            string by6 = "";    //备用6

            //入参数据
            StringBuilder inputParam = new StringBuilder();
            inputParam.Append(ybjzlsh + "|");
            inputParam.Append(djh + "|");
            inputParam.Append(jscxrq + "|");
            inputParam.Append(jbr + "|");
            inputParam.Append(sfblcfbz + "|");
            inputParam.Append(by1 + "|");
            inputParam.Append(by2 + "|");
            inputParam.Append(by3 + "|");
            inputParam.Append(by4 + "|");
            inputParam.Append(by5 + "|");
            inputParam.Append(by6 + "|");

            //入参
            StringBuilder inputData = new StringBuilder();
            inputData.Append(YWBH + "^");
            inputData.Append(YLGHBH + "^");
            inputData.Append(CZYBH + "^");
            inputData.Append(YWZQH + "^");
            inputData.Append(JYLSH + "^");
            inputData.Append(JRFS + "^");
            inputData.Append(DKLX + "^");
            inputData.Append(inputParam + "^");

            WriteLog(sysdate + "  进入门诊/住院费用结算撤销(内部)...");
            WriteLog(sysdate + "  入参|" + inputData.ToString());
            StringBuilder outputData = new StringBuilder(102400);
            List<string> liSQL = new List<string>();

            int i = BUSINESS_HANDLE(inputData, outputData);
            if (i == 0)
            {
                WriteLog(sysdate + "  门诊/住院费用结算撤销成功|出参|" + outputData.ToString());
                WriteLog(sysdate + "  门诊/住院费用结算撤销成功|数据操作成功|");
                if (sfblcfbz.Equals("0"))
                {
                    object[] objFYDJCX = { jzlsh };
                    objFYDJCX = YBMZFYDJCX(objFYDJCX);
                }
                return new object[] { 0, 1, "门诊/住院费用结算撤销成功" };
            }
            else
            {
                WriteLog(sysdate + "  门诊/住院费用结算撤销失败|" + outputData.ToString());
                return new object[] { 0, 0, "门诊/住院费用结算撤销失败|" + outputData.ToString() };
            }
        }
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

        #region 获取病种信息查询
        public static object[] YBBZCX(object[] objParam)
        {
            string yllb = objParam[0].ToString(); // 医疗类别
            string grbh = objParam[1].ToString(); //个人编号
            string jzbz = objParam[2].ToString();   //门诊住院标志 m-门诊 z-住院
            string splb = objParam[3].ToString();   //审批类别

            string[] syl_mz = { "11","13","86"};
            
            string strSql=string.Empty;
            if (jzbz.ToUpper().Equals("M"))
            {
                strSql = string.Format(@"select dm,dmmc,pym,wbm from ybbzmrdr where bz1 in(0,1)");
                if (!syl_mz.Contains(yllb))
                    strSql += string.Format(@" and bz1=1 or LEFT(dm,3) in ('GZY','GZT','GKF')");
            }
            else if (jzbz.ToUpper().Equals("Z"))
            {
                strSql = string.Format(@"select dm,dmmc,pym,wbm from ybbzmrdr where bz1 in(0,2)");
                if (!yllb.Equals("21"))
                    strSql += string.Format(@" and bz1=2 or LEFT(dm,3) in ('GZY','GZT','GKF')");
            }
            else
            {
                return new object[] { 0, 0, "门诊住院标志入参有误" };
            }
            DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            return  new object[] { 0, 1, ds.Tables[0] };
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
