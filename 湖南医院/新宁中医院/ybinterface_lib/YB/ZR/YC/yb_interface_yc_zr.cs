using Srvtools;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Text;

namespace ybinterface_lib.YB.ZR.YC
{
    /// <summary>
    /// 宜春地区医保接口
    /// </summary>
    public class yb_interface_yc_zr
    {

        public static string dqbh = "360925"; //地区编号(靖安县)

        #region 接口DLL加载
        [DllImport("JxdxHisJk.dll", CharSet = CharSet.Ansi)]
        public static extern int f_UserBargaingInit(string UserID, string PassWD, StringBuilder retMsg);

        [DllImport("JxdxHisJk.dll", CharSet = CharSet.Ansi)]
        public static extern int f_UserBargaingClose(StringBuilder retMsg);

        [DllImport("JxdxHisJk.dll", CallingConvention = CallingConvention.StdCall)]
        private static extern int f_UserBargaingApply(string Ywlx, StringBuilder InData, StringBuilder OutData, StringBuilder retMsg);
        #endregion

        #region 变量
        static IWork iWork = new Work();
        static string xmlPath = AppDomain.CurrentDomain.BaseDirectory;
        static List<Item1> lItem = iWork.getXmlConfig1(xmlPath + "EEPNetClient.exe.config");
        static string YBIP = lItem[0].YBIP;     //医保IP地址
        static string YBJGBH = lItem[0].YLJGBH;    //医疗机构编码
        #endregion

        #region 查询类
        #region 批量数据查询下载
        public static object[] YBPLSJCXXZ(object[] objParam)
        {
            string sysdate = GetServerDateTime();
            WriteLog(sysdate + "  进入批量数据查询下载...");
            try
            {
                string sjlb = objParam[0].ToString(); //数据类别
                string startDate = objParam[1].ToString();    //开始时间
                string endData = objParam[2].ToString(); //结束时间

                if (string.IsNullOrEmpty(sjlb))
                    return new object[] { 0, 0, "数据类别不能为空" };

                object[] objReturn = null;

                switch (sjlb)
                {
                    case "04": //疾病诊断目录下载
                        objReturn = YBBZMRXZ(objParam);
                        break;
                    case "08":
                        objReturn = YBMTBZXZ(objParam);
                        break;
                    case "01":
                    case "02":
                    case "03":

                        break;
                }


                return null;
            
            }
            catch (Exception ex)
            {
                WriteLog(sysdate + "  批量数据查询下载异常|" + ex.Message);
                return new object[] { 0, 0, "批量数据查询下载异常|" + ex.Message };
            }
        }
        #endregion

        #region 疾病诊断目录下载
        internal static object[] YBBZMRXZ(object[] objParam)
        {
            string sysdate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string iReturn="0";
            int iCount = 1;
            string Ywlx = "CX02";
            StringBuilder inputData = new StringBuilder();
            StringBuilder outData = new StringBuilder(102400);
            StringBuilder retMsg = new StringBuilder(102400);
            WriteLog(sysdate + "  进入疾病诊断目录下载....");
            List<string> liValue = new List<string>();
            while (iReturn.Equals("0"))
            {
                inputData.Append("SSSS|"); //开始码
                inputData.Append(iCount + "|");
                inputData.Append("ZZZZ");
                WriteLog(sysdate + "  入参|" + inputData.ToString());
                int i = f_UserBargaingApply(Ywlx, inputData, outData, retMsg);
                if (i > 0)
                {
                    WriteLog(sysdate + "  出参|" + outData.ToString());
                    string[] sValue = outData.ToString().Replace("RRRR|", "").Split(';');
                    string[] sVal1 = sValue[0].Split('|');
                    iReturn = sVal1[0];
                    liValue.Add(sValue[1].ToString());

                    WriteLog(sysdate + "    疾病诊断目录下载成功" + sValue[1].ToString());
                    //return new object[] { 0, 0, "疾病诊断目录下载失败" + retMsg.ToString() };
                }
                else
                {
                    WriteLog(sysdate + "    疾病诊断目录下载失败" + retMsg.ToString());
                    iReturn = "-1";
                    //return new object[] { 0, 0, "疾病诊断目录下载失败" + retMsg.ToString() };
                }
                iCount++;
                inputData.Remove(0,inputData.Length);
            }

            if (liValue.Count > 0)
                return new object[] { 0, 1, "疾病诊断目录下载成功"};
            else
                return new object[] { 0, 0, "疾病诊断目录下载失败" + retMsg.ToString() };
        }
        #endregion

        #region 门特病种下载
        internal static object[] YBMTBZXZ(object[] objParam)
        {
            string sysdate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string iReturn = "0";
            int iCount = 1;
            string Ywlx = "CX03";
            StringBuilder inputData = new StringBuilder();
            StringBuilder outData = new StringBuilder(102400);
            StringBuilder retMsg = new StringBuilder(102400);
            WriteLog(sysdate + "  进入门特病种下载....");
            List<string> liValue = new List<string>();
            while (iReturn.Equals("0"))
            {
                inputData.Append("SSSS|"); //开始码
                inputData.Append(iCount + "|");
                inputData.Append("ZZZZ");
                WriteLog(sysdate + "  入参|" + inputData.ToString());
                int i = f_UserBargaingApply(Ywlx, inputData, outData, retMsg);
                if (i > 0)
                {
                    WriteLog(sysdate + "  出参|" + outData.ToString());
                    string[] sValue = outData.ToString().Replace("RRRR|", "").Split(';');
                    string[] sVal1 = sValue[0].Split('|');
                    iReturn = sVal1[0];
                    liValue.Add(sValue[1].ToString());

                    WriteLog(sysdate + "    疾病诊断目录下载成功" + sValue[1].ToString());
                    //return new object[] { 0, 0, "疾病诊断目录下载失败" + retMsg.ToString() };
                }
                else
                {
                    WriteLog(sysdate + "    疾病诊断目录下载失败" + retMsg.ToString());
                    iReturn = "-1";
                    //return new object[] { 0, 0, "疾病诊断目录下载失败" + retMsg.ToString() };
                }
                iCount++;
                inputData.Remove(0, inputData.Length);
            }

            if (liValue.Count > 0)
                return new object[] { 0, 1, "疾病诊断目录下载成功" };
            else
                return new object[] { 0, 0, "疾病诊断目录下载失败" + retMsg.ToString() };
        }
        #endregion

        #region  三大目录限价信息下载
        internal static object[] YBMRXJXZ(object[] objParam)
        {
            string sysdate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string iReturn = "0";
            int iCount = 1;
            string Ywlx = "CX10";
            StringBuilder inputData = new StringBuilder();
            StringBuilder outData = new StringBuilder(102400);
            StringBuilder retMsg = new StringBuilder(102400);
            WriteLog(sysdate + "  进入三大目录限价信息下载....");

            return null;
        }
        #endregion
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

            string sysdate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string hisUserID = CliUtils.fLoginUser;
            string UserID = string.Empty; //用户名
            string PassWD = string.Empty; //密码
            StringBuilder retMsg = new StringBuilder(1024);
            string strSql = string.Format(@"select b1ybno,b1ybpw from bz01h where b1empn='{0}' ", hisUserID);
            DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            if (ds.Tables[0].Rows.Count == 0)
                return new object[] { 0, 0, "该用户登记失败｜医保账户及密码错误" };
            else
            {
                UserID = ds.Tables[0].Rows[0]["b1ybno"].ToString();
                PassWD = ds.Tables[0].Rows[0]["b1ybpw"].ToString();
            }

            if (string.IsNullOrEmpty(UserID))
                return new object[] { 0, 0, "用户名不能为空" };
            if (string.IsNullOrEmpty(PassWD))
                return new object[] { 0, 0, "密码不能为空" };

            int i = f_UserBargaingInit(UserID, PassWD, retMsg);

            WriteLog(sysdate + "  入参|用户" + UserID + "密码" + PassWD + "");
            if (i > 0)
            {
                WriteLog(sysdate + "  用户" + UserID + " 进入医保初始化成功");
                return new object[] { 0, 1, "医保初始化成功" };
            }
            else
            {
                WriteLog(sysdate + "  医保初始化失败|" + i + "|" + retMsg.ToString());
                return new object[] { 0, 0, "医保初始化失败;" + retMsg.ToString() };
            }
        }
        #endregion

        #region 医保退出
        public static object[] YBEXIT(object[] objParam)
        {
            string sysdate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            StringBuilder retMsg = new StringBuilder(1024);
            int i = f_UserBargaingClose(retMsg);
            if (i > 0)
            {
                WriteLog(sysdate + "  医保退出成功");
                return new object[] { 0, 1, "医保退出成功" };
            }
            else
            {
                WriteLog(sysdate + "  医保退出错误|" + retMsg.ToString());
                return new object[] { 0, 0, "医保退出错误" };
            }
        }
        #endregion

        #region 门诊读卡
        public static object[] YBMZDK(object[] objParam)
        {
            string sysdate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string Ywlx = "CX01";
            StringBuilder inputData = new StringBuilder();
            StringBuilder outData = new StringBuilder(102400);
            StringBuilder retMsg = new StringBuilder(102400);
            WriteLog(sysdate + "  进入门诊读卡....");
            WriteLog(sysdate + "  入参|" + inputData.ToString());
            try
            {
                int i = f_UserBargaingApply(Ywlx, inputData, outData, retMsg);

                if (i > 0)
                {
                    List<string> liSQL = new List<string>();
                    WriteLog(sysdate + "  门诊读卡成功|出参|" + outData.ToString());

                    #region 出参
                    /*
                        1 社会保险号 
                        2 社会保障卡号
                        3 姓名
                        4 性别名称
                        5 身份证号码
                        6 出生日期 YYYYMMDD
                        7 地区编号 统筹区编码，详见附件
                        8 地区名称 
                        9 单位名称  格式“单位编码-单位名称”， 如“666666-测试单位”
                        10 单位类型 VARCHAR2(20) 格式“编码-名称”，如“100-正常企业”。见“字典说明”
                        11 参保身份 VARCHAR2(50)格式“编码-名称”，如“1101-企业一般人员”。见“字典说明”
                        12 行政职务级别 VARCHAR2(50) 格式“编码-名称”，如“121-厅局级正职”。见“字典说明”
                        13 参保日期 VARCHAR2(8) YYYYMMDD 医院实时结算平台接口规范
                        14 医疗待遇享受 类别 VARCHAR2(50) 格 式 “ 编 码 - 名 称 ” ， 如“1101-统账在职”。见“字典说明”（仅适用于新余市）
                        15 基本医疗保险 参保标志 VARCHAR2(1) 1-参保；0-未参保
                        16 大病医疗保险 参保标志 VARCHAR2(1) 1-参保；0-未参保
                        17公务员补充医 疗保险参保标志 VARCHAR2(1) 1-参保；0-未参保
                        18 生育保险参保标志 VARCHAR2(1) 1-参保；0-未参保
                        19 工伤保险参保标志 VARCHAR2(1) 1-参保；0-未参保
                        20 医保账户余额 NUMBER(8,2)
                        21 本年累计住院次数 NUMBER(4)
                        22 本年基本医疗基金支付累计 NUMBER(16,2)
                        23 本年大病医疗基金支付累计 NUMBER(16,2)
                        24 本年公务员补充医疗基金支付累计 NUMBER(16,2)
                        25 基本医疗年度剩余可报金额 NUMBER(16,2)
                        26 大病医疗年度剩余可报金额 NUMBER(16,2)
                        27 公务员补充医疗年度剩余可报金额 NUMBER(16,2)
                        28 门特年度剩余可报金额 NUMBER(16,2)
                        29 预留指标1 当地补充说明，自定义用途
                        30 预留指标2 当地补充说明，自定义用途
                    */
                    string[] strValue = outData.ToString().Split(';');
                    string[] str = strValue[0].Split('|');

                    if (str.Length < 10)
                        return new object[] { 0, 0, outData.ToString() + retMsg.ToString() };

                    outParams_dk OP = new outParams_dk();
                    OP.Grbh = str[1].Trim();     //社会保险号
                    OP.Kh = str[2].Trim();      //社会保障卡号
                    OP.Xm = str[3].Trim();      //姓名
                    OP.Xb = str[4].Trim();     //性别
                    OP.Sfhz = str[5].Trim();    //身份证号
                    OP.Csrq = str[6].Trim().Insert(6, "-").Insert(4, "-");    //出生日期
                    OP.Tcqh = str[7].Trim();    //地区编号
                    OP.Qxmc = str[8].Trim();    //地区名称
                    OP.Dwmc = str[9].Trim();    //单位名称
                    OP.Cbdwlx = str[10].Trim();    //单位类型

                    string ylrylb = str[11].Trim();   //医疗人员类别(参保身份)
                    int indexchar = ylrylb.IndexOf('-');
                    OP.Ylrylb = ylrylb.Substring(0, indexchar);

                    OP.Xzzwjb = str[12].Trim(); //行政职务级别
                    OP.Cbrq = str[13].Trim();    //参保日期
                    OP.Yldylb = str[14].Trim();     //医疗待遇享受类别
                    OP.Jbylbxcbbz = str[15].Trim(); //基本医疗保险参保标志
                    OP.Dbylbxcbbz = str[16].Trim(); //大病医疗保险参保标志
                    OP.Gwybcylbxcbbz = str[17].Trim();  //公务员补充医疗保险参保标志
                    OP.Sybxcbbz = str[18].Trim();   //生育保险参保标志
                    OP.Gsbxcbbz = str[19].Trim();   //工伤保险参保标志
                    OP.Zhye = str[20].Trim(); //(医保)账户余额
                    OP.Bnzycs = str[21].Trim();   //本年累计住院次数
                    OP.Bnjbyljjzflj = str[22].Trim(); //本年基本医疗基金支付累计
                    OP.Bndbyljjzflj = str[23].Trim(); //本年大病医疗基金支付累计
                    OP.Bngwybcyljjzflj = str[24].Trim();    //本年公务员补充医疗基金支付累计
                    OP.Jbylndsykbje = str[25].Trim();       //基本医疗年度剩余可报金额
                    OP.Dbylndsykbje = str[26].Trim();       //大病医疗年度剩余可报金额
                    OP.Gwybcylndsykbje = str[27].Trim();    //公务员补充医疗年度剩余可报金额
                    OP.Mtndsykbje = str[28].Trim();     //门特年度剩余可报金额



                    if (OP.Xb.Equals("男"))
                        OP.Xb = "1";
                    else
                        OP.Xb = "2";
                    OP.Nl = (DateTime.Now.Year - Convert.ToDateTime(OP.Csrq).Year).ToString();
                    string[] sV1 = { "1", "2", "3", "4" };
                    if (sV1.Contains(OP.Ylrylb.Substring(0, 1)))
                        OP.Jflx = "0202"; //职工医保
                    else
                        OP.Jflx = "0203"; //居民医保
                    OP.Mz = "01";
                    OP.Rycbzt = OP.Jbylbxcbbz;
                    OP.Nd = "";
                    OP.Zyzt = "0";
                    OP.Bnylflj = "0.00";
                    OP.Mtbz = "0";


                    string strSql = string.Format("delete from YBICKXX where grbh='{0}'", OP.Grbh);
                    liSQL.Add(strSql);

                    string yllb_1 = "11";
                    string mtbmc_1 = "";
                    string mtbbm_1 = "";
                    //是否存在慢病信息
                    if (strValue.Length > 1)
                    {
                        strSql = string.Format("delete from ybmxbdj where bxh='{0}'", OP.Grbh);
                        liSQL.Add(strSql);
                        string sMsg = string.Empty;
                        for (int index = 1; index < strValue.Length; index++)
                        {
                            if (strValue[index].Length > 0)
                            {
                                OP.Mtbz = "1";
                                string[] sV = strValue[index].Split('|');
                                OP.Mtbzbm = sV[0];  //门特病种编码
                                OP.Mtbzmc = sV[1];  //门特病种名称
                                strSql = string.Format(@"select * from ybbzmrdr where dm='{0}'", OP.Mtbzbm);
                                DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                                if (ds.Tables[0].Rows.Count > 0)
                                {
                                    OP.Bzlb = ds.Tables[0].Rows[0]["bzlb"].ToString();
                                    OP.Yllb = ds.Tables[0].Rows[0]["yllb"].ToString();
                                }

                                sMsg += OP.Bzlb + "\t";
                                sMsg += OP.Mtbzbm + "\t" + OP.Mtbzmc + "\r\n";

                                strSql = string.Format(@"insert into  ybmxbdj(BXH,KH,XM,MMBZBM,MMBZMC,YLLB,BZLB) values('{0}','{1}','{2}','{3}','{4}','{5}','{6}') ",
                                                        OP.Grbh, OP.Kh, OP.Xm, OP.Mtbzbm, OP.Mtbzmc, OP.Yllb, OP.Bzlb);
                                liSQL.Add(strSql);

                                if (index == 1)
                                {
                                    yllb_1 = OP.Yllb;
                                    mtbbm_1 = OP.Mtbzbm;
                                    mtbmc_1 = OP.Mtbzmc;
                                }
                            }
                        }

                        OP.Mtmsg = sMsg;
                        //MessageBox.Show(sMsg);
                    }
                    #endregion

                    /*
                     * 个人编号|单位编号|身份证号|姓名|性别|
                     * 民族|出生日期|社会保障卡卡号|医疗待遇类别|人员参保状态|
                     * 异地人员标志|统筹区号|年度|在院状态|帐户余额|
                     * 本年医疗费累计|本年帐户支出累计|本年统筹支出累计|本年救助金支出累计|本年公务员补助基金累计|
                     * 本年城镇居民门诊统筹支付累计|进入统筹费用累计|进入救助金费用累计|起付标准累计|本年住院次数|
                     * 单位名称|年龄|参保单位类型|经办机构编码|缴费类型|
                     * 医保门慢、特资质|医保门慢、特病种说明|医疗类别代码|慢、特病编码|慢、特病名称
                     */
                    string outParams = OP.Grbh + "|" + OP.Dwbh + "|" + OP.Sfhz + "|" + OP.Xm + "|" + OP.Xb + "|" +
                        OP.Mz + "|" + OP.Csrq + "|" + OP.Kh + "|" + OP.Ylrylb + "|" + OP.Rycbzt + "|" +
                        OP.Ydrybz + "|" + OP.Tcqh + "|" + OP.Nd + "|" + OP.Zyzt + "|" + OP.Zhye + "|" +
                        OP.Bnylflj + "|" + OP.Bnzhzclj + "|" + OP.Bntczclj + "|" + OP.Bnjzjzclj + "|" + OP.Bngwybzjjlj + "|" +
                        OP.Bnczjmmztczflj + "|" + OP.Jrtcfylj + "|" + OP.Jrjzjfylj + "|" + OP.Qfbzlj + "|" + OP.Bnzycs + "|" +
                        OP.Dwmc + "|" + OP.Nl + "|" + OP.Cbdwlx + "|" + OP.Jbjgbm + "|" + OP.Jflx + "|" + OP.Mtbz + "|" + OP.Mtmsg + "|" + yllb_1 + "|" + mtbbm_1 + "|" + mtbmc_1;
                    strSql = string.Format(@"insert into YBICKXX(
                                            GRBH,KH,XM,XB,GMSFHM,CSRQ,DQBH,QXMC,DWMC,DWLX,
                                            RYLB,GRSF,CBRQ,YLRYLB,JBYLCBBZ,DBYLCBBZ,GWYYLCBBZ,SYCBBZ,GSCBBZ,GRZHYE,
                                            ZYCS,BNJBYLJJZFLJ,BNDBYLJJZFLJ,BNGWYJJZFLJ,JBYLSYKBXJE,DBYLSYKBXJE,GWYYLSYKBXJE,MTSYKBJE,SJNL)
                                            VALUES(
                                            '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}',
                                            '{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}',
                                            '{20}','{21}','{22}','{23}','{24}','{25}','{26}','{27}','{28}')",
                                            OP.Grbh, OP.Kh, OP.Xm, OP.Xb, OP.Sfhz, OP.Csrq, OP.Tcqh, OP.Qxmc, OP.Dwmc, OP.Cbdwlx,
                                            OP.Ylrylb, OP.Xzzwjb, OP.Cbrq, OP.Yldylb, OP.Jbylbxcbbz, OP.Dbylbxcbbz, OP.Gwybcylbxcbbz, OP.Sybxcbbz, OP.Gsbxcbbz, OP.Zhye,
                                            OP.Bnzycs, OP.Bnjbyljjzflj, OP.Bndbyljjzflj, OP.Bngwybcyljjzflj, OP.Jbylndsykbje, OP.Dbylndsykbje, OP.Gwybcylndsykbje, OP.Mtndsykbje, OP.Nl);
                    liSQL.Add(strSql);
                    object[] obj = liSQL.ToArray();
                    obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);
                    if (obj[1].ToString().Equals("1"))
                    {
                        WriteLog(sysdate + "  门诊读卡成功|" + outParams);
                        return new object[] { 0, 1, outParams };
                    }
                    else
                    {
                        WriteLog(sysdate + "  门诊读卡成功|保存本地数据失败|" + obj[2].ToString());
                        return new object[] { 0, 2, "门诊读卡成功|" + obj[2].ToString() };
                    }
                }
                else
                {
                    WriteLog(sysdate + "  门诊读卡失败|" + retMsg.ToString());
                    return new object[] { 0, 0, "读卡失败|" + retMsg.ToString() };
                }
            }
            catch (Exception ex)
            {
                WriteLog(sysdate + " 门诊读卡异常|" + ex.Message);
                return new object[] { 0, 2, "门诊读卡异常|" + ex.Message };
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
