using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ybinterface_lib.YB.ZR.FZ
{
    public class yb_interface_fh_zr
    {
        public static string dqbh = ""; //地区编号(抚)

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
            string Ywlx = "MZGHSK";
            StringBuilder inputData = new StringBuilder();
            StringBuilder outData = new StringBuilder(102400);
            StringBuilder retMsg = new StringBuilder(102400);
            WriteLog(sysdate + "  进入门诊读卡....");
            WriteLog(sysdate + "  入参|" + inputData.ToString());
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
                18 生育保险参保 标志 VARCHAR2(1) 1-参保；0-未参保
                19 工伤保险参保 标志 VARCHAR2(1) 1-参保；0-未参保
                20 医保账户余额 NUMBER(8,2)
                21 本年累计住院 次数 NUMBER(4)
                22 本年基本医疗 基金支付累计 NUMBER(16,2)
                23 本年大病医疗 基金支付累计 NUMBER(16,2)
                24 本年公务员补 充医疗基金支付累计 NUMBER(16,2)
                25 基本医疗年度 剩余可报金额 NUMBER(16,2)
                26 大病医疗年度 剩余可报金额 NUMBER(16,2)
                27 公务员补充医 疗年度剩余可报金额 NUMBER(16,2)
                28 门特年度剩余可报金额 NUMBER(16,2)
                29 预留指标1 当地补充说明，自定义用途
                30 预留指标2 当地补充说明，自定义用途
                */
                string[] str = outData.ToString().Split('|');
                if (str.Length < 10)
                    return new object[] { 0, 0, outData.ToString() + retMsg.ToString() };
                outParams_dk
                string bxh = str[0].Trim();     //保险号
                string xm = str[1].Trim();      //姓名
                string kh = str[2].Trim();      //卡号
                string dqbh = str[3].Trim();    //地区编号
                string qxmc = str[4].Trim();    //地区名称
                string csrq = str[5].Trim();    //出生日期
                string sjnl = str[6].Trim();    //实际年龄
                string cbrq = str[7].Trim();    //参保日期
                string grsf = str[8].Trim();    //个人身份
                string dwmc = str[9].Trim();    //单位名称
                string xb = str[10].Trim();     //性别
                string rylb = str[11].Trim();   //医疗人员类别
                string kzt = str[12].Trim();    //卡状态
                string grzhye = str[13].Trim(); //账户余额
                string ljzyf = str[14].Trim();  //累计住院支付
                string ljmzf = str[15].Trim();  //累计门诊支付
                string ljtsmzf = str[16].Trim();//累计特殊门诊
                string gmsfhm = str[17].Trim(); //身份号码




                #endregion

                string strSql = string.Format("delete from YBICKXX where grbh='{0}'", bxh);
                liSQL.Add(strSql);
                strSql = string.Format(@"insert into YBICKXX(grbh,xm,kh,dqbh,qxmc,csrq,sjnl,cbrq,grsf,dwmc,
                                        xb,rylb,kzt,grzhye,ljzyf,ljmzf,ljtsmzf,gmsfhm) values(
                                        '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}',
                                        '{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}')",
                                        bxh, xm, kh, dqbh, qxmc, csrq, sjnl, cbrq, grsf, dwmc,
                                        xb, rylb, kzt, grzhye, ljzyf, ljmzf, ljtsmzf, gmsfhm);
                liSQL.Add(strSql);
                object[] obj = liSQL.ToArray();
                obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);
                if (obj[1].ToString().Equals("1"))
                {

                    /*个人编号|单位编号|身份证号|姓名|性别|
                     * 民族|出生日期|社会保障卡卡号|医疗待遇类别|人员参保状态|
                     * 异地人员标志|统筹区号|年度|在院状态|帐户余额|
                     * 本年医疗费累计|本年帐户支出累计|本年统筹支出累计|本年救助金支出累计|本年公务员补助基金累计|
                     * 本年城镇居民门诊统筹支付累计|进入统筹费用累计|进入救助金费用累计|起付标准累计|本年住院次数|
                     * 单位名称|年龄|参保单位类型|经办机构编码|累计住院支付|累计门诊支付|累计特殊门诊
                     */
                    if (xb.Equals("男"))
                        xb = "1";
                    else
                        xb = "2";
                    csrq = csrq.Substring(0, 4) + "-" + csrq.Substring(4, 2) + "-" + csrq.Substring(6, 2);
                    string strValue = bxh + "||" + gmsfhm + "|" + xm + "|" + xb + "||" + csrq + "|" + kh + "|"
                                        + rylb + "|" + kzt + "|0|" + dqbh + "|||" + grzhye + "||"
                                        + "|||||||||" + qxmc + "|" + sjnl + "|||" + ljzyf + "|" + ljmzf + "|" + ljtsmzf;
                    WriteLog(sysdate + "  门诊读卡成功|" + strValue);
                    return new object[] { 0, 1, strValue };
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
        #endregion


    }
}
