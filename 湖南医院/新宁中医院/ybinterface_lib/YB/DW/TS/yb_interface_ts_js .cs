using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Srvtools;
using System.Net.NetworkInformation;
using System.Data;

//南京汤山地区医保接口文件
namespace ybinterface_lib
{
    public class yb_interface_ts_js
    {


        #region 加载DLL
        //取就诊号
        [DllImport(@"C:\HTHis\HInterface.dll", EntryPoint = "FGetRecCode", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        static extern int FGetRecCode(Int32 piRecType, StringBuilder psRecCode);

        //读卡
        [DllImport(@"C:\HTHis\HInterface.dll", EntryPoint = "FGetCardInfo", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        static extern int FGetCardInfo(Int32 piRecType, string psRecCode, string psVoucherID, ref  Int32 piInsID,    StringBuilder psCardID,     StringBuilder psName,    StringBuilder psAreaCode,    StringBuilder psQueryID,    StringBuilder psUnitID,    StringBuilder psUnitName,
              StringBuilder psSex,    StringBuilder psKind,    StringBuilder psBirthday,    StringBuilder psNational,    StringBuilder psIndustry,    StringBuilder psDuty,    StringBuilder psChronic,    StringBuilder psOthers1, ref float pcInHosNum, ref float pcAccIn, ref float pcAccOut, ref float pcFeeNO,
            ref float pcPubPay, ref float pcHelpPay, ref float pcSupplyPay, ref float pcOutpatSum, ref float pcOutpatGen1, ref float pcOutpatGen2, ref float pcOutpatGen3, ref float pcInpatSum, ref float pcInpatGen1, ref float pcInpatGen2, ref float pcOther1,
            ref float pcOther2, ref float pcBankAccPay, ref float pcOtrPay, ref float pcCashPay, ref float pcAccLeft);

        //门诊挂号`
        [DllImport(@"C:\HTHis\HInterface.dll", EntryPoint = "FOutpatReg", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        static extern int FOutpatReg(string psRecCode, string psRegName, string psDepartName, string psRegFeeCode, string psRegFeeName, float RegFee, string psDiagFeeCode, string psDiagFeeName, float DiagFee, string psFeeType,
            string psOpCode, string psRegDate, string pRegMode, ref float pcPubPay, ref float pcAccPay, ref float pcCashPay);

        //取消门诊挂号
        [DllImport(@"C:\HTHis\HInterface.dll", EntryPoint = "FCancleOutpatReg", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        static extern int FCancleOutpatReg(string psRecCode, string psOpCode);

        //取消门诊预结算
        [DllImport(@"C:\HTHis\HInterface.dll", EntryPoint = "FCancelTryOutpatBalance", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        static extern int FCancelTryOutpatBalance(string psRecCode, string psOpCode);

        //门诊费用明细录入
        [DllImport(@"C:\HTHis\HInterface.dll", EntryPoint = "FWriteFeeDetail", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        static extern int FWriteFeeDetail(Int32 piRecType, string psRecCode, string psItmFlag, string psItmCode, string psAliasCode, string psItmName, string psItmUnit, string psItmDesc, string psFeeCode, string psOTCCode,
            float pcQuantity, float pcPharPrice, float pcFactPrice, float pcDosage, string psFrequency, string psUsage,
            float pcDays, string psOpCode, string psDepCode, string psDocCode, string psRecDate, ref float pcRate, ref float pcSelfFee, ref float pcDeduct);


        //住院费用明细录入
        [DllImport(@"C:\HTHis\HInterface.dll", EntryPoint = "FWriteFeeDetail2", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        static extern int FWriteFeeDetail2(Int32 piShowMess, Int32 piRecType, string psRecCode, string psItmFlag, string psItmCode, string psAliasCode, string psItmName, string psItmUnit, string psItmDesc, string psFeeCode, string psOTCCode,
            float pcQuantity, float pcPharPrice, float pcFactPrice, float pcDosage, string psFrequency, string psUsage,
            float pcDays, string psOpCode, string psDepCode, string psDocCode, string psRecDate, ref float pcRate, ref float pcSelfFee, ref float pcDeduct, string psMess);

        //门诊预结算
        [DllImport(@"C:\HTHis\HInterface.dll", EntryPoint = "FTryOutpatBalance", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        static extern int FTryOutpatBalance(string psRecCode, string psOpCode, string psUseAcc, string psDepCode, string psDocCode, string psMedMode, string psRecClass, string psICDMode, string psICD, float pcOther1, float pcOther2,
            string psMemo,StringBuilder psBillCode, ref float pcSumFee, ref float pcGenFee, ref float pcFirstPay, ref float pcSelfFee, ref float pcPayLevel, ref float  pcPubPay , ref float pcPubSelf  , ref float pcHelpPay , ref float pcHelpSelf,
            ref float pcSupplyPay, ref float pcSupplySelf, ref float pcOtrPay, ref float pcMedAccPay, ref float pcBankAccPay, ref float pcCashPay);

        //门诊结算
        [DllImport(@"C:\HTHis\HInterface.dll", EntryPoint = "FOutpatBalance", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        static extern int FOutpatBalance(string psRecCode, string psOpCode, string psUseAcc, string psDepCode, string psDocCode, string psMedMode, string psRecClass, string psICDMode, string psICD, float pcOther1, float pcOther2,
            string psMemo,StringBuilder psBillCode, ref float pcSumFee, ref float pcGenFee, ref float pcFirstPay, ref float pcSelfFee, ref float pcPayLevel, ref float pcPubPay, ref float pcPubSelf, ref float pcHelpPay, ref float pcHelpSelf,
            ref float pcSupplyPay, ref float pcSupplySelf, ref float pcOtrPay, ref float pcMedAccPay, ref float pcBankAccPay, ref float pcCashPay);

        //门诊结算撤销
        [DllImport(@"C:\HTHis\HInterface.dll", EntryPoint = "FCancelOutpatBalance", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        static extern int FCancelOutpatBalance(string psRecCode, string psBillCode, string psOpCode, ref float pcSumFee, ref float pcGenFee, ref float pcFirstPay, ref float pcSelfFee, ref float pcPayLevel, ref float pcPubPay, ref float pcPubSelf,
             ref float pcHelpPay, ref float pcHelpSelf, ref float pcSupplyPay, ref float pcSupplySelf, ref float pcOtrPay, ref float pcMedAccPay, ref float pcBankAccPay, ref float pcCashPay);

        //住院登记
        [DllImport(@"C:\HTHis\HInterface.dll", EntryPoint = "FInpatReg", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        static extern int FInpatReg(string psRecCode, string psMedMode, string psMedClass, string psRegOpCode, string psBegDate, string psICDMode, string psICD, string psDepCode, string psSecCode, string psRegDoc, ref float pcInHosNum);

        //取消住院登记
        [DllImport(@"C:\HTHis\HInterface.dll", EntryPoint = "FCancelInpatReg", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        static extern int FCancelInpatReg(string psRecCode, string psOpCode);

        //修改住院登记信息
        [DllImport(@"C:\HTHis\HInterface.dll", EntryPoint = "FChgInpatReg", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        static extern int FChgInpatReg(string psRecCode, string psMedMode, string psMedClass, string psRegOpCode, string psBegDate, string psICDMode, string psICD, string psDepCode, string psSecCode, string psRegDoc);

        //出院
        [DllImport(@"C:\HTHis\HInterface.dll", EntryPoint = "FInpatLeave", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        static extern int FInpatLeave(string psRecCode, string psOutOpCode, string psEndDate, string psOutCause, string psICDMode, string psICD, string psOutDoc);

        //住院费用预结算
        [DllImport(@"C:\HTHis\HInterface.dll", EntryPoint = "FTryInpatBalance", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        static extern int FTryInpatBalance(string psRecCode, string psOpCode, string psUseAcc, string piLiquiMode, string psRefundID, float pcOther1, float pcOther2, string psMemo,StringBuilder psBillCode, ref float pcSumFee,
            ref float pcGenFee, ref float pcFirstPay, ref float pcSelfFee, ref float pcPayLevel, ref float pcPubPay, ref float pcPubSelf, ref float pcHelpPay, ref float pcHelpSelf, ref float pcSupplyPay, ref float pcSupplySelf,
            ref float pcOtrPay, ref float pcMedAccPay, ref float pcBankAccPay, ref float pcCashPay);

        //住院费用结算
        [DllImport(@"C:\HTHis\HInterface.dll", EntryPoint = "FInpatBalance", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        static extern int FInpatBalance(string psRecCode, string psOpCode, string psUseAcc, string piLiquiMode, string psRefundID, float pcOther1, float pcOther2, string psMemo, StringBuilder psBillCode, ref float pcSumFee,
            ref float pcGenFee, ref float pcFirstPay, ref float pcSelfFee, ref float pcPayLevel, ref float pcPubPay, ref float pcPubSelf, ref float pcHelpPay, ref float pcHelpSelf, ref float pcSupplyPay, ref float pcSupplySelf,
            ref float pcOtrPay, ref float pcMedAccPay, ref float pcBankAccPay, ref float pcCashPay);

        //取消住院结帐
        [DllImport(@"C:\HTHis\HInterface.dll", EntryPoint = "FCancelInpatBalance", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        static extern int FCancelInpatBalance(string psRecCode, string psBillCode, string psOpCode, ref float pcSumFee, ref float pcGenFee, ref float pcFirstPay, ref float pcSelfFee, ref float pcPayLevel, ref float pcPubPay,
            ref float pcPubSelf, ref float pcHelpPay, ref float pcHelpSelf, ref float pcSupplyPay, ref float pcSupplySelf, ref float pcOtrPay, ref float pcMedAccPay, ref float pcBankAccPay, ref float pcCashPay);

        //数据同步
        [DllImport(@"C:\HTHis\HInterface.dll", EntryPoint = "FSynData", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        static extern int FSynData(Int32 piType, string psRecCode);

        //上传
        [DllImport(@"C:\HTHis\HInterface.dll", EntryPoint = "FUpLoad", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        static extern int FUpLoad(Int32 piType, string psRecCode);

        //通用导入函数
        [DllImport(@"C:\HTHis\HInterface.dll", EntryPoint = "FImpInfo", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        static extern int FImpInfo(Int32 piType, string psInfo1, string psInfo2, string psInfo3, string psInfo4, string psRemark, string psOpStaus);

        //通用导出函数
        [DllImport(@"C:\HTHis\HInterface.dll", EntryPoint = "FExpInfo", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        static extern int FExpInfo(string psTable, string psFile);


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
        internal static string YLGHBH = lItem[0].DDYLJGBH; //医疗机构编号
        internal static string DDYLJGBH = lItem[0].DDYLJGBH;//医疗机构编号
        internal static string DDYLJGMC = lItem[0].DDYLJGMC;//医院名称

        static List<string> list = new List<string>();
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
            else
                return new object[] { 1, 1, "连接医保网成功" };
        }
        #endregion

        #region 退出
        public static object[] YBEXIT(object[] objParam)
        {
            int i = 0;
            if (i == 0)
            {
                return new object[] { 0, 1, "医保退出成功|" };
            }
            else
            {
                return new object[] { 0, 0, "医保退出失败|" };
            }
        }
        #endregion

        #region 医保门诊读卡
        public static object[] YBMZDK(object[] objParam)
        {

             try
            {
                string sysdate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                string jzcode = "";

                

                #region 入参
                Int32 piRecType = 0;            //类型 0.门诊，1.住院
                string psRecCode = "";          //就诊号 纯刷卡可为空，业务刷卡需要传值
                string psVoucherID = "";        //个人证号  纯刷卡可为空
                Int32 piInsID = 0;              //个人ID
                StringBuilder psCardID = new StringBuilder(16);           //卡号
                StringBuilder psName = new StringBuilder(8);             //姓名
                StringBuilder psAreaCode = new StringBuilder(8);         //区属代码
                StringBuilder psQueryID = new StringBuilder(18);          //身份证号码
                StringBuilder psUnitID = new StringBuilder(8);           //单位ID
                StringBuilder psUnitName = new StringBuilder(50);         //单位名称
                StringBuilder psSex = new StringBuilder(2);              //性别
                StringBuilder psKind = new StringBuilder(4);             //人员类别
                StringBuilder psBirthday = new StringBuilder(10);         //出生日期
                StringBuilder psNational = new StringBuilder(20);         //民族
                StringBuilder psIndustry = new StringBuilder(20);         //本人身份
                StringBuilder psDuty = new StringBuilder(30);             //行政职务
                StringBuilder psChronic = new StringBuilder(200);          //门诊特殊病种
                StringBuilder psOthers1 = new StringBuilder(200);          //其它
                float pcInHosNum = 0;        //本年累计住院次数
                float pcAccIn = 0;           //帐户总收
                float pcAccOut = 0;          //帐户总支
                float pcFeeNO = 0;           //支出版本号
                float pcPubPay = 0;          //本年统筹支付累计
                float pcHelpPay = 0;         //本年大病基金支付累计
                float pcSupplyPay = 0;       //本年公务员补充/企业补充支付累计
                float pcOutpatSum = 0;       //本年普通门诊费用累计
                float pcOutpatGen1 = 0;      //本年普通门诊三个范围内费用累计
                float pcOutpatGen2 = 0;      //本年特殊门诊三个范围内费用累计
                float pcOutpatGen3 = 0;      //本年比照住院三个范围内费用累计
                float pcInpatSum = 0;        //本年普通住院费用累计
                float pcInpatGen1 = 0;       //本年普通住院三个范围内费用累计
                float pcInpatGen2 = 0;       //本年家庭病床住院三个范围内累计
                float pcOther1 = 0;          //本年累计自付
                float pcOther2 = 0;          //本年累计自费
                float pcBankAccPay = 0;      //保留
                float pcOtrPay = 0;          //本年其它基金支付累计
                float pcCashPay = 0;         //本年现金支付累计
                float pcAccLeft = 0;         //帐户余额
                #endregion

                
                

                WriteLog(sysdate + "  进入读卡...");




                //调用读卡
                int i = FGetCardInfo(piRecType, psRecCode, psVoucherID, ref  piInsID, psCardID, psName, psAreaCode, psQueryID, psUnitID, psUnitName,
                  psSex, psKind, psBirthday, psNational, psIndustry, psDuty, psChronic, psOthers1, ref   pcInHosNum, ref   pcAccIn, ref   pcAccOut, ref   pcFeeNO,
              ref   pcPubPay, ref   pcHelpPay, ref   pcSupplyPay, ref   pcOutpatSum, ref   pcOutpatGen1, ref   pcOutpatGen2, ref   pcOutpatGen3, ref   pcInpatSum, ref   pcInpatGen1, ref   pcInpatGen2, ref   pcOther1,
              ref   pcOther2, ref   pcBankAccPay, ref   pcOtrPay, ref   pcCashPay, ref   pcAccLeft);




                if (i == 0)
                {
                    List<string> liSQL = new List<string>();



                    #region 出参返回

                    string brithday = "";
                    string age = "";
                    if (psQueryID.Length == 18)
                    {
                        brithday = psQueryID.ToString().Substring(6, 8).Insert(6, "-").Insert(4, "-");
                        age = (DateTime.Now.Year - Convert.ToDateTime(brithday).Year).ToString();
                    }
                    else if (psQueryID.Length == 15)
                    {
                        brithday = psQueryID.ToString().Substring(6, 6).Insert(4, "-").Insert(2, "-");
                        age = (DateTime.Now.Year - Convert.ToDateTime(brithday).Year).ToString();
                    }


                    string ylrylbmc = "";
                    string strSql = string.Format(@"SELECT  CODE, NAME FROM  YBXMLBZD where LBMC='医疗人员类别' and CODE='{0}'", psKind);
                    DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                    if (ds.Tables[0].Rows.Count > 0)
                        ylrylbmc = ds.Tables[0].Rows[0]["NAME"].ToString();

                    string tcqhmc = "";
                    strSql = string.Format(@"SELECT  CODE, NAME FROM  YBXMLBZD where LBMC='统筹区号' and CODE='{0}'", psAreaCode);
                    ds.Tables.Clear();
                    ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                    if (ds.Tables[0].Rows.Count > 0)
                        tcqhmc = ds.Tables[0].Rows[0]["NAME"].ToString();

                    string ydrybz = "";
                    if(psCardID.ToString().Contains("YD"))
                    {
                        ydrybz = "1";
                    }
                    else
                    {
                        ydrybz = "0";
                    }


                    string outputData = piInsID + "|" + psUnitID + "|" + psQueryID + "|" + psName + "|" + psSex + "|" + psNational + "|" + psBirthday + "|" + psCardID + "|" + psKind + "||" + ydrybz + "|" + psAreaCode + "|||" +
                   pcAccLeft + "||" + pcOther1 + "|" + pcPubPay + "|" + pcSupplyPay + "|" + pcOutpatSum + "||" + pcOutpatSum + "|||" + pcInHosNum + "|" + psUnitName + "|" + age + "|||" + pcInpatSum + "|" + pcOutpatSum + "|" +
                   pcOutpatGen2 + "||||";


                    WriteLog(sysdate + "  门诊读卡返回参数|" + outputData.ToString());
                    //1000677037|10094171|南京陆拾度温泉酒店有限公司|320121197509114128|孙志花|2|11|0|320115|1252.7|0|0|1|||0||0||0||0||0||0||0|||0||0||0|||0||0|||1|||0|0|0|||0|
                    
                    #endregion
                    //中心交易流水号^联脱机标志^输出参数^
                    //个人编号|单位编号|身份证号|姓名|性别|民族|出生日期|社会保障卡卡号|
                    //医疗待遇类别|人员参保状态|异地人员标志|统筹区号|年度|在院状态|帐户余额|本年医疗费累计|
                    //本年帐户支出累计|本年统筹支出累计|本年救助金支出累计|本年公务员补助基金累计|本年城镇居民门诊统筹支付累计|
                    //进入统筹费用累计|进入救助金费用累计|起付标准累计|本年住院次数|单位名称|年龄|参保单位类型|经办机构编码
                    //累计门诊支付|累计特殊门诊|缴费类型|医保门慢、特资质|医保门慢、特病种说明｜



                    strSql = string.Format(@"delete from ybickxx where grbh='{0}'", piInsID);
                    liSQL.Add(strSql);
                    strSql = string.Format(@"insert into YBICKXX(
                                            GRBH,DWBH,DWMC,GMSFHM,XM,XB,YLRYLB,YDRYBZ,DQBH,GRZHYE,
                                            ZYKT,ZYCS,DYSSBZ,DYBSSYY,BQDJQK,YBMMZG,YBMMBZ,YBMJZG,YBMJBZ,YBMIZG,
                                            YBMIBZ,YBJGGLSZG,YBJGGLSBZ,YBMZXYBZG,YBMZXYBBZ,YBMTZG,YBMTBZ,YBTYZG,YBTYBZ,YBTYMCBM,
                                            JMMDZG,JMMDBZ,JMMZXYBZG,JMMZXYBBZ,JMTYZG,JMTYBZ,JMTYMCBM,NMGMDZG,NMGMDBZ,NMGTYZG,
                                            NMGTYBZ,NMGTYMCBM,NFSSZGMZTC,SYSPLX,MMSYKBJE,MTPZZLSYKBJE,SYSDATE,KH,psBirthday,psIndustry,psDuty,psOthers1) VALUES(
                                            '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}',
                                            '{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}',
                                            '{20}','{21}','{22}','{23}','{24}','{25}','{26}','{27}','{28}','{29}',
                                            '{30}','{31}','{32}','{33}','{34}','{35}','{36}','{37}','{38}','{39}',
                                            '{40}','{41}','{42}','{43}','{44}','{45}','{46}','{47}','{48}','{49}','{50}','{51}')",
                                             piInsID, psUnitID, psUnitName, psQueryID, psName, psSex, psKind, "", psAreaCode, pcAccLeft,
                                             "", pcInHosNum, "", "", psChronic, null, null, null, null, null,
                                             null, null, null, null, null, null, null, null, null, null,
                                             null, null, null, null, null, null, null, null, null, null,
                                             null, null, null, null, null, null, sysdate,psCardID,psBirthday,psIndustry,psDuty,psOthers1);
                    liSQL.Add(strSql);

                    object[] obj = liSQL.ToArray();
                    obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);
                    if (obj[1].ToString().Equals("1"))
                    {
                        WriteLog(sysdate + "  读卡信息成功|" + outputData);
                        return new object[] { 0, 1, outputData };
                    }
                    else
                    {
                        WriteLog(sysdate + "  保存读卡信息失败|" + obj[2].ToString());
                        return new object[] { 0, 0, obj[2].ToString() };
                    }
                }
                else
                {
                    WriteLog(sysdate + "  进入门诊读卡失败|");
                    return new object[] { 0, 0, "读卡失败" };
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
                string jzcode = "";

                

                #region 入参
                Int32 piRecType = 1;            //类型 0.门诊，1.住院
                string psRecCode = "";          //就诊号 纯刷卡可为空，业务刷卡需要传值
                string psVoucherID = "";        //个人证号  纯刷卡可为空
                Int32 piInsID = 0;              //个人ID
                StringBuilder psCardID = new StringBuilder(16);           //卡号
                StringBuilder psName = new StringBuilder(8);             //姓名
                StringBuilder psAreaCode = new StringBuilder(8);         //区属代码
                StringBuilder psQueryID = new StringBuilder(18);          //身份证号码
                StringBuilder psUnitID = new StringBuilder(8);           //单位ID
                StringBuilder psUnitName = new StringBuilder(50);         //单位名称
                StringBuilder psSex = new StringBuilder(2);              //性别
                StringBuilder psKind = new StringBuilder(4);             //人员类别
                StringBuilder psBirthday = new StringBuilder(10);         //出生日期
                StringBuilder psNational = new StringBuilder(20);         //民族
                StringBuilder psIndustry = new StringBuilder(20);         //本人身份
                StringBuilder psDuty = new StringBuilder(30);             //行政职务
                StringBuilder psChronic = new StringBuilder(200);          //门诊特殊病种
                StringBuilder psOthers1 = new StringBuilder(200);          //其它
                float pcInHosNum = 0;        //本年累计住院次数
                float pcAccIn = 0;           //帐户总收
                float pcAccOut = 0;          //帐户总支
                float pcFeeNO = 0;           //支出版本号
                float pcPubPay = 0;          //本年统筹支付累计
                float pcHelpPay = 0;         //本年大病基金支付累计
                float pcSupplyPay = 0;       //本年公务员补充/企业补充支付累计
                float pcOutpatSum = 0;       //本年普通门诊费用累计
                float pcOutpatGen1 = 0;      //本年普通门诊三个范围内费用累计
                float pcOutpatGen2 = 0;      //本年特殊门诊三个范围内费用累计
                float pcOutpatGen3 = 0;      //本年比照住院三个范围内费用累计
                float pcInpatSum = 0;        //本年普通住院费用累计
                float pcInpatGen1 = 0;       //本年普通住院三个范围内费用累计
                float pcInpatGen2 = 0;       //本年家庭病床住院三个范围内累计
                float pcOther1 = 0;          //本年累计自付
                float pcOther2 = 0;          //本年累计自费
                float pcBankAccPay = 0;      //保留
                float pcOtrPay = 0;          //本年其它基金支付累计
                float pcCashPay = 0;         //本年现金支付累计
                float pcAccLeft = 0;         //帐户余额
                #endregion

                
                

                WriteLog(sysdate + "  进入读卡...");


                //调用读卡
                int i = FGetCardInfo(piRecType, psRecCode, psVoucherID, ref  piInsID, psCardID, psName, psAreaCode, psQueryID, psUnitID, psUnitName,
                  psSex, psKind, psBirthday, psNational, psIndustry, psDuty, psChronic, psOthers1, ref   pcInHosNum, ref   pcAccIn, ref   pcAccOut, ref   pcFeeNO,
              ref   pcPubPay, ref   pcHelpPay, ref   pcSupplyPay, ref   pcOutpatSum, ref   pcOutpatGen1, ref   pcOutpatGen2, ref   pcOutpatGen3, ref   pcInpatSum, ref   pcInpatGen1, ref   pcInpatGen2, ref   pcOther1,
              ref   pcOther2, ref   pcBankAccPay, ref   pcOtrPay, ref   pcCashPay, ref   pcAccLeft);




                if (i == 0)
                {
                    List<string> liSQL = new List<string>();



                    #region 出参返回

                    string brithday = "";
                    string age = "";
                    if (psQueryID.Length == 18)
                    {
                        brithday = psQueryID.ToString().Substring(6, 8).Insert(6, "-").Insert(4, "-");
                        age = (DateTime.Now.Year - Convert.ToDateTime(brithday).Year).ToString();
                    }
                    else if (psQueryID.Length == 15)
                    {
                        brithday = psQueryID.ToString().Substring(6, 6).Insert(4, "-").Insert(2, "-");
                        age = (DateTime.Now.Year - Convert.ToDateTime(brithday).Year).ToString();
                    }


                    string ylrylbmc = "";
                    string strSql = string.Format(@"SELECT  CODE, NAME FROM  YBXMLBZD where LBMC='医疗人员类别' and CODE='{0}'", psKind);
                    DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                    if (ds.Tables[0].Rows.Count > 0)
                        ylrylbmc = ds.Tables[0].Rows[0]["NAME"].ToString();

                    string tcqhmc = "";
                    strSql = string.Format(@"SELECT  CODE, NAME FROM  YBXMLBZD where LBMC='统筹区号' and CODE='{0}'", psAreaCode);
                    ds.Tables.Clear();
                    ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                    if (ds.Tables[0].Rows.Count > 0)
                        tcqhmc = ds.Tables[0].Rows[0]["NAME"].ToString();


                     string ydrybz = "";
                    if(psCardID.ToString().Contains("YD"))
                    {
                        ydrybz = "1";
                    }                    
                    else
                    {
                        ydrybz = "0";
                    }

                    string outputData = piInsID + "|" + psUnitID + "|" + psQueryID + "|" + psName + "|" + psSex + "|" + psNational + "|" + psBirthday + "|" + psCardID + "|" + psKind + "||" + ydrybz + "|" + psAreaCode + "|||" +
                   pcAccLeft + "||" + pcOther1 + "|" + pcPubPay + "|" + pcSupplyPay + "|" + pcOutpatSum + "||" + pcOutpatSum + "|||" + pcInHosNum + "|" + psUnitName + "|" + age + "|||" + pcInpatSum + "|" + pcOutpatSum + "|" +
                   pcOutpatGen2 + "||||";


                    WriteLog(sysdate + "  门诊读卡返回参数|" + outputData.ToString());
                    //1000677037|10094171|南京陆拾度温泉酒店有限公司|320121197509114128|孙志花|2|11|0|320115|1252.7|0|0|1|||0||0||0||0||0||0||0|||0||0||0|||0||0|||1|||0|0|0|||0|


                    #endregion
                    //中心交易流水号^联脱机标志^输出参数^
                    //个人编号|单位编号|身份证号|姓名|性别|民族|出生日期|社会保障卡卡号|
                    //医疗待遇类别|人员参保状态|异地人员标志|统筹区号|年度|在院状态|帐户余额|本年医疗费累计|
                    //本年帐户支出累计|本年统筹支出累计|本年救助金支出累计|本年公务员补助基金累计|本年城镇居民门诊统筹支付累计|
                    //进入统筹费用累计|进入救助金费用累计|起付标准累计|本年住院次数|单位名称|年龄|参保单位类型|经办机构编码
                    //累计门诊支付|累计特殊门诊|缴费类型|医保门慢、特资质|医保门慢、特病种说明｜

                    strSql = string.Format(@"delete from ybickxx where grbh='{0}'", piInsID);
                    liSQL.Add(strSql);
                    strSql = string.Format(@"insert into YBICKXX(
                                            GRBH,DWBH,DWMC,GMSFHM,XM,XB,YLRYLB,YDRYBZ,DQBH,GRZHYE,
                                            ZYKT,ZYCS,DYSSBZ,DYBSSYY,BQDJQK,YBMMZG,YBMMBZ,YBMJZG,YBMJBZ,YBMIZG,
                                            YBMIBZ,YBJGGLSZG,YBJGGLSBZ,YBMZXYBZG,YBMZXYBBZ,YBMTZG,YBMTBZ,YBTYZG,YBTYBZ,YBTYMCBM,
                                            JMMDZG,JMMDBZ,JMMZXYBZG,JMMZXYBBZ,JMTYZG,JMTYBZ,JMTYMCBM,NMGMDZG,NMGMDBZ,NMGTYZG,
                                            NMGTYBZ,NMGTYMCBM,NFSSZGMZTC,SYSPLX,MMSYKBJE,MTPZZLSYKBJE,SYSDATE,KH,psBirthday,psIndustry,psDuty,psOthers1) VALUES(
                                            '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}',
                                            '{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}',
                                            '{20}','{21}','{22}','{23}','{24}','{25}','{26}','{27}','{28}','{29}',
                                            '{30}','{31}','{32}','{33}','{34}','{35}','{36}','{37}','{38}','{39}',
                                            '{40}','{41}','{42}','{43}','{44}','{45}','{46}','{47}','{48}','{49}','{50}','{51}')",
                                             piInsID, psUnitID, psUnitName, psQueryID, psName, psSex, psKind, "", psAreaCode, pcAccLeft,
                                             "", pcInHosNum, "", "", psChronic, null, null, null, null, null,
                                             null, null, null, null, null, null, null, null, null, null,
                                             null, null, null, null, null, null, null, null, null, null,
                                             null, null, null, null, null, null, sysdate, psCardID, psBirthday, psIndustry, psDuty, psOthers1);
                    liSQL.Add(strSql);

                    object[] obj = liSQL.ToArray();
                    obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);
                    if (obj[1].ToString().Equals("1"))
                    {
                        WriteLog(sysdate + "  读卡信息成功|" + outputData);
                        return new object[] { 0, 1, outputData };
                    }
                    else
                    {
                        WriteLog(sysdate + "  保存读卡信息失败|" + obj[2].ToString());
                        return new object[] { 0, 0, obj[2].ToString() };
                    }
                }
                else
                {
                    WriteLog(sysdate + "  进入门诊读卡失败|");
                    return new object[] { 0, 0, "读卡失败" };
                }
            }
            catch(SyntaxErrorException err)
            {
                return new object[] { 0, 0, "请插入医保卡或医保读卡出错｜" };
            }
        }
        #endregion

        #region 业务读卡
        public static object[] YBDK(object[] objParam)
        {
            try
            {
                string sysdate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                string jzcode = "";

                

                #region 入参
                Int32 piRecType = 0;            //类型 0.门诊，1.住院
                string psRecCode = "";          //就诊号 纯刷卡可为空，业务刷卡需要传值
                string psVoucherID = "";        //个人证号  纯刷卡可为空
                Int32 piInsID = 0;              //个人ID
                StringBuilder psCardID = new StringBuilder(16);           //卡号
                StringBuilder psName = new StringBuilder(8);             //姓名
                StringBuilder psAreaCode = new StringBuilder(8);         //区属代码
                StringBuilder psQueryID = new StringBuilder(18);          //身份证号码
                StringBuilder psUnitID = new StringBuilder(8);           //单位ID
                StringBuilder psUnitName = new StringBuilder(50);         //单位名称
                StringBuilder psSex = new StringBuilder(2);              //性别
                StringBuilder psKind = new StringBuilder(4);             //人员类别
                StringBuilder psBirthday = new StringBuilder(10);         //出生日期
                StringBuilder psNational = new StringBuilder(20);         //民族
                StringBuilder psIndustry = new StringBuilder(20);         //本人身份
                StringBuilder psDuty = new StringBuilder(30);             //行政职务
                StringBuilder psChronic = new StringBuilder(200);          //门诊特殊病种
                StringBuilder psOthers1 = new StringBuilder(200);          //其它
                float pcInHosNum = 0;        //本年累计住院次数
                float pcAccIn = 0;           //帐户总收
                float pcAccOut = 0;          //帐户总支
                float pcFeeNO = 0;           //支出版本号
                float pcPubPay = 0;          //本年统筹支付累计
                float pcHelpPay = 0;         //本年大病基金支付累计
                float pcSupplyPay = 0;       //本年公务员补充/企业补充支付累计
                float pcOutpatSum = 0;       //本年普通门诊费用累计
                float pcOutpatGen1 = 0;      //本年普通门诊三个范围内费用累计
                float pcOutpatGen2 = 0;      //本年特殊门诊三个范围内费用累计
                float pcOutpatGen3 = 0;      //本年比照住院三个范围内费用累计
                float pcInpatSum = 0;        //本年普通住院费用累计
                float pcInpatGen1 = 0;       //本年普通住院三个范围内费用累计
                float pcInpatGen2 = 0;       //本年家庭病床住院三个范围内累计
                float pcOther1 = 0;          //本年累计自付
                float pcOther2 = 0;          //本年累计自费
                float pcBankAccPay = 0;      //保留
                float pcOtrPay = 0;          //本年其它基金支付累计
                float pcCashPay = 0;         //本年现金支付累计
                float pcAccLeft = 0;         //帐户余额
                #endregion

                
                

                WriteLog(sysdate + "  进入读卡...");

                if (!string.IsNullOrEmpty(objParam[0].ToString()))
                {
                    psRecCode = objParam[0].ToString();
                }

                if (!string.IsNullOrEmpty(objParam[1].ToString()))
                {
                    psVoucherID = objParam[1].ToString();
                }

                if(objParam[2].ToString().Equals("M"))
                    piRecType = 0;
                else if(objParam[2].ToString().Equals("Z"))
                    piRecType = 1;


                //调用读卡
                int i = FGetCardInfo(piRecType, psRecCode, psVoucherID, ref  piInsID,  psCardID,  psName,  psAreaCode,  psQueryID,  psUnitID,  psUnitName,
                  psSex,  psKind,  psBirthday,  psNational,  psIndustry,  psDuty,  psChronic,  psOthers1, ref   pcInHosNum, ref   pcAccIn, ref   pcAccOut, ref   pcFeeNO,
              ref   pcPubPay, ref   pcHelpPay, ref   pcSupplyPay, ref   pcOutpatSum, ref   pcOutpatGen1, ref   pcOutpatGen2, ref   pcOutpatGen3, ref   pcInpatSum, ref   pcInpatGen1, ref   pcInpatGen2, ref   pcOther1,
              ref   pcOther2, ref   pcBankAccPay, ref   pcOtrPay, ref   pcCashPay, ref   pcAccLeft);




                if (i == 0)
                {
                    List<string> liSQL = new List<string>();



                    #region 出参返回

                    string brithday = "";
                    string age = "";
                    if (psQueryID.Length == 18)
                    {
                        brithday = psQueryID.ToString().Substring(6, 8).Insert(6, "-").Insert(4, "-");
                        age = (DateTime.Now.Year - Convert.ToDateTime(brithday).Year).ToString();
                    }
                    else if (psQueryID.Length == 15)
                    {
                        brithday = psQueryID.ToString().Substring(6, 6).Insert(4, "-").Insert(2, "-");
                        age = (DateTime.Now.Year - Convert.ToDateTime(brithday).Year).ToString();
                    }


                    string ylrylbmc = "";
                    string strSql = string.Format(@"SELECT  CODE, NAME FROM  YBXMLBZD where LBMC='医疗人员类别' and CODE='{0}'", psKind);
                    DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                    if (ds.Tables[0].Rows.Count > 0)
                        ylrylbmc = ds.Tables[0].Rows[0]["NAME"].ToString();

                    string tcqhmc = "";
                    strSql = string.Format(@"SELECT  CODE, NAME FROM  YBXMLBZD where LBMC='统筹区号' and CODE='{0}'", psAreaCode);
                    ds.Tables.Clear();
                    ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                    if (ds.Tables[0].Rows.Count > 0)
                        tcqhmc = ds.Tables[0].Rows[0]["NAME"].ToString();


                     string ydrybz = "";
                     if (psCardID.ToString().Contains("YD"))
                    {
                        ydrybz = "1";
                    }
                    else
                    {
                        ydrybz = "0";
                    }


                    string outputData = piInsID + "|" + psUnitID + "|" + psQueryID + "|" + psName + "|" + psSex + "|" + psNational + "|" + psBirthday + "|" + psCardID + "|" + psKind + "||" + ydrybz + "|" + psAreaCode + "|||" +
                   pcAccLeft + "||" + pcOther1 + "|" + pcPubPay + "|" + pcSupplyPay + "|" + pcOutpatSum + "||" + pcOutpatSum + "|||" + pcInHosNum + "|" + psUnitName + "|" + age + "|||" + pcInpatSum + "|" + pcOutpatSum + "|" +
                   pcOutpatGen2 + "||||";


                    WriteLog(sysdate + "  门诊读卡返回参数|" + outputData.ToString());
                    //1000677037|10094171|南京陆拾度温泉酒店有限公司|320121197509114128|孙志花|2|11|0|320115|1252.7|0|0|1|||0||0||0||0||0||0||0|||0||0||0|||0||0|||1|||0|0|0|||0|


                    #endregion
                    //中心交易流水号^联脱机标志^输出参数^
                    //个人编号|单位编号|身份证号|姓名|性别|民族|出生日期|社会保障卡卡号|
                    //医疗待遇类别|人员参保状态|异地人员标志|统筹区号|年度|在院状态|帐户余额|本年医疗费累计|
                    //本年帐户支出累计|本年统筹支出累计|本年救助金支出累计|本年公务员补助基金累计|本年城镇居民门诊统筹支付累计|
                    //进入统筹费用累计|进入救助金费用累计|起付标准累计|本年住院次数|单位名称|年龄|参保单位类型|经办机构编码
                    //累计门诊支付|累计特殊门诊|缴费类型|医保门慢、特资质|医保门慢、特病种说明｜

                    strSql = string.Format(@"delete from ybickxx where grbh='{0}'", piInsID);
                    liSQL.Add(strSql);
                    strSql = string.Format(@"insert into YBICKXX(
                                            GRBH,DWBH,DWMC,GMSFHM,XM,XB,YLRYLB,YDRYBZ,DQBH,GRZHYE,
                                            ZYKT,ZYCS,DYSSBZ,DYBSSYY,BQDJQK,YBMMZG,YBMMBZ,YBMJZG,YBMJBZ,YBMIZG,
                                            YBMIBZ,YBJGGLSZG,YBJGGLSBZ,YBMZXYBZG,YBMZXYBBZ,YBMTZG,YBMTBZ,YBTYZG,YBTYBZ,YBTYMCBM,
                                            JMMDZG,JMMDBZ,JMMZXYBZG,JMMZXYBBZ,JMTYZG,JMTYBZ,JMTYMCBM,NMGMDZG,NMGMDBZ,NMGTYZG,
                                            NMGTYBZ,NMGTYMCBM,NFSSZGMZTC,SYSPLX,MMSYKBJE,MTPZZLSYKBJE,SYSDATE,KH,psBirthday,psIndustry,psDuty,psOthers1) VALUES(
                                            '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}',
                                            '{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}',
                                            '{20}','{21}','{22}','{23}','{24}','{25}','{26}','{27}','{28}','{29}',
                                            '{30}','{31}','{32}','{33}','{34}','{35}','{36}','{37}','{38}','{39}',
                                            '{40}','{41}','{42}','{43}','{44}','{45}','{46}','{47}','{48}','{49}','{50}','{51}')",
                                              piInsID, psUnitID, psUnitName, psQueryID, psName, psSex, psKind, "", psAreaCode, pcAccLeft,
                                              "", pcInHosNum, "", "", psChronic, null, null, null, null, null,
                                              null, null, null, null, null, null, null, null, null, null,
                                              null, null, null, null, null, null, null, null, null, null,
                                              null, null, null, null, null, null, sysdate, psCardID, psBirthday, psIndustry, psDuty, psOthers1);
                    liSQL.Add(strSql);

                    object[] obj = liSQL.ToArray();
                    obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);
                    if (obj[1].ToString().Equals("1"))
                    {
                        WriteLog(sysdate + "  读卡信息成功|" + outputData);
                        return new object[] { 0, 1, outputData };
                    }
                    else
                    {
                        WriteLog(sysdate + "  保存读卡信息失败|" + obj[2].ToString());
                        return new object[] { 0, 0, obj[2].ToString() };
                    }
                }
                else
                {
                    WriteLog(sysdate + "  进入门诊读卡失败|");
                    return new object[] { 0, 0, "读卡失败" };
                }
            }
            catch
            {
                return new object[] { 0, 0, "请插入医保卡或医保读卡出错｜" };
            }
        }
        #endregion

        #region 从医保取就诊流水号
        public static object[] GetJZLSH(object[] objParam)
        {
            string sysdate = GetServerDateTime();
            string flag = objParam[0].ToString();
            int piRecType = 0;
            StringBuilder psRecCode = new StringBuilder(32);

            if (flag == "M")
            {
                piRecType = 0;
            }
            else if (flag == "Z")
            {
                piRecType = 1;
            }


            int i = FGetRecCode(piRecType, psRecCode);

            if (i == 0)
            {
                WriteLog(sysdate + "成功获取医保就诊流水号" + psRecCode.ToString());
                return new object[] { 0, 1, psRecCode };
            }
            else
            {
                return new object[] { 0, 0, "获取医保就诊流水号失败" };
            }
           
        }
        #endregion

        #region 从数据库中取医保就诊流水号
        public static object[] GetJZLSHFromDataBase(string jzlsh)
        {

            string strsql = string.Format("select ybjzlsh from ybmzzydjdr where jzlsh = '{0}' and cxbz = 1", jzlsh);

            DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strsql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            if (ds.Tables[0].Rows.Count == 0)
                return new object[] { 0, 0, "该患者未医保门诊登记" };
            string ybjzlsh = ds.Tables[0].Rows[0]["ybjzlsh"].ToString();  //医保就诊流水号

            return new object[] {0,1,ybjzlsh};
        }
        #endregion

        #region 数据库登记撤销
        public static void YBMZDJCX_N(object[] objParam)
        {
            string jzlsh = objParam[0].ToString();
            string djh = objParam[1].ToString();
            string sysdate = GetServerDateTime();

            string strSql = string.Format(@"insert into ybmzzydjdr(
                                        jzlsh,jylsh,yldylb,ghdjsj,bzbm,bzmc,bq,ksbh,ksmc,cwh,
                                        ysdm,jbr,dwmc,grbh,xm,xb,ybjzlsh,kh,yllb,tcqh,
                                        ydrybz,nl,csrq,zhye,jzbz,cxbz,sysdate)
                                        select jzlsh,jylsh,yldylb,ghdjsj,bzbm,bzmc,bq,ksbh,ksmc,cwh,
                                        ysdm,'{1}',dwmc,grbh,xm,xb,ybjzlsh,kh,yllb,tcqh,
                                        ydrybz,nl,csrq,zhye,jzbz,0, '{2}' 
                                        from ybmzzydjdr where jzlsh='{0}' and cxbz=1", jzlsh, CliUtils.fUserName, sysdate);
            list.Add(strSql);
            strSql = string.Format(@"update ybmzzydjdr set cxbz=2 where jzlsh='{0}' and cxbz=1", jzlsh);
            list.Add(strSql);

        }
        #endregion

        #region 数据库结算撤销
        public static void YBMZJSCX_N(object[] objParam)
        {
            string jzlsh = objParam[0].ToString();
            string djh = objParam[1].ToString();
            string sysdate = GetServerDateTime();

            string strSql = string.Format(@"insert into ybfyjsdr (jzlsh,jylsh,djhin,cyrq,cyyy,bzbm,bzmc,yllb,xm,kh,
                                        grbh,ylfze,zbxje,tcjjzf,dejjzf,dbbyzf,mzjzfy,zhzf,xjzf,zhzfzf,
                                        zhzfzl,xjzhzf,xjzhzl,ybfwnfy,bcjsqzhye,dbzbc,smxx,yfhj,zlxmfhj,bbzf,
                                        bz6,cfmxjylsh,zcfbz,jbr,djh,ybjzlsh,jsrq,yblx,cxbz,sysdate) select jzlsh,jylsh,djhin,cyrq,cyyy,bzbm,bzmc,yllb,xm,kh,
                                        grbh,ylfze,zbxje,tcjjzf,dejjzf,dbbyzf,mzjzfy,zhzf,xjzf,zhzfzf,
                                        zhzfzl,xjzhzf,xjzhzl,ybfwnfy,bcjsqzhye,dbzbc,smxx,yfhj,zlxmfhj,bbzf,
                                        bz6,cfmxjylsh,zcfbz,'{3}',djh,ybjzlsh,jsrq,yblx,0,'{2}' from ybfyjsdr where jzlsh='{0}' and djhin='{1}' and cxbz=1", jzlsh, djh, sysdate, CliUtils.fUserName);
            list.Add(strSql);
            strSql = string.Format(@"update ybfyjsdr set cxbz=2 where jzlsh='{0}' and djhin='{1}' and cxbz=1", jzlsh, djh);
            list.Add(strSql);

        }
        #endregion

        #region 数据库上传撤销
        public static void YBMZSCCX_N(object[] objParam)
        {

            string sysdate = GetServerDateTime();
            string jzlsh = objParam[0].ToString();
            //获取上传处方信息
            string strSql = string.Format(@"select distinct jylsh,ybjzlsh from ybcfmxscindr where jzlsh='{0}' and cxbz=1 
                                            and jylsh not in(select isnull(cfmxjylsh,'') as cfjylsh from ybfyjsdr where cxbz=1 and jzlsh='{0}')"
                                            , jzlsh);
            DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            if (ds.Tables[0].Rows.Count == 0)
            {
                return;
            }
            string cfjylsh = ds.Tables[0].Rows[0]["jylsh"].ToString();
            string ybjzlsh = ds.Tables[0].Rows[0]["ybjzlsh"].ToString();
            string cfh = "";
            string cflsh = "";

             strSql = string.Format(@"insert into ybcfmxscindr(jzlsh,jylsh,sfxmzl,ybcfh,djlsh,cfrq,yysfxmbm,yysfxmmc,dj,sl,
						                 je,ysbm,ksbh,jbr,sflb,grbh,xm,kh,ybjzlsh,cxbz,sysdate) select 
                                         jzlsh,jylsh,sfxmzl,ybcfh,djlsh,cfrq,yysfxmbm,yysfxmmc,dj,sl,
						                 je,ysbm,ksbh,'{2}',sflb,grbh,xm,kh,ybjzlsh,0,'{3}' from ybcfmxscindr where jzlsh='{0}' and jylsh='{1}' and cxbz=1",
                                            jzlsh, cfjylsh, CliUtils.fUserName, sysdate);
            list.Add(strSql);
            strSql = string.Format(@"update ybcfmxscindr set cxbz=2 where jzlsh='{0}' and jylsh='{1}' and cxbz=1", jzlsh, cfjylsh);
            list.Add(strSql);

            strSql = string.Format(@"insert into ybcfmxscfhdr(jzlsh,jylsh,yyxmdm,ybcfh,cfh,je,zfje,zlje,zlbl,qezfbz,
                                         sfxmdj,yyxmmc,grbh,xm,kh,ybjzlsh,cxbz,sysdate ) select
                                         jzlsh,jylsh,yyxmdm,ybcfh,cfh,je,zfje,zlje,zlbl,qezfbz,
                                         sfxmdj,yyxmmc,grbh,xm,kh,ybjzlsh,0,'{2}' from ybcfmxscfhdr where jzlsh='{0}' and jylsh='{1}' and cxbz=1",
                                     jzlsh, cfjylsh, sysdate);
            list.Add(strSql);
            strSql = string.Format(@"update ybcfmxscfhdr set cxbz=2 where jzlsh='{0}' and jylsh='{1}' and cxbz=1", jzlsh, cfjylsh);
            list.Add(strSql);

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


                JYLSH = DateTime.Now.ToString("yyyyMMddHHmmss") + "-" + YLGHBH + "-" + new Random().Next(100).ToString().PadLeft(4, '0');
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


                string ksmc = string.Empty; //科室名称
                string ksbm = string.Empty; //科室编码
                string ybbh = string.Empty; //医保卡号
                string lxdh = string.Empty; //联系电话
                string cwh = "";    //床位号
                string ysbm = string.Empty;   //医生编码
                string jbr = CliUtils.fUserName; //经办人
                string hzxm = string.Empty; //患者姓名
                string ksbm2 = string.Empty; //科室二级代码


                string zlfmc = string.Empty;
                string zlfbm = string.Empty;
                float zlfje = 0;

                string ghfmc = string.Empty;
                string ghfbm = string.Empty;
                float ghfje = 0;

                if (string.IsNullOrEmpty(jzlsh))
                    return new object[] { 0, 0, "就诊流水号不能为空" };
                if (string.IsNullOrEmpty(yllb))
                    return new object[] { 0, 0, "医疗类别代码不能为空|" };
                if (string.IsNullOrEmpty(ghsj))
                    return new object[] { 0, 0, "挂号时间不能为空|" };

                string[] syllb = { "11", "13", "86" };
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



                strSql = string.Format(@"select b2ejks from bz02d where b2ksno = '{0}' and b2ejnm = '{1}' ", ksbm, ksmc);
                ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                if (ds.Tables[0].Rows.Count > 0)
                    ksbm2 = ds.Tables[0].Rows[0]["b2ejks"].ToString();

                strSql = string.Format(@"select * from ybmzzydjdr where jzlsh='{0}' and jzbz='m' and cxbz=1", jzlsh);
                ds.Tables.Clear();
                ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                if (ds.Tables[0].Rows.Count > 0)
                    return new object[] { 0, 0, "患者已进行医保门诊登记，清匆再进行重复操作" };

                string ghlb = "";
                string ybjzlsh = "";

                if (!string.IsNullOrEmpty(ysbm))
                {
                    ghlb = "专家号";
                }
                else
                {
                    ghlb = "普通号";
                }



               


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
                float je = 0;  //金额
                string fs = string.Empty;  //中药饮片副数
                string sfbz = string.Empty;      //按最小计价单位收费标志
                string by3 = "";    //事务处理类别
                string by4 = "";    //处方医院编号
                string by5 = "";    //外配处方号
                string by6 = "";    //药品电子监管码

                string fph = string.Empty; //发票号
                float blbfy = 0; //病历本费用
                float ylkfy = 0; //医疗卡费用
                float ghfy = 0;    //挂号总费用
                float ghfylj = 0;
                float ghfyvv = 0;

                int index = 1;
                List<string> liSQL = new List<string>();
                StringBuilder inputParam = new StringBuilder();

                #region 获取诊查费
                strSql = string.Format(@"select '2' as sfxmzl,a.m1ghno as cfh, a.m1date as cfrq,c.bzmem2 yysfxmbm,
                                    c.bzname yysfxmmc,c.bzmem1 as dj,1 as sl,a.m1gham je, '' as fs, 
                                    a.m1empn as ysbm,a.m1ksno as ksbm,0 as sfbz,'' as bzbm,a.m1invo,a.m1gham1,a.m1blam,a.m1kham,a.m1amnt
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
                je = Convert.ToSingle(dr["je"].ToString());  //金额
                ghfylj += je;
                fs = dr["fs"].ToString();  //中药饮片副数
                ysbm = dr["ysbm"].ToString();    //医生编码
                ksbm = dr["ksbm"].ToString();    //科室编码
                sfbz = dr["sfbz"].ToString();      //按最小计价单位收费标志
                by3 = "";    //事务处理类别
                by4 = "";    //处方医院编号
                by5 = "";    //外配处方号
                by6 = "";    //药品电子监管码

                blbfy = Convert.ToSingle(dr["m1blam"].ToString());
                ylkfy = Convert.ToSingle(dr["m1kham"].ToString());
                ghfy = Convert.ToSingle(dr["m1amnt"].ToString());
                ghfyvv = Convert.ToSingle(dr["m1gham1"].ToString());

                if (string.IsNullOrEmpty(dj))
                    dj = "0";
                if (string.IsNullOrEmpty(sl))
                    sl = "0";

                zlfbm = yyxmbm;
                zlfmc = yyxmmc;
                zlfje = je;



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
                                            a.m1empn as ysbm,a.m1ksno as ksbm,0 as sfbz,'' as bzbm,a.m1invo,a.m1blam,a.m1kham,a.m1amnt
                                            from mz01h a  
                                            left join bz05d  c on c.b5item='800000000160'     
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
                    je = Convert.ToSingle(dr["je"].ToString());  //金额
                    ghfylj += je;
                    fs = dr["fs"].ToString();  //中药饮片副数
                    //ysbm = dr["m1empn"].ToString();    //医生编码
                    //ksbm = dr["m1ksno"].ToString();    //科室编码
                    sfbz = dr["sfbz"].ToString();      //按最小计价单位收费标志
                    by3 = "";    //事务处理类别
                    by4 = "";    //处方医院编号
                    by5 = "";    //外配处方号
                    by6 = "";    //药品电子监管码

                    if (string.IsNullOrEmpty(dj))
                        dj = "0";
                    if (string.IsNullOrEmpty(sl))
                        sl = "0";

                    ghfbm = yyxmbm;
                    ghfje = je;
                    ghfmc = yyxmmc;

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
                                    '' as ysbm,'' as m1ksno, 0 as sfbz,'' as bzbm
                                     from patbh a
                                    left join bz05d b on a.pamark=b.b5item
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
                    je = Convert.ToSingle(dr["je"].ToString());  //金额
                    ghfylj += je;
                    fs = dr["fs"].ToString();  //中药饮片副数
                    //ysbm = dr["m1empn"].ToString();    //医生编码
                    //ksbm = dr["m1ksno"].ToString();    //科室编码
                    sfbz = dr["sfbz"].ToString();      //按最小计价单位收费标志
                    by3 = "";    //事务处理类别
                    by4 = "";    //处方医院编号
                    by5 = "";    //外配处方号
                    by6 = "";    //药品电子监管码

                    if (string.IsNullOrEmpty(dj))
                        dj = "0";
                    if (string.IsNullOrEmpty(sl))
                        sl = "0";

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
                                    '' as ysbm,'' as m1ksno, 0 as sfbz,'' as bzbm
                                     from patbh a
                                    left join bz05d b on a.pamark=b.b5item
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
                    je = Convert.ToSingle(dr["je"].ToString());  //金额
                    ghfylj += je;
                    fs = dr["fs"].ToString();  //中药饮片副数
                    //ysbm = dr["m1empn"].ToString();    //医生编码
                    //ksbm = dr["m1ksno"].ToString();    //科室编码
                    sfbz = dr["sfbz"].ToString();      //按最小计价单位收费标志
                    by3 = "";    //事务处理类别
                    by4 = "";    //处方医院编号
                    by5 = "";    //外配处方号
                    by6 = "";    //药品电子监管码

                    if (string.IsNullOrEmpty(dj))
                        dj = "0";
                    if (string.IsNullOrEmpty(sl))
                        sl = "0";

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

                string djh = string.Empty;
                string outputData = string.Empty;

                //#region 如果是挂号登记,则获取发票号
                //string Sql = string.Format(@"select m1invo,* from mz01h where m1ghno='{0}'", jzlsh);
                //ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                //if (ds.Tables[0].Rows.Count == 0)
                //    return new object[] { 0, 0, "该患者未挂号" };
                //djh = ds.Tables[0].Rows[0]["m1invo"].ToString();
                //#endregion


                 //获取医保就诊流水号
                object[] objjzlsh = { "M" };
                objjzlsh = GetJZLSH(objjzlsh);
                if (objjzlsh[1].ToString().Equals("1"))
                {
                    ybjzlsh = objjzlsh[2].ToString();
                }
                else
                {
                    return new object[] { 0, 0, "获取就诊流水号失败" };
                }

                object[] obj1 = { ybjzlsh, "", "M" };
                obj1 = YBDK(obj1);
                if (obj1[1].ToString().Equals("0"))
                {
                    return new object[] { 0, 0, "挂号读卡失败" };
                }

                float pcPubPay = 0;
                float pcAccPay = 0;
                float pcCashPay = 0;



                int i = FOutpatReg(ybjzlsh, ghlb, ksmc, ghfbm, ghfmc, ghfje, zlfbm, zlfmc, zlfje, "",
                  jbr, sysdate, "F", ref   pcPubPay, ref   pcAccPay, ref   pcCashPay);


                if (i == 0)
                {

                    float ylzfy = pcPubPay + pcAccPay + pcCashPay;//总医疗费
                    float zbxje = pcPubPay + pcAccPay;//总报销金额

                    outputData = ylzfy + "|" + zbxje + "|" + pcPubPay + "||" + pcAccPay + "|" + pcCashPay + "|||||||||||||" + pcPubPay + "|||||||||||||||" + xm + "|" + sysdate + "|" + yllb + "|" + yldylb + "|" + ybjzlsh + "|" + djh + "||" + djh + "||"
                        + JYLSH + "|1||" + YLGHBH + "||||" + grbh + "||||||||||";

                    WriteLog(sysdate + "  门诊登记(挂号)费用结算成功|" + outputData.ToString());

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

                     strSql = string.Format(@"insert into ybmzzydjdr(
                                        jzlsh,jylsh,yldylb,ghdjsj,bzbm,bzmc,bq,ksbh,ksmc,cwh,
                                        ysdm,jbr,dwmc,grbh,xm,xb,ybjzlsh,kh,yllb,cxbz,
                                        sysdate,tcqh,ydrybz,nl,csrq,zhye,jzbz)
                                        values(
                                        '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}',
                                        '{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}',
                                        '{20}','{21}','{22}','{23}','{24}','{25}','{26}')",
                                        jzlsh, JYLSH, yldylb, ghsj, bzbm, bzmc, "", ksbm2, ksmc, cwh,
                                        ysbm, jbr, dwmc, ybbh, xm, xb, ybjzlsh, kh, yllb, 1,
                                        sysdate, tcqh, ydrybz, nl, csrq, zhye, "m");
                    liSQL.Add(strSql);

                    strSql = string.Format(@"insert into ybfyjsdr (jzlsh,jylsh,djhin,cyrq,cyyy,bzbm,bzmc,yllb,xm,kh,
                                        grbh,ylfze,zbxje,tcjjzf,dejjzf,dbbyzf,mzjzfy,zhzf,xjzf,zhzfzf,
                                        zhzfzl,xjzhzf,xjzhzl,ybfwnfy,bcjsqzhye,dbzbc,smxx,yfhj,zlxmfhj,bbzf,
                                        bz6,jbr,zcfbz,djh,cfmxjylsh,zffy,ybjzlsh,cxbz,sysdate,jsrq) values(
                                        '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}',
                                        '{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}',
                                        '{20}','{21}','{22}','{23}','{24}','{25}','{26}','{27}','{28}','{29}',
                                        '{30}','{31}','{32}','{33}','{34}','{35}','{36}','{37}','{38}','{39}')",
                                       jzlsh, JYLSH, djh, ghsj, null, bzbm, bzmc, yllb, xm, kh,
                                       grbh, ylzfy, zbxje, pcPubPay, 0, 0, 0, pcAccPay, pcCashPay, 0,
                                       0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
                                       by6, jbr, null, djh, cflsh, pcCashPay, ybjzlsh, 1, sysdate, sysdate);

                    liSQL.Add(strSql);

                    object[] obj = liSQL.ToArray();
                    obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);

                    if (obj[1].ToString().Equals("1"))
                    {
                        WriteLog(sysdate + "  门诊登记成功|" + outputData.ToString());
                        return new object[] { 0, 1, "门诊登记成功" };
                    }
                    else
                    {
                        //门诊登记撤销
                        object[] objParam1 = { ybjzlsh, djh };
                        objParam1 = N_YBMZZYDJCX(objParam1);
                        if (objParam1[1].ToString().Equals(1))
                        {

                            WriteLog(sysdate + "  门诊登记失败|数据库操作失败|撤销登记成功" + obj[2].ToString());
                            return new object[] { 0, 0, "门诊登记失败|数据库操作失败|撤销登记成功" + obj[2].ToString() };
                        }
                        else
                        {
                            WriteLog(sysdate + "  门诊登记失败|数据库操作失败|撤销登记失败");
                            return new object[] { 0, 1, "门诊登记失败|数据库操作失败|撤销登记失败" };
                        }

                    }
                }
                else
                {

                    WriteLog(sysdate + "  门诊业务登记失败");
                    return new object[] { 0, 0, "门诊业务登记失败" };
                }




            }
            catch (SyntaxErrorException err)
            {
                WriteLog(sysdate + "  门诊业务登记失败" + err.ToString());
                return new object[] { 0, 0, "门诊业务登记失败" };
            }



        }
        #endregion

        #region 门诊登记(对应市医保门诊登记收费，为同步门诊调用)
        public static object[] YBMZDJSF(object[] objParam)
        {
            string sysdate = GetServerDateTime();
            try
            {

                string jzlsh = objParam[0].ToString();//就诊流水号
                string yllb = objParam[1].ToString();//医疗类别代码
                string sj = Convert.ToDateTime(objParam[2].ToString()).ToString("yyyyMMddHHmmss");   // 挂号时间(格式：DateTime.Now.ToString("yyyyMMddHHmmss"))
                string ghsj = Convert.ToDateTime(objParam[2].ToString()).ToString("yyyyMMddHHmmss");   // 挂号时间(格式：DateTime.Now.ToString("yyyyMMddHHmmss"))
                string bzbm = objParam[3].ToString();//病种编码
                string bzmc = objParam[4].ToString();//病种名称
                string[] kxx = objParam[5].ToString().Split('|'); //读卡返回信息
                //string dgysdm=objParam[6].ToString();   // 定岗医师代码(汤山不用)
                //string dgysxm = objParam[7].ToString();   //定岗医师名称(汤山不用)


                JYLSH = DateTime.Now.ToString("yyyyMMddHHmmss") + "-" + YLGHBH + "-" + new Random().Next(100).ToString().PadLeft(4, '0');
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


                string ksmc = string.Empty; //科室名称
                string ksbm = string.Empty; //科室编码
                string ybbh = string.Empty; //医保卡号
                string lxdh = string.Empty; //联系电话
                string cwh = "";    //床位号
                string ysbm = string.Empty;   //医生编码
                string jbr = CliUtils.fUserName; //经办人
                string hzxm = string.Empty; //患者姓名
                string ksbm2 = string.Empty;//科室二级编码

                string zlfmc = string.Empty;
                string zlfbm = string.Empty;
                float zlfje = 0;

                string ghfmc = string.Empty;
                string ghfbm = string.Empty;
                float ghfje = 0;

                if (string.IsNullOrEmpty(jzlsh))
                    return new object[] { 0, 0, "就诊流水号不能为空" };
                if (string.IsNullOrEmpty(yllb))
                    return new object[] { 0, 0, "医疗类别代码不能为空|" };
                if (string.IsNullOrEmpty(ghsj))
                    return new object[] { 0, 0, "挂号时间不能为空|" };

                string[] syllb = { "11", "13", "86" };
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

                strSql = string.Format(@"select b2ejks from bz02d where b2ksno = '{0}' and b2ejnm = '{1}' ", ksbm ,ksmc);
                ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                if (ds.Tables[0].Rows.Count > 0)
                    ksbm2 = ds.Tables[0].Rows[0]["b2ejks"].ToString();


                strSql = string.Format(@"select * from ybmzzydjdr where jzlsh='{0}' and jzbz='m' and cxbz=1", jzlsh);
                ds.Tables.Clear();
                ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                if (ds.Tables[0].Rows.Count > 0)
                    return new object[] { 0, 0, "患者已进行医保门诊登记，清匆再进行重复操作" };

                string ghlb = "";
                string ybjzlsh = "";

                if (!string.IsNullOrEmpty(ysbm))
                {
                    ghlb = "专家号";
                }
                else
                {
                    ghlb = "普通号";
                }






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
                float je = 0;  //金额
                string fs = string.Empty;  //中药饮片副数
                string sfbz = string.Empty;      //按最小计价单位收费标志
                string by3 = "";    //事务处理类别
                string by4 = "";    //处方医院编号
                string by5 = "";    //外配处方号
                string by6 = "";    //药品电子监管码

                string fph = string.Empty; //发票号
                float blbfy = 0; //病历本费用
                float ylkfy = 0; //医疗卡费用
                float ghfy = 0;    //挂号总费用
                float ghfylj = 0;
                float ghfyvv = 0;

                int index = 1;
                List<string> liSQL = new List<string>();
                StringBuilder inputParam = new StringBuilder();

                #region 获取诊查费
                strSql = string.Format(@"select '2' as sfxmzl,a.m1ghno as cfh, a.m1date as cfrq,c.bzmem2 yysfxmbm,
                                    c.bzname yysfxmmc,c.bzmem1 as dj,1 as sl,a.m1gham je, '' as fs, 
                                    a.m1empn as ysbm,a.m1ksno as ksbm,0 as sfbz,'' as bzbm,a.m1invo,a.m1gham1,a.m1blam,a.m1kham,a.m1amnt
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
                je = Convert.ToSingle(dr["je"].ToString());  //金额
                ghfylj += je;
                fs = dr["fs"].ToString();  //中药饮片副数
                ysbm = dr["ysbm"].ToString();    //医生编码
                ksbm = dr["ksbm"].ToString();    //科室编码
                sfbz = dr["sfbz"].ToString();      //按最小计价单位收费标志
                by3 = "";    //事务处理类别
                by4 = "";    //处方医院编号
                by5 = "";    //外配处方号
                by6 = "";    //药品电子监管码

                blbfy = Convert.ToSingle(dr["m1blam"].ToString());
                ylkfy = Convert.ToSingle(dr["m1kham"].ToString());
                ghfy = Convert.ToSingle(dr["m1amnt"].ToString());
                ghfyvv = Convert.ToSingle(dr["m1gham1"].ToString());

                if (string.IsNullOrEmpty(dj))
                    dj = "0";
                if (string.IsNullOrEmpty(sl))
                    sl = "0";


                zlfbm = yyxmbm;
                zlfmc = yyxmmc;
                zlfje = je;


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
                                            a.m1empn as ysbm,a.m1ksno as ksbm,0 as sfbz,'' as bzbm,a.m1invo,a.m1blam,a.m1kham,a.m1amnt
                                            from mz01h a  
                                            left join bz05d  c on c.b5item='800000000160'     
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
                    je = Convert.ToSingle(dr["je"].ToString());  //金额
                    ghfylj += je;
                    fs = dr["fs"].ToString();  //中药饮片副数
                    //ysbm = dr["m1empn"].ToString();    //医生编码
                    //ksbm = dr["m1ksno"].ToString();    //科室编码
                    sfbz = dr["sfbz"].ToString();      //按最小计价单位收费标志
                    by3 = "";    //事务处理类别
                    by4 = "";    //处方医院编号
                    by5 = "";    //外配处方号
                    by6 = "";    //药品电子监管码

                    if (string.IsNullOrEmpty(dj))
                        dj = "0";
                    if (string.IsNullOrEmpty(sl))
                        sl = "0";


                    ghfbm = yyxmbm;
                    ghfmc = yyxmmc;
                    ghfje = je;


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
                                    '' as ysbm,'' as m1ksno, 0 as sfbz,'' as bzbm
                                     from patbh a
                                    left join bz05d b on a.pamark=b.b5item
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
                    je = Convert.ToSingle(dr["je"].ToString());  //金额
                    ghfylj += je;
                    fs = dr["fs"].ToString();  //中药饮片副数
                    //ysbm = dr["m1empn"].ToString();    //医生编码
                    //ksbm = dr["m1ksno"].ToString();    //科室编码
                    sfbz = dr["sfbz"].ToString();      //按最小计价单位收费标志
                    by3 = "";    //事务处理类别
                    by4 = "";    //处方医院编号
                    by5 = "";    //外配处方号
                    by6 = "";    //药品电子监管码

                    if (string.IsNullOrEmpty(dj))
                        dj = "0";
                    if (string.IsNullOrEmpty(sl))
                        sl = "0";

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
                                    '' as ysbm,'' as m1ksno, 0 as sfbz,'' as bzbm
                                     from patbh a
                                    left join bz05d b on a.pamark=b.b5item
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
                    je = Convert.ToSingle(dr["je"].ToString());  //金额
                    ghfylj += je;
                    fs = dr["fs"].ToString();  //中药饮片副数
                    //ysbm = dr["m1empn"].ToString();    //医生编码
                    //ksbm = dr["m1ksno"].ToString();    //科室编码
                    sfbz = dr["sfbz"].ToString();      //按最小计价单位收费标志
                    by3 = "";    //事务处理类别
                    by4 = "";    //处方医院编号
                    by5 = "";    //外配处方号
                    by6 = "";    //药品电子监管码

                    if (string.IsNullOrEmpty(dj))
                        dj = "0";
                    if (string.IsNullOrEmpty(sl))
                        sl = "0";

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

                string djh = string.Empty;
                string outputData = string.Empty;

                #region 如果是挂号登记,则获取发票号
                string Sql = string.Format(@"select m1invo,* from mz01h where m1ghno='{0}'", jzlsh);
                ds = CliUtils.ExecuteSql("sybdj", "cmd", Sql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                if (ds.Tables[0].Rows.Count == 0)
                    return new object[] { 0, 0, "该患者未挂号" };
                djh = ds.Tables[0].Rows[0]["m1invo"].ToString();
                #endregion


                //获取医保就诊流水号
                object[] objjzlsh = { "M" };
                objjzlsh = GetJZLSH(objjzlsh);
                if (objjzlsh[1].ToString().Equals("1"))
                {
                    ybjzlsh = objjzlsh[2].ToString();
                }
                else
                {
                    return new object[] { 0, 0, "获取就诊流水号失败" };
                }

                object[] obj1 = { ybjzlsh, "", "M" };
                obj1 = YBDK(obj1);
                if (obj1[1].ToString().Equals("0"))
                {
                    return new object[] { 0, 0, "挂号读卡失败" };
                }

                float pcPubPay = 0;
                float pcAccPay = 0;
                float pcCashPay = 0;



                int i = FOutpatReg(ybjzlsh, ghlb, ksmc, ghfbm, ghfmc, ghfje, zlfbm, zlfmc, zlfje, "",
                  jbr, sysdate, "F", ref   pcPubPay, ref   pcAccPay, ref   pcCashPay);


                if (i == 0)
                {

                    float ylzfy = pcPubPay + pcAccPay + pcCashPay;//总医疗费
                    float zbxje = pcPubPay + pcAccPay;//总报销金额

                    outputData = ylzfy + "|" + zbxje + "|" + pcPubPay + "||" + pcAccPay + "|" + pcCashPay + "|||||||||||||" + pcPubPay + "|||||||||||||||" + xm + "|" + sysdate + "|" + yllb + "|" + yldylb + "|2057|" + ybjzlsh + "|" + djh + "||" + djh + "||"
                        + JYLSH + "|1||" + YLGHBH + "||||" + grbh + "||||||||||";

                    WriteLog(sysdate + "  门诊登记(挂号)费用结算成功|" + outputData.ToString());

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



                    strSql = string.Format(@"insert into ybmzzydjdr(
                                        jzlsh,jylsh,yldylb,ghdjsj,bzbm,bzmc,bq,ksbh,ksmc,cwh,
                                        ysdm,jbr,dwmc,grbh,xm,xb,ybjzlsh,kh,yllb,cxbz,
                                        sysdate,tcqh,ydrybz,nl,csrq,zhye,jzbz)
                                        values(
                                        '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}',
                                        '{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}',
                                        '{20}','{21}','{22}','{23}','{24}','{25}','{26}')",
                                       jzlsh, JYLSH, yldylb, sj, bzbm, bzmc, "", ksbm2, ksmc, cwh,
                                       ysbm, jbr, dwmc, ybbh, xm, xb, ybjzlsh, kh, yllb, 1,
                                       sysdate, tcqh, ydrybz, nl, csrq, zhye, "m");
                    liSQL.Add(strSql);

                    strSql = string.Format(@"insert into ybfyjsdr (jzlsh,jylsh,djhin,cyrq,cyyy,bzbm,bzmc,yllb,xm,kh,
                                        grbh,ylfze,zbxje,tcjjzf,dejjzf,dbbyzf,mzjzfy,zhzf,xjzf,zhzfzf,
                                        zhzfzl,xjzhzf,xjzhzl,ybfwnfy,bcjsqzhye,dbzbc,smxx,yfhj,zlxmfhj,bbzf,
                                        bz6,jbr,zcfbz,djh,cfmxjylsh,zffy,ybjzlsh,cxbz,sysdate,jsrq) values(
                                        '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}',
                                        '{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}',
                                        '{20}','{21}','{22}','{23}','{24}','{25}','{26}','{27}','{28}','{29}',
                                        '{30}','{31}','{32}','{33}','{34}','{35}','{36}','{37}','{38}','{39}')",
                                        jzlsh, JYLSH, djh, sj, null, bzbm, bzmc, yllb, xm, kh,
                                        grbh, ylzfy, zbxje, pcPubPay, 0, 0, 0, pcAccPay, pcCashPay, 0,
                                        0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                                        by6, CliUtils.fLoginUser, null, djh, cflsh, pcCashPay, ybjzlsh, 1, sysdate, sysdate);

                    liSQL.Add(strSql);

                    object[] obj = liSQL.ToArray();
                    obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);

                    if (obj[1].ToString().Equals("1"))
                    {
                        WriteLog(sysdate + "  门诊登记成功|" + outputData.ToString());
                        return new object[] { 0, 1, outputData };
                    }
                    else
                    {
                        //门诊登记撤销
                        object[] objParam1 = { ybjzlsh, djh };
                        objParam1 = N_YBMZZYDJCX(objParam1);
                        if (objParam1[1].ToString().Equals(1))
                        {

                            WriteLog(sysdate + "  门诊登记失败|数据库操作失败|撤销登记成功" + obj[2].ToString());
                            return new object[] { 0, 0, "门诊登记失败|数据库操作失败|撤销登记成功" + obj[2].ToString() };
                        }
                        else
                        {
                            WriteLog(sysdate + "  门诊登记失败|数据库操作失败|撤销登记失败");
                            return new object[] { 0, 1, "门诊登记失败|数据库操作失败|撤销登记失败" };
                        }

                    }
                }
                else
                {

                    WriteLog(sysdate + "  门诊业务登记失败");
                    return new object[] { 0, 0, "门诊业务登记失败" };
                }




            }
            catch (SyntaxErrorException err)
            {
                WriteLog(sysdate + "  门诊业务登记失败" + err.ToString());
                return new object[] { 0, 0, "门诊业务登记失败" };
            }



        }
        #endregion

        #region 门诊登记撤销
        public static object[] YBMZDJSFCX(object[] objParam)
        {
            string sysdate = GetServerDateTime();
            string jzlsh = objParam[0].ToString();
            string djh = string.Empty;
            string ybjzlsh = "";

            #region 判断是否进行门诊登记
            string strSql = string.Format(@"select * from ybmzzydjdr where cxbz=1 and jzlsh='{0}' and jzbz='m'", jzlsh);
            DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            if (ds.Tables[0].Rows.Count == 0)
                return new object[] { 0, 0, "该患者未医保门诊登记" };
            ybjzlsh = ds.Tables[0].Rows[0]["ybjzlsh"].ToString();
            #endregion


            strSql = string.Format(@"select * from ybfyjsdr where jzlsh='{0}' and cxbz=1", jzlsh);
            ds.Tables[0].Clear();
            ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            djh = ds.Tables[0].Rows[0]["djhin"].ToString();


            object[] obj1 = { ybjzlsh, "", "M" };
            obj1 = YBDK(obj1);
            if (obj1[1].ToString().Equals("0"))
            {
                return new object[] { 0, 0, "挂号撤销读卡失败" };
            }

            //取消挂号
            int i = FCancleOutpatReg(ybjzlsh, CliUtils.fLoginUser);

            if (i == 0)
            {
                object[] obj = { jzlsh, djh };

                YBMZJSCX_N(obj);
                YBMZSCCX_N(obj);
                YBMZJSCX_N(obj);


                obj = list.ToArray();
                obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);
                list.Clear();

                if (obj[1].ToString().Equals("1"))
                {
                    WriteLog(sysdate + "  门诊挂号费用登记撤销成功|");
                    return new object[] { 0, 1, "门诊挂号费用登记撤销成功" };
                }
                else
                {
                    WriteLog(sysdate + "  门诊挂号费用登记撤销失败|数据操作失败|" + obj[2].ToString());
                    return new object[] { 0, 0, "门诊挂号费用登记撤销失败|数据操作失败|" + obj[2].ToString() };
                }

            }
            else
            {
                WriteLog(sysdate + "  门诊业务挂号撤销失败");
                return new object[] { 0, 0, "  门诊业务挂号撤销失败" };

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

            float sfje = 000; //收费金额
            #region 金额有效性
            try
            {
                sfje = Convert.ToSingle(ylhjfy);
            }
            catch
            {
                return new object[] { 0, 0, "收费金额格式错误" };
            }
            #endregion

            JYLSH = DateTime.Now.ToString("yyyyMMddHHmmss") + "-" + YLGHBH + "-" + new Random().Next(100).ToString().PadLeft(4, '0');
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
            strSql = string.Format(@"select m.cfh,m.cflsh,m.sfxmzl,m.yyxmbh, m.yyxmmc,m.dj, sum(m.sl) sl, sum(m.je) je, m.fs,m.ysdm ysdm, n.b1name ysxm, m.ksno ksno, m.unit unit ,m.mcypgg mcypgg,m.mcusex mcusex,m.mcwayx mcwayx,m.mcyphz mcyphz,m.mcdate mcdate,
                                    o.b2ejnm zxks, m.sfbz from (
                                    --药品
                                    select a.mccfno cfh,a.mcseq2 cflsh,a.mcypno yyxmbh, a.mcypnm yyxmmc, a.mcpric dj, a.mcquty sl, a.mcamnt je, a.mcksno ksno, a.mcuser ysdm,  a.mcunit unit, a.mcypgg mcypgg,a.mcusex mcusex, a.mcwayx mcwayx, a.mcyphz mcyphz, a.mcdate mcdate,
                                    case when mcsflb='03' then cast((a.mcquty/a.mcusex) as int) else 0 end as fs,'0' as sfbz,
                                    case when left(mccfno,1)='W' then '3' else '1' end as sfxmzl
                                    from mzcfd a where a.mcghno = '{0}' and a.mccfno in ({1})
                                    union all
                                    --检查/治疗 
                                    select b.mbzlno cfh,b.mbsequ cflsh, b.mbitem yyxmbh, b.mbname yyxmmc, b.mbpric dj, b.mbquty sl, b.mbsjam je, b.mbksno ksno, b.mbuser ysdm,null as unit,null as mcypgg, null as mcusex, null as mcwayx, null as mcyphz,mbdate as mcdate, 0 as fs,'0' as sfbz,
                                    case  when b.mbsfno in(select b5sfno from bz05h where b5mzno='19') then '3' else '2' end as sfxmzl
                                    from mzb2d b where b.mbghno = '{0}' and b.mbzlno in ({1})
                                    union all
                                    --检验
                                    select c.mbzlno cfh,c.mbsequ cflsh, c.mbitem yyxmbh, c.mbname yyxmmc, c.mbpric dj, c.mbquty sl, c.mbsjam je, c.mbksno ksno, c.mbuser ysdm,null as unit,null as mcypgg, null as mcusex, null as mcwayx, null as mcyphz,mbdate as mcdate,  0 as fs,'0' as sfbz,'2' as sfxmzl
                                    from mzb4d c where c.mbghno = '{0}' and c.mbzlno in ({1})
                                    union all
                                    --注射
                                    select d.mdzsno as cfh,'' cflsh,y.b5item yyxmbh, y.b5name yyxmmc, d.mdtiwe dj, d.mddays sl, (d.mdtiwe*d.mddays) je, d.mdzsks ksno, d.mdempn ysdm,null as unit,null as mcypgg, null as mcusex, null as mcwayx, null as mcyphz,mddate as mcdate, 0 as fs,'0' as sfbz,'2' as sfxmzl
                                    from mzd3d d 
                                    left join (select distinct mccfno, mcghno from mzcfd) mzcf on mccfno = mdcfno
                                    left join bz09d on b9mbno = mdtwid 
                                    left join bz05d y on b5item = b9item 
                                    where mdtiwe > 0 and d.mdzsno in ({1})
                                    union all
                                    select d.mdzsno as cfh,'' cflsh,y.b5item yyxmbh, y.b5name yyxmmc, d.mdpric dj, d.mdtims sl, (d.mdpric*d.mdtims) je, d.mdzsks ksno, d.mdempn ysdm,null as unit,null as mcypgg, null as mcusex, null as mcwayx, null as mcyphz,mddate as mcdate, 0 as fs,'0' as sfbz,'2' as sfxmzl
                                    from mzd3d d 
                                    left join bz09d on b9mbno = mdwayid 
                                    left join bz05d y on b5item = b9item
                                    left join (select distinct mccfno, mcghno from mzcfd) mzcf on mccfno = mdcfno 
                                    where mdzsno in ({1})
                                    union all
                                    select d.mdzsno as cfh,'' cflsh,y.b5item yyxmbh, y.b5name yyxmmc, d.mdppri dj, d.mdpqty sl, (d.mdppri*d.mdpqty) je, d.mdzsks ksno, d.mdempn ysdm,null as unit,null as mcypgg,null as mcusex, null as mcwayx, null as mcyphz,mddate as mcdate,  0 as fs,'0' as sfbz,'2' as sfxmzl
                                    from mzd3d d 
                                    left join bz09d on b9mbno = mdpprid 
                                    left join bz05d y on b5item = b9item
                                    left join (select distinct mccfno, mcghno from mzcfd) mzcf on mccfno = mdcfno
                                    where mdpqty > 0 and d.mdzsno in ({1})
                                    ) m left join bz01h n on m.ysdm = n.b1empn 
                                    left join bz02d o on m.ksno = o.b2ejks
                                    group by  m.dj, m.yyxmbh, m.yyxmmc, m.ysdm, n.b1name, m.ksno, o.b2ejnm, m.cfh,m.cflsh,m.sfxmzl,m.fs,m.sfbz,m.unit,m.mcypgg,m.mcusex,m.mcwayx,m.mcyphz,m.mcdate", jzlsh, cfhs);
            ds.Tables.Clear();
            ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            if (ds.Tables[0].Rows.Count == 0)
                return new object[] { 0, 0, "无处方信息" };
            float scfy = 0;
            int index = 0;
            StringBuilder inputParam = new StringBuilder();
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
                string fs = "";    //中药饮片副数
                if (!dr["fs"].ToString().Equals("0"))
                    fs = dr["fs"].ToString();
                string ysbm = dr["ysdm"].ToString();    //医生编码
                string ysxm = dr["ysxm"].ToString();    //医生姓名
                string ksbm = dr["ksno"].ToString();    //科室编码
                string ksmc = dr["zxks"].ToString();    //科室名称
                string sfbz = dr["sfbz"].ToString();    //按最小计价单位收费标志
                string unit = dr["unit"].ToString();    //单位
                string gg = dr["mcypgg"].ToString();    //规格
                string pcDosage = dr["mcusex"].ToString();    //每次用量
                string psFrequency = dr["mcyphz"].ToString();    //使用频次
                string psUsage = dr["mcwayx"].ToString();    //用法
                string psRecDate = dr["mcdate"].ToString(); //开立时间
                bzbm = "";//病种编码
                string by3 = "";    //事务处理类别
                string by4 = "";    //处方医院编号
                string by5 = "";    //外配处方号
                string by6 = "";    //备用6

                if (string.IsNullOrEmpty(unit))
                    unit = "次";
                
                pcDosage = "0";

                scfy += Convert.ToSingle(je);

                string psItmFlag = "";
                string psOTCCode = "";
                //判断是否为药品/收费项目（无非处方药）
                if (sfxmzl.ToString().Equals("1"))
                {
                    psItmFlag = "1";
                    psOTCCode = "1";
                }
                else
                {
                    psItmFlag = "0";
                    psOTCCode = "2";
                }

                float pcRate = 0;
                float pcSelfFee = 0;
                float pcDeduct = 0;



                object[] obj1 = { ybjzlsh, "", "M" };
                obj1 = YBDK(obj1);
                if (obj1[1].ToString().Equals("0"))
                {
                    return new object[] { 0, 0, "门诊预结算读卡失败" };
                }
                psFrequency = "";

                int i = FWriteFeeDetail(0/*门诊*/, ybjzlsh, psItmFlag, yyxmbm, cfh, yyxmmc, unit, gg, "", psOTCCode,
             Convert.ToSingle(sl),   Convert.ToSingle(dj),   Convert.ToSingle(dj),  Convert.ToSingle(pcDosage),   psFrequency,   psUsage,
              0, CliUtils.fLoginUser, ksbm, ysbm, psRecDate, ref   pcRate, ref   pcSelfFee, ref   pcDeduct);

                if (i == 0)
                {
                    strSql = string.Format(@"insert into ybcfmxscindr(jzlsh,jylsh,sfxmzl,ybcfh,djlsh,cfrq,yysfxmbm,yysfxmmc,dj,sl,
						                   je,ysbm,ksbh,jbr,sflb,grbh,xm,kh,cxbz,sysdate,ybjzlsh) values(
                                           '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}',
                                           '{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}','{20}')",
                                         jzlsh, JYLSH, sfxmzl, cfh, cflsh, kbrq1, yyxmbm, yyxmmc, dj, sl,
                                         je, ysbm, ksbm, jbr, sfbz, grbh, xm, kh, 1, sysdate, ybjzlsh);
                    liSQL.Add(strSql);

                    strSql = string.Format(@"insert into ybcfmxscfhdr(jzlsh,jylsh,yyxmdm,ybcfh,cfh,je,zfje,zlje,zlbl,qezfbz,
                                                sfxmdj,yyxmmc,grbh,xm,kh,cxbz,sysdate,ybjzlsh ) values(
                                                '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}',
                                                '{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}')",
                                            jzlsh, JYLSH, yyxmbm, cfh, cflsh, je, pcSelfFee, pcDeduct, pcRate, null,
                                            null, null, grbh, xm, kh, 1, sysdate, ybjzlsh);
                    liSQL.Add(strSql);
                }
                else
                {
                    //门诊费用医保撤销
                    object[] objParam1 = { ybjzlsh };
                    objParam1 = N_YBFYDJCX(objParam1);
                    WriteLog(sysdate + "  门诊费用登记失败|" +ybjzlsh);
                    return new object[] { 0, 0, "门诊费用登记失败" + ybjzlsh };
                }

            }
            if (Math.Abs(scfy - sfje) > 1.0)
                return new object[] { 0, 0, "收费金额与医保结算金额相差" + Math.Abs(scfy - sfje) + ",无法结算，请核实!" };
            #endregion

            WriteLog(sysdate + "  全部上传成功");

            object[] obj = liSQL.ToArray();
            obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);
            if (obj[1].ToString().Equals("1"))
            {
                WriteLog(sysdate + "  门诊费用上传成功|");
                return new object[] { 0, 1, JYLSH };
            }
            else
            {
                WriteLog(sysdate + "  门诊费用登记失败|数据操作失败|" + obj[2].ToString());
                //门诊费用医保撤销
                object[] objParam1 = { ybjzlsh };
                objParam1 = N_YBFYDJCX(objParam1);
                return new object[] { 0, 0, "门诊费用登记失败|数据操作失败|" + obj[2].ToString() };
            }
        }
        #endregion

        #region 门诊费用登记撤销(门诊处方明细上传撤销)
        public static object[] YBMZFYDJCX(object[] objParam)
        {
            string sysdate = GetServerDateTime();
            string jzlsh = objParam[0].ToString();

            if (string.IsNullOrEmpty(jzlsh))
                return new object[] { 0, 0, "就诊流水号不能为空" };

            JYLSH = DateTime.Now.ToString("yyyyMMddHHmmss") + "-" + YLGHBH + "-" + new Random().Next(100).ToString().PadLeft(4, '0');
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
                string cfh = "";
                string cflsh = "";

                strSql = string.Format(@"select * from ybmzzydjdr where jzlsh='{0}' and jzbz='m' and cxbz=1", jzlsh);
                ds.Tables.Clear();
                ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                string ybjzlsh = ds.Tables[0].Rows[0]["ybjzlsh"].ToString();

                int i = FSynData(2, ybjzlsh);

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
                                         sfxmdj,yyxmmc,grbh,xm,kh,ybjzlsh,cxbz,sysdate) select
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
                        WriteLog(sysdate + "  门诊费用登记撤销成功|");
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
                    WriteLog(sysdate + "  门诊费用登记撤销失败|");
                    return new object[] { 0, 0, "门诊费用登记撤销失败|数据操作失败|" };
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
            string dqrq = objParam[2].ToString(); //结算时间
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
                string ydrybz = ds.Tables[0].Rows[0]["ydrybz"].ToString();

                //获取处方医生
                strSql = string.Format(@"select distinct m3empn from mz03d where m3ghno='{0}' and m3cfno in ({1})", jzlsh, cfhs);
                ds.Tables.Clear();
                ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);

                if (ds.Tables[0].Rows.Count == 0)
                    return new object[] { 0, 0, "未获取到处方医生信息,不能进行预结算操作" };
                ysbm = ds.Tables[0].Rows[0]["m3empn"].ToString();  //医生编码

                   
                #region 费用上传前，调用取消门诊预结算
                object[] objQXYJS = { jzlsh ,ybjzlsh};
                //费用登记前，撤销费用登记
                objQXYJS = MZYJSCX(objQXYJS);
                if (!objQXYJS[1].ToString().Equals("1"))
                    return new object[] { 0, 0, objQXYJS[2].ToString() };
                #endregion

                //#region 费用上传前，撤销所有未结算费用
                //object[] objGHFYDJ = { jzlsh };
                ////费用登记前，撤销费用登记
                //objGHFYDJ = YBMZFYDJCX(objGHFYDJ);
                //if (!objGHFYDJ[1].ToString().Equals("1"))
                //    return new object[] { 0, 0, objGHFYDJ[2].ToString() };
                //#endregion

                #region 费用上传
                //门诊费用登记
                object[] objMZFYDJ = { jzlsh, cfhs, ylfhj1, "", dqrq };
                objMZFYDJ = YBMZFYDJ(objMZFYDJ);
                if (!objMZFYDJ[1].ToString().Equals("1"))
                    return new object[] { 0, 0, objMZFYDJ[2].ToString() };
                string cfjylsh = objMZFYDJ[2].ToString();
                #endregion

                JYLSH = DateTime.Now.ToString("yyyyMMddHHmmss") + "-" + YLGHBH + "-" + new Random().Next(100).ToString().PadLeft(4, '0');

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

                string jsrq = Convert.ToDateTime(sysdate).ToString("yyyyMMddHHmmss");   //结算日期
                string cyrq = Convert.ToDateTime(dqrq).ToString("yyyyMMddHHmmss");  //出院日期

                string psMedMode = "1";  //医疗方式  1普通门诊；2普通住院；3特殊门诊；4紧急抢救；5急诊；
                string psICDMode = "A";
                string psICD = "";//报销代码：OA01门诊报销/OB01门诊异地报销/OC01门诊转院报销/IA01住院报销/IB01住院异地报销/IC01住院转院报销

                if(ydrybz.ToString().Equals("1"))
                {
                    psICD = "OB01";//门诊异地报销
                }
                else
                {
                    psICD = "OA01";//门诊本地报销
                }

                if(yllb != "11")
                {
                    yllb = "12";
                    psMedMode = "3";
                }

                float pcOther1 = 0;
                float pcOther2 = 0;
                string psMemo = "";

                #region 业务出参
                StringBuilder psBillCode = new StringBuilder(32);
                float ylzfy = 0;  //医疗总费用
                float sgfwnfy = 0;  //三个范围内费用
                float zfufy = 0;   //自付费用
                float zfefy = 0;   //自费费用
                float qfbz = 0;    //起付标准
                float tcjjzf = 0;  //统筹支付
                float tcjjgrzf = 0;    //统筹个人自负
                float dejjzf = 0;    //大病救助基金支付
                float dejjgrzf = 0;     //大病救助基金个人自负
                float gwybzjjzf = 0;  //公务员/企业补充支付
                float gwybzjjgrzf = 0;     //公务员/企业个人自负
                float qtjjzf = 0;     //其它基金支付
                float zhzf = 0;    //医疗账户支付
                float cxzhzf = 0;   //储蓄账户支付
                float xjzf = 0;    //现金支付
                #endregion

                
                object[] obj1 = { ybjzlsh, "", "M" };
                obj1 = YBDK(obj1);
                if (obj1[1].ToString().Equals("0"))
                {
                    return new object[] { 0, 0, "门诊预结算读卡失败" };
                }



                int i = FTryOutpatBalance(ybjzlsh, CliUtils.fLoginUser, "是", ksbm, ysbm, psMedMode, yllb, psICDMode, psICD, pcOther1, pcOther2,
              psMemo, psBillCode, ref   ylzfy, ref   sgfwnfy, ref   zfufy, ref   zfefy, ref   qfbz, ref    tcjjzf, ref   tcjjgrzf, ref   dejjzf, ref   dejjgrzf,
            ref   gwybzjjzf, ref   gwybzjjgrzf, ref   qtjjzf, ref   zhzf, ref   cxzhzf, ref   xjzf);


                if (i == 0)
                {
                    #region 出参赋值
                    float zbxje = Convert.ToSingle(0);              
                    float dbbyzf = Convert.ToSingle(0);             
                    float mzfdfy = Convert.ToSingle(0);
                    float zhzfzf = zfufy;                              
                    float zhzfzl = Convert.ToSingle(0);
                    float xjzfzf = zfefy;
                    float xjzfzl = Convert.ToSingle(0);
                    float ybfwnfy = Convert.ToSingle(0);
                    float bcjsqzhye = Convert.ToSingle(0);
                    string dbzbm = "";        //单病种病种编码
                    string smxx = "";        //说明信息
                    float yfhj = Convert.ToSingle(0);       //药费合计
                    float zlxmfhj = Convert.ToSingle(0);       //诊疗项目费合计
                    float bbzf = Convert.ToSingle(0);       //补保支付
                    string yllb_r = "";    //医疗类别
                    string by6 = "";

                    float qybcylbxjjzf = Convert.ToSingle(0); //企业补充医疗保险基金支付
                    float dwfdfy = Convert.ToSingle(0);        //单位负担费用    
                    float yyfdfy = Convert.ToSingle(0);       //医院负担费用
                    //float mzfdfy = Convert.ToSingle("0");   //民政负担费用
                    float cxjfy = Convert.ToSingle(0);        //超限价费用单病种病种编码
                    float ylzlfy = Convert.ToSingle(0);       //乙类自理费用
                    float blzlfy = Convert.ToSingle(0);       //丙类自理费用
                    float fhjbylfy = Convert.ToSingle(0);     //符合基本医疗费用
                    float qfbzfy = qfbz;       //起付标准费用
                    float zzzyzffy = Convert.ToSingle(0);     //转诊转院自付费用
                    float jrtcfy = Convert.ToSingle(0);       //进入统筹费用
                    float tcfdzffy = Convert.ToSingle(0);     //统筹分段自付费用
                    float ctcfdxfy = Convert.ToSingle(0);       //超统筹封顶线费用
                    float jrdebsfy = Convert.ToSingle(0);       //进入大额报销费用
                    float defdzffy = Convert.ToSingle(0);       //大额分段自付费用
                    float cdefdxfy = Convert.ToSingle(0);       //超大额封顶线费用
                    float rgqgzffy = Convert.ToSingle(0);       //人工器管自付费用
                    //float bcjsqzfye = Convert.ToSingle(0);    //本次结算前帐户余额
                    float bntczflj = Convert.ToSingle(0);       //本年统筹支付累计
                    float bndezflj = Convert.ToSingle(0);       //本年大额支付累计
                    float bnczjmmztczflj = Convert.ToSingle(0); //本年城镇居民门诊统筹支付累计
                    float bngwybzzflj = Convert.ToSingle(0);  //本年公务员补助支付累计(不含本次)
                    float bnzhzflj = Convert.ToSingle(0);  //本年账户支付累计(不含本次)
                    string bnzycslj = "1";  //本年住院次数累计(不含本次)
                    string zycs = "1";  //住院次数
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
                         * 医保类型
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
                                      xjzf + "|" + gwybzjjzf + "|" + qybcylbxjjzf + "|" + "0" + "|" + dwfdfy + "|" +
                                      yyfdfy + "|" + mzfdfy + "|" + cxjfy + "|" + ylzlfy + "|" + blzlfy + "|" +
                                      fhjbylfy + "|" + qfbzfy + "|" + zzzyzffy + "|" + jrtcfy + "|" + tcfdzffy + "|" +
                                      ctcfdxfy + "|" + "0" + "|" + defdzffy + "|" + cdefdxfy + "|" + rgqgzffy + "|" +
                                      bcjsqzhye + "|" + bntczflj + "|" + bndezflj + "|" + bnczjmmztczflj + "|" + bngwybzzflj + "|" +
                                      bnzhzflj + "|" + bnzycslj + "|" + zycs + "|" + xm + "|" + jsrq + "|" +
                                      yllb_r + "||" + YLGHBH + "|" + ybjzlsh + "|" + djh + "|" +
                                      "|" + djh + "||" + JYLSH + "|1|" +
                                      "|" + YLGHBH + "|0|||" +
                                      grbh + "|0|0|0|0|" +
                                      "0|0|0|" + dbbyzf + "|" + ybfwnfy + "|" +
                                      yblx;
                    WriteLog(sysdate + "  门诊费用预结算返回参数|" + strValue);
                    return new object[] { 0, 1, strValue };
                }
                else
                {
                    WriteLog(sysdate + "  门诊费用预结算失败");
                    //撤销上传费用
                    object[] objMZFYDJCX = { jzlsh };
                    YBMZFYDJCX(objMZFYDJCX);
                    return new object[] { 0, 0, "  门诊费用预结算失败" };
                }
            }
            catch (Exception ex)
            {
                WriteLog(sysdate + "  门诊费用预结算异常|" + ex.Message);
                return new object[] { 0, 0, "门诊费用预结算异常|" + ex.Message };
            }
        }
        #endregion

        #region 门诊费用预结算撤销(用于费用上传前取消上传的费用)
        public static object[] MZYJSCX(object[] objParam)
        {
            string jzlsh = objParam[0].ToString();
            string ybjzlsh = objParam[1].ToString();



            int i = FCancelTryOutpatBalance(ybjzlsh, CliUtils.fLoginUser);


                if (i == 0)
                {
                    return new object[] { 0, 1, "撤销预结算成功" };
                }
                else
                {
                    return new object[] { 0, 0, "  门诊费用预结算失败" };
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
                string ydrybz = ds.Tables[0].Rows[0]["ydrybz"].ToString(); //异地标识
 
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


                string psMedMode = "1";  //医疗方式  1普通门诊；2普通住院；3特殊门诊；4紧急抢救；5急诊；
                string psICDMode = "A";
                string psICD = "OA01";//报销代码：OA01门诊报销/OB01门诊异地报销/OC01门诊转院报销/IA01住院报销/IB01住院异地报销/IC01住院转院报销
                float pcOther1 = 0;
                float pcOther2 = 0;
                string psMemo = "";

                if(ydrybz.ToString().Equals("1"))
                {
                    psICD = "OB01";//门诊异地报销
                }

                if(yllb != "11")
                {
                    yllb = "12";    //特殊门诊
                    psMedMode = "3";//特殊门诊
                }

                #region 业务出参
                StringBuilder psBillCode = new StringBuilder(32);
                float ylzfy = 0;  //医疗总费用
                float sgfwnfy = 0;  //三个范围内费用
                float zfufy = 0;   //自付费用
                float zfefy = 0;   //自费费用
                float qfbz = 0;    //起付标准
                float tcjjzf = 0;  //统筹支付
                float tcjjgrzf = 0;    //统筹个人自负
                float dejjzf = 0;    //大病救助基金支付
                float dejjgrzf = 0;     //大病救助基金个人自负
                float gwybzjjzf = 0;  //公务员/企业补充支付
                float gwybzjjgrzf = 0;     //公务员/企业个人自负
                float qtjjzf = 0;     //其它基金支付
                float zhzf = 0;    //医疗账户支付
                float cxzhzf = 0;   //储蓄账户支付
                float xjzf = 0;    //现金支付
                #endregion



                object[] obj1 = { ybjzlsh, "", "M" };
                obj1 = YBDK(obj1);
                if (obj1[1].ToString().Equals("0"))
                {
                    return new object[] { 0, 0, "门诊结算读卡失败" };
                }


                int i = FOutpatBalance(ybjzlsh, CliUtils.fLoginUser, "是", ksbm, ysbm, psMedMode, yllb, psICDMode, psICD, pcOther1, pcOther2,
              psMemo,psBillCode, ref   ylzfy, ref   sgfwnfy, ref   zfufy, ref   zfefy, ref   qfbz, ref    tcjjzf, ref   tcjjgrzf, ref   dejjzf, ref   dejjgrzf,
            ref   gwybzjjzf, ref   gwybzjjgrzf, ref   qtjjzf, ref   zhzf, ref   cxzhzf, ref   xjzf);


                if (i == 0)
                {
                    #region 出参赋值
                    float zbxje = Convert.ToSingle(0);
                    float dbbyzf = Convert.ToSingle(0);
                    float mzfdfy = Convert.ToSingle(0);
                    float zhzfzf = zfufy;
                    float zhzfzl = Convert.ToSingle(0);
                    float xjzfzf = zfefy;
                    float xjzfzl = Convert.ToSingle(0);
                    float ybfwnfy = Convert.ToSingle(0);
                    float bcjsqzhye = Convert.ToSingle(0);
                    string dbzbm = "";        //单病种病种编码
                    string smxx = "";        //说明信息
                    float yfhj = Convert.ToSingle(0);       //药费合计
                    float zlxmfhj = Convert.ToSingle(0);       //诊疗项目费合计
                    float bbzf = Convert.ToSingle(0);       //补保支付
                    string yllb_r = "";    //医疗类别
                    string by6 = "";

                    float qybcylbxjjzf = Convert.ToSingle(0); //企业补充医疗保险基金支付
                    float dwfdfy = Convert.ToSingle(0);        //单位负担费用    
                    float yyfdfy = Convert.ToSingle(0);       //医院负担费用
                    //float mzfdfy = Convert.ToSingle("0");   //民政负担费用
                    float cxjfy = Convert.ToSingle(0);        //超限价费用单病种病种编码
                    float ylzlfy = Convert.ToSingle(0);       //乙类自理费用
                    float blzlfy = Convert.ToSingle(0);       //丙类自理费用
                    float fhjbylfy = Convert.ToSingle(0);     //符合基本医疗费用
                    float qfbzfy = qfbz;       //起付标准费用
                    float zzzyzffy = Convert.ToSingle(0);     //转诊转院自付费用
                    float jrtcfy = Convert.ToSingle(0);       //进入统筹费用
                    float tcfdzffy = Convert.ToSingle(0);     //统筹分段自付费用
                    float ctcfdxfy = Convert.ToSingle(0);       //超统筹封顶线费用
                    float jrdebsfy = Convert.ToSingle(0);       //进入大额报销费用
                    float defdzffy = Convert.ToSingle(0);       //大额分段自付费用
                    float cdefdxfy = Convert.ToSingle(0);       //超大额封顶线费用
                    float rgqgzffy = Convert.ToSingle(0);       //人工器管自付费用
                    //float bcjsqzfye = Convert.ToSingle(0);    //本次结算前帐户余额
                    float bntczflj = Convert.ToSingle(0);       //本年统筹支付累计
                    float bndezflj = Convert.ToSingle(0);       //本年大额支付累计
                    float bnczjmmztczflj = Convert.ToSingle(0); //本年城镇居民门诊统筹支付累计
                    float bngwybzzflj = Convert.ToSingle(0);  //本年公务员补助支付累计(不含本次)
                    float bnzhzflj = Convert.ToSingle(0);  //本年账户支付累计(不含本次)
                    string bnzycslj = "1";  //本年住院次数累计(不含本次)
                    string zycs = "1";  //住院次数
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
                     * 医保类型
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

                    //本次医疗费总额=本次统筹支付金额+本次大病救助支付+本次大病保险支付+本次民政补助支付+本次帐户支付总额+本次现金支付总额+本次补保支付总额
                    string strValue = ylzfy + "|" + zbxje + "|" + tcjjzf + "|" + dejjzf + "|" + zhzf + "|" +
                      xjzf + "|" + gwybzjjzf + "|" + qybcylbxjjzf + "|" + "0" + "|" + dwfdfy + "|" +
                      yyfdfy + "|" + mzfdfy + "|" + cxjfy + "|" + ylzlfy + "|" + blzlfy + "|" +
                      fhjbylfy + "|" + qfbzfy + "|" + zzzyzffy + "|" + jrtcfy + "|" + tcfdzffy + "|" +
                      ctcfdxfy + "|" + "0" + "|" + defdzffy + "|" + cdefdxfy + "|" + rgqgzffy + "|" +
                      bcjsqzhye + "|" + bntczflj + "|" + bndezflj + "|" + bnczjmmztczflj + "|" + bngwybzzflj + "|" +
                      bnzhzflj + "|" + bnzycslj + "|" + zycs + "|" + xm + "|" + jsrq + "|" +
                      yllb_r + "||" + YLGHBH + "|" + ybjzlsh + "|" + djh + "|" +
                      "|" + djh + "||" + JYLSH + "|1|" +
                      "|" + YLGHBH + "|0|||" +
                      grbh + "|0|0|0|0|" +
                      "0|0|0|" + dbbyzf + "|" + ybfwnfy + "|" +
                      yblx + "|";

                    WriteLog(sysdate + "  门诊费用预结算返回参数|" + strValue);
                    //return new object[] { 0, 1, strValue };
                    strSql = string.Format(@"insert into ybfyjsdr (jzlsh,jylsh,djhin,cyrq,cyyy,bzbm,bzmc,yllb,xm,kh,
                                        grbh,ylfze,zbxje,tcjjzf,dejjzf,dbbyzf,mzjzfy,zhzf,xjzf,zhzfzf,
                                        zhzfzl,xjzhzf,xjzhzl,ybfwnfy,bcjsqzhye,dbzbc,smxx,yfhj,zlxmfhj,bbzf,
                                        bz6,jbr,zcfbz,djh,cfmxjylsh,zffy,ybjzlsh,cxbz,sysdate,jsrq,
                                        yblx,sgfwnfy,tcjjgrzf,dejjgrzf,gwybzjjgrzf,cxzhzf,ybjsh,gwybzjjzf,zffy,zlff) values(
                                        '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}',
                                        '{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}',
                                        '{20}','{21}','{22}','{23}','{24}','{25}','{26}','{27}','{28}','{29}',
                                        '{30}','{31}','{32}','{33}','{34}','{35}','{36}','{37}','{38}','{39}',
                                        '{40}','{41}','{42}','{43}','{44}','{45}','{46}','{47}','{48}','{49}')",
                                        jzlsh, JYLSH, djh, cyrq, cyyy, bzbm, bzmc, yllb, xm, kh,
                                        grbh, ylzfy, zbxje, tcjjzf, dejjzf, dbbyzf, mzfdfy, zhzf, xjzf, zhzfzf,
                                        zhzfzl, xjzfzf, xjzfzl, ybfwnfy, bcjsqzhye, dbzbm, smxx, yfhj, qtjjzf,
                                        bbzf, by6, jbrbh, zcfbz, djh, cfjylsh, xjzf_gh, ybjzlsh, 1, sysdate, sysdate,
                                        yblx, sgfwnfy, tcjjgrzf, dejjgrzf, gwybzjjgrzf, cxzhzf, psBillCode, gwybzjjzf, zfefy, zfufy);
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
                        object[] objFYDJCX = { jzlsh, psBillCode, ybjzlsh };
                        objFYDJCX = N_YBMZZYFYJSCX(objFYDJCX);
                        return new object[] { 0, 0, "门诊费用结算失败|数据操作失败|" + obj[2].ToString() };
                    }
                }
                else
                {
                    WriteLog(sysdate + "  门诊费用结算失败" );
                    return new object[] { 0, 0, "  门诊费用结算失败" };
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

            JYLSH = DateTime.Now.ToString("yyyyMMddHHmmss") + "-" + YLGHBH + "-" + new Random().Next(100).ToString().PadLeft(4, '0');


            if (string.IsNullOrEmpty(jzlsh))
                return new object[] { 0, 0, "就诊流水号不能为空" };
            if (string.IsNullOrEmpty(djh))
                return new object[] { 0, 0, "单据号不能为空" };

            //判断是否已结算
            string strSql = string.Format(@"select * from ybfyjsdr where jzlsh='{0}' and djhin='{1}' and cxbz=1", jzlsh, djh);
            DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            if (ds.Tables[0].Rows.Count == 0)
                return new object[] { 0, 0, "该患者无医保结算信息" };
            string cfjylsh = ds.Tables[0].Rows[0]["cfmxjylsh"].ToString();
            string ybjzlsh = ds.Tables[0].Rows[0]["ybjzlsh"].ToString();
            string ybjsh = ds.Tables[0].Rows[0]["ybjsh"].ToString();

            string jscxrq = Convert.ToDateTime(sysdate).ToString("yyyyMMddHHmmss");//结算撤销日期


            #region 业务出参
            float ylzfy = 0;  //医疗总费用
            float sgfwnfy = 0;  //三个范围内费用
            float zfufy = 0;   //自付费用
            float zfefy = 0;   //自费费用
            float qfbz = 0;    //起付标准
            float tcjjzf = 0;  //统筹支付
            float tcjjgrzf = 0;    //统筹个人自负
            float dejjzf = 0;    //大病救助基金支付
            float dejjgrzf = 0;     //大病救助基金个人自负
            float gwybzjjzf = 0;  //公务员/企业补充支付
            float gwybzjjgrzf = 0;     //公务员/企业个人自负
            float qtjjzf = 0;     //其它基金支付
            float zhzf = 0;    //医疗账户支付
            float cxzhzf = 0;   //储蓄账户支付
            float xjzf = 0;    //现金支付
            #endregion



            object[] obj1 = { ybjzlsh, "", "M" };
            obj1 = YBDK(obj1);
            if (obj1[1].ToString().Equals("0"))
            {
                return new object[] { 0, 0, "门诊结算读卡失败" };
            }

            int i = FCancelOutpatBalance(ybjzlsh, ybjsh, CliUtils.fLoginUser, ref   ylzfy, ref   sgfwnfy, ref   zfufy, ref   zfefy, ref   qfbz, ref    tcjjzf, ref   tcjjgrzf, ref   dejjzf, ref   dejjgrzf,
            ref   gwybzjjzf, ref   gwybzjjgrzf, ref   qtjjzf, ref   zhzf, ref   cxzhzf, ref   xjzf);

            List<string> liSQL = new List<string>();
            if (i == 0)
            {
                WriteLog(sysdate + "  门诊费用结算撤销成功|" );
                strSql = string.Format(@"insert into ybfyjsdr (jzlsh,jylsh,djhin,cyrq,cyyy,bzbm,bzmc,yllb,xm,kh,
                                        grbh,ylfze,zbxje,tcjjzf,dejjzf,dbbyzf,mzjzfy,zhzf,xjzf,zhzfzf,
                                        zhzfzl,xjzhzf,xjzhzl,ybfwnfy,bcjsqzhye,dbzbc,smxx,yfhj,zlxmfhj,bbzf,sgfwnfy,tcjjgrzf,dejjgrzf,gwybzjjgrzf,cxzhzf,ybjsh,
                                        bz6,cfmxjylsh,zcfbz,jbr,djh,ybjzlsh,cxbz,sysdate,jsrq) select jzlsh,jylsh,djhin,cyrq,cyyy,bzbm,bzmc,yllb,xm,kh,
                                        grbh,ylfze,zbxje,tcjjzf,dejjzf,dbbyzf,mzjzfy,zhzf,xjzf,zhzfzf,
                                        zhzfzl,xjzhzf,xjzhzl,ybfwnfy,bcjsqzhye,dbzbc,smxx,yfhj,zlxmfhj,bbzf,sgfwnfy,tcjjgrzf,dejjgrzf,gwybzjjgrzf,cxzhzf,ybjsh,
                                        bz6,cfmxjylsh,zcfbz,'{3}',djh,ybjzlsh,0,'{2}','{2}' from ybfyjsdr where jzlsh='{0}' and djhin='{1}' and cxbz=1", jzlsh, djh, sysdate, CliUtils.fLoginUser);
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
                WriteLog(sysdate + "  门诊费用结算撤销失败|");
                return new object[] { 0, 0, "门诊费用结算撤销失败|" };
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

                JYLSH = DateTime.Now.ToString("yyyyMMddHHmmss") + "-" + YLGHBH + "-" + new Random().Next(100).ToString().PadLeft(4, '0');

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
                string bqnm = string.Empty; //病区名称
                string bqno = string.Empty; //病区编码

                if (string.IsNullOrEmpty(jzlsh))
                    return new object[] { 0, 0, "就诊流水号不能为空" };
                if (string.IsNullOrEmpty(yllb))
                    return new object[] { 0, 0, "医疗类别代码不能为空|" };

                string strSql = string.Format(@"select a.z1zyno,a.z1hznm,a.z1date, a.z1ksno, a.z1ksnm,a.z1empn as z1ysno,a.z1mzys as z1ysnm,'' as z1bedn,a.z1mobi,a.z1yzbm,a.z1bzno,a.z1bznm,z1bqnm,z1bqxx from zy01h a
                                            where a.z1zyno='{0}'", jzlsh);
                DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                if (ds == null || ds.Tables[0].Rows.Count == 0)
                    return new object[] { 0, 0, "无患者信息" };
                rysj = Convert.ToDateTime(ds.Tables[0].Rows[0]["z1date"].ToString()).ToString("yyyy-MM-dd HH:mm:ss");
                ksmc = ds.Tables[0].Rows[0]["z1ksnm"].ToString();
                ksbm = ds.Tables[0].Rows[0]["z1ksno"].ToString();
                cwh = ds.Tables[0].Rows[0]["z1bedn"].ToString();
                ysbm = ds.Tables[0].Rows[0]["z1ysno"].ToString();
                ysxm = ds.Tables[0].Rows[0]["z1ysnm"].ToString();
                lxdh = ds.Tables[0].Rows[0]["z1mobi"].ToString();
                hzxm = ds.Tables[0].Rows[0]["z1hznm"].ToString();
                bqnm = ds.Tables[0].Rows[0]["z1bqnm"].ToString();
                bqno = ds.Tables[0].Rows[0]["z1bqxx"].ToString();
                if (!string.Equals(hzxm, xm))
                    return new object[] { 0, 0, "入院登记姓名和医保卡姓名不相符" };

                strSql = string.Format(@"select * from ybmzzydjdr where jzlsh='{0}' and cxbz=1", jzlsh);
                ds.Tables.Clear();
                ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                if (ds.Tables[0].Rows.Count > 0)
                    return new object[] { 0, 0, "患者已进行医保住院登记，清匆再进行重复操作" };

                string ybjzlsh = "";
        
                  //获取医保就诊流水号
                object[] objjzlsh = { "Z" };
                objjzlsh = GetJZLSH(objjzlsh);
                if (objjzlsh[1].ToString().Equals("1"))
                {
                    ybjzlsh = objjzlsh[2].ToString();
                }
                else
                {
                    return new object[] { 0, 0, "获取就诊流水号失败" };
                }

                object[] obj1 = { ybjzlsh, "", "Z" };
                obj1 = YBDK(obj1);
                if (obj1[1].ToString().Equals("0"))
                {
                    return new object[] { 0, 0, "住院登记读卡失败" };
                }



                #region 入参
                string psRecCode = ybjzlsh;
                string psMedMode = "2"; //医疗方式  1普通门诊；2普通住院；3特殊门诊；4紧急抢救；5急诊；
                string psMedClass = yllb;
                string psRegOpCode = CliUtils.fLoginUser;
                string psBegDate = rysj;
                string psICDMode = "A";
                string psICD = bzbm;
                string psDepCode = ksbm;
                string psSecCode = bqno;
                string psRegDoc = ysbm;
                float pcInHosNum = 0;

                #endregion





                if(yllb != "21")
                {
                    yllb = "22";//特殊病种住院
                }

                int i = FInpatReg(psRecCode, psMedMode, psMedClass, psRegOpCode, psBegDate, psICDMode, psICD, psDepCode, psSecCode, psRegDoc, ref pcInHosNum);


                if (i == 0)
                {
                    List<string> liSQL = new List<string>();

                    strSql = string.Format(@"insert into ybmzzydjdr(
                                        jzlsh,jylsh,yldylb,ghdjsj,bzbm,bzmc,bq,ksbh,ksmc,cwh,
                                        ysdm,jbr,dwmc,grbh,xm,xb,ybjzlsh,kh,yllb,cxbz,
                                        sysdate,tcqh,ydrybz,nl,csrq,zhye,jzbz,dwbh,ysxm)
                                        values(
                                        '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}',
                                        '{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}',
                                        '{20}','{21}','{22}','{23}','{24}','{25}','{26}','{27}','{28}')",
                                            jzlsh, JYLSH, yldylb, rysj, bzbm, bzmc, "", ksbm, ksmc, cwh,
                                            ysbm, jbr, dwmc, ybbh, xm, xb, ybjzlsh, kh, yllb, 1,
                                            sysdate, tcqh, ydrybz, nl, csrq, zhye, "z", dwbh, ysxm);
                    liSQL.Add(strSql);


                    strSql = string.Format(@"update zy01h set z1rylb = '{0}', z1tcdq = '{1}', z1lyjg = '{2}', z1lynm = '{3}', z1ylno = '{4}'
                                            , z1ylnm = '{5}', z1bzno = '{6}', z1bznm = '{7}', z1ybno = '{8}' where z1comp = '{9}' and z1zyno = '{10}'"
                                            , yldylb, tcqh, lyjgdm, lyjgmc, yllb, yllbmc, bzbm, bzmc, grbh, CliUtils.fSiteCode, jzlsh);
                    liSQL.Add(strSql);

                    object[] obj_dj = liSQL.ToArray();
                    obj_dj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj_dj);
                    if (obj_dj[1].ToString().Equals("1"))
                    {
                        WriteLog(sysdate + "  住院登记成功|" + pcInHosNum.ToString());
                        return new object[] { 0, 1, "住院登记成功" };
                    }
                    else
                    {
                        WriteLog(sysdate + "  住院登记失败|数据操作失败|" + obj_dj[2].ToString());
                        //登记撤销
                        object[] objParam1 = { ybjzlsh };
                        objParam1 = N_YBMZZYDJCX(objParam1);
                        return new object[] { 0, 0, "住院登记失败|数据操作失败|" + obj_dj[2].ToString() };
                    }
                }
                else
                {
                    WriteLog(sysdate + "  住院登记失败|" );
                    return new object[] { 0, 0, "  住院登记失败|" };
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

            JYLSH = DateTime.Now.ToString("yyyyMMddHHmmss") + "-" + YLGHBH + "-" + new Random().Next(100).ToString().PadLeft(4, '0');
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



            object[] obj1 = { ybjzlsh, "", "Z" };
            obj1 = YBDK(obj1);
            if (obj1[1].ToString().Equals("0"))
            {
                return new object[] { 0, 0, "住院登记撤销读卡失败" };
            }

            int i = FCancelInpatReg(ybjzlsh, CliUtils.fLoginUser);

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
            string jzsj = objParam[1].ToString();   //截止时间
            string sWhere = "";

            if (string.IsNullOrEmpty(jzlsh))
                return new object[] { 0, 0, "就诊流水号不能为空" };

            if (!string.IsNullOrEmpty(jzsj))
            {
                jzsj = Convert.ToDateTime(jzsj).ToString("yyyy-MM-dd") + " 23:59:59";
                sWhere = string.Format(@"a.z3date <=CONVERT(datetime,'{0}')", jzsj);
            }
                

            JYLSH = DateTime.Now.ToString("yyyyMMddHHmmss") + "-" + YLGHBH + "-" + new Random().Next(100).ToString().PadLeft(4, '0');
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
//            strSql = string.Format(@"select 
//                                    case when a.z3sfno IN(select b5sfno from bz05h where b5zyno in('01','02','03')) then 1 
//                                    when a.z3sfno IN(select b5sfno from bz05h where b5zyno in('19')) then 3
//                                    else 2 end as sfxmzl,MAX(z3date) as zxsj, a.z3item as yyxmbh, a.z3name as yyxmmc,
//                                    a.z3djxx as dj,sum(case LEFT(a.z3endv,1) when '4' then -a.z3jzcs else 1*a.z3jzcs end) as sl,
//                                    sum(case LEFT(a.z3endv,1) when '4' then -1*a.z3jzje else 1*a.z3jzje end) as je, '' as fs,
//                                    a.z3empn as ysdm, a.z3kdys as ysxm,a.z3ksno as ksno, a.z3zxks as zxks, '0' as sfbz,
//									b.b5unit unit
//                                    from zy03d a left join bz05d b on a.z3item = b5item 
//                                    where a.z3ybup is null and LEFT(a.z3kind,1) in(2,4)  and a.z3zyno='{0}' {1} 
//                                    group by a.z3djxx,a.z3item,a.z3name, a.z3empn,a.z3kdys,a.z3ksno,a.z3zxks,a.z3sfno,b.b5unit
//                                    having sum(case LEFT(a.z3endv,1) when '4' then -1*a.z3jzcs else 1*a.z3jzcs end)!=0", jzlsh, sWhere);

            strSql = string.Format(@"select 
                                    case when a.z3sfno IN(select b5sfno from bz05h where b5zyno in('01','02','03')) then 1 
                                    when a.z3sfno IN(select b5sfno from bz05h where b5zyno in('19')) then 3
                                    else 2 end as sfxmzl,MAX(z3date) as zxsj, a.z3item as yyxmbh, a.z3name as yyxmmc,
                                    a.z3djxx as dj,sum(case LEFT(a.z3endv,1) when '4' then -a.z3jzcs else 1*a.z3jzcs end) as sl,
                                    sum(case LEFT(a.z3endv,1) when '4' then -1*a.z3jzje else 1*a.z3jzje end) as je, '' as fs,
                                    a.z3empn as ysdm, a.z3kdys as ysxm,a.z3ksno as ksno, a.z3zxks as zxks, '0' as sfbz,
									case when a.z3sfno IN(select b5sfno from bz05h where b5zyno in('01','02','03')) then c.y1zydw
                                    when a.z3sfno IN(select b5sfno from bz05h where b5zyno in('19')) then b.b5unit
                                    else b.b5unit end as unit
                                    from zy03d a left join bz05d b on a.z3item = b.b5item left join yp01h c on c.y1ypno = a.z3item
                                    where a.z3ybup is null and LEFT(a.z3kind,1) in(2,4)  and a.z3zyno='{0}' {1} 
                                    group by a.z3djxx,a.z3item,a.z3name, a.z3empn,a.z3kdys,a.z3ksno,a.z3zxks,a.z3sfno,b.b5unit,c.y1zydw
                                    having sum(case LEFT(a.z3endv,1) when '4' then -1*a.z3jzcs else 1*a.z3jzcs end)!=0", jzlsh, sWhere);
            ds.Tables.Clear();
            ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            if (ds.Tables[0].Rows.Count == 0)
                return new object[] { 0, 0, "无医嘱信息" };

            List<string> liYZXX = new List<string>(); //医嘱信息
            string sMsg = string.Empty;
            int index_cfh = 1;
            List<string> liSQL = new List<string>();
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                #region 赋值
                string cfh = (index_cfh++).ToString();      //处方号
                string cflsh = "010"+(index_cfh++).ToString();  //处方流水号
                string zxsj = Convert.ToDateTime(dr["zxsj"].ToString()).ToString("yyyy-MM-dd HH:mm:ss");    //执行时间
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
                string unit = dr["unit"].ToString();    //单位;
                string gg = "";
                #endregion

                if (string.IsNullOrEmpty(sfxmzl) || string.IsNullOrEmpty(yyxmbm))
                    sMsg = "存在空数据或未配对数据";

                int piRecType = 1;  //住院
                string psItmFlag = "";
                string psOTCCode = "";
                string psFeeCode = "";
                string psOpCode = CliUtils.fLoginUser;
                string pcDosage = "";
                string psFrequency = "";
                string psUsage = "";
                //判断是否为药品/收费项目（无非处方药）
                if (sfxmzl.ToString().Equals("1"))
                {
                    psItmFlag = "1";
                    psOTCCode = "1";
                }
                else
                {
                    psItmFlag = "0";
                    psOTCCode = "2";
                }

                float pcRate = 0;
                float pcSelfFee = 0;
                float pcDeduct = 0;
                float dsl = Convert.ToSingle(sl);
                float ddj = Convert.ToSingle(dj);
                float dpcdocage = 0;
                float days = 0;
                if(!string.IsNullOrEmpty(pcDosage))
                    dpcdocage = Convert.ToSingle(pcDosage);


                int i = FWriteFeeDetail(piRecType, ybjzlsh, psItmFlag, yyxmbm, cflsh, yyxmmc, unit, gg, psFeeCode, psOTCCode,
             dsl, ddj, ddj, dpcdocage, psFrequency, psUsage,
              days, psOpCode, ksbm, ysbm, zxsj, ref   pcRate, ref   pcSelfFee, ref   pcDeduct);

                if (i == 0)
                {
                    #region 记录上传费用信息
                    strSql = string.Format(@"insert into ybcfmxscindr(jzlsh,jylsh,sfxmzl,ybcfh,djlsh,cfrq,yysfxmbm,yysfxmmc,dj,sl,
						                   je,ysbm,ksbh,jbr,sflb,grbh,xm,kh,ybjzlsh,cxbz,sysdate) values(
                                           '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}',
                                           '{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}','{20}')",
                                             jzlsh, JYLSH, sfxmzl, cfh, cflsh, zxsj, yyxmbm, yyxmmc, dj, sl,
                                             je, ysbm, ksbm, jbr, sfbz, grbh, xm, kh, ybjzlsh, 1, sysdate);
                    liSQL.Add(strSql);

                    strSql = string.Format(@"insert into ybcfmxscfhdr(jzlsh,jylsh,yyxmdm,ybcfh,cfh,je,zfje,zlje,zlbl,qezfbz,
                                                sfxmdj,yyxmmc,grbh,xm,kh,cxbz,sysdate,ybjzlsh ) values(
                                                '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}',
                                                '{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}')",
                                            jzlsh, JYLSH, yyxmbm, cfh, cflsh, je, pcSelfFee, pcDeduct, pcRate, null,
                                            null, null, grbh, xm, kh, 1, sysdate, ybjzlsh);
                    liSQL.Add(strSql);
                    #endregion

                   
                }
                else
                {

                    WriteLog(sysdate + "  门诊费用登记失败|" + ybjzlsh);
                    return new object[] { 0, 0, "门诊费用登记失败" + ybjzlsh };
                }


            }

            if (!string.IsNullOrEmpty(sMsg))
                return new object[] { 0, 0, sMsg };
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
                //内部费用上传撤销
                object[] objParam1 = { ybjzlsh };
                objParam1 = N_YBFYDJCX(objParam1);
                return new object[] { 0, 0, "住院费用登记失败|数据操作失败|" + obj[2].ToString() };
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

            JYLSH = DateTime.Now.ToString("yyyyMMddHHmmss") + "-" + YLGHBH + "-" + new Random().Next(100).ToString().PadLeft(4, '0');
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


            WriteLog(sysdate + "  进入住院费用登记撤销...");

            int i = FSynData(2, ybjzlsh);

            List<string> liSQL = new List<string>();
            if (i == 0)
            {
                strSql = string.Format(@"insert into ybcfmxscindr(jzlsh,jylsh,sfxmzl,ybcfh,djlsh,cfrq,yysfxmbm,yysfxmmc,dj,sl,
						                 je,ysbm,ksbh,jbr,sflb,grbh,xm,kh,cxbz,sysdate) select 
                                         jzlsh,jylsh,sfxmzl,ybcfh,djlsh,cfrq,yysfxmbm,yysfxmmc,dj,sl,
						                 je,ysbm,ksbh,'{1}',sflb,grbh,xm,kh,0,'{2}' from ybcfmxscindr where jzlsh='{0}' and cxbz=1",
                                         jzlsh, jbr, sysdate);
                liSQL.Add(strSql);
                strSql = string.Format(@"update ybcfmxscindr set cxbz=2 where jzlsh='{0}' and cxbz=1", jzlsh);
                liSQL.Add(strSql);

                strSql = string.Format(@"insert into ybcfmxscfhdr(jzlsh,jylsh,yyxmdm,ybcfh,cfh,je,zfje,zlje,zlbl,qezfbz,
                                         sfxmdj,yyxmmc,grbh,xm,kh,cxbz,sysdate ) select
                                         jzlsh,jylsh,yyxmdm,ybcfh,cfh,je,zfje,zlje,zlbl,qezfbz,
                                         sfxmdj,yyxmmc,grbh,xm,kh,0,'{1}' from ybcfmxscfhdr where jzlsh='{0}' and cxbz=1",
                                         jzlsh,  sysdate);
                liSQL.Add(strSql);
                strSql = string.Format(@"update ybcfmxscfhdr set cxbz=2 where jzlsh='{0}'  and cxbz=1", jzlsh);
                liSQL.Add(strSql);

                strSql = string.Format("update zy03d set z3ybup = null where z3zyno = '{0}' ", jzlsh);
                liSQL.Add(strSql);

                object[] obj = liSQL.ToArray();
                obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);
                if (obj[1].ToString().Equals("1"))
                {
                    WriteLog(sysdate + "  住院费用登记撤销成功|" );
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
                WriteLog(sysdate + "  住院费用登记撤销失败|");
                return new object[] { 0, 0, "住院费用登记撤销失败|数据操作失败|" };
            }

        }
        #endregion

        #region 出院
        public static object[] YBCY(object[] objParam)
        {
            string jzlsh = objParam[0].ToString();
            string sysdate = GetServerDateTime();



            string strSql = string.Format(@"select * from ybmzzydjdr where jzlsh='{0}' and cxbz=1", jzlsh);
            DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            if (ds.Tables[0].Rows.Count == 0)
                return new object[] { 0, 0, "该患者未医保门诊登记" };
            string ybjzlsh = ds.Tables[0].Rows[0]["ybjzlsh"].ToString();


            strSql = string.Format("select a.z1outd as cyrq, b.z1cyyy as cyyy, a.z1zyys as zyys from zy01d a left join zy01h b on a.z1ghno = b. z1ghno where left(a.z1endv, 1) = '8' and a.z1zyno = '{0}'", jzlsh);
            ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            if (ds == null || ds.Tables[0].Rows.Count == 0)
                return new object[] { 0, 0, "就诊流水号" + jzlsh + "未拖出床位" };
            string cyrq = Convert.ToDateTime(ds.Tables[0].Rows[0]["cyrq"]).ToString("yyyy-MM-dd HH:mm:ss"); //出院日期
            string cyyy = ds.Tables[0].Rows[0]["cyyy"].ToString();
            string zyys = ds.Tables[0].Rows[0]["zyys"].ToString();


            strSql = string.Format(@"select * from mza1dd where m1xybz = 'Y' and m1mzno='{0}'", jzlsh);
             ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            if (ds.Tables[0].Rows.Count == 0)
                return new object[] { 0, 0, "该患者无出院诊断" };
            string cyzd = ds.Tables[0].Rows[0]["m1xyzd"].ToString();

            string psICDMode = "A";


            object[] obj1 = { ybjzlsh, "", "Z" };
            obj1 = YBDK(obj1);
            if (obj1[1].ToString().Equals("0"))
            {
                return new object[] { 0, 0, "出院读卡失败" };
            }

            int i = FInpatLeave(ybjzlsh, CliUtils.fLoginUser, cyrq, cyyy, psICDMode, cyzd, zyys);

            if (i == 0)
            {
                WriteLog(sysdate + "  医保出院成功");
                return new object[] { 0, 1, "  医保出院成功" };
            }
            else
            {
                WriteLog(sysdate + "  住院费用预结算返回参数|" );
                return new object[] { 0, 1, "  住院费用预结算返回参数|" };
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
            //string ylfhj1 = objParam[5].ToString(); //医疗费合计 

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
            string ydrybz = ds.Tables[0].Rows[0]["ydrybz"].ToString();     //异地人员标识

            strSql = string.Format("select a.z1outd as cyrq from zy01d a where left(a.z1endv, 1) = '8' and a.z1zyno = '{0}'", jzlsh);
            ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            if (ds == null || ds.Tables[0].Rows.Count == 0)
                return new object[] { 0, 0, "就诊流水号" + jzlsh + "未拖出床位" };
            cyrq = Convert.ToDateTime(ds.Tables[0].Rows[0]["cyrq"]).ToString("yyyyMMddHHmmss"); //出院日期



            string psUseAcc = "是";
            string piLiquiMode = "0";
            string psRefundID = "";//报销代码：OA01门诊报销/OB01门诊异地报销/OC01门诊转院报销/IA01住院报销/IB01住院异地报销/IC01住院转院报销
            float pcOther1 = 0;
            float pcOther2 = 0;
            string psMemo = "";
            #region 业务出参
            StringBuilder psBillCode = new StringBuilder(16);
            float ylzfy = 0;  //医疗总费用
            float sgfwnfy = 0;  //三个范围内费用
            float zfufy = 0;   //自付费用
            float zfefy = 0;   //自费费用
            float qfbz = 0;    //起付标准
            float tcjjzf = 0;  //统筹支付
            float tcjjgrzf = 0;    //统筹个人自负
            float dejjzf = 0;    //大病救助基金支付
            float dejjgrzf = 0;     //大病救助基金个人自负
            float gwybzjjzf = 0;  //公务员/企业补充支付
            float gwybzjjgrzf = 0;     //公务员/企业个人自负
            float qtjjzf = 0;     //其它基金支付
            float zhzf = 0;    //医疗账户支付
            float cxzhzf = 0;   //储蓄账户支付
            float xjzf = 0;    //现金支付
            #endregion

            #region 如果是中途结算，则需要对已上传费用进行撤销再上传操作
            if (ztjsbz.Equals("1"))
            {
                piLiquiMode = "1";
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

            string jsrq = Convert.ToDateTime(sysdate).ToString("yyyyMMddHHmmss");   //结算日期
            JYLSH = DateTime.Now.ToString("yyyyMMddHHmmss") + "-" + YLGHBH + "-" + new Random().Next(100).ToString().PadLeft(4, '0');
            string jbr = CliUtils.fUserName;    //经办人



            if (ydrybz.ToString().Equals("1"))
            {
                psRefundID = "IB01";//门诊异地报销
            }
            else
            {
                psRefundID = "IA01";//门诊本地报销
            }


            #region 上传医保数据
            object[] objsc = { jzlsh };
            objsc = YBSC(objsc);
            if (objsc[1].ToString().Equals("0"))
            {
                return new object[] { 0, 0, "医保上传失败" };
            }
            #endregion

            #region 出院
            object[] obj1 = {jzlsh};
            obj1 = YBCY(obj1);
            if (obj1[1].ToString().Equals("0"))
            {
                return new object[] { 0, 0, "医保出院失败" };
            }
            #endregion


            object [] obj2 = {ybjzlsh, "", "Z"};
            obj2 = YBDK(obj2);
            if (obj2[1].ToString().Equals("0"))
            {
                return new object[] { 0, 0, "住院预结算读卡失败" };
            }



            int i = FTryInpatBalance(ybjzlsh, jbr, psUseAcc, piLiquiMode, psRefundID, pcOther1, pcOther2, psMemo,psBillCode, ref   ylzfy, ref   sgfwnfy, ref   zfufy, ref   zfefy, ref   qfbz, ref    tcjjzf, ref   tcjjgrzf, ref   dejjzf, ref   dejjgrzf,
            ref   gwybzjjzf, ref   gwybzjjgrzf, ref   qtjjzf, ref   zhzf, ref   cxzhzf, ref   xjzf);

            
            if (i == 0)
            {
                WriteLog(sysdate + "  住院费用预结算成功|");
                #region 出参赋值
                float zbxje = Convert.ToSingle(0);
                float dbbyzf = Convert.ToSingle(0);
                float mzfdfy = Convert.ToSingle(0);
                float zhzfzf = zfufy;
                float zhzfzl = Convert.ToSingle(0);
                float xjzfzf = zfefy;
                float xjzfzl = Convert.ToSingle(0);
                float ybfwnfy = Convert.ToSingle(0);
                float bcjsqzhye = Convert.ToSingle(0);
                string dbzbm = "";        //单病种病种编码
                string smxx = "";        //说明信息
                float yfhj = Convert.ToSingle(0);       //药费合计
                float zlxmfhj = Convert.ToSingle(0);       //诊疗项目费合计
                float bbzf = Convert.ToSingle(0);       //补保支付
                string yllb_r = "";    //医疗类别
                string by6 = "";


                decimal qybcylbxjjzf = Convert.ToDecimal(0);  //企业补充医疗保险基金支付
                decimal dwfdfy = Convert.ToDecimal(0);        //单位负担费用    
                decimal yyfdfy = Convert.ToDecimal(0);       //医院负担费用
                //decimal mzfdfy = Convert.ToDecimal("0");       //民政负担费用
                decimal cxjfy = Convert.ToDecimal(0);        //超限价费用单病种病种编码
                decimal ylzlfy = Convert.ToDecimal(0);       //乙类自理费用
                decimal blzlfy = Convert.ToDecimal(0);       //丙类自理费用
                decimal fhjbylfy = Convert.ToDecimal(0);     //符合基本医疗费用
                decimal qfbzfy = Convert.ToDecimal(0);       //起付标准费用
                decimal zzzyzffy = Convert.ToDecimal(0);     //转诊转院自付费用
                decimal jrtcfy = Convert.ToDecimal(0);       //进入统筹费用
                decimal tcfdzffy = Convert.ToDecimal(0);     //统筹分段自付费用
                decimal ctcfdxfy = Convert.ToDecimal(0);       //超统筹封顶线费用
                decimal jrdebsfy = Convert.ToDecimal(0);       //进入大额报销费用
                decimal defdzffy = Convert.ToDecimal(0);       //大额分段自付费用
                decimal cdefdxfy = Convert.ToDecimal(0);       //超大额封顶线费用
                decimal rgqgzffy = Convert.ToDecimal(0);       //人工器管自付费用
                //decimal bcjsqzfye = Convert.ToDecimal(0);       //本次结算前帐户余额
                decimal bntczflj = Convert.ToDecimal(0);       //本年统筹支付累计
                decimal bndezflj = Convert.ToDecimal(0);       //本年大额支付累计
                decimal bnczjmmztczflj = Convert.ToDecimal(0);       //本年城镇居民门诊统筹支付累计
                decimal bngwybzzflj = Convert.ToDecimal(0);  //本年公务员补助支付累计(不含本次)
                decimal bnzhzflj = Convert.ToDecimal(0);  //本年账户支付累计(不含本次)
                string bnzycslj = "1";  //本年住院次数累计(不含本次)
                string zycs = "1";        //住院次数
                string yldylb = ""; //医疗待遇类别
                string jbjgbm = ""; //经办机构编码
                string jbrbh = CliUtils.fLoginUser;
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
                    */

                string strValue = ylzfy + "|" + zbxje + "|" + tcjjzf + "|" + dejjzf + "|" + zhzf + "|" +
                    xjzf + "|" + gwybzjjzf + "|" + qybcylbxjjzf + "|" + "0" + "|" + dwfdfy + "|" +
                  yyfdfy + "|" + mzfdfy + "|" + cxjfy + "|" + ylzlfy + "|" + blzlfy + "|" +
                  fhjbylfy + "|" + qfbzfy + "|" + zzzyzffy + "|" + jrtcfy + "|" + tcfdzffy + "|" +
                  ctcfdxfy + "|" + "0" + "|" + defdzffy + "|" + cdefdxfy + "|" + rgqgzffy + "|" +
                  bcjsqzhye + "|" + bntczflj + "|" + bndezflj + "|" + bnczjmmztczflj + "|" + bngwybzzflj + "|" +
                      bnzhzflj + "|" + bnzycslj + "|" + zycs + "|" + xm + "|" + jsrq + "|" +
                      yllb_r + "||" + YLGHBH + "|" + YWZQH + "|" + djh + "|" +
                      "|" + djh + "||" + JYLSH + "|1|" +
                      "|" + YLGHBH + "|0|||" +
                      grbh + "|0|0|0|0|" +
                      "0|0|0|" + dbbyzf + "|" + ybfwnfy + "|";
                WriteLog(sysdate + "  住院费用预结算返回参数|" + strValue);
                strSql = string.Format(@"insert into ybfyyjsdr (jzlsh,jylsh,djhin,cyrq,cyyy,bzbm,bzmc,yllb,xm,kh,
                                        grbh,ylfze,zbxje,tcjjzf,dejjzf,dbbyzf,mzjzfy,zhzf,xjzf,zhzfzf,
                                        zhzfzl,xjzhzf,xjzhzl,ybfwnfy,bcjsqzhye,dbzbc,smxx,yfhj,zlxmfhj,bbzf,
                                        bz6,jbr,ybjzlsh,cfmxjylsh,cxbz,sysdate,jsrq,djh,sgfwnfy,tcjjgrzf,dejjgrzf,gwybzjjgrzf,cxzhzf,ybjsh) values(
                                        '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}',
                                        '{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}',
                                        '{20}','{21}','{22}','{23}','{24}','{25}','{26}','{27}','{28}','{29}',
                                        '{30}','{31}','{32}','{33}','{34}','{35}','{36}','{37}')",
                                     jzlsh, JYLSH, djh, cyrq, cyyy, cyzdbm, cyzdmc, yllb, xm, kh,
                                     grbh, ylzfy, zbxje, tcjjzf, dejjzf, dbbyzf, mzfdfy, zhzf, xjzf, zhzfzf,
                                     zhzfzl, xjzfzf, xjzfzl, ybfwnfy, bcjsqzhye, dbzbm, smxx, yfhj, zlxmfhj, bbzf,
                                     by6, jbrbh, ybjzlsh, cfmxjylsh, 1, sysdate, jsrq, djh, sgfwnfy, tcjjgrzf, dejjgrzf, gwybzjjgrzf, cxzhzf, psBillCode);
                object[] obj = { strSql };
                obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);
                WriteLog(sysdate + "  住院费用预结算返回参数|" + strValue);
                return new object[] { 0, 1, strValue };
            }
            else
            {
                WriteLog(sysdate + "  住院费用预结算失败" );
                return new object[] { 0, 0, "  住院费用预结算失败" };
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
            string ydrybz = ds.Tables[0].Rows[0]["ydrybz"].ToString();    //异地人员标识

            strSql = string.Format("select a.z1outd as cyrq from zy01d a where left(a.z1endv, 1) = '8' and a.z1zyno = '{0}'", jzlsh);
            ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            if (ds == null || ds.Tables[0].Rows.Count == 0)
                return new object[] { 0, 0, "就诊流水号" + jzlsh + "未拖出床位" };
            //cyrq = Convert.ToDateTime(ds.Tables[0].Rows[0]["cyrq"]).ToString("yyyyMMddHHmmss"); //出院日期
            cyrq = Convert.ToDateTime(cyrq).ToString("yyyyMMddHHmmss"); //出院日期



            string psUseAcc = "是";
            string piLiquiMode = "0";
            string psRefundID = "";//报销代码：OA01门诊报销/OB01门诊异地报销/OC01门诊转院报销/IA01住院报销/IB01住院异地报销/IC01住院转院报销
            float pcOther1 = 0;
            float pcOther2 = 0;
            string psMemo = "";
            #region 业务出参
            StringBuilder psBillCode = new StringBuilder(16);
            float ylzfy = 0;  //医疗总费用
            float sgfwnfy = 0;  //三个范围内费用
            float zfufy = 0;   //自付费用
            float zfefy = 0;   //自费费用
            float qfbz = 0;    //起付标准
            float tcjjzf = 0;  //统筹支付
            float tcjjgrzf = 0;    //统筹个人自负
            float dejjzf = 0;    //大病救助基金支付
            float dejjgrzf = 0;     //大病救助基金个人自负
            float gwybzjjzf = 0;  //公务员/企业补充支付
            float gwybzjjgrzf = 0;     //公务员/企业个人自负
            float qtjjzf = 0;     //其它基金支付
            float zhzf = 0;    //医疗账户支付
            float cxzhzf = 0;   //储蓄账户支付
            float xjzf = 0;    //现金支付
            #endregion


            #region 如果是中途结算，则需要对已上传费用进行撤销再上传操作
            if (ztjsbz.Equals("1"))
            {
                piLiquiMode = "1";
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


            string jsrq = Convert.ToDateTime(sysdate).ToString("yyyyMMddHHmmss");   //结算日期
            JYLSH = DateTime.Now.ToString("yyyyMMddHHmmss") + "-" + YLGHBH + "-" + new Random().Next(100).ToString().PadLeft(4, '0');
            string jbr = CliUtils.fUserName;    //经办人
            string jbrbh = CliUtils.fLoginUser;


            if (ydrybz.ToString().Equals("1"))
            {
                psRefundID = "IB01";//门诊异地报销
            }
            else
            {
                psRefundID = "IA01";//门诊本地报销
            }


            object[] obj1 = { ybjzlsh, "", "Z" };
            obj1 = YBDK(obj1);
            if (obj1[1].ToString().Equals("0"))
            {
                return new object[] { 0, 0, "住院预结算读卡失败" };
            }

            int i = FInpatBalance(ybjzlsh, jbr, psUseAcc, piLiquiMode, psRefundID, pcOther1, pcOther2, psMemo, psBillCode, ref   ylzfy, ref   sgfwnfy, ref   zfufy, ref   zfefy, ref   qfbz, ref    tcjjzf, ref   tcjjgrzf, ref   dejjzf, ref   dejjgrzf,
            ref   gwybzjjzf, ref   gwybzjjgrzf, ref   qtjjzf, ref   zhzf, ref   cxzhzf, ref   xjzf);


            if (i == 0)
            {
                WriteLog(sysdate + "  住院费用预结算成功|");
                #region 出参赋值
                float zbxje = Convert.ToSingle(0);
                float dbbyzf = Convert.ToSingle(0);
                float mzfdfy = Convert.ToSingle(0);
                float zhzfzf = zfufy;
                float zhzfzl = Convert.ToSingle(0);
                float xjzfzf = zfefy;
                float xjzfzl = Convert.ToSingle(0);
                float ybfwnfy = Convert.ToSingle(0);
                float bcjsqzhye = Convert.ToSingle(0);
                string dbzbm = "";        //单病种病种编码
                string smxx = "";        //说明信息
                float yfhj = Convert.ToSingle(0);       //药费合计
                float zlxmfhj = Convert.ToSingle(0);       //诊疗项目费合计
                float bbzf = Convert.ToSingle(0);       //补保支付
                string yllb_r = "";    //医疗类别
                string by6 = "";

                decimal qybcylbxjjzf = Convert.ToDecimal(0);  //企业补充医疗保险基金支付
                decimal dwfdfy = Convert.ToDecimal(0);        //单位负担费用    
                decimal yyfdfy = Convert.ToDecimal(0);       //医院负担费用
                //decimal mzfdfy = Convert.ToDecimal("0");       //民政负担费用
                decimal cxjfy = Convert.ToDecimal(0);        //超限价费用单病种病种编码
                decimal ylzlfy = Convert.ToDecimal(0);       //乙类自理费用
                decimal blzlfy = Convert.ToDecimal(0);       //丙类自理费用
                decimal fhjbylfy = Convert.ToDecimal(0);     //符合基本医疗费用
                decimal qfbzfy = Convert.ToDecimal(0);       //起付标准费用
                decimal zzzyzffy = Convert.ToDecimal(0);     //转诊转院自付费用
                decimal jrtcfy = Convert.ToDecimal(0);       //进入统筹费用
                decimal tcfdzffy = Convert.ToDecimal(0);     //统筹分段自付费用
                decimal ctcfdxfy = Convert.ToDecimal(0);       //超统筹封顶线费用
                decimal jrdebsfy = Convert.ToDecimal(0);       //进入大额报销费用
                decimal defdzffy = Convert.ToDecimal(0);       //大额分段自付费用
                decimal cdefdxfy = Convert.ToDecimal(0);       //超大额封顶线费用
                decimal rgqgzffy = Convert.ToDecimal(0);       //人工器管自付费用
                decimal bcjsqzfye = Convert.ToDecimal(0);       //本次结算前帐户余额
                decimal bntczflj = Convert.ToDecimal(0);       //本年统筹支付累计
                decimal bndezflj = Convert.ToDecimal(0);       //本年大额支付累计
                decimal bnczjmmztczflj = Convert.ToDecimal(0);       //本年城镇居民门诊统筹支付累计
                decimal bngwybzzflj = Convert.ToDecimal(0);  //本年公务员补助支付累计(不含本次)
                decimal bnzhzflj = Convert.ToDecimal(0);  //本年账户支付累计(不含本次)
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
                    xjzf + "|" + gwybzjjzf + "|" + qybcylbxjjzf + "|" + "0" + "|" + dwfdfy + "|" +
                  yyfdfy + "|" + mzfdfy + "|" + cxjfy + "|" + ylzlfy + "|" + blzlfy + "|" +
                  fhjbylfy + "|" + qfbzfy + "|" + zzzyzffy + "|" + jrtcfy + "|" + tcfdzffy + "|" +
                  ctcfdxfy + "|" + "0" + "|" + defdzffy + "|" + cdefdxfy + "|" + rgqgzffy + "|" +
                  bcjsqzhye + "|" + bntczflj + "|" + bndezflj + "|" + bnczjmmztczflj + "|" + bngwybzzflj + "|" +
                      bnzhzflj + "|" + bnzycslj + "|" + zycs + "|" + xm + "|" + jsrq + "|" +
                      yllb_r + "||" + YLGHBH + "|" + YWZQH + "|" + djh + "|" +
                      "|" + djh + "||" + JYLSH + "|1|" +
                      "|" + YLGHBH + "|0|||" +
                      grbh + "|0|0|0|0|" +
                      "0|0|0|" + dbbyzf + "|" + ybfwnfy + "|";
                WriteLog(sysdate + "  住院费用预结算返回参数|" + strValue);
                strSql = string.Format(@"insert into ybfyjsdr (jzlsh,jylsh,djhin,cyrq,cyyy,bzbm,bzmc,yllb,xm,kh,
                                        grbh,ylfze,zbxje,tcjjzf,dejjzf,dbbyzf,mzjzfy,zhzf,xjzf,zhzfzf,
                                        zhzfzl,xjzhzf,xjzhzl,ybfwnfy,bcjsqzhye,dbzbc,smxx,yfhj,zlxmfhj,bbzf,
                                        bz6,jbr,ybjzlsh,cfmxjylsh,cxbz,sysdate,jsrq,djh,sgfwnfy,tcjjgrzf,dejjgrzf,gwybzjjgrzf,cxzhzf,ybjsh,gwybzjjzf,zffy,zlff) values(
                                        '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}',
                                        '{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}',
                                        '{20}','{21}','{22}','{23}','{24}','{25}','{26}','{27}','{28}','{29}',
                                        '{30}','{31}','{32}','{33}','{34}','{35}','{36}','{37}','{38}','{39}','{40}','{41}','{42}','{43}','{44}','{45}','{46}')",
                                      jzlsh, JYLSH, djh, cyrq, cyyy, cyzdbm, cyzdmc, yllb, xm, kh,
                                      grbh, ylzfy, zbxje, tcjjzf, dejjzf, dbbyzf, mzfdfy, zhzf, xjzf, zhzfzf,
                                      zhzfzl, xjzfzf, xjzfzl, ybfwnfy, bcjsqzhye, dbzbm, smxx, yfhj, zlxmfhj, qtjjzf,
                                      by6, jbrbh, ybjzlsh, cfmxjylsh, 1, sysdate, jsrq, djh, sgfwnfy, tcjjgrzf, dejjgrzf, gwybzjjgrzf, cxzhzf, psBillCode, gwybzjjzf,zfefy,zfufy);
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
                    object[] objFYDJCX = { jzlsh, psBillCode, ybjzlsh };
                    objFYDJCX = N_YBMZZYFYJSCX(objFYDJCX);
                    return new object[] { 0, 0, "住院费用结算失败|数据操作失败|" + obj[2].ToString() };

                }
            }
            else
            {
                WriteLog(sysdate + "  住院费用预结算失败");
                return new object[] { 0, 0, "  住院费用预结算失败" };
            }
        }
        #endregion

        #region 住院费用结算撤销
        public static object[] YBZYSFJSCX(object[] objParam)
        {
            string sysdate = GetServerDateTime();//当前时间
            string jzlsh = objParam[0].ToString();   // 就诊流水号
            string djh = objParam[1].ToString();     // 单据号(发票号)

            JYLSH = DateTime.Now.ToString("yyyyMMddHHmmss") + "-" + YLGHBH + "-" + new Random().Next(100).ToString().PadLeft(4, '0');


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
            string ybjsh = ds.Tables[0].Rows[0]["ybjsh"].ToString();

            string jscxrq = Convert.ToDateTime(sysdate).ToString("yyyyMMddHHmmss");//结算撤销日期



            #region 业务出参
            float ylzfy = 0;  //医疗总费用
            float sgfwnfy = 0;  //三个范围内费用
            float zfufy = 0;   //自付费用
            float zfefy = 0;   //自费费用
            float qfbz = 0;    //起付标准
            float tcjjzf = 0;  //统筹支付
            float tcjjgrzf = 0;    //统筹个人自负
            float dejjzf = 0;    //大病救助基金支付
            float dejjgrzf = 0;     //大病救助基金个人自负
            float gwybzjjzf = 0;  //公务员/企业补充支付
            float gwybzjjgrzf = 0;     //公务员/企业个人自负
            float qtjjzf = 0;     //其它基金支付
            float zhzf = 0;    //医疗账户支付
            float cxzhzf = 0;   //储蓄账户支付
            float xjzf = 0;    //现金支付
            #endregion

            WriteLog(ybjzlsh+ybjsh);


            int i = FCancelInpatBalance(ybjzlsh, ybjsh, CliUtils.fLoginUser, ref   ylzfy, ref   sgfwnfy, ref   zfufy, ref   zfefy, ref   qfbz, ref    tcjjzf, ref   tcjjgrzf, ref   dejjzf, ref   dejjgrzf,
            ref   gwybzjjzf, ref   gwybzjjgrzf, ref   qtjjzf, ref   zhzf, ref   cxzhzf, ref   xjzf);

       
            List<string> liSQL = new List<string>();


            if (i == 0)
            {
                WriteLog(sysdate + "  住院费用结算撤销成功|出参|");
                strSql = string.Format(@"insert into ybfyjsdr (jzlsh,jylsh,djhin,cyrq,cyyy,bzbm,bzmc,yllb,xm,kh,
                                        grbh,ylfze,zbxje,tcjjzf,dejjzf,dbbyzf,mzjzfy,zhzf,xjzf,zhzfzf,sgfwnfy,tcjjgrzf,dejjgrzf,gwybzjjgrzf,cxzhzf,ybjsh,
                                        zhzfzl,xjzhzf,xjzhzl,ybfwnfy,bcjsqzhye,dbzbc,smxx,yfhj,zlxmfhj,bbzf,
                                        bz6,jsrq,jbr,djh,cxbz,sysdate) select jzlsh,jylsh,djhin,cyrq,cyyy,bzbm,bzmc,yllb,xm,kh,
                                        grbh,ylfze,zbxje,tcjjzf,dejjzf,dbbyzf,mzjzfy,zhzf,xjzf,zhzfzf,sgfwnfy,tcjjgrzf,dejjgrzf,gwybzjjgrzf,cxzhzf,ybjsh,
                                        zhzfzl,xjzhzf,xjzhzl,ybfwnfy,bcjsqzhye,dbzbc,smxx,yfhj,zlxmfhj,bbzf,
                                        bz6,jsrq,djh,'{3}',0,'{2}' from ybfyjsdr where jzlsh='{0}' and djhin='{1}' and cxbz=1", jzlsh, djh, sysdate, CliUtils.fUserName);
                liSQL.Add(strSql);
                strSql = string.Format(@"update ybfyjsdr set cxbz=2 where jzlsh='{0}' and djhin='{1}' and cxbz=1", jzlsh, djh);
                liSQL.Add(strSql);
                object[] obj = liSQL.ToArray();
                obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);
                if (obj[1].ToString().Equals("1"))
                {
                    WriteLog(sysdate + "  住院费用结算撤销成功|数据操作成功|");
                    //门诊费用登记撤销
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
                WriteLog(sysdate + "  住院费用结算撤销失败|");
                return new object[] { 0, 0, "住院费用结算撤销失败|"};
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

        #region 门诊/住院费用结算撤销(内部)
        public static object[] N_YBMZZYFYJSCX(object[] objParam)
        {
            string sysdate = GetServerDateTime();//当前时间
            string jzlsh = objParam[0].ToString();   // 就诊流水号
            string ybjsh = objParam[1].ToString();     // 医保结账流水号
            string ybjzlsh = objParam[2].ToString();

            JYLSH = DateTime.Now.ToString("yyyyMMddHHmmss") + "-" + YLGHBH + "-" + new Random().Next(100).ToString().PadLeft(4, '0');


            if (string.IsNullOrEmpty(jzlsh))
                return new object[] { 0, 0, "就诊流水号不能为空" };


            string jscxrq = Convert.ToDateTime(sysdate).ToString("yyyyMMddHHmmss");//结算撤销日期


            #region 业务出参
            float ylzfy = 0;  //医疗总费用
            float sgfwnfy = 0;  //三个范围内费用
            float zfufy = 0;   //自付费用
            float zfefy = 0;   //自费费用
            float qfbz = 0;    //起付标准
            float tcjjzf = 0;  //统筹支付
            float tcjjgrzf = 0;    //统筹个人自负
            float dejjzf = 0;    //大病救助基金支付
            float dejjgrzf = 0;     //大病救助基金个人自负
            float gwybzjjzf = 0;  //公务员/企业补充支付
            float gwybzjjgrzf = 0;     //公务员/企业个人自负
            float qtjjzf = 0;     //其它基金支付
            float zhzf = 0;    //医疗账户支付
            float cxzhzf = 0;   //储蓄账户支付
            float xjzf = 0;    //现金支付
            #endregion

            int i = FCancelOutpatBalance(ybjzlsh, ybjsh, CliUtils.fLoginUser, ref   ylzfy, ref   sgfwnfy, ref   zfufy, ref   zfefy, ref   qfbz, ref    tcjjzf, ref   tcjjgrzf, ref   dejjzf, ref   dejjgrzf,
            ref   gwybzjjzf, ref   gwybzjjgrzf, ref   qtjjzf, ref   zhzf, ref   cxzhzf, ref   xjzf);

            if (i == 0)
            {
                WriteLog(sysdate + "  门诊费用结算撤销成功|出参|");
                return new object[] { 0, 1, "门诊/住院费用结算撤销成功" };
            }
            else
            {
                WriteLog(sysdate + "  门诊/住院费用结算撤销失败|");
                return new object[] { 0, 0, "门诊/住院费用结算撤销失败|" };
            }
        }
        #endregion

        #region 门诊/住院登记撤销(内部)
        public static object[] N_YBMZZYDJCX(object[] objParam)
        {
            string sysdate=GetServerDateTime();
            string ybjzlsh = objParam[0].ToString();
            

            JYLSH = DateTime.Now.ToString("yyyyMMddHHmmss") + "-" + YLGHBH + "-" + new Random().Next(100).ToString().PadLeft(4, '0');
            CZYBH = CliUtils.fLoginUser;
            YWZQH = CliUtils.fLoginYbNo.Split('|')[0];
            string jbr = CliUtils.fUserName;
            if (string.IsNullOrEmpty(ybjzlsh))
                return new object[] { 0, 0, "就诊流水号不能为空" };

            int i = FCancelInpatReg(ybjzlsh, CliUtils.fLoginUser);
            if (i == 0)
            {
                WriteLog(sysdate + "  医保门诊/住院登记撤销(内部)成功|");
                    return new object[] { 0, 1, "医保门诊登记撤销(内部)成功" };
            }
            else
            {
                WriteLog(sysdate + "  医保门诊/住院登记撤销(内部)失败|" );
                return new object[] { 0, 0, "  医保门诊/住院登记撤销(内部)失败|" };
            }
        }
        #endregion

        #region 门诊/住院费用登记撤销(内部)
        public static object[] N_YBFYDJCX(object[] objParam)
        {
            string sysdate = GetServerDateTime();
            string ybjzlsh = objParam[0].ToString();




            int i = FSynData(2, ybjzlsh);

            if (i == 0)
            {
                WriteLog(sysdate + "  门诊/住院费用登记撤销(内部)成功|");
                return new object[] { 0, 1, "门诊费用登记撤销(内部)成功" };
            }
            else
            {
                WriteLog(sysdate + "  门诊/住院费用登记撤销(内部)失败|");
                return new object[] { 0, 0, "门诊/住院费用登记撤销(内部)失败|数据操作失败|" };
            }

        }
        #endregion

        #region 获取病种信息查询
        public static object[] YBBZCX(object[] objParam)
        {
           
            return new object[] { 0, 0, null };
        }
        #endregion

        #region 上传医保本地库数据
        public static object[] YBSC(object[] objParam)
        {
            string ybjzlsh = objParam[0].ToString();

            int i = FUpLoad(2, ybjzlsh);
            if (i == 0)
            {
                return new object[] { 0, 1, "上传医保本地库成功" };
            }
            else
            {
                return new object[] { 0, 0, "上传医保本地库失败" };
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
