using FastReport;
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
    public class yb_interface_fz_zr
    {
        public static string dqbh = "361025"; //地区编号()

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
                WriteLog(sysdate+" 门诊读卡异常|"+ex.Message);
                return new object[] { 0, 2, "门诊读卡异常|" + ex.Message };
            }
        }
        #endregion

        #region 住院读卡
        public static object[] YBZYDK(object[] objParam)
        {
            string sysdate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string Ywlx = "CX01";
            StringBuilder inputData = new StringBuilder();
            StringBuilder outData = new StringBuilder(102400);
            StringBuilder retMsg = new StringBuilder(102400);
            WriteLog(sysdate + "  进入住院读卡....");
            WriteLog(sysdate + "  入参|" + inputData.ToString());
            try
            {
                int i = f_UserBargaingApply(Ywlx, inputData, outData, retMsg);

                if (i > 0)
                {
                    List<string> liSQL = new List<string>();
                    WriteLog(sysdate + "  住院读卡成功|出参|" + outData.ToString());
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
                    OP.Ydrybz = "0";

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
                    OP.Zyzt = "";
                    OP.Bnylflj = "0.00";
                    OP.Mtbz = "0";

                    
                    string strSql = string.Format("delete from YBICKXX where grbh='{0}'", OP.Grbh);
                    liSQL.Add(strSql);

                    string yllb_1 = "11";
                    string mtbbm_1 = "";
                    string mtbmc_1 = "";

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
                                sMsg += OP.Mtbzbm+"\t"+OP.Mtbzmc + "\r\n";

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
                     * 医保门慢、特资质|医保门慢、特病种说明|医疗类别代码|慢病代码|慢病名称
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
                        WriteLog(sysdate + "  住院读卡成功|" + outParams);
                        return new object[] { 0, 1, outParams };
                    }
                    else
                    {
                        WriteLog(sysdate + "  住院读卡成功|保存本地数据失败|" + obj[2].ToString());
                        return new object[] { 0, 2, "住院读卡成功|" + obj[2].ToString() };
                    }
                }
                else
                {
                    WriteLog(sysdate + "  住院读卡失败|" + retMsg.ToString());
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

        #region 门诊登记(挂号)
        public static object[] YBMZDJ(object[] objParam)
        {
            string sysdate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string Ywlx = "DJ01";

            string jzlsh = objParam[0].ToString(); //就诊流水号
            string yllb = objParam[1].ToString(); //医疗类别代码
            string bzbm = objParam[2].ToString(); //病种编码
            string bzmc = objParam[3].ToString(); //病种名称
            string[] kxx = objParam[4].ToString().Split('|'); //读卡信息
            string ghdjsj = objParam[5].ToString(); //挂号日期时间(yyyy-MM-dd HH:mm:ss)
            string cfysdm = objParam[6].ToString(); //处方医生代码
            string cfysxm = objParam[7].ToString(); //处方医生姓名
            string zszbm = ""; //准生证编号
            string sylb = "";      //生育类别
            string jhsylb = "";    //计划生育类别

            string dgysdm = ""; //定岗医生代码
            string dgysxm = ""; //定岗医生姓名
            string jbr = CliUtils.fUserName;


            #region 获取读卡信息
            if (kxx.Length < 2)
                return new object[] { 0, 0, "无读卡信息反馈" };
            string grbh = kxx[0].ToString(); //个人编号
            //string dwbh = kxx[1].ToString(); //单位编号
            string sfzh = kxx[2].ToString(); //身份证号
            string xm = kxx[3].ToString(); //姓名
            //string xb = kxx[4].ToString(); //性别
            //string mz = kxx[5].ToString(); //民族
            //string csrq = kxx[6].ToString(); //出生日期
            string kh = kxx[7].ToString(); //卡号
            //string yldylb = kxx[8].ToString(); //医疗待遇类别
            //string rycbzt = kxx[9].ToString(); //人员参保状态
            //string ydrybz = kxx[10].ToString(); //异地人员标志
            string tcqh = kxx[11].ToString(); //统筹区号
            //string nd = kxx[12].ToString(); //年度
            //string zyzt = kxx[13].ToString(); //在院状态
            //string zhye = kxx[14].ToString(); //帐户余额
            //string bnylflj = kxx[15].ToString(); //本年医疗费累计
            //string bnzhzclj = kxx[16].ToString(); //本年帐户支出累计
            //string bntczclj = kxx[17].ToString(); //本年统筹支出累计
            //string bnjzjzclj = kxx[18].ToString(); //本年救助金支出累计
            //string bngwybzjjlj = kxx[19].ToString(); //本年公务员补助基金累计
            //string bnczjmmztczflj = kxx[20].ToString(); //本年城镇居民门诊统筹支付累计
            //string jrtcfylj = kxx[21].ToString(); //进入统筹费用累计
            //string jrjzjfylj = kxx[22].ToString(); //进入救助金费用累计
            //string qfbzlj = kxx[23].ToString(); //起付标准累计
            //string bnzycs = kxx[24].ToString(); //本年住院次数
            //string dwmc = kxx[25].ToString(); //单位名称
            //string nl = kxx[26].ToString(); //年龄
            //string cbdwlx = kxx[27].ToString(); //参保单位类型
            //string jbjgbm = kxx[28].ToString(); //经办机构编码
            #endregion

            string ksbh = "";   //科室编号
            string ksmc = "";   //科室名称
            string ghfy = "";   //挂号费
            string ghsj = "";   //挂号时间
            string ghrq = "";  //挂号日期
            string hzname = "";   //患者姓名

            if (string.IsNullOrEmpty(grbh))
                return new object[] { 0, 0, "保险号不能为空" };
            if (string.IsNullOrEmpty(xm))
                return new object[] { 0, 0, "患者姓名不能为空" };
            if (string.IsNullOrEmpty(kh))
                return new object[] { 0, 0, "卡号不能为空" };
            if (string.IsNullOrEmpty(yllb))
                return new object[] { 0, 0, "医疗类别不能为空" };
            if (string.IsNullOrEmpty(ghdjsj))
                return new object[] { 0, 0, "挂号日期时间不能为空" };
            if (string.IsNullOrEmpty(jzlsh))
                return new object[] { 0, 0, "就诊流水号不能为空" };


            if (yllb == "14" || yllb == "16")
            {
                if (string.IsNullOrEmpty(bzbm) || string.IsNullOrEmpty(bzmc))
                    return new object[] { 0,0,"门慢一类、二类时，病种必须输入"};
            }

            if (yllb.Equals("51"))
            {
                if (string.IsNullOrEmpty(sylb) && string.IsNullOrEmpty(jhsylb))
                    return new object[] { 0, 0, "医疗类别为“51-生育门诊”时，生育类别、计划生育类别至少输入一项" };
            }

            WriteLog(sysdate + "  " + jzlsh + " 进入门诊挂号...");

            if (string.IsNullOrEmpty(zszbm))
                zszbm = "NULL";
            try
            {
                #region 判断挂号日期时间有效性
                try
                {
                    ghrq = Convert.ToDateTime(ghdjsj).ToString("yyyyMMdd");
                    ghsj = Convert.ToDateTime(ghdjsj).ToString("HHmm");
                }
                catch
                {
                    return new object[] { 0, 0, "挂号日期时间格式不正确" };
                }
                #endregion

                #region 判断是否在HIS中挂号
                string strSql = string.Empty;
                strSql = string.Format(@"select a.m1name as name,a.m1ksno, (select top 1 b2ejnm from bz02d  where b2ksno=a.m1ksno) as ksmc,a.m1amnt from mz01t  a where a.m1ghno = '{0}'
                union all select a.m1name as name,a.m1ksno, (select top 1 b2ejnm from bz02d  where b2ksno=a.m1ksno) as ksmc,a.m1amnt from mz01h  a where a.m1ghno = '{0}'", jzlsh);
                DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                if (ds.Tables[0].Rows.Count == 0)
                    return new object[] { 0, 0, "该患者未挂号" };
                else
                {
                    ksbh = ds.Tables[0].Rows[0]["m1ksno"].ToString();
                    hzname = ds.Tables[0].Rows[0]["name"].ToString();
                    ksmc = ds.Tables[0].Rows[0]["ksmc"].ToString();
                    ghfy = ds.Tables[0].Rows[0]["m1amnt"].ToString();
                }
                #endregion

                #region 判断是否进门诊挂号登记
                strSql = string.Format(@"select jzlsh from ybmzzydjdr where jzlsh='{0}' and cxbz=1 ", jzlsh);
                
                ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                if (ds.Tables[0].Rows.Count > 0)
                    return new object[] { 0, 0, "该患者已经进行过医保门诊登记" };
                ds.Dispose();
                #endregion

                #region 患者姓名与医保卡姓名是否一致
                if (!xm.Equals(hzname))
                    return new object[] { 0, 0, "医保卡与患者姓名不一致" };
                #endregion

                #region 获取定岗医生信息
                //strSql = string.Format(@"select * from ybdgyszd where ysbm='{0}'", cfysdm);
                //ds.Tables.Clear();
                //ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                //if (ds.Tables[0].Rows.Count == 0)
                //{
                //    //给定默认值
                //    dgysdm = "NULL";
                //    dgysxm = "NULL";
                //}
                //else
                //{
                //    dgysdm = ds.Tables[0].Rows[0]["dgysbm"].ToString();
                //    dgysxm = ds.Tables[0].Rows[0]["ysxm"].ToString();
                //}
                #endregion

                /* 入参数据：
                1 社会保险号 VARCHAR2(20) 否
                2 社会保障卡号 VARCHAR2(20) 否
                3 姓名 VARCHAR2(20) 否
                4 医疗类别 VARCHAR2(3) 否 代码（详见字典说明）
                5 科室代码 VARCHAR2(20) 代码（详见字典说明）
                6 科室名称 VARCHAR2(20)
                7 挂号费 NUMBER(8,2)
                8 挂号日期 VARCHAR2(8) 否 YYYYMMDD
                9 挂号时间 VARCHAR2(6) 否 HH24MISS
                10 定点机构业务流水号 VARCHAR2(20) 否 门诊号\住院号
                11 准生证编号 VARCHAR2(30) 生育保险专用
                12 地区编号 VARCHAR2(6) 否 统筹区编码，同CX01返回第7项
                */
                StringBuilder inputData = new StringBuilder();
                inputData.Append("SSSS|"); //开始码
                inputData.Append(grbh + "|");    //保险号
                inputData.Append(kh + "|");     //卡号
                inputData.Append(xm + "|");     //姓名
                inputData.Append(yllb + "|");   //医疗类别
                inputData.Append("|");   //科室代码
                inputData.Append("|");   //科室名称
                inputData.Append("|");   //挂号费
                inputData.Append(ghrq + "|");  //挂号日期
                inputData.Append(ghsj + "|");   //挂号时间
                inputData.Append(jzlsh + "|");  //医院门诊流水号
                inputData.Append(zszbm + "|");  //准生证号
                inputData.Append(tcqh + "|");   //地区编号
                inputData.Append("ZZZZ");
                StringBuilder outData = new StringBuilder(102400);
                StringBuilder retMsg = new StringBuilder(10240);
                WriteLog(sysdate + "  入参|" + inputData.ToString());
                int i = f_UserBargaingApply(Ywlx, inputData, outData, retMsg);
                if (i > 0)
                {
                    WriteLog(sysdate + "  出参|" + outData.ToString());
                    WriteLog(sysdate + "  出参1|" + outData.ToString().Replace("RRRR|",""));
                    #region 出参
                    /*出参:
                        1 社会保险号 VARCHAR2(20) 医院实时结算平台接口规范
                        2 社会保障卡号 VARCHAR2(20)
                        3 姓名 VARCHAR2(20)
                        4 地区编号 VARCHAR2(6) 统筹区编码
                        5 地区名称 VARCHAR2(50)
                        6 医保账户余额 NUMBER(8,2)
                        7 中心业务流水号 VARCHAR2(20) 门诊号
                        8 医疗类别 VARCHAR2(50) 名称
                        9 本次住院次数 NUMBER(3) 住院业务有效
                        10 出生日期 VARCHAR2(8)
                        11 年龄 VARCHAR2(4)
                        12 参保日期 VARCHAR2(8)
                        13 参保身份 VARCHAR2(10) 格式“编码-名称”，如“1101-企业一般人员”。见“字典说明”
                        14 单位名称 VARCHAR2(100)
                        15 性别 VARCHAR2(10) 名称
                        16 医疗待遇享受类别 VARCHAR2(50) 名称（仅适用于新余市）
                         * 
                            返回的附加消息包：参保人员病种审批信息，多记录往后续加消息包
                         * 
                        1 门特病种编码 VARCHAR2(20)
                        2 门特病种名称 VARCHAR2(100)
                */
                    List<string> liSQL = new List<string>();
                    string strValue = "";
                    string[] str = outData.ToString().Replace("RRRR|","").Split(';');
                    string[] str2 = str[0].Split('|');

                    outParams_dj OPDJ = new outParams_dj();

                    OPDJ.Grbh = str2[0].Trim();       //保险号
                    OPDJ.Kh = str2[1].Trim();        //卡号
                    OPDJ.Xm = str2[2].Trim();        //姓名
                    OPDJ.Tcqh = str2[3].Trim();      //地区编号
                    OPDJ.Qxmc = str2[4].Trim();       //地区名称
                    OPDJ.Zhye = str2[5].Trim();    //账户余额
                    OPDJ.Zxywlsh = str2[6].Trim();    //中心业务流水号
                    OPDJ.Yllb = str2[7].Trim();     //医疗类别
                    OPDJ.Zycs = str2[8].Trim();    //本次看病次数
                    OPDJ.Csrq = str2[9].Trim();       //出生日期
                    OPDJ.Nl = str2[10].Trim();       //实际年龄
                    OPDJ.Cbrq = str2[11].Trim();       //参保日期
                    OPDJ.Cbsf = str2[12].Trim();       //参保身份
                    OPDJ.Dwmc = str2[13].Trim();       //单位名称
                    OPDJ.Xb = str2[14].Trim();        //性别
                    OPDJ.Yldyxslb = str2[15].Trim();      //医疗待遇享受类别(医疗人员类别)
                    OPDJ.Ghfy = ghfy;
                    #endregion
                    if (str.Length > 1)
                    {
                        strSql = string.Format(@"delete from ybmxbdj where bxh='{0}'", OPDJ.Grbh);
                        liSQL.Add(strSql);

                        for (int j = 1; j < str.Length; j++)
                        {
                            str2 = str[j].Split('|');
                            OPDJ.Mtbzbm = str2[0].Trim();
                            OPDJ.Mtbzmc = str2[1].Trim();

                            strSql = string.Format(@"insert into ybmxbdj (jzlsh,bxh,xm,kh,mmbzbm,mmbzmc) 
                                                values(
                                                '{0}','{1}','{2}','{3}','{4}','{5}')",
                                                    jzlsh, OPDJ.Grbh, OPDJ.Xm, OPDJ.Kh, OPDJ.Mtbzbm, OPDJ.Mtbzmc);
                            liSQL.Add(strSql);
                        }
                    }

                    strSql = string.Format(@"insert into ybmzzydjdr(
                                        jzlsh,jylsh,yllb,bzbm,bzmc,ysdm,ysxm,dgysdm,dgysxm,ghdjsj,
                                        zszbm,ksbh,ksmc,ghf,grbh,kh,xm,xb,nl,sfzh,
                                        csrq,zhye,yldylb,dwmc,zycs,tcqh,qxmc,jbr,jzbz,sysdate,
                                        ybjzlsh,sylb,jhsylb) values(
                                       '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}',
                                       '{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}',
                                       '{20}','{21}','{22}','{23}','{24}','{25}','{26}','{27}','{28}','{29}',
                                       '{30}','{31}','{32}')",
                                        jzlsh, OPDJ.Zxywlsh, yllb, bzbm, bzmc, cfysdm, cfysxm, dgysdm, dgysxm, ghdjsj,
                                        zszbm, ksbh, ksmc, OPDJ.Ghfy, OPDJ.Grbh, OPDJ.Kh, OPDJ.Xm, OPDJ.Xb, OPDJ.Nl, sfzh,
                                        OPDJ.Csrq, OPDJ.Zhye, OPDJ.Cbsf, OPDJ.Dwmc, OPDJ.Zycs, OPDJ.Tcqh, OPDJ.Qxmc, jbr, "m", sysdate,
                                        OPDJ.Zxywlsh, sylb, jhsylb);
                    liSQL.Add(strSql);


                    object[] obj = liSQL.ToArray();
                    obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);
                    if (obj[1].ToString() == "1")
                    {
                        WriteLog(sysdate + "  " + jzlsh + " 进入门诊挂号成功|" + outData.ToString());
                        return new object[] { 0, 1, "门诊挂号成功" };
                        //门诊就诊疾病诊断登记
                        //object[] objParam1 = { jzlsh,"1" };
                        //object[] objReturn = YBZDDJ(objParam1);
                        //if (objReturn[1].ToString().Equals("1"))
                        //{
                        //    WriteLog(sysdate + "  " + jzlsh + " 进入门诊挂号成功|" + outData.ToString());
                        //    return new object[] { 0, 1, "门诊挂号成功" };
                        //}
                        //else
                        //{
                        //    //医保登记撤销
                        //    YBMZDJCX(objParam1);
                        //    return new object[] { 0, 0, "门诊挂号失败" };
                        //}
                    }
                    else
                    {
                        WriteLog(sysdate + "  " + jzlsh + " 进入门诊挂号成功|操作本地数据失败|" + obj[2].ToString());
                        //门诊登记（挂号）撤销
                        object[] objParam2 = new object[] { jzlsh, OPDJ.Grbh, OPDJ.Xm, OPDJ.Kh, OPDJ.Tcqh, OPDJ.Zxywlsh };
                        NYBMZDJCX(objParam2);
                        return new object[] { 0, 0, "门诊挂号失败|" + obj[2].ToString() };
                    }
                }
                else
                {
                    WriteLog(sysdate + "  " + jzlsh + " 进入门诊挂号失败|" + retMsg.ToString());
                    return new object[] { 0, 0, "门诊挂号失败|" + retMsg.ToString() };
                }
            }
            catch (Exception ex)
            {
                WriteLog(sysdate + "  门诊登记(挂号)异常|" + ex.Message);
                return new object[] { 0, 0, "门诊登记(挂号)失败|" + ex.Message };
            }
        }
        #endregion

        #region 门诊登记(挂号)自动
        public static object[] YBMZDJ_AUTO(object[] objParam)
        {
            string sysdate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string Ywlx = "DJ01";
            string jzlsh = objParam[0].ToString(); //就诊流水号
            string[] kxx = objParam[1].ToString().Split('|'); //读卡信息
            string ghdjsj = objParam[2].ToString(); //挂号日期时间(yyyy-MM-dd HH:mm:ss)
            string cfysdm = objParam[3].ToString(); //处方医生代码
            string cfysxm = objParam[4].ToString(); //处方医生姓名
            string zszbm = "NULL"; //准生证编号
            string sylb = "NULL";      //生育类别
            string jhsylb = "NULL";    //计划生育类别
            string dgysdm = ""; //定岗医生代码
            string dgysxm = ""; //定岗医生姓名
            string jbr = CliUtils.fUserName;


            #region 获取读卡信息
            if (kxx.Length < 10)
                return new object[] { 0, 0, "无读卡信息反馈" };
            string grbh = kxx[0].ToString(); //个人编号
            string sfzh = kxx[2].ToString(); //身份证号
            string xm = kxx[3].ToString(); //姓名
            string kh = kxx[7].ToString(); //卡号
            string tcqh = kxx[11].ToString(); //统筹区号
            string jflx = kxx[29].ToString(); //缴费类别
            string yllb = kxx[32].ToString(); //医疗类别
            string bzbm = kxx[33].ToString(); //病种代码 
            string bzmc = kxx[34].ToString(); //病种名称  
            #endregion

            string ksbh = "";   //科室编号
            string ksmc = "";   //科室名称
            string ghfy = "";   //挂号费
            string ghsj = "";   //挂号时间
            string ghrq = "";  //挂号日期
            string hzname = "";   //患者姓名

            if (string.IsNullOrEmpty(grbh))
                return new object[] { 0, 0, "保险号不能为空" };
            if (string.IsNullOrEmpty(xm))
                return new object[] { 0, 0, "患者姓名不能为空" };
            if (string.IsNullOrEmpty(kh))
                return new object[] { 0, 0, "卡号不能为空" };
            if (string.IsNullOrEmpty(ghdjsj))
                return new object[] { 0, 0, "挂号日期时间不能为空" };
            if (string.IsNullOrEmpty(jzlsh))
                return new object[] { 0, 0, "就诊流水号不能为空" };


            if (yllb == "14" || yllb == "16")
            {
                if (string.IsNullOrEmpty(bzbm) || string.IsNullOrEmpty(bzmc))
                    return new object[] { 0,0,"门慢一类、二类时，病种必须输入"};
            }
          
            WriteLog(sysdate + "  " + jzlsh + " 进入门诊挂号（自动）...");

            if (string.IsNullOrEmpty(zszbm))
                zszbm = "NULL";
            try
            {

                #region 判断挂号日期时间有效性
                try
                {
                    ghrq = Convert.ToDateTime(ghdjsj).ToString("yyyyMMdd");
                    ghsj = Convert.ToDateTime(ghdjsj).ToString("HHmm");
                }
                catch
                {
                    return new object[] { 0, 0, "挂号日期时间格式不正确" };
                }
                #endregion

                #region 判断是否在HIS中挂号
                string strSql = string.Empty;
                strSql = string.Format(@"select a.m1name as name,a.m1ksno, (select top 1 b2ejnm from bz02d  where b2ksno=a.m1ksno) as ksmc,a.m1amnt from mz01t  a where a.m1ghno = '{0}'
                union all select a.m1name as name,a.m1ksno, (select top 1 b2ejnm from bz02d  where b2ksno=a.m1ksno) as ksmc,a.m1amnt from mz01h  a where a.m1ghno = '{0}'", jzlsh);
                DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                WriteLog(strSql);
                if (ds.Tables[0].Rows.Count == 0)
                    return new object[] { 0, 0, "该患者未挂号" };
                else
                {
                    ksbh = ds.Tables[0].Rows[0]["m1ksno"].ToString();
                    hzname = ds.Tables[0].Rows[0]["name"].ToString();
                    ksmc = ds.Tables[0].Rows[0]["ksmc"].ToString();
                    ghfy = ds.Tables[0].Rows[0]["m1amnt"].ToString();
                }
                #endregion

                #region 判断是否进门诊挂号登记
                strSql = string.Format(@"select jzlsh from ybmzzydjdr where jzlsh='{0}' and cxbz=1 ", jzlsh);
                
                ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                if (ds.Tables[0].Rows.Count > 0)
                    return new object[] { 0, 0, "该患者已经进行过医保门诊登记" };
                #endregion
                ds.Dispose();

                #region 患者姓名与医保卡姓名是否一致
                if (!xm.Equals(hzname))
                    return new object[] { 0, 0, "医保卡与患者姓名不一致" };
                #endregion

                /* 入参数据：
                1 社会保险号 VARCHAR2(20) 否
                2 社会保障卡号 VARCHAR2(20) 否
                3 姓名 VARCHAR2(20) 否
                4 医疗类别 VARCHAR2(3) 否 代码（详见字典说明）
                5 科室代码 VARCHAR2(20) 代码（详见字典说明）
                6 科室名称 VARCHAR2(20)
                7 挂号费 NUMBER(8,2)
                8 挂号日期 VARCHAR2(8) 否 YYYYMMDD
                9 挂号时间 VARCHAR2(6) 否 HH24MISS
                10 定点机构业务流水号 VARCHAR2(20) 否 门诊号\住院号
                11 准生证编号 VARCHAR2(30) 生育保险专用
                12 地区编号 VARCHAR2(6) 否 统筹区编码，同CX01返回第7项
                */
                StringBuilder inputData = new StringBuilder();
                inputData.Append("SSSS|"); //开始码
                inputData.Append(grbh + "|");    //保险号
                inputData.Append(kh + "|");     //卡号
                inputData.Append(xm + "|");     //姓名
                inputData.Append(yllb + "|");   //医疗类别
                inputData.Append("|");   //科室代码
                inputData.Append("|");   //科室名称
                inputData.Append("|");   //挂号费
                inputData.Append(ghrq + "|");  //挂号日期
                inputData.Append(ghsj + "|");   //挂号时间
                inputData.Append(jzlsh + "|");  //医院门诊流水号
                inputData.Append(zszbm + "|");  //准生证号
                inputData.Append(tcqh + "|");   //地区编号
                inputData.Append("ZZZZ");
                StringBuilder outData = new StringBuilder(102400);
                StringBuilder retMsg = new StringBuilder(10240);
                WriteLog(sysdate + "  入参|" + inputData.ToString());
                int i = f_UserBargaingApply(Ywlx, inputData, outData, retMsg);
                if (i > 0)
                {
                    WriteLog(sysdate + "  出参|" + outData.ToString());
                    WriteLog(sysdate + "  出参1|" + outData.ToString().Replace("RRRR|",""));
                    #region 出参
                    /*出参:
                        1 社会保险号 VARCHAR2(20) 医院实时结算平台接口规范
                        2 社会保障卡号 VARCHAR2(20)
                        3 姓名 VARCHAR2(20)
                        4 地区编号 VARCHAR2(6) 统筹区编码
                        5 地区名称 VARCHAR2(50)
                        6 医保账户余额 NUMBER(8,2)
                        7 中心业务流水号 VARCHAR2(20) 门诊号
                        8 医疗类别 VARCHAR2(50) 名称
                        9 本次住院次数 NUMBER(3) 住院业务有效
                        10 出生日期 VARCHAR2(8)
                        11 年龄 VARCHAR2(4)
                        12 参保日期 VARCHAR2(8)
                        13 参保身份 VARCHAR2(10) 格式“编码-名称”，如“1101-企业一般人员”。见“字典说明”
                        14 单位名称 VARCHAR2(100)
                        15 性别 VARCHAR2(10) 名称
                        16 医疗待遇享受类别 VARCHAR2(50) 名称（仅适用于新余市）
                         * 
                            返回的附加消息包：参保人员病种审批信息，多记录往后续加消息包
                         * 
                        1 门特病种编码 VARCHAR2(20)
                        2 门特病种名称 VARCHAR2(100)
                */
                    List<string> liSQL = new List<string>();
                    string[] str = outData.ToString().Replace("RRRR|","").Split(';');
                    string[] str2 = str[0].Split('|');

                    outParams_dj OPDJ = new outParams_dj();

                    OPDJ.Grbh = str2[0].Trim();       //保险号
                    OPDJ.Kh = str2[1].Trim();        //卡号
                    OPDJ.Xm = str2[2].Trim();        //姓名
                    OPDJ.Tcqh = str2[3].Trim();      //地区编号
                    OPDJ.Qxmc = str2[4].Trim();       //地区名称
                    OPDJ.Zhye = str2[5].Trim();    //账户余额
                    OPDJ.Zxywlsh = str2[6].Trim();    //中心业务流水号
                    OPDJ.Yllb = str2[7].Trim();     //医疗类别
                    OPDJ.Zycs = str2[8].Trim();    //本次看病次数
                    OPDJ.Csrq = str2[9].Trim();       //出生日期
                    OPDJ.Nl = str2[10].Trim();       //实际年龄
                    OPDJ.Cbrq = str2[11].Trim();       //参保日期
                    OPDJ.Cbsf = str2[12].Trim();       //参保身份
                    OPDJ.Dwmc = str2[13].Trim();       //单位名称
                    OPDJ.Xb = str2[14].Trim();        //性别
                    OPDJ.Yldyxslb = str2[15].Trim();      //医疗待遇享受类别(医疗人员类别)
                    OPDJ.Ghfy = ghfy;
                    #endregion
                    if (str.Length > 1)
                    {
                        strSql = string.Format(@"delete from ybmxbdj where bxh='{0}'", OPDJ.Grbh);
                        liSQL.Add(strSql);

                        for (int j = 1; j < str.Length; j++)
                        {
                            str2 = str[j].Split('|');
                            OPDJ.Mtbzbm = str2[0].Trim();
                            OPDJ.Mtbzmc = str2[1].Trim();

                            strSql = string.Format(@"insert into ybmxbdj (jzlsh,bxh,xm,kh,mmbzbm,mmbzmc) 
                                                values(
                                                '{0}','{1}','{2}','{3}','{4}','{5}')",
                                                    jzlsh, OPDJ.Grbh, OPDJ.Xm, OPDJ.Kh, OPDJ.Mtbzbm, OPDJ.Mtbzmc);
                            liSQL.Add(strSql);
                        }
                    }

                    strSql = string.Format(@"insert into ybmzzydjdr(
                                        jzlsh,jylsh,yllb,bzbm,bzmc,ysdm,ysxm,dgysdm,dgysxm,ghdjsj,
                                        zszbm,ksbh,ksmc,ghf,grbh,kh,xm,xb,nl,sfzh,
                                        csrq,zhye,yldylb,dwmc,zycs,tcqh,qxmc,jbr,jzbz,sysdate,
                                        ybjzlsh,sylb,jhsylb) values(
                                       '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}',
                                       '{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}',
                                       '{20}','{21}','{22}','{23}','{24}','{25}','{26}','{27}','{28}','{29}',
                                       '{30}','{31}','{32}')",
                                        jzlsh, OPDJ.Zxywlsh, yllb, bzbm, bzmc, cfysdm, cfysxm, dgysdm, dgysxm, ghdjsj,
                                        zszbm, ksbh, ksmc, OPDJ.Ghfy, OPDJ.Grbh, OPDJ.Kh, OPDJ.Xm, OPDJ.Xb, OPDJ.Nl, sfzh,
                                        OPDJ.Csrq, OPDJ.Zhye, OPDJ.Cbsf, OPDJ.Dwmc, OPDJ.Zycs, OPDJ.Tcqh, OPDJ.Qxmc, jbr, "m", sysdate,
                                        OPDJ.Zxywlsh, sylb, jhsylb);
                    liSQL.Add(strSql);

                    strSql = string.Format(@"update mz01h set m1kind='{1}' where m1ghno='{0}'", jzlsh, jflx);
                    liSQL.Add(strSql);


                    object[] obj = liSQL.ToArray();
                    obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);
                    if (obj[1].ToString() == "1")
                    {
                        WriteLog(sysdate + "  " + jzlsh + " 进入门诊挂号成功|" + outData.ToString());
                        return new object[] { 0, 1, "门诊挂号成功" };
                    }
                    else
                    {
                        WriteLog(sysdate + "  " + jzlsh + " 进入门诊挂号成功|操作本地数据失败|" + obj[2].ToString());
                        //门诊登记（挂号）撤销
                        object[] objParam2 = new object[] { jzlsh, OPDJ.Grbh, OPDJ.Xm, OPDJ.Kh, OPDJ.Tcqh, OPDJ.Zxywlsh };
                        NYBMZDJCX(objParam2);
                        return new object[] { 0, 0, "门诊挂号失败|" + obj[2].ToString() };
                    }
                }
                else
                {
                    WriteLog(sysdate + "  " + jzlsh + " 进入门诊挂号失败|" + retMsg.ToString());
                    return new object[] { 0, 0, "门诊挂号失败|" + retMsg.ToString() };
                }
            }
            catch (Exception ex)
            {
                WriteLog(sysdate + "  门诊登记(挂号)异常|" + ex.Message);
                return new object[] { 0, 0, "门诊登记(挂号)失败|" + ex.Message };
            }
        }
        #endregion

        #region 门诊登记(挂号)撤销
        public static object[] YBMZDJCX(object[] objParam)
        {
            string sysdate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string Ywlx = "DJ02";
            string jzlsh = objParam[0].ToString();  //就诊流水号

            WriteLog(sysdate + "  " + jzlsh + " 进入门诊挂号撤销...");

            try
            {
                string jbr = CliUtils.fUserName;    //经办人
                string bxh = "";    //保险号
                string xm = "";     //姓名
                string kh = "";
                string ybjzlsh = "";
                if (string.IsNullOrEmpty(jzlsh))
                    return new object[] { 0, 0, "就诊流水号不能为空" };

                string cxrq = Convert.ToDateTime(sysdate).ToString("yyyyMMdd");
                string cxsj2 = Convert.ToDateTime(sysdate).ToString("HHmm");

                string strSql = string.Format("select  * from ybmzzydjdr where jzlsh='{0}' and cxbz=1 ", jzlsh);
                DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                if (ds.Tables[0].Rows.Count == 0)
                    return new object[] { 0, 0, "该患者未办理挂号登记" };
                else
                {
                    bxh = ds.Tables[0].Rows[0]["grbh"].ToString();
                    xm = ds.Tables[0].Rows[0]["xm"].ToString();
                    kh = ds.Tables[0].Rows[0]["kh"].ToString();
                    dqbh = ds.Tables[0].Rows[0]["tcqh"].ToString();
                    ybjzlsh = ds.Tables[0].Rows[0]["ybjzlsh"].ToString();
                }

                strSql = string.Format(@"select * from ybfyjsdr where jzlsh='{0}' and cxbz=1", jzlsh);
                ds.Tables.Clear();
                ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                if (ds.Tables[0].Rows.Count > 0)
                    return new object[] { 0, 0, "该患者已进行医保收费，不能撤销医保登记！" };

                ds.Dispose();
                /* 入参
                1 社会保险号 VARCHAR2(20) 否 医院实时结算平台接口规范
                2 社会保障卡号 VARCHAR2(20) 否
                3 姓名 VARCHAR2(20) 否
                4中心业务流水号 VARCHAR2(20) 否 门诊号
                5 地区编号 VARCHAR2(6) 否 统筹区编码
                */
                StringBuilder inputData = new StringBuilder();
                inputData.Append("SSSS|");
                inputData.Append(bxh + "|");
                inputData.Append(kh + "|");
                inputData.Append(xm + "|");
                inputData.Append(ybjzlsh + "|");
                inputData.Append(dqbh + "|");
                inputData.Append("ZZZZ");
                StringBuilder outData = new StringBuilder(1024);
                StringBuilder retMsg = new StringBuilder(1024);

                WriteLog(sysdate + "  入参|" + inputData.ToString());
                int i = f_UserBargaingApply(Ywlx, inputData, outData, retMsg);
                if (i > 0)
                {
                    List<string> liSQL = new List<string>();
                    strSql = string.Format(@"insert into ybmzzydjdr(
                                            jzlsh,jylsh,yllb,bzbm,bzmc,ysdm,ysxm,dgysdm,dgysxm,ghdjsj,
                                            zszbm,ksbh,ksmc,ghf,grbh,kh,xm,xb,nl,sfzh,
                                            csrq,zhye,yldylb,dwmc,zycs,tcqh,qxmc,jzbz,
                                            ybjzlsh,sylb,jhsylb,sysdate,jbr,cxbz) 
                                            select 
                                            jzlsh,jylsh,yllb,bzbm,bzmc,ysdm,ysxm,dgysdm,dgysxm,ghdjsj,
                                            zszbm,ksbh,ksmc,ghf,grbh,kh,xm,xb,nl,sfzh,
                                            csrq,zhye,yldylb,dwmc,zycs,tcqh,qxmc,jzbz,
                                            ybjzlsh,sylb,jhsylb,'{2}','{3}',0 
                                            from ybmzzydjdr 
                                           where jzlsh='{0}' and ybjzlsh='{1}' and cxbz=1 ", jzlsh, ybjzlsh, sysdate, jbr);
                    liSQL.Add(strSql);
                    strSql = string.Format(@"update ybmzzydjdr set cxbz =2 where jzlsh = '{0}' and ybjzlsh='{1}' and cxbz=1", jzlsh, ybjzlsh);
                    liSQL.Add(strSql);
                    strSql = string.Format(@"delete from ybmxbdj where jzlsh='{0}'", jzlsh);
                    liSQL.Add(strSql);
                    strSql = string.Format(@"update mz01t set m1kind='0201'  where m1ghno='{0}'", jzlsh);
                    liSQL.Add(strSql);

                    object[] obj = liSQL.ToArray();
                    obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);
                    if (obj[1].ToString() == "1")
                    {
                        WriteLog(sysdate + "  " + jzlsh + " 进入门诊挂号撤销成功|" + outData.ToString());
                        return new object[] { 0, 1, "门诊挂号撤销成功", outData.ToString() };
                    }
                    else
                    {
                        WriteLog(sysdate + "  " + jzlsh + " 进入门诊挂号撤销成功|操作本地数据失败|" + obj[2].ToString());
                        return new object[] { 0, 0, "门诊挂号撤销失败|" + obj[2].ToString() };
                    }
                }
                else
                {
                    WriteLog(sysdate + "  " + jzlsh + " 进入门诊挂号撤销失败|" + retMsg.ToString());
                    return new object[] { 0, 0, "门诊挂号撤销失败|" + retMsg.ToString() };
                }
            }
            catch (Exception ex)
            {
                WriteLog(sysdate + "  门诊登记(挂号)撤销异常|" + ex.Message);
                return new object[] { 0, 0, "门诊登记(挂号)撤销失败|" + ex.Message };
            }
        }
        #endregion

        #region 门诊登记（挂号）收费
        //入参:就诊流水号|医疗类别代码|结算时间|病种编码|病种名称|卡信息|读卡标志|定岗医生代码|定岗医生姓名
        public static object[] YBMZDJSF(object[] objParam)
        {
            string sysdate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string jzlsh = objParam[0].ToString();
            string yllb = objParam[1].ToString();
            string jssj = objParam[2].ToString();
            string bzbm = objParam[3].ToString();
            string bzmc = objParam[4].ToString();
            string kxx = objParam[5].ToString();
            string dkbz = objParam[6].ToString();
            string dgysdm = objParam[7].ToString();     //开方医生代码
            string dgysxm = objParam[8].ToString();     //开方医生姓名
            string zszbm ="";      //准生证编号
            string sylb = "";      //生育类别
            string jhsylb = "";    //计划生育类别
            string jbr=CliUtils.fLoginUser;
            string mzrq = "";
            string mzsj = "";
            string jylsh = DateTime.Now.ToString("yyyyMMddHHmmss") + "-" + new Random().Next(100).ToString().PadLeft(4, '0');

            try
            {
                #region 判断结算日期时间有效性
                try
                {
                    mzrq = Convert.ToDateTime(jssj).ToString("yyyyMMdd");
                    mzsj = Convert.ToDateTime(jssj).ToString("HHmm");
                }
                catch
                {
                    return new object[] { 0, 0, "挂号日期时间格式不正确" };
                }
                #endregion
                object[] objParam1 = { jzlsh, yllb, bzbm, bzmc, kxx, jssj, dgysdm, dgysxm, zszbm, sylb, jhsylb };
                objParam1 = YBMZDJ(objParam1);
                if (objParam1[1].ToString().Equals("1"))
                {
                    //门诊登记成功,进入门诊登记（挂号）收费结算
                    #region 获取门诊登记信息
                    string strSql = string.Format("select * from ybmzzydjdr where jzlsh='{0}' and cxbz=1", jzlsh);
                    DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                    if (ds.Tables[0].Rows.Count == 0)
                        return new object[] { 0, 0, "该患者未做医保登记" };
                    string grbh = ds.Tables[0].Rows[0]["grbh"].ToString();
                    string xm = ds.Tables[0].Rows[0]["xm"].ToString();
                    string kh = ds.Tables[0].Rows[0]["kh"].ToString();
                    string dqbh = ds.Tables[0].Rows[0]["tcqh"].ToString();
                    //string dgysdm1 = ds.Tables[0].Rows[0]["dgysdm"].ToString();
                    //string dgysxm1 = ds.Tables[0].Rows[0]["dgysxm"].ToString();
                    string ybjzlsh = ds.Tables[0].Rows[0]["ybjzlsh"].ToString();
                    string bzbm1 = ds.Tables[0].Rows[0]["bzbm"].ToString() == "" ? "NULL" : bzbm;
                    string bzmc1 = ds.Tables[0].Rows[0]["bzmc"].ToString() == "" ? "NULL" : bzmc;
                    string sfdypj = "TRUE"; //是否打印票据
                    string zhzfbz = "1"; //账户支付标志

                    StringBuilder inputData = new StringBuilder();
                    /*
                    1 社会保险号 VARCHAR2(20) 否
                    2 社会保障卡号 VARCHAR2(20) 否
                    3 姓名 VARCHAR2(20) 否
                    4 中心业务流水号 VARCHAR2(20) 否 门诊号
                    5 医疗类别 VARCHAR2(3) 代码（详见字典说明）
                    6 门特病种编码 VARCHAR2(20) 多病种以分隔符“#”号连接
                    7 门特病种名称 VARCHAR2(100) 多病种以分隔符“#”号连接
                    8 定岗医师编码 VARCHAR2(20) 定岗医师\药师编号，同 DJ13
                    9 定岗医师姓名 VARCHAR2(20)
                    10 门诊日期 VARCHAR2(8) YYYYMMDD
                    11 门诊时间 VARCHAR2(6) HH24MISS
                    12 个人账户抵扣现金支付标志 VARCHAR2(3) 0-不予支付；1-支付（默认 1）（政策允许情况下才有效）
                    13 是否打印结算单据 VARCHAR2(8) 作废，由接口配置文件配置项Js05Print控制
                    14 生育类别 VARCHAR2(3) 编码，见“字典说明”部分。“医疗类别”为“51-生育门诊”、“52-生育住院”时，第14、15项至少输入一项。 
                    15 计划生育类别 VARCHAR2(3)
                    16 地区编号 VARCHAR2(6) 否 统筹区编码
                     */
                    inputData.Append("SSSS|");
                    inputData.Append(grbh + "|");
                    inputData.Append(kh + "|");
                    inputData.Append(xm + "|");
                    inputData.Append(ybjzlsh + "|");
                    inputData.Append(yllb + "|");
                    inputData.Append(bzbm1 + "|");
                    inputData.Append(bzmc1 + "|");
                    inputData.Append(dgysdm + "|");
                    inputData.Append(dgysxm + "|");
                    inputData.Append(mzrq + "|");
                    inputData.Append(mzsj + "|");
                    inputData.Append(zhzfbz + "|");
                    inputData.Append(sfdypj + "|");
                    inputData.Append(sylb + "|");
                    inputData.Append(jhsylb + "|");
                    inputData.Append(dqbh + ";");
                    #endregion

                    #region 获取挂号明细信息
                    string ybxmbh = string.Empty;
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

                    List<string> liSQL = new List<string>();

                    List<string> li_yyxmbh = new List<string>();
                    List<string> li_yyxmmc = new List<string>();
                    List<string> li_ybxmbh = new List<string>();
                    List<string> li_je = new List<string>();
                    List<string> li_cfh = new List<string>();
                    List<string> li_dj = new List<string>();

                    #region 获取诊查费用信息
                    strSql = string.Format(@"select  z.ybxmbh ybxmbh, z.ybxmmc ybxmmc, c.bzmem1 as dj,1 as sl,a.m1gham je,c.bzmem2 yyxmbh, 
                                        c.bzname yyxmmc, a.m1invo,a.m1blam,a.m1kham,a.m1amnt
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
                        ybxmbh = dr["ybxmbh"].ToString();
                        ybxmmc = dr["ybxmmc"].ToString();
                        dj = Convert.ToDouble(dr["dj"].ToString());
                        sl = Convert.ToDouble(dr["sl"].ToString());
                        //je = Convert.ToDouble(dr["je"].ToString());
                        yyxmmc = dr["yyxmmc"].ToString();
                        yyxmbm = dr["yyxmbh"].ToString();
                        djhin = dr["m1invo"].ToString();
                        ghfylj += je;
                        li_yyxmbh.Add(dr["yyxmbh"].ToString());
                        li_yyxmmc.Add(dr["yyxmmc"].ToString());
                        li_ybxmbh.Add(dr["ybxmbh"].ToString());
                        li_je.Add(je.ToString());
                        li_dj.Add(dj.ToString());
                        /*
                        发送的附加消息包：多记录往后续加消息包
                        1 定点医疗机构明细序号 VARCHAR2(20) 否
                        2 定点医疗机构三大目录编码 VARCHAR2(30) 否
                        3 定点医疗机构三大目录名称 VARCHAR2(150) 否
                        4 定点医疗机构三大目录规格 VARCHAR2(50)
                        5 定点医疗机构三大目录剂型 VARCHAR2(50)
                        6 单价 NUMBER(10,4) 否
                        7 数量 NUMBER(8,2) 否
                        8 金额 NUMBER(10,4) 否
                        9 社保三大目录编码 VARCHAR2(20) 否
                        10 社保三大目录名称 VARCHAR2(150) 否
                        11 剂量单位 VARCHAR2(50) 定点机构信息。2013/11/21新增
                        12 用法用量
                        */
                        inputData.Append(isequ + "|");
                        inputData.Append(yyxmbm + "|");
                        inputData.Append(yyxmmc + "|");
                        inputData.Append("NULL|");
                        inputData.Append("NULL|");
                        inputData.Append(dj + "|");
                        inputData.Append(sl + "|");
                        inputData.Append(je + "|");
                        inputData.Append(ybxmbh + "|");
                        inputData.Append(ybxmmc + "|");
                        inputData.Append("NULL;");
                        isequ++;

                        strSql = string.Format(@"insert into ybcfmxscindr(jzlsh,jylsh,xm,kh,ybjzlsh,cfrq,yysfxmbm,yysfxmmc,sfxmzxbm,sfxmzxmc,
                                            dj,sl,je,jbr,sysdate) values(
                                            '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}',
                                            ,'{10}','{11}','{12}','{13}','{14}')",
                                                jzlsh, jylsh, xm, kh, ybjzlsh, cfrq, yyxmbm, yyxmmc, ybxmbh, ybxmmc,
                                                dj, sl, je, jbr, sysdate);
                        liSQL.Add(strSql);
                    }
                    #endregion

                    #region 获取病历费用信息
                    if (blbfy > 0)
                    {
                        strSql = string.Format(@"select c.ybxmbh,c.ybxmmc,b.b5sfam as dj,1 as sl,b.b5sfam as je,a.pamark as yyxmbh,b.b5name as yyxmmc from patbh a
                                            left join bz05d b on a.pamark=b.b5item
                                            left join ybhisdzdr c on a.pamark=c.hisxmbh and b.b5item=c.hisxmbh
                                            where pakind='MZ' and pasequ='0001'");
                        ds.Tables.Clear();
                        ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                        dr = ds.Tables[0].Rows[0];
                        ybxmbh = dr["ybxmbh"].ToString();
                        ybxmmc = dr["ybxmmc"].ToString();
                        dj = Convert.ToDouble(dr["dj"].ToString());
                        sl = Convert.ToDouble(dr["sl"].ToString());
                        je = Convert.ToDouble(dr["je"].ToString());
                        yyxmmc = dr["yyxmmc"].ToString();
                        yyxmbm = dr["yyxmbh"].ToString();
                        ghfylj += je;

                        li_yyxmbh.Add(dr["yyxmbh"].ToString());
                        li_yyxmmc.Add(dr["yyxmmc"].ToString());
                        li_ybxmbh.Add(dr["ybxmbh"].ToString());
                        li_je.Add(je.ToString());
                        li_dj.Add(dj.ToString());

                        /*
                        发送的附加消息包：多记录往后续加消息包
                        1 定点医疗机构明细序号 VARCHAR2(20) 否
                        2 定点医疗机构三大目录编码 VARCHAR2(30) 否
                        3 定点医疗机构三大目录名称 VARCHAR2(150) 否
                        4 定点医疗机构三大目录规格 VARCHAR2(50)
                        5 定点医疗机构三大目录剂型 VARCHAR2(50)
                        6 单价 NUMBER(10,4) 否
                        7 数量 NUMBER(8,2) 否
                        8 金额 NUMBER(10,4) 否
                        9 社保三大目录编码 VARCHAR2(20) 否
                        10 社保三大目录名称 VARCHAR2(150) 否
                        11 剂量单位 VARCHAR2(50) 定点机构信息。2013/11/21新增
                        */
                        inputData.Append(isequ + "|");
                        inputData.Append(yyxmbm + "|");
                        inputData.Append(yyxmmc + "|");
                        inputData.Append("NULL|");
                        inputData.Append("NULL|");
                        inputData.Append(dj + "|");
                        inputData.Append(sl + "|");
                        inputData.Append(je + "|");
                        inputData.Append(ybxmbh + "|");
                        inputData.Append(ybxmmc + "|");
                        inputData.Append("NULL|");
                        inputData.Append("NULL|");
                        inputData.Append("ZZZZ");
                        isequ++;

                        strSql = string.Format(@"insert into ybcfmxscindr(jzlsh,jylsh,xm,kh,ybjzlsh,cfrq,yysfxmbm,yysfxmmc,sfxmzxbm,sfxmzxmc,
                                            dj,sl,je,jbr,sysdate) values(
                                            '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}',
                                            '{10}','{11}','{12}','{13}','{14}')",
                                                jzlsh, jylsh, xm, kh, ybjzlsh, cfrq, yyxmbm, yyxmmc, ybxmbh, ybxmmc,
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

                        ybxmbh = dr["ybxmbh"].ToString();
                        ybxmmc = dr["ybxmmc"].ToString();
                        dj = Convert.ToDouble(dr["dj"].ToString());
                        sl = Convert.ToDouble(dr["sl"].ToString());
                        je = Convert.ToDouble(dr["je"].ToString());
                        yyxmmc = dr["yyxmmc"].ToString();
                        yyxmbm = dr["yyxmbh"].ToString();
                        ghfylj += je;

                        li_yyxmbh.Add(dr["yyxmbh"].ToString());
                        li_yyxmmc.Add(dr["yyxmmc"].ToString());
                        li_ybxmbh.Add(dr["ybxmbh"].ToString());
                        li_je.Add(je.ToString());
                        li_dj.Add(dj.ToString());

                        /*
                        发送的附加消息包：多记录往后续加消息包
                        1 定点医疗机构明细序号 VARCHAR2(20) 否
                        2 定点医疗机构三大目录编码 VARCHAR2(30) 否
                        3 定点医疗机构三大目录名称 VARCHAR2(150) 否
                        4 定点医疗机构三大目录规格 VARCHAR2(50)
                        5 定点医疗机构三大目录剂型 VARCHAR2(50)
                        6 单价 NUMBER(10,4) 否
                        7 数量 NUMBER(8,2) 否
                        8 金额 NUMBER(10,4) 否
                        9 社保三大目录编码 VARCHAR2(20) 否
                        10 社保三大目录名称 VARCHAR2(150) 否
                        11 剂量单位 VARCHAR2(50) 定点机构信息。2013/11/21新增
                        */
                        inputData.Append(isequ + "|");
                        inputData.Append(yyxmbm + "|");
                        inputData.Append(yyxmmc + "|");
                        inputData.Append("NULL|");
                        inputData.Append("NULL|");
                        inputData.Append(dj + "|");
                        inputData.Append(sl + "|");
                        inputData.Append(je + "|");
                        inputData.Append(ybxmbh + "|");
                        inputData.Append(ybxmmc + "|");
                        inputData.Append("NULL;");
                        isequ++;
                        strSql = string.Format(@"insert into ybcfmxscindr(jzlsh,jylsh,xm,kh,ybjzlsh,cfrq,yysfxmbm,yysfxmmc,sfxmzxbm,sfxmzxmc,
                                            dj,sl,je,jbr,sysdate) values(
                                            '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}',
                                            ,'{10}','{11}','{12}','{13}','{14}')",
                                                jzlsh, jylsh, xm, kh, ybjzlsh, cfrq, yyxmbm, yyxmmc, ybxmbh, ybxmmc,
                                                dj, sl, je, jbr, sysdate);
                        liSQL.Add(strSql);
                    }
                    #endregion
                    ds.Dispose();
                    #endregion
                    //判断总费用与累计费用是否相等
                    if (Math.Abs(ghfy - ghfylj) > 1.0)
                        return new object[] { 0, 0, "收费金额与医保结算金额相差" + Math.Abs(ghfy - ghfylj) + ",无法结算，请核实!" };

                    #region 费用结算
                    string Ywlx = "JS05";
                    StringBuilder outData = new StringBuilder(10240);
                    StringBuilder retMsg = new StringBuilder(1024);
                    WriteLog(sysdate + "  " + jzlsh + " 进入门诊登记（挂号）收费结算...");
                    WriteLog(sysdate + "  入参|" + inputData.ToString());
                    int i = f_UserBargaingApply(Ywlx, inputData, outData, retMsg);
                    
                    WriteLog(i.ToString() + "|" + outData.ToString() + "|" + retMsg.ToString());
                    if (i > 0)
                    {
                        WriteLog(sysdate + "  出参|" + outData.ToString());
                        WriteLog(sysdate + "  出参1|" + outData.ToString().Replace("RRRR|", ""));
                        #region 出参
                        /*出参
                        1 社会保险号 VARCHAR2(20)
                        2 社会保障卡号 VARCHAR2(20)
                        3 姓名 VARCHAR2(20)
                        4 性别 VARCHAR2(10) 名称
                        5 出生日期 VARCHAR2(8) YYYYMMDD
                        6 地区编号 VARCHAR2(6) 统筹区编码
                        7 地区名称 VARCHAR2(20)
                        8 参保日期 VARCHAR2(8) YYYYMMDD
                        9 参保身份 VARCHAR2(50) 格式“编码-名称”，如“1101-企业一般人员”。见“字典说明”
                        10 行政职务级别 VARCHAR2(50) 格式“编码-名称”，如“121-厅局级正职”。见“字典说明”
                        11 医疗待遇享受类别 VARCHAR2(50) 格式“编码-名称”，如“1101-统账在职”。见“字典说明”（仅适用于新余市）
                        12 单位名称 VARCHAR2(100) 格式“单位编码-单位名称”， 如“666666-测试单位”
                        13 单位类型 VARCHAR2(20) 格式“编码-名称”，如“100-正常企业”。见“字典说明”
                        14 中心业务流水号 VARCHAR2(20) 住院号
                        15 中心结算单据号 VARCHAR2(20)
                        16 医疗类别 VARCHAR2(50) 名称
                        17 结算费用起始日期 VARCHAR2(8)
                        18 结算费用截止日期 VARCHAR2(8)
                        19 医疗总费用 NUMBER(16,2)  2609
                        20 本次账户支付 NUMBER(16,2)
                        21 本次现金支付 NUMBER(16,2)
                        22 职工基本医疗基金支付 NUMBER(16,2)
                        23 城镇居民基本医疗基金支付 NUMBER(16,2)
                        24 城乡居民基本医疗基金支付 NUMBER(16,2)
                        25 大病医疗基金支付 NUMBER(16,2)
                        26 公务员补充医疗基金支付 NUMBER(16,2)
                        27 企业补充医疗基金支付 NUMBER(16,2)
                        28 二乙专项医疗基金支付 NUMBER(16,2)
                        29 老红军专项医疗基金支付 NUMBER(16,2)
                        30 离休干部单独统筹基金支付 NUMBER(16,2)
                        31 医疗保健支付 NUMBER(16,2) 如新余地厅级干部医疗保健补差
                        32 其他基金支付 NUMBER(16,2) 预留指标项。（抚州市：医疗补贴金额。已计入统筹及大病基金中，此为其中项。）
                        33 工伤基金支付 NUMBER(16,2)
                        34 生育基金支付 NUMBER(16,2)
                        35 民政救助支付 NUMBER(16,2)
                        36 单位负担支付 NUMBER(16,2)
                        37 定点机构支付 NUMBER(16,2)
                        38 医保账户余额 NUMBER(16,2)
                        39 甲类费用 NUMBER(16,2)
                        40 乙类费用 NUMBER(16,2)
                        41 丙类费用 NUMBER(16,2) 抚州指“乙类*”
                        42 自费费用 NUMBER(16,2)
                        43 先行自付 NUMBER(16,2)
                        44 起付段自付 NUMBER(16,2)
                        45 转外自付 NUMBER(16,2)
                        46 统筹段自付 NUMBER(16,2)
                        47 大病统筹自付 NUMBER(16,2)
                        48 公务员补充医疗自付 NUMBER(16,2)
                        49 超过最高封顶线自付 NUMBER(16,2)
                        50 转诊单位负担 NUMBER(16,2)
                        51 统筹段单位负担 NUMBER(16,2)
                        52 大病段单位负担 NUMBER(16,2)
                        53 统筹段基金支付 NUMBER(16,2)
                        54 大病段基金支付 NUMBER(16,2)
                        55 个人账户抵扣现金支付标志 VARCHAR2(3) 0-不予支付；1-支付（默认 1）
                        56 公务员补助基本医疗支付 NUMBER(16,2) 新余：指补助 65%部分。其他：补助统筹段支付
                        57 公务员补助大病医疗支付 NUMBER(16,2) 新余：指补助 80%部分。其他：补助大病段支付
                        58 结算人 VARCHAR2(20)
                        59 结算日期 VARCHAR2(8) YYYYMMDD
                        60 结算时间 VARCHAR2(6) HH24MISS
                         * 
                        返回的附加消息包：费用明细，多记录往后续加消息包。*门诊返回，住院不返回。

                        1 社保三大目录编码 VARCHAR2(20)
                        2 社保三大目录名称 VARCHAR2(150)
                        3 收费项目等级 VARCHAR2(20) 格式“编码-名称”，如“1-甲类”。见“字典说明”
                        4 发票项目类别 VARCHAR2(20) 格式“编码-名称”，如“11-西药”。见“字典说明”
                        5 单价 NUMBER(10,4)
                        6 数量 NUMBER(8,2)
                    */
                        string[] str = outData.ToString().Replace("RRRR|", "").Split(';');
                        string[] str2 = str[0].Split('|');
                        outParams_js op = new outParams_js();
                        op.Grbh = str2[0].Trim();       //保险号
                        op.Kh = str2[1].Trim();         //卡号
                        op.Xm = str2[2].Trim();         //姓名
                        op.Xb = str2[3].Trim();         //性别
                        op.Csrq = str2[4].Trim();       //出生日期
                        op.Dqbh = str2[5].Trim();      //地区编号
                        op.Dqmc = str2[6].Trim();       //地区名称
                        op.Cbrq = str2[7].Trim();       //参保日期
                        op.Cbsf = str2[8].Trim();       //参保身份
                        op.Xzzwjb = str2[9].Trim();     //行政职务级别
                        op.Yldylb = str2[10].Trim();    //医疗待遇类别
                        op.Dwmc = str2[11].Trim();      //单位名称
                        op.Dwlx = str2[12].Trim();      //单位类别
                        op.Ybjzlsh = str2[13].Trim();   //中心业务流水号，医保就诊流水号
                        op.Jslsh = str2[14].Trim();     //结算单据号
                        op.Yllb = str2[15].Trim();      //医疗类别
                        op.Jsqsrq = str2[16].Trim();    //结算起始日期
                        op.Jsjzrq = str2[17].Trim();    //结算截止日期
                        op.Ylfze = str2[18].Trim();     //医疗总费用
                        op.Zhzf = str2[19].Trim();      //本次帐户支付
                        op.Xjzf = str2[20].Trim();      //本次现金支付
                        op.Zgjbyljjzf = str2[21].Trim();
                        op.Jzjmjbyljjzf = str2[22].Trim();
                        op.Cxjmjbyljjzf = str2[23].Trim();
                        op.Dejjzf = str2[24].Trim();
                        op.Gwybzjjzf = str2[25].Trim();
                        op.Qybcylbxjjzf = str2[26].Trim();
                        op.Eyzxyljjzf = str2[27].Trim();
                        op.Lhjzxyljjzf = str2[28].Trim();
                        op.Lxgbdttcjjzf = str2[29].Trim();
                        op.Ylbjzf = str2[30].Trim();
                        op.Qtjjzf = str2[31].Trim();
                        op.Gsjjzf = str2[32].Trim();
                        op.Syjjzf = str2[33].Trim();
                        op.Mzjzfy = str2[34].Trim();
                        op.Dwfdfy = str2[35].Trim();
                        op.Yyfdfy = str2[36].Trim();
                        op.Bcjshzhye = str2[37].Trim();
                        op.Jlfy = str2[38].Trim();
                        op.Ylfy = str2[39].Trim();
                        op.Blfy = str2[40].Trim();
                        op.Zffy = str2[41].Trim();
                        op.Xxzf = str2[42].Trim();      //先行自付
                        op.Qfbzfy = str2[43].Trim();
                        op.Zzzyzffy = str2[44].Trim();
                        op.Tcfdzffy = str2[45].Trim();
                        op.Defdzffy = str2[46].Trim();
                        op.Gwybcylzf = str2[47].Trim();
                        op.Cxjfy = str2[48].Trim();     //
                        op.Zzdwfd = str2[49].Trim();
                        op.Tcddwfd = str2[50].Trim();
                        op.Dbddwfd = str2[51].Trim();
                        op.Jrtcfy = str2[52].Trim();
                        op.Jrdebxfy = str2[53].Trim();
                        op.Zhdkxjzfbz = str2[54].Trim();
                        op.Gwybcjbylzf = str2[55].Trim();
                        op.Gwybzdbylzf = str2[56].Trim();
                        op.Jbr = str2[57].Trim();
                        op.Jsrq = str2[58].Trim();
                        op.Jssj = str2[59].Trim();
                        op.Bcjsqzhye = (Convert.ToDecimal(op.Bcjshzhye) + Convert.ToDecimal(op.Zhzf)).ToString();
                        op.Zbxje = (Convert.ToDecimal(op.Ylfze) - Convert.ToDecimal(op.Xjzf) - Convert.ToDecimal(op.Yyfdfy)).ToString();
                        op.Tcjjzf = (Convert.ToDecimal(op.Ylfze) - Convert.ToDecimal(op.Xjzf) - Convert.ToDecimal(op.Zhzf)
                                    - Convert.ToDecimal(op.Dejjzf) - Convert.ToDecimal(op.Mzjzfy) - Convert.ToDecimal(op.Yyfdfy)).ToString();
                        #endregion
                        op.Cxjfy = "0.00";
                        op.Djh = op.Jslsh;

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
                        string strValue = op.Ylfze + "|" + op.Zbxje + "|" + op.Tcjjzf + "|" + op.Dejjzf + "|" + op.Zhzf + "|" +
                                        op.Xjzf + "|" + op.Gwybzjjzf + "|" + op.Qybcylbxjjzf + "|" + op.Zffy + "|" + op.Dwfdfy + "|" +
                                        op.Yyfdfy + "|" + op.Mzjzfy + "|" + op.Cxjfy + "|" + op.Ylzlfy + "|" + op.Blzlfy + "|" +
                                        op.Fhjjylfy + "|" + op.Qfbzfy + "|" + op.Zzzyzffy + "|" + op.Jrtcfy + "|" + op.Tcfdzffy + "|" +
                                        op.Ctcfdxfy + "|" + op.Jrdebxfy + "|" + op.Defdzffy + "|" + op.Cdefdxfy + "|" + op.Rgqgzffy + "|" +
                                        op.Bcjsqzhye + "|" + op.Bntczflj + "|" + op.Bndezflj + "|" + op.Bnczjmmztczflj + "|" + op.Bngwybzzflj + "|" +
                                        op.Bnzhzflj + "|" + op.Bnzycslj + "|" + op.Zycs + "|" + op.Xm + "|" + op.Jsrq + op.Jssj + "|" +
                                        op.Yllb + "|" + op.Yldylb + "|" + op.Jbjgbm + "|" + op.Ywzqh + "|" + op.Jslsh + "|" +
                                        op.Tsxx + "|" + op.Djh + "|" + op.Jyxl + "|" + op.Yyjylsh + "|" + op.Yxbz + "|" +
                                        op.Grbhgl + "|" + op.Yljgbm + "|" + op.Ecbcje + "|" + op.Mmqflj + "|" + op.Jsfjylsh + "|" +
                                        op.Grbh + "|" + op.Dbzbc + "|" + op.Czzcje + "|" + op.Elmmxezc + "|" + op.Elmmxesy + "|" +
                                        op.Jmgrzfecbcje + "|" + op.Tjje + "|" + op.Syjjzf + "|";

                        strSql = string.Format(@"insert into ybfyjsdr(
                                                jzlsh,djhin,yllb,bzbm,bzmc,grbh,kh,xm,ybjzlsh,djh,
                                                jslsh,xzzwjb,dqbh,dqmc,cbsf,yldyxslb,dwmc,dwlx,cbrq,jsqsrq,
                                                jsjzrq,ylfze,zbxje,tcjjzf,zhzf,xjzf,dejjzf,gwybzjjzf,qybcylbxjjzf,lxgbddtczf,
                                                mzjzfy,dwfdfy,yyfdfy,bcjsqzhye,jlfy,ylfy,blfy,zffy,qfbzfy,zgjbyljjzf,
                                                czjmjbyljjzf,cxjmjjyljjzf,eyzxyljjzf,lhjzxyljjzf,ylbjzf,qtjjzf,gsjjzf,syjjzf,xxzf,zzzyzffy,
                                                jrtczf,jrdbzf,gwybcylzf,cxjfy,zzdwfd,tcddwfd,jrtcfy,jrdebxfy,zhzfbz,gwybzjbylzf,
                                                gwybzdbylzf,jsrq,jbr,sysdate,jylsh) values(
                                                '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}',
                                                '{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}',
                                                '{20}','{21}','{22}','{23}','{24}','{25}','{26}','{27}','{28}','{29}',
                                                '{30}','{31}','{32}','{33}','{34}','{35}','{36}','{37}','{38}','{39}',
                                                '{40}','{41}','{42}','{43}','{44}','{45}','{46}','{47}','{48}','{49}',
                                                '{50}','{51}','{52}','{53}','{54}','{55}','{56}','{57}','{58}','{59}',
                                                '{60}','{61}','{62}','{63}','{64}')",
                                               jzlsh, djhin, op.Yllb, bzbm, bzmc, op.Grbh, op.Kh, op.Xm, op.Ybjzlsh, djhin,
                                               op.Jslsh, op.Xzzwjb, op.Dqbh, op.Dqmc, op.Cbsf, op.Yldylb, op.Dwmc, op.Dwlx, op.Cbrq, op.Jsqsrq,
                                               op.Jsjzrq, op.Ylfze, op.Zbxje, op.Tcjjzf, op.Zhzf, op.Xjzf, op.Dejjzf, op.Gwybzjjzf, op.Qybcylbxjjzf, op.Lxgbdttcjjzf,
                                               op.Mzjzfy, op.Dwfdfy, op.Yyfdfy, op.Bcjsqzhye, op.Jlfy, op.Ylfy, op.Blfy, op.Zffy, op.Qfbzfy, op.Zgjbyljjzf,
                                               op.Jzjmjbyljjzf, op.Cxjmjbyljjzf, op.Eyzxyljjzf, op.Lhjzxyljjzf, op.Ylbjzf, op.Qtjjzf, op.Gsjjzf, op.Syjjzf,op.Xxzf, op.Zzzyzffy,
                                               op.Tcfdzffy, op.Defdzffy, op.Gwybcylzf, op.Cxjfy, op.Zzdwfd, op.Tcddwfd, op.Jrtcfy, op.Jrdebxfy, op.Zhdkxjzfbz, op.Gwybcjbylzf,
                                               op.Gwybzdbylzf, op.Jsrq + op.Jssj, jbr, sysdate, jylsh);
                        liSQL.Add(strSql);

                        /*
                         *  1社保三大目录编码 VARCHAR2(20)
                            2社保三大目录名称 VARCHAR2(150)
                            3 收费项目等级 VARCHAR2(20) 格式“编码-名称”，如“1-甲类”。见“字典说明”
                            4 发票项目类别 VARCHAR2(20) 格式“编码-名称”，如“11-西药”。见“字典说明”
                            5 单价 NUMBER(10,4)
                            6 数量 NUMBER(8,2) 
                            7 金额 NUMBER(10,4)
                            8 自付金额 NUMBER(10,4)
                            9 限价标准 NUMBER(10,4)
                            10 超限价自费金额 NUMBER(10,4)
                            11 限制使用标志 VARCHAR2(3) 是/否
                         */
                        for (int j = 1; j < str.Length; j++)
                        {
                            str2 = str[j].Split('|');
                            outParams_fymx opfy = new outParams_fymx();
                            opfy.Ybxmbm = str2[0].Trim();
                            opfy.Ybxmmc = str2[1].Trim();
                            opfy.Sfxmdj = str2[2].Trim();
                            opfy.Sfxmlb = str2[3].Trim();
                            opfy.Dj = str2[4].Trim();
                            opfy.Sl = str2[5].Trim();
                            opfy.Je = str2[6].Trim();
                            opfy.Zfje = str2[7].Trim();
                            opfy.Xjbz = str2[8].Trim();
                            opfy.Cxjzfje = str2[9].Trim();
                            opfy.Xzsybz = str2[10].Trim();
                            for (int k = 0; k < li_ybxmbh.Count; k++)
                            {
                                if (opfy.Ybxmbm.Equals(li_ybxmbh[k]) && opfy.Dj.Equals(li_dj[k]) && opfy.Je.Equals(li_je[k]))
                                {
                                    strSql = string.Format(@"insert into ybcfmxscfhdr(jzlsh,jylsh,ybjzlsh,xm,kh,yybxmbh,ybxmmc,yyxmdm,yyxmmc,sfxmdj,
                                                        sflb,dj,sl,je,zfje,zlje,cxjzfje,bz,grbh,sysdate) values(
                                                        '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}',
                                                        '{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}')",
                                                        jzlsh, jylsh, op.Ybjzlsh, op.Xm, op.Kh, li_yyxmbh[k], li_yyxmmc[k], opfy.Ybxmbm, opfy.Ybxmmc, opfy.Sfxmdj,
                                                        opfy.Sfxmlb, opfy.Dj, opfy.Sl, opfy.Je, opfy.Zfje, opfy.Xjbz, opfy.Cxjzfje, opfy.Xzsybz, op.Grbh, sysdate);
                                    liSQL.Add(strSql);
                                    break;
                                }
                            }
                        }

                        object[] obj = liSQL.ToArray();
                        obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);
                        if (obj[1].ToString().Equals("1"))
                        {
                            WriteLog(sysdate + "  " + jzlsh + " 门诊登记（挂号）收费结算成功|" + strValue);
                            return new object[] { 0, 1, strValue };
                        }
                        else
                        {
                            WriteLog(sysdate + "  " + jzlsh + " 门诊登记（挂号）收费结算成功|操作本地数据失败|" + obj[2].ToString());
                            //撤销门诊登记（挂号）收费
                            object[] objParam2 = new object[] { jzlsh, op.Grbh, op.Kh, op.Xm, op.Ybjzlsh, op.Jslsh, op.Dqbh };
                            NYBMZSFJSCX(objParam2);
                            //撤销门诊登记（挂号）
                            objParam2 = new object[] { jzlsh };
                            YBMZDJCX(objParam2);
                            return new object[] { 0, 0, "门诊登记（挂号）收费结算失败" };
                        }
                    }
                    else
                    {
                        WriteLog(sysdate + "  " + jzlsh + " 进入门诊登记（挂号）收费结算失败|" + retMsg.ToString());
                        //门诊登记(挂号)撤销
                        object[] objParam2 = new object[] { jzlsh };
                        YBMZDJCX(objParam2);
                        return new object[] { 0, 0, "门诊登记（挂号）收费结算失败|" + retMsg.ToString() };
                    }
                    #endregion
                }
                else
                {
                    return objParam1;
                }
            }
            catch (Exception ex)
            {
                WriteLog(sysdate + ex.Message);
                return new object[] { 0, 0, "系统异常|" + ex.Message };
            }

        }
        #endregion

        #region 门诊登记（挂号）收费撤销
        public static object[] YBMZDJSFCX(object[] objParam)
        {
            string sysdate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string jbr = CliUtils.fUserName;
            string jzlsh = objParam[0].ToString();

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
                ds.Dispose();
                object[] objParam1 = new object[] { jzlsh, "1" };
                //门诊就诊疾病诊断登记撤销
                YBZDDJCX(objParam1);
                //门诊登记（挂号）撤销
                return YBMZDJCX(objParam1);
            }
            else
            {
                strSql = string.Format(@"select m1invo from mz01t where m1ghno='{0}' ", jzlsh);
                ds.Tables[0].Clear();
                ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                //进行门诊登记（挂号）收费，需撤销费用后再撤销登记
                string djh = ds.Tables[0].Rows[0]["m1invo"].ToString();
                ds.Dispose();
                object[] objParam1 = new object[] { jzlsh, djh };
                object[] objReturn = YBMZSFJSCX(objParam1);
                if (objReturn[1].ToString().Equals("1"))
                {
                    //门诊就诊疾病诊断登记撤销
                    object[] objParam2 = new object[] { jzlsh, "1" };
                    YBZDDJCX(objParam2);
                    //撤销门诊登记（挂号）
                    return YBMZDJCX(objParam1);
                }
                else
                    return objReturn;
            }
        }
        #endregion

        #region 门诊收费预结算
        public static object[] YBMZSFYJS(object[] objParam)
        {
            string Ywlx = "JS04";
            string sysdate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            
            #region HIS入参
            string jzlsh = objParam[0].ToString();  //就诊流水号
            string zhsybz = objParam[1].ToString(); //账户使用标志
            string jssj = objParam[2].ToString();   //结算时间
            string bzbh = objParam[3].ToString();   //病种编号
            string bzmc = objParam[4].ToString();   //病种名称
            string cfhs = objParam[5].ToString();   //处方号集
            string yllb = objParam[6].ToString();   //医疗类别
            string sfje = objParam[7].ToString();   //收费金额
            string cfysdm = objParam[8].ToString(); //处方医生代码
            string cfysxm = objParam[9].ToString(); //处方医生姓名
            #endregion

            string djh = "0000";    //单据号（发票号)
            string sfdypj = "TRUE";                //是否打印票据
            double sfje2 = 0.0000; //金额 
            string ybjzlsh = "";   //医保就诊流水号
            string bxh = "";
            string xm = "";
            string kh = "";
            string dgysbm = cfysdm; //定岗医生编码
            string dgysxm = cfysxm; //定岗医生姓名
            string mzrq = string.Empty;
            string mzsj = string.Empty;
            string sylb = "NULL";   //生育类别
            string jhsylb = "NULL"; //计划生育类别
            
            

            if (string.IsNullOrEmpty(jzlsh))
                return new object[] { 0, 0, "就诊流水号不能为空" };
            if (string.IsNullOrEmpty(cfhs))
                return new object[] { 0, 0, "处方号集不能为空" };
            if (string.IsNullOrEmpty(sfje))
                return new object[] { 0, 0, "收费金额不能为空" };
            if (string.IsNullOrEmpty(dgysbm))
                return new object[] { 0, 0, "处方医生不能为空" };


            WriteLog(sysdate + "  " + jzlsh + " 进入门诊收费预结算...");


            #region 金额有效性
            try
            {
                sfje2 = Convert.ToDouble(sfje);
            }
            catch
            {
                return new object[] { 0, 0, "收费金额格式错误" };
            }
            #endregion

            # region 是否门诊医保挂号
            string strSql = string.Format("select * from ybmzzydjdr where jzlsh='{0}' and cxbz=1", jzlsh);
            DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            if (ds.Tables[0].Rows.Count == 0)
            {
                return new object[] { 0, 0, "该患者未办理医保挂号登记" };
            }
            else
            {
                bxh = ds.Tables[0].Rows[0]["grbh"].ToString();
                xm = ds.Tables[0].Rows[0]["xm"].ToString();
                kh = ds.Tables[0].Rows[0]["kh"].ToString();
                dqbh = ds.Tables[0].Rows[0]["tcqh"].ToString();
                ybjzlsh = ds.Tables[0].Rows[0]["ybjzlsh"].ToString();
                yllb = ds.Tables[0].Rows[0]["yllb"].ToString();
                sylb = ds.Tables[0].Rows[0]["sylb"].ToString();
                jhsylb = ds.Tables[0].Rows[0]["jhsylb"].ToString();

                string[] syllb = { "14", "16" };
                if (syllb.Contains(yllb))
                {
                    bzbh = ds.Tables[0].Rows[0]["bzbm"].ToString();  //病种编码
                    bzmc = ds.Tables[0].Rows[0]["bzmc"].ToString();     //病种名称
                }
            }
            #endregion

            #region 获取定岗医生信息
            //strSql = string.Format(@"select * from ybdgyszd where ysbm='{0}'", cfysdm);
            //ds.Tables.Clear();
            //ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            //if (ds.Tables[0].Rows.Count == 0)
            //{
            //    dgysbm = "";
            //    dgysxm = "";
            //}
            //else
            //{
            //    dgysbm = "";
            //    dgysxm = ds.Tables[0].Rows[0]["ysxm"].ToString();
            //}
            #endregion

            #region 判断结算日期时间有效性
            try
            {
                mzrq = Convert.ToDateTime(jssj).ToString("yyyyMMdd");
                mzsj = Convert.ToDateTime(jssj).ToString("HHmm");
            }
            catch
            {
                return new object[] { 0, 0, "挂号日期时间格式不正确" };
            }

           
            #endregion

            #region 疾病诊断登记
            object[] objParam3 = { jzlsh, "1", cfysdm, cfysxm, bzbh, bzmc };
            YBZDDJ(objParam3);
            #endregion

            #region 主入参信息
            /*
               1 社会保险号 VARCHAR2(20) 否
               2 社会保障卡号 VARCHAR2(20) 否
               3 姓名 VARCHAR2(20) 否
               4 中心业务流水号 VARCHAR2(20) 否 门诊号
               5 医疗类别 VARCHAR2(3) 代码（详见字典说明）
               6 门特病种编码 VARCHAR2(20) 多病种以分隔符“#”号连接
               7 门特病种名称 VARCHAR2(100) 多病种以分隔符“#”号连接
               8 定岗医师编码 VARCHAR2(20) 定岗医师\药师编号，同 DJ13
               9 定岗医师姓名 VARCHAR2(20)
               10 门诊日期 VARCHAR2(8) YYYYMMDD
               11 门诊时间 VARCHAR2(6) HH24MISS
               12 个人账户抵扣现金支付标志 VARCHAR2(3) 0-不予支付；1-支付（默认 1）（政策允许情况下才有效）
               13 是否打印结算单据 VARCHAR2(8) 作废，由接口配置文件配置项Js05Print控制
               14 生育类别 VARCHAR2(3) 编码，见“字典说明”部分。“医疗类别”为“51-生育门诊”、“52-生育住院”时，第14、15项至少输入一项。 
               15 计划生育类别 VARCHAR2(3)
               16 地区编号 VARCHAR2(6) 否 统筹区编码
            */
            StringBuilder inputData = new StringBuilder();
            inputData.Append("SSSS|");
            inputData.Append(bxh + "|");
            inputData.Append(kh + "|");
            inputData.Append(xm + "|");
            inputData.Append(ybjzlsh + "|");
            inputData.Append(yllb + "|");
            inputData.Append(bzbh + "|");
            inputData.Append(bzmc + "|");
            inputData.Append(dgysbm + "|");
            inputData.Append(dgysxm + "|");
            inputData.Append(mzrq + "|");
            inputData.Append(mzsj + "|");
            inputData.Append(zhsybz + "|");
            inputData.Append(sfdypj + "|");
            inputData.Append(sylb + "|");
            inputData.Append(jhsylb + "|");
            inputData.Append(dqbh + ";");
            #endregion

            #region 处方明细
            strSql = string.Format(@"select y.ybxmbh, y.ybxmmc, m.dj, sum(m.sl) sl, sum(m.je) je, m.yyxmbh, m.yyxmmc, m.cfh,m.gg,m.jxdw,y.jx,m.yf from 
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
                                        group by y.ybxmbh, y.ybxmmc, m.dj, m.yyxmbh, m.yyxmmc, y.sfxmzldm, y.sflbdm, m.cfh,m.gg,m.jxdw,y.jx,m.yf", jzlsh, cfhs);
            ds.Tables.Clear();
            ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            StringBuilder strMsg = new StringBuilder();
            string cfrq = DateTime.Now.ToString("yyyyMMdd");
            double sfje3 = 0.0000;
            int index = 1;
            int rowNum=ds.Tables[0].Rows.Count;
            if (ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    if (dr["ybxmbh"] == DBNull.Value)
                        strMsg.Append("项目代码：[" + dr["yyxmbh"].ToString() + "]，名称：[" + dr["yyxmmc"].ToString() + "]未对照，不能上传！");
                    else
                    {
                        string yyxmbh = dr["yyxmbh"].ToString();
                        string yyxmmc = dr["yyxmmc"].ToString();
                        string yyxmgg = dr["gg"].ToString();
                        string yyxmjx = dr["jx"].ToString();
                        string dj = dr["dj"].ToString();
                        string sl = dr["sl"].ToString();
                        string je = dr["je"].ToString();
                        string ybxmbh = dr["ybxmbh"].ToString();
                        string ybxmmc = dr["ybxmmc"].ToString();
                        string jxdw = dr["jxdw"].ToString();
                        string yf = dr["yf"].ToString();
                        sfje3 += Convert.ToDouble(je);

                        /*
                         发送的附加消息包：多记录往后续加消息包
                         1 定点医疗机构明细序号 VARCHAR2(20) 否
                         2 定点医疗机构三大目录编码 VARCHAR2(30) 否
                         3 定点医疗机构三大目录名称 VARCHAR2(150) 否
                         4 定点医疗机构三大目录规格 VARCHAR2(50)
                         5 定点医疗机构三大目录剂型 VARCHAR2(50)
                         6 单价 NUMBER(10,4) 否
                         7 数量 NUMBER(8,2) 否
                         8 金额 NUMBER(10,4) 否
                         9 社保三大目录编码 VARCHAR2(20) 否
                         10 社保三大目录名称 VARCHAR2(150) 否
                         11 剂量单位 VARCHAR2(50) 定点机构信息。2013/11/21新增
                         12 用法用量 VARCHAR2(50) 定点机构信息。2013/11/21新增
                         */
                        if (index < rowNum)
                        {
                            inputData.Append(index + "|");
                            inputData.Append(yyxmbh + "|");
                            inputData.Append(yyxmmc + "|");
                            inputData.Append("NULL|");
                            inputData.Append("NULL|");
                            inputData.Append(dj + "|");
                            inputData.Append(sl + "|");
                            inputData.Append(je + "|");
                            inputData.Append(ybxmbh + "|");
                            inputData.Append(ybxmmc + "|");
                            inputData.Append("NULL|");
                            inputData.Append("NULL;");
                        }
                        if (index >= rowNum)
                        {
                            inputData.Append(index + "|");
                            inputData.Append(yyxmbh + "|");
                            inputData.Append(yyxmmc + "|");
                            inputData.Append("NULL|");
                            inputData.Append("NULL|");
                            inputData.Append(dj + "|");
                            inputData.Append(sl + "|");
                            inputData.Append(je + "|");
                            inputData.Append(ybxmbh + "|");
                            inputData.Append(ybxmmc + "|");
                            inputData.Append("NULL|");
                            inputData.Append("NULL|");
                            inputData.Append("ZZZZ");
                        }
                        index++;

                    }
                }
                if (!string.IsNullOrEmpty(strMsg.ToString()))
                    return new object[] { 0, 0, strMsg.ToString() };
                if (Math.Abs(sfje2 - sfje3) > 1.0)
                    return new object[] { 0, 0, "收费金额与医保结算金额相差" + Math.Abs(sfje2 - sfje3) + ",无法结算，请核实!" };
            }
            else
            {
                return new object[] { 0, 0, "无收费明细" };
            }

            #endregion

            ds.Dispose();
            StringBuilder outData = new StringBuilder(102400);
            StringBuilder retMsg = new StringBuilder(102400);
            WriteLog(sysdate + "  入参|" + inputData.ToString());
            int i = f_UserBargaingApply(Ywlx, inputData, outData, retMsg);
            if (i > 0)
            {
                #region 出参
                string[] str = outData.ToString().Replace("RRRR|", "").Split(';');
                string[] str2 = str[0].Split('|');
                
                    /*出参
                    1 社会保险号 VARCHAR2(20)
                    2 社会保障卡号 VARCHAR2(20)
                    3 姓名 VARCHAR2(20)
                    4 性别 VARCHAR2(10) 名称
                    5 出生日期 VARCHAR2(8) YYYYMMDD
                    6 地区编号 VARCHAR2(6) 统筹区编码
                    7 地区名称 VARCHAR2(20)
                    8 参保日期 VARCHAR2(8) YYYYMMDD
                    9 参保身份 VARCHAR2(50) 格式“编码-名称”，如“1101-企业一般人员”。见“字典说明”
                    10 行政职务级别 VARCHAR2(50) 格式“编码-名称”，如“121-厅局级正职”。见“字典说明”
                    11 医疗待遇享受类别 VARCHAR2(50) 格式“编码-名称”，如“1101-统账在职”。见“字典说明”（仅适用于新余市）
                    12 单位名称 VARCHAR2(100) 格式“单位编码-单位名称”， 如“666666-测试单位”
                    13 单位类型 VARCHAR2(20) 格式“编码-名称”，如“100-正常企业”。见“字典说明”
                    14 中心业务流水号 VARCHAR2(20) 住院号
                    15 中心结算单据号 VARCHAR2(20)
                    16 医疗类别 VARCHAR2(50) 名称
                    17 结算费用起始日期 VARCHAR2(8)
                    18 结算费用截止日期 VARCHAR2(8)
                    19 医疗总费用 NUMBER(16,2)
                    20 本次账户支付 NUMBER(16,2)
                    21 本次现金支付 NUMBER(16,2)
                    22 职工基本医疗基金支付 NUMBER(16,2)
                    23 城镇居民基本医疗基金支付 NUMBER(16,2)
                    24 城乡居民基本医疗基金支付 NUMBER(16,2)
                    25 大病医疗基金支付 NUMBER(16,2)
                    26 公务员补充医疗基金支付 NUMBER(16,2)
                    27 企业补充医疗基金支付 NUMBER(16,2)
                    28 二乙专项医疗基金支付 NUMBER(16,2)
                    29 老红军专项医疗基金支付 NUMBER(16,2)
                    30 离休干部单独统筹基金支付 NUMBER(16,2)
                    31 医疗保健支付 NUMBER(16,2) 如新余地厅级干部医疗保健补差
                    32 其他基金支付 NUMBER(16,2) 预留指标项。（抚州市：医疗补贴金额。已计入统筹及大病基金中，此为其中项。）
                    33 工伤基金支付 NUMBER(16,2)
                    34 生育基金支付 NUMBER(16,2)
                    35 民政救助支付 NUMBER(16,2)
                    36 单位负担支付 NUMBER(16,2)
                    37 定点机构支付 NUMBER(16,2)
                    38 医保账户余额 NUMBER(16,2)
                    39 甲类费用 NUMBER(16,2)
                    40 乙类费用 NUMBER(16,2)
                    41 丙类费用 NUMBER(16,2) 抚州指“乙类*”
                    42 自费费用 NUMBER(16,2)
                    43 先行自付 NUMBER(16,2)
                    44 起付段自付 NUMBER(16,2)
                    45 转外自付 NUMBER(16,2)
                    46 统筹段自付 NUMBER(16,2)
                    47 大病统筹自付 NUMBER(16,2)
                    48 公务员补充医疗自付 NUMBER(16,2)
                    49 超过最高封顶线自付 NUMBER(16,2)
                    50 转诊单位负担 NUMBER(16,2)
                    51 统筹段单位负担 NUMBER(16,2)
                    52 大病段单位负担 NUMBER(16,2)
                    53 统筹段基金支付 NUMBER(16,2)
                    54 大病段基金支付 NUMBER(16,2)
                    55 个人账户抵扣现金支付标志 VARCHAR2(3) 0-不予支付；1-支付（默认 1）
                    56 公务员补助基本医疗支付 NUMBER(16,2) 新余：指补助 65%部分。其他：补助统筹段支付
                    57 公务员补助大病医疗支付 NUMBER(16,2) 新余：指补助 80%部分。其他：补助大病段支付
                    58 结算人 VARCHAR2(20)
                    59 结算日期 VARCHAR2(8) YYYYMMDD
                    60 结算时间 VARCHAR2(6) HH24MISS
                     * 
                    返回的附加消息包：费用明细，多记录往后续加消息包。*门诊返回，住院不返回。

                    1 社保三大目录编码 VARCHAR2(20)
                    2 社保三大目录名称 VARCHAR2(150)
                    3 收费项目等级 VARCHAR2(20) 格式“编码-名称”，如“1-甲类”。见“字典说明”
                    4 发票项目类别 VARCHAR2(20) 格式“编码-名称”，如“11-西药”。见“字典说明”
                    5 单价 NUMBER(10,4)
                    6 数量 NUMBER(8,2)
                    */
               
                outParams_js op = new outParams_js();
                op.Grbh = str2[0].Trim();       //保险号
                op.Kh = str2[1].Trim();         //卡号
                op.Xm = str2[2].Trim();         //姓名
                op.Xb = str2[3].Trim();         //性别
                op.Csrq = str2[4].Trim();       //出生日期
                op.Dqbh = str2[5].Trim();      //地区编号
                op.Dqmc = str2[6].Trim();       //地区名称
                op.Cbrq = str2[7].Trim();       //参保日期
                op.Cbsf = str2[8].Trim();       //参保身份
                op.Xzzwjb = str2[9].Trim();     //行政职务级别
                op.Yldylb = str2[10].Trim();    //医疗待遇类别
                op.Dwmc = str2[11].Trim();      //单位名称
                op.Dwlx = str2[12].Trim();      //单位类别
                op.Ybjzlsh = str2[13].Trim();   //中心业务流水号，医保就诊流水号
                op.Jslsh = str2[14].Trim();     //结算单据号
                op.Yllb = str2[15].Trim();      //医疗类别
                op.Jsqsrq = str2[16].Trim();    //结算起始日期
                op.Jsjzrq = str2[17].Trim();    //结算截止日期
                op.Ylfze = str2[18].Trim();     //医疗总费用
                op.Zhzf = str2[19].Trim();      //本次帐户支付
                op.Xjzf = str2[20].Trim();      //本次现金支付
                op.Zgjbyljjzf = str2[21].Trim();
                op.Jzjmjbyljjzf = str2[22].Trim();
                op.Cxjmjbyljjzf = str2[23].Trim();
                op.Dejjzf = str2[24].Trim();
                op.Gwybzjjzf = str2[25].Trim();
                op.Qybcylbxjjzf = str2[26].Trim();
                op.Eyzxyljjzf = str2[27].Trim();
                op.Lhjzxyljjzf = str2[28].Trim();
                op.Lxgbdttcjjzf = str2[29].Trim();
                op.Ylbjzf = str2[30].Trim();
                op.Qtjjzf = str2[31].Trim();
                op.Gsjjzf = str2[32].Trim();
                op.Syjjzf = str2[33].Trim();
                op.Mzjzfy = str2[34].Trim();
                op.Dwfdfy = str2[35].Trim();
                op.Yyfdfy = str2[36].Trim();
                op.Bcjshzhye = str2[37].Trim();
                op.Jlfy = str2[38].Trim();
                op.Ylfy = str2[39].Trim();
                op.Blfy = str2[40].Trim();
                op.Zffy = str2[41].Trim();
                op.Xxzf = str2[42].Trim();      //先行自付
                op.Qfbzfy = str2[43].Trim();
                op.Zzzyzffy = str2[44].Trim();
                op.Tcfdzffy = str2[45].Trim();
                op.Defdzffy = str2[46].Trim();
                op.Gwybcylzf = str2[47].Trim();
                op.Cgzgfd = str2[48].Trim();
                op.Zzdwfd = str2[49].Trim();
                op.Tcddwfd = str2[50].Trim();
                op.Dbddwfd = str2[51].Trim();
                op.Jrtcfy = str2[52].Trim();
                op.Jrdebxfy = str2[53].Trim();
                op.Zhdkxjzfbz = str2[54].Trim();
                op.Gwybcjbylzf = str2[55].Trim();
                op.Gwybzdbylzf = str2[56].Trim();
                op.Jbr = str2[57].Trim();
                op.Jsrq = str2[58].Trim();
                op.Jssj = str2[59].Trim();
                string jbr = CliUtils.fLoginUser;
                op.Bcjsqzhye = (Convert.ToDecimal(op.Bcjshzhye) + Convert.ToDecimal(op.Zhzf)).ToString();
                op.Zbxje = (Convert.ToDecimal(op.Ylfze) - Convert.ToDecimal(op.Xjzf)).ToString();
                op.Tcjjzf = (Convert.ToDecimal(op.Ylfze) - Convert.ToDecimal(op.Xjzf) - Convert.ToDecimal(op.Zhzf)
                            - Convert.ToDecimal(op.Dejjzf) - Convert.ToDecimal(op.Mzjzfy) - Convert.ToDecimal(op.Yyfdfy)).ToString();
                
                op.Cxjfy = "0.00";
                op.Djh = op.Jslsh;
                #endregion

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

                string strValue = op.Ylfze + "|" + op.Zbxje + "|" + op.Tcjjzf + "|" + op.Dejjzf + "|" + op.Zhzf + "|" +
                                op.Xjzf + "|" + op.Gwybzjjzf + "|" + op.Qybcylbxjjzf + "|" + op.Zffy + "|" + op.Dwfdfy + "|" +
                                op.Yyfdfy + "|" + op.Mzjzfy + "|" + op.Cxjfy + "|" + op.Ylzlfy + "|" + op.Blzlfy + "|" +
                                op.Fhjjylfy + "|" + op.Qfbzfy + "|" + op.Zzzyzffy + "|" + op.Jrtcfy + "|" + op.Tcfdzffy + "|" +
                                op.Ctcfdxfy + "|" + op.Jrdebxfy + "|" + op.Defdzffy + "|" + op.Cdefdxfy + "|" + op.Rgqgzffy + "|" +
                                op.Bcjsqzhye + "|" + op.Bntczflj + "|" + op.Bndezflj + "|" + op.Bnczjmmztczflj + "|" + op.Bngwybzzflj + "|" +
                                op.Bnzhzflj + "|" + op.Bnzycslj + "|" + op.Zycs + "|" + op.Xm + "|" + op.Jsrq + op.Jssj + "|" +
                                op.Yllb + "|" + op.Yldylb + "|" + op.Jbjgbm + "|" + op.Ywzqh + "|" + op.Jslsh + "|" +
                                op.Tsxx + "|" + op.Djh + "|" + op.Jyxl + "|" + op.Yyjylsh + "|" + op.Yxbz + "|" +
                                op.Grbhgl + "|" + op.Yljgbm + "|" + op.Ecbcje + "|" + op.Mmqflj + "|" + op.Jsfjylsh + "|" +
                                op.Grbh + "|" + op.Dbzbc + "|" + op.Czzcje + "|" + op.Elmmxezc + "|" + op.Elmmxesy + "|" +
                                op.Jmgrzfecbcje + "|" + op.Tjje + "|" + op.Syjjzf + "|";
                WriteLog(sysdate + "  " + jzlsh + " 进入门诊收费预结算成功|" + strValue);
                return new object[] { 0, 1, strValue };
            }
            else
            {
                WriteLog(sysdate + "  " + jzlsh + " 进入门诊收费预结算失败|" + retMsg.ToString());
                return new object[] { 0, 0, "门诊收费预结算失败|" + retMsg.ToString() };
            }
        }
        #endregion

        #region 门诊收费结算
        public static object[] YBMZSFJS(object[] objParam)
        {
            string sysdate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string Ywlx = "JS05";
            try
            {
                #region HIS入参
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
                string sfdypj = "TRUE";                //是否打印票据
                double sfje2 = 0.0000; //金额 
                string ybjzlsh = "";   //医保就诊流水号
                string bxh = "";
                string xm = "";
                string kh = "";
                string dgysbm = cfysdm; //定岗医生编码
                string dgysxm = cfysxm; //开方医生(定岗医生)
                string mzrq = string.Empty;
                string mzsj = string.Empty;
                string sylb = "NULL";   //生育类别
                string jhsylb = "NULL"; //计划生育类别
                string jbr = CliUtils.fLoginUser;

                string jylsh = DateTime.Now.ToString("yyyyMMddHHmmss") + "-" + new Random().Next(100).ToString().PadLeft(4, '0');

                if (string.IsNullOrEmpty(jzlsh))
                    return new object[] { 0, 0, "就诊流水号不能为空" };
                if (string.IsNullOrEmpty(djh))
                    return new object[] { 0, 0, "单据号(发票号)不能为空" };
                if (string.IsNullOrEmpty(cfhs))
                    return new object[] { 0, 0, "处方号集不能为空" };
                if (string.IsNullOrEmpty(sfje))
                    return new object[] { 0, 0, "收费金额不能为空" };
                if (string.IsNullOrEmpty(dgysxm))
                    return new object[] { 0, 0, "处方医生不能为空" };

                WriteLog(sysdate + "  " + jzlsh + " 进入门诊收费结算...");

                #region 金额有效性
                try
                {
                    sfje2 = Convert.ToDouble(sfje);
                }
                catch
                {
                    return new object[] { 0, 0, "收费金额格式错误" };
                }
                #endregion

                #region 是否门诊医保挂号
                string strSql = string.Format("select * from ybmzzydjdr where jzlsh='{0}' and cxbz=1", jzlsh);
                DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                if (ds.Tables[0].Rows.Count == 0)
                    return new object[] { 0, 0, "该患者未办理医保挂号登记" };
                else
                {
                    bxh = ds.Tables[0].Rows[0]["grbh"].ToString();
                    xm = ds.Tables[0].Rows[0]["xm"].ToString();
                    kh = ds.Tables[0].Rows[0]["kh"].ToString();
                    dqbh = ds.Tables[0].Rows[0]["tcqh"].ToString();
                    ybjzlsh = ds.Tables[0].Rows[0]["ybjzlsh"].ToString();
                    yllb = ds.Tables[0].Rows[0]["yllb"].ToString();
                    sylb = ds.Tables[0].Rows[0]["sylb"].ToString();
                    jhsylb = ds.Tables[0].Rows[0]["jhsylb"].ToString();

                    string[] syllb = { "14", "16" };
                    if (syllb.Contains(yllb))
                    {
                        bzbh = ds.Tables[0].Rows[0]["bzbm"].ToString();  //病种编码
                        bzmc = ds.Tables[0].Rows[0]["bzmc"].ToString();     //病种名称
                    }
                }
                ds.Dispose();
                #endregion

                #region 判断结算日期时间有效性
                try
                {
                    mzrq = Convert.ToDateTime(jssj).ToString("yyyyMMdd");
                    mzsj = Convert.ToDateTime(jssj).ToString("HHmm");
                }
                catch
                {
                    return new object[] { 0, 0, "挂号日期时间格式不正确" };
                }
                #endregion

                #region 主入参信息
                /*
               1 社会保险号 VARCHAR2(20) 否
               2 社会保障卡号 VARCHAR2(20) 否
               3 姓名 VARCHAR2(20) 否
               4 中心业务流水号 VARCHAR2(20) 否 门诊号
               5 医疗类别 VARCHAR2(3) 代码（详见字典说明）
               6 门特病种编码 VARCHAR2(20) 多病种以分隔符“#”号连接
               7 门特病种名称 VARCHAR2(100) 多病种以分隔符“#”号连接
               8 定岗医师编码 VARCHAR2(20) 定岗医师\药师编号，同 DJ13
               9 定岗医师姓名 VARCHAR2(20)
               10 门诊日期 VARCHAR2(8) YYYYMMDD
               11 门诊时间 VARCHAR2(6) HH24MISS
               12 个人账户抵扣现金支付标志 VARCHAR2(3) 0-不予支付；1-支付（默认 1）（政策允许情况下才有效）
               13 是否打印结算单据 VARCHAR2(8) 作废，由接口配置文件配置项Js05Print控制
               14 生育类别 VARCHAR2(3) 编码，见“字典说明”部分。“医疗类别”为“51-生育门诊”、“52-生育住院”时，第14、15项至少输入一项。 
               15 计划生育类别 VARCHAR2(3)
               16 地区编号 VARCHAR2(6) 否 统筹区编码
            */
                StringBuilder inputData = new StringBuilder();
                inputData.Clear();

                inputData.Append("SSSS|");
                inputData.Append(bxh + "|");
                inputData.Append(kh + "|");
                inputData.Append(xm + "|");
                inputData.Append(ybjzlsh + "|");
                inputData.Append(yllb + "|");
                inputData.Append(bzbh + "|");
                inputData.Append(bzmc + "|");
                inputData.Append(dgysbm + "|");
                inputData.Append(dgysxm + "|");
                inputData.Append(mzrq + "|");
                inputData.Append(mzsj + "|");
                inputData.Append(zhsybz + "|");
                inputData.Append(sfdypj + "|");
                inputData.Append(sylb + "|");
                inputData.Append(jhsylb + "|");
                inputData.Append(dqbh + ";");
                #endregion

                #region 收费明细
                strSql = string.Format(@"select y.ybxmbh, y.ybxmmc, m.dj, sum(m.sl) sl, sum(m.je) je, m.yyxmbh, m.yyxmmc, m.cfh,m.gg,m.jxdw,y.jx,m.yf from 
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
                                        group by y.ybxmbh, y.ybxmmc, m.dj, m.yyxmbh, m.yyxmmc, y.sfxmzldm, y.sflbdm, m.cfh,m.gg,m.jxdw,y.jx,m.yf", jzlsh, cfhs);
                ds.Dispose();
                ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                StringBuilder strMsg = new StringBuilder();
                string cfrq = DateTime.Now.ToString("yyyyMMdd");
                double sfje3 = 0.0000;
                List<string> li_yyxmbh = new List<string>();
                List<string> li_yyxmmc = new List<string>();
                List<string> li_ybxmbh = new List<string>();
                List<string> li_cfh = new List<string>();
                List<string> li_je = new List<string>();
                List<string> li_dj = new List<string>();

                List<string> liSQL = new List<string>();

                int index = 1;
                int rowNum = ds.Tables[0].Rows.Count;

                if (ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        if (dr["ybxmbh"] == DBNull.Value)
                        {
                            strMsg.Append("项目代码：[" + dr["yyxmbh"].ToString() + "]，名称：[" + dr["yyxmmc"].ToString() + "]未对照，不能上传！");
                        }
                        else
                        {
                            string yyxmbh = dr["yyxmbh"].ToString();
                            string yyxmmc = dr["yyxmmc"].ToString();
                            string yyxmgg = dr["gg"].ToString();
                            string yyxmjx = dr["jx"].ToString();
                            string dj = dr["dj"].ToString();
                            string sl = dr["sl"].ToString();
                            string je = dr["je"].ToString();
                            string ybxmbh = dr["ybxmbh"].ToString();
                            string ybxmmc = dr["ybxmmc"].ToString();
                            string jxdw = dr["jxdw"].ToString();
                            string yf = dr["yf"].ToString();
                            string cfh = dr["cfh"].ToString();
                            sfje3 += Convert.ToDouble(je);

                            /*
                             发送的附加消息包：多记录往后续加消息包
                             1 定点医疗机构明细序号 VARCHAR2(20) 否
                             2 定点医疗机构三大目录编码 VARCHAR2(30) 否
                             3 定点医疗机构三大目录名称 VARCHAR2(150) 否
                             4 定点医疗机构三大目录规格 VARCHAR2(50)
                             5 定点医疗机构三大目录剂型 VARCHAR2(50)
                             6 单价 NUMBER(10,4) 否
                             7 数量 NUMBER(8,2) 否
                             8 金额 NUMBER(10,4) 否
                             9 社保三大目录编码 VARCHAR2(20) 否
                             10 社保三大目录名称 VARCHAR2(150) 否
                             11 剂量单位 VARCHAR2(50) 定点机构信息。2013/11/21新增
                             12 用法用量 VARCHAR2(50) 定点机构信息。2013/11/21新增
                             */
                            if (index < rowNum)
                            {
                                inputData.Append(index + "|");
                                inputData.Append(yyxmbh + "|");
                                inputData.Append(yyxmmc + "|");
                                inputData.Append(yyxmgg + "|");
                                inputData.Append(yyxmjx + "|");
                                inputData.Append(dj + "|");
                                inputData.Append(sl + "|");
                                inputData.Append(je + "|");
                                inputData.Append(ybxmbh + "|");
                                inputData.Append(ybxmmc + "|");
                                inputData.Append(jxdw + "|");
                                inputData.Append(yf + ";");
                            }
                            else
                            {
                                inputData.Append(index + "|");
                                inputData.Append(yyxmbh + "|");
                                inputData.Append(yyxmmc + "|");
                                inputData.Append(yyxmgg + "|");
                                inputData.Append(yyxmjx + "|");
                                inputData.Append(dj + "|");
                                inputData.Append(sl + "|");
                                inputData.Append(je + "|");
                                inputData.Append(ybxmbh + "|");
                                inputData.Append(ybxmmc + "|");
                                inputData.Append(jxdw + "|");
                                inputData.Append(yf + "|");
                                inputData.Append("ZZZZ");

                            }
                            index++;
                            li_yyxmbh.Add(dr["yyxmbh"].ToString());
                            li_yyxmmc.Add(dr["yyxmmc"].ToString());
                            li_ybxmbh.Add(dr["ybxmbh"].ToString());
                            li_cfh.Add(dr["cfh"].ToString());
                            li_je.Add(je.ToString());
                            li_dj.Add(dj.ToString());

                            strSql = string.Format(@"insert into ybcfmxscindr(jzlsh,jylsh,xm,kh,ybjzlsh,cfrq,yysfxmbm,yysfxmmc,sfxmzxbm,sfxmzxmc,
                                                    dj,sl,je,jbr,sysdate,ybcfh) values(
                                                    '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}',
                                                    '{10}','{11}','{12}','{13}','{14}','{15}')",
                                                    jzlsh, jylsh, xm, kh, ybjzlsh, cfrq, yyxmbh, yyxmmc, ybxmbh, ybxmmc,
                                                    dj, sl, je, jbr, sysdate, cfh);
                            liSQL.Add(strSql);
                        }
                    }
                    if (!string.IsNullOrEmpty(strMsg.ToString()))
                        return new object[] { 0, 0, strMsg.ToString() };
                    sfje2 = Convert.ToDouble(sfje);
                    if (Math.Abs(sfje2 - sfje3) > 1.0)
                        return new object[] { 0, 0, "收费金额与医保结算金额相差" + Math.Abs(sfje2 - sfje3) + ",无法结算，请核实!" };
                }
                else
                    return new object[] { 0, 0, "无收费明细" };
                ds.Dispose();
                #endregion

                StringBuilder outData = new StringBuilder(102400);
                StringBuilder retMsg = new StringBuilder(102400);
                outData.Clear();
                retMsg.Clear();
             

                WriteLog(sysdate + "  入参|" + inputData.ToString());
                int i = f_UserBargaingApply(Ywlx, inputData, outData, retMsg);
                //WriteLog("门诊收费结算状态|" + i.ToString());
                if (i > 0)
                {
                    WriteLog(sysdate + "  出参|" + outData.ToString());
                    #region 出参
                    string[] str = outData.ToString().Replace("RRRR|","").Split(';');
                    string[] str2 = str[0].Split('|');
                    /*出参
                    1 社会保险号 VARCHAR2(20)
                    2 社会保障卡号 VARCHAR2(20)
                    3 姓名 VARCHAR2(20)
                    4 性别 VARCHAR2(10) 名称
                    5 出生日期 VARCHAR2(8) YYYYMMDD
                    6 地区编号 VARCHAR2(6) 统筹区编码
                    7 地区名称 VARCHAR2(20)
                    8 参保日期 VARCHAR2(8) YYYYMMDD
                    9 参保身份 VARCHAR2(50) 格式“编码-名称”，如“1101-企业一般人员”。见“字典说明”
                    10 行政职务级别 VARCHAR2(50) 格式“编码-名称”，如“121-厅局级正职”。见“字典说明”
                    11 医疗待遇享受类别 VARCHAR2(50) 格式“编码-名称”，如“1101-统账在职”。见“字典说明”（仅适用于新余市）
                    12 单位名称 VARCHAR2(100) 格式“单位编码-单位名称”， 如“666666-测试单位”
                    13 单位类型 VARCHAR2(20) 格式“编码-名称”，如“100-正常企业”。见“字典说明”
                    14 中心业务流水号 VARCHAR2(20) 住院号
                    15 中心结算单据号 VARCHAR2(20)
                    16 医疗类别 VARCHAR2(50) 名称
                    17 结算费用起始日期 VARCHAR2(8)
                    18 结算费用截止日期 VARCHAR2(8)
                    19 医疗总费用 NUMBER(16,2)
                    20 本次账户支付 NUMBER(16,2)
                    21 本次现金支付 NUMBER(16,2)
                    22 职工基本医疗基金支付 NUMBER(16,2)
                    23 城镇居民基本医疗基金支付 NUMBER(16,2)
                    24 城乡居民基本医疗基金支付 NUMBER(16,2)
                    25 大病医疗基金支付 NUMBER(16,2)
                    26 公务员补充医疗基金支付 NUMBER(16,2)
                    27 企业补充医疗基金支付 NUMBER(16,2)
                    28 二乙专项医疗基金支付 NUMBER(16,2)
                    29 老红军专项医疗基金支付 NUMBER(16,2)
                    30 离休干部单独统筹基金支付 NUMBER(16,2)
                    31 医疗保健支付 NUMBER(16,2) 如新余地厅级干部医疗保健补差
                    32 其他基金支付 NUMBER(16,2) 预留指标项。（抚州市：医疗补贴金额。已计入统筹及大病基金中，此为其中项。）
                    33 工伤基金支付 NUMBER(16,2)
                    34 生育基金支付 NUMBER(16,2)
                    35 民政救助支付 NUMBER(16,2)
                    36 单位负担支付 NUMBER(16,2)
                    37 定点机构支付 NUMBER(16,2)
                    38 医保账户余额 NUMBER(16,2)
                    39 甲类费用 NUMBER(16,2)
                    40 乙类费用 NUMBER(16,2)
                    41 丙类费用 NUMBER(16,2) 抚州指“乙类*”
                    42 自费费用 NUMBER(16,2)
                    43 先行自付 NUMBER(16,2)
                    44 起付段自付 NUMBER(16,2)
                    45 转外自付 NUMBER(16,2)
                    46 统筹段自付 NUMBER(16,2)
                    47 大病统筹自付 NUMBER(16,2)
                    48 公务员补充医疗自付 NUMBER(16,2)
                    49 超过最高封顶线自付 NUMBER(16,2)
                    50 转诊单位负担 NUMBER(16,2)
                    51 统筹段单位负担 NUMBER(16,2)
                    52 大病段单位负担 NUMBER(16,2)
                    53 统筹段基金支付 NUMBER(16,2)
                    54 大病段基金支付 NUMBER(16,2)
                    55 个人账户抵扣现金支付标志 VARCHAR2(3) 0-不予支付；1-支付（默认 1）
                    56 公务员补助基本医疗支付 NUMBER(16,2) 新余：指补助 65%部分。其他：补助统筹段支付
                    57 公务员补助大病医疗支付 NUMBER(16,2) 新余：指补助 80%部分。其他：补助大病段支付
                    58 结算人 VARCHAR2(20)
                    59 结算日期 VARCHAR2(8) YYYYMMDD
                    60 结算时间 VARCHAR2(6) HH24MISS
                     * 
                    返回的附加消息包：费用明细，多记录往后续加消息包。*门诊返回，住院不返回。

                    1 社保三大目录编码 VARCHAR2(20)
                    2 社保三大目录名称 VARCHAR2(150)
                    3 收费项目等级 VARCHAR2(20) 格式“编码-名称”，如“1-甲类”。见“字典说明”
                    4 发票项目类别 VARCHAR2(20) 格式“编码-名称”，如“11-西药”。见“字典说明”
                    5 单价 NUMBER(10,4)
                    6 数量 NUMBER(8,2)
                    */

                    outParams_js op = new outParams_js();
                    op.Grbh = str2[0].Trim();       //保险号
                    op.Kh = str2[1].Trim();         //卡号
                    op.Xm = str2[2].Trim();         //姓名
                    op.Xb = str2[3].Trim();         //性别
                    op.Csrq = str2[4].Trim();       //出生日期
                    op.Dqbh = str2[5].Trim();      //地区编号
                    op.Dqmc = str2[6].Trim();       //地区名称
                    op.Cbrq = str2[7].Trim();       //参保日期
                    op.Cbsf = str2[8].Trim();       //参保身份
                    op.Xzzwjb = str2[9].Trim();     //行政职务级别
                    op.Yldylb = str2[10].Trim();    //医疗待遇类别
                    op.Dwmc = str2[11].Trim();      //单位名称
                    op.Dwlx = str2[12].Trim();      //单位类别
                    op.Ybjzlsh = str2[13].Trim();   //中心业务流水号，医保就诊流水号
                    op.Jslsh = str2[14].Trim();     //结算单据号
                    op.Yllb = str2[15].Trim();      //医疗类别
                    op.Jsqsrq = str2[16].Trim();    //结算起始日期
                    op.Jsjzrq = str2[17].Trim();    //结算截止日期
                    op.Ylfze = str2[18].Trim();     //医疗总费用
                    op.Zhzf = str2[19].Trim();      //本次帐户支付
                    op.Xjzf = str2[20].Trim();      //本次现金支付
                    op.Zgjbyljjzf = str2[21].Trim();
                    op.Jzjmjbyljjzf = str2[22].Trim();
                    op.Cxjmjbyljjzf = str2[23].Trim();
                    op.Dejjzf = str2[24].Trim();
                    op.Gwybzjjzf = str2[25].Trim();
                    op.Qybcylbxjjzf = str2[26].Trim();
                    op.Eyzxyljjzf = str2[27].Trim();
                    op.Lhjzxyljjzf = str2[28].Trim();
                    op.Lxgbdttcjjzf = str2[29].Trim();
                    op.Ylbjzf = str2[30].Trim();
                    op.Qtjjzf = str2[31].Trim();
                    op.Gsjjzf = str2[32].Trim();
                    op.Syjjzf = str2[33].Trim();
                    op.Mzjzfy = str2[34].Trim();
                    op.Dwfdfy = str2[35].Trim();
                    op.Yyfdfy = str2[36].Trim();
                    op.Bcjshzhye = str2[37].Trim();
                    op.Jlfy = str2[38].Trim();
                    op.Ylfy = str2[39].Trim();
                    op.Blfy = str2[40].Trim();
                    op.Zffy = str2[41].Trim();
                    op.Xxzf = str2[42].Trim();      //先行自付
                    op.Qfbzfy = str2[43].Trim();
                    op.Zzzyzffy = str2[44].Trim();
                    op.Tcfdzffy = str2[45].Trim();
                    op.Defdzffy = str2[46].Trim();
                    op.Gwybcylzf = str2[47].Trim();
                    op.Cgzgfd = str2[48].Trim();
                    op.Zzdwfd = str2[49].Trim();
                    op.Tcddwfd = str2[50].Trim();
                    op.Dbddwfd = str2[51].Trim();
                    op.Jrtcfy = str2[52].Trim();
                    op.Jrdebxfy = str2[53].Trim();
                    op.Zhdkxjzfbz = str2[54].Trim();
                    op.Gwybcjbylzf = str2[55].Trim();
                    op.Gwybzdbylzf = str2[56].Trim();
                    op.Jbr = str2[57].Trim();
                    op.Jsrq = str2[58].Trim();
                    op.Jssj = str2[59].Trim();
                    op.Bcjsqzhye = (Convert.ToDecimal(op.Bcjshzhye) + Convert.ToDecimal(op.Zhzf)).ToString();
                    op.Zbxje = (Convert.ToDecimal(op.Ylfze) - Convert.ToDecimal(op.Xjzf)).ToString();
                    op.Tcjjzf = (Convert.ToDecimal(op.Ylfze) - Convert.ToDecimal(op.Xjzf) - Convert.ToDecimal(op.Zhzf)
                                - Convert.ToDecimal(op.Dejjzf) - Convert.ToDecimal(op.Mzjzfy) - Convert.ToDecimal(op.Yyfdfy)).ToString();

                    op.Cxjfy = "0.00";
                    op.Djh = op.Jslsh;
                    #endregion
                    /*
                     * 医疗费总额|总报销金额|统筹基金支付|大额基金支付|账户支付|
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
                    string strValue = op.Ylfze + "|" + op.Zbxje + "|" + op.Tcjjzf + "|" + op.Dejjzf + "|" + op.Zhzf + "|" +
                                op.Xjzf + "|" + op.Gwybzjjzf + "|" + op.Qybcylbxjjzf + "|" + op.Zffy + "|" + op.Dwfdfy + "|" +
                                op.Yyfdfy + "|" + op.Mzjzfy + "|" + op.Cxjfy + "|" + op.Ylzlfy + "|" + op.Blzlfy + "|" +
                                op.Fhjjylfy + "|" + op.Qfbzfy + "|" + op.Zzzyzffy + "|" + op.Jrtcfy + "|" + op.Tcfdzffy + "|" +
                                op.Ctcfdxfy + "|" + op.Jrdebxfy + "|" + op.Defdzffy + "|" + op.Cdefdxfy + "|" + op.Rgqgzffy + "|" +
                                op.Bcjsqzhye + "|" + op.Bntczflj + "|" + op.Bndezflj + "|" + op.Bnczjmmztczflj + "|" + op.Bngwybzzflj + "|" +
                                op.Bnzhzflj + "|" + op.Bnzycslj + "|" + op.Zycs + "|" + op.Xm + "|" + op.Jsrq + op.Jssj + "|" +
                                op.Yllb + "|" + op.Yldylb + "|" + op.Jbjgbm + "|" + op.Ywzqh + "|" + op.Jslsh + "|" +
                                op.Tsxx + "|" + op.Djh + "|" + op.Jyxl + "|" + op.Yyjylsh + "|" + op.Yxbz + "|" +
                                op.Grbhgl + "|" + op.Yljgbm + "|" + op.Ecbcje + "|" + op.Mmqflj + "|" + op.Jsfjylsh + "|" +
                                op.Grbh + "|" + op.Dbzbc + "|" + op.Czzcje + "|" + op.Elmmxezc + "|" + op.Elmmxesy + "|" +
                                op.Jmgrzfecbcje + "|" + op.Tjje + "|" + op.Syjjzf + "|";



                    strSql = string.Format(@"insert into ybfyjsdr(
                                                jzlsh,djhin,yllb,bzbm,bzmc,grbh,kh,xm,ybjzlsh,djh,
                                                jslsh,xzzwjb,dqbh,dqmc,cbsf,yldyxslb,dwmc,dwlx,cbrq,jsqsrq,
                                                jsjzrq,ylfze,zbxje,tcjjzf,zhzf,xjzf,dejjzf,gwybzjjzf,qybcylbxjjzf,lxgbddtczf,
                                                mzjzfy,dwfdfy,yyfdfy,bcjsqzhye,jlfy,ylfy,blfy,zffy,qfbzfy,zgjbyljjzf,
                                                czjmjbyljjzf,cxjmjjyljjzf,eyzxyljjzf,lhjzxyljjzf,ylbjzf,qtjjzf,gsjjzf,syjjzf,xxzf,zzzyzffy,
                                                jrtczf,jrdbzf,gwybcylzf,cxjfy,zzdwfd,tcddwfd,jrtcfy,jrdebxfy,zhzfbz,gwybzjbylzf,
                                                gwybzdbylzf,jsrq,jbr,sysdate,jylsh) values(
                                                '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}',
                                                '{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}',
                                                '{20}','{21}','{22}','{23}','{24}','{25}','{26}','{27}','{28}','{29}',
                                                '{30}','{31}','{32}','{33}','{34}','{35}','{36}','{37}','{38}','{39}',
                                                '{40}','{41}','{42}','{43}','{44}','{45}','{46}','{47}','{48}','{49}',
                                                '{50}','{51}','{52}','{53}','{54}','{55}','{56}','{57}','{58}','{59}',
                                                '{60}','{61}','{62}','{63}','{64}')",
                                               jzlsh, djh, op.Yllb, bzbh, bzmc, op.Grbh, op.Kh, op.Xm, op.Ybjzlsh, djh,
                                               op.Jslsh, op.Xzzwjb, op.Dqbh, op.Dqmc, op.Cbsf, op.Yldylb, op.Dwmc, op.Dwlx, op.Cbrq, op.Jsqsrq,
                                               op.Jsjzrq, op.Ylfze, op.Zbxje, op.Tcjjzf, op.Zhzf, op.Xjzf, op.Dejjzf, op.Gwybzjjzf, op.Qybcylbxjjzf, op.Lxgbdttcjjzf,
                                               op.Mzjzfy, op.Dwfdfy, op.Yyfdfy, op.Bcjsqzhye, op.Jlfy, op.Ylfy, op.Blfy, op.Zffy, op.Qfbzfy, op.Zgjbyljjzf,
                                               op.Jzjmjbyljjzf, op.Cxjmjbyljjzf, op.Eyzxyljjzf, op.Lhjzxyljjzf, op.Ylbjzf, op.Qtjjzf, op.Gsjjzf, op.Syjjzf, op.Xxzf, op.Zzzyzffy,
                                               op.Tcfdzffy, op.Defdzffy, op.Gwybcylzf, op.Cxjfy, op.Zzdwfd, op.Tcddwfd, op.Jrtcfy, op.Jrdebxfy, op.Zhdkxjzfbz, op.Gwybcjbylzf,
                                               op.Gwybzdbylzf, op.Jsrq + op.Jssj, jbr, sysdate, jylsh);
                    liSQL.Add(strSql);
                    /*
                   *  1社保三大目录编码 VARCHAR2(20)
                      2社保三大目录名称 VARCHAR2(150)
                      3 收费项目等级 VARCHAR2(20) 格式“编码-名称”，如“1-甲类”。见“字典说明”
                      4 发票项目类别 VARCHAR2(20) 格式“编码-名称”，如“11-西药”。见“字典说明”
                      5 单价 NUMBER(10,4)
                      6 数量 NUMBER(8,2) 
                      7 金额 NUMBER(10,4)
                      8 自付金额 NUMBER(10,4)
                      9 限价标准 NUMBER(10,4)
                      10 超限价自费金额 NUMBER(10,4)
                      11 限制使用标志 VARCHAR2(3) 是/否
                   */
                    for (int j = 1; j < str.Length; j++)
                    {
                        str2 = str[j].Split('|');
                        outParams_fymx opfy = new outParams_fymx();
                        opfy.Ybxmbm = str2[0].Trim();
                        opfy.Ybxmmc = str2[1].Trim();
                        opfy.Sfxmdj = str2[2].Trim();
                        opfy.Sfxmlb = str2[3].Trim();
                        opfy.Dj = str2[4].Trim();
                        opfy.Sl = str2[5].Trim();
                        opfy.Je = str2[6].Trim();
                        opfy.Zfje = str2[7].Trim();
                        opfy.Xjbz = str2[8].Trim();
                        opfy.Cxjzfje = str2[9].Trim();
                        opfy.Xzsybz = str2[10].Trim();
                        for (int k = 0; k < li_ybxmbh.Count; k++)
                        {
                            if (opfy.Ybxmbm.Equals(li_ybxmbh[k]))
                            {
                                strSql = string.Format(@"insert into ybcfmxscfhdr(jzlsh,jylsh,ybjzlsh,xm,kh,yybxmbh,ybxmmc,yyxmdm,yyxmmc,sfxmdj,
                                                        sflb,dj,sl,je,zfje,zlje,cxjzfje,bz,ybcfh,cfh,
                                                        grbh,sysdate) values(
                                                        '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}',
                                                        '{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}',
                                                        '{20}','{21}')",
                                                        jzlsh, jylsh, op.Ybjzlsh, op.Xm, op.Kh, li_yyxmbh[k], li_yyxmmc[k], opfy.Ybxmbm, opfy.Ybxmmc, opfy.Sfxmdj,
                                                        opfy.Sfxmlb, opfy.Dj, opfy.Sl, opfy.Je, opfy.Zfje, opfy.Xjbz, opfy.Cxjzfje, opfy.Xzsybz, li_cfh[k], li_cfh[k],
                                                        op.Grbh, sysdate);
                                liSQL.Add(strSql);
                                break;
                            }
                        }
                    }
                    object[] obj = liSQL.ToArray();
                    obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);
                    if (obj[1].ToString().Equals("1"))
                    {
                        WriteLog(sysdate + "  " + jzlsh + " 进入门诊收费结算成功|" + strValue);
                        return new object[] { 0, 1, strValue };
                    }
                    else
                    {
                        WriteLog(sysdate + "  " + jzlsh + " 进入门诊收费结算成功|操作本地数据失败|" + obj[2].ToString());
                        //撤销门诊费用结算
                        object[] objParam2 = new object[] { jzlsh, op.Grbh, op.Kh, op.Xm, op.Ybjzlsh, op.Jslsh, op.Dqbh };
                        NYBMZSFJSCX(objParam2);
                        return new object[] { 0, 0, "门诊收费结算失败|本地数据操作失败" };
                    }
                }
                else
                {
                    WriteLog(sysdate + "  " + jzlsh + " 进入门诊收费结算失败|" + retMsg.ToString());
                    return new object[] { 0, 0, "门诊收费结算失败|" + retMsg.ToString() };
                }
            }
            catch (Exception ex)
            {
                return new object[] { 0, 0, "门诊收费结算异常" + ex.Message };
            }
        }
        #endregion

        #region 门诊收费结算撤销
        public static object[] YBMZSFJSCX(object[] objParam)
        {
            string sysdate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string Ywlx = "JS06";
            string jzlsh = objParam[0].ToString();  //就诊流水号
            string djh = objParam[1].ToString();    //发票号
            string bxh = string.Empty;
            string xm = string.Empty;
            string kh = string.Empty;
            string dqbh = string.Empty;
            string djlsh = string.Empty;    //医保反馈单据流水号 
            string ybjzlsh = "";
            string jbr = CliUtils.fLoginUser;
            string jylsh = "";
            if (string.IsNullOrEmpty(jzlsh))
                return new object[] { 0, 0, "就诊流水号不能为空" };

            #region 是否门诊挂号
            string strSql = string.Format("select * from ybmzzydjdr where jzlsh='{0}' and cxbz=1", jzlsh);
            DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            if (ds.Tables[0].Rows.Count == 0)
                return new object[] { 0, 0, "该患者未办理挂号登记" };
            else
            {
                bxh = ds.Tables[0].Rows[0]["grbh"].ToString();
                xm = ds.Tables[0].Rows[0]["xm"].ToString();
                kh = ds.Tables[0].Rows[0]["kh"].ToString();
                dqbh = ds.Tables[0].Rows[0]["tcqh"].ToString();
                ybjzlsh = ds.Tables[0].Rows[0]["ybjzlsh"].ToString();
            }
            #endregion
            #region 是否结算
            strSql = string.Format("select * from ybfyjsdr where jzlsh='{0}' and cxbz=1 and djhin='{1}'", jzlsh, djh);
            ds.Tables.Clear();
            ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            if (ds.Tables[0].Rows.Count == 0)
                return new object[] { 0, 0, "该患者无结算信息" };
            else
            {
                djlsh = ds.Tables[0].Rows[0]["jslsh"].ToString();
                jylsh = ds.Tables[0].Rows[0]["jylsh"].ToString();
            }
            #endregion

            StringBuilder inputData = new StringBuilder();
            /*
             * 1 社会保险号 VARCHAR2(20) 否
             * 2 社会保障卡号 VARCHAR2(20) 否
             * 3 姓名 VARCHAR2(20) 否
             * 4 中心业务流水号 VARCHAR2(20) 否 门诊号
             * 5 中心结算单据号 VARCHAR2(20) 否 JS05交易返回的第15项信息
             * 6 地区编号 VARCHAR2(6) 否 统筹区编码
             */
            inputData.Append("SSSS|");
            inputData.Append(bxh + "|");
            inputData.Append(kh + "|");
            inputData.Append(xm + "|");
            inputData.Append(ybjzlsh + "|");
            inputData.Append(djlsh+ "|");
            inputData.Append(dqbh+"|");
            inputData.Append("ZZZZ|");
            StringBuilder outData = new StringBuilder(1024);
            StringBuilder retMsg = new StringBuilder(1024);

            WriteLog(sysdate + "  " + jzlsh + " 进入门诊收费撤销...");
            WriteLog(sysdate + "  入参|" + inputData.ToString());
            int i = f_UserBargaingApply(Ywlx, inputData, outData, retMsg);
            if (i > 0)
            {
                List<string> liSQL = new List<string>();
                strSql = string.Format(@"insert into ybfyjsdr(jzlsh,djhin,yllb,bzbm,bzmc,grbh,kh,xm,ybjzlsh,djh,
                                        jslsh,xzzwjb,dqbh,dqmc,cbsf,yldyxslb,dwmc,dwlx,cbrq,jsqsrq,
                                        jsjzrq,ylfze,zbxje,tcjjzf,zhzf,xjzf,dejjzf,gwybzjjzf,qybcylbxjjzf,lxgbddtczf,
                                        mzjzfy,dwfdfy,yyfdfy,bcjsqzhye,jlfy,ylfy,blfy,zffy,qfbzfy,zgjbyljjzf,
                                        czjmjbyljjzf,cxjmjjyljjzf,eyzxyljjzf,lhjzxyljjzf,ylbjzf,qtjjzf,gsjjzf,syjjzf,xxzf,zzzyzffy,
                                        jrtczf,jrdbzf,gwybcylzf,cxjfy,zzdwfd,tcddwfd,jrtcfy,jrdebxfy,zhzfbz,gwybzjbylzf,
                                        gwybzdbylzf,jsrq,jylsh,jbr,sysdate,cxbz) select 
                                        jzlsh,djhin,yllb,bzbm,bzmc,grbh,kh,xm,ybjzlsh,djh,
                                        jslsh,xzzwjb,dqbh,dqmc,cbsf,yldyxslb,dwmc,dwlx,cbrq,jsqsrq,
                                        jsjzrq,ylfze,zbxje,tcjjzf,zhzf,xjzf,dejjzf,gwybzjjzf,qybcylbxjjzf,lxgbddtczf,
                                        mzjzfy,dwfdfy,yyfdfy,bcjsqzhye,jlfy,ylfy,blfy,zffy,qfbzfy,zgjbyljjzf,
                                        czjmjbyljjzf,cxjmjjyljjzf,eyzxyljjzf,lhjzxyljjzf,ylbjzf,qtjjzf,gsjjzf,syjjzf,xxzf,zzzyzffy,
                                        jrtczf,jrdbzf,gwybcylzf,cxjfy,zzdwfd,tcddwfd,jrtcfy,jrdebxfy,zhzfbz,gwybzjbylzf,
                                        gwybzdbylzf,jsrq,jylsh,'{3}','{2}',0 from ybfyjsdr where jslsh='{0}' and jzlsh='{1}' and cxbz=1", djlsh, jzlsh, sysdate, jbr);
                liSQL.Add(strSql);
                strSql = string.Format("update ybfyjsdr set cxbz=2 where jslsh='{0}' and jzlsh='{1}' and cxbz=1", djlsh, jzlsh);
                liSQL.Add(strSql);
                strSql = string.Format(@"insert into ybcfmxscindr(jzlsh,jylsh,xm,kh,ybjzlsh,cfrq,yysfxmbm,yysfxmmc,sfxmzxbm,sfxmzxmc,
                                        dj,sl,je,ybcfh,jbr,sysdate,cxbz) select
                                        jzlsh,jylsh,xm,kh,ybjzlsh,cfrq,yysfxmbm,yysfxmmc,sfxmzxbm,sfxmzxmc,
                                        dj,sl,je,ybcfh,jbr,'{2}',0 
                                        from ybcfmxscindr where jylsh = '{0}' and jzlsh='{1}' and cxbz=1", jylsh, jzlsh, sysdate);
                liSQL.Add(strSql);
                strSql = string.Format("update ybcfmxscindr set cxbz = 2 where jylsh = '{0}' and cxbz = 1 ", jylsh);
                liSQL.Add(strSql);

                strSql = string.Format(@"insert into ybcfmxscfhdr(jzlsh,jylsh,ybjzlsh,xm,kh,yybxmbh,ybxmmc,yyxmdm,yyxmmc,sfxmdj,
                                        sflb,dj,sl,je,zfje,zlje,cxjzfje,bz,ybcfh,cfh,grbh,
                                        sysdate,cxbz) select
                                        jzlsh,jylsh,ybjzlsh,xm,kh,yybxmbh,ybxmmc,yyxmdm,yyxmmc,sfxmdj,
                                        sflb,dj,sl,je,zfje,zlje,cxjzfje,bz,ybcfh,cfh,grbh,
                                        '{2}',0 
                                        from ybcfmxscfhdr where jylsh = '{0}' and jzlsh='{1}' and cxbz=1", jylsh, jzlsh, sysdate);
                liSQL.Add(strSql);
                strSql = string.Format("update ybcfmxscfhdr set cxbz = 2 where jylsh = '{0}' and cxbz = 1 ", jylsh);
                liSQL.Add(strSql);
                object[] obj = liSQL.ToArray();
                obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);
                if (obj[1].ToString().Equals("1"))
                {
                    WriteLog(sysdate + "  " + jzlsh + " 进入门诊收费撤销成功|" + outData.ToString());
                    return new object[] { 0, 1, outData.ToString() };
                }
                else
                {
                    WriteLog(sysdate + "  " + jzlsh + " 进入门诊收费撤销成功|操作本地数据失败|" + obj[2].ToString());
                    return new object[] { 0, 2, "门诊收费撤销失败|" + obj[2].ToString() };
                }
            }
            else
            {
                WriteLog(sysdate + "  " + jzlsh + " 进入门诊收费撤销失败|" + retMsg.ToString());
                return new object[] { 0, 0, "门诊收费撤销失败|" + retMsg.ToString() };
            }
        }
        #endregion

        #region 门诊收费结算单
        public static object[] YBMZJSD(object[] objParam)
        {
            string sysdate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string jzlsh = objParam[0].ToString(); //就诊流水号
            string djh = objParam[1].ToString();  //单据号/发票号

            string Ywlx = "JS13";
            string grbh = "";
            string kh = "";
            string xm = "";
            string ybjzlsh = "";
            string tcqh = "";
            string jslsh = "";


            if (string.IsNullOrEmpty(jzlsh))
                return new object[] { 0, 0, "就诊流水号不能为空" };

            string strSql = string.Format(@"select * from ybfyjsdr where jzlsh='{0}' and djh='{1}' and cxbz=1", jzlsh, djh);
            DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            if (ds.Tables[0].Rows.Count == 0)
                return new object[] { 0, 0, "该患者未做出院结算" };
            jslsh = ds.Tables[0].Rows[0]["jslsh"].ToString();

            strSql = string.Format(@"select xm,kh,grbh,ybjzlsh,tcqh from ybmzzydjdr where cxbz=1 and jzlsh='{0}'", jzlsh);
            ds.Tables[0].Clear();
            ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            if (ds.Tables[0].Rows.Count == 0)
                return new object[] { 0, 0, "该患者未医保登记" };
            grbh = ds.Tables[0].Rows[0]["grbh"].ToString();
            kh = ds.Tables[0].Rows[0]["kh"].ToString();
            xm = ds.Tables[0].Rows[0]["xm"].ToString();
            ybjzlsh = ds.Tables[0].Rows[0]["ybjzlsh"].ToString();
            tcqh = ds.Tables[0].Rows[0]["tcqh"].ToString();
            ds.Dispose();

            StringBuilder inputData = new StringBuilder();
            inputData.Append("SSSS|");
            inputData.Append(grbh + "|");
            inputData.Append(kh + "|");
            inputData.Append(xm + "|");
            inputData.Append(jslsh + "|");
            inputData.Append(tcqh + "|");
            inputData.Append("ZZZZ");

            StringBuilder outData = new StringBuilder(10240);
            StringBuilder retMsg = new StringBuilder(1024);

            WriteLog(sysdate + "  " + jzlsh + " 进入住院补打结算单...");
            WriteLog(sysdate + " 入参|" + inputData.ToString());
            int i = f_UserBargaingApply(Ywlx, inputData, outData, retMsg);
            if (i > 0)
            {
                WriteLog(sysdate + "  住院补打结算单成功");
                return new object[] { 0, 1, "住院补打结算单成功" };
            }
            else
            {
                WriteLog(sysdate + "  " + jzlsh + " 住院补打结算单失败" + retMsg.ToString());
                return new object[] { 0, 0, "住院补打结算单失败|" + retMsg.ToString() };
            }
        }
        #endregion

        #region 住院登记
        public static object[] YBZYDJ(object[] objParam)
        {
            string sysdate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string Ywlx = "DJ03";
            string jzlsh = objParam[0].ToString(); //就诊流水号
            string yllb = objParam[1].ToString(); //医疗类别代码
            string bzbm = objParam[2].ToString(); //病种编码
            string bzmc = objParam[3].ToString(); //病种名称
            string[] kxx = objParam[4].ToString().Split('|'); //读卡返回信息
            #region 读卡信息
            if (kxx.Length < 2)
                return new object[] { 0, 0, "无读卡信息反馈" };

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
            string cbdwlx = kxx[27].ToString(); //参保单位类型
            string jbjgbm = kxx[28].ToString(); //经办机构编码
            #endregion
            string lyjgdm = objParam[5].ToString();//来源机构代码
            string lyjgmc = objParam[6].ToString();//来源机构名称
            string yllbmc = objParam[7].ToString();//医疗类别名称
            string dgysdm = objParam[8].ToString(); //定岗医生代码
            string dgysxm = objParam[9].ToString(); //定岗医生姓名
            string zszbm = objParam[10].ToString(); //准生证编号
            string sylb = objParam[11].ToString();      //生育类别
            string jhsylb = objParam[12].ToString();    //计划生育类别
            

            string jbr = CliUtils.fUserName; //经办人
            string bxh = grbh;    //保险号
            string bzbm_r = string.Empty;
            string bzmc_r = string.Empty;
            string bqms = "NULL"; //病情描述

            string ksbh = "";   //科室编号
            string ksmc = "";   //科室名称
            string zych = "";   //住院床号
            string zyrq = "";
            string zyrq1 = "";
            string zysj = "";
            string hznm = "";
            string ybksdm = "";
            string ybksmc = "";
            string jylsh = jzlsh + DateTime.Now.ToString("yyyyMMdd");
            string jbr2 = CliUtils.fLoginUser;

            if (string.IsNullOrEmpty(yllb))
                return new object[] { 0, 0, "医疗类别不能为空" };
            if (string.IsNullOrEmpty(jzlsh))
                return new object[] { 0, 0, "就诊流水号不能为空" };
            if (string.IsNullOrEmpty(dgysdm) || string.IsNullOrEmpty(dgysxm))
                return new object[] { 0, 0, "医生不能为空" };

            #region 是否办理住院

            string strSql = string.Format(@"select a.z1date as ryrq,z1hznm,a.z1ksno,a.z1ksnm,'' as z1bedn,b.ybksdm,b.ybksmc from zy01h a 
                                            left join ybkszd b on a.z1ksno=b.ksdm
                                            where a.z1zyno='{0}'", jzlsh);
            DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            if (ds.Tables[0].Rows.Count == 0)
                return new object[] { 0, 0, "该患者未办理入院登记" };
            else
            {
                zyrq = ds.Tables[0].Rows[0]["ryrq"].ToString();
                hznm = ds.Tables[0].Rows[0]["z1hznm"].ToString();
                ksbh = ds.Tables[0].Rows[0]["z1ksno"].ToString();
                ksmc = ds.Tables[0].Rows[0]["z1ksnm"].ToString();
                zych = ds.Tables[0].Rows[0]["z1bedn"].ToString();
                ybksdm = ds.Tables[0].Rows[0]["ybksdm"].ToString();
                ybksmc = ds.Tables[0].Rows[0]["ybksmc"].ToString();
            }
            #endregion
            #region 时间有效性
            try
            {
                DateTime dt_zy = Convert.ToDateTime(zyrq);
                zyrq1 = dt_zy.ToString("yyyyMMdd");
                zysj = dt_zy.ToString("HHmm");
            }
            catch
            {
                return new object[] { 0, 0, "住院日期格式错误" };
            }
            #endregion
            #region 判断是否已进行住院登记
            strSql = string.Format("select * from ybmzzydjdr where jzlsh='{0}' and cxbz=1", jzlsh);
            ds.Tables.Clear();
            ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            if (ds.Tables[0].Rows.Count > 0)
                return new object[] { 0, 0, "该患者已办理医保登记" };
            ds.Dispose();
            #endregion
            #region 判断姓名是否一致
            strSql = string.Format(@"select XM,SJNL from YBICKXX where grbh='{0}' ", bxh);
            ds.Tables.Clear();
            ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            if (ds.Tables[0].Rows.Count == 0)
                return new object[] { 0, 0, "读卡数据有误" };
            string kxm = ds.Tables[0].Rows[0]["xm"].ToString();
            int ksjnl = int.Parse(ds.Tables[0].Rows[0]["SJNL"].ToString());
            if (ksjnl >= 1)
            {
                if (!hznm.Equals(xm))
                    return new object[] { 0, 0, "医保卡与患者姓名不一致" };
            }

            #endregion
            StringBuilder inputData = new StringBuilder();
            /*
             1 社会保险号 VARCHAR2(20) 否
            2 社会保障卡号 VARCHAR2(20) 否
            3 姓名 VARCHAR2(20) 否
            4 医疗类别 VARCHAR2(3) 否 代码（详见字典说明）
            5 科室代码 VARCHAR2(20) 代码（详见字典说明）
            6 科室名称 VARCHAR2(20)
            7 入院日期 VARCHAR2(8) 否 YYYYMMDD
            8 入院时间 VARCHAR2(6) 否 HH24MISS
            9 住院床号 VARCHAR2(20)
            10 定点机构业务流水号 VARCHAR2(20) 否 住院号
            11 主治医师编码 VARCHAR2(20) 定岗医师\药师编号，同DJ13
            12 主治医师姓名 VARCHAR2(20)
            13 主要病情描述 VARCHAR2(200)
            14 准生证编号 VARCHAR2(30) 生育保险专用
            15 地区编号 VARCHAR2(6) 否 统筹区编码，同CX01返回第7项
             */
            inputData.Append("SSSS|");
            inputData.Append(bxh + "|");
            inputData.Append(kh + "|");
            inputData.Append(xm + "|");
            inputData.Append(yllb + "|");
            inputData.Append(ybksdm + "|");
            inputData.Append(ybksmc + "|");
            inputData.Append(zyrq1 + "|");
            inputData.Append(zysj + "|");
            inputData.Append("NULL|");
            inputData.Append(jzlsh + "|");
            inputData.Append(dgysdm + "|");
            inputData.Append(dgysxm + "|");
            inputData.Append(bzmc + "|");
            inputData.Append(zszbm + "|");
            inputData.Append(dqbh+"|");
            inputData.Append("ZZZZ");
            StringBuilder outData = new StringBuilder(10240);
            StringBuilder retMsg = new StringBuilder(10240);

            WriteLog(sysdate + "  " + jzlsh + " 进入住院医保登记...");
            WriteLog(sysdate + "  入参|" + inputData.ToString());
            int i = f_UserBargaingApply(Ywlx, inputData, outData, retMsg);
            if (i > 0)
            {
                WriteLog(sysdate+"  出参|"+outData.ToString());
                List<string> liSQL = new List<string>();
                #region 返回参数
                /*出参:
                1 社会保险号 VARCHAR2(20) 医院实时结算平台接口规范
                2 社会保障卡号 VARCHAR2(20)
                3 姓名 VARCHAR2(20)
                4 地区编号 VARCHAR2(6) 统筹区编码
                5 地区名称 VARCHAR2(50)
                6 医保账户余额 NUMBER(8,2)
                7 中心业务流水号 VARCHAR2(20) 门诊号
                8 医疗类别 VARCHAR2(50) 名称
                9 本次住院次数 NUMBER(3) 住院业务有效
                10 出生日期 VARCHAR2(8)
                11 年龄 VARCHAR2(4)
                12 参保日期 VARCHAR2(8)
                13 参保身份 VARCHAR2(10) 格式“编码-名称”，如“1101-企业一般人员”。见“字典说明”
                14 单位名称 VARCHAR2(100)
                15 性别 VARCHAR2(10) 名称
                16 医疗待遇享受类别 VARCHAR2(50) 名称（仅适用于新余市）
                 * 
                    返回的附加消息包：参保人员病种审批信息，多记录往后续加消息包
                 * 
                1 门特病种编码 VARCHAR2(20)
                2 门特病种名称 VARCHAR2(100)
                */
                WriteLog(sysdate + "  出参1|" + outData.ToString().Replace("RRRR|", ""));
                string[] str = outData.ToString().Replace("RRRR|", "").Split(';');
                string[] str2 = str[0].Split('|');
                outParams_dj OPDJ = new outParams_dj();
                OPDJ.Grbh = str2[0].Trim();      //保险号
                OPDJ.Kh = str2[1].Trim();        //卡号
                OPDJ.Xm = str2[2].Trim();        //姓名
                OPDJ.Tcqh = str2[3].Trim();      //地区编号
                OPDJ.Qxmc = str2[4].Trim();      //地区名称
                OPDJ.Zhye = str2[5].Trim();      //账户余额
                OPDJ.Zxywlsh = str2[6].Trim();   //中心业务流水号
                OPDJ.Yllb = str2[7].Trim();      //医疗类别
                OPDJ.Zycs = str2[8].Trim();      //本次看病次数
                OPDJ.Csrq = str2[9].Trim();      //出生日期
                OPDJ.Nl = str2[10].Trim();       //实际年龄
                OPDJ.Cbrq = str2[11].Trim();     //参保日期
                OPDJ.Cbsf = str2[12].Trim();     //参保身份
                OPDJ.Dwmc = str2[13].Trim();     //单位名称
                OPDJ.Xb = str2[14].Trim();       //性别
                OPDJ.Yldyxslb = str2[15].Trim(); //医疗待遇享受类别(医疗人员类别)
                #endregion

                if (str.Length > 1)
                {
                    strSql = string.Format(@"delete from ybmxbdj where bxh='{0}'", OPDJ.Grbh);
                    liSQL.Add(strSql);

                    for (int j = 1; j < str.Length; j++)
                    {
                        str2 = str[j].Split('|');
                        OPDJ.Mtbzbm = str2[0].Trim();
                        OPDJ.Mtbzmc = str2[1].Trim();
                        strSql = string.Format(@"insert into ybmxbdj (jzlsh,bxh,xm,kh,mmbzbm,mmbzmc) 
                                                values(
                                                '{0}','{1}','{2}','{3}','{4}','{5}')",
                                                jzlsh, OPDJ.Grbh, OPDJ.Xm, OPDJ.Kh, OPDJ.Mtbzbm, OPDJ.Mtbzmc);
                        liSQL.Add(strSql);
                    }
                }

                strSql = string.Format(@"insert into ybmzzydjdr(
                                        jzlsh,jylsh,yllb,bzbm,bzmc,ysdm,ysxm,dgysdm,dgysxm,ghdjsj,
                                        zszbm,ksbh,ksmc,ghf,grbh,kh,xm,xb,nl,sfzh,
                                        csrq,zhye,yldylb,dwmc,zycs,tcqh,qxmc,jbr,jzbz,sysdate,
                                        ybjzlsh,sylb,jhsylb) values(
                                       '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}',
                                       '{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}',
                                       '{20}','{21}','{22}','{23}','{24}','{25}','{26}','{27}','{28}','{29}',
                                       '{30}','{31}','{32}')",
                                        jzlsh, OPDJ.Zxywlsh, yllb, bzbm, bzmc, dgysdm, dgysxm, dgysdm, dgysxm, zyrq,
                                        zszbm, ksbh, ksmc, OPDJ.Ghfy, OPDJ.Grbh, OPDJ.Kh, OPDJ.Xm, OPDJ.Xb, OPDJ.Nl, sfzh,
                                        OPDJ.Csrq, OPDJ.Zhye, OPDJ.Cbsf, OPDJ.Dwmc, OPDJ.Zycs, OPDJ.Tcqh, OPDJ.Qxmc, jbr, "z", sysdate,
                                        OPDJ.Zxywlsh, sylb, jhsylb);
                liSQL.Add(strSql);

                strSql = string.Format(@"update zy01h set z1rylb = '{0}', z1tcdq = '{1}', z1lyjg = '{2}', z1lynm = '{3}', z1ylno = '{4}'
                    , z1ylnm = '{5}', z1bzno = '{6}', z1bznm = '{7}', z1ybno = '{8}' where z1comp = '{9}' and z1zyno = '{10}'"
                     , OPDJ.Yldyxslb, OPDJ.Tcqh, lyjgdm, lyjgmc, yllb, yllbmc, bzbm, bzmc, grbh, CliUtils.fSiteCode, jzlsh);
                liSQL.Add(strSql);

                object[] obj = liSQL.ToArray();
                obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);
                if (obj[1].ToString() == "1")
                {
                    //门诊就诊疾病诊断登记
                    object[] objParam1 = { jzlsh, "2", dgysdm, dgysxm, bzbm, bzmc };
                    object[] objReturn = YBZDDJ(objParam1);
                    if (objReturn[1].ToString().Equals("1"))
                    {
                        WriteLog(sysdate + "  " + jzlsh + " 进入住院医保登记成功|" + outData.ToString());
                        return new object[] { 0, 1, "住院医保登记成功" };
                    }
                    else
                    {
                        //医保登记撤销
                        YBZYDJCX(objParam1);
                        return new object[] { 0, 0, "住院医保登记失败" };
                    }
                }
                else
                {
                    WriteLog(sysdate + "  " + jzlsh + " 进入住院医保登记成功|操作本地数据失败|" + obj[2].ToString());
                    //住院登记（挂号）撤销
                    object[] objParam2 = new object[] { jzlsh, OPDJ.Grbh, OPDJ.Kh, OPDJ.Xm, OPDJ.Zxywlsh, OPDJ.Tcqh };
                    NYBZYDJCX(objParam2);
                    return new object[] { 0, 0, "住院医保登记失败" };
                }
            }
            else
            {
                //在医保登记失败情况下，修改缴费类别
                List<string> liSQL = new List<string>();
                strSql = string.Format(@"update zy01h set z1lyjg='0201',z1lynm='全费自理' where z1zyno='{0}'", jzlsh);
                liSQL.Add(strSql);
                object[] obj = liSQL.ToArray();
                obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);

                WriteLog(sysdate + "  " + jzlsh + " 进入住院医保登记失败|" + retMsg.ToString());
                return new object[] { 0, 0, "住院医保登记失败|" + retMsg.ToString() };
            }
        }
        #endregion

        #region 住院登记变更
        public static object[] YBZYDJBG(object[] objParam)
        {
            string sysdate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string Ywlx = "DJ05";
            string jzlsh = objParam[0].ToString();
            string yllb = objParam[1].ToString();
            string bzbm = objParam[2].ToString(); //病种编码
            string bzmc = objParam[3].ToString(); //病种名称
            string zyrqsj = objParam[4].ToString();
            string zszbm = objParam[5].ToString();
            string sylb = objParam[6].ToString();
            string jhsylb = objParam[7].ToString();

            string grbh = "";
            string xm = "";
            string kh = "";
            string ybjzlsh = "";
            string zyrq = "";
            string zysj = "";
            string zych = "";
            string ysdm = "";
            string ysxm = "";
            string ksdm = "";
            string ksmc = "";
            string dqbh = "";



            if (string.IsNullOrEmpty(yllb))
                return new object[] { 0, 0, "医疗类别不能为空" };
            if (string.IsNullOrEmpty(jzlsh))
                return new object[] { 0, 0, "就诊流水号不能为空" };
            if (string.IsNullOrEmpty(bzbm))
                return new object[] { 0, 0, "诊断不能为空" };

            #region 判断时间有效性
            try
            {
                DateTime dt_zy = Convert.ToDateTime(zyrqsj);
                zyrq = dt_zy.ToString("yyyyMMdd");
                zysj = dt_zy.ToString("HHmm");
            }
            catch
            {
                return new object[] { 0, 0, "住院日期格式错误" };
            }
            #endregion

            #region 判断是否已进行医保住院登记
            string strSql = string.Format(@"select grbh,kh,xm,tcqh,ybjzlsh,dgysdm,dgysxm,b.ybksdm,b.ybksmc from ybmzzydjdr a
                                            left join ybkszd  b on a.ksbh=b.ksdm  where jzlsh='{0}' and cxbz=1", jzlsh);
            DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            if (ds.Tables[0].Rows.Count == 0)
                return new object[] { 0, 0, "该患者未办理医保登记" };
            ds.Dispose();

            grbh = ds.Tables[0].Rows[0]["grbh"].ToString();
            kh = ds.Tables[0].Rows[0]["kh"].ToString();
            xm = ds.Tables[0].Rows[0]["xm"].ToString();
            ybjzlsh = ds.Tables[0].Rows[0]["ybjzlsh"].ToString();
            dqbh = ds.Tables[0].Rows[0]["tcqh"].ToString();
            ysdm = ds.Tables[0].Rows[0]["dgysdm"].ToString();
            ysxm = ds.Tables[0].Rows[0]["dgysxm"].ToString();
            ksdm = ds.Tables[0].Rows[0]["ybksdm"].ToString();
            ksmc = ds.Tables[0].Rows[0]["ybksmc"].ToString();
            #endregion

            /*
             1 社会保险号 VARCHAR2(20) 否
            2 社会保障卡号 VARCHAR2(20) 否
            3 姓名 VARCHAR2(20) 否
            4 中心业务流水号
            4 医疗类别 VARCHAR2(3) 否 代码（详见字典说明）
            5 科室代码 VARCHAR2(20) 代码（详见字典说明）
            6 科室名称 VARCHAR2(20)
            7 住院日期 VARCHAR2(8) 否 YYYYMMDD
            8 住院时间 VARCHAR2(6) 否 HH24MISS
            9 住院床号 VARCHAR2(20)
            11 主治医师编码 VARCHAR2(20) 定岗医师\药师编号，同DJ13
            12 主治医师姓名 VARCHAR2(20)
            13 主要病情描述 VARCHAR2(200)
            14 准生证编号 VARCHAR2(30) 生育保险专用
            15 地区编号 VARCHAR2(6) 否 统筹区编码，同CX01返回第7项
             */

            StringBuilder inputData = new StringBuilder(); 
            inputData.Append("SSSS|");
            inputData.Append(grbh + "|");
            inputData.Append(kh + "|");
            inputData.Append(xm + "|");
            inputData.Append(ybjzlsh + "|");
            inputData.Append(yllb + "|");
            inputData.Append(ksdm + "|");
            inputData.Append(ksmc + "|");
            inputData.Append(zyrq + "|");
            inputData.Append(zysj + "|");
            inputData.Append("NULL|");
            inputData.Append(ysdm + "|");
            inputData.Append(ysxm + "|");
            inputData.Append(bzmc + "|");
            inputData.Append(zszbm + "|");
            inputData.Append(dqbh + "|");
            inputData.Append("ZZZZ");

            StringBuilder outData = new StringBuilder(10240);
            StringBuilder retMsg = new StringBuilder(10240);

            WriteLog(sysdate + "  " + jzlsh + " 进入住院医保登记变更...");
            WriteLog(sysdate + "  入参|" + inputData.ToString());
            int i = f_UserBargaingApply(Ywlx, inputData, outData, retMsg);
            if (i > 0)
            {
                #region 诊断登记
                //先诊断撤销后登记
                object[] objParam_zd = { jzlsh, "2", ysdm, ysxm, bzbm, bzmc };
                YBZDDJCX(objParam_zd);
                YBZDDJ(objParam_zd);
                #endregion
                strSql = string.Format(@"update ybmzzydjdr set yllb='{1}',bzbm='{2}',bzmc='{3}',ghdjsj='{4}',zszbm='{5}',sylb='{6}',jhsylb='{7}' where jzlsh='{0}' and cxbz=1",
                                            jzlsh, yllb, bzbm, bzmc, zyrqsj, zszbm, sylb, jhsylb);
                object[] obj = { strSql };

                 obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);
                 if (obj[1].ToString() == "1")
                 {
                     WriteLog(sysdate + " 住院登记变更成功");
                     return new object[] { 0, 1, "住院登记变更成功" };
                 }
                 else
                 {
                     WriteLog(sysdate + " 住院登记变更失败"+obj[2].ToString());
                     return new object[] { 0, 0, "住院登记变更失败" + obj[2].ToString() };
                 }
            }
            else
            {
                WriteLog(sysdate + " 住院登记变更失败|" + retMsg.ToString());
                return new object[] { 0, 0, "住院登记变更失败|" + retMsg.ToString() };
            }
        }
        #endregion

        #region 住院登记撤销
        public static object[] YBZYDJCX(object[] objParam)
        {
            string sysdate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string Ywlx = "DJ04";

            string jzlsh = objParam[0].ToString();  //就诊流水号
            string jbr = CliUtils.fUserName;   //经办人
            string cxrq = sysdate;                  //撤销日期时间
            string bxh = "";
            string xm = "";
            string kh = "";
            string ybjzlsh = "";
            if (string.IsNullOrEmpty(jzlsh))
                return new object[] { 0, 0, "就诊流水号不能为空" };

            WriteLog(sysdate + "  " + jzlsh + " 进入住院医保登记撤销...");

            #region 判断是否医保登记
            string strSql = string.Format("select * from ybmzzydjdr WHERE jzlsh='{0}' and cxbz=1 ", jzlsh);
            DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            if (ds.Tables[0].Rows.Count == 0)
                return new object[] { 0, 0, "该患者未办理医保登记" };
            else
            {
                bxh = ds.Tables[0].Rows[0]["grbh"].ToString();
                xm = ds.Tables[0].Rows[0]["xm"].ToString();
                kh = ds.Tables[0].Rows[0]["kh"].ToString();
                dqbh = ds.Tables[0].Rows[0]["tcqh"].ToString();
                ybjzlsh = ds.Tables[0].Rows[0]["ybjzlsh"].ToString();
            }
            #endregion
            #region 判断是否撤销费用登记
            strSql = string.Format(@"select * from ybcfmxscindr where jzlsh='{0}' and cxbz=1", jzlsh);
            ds.Tables.Clear();
            ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            if (ds.Tables[0].Rows.Count > 0)
                return new object[] { 0, 0, "该患者已上传费用明细，请先撤销费用明细" };
            ds.Dispose();
            #endregion

            StringBuilder inputData = new StringBuilder();
            /*
                1 社会保险号 VARCHAR2(20) 否
                2 社会保障卡号 VARCHAR2(20) 否
                3 姓名 VARCHAR2(20) 否
                4 中心业务流水号 VARCHAR2(20) 否 住院号
                5 地区编号 VARCHAR2(6) 否 统筹区编码
             */
            inputData.Append("SSSS|");
            inputData.Append(bxh + "|");
            inputData.Append(kh + "|");
            inputData.Append(xm + "|");
            inputData.Append(ybjzlsh + "|");
            inputData.Append(dqbh+"|");
            inputData.Append("ZZZZ");

            StringBuilder outData = new StringBuilder(10240);
            StringBuilder retMsg = new StringBuilder(10240);

            WriteLog(sysdate + "  入参|" + inputData.ToString());
            int i = f_UserBargaingApply(Ywlx, inputData, outData, retMsg);
            if (i > 0)
            {
                List<string> liSQL = new List<string>();
                strSql = string.Format(@"insert into ybmzzydjdr(
                                       jzlsh,jylsh,yllb,bzbm,bzmc,ysdm,ysxm,dgysdm,dgysxm,ghdjsj,
                                       zszbm,ksbh,ksmc,ghf,grbh,kh,xm,xb,nl,sfzh,
                                       csrq,zhye,yldylb,dwmc,zycs,tcqh,qxmc,jbr,jzbz,ybjzlsh,
                                       sylb,jhsylb,sysdate,cxbz) select 
                                       jzlsh,jylsh,yllb,bzbm,bzmc,ysdm,ysxm,dgysdm,dgysxm,ghdjsj,
                                       zszbm,ksbh,ksmc,ghf,grbh,kh,xm,xb,nl,sfzh,
                                       csrq,zhye,yldylb,dwmc,zycs,tcqh,qxmc,'{3}',jzbz,ybjzlsh,
                                       sylb,jhsylb,'{2}',0 from ybmzzydjdr where jzlsh='{0}' and ybjzlsh='{1}' and cxbz=1", jzlsh, ybjzlsh, sysdate, jbr);
                liSQL.Add(strSql);

                strSql = string.Format("update ybmzzydjdr set cxbz = 2 where jzlsh = '{0}' and ybjzlsh='{1}' and cxbz=1", jzlsh, ybjzlsh);
                liSQL.Add(strSql);

                strSql = string.Format(@"update zy01h set z1lyjg='0201',z1lynm='全费自理' where z1zyno='{0}'", jzlsh);
                liSQL.Add(strSql);

                object[] obj = liSQL.ToArray();
                obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);
                if (obj[1].ToString() == "1")
                {
                    WriteLog(sysdate + "  " + jzlsh + " 进入住院医保登记撤销成功|" + outData.ToString());
                    return new object[] { 0, 1, "住院登记撤销成功", outData.ToString() };
                }
                else
                {
                    WriteLog(sysdate + "  " + jzlsh + " 进入住院医保登记撤销成功|操作本地数据失败|" + obj[2].ToString());
                    return new object[] { 0, 0, "住院登记撤销失败|操作本地数据失败|" + obj[2].ToString() };
                }
            }
            else
            {
                WriteLog(sysdate + "  " + jzlsh + " 进入住院医保登记撤销失败|" + retMsg.ToString());
                return new object[] { 0, 0, "住院登记撤销失败|" + retMsg.ToString() };
            }
        }
        #endregion

        #region 住院收费登记
        public static object[] YBZYSFDJ(object[] objParam)
        {
            string sysdate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string Ywlx = "DJ06";
            string jzlsh = objParam[0].ToString(); //就诊流水号
            string bxh = "";    //保险号
            string xm = "";     //姓名
            string kh = "";     //卡号
            string dgysdm = "";   //定岗医生编码
            string dgysxm = ""; //定岗医生姓名
            string ybjzlsh = "";//医保就诊流水号
            string scrq = "";
            string scsj = "";
            string yzbh = "";
            string jbr = CliUtils.fLoginUser;

            string jylsh = DateTime.Now.ToString("yyyyMMddHHmmss") + "-" + new Random().Next(100).ToString().PadLeft(4, '0');

            if (string.IsNullOrEmpty(jzlsh))
                return new object[] { 0, 0, "就诊流水号不能为空" };


            #region 判断是否医保登记
            string strSql = string.Format("select * from ybmzzydjdr where jzlsh='{0}' and cxbz=1", jzlsh);
            DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            if (ds.Tables[0].Rows.Count == 0)
                return new object[] { 0, 0, "该患者未办理医保登记" };
            else
            {
                bxh = ds.Tables[0].Rows[0]["grbh"].ToString();
                xm = ds.Tables[0].Rows[0]["xm"].ToString();
                kh = ds.Tables[0].Rows[0]["kh"].ToString();
                dqbh = ds.Tables[0].Rows[0]["tcqh"].ToString();
                dgysdm = ds.Tables[0].Rows[0]["dgysdm"].ToString();
                dgysxm = ds.Tables[0].Rows[0]["dgysxm"].ToString();
                ybjzlsh = ds.Tables[0].Rows[0]["ybjzlsh"].ToString();
                DateTime dt_zyrq = Convert.ToDateTime(ds.Tables[0].Rows[0]["ghdjsj"].ToString());
                scrq = dt_zyrq.ToString("yyyyMMdd");
                scsj = dt_zyrq.ToString("HHmmss");
                yzbh = dt_zyrq.ToString("yyMMddHHmmss") + "-" + new Random().Next(100).ToString().PadLeft(4, '0');
            }
            #endregion

            StringBuilder inputData = new StringBuilder();
            /*
             1 社会保险号 VARCHAR2(20) 否
            2 社会保障卡号 VARCHAR2(20) 否
            3 姓名 VARCHAR2(20) 否
            4 中心业务流水号 VARCHAR2(20) 否 住院号
            5 医嘱开具医师编号 VARCHAR2(20) 否 定岗医师\药师编号，同DJ13
            6 医嘱开具医师姓名 VARCHAR2(20) 否
            7 医嘱开具日期 VARCHAR2(8) 否 YYYYMMDD
            8 医嘱开具时间 VARCHAR2(6) 否 HH24MISS
            9 定点机构医嘱编号 VARCHAR2(20) 否
            10 地区编号 VARCHAR2(6) 否 统筹区编码
             */
            inputData.Append("SSSS|");
            inputData.Append(bxh + "|");
            inputData.Append(kh + "|");
            inputData.Append(xm + "|");
            inputData.Append(ybjzlsh + "|");
            inputData.Append(dgysxm + "|");
            inputData.Append(dgysxm + "|");
            inputData.Append(scrq + "|");
            inputData.Append(scsj + "|");
            inputData.Append(yzbh + "|");
            inputData.Append(dqbh + ";");

            #region 收费明细信息
            //List<string> li_yyxmbh = new List<string>();
            //List<string> li_yyxmmc = new List<string>();
            //List<string> li_ybxmbh = new List<string>();
            //List<string> li_sn = new List<string>();
            //List<string> li_dj = new List<string>();
            //List<string> li_je = new List<string>();

            List<string> li_inputData = new List<string>();
            List<string> liSQL = new List<string>();
            int index = 1;

            strSql = string.Format(@"select b.ybxmbh,b.ybxmmc,a.z3djxx as dj,case LEFT(a.z3endv,1) when '4' then -a.z3jzcs else a.z3jzcs end as sl,
                                    case LEFT(a.z3endv,1) when '4' then -a.z3jzje else a.z3jzje end as je,a.z3item as yyxmbh, a.z3name as yyxmmc,
                                    a.z3empn as ysdm, a.z3kdys as ysxm,convert(datetime,a.z3date) as yysj,a.z3ksno as ksno, a.z3zxks as zxks, z3sfno as sfno, 
                                    b.sfxmzldm as ybsfxmzldm, b.sflbdm as ybsflbdm,b.sfxmdjdm as xmlx,a.z3ghno+a.z3sequ as sn from zy03d a left join ybhisdzdr b on 
                                    a.z3item=b.hisxmbh where a.z3ybup is null and LEFT(a.z3kind,1)in(2,4)  and a.z3zyno='{0}'", jzlsh);
            ds.Tables.Clear();
            ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            
            if (ds.Tables[0].Rows.Count > 0)
            {
                StringBuilder strMsg = new StringBuilder();
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    if (dr["ybxmbh"] == DBNull.Value)
                    {
                        strMsg.Append("项目代码：[" + dr["yyxmbh"].ToString() + "]，名称：[" + dr["yyxmmc"].ToString() + "]未对照，不能上传！\r\n");
                    }
                    else
                    {
                        string ybxmbh = dr["ybxmbh"].ToString();
                        string ybxmmc = dr["ybxmmc"].ToString();
                        string yyxmbh = dr["yyxmbh"].ToString();
                        string yyxmmc = dr["yyxmmc"].ToString();
                        double dj = Convert.ToDouble(dr["dj"]);
                        double sl = Convert.ToDouble(dr["sl"]);
                        double je = Convert.ToDouble(dr["je"]);
                        string fysj = dr["yysj"].ToString();
                        string yyrq = Convert.ToDateTime(dr["yysj"].ToString()).ToString("yyyyMMdd");
                        string yysj = Convert.ToDateTime(dr["yysj"].ToString()).ToString("HHmmss");
                        string gg = "";
                        string jx = "";
                        string jldw = "";
                        string yfyl = "";


                        /*
                            发送的附加消息包：多记录往后续加消息包
                            1 定点医疗机构明细序号 VARCHAR2(20) 否
                            2 定点医疗机构三大目录编码 VARCHAR2(30) 否
                            3 定点医疗机构三大目录名称 VARCHAR2(150) 否
                            4 定点医疗机构三大目录规格 VARCHAR2(50)
                            5 定点医疗机构三大目录剂型 VARCHAR2(50)
                            6 单价 NUMBER(10,4) 否
                            7 数量 NUMBER(8,2) 否
                            8 金额 NUMBER(10,4) 否
                            9 社保三大目录编码 VARCHAR2(20) 否
                            10 社保三大目录名称 VARCHAR2(150) 否
                            11 费用发生日期 VARCHAR2(8) 否 YYYYMMDD
                            12 费用发生时间 VARCHAR2(6) 否 HH24MISS
                            13 剂量单位 VARCHAR2(50) 定点机构信息。2013/11/21新增
                            14 用法用量 VARCHAR2(50) 定点机构信息。2013/11/21新增
                            */
                        StringBuilder inputData2 = new StringBuilder();
                        inputData2.Append(index + "|");
                        inputData2.Append(yyxmbh + "|");
                        inputData2.Append(yyxmmc + "|");
                        inputData2.Append(gg + "|");
                        inputData2.Append(jx + "|");
                        inputData2.Append(dj + "|");
                        inputData2.Append(sl + "|");
                        inputData2.Append(je + "|");
                        inputData2.Append(ybxmbh + "|");
                        inputData2.Append(ybxmmc + "|");
                        inputData2.Append(yyrq + "|");
                        inputData2.Append(yysj + "|");
                        inputData2.Append(jldw + "|");
                        inputData2.Append(yfyl + ";");

                        index++;
                        li_inputData.Add(inputData2.ToString());
                    }
                }
                if (!string.IsNullOrEmpty(strMsg.ToString()))
                    return new object[] { 0, 0, strMsg.ToString() };
            }
            else
                return new object[] { 0, 0, "无费用明细" };
            ds.Dispose();
            #endregion

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
                    StringBuilder retMsg = new StringBuilder(102400);

                    StringBuilder inputData1 = new StringBuilder();
                    inputData1.Append(inputData.ToString().TrimEnd(';') + "|ZZZZ");

                    WriteLog(sysdate + "  " + jzlsh + " 进入住院费用明细上传(分段)...");
                    WriteLog(sysdate + "  入参|" + inputData1.ToString());
                    int i = f_UserBargaingApply(Ywlx, inputData1, outData, retMsg);
                    if (i <= 0)
                    {
                        WriteLog(sysdate + "  " + jzlsh + " 进入住院费用明细上传(分段)失败|" + retMsg.ToString());
                        return new object[] { 0, 0, "住院收费登记失败|" + retMsg.ToString() };
                    }
                    WriteLog(sysdate + "  出参|" + outData.ToString());
                    string[] str = outData.ToString().Replace("RRRR|", "").Split(';');
                    string[] str1 = str[0].Split('|');
                    outParams_fymx opfy = new outParams_fymx();
                    if (str1.Length > 1)
                    {
                        opfy.Ybjzlsh = str1[0].Trim();
                        opfy.Zxyzbh = str1[1].Trim();
                        opfy.Yyyzbh = str1[2].Trim();
                    }
                    for (int j = 1; j < str.Length; j++)
                    {
                        string[] str2 = str[j].Split('|');
                        /*
                        1定点机构明细序号 VARCHAR2(20) 否
                        2定点机构三大目录编码 VARCHAR2(30) 否
                        3定点机构三大目录名称 VARCHAR2(150) 否
                        4定点机构三大目录规格 VARCHAR2(50)
                        5定点机构三大目录剂型 VARCHAR2(50)
                        6 单价 NUMBER(10,4) 否
                        7 数量 NUMBER(8,2) 否
                        8 金额 NUMBER(10,4) 否
                        9社保三大目录编码 VARCHAR2(20) 否
                        10 社保三大目录名称 VARCHAR2(150) 否
                        11 收费项目等级 VARCHAR2(20) 名称(甲类、乙类、丙类、自费)
                        12 发票项目类别 VARCHAR2(20) 名称（西药费、检查费等）
                        13 中心明细序号 VARCHAR2(20) 否 可用于精确退费（DJ07）
                        14 费用发生日期 VARCHAR2(8) 否 YYYYMMDD
                        15 费用上传时间 VARCHAR2(20) 否
                         */
                        opfy.Yysequ = str2[0].Trim();
                        opfy.Yyxmbh = str2[1].Trim();
                        opfy.Yyxmmc = str2[2].Trim();
                        opfy.Yygg = str2[3].Trim();
                        opfy.Yyjx = str2[4].Trim();
                        opfy.Dj = str2[5].Trim();
                        opfy.Sl = str2[6].Trim();    //数量
                        opfy.Je = str2[7].Trim();    //金额
                        opfy.Ybxmbm = str2[8].Trim();
                        opfy.Ybxmmc = str2[9].Trim();
                        opfy.Sfxmdj = str2[10].Trim();
                        opfy.Sfxmlb = str2[11].Trim();
                        opfy.Zxsequ = str2[12].Trim();
                        opfy.Fyscrq = str2[13].Trim();
                        opfy.Fyscsj = str2[14].Trim();
                        opfy.Cxjzfje = "0";
                        opfy.Zfje = "0";
                        opfy.Xjbz = "0";
                        opfy.Xzsybz = "0";


                        //MessageBox.Show(li_dj[j] + "||" + Convert.ToDouble(dj2).ToString());
                        strSql = string.Format(@"insert into ybcfmxscfhdr(jzlsh,jylsh,ybjzlsh,xm,kh,yybxmbh,ybxmmc,yyxmdm,yyxmmc,sfxmdj,
                                                        sflb,dj,sl,je,zfje,zlje,cxjzfje,bz,ybcfh,cfh,
                                                        ybsequ,yysequ,sysdate,grbh) values(
                                                        '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}',
                                                        '{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}',
                                                        '{20}','{21}','{22}','{23}')",
                                                         jzlsh, jylsh, opfy.Ybjzlsh, xm, kh, opfy.Yyxmbh, opfy.Yyxmmc, opfy.Ybxmbm, opfy.Ybxmmc, opfy.Sfxmdj,
                                                         opfy.Sfxmlb, opfy.Dj, opfy.Sl, opfy.Je, opfy.Zfje, opfy.Xjbz, opfy.Cxjzfje, opfy.Xzsybz, opfy.Zxyzbh, opfy.Yyyzbh,
                                                         opfy.Zxsequ, opfy.Yysequ, sysdate, bxh);
                        liSQL.Add(strSql);
                        //WriteLog(sysdate + "  " + jzlsh + "上传处方明细-->" + str[j]);

                    }


                    iTemp = 1;
                    inputData.Remove(0, inputData.Length);
                    inputData.Append("SSSS|");
                    inputData.Append(bxh + "|");
                    inputData.Append(kh + "|");
                    inputData.Append(xm + "|");
                    inputData.Append(ybjzlsh + "|");
                    inputData.Append(dgysxm + "|");
                    inputData.Append(dgysxm + "|");
                    inputData.Append(scrq + "|");
                    inputData.Append(scsj + "|");
                    inputData.Append(yzbh + "|");
                    inputData.Append(dqbh + ";");
                    inputData.Append(inputData3);
                }
            }
            #endregion
            #region 明细不足50条时，一次性上传
            if (iTemp > 0)
            {
                StringBuilder outData = new StringBuilder(102400);
                StringBuilder retMsg = new StringBuilder(102400);
                WriteLog(sysdate + "  " + jzlsh + " 进入住院费用明细上传(补传、一次性上传)...");
                //WriteLog(sysdate + " 入参|" + inputData.ToString());
                StringBuilder inputData1 = new StringBuilder();
                    inputData1.Append(inputData.ToString().TrimEnd(';') + "|ZZZZ");
                
                WriteLog(sysdate + " 入参1|" + inputData1.ToString());
                int i = f_UserBargaingApply(Ywlx, inputData1, outData, retMsg);
                if (i <= 0)
                {
                    WriteLog(sysdate + "  " + jzlsh + " 进入住院费用明细上传(补传、一次性上传)失败|" + retMsg.ToString());
                    return new object[] { 0, 0, "住院收费登记失败|" + retMsg.ToString() };

                }
                string[] str = outData.ToString().Replace("RRRR|", "").Split(';');
                string[] str1 = str[0].Split('|');
                outParams_fymx opfy = new outParams_fymx();
                if (str1.Length > 1)
                {
                    opfy.Ybjzlsh = str1[0].Trim();
                    opfy.Zxyzbh = str1[1].Trim();
                    opfy.Yyyzbh = str1[2].Trim();
                }
                for (int j = 1; j < str.Length; j++)
                {
                    string[] str2 = str[j].Split('|');
                    /*
                    1定点机构明细序号 VARCHAR2(20) 否
                    2定点机构三大目录编码 VARCHAR2(30) 否
                    3定点机构三大目录名称 VARCHAR2(150) 否
                    4定点机构三大目录规格 VARCHAR2(50)
                    5定点机构三大目录剂型 VARCHAR2(50)
                    6 单价 NUMBER(10,4) 否
                    7 数量 NUMBER(8,2) 否
                    8 金额 NUMBER(10,4) 否
                    9社保三大目录编码 VARCHAR2(20) 否
                    10 社保三大目录名称 VARCHAR2(150) 否
                    11 收费项目等级 VARCHAR2(20) 名称(甲类、乙类、丙类、自费)
                    12 发票项目类别 VARCHAR2(20) 名称（西药费、检查费等）
                    13 中心明细序号 VARCHAR2(20) 否 可用于精确退费（DJ07）
                    14 费用发生日期 VARCHAR2(8) 否 YYYYMMDD
                    15 费用上传时间 VARCHAR2(20) 否
                     */
                    opfy.Yysequ = str2[0].Trim();
                    opfy.Yyxmbh = str2[1].Trim();
                    opfy.Yyxmmc = str2[2].Trim();
                    opfy.Yygg = str2[3].Trim();
                    opfy.Yyjx = str2[4].Trim();
                    opfy.Dj = str2[5].Trim();
                    opfy.Sl = str2[6].Trim();    //数量
                    opfy.Je = str2[7].Trim();    //金额
                    opfy.Ybxmbm = str2[8].Trim();
                    opfy.Ybxmmc = str2[9].Trim();
                    opfy.Sfxmdj = str2[10].Trim();
                    opfy.Sfxmlb = str2[11].Trim();
                    opfy.Zxsequ = str2[12].Trim();
                    opfy.Fyscrq = str2[13].Trim();
                    opfy.Fyscsj = str2[14].Trim();
                    opfy.Cxjzfje = "0";
                    opfy.Zfje = "0";
                    opfy.Xjbz = "0";
                    opfy.Xzsybz = "0";

                    strSql = string.Format(@"insert into ybcfmxscfhdr(jzlsh,jylsh,ybjzlsh,xm,kh,yybxmbh,ybxmmc,yyxmdm,yyxmmc,sfxmdj,
                                            sflb,dj,sl,je,zfje,zlje,cxjzfje,bz,ybcfh,cfh,
                                            ybsequ,yysequ,sysdate,grbh) values(
                                            '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}',
                                            '{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}',
                                            '{20}','{21}','{22}','{23}')",
                                            jzlsh, jylsh, opfy.Ybjzlsh, xm, kh, opfy.Yyxmbh, opfy.Yyxmmc, opfy.Ybxmbm, opfy.Ybxmmc, opfy.Sfxmdj,
                                            opfy.Sfxmlb, opfy.Dj, opfy.Sl, opfy.Je, opfy.Zfje, opfy.Xjbz, opfy.Cxjzfje, opfy.Xzsybz, opfy.Zxyzbh, opfy.Yyyzbh,
                                            opfy.Zxsequ, opfy.Yysequ, sysdate, bxh);
                    liSQL.Add(strSql);
                    //WriteLog(sysdate + "  " + jzlsh + "上传处方明细-->" + str[j]);
                }
            }
            #endregion

            strSql = string.Format(@"update zy03d set z3ybup = '{0}' where z3ybup is null and LEFT(z3kind,1)=2 and z3zyno = '{1}' ", ybjzlsh, jzlsh);
            liSQL.Add(strSql);
            object[] obj = liSQL.ToArray();
            obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);
            if (obj[1].ToString() == "1")
            {
                WriteLog(sysdate + "  " + jzlsh + " 进入住院费用明细上传成功｜操作本地数据库成功|");
                return new object[] { 0, 1, "住院收费登记成功" };
            }
            else
            {
                WriteLog(sysdate + "  " + jzlsh + " 进入住院费用明细上传成功｜操作本地数据库失败|" + obj[2].ToString());
                object[] objParam2 = new object[] { jzlsh, bxh, xm, kh, ybjzlsh, dqbh };
                NYBZYSFDJCX(objParam2);
                return new object[] { 0, 0, "住院收费登记失败" };
            }
        }
        #endregion

        #region 住院收费登记撤销(全部)
        public static object[] YBZYSFDJCX(object[] objParam)
        {
            string sysdate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string Ywlx = "DJ08";
            string jzlsh = objParam[0].ToString();
            string bxh = "";
            string xm = "";
            string kh = "";
            string ybjzlsh = "";

            if (string.IsNullOrEmpty(jzlsh))
                return new object[] { 0, 0, "就诊流水号不能为空" };

            WriteLog(sysdate + "  " + jzlsh + " 进入住院费用明细撤销(全部)...");

            #region 获取登记信息
            string strSql = string.Format("select * from ybmzzydjdr where jzlsh='{0}' and cxbz=1", jzlsh);
            DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            if (ds.Tables[0].Rows.Count == 0)
            {
                return new object[] { 0, 0, "该患者未办理医保登记" };
            }
            else
            {
                bxh = ds.Tables[0].Rows[0]["grbh"].ToString();
                xm = ds.Tables[0].Rows[0]["xm"].ToString();
                kh = ds.Tables[0].Rows[0]["kh"].ToString();
                dqbh = ds.Tables[0].Rows[0]["tcqh"].ToString();
                ybjzlsh = ds.Tables[0].Rows[0]["ybjzlsh"].ToString();
            }
            #endregion

            #region 是否进行医保结算
            strSql = string.Format("select * from ybfyjsdr where jzlsh='{0}' and cxbz=1", jzlsh);
            ds.Tables.Clear();
            ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            if (ds.Tables[0].Rows.Count > 0)
                return new object[] { 0, 0, "该患者已做费用结算，请先撤销费用结算" };
            #endregion

            #region 是否重复撤销
            strSql = string.Format(@"select * from ybcfmxscfhdr where  jzlsh='{0}' and cxbz=1", jzlsh);
            ds.Tables.Clear();
            ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            if (ds.Tables[0].Rows.Count == 0)
                return new object[] { 0, 0, "无撤销费用或费用已经撤销" };
            #endregion
            ds.Dispose();

            StringBuilder inputData = new StringBuilder();
            /*
             1 社会保险号 VARCHAR2(20) 否
            2 社会保障卡号 VARCHAR2(20) 否
            3 姓名 VARCHAR2(20) 否
            4 中心业务流水号 VARCHAR2(20) 否 住院号
            5 地区编号 VARCHAR2(6) 否 统筹区编码             */
            inputData.Append("SSSS|");
            inputData.Append(bxh + "|");
            inputData.Append(kh + "|");
            inputData.Append(xm + "|");
            inputData.Append(ybjzlsh + "|");
            inputData.Append(dqbh + "|");
            inputData.Append("ZZZZ");
            StringBuilder outData = new StringBuilder(1024);
            StringBuilder retMsg = new StringBuilder(1024);

            WriteLog(sysdate + "  入参|" + inputData.ToString());
            int i = f_UserBargaingApply(Ywlx, inputData, outData, retMsg);
            if (i > 0)
            {
                List<string> liSQL = new List<string>();
                strSql = string.Format(@"insert into ybcfmxscfhdr(jzlsh,jylsh,ybjzlsh,xm,kh,yybxmbh,ybxmmc,yyxmdm,yyxmmc,sfxmdj,
                                        sflb,dj,sl,je,zfje,zlje,cxjzfje,bz,ybcfh,cfh,grbh,
                                        ybsequ,yysequ,sysdate,cxbz) select 
                                        jzlsh,jylsh,ybjzlsh,xm,kh,yybxmbh,ybxmmc,yyxmdm,yyxmmc,sfxmdj,
                                        sflb,dj,sl,je,zfje,zlje,cxjzfje,bz,ybcfh,cfh,grbh,
                                        ybsequ,yysequ,'{2}',0 from ybcfmxscfhdr 
                                        where jzlsh='{0}' and ybjzlsh='{1}' and cxbz=1 ", jzlsh, ybjzlsh, sysdate);
                liSQL.Add(strSql);
                strSql = string.Format("update ybcfmxscfhdr set cxbz=2 where jzlsh='{0}' and ybjzlsh='{1}' and cxbz=1", jzlsh, ybjzlsh);
                liSQL.Add(strSql);
                strSql = string.Format("update zy03d set z3ybup = null where z3ybup is not null and z3zyno = '{0}' and left(z3kind, 1) = '2'", jzlsh);
                liSQL.Add(strSql);
                object[] obj = liSQL.ToArray();
                obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);
                if (obj[1].ToString() == "1")
                {
                    WriteLog(sysdate + "  " + jzlsh + " 进入住院费用明细撤销(全部)成功|" + outData.ToString());
                    return new object[] { 0, 1, "住院收费退费成功", outData.ToString() };
                }
                else
                {
                    WriteLog(sysdate + "  " + jzlsh + " 进入住院费用明细撤销(全部)成功|操作本地数据失败|" + obj[2].ToString());
                    return new object[] { 0, 0, "住院收费退费失败|" + obj[2].ToString() };
                }
            }
            else
            {
                WriteLog(sysdate + "  " + jzlsh + " 进入住院费用明细撤销(全部)失败|" + retMsg.ToString());
                return new object[] { 0, 0, "住院收费退费失败|" + retMsg.ToString() };
            }
        }
        #endregion

        #region 住院收费预结算
        public static object[] YBZYSFYJS(object[] objParam)
        {
            string sysdate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string Ywlx = "JS15";

            string jzlsh = objParam[0].ToString();      //就诊流水号
            string cyyy = objParam[1].ToString();       //出院原因
            string zhsybz = objParam[2].ToString();     //账户使用标志
            string ztjsbz = objParam[3].ToString();     //中途结算标志
            string jsrqsj = objParam[4].ToString();     //结算日期时间
            string sfdypj = "FALSE";     //是否打印票据
            string cyzd = "";       //出院诊断
            string djh = "00000";        //单据号（发票号）
            string cyrqsj = objParam[5].ToString();     //出院日期时间
            string sfje = objParam[6].ToString();       //收费金额
            string bzbh = "";
            string bzmc = "";
            string cfqsrq = "NULL";
            string cfjzrq = "NULL";
            string cyrq = "";
            string cysj = "";
            double sfje2 = 0.000;
            string bxh = "";
            string xm = "";
            string kh = "";
            string ybjzlsh = "";
            string yllb = "";
            string sylb = "";
            string jhsylb = "";
            string jbr = CliUtils.fLoginUser;
            string jylsh = DateTime.Now.ToString("yyyyMMddHHmmss") + "-" + new Random().Next(100).ToString().PadLeft(4, '0');

            if (string.IsNullOrEmpty(jzlsh))
                return new object[] { 0, 0, "就诊流水号不能为空" };
            if (string.IsNullOrEmpty(cyyy))
                return new object[] { 0, 0, "出院原因不能为空" };
            if (string.IsNullOrEmpty(cyrqsj))
                return new object[] { 0, 0, "出院日期时间不能为空" };
            if (string.IsNullOrEmpty(sfdypj))
                return new object[] { 0, 0, "是否打印票据不能为空" };
            if (string.IsNullOrEmpty(sfje))
                return new object[] { 0, 0, "收费金额不能为空" };


            WriteLog(sysdate + "  " + jzlsh + " 进入住院收费预结算...");

            #region 时间转换
            try
            {
                DateTime dt_cyrq = Convert.ToDateTime(cyrqsj);
                cyrq = dt_cyrq.ToString("yyyyMMdd");
                cysj = dt_cyrq.ToString("HHmm");
            }
            catch
            {
                return new object[] { 0, 0, "出院日期时间格式错误" };
            }
            #endregion

            #region 金额合理性
            try
            {
                sfje2 = Convert.ToDouble(sfje);
            }
            catch
            {
                return new object[] { 0, 0, "收费金额数据格式有误" };
            }
            #endregion

            #region 是否未办理医保登记
            string strSql = string.Format("select * from ybmzzydjdr where jzlsh='{0}' and cxbz=1", jzlsh);
            DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            if (ds.Tables[0].Rows.Count == 0)
                return new object[] { 0, 0, "该患者未办理医保登记" };
            else
            {
                bxh = ds.Tables[0].Rows[0]["grbh"].ToString();
                xm = ds.Tables[0].Rows[0]["xm"].ToString();
                kh = ds.Tables[0].Rows[0]["kh"].ToString();
                dqbh = ds.Tables[0].Rows[0]["tcqh"].ToString();
                ybjzlsh = ds.Tables[0].Rows[0]["ybjzlsh"].ToString();
                bzbh = ds.Tables[0].Rows[0]["bzbm"].ToString();
                bzmc = ds.Tables[0].Rows[0]["bzmc"].ToString();
                yllb = ds.Tables[0].Rows[0]["yllb"].ToString(); 
                sylb = ds.Tables[0].Rows[0]["sylb"].ToString();
                jhsylb = ds.Tables[0].Rows[0]["jhsylb"].ToString();
            }
            #endregion

            #region 是否已经医保结算
            strSql = string.Format(@"select * from ybfyjsdr where jzlsh='{0}' and cxbz=1", jzlsh);
            ds.Tables.Clear();
            ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            if (ds.Tables[0].Rows.Count > 0)
                return new object[] { 0, 0, "该患者已办理医保结算" };
            #endregion

            #region 上传费用总金额
            strSql = string.Format("select SUM(je) from ybcfmxscfhdr where jzlsh='{0}' and ybjzlsh='{1}' and cxbz=1", jzlsh, ybjzlsh);
            ds.Tables.Clear();
            ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            if (string.IsNullOrEmpty(ds.Tables[0].Rows[0][0].ToString()))
                return new object[] { 0, 0, "费用明细未上传" };

            double sfje3 = Convert.ToDouble(ds.Tables[0].Rows[0][0].ToString());
            if (Math.Abs(sfje2 - sfje3) > 1)
                return new object[] { 0, 0, "总费用与医保结算总费用相差" + Math.Abs(sfje2 - sfje3) + ",无法结算，请核实!" };
            #endregion

            ds.Dispose();
            StringBuilder inputData = new StringBuilder();
            /*
             1 社会保险号 VARCHAR2(20) 否
             2 社会保障卡号 VARCHAR2(20) 否
             3 姓名 VARCHAR2(20) 否
             4 地区编码 
             5 中心业务流水号 VARCHAR2(20) 否 门诊号
             6 医疗类别 VARCHAR2(3) 代码（详见字典说明）
             7 特病种编码 VARCHAR2(20) 多病种以分隔符“#”号连接
             8 特病种名称 VARCHAR2(100) 多病种以分隔符“#”号连接
             9 结算费用起始日期 VARCHAR2(8) YYYYMMDD，可以为NULL值，不指定日期表示结算所有未结算费用。
             10 结算费用截止日期 VARCHAR2(8)
             11 个人账户抵扣现金支付标志 VARCHAR2(3) 0-不予支付；1-支付（默认 1）（政策允许情况下才有效）
             12 生育类别 VARCHAR2(3) 编码，见“字典说明”部分。“医疗类别”为“51-生育门诊”、“52-生育住院”时，第14、15项至少输入一项。 
             13 计划生育类别 VARCHAR2(3)
             14 地区编号 VARCHAR2(6) 否 统筹区编码
             15 出院日期
             16 出院时间
             17 出院原因
          */
            inputData.Append("SSSS|");
            inputData.Append(bxh + "|");
            inputData.Append(kh + "|");
            inputData.Append(xm + "|");
            inputData.Append(dqbh + "|");
            inputData.Append(ybjzlsh + "|");
            inputData.Append(yllb + "|");
            inputData.Append(bzbh + "|");
            inputData.Append(bzmc + "|");
            inputData.Append(cfqsrq + "|");
            inputData.Append(cfjzrq + "|");
            inputData.Append(zhsybz + "|");
            inputData.Append(sylb + "|");
            inputData.Append(jhsylb + "|");
            inputData.Append(cyrq + "|");
            inputData.Append(cysj + "|");
            inputData.Append(cyyy + "|");
            inputData.Append("ZZZZ");
            StringBuilder outData = new StringBuilder(10240);
            StringBuilder retMsg = new StringBuilder(1024);

            WriteLog(sysdate + " 入参|" + inputData.ToString());
            List<string> liSQL = new List<string>();
            int i = f_UserBargaingApply(Ywlx, inputData, outData, retMsg);
            if (i > 0)
            {
                #region 出参
                WriteLog(sysdate + "  " + jzlsh + " 出参|" + outData.ToString());
                /*出参
                1 社会保险号 VARCHAR2(20)
                2 社会保障卡号 VARCHAR2(20)
                3 姓名 VARCHAR2(20)
                4 性别 VARCHAR2(10) 名称
                5 出生日期 VARCHAR2(8) YYYYMMDD
                6 地区编号 VARCHAR2(6) 统筹区编码
                7 地区名称 VARCHAR2(20)
                8 参保日期 VARCHAR2(8) YYYYMMDD
                9 参保身份 VARCHAR2(50) 格式“编码-名称”，如“1101-企业一般人员”。见“字典说明”
                10 行政职务级别 VARCHAR2(50) 格式“编码-名称”，如“121-厅局级正职”。见“字典说明”
                11 医疗待遇享受类别 VARCHAR2(50) 格式“编码-名称”，如“1101-统账在职”。见“字典说明”（仅适用于新余市）
                12 单位名称 VARCHAR2(100) 格式“单位编码-单位名称”， 如“666666-测试单位”
                13 单位类型 VARCHAR2(20) 格式“编码-名称”，如“100-正常企业”。见“字典说明”
                14 中心业务流水号 VARCHAR2(20) 住院号
                15 中心结算单据号 VARCHAR2(20)
                16 医疗类别 VARCHAR2(50) 名称
                17 结算费用起始日期 VARCHAR2(8)
                18 结算费用截止日期 VARCHAR2(8)
                19 医疗总费用 NUMBER(16,2)
                20 本次账户支付 NUMBER(16,2)
                21 本次现金支付 NUMBER(16,2)
                22 职工基本医疗基金支付 NUMBER(16,2)
                23 城镇居民基本医疗基金支付 NUMBER(16,2)
                24 城乡居民基本医疗基金支付 NUMBER(16,2)
                25 大病医疗基金支付 NUMBER(16,2)
                26 公务员补充医疗基金支付 NUMBER(16,2)
                27 企业补充医疗基金支付 NUMBER(16,2)
                28 二乙专项医疗基金支付 NUMBER(16,2)
                29 老红军专项医疗基金支付 NUMBER(16,2)
                30 离休干部单独统筹基金支付 NUMBER(16,2)
                31 医疗保健支付 NUMBER(16,2) 如新余地厅级干部医疗保健补差
                32 其他基金支付 NUMBER(16,2) 预留指标项。（抚州市：医疗补贴金额。已计入统筹及大病基金中，此为其中项。）
                33 工伤基金支付 NUMBER(16,2)
                34 生育基金支付 NUMBER(16,2)
                35 民政救助支付 NUMBER(16,2)
                36 单位负担支付 NUMBER(16,2)
                37 定点机构支付 NUMBER(16,2)
                38 医保账户余额 NUMBER(16,2)
                39 甲类费用 NUMBER(16,2)
                40 乙类费用 NUMBER(16,2)
                41 丙类费用 NUMBER(16,2) 抚州指“乙类*”
                42 自费费用 NUMBER(16,2)
                43 先行自付 NUMBER(16,2)
                44 起付段自付 NUMBER(16,2)
                45 转外自付 NUMBER(16,2)
                46 统筹段自付 NUMBER(16,2)
                47 大病统筹自付 NUMBER(16,2)
                48 公务员补充医疗自付 NUMBER(16,2)
                49 超过最高封顶线自付 NUMBER(16,2)
                50 转诊单位负担 NUMBER(16,2)
                51 统筹段单位负担 NUMBER(16,2)
                52 大病段单位负担 NUMBER(16,2)
                53 统筹段基金支付 NUMBER(16,2)
                54 大病段基金支付 NUMBER(16,2)
                55 个人账户抵扣现金支付标志 VARCHAR2(3) 0-不予支付；1-支付（默认 1）
                56 公务员补助基本医疗支付 NUMBER(16,2) 新余：指补助 65%部分。其他：补助统筹段支付
                57 公务员补助大病医疗支付 NUMBER(16,2) 新余：指补助 80%部分。其他：补助大病段支付
                58 结算人 VARCHAR2(20)
                59 结算日期 VARCHAR2(8) YYYYMMDD
                60 结算时间 VARCHAR2(6) HH24MISS
                    * 
                返回的附加消息包：费用明细，多记录往后续加消息包。*门诊返回，住院不返回。

                1 社保三大目录编码 VARCHAR2(20)
                2 社保三大目录名称 VARCHAR2(150)
                3 收费项目等级 VARCHAR2(20) 格式“编码-名称”，如“1-甲类”。见“字典说明”
                4 发票项目类别 VARCHAR2(20) 格式“编码-名称”，如“11-西药”。见“字典说明”
                5 单价 NUMBER(10,4)
                6 数量 NUMBER(8,2)
                */
                string[] str = outData.ToString().Replace("RRRR|", "").Split(';');
                string[] str2 = str[0].Split('|');
                outParams_js op = new outParams_js();
                op.Grbh = str2[0].Trim();       //保险号
                op.Kh = str2[1].Trim();         //卡号
                op.Xm = str2[2].Trim();         //姓名
                op.Xb = str2[3].Trim();         //性别
                op.Csrq = str2[4].Trim();       //出生日期
                op.Dqbh = str2[5].Trim();      //地区编号
                op.Dqmc = str2[6].Trim();       //地区名称
                op.Cbrq = str2[7].Trim();       //参保日期
                op.Cbsf = str2[8].Trim();       //参保身份
                op.Xzzwjb = str2[9].Trim();     //行政职务级别
                op.Yldylb = str2[10].Trim();    //医疗待遇类别
                op.Dwmc = str2[11].Trim();      //单位名称
                op.Dwlx = str2[12].Trim();      //单位类别
                op.Ybjzlsh = str2[13].Trim();   //中心业务流水号，医保就诊流水号
                op.Jslsh = str2[14].Trim();     //结算单据号
                op.Yllb = str2[15].Trim();      //医疗类别
                op.Jsqsrq = str2[16].Trim();    //结算起始日期
                op.Jsjzrq = str2[17].Trim();    //结算截止日期
                op.Ylfze = str2[18].Trim();     //医疗总费用
                op.Zhzf = str2[19].Trim();      //本次帐户支付
                op.Xjzf = str2[20].Trim();      //本次现金支付
                op.Zgjbyljjzf = str2[21].Trim();
                op.Jzjmjbyljjzf = str2[22].Trim();
                op.Cxjmjbyljjzf = str2[23].Trim();
                op.Dejjzf = str2[24].Trim();
                op.Gwybzjjzf = str2[25].Trim();
                op.Qybcylbxjjzf = str2[26].Trim();
                op.Eyzxyljjzf = str2[27].Trim();
                op.Lhjzxyljjzf = str2[28].Trim();
                op.Lxgbdttcjjzf = str2[29].Trim();
                op.Ylbjzf = str2[30].Trim();
                op.Qtjjzf = str2[31].Trim();
                op.Gsjjzf = str2[32].Trim();
                op.Syjjzf = str2[33].Trim();
                op.Mzjzfy = str2[34].Trim();
                op.Dwfdfy = str2[35].Trim();
                op.Yyfdfy = str2[36].Trim();
                op.Bcjshzhye = str2[37].Trim();
                op.Jlfy = str2[38].Trim();
                op.Ylfy = str2[39].Trim();
                op.Blfy = str2[40].Trim();
                op.Zffy = str2[41].Trim();
                op.Xxzf = str2[42].Trim();      //先行自付
                op.Qfbzfy = str2[43].Trim();
                op.Zzzyzffy = str2[44].Trim();
                op.Tcfdzffy = str2[45].Trim();
                op.Defdzffy = str2[46].Trim();
                op.Gwybcylzf = str2[47].Trim();
                op.Cgzgfd = str2[48].Trim();
                op.Zzdwfd = str2[49].Trim();
                op.Tcddwfd = str2[50].Trim();
                op.Dbddwfd = str2[51].Trim();
                op.Jrtcfy = str2[52].Trim();
                op.Jrdebxfy = str2[53].Trim();
                op.Zhdkxjzfbz = str2[54].Trim();
                op.Gwybcjbylzf = str2[55].Trim();
                op.Gwybzdbylzf = str2[56].Trim();
                op.Jbr = str2[57].Trim();
                op.Jsrq = str2[58].Trim();
                op.Jssj = str2[59].Trim();
                op.Bcjsqzhye = (Convert.ToDecimal(op.Bcjshzhye) + Convert.ToDecimal(op.Zhzf)).ToString();
                op.Zbxje = (Convert.ToDecimal(op.Ylfze) - Convert.ToDecimal(op.Xjzf)).ToString();
                op.Tcjjzf = (Convert.ToDecimal(op.Ylfze) - Convert.ToDecimal(op.Xjzf) - Convert.ToDecimal(op.Zhzf)
                            - Convert.ToDecimal(op.Dejjzf) - Convert.ToDecimal(op.Mzjzfy) - Convert.ToDecimal(op.Yyfdfy)).ToString();

                op.Cxjfy = "0.00";
                op.Djh = op.Jslsh;
                #endregion

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
                string strValue = op.Ylfze + "|" + op.Zbxje + "|" + op.Tcjjzf + "|" + op.Dejjzf + "|" + op.Zhzf + "|" +
                                 op.Xjzf + "|" + op.Gwybzjjzf + "|" + op.Qybcylbxjjzf + "|" + op.Zffy + "|" + op.Dwfdfy + "|" +
                                 op.Yyfdfy + "|" + op.Mzjzfy + "|" + op.Cxjfy + "|" + op.Ylzlfy + "|" + op.Blzlfy + "|" +
                                 op.Fhjjylfy + "|" + op.Qfbzfy + "|" + op.Zzzyzffy + "|" + op.Jrtcfy + "|" + op.Tcfdzffy + "|" +
                                 op.Ctcfdxfy + "|" + op.Jrdebxfy + "|" + op.Defdzffy + "|" + op.Cdefdxfy + "|" + op.Rgqgzffy + "|" +
                                 op.Bcjsqzhye + "|" + op.Bntczflj + "|" + op.Bndezflj + "|" + op.Bnczjmmztczflj + "|" + op.Bngwybzzflj + "|" +
                                 op.Bnzhzflj + "|" + op.Bnzycslj + "|" + op.Zycs + "|" + op.Xm + "|" + op.Jsrq + op.Jssj + "|" +
                                 op.Yllb + "|" + op.Yldylb + "|" + op.Jbjgbm + "|" + op.Ywzqh + "|" + op.Jslsh + "|" +
                                 op.Tsxx + "|" + op.Djh + "|" + op.Jyxl + "|" + op.Yyjylsh + "|" + op.Yxbz + "|" +
                                 op.Grbhgl + "|" + op.Yljgbm + "|" + op.Ecbcje + "|" + op.Mmqflj + "|" + op.Jsfjylsh + "|" +
                                 op.Grbh + "|" + op.Dbzbc + "|" + op.Czzcje + "|" + op.Elmmxezc + "|" + op.Elmmxesy + "|" +
                                 op.Jmgrzfecbcje + "|" + op.Tjje + "|" + op.Syjjzf + "|";

                strSql = string.Format(@"delete from ybfyyjsdr where jzlsh='{0}'", jzlsh);
                liSQL.Add(strSql);
                strSql = string.Format(@"insert into ybfyyjsdr(
                                        jzlsh,djhin,yllb,bzbm,bzmc,grbh,kh,xm,ybjzlsh,djh,
                                        jslsh,xzzwjb,dqbh,dqmc,cbsf,yldyxslb,dwmc,dwlx,cbrq,jsqsrq,
                                        jsjzrq,ylfze,zbxje,tcjjzf,zhzf,xjzf,dejjzf,gwybzjjzf,qybcylbxjjzf,lxgbddtczf,
                                        mzjzfy,dwfdfy,yyfdfy,bcjsqzhye,jlfy,ylfy,blfy,zffy,qfbzfy,zgjbyljjzf,
                                        czjmjbyljjzf,cxjmjjyljjzf,eyzxyljjzf,lhjzxyljjzf,ylbjzf,qtjjzf,gsjjzf,syjjzf,xxzf,zzzyzffy,
                                        jrtczf,jrdbzf,gwybcylzf,cxjfy,zzdwfd,tcddwfd,jrtcfy,jrdebxfy,zhzfbz,gwybzjbylzf,
                                        gwybzdbylzf,jsrq,jbr,sysdate,jylsh) values(
                                        '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}',
                                        '{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}',
                                        '{20}','{21}','{22}','{23}','{24}','{25}','{26}','{27}','{28}','{29}',
                                        '{30}','{31}','{32}','{33}','{34}','{35}','{36}','{37}','{38}','{39}',
                                        '{40}','{41}','{42}','{43}','{44}','{45}','{46}','{47}','{48}','{49}',
                                        '{50}','{51}','{52}','{53}','{54}','{55}','{56}','{57}','{58}','{59}',
                                        '{60}','{61}','{62}','{63}','{64}')",
                                        jzlsh, djh, op.Yllb, bzbh, bzmc, op.Grbh, op.Kh, op.Xm, op.Ybjzlsh, op.Jslsh,
                                        op.Jslsh, op.Xzzwjb, op.Dqbh, op.Dqmc, op.Cbsf, op.Yldylb, op.Dwmc, op.Dwlx, op.Cbrq, op.Jsqsrq,
                                        op.Jsjzrq, op.Ylfze, op.Zbxje, op.Tcjjzf, op.Zhzf, op.Xjzf, op.Dejjzf, op.Gwybzjjzf, op.Qybcylbxjjzf, op.Lxgbdttcjjzf,
                                        op.Mzjzfy, op.Dwfdfy, op.Yyfdfy, op.Bcjsqzhye, op.Jlfy, op.Ylfy, op.Blfy, op.Zffy, op.Qfbzfy, op.Zgjbyljjzf,
                                        op.Jzjmjbyljjzf, op.Cxjmjbyljjzf, op.Eyzxyljjzf, op.Lhjzxyljjzf, op.Ylbjzf, op.Qtjjzf, op.Gsjjzf, op.Syjjzf, op.Xxzf, op.Zzzyzffy,
                                        op.Tcfdzffy, op.Defdzffy, op.Gwybcylzf, op.Cxjfy, op.Zzdwfd, op.Tcddwfd, op.Jrtcfy, op.Jrdebxfy, op.Zhdkxjzfbz, op.Gwybcjbylzf,
                                        op.Gwybzdbylzf, op.Jsrq + op.Jssj, jbr, sysdate, jylsh);
                liSQL.Add(strSql);
                object[] obj = liSQL.ToArray();
                obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);
                if (obj[1].ToString() == "1")
                {
                    WriteLog(sysdate + "  " + jzlsh + " 进入住院收费预结算成功" + strValue);
                    return new object[] { 0, 1, strValue };
                }
                else
                {
                    WriteLog(sysdate + "  " + jzlsh + " 进入住院收费预结算成功|操作本地数据失败|" + obj[2].ToString());
                }

                WriteLog(sysdate + "  " + jzlsh + " 进入住院收费预结算成功" + strValue);
                return new object[] { 0, 1, strValue };
            }
            else
            {
                WriteLog(sysdate + "  " + jzlsh + " 进入住院收费预结算失败" + retMsg.ToString());
                return new object[] { 0, 0, "住院收费预结算失败|" + retMsg.ToString() };
            }
        }
        #endregion

        #region 住院收费结算
        public static object[] YBZYSFJS(object[] objParam)
        {
            string sysdate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string Ywlx = "JS16";
            try
            {
                string jzlsh = objParam[0].ToString();      //就诊流水号
                string djh = objParam[1].ToString();        //单据号（发票号）
                string cyyy = objParam[2].ToString();       //出院原因
                string zhsybz = objParam[3].ToString();     //账户使用标志
                string ztjsbz = objParam[4].ToString();     //中途结算标志
                string jsrqsj = objParam[5].ToString();     //结算日期时间
                string sfdypj = "TRUE";     //是否打印票据
                string cyzd = "";       //出院诊断
                string cyrqsj = objParam[6].ToString();     //出院日期时间
                string sfje = objParam[7].ToString();       //收费金额
                string bzbh = "";
                string bzmc = "";
                string cfqsrq = "NULL";
                string cfjzrq = "NULL";
                string cyrq = "";
                string cysj = "";
                double sfje2 = 0.000;
                string bxh = "";
                string xm = "";
                string kh = "";
                string ybjzlsh = "";
                string yllb = "";
                string sylb = "";
                string jhsylb = "";
                string jbr = CliUtils.fLoginUser;
                string jylsh = DateTime.Now.ToString("yyyyMMddHHmmss") + "-" + new Random().Next(100).ToString().PadLeft(4, '0');
                if (string.IsNullOrEmpty(jzlsh))
                    return new object[] { 0, 0, "就诊流水号不能为空" };
                if (string.IsNullOrEmpty(cyyy))
                    return new object[] { 0, 0, "出院原因不能为空" };
                if (string.IsNullOrEmpty(cyrqsj))
                    return new object[] { 0, 0, "出院日期时间不能为空" };
                if (string.IsNullOrEmpty(sfdypj))
                    return new object[] { 0, 0, "是否打印票据不能为空" };
                if (string.IsNullOrEmpty(sfje))
                    return new object[] { 0, 0, "收费金额不能为空" };

                #region 时间
                try
                {
                    DateTime dt_cyrq = Convert.ToDateTime(cyrqsj);
                    cyrq = dt_cyrq.ToString("yyyyMMdd");
                    cysj = dt_cyrq.ToString("HHmm");
                }
                catch
                {
                    return new object[] { 0, 0, "出院日期时间格式错误" };
                }
                #endregion

                #region 金额有效
                try
                {
                    sfje2 = Convert.ToDouble(sfje);
                }
                catch
                {
                    return new object[] { 0, 0, "收费金额数据格式有误" };
                }
                #endregion

                #region 是否办理医保登记
                string strSql = string.Format("select * from ybmzzydjdr where jzlsh='{0}' and cxbz=1", jzlsh);
                DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                if (ds.Tables[0].Rows.Count == 0)
                    return new object[] { 0, 0, "该患者未办理医保登记" };
                else
                {
                    bxh = ds.Tables[0].Rows[0]["grbh"].ToString();
                    xm = ds.Tables[0].Rows[0]["xm"].ToString();
                    kh = ds.Tables[0].Rows[0]["kh"].ToString();
                    dqbh = ds.Tables[0].Rows[0]["tcqh"].ToString();
                    ybjzlsh = ds.Tables[0].Rows[0]["ybjzlsh"].ToString();
                    bzbh = ds.Tables[0].Rows[0]["bzbm"].ToString();
                    bzmc = ds.Tables[0].Rows[0]["bzmc"].ToString();
                    yllb = ds.Tables[0].Rows[0]["yllb"].ToString();
                    sylb = ds.Tables[0].Rows[0]["sylb"].ToString();
                    jhsylb = ds.Tables[0].Rows[0]["jhsylb"].ToString();
                }
                #endregion

                #region 费用总额
                strSql = string.Format("select SUM(je) from ybcfmxscfhdr where jzlsh='{0}' and ybjzlsh='{1}' and cxbz=1", jzlsh, ybjzlsh);
                ds.Tables.Clear();
                ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                if (string.IsNullOrEmpty(ds.Tables[0].Rows[0][0].ToString()))
                    return new object[] { 0, 0, "费用明细未上传" };

                double sfje3 = Convert.ToDouble(ds.Tables[0].Rows[0][0].ToString());
                if (Math.Abs(sfje2 - sfje3) > 1.0)
                    return new object[] { 0, 0, "总费用与医保结算总费用相差" + Math.Abs(sfje2 - sfje3) + ",无法结算，请核实!" };
                #endregion

                ds.Dispose();
                StringBuilder inputData = new StringBuilder();

                /*
            1 社会保险号 VARCHAR2(20) 否
            2 社会保障卡号 VARCHAR2(20) 否
            3 姓名 VARCHAR2(20) 否
            4 地区编码 
            5 中心业务流水号 VARCHAR2(20) 否 门诊号
            6 医疗类别 VARCHAR2(3) 代码（详见字典说明）
            7 特病种编码 VARCHAR2(20) 多病种以分隔符“#”号连接
            8 特病种名称 VARCHAR2(100) 多病种以分隔符“#”号连接
            9 结算费用起始日期 VARCHAR2(8) YYYYMMDD，可以为NULL值，不指定日期表示结算所有未结算费用。
            10 结算费用截止日期 VARCHAR2(8)
            11 个人账户抵扣现金支付标志 VARCHAR2(3) 0-不予支付；1-支付（默认 1）（政策允许情况下才有效）
            12 生育类别 VARCHAR2(3) 编码，见“字典说明”部分。“医疗类别”为“51-生育门诊”、“52-生育住院”时，第14、15项至少输入一项。 
            13 计划生育类别 VARCHAR2(3)
            14 地区编号 VARCHAR2(6) 否 统筹区编码
            15 出院日期
            16 出院时间
            17 出院原因
         */
                inputData.Append("SSSS|");
                inputData.Append(bxh + "|");
                inputData.Append(kh + "|");
                inputData.Append(xm + "|");
                inputData.Append(dqbh + "|");
                inputData.Append(ybjzlsh + "|");
                inputData.Append(yllb + "|");
                inputData.Append(bzbh + "|");
                inputData.Append(bzmc + "|");
                inputData.Append(cfqsrq + "|");
                inputData.Append(cfjzrq + "|");
                inputData.Append(zhsybz + "|");
                inputData.Append(sylb + "|");
                inputData.Append(jhsylb + "|");
                inputData.Append(cyrq + "|");
                inputData.Append(cysj + "|");
                inputData.Append(cyyy + "|");
                inputData.Append("ZZZZ");
                StringBuilder outData = new StringBuilder(10240);
                StringBuilder retMsg = new StringBuilder(1024);

                WriteLog(sysdate + "  " + jzlsh + " 进入住院收费结算...");
                WriteLog(sysdate + " 入参|" + inputData.ToString());
                int i = f_UserBargaingApply(Ywlx, inputData, outData, retMsg);
                if (i > 0)
                {
                    List<string> liSQL = new List<string>();

                    #region 出参
                    WriteLog(sysdate + "  " + jzlsh + " 出参|" + outData.ToString());
                    /*出参
                    1 社会保险号 VARCHAR2(20)
                    2 社会保障卡号 VARCHAR2(20)
                    3 姓名 VARCHAR2(20)
                    4 性别 VARCHAR2(10) 名称
                    5 出生日期 VARCHAR2(8) YYYYMMDD
                    6 地区编号 VARCHAR2(6) 统筹区编码
                    7 地区名称 VARCHAR2(20)
                    8 参保日期 VARCHAR2(8) YYYYMMDD
                    9 参保身份 VARCHAR2(50) 格式“编码-名称”，如“1101-企业一般人员”。见“字典说明”
                    10 行政职务级别 VARCHAR2(50) 格式“编码-名称”，如“121-厅局级正职”。见“字典说明”
                    11 医疗待遇享受类别 VARCHAR2(50) 格式“编码-名称”，如“1101-统账在职”。见“字典说明”（仅适用于新余市）
                    12 单位名称 VARCHAR2(100) 格式“单位编码-单位名称”， 如“666666-测试单位”
                    13 单位类型 VARCHAR2(20) 格式“编码-名称”，如“100-正常企业”。见“字典说明”
                    14 中心业务流水号 VARCHAR2(20) 住院号
                    15 中心结算单据号 VARCHAR2(20)
                    16 医疗类别 VARCHAR2(50) 名称
                    17 结算费用起始日期 VARCHAR2(8)
                    18 结算费用截止日期 VARCHAR2(8)
                    19 医疗总费用 NUMBER(16,2)
                    20 本次账户支付 NUMBER(16,2)
                    21 本次现金支付 NUMBER(16,2)
                    22 职工基本医疗基金支付 NUMBER(16,2)
                    23 城镇居民基本医疗基金支付 NUMBER(16,2)
                    24 城乡居民基本医疗基金支付 NUMBER(16,2)
                    25 大病医疗基金支付 NUMBER(16,2)
                    26 公务员补充医疗基金支付 NUMBER(16,2)
                    27 企业补充医疗基金支付 NUMBER(16,2)
                    28 二乙专项医疗基金支付 NUMBER(16,2)
                    29 老红军专项医疗基金支付 NUMBER(16,2)
                    30 离休干部单独统筹基金支付 NUMBER(16,2)
                    31 医疗保健支付 NUMBER(16,2) 如新余地厅级干部医疗保健补差
                    32 其他基金支付 NUMBER(16,2) 预留指标项。（抚州市：医疗补贴金额。已计入统筹及大病基金中，此为其中项。）
                    33 工伤基金支付 NUMBER(16,2)
                    34 生育基金支付 NUMBER(16,2)
                    35 民政救助支付 NUMBER(16,2)
                    36 单位负担支付 NUMBER(16,2)
                    37 定点机构支付 NUMBER(16,2)
                    38 医保账户余额 NUMBER(16,2)
                    39 甲类费用 NUMBER(16,2)
                    40 乙类费用 NUMBER(16,2)
                    41 丙类费用 NUMBER(16,2) 抚州指“乙类*”
                    42 自费费用 NUMBER(16,2)
                    43 先行自付 NUMBER(16,2)
                    44 起付段自付 NUMBER(16,2)
                    45 转外自付 NUMBER(16,2)
                    46 统筹段自付 NUMBER(16,2)
                    47 大病统筹自付 NUMBER(16,2)
                    48 公务员补充医疗自付 NUMBER(16,2)
                    49 超过最高封顶线自付 NUMBER(16,2)
                    50 转诊单位负担 NUMBER(16,2)
                    51 统筹段单位负担 NUMBER(16,2)
                    52 大病段单位负担 NUMBER(16,2)
                    53 统筹段基金支付 NUMBER(16,2)
                    54 大病段基金支付 NUMBER(16,2)
                    55 个人账户抵扣现金支付标志 VARCHAR2(3) 0-不予支付；1-支付（默认 1）
                    56 公务员补助基本医疗支付 NUMBER(16,2) 新余：指补助 65%部分。其他：补助统筹段支付
                    57 公务员补助大病医疗支付 NUMBER(16,2) 新余：指补助 80%部分。其他：补助大病段支付
                    58 结算人 VARCHAR2(20)
                    59 结算日期 VARCHAR2(8) YYYYMMDD
                    60 结算时间 VARCHAR2(6) HH24MISS
                        * 
                    返回的附加消息包：费用明细，多记录往后续加消息包。*门诊返回，住院不返回。

                    1 社保三大目录编码 VARCHAR2(20)
                    2 社保三大目录名称 VARCHAR2(150)
                    3 收费项目等级 VARCHAR2(20) 格式“编码-名称”，如“1-甲类”。见“字典说明”
                    4 发票项目类别 VARCHAR2(20) 格式“编码-名称”，如“11-西药”。见“字典说明”
                    5 单价 NUMBER(10,4)
                    6 数量 NUMBER(8,2)
                    */
                    string[] str = outData.ToString().Replace("RRRR|", "").Split(';');
                    string[] str2 = str[0].Split('|');
                    outParams_js op = new outParams_js();
                    op.Grbh = str2[0].Trim();       //保险号
                    op.Kh = str2[1].Trim();         //卡号
                    op.Xm = str2[2].Trim();         //姓名
                    op.Xb = str2[3].Trim();         //性别
                    op.Csrq = str2[4].Trim();       //出生日期
                    op.Dqbh = str2[5].Trim();      //地区编号
                    op.Dqmc = str2[6].Trim();       //地区名称
                    op.Cbrq = str2[7].Trim();       //参保日期
                    op.Cbsf = str2[8].Trim();       //参保身份
                    op.Xzzwjb = str2[9].Trim();     //行政职务级别
                    op.Yldylb = str2[10].Trim();    //医疗待遇类别
                    op.Dwmc = str2[11].Trim();      //单位名称
                    op.Dwlx = str2[12].Trim();      //单位类别
                    op.Ybjzlsh = str2[13].Trim();   //中心业务流水号，医保就诊流水号
                    op.Jslsh = str2[14].Trim();     //结算单据号
                    op.Yllb = str2[15].Trim();      //医疗类别
                    op.Jsqsrq = str2[16].Trim();    //结算起始日期
                    op.Jsjzrq = str2[17].Trim();    //结算截止日期
                    op.Ylfze = str2[18].Trim();     //医疗总费用
                    op.Zhzf = str2[19].Trim();      //本次帐户支付
                    op.Xjzf = str2[20].Trim();      //本次现金支付
                    op.Zgjbyljjzf = str2[21].Trim();
                    op.Jzjmjbyljjzf = str2[22].Trim();
                    op.Cxjmjbyljjzf = str2[23].Trim();
                    op.Dejjzf = str2[24].Trim();
                    op.Gwybzjjzf = str2[25].Trim();
                    op.Qybcylbxjjzf = str2[26].Trim();
                    op.Eyzxyljjzf = str2[27].Trim();
                    op.Lhjzxyljjzf = str2[28].Trim();
                    op.Lxgbdttcjjzf = str2[29].Trim();
                    op.Ylbjzf = str2[30].Trim();
                    op.Qtjjzf = str2[31].Trim();
                    op.Gsjjzf = str2[32].Trim();
                    op.Syjjzf = str2[33].Trim();
                    op.Mzjzfy = str2[34].Trim();
                    op.Dwfdfy = str2[35].Trim();
                    op.Yyfdfy = str2[36].Trim();
                    op.Bcjshzhye = str2[37].Trim();
                    op.Jlfy = str2[38].Trim();
                    op.Ylfy = str2[39].Trim();
                    op.Blfy = str2[40].Trim();
                    op.Zffy = str2[41].Trim();
                    op.Xxzf = str2[42].Trim();      //先行自付
                    op.Qfbzfy = str2[43].Trim();
                    op.Zzzyzffy = str2[44].Trim();
                    op.Tcfdzffy = str2[45].Trim();
                    op.Defdzffy = str2[46].Trim();
                    op.Gwybcylzf = str2[47].Trim();
                    op.Cgzgfd = str2[48].Trim();
                    op.Zzdwfd = str2[49].Trim();
                    op.Tcddwfd = str2[50].Trim();
                    op.Dbddwfd = str2[51].Trim();
                    op.Jrtcfy = str2[52].Trim();
                    op.Jrdebxfy = str2[53].Trim();
                    op.Zhdkxjzfbz = str2[54].Trim();
                    op.Gwybcjbylzf = str2[55].Trim();
                    op.Gwybzdbylzf = str2[56].Trim();
                    op.Jbr = str2[57].Trim();
                    op.Jsrq = str2[58].Trim();
                    op.Jssj = str2[59].Trim();
                    op.Bcjsqzhye = (Convert.ToDecimal(op.Bcjshzhye) + Convert.ToDecimal(op.Zhzf)).ToString();
                    op.Zbxje = (Convert.ToDecimal(op.Ylfze) - Convert.ToDecimal(op.Xjzf)).ToString();
                    op.Tcjjzf = (Convert.ToDecimal(op.Ylfze) - Convert.ToDecimal(op.Xjzf) - Convert.ToDecimal(op.Zhzf)
                                - Convert.ToDecimal(op.Dejjzf) - Convert.ToDecimal(op.Mzjzfy) - Convert.ToDecimal(op.Yyfdfy)).ToString();

                    op.Cxjfy = "0.00";
                    op.Djh = op.Jslsh;
                    #endregion

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
                    string strValue = op.Ylfze + "|" + op.Zbxje + "|" + op.Tcjjzf + "|" + op.Dejjzf + "|" + op.Zhzf + "|" +
                                     op.Xjzf + "|" + op.Gwybzjjzf + "|" + op.Qybcylbxjjzf + "|" + op.Zffy + "|" + op.Dwfdfy + "|" +
                                     op.Yyfdfy + "|" + op.Mzjzfy + "|" + op.Cxjfy + "|" + op.Ylzlfy + "|" + op.Blzlfy + "|" +
                                     op.Fhjjylfy + "|" + op.Qfbzfy + "|" + op.Zzzyzffy + "|" + op.Jrtcfy + "|" + op.Tcfdzffy + "|" +
                                     op.Ctcfdxfy + "|" + op.Jrdebxfy + "|" + op.Defdzffy + "|" + op.Cdefdxfy + "|" + op.Rgqgzffy + "|" +
                                     op.Bcjsqzhye + "|" + op.Bntczflj + "|" + op.Bndezflj + "|" + op.Bnczjmmztczflj + "|" + op.Bngwybzzflj + "|" +
                                     op.Bnzhzflj + "|" + op.Bnzycslj + "|" + op.Zycs + "|" + op.Xm + "|" + op.Jsrq + op.Jssj + "|" +
                                     op.Yllb + "|" + op.Yldylb + "|" + op.Jbjgbm + "|" + op.Ywzqh + "|" + op.Jslsh + "|" +
                                     op.Tsxx + "|" + op.Djh + "|" + op.Jyxl + "|" + op.Yyjylsh + "|" + op.Yxbz + "|" +
                                     op.Grbhgl + "|" + op.Yljgbm + "|" + op.Ecbcje + "|" + op.Mmqflj + "|" + op.Jsfjylsh + "|" +
                                     op.Grbh + "|" + op.Dbzbc + "|" + op.Czzcje + "|" + op.Elmmxezc + "|" + op.Elmmxesy + "|" +
                                     op.Jmgrzfecbcje + "|" + op.Tjje + "|" + op.Syjjzf + "|";

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
                    strSql = string.Format(@"insert into ybfyjsdr(
                                        jzlsh,djhin,yllb,bzbm,bzmc,grbh,kh,xm,ybjzlsh,djh,
                                        jslsh,xzzwjb,dqbh,dqmc,cbsf,yldyxslb,dwmc,dwlx,cbrq,jsqsrq,
                                        jsjzrq,ylfze,zbxje,tcjjzf,zhzf,xjzf,dejjzf,gwybzjjzf,qybcylbxjjzf,lxgbddtczf,
                                        mzjzfy,dwfdfy,yyfdfy,bcjsqzhye,jlfy,ylfy,blfy,zffy,qfbzfy,zgjbyljjzf,
                                        czjmjbyljjzf,cxjmjjyljjzf,eyzxyljjzf,lhjzxyljjzf,ylbjzf,qtjjzf,gsjjzf,syjjzf,xxzf,zzzyzffy,
                                        jrtczf,jrdbzf,gwybcylzf,cxjfy,zzdwfd,tcddwfd,jrtcfy,jrdebxfy,zhzfbz,gwybzjbylzf,
                                        gwybzdbylzf,jsrq,jbr,sysdate,jylsh,cyrq,cyyy) values(
                                        '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}',
                                        '{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}',
                                        '{20}','{21}','{22}','{23}','{24}','{25}','{26}','{27}','{28}','{29}',
                                        '{30}','{31}','{32}','{33}','{34}','{35}','{36}','{37}','{38}','{39}',
                                        '{40}','{41}','{42}','{43}','{44}','{45}','{46}','{47}','{48}','{49}',
                                        '{50}','{51}','{52}','{53}','{54}','{55}','{56}','{57}','{58}','{59}',
                                        '{60}','{61}','{62}','{63}','{64}','{65}','{66}')",
                                        jzlsh, djh, op.Yllb, bzbh, bzmc, op.Grbh, op.Kh, op.Xm, op.Ybjzlsh, djh,
                                        op.Jslsh, op.Xzzwjb, op.Dqbh, op.Dqmc, op.Cbsf, op.Yldylb, op.Dwmc, op.Dwlx, op.Cbrq, op.Jsqsrq,
                                        op.Jsjzrq, op.Ylfze, op.Zbxje, op.Tcjjzf, op.Zhzf, op.Xjzf, op.Dejjzf, op.Gwybzjjzf, op.Qybcylbxjjzf, op.Lxgbdttcjjzf,
                                        op.Mzjzfy, op.Dwfdfy, op.Yyfdfy, op.Bcjsqzhye, op.Jlfy, op.Ylfy, op.Blfy, op.Zffy, op.Qfbzfy, op.Zgjbyljjzf,
                                        op.Jzjmjbyljjzf, op.Cxjmjbyljjzf, op.Eyzxyljjzf, op.Lhjzxyljjzf, op.Ylbjzf, op.Qtjjzf, op.Gsjjzf, op.Syjjzf, op.Xxzf, op.Zzzyzffy,
                                        op.Tcfdzffy, op.Defdzffy, op.Gwybcylzf, op.Cxjfy, op.Zzdwfd, op.Tcddwfd, op.Jrtcfy, op.Jrdebxfy, op.Zhdkxjzfbz, op.Gwybcjbylzf,
                                        op.Gwybzdbylzf, op.Jsrq + op.Jssj, jbr, sysdate, jylsh, cyrqsj, cyyy);
                    liSQL.Add(strSql);
                    object[] obj = liSQL.ToArray();
                    obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);
                    if (obj[1].ToString() == "1")
                    {
                        WriteLog(sysdate + "  " + jzlsh + " 进入住院收费结算成功" + strValue);
                        return new object[] { 0, 1, strValue };
                        //出院登记
                        //object[] objParam_cy = { jzlsh, djh, cyyy, cyrqsj };
                        //objParam_cy=YBCYDJ(objParam_cy);
                        //if (objParam_cy[1].ToString().Equals("1"))
                        //else
                        //{
                        //    //结算撤销
                        //    object[] objParam_cx = { jzlsh, djh, "0" };
                        //    YBZYSFJSCX(objParam_cx);
                        //    return objParam_cy;
                        //}
                    }
                    else
                    {
                        WriteLog(sysdate + "  " + jzlsh + " 进入住院收费结算成功|操作本地数据失败|" + obj[2].ToString());
                        //住院收费结算撤销
                        object[] objParam2 = new object[] { jzlsh, op.Jslsh };
                        NYBZYSFJSCX(objParam2);

                        return new object[] { 0, 0, "住院收费结算失败" };
                    }
                }
                else
                {
                    WriteLog(sysdate + "  " + jzlsh + " 进入住院收费结算失败" + retMsg.ToString());
                    return new object[] { 0, 0, "住院收费预结算失败|" + retMsg.ToString() };
                }
            }
            catch (Exception ex)
            {
                WriteLog(sysdate + "  住院结算失败" + ex.Message);
                return new object[] { 0, 0, "住院结算失败" };
            }
        }
        #endregion

        #region 住院收费结算撤销
        public static object[] YBZYSFJSCX(object[] objParam)
        {
            string sysdate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string Ywlx = "JS17";
            string jzlsh = objParam[0].ToString();
            string djh = objParam[1].ToString();
            string dkbz = objParam[2].ToString(); //读卡标志
            string bxh = "";
            string xm = "";
            string kh = "";
            string ybjzlsh = "";
            string djlsh = "";
            string cyrqsj = "";
            string cyyy = "";
            string cyzt = "";
            string sfmxsfzf = "TRUE";

            if (string.IsNullOrEmpty(jzlsh))
                return new object[] { 0, 0, "就诊流水号不能为空" };
            if (string.IsNullOrEmpty(djh))
                return new object[] { 0, 0, "发票号不能为空" };

            string strSql = string.Format("select * from ybmzzydjdr where jzlsh='{0}' and cxbz=1", jzlsh);
            DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            if (ds.Tables[0].Rows.Count == 0)
                return new object[] { 0, 0, "该患者未办理医保登记" };
            else
            {
                bxh = ds.Tables[0].Rows[0]["grbh"].ToString();
                xm = ds.Tables[0].Rows[0]["xm"].ToString();
                kh = ds.Tables[0].Rows[0]["kh"].ToString();
                dqbh = ds.Tables[0].Rows[0]["tcqh"].ToString();
                ybjzlsh = ds.Tables[0].Rows[0]["ybjzlsh"].ToString();

            }
            strSql = string.Format("select jslsh,cyrq,cyyy,cyzt from ybfyjsdr where jzlsh='{0}' and djh='{1}' and cxbz=1", jzlsh, djh);
            ds.Tables.Clear();
            ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            if (ds.Tables[0].Rows.Count == 0)
                return new object[] { 0, 0, "该患者未办理住院收费结算" };
            else
            {
                djlsh = ds.Tables[0].Rows[0]["jslsh"].ToString();
                cyrqsj = ds.Tables[0].Rows[0]["cyrq"].ToString();
                cyyy = ds.Tables[0].Rows[0]["cyyy"].ToString();
                cyzt = ds.Tables[0].Rows[0]["cyzt"].ToString();
            }
            ds.Dispose();
            //object[] objParam_cy = { jzlsh, djh, cyyy, cyrqsj };
            //if (cyzt.Equals("1"))
            //{
            //    //出院召回（出院登记撤销)
            //    objParam_cy = YBCYDJCX(objParam_cy);
            //    if (objParam_cy[1].ToString().Equals("0"))
            //        return objParam_cy;
            //}

            StringBuilder inputData = new StringBuilder();
            //入参:
            inputData.Append("SSSS|");
            inputData.Append(bxh + "|");
            inputData.Append(kh + "|");
            inputData.Append(xm + "|");
            inputData.Append(dqbh + "|");
            inputData.Append(ybjzlsh + "|");
            //inputData.Append(djlsh + "|");
            inputData.Append(sfmxsfzf + "|");
            inputData.Append("ZZZZ");

            StringBuilder outData = new StringBuilder(1024);
            StringBuilder retMsg = new StringBuilder(1024);
            WriteLog(sysdate + "  " + jzlsh + " 进入住院收费结算撤销...");
            WriteLog(sysdate + " 入参|" + inputData.ToString());
            int i = f_UserBargaingApply(Ywlx, inputData, outData, retMsg);
            if (i > 0)
            {
                List<string> liSQL = new List<string>();
                strSql = string.Format(@"insert into ybfyjsdr(jzlsh,jylsh,djhin,cyrq,cyyy,zhsybz,ztjsbz,jbr,xm,kh,
                                        grbh,jsrq,yllb,yldylb,jslsh,ylfze,zhzf,xjzf,tcjjzf,dejjzf,
                                        mzjzfy,dwfdfy,lxgbddtczf,jlfy,ylfy,blfy,zffy,qfbzfy,fybzf,ylypzf,
                                        tjtzzf,jrtczf,jrdbzf,zdjbfwnbcje,zdjbfwybcje,yyfdfy,ecbcje,mzdbjzje,czzcje,gwybzjjzf,
				                        djh,ryrq,bcjsqzhye,sysdate,cxbz)  select 
                                        jzlsh,jylsh,djhin,cyrq,cyyy,zhsybz,ztjsbz,jbr,xm,kh,
                                        grbh,jsrq,yllb,yldylb,jslsh,ylfze,zhzf,xjzf,tcjjzf,dejjzf,
                                        mzjzfy,dwfdfy,lxgbddtczf,jlfy,ylfy,blfy,zffy,qfbzfy,fybzf,ylypzf,
                                        tjtzzf,jrtczf,jrdbzf,zdjbfwnbcje,zdjbfwybcje,yyfdfy,ecbcje,mzdbjzje,czzcje,gwybzjjzf,
				                        djh,ryrq,bcjsqzhye,'{2}',0 from ybfyjsdr where jzlsh='{0}' and jslsh='{1}' and cxbz=1", jzlsh, djlsh, sysdate);
                liSQL.Add(strSql);
                strSql = string.Format("update ybfyjsdr set cxbz = 2 where jzlsh = '{0}' and jslsh = '{1}' and cxbz = 1 ", jzlsh, djlsh);
                liSQL.Add(strSql);
                strSql = string.Format(@"delete from  ybfyyjsdr where jzlsh='{0}' and cxbz=1", jzlsh);
                liSQL.Add(strSql);
                strSql = string.Format(@"insert into ybcfmxscfhdr(jzlsh,jylsh,ybjzlsh,xm,kh,yybxmbh,ybxmmc,yyxmdm,yyxmmc,sfxmdj,
                                        sflb,dj,sl,je,zfje,zlje,cxjzfje,bz,ybcfh,cfh,grbh,
                                        ybsequ,yysequ,sysdate,cxbz) select 
                                        jzlsh,jylsh,ybjzlsh,xm,kh,yybxmbh,ybxmmc,yyxmdm,yyxmmc,sfxmdj,
                                        sflb,dj,sl,je,zfje,zlje,cxjzfje,bz,ybcfh,cfh,grbh,
                                        ybsequ,yysequ,'{2}',0 from ybcfmxscfhdr 
                                        where jzlsh='{0}' and ybjzlsh='{1}' and cxbz=1 ", jzlsh, ybjzlsh, sysdate);
                liSQL.Add(strSql);
                strSql = string.Format("update ybcfmxscfhdr set cxbz=2 where jzlsh='{0}' and ybjzlsh='{1}' and cxbz=1", jzlsh, ybjzlsh);
                liSQL.Add(strSql);
                strSql = string.Format("update zy03d set z3ybup = null where z3ybup is not null and z3zyno = '{0}' and left(z3kind, 1) = '2'", jzlsh);
                liSQL.Add(strSql);

                object[] obj = liSQL.ToArray();
                obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);
                if (obj[1].ToString() == "1")
                {
                    WriteLog(sysdate + "  " + jzlsh + " 进入住院收费结算撤销成功" + outData.ToString());
                    return new object[] { 0, 1, "住院收费结算撤销成功", outData.ToString() };
                }
                else
                {
                    WriteLog(sysdate + "  " + jzlsh + " 进入住院收费结算撤销成功|操作本地数据失败" + obj[2].ToString());
                    return new object[] { 0, 0, "住院收费结算撤销失败|" + obj[2].ToString() };
                }
            }
            else
            {
                WriteLog(sysdate + "  " + jzlsh + " 进入住院收费结算撤销失败" + retMsg.ToString());
                return new object[] { 0, 0, "住院收费结算撤销失败|" + retMsg.ToString() };
            }
        }
        #endregion

        #region 就诊疾病诊断登记
        public static object[] YBZDDJ(object[] objParam)
        {
            string sysdate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string Ywlx = "DJ21";
            string jzlsh = objParam[0].ToString();  //就诊流水号
            string zdlx = objParam[1].ToString();   //诊断类型 //疾病诊断类型 1-门诊诊断 2-入院诊断 3-出院诊断 9-其他
            string dgysbm = objParam[2].ToString(); //诊断医生编码（开方医生)
            string dgysxm = objParam[3].ToString(); //医生姓名
            string bzbm = objParam[4].ToString();   //病种编码
            string bzmc = objParam[5].ToString();   //病种名称

            if (string.IsNullOrEmpty(bzbm))
            {
                string strSql = string.Format(@"update ybmzzydjdr set ysdm='{1}',ysxm='{2}',dgysdm='{1}',dgysxm='{2}' where jzlsh='{0}' and cxbz=1", jzlsh, dgysbm, dgysxm);
                object[] objSql = { strSql };
                objSql = CliUtils.CallMethod("sybdj", "BatExecuteSql", objSql);
                return new object[] { 0,1,"就诊疾病诊断登记成功"};
            }

            if (string.IsNullOrEmpty(jzlsh))
                return new object[] { 0, 0, "就诊流水号不能为空" };
            if (string.IsNullOrEmpty(zdlx))
                return new object[] { 0, 0, "诊断类型不能为空" };
            if (string.IsNullOrEmpty(dgysbm))
                return new object[] { 0, 0, "诊断医生不能为空" };


            WriteLog(sysdate + "  " + jzlsh + " 进入就诊疾病诊断登记...");

            try
            {
                string strSql = string.Format(@"select * from ybmzzydjdr where jzlsh='{0}' and cxbz=1", jzlsh);
                DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                if (ds.Tables[0].Rows.Count == 0)
                    return new object[] { 0, 0, "该患者未进行医保登记" };

                string grbh = ds.Tables[0].Rows[0]["grbh"].ToString();
                string kh = ds.Tables[0].Rows[0]["kh"].ToString();
                string xm = ds.Tables[0].Rows[0]["xm"].ToString();
                string ybjzlsh = ds.Tables[0].Rows[0]["ybjzlsh"].ToString(); //中心业务流水号
                string jbzdlx = zdlx; 
                string tcqh = ds.Tables[0].Rows[0]["tcqh"].ToString();
                //string bzbm = ds.Tables[0].Rows[0]["bzbm"].ToString();
                //string bzmc = ds.Tables[0].Rows[0]["bzmc"].ToString();
                ds.Dispose();
                /*
                1 社会保险号 VARCHAR2(20) 否
                2 社保保障卡号 VARCHAR2(20) 否
                3 姓名 VARCHAR2(20) 否
                4 中心业务流水号
                VARCHAR2(20) 否 门诊号、住院号
                5 疾病诊断类型 VARCHAR2(3) 否 代码（详见字典说明）
                6疾病诊断医生编码 VARCHAR2(20) 否 定岗医师\药师编号，同DJ13
                7疾病诊断医生姓名 VARCHAR2(20) 否
                8 地区编号 VARCHAR2(6) 否 统筹区编码
 
                  发送的附加消息包：多记录往后续加消息包
                1 疾病诊断编码 VARCHAR2(20) 否
                2 疾病诊断名称 VARCHAR2(100) 否
                */
                StringBuilder inputData = new StringBuilder();
                inputData.Append("SSSS|");
                inputData.Append(grbh + "|");
                inputData.Append(kh + "|");
                inputData.Append(xm + "|");
                inputData.Append(ybjzlsh + "|");
                inputData.Append(jbzdlx + "|");
                inputData.Append(dgysbm + "|");
                inputData.Append(dgysxm + "|");
                inputData.Append(tcqh + ";");
                inputData.Append(bzbm + "|");
                inputData.Append(bzmc + "|");
                inputData.Append("ZZZZ");
                StringBuilder outData = new StringBuilder(1024);
                StringBuilder retMsg = new StringBuilder(1024);

                WriteLog(sysdate + "  入参|" + inputData.ToString());
                int i = f_UserBargaingApply(Ywlx, inputData, outData, retMsg);
              
                if (i > 0)
                {
                    strSql = string.Format(@"update ybmzzydjdr set ysdm='{1}',ysxm='{2}',dgysdm='{1}',dgysxm='{2}' where jzlsh='{0}' and cxbz=1", jzlsh, dgysbm, dgysxm);
                    object[] objSql = { strSql };
                    objSql = CliUtils.CallMethod("sybdj", "BatExecuteSql", objSql);
                    WriteLog(sysdate + "  出参|" + outData.ToString());
                    WriteLog(sysdate + "  就诊疾病诊断登记成功");
                    return new object[] { 0, 1, "就诊疾病诊断登记成功" };
                }
                else
                {
                    WriteLog(sysdate + "  就诊疾病诊断登记失败");
                    return new object[] { 0, 0, "就诊疾病诊断登记失败" };
                }
            }
            catch (Exception ex)
            {
                WriteLog(sysdate + "  就诊疾病诊断登记异常|" + ex.Message);
                return new object[] { 0, 0, "就诊疾病诊断登记异常|" + ex.Message };
            }
        }
        #endregion

        #region 就诊疾病诊断登记撤销
        public static object[] YBZDDJCX(object[] objParam)
        {
            string sysdate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string Ywlx = "DJ22";
            string jzlsh = objParam[0].ToString(); 
            string zdlx = objParam[1].ToString();   //诊断类型 //疾病诊断类型 1-门诊诊断 2-入院诊断 3-出院诊断 9-其他

            if (string.IsNullOrEmpty(jzlsh))
                return new object[] { 0, 0, "就诊流水号不能为空" };
            if (string.IsNullOrEmpty(zdlx))
                return new object[] { 0, 0, "诊断类型不能为空" }; 

            WriteLog(sysdate + "  " + jzlsh + " 进入就诊疾病诊断登记撤销...");

            try
            {
                string strSql = string.Format(@"select * from ybmzzydjdr where jzlsh='{0}' and cxbz=1", jzlsh);
                DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                if (ds.Tables[0].Rows.Count == 0)
                    return new object[] { 0, 0, "该患者未进行医保登记" };

                string grbh = ds.Tables[0].Rows[0]["grbh"].ToString();
                string kh = ds.Tables[0].Rows[0]["kh"].ToString();
                string xm = ds.Tables[0].Rows[0]["xm"].ToString();
                string ybjzlsh = ds.Tables[0].Rows[0]["ybjzlsh"].ToString(); //中心业务流水号
                string jbzdlx = zdlx; //疾病诊断类型 1-门诊诊断 2-入院诊断 3-出院诊断 9-其他
                string cxlx = "0"; //撤销类型 0-全部撤销（默认） 1-指定撤销
                string tcqh = ds.Tables[0].Rows[0]["tcqh"].ToString();
                //string dgysbm = ds.Tables[0].Rows[0]["dgysdm"].ToString();
                //string dgysxm = ds.Tables[0].Rows[0]["dgysxm"].ToString();

                //string bzbm = ds.Tables[0].Rows[0]["bzbm"].ToString();
                //string bzmc = ds.Tables[0].Rows[0]["bzmc"].ToString();

                ds.Dispose();
                /*
                1 社会保险号 VARCHAR2(20) 否
                2 社保保障卡号 VARCHAR2(20) 否
                3 姓名 VARCHAR2(20) 否
                4中心业务流水号 VARCHAR2(20) 否 门诊号、住院号
                5 疾病诊断类型 VARCHAR2(3) 否 代码（详见字典说明）
                6 撤销类型 VARCHAR2(3) 0-全部撤销（默认） 1-指定撤销。
                7 地区编号 VARCHAR2(6) 否 统筹区编码
  
                    发送的附加消息包：非指定撤销时可不传，多记录往后续加消息包
                1 疾病诊断编码 VARCHAR2(20) 否
                2 疾病诊断名称 VARCHAR2(100) 否
                */
                StringBuilder inputData = new StringBuilder();
                inputData.Append("SSSS|");
                inputData.Append(grbh + "|");
                inputData.Append(kh + "|");
                inputData.Append(xm + "|");
                inputData.Append(ybjzlsh + "|");
                inputData.Append(jbzdlx + "|");
                inputData.Append(cxlx + "|");
                inputData.Append(tcqh+"|");
                inputData.Append("ZZZZ");

                StringBuilder outData = new StringBuilder(1024);
                StringBuilder retMsg = new StringBuilder(1024);

                WriteLog(sysdate + "  入参|" + inputData.ToString());
                int i = f_UserBargaingApply(Ywlx, inputData, outData, retMsg);
                if (i > 0)
                {
                    WriteLog(sysdate + "  出参|" + outData.ToString());
                    WriteLog(sysdate + "  就诊疾病诊断登记撤销成功");
                    return new object[] { 0, 1, "就诊疾病诊断登记撤销成功" };
                }
                else
                {
                    WriteLog(sysdate + "  就诊疾病诊断登记撤销失败");
                    return new object[] { 0, 0, "就诊疾病诊断登记撤销失败" };
                }
            }
            catch (Exception ex)
            {
                WriteLog(sysdate + "  就诊疾病诊断登记异常|" + ex.Message);
                return new object[] { 0, 0, "就诊疾病诊断登记异常|" + ex.Message };
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

            string[] syl_mz = { "14", "16" };
            WriteLog("  获取病种信息查询|入参|" + yllb + "|" + grbh + "|" + jzbz + "|" + splb);

            string strSql = string.Empty;
            if (jzbz.ToUpper().Equals("M"))
            {
                strSql = string.Format(@"select dm,dmmc,pym,wbm from ybbzmrdr where 1=1");
                if (syl_mz.Contains(yllb))
                    strSql += string.Format(@" and ybm in(2,3,4)");
                else
                    strSql += string.Format(@" and ybm=1");
            }
            else if (jzbz.ToUpper().Equals("Z"))
            {
                strSql = string.Format(@"select dm,dmmc,pym,wbm from ybbzmrdr where 1=1");
                if (yllb.Equals("21"))
                    strSql += string.Format(@" and ybm in(2,3,4)");
                else
                    strSql += string.Format(@" and ybm=1");
            }
            else
            {
                return new object[] { 0, 0, "门诊住院标志入参有误" };
            }
            DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            WriteLog("获取病种信息查询成功|");
            return new object[] { 0, 1, ds.Tables[0] };
        }
        #endregion

        #region 医疗待遇审批信息查询
        public static object[] YBYLDYSPXXCX(object[] ojbParam)
        {
            string sysdate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            WriteLog(sysdate + "  进入医疗待遇审批信息查询...");
            return new object[] { 0, 1, "无医疗待遇审批信息查询接口" };
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

        #region 住院结算单打印(补打出院结算单据)
        public static object[] YBZYJSD(object[] objParam)
        {
            string sysdate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string jzlsh = objParam[0].ToString();
            string djh = objParam[1].ToString();

            string Ywlx = "JS14";
            string grbh = "";
            string kh = "";
            string xm = "";
            string ybjzlsh = "";
            string tcqh = "";


            if (string.IsNullOrEmpty(jzlsh))
                return new object[] { 0, 0, "就诊流水号不能为空" };

            string strSql = string.Format(@"select * from ybfyjsdr where jzlsh='{0}' and djh='{1}' and cxbz=1", jzlsh, djh);
            DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            if (ds.Tables[0].Rows.Count == 0)
                return new object[] { 0, 0, "该患者未做出院结算" };

            strSql = string.Format(@"select xm,kh,grbh,ybjzlsh,tcqh from ybmzzydjdr where cxbz=1 and jzlsh='{0}'", jzlsh);
            ds.Tables[0].Clear();
            ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            if (ds.Tables[0].Rows.Count == 0)
                return new object[] { 0, 0, "该患者未医保登记" };
            grbh = ds.Tables[0].Rows[0]["grbh"].ToString();
            kh = ds.Tables[0].Rows[0]["kh"].ToString();
            xm = ds.Tables[0].Rows[0]["xm"].ToString();
            ybjzlsh = ds.Tables[0].Rows[0]["ybjzlsh"].ToString();
            tcqh = ds.Tables[0].Rows[0]["tcqh"].ToString();

            StringBuilder inputData = new StringBuilder();
            inputData.Append("SSSS|");
            inputData.Append(grbh + "|");
            inputData.Append(kh + "|");
            inputData.Append(xm + "|");
            inputData.Append(ybjzlsh + "|");
            inputData.Append(tcqh + "|");
            inputData.Append("ZZZZ");

            StringBuilder outData = new StringBuilder(10240);
            StringBuilder retMsg = new StringBuilder(1024);

            WriteLog(sysdate + "  " + jzlsh + " 进入住院收费结算...");
            WriteLog(sysdate + " 入参|" + inputData.ToString());
            int i = f_UserBargaingApply(Ywlx, inputData, outData, retMsg);
            if (i > 0)
            {
                WriteLog(sysdate + "  打印结算单功能成功");
                return new object[] { 0, 1, "打印结算单功能成功" };
            }
            else
            {
                WriteLog(sysdate + "  " + jzlsh + " 打印结算单功能失败" + retMsg.ToString());
                return new object[] { 0, 0, "打印结算单功能失败|" + retMsg.ToString() };
            }
        }
        #endregion

        #region 补打结算单(费用结算时打印)
        public static object[] YBZYJSD1(object[] objParam)
        {
            string sysdate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string jzlsh = objParam[0].ToString();
            string djh = objParam[1].ToString();

            string Ywlx = "JS13";
            string grbh = "";
            string kh = "";
            string xm = "";
            string ybjzlsh = "";
            string tcqh = "";
            string jslsh = "";


            if (string.IsNullOrEmpty(jzlsh))
                return new object[] { 0, 0, "就诊流水号不能为空" };

            string strSql = string.Format(@"select * from ybfyjsdr where jzlsh='{0}' and djh='{1}' and cxbz=1", jzlsh, djh);
            DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            if (ds.Tables[0].Rows.Count == 0)
                return new object[] { 0, 0, "该患者未做出院结算" };
            jslsh = ds.Tables[0].Rows[0]["jslsh"].ToString();

            strSql = string.Format(@"select xm,kh,grbh,ybjzlsh,tcqh from ybmzzydjdr where cxbz=1 and jzlsh='{0}'", jzlsh);
            ds.Tables[0].Clear();
            ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            if (ds.Tables[0].Rows.Count == 0)
                return new object[] { 0, 0, "该患者未医保登记" };
            grbh = ds.Tables[0].Rows[0]["grbh"].ToString();
            kh = ds.Tables[0].Rows[0]["kh"].ToString();
            xm = ds.Tables[0].Rows[0]["xm"].ToString();
            ybjzlsh = ds.Tables[0].Rows[0]["ybjzlsh"].ToString();
            tcqh = ds.Tables[0].Rows[0]["tcqh"].ToString();

            StringBuilder inputData = new StringBuilder();
            inputData.Append("SSSS|");
            inputData.Append(grbh + "|");
            inputData.Append(kh + "|");
            inputData.Append(xm + "|");
            inputData.Append(jslsh + "|");
            inputData.Append(tcqh + "|");
            inputData.Append("ZZZZ");

            StringBuilder outData = new StringBuilder(10240);
            StringBuilder retMsg = new StringBuilder(1024);

            WriteLog(sysdate + "  " + jzlsh + " 进入住院补打结算单...");
            WriteLog(sysdate + " 入参|" + inputData.ToString());
            int i = f_UserBargaingApply(Ywlx, inputData, outData, retMsg);
            if (i > 0)
            {
                WriteLog(sysdate + "  住院补打结算单成功");
                return new object[] { 0, 1, "住院补打结算单成功" };
            }
            else
            {
                WriteLog(sysdate + "  " + jzlsh + " 住院补打结算单失败" + retMsg.ToString());
                return new object[] { 0, 0, "住院补打结算单失败|" + retMsg.ToString() };
            }
        }
        #endregion

        #region 离院病人结算费用单
        public static object[] YBZYJSFYD(object[] objParam)
        {
            string sysdate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string jzlsh = objParam[0].ToString();

            if (string.IsNullOrEmpty(jzlsh))
                return new object[] { 0, 0, "就诊流水号不能为空" };

            try
            {
                //结算信息
                string strSql = string.Format(@"select a.jslsh,d.z3fphx as djh,convert(date,c.z1date) as ryrq,convert(date,c.z1ldat) as cyrq,datediff(day,c.z1date,c.z1ldat)+1 as zyts,
                                            a.xm,b.xb,a.jzlsh,a.ybjzlsh,b.ksmc,d.z3amtz,d.z3amt1,
                                            case when left(d.z3stbz,1)=0 then z3ysje else 0.00 end as cybjk,
                                            case when left(d.z3stbz,1)=1 then z3ysje else 0.00 end as cychk,a.zbxje,a.zhzf,a.xjzf
                                            from ybfyjsdr a 
                                            left join ybmzzydjdr b on a.jzlsh=b.jzlsh and a.ybjzlsh=b.ybjzlsh and b.cxbz=1
                                            left join zy01h c on a.jzlsh=c.z1zyno 
                                            left join zy03dw d on a.jzlsh=d.z3zyno and left(d.z3endv,1)=1
                                            where a.jzlsh='{0}' and a.cxbz=1", jzlsh);
                DataSet ds = CliUtils.ExecuteSql("szy01", "cmd", strSql, CliUtils.fLoginDB.ToString(), true, CliUtils.fCurrentProject.ToString());
                ds.Tables[0].TableName = "jsinfo";


                //费用构成
                strSql = string.Format(@"select sflb,sum(je) as je from ybcfmxscfhdr where jzlsh='{0}' and cxbz=1 group by sflb", jzlsh);
                DataSet ds_detail = CliUtils.ExecuteSql("szy01", "cmd", strSql, CliUtils.fLoginDB.ToString(), true, CliUtils.fCurrentProject.ToString());
                ds_detail.Tables[0].TableName = "detail";
                ds.Tables.Add(ds_detail.Tables[0].Copy());

                ds.WriteXmlSchema(@"D:\mm.xml");
                

                string s_path = string.Empty;
                s_path = string.IsNullOrEmpty(s_path) ? @"C:\Program Files\Infolight\EEP2012\EEPNetClient\FastReport" : s_path;//空值取默认

                string c_file = Application.StartupPath + @"\FastReport\YB\离院病人结算费用_乐安.frx"; //client
                string s_file = s_path + @"\YB\离院病人结算费用_乐安.frx";    //server  
                CliUtils.DownLoad(s_file, c_file);
                //检查报表文件是否存
                if (!File.Exists(c_file))
                {
                    ds.Dispose();
                    return new object[] { 0, 0, c_file + "不存在" };
                }
                else
                {
                    Report report = new Report();
                    report.PrintSettings.ShowDialog = false;
                    report.Load(c_file);
                    report.RegisterData(ds);

                    report.SetParameterValue("hdry", CliUtils.fUserName);//核对 
                    report.SetParameterValue("dyrq", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));//打印日期
                    report.Show();
                    report.Dispose(); 
                    ds.Dispose();
                    return new object[] { 0, 1, "离院病人结算费用打印成功" };
                }
            }
            catch (Exception ex)
            {
                return new object[] { 0, 0, "离院病人结算费用打印异常" + ex.Message };
            }

        }
        #endregion

        #region 医保出院登记
        public static object[] YBCYDJ(object[] objParam)
        {
            string sysdate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string Ywlx = "DJ19";
            string jzlsh = objParam[0].ToString();
            string djh = objParam[1].ToString();
            string cyyy = objParam[2].ToString();       //出院原因 
            string cyrqsj = objParam[3].ToString();     //出院日期时间
            string bxh = "";
            string xm = "";
            string kh = "";
            string ybjzlsh = "";
            string dqbh = "";
            string cyrq = "";
            string cysj = "";

            if (string.IsNullOrEmpty(jzlsh))
                return new object[] { 0, 0, "就诊流水号不能为空" };
            if (string.IsNullOrEmpty(cyrqsj))
                return new object[] { 0, 0, "出院日期不能为空" };


            WriteLog(sysdate + "  " + jzlsh + " 进入出院登记...");

            #region 时间
            try
            {
                DateTime dt_cyrq = Convert.ToDateTime(cyrqsj);
                cyrq = dt_cyrq.ToString("yyyyMMdd");
                cysj = dt_cyrq.ToString("HHmm");
            }
            catch
            {
                return new object[] { 0, 0, "出院日期时间格式错误" };
            }
            #endregion



            string strSql = string.Format("select * from ybmzzydjdr where jzlsh='{0}' and cxbz=1", jzlsh);
            DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            if (ds.Tables[0].Rows.Count == 0)
                return new object[] { 0, 0, "该患者未办理医保登记" };
            else
            {
                bxh = ds.Tables[0].Rows[0]["grbh"].ToString();
                xm = ds.Tables[0].Rows[0]["xm"].ToString();
                kh = ds.Tables[0].Rows[0]["kh"].ToString();
                dqbh = ds.Tables[0].Rows[0]["tcqh"].ToString();
                ybjzlsh = ds.Tables[0].Rows[0]["ybjzlsh"].ToString();

            }
            strSql = string.Format("select jslsh from ybfyjsdr where jzlsh='{0}' and djh='{1}' and cxbz=1", jzlsh, djh);
            ds.Tables.Clear();
            ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            if (ds.Tables[0].Rows.Count == 0)
                return new object[] { 0, 0, "该患者未办理住院收费结算" };
            ds.Dispose();
            StringBuilder inputData = new StringBuilder();

            inputData.Append("SSSS|");
            inputData.Append(bxh + "|");
            inputData.Append(kh + "|");
            inputData.Append(xm + "|");
            inputData.Append(ybjzlsh + "|");
            inputData.Append(cyrq + "|");
            inputData.Append(cysj + "|");
            inputData.Append(cyyy + "|");
            inputData.Append(dqbh + "|");
            inputData.Append("ZZZZ");

            StringBuilder outData = new StringBuilder(1024);
            StringBuilder retMsg = new StringBuilder(1024);
            WriteLog(sysdate + " 入参|" + inputData.ToString());
            int i = f_UserBargaingApply(Ywlx, inputData, outData, retMsg);
            if (i > 0)
            {
                WriteLog(sysdate + "  " + jzlsh + " 出院登记成功|" + outData.ToString());
                strSql = string.Format(@"update ybfyjsdr set cyzt=1 where cxbz=1 and jzlsh='{0}' and djh='{1}'", jzlsh, djh);
                object[] obj = { strSql};
                obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);
                return new object[] { 0, 1, "出院登记成功|", outData.ToString() };
            }
            else
            {
                WriteLog(sysdate + "  " + jzlsh + " 出院登记失败|" + retMsg.ToString());
                return new object[] { 0, 0, "出院登记失败|"+retMsg.ToString() };
            }
        }
        #endregion

        #region 医保出院登记撤销
        public static object[] YBCYDJCX(object[] objParam)
        {
            string sysdate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string Ywlx = "DJ20";
            string jzlsh = objParam[0].ToString();
            string bxh = "";
            string xm = "";
            string kh = "";
            string ybjzlsh = "";
            string dqbh = "";

            if (string.IsNullOrEmpty(jzlsh))
                return new object[] { 0, 0, "就诊流水号不能为空" };

            string strSql = string.Format("select * from ybmzzydjdr where jzlsh='{0}' and cxbz=1", jzlsh);
            DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            if (ds.Tables[0].Rows.Count == 0)
                return new object[] { 0, 0, "该患者未办理医保登记" };
            else
            {
                bxh = ds.Tables[0].Rows[0]["grbh"].ToString();
                xm = ds.Tables[0].Rows[0]["xm"].ToString();
                kh = ds.Tables[0].Rows[0]["kh"].ToString();
                dqbh = ds.Tables[0].Rows[0]["tcqh"].ToString();
                ybjzlsh = ds.Tables[0].Rows[0]["ybjzlsh"].ToString();
            }
            ds.Dispose();

            StringBuilder inputData = new StringBuilder();

            inputData.Append("SSSS|");
            inputData.Append(bxh + "|");
            inputData.Append(kh + "|");
            inputData.Append(xm + "|");
            inputData.Append(ybjzlsh + "|");
            inputData.Append(dqbh + "|");
            inputData.Append("ZZZZ");

            StringBuilder outData = new StringBuilder(1024);
            StringBuilder retMsg = new StringBuilder(1024);
            WriteLog(sysdate + " 入参|" + inputData.ToString());
            int i = f_UserBargaingApply(Ywlx, inputData, outData, retMsg);
            if (i > 0)
            {
                strSql = string.Format(@"update ybfyjsdr set cyzt=0 where cxbz=1 and jzlsh='{0}'", jzlsh);
                object[] obj = { strSql };
                obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);
                WriteLog(sysdate + "  " + jzlsh + " 出院登记撤销成功|" + outData.ToString());
                return new object[] { 0, 1, "出院登记撤销成功|", outData.ToString() };
            }
            else
            {
                WriteLog(sysdate + "  " + jzlsh + " 出院登记撤销失败|" + retMsg.ToString());
                return new object[] { 0, 0, "出院登记撤销失败|" + retMsg.ToString() };
            }
        }
        #endregion

        #region 门诊登记(挂号)撤销（内部）
        public static object[] NYBMZDJCX(object[] objParam)
        {
            string sysdate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string Ywlx = "DJ02";
            string jzlsh = objParam[0].ToString(); //就诊流水号
            string grbh = objParam[1].ToString(); //保险号(个人编号)
            string xm = objParam[2].ToString();  //姓名
            string kh = objParam[3].ToString(); //卡号
            string tcqh = objParam[4].ToString(); //地区编号
            string ybjzlsh = objParam[5].ToString();//中心业务流水号(医保就诊流水号)

            /* 入参
            1 社会保险号 VARCHAR2(20) 否 医院实时结算平台接口规范
            2 社会保障卡号 VARCHAR2(20) 否
            3 姓名 VARCHAR2(20) 否
            4中心业务流水号 VARCHAR2(20) 否 门诊号
            5 地区编号 VARCHAR2(6) 否 统筹区编码
            */
            StringBuilder inputData = new StringBuilder();
            inputData.Append("SSSS|");
            inputData.Append(grbh + "|");
            inputData.Append(kh + "|");
            inputData.Append(xm + "|");
            inputData.Append(ybjzlsh+"|");
            inputData.Append(tcqh + "|");
            inputData.Append("ZZZZ");
            StringBuilder outData = new StringBuilder(1024);
            StringBuilder retMsg = new StringBuilder(1024);

            WriteLog(sysdate + "  " + jzlsh + " 进入门诊挂号撤销(内部)...");
            WriteLog(sysdate + "  入参|" + inputData.ToString());
            int i = f_UserBargaingApply(Ywlx, inputData, outData, retMsg);
            if (i > 0)
            {
                WriteLog(sysdate + "  " + jzlsh + " 进入门诊挂号撤销(内部)成功|" + outData.ToString());
                return new object[] { 0, 1, "门诊挂号撤销(内部)成功", outData.ToString() };
            }
            else
            {
                WriteLog(sysdate + "  " + jzlsh + " 进入门诊挂号撤销(内部)失败|" + outData.ToString());
                return new object[] { 0, 0, "门诊挂号撤销(内部)失败", outData.ToString() };
            }
        }
        #endregion

        #region 门诊收费结算撤销(内部)
        public static object[] NYBMZSFJSCX(object[] objParam)
        {
            string sysdate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string Ywlx = "JS06";
            string jzlsh = objParam[0].ToString();
            string bxh = objParam[1].ToString();
            string kh = objParam[2].ToString();
            string xm = objParam[3].ToString();
            string ybjzlsh = objParam[4].ToString();
            string djlsh = objParam[5].ToString();   //医保反馈单据流水号 
            string dqbh = objParam[6].ToString();

            StringBuilder inputData = new StringBuilder();
            /*
             * 1 社会保险号 VARCHAR2(20) 否
             * 2 社会保障卡号 VARCHAR2(20) 否
             * 3 姓名 VARCHAR2(20) 否
             * 4 中心业务流水号 VARCHAR2(20) 否 门诊号
             * 5 中心结算单据号 VARCHAR2(20) 否 JS05交易返回的第15项信息
             * 6 地区编号 VARCHAR2(6) 否 统筹区编码
             */
            inputData.Append("SSSS|");
            inputData.Append(bxh + "|");
            inputData.Append(kh + "|");
            inputData.Append(xm + "|");
            inputData.Append(ybjzlsh + "|");
            inputData.Append(djlsh + "|");
            inputData.Append(dqbh + "|");
            inputData.Append("ZZZZ");
            StringBuilder outData = new StringBuilder(1024);
            StringBuilder retMsg = new StringBuilder(1024);

            WriteLog(sysdate + "  " + jzlsh + " 进入门诊收费撤销(内部)...");
            WriteLog(sysdate + "  入参|" + inputData.ToString());
            int i = f_UserBargaingApply(Ywlx, inputData, outData, retMsg);
            if (i > 0)
            {
              
                    WriteLog(sysdate + "  " + jzlsh + " 进入门诊收费撤销(内部)成功|" + outData.ToString());
                    return new object[] { 0, 1, outData.ToString() };
            }
            else
            {
                WriteLog(sysdate + "  " + jzlsh + " 进入门诊收费撤销(内部)失败|" + retMsg.ToString());
                return new object[] { 0, 0, "门诊收费撤销(内部)失败|" + retMsg.ToString() };
            }
        }
        #endregion

        #region 住院登记撤销(内部)
        public static object[] NYBZYDJCX(object[] objParam)
        {
            string sysdate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string Ywlx = "DJ04";

            string jzlsh = objParam[0].ToString();  //就诊流水号
            string jbr = CliUtils.fUserName;   //经办人
            string bxh = objParam[1].ToString();
            string kh = objParam[2].ToString();
            string xm = objParam[3].ToString();
            string ybjzlsh = objParam[4].ToString();
            string dqbh = objParam[5].ToString();
            if (string.IsNullOrEmpty(jzlsh))
                return new object[] { 0, 0, "就诊流水号不能为空" };

            WriteLog(sysdate + "  " + jzlsh + " 进入住院医保登记撤销(内部)...");

            StringBuilder inputData = new StringBuilder();
            /*
                1 社会保险号 VARCHAR2(20) 否
                2 社会保障卡号 VARCHAR2(20) 否
                3 姓名 VARCHAR2(20) 否
                4 中心业务流水号 VARCHAR2(20) 否 住院号
                5 地区编号 VARCHAR2(6) 否 统筹区编码
             */
            inputData.Append("SSSS|");
            inputData.Append(bxh + "|");
            inputData.Append(kh + "|");
            inputData.Append(xm + "|");
            inputData.Append(ybjzlsh + "|");
            inputData.Append(dqbh + "|");
            inputData.Append("ZZZZ");

            StringBuilder outData = new StringBuilder(10240);
            StringBuilder retMsg = new StringBuilder(10240);

            WriteLog(sysdate + "  入参|" + inputData.ToString());
            int i = f_UserBargaingApply(Ywlx, inputData, outData, retMsg);
            if (i > 0)
            {
                WriteLog(sysdate + "  " + jzlsh + " 进入住院医保登记撤销(内部)成功|" + outData.ToString());
                return new object[] { 0, 1, "住院登记撤销(内部)成功", outData.ToString() };
            }
            else
            {
                WriteLog(sysdate + "  " + jzlsh + " 进入住院医保登记撤销(内部)失败|" + retMsg.ToString());
                return new object[] { 0, 0, "住院登记撤销(内部)失败|" + retMsg.ToString() };
            }
        }
        #endregion

        #region 住院收费登记撤销(全部)(内部)
        public static object[] NYBZYSFDJCX(object[] objParam)
        {
            string sysdate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string Ywlx = "DJ08";
            string jzlsh = objParam[0].ToString();
            string bxh = objParam[1].ToString();
            string xm = objParam[2].ToString();
            string kh = objParam[3].ToString();
            string ybjzlsh = objParam[4].ToString();
            string bqbh = objParam[5].ToString();

            if (string.IsNullOrEmpty(jzlsh))
                return new object[] { 0, 0, "就诊流水号不能为空" };

            WriteLog(sysdate + "  " + jzlsh + " 进入住院费用明细撤销(全部)(内部)...");

            StringBuilder inputData = new StringBuilder();
            /*
             1 社会保险号 VARCHAR2(20) 否
            2 社会保障卡号 VARCHAR2(20) 否
            3 姓名 VARCHAR2(20) 否
            4 中心业务流水号 VARCHAR2(20) 否 住院号
            5 地区编号 VARCHAR2(6) 否 统筹区编码             */
            inputData.Append("SSSS|");
            inputData.Append(bxh + "|");
            inputData.Append(kh + "|");
            inputData.Append(xm + "|");
            inputData.Append(ybjzlsh + "|");
            inputData.Append(dqbh + "|");
            inputData.Append("ZZZZ");
            StringBuilder outData = new StringBuilder(1024);
            StringBuilder retMsg = new StringBuilder(1024);
            int i = f_UserBargaingApply(Ywlx, inputData, outData, retMsg);
            if (i > 0)
            {
               
                    WriteLog(sysdate + "  " + jzlsh + " 进入住院费用明细撤销(全部)(内部)成功|" + outData.ToString());
                    return new object[] { 0, 1, "住院收费退费成功", outData.ToString() };
                
            }
            else
            {
                WriteLog(sysdate + "  " + jzlsh + " 进入住院费用明细撤销(全部)(内部)失败|" + retMsg.ToString());
                return new object[] { 0, 0, "住院收费退费失败|" + retMsg.ToString() };
            }
        }
        #endregion

        #region 住院收费结算撤销(内部)
        public static object[] NYBZYSFJSCX(object[] objParam)
        {
            string sysdate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string Ywlx = "JS12";
            string jzlsh = objParam[0].ToString();
            string djlsh = objParam[1].ToString();
            string bxh = "";
            string xm = "";
            string kh = "";
            string ybjzlsh = "";
            string sfmxsfzf = "FALSE";

            if (string.IsNullOrEmpty(jzlsh))
                return new object[] { 0, 0, "就诊流水号不能为空" };

            string strSql = string.Format("select * from ybmzzydjdr where jzlsh='{0}' and cxbz=1", jzlsh);
            DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            if (ds.Tables[0].Rows.Count == 0)
                return new object[] { 0, 0, "该患者未办理医保登记" };
            else
            {
                bxh = ds.Tables[0].Rows[0]["grbh"].ToString();
                xm = ds.Tables[0].Rows[0]["xm"].ToString();
                kh = ds.Tables[0].Rows[0]["kh"].ToString();
                dqbh = ds.Tables[0].Rows[0]["tcqh"].ToString();
                ybjzlsh = ds.Tables[0].Rows[0]["ybjzlsh"].ToString();

            }

            StringBuilder inputData = new StringBuilder();
            //入参:
            inputData.Append("SSSS|");
            inputData.Append(bxh + "|");
            inputData.Append(kh + "|");
            inputData.Append(xm + "|");
            inputData.Append(ybjzlsh + "|");
            inputData.Append(djlsh + "|");
            inputData.Append(sfmxsfzf + "|");
            inputData.Append(dqbh + "|");
            inputData.Append("ZZZZ");

            StringBuilder outData = new StringBuilder(1024);
            StringBuilder retMsg = new StringBuilder(1024);
            WriteLog(sysdate + "  " + jzlsh + " 进入住院收费结算撤销(内部)...");
            WriteLog(sysdate + " 入参|" + inputData.ToString());
            int i = f_UserBargaingApply(Ywlx, inputData, outData, retMsg);
            if (i > 0)
            {
                    WriteLog(sysdate + "  " + jzlsh + " 进入住院收费结算撤销(内部)成功" + outData.ToString());
                    return new object[] { 0, 1, "住院收费结算撤销(内部)成功", outData.ToString() };
            }
            else
            {
                WriteLog(sysdate + "  " + jzlsh + " 进入住院收费结算撤销(内部)失败" + retMsg.ToString());
                return new object[] { 0, 0, "住院收费结算撤销(内部)失败|" + retMsg.ToString() };
            }
        }
        #endregion


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
