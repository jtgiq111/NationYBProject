using FastReport;
using Srvtools;
using System;
using System.Configuration;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Data;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace ybinterface_lib
{
    /// <summary>
    /// 
    /// </summary>
    public class yb_interface_jj_dr 
    {

        #region 医保dll函数声明
        [DllImport("SiInterface.dll", EntryPoint = "INIT", CharSet = CharSet.Ansi)]
        protected static extern int INIT(StringBuilder pErrMsg);

        [DllImport("SiInterface.dll", EntryPoint = "BUSINESS_HANDLE", CharSet = CharSet.Ansi)]
        protected static extern int BUSINESS_HANDLE(StringBuilder inputData, StringBuilder outputData);
        #endregion 医保dll函数声明

        static GocentPara Para = new GocentPara();
        static readonly yb_interface_jj_dr yb_interface_jj_dr1 = new yb_interface_jj_dr();

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

        internal static string DDYLJGBH = lItem[0].DDYLJGBH;//医疗机构编号
        internal static string DDYLJGMC = lItem[0].DDYLJGMC;//医院名称
        internal static string Gocent = "03";    //开发商标识
        internal static string YBZDSCGH = lItem[0].YBZDSCGH;//自动上传工号
        internal static string YBIP = lItem[0].YBIP;        //医保IP地址
        internal static string ZXBM = "0000";//中心编码
        internal static string LJBZ = "1";   //联机标识，固定为1   
        #endregion 变量

        public static yb_interface_jj_dr GetInstance()
        {
            return yb_interface_jj_dr1;
        }

        #region 认证类东软
        #region 初始化签到东软
        /// <summary>
        /// 初始化签到东软
        /// </summary>
        /// <param>无</param>
        /// <returns>1为成功</returns>
        public static object[] YBINIT(object[] objParam)
        {
            try
            {
                //Ping ping = new Ping();
                //PingReply pingReply = ping.Send(YBIP);

                //if (pingReply.Status != IPStatus.Success)
                //{
                //    return new object[] { 0, 0, "医保提示：未连接医保网" };
                //}
                StringBuilder mess = new StringBuilder(100000);
                int i = INIT(mess);

                if (i == 0)
                {
                    object[] obj = YBQDDR(new object[] {});
                    return obj;
                }
                else
                {
                    return new object[] { 0, 0, mess.ToString() };
                }
            }
            catch (Exception error)
            {
                return new object[] { 0, 2, "Error:" + error.ToString() };
            }
        }
        #endregion 初始化签到东软

        #region 签到东软
        /// <summary>
        /// 签到东软
        /// </summary>
        /// <param>无</param>
        /// <returns>业务周期号</returns>
        public static object[] YBQDDR(object[] objParam)
        {
            ZXBM = "0000";

            try
            {
                string sysdate = Common.GetServerTime();
                if (string.IsNullOrWhiteSpace(CliUtils.fLoginUser))
                {
                    return new object[] { 0, 0, "医保提示：用户工号不能为空" };
                }

                if (string.IsNullOrWhiteSpace(DDYLJGBH))
                {
                    return new object[] { 0, 0, "医保提示：定点医疗机构编号不能为空" };
                }

                DateTime dqsj = Convert.ToDateTime(Common.GetServerTime());
                string jylsh = dqsj.ToString("yyyyMMddHHmmss") + "-" + DDYLJGBH + "-" + CliUtils.fLoginUser;
                StringBuilder inputData = new StringBuilder(string.Format("{0}^{1}^{2}^^{3}^{4}^^{5}^", "9100", DDYLJGBH, CliUtils.fLoginUser, jylsh, ZXBM, "1"));
                StringBuilder outputData = new StringBuilder(100000);
                CZYBH = CliUtils.fLoginUser;

                WriteLog(sysdate + "  用户" + CZYBH + " 进入医保签到...");
                WriteLog(sysdate + "  入参|" + inputData.ToString());

                int i = BUSINESS_HANDLE(inputData, outputData);

                if (i == 0)
                {

                    YWZQH = outputData.ToString().Split('^')[2].TrimEnd('|');
                    CliUtils.fLoginYbNo = outputData.ToString().Split('^')[2].TrimEnd('|');
                    if (string.IsNullOrWhiteSpace(YWZQH))
                    {
                        return new object[] { 0, 0, "医保提示：业务周期号不能为空" };
                    }


                    WriteLog(sysdate + "  用户" + CZYBH + "医保签到成功");
                    WriteLog(sysdate + "  入参|" + inputData.ToString());

                    return new object[] { 0, 1, YWZQH };
                }
                else
                {
                    Common.InsertYBLog("", inputData.ToString(), outputData.ToString());
                    return new object[] { 0, 0, outputData };
                }
            }
            catch (Exception error)
            {
                Common.InsertYBLog("", "", error.ToString());
                return new object[] { 0, 2, "Error:" + error.ToString() };
            }
        }
        #endregion 签到东软

        #region 签退东软
        /// <summary>
        /// 签退东软
        /// </summary>
        /// <param>无</param>
        /// <returns>1成功非1失败</returns>
        public static object[] YBQTDR(object[] objParam)
        {
            ZXBM = "0000";

            try
            {
                DateTime dqsj = Convert.ToDateTime(Common.GetServerTime());
                string jylsh = dqsj.ToString("yyyyMMddHHmmss") + "-" + DDYLJGBH + "-" + CliUtils.fLoginUser;
                StringBuilder inputData = new StringBuilder(string.Format("{0}^{1}^{2}^{3}^{4}^{5}^^{6}^", "9110", DDYLJGBH, CliUtils.fLoginUser, YWZQH, jylsh, ZXBM, "1"));
                StringBuilder outputData = new StringBuilder(100000);
                int i = BUSINESS_HANDLE(inputData, outputData);

                if (i == 0)
                {
                    return new object[] { 0, 1, outputData };
                }
                else
                {
                    Common.InsertYBLog("", inputData.ToString(), outputData.ToString());
                    return new object[] { 0, 0, outputData };
                }
            }
            catch (Exception error)
            {
                Common.InsertYBLog("", "", error.ToString());
                return new object[] { 0, 2, "Error:" + error.ToString() };
            }
        }
        #endregion 签退东软
        #endregion 认证类东软

        #region 数据上传类

        #region 医院审批信息上报
        /// <summary>
        /// 医院审批信息上报
        /// </summary>
        /// <param>个人编号,审批类别</param>
        /// <returns>1成功，否则失败</returns>
        public static object[] YBYYSPXXSBDR(object[] objParam)
        {
            string grbh = objParam[0].ToString();
            string splb = objParam[1].ToString();
            
            if (string.IsNullOrWhiteSpace(CliUtils.fLoginUser) || string.IsNullOrWhiteSpace(YWZQH) || string.IsNullOrWhiteSpace(grbh) || string.IsNullOrWhiteSpace(splb) || string.IsNullOrWhiteSpace(CliUtils.fUserName))
            {
                return new object[] { 0, 0, "入参不完整或空" };
            }

            ZXBM = "0000";

            try
            {
                DateTime dqsj = Convert.ToDateTime(Common.GetServerTime());
                string jylsh = dqsj.ToString("yyyyMMddHHmmss") + "-" + DDYLJGBH + "-" + grbh;
                string rc = grbh + "|" + splb + "||||||||||" + dqsj.ToString("yyyyMMddHHmmss") + "||" + CliUtils.fUserName + "|||||";
                StringBuilder inputData = new StringBuilder(string.Format("{0}^{1}^{2}^{3}^{4}^{5}^{6}^{7}^", "3110", DDYLJGBH, CliUtils.fLoginUser, YWZQH, jylsh, ZXBM, rc, "1"));
                StringBuilder outputData = new StringBuilder(100000);
                int i = BUSINESS_HANDLE(inputData, outputData);

                if (i == 0)
                {
                    i = 1;
                }
                else
                {
                    i = 0;
                    Common.InsertYBLog("", inputData.ToString(), outputData.ToString());
                }

                return new object[] { 0, i, outputData };
            }
            catch (Exception error)
            {
                Common.InsertYBLog("", "", error.ToString());
                throw error;
            }
        }
        #endregion 医院审批信息上报

        #region 医院审批信息上报撤销东软
        /// <summary>
        /// 医院审批信息上报撤销东软
        /// </summary>
        /// <param>审批类别,个人编号</param>
        /// <returns>bool</returns>
        public static object[] YBYYSPXXCXDR(object[] objParam)
        {
            string splb = objParam[0].ToString();
            string grbh = objParam[1].ToString();
            ZXBM = "0000";

            try
            {
                if (string.IsNullOrWhiteSpace(splb) || string.IsNullOrWhiteSpace(grbh))
                {
                    return new object[]{"0"};
                }

                DateTime dqsj = Convert.ToDateTime(Common.GetServerTime());
                string jylsh = dqsj.ToString("yyyyMMddHHmmss") + "-" + DDYLJGBH + "-" + grbh;
                string rc = splb + "|" + grbh + "|||||";
                StringBuilder inputData = new StringBuilder(string.Format("{0}^{1}^{2}^{3}^{4}^{5}^{6}^{7}^", "3120", DDYLJGBH, CliUtils.fLoginUser, YWZQH, jylsh, ZXBM, rc, "1"));
                StringBuilder outputData = new StringBuilder(100000);
                int i = BUSINESS_HANDLE(inputData, outputData);

                if (i != 0)
                {
                    Common.InsertYBLog("", inputData.ToString(), outputData.ToString());
                    return new object[] { "0" };
                }
                else return new object[] { "1" };
                
            }
            catch (Exception error)
            {
                Common.InsertYBLog("", "", error.ToString());
                throw error;
            }
        }
        #endregion 医院审批信息上报撤销东软

        #region 对照信息批量上传东软
        /// <summary>
        /// 对照信息批量上传东软
        /// </summary>
        /// <param>收费种类代码|医保项目代码|医院项目代码|医院项目名称|当前时间DateTime.Now.ToString("yyyyMMddHHmmss")|$</param>
        /// <returns>1:成功，0:不成功，2:报错</returns>
        public static object[] YBDZXXPLSCDR(object[] objParam)
        {
            ZXBM = "0000";

            try
            {
                string rc = objParam[0].ToString();

                if (string.IsNullOrWhiteSpace(rc))
                {
                    return new object[] { 0, 0, "医保提示：对照信息为空" };
                }

                DateTime dqsj = Convert.ToDateTime(Common.GetServerTime());
                string jylsh = dqsj.ToString("yyyyMMddHHmmss") + "-" + DDYLJGBH + "-" + CliUtils.fLoginUser;
                StringBuilder inputData = new StringBuilder(string.Format("{0}^{1}^{2}^{3}^{4}^{5}^{6}^{7}^", "3300", DDYLJGBH, CliUtils.fLoginUser, YWZQH, jylsh, ZXBM, rc.ToString(), "1"));
                StringBuilder outputData = new StringBuilder(100000);
                int i = BUSINESS_HANDLE(inputData, outputData);

                if (i == 0)
                {
                    return new object[] { 0, 1, outputData };
                }
                else
                {
                    Common.InsertYBLog("", inputData.ToString(), outputData.ToString());
                    return new object[] { 0, 0, outputData };
                }
            }
            catch (Exception error)
            {
                Common.InsertYBLog("", "", error.ToString());
                return new object[] { 0, 2, "Error:" + error.ToString() };
            }
        }
        #endregion 对照信息批量上传东软

        #region 医师信息上传东软
        /// <summary>
        /// 医师信息上传东软
        /// </summary>
        /// <param>无</param>
        /// <returns>1:成功，0:不成功，2:报错</returns>
        public static object[] YBYSXXSCDR(params object[] objParam)
        {
            ZXBM = "0000";

            try
            {
                DateTime dqsj = Convert.ToDateTime(Common.GetServerTime());
                string jylsh = dqsj.ToString("yyyyMMddHHmmss") + "-" + DDYLJGBH + "-" + CliUtils.fLoginUser;
                StringBuilder rc = new StringBuilder();
                string strSql = string.Format("select b1empn, b1name, b1yscf, b1cycf, b1zycf, b1ksno from bz01h where b1ybup = 0");//0秒
                DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);

                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    string ysno = dr["b1empn"].ToString();
                    string ysnm = dr["b1name"].ToString();
                    string yscfq = "";

                    if (dr["b1yscf"].ToString() == "1" || dr["b1cycf"].ToString() == "1" || dr["b1zycf"].ToString() == "1")
                    {
                        yscfq = "1";
                    }
                    else
                    {
                        yscfq = "1";
                    }

                    string ksno = dr["b1ksno"].ToString();
                    rc.Append(ysno + "|" + ysnm + "|" + yscfq + "||" + ksno + "|||||||||||||||$");
                }

                if (string.IsNullOrWhiteSpace(rc.ToString()))
                {
                    return new object[] { 0, 0, "医保提示：没有需要上传的医师信息" };
                }

                StringBuilder inputData = new StringBuilder(string.Format("{0}^{1}^{2}^{3}^{4}^{5}^{6}^{7}^", "3400", DDYLJGBH, CliUtils.fLoginUser, YWZQH, jylsh, ZXBM, rc.ToString().TrimEnd('$'), "1"));
                StringBuilder outputData = new StringBuilder(100000);
                int i = BUSINESS_HANDLE(inputData, outputData);

                if (i == 0)
                {
                    string sqlybup = string.Format("update bz01h set b1ybup = 1 where b1ybup = 0");//0秒
                    object[] obj = { sqlybup };
                    obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);

                    if (obj[1].ToString() == "1")
                    {
                        return new object[] { 0, 1, outputData };
                    }
                    else
                    {
                        Common.InsertYBLog("", "", obj[2].ToString());
                        return new object[] { 0, obj[1].ToString(), "his提示：数据库操作错误，" + obj[2].ToString() };
                    }
                }
                else if (i == -1)
                {
                    Common.InsertYBLog("", inputData.ToString(), outputData.ToString());
                    return new object[] { 0, -1, "医保提示：医保系统级别错误，" + outputData.ToString() };
                }
                else
                {
                    Common.InsertYBLog("", inputData.ToString(), outputData.ToString());
                    return new object[] { 0, -2, "医保提示：医保业务级别或未知错误，" + outputData.ToString() };
                }
            }
            catch (Exception error)
            {
                Common.InsertYBLog("", "", error.ToString());
                return new object[] { 0, 2, "Error:" + error.ToString() };
            }
        }
        #endregion 医师信息上传东软

        #region 科室信息上传东软
        /// <summary>
        /// 科室信息上传东软
        /// </summary>
        /// <param>无</param>
        /// <returns>1:成功，0:不成功，2:报错</returns>
        public static object[] YBKSXXSCDR(params object[] objParam)
        {
            ZXBM = "0000";

            try
            {
                DateTime dqsj = Convert.ToDateTime(Common.GetServerTime());
                string jylsh = dqsj.ToString("yyyyMMddHHmmss") + "-" + DDYLJGBH + "-" + CliUtils.fLoginUser;
                StringBuilder rc = new StringBuilder();
                string strSql = string.Format(@"select b2ejks, b2ejnm from bz02d where b2ybup = 0");//0秒
                DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);

                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    string ksno = dr["b2ejks"].ToString();
                    string ksnm = dr["b2ejnm"].ToString();
                    rc.Append(ksno + "|" + ksnm + "|||||||$");
                }

                if (string.IsNullOrWhiteSpace(rc.ToString()))
                {
                    return new object[] { 0, 0, "医保提示：没有需要上传的科室信息" };
                }

                StringBuilder inputData = new StringBuilder(string.Format("{0}^{1}^{2}^{3}^{4}^{5}^{6}^{7}^", "3500", DDYLJGBH, CliUtils.fLoginUser, YWZQH, jylsh, ZXBM, rc.ToString().TrimEnd('$'), "1"));
                StringBuilder outputData = new StringBuilder(100000);
                int i = BUSINESS_HANDLE(inputData, outputData);

                if (i == 0)
                {
                    string sqlybup = string.Format("update bz02d set b2ybup = 1 where b2ybup = 0");//0秒
                    object[] obj = { sqlybup };
                    obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);

                    if (obj[1].ToString() == "1")
                    {
                        return new object[] { 0, 1, outputData };
                    }
                    else
                    {
                        Common.InsertYBLog("", "", obj[2].ToString());
                        return new object[] { 0, obj[1].ToString(), "his提示：数据库操作错误，" + obj[2].ToString() };
                    }
                }
                else if (i == -1)
                {
                    Common.InsertYBLog("", inputData.ToString(), outputData.ToString());
                    return new object[] { 0, -1, "医保提示：医保系统级别错误，" + outputData.ToString() };
                }
                else
                {
                    Common.InsertYBLog("", inputData.ToString(), outputData.ToString());
                    return new object[] { 0, -2, "医保提示：医保业务级别或未知错误，" + outputData.ToString() };
                }
            }
            catch (Exception error)
            {
                Common.InsertYBLog("", "", error.ToString());
                return new object[] { 0, 2, "Error:" + error.ToString() };
            }
        }
        #endregion 科室信息上传东软

        #endregion 数据上传类

        #region	查询类

        #region 个人就诊登记信息查询东软
        /// <summary>
        /// 个人就诊登记信息查询东软
        /// </summary>
        /// <param>开始时间（格式：DateTime.Now.ToString("yyyyMMddHHmmss")）,终止时间（格式：DateTime.Now.ToString("yyyyMMddHHmmss")）,个人编号</param>
        /// <returns>定点医疗机构编码|就诊流水号|医疗类别|入院日期|在院状态|医疗待遇类别|经办机构编码|定点医疗机构名称|经办时间|姓名</returns>
        public static object[] YBGRJZDJXXCXDR(object[] objParam)
        {
            string kssj = objParam[0].ToString();
            string zzsj = objParam[1].ToString();
            string grbh = objParam[2].ToString();

            if (string.IsNullOrWhiteSpace(kssj))
            {
                return new object[] { 0, 0, "医保提示：开始时间为空" };
            }
            else if (string.IsNullOrWhiteSpace(zzsj))
            {
                return new object[] { 0, 0, "医保提示：终止时间为空" };
            }
            else if (string.IsNullOrWhiteSpace(grbh))
            {
                return new object[] { 0, 0, "医保提示：个人编号为空" };
            }

            DateTime dqsj = Convert.ToDateTime(Common.GetServerTime());
            string jylsh = dqsj.ToString("yyyyMMddHHmmss") + "-" + DDYLJGBH + "-" + grbh;

            try
            {
                string rc = kssj + "|" + zzsj + "|" + grbh + "|";
                StringBuilder inputData = new StringBuilder(string.Format("{0}^{1}^{2}^{3}^{4}^{5}^{6}^{7}^", "1103", DDYLJGBH, CliUtils.fLoginUser, YWZQH, jylsh, ZXBM, rc, "1"));
                StringBuilder outputData = new StringBuilder(100000);
                int i = BUSINESS_HANDLE(inputData, outputData);

                if (i != 0)
                {
                    Common.InsertYBLog("", inputData.ToString(), outputData.ToString());
                }

                string tsxx = outputData.ToString().Split('^')[2];
                return new object[] { 0, i, tsxx };
            }
            catch (Exception error)
            {
                Common.InsertYBLog("", "", error.ToString());
                return new object[] { 0, 2, "Error:" + error.ToString() };
            }
        }
        #endregion 个人就诊登记信息查询东软

        #region 医疗费信息查询东软
        /// <summary>
        /// 医疗费信息查询东软
        /// </summary>
        /// <param>开始时间（格式：DateTime.Now.ToString("yyyyMMddHHmmss")）,终止时间（格式：DateTime.Now.ToString("yyyyMMddHHmmss")）</param>
        /// <returns>同预结算</returns>
        public static object[] YBYLFYXXCXDR(object[] objParam)
        {
            string kssj = objParam[0].ToString();
            string zzsj = objParam[1].ToString();

            if (string.IsNullOrWhiteSpace(kssj))
            {
                return new object[] { 0, 0, "医保提示：开始时间为空" };
            }
            else if (string.IsNullOrWhiteSpace(zzsj))
            {
                return new object[] { 0, 0, "医保提示：终止时间为空" };
            }

            DateTime dqsj = Convert.ToDateTime(Common.GetServerTime());
            string jylsh = dqsj.ToString("yyyyMMddHHmmss") + "-" + DDYLJGBH + "-" + CliUtils.fLoginUser;

            try
            {
                string rc = kssj + "|" + zzsj + "||";
                StringBuilder inputData = new StringBuilder(string.Format("{0}^{1}^{2}^{3}^{4}^{5}^{6}^{7}^", "1100", DDYLJGBH, CliUtils.fLoginUser, YWZQH, jylsh, ZXBM, rc, "1"));
                StringBuilder outputData = new StringBuilder(100000);
                int i = BUSINESS_HANDLE(inputData, outputData);

                if (i == 0)
                {
                    i = 1;
                }
                else
                {
                    Common.InsertYBLog("", inputData.ToString(), outputData.ToString());
                }

                string tsxx = outputData.ToString().Split('^')[2];
                return new object[] { 0, i, tsxx };
            }
            catch (Exception error)
            {
                Common.InsertYBLog("", "", error.ToString());
                return new object[] { 0, 2, "Error:" + error.ToString() };
            }
        }
        #endregion 医疗费信息查询东软

        #region 医疗费用明细信息查询东软
        /// <summary>
        /// 医疗费用明细信息查询东软
        /// </summary>
        /// <param>就诊流水号</param>
        /// <returns>定点医疗机构编号|就诊流水号|单据号|处方号|医院收费项目内码|医院收费项目名称|中心收费项目内码|中心收费项目名称|交易类型|个人编号|处方日期|结算时间|收费类别|收费项目等级|收费项目种类|单价|数量|金额|自理金额|超限价自付|最高限价|自费金额|自付比例|退费流水号|结算标志|审批编号|医院交易流水号|中心交易流水号</returns>
        public static object[] YBYLFYMXXXCXDR(object[] objParam)
        {
            string jzlsh = objParam[0].ToString();

            if (string.IsNullOrWhiteSpace(jzlsh))
            {
                return new object[] { 0, 0, "医保提示：就诊流水号为空" };
            }

            DateTime dqsj = Convert.ToDateTime(Common.GetServerTime());
            string jylsh = dqsj.ToString("yyyyMMddHHmmss") + "-" + DDYLJGBH + "-" + jzlsh;

            try
            {
                string rc = jzlsh + "|";
                StringBuilder inputData = new StringBuilder(string.Format("{0}^{1}^{2}^{3}^{4}^{5}^{6}^{7}^", "1200", DDYLJGBH, CliUtils.fLoginUser, YWZQH, jylsh, ZXBM, rc, "1"));
                StringBuilder outputData = new StringBuilder(100000);
                int i = BUSINESS_HANDLE(inputData, outputData);

                if (i == 0)
                {
                    i = 1;
                }
                else
                {
                    Common.InsertYBLog(jzlsh, inputData.ToString(), outputData.ToString());
                }

                string tsxx = outputData.ToString().Split('^')[2];
                return new object[] { 0, i, tsxx };
            }
            catch (Exception error)
            {
                Common.InsertYBLog("", "", error.ToString());
                return new object[] { 0, 2, "Error:" + error.ToString() };
            }
        }
        #endregion 医疗费用明细信息查询东软

        #region 结算信息查询东软
        /// <summary>
        /// 结算信息查询东软
        /// </summary>
        /// <param>就诊流水号,单据号</param>
        /// <returns>同预结算</returns>
        public static object[] YBJSXXCXDR(object[] objParam)
        {
            string jzlsh = objParam[0].ToString();
            string djh = objParam[1].ToString();

            if (string.IsNullOrWhiteSpace(jzlsh))
            {
                return new object[] { 0, 0, "医保提示：就诊流水号为空" };
            }

            DateTime dqsj = Convert.ToDateTime(Common.GetServerTime());
            string jylsh = dqsj.ToString("yyyyMMddHHmmss") + "-" + DDYLJGBH + "-" + djh;

            try
            {
                string rc = jzlsh + "|" + djh + "|";
                StringBuilder inputData = new StringBuilder(string.Format("{0}^{1}^{2}^{3}^{4}^{5}^{6}^{7}^", "1101", DDYLJGBH, CliUtils.fLoginUser, YWZQH, jylsh, ZXBM, rc, "1"));
                StringBuilder outputData = new StringBuilder(100000);
                int i = BUSINESS_HANDLE(inputData, outputData);

                if (i != 0)
                {
                    Common.InsertYBLog(jzlsh, inputData.ToString(), outputData.ToString());
                }

                string tsxx = outputData.ToString().Split('^')[2];
                return new object[] { 0, i, tsxx };
            }
            catch (Exception error)
            {
                Common.InsertYBLog(jzlsh, "", error.ToString());
                return new object[] { 0, 2, "Error:" + error.ToString() };
            }
        }
        #endregion 结算信息查询东软

        #region 个人医疗费信息查询东软
        /// <summary>
        /// 个人医疗费信息查询东软
        /// </summary>
        /// <param>开始时间（格式：DateTime.Now.ToString("yyyyMMddHHmmss")）,终止时间（格式：DateTime.Now.ToString("yyyyMMddHHmmss")）,个人编号</param>
        /// <returns>同预结算</returns>
        public static object[] YBGRYLFXXCXDR(object[] objParam)
        {
            string kssj = objParam[0].ToString();
            string zzsj = objParam[1].ToString();
            string grbh = objParam[2].ToString();

            if (string.IsNullOrWhiteSpace(kssj))
            {
                return new object[] { 0, 0, "医保提示：开始时间为空" };
            }
            else if (string.IsNullOrWhiteSpace(zzsj))
            {
                return new object[] { 0, 0, "医保提示：终止时间为空" };
            }
            else if (string.IsNullOrWhiteSpace(grbh))
            {
                return new object[] { 0, 0, "医保提示：个人编号为空" };
            }

            DateTime dqsj = Convert.ToDateTime(Common.GetServerTime());
            string jylsh = dqsj.ToString("yyyyMMddHHmmss") + "-" + DDYLJGBH + "-" + grbh;

            try
            {
                string rc = kssj + "|" + zzsj + "|" + grbh + "|";
                StringBuilder inputData = new StringBuilder(string.Format("{0}^{1}^{2}^{3}^{4}^{5}^{6}^{7}^", "1102", DDYLJGBH, CliUtils.fLoginUser, YWZQH, jylsh, ZXBM, rc, "1"));
                StringBuilder outputData = new StringBuilder(100000);
                int i = BUSINESS_HANDLE(inputData, outputData);

                if (i != 0)
                {
                    Common.InsertYBLog("", inputData.ToString(), outputData.ToString());
                }

                string tsxx = outputData.ToString().Split('^')[2];
                return new object[] { 0, i, tsxx };
            }
            catch (Exception error)
            {
                Common.InsertYBLog("", "", error.ToString());
                return new object[] { 0, 2, "Error:" + error.ToString() };
            }
        }
        #endregion 个人医疗费信息查询东软

        #region 个人基本信息及账户信息查询东软
        /// <summary>
        /// 个人基本信息及账户信息查询东软
        /// </summary>
        /// <param>身份证号</param>
        /// <returns>个人编号|单位编号|身份证号|姓名|性别|民族|出生日期|社会保障卡卡号|医疗待遇类别|人员参保状态|异地人员标志|统筹区号|年度|在院状态|帐户余额|本年医疗费累计|本年帐户支出累计|本年统筹支出累计|本年救助金支出累计|本年公务员补助基金累计|本年城镇居民门诊统筹支付累计|进入统筹费用累计|进入救助金费用累计|起付标准累计|本年住院次数|单位名称|年龄|参保单位类型|经办机构编码|</returns>
        public static object[] YBGRJBXXJZHXXCXDR(object[] objParam)
        {
            string sfzh = objParam[0].ToString();

            if (string.IsNullOrWhiteSpace(sfzh))
            {
                return new object[] { 0, 0, "医保提示：身份证号为空" };
            }

            DateTime dqsj = Convert.ToDateTime(Common.GetServerTime());
            string jylsh = dqsj.ToString("yyyyMMddHHmmss") + "-" + DDYLJGBH + "-" + sfzh;

            try
            {
                string rc = sfzh + "|";
                StringBuilder inputData = new StringBuilder(string.Format("{0}^{1}^{2}^{3}^{4}^{5}^{6}^{7}^", "1400", DDYLJGBH, CliUtils.fLoginUser, YWZQH, jylsh, ZXBM, rc, "1"));
                StringBuilder outputData = new StringBuilder(100000);
                int i = BUSINESS_HANDLE(inputData, outputData);

                if (i == 0)
                {
                    string tsxx = outputData.ToString().Split('^')[2];
                    return new object[] { 0, 1, tsxx };
                }
                else
                {
                    string tsxx = outputData.ToString();
                    Common.InsertYBLog("", inputData.ToString(), outputData.ToString());
                    return new object[] { 0, 0, tsxx };
                }
            }
            catch (Exception error)
            {
                Common.InsertYBLog("", "", error.ToString());
                return new object[] { 0, 2, "Error:" + error.ToString() };
            }
        }
        #endregion 个人基本信息及账户信息查询东软

        #region 医疗待遇封锁信息查询(一年内)东软
        /// <summary>
        /// 医疗待遇封锁信息查询(一年内)东软
        /// </summary>
        /// <param>个人编号,社会保障卡卡号</param>
        /// <returns>返回0为未封锁，其他为封锁</returns>
        public static object[] YBYLDYFSXXCXYNNDR(object[] objParam)
        {
            string grbh = objParam[0].ToString();
            string kh = objParam[1].ToString();
            ZXBM = "0000";
            DateTime dqsj = Convert.ToDateTime(Common.GetServerTime());
            string pdsj = dqsj.ToString("yyyyMMdd");

            if (string.IsNullOrWhiteSpace(grbh))
            {
                return new object[] { 0, -1, "医保提示：个人编号为空" };
            }

            if (string.IsNullOrWhiteSpace(kh))
            {
                return new object[] { 0, -1, "医保提示：社会保障卡卡号为空" };
            }

            string jylsh = dqsj.ToString("yyyyMMddHHmmss") + "-" + DDYLJGBH + "-" + grbh;
            StringBuilder outputData = new StringBuilder(100000);

            try
            {
                string rc = grbh + "|" + kh + "|" + pdsj + "|";
                StringBuilder inputData = new StringBuilder(string.Format("{0}^{1}^{2}^{3}^{4}^{5}^{6}^{7}^", "1500", DDYLJGBH, CliUtils.fLoginUser, YWZQH, jylsh, ZXBM, rc, "1"));
                int i = BUSINESS_HANDLE(inputData, outputData);
                string tsxx = outputData.ToString().Split('^')[2];

                if (!tsxx.Contains("|"))
                {
                    Common.InsertYBLog("", inputData.ToString(), outputData.ToString());
                    return new object[] { 0, -1, tsxx };
                }

                string bz = tsxx.Split('|')[0];

                if (bz == "0")
                {
                    bz = "1";
                }
                else if (bz == "1")
                {
                    bz = "0";
                }

                if (bz == "1")
                {
                    rc = grbh + "|" + kh + "|" + dqsj.AddYears(1).ToString("yyyyMMdd") + "|";
                    inputData = new StringBuilder(string.Format("{0}^{1}^{2}^{3}^{4}^{5}^{6}^{7}^", "1500", DDYLJGBH, CliUtils.fLoginUser, YWZQH, jylsh, "0000", rc, "1"));
                    outputData = new StringBuilder(100000);
                    i = BUSINESS_HANDLE(inputData, outputData);
                    tsxx = outputData.ToString().Split('^')[2];

                    if (!tsxx.Contains("|"))
                    {
                        Common.InsertYBLog("", inputData.ToString(), outputData.ToString());
                        return new object[] { 0, -1, tsxx };
                    }
                    else
                    {
                        return new object[] { 0, 1, tsxx.Split('|')[1] };
                    }
                }
                else
                {
                    Common.InsertYBLog("", inputData.ToString(), outputData.ToString());
                    return new object[] { 0, bz, tsxx.Split('|')[1] };
                }
            }
            catch (Exception error)
            {
                Common.InsertYBLog("", "", error.ToString());
                return new object[] { 0, 2, "Error:" + error.ToString() + outputData.ToString() };
            }
        }
        #endregion 医疗待遇封锁信息查询(一年内)东软

        #region 医疗待遇封锁信息查询(时间空)东软
        /// <summary>
        /// 医疗待遇封锁信息查询(时间空)东软
        /// </summary>
        /// <param>个人编号,社会保障卡卡号</param>
        /// <returns>返回0为未封锁，其他为封锁</returns>
        public static object[] YBYLDYFSXXCXSJKDR(object[] objParam)
        {
            string grbh = objParam[0].ToString();
            string kh = objParam[1].ToString();
            ZXBM = "0000";
            DateTime dqsj = Convert.ToDateTime(Common.GetServerTime());
            string pdsj = "";

            if (string.IsNullOrWhiteSpace(grbh))
            {
                return new object[] { 0, -1, "医保提示：个人编号为空" };
            }

            if (string.IsNullOrWhiteSpace(kh))
            {
                return new object[] { 0, -1, "医保提示：社会保障卡卡号为空" };
            }

            string jylsh = dqsj.ToString("yyyyMMddHHmmss") + "-" + DDYLJGBH + "-" + grbh;
            StringBuilder outputData = new StringBuilder(100000);

            try
            {
                string rc = grbh + "|" + kh + "|" + pdsj + "|";
                StringBuilder inputData = new StringBuilder(string.Format("{0}^{1}^{2}^{3}^{4}^{5}^{6}^{7}^", "1500", DDYLJGBH, CliUtils.fLoginUser, YWZQH, jylsh, ZXBM, rc, "1"));
                int i = BUSINESS_HANDLE(inputData, outputData);
                string tsxx = outputData.ToString().Split('^')[2];

                if (!tsxx.Contains("|"))
                {
                    Common.InsertYBLog("", inputData.ToString(), outputData.ToString());
                    return new object[] { 0, -1, tsxx };
                }

                string bz = tsxx.Split('|')[0];

                if (bz == "0")
                {
                    bz = "1";
                }
                else if (bz == "1")
                {
                    bz = "0";
                }

                if (bz == "1")
                {
                    return new object[] { 0, 1, tsxx.Split('|')[1] };
                }
                else
                {
                    Common.InsertYBLog("", inputData.ToString(), outputData.ToString());
                    return new object[] { 0, bz, tsxx.Split('|')[1] };
                }
            }
            catch (Exception error)
            {
                Common.InsertYBLog("", "", error.ToString());
                return new object[] { 0, 2, "Error:" + error.ToString() + outputData.ToString() };
            }
        }
        #endregion 医疗待遇封锁信息查询(时间空)东软

        #region 医疗待遇封锁信息查询东软
        /// <summary>
        /// 医疗待遇封锁信息查询东软
        /// </summary>
        /// <param>个人编号,社会保障卡卡号</param>
        /// <returns>返回0为未封锁，其他为封锁</returns>
        public static object[] YBYLDYFSXXCXDR(object[] objParam)
        {
            string grbh = objParam[0].ToString();
            string kh = objParam[1].ToString();
            ZXBM = "0000";
            DateTime dqsj = Convert.ToDateTime(Common.GetServerTime());

            if (string.IsNullOrWhiteSpace(grbh))
            {
                return new object[] { 0, -1, "医保提示：个人编号为空" };
            }

            if (string.IsNullOrWhiteSpace(kh))
            {
                return new object[] { 0, -1, "医保提示：社会保障卡卡号为空" };
            }

            string jylsh = dqsj.ToString("yyyyMMddHHmmss") + "-" + DDYLJGBH + "-" + grbh;
            StringBuilder outputData = new StringBuilder(100000);

            try
            {
                string rc = grbh + "|" + kh + "|" + dqsj.ToString("yyyyMMdd") + "|";
                StringBuilder inputData = new StringBuilder(string.Format("{0}^{1}^{2}^{3}^{4}^{5}^{6}^{7}^", "1500", DDYLJGBH, CliUtils.fLoginUser, YWZQH, jylsh, ZXBM, rc, "1"));
                int i = BUSINESS_HANDLE(inputData, outputData);
                string tsxx = outputData.ToString().Split('^')[2];

                if (!tsxx.Contains("|"))
                {
                    Common.InsertYBLog("", inputData.ToString(), outputData.ToString());
                    return new object[] { 0, -1, tsxx };
                }

                string bz = tsxx.Split('|')[0];

                if (bz == "0")
                {
                    bz = "1";
                }
                else if (bz == "1")
                {
                    bz = "0";
                }

                if (bz == "1")
                {
                    return new object[] { 0, 1, tsxx.Split('|')[1] };
                }
                else
                {
                    Common.InsertYBLog("", inputData.ToString(), outputData.ToString());
                    return new object[] { 0, bz, tsxx.Split('|')[1] };
                }
            }
            catch (Exception error)
            {
                Common.InsertYBLog("", "", error.ToString());
                return new object[] { 0, 2, "Error:" + error.ToString() + outputData.ToString() };
            }
        }
        #endregion 医疗待遇封锁信息查询东软

        #region 批量数据查询下载东软
        /// <summary>
        /// 批量数据查询下载东软
        /// </summary>
        /// <param>批量下载数据类别(01-药品目录,02-诊疗项目目录,03-医疗服务设施目录,04-病种编码信息,05-定点信息,06-三大目录政策参数信息,07-病种治疗方案)</param>
        /// <returns>文件路径及文件名称</returns>
        public static object[] YBPLSJCXXZDR(object[] objParam)
        {
            ZXBM = "0000";

            try
            {
                string sjlb = objParam[0].ToString();

                if (string.IsNullOrWhiteSpace(sjlb))
                {
                    return new object[] { 0, 0, "医保提示：数据类别为空" };
                }

                DateTime dqsj = Convert.ToDateTime(Common.GetServerTime());
                string jylsh = dqsj.ToString("yyyyMMddHHmmss") + "-" + DDYLJGBH + "-" + CliUtils.fLoginUser;
                string rc = sjlb + "|19000000000000|";
                StringBuilder inputData = new StringBuilder(string.Format("{0}^{1}^{2}^{3}^{4}^{5}^{6}^{7}^", "1300", DDYLJGBH, CliUtils.fLoginUser, YWZQH, jylsh, ZXBM, rc, "1"));
                StringBuilder outputData = new StringBuilder(100000);
                int i = BUSINESS_HANDLE(inputData, outputData);

                if (i == 0)
                {
                    return new object[] { 0, 1, outputData.ToString().Split('^')[2] };
                }
                else
                {
                    Common.InsertYBLog("", inputData.ToString(), outputData.ToString());
                    return new object[] { 0, 0, outputData };
                }
            }
            catch (Exception error)
            {
                Common.InsertYBLog("", "", error.ToString());
                return new object[] { 0, 2, "Error:" + error.ToString() };
            }
        }
        #endregion 批量数据查询下载东软

        #region 月结算费用对账东软
        /// <summary>
        /// 月结算费用对账东软
        /// </summary>
        /// <param>开始时间(YYYYMMDDHH24MISS),终止时间(YYYYMMDDHH24MISS),统筹区编码</param>
        /// <returns>文件路径及文件名称</returns>
        public static object[] YBYJSFYDZDR(object[] objParam)
        {
            ZXBM = "0000";

            try
            {
                string kssj = objParam[0].ToString();
                string zzsj = objParam[1].ToString();
                string tcqh = objParam[2].ToString();

                if (string.IsNullOrWhiteSpace(kssj))
                {
                    return new object[] { 0, 0, "医保提示：开始时间为空" };
                }

                if (string.IsNullOrWhiteSpace(zzsj))
                {
                    return new object[] { 0, 0, "医保提示：结束时间为空" };
                }

                if (string.IsNullOrWhiteSpace(tcqh))
                {
                    return new object[] { 0, 0, "医保提示：统筹区为空" };
                }

                DateTime dqsj = Convert.ToDateTime(Common.GetServerTime());
                string jylsh = dqsj.ToString("yyyyMMddHHmmss") + "-" + DDYLJGBH + "-" + CliUtils.fLoginUser;
                string rc = DDYLJGBH + "|" + kssj + "|" + zzsj + "|" + tcqh;
                StringBuilder inputData = new StringBuilder(string.Format("{0}^{1}^{2}^{3}^{4}^{5}^{6}^{7}^", "1140", DDYLJGBH, CliUtils.fLoginUser, YWZQH, jylsh, ZXBM, rc, "1"));
                StringBuilder outputData = new StringBuilder(100000);
                int i = BUSINESS_HANDLE(inputData, outputData);

                if (i == 0)
                {
                    return new object[] { 0, 1, outputData.ToString().Split('^')[2] };
                }
                else
                {
                    Common.InsertYBLog("", inputData.ToString(), outputData.ToString());
                    return new object[] { 0, 0, outputData };
                }
            }
            catch (Exception error)
            {
                Common.InsertYBLog("", "", error.ToString());
                return new object[] { 0, 2, "Error:" + error.ToString() };
            }
        }
        #endregion 月结算费用对账东软

        #region 医疗待遇审批信息查询
        /// <summary>
        /// 医疗待遇审批信息查询
        /// </summary>
        /// <param>个人编号,审批类别（空字符串为所有）</param>
        /// <returns>多条记录间以‘$’ 分割,审批标志|审批编号|个人编号|审批类别|就诊流水号|医院等级|病种编码|病种名称|医院意见|项目编码|项目名称|审批数量|申报日期|开始时间|终止时间|经办人|转外城市|备注|就诊医院编号|就诊医院名称</returns>
        /// 示例：1|2000051006|13400318|16|||SCD0302|脑血管意外后遗症|||||20150402152756|20150402|20161231|李颖|||||$1|3000009302|13400318|16|||SCD0009|高血压病|||||20100611151209|20150101|20161231|李颖|||||
        public static object[] YBYLDYSPXXCXDR(object[] objParam)
        {
            try
            {

                string cc = objParam[0].ToString().Split('^')[2];
                string splb = objParam[1].ToString();
                ZXBM = "0000";

                //if (string.IsNullOrWhiteSpace(grbh))
                //{
                //    return new object[] { 0, 0, "医保提示：个人编号为空" };
                //}

                //if (string.IsNullOrWhiteSpace(splb))
                //{
                //    return new object[] { 0, 0, "医保提示：审批类别为空" };
                //}

                DateTime dqsj = Convert.ToDateTime(Common.GetServerTime());
                string jylsh = dqsj.ToString("yyyyMMddHHmmss") + "-" + DDYLJGBH + "-" + cc;
                string rc = cc + "|" + splb + "||||||";
                StringBuilder inputData = new StringBuilder(string.Format("{0}^{1}^{2}^{3}^{4}^{5}^{6}^{7}^", "1600", DDYLJGBH, CliUtils.fLoginUser, YWZQH, jylsh, ZXBM, rc, "1"));
                StringBuilder outputData = new StringBuilder(100000);
                int i = BUSINESS_HANDLE(inputData, outputData);

                if (i == 0)
                {
                    return new object[] { 0, 1, outputData.ToString().Split('^')[2] };
                }
                else
                {
                    Common.InsertYBLog("", inputData.ToString(), outputData.ToString());
                    return new object[] { 0, 0, outputData };
                }
            }
            catch (Exception error)
            {
                Common.InsertYBLog("", "", error.ToString());
                return new object[] { 0, 2, "Error:" + error.ToString() };
            }
        }
        #endregion 医疗待遇审批信息查询

        #endregion 查询类

        #region 业务类

        #region 读卡东软
        /// <summary>
        /// 门诊读卡东软
        /// </summary>
        /// <param>无</param>
        /// <returns>个人编号|单位编号|身份证号|姓名|性别|民族|出生日期|社会保障卡卡号|医疗待遇类别|人员参保状态|异地人员标志|统筹区号|年度|在院状态|帐户余额|本年医疗费累计|本年帐户支出累计|本年统筹支出累计|本年救助金支出累计|本年公务员补助基金累计|本年城镇居民门诊统筹支付累计|进入统筹费用累计|进入救助金费用累计|起付标准累计|本年住院次数|单位名称|年龄|参保单位类型|经办机构编码|</returns>
        public static object[] YBDKDR(object[] objParam)
        {
            ZXBM = "0000";

            try
            {
                DateTime dqsj = Convert.ToDateTime(Common.GetServerTime());
                string jylsh = dqsj.ToString("yyyyMMddHHmmss") + "-" + DDYLJGBH + "-" + CliUtils.fLoginUser;
                StringBuilder inputData = new StringBuilder(string.Format("{0}^{1}^{2}^{3}^{4}^{5}^^{6}^", "2100", DDYLJGBH, CliUtils.fLoginUser, YWZQH, jylsh, ZXBM, "1"));
                StringBuilder outputData = new StringBuilder(100000);
                int i = BUSINESS_HANDLE(inputData, outputData);

                if (i == 0)
                {
                    string cc = outputData.ToString().Split('^')[2];

                    string[] str = cc.Split('|');

                    string ydrybz = cc.Split('|')[10];
                    string tcqh = cc.Split('|')[11];
                    string yldylb = cc.Split('|')[8];
                    #region 出参
                    /*保险号|姓名|卡号|地区编号|地区名称|出生日期|实际年龄|参保日期|个人身份|
                 * 单位名称|性别|医疗人员类别|卡状态|账户余额|累计住院支付|累计门诊支付|
                 * 累计特殊门诊|身份号码
                 */
                    string grbh = str[0].Trim();     //个人编号
                    string dwbh = str[1].Trim();  //单位编号
                    string sfzh = str[2].Trim();  //身份证号
                    string xm = str[3].Trim(); //姓名
                    string xb = str[4].Trim(); //性别
                    string csrq = str[6].Trim();  //出生日期
                    string ybkh = str[7].Trim();  //医保卡号
                    string dylb = str[8].Trim();  //待遇类别
                    string cbzt = str[9].Trim();  //参保状态
                    string ydbz = str[10].Trim();  //异地标识
                    string zhye = str[14].Trim(); //账户余额
                    string dwmc = str[25].Trim();  //单位名称
                    string nl = str[26].Trim();    //年龄
                    string cbd = str[11].Trim();    //统筹区号
                    #endregion
                    //if (yldylb == "99")
                    //{
                    //    return new object[] { 0, 0, "此人待遇类别为99其他，请联系参保地备案" };
                    //}

                    List<string> liSQL = new List<string>();
                    string strSql = string.Format("delete from YBICKXX where grbh='{0}'", grbh);
                    liSQL.Add(strSql);
                    strSql = string.Format(@"insert into YBICKXX(grbh,dwbh,sfzh,xm,xb,csrq,ybkh,dylb,cbzt,ydbz,
                                        zhye,dwmc,cbd) values(
                                        '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}',
                                        '{10}','{11}','{12}')",
                                            grbh, dwbh, sfzh, xm, xb, csrq, ybkh, dylb, cbzt, ydbz,
                                            zhye, dwmc, cbd);
                    liSQL.Add(strSql);
                    object[] obj = liSQL.ToArray();
                    obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);


                    if (ydrybz == "1")
                    {
                        ZXBM = tcqh;
                    }

                    if (obj[1].ToString().Equals("1"))
                    {
                        return new object[] { 0, 1, outputData };
                    }
                    else
                    {
                        WriteLog(Common.GetServerTime() + "  门诊读卡成功|保存本地数据失败|" + obj[2].ToString());
                        return new object[] { 0, 2, "门诊读卡成功|" + obj[2].ToString() };
                    }
                }
                else
                {
                    Common.InsertYBLog("", inputData.ToString(), outputData.ToString());
                    return new object[] { 0, 0, outputData };
                }
            }
            catch (Exception error)
            {
                Common.InsertYBLog("", "", error.ToString());
                return new object[] { 0, 2, "Error:" + error.ToString() };
            }
        }

        /// <summary>
        /// 住院读卡东软
        /// </summary>
        /// <param>无</param>
        /// <returns>个人编号|单位编号|身份证号|姓名|性别|民族|出生日期|社会保障卡卡号|医疗待遇类别|人员参保状态|异地人员标志|统筹区号|年度|在院状态|帐户余额|本年医疗费累计|本年帐户支出累计|本年统筹支出累计|本年救助金支出累计|本年公务员补助基金累计|本年城镇居民门诊统筹支付累计|进入统筹费用累计|进入救助金费用累计|起付标准累计|本年住院次数|单位名称|年龄|参保单位类型|经办机构编码|</returns>
        public static object[] YBZYDKDR(object[] objParam)
        {
            ZXBM = "0000";

            try
            {
                DateTime dqsj = Convert.ToDateTime(Common.GetServerTime());
                string jylsh = dqsj.ToString("yyyyMMddHHmmss") + "-" + DDYLJGBH + "-" + CliUtils.fLoginUser;
                StringBuilder inputData = new StringBuilder(string.Format("{0}^{1}^{2}^{3}^{4}^{5}^^{6}^", "2100", DDYLJGBH, CliUtils.fLoginUser, YWZQH, jylsh, ZXBM, "1"));
                StringBuilder outputData = new StringBuilder(100000);
                int i = BUSINESS_HANDLE(inputData, outputData);

                if (i == 0)
                {
                    string cc = outputData.ToString().Split('^')[2];

                    string[] str = cc.Split('|');

                    string ydrybz = cc.Split('|')[10];
                    string tcqh = cc.Split('|')[11];
                    string yldylb = cc.Split('|')[8];
                    #region 出参
                    /*保险号|姓名|卡号|地区编号|地区名称|出生日期|实际年龄|参保日期|个人身份|
                 * 单位名称|性别|医疗人员类别|卡状态|账户余额|累计住院支付|累计门诊支付|
                 * 累计特殊门诊|身份号码
                 */
                    string grbh = str[0].Trim();     //个人编号
                    string dwbh = str[1].Trim();  //单位编号
                    string sfzh = str[2].Trim();  //身份证号
                    string xm = str[3].Trim(); //姓名
                    string xb = str[4].Trim(); //性别
                    string csrq = str[6].Trim();  //出生日期
                    string ybkh = str[7].Trim();  //医保卡号
                    string dylb = str[8].Trim();  //待遇类别
                    string cbzt = str[9].Trim();  //参保状态
                    string ydbz = str[10].Trim();  //异地标识
                    string zhye = str[14].Trim(); //账户余额
                    string dwmc = str[25].Trim();  //单位名称
                    string nl = str[26].Trim();    //年龄
                    string cbd = str[11].Trim();    //统筹区号
                    #endregion
                    //if (yldylb == "99")
                    //{
                    //    return new object[] { 0, 0, "此人待遇类别为99其他，请联系参保地备案" };
                    //}

                    List<string> liSQL = new List<string>();
                    string strSql = string.Format("delete from YBICKXX where grbh='{0}'", grbh);
                    liSQL.Add(strSql);
                    strSql = string.Format(@"insert into YBICKXX(grbh,dwbh,sfzh,xm,xb,csrq,ybkh,dylb,cbzt,ydbz,
                                        zhye,dwmc,cbd) values(
                                        '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}',
                                        '{10}','{11}','{12}')",
                                            grbh, dwbh, sfzh, xm, xb, csrq, ybkh, dylb, cbzt, ydbz,
                                            zhye, dwmc,cbd);
                    liSQL.Add(strSql);
                    object[] obj = liSQL.ToArray();
                    obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);


                    if (ydrybz == "1")
                    {
                        ZXBM = tcqh;
                    }

                    if (obj[1].ToString().Equals("1"))
                    {
                        return new object[] { 0, 1, outputData };
                    }
                    else
                    {
                        WriteLog(Common.GetServerTime() + "  住院读卡成功|保存本地数据失败|" + obj[2].ToString());
                        return new object[] { 0, 2, "住院读卡成功|" + obj[2].ToString() };
                    }
                }
                else
                {
                    Common.InsertYBLog("", inputData.ToString(), outputData.ToString());
                    return new object[] { 0, 0, outputData };
                }
            }
            catch (Exception error)
            {
                Common.InsertYBLog("", "", error.ToString());
                return new object[] { 0, 2, "Error:" + error.ToString() };
            }
        }
        #endregion 读卡东软

        #region 冲正交易东软
        /// <summary>
        /// 冲正交易东软
        /// </summary>
        /// <param>冲正业务交易编码,被冲正交易医院交易流水号,中心编码</param>
        /// <returns>无</returns>
        public static object[] YBCZJYDR(object[] objParam)
        {
            try
            {
                string ywbh = objParam[0].ToString();
                string cxjylsh = objParam[1].ToString();
                ZXBM = objParam[2].ToString();

                if (string.IsNullOrWhiteSpace(ywbh))
                {
                    return new object[] { 0, 0, "医保提示：冲正业务交易编码为空" };
                }

                if (string.IsNullOrWhiteSpace(cxjylsh))
                {
                    return new object[] { 0, 0, "医保提示：被冲正交易医院交易流水号为空" };
                }

                DateTime dqsj = Convert.ToDateTime(Common.GetServerTime());
                string jylsh = dqsj.ToString("yyyyMMddHHmmss") + "-" + DDYLJGBH + "-" + CliUtils.fLoginUser;
                string rc = ywbh + "|" + cxjylsh + "|";
                StringBuilder inputData = new StringBuilder(string.Format("{0}^{1}^{2}^{3}^{4}^{5}^{6}^{7}^", "2421", DDYLJGBH, CliUtils.fLoginUser, YWZQH, jylsh, ZXBM, rc, "1"));
                StringBuilder outputData = new StringBuilder(100000);
                int i = BUSINESS_HANDLE(inputData, outputData);

                if (i == 0)
                {
                    return new object[] { 0, 1, outputData };
                }
                else
                {
                    Common.InsertYBLog("", inputData.ToString(), outputData.ToString());
                    return new object[] { 0, 0, outputData };
                }
            }
            catch (Exception error)
            {
                Common.InsertYBLog("", "", error.ToString());
                return new object[] { 0, 2, error.ToString() };
            }
        }
        #endregion 冲正交易东软

        #region 门诊登记
        /// <summary>
        /// 门诊登记
        /// </summary>
        /// <param>his就诊流水号,医疗类别代码,病种编码,病种名称,卡信息(读卡返回的一串个人信息),登记时间(格式：yyyy-MM-dd HH:mm:ss)</param>
        /// <returns>1:成功，0:不成功，2:报错</returns>
        public static object[] YBMZDJDR(object[] objParam)
        {
            bool isybok = false;
            string ybjzlsh = "";
            string grbh = "";
            string xm = "";
            string kh = "";

            try
            {
                string czygh = CliUtils.fLoginUser;
                string ywzqh = YWZQH;
                string jbr = CliUtils.fUserName;
                string jzlsh = objParam[0].ToString(); //就诊流水号
                string yllb = objParam[1].ToString(); //医疗类别代码
                string bzbm = objParam[2].ToString(); //病种编码
                string bzmc = objParam[3].ToString(); //病种名称
                string[] kxx = objParam[4].ToString().Split('|');
                grbh = kxx[0].Split('^')[2].ToString();//个人编号
                string dwbh = kxx[1].ToString(); //单位编号
                string sfzh = kxx[2].ToString(); //身份证号
                xm = kxx[3].ToString(); //姓名
                string xb = kxx[4].ToString(); //性别
                string mz = kxx[5].ToString(); //民族
                string csrq = kxx[6].ToString(); //出生日期
                kh = kxx[7].ToString(); //卡号
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
                DateTime time = Convert.ToDateTime(objParam[5].ToString());
                string ghdjsj = time.ToString("yyyyMMddHHmmss");
                
                //string ghdjsj = Convert.ToDateTime(objParam[5]).ToString("yyyyMMddHHmmss");
                object[] obj = null;
                string fsxx = "";
                string dkydrybz = ydrybz;

                if ("99" == yldylb)
                {
                    //return new object[] { 0, 0, "此人待遇类别为99其他，请联系参保地备案" };
                    MessageBox.Show("医保提示：此人待遇类别为99其他", "提示");
                }

                if (ydrybz != "0")
                {
                    string strSqlyd = string.Format(@"select a.bzmem1 from bztbd a where a.bzcodn = 'DQ' and a.bzkeyx = '{0}' and a.bzmem1 = '0'", tcqh);
                    DataSet dsyd = CliUtils.ExecuteSql("sybdj", "cmd", strSqlyd, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);

                    if (dsyd != null && dsyd.Tables[0].Rows.Count > 0)
                    {
                        ydrybz = "0";
                    }
                    else
                    {
                        if (MessageBox.Show("你确定该患者走异地报销吗？", "确认", MessageBoxButtons.YesNo) == DialogResult.No)
                        {
                            ydrybz = "0";
                        }
                    }
                }

                ZXBM = ydrybz == "0" ? "0000" : tcqh;

                if (ydrybz == "0")//本地
                {
                    obj = new object[] { grbh, kh };
                    obj = YBYLDYFSXXCXDR(obj);

                    if (obj[1].ToString() == "-2")//错误
                    {
                        MessageBox.Show("医保提示：封锁信息查询异常", "提示");
                        return obj;
                    }
                    else if (obj[1].ToString() != "1")//封锁
                    {
                        if (DialogResult.Cancel == MessageBox.Show("医保提示：这张卡已被封锁，信息：" + obj[2].ToString() + "，是否继续医保挂号登记", "提示", MessageBoxButtons.OKCancel))
                        {
                            return obj;
                        }
                    }

                    fsxx = obj[2].ToString();
                }
                else if (yllb != "11" && yllb != "13")
                {
                    return new object[] { 0, 0, "医保提示：异地就医医疗类别只能是普通门诊或门诊特殊慢性病" };
                }

                if (string.IsNullOrWhiteSpace(jzlsh))
                {
                    return new object[] { 0, 0, "医保提示：his就诊流水号为空" };
                }
                else if (string.IsNullOrWhiteSpace(yllb))
                {
                    return new object[] { 0, 0, "医保提示：医疗类别代码为空" };
                }
                else if (string.IsNullOrWhiteSpace(ghdjsj))
                {
                    return new object[] { 0, 0, "医保提示：登记时间为空" };
                }

                string strSql = "";
                DataSet ds = null;

                if (ydrybz == "0")
                {
                    strSql = string.Format("select a.jzlsh from ybmzzydjdr a where a.jzlsh = '{0}' and a.cxbz = 1 and a.jzbz = 'm'", jzlsh);//0秒
                    ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);

                    if (ds != null && ds.Tables[0].Rows.Count > 0)
                    {
                        return new object[] { 0, 0, "医保提示：his就诊流水号" + jzlsh + "已登记医保挂号" };
                    }
                }

                strSql = string.Format(@"select top 1 a.m1ksno, d.b2ksnm m1ksnm, a.m1name hzxm from mz01h a join bz02h d on a.m1ksno = d.b2ksno 
                where a.m1ghno = '{0}'", jzlsh);
                ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);

                if (ds == null || ds.Tables[0].Rows.Count == 0)
                {
                    return new object[] { 0, 0, "医保提示：his就诊流水号" + jzlsh + "未登记his挂号" };
                }

                DataRow dr = ds.Tables[0].Rows[0];
                string ksbh = dr["m1ksno"].ToString();
                string ksmc = dr["m1ksnm"].ToString();
                string hzxm = dr["hzxm"].ToString();

                if (!string.Equals(hzxm, xm))
                {
                    return new object[] { 0, 0, "医保提示：his就诊流水号" + jzlsh + "挂号登记姓名和医保卡姓名不相符" };
                }

                DateTime dqsj = Convert.ToDateTime(Common.GetServerTime());
                string jylsh = dqsj.ToString("yyyyMMddHHmmss") + "-" + DDYLJGBH + "-" + jzlsh;
                string rc = jzlsh + "|" + yllb + "|" + ghdjsj + "|" + bzbm + "|" + bzmc + "|" + ksbh + "|" + ksmc + "||||" + jbr + "|" + grbh + "||||||" + xm + "|" + kh + "|||";

                //if (ydrybz != "0")
                //{
                //    yllb = "11";
                //    bzbm = "";
                //    bzmc = "";
                //    rc = jzlsh + "|" + yllb + "|" + ghdjsj + "|" + bzbm + "|" + bzmc + "|" + ksbh + "|" + ksmc + "||||" + jbr + "|" + grbh + "||||||" + xm + "|" + kh + "|||";
                //}

                StringBuilder inputData = new StringBuilder(string.Format("{0}^{1}^{2}^{3}^{4}^{5}^{6}^{7}^", "2210", DDYLJGBH, czygh, ywzqh, jylsh, ZXBM, rc, "1"));
                StringBuilder outputData = new StringBuilder(100000);
                int i = BUSINESS_HANDLE(inputData, outputData);
                ybjzlsh = jzlsh;

                if (i == 0)
                {
                    string bzbmmcs = "";

                    if (ydrybz != "0")
                    {
                        string yddjfh = outputData.ToString().Split('^')[2];
                        ybjzlsh = yddjfh.Split('|')[0];

                        if (yddjfh.Contains("|") && yddjfh.Length > yddjfh.IndexOf('|') + 1)
                        {
                            bzbmmcs = yddjfh.Substring(yddjfh.IndexOf('|') + 1).TrimEnd(';');
                        }

                    }

                    isybok = true;
                    strSql = string.Format(@"insert into ybmzzydjdr(jzlsh, jylsh, dwbh, sfzh, xb, mz, csrq, yldylb, rycbzt, ydrybz, tcqh, nd
                    , zyzt, zhye, bnylflj, bnzhzclj, bntczclj, bnjzjzclj, bngwybzjjlj, bnczjmmztczflj, jrtcfylj, jrjzjfylj, qfbzlj, bnzycs, dwmc
                    , nl, cbdwlx, jbjgbm, yllb, ghdjsj, bzbm, bzmc, ksbh, ksmc, ysdm, ysxm, jbr, grbh, xm, kh, ybjzlsh, jzbz, bzbmmcs, dkydrybz, fsxx, kxx) 
                    values('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', {13}, {14}, {15}, {16}
                    , {17}, {18}, {19}, {20}, {21}, {22}, '{23}', '{24}', '{25}', '{26}', '{27}', '{28}', '{29}', '{30}', '{31}', '{32}', '{33}'
                    , '{34}', '{35}', '{36}', '{37}', '{38}', '{39}', '{40}', '{41}', '{42}', '{43}', '{44}', '{45}')"
                    , jzlsh, jylsh, dwbh, sfzh, xb, mz, csrq, yldylb, rycbzt, ydrybz, tcqh, nd, zyzt, zhye, bnylflj, bnzhzclj, bntczclj, bnjzjzclj
                    , bngwybzjjlj, bnczjmmztczflj, jrtcfylj, jrjzjfylj, qfbzlj, bnzycs, dwmc, nl, cbdwlx, jbjgbm, yllb, ghdjsj, bzbm, bzmc
                    , ksbh, ksmc, DBNull.Value, DBNull.Value, jbr, grbh, xm, kh, ybjzlsh, "m", bzbmmcs, dkydrybz, fsxx, objParam[4].ToString());
                    obj = new object[] { strSql };
                    obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);

                    if (obj[1].ToString() == "1")
                    {
                        return new object[] { 0, 1, ybjzlsh };
                    }
                    else
                    {
                        i = YBDJSDCXDR(ybjzlsh, grbh, xm, kh, ZXBM);

                        if (i == 1)
                        {
                            string tsxx = "医保提示：his数据库操作失败,自动撤销医保挂号登记成功,错误信息：" + obj[2].ToString();
                            Common.InsertYBLog(jzlsh, "", tsxx);
                            return new object[] { 0, 0, tsxx };
                        }
                        else
                        {
                            string tsxx = "医保提示：his数据库操作失败,自动撤销医保挂号登记失败,错误信息：" + obj[2].ToString();
                            Common.InsertYBLog(jzlsh, "", tsxx);
                            return new object[] { 0, 0, tsxx };
                        }
                    }
                }
                else if (i == -1)
                {
                    Common.InsertYBLog(jzlsh, inputData.ToString(), outputData.ToString());
                    i = YBDJSDCXDR(ybjzlsh, grbh, xm, kh, ZXBM);//如果异地，医保就诊流水号还没返回，所以撤销可能有问题

                    if (i == 1)
                    {
                        return new object[] { 0, -1, "医保提示：医保挂号登记系统级别错误，自动撤销医保挂号登记成功，" + outputData.ToString() };
                    }
                    else
                    {
                        return new object[] { 0, -1, "医保提示：医保挂号登记系统级别错误，自动撤销医保挂号登记失败，" + outputData.ToString() };
                    }
                }
                else
                {
                    Common.InsertYBLog(jzlsh, inputData.ToString(), outputData.ToString());
                    return new object[] { 0, -2, "医保提示：医保挂号登记业务级别或未知错误，" + outputData.ToString() };
                }
            }
            catch (Exception error)
            {
                Common.InsertYBLog("", "", error.ToString());

                if (isybok)
                {
                    int i = YBDJSDCXDR(ybjzlsh, grbh, xm, kh, ZXBM);

                    if (i == 1)
                    {
                        return new object[] { 0, 2, "医保提示：非医保异常，医保挂号登记自动撤销成功，" + error.ToString() };
                    }
                    else
                    {
                        return new object[] { 0, 2, "医保提示：非医保异常，医保挂号登记自动撤销失败，" + error.ToString() };
                    }
                }

                return new object[] { 0, 2, "Error:" + error.ToString() };
            }
        }
        #endregion 门诊登记

        #region 门诊登记撤销
        /// <summary>
        /// 门诊登记撤销
        /// </summary>
        /// <param>his就诊流水号</param>
        /// <returns>1:成功，0:不成功，2:报错</returns>
        public static object[] YBMZDJCXDR(object[] objParam)
        {
            string czygh = CliUtils.fLoginUser;
            string ywzqh = YWZQH;
            string jbr = CliUtils.fUserName;
            string jzlsh = objParam[0].ToString();
            DateTime dqsj = Convert.ToDateTime(Common.GetServerTime());
            string jylsh = dqsj.ToString("yyyyMMddHHmmss") + "-" + DDYLJGBH + "-" + jzlsh;

            try
            {
                string strSql = string.Format("select top 1 grbh, xm, kh, ybjzlsh, ydrybz, tcqh from ybmzzydjdr a where a.jzlsh = '{0}' and a.cxbz = 1 and a.jzbz = 'm' order by sysdate desc", jzlsh);//0秒
                DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);

                if (ds == null || ds.Tables[0].Rows.Count == 0)
                {
                    return new object[] { 0, 0, "医保门诊登记撤销提示：医保未办理挂号登记" };
                }

                DataRow dr = ds.Tables[0].Rows[0];
                string grbh = dr["grbh"].ToString();
                string xm = dr["xm"].ToString();
                string kh = dr["kh"].ToString();
                string ybjzlsh = dr["ybjzlsh"].ToString();
                string ydrybz = dr["ydrybz"].ToString();
                ZXBM = ydrybz == "0" ? "0000" : dr["tcqh"].ToString();
                strSql = string.Format("select a.jzlsh from ybfyjsdr a where a.jzlsh = '{0}' and a.cxbz = 1 and (a.ybjzlsh = '{1}' or a.ybjzlsh is null) order by a.sysdate desc", jzlsh, ybjzlsh);//0秒
                ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);

                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    return new object[] { 0, 0, "医保门诊登记撤销提示：医保已结算" };
                }

                string rc = ybjzlsh + "|" + jbr + "|" + grbh + "|" + xm + "|" + kh + "|";
                StringBuilder inputData = new StringBuilder(string.Format("{0}^{1}^{2}^{3}^{4}^{5}^{6}^{7}^", "2240", DDYLJGBH, czygh, ywzqh, jylsh, ZXBM, rc, "1"));
                StringBuilder outputData = new StringBuilder(100000);
                int i = BUSINESS_HANDLE(inputData, outputData);

                if (i == 0)
                {
                    string strSql1 = string.Format(@"insert into ybmzzydjdr(jzlsh, jylsh, dwbh, sfzh, xb, mz, csrq, yldylb, rycbzt, ydrybz, tcqh
                    , nd, zyzt, zhye, bnylflj, bnzhzclj, bntczclj, bnjzjzclj, bngwybzjjlj, bnczjmmztczflj, jrtcfylj, jrjzjfylj, qfbzlj, bnzycs
                    , dwmc, nl, cbdwlx, jbjgbm, elmmxezc, elmmxesy, yldyxz, gsdyxz, sydyxz, yllb, ghdjsj, bzbm, bzmc, ksbh, ksmc, cwh, ysdm
                    , ysxm, jbr, grbh, bq, zzyybh, zzyymc, bz, tsfybz, xm, kh, ghf, ybzl, mmbzbm1, mmbzmc1, mmbzbm2, mmbzmc2, mmbzbm3
                    , mmbzmc3, mmbzbm4, mmbzmc4, ybjzlsh, jzbz, cxbz) 
                    select jzlsh, jylsh, dwbh, sfzh, xb, mz, csrq, yldylb, rycbzt, ydrybz, tcqh, nd, zyzt, zhye, bnylflj, bnzhzclj, bntczclj
                    , bnjzjzclj, bngwybzjjlj, bnczjmmztczflj, jrtcfylj, jrjzjfylj, qfbzlj, bnzycs, dwmc, nl, cbdwlx, jbjgbm, elmmxezc, elmmxesy
                    , yldyxz, gsdyxz, sydyxz, yllb, ghdjsj, bzbm, bzmc, ksbh, ksmc, cwh, ysdm, ysxm, '{1}', grbh, bq, zzyybh, zzyymc, bz, tsfybz
                    , xm, kh, ghf, ybzl, mmbzbm1, mmbzmc1, mmbzbm2, mmbzmc2, mmbzbm3, mmbzmc3, mmbzbm4, mmbzmc4, ybjzlsh, jzbz, 0 
                    from ybmzzydjdr where jzlsh = '{0}' and cxbz = 1 and jzbz = 'm' and ybjzlsh = '{2}'", jzlsh, jbr, ybjzlsh);//0秒
                    string strSql2 = string.Format("update ybmzzydjdr set cxbz = 2 where jzlsh = '{0}' and cxbz = 1 and jzbz = 'm' and ybjzlsh = '{1}'", jzlsh, ybjzlsh);//0秒
                    object[] obj = new object[] { strSql1, strSql2 };
                    obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);

                    if (obj[1].ToString() == "1")
                    {
                        return new object[] { 0, 1, outputData };
                    }
                    else
                    {
                        Common.InsertYBLog(jzlsh, "", obj[2].ToString());
                        return new object[] { 0, 0, "医保门诊登记撤销提示：his数据库操作失败：" + obj[2].ToString() };
                    }
                }
                else if (i == -1)
                {
                    Common.InsertYBLog(jzlsh, inputData.ToString(), outputData.ToString());
                    string strSql1 = string.Format(@"insert into ybmzzydjdr(jzlsh, jylsh, dwbh, sfzh, xb, mz, csrq, yldylb, rycbzt, ydrybz, tcqh
                    , nd, zyzt, zhye, bnylflj, bnzhzclj, bntczclj, bnjzjzclj, bngwybzjjlj, bnczjmmztczflj, jrtcfylj, jrjzjfylj, qfbzlj, bnzycs
                    , dwmc, nl, cbdwlx, jbjgbm, elmmxezc, elmmxesy, yldyxz, gsdyxz, sydyxz, yllb, ghdjsj, bzbm, bzmc, ksbh, ksmc, cwh, ysdm
                    , ysxm, jbr, grbh, bq, zzyybh, zzyymc, bz, tsfybz, xm, kh, ghf, ybzl, mmbzbm1, mmbzmc1, mmbzbm2, mmbzmc2, mmbzbm3
                    , mmbzmc3, mmbzbm4, mmbzmc4, ybjzlsh, jzbz, cxbz) 
                    select jzlsh, jylsh, dwbh, sfzh, xb, mz, csrq, yldylb, rycbzt, ydrybz, tcqh, nd, zyzt, zhye, bnylflj, bnzhzclj, bntczclj
                    , bnjzjzclj, bngwybzjjlj, bnczjmmztczflj, jrtcfylj, jrjzjfylj, qfbzlj, bnzycs, dwmc, nl, cbdwlx, jbjgbm, elmmxezc, elmmxesy
                    , yldyxz, gsdyxz, sydyxz, yllb, ghdjsj, bzbm, bzmc, ksbh, ksmc, cwh, ysdm, ysxm, '{1}', grbh, bq, zzyybh, zzyymc, bz, tsfybz
                    , xm, kh, ghf, ybzl, mmbzbm1, mmbzmc1, mmbzbm2, mmbzmc2, mmbzbm3, mmbzmc3, mmbzbm4, mmbzmc4, ybjzlsh, jzbz, 0 
                    from ybmzzydjdr where jzlsh = '{0}' and cxbz = 1 and jzbz = 'm' and ybjzlsh = '{2}'", jzlsh, jbr, ybjzlsh);//0秒
                    string strSql2 = string.Format("update ybmzzydjdr set cxbz = 2 where jzlsh = '{0}' and cxbz = 1 and jzbz = 'm' and ybjzlsh = '{1}'", jzlsh, ybjzlsh);//0秒
                    object[] obj = new object[] { strSql1, strSql2 };
                    obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);

                    if (obj[1].ToString() == "1")
                    {
                        return new object[] { 0, -1, "医保提示：门诊登记撤销系统级别错误，自动删除医保登记成功，" + outputData.ToString() };
                    }
                    else
                    {
                        Common.InsertYBLog(jzlsh, "", obj[2].ToString());
                        return new object[] { 0, -1, "医保提示：门诊登记撤销系统级别错误，自动删除医保登记失败，" + obj[2].ToString() };
                    }
                }
                else
                {
                    Common.InsertYBLog(jzlsh, inputData.ToString(), outputData.ToString());
                    return new object[] { 0, -2, "医保提示：门诊登记撤销业务级别或未知错误，" + outputData.ToString() };
                }
            }
            catch (Exception error)
            {
                Common.InsertYBLog(jzlsh, "", error.ToString());
                return new object[] { 0, 2, "医保门诊登记撤销Error:" + error.ToString() };
            }
        }
        #endregion 门诊登记撤销

        #region 门诊登记收费
        /// <summary>
        /// 门诊登记收费
        /// </summary>
        /// <param>his就诊流水号,医疗类别代码,登记时间(格式：yyyy-MM-dd HH:mm:ss),病种编码（慢性病要传，否则传空字符串）,病种名称（慢性病要传，否则传空字符串）,卡信息(读卡返回的一串个人信息)，读卡标志(不传表示读卡，传了则不读卡)，定岗医生代码，定岗医生姓名</param>
        /// <returns>1:成功(医疗费总额|总报销金额|统筹基金支付|大额基金支付|账户支付|现金支付|公务员补助基金支付|企业补充医疗保险基金支付|自费费用|单位负担费用|医院负担费用|民政救助费用|超限价费用|乙类自理费用|丙类自理费用|符合基本医疗费用|起付标准费用|转诊转院自付费用|进入统筹费用|统筹分段自付费用|超统筹封顶线费用|进入大额报销费用|大额分段自付费用|超大额封顶线费用|人工器官自付费用|本次结算前帐户余额|本年统筹支付累计(不含本次)|本年大额支付累计(不含本次)|本年城镇居民门诊统筹支付累计(不含本次)|本年公务员补助支付累计(不含本次)|本年账户支付累计(不含本次)|本年住院次数累计(不含本次)|住院次数|姓名|结算时间|医疗类别|医疗待遇类别|经办机构编码|业务周期号|结算流水号|提示信息|单据号|交易类型|医院交易流水号|有效标志|个人编号|医疗机构编码|二次补偿金额|门慢起付累计|接收方交易流水号|个人编号|单病种补差|财政支出金额|二类门慢限额支出|二类门慢限额剩余|居民个人自付二次补偿金额|体检金额|生育基金支付|),0:失败，2:报错</returns>
        public static object[] YBMZDJSFDR(object[] objParam)
        {
            int isybghjsok = 0;//1:医保挂号成功,2:医保挂号数据库操作成功,3:医保处方明细上传成功,4:医保处方明细上传数据库操作成功
            string ybjzlsh = "";
            string grbh = "";
            string xm = "";
            string kh = "";
            string jzlsh = "";
            string djh = "";
            string ghdjsj = "";


            try
            {
                jzlsh = objParam[0].ToString();//his就诊流水号
                string yllb = objParam[1].ToString();//医疗类别代码
                ghdjsj = Convert.ToDateTime(objParam[2]).ToString("yyyyMMddHHmmss");//挂号时间
                string bzbm = objParam[3].ToString();//病种编码
                string bzmc = objParam[4].ToString();//病种名称
                string[] kxx = objParam[5].ToString().Split('|');
                decimal hisylfze = decimal.Zero;
                //decimal hisylfze = Convert.ToDecimal(objParam[6]);
                grbh = kxx[0].Split('^')[2].ToString();//个人编号
                string dwbh = kxx[1].ToString();//单位编号
                string sfzh = kxx[2].ToString(); //身份证号
                xm = kxx[3].ToString();//姓名
                string xb = kxx[4].ToString(); //性别
                string mz = kxx[5].ToString(); //民族
                string csrq = kxx[6].ToString(); //出生日期
                kh = kxx[7].ToString();//卡号
                string yldylb = kxx[8].ToString();  //医疗待遇类别
                string rycbzt = kxx[9].ToString(); //人员参保状态
                string ydrybz = kxx[10].ToString();  //异地人员标志
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
                object[] obj = null;
                string fsxx = "";
                string dkydrybz = ydrybz;


                //object[] objdj  = YBMZDJDR(new object[] {jzlsh,yllb,bzbm,bzmc,objParam[5].ToString(),ghdjsj});
                //if (objdj[1].ToString().Equals("1"))//门诊挂号登记成功
                //{
                if (yldylb == "99")
                {
                    //return new object[] { 0, 0, "此人待遇类别为99其他，请联系参保地备案" };
                    MessageBox.Show("医保提示：此人待遇类别为99其他", "提示");
                }

                if (ydrybz != "0")
                {
                    string strSqlyd = string.Format(@"select a.bzmem1 from bztbd a where a.bzcodn = 'DQ' and a.bzkeyx = '{0}' and a.bzmem1 = '0'", tcqh);
                    DataSet dsyd = CliUtils.ExecuteSql("sybdj", "cmd", strSqlyd, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);

                    if (dsyd != null && dsyd.Tables[0].Rows.Count > 0)
                    {
                        ydrybz = "0";
                    }
                    else
                    {
                        if (MessageBox.Show("你确定该患者走异地报销吗？", "确认", MessageBoxButtons.YesNo) == DialogResult.No)
                        {
                            ydrybz = "0";
                        }
                    }
                }

                ZXBM = ydrybz == "0" ? "0000" : tcqh;

                if (objParam.Length == 7)
                {
                    obj = YBDKDR(new object[] { });

                    if (obj[1].ToString() == "1")
                    {
                        if (xm != obj[2].ToString().Split('|')[3])
                        {
                            return new object[] { 0, 0, "医保提示：不是本人卡" };
                        }
                    }
                    else
                    {
                        return new object[] { 0, 0, "医保提示：读卡错误：" + obj[2].ToString() };
                    }
                }

                if (ydrybz == "0")//本地
                {
                    obj = new object[] { grbh, kh };
                    obj = YBYLDYFSXXCXDR(obj);

                    if (obj[1].ToString() == "-2")//错误
                    {
                        MessageBox.Show("医保提示：封锁信息查询异常", "提示");
                        return obj;
                    }
                    else if (obj[1].ToString() != "1")//封锁
                    {
                        if (DialogResult.Cancel == MessageBox.Show("医保提示：这张卡已被封锁，信息：" + obj[2].ToString() + "，是否继续挂号", "提示", MessageBoxButtons.OKCancel))
                        {
                            return obj;
                        }
                    }

                    fsxx = obj[2].ToString();
                }
                else if (yllb != "11" && yllb != "13")
                {
                    return new object[] { 0, 0, "医保提示：异地就医医疗类别只能是普通门诊或门诊特殊慢性病" };
                }

                if (string.IsNullOrWhiteSpace(jzlsh))
                {
                    return new object[] { 0, 0, "医保提示：his就诊流水号为空" };
                }
                else if (string.IsNullOrWhiteSpace(yllb))
                {
                    return new object[] { 0, 0, "医保提示：医疗类别代码为空" };
                }
                else if (string.IsNullOrWhiteSpace(ghdjsj))
                {
                    return new object[] { 0, 0, "医保提示：挂号时间为空" };
                }

                string strSql = "";
                DataSet ds = null;

                if (ydrybz == "0")
                {
                    strSql = string.Format("select a.jzlsh from ybmzzydjdr a where a.jzlsh = '{0}' and a.cxbz = 1 and a.jzbz = 'm'", jzlsh);//0秒
                    ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);

                    if (ds != null && ds.Tables[0].Rows.Count > 0)
                    {
                        return new object[] { 0, 0, "医保提示：his就诊流水号" + jzlsh + "已登记医保挂号" };
                    }
                }
                #region 获取挂号费
                strSql = string.Format(@"select  z.ybxmbh ybxmbh, z.ybxmmc ybxmmc, c.bzmem1 as dj,1 as sl,a.m1gham je,c.bzmem2 yyxmbh, c.bzname yyxmmc, a.m1invo,a.m1blam,a.m1kham,a.m1amnt
                                        from mz01h a           
                                        join bztbd c on a.m1zlfb = c.bzkeyx and c.bzcodn = 'A7' 
                                        left join ybhisdzdr z on c.bzmem2 = z.hisxmbh                
                                        where a.m1ghno = '{0}'", jzlsh);
                if(ds != null)
                    ds.Tables.Clear();
                ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);

                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    DataRow dr = ds.Tables[0].Rows[0];
                    string je = dr["je"].ToString();
                    hisylfze = Convert.ToDecimal(je);
                }

                #endregion


                strSql = string.Format(@"select a.m1zlfb zlfb, a.m1gham1 djgh, a.m1gham1 jegh, a.m1date cfrq, a.m1name hzxm, z.ybxmbh ybxmbhzc
                , z.ybxmmc ybxmmczc, a.m1gham djzc, a.m1gham jezc, c.bzmem2 yyxmbhzc, c.bzname yyxmmczc, z.sfxmzldm ybsfxmzldmzc
                , z.sflbdm ybsflbdmzc, a.m1ksno, d.b2ksnm, a.m1empn, e.b1name, a.m1invo
                from mz01h a           
                join bztbd c on a.m1zlfb = c.bzkeyx and c.bzcodn = 'A7' 
                join bz02h d on a.m1ksno = d.b2ksno
                left join bz01h e on a.m1empn = e.b1empn 
                left join ybhisdzdr z on c.bzmem2 = z.hisxmbh and z.scbz = 1
                where a.m1ghno = '{0}'", jzlsh);//2秒
                ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);

                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    DataRow dr = ds.Tables[0].Rows[0];
                    string hzxm = dr["hzxm"].ToString();

                    if (!string.Equals(hzxm, xm))
                    {
                        return new object[] { 0, 0, "医保提示：his就诊流水号" + jzlsh + "挂号登记姓名和医保卡姓名不相符" };
                    }
                    else if (dr["ybxmbhzc"] == DBNull.Value)
                    {
                        return new object[] { 0, 0, "医保提示：项目代码：[" + dr["yyxmbhzc"].ToString() + "]，名称：[" + dr["yyxmmczc"].ToString() + "]未对照或未上传，不能上传费用！" };
                    }
                    else
                    {
                        int sl = 1;
                        decimal mcyl = 1;
                        string ypjldw = "-";
                        decimal djgh = Convert.ToDecimal(dr["djgh"]);
                        decimal ghf = Convert.ToDecimal(dr["jegh"]);
                        string ybsfxmzldmzc = dr["ybsfxmzldmzc"].ToString();
                        string ybsflbdmzc = dr["ybsflbdmzc"].ToString();
                        string yyxmbhzc = dr["yyxmbhzc"].ToString();
                        string ybxmbhzc = dr["ybxmbhzc"].ToString();
                        string yyxmmczc = dr["yyxmmczc"].ToString();
                        decimal djzc = Convert.ToDecimal(dr["djzc"])-1;
                        decimal ybzl = Convert.ToDecimal(dr["jezc"])-1;
                        string ksbh = dr["m1ksno"].ToString();
                        string ksmc = dr["b2ksnm"].ToString();
                        string ysdm = string.IsNullOrWhiteSpace(dr["m1empn"].ToString()) ? "010001" : dr["m1empn"].ToString();
                        string ysxm = string.IsNullOrWhiteSpace(dr["b1name"].ToString()) ? "严东标" : dr["b1name"].ToString();
                        string fph = dr["m1invo"].ToString();

                        if (ybzl <= 0)
                        {
                            return new object[] { 0, 0, "医保提示：挂号诊查费不能为0" };
                        }

                        //begin挂号
                        DateTime dqsj = Convert.ToDateTime(Common.GetServerTime());
                        string jylsh = dqsj.ToString("yyyyMMddHHmmss") + "-" + DDYLJGBH + "-" + jzlsh + "g";
                        string rcdj = jzlsh + "|" + yllb + "|" + ghdjsj + "|" + bzbm + "|" + bzmc + "|" + ksbh + "|" + ksmc + "||||" + CliUtils.fUserName + "|" + grbh + "||||||" + xm + "|" + kh + "|||";

                        if (ydrybz != "0")
                        {
                            rcdj = jzlsh + "|" + "11" + "|" + ghdjsj + "|" + "" + "|" + "" + "|" + ksbh + "|" + ksmc + "||||" + CliUtils.fUserName + "|" + grbh + "||||||" + xm + "|" + kh + "|||";
                        }

                        StringBuilder inputData = new StringBuilder(string.Format("{0}^{1}^{2}^{3}^{4}^{5}^{6}^{7}^", "2210", DDYLJGBH, CliUtils.fLoginUser, YWZQH, jylsh, ZXBM, rcdj, "1"));
                        StringBuilder outputData = new StringBuilder(100000);
                        int i = BUSINESS_HANDLE(inputData, outputData);
                        ybjzlsh = jzlsh;

                        if (i == 0)//挂号成功
                        {
                            string bzbmmcs = "";

                            if (ydrybz != "0")
                            {
                                string yddjfh = outputData.ToString().Split('^')[2];
                                ybjzlsh = yddjfh.Split('|')[0];

                                if (yddjfh.Contains("|") && yddjfh.Length > yddjfh.IndexOf('|') + 1)
                                {
                                    bzbmmcs = yddjfh.Substring(yddjfh.IndexOf('|') + 1).TrimEnd(';');
                                }
                            }

                            isybghjsok = 1;
                            string strSql1 = string.Format(@"insert into ybmzzydjdr(jzlsh, jylsh, dwbh, sfzh, xb, mz, csrq, yldylb, rycbzt, ydrybz
                            , tcqh, nd, zyzt, zhye, bnylflj, bnzhzclj, bntczclj, bnjzjzclj, bngwybzjjlj, bnczjmmztczflj, jrtcfylj, jrjzjfylj, qfbzlj, bnzycs
                            , dwmc, nl, cbdwlx, jbjgbm, yllb, ghdjsj, bzbm, bzmc, ksbh, ksmc, ysdm, ysxm, jbr, grbh, xm, kh, ghf, ybzl, ybjzlsh, jzbz, bzbmmcs, dkydrybz, fsxx, kxx) 
                            values('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', {13}, {14}
                            , {15}, {16}, {17}, {18}, {19}, {20}, {21}, {22}, '{23}', '{24}', '{25}', '{26}', '{27}', '{28}', '{29}', '{30}', '{31}', '{32}'
                            , '{33}', '{34}', '{35}', '{36}', '{37}', '{38}', '{39}', '{40}', '{41}', '{42}', '{43}', '{44}', '{45}', '{46}', '{47}')"
                            , jzlsh, jylsh, dwbh, sfzh, xb, mz, csrq, yldylb, rycbzt, ydrybz, tcqh, nd, zyzt, zhye, bnylflj, bnzhzclj, bntczclj, bnjzjzclj
                            , bngwybzjjlj, bnczjmmztczflj, jrtcfylj, jrjzjfylj, qfbzlj, bnzycs, dwmc, nl, cbdwlx, jbjgbm, yllb, ghdjsj, bzbm, bzmc
                            , ksbh, ksmc, "", "", CliUtils.fUserName, grbh, xm, kh, ghf, ybzl, ybjzlsh, "m", bzbmmcs, dkydrybz, fsxx, objParam[5].ToString());
                            obj = new object[] { strSql1 };
                            obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);

                            if (obj[1].ToString() == "1")
                            {
                                isybghjsok = 2;

                                if (ydrybz == "0")//本地就医
                                {
                                    //begin上传处方明细
                                    dqsj = Convert.ToDateTime(Common.GetServerTime());
                                    jylsh = dqsj.ToString("yyyyMMddHHmmss") + "-" + DDYLJGBH + "-" + jzlsh + "c";
                                    string rcsf = jzlsh + "|" + ybsfxmzldmzc + "|" + ybsflbdmzc + "|" + jzlsh + dqsj.ToString("HHmmss") + "|" + ghdjsj
                                    + "|" + yyxmbhzc + "|" + ybxmbhzc + "|" + yyxmmczc + "|" + Math.Round(djzc, 4).ToString() + "|" + sl.ToString() + "|"
                                    + ybzl.ToString() + "|||" + mcyl.ToString() + "||" + ysdm + "|" + ysxm + "|||" + ksbh + "|"
                                    + ksmc + "|||" + CliUtils.fUserName + "|" + ypjldw + "||" + grbh + "|" + xm + "|" + kh + "|";
                                    inputData = new StringBuilder(string.Format("{0}^{1}^{2}^{3}^{4}^{5}^{6}^{7}^", "2310", DDYLJGBH, CliUtils.fLoginUser, YWZQH, jylsh, ZXBM, rcsf, "1"));
                                    outputData = new StringBuilder(100000);
                                    i = BUSINESS_HANDLE(inputData, outputData);

                                    if (i == 0)//处方明细上传成功
                                    {
                                        isybghjsok = 3;
                                        string[] zysfdjfhs = outputData.ToString().Split('^')[2].TrimEnd('$').Split('$');
                                        List<string> lizysfdjfh = new List<string>();

                                        foreach (string zysfdjmx in zysfdjfhs)
                                        {
                                            string[] zysfdjfh = zysfdjmx.Split('|');
                                            decimal je;
                                            bool isConvert = decimal.TryParse(zysfdjfh[0], out je);
                                            decimal zlje;
                                            isConvert = decimal.TryParse(zysfdjfh[1], out zlje);
                                            decimal zfje;
                                            isConvert = decimal.TryParse(zysfdjfh[2], out zfje);
                                            decimal cxjzfje;
                                            isConvert = decimal.TryParse(zysfdjfh[3], out cxjzfje);
                                            string sflb1 = zysfdjfh[4];
                                            string sfxmdj = zysfdjfh[5];
                                            string qezfbz = zysfdjfh[6];
                                            decimal zlbl;
                                            isConvert = decimal.TryParse(zysfdjfh[7], out zlbl);
                                            decimal xj;
                                            isConvert = decimal.TryParse(zysfdjfh[8], out xj);
                                            string bz = zysfdjfh[9];
                                            string strSql2 = string.Format(@"insert into ybcfmxscindr(jzlsh, jylsh, sfxmzl, sflb, ybcfh, cfrq, yysfxmbm
                                            , sfxmzxbm, yysfxmmc, dj, sl, je, mcyl, ysbm, ysxm, ksbh, ksmc, jbr, ypjldw, grbh, xm, kh)
                                            values ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', {9}, {10}, {11}, {12}
                                            , '{13}', '{14}', '{15}', '{16}', '{17}', '{18}', '{19}', '{20}', '{21}')"
                                            , jzlsh, jylsh, ybsfxmzldmzc, ybsflbdmzc, ghdjsj, ghdjsj, yyxmbhzc, ybxmbhzc, yyxmmczc
                                            , Math.Round(djzc, 4).ToString(), sl, ybzl, mcyl, ysdm, ysxm, ksbh, ksmc, CliUtils.fUserName, ypjldw, grbh, xm, kh);
                                            lizysfdjfh.Add(strSql2);
                                            strSql2 = string.Format(@"insert into ybcfmxscfhdr(jzlsh, jylsh, je, zlje, zfje, cxjzfje, sflb, sfxmdj, qezfbz, zlbl, xj, bz, grbh, xm, kh, cfh, ybcfh, yyxmdm, yyxmmc) 
                                            values('{0}', '{1}', {2}, {3}, {4}, {5}, '{6}', '{7}', '{8}', {9}, {10}, '{11}', '{12}', '{13}', '{14}', '{15}', '{16}', '{17}', '{18}')"
                                            , jzlsh, jylsh, je, zlje, zfje, cxjzfje, sflb1, sfxmdj, qezfbz, zlbl, xj, bz, grbh, xm, kh, jzlsh, ghdjsj, yyxmbhzc, yyxmmczc);
                                            lizysfdjfh.Add(strSql2);
                                        }

                                        obj = lizysfdjfh.ToArray();
                                        obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);

                                        if (obj[1].ToString() == "1")
                                        {
                                            isybghjsok = 4;
                                            obj = new object[] { jzlsh, fph, "1", objParam[2], bzbm, bzmc, "", yllb, hisylfze, 1 };
                                            obj = YBJSMZDR(obj);

                                            if (obj[1].ToString() != "1")
                                            {
                                                string cxghxx = "";
                                                object[] objcfmx = { jzlsh };
                                                objcfmx = YBMZCFMXCXDR(objcfmx);

                                                if (objcfmx[1].ToString() == "1")
                                                {
                                                    cxghxx = "；自动撤销医保处方明细上传成功";
                                                    object[] objmzdj = { jzlsh };
                                                    objmzdj = YBMZDJCXDR(obj);

                                                    if (objmzdj[1].ToString() == "1")
                                                    {
                                                        cxghxx += "；自动撤销医保挂号登记成功";
                                                    }
                                                    else
                                                    {
                                                        cxghxx += "；自动撤销医保挂号登记失败：" + objmzdj[2].ToString();
                                                    }
                                                }
                                                else
                                                {
                                                    cxghxx = "；自动撤销医保处方明细上传失败：" + objcfmx[2].ToString();
                                                }

                                                obj[2] += cxghxx;
                                            }

                                            return obj;
                                            //end费用结算
                                        }
                                        else
                                        {
                                            string sqlerror = obj[2].ToString();
                                            Common.InsertYBLog("", "", sqlerror);
                                            i = ChangeYBCFMXSDCXDR(new object[] { ybjzlsh, grbh, xm, kh, ZXBM });

                                            if (i == 1)
                                            {
                                                obj = new object[] { jzlsh };
                                                obj = YBMZDJCXDR(obj);
                                                string cxghxx = "";

                                                if (obj[1].ToString() == "1")
                                                {
                                                    cxghxx = "；自动撤销医保挂号登记成功";
                                                }
                                                else
                                                {
                                                    cxghxx = "；自动撤销医保挂号登记失败：" + obj[2].ToString();
                                                }

                                                return new object[] { 0, 0, "医保提示：his数据库操作失败,自动撤销处方明细成功,错误信息：" + sqlerror + cxghxx };
                                            }
                                            else
                                            {
                                                return new object[] { 0, 0, "医保提示：his数据库操作失败,自动撤销处方明细失败,错误信息：" + sqlerror };
                                            }
                                        }
                                    }
                                    else if (i == -1)
                                    {
                                        Common.InsertYBLog(jzlsh, inputData.ToString(), outputData.ToString());
                                        i = ChangeYBCFMXSDCXDR(new object[] { ybjzlsh, grbh, xm, kh, ZXBM });

                                        if (i == 1)
                                        {
                                            obj = new object[] { jzlsh };
                                            obj = YBMZDJCXDR(obj);
                                            string cxghxx = "";

                                            if (obj[1].ToString() == "1")
                                            {
                                                cxghxx = "；自动撤销医保挂号登记成功";
                                            }
                                            else
                                            {
                                                cxghxx = "；自动撤销医保挂号登记失败：" + obj[2].ToString();
                                            }

                                            return new object[] { 0, -1, "医保提示：医保系统级别错误，自动撤销医保处方明细成功" + outputData.ToString() + cxghxx };
                                        }
                                        else
                                        {
                                            return new object[] { 0, -1, "医保提示：医保系统级别错误，自动撤销医保处方明细失败" + outputData.ToString() };
                                        }
                                    }
                                    else
                                    {
                                        Common.InsertYBLog(jzlsh, inputData.ToString(), outputData.ToString());
                                        obj = new object[] { jzlsh };
                                        obj = YBMZDJCXDR(obj);
                                        string cxghxx = "";

                                        if (obj[1].ToString() == "1")
                                        {
                                            cxghxx = "；自动撤销医保挂号登记成功";
                                        }
                                        else
                                        {
                                            cxghxx = "；自动撤销医保挂号登记失败：" + obj[2].ToString();
                                        }

                                        return new object[] { 0, -2, "医保提示：医保挂号处方明细上传业务级别或未知错误，" + outputData.ToString() + cxghxx };
                                    }
                                    //end上传处方明细
                                }
                                else//异地
                                {
                                    dqsj = Convert.ToDateTime(Common.GetServerTime());
                                    jylsh = dqsj.ToString("yyyyMMddHHmmss") + "-" + DDYLJGBH + "-" + jzlsh + "y";
                                    string rc = ybjzlsh + "|" + fph + "|" + ghdjsj + "|" + CliUtils.fUserName + "|" + bzbm + "|" + bzmc + "|" + yllb + "|" + Gocent + "|" + grbh + "|" + xm + "|" + kh;

                                    if (ydrybz != "0")
                                    {
                                        rc = ybjzlsh + "|" + fph + "|" + ghdjsj + "|" + CliUtils.fUserName + "|" + "" + "|" + "" + "|" + "11" + "|" + Gocent + "|" + grbh + "|" + xm + "|" + kh;
                                    }

                                    rc += "$" + ybjzlsh + "|" + ybsfxmzldmzc + "|" + ybsflbdmzc + "|" + ghdjsj + "|" + ghdjsj
                                    + "|" + yyxmbhzc + "|" + ybxmbhzc + "|" + yyxmmczc + "|" + Math.Round(djzc, 4).ToString() + "|" + sl.ToString() + "|"
                                    + Math.Round(ybzl, 4).ToString() + "|||" + mcyl.ToString() + "||" + ysdm + "|" + ysxm + "|||" + ksbh + "|"
                                    + ksmc + "|||" + CliUtils.fUserName + "|" + ypjldw + "||";
                                    string strSqlghmx = string.Format(@"insert into ybcfmxscindr(jzlsh, jylsh, sfxmzl, sflb, ybcfh, cfrq, yysfxmbm, sfxmzxbm
                                    , yysfxmmc, dj, sl, je, mcyl, ysbm, ysxm, ksbh, ksmc, cydffbz, jbr, ypjldw, grbh, xm, kh)
                                    values ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', {9}, {10}, {11}, {12}, '{13}', '{14}'
                                    , '{15}', '{16}', '{17}', '{18}', '{19}', '{20}', '{21}', '{22}')"
                                    , jzlsh, jylsh, ybsfxmzldmzc, ybsflbdmzc, ghdjsj, ghdjsj, yyxmbhzc, ybxmbhzc, yyxmmczc, Math.Round(djzc, 4).ToString(), sl
                                    , Math.Round(ybzl, 4).ToString(), mcyl, ysdm, ysxm, ksbh, ksmc, "", CliUtils.fUserName, ypjldw, grbh, xm, kh);
                                    inputData = new StringBuilder(string.Format("{0}^{1}^{2}^{3}^{4}^{5}^{6}^{7}^", "2620", DDYLJGBH, CliUtils.fLoginUser, YWZQH, jylsh, ZXBM, rc, "1"));
                                    outputData = new StringBuilder(100000);
                                    i = BUSINESS_HANDLE(inputData, outputData);

                                    if (i == 0)
                                    {
                                        string fjsfh = outputData.ToString().Split('^')[2];
                                        string[] sfjsfh = fjsfh.Split('|');
                                        string ylfze = sfjsfh[0]; //医疗费总额
                                        string zbxje = sfjsfh[1]; //总报销金额
                                        string tcjjzf = sfjsfh[2]; //统筹基金支付
                                        string dejjzf = sfjsfh[3]; //大额基金支付
                                        string zhzf = sfjsfh[4]; //账户支付
                                        string xjzf = sfjsfh[5]; //现金支付
                                        string gwybzjjzf = sfjsfh[6]; //公务员补助基金支付
                                        string qybcylbxjjzf = sfjsfh[7]; //企业补充医疗保险基金支付
                                        string zffy = sfjsfh[8]; //自费费用
                                        string dwfdfy = sfjsfh[9]; //单位负担费用
                                        string yyfdfy = sfjsfh[10]; //医院负担费用
                                        string mzjzfy = sfjsfh[11]; //民政救助费用
                                        string cxjfy = sfjsfh[12]; //超限价费用
                                        string ylzlfy = sfjsfh[13]; //乙类自理费用
                                        string blzlfy = sfjsfh[14]; //丙类自理费用
                                        string fhjbylfy = sfjsfh[15]; //符合基本医疗费用
                                        string qfbzfy = sfjsfh[16]; //起付标准费用
                                        string zzzyzffy = sfjsfh[17]; //转诊转院自付费用
                                        string jrtcfy = sfjsfh[18]; //进入统筹费用
                                        string tcfdzffy = sfjsfh[19]; //统筹分段自付费用
                                        string ctcfdxfy = sfjsfh[20]; //超统筹封顶线费用
                                        string jrdebxfy = sfjsfh[21]; //进入大额报销费用
                                        string defdzffy = sfjsfh[22]; //大额分段自付费用
                                        string cdefdxfy = sfjsfh[23]; //超大额封顶线费用
                                        string rgqgzffy = sfjsfh[24]; //人工器官自付费用
                                        string bcjsqzhye = sfjsfh[25]; //本次结算前帐户余额
                                        string bntczflj = sfjsfh[26]; //本年统筹支付累计(不含本次)
                                        string bndezflj = sfjsfh[27]; //本年大额支付累计(不含本次)
                                        string bnczjmmztczflj1 = sfjsfh[28]; //本年城镇居民门诊统筹支付累计(不含本次)
                                        string bngwybzzflj = sfjsfh[29]; //本年公务员补助支付累计(不含本次)
                                        string bnzhzflj = sfjsfh[30]; //本年账户支付累计(不含本次) 
                                        string bnzycslj = sfjsfh[31]; //本年住院次数累计(不含本次)
                                        string zycs = sfjsfh[32]; //住院次数
                                        string jsrq1 = sfjsfh[34]; //结算时间
                                        string yllb1 = sfjsfh[35]; //医疗类别
                                        string yldylb1 = sfjsfh[36]; //医疗待遇类别
                                        string jbjgbm1 = sfjsfh[37]; //经办机构编码
                                        string ywzqh1 = sfjsfh[38]; //业务周期号
                                        string jslsh = sfjsfh[39]; //结算流水号
                                        string tsxx = sfjsfh[40]; //提示信息
                                        djh = sfjsfh[41]; //单据号
                                        string jylx = sfjsfh[42]; //交易类型
                                        string yyjylsh = sfjsfh[43]; //医院交易流水号
                                        string yxbz = sfjsfh[44]; //有效标志
                                        string grbhgl = sfjsfh[45]; //个人编号管理
                                        string yljgbm = sfjsfh[46]; //医疗机构编码
                                        string jjjmzcfwwkbxfy = sfjsfh[63]; //九江居民政策范围外可报销费用
                                        string jmdbydje = sfjsfh[58]; //居民大病一段金额
                                        string jmdbedje = sfjsfh[59]; //居民大病二段金额
                                        string jbbcfwnzfje = sfjsfh[60]; //疾病补充范围内费用支付金额
                                        string jbbcbxbczcfwwfyzfje = sfjsfh[61]; //疾病补充保险本次政策范围外费用支付金额
                                        string bnzfddjjfylj = sfjsfh[74]; //本年政府兜底基金费用累计
                                        string zfddjjfy = sfjsfh[64]; //政府兜底基金费用
                                        string strSqljs = string.Format(@"insert into ybfyjsdr(jzlsh, jylsh, djhin, bzbm, bzmc, jbr, kfsbz, ylfze, zbxje, tcjjzf
                                        , dejjzf, zhzf, xjzf, gwybzjjzf, qybcylbxjjzf, zffy, dwfdfy, yyfdfy, mzjzfy, cxjfy, ylzlfy, blzlfy, fhjbylfy, qfbzfy, zzzyzffy
                                        , jrtcfy, tcfdzffy, ctcfdxfy, jrdebxfy, defdzffy, cdefdxfy, rgqgzffy, bcjsqzhye, bntczflj, bndezflj, bnczjmmztczflj, bngwybzzflj, bnzhzflj
                                        , bnzycslj, zycs, xm, jsrq, yllb, yldylb, jbjgbm, ywzqh, jslsh, tsxx, djh, jylx, yyjylsh, yxbz, grbhgl, yljgbm, grbh, kh, ybjzlsh
                                        ,jjjmzcfwwkbxfy,jmdbydje,jmdbedje,jbbcfwnzfje,jbbcbxbczcfwwfyzfje,bnzfddjjfylj,zfddjjfy) 
                                        values('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}'
                                        , '{15}', '{16}', '{17}', '{18}', '{19}', '{20}', '{21}', '{22}', '{23}', '{24}', '{25}', '{26}', '{27}', '{28}'
                                        , '{29}', '{30}', '{31}', '{32}', '{33}', '{34}', '{35}', '{36}', '{37}', '{38}', '{39}', '{40}', '{41}', '{42}'
                                        , '{43}', '{44}', '{45}', '{46}', '{47}', '{48}', '{49}', '{50}', '{51}', '{52}', '{53}', '{54}', '{55}', '{56}'
                                        , '{57}', '{58}', '{59}', '{60}', '{61}', '{62}', '{63}')"
                                        , jzlsh, jylsh, fph, bzbm, bzmc, CliUtils.fUserName, Gocent, ylfze, zbxje, tcjjzf, dejjzf, zhzf, xjzf, gwybzjjzf, qybcylbxjjzf
                                        , zffy, dwfdfy, yyfdfy, mzjzfy, cxjfy, ylzlfy, blzlfy, fhjbylfy, qfbzfy, zzzyzffy, jrtcfy, tcfdzffy, ctcfdxfy
                                        , jrdebxfy, defdzffy, cdefdxfy, rgqgzffy, bcjsqzhye, bntczflj, bndezflj, bnczjmmztczflj1, bngwybzzflj, bnzhzflj
                                        , bnzycslj, zycs, xm, ghdjsj, yllb, yldylb, jbjgbm, ywzqh1, jslsh, tsxx, djh, jylx, yyjylsh, yxbz, grbhgl, yljgbm, grbh, kh, ybjzlsh
                                        , jjjmzcfwwkbxfy, jmdbydje, jmdbedje, jbbcfwnzfje, jbbcbxbczcfwwfyzfje, bnzfddjjfylj, zfddjjfy);
                                        string strSqlcfmxin = string.Format("update ybcfmxscindr set jsdjh = '{0}' where jzlsh = '{1}' and jsdjh is null and cxbz = 1", fph, jzlsh);
                                        obj = new object[] { strSqlghmx, strSqljs, strSqlcfmxin };
                                        obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);

                                        if (obj[1].ToString() == "1")
                                        {
                                            return new object[] { 0, 1, outputData.ToString().Split('^')[2] };
                                        }
                                        else
                                        {
                                            Common.InsertYBLog(jzlsh, "", obj[2].ToString());
                                            string sqlerror = obj[2].ToString();
                                            i = YBFYJSSDCXDR(ybjzlsh, djh, ghdjsj, grbh, xm, kh, ZXBM);

                                            if (i == 1)
                                            {
                                                obj = new object[] { jzlsh };
                                                obj = YBMZDJCXDR(obj);
                                                string cxghxx = "";

                                                if (obj[1].ToString() == "1")
                                                {
                                                    cxghxx = "；自动撤销医保挂号登记成功";
                                                }
                                                else
                                                {
                                                    cxghxx = "；自动撤销医保挂号登记失败：" + obj[2].ToString();
                                                }

                                                return new object[] { 0, 0, "医保提示：his数据库操作失败，自动撤销医保挂号结算成功,错误信息：" + sqlerror + cxghxx };
                                            }
                                            else
                                            {
                                                return new object[] { 0, 0, "医保提示：his数据库操作失败，自动撤销医保挂号结算失败,错误信息：" + sqlerror };
                                            }
                                        }
                                    }
                                    else if (i == -1)
                                    {
                                        Common.InsertYBLog(jzlsh, inputData.ToString(), outputData.ToString());
                                        i = YBFYJSSDCXDR(ybjzlsh, djh, ghdjsj, grbh, xm, kh, ZXBM);//如果异地，医保单据号还没返回，所以撤销结算可能有问题

                                        if (i == 1)
                                        {
                                            obj = new object[] { jzlsh };
                                            obj = YBMZDJCXDR(obj);
                                            string cxghxx = "";

                                            if (obj[1].ToString() == "1")
                                            {
                                                cxghxx = "；自动撤销医保挂号登记成功";
                                            }
                                            else
                                            {
                                                cxghxx = "；自动撤销医保挂号登记失败：" + obj[2].ToString();
                                            }

                                            return new object[] { 0, -1, "医保提示：医保系统级别错误，自动撤销医保挂号结算成功：" + outputData.ToString() + cxghxx };
                                        }
                                        else
                                        {
                                            return new object[] { 0, -1, "医保提示：医保系统级别错误，自动撤销医保挂号结算失败" + outputData.ToString() };
                                        }
                                    }
                                    else
                                    {
                                        Common.InsertYBLog(jzlsh, inputData.ToString(), outputData.ToString());
                                        obj = new object[] { jzlsh };
                                        obj = YBMZDJCXDR(obj);
                                        string cxghxx = "";

                                        if (obj[1].ToString() == "1")
                                        {
                                            cxghxx = "；自动撤销医保挂号登记成功";
                                        }
                                        else
                                        {
                                            cxghxx = "；自动撤销医保挂号登记失败：" + obj[2].ToString();
                                        }

                                        return new object[] { 0, -2, "医保提示：医保挂号结算业务级别或未知错误，" + outputData.ToString() + cxghxx };
                                    }
                                }
                            }
                            else
                            {
                                Common.InsertYBLog(jzlsh, "", obj[2].ToString());
                                i = YBDJSDCXDR(ybjzlsh, grbh, xm, kh, ZXBM);

                                if (i == 1)
                                {
                                    return new object[] { 0, 0, "医保提示：his数据库操作失败,自动撤销医保挂号登记成功,错误信息：" + obj[2].ToString() };
                                }
                                else
                                {
                                    return new object[] { 0, 0, "医保提示：his数据库操作失败,自动撤销医保挂号登记失败,错误信息：" + obj[2].ToString() };
                                }
                            }
                        }
                        else if (i == -1)
                        {
                            Common.InsertYBLog(jzlsh, inputData.ToString(), outputData.ToString());
                            i = YBDJSDCXDR(ybjzlsh, grbh, xm, kh, ZXBM);//如果异地，医保就诊流水号还没返回，所以撤销挂号登记可能有问题

                            if (i == 1)
                            {
                                return new object[] { 0, -1, "医保提示：医保挂号登记系统级别错误，自动撤销医保挂号登记成功，" + outputData.ToString() };
                            }
                            else
                            {
                                return new object[] { 0, -1, "医保提示：医保挂号登记系统级别错误，自动撤销医保挂号登记失败，" + outputData.ToString() };
                            }
                        }
                        else
                        {
                            Common.InsertYBLog(jzlsh, inputData.ToString(), outputData.ToString());
                            return new object[] { 0, -2, "医保提示：医保挂号登记业务级别或未知错误，" + outputData.ToString() };
                        }
                    }
                    //end挂号
                }
                else
                {
                    return new object[] { 0, 0, "医保提示：无his挂号费用明细" };
                }
                //}
                //return (new object[] { 0, 0, "门诊登记失败" });

            }
            catch (Exception error)
            {
                Common.InsertYBLog(jzlsh, "", error.ToString());

                if (isybghjsok == 1)//医保挂号成功
                {
                    int i = YBDJSDCXDR(ybjzlsh, grbh, xm, kh, ZXBM);

                    if (i == 1)
                    {
                        return new object[] { 0, 2, "医保提示：非医保异常，医保挂号登记自动撤销成功，" + error.ToString() };
                    }
                    else
                    {
                        return new object[] { 0, 2, "医保提示：非医保异常，医保挂号登记自动撤销失败，" + error.ToString() };
                    }
                }
                else if (isybghjsok == 2)//医保挂号数据库操作成功
                {
                    object[] obj = { jzlsh };
                    obj = YBMZDJCXDR(obj);

                    if (obj[1].ToString() == "1")
                    {
                        return new object[] { 0, 2, "医保提示：医保异常，医保挂号登记自动撤销成功，" + error.ToString() };
                    }
                    else
                    {
                        return new object[] { 0, 2, "医保提示：医保异常，医保挂号登记自动撤销失败，" + error.ToString() };
                    }
                }
                else if (isybghjsok == 3)//医保处方明细上传成功
                {
                    int i = ChangeYBCFMXSDCXDR(new object[] { ybjzlsh, grbh, xm, kh, ZXBM });

                    if (i == 1)
                    {
                        object[] obj = { jzlsh };
                        obj = YBMZDJCXDR(obj);
                        string cxghxx = "";

                        if (obj[1].ToString() == "1")
                        {
                            cxghxx = "；自动撤销医保挂号登记成功";
                        }
                        else
                        {
                            cxghxx = "；自动撤销医保挂号登记失败：" + obj[2].ToString();
                        }

                        return new object[] { 0, 2, "医保提示：非医保异常，自动撤销医保处方明细上传成功：" + cxghxx + "，" + error.ToString() };
                    }
                    else
                    {
                        return new object[] { 0, 2, "医保提示：非医保异常，自动撤销医保处方明细上传失败，" + error.ToString() };
                    }
                }
                else if (isybghjsok == 4)//医保处方明细上传数据库操作成功
                {
                    object[] obj = { jzlsh };
                    obj = YBMZCFMXCXDR(obj);

                    if (obj[1].ToString() == "1")
                    {
                        obj = new object[] { jzlsh };
                        obj = YBMZDJCXDR(obj);
                        string cxghxx = "";

                        if (obj[1].ToString() == "1")
                        {
                            cxghxx = "；自动撤销医保挂号登记成功";
                        }
                        else
                        {
                            cxghxx = "；自动撤销医保挂号登记失败：" + obj[2].ToString();
                        }

                        return new object[] { 0, 2, "医保提示：医保异常，自动撤销医保处方明细上传成功：" + cxghxx + "，" + error.ToString() };
                    }
                    else
                    {
                        return new object[] { 0, 2, "医保提示：医保异常，自动撤销医保处方明细上传失败，" + error.ToString() };
                    }
                }

                return new object[] { 0, 2, "Error:" + error.ToString() };
            }
        }
        #endregion 门诊登记收费

        #region 门诊登记收费撤销
        /// <summary>
        /// 门诊登记收费撤销
        /// </summary>
        /// <param>his就诊流水号,读卡标志(不传表示读卡，传了则不读卡)</param>
        /// <returns>1:成功，0:不成功，2:报错</returns>
        public static object[] YBMZDJSFCXDR(object[] objParam)
        {
            string czygh = CliUtils.fLoginUser;
            string ywzqh = YWZQH;
            string jbr = CliUtils.fUserName;
            string jzlsh = objParam[0].ToString();
            DateTime dqsj = Convert.ToDateTime(Common.GetServerTime());
            string jylsh = dqsj.ToString("yyyyMMddHHmmss") + "-" + DDYLJGBH + "-" + jzlsh;

            try
            {
                string strSql = string.Format("select top 1 * from ybfyjsdr a where a.jzlsh = '{0}' and a.cxbz = 1 order by a.sysdate desc", jzlsh);//0秒
                DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);

                if (ds == null || ds.Tables[0].Rows.Count == 0)
                {
                    return YBMZDJCXDR(new object[] { jzlsh });
                }

                DataRow dr = ds.Tables[0].Rows[0];
                string ybjzlsh = dr["ybjzlsh"].ToString();
                strSql = string.Format("select top 1 a.grbh, a.xm, a.kh, a.ybjzlsh, a.ydrybz, a.tcqh from ybmzzydjdr a where a.jzlsh = '{0}' and a.cxbz = 1 and a.jzbz = 'm'", jzlsh);//0秒

                if (!string.IsNullOrWhiteSpace(ybjzlsh))
                {
                    strSql += string.Format(" and a.ybjzlsh = '{0}'", ybjzlsh);
                }

                strSql += " order by a.sysdate desc";
                ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);

                if (ds == null || ds.Tables[0].Rows.Count == 0)
                {
                    return new object[] { 0, 0, "医保提示：医保未办理挂号登记" };
                }

                dr = ds.Tables[0].Rows[0];
                string grbh = dr["grbh"].ToString();
                string xm = dr["xm"].ToString();
                string kh = dr["kh"].ToString();
                ybjzlsh = dr["ybjzlsh"].ToString();
                string ydrybz = dr["ydrybz"].ToString();
                ZXBM = ydrybz == "0" ? "0000" : dr["tcqh"].ToString();
                strSql = string.Format("select a.m1invo from mz01h a where a.m1ghno = '{0}'", jzlsh);//0秒
                ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);

                if (ds == null || ds.Tables[0].Rows.Count == 0)
                {
                    return new object[] { 0, 0, "医保提示：his未办理挂号登记" };
                }

                dr = ds.Tables[0].Rows[0];
                string djh = dr["m1invo"].ToString();
                object[] obj = null;

                if (objParam.Length == 1)
                {
                    obj = new object[] { jzlsh, djh };
                }
                else
                {
                    obj = new object[] { jzlsh, djh, objParam[1] };
                }

                obj = YBFYJSCXDR(obj);

                if (obj[1].ToString() != "1")
                {
                    return obj;
                }

                string rc = ybjzlsh + "|" + jbr + "|" + grbh + "|" + xm + "|" + kh + "|";
                StringBuilder inputData = new StringBuilder(string.Format("{0}^{1}^{2}^{3}^{4}^{5}^{6}^{7}^", "2240", DDYLJGBH, czygh, ywzqh, jylsh, ZXBM, rc, "1"));
                StringBuilder outputData = new StringBuilder(100000);
                int i = BUSINESS_HANDLE(inputData, outputData);

                if (i == 0)
                {
                    string strSql1 = string.Format(@"insert into ybmzzydjdr(jzlsh, jylsh, dwbh, sfzh, xb, mz, csrq, yldylb, rycbzt, ydrybz, tcqh
                    , nd, zyzt, zhye, bnylflj, bnzhzclj, bntczclj, bnjzjzclj, bngwybzjjlj, bnczjmmztczflj, jrtcfylj, jrjzjfylj, qfbzlj, bnzycs
                    , dwmc, nl, cbdwlx, jbjgbm, elmmxezc, elmmxesy, yldyxz, gsdyxz, sydyxz, yllb, ghdjsj, bzbm, bzmc, ksbh, ksmc, cwh, ysdm
                    , ysxm, jbr, grbh, bq, zzyybh, zzyymc, bz, tsfybz, xm, kh, ghf, ybzl, mmbzbm1, mmbzmc1, mmbzbm2, mmbzmc2, mmbzbm3
                    , mmbzmc3, mmbzbm4, mmbzmc4, ybjzlsh, jzbz, cxbz) 
                    select jzlsh, jylsh, dwbh, sfzh, xb, mz, csrq, yldylb, rycbzt, ydrybz, tcqh, nd, zyzt, zhye, bnylflj, bnzhzclj, bntczclj
                    , bnjzjzclj, bngwybzjjlj, bnczjmmztczflj, jrtcfylj, jrjzjfylj, qfbzlj, bnzycs, dwmc, nl, cbdwlx, jbjgbm, elmmxezc, elmmxesy
                    , yldyxz, gsdyxz, sydyxz, yllb, ghdjsj, bzbm, bzmc, ksbh, ksmc, cwh, ysdm, ysxm, '{1}', grbh, bq, zzyybh, zzyymc, bz, tsfybz
                    , xm, kh, ghf, ybzl, mmbzbm1, mmbzmc1, mmbzbm2, mmbzmc2, mmbzbm3, mmbzmc3, mmbzbm4, mmbzmc4, ybjzlsh, jzbz, 0 
                    from ybmzzydjdr where jzlsh = '{0}' and cxbz = 1 and jzbz = 'm' and ybjzlsh = '{2}'", jzlsh, jbr, ybjzlsh);//0秒
                    string strSql2 = string.Format("update ybmzzydjdr set cxbz = 2 where jzlsh = '{0}' and cxbz = 1 and jzbz = 'm' and ybjzlsh = '{1}'", jzlsh, ybjzlsh);//0秒
                    obj = new object[] { strSql1, strSql2 };
                    obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);

                    if (obj[1].ToString() == "1")
                    {
                        return new object[] { 0, 1, outputData };
                    }
                    else
                    {
                        Common.InsertYBLog(jzlsh, "", obj[2].ToString());
                        return new object[] { 0, 0, "医保提示：his数据库操作失败：" + obj[2].ToString() };
                    }
                }
                else if (i == -1)
                {
                    Common.InsertYBLog(jzlsh, inputData.ToString(), outputData.ToString());
                    string strSql1 = string.Format(@"insert into ybmzzydjdr(jzlsh, jylsh, dwbh, sfzh, xb, mz, csrq, yldylb, rycbzt, ydrybz, tcqh
                    , nd, zyzt, zhye, bnylflj, bnzhzclj, bntczclj, bnjzjzclj, bngwybzjjlj, bnczjmmztczflj, jrtcfylj, jrjzjfylj, qfbzlj, bnzycs
                    , dwmc, nl, cbdwlx, jbjgbm, elmmxezc, elmmxesy, yldyxz, gsdyxz, sydyxz, yllb, ghdjsj, bzbm, bzmc, ksbh, ksmc, cwh, ysdm
                    , ysxm, jbr, grbh, bq, zzyybh, zzyymc, bz, tsfybz, xm, kh, ghf, ybzl, mmbzbm1, mmbzmc1, mmbzbm2, mmbzmc2, mmbzbm3
                    , mmbzmc3, mmbzbm4, mmbzmc4, ybjzlsh, jzbz, cxbz) 
                    select jzlsh, jylsh, dwbh, sfzh, xb, mz, csrq, yldylb, rycbzt, ydrybz, tcqh, nd, zyzt, zhye, bnylflj, bnzhzclj, bntczclj
                    , bnjzjzclj, bngwybzjjlj, bnczjmmztczflj, jrtcfylj, jrjzjfylj, qfbzlj, bnzycs, dwmc, nl, cbdwlx, jbjgbm, elmmxezc, elmmxesy
                    , yldyxz, gsdyxz, sydyxz, yllb, ghdjsj, bzbm, bzmc, ksbh, ksmc, cwh, ysdm, ysxm, '{1}', grbh, bq, zzyybh, zzyymc, bz, tsfybz
                    , xm, kh, ghf, ybzl, mmbzbm1, mmbzmc1, mmbzbm2, mmbzmc2, mmbzbm3, mmbzmc3, mmbzbm4, mmbzmc4, ybjzlsh, jzbz, 0 
                    from ybmzzydjdr where jzlsh = '{0}' and cxbz = 1 and jzbz = 'm' and ybjzlsh = '{2}'", jzlsh, jbr, ybjzlsh);//0秒
                    string strSql2 = string.Format("update ybmzzydjdr set cxbz = 2 where jzlsh = '{0}' and cxbz = 1 and jzbz = 'm' and ybjzlsh = '{1}'", jzlsh, ybjzlsh);//0秒
                    obj = new object[] { strSql1, strSql2 };
                    obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);

                    if (obj[1].ToString() == "1")
                    {
                        return new object[] { 0, -1, "医保提示：医保门诊登记撤销系统级别错误，自动删除医保门诊登记成功，" + outputData.ToString() };
                    }
                    else
                    {
                        Common.InsertYBLog(jzlsh, "", obj[2].ToString());
                        return new object[] { 0, -1, "医保提示：医保门诊登记撤销系统级别错误，自动删除医保门诊登记失败，" + obj[2].ToString() + outputData.ToString() };
                    }
                }
                else
                {
                    Common.InsertYBLog(jzlsh, inputData.ToString(), outputData.ToString());
                    return new object[] { 0, -2, "医保提示：医保门诊登记撤销业务级别或未知错误，" + outputData.ToString() };
                }
            }
            catch (Exception error)
            {
                Common.InsertYBLog(jzlsh, "", error.ToString());
                return new object[] { 0, 2, "Error:" + error.ToString() };
            }
        }
        #endregion 门诊登记收费撤销

        #region 门诊预结算
        /// <summary>
        /// 门诊预结算
        /// </summary>
        /// <param>his就诊流水号,账户使用标志（0或1）,结算时间(格式：yyyy-MM-dd HH:mm:ss)，病种编码(非慢性病传空字符串)，病种名称(非慢性病传空字符串)，处方号集合(格式：'01','02')，医疗类别代码</param>
        /// <returns>医疗费总额|总报销金额|统筹基金支付|大额基金支付|账户支付|现金支付|公务员补助基金支付|企业补充医疗保险基金支付|自费费用|单位负担费用|医院负担费用|民政救助费用|超限价费用|乙类自理费用|丙类自理费用|符合基本医疗费用|起付标准费用|转诊转院自付费用|进入统筹费用|统筹分段自付费用|超统筹封顶线费用|进入大额报销费用|大额分段自付费用|超大额封顶线费用|人工器官自付费用|本次结算前帐户余额|本年统筹支付累计(不含本次)|本年大额支付累计(不含本次)|本年城镇居民门诊统筹支付累计(不含本次)|本年公务员补助支付累计(不含本次)|本年账户支付累计(不含本次)|本年住院次数累计(不含本次)|住院次数|姓名|结算时间|医疗类别|医疗待遇类别|经办机构编码|业务周期号|结算流水号|提示信息|单据号|交易类型|医院交易流水号|有效标志|个人编号|医疗机构编码|二次补偿金额|门慢起付累计|接收方交易流水号|个人编号|单病种补差|财政支出金额|二类门慢限额支出|二类门慢限额剩余|居民个人自付二次补偿金额|</returns>
        public static object[] YBYJSMZDR(object[] objParam)
        {
            string jzlsh = objParam[0].ToString();
            string zhsybz = objParam[1].ToString();
            string jssj = objParam[2].ToString();
            string bzbm = objParam[3].ToString();
            string bzmc = objParam[4].ToString();
            string cfhs = objParam[5].ToString();
            string yllb = objParam[6].ToString();

            if (string.IsNullOrWhiteSpace(jzlsh))
            {
                return new object[] { 0, 0, "医保提示：his就诊流水号为空" };
            }
            else if (string.IsNullOrWhiteSpace(zhsybz))
            {
                return new object[] { 0, 0, "医保提示：账户使用标志为空" };
            }
            else if (string.IsNullOrWhiteSpace(jssj))
            {
                return new object[] { 0, 0, "医保提示：结算时间为空" };
            }
            else if (string.IsNullOrWhiteSpace(cfhs))
            {
                return new object[] { 0, 0, "医保提示：his处方号集合为空" };
            }
            else if (string.IsNullOrWhiteSpace(yllb))
            {
                return new object[] { 0, 0, "医保提示：医保医疗类别代码为空" };
            }

            string strSql = string.Format("select ydrybz from ybmzzydjdr a where a.jzlsh = '{0}' and a.cxbz = 1 and a.jzbz = 'm'", jzlsh);//0秒
            DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);

            if (ds == null || ds.Tables[0].Rows.Count == 0)
            {
                return new object[] { 0, 0, "医保提示：无医保挂号登记记录" };
            }

            DataTable dt = ds.Tables[0];
            DataRow dr = dt.Rows[0];
            string ydrybz = dr["ydrybz"].ToString();

            if (ydrybz == "0")
            {
                object[] obj = YBMZCFMXSBDR(new object[] { jzlsh, cfhs, jssj });

                if (obj[1].ToString() != "1")
                {
                    return obj;
                }

                return YBYJSMZBD(new object[] { jzlsh, zhsybz, jssj, bzbm, bzmc, yllb });
            }
            else
            {
                return YBYJSMZYD(new object[] { jzlsh, jssj, bzbm, bzmc, cfhs, yllb });
            }
        }
        #endregion

        #region 门诊预结算本地
        /// <summary>
        /// 门诊预结算本地
        /// </summary>
        /// <param>his就诊流水号,账户使用标志（0或1）,结算时间(格式：yyyy-MM-dd HH:mm:ss)，病种编码(非慢性病传空字符串)，病种名称(非慢性病传空字符串)，医疗类别代码</param>
        /// <returns>医疗费总额|总报销金额|统筹基金支付|大额基金支付|账户支付|现金支付|公务员补助基金支付|企业补充医疗保险基金支付|自费费用|单位负担费用|医院负担费用|民政救助费用|超限价费用|乙类自理费用|丙类自理费用|符合基本医疗费用|起付标准费用|转诊转院自付费用|进入统筹费用|统筹分段自付费用|超统筹封顶线费用|进入大额报销费用|大额分段自付费用|超大额封顶线费用|人工器官自付费用|本次结算前帐户余额|本年统筹支付累计(不含本次)|本年大额支付累计(不含本次)|本年城镇居民门诊统筹支付累计(不含本次)|本年公务员补助支付累计(不含本次)|本年账户支付累计(不含本次)|本年住院次数累计(不含本次)|住院次数|姓名|结算时间|医疗类别|医疗待遇类别|经办机构编码|业务周期号|结算流水号|提示信息|单据号|交易类型|医院交易流水号|有效标志|个人编号|医疗机构编码|二次补偿金额|门慢起付累计|接收方交易流水号|个人编号|单病种补差|财政支出金额|二类门慢限额支出|二类门慢限额剩余|居民个人自付二次补偿金额|</returns>
        public static object[] YBYJSMZBD(object[] objParam)
        {
            try
            {
                string czygh = CliUtils.fLoginUser;
                string ywzqh = YWZQH;
                string jbr = CliUtils.fUserName;
                string jzlsh = objParam[0].ToString();
                string zhsybz = objParam[1].ToString();
                string ztjsbz = "0";
                string bzbm = objParam[3].ToString();
                string bzmc = objParam[4].ToString();
                string yllb = objParam[5].ToString();
                string cyyy = "9";
                ZXBM = "0000";

                if (string.IsNullOrWhiteSpace(jzlsh))
                {
                    return new object[] { 0, 0, "医保提示：his就诊流水号为空" };
                }
                else if (string.IsNullOrWhiteSpace(zhsybz))
                {
                    return new object[] { 0, 0, "医保提示：账户使用标志为空" };
                }
                else if (string.IsNullOrWhiteSpace(objParam[2].ToString()))
                {
                    return new object[] { 0, 0, "医保提示：结算时间为空" };
                }
                else if (string.IsNullOrWhiteSpace(yllb))
                {
                    return new object[] { 0, 0, "医保提示：医疗类别代码为空" };
                }

                string dqrq = Convert.ToDateTime(objParam[2]).ToString("yyyyMMddHHmmss");
                DateTime dqsj = Convert.ToDateTime(Common.GetServerTime());
                string jylsh = dqsj.ToString("yyyyMMddHHmmss") + "-" + DDYLJGBH + "-" + jzlsh;
                string strSql = string.Format("select distinct jylsh from ybcfmxscfhdr a where a.jzlsh = '{0}' and a.cxbz = 1 and a.jsdjh is null", jzlsh);//0秒
                DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);

                if (ds == null || ds.Tables[0].Rows.Count == 0)
                {
                    return new object[] { 0, 0, "医保提示：his就诊流水号" + jzlsh + "无费用明细上传记录" };
                }

                strSql = string.Format("select ybjzlsh, grbh, xm, kh, ydrybz from ybmzzydjdr a where a.jzlsh = '{0}' and a.cxbz = 1 and a.jzbz = 'm'", jzlsh);//0秒
                ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);

                if (ds == null || ds.Tables[0].Rows.Count == 0)
                {
                    return new object[] { 0, 0, "医保提示：his就诊流水号" + jzlsh + "无挂号登记记录" };
                }

                DataRow dr = ds.Tables[0].Rows[0];
                string ybjzlsh = dr["ybjzlsh"].ToString();
                string grbh = dr["grbh"].ToString();
                string xm = dr["xm"].ToString();
                string kh = dr["kh"].ToString();
                string ydrybz = dr["ydrybz"].ToString();
                string djhin = "0000";

                object[] obj = { grbh, kh };
                if (ydrybz == "0")
                {
                    obj = YBYLDYFSXXCXDR(obj);

                    if (obj[1].ToString() == "-2")//错误
                    {
                        MessageBox.Show("医保提示：封锁信息查询异常", "提示");
                        return obj;
                    }
                    else if (obj[1].ToString() != "1")//封锁
                    {
                        if (DialogResult.Cancel == MessageBox.Show("医保提示：这张卡已被封锁，信息：" + obj[2].ToString() + ",是否继续预结算", "提示", MessageBoxButtons.OKCancel))
                        {
                            return obj;
                        }
                    }
                }
                

                string rc = ybjzlsh + "|" + djhin + "|" + yllb + "|" + dqrq + "|" + dqrq + "|" + cyyy + "|"
                + bzbm + "|" + bzmc + "|" + zhsybz + "|" + ztjsbz + "|" + jbr + "|"
                + Gocent + "||||||||||" + grbh + "|" + xm + "|" + kh + "|0|";
                StringBuilder inputData = new StringBuilder(string.Format("{0}^{1}^{2}^{3}^{4}^{5}^{6}^{7}^", "2420", DDYLJGBH, czygh, ywzqh, jylsh, ZXBM, rc, "1"));
                StringBuilder outputData = new StringBuilder(100000);
                int i = BUSINESS_HANDLE(inputData, outputData);

                if (i == 0)
                {
                    string fjsfh = outputData.ToString().Split('^')[2];
                    string[] sfjsfh = fjsfh.Split('|');
                    string ylfze = sfjsfh[0];//医疗费总额
                    string zbxje = sfjsfh[1];//总报销金额
                    string tcjjzf = sfjsfh[2];//统筹基金支付
                    string dejjzf = sfjsfh[3];//大额基金支付
                    string zhzf = sfjsfh[4];//账户支付
                    string xjzf = sfjsfh[5];//现金支付
                    string gwybzjjzf = sfjsfh[6];//公务员补助基金支付
                    string qybcylbxjjzf = sfjsfh[7];//企业补充医疗保险基金支付
                    string zffy = sfjsfh[8];//自费费用
                    string dwfdfy = sfjsfh[9];//单位负担费用
                    string yyfdfy = sfjsfh[10];//医院负担费用
                    string mzjzfy = sfjsfh[11];//民政救助费用
                    string cxjfy = sfjsfh[12];//超限价费用
                    string ylzlfy = sfjsfh[13];//乙类自理费用
                    string blzlfy = sfjsfh[14];//丙类自理费用
                    string fhjbylfy = sfjsfh[15];//符合基本医疗费用
                    string qfbzfy = sfjsfh[16];//起付标准费用
                    string zzzyzffy = sfjsfh[17];//转诊转院自付费用
                    string jrtcfy = sfjsfh[18];//进入统筹费用
                    string tcfdzffy = sfjsfh[19];//统筹分段自付费用
                    string ctcfdxfy = sfjsfh[20];//超统筹封顶线费用
                    string jrdebxfy = sfjsfh[21];//进入大额报销费用
                    string defdzffy = sfjsfh[22];//大额分段自付费用
                    string cdefdxfy = sfjsfh[23];//超大额封顶线费用
                    string rgqgzffy = sfjsfh[24];//人工器官自付费用
                    string bcjsqzhye = sfjsfh[25];//本次结算前帐户余额
                    string bntczflj = sfjsfh[26];//本年统筹支付累计(不含本次)
                    string bndezflj = sfjsfh[27];//本年大额支付累计(不含本次)
                    string bnczjmmztczflj = sfjsfh[28];//本年城镇居民门诊统筹支付累计(不含本次)
                    string bngwybzzflj = sfjsfh[29];//本年公务员补助支付累计(不含本次)
                    string bnzhzflj = sfjsfh[30];//本年账户支付累计(不含本次) 
                    string bnzycslj = sfjsfh[31];//本年住院次数累计(不含本次)
                    string zycs = sfjsfh[32];//住院次数
                    string jsrq = sfjsfh[34];//结算时间
                    string yllbdm = sfjsfh[35];//医疗类别
                    string yldylb = sfjsfh[36];//医疗待遇类别
                    string jbjgbm = sfjsfh[37];//经办机构编码
                    string ywzqh1 = sfjsfh[38];//业务周期号
                    string jslsh = sfjsfh[39];//结算流水号
                    string tsxx = sfjsfh[40];//提示信息
                    string djh = sfjsfh[41];//单据号
                    string jylx = sfjsfh[42];//交易类型
                    string yyjylsh = sfjsfh[43];//医院交易流水号
                    string yxbz = sfjsfh[44];//有效标志
                    string grbhgl = sfjsfh[45];//个人编号管理
                    string yljgbm = sfjsfh[46];//医疗机构编码
                    string ecbcje = sfjsfh[47];//二次补偿金额
                    string mmqflj = sfjsfh[48];//门慢起付累计
                    string jsfjylsh = sfjsfh[49];//接收方交易流水号
                    string dbzbc = sfjsfh[51];//单病种补差
                    string czzcje = sfjsfh[52];//财政支出金额
                    string elmmxezc = sfjsfh[53];//二类门慢限额支出
                    string elmmxesy = sfjsfh[54];//二类门慢限额剩余
                    string jmgrzfecbcje = sfjsfh[55];//居民个人自付二次补偿金额
                    string tjje = sfjsfh[56];//体检金额
                    string syjjzf = sfjsfh[57];//生育基金支付
                    string jjjmdbydje = sfjsfh[58];//九江居民大病一段金额
                    string jjjmdbedje = sfjsfh[59];//九江居民大病二段金额
                    string jjjmjbbcfwnje = sfjsfh[60];//九江居民疾病补充范围内金额
                    string jjjmjbbcfwwje = sfjsfh[61];//九江居民疾病补充范围外金额
                    string strSql1 = string.Format(@"insert into ybfyyjsdr(jzlsh, jylsh, djhin, cyrq, cyyy, bzbm, bzmc, zhsybz, ztjsbz, jbr, kfsbz
                    , cyzdbm1, cyzdmc1, cyzdbm2, cyzdmc2, cyzdbm3, cyzdmc3, cyzdbm4, cyzdmc4, zlff, tes, ylfze, zbxje, tcjjzf, dejjzf, zhzf, xjzf
                    , gwybzjjzf, qybcylbxjjzf, zffy, dwfdfy, yyfdfy, mzjzfy, cxjfy, ylzlfy, blzlfy, fhjbylfy, qfbzfy, zzzyzffy, jrtcfy, tcfdzffy
                    , ctcfdxfy, jrdebxfy, defdzffy, cdefdxfy, rgqgzffy, bcjsqzhye, bntczflj, bndezflj, bnczjmmztczflj, bngwybzzflj, bnzhzflj
                    , bnzycslj, zycs, xm, jsrq, yllb, yldylb, jbjgbm, ywzqh, jslsh, tsxx, djh, jylx, yyjylsh, yxbz, grbhgl, yljgbm, ecbcje
                    , mmqflj, jsfjylsh, grbh, dbzbc, czzcje, elmmxezc, elmmxesy, jmgrzfecbcje, tjje, syjjzf, kh, jjjmdbydje, jjjmdbedje, jjjmjbbcfwnje, jjjmjbbcfwwje) 
                    values('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}'
                    , '{15}', '{16}', '{17}', '{18}', '{19}', '{20}', '{21}', '{22}', '{23}', '{24}', '{25}', '{26}', '{27}', '{28}'
                    , '{29}', '{30}', '{31}', '{32}', '{33}', '{34}', '{35}', '{36}', '{37}', '{38}', '{39}', '{40}', '{41}', '{42}'
                    , '{43}', '{44}', '{45}', '{46}', '{47}', '{48}', '{49}', '{50}', '{51}', '{52}', '{53}', '{54}', '{55}', '{56}', '{57}'
                    , '{58}', '{59}', '{60}', '{61}', '{62}', '{63}', '{64}', '{65}', '{66}', '{67}', '{68}', '{69}', '{70}', '{71}'
                    , '{72}', '{73}', '{74}', '{75}', '{76}', '{77}', '{78}', '{79}', '{80}', '{81}', '{82}', '{83}')"
                    , jzlsh, jylsh, djhin, dqrq, cyyy, bzbm, bzmc, zhsybz, ztjsbz, jbr, Gocent, "", "", "", "", "", "", "", "", "", ""
                    , ylfze, zbxje, tcjjzf, dejjzf, zhzf, xjzf, gwybzjjzf, qybcylbxjjzf, zffy, dwfdfy, yyfdfy, mzjzfy, cxjfy, ylzlfy
                    , blzlfy, fhjbylfy, qfbzfy, zzzyzffy, jrtcfy, tcfdzffy, ctcfdxfy, jrdebxfy, defdzffy, cdefdxfy, rgqgzffy, bcjsqzhye
                    , bntczflj, bndezflj, bnczjmmztczflj, bngwybzzflj, bnzhzflj, bnzycslj, zycs, xm, dqrq, yllb, yldylb, jbjgbm, ywzqh
                    , jslsh, tsxx, djh, jylx, yyjylsh, yxbz, grbhgl, yljgbm, ecbcje, mmqflj, jsfjylsh, grbh, dbzbc, czzcje, elmmxezc
                    , elmmxesy, jmgrzfecbcje, tjje, syjjzf, kh, jjjmdbydje, jjjmdbedje, jjjmjbbcfwnje, jjjmjbbcfwwje);
                    obj = new object[] { strSql1 };
                    obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);

                    if (obj[1].ToString() == "1")
                    {
                        return new object[] { 0, 1, outputData.ToString().Split('^')[2] };
                    }
                    else
                    {
                        Common.WriteYBLog(obj[2].ToString());
                        return new object[] { 0, 0, "医保提示：his数据库操作失败,错误信息：" + obj[2].ToString() };
                    }
                }
                else
                {
                    return new object[] { 0, 0, outputData };
                }
            }
            catch (Exception error)
            {
                Common.WriteYBLog(error.ToString());
                return new object[] { 0, 2, error.ToString() };
            }
        }
        #endregion 门诊预结算本地

        #region 门诊预结算异地
        /// <summary>
        /// 门诊预结算异地
        /// </summary>
        /// <param>his就诊流水号，结算时间(格式：yyyy-MM-dd HH:mm:ss)，病种编码(非慢性病传空字符串)，病种名称(非慢性病传空字符串),处方号集合(格式：'01','02')，医疗类别代码</param>
        /// <returns>医疗费总额|总报销金额|统筹基金支付|大额基金支付|账户支付|现金支付|公务员补助基金支付|企业补充医疗保险基金支付|自费费用|单位负担费用|医院负担费用|民政救助费用|超限价费用|乙类自理费用|丙类自理费用|符合基本医疗费用|起付标准费用|转诊转院自付费用|进入统筹费用|统筹分段自付费用|超统筹封顶线费用|进入大额报销费用|大额分段自付费用|超大额封顶线费用|人工器官自付费用|本次结算前帐户余额|本年统筹支付累计(不含本次)|本年大额支付累计(不含本次)|本年城镇居民门诊统筹支付累计(不含本次)|本年公务员补助支付累计(不含本次)|本年账户支付累计(不含本次)|本年住院次数累计(不含本次)|住院次数|姓名|结算时间|医疗类别|医疗待遇类别|经办机构编码|业务周期号|结算流水号|提示信息|单据号|交易类型|医院交易流水号|有效标志|个人编号|医疗机构编码|二次补偿金额|门慢起付累计|接收方交易流水号|个人编号|单病种补差|财政支出金额|二类门慢限额支出|二类门慢限额剩余|居民个人自付二次补偿金额|</returns>
        public static object[] YBYJSMZYD(object[] objParam)
        {
            try
            {
                string czygh = CliUtils.fLoginUser;
                string ywzqh = YWZQH;
                string jbr = CliUtils.fUserName;
                string jzlsh = objParam[0].ToString();
                string bzbm = objParam[2].ToString();
                string bzmc = objParam[3].ToString();
                string cfhs = objParam[4].ToString();
                string yllb = objParam[5].ToString();

                if (string.IsNullOrWhiteSpace(jzlsh))
                {
                    return new object[] { 0, 0, "医保提示：his就诊流水号为空" };
                }
                else if (string.IsNullOrWhiteSpace(objParam[1].ToString()))
                {
                    return new object[] { 0, 0, "医保提示：结算时间为空" };
                }
                else if (string.IsNullOrWhiteSpace(cfhs))
                {
                    return new object[] { 0, 0, "医保提示：处方号集合为空" };
                }
                else if (string.IsNullOrWhiteSpace(yllb))
                {
                    return new object[] { 0, 0, "医保提示：医疗类别代码为空" };
                }

                string strSql = string.Format("select top 1 a.grbh, a.xm, a.kh, a.tcqh, a.kxx from ybmzzydjdr a where a.jzlsh = '{0}' and a.cxbz = 1 and a.jzbz = 'm'", jzlsh);//0秒
                DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);

                if (ds == null || ds.Tables[0].Rows.Count == 0)
                {
                    return new object[] { 0, 0, "医保提示：his就诊流水号" + jzlsh + "无挂号登记记录" };
                }

                DataRow dr1 = ds.Tables[0].Rows[0];
                //string ybjzlsh = dr1["ybjzlsh"].ToString();

                //if (yllb != "11")
                //{
                //    ybjzlsh = dr1["ybjzlshydmxb"].ToString();
                //}

                string grbh = dr1["grbh"].ToString();
                string xm = dr1["xm"].ToString();
                string kh = dr1["kh"].ToString();
                ZXBM = dr1["tcqh"].ToString();
                string kxx = dr1["kxx"].ToString();
                object[] obj = { jzlsh, yllb, bzbm, bzmc, kxx, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") };
                obj = YBMZDJDR(obj);

                if (obj[1].ToString() != "1")
                {
                    return new object[] { 0, 0, "医保提示：his就诊流水号" + jzlsh + "登记医保挂号失败；" + obj[2].ToString() };
                }

                string ybjzlsh = obj[2].ToString();
                string cfsj = Convert.ToDateTime(objParam[1]).ToString("yyyyMMddHHmmss");
                DateTime dqsj = Convert.ToDateTime(Common.GetServerTime());
                string jylsh = dqsj.ToString("yyyyMMddHHmmss") + "-" + DDYLJGBH + "-" + jzlsh;
                string djhin = "";
                //object[] obj = null;
                string rc = ybjzlsh + "|" + djhin + "|" + cfsj + "|" + jbr + "|" + bzbm + "|" + bzmc + "|" + yllb + "|" + Gocent + "|" + grbh + "|" + xm + "|" + kh;

                if (ZXBM != "0000")
                {
                    strSql = string.Format(@"select case y.ybxmbh when '39199999990001000001' then 'Z-00000-00000' when '61048000000500000000' then '61048000000400000000' else y.ybxmbh end ybxmbh, case y.ybxmbh when '39199999990001000001' then '医保不予支付的中成药费用' when '61048000000500000000' then '人工煎药' else y.ybxmmc end ybxmmc, m.dj, sum(m.sl) sl, sum(m.je) je, m.yyxmbh, m.yyxmmc, m.ysdm, n.b1name ysxm, m.ksno, o.b2ejnm zxks, m.sfno, y.sfxmzldm ybsfxmzldm, y.sflbdm ybsflbdm, m.cfh from 
                        (
                            --药品
                            select a.mcypno yyxmbh, a.mcypnm yyxmmc, a.mcpric dj, a.mcquty sl, a.mcamnt je, a.mcksno ksno, a.mcuser ysdm, a.mcsflb sfno, a.mccfno cfh
                            from mzcfd a 
                            where a.mcghno = '{0}' and a.mccfno in ({1})
                            union all
                            --检查/治疗
                            select b.mbitem yyxmbh, b.mbname yyxmmc, b.mbpric dj, b.mbquty sl, b.mbsjam je, b.mbksno ksno, b.mbuser ysdm, b.mbsfno sfno , b.mbzlno cfh          
                            from mzb2d b 
                            where b.mbghno = '{0}' and b.mbzlno in ({1})
                            union all
                            --检验
                            select c.mbitem yyxmbh, c.mbname yyxmmc, c.mbpric dj, c.mbquty sl, c.mbsjam je, c.mbksno ksno, c.mbuser ysdm, c.mbsfno sfno, c.mbzlno cfh
                            from mzb4d c 
                            where c.mbghno = '{0}' and c.mbzlno in ({1})
                            union all
                            --注射
                            select b5item yyxmbh, b5name yyxmmc, b5sfam dj, mddays sl, b5sfam * mddays je, mdzsks ksno, mdempn ysdm, b5sfno sfno, mdzsno cfh
                            from mzd3d
                            left join (select distinct mccfno, mcghno from mzcfd) mzcf on mccfno = mdcfno
                            left join bz09d on b9mbno = mdtwid 
                            left join bz05d on b5item = b9item where mdtiwe > 0 and mdzsno in ({1})
                            union all
                            select b5item yyxmbh, b5name yyxmmc, b5sfam dj, mdtims sl, b5sfam * mdtims je, mdzsks ksno, mdempn ysdm, b5sfno sfno, mdzsno cfh
                            from mzd3d 
                            left join bz09d on b9mbno = mdwayid 
                            left join bz05d on b5item = b9item
                            left join (select distinct mccfno, mcghno from mzcfd) mzcf on mccfno = mdcfno 
                            where mdzsno in ({1})
                            union all
                            select b5item yyxmbh, b5name yyxmmc, b5sfam dj, mdpqty sl, b5sfam * mdpqty je, mdzsks ksno, mdempn ysdm, b5sfno sfno, mdzsno cfh
                            from mzd3d 
                            left join bz09d on b9mbno = mdpprid 
                            left join bz05d on b5item = b9item
                            left join (select distinct mccfno, mcghno from mzcfd) mzcf on mccfno = mdcfno
                            where mdpqty > 0 and mdzsno in ({1})
                            union all
                            --处方划价
                            select a.ygypno yyxmbh, a.ygypnm yyxmmc, ((a.ygamnt + 0.0) / a.ygslxx) dj, a.ygslxx sl, a.ygamnt je, b.ygksno ksno, b.ygysno ysdm, c.y1sflb, a.ygshno cfh
                            from yp17d a 
                            join yp17h b on a.ygcomp = b.ygcomp and a.ygshno = b.ygshno
                            join yp01h c on c.y1ypno = a.ygypno
                            where b.ygghno = '{0}' and a.ygshno in ({1}) and a.ygslxx > 0
                        ) m 
                        left join bz01h n on m.ysdm = n.b1empn 
                        left join bz02d o on m.ksno = o.b2ejks
                        left join ybhisdzdr y on m.yyxmbh = y.hisxmbh and y.scbz = 1
                        group by case y.ybxmbh when '39199999990001000001' then 'Z-00000-00000' when '61048000000500000000' then '61048000000400000000' else y.ybxmbh end, case y.ybxmbh when '39199999990001000001' then '医保不予支付的中成药费用' when '61048000000500000000' then '人工煎药' else y.ybxmmc end, m.dj, m.yyxmbh, m.yyxmmc, m.ysdm, n.b1name, m.ksno, o.b2ejnm, m.sfno, y.sfxmzldm, y.sflbdm, m.cfh"
                                        , jzlsh, cfhs);//3秒
                }
                else
                {
                    strSql = string.Format(@"select y.ybxmbh, y.ybxmmc, m.dj, sum(m.sl) sl, sum(m.je) je, m.yyxmbh, m.yyxmmc, m.ysdm, n.b1name ysxm, m.ksno, o.b2ejnm zxks, m.sfno, y.sfxmzldm ybsfxmzldm, y.sflbdm ybsflbdm, m.cfh from 
                        (
                            --药品
                            select a.mcypno yyxmbh, a.mcypnm yyxmmc, a.mcpric dj, a.mcquty sl, a.mcamnt je, a.mcksno ksno, a.mcuser ysdm, a.mcsflb sfno, a.mccfno cfh
                            from mzcfd a 
                            where a.mcghno = '{0}' and a.mccfno in ({1})
                            union all
                            --检查/治疗
                            select b.mbitem yyxmbh, b.mbname yyxmmc, b.mbpric dj, b.mbquty sl, b.mbsjam je, b.mbksno ksno, b.mbuser ysdm, b.mbsfno sfno , b.mbzlno cfh          
                            from mzb2d b 
                            where b.mbghno = '{0}' and b.mbzlno in ({1})
                            union all
                            --检验
                            select c.mbitem yyxmbh, c.mbname yyxmmc, c.mbpric dj, c.mbquty sl, c.mbsjam je, c.mbksno ksno, c.mbuser ysdm, c.mbsfno sfno, c.mbzlno cfh
                            from mzb4d c 
                            where c.mbghno = '{0}' and c.mbzlno in ({1})
                            union all
                            --注射
                            select b5item yyxmbh, b5name yyxmmc, b5sfam dj, mddays sl, b5sfam * mddays je, mdzsks ksno, mdempn ysdm, b5sfno sfno, mdzsno cfh
                            from mzd3d
                            left join (select distinct mccfno, mcghno from mzcfd) mzcf on mccfno = mdcfno
                            left join bz09d on b9mbno = mdtwid 
                            left join bz05d on b5item = b9item where mdtiwe > 0 and mdzsno in ({1})
                            union all
                            select b5item yyxmbh, b5name yyxmmc, b5sfam dj, mdtims sl, b5sfam * mdtims je, mdzsks ksno, mdempn ysdm, b5sfno sfno, mdzsno cfh
                            from mzd3d 
                            left join bz09d on b9mbno = mdwayid 
                            left join bz05d on b5item = b9item
                            left join (select distinct mccfno, mcghno from mzcfd) mzcf on mccfno = mdcfno 
                            where mdzsno in ({1})
                            union all
                            select b5item yyxmbh, b5name yyxmmc, b5sfam dj, mdpqty sl, b5sfam * mdpqty je, mdzsks ksno, mdempn ysdm, b5sfno sfno, mdzsno cfh
                            from mzd3d 
                            left join bz09d on b9mbno = mdpprid 
                            left join bz05d on b5item = b9item
                            left join (select distinct mccfno, mcghno from mzcfd) mzcf on mccfno = mdcfno
                            where mdpqty > 0 and mdzsno in ({1})
                            union all
                            --处方划价
                            select a.ygypno yyxmbh, a.ygypnm yyxmmc, ((a.ygamnt + 0.0) / a.ygslxx) dj, a.ygslxx sl, a.ygamnt je, b.ygksno ksno, b.ygysno ysdm, c.y1sflb, a.ygshno cfh
                            from yp17d a 
                            join yp17h b on a.ygcomp = b.ygcomp and a.ygshno = b.ygshno
                            join yp01h c on c.y1ypno = a.ygypno
                            where b.ygghno = '{0}' and a.ygshno in ({1}) and a.ygslxx > 0
                        ) m 
                        left join bz01h n on m.ysdm = n.b1empn 
                        left join bz02d o on m.ksno = o.b2ejks
                        left join ybhisdzdr y on m.yyxmbh = y.hisxmbh and y.scbz = 1
                        group by y.ybxmbh, y.ybxmmc, m.dj, m.yyxmbh, m.yyxmmc, m.ysdm, n.b1name, m.ksno, o.b2ejnm, m.sfno, y.sfxmzldm, y.sflbdm, m.cfh"
                                        , jzlsh, cfhs);//3秒
                }

                ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);

                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    DataTable dt = ds.Tables[0];
                    StringBuilder wdzxms = new StringBuilder();

                    for (int k = 0; k < dt.Rows.Count; k++)
                    {
                        DataRow dr = dt.Rows[k];

                        if (dr["ybxmbh"] == DBNull.Value)
                        {
                            wdzxms.Append("医保提示：项目代码：[" + dr["yyxmbh"].ToString() + "]，名称：[" + dr["yyxmmc"].ToString() + "]未对照或未上传，不能上传费用");
                        }
                        else if (dr["ksno"] == DBNull.Value || dr["ksno"].ToString() == "")
                        {
                            return new object[] { 0, 0, "医保提示：有科室编号为空的记录" };
                        }
                        else if (dr["zxks"] == DBNull.Value || dr["zxks"].ToString() == "")
                        {
                            return new object[] { 0, 0, "医保提示：无此科室编号[" + dr["ksno"].ToString() + "]" };
                        }
                        else
                        {
                            string ybsfxmzldm = dr["ybsfxmzldm"].ToString();
                            string ybsflbdm = dr["ybsflbdm"].ToString();
                            string yyxmbh = dr["yyxmbh"].ToString();
                            string ybxmbh = dr["ybxmbh"].ToString();
                            string yyxmmc = dr["yyxmmc"].ToString();
                            decimal dj = Convert.ToDecimal(dr["dj"]);
                            decimal sl = Convert.ToDecimal(dr["sl"]);
                            decimal je = Convert.ToDecimal(dr["je"]);
                            decimal mcyl = 1;
                            string ysbm = string.IsNullOrWhiteSpace(dr["ysdm"].ToString()) ? "010001" : dr["ysdm"].ToString();
                            string ysxm = string.IsNullOrWhiteSpace(dr["ysxm"].ToString()) ? "严东标" : dr["ysxm"].ToString();
                            string ksdm = dr["ksno"].ToString();
                            string ksmc = dr["zxks"].ToString();
                            string ybcfh = cfsj + k.ToString();
                            string ypjldw = "-";
                            string cydffbz = "0";

                            if (ybsfxmzldm == "1")
                            {
                                cydffbz = "1";
                            }

                            if (dr["sfno"].ToString() == "01" || dr["sfno"].ToString() == "02" || dr["sfno"].ToString() == "03")
                            {
                                ypjldw = "粒";
                            }

                            rc += "$" + ybjzlsh + "|" + ybsfxmzldm + "|" + ybsflbdm + "|" + ybcfh + "|" + cfsj
                            + "|" + yyxmbh + "|" + ybxmbh + "|" + yyxmmc + "|" + Math.Round(dj, 4).ToString() + "|" + Math.Round(sl, 2).ToString() + "|"
                            + Math.Round(je, 4).ToString() + "|101||" + mcyl.ToString() + "||" + ysbm + "|" + ysxm + "|||" + ksdm + "|"
                            + ksmc + "||" + cydffbz + "|" + jbr + "|" + ypjldw + "||";
                        }
                    }

                    if (wdzxms.Length > 0)
                    {
                        return new object[] { 0, 0, "医保提示：" + wdzxms };
                    }
                }
                else
                {
                    return new object[] { 0, 0, "医保提示：无his费用明细" };
                }

                Frm_ybfyscxxdrjj frm_ybfyscxxdrjj = new Frm_ybfyscxxdrjj(jzlsh, cfhs, xm);
                frm_ybfyscxxdrjj.ShowDialog();
                StringBuilder inputData = new StringBuilder(string.Format("{0}^{1}^{2}^{3}^{4}^{5}^{6}^{7}^", "2610", DDYLJGBH, czygh, ywzqh, jylsh, ZXBM, rc, "1"));
                StringBuilder outputData = new StringBuilder(100000);
                int i = BUSINESS_HANDLE(inputData, outputData);

                if (i == 0)
                {
                    string fjsfh = outputData.ToString().Split('^')[2];
                    string[] sfjsfh = fjsfh.Split('|');
                    string ylfze = sfjsfh[0];//医疗费总额
                    string zbxje = sfjsfh[1];//总报销金额
                    string tcjjzf = sfjsfh[2];//统筹基金支付
                    string dejjzf = sfjsfh[3];//大额基金支付
                    string zhzf = sfjsfh[4];//账户支付
                    string xjzf = sfjsfh[5];//现金支付
                    string gwybzjjzf = sfjsfh[6];//公务员补助基金支付
                    string qybcylbxjjzf = sfjsfh[7];//企业补充医疗保险基金支付
                    string zffy = sfjsfh[8];//自费费用
                    string dwfdfy = sfjsfh[9];//单位负担费用
                    string yyfdfy = sfjsfh[10];//医院负担费用
                    string mzjzfy = sfjsfh[11];//民政救助费用
                    string cxjfy = sfjsfh[12];//超限价费用
                    string ylzlfy = sfjsfh[13];//乙类自理费用
                    string blzlfy = sfjsfh[14];//丙类自理费用
                    string fhjbylfy = sfjsfh[15];//符合基本医疗费用
                    string qfbzfy = sfjsfh[16];//起付标准费用
                    string zzzyzffy = sfjsfh[17];//转诊转院自付费用
                    string jrtcfy = sfjsfh[18];//进入统筹费用
                    string tcfdzffy = sfjsfh[19];//统筹分段自付费用
                    string ctcfdxfy = sfjsfh[20];//超统筹封顶线费用
                    string jrdebxfy = sfjsfh[21];//进入大额报销费用
                    string defdzffy = sfjsfh[22];//大额分段自付费用
                    string cdefdxfy = sfjsfh[23];//超大额封顶线费用
                    string rgqgzffy = sfjsfh[24];//人工器官自付费用
                    string bcjsqzhye = sfjsfh[25];//本次结算前帐户余额
                    string bntczflj = sfjsfh[26];//本年统筹支付累计(不含本次)
                    string bndezflj = sfjsfh[27];//本年大额支付累计(不含本次)
                    string bnczjmmztczflj = sfjsfh[28];//本年城镇居民门诊统筹支付累计(不含本次)
                    string bngwybzzflj = sfjsfh[29];//本年公务员补助支付累计(不含本次)
                    string bnzhzflj = sfjsfh[30];//本年账户支付累计(不含本次) 
                    string bnzycslj = sfjsfh[31];//本年住院次数累计(不含本次)
                    string zycs = sfjsfh[32];//住院次数
                    string jsrq1 = sfjsfh[34];//结算时间
                    string yllb1 = sfjsfh[35];//医疗类别
                    string yldylb = sfjsfh[36];//医疗待遇类别
                    string jbjgbm = sfjsfh[37];//经办机构编码
                    string ywzqh1 = sfjsfh[38];//业务周期号
                    string jslsh = sfjsfh[39];//结算流水号
                    string tsxx = sfjsfh[40];//提示信息
                    string djh = sfjsfh[41];//单据号
                    string jylx = sfjsfh[42];//交易类型
                    string yyjylsh = sfjsfh[43];//医院交易流水号
                    string yxbz = sfjsfh[44];//有效标志
                    string grbhgl = sfjsfh[45];//个人编号管理
                    string yljgbm = sfjsfh[46];//医疗机构编码
                    string strSql1 = string.Format(@"insert into ybfyyjsdr(jzlsh, jylsh, djhin, jsrq, jbr, bzbm, bzmc, yllb, kfsbz, grbh, xm, kh
                    , ylfze, zbxje, tcjjzf, dejjzf, zhzf, xjzf, gwybzjjzf, qybcylbxjjzf, zffy, dwfdfy, yyfdfy, mzjzfy, cxjfy, ylzlfy, blzlfy
                    , fhjbylfy, qfbzfy, zzzyzffy, jrtcfy, tcfdzffy, ctcfdxfy, jrdebxfy, defdzffy, cdefdxfy, rgqgzffy, bcjsqzhye, bntczflj
                    , bndezflj, bnczjmmztczflj, bngwybzzflj, bnzhzflj, bnzycslj, zycs, yldylb, jbjgbm, ywzqh, jslsh, tsxx, djh, jylx
                    , yyjylsh, yxbz, grbhgl, yljgbm) 
                    values('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}'
                    , '{15}', '{16}', '{17}', '{18}', '{19}', '{20}', '{21}', '{22}', '{23}', '{24}', '{25}', '{26}', '{27}', '{28}'
                    , '{29}', '{30}', '{31}', '{32}', '{33}', '{34}', '{35}', '{36}', '{37}', '{38}', '{39}', '{40}', '{41}', '{42}'
                    , '{43}', '{44}', '{45}', '{46}', '{47}', '{48}', '{49}', '{50}', '{51}', '{52}', '{53}', '{54}', '{55}')"
                    , jzlsh, jylsh, djhin, cfsj, jbr, bzbm, bzmc, yllb, Gocent, grbh, xm, kh
                    , ylfze, zbxje, tcjjzf, dejjzf, zhzf, xjzf, gwybzjjzf, qybcylbxjjzf, zffy, dwfdfy, yyfdfy, mzjzfy, cxjfy, ylzlfy, blzlfy
                    , fhjbylfy, qfbzfy, zzzyzffy, jrtcfy, tcfdzffy, ctcfdxfy, jrdebxfy, defdzffy, cdefdxfy, rgqgzffy, bcjsqzhye, bntczflj
                    , bndezflj, bnczjmmztczflj, bngwybzzflj, bnzhzflj, bnzycslj, zycs, yldylb, jbjgbm, ywzqh, jslsh, tsxx, djh, jylx
                    , yyjylsh, yxbz, grbhgl, yljgbm);
                    obj = new object[] { strSql1 };
                    obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);

                    if (obj[1].ToString() == "1")
                    {
                        return new object[] { 0, 1, outputData.ToString().Split('^')[2] + "0|0|0|0|0|0|0|0|0|0|0|0|" };
                    }
                    else
                    {
                        Common.WriteYBLog(obj[2].ToString());
                        return new object[] { 0, 0, "医保提示：his数据库操作失败,错误信息：" + obj[2].ToString() };
                    }
                }
                else
                {
                    return new object[] { 0, 0, outputData };
                }
            }
            catch (Exception error)
            {
                Common.WriteYBLog(error.ToString());
                return new object[] { 0, 2, error.ToString() };
            }
        }
        #endregion 门诊预结算异地

        #region 门诊结算
        /// <summary>
        /// 门诊结算
        /// </summary> 
        /// <param>his就诊流水号,单据号,账户使用标志（0或1）,结算时间(格式：yyyy-MM-dd HH:mm:ss)，病种编码(非慢性病传空字符串)，病种名称(非慢性病传空字符串)，处方号集合(格式：'01','02')，医疗类别代码,总金额,读卡标志(不传表示读卡，传了则不读卡)</param>
        /// <returns>医疗费总额|总报销金额|统筹基金支付|大额基金支付|账户支付|现金支付|公务员补助基金支付|企业补充医疗保险基金支付|自费费用|单位负担费用|医院负担费用|民政救助费用|超限价费用|乙类自理费用|丙类自理费用|符合基本医疗费用|起付标准费用|转诊转院自付费用|进入统筹费用|统筹分段自付费用|超统筹封顶线费用|进入大额报销费用|大额分段自付费用|超大额封顶线费用|人工器官自付费用|本次结算前帐户余额|本年统筹支付累计(不含本次)|本年大额支付累计(不含本次)|本年城镇居民门诊统筹支付累计(不含本次)|本年公务员补助支付累计(不含本次)|本年账户支付累计(不含本次)|本年住院次数累计(不含本次)|住院次数|姓名|结算时间|医疗类别|医疗待遇类别|经办机构编码|业务周期号|结算流水号|提示信息|单据号|交易类型|医院交易流水号|有效标志|个人编号|医疗机构编码|二次补偿金额|门慢起付累计|接收方交易流水号|个人编号|单病种补差|财政支出金额|二类门慢限额支出|二类门慢限额剩余|居民个人自付二次补偿金额|</returns>
        public static object[] YBJSMZDR(object[] objParam)
        {
            string jzlsh = objParam[0].ToString();
            string djh = objParam[1].ToString();
            string zhsybz = objParam[2].ToString();
            string jssj = objParam[3].ToString();
            string bzbm = objParam[4].ToString();
            string bzmc = objParam[5].ToString();
            string cfhs = objParam[6].ToString();
            string yllb = objParam[7].ToString();
            decimal hisylfze = Convert.ToDecimal(objParam[8]);

            if (string.IsNullOrWhiteSpace(jzlsh))
            {
                return new object[] { 0, 0, "医保提示：his就诊流水号为空" };
            }
            else if (string.IsNullOrWhiteSpace(djh))
            {
                return new object[] { 0, 0, "医保提示：单据号为空" };
            }
            else if (string.IsNullOrWhiteSpace(zhsybz))
            {
                return new object[] { 0, 0, "医保提示：账户使用标志为空" };
            }
            else if (string.IsNullOrWhiteSpace(jssj))
            {
                return new object[] { 0, 0, "医保提示：结算时间为空" };
            }
            else if (string.IsNullOrWhiteSpace(yllb))
            {
                return new object[] { 0, 0, "医保提示：医保医疗类别代码为空" };
            }

            string strSql = string.Format("select ydrybz from ybmzzydjdr a where a.jzlsh = '{0}' and a.cxbz = 1 and a.jzbz = 'm'", jzlsh);//0秒
            DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);

            if (ds == null || ds.Tables[0].Rows.Count == 0)
            {
                return new object[] { 0, 0, "医保提示：无医保挂号登记记录" };
            }

            DataTable dt = ds.Tables[0];
            DataRow dr = dt.Rows[0];
            string ydrybz = dr["ydrybz"].ToString();

            if (ydrybz == "0")
            {
                if (objParam.Length == 10)
                {
                    return YBJSMZBD(new object[] { jzlsh, djh, zhsybz, jssj, bzbm, bzmc, yllb, hisylfze, objParam[9] });
                }
                else
                {
                    return YBJSMZBD(new object[] { jzlsh, djh, zhsybz, jssj, bzbm, bzmc, yllb, hisylfze });
                }
            }
            else
            {
                if (string.IsNullOrWhiteSpace(cfhs))
                {
                    return new object[] { 0, 0, "医保提示：处方号集合为空" };
                }

                if (objParam.Length == 10)
                {
                    return YBJSMZYD(new object[] { jzlsh, djh, jssj, bzbm, bzmc, cfhs, yllb, hisylfze, objParam[9] });
                }
                else
                {
                    return YBJSMZYD(new object[] { jzlsh, djh, jssj, bzbm, bzmc, cfhs, yllb, hisylfze });
                }
            }
        }
        #endregion 门诊结算

        #region 门诊结算本地
        /// <summary>
        /// 门诊结算本地
        /// </summary>
        /// <param>his就诊流水号,单据号,账户使用标志（0或1）,结算时间(格式：yyyy-MM-dd HH:mm:ss)，病种编码(非慢性病传空字符串)，病种名称(非慢性病传空字符串)，医疗类别代码，总金额, 读卡标志(不传表示读卡，传了则不读卡)</param>
        /// <returns>医疗费总额|总报销金额|统筹基金支付|大额基金支付|账户支付|现金支付|公务员补助基金支付|企业补充医疗保险基金支付|自费费用|单位负担费用|医院负担费用|民政救助费用|超限价费用|乙类自理费用|丙类自理费用|符合基本医疗费用|起付标准费用|转诊转院自付费用|进入统筹费用|统筹分段自付费用|超统筹封顶线费用|进入大额报销费用|大额分段自付费用|超大额封顶线费用|人工器官自付费用|本次结算前帐户余额|本年统筹支付累计(不含本次)|本年大额支付累计(不含本次)|本年城镇居民门诊统筹支付累计(不含本次)|本年公务员补助支付累计(不含本次)|本年账户支付累计(不含本次)|本年住院次数累计(不含本次)|住院次数|姓名|结算时间|医疗类别|医疗待遇类别|经办机构编码|业务周期号|结算流水号|提示信息|单据号|交易类型|医院交易流水号|有效标志|个人编号|医疗机构编码|二次补偿金额|门慢起付累计|接收方交易流水号|个人编号|单病种补差|财政支出金额|二类门慢限额支出|二类门慢限额剩余|居民个人自付二次补偿金额|</returns>
        public static object[] YBJSMZBD(object[] objParam)
        {
            bool isybjsok = false;
            string ybjzlsh = "";
            string djhin = "";
            string dqrq = "";
            string grbh = "";
            string xm = "";
            string kh = "";
            ZXBM = "0000";

            try
            {
                string czygh = CliUtils.fLoginUser;
                string ywzqh = YWZQH;
                string jbr = CliUtils.fUserName;
                string jzlsh = objParam[0].ToString();
                djhin = objParam[1].ToString();
                string cyyy = "9";
                string zhsybz = objParam[2].ToString();
                string ztjsbz = "0";
                string bzbm = objParam[4].ToString();
                string bzmc = objParam[5].ToString();
                string yllb = objParam[6].ToString();
                decimal hisylfze = Convert.ToDecimal(objParam[7]);

                if (string.IsNullOrWhiteSpace(jzlsh))
                {
                    return new object[] { 0, 0, "医保提示：his就诊流水号为空" };
                }
                else if (string.IsNullOrWhiteSpace(djhin))
                {
                    return new object[] { 0, 0, "医保提示：单据号为空" };
                }
                else if (string.IsNullOrWhiteSpace(zhsybz))
                {
                    return new object[] { 0, 0, "医保提示：账户使用标志为空" };
                }
                else if (string.IsNullOrWhiteSpace(objParam[3].ToString()))
                {
                    return new object[] { 0, 0, "医保提示：结算时间为空" };
                }
                else if (string.IsNullOrWhiteSpace(yllb))
                {
                    return new object[] { 0, 0, "医保提示：医疗类别代码为空" };
                }

                dqrq = Convert.ToDateTime(objParam[3]).ToString("yyyyMMddHHmmss");
                DateTime dqsj = Convert.ToDateTime(Common.GetServerTime());
                string jylsh = dqsj.ToString("yyyyMMddHHmmss") + "-" + DDYLJGBH + "-" + jzlsh;
                string strSql = string.Format("select jzlsh from ybfyjsdr a where a.jzlsh = '{0}' and a.djhin = '{1}' and a.cxbz = 1", jzlsh, djhin);//0秒
                DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);

                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    return new object[] { 0, 0, "医保提示：his就诊流水号：" + jzlsh + "，单据号：" + djhin + "已结算" };
                }

                strSql = string.Format("select distinct jylsh from ybcfmxscfhdr a where a.cxbz = 1 and a.jzlsh = '{0}' and a.jsdjh is null", jzlsh);//0秒
                ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);

                if (ds == null || ds.Tables[0].Rows.Count == 0)
                {
                    return new object[] { 0, 0, "医保提示：his就诊流水号：" + jzlsh + "无费用明细上传记录" };
                }

                strSql = string.Format("select xm, ybjzlsh, grbh, xm, kh from ybmzzydjdr a where a.jzlsh = '{0}' and a.cxbz = 1 and a.jzbz = 'm'", jzlsh);//0秒
                ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);

                if (ds == null || ds.Tables[0].Rows.Count == 0)
                {
                    return new object[] { 0, 0, "医保提示：his就诊流水号：" + jzlsh + "无挂号登记记录" };
                }

                DataRow dr = ds.Tables[0].Rows[0];
                object[] obj = null;

                if (objParam.Length == 8)
                {
                    obj = YBDKDR(new object[] { });

                    if (obj[1].ToString() == "1")
                    {
                        if (dr["xm"].ToString() != obj[2].ToString().Split('|')[3])
                        {
                            return new object[] { 0, 0, "医保提示：不是本人卡" };
                        }
                    }
                    else
                    {
                        return new object[] { 0, 0, "医保提示：读卡错误：" + obj[2].ToString() };
                    }
                }

                ybjzlsh = dr["ybjzlsh"].ToString();
                grbh = dr["grbh"].ToString();
                xm = dr["xm"].ToString();
                kh = dr["kh"].ToString();
                string rc = ybjzlsh + "|" + djhin + "|" + yllb + "|" + dqrq + "|" + dqrq + "|" + cyyy + "|"
                + bzbm + "|" + bzmc + "|" + zhsybz + "|" + ztjsbz + "|" + jbr + "|"
                + Gocent + "||||||||||" + grbh + "|" + xm + "|" + kh + "|0|";
                StringBuilder inputData = new StringBuilder(string.Format("{0}^{1}^{2}^{3}^{4}^{5}^{6}^{7}^", "2410", DDYLJGBH, czygh, ywzqh, jylsh, ZXBM, rc, "1"));
                StringBuilder outputData = new StringBuilder(100000);
                int i = BUSINESS_HANDLE(inputData, outputData);

                if (i == 0)
                {
                    isybjsok = true;
                    string[] sfjsfh = outputData.ToString().Split('^')[2].Split('|');
                    string ylfze = sfjsfh[0];//医疗费总额
                    string zbxje = sfjsfh[1];//总报销金额
                    string tcjjzf = sfjsfh[2];//统筹基金支付
                    string dejjzf = sfjsfh[3];//大额基金支付
                    string zhzf = sfjsfh[4];//账户支付
                    string xjzf = sfjsfh[5];//现金支付
                    string gwybzjjzf = sfjsfh[6];//公务员补助基金支付
                    string qybcylbxjjzf = sfjsfh[7];//企业补充医疗保险基金支付
                    string zffy = sfjsfh[8];//自费费用
                    string dwfdfy = sfjsfh[9];//单位负担费用
                    string yyfdfy = sfjsfh[10];//医院负担费用
                    string mzjzfy = sfjsfh[11];//民政救助费用
                    string cxjfy = sfjsfh[12];//超限价费用
                    string ylzlfy = sfjsfh[13];//乙类自理费用
                    string blzlfy = sfjsfh[14];//丙类自理费用
                    string fhjbylfy = sfjsfh[15];//符合基本医疗费用
                    string qfbzfy = sfjsfh[16];//起付标准费用
                    string zzzyzffy = sfjsfh[17];//转诊转院自付费用
                    string jrtcfy = sfjsfh[18];//进入统筹费用
                    string tcfdzffy = sfjsfh[19];//统筹分段自付费用
                    string ctcfdxfy = sfjsfh[20];//超统筹封顶线费用
                    string jrdebxfy = sfjsfh[21];//进入大额报销费用
                    string defdzffy = sfjsfh[22];//大额分段自付费用
                    string cdefdxfy = sfjsfh[23];//超大额封顶线费用
                    string rgqgzffy = sfjsfh[24];//人工器官自付费用
                    string bcjsqzhye = sfjsfh[25];//本次结算前帐户余额
                    string bntczflj = sfjsfh[26];//本年统筹支付累计(不含本次)
                    string bndezflj = sfjsfh[27];//本年大额支付累计(不含本次)
                    string bnczjmmztczflj = sfjsfh[28];//本年城镇居民门诊统筹支付累计(不含本次)
                    string bngwybzzflj = sfjsfh[29];//本年公务员补助支付累计(不含本次)
                    string bnzhzflj = sfjsfh[30];//本年账户支付累计(不含本次) 
                    string bnzycslj = sfjsfh[31];//本年住院次数累计(不含本次)
                    string zycs = sfjsfh[32];//住院次数
                    //姓名
                    string jsrq = sfjsfh[34];//结算时间
                    string yllb1 = sfjsfh[35];//医疗类别
                    string yldylb = sfjsfh[36];//医疗待遇类别
                    string jbjgbm = sfjsfh[37];//经办机构编码
                    string ywzqh1 = sfjsfh[38];//业务周期号
                    string jslsh = sfjsfh[39];//结算流水号
                    string tsxx = sfjsfh[40];//提示信息
                    string djh = sfjsfh[41];//单据号
                    string jylx = sfjsfh[42];//交易类型
                    string yyjylsh = sfjsfh[43];//医院交易流水号
                    string yxbz = sfjsfh[44];//有效标志
                    string grbhgl = sfjsfh[45];//个人编号管理
                    string yljgbm = sfjsfh[46];//医疗机构编码
                    string ecbcje = sfjsfh[47];//二次补偿金额
                    string mmqflj = sfjsfh[48];//门慢起付累计
                    string jsfjylsh = sfjsfh[49];//接收方交易流水号
                    string dbzbc = sfjsfh[51];//单病种补差
                    string czzcje = sfjsfh[52];//财政支出金额
                    string elmmxezc = sfjsfh[53];//二类门慢限额支出
                    string elmmxesy = sfjsfh[54];//二类门慢限额剩余
                    string jmgrzfecbcje = sfjsfh[55];//居民个人自付二次补偿金额
                    string tjje = sfjsfh[56];//体检金额
                    string syjjzf = sfjsfh[57];//生育基金支付


                    string jjjmdbydje = sfjsfh[58];//九江居民大病一段金额
                    string jjjmdbedje = sfjsfh[59];//九江居民大病二段金额
                    string jjjmjbbcfwnje = sfjsfh[60];//九江居民疾病补充范围内金额
                    string jjjmjbbcfwwje = sfjsfh[61];//九江居民疾病补充范围外金额
                    string mgwxlcjjzf = sfjsfh[62];//美国微笑列车基金支付
                    string jjjmzcfwwkbxfy = sfjsfh[63];//九江居民政策范围外可报销费用
                    string zfddjjfy = sfjsfh[64];//政府兜底基金费用
                    string mfmzjj = sfjsfh[65];//免费门诊基金（余江）
                    string jddbbcbcydzfje = sfjsfh[66];//建档大病补偿本次一段支付金额
                    string jddbbcbcedzfje = sfjsfh[67];//建档大病补偿本次二段支付金额
                    string jddbbcbcsdzfje = sfjsfh[68];//建档大病补偿本次三段支付金额
                    string jdecbcbcydzfje = sfjsfh[69];//建档二次补偿本次一段支付金额
                    string jdecbcbcedzfje = sfjsfh[70];//建档二次补偿本次二段支付金额
                    string jdecbcbcsdzfje = sfjsfh[71];//建档二次补偿本次三段支付金额
                    string jbbcbxbczcfwnfyydzfje = sfjsfh[72];//疾病补充保险本次政策范围内费用一段支付金额
                    string jbbcbxbczcfwnfyedzfje = sfjsfh[73];//疾病补充保险本次政策范围内费用二段支付金额
                    string jmdbydje = sfjsfh[58]; //居民大病一段金额
                    string jmdbedje = sfjsfh[59]; //居民大病二段金额
                    string jbbcfwnzfje = sfjsfh[60]; //疾病补充范围内费用支付金额
                    string jbbcbxbczcfwwfyzfje = sfjsfh[61]; //疾病补充保险本次政策范围外费用支付金额
                    string bnzfddjjfylj = sfjsfh[74]; //本年政府兜底基金费用累计


                    //string strValue = ylfze + "|"+zbxje+"|" + tcjjzf + "|" + dejjzf + "|" + zhzf + "|" + xjzf + "|" + gwybzjjzf + "|" + dwfdfy + "|" + zffy + "|" + dwfdfy + "|" +
                    //                yyfdfy + "|" + mzjzfy + "|" + cxjfy + "|" + ylzlfy + "|"+blzlfy+"|"+fhjbylfy+"|" + qfbzfy + "|"+zzzyzffy+"|" + jrtcfy + "|"+tcfdzffy+"|" +
                    //                ctcfdxfy + "|" + jrdebxfy + "|" + defdzffy + "|"+cdefdxfy+"|"+rgqgzffy+"|" + bcjsqzhye + "|"+bntczflj+"|"+bndezflj+"|"+bnczjmmztczflj+"|"+bngwybzzflj+"|" +
                    //                bnzhzflj + "|" + bnzycslj + "|" + zycs + "|" + xm + "|" + jsrq + "|" + yllb2 + "|" + ylrylb + "|" + dqbh2 + "||" + jslsh + "||" +
                    //                djhin + "|||1||" + dqbh2 + "|" + dbecbc + "|" + 0.00 + "|0.00|" + bxh2 + "||" +
                    //                zftdjj + "|" + 0.00 + "|" + 0.00 + "|" + 0.00 + "|" + 0.00 + "|" + 0.00 + "|";

                    if (Math.Abs(hisylfze - Convert.ToDecimal(ylfze)) > 1)
                    {
                        i = YBFYJSSDCXDR(ybjzlsh, djh, dqrq, grbh, xm, kh, ZXBM);

                        if (i == 1)
                        {
                            return new object[] { 0, 3, "医保提示：his就诊流水号：" + jzlsh + "his总金额" + hisylfze + "与医保总金额" + ylfze + "不相同，已自动冲销，请重新结算" };
                        }
                        else
                        {
                            return new object[] { 0, 0, "医保提示：his就诊流水号：" + jzlsh + "his总金额" + hisylfze + "与医保总金额" + ylfze + "不相同，自动冲销失败，请手工冲销" };
                        }
                    }

                    string strSql1 = string.Format(@"insert into ybfyjsdr(jzlsh, jylsh, djhin, cyrq, cyyy, bzbm, bzmc, zhsybz, ztjsbz, jbr, kfsbz
                    , cyzdbm1, cyzdmc1, cyzdbm2, cyzdmc2, cyzdbm3, cyzdmc3, cyzdbm4, cyzdmc4, zlff, tes, ylfze, zbxje, tcjjzf, dejjzf, zhzf, xjzf
                    , gwybzjjzf, qybcylbxjjzf, zffy, dwfdfy, yyfdfy, mzjzfy, cxjfy, ylzlfy, blzlfy, fhjbylfy, qfbzfy, zzzyzffy, jrtcfy, tcfdzffy
                    , ctcfdxfy, jrdebxfy, defdzffy, cdefdxfy, rgqgzffy, bcjsqzhye, bntczflj, bndezflj, bnczjmmztczflj, bngwybzzflj, bnzhzflj
                    , bnzycslj, zycs, xm, jsrq, yllb, yldylb, jbjgbm, ywzqh, jslsh, tsxx, djh, jylx, yyjylsh, yxbz, grbhgl, yljgbm, ecbcje
                    , mmqflj, jsfjylsh, grbh, dbzbc, czzcje, elmmxezc, elmmxesy, jmgrzfecbcje, tjje, syjjzf, kh, jjjmdbydje, jjjmdbedje, jjjmjbbcfwnje, jjjmjbbcfwwje
                    , zfddjjfy,jjjmzcfwwkbxfy,jmdbydje,jmdbedje,jbbcfwnzfje,jbbcbxbczcfwwfyzfje,bnzfddjjfylj) 
                    values('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}'
                    , '{15}', '{16}', '{17}', '{18}', '{19}', '{20}', '{21}', '{22}', '{23}', '{24}', '{25}', '{26}', '{27}', '{28}'
                    , '{29}', '{30}', '{31}', '{32}', '{33}', '{34}', '{35}', '{36}', '{37}', '{38}', '{39}', '{40}', '{41}', '{42}'
                    , '{43}', '{44}', '{45}', '{46}', '{47}', '{48}', '{49}', '{50}', '{51}', '{52}', '{53}', '{54}', '{55}', '{56}', '{57}'
                    , '{58}', '{59}', '{60}', '{61}', '{62}', '{63}', '{64}', '{65}', '{66}', '{67}', '{68}', '{69}', '{70}', '{71}'
                    , '{72}', '{73}', '{74}', '{75}', '{76}', '{77}', '{78}', '{79}', '{80}', '{81}', '{82}', '{83}'
                    , '{84}', '{85}', '{86}', '{87}', '{88}', '{89}', '{90}')"
                    , jzlsh, jylsh, djhin, dqrq, cyyy, bzbm, bzmc, zhsybz, ztjsbz, jbr, Gocent, "", "", "", "", "", "", "", "", "", ""
                    , ylfze, zbxje, tcjjzf, dejjzf, zhzf, xjzf, gwybzjjzf, qybcylbxjjzf, zffy, dwfdfy, yyfdfy, mzjzfy, cxjfy, ylzlfy
                    , blzlfy, fhjbylfy, qfbzfy, zzzyzffy, jrtcfy, tcfdzffy, ctcfdxfy, jrdebxfy, defdzffy, cdefdxfy, rgqgzffy, bcjsqzhye
                    , bntczflj, bndezflj, bnczjmmztczflj, bngwybzzflj, bnzhzflj, bnzycslj, zycs, xm, dqrq, yllb, yldylb, jbjgbm, ywzqh
                    , jslsh, tsxx, djh, jylx, yyjylsh, yxbz, grbhgl, yljgbm, ecbcje, mmqflj, jsfjylsh, grbh, dbzbc, czzcje, elmmxezc
                    , elmmxesy, jmgrzfecbcje, tjje, syjjzf, kh, jjjmdbydje, jjjmdbedje, jjjmjbbcfwnje, jjjmjbbcfwwje
                    , zfddjjfy, jjjmzcfwwkbxfy, jmdbydje, jmdbedje, jbbcfwnzfje, jbbcbxbczcfwwfyzfje, bnzfddjjfylj);
                    string strSql2 = string.Format("update ybcfmxscfhdr set jsdjh = '{0}' where jzlsh = '{1}' and jsdjh is null and cxbz = 1", djhin, jzlsh);//0秒
                    string strSql3 = string.Format("update ybcfmxscindr set jsdjh = '{0}' where jzlsh = '{1}' and jsdjh is null and cxbz = 1", djhin, jzlsh);//0秒
                    string strSql4 = string.Format("delete from ybcfmxscindr where jzlsh = '{0}' and cxbz in (0, 2)", jzlsh);//0秒
                    string strSql5 = string.Format("delete from ybcfmxscfhdr where jzlsh = '{0}' and cxbz in (0, 2)", jzlsh);//0秒
                    obj = new object[] { strSql1, strSql2, strSql3, strSql4, strSql5 };
                    obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);

                    if (obj[1].ToString() == "1")
                    {
                        return new object[] { 0, 1, outputData.ToString().Split('^')[2] };
                    }
                    else
                    {
                        Common.WriteYBLog(obj[2].ToString());
                        i = YBFYJSSDCXDR(ybjzlsh, djhin, dqrq, grbh, xm, kh, ZXBM);

                        if (i == 1)
                        {
                            return new object[] { 0, 0, "医保提示：his数据库操作失败,自动撤销医保结算成功,错误信息：" + obj[2].ToString() };
                        }
                        else
                        {
                            return new object[] { 0, 0, "医保提示：his数据库操作失败,自动撤销医保结算失败,错误信息：" + obj[2].ToString() };
                        }
                    }
                }
                else if (i == -1)
                {
                    i = YBFYJSSDCXDR(ybjzlsh, djhin, dqrq, grbh, xm, kh, ZXBM);

                    if (i == 1)
                    {
                        return new object[] { 0, -1, "医保提示：医保门诊结算系统级别错误，自动撤销医保结算成功，" + outputData.ToString() };
                    }
                    else
                    {
                        return new object[] { 0, -1, "医保提示：医保门诊结算系统级别错误，自动撤销医保结算失败，" + outputData.ToString() };
                    }
                }
                else
                {
                    return new object[] { 0, -2, "医保提示：医保门诊结算业务级别或未知错误，" + outputData.ToString() };
                }
            }
            catch (Exception error)
            {
                Common.WriteYBLog(error.ToString());

                if (isybjsok)
                {
                    int i = YBFYJSSDCXDR(ybjzlsh, djhin, dqrq, grbh, xm, kh, ZXBM);

                    if (i == 1)
                    {
                        return new object[] { 0, 2, "医保提示：异常，自动撤销医保结算成功，" + error.ToString() };
                    }
                    else
                    {
                        return new object[] { 0, 2, "医保提示：异常，自动撤销医保结算失败，" + error.ToString() };
                    }
                }

                return new object[] { 0, 2, "Error:" + error.ToString() };
            }
        }
        #endregion 门诊结算本地

        #region 门诊结算异地
        /// <summary>
        /// 门诊结算异地
        /// </summary>
        /// <param>his就诊流水号,单据号,结算时间(格式：yyyy-MM-dd HH:mm:ss)，病种编码(非慢性病传空字符串)，病种名称(非慢性病传空字符串)，处方号集合(格式：'01','02')，医疗类别代码,总金额,读卡标志(不传表示读卡，传了则不读卡)</param>
        /// <returns>医疗费总额|总报销金额|统筹基金支付|大额基金支付|账户支付|现金支付|公务员补助基金支付|企业补充医疗保险基金支付|自费费用|单位负担费用|医院负担费用|民政救助费用|超限价费用|乙类自理费用|丙类自理费用|符合基本医疗费用|起付标准费用|转诊转院自付费用|进入统筹费用|统筹分段自付费用|超统筹封顶线费用|进入大额报销费用|大额分段自付费用|超大额封顶线费用|人工器官自付费用|本次结算前帐户余额|本年统筹支付累计(不含本次)|本年大额支付累计(不含本次)|本年城镇居民门诊统筹支付累计(不含本次)|本年公务员补助支付累计(不含本次)|本年账户支付累计(不含本次)|本年住院次数累计(不含本次)|住院次数|姓名|结算时间|医疗类别|医疗待遇类别|经办机构编码|业务周期号|结算流水号|提示信息|单据号|交易类型|医院交易流水号|有效标志|个人编号|医疗机构编码|二次补偿金额|门慢起付累计|接收方交易流水号|个人编号|单病种补差|财政支出金额|二类门慢限额支出|二类门慢限额剩余|居民个人自付二次补偿金额|</returns>
        public static object[] YBJSMZYD(object[] objParam)
        {
            bool isybjsmzyd = false;
            string ybjzlsh = "";
            string djh = "";
            string cfsj = "";
            string grbh = "";
            string xm = "";
            string kh = "";

            try
            {
                string czygh = CliUtils.fLoginUser;
                string ywzqh = YWZQH;
                string jbr = CliUtils.fUserName;
                string jzlsh = objParam[0].ToString();
                string djhin = objParam[1].ToString();
                string bzbm = objParam[3].ToString();
                string bzmc = objParam[4].ToString();
                string cfhs = objParam[5].ToString();
                string yllb = objParam[6].ToString();
                decimal hisylfze = Convert.ToDecimal(objParam[7]);

                if (string.IsNullOrWhiteSpace(jzlsh))
                {
                    return new object[] { 0, 0, "医保提示：his就诊流水号为空" };
                }
                else if (string.IsNullOrWhiteSpace(djhin))
                {
                    return new object[] { 0, 0, "医保提示：单据号为空" };
                }
                else if (string.IsNullOrWhiteSpace(objParam[2].ToString()))
                {
                    return new object[] { 0, 0, "医保提示：结算时间为空" };
                }
                else if (string.IsNullOrWhiteSpace(cfhs))
                {
                    return new object[] { 0, 0, "医保提示：处方号集合为空" };
                }
                else if (string.IsNullOrWhiteSpace(yllb))
                {
                    return new object[] { 0, 0, "医保提示：医疗类别代码为空" };
                }

                cfsj = Convert.ToDateTime(objParam[2]).ToString("yyyyMMddHHmmss");
                DateTime dqsj = Convert.ToDateTime(Common.GetServerTime());
                string jylsh = dqsj.ToString("yyyyMMddHHmmss") + "-" + DDYLJGBH + "-" + jzlsh;
                string strSql = string.Format("select top 1 a.grbh, a.xm, a.kh, a.tcqh, a.ybjzlsh from ybmzzydjdr a where a.jzlsh = '{0}' and a.cxbz = 1 and a.jzbz = 'm' order by a.sysdate desc", jzlsh);//0秒
                DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);

                if (ds == null || ds.Tables[0].Rows.Count == 0)
                {
                    return new object[] { 0, 0, "医保提示：his就诊流水号" + jzlsh + "无挂号登记记录" };
                }

                DataRow dr1 = ds.Tables[0].Rows[0];
                //ybjzlsh = dr1["ybjzlsh"].ToString();

                //if (yllb != "11")
                //{
                //    ybjzlsh = dr1["ybjzlshydmxb"].ToString();
                //}
                ybjzlsh = dr1["ybjzlsh"].ToString();
                grbh = dr1["grbh"].ToString();
                xm = dr1["xm"].ToString();
                kh = dr1["kh"].ToString();
                ZXBM = dr1["tcqh"].ToString();
                object[] obj = null;

                if (objParam.Length == 8)
                {
                    obj = YBDKDR(new object[] { });

                    if (obj[1].ToString() == "1")
                    {
                        if (xm != obj[2].ToString().Split('|')[3])
                        {
                            return new object[] { 0, 0, "医保提示：不是本人卡" };
                        }
                    }
                    else
                    {
                        return new object[] { 0, 0, "医保提示：读卡错误：" + obj[2].ToString() };
                    }
                }

                strSql = string.Format("select jzlsh from ybfyjsdr a where a.jzlsh = '{0}' and a.djhin = '{1}' and a.cxbz = 1", jzlsh, djhin);//0秒
                ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);

                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    return new object[] { 0, 0, "医保提示：his就诊流水号：" + jzlsh + "，单据号：" + djhin + "已结算" };
                }

                string rc = ybjzlsh + "|" + djhin + "|" + cfsj + "|" + jbr + "|" + bzbm + "|" + bzmc + "|" + yllb + "|" + Gocent + "|" + grbh + "|" + xm + "|" + kh;

                if (ZXBM != "0000")
                {
                    strSql = string.Format(@"select case y.ybxmbh when '39199999990001000001' then 'Z-00000-00000' when '61048000000500000000' then '61048000000400000000' else y.ybxmbh end ybxmbh, case y.ybxmbh when '39199999990001000001' then '医保不予支付的中成药费用' when '61048000000500000000' then '人工煎药' else y.ybxmmc end ybxmmc, m.dj, sum(m.sl) sl, sum(m.je) je, m.yyxmbh, m.yyxmmc, m.ysdm, n.b1name ysxm, m.ksno, o.b2ejnm zxks, m.sfno, y.sfxmzldm ybsfxmzldm, y.sflbdm ybsflbdm, m.cfh from 
                        (
                            --药品
                            select a.mcypno yyxmbh, a.mcypnm yyxmmc, a.mcpric dj, a.mcquty sl, a.mcamnt je, a.mcksno ksno, a.mcuser ysdm, a.mcsflb sfno, a.mccfno cfh
                            from mzcfd a 
                            where a.mcghno = '{0}' and a.mccfno in ({1})
                            union all
                            --检查/治疗
                            select b.mbitem yyxmbh, b.mbname yyxmmc, b.mbpric dj, b.mbquty sl, b.mbsjam je, b.mbksno ksno, b.mbuser ysdm, b.mbsfno sfno , b.mbzlno cfh          
                            from mzb2d b 
                            where b.mbghno = '{0}' and b.mbzlno in ({1})
                            union all
                            --检验
                            select c.mbitem yyxmbh, c.mbname yyxmmc, c.mbpric dj, c.mbquty sl, c.mbsjam je, c.mbksno ksno, c.mbuser ysdm, c.mbsfno sfno, c.mbzlno cfh
                            from mzb4d c 
                            where c.mbghno = '{0}' and c.mbzlno in ({1})
                            union all
                            --注射
                            select b5item yyxmbh, b5name yyxmmc, b5sfam dj, mddays sl, b5sfam * mddays je, mdzsks ksno, mdempn ysdm, b5sfno sfno, mdzsno cfh
                            from mzd3d
                            left join (select distinct mccfno, mcghno from mzcfd) mzcf on mccfno = mdcfno
                            left join bz09d on b9mbno = mdtwid 
                            left join bz05d on b5item = b9item where mdtiwe > 0 and mdzsno in ({1})
                            union all
                            select b5item yyxmbh, b5name yyxmmc, b5sfam dj, mdtims sl, b5sfam * mdtims je, mdzsks ksno, mdempn ysdm, b5sfno sfno, mdzsno cfh
                            from mzd3d 
                            left join bz09d on b9mbno = mdwayid 
                            left join bz05d on b5item = b9item
                            left join (select distinct mccfno, mcghno from mzcfd) mzcf on mccfno = mdcfno 
                            where mdzsno in ({1})
                            union all
                            select b5item yyxmbh, b5name yyxmmc, b5sfam dj, mdpqty sl, b5sfam * mdpqty je, mdzsks ksno, mdempn ysdm, b5sfno sfno, mdzsno cfh
                            from mzd3d 
                            left join bz09d on b9mbno = mdpprid 
                            left join bz05d on b5item = b9item
                            left join (select distinct mccfno, mcghno from mzcfd) mzcf on mccfno = mdcfno
                            where mdpqty > 0 and mdzsno in ({1})
                            union all
                            --处方划价
                            select a.ygypno yyxmbh, a.ygypnm yyxmmc, ((a.ygamnt + 0.0) / a.ygslxx) dj, a.ygslxx sl, a.ygamnt je, b.ygksno ksno, b.ygysno ysdm, c.y1sflb, a.ygshno cfh
                            from yp17d a 
                            join yp17h b on a.ygcomp = b.ygcomp and a.ygshno = b.ygshno
                            join yp01h c on c.y1ypno = a.ygypno
                            where b.ygghno = '{0}' and a.ygshno in ({1}) and a.ygslxx > 0
                        ) m 
                        left join bz01h n on m.ysdm = n.b1empn 
                        left join bz02d o on m.ksno = o.b2ejks
                        left join ybhisdzdr y on m.yyxmbh = y.hisxmbh and y.scbz = 1
                        group by case y.ybxmbh when '39199999990001000001' then 'Z-00000-00000' when '61048000000500000000' then '61048000000400000000' else y.ybxmbh end, case y.ybxmbh when '39199999990001000001' then '医保不予支付的中成药费用' when '61048000000500000000' then '人工煎药' else y.ybxmmc end, m.dj, m.yyxmbh, m.yyxmmc, m.ysdm, n.b1name, m.ksno, o.b2ejnm, m.sfno, y.sfxmzldm, y.sflbdm, m.cfh"
                                        , jzlsh, cfhs);//3秒
                }
                else
                {
                    strSql = string.Format(@"select y.ybxmbh, y.ybxmmc, m.dj, sum(m.sl) sl, sum(m.je) je, m.yyxmbh, m.yyxmmc, m.ysdm, n.b1name ysxm, m.ksno, o.b2ejnm zxks, m.sfno, y.sfxmzldm ybsfxmzldm, y.sflbdm ybsflbdm, m.cfh from 
                        (
                            --药品
                            select a.mcypno yyxmbh, a.mcypnm yyxmmc, a.mcpric dj, a.mcquty sl, a.mcamnt je, a.mcksno ksno, a.mcuser ysdm, a.mcsflb sfno, a.mccfno cfh
                            from mzcfd a 
                            where a.mcghno = '{0}' and a.mccfno in ({1})
                            union all
                            --检查/治疗
                            select b.mbitem yyxmbh, b.mbname yyxmmc, b.mbpric dj, b.mbquty sl, b.mbsjam je, b.mbksno ksno, b.mbuser ysdm, b.mbsfno sfno , b.mbzlno cfh          
                            from mzb2d b 
                            where b.mbghno = '{0}' and b.mbzlno in ({1})
                            union all
                            --检验
                            select c.mbitem yyxmbh, c.mbname yyxmmc, c.mbpric dj, c.mbquty sl, c.mbsjam je, c.mbksno ksno, c.mbuser ysdm, c.mbsfno sfno, c.mbzlno cfh
                            from mzb4d c 
                            where c.mbghno = '{0}' and c.mbzlno in ({1})
                            union all
                            --注射
                            select b5item yyxmbh, b5name yyxmmc, b5sfam dj, mddays sl, b5sfam * mddays je, mdzsks ksno, mdempn ysdm, b5sfno sfno, mdzsno cfh
                            from mzd3d
                            left join (select distinct mccfno, mcghno from mzcfd) mzcf on mccfno = mdcfno
                            left join bz09d on b9mbno = mdtwid 
                            left join bz05d on b5item = b9item where mdtiwe > 0 and mdzsno in ({1})
                            union all
                            select b5item yyxmbh, b5name yyxmmc, b5sfam dj, mdtims sl, b5sfam * mdtims je, mdzsks ksno, mdempn ysdm, b5sfno sfno, mdzsno cfh
                            from mzd3d 
                            left join bz09d on b9mbno = mdwayid 
                            left join bz05d on b5item = b9item
                            left join (select distinct mccfno, mcghno from mzcfd) mzcf on mccfno = mdcfno 
                            where mdzsno in ({1})
                            union all
                            select b5item yyxmbh, b5name yyxmmc, b5sfam dj, mdpqty sl, b5sfam * mdpqty je, mdzsks ksno, mdempn ysdm, b5sfno sfno, mdzsno cfh
                            from mzd3d 
                            left join bz09d on b9mbno = mdpprid 
                            left join bz05d on b5item = b9item
                            left join (select distinct mccfno, mcghno from mzcfd) mzcf on mccfno = mdcfno
                            where mdpqty > 0 and mdzsno in ({1})
                            union all
                            --处方划价
                            select a.ygypno yyxmbh, a.ygypnm yyxmmc, ((a.ygamnt + 0.0) / a.ygslxx) dj, a.ygslxx sl, a.ygamnt je, b.ygksno ksno, b.ygysno ysdm, c.y1sflb, a.ygshno cfh
                            from yp17d a 
                            join yp17h b on a.ygcomp = b.ygcomp and a.ygshno = b.ygshno
                            join yp01h c on c.y1ypno = a.ygypno
                            where b.ygghno = '{0}' and a.ygshno in ({1}) and a.ygslxx > 0
                        ) m 
                        left join bz01h n on m.ysdm = n.b1empn 
                        left join bz02d o on m.ksno = o.b2ejks
                        left join ybhisdzdr y on m.yyxmbh = y.hisxmbh and y.scbz = 1
                        group by y.ybxmbh, y.ybxmmc, m.dj, m.yyxmbh, m.yyxmmc, m.ysdm, n.b1name, m.ksno, o.b2ejnm, m.sfno, y.sfxmzldm, y.sflbdm, m.cfh"
                                        , jzlsh, cfhs);//3秒
                }

                ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                List<string> lizysfdj = new List<string>();

                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    DataTable dt = ds.Tables[0];
                    StringBuilder wdzxms = new StringBuilder();

                    for (int k = 0; k < dt.Rows.Count; k++)
                    {
                        DataRow dr = dt.Rows[k];

                        if (dr["ybxmbh"] == DBNull.Value)
                        {
                            wdzxms.Append("医保提示：项目代码：[" + dr["yyxmbh"].ToString() + "]，名称：[" + dr["yyxmmc"].ToString() + "]未对照或未上传，不能上传费用");
                        }
                        else if (dr["ksno"] == DBNull.Value || dr["ksno"].ToString() == "")
                        {
                            return new object[] { 0, 0, "医保提示：有科室编号为空的记录" };
                        }
                        else if (dr["zxks"] == DBNull.Value || dr["zxks"].ToString() == "")
                        {
                            return new object[] { 0, 0, "医保提示：无此科室编号[" + dr["ksno"].ToString() + "]" };
                        }
                        else
                        {
                            string ybsfxmzldm = dr["ybsfxmzldm"].ToString();
                            string ybsflbdm = dr["ybsflbdm"].ToString();
                            string yyxmbh = dr["yyxmbh"].ToString();
                            string ybxmbh = dr["ybxmbh"].ToString();
                            string yyxmmc = dr["yyxmmc"].ToString();
                            decimal dj = Convert.ToDecimal(dr["dj"]);
                            decimal sl = Convert.ToDecimal(dr["sl"]);
                            decimal je = Convert.ToDecimal(dr["je"]);
                            decimal mcyl = 1;
                            string ysbm = string.IsNullOrWhiteSpace(dr["ysdm"].ToString()) ? "010001" : dr["ysdm"].ToString();
                            string ysxm = string.IsNullOrWhiteSpace(dr["ysxm"].ToString()) ? "严东标" : dr["ysxm"].ToString();
                            string ksdm = dr["ksno"].ToString();
                            string ksmc = dr["zxks"].ToString();
                            string ybcfh = cfsj + k.ToString();
                            string ypjldw = "-";
                            string cydffbz = "0";

                            if (ybsfxmzldm == "1")
                            {
                                cydffbz = "1";
                            }

                            if (dr["sfno"].ToString() == "01" || dr["sfno"].ToString() == "02" || dr["sfno"].ToString() == "03")
                            {
                                ypjldw = "粒";
                            }

                            rc += "$" + ybjzlsh + "|" + ybsfxmzldm + "|" + ybsflbdm + "|" + ybcfh + "|" + cfsj
                            + "|" + yyxmbh + "|" + ybxmbh + "|" + yyxmmc + "|" + Math.Round(dj, 4).ToString() + "|" + Math.Round(sl, 2).ToString() + "|"
                            + Math.Round(je, 4).ToString() + "|101||" + mcyl.ToString() + "||" + ysbm + "|" + ysxm + "|||" + ksdm + "|"
                            + ksmc + "||" + cydffbz + "|" + jbr + "|" + ypjldw + "||";
                            string strSql1 = string.Format(@"insert into ybcfmxscindr(jzlsh, jylsh, sfxmzl, sflb, ybcfh, cfrq, yysfxmbm, sfxmzxbm
                            , yysfxmmc, dj, sl, je, mcyl, ysbm, ysxm, ksbh, ksmc, cydffbz, jbr, ypjldw, grbh, xm, kh)
                            values ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', {9}, {10}, {11}, {12}, '{13}', '{14}'
                            , '{15}', '{16}', '{17}', '{18}', '{19}', '{20}', '{21}', '{22}')"
                            , jzlsh, jylsh, ybsfxmzldm, ybsflbdm, ybcfh, cfsj, yyxmbh, ybxmbh, yyxmmc, Math.Round(dj, 4).ToString(), sl
                            , Math.Round(je, 4).ToString(), mcyl, ysbm, ysxm, ksdm, ksmc, cydffbz, jbr, ypjldw, grbh, xm, kh);
                            lizysfdj.Add(strSql1);
                        }
                    }

                    if (wdzxms.Length > 0)
                    {
                        return new object[] { 0, 0, "医保提示：" + wdzxms };
                    }
                }
                else
                {
                    return new object[] { 0, 0, "医保提示：his无费用明细" };
                }

                StringBuilder inputData = new StringBuilder(string.Format("{0}^{1}^{2}^{3}^{4}^{5}^{6}^{7}^", "2620", DDYLJGBH, czygh, ywzqh, jylsh, ZXBM, rc, "1"));
                StringBuilder outputData = new StringBuilder(100000);
                int i = BUSINESS_HANDLE(inputData, outputData);

                if (i == 0)
                {
                    isybjsmzyd = true;
                    string fjsfh = outputData.ToString().Split('^')[2];
                    string[] sfjsfh = fjsfh.Split('|');
                    string ylfze = sfjsfh[0]; //医疗费总额
                    string zbxje = sfjsfh[1]; //总报销金额
                    string tcjjzf = sfjsfh[2]; //统筹基金支付
                    string dejjzf = sfjsfh[3]; //大额基金支付
                    string zhzf = sfjsfh[4]; //账户支付
                    string xjzf = sfjsfh[5]; //现金支付
                    string gwybzjjzf = sfjsfh[6]; //公务员补助基金支付
                    string qybcylbxjjzf = sfjsfh[7]; //企业补充医疗保险基金支付
                    string zffy = sfjsfh[8]; //自费费用
                    string dwfdfy = sfjsfh[9]; //单位负担费用
                    string yyfdfy = sfjsfh[10]; //医院负担费用
                    string mzjzfy = sfjsfh[11]; //民政救助费用
                    string cxjfy = sfjsfh[12]; //超限价费用
                    string ylzlfy = sfjsfh[13]; //乙类自理费用
                    string blzlfy = sfjsfh[14]; //丙类自理费用
                    string fhjbylfy = sfjsfh[15]; //符合基本医疗费用
                    string qfbzfy = sfjsfh[16]; //起付标准费用
                    string zzzyzffy = sfjsfh[17]; //转诊转院自付费用
                    string jrtcfy = sfjsfh[18]; //进入统筹费用
                    string tcfdzffy = sfjsfh[19]; //统筹分段自付费用
                    string ctcfdxfy = sfjsfh[20]; //超统筹封顶线费用
                    string jrdebxfy = sfjsfh[21]; //进入大额报销费用
                    string defdzffy = sfjsfh[22]; //大额分段自付费用
                    string cdefdxfy = sfjsfh[23]; //超大额封顶线费用
                    string rgqgzffy = sfjsfh[24]; //人工器官自付费用
                    string bcjsqzhye = sfjsfh[25]; //本次结算前帐户余额
                    string bntczflj = sfjsfh[26]; //本年统筹支付累计(不含本次)
                    string bndezflj = sfjsfh[27]; //本年大额支付累计(不含本次)
                    string bnczjmmztczflj = sfjsfh[28]; //本年城镇居民门诊统筹支付累计(不含本次)
                    string bngwybzzflj = sfjsfh[29]; //本年公务员补助支付累计(不含本次)
                    string bnzhzflj = sfjsfh[30]; //本年账户支付累计(不含本次) 
                    string bnzycslj = sfjsfh[31]; //本年住院次数累计(不含本次)
                    string zycs = sfjsfh[32]; //住院次数
                    string jsrq1 = sfjsfh[34]; //结算时间
                    string yllb1 = sfjsfh[35]; //医疗类别
                    string yldylb = sfjsfh[36]; //医疗待遇类别
                    string jbjgbm = sfjsfh[37]; //经办机构编码
                    string ywzqh1 = sfjsfh[38]; //业务周期号
                    string jslsh = sfjsfh[39]; //结算流水号
                    string tsxx = sfjsfh[40]; //提示信息
                    djh = sfjsfh[41]; //单据号
                    string jylx = sfjsfh[42]; //交易类型
                    string yyjylsh = sfjsfh[43]; //医院交易流水号
                    string yxbz = sfjsfh[44]; //有效标志
                    string grbhgl = sfjsfh[45]; //个人编号管理
                    string yljgbm = sfjsfh[46]; //医疗机构编码

                    string jjjmzcfwwkbxfy = sfjsfh[63]; //九江居民政策范围外可报销费用
                    string jmdbydje = sfjsfh[58]; //居民大病一段金额
                    string jmdbedje = sfjsfh[59]; //居民大病二段金额
                    string jbbcfwnzfje = sfjsfh[60]; //疾病补充范围内费用支付金额
                    string jbbcbxbczcfwwfyzfje = sfjsfh[61]; //疾病补充保险本次政策范围外费用支付金额
                    string bnzfddjjfylj = sfjsfh[74]; //本年政府兜底基金费用累计
                    string zfddjjfy = sfjsfh[64]; //政府兜底基金费用

                    if (Math.Abs(hisylfze - Convert.ToDecimal(ylfze)) >= 1)
                    {
                        i = YBFYJSSDCXDR(ybjzlsh, djh, cfsj, grbh, xm, kh, ZXBM);

                        if (i == 1)
                        {
                            return new object[] { 0, 0, "医保提示：his就诊流水号：" + jzlsh + "his总金额" + hisylfze + "与医保总金额" + ylfze + "不相同，已自动冲销，请重新结算" };
                        }
                        else
                        {
                            return new object[] { 0, 0, "医保提示：his就诊流水号：" + jzlsh + "his总金额" + hisylfze + "与医保总金额" + ylfze + "不相同，自动冲销失败，请手工冲销" };
                        }
                    }

                    string strSql1 = string.Format(@"insert into ybfyjsdr(jzlsh, jylsh, djhin, bzbm, bzmc, jbr, kfsbz, ylfze, zbxje, tcjjzf
                    , dejjzf, zhzf, xjzf, gwybzjjzf, qybcylbxjjzf, zffy, dwfdfy, yyfdfy, mzjzfy, cxjfy, ylzlfy, blzlfy, fhjbylfy, qfbzfy, zzzyzffy
                    , jrtcfy, tcfdzffy, ctcfdxfy, jrdebxfy, defdzffy, cdefdxfy, rgqgzffy, bcjsqzhye, bntczflj, bndezflj, bnczjmmztczflj, bngwybzzflj, bnzhzflj
                    , bnzycslj, zycs, xm, jsrq, yllb, yldylb, jbjgbm, ywzqh, jslsh, tsxx, djh, jylx, yyjylsh, yxbz, grbhgl, yljgbm, grbh, kh, ybjzlsh
                    , jjjmzcfwwkbxfy,jmdbydje,jmdbedje,jbbcfwnzfje,jbbcbxbczcfwwfyzfje,bnzfddjjfylj,zfddjjfy) 
                    values('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}'
                    , '{15}', '{16}', '{17}', '{18}', '{19}', '{20}', '{21}', '{22}', '{23}', '{24}', '{25}', '{26}', '{27}', '{28}'
                    , '{29}', '{30}', '{31}', '{32}', '{33}', '{34}', '{35}', '{36}', '{37}', '{38}', '{39}', '{40}', '{41}', '{42}'
                    , '{43}', '{44}', '{45}', '{46}', '{47}', '{48}', '{49}', '{50}', '{51}', '{52}', '{53}', '{54}', '{55}', '{56}'
                    , '{57}', '{58}', '{59}', '{60}', '{61}', '{62}', '{63}')"
                    , jzlsh, jylsh, djhin, bzbm, bzmc, jbr, Gocent, ylfze, zbxje, tcjjzf, dejjzf, zhzf, xjzf, gwybzjjzf, qybcylbxjjzf
                    , zffy, dwfdfy, yyfdfy, mzjzfy, cxjfy, ylzlfy, blzlfy, fhjbylfy, qfbzfy, zzzyzffy, jrtcfy, tcfdzffy, ctcfdxfy
                    , jrdebxfy, defdzffy, cdefdxfy, rgqgzffy, bcjsqzhye, bntczflj, bndezflj, bnczjmmztczflj, bngwybzzflj, bnzhzflj
                    , bnzycslj, zycs, xm, cfsj, yllb, yldylb, jbjgbm, ywzqh, jslsh, tsxx, djh, jylx, yyjylsh, yxbz, grbhgl, yljgbm, grbh, kh, ybjzlsh
                    ,  jjjmzcfwwkbxfy,jmdbydje,jmdbedje,jbbcfwnzfje,jbbcbxbczcfwwfyzfje,bnzfddjjfylj,zfddjjfy);
                    string strSql3 = string.Format("update ybcfmxscindr set jsdjh = '{0}' where jzlsh = '{1}' and jsdjh is null and cxbz = 1", djhin, jzlsh);//0秒
                    string strSql4 = string.Format("delete from ybcfmxscindr where jzlsh = '{0}' and jsdjh is null and cxbz in (0, 2)", jzlsh);//0秒
                    lizysfdj.Add(strSql1);
                    lizysfdj.Add(strSql3);
                    lizysfdj.Add(strSql4);
                    obj = lizysfdj.ToArray();
                    obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);

                    if (obj[1].ToString() == "1")
                    {
                        return new object[] { 0, 1, outputData.ToString().Split('^')[2] + "0|0|0|0|0|0|0|0|0|0|0|0|" };
                    }
                    else
                    {
                        Common.WriteYBLog(obj[2].ToString());
                        i = YBFYJSSDCXDR(ybjzlsh, djh, cfsj, grbh, xm, kh, ZXBM);

                        if (i == 1)
                        {
                            return new object[] { 0, 0, "医保提示：his数据库操作失败,自动撤销医保结算成功,错误信息：" + obj[2].ToString() };
                        }
                        else
                        {
                            return new object[] { 0, 0, "医保提示：his数据库操作失败,自动撤销医保结算失败,错误信息：" + obj[2].ToString() };
                        }
                    }
                }
                else if (i == -1)
                {
                    return new object[] { 0, -1, "医保提示：医保门诊异地结算系统级别错误，但未返回结算单据号，无法撤销，" + outputData.ToString() };
                }
                else
                {
                    return new object[] { 0, -2, "医保提示：医保门诊异地结算业务级别或未知错误，" + outputData.ToString() };
                }
            }
            catch (Exception error)
            {
                Common.WriteYBLog(error.ToString());

                if (isybjsmzyd)
                {
                    int i = YBFYJSSDCXDR(ybjzlsh, djh, cfsj, grbh, xm, kh, ZXBM);

                    if (i == 1)
                    {
                        return new object[] { 0, 2, "医保提示：非医保异常，自动撤销医保结算成功，" + error.ToString() };
                    }
                    else
                    {
                        return new object[] { 0, 2, "医保提示：非医保异常，自动撤销医保结算失败，" + error.ToString() };
                    }
                }

                return new object[] { 0, 2, error.ToString() };
            }
        }
        #endregion 门诊结算异地

        #region 住院登记
        /// <summary>
        /// 住院登记
        /// </summary>
        /// <param>his就诊流水号,医疗类别代码,病种编码,病种名称,卡信息(读卡返回的一串个人信息), 来源机构代码，来源机构名称, 医疗类别名称</param>
        /// <returns>1:成功，0:不成功，2:报错</returns>
        public static object[] YBZYDJDR(object[] objParam)
        {
            bool isybzydj = false;
            string ybjzlsh = "";
            string grbh = "";
            string xm = "";
            string kh = "";

            try
            {
                string czygh = CliUtils.fLoginUser;
                string ywzqh = YWZQH;
                string jbr = CliUtils.fUserName;

                if (jbr == "siinterface")
                {
                    return new object[] { 0, 0, "医保提示：入院登记出现经办人为siinterface的,his就诊流水号为：" + objParam[0].ToString() + "，请通知医保工程师" };
                }

                string jzlsh = objParam[0].ToString(); //his就诊流水号
                string yllb = objParam[1].ToString(); //医疗类别代码
                string bzbm = objParam[2].ToString(); //病种编码
                string bzmc = objParam[3].ToString(); //病种名称
                string[] kxx = objParam[4].ToString().Split('|');
                grbh = kxx[0].Split('^')[2].ToString();//个人编号
                string dwbh = kxx[1].ToString(); //单位编号
                string sfzh = kxx[2].ToString(); //身份证号
                xm = kxx[3].ToString(); //姓名
                string xb = kxx[4].ToString(); //性别
                string mz = kxx[5].ToString(); //民族
                string csrq = kxx[6].ToString(); //出生日期
                kh = kxx[7].ToString(); //卡号
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
                string lyjgdm = objParam[5].ToString();//来源机构代码
                string lyjgmc = objParam[6].ToString();//来源机构名称
                string yllbmc = objParam[7].ToString();//医疗类别名称
                DateTime dqsj = Convert.ToDateTime(Common.GetServerTime());
                ybjzlsh = jzlsh + dqsj.ToString("HHmmss");
                object[] obj = null;
                string fsxx = "";
                string dkydrybz = ydrybz;

                if (yldylb == "99")
                {
                    //return new object[] { 0, 0, "此人待遇类别为99其他，请联系参保地备案" };
                    MessageBox.Show("医保提示：此人待遇类别为99其他", "提示");
                }

                if (ydrybz != "0")
                {
                    string strSqlyd = string.Format(@"select bzmem1 from bztbd where bzcodn = 'DQ' and bzkeyx = '{0}' and bzmem1 = '0'", tcqh);
                    DataSet dsyd = CliUtils.ExecuteSql("sybdj", "cmd", strSqlyd, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);

                    if (dsyd != null && dsyd.Tables[0].Rows.Count > 0)
                    {
                        ydrybz = "0";
                    }
                    else
                    {
                        if (MessageBox.Show("你确定该患者走异地报销吗？", "确认", MessageBoxButtons.YesNo) == DialogResult.No)
                        {
                            ydrybz = "0";
                        }
                    }
                }

                ZXBM = ydrybz == "0" ? "0000" : tcqh;

                if (ydrybz == "0")
                {
                    obj = new object[] { grbh, kh };
                    obj = YBYLDYFSXXCXDR(obj);

                    if (obj[1].ToString() == "-2")//错误
                    {
                        MessageBox.Show("医保提示：封锁信息查询异常", "提示");
                        return obj;
                    }
                    else if (obj[1].ToString() != "1")//封锁
                    {
                        if (DialogResult.Cancel == MessageBox.Show("医保提示：这张卡已被封锁，信息：" + obj[2].ToString() + ",是否继续入院登记", "提示", MessageBoxButtons.OKCancel))
                        {
                            return obj;
                        }
                    }

                    fsxx = obj[2].ToString();
                }

                if (string.IsNullOrWhiteSpace(jzlsh))
                {
                    return new object[] { 0, 0, "医保提示：his就诊流水号为空" };
                }
                else if (string.IsNullOrWhiteSpace(yllb))
                {
                    return new object[] { 0, 0, "医保提示：医疗类别代码为空" };
                }
                else if (string.IsNullOrWhiteSpace(bzbm))
                {
                    return new object[] { 0, 0, "医保提示：入院病种编码为空" };
                }
                else if (string.IsNullOrWhiteSpace(bzmc))
                {
                    return new object[] { 0, 0, "医保提示：入院病种名称为空" };
                }

                string strSql = string.Format("select * from zy03dw a where a.z3zyno = '{0}' and left(a.z3endv, 1) = '1'", jzlsh);//0秒
                DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);

                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    return new object[] { 0, 0, "医保提示：his就诊流水号" + jzlsh + "已进行结算" };
                }

                strSql = string.Format("select jzlsh from ybmzzydjdr a where a.jzlsh = '{0}' and a.cxbz = 1 and a.jzbz = 'z'", jzlsh);//0秒
                ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);

                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    return new object[] { 0, 0, "医保提示：his就诊流水号" + jzlsh + "已登记医保入院" };
                }

                strSql = string.Format(@"select a.z1date rysj, a.z1hznm hzxm, a.z1ksno, a.z1ksnm, a.z1empn z1ysno, a.z1mzys z1ysnm from zy01h a where a.z1zyno = '{0}'", jzlsh);//0秒
                ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);

                if (ds == null || ds.Tables[0].Rows.Count == 0)
                {
                    return new object[] { 0, 0, "医保提示：his就诊流水号" + jzlsh + "未登记his入院" };
                }

                DataRow dr = ds.Tables[0].Rows[0];
                string ksbh = dr["z1ksno"].ToString(); //科室编号
                string ksmc = dr["z1ksnm"].ToString(); //科室名称
                string ysdm = dr["z1ysno"].ToString(); //医师代码
                string ysxm = dr["z1ysnm"].ToString(); //医师姓名
                string ghdjsj = Convert.ToDateTime(dr["rysj"]).ToString("yyyyMMddHHmmss");
                string bedno = "";
                string hzxm = dr["hzxm"].ToString();

                if (string.IsNullOrWhiteSpace(ysdm) || string.IsNullOrWhiteSpace(ysxm))
                {
                    ysdm = "010001";
                    ysxm = "严东标";
                }

                if (!string.Equals(hzxm, xm))
                {
                    return new object[] { 0, 0, "医保提示：his就诊流水号" + jzlsh + "住院登记姓名和医保卡姓名不相符" };
                }

                string jylsh = dqsj.ToString("yyyyMMddHHmmss") + "-" + DDYLJGBH + "-" + jzlsh;
                string rc = ybjzlsh + "|" + yllb + "|" + ghdjsj + "|" + bzbm + "|" + bzmc + "|" + ksbh + "|" + ksmc + "|" + bedno + "|"
                + ysdm + "|" + ysxm + "|" + jbr + "|" + grbh + "||||||" + xm + "|" + kh + "|||";
                StringBuilder inputData = new StringBuilder(string.Format("{0}^{1}^{2}^{3}^{4}^{5}^{6}^{7}^", "2210", DDYLJGBH, czygh, ywzqh, jylsh, ZXBM, rc, "1"));
                StringBuilder outputData = new StringBuilder(100000);
                int i = BUSINESS_HANDLE(inputData, outputData);

                if (i == 0)
                {
                    isybzydj = true;

                    if (ydrybz != "0")
                    {
                        ybjzlsh = outputData.ToString().Split('^')[2].Split('|')[0];
                    }

                    strSql = string.Format(@"insert into ybmzzydjdr(jzlsh, jylsh, dwbh, sfzh, xb, mz, csrq, yldylb, rycbzt, ydrybz, tcqh, nd
                    , zyzt, zhye, bnylflj, bnzhzclj, bntczclj, bnjzjzclj, bngwybzjjlj, bnczjmmztczflj, jrtcfylj, jrjzjfylj, qfbzlj, bnzycs, dwmc
                    , nl, cbdwlx, jbjgbm, yllb, ghdjsj, bzbm, bzmc, ksbh, ksmc, ysdm, ysxm, jbr, grbh, xm, kh, ybjzlsh, jzbz, fsxx, dkydrybz, kxx) 
                    values('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', {13}, {14}, {15}, {16}
                    , {17}, {18}, {19}, {20}, {21}, {22}, '{23}', '{24}', '{25}', '{26}', '{27}', '{28}', '{29}', '{30}', '{31}', '{32}', '{33}'
                    , '{34}', '{35}', '{36}', '{37}', '{38}', '{39}', '{40}', '{41}', '{42}', '{43}', '{44}')"
                    , jzlsh, jylsh, dwbh, sfzh, xb, mz, csrq, yldylb, rycbzt, ydrybz, tcqh, nd, zyzt, zhye, bnylflj, bnzhzclj, bntczclj, bnjzjzclj
                    , bngwybzjjlj, bnczjmmztczflj, jrtcfylj, jrjzjfylj, qfbzlj, bnzycs, dwmc, nl, cbdwlx, jbjgbm, yllb, ghdjsj, bzbm, bzmc
                    , ksbh, ksmc, ysdm, ysxm, jbr, grbh, xm, kh, ybjzlsh, "z", fsxx, dkydrybz, objParam[4].ToString());
                    string strSql1 = string.Format(@"update zy01h set z1rylb = '{0}', z1tcdq = '{1}', z1lyjg = '{2}', z1lynm = '{3}', z1ylno = '{4}'
                    , z1ylnm = '{5}', z1bzno = '{6}', z1bznm = '{7}', z1ybno = '{8}' where z1comp = '{9}' and z1zyno = '{10}'"
                    , yldylb, tcqh, lyjgdm, lyjgmc, yllb, yllbmc, bzbm, bzmc, grbh, CliUtils.fSiteCode, jzlsh);//0秒
                    obj = new object[] { strSql, strSql1 };
                    obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);

                    if (obj[1].ToString() == "1")
                    {
                        return new object[] { 0, 1, outputData };
                    }
                    else
                    {
                        Common.InsertYBLog(jzlsh, "", obj[2].ToString());
                        i = YBDJSDCXDR(ybjzlsh, grbh, xm, kh, ZXBM);

                        if (i == 1)
                        {
                            return new object[] { 0, 0, "医保提示：his数据库操作失败,自动撤销医保入院登记成功,错误信息：" + obj[2].ToString() };
                        }
                        else
                        {
                            return new object[] { 0, 0, "医保提示：his数据库操作失败,自动撤销医保入院登记失败,错误信息：" + obj[2].ToString() };
                        }
                    }
                }
                else if (i == -1)
                {
                    Common.InsertYBLog(jzlsh, inputData.ToString(), outputData.ToString());
                    i = YBDJSDCXDR(ybjzlsh, grbh, xm, kh, ZXBM);//如果是异地，ybjzlsh可能还没返回，撤销失败

                    if (i == 1)
                    {
                        return new object[] { 0, -1, "医保提示：医保入院登记系统级别错误，自动撤销医保入院登记成功，" + outputData.ToString() };
                    }
                    else
                    {
                        return new object[] { 0, -1, "医保提示：医保入院登记系统级别错误，自动撤销医保入院登记失败，" + outputData.ToString() };
                    }
                }
                else if (i == -2)
                {
                    Common.InsertYBLog(jzlsh, inputData.ToString(), outputData.ToString());

                    if (outputData.ToString().Contains("该人已经产生15"))
                    {
                        object[] objlxzy = YBYYSPXXSBDR(new object[]{grbh, "23"});

                        if (objlxzy[1].ToString() == "1")
                        {
                            outputData.Append("；已上传连续住院申请：" + objlxzy[1].ToString() + "；" + objlxzy[2].ToString());
                        }
                        else
                        {
                            outputData.Append("；上传连续住院申请失败：" + objlxzy[1].ToString() + "；" + objlxzy[2].ToString());
                        }
                    }

                    return new object[] { 0, -2, "医保提示：医保入院登记业务级别错误，" + outputData.ToString() };
                }
                else
                {
                    Common.InsertYBLog(jzlsh, inputData.ToString(), outputData.ToString());
                    return new object[] { 0, -3, "医保提示：医保入院登记未知错误，" + outputData.ToString() };
                }
            }
            catch (Exception error)
            {
                Common.InsertYBLog("", "", error.ToString());

                if (isybzydj)
                {
                    int i = YBDJSDCXDR(ybjzlsh, grbh, xm, kh, ZXBM);

                    if (i == 1)
                    {
                        return new object[] { 0, 2, "医保提示：非医保异常，自动撤销医保入院登记成功，" + error.ToString() };
                    }
                    else
                    {
                        return new object[] { 0, 2, "医保提示：非医保异常，自动撤销医保入院登记失败，" + error.ToString() };
                    }
                }

                return new object[] { 0, 2, "Error:" + error.ToString() };
            }
        }
        #endregion 住院登记

        #region 住院登记修改(暂不使用)
        ///// <summary>
        ///// 住院登记修改
        ///// </summary>
        ///// <param>就诊流水号,医疗类别代码</param>
        ///// <returns>1:成功，0:不成功，2:报错</returns>
        //public static object[] YBZYDJXGDR(object[] objParam)
        //{
        //    string ybjzlsh = objParam[0].ToString();
        //    string jbr = CliUtils.fUserName;
        //    string grbh = objParam[1].ToString();
        //    string xm = objParam[2].ToString();
        //    string kh = objParam[3].ToString();
        //    string yllb = objParam[4].ToString();
        //    try
        //    {
        //        string czygh = CliUtils.fLoginUser;
        //        string ywzqh = YWZQH;
        //        //string jbr = CliUtils.fUserName;
        //        //string jzlsh = objParam[0].ToString(); //就诊流水号
        //        //string yllb = objParam[1].ToString(); //医疗类别代码
        //        //string bzbm = objParam[2].ToString(); //病种编码
        //        //string bzmc = objParam[3].ToString(); //病种名称
        //        //string[] kxx = objParam[4].ToString().Split('|');
        //        //grbh = kxx[0].ToString(); //个人编号
        //        //string dwbh = kxx[1].ToString(); //单位编号
        //        //string sfzh = kxx[2].ToString(); //身份证号
        //        //xm = kxx[3].ToString(); //姓名
        //        //string xb = kxx[4].ToString(); //性别
        //        //string mz = kxx[5].ToString(); //民族
        //        //string csrq = kxx[6].ToString(); //出生日期
        //        //kh = kxx[7].ToString(); //卡号
        //        //string yldylb = kxx[8].ToString(); //医疗待遇类别
        //        //string rycbzt = kxx[9].ToString(); //人员参保状态
        //        //string ydrybz = kxx[10].ToString(); //异地人员标志
        //        //string tcqh = kxx[11].ToString(); //统筹区号
        //        //string nd = kxx[12].ToString(); //年度
        //        //string zyzt = kxx[13].ToString(); //在院状态
        //        //string zhye = kxx[14].ToString(); //帐户余额
        //        //string bnylflj = kxx[15].ToString(); //本年医疗费累计
        //        //string bnzhzclj = kxx[16].ToString(); //本年帐户支出累计
        //        //string bntczclj = kxx[17].ToString(); //本年统筹支出累计
        //        //string bnjzjzclj = kxx[18].ToString(); //本年救助金支出累计
        //        //string bngwybzjjlj = kxx[19].ToString(); //本年公务员补助基金累计
        //        //string bnczjmmztczflj = kxx[20].ToString(); //本年城镇居民门诊统筹支付累计
        //        //string jrtcfylj = kxx[21].ToString(); //进入统筹费用累计
        //        //string jrjzjfylj = kxx[22].ToString(); //进入救助金费用累计
        //        //string qfbzlj = kxx[23].ToString(); //起付标准累计
        //        //string bnzycs = kxx[24].ToString(); //本年住院次数
        //        //string dwmc = kxx[25].ToString(); //单位名称
        //        //string nl = kxx[26].ToString(); //年龄
        //        //string cbdwlx = kxx[27].ToString(); //参保单位类型
        //        //string jbjgbm = kxx[28].ToString(); //经办机构编码
        //        //string lyjgdm = objParam[5].ToString();//来源机构代码
        //        //string lyjgmc = objParam[6].ToString();//来源机构名称
        //        //string yllbmc = objParam[7].ToString();//医疗类别名称
        //        //ybjzlsh = jzlsh + DateTime.Now.ToString("HHmmss");
        //        //object[] obj = null;
        //        //string fsxx = "";
        //        //ZXBM = ydrybz == "1" ? tcqh : "0000";

        //        //if (ydrybz == "0")
        //        //{
        //        //    obj = new object[] { grbh, kh };
        //        //    obj = YBYLDYFSXXCXDR(obj);

        //        //    if (obj[1].ToString() == "-2")//错误
        //        //    {
        //        //        MessageBox.Show("医保提示：封锁信息查询异常", "提示");
        //        //        return obj;
        //        //    }
        //        //    else if (obj[1].ToString() != "1")//封锁
        //        //    {
        //        //        if (DialogResult.Cancel == MessageBox.Show("医保提示：这张卡已被封锁，信息：" + obj[2].ToString() + ",是否继续入院登记", "提示", MessageBoxButtons.OKCancel))
        //        //        {
        //        //            return obj;
        //        //        }
        //        //    }
        //        //    else
        //        //    {
        //        //        fsxx = obj[2].ToString();
        //        //    }
        //        //}

        //        //if (string.IsNullOrWhiteSpace(jzlsh))
        //        //{
        //        //    return new object[] { 0, 0, "医保提示：his就诊流水号为空" };
        //        //}
        //        //else if (string.IsNullOrWhiteSpace(yllb))
        //        //{
        //        //    return new object[] { 0, 0, "医保提示：医疗类别代码为空" };
        //        //}
        //        //else if (string.IsNullOrWhiteSpace(bzbm))
        //        //{
        //        //    return new object[] { 0, 0, "医保提示：入院病种编码为空" };
        //        //}
        //        //else if (string.IsNullOrWhiteSpace(bzmc))
        //        //{
        //        //    return new object[] { 0, 0, "医保提示：入院病种名称为空" };
        //        //}

        //        //string strSql = string.Format("select * from ybmzzydjdr a where a.jzlsh = '{0}' and a.cxbz = 1 and a.jzbz = 'z'", jzlsh);
        //        //DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);

        //        //if (ds != null && ds.Tables[0].Rows.Count > 0)
        //        //{
        //        //    return new object[] { 0, 0, "医保提示：his就诊流水号" + jzlsh + "已登记医保入院" };
        //        //}

        //        //strSql = string.Format(@"select a.z1date rysj, a.z1hznm hzxm, a.z1ksno, a.z1ksnm, a.z1empn z1ysno, a.z1mzys z1ysnm from zy01h a where a.z1zyno = '{0}'", jzlsh);
        //        //ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);

        //        //if (ds == null || ds.Tables[0].Rows.Count == 0)
        //        //{
        //        //    return new object[] { 0, 0, "医保提示：his就诊流水号" + jzlsh + "未登记his入院" };
        //        //}

        //        //DataRow dr = ds.Tables[0].Rows[0];
        //        //string ksbh = dr["z1ksno"].ToString(); //科室编号
        //        //string ksmc = dr["z1ksnm"].ToString(); //科室名称
        //        //string ysdm = dr["z1ysno"].ToString(); //医师代码
        //        //string ysxm = dr["z1ysnm"].ToString(); //医师姓名
        //        //string ghdjsj = Convert.ToDateTime(dr["rysj"]).ToString("yyyyMMddHHmmss");
        //        //string bedno = "";
        //        //string hzxm = dr["hzxm"].ToString();

        //        //if (string.IsNullOrWhiteSpace(ysdm) || string.IsNullOrWhiteSpace(ysxm))
        //        //{
        //        //    //return new object[] { 0, 0, "医保提示：his就诊流水号" + jzlsh + "医生编码或医生姓名为空" };
        //        //    ysdm = "010001";
        //        //    ysxm = "严东标";
        //        //}

        //        //if (!string.Equals(hzxm, xm))
        //        //{
        //        //    return new object[] { 0, 0, "医保提示：his就诊流水号" + jzlsh + "住院登记姓名和医保卡姓名不相符" };
        //        //}

        //        string rc = ybjzlsh + "|||||||" + jbr + "||||||||" + grbh + "|" + xm + "|" + kh + "|" + yllb + "|";
        //        string jylsh = DateTime.Now.ToString("yyyyMMddHHmmss") + "-" + DDYLJGBH + "-" + ybjzlsh;
        //        StringBuilder inputData = new StringBuilder(string.Format("{0}^{1}^{2}^{3}^{4}^{5}^{6}^{7}^", "2210", DDYLJGBH, czygh, ywzqh, jylsh, ZXBM, rc, "1"));
        //        StringBuilder outputData = new StringBuilder(100000);
        //        int i = BUSINESS_HANDLE(inputData, outputData);
        //        //Common.WriteYBLog(inputData.ToString());
        //        //Common.WriteYBLog(outputData.ToString());

        //        if (i == 0)
        //        {
        //            return new object[] { 0, 1, "修改成功" };
        //        }
        //        else
        //        {
        //            return new object[] { 0, 2, outputData };
        //        }
        //    }
        //    catch (Exception error)
        //    {
        //        Common.WriteYBLog(error.ToString());
        //        return new object[] { 0, 2, "Error:" + error.ToString() };
        //    }
        //}
        #endregion 住院登记修改(暂不使用)

        #region 住院登记撤销
        /// <summary>
        /// 住院登记撤销
        /// </summary>
        /// <param>his就诊流水号</param>
        /// <returns>1:成功，0:不成功，2:报错</returns>
        public static object[] YBZYDJCXDR(object[] objParam)
        {
            string czygh = CliUtils.fLoginUser;
            string ywzqh = YWZQH;
            string jbr = CliUtils.fUserName;
            string jzlsh = objParam[0].ToString();

            if (string.IsNullOrWhiteSpace(jzlsh))
            {
                return new object[] { 0, 0, "医保提示：his就诊流水号为空" };
            }

            DateTime dqsj = Convert.ToDateTime(Common.GetServerTime());
            string jylsh = dqsj.ToString("yyyyMMddHHmmss") + "-" + DDYLJGBH + "-" + jzlsh;

            try
            {
                string strSql = string.Format("select ybjzlsh, grbh, xm, kh, ydrybz, tcqh from ybmzzydjdr a where a.jzlsh = '{0}' and a.cxbz = 1 and a.jzbz = 'z'", jzlsh);//0秒
                DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);

                if (ds == null || ds.Tables[0].Rows.Count == 0)
                {
                    return new object[] { 0, 0, "医保提示：医保未办理入院登记" };
                }

                DataRow dr = ds.Tables[0].Rows[0];
                string ybjzlsh = dr["ybjzlsh"].ToString();
                string grbh = dr["grbh"].ToString();
                string xm = dr["xm"].ToString();
                string kh = dr["kh"].ToString();
                string ydrybz = dr["ydrybz"].ToString();
                ZXBM = ydrybz == "0" ? "0000" : dr["tcqh"].ToString();
                strSql = string.Format("select jzlsh from ybfyjsdr a where a.jzlsh = '{0}' and a.cxbz = 1", jzlsh);//0秒
                ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);

                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    return new object[] { 0, 0, "医保门诊登记撤销提示：医保已结算" };
                }

                if (ydrybz == "0")
                {
                    strSql = string.Format("select jzlsh from ybcfmxscfhdr a where a.jzlsh = '{0}' and a.cxbz = 1", jzlsh);//0秒
                    ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);

                    if (ds != null && ds.Tables[0].Rows.Count > 0)
                    {
                        return new object[] { 0, 0, "医保提示：医保已上传费用明细不能撤销入院登记" };
                    }
                }

                string rc = ybjzlsh + "|" + jbr + "|" + grbh + "|" + xm + "|" + kh + "|";
                StringBuilder inputData = new StringBuilder(string.Format("{0}^{1}^{2}^{3}^{4}^{5}^{6}^{7}^", "2240", DDYLJGBH, czygh, ywzqh, jylsh, ZXBM, rc, "1"));
                StringBuilder outputData = new StringBuilder(100000);
                int i = BUSINESS_HANDLE(inputData, outputData);

                if (i == 0)
                {
                    string strSql1 = string.Format(@"insert into ybmzzydjdr(jzlsh, jylsh, dwbh, sfzh, xb, mz, csrq, yldylb, rycbzt, ydrybz, tcqh
                    , nd, zyzt, zhye, bnylflj, bnzhzclj, bntczclj, bnjzjzclj, bngwybzjjlj, bnczjmmztczflj, jrtcfylj, jrjzjfylj, qfbzlj, bnzycs
                    , dwmc, nl, cbdwlx, jbjgbm, elmmxezc, elmmxesy, yldyxz, gsdyxz, sydyxz, yllb, ghdjsj, bzbm, bzmc, ksbh, ksmc, cwh, ysdm
                    , ysxm, jbr, grbh, bq, zzyybh, zzyymc, bz, tsfybz, xm, kh, ghf, ybzl, mmbzbm1, mmbzmc1, mmbzbm2, mmbzmc2, mmbzbm3, mmbzmc3
                    , mmbzbm4, mmbzmc4, ybjzlsh, jzbz, fsxx, cxbz) 
                    select jzlsh, jylsh, dwbh, sfzh, xb, mz, csrq, yldylb, rycbzt, ydrybz, tcqh, nd, zyzt, zhye, bnylflj, bnzhzclj, bntczclj
                    , bnjzjzclj, bngwybzjjlj, bnczjmmztczflj, jrtcfylj, jrjzjfylj, qfbzlj, bnzycs, dwmc, nl, cbdwlx, jbjgbm, elmmxezc, elmmxesy
                    , yldyxz, gsdyxz, sydyxz, yllb, ghdjsj, bzbm, bzmc, ksbh, ksmc, cwh, ysdm, ysxm, '{1}', grbh, bq, zzyybh, zzyymc, bz, tsfybz
                    , xm, kh, ghf, ybzl, mmbzbm1, mmbzmc1, mmbzbm2, mmbzmc2, mmbzbm3, mmbzmc3, mmbzbm4, mmbzmc4, ybjzlsh, jzbz, fsxx, 0 
                    from ybmzzydjdr where jzlsh = '{0}' and cxbz = 1 and jzbz = 'z'", jzlsh, jbr);//0秒
                    string strSql2 = string.Format("update ybmzzydjdr set cxbz = 2 where jzlsh = '{0}' and cxbz = 1 and jzbz = 'z'", jzlsh);//0秒
                    object[] obj = { strSql1, strSql2 };
                    obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);

                    if (obj[1].ToString() == "1")
                    {
                        return new object[] { 0, 1, outputData };
                    }
                    else
                    {
                        Common.InsertYBLog(jzlsh, "", obj[2].ToString());
                        return new object[] { 0, 0, "医保提示：his数据库操作失败：" + obj[2].ToString() };
                    }
                }
                else if (i == -1)
                {
                    Common.InsertYBLog(jzlsh, inputData.ToString(), outputData.ToString());
                    string strSql1 = string.Format(@"insert into ybmzzydjdr(jzlsh, jylsh, dwbh, sfzh, xb, mz, csrq, yldylb, rycbzt, ydrybz, tcqh
                    , nd, zyzt, zhye, bnylflj, bnzhzclj, bntczclj, bnjzjzclj, bngwybzjjlj, bnczjmmztczflj, jrtcfylj, jrjzjfylj, qfbzlj, bnzycs
                    , dwmc, nl, cbdwlx, jbjgbm, elmmxezc, elmmxesy, yldyxz, gsdyxz, sydyxz, yllb, ghdjsj, bzbm, bzmc, ksbh, ksmc, cwh, ysdm
                    , ysxm, jbr, grbh, bq, zzyybh, zzyymc, bz, tsfybz, xm, kh, ghf, ybzl, mmbzbm1, mmbzmc1, mmbzbm2, mmbzmc2, mmbzbm3, mmbzmc3
                    , mmbzbm4, mmbzmc4, ybjzlsh, jzbz, fsxx, cxbz) 
                    select jzlsh, jylsh, dwbh, sfzh, xb, mz, csrq, yldylb, rycbzt, ydrybz, tcqh, nd, zyzt, zhye, bnylflj, bnzhzclj, bntczclj
                    , bnjzjzclj, bngwybzjjlj, bnczjmmztczflj, jrtcfylj, jrjzjfylj, qfbzlj, bnzycs, dwmc, nl, cbdwlx, jbjgbm, elmmxezc, elmmxesy
                    , yldyxz, gsdyxz, sydyxz, yllb, ghdjsj, bzbm, bzmc, ksbh, ksmc, cwh, ysdm, ysxm, '{1}', grbh, bq, zzyybh, zzyymc, bz, tsfybz
                    , xm, kh, ghf, ybzl, mmbzbm1, mmbzmc1, mmbzbm2, mmbzmc2, mmbzbm3, mmbzmc3, mmbzbm4, mmbzmc4, ybjzlsh, jzbz, fsxx, 0 
                    from ybmzzydjdr where jzlsh = '{0}' and cxbz = 1 and jzbz = 'z'", jzlsh, jbr);//0秒
                    string strSql2 = string.Format("update ybmzzydjdr set cxbz = 2 where jzlsh = '{0}' and cxbz = 1 and jzbz = 'z'", jzlsh);//0秒
                    object[] obj = { strSql1, strSql2 };
                    obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);

                    if (obj[1].ToString() == "1")
                    {
                        return new object[] { 0, -1, "医保提示：医保住院登记撤销系统级别错误，自动删除医保住院登记成功，" + outputData.ToString() };
                    }
                    else
                    {
                        Common.InsertYBLog(jzlsh, "", obj[2].ToString());
                        return new object[] { 0, -1, "医保提示：医保住院登记撤销系统级别错误，自动删除医保住院登记失败，" + obj[2].ToString() + outputData.ToString() };
                    }
                }
                else
                {
                    Common.InsertYBLog(jzlsh, inputData.ToString(), outputData.ToString());
                    return new object[] { 0, -2, "医保提示：医保住院登记撤销业务级别或未知错误，" + outputData.ToString() };
                }
            }
            catch (Exception error)
            {
                Common.InsertYBLog(jzlsh, "", error.ToString());
                return new object[] { 0, 2, "Error:" + error.ToString() };
            }
        }
        #endregion 住院登记撤销

        #region 获取患者参保类型东软
        /// <summary>
        /// 获取患者参保类型东软
        /// </summary>
        /// <param>his就诊流水号</param>
        /// <returns>Y：医保；N：农保</returns>
        public static object[] GetCBLXDR(object[] objParam)
        {
            if (objParam.Length == 0)
            {
                return new object[] { 0, 0, "医保提示：入参为0个" };
            }

            string jzlsh = objParam[0].ToString();

            if (string.IsNullOrWhiteSpace(jzlsh))
            {
                return new object[] { 0, 0, "医保提示：就诊流水号为空" };
            }

            try
            {
                string sql = string.Format(@"select 'Y' cblx from ybmzzydjdr a where jzlsh = '{0}' and cxbz = 1 
                union all 
                select 'N' from nhrydj where illcase_no = '{0}' and bz = 1", jzlsh);
                DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", sql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                DataTable dt = ds.Tables[0];

                if (dt.Rows.Count > 0)
                {
                    if (dt.Rows[0]["cblx"].ToString() == "Y")
                    {
                        return new object[] { 0, 1, "Y" };
                    }
                    else
                    {
                        return new object[] { 0, 1, "N" };
                    }
                }
                else
                {
                    return new object[] { 0, 1, "Z" };
                }
            }
            catch (Exception ee)
            {
                return new object[] { 0, 2, "医保提示：" + ee.ToString() };
            }
        }
        #endregion 获取患者参保类型东软

        #region 大额基金提示东软
        /// <summary>
        /// 大额基金提示东软
        /// </summary>
        /// <param>his就诊流水号，结算号</param>
        /// <returns></returns>
        public virtual object[] GetDejjts(object[] objParam)
        {
            if (objParam.Length < 2)
            {
                return new object[] { 0, 0, "医保提示：入参小于2个" };
            }

            string jzlsh = objParam[0].ToString();
            string jsh = objParam[1].ToString();

            if (string.IsNullOrWhiteSpace(jzlsh) || string.IsNullOrWhiteSpace(jsh))
            {
                return new object[] { 0, 0, "医保提示：his就诊流水号或结算号为空" };
            }

            try
            {
                string sql = string.Format(@"select b.dejjzf, b.bndezflj, c.bzname tcq, a.dwmc from ybmzzydjdr(nolock) a join ybfyjsdr(nolock) b on a.jzlsh = b.jzlsh 
                join bztbd(nolock) c on c.bzkeyx = a.tcqh 
                where bzcodn = 'DQ' and bzusex = 1 and a.cxbz = 1 and b.cxbz = 1 and b.jzlsh = '{0}' and b.djhin = '{1}'", jzlsh, jsh);//0秒
                DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", sql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                DataTable dt = ds.Tables[0];

                if (dt.Rows.Count > 0)
                {
                    DataRow dr = dt.Rows[0];
                    decimal dejjzf = Convert.ToDecimal(dr["dejjzf"]);
                    decimal bndezflj = Convert.ToDecimal(dr["bndezflj"]);

                    if (dejjzf > 0 || bndezflj > 0)
                    {
                        Frm_ybdejjtsdr frm_ybdejjtsdr = new Frm_ybdejjtsdr(dejjzf, bndezflj, dr["tcq"].ToString(), dr["dwmc"].ToString());
                        frm_ybdejjtsdr.ShowDialog();
                    }
                }

                return null;
            }
            catch (Exception ee)
            {
                return new object[] { 0, 2, "医保提示：" + ee.ToString() };
            }
        }
        #endregion 大额基金提示东软

        #region 门诊处方明细上报
        /// <summary>
        /// 门诊处方明细上报
        /// </summary>
        /// <param>his就诊流水号,处方号(格式：'01','02'),结算时间(格式：yyyy-MM-dd HH:mm:ss)</param>
        /// <returns>1成功（金额|自理金额|自费金额|超限价自付金额|收费类别|收费项目等级|全额自费标志|自理比例|限价|备注）,2失败</returns>
        public static object[] YBMZCFMXSBDR(object[] objParam)
        {
            string czygh = CliUtils.fLoginUser;
            string ywzqh = YWZQH;
            string jbr = CliUtils.fUserName;
            string jzlsh = objParam[0].ToString();
            string cfhs = objParam[1].ToString();
            string cfsj = Convert.ToDateTime(objParam[2]).ToString("yyyyMMddHHmmss");
            DateTime dqsj = Convert.ToDateTime(Common.GetServerTime());
            string jylsh = dqsj.ToString("yyyyMMddHHmmss") + "-" + DDYLJGBH + "-" + jzlsh;
            string cfsjh = jzlsh + dqsj.ToString("HHmmss");
            bool isybcfmxsc = false;
            string ybjzlsh = "";
            string grbh = "";
            string xm = "";
            string kh = "";

            try
            {
                string strSql = string.Format("select grbh, xm, kh, ybjzlsh from ybmzzydjdr a where a.jzlsh = '{0}' and a.cxbz = 1 and a.jzbz = 'm'", jzlsh);//0秒
                DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);

                if (ds == null || ds.Tables[0].Rows.Count == 0)
                {
                    return new object[] { 0, 0, "医保提示：医保无门诊挂号信息" };
                }

                DataRow dr1 = ds.Tables[0].Rows[0];
                grbh = dr1["grbh"].ToString();
                xm = dr1["xm"].ToString();
                kh = dr1["kh"].ToString();
                ybjzlsh = dr1["ybjzlsh"].ToString();
                ZXBM = "0000";
                string sqlcx = string.Format(@"select distinct a.jzlsh from ybcfmxscfhdr a where a.grbh = '{0}' and a.cxbz = 1 and isnull(a.jsdjh, '') = ''", grbh);//0秒
                DataTable dtcx = CliUtils.ExecuteSql("sybdj", "cmd", sqlcx, CliUtils.fLoginDB, true, CliUtils.fCurrentProject).Tables[0];

                if (dtcx != null && dtcx.Rows.Count > 0)
                {
                    foreach (DataRow dr in dtcx.Rows)
                    {
                        object[] obj = { dr["jzlsh"].ToString() };
                        obj = YBMZCFMXCXDR(obj);

                        if (obj[1].ToString() == "1")
                        {

                        }
                        else
                        {
                            Common.InsertYBLog(jzlsh, "", obj[2].ToString());
                        }
                    }
                }

                StringBuilder rc = new StringBuilder();
                strSql = string.Format(@"select y.ybxmbh, y.ybxmmc, m.dj, sum(m.sl) sl, sum(m.je) je, m.yyxmbh, m.yyxmmc, m.ysdm, n.b1name ysxm, m.ksno, o.b2ejnm zxks, m.sfno, y.sfxmzldm ybsfxmzldm, y.sflbdm ybsflbdm, m.cfh from 
                        (
                            --药品
                            select a.mcypno yyxmbh, a.mcypnm yyxmmc, a.mcpric dj, a.mcquty sl, a.mcamnt je, a.mcksno ksno, a.mcuser ysdm, a.mcsflb sfno, a.mccfno cfh
                            from mzcfd a 
                            where a.mcghno = '{0}' and a.mccfno in ({1})
                            union all
                            --检查/治疗
                            select b.mbitem yyxmbh, b.mbname yyxmmc, b.mbpric dj, b.mbquty sl, b.mbsjam je, b.mbksno ksno, b.mbuser ysdm, b.mbsfno sfno , b.mbzlno cfh          
                            from mzb2d b 
                            where b.mbghno = '{0}' and b.mbzlno in ({1})
                            union all
                            --检验
                            select c.mbitem yyxmbh, c.mbname yyxmmc, c.mbpric dj, c.mbquty sl, c.mbsjam je, c.mbksno ksno, c.mbuser ysdm, c.mbsfno sfno, c.mbzlno cfh
                            from mzb4d c 
                            where c.mbghno = '{0}' and c.mbzlno in ({1})
                            union all
                            --注射
                            select b5item yyxmbh, b5name yyxmmc, b5sfam dj, mddays sl, b5sfam * mddays je, mdzsks ksno, mdempn ysdm, b5sfno sfno, mdzsno cfh
                            from mzd3d
                            left join (select distinct mccfno, mcghno from mzcfd) mzcf on mccfno = mdcfno
                            left join bz09d on b9mbno = mdtwid 
                            left join bz05d on b5item = b9item where mdtiwe > 0 and mdzsno in ({1})
                            union all
                            select b5item yyxmbh, b5name yyxmmc, b5sfam dj, mdtims sl, b5sfam * mdtims je, mdzsks ksno, mdempn ysdm, b5sfno sfno, mdzsno cfh
                            from mzd3d 
                            left join bz09d on b9mbno = mdwayid 
                            left join bz05d on b5item = b9item
                            left join (select distinct mccfno, mcghno from mzcfd) mzcf on mccfno = mdcfno 
                            where mdzsno in ({1})
                            union all
                            select b5item yyxmbh, b5name yyxmmc, b5sfam dj, mdpqty sl, b5sfam * mdpqty je, mdzsks ksno, mdempn ysdm, b5sfno sfno, mdzsno cfh
                            from mzd3d 
                            left join bz09d on b9mbno = mdpprid 
                            left join bz05d on b5item = b9item
                            left join (select distinct mccfno, mcghno from mzcfd) mzcf on mccfno = mdcfno
                            where mdpqty > 0 and mdzsno in ({1})
                            union all
                            --处方划价
                            select a.ygypno yyxmbh, a.ygypnm yyxmmc, ((a.ygamnt + 0.0) / a.ygslxx) dj, a.ygslxx sl, a.ygamnt je, b.ygksno ksno, b.ygysno ysdm, c.y1sflb, a.ygshno cfh
                            from yp17d a 
                            join yp17h b on a.ygcomp = b.ygcomp and a.ygshno = b.ygshno
                            join yp01h c on c.y1ypno = a.ygypno
                            where b.ygghno = '{0}' and a.ygshno in ({1}) and a.ygslxx > 0
                        ) m 
                        left join bz01h n on m.ysdm = n.b1empn 
                        left join bz02d o on m.ksno = o.b2ejks
                        left join ybhisdzdr y on m.yyxmbh = y.hisxmbh and y.scbz = 1
                        group by y.ybxmbh, y.ybxmmc, m.dj, m.yyxmbh, m.yyxmmc, m.ysdm, n.b1name, m.ksno, o.b2ejnm, m.sfno, y.sfxmzldm, y.sflbdm, m.cfh"
                                    , jzlsh, cfhs);//3秒
                ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                List<string> licfh = new List<string>();
                List<string> liybcfh = new List<string>();
                List<string> liyyxmdm = new List<string>();
                List<string> liyyxmmc = new List<string>();
                List<string> lizysfdj = new List<string>();

                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    DataTable dt = ds.Tables[0];
                    StringBuilder wdzxms = new StringBuilder();

                    for (int k = 0; k < dt.Rows.Count; k++)
                    {
                        DataRow dr = dt.Rows[k];

                        if (dr["ybxmbh"] == DBNull.Value)
                        {
                            wdzxms.Append("医保提示：项目代码：[" + dr["yyxmbh"].ToString() + "]，名称：[" + dr["yyxmmc"].ToString() + "]未对照或未上传，不能上传费用！");
                        }
                        else if (dr["ksno"] == DBNull.Value || dr["ksno"].ToString() == "")
                        {
                            return new object[] { 0, 0, "医保提示：有科室编号为空的记录" };
                        }
                        else if (dr["zxks"] == DBNull.Value || dr["zxks"].ToString() == "")
                        {
                            return new object[] { 0, 0, "医保提示：无此科室编号[" + dr["ksno"].ToString() + "]" };
                        }
                        else
                        {
                            string ybsfxmzldm = dr["ybsfxmzldm"].ToString();
                            string ybsflbdm = dr["ybsflbdm"].ToString();
                            string yyxmbh = dr["yyxmbh"].ToString();
                            string ybxmbh = dr["ybxmbh"].ToString();
                            string yyxmmc = dr["yyxmmc"].ToString();
                            decimal dj = Convert.ToDecimal(dr["dj"]);
                            decimal sl = Convert.ToDecimal(dr["sl"]);
                            decimal je = Convert.ToDecimal(dr["je"]);
                            decimal mcyl = 1;
                            string ysbm = string.IsNullOrWhiteSpace(dr["ysdm"].ToString()) ? "010001" : dr["ysdm"].ToString();
                            string ysxm = string.IsNullOrWhiteSpace(dr["ysxm"].ToString()) ? "严东标" : dr["ysxm"].ToString();
                            string ksdm = dr["ksno"].ToString();
                            string ksmc = dr["zxks"].ToString();
                            string cfh = dr["cfh"].ToString();
                            licfh.Add(cfh);
                            string ybcfh = cfsjh + k.ToString();
                            liybcfh.Add(ybcfh);
                            liyyxmdm.Add(yyxmbh);
                            liyyxmmc.Add(yyxmmc);
                            string ypjldw = "-";
                            string cydffbz = "0";

                            if (ybsfxmzldm == "1")
                            {
                                cydffbz = "1";
                            }

                            if (dr["sfno"].ToString() == "01" || dr["sfno"].ToString() == "02" || dr["sfno"].ToString() == "03")
                            {
                                ypjldw = "粒";
                            }

                            rc.Append(ybjzlsh + "|" + ybsfxmzldm + "|" + ybsflbdm + "|" + ybcfh + "|" + cfsj
                            + "|" + yyxmbh + "|" + ybxmbh + "|" + yyxmmc + "|" + Math.Round(dj, 4).ToString() + "|" + Math.Round(sl, 2).ToString() + "|"
                            + Math.Round(je, 4).ToString() + "|101||" + mcyl.ToString() + "||" + ysbm + "|" + ysxm + "|||" + ksdm + "|"
                            + ksmc + "||" + cydffbz + "|" + jbr + "|" + ypjldw + "||" + grbh + "|" + xm + "|" + kh + "|$");
                            string strSql1 = string.Format(@"insert into ybcfmxscindr(jzlsh, jylsh, sfxmzl, sflb, ybcfh, cfrq, yysfxmbm, sfxmzxbm
                            , yysfxmmc, dj, sl, je, mcyl, ysbm, ysxm, ksbh, ksmc, cydffbz, jbr, ypjldw, grbh, xm, kh)
                            values ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', {9}, {10}, {11}, {12}, '{13}', '{14}'
                            , '{15}', '{16}', '{17}', '{18}', '{19}', '{20}', '{21}', '{22}')"
                            , jzlsh, jylsh, ybsfxmzldm, ybsflbdm, ybcfh, cfsj, yyxmbh, ybxmbh, yyxmmc, Math.Round(dj, 4).ToString(), sl
                            , Math.Round(je, 4).ToString(), mcyl, ysbm, ysxm, ksdm, ksmc, cydffbz, jbr, ypjldw, grbh, xm, kh);
                            lizysfdj.Add(strSql1);
                        }
                    }

                    if (wdzxms.Length > 0)
                    {
                        return new object[] { 0, 0, "医保提示：" + wdzxms };
                    }
                }
                else
                {
                    return new object[] { 0, 0, "医保提示：his无费用明细" };
                }

                Frm_ybfyscxxdrjj frm_ybfyscxxdrjj = new Frm_ybfyscxxdrjj(jzlsh, cfhs, xm);
                frm_ybfyscxxdrjj.ShowDialog();
                StringBuilder inputData = new StringBuilder(string.Format("{0}^{1}^{2}^{3}^{4}^{5}^{6}^{7}^", "2310", DDYLJGBH, czygh, ywzqh, jylsh, ZXBM, rc.ToString().TrimEnd('$'), "1"));
                StringBuilder outputData = new StringBuilder(100000);
                int i = BUSINESS_HANDLE(inputData, outputData);

                if (i == 0)
                {
                    isybcfmxsc = true;
                    string[] zysfdjfhs = outputData.ToString().Split('^')[2].TrimEnd('$').Split('$');

                    for (int j = 0; j < zysfdjfhs.Length; j++)
                    {
                        string[] zysfdjfh = zysfdjfhs[j].Split('|');
                        decimal je;
                        bool isConvert = decimal.TryParse(zysfdjfh[0], out je);
                        decimal zlje;
                        isConvert = decimal.TryParse(zysfdjfh[1], out zlje);
                        decimal zfje;
                        isConvert = decimal.TryParse(zysfdjfh[2], out zfje);
                        decimal cxjzfje;
                        isConvert = decimal.TryParse(zysfdjfh[3], out cxjzfje);
                        string sflb1 = zysfdjfh[4];
                        string sfxmdj = zysfdjfh[5];
                        string qezfbz = zysfdjfh[6];
                        decimal zlbl;
                        isConvert = decimal.TryParse(zysfdjfh[7], out zlbl);
                        decimal xj;
                        isConvert = decimal.TryParse(zysfdjfh[8], out xj);
                        string bz = zysfdjfh[9];
                        string strSql1 = string.Format(@"insert into ybcfmxscfhdr(jzlsh, jylsh, je, zlje, zfje, cxjzfje, sflb, sfxmdj, qezfbz, zlbl, xj, bz, grbh, xm, kh, cfh, ybcfh, yyxmdm, yyxmmc) 
                        values('{0}', '{1}', {2}, {3}, {4}, {5}, '{6}', '{7}', '{8}', {9}, {10}, '{11}', '{12}', '{13}', '{14}', '{15}', '{16}', '{17}', '{18}')"
                        , jzlsh, jylsh, je, zlje, zfje, cxjzfje, sflb1, sfxmdj, qezfbz, zlbl, xj, bz, grbh, xm, kh, licfh[j], liybcfh[j], liyyxmdm[j], liyyxmmc[j]);
                        lizysfdj.Add(strSql1);
                    }

                    object[] obj = lizysfdj.ToArray();
                    obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);

                    if (obj[1].ToString() == "1")
                    {
                        return new object[] { 0, 1, "医保提示：医保处方明细上传成功" };
                    }
                    else
                    {
                        Common.InsertYBLog(jzlsh, "", obj[2].ToString());
                        i = ChangeYBCFMXSDCXDR(new object[]{ybjzlsh, grbh, xm, kh, ZXBM});

                        if (i == 1)
                        {
                            return new object[] { 0, 0, "医保提示：his数据库操作失败,自动撤销医保处方明细上传成功,错误信息：" + obj[2].ToString() };
                        }
                        else
                        {
                            return new object[] { 0, 0, "医保提示：his数据库操作失败,自动撤销医保处方明细上传失败,错误信息：" + obj[2].ToString() };
                        }
                    }
                }
                else if (i == -1)
                {
                    Common.InsertYBLog(jzlsh, inputData.ToString(), outputData.ToString());
                    i = ChangeYBCFMXSDCXDR(new object[]{ybjzlsh, grbh, xm, kh, ZXBM});

                    if (i == 1)
                    {
                        return new object[] { 0, -1, "医保提示：医保系统级别错误，自动撤销医保门诊处方明细上传成功，" + outputData.ToString() };
                    }
                    else
                    {
                        return new object[] { 0, -1, "医保提示：医保系统级别错误，自动撤销医保门诊处方明细上传失败，" + outputData.ToString() };
                    }
                }
                else
                {
                    Common.InsertYBLog(jzlsh, inputData.ToString(), outputData.ToString());
                    return new object[] { 0, -2, "医保提示：医保门诊处方明细上传业务级别或未知错误，" + outputData.ToString() };
                }
            }
            catch (Exception error)
            {
                Common.InsertYBLog(jzlsh, "", error.ToString());

                if (isybcfmxsc)
                {
                    int i = ChangeYBCFMXSDCXDR(new object[]{ybjzlsh, grbh, xm, kh, ZXBM});

                    if (i == 1)
                    {
                        return new object[] { 0, 2, "医保提示：非医保异常，自动撤销医保门诊处方明细上传成功，" + error.ToString() };
                    }
                    else
                    {
                        return new object[] { 0, 2, "医保提示：非医保异常，自动撤销医保门诊处方明细上传失败，" + error.ToString() };
                    }
                }

                return new object[] { 0, 2, "Error:" + error.ToString() };
            }
        }
        #endregion 门诊处方明细上报

        #region 门诊处方明细撤销
        /// <summary>
        /// 门诊处方明细撤销
        /// </summary>
        /// <param>his就诊流水号</param>
        /// <returns>1:成功，0:不成功，2:报错</returns>
        public static object[] YBMZCFMXCXDR(object[] objParam)
        {
            try
            {
                ZXBM = "0000";
                string czygh = CliUtils.fLoginUser;
                string ywzqh = YWZQH;
                string jbr = CliUtils.fUserName;
                string jzlsh = objParam[0].ToString();
                DateTime dqsj = Convert.ToDateTime(Common.GetServerTime());
                string jylsh = dqsj.ToString("yyyyMMddHHmmss") + "-" + DDYLJGBH + "-" + jzlsh;
                string strSql = string.Format(@"select jzlsh from ybcfmxscfhdr a where a.jzlsh = '{0}' and a.cxbz = 1 and a.jsdjh is null", jzlsh);//1秒
                DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);

                if (ds == null || ds.Tables[0].Rows.Count == 0)
                {
                    return new object[] { 0, 0, "医保提示：医保已收费结算，不能撤销处方明细" };
                }

                strSql = string.Format("select grbh, xm, kh from ybmzzydjdr a where a.jzlsh = '{0}' and a.cxbz = 1 and a.jzbz = 'm'", jzlsh);//0秒
                ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);

                if (ds == null || ds.Tables[0].Rows.Count == 0)
                {
                    return new object[] { 0, 0, "医保提示：医保无门诊挂号记录" };
                }

                DataRow dr = ds.Tables[0].Rows[0];
                string grbh = dr["grbh"].ToString();
                string xm = dr["xm"].ToString();
                string kh = dr["kh"].ToString();
                string rc = jzlsh + "||" + jbr + "|" + grbh + "|" + xm + "|" + kh + "|";
                StringBuilder inputData = new StringBuilder(string.Format("{0}^{1}^{2}^{3}^{4}^{5}^{6}^{7}^", "2320", DDYLJGBH, czygh, ywzqh, jylsh, ZXBM, rc, "1"));
                StringBuilder outputData = new StringBuilder(100000);
                int i = BUSINESS_HANDLE(inputData, outputData);

                if (i == 0)
                {
                    string strSql1 = string.Format(@"delete from ybcfmxscfhdr where jzlsh = '{0}' and cxbz = 1 and jsdjh is null", jzlsh);//1秒
                    string strSql2 = string.Format(@"delete from ybcfmxscindr where jzlsh = '{0}' and cxbz = 1 and jsdjh is null", jzlsh);//1秒
                    object[] obj = { strSql1, strSql2 };
                    obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);

                    if (obj[1].ToString() == "1")
                    {
                        return new object[] { 0, 1, outputData };
                    }
                    else
                    {
                        Common.WriteYBLog(obj[2].ToString());
                        return new object[] { 0, 0, "医保提示：his数据库操作失败：" + obj[2].ToString() };
                    }
                }
                else if (i == -1)
                {
                    string strSql1 = string.Format("delete from ybcfmxscfhdr where jzlsh = '{0}' and cxbz = 1 and jsdjh is null", jzlsh);//1秒
                    string strSql2 = string.Format("delete from ybcfmxscindr where jzlsh = '{0}' and cxbz = 1 and jsdjh is null", jzlsh);//1秒
                    object[] obj = { strSql1, strSql2 };
                    obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);

                    if (obj[1].ToString() == "1")
                    {
                        return new object[] { 0, -1, "医保提示：医保门诊处方撤销系统级别错误，自动删除医保处方明细成功，" + outputData.ToString() };
                    }
                    else
                    {
                        return new object[] { 0, -1, "医保提示：医保门诊处方撤销系统级别错误，自动删除医保处方明细失败，" + outputData.ToString() };
                    }
                }
                else
                {
                    return new object[] { 0, -2, "医保提示：医保门诊处方撤销业务级别或未知错误，" + outputData.ToString() };
                }
            }
            catch (Exception error)
            {
                Common.WriteYBLog(error.ToString());
                return new object[] { 0, 2, "Error:" + error.ToString() };
            }
        }
        #endregion 门诊处方明细撤销

        #region 老处方明细上报
        #region 住院处方明细上报 负交易模式(未使用)
        /// <summary>
        /// 住院处方明细上报
        /// </summary>
        /// <param>就诊流水号</param>
        /// <returns>金额|自理金额|自费金额|超限价自付金额|收费类别|收费项目等级|全额自费标志|自理比例|限价|备注</returns>
        //        public static object[] YBZYCFMXSB(object[] objParam)
        //        {
        //            string czygh = CliUtils.fLoginUser;
        //            string ywzqh = YWZQH;
        //            string jbr = CliUtils.fUserName;
        //            string jzlsh = objParam[0].ToString();
        //            string cfsj = DateTime.Now.ToString("yyyyMMddHHmmss");
        //            string jylsh = cfsj + "-" + DDYLJGBH + "-" + new Random().Next(100).ToString().PadLeft(4, '0');

        //            try
        //            {
        //                string strSql = string.Format("select * from ybmzzydjdr a where a.jzlsh = '{0}' and a.cxbz = 1", jzlsh);
        //                DataSet ds = CliUtils.ExecuteSql("sybdjdr", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);

        //                if (ds == null || ds.Tables[0].Rows.Count == 0)
        //                {
        //                    return new object[] { 0, 0, "无医保住院登记信息" };
        //                }

        //                DataRow dr1 = ds.Tables[0].Rows[0];
        //                string ybjzlsh = dr1["ybjzlsh"].ToString();
        //                string grbh = dr1["grbh"].ToString();
        //                string xm = dr1["xm"].ToString();
        //                string kh = dr1["kh"].ToString();
        //                string ydrybz = dr1["ydrybz"].ToString();
        //                ZXBM = ydrybz == "0" ? "0000" : dr1["tcqh"].ToString();
        //                StringBuilder rc = new StringBuilder();
        //                strSql = string.Format(@"select y.ybxmbh, y.ybxmmc, t.z3djxx dj
        //                    , sum(case left(t.z3endv, 1) when '4' then -t.z3jzcs else t.z3jzcs end) sl
        //                    , sum(case left(t.z3endv, 1) when '4' then -t.z3jzje else t.z3jzje end) je
        //                    , t.z3mbno yyxmbh, t.z3item yyxmmc, t.z3empn ysdm, t.z3kdys ysxm, c.z1ksno ksno, c.z1ksnm zxks
        //                    , t.z3sfno sfno, y.sfxmzldm ybsfxmzldm, y.sflbdm ybsflbdm, max(t.z3date) cfsj
        //                    from (select a.z3djxx, a.z3jzcs, a.z3jzje, a.z3mbno, a.z3item, a.z3empn, a.z3kdys, a.z3ksno
        //                    , a.z3zxks, a.z3sfno, a.z3date, a.z3ybup, a.z3kind, a.z3zyno, a.z3endv
        //                    from zy03d a 
        //                    union all
        //                    select b.z3djxx, b.z3jzcs, b.z3jzje, b.z3mbno, b.z3item, b.z3empn, b.z3kdys, b.z3ksno
        //                    , b.z3zxks, b.z3sfno, b.z3date, b.z3ybup, b.z3kind, b.z3zyno, b.z3endv
        //                    from zy03dz b) t
        //                    join zy01h c on t.z3zyno = c.z1zyno
        //                    left join ybhisdzdr y on t.z3mbno = y.hisxmbh 
        //                    where t.z3ybup is null and left(t.z3kind, 1) in ('2', '4') and t.z3zyno = '{0}' 
        //                    group by y.ybxmbh, y.ybxmmc, t.z3djxx, t.z3mbno, t.z3item
        //                    , t.z3empn, t.z3kdys, c.z1ksno, c.z1ksnm, t.z3sfno, y.sfxmzldm, y.sflbdm
        //                    having sum(case left(t.z3endv, 1) when '4' then -t.z3jzcs else t.z3jzcs end) != 0"
        //                    , jzlsh);
        //                ds = CliUtils.ExecuteSql("sybdjdr", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
        //                List<string> liybcfh = new List<string>();
        //                List<string> liyyxmdm = new List<string>();
        //                List<string> liyyxmmc = new List<string>();
        //                List<string> lizysfdj = new List<string>();

        //                if (ds != null && ds.Tables[0].Rows.Count > 0)
        //                {
        //                    DataTable dt = ds.Tables[0];
        //                    StringBuilder wdzxms = new StringBuilder();

        //                    for (int k = 0; k < dt.Rows.Count; k++)
        //                    {
        //                        DataRow dr = dt.Rows[k];

        //                        if (dr["ybxmbh"] == DBNull.Value)
        //                        {
        //                            wdzxms.Append("项目代码：[" + dr["yyxmbh"].ToString() + "]，名称：[" + dr["yyxmmc"].ToString() + "]未对照，不能上传！");
        //                        }
        //                        else if (dr["ysdm"] == DBNull.Value || dr["ysdm"].ToString() == "")
        //                        {
        //                            return new object[] { 0, 0, "有医生工号为空的记录" };
        //                        }
        //                        else if (dr["ysxm"] == DBNull.Value || dr["ysxm"].ToString() == "")
        //                        {
        //                            return new object[] { 0, 0, "无此医生工号[" + dr["ysdm"].ToString() + "]" };
        //                        }
        //                        else if (dr["ksno"] == DBNull.Value || dr["ksno"].ToString() == "")
        //                        {
        //                            return new object[] { 0, 0, "有科室编号为空的记录" };
        //                        }
        //                        else if (dr["zxks"] == DBNull.Value || dr["zxks"].ToString() == "")
        //                        {
        //                            return new object[] { 0, 0, "无此科室编号[" + dr["ksno"].ToString() + "]" };
        //                        }
        //                        else
        //                        {
        //                            string ybsfxmzldm = dr["ybsfxmzldm"].ToString();
        //                            string ybsflbdm = dr["ybsflbdm"].ToString();
        //                            string yyxmbh = dr["yyxmbh"].ToString();
        //                            string ybxmbh = dr["ybxmbh"].ToString();
        //                            string yyxmmc = dr["yyxmmc"].ToString();
        //                            decimal dj = Convert.ToDecimal(dr["dj"]);
        //                            decimal sl = Convert.ToDecimal(dr["sl"]);
        //                            decimal je = Convert.ToDecimal(dr["je"]);
        //                            decimal mcyl = 1;
        //                            string ysdm = dr["ysdm"].ToString();
        //                            string ysxm = dr["ysxm"].ToString();
        //                            string ksdm = dr["ksno"].ToString();
        //                            string ksmc = dr["zxks"].ToString();
        //                            string ypjldw = "-";
        //                            string ybcfh = cfsj + k.ToString();
        //                            string cydffbz = "0";

        //                            if (ybsfxmzldm == "1")
        //                            {
        //                                cydffbz = "1";
        //                            }

        //                            if (dr["sfno"].ToString() == "01" || dr["sfno"].ToString() == "02" || dr["sfno"].ToString() == "03")
        //                            {
        //                                ypjldw = "粒";
        //                            }

        //                            string cfsj1 = Convert.ToDateTime(dr["cfsj"]).ToString("yyyyMMddHHmmss");

        //                            //退费begin
        //                            if (je < 0)
        //                            {
        //                                string strtfSql = string.Format(@"select id, ybcfh, je - ytje from ybcfmxscfhdr where cxbz = 1 and jzlsh = '{0}' and yyxmdm = '{1}' order by je - ytje desc", jzlsh, yyxmbh);
        //                                DataSet dstf = CliUtils.ExecuteSql("sybdjdr", "cmd", strtfSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);

        //                                if (dstf != null && dstf.Tables[0].Rows.Count > 0)
        //                                {
        //                                    DataTable dttf = dstf.Tables[0];

        //                                    for (int j = 0; j < dttf.Rows.Count; j++)
        //                                    {
        //                                        DataRow drtf = dttf.Rows[j];
        //                                        string id = drtf[0].ToString();
        //                                        ybcfh = drtf[1].ToString();
        //                                        decimal tfje = Convert.ToDecimal(drtf[2]);
        //                                        decimal tfsl = tfje / dj;

        //                                        if (-sl > tfsl)
        //                                        {
        //                                            rc.Append(ybjzlsh + "|" + ybsfxmzldm + "|" + ybsflbdm + "|" + ybcfh + "|" + cfsj1 + "|" + yyxmbh
        //                                            + "|" + ybxmbh + "|" + yyxmmc + "|" + Math.Round(dj, 4).ToString() + "|" + Math.Round(-tfsl, 2).ToString() + "|" + Math.Round(-tfje, 4).ToString()
        //                                            + "|101||" + mcyl.ToString() + "||" + ysdm + "|" + ysxm + "|||" + ksdm + "|" + ksmc + "||" + cydffbz + "|"
        //                                            + jbr + "|" + ypjldw + "||" + grbh + "|" + xm + "|" + kh + "|$");
        //                                            lizysfdj.Add(string.Format("update ybcfmxscfhdr set ytje += {0} where id = {1}", tfje, id));
        //                                            string strSql1 = string.Format(@"insert into ybcfmxscindr(jzlsh, jylsh, sfxmzl, sflb, ybcfh
        //                                            , cfrq, yysfxmbm, sfxmzxbm, yysfxmmc, dj, sl, je, jx, gg, mcyl, sypc, ysbm, ysxm, yf, dw, ksbh, ksmc, zxts, cydffbz, jbr, ypjldw, qezfbz, grbh, xm, kh)
        //                                            values ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', {9}, {10}, {11}, '{12}'
        //                                            , '{13}', {14}, '{15}', '{16}', '{17}', '{18}', '{19}', '{20}', '{21}', '{22}', '{23}', '{24}', '{25}', '{26}', '{27}', '{28}', '{29}')"
        //                                            , jzlsh, jylsh, ybsfxmzldm, ybsflbdm, ybcfh, cfsj1, yyxmbh, ybxmbh, yyxmmc, Math.Round(dj, 4).ToString(), Math.Round(-tfsl, 2).ToString(), Math.Round(-tfje, 4).ToString()
        //                                            , "101", "", mcyl, "", ysdm, ysxm, "", "", ksdm, ksmc, "", cydffbz, jbr, ypjldw, "", grbh, xm, kh);
        //                                            lizysfdj.Add(strSql1);
        //                                            je += tfje;
        //                                            sl += tfsl;
        //                                            liybcfh.Add(ybcfh);
        //                                            liyyxmdm.Add(yyxmbh);
        //                                            liyyxmmc.Add(yyxmmc);
        //                                        }
        //                                        else
        //                                        {
        //                                            if (sl < 0)
        //                                            {
        //                                                rc.Append(ybjzlsh + "|" + ybsfxmzldm + "|" + ybsflbdm + "|" + ybcfh + "|" + cfsj1 + "|" + yyxmbh
        //                                             + "|" + ybxmbh + "|" + yyxmmc + "|" + Math.Round(dj, 4).ToString() + "|" + Math.Round(sl, 2).ToString() + "|" + Math.Round(je, 4).ToString()
        //                                             + "|101||" + mcyl.ToString() + "||" + ysdm + "|" + ysxm + "|||" + ksdm + "|" + ksmc + "||" + cydffbz + "|"
        //                                             + jbr + "|" + ypjldw + "||" + grbh + "|" + xm + "|" + kh + "|$");
        //                                                lizysfdj.Add(string.Format("update ybcfmxscfhdr set ytje += {0} where id = {1}", -je, id));
        //                                                string strSql1 = string.Format(@"insert into ybcfmxscindr(jzlsh, jylsh, sfxmzl, sflb, ybcfh, cfrq, yysfxmbm, sfxmzxbm
        //                                                , yysfxmmc, dj, sl, je, jx, gg, mcyl, sypc, ysbm, ysxm, yf, dw, ksbh, ksmc, zxts, cydffbz, jbr, ypjldw, qezfbz, grbh, xm, kh)
        //                                                values ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', {9}, {10}, {11}, '{12}', '{13}', {14}
        //                                                , '{15}', '{16}', '{17}', '{18}', '{19}', '{20}', '{21}', '{22}', '{23}', '{24}', '{25}', '{26}', '{27}', '{28}', '{29}')"
        //                                                , jzlsh, jylsh, ybsfxmzldm, ybsflbdm, ybcfh, cfsj1, yyxmbh, ybxmbh, yyxmmc, Math.Round(dj, 4).ToString(), Math.Round(sl, 2).ToString(), Math.Round(je, 4).ToString(), "101", "", mcyl, "", ysdm, ysxm, "", "", ksdm, ksmc, "", cydffbz, jbr, ypjldw, "", grbh, xm, kh);
        //                                                lizysfdj.Add(strSql1);
        //                                                je = 0;
        //                                                sl = 0;
        //                                                liybcfh.Add(ybcfh);
        //                                                liyyxmdm.Add(yyxmbh);
        //                                                liyyxmmc.Add(yyxmmc);
        //                                            }

        //                                            break;
        //                                        }
        //                                    }
        //                                }
        //                            }
        //                            //退费end
        //                            else
        //                            {
        //                                liybcfh.Add(ybcfh);
        //                                liyyxmdm.Add(yyxmbh);
        //                                liyyxmmc.Add(yyxmmc);
        //                                rc.Append(ybjzlsh + "|" + ybsfxmzldm + "|" + ybsflbdm + "|" + ybcfh + "|" + cfsj1 + "|" + yyxmbh
        //                                    + "|" + ybxmbh + "|" + yyxmmc + "|" + Math.Round(dj, 4).ToString() + "|" + Math.Round(sl, 2).ToString() + "|" + Math.Round(je, 4).ToString()
        //                                    + "|101||" + mcyl.ToString() + "||" + ysdm + "|" + ysxm + "|||" + ksdm + "|" + ksmc + "||" + cydffbz + "|"
        //                                    + jbr + "|" + ypjldw + "||" + grbh + "|" + xm + "|" + kh + "|$");
        //                                string strSql1 = string.Format(@"insert into ybcfmxscindr(jzlsh, jylsh, sfxmzl, sflb, ybcfh, cfrq, yysfxmbm, sfxmzxbm
        //, yysfxmmc, dj, sl, je, jx, gg, mcyl, sypc, ysbm, ysxm, yf, dw, ksbh, ksmc, zxts, cydffbz, jbr, ypjldw, qezfbz, grbh, xm, kh) values('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', {9}, {10}, {11}, '{12}', '{13}', {14}
        //, '{15}', '{16}', '{17}', '{18}', '{19}', '{20}', '{21}', '{22}', '{23}', '{24}', '{25}', '{26}', '{27}', '{28}', '{29}')"
        //, jzlsh, jylsh, ybsfxmzldm, ybsflbdm, ybcfh, cfsj1, yyxmbh, ybxmbh, yyxmmc, Math.Round(dj, 4).ToString(), Math.Round(sl, 2).ToString(), Math.Round(je, 4).ToString(), "101", "", mcyl, "", ysdm, ysxm, "", "", ksdm, ksmc, "", cydffbz, jbr, ypjldw, "", grbh, xm, kh);
        //                                lizysfdj.Add(strSql1);
        //                            }
        //                        }
        //                    }

        //                    if (wdzxms.Length > 0)
        //                    {
        //                        return new object[] { 0, 0, wdzxms.ToString() };
        //                    }
        //                }

        //                if (ds == null || ds.Tables[0].Rows.Count == 0)
        //                {
        //                    return new object[] { 0, 0, "无住院费用上传" };
        //                }

        //                StringBuilder inputData = new StringBuilder(string.Format("{0}^{1}^{2}^{3}^{4}^{5}^{6}^{7}^", "2310", DDYLJGBH, czygh, ywzqh, jylsh, ZXBM, rc.ToString().TrimEnd('$'), "1"));
        //                StringBuilder outputData = new StringBuilder(100000);
        //                int i = BUSINESS_HANDLE(inputData, outputData);
        //                WriteLogFile("YBZYCFMXSB_In.txt", inputData.ToString());
        //                WriteLogFile("YBZYCFMXSB_Out.txt", outputData.ToString());

        //                if (i == 0)
        //                {
        //                    string[] zysfdjfhs = outputData.ToString().Split('^')[2].TrimEnd('$').Split('$');

        //                    //begin判断返回记录数是否和上传记录数相等
        //                    if (zysfdjfhs.Length != liybcfh.Count || zysfdjfhs.Length != liyyxmdm.Count || zysfdjfhs.Length != liyyxmmc.Count)
        //                    {
        //                        i = YBCFMXSDCX(ybjzlsh, grbh, xm, kh, ZXBM);

        //                        if (i == 1)
        //                        {
        //                            return new object[] { 0, 0, "返回记录数和上传记录数不相等,自动冲销医保处方明细成功" };
        //                        }
        //                        else
        //                        {
        //                            return new object[] { 0, 0, "返回记录数和上传记录数不相等,自动冲销医保处方明细失败" };
        //                        }
        //                    }
        //                    //end判断返回记录数是否和上传记录数相等

        //                    for (int j = 0; j < zysfdjfhs.Length; j++)
        //                    {
        //                        string[] zysfdjfh = zysfdjfhs[j].Split('|');
        //                        decimal je;
        //                        bool isConvert = decimal.TryParse(zysfdjfh[0], out je);
        //                        decimal zlje;
        //                        isConvert = decimal.TryParse(zysfdjfh[1], out zlje);
        //                        decimal zfje;
        //                        isConvert = decimal.TryParse(zysfdjfh[2], out zfje);
        //                        decimal cxjzfje;
        //                        isConvert = decimal.TryParse(zysfdjfh[3], out cxjzfje);
        //                        string sflb1 = zysfdjfh[4];
        //                        string sfxmdj = zysfdjfh[5];
        //                        string qezfbz = zysfdjfh[6];
        //                        decimal zlbl;
        //                        isConvert = decimal.TryParse(zysfdjfh[7], out zlbl);
        //                        decimal xj;
        //                        isConvert = decimal.TryParse(zysfdjfh[8], out xj);
        //                        string bz = zysfdjfh[9];
        //                        string strSql1 = string.Format(@"insert into ybcfmxscfhdr(jzlsh,jylsh,je,zlje,zfje,cxjzfje,sflb,sfxmdj,qezfbz,zlbl,xj,bz,grbh,xm,kh,cfh,ybcfh,yyxmdm,yyxmmc) 
        //                        values('{0}','{1}',{2},{3},{4},{5},'{6}','{7}','{8}',{9},{10},'{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}')"
        //                             , jzlsh, jylsh, je, zlje, zfje, cxjzfje, sflb1, sfxmdj, qezfbz, zlbl, xj, bz, grbh, xm, kh, jzlsh, liybcfh[j], liyyxmdm[j], liyyxmmc[j]);
        //                        lizysfdj.Add(strSql1);
        //                    }

        //                    string strSql2 = string.Format("update zy03d set z3ybup = '{0}' where z3ybup is null and z3zyno = '{1}'", jylsh, jzlsh);
        //                    lizysfdj.Add(strSql2);
        //                    object[] obj = lizysfdj.ToArray();
        //                    obj = CliUtils.CallMethod("sybdjdr", "BatExecuteSql", obj);

        //                    if (obj[1].ToString() == "1")
        //                    {
        //                        return new object[] { 0, 1, outputData.ToString() };
        //                    }
        //                    else
        //                    {
        //                        WriteLogFile("YBZYCFMXSB_Error.txt", obj[2].ToString());
        //                        i = YBCFMXSDCX(ybjzlsh, grbh, xm, kh, ZXBM);

        //                        if (i == 1)
        //                        {
        //                            return new object[] { 0, 0, "数据库操作失败,自动冲销医保处方明细成功,错误信息：" + obj[2].ToString() };
        //                        }
        //                        else
        //                        {
        //                            return new object[] { 0, 0, "数据库操作失败,自动冲销医保处方明细失败,错误信息：" + obj[2].ToString() };
        //                        }
        //                    }
        //                }
        //                else
        //                {
        //                    return new object[] { 0, 0, outputData.ToString() };
        //                }
        //            }
        //            catch (Exception error)
        //            {
        //                WriteLogFile("YBZYCFMXSB_CatchError.txt", error.Message);
        //                return new object[] { 0, 2, "Error:" + error.Message };
        //            }
        //        }
        #endregion 负交易模式

        #region 逐条住院处方明细上报20180101
        /// <summary>
        /// 逐条住院处方明细上报20180101
        /// </summary>
        /// <param>his就诊流水号, 截止时间(中途预结算需要传，格式：yyyy-MM-dd HH:mm:ss)</param>
        /// <returns>金额|自理金额|自费金额|超限价自付金额|收费类别|收费项目等级|全额自费标志|自理比例|限价|备注</returns>
        public static object[] ZTYBZYCFMXSBDR20180101(object[] objParam)
        {
            string czygh = CliUtils.fLoginUser;
            string ywzqh = YWZQH;
            string jbr = CliUtils.fUserName;
            string jzlsh = objParam[0].ToString();
            string endTime = "";

            if (objParam.Length > 1)
            {
                endTime = Convert.ToDateTime(objParam[1]).AddDays(1).ToString();
            }

            if (string.IsNullOrWhiteSpace(jzlsh))
            {
                return new object[] { 0, 0, "医保提示：his就诊流水号为空" };
            }

            DateTime dqsj = Convert.ToDateTime(Common.GetServerTime());
            string jylsh = dqsj.ToString("yyyyMMddHHmmss") + "-" + DDYLJGBH + "-" + jzlsh;
            string cfsjh = jzlsh + dqsj.ToString("HHmmss");
            bool isybzycfmxsb = false;
            string ybjzlsh = "";
            string grbh = "";
            string xm = "";
            string kh = "";

            try
            {
                string strSql = string.Format("select * from zy03dw a where a.z3zyno = '{0}' and left(a.z3endv, 1) = '1'", jzlsh);//0秒
                DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);

                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    return new object[] { 0, 0, "医保提示：his就诊流水号" + jzlsh + "已进行结算" };
                }

                strSql = string.Format("select ybjzlsh, grbh, xm, kh, ydrybz, tcqh from ybmzzydjdr a where a.jzlsh = '{0}' and a.cxbz = 1 and a.jzbz = 'z'", jzlsh);//0秒
                ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);

                if (ds == null || ds.Tables[0].Rows.Count == 0)
                {
                    return new object[] { 0, 0, "医保提示：医保无住院登记信息" };
                }

                DataRow dr1 = ds.Tables[0].Rows[0];
                ybjzlsh = dr1["ybjzlsh"].ToString();
                grbh = dr1["grbh"].ToString();
                xm = dr1["xm"].ToString();
                kh = dr1["kh"].ToString();
                string ydrybz = dr1["ydrybz"].ToString();
                ZXBM = ydrybz == "0" ? "0000" : dr1["tcqh"].ToString();

                //新增
                string strSqlxz = string.Format("select a.z1outd cyrq from zy01d a where left(a.z1endv, 1) = '8' and a.z1zyno = '{0}'", jzlsh);//0秒
                DataSet dsxz = CliUtils.ExecuteSql("sybdj", "cmd", strSqlxz, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);

                if (dsxz == null || dsxz.Tables[0].Rows.Count == 0)
                {
                    return new object[] { 0, 0, "his就诊流水号" + jzlsh + "未拖出床位" };
                }
                //end新增

                strSql = string.Format(@"select y.ybxmbh, y.ybxmmc, t.z3djxx dj, sum(case left(t.z3endv, 1) when '4' then -t.z3jzcs else t.z3jzcs end) sl
                , sum(case left(t.z3endv, 1) when '4' then -t.z3jzje else t.z3jzje end) je, t.z3item yyxmbh, t.z3name yyxmmc, t.z3empn ysdm
                , t.z3kdys ysxm, c.z1ksno ksno, c.z1ksnm zxks, t.z3sfno sfno, y.sfxmzldm ybsfxmzldm, y.sflbdm ybsflbdm, min(t.z3date) cfsj
                from zy03d t
                join zy01h c on t.z3comp = c.z1comp and t.z3zyno = c.z1zyno and t.z3ghno = c.z1ghno
                left join ybhisdzdr y on t.z3item = y.hisxmbh and y.scbz = 1
                where isnull(t.z3ybup, '') = '' and isnull(t.z3jshx, '') = '' and t.z3kind like '2%' and t.z3zyno = '{0}'", jzlsh);
                strSql += " and convert(datetime, t.z3date) >= '2018-01-01'";

                if (!string.IsNullOrWhiteSpace(endTime))
                {
                    strSql += " and convert(datetime, t.z3date) < '" + endTime + "'";
                }

                strSql += @" group by y.ybxmbh, y.ybxmmc, t.z3djxx, t.z3item, t.z3name, t.z3empn, t.z3kdys, c.z1ksno, c.z1ksnm, t.z3sfno, y.sfxmzldm, y.sflbdm
                having sum(case left(t.z3endv, 1) when '4' then -t.z3jzcs else t.z3jzcs end) < 0 and sum(case left(t.z3endv, 1) when '4' then -t.z3jzje else t.z3jzje end) < 0";
                ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);

                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    string tfxx = "";
                    string xmbhs = "";

                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        xmbhs += "'" + dr["yyxmbh"].ToString() + "',";
                        tfxx += ";医院项目编码：" + dr["yyxmbh"].ToString();
                        tfxx += "医院项目名称：" + dr["yyxmmc"].ToString();
                        tfxx += "数量：" + dr["sl"].ToString();
                    }

                    return new object[] { 0, 0, "医保提示：1月1号后产生过退1号以前的费用" + tfxx, xmbhs.TrimEnd(',') };
                }

                if (ZXBM != "0000")//X-00000-0000-0000医保不予支付的西药费用
                {
                    strSql = string.Format(@"select y.ybxmbh, y.ybxmmc, t.z3djxx dj, sum(case left(t.z3endv, 1) when '4' then -t.z3jzcs else t.z3jzcs end) sl
                    , sum(case left(t.z3endv, 1) when '4' then -t.z3jzje else t.z3jzje end) je, t.z3item yyxmbh, t.z3name yyxmmc, t.z3empn ysdm
                    , t.z3kdys ysxm, c.z1ksno ksno, c.z1ksnm zxks, t.z3sfno sfno, y.sfxmzldm ybsfxmzldm, y.sflbdm ybsflbdm, min(t.z3date) cfsj
                    from zy03d t
                    join zy01h c on t.z3comp = c.z1comp and t.z3zyno = c.z1zyno and t.z3ghno = c.z1ghno
                    left join ybhisdzdr20171231 y on t.z3item = y.hisxmbh and y.scbz = 1
                    where isnull(t.z3ybup, '') = '' and isnull(t.z3jshx, '') = '' and t.z3kind like '2%' and t.z3zyno = '{0}'", jzlsh);
                    strSql += " and convert(datetime, t.z3date) < '2018-01-01'";

                    if (!string.IsNullOrWhiteSpace(endTime))
                    {
                        strSql += " and convert(datetime, t.z3date) < '" + endTime + "'";
                    }

                    strSql += @" group by y.ybxmbh, y.ybxmmc, t.z3djxx, t.z3item, t.z3name, t.z3empn, t.z3kdys, c.z1ksno, c.z1ksnm, t.z3sfno, y.sfxmzldm, y.sflbdm
                    having sum(case left(t.z3endv, 1) when '4' then -t.z3jzcs else t.z3jzcs end) > 0 and sum(case left(t.z3endv, 1) when '4' then -t.z3jzje else t.z3jzje end) > 0";
                    strSql += string.Format(@" union all select case y.ybxmbh when '39199999990001000001' then 'Z-00000-00000' when '61048000000500000000' then '61048000000400000000' else y.ybxmbh end ybxmbh, case y.ybxmbh when '39199999990001000001' then '医保不予支付的中成药费用' when '61048000000500000000' then '人工煎药' else y.ybxmmc end ybxmmc, t.z3djxx dj, sum(case left(t.z3endv, 1) when '4' then -t.z3jzcs else t.z3jzcs end) sl
                    , sum(case left(t.z3endv, 1) when '4' then -t.z3jzje else t.z3jzje end) je, t.z3item yyxmbh, t.z3name yyxmmc, t.z3empn ysdm
                    , t.z3kdys ysxm, c.z1ksno ksno, c.z1ksnm zxks, t.z3sfno sfno, y.sfxmzldm ybsfxmzldm, y.sflbdm ybsflbdm, min(t.z3date) cfsj
                    from zy03d t
                    join zy01h c on t.z3comp = c.z1comp and t.z3zyno = c.z1zyno and t.z3ghno = c.z1ghno
                    left join ybhisdzdr y on t.z3item = y.hisxmbh and y.scbz = 1
                    where isnull(t.z3ybup, '') = '' and isnull(t.z3jshx, '') = '' and t.z3kind like '2%' and t.z3zyno = '{0}'", jzlsh);
                    strSql += " and convert(datetime, t.z3date) >= '2018-01-01'";

                    if (!string.IsNullOrWhiteSpace(endTime))
                    {
                        strSql += " and convert(datetime, t.z3date) < '" + endTime + "'";
                    }

                    strSql += @" group by case y.ybxmbh when '39199999990001000001' then 'Z-00000-00000' when '61048000000500000000' then '61048000000400000000' else y.ybxmbh end, case y.ybxmbh when '39199999990001000001' then '医保不予支付的中成药费用' when '61048000000500000000' then '人工煎药' else y.ybxmmc end, t.z3djxx, t.z3item, t.z3name, t.z3empn, t.z3kdys, c.z1ksno, c.z1ksnm, t.z3sfno, y.sfxmzldm, y.sflbdm
                    having sum(case left(t.z3endv, 1) when '4' then -t.z3jzcs else t.z3jzcs end) > 0 and sum(case left(t.z3endv, 1) when '4' then -t.z3jzje else t.z3jzje end) > 0";//已优化
                }
                else
                {
                    strSql = string.Format(@"select y.ybxmbh, y.ybxmmc, t.z3djxx dj, sum(case left(t.z3endv, 1) when '4' then -t.z3jzcs else t.z3jzcs end) sl
                    , sum(case left(t.z3endv, 1) when '4' then -t.z3jzje else t.z3jzje end) je, t.z3item yyxmbh, t.z3name yyxmmc, t.z3empn ysdm
                    , t.z3kdys ysxm, c.z1ksno ksno, c.z1ksnm zxks, t.z3sfno sfno, y.sfxmzldm ybsfxmzldm, y.sflbdm ybsflbdm, min(t.z3date) cfsj
                    from zy03d t
                    join zy01h c on t.z3comp = c.z1comp and t.z3zyno = c.z1zyno and t.z3ghno = c.z1ghno
                    left join ybhisdzdr20171231 y on t.z3item = y.hisxmbh and y.scbz = 1
                    where isnull(t.z3ybup, '') = '' and isnull(t.z3jshx, '') = '' and t.z3kind like '2%' and t.z3zyno = '{0}'", jzlsh);
                    strSql += " and convert(datetime, t.z3date) < '2018-01-01'";

                    if (!string.IsNullOrWhiteSpace(endTime))
                    {
                        strSql += " and convert(datetime, t.z3date) < '" + endTime + "'";
                    }

                    strSql += @" group by y.ybxmbh, y.ybxmmc, t.z3djxx, t.z3item, t.z3name, t.z3empn, t.z3kdys, c.z1ksno, c.z1ksnm, t.z3sfno, y.sfxmzldm, y.sflbdm
                    having sum(case left(t.z3endv, 1) when '4' then -t.z3jzcs else t.z3jzcs end) > 0 and sum(case left(t.z3endv, 1) when '4' then -t.z3jzje else t.z3jzje end) > 0";
                    strSql += string.Format(@" union all select y.ybxmbh, y.ybxmmc, t.z3djxx dj, sum(case left(t.z3endv, 1) when '4' then -t.z3jzcs else t.z3jzcs end) sl
                    , sum(case left(t.z3endv, 1) when '4' then -t.z3jzje else t.z3jzje end) je, t.z3item yyxmbh, t.z3name yyxmmc, t.z3empn ysdm
                    , t.z3kdys ysxm, c.z1ksno ksno, c.z1ksnm zxks, t.z3sfno sfno, y.sfxmzldm ybsfxmzldm, y.sflbdm ybsflbdm, min(t.z3date) cfsj
                    from zy03d t
                    join zy01h c on t.z3comp = c.z1comp and t.z3zyno = c.z1zyno and t.z3ghno = c.z1ghno
                    left join ybhisdzdr y on t.z3item = y.hisxmbh and y.scbz = 1
                    where isnull(t.z3ybup, '') = '' and isnull(t.z3jshx, '') = '' and t.z3kind like '2%' and t.z3zyno = '{0}'", jzlsh);
                    strSql += " and convert(datetime, t.z3date) >= '2018-01-01'";

                    if (!string.IsNullOrWhiteSpace(endTime))
                    {
                        strSql += " and convert(datetime, t.z3date) < '" + endTime + "'";
                    }

                    strSql += @" group by y.ybxmbh, y.ybxmmc, t.z3djxx, t.z3item, t.z3name, t.z3empn, t.z3kdys, c.z1ksno, c.z1ksnm, t.z3sfno, y.sfxmzldm, y.sflbdm
                    having sum(case left(t.z3endv, 1) when '4' then -t.z3jzcs else t.z3jzcs end) > 0 and sum(case left(t.z3endv, 1) when '4' then -t.z3jzje else t.z3jzje end) > 0";//已优化
                }

                ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                List<string> liybcfh = new List<string>();
                List<string> liyyxmdm = new List<string>();
                List<string> liyyxmmc = new List<string>();
                List<string> lizysfdj = new List<string>();
                StringBuilder rc = new StringBuilder();
                bool isfpsc = false;
                StringBuilder wdzxms = new StringBuilder();

                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    DataTable dt = ds.Tables[0];
                    int dtcount = dt.Rows.Count;

                    for (int k = 0; k < dtcount; k++)
                    {
                        DataRow dr = dt.Rows[k];

                        if (dr["ybxmbh"] == DBNull.Value)
                        {
                            wdzxms.Append("医保提示：项目代码：[" + dr["yyxmbh"].ToString() + "]，名称：[" + dr["yyxmmc"].ToString() + "]未对照或未上传，不能上传费用！");
                        }
                    }
                }

                if (wdzxms.Length > 0)
                {
                    return new object[] { 0, 0, wdzxms };
                }

                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    DataTable dt = ds.Tables[0];
                    int dtcount = dt.Rows.Count;

                    for (int k = 0; k < dtcount; k++)
                    {
                        DataRow dr = dt.Rows[k];

                        if (dr["ksno"] == DBNull.Value || dr["ksno"].ToString() == "")
                        {
                            return new object[] { 0, 0, "医保提示：有科室编号为空的记录" };
                        }
                        else if (dr["zxks"] == DBNull.Value || dr["zxks"].ToString() == "")
                        {
                            return new object[] { 0, 0, "医保提示：无此科室编号[" + dr["ksno"].ToString() + "]" };
                        }
                        else
                        {
                            string ybsfxmzldm = dr["ybsfxmzldm"].ToString();
                            string ybsflbdm = dr["ybsflbdm"].ToString();
                            string yyxmbh = dr["yyxmbh"].ToString();
                            string ybxmbh = dr["ybxmbh"].ToString();
                            string yyxmmc = dr["yyxmmc"].ToString();
                            decimal dj = Convert.ToDecimal(dr["dj"]);
                            decimal sl = Convert.ToDecimal(dr["sl"]);
                            decimal je = Convert.ToDecimal(dr["je"]);
                            decimal mcyl = 1;
                            string ysdm = string.IsNullOrWhiteSpace(dr["ysdm"].ToString()) ? "010001" : dr["ysdm"].ToString();
                            string ysxm = string.IsNullOrWhiteSpace(dr["ysxm"].ToString()) ? "严东标" : dr["ysxm"].ToString();
                            string ksdm = dr["ksno"].ToString();
                            string ksmc = dr["zxks"].ToString();
                            string ypjldw = "-";
                            string ybcfh = cfsjh + k.ToString();
                            string cydffbz = "0";

                            if (ybsfxmzldm == "1")
                            {
                                cydffbz = "1";
                            }

                            if (dr["sfno"].ToString() == "01" || dr["sfno"].ToString() == "02" || dr["sfno"].ToString() == "03")
                            {
                                ypjldw = "粒";
                            }

                            string cfsj = Convert.ToDateTime(dr["cfsj"]).ToString("yyyyMMddHHmmss");
                            liybcfh.Add(ybcfh);
                            liyyxmdm.Add(yyxmbh);
                            liyyxmmc.Add(yyxmmc);
                            string strSql1 = string.Format(@"insert into ybcfmxscindr(jzlsh, jylsh, sfxmzl, sflb, ybcfh, cfrq, yysfxmbm, sfxmzxbm
                            , yysfxmmc, dj, sl, je, jx, gg, mcyl, sypc, ysbm, ysxm, yf, dw, ksbh, ksmc, zxts, cydffbz, jbr, ypjldw, qezfbz, grbh, xm, kh)
                            values ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', {9}, {10}, {11}, '{12}', '{13}', {14}, '{15}'
                            , '{16}', '{17}', '{18}', '{19}', '{20}', '{21}', '{22}', '{23}', '{24}', '{25}', '{26}', '{27}', '{28}', '{29}')"
                            , jzlsh, jylsh, ybsfxmzldm, ybsflbdm, ybcfh, cfsj, yyxmbh, ybxmbh, yyxmmc, Math.Round(dj, 4).ToString(), Math.Round(sl, 2).ToString(), Math.Round(je, 4).ToString()
                            , "101", "", mcyl, "", ysdm, ysxm, "", "", ksdm, ksmc, "", cydffbz, jbr, ypjldw, "", grbh, xm, kh);
                            lizysfdj.Add(strSql1);
                            rc.Append(ybjzlsh + "|" + ybsfxmzldm + "|" + ybsflbdm + "|" + ybcfh + "|" + cfsj + "|" + yyxmbh
                            + "|" + ybxmbh + "|" + yyxmmc + "|" + Math.Round(dj, 4).ToString() + "|" + Math.Round(sl, 2).ToString() + "|" + Math.Round(je, 4).ToString()
                            + "|101||" + mcyl.ToString() + "||" + ysdm + "|" + ysxm + "|||" + ksdm + "|" + ksmc + "||" + cydffbz + "|"
                            + jbr + "|" + ypjldw + "||" + grbh + "|" + xm + "|" + kh + "|$");
                            int scjls = 1;

                            #region 分批上传
                            if (dtcount > scjls)
                            {
                                isfpsc = true;

                                if ((k + 1) % scjls == 0)
                                {
                                    StringBuilder inputData = new StringBuilder(string.Format("{0}^{1}^{2}^{3}^{4}^{5}^{6}^{7}^", "2310", DDYLJGBH, czygh, ywzqh, jylsh, ZXBM, rc.ToString().TrimEnd('$'), "1"));
                                    StringBuilder outputData = new StringBuilder(100000);
                                    int i = BUSINESS_HANDLE(inputData, outputData);

                                    if (i == 0)
                                    {
                                        isybzycfmxsb = true;
                                        string[] zysfdjfhs = outputData.ToString().Split('^')[2].TrimEnd('$').Split('$');

                                        if (ydrybz == "0")
                                        {
                                            //begin判断返回记录数是否和上传记录数相等
                                            if (zysfdjfhs.Length != liybcfh.Count || zysfdjfhs.Length != liyyxmdm.Count || zysfdjfhs.Length != liyyxmmc.Count)
                                            {
                                                i = ChangeYBCFMXSDCXDR(new object[]{ybjzlsh, grbh, xm, kh, ZXBM});

                                                if (i == 1)
                                                {
                                                    return new object[] { 0, 0, "医保提示：医保返回记录数和上传记录数不相等,自动撤销医保处方明细成功" };
                                                }
                                                else
                                                {
                                                    return new object[] { 0, 0, "医保提示：医保返回记录数和上传记录数不相等,自动撤销医保处方明细失败" };
                                                }
                                            }
                                            //end判断返回记录数是否和上传记录数相等

                                            for (int j = 0; j < zysfdjfhs.Length; j++)
                                            {
                                                string[] zysfdjfh = zysfdjfhs[j].Split('|');
                                                bool isConvert = decimal.TryParse(zysfdjfh[0], out je);
                                                decimal zlje;
                                                isConvert = decimal.TryParse(zysfdjfh[1], out zlje);
                                                decimal zfje;
                                                isConvert = decimal.TryParse(zysfdjfh[2], out zfje);
                                                decimal cxjzfje;
                                                isConvert = decimal.TryParse(zysfdjfh[3], out cxjzfje);
                                                string sflb1 = zysfdjfh[4];
                                                string sfxmdj = zysfdjfh[5];
                                                string qezfbz = zysfdjfh[6];
                                                decimal zlbl;
                                                isConvert = decimal.TryParse(zysfdjfh[7], out zlbl);
                                                decimal xj;
                                                isConvert = decimal.TryParse(zysfdjfh[8], out xj);
                                                string bz = zysfdjfh[9];
                                                strSql1 = string.Format(@"insert into ybcfmxscfhdr(jzlsh, jylsh, je, zlje, zfje, cxjzfje, sflb, sfxmdj, qezfbz, zlbl, xj, bz, grbh, xm, kh, cfh, ybcfh, yyxmdm, yyxmmc) 
                                                values('{0}', '{1}', {2}, {3}, {4}, {5}, '{6}', '{7}', '{8}', {9}, {10}, '{11}', '{12}', '{13}', '{14}', '{15}', '{16}', '{17}', '{18}')"
                                                , jzlsh, jylsh, je, zlje, zfje, cxjzfje, sflb1, sfxmdj, qezfbz, zlbl, xj, bz, grbh, xm, kh, jzlsh, liybcfh[j], liyyxmdm[j], liyyxmmc[j]);
                                                lizysfdj.Add(strSql1);
                                            }
                                        }

                                        string strSql2 = string.Format("update zy03d set z3ybup = '{0}' where z3comp = '{1}' and z3zyno = '{2}' and z3kind like '2%' and z3ybup is null", jylsh, CliUtils.fSiteCode, jzlsh);//0秒

                                        if (!string.IsNullOrWhiteSpace(endTime))
                                        {
                                            strSql2 += " and convert(datetime, z3date) <= '" + endTime + "'";
                                        }

                                        lizysfdj.Add(strSql2);
                                        object[] obj = lizysfdj.ToArray();
                                        obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);

                                        if (obj[1].ToString() == "1")
                                        {
                                            rc.Clear();
                                            liybcfh.Clear();
                                            liyyxmdm.Clear();
                                            liyyxmmc.Clear();
                                            lizysfdj.Clear();

                                            if (k + 1 == dtcount)
                                            {
                                                return new object[] { 0, 1, outputData };
                                            }
                                        }
                                        else
                                        {
                                            Common.WriteYBLog(obj[2].ToString());
                                            i = ChangeYBCFMXSDCXDR(new object[]{ybjzlsh, grbh, xm, kh, ZXBM});

                                            if (i == 1)
                                            {
                                                return new object[] { 0, 0, "医保提示：his数据库操作失败,自动撤销医保住院处方明细成功,错误信息：" + obj[2].ToString() };
                                            }
                                            else
                                            {
                                                return new object[] { 0, 0, "医保提示：his数据库操作失败,自动撤销医保住院处方明细失败,错误信息：" + obj[2].ToString() };
                                            }
                                        }
                                    }
                                    else if (i == -1)
                                    {
                                        i = ChangeYBCFMXSDCXDR(new object[]{ybjzlsh, grbh, xm, kh, ZXBM});

                                        if (i == 1)
                                        {
                                            return new object[] { 0, -1, "医保提示：医保系统级别错误，自动撤销医保住院处方明细上传成功，" + outputData.ToString() };
                                        }
                                        else
                                        {
                                            return new object[] { 0, -1, "医保提示：医保系统级别错误，自动撤销医保住院处方明细上传失败，" + outputData.ToString() };
                                        }
                                    }
                                    else
                                    {
                                        return new object[] { 0, -2, "医保提示：医保住院处方明细上传业务级别或未知错误，" + outputData.ToString() };
                                    }
                                }
                                else
                                {
                                    if (dtcount % scjls > 0 && k + 1 == dtcount)
                                    {
                                        StringBuilder inputData = new StringBuilder(string.Format("{0}^{1}^{2}^{3}^{4}^{5}^{6}^{7}^", "2310", DDYLJGBH, czygh, ywzqh, jylsh, ZXBM, rc.ToString().TrimEnd('$'), "1"));
                                        StringBuilder outputData = new StringBuilder(100000);
                                        int i = BUSINESS_HANDLE(inputData, outputData);

                                        if (i == 0)
                                        {
                                            isybzycfmxsb = true;
                                            string[] zysfdjfhs = outputData.ToString().Split('^')[2].TrimEnd('$').Split('$');

                                            if (ydrybz == "0")
                                            {
                                                //begin判断返回记录数是否和上传记录数相等
                                                if (zysfdjfhs.Length != liybcfh.Count || zysfdjfhs.Length != liyyxmdm.Count || zysfdjfhs.Length != liyyxmmc.Count)
                                                {
                                                    i = ChangeYBCFMXSDCXDR(new object[]{ybjzlsh, grbh, xm, kh, ZXBM});

                                                    if (i == 1)
                                                    {
                                                        return new object[] { 0, 0, "医保提示：医保返回记录数和上传记录数不相等,自动撤销医保处方明细成功" };
                                                    }
                                                    else
                                                    {
                                                        return new object[] { 0, 0, "医保提示：医保返回记录数和上传记录数不相等,自动撤销医保处方明细失败" };
                                                    }
                                                }
                                                //end判断返回记录数是否和上传记录数相等

                                                for (int j = 0; j < zysfdjfhs.Length; j++)
                                                {
                                                    string[] zysfdjfh = zysfdjfhs[j].Split('|');
                                                    bool isConvert = decimal.TryParse(zysfdjfh[0], out je);
                                                    decimal zlje;
                                                    isConvert = decimal.TryParse(zysfdjfh[1], out zlje);
                                                    decimal zfje;
                                                    isConvert = decimal.TryParse(zysfdjfh[2], out zfje);
                                                    decimal cxjzfje;
                                                    isConvert = decimal.TryParse(zysfdjfh[3], out cxjzfje);
                                                    string sflb1 = zysfdjfh[4];
                                                    string sfxmdj = zysfdjfh[5];
                                                    string qezfbz = zysfdjfh[6];
                                                    decimal zlbl;
                                                    isConvert = decimal.TryParse(zysfdjfh[7], out zlbl);
                                                    decimal xj;
                                                    isConvert = decimal.TryParse(zysfdjfh[8], out xj);
                                                    string bz = zysfdjfh[9];
                                                    strSql1 = string.Format(@"insert into ybcfmxscfhdr(jzlsh, jylsh, je, zlje, zfje, cxjzfje, sflb, sfxmdj, qezfbz, zlbl, xj, bz, grbh, xm, kh, cfh, ybcfh, yyxmdm, yyxmmc) 
                                                    values('{0}', '{1}', {2}, {3}, {4}, {5}, '{6}', '{7}', '{8}', {9}, {10}, '{11}', '{12}', '{13}', '{14}', '{15}', '{16}', '{17}', '{18}')"
                                                    , jzlsh, jylsh, je, zlje, zfje, cxjzfje, sflb1, sfxmdj, qezfbz, zlbl, xj, bz, grbh, xm, kh, jzlsh, liybcfh[j], liyyxmdm[j], liyyxmmc[j]);
                                                    lizysfdj.Add(strSql1);
                                                }
                                            }

                                            string strSql2 = string.Format("update zy03d set z3ybup = '{0}' where z3comp = '{1}' and z3zyno = '{2}' and z3kind like '2%' and z3ybup is null", jylsh, CliUtils.fSiteCode, jzlsh);//0秒

                                            if (!string.IsNullOrWhiteSpace(endTime))
                                            {
                                                strSql2 += " and convert(datetime, z3date) <= '" + endTime + "'";
                                            }

                                            lizysfdj.Add(strSql2);
                                            object[] obj = lizysfdj.ToArray();
                                            obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);

                                            if (obj[1].ToString() == "1")
                                            {
                                                rc.Clear();
                                                liybcfh.Clear();
                                                liyyxmdm.Clear();
                                                liyyxmmc.Clear();
                                                lizysfdj.Clear();

                                                if (k + 1 == dtcount)
                                                {
                                                    return new object[] { 0, 1, outputData };
                                                }
                                            }
                                            else
                                            {
                                                Common.WriteYBLog(obj[2].ToString());
                                                i = ChangeYBCFMXSDCXDR(new object[]{ybjzlsh, grbh, xm, kh, ZXBM});

                                                if (i == 1)
                                                {
                                                    return new object[] { 0, 0, "医保提示：his数据库操作失败,自动撤销医保住院处方明细成功,错误信息：" + obj[2].ToString() };
                                                }
                                                else
                                                {
                                                    return new object[] { 0, 0, "医保提示：his数据库操作失败,自动撤销医保住院处方明细失败,错误信息：" + obj[2].ToString() };
                                                }
                                            }
                                        }
                                        else if (i == -1)
                                        {
                                            i = ChangeYBCFMXSDCXDR(new object[]{ybjzlsh, grbh, xm, kh, ZXBM});

                                            if (i == 1)
                                            {
                                                return new object[] { 0, -1, "医保提示：医保系统级别错误，自动撤销医保住院处方明细上传成功，" + outputData.ToString() };
                                            }
                                            else
                                            {
                                                return new object[] { 0, -1, "医保提示：医保系统级别错误，自动撤销医保住院处方明细上传失败，" + outputData.ToString() };
                                            }
                                        }
                                        else
                                        {
                                            return new object[] { 0, -2, "医保提示：医保住院处方明细上传业务级别或未知错误，" + outputData.ToString() };
                                        }
                                    }
                                }
                            }
                            #endregion 分批上传
                        }
                    }
                }
                else
                {
                    return new object[] { 0, 0, "医保提示：医保本次无住院费用上传" };
                }

                if (!isfpsc)
                {
                    StringBuilder inputData = new StringBuilder(string.Format("{0}^{1}^{2}^{3}^{4}^{5}^{6}^{7}^", "2310", DDYLJGBH, czygh, ywzqh, jylsh, ZXBM, rc.ToString().TrimEnd('$'), "1"));
                    StringBuilder outputData = new StringBuilder(100000);
                    int i = BUSINESS_HANDLE(inputData, outputData);

                    if (i == 0)
                    {
                        isybzycfmxsb = true;
                        string[] zysfdjfhs = outputData.ToString().Split('^')[2].TrimEnd('$').Split('$');

                        if (ydrybz == "0")
                        {
                            //begin判断返回记录数是否和上传记录数相等
                            if (zysfdjfhs.Length != liybcfh.Count || zysfdjfhs.Length != liyyxmdm.Count || zysfdjfhs.Length != liyyxmmc.Count)
                            {
                                i = ChangeYBCFMXSDCXDR(new object[]{ybjzlsh, grbh, xm, kh, ZXBM});

                                if (i == 1)
                                {
                                    return new object[] { 0, 0, "医保提示：医保返回记录数和上传记录数不相等,自动撤销医保处方明细成功" };
                                }
                                else
                                {
                                    return new object[] { 0, 0, "医保提示：医保返回记录数和上传记录数不相等,自动撤销医保处方明细失败" };
                                }
                            }
                            //end判断返回记录数是否和上传记录数相等

                            for (int j = 0; j < zysfdjfhs.Length; j++)
                            {
                                string[] zysfdjfh = zysfdjfhs[j].Split('|');
                                decimal je;
                                bool isConvert = decimal.TryParse(zysfdjfh[0], out je);
                                decimal zlje;
                                isConvert = decimal.TryParse(zysfdjfh[1], out zlje);
                                decimal zfje;
                                isConvert = decimal.TryParse(zysfdjfh[2], out zfje);
                                decimal cxjzfje;
                                isConvert = decimal.TryParse(zysfdjfh[3], out cxjzfje);
                                string sflb1 = zysfdjfh[4];
                                string sfxmdj = zysfdjfh[5];
                                string qezfbz = zysfdjfh[6];
                                decimal zlbl;
                                isConvert = decimal.TryParse(zysfdjfh[7], out zlbl);
                                decimal xj;
                                isConvert = decimal.TryParse(zysfdjfh[8], out xj);
                                string bz = zysfdjfh[9];
                                string strSql1 = string.Format(@"insert into ybcfmxscfhdr(jzlsh, jylsh, je, zlje, zfje, cxjzfje, sflb, sfxmdj, qezfbz, zlbl, xj, bz, grbh, xm, kh, cfh, ybcfh, yyxmdm, yyxmmc) 
                                values('{0}', '{1}', {2}, {3}, {4}, {5}, '{6}', '{7}', '{8}', {9}, {10}, '{11}', '{12}', '{13}', '{14}', '{15}', '{16}', '{17}', '{18}')"
                                , jzlsh, jylsh, je, zlje, zfje, cxjzfje, sflb1, sfxmdj, qezfbz, zlbl, xj, bz, grbh, xm, kh, jzlsh, liybcfh[j], liyyxmdm[j], liyyxmmc[j]);
                                lizysfdj.Add(strSql1);
                            }
                        }

                        string strSql2 = string.Format("update zy03d set z3ybup = '{0}' where z3comp = '{1}' and z3zyno = '{2}' and z3kind like '2%' and z3ybup is null", jylsh, CliUtils.fSiteCode, jzlsh);//0秒

                        if (!string.IsNullOrWhiteSpace(endTime))
                        {
                            strSql2 += " and convert(datetime, z3date) <= '" + endTime + "'";
                        }

                        lizysfdj.Add(strSql2);
                        object[] obj = lizysfdj.ToArray();
                        obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);

                        if (obj[1].ToString() == "1")
                        {
                            return new object[] { 0, 1, outputData };
                        }
                        else
                        {
                            Common.WriteYBLog(obj[2].ToString());
                            i = ChangeYBCFMXSDCXDR(new object[]{ybjzlsh, grbh, xm, kh, ZXBM});

                            if (i == 1)
                            {
                                return new object[] { 0, 0, "医保提示：his数据库操作失败,自动撤销医保住院处方明细成功,错误信息：" + obj[2].ToString() };
                            }
                            else
                            {
                                return new object[] { 0, 0, "医保提示：his数据库操作失败,自动撤销医保住院处方明细失败,错误信息：" + obj[2].ToString() };
                            }
                        }
                    }
                    else if (i == -1)
                    {
                        i = ChangeYBCFMXSDCXDR(new object[]{ybjzlsh, grbh, xm, kh, ZXBM});

                        if (i == 1)
                        {
                            return new object[] { 0, -1, "医保提示：医保系统级别错误，自动撤销医保住院处方明细上传成功，" + outputData.ToString() };
                        }
                        else
                        {
                            return new object[] { 0, -1, "医保提示：医保系统级别错误，自动撤销医保住院处方明细上传失败，" + outputData.ToString() };
                        }
                    }
                    else
                    {
                        return new object[] { 0, -2, "医保提示：医保住院处方明细上传业务级别或未知错误，" + outputData.ToString() };
                    }
                }
                else
                {
                    return new object[] { 0, -3, "医保提示：既不是一次也不是分批上传，请联系管理员" };
                }
            }
            catch (Exception error)
            {
                Common.WriteYBLog(error.ToString());

                if (isybzycfmxsb)
                {
                    int i = ChangeYBCFMXSDCXDR(new object[]{ybjzlsh, grbh, xm, kh, ZXBM});

                    if (i == 1)
                    {
                        return new object[] { 0, 2, "医保提示：非医保异常，自动撤销医保住院处方明细上传成功，" + error.ToString() };
                    }
                    else
                    {
                        return new object[] { 0, 2, "医保提示：非医保异常，自动撤销医保住院处方明细上传失败，" + error.ToString() };
                    }
                }

                return new object[] { 0, 2, "Error:" + error.ToString() };
            }
        }
        #endregion 逐条住院处方明细上报20180101

        #region 住院处方明细上报20180101
        /// <summary>
        /// 住院处方明细上报20180101
        /// </summary>
        /// <param>his就诊流水号, 截止时间(中途预结算需要传，格式：yyyy-MM-dd HH:mm:ss)</param>
        /// <returns>金额|自理金额|自费金额|超限价自付金额|收费类别|收费项目等级|全额自费标志|自理比例|限价|备注</returns>
        public static object[] YBZYCFMXSBDR20180101(object[] objParam)
        {
            string czygh = CliUtils.fLoginUser;
            string ywzqh = YWZQH;
            string jbr = CliUtils.fUserName;
            string jzlsh = objParam[0].ToString();
            string endTime = "";

            if (objParam.Length > 1)
            {
                endTime = Convert.ToDateTime(objParam[1]).AddDays(1).ToString();
            }

            if (string.IsNullOrWhiteSpace(jzlsh))
            {
                return new object[] { 0, 0, "医保提示：his就诊流水号为空" };
            }

            DateTime dqsj = Convert.ToDateTime(Common.GetServerTime());
            string jylsh = dqsj.ToString("yyyyMMddHHmmss") + "-" + DDYLJGBH + "-" + jzlsh;
            string cfsjh = jzlsh + dqsj.ToString("HHmmss");
            bool isybzycfmxsb = false;
            string ybjzlsh = "";
            string grbh = "";
            string xm = "";
            string kh = "";

            try
            {
                string strSql = string.Format("select * from zy03dw a where a.z3zyno = '{0}' and left(a.z3endv, 1) = '1'", jzlsh);//0秒
                DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);

                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    return new object[] { 0, 0, "医保提示：his就诊流水号" + jzlsh + "已进行结算" };
                }

                strSql = string.Format("select ybjzlsh, grbh, xm, kh, ydrybz, tcqh from ybmzzydjdr a where a.jzlsh = '{0}' and a.cxbz = 1 and a.jzbz = 'z'", jzlsh);//0秒
                ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);

                if (ds == null || ds.Tables[0].Rows.Count == 0)
                {
                    return new object[] { 0, 0, "医保提示：医保无住院登记信息" };
                }

                DataRow dr1 = ds.Tables[0].Rows[0];
                ybjzlsh = dr1["ybjzlsh"].ToString();
                grbh = dr1["grbh"].ToString();
                xm = dr1["xm"].ToString();
                kh = dr1["kh"].ToString();
                string ydrybz = dr1["ydrybz"].ToString();
                ZXBM = ydrybz == "0" ? "0000" : dr1["tcqh"].ToString();

                //新增
                string strSqlxz = string.Format("select a.z1outd cyrq from zy01d a where left(a.z1endv, 1) = '8' and a.z1zyno = '{0}'", jzlsh);//0秒
                DataSet dsxz = CliUtils.ExecuteSql("sybdj", "cmd", strSqlxz, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);

                if (dsxz == null || dsxz.Tables[0].Rows.Count == 0)
                {
                    return new object[] { 0, 0, "his就诊流水号" + jzlsh + "未拖出床位" };
                }
                //end新增

                strSql = string.Format(@"select y.ybxmbh, y.ybxmmc, t.z3djxx dj, sum(case left(t.z3endv, 1) when '4' then -t.z3jzcs else t.z3jzcs end) sl
                , sum(case left(t.z3endv, 1) when '4' then -t.z3jzje else t.z3jzje end) je, t.z3item yyxmbh, t.z3name yyxmmc, t.z3empn ysdm
                , t.z3kdys ysxm, c.z1ksno ksno, c.z1ksnm zxks, t.z3sfno sfno, y.sfxmzldm ybsfxmzldm, y.sflbdm ybsflbdm, min(t.z3date) cfsj
                from zy03d t
                join zy01h c on t.z3comp = c.z1comp and t.z3zyno = c.z1zyno and t.z3ghno = c.z1ghno
                left join ybhisdzdr y on t.z3item = y.hisxmbh and y.scbz = 1
                where isnull(t.z3ybup, '') = '' and isnull(t.z3jshx, '') = '' and t.z3kind like '2%' and t.z3zyno = '{0}'", jzlsh);
                strSql += " and convert(datetime, t.z3date) >= '2018-01-01'";

                if (!string.IsNullOrWhiteSpace(endTime))
                {
                    strSql += " and convert(datetime, t.z3date) < '" + endTime + "'";
                }

                strSql += @" group by y.ybxmbh, y.ybxmmc, t.z3djxx, t.z3item, t.z3name, t.z3empn, t.z3kdys, c.z1ksno, c.z1ksnm, t.z3sfno, y.sfxmzldm, y.sflbdm
                having sum(case left(t.z3endv, 1) when '4' then -t.z3jzcs else t.z3jzcs end) < 0 and sum(case left(t.z3endv, 1) when '4' then -t.z3jzje else t.z3jzje end) < 0";
                ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);

                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    string tfxx = "";
                    string xmbhs = "";

                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        xmbhs += "'" + dr["yyxmbh"].ToString() + "',";
                        tfxx += ";医院项目编码：" + dr["yyxmbh"].ToString();
                        tfxx += "医院项目名称：" + dr["yyxmmc"].ToString();
                        tfxx += "数量：" + dr["sl"].ToString();
                    }

                    return new object[] { 0, 0, "医保提示：1月1号后产生过退1号以前的费用" + tfxx, xmbhs.TrimEnd(',') };
                }

                if (ZXBM != "0000")//X-00000-0000-0000医保不予支付的西药费用
                {
                    strSql = string.Format(@"select y.ybxmbh, y.ybxmmc, t.z3djxx dj, sum(case left(t.z3endv, 1) when '4' then -t.z3jzcs else t.z3jzcs end) sl
                    , sum(case left(t.z3endv, 1) when '4' then -t.z3jzje else t.z3jzje end) je, t.z3item yyxmbh, t.z3name yyxmmc, t.z3empn ysdm
                    , t.z3kdys ysxm, c.z1ksno ksno, c.z1ksnm zxks, t.z3sfno sfno, y.sfxmzldm ybsfxmzldm, y.sflbdm ybsflbdm, min(t.z3date) cfsj
                    from zy03d t
                    join zy01h c on t.z3comp = c.z1comp and t.z3zyno = c.z1zyno and t.z3ghno = c.z1ghno
                    left join ybhisdzdr20171231 y on t.z3item = y.hisxmbh and y.scbz = 1
                    where isnull(t.z3ybup, '') = '' and isnull(t.z3jshx, '') = '' and t.z3kind like '2%' and t.z3zyno = '{0}'", jzlsh);
                    strSql += " and convert(datetime, t.z3date) < '2018-01-01'";

                    if (!string.IsNullOrWhiteSpace(endTime))
                    {
                        strSql += " and convert(datetime, t.z3date) < '" + endTime + "'";
                    }

                    strSql += @" group by y.ybxmbh, y.ybxmmc, t.z3djxx, t.z3item, t.z3name, t.z3empn, t.z3kdys, c.z1ksno, c.z1ksnm, t.z3sfno, y.sfxmzldm, y.sflbdm
                    having sum(case left(t.z3endv, 1) when '4' then -t.z3jzcs else t.z3jzcs end) > 0 and sum(case left(t.z3endv, 1) when '4' then -t.z3jzje else t.z3jzje end) > 0";
                    strSql += string.Format(@" union all select case y.ybxmbh when '39199999990001000001' then 'Z-00000-00000' when '61048000000500000000' then '61048000000400000000' else y.ybxmbh end ybxmbh, case y.ybxmbh when '39199999990001000001' then '医保不予支付的中成药费用' when '61048000000500000000' then '人工煎药' else y.ybxmmc end ybxmmc, t.z3djxx dj, sum(case left(t.z3endv, 1) when '4' then -t.z3jzcs else t.z3jzcs end) sl
                    , sum(case left(t.z3endv, 1) when '4' then -t.z3jzje else t.z3jzje end) je, t.z3item yyxmbh, t.z3name yyxmmc, t.z3empn ysdm
                    , t.z3kdys ysxm, c.z1ksno ksno, c.z1ksnm zxks, t.z3sfno sfno, y.sfxmzldm ybsfxmzldm, y.sflbdm ybsflbdm, min(t.z3date) cfsj
                    from zy03d t
                    join zy01h c on t.z3comp = c.z1comp and t.z3zyno = c.z1zyno and t.z3ghno = c.z1ghno
                    left join ybhisdzdr y on t.z3item = y.hisxmbh and y.scbz = 1
                    where isnull(t.z3ybup, '') = '' and isnull(t.z3jshx, '') = '' and t.z3kind like '2%' and t.z3zyno = '{0}'", jzlsh);
                    strSql += " and convert(datetime, t.z3date) >= '2018-01-01'";

                    if (!string.IsNullOrWhiteSpace(endTime))
                    {
                        strSql += " and convert(datetime, t.z3date) < '" + endTime + "'";
                    }

                    strSql += @" group by case y.ybxmbh when '39199999990001000001' then 'Z-00000-00000' when '61048000000500000000' then '61048000000400000000' else y.ybxmbh end, case y.ybxmbh when '39199999990001000001' then '医保不予支付的中成药费用' when '61048000000500000000' then '人工煎药' else y.ybxmmc end, t.z3djxx, t.z3item, t.z3name, t.z3empn, t.z3kdys, c.z1ksno, c.z1ksnm, t.z3sfno, y.sfxmzldm, y.sflbdm
                    having sum(case left(t.z3endv, 1) when '4' then -t.z3jzcs else t.z3jzcs end) > 0 and sum(case left(t.z3endv, 1) when '4' then -t.z3jzje else t.z3jzje end) > 0";//已优化
                }
                else
                {
                    strSql = string.Format(@"select y.ybxmbh, y.ybxmmc, t.z3djxx dj, sum(case left(t.z3endv, 1) when '4' then -t.z3jzcs else t.z3jzcs end) sl
                    , sum(case left(t.z3endv, 1) when '4' then -t.z3jzje else t.z3jzje end) je, t.z3item yyxmbh, t.z3name yyxmmc, t.z3empn ysdm
                    , t.z3kdys ysxm, c.z1ksno ksno, c.z1ksnm zxks, t.z3sfno sfno, y.sfxmzldm ybsfxmzldm, y.sflbdm ybsflbdm, min(t.z3date) cfsj
                    from zy03d t
                    join zy01h c on t.z3comp = c.z1comp and t.z3zyno = c.z1zyno and t.z3ghno = c.z1ghno
                    left join ybhisdzdr20171231 y on t.z3item = y.hisxmbh and y.scbz = 1
                    where isnull(t.z3ybup, '') = '' and isnull(t.z3jshx, '') = '' and t.z3kind like '2%' and t.z3zyno = '{0}'", jzlsh);
                    strSql += " and convert(datetime, t.z3date) < '2018-01-01'";

                    if (!string.IsNullOrWhiteSpace(endTime))
                    {
                        strSql += " and convert(datetime, t.z3date) < '" + endTime + "'";
                    }

                    strSql += @" group by y.ybxmbh, y.ybxmmc, t.z3djxx, t.z3item, t.z3name, t.z3empn, t.z3kdys, c.z1ksno, c.z1ksnm, t.z3sfno, y.sfxmzldm, y.sflbdm
                    having sum(case left(t.z3endv, 1) when '4' then -t.z3jzcs else t.z3jzcs end) > 0 and sum(case left(t.z3endv, 1) when '4' then -t.z3jzje else t.z3jzje end) > 0";
                    strSql += string.Format(@" union all select y.ybxmbh, y.ybxmmc, t.z3djxx dj, sum(case left(t.z3endv, 1) when '4' then -t.z3jzcs else t.z3jzcs end) sl
                    , sum(case left(t.z3endv, 1) when '4' then -t.z3jzje else t.z3jzje end) je, t.z3item yyxmbh, t.z3name yyxmmc, t.z3empn ysdm
                    , t.z3kdys ysxm, c.z1ksno ksno, c.z1ksnm zxks, t.z3sfno sfno, y.sfxmzldm ybsfxmzldm, y.sflbdm ybsflbdm, min(t.z3date) cfsj
                    from zy03d t
                    join zy01h c on t.z3comp = c.z1comp and t.z3zyno = c.z1zyno and t.z3ghno = c.z1ghno
                    left join ybhisdzdr y on t.z3item = y.hisxmbh and y.scbz = 1
                    where isnull(t.z3ybup, '') = '' and isnull(t.z3jshx, '') = '' and t.z3kind like '2%' and t.z3zyno = '{0}'", jzlsh);
                    strSql += " and convert(datetime, t.z3date) >= '2018-01-01'";

                    if (!string.IsNullOrWhiteSpace(endTime))
                    {
                        strSql += " and convert(datetime, t.z3date) < '" + endTime + "'";
                    }

                    strSql += @" group by y.ybxmbh, y.ybxmmc, t.z3djxx, t.z3item, t.z3name, t.z3empn, t.z3kdys, c.z1ksno, c.z1ksnm, t.z3sfno, y.sfxmzldm, y.sflbdm
                    having sum(case left(t.z3endv, 1) when '4' then -t.z3jzcs else t.z3jzcs end) > 0 and sum(case left(t.z3endv, 1) when '4' then -t.z3jzje else t.z3jzje end) > 0";//已优化
                }

                ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                List<string> liybcfh = new List<string>();
                List<string> liyyxmdm = new List<string>();
                List<string> liyyxmmc = new List<string>();
                List<string> lizysfdj = new List<string>();
                StringBuilder rc = new StringBuilder();
                bool isfpsc = false;
                StringBuilder wdzxms = new StringBuilder();

                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    DataTable dt = ds.Tables[0];
                    int dtcount = dt.Rows.Count;

                    for (int k = 0; k < dtcount; k++)
                    {
                        DataRow dr = dt.Rows[k];

                        if (dr["ybxmbh"] == DBNull.Value)
                        {
                            wdzxms.Append("医保提示：项目代码：[" + dr["yyxmbh"].ToString() + "]，名称：[" + dr["yyxmmc"].ToString() + "]未对照或未上传，不能上传费用！");
                        }
                    }
                }

                if (wdzxms.Length > 0)
                {
                    return new object[] { 0, 0, wdzxms };
                }

                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    DataTable dt = ds.Tables[0];
                    int dtcount = dt.Rows.Count;

                    for (int k = 0; k < dtcount; k++)
                    {
                        DataRow dr = dt.Rows[k];

                        if (dr["ksno"] == DBNull.Value || dr["ksno"].ToString() == "")
                        {
                            return new object[] { 0, 0, "医保提示：有科室编号为空的记录" };
                        }
                        else if (dr["zxks"] == DBNull.Value || dr["zxks"].ToString() == "")
                        {
                            return new object[] { 0, 0, "医保提示：无此科室编号[" + dr["ksno"].ToString() + "]" };
                        }
                        else
                        {
                            string ybsfxmzldm = dr["ybsfxmzldm"].ToString();
                            string ybsflbdm = dr["ybsflbdm"].ToString();
                            string yyxmbh = dr["yyxmbh"].ToString();
                            string ybxmbh = dr["ybxmbh"].ToString();
                            string yyxmmc = dr["yyxmmc"].ToString();
                            decimal dj = Convert.ToDecimal(dr["dj"]);
                            decimal sl = Convert.ToDecimal(dr["sl"]);
                            decimal je = Convert.ToDecimal(dr["je"]);
                            decimal mcyl = 1;
                            string ysdm = string.IsNullOrWhiteSpace(dr["ysdm"].ToString()) ? "010001" : dr["ysdm"].ToString();
                            string ysxm = string.IsNullOrWhiteSpace(dr["ysxm"].ToString()) ? "严东标" : dr["ysxm"].ToString();
                            string ksdm = dr["ksno"].ToString();
                            string ksmc = dr["zxks"].ToString();
                            string ypjldw = "-";
                            string ybcfh = cfsjh + k.ToString();
                            string cydffbz = "0";

                            if (ybsfxmzldm == "1")
                            {
                                cydffbz = "1";
                            }

                            if (dr["sfno"].ToString() == "01" || dr["sfno"].ToString() == "02" || dr["sfno"].ToString() == "03")
                            {
                                ypjldw = "粒";
                            }

                            string cfsj = Convert.ToDateTime(dr["cfsj"]).ToString("yyyyMMddHHmmss");
                            liybcfh.Add(ybcfh);
                            liyyxmdm.Add(yyxmbh);
                            liyyxmmc.Add(yyxmmc);
                            string strSql1 = string.Format(@"insert into ybcfmxscindr(jzlsh, jylsh, sfxmzl, sflb, ybcfh, cfrq, yysfxmbm, sfxmzxbm
                            , yysfxmmc, dj, sl, je, jx, gg, mcyl, sypc, ysbm, ysxm, yf, dw, ksbh, ksmc, zxts, cydffbz, jbr, ypjldw, qezfbz, grbh, xm, kh)
                            values ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', {9}, {10}, {11}, '{12}', '{13}', {14}, '{15}'
                            , '{16}', '{17}', '{18}', '{19}', '{20}', '{21}', '{22}', '{23}', '{24}', '{25}', '{26}', '{27}', '{28}', '{29}')"
                            , jzlsh, jylsh, ybsfxmzldm, ybsflbdm, ybcfh, cfsj, yyxmbh, ybxmbh, yyxmmc, Math.Round(dj, 4).ToString(), Math.Round(sl, 2).ToString(), Math.Round(je, 4).ToString()
                            , "101", "", mcyl, "", ysdm, ysxm, "", "", ksdm, ksmc, "", cydffbz, jbr, ypjldw, "", grbh, xm, kh);
                            lizysfdj.Add(strSql1);
                            rc.Append(ybjzlsh + "|" + ybsfxmzldm + "|" + ybsflbdm + "|" + ybcfh + "|" + cfsj + "|" + yyxmbh
                            + "|" + ybxmbh + "|" + yyxmmc + "|" + Math.Round(dj, 4).ToString() + "|" + Math.Round(sl, 2).ToString() + "|" + Math.Round(je, 4).ToString()
                            + "|101||" + mcyl.ToString() + "||" + ysdm + "|" + ysxm + "|||" + ksdm + "|" + ksmc + "||" + cydffbz + "|"
                            + jbr + "|" + ypjldw + "||" + grbh + "|" + xm + "|" + kh + "|$");
                            int scjls = 100;

                            #region 分批上传
                            if (dtcount > scjls)
                            {
                                isfpsc = true;

                                if ((k + 1) % scjls == 0)
                                {
                                    StringBuilder inputData = new StringBuilder(string.Format("{0}^{1}^{2}^{3}^{4}^{5}^{6}^{7}^", "2310", DDYLJGBH, czygh, ywzqh, jylsh, ZXBM, rc.ToString().TrimEnd('$'), "1"));
                                    StringBuilder outputData = new StringBuilder(100000);
                                    int i = BUSINESS_HANDLE(inputData, outputData);

                                    if (i == 0)
                                    {
                                        isybzycfmxsb = true;
                                        string[] zysfdjfhs = outputData.ToString().Split('^')[2].TrimEnd('$').Split('$');

                                        if (ydrybz == "0")
                                        {
                                            //begin判断返回记录数是否和上传记录数相等
                                            if (zysfdjfhs.Length != liybcfh.Count || zysfdjfhs.Length != liyyxmdm.Count || zysfdjfhs.Length != liyyxmmc.Count)
                                            {
                                                i = ChangeYBCFMXSDCXDR(new object[]{ybjzlsh, grbh, xm, kh, ZXBM});

                                                if (i == 1)
                                                {
                                                    return new object[] { 0, 0, "医保提示：医保返回记录数和上传记录数不相等,自动撤销医保处方明细成功" };
                                                }
                                                else
                                                {
                                                    return new object[] { 0, 0, "医保提示：医保返回记录数和上传记录数不相等,自动撤销医保处方明细失败" };
                                                }
                                            }
                                            //end判断返回记录数是否和上传记录数相等

                                            for (int j = 0; j < zysfdjfhs.Length; j++)
                                            {
                                                string[] zysfdjfh = zysfdjfhs[j].Split('|');
                                                bool isConvert = decimal.TryParse(zysfdjfh[0], out je);
                                                decimal zlje;
                                                isConvert = decimal.TryParse(zysfdjfh[1], out zlje);
                                                decimal zfje;
                                                isConvert = decimal.TryParse(zysfdjfh[2], out zfje);
                                                decimal cxjzfje;
                                                isConvert = decimal.TryParse(zysfdjfh[3], out cxjzfje);
                                                string sflb1 = zysfdjfh[4];
                                                string sfxmdj = zysfdjfh[5];
                                                string qezfbz = zysfdjfh[6];
                                                decimal zlbl;
                                                isConvert = decimal.TryParse(zysfdjfh[7], out zlbl);
                                                decimal xj;
                                                isConvert = decimal.TryParse(zysfdjfh[8], out xj);
                                                string bz = zysfdjfh[9];
                                                strSql1 = string.Format(@"insert into ybcfmxscfhdr(jzlsh, jylsh, je, zlje, zfje, cxjzfje, sflb, sfxmdj, qezfbz, zlbl, xj, bz, grbh, xm, kh, cfh, ybcfh, yyxmdm, yyxmmc) 
                                                values('{0}', '{1}', {2}, {3}, {4}, {5}, '{6}', '{7}', '{8}', {9}, {10}, '{11}', '{12}', '{13}', '{14}', '{15}', '{16}', '{17}', '{18}')"
                                                , jzlsh, jylsh, je, zlje, zfje, cxjzfje, sflb1, sfxmdj, qezfbz, zlbl, xj, bz, grbh, xm, kh, jzlsh, liybcfh[j], liyyxmdm[j], liyyxmmc[j]);
                                                lizysfdj.Add(strSql1);
                                            }
                                        }

                                        string strSql2 = string.Format("update zy03d set z3ybup = '{0}' where z3comp = '{1}' and z3zyno = '{2}' and z3kind like '2%' and z3ybup is null", jylsh, CliUtils.fSiteCode, jzlsh);//0秒

                                        if (!string.IsNullOrWhiteSpace(endTime))
                                        {
                                            strSql2 += " and convert(datetime, z3date) <= '" + endTime + "'";
                                        }

                                        lizysfdj.Add(strSql2);
                                        object[] obj = lizysfdj.ToArray();
                                        obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);

                                        if (obj[1].ToString() == "1")
                                        {
                                            rc.Clear();
                                            liybcfh.Clear();
                                            liyyxmdm.Clear();
                                            liyyxmmc.Clear();
                                            lizysfdj.Clear();

                                            if (k + 1 == dtcount)
                                            {
                                                return new object[] { 0, 1, outputData };
                                            }
                                        }
                                        else
                                        {
                                            Common.WriteYBLog(obj[2].ToString());
                                            i = ChangeYBCFMXSDCXDR(new object[]{ybjzlsh, grbh, xm, kh, ZXBM});

                                            if (i == 1)
                                            {
                                                return new object[] { 0, 0, "医保提示：his数据库操作失败,自动撤销医保住院处方明细成功,错误信息：" + obj[2].ToString() };
                                            }
                                            else
                                            {
                                                return new object[] { 0, 0, "医保提示：his数据库操作失败,自动撤销医保住院处方明细失败,错误信息：" + obj[2].ToString() };
                                            }
                                        }
                                    }
                                    else if (i == -1)
                                    {
                                        i = ChangeYBCFMXSDCXDR(new object[]{ybjzlsh, grbh, xm, kh, ZXBM});

                                        if (i == 1)
                                        {
                                            return new object[] { 0, -1, "医保提示：医保系统级别错误，自动撤销医保住院处方明细上传成功，" + outputData.ToString() };
                                        }
                                        else
                                        {
                                            return new object[] { 0, -1, "医保提示：医保系统级别错误，自动撤销医保住院处方明细上传失败，" + outputData.ToString() };
                                        }
                                    }
                                    else
                                    {
                                        return new object[] { 0, -2, "医保提示：医保住院处方明细上传业务级别或未知错误，" + outputData.ToString() };
                                    }
                                }
                                else
                                {
                                    if (dtcount % scjls > 0 && k + 1 == dtcount)
                                    {
                                        StringBuilder inputData = new StringBuilder(string.Format("{0}^{1}^{2}^{3}^{4}^{5}^{6}^{7}^", "2310", DDYLJGBH, czygh, ywzqh, jylsh, ZXBM, rc.ToString().TrimEnd('$'), "1"));
                                        StringBuilder outputData = new StringBuilder(100000);
                                        int i = BUSINESS_HANDLE(inputData, outputData);

                                        if (i == 0)
                                        {
                                            isybzycfmxsb = true;
                                            string[] zysfdjfhs = outputData.ToString().Split('^')[2].TrimEnd('$').Split('$');

                                            if (ydrybz == "0")
                                            {
                                                //begin判断返回记录数是否和上传记录数相等
                                                if (zysfdjfhs.Length != liybcfh.Count || zysfdjfhs.Length != liyyxmdm.Count || zysfdjfhs.Length != liyyxmmc.Count)
                                                {
                                                    i = ChangeYBCFMXSDCXDR(new object[]{ybjzlsh, grbh, xm, kh, ZXBM});

                                                    if (i == 1)
                                                    {
                                                        return new object[] { 0, 0, "医保提示：医保返回记录数和上传记录数不相等,自动撤销医保处方明细成功" };
                                                    }
                                                    else
                                                    {
                                                        return new object[] { 0, 0, "医保提示：医保返回记录数和上传记录数不相等,自动撤销医保处方明细失败" };
                                                    }
                                                }
                                                //end判断返回记录数是否和上传记录数相等

                                                for (int j = 0; j < zysfdjfhs.Length; j++)
                                                {
                                                    string[] zysfdjfh = zysfdjfhs[j].Split('|');
                                                    bool isConvert = decimal.TryParse(zysfdjfh[0], out je);
                                                    decimal zlje;
                                                    isConvert = decimal.TryParse(zysfdjfh[1], out zlje);
                                                    decimal zfje;
                                                    isConvert = decimal.TryParse(zysfdjfh[2], out zfje);
                                                    decimal cxjzfje;
                                                    isConvert = decimal.TryParse(zysfdjfh[3], out cxjzfje);
                                                    string sflb1 = zysfdjfh[4];
                                                    string sfxmdj = zysfdjfh[5];
                                                    string qezfbz = zysfdjfh[6];
                                                    decimal zlbl;
                                                    isConvert = decimal.TryParse(zysfdjfh[7], out zlbl);
                                                    decimal xj;
                                                    isConvert = decimal.TryParse(zysfdjfh[8], out xj);
                                                    string bz = zysfdjfh[9];
                                                    strSql1 = string.Format(@"insert into ybcfmxscfhdr(jzlsh, jylsh, je, zlje, zfje, cxjzfje, sflb, sfxmdj, qezfbz, zlbl, xj, bz, grbh, xm, kh, cfh, ybcfh, yyxmdm, yyxmmc) 
                                                    values('{0}', '{1}', {2}, {3}, {4}, {5}, '{6}', '{7}', '{8}', {9}, {10}, '{11}', '{12}', '{13}', '{14}', '{15}', '{16}', '{17}', '{18}')"
                                                    , jzlsh, jylsh, je, zlje, zfje, cxjzfje, sflb1, sfxmdj, qezfbz, zlbl, xj, bz, grbh, xm, kh, jzlsh, liybcfh[j], liyyxmdm[j], liyyxmmc[j]);
                                                    lizysfdj.Add(strSql1);
                                                }
                                            }

                                            string strSql2 = string.Format("update zy03d set z3ybup = '{0}' where z3comp = '{1}' and z3zyno = '{2}' and z3kind like '2%' and z3ybup is null", jylsh, CliUtils.fSiteCode, jzlsh);//0秒

                                            if (!string.IsNullOrWhiteSpace(endTime))
                                            {
                                                strSql2 += " and convert(datetime, z3date) <= '" + endTime + "'";
                                            }

                                            lizysfdj.Add(strSql2);
                                            object[] obj = lizysfdj.ToArray();
                                            obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);

                                            if (obj[1].ToString() == "1")
                                            {
                                                rc.Clear();
                                                liybcfh.Clear();
                                                liyyxmdm.Clear();
                                                liyyxmmc.Clear();
                                                lizysfdj.Clear();

                                                if (k + 1 == dtcount)
                                                {
                                                    return new object[] { 0, 1, outputData };
                                                }
                                            }
                                            else
                                            {
                                                Common.WriteYBLog(obj[2].ToString());
                                                i = ChangeYBCFMXSDCXDR(new object[]{ybjzlsh, grbh, xm, kh, ZXBM});

                                                if (i == 1)
                                                {
                                                    return new object[] { 0, 0, "医保提示：his数据库操作失败,自动撤销医保住院处方明细成功,错误信息：" + obj[2].ToString() };
                                                }
                                                else
                                                {
                                                    return new object[] { 0, 0, "医保提示：his数据库操作失败,自动撤销医保住院处方明细失败,错误信息：" + obj[2].ToString() };
                                                }
                                            }
                                        }
                                        else if (i == -1)
                                        {
                                            i = ChangeYBCFMXSDCXDR(new object[]{ybjzlsh, grbh, xm, kh, ZXBM});

                                            if (i == 1)
                                            {
                                                return new object[] { 0, -1, "医保提示：医保系统级别错误，自动撤销医保住院处方明细上传成功，" + outputData.ToString() };
                                            }
                                            else
                                            {
                                                return new object[] { 0, -1, "医保提示：医保系统级别错误，自动撤销医保住院处方明细上传失败，" + outputData.ToString() };
                                            }
                                        }
                                        else
                                        {
                                            return new object[] { 0, -2, "医保提示：医保住院处方明细上传业务级别或未知错误，" + outputData.ToString() };
                                        }
                                    }
                                }
                            }
                            #endregion 分批上传
                        }
                    }
                }
                else
                {
                    return new object[] { 0, 0, "医保提示：医保本次无住院费用上传" };
                }

                if (!isfpsc)
                {
                    StringBuilder inputData = new StringBuilder(string.Format("{0}^{1}^{2}^{3}^{4}^{5}^{6}^{7}^", "2310", DDYLJGBH, czygh, ywzqh, jylsh, ZXBM, rc.ToString().TrimEnd('$'), "1"));
                    StringBuilder outputData = new StringBuilder(100000);
                    int i = BUSINESS_HANDLE(inputData, outputData);

                    if (i == 0)
                    {
                        isybzycfmxsb = true;
                        string[] zysfdjfhs = outputData.ToString().Split('^')[2].TrimEnd('$').Split('$');

                        if (ydrybz == "0")
                        {
                            //begin判断返回记录数是否和上传记录数相等
                            if (zysfdjfhs.Length != liybcfh.Count || zysfdjfhs.Length != liyyxmdm.Count || zysfdjfhs.Length != liyyxmmc.Count)
                            {
                                i = ChangeYBCFMXSDCXDR(new object[]{ybjzlsh, grbh, xm, kh, ZXBM});

                                if (i == 1)
                                {
                                    return new object[] { 0, 0, "医保提示：医保返回记录数和上传记录数不相等,自动撤销医保处方明细成功" };
                                }
                                else
                                {
                                    return new object[] { 0, 0, "医保提示：医保返回记录数和上传记录数不相等,自动撤销医保处方明细失败" };
                                }
                            }
                            //end判断返回记录数是否和上传记录数相等

                            for (int j = 0; j < zysfdjfhs.Length; j++)
                            {
                                string[] zysfdjfh = zysfdjfhs[j].Split('|');
                                decimal je;
                                bool isConvert = decimal.TryParse(zysfdjfh[0], out je);
                                decimal zlje;
                                isConvert = decimal.TryParse(zysfdjfh[1], out zlje);
                                decimal zfje;
                                isConvert = decimal.TryParse(zysfdjfh[2], out zfje);
                                decimal cxjzfje;
                                isConvert = decimal.TryParse(zysfdjfh[3], out cxjzfje);
                                string sflb1 = zysfdjfh[4];
                                string sfxmdj = zysfdjfh[5];
                                string qezfbz = zysfdjfh[6];
                                decimal zlbl;
                                isConvert = decimal.TryParse(zysfdjfh[7], out zlbl);
                                decimal xj;
                                isConvert = decimal.TryParse(zysfdjfh[8], out xj);
                                string bz = zysfdjfh[9];
                                string strSql1 = string.Format(@"insert into ybcfmxscfhdr(jzlsh, jylsh, je, zlje, zfje, cxjzfje, sflb, sfxmdj, qezfbz, zlbl, xj, bz, grbh, xm, kh, cfh, ybcfh, yyxmdm, yyxmmc) 
                                values('{0}', '{1}', {2}, {3}, {4}, {5}, '{6}', '{7}', '{8}', {9}, {10}, '{11}', '{12}', '{13}', '{14}', '{15}', '{16}', '{17}', '{18}')"
                                , jzlsh, jylsh, je, zlje, zfje, cxjzfje, sflb1, sfxmdj, qezfbz, zlbl, xj, bz, grbh, xm, kh, jzlsh, liybcfh[j], liyyxmdm[j], liyyxmmc[j]);
                                lizysfdj.Add(strSql1);
                            }
                        }

                        string strSql2 = string.Format("update zy03d set z3ybup = '{0}' where z3comp = '{1}' and z3zyno = '{2}' and z3kind like '2%' and z3ybup is null", jylsh, CliUtils.fSiteCode, jzlsh);//0秒

                        if (!string.IsNullOrWhiteSpace(endTime))
                        {
                            strSql2 += " and convert(datetime, z3date) <= '" + endTime + "'";
                        }

                        lizysfdj.Add(strSql2);
                        object[] obj = lizysfdj.ToArray();
                        obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);

                        if (obj[1].ToString() == "1")
                        {
                            return new object[] { 0, 1, outputData };
                        }
                        else
                        {
                            Common.WriteYBLog(obj[2].ToString());
                            i = ChangeYBCFMXSDCXDR(new object[]{ybjzlsh, grbh, xm, kh, ZXBM});

                            if (i == 1)
                            {
                                return new object[] { 0, 0, "医保提示：his数据库操作失败,自动撤销医保住院处方明细成功,错误信息：" + obj[2].ToString() };
                            }
                            else
                            {
                                return new object[] { 0, 0, "医保提示：his数据库操作失败,自动撤销医保住院处方明细失败,错误信息：" + obj[2].ToString() };
                            }
                        }
                    }
                    else if (i == -1)
                    {
                        i = ChangeYBCFMXSDCXDR(new object[]{ybjzlsh, grbh, xm, kh, ZXBM});

                        if (i == 1)
                        {
                            return new object[] { 0, -1, "医保提示：医保系统级别错误，自动撤销医保住院处方明细上传成功，" + outputData.ToString() };
                        }
                        else
                        {
                            return new object[] { 0, -1, "医保提示：医保系统级别错误，自动撤销医保住院处方明细上传失败，" + outputData.ToString() };
                        }
                    }
                    else
                    {
                        return new object[] { 0, -2, "医保提示：医保住院处方明细上传业务级别或未知错误，" + outputData.ToString() };
                    }
                }
                else
                {
                    return new object[] { 0, -3, "医保提示：既不是一次也不是分批上传，请联系管理员" };
                }
            }
            catch (Exception error)
            {
                Common.WriteYBLog(error.ToString());

                if (isybzycfmxsb)
                {
                    int i = ChangeYBCFMXSDCXDR(new object[]{ybjzlsh, grbh, xm, kh, ZXBM});

                    if (i == 1)
                    {
                        return new object[] { 0, 2, "医保提示：非医保异常，自动撤销医保住院处方明细上传成功，" + error.ToString() };
                    }
                    else
                    {
                        return new object[] { 0, 2, "医保提示：非医保异常，自动撤销医保住院处方明细上传失败，" + error.ToString() };
                    }
                }

                return new object[] { 0, 2, "Error:" + error.ToString() };
            }
        }
        #endregion 住院处方明细上报20180101

        #region 住院处方明细自动上报判断东软
        /// <summary>
        /// 住院处方明细自动上报判断东软
        /// </summary>
        /// <param>无</param>
        /// <returns>1:有</returns>
        public virtual object[] YBZYCFMXZDSBPDDR()
        {
            try
            {
                string sql = string.Format(@"select count(1)
                from zy01h join ybmzzydjdr on z1zyno = jzlsh and cxbz = 1
                join zy03d on z1zyno = z3zyno and z1ghno = z3ghno and z1comp = z3comp 
                where left(z1endv ,1) = '0' and left(z3kind, 1) in ('2', '4') and left(z3endv, 1) != '4'  
                and isnull(z3ybup, '') = '' and isnull(z3fphx, '') = ''");//9秒
                DataSet dshz = CliUtils.ExecuteSql("sybdj", "cmd", sql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                DataTable dthz = dshz.Tables[0];

                if (dthz.Rows.Count > 0 && YBZDSCGH == CliUtils.fLoginUser)
                {
                    //Frm_ybzyfyzdscdr frmzdsc = new Frm_ybzyfyzdscdr();
                    //frmzdsc.ShowDialog();
                    //return new object[] { 0, 1, "医保提示：有费用明细需要上传" };
                    return new object[] { 0, 0, "医保提示：没有需要上传的费用信息" };
                }
                else
                {
                    return new object[] { 0, 0, "医保提示：没有需要上传的费用信息" };
                }
            }
            catch (Exception ee)
            {
                return new object[] { 0, 2, "医保提示：" + ee.ToString() };
            }
        }
        #endregion 住院处方明细自动上报判断东软

        #region 住院处方明细自动上报东软
        /// <summary>
        /// 住院处方明细自动上报东软
        /// </summary>
        /// <param>无</param>
        /// <returns>金额|自理金额|自费金额|超限价自付金额|收费类别|收费项目等级|全额自费标志|自理比例|限价|备注</returns>
        public static object[] YBZYCFMXZDSBDR(params object[] objParam)
        {
            string errorMess = "";
            int errorCount = 0;
            string sql = string.Format(@"select z1zyno, grbh bxh, z1hznm, z1ksno, z1ksnm, z1amt2
            , sum(z3amnt * (case left(z3endv, 1) when '4' then -1 else 1 end)) amt2
            , z1amt1, z1amt3, xm, jylsh, kh, min(z3kdys) z3kdys 
            from zy01h join ybmzzydjdr on z1zyno = jzlsh and cxbz = 1
            join zy03d on z1zyno = z3zyno and z1ghno = z3ghno and z1comp = z3comp 
            where left(z1endv ,1) = '0' and left(z3kind, 1) in ('2', '4') and left(z3endv, 1) != '4'  
            and isnull(z3ybup, '') = '' and isnull(z3fphx, '') = '' 
            group by z1zyno, z1hznm, z1ksno, z1ksnm, z1amt1, z1amt2, z1amt3, xm, grbh, jylsh, kh");//32秒
            DataSet dshz = CliUtils.ExecuteSql("sybdj", "cmd", sql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            DataTable dthz = dshz.Tables[0];

            if (dthz.Rows.Count > 0)
            {
                for (int i = 0; i < dthz.Rows.Count; i++)
                {
                    DataRow drhz = dthz.Rows[i];
                    string zyno = drhz["z1zyno"].ToString();

                    if (string.IsNullOrWhiteSpace(zyno))
                    {
                        return new object[] { 0, 0, "医保提示：就诊流水号为空" };
                    }

                    object[] param = { zyno };
                    object[] back_mess = YBZYCFMXSBDR(param);

                    if (back_mess[1].ToString() != "1")
                    {
                        errorCount++;
                        errorMess += zyno + ":" + back_mess[2].ToString() + ",";
                    }
                }

                if (errorCount == 0)
                {
                    return new object[] { 0, 1, "医保提示：所有病人费用上传成功" };
                }
                else
                {
                    return new object[] { 0, 0, "医保提示：有" + errorCount.ToString() + "个病人的费用上传失败:" + errorMess.TrimEnd(',') };
                }
            }
            else
            {
                return new object[] { 0, 0, "医保提示：没有需要上传的费用信息" };
            }
        }
        #endregion 住院处方明细自动上报东软

        #region 住院处方明细上报
        /// <summary>
        /// 住院处方明细上报20171230
        /// </summary>
        /// <param>his就诊流水号, 截止时间(中途预结算需要传，格式：yyyy-MM-dd HH:mm:ss), 截断日期(中途参保缴费需要传，格式：yyyy-MM-dd)</param>
        /// <returns>金额|自理金额|自费金额|超限价自付金额|收费类别|收费项目等级|全额自费标志|自理比例|限价|备注</returns>
        public static object[] YBZYCFMXSBDR(object[] objParam)
        {
            string czygh = CliUtils.fLoginUser;
            string ywzqh = YWZQH;
            string jbr = CliUtils.fUserName;
            string jzlsh = objParam[0].ToString();
            string endTime = "";
            string jdrq = "";

            if (objParam.Length > 1 && objParam[1].ToString() != "")
            {
                endTime = Convert.ToDateTime(objParam[1]).AddDays(1).ToString();
            }

            if (objParam.Length > 2 && objParam[2].ToString() != "")
            {
                jdrq = Convert.ToDateTime(objParam[2]).AddDays(1).ToString();
            }

            if (string.IsNullOrWhiteSpace(jzlsh))
            {
                return new object[] { 0, 0, "医保提示：his就诊流水号为空" };
            }

            DateTime dqsj = Convert.ToDateTime(Common.GetServerTime());
            string jylsh = dqsj.ToString("yyyyMMddHHmmss") + "-" + DDYLJGBH + "-" + jzlsh;
            string cfsjh = jzlsh + dqsj.ToString("HHmmss");
            bool isybzycfmxsb = false;
            string ybjzlsh = "";
            string grbh = "";
            string xm = "";
            string kh = "";

            try
            {
                string strSql = string.Format("select * from zy03dw a where a.z3zyno = '{0}' and left(a.z3endv, 1) = '1'", jzlsh);//0秒
                DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);

                //if (ds != null && ds.Tables[0].Rows.Count > 0)
                //{
                //    return new object[] { 0, 0, "医保提示：his就诊流水号" + jzlsh + "已进行结算" };
                //}

                strSql = string.Format("select ybjzlsh, grbh, xm, kh, ydrybz, tcqh from ybmzzydjdr a where a.jzlsh = '{0}' and a.cxbz = 1 and a.jzbz = 'z'", jzlsh);//0秒
                ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);

                if (ds == null || ds.Tables[0].Rows.Count == 0)
                {
                    return new object[] { 0, 0, "医保提示：医保无住院登记信息" };
                }

                DataRow dr1 = ds.Tables[0].Rows[0];
                ybjzlsh = dr1["ybjzlsh"].ToString();
                grbh = dr1["grbh"].ToString();
                xm = dr1["xm"].ToString();
                kh = dr1["kh"].ToString();
                string ydrybz = dr1["ydrybz"].ToString();
                ZXBM = ydrybz == "0" ? "0000" : dr1["tcqh"].ToString();

                //新增
                string strSqlxz = string.Format("select a.z1outd cyrq from zy01d a where left(a.z1endv, 1) = '8' and a.z1zyno = '{0}'", jzlsh);//0秒
                DataSet dsxz = CliUtils.ExecuteSql("sybdj", "cmd", strSqlxz, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);

                if (dsxz == null || dsxz.Tables[0].Rows.Count == 0)
                {
                    return new object[] { 0, 0, "his就诊流水号" + jzlsh + "未拖出床位" };
                }
                //end新增

                strSql = string.Format(@"select y.ybxmbh, y.ybxmmc, t.z3djxx dj, sum(case left(t.z3endv, 1) when '4' then -t.z3jzcs else t.z3jzcs end) sl
                , sum(case left(t.z3endv, 1) when '4' then -t.z3jzje else t.z3jzje end) je, t.z3item yyxmbh, t.z3name yyxmmc, t.z3empn ysdm
                , t.z3kdys ysxm, c.z1ksno ksno, c.z1ksnm zxks, t.z3sfno sfno, y.sfxmzldm ybsfxmzldm, y.sflbdm ybsflbdm, min(t.z3date) cfsj
                from zy03d t
                join zy01h c on t.z3comp = c.z1comp and t.z3zyno = c.z1zyno and t.z3ghno = c.z1ghno
                left join ybhisdzdr y on t.z3item = y.hisxmbh and y.scbz = 1
                where isnull(t.z3ybup, '') = '' and isnull(t.z3jshx, '') = '' and t.z3kind like '2%' and t.z3zyno = '{0}'", jzlsh);
                strSql += " and convert(datetime, t.z3date) >= '2017-12-30'";

                if (!string.IsNullOrWhiteSpace(endTime))
                {
                    strSql += " and convert(datetime, t.z3date) < '" + endTime + "'";
                }

                strSql += @" group by y.ybxmbh, y.ybxmmc, t.z3djxx, t.z3item, t.z3name, t.z3empn, t.z3kdys, c.z1ksno, c.z1ksnm, t.z3sfno, y.sfxmzldm, y.sflbdm
                having sum(case left(t.z3endv, 1) when '4' then -t.z3jzcs else t.z3jzcs end) < 0 and sum(case left(t.z3endv, 1) when '4' then -t.z3jzje else t.z3jzje end) < 0";
                ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);

                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    string tfxx = "";
                    string xmbhs = "";

                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        xmbhs += "'" + dr["yyxmbh"].ToString() + "',";
                        tfxx += ";医院项目编码：" + dr["yyxmbh"].ToString();
                        tfxx += "医院项目名称：" + dr["yyxmmc"].ToString();
                        tfxx += "数量：" + dr["sl"].ToString();
                    }

                    return new object[] { 0, 0, "医保提示：12月30号后产生过退30号以前的费用" + tfxx, xmbhs.TrimEnd(',') };
                }

                if (ZXBM != "0000")//X-00000-0000-0000医保不予支付的西药费用
                {
                    if (string.IsNullOrWhiteSpace(jdrq))
                    {
                        strSql = string.Format(@"select y.ybxmbh, y.ybxmmc, t.z3djxx dj, sum(case left(t.z3endv, 1) when '4' then -t.z3jzcs else t.z3jzcs end) sl
                    , sum(case left(t.z3endv, 1) when '4' then -t.z3jzje else t.z3jzje end) je, t.z3item yyxmbh, t.z3name yyxmmc, t.z3empn ysdm
                    , t.z3kdys ysxm, c.z1ksno ksno, c.z1ksnm zxks, t.z3sfno sfno, y.sfxmzldm ybsfxmzldm, y.sflbdm ybsflbdm, min(t.z3date) cfsj
                    from zy03d t
                    join zy01h c on t.z3comp = c.z1comp and t.z3zyno = c.z1zyno and t.z3ghno = c.z1ghno
                    left join ybhisdzdr20171231 y on t.z3item = y.hisxmbh and y.scbz = 1
                    where isnull(t.z3ybup, '') = '' and   isnull(t.z3jshx, '') = '' and t.z3kind like '2%' and t.z3zyno = '{0}'", jzlsh);
                        strSql += " and convert(datetime, t.z3date) < '2017-12-30'";

                        if (!string.IsNullOrWhiteSpace(endTime))
                        {
                            strSql += " and convert(datetime, t.z3date) < '" + endTime + "'";
                        }

                        strSql += @" group by y.ybxmbh, y.ybxmmc, t.z3djxx, t.z3item, t.z3name, t.z3empn, t.z3kdys, c.z1ksno, c.z1ksnm, t.z3sfno, y.sfxmzldm, y.sflbdm
                    having sum(case left(t.z3endv, 1) when '4' then -t.z3jzcs else t.z3jzcs end) > 0 and sum(case left(t.z3endv, 1) when '4' then -t.z3jzje else t.z3jzje end) > 0";
                        strSql += string.Format(@" union all select case y.ybxmbh when '39199999990001000001' then 'Z-00000-00000'
when '61048000000500000000' then '61048000000400000000' else y.ybxmbh
end ybxmbh, case y.ybxmbh when '39199999990001000001' then '医保不予支付的中成药费用'
when '61048000000500000000' then '人工煎药' else y.ybxmmc end ybxmmc, t.z3djxx dj, sum(case left(t.z3endv, 1) when '4' then -t.z3jzcs else t.z3jzcs end) sl
                    , sum(case left(t.z3endv, 1) when '4' then -t.z3jzje else t.z3jzje end) je, t.z3item yyxmbh, t.z3name yyxmmc, t.z3empn ysdm
                    , t.z3kdys ysxm, c.z1ksno ksno, c.z1ksnm zxks, t.z3sfno sfno, y.sfxmzldm ybsfxmzldm, y.sflbdm ybsflbdm, min(t.z3date) cfsj
                    from zy03d t
                    join zy01h c on t.z3comp = c.z1comp and t.z3zyno = c.z1zyno and t.z3ghno = c.z1ghno
                    left join ybhisdzdr y on t.z3item = y.hisxmbh and y.scbz = 1
                    where isnull(t.z3ybup, '') = '' and  isnull(t.z3jshx, '') = '' and t.z3kind like '2%' and t.z3zyno = '{0}'", jzlsh);
                        strSql += " and convert(datetime, t.z3date) >= '2017-12-30'";

                        if (!string.IsNullOrWhiteSpace(endTime))
                        {
                            strSql += " and convert(datetime, t.z3date) < '" + endTime + "'";
                        }

                        strSql += @" group by case y.ybxmbh when '39199999990001000001' then 'Z-00000-00000' 
when '61048000000500000000' then '61048000000400000000' else y.ybxmbh end
, case y.ybxmbh when '39199999990001000001' then '医保不予支付的中成药费用' 
when '61048000000500000000' then '人工煎药' else y.ybxmmc end, t.z3djxx, t.z3item, t.z3name, t.z3empn, t.z3kdys, c.z1ksno, c.z1ksnm, t.z3sfno, y.sfxmzldm, y.sflbdm
                    having sum(case left(t.z3endv, 1) when '4' then -t.z3jzcs else t.z3jzcs end) > 0 and sum(case left(t.z3endv, 1) when '4' then -t.z3jzje else t.z3jzje end) > 0";//已优化
                    }
                    else
                    {
                        strSql = string.Format(@" select case y.ybxmbh when '39199999990001000001' then 'Z-00000-00000' when '61048000000500000000' then '61048000000400000000' else y.ybxmbh end ybxmbh, case y.ybxmbh when '39199999990001000001' then '医保不予支付的中成药费用' when '61048000000500000000' then '人工煎药' else y.ybxmmc end ybxmmc, t.z3djxx dj, sum(case left(t.z3endv, 1) when '4' then -t.z3jzcs else t.z3jzcs end) sl
                    , sum(case left(t.z3endv, 1) when '4' then -t.z3jzje else t.z3jzje end) je, t.z3item yyxmbh, t.z3name yyxmmc, t.z3empn ysdm
                    , t.z3kdys ysxm, c.z1ksno ksno, c.z1ksnm zxks, t.z3sfno sfno, y.sfxmzldm ybsfxmzldm, y.sflbdm ybsflbdm, min(t.z3date) cfsj
                    from zy03d t
                    join zy01h c on t.z3comp = c.z1comp and t.z3zyno = c.z1zyno and t.z3ghno = c.z1ghno
                    left join ybhisdzdr y on t.z3item = y.hisxmbh and y.scbz = 1
                    where isnull(t.z3ybup, '') = '' and  isnull(t.z3jshx, '') = '' and t.z3kind like '2%' and t.z3zyno = '{0}'", jzlsh);
                        strSql += " and convert(datetime, t.z3date) < '" + jdrq + "'";

                        if (!string.IsNullOrWhiteSpace(endTime))
                        {
                            strSql += " and convert(datetime, t.z3date) < '" + endTime + "'";
                        }

                        strSql += @" group by case y.ybxmbh when '39199999990001000001' then 'Z-00000-00000' when '61048000000500000000' then '61048000000400000000' else y.ybxmbh end, case y.ybxmbh when '39199999990001000001' then '医保不予支付的中成药费用' when '61048000000500000000' then '人工煎药' else y.ybxmmc end, t.z3djxx, t.z3item, t.z3name, t.z3empn, t.z3kdys, c.z1ksno, c.z1ksnm, t.z3sfno, y.sfxmzldm, y.sflbdm
                    having sum(case left(t.z3endv, 1) when '4' then -t.z3jzcs else t.z3jzcs end) > 0 and sum(case left(t.z3endv, 1) when '4' then -t.z3jzje else t.z3jzje end) > 0";//已优化
                        strSql += string.Format(@" union all select case y.ybxmbh when '39199999990001000001' then 'Z-00000-00000' when '61048000000500000000' then '61048000000400000000' else y.ybxmbh end ybxmbh, case y.ybxmbh when '39199999990001000001' then '医保不予支付的中成药费用' when '61048000000500000000' then '人工煎药' else y.ybxmmc end ybxmmc, t.z3djxx dj, sum(case left(t.z3endv, 1) when '4' then -t.z3jzcs else t.z3jzcs end) sl
                    , sum(case left(t.z3endv, 1) when '4' then -t.z3jzje else t.z3jzje end) je, t.z3item yyxmbh, t.z3name yyxmmc, t.z3empn ysdm
                    , t.z3kdys ysxm, c.z1ksno ksno, c.z1ksnm zxks, t.z3sfno sfno, y.sfxmzldm ybsfxmzldm, y.sflbdm ybsflbdm, min(t.z3date) cfsj
                    from zy03d t
                    join zy01h c on t.z3comp = c.z1comp and t.z3zyno = c.z1zyno and t.z3ghno = c.z1ghno
                    left join ybhisdzdr y on t.z3item = y.hisxmbh and y.scbz = 1
                    where isnull(t.z3ybup, '') = '' and  isnull(t.z3jshx, '') = '' and t.z3kind like '2%' and t.z3zyno = '{0}'", jzlsh);
                        strSql += " and convert(datetime, t.z3date) >= '" + jdrq + "'";

                        if (!string.IsNullOrWhiteSpace(endTime))
                        {
                            strSql += " and convert(datetime, t.z3date) < '" + endTime + "'";
                        }

                        strSql += @" group by case y.ybxmbh when '39199999990001000001' then 'Z-00000-00000' when '61048000000500000000' then '61048000000400000000' else y.ybxmbh end, case y.ybxmbh when '39199999990001000001' then '医保不予支付的中成药费用' when '61048000000500000000' then '人工煎药' else y.ybxmmc end, t.z3djxx, t.z3item, t.z3name, t.z3empn, t.z3kdys, c.z1ksno, c.z1ksnm, t.z3sfno, y.sfxmzldm, y.sflbdm
                    having sum(case left(t.z3endv, 1) when '4' then -t.z3jzcs else t.z3jzcs end) > 0 and sum(case left(t.z3endv, 1) when '4' then -t.z3jzje else t.z3jzje end) > 0";//已优化
                    }
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(jdrq))
                    {
                        strSql = string.Format(@"select y.ybxmbh, y.ybxmmc, t.z3djxx dj, sum(case left(t.z3endv, 1) when '4' then -t.z3jzcs else t.z3jzcs end) sl
                    , sum(case left(t.z3endv, 1) when '4' then -t.z3jzje else t.z3jzje end) je, t.z3item yyxmbh, t.z3name yyxmmc, t.z3empn ysdm
                    , t.z3kdys ysxm, c.z1ksno ksno, c.z1ksnm zxks, t.z3sfno sfno, y.sfxmzldm ybsfxmzldm, y.sflbdm ybsflbdm, min(t.z3date) cfsj
                    from zy03d t
                    join zy01h c on t.z3comp = c.z1comp and t.z3zyno = c.z1zyno and t.z3ghno = c.z1ghno
                    left join ybhisdzdr20171231 y on t.z3item = y.hisxmbh and y.scbz = 1
                    where isnull(t.z3ybup, '') = '' and  isnull(t.z3jshx, '') = '' and t.z3kind like '2%' and t.z3zyno = '{0}'", jzlsh);
                        strSql += " and convert(datetime, t.z3date) < '2017-12-30'";

                        if (!string.IsNullOrWhiteSpace(endTime))
                        {
                            strSql += " and convert(datetime, t.z3date) < '" + endTime + "'";
                        }

                        strSql += @" group by y.ybxmbh, y.ybxmmc, t.z3djxx, t.z3item, t.z3name, t.z3empn, t.z3kdys, c.z1ksno, c.z1ksnm, t.z3sfno, y.sfxmzldm, y.sflbdm
                    having sum(case left(t.z3endv, 1) when '4' then -t.z3jzcs else t.z3jzcs end) > 0 and sum(case left(t.z3endv, 1) when '4' then -t.z3jzje else t.z3jzje end) > 0";
                        strSql += string.Format(@" union all select y.ybxmbh, y.ybxmmc, t.z3djxx dj, sum(case left(t.z3endv, 1) when '4' then -t.z3jzcs else t.z3jzcs end) sl
                    , sum(case left(t.z3endv, 1) when '4' then -t.z3jzje else t.z3jzje end) je, t.z3item yyxmbh, t.z3name yyxmmc, t.z3empn ysdm
                    , t.z3kdys ysxm, c.z1ksno ksno, c.z1ksnm zxks, t.z3sfno sfno, y.sfxmzldm ybsfxmzldm, y.sflbdm ybsflbdm, min(t.z3date) cfsj
                    from zy03d t
                    join zy01h c on t.z3comp = c.z1comp and t.z3zyno = c.z1zyno and t.z3ghno = c.z1ghno
                    left join ybhisdzdr y on t.z3item = y.hisxmbh and y.scbz = 1
                    where  isnull(t.z3ybup, '') = '' and  isnull(t.z3jshx, '') = '' and t.z3kind like '2%' and t.z3zyno = '{0}'", jzlsh);
                        strSql += " and convert(datetime, t.z3date) >= '2017-12-30'";

                        if (!string.IsNullOrWhiteSpace(endTime))
                        {
                            strSql += " and convert(datetime, t.z3date) < '" + endTime + "'";
                        }

                        strSql += @" group by y.ybxmbh, y.ybxmmc, t.z3djxx, t.z3item, t.z3name, t.z3empn, t.z3kdys, c.z1ksno, c.z1ksnm, t.z3sfno, y.sfxmzldm, y.sflbdm
                    having sum(case left(t.z3endv, 1) when '4' then -t.z3jzcs else t.z3jzcs end) > 0 and sum(case left(t.z3endv, 1) when '4' then -t.z3jzje else t.z3jzje end) > 0";//已优化
                    }
                    else
                    {
                        strSql = string.Format(@"select y.ybxmbh, y.ybxmmc, t.z3djxx dj, sum(case left(t.z3endv, 1) when '4' then -t.z3jzcs else t.z3jzcs end) sl
                    , sum(case left(t.z3endv, 1) when '4' then -t.z3jzje else t.z3jzje end) je, t.z3item yyxmbh, t.z3name yyxmmc, t.z3empn ysdm
                    , t.z3kdys ysxm, c.z1ksno ksno, c.z1ksnm zxks, t.z3sfno sfno, y.sfxmzldm ybsfxmzldm, y.sflbdm ybsflbdm, min(t.z3date) cfsj
                    from zy03d t
                    join zy01h c on t.z3comp = c.z1comp and t.z3zyno = c.z1zyno and t.z3ghno = c.z1ghno
                    left join ybhisdzdr y on t.z3item = y.hisxmbh and y.scbz = 1
                    where isnull(t.z3ybup, '') = '' and  isnull(t.z3jshx, '') = '' and t.z3kind like '2%' and t.z3zyno = '{0}'", jzlsh);
                        strSql += " and convert(datetime, t.z3date) < '" + jdrq + "'";

                        if (!string.IsNullOrWhiteSpace(endTime))
                        {
                            strSql += " and convert(datetime, t.z3date) < '" + endTime + "'";
                        }

                        strSql += @" group by y.ybxmbh, y.ybxmmc, t.z3djxx, t.z3item, t.z3name, t.z3empn, t.z3kdys, c.z1ksno, c.z1ksnm, t.z3sfno, y.sfxmzldm, y.sflbdm
                    having sum(case left(t.z3endv, 1) when '4' then -t.z3jzcs else t.z3jzcs end) > 0 and sum(case left(t.z3endv, 1) when '4' then -t.z3jzje else t.z3jzje end) > 0";//已优化
                        strSql += string.Format(@" union all select y.ybxmbh, y.ybxmmc, t.z3djxx dj, sum(case left(t.z3endv, 1) when '4' then -t.z3jzcs else t.z3jzcs end) sl
                    , sum(case left(t.z3endv, 1) when '4' then -t.z3jzje else t.z3jzje end) je, t.z3item yyxmbh, t.z3name yyxmmc, t.z3empn ysdm
                    , t.z3kdys ysxm, c.z1ksno ksno, c.z1ksnm zxks, t.z3sfno sfno, y.sfxmzldm ybsfxmzldm, y.sflbdm ybsflbdm, min(t.z3date) cfsj
                    from zy03d t
                    join zy01h c on t.z3comp = c.z1comp and t.z3zyno = c.z1zyno and t.z3ghno = c.z1ghno
                    left join ybhisdzdr y on t.z3item = y.hisxmbh and y.scbz = 1
                    where isnull(t.z3ybup, '') = '' and  isnull(t.z3jshx, '') = '' and t.z3kind like '2%' and t.z3zyno = '{0}'", jzlsh);
                        strSql += " and convert(datetime, t.z3date) >= '" + jdrq + "'";

                        if (!string.IsNullOrWhiteSpace(endTime))
                        {
                            strSql += " and convert(datetime, t.z3date) < '" + endTime + "'";
                        }

                        strSql += @" group by y.ybxmbh, y.ybxmmc, t.z3djxx, t.z3item, t.z3name, t.z3empn, t.z3kdys, c.z1ksno, c.z1ksnm, t.z3sfno, y.sfxmzldm, y.sflbdm
                    having sum(case left(t.z3endv, 1) when '4' then -t.z3jzcs else t.z3jzcs end) > 0 and sum(case left(t.z3endv, 1) when '4' then -t.z3jzje else t.z3jzje end) > 0";//已优化
                    }
                }

                ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                List<string> liybcfh = new List<string>();
                List<string> liyyxmdm = new List<string>();
                List<string> liyyxmmc = new List<string>();
                List<string> lizysfdj = new List<string>();
                StringBuilder rc = new StringBuilder();
                bool isfpsc = false;
                StringBuilder wdzxms = new StringBuilder();

                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    DataTable dt = ds.Tables[0];
                    int dtcount = dt.Rows.Count;

                    for (int k = 0; k < dtcount; k++)
                    {
                        DataRow dr = dt.Rows[k];

                        if (dr["ybxmbh"] == DBNull.Value)
                        {
                            wdzxms.Append("医保提示：项目代码：[" + dr["yyxmbh"].ToString() + "]，名称：[" + dr["yyxmmc"].ToString() + "]未对照或未上传，不能上传费用！");
                        }
                    }
                }

                if (wdzxms.Length > 0)
                {
                    return new object[] { 0, 0, wdzxms };
                }

                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    DataTable dt = ds.Tables[0];
                    int dtcount = dt.Rows.Count;

                    for (int k = 0; k < dtcount; k++)
                    {
                        DataRow dr = dt.Rows[k];

                        if (dr["ksno"] == DBNull.Value || dr["ksno"].ToString() == "")
                        {
                            return new object[] { 0, 0, "医保提示：有科室编号为空的记录" };
                        }
                        else if (dr["zxks"] == DBNull.Value || dr["zxks"].ToString() == "")
                        {
                            return new object[] { 0, 0, "医保提示：无此科室编号[" + dr["ksno"].ToString() + "]" };
                        }
                        else
                        {
                            string ybsfxmzldm = dr["ybsfxmzldm"].ToString();
                            string ybsflbdm = dr["ybsflbdm"].ToString();
                            string yyxmbh = dr["yyxmbh"].ToString();
                            string ybxmbh = dr["ybxmbh"].ToString();
                            string yyxmmc = dr["yyxmmc"].ToString();
                            decimal dj = Convert.ToDecimal(dr["dj"]);
                            decimal sl = Convert.ToDecimal(dr["sl"]);
                            decimal je = Convert.ToDecimal(dr["je"]);
                            decimal mcyl = 1;
                            string ysdm = string.IsNullOrWhiteSpace(dr["ysdm"].ToString()) ? "010001" : dr["ysdm"].ToString();
                            string ysxm = string.IsNullOrWhiteSpace(dr["ysxm"].ToString()) ? "严东标" : dr["ysxm"].ToString();
                            string ksdm = dr["ksno"].ToString();
                            string ksmc = dr["zxks"].ToString();
                            string ypjldw = "-";
                            string ybcfh = cfsjh + k.ToString();
                            string cydffbz = "0";

                            if (ybsfxmzldm == "1")
                            {
                                cydffbz = "1";
                            }

                            if (dr["sfno"].ToString() == "01" || dr["sfno"].ToString() == "02" || dr["sfno"].ToString() == "03")
                            {
                                ypjldw = "粒";
                            }

                            string cfsj = Convert.ToDateTime(dr["cfsj"]).ToString("yyyyMMddHHmmss");
                            liybcfh.Add(ybcfh);
                            liyyxmdm.Add(yyxmbh);
                            liyyxmmc.Add(yyxmmc);
                            string strSql1 = string.Format(@"insert into ybcfmxscindr(jzlsh, jylsh, sfxmzl, sflb, ybcfh, cfrq, yysfxmbm, sfxmzxbm
                            , yysfxmmc, dj, sl, je, jx, gg, mcyl, sypc, ysbm, ysxm, yf, dw, ksbh, ksmc, zxts, cydffbz, jbr, ypjldw, qezfbz, grbh, xm, kh)
                            values ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', {9}, {10}, {11}, '{12}', '{13}', {14}, '{15}'
                            , '{16}', '{17}', '{18}', '{19}', '{20}', '{21}', '{22}', '{23}', '{24}', '{25}', '{26}', '{27}', '{28}', '{29}')"
                            , jzlsh, jylsh, ybsfxmzldm, ybsflbdm, ybcfh, cfsj, yyxmbh, ybxmbh, yyxmmc, Math.Round(dj, 4).ToString(), Math.Round(sl, 2).ToString(), Math.Round(je, 4).ToString()
                            , "101", "", mcyl, "", ysdm, ysxm, "", "", ksdm, ksmc, "", cydffbz, jbr, ypjldw, "", grbh, xm, kh);
                            lizysfdj.Add(strSql1);
                            rc.Append(ybjzlsh + "|" + ybsfxmzldm + "|" + ybsflbdm + "|" + ybcfh + "|" + cfsj + "|" + yyxmbh
                            + "|" + ybxmbh + "|" + yyxmmc + "|" + Math.Round(dj, 4).ToString() + "|" + Math.Round(sl, 2).ToString() + "|" + Math.Round(je, 4).ToString()
                            + "|101||" + mcyl.ToString() + "||" + ysdm + "|" + ysxm + "|||" + ksdm + "|" + ksmc + "||" + cydffbz + "|"
                            + jbr + "|" + ypjldw + "||" + grbh + "|" + xm + "|" + kh + "|$");
                            int scjls = 100;

                            #region 分批上传
                            if (dtcount > scjls)
                            {
                                isfpsc = true;

                                if ((k + 1) % scjls == 0)
                                {
                                    StringBuilder inputData = new StringBuilder(string.Format("{0}^{1}^{2}^{3}^{4}^{5}^{6}^{7}^", "2310", DDYLJGBH, czygh, ywzqh, jylsh, ZXBM, rc.ToString().TrimEnd('$'), "1"));
                                    StringBuilder outputData = new StringBuilder(100000);
                                    int i = BUSINESS_HANDLE(inputData, outputData);

                                    if (i == 0)
                                    {
                                        isybzycfmxsb = true;
                                        string[] zysfdjfhs = outputData.ToString().Split('^')[2].TrimEnd('$').Split('$');

                                        if (ydrybz == "0")
                                        {
                                            //begin判断返回记录数是否和上传记录数相等
                                            if (zysfdjfhs.Length != liybcfh.Count || zysfdjfhs.Length != liyyxmdm.Count || zysfdjfhs.Length != liyyxmmc.Count)
                                            {
                                                i = ChangeYBCFMXSDCXDR(new object[]{ybjzlsh, grbh, xm, kh, ZXBM});

                                                if (i == 1)
                                                {
                                                    return new object[] { 0, 0, "医保提示：医保返回记录数和上传记录数不相等,自动撤销医保处方明细成功" };
                                                }
                                                else
                                                {
                                                    return new object[] { 0, 0, "医保提示：医保返回记录数和上传记录数不相等,自动撤销医保处方明细失败" };
                                                }
                                            }
                                            //end判断返回记录数是否和上传记录数相等

                                            for (int j = 0; j < zysfdjfhs.Length; j++)
                                            {
                                                string[] zysfdjfh = zysfdjfhs[j].Split('|');
                                                bool isConvert = decimal.TryParse(zysfdjfh[0], out je);
                                                decimal zlje;
                                                isConvert = decimal.TryParse(zysfdjfh[1], out zlje);
                                                decimal zfje;
                                                isConvert = decimal.TryParse(zysfdjfh[2], out zfje);
                                                decimal cxjzfje;
                                                isConvert = decimal.TryParse(zysfdjfh[3], out cxjzfje);
                                                string sflb1 = zysfdjfh[4];
                                                string sfxmdj = zysfdjfh[5];
                                                string qezfbz = zysfdjfh[6];
                                                decimal zlbl;
                                                isConvert = decimal.TryParse(zysfdjfh[7], out zlbl);
                                                decimal xj;
                                                isConvert = decimal.TryParse(zysfdjfh[8], out xj);
                                                string bz = zysfdjfh[9];
                                                strSql1 = string.Format(@"insert into ybcfmxscfhdr(jzlsh, jylsh, je, zlje, zfje, cxjzfje, sflb, sfxmdj, qezfbz, zlbl, xj, bz, grbh, xm, kh, cfh, ybcfh, yyxmdm, yyxmmc) 
                                                values('{0}', '{1}', {2}, {3}, {4}, {5}, '{6}', '{7}', '{8}', {9}, {10}, '{11}', '{12}', '{13}', '{14}', '{15}', '{16}', '{17}', '{18}')"
                                                , jzlsh, jylsh, je, zlje, zfje, cxjzfje, sflb1, sfxmdj, qezfbz, zlbl, xj, bz, grbh, xm, kh, jzlsh, liybcfh[j], liyyxmdm[j], liyyxmmc[j]);
                                                lizysfdj.Add(strSql1);
                                            }
                                        }

                                        string strSql2 = string.Format("update zy03d set z3ybup = '{0}' where z3comp = '{1}' and z3zyno = '{2}' and z3kind like '2%' and z3ybup is null", jylsh, CliUtils.fSiteCode, jzlsh);//0秒

                                        if (!string.IsNullOrWhiteSpace(endTime))
                                        {
                                            strSql2 += " and convert(datetime, z3date) <= '" + endTime + "'";
                                        }

                                        lizysfdj.Add(strSql2);
                                        object[] obj = lizysfdj.ToArray();
                                        obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);

                                        if (obj[1].ToString() == "1")
                                        {
                                            rc.Clear();
                                            liybcfh.Clear();
                                            liyyxmdm.Clear();
                                            liyyxmmc.Clear();
                                            lizysfdj.Clear();

                                            if (k + 1 == dtcount)
                                            {
                                                return new object[] { 0, 1, outputData };
                                            }
                                        }
                                        else
                                        {
                                            Common.WriteYBLog(obj[2].ToString());
                                            i = ChangeYBCFMXSDCXDR(new object[] {ybjzlsh, grbh, xm, kh, ZXBM});

                                            if (i == 1)
                                            {
                                                return new object[] { 0, 0, "医保提示：his数据库操作失败,自动撤销医保住院处方明细成功,错误信息：" + obj[2].ToString() };
                                            }
                                            else
                                            {
                                                return new object[] { 0, 0, "医保提示：his数据库操作失败,自动撤销医保住院处方明细失败,错误信息：" + obj[2].ToString() };
                                            }
                                        }
                                    }
                                    else if (i == -1)
                                    {
                                        i = ChangeYBCFMXSDCXDR(new object[]{ybjzlsh, grbh, xm, kh, ZXBM});

                                        if (i == 1)
                                        {
                                            return new object[] { 0, -1, "医保提示：医保系统级别错误，自动撤销医保住院处方明细上传成功，" + outputData.ToString() };
                                        }
                                        else
                                        {
                                            return new object[] { 0, -1, "医保提示：医保系统级别错误，自动撤销医保住院处方明细上传失败，" + outputData.ToString() };
                                        }
                                    }
                                    else
                                    {
                                        return new object[] { 0, -2, "医保提示：医保住院处方明细上传业务级别或未知错误，" + outputData.ToString() };
                                    }
                                }
                                else
                                {
                                    if (dtcount % scjls > 0 && k + 1 == dtcount)
                                    {
                                        StringBuilder inputData = new StringBuilder(string.Format("{0}^{1}^{2}^{3}^{4}^{5}^{6}^{7}^", "2310", DDYLJGBH, czygh, ywzqh, jylsh, ZXBM, rc.ToString().TrimEnd('$'), "1"));
                                        StringBuilder outputData = new StringBuilder(100000);
                                        int i = BUSINESS_HANDLE(inputData, outputData);

                                        if (i == 0)
                                        {
                                            isybzycfmxsb = true;
                                            string[] zysfdjfhs = outputData.ToString().Split('^')[2].TrimEnd('$').Split('$');

                                            if (ydrybz == "0")
                                            {
                                                //begin判断返回记录数是否和上传记录数相等
                                                if (zysfdjfhs.Length != liybcfh.Count || zysfdjfhs.Length != liyyxmdm.Count || zysfdjfhs.Length != liyyxmmc.Count)
                                                {
                                                    i = ChangeYBCFMXSDCXDR(new object[]{ybjzlsh, grbh, xm, kh, ZXBM});

                                                    if (i == 1)
                                                    {
                                                        return new object[] { 0, 0, "医保提示：医保返回记录数和上传记录数不相等,自动撤销医保处方明细成功" };
                                                    }
                                                    else
                                                    {
                                                        return new object[] { 0, 0, "医保提示：医保返回记录数和上传记录数不相等,自动撤销医保处方明细失败" };
                                                    }
                                                }
                                                //end判断返回记录数是否和上传记录数相等

                                                for (int j = 0; j < zysfdjfhs.Length; j++)
                                                {
                                                    string[] zysfdjfh = zysfdjfhs[j].Split('|');
                                                    bool isConvert = decimal.TryParse(zysfdjfh[0], out je);
                                                    decimal zlje;
                                                    isConvert = decimal.TryParse(zysfdjfh[1], out zlje);
                                                    decimal zfje;
                                                    isConvert = decimal.TryParse(zysfdjfh[2], out zfje);
                                                    decimal cxjzfje;
                                                    isConvert = decimal.TryParse(zysfdjfh[3], out cxjzfje);
                                                    string sflb1 = zysfdjfh[4];
                                                    string sfxmdj = zysfdjfh[5];
                                                    string qezfbz = zysfdjfh[6];
                                                    decimal zlbl;
                                                    isConvert = decimal.TryParse(zysfdjfh[7], out zlbl);
                                                    decimal xj;
                                                    isConvert = decimal.TryParse(zysfdjfh[8], out xj);
                                                    string bz = zysfdjfh[9];
                                                    strSql1 = string.Format(@"insert into ybcfmxscfhdr(jzlsh, jylsh, je, zlje, zfje, cxjzfje, sflb, sfxmdj, qezfbz, zlbl, xj, bz, grbh, xm, kh, cfh, ybcfh, yyxmdm, yyxmmc) 
                                                    values('{0}', '{1}', {2}, {3}, {4}, {5}, '{6}', '{7}', '{8}', {9}, {10}, '{11}', '{12}', '{13}', '{14}', '{15}', '{16}', '{17}', '{18}')"
                                                    , jzlsh, jylsh, je, zlje, zfje, cxjzfje, sflb1, sfxmdj, qezfbz, zlbl, xj, bz, grbh, xm, kh, jzlsh, liybcfh[j], liyyxmdm[j], liyyxmmc[j]);
                                                    lizysfdj.Add(strSql1);
                                                }
                                            }

                                            string strSql2 = string.Format("update zy03d set z3ybup = '{0}' where z3comp = '{1}' and z3zyno = '{2}' and z3kind like '2%' and z3ybup is null", jylsh, CliUtils.fSiteCode, jzlsh);//0秒

                                            if (!string.IsNullOrWhiteSpace(endTime))
                                            {
                                                strSql2 += " and convert(datetime, z3date) <= '" + endTime + "'";
                                            }

                                            lizysfdj.Add(strSql2);
                                            object[] obj = lizysfdj.ToArray();
                                            obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);

                                            if (obj[1].ToString() == "1")
                                            {
                                                rc.Clear();
                                                liybcfh.Clear();
                                                liyyxmdm.Clear();
                                                liyyxmmc.Clear();
                                                lizysfdj.Clear();

                                                if (k + 1 == dtcount)
                                                {
                                                    return new object[] { 0, 1, outputData };
                                                }
                                            }
                                            else
                                            {
                                                Common.WriteYBLog(obj[2].ToString());
                                                i = ChangeYBCFMXSDCXDR(new object[]{ybjzlsh, grbh, xm, kh, ZXBM});

                                                if (i == 1)
                                                {
                                                    return new object[] { 0, 0, "医保提示：his数据库操作失败,自动撤销医保住院处方明细成功,错误信息：" + obj[2].ToString() };
                                                }
                                                else
                                                {
                                                    return new object[] { 0, 0, "医保提示：his数据库操作失败,自动撤销医保住院处方明细失败,错误信息：" + obj[2].ToString() };
                                                }
                                            }
                                        }
                                        else if (i == -1)
                                        {
                                            i = ChangeYBCFMXSDCXDR(new object[]{ybjzlsh, grbh, xm, kh, ZXBM});

                                            if (i == 1)
                                            {
                                                return new object[] { 0, -1, "医保提示：医保系统级别错误，自动撤销医保住院处方明细上传成功，" + outputData.ToString() };
                                            }
                                            else
                                            {
                                                return new object[] { 0, -1, "医保提示：医保系统级别错误，自动撤销医保住院处方明细上传失败，" + outputData.ToString() };
                                            }
                                        }
                                        else
                                        {
                                            return new object[] { 0, -2, "医保提示：医保住院处方明细上传业务级别或未知错误，" + outputData.ToString() };
                                        }
                                    }
                                }
                            }
                            #endregion 分批上传
                        }
                    }
                }
                else
                {
                    return new object[] { 0, 0, "医保提示：医保本次无住院费用上传" };
                }

                if (!isfpsc)
                {
                    StringBuilder inputData = new StringBuilder(string.Format("{0}^{1}^{2}^{3}^{4}^{5}^{6}^{7}^", "2310", DDYLJGBH, czygh, ywzqh, jylsh, ZXBM, rc.ToString().TrimEnd('$'), "1"));
                    StringBuilder outputData = new StringBuilder(100000);
                    int i = BUSINESS_HANDLE(inputData, outputData);

                    if (i == 0)
                    {
                        isybzycfmxsb = true;
                        string[] zysfdjfhs = outputData.ToString().Split('^')[2].TrimEnd('$').Split('$');

                        if (ydrybz == "0")
                        {
                            //begin判断返回记录数是否和上传记录数相等
                            if (zysfdjfhs.Length != liybcfh.Count || zysfdjfhs.Length != liyyxmdm.Count || zysfdjfhs.Length != liyyxmmc.Count)
                            {
                                i = ChangeYBCFMXSDCXDR(new object[]{ybjzlsh, grbh, xm, kh, ZXBM});

                                if (i == 1)
                                {
                                    return new object[] { 0, 0, "医保提示：医保返回记录数和上传记录数不相等,自动撤销医保处方明细成功" };
                                }
                                else
                                {
                                    return new object[] { 0, 0, "医保提示：医保返回记录数和上传记录数不相等,自动撤销医保处方明细失败" };
                                }
                            }
                            //end判断返回记录数是否和上传记录数相等

                            for (int j = 0; j < zysfdjfhs.Length; j++)
                            {
                                string[] zysfdjfh = zysfdjfhs[j].Split('|');
                                decimal je;
                                bool isConvert = decimal.TryParse(zysfdjfh[0], out je);
                                decimal zlje;
                                isConvert = decimal.TryParse(zysfdjfh[1], out zlje);
                                decimal zfje;
                                isConvert = decimal.TryParse(zysfdjfh[2], out zfje);
                                decimal cxjzfje;
                                isConvert = decimal.TryParse(zysfdjfh[3], out cxjzfje);
                                string sflb1 = zysfdjfh[4];
                                string sfxmdj = zysfdjfh[5];
                                string qezfbz = zysfdjfh[6];
                                decimal zlbl;
                                isConvert = decimal.TryParse(zysfdjfh[7], out zlbl);
                                decimal xj;
                                isConvert = decimal.TryParse(zysfdjfh[8], out xj);
                                string bz = zysfdjfh[9];
                                string strSql1 = string.Format(@"insert into ybcfmxscfhdr(jzlsh, jylsh, je, zlje, zfje, cxjzfje, sflb, sfxmdj, qezfbz, zlbl, xj, bz, grbh, xm, kh, cfh, ybcfh, yyxmdm, yyxmmc) 
                                values('{0}', '{1}', {2}, {3}, {4}, {5}, '{6}', '{7}', '{8}', {9}, {10}, '{11}', '{12}', '{13}', '{14}', '{15}', '{16}', '{17}', '{18}')"
                                , jzlsh, jylsh, je, zlje, zfje, cxjzfje, sflb1, sfxmdj, qezfbz, zlbl, xj, bz, grbh, xm, kh, jzlsh, liybcfh[j], liyyxmdm[j], liyyxmmc[j]);
                                lizysfdj.Add(strSql1);
                            }
                        }

                        string strSql2 = string.Format("update zy03d set z3ybup = '{0}' where z3comp = '{1}' and z3zyno = '{2}' and z3kind like '2%' and z3ybup is null", jylsh, CliUtils.fSiteCode, jzlsh);//0秒

                        if (!string.IsNullOrWhiteSpace(endTime))
                        {
                            strSql2 += " and convert(datetime, z3date) <= '" + endTime + "'";
                        }

                        lizysfdj.Add(strSql2);
                        object[] obj = lizysfdj.ToArray();
                        obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);

                        if (obj[1].ToString() == "1")
                        {
                            return new object[] { 0, 1, outputData };
                        }
                        else
                        {
                            Common.WriteYBLog(obj[2].ToString());
                            i = ChangeYBCFMXSDCXDR(new object[] {ybjzlsh, grbh, xm, kh, ZXBM});

                            if (i == 1)
                            {
                                return new object[] { 0, 0, "医保提示：his数据库操作失败,自动撤销医保住院处方明细成功,错误信息：" + obj[2].ToString() };
                            }
                            else
                            {
                                return new object[] { 0, 0, "医保提示：his数据库操作失败,自动撤销医保住院处方明细失败,错误信息：" + obj[2].ToString() };
                            }
                        }
                    }
                    else if (i == -1)
                    {
                        i = ChangeYBCFMXSDCXDR(new object[]{ybjzlsh, grbh, xm, kh, ZXBM});

                        if (i == 1)
                        {
                            return new object[] { 0, -1, "医保提示：医保系统级别错误，自动撤销医保住院处方明细上传成功，" + outputData.ToString() };
                        }
                        else
                        {
                            return new object[] { 0, -1, "医保提示：医保系统级别错误，自动撤销医保住院处方明细上传失败，" + outputData.ToString() };
                        }
                    }
                    else
                    {
                        return new object[] { 0, -2, "医保提示：医保住院处方明细上传业务级别或未知错误，" + outputData.ToString() };
                    }
                }
                else
                {
                    return new object[] { 0, -3, "医保提示：既不是一次也不是分批上传，请联系管理员" };
                }
            }
            catch (Exception error)
            {
                Common.WriteYBLog(error.ToString());

                if (isybzycfmxsb)
                {
                    int i = ChangeYBCFMXSDCXDR(new object[]{ybjzlsh, grbh, xm, kh, ZXBM});

                    if (i == 1)
                    {
                        return new object[] { 0, 2, "医保提示：非医保异常，自动撤销医保住院处方明细上传成功，" + error.ToString() };
                    }
                    else
                    {
                        return new object[] { 0, 2, "医保提示：非医保异常，自动撤销医保住院处方明细上传失败，" + error.ToString() };
                    }
                }

                return new object[] { 0, 2, "Error:" + error.ToString() };
            }
        }
        #endregion 住院处方明细上报

        #region 逐条住院处方明细上报
        /// <summary>
        /// 逐条住院处方明细上报20171230
        /// </summary>
        /// <param>his就诊流水号, 截止时间(中途预结算需要传，格式：yyyy-MM-dd HH:mm:ss), 截断日期(中途参保缴费需要传，格式：yyyy-MM-dd)</param>
        /// <returns>金额|自理金额|自费金额|超限价自付金额|收费类别|收费项目等级|全额自费标志|自理比例|限价|备注</returns>
        public static object[] ZTYBZYCFMXSBDR(object[] objParam)
        {
            string czygh = CliUtils.fLoginUser;
            string ywzqh = YWZQH;
            string jbr = CliUtils.fUserName;
            string jzlsh = objParam[0].ToString();
            string endTime = "";
            string jdrq = "";

            if (objParam.Length > 1 && objParam[1].ToString() != "")
            {
                endTime = Convert.ToDateTime(objParam[1]).AddDays(1).ToString();
            }

            if (objParam.Length > 2 && objParam[2].ToString() != "")
            {
                jdrq = Convert.ToDateTime(objParam[2]).AddDays(1).ToString();
            }

            if (string.IsNullOrWhiteSpace(jzlsh))
            {
                return new object[] { 0, 0, "医保提示：his就诊流水号为空" };
            }

            DateTime dqsj = Convert.ToDateTime(Common.GetServerTime());
            string jylsh = dqsj.ToString("yyyyMMddHHmmss") + "-" + DDYLJGBH + "-" + jzlsh;
            string cfsjh = jzlsh + dqsj.ToString("HHmmss");
            bool isybzycfmxsb = false;
            string ybjzlsh = "";
            string grbh = "";
            string xm = "";
            string kh = "";

            try
            {
                string strSql = string.Format("select * from zy03dw a where a.z3zyno = '{0}' and left(a.z3endv, 1) = '1'", jzlsh);//0秒
                DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);

                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    return new object[] { 0, 0, "医保提示：his就诊流水号" + jzlsh + "已进行结算" };
                }

                strSql = string.Format("select ybjzlsh, grbh, xm, kh, ydrybz, tcqh from ybmzzydjdr a where a.jzlsh = '{0}' and a.cxbz = 1 and a.jzbz = 'z'", jzlsh);//0秒
                ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);

                if (ds == null || ds.Tables[0].Rows.Count == 0)
                {
                    return new object[] { 0, 0, "医保提示：医保无住院登记信息" };
                }

                DataRow dr1 = ds.Tables[0].Rows[0];
                ybjzlsh = dr1["ybjzlsh"].ToString();
                grbh = dr1["grbh"].ToString();
                xm = dr1["xm"].ToString();
                kh = dr1["kh"].ToString();
                string ydrybz = dr1["ydrybz"].ToString();
                ZXBM = ydrybz == "0" ? "0000" : dr1["tcqh"].ToString();

                //新增
                string strSqlxz = string.Format("select a.z1outd cyrq from zy01d a where left(a.z1endv, 1) = '8' and a.z1zyno = '{0}'", jzlsh);//0秒
                DataSet dsxz = CliUtils.ExecuteSql("sybdj", "cmd", strSqlxz, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);

                if (dsxz == null || dsxz.Tables[0].Rows.Count == 0)
                {
                    return new object[] { 0, 0, "his就诊流水号" + jzlsh + "未拖出床位" };
                }
                //end新增

                strSql = string.Format(@"select y.ybxmbh, y.ybxmmc, t.z3djxx dj, sum(case left(t.z3endv, 1) when '4' then -t.z3jzcs else t.z3jzcs end) sl
                , sum(case left(t.z3endv, 1) when '4' then -t.z3jzje else t.z3jzje end) je, t.z3item yyxmbh, t.z3name yyxmmc, t.z3empn ysdm
                , t.z3kdys ysxm, c.z1ksno ksno, c.z1ksnm zxks, t.z3sfno sfno, y.sfxmzldm ybsfxmzldm, y.sflbdm ybsflbdm, min(t.z3date) cfsj
                from zy03d t
                join zy01h c on t.z3comp = c.z1comp and t.z3zyno = c.z1zyno and t.z3ghno = c.z1ghno
                left join ybhisdzdr y on t.z3item = y.hisxmbh and y.scbz = 1
                where isnull(t.z3ybup, '') = '' and isnull(t.z3jshx, '') = '' and t.z3kind like '2%' and t.z3zyno = '{0}'", jzlsh);
                strSql += " and convert(datetime, t.z3date) >= '2017-12-30'";

                if (!string.IsNullOrWhiteSpace(endTime))
                {
                    strSql += " and convert(datetime, t.z3date) < '" + endTime + "'";
                }

                strSql += @" group by y.ybxmbh, y.ybxmmc, t.z3djxx, t.z3item, t.z3name, t.z3empn, t.z3kdys, c.z1ksno, c.z1ksnm, t.z3sfno, y.sfxmzldm, y.sflbdm
                having sum(case left(t.z3endv, 1) when '4' then -t.z3jzcs else t.z3jzcs end) < 0 and sum(case left(t.z3endv, 1) when '4' then -t.z3jzje else t.z3jzje end) < 0";
                ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);

                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    string tfxx = "";
                    string xmbhs = "";

                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        xmbhs += "'" + dr["yyxmbh"].ToString() + "',";
                        tfxx += ";医院项目编码：" + dr["yyxmbh"].ToString();
                        tfxx += "医院项目名称：" + dr["yyxmmc"].ToString();
                        tfxx += "数量：" + dr["sl"].ToString();
                    }

                    return new object[] { 0, 0, "医保提示：12月30号后产生过退30号以前的费用" + tfxx, xmbhs.TrimEnd(',') };
                }

                if (ZXBM != "0000")//X-00000-0000-0000医保不予支付的西药费用
                {
                    if (string.IsNullOrWhiteSpace(jdrq))
                    {
                        strSql = string.Format(@"select y.ybxmbh, y.ybxmmc, t.z3djxx dj, sum(case left(t.z3endv, 1) when '4' then -t.z3jzcs else t.z3jzcs end) sl
                    , sum(case left(t.z3endv, 1) when '4' then -t.z3jzje else t.z3jzje end) je, t.z3item yyxmbh, t.z3name yyxmmc, t.z3empn ysdm
                    , t.z3kdys ysxm, c.z1ksno ksno, c.z1ksnm zxks, t.z3sfno sfno, y.sfxmzldm ybsfxmzldm, y.sflbdm ybsflbdm, min(t.z3date) cfsj
                    from zy03d t
                    join zy01h c on t.z3comp = c.z1comp and t.z3zyno = c.z1zyno and t.z3ghno = c.z1ghno
                    left join ybhisdzdr20171231 y on t.z3item = y.hisxmbh and y.scbz = 1
                    where isnull(t.z3ybup, '') = '' and isnull(t.z3jshx, '') = '' and t.z3kind like '2%' and t.z3zyno = '{0}'", jzlsh);
                        strSql += " and convert(datetime, t.z3date) < '2017-12-30'";

                        if (!string.IsNullOrWhiteSpace(endTime))
                        {
                            strSql += " and convert(datetime, t.z3date) < '" + endTime + "'";
                        }

                        strSql += @" group by y.ybxmbh, y.ybxmmc, t.z3djxx, t.z3item, t.z3name, t.z3empn, t.z3kdys, c.z1ksno, c.z1ksnm, t.z3sfno, y.sfxmzldm, y.sflbdm
                    having sum(case left(t.z3endv, 1) when '4' then -t.z3jzcs else t.z3jzcs end) > 0 and sum(case left(t.z3endv, 1) when '4' then -t.z3jzje else t.z3jzje end) > 0";
                        strSql += string.Format(@" union all select case y.ybxmbh when '39199999990001000001' then 'Z-00000-00000'
when '61048000000500000000' then '61048000000400000000' else y.ybxmbh
end ybxmbh, case y.ybxmbh when '39199999990001000001' then '医保不予支付的中成药费用'
when '61048000000500000000' then '人工煎药' else y.ybxmmc end ybxmmc, t.z3djxx dj, sum(case left(t.z3endv, 1) when '4' then -t.z3jzcs else t.z3jzcs end) sl
                    , sum(case left(t.z3endv, 1) when '4' then -t.z3jzje else t.z3jzje end) je, t.z3item yyxmbh, t.z3name yyxmmc, t.z3empn ysdm
                    , t.z3kdys ysxm, c.z1ksno ksno, c.z1ksnm zxks, t.z3sfno sfno, y.sfxmzldm ybsfxmzldm, y.sflbdm ybsflbdm, min(t.z3date) cfsj
                    from zy03d t
                    join zy01h c on t.z3comp = c.z1comp and t.z3zyno = c.z1zyno and t.z3ghno = c.z1ghno
                    left join ybhisdzdr y on t.z3item = y.hisxmbh and y.scbz = 1
                    where isnull(t.z3ybup, '') = '' and isnull(t.z3jshx, '') = '' and t.z3kind like '2%' and t.z3zyno = '{0}'", jzlsh);
                        strSql += " and convert(datetime, t.z3date) >= '2017-12-30'";

                        if (!string.IsNullOrWhiteSpace(endTime))
                        {
                            strSql += " and convert(datetime, t.z3date) < '" + endTime + "'";
                        }

                        strSql += @" group by case y.ybxmbh when '39199999990001000001' then 'Z-00000-00000' 
when '61048000000500000000' then '61048000000400000000' else y.ybxmbh end
, case y.ybxmbh when '39199999990001000001' then '医保不予支付的中成药费用' 
when '61048000000500000000' then '人工煎药' else y.ybxmmc end, t.z3djxx, t.z3item, t.z3name, t.z3empn, t.z3kdys, c.z1ksno, c.z1ksnm, t.z3sfno, y.sfxmzldm, y.sflbdm
                    having sum(case left(t.z3endv, 1) when '4' then -t.z3jzcs else t.z3jzcs end) > 0 and sum(case left(t.z3endv, 1) when '4' then -t.z3jzje else t.z3jzje end) > 0";//已优化
                    }
                    else
                    {
                        strSql = string.Format(@" select case y.ybxmbh when '39199999990001000001' then 'Z-00000-00000' when '61048000000500000000' then '61048000000400000000' else y.ybxmbh end ybxmbh, case y.ybxmbh when '39199999990001000001' then '医保不予支付的中成药费用' when '61048000000500000000' then '人工煎药' else y.ybxmmc end ybxmmc, t.z3djxx dj, sum(case left(t.z3endv, 1) when '4' then -t.z3jzcs else t.z3jzcs end) sl
                    , sum(case left(t.z3endv, 1) when '4' then -t.z3jzje else t.z3jzje end) je, t.z3item yyxmbh, t.z3name yyxmmc, t.z3empn ysdm
                    , t.z3kdys ysxm, c.z1ksno ksno, c.z1ksnm zxks, t.z3sfno sfno, y.sfxmzldm ybsfxmzldm, y.sflbdm ybsflbdm, min(t.z3date) cfsj
                    from zy03d t
                    join zy01h c on t.z3comp = c.z1comp and t.z3zyno = c.z1zyno and t.z3ghno = c.z1ghno
                    left join ybhisdzdr y on t.z3item = y.hisxmbh and y.scbz = 1
                    where isnull(t.z3ybup, '') = '' and isnull(t.z3jshx, '') = '' and t.z3kind like '2%' and t.z3zyno = '{0}'", jzlsh);
                        strSql += " and convert(datetime, t.z3date) < '" + jdrq + "'";

                        if (!string.IsNullOrWhiteSpace(endTime))
                        {
                            strSql += " and convert(datetime, t.z3date) < '" + endTime + "'";
                        }

                        strSql += @" group by case y.ybxmbh when '39199999990001000001' then 'Z-00000-00000' when '61048000000500000000' then '61048000000400000000' else y.ybxmbh end, case y.ybxmbh when '39199999990001000001' then '医保不予支付的中成药费用' when '61048000000500000000' then '人工煎药' else y.ybxmmc end, t.z3djxx, t.z3item, t.z3name, t.z3empn, t.z3kdys, c.z1ksno, c.z1ksnm, t.z3sfno, y.sfxmzldm, y.sflbdm
                    having sum(case left(t.z3endv, 1) when '4' then -t.z3jzcs else t.z3jzcs end) > 0 and sum(case left(t.z3endv, 1) when '4' then -t.z3jzje else t.z3jzje end) > 0";//已优化
                        strSql += string.Format(@" union all select case y.ybxmbh when '39199999990001000001' then 'Z-00000-00000' when '61048000000500000000' then '61048000000400000000' else y.ybxmbh end ybxmbh, case y.ybxmbh when '39199999990001000001' then '医保不予支付的中成药费用' when '61048000000500000000' then '人工煎药' else y.ybxmmc end ybxmmc, t.z3djxx dj, sum(case left(t.z3endv, 1) when '4' then -t.z3jzcs else t.z3jzcs end) sl
                    , sum(case left(t.z3endv, 1) when '4' then -t.z3jzje else t.z3jzje end) je, t.z3item yyxmbh, t.z3name yyxmmc, t.z3empn ysdm
                    , t.z3kdys ysxm, c.z1ksno ksno, c.z1ksnm zxks, t.z3sfno sfno, y.sfxmzldm ybsfxmzldm, y.sflbdm ybsflbdm, min(t.z3date) cfsj
                    from zy03d t
                    join zy01h c on t.z3comp = c.z1comp and t.z3zyno = c.z1zyno and t.z3ghno = c.z1ghno
                    left join ybhisdzdr y on t.z3item = y.hisxmbh and y.scbz = 1
                    where isnull(t.z3ybup, '') = '' and isnull(t.z3jshx, '') = '' and t.z3kind like '2%' and t.z3zyno = '{0}'", jzlsh);
                        strSql += " and convert(datetime, t.z3date) >= '" + jdrq + "'";

                        if (!string.IsNullOrWhiteSpace(endTime))
                        {
                            strSql += " and convert(datetime, t.z3date) < '" + endTime + "'";
                        }

                        strSql += @" group by case y.ybxmbh when '39199999990001000001' then 'Z-00000-00000' when '61048000000500000000' then '61048000000400000000' else y.ybxmbh end, case y.ybxmbh when '39199999990001000001' then '医保不予支付的中成药费用' when '61048000000500000000' then '人工煎药' else y.ybxmmc end, t.z3djxx, t.z3item, t.z3name, t.z3empn, t.z3kdys, c.z1ksno, c.z1ksnm, t.z3sfno, y.sfxmzldm, y.sflbdm
                    having sum(case left(t.z3endv, 1) when '4' then -t.z3jzcs else t.z3jzcs end) > 0 and sum(case left(t.z3endv, 1) when '4' then -t.z3jzje else t.z3jzje end) > 0";//已优化
                    }
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(jdrq))
                    {
                        strSql = string.Format(@"select y.ybxmbh, y.ybxmmc, t.z3djxx dj, sum(case left(t.z3endv, 1) when '4' then -t.z3jzcs else t.z3jzcs end) sl
                    , sum(case left(t.z3endv, 1) when '4' then -t.z3jzje else t.z3jzje end) je, t.z3item yyxmbh, t.z3name yyxmmc, t.z3empn ysdm
                    , t.z3kdys ysxm, c.z1ksno ksno, c.z1ksnm zxks, t.z3sfno sfno, y.sfxmzldm ybsfxmzldm, y.sflbdm ybsflbdm, min(t.z3date) cfsj
                    from zy03d t
                    join zy01h c on t.z3comp = c.z1comp and t.z3zyno = c.z1zyno and t.z3ghno = c.z1ghno
                    left join ybhisdzdr20171231 y on t.z3item = y.hisxmbh and y.scbz = 1
                    where isnull(t.z3ybup, '') = '' and isnull(t.z3jshx, '') = '' and t.z3kind like '2%' and t.z3zyno = '{0}'", jzlsh);
                        strSql += " and convert(datetime, t.z3date) < '2017-12-30'";

                        if (!string.IsNullOrWhiteSpace(endTime))
                        {
                            strSql += " and convert(datetime, t.z3date) < '" + endTime + "'";
                        }

                        strSql += @" group by y.ybxmbh, y.ybxmmc, t.z3djxx, t.z3item, t.z3name, t.z3empn, t.z3kdys, c.z1ksno, c.z1ksnm, t.z3sfno, y.sfxmzldm, y.sflbdm
                    having sum(case left(t.z3endv, 1) when '4' then -t.z3jzcs else t.z3jzcs end) > 0 and sum(case left(t.z3endv, 1) when '4' then -t.z3jzje else t.z3jzje end) > 0";
                        strSql += string.Format(@" union all select y.ybxmbh, y.ybxmmc, t.z3djxx dj, sum(case left(t.z3endv, 1) when '4' then -t.z3jzcs else t.z3jzcs end) sl
                    , sum(case left(t.z3endv, 1) when '4' then -t.z3jzje else t.z3jzje end) je, t.z3item yyxmbh, t.z3name yyxmmc, t.z3empn ysdm
                    , t.z3kdys ysxm, c.z1ksno ksno, c.z1ksnm zxks, t.z3sfno sfno, y.sfxmzldm ybsfxmzldm, y.sflbdm ybsflbdm, min(t.z3date) cfsj
                    from zy03d t
                    join zy01h c on t.z3comp = c.z1comp and t.z3zyno = c.z1zyno and t.z3ghno = c.z1ghno
                    left join ybhisdzdr y on t.z3item = y.hisxmbh and y.scbz = 1
                    where isnull(t.z3ybup, '') = '' and isnull(t.z3jshx, '') = '' and t.z3kind like '2%' and t.z3zyno = '{0}'", jzlsh);
                        strSql += " and convert(datetime, t.z3date) >= '2017-12-30'";

                        if (!string.IsNullOrWhiteSpace(endTime))
                        {
                            strSql += " and convert(datetime, t.z3date) < '" + endTime + "'";
                        }

                        strSql += @" group by y.ybxmbh, y.ybxmmc, t.z3djxx, t.z3item, t.z3name, t.z3empn, t.z3kdys, c.z1ksno, c.z1ksnm, t.z3sfno, y.sfxmzldm, y.sflbdm
                    having sum(case left(t.z3endv, 1) when '4' then -t.z3jzcs else t.z3jzcs end) > 0 and sum(case left(t.z3endv, 1) when '4' then -t.z3jzje else t.z3jzje end) > 0";//已优化
                    }
                    else
                    {
                        strSql = string.Format(@"select y.ybxmbh, y.ybxmmc, t.z3djxx dj, sum(case left(t.z3endv, 1) when '4' then -t.z3jzcs else t.z3jzcs end) sl
                    , sum(case left(t.z3endv, 1) when '4' then -t.z3jzje else t.z3jzje end) je, t.z3item yyxmbh, t.z3name yyxmmc, t.z3empn ysdm
                    , t.z3kdys ysxm, c.z1ksno ksno, c.z1ksnm zxks, t.z3sfno sfno, y.sfxmzldm ybsfxmzldm, y.sflbdm ybsflbdm, min(t.z3date) cfsj
                    from zy03d t
                    join zy01h c on t.z3comp = c.z1comp and t.z3zyno = c.z1zyno and t.z3ghno = c.z1ghno
                    left join ybhisdzdr y on t.z3item = y.hisxmbh and y.scbz = 1
                    where isnull(t.z3ybup, '') = '' and isnull(t.z3jshx, '') = '' and t.z3kind like '2%' and t.z3zyno = '{0}'", jzlsh);
                        strSql += " and convert(datetime, t.z3date) < '" + jdrq + "'";

                        if (!string.IsNullOrWhiteSpace(endTime))
                        {
                            strSql += " and convert(datetime, t.z3date) < '" + endTime + "'";
                        }

                        strSql += @" group by y.ybxmbh, y.ybxmmc, t.z3djxx, t.z3item, t.z3name, t.z3empn, t.z3kdys, c.z1ksno, c.z1ksnm, t.z3sfno, y.sfxmzldm, y.sflbdm
                    having sum(case left(t.z3endv, 1) when '4' then -t.z3jzcs else t.z3jzcs end) > 0 and sum(case left(t.z3endv, 1) when '4' then -t.z3jzje else t.z3jzje end) > 0";//已优化
                        strSql += string.Format(@" union all select y.ybxmbh, y.ybxmmc, t.z3djxx dj, sum(case left(t.z3endv, 1) when '4' then -t.z3jzcs else t.z3jzcs end) sl
                    , sum(case left(t.z3endv, 1) when '4' then -t.z3jzje else t.z3jzje end) je, t.z3item yyxmbh, t.z3name yyxmmc, t.z3empn ysdm
                    , t.z3kdys ysxm, c.z1ksno ksno, c.z1ksnm zxks, t.z3sfno sfno, y.sfxmzldm ybsfxmzldm, y.sflbdm ybsflbdm, min(t.z3date) cfsj
                    from zy03d t
                    join zy01h c on t.z3comp = c.z1comp and t.z3zyno = c.z1zyno and t.z3ghno = c.z1ghno
                    left join ybhisdzdr y on t.z3item = y.hisxmbh and y.scbz = 1
                    where isnull(t.z3ybup, '') = '' and isnull(t.z3jshx, '') = '' and t.z3kind like '2%' and t.z3zyno = '{0}'", jzlsh);
                        strSql += " and convert(datetime, t.z3date) >= '" + jdrq + "'";

                        if (!string.IsNullOrWhiteSpace(endTime))
                        {
                            strSql += " and convert(datetime, t.z3date) < '" + endTime + "'";
                        }

                        strSql += @" group by y.ybxmbh, y.ybxmmc, t.z3djxx, t.z3item, t.z3name, t.z3empn, t.z3kdys, c.z1ksno, c.z1ksnm, t.z3sfno, y.sfxmzldm, y.sflbdm
                    having sum(case left(t.z3endv, 1) when '4' then -t.z3jzcs else t.z3jzcs end) > 0 and sum(case left(t.z3endv, 1) when '4' then -t.z3jzje else t.z3jzje end) > 0";//已优化
                    }
                }

                ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                List<string> liybcfh = new List<string>();
                List<string> liyyxmdm = new List<string>();
                List<string> liyyxmmc = new List<string>();
                List<string> lizysfdj = new List<string>();
                StringBuilder rc = new StringBuilder();
                bool isfpsc = false;
                StringBuilder wdzxms = new StringBuilder();

                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    DataTable dt = ds.Tables[0];
                    int dtcount = dt.Rows.Count;

                    for (int k = 0; k < dtcount; k++)
                    {
                        DataRow dr = dt.Rows[k];

                        if (dr["ybxmbh"] == DBNull.Value)
                        {
                            wdzxms.Append("医保提示：项目代码：[" + dr["yyxmbh"].ToString() + "]，名称：[" + dr["yyxmmc"].ToString() + "]未对照或未上传，不能上传费用！");
                        }
                    }
                }

                if (wdzxms.Length > 0)
                {
                    return new object[] { 0, 0, wdzxms };
                }

                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    DataTable dt = ds.Tables[0];
                    int dtcount = dt.Rows.Count;

                    for (int k = 0; k < dtcount; k++)
                    {
                        DataRow dr = dt.Rows[k];

                        if (dr["ksno"] == DBNull.Value || dr["ksno"].ToString() == "")
                        {
                            return new object[] { 0, 0, "医保提示：有科室编号为空的记录" };
                        }
                        else if (dr["zxks"] == DBNull.Value || dr["zxks"].ToString() == "")
                        {
                            return new object[] { 0, 0, "医保提示：无此科室编号[" + dr["ksno"].ToString() + "]" };
                        }
                        else
                        {
                            string ybsfxmzldm = dr["ybsfxmzldm"].ToString();
                            string ybsflbdm = dr["ybsflbdm"].ToString();
                            string yyxmbh = dr["yyxmbh"].ToString();
                            string ybxmbh = dr["ybxmbh"].ToString();
                            string yyxmmc = dr["yyxmmc"].ToString();
                            decimal dj = Convert.ToDecimal(dr["dj"]);
                            decimal sl = Convert.ToDecimal(dr["sl"]);
                            decimal je = Convert.ToDecimal(dr["je"]);
                            decimal mcyl = 1;
                            string ysdm = string.IsNullOrWhiteSpace(dr["ysdm"].ToString()) ? "010001" : dr["ysdm"].ToString();
                            string ysxm = string.IsNullOrWhiteSpace(dr["ysxm"].ToString()) ? "严东标" : dr["ysxm"].ToString();
                            string ksdm = dr["ksno"].ToString();
                            string ksmc = dr["zxks"].ToString();
                            string ypjldw = "-";
                            string ybcfh = cfsjh + k.ToString();
                            string cydffbz = "0";

                            if (ybsfxmzldm == "1")
                            {
                                cydffbz = "1";
                            }

                            if (dr["sfno"].ToString() == "01" || dr["sfno"].ToString() == "02" || dr["sfno"].ToString() == "03")
                            {
                                ypjldw = "粒";
                            }

                            string cfsj = Convert.ToDateTime(dr["cfsj"]).ToString("yyyyMMddHHmmss");
                            liybcfh.Add(ybcfh);
                            liyyxmdm.Add(yyxmbh);
                            liyyxmmc.Add(yyxmmc);
                            string strSql1 = string.Format(@"insert into ybcfmxscindr(jzlsh, jylsh, sfxmzl, sflb, ybcfh, cfrq, yysfxmbm, sfxmzxbm
                            , yysfxmmc, dj, sl, je, jx, gg, mcyl, sypc, ysbm, ysxm, yf, dw, ksbh, ksmc, zxts, cydffbz, jbr, ypjldw, qezfbz, grbh, xm, kh)
                            values ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', {9}, {10}, {11}, '{12}', '{13}', {14}, '{15}'
                            , '{16}', '{17}', '{18}', '{19}', '{20}', '{21}', '{22}', '{23}', '{24}', '{25}', '{26}', '{27}', '{28}', '{29}')"
                            , jzlsh, jylsh, ybsfxmzldm, ybsflbdm, ybcfh, cfsj, yyxmbh, ybxmbh, yyxmmc, Math.Round(dj, 4).ToString(), Math.Round(sl, 2).ToString(), Math.Round(je, 4).ToString()
                            , "101", "", mcyl, "", ysdm, ysxm, "", "", ksdm, ksmc, "", cydffbz, jbr, ypjldw, "", grbh, xm, kh);
                            lizysfdj.Add(strSql1);
                            rc.Append(ybjzlsh + "|" + ybsfxmzldm + "|" + ybsflbdm + "|" + ybcfh + "|" + cfsj + "|" + yyxmbh
                            + "|" + ybxmbh + "|" + yyxmmc + "|" + Math.Round(dj, 4).ToString() + "|" + Math.Round(sl, 2).ToString() + "|" + Math.Round(je, 4).ToString()
                            + "|101||" + mcyl.ToString() + "||" + ysdm + "|" + ysxm + "|||" + ksdm + "|" + ksmc + "||" + cydffbz + "|"
                            + jbr + "|" + ypjldw + "||" + grbh + "|" + xm + "|" + kh + "|$");
                            int scjls = 1;

                            #region 分批上传
                            if (dtcount > scjls)
                            {
                                isfpsc = true;

                                if ((k + 1) % scjls == 0)
                                {
                                    StringBuilder inputData = new StringBuilder(string.Format("{0}^{1}^{2}^{3}^{4}^{5}^{6}^{7}^", "2310", DDYLJGBH, czygh, ywzqh, jylsh, ZXBM, rc.ToString().TrimEnd('$'), "1"));
                                    StringBuilder outputData = new StringBuilder(100000);
                                    int i = BUSINESS_HANDLE(inputData, outputData);

                                    if (i == 0)
                                    {
                                        isybzycfmxsb = true;
                                        string[] zysfdjfhs = outputData.ToString().Split('^')[2].TrimEnd('$').Split('$');

                                        if (ydrybz == "0")
                                        {
                                            //begin判断返回记录数是否和上传记录数相等
                                            if (zysfdjfhs.Length != liybcfh.Count || zysfdjfhs.Length != liyyxmdm.Count || zysfdjfhs.Length != liyyxmmc.Count)
                                            {
                                                i = ChangeYBCFMXSDCXDR(new object[] { ybjzlsh, grbh, xm, kh, ZXBM });

                                                if (i == 1)
                                                {
                                                    return new object[] { 0, 0, "医保提示：医保返回记录数和上传记录数不相等,自动撤销医保处方明细成功" };
                                                }
                                                else
                                                {
                                                    return new object[] { 0, 0, "医保提示：医保返回记录数和上传记录数不相等,自动撤销医保处方明细失败" };
                                                }
                                            }
                                            //end判断返回记录数是否和上传记录数相等

                                            for (int j = 0; j < zysfdjfhs.Length; j++)
                                            {
                                                string[] zysfdjfh = zysfdjfhs[j].Split('|');
                                                bool isConvert = decimal.TryParse(zysfdjfh[0], out je);
                                                decimal zlje;
                                                isConvert = decimal.TryParse(zysfdjfh[1], out zlje);
                                                decimal zfje;
                                                isConvert = decimal.TryParse(zysfdjfh[2], out zfje);
                                                decimal cxjzfje;
                                                isConvert = decimal.TryParse(zysfdjfh[3], out cxjzfje);
                                                string sflb1 = zysfdjfh[4];
                                                string sfxmdj = zysfdjfh[5];
                                                string qezfbz = zysfdjfh[6];
                                                decimal zlbl;
                                                isConvert = decimal.TryParse(zysfdjfh[7], out zlbl);
                                                decimal xj;
                                                isConvert = decimal.TryParse(zysfdjfh[8], out xj);
                                                string bz = zysfdjfh[9];
                                                strSql1 = string.Format(@"insert into ybcfmxscfhdr(jzlsh, jylsh, je, zlje, zfje, cxjzfje, sflb, sfxmdj, qezfbz, zlbl, xj, bz, grbh, xm, kh, cfh, ybcfh, yyxmdm, yyxmmc) 
                                                values('{0}', '{1}', {2}, {3}, {4}, {5}, '{6}', '{7}', '{8}', {9}, {10}, '{11}', '{12}', '{13}', '{14}', '{15}', '{16}', '{17}', '{18}')"
                                                , jzlsh, jylsh, je, zlje, zfje, cxjzfje, sflb1, sfxmdj, qezfbz, zlbl, xj, bz, grbh, xm, kh, jzlsh, liybcfh[j], liyyxmdm[j], liyyxmmc[j]);
                                                lizysfdj.Add(strSql1);
                                            }
                                        }

                                        string strSql2 = string.Format("update zy03d set z3ybup = '{0}' where z3comp = '{1}' and z3zyno = '{2}' and z3kind like '2%' and z3ybup is null", jylsh, CliUtils.fSiteCode, jzlsh);//0秒

                                        if (!string.IsNullOrWhiteSpace(endTime))
                                        {
                                            strSql2 += " and convert(datetime, z3date) <= '" + endTime + "'";
                                        }

                                        lizysfdj.Add(strSql2);
                                        object[] obj = lizysfdj.ToArray();
                                        obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);

                                        if (obj[1].ToString() == "1")
                                        {
                                            rc.Clear();
                                            liybcfh.Clear();
                                            liyyxmdm.Clear();
                                            liyyxmmc.Clear();
                                            lizysfdj.Clear();

                                            if (k + 1 == dtcount)
                                            {
                                                return new object[] { 0, 1, outputData };
                                            }
                                        }
                                        else
                                        {
                                            Common.WriteYBLog(obj[2].ToString());
                                            i = ChangeYBCFMXSDCXDR(new object[] { ybjzlsh, grbh, xm, kh, ZXBM });

                                            if (i == 1)
                                            {
                                                return new object[] { 0, 0, "医保提示：his数据库操作失败,自动撤销医保住院处方明细成功,错误信息：" + obj[2].ToString() };
                                            }
                                            else
                                            {
                                                return new object[] { 0, 0, "医保提示：his数据库操作失败,自动撤销医保住院处方明细失败,错误信息：" + obj[2].ToString() };
                                            }
                                        }
                                    }
                                    else if (i == -1)
                                    {
                                        i = ChangeYBCFMXSDCXDR(new object[] { ybjzlsh, grbh, xm, kh, ZXBM });

                                        if (i == 1)
                                        {
                                            return new object[] { 0, -1, "医保提示：医保系统级别错误，自动撤销医保住院处方明细上传成功，" + outputData.ToString() };
                                        }
                                        else
                                        {
                                            return new object[] { 0, -1, "医保提示：医保系统级别错误，自动撤销医保住院处方明细上传失败，" + outputData.ToString() };
                                        }
                                    }
                                    else
                                    {
                                        return new object[] { 0, -2, "医保提示：医保住院处方明细上传业务级别或未知错误，" + outputData.ToString() };
                                    }
                                }
                                else
                                {
                                    if (dtcount % scjls > 0 && k + 1 == dtcount)
                                    {
                                        StringBuilder inputData = new StringBuilder(string.Format("{0}^{1}^{2}^{3}^{4}^{5}^{6}^{7}^", "2310", DDYLJGBH, czygh, ywzqh, jylsh, ZXBM, rc.ToString().TrimEnd('$'), "1"));
                                        StringBuilder outputData = new StringBuilder(100000);
                                        int i = BUSINESS_HANDLE(inputData, outputData);

                                        if (i == 0)
                                        {
                                            isybzycfmxsb = true;
                                            string[] zysfdjfhs = outputData.ToString().Split('^')[2].TrimEnd('$').Split('$');

                                            if (ydrybz == "0")
                                            {
                                                //begin判断返回记录数是否和上传记录数相等
                                                if (zysfdjfhs.Length != liybcfh.Count || zysfdjfhs.Length != liyyxmdm.Count || zysfdjfhs.Length != liyyxmmc.Count)
                                                {
                                                    i = ChangeYBCFMXSDCXDR(new object[] { ybjzlsh, grbh, xm, kh, ZXBM });

                                                    if (i == 1)
                                                    {
                                                        return new object[] { 0, 0, "医保提示：医保返回记录数和上传记录数不相等,自动撤销医保处方明细成功" };
                                                    }
                                                    else
                                                    {
                                                        return new object[] { 0, 0, "医保提示：医保返回记录数和上传记录数不相等,自动撤销医保处方明细失败" };
                                                    }
                                                }
                                                //end判断返回记录数是否和上传记录数相等

                                                for (int j = 0; j < zysfdjfhs.Length; j++)
                                                {
                                                    string[] zysfdjfh = zysfdjfhs[j].Split('|');
                                                    bool isConvert = decimal.TryParse(zysfdjfh[0], out je);
                                                    decimal zlje;
                                                    isConvert = decimal.TryParse(zysfdjfh[1], out zlje);
                                                    decimal zfje;
                                                    isConvert = decimal.TryParse(zysfdjfh[2], out zfje);
                                                    decimal cxjzfje;
                                                    isConvert = decimal.TryParse(zysfdjfh[3], out cxjzfje);
                                                    string sflb1 = zysfdjfh[4];
                                                    string sfxmdj = zysfdjfh[5];
                                                    string qezfbz = zysfdjfh[6];
                                                    decimal zlbl;
                                                    isConvert = decimal.TryParse(zysfdjfh[7], out zlbl);
                                                    decimal xj;
                                                    isConvert = decimal.TryParse(zysfdjfh[8], out xj);
                                                    string bz = zysfdjfh[9];
                                                    strSql1 = string.Format(@"insert into ybcfmxscfhdr(jzlsh, jylsh, je, zlje, zfje, cxjzfje, sflb, sfxmdj, qezfbz, zlbl, xj, bz, grbh, xm, kh, cfh, ybcfh, yyxmdm, yyxmmc) 
                                                    values('{0}', '{1}', {2}, {3}, {4}, {5}, '{6}', '{7}', '{8}', {9}, {10}, '{11}', '{12}', '{13}', '{14}', '{15}', '{16}', '{17}', '{18}')"
                                                    , jzlsh, jylsh, je, zlje, zfje, cxjzfje, sflb1, sfxmdj, qezfbz, zlbl, xj, bz, grbh, xm, kh, jzlsh, liybcfh[j], liyyxmdm[j], liyyxmmc[j]);
                                                    lizysfdj.Add(strSql1);
                                                }
                                            }

                                            string strSql2 = string.Format("update zy03d set z3ybup = '{0}' where z3comp = '{1}' and z3zyno = '{2}' and z3kind like '2%' and z3ybup is null", jylsh, CliUtils.fSiteCode, jzlsh);//0秒

                                            if (!string.IsNullOrWhiteSpace(endTime))
                                            {
                                                strSql2 += " and convert(datetime, z3date) <= '" + endTime + "'";
                                            }

                                            lizysfdj.Add(strSql2);
                                            object[] obj = lizysfdj.ToArray();
                                            obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);

                                            if (obj[1].ToString() == "1")
                                            {
                                                rc.Clear();
                                                liybcfh.Clear();
                                                liyyxmdm.Clear();
                                                liyyxmmc.Clear();
                                                lizysfdj.Clear();

                                                if (k + 1 == dtcount)
                                                {
                                                    return new object[] { 0, 1, outputData };
                                                }
                                            }
                                            else
                                            {
                                                Common.WriteYBLog(obj[2].ToString());
                                                i = ChangeYBCFMXSDCXDR(new object[] { ybjzlsh, grbh, xm, kh, ZXBM });

                                                if (i == 1)
                                                {
                                                    return new object[] { 0, 0, "医保提示：his数据库操作失败,自动撤销医保住院处方明细成功,错误信息：" + obj[2].ToString() };
                                                }
                                                else
                                                {
                                                    return new object[] { 0, 0, "医保提示：his数据库操作失败,自动撤销医保住院处方明细失败,错误信息：" + obj[2].ToString() };
                                                }
                                            }
                                        }
                                        else if (i == -1)
                                        {
                                            i = ChangeYBCFMXSDCXDR(new object[] { ybjzlsh, grbh, xm, kh, ZXBM });

                                            if (i == 1)
                                            {
                                                return new object[] { 0, -1, "医保提示：医保系统级别错误，自动撤销医保住院处方明细上传成功，" + outputData.ToString() };
                                            }
                                            else
                                            {
                                                return new object[] { 0, -1, "医保提示：医保系统级别错误，自动撤销医保住院处方明细上传失败，" + outputData.ToString() };
                                            }
                                        }
                                        else
                                        {
                                            return new object[] { 0, -2, "医保提示：医保住院处方明细上传业务级别或未知错误，" + outputData.ToString() };
                                        }
                                    }
                                }
                            }
                            #endregion 分批上传
                        }
                    }
                }
                else
                {
                    return new object[] { 0, 0, "医保提示：医保本次无住院费用上传" };
                }

                if (!isfpsc)
                {
                    StringBuilder inputData = new StringBuilder(string.Format("{0}^{1}^{2}^{3}^{4}^{5}^{6}^{7}^", "2310", DDYLJGBH, czygh, ywzqh, jylsh, ZXBM, rc.ToString().TrimEnd('$'), "1"));
                    StringBuilder outputData = new StringBuilder(100000);
                    int i = BUSINESS_HANDLE(inputData, outputData);

                    if (i == 0)
                    {
                        isybzycfmxsb = true;
                        string[] zysfdjfhs = outputData.ToString().Split('^')[2].TrimEnd('$').Split('$');

                        if (ydrybz == "0")
                        {
                            //begin判断返回记录数是否和上传记录数相等
                            if (zysfdjfhs.Length != liybcfh.Count || zysfdjfhs.Length != liyyxmdm.Count || zysfdjfhs.Length != liyyxmmc.Count)
                            {
                                i = ChangeYBCFMXSDCXDR(new object[] { ybjzlsh, grbh, xm, kh, ZXBM });

                                if (i == 1)
                                {
                                    return new object[] { 0, 0, "医保提示：医保返回记录数和上传记录数不相等,自动撤销医保处方明细成功" };
                                }
                                else
                                {
                                    return new object[] { 0, 0, "医保提示：医保返回记录数和上传记录数不相等,自动撤销医保处方明细失败" };
                                }
                            }
                            //end判断返回记录数是否和上传记录数相等

                            for (int j = 0; j < zysfdjfhs.Length; j++)
                            {
                                string[] zysfdjfh = zysfdjfhs[j].Split('|');
                                decimal je;
                                bool isConvert = decimal.TryParse(zysfdjfh[0], out je);
                                decimal zlje;
                                isConvert = decimal.TryParse(zysfdjfh[1], out zlje);
                                decimal zfje;
                                isConvert = decimal.TryParse(zysfdjfh[2], out zfje);
                                decimal cxjzfje;
                                isConvert = decimal.TryParse(zysfdjfh[3], out cxjzfje);
                                string sflb1 = zysfdjfh[4];
                                string sfxmdj = zysfdjfh[5];
                                string qezfbz = zysfdjfh[6];
                                decimal zlbl;
                                isConvert = decimal.TryParse(zysfdjfh[7], out zlbl);
                                decimal xj;
                                isConvert = decimal.TryParse(zysfdjfh[8], out xj);
                                string bz = zysfdjfh[9];
                                string strSql1 = string.Format(@"insert into ybcfmxscfhdr(jzlsh, jylsh, je, zlje, zfje, cxjzfje, sflb, sfxmdj, qezfbz, zlbl, xj, bz, grbh, xm, kh, cfh, ybcfh, yyxmdm, yyxmmc) 
                                values('{0}', '{1}', {2}, {3}, {4}, {5}, '{6}', '{7}', '{8}', {9}, {10}, '{11}', '{12}', '{13}', '{14}', '{15}', '{16}', '{17}', '{18}')"
                                , jzlsh, jylsh, je, zlje, zfje, cxjzfje, sflb1, sfxmdj, qezfbz, zlbl, xj, bz, grbh, xm, kh, jzlsh, liybcfh[j], liyyxmdm[j], liyyxmmc[j]);
                                lizysfdj.Add(strSql1);
                            }
                        }

                        string strSql2 = string.Format("update zy03d set z3ybup = '{0}' where z3comp = '{1}' and z3zyno = '{2}' and z3kind like '2%' and z3ybup is null", jylsh, CliUtils.fSiteCode, jzlsh);//0秒

                        if (!string.IsNullOrWhiteSpace(endTime))
                        {
                            strSql2 += " and convert(datetime, z3date) <= '" + endTime + "'";
                        }

                        lizysfdj.Add(strSql2);
                        object[] obj = lizysfdj.ToArray();
                        obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);

                        if (obj[1].ToString() == "1")
                        {
                            return new object[] { 0, 1, outputData };
                        }
                        else
                        {
                            Common.WriteYBLog(obj[2].ToString());
                            i = ChangeYBCFMXSDCXDR(new object[] { ybjzlsh, grbh, xm, kh, ZXBM });

                            if (i == 1)
                            {
                                return new object[] { 0, 0, "医保提示：his数据库操作失败,自动撤销医保住院处方明细成功,错误信息：" + obj[2].ToString() };
                            }
                            else
                            {
                                return new object[] { 0, 0, "医保提示：his数据库操作失败,自动撤销医保住院处方明细失败,错误信息：" + obj[2].ToString() };
                            }
                        }
                    }
                    else if (i == -1)
                    {
                        i = ChangeYBCFMXSDCXDR(new object[] { ybjzlsh, grbh, xm, kh, ZXBM });

                        if (i == 1)
                        {
                            return new object[] { 0, -1, "医保提示：医保系统级别错误，自动撤销医保住院处方明细上传成功，" + outputData.ToString() };
                        }
                        else
                        {
                            return new object[] { 0, -1, "医保提示：医保系统级别错误，自动撤销医保住院处方明细上传失败，" + outputData.ToString() };
                        }
                    }
                    else
                    {
                        return new object[] { 0, -2, "医保提示：医保住院处方明细上传业务级别或未知错误，" + outputData.ToString() };
                    }
                }
                else
                {
                    return new object[] { 0, -3, "医保提示：既不是一次也不是分批上传，请联系管理员" };
                }
            }
            catch (Exception error)
            {
                Common.WriteYBLog(error.ToString());

                if (isybzycfmxsb)
                {
                    int i = ChangeYBCFMXSDCXDR(new object[] { ybjzlsh, grbh, xm, kh, ZXBM });

                    if (i == 1)
                    {
                        return new object[] { 0, 2, "医保提示：非医保异常，自动撤销医保住院处方明细上传成功，" + error.ToString() };
                    }
                    else
                    {
                        return new object[] { 0, 2, "医保提示：非医保异常，自动撤销医保住院处方明细上传失败，" + error.ToString() };
                    }
                }

                return new object[] { 0, 2, "Error:" + error.ToString() };
            }
        }
        #endregion 逐条住院处方明细上报

        #region 住院处方明细上报（预结算调用）
        /// <summary>
        /// 住院处方明细上报（预结算调用）
        /// </summary>
        /// <param>his就诊流水号, 截止时间(中途预结算需要传，格式：yyyy-MM-dd HH:mm:ss)</param>
        /// <returns>金额|自理金额|自费金额|超限价自付金额|收费类别|收费项目等级|全额自费标志|自理比例|限价|备注</returns>
        public static object[] YBZYCFMXSBBYYJSDR(object[] objParam)
        {
            string czygh = CliUtils.fLoginUser;
            string ywzqh = YWZQH;
            string jbr = CliUtils.fUserName;
            string jzlsh = objParam[0].ToString();
            string endTime = "";

            if (objParam.Length > 1)
            {
                endTime = Convert.ToDateTime(objParam[1]).AddDays(1).ToString();
            }

            if (string.IsNullOrWhiteSpace(jzlsh))
            {
                return new object[] { 0, 0, "医保提示：his就诊流水号为空" };
            }

            DateTime dqsj = Convert.ToDateTime(Common.GetServerTime());
            string jylsh = dqsj.ToString("yyyyMMddHHmmss") + "-" + DDYLJGBH + "-" + jzlsh;
            string cfsjh = jzlsh + dqsj.ToString("HHmmss");
            bool isybzycfmxsb = false;
            string ybjzlsh = "";
            string grbh = "";
            string xm = "";
            string kh = "";

            try
            {
                string sqlDelcfmxin1 = string.Format(@"delete from ybcfmxscindr where jzlsh = '{0}' and jsdjh is null", jzlsh);//0秒
                object[] objcfmx1 = { sqlDelcfmxin1 };
                objcfmx1 = CliUtils.CallMethod("sybdj", "BatExecuteSql", objcfmx1);
                string strSql = string.Format("select ybjzlsh, grbh, xm, kh, ydrybz, tcqh from ybmzzydjdr a where a.jzlsh = '{0}' and a.cxbz = 1 and a.jzbz = 'z'", jzlsh);//0秒
                DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);

                if (ds == null || ds.Tables[0].Rows.Count == 0)
                {
                    return new object[] { 0, 0, "医保提示：医保无住院登记信息" };
                }

                DataRow dr1 = ds.Tables[0].Rows[0];
                ybjzlsh = dr1["ybjzlsh"].ToString();
                grbh = dr1["grbh"].ToString();
                xm = dr1["xm"].ToString();
                kh = dr1["kh"].ToString();
                string ydrybz = dr1["ydrybz"].ToString();
                ZXBM = ydrybz == "0" ? "0000" : dr1["tcqh"].ToString();

                if (ZXBM != "0000")
                {
                    strSql = string.Format(@"select y.ybxmbh, y.ybxmmc, t.z3djxx dj, sum(case left(t.z3endv, 1) when '4' then -t.z3jzcs else t.z3jzcs end) sl
                    , sum(case left(t.z3endv, 1) when '4' then -t.z3jzje else t.z3jzje end) je, t.z3item yyxmbh, t.z3name yyxmmc, t.z3empn ysdm
                    , t.z3kdys ysxm, c.z1ksno ksno, c.z1ksnm zxks, t.z3sfno sfno, y.sfxmzldm ybsfxmzldm, y.sflbdm ybsflbdm, min(t.z3date) cfsj
                    from zy03d t
                    join zy01h c on t.z3comp = c.z1comp and t.z3zyno = c.z1zyno and t.z3ghno = c.z1ghno
                    left join ybhisdzdr20171231 y on t.z3item = y.hisxmbh and y.scbz = 1
                    where isnull(t.z3ybup, '') = '' and isnull(t.z3jshx, '') = '' and t.z3kind like '2%' and t.z3zyno = '{0}'", jzlsh);
                    strSql += " and convert(datetime, t.z3date) < '2018-01-01'";

                    if (!string.IsNullOrWhiteSpace(endTime))
                    {
                        strSql += " and convert(datetime, t.z3date) < '" + endTime + "'";
                    }

                    strSql += @" group by y.ybxmbh, y.ybxmmc, t.z3djxx, t.z3item, t.z3name, t.z3empn, t.z3kdys, c.z1ksno, c.z1ksnm, t.z3sfno, y.sfxmzldm, y.sflbdm
                    having sum(case left(t.z3endv, 1) when '4' then -t.z3jzcs else t.z3jzcs end) > 0 and sum(case left(t.z3endv, 1) when '4' then -t.z3jzje else t.z3jzje end) > 0";
                    strSql += string.Format(@" union all select case y.ybxmbh when '39199999990001000001' then 'Z-00000-00000' when '61048000000500000000' then '61048000000400000000' else y.ybxmbh end ybxmbh, case y.ybxmbh when '39199999990001000001' then '医保不予支付的中成药费用' when '61048000000500000000' then '人工煎药' else y.ybxmmc end ybxmmc, t.z3djxx dj, sum(case left(t.z3endv, 1) when '4' then -t.z3jzcs else t.z3jzcs end) sl
                    , sum(case left(t.z3endv, 1) when '4' then -t.z3jzje else t.z3jzje end) je, t.z3item yyxmbh, t.z3name yyxmmc, t.z3empn ysdm
                    , t.z3kdys ysxm, c.z1ksno ksno, c.z1ksnm zxks, t.z3sfno sfno, y.sfxmzldm ybsfxmzldm, y.sflbdm ybsflbdm, min(t.z3date) cfsj
                    from zy03d t
                    join zy01h c on t.z3comp = c.z1comp and t.z3zyno = c.z1zyno and t.z3ghno = c.z1ghno
                    left join ybhisdzdr y on t.z3item = y.hisxmbh and y.scbz = 1
                    where isnull(t.z3ybup, '') = '' and isnull(t.z3jshx, '') = '' and t.z3kind like '2%' and t.z3zyno = '{0}'", jzlsh);
                    strSql += " and convert(datetime, t.z3date) >= '2018-01-01'";

                    if (!string.IsNullOrWhiteSpace(endTime))
                    {
                        strSql += " and convert(datetime, t.z3date) < '" + endTime + "'";
                    }

                    strSql += @" group by case y.ybxmbh when '39199999990001000001' then 'Z-00000-00000' when '61048000000500000000' then '61048000000400000000' else y.ybxmbh end, case y.ybxmbh when '39199999990001000001' then '医保不予支付的中成药费用' when '61048000000500000000' then '人工煎药' else y.ybxmmc end, t.z3djxx, t.z3item, t.z3name, t.z3empn, t.z3kdys, c.z1ksno, c.z1ksnm, t.z3sfno, y.sfxmzldm, y.sflbdm
                    having sum(case left(t.z3endv, 1) when '4' then -t.z3jzcs else t.z3jzcs end) > 0 and sum(case left(t.z3endv, 1) when '4' then -t.z3jzje else t.z3jzje end) > 0";
                }
                else
                {
                    strSql = string.Format(@"select y.ybxmbh, y.ybxmmc, t.z3djxx dj, sum(case left(t.z3endv, 1) when '4' then -t.z3jzcs else t.z3jzcs end) sl
                    , sum(case left(t.z3endv, 1) when '4' then -t.z3jzje else t.z3jzje end) je, t.z3item yyxmbh, t.z3name yyxmmc, t.z3empn ysdm
                    , t.z3kdys ysxm, c.z1ksno ksno, c.z1ksnm zxks, t.z3sfno sfno, y.sfxmzldm ybsfxmzldm, y.sflbdm ybsflbdm, min(t.z3date) cfsj
                    from zy03d t
                    join zy01h c on t.z3comp = c.z1comp and t.z3zyno = c.z1zyno and t.z3ghno = c.z1ghno
                    left join ybhisdzdr20171231 y on t.z3item = y.hisxmbh and y.scbz = 1
                    where isnull(t.z3ybup, '') = '' and isnull(t.z3jshx, '') = '' and t.z3kind like '2%' and t.z3zyno = '{0}'", jzlsh);
                    strSql += " and convert(datetime, t.z3date) < '2018-01-01'";

                    if (!string.IsNullOrWhiteSpace(endTime))
                    {
                        strSql += " and convert(datetime, t.z3date) < '" + endTime + "'";
                    }

                    strSql += @" group by y.ybxmbh, y.ybxmmc, t.z3djxx, t.z3item, t.z3name, t.z3empn, t.z3kdys, c.z1ksno, c.z1ksnm, t.z3sfno, y.sfxmzldm, y.sflbdm
                    having sum(case left(t.z3endv, 1) when '4' then -t.z3jzcs else t.z3jzcs end) > 0 and sum(case left(t.z3endv, 1) when '4' then -t.z3jzje else t.z3jzje end) > 0";
                    strSql += string.Format(@" union all select y.ybxmbh, y.ybxmmc, t.z3djxx dj, sum(case left(t.z3endv, 1) when '4' then -t.z3jzcs else t.z3jzcs end) sl
                    , sum(case left(t.z3endv, 1) when '4' then -t.z3jzje else t.z3jzje end) je, t.z3item yyxmbh, t.z3name yyxmmc, t.z3empn ysdm
                    , t.z3kdys ysxm, c.z1ksno ksno, c.z1ksnm zxks, t.z3sfno sfno, y.sfxmzldm ybsfxmzldm, y.sflbdm ybsflbdm, min(t.z3date) cfsj
                    from zy03d t
                    join zy01h c on t.z3comp = c.z1comp and t.z3zyno = c.z1zyno and t.z3ghno = c.z1ghno
                    left join ybhisdzdr y on t.z3item = y.hisxmbh and y.scbz = 1
                    where isnull(t.z3ybup, '') = '' and isnull(t.z3jshx, '') = '' and t.z3kind like '2%' and t.z3zyno = '{0}'", jzlsh);
                    strSql += " and convert(datetime, t.z3date) >= '2018-01-01'";

                    if (!string.IsNullOrWhiteSpace(endTime))
                    {
                        strSql += " and convert(datetime, t.z3date) < '" + endTime + "'";
                    }

                    strSql += @" group by y.ybxmbh, y.ybxmmc, t.z3djxx, t.z3item, t.z3name, t.z3empn, t.z3kdys, c.z1ksno, c.z1ksnm, t.z3sfno, y.sfxmzldm, y.sflbdm
                    having sum(case left(t.z3endv, 1) when '4' then -t.z3jzcs else t.z3jzcs end) > 0 and sum(case left(t.z3endv, 1) when '4' then -t.z3jzje else t.z3jzje end) > 0";
                }

                ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                List<string> liybcfh = new List<string>();
                List<string> liyyxmdm = new List<string>();
                List<string> liyyxmmc = new List<string>();
                List<string> lizysfdj = new List<string>();
                List<string> lizysfdjin = new List<string>();

                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    DataTable dt = ds.Tables[0];
                    StringBuilder wdzxms = new StringBuilder();
                    int dtcount = dt.Rows.Count;

                    for (int k = 0; k < dtcount; k++)
                    {
                        DataRow dr = dt.Rows[k];

                        if (dr["ybxmbh"] == DBNull.Value)
                        {
                            wdzxms.Append("医保提示：项目代码：[" + dr["yyxmbh"].ToString() + "]，名称：[" + dr["yyxmmc"].ToString() + "]未对照或未上传，不能上传费用");
                        }
                        else if (dr["ksno"] == DBNull.Value || dr["ksno"].ToString() == "")
                        {
                            return new object[] { 0, 0, "医保提示：有科室编号为空的记录" };
                        }
                        else if (dr["zxks"] == DBNull.Value || dr["zxks"].ToString() == "")
                        {
                            return new object[] { 0, 0, "医保提示：无此科室编号[" + dr["ksno"].ToString() + "]" };
                        }
                        else
                        {
                            string ybsfxmzldm = dr["ybsfxmzldm"].ToString();
                            string ybsflbdm = dr["ybsflbdm"].ToString();
                            string yyxmbh = dr["yyxmbh"].ToString();
                            string ybxmbh = dr["ybxmbh"].ToString();
                            string yyxmmc = dr["yyxmmc"].ToString();
                            decimal dj = Convert.ToDecimal(dr["dj"]);
                            decimal sl = Convert.ToDecimal(dr["sl"]);
                            decimal je = Convert.ToDecimal(dr["je"]);
                            decimal mcyl = 1;
                            string ysdm = string.IsNullOrWhiteSpace(dr["ysdm"].ToString()) ? "010001" : dr["ysdm"].ToString();
                            string ysxm = string.IsNullOrWhiteSpace(dr["ysxm"].ToString()) ? "严东标" : dr["ysxm"].ToString();
                            string ksdm = dr["ksno"].ToString();
                            string ksmc = dr["zxks"].ToString();
                            string ypjldw = "-";
                            string ybcfh = cfsjh + k.ToString();
                            string cydffbz = "0";

                            if (ybsfxmzldm == "1")
                            {
                                cydffbz = "1";
                            }

                            if (dr["sfno"].ToString() == "01" || dr["sfno"].ToString() == "02" || dr["sfno"].ToString() == "03")
                            {
                                ypjldw = "粒";
                            }

                            string cfsj = Convert.ToDateTime(dr["cfsj"]).ToString("yyyyMMddHHmmss");
                            string strSql1 = string.Format(@"insert into ybcfmxscindr(jzlsh, jylsh, sfxmzl, sflb, ybcfh, cfrq, yysfxmbm, sfxmzxbm
                            , yysfxmmc, dj, sl, je, jx, gg, mcyl, sypc, ysbm, ysxm, yf, dw, ksbh, ksmc, zxts, cydffbz, jbr, ypjldw, qezfbz, grbh, xm, kh)
                            values ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', {9}, {10}, {11}, '{12}', '{13}', {14}, '{15}'
                            , '{16}', '{17}', '{18}', '{19}', '{20}', '{21}', '{22}', '{23}', '{24}', '{25}', '{26}', '{27}', '{28}', '{29}')"
                            , jzlsh, jylsh, ybsfxmzldm, ybsflbdm, ybcfh, cfsj, yyxmbh, ybxmbh, yyxmmc, Math.Round(dj, 4).ToString(), Math.Round(sl, 2).ToString(), Math.Round(je, 4).ToString()
                            , "101", "", mcyl, "", ysdm, ysxm, "", "", ksdm, ksmc, "", cydffbz, jbr, ypjldw, "", grbh, xm, kh);
                            lizysfdjin.Add(strSql1);
                        }
                    }

                    if (wdzxms.Length > 0)
                    {
                        return new object[] { 0, 0, wdzxms };
                    }
                }
                else
                {
                    return new object[] { 0, 0, "医保提示：医保本次无住院费用上传" };
                }

                object[] objin = lizysfdjin.ToArray();
                objin = CliUtils.CallMethod("sybdj", "BatExecuteSql", objin);

                if (objin[1].ToString() != "1")
                {
                    return new object[] { 0, 0, "医保提示：his数据库操作失败,错误信息：" + objin[2].ToString() };
                }

                Frm_ybfyscxgdrjj frm_ybfyscxgdrjj = new Frm_ybfyscxgdrjj(jzlsh, xm);
                frm_ybfyscxgdrjj.ShowDialog();
                bool isfpsc = false;
                StringBuilder rc = new StringBuilder();
                strSql = string.Format(@"select sfxmzl, sflb, ybcfh, cfrq, yysfxmbm, sfxmzxbm, yysfxmmc, dj, sl, je
                , mcyl, ysbm, ysxm, ksbh, ksmc, ypjldw, cydffbz from ybcfmxscindr a where a.jzlsh = '{0}' and a.cxbz = 1 and a.jsdjh is null", jzlsh);//0秒
                ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);

                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    DataTable dt = ds.Tables[0];
                    int dtcount = dt.Rows.Count;

                    for (int k = 0; k < dtcount; k++)
                    {
                        DataRow dr = dt.Rows[k];
                        string ybsfxmzldm = dr["sfxmzl"].ToString();
                        string ybsflbdm = dr["sflb"].ToString();
                        string ybcfh = dr["ybcfh"].ToString();
                        string cfsj = dr["cfrq"].ToString();
                        string yyxmbh = dr["yysfxmbm"].ToString();
                        string ybxmbh = dr["sfxmzxbm"].ToString();
                        string yyxmmc = dr["yysfxmmc"].ToString();
                        decimal dj = Convert.ToDecimal(dr["dj"]);
                        decimal sl = Convert.ToDecimal(dr["sl"]);
                        decimal je = Convert.ToDecimal(dr["je"]);
                        string mcyl = dr["mcyl"].ToString();
                        string ysdm = dr["ysbm"].ToString();
                        string ysxm = dr["ysxm"].ToString();
                        string ksdm = dr["ksbh"].ToString();
                        string ksmc = dr["ksmc"].ToString();
                        string ypjldw = dr["ypjldw"].ToString();
                        string cydffbz = dr["cydffbz"].ToString();
                        rc.Append(ybjzlsh + "|" + ybsfxmzldm + "|" + ybsflbdm + "|" + ybcfh + "|" + cfsj + "|" + yyxmbh
                        + "|" + ybxmbh + "|" + yyxmmc + "|" + Math.Round(dj, 4).ToString() + "|" + Math.Round(sl, 2).ToString() + "|" + Math.Round(je, 4).ToString()
                        + "|101||" + mcyl.ToString() + "||" + ysdm + "|" + ysxm + "|||" + ksdm + "|" + ksmc + "||" + cydffbz + "|"
                        + jbr + "|" + ypjldw + "||" + grbh + "|" + xm + "|" + kh + "|$");
                        liybcfh.Add(ybcfh);
                        liyyxmdm.Add(yyxmbh);
                        liyyxmmc.Add(yyxmmc);
                        int scjls = 100;

                        #region 分批上传
                        if (dtcount > scjls)
                        {
                            isfpsc = true;

                            if ((k + 1) % scjls == 0)
                            {
                                StringBuilder inputData = new StringBuilder(string.Format("{0}^{1}^{2}^{3}^{4}^{5}^{6}^{7}^", "2310", DDYLJGBH, czygh, ywzqh, jylsh, ZXBM, rc.ToString().TrimEnd('$'), "1"));
                                StringBuilder outputData = new StringBuilder(100000);
                                int i = BUSINESS_HANDLE(inputData, outputData);

                                if (i == 0)
                                {
                                    isybzycfmxsb = true;
                                    string[] zysfdjfhs = outputData.ToString().Split('^')[2].TrimEnd('$').Split('$');

                                    if (ydrybz == "0")
                                    {
                                        //begin判断返回记录数是否和上传记录数相等
                                        if (zysfdjfhs.Length != liybcfh.Count || zysfdjfhs.Length != liyyxmdm.Count || zysfdjfhs.Length != liyyxmmc.Count)
                                        {
                                            string sqlDelcfmxin = string.Format("delete from ybcfmxscindr where jzlsh = '{0}' and jsdjh is null", jzlsh);//1秒
                                            object[] objcfmx = { sqlDelcfmxin };
                                            objcfmx = CliUtils.CallMethod("sybdj", "BatExecuteSql", objcfmx);
                                            i = ChangeYBCFMXSDCXDR(new object[] { ybjzlsh, grbh, xm, kh, ZXBM });

                                            if (i == 1)
                                            {
                                                return new object[] { 0, 0, "医保提示：医保返回记录数和上传记录数不相等,自动撤销医保处方明细成功" };
                                            }
                                            else
                                            {
                                                return new object[] { 0, 0, "医保提示：医保返回记录数和上传记录数不相等,自动撤销医保处方明细失败" };
                                            }
                                        }
                                        //end判断返回记录数是否和上传记录数相等

                                        for (int j = 0; j < zysfdjfhs.Length; j++)
                                        {
                                            string[] zysfdjfh = zysfdjfhs[j].Split('|');
                                            bool isConvert = decimal.TryParse(zysfdjfh[0], out je);
                                            decimal zlje;
                                            isConvert = decimal.TryParse(zysfdjfh[1], out zlje);
                                            decimal zfje;
                                            isConvert = decimal.TryParse(zysfdjfh[2], out zfje);
                                            decimal cxjzfje;
                                            isConvert = decimal.TryParse(zysfdjfh[3], out cxjzfje);
                                            string sflb1 = zysfdjfh[4];
                                            string sfxmdj = zysfdjfh[5];
                                            string qezfbz = zysfdjfh[6];
                                            decimal zlbl;
                                            isConvert = decimal.TryParse(zysfdjfh[7], out zlbl);
                                            decimal xj;
                                            isConvert = decimal.TryParse(zysfdjfh[8], out xj);
                                            string bz = zysfdjfh[9];
                                            string strSql1 = string.Format(@"insert into ybcfmxscfhdr(jzlsh, jylsh, je, zlje, zfje, cxjzfje, sflb, sfxmdj, qezfbz, zlbl, xj, bz, grbh, xm, kh, cfh, ybcfh, yyxmdm, yyxmmc) 
                                            values('{0}', '{1}', {2}, {3}, {4}, {5}, '{6}', '{7}', '{8}', {9}, {10}, '{11}', '{12}', '{13}', '{14}', '{15}', '{16}', '{17}', '{18}')"
                                            , jzlsh, jylsh, je, zlje, zfje, cxjzfje, sflb1, sfxmdj, qezfbz, zlbl, xj, bz, grbh, xm, kh, jzlsh, liybcfh[j], liyyxmdm[j], liyyxmmc[j]);
                                            lizysfdj.Add(strSql1);
                                        }
                                    }

                                    string strSql2 = string.Format("update zy03d set z3ybup = '{0}' where z3comp = '{1}' and z3zyno = '{2}' and z3kind like '2%' and z3ybup is null", jylsh, CliUtils.fSiteCode, jzlsh);//已优化

                                    if (!string.IsNullOrWhiteSpace(endTime))
                                    {
                                        strSql2 += " and convert(datetime, z3date) <= '" + endTime + "'";
                                    }

                                    lizysfdj.Add(strSql2);
                                    object[] obj = lizysfdj.ToArray();
                                    obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);

                                    if (obj[1].ToString() == "1")
                                    {
                                        rc.Clear();
                                        liybcfh.Clear();
                                        liyyxmdm.Clear();
                                        liyyxmmc.Clear();
                                        lizysfdj.Clear();

                                        if (k + 1 == dtcount)
                                        {
                                            return new object[] { 0, 1, outputData };
                                        }
                                    }
                                    else
                                    {
                                        Common.WriteYBLog(obj[2].ToString());
                                        string sqlDelcfmxin = string.Format("delete from ybcfmxscindr where jzlsh = '{0}' and jsdjh is null", jzlsh);//1秒
                                        object[] objcfmx = { sqlDelcfmxin };
                                        objcfmx = CliUtils.CallMethod("sybdj", "BatExecuteSql", objcfmx);
                                        i = ChangeYBCFMXSDCXDR(new object[] { ybjzlsh, grbh, xm, kh, ZXBM });

                                        if (i == 1)
                                        {
                                            return new object[] { 0, 0, "医保提示：his数据库操作失败,自动撤销医保住院处方明细成功,错误信息：" + obj[2].ToString() };
                                        }
                                        else
                                        {
                                            return new object[] { 0, 0, "医保提示：his数据库操作失败,自动撤销医保住院处方明细失败,错误信息：" + obj[2].ToString() };
                                        }
                                    }
                                }
                                else
                                {
                                    string sqlDelcfmxin = string.Format("delete from ybcfmxscindr where jzlsh = '{0}' and jsdjh is null", jzlsh);//1秒
                                    string sqlUpdateypup = string.Format("update zy03d set z3ybup = null where z3comp = '{0}' and z3zyno = '{1}' and z3kind like '2%' and z3ybup is not null", CliUtils.fSiteCode, jzlsh);//已优化
                                    object[] objcfmx = { sqlDelcfmxin, sqlUpdateypup };
                                    objcfmx = CliUtils.CallMethod("sybdj", "BatExecuteSql", objcfmx);

                                    if (i == -1)
                                    {
                                        i = ChangeYBCFMXSDCXDR(new object[] { ybjzlsh, grbh, xm, kh, ZXBM });

                                        if (i == 1)
                                        {
                                            return new object[] { 0, -1, "医保提示：医保住院处方明细上传系统级别错误，自动撤销医保住院处方明细上传成功，" + outputData.ToString() };
                                        }
                                        else
                                        {
                                            return new object[] { 0, -1, "医保提示：医保住院处方明细上传系统级别错误，自动撤销医保住院处方明细上传失败，" + outputData.ToString() };
                                        }
                                    }
                                    else
                                    {
                                        return new object[] { 0, -2, "医保提示：医保住院处方明细上传业务级别或未知错误，" + outputData.ToString() };
                                    }
                                }
                            }
                            else
                            {
                                if (dtcount % scjls > 0 && k + 1 == dtcount)
                                {
                                    StringBuilder inputData = new StringBuilder(string.Format("{0}^{1}^{2}^{3}^{4}^{5}^{6}^{7}^", "2310", DDYLJGBH, czygh, ywzqh, jylsh, ZXBM, rc.ToString().TrimEnd('$'), "1"));
                                    StringBuilder outputData = new StringBuilder(100000);
                                    int i = BUSINESS_HANDLE(inputData, outputData);

                                    if (i == 0)
                                    {
                                        isybzycfmxsb = true;
                                        string[] zysfdjfhs = outputData.ToString().Split('^')[2].TrimEnd('$').Split('$');

                                        if (ydrybz == "0")
                                        {
                                            //begin判断返回记录数是否和上传记录数相等
                                            if (zysfdjfhs.Length != liybcfh.Count || zysfdjfhs.Length != liyyxmdm.Count || zysfdjfhs.Length != liyyxmmc.Count)
                                            {
                                                string sqlDelcfmxin = string.Format("delete from ybcfmxscindr where jzlsh = '{0}' and jsdjh is null", jzlsh);//1秒
                                                object[] objcfmx = { sqlDelcfmxin };
                                                objcfmx = CliUtils.CallMethod("sybdj", "BatExecuteSql", objcfmx);
                                                i = ChangeYBCFMXSDCXDR(new object[] { ybjzlsh, grbh, xm, kh, ZXBM });

                                                if (i == 1)
                                                {
                                                    return new object[] { 0, 0, "医保提示：医保返回记录数和上传记录数不相等,自动撤销医保处方明细成功" };
                                                }
                                                else
                                                {
                                                    return new object[] { 0, 0, "医保提示：医保返回记录数和上传记录数不相等,自动撤销医保处方明细失败" };
                                                }
                                            }
                                            //end判断返回记录数是否和上传记录数相等

                                            for (int j = 0; j < zysfdjfhs.Length; j++)
                                            {
                                                string[] zysfdjfh = zysfdjfhs[j].Split('|');
                                                bool isConvert = decimal.TryParse(zysfdjfh[0], out je);
                                                decimal zlje;
                                                isConvert = decimal.TryParse(zysfdjfh[1], out zlje);
                                                decimal zfje;
                                                isConvert = decimal.TryParse(zysfdjfh[2], out zfje);
                                                decimal cxjzfje;
                                                isConvert = decimal.TryParse(zysfdjfh[3], out cxjzfje);
                                                string sflb1 = zysfdjfh[4];
                                                string sfxmdj = zysfdjfh[5];
                                                string qezfbz = zysfdjfh[6];
                                                decimal zlbl;
                                                isConvert = decimal.TryParse(zysfdjfh[7], out zlbl);
                                                decimal xj;
                                                isConvert = decimal.TryParse(zysfdjfh[8], out xj);
                                                string bz = zysfdjfh[9];
                                                string strSql1 = string.Format(@"insert into ybcfmxscfhdr(jzlsh, jylsh, je, zlje, zfje, cxjzfje, sflb, sfxmdj, qezfbz, zlbl, xj, bz, grbh, xm, kh, cfh, ybcfh, yyxmdm, yyxmmc) 
                                                values('{0}', '{1}', {2}, {3}, {4}, {5}, '{6}', '{7}', '{8}', {9}, {10}, '{11}', '{12}', '{13}', '{14}', '{15}', '{16}', '{17}', '{18}')"
                                                , jzlsh, jylsh, je, zlje, zfje, cxjzfje, sflb1, sfxmdj, qezfbz, zlbl, xj, bz, grbh, xm, kh, jzlsh, liybcfh[j], liyyxmdm[j], liyyxmmc[j]);
                                                lizysfdj.Add(strSql1);
                                            }
                                        }

                                        string strSql2 = string.Format("update zy03d set z3ybup = '{0}' where z3comp = '{1}' and z3zyno = '{2}' and z3kind like '2%' and z3ybup is null", jylsh, CliUtils.fSiteCode, jzlsh);//已优化

                                        if (!string.IsNullOrWhiteSpace(endTime))
                                        {
                                            strSql2 += " and convert(datetime, z3date) <= '" + endTime + "'";
                                        }

                                        lizysfdj.Add(strSql2);
                                        object[] obj = lizysfdj.ToArray();
                                        obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);

                                        if (obj[1].ToString() == "1")
                                        {
                                            rc.Clear();
                                            liybcfh.Clear();
                                            liyyxmdm.Clear();
                                            liyyxmmc.Clear();
                                            lizysfdj.Clear();

                                            if (k + 1 == dtcount)
                                            {
                                                return new object[] { 0, 1, outputData };
                                            }
                                        }
                                        else
                                        {
                                            Common.WriteYBLog(obj[2].ToString());
                                            string sqlDelcfmxin = string.Format("delete from ybcfmxscindr where jzlsh = '{0}' and jsdjh is null", jzlsh);//1秒
                                            object[] objcfmx = { sqlDelcfmxin };
                                            objcfmx = CliUtils.CallMethod("sybdj", "BatExecuteSql", objcfmx);
                                            i = ChangeYBCFMXSDCXDR(new object[] { ybjzlsh, grbh, xm, kh, ZXBM });

                                            if (i == 1)
                                            {
                                                return new object[] { 0, 0, "医保提示：his数据库操作失败,自动撤销医保住院处方明细成功,错误信息：" + obj[2].ToString() };
                                            }
                                            else
                                            {
                                                return new object[] { 0, 0, "医保提示：his数据库操作失败,自动撤销医保住院处方明细失败,错误信息：" + obj[2].ToString() };
                                            }
                                        }
                                    }
                                    else
                                    {
                                        string sqlDelcfmxin = string.Format("delete from ybcfmxscindr where jzlsh = '{0}' and jsdjh is null", jzlsh);//1秒
                                        object[] objcfmx = { sqlDelcfmxin };
                                        objcfmx = CliUtils.CallMethod("sybdj", "BatExecuteSql", objcfmx);

                                        if (i == -1)
                                        {
                                            i = ChangeYBCFMXSDCXDR(new object[] { ybjzlsh, grbh, xm, kh, ZXBM });

                                            if (i == 1)
                                            {
                                                return new object[] { 0, -1, "医保提示：医保住院处方明细上传系统级别错误，自动撤销医保住院处方明细上传成功，" + outputData.ToString() };
                                            }
                                            else
                                            {
                                                return new object[] { 0, -1, "医保提示：医保住院处方明细上传系统级别错误，自动撤销医保住院处方明细上传失败，" + outputData.ToString() };
                                            }
                                        }
                                        else
                                        {
                                            return new object[] { 0, -2, "医保提示：医保住院处方明细上传业务级别或未知错误，" + outputData.ToString() };
                                        }
                                    }
                                }
                            }
                        }
                        #endregion 分批上传
                    }
                }
                else
                {
                    return new object[] { 0, 0, "医保提示：医保本次无住院费用上传" };
                }

                if (!isfpsc)
                {
                    StringBuilder inputData = new StringBuilder(string.Format("{0}^{1}^{2}^{3}^{4}^{5}^{6}^{7}^", "2310", DDYLJGBH, czygh, ywzqh, jylsh, ZXBM, rc.ToString().TrimEnd('$'), "1"));
                    StringBuilder outputData = new StringBuilder(100000);
                    int i = BUSINESS_HANDLE(inputData, outputData);

                    if (i == 0)
                    {
                        isybzycfmxsb = true;
                        string[] zysfdjfhs = outputData.ToString().Split('^')[2].TrimEnd('$').Split('$');

                        if (ydrybz == "0")
                        {
                            //begin判断返回记录数是否和上传记录数相等
                            if (zysfdjfhs.Length != liybcfh.Count || zysfdjfhs.Length != liyyxmdm.Count || zysfdjfhs.Length != liyyxmmc.Count)
                            {
                                string sqlDelcfmxin = string.Format("delete from ybcfmxscindr where jzlsh = '{0}' and jsdjh is null", jzlsh);//1秒
                                object[] objcfmx = { sqlDelcfmxin };
                                objcfmx = CliUtils.CallMethod("sybdj", "BatExecuteSql", objcfmx);
                                i = ChangeYBCFMXSDCXDR(new object[] { ybjzlsh, grbh, xm, kh, ZXBM });

                                if (i == 1)
                                {
                                    return new object[] { 0, 0, "医保提示：医保返回记录数和上传记录数不相等,自动撤销医保处方明细成功" };
                                }
                                else
                                {
                                    return new object[] { 0, 0, "医保提示：医保返回记录数和上传记录数不相等,自动撤销医保处方明细失败" };
                                }
                            }
                            //end判断返回记录数是否和上传记录数相等

                            for (int j = 0; j < zysfdjfhs.Length; j++)
                            {
                                string[] zysfdjfh = zysfdjfhs[j].Split('|');
                                decimal je;
                                bool isConvert = decimal.TryParse(zysfdjfh[0], out je);
                                decimal zlje;
                                isConvert = decimal.TryParse(zysfdjfh[1], out zlje);
                                decimal zfje;
                                isConvert = decimal.TryParse(zysfdjfh[2], out zfje);
                                decimal cxjzfje;
                                isConvert = decimal.TryParse(zysfdjfh[3], out cxjzfje);
                                string sflb1 = zysfdjfh[4];
                                string sfxmdj = zysfdjfh[5];
                                string qezfbz = zysfdjfh[6];
                                decimal zlbl;
                                isConvert = decimal.TryParse(zysfdjfh[7], out zlbl);
                                decimal xj;
                                isConvert = decimal.TryParse(zysfdjfh[8], out xj);
                                string bz = zysfdjfh[9];
                                string strSql1 = string.Format(@"insert into ybcfmxscfhdr(jzlsh, jylsh, je, zlje, zfje, cxjzfje, sflb, sfxmdj, qezfbz, zlbl, xj, bz, grbh, xm, kh, cfh, ybcfh, yyxmdm, yyxmmc) 
                                values('{0}', '{1}', {2}, {3}, {4}, {5}, '{6}', '{7}', '{8}', {9}, {10}, '{11}', '{12}', '{13}', '{14}', '{15}', '{16}', '{17}', '{18}')"
                                , jzlsh, jylsh, je, zlje, zfje, cxjzfje, sflb1, sfxmdj, qezfbz, zlbl, xj, bz, grbh, xm, kh, jzlsh, liybcfh[j], liyyxmdm[j], liyyxmmc[j]);
                                lizysfdj.Add(strSql1);
                            }
                        }

                        string strSql2 = string.Format("update zy03d set z3ybup = '{0}' where z3comp = '{1}' and z3zyno = '{2}' and z3kind like '2%' and z3ybup is null", jylsh, CliUtils.fSiteCode, jzlsh);//已优化

                        if (!string.IsNullOrWhiteSpace(endTime))
                        {
                            strSql2 += " and convert(datetime, z3date) <= '" + endTime + "'";
                        }

                        lizysfdj.Add(strSql2);
                        object[] obj = lizysfdj.ToArray();
                        obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);

                        if (obj[1].ToString() == "1")
                        {
                            return new object[] { 0, 1, outputData };
                        }
                        else
                        {
                            Common.WriteYBLog(obj[2].ToString());
                            string sqlDelcfmxin = string.Format("delete from ybcfmxscindr where jzlsh = '{0}' and jsdjh is null", jzlsh);//1秒
                            object[] objcfmx = { sqlDelcfmxin };
                            objcfmx = CliUtils.CallMethod("sybdj", "BatExecuteSql", objcfmx);
                            i = ChangeYBCFMXSDCXDR(new object[] { ybjzlsh, grbh, xm, kh, ZXBM });

                            if (i == 1)
                            {
                                return new object[] { 0, 0, "医保提示：his数据库操作失败,自动撤销医保住院处方明细成功,错误信息：" + obj[2].ToString() };
                            }
                            else
                            {
                                return new object[] { 0, 0, "医保提示：his数据库操作失败,自动撤销医保住院处方明细失败,错误信息：" + obj[2].ToString() };
                            }
                        }
                    }
                    else
                    {
                        string sqlDelcfmxin = string.Format("delete from ybcfmxscindr where jzlsh = '{0}' and jsdjh is null", jzlsh);//1秒
                        object[] objcfmx = { sqlDelcfmxin };
                        objcfmx = CliUtils.CallMethod("sybdj", "BatExecuteSql", objcfmx);

                        if (i == -1)
                        {
                            i = ChangeYBCFMXSDCXDR(new object[] { ybjzlsh, grbh, xm, kh, ZXBM });

                            if (i == 1)
                            {
                                return new object[] { 0, -1, "医保提示：医保住院处方明细上传系统级别错误，自动撤销医保住院处方明细上传成功，" + outputData.ToString() };
                            }
                            else
                            {
                                return new object[] { 0, -1, "医保提示：医保住院处方明细上传系统级别错误，自动撤销医保住院处方明细上传失败，" + outputData.ToString() };
                            }
                        }
                        else
                        {
                            return new object[] { 0, -2, "医保提示：医保住院处方明细上传业务级别或未知错误，" + outputData.ToString() };
                        }
                    }
                }
                else
                {
                    return new object[] { 0, -3, "医保提示：既不是一次也不是分批上传，请联系管理员" };
                }
            }
            catch (Exception error)
            {
                Common.WriteYBLog(error.ToString());
                string sqlDelcfmxin = string.Format(@"delete from ybcfmxscindr where jzlsh = '{0}' and jsdjh is null", jzlsh);//1秒
                object[] objcfmx = { sqlDelcfmxin };
                objcfmx = CliUtils.CallMethod("sybdj", "BatExecuteSql", objcfmx);

                if (isybzycfmxsb)
                {
                    int i = ChangeYBCFMXSDCXDR(new object[] { ybjzlsh, grbh, xm, kh, ZXBM });

                    if (i == 1)
                    {
                        return new object[] { 0, 2, "医保提示：非医保异常，自动撤销医保住院处方明细上传成功，" + error.ToString() };
                    }
                    else
                    {
                        return new object[] { 0, 2, "医保提示：非医保异常，自动撤销医保住院处方明细上传失败，" + error.ToString() };
                    }
                }

                return new object[] { 0, 2, "Error:" + error.ToString() };
            }
        }
        #endregion 住院处方明细上报（预结算调用）

        #region 住院处方明细撤销
        /// <summary>
        /// 住院处方明细撤销
        /// </summary>
        /// <param>his就诊流水号</param>
        /// <returns>1:成功，0:不成功，2:报错</returns>
        public static object[] YBZYCFMXCXDR(object[] objParam)
        {
            try
            {
                string czygh = CliUtils.fLoginUser;
                string ywzqh = YWZQH;
                string jbr = CliUtils.fUserName;
                string jzlsh = objParam[0].ToString();

                if (string.IsNullOrWhiteSpace(jzlsh))
                {
                    return new object[] { 0, 0, "医保提示：his就诊流水号为空" };
                }

                DateTime dqsj = Convert.ToDateTime(Common.GetServerTime());
                string jylsh = dqsj.ToString("yyyyMMddHHmmss") + "-" + DDYLJGBH + "-" + jzlsh;
                string strSql = string.Format("select grbh, xm, kh, ybjzlsh, ydrybz, tcqh, jylsh from ybmzzydjdr a where a.jzlsh = '{0}' and a.cxbz = 1 and a.jzbz = 'z'", jzlsh);//0秒
                DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);

                if (ds == null || ds.Tables[0].Rows.Count == 0)
                {
                    return new object[] { 0, 0, "医保提示：医保无入院登记记录" };
                }

                DataTable dt = ds.Tables[0];
                DataRow dr = dt.Rows[0];
                string grbh = dr["grbh"].ToString();
                string xm = dr["xm"].ToString();
                string kh = dr["kh"].ToString();
                string ybjzlsh = dr["ybjzlsh"].ToString();
                string ydrybz = dr["ydrybz"].ToString();
                ZXBM = ydrybz == "0" ? "0000" : dr["tcqh"].ToString();

                if (ydrybz == "0" && dr["jylsh"] != DBNull.Value)
                {
                    strSql = string.Format(@"select distinct jylsh from ybcfmxscfhdr a where a.jzlsh = '{0}' and a.cxbz = 1 and a.jsdjh is null", jzlsh);//0秒
                    ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);

                    if (ds == null || ds.Tables[0].Rows.Count == 0)
                    {
                        return new object[] { 0, 0, "医保提示：医保已收费结算或无已上传费用，不能撤销" };
                    }
                }
                else
                {
                    strSql = string.Format(@"select z3zyno from zy03d a where a.z3comp = '{0}' and a.z3zyno = '{1}' and isnull(a.z3jshx, '') = '' and a.z3kind like '2%' and a.z3ybup is not null", CliUtils.fSiteCode, jzlsh);//0秒
                    ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);

                    if (ds == null && ds.Tables[0].Rows.Count == 0)
                    {
                        return new object[] { 0, 0, "医保提示：没有未结算的费用明细，不能撤销" };
                    }
                }

                string rc = ybjzlsh + "||" + jbr + "|" + grbh + "|" + xm + "|" + kh + "|";
                StringBuilder inputData = new StringBuilder(string.Format("{0}^{1}^{2}^{3}^{4}^{5}^{6}^{7}^", "2320", DDYLJGBH, czygh, ywzqh, jylsh, ZXBM, rc, "1"));
                StringBuilder outputData = new StringBuilder(100000);
                int i = BUSINESS_HANDLE(inputData, outputData);

                if (i == 0)
                {
                    string strSql1 = string.Format("update zy03d set z3ybup = null where z3comp = '{0}' and z3zyno = '{1}' and z3jshx is null", CliUtils.fSiteCode, jzlsh);//0秒
                    string strSql2 = string.Format("delete from ybcfmxscfhdr where jzlsh = '{0}' and cxbz = 1 and jsdjh is null", jzlsh);//0秒
                    string strSql3 = string.Format("delete from ybcfmxscindr where jzlsh = '{0}' and cxbz = 1 and jsdjh is null", jzlsh);//0秒
                    object[] obj = { strSql1, strSql2, strSql3 };
                    obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);

                    if (obj[1].ToString() == "1")
                    {
                        return new object[] { 0, 1, outputData };
                    }
                    else
                    {
                        Common.WriteYBLog(obj[2].ToString());
                        return new object[] { 0, 0, "医保提示：his数据库操作失败：" + obj[2].ToString() };
                    }
                }
                else if (i == -1)
                {
                    string strSql1 = string.Format("update zy03d set z3ybup = null where z3comp = '{0}' and z3zyno = '{1}' and z3jshx is null", CliUtils.fSiteCode, jzlsh);//0秒
                    string strSql2 = string.Format("delete from ybcfmxscfhdr where jzlsh = '{0}' and cxbz = 1 and jsdjh is null", jzlsh);//0秒
                    string strSql3 = string.Format("delete from ybcfmxscindr where jzlsh = '{0}' and cxbz = 1 and jsdjh is null", jzlsh);//0秒
                    object[] obj = { strSql1, strSql2, strSql3 };
                    obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);

                    if (obj[1].ToString() == "1")
                    {
                        return new object[] { 0, -1, "医保提示：医保住院处方明细撤销系统级别错误，自动删除医保处方明细成功，" + outputData.ToString() };
                    }
                    else
                    {
                        return new object[] { 0, -1, "医保提示：医保住院处方明细撤销系统级别错误，自动删除医保处方明细失败，" + outputData.ToString() };
                    }
                }
                else
                {
                    string errorMsg = outputData.ToString();

                    if (errorMsg.Contains("系统错误"))
                    {
                        object[] re = yb_interface.ybs_interface("5008", new object[] { jzlsh, "z" });

                        if (re[0].ToString() == "1")
                        {
                            errorMsg += "数据库明细自动冲销成功，请重新上传费用";
                        }
                        else
                        {
                            errorMsg += "数据库明细自动冲销失败，请联系工程师";
                        }
                    }

                    return new object[] { 0, -2, "医保提示：医保住院处方明细撤销业务级别或未知错误，" + errorMsg };
                }
            }
            catch (Exception error)
            {
                Common.WriteYBLog(error.ToString());
                return new object[] { 0, 2, "Error:" + error.ToString() };
            }
        }
        #endregion 住院处方明细撤销

        #endregion

        #region 数据库处方明细撤销东软
        /// <summary>
        /// 数据库处方明细撤销东软
        /// </summary>
        /// <param>his就诊流水号,就诊标志(m:门诊 z:住院)</param>
        /// <returns></returns>
        public static object[] YBSJKCFMXCXDR(object[] objParam)
        {
            string jzlsh = objParam[0].ToString();
            string jzbz = objParam[1].ToString();
            object[] obj;
            string strSql1 = string.Format(@"delete from ybcfmxscindr where jzlsh = '{0}' and cxbz = 1 and jsdjh is null", jzlsh);//0秒
            string strSql2 = string.Format(@"delete from ybcfmxscfhdr where jzlsh = '{0}' and cxbz = 1 and jsdjh is null", jzlsh);//0秒
            string strSql3 = string.Format(@"update zy03d set z3ybup = null where z3zyno = '{0}'", jzlsh);//0秒

            if (jzbz == "m")
            {
                obj = new object[] { strSql1, strSql2 };
            }
            else
            {
                obj = new object[] { strSql1, strSql2, strSql3 };
            }

            obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);

            if (obj[1].ToString() == "1")
            {
                return (new object[]{"1"});
            }
            else
            {
                Common.InsertYBLog("", "", obj[2].ToString());
                return (new object[] { "0" });
            }
        }
        #endregion 数据库处方明细撤销东软

        #region 住院预结算
        /// <summary>
        /// 住院预结算
        /// </summary>
        /// <param>his就诊流水号,出院原因代码,账户使用标志（0或1）,中途结算标志（0或1）,结算时间(格式：yyyy-MM-dd HH:mm:ss),撤销之前处方明细标志(不传则撤销，传则不撤销)</param>
        /// <returns>医疗费总额|总报销金额|统筹基金支付|大额基金支付|账户支付|现金支付|公务员补助基金支付|企业补充医疗保险基金支付|自费费用|单位负担费用|医院负担费用|民政救助费用|超限价费用|乙类自理费用|丙类自理费用|符合基本医疗费用|起付标准费用|转诊转院自付费用|进入统筹费用|统筹分段自付费用|超统筹封顶线费用|进入大额报销费用|大额分段自付费用|超大额封顶线费用|人工器官自付费用|本次结算前帐户余额|本年统筹支付累计(不含本次)|本年大额支付累计(不含本次)|本年城镇居民门诊统筹支付累计(不含本次)|本年公务员补助支付累计(不含本次)|本年账户支付累计(不含本次)|本年住院次数累计(不含本次)|住院次数|姓名|结算时间|医疗类别|医疗待遇类别|经办机构编码|业务周期号|结算流水号|提示信息|单据号|交易类型|医院交易流水号|有效标志|个人编号|医疗机构编码|二次补偿金额|门慢起付累计|接收方交易流水号|个人编号|单病种补差|财政支出金额|二类门慢限额支出|二类门慢限额剩余|居民个人自付二次补偿金额|</returns>
        public static object[] YBYJSZYDR(object[] objParam)
        {
            string jzlsh = objParam[0].ToString();
            string cyyy = objParam[1].ToString();
            string zhsybz = objParam[2].ToString();
            string ztjsbz = objParam[3].ToString();
            string jssj = objParam[4].ToString();

            if (string.IsNullOrWhiteSpace(jzlsh))
            {
                return new object[] { 0, 0, "医保提示：his就诊流水号为空" };
            }
            else if (string.IsNullOrWhiteSpace(cyyy))
            {
                return new object[] { 0, 0, "医保提示：出院原因代码为空" };
            }
            else if (string.IsNullOrWhiteSpace(zhsybz))
            {
                return new object[] { 0, 0, "医保提示：账户使用标志为空" };
            }
            else if (string.IsNullOrWhiteSpace(ztjsbz))
            {
                return new object[] { 0, 0, "医保提示：中途结算标志为空" };
            }
            else if (string.IsNullOrWhiteSpace(jssj))
            {
                return new object[] { 0, 0, "医保提示：结算时间为空" };
            }

            try
            {
                string czygh = CliUtils.fLoginUser;
                string ywzqh = YWZQH;
                string jbr = CliUtils.fUserName;
                string cyrq = "";
                DateTime dqsj = Convert.ToDateTime(Common.GetServerTime());
                string dqrq = dqsj.ToString("yyyyMMddHHmmss");
                string jylsh = dqrq + "-" + DDYLJGBH + "-" + jzlsh;
                string strSql = string.Format("select * from zy03dw a where a.z3zyno = '{0}' and left(a.z3endv, 1) = '1'", jzlsh);//0秒
                DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);

                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    return new object[] { 0, 0, "医保提示：his就诊流水号" + jzlsh + "已进行结算" };
                }

                strSql = string.Format("select ybjzlsh, yllb, grbh, xm, kh, ydrybz, bzbm, bzmc, tcqh, jylsh, ghdjsj from ybmzzydjdr a where a.jzlsh = '{0}' and a.cxbz = 1 and a.jzbz = 'z'", jzlsh);//0秒
                ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);

                if (ds == null || ds.Tables[0].Rows.Count == 0)
                {
                    return new object[] { 0, 0, "医保提示：his就诊流水号" + jzlsh + "无医保入院登记记录" };
                }

                DataRow dr = ds.Tables[0].Rows[0];
                string ybjzlsh = dr["ybjzlsh"].ToString();
                string yllb = dr["yllb"].ToString();
                string grbh = dr["grbh"].ToString();
                string xm = dr["xm"].ToString();
                string kh = dr["kh"].ToString();
                string djhin = "0000";
                string ydrybz = dr["ydrybz"].ToString();
                string bzbm = dr["bzbm"].ToString();
                string bzmc = dr["bzmc"].ToString();
                string tcqh = dr["tcqh"].ToString();
                string ghdjsj = dr["ghdjsj"].ToString();
                ZXBM = ydrybz == "0" ? "0000" : tcqh;
                object[] obj;

                if (ydrybz == "0" && dr["jylsh"] != DBNull.Value)
                {
                    strSql = string.Format("select distinct jylsh from ybcfmxscfhdr a where a.jzlsh = '{0}' and a.cxbz = 1 and a.jsdjh is null", jzlsh);//0秒
                    ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);

                    if (ds == null || ds.Tables[0].Rows.Count == 0)
                    {
                        return new object[] { 0, 0, "医保提示：his就诊流水号" + jzlsh + "无医保费用明细上传记录" };
                    }

                    obj = new object[] { grbh, kh };
                    obj = YBYLDYFSXXCXDR(obj);

                    if (obj[1].ToString() == "-2")//错误
                    {
                        MessageBox.Show("医保提示：封锁信息查询异常", "提示");
                        return obj;
                    }
                    else if (obj[1].ToString() != "1")//封锁
                    {
                        if (DialogResult.Cancel == MessageBox.Show("医保提示：这张卡已被封锁，信息：" + obj[2].ToString() + ",是否继续预结算", "提示", MessageBoxButtons.OKCancel))
                        {
                            return obj;
                        }
                    }
                }

                if (ztjsbz == "1")
                {
                    dqrq = Convert.ToDateTime(jssj).ToString("yyyyMMddHHmmss");

                    if (CliUtils.fLoginUser == "040028")// || CliUtils.fLoginUser == "040044"新增 && (DateTime.ParseExact(ghdjsj.Substring(0, 8),"yyyyMMdd",System.Globalization.CultureInfo.CurrentCulture) >= new DateTime(2018,1,1))
                    {
                        object[] obj1 = { jzlsh };
                        obj1 = YBZYCFMXCXDR(obj1);

                        if (obj1[1].ToString() != "1")
                        {
                            return obj1;
                        }

                        object[] obj2 = { jzlsh, jssj };
                        obj2 = YBZYCFMXSBBYYJSDR(obj2);

                        if (obj2[1].ToString() != "1")
                        {
                            return obj2;
                        }
                    }
                }
                else
                {
                    strSql = string.Format("select a.z1outd cyrq from zy01d a where left(a.z1endv, 1) = '8' and a.z1zyno = '{0}'", jzlsh);//0秒
                    DataSet dscy = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);

                    if (dscy == null || dscy.Tables[0].Rows.Count == 0)
                    {
                        return new object[] { 0, 0, "医保提示：his就诊流水号" + jzlsh + "未拖出床位" };
                    }
                    else
                    {
                        DataRow drcy = dscy.Tables[0].Rows[0];

                        if (drcy["cyrq"] == DBNull.Value)
                        {
                            return new object[] { 0, 0, "医保提示：his就诊流水号" + jzlsh + "出科日期为空" };
                        }

                        cyrq = Convert.ToDateTime(drcy["cyrq"]).ToString("yyyyMMddHHmmss");
                        strSql = string.Format("select z3zyno from zy03d a where a.z3comp = '{0}' and a.z3zyno = '{1}' and a.z3kind like '2%' and left(a.z3endv, 1) != '4' and a.z3ybup is null", CliUtils.fSiteCode, jzlsh);//0秒
                        ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);

                        if (ds != null && ds.Tables[0].Rows.Count > 0)
                        {
                            return new object[] { 0, 0, "医保提示：his就诊流水号：" + jzlsh + "还有未上传费用" };
                        }
                        else
                        {
                            if (CliUtils.fLoginUser == "040028")// || CliUtils.fLoginUser == "040044"新增 && (DateTime.ParseExact(ghdjsj.Substring(0, 8), "yyyyMMdd", System.Globalization.CultureInfo.CurrentCulture) >= new DateTime(2018, 1, 1))
                            {
                                if (objParam.Length == 5)
                                {
                                    object[] obj1 = new object[] { jzlsh };
                                    obj1 = YBZYCFMXCXDR(obj1);

                                    if (obj1[1].ToString() != "1")
                                    {
                                        return obj1;
                                    }

                                    object[] obj2 = new object[] { jzlsh };
                                    obj2 = YBZYCFMXSBBYYJSDR(obj2);

                                    if (obj2[1].ToString() != "1")
                                    {
                                        return obj2;
                                    }
                                }
                            }
                        }
                    }
                }

                string rc = ybjzlsh + "|" + djhin + "|" + yllb + "|" + dqrq + "|" + cyrq + "|" + cyyy + "|"
                + bzbm + "|" + bzmc + "|" + zhsybz + "|" + ztjsbz + "|" + jbr + "|"
                + Gocent + "||||||||||" + grbh + "|" + xm + "|" + kh + "|0|";
                StringBuilder inputData = new StringBuilder(string.Format("{0}^{1}^{2}^{3}^{4}^{5}^{6}^{7}^", "2420", DDYLJGBH, czygh, ywzqh, jylsh, ZXBM, rc, "1"));
                StringBuilder outputData = new StringBuilder(100000);
                int i = BUSINESS_HANDLE(inputData, outputData);

                if (i == 0)
                {
                    string fjsfh = outputData.ToString().Split('^')[2];
                    string[] sfjsfh = fjsfh.Split('|');
                    string ylfze = sfjsfh[0];//医疗费总额
                    string zbxje = sfjsfh[1];//总报销金额
                    string tcjjzf = sfjsfh[2];//统筹基金支付
                    string dejjzf = sfjsfh[3];//大额基金支付
                    string zhzf = sfjsfh[4];//账户支付
                    string xjzf = sfjsfh[5];//现金支付
                    string gwybzjjzf = sfjsfh[6];//公务员补助基金支付
                    string qybcylbxjjzf = sfjsfh[7];//企业补充医疗保险基金支付
                    string zffy = sfjsfh[8];//自费费用
                    string dwfdfy = sfjsfh[9];//单位负担费用
                    string yyfdfy = sfjsfh[10];//医院负担费用
                    string mzjzfy = sfjsfh[11];//民政救助费用
                    string cxjfy = sfjsfh[12];//超限价费用
                    string ylzlfy = sfjsfh[13];//乙类自理费用
                    string blzlfy = sfjsfh[14];//丙类自理费用
                    string fhjbylfy = sfjsfh[15];//符合基本医疗费用
                    string qfbzfy = sfjsfh[16];//起付标准费用
                    string zzzyzffy = sfjsfh[17];//转诊转院自付费用
                    string jrtcfy = sfjsfh[18];//进入统筹费用
                    string tcfdzffy = sfjsfh[19];//统筹分段自付费用
                    string ctcfdxfy = sfjsfh[20];//超统筹封顶线费用
                    string jrdebxfy = sfjsfh[21];//进入大额报销费用
                    string defdzffy = sfjsfh[22];//大额分段自付费用
                    string cdefdxfy = sfjsfh[23];//超大额封顶线费用
                    string rgqgzffy = sfjsfh[24];//人工器官自付费用
                    string bcjsqzhye = sfjsfh[25];//本次结算前帐户余额
                    string bntczflj = sfjsfh[26];//本年统筹支付累计(不含本次)
                    string bndezflj = sfjsfh[27];//本年大额支付累计(不含本次)
                    string bnczjmmztczflj = sfjsfh[28];//本年城镇居民门诊统筹支付累计(不含本次)
                    string bngwybzzflj = sfjsfh[29];//本年公务员补助支付累计(不含本次)
                    string bnzhzflj = sfjsfh[30];//本年账户支付累计(不含本次) 
                    string bnzycslj = sfjsfh[31];//本年住院次数累计(不含本次)
                    string zycs = sfjsfh[32];//住院次数
                    string jsrq = sfjsfh[34];//结算时间
                    string yllb1 = sfjsfh[35];//医疗类别
                    string yldylb = sfjsfh[36];//医疗待遇类别
                    string jbjgbm = sfjsfh[37];//经办机构编码
                    string ywzqh1 = sfjsfh[38];//业务周期号
                    string jslsh = sfjsfh[39];//结算流水号
                    string tsxx = sfjsfh[40];//提示信息
                    string djh = sfjsfh[41];//单据号
                    string jylx = sfjsfh[42];//交易类型
                    string yyjylsh = sfjsfh[43];//医院交易流水号
                    string yxbz = sfjsfh[44];//有效标志
                    string grbhgl = sfjsfh[45];//个人编号管理
                    string yljgbm = sfjsfh[46];//医疗机构编码
                    string ecbcje = sfjsfh[47];//二次补偿金额
                    string mmqflj = sfjsfh[48];//门慢起付累计
                    string jsfjylsh = sfjsfh[49];//接收方交易流水号
                    string dbzbc = sfjsfh[51];//单病种补差
                    string czzcje = sfjsfh[52];//财政支出金额
                    string elmmxezc = sfjsfh[53];//二类门慢限额支出
                    string elmmxesy = sfjsfh[54];//二类门慢限额剩余
                    string jmgrzfecbcje = sfjsfh[55];//居民个人自付二次补偿金额
                    string tjje = sfjsfh[56];//体检金额
                    string syjjzf = sfjsfh[57];//生育基金支付
                    string jjjmdbydje = "0.00";//九江居民大病一段金额
                    string jjjmdbedje = "0.00";//九江居民大病二段金额
                    string jjjmjbbcfwnje = "0.00";//九江居民疾病补充范围内金额
                    string jjjmjbbcfwwje = "0.00";//九江居民疾病补充范围外金额

                    if (ydrybz == "0")
                    {
                        jjjmdbydje = sfjsfh[58];//九江居民大病一段金额
                        jjjmdbedje = sfjsfh[59];//九江居民大病二段金额
                        jjjmjbbcfwnje = sfjsfh[60];//九江居民疾病补充范围内金额
                        jjjmjbbcfwwje = sfjsfh[61];//九江居民疾病补充范围外金额
                    }

                    string strSql1 = string.Format(@"insert into ybfyyjsdr(jzlsh, jylsh, djhin, cyrq, cyyy, bzbm, bzmc, zhsybz, ztjsbz, jbr, kfsbz
                    , cyzdbm1, cyzdmc1, cyzdbm2, cyzdmc2, cyzdbm3, cyzdmc3, cyzdbm4, cyzdmc4, zlff, tes, ylfze, zbxje, tcjjzf, dejjzf, zhzf, xjzf
                    , gwybzjjzf, qybcylbxjjzf, zffy, dwfdfy, yyfdfy, mzjzfy, cxjfy, ylzlfy, blzlfy, fhjbylfy, qfbzfy, zzzyzffy, jrtcfy, tcfdzffy
                    , ctcfdxfy, jrdebxfy, defdzffy, cdefdxfy, rgqgzffy, bcjsqzhye, bntczflj, bndezflj, bnczjmmztczflj, bngwybzzflj, bnzhzflj
                    , bnzycslj, zycs, xm, jsrq, yllb, yldylb, jbjgbm, ywzqh, jslsh, tsxx, djh, jylx, yyjylsh, yxbz, grbhgl, yljgbm, ecbcje
                    , mmqflj, jsfjylsh, grbh, dbzbc, czzcje, elmmxezc, elmmxesy, jmgrzfecbcje, tjje, syjjzf, kh, jjjmdbydje, jjjmdbedje, jjjmjbbcfwnje, jjjmjbbcfwwje) 
                    values('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}'
                    , '{15}', '{16}', '{17}', '{18}', '{19}', '{20}', '{21}', '{22}', '{23}', '{24}', '{25}', '{26}', '{27}', '{28}'
                    , '{29}', '{30}', '{31}', '{32}', '{33}', '{34}', '{35}', '{36}', '{37}', '{38}', '{39}', '{40}', '{41}', '{42}'
                    , '{43}', '{44}', '{45}', '{46}', '{47}', '{48}', '{49}', '{50}', '{51}', '{52}', '{53}', '{54}', '{55}', '{56}', '{57}'
                    , '{58}', '{59}', '{60}', '{61}', '{62}', '{63}', '{64}', '{65}', '{66}', '{67}', '{68}', '{69}', '{70}', '{71}'
                    , '{72}', '{73}', '{74}', '{75}', '{76}', '{77}', '{78}', '{79}', '{80}', '{81}', '{82}', '{83}')"
                    , jzlsh, jylsh, djhin, cyrq, cyyy, bzbm, bzmc, zhsybz, ztjsbz, jbr, Gocent, "", "", "", "", "", "", "", "", "", ""
                    , ylfze, zbxje, tcjjzf, dejjzf, zhzf, xjzf, gwybzjjzf, qybcylbxjjzf, zffy, dwfdfy, yyfdfy, mzjzfy, cxjfy, ylzlfy
                    , blzlfy, fhjbylfy, qfbzfy, zzzyzffy, jrtcfy, tcfdzffy, ctcfdxfy, jrdebxfy, defdzffy, cdefdxfy, rgqgzffy, bcjsqzhye
                    , bntczflj, bndezflj, bnczjmmztczflj, bngwybzzflj, bnzhzflj, bnzycslj, zycs, xm, dqrq, yllb, yldylb, jbjgbm, ywzqh
                    , jslsh, tsxx, djh, jylx, yyjylsh, yxbz, grbhgl, yljgbm, ecbcje, mmqflj, jsfjylsh, grbh, dbzbc, czzcje, elmmxezc
                    , elmmxesy, jmgrzfecbcje, tjje, syjjzf, kh, jjjmdbydje, jjjmdbedje, jjjmjbbcfwnje, jjjmjbbcfwwje);
                    strSql = string.Format("select jzlsh from ybfyyjsdr a where a.jzlsh = '{0}'", jzlsh);//0秒
                    ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);

                    if (ds != null && ds.Tables[0].Rows.Count > 0)
                    {
                        strSql1 = string.Format(@"update ybfyyjsdr set jylsh = '{0}', cyrq = '{1}', jbr = '{2}', ylfze = '{3}', zbxje = '{4}'
                        , tcjjzf = '{5}', dejjzf = '{6}', zhzf = '{7}', xjzf = '{8}', gwybzjjzf = '{9}', qybcylbxjjzf = '{10}', zffy = '{11}'
                        , dwfdfy = '{12}', yyfdfy = '{13}', mzjzfy = '{14}', cxjfy = '{15}', ylzlfy = '{16}', blzlfy = '{17}', fhjbylfy = '{18}'
                        , qfbzfy = '{19}', zzzyzffy = '{20}', jrtcfy = '{21}', tcfdzffy = '{22}', ctcfdxfy = '{23}', jrdebxfy = '{24}', defdzffy = '{25}'
                        , cdefdxfy = '{26}', rgqgzffy = '{27}', bcjsqzhye = '{28}', bntczflj = '{29}', bndezflj = '{30}', bnczjmmztczflj = '{31}'
                        , bngwybzzflj = '{32}', bnzhzflj = '{33}', bnzycslj = '{34}', zycs = '{35}', xm = '{36}', jsrq = '{37}', yllb = '{38}'
                        , yldylb = '{39}', jbjgbm = '{40}', ywzqh = '{41}', jslsh = '{42}', tsxx = '{43}', djh = '{44}', jylx = '{45}', yyjylsh = '{46}'
                        , yxbz = '{47}', grbhgl = '{48}', yljgbm = '{49}', ecbcje = '{50}', mmqflj = '{51}', jsfjylsh = '{52}', grbh = '{53}', dbzbc = '{54}'
                        , czzcje = '{55}', elmmxezc = '{56}', elmmxesy = '{57}', jmgrzfecbcje = '{58}', tjje = '{59}', syjjzf = '{60}'
                        , kh = '{61}', jjjmdbydje = '{63}', jjjmdbedje = '{64}', jjjmjbbcfwnje = '{65}', jjjmjbbcfwwje = '{66}' where jzlsh = '{62}'"
                        , jylsh, cyrq, jbr, ylfze, zbxje, tcjjzf, dejjzf, zhzf, xjzf, gwybzjjzf, qybcylbxjjzf, zffy, dwfdfy, yyfdfy, mzjzfy, cxjfy, ylzlfy
                        , blzlfy, fhjbylfy, qfbzfy, zzzyzffy, jrtcfy, tcfdzffy, ctcfdxfy, jrdebxfy, defdzffy, cdefdxfy, rgqgzffy, bcjsqzhye
                        , bntczflj, bndezflj, bnczjmmztczflj, bngwybzzflj, bnzhzflj, bnzycslj, zycs, xm, dqrq, yllb, yldylb, jbjgbm, ywzqh
                        , jslsh, tsxx, djh, jylx, yyjylsh, yxbz, grbhgl, yljgbm, ecbcje, mmqflj, jsfjylsh, grbh, dbzbc, czzcje, elmmxezc
                        , elmmxesy, jmgrzfecbcje, tjje, syjjzf, kh, jzlsh, jjjmdbydje, jjjmdbedje, jjjmjbbcfwnje, jjjmjbbcfwwje);
                    }

                    obj = new object[] { strSql1 };
                    obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);

                    if (obj[1].ToString() == "1")
                    {
                        return new object[] { 0, 1, outputData.ToString().Split('^')[2] };
                    }
                    else
                    {
                        Common.WriteYBLog(obj[2].ToString());
                        return new object[] { 0, 0, "医保提示：his数据库操作失败,错误信息：" + obj[2].ToString() };
                    }
                }
                else
                {
                    return new object[] { 0, 0, outputData };
                }
            }
            catch (Exception error)
            {
                Common.WriteYBLog(error.ToString());
                return new object[] { 0, 2, error.ToString() };
            }
        }
        #endregion 住院预结算

        //#region 门诊结算（多个发票号，目前作废）
        ///// <summary>
        ///// 门诊结算（多个发票号）
        ///// </summary>
        ///// <param>就诊流水号,单据号和处方号集合(格式：单据号1|'处方号1','处方号2';单据号2|'处方号3','处方号4'),账户使用标志（0或1）,结算时间(格式：yyyy-MM-dd HH:mm:ss)，病种编码(非慢性病传空字符串)，病种名称(非慢性病传空字符串)，医疗类别代码</param>
        ///// <returns>1:成功，非1:不成功<</returns>
        //public static object[] YBJSDCMZDR(object[] objParam)
        //{
        //    string jzlsh = objParam[0].ToString();
        //    string djhscfhs = objParam[1].ToString();
        //    string zhsybz = objParam[2].ToString();
        //    string jssj = objParam[3].ToString();
        //    string bzbm = objParam[4].ToString();
        //    string bzmc = objParam[5].ToString();
        //    //string cfhs = objParam[6].ToString();
        //    string yllb = objParam[6].ToString();

        //    if (string.IsNullOrWhiteSpace(jzlsh))
        //    {
        //        return new object[] { 0, 0, "医保提示：就诊流水号为空" };
        //    }
        //    else if (string.IsNullOrWhiteSpace(djhscfhs))
        //    {
        //        return new object[] { 0, 0, "医保提示：单据号和处方号为空" };
        //    }
        //    else if (string.IsNullOrWhiteSpace(zhsybz))
        //    {
        //        return new object[] { 0, 0, "医保提示：账户使用标志为空" };
        //    }
        //    else if (string.IsNullOrWhiteSpace(jssj))
        //    {
        //        return new object[] { 0, 0, "医保提示：结算时间为空" };
        //    }
        //    else if (string.IsNullOrWhiteSpace(yllb))
        //    {
        //        return new object[] { 0, 0, "医保提示：医保医疗类别代码为空" };
        //    }

        //    string[] djhcfhs = djhscfhs.Split(';');
        //    object[] obj = null;

        //    for (int i = 0; i < djhcfhs.Length; i++)
        //    {
        //        string[] djscfs = djhcfhs[i].Split('|');
        //        obj = new object[] { jzlsh, djscfs[1], jssj };
        //        obj = YBMZCFMXSB(obj);

        //        if (obj[1].ToString() != "1" && i > 0)
        //        {
        //            for (int j = i - 1; j >= 0; j--)
        //            {
        //                obj = new object[] { jzlsh, djhcfhs[j].Split('|')[0], "1" };
        //                obj = YBFYJSCXDR(obj);
        //            }

        //            return new object[] { 0, 0, "" };
        //        }

        //        obj = new object[] { jzlsh, djscfs[0], zhsybz, jssj, bzbm, bzmc, djscfs[1], yllb, "1" };
        //        obj = YBJSMZDR(obj);

        //        if (obj[1].ToString() != "1" && i > 0)
        //        {
        //            for (int j = i - 1; j >= 0; j--)
        //            {
        //                obj = new object[] { jzlsh, djhcfhs[j].Split('|')[0], "1" };
        //                obj = YBFYJSCXDR(obj);
        //            }

        //            return new object[] { 0, 0, "" };
        //        }
        //    }

        //    return obj;
        //}
        //#endregion 门诊结算（多个发票号，目前作废）

        #region 住院结算
        /// <summary>
        /// 住院结算
        /// </summary>
        /// <param>his就诊流水号,单据号,出院原因代码,账户使用标志（0或1）,中途结算标志（0或1）,结算时间(格式：yyyy-MM-dd HH:mm:ss),读卡标志(不传表示读卡，传了则不读卡)</param>
        /// <returns>医疗费总额|总报销金额|统筹基金支付|大额基金支付|账户支付|现金支付|公务员补助基金支付|企业补充医疗保险基金支付|自费费用|单位负担费用|医院负担费用|民政救助费用|超限价费用|乙类自理费用|丙类自理费用|符合基本医疗费用|起付标准费用|转诊转院自付费用|进入统筹费用|统筹分段自付费用|超统筹封顶线费用|进入大额报销费用|大额分段自付费用|超大额封顶线费用|人工器官自付费用|本次结算前帐户余额|本年统筹支付累计(不含本次)|本年大额支付累计(不含本次)|本年城镇居民门诊统筹支付累计(不含本次)|本年公务员补助支付累计(不含本次)|本年账户支付累计(不含本次)|本年住院次数累计(不含本次)|住院次数|姓名|结算时间|医疗类别|医疗待遇类别|经办机构编码|业务周期号|结算流水号|提示信息|单据号|交易类型|医院交易流水号|有效标志|个人编号|医疗机构编码|二次补偿金额|门慢起付累计|接收方交易流水号|个人编号|单病种补差|财政支出金额|二类门慢限额支出|二类门慢限额剩余|居民个人自付二次补偿金额|</returns>
        public static object[] YBJSZYDR(object[] objParam)
        {
            string jzlsh = objParam[0].ToString();
            string djhin = objParam[1].ToString();
            string cyyy = objParam[2].ToString();
            string zhsybz = objParam[3].ToString();
            string ztjsbz = objParam[4].ToString();
            string jssj = objParam[5].ToString();
            bool isybok = false;
            string djh = djhin;
            string ybjzlsh = "";
            string grbh = "";
            string xm = "";
            string kh = "";
            DateTime dqsj = Convert.ToDateTime(Common.GetServerTime());
            string dqrq = dqsj.ToString("yyyyMMddHHmmss");

            try
            {
                string czygh = CliUtils.fLoginUser;
                string ywzqh = YWZQH;
                string jbr = CliUtils.fUserName;

                if (string.IsNullOrWhiteSpace(jzlsh))
                {
                    return new object[] { 0, 0, "医保提示：his就诊流水号为空" };
                }

                if (string.IsNullOrWhiteSpace(djhin))
                {
                    return new object[] { 0, 0, "医保提示：单据号为空" };
                }

                if (string.IsNullOrWhiteSpace(cyyy))
                {
                    return new object[] { 0, 0, "医保提示：出院原因代码为空" };
                }

                if (string.IsNullOrWhiteSpace(zhsybz))
                {
                    return new object[] { 0, 0, "医保提示：账户使用标志为空" };
                }

                if (string.IsNullOrWhiteSpace(ztjsbz))
                {
                    return new object[] { 0, 0, "医保提示：中途结算标志为空" };
                }

                if (string.IsNullOrWhiteSpace(jssj))
                {
                    return new object[] { 0, 0, "医保提示：结算时间为空" };
                }

                if (djhin.Length > 21)
                {
                    return new object[] { 0, 0, "医保提示：单据号太长超过21位" };
                }
                else if (djhin.Length == 21)
                {
                    djh = djhin.Substring(1);
                }

                string jylsh = dqsj.ToString("yyyyMMddHHmmss") + "-" + DDYLJGBH + "-" + jzlsh;
                string strSql = string.Format("select * from zy03dw a where a.z3zyno = '{0}' and left(a.z3endv, 1) = '1'", jzlsh);//0秒
                DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);

                //if (ds != null && ds.Tables[0].Rows.Count > 0)
                //{
                //    return new object[] { 0, 0, "医保提示：his就诊流水号" + jzlsh + "已进行结算" };
                //}

                strSql = string.Format("select bzbm, bzmc, ybjzlsh, yllb, grbh, xm, kh, ydrybz, tcqh, jylsh from ybmzzydjdr a where a.jzlsh = '{0}' and a.cxbz = 1 and a.jzbz = 'z'", jzlsh);//0秒
                ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);

                if (ds == null || ds.Tables[0].Rows.Count == 0)
                {
                    return new object[] { 0, 0, "医保提示：his就诊流水号：" + jzlsh + "无入院登记记录" };
                }

                DataRow dr = ds.Tables[0].Rows[0];
                string bzbm = dr["bzbm"].ToString();
                string bzmc = dr["bzmc"].ToString();
                string cyrq = "";
                ybjzlsh = dr["ybjzlsh"].ToString();
                string yllb = dr["yllb"].ToString();
                grbh = dr["grbh"].ToString();
                xm = dr["xm"].ToString();
                kh = dr["kh"].ToString();
                string ydrybz = dr["ydrybz"].ToString();
                ZXBM = ydrybz == "0" ? "0000" : dr["tcqh"].ToString();

                if (ydrybz == "0" && dr["jylsh"] != DBNull.Value)
                {
                    strSql = string.Format("select distinct jylsh from ybcfmxscfhdr a where a.cxbz = 1 and a.jzlsh = '{0}' and a.jsdjh is null", jzlsh);//0秒
                    ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);

                    if (ds == null || ds.Tables[0].Rows.Count == 0)
                    {
                        return new object[] { 0, 0, "医保提示：his就诊流水号：" + jzlsh + "无费用明细上传记录" };
                    }
                }

                object[] obj = null;

                if (objParam.Length == 6)
                {
                    obj = YBDKDR(new object[] { });

                    if (obj[1].ToString() == "1")
                    {
                        if (xm != obj[2].ToString().Split('|')[3])
                        {
                            return new object[] { 0, 0, "医保提示：不是本人卡" };
                        }
                    }
                    else
                    {
                        return new object[] { 0, 0, "医保提示：读卡错误：" + obj[2].ToString() };
                    }
                }

                if (ztjsbz == "0")
                {
                    strSql = string.Format("select a.z1outd cyrq from zy01d a where a.z1zyno = '{0}'", jzlsh);//0秒
                    DataSet dscy = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);

                    if (dscy == null || dscy.Tables[0].Rows.Count == 0)
                    {
                        return new object[] { 0, 0, "医保提示：his就诊流水号" + jzlsh + "未拖出床位" };
                    }
                    else
                    {
                        cyrq = Convert.ToDateTime(dscy.Tables[0].Rows[0]["cyrq"]).ToString("yyyyMMddHHmmss");
                    }
                }
                else
                {
                    dqrq = Convert.ToDateTime(jssj).ToString("yyyyMMddHHmmss");
                }

                strSql = string.Format("select jzlsh from ybfyjsdr a where a.jzlsh = '{0}' and a.djhin = '{1}' and a.cxbz = 1", jzlsh, djhin);//0秒
                ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);

                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    return new object[] { 0, 0, "医保提示：his就诊流水号：" + jzlsh + "，单据号：" + djhin + "已结算" };
                }

                string rc = ybjzlsh + "|" + djh + "|" + yllb + "|" + dqrq + "|" + cyrq + "|" + cyyy + "|"
                + bzbm + "|" + bzmc + "|" + zhsybz + "|" + ztjsbz + "|" + jbr + "|" + Gocent + "||||||||||" + grbh + "|" + xm + "|" + kh + "|0|";
                StringBuilder inputData = new StringBuilder(string.Format("{0}^{1}^{2}^{3}^{4}^{5}^{6}^{7}^", "2410", DDYLJGBH, czygh, ywzqh, jylsh, ZXBM, rc, "1"));
                StringBuilder outputData = new StringBuilder(100000);
                object[] dkobj = YBDKDR(new object[] {});
                int i = BUSINESS_HANDLE(inputData, outputData);

                if (i == 0)
                {
                    isybok = true;
                    string[] sfjsfh = outputData.ToString().Split('^')[2].Split('|');
                    string ylfze = sfjsfh[0];//医疗费总额
                    string zbxje = sfjsfh[1];//总报销金额
                    string tcjjzf = sfjsfh[2];//统筹基金支付
                    string dejjzf = sfjsfh[3];//大额基金支付
                    string zhzf = sfjsfh[4];//账户支付
                    string xjzf = sfjsfh[5];//现金支付
                    string gwybzjjzf = sfjsfh[6];//公务员补助基金支付
                    string qybcylbxjjzf = sfjsfh[7];//企业补充医疗保险基金支付
                    string zffy = sfjsfh[8];//自费费用
                    string dwfdfy = sfjsfh[9];//单位负担费用
                    string yyfdfy = sfjsfh[10];//医院负担费用
                    string mzjzfy = sfjsfh[11];//民政救助费用
                    string cxjfy = sfjsfh[12];//超限价费用
                    string ylzlfy = sfjsfh[13];//乙类自理费用
                    string blzlfy = sfjsfh[14];//丙类自理费用
                    string fhjbylfy = sfjsfh[15];//符合基本医疗费用
                    string qfbzfy = sfjsfh[16];//起付标准费用
                    string zzzyzffy = sfjsfh[17];//转诊转院自付费用
                    string jrtcfy = sfjsfh[18];//进入统筹费用
                    string tcfdzffy = sfjsfh[19];//统筹分段自付费用
                    string ctcfdxfy = sfjsfh[20];//超统筹封顶线费用
                    string jrdebxfy = sfjsfh[21];//进入大额报销费用
                    string defdzffy = sfjsfh[22];//大额分段自付费用
                    string cdefdxfy = sfjsfh[23];//超大额封顶线费用
                    string rgqgzffy = sfjsfh[24];//人工器官自付费用
                    string bcjsqzhye = sfjsfh[25];//本次结算前帐户余额
                    string bntczflj = sfjsfh[26];//本年统筹支付累计(不含本次)
                    string bndezflj = sfjsfh[27];//本年大额支付累计(不含本次)
                    string bnczjmmztczflj = sfjsfh[28];//本年城镇居民门诊统筹支付累计(不含本次)
                    string bngwybzzflj = sfjsfh[29];//本年公务员补助支付累计(不含本次)
                    string bnzhzflj = sfjsfh[30];//本年账户支付累计(不含本次) 
                    string bnzycslj = sfjsfh[31];//本年住院次数累计(不含本次)
                    string zycs = sfjsfh[32];//住院次数
                    string jsrq = sfjsfh[34];//结算时间
                    string yllb1 = sfjsfh[35];//医疗类别
                    string yldylb = sfjsfh[36];//医疗待遇类别
                    string jbjgbm = sfjsfh[37];//经办机构编码
                    string ywzqh1 = sfjsfh[38];//业务周期号
                    string jslsh = sfjsfh[39];//结算流水号
                    string tsxx = sfjsfh[40];//提示信息
                    djh = sfjsfh[41];//单据号
                    string jylx = sfjsfh[42];//交易类型
                    string yyjylsh = sfjsfh[43];//医院交易流水号
                    string yxbz = sfjsfh[44];//有效标志
                    string grbhgl = sfjsfh[45];//个人编号管理
                    string yljgbm = sfjsfh[46];//医疗机构编码
                    string ecbcje = sfjsfh[47];//二次补偿金额
                    string mmqflj = sfjsfh[48];//门慢起付累计
                    string jsfjylsh = sfjsfh[49];//接收方交易流水号
                    string dbzbc = sfjsfh[51];//单病种补差
                    string czzcje = sfjsfh[52];//财政支出金额
                    string elmmxezc = sfjsfh[53];//二类门慢限额支出
                    string elmmxesy = sfjsfh[54];//二类门慢限额剩余
                    string jmgrzfecbcje = sfjsfh[55];//居民个人自付二次补偿金额
                    string tjje = sfjsfh[56];//体检金额
                    string syjjzf = sfjsfh[57];//生育基金支付
                    string jjjmdbydje = "0.00";//九江居民大病一段金额
                    string jjjmdbedje = "0.00";//九江居民大病二段金额
                    string jjjmjbbcfwnje = "0.00";//九江居民疾病补充范围内金额
                    string jjjmjbbcfwwje = "0.00";//九江居民疾病补充范围外金额

                    string jjjmzcfwwkbxfy = sfjsfh[63]; //九江居民政策范围外可报销费用
                    string jmdbydje = sfjsfh[58]; //居民大病一段金额
                    string jmdbedje = sfjsfh[59]; //居民大病二段金额
                    string jbbcfwnzfje = sfjsfh[60]; //疾病补充范围内费用支付金额
                    string jbbcbxbczcfwwfyzfje = sfjsfh[61]; //疾病补充保险本次政策范围外费用支付金额
                    string bnzfddjjfylj = sfjsfh[74]; //本年政府兜底基金费用累计
                    string zfddjjfy = sfjsfh[64]; //政府兜底基金费用

                    if (ydrybz == "0")
                    {
                        jjjmdbydje = sfjsfh[58];//九江居民大病一段金额
                        jjjmdbedje = sfjsfh[59];//九江居民大病二段金额
                        jjjmjbbcfwnje = sfjsfh[60];//九江居民疾病补充范围内金额
                        jjjmjbbcfwwje = sfjsfh[61];//九江居民疾病补充范围外金额
                    }

                    string strSql1 = string.Format(@"insert into ybfyjsdr(jzlsh, jylsh, djhin, cyrq, cyyy, bzbm, bzmc, zhsybz, ztjsbz, jbr, kfsbz
                    , cyzdbm1, cyzdmc1, cyzdbm2, cyzdmc2, cyzdbm3, cyzdmc3, cyzdbm4, cyzdmc4, zlff, tes, ylfze, zbxje, tcjjzf, dejjzf, zhzf, xjzf
                    , gwybzjjzf, qybcylbxjjzf, zffy, dwfdfy, yyfdfy, mzjzfy, cxjfy, ylzlfy, blzlfy, fhjbylfy, qfbzfy, zzzyzffy, jrtcfy, tcfdzffy
                    , ctcfdxfy, jrdebxfy, defdzffy, cdefdxfy, rgqgzffy, bcjsqzhye, bntczflj, bndezflj, bnczjmmztczflj, bngwybzzflj, bnzhzflj
                    , bnzycslj, zycs, xm, jsrq, yllb, yldylb, jbjgbm, ywzqh, jslsh, tsxx, djh, jylx, yyjylsh, yxbz, grbhgl, yljgbm, ecbcje
                    , mmqflj, jsfjylsh, grbh, dbzbc, czzcje, elmmxezc, elmmxesy, jmgrzfecbcje, tjje, syjjzf, kh, jjjmdbydje, jjjmdbedje, jjjmjbbcfwnje, jjjmjbbcfwwje
                    ,jjjmzcfwwkbxfy,jmdbydje,jmdbedje,jbbcfwnzfje,jbbcbxbczcfwwfyzfje,bnzfddjjfylj,zfddjjfy) 
                    values('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}'
                    , '{15}', '{16}', '{17}', '{18}', '{19}', '{20}', '{21}', '{22}', '{23}', '{24}', '{25}', '{26}', '{27}', '{28}'
                    , '{29}', '{30}', '{31}', '{32}', '{33}', '{34}', '{35}', '{36}', '{37}', '{38}', '{39}', '{40}', '{41}', '{42}'
                    , '{43}', '{44}', '{45}', '{46}', '{47}', '{48}', '{49}', '{50}', '{51}', '{52}', '{53}', '{54}', '{55}', '{56}', '{57}'
                    , '{58}', '{59}', '{60}', '{61}', '{62}', '{63}', '{64}', '{65}', '{66}', '{67}', '{68}', '{69}', '{70}', '{71}'
                    , '{72}', '{73}', '{74}', '{75}', '{76}', '{77}', '{78}', '{79}', '{80}', '{81}', '{82}', '{83}'
                    , '{84}', '{85}', '{86}', '{87}', '{88}', '{89}', '{90}')"
                    , jzlsh, jylsh, djhin, cyrq, cyyy, bzbm, bzmc, zhsybz, ztjsbz, jbr, Gocent, "", "", "", "", "", "", "", "", "", ""
                    , ylfze, zbxje, tcjjzf, dejjzf, zhzf, xjzf, gwybzjjzf, qybcylbxjjzf, zffy, dwfdfy, yyfdfy, mzjzfy, cxjfy, ylzlfy
                    , blzlfy, fhjbylfy, qfbzfy, zzzyzffy, jrtcfy, tcfdzffy, ctcfdxfy, jrdebxfy, defdzffy, cdefdxfy, rgqgzffy, bcjsqzhye
                    , bntczflj, bndezflj, bnczjmmztczflj, bngwybzzflj, bnzhzflj, bnzycslj, zycs, xm, dqrq, yllb, yldylb, jbjgbm, ywzqh
                    , jslsh, tsxx, djh, jylx, yyjylsh, yxbz, grbhgl, yljgbm, ecbcje, mmqflj, jsfjylsh, grbh, dbzbc, czzcje, elmmxezc
                    , elmmxesy, jmgrzfecbcje, tjje, syjjzf, kh, jjjmdbydje, jjjmdbedje, jjjmjbbcfwnje, jjjmjbbcfwwje
                    , jjjmzcfwwkbxfy, jmdbydje, jmdbedje, jbbcfwnzfje, jbbcbxbczcfwwfyzfje, bnzfddjjfylj, zfddjjfy);
                    string strSql2 = string.Format("update ybcfmxscfhdr set jsdjh = '{0}' where jzlsh = '{1}' and jsdjh is null and cxbz = 1", djhin, jzlsh);
                    string strSql3 = string.Format("update ybcfmxscindr set jsdjh = '{0}' where jzlsh = '{1}' and jsdjh is null and cxbz = 1", djhin, jzlsh);
                    string strSql4 = string.Format("delete from ybcfmxscindr where jzlsh = '{0}' and cxbz in (0, 2)", jzlsh);
                    string strSql5 = string.Format("delete from ybcfmxscfhdr where jzlsh = '{0}' and cxbz in (0, 2)", jzlsh);
                    obj = new object[] { strSql1, strSql2, strSql5, strSql3, strSql4 };
                    obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);

                    if (obj[1].ToString() == "1")
                    {
                        return new object[] { 0, 1, outputData.ToString().Split('^')[2] };
                    }
                    else
                    {
                        Common.WriteYBLog(obj[2].ToString());
                        i = YBFYJSSDCXDR(ybjzlsh, djh, dqrq, grbh, xm, kh, ZXBM);

                        if (i == 1)
                        {
                            return new object[] { 0, 0, "医保提示：his数据库操作失败,自动撤销医保结算成功,错误信息：" + obj[2].ToString() };
                        }
                        else
                        {
                            return new object[] { 0, 0, "医保提示：his数据库操作失败,自动撤销医保结算失败,错误信息：" + obj[2].ToString() };
                        }
                    }
                }
                else if (i == -1)
                {
                    if (ydrybz == "0")
                    {
                        i = YBFYJSSDCXDR(ybjzlsh, djh, dqrq, grbh, xm, kh, ZXBM);

                        if (i == 1)
                        {
                            return new object[] { 0, -1, "医保提示：医保住院结算系统级别错误，自动撤销医保结算成功，" + outputData.ToString() };
                        }
                        else
                        {
                            return new object[] { 0, -1, "医保提示：医保住院结算系统级别错误，自动撤销医保结算失败，" + outputData.ToString() };
                        }
                    }
                    else
                    {
                        return new object[] { 0, -1, "医保提示：医保住院异地结算系统级别错误，但未返回结算单据号，无法撤销，" + outputData.ToString() };
                    }
                }
                else
                {
                    return new object[] { 0, -2, "医保提示：医保住院结算业务级别或未知错误，" + outputData.ToString() };
                }
            }
            catch (Exception error)
            {
                Common.WriteYBLog(error.ToString());

                if (isybok)
                {
                    int i = YBFYJSSDCXDR(ybjzlsh, djh, dqrq, grbh, xm, kh, ZXBM);

                    if (i == 1)
                    {
                        return new object[] { 0, 2, "医保提示：非医保异常，医保结算自动撤销成功，" + error.ToString() };
                    }
                    else
                    {
                        return new object[] { 0, 2, "医保提示：非医保异常，医保结算自动撤销失败，" + error.ToString() };
                    }
                }

                return new object[] { 0, 2, "Error:" + error.ToString() };
            }
        }
        #endregion 住院结算

        #region 费用结算撤销
        /// <summary>
        /// 费用结算撤销
        /// </summary>
        /// <param>his就诊流水号,单据号,读卡标志(不传表示读卡，传了则不读卡)</param>
        /// <returns>1:成功，0:不成功，2:报错</returns>
        public static object[] YBFYJSCXDR(object[] objParam)
        {
            try
            {
                string czygh = CliUtils.fLoginUser;
                string ywzqh = YWZQH;
                string jbr = CliUtils.fUserName;
                string jzlsh = objParam[0].ToString();
                string djhin = objParam[1].ToString();

                if (string.IsNullOrWhiteSpace(jzlsh))
                {
                    return new object[] { 0, 0, "医保提示：his就诊流水号为空" };
                }

                if (string.IsNullOrWhiteSpace(djhin))
                {
                    return new object[] { 0, 0, "医保提示：单据号为空" };
                }

                DateTime dqsj = Convert.ToDateTime(Common.GetServerTime());
                string jylsh = dqsj.ToString("yyyyMMddHHmmss") + "-" + DDYLJGBH + "-" + jzlsh;
                string strSql = string.Format(@"select a.xm, a.kh, a.jsrq, a.djh, a.ybjzlsh from ybfyjsdr a where a.jzlsh = '{0}' and a.djhin = '{1}' and a.cxbz = 1", jzlsh, djhin);//0秒
                DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                string strtj = "";

                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    DataRow dr = ds.Tables[0].Rows[0];
                    string ybjzlsh = dr["ybjzlsh"].ToString();

                    if (!string.IsNullOrWhiteSpace(ybjzlsh))
                    {
                        strtj = string.Format(" and b.ybjzlsh = '{0}'", ybjzlsh);
                    }
                }

                strSql = string.Format(@"select a.xm, a.kh, a.jsrq, a.djh, b.ybjzlsh, b.jzbz, b.tcqh, b.ydrybz, b.grbh grbhs from ybfyjsdr a join ybmzzydjdr b on a.jzlsh = b.jzlsh where a.jzlsh = '{0}' and a.djhin = '{1}' and a.cxbz = 1 and b.cxbz = 1", jzlsh, djhin);//0秒
                strSql = strSql + strtj;
                ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);

                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    DataRow dr = ds.Tables[0].Rows[0];
                    string ybjzlsh = dr["ybjzlsh"].ToString();
                    string xm = dr["xm"].ToString();
                    string kh = dr["kh"].ToString();
                    string grbh = dr["grbhs"].ToString();
                    string jsrq = dr["jsrq"].ToString();
                    string djh = dr["djh"].ToString();
                    string ydrybz = dr["ydrybz"].ToString();
                    ZXBM = ydrybz == "0" ? "0000" : dr["tcqh"].ToString();
                    object[] obj = null;
                    string jzbz = dr["jzbz"].ToString();

                    //if (jzbz == "z")
                    //{
                    //    //string strSqlz = string.Format(@"select z3endv from zy03dw a where a.z3comp = '{0}' and a.z3zyno = '{1}' and a.z3jshx = '{2}'", CliUtils.fSiteCode, jzlsh, djhin);//0秒
                    //    //DataSet dsz = CliUtils.ExecuteSql("sybdj", "cmd", strSqlz, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);

                    //    //if (dsz != null && dsz.Tables[0].Rows.Count > 0)
                    //    //{
                    //    //    string endv = dsz.Tables[0].Rows[0]["z3endv"].ToString();

                    //    //    if (endv.Substring(0, 1) != "2" && endv.Substring(0, 1) != "3")
                    //    //    {
                    //    //        return new object[] { 0, 0, "医保提示：his住院号" + jzlsh + "没撤销结算，请先撤销his结算" };
                    //    //    }
                    //    //}
                    //}

                    if (objParam.Length == 2)
                    {
                        obj = YBDKDR(new object[]{});

                        if (obj[1].ToString() == "1")
                        {
                            if (xm != obj[2].ToString().Split('|')[3])
                            {
                                return new object[] { 0, 0, "医保提示：不是本人卡" };
                            }
                        }
                        else
                        {
                            return new object[] { 0, 0, "医保提示：读卡错误：" + obj[2].ToString() };
                        }
                    }

                    string rc = ybjzlsh + "|" + djh + "|" + jsrq + "|" + jbr + "||" + Gocent + "|" + grbh + "|" + xm + "|" + kh + "|"; //不保存处方明细
                    StringBuilder inputData = new StringBuilder(string.Format("{0}^{1}^{2}^{3}^{4}^{5}^{6}^{7}^", "2430", DDYLJGBH, czygh, ywzqh, jylsh, ZXBM, rc, "1"));
                    StringBuilder outputData = new StringBuilder(100000);
                    int i = BUSINESS_HANDLE(inputData, outputData);
                     
                    if (i == 0)
                    {
                        List<string> lijsmxcx = new List<string>();
                        string strSql1 = string.Format(@"insert into ybfyjsdr(jzlsh, jylsh, ylfze, zbxje, tcjjzf, dejjzf, zhzf, xjzf, gwybzjjzf
                        , qybcylbxjjzf, zffy, dwfdfy, yyfdfy, mzjzfy, cxjfy, ylzlfy, blzlfy, fhjbylfy, qfbzfy, zzzyzffy, jrtcfy, tcfdzffy, ctcfdxfy
                        , jrdebxfy, defdzffy, cdefdxfy, rgqgzffy, bcjsqzhye, bntczflj, bndezflj, bnczjmmztczflj, bngwybzzflj, bnzhzflj, bnzycslj
                        , zycs, xm, jsrq, yllb, yldylb, jbjgbm, ywzqh, jslsh, tsxx, djh, jylx, yyjylsh, yxbz, grbhgl, yljgbm, ecbcje, mmqflj
                        , jsfjylsh, grbh, dbzbc, czzcje, elmmxezc, elmmxesy, jmgrzfecbcje, tjje, syjjzf, kh, cxbz, djhin, cyrq, cyyy
                        , bzbm, bzmc, zhsybz, ztjsbz, jbr, kfsbz, cyzdbm1, cyzdmc1, cyzdbm2, cyzdmc2, cyzdbm3, cyzdmc3, cyzdbm4, cyzdmc4, zlff, tes
                        , jjjmdbydje, jjjmdbedje, jjjmjbbcfwnje, jjjmjbbcfwwje
                        ,jjjmzcfwwkbxfy,jmdbydje,jmdbedje,jbbcfwnzfje,jbbcbxbczcfwwfyzfje,bnzfddjjfylj,zfddjjfy) 
                        select jzlsh, jylsh, ylfze, zbxje, tcjjzf, dejjzf, zhzf, xjzf, gwybzjjzf, qybcylbxjjzf, zffy, dwfdfy, yyfdfy, mzjzfy
                        , cxjfy, ylzlfy, blzlfy, fhjbylfy, qfbzfy, zzzyzffy, jrtcfy, tcfdzffy, ctcfdxfy, jrdebxfy, defdzffy, cdefdxfy, rgqgzffy
                        , bcjsqzhye, bntczflj, bndezflj, bnczjmmztczflj, bngwybzzflj, bnzhzflj, bnzycslj, zycs, xm, jsrq, yllb, yldylb, jbjgbm
                        , ywzqh, jslsh, tsxx, djh, '-1', yyjylsh, yxbz, grbhgl, yljgbm, ecbcje, mmqflj, jsfjylsh, grbh, dbzbc, czzcje, elmmxezc
                        , elmmxesy, jmgrzfecbcje, tjje, syjjzf, kh, 0, djhin, cyrq, cyyy, bzbm, bzmc, zhsybz, ztjsbz, '{2}', kfsbz, cyzdbm1
                        , cyzdmc1, cyzdbm2, cyzdmc2, cyzdbm3, cyzdmc3, cyzdbm4, cyzdmc4, zlff, tes, jjjmdbydje, jjjmdbedje, jjjmjbbcfwnje, jjjmjbbcfwwje
                        ,jjjmzcfwwkbxfy,jmdbydje,jmdbedje,jbbcfwnzfje,jbbcbxbczcfwwfyzfje,bnzfddjjfylj,zfddjjfy
                        from ybfyjsdr where jzlsh = '{0}' and djhin = '{1}' and cxbz = 1", jzlsh, djhin, jbr);//0秒
                        string strSql11 = string.Format("update ybfyjsdr set cxbz = 2 where jzlsh = '{0}' and djhin = '{1}' and cxbz = 1", jzlsh, djhin);//0秒
                        lijsmxcx.Add(strSql1);
                        lijsmxcx.Add(string.Format("delete from ybfyyjsdr where jzlsh = '{0}'", jzlsh));//0秒
                        lijsmxcx.Add(strSql11);
                        string strSql2 = string.Format(@"insert into ybcfmxscfhdr(jzlsh, jylsh, je, zlje, zfje, cxjzfje, sflb, sfxmdj, qezfbz, zlbl, xj, bz, grbh, xm, kh, cfh, ybcfh, yyxmdm, yyxmmc, jsdjh, cxbz) 
                        select jzlsh, jylsh, je, zlje, zfje, cxjzfje, sflb, sfxmdj, qezfbz, zlbl, xj, bz, grbh, xm, kh, cfh, ybcfh, yyxmdm, yyxmmc, jsdjh, 0 from ybcfmxscfhdr where jzlsh = '{0}' and cxbz = 1 and jsdjh = '{1}'", jzlsh, djhin);//0秒
                        lijsmxcx.Add(strSql2);
                        strSql2 = string.Format("update ybcfmxscfhdr set cxbz = 2 where jzlsh = '{0}' and cxbz = 1 and jsdjh = '{1}'", jzlsh, djhin);//0秒
                        lijsmxcx.Add(strSql2);
                        strSql2 = string.Format(@"insert into ybcfmxscindr(jzlsh, jylsh, sfxmzl, sflb, ybcfh, cfrq, yysfxmbm, sfxmzxbm
                        , yysfxmmc, dj, sl, je, jx, gg, mcyl, sypc, ysbm, ysxm, yf, dw, ksbh, ksmc, zxts, cydffbz, jbr, ypjldw, qezfbz, grbh, xm, kh, jsdjh, cxbz) 
                        select jzlsh, jylsh, sfxmzl, sflb, ybcfh, cfrq, yysfxmbm, sfxmzxbm, yysfxmmc, dj, sl, je, jx, gg, mcyl, sypc, ysbm
                        , ysxm, yf, dw, ksbh, ksmc, zxts, cydffbz, '{2}', ypjldw, qezfbz, grbh, xm, kh, jsdjh, 0 
                        from ybcfmxscindr where jzlsh = '{0}' and cxbz = 1 and jsdjh = '{1}'", jzlsh, djhin, jbr);//0秒
                        lijsmxcx.Add(strSql2);
                        strSql2 = string.Format("update ybcfmxscindr set cxbz = 2 where jzlsh = '{0}' and cxbz = 1 and jsdjh = '{1}'", jzlsh, djhin);//0秒
                        lijsmxcx.Add(strSql2);

                        if (jzbz == "z")
                        {
                            strSql2 = string.Format("update zy03d set z3ybup = null where z3comp = '{0}' and z3zyno = '{1}' and (z3jshx = '{2}' or z3jshx is null)", CliUtils.fSiteCode, jzlsh, djhin);//0秒
                            lijsmxcx.Add(strSql2);
                        }

                        obj = lijsmxcx.ToArray();
                        obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);

                        if (obj[1].ToString() == "1")
                        {
                            return new object[] { 0, 1, outputData };
                        }
                        else
                        {
                            Common.WriteYBLog(obj[2].ToString());
                            return new object[] { 0, 0, "医保提示：his数据库操作失败：" + obj[2].ToString() };
                        }
                    }
                    else if (i == -1)
                    {
                        List<string> lijsmxcx = new List<string>();
                        string strSql1 = string.Format(@"insert into ybfyjsdr(jzlsh, jylsh, ylfze, zbxje, tcjjzf, dejjzf, zhzf, xjzf, gwybzjjzf
                        , qybcylbxjjzf, zffy, dwfdfy, yyfdfy, mzjzfy, cxjfy, ylzlfy, blzlfy, fhjbylfy, qfbzfy, zzzyzffy, jrtcfy, tcfdzffy, ctcfdxfy
                        , jrdebxfy, defdzffy, cdefdxfy, rgqgzffy, bcjsqzhye, bntczflj, bndezflj, bnczjmmztczflj, bngwybzzflj, bnzhzflj, bnzycslj
                        , zycs, xm, jsrq, yllb, yldylb, jbjgbm, ywzqh, jslsh, tsxx, djh, jylx, yyjylsh, yxbz, grbhgl, yljgbm, ecbcje, mmqflj
                        , jsfjylsh, grbh, dbzbc, czzcje, elmmxezc, elmmxesy, jmgrzfecbcje, tjje, syjjzf, kh, cxbz, djhin, cyrq, cyyy
                        , bzbm, bzmc, zhsybz, ztjsbz, jbr, kfsbz, cyzdbm1, cyzdmc1, cyzdbm2, cyzdmc2, cyzdbm3, cyzdmc3, cyzdbm4, cyzdmc4, zlff, tes
                        , jjjmdbydje, jjjmdbedje, jjjmjbbcfwnje, jjjmjbbcfwwje
                        , jjjmzcfwwkbxfy,jmdbydje,jmdbedje,jbbcfwnzfje,jbbcbxbczcfwwfyzfje,bnzfddjjfylj,zfddjjfy) 
                        select jzlsh, jylsh, ylfze, zbxje, tcjjzf, dejjzf, zhzf, xjzf, gwybzjjzf, qybcylbxjjzf, zffy, dwfdfy, yyfdfy, mzjzfy
                        , cxjfy, ylzlfy, blzlfy, fhjbylfy, qfbzfy, zzzyzffy, jrtcfy, tcfdzffy, ctcfdxfy, jrdebxfy, defdzffy, cdefdxfy, rgqgzffy
                        , bcjsqzhye, bntczflj, bndezflj, bnczjmmztczflj, bngwybzzflj, bnzhzflj, bnzycslj, zycs, xm, jsrq, yllb, yldylb, jbjgbm
                        , ywzqh, jslsh, tsxx, djh, '-1', yyjylsh, yxbz, grbhgl, yljgbm, ecbcje, mmqflj, jsfjylsh, grbh, dbzbc, czzcje, elmmxezc
                        , elmmxesy, jmgrzfecbcje, tjje, syjjzf, kh, 0, djhin, cyrq, cyyy, bzbm, bzmc, zhsybz, ztjsbz, '{2}', kfsbz, cyzdbm1
                        , cyzdmc1, cyzdbm2, cyzdmc2, cyzdbm3, cyzdmc3, cyzdbm4, cyzdmc4, zlff, tes, jjjmdbydje, jjjmdbedje, jjjmjbbcfwnje, jjjmjbbcfwwje
                        , jjjmzcfwwkbxfy,jmdbydje,jmdbedje,jbbcfwnzfje,jbbcbxbczcfwwfyzfje,bnzfddjjfylj,zfddjjfy
                        from ybfyjsdr where jzlsh = '{0}' and djhin = '{1}' and cxbz = 1", jzlsh, djhin, jbr);//0秒
                        string strSql11 = string.Format("update ybfyjsdr set cxbz = 2 where jzlsh = '{0}' and djhin = '{1}' and cxbz = 1", jzlsh, djhin);//1秒
                        lijsmxcx.Add(strSql1);
                        lijsmxcx.Add(string.Format("delete from ybfyyjsdr where jzlsh = '{0}'", jzlsh));
                        lijsmxcx.Add(strSql11);
                        string strSql2 = string.Format(@"insert into ybcfmxscfhdr(jzlsh, jylsh, je, zlje, zfje, cxjzfje, sflb, sfxmdj, qezfbz, zlbl, xj, bz, grbh, xm, kh, cfh, ybcfh, yyxmdm, yyxmmc, jsdjh, cxbz) 
                        select jzlsh, jylsh, je, zlje, zfje, cxjzfje, sflb, sfxmdj, qezfbz, zlbl, xj, bz, grbh, xm, kh, cfh, ybcfh, yyxmdm, yyxmmc, jsdjh, 0 from ybcfmxscfhdr where jzlsh = '{0}' and cxbz = 1 and jsdjh = '{1}'", jzlsh, djhin);//0秒
                        lijsmxcx.Add(strSql2);
                        strSql2 = string.Format("update ybcfmxscfhdr set cxbz = 2 where jzlsh = '{0}' and cxbz = 1 and jsdjh = '{1}'", jzlsh, djhin);//0秒
                        lijsmxcx.Add(strSql2);
                        strSql2 = string.Format(@"insert into ybcfmxscindr(jzlsh, jylsh, sfxmzl, sflb, ybcfh, cfrq, yysfxmbm, sfxmzxbm
                        , yysfxmmc, dj, sl, je, jx, gg, mcyl, sypc, ysbm, ysxm, yf, dw, ksbh, ksmc, zxts, cydffbz, jbr, ypjldw, qezfbz, grbh, xm, kh, jsdjh, cxbz) 
                        select jzlsh, jylsh, sfxmzl, sflb, ybcfh, cfrq, yysfxmbm, sfxmzxbm, yysfxmmc, dj, sl, je, jx, gg, mcyl, sypc, ysbm
                        , ysxm, yf, dw, ksbh, ksmc, zxts, cydffbz, '{2}', ypjldw, qezfbz, grbh, xm, kh, jsdjh, 0 
                        from ybcfmxscindr where jzlsh = '{0}' and cxbz = 1 and jsdjh = '{1}'", jzlsh, djhin, jbr);//0秒
                        lijsmxcx.Add(strSql2);
                        strSql2 = string.Format("update ybcfmxscindr set cxbz = 2 where jzlsh = '{0}' and cxbz = 1 and jsdjh = '{1}'", jzlsh, djhin);//0秒
                        lijsmxcx.Add(strSql2);

                        if (jzbz == "z")
                        {
                            strSql2 = string.Format("update zy03d set z3ybup = null where z3comp = '{0}' and z3zyno = '{1}' and (z3jshx = '{2}' or z3jshx is null)", CliUtils.fSiteCode, jzlsh, djhin);//0秒
                            lijsmxcx.Add(strSql2);
                        }

                        obj = lijsmxcx.ToArray();
                        obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);

                        if (obj[1].ToString() == "1")
                        {
                            return new object[] { 0, -1, "医保提示：医保结算撤销系统级别错误，自动删除医保结算和处方明细成功，" + outputData.ToString() };
                        }
                        else
                        {
                            return new object[] { 0, -1, "医保提示：医保结算撤销系统级别错误，自动删除医保结算和处方明细失败，" + outputData.ToString() };
                        }
                    }
                    else
                    {
                        return new object[] { 0, -2, "医保提示：医保结算撤销业务级别或未知错误，" + outputData.ToString() };
                    }
                }
                else
                {
                    return new object[] { 0, 0, "医保提示：医保无结算信息" };
                }
            }
            catch (Exception error)
            {
                Common.WriteYBLog(error.ToString());
                return new object[] { 0, 2, "Error:" + error.ToString() };
            }
        }
        #endregion 费用结算撤销

        #region 挂号/住院登记手动撤销（无数据库）东软
        /// <summary>
        /// 挂号/住院登记撤销东软
        /// </summary>
        /// <param>医保就诊流水号,个人编号,姓名,卡号,中心编码</param>
        /// <returns>1:成功，0:不成功，2:报错</returns>
        public static int YBDJSDCXDR(string ybjzlsh, string grbh, string xm, string kh, string zxbm)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(ybjzlsh))
                {
                    return 0;
                }
                else if (string.IsNullOrWhiteSpace(grbh))
                {
                    return 0;
                }
                else if (string.IsNullOrWhiteSpace(xm))
                {
                    return 0;
                }
                else if (string.IsNullOrWhiteSpace(kh))
                {
                    return 0;
                }

                ZXBM = zxbm;
                DateTime dqsj = Convert.ToDateTime(Common.GetServerTime());
                string jylsh = dqsj.ToString("yyyyMMddHHmmss") + "-" + DDYLJGBH + "-" + ybjzlsh;
                string rc = ybjzlsh + "|" + CliUtils.fUserName + "|" + grbh + "|" + xm + "|" + kh + "|";
                StringBuilder inputData = new StringBuilder(string.Format("{0}^{1}^{2}^{3}^{4}^{5}^{6}^{7}^", "2240", DDYLJGBH, CliUtils.fLoginUser, YWZQH, jylsh, ZXBM, rc, "1"));
                StringBuilder outputData = new StringBuilder(100000);
                int i = BUSINESS_HANDLE(inputData, outputData);

                if (i == 0)
                {
                    return 1;
                }
                else
                {
                    Common.InsertYBLog(ybjzlsh, inputData.ToString(), outputData.ToString());
                    return 0;
                }
            }
            catch (Exception error)
            {
                Common.InsertYBLog(ybjzlsh, "", error.ToString());
                return 2;
            }
        }
        #endregion 挂号/住院登记手动撤销（无数据库）东软

        #region 处方明细手动撤销（无数据库）东软
        /// <summary>
        /// 处方明细撤销（无数据库）东软
        /// </summary>
        /// <param>医保就诊流水号,个人编号,姓名，卡号,中心编码</param>
        /// <returns>1:成功，0:不成功，2:报错</returns>
        public static object[] YBCFMXSDCXDR(object[] objParam)
        {
            string ybjzlsh = objParam[0].ToString();
            string grbh = objParam[1].ToString();
            string xm = objParam[2].ToString();
            string kh = objParam[3].ToString();
            string zxbm = objParam[4].ToString();
            try
            {
                if (string.IsNullOrWhiteSpace(ybjzlsh))
                {
                    return (new object[]{0});
                }
                else if (string.IsNullOrWhiteSpace(grbh))
                {
                    return (new object[] { 0 });
                }
                else if (string.IsNullOrWhiteSpace(xm))
                {
                    return (new object[] { 0 });
                }
                else if (string.IsNullOrWhiteSpace(kh))
                {
                    return (new object[] { 0 });
                }

                ZXBM = zxbm;
                DateTime dqsj = Convert.ToDateTime(Common.GetServerTime());
                string jylsh = dqsj.ToString("yyyyMMddHHmmss") + "-" + DDYLJGBH + "-" + ybjzlsh;
                string rc = ybjzlsh + "||" + CliUtils.fUserName + "|" + grbh + "|" + xm + "|" + kh + "|";
                StringBuilder inputData = new StringBuilder(string.Format("{0}^{1}^{2}^{3}^{4}^{5}^{6}^{7}^", "2320", DDYLJGBH, CliUtils.fLoginUser, YWZQH, jylsh, ZXBM, rc, "1"));
                StringBuilder outputData = new StringBuilder(100000);
                int i = BUSINESS_HANDLE(inputData, outputData);

                if (i == 0)
                {
                    return (new object[] { 1 });
                }
                else
                {
                    Common.InsertYBLog(ybjzlsh, inputData.ToString(), outputData.ToString());
                    return (new object[] { 0 });
                }
            }
            catch (Exception error)
            {
                Common.InsertYBLog(ybjzlsh, "", error.ToString());
                return (new object[] { 2 });
            }
        }
        #endregion 处方明细手动撤销（无数据库）东软

        #region 费用结算手动撤销（无数据库）东软
        /// <summary>
        /// 费用结算撤销（无数据库）东软
        /// </summary>
        /// <param>医保就诊流水号,单据号,结算时间,个人编号,姓名,卡号,中心编码,是否保存处方</param>
        /// <returns>1:成功，0:不成功，2:报错</returns>
        public static int YBFYJSSDCXDR(string ybjzlsh, string djh, string jsrq, string grbh, string xm, string kh, string zxbm, params int[] bccf)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(ybjzlsh))
                {
                    return 0;
                }
                else if (string.IsNullOrWhiteSpace(djh))
                {
                    return 0;
                }
                else if (string.IsNullOrWhiteSpace(jsrq))
                {
                    return 0;
                }
                else if (string.IsNullOrWhiteSpace(grbh))
                {
                    return 0;
                }
                else if (string.IsNullOrWhiteSpace(xm))
                {
                    return 0;
                }
                else if (string.IsNullOrWhiteSpace(kh))
                {
                    return 0;
                }

                ZXBM = zxbm;
                DateTime dqsj = Convert.ToDateTime(Common.GetServerTime());
                string jylsh = dqsj.ToString("yyyyMMddHHmmss") + "-" + DDYLJGBH + "-" + djh;
                int bccfbz = 1;

                if (bccf.Length > 0)
                {
                    bccfbz = bccf[0];
                }

                string rc = ybjzlsh + "|" + djh + "|" + jsrq + "|" + CliUtils.fUserName + "||" + Gocent + "|" + grbh + "|" + xm + "|" + kh + "|";
                StringBuilder inputData = new StringBuilder(string.Format("{0}^{1}^{2}^{3}^{4}^{5}^{6}^{7}^", "2430", DDYLJGBH, CliUtils.fLoginUser, YWZQH, jylsh, ZXBM, rc, "1"));
                StringBuilder outputData = new StringBuilder(100000);
                int i = BUSINESS_HANDLE(inputData, outputData);
                WriteLog("出参" +outputData.ToString());

                if (i == 0)
                {
                    return 1;
                }
                else
                {
                    Common.InsertYBLog(ybjzlsh, inputData.ToString(), outputData.ToString());
                    return 0;
                }
            }
            catch (Exception error)
            {
                Common.InsertYBLog(ybjzlsh, "", error.ToString());
                return 2;
            }
        }
        #endregion 费用结算手动撤销（无数据库）东软

        #region 明细对账东软
        /// <summary>
        /// 明细对账东软
        /// </summary>
        /// <param>就诊流水号</param>
        /// <returns>费用总额,超限价总额,自理费用总额,自费费用总额,数据条数</returns>
        public static object[] YBMXDZDR(object[] objParam)
        {
            ZXBM = "0000";

            try
            {
                string jzlsh = objParam[0].ToString();

                if (string.IsNullOrWhiteSpace(jzlsh))
                {
                    return new object[] { 0, 0, "医保提示：his就诊流水号为空" };
                }

                DateTime dqsj = Convert.ToDateTime(Common.GetServerTime());
                string jylsh = dqsj.ToString("yyyyMMddHHmmss") + "-" + DDYLJGBH + "-" + jzlsh;
                string rc = jzlsh + "|";
                StringBuilder inputData = new StringBuilder(string.Format("{0}^{1}^{2}^{3}^{4}^{5}^{6}^{7}^", "1130", DDYLJGBH, CliUtils.fLoginUser, YWZQH, jylsh, ZXBM, rc, "1"));
                StringBuilder outputData = new StringBuilder(100000);
                int i = BUSINESS_HANDLE(inputData, outputData);

                if (i == 0)
                {
                    return new object[] { 0, 1, outputData };
                }
                else
                {
                    Common.InsertYBLog(jzlsh, inputData.ToString(), outputData.ToString());
                    return new object[] { 0, i, outputData };
                }
            }
            catch (Exception error)
            {
                Common.InsertYBLog("", "", error.ToString());
                return new object[] { 0, 2, error.ToString() };
            }
        }
        #endregion 明细对账东软

        #region 打印结算清单
        /// <summary>
        /// 打印结算清单
        /// </summary>
        /// <param>his就诊流水号，单据号，出院日期(格式：yyyy-MM-dd HH:mm:ss)，重打标志</param>
        public static object[] PrintYBQDDR(object[] objParam)
        {
            try
            {
                string hisjzlsh = objParam[0].ToString();
                string djh = objParam[1].ToString();
                string ldat = objParam[2].ToString();
                string cd = objParam[3].ToString();
                string sql = string.Format(@"select jzbz from ybmzzydjdr(nolock) a where a.jzlsh = '{0}' and cxbz = 1", hisjzlsh);//0秒
                DataSet dsP = CliUtils.ExecuteSql("sybdj", "cmd", sql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);

                if (dsP.Tables[0].Rows.Count > 0)
                {
                    DataRow dr = dsP.Tables[0].Rows[0];
                    string jzbz = dr["jzbz"].ToString();
                    string cdbz = "";

                    if (cd.Length > 0)
                    {
                        cdbz = cd;
                    }

                    if (jzbz == "m")
                    {
                        return PrintYBQDMZDR(djh, cdbz);
                    }
                    else
                    {
                        return PrintYBQDZYDR(hisjzlsh, djh, ldat, cdbz);
                    }
                }
                else
                {
                    MessageBox.Show("医保提示：医保无该病人就诊信息", "提示");
                    return new object[] { 0, 0, "医保提示：医保无该病人就诊信息", "提示" };
                }
            }
            catch (Exception err)
            {
                MessageBox.Show("打印医保清单失败：" + err.ToString(), "提示");
                return new object[] { 0, 2, "打印医保清单失败：" + err.ToString(), "提示" };
            }
        }
        #endregion 打印结算清单

        #region 打印结算清单全部
        /// <summary>
        /// 打印结算清单全部
        /// </summary>
        /// <param>his就诊流水号，单据号，出院日期(格式：yyyy-MM-dd HH:mm:ss)，重打标志</param>
        public static object[] PrintYBQDQBDR(object[] objParam)
        {
            try
            {
                string hisjzlsh = objParam[0].ToString();
                string djh = objParam[1].ToString();
                string ldat = objParam[2].ToString();
                string cd = objParam[3].ToString();
                string sql = string.Format(@"select jzbz from ybmzzydjdr(nolock) a where a.jzlsh = '{0}' and cxbz = 1", hisjzlsh);//0秒
                DataSet dsP = CliUtils.ExecuteSql("sybdj", "cmd", sql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);

                if (dsP.Tables[0].Rows.Count > 0)
                {
                    DataRow dr = dsP.Tables[0].Rows[0];
                    string jzbz = dr["jzbz"].ToString();
                    string cdbz = "";

                    if (cd.Length > 0)
                    {
                        cdbz = cd;
                    }

                    if (jzbz == "m")
                    {
                        return PrintYBQDMZQBDR(new object[]{djh, cdbz});
                    }
                    else
                    {
                        return PrintYBQDZYQBDR(new object[]{hisjzlsh, djh, ldat, cdbz});
                    }
                }
                else
                {
                    MessageBox.Show("医保提示：医保无该病人就诊信息", "提示");
                    return new object[] { 0, 0, "医保提示：医保无该病人就诊信息", "提示" };
                }
            }
            catch (Exception err)
            {
                MessageBox.Show("打印医保清单失败：" + err.ToString(), "提示");
                return new object[] { 0, 2, "打印医保清单失败：" + err.ToString(), "提示" };
            }
        }
        #endregion 打印结算清单全部

        #region 打印住院结算清单
        /// <summary>
        /// 打印住院结算清单
        /// </summary>
        /// <param>his住院号，单据号，出院日期(格式：yyyy-MM-dd HH:mm:ss),重打标志</param>
        public static object[] PrintYBQDZYDR(string zybah, string djhin, string ldat, string cd)
        {
            try
            {
                string sql = string.Format(@"select distinct a.z1date, b.z1outd from zy01h(nolock) a join zy01d(nolock) b on a.z1zyno = b.z1zyno where a.z1comp = '{0}' and b.z1comp = '{0}' and a.z1zyno = '{1}' and left(b.z1endv, 1) = '8'", CliUtils.fSiteCode, zybah);//0秒
                DataSet dsP = CliUtils.ExecuteSql("sybdj", "cmd", sql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);

                if (dsP.Tables[0].Rows.Count > 0)
                {
                    DataRow dr = dsP.Tables[0].Rows[0];
                    DateTime d1 = Convert.ToDateTime(dr["z1date"]);
                    DateTime d2 = dr["z1outd"] == DBNull.Value ? Convert.ToDateTime(ldat) : Convert.ToDateTime(dr["z1outd"]);
                    string ryrq = d1.ToString("yyyy-MM-dd");
                    string cyrq = d2.ToString("yyyy-MM-dd");
                    TimeSpan ts = d2.Date - d1.Date;
                    int zyts = ts.Days;
                    string czy = CliUtils.fUserName;
                    string sql1 = string.Format(@"select a.grbh, a.djh, a.djhin, a.jzlsh, a.sysdate jsrq, b.xm, case b.xb when '1' then '男' else '女' end xb
, a.kh, c.bzname yllb, d.bzname yldylb, e.bzname tcq, b.bzmc jbzd, a.yljgbm yljgbh, a.ylfze, a.jrtcfy, a.qfbzfy qfx, a.ylzlfy, a.blzlfy
, a.cxjfy, a.zffy, isnull(a.jjjmzcfwwkbxfy, 0) ypylqc, a.bcjsqzhye - a.zhzf zhye, a.bntczflj, a.tcjjzf, a.tcjjzf + a.zhzf tcjjzfzhzfhj
, a.bndezflj, a.jmdbydje, a.jmdbedje, a.ecbcje, a.dejjzf - a.ecbcje - a.jmdbydje - a.jmdbedje dbqfx, a.dejjzf bcdbbxbxhj
, a.jjjmjbbcfwnje jbbcfwnzfje, a.jjjmjbbcfwwje jbbcbxbczcfwnfyedzfje, a.qybcylbxjjzf bcjbbcbxbxhj, 0 bcjzjemt, a.mzjzfy bcjzjezy
, a.mzjzfy bcjzhj, isnull(a.bnzfddjjfylj, 0) bnzfddjjfylj, isnull(a.zfddjjfy, 0) zfddjjfy, a.zhzf + a.xjzf grfdfy, a.zhzf, a.xjzf, a.yyfdfy ,b.dwmc
                    from ybfyjsdr(nolock) a
                    join ybmzzydjdr(nolock) b on a.jzlsh = b.jzlsh
                    left join bztbd(nolock) c on c.bzcodn = 'YL' and c.bzmem1 = a.yllb
                    left join bztbd(nolock) d on d.bzcodn = 'DL' and d.bzmem1 = a.yldylb
                    left join bztbd(nolock) e on b.tcqh = e.bzkeyx and e.bzcodn = 'DQ' and e.bzusex = 1
                    where a.jzlsh = '{0}' and a.djhin = '{1}' and a.cxbz = 1 and b.cxbz = 1 and b.jzbz = 'z'", zybah, djhin);//1秒
                    DataSet ds1 = CliUtils.ExecuteSql("sybdj", "cmd", sql1, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);

                    if (ds1 == null || ds1.Tables[0].Rows.Count == 0)
                    {
                        return new object[] { 0, 0, "医保提示：无结算数据", "提示" };
                    }

                    DataRow dr1 = ds1.Tables[0].Rows[0];
                    string jzlsh = dr1["jzlsh"].ToString();
                    string jsrq = dr1["jsrq"].ToString();
                    string xm = dr1["xm"].ToString();
                    string xb = dr1["xb"].ToString();
                    string kh = dr1["kh"].ToString();
                    string grbh = dr1["grbh"].ToString();
                    string yllb = dr1["yllb"].ToString();
                    string yldylb = dr1["yldylb"].ToString();
                    string tcq = dr1["tcq"].ToString();
                    string jbzd = dr1["jbzd"].ToString();
                    string yljgbh = dr1["yljgbh"].ToString();
                    string ylfze = dr1["ylfze"].ToString();
                    string jrtcfy = dr1["jrtcfy"].ToString();
                    string qfx = dr1["qfx"].ToString();
                    string ylzlfy = dr1["ylzlfy"].ToString();
                    string blzlfy = dr1["blzlfy"].ToString();
                    string cxjfy = dr1["cxjfy"].ToString();
                    string zffy = dr1["zffy"].ToString();
                    string ypylqc = dr1["ypylqc"].ToString();
                    string zhye = dr1["zhye"].ToString();
                    string bntczflj = dr1["bntczflj"].ToString();
                    string tcjjzf = dr1["tcjjzf"].ToString();
                    string tcjjzfzhzfhj = dr1["tcjjzfzhzfhj"].ToString();
                    string bndezflj = dr1["bndezflj"].ToString();
                    string jjjmdbydje = dr1["jmdbydje"].ToString();
                    string jjjmdbedje = dr1["jmdbedje"].ToString();
                    string ecbcje = dr1["ecbcje"].ToString();
                    string dbqfx = dr1["dbqfx"].ToString();
                    string bcdbbxbxhj = dr1["bcdbbxbxhj"].ToString();
                    string bczcfwnfybxje = dr1["jbbcfwnzfje"].ToString();
                    string bczfyphylqcfybxje = dr1["jbbcbxbczcfwnfyedzfje"].ToString();
                    string bcjbbcbxbxhj = dr1["bcjbbcbxbxhj"].ToString();
                    string bcjzjemt = dr1["bcjzjemt"].ToString();
                    string bcjzjezy = dr1["bcjzjezy"].ToString();
                    string bcjzhj = dr1["bcjzhj"].ToString();
                    string bnzfddjjfylj = dr1["bnzfddjjfylj"].ToString();
                    string zfddjjfy = dr1["zfddjjfy"].ToString();
                    string grfdfy = dr1["grfdfy"].ToString();
                    string zhzf = dr1["zhzf"].ToString();
                    string xjzf = dr1["xjzf"].ToString();
                    string yyfdfy = dr1["yyfdfy"].ToString();
                    string dxtcjjzfzhzfhj = Common.MoneyToUpper(dr1["tcjjzfzhzfhj"].ToString());
                    string dxbcdbbxbxhj = Common.MoneyToUpper(dr1["bcdbbxbxhj"].ToString());
                    string dxbcjbbcbxbxhj = Common.MoneyToUpper(dr1["bcjbbcbxbxhj"].ToString());
                    string dxbcjzhj = Common.MoneyToUpper(dr1["bcjzhj"].ToString());
                    string dxzfddjjfy = Common.MoneyToUpper(dr1["zfddjjfy"].ToString());
                    string cdbz = "";
                    string dwmc = dr1["dwmc"].ToString();

                    if (cd.Length > 0)
                    {
                        cdbz = cd[0].ToString();
                    }

                    string spath = Para.sys_parameter("BZ", "BZ0007");
                    spath = string.IsNullOrEmpty(spath) ? @"C:\Program Files (x86)\Infolight\EEP2012\EEPNetClient\FastReport" : spath;
                    string cfile = Application.StartupPath + @"\FastReport\YB\九江市城乡居民医保一站式结算单.frx";
                    string sfile = spath + @"\YB\九江市城乡居民医保一站式结算单.frx";
                    CliUtils.DownLoad(sfile, cfile);

                    if (!File.Exists(cfile))
                    {
                        MessageBox.Show("医保提示：" + cfile + "不存在!", "提示");
                        return new object[] { 0, 0, "医保提示：" + cfile + "不存在!", "提示" };
                    }

                    Report report = new Report();
                    report.PrintSettings.ShowDialog = false;
                    report.Load(cfile);
                    report.RegisterData(ds1);
                    report.SetParameterValue("yljgmc", CliUtils.fLoginComp);
                    report.SetParameterValue("ryrq", ryrq);
                    report.SetParameterValue("cyrq", cyrq);
                    report.SetParameterValue("zyts", zyts);
                    report.SetParameterValue("jbr", czy);
                    report.SetParameterValue("yljgdj", "二级医院" + cdbz);
                    report.SetParameterValue("dyrq", DateTime.Now.ToString("yyyy-MM-dd"));
                    report.SetParameterValue("djh", djhin);
                    report.SetParameterValue("jzlsh", jzlsh);
                    report.SetParameterValue("jsrq", jsrq);
                    report.SetParameterValue("xm", xm);
                    report.SetParameterValue("xb", xb);
                    report.SetParameterValue("kh", grbh);
                    report.SetParameterValue("yllb", yllb);
                    report.SetParameterValue("yldylb", yldylb);
                    report.SetParameterValue("tcq", tcq);
                    report.SetParameterValue("jbzd", jbzd);
                    report.SetParameterValue("yljgbh", yljgbh);
                    report.SetParameterValue("ylfze", ylfze);
                    report.SetParameterValue("jrtcfy", jrtcfy);
                    report.SetParameterValue("qfx", qfx);
                    report.SetParameterValue("ylzlfy", ylzlfy);
                    report.SetParameterValue("blzlfy", blzlfy);
                    report.SetParameterValue("cxjfy", cxjfy);
                    report.SetParameterValue("zffy", zffy);
                    report.SetParameterValue("ypylqc", ypylqc);
                    report.SetParameterValue("zhye", zhye);
                    report.SetParameterValue("bntczflj", bntczflj);
                    report.SetParameterValue("tcjjzf", tcjjzf);
                    report.SetParameterValue("tcjjzfzhzfhj", tcjjzfzhzfhj);
                    report.SetParameterValue("bndezflj", bndezflj);
                    report.SetParameterValue("jjjmdbydje", jjjmdbydje);
                    report.SetParameterValue("jjjmdbedje", jjjmdbedje);
                    report.SetParameterValue("ecbcje", ecbcje);
                    report.SetParameterValue("dbqfx", dbqfx);
                    report.SetParameterValue("bcdbbxbxhj", bcdbbxbxhj);
                    report.SetParameterValue("bczcfwnfybxje", bczcfwnfybxje);
                    report.SetParameterValue("bczfyphylqcfybxje", bczfyphylqcfybxje);
                    report.SetParameterValue("bcjbbcbxbxhj", bcjbbcbxbxhj);
                    report.SetParameterValue("bcjzjemt", bcjzjemt);
                    report.SetParameterValue("bcjzjezy", bcjzjezy);
                    report.SetParameterValue("bcjzhj", bcjzhj);
                    report.SetParameterValue("bnzfddjjfylj", bnzfddjjfylj);
                    report.SetParameterValue("zfddjjfy", zfddjjfy);
                    report.SetParameterValue("grfdfy", grfdfy);
                    report.SetParameterValue("zhzf", zhzf);
                    report.SetParameterValue("xjzf", xjzf);
                    report.SetParameterValue("yyfdfy", yyfdfy);
                    report.SetParameterValue("dxtcjjzfzhzfhj", dxtcjjzfzhzfhj);
                    report.SetParameterValue("dxbcdbbxbxhj", dxbcdbbxbxhj);
                    report.SetParameterValue("dxbcjbbcbxbxhj", dxbcjbbcbxbxhj);
                    report.SetParameterValue("dxbcjzhj", dxbcjzhj);
                    report.SetParameterValue("dxzfddjjfy", dxzfddjjfy);
                    report.SetParameterValue("dwmc", dwmc);

                    if (CliUtils.fPrintPreview == "True")
                    {
                        report.Show();
                    }
                    else
                    {
                        report.Print();
                    }

                    report.Dispose();
                    return new object[] { 0, 1, "医保提示：打印成功", "提示" };
                }
                else
                {
                    MessageBox.Show("医保提示：his无该病人住院信息", "提示");
                    return new object[] { 0, 0, "医保提示：his无该病人住院信息", "提示" };
                }
            }
            catch (Exception err)
            {
                MessageBox.Show("打印医保清单失败：" + err.ToString(), "提示");
                return new object[] { 0, 2, "打印医保清单失败：" + err.ToString(), "提示" };
            }
        }
        #endregion 打印住院结算清单

        #region 打印住院结算清单全部
        /// <summary>
        /// 打印住院结算清单全部
        /// </summary>
        /// <param>his住院号，单据号，出院日期(格式：yyyy-MM-dd HH:mm:ss),重打标志</param>
        public static object[] PrintYBQDZYQBDR(object[] objParam)
        {
            try
            {
                string zybah = objParam[0].ToString();
                string djh = objParam[1].ToString();
                string ldat = objParam[2].ToString();
                string cd = objParam[3].ToString();
                string sql = string.Format(@"select distinct a.z1date, b.z1outd from zy01h(nolock) a join zy01d(nolock) b on a.z1zyno = b.z1zyno where a.z1comp = '{0}' and b.z1comp = '{0}' and a.z1zyno = '{1}' and left(b.z1endv, 1) = '8'", CliUtils.fSiteCode, zybah);//0秒
                DataSet dsP = CliUtils.ExecuteSql("sybdj", "cmd", sql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);

                if (dsP.Tables[0].Rows.Count > 0)
                {
                    DataRow dr = dsP.Tables[0].Rows[0];
                    DateTime d1 = Convert.ToDateTime(dr["z1date"]);
                    DateTime d2 = dr["z1outd"] == DBNull.Value ? Convert.ToDateTime(ldat) : Convert.ToDateTime(dr["z1outd"]);
                    string ryrq = d1.ToString("yyyy-MM-dd");
                    string cyrq = d2.ToString("yyyy-MM-dd");
                    TimeSpan ts = d2.Date - d1.Date;
                    int zyts = ts.Days;
                    string czy = CliUtils.fUserName;
                    sql = string.Format(@"select sum(z3amnt * (case left(z3endv, 1) when '4' then -1 else 1 end)) z3amt2, sflb 
                    from zy03d(nolock) 
                    left join ybhisdzdr(nolock) on z3item = hisxmbh and scbz = 1
                    where isnull(z3ybup, '') != '' and (z3jshx is null or z3jshx = '{3}') and z3date < '{2}' and left(z3kind, 1) in ('2', '4') and z3comp = '{1}' and z3zyno = '{0}' 
                    group by sflb having sum(case left(z3endv, 1) when '4' then -z3jzcs else z3jzcs end) > 0 and sum(case left(z3endv, 1) when '4' then -z3jzje else z3jzje end) > 0", zybah, CliUtils.fSiteCode, d2.AddDays(1), djh);//0秒
                    DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", sql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);

                    if (ds == null || ds.Tables[0].Rows.Count == 0)
                    {
                        MessageBox.Show("医保提示：该病人未做医保结算", "提示");
                        return new object[] { 0, 0, "医保提示：该病人未做医保结算", "提示" };
                    }

                    string sql1 = string.Format(@"select c.bzname, c.bzname yllb, a.jylsh, a.djh, a.djhin, a.xm, a.grbh, a.kh, b.ybjzlsh, d.bzname yldylb
                    , b.dwmc + e.bzname dwmc, a.bcjsqzhye - a.zhzf zhye, a.jzlsh, a.ylfze, a.zbxje, a.tcjjzf, a.dejjzf
                    , a.zhzf, a.xjzf, a.ylzlfy, a.blzlfy, a.zffy, a.dwfdfy, a.cxjfy, a.mzjzfy, a.qfbzfy, a.zzzyzffy, a.jrtcfy, a.tcfdzffy, a.ctcfdxfy
                    , a.jrdebxfy, a.bntczflj, a.defdzffy, a.dbzbc, a.ecbcje, a.tjje, a.bnzhzflj, a.sysdate
                    , a.gwybzjjzf, a.qybcylbxjjzf, a.yyfdfy, a.fhjbylfy, a.cdefdxfy, a.rgqgzffy, a.bcjsqzhye
                    , a.bndezflj, a.bnczjmmztczflj, a.bngwybzzflj, a.bnzycslj, a.mmqflj, a.czzcje, a.elmmxezc
                    , a.elmmxesy, a.jmgrzfecbcje, a.syjjzf, a.jjjmdbydje, a.jjjmdbedje, a.jjjmjbbcfwnje, a.jjjmjbbcfwwje
                    from ybfyjsdr(nolock) a
                    join ybmzzydjdr(nolock) b on a.jzlsh = b.jzlsh
                    left join bztbd(nolock) c on c.bzcodn = 'YL' and c.bzmem1 = a.yllb
                    left join bztbd(nolock) d on d.bzcodn = 'DL' and d.bzmem1 = a.yldylb
                    left join bztbd(nolock) e on b.tcqh = e.bzkeyx and e.bzcodn = 'DQ' and e.bzusex = 1
                    where a.jzlsh = '{0}' and a.djhin = '{1}' and a.cxbz = 1 and b.cxbz = 1 and b.jzbz = 'z'", zybah, djh);//1秒
                    DataSet ds1 = CliUtils.ExecuteSql("szy02", "zy01h", sql1, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                    ds.Tables.Add(ds1.Tables[0].Copy());
                    string spath = Para.sys_parameter("BZ", "BZ0007");
                    spath = string.IsNullOrEmpty(spath) ? @"C:\Program Files (x86)\Infolight\EEP2012\EEPNetClient\FastReport" : spath;
                    string cfile = Application.StartupPath + @"\FastReport\YB\九江市医疗社会保险住院收费单全部.frx";
                    string sfile = spath + @"\YB\九江市医疗社会保险住院收费单全部.frx";
                    CliUtils.DownLoad(sfile, cfile);

                    if (!File.Exists(cfile))
                    {
                        MessageBox.Show("医保提示：" + cfile + "不存在!", "提示");
                        return new object[] { 0, 0, "医保提示：" + cfile + "不存在!", "提示" };
                    }

                    Report report = new Report();
                    report.PrintSettings.ShowDialog = false;
                    report.Load(cfile);
                    report.RegisterData(ds);
                    report.SetParameterValue("comp", CliUtils.fLoginComp);
                    report.SetParameterValue("ryrq", ryrq);
                    report.SetParameterValue("cyrq", cyrq);
                    report.SetParameterValue("zyts", zyts);
                    report.SetParameterValue("czy", czy);
                    report.SetParameterValue("yydj", "三级医院" + cd);

                    if (CliUtils.fPrintPreview == "True")
                    {
                        report.Show();
                    }
                    else
                    {
                        report.Print();
                    }

                    report.Dispose();
                    return new object[] { 0, 1, "医保提示：打印成功", "提示" };
                }
                else
                {
                    MessageBox.Show("医保提示：his无该病人住院信息", "提示");
                    return new object[] { 0, 0, "医保提示：his无该病人住院信息", "提示" };
                }
            }
            catch (Exception err)
            {
                MessageBox.Show("打印医保清单失败：" + err.ToString(), "提示");
                return new object[] { 0, 2, "打印医保清单失败：" + err.ToString(), "提示" };
            }
        }
        #endregion 打印住院结算清单全部

        #region 打印门诊结算清单
        /// <summary>
        /// 打印门诊结算清单
        /// </summary>
        /// <param>单据号，重打标志</param>
        public static object[] PrintYBQDMZDR(string djh, string cd)
        {
            try
            {
                string czy = CliUtils.fUserName;
                string sql = string.Format(@"select sum(a.m3camt) m3camt, a.m3sfnm 
                from mz03d(nolock) a 
                where left(a.m3endv, 1) not in ('1', '4') and a.m3shno = '{0}' 
                group by a.m3sfnm
                union all select top 1 a.m1amnt, '诊查费'
                from mz01h(nolock) a 
                where m1invo = '{0}' and not exists (select 1 from mz01h(nolock) b where b.m1ghno = 'T' + a.m1ghno)", djh);//已优化
                DataSet ds = CliUtils.ExecuteSql("smz03", "mz03d", sql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);

                if (ds == null || ds.Tables[0].Rows.Count == 0)
                {
                    MessageBox.Show("医保提示：该病人无费用结算信息", "提示");
                    return new object[] { 0, 0, "医保提示：该病人无费用结算信息", "提示" };
                }

                string sql1 = string.Format(@"select c.bzname, c.bzname yllb, a.jylsh, a.djh, a.djhin, a.xm, a.grbh, a.kh, b.ybjzlsh, d.bzname yldylb
                , b.dwmc, e.bzname tcq, a.bcjsqzhye - a.zhzf zhye, a.jzlsh, a.ylfze, a.zbxje, a.tcjjzf, a.dejjzf
                , a.zhzf, a.xjzf, a.ylzlfy, a.blzlfy, a.zffy, a.dwfdfy, a.cxjfy, a.mzjzfy, a.qfbzfy, a.zzzyzffy, a.jrtcfy, a.tcfdzffy, a.ctcfdxfy
                , a.jrdebxfy, a.bntczflj, a.defdzffy, a.dbzbc, a.ecbcje, a.tjje, a.bnzhzflj, a.sysdate, a.qybcylbxjjzf
                from ybfyjsdr(nolock) a
                join ybmzzydjdr(nolock) b on a.jzlsh = b.jzlsh
                left join bztbd(nolock) c on c.bzcodn = 'YL' and c.bzmem1 = a.yllb
                left join bztbd(nolock) d on d.bzcodn = 'DL' and d.bzmem1 = a.yldylb
                left join bztbd(nolock) e on b.tcqh = e.bzkeyx and e.bzcodn = 'DQ' and e.bzusex = 1
                where a.djhin = '{0}' and a.cxbz = 1 and b.cxbz = 1 and b.jzbz = 'm'", djh);//0秒
                DataSet ds1 = CliUtils.ExecuteSql("sybdj", "cmd", sql1, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                ds.Tables.Add(ds1.Tables[0].Copy());
                string spath = Para.sys_parameter("BZ", "BZ0007");
                spath = string.IsNullOrEmpty(spath) ? @"C:\Program Files (x86)\Infolight\EEP2012\EEPNetClient\FastReport" : spath;
                string cfile = Application.StartupPath + @"\FastReport\YB\九江市医疗社会保险门诊收费单.frx";
                string sfile = spath + @"\YB\九江市医疗社会保险门诊收费单.frx";
                CliUtils.DownLoad(sfile, cfile);

                if (!File.Exists(cfile))
                {
                    MessageBox.Show("医保提示：" + cfile + "不存在!", "提示");
                    return new object[] { 0, 0, "医保提示：" + cfile + "不存在!", "提示" };
                }

                Report report = new Report();
                report.PrintSettings.ShowDialog = false;
                report.Load(cfile);
                report.RegisterData(ds);
                report.SetParameterValue("comp", CliUtils.fLoginComp);
                report.SetParameterValue("jzrq", Convert.ToDateTime(ds1.Tables[0].Rows[0]["sysdate"]).ToString("yyyy-MM-dd"));
                report.SetParameterValue("czy", czy);
                report.SetParameterValue("yydj", "三级医院" + cd);

                if (CliUtils.fPrintPreview == "True")
                {
                    report.Show();
                }
                else
                {
                    report.Print();
                }

                report.Dispose();
                return new object[] { 0, 1, "医保提示：打印成功", "提示" };
            }
            catch (Exception err)
            {
                MessageBox.Show("打印医保清单失败：" + err.ToString(), "提示");
                return new object[] { 0, 2, "打印医保清单失败：" + err.ToString(), "提示" };
            }
        }
        #endregion 打印门诊结算清单

        #region 打印门诊结算清单全部
        /// <summary>
        /// 打印门诊结算清单全部
        /// </summary>
        /// <param>单据号，重打标志</param>
        public static object[] PrintYBQDMZQBDR(object[] objParam)
        {
            try
            {
                string djh = objParam[0].ToString();
                string cd = objParam[0].ToString();
                string czy = CliUtils.fUserName;
                string sql = string.Format(@"select sum(a.m3camt) m3camt, a.m3sfnm 
                from mz03d(nolock) a 
                where left(a.m3endv, 1) not in ('1', '4') and a.m3shno = '{0}' 
                group by a.m3sfnm", djh);//已优化
                DataSet ds = CliUtils.ExecuteSql("smz03", "mz03d", sql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);

                if (ds == null || ds.Tables[0].Rows.Count == 0)
                {
                    MessageBox.Show("医保提示：该病人无费用结算信息", "提示");
                    return new object[] { 0, 0, "医保提示：该病人无费用结算信息", "提示" };
                }

                string sql1 = string.Format(@"select c.bzname, c.bzname yllb, a.jylsh, a.djh, a.djhin, a.xm, a.grbh, a.kh, b.ybjzlsh, d.bzname yldylb
                , b.dwmc + e.bzname dwmc, a.bcjsqzhye - a.zhzf zhye, a.jzlsh, a.ylfze, a.zbxje, a.tcjjzf, a.dejjzf
                , a.zhzf, a.xjzf, a.ylzlfy, a.blzlfy, a.zffy, a.dwfdfy, a.cxjfy, a.mzjzfy, a.qfbzfy, a.zzzyzffy, a.jrtcfy, a.tcfdzffy, a.ctcfdxfy
                , a.jrdebxfy, a.bntczflj, a.defdzffy, a.dbzbc, a.ecbcje, a.tjje, a.bnzhzflj, a.sysdate
                , a.gwybzjjzf, a.qybcylbxjjzf, a.yyfdfy, a.fhjbylfy, a.cdefdxfy, a.rgqgzffy, a.bcjsqzhye
                , a.bndezflj, a.bnczjmmztczflj, a.bngwybzzflj, a.bnzycslj, a.mmqflj, a.czzcje, a.elmmxezc
                , a.elmmxesy, a.jmgrzfecbcje, a.syjjzf, a.jjjmdbydje, a.jjjmdbedje, a.jjjmjbbcfwnje, a.jjjmjbbcfwwje
                from ybfyjsdr(nolock) a
                join ybmzzydjdr(nolock) b on a.jzlsh = b.jzlsh
                left join bztbd(nolock) c on c.bzcodn = 'YL' and c.bzmem1 = a.yllb
                left join bztbd(nolock) d on d.bzcodn = 'DL' and d.bzmem1 = a.yldylb
                left join bztbd(nolock) e on b.tcqh = e.bzkeyx and e.bzcodn = 'DQ' and e.bzusex = 1
                where a.djhin = '{0}' and a.cxbz = 1 and b.cxbz = 1 and b.jzbz = 'm'", djh);//1秒
                DataSet ds1 = CliUtils.ExecuteSql("sybdj", "cmd", sql1, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                ds.Tables.Add(ds1.Tables[0].Copy());
                string spath = Para.sys_parameter("BZ", "BZ0007");
                spath = string.IsNullOrEmpty(spath) ? @"C:\Program Files (x86)\Infolight\EEP2012\EEPNetClient\FastReport" : spath;
                string cfile = Application.StartupPath + @"\FastReport\YB\九江市医疗社会保险门诊收费单全部.frx";
                string sfile = spath + @"\YB\九江市医疗社会保险门诊收费单全部.frx";
                CliUtils.DownLoad(sfile, cfile);

                if (!File.Exists(cfile))
                {
                    MessageBox.Show("医保提示：" + cfile + "不存在!", "提示");
                    return new object[] { 0, 0, "医保提示：" + cfile + "不存在!", "提示" };
                }

                Report report = new Report();
                report.PrintSettings.ShowDialog = false;
                report.Load(cfile);
                report.RegisterData(ds);
                report.SetParameterValue("comp", CliUtils.fLoginComp);
                report.SetParameterValue("jzrq", Convert.ToDateTime(ds1.Tables[0].Rows[0]["sysdate"]).ToString("yyyy-MM-dd"));
                report.SetParameterValue("czy", czy);
                report.SetParameterValue("yydj", "三级医院" + cd);

                if (CliUtils.fPrintPreview == "True")
                {
                    report.Show();
                }
                else
                {
                    report.Print();
                }

                report.Dispose();
                return new object[] { 0, 1, "医保提示：打印成功", "提示" };
            }
            catch (Exception err)
            {
                MessageBox.Show("打印医保清单失败：" + err.ToString(), "提示");
                return new object[] { 0, 2, "打印医保清单失败：" + err.ToString(), "提示" };
            }
        }
        #endregion 打印门诊结算清单全部

        #region 打印费用明细清单
        /// <summary>
        /// 打印费用明细清单
        /// </summary>
        /// <param>his就诊流水号，单据号</param>
        public static void PrintYBFYMXQDDR(object[] objParam)
        {
            try
            {
                string jzlsh = objParam[0].ToString();
                string sql = string.Format(@"select * from ybcfmxscindr(nolock) a where a.jzlsh = '{0}' and cxbz = 1", jzlsh);//0秒
                DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", sql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    string spath = Para.sys_parameter("BZ", "BZ0007");
                    spath = string.IsNullOrEmpty(spath) ? @"C:\Program Files (x86)\Infolight\EEP2012\EEPNetClient\FastReport" : spath;
                    string cfile = Application.StartupPath + @"\FastReport\YB\九江市中医院医保上传费用明细报表.frx";
                    string sfile = spath + @"\YB\九江市中医院医保上传费用明细报表.frx";
                    CliUtils.DownLoad(sfile, cfile);

                    if (!File.Exists(cfile))
                    {
                        MessageBox.Show("医保提示：" + cfile + "不存在!", "提示");
                    }

                    Report report = new Report();
                    report.PrintSettings.ShowDialog = false;
                    report.Load(cfile);
                    report.RegisterData(ds);
                    report.SetParameterValue("user", CliUtils.fUserName);

                    if (CliUtils.fPrintPreview == "True")
                    {
                        report.Show();
                    }
                    else
                    {
                        report.Print();
                    }

                    report.Dispose();
                }
                else
                {
                    MessageBox.Show("医保提示：医保无该病人费用上传信息", "提示");
                }
            }
            catch (Exception err)
            {
                MessageBox.Show("打印费用明细清单失败：" + err.ToString(), "提示");
            }
        }
        #endregion 打印费用明细清单

        #region 住院处方日期修改
        /// <summary>
        /// 住院处方日期修改
        /// </summary>
        /// <param>his就诊流水号,医院项目编码集(格式：'01','02')</param>
        /// <returns>1:成功，0:不成功，2:报错</returns>
        public static object[] YBZYCFRQXGDR(object[] objParam)
        {
            string jzlsh = objParam[0].ToString();
            string xmbhs = objParam[1].ToString();

            if ("" == jzlsh || "" == xmbhs)
            {
                return new object[] { 0, 0, "医保提示：his就诊流水号或医院项目编码为空", "提示" };
            }

            try
            {
                string strSql = string.Format("update zy03d set z3date = '2017-12-29 23:59:59' where z3zyno = '{0}' and z3item in ({1}) and convert(datetime, z3date) >= '2017-12-30'", jzlsh, xmbhs);//0秒 and left(z3endv, 1) = '4'
                object[] obj = new object[] { strSql };
                obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);

                if (obj[1].ToString() == "1")
                {
                    return new object[] { 0, 1, "医保提示：住院处方日期修改成功" };
                }
                else
                {
                    return new object[] { 0, -1, "医保提示：住院处方日期修改失败" };
                }
            }
            catch (Exception error)
            {
                Common.WriteYBLog(error.ToString());
                return new object[] { 0, 2, "住院处方日期修改Error:" + error.ToString() };
            }
        }
        #endregion 住院处方日期修改

        #region 住院处方日期修改20180101
        /// <summary>
        /// 住院处方日期修改20180101
        /// </summary>
        /// <param>his就诊流水号,医院项目编码集(格式：'01','02')</param>
        /// <returns>1:成功，0:不成功，2:报错</returns>
        public static object[] YBZYCFRQXGDR20180101(object[] objParam)
        {
            string jzlsh = objParam[0].ToString();
            string xmbhs = objParam[1].ToString();

            if ("" == jzlsh || "" == xmbhs)
            {
                return new object[] { 0, 0, "医保提示：his就诊流水号或医院项目编码为空", "提示" };
            }

            try
            {
                string strSql = string.Format("update zy03d set z3date = '2017-12-31 23:59:59' where z3zyno = '{0}' and z3item in ({1}) and convert(datetime, z3date) >= '2018-01-01'", jzlsh, xmbhs);//0秒 and left(z3endv, 1) = '4'
                object[] obj = new object[] { strSql };
                obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);

                if (obj[1].ToString() == "1")
                {
                    return new object[] { 0, 1, "医保提示：住院处方日期修改成功" };
                }
                else
                {
                    return new object[] { 0, -1, "医保提示：住院处方日期修改失败" };
                }
            }
            catch (Exception error)
            {
                Common.WriteYBLog(error.ToString());
                return new object[] { 0, 2, "住院处方日期修改Error:" + error.ToString() };
            }
        }
        #endregion 住院处方日期修改20180101

        //处方明细手动撤销 内外部调用 转换
        public static int ChangeYBCFMXSDCXDR(object[] objParam)
        {
            object[] re = YBCFMXSDCXDR(objParam);
            if (re[0] == "0")
                return 0;
            else if (re[0] == "2")
                return 2;
            else return 1;
        }

        //挂号/住院登记撤销 转换
        public static object[] ChangeYBDJSDCXDR(object[] objParam)
        {
            int i = YBDJSDCXDR(objParam[0].ToString(), objParam[1].ToString(), objParam[2].ToString(), objParam[3].ToString(), objParam[4].ToString());
            if (i == 0)
                return (new object[] { "0" });
            else if (i == 1)
                return (new object[] { "1" });
            else
                return (new object[] { "2" });
        }

        //费用结算撤销（无数据库）转换
        public static object[] ChangeYBFYJSSDCXDR(object[] objParam)
        {
            if (false)
            {
                int i = YBFYJSSDCXDR(objParam[0].ToString(), objParam[1].ToString(), objParam[2].ToString(), objParam[3].ToString(), objParam[4].ToString(), objParam[5].ToString(), objParam[6].ToString());
                if (i == 0)
                    return (new object[] { "0" });
                else if (i == 1)
                    return (new object[] { "1" });
                else
                    return (new object[] { "2" });
            }
            try
            {
                string ybjzlsh = objParam[0].ToString();
                string djh = objParam[1].ToString();
                string jsrq = objParam[2].ToString();
                string grbh = objParam[3].ToString();
                string xm = objParam[4].ToString();
                string kh = objParam[5].ToString();

                DateTime dqsj = Convert.ToDateTime(Common.GetServerTime());
                string jylsh = dqsj.ToString("yyyyMMddHHmmss") + "-" + DDYLJGBH + "-" + djh;

                string rc = ybjzlsh + "|" + djh + "|" + jsrq + "|" + CliUtils.fUserName + "||" + Gocent + "|" + grbh + "|" + xm + "|" + kh + "|";
                StringBuilder inputData = new StringBuilder(string.Format("{0}^{1}^{2}^{3}^{4}^{5}^{6}^{7}^", "2430", DDYLJGBH, CliUtils.fLoginUser, YWZQH, jylsh, ZXBM, rc, "1"));
                StringBuilder outputData = new StringBuilder(100000);
                int i = BUSINESS_HANDLE(inputData, outputData);

                if (i == 0)
                {
                    return new object[]{1,outputData.ToString()};
                }
                else
                {
                    Common.InsertYBLog(ybjzlsh, inputData.ToString(), outputData.ToString());
                    return new object[]{0,outputData.ToString()};
                }
            }
            catch (Exception error)
            {
                return new object[]{"2",error.ToString()};
            }

           
        }
        #endregion 业务类

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