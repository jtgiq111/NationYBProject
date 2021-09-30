using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Srvtools;
using System.Net.NetworkInformation;
using System.IO;
using System.Data;
using System.Windows.Forms;
using System.Threading;

namespace ybinterface_lib
{
    public class yb_interface_yt_dr
    {
        #region 变量
        internal static string YWBH = string.Empty; //业务编号
        internal static string CZYBH = string.Empty;//操作员工号
        internal static string YWZQH = string.Empty;//业务周期号
        internal static string JYLSH = string.Empty;//交易流水号
        internal static string ZXBM = "0000";       //中心编码
        internal static string LJBZ = "1";          //联机标志
        internal static string YBKLX="0";                  //医保卡类型    0:正式卡，1：临时卡
        internal static string DQJBBZ = "1";        //地区级别标志 1-本市级 2-本省异地  3-其他
        internal static string DKXX = "";

        static IWork iWork = new Work();
        static string xmlPath = AppDomain.CurrentDomain.BaseDirectory;
        static List<Item1> lItem = iWork.getXmlConfig1(xmlPath + "EEPNetClient.exe.config");
        internal static string YLGHBH = lItem[0].DDYLJGBH; //医疗机构编号
        internal static string DDYLJGMC = lItem[0].DDYLJGMC;//医院名称
        internal static string YBIP = lItem[0].YBIP;        //医保IP地址
        internal static string TIMEOUT = lItem[0].TIMEOUT; //设置读卡过期时间
        #endregion

        #region 医保dll函数声明
        [DllImport("SiInterface.dll", EntryPoint = "INIT", CharSet = CharSet.Ansi)]
        static extern int INIT(StringBuilder pErrMsg);

        [DllImport("SiInterface.dll", EntryPoint = "BUSINESS_HANDLE", CharSet = CharSet.Ansi)]
        static extern int BUSINESS_HANDLE(StringBuilder inputData, StringBuilder outputData);
        #endregion

        #region 查询类
        #region 医疗费信息查询
        public static object[] YBYLFXXCX(object[] objParam)
        {
            string sysdate = GetServerDateTime();
            WriteLog(sysdate + "  进入医疗费信息查询...");

            try
            {
                YWZQH = CliUtils.fLoginYbNo.Split('|')[0].ToString();    //业务周期号
            }
            catch
            {
                return new object[] { 0, 0, "医保未连接或未初始化" };
            }
            try
            {
                string startDate = objParam[0].ToString(); //开始时间
                string endDate = objParam[1].ToString();    //结束时间
                string sywzqh = objParam[2].ToString();     //如果输入了业务周期号，则按照输入的业务周期号进行查询

                if (string.IsNullOrEmpty(startDate))
                    return new object[] { 0, 0, "开始时间不能为空" };
                if (string.IsNullOrEmpty(endDate))
                    return new object[] { 0, 0, "结束时间不能为空" };

                string sDate = Convert.ToDateTime(startDate).ToString("yyyyMMddHHmmss");
                string eDate = Convert.ToDateTime(endDate).ToString("yyyyMMddHHmmss");

                CZYBH = CliUtils.fLoginUser; //操作员工号
                JYLSH = DateTime.Now.ToString("yyyyMMddHHmmss") + "-" + YLGHBH + "-" + new Random().Next(100).ToString().PadLeft(4, '0');
                YWBH = "1100";  //业务编号

                StringBuilder inParam = new StringBuilder();
                inParam.Append(sDate + "|");
                inParam.Append(eDate + "|");
                inParam.Append(sywzqh + "|");

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
                    WriteLog(sysdate + "  医疗费信息查询成功|出参|" + outputData.ToString());
                    return new object[] { 0, 1, "医疗费信息查询成功|" + outputData.ToString(), outputData.ToString().Split('^')[2] };
                }
                else
                {
                    WriteLog(sysdate + "  医疗费信息查询失败|" + outputData.ToString());
                    return new object[] { 0, 0, "医疗费信息查询失败|" + outputData.ToString() };
                }
            }
            catch (Exception ex)
            {
                WriteLog(sysdate + "  医疗费信息查询异常|" + ex.Message);
                return new object[] { 0, 0, "医疗费信息查询异常|" + ex.Message };
            }
        }
        #endregion

        #region 结算信息查询
        public static object[] YBJSXXCX(object[] objParam)
        {
            string sysdate = GetServerDateTime();
            WriteLog(sysdate + "  进入结算信息查询...");
            try
            {
                string ybjzlsh = objParam[0].ToString();  //医保就诊流水号
                string djh = objParam[1].ToString();    //单据号(医保返回)

                if (string.IsNullOrEmpty(ybjzlsh))
                    return new object[] { 0, 0, "医保就诊流水号不能为空" };
                if (string.IsNullOrEmpty(djh))
                    return new object[] { 0, 0, "单据号不能为空" };

                CZYBH = CliUtils.fLoginUser; //操作员工号
                YWZQH = CliUtils.fLoginYbNo.Split('|')[0].ToString();    //业务周期号
                JYLSH = DateTime.Now.ToString("yyyyMMddHHmmss") + "-" + YLGHBH + "-" + new Random().Next(100).ToString().PadLeft(4, '0');
                YWBH = "1101";  //业务编号

                StringBuilder inParam = new StringBuilder();
                inParam.Append(ybjzlsh + "|");
                inParam.Append(djh + "|");

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
                    WriteLog(sysdate + "  结算信息查询成功|出参|"+outputData.ToString());
                    return new object[] { 0, 1, "结算信息查询成功|"+outputData.ToString()};
                }
                else
                {
                    WriteLog(sysdate + "  结算信息查询失败|" + outputData.ToString());
                    return new object[] { 0, 0, "结算信息查询失败|" + outputData.ToString() };
                }
            }
            catch (Exception ex)
            {
                WriteLog(sysdate + "  结算信息查询异常|"+ex.Message);
                return new object[] { 0, 0, "结算信息查询异常|" + ex.Message };
            }
        }
        #endregion

        #region 个人医疗费信息查询
        public static object[] YBGRYLFXXCX(object[] objParam)
        {
            string sysdate = GetServerDateTime();
            WriteLog(sysdate + "  进入个人医疗费信息查询...");
            try
            {
                string startDate = objParam[0].ToString(); //开始时间
                string endDate = objParam[1].ToString();    //结束时间
                string grbh = objParam[2].ToString();     //如果输入了业务周期号，则按照输入的业务周期号进行查询

                if (string.IsNullOrEmpty(startDate))
                    return new object[] { 0, 0, "开始时间不能为空" };
                if (string.IsNullOrEmpty(endDate))
                    return new object[] { 0, 0, "结束时间不能为空" };
                if (string.IsNullOrEmpty(grbh))
                    return new object[] { 0, 0, "个人编号不能为空" };

                string sDate = Convert.ToDateTime(startDate).ToString("yyyyMMddHHmmss");
                string eDate = Convert.ToDateTime(endDate).ToString("yyyyMMddHHmmss");



                CZYBH = CliUtils.fLoginUser; //操作员工号
                YWZQH = CliUtils.fLoginYbNo.Split('|')[0].ToString();    //业务周期号
                JYLSH = DateTime.Now.ToString("yyyyMMddHHmmss") + "-" + YLGHBH + "-" + new Random().Next(100).ToString().PadLeft(4, '0');
                YWBH = "1102";  //业务编号

                StringBuilder inParam = new StringBuilder();
                inParam.Append(sDate + "|");
                inParam.Append(eDate + "|");
                inParam.Append(grbh + "|");

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
                    WriteLog(sysdate + "  个人医疗费信息查询成功|出参|" + outputData.ToString());
                    return new object[] { 0, 1, "个人医疗费信息查询成功|" + outputData.ToString() };
                }
                else
                {
                    WriteLog(sysdate + "  个人医疗费信息查询失败|" + outputData.ToString());
                    return new object[] { 0, 0, "个人医疗费信息查询失败|" + outputData.ToString() };
                }
            }
            catch (Exception ex)
            {
                WriteLog(sysdate + "  个人医疗费信息查询异常|" + ex.Message);
                return new object[] { 0, 0, "个人医疗费信息查询异常|" + ex.Message };
            }
        }
        #endregion

        #region 个人就诊登记信息查询
        public static object[] YBGRJZDJXXCX(object[] objParam)
        {
            string sysdate = GetServerDateTime();
            WriteLog(sysdate + "  进入个人就诊登记信息查询...");
            try
            {
                string startDate = objParam[0].ToString(); //开始时间
                string endDate = objParam[1].ToString();    //结束时间
                string grbh = objParam[2].ToString();     //如果输入了业务周期号，则按照输入的业务周期号进行查询

                if (string.IsNullOrEmpty(startDate))
                    return new object[] { 0, 0, "开始时间不能为空" };
                if (string.IsNullOrEmpty(endDate))
                    return new object[] { 0, 0, "结束时间不能为空" };
                if (string.IsNullOrEmpty(grbh))
                    return new object[] { 0, 0, "个人编号不能为空" };

                string sDate = Convert.ToDateTime(startDate).ToString("yyyyMMddHHmmss");
                string eDate = Convert.ToDateTime(endDate).ToString("yyyyMMddHHmmss");



                CZYBH = CliUtils.fLoginUser; //操作员工号
                YWZQH = CliUtils.fLoginYbNo.Split('|')[0].ToString();    //业务周期号
                JYLSH = DateTime.Now.ToString("yyyyMMddHHmmss") + "-" + YLGHBH + "-" + new Random().Next(100).ToString().PadLeft(4, '0');
                YWBH = "1103";  //业务编号

                StringBuilder inParam = new StringBuilder();
                inParam.Append(sDate + "|");
                inParam.Append(eDate + "|");
                inParam.Append(grbh + "|");

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
                    WriteLog(sysdate + "  个人就诊登记信息查询成功|出参|" + outputData.ToString());
                    return new object[] { 0, 1, "个人就诊登记信息查询成功|" + outputData.ToString() };
                }
                else
                {
                    WriteLog(sysdate + "  个人就诊登记信息查询失败|" + outputData.ToString());
                    return new object[] { 0, 0, "个人就诊登记信息查询失败|" + outputData.ToString() };
                }
            }
            catch (Exception ex)
            {
                WriteLog(sysdate + "  个人就诊登记信息查询异常|" + ex.Message);
                return new object[] { 0, 0, "个人就诊登记信息查询异常|" + ex.Message };
            }
        }
        #endregion

        #region 月结算费用对帐
        public static object[] YBYJSFYDZ(object[] objParam)
        {
            string sysdate = GetServerDateTime();
            WriteLog(sysdate + "  进入月结算费用对帐...");
            try
            {
                string startDate = objParam[0].ToString(); //开始时间
                string endDate = objParam[1].ToString();    //结束时间
                string tcqh = objParam[2].ToString();     //

                if (string.IsNullOrEmpty(startDate))
                    return new object[] { 0, 0, "开始时间不能为空" };
                if (string.IsNullOrEmpty(endDate))
                    return new object[] { 0, 0, "结束时间不能为空" };
                if (string.IsNullOrEmpty(tcqh))
                    return new object[] { 0, 0, "统筹区号不能为空" };

                string sDate = Convert.ToDateTime(startDate).ToString("yyyyMMddHHmmss");
                string eDate = Convert.ToDateTime(endDate).ToString("yyyyMMddHHmmss");



                CZYBH = CliUtils.fLoginUser; //操作员工号
                YWZQH = CliUtils.fLoginYbNo.Split('|')[0].ToString();    //业务周期号
                JYLSH = DateTime.Now.ToString("yyyyMMddHHmmss") + "-" + YLGHBH + "-" + new Random().Next(100).ToString().PadLeft(4, '0');
                YWBH = "1140";  //业务编号

                StringBuilder inParam = new StringBuilder();
                inParam.Append(sDate + "|");
                inParam.Append(eDate + "|");
                inParam.Append(tcqh + "|");

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
                    WriteLog(sysdate + "  月结算费用对帐成功|出参|" + outputData.ToString());
                    return new object[] { 0, 1, "月结算费用对帐成功|" + outputData.ToString() };
                }
                else
                {
                    WriteLog(sysdate + "  月结算费用对帐失败|" + outputData.ToString());
                    return new object[] { 0, 0, "月结算费用对帐失败|" + outputData.ToString() };
                }
            }
            catch (Exception ex)
            {
                WriteLog(sysdate + "  月结算费用对帐异常|" + ex.Message);
                return new object[] { 0, 0, "月结算费用对帐异常|" + ex.Message };
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


        #region 冲正交易
        public static object[] YBCZJY(object[] objParam)
        {
            string sysdate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string ybjzlsh = objParam[0].ToString(); //医保就诊流水号 
            string ywbm = objParam[1].ToString(); //业务编码
            string czjylsh = objParam[2].ToString(); //交易流水号
            string jyrq = objParam[3].ToString(); //交易流水号(yyyyMMdd)

            if (string.IsNullOrEmpty(ybjzlsh))
                return new object[] { 0, 0, "医保就诊流水号不能为空" };
            if (string.IsNullOrEmpty(ywbm))
                return new object[] { 0, 0, "业务编码不能为空" };
            if (string.IsNullOrEmpty(czjylsh))
                return new object[] { 0, 0, "交易流水号不能为空" };

            try
            {
                CZYBH = CliUtils.fLoginUser; //操作员工号
                YWZQH = CliUtils.fLoginYbNo.Split('|')[0].ToString();    //业务周期号
                JYLSH = DateTime.Now.ToString("yyyyMMddHHmmss") + "-" + YLGHBH + "-" + new Random().Next(100).ToString().PadLeft(4, '0');

                YWBH = "2421";

                string strSql = string.Format(@"select * from ybmzzydjdr where ybjzlsh='{0}' and cxbz=1", ybjzlsh);
                DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                if (ds.Tables[0].Rows.Count == 0)
                {
                    WriteLog(sysdate + "  无医保登记信息");
                    return new object[] { 0, 0, "无医保登记信息" };
                }

                string grbh = ds.Tables[0].Rows[0]["grbh"].ToString();
                string xm = ds.Tables[0].Rows[0]["xm"].ToString();
                string kh = ds.Tables[0].Rows[0]["kh"].ToString();


                StringBuilder inputParam = new StringBuilder();
                inputParam.Append(ywbm + "|");   //业务编码
                inputParam.Append(czjylsh + "|");      //冲正交易流水号
                inputParam.Append(grbh + "|");      //个人编号
                inputParam.Append(kh + "|");      //卡号
                inputParam.Append(ybjzlsh + "|");      //就诊流水号
                inputParam.Append(jyrq + "|");      //交易日期

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
                WriteLog(sysdate + "  出参|" + outputData.ToString());
                if (i == 0)
                {
                    return new object[] { 0, 1, "冲正交易成功"};
                }
                else
                {
                    WriteLog(sysdate + "  出参|" + outputData.ToString());
                    return new object[] { 0, 0, "冲正交易失败" + outputData.ToString() };
                }
            }
            catch (Exception ex)
            {
                WriteLog(sysdate + "  冲正交易|异常|" + ex.Message);
                return new object[] { 0, 0, "冲正交易|异常|" + ex.Message };
            }
        }
        #endregion

        #endregion

        #region 认证类
        #region 医疗待遇封锁信息查询
        public static object[] YBYLDYFSXXCX(object[] objParam)
        {
            string sysdate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string grbh = objParam[0].ToString(); //个人编号 
            string kh = objParam[1].ToString(); //社会保障卡号
            string fssj = objParam[2].ToString(); //判断时间


            if (string.IsNullOrEmpty(grbh))
                return new object[] { 0, 0, "个人编码不能为空" };
            if (string.IsNullOrEmpty(kh))
                return new object[] { 0, 0, "医疗类别不能为空" };
            try
            {
                CZYBH = CliUtils.fLoginUser; //操作员工号
                YWZQH = CliUtils.fLoginYbNo.Split('|')[0].ToString();    //业务周期号
                JYLSH = DateTime.Now.ToString("yyyyMMddHHmmss") + "-" + YLGHBH + "-" + new Random().Next(100).ToString().PadLeft(4, '0');

                #region 参数
                /*
                1		个人编号	VARCHAR2(14)	NOT NULL	
                2		个人卡号	VARCHAR2(20)		NOT NULL
                3		判断时间	VARCHAR2(8)	NOT NULL	判断有效期，YYYYMMDD
                */
                YWBH = "1500";
                StringBuilder inputParam = new StringBuilder();
                inputParam.Append(grbh + "|");   //个人编号
                inputParam.Append(kh+"|");      //个人卡号
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
                if (i == 0)
                {
                    WriteLog(sysdate + " 医疗待遇封锁信息查询|出参|" + outputData.ToString());
                    string tsxx = outputData.ToString().Split('^')[2];
                    if(string.IsNullOrEmpty(tsxx))
                        return new object[] { 0, 0, "无封锁信息" };

                    //状态返回为0时，为无封锁信息   共他状态为封锁及返回信息。
                    return new object[] { 0, tsxx.Split('|')[0], tsxx };
                }
                else
                {
                    WriteLog(sysdate + " 医疗待遇封锁信息查询失败|" + outputData.ToString().Split('^')[2]);
                    return new object[] { 0, 0, outputData.ToString().Split('^')[2] };
                }
            }
            catch (Exception ex)
            {
                WriteLog(sysdate + "  医疗待遇封锁信息查询|异常|" + ex.Message);
                return new object[] { 0, 0, "医疗待遇封锁信息查询|异常|" + ex.Message };
            }
        }
        #endregion

        #region 医疗待遇审批信息查询
        public static object[] YBYLDYSPXXCX(object[] objParam)
        {
            string sysdate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            WriteLog(sysdate + "  进入医疗待遇审批信息查询...");
            try
            {
                string grbh = objParam[0].ToString();
                string splx = ""; //16-门诊慢性病 17-门诊特病

                if (string.IsNullOrEmpty(grbh))
                    return new object[] { 0, 0, "个人编码不能为空" };

                CZYBH = CliUtils.fLoginUser; //操作员工号
                YWZQH = CliUtils.fLoginYbNo.Split('|')[0].ToString();    //业务周期号
                JYLSH = DateTime.Now.ToString("yyyyMMddHHmmss") + "-" + YLGHBH + "-" + new Random().Next(100).ToString().PadLeft(4, '0');
                YWBH = "1600";
                #region 参数
                /*
                 1		个人编号	VARCHAR2(14)	NOT NULL	
                    2		审批类别	VARCHAR2(3)		审批类别为空时返还查询的审批信息，不为空时查询对应的审批信息
                    3		医院编号	VARCHAR2(14)		当为慢性病、特检特治特药时不能为空
                    4		就诊号	VARCHAR2(18)		审批类别为特检特治时:如果为空,当作查询挂号前,对应到某个人的审批查询,否则当作具体到某个项目的审批
                    5		项目编号	VARCHAR2(20)		当为特检特治特药时并且就诊号不为空时不能为空
                    6		病种编码	VARCHAR2(20)		
                    7		判断时间	VARCHAR2(8)		YYYYMMDD
                */
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
                     *1		审批标志	VARCHAR2(18)		二级代码
                        2		审批编号	VARCHAR2(16)		
                        3		个人编号	VARCHAR2(14)		
                        4		审批类别	VARCHAR2(3)		二级代码
                        5		就诊流水号	VARCHAR2(20)		
                        6		医院等级	VARCHAR2(3)		二级代码
                        7		病种编码	VARCHAR2(20)		
                        8		病种名称	VARCHAR2(50)		
                        9		医院意见	VARCHAR2(100)		特殊项目申报时指诊断意见
                        10		项目编码	VARCHAR2(20)		
                        11		项目名称	VARCHAR2(100)		
                        12		审批数量	VARCHAR2(6)		特殊项目审批的数量
                        13		申报日期	VARCHAR2(14)		YYYYMMDDHH24MISS
                        14		开始时间	VARCHAR2(14)		YYYYMMDDHH24MISS
                        15		终止时间	VARCHAR2(14)		YYYYMMDDHH24MISS
                        16		经办人	VARCHAR2(50)	NOT NULL	医疗机构操作员姓名
                        17		转外城市	VARCHAR2(100)		城市名称
                        18		备注	VARCHAR2(100)		
                        19		就诊医院编号	VARCHAR2(20)		
                        20		就诊医院名称	VARCHAR2(100)		
                        21		医师编号	VARCHAR2(20)		
                        22		医师姓名	VARCHAR2(50)		

                     * 1|4729071|0201|安义县中医院|TSB0004|高血压|20170101||$1|4729072|0201|安义县中医院|TSB0005|糖尿病|20170101||
                     */
                    string[] sval = outputData.ToString().Split('^')[2].ToString().Split('$');
                    foreach (string val in sval)
                    {
                        string[] sval1 = val.Split('|');
                        if (sval1[0].ToString().Equals("1"))
                        {
                            string svalue = sval1[6] + "|" + sval1[7];
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

        #endregion

        #region 业务类
        #region 初始化
        public static object[] YBINIT(object[] objParam)
        {
            try
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
            catch (Exception ex)
            {
                return new object[] { 0, 0, "初始化异常|" + ex.Message };
            }
        }
        #endregion

        #region 医保退出
        public static object[] YBEXIT(object[] objParam)
        {
            string sysdate = GetServerDateTime(); //获取系统时间
            CZYBH = CliUtils.fLoginUser; //操作员工号
            try
            {
                YWZQH = CliUtils.fLoginYbNo.Split('|')[0].ToString();    //业务周期号
            }
            catch
            {
                return new object[] { 0,0,"医保未连接或未初始化"};
            }

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
            DialogResult dresult = MessageBox.Show("请确认医保卡是否正常插入", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (dresult == DialogResult.Yes)
            {
                string sysdate = GetServerDateTime();
                CZYBH = CliUtils.fLoginUser; //操作员工号
                try
                {
                    YWZQH = CliUtils.fLoginYbNo.Split('|')[0].ToString();    //业务周期号
                }
                catch
                {
                    return new object[] { 0, 0, "医保未连接或未初始化" };
                }
                WriteLog(sysdate + "  进入门诊读卡...");
                try
                {
                    //string YBKLX = objParam[0].ToString();  //医保卡类型    0:正式卡，1：临时卡
                    //DQJBBZ = objParam[1].ToString(); //异地标志  1 市本级 2 异地医保
                      //业务周期号
                    ZXBM = "0000";
                    JYLSH = DateTime.Now.ToString("yyyyMMddHHmmss") + "-" + YLGHBH + "-" + new Random().Next(100).ToString().PadLeft(4, '0');
                    string YBKLX = "0";
                    YWBH = "2100";
                    //入参
                    StringBuilder inputData = new StringBuilder();
                    StringBuilder outputData = new StringBuilder(1024);
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

                    WriteLog(sysdate + "  门诊读卡|出参|" + outputData.ToString());

                    //int i = 0;
                    if (i == 0)
                    {
                        List<string> liSQL = new List<string>();
                        #region 出参
                        string[] sValue = outputData.ToString().Split('^')[2].Split('|');
                        ////MessageBox.Show(outputData.ToString().Split('^')[2].ToString());
                        //string sTmp = "9800956230||360602198705311049|吴文珊|2||1987-05-31|018001401102761|11||1|369900|2018|0|987.29|0.00|0.00|15278.7|0.00|0.00|0.00|0.00|0.00|0.00|1.0|中国铁路南昌局集团有限公司鹰潭车站|31||360000|||31|||";
                        //string[] sValue = sTmp.Split('|');

                        /*
                         * 00009988|3606990600001383|360621197406054219|龚建华|1|
                         * 01|1974-06-05|520705581|11|1|
                         * 0|360699|2018|0|140.04|
                         * 935.09|831.09|104.0|0.0|0.0|
                         * 0.0|104.0|0.0|0.0|0.0|
                         * 鹰潭市中医院|44|30|360699||
                         * |||||
                         * |0|0|
                         * 
                         * 9800956230||360602198705311049|吴文珊|2|
                         * |1987-05-31|018001401102761|11||
                         * 1|369900|2018|0|987.29|
                         * 0.00|0.00|15278.7|0.00|0.00|
                         * 0.00|0.00|0.00|0.00|1.0|
                         * 中国铁路南昌局集团有限公司鹰潭车站|31||360000||
                         * |31|||
                         * 
                         * 1		个人编号	VARHCAR2(20)		
                            2		单位编号	VARHCAR2(16)		
                            3		身份证号	VARHCAR2(20)		
                            4		姓名	VARHCAR2(50)		
                            5		性别	VARHCAR2(3)		二级代码
                            6		民族	VARHCAR2(3)		二级代码
                            7		出生日期	VARHCAR2(8)		YYYYMMDD
                            8		社会保障卡卡号	VARHCAR2(20)		
                            9		医疗待遇类别	VARHCAR2(3)		二级代码
                            10		人员参保状态	VARHCAR2(3)		二级代码
                            11		异地人员标志	VARHCAR2(3)		二级代码
                            12		统筹区号	VARHCAR2(6)		
                            13		年度	VARHCAR2(4)		
                            14		在院状态	VARHCAR2(3)		
                            15		帐户余额	VARHCAR2(16)		2位小数
                            16		本年医疗费累计	VARHCAR2(16)		2位小数
                            17		本年帐户支出累计	VARHCAR2(16)		2位小数
                            18		本年统筹支出累计	VARHCAR2(16)		2位小数
                            19		本年救助金支出累计	VARHCAR2(16)		2位小数
                            20		本年公务员补助基金累计	VARHCAR2(16)		2位小数
                            21		本年城镇居民门诊统筹支付累计	VARHCAR2(16)		2位小数
                            22		进入统筹费用累计	VARHCAR2(16)		2位小数
                            23		进入救助金费用累计	VARHCAR2(16)		2位小数
                            24		起付标准累计	VARHCAR2(16)		2位小数
                            25		本年住院次数	VARHCAR2(3)		
                            26		单位名称	VARHCAR2(100)		
                            27		年龄	VARHCAR2(3)		
                            28		参保单位类型	VARHCAR2(3)		二级代码
                            29		经办机构编码	VARHCAR2(16)		二级代码
                            30	二类门慢限额支出	VARCHAR2(16)		【景德镇】专用
                            31	二类门慢限额剩余	VARCHAR2(16)		【景德镇】专用
                            32	医疗待遇险种	VARCHAR2(3)		【萍乡】专用显示是否正常参保
                            33	工伤待遇险种	VARCHAR2(3)		【萍乡】专用显示是否正常参保
                            34	生育待遇险种	VARCHAR2(3)		【萍乡】专用显示是否正常参保
                            35	慢性病审批有效时间不足30提示	VARCHAR2(200)		
                            36	保险公司	VARCHAR2(3)		二级代码
                            37	民政救助标志	VARCHAR2(3)		鹰潭用
                            38	居民优抚对象	VARCHAR2(3)		鹰潭用
                         */

                        outParams_dk OP = new outParams_dk();
                        OP.Grbh = sValue[0];
                        OP.Dwbh = sValue[1];
                        OP.Sfhz = sValue[2];
                        OP.Xm = sValue[3];
                        OP.Xb = sValue[4];
                        OP.Mz = sValue[5];
                        OP.Csrq = sValue[6];
                        OP.Kh = sValue[7];
                        OP.Yldylb = sValue[8];
                        OP.Rycbzt = sValue[9];
                        OP.Ydrybz = sValue[10];
                        OP.Tcqh = sValue[11];
                        OP.Nd = sValue[12];
                        OP.Zyzt = sValue[13];
                        OP.Zhye = sValue[14];
                        OP.Bnylflj = sValue[15];
                        OP.Bnzhzclj = sValue[16];
                        OP.Bntczclj = sValue[17];
                        OP.Bnjzjzclj = sValue[18];
                        OP.Bngwybzjjlj = sValue[19];
                        OP.Bnczjmmztczflj = sValue[20];
                        OP.Jrtcfylj = sValue[21];
                        OP.Jrjzjfylj = sValue[22];
                        OP.Qfbzlj = sValue[23];
                        OP.Bnzycs = sValue[24];
                        OP.Dwmc = sValue[25];
                        OP.Nl = sValue[26];
                        OP.Cbdwlx = sValue[27];
                        OP.Jbjgbm = sValue[28];
                        OP.Elmmxezc = sValue[29];
                        OP.Elmmxesy = sValue[30];
                        OP.Jjlx = sValue[31];
                        OP.Gsbxcbbz = sValue[32];
                        OP.Sybxcbbz = sValue[33];
                        OP.Mxbmzyy = sValue[34];
                        if (OP.Ydrybz.Equals("0"))
                        {
                            OP.Bxgs = sValue[35];
                            OP.Mzjzrybz = sValue[36];
                            OP.Dbdxbz = sValue[37]; //低保对象标志 ,居民优抚对象
                        }
                        OP.Ylrylb = OP.Yldylb;

                        string[] sV1 = { "1", "2", "3" };
                        if (sV1.Contains(OP.Yldylb.Substring(0, 1)))
                            OP.Jflx = "01"; //职工医保
                        else
                            OP.Jflx = "02"; //居民医保


                        string strSql = string.Format("delete from YBICKXX where grbh='{0}'", OP.Grbh);
                        liSQL.Add(strSql);
                        #endregion

                        //异地医保卡
                        if (!OP.Tcqh.Substring(0,4).Equals("3606"))
                        {
                            DQJBBZ = "2";
                            OP.Mtbz = "0";
                            OP.Yllb = "11";
                        }
                        else
                        {
                            DQJBBZ = "1";
                            #region 医疗待遇封锁信息查询
                            object[] objParam3 = { OP.Grbh, OP.Kh, DateTime.Now.ToString("yyyyMMdd") };
                            objParam3 = YBYLDYFSXXCX(objParam3);
                            if (!objParam3[1].Equals("0"))
                                MessageBox.Show(objParam3[2].ToString());
                            #endregion

                            #region 获取慢、特病信息
                            OP.Mtbz = "0";
                            OP.Yllb = "11";
                            #region 二类慢性病、特殊病种审批
                            object[] objParam1 = { OP.Grbh, "" };
                            objParam1 = YBYLDYSPXXCX(objParam1);
                            if (objParam1[1].ToString().Equals("1"))
                            {
                                OP.Mtbz = "1";
                                OP.Yllb = "12";
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
                            #endregion
                            #endregion
                        }

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
                           OP.Dwmc + "|" + OP.Nl + "|" + OP.Cbdwlx + "|" + OP.Jbjgbm + "|" + OP.Jflx + "|" + OP.Mtbz + "|" + OP.Mtmsg + "|" + OP.Yllb + "|" + OP.Mtbzbm + "|" + OP.Mtbzmc + "|"
                           + YBKLX + "|";

                        strSql = string.Format(@"insert into YBICKXX(
                                           GRBH,DWBH,GMSFHM,XM,XB,MZ,CSRQ,KH,YLDYLB,KZT,
                                            YDRYBZ,DQBH,GRZHYE,BNYLFLJ,BNZHZCLJ,BNTCZCLJ,BNGWYJJZFLJ,BNCZJMMZTCZFLJ,JRTCFYLJ,JRMZJFYLJ,
                                            QFBZLJ,ZYCS,DWMC,SJNL,DWLX,MZJZBZ,DBDXBZ,DQJBBZ,SYSDATE)
                                            VALUES(
                                            '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}',
                                            '{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}',
                                            '{20}','{21}','{22}','{23}','{24}','{25}','{26}','{27}','{28}')",
                                                OP.Grbh, OP.Dwbh, OP.Sfhz, OP.Xm, OP.Xb, OP.Mz, OP.Csrq, OP.Kh, OP.Yldylb, OP.Rycbzt,
                                                OP.Ydrybz, OP.Tcqh, OP.Zhye, OP.Bnylflj, OP.Bnzhzclj, OP.Bntczclj, OP.Bngwybzjjlj, OP.Bnczjmmztczflj, OP.Jrtcfylj, OP.Jrjzjfylj,
                                                OP.Qfbzlj, OP.Bnzycs, OP.Dwmc, OP.Nl, OP.Cbdwlx, OP.Mzjzrybz, OP.Dbdxbz, DQJBBZ, sysdate);
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
                        WriteLog(sysdate + "  出参|门诊读卡失败|" + outputData.ToString());
                        return new object[] { 0, 0, outputData.ToString() };
                    }
                }
                catch (Exception ex)
                {
                    WriteLog(sysdate + "  系统异常|" + ex.Message);
                    return new object[] { 0, 0, "Error:" + ex.Message };
                }
            }
            else
            {
                return new object[] { 0, 0, "读卡失败" };
            }
        }
        #endregion

        #region 医保住院读卡
        public static object[] YBZYDK(object[] objParam)
        {
            DialogResult dresult = MessageBox.Show("请确认医保卡是否正常插入！", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (dresult == DialogResult.Yes)
            {
                string sysdate = GetServerDateTime();
                CZYBH = CliUtils.fLoginUser; //操作员工号
                try
                {
                    YWZQH = CliUtils.fLoginYbNo.Split('|')[0].ToString();    //业务周期号
                }
                catch
                {
                    return new object[] { 0, 0, "医保未连接或未初始化" };
                }
                WriteLog(sysdate + "  进入住院读卡...");
                try
                {
                    //string YBKLX = objParam[0].ToString();  //医保卡类型    0:正式卡，1：临时卡
                    //DQJBBZ = objParam[1].ToString(); //异地标志  1 市本级 2 异地医保
                    ZXBM = "0000";
                    JYLSH = DateTime.Now.ToString("yyyyMMddHHmmss") + "-" + YLGHBH + "-" + new Random().Next(100).ToString().PadLeft(4, '0');
                    string YBKLX = "0";

                    YWBH = "2100";
                    //入参
                    StringBuilder inputData = new StringBuilder();
                    StringBuilder outputData = new StringBuilder(1024);
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
                        List<string> liSQL = new List<string>();
                        WriteLog(sysdate + "  住院读卡成功|出参|" + outputData.ToString());
                        #region 出参
                        string[] sValue = outputData.ToString().Split('^')[2].Split('|');
                        /*
                         * 1		个人编号	VARHCAR2(20)		
                            2		单位编号	VARHCAR2(16)		
                            3		身份证号	VARHCAR2(20)		
                            4		姓名	VARHCAR2(50)		
                            5		性别	VARHCAR2(3)		二级代码
                            6		民族	VARHCAR2(3)		二级代码
                            7		出生日期	VARHCAR2(8)		YYYYMMDD
                            8		社会保障卡卡号	VARHCAR2(20)		
                            9		医疗待遇类别	VARHCAR2(3)		二级代码
                            10		人员参保状态	VARHCAR2(3)		二级代码
                            11		异地人员标志	VARHCAR2(3)		二级代码
                            12		统筹区号	VARHCAR2(6)		
                            13		年度	VARHCAR2(4)		
                            14		在院状态	VARHCAR2(3)		
                            15		帐户余额	VARHCAR2(16)		2位小数
                            16		本年医疗费累计	VARHCAR2(16)		2位小数
                            17		本年帐户支出累计	VARHCAR2(16)		2位小数
                            18		本年统筹支出累计	VARHCAR2(16)		2位小数
                            19		本年救助金支出累计	VARHCAR2(16)		2位小数
                            20		本年公务员补助基金累计	VARHCAR2(16)		2位小数
                            21		本年城镇居民门诊统筹支付累计	VARHCAR2(16)		2位小数
                            22		进入统筹费用累计	VARHCAR2(16)		2位小数
                            23		进入救助金费用累计	VARHCAR2(16)		2位小数
                            24		起付标准累计	VARHCAR2(16)		2位小数
                            25		本年住院次数	VARHCAR2(3)		
                            26		单位名称	VARHCAR2(100)		
                            27		年龄	VARHCAR2(3)		
                            28		参保单位类型	VARHCAR2(3)		二级代码
                            29		经办机构编码	VARHCAR2(16)		二级代码
                            30	二类门慢限额支出	VARCHAR2(16)		【景德镇】专用
                            31	二类门慢限额剩余	VARCHAR2(16)		【景德镇】专用
                            32	医疗待遇险种	VARCHAR2(3)		【萍乡】专用显示是否正常参保
                            33	工伤待遇险种	VARCHAR2(3)		【萍乡】专用显示是否正常参保
                            34	生育待遇险种	VARCHAR2(3)		【萍乡】专用显示是否正常参保
                            35	慢性病审批有效时间不足30提示	VARCHAR2(200)		
                            36	保险公司	VARCHAR2(3)		二级代码
                            37	民政救助标志	VARCHAR2(3)		鹰潭用
                            38	居民优抚对象	VARCHAR2(3)		鹰潭用
                         */

                        outParams_dk OP = new outParams_dk();
                        OP.Grbh = sValue[0];
                        OP.Dwbh = sValue[1];
                        OP.Sfhz = sValue[2];
                        OP.Xm = sValue[3];
                        OP.Xb = sValue[4];
                        OP.Mz = sValue[5];
                        OP.Csrq = sValue[6];
                        OP.Kh = sValue[7];
                        OP.Yldylb = sValue[8];
                        OP.Rycbzt = sValue[9];
                        OP.Ydrybz = sValue[10];
                        OP.Tcqh = sValue[11];
                        OP.Nd = sValue[12];
                        OP.Zyzt = sValue[13];
                        OP.Zhye = sValue[14];
                        OP.Bnylflj = sValue[15];
                        OP.Bnzhzclj = sValue[16];
                        OP.Bntczclj = sValue[17];
                        OP.Bnjzjzclj = sValue[18];
                        OP.Bngwybzjjlj = sValue[19];
                        OP.Bnczjmmztczflj = sValue[20];
                        OP.Jrtcfylj = sValue[21];
                        OP.Jrjzjfylj = sValue[22];
                        OP.Qfbzlj = sValue[23];
                        OP.Bnzycs = sValue[24];
                        OP.Dwmc = sValue[25];
                        OP.Nl = sValue[26];
                        OP.Cbdwlx = sValue[27];
                        OP.Jbjgbm = sValue[28];
                        OP.Elmmxezc = sValue[29];
                        OP.Elmmxesy = sValue[30];
                        OP.Jjlx = sValue[31];
                        OP.Gsbxcbbz = sValue[32];
                        OP.Sybxcbbz = sValue[33];
                        OP.Mxbmzyy = sValue[34];
                        if (OP.Ydrybz.Equals("0"))
                        {
                            OP.Bxgs = sValue[35];
                            OP.Mzjzrybz = sValue[36];
                            OP.Dbdxbz = sValue[37]; //低保对象标志 ,居民优抚对象
                        }
                        OP.Ylrylb = OP.Yldylb;

                        string[] sV1 = { "1", "2", "3" };
                        if (sV1.Contains(OP.Yldylb.Substring(0, 1)))
                            OP.Jflx = "01"; //职工医保
                        else
                            OP.Jflx = "02"; //居民医保

                        string strSql = string.Format("delete from YBICKXX where grbh='{0}'", OP.Grbh);
                        liSQL.Add(strSql);
                        #endregion

                        //异地医保卡
                        if (!OP.Tcqh.Substring(0, 4).Equals("3606"))
                        {
                            DQJBBZ = "2";
                            OP.Mtbz = "0";
                            OP.Yllb = "11";
                        }
                        else
                        {
                            DQJBBZ = "1";
                            #region 医疗待遇封锁信息查询
                            object[] objParam3 = { OP.Grbh, OP.Kh, DateTime.Now.ToString("yyyyMMdd") };
                            objParam3 = YBYLDYFSXXCX(objParam3);
                            if (!objParam3[1].Equals("0"))
                                MessageBox.Show(objParam3[2].ToString());
                            #endregion

                            #region 获取慢、特病信息
                            OP.Mtbz = "0";
                            OP.Yllb = "21";
                            #region 二类慢性病审批
                            object[] objParam1 = { OP.Grbh, "" };
                            objParam1 = YBYLDYSPXXCX(objParam1);
                            if (objParam1[1].ToString().Equals("1"))
                            {
                                OP.Mtbz = "1";
                                OP.Yllb = "12";
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
                            #endregion
                            #endregion
                        }

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
                           OP.Dwmc + "|" + OP.Nl + "|" + OP.Cbdwlx + "|" + OP.Jbjgbm + "|" + OP.Jflx + "|" + OP.Mtbz + "|" + OP.Mtmsg + "|" + OP.Yllb + "|" + OP.Mtbzbm + "|" + OP.Mtbzmc + "|"
                           + YBKLX + "|";

                        strSql = string.Format(@"insert into YBICKXX(
                                           GRBH,DWBH,GMSFHM,XM,XB,MZ,CSRQ,KH,YLDYLB,KZT,
                                            YDRYBZ,DQBH,GRZHYE,BNYLFLJ,BNZHZCLJ,BNTCZCLJ,BNGWYJJZFLJ,BNCZJMMZTCZFLJ,JRTCFYLJ,JRMZJFYLJ,
                                            QFBZLJ,ZYCS,DWMC,SJNL,DWLX,MZJZBZ,DBDXBZ,DQJBBZ,SYSDATE)
                                            VALUES(
                                            '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}',
                                            '{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}',
                                            '{20}','{21}','{22}','{23}','{24}','{25}','{26}','{27}','{28}')",
                                            OP.Grbh, OP.Dwbh, OP.Sfhz, OP.Xm, OP.Xb, OP.Mz, OP.Csrq, OP.Kh, OP.Yldylb, OP.Rycbzt,
                                            OP.Ydrybz, OP.Tcqh, OP.Zhye, OP.Bnylflj, OP.Bnzhzclj, OP.Bntczclj, OP.Bngwybzjjlj, OP.Bnczjmmztczflj, OP.Jrtcfylj, OP.Jrjzjfylj,
                                            OP.Qfbzlj, OP.Bnzycs, OP.Dwmc, OP.Nl, OP.Cbdwlx, OP.Mzjzrybz, OP.Dbdxbz, DQJBBZ, sysdate);
                        liSQL.Add(strSql);
                        object[] obj = liSQL.ToArray();
                        obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);
                        if (obj[1].ToString().Equals("1"))
                        {

                            WriteLog(sysdate + "  住院读卡成功|" + outParams);
                            Frm_YBICKXX ick = new Frm_YBICKXX(outputData.ToString().Split('^')[2]);
                            ick.ShowDialog();
                            return new object[] { 0, 1, outParams };
                        }
                        else
                        {
                            WriteLog(sysdate + "  住院读卡成功|保存本地数据失败|" + obj[2].ToString());
                            return new object[] { 0, 0, "住院读卡成功|" + obj[2].ToString() };
                        }
                    }
                    else
                    {
                        WriteLog(sysdate + "  出参|住院读卡失败|" + outputData.ToString());
                        return new object[] { 0, 0, outputData.ToString() };
                    }
                }
                catch (Exception ex)
                {
                    WriteLog(sysdate + "  系统异常|" + ex.Message);
                    return new object[] { 0, 0, "Error:" + ex.Message };
                }
            }
            else
            {
                return new object[] { 0, 0, "读卡失败" };
            }
        }
        #region 22
//        public static object[] YBZYDK(object[] objParam)
//        {
//            return CallWithTimeout(YBDK, objParam, 10000);
//        }

//        static void YBDK(object[] objParam)
//        {
//            string sysdate = GetServerDateTime();
//            WriteLog(sysdate + "  进入住院读卡...");
//            try
//            {
//                //string YBKLX = objParam[0].ToString();  //医保卡类型    0:正式卡，1：临时卡
//                //DQJBBZ = objParam[1].ToString(); //异地标志  1 市本级 2 异地医保
//                MessageBox.Show(YWZQH);
//                CZYBH = CliUtils.fLoginUser; //操作员工号
//                YWZQH = CliUtils.fLoginYbNo.Split('|')[0].ToString();    //业务周期号
//                JYLSH = DateTime.Now.ToString("yyyyMMddHHmmss") + "-" + YLGHBH + "-" + new Random().Next(100).ToString().PadLeft(4, '0');
//                string YBKLX = "0";

//                YWBH = "2100";
//                //入参
//                StringBuilder inputData = new StringBuilder();
//                StringBuilder outputData = new StringBuilder(1024);
//                inputData.Append(YWBH + "^");
//                inputData.Append(YLGHBH + "^");
//                inputData.Append(CZYBH + "^");
//                inputData.Append(YWZQH + "^");
//                inputData.Append(JYLSH + "^");
//                inputData.Append(ZXBM + "^");
//                inputData.Append("^");
//                inputData.Append(LJBZ + "^");
//                WriteLog(sysdate + "  入参|" + inputData.ToString());
//                //进入本市级读卡
//                int i = BUSINESS_HANDLE(inputData, outputData);

//                MessageBox.Show(i.ToString());
//                if (i == 0)
//                {
//                    List<string> liSQL = new List<string>();
//                    WriteLog(sysdate + "  住院读卡成功|出参|" + outputData.ToString());
//                    #region 出参
//                    string[] sValue = outputData.ToString().Split('^')[2].Split('|');
//                    /*
//                     * 1		个人编号	VARHCAR2(20)		
//                        2		单位编号	VARHCAR2(16)		
//                        3		身份证号	VARHCAR2(20)		
//                        4		姓名	VARHCAR2(50)		
//                        5		性别	VARHCAR2(3)		二级代码
//                        6		民族	VARHCAR2(3)		二级代码
//                        7		出生日期	VARHCAR2(8)		YYYYMMDD
//                        8		社会保障卡卡号	VARHCAR2(20)		
//                        9		医疗待遇类别	VARHCAR2(3)		二级代码
//                        10		人员参保状态	VARHCAR2(3)		二级代码
//                        11		异地人员标志	VARHCAR2(3)		二级代码
//                        12		统筹区号	VARHCAR2(6)		
//                        13		年度	VARHCAR2(4)		
//                        14		在院状态	VARHCAR2(3)		
//                        15		帐户余额	VARHCAR2(16)		2位小数
//                        16		本年医疗费累计	VARHCAR2(16)		2位小数
//                        17		本年帐户支出累计	VARHCAR2(16)		2位小数
//                        18		本年统筹支出累计	VARHCAR2(16)		2位小数
//                        19		本年救助金支出累计	VARHCAR2(16)		2位小数
//                        20		本年公务员补助基金累计	VARHCAR2(16)		2位小数
//                        21		本年城镇居民门诊统筹支付累计	VARHCAR2(16)		2位小数
//                        22		进入统筹费用累计	VARHCAR2(16)		2位小数
//                        23		进入救助金费用累计	VARHCAR2(16)		2位小数
//                        24		起付标准累计	VARHCAR2(16)		2位小数
//                        25		本年住院次数	VARHCAR2(3)		
//                        26		单位名称	VARHCAR2(100)		
//                        27		年龄	VARHCAR2(3)		
//                        28		参保单位类型	VARHCAR2(3)		二级代码
//                        29		经办机构编码	VARHCAR2(16)		二级代码
//                        30	二类门慢限额支出	VARCHAR2(16)		【景德镇】专用
//                        31	二类门慢限额剩余	VARCHAR2(16)		【景德镇】专用
//                        32	医疗待遇险种	VARCHAR2(3)		【萍乡】专用显示是否正常参保
//                        33	工伤待遇险种	VARCHAR2(3)		【萍乡】专用显示是否正常参保
//                        34	生育待遇险种	VARCHAR2(3)		【萍乡】专用显示是否正常参保
//                        35	慢性病审批有效时间不足30提示	VARCHAR2(200)		
//                        36	保险公司	VARCHAR2(3)		二级代码
//                        37	民政救助标志	VARCHAR2(3)		鹰潭用
//                        38	居民优抚对象	VARCHAR2(3)		鹰潭用
//                     */

//                    outParams_dk OP = new outParams_dk();
//                    OP.Grbh = sValue[0];
//                    OP.Dwbh = sValue[1];
//                    OP.Sfhz = sValue[2];
//                    OP.Xm = sValue[3];
//                    OP.Xb = sValue[4];
//                    OP.Mz = sValue[5];
//                    OP.Csrq = sValue[6];
//                    OP.Kh = sValue[7];
//                    OP.Yldylb = sValue[8];
//                    OP.Rycbzt = sValue[9];
//                    OP.Ydrybz = sValue[10];
//                    OP.Tcqh = sValue[11];
//                    OP.Nd = sValue[12];
//                    OP.Zyzt = sValue[13];
//                    OP.Zhye = sValue[14];
//                    OP.Bnylflj = sValue[15];
//                    OP.Bnzhzclj = sValue[16];
//                    OP.Bntczclj = sValue[17];
//                    OP.Bnjzjzclj = sValue[18];
//                    OP.Bngwybzjjlj = sValue[19];
//                    OP.Bnczjmmztczflj = sValue[20];
//                    OP.Jrtcfylj = sValue[21];
//                    OP.Jrjzjfylj = sValue[22];
//                    OP.Qfbzlj = sValue[23];
//                    OP.Bnzycs = sValue[24];
//                    OP.Dwmc = sValue[25];
//                    OP.Nl = sValue[26];
//                    OP.Cbdwlx = sValue[27];
//                    OP.Jbjgbm = sValue[28];
//                    OP.Elmmxezc = sValue[29];
//                    OP.Elmmxesy = sValue[30];
//                    OP.Jjlx = sValue[31];
//                    OP.Gsbxcbbz = sValue[32];
//                    OP.Sybxcbbz = sValue[33];
//                    OP.Mxbmzyy = sValue[34];
//                    if (OP.Ydrybz.Equals("0"))
//                    {
//                        OP.Bxgs = sValue[35];
//                        OP.Mzjzrybz = sValue[36];
//                        OP.Dbdxbz = sValue[37]; //低保对象标志 ,居民优抚对象
//                    }
//                    OP.Ylrylb = OP.Yldylb;

//                    string[] sV1 = { "1", "2", "3" };
//                    if (sV1.Contains(OP.Yldylb.Substring(0, 1)))
//                        OP.Jflx = "0202"; //职工医保
//                    else
//                        OP.Jflx = "0203"; //居民医保

//                    if (OP.Ydrybz.Equals("1"))
//                        DQJBBZ = "2";
//                    else
//                        DQJBBZ = "1";

//                    string strSql = string.Format("delete from YBICKXX where grbh='{0}'", OP.Grbh);
//                    liSQL.Add(strSql);
//                    #endregion

//                    //异地医保卡
//                    if (OP.Ydrybz.Equals("1"))
//                    {
//                        DQJBBZ = "2";
//                        OP.Mtbz = "0";
//                        OP.Yllb = "11";
//                    }
//                    else
//                    {
//                        DQJBBZ = "1";
//                        #region 医疗待遇封锁信息查询
//                        object[] objParam3 = { OP.Grbh, OP.Kh };
//                        objParam3 = YBYLDYFSXXCX(objParam3);
//                        if (!objParam3[1].Equals("0"))
//                            DKXX = "0&" + objParam3[2].ToString();
//                        #endregion

//                        #region 获取慢、特病信息
//                        OP.Mtbz = "0";
//                        OP.Yllb = "21";
//                        #region 二类慢性病审批
//                        object[] objParam1 = { OP.Grbh, "" };
//                        objParam1 = YBYLDYSPXXCX(objParam1);
//                        if (objParam1[1].ToString().Equals("1"))
//                        {
//                            OP.Mtbz = "1";
//                            OP.Yllb = "12";
//                            strSql = string.Format("delete from ybmxbdj where bxh='{0}'", OP.Grbh);
//                            liSQL.Add(strSql);
//                            string[] sV = objParam1[2].ToString().Split('$');
//                            foreach (string s in sV)
//                            {
//                                string[] sV_temp = s.Split('|');
//                                OP.Mtbzbm = sV_temp[0];  //门特病种编码
//                                OP.Mtbzmc = sV_temp[1];  //门特病种名称
//                                strSql = string.Format(@"select * from ybbzmrdr where dm='{0}'", OP.Mtbzbm);
//                                DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
//                                if (ds.Tables[0].Rows.Count > 0)
//                                {
//                                    OP.Bzlb = ds.Tables[0].Rows[0]["bzlb"].ToString();
//                                    OP.Yllb = ds.Tables[0].Rows[0]["yllb"].ToString();
//                                }
//                                //OP.Mtmsg += OP.Bzlb + "\t";
//                                OP.Mtmsg += OP.Mtbzbm + "\t" + OP.Mtbzmc + "\r\n";

//                                strSql = string.Format(@"insert into  ybmxbdj(BXH,KH,XM,MMBZBM,MMBZMC,YLLB,BZLB) values('{0}','{1}','{2}','{3}','{4}','{5}','{6}') ",
//                                                        OP.Grbh, OP.Kh, OP.Xm, OP.Mtbzbm, OP.Mtbzmc, OP.Yllb, OP.Bzlb);
//                                liSQL.Add(strSql);
//                            }
//                        }
//                        #endregion
//                        #endregion
//                    }

//                    /*
//                     * 个人编号|单位编号|身份证号|姓名|性别|
//                     * 民族|出生日期|社会保障卡卡号|医疗待遇类别|人员参保状态|
//                     * 异地人员标志|统筹区号|年度|在院状态|帐户余额|
//                     * 本年医疗费累计|本年帐户支出累计|本年统筹支出累计|本年救助金支出累计|本年公务员补助基金累计|
//                     * 本年城镇居民门诊统筹支付累计|进入统筹费用累计|进入救助金费用累计|起付标准累计|本年住院次数|
//                     * 单位名称|年龄|参保单位类型|经办机构编码|缴费类型|
//                     * 医保门慢、特资质|医保门慢、特病种说明|医疗类别代码|慢、特病编码|慢、特病名称|
//                     * 医保卡类型
//                     */
//                    string outParams = OP.Grbh + "|" + OP.Dwbh + "|" + OP.Sfhz + "|" + OP.Xm + "|" + OP.Xb + "|" +
//                       OP.Mz + "|" + OP.Csrq + "|" + OP.Kh + "|" + OP.Yldylb + "|" + OP.Rycbzt + "|" +
//                       OP.Ydrybz + "|" + OP.Tcqh + "|" + OP.Nd + "|" + OP.Zyzt + "|" + OP.Zhye + "|" +
//                       OP.Bnylflj + "|" + OP.Bnzhzclj + "|" + OP.Bntczclj + "|" + OP.Bndbyljjzflj + "|" + OP.Bngwybzjjlj + "|" +
//                       OP.Bnczjmmztczflj + "|" + OP.Jrtcfylj + "|" + OP.Jrdbfylj + "|" + OP.Qfbzlj + "|" + OP.Bnzycs + "|" +
//                       OP.Dwmc + "|" + OP.Nl + "|" + OP.Cbdwlx + "|" + OP.Jbjgbm + "|" + OP.Jflx + "|" + OP.Mtbz + "|" + OP.Mtmsg + "|" + OP.Yllb + "|" + OP.Mtbzbm + "|" + OP.Mtbzmc + "|"
//                       + YBKLX + "|";

//                    strSql = string.Format(@"insert into YBICKXX(
//                                           GRBH,DWBH,GMSFHM,XM,XB,MZ,CSRQ,KH,YLDYLB,KZT,
//                                            YDRYBZ,DQBH,GRZHYE,BNYLFLJ,BNZHZCLJ,BNTCZCLJ,BNGWYJJZFLJ,BNCZJMMZTCZFLJ,JRTCFYLJ,JRMZJFYLJ,
//                                            QFBZLJ,ZYCS,DWMC,SJNL,DWLX,MZJZBZ,DBDXBZ,DQJBBZ,SYSDATE)
//                                            VALUES(
//                                            '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}',
//                                            '{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}',
//                                            '{20}','{21}','{22}','{23}','{24}','{25}','{26}','{27}','{28}')",
//                                            OP.Grbh, OP.Dwbh, OP.Sfhz, OP.Xm, OP.Xb, OP.Mz, OP.Csrq, OP.Kh, OP.Yldylb, OP.Rycbzt,
//                                            OP.Ydrybz, OP.Tcqh, OP.Zhye, OP.Bnylflj, OP.Bnzhzclj, OP.Bntczclj, OP.Bngwybzjjlj, OP.Bnczjmmztczflj, OP.Jrtcfylj, OP.Jrjzjfylj,
//                                            OP.Qfbzlj, OP.Bnzycs, OP.Dwmc, OP.Nl, OP.Cbdwlx, OP.Mzjzrybz, OP.Dbdxbz, DQJBBZ, sysdate);
//                    liSQL.Add(strSql);
//                    object[] obj = liSQL.ToArray();
//                    obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);
//                    if (obj[1].ToString().Equals("1"))
//                    {

//                        WriteLog(sysdate + "  住院读卡成功|" + outParams);
//                        Frm_YBICKXX ick = new Frm_YBICKXX(outputData.ToString().Split('^')[2]);
//                        ick.ShowDialog();
//                        DKXX = "1&" + outParams;
//                    }
//                    else
//                    {
//                        WriteLog(sysdate + "  住院读卡成功|保存本地数据失败|" + obj[2].ToString());
//                        DKXX = "0&" + obj[2].ToString();
//                    }
//                }
//                else
//                {
//                    WriteLog(sysdate + "  出参|住院读卡失败|" + outputData.ToString());
//                    DKXX = "0&" + outputData.ToString();
//                }
//            }
//            catch (Exception ex)
//            {
//                WriteLog(sysdate + "  系统异常|" + ex.Message);
//                DKXX = "0&" + ex.Message;
//            }
//        }
        #endregion
        #endregion

        #region 门诊登记
        public static object[] YBMZDJ(object[] objParam)
        {
            string sysdate = GetServerDateTime();
            WriteLog(sysdate + "  进入门诊登记...");
            try
            {
                YWZQH = CliUtils.fLoginYbNo.Split('|')[0].ToString();    //业务周期号
            }
            catch
            {
                return new object[] { 0, 0, "医保未连接或未初始化" };
            }

            try
            {
                #region his参数
                CZYBH = CliUtils.fLoginUser;    //操作员工号
                ZXBM = "0000";
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
                string ybjzlsh_snyd = "";
                #endregion

                if (string.IsNullOrEmpty(jzlsh))
                    return new object[] { 0, 0, "就诊流水号不能为空" };
                if (string.IsNullOrEmpty(yllb))
                    return new object[] { 0, 0, "医疗类别不能为空" };
                if (string.IsNullOrEmpty(ickxx))
                    return new object[] { 0, 0, "读卡信息不能为空" };
                if (string.IsNullOrEmpty(ghsj))
                    return new object[] { 0, 0, "登记时间不能为空" };

                if (yllb == "13" || yllb == "12")
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

                if (!ssqh.Substring(0, 4).Equals("3606"))
                {
                    ZXBM = ssqh;
                    DQJBBZ = "2";
                    WriteLog(sysdate + "  进入门诊登记|异地医保...");
                }
                else
                {
                    ZXBM = "0000";
                    DQJBBZ = "1";
                    WriteLog(sysdate + "  进入门诊登记|市本级...");
                }
                #endregion

                JYLSH = ghdjsj + "-" + YLGHBH + "-" + new Random().Next(100).ToString().PadLeft(4, '0');
                string strSql = string.Format(@"select m1ghno,m1name,m1ksno,b2ksnm,m1empn,b1name,m1amnt from mz01t  a
                                                left join bz01h b on a.m1empn=b.b1empn
                                                left join bz02h c on a.m1ksno=c.b2ksno
                                                where m1ghno='{0}'
                                                union all
												select m1ghno,m1name,m1ksno,b2ksnm,m1empn,b1name,m1amnt from mz01h  a
                                                left join bz01h b on a.m1empn=b.b1empn
                                                left join bz02h c on a.m1ksno=c.b2ksno
                                                where m1ghno='{0}'", jzlsh);

                DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                if (ds.Tables[0].Rows.Count == 0)
                {
                    WriteLog(sysdate + "  无挂号费用明细");
                    return new object[] { 0, 0, "无挂号费用明细" };
                }

                string jegh = "0";
                string jezc = "0";

                ysdm = ds.Tables[0].Rows[0]["m1empn"].ToString();
                ysxm = ds.Tables[0].Rows[0]["b1name"].ToString();
                ksbh = ds.Tables[0].Rows[0]["m1ksno"].ToString();
                ksmc = ds.Tables[0].Rows[0]["b2ksnm"].ToString();
                hzxm = ds.Tables[0].Rows[0]["m1name"].ToString();
                string ybjzlsh = "";
                if (string.IsNullOrEmpty(bzbm))
                {
                    ybjzlsh = "MZ" + yllb + jzlsh;
                }
                else
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
                /*
                 * 
                 * MZ11000002111|11|20180917080142|||
                 * M007|急诊门诊||||
                 * 管理员|00009988||||
                 * ||龚建华|520705581|0.00|0.00|||||||||||||
                 * 1		就诊流水号	VARCHAR2(20)	NOT NULL	同一家医院的就诊流水号是唯一的
                    2		医疗类别	VARCHAR2(3)	NOT NULL	二级代码
                    3		挂号/登记时间	VARCHAR2(14)	NOT NULL 	YYYYMMDDHH24MISS
                    4		病种编码	VARCHAR2(20)		门诊大病、慢病、住院不能为空。
                    5		病种名称	VARCHAR2(50)		有病种编码时，对应的病种名称不能为空。
                    6		科室编号	VARCHAR2(20)	NOT NULL	
                    7		科室名称	VARCHAR2(50)	NOT NULL	
                    8		床位号	VARCHAR2(20)		住院类需要传入床位号，如果入院时没有分配床位号，可以通过调用住院信息修改交易录入床位号
                    9		医师代码	VARCHAR2(20)	NOT NULL	（住院类的必须上传）入院登记不判断医师处方权。
                    10		医师姓名	VARCHAR2(50)	NOT NULL	（住院类的必须上传）入院登记不判断医师处方权。
                    11		经办人	VARCHAR2(50)	NOT NULL	医疗机构操作员姓名
                    12		个人编号	VARCHAR2(16)	NOT NULL	如果不读卡，先调用查询人员信息的交易，从人员信息中获得个人电脑编号
                    13		病区	VARCHAR2(20)		
                    14		转诊医院编号	VARCHAR2(50)		以备转外的费用放到医院报销时使用
                    15		转诊医院名称	VARCHAR2(50)		以备转外的费用放到医院报销时使用
                    16		备注	VARCHAR2(100)		
                    17		特殊费用标志	VARCHAR2(10)		二级代码，供外伤报销、转外报销等情况使用，若同时存在多种情况，则多个代码间用逗号分隔
                    18		姓名	VARCHAR2(20)	NOT NULL	异地结算
                    19		卡号	VARCHAR2(18)	NOT NULL	异地结算
                    20		挂号费	VARHCAR2(16)		（普通门诊时挂号费）本地必须输入
                    21		一般诊疗	VARHCAR2(16)		（普通门诊时挂号费）本地必须输入
                    22		门慢病种编码1	VARHCAR2(20)		【萍乡/景德镇】门慢需求要传多病种，其他地市不同传
                    23		门慢病种名称1	VARHCAR2(100)		【萍乡/景德镇】同上
                    24		门慢病种编码2	VARHCAR2(20)		【萍乡/景德镇】同上
                    25		门慢病种名称2	VARHCAR2(100)		【萍乡/景德镇】同上
                    26		门慢病种编码3	VARHCAR2(20)		【萍乡/景德镇】同上
                    27		门慢病种名称3	VARHCAR2(100)		【萍乡/景德镇】同上
                    28		门慢病种编码4	VARHCAR2(20)		【萍乡/景德镇】同上
                    29		门慢病种名称4	VARHCAR2(100)		【萍乡/景德镇】同上
                    30		主要病情描述	VARHCAR2(1000)		指医师根据患者描述对症状的详细描述。(全国异地参数)
                    31		病历号	VARHCAR2(20)		(全国异地参数)
                    32		急诊标志	VARHCAR2(3)		用于记录急诊就医。(全国异地参数)
                    33		外伤标识	VARHCAR2(3)		用于记录外伤就医。(全国异地参数)

                 */
                inputParam.Append(ybjzlsh + "|");   //门诊/住院流水号
                inputParam.Append(yllb + "|");      //医疗类别
                inputParam.Append(ghdjsj + "|");    //挂号/登记时间
                inputParam.Append(bzbm + "|");      //病种编码
                inputParam.Append(bzmc + "|");      //病种名称
                inputParam.Append(ksbh + "|");      //科室编码
                inputParam.Append(ksmc + "|");      //科室名称
                inputParam.Append("|");             //床位号
                inputParam.Append(ysdm + "|");      //医生代码
                inputParam.Append(ysxm + "|");      //医生姓名
                inputParam.Append(jbr + "|");
                inputParam.Append(grbh + "|");
                inputParam.Append("|");
                inputParam.Append("|");
                inputParam.Append("|");
                inputParam.Append("|");
                inputParam.Append("|");
                inputParam.Append(xm + "|");      //姓名
                inputParam.Append(kh + "|");      //卡号
                inputParam.Append(jegh + "|");      //挂号费
                inputParam.Append(jezc + "|");      //一般诊疗
                //inputParam.Append("|");
                //inputParam.Append("|");
                //inputParam.Append("|");
                //inputParam.Append("|");
                //inputParam.Append("|");
                //inputParam.Append("|");
                //inputParam.Append("|");
                //inputParam.Append("|");
                //inputParam.Append("|"); //主要病情描述
                //inputParam.Append("|");
                //inputParam.Append("|");
                //inputParam.Append("|");//外伤标识
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

                WriteLog(sysdate + "  门诊登记|入参|" + inputData.ToString());
                StringBuilder outputData = new StringBuilder(10240);
                int i = BUSINESS_HANDLE(inputData, outputData);
                WriteLog(sysdate + "   门诊登记|出参|" + outputData.ToString());
                if (i == 0)
                {
                    List<string> liSQL = new List<string>();
                    #region 异地返回就诊流水号
                    if (!ssqh.Substring(0, 4).Equals("3606"))
                    {
                        string[] rParam = outputData.ToString().Split('^')[2].Split(';');
                        string[] fParam = rParam[0].Split('|');
                        ybjzlsh_snyd = fParam[0];

                        string mmbzbm = "";
                        string mmbzmc = "";
                        string mmbzlb = "";
                        string mmyllb = "";
                        string Mtmsg = "";

                        /*
                         第一位：登记的交易流水号；第二位开始：病种编码1|病种名称1;病种编码2|病种名称2;(两个以上病种用“;”隔开) 
                         例如：20160526360299335819|SCD0001|恶性肿瘤;SCD0011|慢性肝炎;SCD0010|糖尿病;SCD0005|尿毒症SCD0003|再生障碍性贫血;
                         */
                        if (fParam.Length > 2)
                        {
                            strSql = string.Format("delete from ybmxbdj where bxh='{0}'", grbh);
                            liSQL.Add(strSql);
                            mmbzbm = fParam[1];
                            mmbzmc = fParam[2];
                            strSql = string.Format(@"select * from ybbzmrdr where dm='{0}'", mmbzbm);
                            ds.Tables[0].Clear();
                            ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                            if (ds.Tables[0].Rows.Count > 0)
                            {
                                mmbzlb = ds.Tables[0].Rows[0]["bzlb"].ToString();
                                mmyllb = ds.Tables[0].Rows[0]["yllb"].ToString();
                            }

                            strSql = string.Format(@"insert into  ybmxbdj(BXH,KH,XM,MMBZBM,MMBZMC,YLLB,BZLB) values('{0}','{1}','{2}','{3}','{4}','{5}','{6}') ",
                                                    grbh, kh, xm, mmbzbm, mmbzmc, mmyllb, mmbzlb);
                            liSQL.Add(strSql);
                        }
                        if (rParam.Length > 1)
                        {
                            for (int jj = 1; jj < rParam.Length; jj++)
                            {
                                string[] TParam = rParam[jj].Split('|');
                                mmbzbm = fParam[0];
                                mmbzmc = fParam[1];
                                strSql = string.Format(@"select * from ybbzmrdr where dm='{0}'", mmbzbm);
                                ds.Tables[0].Clear();
                                ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                                if (ds.Tables[0].Rows.Count > 0)
                                {
                                    mmbzlb = ds.Tables[0].Rows[0]["bzlb"].ToString();
                                    mmyllb = ds.Tables[0].Rows[0]["yllb"].ToString();
                                }

                                strSql = string.Format(@"insert into  ybmxbdj(BXH,KH,XM,MMBZBM,MMBZMC,YLLB,BZLB) values('{0}','{1}','{2}','{3}','{4}','{5}','{6}') ",
                                                        grbh, kh, xm, mmbzbm, mmbzmc, mmyllb, mmbzlb);
                                liSQL.Add(strSql);
                            }
                        }

                        if (!string.IsNullOrEmpty(Mtmsg) && yllb.Equals("11"))
                        {
                            DialogResult result = MessageBox.Show("注:慢病患者!\r\n" + Mtmsg + "当前医疗类别[普通门诊],与慢病类别不符\r\n是否继续当前挂号?\r\n点击[是] 继续操作  点击[否] 取销当前操作", "门诊医保登记", MessageBoxButtons.YesNo, MessageBoxIcon.Information, MessageBoxDefaultButton.Button2);
                            if (result == DialogResult.No)
                            {
                                WriteLog(sysdate + "   进入门诊登记(异地医保)撤销操作...");
                                //门诊登记撤销
                                object[] objParam1 = { ybjzlsh, jbr, yllb, grbh, xm, kh, ssqh, ybjzlsh_snyd, DQJBBZ };
                                return NYBMZDJCX(objParam1);
                            }
                        }
                    }
                    #endregion

                    strSql = string.Format(@"insert into ybmzzydjdr(
                                            jzlsh,jylsh,ybjzlsh,yllb,ghdjsj,bzbm,bzmc,ksbh,ksmc,ysdm,
                                            ysxm,ghf,jbr,xm,grbh,kh,yldylb,xb,tcqh,zhye,
                                            jzbz,ydrybz,dqjbbz,sysdate,ybklx,ybzl,ybjzlsh_snyd) 
                                            values(
                                            '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}',
                                            '{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}',
                                            '{20}','{21}','{22}','{23}','{24}','{25}','{26}')"
                                            , jzlsh, JYLSH, ybjzlsh, yllb, ghdjsj, bzbm, bzmc, ksbh, ksmc, ysdm,
                                            ysxm, jegh, jbr, xm, grbh, kh, yldylb, xb, ssqh, zhye,
                                            "m", ydrybz, DQJBBZ, sysdate, ybklx, jezc, ybjzlsh_snyd);
                    liSQL.Add(strSql);
                    object[] obj = liSQL.ToArray();

                    obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);
                    if (obj[1].ToString() == "1")
                    {
                        WriteLog(sysdate + "   门诊登记成功|本地数据操作成功|" + outputData.ToString());
                        return new object[] { 0, 1, "门诊登记挂号成功" };
                    }
                    else
                    {
                        WriteLog(sysdate + "   门诊登记成功|本地数据操作失败|" + obj[2].ToString());
                        //门诊登记撤销
                        object[] objParam1 = { ybjzlsh, jbr, yllb, grbh, xm, kh, ssqh, ybjzlsh_snyd, DQJBBZ };
                        NYBMZDJCX(objParam1);
                        return new object[] { 0, 0, "门诊登记成功|本地数据操作失败|" + obj[2].ToString() };
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
                WriteLog(sysdate + "   门诊登记|接口异常|" + error.Message);
                return new object[] { 0, 2, error.Message };
            }
        }
        #endregion

        #region 门诊登记撤销 
        public static object[] YBMZDJCX(object[] objParam)
        {
            string sysdate = GetServerDateTime();
            WriteLog(sysdate + "  进入门诊登记撤销...");

            CZYBH = CliUtils.fLoginUser;    //操作员工号
            try
            {
                YWZQH = CliUtils.fLoginYbNo.Split('|')[0].ToString();    //业务周期号
            }
            catch
            {
                return new object[] { 0, 0, "医保未连接或未初始化" };
            }
            ZXBM = "0000";
            string jbr = CliUtils.fUserName; //操作员姓名
            string jzlsh = objParam[0].ToString();  //就诊流水号
            string yllb = objParam[1].ToString();   //医疗类别

            JYLSH = DateTime.Now.ToString("yyyyMMddHHmmss") + "-" + YLGHBH + "-" + new Random().Next(100).ToString().PadLeft(4, '0');
            try
            {
                string strSql = string.Format(@"select a.* from ybmzzydjdr a where a.jzlsh='{0}' and a.cxbz=1 
                                                and not exists(select 1 from ybfyjsdr b where a.jzlsh=b.jzlsh and a.ybjzlsh=b.ybjzlsh and b.cxbz=1) ", jzlsh, yllb);
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
                    /*
                    1	就诊流水号	VARCHAR2(20)	NOT NULL	唯一
                    2	经办人	VARCHAR2(20)	NOT NULL	医疗机构操作员姓名
                    3	个人编号	VARCHAR2(10)	NOT NULL	异地结算
                    4	姓名	VARCHAR2(20)	NOT NULL	异地结算
                    5	卡号	VARCHAR2(18)	NOT NULL	异地结算
                     */

                    if (DQJBBZ.Equals("1"))
                    {
                        //撤销未结算费用明细
                        object[] objParamcf = { ybjzlsh, "", grbh, xm, kh };
                        YBMZCFMXSCCX(objParamcf);
                        inputParam.Append(ybjzlsh + "|");
                    }
                    else //省内异地
                    {
                        ZXBM = dqbh;
                        inputParam.Append(ybjzlsh_snyd + "|");
                    }
                    inputParam.Append(jbr + "|");
                    inputParam.Append(grbh + "|");
                    inputParam.Append(xm + "|");
                    inputParam.Append(kh + "|");

                    YWBH = "2240";
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
            string sysdate = GetServerDateTime();
            WriteLog(sysdate + "  进入门诊登记收费...");
            try
            {
                YWZQH = CliUtils.fLoginYbNo.Split('|')[0].ToString();    //业务周期号
            }
            catch
            {
                return new object[] { 0, 0, "医保未连接或初始化失败" };
            }

            try
            {
                #region his参数
                CZYBH = CliUtils.fLoginUser;    //操作员工号
                string jbr = CliUtils.fUserName;   // 经办人姓名 
                ZXBM = "0000";

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
                string ydrybz = kxx.Split('|')[10].ToString();  //异地人员标志
                string ssqh = kxx.Split('|')[11].ToString();  //所属区号
                string zhye = kxx.Split('|')[14].ToString(); //帐户余额
                string ghdjsj = "";
                string djh = ""; //发票号
                string sslxdm = "";                         //手术类别
                string grbh = "";        // 个人编号   
                string xm = "";            // 姓名
                string kh = "";            // 卡号
                string ybjzlsh = "";
                string ybjzlsh_snyd = "";
                string ksbm = "";
                string ksmc = "";
                #endregion

                #region 入参判断
                if (string.IsNullOrEmpty(jzlsh))
                    return new object[] { 0, 0, "就诊流水号不能为空" };
                if (string.IsNullOrEmpty(yllb))
                    return new object[] { 0, 0, "医疗类别不能为空" };

                if (yllb == "13" || yllb == "12")
                {
                    if (string.IsNullOrEmpty(bzbm) || string.IsNullOrEmpty(bzmc))
                        return new object[] { 0, 0, "选择门诊慢性病时，病种必须输入" };
                }
                #endregion

                ghdjsj = Convert.ToDateTime(jsrq).ToString("yyyyMMddHHmmss");
                if (string.IsNullOrEmpty(bzbm))
                    ybjzlsh = "MZ" + yllb + jzlsh;
                else
                    ybjzlsh = "MZ" + yllb + jzlsh + bzbm;

                //医院交易流水号
                JYLSH = ghdjsj + "-" + YLGHBH + "-" + new Random().Next(100).ToString().PadLeft(4, '0');
                List<string> liSQL = new List<string>();

                #region 医保登记(挂号)
                string strSql = string.Format(@"select * from ybmzzydjdr where ybjzlsh='{0}' and cxbz=1", ybjzlsh);
                DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                if (ds.Tables[0].Rows.Count == 0)
                {
                    object[] objParam1 = { jzlsh, yllb, bzbm, bzmc, kxx, jsrq, "", "" };
                    objParam1 = YBMZDJ(objParam1);
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
                ybjzlsh_snyd = dr["ybjzlsh_snyd"].ToString();
                ksbm = dr["ksbh"].ToString();
                ksmc = dr["ksmc"].ToString();
                DQJBBZ = dr["DQJBBZ"].ToString();
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
                #endregion

                #region 出参初始化
                outParams_js js = new outParams_js();
                js.Ylfze = "0.00";         //医疗费总费用
                js.Zbxje = "0.00";       //总报销金额
                js.Tcjjzf = "0.00";        //统筹支出
                js.Dejjzf = "0.00";       //大病支出
                js.Zhzf = "0.00";         //本次帐户支付
                js.Xjzf = "0.00";         //个人现金支付
                js.Gwybzjjzf = "0.00";     //公务员补助支付金额
                js.Qybcylbxjjzf = "0.00";  //企业补充支付金额
                js.Zffy = "0.00";         //自费费用
                js.Dwfdfy = "0.00";
                js.Yyfdfy = "0.00";
                js.Mzjzfy = "0.00";     //民政救助费用
                js.Cxjfy = "0.00";
                js.Ylzlfy = "0.00";
                js.Blzlfy = "0.00";
                js.Fhjjylfy = "0.00";
                js.Qfbzfy = "0.00";
                js.Zzzyzffy = "0.00";
                js.Jrtcfy = "0.00";
                js.Tcfdzffy = "0.00";
                js.Ctcfdxfy = "0.00";
                js.Jrdebxfy = "0.00";
                js.Defdzffy = "0.00";
                js.Cdefdxfy = "0.00";
                js.Rgqgzffy = "0.00";
                js.Bcjsqzhye = "0.00";
                js.Bntczflj = "0.00";
                js.Bndezflj = "0.00";
                js.Bnczjmmztczflj = "0.00";
                js.Bngwybzzflj = "0.00";
                js.Bnzhzflj = "0.00";
                js.Bnzycslj = "0.00";
                js.Qtjjzf = "0.00";
                js.Scryylbzjj = "0.00";
                js.Ecbcje = "0.00";
                js.Mmqflj = "0.00";
                js.Jsfjylsh = "0.00";
                js.Grbh = "0.00";
                js.Dbzbc = "0.00";
                js.Czzcje = "0.00";
                js.Elmmxezc = "0.00";
                js.Elmmxesy = "0.00";
                js.Jmecbc = "0.00";
                js.Tjje = "0.00";
                js.Syjjzf = "0.00";
                js.Jmdbydje = "0.00";
                js.Jmdbedje = "0.00";
                js.Jbbcfwnfyzfje = "0.00";
                js.Jbbcybbczcfywfyzf = "0.00";
                js.Mgwxlcjjzf = "0.00";
                js.Jjjmzcfwwkbxfy = "0.00";
                js.Zftdfy = "0.00";
                js.Mfmzjj = "0.00";
                js.Jddbbcbcydzfje = "0.00";
                js.Jddbbcbcedzfje = "0.00";
                js.Jddbbcbcsdzfje = "0.00";
                js.Jdecbcbcydzfje = "0.00";
                js.Jdecbcbcedzfje = "0.00";
                js.Jdecbcbcsdzfje = "0.00";
                js.Jbbcbxbczcfwnfyydzfje = "0.00";
                js.Jbbcbxbczcfwnfyedzfje = "0.00";
                js.Bnzftdjjfylj = "0.00";
                js.Mmxe = "0.00";
                js.Jzgbbzzf = "0.00";
                js.Lxgbdttcjjzf = "0.00";
                #endregion

                #region 医保门诊登记收费
                if (DQJBBZ.Equals("2")) //异地
                {

                    ZXBM = ssqh;
                    WriteLog(sysdate + "  进入门诊登记收费|异地医保...");
                    #region 入参
                    /*
                        1		就诊流水号	VARCHAR2(20)	NOT NULL	唯一
                        2		单据号	VARCHAR2(20)	NULL	预结算为空
                        3		交易日期	VARCHAR2(14)	NOT NULL  	YYYYMMDDHH24MISS
                        4		经办人	VARCHAR2(50)		
                        5		诊断疾病编码	VARCHAR2(20)		
                        6		诊断疾病名称	VARCHAR2(50)		
                        7		医疗类别	VARCHAR2(3)		二级代码
                        8		开发商标志	VARCHAR2(20)	NOT NULL	HIS开发商自定义的特殊标记，能够区分出不同的开发商即可。
                        9		个人编号	VARCHAR2(10)	NOT NULL	异地结算
                        10		姓名	VARCHAR2(20)	NOT NULL	异地结算
                        11		卡号	VARCHAR2(18)	NOT NULL	异地结算
                     * 
                        以下为费用明细入参，这两个参数之间以$符分割，费用明细入参可以传多条, 不允许以$结尾	5	医疗类别	VARCHAR2(3)	
                        0	就诊流水号	VARCHAR2(20)	NOT NULL	唯一
                        1	收费项目种类	VARCHAR2(3)	NOT NULL	二级代码
                        2	费用类别	VARCHAR2(3)	NOT NULL	二级代码
                        3	处方号	VARCHAR2(20)	NOT NULL	非处方药没有处方号，统一传AAAA
                        4	处方日期	VARCHAR2(14)	NOT NULL	YYYYMMDDHH24MISS，非处方药同交易日期
                        5	医院收费项目内码	VARCHAR2(20)		
                        6	收费项目中心编码	VARCHAR2(20)	NOT NULL	中心编号
                        7	医院收费项目名称	VARCHAR2(50)		
                        8	单价	VARCHAR2(12)	NOT NULL	4位小数
                        9	数量	VARCHAR2(12)	NOT NULL	2位小数
                        10	金额	VARCHAR2(12)	NOT NULL	4位小数，金额与 (单价*数量)的差值不能大于0.01
                        11	剂型	VARCHAR2(20)		
                        12	规格	VARCHAR2(100)		
                        13	每次用量	VARCHAR2(12)		2位小数
                        14	使用频次	VARCHAR2(20)		
                        15	医师编号	VARCHAR2(20)		
                        16	医师姓名	VARCHAR2(20)		
                        17	用法	VARCHAR2(20)		
                        18	单位	VARCHAR2(20)		
                        19	科室编号	VARCHAR2(20)		
                        20	科室名称	VARCHAR2(20)		
                        21	限定天数	VARCHAR2(4)		
                        22	草药单复方标志	VARCHAR2(3)		二级代码
                        23	经办人	VARCHAR2(20)		操作员姓名
                        24	药品剂量单位	VARCHAR2(20)		
                        25	自费标志	VARCHAR2(3)		按照自费项目处理
                     */

                    StringBuilder inputParam = new StringBuilder();
                    inputParam.Append(ybjzlsh_snyd + "|");
                    inputParam.Append(djh + "|");
                    inputParam.Append(ghdjsj + "|");
                    inputParam.Append(jbr + "|");
                    inputParam.Append(bzbm + "|");
                    inputParam.Append(bzmc + "|");
                    inputParam.Append(yllb + "|");
                    inputParam.Append("gocent|");
                    inputParam.Append(grbh + "|");
                    inputParam.Append(xm + "|");
                    inputParam.Append(kh + "|$");
                    string xmlb = ds.Tables[0].Rows[0]["sfxmzldm"].ToString();
                    string sflb = ds.Tables[0].Rows[0]["sflbdm"].ToString();
                    string cfh = ghdjsj + "A701";
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
                    string ycyl = "1.00";
                    string pd = "";
                    string ysxm = "周长春";
                    string ysdm = "010002";
                    string yf = "";
                    string dw = "次";
                    string txts = "";
                    string zcffbz = "0";
                    string ypjldw = "次";
                    string qezfbz = "";
                    string ksbm1 = "M001";
                    string ksmc1 = "门诊";
                    djh = ds.Tables[0].Rows[0]["m1invo"].ToString();
                    ds.Dispose();

                    inputParam.Append(ybjzlsh_snyd + "|");
                    inputParam.Append(xmlb + "|");
                    inputParam.Append(sflb + "|");
                    inputParam.Append(cfh + "|");
                    inputParam.Append(cfrq + "|");
                    inputParam.Append(yyxmbh + "|");
                    inputParam.Append(ybxmbh + "|");
                    inputParam.Append(yyxmmc + "|");
                    inputParam.Append(dj + "|");
                    inputParam.Append(sl + "|");
                    inputParam.Append(je + "|");
                    inputParam.Append(jx + "|");
                    inputParam.Append(gg + "|");
                    inputParam.Append(ycyl + "|");
                    inputParam.Append(pd + "|");
                    inputParam.Append(ysxm + "|");
                    inputParam.Append(ysdm + "|");
                    inputParam.Append(yf + "|");
                    inputParam.Append(dw + "|");
                    inputParam.Append(ksbm1 + "|");
                    inputParam.Append(ksmc1 + "|");
                    inputParam.Append(txts + "|");
                    inputParam.Append(zcffbz + "|");
                    inputParam.Append(jbr + "|");
                    inputParam.Append(ypjldw + "|");
                    inputParam.Append(qezfbz + "|");
                    #endregion

                    #region 结算
                    StringBuilder inputData = new StringBuilder();
                    YWBH = "2620";
                    inputData.Append(YWBH + "^");
                    inputData.Append(YLGHBH + "^");
                    inputData.Append(CZYBH + "^");
                    inputData.Append(YWZQH + "^");
                    inputData.Append(JYLSH + "^");
                    inputData.Append(ZXBM + "^");
                    inputData.Append(inputParam.ToString() + "^");
                    inputData.Append(LJBZ + "^");
                    WriteLog(sysdate + "  门诊登记收费|费用结算|入参|" + inputData.ToString());

                    StringBuilder outputData = new StringBuilder(10240);
                    int i = BUSINESS_HANDLE(inputData, outputData);
                    WriteLog(sysdate + "  门诊登记收费|费用结算|出参|" + outputData.ToString());
                    if (i < 0)
                        return new object[] { 0, 0, outputData.ToString().Split('^')[2].ToString() };


                    #endregion

                    #region 出参
                    /*
                        1		医疗费总额	VARCHAR2(16)		2位小数
                        2		总报销金额	VARCHAR2(16)		总报销金额+现金=医疗费总额，2位小数
                        3		统筹基金支付	VARCHAR2(16)		2位小数
                        4		大额基金支付	VARCHAR2(16)		2位小数
                        5		账户支付	VARCHAR2(16)		2位小数
                        6		现金支付	VARCHAR2(16)		2位小数
                        7		公务员补助基金支付	VARCHAR2(16)		2位小数
                        8		企业补充医疗保险基金支付	VARCHAR2(16)		2位小数
                        9		自费费用	VARCHAR2(16)		2位小数
                        10		单位负担费用	VARCHAR2(16)		2位小数
                        11		医院负担费用	VARCHAR2(16)		2位小数
                        12		民政救助费用	VARCHAR2(16)		2位小数
                        13		超限价费用	VARCHAR2(16)		2位小数
                        14		乙类自理费用	VARCHAR2(16)		2位小数
                        15		丙类自理费用	VARCHAR2(16)		2位小数
                        16		符合基本医疗费用	VARCHAR2(16)		2位小数
                        17		起付标准费用	VARCHAR2(16)		2位小数
                        18		转诊转院自付费用	VARCHAR2(16)		2位小数
                        19		进入统筹费用	VARCHAR2(16)		2位小数
                        20		统筹分段自付费用	VARCHAR2(16)		2位小数
                        21		超统筹封顶线费用	VARCHAR2(16)		2位小数
                        22		进入大额报销费用	VARCHAR2(16)		2位小数
                        23		大额分段自付费用	VARCHAR2(16)		2位小数
                        24		超大额封顶线费用	VARCHAR2(16)		2位小数
                        25		人工器官自付费用	VARHCAR2(16)		2位小数
                        26		本次结算前帐户余额	VARHCAR2(16)		2位小数
                        27		本年统筹支付累计(不含本次)	VARHCAR2(16)		2位小数
                        28		本年大额支付累计(不含本次)	VARHCAR2(16)		2位小数
                        29		本年城镇居民门诊统筹支付累计(不含本次)	VARHCAR2(16)		2位小数
                        30		本年公务员补助支付累计(不含本次)	VARHCAR2(16)		2位小数
                        31		本年账户支付累计(不含本次)	VARCHAR2(16)		2位小数
                        32		本年住院次数累计(不含本次)	VARCHAR2(16)		2位小数
                        33		住院次数	VARCHAR2(5)		
                        34		姓名	VARCHAR2(50)		
                        35		结算日期	VARCHAR2(14)		YYYYMMDDHH24MISS
                        36		医疗类别	VARCHAR2(3)		二级代码
                        37		医疗待遇类别	VARCHAR2(3)		二级代码
                        38		经办机构编码	VARCHAR2(16)		二级代码
                        39		业务周期号	VARCHAR2(36)		
                        40		结算流水号	VARCHAR2(20)		获取结算单交易的入参
                        41		提示信息	VARCHAR2(200)		His端必须将此信息显示到前台
                        42		单据号	VARCHAR2(20)		
                        43		交易类型	VARCHAR2(3)		二级代码 
                        44		医院交易流水号	VARCHAR2(50)		
                        45		有效标志	VARCHAR2(3)		二级代码 
                        46		个人编号	VARCHAR2(10)		
                        47		医疗机构编码	VARCHAR2(20)		
                        48		就诊流水号	VARCHAR2(20)		异地撤销要取异地返回的就诊流水号进行撤销
                        49		其他基金支付金额	VARCHAR2(20)		异地就医就医地返回给定点出参（目前只添加了异地就医就医地出参部分，位数按时2420出参位数）
                        50	    伤残人员医疗保障基金	VARCHAR2(16)		异地就医就医地返给定点出参；本地就医返参为0
                     
                     */

                    string[] sfjsfh = outputData.ToString().Split('^')[2].Split('|');
                    js.Ylfze = sfjsfh[0];         //医疗费总费用
                    js.Zbxje = sfjsfh[1];         //总报销金额
                    js.Tcjjzf = sfjsfh[2];        //统筹支出
                    js.Dejjzf = sfjsfh[3];        //大病支出
                    js.Zhzf = sfjsfh[4];          //本次帐户支付
                    js.Xjzf = sfjsfh[5];         //个人现金支付
                    js.Gwybzjjzf = sfjsfh[6];     //公务员补助支付金额
                    js.Qybcylbxjjzf = sfjsfh[7];  //企业补充支付金额
                    js.Zffy = sfjsfh[8];          //自费费用
                    js.Dwfdfy = sfjsfh[9];
                    js.Yyfdfy = sfjsfh[10];
                    js.Mzjzfy = sfjsfh[11];       //民政救助费用
                    js.Cxjfy = sfjsfh[12];
                    js.Ylzlfy = sfjsfh[13];
                    js.Blzlfy = sfjsfh[14];
                    js.Fhjjylfy = sfjsfh[15];
                    js.Qfbzfy = sfjsfh[16];
                    js.Zzzyzffy = sfjsfh[17];
                    js.Jrtcfy = sfjsfh[18];
                    js.Tcfdzffy = sfjsfh[19];
                    js.Ctcfdxfy = sfjsfh[20];
                    js.Jrdebxfy = sfjsfh[21];
                    js.Defdzffy = sfjsfh[22];
                    js.Cdefdxfy = sfjsfh[23];
                    js.Rgqgzffy = sfjsfh[24];
                    js.Bcjsqzhye = sfjsfh[25];
                    js.Bntczflj = sfjsfh[26];
                    js.Bndezflj = sfjsfh[27];
                    js.Bnczjmmztczflj = sfjsfh[28];
                    js.Bngwybzzflj = sfjsfh[29];
                    js.Bnzhzflj = sfjsfh[30];
                    js.Bnzycslj = sfjsfh[31];
                    js.Zycs = sfjsfh[32];
                    js.Xm = sfjsfh[33];
                    js.Jsrq = sfjsfh[34];
                    js.Yllb = sfjsfh[35];
                    js.Yldylb = sfjsfh[36];
                    js.Jbjgbm = sfjsfh[37];
                    js.Ywzqh = sfjsfh[38];
                    js.Jslsh = sfjsfh[39];
                    js.Tsxx = sfjsfh[40];
                    js.Djh = sfjsfh[41];
                    js.Jyxl = sfjsfh[42];
                    js.Yyjylsh = sfjsfh[43];
                    js.Yxbz = sfjsfh[44];
                    js.Grbh = sfjsfh[45];
                    js.Yljgbm = sfjsfh[46];
                    js.Ybjzlsh = sfjsfh[47];
                    js.Qtjjzf = sfjsfh[48];
                    js.Scryylbzjj = sfjsfh[49];

                    js.Ecbcje = "0.00";
                    js.Jzgbbzzf = "0.00";



                    string[] sDBRY = new string[] { "80", "83", "84", "85", "86", "87" };
                    if (sDBRY.Contains(js.Yldylb))
                    {
                        /*
                        * 建档立卡人员除个人自负部分的费用外，均先由医院垫付
                        */
                        js.Ybxjzf = js.Xjzf;
                        js.Zhzbxje = (Convert.ToDecimal(js.Ylfze) - Convert.ToDecimal(js.Ybxjzf)).ToString();
                    }
                    else
                    {
                        /*
                         * 非建档立卡人员（含民政救助对象）住院（含门诊慢性病治疗）的医院垫付金额为：
                         * 统筹支出+账户支付+医院负担费用+民政救助费用+二次补偿金额+企业军转干基金支付。
                         */
                        js.Zhzbxje = (Convert.ToDecimal(js.Tcjjzf) + Convert.ToDecimal(js.Zhzf) + Convert.ToDecimal(js.Yyfdfy) + Convert.ToDecimal(js.Mzjzfy)
                                     + Convert.ToDecimal(js.Ecbcje) + Convert.ToDecimal(js.Jzgbbzzf)).ToString();
                        js.Ybxjzf = (Convert.ToDecimal(js.Ylfze) - Convert.ToDecimal(js.Zhzbxje)).ToString();
                    }
                    js.Qtybzf = (Convert.ToDecimal(js.Ylfze) - Convert.ToDecimal(js.Tcjjzf) - Convert.ToDecimal(js.Ybxjzf) - Convert.ToDecimal(js.Zhzf)).ToString(); ;


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
                    WriteLog(sysdate + "  门诊登记收费结算|整合出参|" + strValue);
                    #endregion

                    #region 数据操作
                    strSql = string.Format(@"insert into ybfyjsdr(jzlsh,ybjzlsh,jylsh,djhin,ylfze,zbxje,tcjjzf,dejjzf,zhzf,xjzf,
                                            gwybzjjzf,qybcylbxjjzf,zffy,dwfdfy,yyfdfy,mzjzfy,cxjfy,ylzlfy,blzlfy,fhjbylfy,
                                            qfbzfy,zzzyzffy,jrtcfy,tcfdzffy,ctcfdxfy,jrdebxfy,defdzffy,cdefdxfy,rgqgzffy,bcjsqzhye,
                                            bntczflj,bndezflj,bnczjmmztczflj,bngwybzzflj,bnzhzflj,bnzycslj,zycs,xm,kh,jsrq,
                                            yllb,yldylb,jbjgbm,ywzqh,jslsh,tsxx,djh,yyjylsh,grbhgl,yljgbm,
                                            ecbcje,mmqflj,jsfjylsh,grbh,czzcje,elmmxezc,elmmxesy,jmecbc,tjje,syjjzf,
                                            jjjmdbydje,jjjmdbedje,jjjmjbbcfwnje,jjjmjbbcfwwje,mgwxlcjjzf,jjjmzcfwwkbxfy,zftdjjzf,mfmzjj,jddbbcbcydzfje,jddbbcbcedzfje,
                                            jddbbcbcsdzfje,jdecbcbcydzfje,jdecbcbcedzfje,jdecbcbcsdzfje,jbbcbxbczcfwnfyydzfje,jbbcbxbczcfwnfyedzfje,bnzftdjjfylj,lxgbddtczf,qtjjzf,scryylbzjj,
                                            zhxjzffy,qtybfy,sysdate,jbr,zhzbxje,cfmxjylsh)
                                            values('{0}','{1}','{2}','{3}','{4}','{5}' ,'{6}','{7}','{8}' ,'{9}' ,
                                            '{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}',
                                            '{20}','{21}','{22}','{23}','{24}','{25}','{26}','{27}','{28}','{29}',
                                            '{30}','{31}','{32}','{33}','{34}','{35}','{36}','{37}','{38}','{39}',
                                            '{40}','{41}','{42}','{43}','{44}','{45}','{46}','{47}','{48}','{49}',
                                            '{50}','{51}','{52}','{53}','{54}','{55}','{56}','{57}','{58}','{59}',
                                            '{60}','{61}','{62}','{63}','{64}','{65}','{66}','{67}','{68}','{69}',
                                            '{70}','{71}','{72}','{73}','{74}','{75}','{76}','{77}','{78}','{79}',
                                            '{80}','{81}','{82}','{83}','{84}','{85}')",
                                         jzlsh, ybjzlsh, JYLSH, djh, js.Ylfze, js.Zbxje, js.Tcjjzf, js.Dejjzf, js.Zhzf, js.Xjzf,
                                         js.Gwybzjjzf, js.Qybcylbxjjzf, js.Zffy, js.Dwfdfy, js.Yyfdfy, js.Mzjzfy, js.Cxjfy, js.Ylzlfy, js.Blzlfy, js.Fhjjylfy,
                                         js.Qfbzfy, js.Zzzyzffy, js.Jrtcfy, js.Tcfdzffy, js.Ctcfdxfy, js.Jrdebxfy, js.Defdzffy, js.Cdefdxfy, js.Rgqgzffy, js.Bcjsqzhye,
                                         js.Bntczflj, js.Bndezflj, js.Bnczjmmztczflj, js.Bngwybzzflj, js.Bnzhzflj, js.Bnzycslj, js.Zycs, js.Xm, js.Kh, js.Jsrq,
                                         js.Yllb, js.Yldylb, js.Jbjgbm, js.Ywzqh, js.Jslsh, js.Tsxx, js.Djh, js.Yyjylsh, js.Grbhgl, js.Yljgbm,
                                         js.Ecbcje, js.Mmqflj, js.Jsfjylsh, js.Grbh, js.Czzcje, js.Elmmxezc, js.Elmmxesy, js.Jmecbc, js.Tjje, js.Syjjzf,
                                         js.Jmdbydje, js.Jmdbedje, js.Jbbcfwnfyzfje, js.Jbbcybbczcfywfyzf, js.Mgwxlcjjzf, js.Jjjmzcfwwkbxfy, js.Zftdfy, js.Mfmzjj, js.Jddbbcbcydzfje, js.Jddbbcbcedzfje,
                                         js.Jddbbcbcsdzfje, js.Jdecbcbcydzfje, js.Jdecbcbcedzfje, js.Jdecbcbcsdzfje, js.Jbbcbxbczcfwnfyydzfje, js.Jbbcbxbczcfwnfyedzfje, js.Bnzftdjjfylj, js.Lxgbdttcjjzf, js.Qtjjzf, js.Scryylbzjj,
                                         js.Ybxjzf, js.Qtybzf, sysdate, jbr, js.Zhzbxje, JYLSH);

                    liSQL.Add(strSql);
                    object[] obj = liSQL.ToArray();
                    obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);
                    if (obj[1].ToString() == "1")
                    {
                        WriteLog(sysdate + "  门诊登记收费成功|本地数据操作成功|" + outputData.ToString().Split('^')[2]);
                        return new object[] { 0, 1, strValue };
                    }
                    else
                    {
                        WriteLog(sysdate + "  门诊登记收费成功|本地数据操作失败|" + obj[2].ToString());
                        //费用结算撤销信息、处方明细上传撤销
                        object[] objFYJSCX = { jzlsh, js.Djh, ybjzlsh, ghdjsj, grbh, xm, kh, ssqh, ybjzlsh_snyd, "", DQJBBZ };
                        NYBFYJSCX(objFYJSCX);
                        return new object[] { 0, 0, "数据库操作失败" + obj[2].ToString() };
                    }
                    #endregion
                }
                else //市本级
                {
                    ZXBM = "0000";
                    WriteLog(sysdate + "  进入门诊登记收费|市本级...");

                    #region 费用上传&入参
                    /*
                        1		就诊流水号	VARCHAR2(18)	NOT NULL	同登记时的就诊流水号
                        2		收费项目种类	VARCHAR2(3)	NOT NULL	二级代码
                        3		收费类别	VARCHAR2(3)	NOT NULL	二级代码
                        4		处方号	VARCHAR2(20)	NOT NULL	
                        5		处方日期	VARCHAR2(14)	NOT NULL	YYYYMMDDHH24MISS
                        6		医院收费项目内码	VARCHAR2(20)	NOT NULL	
                        7		收费项目中心编码	VARCHAR2(20)	NOT NULL	中心编号
                        8		医院收费项目名称	VARCHAR2(50)	NOT NULL	
                        9		单价	VARCHAR2(12)	NOT NULL	4位小数
                        10		数量	VARCHAR2(12)	NOT NULL	2位小数
                        11		金额	VARCHAR2(12)	NOT NULL	4位小数，金额与 (单价*数量)的差值不能大于0.01
                        12		剂型	VARCHAR2(20)		二级代码(收费类别为西药和中成药时必传)
                        13		规格	VARCHAR2(100)		二级代码(收费类别为西药和中成药时必传)
                        14		每次用量	VARCHAR2(12)	NOT NULL	2位小数
                        15		使用频次	VARCHAR2(20)	NOT NULL	
                        16		医师编码	VARCHAR2(20)	NOT NULL	
                        17		医师姓名	VARCHAR2(50)	NOT NULL	
                        18		用法	VARCHAR2(100)		
                        19		单位	VARCHAR2(20)		
                        20		科室编号	VARCHAR2(20)	NOT NULL	
                        21		科室名称	VARCHAR2(50)	NOT NULL	
                        22		执行天数	VARCHAR2(4)		
                        23		草药单复方标志	VARCHAR2(3)		二级代码
                        24		经办人	VARCHAR2(20)	NOT NULL	医疗机构操作员姓名
                        25		药品剂量单位	VARCHAR2(20)		当上传药品时非空
                        26		全额自费标志	VARCHAR2(3)		当传入自费标志，认为本项目按照自费处理，针对限制用药使用
                        27	个人编号	VARCHAR2(10)	NOT NULL	异地结算
                        28	姓名	VARCHAR2(20)	NOT NULL	异地结算
                        29	卡号	VARCHAR2(18)	NOT NULL	异地结算
                 */
                    string xmlb = ds.Tables[0].Rows[0]["sfxmzldm"].ToString();
                    string sflb = ds.Tables[0].Rows[0]["sflbdm"].ToString();
                    string cfh = ghdjsj + "A701";
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
                    string ycyl = "1.00";
                    string pd = "";
                    string ysxm = "梁秀娟";
                    string ysdm = "010007";
                    string yf = "";
                    string dw = "次";
                    string txts = "";
                    string zcffbz = "0";
                    string ypjldw = "次";
                    string qezfbz = "";
                    string ksbm1 = "M001";
                    string ksmc1 = "门诊";
                    djh = ds.Tables[0].Rows[0]["m1invo"].ToString();
                    ds.Dispose();

                    StringBuilder inputParam = new StringBuilder();
                    inputParam.Append(ybjzlsh + "|");
                    inputParam.Append(xmlb + "|");
                    inputParam.Append(sflb + "|");
                    inputParam.Append(cfh + "|");
                    inputParam.Append(cfrq + "|");
                    inputParam.Append(yyxmbh + "|");
                    inputParam.Append(ybxmbh + "|");
                    inputParam.Append(yyxmmc + "|");
                    inputParam.Append(dj + "|");
                    inputParam.Append(sl + "|");
                    inputParam.Append(je + "|");
                    inputParam.Append(jx + "|");
                    inputParam.Append(gg + "|");
                    inputParam.Append(ycyl + "|");
                    inputParam.Append(pd + "|");
                    inputParam.Append(ysdm + "|");
                    inputParam.Append(ysxm + "|");
                    inputParam.Append(yf + "|");
                    inputParam.Append(dw + "|");
                    inputParam.Append(ksbm1 + "|");
                    inputParam.Append(ksmc1 + "|");
                    inputParam.Append(txts + "|");
                    inputParam.Append(zcffbz + "|");
                    inputParam.Append(jbr + "|");
                    inputParam.Append(ypjldw + "|");
                    inputParam.Append(qezfbz + "|");
                    inputParam.Append(grbh + "|");
                    inputParam.Append(xm + "|");
                    inputParam.Append(kh + "|");
                    #endregion

                    #region 费用上传
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
                    strSql = string.Format(@"insert into ybcfmxscindr(jzlsh,jylsh,xm,kh,ybjzlsh,cfrq,yysfxmbm,yysfxmmc,sfxmzxbm,sfxmzxmc,
                                        dj,sl,je,jbr,sysdate,sflb,ybcfh) values(
                                        '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}',
                                        '{10}','{11}','{12}','{13}','{14}','{15}','{16}')",
                                            jzlsh, JYLSH, xm, kh, ybjzlsh, cfrq, yyxmbh, yyxmmc, ybxmbh, ybxmmc,
                                            dj, sl, je, jbr, sysdate, sflb, cfh);
                    liSQL.Add(strSql);
                    string[] cffh = outputData.ToString().Split('^')[2].Split('#');
                    string[] cfmx = cffh[0].Split('|');


                    /*1		金额	VARCHAR2(16)		4位小数
                    2		自理金额(自付金额)	VARCHAR2(16)		4位小数
                    3		自费金额	VARCHAR2(16)		4位小数
                    4		超限价自付金额	VARCHAR2(16)		4位小数，包含在自费金额中
                    5		收费类别	VARCHAR2(3)		二级代码
                    6		收费项目等级	VARCHAR2(3)		二级代码
                    7		全额自费标志	VARCHAR2(3)		二级代码
                    8		自理比例（自付比例）	VARCHAR2(5)		4位小数
                    9		限价	VARCHAR2(16)		4位小数
                    10		备注	VARCHAR2(200)		说明出现自费自理金额的原因，his端必须将此信息显示到前台。
                    1	疑点状态	VARCHAR2(3)		0：无疑点   1：有疑点
                    2	疑点（包含疑点ID和疑点说明）	VARCHAR2(4000)		疑点可能是多条，用“|”分隔，每条疑点里的疑点ID和疑点说明用“~”分隔
                        例如：#疑点状态|疑点ID~疑点说明|疑点ID~疑点说明

                     */
                    outParams_fymx op = new outParams_fymx();
                    op.Je = cfmx[0];
                    op.Zlje = cfmx[1];
                    op.Zfje = cfmx[2];
                    op.Cxjzfje = cfmx[3];
                    op.Sfxmlb = cfmx[4];
                    op.Sfxmdj = cfmx[5];
                    op.Zfbz = cfmx[6];
                    op.Zlbz = cfmx[7];
                    op.Xjbz = cfmx[8]; //限价
                    op.Bz = cfmx[9];

                    if (cffh.Length > 1)
                        op.Bz += cffh[1];

                    strSql = string.Format(@"insert into ybcfmxscfhdr(jzlsh,jylsh,ybjzlsh,yyxmdm,yyxmmc,yybxmbh,ybxmmc,je,zlje,zfje,
                                        cxjzfje,sflb,sfxmdj,qezfbz,zlbl,xj,bz,ybcfh) 
                                        values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}',
                                        '{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}')",
                                            jzlsh, JYLSH, ybjzlsh, yyxmbh, yyxmmc, ybxmbh, ybxmmc, op.Je, op.Zlje, op.Zfje,
                                            op.Cxjzfje, op.Sfxmlb, op.Sfxmdj, op.Zfbz, op.Zlbz, op.Xjbz, op.Bz, cfh);
                    liSQL.Add(strSql);
                    #endregion

                    #region 结算& 入参
                    inputParam.Clear();
                    /*
                     1		就诊流水号	VARCHAR2(20)	NOT NULL	同登记时的就诊流水号
                    2		单据号	VARCHAR2(20)	NOT NULL	固定传“0000”
                    3		医疗类别	VARCHAR2(3)	NOT NULL	二级代码
                    4		结算日期	VARCHAR2(14)	NOT NULL	YYYYMMDDHH24MISS
                    5		出院日期	VARCHAR2(14)	NOT NULL	YYYYMMDDHH24MISS
                    6		出院原因	VARCHAR2(3)	NOT NULL	二级代码
                    7		病种编码	VARCHAR2(20)		门诊大病、慢病、住院不能为空。
                    8		病种名称	VARCHAR2(50)		有病种编码时，对应的病种名称不能为空。
                    9		账户使用标志	VARCHAR2(3)	NOT NULL	二级代码【景德镇】普通门诊和药店购药使用账户支付，普通住院不允许使用账户支付，门诊慢性病1类不允许使用个人账户支付，二类门诊慢性病强制使用账户支付个人负担部分，账户余额不足使用现金支付
                    10		中途结算标志	VARCHAR2(3)		二级代码
                    11		经办人	VARCHAR2(50)	NOT NULL	医疗机构操作员姓名
                    12		开发商标志	VARCHAR2(20)	NOT NULL	HIS开发商自定义的特殊标记，能够区分出不同的开发商即可 开发商标志：01     东软 02     中软 03     其他
                    13		次要诊断编码1	VARCHAR2(20)		
                    14		次要诊断名称1	VARCHAR2(50)		
                    15		次要诊断编码2	VARCHAR2(20)		
                    16		次要诊断名称2	VARCHAR2(50)		
                    17		次要诊断编码3	VARCHAR2(20)		
                    18		次要诊断名称3	VARCHAR2(50)		
                    19		次要诊断编码4	VARCHAR2(20)		
                    20		次要诊断名称4	VARCHAR2(50)		
                    21		治疗方法	VARCHAR2(3)	医疗类别：单病种	代码为：01常规手术 02鼻内镜 03膀胱镜 04 腹腔镜手术 05 使用肛肠吻合器
                    22	个人编号	VARCHAR2(10)	NOT NULL	异地结算
                    23	姓名	VARCHAR2(20)	NOT NULL	异地结算
                    24	卡号	VARCHAR2(18)	NOT NULL	异地结算
                    25	胎儿数	VARCHAR2(3)	NOT NULL	生育住院时，生产病种必须上传。
                    26	手术类型	VARCHAR2(3)		省本级 二级代码 0-	非手术 1-手术 （定点做住院类（普通住院、放化疗住院）结算与预结算交易时必须传入手术类型（0-非手术 1-手术），其他情况可为空。）
                     */
                    inputParam.Append(ybjzlsh + "|");
                    inputParam.Append(djh + "|");
                    inputParam.Append(yllb + "|");
                    inputParam.Append(ghdjsj + "|");
                    inputParam.Append(ghdjsj+"|");
                    inputParam.Append("|");
                    inputParam.Append(bzbm + "|");
                    inputParam.Append(bzmc + "|");
                    inputParam.Append("1|");
                    inputParam.Append("0|");
                    inputParam.Append(jbr + "|");
                    inputParam.Append("03|");
                    inputParam.Append("|");
                    inputParam.Append("|");
                    inputParam.Append("|");
                    inputParam.Append("|");
                    inputParam.Append("|");
                    inputParam.Append("|");
                    inputParam.Append("|");
                    inputParam.Append("|");
                    inputParam.Append("|"); //治疗方法
                    inputParam.Append(grbh + "|");
                    inputParam.Append(xm + "|");
                    inputParam.Append(kh + "|");
                    inputParam.Append("|");
                    inputParam.Append("|");
                    #endregion

                    #region 医保结算
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
                    WriteLog(sysdate + "  门诊登记收费|费用结算|出参|" + outputData.ToString());
                    if (i != 0)
                    {
                        object[] objFYMXCX = { ybjzlsh, "", grbh, xm, kh };
                        YBMZCFMXSCCX(objFYMXCX);
                        object[] objMZDJCX = { jzlsh,yllb};
                        YBMZDJCX(objMZDJCX);
                        return new object[] { 0, 0, outputData.ToString() };
                    }

                    #endregion

                    #region 出参
                    string[] sfjsfh = outputData.ToString().Split('^')[2].Split('|');
                    /*
                     1		医疗费总额	VARCHAR2(16)		2位小数
                    2		总报销金额	VARCHAR2(16)		总报销金额+现金=医疗费总额，2位小数
                    3		统筹基金支付	VARCHAR2(16)		2位小数
                    4		大额基金支付	VARCHAR2(16)		2位小数
                    5		账户支付	VARCHAR2(16)		2位小数
                    6		现金支付	VARCHAR2(16)		2位小数
                    7		公务员补助基金支付	VARCHAR2(16)		2位小数
                    8		企业补充医疗保险基金支付（九江、萍乡、鹰潭疾病补充保险支付）	VARCHAR2(16)		2位小数
                    9		自费费用	VARCHAR2(16)		2位小数
                    10		单位负担费用	VARCHAR2(16)		2位小数
                    11		医院负担费用	VARCHAR2(16)		2位小数
                    12		民政救助费用	VARCHAR2(16)		2位小数
                    13		超限价费用	VARCHAR2(16)		2位小数
                    14		乙类自理费用	VARCHAR2(16)		2位小数
                    15		丙类自理费用	VARCHAR2(16)		2位小数
                    16		符合基本医疗费用	VARCHAR2(16)		2位小数
                    17		起付标准费用	VARCHAR2(16)		2位小数
                    18		转诊转院自付费用	VARCHAR2(16)		2位小数
                    19		进入统筹费用	VARCHAR2(16)		2位小数
                    20		统筹分段自付费用	VARCHAR2(16)		2位小数
                    21		超统筹封顶线费用	VARCHAR2(16)		2位小数
                    22		进入大额报销费用	VARCHAR2(16)		2位小数
                    23		大额分段自付费用	VARCHAR2(16)		2位小数
                    24		超大额封顶线费用	VARCHAR2(16)		2位小数
                    25		人工器官自付费用	VARHCAR2(16)		2位小数
                    26		本次结算前帐户余额	VARHCAR2(16)		2位小数
                    27		本年统筹支付累计(不含本次)	VARHCAR2(16)		2位小数
                    28		本年大额支付累计(不含本次)	VARHCAR2(16)		2位小数
                    29		本年城镇居民门诊统筹支付累计(不含本次)	VARHCAR2(16)		2位小数
                    30		本年公务员补助支付累计(不含本次)	VARHCAR2(16)		2位小数
                    31		本年账户支付累计(不含本次)	VARCHAR2(16)		2位小数
                    32		本年住院次数累计(不含本次)	VARCHAR2(16)		2位小数
                    33		住院次数	VARCHAR2(5)		
                    34		姓名	VARCHAR2(50)		
                    35		结算日期	VARCHAR2(14)		YYYYMMDDHH24MISS
                    36		医疗类别	VARCHAR2(3)		二级代码
                    37		医疗待遇类别	VARCHAR2(3)		二级代码
                    38		经办机构编码	VARCHAR2(16)		二级代码
                    39		业务周期号	VARCHAR2(36)		
                    40		结算流水号	VARCHAR2(20)		获取结算单交易的入参
                    41		提示信息	VARCHAR2(200)		His端必须将此信息显示到前台
                    42		单据号	VARCHAR2(20)		
                    43		交易类型	VARCHAR2(3)		二级代码 
                    44		医院交易流水号	VARCHAR2(50)		
                    45		有效标志	VARCHAR2(3)		二级代码 
                    46		个人编号管理	VARCHAR2(10)		
                    47		医疗机构编码	VARCHAR2(20)		
                    48		二次补偿金额	VARCHAR2(16)		2位小数
                    49		门慢起付累计	VARCHAR2(16)		2位小数
                    50		接收方交易流水号	VARCHAR2(50)		
                    51		个人编号	VARCHAR2(16)		
                    52		单病种补差	VARCHAR2(16)		2位小数【鹰潭专用】
                    53		财政支出金额	VARCHAR2(16)		【萍乡】公立医院用【2位小数】
                    54		二类门慢限额支出（景德镇）门慢限额支出（省本级）	VARCHAR2(16)		【景德镇】【省本级】专用
                    55		二类门慢限额剩余	VARCHAR2(16)		【景德镇】【省本级】专用
                    56		居民二次补偿（大病支付）	VARCHAR2(16)		【鹰潭】专用2位小数
                    57		体检金额	VARCHAR2(16)		【九江】专用2位小数
                    58		生育基金支付	VARCHAR2(16)		
                    59		居民大病一段金额	VARCHAR2(16)		【九江、鹰潭居民】专用2位小数
                    60		居民大病二段金额	VARCHAR2(16)		【九江、鹰潭居民】
                    61		疾病补充范围内费用支付金额	VARCHAR2(16)		【九江/鹰潭/萍乡居民】
                    62		疾病补充保险本次政策范围外费用支付金额	VARCHAR2(16)		【九江/鹰潭/萍乡居民】
                    63		美国微笑列车基金支付	VARCHAR2(16)		【九江/鹰潭居民】
                    64		九江居民政策范围外可报销费用	VARCHAR2(16)		【九江居民】
                    65		政府兜底基金费用	VARCHAR2(16)		【萍乡/鹰潭/九江居民】
                    66		免费门诊基金（余江）	VARCHAR2(16)		【鹰潭居民】
                    67		建档大病补偿本次一段支付金额	VARCHAR2(16)		【鹰潭居民】
                    68		建档大病补偿本次二段支付金额	VARCHAR2(16)		【鹰潭居民】
                    69		建档大病补偿本次三段支付金额	VARCHAR2(16)		【鹰潭居民】
                    70		建档二次补偿本次一段支付金额	VARCHAR2(16)		【鹰潭居民】
                    71		建档二次补偿本次二段支付金额	VARCHAR2(16)		【鹰潭居民】
                    72		建档二次补偿本次三段支付金额	VARCHAR2(16)		【鹰潭居民】
                    73		疾病补充保险本次政策范围内费用一段支付金额	VARCHAR2(16)		【鹰潭居民】
                    74		疾病补充保险本次政策范围内费用二段支付金额	VARCHAR2(16)		【鹰潭居民】
                    75		本年政府兜底基金费用累计(不含本次)	VARCHAR2(16)		【九江居民】
                    76		门慢限额	VARCHAR2(16)		【江西省本级】
                    77		企业军转干基金支付	VARCHAR2(16)		【鹰潭】
                    78		其他基金支付金额	VARCHAR2(16)		异地就医就医地返给定点出参；本地就医返参为0
                    79		伤残人员医疗保障基金	VARCHAR2(16)		异地就医就医地返给定点出参；本地就医返参为0
                 */

                    js.Ylfze = sfjsfh[0];         //医疗费总费用
                    js.Zbxje = sfjsfh[1];         //总报销金额
                    js.Tcjjzf = sfjsfh[2];        //统筹支出
                    js.Dejjzf = sfjsfh[3];        //大病支出
                    js.Zhzf = sfjsfh[4];          //本次帐户支付
                    js.Xjzf = sfjsfh[5];         //个人现金支付
                    js.Gwybzjjzf = sfjsfh[6];     //公务员补助支付金额
                    js.Qybcylbxjjzf = sfjsfh[7];  //企业补充支付金额
                    js.Zffy = sfjsfh[8];          //自费费用
                    js.Dwfdfy = sfjsfh[9];
                    js.Yyfdfy = sfjsfh[10];
                    js.Mzjzfy = sfjsfh[11];       //民政救助费用
                    js.Cxjfy = sfjsfh[12];
                    js.Ylzlfy = sfjsfh[13];
                    js.Blzlfy = sfjsfh[14];
                    js.Fhjjylfy = sfjsfh[15];
                    js.Qfbzfy = sfjsfh[16];
                    js.Zzzyzffy = sfjsfh[17];
                    js.Jrtcfy = sfjsfh[18];
                    js.Tcfdzffy = sfjsfh[19];
                    js.Ctcfdxfy = sfjsfh[20];
                    js.Jrdebxfy = sfjsfh[21];
                    js.Defdzffy = sfjsfh[22];
                    js.Cdefdxfy = sfjsfh[23];
                    js.Rgqgzffy = sfjsfh[24];
                    js.Bcjsqzhye = sfjsfh[25];
                    js.Bntczflj = sfjsfh[26];
                    js.Bndezflj = sfjsfh[27];
                    js.Bnczjmmztczflj = sfjsfh[28];
                    js.Bngwybzzflj = sfjsfh[29];
                    js.Bnzhzflj = sfjsfh[30];
                    js.Bnzycslj = sfjsfh[31];
                    js.Zycs = sfjsfh[32];
                    js.Xm = sfjsfh[33];
                    js.Jsrq = sfjsfh[34];
                    js.Yllb = sfjsfh[35];
                    js.Yldylb = sfjsfh[36];
                    js.Jbjgbm = sfjsfh[37];
                    js.Ywzqh = sfjsfh[38];
                    js.Jslsh = sfjsfh[39];
                    js.Tsxx = sfjsfh[40];
                    js.Djh = sfjsfh[41];
                    js.Jyxl = sfjsfh[42];
                    js.Yyjylsh = sfjsfh[43];
                    js.Yxbz = sfjsfh[44];
                    js.Grbhgl = sfjsfh[45];
                    js.Yljgbm = sfjsfh[46];
                    js.Ecbcje = sfjsfh[47];
                    js.Mmqflj = sfjsfh[48];
                    js.Jsfjylsh = sfjsfh[49];
                    js.Grbh = sfjsfh[50];
                    js.Dbzbc = sfjsfh[51];
                    js.Czzcje = sfjsfh[52];
                    js.Elmmxezc = sfjsfh[53];
                    js.Elmmxesy = sfjsfh[54];
                    js.Jmecbc = sfjsfh[55];
                    js.Tjje = sfjsfh[56];
                    js.Syjjzf = sfjsfh[57];
                    js.Jmdbydje = sfjsfh[58];
                    js.Jmdbedje = sfjsfh[59];
                    js.Jbbcfwnfyzfje = sfjsfh[60];
                    js.Jbbcybbczcfywfyzf = sfjsfh[61];
                    js.Mgwxlcjjzf = sfjsfh[62];
                    js.Jjjmzcfwwkbxfy = sfjsfh[63];
                    js.Zftdfy = sfjsfh[64];
                    js.Mfmzjj = sfjsfh[65];
                    js.Jddbbcbcydzfje = sfjsfh[66];
                    js.Jddbbcbcedzfje = sfjsfh[67];
                    js.Jddbbcbcsdzfje = sfjsfh[68];
                    js.Jdecbcbcydzfje = sfjsfh[69];
                    js.Jdecbcbcedzfje = sfjsfh[70];
                    js.Jdecbcbcsdzfje = sfjsfh[71];
                    js.Jbbcbxbczcfwnfyydzfje = sfjsfh[72];
                    js.Jbbcbxbczcfwnfyedzfje = sfjsfh[73];
                    js.Bnzftdjjfylj = sfjsfh[74];
                    js.Mmxe = sfjsfh[75];
                    js.Jzgbbzzf = sfjsfh[76];
                    js.Qtjjzf = sfjsfh[77];
                    js.Scryylbzjj = sfjsfh[78];
                    if (string.IsNullOrEmpty(js.Scryylbzjj))
                        js.Scryylbzjj = "0.00";

                    string[] sDBRY = new string[] { "80", "83", "84", "85", "86", "87" };
                    if (sDBRY.Contains(js.Yldylb))
                    {
                        /*
                        * 建档立卡人员除个人自负部分的费用外，均先由医院垫付
                        */
                        js.Ybxjzf = js.Xjzf;
                        js.Zhzbxje = (Convert.ToDecimal(js.Ylfze) - Convert.ToDecimal(js.Ybxjzf)).ToString();
                    }
                    else
                    {
                        /*
                         * 非建档立卡人员（含民政救助对象）住院（含门诊慢性病治疗）的医院垫付金额为：
                         * 统筹支出+账户支付+医院负担费用+民政救助费用+二次补偿金额+企业军转干基金支付。
                         */
                        js.Zhzbxje = (Convert.ToDecimal(js.Tcjjzf) + Convert.ToDecimal(js.Zhzf) + Convert.ToDecimal(js.Yyfdfy) + Convert.ToDecimal(js.Mzjzfy)
                                     + Convert.ToDecimal(js.Ecbcje) + Convert.ToDecimal(js.Jzgbbzzf)).ToString();
                        js.Ybxjzf = (Convert.ToDecimal(js.Ylfze) - Convert.ToDecimal(js.Zhzbxje)).ToString();
                    }
                    js.Qtybzf = (Convert.ToDecimal(js.Ylfze) - Convert.ToDecimal(js.Tcjjzf) - Convert.ToDecimal(js.Ybxjzf) - Convert.ToDecimal(js.Zhzf)).ToString(); ;

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
                    string strValue = js.Ylfze + "|" + js.Zhzbxje + "|" + js.Tcjjzf + "|" + js.Dejjzf + "|" + js.Zhzf + "|" +
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
                                    js.Jmgrzfecbcje + "|" + js.Tjje + "|" + js.Syjjzf + "|";
                    WriteLog(sysdate + "  门诊登记收费结算|整合出参|" + strValue);
                    #endregion

                    #region 数据操作
                    strSql = string.Format(@"insert into ybfyjsdr(jzlsh,ybjzlsh,jylsh,djhin,ylfze,zbxje,tcjjzf,dejjzf,zhzf,xjzf,
                                            gwybzjjzf,qybcylbxjjzf,zffy,dwfdfy,yyfdfy,mzjzfy,cxjfy,ylzlfy,blzlfy,fhjbylfy,
                                            qfbzfy,zzzyzffy,jrtcfy,tcfdzffy,ctcfdxfy,jrdebxfy,defdzffy,cdefdxfy,rgqgzffy,bcjsqzhye,
                                            bntczflj,bndezflj,bnczjmmztczflj,bngwybzzflj,bnzhzflj,bnzycslj,zycs,xm,kh,jsrq,
                                            yllb,yldylb,jbjgbm,ywzqh,jslsh,tsxx,djh,yyjylsh,grbhgl,yljgbm,
                                            ecbcje,mmqflj,jsfjylsh,grbh,czzcje,elmmxezc,elmmxesy,jmecbc,tjje,syjjzf,
                                            jjjmdbydje,jjjmdbedje,jjjmjbbcfwnje,jjjmjbbcfwwje,mgwxlcjjzf,jjjmzcfwwkbxfy,zftdjjzf,mfmzjj,jddbbcbcydzfje,jddbbcbcedzfje,
                                            jddbbcbcsdzfje,jdecbcbcydzfje,jdecbcbcedzfje,jdecbcbcsdzfje,jbbcbxbczcfwnfyydzfje,jbbcbxbczcfwnfyedzfje,bnzftdjjfylj,lxgbddtczf,qtjjzf,scryylbzjj,
                                            zhxjzffy,qtybfy,sysdate,jbr,zhzbxje,cfmxjylsh)
                                            values('{0}','{1}','{2}','{3}','{4}','{5}' ,'{6}','{7}','{8}' ,'{9}' ,
                                            '{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}',
                                            '{20}','{21}','{22}','{23}','{24}','{25}','{26}','{27}','{28}','{29}',
                                            '{30}','{31}','{32}','{33}','{34}','{35}','{36}','{37}','{38}','{39}',
                                            '{40}','{41}','{42}','{43}','{44}','{45}','{46}','{47}','{48}','{49}',
                                            '{50}','{51}','{52}','{53}','{54}','{55}','{56}','{57}','{58}','{59}',
                                            '{60}','{61}','{62}','{63}','{64}','{65}','{66}','{67}','{68}','{69}',
                                            '{70}','{71}','{72}','{73}','{74}','{75}','{76}','{77}','{78}','{79}',
                                            '{80}','{81}','{82}','{83}','{84}','{85}')",
                                        jzlsh, ybjzlsh, JYLSH, djh, js.Ylfze, js.Zbxje, js.Tcjjzf, js.Dejjzf, js.Zhzf, js.Xjzf,
                                        js.Gwybzjjzf, js.Qybcylbxjjzf, js.Zffy, js.Dwfdfy, js.Yyfdfy, js.Mzjzfy, js.Cxjfy, js.Ylzlfy, js.Blzlfy, js.Fhjjylfy,
                                        js.Qfbzfy, js.Zzzyzffy, js.Jrtcfy, js.Tcfdzffy, js.Ctcfdxfy, js.Jrdebxfy, js.Defdzffy, js.Cdefdxfy, js.Rgqgzffy, js.Bcjsqzhye,
                                        js.Bntczflj, js.Bndezflj, js.Bnczjmmztczflj, js.Bngwybzzflj, js.Bnzhzflj, js.Bnzycslj, js.Zycs, js.Xm, js.Kh, js.Jsrq,
                                        js.Yllb, js.Yldylb, js.Jbjgbm, js.Ywzqh, js.Jslsh, js.Tsxx, js.Djh, js.Yyjylsh, js.Grbhgl, js.Yljgbm,
                                        js.Ecbcje, js.Mmqflj, js.Jsfjylsh, js.Grbh, js.Czzcje, js.Elmmxezc, js.Elmmxesy, js.Jmecbc, js.Tjje, js.Syjjzf,
                                        js.Jmdbydje, js.Jmdbedje, js.Jbbcfwnfyzfje, js.Jbbcybbczcfywfyzf, js.Mgwxlcjjzf, js.Jjjmzcfwwkbxfy, js.Zftdfy, js.Mfmzjj, js.Jddbbcbcydzfje, js.Jddbbcbcedzfje,
                                        js.Jddbbcbcsdzfje, js.Jdecbcbcydzfje, js.Jdecbcbcedzfje, js.Jdecbcbcsdzfje, js.Jbbcbxbczcfwnfyydzfje, js.Jbbcbxbczcfwnfyedzfje, js.Bnzftdjjfylj, js.Lxgbdttcjjzf, js.Qtjjzf, js.Scryylbzjj,
                                        js.Ybxjzf, js.Qtybzf, sysdate, jbr, js.Zhzbxje,JYLSH);

                    liSQL.Add(strSql);
                    object[] obj = liSQL.ToArray();
                    obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);
                    if (obj[1].ToString() == "1")
                    {
                        WriteLog(sysdate + "  门诊登记收费成功|本地数据操作成功|" + outputData.ToString().Split('^')[2]);
                        return new object[] { 0, 1, strValue };
                    }
                    else
                    {
                        WriteLog(sysdate + "  门诊登记收费成功|本地数据操作失败|" + obj[2].ToString());
                        //费用结算撤销信息、处方明细上传撤销
                        object[] objFYJSCX = { jzlsh, js.Djh, ybjzlsh, ghdjsj, grbh, xm, kh, ssqh, ybjzlsh_snyd, "", DQJBBZ };
                        NYBFYJSCX(objFYJSCX);
                        return new object[] { 0, 0, "数据库操作失败" + obj[2].ToString() };
                    }
                    #endregion
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

        #region 门诊登记收费撤销
        public static object YBMZDJSFCX(object[] objParam)
        {
            string sysdate = GetServerDateTime();
            WriteLog(sysdate + "  进入门诊登记收费撤销...");
            try
            {
                string jzlsh = objParam[0].ToString();
                string dkbz = objParam[1].ToString(); //读卡标志
                //string djh = objParam[2].ToString();
                string jbr = CliUtils.fUserName;
                string yllb = "";

                object[] objParam1 = new object[] { };
                //objParam1 = YBMZDK(objParam1);
                //if (objParam1[1].ToString().Equals("0"))
                //    return objParam1;
                string strSql = string.Format(@"select m1invo from mz01t where m1ghno='{0}'", jzlsh);
                DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                string djh = ds.Tables[0].Rows[0]["m1invo"].ToString();
                strSql = string.Format(@"select b.yllb from ybfyjsdr a 
                                                inner join ybmzzydjdr b on a.jzlsh=b.jzlsh and a.ybjzlsh=b.ybjzlsh and a.cxbz=b.cxbz 
                                                where a.jzlsh='{0}' and a.djhin='{1}' and a.cxbz=1", jzlsh, djh);
                ds.Tables.Clear();
                ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                if (ds.Tables[0].Rows.Count == 0)
                {
                    object[] objDJCX = { jzlsh, "" };
                    objDJCX = YBMZDJCX(objDJCX);
                    if (objDJCX[1].ToString().Equals("1"))
                        return new object[] { 0, 1, "门诊登记收费撤销成功" };
                    else
                        return objDJCX;
                }
                else
                {
                    object[] objJSCX = { jzlsh, djh };

                    objJSCX = YBMZSFJSCX(objJSCX);
                    if (objJSCX[1].ToString().Equals("1"))
                    {
                        object[] objDJCX = { jzlsh, "" };
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
                YWZQH = CliUtils.fLoginYbNo.Split('|')[0].ToString();    //业务周期号
            }
            catch
            {
                return new object[] { 0, 0, "医保未连接或初始化失败" };
            }


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
                ZXBM = "0000";
                string jbr = CliUtils.fUserName;    //经办人姓名
                string cfsj = DateTime.Now.ToString("yyyyMMddHHmmss");  //处方时间
                JYLSH = cfsj + "-" + YLGHBH + "-" + new Random().Next(100).ToString().PadLeft(4, '0');

                decimal sfje = Convert.ToDecimal(ylfze);

                #region 是否医保登记
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
                string ksbm = dr1["ksbh"].ToString();
                string ksmc = dr1["ksmc"].ToString();
                #endregion

                StringBuilder inputParam = new StringBuilder();

                #region  判断草药单复方标志
                string zcffbz = "";//草药单复方标志
                strSql = string.Format(@"select mcmzno as mcmzno from mzcfd where  mccfno LIKE 'C%' and  mcghno='{0}' AND mccfno IN ({1})", jzlsh, cfhs);
                ds.Tables.Clear();
                ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                if (ds.Tables[0].Rows.Count > 1)
                    zcffbz = "1";
                else if (ds.Tables[0].Rows.Count == 1)
                    zcffbz = "0";
                #endregion

                #region 获取处方明细信息
                strSql = string.Format(@"select y.ybxmbh, y.ybxmmc, m.dj, sum(m.sl) sl, sum(m.je) je, m.yyxmbh, m.yyxmmc, y.sfxmzldm, y.sflbdm,y.jxdm, m.cfh,m.gg,m.jxdw,y.jx,m.yf,m.sfno,y.dw from 
                                        (
                                        --药品
                                        select a.mcypno yyxmbh, a.mcypnm yyxmmc, a.mcpric dj, a.mcquty sl, a.mcamnt je, a.mccfno cfh,a.mcypgg gg,a.mcunt1 as jxdw,a.mcwayx as yf,a.mcsflb as sfno
                                        from mzcfd a 
                                        where a.mcghno = '{0}' and a.mccfno in ({1})
                                        union all
                                        --检查/治疗
                                        select b.mbitem yyxmbh, b.mbname yyxmmc, b.mbpric dj, b.mbquty sl, b.mbsjam je,b.mbzlno cfh,NULL as gg,NULL as jxdw,NULL AS yf,b.mbsfno as sfno         
                                        from mzb2d b 
                                        where b.mbghno = '{0}' and b.mbzlno in ({1})
                                        union all
                                        --检验
                                        select c.mbitem yyxmbh, c.mbname yyxmmc, c.mbpric dj, c.mbquty sl, c.mbsjam je, c.mbzlno cfh,NULL as gg,NULL as jxdw,NULL AS yf,c.mbsfno as sfno 
                                        from mzb4d c 
                                        where c.mbghno = '{0}' and c.mbzlno in ({1})
                                        union all
                                        --注射
                                        select b5item yyxmbh, b5name yyxmmc, b5sfam dj, mddays sl, b5sfam * mddays je, mdzsno cfh,NULL as gg,NULL as jxdw,NULL AS yf,mdsflb as sfno 
                                        from mzd3d
                                        left join (select distinct mccfno, mcghno from mzcfd) mzcf on mccfno = mdcfno
                                        left join bz09d on b9mbno = mdtwid 
                                        left join bz05d on b5item = b9item where mdtiwe > 0 and mdzsno in ({1})
                                        union all
                                        select b5item yyxmbh, b5name yyxmmc, b5sfam dj, mdtims sl, b5sfam * mdtims je,mdzsno cfh,NULL as gg,NULL as jxdw,NULL AS yf,mdsflb as sfno 
                                        from mzd3d 
                                        left join bz09d on b9mbno = mdwayid left join bz05d on b5item = b9item
                                        left join (select distinct mccfno, mcghno from mzcfd) mzcf on mccfno = mdcfno 
                                        where mdzsno in ({1})
                                        union all
                                        select b5item yyxmbh, b5name yyxmmc, b5sfam dj, mdpqty sl, b5sfam * mdpqty je,mdzsno cfh,NULL as gg,NULL as jxdw,NULL AS yf,mdsflb as sfno 
                                        from mzd3d 
                                        left join bz09d on b9mbno = mdpprid 
                                        left join bz05d on b5item = b9item
                                        left join (select distinct mccfno, mcghno from mzcfd) mzcf on mccfno = mdcfno
                                        where mdpqty > 0 and mdzsno in ({1})
                                        ) m 
                                        left join ybhisdzdr y on m.yyxmbh = y.hisxmbh
                                        group by y.ybxmbh, y.ybxmmc, m.dj, m.yyxmbh, m.yyxmmc, y.sfxmzldm, y.sflbdm,y.jxdm, m.cfh,m.gg,m.jxdw,y.jx,m.yf,m.sfno,y.dw", jzlsh, cfhs);
                ds.Tables.Clear();
                ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                if (ds.Tables[0].Rows.Count == 0)
                {
                    WriteLog(sysdate + "  无费用明细");
                    return new object[] { 0, 0, "无费用明细" };
                }
                #endregion

                #region 处方明细处理
                DataTable dt = ds.Tables[0];
                StringBuilder wdzxms = new StringBuilder();
                List<string> liSQL = new List<string>();
                List<string> liyyxmbh = new List<string>();
                List<string> liyyxmmc = new List<string>();
                List<string> liybxmbm = new List<string>();
                List<string> liybxmmc = new List<string>();
                List<string> licfh = new List<string>();
                decimal sfje_total = 0;
                int cfindex = 0;
                for (int k = 0; k < dt.Rows.Count; k++)
                {
                    DataRow dr = dt.Rows[k];

                    if (dr["ybxmbh"] == DBNull.Value)
                    {
                        wdzxms.Append("项目代码：[" + dr["yyxmbh"].ToString() + "]，名称：[" + dr["yyxmmc"].ToString() + "]未对照，不能上传！");
                    }
                    else
                    {
                        string xmlb = dr["sfxmzldm"].ToString();  //收费项目等级代码
                        string sflb = dr["sflbdm"].ToString();      //收费类别代码
                        string yyxmbh = dr["yyxmbh"].ToString();          //检查项目代码
                        string ybxmbh = dr["ybxmbh"].ToString();          //医保项目编号
                        string ybxmmc = dr["ybxmmc"].ToString();          //医保项目名称
                        string yyxmmc = dr["yyxmmc"].ToString();          //项目名称
                        decimal dj = Convert.ToDecimal(dr["dj"]);
                        decimal sl = Convert.ToDecimal(dr["sl"]);
                        decimal je = Convert.ToDecimal(dr["je"]);
                        sfje_total += je;
                        string jx = dr["jxdm"].ToString();
                        string gg = dr["gg"].ToString();
                        decimal mcyl = 1;
                        string pd = "qd";
                        string yf = "";
                        string dw = "";
                        string txts = "";
                        string zcffbz1 = "";
                        string qezfbz = "";
                        string cfh = dr["cfh"].ToString() + cfindex;
                        cfindex++;
                        string ybcfh = cfsj + k.ToString();
                        liyyxmbh.Add(yyxmbh);
                        liyyxmmc.Add(yyxmmc);
                        liybxmbm.Add(ybxmbh);
                        liybxmmc.Add(ybxmmc);
                        licfh.Add(cfh);

                        string ypjldw = "";

                        if (dr["sfno"].ToString() == "01" || dr["sfno"].ToString() == "02" || dr["sfno"].ToString() == "03")
                        {
                            zcffbz1 = zcffbz;
                            ypjldw = dr["dw"].ToString();
                        }


                        /*
                    1		就诊流水号	VARCHAR2(18)	NOT NULL	同登记时的就诊流水号
                    2		收费项目种类	VARCHAR2(3)	NOT NULL	二级代码
                    3		收费类别	VARCHAR2(3)	NOT NULL	二级代码
                    4		处方号	VARCHAR2(20)	NOT NULL	
                    5		处方日期	VARCHAR2(14)	NOT NULL	YYYYMMDDHH24MISS
                    6		医院收费项目内码	VARCHAR2(20)	NOT NULL	
                    7		收费项目中心编码	VARCHAR2(20)	NOT NULL	中心编号
                    8		医院收费项目名称	VARCHAR2(50)	NOT NULL	
                    9		单价	VARCHAR2(12)	NOT NULL	4位小数
                    10		数量	VARCHAR2(12)	NOT NULL	2位小数
                    11		金额	VARCHAR2(12)	NOT NULL	4位小数，金额与 (单价*数量)的差值不能大于0.01
                    12		剂型	VARCHAR2(20)		二级代码(收费类别为西药和中成药时必传)
                    13		规格	VARCHAR2(100)		二级代码(收费类别为西药和中成药时必传)
                    14		每次用量	VARCHAR2(12)	NOT NULL	2位小数
                    15		使用频次	VARCHAR2(20)	NOT NULL	
                    16		医师编码	VARCHAR2(20)	NOT NULL	
                    17		医师姓名	VARCHAR2(50)	NOT NULL	
                    18		用法	VARCHAR2(100)		
                    19		单位	VARCHAR2(20)		
                    20		科室编号	VARCHAR2(20)	NOT NULL	
                    21		科室名称	VARCHAR2(50)	NOT NULL	
                    22		执行天数	VARCHAR2(4)		
                    23		草药单复方标志	VARCHAR2(3)		二级代码
                    24		经办人	VARCHAR2(20)	NOT NULL	医疗机构操作员姓名
                    25		药品剂量单位	VARCHAR2(20)		当上传药品时非空
                    26		全额自费标志	VARCHAR2(3)		当传入自费标志，认为本项目按照自费处理，针对限制用药使用
                    27	个人编号	VARCHAR2(10)	NOT NULL	异地结算
                    28	姓名	VARCHAR2(20)	NOT NULL	异地结算
                    29	卡号	VARCHAR2(18)	NOT NULL	异地结算
                     */
                        inputParam.Append(ybjzlsh + "|");
                        inputParam.Append(xmlb + "|");
                        inputParam.Append(sflb + "|");
                        inputParam.Append(cfh + "|");
                        inputParam.Append(cfsj + "|");
                        inputParam.Append(yyxmbh + "|");
                        inputParam.Append(ybxmbh + "|");
                        inputParam.Append(yyxmmc + "|");
                        inputParam.Append(dj + "|");
                        inputParam.Append(sl + "|");
                        inputParam.Append(je + "|");
                        inputParam.Append(jx + "|");
                        inputParam.Append(gg + "|");
                        inputParam.Append(mcyl + "|");
                        inputParam.Append(pd + "|");
                        inputParam.Append(cfysdm + "|");
                        inputParam.Append(cfysxm + "|");
                        inputParam.Append(yf + "|");
                        inputParam.Append(dw + "|");
                        inputParam.Append(ksbm + "|");
                        inputParam.Append(ksmc + "|");
                        inputParam.Append(txts + "|");
                        inputParam.Append(zcffbz1 + "|");
                        inputParam.Append(jbr + "|");
                        inputParam.Append(ypjldw + "|");
                        inputParam.Append(qezfbz + "|");
                        inputParam.Append(grbh + "|");
                        inputParam.Append(xm + "|");
                        inputParam.Append(kh + "|$");

                        strSql = string.Format(@"insert into ybcfmxscindr(jzlsh,jylsh,xm,kh,ybjzlsh,cfrq,yysfxmbm,yysfxmmc,sfxmzxbm,sfxmzxmc,
                                            dj,sl,je,jbr,sysdate,sflb,ybcfh) values(
                                            '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}',
                                            '{10}','{11}','{12}','{13}','{14}','{15}','{16}')",
                                            jzlsh, JYLSH, xm, kh, ybjzlsh, cfsj, yyxmbh, yyxmmc, ybxmbh, ybxmmc,
                                            dj, sl, je, jbr, sysdate, sflb, cfh);
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

                #region 处方上传
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
                        /*
                           1		金额	VARCHAR2(16)		4位小数
                           2		自理金额(自付金额)	VARCHAR2(16)		4位小数
                           3		自费金额	VARCHAR2(16)		4位小数
                           4		超限价自付金额	VARCHAR2(16)		4位小数，包含在自费金额中
                           5		收费类别	VARCHAR2(3)		二级代码
                           6		收费项目等级	VARCHAR2(3)		二级代码
                           7		全额自费标志	VARCHAR2(3)		二级代码
                           8		自理比例（自付比例）	VARCHAR2(5)		4位小数
                           9		限价	VARCHAR2(16)		4位小数
                           10		备注	VARCHAR2(200)		说明出现自费自理金额的原因，his端必须将此信息显示到前台。
                           1	    疑点状态	VARCHAR2(3)		0：无疑点   1：有疑点
                           2	    疑点（包含疑点ID和疑点说明）	VARCHAR2(4000)		疑点可能是多条，用“|”分隔，每条疑点里的疑点ID和疑点说明用“~”分隔例如：#疑点状态|疑点ID~疑点说明|疑点ID~疑点说明
                        */

                        string[] cffh = outputData.ToString().Split('^')[2].Split('#');
                        string[] cfmx = cffh[0].Split('|');
                        outParams_fymx op = new outParams_fymx();
                        op.Je = cfmx[0];
                        op.Zlje = cfmx[1];
                        op.Zfje = cfmx[2];
                        op.Cxjzfje = cfmx[3];
                        op.Sfxmlb = cfmx[4];
                        op.Sfxmdj = cfmx[5];
                        op.Zfbz = cfmx[6];
                        op.Zlbz = cfmx[7];
                        op.Xjbz = cfmx[8]; //限价
                        op.Bz = cfmx[9];

                        if (cffh.Length > 1)
                            op.Bz += cffh[1];

                        strSql = string.Format(@"insert into ybcfmxscfhdr(jzlsh,jylsh,ybjzlsh,yyxmdm,yyxmmc,yybxmbh,ybxmmc,je,zlje,zfje,
                                        cxjzfje,sflb,sfxmdj,qezfbz,zlbl,xj,bz,ybcfh) 
                                        values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}',
                                        '{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}')",
                                        jzlsh, JYLSH, ybjzlsh, liyyxmbh[j], liyyxmmc[j], liybxmbm[j], liybxmmc[j], op.Je, op.Zlje, op.Zfje,
                                        op.Cxjzfje, op.Sfxmlb, op.Sfxmdj, op.Zfbz, op.Zlbz, op.Xjbz, op.Bz, licfh[j]);
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
                #endregion
            }
            catch (Exception error)
            {
                WriteLog(sysdate + "  门诊入方明细上传(市本级)成功|系统异常|" + error.Message);
                return new object[] { 0, 2, "Error:" + error.Message };
            }
        }
        #endregion

        #region 门诊处方明细上传(异地医保)
        public static object[] YBMZCFMXSC_YD(object[] objParam)
        {
            string sysdate = GetServerDateTime();
            WriteLog(sysdate + "  进入门诊入方明细上传(异地医保)...");
            try
            {
                YWZQH = CliUtils.fLoginYbNo.Split('|')[0].ToString();    //业务周期号
            }
            catch
            {
                return new object[] { 0, 0, "医保未连接或初始化失败" };
            }

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
                string jbr = CliUtils.fUserName;    //经办人姓名
                string cfsj = DateTime.Now.ToString("yyyyMMddHHmmss");  //处方时间
                JYLSH = cfsj + "-" + YLGHBH + "-" + new Random().Next(100).ToString().PadLeft(4, '0');

                decimal sfje = Convert.ToDecimal(ylfze);

                #region 是否医保登记
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
                string ksbm = dr1["ksbh"].ToString();
                string ksmc = dr1["ksmc"].ToString();
                string ybjzlsh_snyd = dr1["ybjzlsh_snyd"].ToString();
                #endregion

                StringBuilder inputParam = new StringBuilder();

                #region  判断草药单复方标志
                string zcffbz = "";//草药单复方标志
                strSql = string.Format(@"select mcmzno as mcmzno from mzcfd where  mccfno LIKE 'C%' and  mcghno='{0}' AND mccfno IN ({1})", jzlsh, cfhs);
                ds.Tables.Clear();
                ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                if (ds.Tables[0].Rows.Count > 1)
                    zcffbz = "1";
                else if (ds.Tables[0].Rows.Count == 1)
                    zcffbz = "0";
                #endregion

                #region 获取处方明细信息
                strSql = string.Format(@"select y.ybxmbh, y.ybxmmc, m.dj, sum(m.sl) sl, sum(m.je) je, m.yyxmbh, m.yyxmmc, y.sfxmzldm, y.sflbdm,y.jxdm, m.cfh,m.gg,m.jxdw,y.jx,m.yf,m.sfno from 
                                        (
                                        --药品
                                        select a.mcypno yyxmbh, a.mcypnm yyxmmc, a.mcpric dj, a.mcquty sl, a.mcamnt je, a.mccfno cfh,a.mcypgg gg,a.mcunt1 as jxdw,a.mcwayx as yf,a.mcsflb as sfno
                                        from mzcfd a  
                                        where a.mcghno = '{0}' and a.mccfno in ({1})
                                        union all
                                        --检查/治疗
                                        select b.mbitem yyxmbh, b.mbname yyxmmc, b.mbpric dj, b.mbquty sl, b.mbsjam je,b.mbzlno cfh,NULL as gg,NULL as jxdw,NULL AS yf,b.mbsfno as sfno         
                                        from mzb2d b 
                                        where b.mbghno = '{0}' and b.mbzlno in ({1})
                                        union all
                                        --检验
                                        select c.mbitem yyxmbh, c.mbname yyxmmc, c.mbpric dj, c.mbquty sl, c.mbsjam je, c.mbzlno cfh,NULL as gg,NULL as jxdw,NULL AS yf,c.mbsfno as sfno 
                                        from mzb4d c 
                                        where c.mbghno = '{0}' and c.mbzlno in ({1})
                                        union all
                                        --注射
                                        select b5item yyxmbh, b5name yyxmmc, b5sfam dj, mddays sl, b5sfam * mddays je, mdzsno cfh,NULL as gg,NULL as jxdw,NULL AS yf,mdsflb as sfno 
                                        from mzd3d
                                        left join (select distinct mccfno, mcghno from mzcfd) mzcf on mccfno = mdcfno
                                        left join bz09d on b9mbno = mdtwid 
                                        left join bz05d on b5item = b9item where mdtiwe > 0 and mdzsno in ({1})
                                        union all
                                        select b5item yyxmbh, b5name yyxmmc, b5sfam dj, mdtims sl, b5sfam * mdtims je,mdzsno cfh,NULL as gg,NULL as jxdw,NULL AS yf,mdsflb as sfno 
                                        from mzd3d 
                                        left join bz09d on b9mbno = mdwayid left join bz05d on b5item = b9item
                                        left join (select distinct mccfno, mcghno from mzcfd) mzcf on mccfno = mdcfno 
                                        where mdzsno in ({1})
                                        union all
                                        select b5item yyxmbh, b5name yyxmmc, b5sfam dj, mdpqty sl, b5sfam * mdpqty je,mdzsno cfh,NULL as gg,NULL as jxdw,NULL AS yf,mdsflb as sfno 
                                        from mzd3d 
                                        left join bz09d on b9mbno = mdpprid 
                                        left join bz05d on b5item = b9item
                                        left join (select distinct mccfno, mcghno from mzcfd) mzcf on mccfno = mdcfno
                                        where mdpqty > 0 and mdzsno in ({1})
                                        ) m 
                                        left join ybhisdzdr y on m.yyxmbh = y.hisxmbh
                                        group by y.ybxmbh, y.ybxmmc, m.dj, m.yyxmbh, m.yyxmmc, y.sfxmzldm, y.sflbdm,y.jxdm, m.cfh,m.gg,m.jxdw,y.jx,m.yf,m.sfno", jzlsh, cfhs);
                ds.Tables.Clear();
                ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                if (ds.Tables[0].Rows.Count == 0)
                {
                    WriteLog(sysdate + "  无费用明细");
                    return new object[] { 0, 0, "无费用明细" };
                }
                #endregion

                #region 处方明细处理
                DataTable dt = ds.Tables[0];
                StringBuilder wdzxms = new StringBuilder();
                List<string> liSQL = new List<string>();
                List<string> liyyxmbh = new List<string>();
                List<string> liyyxmmc = new List<string>();
                List<string> liybxmbm = new List<string>();
                List<string> liybxmmc = new List<string>();
                List<string> licfh = new List<string>();
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
                        string xmlb = dr["sfxmzldm"].ToString();  //收费项目等级代码
                        string sflb = dr["sflbdm"].ToString();      //收费类别代码
                        string yyxmbh = dr["yyxmbh"].ToString();          //检查项目代码
                        string ybxmbh = dr["ybxmbh"].ToString();          //医保项目编号
                        string ybxmmc = dr["ybxmmc"].ToString();          //医保项目名称
                        string yyxmmc = dr["yyxmmc"].ToString();          //项目名称
                        decimal dj = Convert.ToDecimal(dr["dj"]);
                        decimal sl = Convert.ToDecimal(dr["sl"]);
                        decimal je = Convert.ToDecimal(dr["je"]);
                        sfje_total += je;
                        string jx = dr["jxdm"].ToString();
                        string gg = dr["gg"].ToString();
                        decimal mcyl = 1;
                        string pd = "qd";
                        string yf = "";
                        string dw = "";
                        string txts = "";
                        string zcffbz1 = "";
                        string qezfbz = "";
                        string cfh = dr["cfh"].ToString();
                        string ybcfh = cfsj + k.ToString();
                        liyyxmbh.Add(yyxmbh);
                        liyyxmmc.Add(yyxmmc);
                        liybxmbm.Add(ybxmbh);
                        liybxmmc.Add(ybxmmc);
                        licfh.Add(cfh);

                        string ypjldw = "";

                        if (dr["sfno"].ToString() == "01" || dr["sfno"].ToString() == "02" || dr["sfno"].ToString() == "03")
                        {
                            zcffbz1 = zcffbz;
                            ypjldw = "粒";
                        }


                        /*
                        0	就诊流水号	VARCHAR2(20)	NOT NULL	唯一
                        1	收费项目种类	VARCHAR2(3)	NOT NULL	二级代码
                        2	费用类别	VARCHAR2(3)	NOT NULL	二级代码
                        3	处方号	VARCHAR2(20)	NOT NULL	非处方药没有处方号，统一传AAAA
                        4	处方日期	VARCHAR2(14)	NOT NULL	YYYYMMDDHH24MISS，非处方药同交易日期
                        5	医院收费项目内码	VARCHAR2(20)		
                        6	收费项目中心编码	VARCHAR2(20)	NOT NULL	中心编号
                        7	医院收费项目名称	VARCHAR2(50)		
                        8	单价	VARCHAR2(12)	NOT NULL	4位小数
                        9	数量	VARCHAR2(12)	NOT NULL	2位小数
                        10	金额	VARCHAR2(12)	NOT NULL	4位小数，金额与 (单价*数量)的差值不能大于0.01
                        11	剂型	VARCHAR2(20)		
                        12	规格	VARCHAR2(100)		
                        13	每次用量	VARCHAR2(12)		2位小数
                        14	使用频次	VARCHAR2(20)		
                        15	医师编号	VARCHAR2(20)		
                        16	医师姓名	VARCHAR2(20)		
                        17	用法	VARCHAR2(20)		
                        18	单位	VARCHAR2(20)		
                        19	科室编号	VARCHAR2(20)		
                        20	科室名称	VARCHAR2(20)		
                        21	限定天数	VARCHAR2(4)		
                        22	草药单复方标志	VARCHAR2(3)		二级代码
                        23	经办人	VARCHAR2(20)		操作员姓名
                        24	药品剂量单位	VARCHAR2(20)		
                        25	自费标志	VARCHAR2(3)		按照自费项目处理
                     */
                        inputParam.Append(ybjzlsh_snyd + "|");
                        inputParam.Append(xmlb + "|");
                        inputParam.Append(sflb + "|");
                        inputParam.Append(cfh + "|");
                        inputParam.Append(cfsj + "|");
                        inputParam.Append(yyxmbh + "|");
                        inputParam.Append(ybxmbh + "|");
                        inputParam.Append(yyxmmc + "|");
                        inputParam.Append(dj + "|");
                        inputParam.Append(sl + "|");
                        inputParam.Append(je + "|");
                        inputParam.Append(jx + "|");
                        inputParam.Append(gg + "|");
                        inputParam.Append(mcyl + "|");
                        inputParam.Append(pd + "|");
                        inputParam.Append(cfysdm + "|");
                        inputParam.Append(cfysxm + "|");
                        inputParam.Append(yf + "|");
                        inputParam.Append(dw + "|");
                        inputParam.Append(ksbm + "|");
                        inputParam.Append(ksmc + "|");
                        inputParam.Append(txts + "|");
                        inputParam.Append(zcffbz1 + "|");
                        inputParam.Append(jbr + "|");
                        inputParam.Append(ypjldw + "|");
                        inputParam.Append(qezfbz + "|$");
                    }
                }

                if (wdzxms.Length > 0)
                {
                    return new object[] { 0, 0, wdzxms.ToString() };
                }

                if (Math.Abs(sfje_total - sfje) > 1)
                    return new object[] { 0, 0, "收费金额与医保结算金额相差" + Math.Abs(sfje_total - sfje) + ",无法结算，请核实!" };
                #endregion

                //返回数据串
                return new object[] { 0, 1, inputParam.ToString().TrimEnd('$'), liSQL.ToArray() };

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
                YWZQH = CliUtils.fLoginYbNo.Split('|')[0].ToString();    //业务周期号
            }
            catch
            {
                return new object[] { 0, 0, "医保未连接或初始化失败" };
            }
            try
            {
                CZYBH = CliUtils.fLoginUser;  // 操作员工号
                ZXBM = "0000";
                string jbr = CliUtils.fUserName;    // 经办人姓名
                string ybjzlsh = objParam[0].ToString();  // 医保就诊流水号
                string cxjylsh = objParam[1].ToString();  // 撤销交易流水号(全部撤销则传空字符串)
                string grbh = objParam[2].ToString();
                string xm = objParam[3].ToString();
                string kh = objParam[4].ToString();

                JYLSH = DateTime.Now.ToString("yyyyMMddHHmmss") + "-" + YLGHBH + "-" + new Random().Next(100).ToString().PadLeft(4, '0');

                /*
                1	就诊流水号	VARCHAR2(20)	NOT NULL	
                2	被撤销医院交易流水号	VARCHAR2(30)		如果只撤销一部分，则此值不为空
                3	经办人	VARCHAR2(50)	NOT NULL	姓名
                4	个人编号	VARCHAR2(10)	NOT NULL	异地结算
                5	姓名	VARCHAR2(20)	NOT NULL	异地结算
                6	卡号	VARCHAR2(18)	NOT NULL	异地结算
                 */
                YWBH = "2320";
                StringBuilder inputParam = new StringBuilder();
                inputParam.Append(ybjzlsh + "|");
                inputParam.Append("|");
                inputParam.Append(jbr + "|");
                inputParam.Append(grbh + "|");
                inputParam.Append(xm + "|");
                inputParam.Append(kh + "|");

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
                    string strSql = string.Format(@"delete from ybcfmxscindr where ybjzlsh = '{0}' and cxbz = 1", ybjzlsh);
                    if (!string.IsNullOrEmpty(cxjylsh))
                        strSql += string.Format(" and jylsh in({0})", cxjylsh);
                    liSQL.Add(strSql);

                    strSql = string.Format(@"delete from ybcfmxscfhdr where ybjzlsh = '{0}' and cxbz = 1", ybjzlsh); 
                    if (!string.IsNullOrEmpty(cxjylsh))
                        strSql += string.Format(" and jylsh in({0})", cxjylsh);
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

        #region 门诊费用预结算
        public static object[] YBMZSFYJS(object[] objParam)
        {
            string sysdate = GetServerDateTime();
            WriteLog(sysdate + "  进入门诊费用预结算...");
            try
            {
                YWZQH = CliUtils.fLoginYbNo.Split('|')[0].ToString();    //业务周期号
            }
            catch
            {
                return new object[] { 0, 0, "医保未连接或初始化失败" };
            }
            try
            {
                #region his入参
                CZYBH = CliUtils.fLoginUser;  //操作员工号
                ZXBM = "0000";
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
                string jsrq1 = Convert.ToDateTime(jsrq).ToString("yyyyMMddHHmmss");
                JYLSH = jsrq + "-" + YLGHBH + "-" + new Random().Next(100).ToString().PadLeft(4, '0');

                #region 是否医保登记
                strSql = string.Format("select * from ybmzzydjdr a where a.ybjzlsh = '{0}'  and a.jzbz='m' and a.cxbz = 1", ybjzlsh);
                ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);

                if (ds == null || ds.Tables[0].Rows.Count == 0)
                    return new object[] { 0, 0, "就诊流水号" + jzlsh + "无挂号或入院登记记录" };
                DataRow dr2 = ds.Tables[0].Rows[0];
                bzbh = dr2["bzbm"].ToString();
                bzmc = dr2["bzmc"].ToString();
                string grbh = dr2["grbh"].ToString();
                string kh = dr2["kh"].ToString();
                string xm = dr2["xm"].ToString();
                string ydrybz = dr2["ydrybz"].ToString();
                string ybjzlsh_snyd = dr2["ybjzlsh_snyd"].ToString();
                string dqbh = dr2["tcqh"].ToString();
                DQJBBZ = dr2["DQJBBZ"].ToString();
                #endregion

                #region 判断未撤销的处方明细
                strSql = string.Format(@"select a.jylsh from ybcfmxscindr a  
                                        where jylsh not in(select cfmxjylsh from ybfyjsdr where ybjzlsh=a.ybjzlsh and cxbz=1) 
                                        and a.ybjzlsh='{0}' and a.cxbz=1", ybjzlsh);
                ds.Tables.Clear();
                ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                foreach(DataRow dr1 in ds.Tables[0].Rows)
                    cfmxjylsh += "'" + dr1["jylsh"].ToString() + "',";
                ds.Dispose();
                #endregion

                if (DQJBBZ.Equals("1")) //市本级
                {
                    #region 费用上传
                    //撤销处方
                    object[] objFYMXCX = { ybjzlsh, cfmxjylsh.TrimEnd(','), grbh, xm, kh };
                    YBMZCFMXSCCX(objFYMXCX);
                    //上传处方
                    object[] objFYMXSC = { jzlsh, cfhs, ybjzlsh, sfje, cfysdm, cfysxm };
                    objFYMXSC = YBMZCFMXSC(objFYMXSC);
                    if (objFYMXSC[1].ToString().Equals("1"))
                        cfmxjylsh = objFYMXSC[2].ToString();
                    else
                        return objFYMXSC;
                    #endregion

                    #region 预结算 &入参
                    /*
                        1		就诊流水号	VARCHAR2(20)	NOT NULL	同登记时的就诊流水号
                        2		单据号	VARCHAR2(20)	NOT NULL	固定传“0000”
                        3		医疗类别	VARCHAR2(3)	NOT NULL	二级代码
                        4		结算日期	VARCHAR2(14)	NOT NULL	YYYYMMDDHH24MISS
                        5		出院日期	VARCHAR2(14)	NOT NULL	YYYYMMDDHH24MISS
                        6		出院原因	VARCHAR2(3)	NOT NULL	二级代码
                        7		病种编码	VARCHAR2(20)		门诊大病、慢病、住院不能为空。
                        8		病种名称	VARCHAR2(50)		有病种编码时，对应的病种名称不能为空。
                        9		账户使用标志	VARCHAR2(3)	NOT NULL	二级代码 【景德镇】普通门诊和药店购药使用账户支付，普通住院不允许使用账户支付，门诊慢性病1类不允许使用个人账户支付，二类门诊慢性病强制使用账户支付个人负担部分，账户余额不足使用现金支付
                        10		中途结算标志	VARCHAR2(3)		二级代码
                        11		经办人	VARCHAR2(50)	NOT NULL	医疗机构操作员姓名
                        12		开发商标志	VARCHAR2(20)	NOT NULL	HIS开发商自定义的特殊标记，能够区分出不同的开发商即可 开发商标志： 01     东软 02     中软 03     其他
                        13		次要诊断编码1	VARCHAR2(20)		
                        14		次要诊断名称1	VARCHAR2(50)		
                        15		次要诊断编码2	VARCHAR2(20)		
                        16		次要诊断名称2	VARCHAR2(50)		
                        17		次要诊断编码3	VARCHAR2(20)		
                        18		次要诊断名称3	VARCHAR2(50)		
                        19		次要诊断编码4	VARCHAR2(20)		
                        20		次要诊断名称4	VARCHAR2(50)		
                        21		治疗方法	VARCHAR2(3)	医疗类别：单病种	代码为：01常规手术 02鼻内镜 03膀胱镜 04 腹腔镜手术 05 使用肛肠吻合器
                        22	个人编号	VARCHAR2(10)	NOT NULL	异地结算
                        23	姓名	VARCHAR2(20)	NOT NULL	异地结算
                        24	卡号	VARCHAR2(18)	NOT NULL	异地结算
                        25	胎儿数	VARCHAR2(3)	NOT NULL	生育住院时，生产病种必须上传。
                        26	手术类型	VARCHAR2(3)		省本级 二级代码 0-	非手术 1-手术 （定点做住院类（普通住院、放化疗住院）结算与预结算交易时必须传入手术类型（0-非手术 1-手术），其他情况可为空。）
                     */
                    StringBuilder inputParam = new StringBuilder();
                    inputParam.Append(ybjzlsh + "|");
                    inputParam.Append(djh + "|");
                    inputParam.Append(yllb + "|");
                    inputParam.Append(jsrq1 + "|");
                    inputParam.Append(jsrq1 + "|");
                    inputParam.Append("9|");
                    inputParam.Append(bzbh + "|");
                    inputParam.Append(bzmc + "|");
                    inputParam.Append(zhsybz + "|");
                    inputParam.Append("0" + "|");
                    inputParam.Append(jbr + "|"); //经办人
                    inputParam.Append("03" + "|");
                    inputParam.Append("|");
                    inputParam.Append("|");
                    inputParam.Append("|");
                    inputParam.Append("|");
                    inputParam.Append("|");
                    inputParam.Append("|");
                    inputParam.Append("|");
                    inputParam.Append("|");
                    inputParam.Append("|");
                    inputParam.Append(grbh + "|");
                    inputParam.Append(xm + "|");
                    inputParam.Append(kh + "|");
                    inputParam.Append("" + "|");    //胎儿数
                    inputParam.Append(sslxdm + "|"); //手术类型

                    YWBH = "2420";
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

                    #region 预结算
                    StringBuilder outputData = new StringBuilder(10240);
                    int i = BUSINESS_HANDLE(inputData, outputData);
                    WriteLog(sysdate + "  门诊费用预结算(市本级)|出参|" + outputData.ToString());
                    if (i != 0)
                    {
                        WriteLog(sysdate + "  门诊费用预结算(市本级)失败|" + outputData.ToString());
                        return new object[] { 0, 0, outputData.ToString() };
                    }
                    #endregion

                    #region 出参
                    string[] sfjsfh = outputData.ToString().Split('^')[2].Split('|');
                    outParams_js js = new outParams_js();
                    /*
                 1		医疗费总额	VARCHAR2(16)		2位小数
                2		总报销金额	VARCHAR2(16)		总报销金额+现金=医疗费总额，2位小数
                3		统筹基金支付	VARCHAR2(16)		2位小数
                4		大额基金支付	VARCHAR2(16)		2位小数
                5		账户支付	VARCHAR2(16)		2位小数
                6		现金支付	VARCHAR2(16)		2位小数
                7		公务员补助基金支付	VARCHAR2(16)		2位小数
                8		企业补充医疗保险基金支付（九江、萍乡、鹰潭疾病补充保险支付）	VARCHAR2(16)		2位小数
                9		自费费用	VARCHAR2(16)		2位小数
                10		单位负担费用	VARCHAR2(16)		2位小数
                11		医院负担费用	VARCHAR2(16)		2位小数
                12		民政救助费用	VARCHAR2(16)		2位小数
                13		超限价费用	VARCHAR2(16)		2位小数
                14		乙类自理费用	VARCHAR2(16)		2位小数
                15		丙类自理费用	VARCHAR2(16)		2位小数
                16		符合基本医疗费用	VARCHAR2(16)		2位小数
                17		起付标准费用	VARCHAR2(16)		2位小数
                18		转诊转院自付费用	VARCHAR2(16)		2位小数
                19		进入统筹费用	VARCHAR2(16)		2位小数
                20		统筹分段自付费用	VARCHAR2(16)		2位小数
                21		超统筹封顶线费用	VARCHAR2(16)		2位小数
                22		进入大额报销费用	VARCHAR2(16)		2位小数
                23		大额分段自付费用	VARCHAR2(16)		2位小数
                24		超大额封顶线费用	VARCHAR2(16)		2位小数
                25		人工器官自付费用	VARHCAR2(16)		2位小数
                26		本次结算前帐户余额	VARHCAR2(16)		2位小数
                27		本年统筹支付累计(不含本次)	VARHCAR2(16)		2位小数
                28		本年大额支付累计(不含本次)	VARHCAR2(16)		2位小数
                29		本年城镇居民门诊统筹支付累计(不含本次)	VARHCAR2(16)		2位小数
                30		本年公务员补助支付累计(不含本次)	VARHCAR2(16)		2位小数
                31		本年账户支付累计(不含本次)	VARCHAR2(16)		2位小数
                32		本年住院次数累计(不含本次)	VARCHAR2(16)		2位小数
                33		住院次数	VARCHAR2(5)		
                34		姓名	VARCHAR2(50)		
                35		结算日期	VARCHAR2(14)		YYYYMMDDHH24MISS
                36		医疗类别	VARCHAR2(3)		二级代码
                37		医疗待遇类别	VARCHAR2(3)		二级代码
                38		经办机构编码	VARCHAR2(16)		二级代码
                39		业务周期号	VARCHAR2(36)		
                40		结算流水号	VARCHAR2(20)		获取结算单交易的入参
                41		提示信息	VARCHAR2(200)		His端必须将此信息显示到前台
                42		单据号	VARCHAR2(20)		
                43		交易类型	VARCHAR2(3)		二级代码 
                44		医院交易流水号	VARCHAR2(50)		
                45		有效标志	VARCHAR2(3)		二级代码 
                46		个人编号管理	VARCHAR2(10)		
                47		医疗机构编码	VARCHAR2(20)		
                48		二次补偿金额	VARCHAR2(16)		2位小数
                49		门慢起付累计	VARCHAR2(16)		2位小数
                50		接收方交易流水号	VARCHAR2(50)		
                51		个人编号	VARCHAR2(16)		
                52		单病种补差	VARCHAR2(16)		2位小数【鹰潭专用】
                53		财政支出金额	VARCHAR2(16)		【萍乡】公立医院用【2位小数】
                54		二类门慢限额支出（景德镇）
                门慢限额支出（省本级）	VARCHAR2(16)		【景德镇】【省本级】专用
                55		二类门慢限额剩余	VARCHAR2(16)		【景德镇】【省本级】专用
                56		居民二次补偿（大病支付）	VARCHAR2(16)		【鹰潭】专用2位小数
                57		体检金额	VARCHAR2(16)		【九江】专用2位小数
                58		生育基金支付	VARCHAR2(16)		
                59		居民大病一段金额	VARCHAR2(16)		【九江、鹰潭居民】专用2位小数
                60		居民大病二段金额	VARCHAR2(16)		【九江、鹰潭居民】
                61		疾病补充范围内费用支付金额	VARCHAR2(16)		【九江/鹰潭/萍乡居民】
                62		疾病补充保险本次政策范围外费用支付金额	VARCHAR2(16)		【九江/鹰潭/萍乡居民】
                63		美国微笑列车基金支付	VARCHAR2(16)		【九江/鹰潭居民】
                64		九江居民政策范围外可报销费用	VARCHAR2(16)		【九江居民】
                65		政府兜底基金费用	VARCHAR2(16)		【萍乡/鹰潭/九江居民】
                66		免费门诊基金（余江）	VARCHAR2(16)		【鹰潭居民】
                67		建档大病补偿本次一段支付金额	VARCHAR2(16)		【鹰潭居民】
                68		建档大病补偿本次二段支付金额	VARCHAR2(16)		【鹰潭居民】
                69		建档大病补偿本次三段支付金额	VARCHAR2(16)		【鹰潭居民】
                70		建档二次补偿本次一段支付金额	VARCHAR2(16)		【鹰潭居民】
                71		建档二次补偿本次二段支付金额	VARCHAR2(16)		【鹰潭居民】
                72		建档二次补偿本次三段支付金额	VARCHAR2(16)		【鹰潭居民】
                73		疾病补充保险本次政策范围内费用一段支付金额	VARCHAR2(16)		【鹰潭居民】
                74		疾病补充保险本次政策范围内费用二段支付金额	VARCHAR2(16)		【鹰潭居民】
                75		本年政府兜底基金费用累计(不含本次)	VARCHAR2(16)		【九江居民】
                76		门慢限额	VARCHAR2(16)		【江西省本级】
                77		企业军转干基金支付	VARCHAR2(16)		【鹰潭】
                78		其他基金支付金额	VARCHAR2(16)		异地就医就医地返给定点出参；本地就医返参为0
                79		伤残人员医疗保障基金	VARCHAR2(16)		异地就医就医地返给定点出参；本地就医返参为0
                 */

                    js.Ylfze = sfjsfh[0];         //医疗费总费用
                    js.Zbxje = sfjsfh[1];         //总报销金额
                    js.Tcjjzf = sfjsfh[2];        //统筹支出
                    js.Dejjzf = sfjsfh[3];        //大病支出
                    js.Zhzf = sfjsfh[4];          //本次帐户支付
                    js.Xjzf = sfjsfh[5];         //个人现金支付
                    js.Gwybzjjzf = sfjsfh[6];     //公务员补助支付金额
                    js.Qybcylbxjjzf = sfjsfh[7];  //企业补充支付金额
                    js.Zffy = sfjsfh[8];          //自费费用
                    js.Dwfdfy = sfjsfh[9];
                    js.Yyfdfy = sfjsfh[10];
                    js.Mzjzfy = sfjsfh[11];       //民政救助费用
                    js.Cxjfy = sfjsfh[12];
                    js.Ylzlfy = sfjsfh[13];
                    js.Blzlfy = sfjsfh[14];
                    js.Fhjjylfy = sfjsfh[15];
                    js.Qfbzfy = sfjsfh[16];
                    js.Zzzyzffy = sfjsfh[17];
                    js.Jrtcfy = sfjsfh[18];
                    js.Tcfdzffy = sfjsfh[19];
                    js.Ctcfdxfy = sfjsfh[20];
                    js.Jrdebxfy = sfjsfh[21];
                    js.Defdzffy = sfjsfh[22];
                    js.Cdefdxfy = sfjsfh[23];
                    js.Rgqgzffy = sfjsfh[24];
                    js.Bcjsqzhye = sfjsfh[25];
                    js.Bntczflj = sfjsfh[26];
                    js.Bndezflj = sfjsfh[27];
                    js.Bnczjmmztczflj = sfjsfh[28];
                    js.Bngwybzzflj = sfjsfh[29];
                    js.Bnzhzflj = sfjsfh[30];
                    js.Bnzycslj = sfjsfh[31];
                    js.Zycs = sfjsfh[32];
                    js.Xm = sfjsfh[33];
                    js.Jsrq = sfjsfh[34];
                    js.Yllb = sfjsfh[35];
                    js.Yldylb = sfjsfh[36];
                    js.Jbjgbm = sfjsfh[37];
                    js.Ywzqh = sfjsfh[38];
                    js.Jslsh = sfjsfh[39];
                    js.Tsxx = sfjsfh[40];
                    js.Djh = sfjsfh[41];
                    js.Jyxl = sfjsfh[42];
                    js.Yyjylsh = sfjsfh[43];
                    js.Yxbz = sfjsfh[44];
                    js.Grbhgl = sfjsfh[45];
                    js.Yljgbm = sfjsfh[46];
                    js.Ecbcje = sfjsfh[47];
                    js.Mmqflj = sfjsfh[48];
                    js.Jsfjylsh = sfjsfh[49];
                    js.Grbh = sfjsfh[50];
                    js.Dbzbc = sfjsfh[51];
                    js.Czzcje = sfjsfh[52];
                    js.Elmmxezc = sfjsfh[53];
                    js.Elmmxesy = sfjsfh[54];
                    js.Jmecbc = sfjsfh[55];
                    js.Tjje = sfjsfh[56];
                    js.Syjjzf = sfjsfh[57];
                    js.Jmdbydje = sfjsfh[58];
                    js.Jmdbedje = sfjsfh[59];
                    js.Jbbcfwnfyzfje = sfjsfh[60];
                    js.Jbbcybbczcfywfyzf = sfjsfh[61];
                    js.Mgwxlcjjzf = sfjsfh[62];
                    js.Jjjmzcfwwkbxfy = sfjsfh[63];
                    js.Zftdfy = sfjsfh[64];
                    js.Mfmzjj = sfjsfh[65];
                    js.Jddbbcbcydzfje = sfjsfh[66];
                    js.Jddbbcbcedzfje = sfjsfh[67];
                    js.Jddbbcbcsdzfje = sfjsfh[68];
                    js.Jdecbcbcydzfje = sfjsfh[69];
                    js.Jdecbcbcedzfje = sfjsfh[70];
                    js.Jdecbcbcsdzfje = sfjsfh[71];
                    js.Jbbcbxbczcfwnfyydzfje = sfjsfh[72];
                    js.Jbbcbxbczcfwnfyedzfje = sfjsfh[73];
                    js.Bnzftdjjfylj = sfjsfh[74];
                    js.Mmxe = sfjsfh[75];
                    js.Jzgbbzzf = sfjsfh[76];
                    js.Qtjjzf = sfjsfh[77];
                    js.Scryylbzjj = sfjsfh[78];
                    string sMsg = "";
                    if (Convert.ToDecimal(js.Mzjzfy) > 0)
                        sMsg += "结算费用中存在民政救助费用\r\n";
                    if (Convert.ToDecimal(js.Zftdfy) > 0)
                        sMsg += "结算费用中存在政府兜底基金费用\r\n";
                    if (Convert.ToDecimal(js.Dejjzf) > 0)
                        sMsg += "结算费用中存在大病支付费用\r\n";
                    if (Convert.ToDecimal(js.Qybcylbxjjzf) > 0)
                        sMsg += "结算费用中存在商业补充保险支付费用\r\n";
                    if (Convert.ToDecimal(js.Jddbbcbcydzfje) > 0 || Convert.ToDecimal(js.Jddbbcbcedzfje) > 0 || Convert.ToDecimal(js.Jddbbcbcsdzfje) > 0)
                        sMsg += "结算费用中存在建档大病补偿段支付金额\r\n";

                    if(!string.IsNullOrEmpty(sMsg))
                    {
                        MessageBox.Show(sMsg);
                    }

                    string[] sDBRY = new string[] { "80", "83", "84", "85", "86", "87" };
                    if (sDBRY.Contains(js.Yldylb))
                    {
                        /*
                        * 建档立卡人员除个人自负部分的费用外，均先由医院垫付
                        */
                        js.Ybxjzf = js.Xjzf;
                        js.Zhzbxje = (Convert.ToDecimal(js.Ylfze) - Convert.ToDecimal(js.Ybxjzf)).ToString();
                    }
                    else
                    {
                        /*
                         * 非建档立卡人员（含民政救助对象）住院（含门诊慢性病治疗）的医院垫付金额为：
                         * 统筹支出+账户支付+医院负担费用+民政救助费用+二次补偿金额+企业军转干基金支付。
                         */
                        js.Zhzbxje = (Convert.ToDecimal(js.Tcjjzf) + Convert.ToDecimal(js.Zhzf) + Convert.ToDecimal(js.Yyfdfy) + Convert.ToDecimal(js.Mzjzfy)
                                     + Convert.ToDecimal(js.Ecbcje) + Convert.ToDecimal(js.Jzgbbzzf)).ToString();
                        js.Ybxjzf = (Convert.ToDecimal(js.Ylfze) - Convert.ToDecimal(js.Zhzbxje)).ToString();
                    }

                    js.Qtybzf = (Convert.ToDecimal(js.Ylfze) - Convert.ToDecimal(js.Tcjjzf) - Convert.ToDecimal(js.Ybxjzf) - Convert.ToDecimal(js.Zhzf)).ToString(); ;



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
                    string strValue = js.Ylfze + "|" + js.Zhzbxje + "|" + js.Tcjjzf + "|" + js.Dejjzf + "|" + js.Zhzf + "|" +
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

                    WriteLog(sysdate + "  门诊收费预结算|整合出参|" + strValue);
                    return new object[] { 0, 1, strValue };
                    #endregion
                   
                }
                else
                {
                    ZXBM = dqbh;
                    #region 获取费用明细
                    string cfmxxx = "";
                    object[] objFYMXSC = { jzlsh, cfhs, ybjzlsh, sfje, cfysdm, cfysxm };
                    objFYMXSC = YBMZCFMXSC_YD(objFYMXSC);
                    if (objFYMXSC[1].ToString().Equals("1"))
                        cfmxxx = objFYMXSC[2].ToString();
                    else
                        return objFYMXSC;
                    #endregion

                    #region 入参
                    /*
                        1		就诊流水号	VARCHAR2(20)	NOT NULL	唯一
                        2		单据号	VARCHAR2(20)	NULL	预结算为空
                        3		交易日期	VARCHAR2(14)	NOT NULL  	YYYYMMDDHH24MISS
                        4		经办人	VARCHAR2(50)		
                        5		诊断疾病编码	VARCHAR2(20)		
                        6		诊断疾病名称	VARCHAR2(50)		
                        7		医疗类别	VARCHAR2(3)		二级代码
                        8		开发商标志	VARCHAR2(20)	NOT NULL	HIS开发商自定义的特殊标记，能够区分出不同的开发商即可。
                        9		个人编号	VARCHAR2(10)	NOT NULL	异地结算
                        10		姓名	VARCHAR2(20)	NOT NULL	异地结算
                        11		卡号	VARCHAR2(18)	NOT NULL	异地结算                     
                     */
                    StringBuilder inputParam = new StringBuilder();
                    inputParam.Append(ybjzlsh_snyd + "|");
                    inputParam.Append(djh + "|");
                    inputParam.Append(jsrq + "|");
                    inputParam.Append(jbr + "|");
                    inputParam.Append(bzbh + "|");
                    inputParam.Append(bzmc + "|");
                    inputParam.Append(yllb + "|");
                    inputParam.Append("gocent|");
                    inputParam.Append(grbh + "|");
                    inputParam.Append(xm + "|");
                    inputParam.Append(kh + "|$");
                    inputParam.Append(cfmxxx);
                    #endregion

                    #region 预结算
                    StringBuilder inputData = new StringBuilder();
                    YWBH = "2610";
                    inputData.Append(YWBH + "^");
                    inputData.Append(YLGHBH + "^");
                    inputData.Append(CZYBH + "^");
                    inputData.Append(YWZQH + "^");
                    inputData.Append(JYLSH + "^");
                    inputData.Append(ZXBM + "^");
                    inputData.Append(inputParam.ToString() + "^");
                    inputData.Append(LJBZ + "^");
                    WriteLog(sysdate + "   费用预结算(异地医保)|入参|" + inputData.ToString());

                    StringBuilder outputData = new StringBuilder(10240);
                    int i = BUSINESS_HANDLE(inputData, outputData);
                    WriteLog(sysdate + "  费用预结算(异地医保)|出参|" + outputData.ToString());
                    if (i < 0)
                        return new object[] { 0, 0, outputData.ToString().Split('^')[2].ToString() };
                    #endregion

                    #region 出参
                    /*
                        1		医疗费总额	VARCHAR2(16)		2位小数
                        2		总报销金额	VARCHAR2(16)		总报销金额+现金=医疗费总额，2位小数
                        3		统筹基金支付	VARCHAR2(16)		2位小数
                        4		大额基金支付	VARCHAR2(16)		2位小数
                        5		账户支付	VARCHAR2(16)		2位小数
                        6		现金支付	VARCHAR2(16)		2位小数
                        7		公务员补助基金支付	VARCHAR2(16)		2位小数
                        8		企业补充医疗保险基金支付	VARCHAR2(16)		2位小数
                        9		自费费用	VARCHAR2(16)		2位小数
                        10		单位负担费用	VARCHAR2(16)		2位小数
                        11		医院负担费用	VARCHAR2(16)		2位小数
                        12		民政救助费用	VARCHAR2(16)		2位小数
                        13		超限价费用	VARCHAR2(16)		2位小数
                        14		乙类自理费用	VARCHAR2(16)		2位小数
                        15		丙类自理费用	VARCHAR2(16)		2位小数
                        16		符合基本医疗费用	VARCHAR2(16)		2位小数
                        17		起付标准费用	VARCHAR2(16)		2位小数
                        18		转诊转院自付费用	VARCHAR2(16)		2位小数
                        19		进入统筹费用	VARCHAR2(16)		2位小数
                        20		统筹分段自付费用	VARCHAR2(16)		2位小数
                        21		超统筹封顶线费用	VARCHAR2(16)		2位小数
                        22		进入大额报销费用	VARCHAR2(16)		2位小数
                        23		大额分段自付费用	VARCHAR2(16)		2位小数
                        24		超大额封顶线费用	VARCHAR2(16)		2位小数
                        25		人工器官自付费用	VARHCAR2(16)		2位小数
                        26		本次结算前帐户余额	VARHCAR2(16)		2位小数
                        27		本年统筹支付累计(不含本次)	VARHCAR2(16)		2位小数
                        28		本年大额支付累计(不含本次)	VARHCAR2(16)		2位小数
                        29		本年城镇居民门诊统筹支付累计(不含本次)	VARHCAR2(16)		2位小数
                        30		本年公务员补助支付累计(不含本次)	VARHCAR2(16)		2位小数
                        31		本年账户支付累计(不含本次)	VARCHAR2(16)		2位小数
                        32		本年住院次数累计(不含本次)	VARCHAR2(16)		2位小数
                        33		住院次数	VARCHAR2(5)		
                        34		姓名	VARCHAR2(50)		
                        35		结算日期	VARCHAR2(14)		YYYYMMDDHH24MISS
                        36		医疗类别	VARCHAR2(3)		二级代码
                        37		医疗待遇类别	VARCHAR2(3)		二级代码
                        38		经办机构编码	VARCHAR2(16)		二级代码
                        39		业务周期号	VARCHAR2(36)		
                        40		结算流水号	VARCHAR2(20)		获取结算单交易的入参
                        41		提示信息	VARCHAR2(200)		His端必须将此信息显示到前台
                        42		单据号	VARCHAR2(20)		
                        43		交易类型	VARCHAR2(3)		二级代码 
                        44		医院交易流水号	VARCHAR2(50)		
                        45		有效标志	VARCHAR2(3)		二级代码 
                        46		个人编号	VARCHAR2(10)		
                        47		医疗机构编码	VARCHAR2(20)		
                        48		就诊流水号	VARCHAR2(20)		异地撤销要取异地返回的就诊流水号进行撤销
                        49		其他基金支付金额	VARCHAR2(20)		异地就医就医地返回给定点出参（目前只添加了异地就医就医地出参部分，位数按时2420出参位数）
                        50	    伤残人员医疗保障基金	VARCHAR2(16)		异地就医就医地返给定点出参；本地就医返参为0
                     
                     */

                    string[] sfjsfh = outputData.ToString().Split('^')[2].Split('|');
                    outParams_js js = new outParams_js();
                    js.Ylfze = sfjsfh[0];         //医疗费总费用
                    js.Zbxje = sfjsfh[1];         //总报销金额
                    js.Tcjjzf = sfjsfh[2];        //统筹支出
                    js.Dejjzf = sfjsfh[3];        //大病支出
                    js.Zhzf = sfjsfh[4];          //本次帐户支付
                    js.Xjzf = sfjsfh[5];         //个人现金支付
                    js.Gwybzjjzf = sfjsfh[6];     //公务员补助支付金额
                    js.Qybcylbxjjzf = sfjsfh[7];  //企业补充支付金额
                    js.Zffy = sfjsfh[8];          //自费费用
                    js.Dwfdfy = sfjsfh[9];
                    js.Yyfdfy = sfjsfh[10];
                    js.Mzjzfy = sfjsfh[11];       //民政救助费用
                    js.Cxjfy = sfjsfh[12];
                    js.Ylzlfy = sfjsfh[13];
                    js.Blzlfy = sfjsfh[14];
                    js.Fhjjylfy = sfjsfh[15];
                    js.Qfbzfy = sfjsfh[16];
                    js.Zzzyzffy = sfjsfh[17];
                    js.Jrtcfy = sfjsfh[18];
                    js.Tcfdzffy = sfjsfh[19];
                    js.Ctcfdxfy = sfjsfh[20];
                    js.Jrdebxfy = sfjsfh[21];
                    js.Defdzffy = sfjsfh[22];
                    js.Cdefdxfy = sfjsfh[23];
                    js.Rgqgzffy = sfjsfh[24];
                    js.Bcjsqzhye = sfjsfh[25];
                    js.Bntczflj = sfjsfh[26];
                    js.Bndezflj = sfjsfh[27];
                    js.Bnczjmmztczflj = sfjsfh[28];
                    js.Bngwybzzflj = sfjsfh[29];
                    js.Bnzhzflj = sfjsfh[30];
                    js.Bnzycslj = sfjsfh[31];
                    js.Zycs = sfjsfh[32];
                    js.Xm = sfjsfh[33];
                    js.Jsrq = sfjsfh[34];
                    js.Yllb = sfjsfh[35];
                    js.Yldylb = sfjsfh[36];
                    js.Jbjgbm = sfjsfh[37];
                    js.Ywzqh = sfjsfh[38];
                    js.Jslsh = sfjsfh[39];
                    js.Tsxx = sfjsfh[40];
                    js.Djh = sfjsfh[41];
                    js.Jyxl = sfjsfh[42];
                    js.Yyjylsh = sfjsfh[43];
                    js.Yxbz = sfjsfh[44];
                    js.Grbh = sfjsfh[45];
                    js.Yljgbm = sfjsfh[46];
                    js.Ybjzlsh = sfjsfh[47];
                    js.Qtjjzf = sfjsfh[48];
                    js.Scryylbzjj = sfjsfh[49];

                    js.Jmdbydje = "0.00";
                    js.Jmdbedje = "0.00";
                    js.Ecbcje = "0.00";
                    js.Jzgbbzzf = "0.00";

                    string sMsg = "";
                    if (Convert.ToDecimal(js.Mzjzfy) > 0)
                        sMsg += "结算费用中存在民政救助费用\r\n";
                    if (Convert.ToDecimal(js.Zftdfy) > 0)
                        sMsg += "结算费用中存在政府兜底基金费用\r\n";
                    if (Convert.ToDecimal(js.Dejjzf) > 0)
                        sMsg += "结算费用中存在大病支付费用\r\n";
                    if (Convert.ToDecimal(js.Qybcylbxjjzf) > 0)
                        sMsg += "结算费用中存在商业补充保险支付费用\r\n";
                    if (Convert.ToDecimal(js.Jddbbcbcydzfje) > 0 || Convert.ToDecimal(js.Jddbbcbcedzfje) > 0 || Convert.ToDecimal(js.Jddbbcbcsdzfje) > 0)
                        sMsg += "结算费用中存在建档大病补偿段支付金额\r\n";

                    if (!string.IsNullOrEmpty(sMsg))
                    {
                        MessageBox.Show(sMsg);
                    }

                    string[] sDBRY = new string[] { "80", "83", "84", "85", "86", "87" };
                    if (sDBRY.Contains(js.Yldylb))
                    {
                        /*
                        * 建档立卡人员除个人自负部分的费用外，均先由医院垫付
                        */
                        js.Ybxjzf = js.Xjzf;
                        js.Zhzbxje = (Convert.ToDecimal(js.Ylfze) - Convert.ToDecimal(js.Ybxjzf)).ToString();
                    }
                    else
                    {
                        /*
                         * 非建档立卡人员（含民政救助对象）住院（含门诊慢性病治疗）的医院垫付金额为：
                         * 统筹支出+账户支付+医院负担费用+民政救助费用+二次补偿金额+企业军转干基金支付。
                         */
                        js.Zhzbxje = (Convert.ToDecimal(js.Tcjjzf) + Convert.ToDecimal(js.Zhzf) + Convert.ToDecimal(js.Yyfdfy) + Convert.ToDecimal(js.Mzjzfy)
                                     + Convert.ToDecimal(js.Ecbcje) + Convert.ToDecimal(js.Jzgbbzzf)).ToString();
                        js.Ybxjzf = (Convert.ToDecimal(js.Ylfze) - Convert.ToDecimal(js.Zhzbxje)).ToString();
                    }

                    js.Qtybzf = (Convert.ToDecimal(js.Ylfze) - Convert.ToDecimal(js.Tcjjzf) - Convert.ToDecimal(js.Ybxjzf) - Convert.ToDecimal(js.Zhzf)).ToString(); ;


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
                    string strValue = js.Ylfze + "|" + js.Zhzbxje + "|" + js.Tcjjzf + "|" + js.Dejjzf + "|" + js.Zhzf + "|" +
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

                    WriteLog(sysdate + "  门诊收费预结算(异地医保)｜整合出参|" + strValue);
                    return new object[] { 0, 1, strValue };
                    #endregion
                }
            }
            catch (Exception error)
            {
                WriteLog(sysdate + "  系统异常|" + error.Message);
                return new object[] { 0, 2, error.Message };
            }
        }
        #endregion

        #region 门诊费用结算
        public static object[] YBMZSFJS(object[] objParam)
        {
            string sysdate = GetServerDateTime();
            WriteLog(sysdate + "  进入门诊费用结算...");

            try
            {
                YWZQH = CliUtils.fLoginYbNo.Split('|')[0].ToString();    //业务周期号
            }
            catch
            {
                return new object[] { 0, 0, "医保未连接或初始化失败" };
            }
            try
            {
                CZYBH = CliUtils.fLoginUser;  //操作员工号
                ZXBM = "0000";
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
                string cfmxjylsh = "";
                decimal sfje1 = Convert.ToDecimal(sfje);
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

                #region 获取医保登记信息
                strSql = string.Format("select * from ybmzzydjdr a where a.ybjzlsh = '{0}'  and a.jzbz='m' and a.cxbz = 1", ybjzlsh);
                ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);

                if (ds == null || ds.Tables[0].Rows.Count == 0)
                    return new object[] { 0, 0, "无挂号或入院登记记录" };

                DataRow dr2 = ds.Tables[0].Rows[0];
                yllb = dr2["yllb"].ToString();
                bzbm = dr2["bzbm"].ToString();
                bzmc = dr2["bzmc"].ToString();
                string xm = dr2["xm"].ToString();
                string grbh = dr2["grbh"].ToString();
                string kh = dr2["kh"].ToString();
                string ydrybz = dr2["ydrybz"].ToString();
                string ybjzlsh_snyd = dr2["ybjzlsh_snyd"].ToString();
                string dqbh = dr2["tcqh"].ToString();
                string ksbm = dr2["ksbh"].ToString();
                string ksmc = dr2["ksmc"].ToString();
                DQJBBZ = dr2["dqjbbz"].ToString();
                #endregion

                #region 出参初始化
                outParams_js js = new outParams_js();
                js.Ylfze = "0.00";         //医疗费总费用
                js.Zbxje = "0.00";       //总报销金额
                js.Tcjjzf = "0.00";        //统筹支出
                js.Dejjzf = "0.00";       //大病支出
                js.Zhzf = "0.00";         //本次帐户支付
                js.Xjzf = "0.00";         //个人现金支付
                js.Gwybzjjzf = "0.00";     //公务员补助支付金额
                js.Qybcylbxjjzf = "0.00";  //企业补充支付金额
                js.Zffy = "0.00";         //自费费用
                js.Dwfdfy = "0.00";
                js.Yyfdfy = "0.00";
                js.Mzjzfy = "0.00";     //民政救助费用
                js.Cxjfy = "0.00";
                js.Ylzlfy = "0.00";
                js.Blzlfy = "0.00";
                js.Fhjjylfy = "0.00";
                js.Qfbzfy = "0.00";
                js.Zzzyzffy = "0.00";
                js.Jrtcfy = "0.00";
                js.Tcfdzffy = "0.00";
                js.Ctcfdxfy = "0.00";
                js.Jrdebxfy = "0.00";
                js.Defdzffy = "0.00";
                js.Cdefdxfy = "0.00";
                js.Rgqgzffy = "0.00";
                js.Bcjsqzhye = "0.00";
                js.Bntczflj = "0.00";
                js.Bndezflj = "0.00";
                js.Bnczjmmztczflj = "0.00";
                js.Bngwybzzflj = "0.00";
                js.Bnzhzflj = "0.00";
                js.Bnzycslj = "0.00";
                js.Qtjjzf = "0.00";
                js.Scryylbzjj = "0.00";
                js.Ecbcje = "0.00";
                js.Mmqflj = "0.00";
                js.Jsfjylsh = "0.00";
                js.Grbh = "0.00";
                js.Dbzbc = "0.00";
                js.Czzcje = "0.00";
                js.Elmmxezc = "0.00";
                js.Elmmxesy = "0.00";
                js.Jmecbc = "0.00";
                js.Tjje = "0.00";
                js.Syjjzf = "0.00";
                js.Jmdbydje = "0.00";
                js.Jmdbedje = "0.00";
                js.Jbbcfwnfyzfje = "0.00";
                js.Jbbcybbczcfywfyzf = "0.00";
                js.Mgwxlcjjzf = "0.00";
                js.Jjjmzcfwwkbxfy = "0.00";
                js.Zftdfy = "0.00";
                js.Mfmzjj = "0.00";
                js.Jddbbcbcydzfje = "0.00";
                js.Jddbbcbcedzfje = "0.00";
                js.Jddbbcbcsdzfje = "0.00";
                js.Jdecbcbcydzfje = "0.00";
                js.Jdecbcbcedzfje = "0.00";
                js.Jdecbcbcsdzfje = "0.00";
                js.Jbbcbxbczcfwnfyydzfje = "0.00";
                js.Jbbcbxbczcfwnfyedzfje = "0.00";
                js.Bnzftdjjfylj = "0.00";
                js.Mmxe = "0.00";
                js.Jzgbbzzf = "0.00";
                js.Lxgbdttcjjzf = "0.00";
                #endregion

                if (DQJBBZ.Equals("1"))
                {
                    #region 是否处方已上传
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
                    #endregion

                    #region 结算 &入参
                    /*
                        1		就诊流水号	VARCHAR2(20)	NOT NULL	同登记时的就诊流水号
                        2		单据号	VARCHAR2(20)	NOT NULL	固定传“0000”
                        3		医疗类别	VARCHAR2(3)	NOT NULL	二级代码
                        4		结算日期	VARCHAR2(14)	NOT NULL	YYYYMMDDHH24MISS
                        5		出院日期	VARCHAR2(14)	NOT NULL	YYYYMMDDHH24MISS
                        6		出院原因	VARCHAR2(3)	NOT NULL	二级代码
                        7		病种编码	VARCHAR2(20)		门诊大病、慢病、住院不能为空。
                        8		病种名称	VARCHAR2(50)		有病种编码时，对应的病种名称不能为空。
                        9		账户使用标志	VARCHAR2(3)	NOT NULL	二级代码 【景德镇】普通门诊和药店购药使用账户支付，普通住院不允许使用账户支付，门诊慢性病1类不允许使用个人账户支付，二类门诊慢性病强制使用账户支付个人负担部分，账户余额不足使用现金支付
                        10		中途结算标志	VARCHAR2(3)		二级代码
                        11		经办人	VARCHAR2(50)	NOT NULL	医疗机构操作员姓名
                        12		开发商标志	VARCHAR2(20)	NOT NULL	HIS开发商自定义的特殊标记，能够区分出不同的开发商即可 开发商标志： 01     东软 02     中软 03     其他
                        13		次要诊断编码1	VARCHAR2(20)		
                        14		次要诊断名称1	VARCHAR2(50)		
                        15		次要诊断编码2	VARCHAR2(20)		
                        16		次要诊断名称2	VARCHAR2(50)		
                        17		次要诊断编码3	VARCHAR2(20)		
                        18		次要诊断名称3	VARCHAR2(50)		
                        19		次要诊断编码4	VARCHAR2(20)		
                        20		次要诊断名称4	VARCHAR2(50)		
                        21		治疗方法	VARCHAR2(3)	医疗类别：单病种	代码为：01常规手术 02鼻内镜 03膀胱镜 04 腹腔镜手术 05 使用肛肠吻合器
                        22	个人编号	VARCHAR2(10)	NOT NULL	异地结算
                        23	姓名	VARCHAR2(20)	NOT NULL	异地结算
                        24	卡号	VARCHAR2(18)	NOT NULL	异地结算
                        25	胎儿数	VARCHAR2(3)	NOT NULL	生育住院时，生产病种必须上传。
                        26	手术类型	VARCHAR2(3)		省本级 二级代码 0-	非手术 1-手术 （定点做住院类（普通住院、放化疗住院）结算与预结算交易时必须传入手术类型（0-非手术 1-手术），其他情况可为空。）
                     */
                    StringBuilder inputParam = new StringBuilder();
                    inputParam.Append(ybjzlsh + "|");
                    inputParam.Append(djh + "|");
                    inputParam.Append(yllb + "|");
                    inputParam.Append(jsrq + "|");
                    inputParam.Append(jsrq+"|");
                    inputParam.Append("9|");
                    inputParam.Append(bzbm + "|");
                    inputParam.Append(bzmc + "|");
                    inputParam.Append(zhsybz + "|");
                    inputParam.Append("0" + "|");
                    inputParam.Append(jbr + "|"); //经办人
                    inputParam.Append("03" + "|");
                    inputParam.Append("|");
                    inputParam.Append("|");
                    inputParam.Append("|");
                    inputParam.Append("|");
                    inputParam.Append("|");
                    inputParam.Append("|");
                    inputParam.Append("|");
                    inputParam.Append("|");
                    inputParam.Append("|");
                    inputParam.Append(grbh + "|");
                    inputParam.Append(xm + "|");
                    inputParam.Append(kh + "|");
                    inputParam.Append("" + "|");    //胎儿数
                    inputParam.Append(sslxdm + "|"); //手术类型

                    YWBH = "2410";
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

                    #region 费用结算
                    StringBuilder outputData = new StringBuilder(10240);
                    int i = BUSINESS_HANDLE(inputData, outputData);
                    WriteLog(sysdate + "  门诊费用结算(市本级)|出参|" + outputData.ToString());
                    if (i != 0)
                    {
                        WriteLog(sysdate + "  费用结算(市本级)失败|" + outputData.ToString());
                        return new object[] { 0, 0, outputData.ToString() };
                    }
                    #endregion

                    #region 出参
                    string[] sfjsfh = outputData.ToString().Split('^')[2].Split('|');
                    /*
                     1		医疗费总额	VARCHAR2(16)		2位小数
                    2		总报销金额	VARCHAR2(16)		总报销金额+现金=医疗费总额，2位小数
                    3		统筹基金支付	VARCHAR2(16)		2位小数
                    4		大额基金支付	VARCHAR2(16)		2位小数
                    5		账户支付	VARCHAR2(16)		2位小数
                    6		现金支付	VARCHAR2(16)		2位小数
                    7		公务员补助基金支付	VARCHAR2(16)		2位小数
                    8		企业补充医疗保险基金支付（九江、萍乡、鹰潭疾病补充保险支付）	VARCHAR2(16)		2位小数
                    9		自费费用	VARCHAR2(16)		2位小数
                    10		单位负担费用	VARCHAR2(16)		2位小数
                    11		医院负担费用	VARCHAR2(16)		2位小数
                    12		民政救助费用	VARCHAR2(16)		2位小数
                    13		超限价费用	VARCHAR2(16)		2位小数
                    14		乙类自理费用	VARCHAR2(16)		2位小数
                    15		丙类自理费用	VARCHAR2(16)		2位小数
                    16		符合基本医疗费用	VARCHAR2(16)		2位小数
                    17		起付标准费用	VARCHAR2(16)		2位小数
                    18		转诊转院自付费用	VARCHAR2(16)		2位小数
                    19		进入统筹费用	VARCHAR2(16)		2位小数
                    20		统筹分段自付费用	VARCHAR2(16)		2位小数
                    21		超统筹封顶线费用	VARCHAR2(16)		2位小数
                    22		进入大额报销费用	VARCHAR2(16)		2位小数
                    23		大额分段自付费用	VARCHAR2(16)		2位小数
                    24		超大额封顶线费用	VARCHAR2(16)		2位小数
                    25		人工器官自付费用	VARHCAR2(16)		2位小数
                    26		本次结算前帐户余额	VARHCAR2(16)		2位小数
                    27		本年统筹支付累计(不含本次)	VARHCAR2(16)		2位小数
                    28		本年大额支付累计(不含本次)	VARHCAR2(16)		2位小数
                    29		本年城镇居民门诊统筹支付累计(不含本次)	VARHCAR2(16)		2位小数
                    30		本年公务员补助支付累计(不含本次)	VARHCAR2(16)		2位小数
                    31		本年账户支付累计(不含本次)	VARCHAR2(16)		2位小数
                    32		本年住院次数累计(不含本次)	VARCHAR2(16)		2位小数
                    33		住院次数	VARCHAR2(5)		
                    34		姓名	VARCHAR2(50)		
                    35		结算日期	VARCHAR2(14)		YYYYMMDDHH24MISS
                    36		医疗类别	VARCHAR2(3)		二级代码
                    37		医疗待遇类别	VARCHAR2(3)		二级代码
                    38		经办机构编码	VARCHAR2(16)		二级代码
                    39		业务周期号	VARCHAR2(36)		
                    40		结算流水号	VARCHAR2(20)		获取结算单交易的入参
                    41		提示信息	VARCHAR2(200)		His端必须将此信息显示到前台
                    42		单据号	VARCHAR2(20)		
                    43		交易类型	VARCHAR2(3)		二级代码 
                    44		医院交易流水号	VARCHAR2(50)		
                    45		有效标志	VARCHAR2(3)		二级代码 
                    46		个人编号管理	VARCHAR2(10)		
                    47		医疗机构编码	VARCHAR2(20)		
                    48		二次补偿金额	VARCHAR2(16)		2位小数
                    49		门慢起付累计	VARCHAR2(16)		2位小数
                    50		接收方交易流水号	VARCHAR2(50)		
                    51		个人编号	VARCHAR2(16)		
                    52		单病种补差	VARCHAR2(16)		2位小数【鹰潭专用】
                    53		财政支出金额	VARCHAR2(16)		【萍乡】公立医院用【2位小数】
                    54		二类门慢限额支出（景德镇）门慢限额支出（省本级）	VARCHAR2(16)		【景德镇】【省本级】专用
                    55		二类门慢限额剩余	VARCHAR2(16)		【景德镇】【省本级】专用
                    56		居民二次补偿（大病支付）	VARCHAR2(16)		【鹰潭】专用2位小数
                    57		体检金额	VARCHAR2(16)		【九江】专用2位小数
                    58		生育基金支付	VARCHAR2(16)		
                    59		居民大病一段金额	VARCHAR2(16)		【九江、鹰潭居民】专用2位小数
                    60		居民大病二段金额	VARCHAR2(16)		【九江、鹰潭居民】
                    61		疾病补充范围内费用支付金额	VARCHAR2(16)		【九江/鹰潭/萍乡居民】
                    62		疾病补充保险本次政策范围外费用支付金额	VARCHAR2(16)		【九江/鹰潭/萍乡居民】
                    63		美国微笑列车基金支付	VARCHAR2(16)		【九江/鹰潭居民】
                    64		九江居民政策范围外可报销费用	VARCHAR2(16)		【九江居民】
                    65		政府兜底基金费用	VARCHAR2(16)		【萍乡/鹰潭/九江居民】
                    66		免费门诊基金（余江）	VARCHAR2(16)		【鹰潭居民】
                    67		建档大病补偿本次一段支付金额	VARCHAR2(16)		【鹰潭居民】
                    68		建档大病补偿本次二段支付金额	VARCHAR2(16)		【鹰潭居民】
                    69		建档大病补偿本次三段支付金额	VARCHAR2(16)		【鹰潭居民】
                    70		建档二次补偿本次一段支付金额	VARCHAR2(16)		【鹰潭居民】
                    71		建档二次补偿本次二段支付金额	VARCHAR2(16)		【鹰潭居民】
                    72		建档二次补偿本次三段支付金额	VARCHAR2(16)		【鹰潭居民】
                    73		疾病补充保险本次政策范围内费用一段支付金额	VARCHAR2(16)		【鹰潭居民】
                    74		疾病补充保险本次政策范围内费用二段支付金额	VARCHAR2(16)		【鹰潭居民】
                    75		本年政府兜底基金费用累计(不含本次)	VARCHAR2(16)		【九江居民】
                    76		门慢限额	VARCHAR2(16)		【江西省本级】
                    77		企业军转干基金支付	VARCHAR2(16)		【鹰潭】
                    78		其他基金支付金额	VARCHAR2(16)		异地就医就医地返给定点出参；本地就医返参为0
                    79		伤残人员医疗保障基金	VARCHAR2(16)		异地就医就医地返给定点出参；本地就医返参为0
                 */

                    js.Ylfze = sfjsfh[0];         //医疗费总费用
                    js.Zbxje = sfjsfh[1];         //总报销金额
                    js.Tcjjzf = sfjsfh[2];        //统筹支出
                    js.Dejjzf = sfjsfh[3];        //大病支出
                    js.Zhzf = sfjsfh[4];          //本次帐户支付
                    js.Xjzf = sfjsfh[5];         //个人现金支付
                    js.Gwybzjjzf = sfjsfh[6];     //公务员补助支付金额
                    js.Qybcylbxjjzf = sfjsfh[7];  //企业补充支付金额
                    js.Zffy = sfjsfh[8];          //自费费用
                    js.Dwfdfy = sfjsfh[9];
                    js.Yyfdfy = sfjsfh[10];
                    js.Mzjzfy = sfjsfh[11];       //民政救助费用
                    js.Cxjfy = sfjsfh[12];
                    js.Ylzlfy = sfjsfh[13];
                    js.Blzlfy = sfjsfh[14];
                    js.Fhjjylfy = sfjsfh[15];
                    js.Qfbzfy = sfjsfh[16];
                    js.Zzzyzffy = sfjsfh[17];
                    js.Jrtcfy = sfjsfh[18];
                    js.Tcfdzffy = sfjsfh[19];
                    js.Ctcfdxfy = sfjsfh[20];
                    js.Jrdebxfy = sfjsfh[21];
                    js.Defdzffy = sfjsfh[22];
                    js.Cdefdxfy = sfjsfh[23];
                    js.Rgqgzffy = sfjsfh[24];
                    js.Bcjsqzhye = sfjsfh[25];
                    js.Bntczflj = sfjsfh[26];
                    js.Bndezflj = sfjsfh[27];
                    js.Bnczjmmztczflj = sfjsfh[28];
                    js.Bngwybzzflj = sfjsfh[29];
                    js.Bnzhzflj = sfjsfh[30];
                    js.Bnzycslj = sfjsfh[31];
                    js.Zycs = sfjsfh[32];
                    js.Xm = sfjsfh[33];
                    js.Jsrq = sfjsfh[34];
                    js.Yllb = sfjsfh[35];
                    js.Yldylb = sfjsfh[36];
                    js.Jbjgbm = sfjsfh[37];
                    js.Ywzqh = sfjsfh[38];
                    js.Jslsh = sfjsfh[39];
                    js.Tsxx = sfjsfh[40];
                    js.Djh = sfjsfh[41];
                    js.Jyxl = sfjsfh[42];
                    js.Yyjylsh = sfjsfh[43];
                    js.Yxbz = sfjsfh[44];
                    js.Grbhgl = sfjsfh[45];
                    js.Yljgbm = sfjsfh[46];
                    js.Ecbcje = sfjsfh[47];
                    js.Mmqflj = sfjsfh[48];
                    js.Jsfjylsh = sfjsfh[49];
                    js.Grbh = sfjsfh[50];
                    js.Dbzbc = sfjsfh[51];
                    js.Czzcje = sfjsfh[52];
                    js.Elmmxezc = sfjsfh[53];
                    js.Elmmxesy = sfjsfh[54];
                    js.Jmecbc = sfjsfh[55];
                    js.Tjje = sfjsfh[56];
                    js.Syjjzf = sfjsfh[57];
                    js.Jmdbydje = sfjsfh[58];
                    js.Jmdbedje = sfjsfh[59];
                    js.Jbbcfwnfyzfje = sfjsfh[60];
                    js.Jbbcybbczcfywfyzf = sfjsfh[61];
                    js.Mgwxlcjjzf = sfjsfh[62];
                    js.Jjjmzcfwwkbxfy = sfjsfh[63];
                    js.Zftdfy = sfjsfh[64];
                    js.Mfmzjj = sfjsfh[65];
                    js.Jddbbcbcydzfje = sfjsfh[66];
                    js.Jddbbcbcedzfje = sfjsfh[67];
                    js.Jddbbcbcsdzfje = sfjsfh[68];
                    js.Jdecbcbcydzfje = sfjsfh[69];
                    js.Jdecbcbcedzfje = sfjsfh[70];
                    js.Jdecbcbcsdzfje = sfjsfh[71];
                    js.Jbbcbxbczcfwnfyydzfje = sfjsfh[72];
                    js.Jbbcbxbczcfwnfyedzfje = sfjsfh[73];
                    js.Bnzftdjjfylj = sfjsfh[74];
                    js.Mmxe = sfjsfh[75];
                    js.Jzgbbzzf = sfjsfh[76];
                    //js.Qtjjzf = sfjsfh[77];
                    //js.Scryylbzjj = sfjsfh[78];

                    string[] sDBRY = new string[] { "80", "83", "84", "85", "86", "87" };
                    if (sDBRY.Contains(js.Yldylb))
                    {
                        /*
                        * 建档立卡人员除个人自负部分的费用外，均先由医院垫付
                        */
                        js.Ybxjzf = js.Xjzf;
                        js.Zhzbxje = (Convert.ToDecimal(js.Ylfze) - Convert.ToDecimal(js.Ybxjzf)).ToString();
                    }
                    else
                    {
                        /*
                         * 非建档立卡人员（含民政救助对象）住院（含门诊慢性病治疗）的医院垫付金额为：
                         * 统筹支出+账户支付+医院负担费用+民政救助费用+二次补偿金额+企业军转干基金支付。
                         */
                        js.Zhzbxje = (Convert.ToDecimal(js.Tcjjzf) + Convert.ToDecimal(js.Zhzf) + Convert.ToDecimal(js.Yyfdfy) + Convert.ToDecimal(js.Mzjzfy)
                                     + Convert.ToDecimal(js.Ecbcje) + Convert.ToDecimal(js.Jzgbbzzf)).ToString();
                        js.Ybxjzf = (Convert.ToDecimal(js.Ylfze) - Convert.ToDecimal(js.Zhzbxje)).ToString();
                    }
                    js.Qtybzf = (Convert.ToDecimal(js.Ylfze) - Convert.ToDecimal(js.Tcjjzf) - Convert.ToDecimal(js.Ybxjzf) - Convert.ToDecimal(js.Zhzf)).ToString(); ;

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
                    string strValue = js.Ylfze + "|" + js.Zhzbxje + "|" + js.Tcjjzf + "|" + js.Dejjzf + "|" + js.Zhzf + "|" +
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
                    WriteLog(sysdate + "  门诊收费结算｜整合出参|" + strValue);
                    #endregion

                    #region 数据操作
                    List<string> liSQL = new List<string>();
                    strSql = string.Format(@"insert into ybfyjsdr(jzlsh,ybjzlsh,jylsh,djhin,ylfze,zbxje,tcjjzf,dejjzf,zhzf,xjzf,
                                            gwybzjjzf,qybcylbxjjzf,zffy,dwfdfy,yyfdfy,mzjzfy,cxjfy,ylzlfy,blzlfy,fhjbylfy,
                                            qfbzfy,zzzyzffy,jrtcfy,tcfdzffy,ctcfdxfy,jrdebxfy,defdzffy,cdefdxfy,rgqgzffy,bcjsqzhye,
                                            bntczflj,bndezflj,bnczjmmztczflj,bngwybzzflj,bnzhzflj,bnzycslj,zycs,xm,kh,jsrq,
                                            yllb,yldylb,jbjgbm,ywzqh,jslsh,tsxx,djh,yyjylsh,grbhgl,yljgbm,
                                            ecbcje,mmqflj,jsfjylsh,grbh,czzcje,elmmxezc,elmmxesy,jmecbc,tjje,syjjzf,
                                            jjjmdbydje,jjjmdbedje,jjjmjbbcfwnje,jjjmjbbcfwwje,mgwxlcjjzf,jjjmzcfwwkbxfy,zftdjjzf,mfmzjj,jddbbcbcydzfje,jddbbcbcedzfje,
                                            jddbbcbcsdzfje,jdecbcbcydzfje,jdecbcbcedzfje,jdecbcbcsdzfje,jbbcbxbczcfwnfyydzfje,jbbcbxbczcfwnfyedzfje,bnzftdjjfylj,lxgbddtczf,qtjjzf,scryylbzjj,
                                            zhxjzffy,qtybfy,sysdate,jbr,zhzbxje,cfmxjylsh)
                                            values('{0}','{1}','{2}','{3}','{4}','{5}' ,'{6}','{7}','{8}' ,'{9}' ,
                                            '{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}',
                                            '{20}','{21}','{22}','{23}','{24}','{25}','{26}','{27}','{28}','{29}',
                                            '{30}','{31}','{32}','{33}','{34}','{35}','{36}','{37}','{38}','{39}',
                                            '{40}','{41}','{42}','{43}','{44}','{45}','{46}','{47}','{48}','{49}',
                                            '{50}','{51}','{52}','{53}','{54}','{55}','{56}','{57}','{58}','{59}',
                                            '{60}','{61}','{62}','{63}','{64}','{65}','{66}','{67}','{68}','{69}',
                                            '{70}','{71}','{72}','{73}','{74}','{75}','{76}','{77}','{78}','{79}',
                                            '{80}','{81}','{82}','{83}','{84}','{85}')",
                                        jzlsh, ybjzlsh, JYLSH, djh, js.Ylfze, js.Zbxje, js.Tcjjzf, js.Dejjzf, js.Zhzf, js.Xjzf,
                                        js.Gwybzjjzf, js.Qybcylbxjjzf, js.Zffy, js.Dwfdfy, js.Yyfdfy, js.Mzjzfy, js.Cxjfy, js.Ylzlfy, js.Blzlfy, js.Fhjjylfy,
                                        js.Qfbzfy, js.Zzzyzffy, js.Jrtcfy, js.Tcfdzffy, js.Ctcfdxfy, js.Jrdebxfy, js.Defdzffy, js.Cdefdxfy, js.Rgqgzffy, js.Bcjsqzhye,
                                        js.Bntczflj, js.Bndezflj, js.Bnczjmmztczflj, js.Bngwybzzflj, js.Bnzhzflj, js.Bnzycslj, js.Zycs, js.Xm, js.Kh, js.Jsrq,
                                        js.Yllb, js.Yldylb, js.Jbjgbm, js.Ywzqh, js.Jslsh, js.Tsxx, js.Djh, js.Yyjylsh, js.Grbhgl, js.Yljgbm,
                                        js.Ecbcje, js.Mmqflj, js.Jsfjylsh, js.Grbh, js.Czzcje, js.Elmmxezc, js.Elmmxesy, js.Jmecbc, js.Tjje, js.Syjjzf,
                                        js.Jmdbydje, js.Jmdbedje, js.Jbbcfwnfyzfje, js.Jbbcybbczcfywfyzf, js.Mgwxlcjjzf, js.Jjjmzcfwwkbxfy, js.Zftdfy, js.Mfmzjj, js.Jddbbcbcydzfje, js.Jddbbcbcedzfje,
                                        js.Jddbbcbcsdzfje, js.Jdecbcbcydzfje, js.Jdecbcbcedzfje, js.Jdecbcbcsdzfje, js.Jbbcbxbczcfwnfyydzfje, js.Jbbcbxbczcfwnfyedzfje, js.Bnzftdjjfylj, js.Lxgbdttcjjzf, js.Qtjjzf, js.Scryylbzjj,
                                        js.Ybxjzf, js.Qtybzf, sysdate, jbr, js.Zhzbxje, cfmxjylsh);

                    liSQL.Add(strSql);
                    object[] obj = liSQL.ToArray();
                    obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);
                    if (obj[1].ToString() == "1")
                    {
                        WriteLog(sysdate + "  门诊收费结算成功|本地数据操作成功|" + outputData.ToString().Split('^')[2]);
                        return new object[] { 0, 1, strValue };
                    }
                    else
                    {
                        WriteLog(sysdate + "  门诊收费结算成功|本地数据操作失败|" + obj[2].ToString());
                        //费用结算撤销信息、处方明细上传撤销
                        object[] objFYJSCX = { jzlsh, js.Djh, ybjzlsh, jsrq, grbh, xm, kh, dqbh, ybjzlsh_snyd, "", DQJBBZ };
                        NYBFYJSCX(objFYJSCX);
                        return new object[] { 0, 0, "门诊收费结算成功|数据库操作失败" + obj[2].ToString() };
                    }
                    #endregion
                }
                else
                {
                    ZXBM = dqbh;
                    #region 入参
                    /*
                        1		就诊流水号	VARCHAR2(20)	NOT NULL	唯一
                        2		单据号	VARCHAR2(20)	NULL	预结算为空
                        3		交易日期	VARCHAR2(14)	NOT NULL  	YYYYMMDDHH24MISS
                        4		经办人	VARCHAR2(50)		
                        5		诊断疾病编码	VARCHAR2(20)		
                        6		诊断疾病名称	VARCHAR2(50)		
                        7		医疗类别	VARCHAR2(3)		二级代码
                        8		开发商标志	VARCHAR2(20)	NOT NULL	HIS开发商自定义的特殊标记，能够区分出不同的开发商即可。
                        9		个人编号	VARCHAR2(10)	NOT NULL	异地结算
                        10		姓名	VARCHAR2(20)	NOT NULL	异地结算
                        11		卡号	VARCHAR2(18)	NOT NULL	异地结算                     
                     */
                    StringBuilder inputParam = new StringBuilder();
                    inputParam.Append(ybjzlsh_snyd + "|");
                    inputParam.Append(djh + "|");
                    inputParam.Append(jsrq + "|");
                    inputParam.Append(jbr + "|");
                    inputParam.Append(bzbm + "|");
                    inputParam.Append(bzmc + "|");
                    inputParam.Append(yllb + "|");
                    inputParam.Append("gocent|");
                    inputParam.Append(grbh + "|");
                    inputParam.Append(xm + "|");
                    inputParam.Append(kh + "|$");

                    #endregion

                    #region  判断草药单复方标志
                    string zcffbz = "";//草药单复方标志
                    strSql = string.Format(@"select mcmzno as mcmzno from mzcfd where  mccfno LIKE 'C%' and  mcghno='{0}' AND mccfno IN ({1})", jzlsh, cfhs);
                    ds.Tables.Clear();
                    ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                    if (ds.Tables[0].Rows.Count > 1)
                        zcffbz = "1";
                    else if (ds.Tables[0].Rows.Count == 1)
                        zcffbz = "0";
                    #endregion

                    #region 获取处方明细信息
                    strSql = string.Format(@"select y.ybxmbh, y.ybxmmc, m.dj, sum(m.sl) sl, sum(m.je) je, m.yyxmbh, m.yyxmmc, y.sfxmzldm, y.sflbdm,y.jxdm, m.cfh,m.gg,m.jxdw,y.jx,m.yf,m.sfno from 
                                        (
                                        --药品
                                        select a.mcypno yyxmbh, a.mcypnm yyxmmc, a.mcpric dj, a.mcquty sl, a.mcamnt je, a.mccfno cfh,a.mcypgg gg,a.mcunt1 as jxdw,a.mcwayx as yf,a.mcsflb as sfno
                                        from mzcfd a 
                                        where a.mcghno = '{0}' and a.mccfno in ({1})
                                        union all
                                        --检查/治疗
                                        select b.mbitem yyxmbh, b.mbname yyxmmc, b.mbpric dj, b.mbquty sl, b.mbsjam je,b.mbzlno cfh,NULL as gg,NULL as jxdw,NULL AS yf,b.mbsfno as sfno         
                                        from mzb2d b 
                                        where b.mbghno = '{0}' and b.mbzlno in ({1})
                                        union all
                                        --检验
                                        select c.mbitem yyxmbh, c.mbname yyxmmc, c.mbpric dj, c.mbquty sl, c.mbsjam je, c.mbzlno cfh,NULL as gg,NULL as jxdw,NULL AS yf,c.mbsfno as sfno 
                                        from mzb4d c 
                                        where c.mbghno = '{0}' and c.mbzlno in ({1})
                                        union all
                                        --注射
                                        select b5item yyxmbh, b5name yyxmmc, b5sfam dj, mddays sl, b5sfam * mddays je, mdzsno cfh,NULL as gg,NULL as jxdw,NULL AS yf,mdsflb as sfno 
                                        from mzd3d
                                        left join (select distinct mccfno, mcghno from mzcfd) mzcf on mccfno = mdcfno
                                        left join bz09d on b9mbno = mdtwid 
                                        left join bz05d on b5item = b9item where mdtiwe > 0 and mdzsno in ({1})
                                        union all
                                        select b5item yyxmbh, b5name yyxmmc, b5sfam dj, mdtims sl, b5sfam * mdtims je,mdzsno cfh,NULL as gg,NULL as jxdw,NULL AS yf,mdsflb as sfno 
                                        from mzd3d 
                                        left join bz09d on b9mbno = mdwayid left join bz05d on b5item = b9item
                                        left join (select distinct mccfno, mcghno from mzcfd) mzcf on mccfno = mdcfno 
                                        where mdzsno in ({1})
                                        union all
                                        select b5item yyxmbh, b5name yyxmmc, b5sfam dj, mdpqty sl, b5sfam * mdpqty je,mdzsno cfh,NULL as gg,NULL as jxdw,NULL AS yf,mdsflb as sfno 
                                        from mzd3d 
                                        left join bz09d on b9mbno = mdpprid 
                                        left join bz05d on b5item = b9item
                                        left join (select distinct mccfno, mcghno from mzcfd) mzcf on mccfno = mdcfno
                                        where mdpqty > 0 and mdzsno in ({1})
                                        ) m 
                                        left join ybhisdzdr y on m.yyxmbh = y.hisxmbh
                                        group by y.ybxmbh, y.ybxmmc, m.dj, m.yyxmbh, m.yyxmmc, y.sfxmzldm, y.sflbdm,y.jxdm, m.cfh,m.gg,m.jxdw,y.jx,m.yf,m.sfno", jzlsh, cfhs);
                    ds.Tables.Clear();
                    ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                    if (ds.Tables[0].Rows.Count == 0)
                    {
                        WriteLog(sysdate + "  无费用明细");
                        return new object[] { 0, 0, "无费用明细" };
                    }
                    #endregion

                    #region 处方明细处理
                    DataTable dt = ds.Tables[0];
                    string cfsj = DateTime.Now.ToString("yyyyMMddHHmmss");  //处方时间
                    StringBuilder wdzxms = new StringBuilder();
                    List<string> liSQL = new List<string>();
                    List<string> liyyxmbh = new List<string>();
                    List<string> liyyxmmc = new List<string>();
                    List<string> liybxmbm = new List<string>();
                    List<string> liybxmmc = new List<string>();
                    List<string> licfh = new List<string>();
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
                            string xmlb = dr["sfxmzldm"].ToString();  //收费项目等级代码
                            string sflb = dr["sflbdm"].ToString();      //收费类别代码
                            string yyxmbh = dr["yyxmbh"].ToString();          //检查项目代码
                            string ybxmbh = dr["ybxmbh"].ToString();          //医保项目编号
                            string ybxmmc = dr["ybxmmc"].ToString();          //医保项目名称
                            string yyxmmc = dr["yyxmmc"].ToString();          //项目名称
                            decimal dj = Convert.ToDecimal(dr["dj"]);
                            decimal sl = Convert.ToDecimal(dr["sl"]);
                            decimal je = Convert.ToDecimal(dr["je"]);
                            sfje_total += je;
                            string jx = dr["jxdm"].ToString();
                            string gg = dr["gg"].ToString();
                            decimal mcyl = 1;
                            string pd = "qd";
                            string yf = "";
                            string dw = "";
                            string txts = "";
                            string zcffbz1 = "";
                            string qezfbz = "";
                            string cfh = dr["cfh"].ToString();
                            string ybcfh = cfsj + k.ToString();
                            liyyxmbh.Add(yyxmbh);
                            liyyxmmc.Add(yyxmmc);
                            liybxmbm.Add(ybxmbh);
                            liybxmmc.Add(ybxmmc);
                            licfh.Add(cfh);

                            string ypjldw = "";

                            if (dr["sfno"].ToString() == "01" || dr["sfno"].ToString() == "02" || dr["sfno"].ToString() == "03")
                            {
                                zcffbz1 = zcffbz;
                                ypjldw = "粒";
                            }


                            /*
                            0	就诊流水号	VARCHAR2(20)	NOT NULL	唯一
                            1	收费项目种类	VARCHAR2(3)	NOT NULL	二级代码
                            2	费用类别	VARCHAR2(3)	NOT NULL	二级代码
                            3	处方号	VARCHAR2(20)	NOT NULL	非处方药没有处方号，统一传AAAA
                            4	处方日期	VARCHAR2(14)	NOT NULL	YYYYMMDDHH24MISS，非处方药同交易日期
                            5	医院收费项目内码	VARCHAR2(20)		
                            6	收费项目中心编码	VARCHAR2(20)	NOT NULL	中心编号
                            7	医院收费项目名称	VARCHAR2(50)		
                            8	单价	VARCHAR2(12)	NOT NULL	4位小数
                            9	数量	VARCHAR2(12)	NOT NULL	2位小数
                            10	金额	VARCHAR2(12)	NOT NULL	4位小数，金额与 (单价*数量)的差值不能大于0.01
                            11	剂型	VARCHAR2(20)		
                            12	规格	VARCHAR2(100)		
                            13	每次用量	VARCHAR2(12)		2位小数
                            14	使用频次	VARCHAR2(20)		
                            15	医师编号	VARCHAR2(20)		
                            16	医师姓名	VARCHAR2(20)		
                            17	用法	VARCHAR2(20)		
                            18	单位	VARCHAR2(20)		
                            19	科室编号	VARCHAR2(20)		
                            20	科室名称	VARCHAR2(20)		
                            21	限定天数	VARCHAR2(4)		
                            22	草药单复方标志	VARCHAR2(3)		二级代码
                            23	经办人	VARCHAR2(20)		操作员姓名
                            24	药品剂量单位	VARCHAR2(20)		
                            25	自费标志	VARCHAR2(3)		按照自费项目处理
                         */
                            inputParam.Append(ybjzlsh_snyd + "|");
                            inputParam.Append(xmlb + "|");
                            inputParam.Append(sflb + "|");
                            inputParam.Append(cfh + "|");
                            inputParam.Append(cfsj + "|");
                            inputParam.Append(yyxmbh + "|");
                            inputParam.Append(ybxmbh + "|");
                            inputParam.Append(yyxmmc + "|");
                            inputParam.Append(dj + "|");
                            inputParam.Append(sl + "|");
                            inputParam.Append(je + "|");
                            inputParam.Append(jx + "|");
                            inputParam.Append(gg + "|");
                            inputParam.Append(mcyl + "|");
                            inputParam.Append(pd + "|");
                            inputParam.Append(cfysdm + "|");
                            inputParam.Append(cfysxm + "|");
                            inputParam.Append(yf + "|");
                            inputParam.Append(dw + "|");
                            inputParam.Append(ksbm + "|");
                            inputParam.Append(ksmc + "|");
                            inputParam.Append(txts + "|");
                            inputParam.Append(zcffbz1 + "|");
                            inputParam.Append(jbr + "|");
                            inputParam.Append(ypjldw + "|");
                            inputParam.Append(qezfbz + "|$");

                            strSql = string.Format(@"insert into ybcfmxscindr(jzlsh,jylsh,xm,kh,ybjzlsh,cfrq,yysfxmbm,yysfxmmc,sfxmzxbm,sfxmzxmc,
                                            dj,sl,je,jbr,sysdate,sflb,ybcfh) values(
                                            '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}',
                                            '{10}','{11}','{12}','{13}','{14}','{15}','{16}')",
                                                jzlsh, JYLSH, xm, kh, ybjzlsh, cfsj, yyxmbh, yyxmmc, ybxmbh, ybxmmc,
                                                dj, sl, je, jbr, sysdate, sflb, cfh);
                            liSQL.Add(strSql);
                        }
                    }

                    if (wdzxms.Length > 0)
                    {
                        return new object[] { 0, 0, wdzxms.ToString() };
                    }

                    if (Math.Abs(sfje_total - sfje1) > 1)
                        return new object[] { 0, 0, "收费金额与医保结算金额相差" + Math.Abs(sfje_total - sfje1) + ",无法结算，请核实!" };
                    #endregion

                    #region 结算
                    StringBuilder inputData = new StringBuilder();
                    YWBH = "2620";
                    inputData.Append(YWBH + "^");
                    inputData.Append(YLGHBH + "^");
                    inputData.Append(CZYBH + "^");
                    inputData.Append(YWZQH + "^");
                    inputData.Append(JYLSH + "^");
                    inputData.Append(ZXBM + "^");
                    inputData.Append(inputParam.ToString().TrimEnd('$') + "^");
                    inputData.Append(LJBZ + "^");
                    WriteLog(sysdate + "   费用结算(异地医保)|入参|" + inputData.ToString());

                    StringBuilder outputData = new StringBuilder(10240);
                    int i = BUSINESS_HANDLE(inputData, outputData);
                    WriteLog(sysdate + "  费用结算(异地医保)|出参|" + outputData.ToString());
                    if (i < 0)
                        return new object[] { 0, 0, outputData.ToString().Split('^')[2].ToString() };
                    #endregion

                    #region 出参
                    /*
                        1		医疗费总额	VARCHAR2(16)		2位小数
                        2		总报销金额	VARCHAR2(16)		总报销金额+现金=医疗费总额，2位小数
                        3		统筹基金支付	VARCHAR2(16)		2位小数
                        4		大额基金支付	VARCHAR2(16)		2位小数
                        5		账户支付	VARCHAR2(16)		2位小数
                        6		现金支付	VARCHAR2(16)		2位小数
                        7		公务员补助基金支付	VARCHAR2(16)		2位小数
                        8		企业补充医疗保险基金支付	VARCHAR2(16)		2位小数
                        9		自费费用	VARCHAR2(16)		2位小数
                        10		单位负担费用	VARCHAR2(16)		2位小数
                        11		医院负担费用	VARCHAR2(16)		2位小数
                        12		民政救助费用	VARCHAR2(16)		2位小数
                        13		超限价费用	VARCHAR2(16)		2位小数
                        14		乙类自理费用	VARCHAR2(16)		2位小数
                        15		丙类自理费用	VARCHAR2(16)		2位小数
                        16		符合基本医疗费用	VARCHAR2(16)		2位小数
                        17		起付标准费用	VARCHAR2(16)		2位小数
                        18		转诊转院自付费用	VARCHAR2(16)		2位小数
                        19		进入统筹费用	VARCHAR2(16)		2位小数
                        20		统筹分段自付费用	VARCHAR2(16)		2位小数
                        21		超统筹封顶线费用	VARCHAR2(16)		2位小数
                        22		进入大额报销费用	VARCHAR2(16)		2位小数
                        23		大额分段自付费用	VARCHAR2(16)		2位小数
                        24		超大额封顶线费用	VARCHAR2(16)		2位小数
                        25		人工器官自付费用	VARHCAR2(16)		2位小数
                        26		本次结算前帐户余额	VARHCAR2(16)		2位小数
                        27		本年统筹支付累计(不含本次)	VARHCAR2(16)		2位小数
                        28		本年大额支付累计(不含本次)	VARHCAR2(16)		2位小数
                        29		本年城镇居民门诊统筹支付累计(不含本次)	VARHCAR2(16)		2位小数
                        30		本年公务员补助支付累计(不含本次)	VARHCAR2(16)		2位小数
                        31		本年账户支付累计(不含本次)	VARCHAR2(16)		2位小数
                        32		本年住院次数累计(不含本次)	VARCHAR2(16)		2位小数
                        33		住院次数	VARCHAR2(5)		
                        34		姓名	VARCHAR2(50)		
                        35		结算日期	VARCHAR2(14)		YYYYMMDDHH24MISS
                        36		医疗类别	VARCHAR2(3)		二级代码
                        37		医疗待遇类别	VARCHAR2(3)		二级代码
                        38		经办机构编码	VARCHAR2(16)		二级代码
                        39		业务周期号	VARCHAR2(36)		
                        40		结算流水号	VARCHAR2(20)		获取结算单交易的入参
                        41		提示信息	VARCHAR2(200)		His端必须将此信息显示到前台
                        42		单据号	VARCHAR2(20)		
                        43		交易类型	VARCHAR2(3)		二级代码 
                        44		医院交易流水号	VARCHAR2(50)		
                        45		有效标志	VARCHAR2(3)		二级代码 
                        46		个人编号	VARCHAR2(10)		
                        47		医疗机构编码	VARCHAR2(20)		
                        48		就诊流水号	VARCHAR2(20)		异地撤销要取异地返回的就诊流水号进行撤销
                        49		其他基金支付金额	VARCHAR2(20)		异地就医就医地返回给定点出参（目前只添加了异地就医就医地出参部分，位数按时2420出参位数）
                        50	    伤残人员医疗保障基金	VARCHAR2(16)		异地就医就医地返给定点出参；本地就医返参为0
                     */

                    string[] sfjsfh = outputData.ToString().Split('^')[2].Split('|');
                    js.Ylfze = sfjsfh[0];         //医疗费总费用
                    js.Zbxje = sfjsfh[1];         //总报销金额
                    js.Tcjjzf = sfjsfh[2];        //统筹支出
                    js.Dejjzf = sfjsfh[3];        //大病支出
                    js.Zhzf = sfjsfh[4];          //本次帐户支付
                    js.Xjzf = sfjsfh[5];         //个人现金支付
                    js.Gwybzjjzf = sfjsfh[6];     //公务员补助支付金额
                    js.Qybcylbxjjzf = sfjsfh[7];  //企业补充支付金额
                    js.Zffy = sfjsfh[8];          //自费费用
                    js.Dwfdfy = sfjsfh[9];
                    js.Yyfdfy = sfjsfh[10];
                    js.Mzjzfy = sfjsfh[11];       //民政救助费用
                    js.Cxjfy = sfjsfh[12];
                    js.Ylzlfy = sfjsfh[13];
                    js.Blzlfy = sfjsfh[14];
                    js.Fhjjylfy = sfjsfh[15];
                    js.Qfbzfy = sfjsfh[16];
                    js.Zzzyzffy = sfjsfh[17];
                    js.Jrtcfy = sfjsfh[18];
                    js.Tcfdzffy = sfjsfh[19];
                    js.Ctcfdxfy = sfjsfh[20];
                    js.Jrdebxfy = sfjsfh[21];
                    js.Defdzffy = sfjsfh[22];
                    js.Cdefdxfy = sfjsfh[23];
                    js.Rgqgzffy = sfjsfh[24];
                    js.Bcjsqzhye = sfjsfh[25];
                    js.Bntczflj = sfjsfh[26];
                    js.Bndezflj = sfjsfh[27];
                    js.Bnczjmmztczflj = sfjsfh[28];
                    js.Bngwybzzflj = sfjsfh[29];
                    js.Bnzhzflj = sfjsfh[30];
                    js.Bnzycslj = sfjsfh[31];
                    js.Zycs = sfjsfh[32];
                    js.Xm = sfjsfh[33];
                    js.Jsrq = sfjsfh[34];
                    js.Yllb = sfjsfh[35];
                    js.Yldylb = sfjsfh[36];
                    js.Jbjgbm = sfjsfh[37];
                    js.Ywzqh = sfjsfh[38];
                    js.Jslsh = sfjsfh[39];
                    js.Tsxx = sfjsfh[40];
                    js.Djh = sfjsfh[41];
                    js.Jyxl = sfjsfh[42];
                    js.Yyjylsh = sfjsfh[43];
                    js.Yxbz = sfjsfh[44];
                    js.Grbh = sfjsfh[45];
                    js.Yljgbm = sfjsfh[46];
                    js.Ybjzlsh = sfjsfh[47];
                    js.Qtjjzf = sfjsfh[48];
                    js.Scryylbzjj = sfjsfh[49];
                    if (string.IsNullOrEmpty(js.Scryylbzjj))
                        js.Scryylbzjj = "0.00";

                    js.Jmdbydje = "0.00";
                    js.Jmdbedje = "0.00";
                    js.Ecbcje = "0.00";
                    js.Jzgbbzzf = "0.00";

                    string[] sDBRY = new string[] { "80", "83", "84", "85", "86", "87" };
                    if (sDBRY.Contains(js.Yldylb))
                    {
                        /*
                        * 建档立卡人员除个人自负部分的费用外，均先由医院垫付
                        */
                        js.Ybxjzf = js.Xjzf;
                        js.Zhzbxje = (Convert.ToDecimal(js.Ylfze) - Convert.ToDecimal(js.Ybxjzf)).ToString();
                    }
                    else
                    {
                        /*
                         * 非建档立卡人员（含民政救助对象）住院（含门诊慢性病治疗）的医院垫付金额为：
                         * 统筹支出+账户支付+医院负担费用+民政救助费用+二次补偿金额+企业军转干基金支付。
                         */
                        js.Zhzbxje = (Convert.ToDecimal(js.Tcjjzf) + Convert.ToDecimal(js.Zhzf) + Convert.ToDecimal(js.Yyfdfy) + Convert.ToDecimal(js.Mzjzfy)
                                     + Convert.ToDecimal(js.Ecbcje) + Convert.ToDecimal(js.Jzgbbzzf)).ToString();
                        js.Ybxjzf = (Convert.ToDecimal(js.Ylfze) - Convert.ToDecimal(js.Zhzbxje)).ToString();
                    }
                    js.Qtybzf = (Convert.ToDecimal(js.Ylfze) - Convert.ToDecimal(js.Tcjjzf) - Convert.ToDecimal(js.Ybxjzf) - Convert.ToDecimal(js.Zhzf)).ToString(); ;



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
                    string strValue = js.Ylfze + "|" + js.Zhzbxje + "|" + js.Tcjjzf + "|" + js.Dejjzf + "|" + js.Zhzf + "|" +
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
                    WriteLog(sysdate + "  门诊收费结算(异地医保)｜整合出参|" + strValue);
                    #endregion

                    #region 数据操作
                    strSql = string.Format(@"insert into ybfyjsdr(jzlsh,ybjzlsh,jylsh,djhin,ylfze,zbxje,tcjjzf,dejjzf,zhzf,xjzf,
                                            gwybzjjzf,qybcylbxjjzf,zffy,dwfdfy,yyfdfy,mzjzfy,cxjfy,ylzlfy,blzlfy,fhjbylfy,
                                            qfbzfy,zzzyzffy,jrtcfy,tcfdzffy,ctcfdxfy,jrdebxfy,defdzffy,cdefdxfy,rgqgzffy,bcjsqzhye,
                                            bntczflj,bndezflj,bnczjmmztczflj,bngwybzzflj,bnzhzflj,bnzycslj,zycs,xm,kh,jsrq,
                                            yllb,yldylb,jbjgbm,ywzqh,jslsh,tsxx,djh,yyjylsh,grbhgl,yljgbm,
                                            ecbcje,mmqflj,jsfjylsh,grbh,czzcje,elmmxezc,elmmxesy,jmecbc,tjje,syjjzf,
                                            jjjmdbydje,jjjmdbedje,jjjmjbbcfwnje,jjjmjbbcfwwje,mgwxlcjjzf,jjjmzcfwwkbxfy,zftdjjzf,mfmzjj,jddbbcbcydzfje,jddbbcbcedzfje,
                                            jddbbcbcsdzfje,jdecbcbcydzfje,jdecbcbcedzfje,jdecbcbcsdzfje,jbbcbxbczcfwnfyydzfje,jbbcbxbczcfwnfyedzfje,bnzftdjjfylj,lxgbddtczf,qtjjzf,scryylbzjj,
                                            zhxjzffy,qtybfy,sysdate,jbr,zhzbxje,cfmxjylsh)
                                            values('{0}','{1}','{2}','{3}','{4}','{5}' ,'{6}','{7}','{8}' ,'{9}' ,
                                            '{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}',
                                            '{20}','{21}','{22}','{23}','{24}','{25}','{26}','{27}','{28}','{29}',
                                            '{30}','{31}','{32}','{33}','{34}','{35}','{36}','{37}','{38}','{39}',
                                            '{40}','{41}','{42}','{43}','{44}','{45}','{46}','{47}','{48}','{49}',
                                            '{50}','{51}','{52}','{53}','{54}','{55}','{56}','{57}','{58}','{59}',
                                            '{60}','{61}','{62}','{63}','{64}','{65}','{66}','{67}','{68}','{69}',
                                            '{70}','{71}','{72}','{73}','{74}','{75}','{76}','{77}','{78}','{79}',
                                            '{80}','{81}','{82}','{83}','{84}','{85}')",
                                        jzlsh, ybjzlsh, JYLSH, djh, js.Ylfze, js.Zbxje, js.Tcjjzf, js.Dejjzf, js.Zhzf, js.Xjzf,
                                        js.Gwybzjjzf, js.Qybcylbxjjzf, js.Zffy, js.Dwfdfy, js.Yyfdfy, js.Mzjzfy, js.Cxjfy, js.Ylzlfy, js.Blzlfy, js.Fhjjylfy,
                                        js.Qfbzfy, js.Zzzyzffy, js.Jrtcfy, js.Tcfdzffy, js.Ctcfdxfy, js.Jrdebxfy, js.Defdzffy, js.Cdefdxfy, js.Rgqgzffy, js.Bcjsqzhye,
                                        js.Bntczflj, js.Bndezflj, js.Bnczjmmztczflj, js.Bngwybzzflj, js.Bnzhzflj, js.Bnzycslj, js.Zycs, js.Xm, js.Kh, js.Jsrq,
                                        js.Yllb, js.Yldylb, js.Jbjgbm, js.Ywzqh, js.Jslsh, js.Tsxx, js.Djh, js.Yyjylsh, js.Grbhgl, js.Yljgbm,
                                        js.Ecbcje, js.Mmqflj, js.Jsfjylsh, js.Grbh, js.Czzcje, js.Elmmxezc, js.Elmmxesy, js.Jmecbc, js.Tjje, js.Syjjzf,
                                        js.Jmdbydje, js.Jmdbedje, js.Jbbcfwnfyzfje, js.Jbbcybbczcfywfyzf, js.Mgwxlcjjzf, js.Jjjmzcfwwkbxfy, js.Zftdfy, js.Mfmzjj, js.Jddbbcbcydzfje, js.Jddbbcbcedzfje,
                                        js.Jddbbcbcsdzfje, js.Jdecbcbcydzfje, js.Jdecbcbcedzfje, js.Jdecbcbcsdzfje, js.Jbbcbxbczcfwnfyydzfje, js.Jbbcbxbczcfwnfyedzfje, js.Bnzftdjjfylj, js.Lxgbdttcjjzf, js.Qtjjzf, js.Scryylbzjj,
                                        js.Ybxjzf, js.Qtybzf, sysdate, jbr, js.Zhzbxje, JYLSH);
                    liSQL.Add(strSql);

                    object[] obj = liSQL.ToArray();
                    obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);
                    if (obj[1].ToString() == "1")
                    {
                        WriteLog(sysdate + "  门诊收费结算(异地医保)成功|本地数据操作成功|" + outputData.ToString().Split('^')[2]);
                        return new object[] { 0, 1, strValue };
                    }
                    else
                    {
                        WriteLog(sysdate + "  门诊收费结算(异地医保)成功|本地数据操作失败|" + obj[2].ToString());
                        //费用结算撤销信息、处方明细上传撤销
                        object[] objFYJSCX = { jzlsh, js.Djh, ybjzlsh, jsrq, grbh, xm, kh, dqbh, ybjzlsh_snyd, "", DQJBBZ };
                        NYBFYJSCX(objFYJSCX);
                        return new object[] { 0, 0, "门诊收费结算(异地医保)成功|数据库操作失败" + obj[2].ToString() };
                    }
                    #endregion
                }
            }
            catch (Exception error)
            {
                WriteLog(sysdate + "  系统异常|" + error.Message);
                return new object[] { 0, 2, error.Message };
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
                YWZQH = CliUtils.fLoginYbNo.Split('|')[0].ToString();    //业务周期号
            }
            catch
            {
                return new object[] { 0, 0, "医保未连接或初始化失败" };
            }
            try
            {
                #region 入参
                string jzlsh = objParam[0].ToString();   // 就诊流水号
                string djh = objParam[1].ToString();     // 发票号

                CZYBH = CliUtils.fLoginUser;   // 操作员工号 
                ZXBM = "0000";
                string jbr = CliUtils.fUserName;    //经办人
                #endregion

                //交易流水号
                JYLSH = DateTime.Now.ToString("yyyyMMddHHmmss") + "-" + YLGHBH + "-" + new Random().Next(100).ToString().PadLeft(4, '0');

                //判断是否已结算
                string strSql = string.Format(@"select a.cfmxjylsh,a.jslsh,a.jsrq,a.djh,b.*  from ybfyjsdr a join ybmzzydjdr b on a.ybjzlsh = b.ybjzlsh where a.jzlsh = '{0}' and a.djhin = '{1}' and a.cxbz = 1  and b.cxbz = 1 ", jzlsh, djh);
                DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                WriteLog(strSql);
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
                string ybdjh = dr["djh"].ToString();
                string ybjzlsh_snyd = dr["ybjzlsh_snyd"].ToString();
                DQJBBZ = ds.Tables[0].Rows[0]["DQJBBZ"].ToString();


                #region 入参
                /*
                    1		就诊流水号	VARCHAR2(20)	NOT NULL	同登记时的就诊流水号
                    2		单据号	VARCHAR2(20)	NOT NULL	
                    3		结算日期	VARCHAR2(14)		
                    4		经办人	VARCHAR2(20)	NOT NULL	医疗机构操作员姓名
                    5		是否保存处方标志	VARCHAR2(3)		二级代码
                    6		开发商标志	VARCHAR2(20)		开发商标志：
                    01     东软
                    02     中软
                    03     其他
                    7	个人编号	VARCHAR2(10)	NOT NULL	
                    8	姓名	VARCHAR2(20)	NOT NULL	
                    9	卡号	VARCHAR2(18)	NOT NULL	
               */
                StringBuilder inputParam = new StringBuilder();
                if (DQJBBZ.Equals("1"))
                    inputParam.Append(ybjzlsh + "|");
                else
                {
                    ZXBM = dqbh;
                    inputParam.Append(ybjzlsh_snyd + "|");
                }
                inputParam.Append(ybdjh + "|");
                inputParam.Append(jsrq + "|");
                inputParam.Append(jbr + "|");
                inputParam.Append("0" + "|");
                inputParam.Append("03" + "|");
                inputParam.Append(grbh + "|");
                inputParam.Append(xm + "|");
                inputParam.Append(kh + "|");

                YWBH = "2430";
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
                    strSql = string.Format(@"insert into ybfyjsdr(jzlsh,ybjzlsh,jylsh,djhin,ylfze,zbxje,tcjjzf,dejjzf,zhzf,xjzf,
                                            gwybzjjzf,qybcylbxjjzf,zffy,dwfdfy,yyfdfy,mzjzfy,cxjfy,ylzlfy,blzlfy,fhjbylfy,
                                            qfbzfy,zzzyzffy,jrtcfy,tcfdzffy,ctcfdxfy,jrdebxfy,defdzffy,cdefdxfy,rgqgzffy,bcjsqzhye,
                                            bntczflj,bndezflj,bnczjmmztczflj,bngwybzzflj,bnzhzflj,bnzycslj,zycs,xm,kh,jsrq,
                                            yllb,yldylb,jbjgbm,ywzqh,jslsh,tsxx,djh,yyjylsh,grbhgl,yljgbm,
                                            ecbcje,mmqflj,jsfjylsh,grbh,czzcje,elmmxezc,elmmxesy,jmecbc,tjje,syjjzf,
                                            jjjmdbydje,jjjmdbedje,jjjmjbbcfwnje,jjjmjbbcfwwje,mgwxlcjjzf,jjjmzcfwwkbxfy,zftdjjzf,mfmzjj,jddbbcbcydzfje,jddbbcbcedzfje,
                                            jddbbcbcsdzfje,jdecbcbcydzfje,jdecbcbcedzfje,jdecbcbcsdzfje,jbbcbxbczcfwnfyydzfje,jbbcbxbczcfwnfyedzfje,bnzftdjjfylj,lxgbddtczf,qtjjzf,scryylbzjj,
                                            zhxjzffy,jbr,zhzbxje,qtybfy,cxbz,sysdate) 
                                            select jzlsh,ybjzlsh,jylsh,djhin,ylfze,zbxje,tcjjzf,dejjzf,zhzf,xjzf,
                                            gwybzjjzf,qybcylbxjjzf,zffy,dwfdfy,yyfdfy,mzjzfy,cxjfy,ylzlfy,blzlfy,fhjbylfy,
                                            qfbzfy,zzzyzffy,jrtcfy,tcfdzffy,ctcfdxfy,jrdebxfy,defdzffy,cdefdxfy,rgqgzffy,bcjsqzhye,
                                            bntczflj,bndezflj,bnczjmmztczflj,bngwybzzflj,bnzhzflj,bnzycslj,zycs,xm,kh,jsrq,
                                            yllb,yldylb,jbjgbm,ywzqh,jslsh,tsxx,djh,yyjylsh,grbhgl,yljgbm,
                                            ecbcje,mmqflj,jsfjylsh,grbh,czzcje,elmmxezc,elmmxesy,jmecbc,tjje,syjjzf,
                                            jjjmdbydje,jjjmdbedje,jjjmjbbcfwnje,jjjmjbbcfwwje,mgwxlcjjzf,jjjmzcfwwkbxfy,zftdjjzf,mfmzjj,jddbbcbcydzfje,jddbbcbcedzfje,
                                            jddbbcbcsdzfje,jdecbcbcydzfje,jdecbcbcedzfje,jdecbcbcsdzfje,jbbcbxbczcfwnfyydzfje,jbbcbxbczcfwnfyedzfje,bnzftdjjfylj,lxgbddtczf,qtjjzf,scryylbzjj,
                                            zhxjzffy,jbr,zhzbxje,qtybfy,0,'{2}' 
                                            from ybfyjsdr where jzlsh = '{0}' and djhin = '{1}' and cxbz = 1", jzlsh, djh, sysdate);
                    lijsmxcx.Add(strSql);
                    strSql = string.Format(@"update ybfyjsdr set cxbz = 2 where jzlsh = '{0}' and djhin = '{1}' and cxbz = 1", jzlsh, djh);
                    lijsmxcx.Add(strSql);
                    strSql = string.Format(@"delete from ybcfmxscfhdr where jzlsh = '{0}' and jylsh = '{1}' and cxbz = 1", jzlsh, cfmxjylsh);
                    lijsmxcx.Add(strSql);
                    strSql = string.Format(@"delete from ybcfmxscindr where jzlsh = '{0}' and jylsh = '{1}' and cxbz = 1", jzlsh, cfmxjylsh);
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

        #region 获取病种信息查询
        public static object[] YBBZCX(object[] objParam)
        {
            string yllb = objParam[0].ToString(); // 医疗类别
            string grbh = objParam[1].ToString(); //个人编号
            string jzbz = objParam[2].ToString();   //门诊住院标志 m-门诊 z-住院
            string splb = objParam[3].ToString();   //审批类别
            DataSet ds = null;
            string[] syl_mz = { "12", "13" };
            WriteLog("  获取病种信息查询|入参|" + yllb + "|" + grbh + "|" + jzbz + "|" + splb);

            string strSql = string.Empty;
            if (jzbz.ToUpper().Equals("M"))
            {
                if (syl_mz.Contains(yllb))
                {
                    strSql = string.Format(@"select dm,dmmc,pym,wbm from ybbzmrdr where yllb=12");
                    ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                    WriteLog("获取病种信息查询成功|");
                    return new object[] { 0, 1, ds.Tables[0] };
                }
                else
                {
                    strSql = string.Format(@"select dm,dmmc,pym,wbm from ybbzmrdr where yllb=11");
                    ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                    WriteLog("获取病种信息查询成功|");
                    return new object[] { 0, 1, ds.Tables[0] };
                }
            }
            else
            {
                strSql = string.Format(@"select dm,dmmc,pym,wbm from ybbzmrdr where yllb=11 ");
                ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                WriteLog("获取病种信息查询成功|");
                return new object[] { 0, 1, ds.Tables[0] };
            }
        }
        #endregion

        #region 住院登记
        public static object[] YBZYDJ(object[] objParam)
        {
            string sysdate = GetServerDateTime();
            WriteLog(sysdate + "  进入住院登记...");
            try
            {
                YWZQH = CliUtils.fLoginYbNo.Split('|')[0].ToString();    //业务周期号
            }
            catch
            {
                return new object[] { 0, 0, "医保未连接或初始化失败" };
            }
            try
            {
                CZYBH = CliUtils.fLoginUser;    //操作员工号
                ZXBM = "0000";
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
                string ybjzlsh_snyd = "";
                #endregion

                if (string.IsNullOrEmpty(jzlsh))
                    return new object[] { 0, 0, "就诊流水号不能为空" };
                if (string.IsNullOrEmpty(yllb))
                    return new object[] { 0, 0, "医疗类别不能为空" };
                if (string.IsNullOrEmpty(bzbm))
                    return new object[] { 0, 0, "病种不能为空" };
                if (string.IsNullOrEmpty(dgysdm))
                    return new object[] { 0, 0, "医生信息不能为空" };

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

                #region 异地医保
                if (!ssqh.Substring(0, 4).Equals("3606"))
                {
                    ZXBM = ssqh;
                    DQJBBZ = "2";
                }
                else
                    DQJBBZ = "1";
                #endregion

                #region 判断是否已入院
                string strSql = string.Format("select z1date as rysj,z1hznm,z1ksno,z1ksnm,cast(floor(rand()*100) as int) as z1bedn from zy01h where z1zyno = '{0}'", jzlsh);
                DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                if (ds.Tables[0].Rows.Count == 0)
                {
                    WriteLog(sysdate + "  就诊流水号" + jzlsh + "未登记入院");
                    return new object[] { 0, 0, "就诊流水号" + jzlsh + "未登记入院" };
                }
                string djsj = Convert.ToDateTime(ds.Tables[0].Rows[0]["rysj"]).ToString("yyyyMMddHHmmss");
                string ksmc = ds.Tables[0].Rows[0]["z1ksnm"].ToString();
                string ksbm = ds.Tables[0].Rows[0]["z1ksno"].ToString();
                string bedno = ds.Tables[0].Rows[0]["z1bedn"].ToString();

                string ybjzlsh = jzlsh + DateTime.Now.ToString("HHmmss");
                JYLSH = djsj + "-" + YLGHBH + "-" + new Random().Next(100).ToString().PadLeft(4, '0');
                #endregion

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

                #region 入参
                StringBuilder inputParam = new StringBuilder();
                /*
                 * 
                 * 00101990094941|21|20180917085733|A01.000|伤寒|
                 * Z018-01|内二科|54|010043|杨水娥|
                 * 管理员|00009988||||
                 * ||龚建华|520705581||
                 * |||||
                 * |||||
                 * |||
                  * 1		就诊流水号	VARCHAR2(20)	NOT NULL	同一家医院的就诊流水号是唯一的
                     2		医疗类别	VARCHAR2(3)	NOT NULL	二级代码
                     3		挂号/登记时间	VARCHAR2(14)	NOT NULL 	YYYYMMDDHH24MISS
                     4		病种编码	VARCHAR2(20)		门诊大病、慢病、住院不能为空。
                     5		病种名称	VARCHAR2(50)		有病种编码时，对应的病种名称不能为空。
                     6		科室编号	VARCHAR2(20)	NOT NULL	
                     7		科室名称	VARCHAR2(50)	NOT NULL	
                     8		床位号	VARCHAR2(20)		住院类需要传入床位号，如果入院时没有分配床位号，可以通过调用住院信息修改交易录入床位号
                     9		医师代码	VARCHAR2(20)	NOT NULL	（住院类的必须上传）入院登记不判断医师处方权。
                     10		医师姓名	VARCHAR2(50)	NOT NULL	（住院类的必须上传）入院登记不判断医师处方权。
                     11		经办人	VARCHAR2(50)	NOT NULL	医疗机构操作员姓名
                     12		个人编号	VARCHAR2(16)	NOT NULL	如果不读卡，先调用查询人员信息的交易，从人员信息中获得个人电脑编号
                     13		病区	VARCHAR2(20)		
                     14		转诊医院编号	VARCHAR2(50)		以备转外的费用放到医院报销时使用
                     15		转诊医院名称	VARCHAR2(50)		以备转外的费用放到医院报销时使用
                     16		备注	VARCHAR2(100)		
                     17		特殊费用标志	VARCHAR2(10)		二级代码，供外伤报销、转外报销等情况使用，若同时存在多种情况，则多个代码间用逗号分隔
                     18		姓名	VARCHAR2(20)	NOT NULL	异地结算
                     19		卡号	VARCHAR2(18)	NOT NULL	异地结算
                     20		挂号费	VARHCAR2(16)		（普通门诊时挂号费）本地必须输入
                     21		一般诊疗	VARHCAR2(16)		（普通门诊时挂号费）本地必须输入
                     22		门慢病种编码1	VARHCAR2(20)		【萍乡/景德镇】门慢需求要传多病种，其他地市不同传
                     23		门慢病种名称1	VARHCAR2(100)		【萍乡/景德镇】同上
                     24		门慢病种编码2	VARHCAR2(20)		【萍乡/景德镇】同上
                     25		门慢病种名称2	VARHCAR2(100)		【萍乡/景德镇】同上
                     26		门慢病种编码3	VARHCAR2(20)		【萍乡/景德镇】同上
                     27		门慢病种名称3	VARHCAR2(100)		【萍乡/景德镇】同上
                     28		门慢病种编码4	VARHCAR2(20)		【萍乡/景德镇】同上
                     29		门慢病种名称4	VARHCAR2(100)		【萍乡/景德镇】同上
                     30		主要病情描述	VARHCAR2(1000)		指医师根据患者描述对症状的详细描述。(全国异地参数)
                     31		病历号	VARHCAR2(20)		(全国异地参数)
                     32		急诊标志	VARHCAR2(3)		用于记录急诊就医。(全国异地参数)
                     33		外伤标识	VARHCAR2(3)		用于记录外伤就医。(全国异地参数)

                  */
                inputParam.Append(ybjzlsh + "|");   //门诊/住院流水号
                inputParam.Append(yllb + "|");      //医疗类别
                inputParam.Append(djsj + "|");    //挂号/登记时间
                inputParam.Append(bzbm + "|");      //病种编码
                inputParam.Append(bzmc + "|");      //病种名称
                inputParam.Append(ksbm + "|");      //科室编码
                inputParam.Append(ksmc + "|");      //科室名称
                inputParam.Append(bedno + "|");             //床位号
                inputParam.Append(dgysdm + "|");      //医生代码
                inputParam.Append(dgysxm + "|");      //医生姓名
                inputParam.Append(jbr + "|");
                inputParam.Append(grbh + "|");
                inputParam.Append("|");
                inputParam.Append("|");
                inputParam.Append("|");
                inputParam.Append("|");
                inputParam.Append("|");
                inputParam.Append(xm + "|");      //姓名
                inputParam.Append(kh + "|");      //卡号
                inputParam.Append("|");      //挂号费
                inputParam.Append("|");      //一般诊疗
                inputParam.Append("|");
                inputParam.Append("|");
                inputParam.Append("|");
                inputParam.Append("|");
                inputParam.Append("|");
                inputParam.Append("|");
                inputParam.Append("|");
                inputParam.Append("|");
                inputParam.Append("|"); //主要病情描述
                inputParam.Append("|");
                inputParam.Append("|");
                inputParam.Append("|");//外伤标识

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
                #endregion

                #region 医保登记
                List<string> liSQL = new List<string>();
                WriteLog(sysdate + "  住院登记|入参|" + inputData.ToString());
                StringBuilder outputData = new StringBuilder(10240);
                int i = BUSINESS_HANDLE(inputData, outputData);
                WriteLog(sysdate + "  住院登记(市本级)|出参|" + inputData.ToString());
                if (i != 0)
                {
                    WriteLog(sysdate + "  住院登记(市本级)失败|" + outputData.ToString());
                    return new object[] { 0, 0, outputData.ToString() };
                }
                #endregion

                #region 异地返回就诊流水号
                if (ydrybz.Equals("1"))
                {
                    string[] rParam = outputData.ToString().Split('^')[2].Split(';');
                    string[] fParam = rParam[0].Split('|');
                    ybjzlsh_snyd = fParam[0];
                    string mmbzbm = "";
                    string mmbzmc = "";
                    string mmbzlb = "";
                    string mmyllb = "";
                    string Mtmsg = "";

                    /*
                     第一位：登记的交易流水号；第二位开始：病种编码1|病种名称1;病种编码2|病种名称2;(两个以上病种用“;”隔开) 
                     例如：20160526360299335819|SCD0001|恶性肿瘤;SCD0011|慢性肝炎;SCD0010|糖尿病;SCD0005|尿毒症SCD0003|再生障碍性贫血;
                     */
                    if (fParam.Length > 1)
                    {
                        strSql = string.Format("delete from ybmxbdj where bxh='{0}'", grbh);
                        liSQL.Add(strSql);
                        mmbzbm = fParam[1];
                        mmbzmc = fParam[2];
                        strSql = string.Format(@"select * from ybbzmrdr where dm='{0}'", mmbzbm);
                        ds.Tables[0].Clear();
                        ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            mmbzlb = ds.Tables[0].Rows[0]["bzlb"].ToString();
                            mmyllb = ds.Tables[0].Rows[0]["yllb"].ToString();
                        }

                        strSql = string.Format(@"insert into  ybmxbdj(BXH,KH,XM,MMBZBM,MMBZMC,YLLB,BZLB) values('{0}','{1}','{2}','{3}','{4}','{5}','{6}') ",
                                                grbh, kh, xm, mmbzbm, mmbzmc, mmyllb, mmbzlb);
                        liSQL.Add(strSql);
                    }
                    if (rParam.Length > 1)
                    {
                        for (int jj = 1; jj < rParam.Length; jj++)
                        {
                            string[] TParam = rParam[jj].Split('|');
                            mmbzbm = fParam[0];
                            mmbzmc = fParam[1];
                            strSql = string.Format(@"select * from ybbzmrdr where dm='{0}'", mmbzbm);
                            ds.Tables[0].Clear();
                            ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                            if (ds.Tables[0].Rows.Count > 0)
                            {
                                mmbzlb = ds.Tables[0].Rows[0]["bzlb"].ToString();
                                mmyllb = ds.Tables[0].Rows[0]["yllb"].ToString();
                            }

                            strSql = string.Format(@"insert into  ybmxbdj(BXH,KH,XM,MMBZBM,MMBZMC,YLLB,BZLB) values('{0}','{1}','{2}','{3}','{4}','{5}','{6}') ",
                                                    grbh, kh, xm, mmbzbm, mmbzmc, mmyllb, mmbzlb);
                            liSQL.Add(strSql);
                        }
                    }

                    if (!string.IsNullOrEmpty(Mtmsg) && yllb.Equals("11"))
                    {
                        DialogResult result = MessageBox.Show("注:慢病患者!\r\n" + Mtmsg + "当前医疗类别[普通门诊],与慢病类别不符\r\n是否继续当前挂号?\r\n点击[是] 继续操作  点击[否] 取销当前操作", "门诊医保登记", MessageBoxButtons.YesNo, MessageBoxIcon.Information, MessageBoxDefaultButton.Button2);
                        if (result == DialogResult.No)
                        {
                            WriteLog(sysdate + "   进入门诊登记(异地医保)撤销操作...");
                            //门诊登记撤销
                            object[] objParam1 = { ybjzlsh, jbr, yllb, grbh, xm, kh, ssqh, ybjzlsh_snyd, DQJBBZ };
                            return NYBMZDJCX(objParam1);
                        }
                    }
                }
                #endregion

                #region 数据操作
                strSql = string.Format(@"insert into ybmzzydjdr(
                                         jzlsh,jylsh,ybjzlsh,yllb,ghdjsj,bzbm,bzmc,ksbh,ksmc,ysdm,
                                         ysxm,ghf,jbr,xm,grbh,kh,yldylb,xb,tcqh,zhye,
                                         jzbz,ydrybz,dqjbbz,sysdate,ybjzlsh_snyd) 
                                         values(
                                        '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}',
                                        '{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}',
                                        '{20}','{21}','{22}','{23}','{24}')"
                                        ,jzlsh, JYLSH, ybjzlsh, yllb, djsj, bzbm, bzmc, ksbm, ksmc, dgysdm,
                                        dgysxm, 0, jbr, xm, grbh, kh, yldylb, xb, ssqh, zhye,
                                        "z", ydrybz, DQJBBZ, sysdate, ybjzlsh_snyd);
                liSQL.Add(strSql);
                strSql = string.Format(@"update zy01h set z1rylb = '{0}', z1tcdq = '{1}', z1lyjg = '{2}', z1lynm = '{3}', z1ylno = '{4}'
                                         , z1ylnm = '{5}', z1bzno = '{6}', z1bznm = '{7}', z1ybno = '{8}' where z1comp = '{9}' and z1zyno = '{10}'"
                                        , yldylb, ssqh, lyjgdm, lyjgmc, yllb, yllbmc, bzbm, bzmc, grbh, CliUtils.fSiteCode, jzlsh);
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
                    object[] objZYDJCX = { jzlsh, ybjzlsh, yllb, grbh, xm, kh, ssqh, ybjzlsh_snyd, DQJBBZ };
                    NYBZYDJCX(objZYDJCX);
                    return new object[] { 0, 0, "住院登记(市本级)成功|本地数据操作失败|" + obj[2].ToString() };
                }
                #endregion
            }
            catch (Exception error)
            {
                WriteLog(sysdate + "  住院登记(市本级)|系统异常|" + error.Message);
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
                YWZQH = CliUtils.fLoginYbNo.Split('|')[0].ToString();    //业务周期号
            }
            catch
            {
                return new object[] { 0, 0, "医保未连接或初始化失败" };
            }
            try
            {
                string jzlsh = objParam[0].ToString();  // 就诊流水号
                CZYBH = CliUtils.fLoginUser;  // 操作员工号
                ZXBM = "0000";
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


                /*
                   1		就诊流水号	VARCHAR2(20)	NOT NULL	唯一
                   2		经办人	VARCHAR2(20)	NOT NULL	医疗机构操作员姓名
                   3	个人编号	VARCHAR2(10)	NOT NULL	异地结算
                   4	姓名	VARCHAR2(20)	NOT NULL	异地结算
                   5	卡号	VARCHAR2(18)	NOT NULL	异地结算
                */
                StringBuilder inputParam = new StringBuilder();
                if (DQJBBZ.Equals("1"))
                {
                    inputParam.Append(ybjzlsh + "|");
                }
                else
                {
                    ZXBM = dqbh;
                    inputParam.Append(ybjzlsh_snyd + "|");
                }
                inputParam.Append(jbr + "|");
                inputParam.Append(grbh + "|");
                inputParam.Append(xm + "|");
                inputParam.Append(kh + "|");

                YWBH = "2240";
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
                StringBuilder outputData = new StringBuilder(10240);
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
                    strSql = string.Format(@"update zy01h set z1lyjg='07',z1lynm='全自费' where z1zyno='{0}'", jzlsh);
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
            string sysdate = GetServerDateTime();
            WriteLog(sysdate + "  进入住院费用上传...");
            try
            {
                YWZQH = CliUtils.fLoginYbNo.Split('|')[0].ToString();    //业务周期号
            }
            catch
            {
                return new object[] { 0, 0, "医保未连接或初始化失败" };
            }

            try
            {
                string jzlsh = objParam[0].ToString();  //就诊流水号
                string ztjssj = objParam[1].ToString(); //中途结算时间
                string scrow = objParam[2].ToString();  //上传条数
                string cfrqbz = objParam[3].ToString(); //上传处方日期标志 0-正常处方日期，1-出院日期

                CZYBH = CliUtils.fLoginUser;  //操作员工号
                ZXBM = "0000";
                string jbr = CliUtils.fUserName;
                string cfsj = Convert.ToDateTime(sysdate).ToString("yyMMddHHmmss");
                string ztjssj1 = "";
                JYLSH = cfsj + "-" + YLGHBH + "-" + new Random().Next(100).ToString().PadLeft(4, '0');
                YWBH = "2310";

                if (!string.IsNullOrEmpty(ztjssj))
                {
                    ztjssj1 = Convert.ToDateTime(ztjssj).AddDays(1).ToString("yyyy-MM-dd");
                }

                #region 判断是否医保登记
                string strSql = string.Format("select a.*,b.z1ldat from ybmzzydjdr a left join zy01h b on a.jzlsh=b.z1zyno where a.jzlsh = '{0}' and jzbz='z' and a.cxbz = 1", jzlsh);
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
                string ksbh = dr1["ksbh"].ToString();
                string ksmc = dr1["ksmc"].ToString();
                string tcqh = dr1["tcqh"].ToString();
                string ybjzlsh_snyd = dr1["ybjzlsh_snyd"].ToString();
                DQJBBZ = dr1["dqjbbz"].ToString();
                string cyrq = dr1["z1ldat"].ToString();
                if (cfrqbz.Equals("1") && string.IsNullOrEmpty(cyrq))
                    return new object[] { 0, 0, "患者未出院不能按出院日期上传费用明细" };
                #endregion

                #region 上传前先撤销医保
                //医保费用撤销
                object[] objParam1 = { jzlsh };
                YBZYSFDJCX(objParam1);
                #endregion

                List<string> liSQL = new List<string>();
                #region 获取费用明细信息
                strSql = string.Format(@"select y.ybxmbh, y.ybxmmc, a.z3djxx as dj
                                        , sum(case left(a.z3endv, 1) when '4' then -a.z3jzcs else a.z3jzcs end) as sl
                                        , sum(case left(a.z3endv, 1) when '4' then -a.z3jzje else a.z3jzje end) as je
                                        , a.z3item as yyxmbh, a.z3name as yyxmmc, min(a.z3empn) as ysdm, min(a.z3kdys) as ysxm
                                        , z3sfno as sfno, y.sfxmzldm as ybsfxmzldm, y.sflbdm as ybsflbdm,max(a.z3date) as yysj,y.sfxmdjdm,y.gg,y.dw
                                        from zy03d a 
                                        left join ybhisdzdr y on a.z3item = y.hisxmbh 
                                        where a.z3ybup is null and left(a.z3kind, 1) in ('2', '4') and isnull(a.z3jshx,'')=''  and a.z3zyno = '{0}' ", jzlsh);
                if (!string.IsNullOrEmpty(ztjssj))
                    strSql += string.Format(@"and Convert(datetime,z3date)<'{0}' ", ztjssj1);
                strSql += string.Format(@"group by y.ybxmbh, y.ybxmmc, a.z3djxx, a.z3name, a.z3item
                                        ,z3sfno,y.sfxmzldm, y.sflbdm,y.sfxmdjdm,y.gg,y.dw,a.z3yzxh  
                                        having sum(case left(a.z3endv, 1) when '4' then -a.z3jzje else a.z3jzje end) > 0
                                        order by max(a.z3date)");
                ds.Tables.Clear();
                ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);

                List<string> li_inputData = new List<string>();
                List<string> liyyxmbh = new List<string>();
                List<string> liyyxmmc = new List<string>();
                List<string> liybxmbm = new List<string>();
                List<string> liybxmmc = new List<string>();
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
                            string xmlb = dr["ybsfxmzldm"].ToString(); // 收费项目种类代码 
                            string sflb = dr["ybsflbdm"].ToString();     // 收费类别代码
                            string ybxmbh = dr["ybxmbh"].ToString();
                            string ybxmmc = dr["ybxmmc"].ToString();
                            string yyxmbh = dr["yyxmbh"].ToString();
                            string yyxmmc = dr["yyxmmc"].ToString();
                            string ybxmdj = dr["sfxmdjdm"].ToString();
                            double dj = Convert.ToDouble(dr["dj"]);
                            double sl = Convert.ToDouble(dr["sl"]);
                            double je = Convert.ToDouble(dr["je"]);
                            string fysj = Convert.ToDateTime(dr["yysj"].ToString()).ToString("yyyyMMddHHmmss");
                            string gg = dr["gg"].ToString();
                            string jx = "";
                            string jldw = "";
                            string yfyl = "";
                            string ybcfh = jzlsh.Substring(4, 4) + DateTime.Now.ToString("yyMMddHHmmss") + index.ToString().PadLeft(3, '0');
                            string ypjldw = "";
                            decimal mcyl = 1;
                            string pd = "qd";
                            string yf = "";
                            string dw = "";
                            string txts = "";
                            string zcffbz1 = "1";
                            string qezfbz = "";
                            liyyxmbh.Add(yyxmbh);
                            liyyxmmc.Add(yyxmmc);
                            liybxmbm.Add(ybxmbh);
                            liybxmmc.Add(ybxmmc);

                            if (dr["sfno"].ToString() == "01" || dr["sfno"].ToString() == "02" || dr["sfno"].ToString() == "03")
                            {
                                ypjldw = dr["dw"].ToString();
                            }
                            index++;
                            #region 入参
                            /*
                                20180929369900146230|2|08|201809291529431|20180929135055|
                             * 101100100002|60011100000200000000|6院内会诊|45|1|
                             * 45|||1|qd|
                             * 010002|周长春||
                             * |M008200003-01|内三科||1|徐秀慧||
                             * |9800956230|吴文珊|018001401102761|$
                             * 20180917369900852431|2|27|201809180824041|20180917161903|
                             * 303040000000|60011020000600000000|眼部手术|200|2|
                             * 400|||1|qd|
                             * 010043|杨水娥|||Z018030003-01|
                             * 内二科||1|管理员||
                             * |9800956230|吴文珊|018001401102761|
                            1		就诊流水号	VARCHAR2(18)	NOT NULL	同登记时的就诊流水号
                            2		收费项目种类	VARCHAR2(3)	NOT NULL	二级代码
                            3		收费类别	VARCHAR2(3)	NOT NULL	二级代码
                            4		处方号	VARCHAR2(20)	NOT NULL	
                            5		处方日期	VARCHAR2(14)	NOT NULL	YYYYMMDDHH24MISS
                            6		医院收费项目内码	VARCHAR2(20)	NOT NULL	
                            7		收费项目中心编码	VARCHAR2(20)	NOT NULL	中心编号
                            8		医院收费项目名称	VARCHAR2(50)	NOT NULL	
                            9		单价	VARCHAR2(12)	NOT NULL	4位小数
                            10		数量	VARCHAR2(12)	NOT NULL	2位小数
                            11		金额	VARCHAR2(12)	NOT NULL	4位小数，金额与 (单价*数量)的差值不能大于0.01
                            12		剂型	VARCHAR2(20)		二级代码(收费类别为西药和中成药时必传)
                            13		规格	VARCHAR2(100)		二级代码(收费类别为西药和中成药时必传)
                            14		每次用量	VARCHAR2(12)	NOT NULL	2位小数
                            15		使用频次	VARCHAR2(20)	NOT NULL	
                            16		医师编码	VARCHAR2(20)	NOT NULL	
                            17		医师姓名	VARCHAR2(50)	NOT NULL	
                            18		用法	VARCHAR2(100)		
                            19		单位	VARCHAR2(20)		
                            20		科室编号	VARCHAR2(20)	NOT NULL	
                            21		科室名称	VARCHAR2(50)	NOT NULL	
                            22		执行天数	VARCHAR2(4)		
                            23		草药单复方标志	VARCHAR2(3)		二级代码
                            24		经办人	VARCHAR2(20)	NOT NULL	医疗机构操作员姓名
                            25		药品剂量单位	VARCHAR2(20)		当上传药品时非空
                            26		全额自费标志	VARCHAR2(3)		当传入自费标志，认为本项目按照自费处理，针对限制用药使用
                            27	个人编号	VARCHAR2(10)	NOT NULL	异地结算
                            28	姓名	VARCHAR2(20)	NOT NULL	异地结算
                            29	卡号	VARCHAR2(18)	NOT NULL	异地结算
                             */
                            StringBuilder inputParam = new StringBuilder();
                            if (DQJBBZ.Equals("2"))
                            {
                                inputParam.Append(ybjzlsh_snyd + "|");
                                ZXBM = tcqh;
                            }
                            else
                                inputParam.Append(ybjzlsh + "|");
                            inputParam.Append(xmlb + "|");
                            inputParam.Append(sflb + "|");
                            inputParam.Append(ybcfh + "|");
                            if (cfrqbz.Equals("1"))
                                inputParam.Append(Convert.ToDateTime(cyrq).ToString("yyyyMMddHHmmss") + "|");
                            else
                                inputParam.Append(fysj + "|");
                            inputParam.Append(yyxmbh + "|");
                            inputParam.Append(ybxmbh + "|");
                            inputParam.Append(yyxmmc + "|");
                            inputParam.Append(dj + "|");
                            inputParam.Append(sl + "|");
                            inputParam.Append(je + "|");
                            inputParam.Append(jx + "|");
                            inputParam.Append(gg + "|");
                            inputParam.Append(mcyl + "|");
                            inputParam.Append(pd + "|");
                            inputParam.Append(ysdm + "|");
                            inputParam.Append(ysxm + "|");
                            inputParam.Append(yf + "|");
                            inputParam.Append(dw + "|");
                            inputParam.Append(ksbh + "|");
                            inputParam.Append(ksmc + "|");
                            inputParam.Append(txts + "|");
                            inputParam.Append(zcffbz1 + "|");
                            inputParam.Append(jbr + "|");
                            inputParam.Append(ypjldw + "|");
                            inputParam.Append(qezfbz + "|");
                            inputParam.Append(grbh + "|");
                            inputParam.Append(xm + "|");
                            inputParam.Append(kh + "|$");
                            li_inputData.Add(inputParam.ToString());

                            strSql = string.Format(@"insert into ybcfmxscindr(jzlsh,jylsh,xm,kh,ybjzlsh,cfrq,yysfxmbm,yysfxmmc,sfxmzxbm,sfxmzxmc,
                                            dj,sl,je,jbr,sysdate,sflb,sfxmdj) values(
                                            '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}',
                                            '{10}','{11}','{12}','{13}','{14}','{15}','{16}')",
                                            jzlsh, JYLSH, xm, kh, ybjzlsh, fysj, yyxmbh, yyxmmc, ybxmbh, ybxmmc,
                                            dj, sl, je, jbr, sysdate, sflb, ybxmdj);
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

                YWBH = "2310";
                StringBuilder inputData = new StringBuilder();
                StringBuilder outputData = new StringBuilder(102400);
                int iscRow = int.Parse(scrow); // 每交上传条数
                int iTemp = 0;
                int mxindex = 0;
                #region 分段上传
                foreach (string inputData3 in li_inputData)
                {
                    if (iTemp <= iscRow)
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

                        WriteLog(sysdate + "  住院费用上传(分段)|入参|" + inputData1.ToString());
                        int i = BUSINESS_HANDLE(inputData1, outData);
                        if (i != 0)
                        {
                            WriteLog(sysdate + "  " + jzlsh + " 住院费用上传(分段)失败|" + outData.ToString());
                            return new object[] { 0, 0, "住院费用上传(分段)失败|" + outData.ToString().Split('^')[2] };
                        }
                        WriteLog(sysdate + "  住院费用上传(分段)|出参|" + outData.ToString());

                        string scfhxx = outData.ToString().Split('^')[2].TrimEnd('$');
                        if (!string.IsNullOrEmpty(scfhxx))
                        {
                            string[] zysfdjfhs = outData.ToString().Split('^')[2].TrimEnd('$').Split('$');

                            List<string> lizysfdjfh = new List<string>();

                            for (int j = 0; j < zysfdjfhs.Length; j++)
                            {
                                /*
                                 1		金额	VARCHAR2(16)		4位小数
                                 2		自理金额(自付金额)	VARCHAR2(16)		4位小数
                                 3		自费金额	VARCHAR2(16)		4位小数
                                 4		超限价自付金额	VARCHAR2(16)		4位小数，包含在自费金额中
                                 5		收费类别	VARCHAR2(3)		二级代码
                                 6		收费项目等级	VARCHAR2(3)		二级代码
                                 7		全额自费标志	VARCHAR2(3)		二级代码
                                 8		自理比例（自付比例）	VARCHAR2(5)		4位小数
                                 9		限价	VARCHAR2(16)		4位小数
                                 10		备注	VARCHAR2(200)		说明出现自费自理金额的原因，his端必须将此信息显示到前台。
                                 1	    疑点状态	VARCHAR2(3)		0：无疑点   1：有疑点
                                 2	    疑点（包含疑点ID和疑点说明）	VARCHAR2(4000)		疑点可能是多条，用“|”分隔，每条疑点里的疑点ID和疑点说明用“~”分隔例如：#疑点状态|疑点ID~疑点说明|疑点ID~疑点说明
                              */

                                string[] cffh = zysfdjfhs[j].Split('#');
                                string[] cfmx = cffh[0].Split('|');
                                outParams_fymx op = new outParams_fymx();
                                op.Je = cfmx[0];
                                op.Zlje = cfmx[1];
                                op.Zfje = cfmx[2];
                                op.Cxjzfje = cfmx[3];
                                op.Sfxmlb = cfmx[4];
                                op.Sfxmdj = cfmx[5];
                                op.Zfbz = cfmx[6];
                                op.Zlbz = cfmx[7];
                                op.Xjbz = cfmx[8]; //限价
                                op.Bz = cfmx[9];

                                if (cffh.Length > 2)
                                    op.Bz += cffh[2];

                                strSql = string.Format(@"insert into ybcfmxscfhdr(jzlsh,jylsh,ybjzlsh,yyxmdm,yyxmmc,yybxmbh,ybxmmc,je,zlje,zfje,
                                        cxjzfje,sflb,sfxmdj,qezfbz,zlbl,xj,bz) 
                                        values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}',
                                        '{10}','{11}','{12}','{13}','{14}','{15}','{16}')",
                                                jzlsh, JYLSH, ybjzlsh, liyyxmbh[mxindex], liyyxmmc[mxindex], liybxmbm[mxindex], liybxmmc[mxindex], op.Je, op.Zlje, op.Zfje,
                                                op.Cxjzfje, op.Sfxmlb, op.Sfxmdj, op.Zfbz, op.Zlbz, op.Xjbz, op.Bz);
                                liSQL.Add(strSql);
                                mxindex++;
                            }
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
                    WriteLog(sysdate + "  " + jzlsh + " 住院费用上传(补传、一次性上传)...");
                    StringBuilder inputData1 = new StringBuilder();

                    inputData1.Append(YWBH + "^");
                    inputData1.Append(YLGHBH + "^");
                    inputData1.Append(CZYBH + "^");
                    inputData1.Append(YWZQH + "^");
                    inputData1.Append(JYLSH + "^");
                    inputData1.Append(ZXBM + "^");
                    inputData1.Append(inputData.ToString().TrimEnd('$') + "^");
                    inputData1.Append(LJBZ + "^");

                    WriteLog(sysdate + " 住院费用上传(补传、一次性上传)|入参|" + inputData1.ToString());
                    int i = BUSINESS_HANDLE(inputData1, outData);
                    if (i != 0)
                    {
                        WriteLog(sysdate + "  " + jzlsh + " 住院费用上传(补传、一次性上传)失败|" + outData.ToString());
                        return new object[] { 0, 0, "住院费用上传(补传、一次性上传)失败|" + outData.ToString().Split('^')[2] };

                    }
                    WriteLog(sysdate + " 住院费用上传(补传、一次性上传)|出参|" + outData.ToString().Split('^')[2]);
                    string scfhxx = outData.ToString().Split('^')[2].TrimEnd('$');
                    if (!string.IsNullOrEmpty(scfhxx))
                    {
                        string[] zysfdjfhs = outData.ToString().Split('^')[2].TrimEnd('$').Split('$');
                        List<string> lizysfdjfh = new List<string>();

                        for (int j = 0; j < zysfdjfhs.Length; j++)
                        {
                            /*
                             * 200.0|0.0|0.0|0.0|3|1|0|0.0|20.0||$
                                300.0|0.0|0.0|0.0|2|1|0|0.0|0.0||$
                                50.0|0.0|0.0|0.0|2|1|0|0.0|0.0||$
                                50.0|0.0|0.0|0.0|2|1|0|0.0|0.0||$
                                100.0|10.0|0.0|0.0|2|2|0|0.1|0.0|## 上传的处方为乙类，产生自费、自理费用|$
                                130.0|13.0|0.0|0.0|2|2|0|0.1|0.0|## 上传的处方为乙类，产生自费、自理费用|$
                                70.0|0.0|70.0|0.0|2|9|0|1.0|0.0|## 上传的处方为全自费，产生自费费用|$
                              1		金额	VARCHAR2(16)		4位小数
                              2		自理金额(自付金额)	VARCHAR2(16)		4位小数
                              3		自费金额	VARCHAR2(16)		4位小数
                              4		超限价自付金额	VARCHAR2(16)		4位小数，包含在自费金额中
                              5		收费类别	VARCHAR2(3)		二级代码
                              6		收费项目等级	VARCHAR2(3)		二级代码
                              7		全额自费标志	VARCHAR2(3)		二级代码
                              8		自理比例（自付比例）	VARCHAR2(5)		4位小数
                              9		限价	VARCHAR2(16)		4位小数
                              10		备注	VARCHAR2(200)		说明出现自费自理金额的原因，his端必须将此信息显示到前台。
                              1	    疑点状态	VARCHAR2(3)		0：无疑点   1：有疑点
                              2	    疑点（包含疑点ID和疑点说明）	VARCHAR2(4000)		疑点可能是多条，用“|”分隔，每条疑点里的疑点ID和疑点说明用“~”分隔例如：#疑点状态|疑点ID~疑点说明|疑点ID~疑点说明
                           */

                            string[] cffh = zysfdjfhs[j].Split('#');
                            string[] cfmx = cffh[0].Split('|');
                            outParams_fymx op = new outParams_fymx();
                            op.Je = cfmx[0];
                            op.Zlje = cfmx[1];
                            op.Zfje = cfmx[2];
                            op.Cxjzfje = cfmx[3];
                            op.Sfxmlb = cfmx[4];
                            op.Sfxmdj = cfmx[5];
                            op.Zfbz = cfmx[6];
                            op.Zlbz = cfmx[7];
                            op.Xjbz = cfmx[8]; //限价
                            op.Bz = cfmx[9];

                            if (cffh.Length > 2)
                                op.Bz += cffh[2];

                            strSql = string.Format(@"insert into ybcfmxscfhdr(jzlsh,jylsh,ybjzlsh,yyxmdm,yyxmmc,yybxmbh,ybxmmc,je,zlje,zfje,
                                        cxjzfje,sflb,sfxmdj,qezfbz,zlbl,xj,bz) 
                                        values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}',
                                        '{10}','{11}','{12}','{13}','{14}','{15}','{16}')",
                                            jzlsh, JYLSH, ybjzlsh, liyyxmbh[mxindex], liyyxmmc[mxindex], liybxmbm[mxindex], liybxmmc[mxindex], op.Je, op.Zlje, op.Zfje,
                                            op.Cxjzfje, op.Sfxmlb, op.Sfxmdj, op.Zfbz, op.Zlbz, op.Xjbz, op.Bz);
                            liSQL.Add(strSql);
                            mxindex++;
                        }
                    }
                }
                #endregion

                #endregion

                #region 本地数据操作
                strSql = string.Format(@"update zy03d set z3ybup = '{0}' where z3ybup is null and LEFT(z3kind,1)=2 and z3zyno = '{1}' ", JYLSH, jzlsh);
                if (!string.IsNullOrEmpty(ztjssj))
                    strSql += string.Format(@"and Convert(datetime,z3date)<'{0}' ", ztjssj1);
                liSQL.Add(strSql);
                object[] obj = liSQL.ToArray();
                obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);

                if (obj[1].ToString() == "1")
                {
                    WriteLog(sysdate + "    住院费用上传成功|本地数据操作成功|" + outputData.ToString());
                    return new object[] { 0, 1, JYLSH };
                }
                else
                {
                    WriteLog(sysdate + "    住院费用上传成功|本地数据操作失败|" + obj[2].ToString());
                    object[] objFYMXCX = { ybjzlsh, JYLSH, jbr, grbh, xm, kh, tcqh, ybjzlsh_snyd, DQJBBZ };
                    NYBZYCFMXSCCX(objFYMXCX);
                    return new object[] { 0, 0, "住院费用上传失败|" + obj[2].ToString() };
                }
                #endregion
            }
            catch (Exception error)
            {
                WriteLog(sysdate + "  住院费用上传|接口异常|" + error.ToString());
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
                YWZQH = CliUtils.fLoginYbNo.Split('|')[0].ToString();    //业务周期号
            }
            catch
            {
                return new object[] { 0, 0, "医保未连接或初始化失败" };
            }
            try
            {
                string jzlsh = objParam[0].ToString(); // 就诊流水号
                CZYBH = CliUtils.fLoginUser; // 操作员工号 
                ZXBM = "0000";
                string jbr = CliUtils.fUserName;   // 经办人姓名
                JYLSH = DateTime.Now.ToString("yyyyMMddHHmmss") + "-" + YLGHBH + "-" + new Random().Next(100).ToString().PadLeft(4, '0');
                //结算情况下不能撤销
                string strSql = string.Format(@"select * from ybfyjsdr a where a.jzlsh = '{0}' and a.cxbz = 1 and ztjsbz=0", jzlsh);
                DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                if (ds != null && ds.Tables[0].Rows.Count > 0)
                    return new object[] { 0, 0, "该患者已做医保结算，请先撤销医保结算后再撤销费用明细" };

                //是否存在撤销明细
                strSql = string.Format(@"select * from ybcfmxscindr a where a.jzlsh = '{0}' and a.cxbz = 1 and isnull(ybdjh,'')=''", jzlsh);
                ds.Tables.Clear();
                ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                if (ds != null && ds.Tables[0].Rows.Count == 0)
                    return new object[] { 0, 0, "无费用撤销或已经撤销完成" };

                #region 医保登记信息
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
                #endregion

                StringBuilder inputParam = new StringBuilder();
                if (DQJBBZ.Equals("1"))
                    inputParam.Append(ybjzlsh + "|");
                else
                {
                    ZXBM = dqbh;
                    inputParam.Append(ybjzlsh_sndy + "|");
                }
                inputParam.Append("|");
                inputParam.Append(jbr + "|");
                inputParam.Append(grbh + "|");
                inputParam.Append(xm + "|");
                inputParam.Append(kh + "|");
                inputParam.Append(dqbh + "|");

                YWBH = "2320";
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
                    strSql = string.Format(@"delete from ybcfmxscindr where jzlsh = '{0}' and isnull(ybdjh,'')='' and cxbz = 1", jzlsh, sysdate);
                    liSql.Add(strSql);
                    strSql = string.Format(@"delete from ybcfmxscfhdr where jzlsh = '{0}' and isnull(ybdjh,'')='' and cxbz = 1", jzlsh, sysdate);
                    liSql.Add(strSql);
                    strSql = string.Format(@"update zy03d set z3ybup = null where z3ybup is not null and z3zyno = '{0}'
                                            and z3fphx not in (select b.z3fphx from zy03dw b where  b.z3endv like '1%' and b.z3zyno='{0}')", jzlsh);
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
            string sysdate = GetServerDateTime();
            WriteLog(sysdate + "  进入住院收费预结算(市本级)...");
            try
            {
                YWZQH = CliUtils.fLoginYbNo.Split('|')[0].ToString();    //业务周期号
            }
            catch
            {
                return new object[] { 0, 0, "医保未连接或初始化失败" };
            }
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
                string grzhye = "0.00";
                if (string.IsNullOrEmpty(jzlsh))
                    return new object[] { 0, 0, "就诊流水号不能为空" };
                if (string.IsNullOrEmpty(cyyy))
                    return new object[] { 0, 0, "出院原因不能为空" };

                jsrqsj = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                WriteLog(sysdate + "  " + jzlsh + " 进入住院收费预结算...");

                CZYBH = CliUtils.fLoginUser;   // 操作员工号
                ZXBM = "0000";
                string jbr = CliUtils.fUserName;     // 经办人姓名
                string cyrq = "";
                string jsrq = Convert.ToDateTime(jsrqsj).ToString("yyyyMMddHHmmss");
                string dqrq = Convert.ToDateTime(sysdate).ToString("yyyyMMddHHmmss");  // 当前日期
                if (ztjsbz.Equals("1"))
                    cyrq = "";  //出院日期
                else
                    cyrq = Convert.ToDateTime(cyrqsj).ToString("yyyyMMddHHmmss");
                string sslxdm = "0";    //手术类型代码  

                JYLSH = dqrq + "-" + YLGHBH + "-" + new Random().Next(100).ToString().PadLeft(4, '0');

                #region 中途结算，先撤销后结算
                if (ztjsbz.Equals("1"))
                {
                    //医保费用撤销
                    object[] objParam1 = { jzlsh, jsrqsj, "20", "0" };
                    YBZYSFDJCX(objParam1);
                    //处方重新上传
                    YBZYSFDJ(objParam1);
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
                string ryrq = dr["ghdjsj"].ToString();
                string dqbh = dr["tcqh"].ToString();
                string ybjzlsh_snyd = dr["ybjzlsh_snyd"].ToString();
                DQJBBZ = dr["dqjbbz"].ToString();
                #endregion

                #region 获取个人账户余额
                strSql = string.Format(@"select GRZHYE from ybickxx where grbh='{0}'",grbh);
                ds.Tables.Clear();
                ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                if (ds.Tables[0].Rows.Count > 0)
                    grzhye = ds.Tables[0].Rows[0]["GRZHYE"].ToString();
                #endregion

                #region 是否已经医保结算
                strSql = string.Format(@"select * from ybfyjsdr where jzlsh='{0}' and ztjsbz=0 and cxbz=1", jzlsh);
                ds.Tables.Clear();
                ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                if (ds.Tables[0].Rows.Count > 0)
                    return new object[] { 0, 0, "该患者已办理医保结算" };
                #endregion

                #region 入参
                /*
                        1		就诊流水号	VARCHAR2(20)	NOT NULL	同登记时的就诊流水号
                        2		单据号	VARCHAR2(20)	NOT NULL	固定传“0000”
                        3		医疗类别	VARCHAR2(3)	NOT NULL	二级代码
                        4		结算日期	VARCHAR2(14)	NOT NULL	YYYYMMDDHH24MISS
                        5		出院日期	VARCHAR2(14)	NOT NULL	YYYYMMDDHH24MISS
                        6		出院原因	VARCHAR2(3)	NOT NULL	二级代码
                        7		病种编码	VARCHAR2(20)		门诊大病、慢病、住院不能为空。
                        8		病种名称	VARCHAR2(50)		有病种编码时，对应的病种名称不能为空。
                        9		账户使用标志	VARCHAR2(3)	NOT NULL	二级代码 【景德镇】普通门诊和药店购药使用账户支付，普通住院不允许使用账户支付，门诊慢性病1类不允许使用个人账户支付，二类门诊慢性病强制使用账户支付个人负担部分，账户余额不足使用现金支付
                        10		中途结算标志	VARCHAR2(3)		二级代码
                        11		经办人	VARCHAR2(50)	NOT NULL	医疗机构操作员姓名
                        12		开发商标志	VARCHAR2(20)	NOT NULL	HIS开发商自定义的特殊标记，能够区分出不同的开发商即可 开发商标志： 01     东软 02     中软 03     其他
                        13		次要诊断编码1	VARCHAR2(20)		
                        14		次要诊断名称1	VARCHAR2(50)		
                        15		次要诊断编码2	VARCHAR2(20)		
                        16		次要诊断名称2	VARCHAR2(50)		
                        17		次要诊断编码3	VARCHAR2(20)		
                        18		次要诊断名称3	VARCHAR2(50)		
                        19		次要诊断编码4	VARCHAR2(20)		
                        20		次要诊断名称4	VARCHAR2(50)		
                        21		治疗方法	VARCHAR2(3)	医疗类别：单病种	代码为：01常规手术 02鼻内镜 03膀胱镜 04 腹腔镜手术 05 使用肛肠吻合器
                        22	个人编号	VARCHAR2(10)	NOT NULL	异地结算
                        23	姓名	VARCHAR2(20)	NOT NULL	异地结算
                        24	卡号	VARCHAR2(18)	NOT NULL	异地结算
                        25	胎儿数	VARCHAR2(3)	NOT NULL	生育住院时，生产病种必须上传。
                        26	手术类型	VARCHAR2(3)		省本级 二级代码 0-	非手术 1-手术 （定点做住院类（普通住院、放化疗住院）结算与预结算交易时必须传入手术类型（0-非手术 1-手术），其他情况可为空。）
                     */
                StringBuilder inputParam = new StringBuilder();
                if (DQJBBZ.Equals("1"))
                    inputParam.Append(ybjzlsh + "|");
                else
                {
                    ZXBM = dqbh;
                    inputParam.Append(ybjzlsh_snyd + "|");
 
                }
                inputParam.Append(djh + "|");
                inputParam.Append(yllb + "|");
                inputParam.Append(jsrq + "|");
                inputParam.Append(cyrq + "|");
                inputParam.Append(cyyy + "|");
                inputParam.Append(bzbm + "|");
                inputParam.Append(bzmc + "|");
                inputParam.Append(zhsybz + "|");
                inputParam.Append(ztjsbz + "|");
                inputParam.Append(jbr + "|"); //经办人
                inputParam.Append("03" + "|");
                inputParam.Append("|");
                inputParam.Append("|");
                inputParam.Append("|");
                inputParam.Append("|");
                inputParam.Append("|");
                inputParam.Append("|");
                inputParam.Append("|");
                inputParam.Append("|");
                inputParam.Append("|");
                inputParam.Append(grbh + "|");
                inputParam.Append(xm + "|");
                inputParam.Append(kh + "|");
                inputParam.Append("" + "|");    //胎儿数
                inputParam.Append(sslxdm + "|"); //手术类型

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
                #endregion

                #region  预结算
                WriteLog(sysdate + "  住院收费预结算|入参|" + inputData.ToString());
                StringBuilder outputData = new StringBuilder(102400);
                int i = BUSINESS_HANDLE(inputData, outputData);
                WriteLog(sysdate + "  住院收费预结算|出参|" + outputData.ToString());
                List<string> liSQL = new List<string>();
                if (i != 0)
                {
                    WriteLog(sysdate + "  住院收费预结算失败|" + outputData.ToString());
                    return new object[] { 0, 0, outputData.ToString() };
                }
                #endregion

                #region 出参
                string[] sfjsfh = outputData.ToString().Split('^')[2].Split('|');
                outParams_js js = new outParams_js();
                /*12519.94|10947.22|9600.08|0.0|1347.14|
                 * 1572.72|0.0|0.0|84.0|0.0|
                 * 0.0|0.0|0.0|8364.66|0.0|0.0|560.0|0.0|0.0|1066.67|0.0|0.0|0.0|0.0|0.0|0.0|0.0|0.0|0.0|0.0|0.0|0.0|0.0|何流水||21|21||||结算成功|20181004163252|1|||622669331|360699100002|0.0|0.00|0|622669331|0.00|0.00|0.0|0.0|0.00|0|0.00|0.00|0.00|0.00|0.00|0.00|0.00|0.00|0.00|0.00|0.00|0.00|0.00|0.00|0.00|0.00|0.00|0.00|0.00|0.00|0.0|0.0|
                 1		医疗费总额	VARCHAR2(16)		2位小数
                2		总报销金额	VARCHAR2(16)		总报销金额+现金=医疗费总额，2位小数
                3		   统筹基金支付	VARCHAR2(16)		2位小数
                4		大额基金支付	VARCHAR2(16)		2位小数
                5		   账户支付	VARCHAR2(16)		2位小数
                6		现金支付	VARCHAR2(16)		2位小数
                7		公务员补助基金支付	VARCHAR2(16)		2位小数
                8		企业补充医疗保险基金支付（九江、萍乡、鹰潭疾病补充保险支付）	VARCHAR2(16)		2位小数
                9		自费费用	VARCHAR2(16)		2位小数
                10		单位负担费用	VARCHAR2(16)		2位小数
                11		   医院负担费用	VARCHAR2(16)		2位小数
                12		   民政救助费用	VARCHAR2(16)		2位小数
                13		超限价费用	VARCHAR2(16)		2位小数
                14		乙类自理费用	VARCHAR2(16)		2位小数
                15		丙类自理费用	VARCHAR2(16)		2位小数
                16		符合基本医疗费用	VARCHAR2(16)		2位小数
                17		起付标准费用	VARCHAR2(16)		2位小数
                18		转诊转院自付费用	VARCHAR2(16)		2位小数
                19		进入统筹费用	VARCHAR2(16)		2位小数
                20		统筹分段自付费用	VARCHAR2(16)		2位小数
                21		超统筹封顶线费用	VARCHAR2(16)		2位小数
                22		进入大额报销费用	VARCHAR2(16)		2位小数
                23		大额分段自付费用	VARCHAR2(16)		2位小数
                24		超大额封顶线费用	VARCHAR2(16)		2位小数
                25		人工器官自付费用	VARHCAR2(16)		2位小数
                26		本次结算前帐户余额	VARHCAR2(16)		2位小数
                27		本年统筹支付累计(不含本次)	VARHCAR2(16)		2位小数
                28		本年大额支付累计(不含本次)	VARHCAR2(16)		2位小数
                29		本年城镇居民门诊统筹支付累计(不含本次)	VARHCAR2(16)		2位小数
                30		本年公务员补助支付累计(不含本次)	VARHCAR2(16)		2位小数
                31		本年账户支付累计(不含本次)	VARCHAR2(16)		2位小数
                32		本年住院次数累计(不含本次)	VARCHAR2(16)		2位小数
                33		住院次数	VARCHAR2(5)		
                34		姓名	VARCHAR2(50)		
                35		结算日期	VARCHAR2(14)		YYYYMMDDHH24MISS
                36		医疗类别	VARCHAR2(3)		二级代码
                37		医疗待遇类别	VARCHAR2(3)		二级代码
                38		经办机构编码	VARCHAR2(16)		二级代码
                39		业务周期号	VARCHAR2(36)		
                40		结算流水号	VARCHAR2(20)		获取结算单交易的入参
                41		提示信息	VARCHAR2(200)		His端必须将此信息显示到前台
                42		单据号	VARCHAR2(20)		
                43		交易类型	VARCHAR2(3)		二级代码 
                44		医院交易流水号	VARCHAR2(50)		
                45		有效标志	VARCHAR2(3)		二级代码 
                46		个人编号管理	VARCHAR2(10)		
                47		医疗机构编码	VARCHAR2(20)		
                48		     二次补偿金额	VARCHAR2(16)		2位小数
                49		门慢起付累计	VARCHAR2(16)		2位小数
                50		接收方交易流水号	VARCHAR2(50)		
                51		个人编号	VARCHAR2(16)		
                52		单病种补差	VARCHAR2(16)		2位小数【鹰潭专用】
                53		财政支出金额	VARCHAR2(16)		【萍乡】公立医院用【2位小数】
                54		二类门慢限额支出（景德镇）门慢限额支出（省本级）	VARCHAR2(16)		【景德镇】【省本级】专用
                55		二类门慢限额剩余	VARCHAR2(16)		【景德镇】【省本级】专用
                56		居民二次补偿（大病支付）	VARCHAR2(16)		【鹰潭】专用2位小数
                57		体检金额	VARCHAR2(16)		【九江】专用2位小数
                58		生育基金支付	VARCHAR2(16)		
                59		居民大病一段金额	VARCHAR2(16)		【九江、鹰潭居民】专用2位小数
                60		居民大病二段金额	VARCHAR2(16)		【九江、鹰潭居民】
                61		疾病补充范围内费用支付金额	VARCHAR2(16)		【九江/鹰潭/萍乡居民】
                62		疾病补充保险本次政策范围外费用支付金额	VARCHAR2(16)		【九江/鹰潭/萍乡居民】
                63		美国微笑列车基金支付	VARCHAR2(16)		【九江/鹰潭居民】
                64		九江居民政策范围外可报销费用	VARCHAR2(16)		【九江居民】
                65		政府兜底基金费用	VARCHAR2(16)		【萍乡/鹰潭/九江居民】
                66		免费门诊基金（余江）	VARCHAR2(16)		【鹰潭居民】
                67		建档大病补偿本次一段支付金额	VARCHAR2(16)		【鹰潭居民】
                68		建档大病补偿本次二段支付金额	VARCHAR2(16)		【鹰潭居民】
                69		建档大病补偿本次三段支付金额	VARCHAR2(16)		【鹰潭居民】
                70		建档二次补偿本次一段支付金额	VARCHAR2(16)		【鹰潭居民】
                71		建档二次补偿本次二段支付金额	VARCHAR2(16)		【鹰潭居民】
                72		建档二次补偿本次三段支付金额	VARCHAR2(16)		【鹰潭居民】
                73		疾病补充保险本次政策范围内费用一段支付金额	VARCHAR2(16)		【鹰潭居民】
                74		疾病补充保险本次政策范围内费用二段支付金额	VARCHAR2(16)		【鹰潭居民】
                75		本年政府兜底基金费用累计(不含本次)	VARCHAR2(16)		【九江居民】
                76		门慢限额	VARCHAR2(16)		【江西省本级】
                77		      企业军转干基金支付	VARCHAR2(16)		【鹰潭】
                78		其他基金支付金额	VARCHAR2(16)		异地就医就医地返给定点出参；本地就医返参为0
                79		伤残人员医疗保障基金	VARCHAR2(16)		异地就医就医地返给定点出参；本地就医返参为0
             */

                js.Ylfze = sfjsfh[0];         //医疗费总费用
                js.Zbxje = sfjsfh[1];         //总报销金额
                js.Tcjjzf = sfjsfh[2];        //统筹支出
                js.Dejjzf = sfjsfh[3];        //大病支出
                js.Zhzf = sfjsfh[4];          //本次帐户支付
                js.Xjzf = sfjsfh[5];         //个人现金支付
                js.Gwybzjjzf = sfjsfh[6];     //公务员补助支付金额
                js.Qybcylbxjjzf = sfjsfh[7];  //企业补充支付金额
                js.Zffy = sfjsfh[8];          //自费费用
                js.Dwfdfy = sfjsfh[9];
                js.Yyfdfy = sfjsfh[10];
                js.Mzjzfy = sfjsfh[11];       //民政救助费用
                js.Cxjfy = sfjsfh[12];
                js.Ylzlfy = sfjsfh[13];
                js.Blzlfy = sfjsfh[14];
                js.Fhjjylfy = sfjsfh[15];
                js.Qfbzfy = sfjsfh[16];
                js.Zzzyzffy = sfjsfh[17];
                js.Jrtcfy = sfjsfh[18];
                js.Tcfdzffy = sfjsfh[19];
                js.Ctcfdxfy = sfjsfh[20];
                js.Jrdebxfy = sfjsfh[21];
                js.Defdzffy = sfjsfh[22];
                js.Cdefdxfy = sfjsfh[23];
                js.Rgqgzffy = sfjsfh[24];
                js.Bcjsqzhye = sfjsfh[25];
                js.Bntczflj = sfjsfh[26];
                js.Bndezflj = sfjsfh[27];
                js.Bnczjmmztczflj = sfjsfh[28];
                js.Bngwybzzflj = sfjsfh[29];
                js.Bnzhzflj = sfjsfh[30];
                js.Bnzycslj = sfjsfh[31];
                js.Zycs = sfjsfh[32];
                js.Xm = sfjsfh[33];
                js.Jsrq = sfjsfh[34];
                js.Yllb = sfjsfh[35];
                js.Yldylb = sfjsfh[36];
                js.Jbjgbm = sfjsfh[37];
                js.Ywzqh = sfjsfh[38];
                js.Jslsh = sfjsfh[39];
                js.Tsxx = sfjsfh[40];
                js.Djh = sfjsfh[41];
                js.Jyxl = sfjsfh[42];
                js.Yyjylsh = sfjsfh[43];
                js.Yxbz = sfjsfh[44];
                js.Grbhgl = sfjsfh[45];
                js.Yljgbm = sfjsfh[46];
                js.Ecbcje = sfjsfh[47];
                js.Mmqflj = sfjsfh[48];
                js.Jsfjylsh = sfjsfh[49];
                js.Grbh = sfjsfh[50];
                js.Dbzbc = sfjsfh[51];
                js.Czzcje = sfjsfh[52];
                js.Elmmxezc = sfjsfh[53];
                js.Elmmxesy = sfjsfh[54];
                js.Jmecbc = sfjsfh[55];
                js.Tjje = sfjsfh[56];
                js.Syjjzf = sfjsfh[57];
                js.Jmdbydje = sfjsfh[58];
                js.Jmdbedje = sfjsfh[59];
                js.Jbbcfwnfyzfje = sfjsfh[60];
                js.Jbbcybbczcfywfyzf = sfjsfh[61];
                js.Mgwxlcjjzf = sfjsfh[62];
                js.Jjjmzcfwwkbxfy = sfjsfh[63];
                js.Zftdfy = sfjsfh[64];
                js.Mfmzjj = sfjsfh[65];
                js.Jddbbcbcydzfje = sfjsfh[66];
                js.Jddbbcbcedzfje = sfjsfh[67];
                js.Jddbbcbcsdzfje = sfjsfh[68];
                js.Jdecbcbcydzfje = sfjsfh[69];
                js.Jdecbcbcedzfje = sfjsfh[70];
                js.Jdecbcbcsdzfje = sfjsfh[71];
                js.Jbbcbxbczcfwnfyydzfje = sfjsfh[72];
                js.Jbbcbxbczcfwnfyedzfje = sfjsfh[73];
                js.Bnzftdjjfylj = sfjsfh[74];
                js.Mmxe = sfjsfh[75];
                js.Jzgbbzzf = sfjsfh[76];
                js.Qtjjzf = sfjsfh[77];
                js.Scryylbzjj = sfjsfh[78];
                if (DQJBBZ.Equals("2"))
                    js.Bcjsqzhye = grzhye;

                if (string.IsNullOrEmpty(js.Scryylbzjj))
                    js.Scryylbzjj = "0.00";
                js.Lxgbdttcjjzf = "0.00";

                string[] sDBRY = new string[] { "80", "83", "84", "85", "86", "87" };
                if (sDBRY.Contains(js.Yldylb))
                {
                    /*
                    * 建档立卡人员除个人自负部分的费用外，均先由医院垫付
                    */
                    js.Ybxjzf = js.Xjzf;
                    js.Zhzbxje = (Convert.ToDecimal(js.Ylfze) - Convert.ToDecimal(js.Ybxjzf)).ToString();
                }
                else
                {
                    /*
                    * 非建档立卡人员（含民政救助对象）住院（含门诊慢性病治疗）的医院垫付金额为：
                    * 统筹支出+账户支付+医院负担费用+民政救助费用+二次补偿金额+企业军转干基金支付。
                    */
                    js.Zhzbxje = (Convert.ToDecimal(js.Tcjjzf) + Convert.ToDecimal(js.Zhzf) + Convert.ToDecimal(js.Yyfdfy) + Convert.ToDecimal(js.Mzjzfy)
                                 + Convert.ToDecimal(js.Ecbcje) + Convert.ToDecimal(js.Jzgbbzzf)).ToString();
                    //整合后现金支付=总费用-总报销金额
                    js.Ybxjzf = (Convert.ToDecimal(js.Ylfze) - Convert.ToDecimal(js.Zhzbxje)).ToString();
                }
                //其他医保支付=总费用-统筹-整合后现金支付-账户支付
                js.Qtybzf = (Convert.ToDecimal(js.Ylfze) - Convert.ToDecimal(js.Tcjjzf) - Convert.ToDecimal(js.Ybxjzf) - Convert.ToDecimal(js.Zhzf)).ToString(); ;

                #region 出参格式
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
                #endregion

                string strValue = js.Ylfze + "|" + js.Zhzbxje + "|" + js.Tcjjzf + "|" + js.Dejjzf + "|" + js.Zhzf + "|" +
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

                WriteLog(sysdate + "  住院收费预结算|整合出参|" + strValue);
                #endregion

                #region 数据操作
                strSql = string.Format(@"delete from ybfyyjsdr where jzlsh='{0}'", jzlsh);
                liSQL.Add(strSql);

                strSql = string.Format(@"insert into ybfyyjsdr(jzlsh,ybjzlsh,jylsh,djhin,ylfze,zbxje,tcjjzf,dejjzf,zhzf,xjzf,
                                        gwybzjjzf,qybcylbxjjzf,zffy,dwfdfy,yyfdfy,mzjzfy,cxjfy,ylzlfy,blzlfy,fhjbylfy,
                                        qfbzfy,zzzyzffy,jrtcfy,tcfdzffy,ctcfdxfy,jrdebxfy,defdzffy,cdefdxfy,rgqgzffy,bcjsqzhye,
                                        bntczflj,bndezflj,bnczjmmztczflj,bngwybzzflj,bnzhzflj,bnzycslj,zycs,xm,kh,jsrq,
                                        yllb,yldylb,jbjgbm,ywzqh,jslsh,tsxx,djh,yyjylsh,grbhgl,yljgbm,
                                        ecbcje,mmqflj,jsfjylsh,grbh,czzcje,elmmxezc,elmmxesy,jmecbc,tjje,syjjzf,
                                        jjjmdbydje,jjjmdbedje,jjjmjbbcfwnje,jjjmjbbcfwwje,mgwxlcjjzf,jjjmzcfwwkbxfy,zftdjjzf,mfmzjj,jddbbcbcydzfje,jddbbcbcedzfje,
                                        jddbbcbcsdzfje,jdecbcbcydzfje,jdecbcbcedzfje,jdecbcbcsdzfje,jbbcbxbczcfwnfyydzfje,jbbcbxbczcfwnfyedzfje,bnzftdjjfylj,lxgbddtczf,qtjjzf,scryylbzjj,
                                        zhxjzffy,qtybfy,sysdate,zhzbxje,jbr,ztjsbz,ryrq,cyrq,cyyy,jzgbjjzf,
                                        dbzbc)
                                        values('{0}','{1}','{2}','{3}','{4}','{5}' ,'{6}','{7}','{8}' ,'{9}' ,
                                        '{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}',
                                        '{20}','{21}','{22}','{23}','{24}','{25}','{26}','{27}','{28}','{29}',
                                        '{30}','{31}','{32}','{33}','{34}','{35}','{36}','{37}','{38}','{39}',
                                        '{40}','{41}','{42}','{43}','{44}','{45}','{46}','{47}','{48}','{49}',
                                        '{50}','{51}','{52}','{53}','{54}','{55}','{56}','{57}','{58}','{59}',
                                        '{60}','{61}','{62}','{63}','{64}','{65}','{66}','{67}','{68}','{69}',
                                        '{70}','{71}','{72}','{73}','{74}','{75}','{76}','{77}','{78}','{79}',
                                        '{80}','{81}','{82}','{83}','{84}','{85}','{86}','{87}','{88}','{89}',
                                        '{90}')",
                                        jzlsh, ybjzlsh, JYLSH, djh, js.Ylfze, js.Zbxje, js.Tcjjzf, js.Dejjzf, js.Zhzf, js.Xjzf,
                                        js.Gwybzjjzf, js.Qybcylbxjjzf, js.Zffy, js.Dwfdfy, js.Yyfdfy, js.Mzjzfy, js.Cxjfy, js.Ylzlfy, js.Blzlfy, js.Fhjjylfy,
                                        js.Qfbzfy, js.Zzzyzffy, js.Jrtcfy, js.Tcfdzffy, js.Ctcfdxfy, js.Jrdebxfy, js.Defdzffy, js.Cdefdxfy, js.Rgqgzffy, js.Bcjsqzhye,
                                        js.Bntczflj, js.Bndezflj, js.Bnczjmmztczflj, js.Bngwybzzflj, js.Bnzhzflj, js.Bnzycslj, js.Zycs, js.Xm, js.Kh, js.Jsrq,
                                        js.Yllb, js.Yldylb, js.Jbjgbm, js.Ywzqh, js.Jslsh, js.Tsxx, js.Djh, js.Yyjylsh, js.Grbhgl, js.Yljgbm,
                                        js.Ecbcje, js.Mmqflj, js.Jsfjylsh, js.Grbh, js.Czzcje, js.Elmmxezc, js.Elmmxesy, js.Jmecbc, js.Tjje, js.Syjjzf,
                                        js.Jmdbydje, js.Jmdbedje, js.Jbbcfwnfyzfje, js.Jbbcybbczcfywfyzf, js.Mgwxlcjjzf, js.Jjjmzcfwwkbxfy, js.Zftdfy, js.Mfmzjj, js.Jddbbcbcydzfje, js.Jddbbcbcedzfje,
                                        js.Jddbbcbcsdzfje, js.Jdecbcbcydzfje, js.Jdecbcbcedzfje, js.Jdecbcbcsdzfje, js.Jbbcbxbczcfwnfyydzfje, js.Jbbcbxbczcfwnfyedzfje, js.Bnzftdjjfylj, js.Lxgbdttcjjzf, js.Qtjjzf, js.Scryylbzjj,
                                        js.Ybxjzf, js.Qtybzf, sysdate, js.Zhzbxje, jbr, ztjsbz, ryrq, cyrq, cyyy, js.Jzgbbzzf,
                                        js.Dbzbc);
                liSQL.Add(strSql);
               
                object[] obj = liSQL.ToArray();
                obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);
                if (obj[1].ToString() == "1")
                {
                    WriteLog(sysdate + "  住院收费预结算成功|本地数据操作成功|" + outputData.ToString());
                    return new object[] { 0, 1, strValue };
                }
                else
                {
                    WriteLog(sysdate + "  住院收费预结算成功|本地数据操作失败|" + obj[2].ToString());
                    return new object[] { 0, 0, obj[2].ToString() };
                }
                #endregion
            }
            catch (Exception error)
            {
                WriteLog(sysdate + "  住院收费预结算|系统异常|" + error.Message);
                return new object[] { 0, 2, error.Message };
            }
        }
        #endregion

        #region 住院收费结算
        public static object[] YBZYSFJS(object[] objParam)
        {
             DialogResult dresult = MessageBox.Show("请确认医保卡是否正常插入", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
             if (dresult == DialogResult.Yes)
             {
                 string sysdate = GetServerDateTime();
                 WriteLog(sysdate + "  进入住院收费结算...");
                 try
                 {
                     YWZQH = CliUtils.fLoginYbNo.Split('|')[0].ToString();    //业务周期号
                 }
                 catch
                 {
                     return new object[] { 0, 0, "医保未连接或初始化失败" };
                 }
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
                     string grzhye = "0.00";
                     if (string.IsNullOrEmpty(jzlsh))
                         return new object[] { 0, 0, "就诊流水号不能为空" };
                     if (string.IsNullOrEmpty(djh))
                         return new object[] { 0, 0, "单据号不能为空" };

                     CZYBH = CliUtils.fLoginUser;   // 操作员工号
                     YWZQH = CliUtils.fLoginYbNo.Split('|')[0].ToString();   // 业务周期号
                     ZXBM = "0000";
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
                     string ryrq = dr["ghdjsj"].ToString();
                     string dqbh = dr["tcqh"].ToString();
                     string ybjzlsh_snyd = dr["ybjzlsh_snyd"].ToString();
                     DQJBBZ = dr["dqjbbz"].ToString();
                     #endregion

                     #region 获取个人账户余额
                     strSql = string.Format(@"select GRZHYE from ybickxx where grbh='{0}'", grbh);
                     ds.Tables.Clear();
                     ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                     if (ds.Tables[0].Rows.Count > 0)
                         grzhye = ds.Tables[0].Rows[0]["GRZHYE"].ToString();
                     #endregion

                     #region 是否已经医保结算
                     strSql = string.Format(@"select * from ybfyjsdr where jzlsh='{0}' and ztjsbz=0 and cxbz=1", jzlsh);
                     ds.Tables.Clear();
                     ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                     if (ds.Tables[0].Rows.Count > 0)
                         return new object[] { 0, 0, "该患者已办理医保结算" };
                     #endregion

                     #region 入参
                     /*
                        1		就诊流水号	VARCHAR2(20)	NOT NULL	同登记时的就诊流水号
                        2		单据号	VARCHAR2(20)	NOT NULL	固定传“0000”
                        3		医疗类别	VARCHAR2(3)	NOT NULL	二级代码
                        4		结算日期	VARCHAR2(14)	NOT NULL	YYYYMMDDHH24MISS
                        5		出院日期	VARCHAR2(14)	NOT NULL	YYYYMMDDHH24MISS
                        6		出院原因	VARCHAR2(3)	NOT NULL	二级代码
                        7		病种编码	VARCHAR2(20)		门诊大病、慢病、住院不能为空。
                        8		病种名称	VARCHAR2(50)		有病种编码时，对应的病种名称不能为空。
                        9		账户使用标志	VARCHAR2(3)	NOT NULL	二级代码 【景德镇】普通门诊和药店购药使用账户支付，普通住院不允许使用账户支付，门诊慢性病1类不允许使用个人账户支付，二类门诊慢性病强制使用账户支付个人负担部分，账户余额不足使用现金支付
                        10		中途结算标志	VARCHAR2(3)		二级代码
                        11		经办人	VARCHAR2(50)	NOT NULL	医疗机构操作员姓名
                        12		开发商标志	VARCHAR2(20)	NOT NULL	HIS开发商自定义的特殊标记，能够区分出不同的开发商即可 开发商标志： 01     东软 02     中软 03     其他
                        13		次要诊断编码1	VARCHAR2(20)		
                        14		次要诊断名称1	VARCHAR2(50)		
                        15		次要诊断编码2	VARCHAR2(20)		
                        16		次要诊断名称2	VARCHAR2(50)		
                        17		次要诊断编码3	VARCHAR2(20)		
                        18		次要诊断名称3	VARCHAR2(50)		
                        19		次要诊断编码4	VARCHAR2(20)		
                        20		次要诊断名称4	VARCHAR2(50)		
                        21		治疗方法	VARCHAR2(3)	医疗类别：单病种	代码为：01常规手术 02鼻内镜 03膀胱镜 04 腹腔镜手术 05 使用肛肠吻合器
                        22	个人编号	VARCHAR2(10)	NOT NULL	异地结算
                        23	姓名	VARCHAR2(20)	NOT NULL	异地结算
                        24	卡号	VARCHAR2(18)	NOT NULL	异地结算
                        25	胎儿数	VARCHAR2(3)	NOT NULL	生育住院时，生产病种必须上传。
                        26	手术类型	VARCHAR2(3)		省本级 二级代码 0-	非手术 1-手术 （定点做住院类（普通住院、放化疗住院）结算与预结算交易时必须传入手术类型（0-非手术 1-手术），其他情况可为空。）
                     */
                     StringBuilder inputParam = new StringBuilder();
                     if (DQJBBZ.Equals("1"))
                         inputParam.Append(ybjzlsh + "|");
                     else
                     {
                         ZXBM = dqbh;
                         inputParam.Append(ybjzlsh_snyd + "|");

                     }
                     inputParam.Append(djh + "|");
                     inputParam.Append(yllb + "|");
                     inputParam.Append(jsrq + "|");
                     inputParam.Append(cyrq + "|");
                     inputParam.Append(cyyy + "|");
                     inputParam.Append(bzbm + "|");
                     inputParam.Append(bzmc + "|");
                     inputParam.Append(zhsybz + "|");
                     inputParam.Append(ztjsbz + "|");
                     inputParam.Append(jbr + "|"); //经办人
                     inputParam.Append("03" + "|");
                     inputParam.Append("|");
                     inputParam.Append("|");
                     inputParam.Append("|");
                     inputParam.Append("|");
                     inputParam.Append("|");
                     inputParam.Append("|");
                     inputParam.Append("|");
                     inputParam.Append("|");
                     inputParam.Append("|");
                     inputParam.Append(grbh + "|");
                     inputParam.Append(xm + "|");
                     inputParam.Append(kh + "|");
                     inputParam.Append("" + "|");    //胎儿数
                     inputParam.Append(sslxdm + "|"); //手术类型

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
                     #endregion

                     #region 结算
                     WriteLog(sysdate + "  住院收费结算|入参|" + inputData.ToString());
                     StringBuilder outputData = new StringBuilder(102400);
                     int i = BUSINESS_HANDLE(inputData, outputData);
                     WriteLog(sysdate + "  住院收费结算|出参|" + outputData.ToString());
                     List<string> liSQL = new List<string>();
                     if (i != 0)
                     {
                         WriteLog(sysdate + "  住院收费结算失败|" + outputData.ToString());
                         return new object[] { 0, 0, outputData.ToString() };
                     }
                     #endregion

                     #region 出参
                     string[] sfjsfh = outputData.ToString().Split('^')[2].Split('|');
                     outParams_js js = new outParams_js();
                     /*
                      1		医疗费总额	VARCHAR2(16)		2位小数
                     2		总报销金额	VARCHAR2(16)		总报销金额+现金=医疗费总额，2位小数
                     3		统筹基金支付	VARCHAR2(16)		2位小数
                     4		大额基金支付	VARCHAR2(16)		2位小数
                     5		账户支付	VARCHAR2(16)		2位小数
                     6		现金支付	VARCHAR2(16)		2位小数
                     7		公务员补助基金支付	VARCHAR2(16)		2位小数
                     8		企业补充医疗保险基金支付（九江、萍乡、鹰潭疾病补充保险支付）	VARCHAR2(16)		2位小数
                     9		自费费用	VARCHAR2(16)		2位小数
                     10		单位负担费用	VARCHAR2(16)		2位小数
                     11		医院负担费用	VARCHAR2(16)		2位小数
                     12		民政救助费用	VARCHAR2(16)		2位小数
                     13		超限价费用	VARCHAR2(16)		2位小数
                     14		乙类自理费用	VARCHAR2(16)		2位小数
                     15		丙类自理费用	VARCHAR2(16)		2位小数
                     16		符合基本医疗费用	VARCHAR2(16)		2位小数
                     17		起付标准费用	VARCHAR2(16)		2位小数
                     18		转诊转院自付费用	VARCHAR2(16)		2位小数
                     19		进入统筹费用	VARCHAR2(16)		2位小数
                     20		统筹分段自付费用	VARCHAR2(16)		2位小数
                     21		超统筹封顶线费用	VARCHAR2(16)		2位小数
                     22		进入大额报销费用	VARCHAR2(16)		2位小数
                     23		大额分段自付费用	VARCHAR2(16)		2位小数
                     24		超大额封顶线费用	VARCHAR2(16)		2位小数
                     25		人工器官自付费用	VARHCAR2(16)		2位小数
                     26		本次结算前帐户余额	VARHCAR2(16)		2位小数
                     27		本年统筹支付累计(不含本次)	VARHCAR2(16)		2位小数
                     28		本年大额支付累计(不含本次)	VARHCAR2(16)		2位小数
                     29		本年城镇居民门诊统筹支付累计(不含本次)	VARHCAR2(16)		2位小数
                     30		本年公务员补助支付累计(不含本次)	VARHCAR2(16)		2位小数
                     31		本年账户支付累计(不含本次)	VARCHAR2(16)		2位小数
                     32		本年住院次数累计(不含本次)	VARCHAR2(16)		2位小数
                     33		住院次数	VARCHAR2(5)		
                     34		姓名	VARCHAR2(50)		
                     35		结算日期	VARCHAR2(14)		YYYYMMDDHH24MISS
                     36		医疗类别	VARCHAR2(3)		二级代码
                     37		医疗待遇类别	VARCHAR2(3)		二级代码
                     38		经办机构编码	VARCHAR2(16)		二级代码
                     39		业务周期号	VARCHAR2(36)		
                     40		结算流水号	VARCHAR2(20)		获取结算单交易的入参
                     41		提示信息	VARCHAR2(200)		His端必须将此信息显示到前台
                     42		单据号	VARCHAR2(20)		
                     43		交易类型	VARCHAR2(3)		二级代码 
                     44		医院交易流水号	VARCHAR2(50)		
                     45		有效标志	VARCHAR2(3)		二级代码 
                     46		个人编号管理	VARCHAR2(10)		
                     47		医疗机构编码	VARCHAR2(20)		
                     48		二次补偿金额	VARCHAR2(16)		2位小数
                     49		门慢起付累计	VARCHAR2(16)		2位小数
                     50		接收方交易流水号	VARCHAR2(50)		
                     51		个人编号	VARCHAR2(16)		
                     52		单病种补差	VARCHAR2(16)		2位小数【鹰潭专用】
                     53		财政支出金额	VARCHAR2(16)		【萍乡】公立医院用【2位小数】
                     54		二类门慢限额支出（景德镇）
                     门慢限额支出（省本级）	VARCHAR2(16)		【景德镇】【省本级】专用
                     55		二类门慢限额剩余	VARCHAR2(16)		【景德镇】【省本级】专用
                     56		居民二次补偿（大病支付）	VARCHAR2(16)		【鹰潭】专用2位小数
                     57		体检金额	VARCHAR2(16)		【九江】专用2位小数
                     58		生育基金支付	VARCHAR2(16)		
                     59		居民大病一段金额	VARCHAR2(16)		【九江、鹰潭居民】专用2位小数
                     60		居民大病二段金额	VARCHAR2(16)		【九江、鹰潭居民】
                     61		疾病补充范围内费用支付金额	VARCHAR2(16)		【九江/鹰潭/萍乡居民】
                     62		疾病补充保险本次政策范围外费用支付金额	VARCHAR2(16)		【九江/鹰潭/萍乡居民】
                     63		美国微笑列车基金支付	VARCHAR2(16)		【九江/鹰潭居民】
                     64		九江居民政策范围外可报销费用	VARCHAR2(16)		【九江居民】
                     65		政府兜底基金费用	VARCHAR2(16)		【萍乡/鹰潭/九江居民】
                     66		免费门诊基金（余江）	VARCHAR2(16)		【鹰潭居民】
                     67		建档大病补偿本次一段支付金额	VARCHAR2(16)		【鹰潭居民】
                     68		建档大病补偿本次二段支付金额	VARCHAR2(16)		【鹰潭居民】
                     69		建档大病补偿本次三段支付金额	VARCHAR2(16)		【鹰潭居民】
                     70		建档二次补偿本次一段支付金额	VARCHAR2(16)		【鹰潭居民】
                     71		建档二次补偿本次二段支付金额	VARCHAR2(16)		【鹰潭居民】
                     72		建档二次补偿本次三段支付金额	VARCHAR2(16)		【鹰潭居民】
                     73		疾病补充保险本次政策范围内费用一段支付金额	VARCHAR2(16)		【鹰潭居民】
                     74		疾病补充保险本次政策范围内费用二段支付金额	VARCHAR2(16)		【鹰潭居民】
                     75		本年政府兜底基金费用累计(不含本次)	VARCHAR2(16)		【九江居民】
                     76		门慢限额	VARCHAR2(16)		【江西省本级】
                     77		企业军转干基金支付	VARCHAR2(16)		【鹰潭】
                     78		其他基金支付金额	VARCHAR2(16)		异地就医就医地返给定点出参；本地就医返参为0
                     79		伤残人员医疗保障基金	VARCHAR2(16)		异地就医就医地返给定点出参；本地就医返参为0
                  */

                     js.Ylfze = sfjsfh[0];         //医疗费总费用
                     js.Zbxje = sfjsfh[1];         //总报销金额
                     js.Tcjjzf = sfjsfh[2];        //统筹支出
                     js.Dejjzf = sfjsfh[3];        //大病支出
                     js.Zhzf = sfjsfh[4];          //本次帐户支付
                     js.Xjzf = sfjsfh[5];         //个人现金支付
                     js.Gwybzjjzf = sfjsfh[6];     //公务员补助支付金额
                     js.Qybcylbxjjzf = sfjsfh[7];  //企业补充支付金额
                     js.Zffy = sfjsfh[8];          //自费费用
                     js.Dwfdfy = sfjsfh[9];
                     js.Yyfdfy = sfjsfh[10];
                     js.Mzjzfy = sfjsfh[11];       //民政救助费用
                     js.Cxjfy = sfjsfh[12];
                     js.Ylzlfy = sfjsfh[13];
                     js.Blzlfy = sfjsfh[14];
                     js.Fhjjylfy = sfjsfh[15];
                     js.Qfbzfy = sfjsfh[16];
                     js.Zzzyzffy = sfjsfh[17];
                     js.Jrtcfy = sfjsfh[18];
                     js.Tcfdzffy = sfjsfh[19];
                     js.Ctcfdxfy = sfjsfh[20];
                     js.Jrdebxfy = sfjsfh[21];
                     js.Defdzffy = sfjsfh[22];
                     js.Cdefdxfy = sfjsfh[23];
                     js.Rgqgzffy = sfjsfh[24];
                     js.Bcjsqzhye = sfjsfh[25];
                     js.Bntczflj = sfjsfh[26];
                     js.Bndezflj = sfjsfh[27];
                     js.Bnczjmmztczflj = sfjsfh[28];
                     js.Bngwybzzflj = sfjsfh[29];
                     js.Bnzhzflj = sfjsfh[30];
                     js.Bnzycslj = sfjsfh[31];
                     js.Zycs = sfjsfh[32];
                     js.Xm = sfjsfh[33];
                     js.Jsrq = sfjsfh[34];
                     js.Yllb = sfjsfh[35];
                     js.Yldylb = sfjsfh[36];
                     js.Jbjgbm = sfjsfh[37];
                     js.Ywzqh = sfjsfh[38];
                     js.Jslsh = sfjsfh[39];
                     js.Tsxx = sfjsfh[40];
                     js.Djh = sfjsfh[41];
                     js.Jyxl = sfjsfh[42];
                     js.Yyjylsh = sfjsfh[43];
                     js.Yxbz = sfjsfh[44];
                     js.Grbhgl = sfjsfh[45];
                     js.Yljgbm = sfjsfh[46];
                     js.Ecbcje = sfjsfh[47];
                     js.Mmqflj = sfjsfh[48];
                     js.Jsfjylsh = sfjsfh[49];
                     js.Grbh = sfjsfh[50];
                     js.Dbzbc = sfjsfh[51];
                     js.Czzcje = sfjsfh[52];
                     js.Elmmxezc = sfjsfh[53];
                     js.Elmmxesy = sfjsfh[54];
                     js.Jmecbc = sfjsfh[55];
                     js.Tjje = sfjsfh[56];
                     js.Syjjzf = sfjsfh[57];
                     js.Jmdbydje = sfjsfh[58];
                     js.Jmdbedje = sfjsfh[59];
                     js.Jbbcfwnfyzfje = sfjsfh[60];
                     js.Jbbcybbczcfywfyzf = sfjsfh[61];
                     js.Mgwxlcjjzf = sfjsfh[62];
                     js.Jjjmzcfwwkbxfy = sfjsfh[63];
                     js.Zftdfy = sfjsfh[64];
                     js.Mfmzjj = sfjsfh[65];
                     js.Jddbbcbcydzfje = sfjsfh[66];
                     js.Jddbbcbcedzfje = sfjsfh[67];
                     js.Jddbbcbcsdzfje = sfjsfh[68];
                     js.Jdecbcbcydzfje = sfjsfh[69];
                     js.Jdecbcbcedzfje = sfjsfh[70];
                     js.Jdecbcbcsdzfje = sfjsfh[71];
                     js.Jbbcbxbczcfwnfyydzfje = sfjsfh[72];
                     js.Jbbcbxbczcfwnfyedzfje = sfjsfh[73];
                     js.Bnzftdjjfylj = sfjsfh[74];
                     js.Mmxe = sfjsfh[75];
                     js.Jzgbbzzf = sfjsfh[76];
                     js.Qtjjzf = sfjsfh[77];
                     js.Scryylbzjj = sfjsfh[78];
                     if (string.IsNullOrEmpty(js.Scryylbzjj))
                         js.Scryylbzjj = "0.00";
                     js.Lxgbdttcjjzf = "0.00";

                     if (DQJBBZ.Equals("2"))
                         js.Bcjsqzhye = grzhye;

                     //js.Bcjsqzhye = (Convert.ToDecimal(js.Bcjsqzhye) + Convert.ToDecimal(js.Zhzf)).ToString();

                     if (DQJBBZ.Equals("2"))
                         js.Jsrq = jsrq;


                     string[] sDBRY = new string[] { "80", "83", "84", "85", "86", "87" };
                     if (sDBRY.Contains(js.Yldylb))
                     {
                         /*
                         * 建档立卡人员除个人自负部分的费用外，均先由医院垫付
                         */
                         js.Ybxjzf = js.Xjzf;
                         js.Zhzbxje = (Convert.ToDecimal(js.Ylfze) - Convert.ToDecimal(js.Ybxjzf)).ToString();
                     }
                     else
                     {
                         /*
                         * 非建档立卡人员（含民政救助对象）住院（含门诊慢性病治疗）的医院垫付金额为：
                         * 统筹支出+账户支付+医院负担费用+民政救助费用+二次补偿金额+企业军转干基金支付。
                         */
                         js.Zhzbxje = (Convert.ToDecimal(js.Tcjjzf) + Convert.ToDecimal(js.Zhzf) + Convert.ToDecimal(js.Yyfdfy) + Convert.ToDecimal(js.Mzjzfy)
                                      + Convert.ToDecimal(js.Ecbcje)+ Convert.ToDecimal(js.Jzgbbzzf)).ToString();
                         js.Ybxjzf = (Convert.ToDecimal(js.Ylfze) - Convert.ToDecimal(js.Zhzbxje)).ToString();
                     }
                     js.Qtybzf = (Convert.ToDecimal(js.Ylfze) - Convert.ToDecimal(js.Tcjjzf) - Convert.ToDecimal(js.Ybxjzf) - Convert.ToDecimal(js.Zhzf)).ToString(); ;

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
                     string strValue = js.Ylfze + "|" + js.Zhzbxje + "|" + js.Tcjjzf + "|" + js.Dejjzf + "|" + js.Zhzf + "|" +
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

                     WriteLog(sysdate + "  住院收费结算|整合出参|" + strValue);
                     #endregion

                     #region 数据操作


                     strSql = string.Format(@"insert into ybfyjsdr(jzlsh,ybjzlsh,jylsh,djhin,ylfze,zbxje,tcjjzf,dejjzf,zhzf,xjzf,
                                        gwybzjjzf,qybcylbxjjzf,zffy,dwfdfy,yyfdfy,mzjzfy,cxjfy,ylzlfy,blzlfy,fhjbylfy,
                                        qfbzfy,zzzyzffy,jrtcfy,tcfdzffy,ctcfdxfy,jrdebxfy,defdzffy,cdefdxfy,rgqgzffy,bcjsqzhye,
                                        bntczflj,bndezflj,bnczjmmztczflj,bngwybzzflj,bnzhzflj,bnzycslj,zycs,xm,kh,jsrq,
                                        yllb,yldylb,jbjgbm,ywzqh,jslsh,tsxx,djh,yyjylsh,grbhgl,yljgbm,
                                        ecbcje,mmqflj,jsfjylsh,grbh,czzcje,elmmxezc,elmmxesy,jmecbc,tjje,syjjzf,
                                        jjjmdbydje,jjjmdbedje,jjjmjbbcfwnje,jjjmjbbcfwwje,mgwxlcjjzf,jjjmzcfwwkbxfy,zftdjjzf,mfmzjj,jddbbcbcydzfje,jddbbcbcedzfje,
                                        jddbbcbcsdzfje,jdecbcbcydzfje,jdecbcbcedzfje,jdecbcbcsdzfje,jbbcbxbczcfwnfyydzfje,jbbcbxbczcfwnfyedzfje,bnzftdjjfylj,lxgbddtczf,qtjjzf,scryylbzjj,
                                        zhxjzffy,qtybfy,sysdate,zhzbxje,jbr,ztjsbz,ryrq,cyrq,cyyy,jzgbjjzf,dbzbc)
                                        values('{0}','{1}','{2}','{3}','{4}','{5}' ,'{6}','{7}','{8}' ,'{9}' ,
                                        '{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}',
                                        '{20}','{21}','{22}','{23}','{24}','{25}','{26}','{27}','{28}','{29}',
                                        '{30}','{31}','{32}','{33}','{34}','{35}','{36}','{37}','{38}','{39}',
                                        '{40}','{41}','{42}','{43}','{44}','{45}','{46}','{47}','{48}','{49}',
                                        '{50}','{51}','{52}','{53}','{54}','{55}','{56}','{57}','{58}','{59}',
                                        '{60}','{61}','{62}','{63}','{64}','{65}','{66}','{67}','{68}','{69}',
                                        '{70}','{71}','{72}','{73}','{74}','{75}','{76}','{77}','{78}','{79}',
                                        '{80}','{81}','{82}','{83}','{84}','{85}','{86}','{87}','{88}','{89}',
                                        '{90}')",
                                        jzlsh, ybjzlsh, JYLSH, djh, js.Ylfze, js.Zbxje, js.Tcjjzf, js.Dejjzf, js.Zhzf, js.Xjzf,
                                        js.Gwybzjjzf, js.Qybcylbxjjzf, js.Zffy, js.Dwfdfy, js.Yyfdfy, js.Mzjzfy, js.Cxjfy, js.Ylzlfy, js.Blzlfy, js.Fhjjylfy,
                                        js.Qfbzfy, js.Zzzyzffy, js.Jrtcfy, js.Tcfdzffy, js.Ctcfdxfy, js.Jrdebxfy, js.Defdzffy, js.Cdefdxfy, js.Rgqgzffy, js.Bcjsqzhye,
                                        js.Bntczflj, js.Bndezflj, js.Bnczjmmztczflj, js.Bngwybzzflj, js.Bnzhzflj, js.Bnzycslj, js.Zycs, js.Xm, js.Kh, js.Jsrq,
                                        js.Yllb, js.Yldylb, js.Jbjgbm, js.Ywzqh, js.Jslsh, js.Tsxx, js.Djh, js.Yyjylsh, js.Grbhgl, js.Yljgbm,
                                        js.Ecbcje, js.Mmqflj, js.Jsfjylsh, js.Grbh, js.Czzcje, js.Elmmxezc, js.Elmmxesy, js.Jmecbc, js.Tjje, js.Syjjzf,
                                        js.Jmdbydje, js.Jmdbedje, js.Jbbcfwnfyzfje, js.Jbbcybbczcfywfyzf, js.Mgwxlcjjzf, js.Jjjmzcfwwkbxfy, js.Zftdfy, js.Mfmzjj, js.Jddbbcbcydzfje, js.Jddbbcbcedzfje,
                                        js.Jddbbcbcsdzfje, js.Jdecbcbcydzfje, js.Jdecbcbcedzfje, js.Jdecbcbcsdzfje, js.Jbbcbxbczcfwnfyydzfje, js.Jbbcbxbczcfwnfyedzfje, js.Bnzftdjjfylj, js.Lxgbdttcjjzf, js.Qtjjzf, js.Scryylbzjj,
                                        js.Ybxjzf, js.Qtybzf, sysdate, js.Zhzbxje, jbr, ztjsbz, ryrq, cyrq, cyyy, js.Jzgbbzzf,
                                        js.Dbzbc);
                     liSQL.Add(strSql);
                     strSql = string.Format("update ybcfmxscfhdr set ybdjh='{0}' where isnull(ybdjh,'')='' and jzlsh='{1}' and cxbz=1", djh, jzlsh);
                     liSQL.Add(strSql);
                     strSql = string.Format("update ybcfmxscindr set ybdjh='{0}' where isnull(ybdjh,'')='' and jzlsh='{1}' and cxbz=1", djh, jzlsh);
                     liSQL.Add(strSql);

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
                         object[] objJSCX = { jzlsh, js.Djh, ybjzlsh, xm, kh, grbh, dqrq, js.Jsrq, js.Jslsh, ybjzlsh_snyd, DQJBBZ };
                         NYBZYFYJSCX(objJSCX);
                         return new object[] { 0, 0, "住院收费结算(市本级)成功|本地数据操作失败|" + obj[2].ToString() };
                     }
                     #endregion
                 }
                 catch (Exception error)
                 {
                     WriteLog(sysdate + "  住院收费结算(市本级)|异常|" + error.Message);
                     return new object[] { 0, 2, "Error:" + error.Message };
                 }
             }
             else
             {
                 return new object[] { 0, 0, "医保读卡失败!" };
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
                YWZQH = CliUtils.fLoginYbNo.Split('|')[0].ToString();    //业务周期号
            }
            catch
            {
                return new object[] { 0, 0, "医保未连接或初始化失败" };
            }
            try
            {
                CZYBH = CliUtils.fLoginUser;   // 操作员工号 
                YWZQH = CliUtils.fLoginYbNo.Split('|')[0].ToString();   // 业务周期号
                ZXBM = "0000";
                string jbr = CliUtils.fUserName;  //经办人
                string jzlsh = objParam[0].ToString();   // 就诊流水号
                string djh = objParam[1].ToString();     // 结算单据号
                string fphx = "";
                JYLSH = DateTime.Now.ToString("yyyyMMddHHmmss") + "-" + YLGHBH + "-" + new Random().Next(100).ToString().PadLeft(4, '0');

                //获取医保结算信息
                string strSql = string.Format(@"select a.jslsh,a.cfmxjylsh,c.z3fphx,a.jsrq,a.djh,b.* from ybfyjsdr a 
                                                left join ybmzzydjdr b on a.jzlsh = b.jzlsh and a.cxbz=b.cxbz
                                                left join zy03dw c on a.jzlsh=c.z3zyno and a.djhin=c.z3jshx and c.z3endv like '1%'
                                                where a.jzlsh = '{0}'  
                                                and a.djhin = '{1}' and a.cxbz = 1  ", jzlsh, djh);
                DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                if (ds.Tables[0].Rows.Count == 0)
                    return new object[] { 0, 0, "该患者无医保结算信息" };

                DataRow dr = ds.Tables[0].Rows[0];
                string ybjzlsh = dr["ybjzlsh"].ToString();    //"1100008927"
                string xm = dr["xm"].ToString();          // // "徐国强""施美荣";
                string kh = dr["kh"].ToString();          // ;"520313262"//"469285189";
                string grbh = dr["grbh"].ToString();      //"360402837288" ;//"360403700513";
                string jsrq = dr["jsrq"].ToString();      //"20160308180140" ;//"20160129000000";
                string ybjzlsh_snyd = dr["ybjzlsh_snyd"].ToString();
                string dqbh = dr["tcqh"].ToString();
                string ybdjh = dr["djh"].ToString();
                DQJBBZ = dr["dqjbbz"].ToString();    //地区级别标志
                fphx = dr["z3fphx"].ToString();

                #region 入参
                /*
                 1		就诊流水号	VARCHAR2(20)	NOT NULL	同登记时的就诊流水号
                    2		单据号	VARCHAR2(20)	NOT NULL	
                    3		结算日期	VARCHAR2(14)		
                    4		经办人	VARCHAR2(20)	NOT NULL	医疗机构操作员姓名
                    5		是否保存处方标志	VARCHAR2(3)		二级代码
                    6		开发商标志	VARCHAR2(20)		开发商标志： 01     东软 02     中软 03     其他
                    7	个人编号	VARCHAR2(10)	NOT NULL	
                    8	姓名	VARCHAR2(20)	NOT NULL	
                    9	卡号	VARCHAR2(18)	NOT NULL	
                 */
                StringBuilder inputParam = new StringBuilder();
                if (DQJBBZ.Equals("1"))
                    inputParam.Append(ybjzlsh + "|");
                else
                {
                    ZXBM = dqbh;
                    inputParam.Append(ybjzlsh_snyd + "|");
                }
                inputParam.Append(ybdjh + "|");
                inputParam.Append(jsrq + "|");
                inputParam.Append(jbr + "|");
                inputParam.Append("0|");
                inputParam.Append("03|");
                inputParam.Append(grbh + "|");
                inputParam.Append(xm + "|");
                inputParam.Append(kh + "|");

                YWBH = "2430";
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
                    strSql = string.Format(@"insert into ybfyjsdr(jzlsh,ybjzlsh,jylsh,djhin,ylfze,zbxje,tcjjzf,dejjzf,zhzf,xjzf,
                                        gwybzjjzf,qybcylbxjjzf,zffy,dwfdfy,yyfdfy,mzjzfy,cxjfy,ylzlfy,blzlfy,fhjbylfy,
                                        qfbzfy,zzzyzffy,jrtcfy,tcfdzffy,ctcfdxfy,jrdebxfy,defdzffy,cdefdxfy,rgqgzffy,bcjsqzhye,
                                        bntczflj,bndezflj,bnczjmmztczflj,bngwybzzflj,bnzhzflj,bnzycslj,zycs,xm,kh,jsrq,
                                        yllb,yldylb,jbjgbm,ywzqh,jslsh,tsxx,djh,yyjylsh,grbhgl,yljgbm,
                                        ecbcje,mmqflj,jsfjylsh,grbh,czzcje,elmmxezc,elmmxesy,jmecbc,tjje,syjjzf,
                                        jjjmdbydje,jjjmdbedje,jjjmjbbcfwnje,jjjmjbbcfwwje,mgwxlcjjzf,jjjmzcfwwkbxfy,zftdjjzf,mfmzjj,jddbbcbcydzfje,jddbbcbcedzfje,
                                        jddbbcbcsdzfje,jdecbcbcydzfje,jdecbcbcedzfje,jdecbcbcsdzfje,jbbcbxbczcfwnfyydzfje,jbbcbxbczcfwnfyedzfje,bnzftdjjfylj,lxgbddtczf,qtjjzf,scryylbzjj,
                                        zhxjzffy,qtybfy,zhzbxje,jbr,ztjsbz,ryrq,cyrq,cyyy,jzgbjjzf,dbzbc,cxbz,sysdate) 
                                        select jzlsh,ybjzlsh,jylsh,djhin,ylfze,zbxje,tcjjzf,dejjzf,zhzf,xjzf,
                                        gwybzjjzf,qybcylbxjjzf,zffy,dwfdfy,yyfdfy,mzjzfy,cxjfy,ylzlfy,blzlfy,fhjbylfy,
                                        qfbzfy,zzzyzffy,jrtcfy,tcfdzffy,ctcfdxfy,jrdebxfy,defdzffy,cdefdxfy,rgqgzffy,bcjsqzhye,
                                        bntczflj,bndezflj,bnczjmmztczflj,bngwybzzflj,bnzhzflj,bnzycslj,zycs,xm,kh,jsrq,
                                        yllb,yldylb,jbjgbm,ywzqh,jslsh,tsxx,djh,yyjylsh,grbhgl,yljgbm,
                                        ecbcje,mmqflj,jsfjylsh,grbh,czzcje,elmmxezc,elmmxesy,jmecbc,tjje,syjjzf,
                                        jjjmdbydje,jjjmdbedje,jjjmjbbcfwnje,jjjmjbbcfwwje,mgwxlcjjzf,jjjmzcfwwkbxfy,zftdjjzf,mfmzjj,jddbbcbcydzfje,jddbbcbcedzfje,
                                        jddbbcbcsdzfje,jdecbcbcydzfje,jdecbcbcedzfje,jdecbcbcsdzfje,jbbcbxbczcfwnfyydzfje,jbbcbxbczcfwnfyedzfje,bnzftdjjfylj,lxgbddtczf,qtjjzf,scryylbzjj,
                                        zhxjzffy,qtybfy,zhzbxje,jbr,ztjsbz,ryrq,cyrq,cyyy,jzgbjjzf,dbzbc,0,sysdate
                                        from ybfyjsdr where jzlsh = '{0}' and djhin = '{1}' and cxbz = 1", jzlsh, djh, sysdate);
                    liSql.Add(strSql);

                    strSql = string.Format(@"update ybfyjsdr set cxbz = 2 where jzlsh = '{0}' and djhin = '{1}' and cxbz = 1", jzlsh, djh);
                    liSql.Add(strSql);

                    //撤销费用明细
                    strSql = string.Format(@"delete from ybcfmxscfhdr where jzlsh = '{0}' and ybdjh='{1}' and cxbz = 1", jzlsh, djh, sysdate);
                    liSql.Add(strSql);
                    strSql = string.Format(@"delete from ybcfmxscindr where jzlsh = '{0}' and ybdjh='{1}' and cxbz = 1", jzlsh, djh, sysdate);
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

        #region 住院收费结算信息查询
        public static object[] YBZYJSXXCX(object[] objParam)
        {
            string jzlsh = objParam[0].ToString();
            try
            {
                Frm_ybjsmx jsmx = new Frm_ybjsmx(jzlsh);
                jsmx.ShowDialog();
                return new object[] { 0, 1, "查询成功" };
            }
            catch
            {
                return new object[] { 0, 1, "查询失败" };
            }
        }
        #endregion

        #region 住院收费结算单打印
        public static object[] YBZYJSD(object[] objParam)
        {
            WriteLog("结算单不启用.....");
            return new object[] { 0, 1, "结算单不启用" };
        }
        #endregion

        #region 门诊登记撤销_内部
        public static object[] NYBMZDJCX(object[] objParam)
        {
            string sysdate = GetServerDateTime();
            WriteLog(sysdate + "  进入门诊登记撤销(内部)...");

            CZYBH = CliUtils.fLoginUser;    //操作员工号
            YWZQH = CliUtils.fLoginYbNo.Split('|')[0].ToString();    //业务周期号
            ZXBM = "0000";

            string ybjzlsh = objParam[0].ToString();
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
                /*
                1		就诊流水号	VARCHAR2(20)	NOT NULL	唯一
                2		经办人	VARCHAR2(20)	NOT NULL	医疗机构操作员姓名
                3	个人编号	VARCHAR2(10)	NOT NULL	异地结算
                4	姓名	VARCHAR2(20)	NOT NULL	异地结算
                5	卡号	VARCHAR2(18)	NOT NULL	异地结算
                 */
                StringBuilder inputParam = new StringBuilder();

                if (DQJBBZ.Equals("2"))
                {
                    ZXBM = dqbh;
                    inputParam.Append(ybjzlsh_snyd + "|");
                }
                else
                    inputParam.Append(ybjzlsh + "|");
                inputParam.Append(jbr + "|");
                inputParam.Append(grbh + "|");
                inputParam.Append(xm + "|");
                inputParam.Append(kh + "|");


                StringBuilder inputData = new StringBuilder();
                YWBH = "2240";
                inputData.Append(YWBH + "^");
                inputData.Append(YLGHBH + "^");
                inputData.Append(CZYBH + "^");
                inputData.Append(YWZQH + "^");
                inputData.Append(JYLSH + "^");
                inputData.Append(ZXBM + "^");
                inputData.Append(inputParam.ToString() + "^");
                inputData.Append(LJBZ + "^");
                WriteLog(sysdate + "  门诊登记撤销(内部)|入参|" + inputData.ToString());

                StringBuilder outputData = new StringBuilder(1024);
                int i = BUSINESS_HANDLE(inputData, outputData);

                if (i == 0)
                {
                    WriteLog(sysdate + "  门诊登记撤销(内部)成功|出参|" + outputData.ToString());
                    return new object[] { 0, 1, outputData.ToString() };
                }
                else
                {
                    WriteLog(sysdate + "  门诊登记撤销(内部)失败|出参|" + outputData.ToString());
                    return new object[] { 0, 0, outputData.ToString() };
                }
            }
            catch (Exception error)
            {
                WriteLog(sysdate + "  门诊登记撤销(内部)|接口异常|" + error.Message);
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
                ZXBM = "0000";
                string jbr = CliUtils.fUserName;
                string jzlsh = objParam[0].ToString();  //就诊流水号
                string cxjylsh = objParam[1].ToString();  //撤销交易流水号(全部撤销则传空字符串)
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
                //交易流水号
                JYLSH = DateTime.Now.ToString("yyyyMMddHHmmss") + "-" + YLGHBH + "-" + new Random().Next(100).ToString().PadLeft(4, '0');
                ZXBM = "0000";
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
                /*
                1		就诊流水号	VARCHAR2(20)	NOT NULL	同登记时的就诊流水号
                2		单据号	VARCHAR2(20)	NOT NULL	
                3		结算日期	VARCHAR2(14)		
                4		经办人	VARCHAR2(20)	NOT NULL	医疗机构操作员姓名
                5		是否保存处方标志	VARCHAR2(3)		二级代码
                6		开发商标志	VARCHAR2(20)		开发商标志：
                01     东软
                02     中软
                03     其他
                7	个人编号	VARCHAR2(10)	NOT NULL	
                8	姓名	VARCHAR2(20)	NOT NULL	
                9	卡号	VARCHAR2(18)	NOT NULL	
                    */
                YWBH = "2430";
                if (DQJBBZ.Equals("2"))
                {
                    ZXBM = dqbh;
                    inputParam.Append(ybjzlsh_snyd + "|");
                }
                else
                    inputParam.Append(ybjzlsh + "|");
                inputParam.Append(djh + "|");
                inputParam.Append(jsrq + "|");
                inputParam.Append(jbr + "|");
                inputParam.Append("0" + "|");
                inputParam.Append("03" + "|");
                inputParam.Append(grbh + "|");
                inputParam.Append(xm + "|");
                inputParam.Append(kh + "|");


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
                    string strSql = string.Format(@"delete from ybcfmxscfhdr
                                                    where ybjzlsh='{0}' and jylsh='{1}' and cxbz=1", ybjzlsh, cfmxjylsh, sysdate);
                    liSql.Add(strSql);
                  
                    strSql = string.Format(@"delete from ybcfmxscindr
                                            where ybjzlsh='{0}' and jylsh='{1}' and cxbz=1", ybjzlsh, cfmxjylsh, sysdate);
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
                CZYBH = CliUtils.fLoginUser;  // 操作员工号
                YWZQH = CliUtils.fLoginYbNo.Split('|')[0].ToString();  // 业务周期号
                ZXBM = "0000";
                string jbr = CliUtils.fLoginUser;

                string jzlsh = objParam[0].ToString();      // 就诊流水号
                string ybjzlsh = objParam[1].ToString();    // 医保就诊流水号
                string yllb = objParam[2].ToString();
                string grbh = objParam[3].ToString();       // 个人编号
                string xm = objParam[4].ToString();         // 姓名
                string kh = objParam[5].ToString();         // 卡号
                string dqbh = objParam[6].ToString();       // 地区编号
                string ybjzlsh_snyd = objParam[7].ToString();
                DQJBBZ = objParam[8].ToString();

                //交易流水号
                JYLSH = DateTime.Now.ToString("yyyyMMddHHmmss") + "-" + YLGHBH + "-" + new Random().Next(100).ToString().PadLeft(4, '0');

                /*
               1		就诊流水号	VARCHAR2(20)	NOT NULL	唯一
               2		经办人	VARCHAR2(20)	NOT NULL	医疗机构操作员姓名
               3	个人编号	VARCHAR2(10)	NOT NULL	异地结算
               4	姓名	VARCHAR2(20)	NOT NULL	异地结算
               5	卡号	VARCHAR2(18)	NOT NULL	异地结算
                */
                StringBuilder inputParam = new StringBuilder();
                if (DQJBBZ.Equals("1"))
                {
                    inputParam.Append(ybjzlsh + "|");
                    
                }
                else
                {
                    ZXBM = dqbh;
                    inputParam.Append(ybjzlsh_snyd + "|");
                }
                inputParam.Append(jbr + "|");
                inputParam.Append(grbh + "|");
                inputParam.Append(xm + "|");
                inputParam.Append(kh + "|");

                YWBH = "2240";
                StringBuilder inputData = new StringBuilder();
                inputData.Append(YWBH + "^");
                inputData.Append(YLGHBH + "^");
                inputData.Append(CZYBH + "^");
                inputData.Append(YWZQH + "^");
                inputData.Append(JYLSH + "^");
                inputData.Append(ZXBM + "^");
                inputData.Append(inputParam.ToString() + "^");
                inputData.Append(LJBZ + "^");

                WriteLog(sysdate + "  住院登记撤销(内部)|入参|" + inputData.ToString());
                StringBuilder outputData = new StringBuilder(102400);
                int i = BUSINESS_HANDLE(inputData, outputData);
                if (i == 0)
                {
                    WriteLog(sysdate + "  住院登记撤销(内部)成功|" + outputData.ToString());
                    return new object[] { 0, 1, outputData.ToString() };
                }
                else
                {
                    WriteLog(sysdate + "  住院登记撤销(内部)失败|" + outputData.ToString());
                    return new object[] { 0, 0, outputData.ToString() };
                }
            }
            catch (Exception error)
            {
                WriteLog(sysdate + "  住院登记撤销(内部)|系统异常|" + error.Message);
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
                CZYBH = CliUtils.fLoginUser; //操作员工号
                YWZQH = CliUtils.fLoginYbNo.Split('|')[0].ToString();    //业务周期号
                ZXBM = "0000";
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
                    ZXBM = "0000";
                    inputParam.Append(ybjzlsh + "|");
                }
                else
                {
                    ZXBM = dqbh;
                    inputParam.Append(ybjzlsh_sndy + "|");
                }
                inputParam.Append(cfmxjylsh + "|");
                inputParam.Append(jbr + "|");
                inputParam.Append(grbh + "|");
                inputParam.Append(xm + "|");
                inputParam.Append(kh + "|");

                YWBH = "2320";
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
                ZXBM = "0000";
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
                /*
                    1		就诊流水号	VARCHAR2(20)	NOT NULL	同登记时的就诊流水号
                    2		单据号	VARCHAR2(20)	NOT NULL	
                    3		结算日期	VARCHAR2(14)		
                    4		经办人	VARCHAR2(20)	NOT NULL	医疗机构操作员姓名
                    5		是否保存处方标志	VARCHAR2(3)		二级代码
                    6		开发商标志	VARCHAR2(20)		开发商标志：
                    01     东软
                    02     中软
                    03     其他
                    7	个人编号	VARCHAR2(10)	NOT NULL	
                    8	姓名	VARCHAR2(20)	NOT NULL	
                    9	卡号	VARCHAR2(18)	NOT NULL	

                    */
                StringBuilder inputParam = new StringBuilder();
                if (DQJBBZ.Equals("1"))
                    inputParam.Append(ybjzlsh + "|");
                else
                {
                    ZXBM = dqbh;
                    inputParam.Append(ybjzlsh_snyd + "|");
                }
                inputParam.Append(djh + "|");
                inputParam.Append(jsrq + "|");
                inputParam.Append(jbr + "|");
                inputParam.Append("0|");
                inputParam.Append("03|");
                inputParam.Append(grbh + "|");
                inputParam.Append(xm + "|");
                inputParam.Append(kh + "|");


                YWBH = "2430";
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
                    WriteLog(sysdate + "  住院收费结算撤销(内部)成功|出参|" + inputData.ToString());
                    return new object[] { 0, 1, "住院收费结算撤销(内部)成功" };
                    
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
        #endregion

        #region 数据上传类
        #region 医保对照上传
        public static object[] YBDZXXPLSC(object[] objParam)
        {
            string sysdate = GetServerDateTime();
            WriteLog(sysdate + "  进入医保对照上传...");
            try
            {
                CZYBH = CliUtils.fLoginUser; //操作员工号
                YWZQH = CliUtils.fLoginYbNo.Split('|')[0].ToString();    //业务周期号
                ZXBM = "0000";
                JYLSH = DateTime.Now.ToString("yyyyMMddHHmmss") + "-" + YLGHBH + "-" + new Random().Next(100).ToString().PadLeft(4, '0');
                string hisxmbh = objParam[0].ToString();
                

                #region 对照数据
                StringBuilder inParam = new StringBuilder();
                string sDate = DateTime.Now.ToString("yyyyMMddHHmmss");
                string strSql = string.Format(@"select sfxmzldm,ybxmbh,hisxmbh,hisxmmc,czyxm,gg,left(gg,14) as gg,dj,jxdm from ybhisdzdr where scbz=0 ");
                if (!string.IsNullOrEmpty(hisxmbh))
                    strSql += string.Format(@"and hisxmbh='{0}'", hisxmbh);
                DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                if (ds.Tables[0].Rows.Count == 0)
                    return new object[] { 0, 1, "没有需要上传的数据 " };
                List<string> liSql = new List<string>();
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    /*
                     * 1|X-J01CA-A040-E001|101010021085|阿莫西林胶囊Z*|472||||管理员|20181001103556|20181001103556|||||$
                     * 1		收费项目种类	VARCHAR2(3)	NOT NULL	二级代码
                        2		三大目录中心编码	VARCHAR2(20)	NOT NULL	项目中心编码
                        3		医院项目内码	VARCHAR2(20)	NOT NULL	
                        4		医院项目名称	VARCHAR2(50)	NOT NULL	
                        5		定点医疗机构药品剂型	VARCHAR2(20)		二级代码
                        6		规格	VARCHAR2(14)		
                        7		医院端价格	VARCHAR2(12)		4位小数
                        8		医院端产地	VARCHAR2(50)		
                        9		对照经办人	VARCHAR2(20)		
                        10		对照操作时间	VARCHAR2(14)		YYYYMMDDHH24MISS
                        11		开始时间	VARCHAR2(14)	NOT NULL	YYYYMMDDHH24MISS
                        12		终止时间	VARCHAR2(14)		YYYYMMDDHH24MISS
                        13		药品禁忌	VARCHAR2(500)		
                        14		不良反应	VARCHAR2(500)		
                        15		注意事项	VARCHAR2(500)		
                     */
                    inParam.Append(dr["sfxmzldm"].ToString() + "|");
                    inParam.Append(dr["ybxmbh"].ToString() + "|");
                    inParam.Append(dr["hisxmbh"].ToString() + "|");
                    inParam.Append(dr["hisxmmc"].ToString() + "|");
                    inParam.Append(dr["jxdm"].ToString()+"|");
                    inParam.Append(dr["gg"].ToString()+"|");
                    inParam.Append(dr["dj"].ToString() + "|");
                    inParam.Append("|");
                    inParam.Append(dr["czyxm"].ToString() + "|");
                    inParam.Append(sDate + "|");
                    inParam.Append(sDate + "|");
                    inParam.Append("|");
                    inParam.Append("|");
                    inParam.Append("|");
                    inParam.Append("|$");

                    liSql.Add(inParam.ToString());
                    inParam.Remove(0, inParam.Length);
                }
                #endregion

                //入参
                YWBH = "3300";  //业务编号
                int iTemp = 0;
                StringBuilder inputData = new StringBuilder();
                StringBuilder outputData = new StringBuilder(102400);
                foreach (string inputParam in liSql)
                {
                    if (iTemp <= 10)
                    {
                        inputData.Append(inputParam.ToString());
                        iTemp++;
                    }
                    else
                    {
                        YWBH = "3300";  //业务编号
                        StringBuilder inputData1 = new StringBuilder();
                        inputData1.Append(YWBH + "^");
                        inputData1.Append(YLGHBH + "^");
                        inputData1.Append(CZYBH + "^");
                        inputData1.Append(YWZQH + "^");
                        inputData1.Append(JYLSH + "^");
                        inputData1.Append(ZXBM + "^");
                        inputData1.Append(inputData.ToString().TrimEnd('$') + "^");
                        inputData1.Append(LJBZ + "^");
                        WriteLog(sysdate + "  入参|" + inputData1.ToString());
                        int i = BUSINESS_HANDLE(inputData1, outputData);
                        if (i == 0)
                            WriteLog(sysdate + "  医保对照上传成功|出参|" + outputData.ToString());
                        else
                        {
                            WriteLog(sysdate + "  医保对照上传失败|" + outputData.ToString());
                            return new object[] { 0, 0, "医保对照上传失败|" + outputData.ToString() };
                        }

                        iTemp = 1;
                        inputData.Remove(0, inputData.Length);
                        inputData.Append(inputParam.ToString());
                    }
                }


                #region 不足50条时，一次性上传
                if (iTemp > 0)
                {
                    StringBuilder inputData1 = new StringBuilder();

                    YWBH = "3300";  //业务编号
                    inputData1.Append(YWBH + "^");
                    inputData1.Append(YLGHBH + "^");
                    inputData1.Append(CZYBH + "^");
                    inputData1.Append(YWZQH + "^");
                    inputData1.Append(JYLSH + "^");
                    inputData1.Append(ZXBM + "^");
                    inputData1.Append(inputData.ToString().TrimEnd('$') + "^");
                    inputData1.Append(LJBZ + "^");

                    WriteLog(sysdate + "  入参|" + inputData.ToString());
                    int i = BUSINESS_HANDLE(inputData1, outputData);
                    if (i == 0)
                        WriteLog(sysdate + "  医保对照上传成功|出参|" + outputData.ToString());
                    else
                    {
                        WriteLog(sysdate + "  医保对照上传失败|" + outputData.ToString());
                        return new object[] { 0, 0, "医保对照上传失败|" + outputData.ToString() };
                    }
                }
                #endregion

                strSql = string.Format(@"update ybhisdzdr set scbz=1 where scbz=0");
                object[] obj = { strSql };
                obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);

                return new object[] { 0, 1, "对照上传成功" };
             
            }
            catch (Exception ex)
            {
                WriteLog(sysdate + "  医保对照上传异常|" + ex.Message);
                return new object[] { 0, 0, "医保对照上传异常|" + ex.Message };
            }
        }
        #endregion

        #region 医保科室信息上传
        public static object[] YBKSXXSC(object[] objParam)
        {
            string sysdate = GetServerDateTime();
            WriteLog(sysdate + "  进入科室信息上传...");
            try
            {
                CZYBH = CliUtils.fLoginUser; //操作员工号
                YWZQH = CliUtils.fLoginYbNo.Split('|')[0].ToString();    //业务周期号
                ZXBM = "0000";
                JYLSH = DateTime.Now.ToString("yyyyMMddHHmmss") + "-" + YLGHBH + "-" + new Random().Next(100).ToString().PadLeft(4, '0');
                string ksbm = objParam[0].ToString();

                #region 对照数据
                StringBuilder inParam = new StringBuilder();
                string sDate = DateTime.Now.ToString("yyyyMMddHHmmss");
                string strSql = string.Format(@"select b2ejks,b2ejnm from bz02d where b2mark LIKE 'Z%' ");
                if (!string.IsNullOrEmpty(ksbm))
                    strSql += string.Format(@" and b2ejks='{0}'", ksbm);

                DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                if (ds.Tables[0].Rows.Count == 0)
                    return new object[] { 0, 1, "HIS中没有查询到对应的科室信息" };
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    /*
                     * Z018-01|内二科|||||||
                     * 1		科室编号	VARCHAR2(20)	NOT NULL	
                        2		科室名称	VARCHAR2(50)	NOT NULL	
                        3		在编职工数	NUMBER(16)		
                        4		床位数	NUMBER(16)		
                        5		负责人	VARCHAR2(50)		
                        6		负责人联系电话	VARCHAR2(50)		
                        7		经办人	VARCHAR2(50)		
                        8		经办时间	VARCHAR2(14)		YYYYMMDDHH24MISS

                     */
                    inParam.Append(dr["b2ejks"].ToString() + "|");
                    inParam.Append(dr["b2ejnm"].ToString() + "|");
                    inParam.Append("|");
                    inParam.Append("|");
                    inParam.Append("|");
                    inParam.Append("|");
                    inParam.Append("|");
                    inParam.Append("|$");
                }
                #endregion

                //入参
                YWBH = "3500";  //业务编号
                StringBuilder inputData = new StringBuilder();
                inputData.Append(YWBH + "^");
                inputData.Append(YLGHBH + "^");
                inputData.Append(CZYBH + "^");
                inputData.Append(YWZQH + "^");
                inputData.Append(JYLSH + "^");
                inputData.Append(ZXBM + "^");
                inputData.Append(inParam.ToString().TrimEnd('$') + "^");
                inputData.Append(LJBZ + "^");

                StringBuilder outputData = new StringBuilder(10240);
                WriteLog(sysdate + "  入参|" + inputData.ToString());
                int i = BUSINESS_HANDLE(inputData, outputData);
                if (i == 0)
                {
                    WriteLog(sysdate + "  科室信息上传成功|出参|" + outputData.ToString());
                    return new object[] { 0, 1, "科室信息上传成功|" + outputData.ToString() };
                }
                else
                {
                    WriteLog(sysdate + "  科室信息上传失败|" + outputData.ToString());
                    return new object[] { 0, 0, "科室信息上传失败|" + outputData.ToString() };
                }
            }
            catch (Exception ex)
            {
                WriteLog(sysdate + "  科室信息上传异常|" + ex.Message);
                return new object[] { 0, 0, "科室信息上传异常|" + ex.Message };
            }
        }
        #endregion

        #region 医保医师信息上传
        public static object[] YBYSXXSC(object[] objParam)
        {
            string sysdate = GetServerDateTime();
            WriteLog(sysdate + "  进入医师信息上传...");
            try
            {
                CZYBH = CliUtils.fLoginUser; //操作员工号
                YWZQH = CliUtils.fLoginYbNo.Split('|')[0].ToString();    //业务周期号
                ZXBM = "0000";
                JYLSH = DateTime.Now.ToString("yyyyMMddHHmmss") + "-" + YLGHBH + "-" + new Random().Next(100).ToString().PadLeft(4, '0');
                string ysbm = objParam[0].ToString();

                #region 对照数据
                StringBuilder inParam = new StringBuilder();
                string strSql = string.Format(@"select b1empn,b1name,b1ksno from bz01h where b1type=1 ");
                if (!string.IsNullOrEmpty(ysbm))
                    strSql += string.Format(@"and b1empn='{0}' ", ysbm);

                DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                if (ds.Tables[0].Rows.Count == 0)
                    return new object[] { 0, 1, "HIS中没有查询到对应的科室信息" };
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    /*
                     * 
                     * 
                     * 1		医师编号	VARCHAR2(20)	NOT NULL	
                        2		医师姓名	VARCHAR2(50)	NOT NULL	
                        3		医保处方权	VARCHAR2(3)	NOT NULL	二级代码
                        4		医师级别	VARCHAR2(3)		二级代码
                        5		科室编号	VARCHAR2(20)	NOT NULL	必须为医院已上报的科室编号。
                        6		科室名称	VARCHAR2(50)		
                        7		医师执业证号	VARCHAR2(50)		
                        8		职称	VARCHAR2(3)		二级代码
                        9		行政职务	VARCHAR2(3)		二级代码
                        10		学术职务	VARCHAR2(20)		
                        11		毕业院校	VARCHAR2(50)		
                        12		性别	VARCHAR2(3)		二级代码
                        13		联系电话	VARCHAR2(50)		
                        14		医师身份证号码	VARCHAR2(18)		
                        15		主治疾病内容	VARCHAR2(500)		
                        16		疾病种类	VARCHAR2(50)		
                        17		医院人员类别	VARCHAR2(3)		二级代码
                        18		经办人	VARCHAR2(50)		
                        19		经办时间	VARCHAR2(14)		YYYYMMDDHH24MISS

                     */
                    inParam.Append(dr["b1empn"].ToString() + "|");
                    inParam.Append(dr["b1name"].ToString() + "|");
                    inParam.Append("1|");
                    inParam.Append("|");
                    inParam.Append(dr["b1ksno"].ToString() + "|");
                    inParam.Append("|");
                    inParam.Append("|");
                    inParam.Append("|");
                    inParam.Append("|");
                    inParam.Append("|");
                    inParam.Append("|");
                    inParam.Append("|");
                    inParam.Append("|");
                    inParam.Append("|");
                    inParam.Append("|");
                    inParam.Append("|");
                    inParam.Append("|");
                    inParam.Append("|");
                    inParam.Append("|$");
                }
                #endregion

                //入参

                YWBH = "3400";  //业务编号
                StringBuilder inputData = new StringBuilder();
                inputData.Append(YWBH + "^");
                inputData.Append(YLGHBH + "^");
                inputData.Append(CZYBH + "^");
                inputData.Append(YWZQH + "^");
                inputData.Append(JYLSH + "^");
                inputData.Append(ZXBM + "^");
                inputData.Append(inParam.ToString().TrimEnd('$') + "^");
                inputData.Append(LJBZ + "^");
                StringBuilder outputData = new StringBuilder(10240);
                WriteLog(sysdate + "  入参|" + inputData.ToString());
                int i = BUSINESS_HANDLE(inputData, outputData);
                if (i == 0)
                {
                    WriteLog(sysdate + "  医师信息上传成功|出参|" + outputData.ToString());
                    return new object[] { 0, 1, "医师信息上传成功|" + outputData.ToString() };
                }
                else
                {
                    WriteLog(sysdate + "  医师信息上传失败|" + outputData.ToString());
                    return new object[] { 0, 0, "医师信息上传失败|" + outputData.ToString() };
                }
            }
            catch (Exception ex)
            {
                WriteLog(sysdate + "  医师信息上传异常|" + ex.Message);
                return new object[] { 0, 0, "医师信息上传异常|" + ex.Message };
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

        #region 读卡超时控制
        static object[] CallWithTimeout(Action<object[]> action, object[] objParam, int timeoutMilliseconds)
        {
            Thread threadToKill = null;
            Action wrappedAction = () =>
            {
                threadToKill = Thread.CurrentThread;
                action(objParam);
            };

            IAsyncResult result = wrappedAction.BeginInvoke(null, null);
            if (result.AsyncWaitHandle.WaitOne(timeoutMilliseconds))
            {
                wrappedAction.EndInvoke(result);

                if (string.IsNullOrEmpty(DKXX))
                    return new object[] { 0, 0, "读卡信息失败" };
                string[] sVal = DKXX.Split('&');
                if (sVal[0].Equals("0"))
                {
                    return new object[] { 0, 0, sVal[1] };
                }
                else
                    return new object[] { 0, 1, sVal[1] };
            }
            else
            {
                threadToKill.Abort();
                //throw new TimeoutException();
                return new object[] { 0, 0, "读卡超时" };
            }
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
