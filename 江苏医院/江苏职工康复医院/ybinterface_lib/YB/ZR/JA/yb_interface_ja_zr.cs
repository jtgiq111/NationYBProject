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

//吉安地区医保接口文件
namespace ybinterface_lib
{
    public class yb_interface_ja_zr
    {

        public static string dqbh = "360821"; //吉安县

        #region 接口DLL加载
        [DllImport("ZRHosJK.dll", CharSet = CharSet.Ansi)]
        public static extern int f_UserBargaingInit(string UserID, string PassWD, StringBuilder retMsg);

        [DllImport("ZRHosJK.dll", CharSet = CharSet.Ansi)]
        public static extern int f_UserBargaingClose(StringBuilder retMsg);

        [DllImport("ZRHosJK.dll", CallingConvention = CallingConvention.StdCall)]
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

            WriteLog(sysdate + "  入参|用户" + UserID + "");
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
            try
            {
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
                    /*保险号|姓名|卡号|地区编号|地区名称|出生日期|实际年龄|参保日期|个人身份|
                 * 单位名称|性别|医疗人员类别|卡状态|账户余额|累计住院支付|累计门诊支付|
                 * 累计特殊门诊|身份号码
                 */
                    string[] str = outData.ToString().Split('|');
                    if (str.Length < 10)
                        return new object[] { 0, 0, outData.ToString() + retMsg.ToString() };
                    outParams_dk dk = new outParams_dk();
                    dk.Grbh = str[0].Trim();     //保险号
                    dk.Xm = str[1].Trim();      //姓名
                    dk.Kh = str[2].Trim();      //卡号
                    dk.Tcqh = str[3].Trim();    //地区编号
                    dk.Qxmc = str[4].Trim();    //地区名称
                    dk.Csrq = str[5].Trim();    //出生日期
                    dk.Nl = str[6].Trim();    //实际年龄
                    dk.Cbrq = str[7].Trim();    //参保日期
                    dk.Grsf = str[8].Trim();    //个人身份
                    dk.Dwmc = str[9].Trim();    //单位名称
                    dk.Xb = str[10].Trim();     //性别
                    dk.Ylrylb = str[11].Trim();   //医疗人员类别
                    dk.Zkt = str[12].Trim();    //卡状态
                    dk.Zhye = str[13].Trim(); //账户余额
                    dk.Ljzyzf = str[14].Trim();  //累计住院支付
                    dk.Ljmzzf = str[15].Trim();  //累计门诊支付
                    dk.Ljtsmzzf = str[16].Trim();//累计特殊门诊
                    dk.Sfhz = str[17].Trim(); //身份号码
                    string YBKLX = "0";
                    dk.Yllb = "11";
                    dk.Mtbz = "0";
                    dk.Yldylb = dk.Ylrylb;
                    if (dk.Xb.Equals("男"))
                        dk.Xb = "1";
                    else
                        dk.Xb = "2";
                    dk.Csrq = dk.Csrq.Substring(0, 4) + "-" + dk.Csrq.Substring(4, 2) + "-" + dk.Csrq.Substring(6, 2);
                    dk.Rycbzt = dk.Zkt;
                    dk.Ydrybz = "0";
                    if (dk.Ylrylb.Contains("居民"))
                        dk.Jflx = "0203";
                    else
                        dk.Jflx = "0202";
                    /*
                    * 个人编号|单位编号|身份证号|姓名|性别|
                    * 民族|出生日期|社会保障卡卡号|医疗待遇类别|人员参保状态|
                    * 异地人员标志|统筹区号|年度|在院状态|帐户余额|
                    * 本年医疗费累计|本年帐户支出累计|本年统筹支出累计|本年救助金支出累计|本年公务员补助基金累计|
                    * 本年城镇居民门诊统筹支付累计|进入统筹费用累计|进入救助金费用累计|起付标准累计|本年住院次数|
                    * 单位名称|年龄|参保单位类型|经办机构编码|缴费类型|
                    * 医保门慢、特资质|医保门慢、特病种说明|医疗类别代码|慢、特病编码|慢、特病名称
                    */
                    string outParams = dk.Grbh + "|" + dk.Dwbh + "|" + dk.Sfhz + "|" + dk.Xm + "|" + dk.Xb + "|" +
                                       dk.Mz + "|" + dk.Csrq + "|" + dk.Kh + "|" + dk.Yldylb + "|" + dk.Rycbzt + "|" +
                                       dk.Ydrybz + "|" + dk.Tcqh + "|" + dk.Nd + "|" + dk.Zyzt + "|" + dk.Zhye + "|" +
                                       dk.Bnylflj + "|" + dk.Bnzhzclj + "|" + dk.Bntczclj + "|" + dk.Bndbyljjzflj + "|" + dk.Bngwybzjjlj + "|" +
                                       dk.Bnczjmmztczflj + "|" + dk.Jrtcfylj + "|" + dk.Jrdbfylj + "|" + dk.Qfbzlj + "|" + dk.Bnzycs + "|" +
                                       dk.Dwmc + "|" + dk.Nl + "|" + dk.Cbdwlx + "|" + dk.Jbjgbm + "|" + dk.Jflx + "|" +
                                       dk.Mtbz + "|" + dk.Mtmsg + "|" + dk.Yllb + "|" + dk.Mtbzbm + "|" + dk.Mtbzmc + "|" + YBKLX + "|";
                    #endregion

                    string strSql = string.Format("delete from YBICKXX where grbh='{0}'", dk.Grbh);
                    liSQL.Add(strSql);
                    strSql = string.Format(@"insert into YBICKXX(grbh,xm,kh,dqbh,qxmc,csrq,sjnl,cbrq,grsf,dwmc,
                                        xb,rylb,kzt,grzhye,ljzyf,ljmzf,ljtsmzf,gmsfhm) values(
                                        '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}',
                                        '{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}')",
                                        dk.Grbh, dk.Xm, dk.Kh, dk.Tcqh, dk.Qxmc, dk.Csrq, dk.Nl, dk.Cbrq, dk.Grsf, dk.Dwmc,
                                        dk.Xb, dk.Ylrylb, dk.Zkt, dk.Zhye, dk.Ljzyzf, dk.Ljmzzf, dk.Ljtsmzzf, dk.Sfhz);
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
                WriteLog(sysdate + "  门诊读卡异常|" + ex.Message);
                return new object[] { 0, 0, "门诊读卡异常|" + ex.Message };
            }
        }
        #endregion

        #region 门诊登记(挂号)
        public static object[] YBMZDJ(object[] objParam)
        {
            string sysdate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            try
            {
                string Ywlx = "MZGH";

                string jzlsh = objParam[0].ToString(); //就诊流水号
                string yllb = objParam[1].ToString(); //医疗类别代码
                string bzbm = objParam[2].ToString(); //病种编码
                string bzmc = objParam[3].ToString(); //病种名称
                string[] kxx = objParam[4].ToString().Split('|'); //读卡信息
                string ghdjsj = objParam[5].ToString(); //挂号日期时间(yyyy-MM-dd HH:mm:ss)
                string cfysdm = objParam[6].ToString(); //处方医生代码
                string cfysxm = objParam[7].ToString(); //处方医生姓名
                string dgysdm = ""; //定岗医生代码
                string dgysxm = ""; //定岗医生姓名

                #region 获取读卡信息
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

                string ksbh = "";   //科室编号
                string ksmc = "";   //科室名称
                string ghfy = "0.00";   //挂号费
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

                //if (!yllb.Equals("11"))
                //{
                //    if (string.IsNullOrEmpty(bzbm))
                //        return new object[] { 0, 0, "慢性病种不能为空" };
                //}
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

                string jzlsh1 = "MZ" + yllb + jzlsh;

                #region 判断是否在HIS中挂号
                string strSql = string.Empty;
                strSql = string.Format("select a.m1name as name,a.m1ksno as ksno, (select top 1 b2ejnm from bz02d  where b2ksno=a.m1ksno) as ksmc from mz01t  a where a.m1ghno = '{0}'", jzlsh);
                DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                if (ds.Tables[0].Rows.Count == 0)
                    return new object[] { 0, 0, "该患者未挂号" };
                else
                {
                    hzname = ds.Tables[0].Rows[0]["name"].ToString();
                    ksbh = ds.Tables[0].Rows[0]["ksno"].ToString();
                    ksmc = ds.Tables[0].Rows[0]["ksmc"].ToString();
                }
                #endregion

                #region 判断是否进门诊挂号登记
                strSql = string.Format(@"select jzlsh from ybmzzydjdr where jzlsh1='{0}' and cxbz=1 ", jzlsh1);
                ds.Tables.Clear();
                ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                if (ds.Tables[0].Rows.Count > 0)
                    return new object[] { 0, 0, "该患者已经进行过医保门诊登记" };
                #endregion

                #region 获取定岗医生信息
                strSql = string.Format(@"select * from ybdgyszd where ysbm='{0}'", cfysdm);
                ds.Tables.Clear();
                ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                if (ds.Tables[0].Rows.Count == 0)
                {
                    //给定默认值
                    dgysdm = "30";
                    dgysxm = "肖东明";
                }
                else
                {
                    dgysdm = ds.Tables[0].Rows[0]["dgysbm"].ToString();
                    dgysxm = ds.Tables[0].Rows[0]["ysxm"].ToString();
                }
                #endregion
                ds.Dispose();

                #region 患者姓名与医保卡姓名是否一致
                if (!xm.Equals(hzname))
                    return new object[] { 0, 0, "医保卡与患者姓名不一致" };
                #endregion

                //入参: 保险号|姓名|卡号|地区编号|医疗类别|科室名称|挂号费|挂号日期|挂号时间|医院门诊流水号
                StringBuilder inputData = new StringBuilder();
                inputData.Append(grbh + "|");    //保险号
                inputData.Append(xm + "|");     //姓名
                inputData.Append(kh + "|");     //卡号
                inputData.Append(tcqh + "|");   //地区编号
                inputData.Append(yllb + "|");   //医疗类别
                inputData.Append(ksmc + "|");   //科室名称
                inputData.Append(ghfy + "|");   //挂号费
                inputData.Append(ghrq + "|");  //挂号日期
                inputData.Append(ghsj + "|");   //挂号时间
                inputData.Append(jzlsh);        //医院门诊流水号
                StringBuilder outData = new StringBuilder(1024);
                StringBuilder retMsg = new StringBuilder(1024);
                WriteLog(sysdate + "  " + jzlsh + " 进入门诊挂号...");
                WriteLog(sysdate + "  入参|" + inputData.ToString());
                int i = f_UserBargaingApply(Ywlx, inputData, outData, retMsg);
                if (i > 0)
                {
                    #region 出参
                    /*出参:保险号|姓名|卡号|地区编号|地区名称|出生日期|实际年龄|参保日期|个人身份|单位名称|
                 * 性别|医疗人员类别|卡状态|账户余额|门诊(住院号)|医疗类别|科室名称|挂号费|本次看病次数|
                 * 住院床号|入院日期|入院时间|经办人
                     * 
                     * 1230078452|肖兵|36082600599810|360826|泰和县|19841223|34|20071201|12441637|肖兵|男|居民-成年居民|正常|0|75097188|普通门诊|内二科|0|0|NULL|20180814|1907|朱友姬
                 */
                    List<string> liSQL = new List<string>();
                    string strValue = "";
                    string[] str = outData.ToString().Split(';');
                    string[] str2 = str[0].Split('|');
                    string bxh2 = str2[0].Trim();       //保险号
                    string xm2 = str2[1].Trim();        //姓名
                    string kh2 = str2[2].Trim();        //卡号
                    string dqbh1 = str2[3].Trim();      //地区编号
                    string dqmc = str2[4].Trim();       //地区名称
                    string csrq1 = str2[5].Trim();       //出生日期
                    string sjnl = str2[6].Trim();       //实际年龄
                    string cbrq = str2[7].Trim();       //参保日期
                    string grsf = str2[8].Trim();       //个人身份
                    string dwmc1 = str2[9].Trim();       //单位名称
                    string xb1 = str2[10].Trim();        //性别
                    string rylb = str2[11].Trim();      //医疗人员类别
                    string kzt = str2[12].Trim();       //卡状态
                    string grzhye = str2[13].Trim();    //账户余额
                    string jzlsh2 = str2[14].Trim();    //门诊(住院号)
                    string yllb2 = str2[15].Trim();     //医疗类别
                    string ksmc2 = str2[16].Trim();     //科室名称
                    string ghf = str2[17].Trim();       //挂号费
                    string bckbcs = str2[18].Trim();    //本次看病次数
                    string zych = str2[19].Trim();      //住院床号
                    string djrq = str2[20].Trim();      //入院日期
                    string djsj = str2[21].Trim();      //入院时间
                    string jbr = str2[22].Trim();       //经办人
                    if (string.IsNullOrEmpty(ghf)) //挂号费为NULL时，传入0
                        ghf = "0.00";
                    #endregion

                    string bzbm_r = string.Empty;
                    string bzmc_r = string.Empty;
                    if (str.Length > 1)
                    {
                        for (int j = 1; j < str.Length; j++)
                        {
                            str2 = str[j].Split('|');
                            bzbm_r = str2[0].Trim();
                            bzmc_r = str2[1].Trim();
                            bzbm = bzbm_r;
                            bzmc = bzmc_r;
                            strValue += "病种编码:【" + bzbm_r + "】 病种名称:【" + bzmc_r + "】\r\n";
                            strSql = string.Format(@"insert into ybmxbdj (jzlsh,bxh,xm,kh,mmbzbm,mmbzmc) values(
                                                '{0}','{1}','{2}','{3}','{4}','{5}')",
                                                    jzlsh, bxh2, xm2, kh2, bzbm_r, bzmc_r);
                            liSQL.Add(strSql);
                        }
                    }

                    strSql = string.Format(@"insert into ybmzzydjdr(grbh,xm,kh,tcqh,bq,csrq,nl,nd,sfzh,dwmc,
                                        xb,yldylb,rycbzt,zhye,ybjzlsh,yllb,ksmc,ghf,zycs,bnzycs,
                                        ghdjsj,jbr,dgysdm,dgysxm,ksbh,jzbz,sysdate,jzlsh,bzbm,bzmc,
                                        jylsh,mmbzbm1,mmbzmc1,ysdm,ysxm,jzlsh1) values(
                                       '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}',
                                       '{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}',
                                       '{20}','{21}','{22}','{23}','{24}','{25}','{26}','{27}','{28}','{29}',
                                       '{30}','{31}','{32}','{33}','{34}','{35}')",
                                        bxh2, xm2, kh2, tcqh, dqmc, csrq, sjnl, "", sfzh, dwmc1,
                                        xb1, yldylb, kzt, grzhye, jzlsh2, yllb, ksmc2, ghf, bckbcs, zych,
                                        ghdjsj, jbr, dgysdm, dgysxm, ksbh, "m", sysdate, jzlsh, bzbm, bzmc,
                                        jzlsh2, bzbm_r, bzmc_r, cfysdm, cfysxm,jzlsh1);
                    liSQL.Add(strSql);

                    if (!string.IsNullOrEmpty(strValue) && yllb.Equals("11"))
                    {
                        DialogResult result = MessageBox.Show("注:慢病患者!\r\n" + strValue + "当前医疗类别[普通门诊],与慢病类别不符\r\n是否继续当前挂号?\r\n点击[是] 继续操作  点击[否] 取销当前操作", "门诊医保登记", MessageBoxButtons.YesNo, MessageBoxIcon.Information, MessageBoxDefaultButton.Button2);
                        if (result == DialogResult.Yes)
                        {
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
                                object[] objParam2 = new object[] { jzlsh, bxh2, xm2, kh2, dqbh1, jzlsh2 };
                                NYBMZDJCX(objParam2);
                                return new object[] { 0, 0, "门诊挂号失败" };
                            }
                        }
                        else
                        {
                            //撤销当前挂号 
                            //门诊登记（挂号）撤销
                            object[] objParam2 = new object[] { jzlsh, bxh2, xm2, kh2, dqbh1, jzlsh2 };
                            objParam2 = NYBMZDJCX(objParam2);
                            if (objParam2[1].ToString().Equals("1"))
                                return new object[] { 0, 0, "当前医保登记撤销成功!" };
                            else
                                return new object[] { 0, 0, "当前医保登记撤销失败!" };
                        }
                    }
                    else
                    {
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
                            object[] objParam2 = new object[] { jzlsh };
                            objParam2 = YBMZDJCX(objParam2);
                            return new object[] { 0, 0, "门诊挂号失败" };
                        }
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
                WriteLog(sysdate + "  门诊读卡异常|" + ex.Message);
                return new object[] { 0, 0, "门诊读卡异常|" + ex.Message };
            }
        }
        #endregion

        #region 门诊登记(挂号)撤销
        public static object[] YBMZDJCX(object[] objParam)
        {
            string sysdate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            try
            {
                string Ywlx = "MZGHCX";
                string jzlsh = objParam[0].ToString();  //就诊流水号
                string jbr = CliUtils.fUserName;    //经办人
                string bxh = "";    //保险号
                string xm = "";     //姓名
                string kh = "";
                string ybjzlsh = "";
                string yllb = "";
                if (string.IsNullOrEmpty(jzlsh))
                    return new object[] { 0, 0, "就诊流水号不能为空" };

                string cxrq = Convert.ToDateTime(sysdate).ToString("yyyyMMdd");
                string cxsj2 = Convert.ToDateTime(sysdate).ToString("HHmm");

                string strSql = string.Format(@"select a.* from ybmzzydjdr a where a.jzlsh='{0}' and a.cxbz=1 
                                                and not exists(select 1 from ybfyjsdr b where a.ybjzlsh=b.ybjzlsh and b.cxbz=1) ", jzlsh);
                DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                if (ds.Tables[0].Rows.Count == 0)
                    return new object[] { 0, 0, "该患者未办理挂号登记或已收费不能撤销登记信息" };

                List<string> liSQL = new List<string>();
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    bxh = dr["grbh"].ToString();
                    xm = dr["xm"].ToString();
                    kh = dr["kh"].ToString();
                    dqbh = dr["tcqh"].ToString();
                    ybjzlsh = dr["ybjzlsh"].ToString();
                    jbr = dr["jbr"].ToString();
                    yllb = dr["yllb"].ToString();

                    //strSql = string.Format(@"select * from ybfyjsdr where ybjzlsh='{0}' and cxbz=1", ybjzlsh);
                    //ds.Tables.Clear();
                    //ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                    //if (ds.Tables[0].Rows.Count > 0)
                    //    return new object[] { 0, 0, "该患者已进行医保收费，不能撤销医保登记！" };
                    
                    //保险号|姓名|卡号|地区编号|门诊号
                    StringBuilder inputData = new StringBuilder();
                    inputData.Append(bxh + "|");
                    inputData.Append(xm + "|");
                    inputData.Append(kh + "|");
                    inputData.Append(dqbh + "|");
                    inputData.Append(ybjzlsh);
                    StringBuilder outData = new StringBuilder(1024);
                    StringBuilder retMsg = new StringBuilder(1024);

                    WriteLog(sysdate + "  " + jzlsh + " 进入门诊挂号撤销...");
                    WriteLog(sysdate + "  入参|" + inputData.ToString());
                    int i = f_UserBargaingApply(Ywlx, inputData, outData, retMsg);
                    if (i > 0)
                    {
                        strSql = string.Format(@"insert into ybmzzydjdr(
                                           grbh,xm,kh,tcqh,bq,csrq,nl,nd,sfzh,dwmc,
                                           xb,yldylb,rycbzt,zhye,ybjzlsh,yllb,ksmc,ghf,zycs,bnzycs,
                                           ghdjsj,jbr,dgysdm,dgysxm,ksbh,jzbz,jzlsh,bzbm,bzmc,
                                           jylsh,mmbzbm1,mmbzmc1,ysdm,ysxm,sysdate,cxbz) 
                                           select 
                                           grbh,xm,kh,tcqh,bq,csrq,nl,nd,sfzh,dwmc,
                                           xb,yldylb,rycbzt,zhye,ybjzlsh,yllb,ksmc,ghf,zycs,bnzycs,
                                           ghdjsj,jbr,dgysdm,dgysxm,ksbh,jzbz,jzlsh,bzbm,bzmc,
                                           jylsh,mmbzbm1,mmbzmc1,ysdm,ysxm,'{2}',0 from ybmzzydjdr 
                                           where jzlsh='{0}' and ybjzlsh='{1}' and cxbz=1 ", jzlsh, ybjzlsh, sysdate);
                        liSQL.Add(strSql);
                        strSql = string.Format(@"update ybmzzydjdr set cxbz =2 where jzlsh = '{0}' and ybjzlsh='{1}' and cxbz=1", jzlsh, ybjzlsh);
                        liSQL.Add(strSql);
                        strSql = string.Format(@"delete from ybmxbdj where jzlsh='{0}'", jzlsh);
                        liSQL.Add(strSql);
                        strSql = string.Format(@"update mz01h set m1kind='0201'  where m1ghno='{0}'", jzlsh);
                        liSQL.Add(strSql);
                    }
                    else
                    {
                        WriteLog(sysdate + "  " + jzlsh + " 进入门诊挂号撤销失败|" + retMsg.ToString());
                        return new object[] { 0, 0, "门诊挂号撤销失败|" + retMsg.ToString() };
                    }
                }

                ds.Dispose();
                object[] obj = liSQL.ToArray();
                obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);
                if (obj[1].ToString() == "1")
                {
                    WriteLog(sysdate + "  " + jzlsh + " 进入门诊挂号撤销成功|" + obj[2].ToString());
                    return new object[] { 0, 1, "门诊挂号撤销成功", obj[2].ToString() };
                }
                else
                {
                    WriteLog(sysdate + "  " + jzlsh + " 进入门诊挂号撤销成功|操作本地数据失败|" + obj[2].ToString());
                    return new object[] { 0, 2, "门诊挂号撤销失败|" + obj[2].ToString() };
                }
            }
            catch (Exception ex)
            {
                WriteLog(sysdate + "  门诊登记(挂号)撤销异常|" + ex.Message);
                return new object[] { 0, 0, "门诊登记(挂号)撤销异常|" + ex.Message };

            }
        }
        #endregion

        #region 门诊登记（挂号）收费
        //入参:就诊流水号|医疗类别代码|结算时间|病种编码|病种名称|卡信息|读卡标志|定岗医生代码|定岗医生姓名
        public static object[] YBMZDJSF(object[] objParam)
        {
            string sysdate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            try
            {
                string jzlsh = objParam[0].ToString();
                string yllb = objParam[1].ToString();
                string jssj = objParam[2].ToString();
                string bzbm = objParam[3].ToString();
                string bzmc = objParam[4].ToString();
                string kxx = objParam[5].ToString();
                string dkbz = objParam[6].ToString();
                string dgysdm = objParam[7].ToString();
                string dgysxm = objParam[8].ToString();
                object[] objParam1 = { jzlsh, yllb, bzbm, bzmc, kxx, jssj, dgysdm, dgysxm };
                objParam1 = YBMZDJ(objParam1);
                if (objParam1[1].ToString().Equals("1"))
                {

                    string jzlsh1 = "MZ" + yllb + jzlsh;
                    //门诊登记成功,进入门诊登记（挂号）收费结算
                    #region 获取门诊登记信息
                    string strSql = string.Format("select * from ybmzzydjdr where jzlsh1='{0}' and cxbz=1", jzlsh1);
                    DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                    if (ds.Tables[0].Rows.Count == 0)
                        return new object[] { 0, 0, "该患者未做医保登记" };
                    string bxh = ds.Tables[0].Rows[0]["grbh"].ToString();
                    string xm = ds.Tables[0].Rows[0]["xm"].ToString();
                    string kh = ds.Tables[0].Rows[0]["kh"].ToString();
                    string dqbh = ds.Tables[0].Rows[0]["tcqh"].ToString();
                    string dgysxm1 = ds.Tables[0].Rows[0]["dgysxm"].ToString();
                    string ybjzlsh = ds.Tables[0].Rows[0]["ybjzlsh"].ToString();
                    string bzbm1 = "NULL";
                    string bzmc1 = "NULL";
                    if (!yllb.Equals("11"))
                    {
                        bzbm1 = ds.Tables[0].Rows[0]["bzbm"].ToString();
                        bzmc1 = ds.Tables[0].Rows[0]["bzmc"].ToString();
                    }
                    string sfdypj = "TRUE"; //是否打印票据
                    StringBuilder inputData = new StringBuilder();
                    //入参:保险号|姓名|卡号|地区编号|门诊号|病种编号|病种名称|开方医生|是否打印票据
                    inputData.Append(bxh + "|");
                    inputData.Append(xm + "|");
                    inputData.Append(kh + "|");
                    inputData.Append(dqbh + "|");
                    inputData.Append(ybjzlsh + "|");
                    inputData.Append(bzbm1 + "|");
                    inputData.Append(bzmc1 + "|");
                    inputData.Append(dgysxm1 + "|");
                    inputData.Append(sfdypj + ";");
                    #endregion

                    #region 获取挂号明细信息
                    string ybxmbh = string.Empty;
                    string ybxmmc = string.Empty;
                    double dj = 0.00;
                    double sl = 0.00;
                    double je = 0.00;
                    string yyxmxx = string.Empty;
                    double blbfy = 0.00; //病历本费用
                    double ylkfy = 0.00; //就诊卡费用
                    double ghfy = 0.00;   //挂号总费用
                    double ghfylj = 0.00; //挂号费用累计
                    string djhin = string.Empty; //单据号(发票号)
                    string cfrq = DateTime.Now.ToString("yyyyMMdd");

                    List<string> li_yyxmbh = new List<string>();
                    List<string> li_yyxmmc = new List<string>();
                    List<string> li_ybxmbh = new List<string>();
                    List<string> li_je = new List<string>();
                    List<string> li_cfh = new List<string>();

                    #region 获取诊查费用信息
                    strSql = string.Format(@"select  z.ybxmbh ybxmbh, z.ybxmmc ybxmmc, c.bzmem1 as dj,1 as sl,a.m1gham je,c.bzmem2 yyxmbh, c.bzname yyxmmc, a.m1invo,a.m1blam,a.m1kham,a.m1amnt
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
                    ybxmbh = dr["ybxmbh"].ToString();
                    ybxmmc = dr["ybxmmc"].ToString();
                    dj = Convert.ToDouble(dr["dj"].ToString());
                    sl = Convert.ToDouble(dr["sl"].ToString());
                    je = Convert.ToDouble(dr["je"].ToString());
                    yyxmxx = dr["yyxmbh"].ToString() + "&" + dr["yyxmmc"].ToString();
                    blbfy = Convert.ToDouble(dr["m1blam"].ToString());
                    ylkfy = Convert.ToDouble(dr["m1blam"].ToString());
                    ghfy = Convert.ToDouble(dr["m1amnt"].ToString());
                    ghfylj += je;
                    li_yyxmbh.Add(dr["yyxmbh"].ToString());
                    li_yyxmmc.Add(dr["yyxmmc"].ToString());
                    li_ybxmbh.Add(dr["ybxmbh"].ToString());

                    if (je > 0)
                    {
                        //明细入参:项目编号|项目名称|单价|数量|金额|处方日期|医院项目信息
                        inputData.Append(ybxmbh + "|");
                        inputData.Append(ybxmmc + "|");
                        inputData.Append(dj + "|");
                        inputData.Append(sl + "|");
                        inputData.Append(je + "|");
                        inputData.Append(cfrq + "|");
                        inputData.Append(yyxmxx + ";");
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
                        yyxmxx = dr["yyxmbh"].ToString() + "&" + dr["yyxmmc"].ToString();
                        ghfylj += je;

                        li_yyxmbh.Add(dr["yyxmbh"].ToString());
                        li_yyxmmc.Add(dr["yyxmmc"].ToString());
                        li_ybxmbh.Add(dr["ybxmbh"].ToString());

                        //明细入参:项目编号|项目名称|单价|数量|金额|处方日期|医院项目信息
                        inputData.Append(ybxmbh + "|");
                        inputData.Append(ybxmmc + "|");
                        inputData.Append(dj + "|");
                        inputData.Append(sl + "|");
                        inputData.Append(je + "|");
                        inputData.Append(cfrq + "|");
                        inputData.Append(yyxmxx + ";");
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
                        yyxmxx = dr["yyxmbh"].ToString() + "&" + dr["yyxmmc"].ToString();
                        ghfylj += je;

                        li_yyxmbh.Add(dr["yyxmbh"].ToString());
                        li_yyxmmc.Add(dr["yyxmmc"].ToString());
                        li_ybxmbh.Add(dr["ybxmbh"].ToString());

                        //明细入参:项目编号|项目名称|单价|数量|金额|处方日期|医院项目信息
                        inputData.Append(ybxmbh + "|");
                        inputData.Append(ybxmmc + "|");
                        inputData.Append(dj + "|");
                        inputData.Append(sl + "|");
                        inputData.Append(je + "|");
                        inputData.Append(cfrq + "|");
                        inputData.Append(yyxmxx + ";");
                    }
                    #endregion

                    ds.Dispose();
                    #endregion

                    //判断总费用与累计费用是否相等
                    if (Math.Abs(ghfy - ghfylj) > 1.0)
                        return new object[] { 0, 0, "收费金额与医保结算金额相差" + Math.Abs(ghfy - ghfylj) + ",无法结算，请核实!" };

                    #region 费用结算
                    string Ywlx = "MZSF";
                    StringBuilder outData = new StringBuilder(10240);
                    StringBuilder retMsg = new StringBuilder(1024);
                    WriteLog(sysdate + "  " + jzlsh + " 进入门诊登记（挂号）收费结算...");
                    WriteLog(sysdate + "  入参|" + inputData.ToString());
                    int i = f_UserBargaingApply(Ywlx, inputData, outData, retMsg);

                    WriteLog(i.ToString() + "|" + outData.ToString() + "|" + retMsg.ToString());
                    if (i > 0)
                    {
                        List<string> liSQL = new List<string>();
                        #region 出参
                        /*出参:保险号|姓名|卡号|地区编号|地区名称|出生日期|实际年龄|参保日期|个人身份|单位名称|
                     * 性别|医疗人员类别|卡状态|账户余额|门诊(住院)号|单据流水号|医疗类别|科室名称|本次看病次数|住院床号|入院日期|
                     * 入院时间|出院日期|出院时间|出院原因|医疗总费用|本次账户支付|本次现金支付|本次基金支付|大病基金支付|救助金额|
                     * 单位补充医保支付|离休干部单独统筹支付|甲类费用|乙类费用|丙类费用|自费费用|结算人|结算日期|结算时间|起付标准自付|
                     * 非医保自付|乙类药品自付|特检特治自付|进入统筹自付|进入大病自付|重大疾病范围内补偿金额|重大疾病范围外补偿金额|
                     * 医院负担金额|大病二次补偿|民政大病救助基金|政府兜底基金|其中公务员补助部分
                     */
                        string[] str = outData.ToString().Split(';');
                        string[] str2 = str[0].Split('|');
                        string bxh2 = str2[0].Trim();       //保险号
                        string xm2 = str2[1].Trim();        //姓名
                        string kh2 = str2[2].Trim();        //卡号
                        string dqbh2 = str2[3].Trim();      //地区编号
                        string dqmc = str2[4].Trim();       //地区名称
                        string csrq = str2[5].Trim();       //出生日期
                        string sjnl = str2[6].Trim();       //实际年龄
                        string cbrq = str2[7].Trim();       //参保日期
                        string grsf = str2[8].Trim();       //个人身份
                        string dwmc = str2[9].Trim();       //单位名称
                        string xb = str2[10].Trim();        //性别
                        string ylrylb = str2[11].Trim();    //医疗人员类别
                        string kzt = str2[12].Trim();       //卡状态
                        string zfye = str2[13].Trim();      //账户余额
                        string jzlsh2 = str2[14].Trim();    //门诊(住院)号
                        string jslsh = str2[15].Trim();     //单据流水号
                        string yllb2 = str2[16].Trim();      //医疗类别
                        string ksmc = str2[17].Trim();      //科室名称
                        string zycs = str2[18].Trim();      //本次看病次数
                        string zych = str2[19];      //住院床号
                        string ryrq = str2[20].Trim();      //入院日期
                        string rysj = str2[21].Trim();      //入院时间
                        string cyrq = str2[22];      //出院日期
                        string cysj = str2[23];      //出院时间
                        string cyyy = str2[24];      //出院原因
                        string ylfze = str2[25].Trim();     //医疗总费用
                        string zhzf = str2[26].Trim();      //本次账户支付
                        string xjzf = str2[27].Trim();      //本次现金支付
                        string tcjjzf = str2[28].Trim();    //本次基金支付
                        string dejjzf = str2[29].Trim();    //大病基金支付
                        string mzjzfy = str2[30].Trim();    //救助金额
                        string dwfdfy = str2[31].Trim();    //单位补充医保支付
                        string lxgbddtczf = str2[32].Trim();//离休干部单独统筹支付
                        string jlfy = str2[33].Trim();      //甲类费用
                        string ylfy = str2[34].Trim();      //乙类费用
                        string blfy = str2[35].Trim();      //丙类费用
                        string zffy = str2[36].Trim();      //自费费用
                        string jsr = str2[37].Trim();       //结算人
                        string jsrq = str2[38].Trim();      //结算日期
                        string jssj2 = str2[39].Trim();      //结算时间
                        string qfbzfy = str2[40].Trim();    //起付标准自付
                        string fybzf = str2[41].Trim();     //非医保自付
                        string ylypzf = str2[42].Trim();    //乙类药品自付
                        string tjtzzf = str2[43].Trim();    //特检特治自付
                        string jrtcfy = str2[44].Trim();    //进入统筹自付
                        string jrdebxfy = str2[45].Trim();  //进入大病自付
                        string zdjbfwnbcje = "0.00"; //重大疾病范围内补偿金额
                        string zdjbfwybcje = "0.00"; //重大疾病范围外补偿金额
                        string yyfdje = "0.00"; //医院负担金额
                        string dbecbc = "0.00"; //大病二次补偿
                        string mzdbjzjj = "0.00"; //民政大病救助基金
                        string zftdjj = "0.00"; //政府兜底基金
                        string gwybzfy = "0.00";   //其中公务员补助部分
                        double bcjsqzhye = Convert.ToDouble(zhzf) + Convert.ToDouble(zfye);
                        string jbr = CliUtils.fLoginUser;
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
                     * 居民个人自付二次补偿金额|体检金额|生育基金支付|其他医保支付|
                     */

                        string strValue = ylfze + "|0.00|" + tcjjzf + "|" + dejjzf + "|" + zhzf + "|" + xjzf + "|" + gwybzfy + "|" + dwfdfy + "|" + zffy + "|" + dwfdfy + "|" +
                                        yyfdje + "|" + mzjzfy + "|0.00|0.00|0.00|0.00|" + qfbzfy + "|0.00|" + jrtcfy + "|0.00|" +
                                        0.00 + "|" + jrdebxfy + "|0.00|0.00|0.00|" + bcjsqzhye + "|0.00|0.00|0.00|0.00|" +
                                        0.00 + "|" + 0.00 + "|" + zycs + "|" + xm2 + "|" + jsrq + "|" + yllb2 + "|" + ylrylb + "|" + dqbh2 + "||" + jslsh + "||" +
                                        djhin + "|||1||" + dqbh2 + "|" + dbecbc + "|" + 0.00 + "|0.00|" + bxh2 + "||" +
                                        zftdjj + "|" + 0.00 + "|" + 0.00 + "|" + 0.00 + "|" + 0.00 + "|" + 0.00 + "|";
                        strSql = string.Format(@"insert into ybfyjsdr(jzlsh,jylsh,djhin,cyrq,cyyy,zhsybz,ztjsbz,jbr,xm,kh,
                                            grbh,jsrq,yllb,yldylb,jslsh,ylfze,zhzf,xjzf,tcjjzf,dejjzf,
                                            mzjzfy,dwfdfy,lxgbddtczf,jlfy,ylfy,blfy,zffy,qfbzfy,fybzf,ylypzf,
                                            tjtzzf,jrtczf,jrdbzf,zdjbfwnbcje,zdjbfwybcje,yyfdfy,ecbcje,mzdbjzje,czzcje,gwybzjjzf,
				                            sysdate,djh) values(
                                            '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}',
                                            '{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}',
                                            '{20}','{21}','{22}','{23}','{24}','{25}','{26}','{27}','{28}','{29}',
                                            '{30}','{31}','{32}','{33}','{34}','{35}','{36}','{37}','{38}','{39}',
                                            '{40}','{41}')",
                                               jzlsh, jslsh, djhin, cyrq, cyyy, 1, 0, jbr, xm2, kh2,
                                               bxh2, jsrq, yllb2, ylrylb, jslsh, ylfze, zhzf, xjzf, tcjjzf, dejjzf,
                                               mzjzfy, dwfdfy, lxgbddtczf, jlfy, ylfy, blfy, zffy, qfbzfy, fybzf, ylypzf,
                                               tjtzzf, jrtcfy, jrdebxfy, zdjbfwnbcje, zdjbfwybcje, yyfdje, dbecbc, mzdbjzjj, zftdjj, gwybzfy,
                                               sysdate, djhin);
                        liSQL.Add(strSql);

                        /*
                         * 门诊(住院)号|单据流水号|项目编号|项目名称|项目等级|收费类别|单价|数量|金额|自付金额
                         */
                        for (int j = 1; j < str.Length; j++)
                        {
                            str2 = str[j].Split('|');
                            string jzlsh3 = str2[0].Trim(); //门诊(住院)号
                            string djlsh = str2[1].Trim();  //单据流水号
                            string ybxmbh3 = str2[2].Trim(); //项目编号
                            string ybxmmc3 = str2[3].Trim(); //项目名称
                            string ybxmdj = str2[4].Trim(); //项目等级
                            string ybsflb = str2[5].Trim(); //收费类别
                            string dj3 = str2[6].Trim();     //单价
                            string sl3 = str2[7].Trim();     //数量
                            string je3 = str2[8].Trim();     //金额
                            string zfje = str2[9].Trim();   //自付金额
                            for (int k = 0; k < li_ybxmbh.Count; k++)
                            {
                                if (ybxmbh3.Equals(li_ybxmbh[k]))
                                {
                                    strSql = string.Format(@"insert into ybcfmxscindr(jzlsh,je,xm,kh,ybcfh,yysfxmbm,yysfxmmc,sysdate,ybjzlsh,djlsh,
                                                    sfxmzxbm,sfxmzxmc,sflb,sfxmzl,dj,sl,zfje,jsdjh) values(
                                                    '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}',
                                                    '{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}')",
                                                            jzlsh, je3, xm2, kh2, "", li_yyxmbh[k], li_yyxmmc[k], sysdate, jzlsh3, djlsh,
                                                            ybxmbh3, ybxmmc3, ybsflb, ybxmdj, dj3, sl3, zfje, djhin);
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
                            object[] objParam2 = new object[] { jzlsh, jslsh };
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
                WriteLog(sysdate + "  门诊登记（挂号）收费异常|" + ex.Message);
                return new object[] { 0, 0, "门诊登记（挂号）收费异常|" + ex.Message };
 
            }
        }
        #endregion

        #region 门诊登记（挂号）收费撤销
        public static object[] YBMZDJSFCX(object[] objParam)
        {
            string sysdate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            try
            {
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
                    //只做门诊登记（挂号）撤销
                    object[] objParam1 = new object[] { jzlsh };
                    return YBMZDJCX(objParam1);
                }
                else
                {
                    //进行门诊登记（挂号）收费，需撤销费用后再撤销登记
                    string djh = ds.Tables[0].Rows[0]["djhin"].ToString();
                    object[] objParam1 = new object[] { jzlsh, djh };
                    object[] objReturn = YBMZSFJSCX(objParam1);
                    if (objReturn[1].ToString().Equals("1"))
                        //撤销门诊登记（挂号）
                        return YBMZDJCX(objParam1);
                    else
                        return objReturn;
                }
                ds.Dispose();
            }
            catch (Exception ex)
            {
                WriteLog(sysdate + "  门诊登记（挂号）收费撤销异常|" + ex.Message);
                return new object[] { 0, 0, "门诊登记（挂号）收费撤销异常|" + ex.Message };
            }
        }
        #endregion

        #region 门诊收费预结算
        public static object[] YBMZSFYJS(object[] objParam)
        {
            string sysdate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            try
            {
                string Ywlx = "MZSFYJS";
                string jzlsh = objParam[0].ToString();  //就诊流水号
                string zhsybz = objParam[1].ToString(); //账户使用标志(；在吉安地区用不到,门诊收费走账户)
                string jssj = objParam[2].ToString();   //结算时间
                string bzbh = objParam[3].ToString();   //病种编号
                string bzmc = objParam[4].ToString();   //病种名称
                string cfhs = objParam[5].ToString();   //处方号集
                string yllb = objParam[6].ToString();   //医疗类别
                string sfje = objParam[7].ToString();   //收费金额
                string cfysdm = objParam[8].ToString(); //处方医生代码
                string cfysxm = objParam[9].ToString(); //处方医生姓名
                string djh = "0000";    //单据号（发票号)
                string sfdypj = "False";                //是否打印票据
                double sfje2 = 0.0000; //金额 
                string ybjzlsh = "";   //医保就诊流水号
                string bxh = "";
                string xm = "";
                string kh = "";
                string dgysxm = ""; //开方医生(定岗医生)

                if (string.IsNullOrEmpty(jzlsh))
                    return new object[] { 0, 0, "就诊流水号不能为空" };
                if (string.IsNullOrEmpty(cfhs))
                    return new object[] { 0, 0, "处方号集不能为空" };
                if (string.IsNullOrEmpty(sfje))
                    return new object[] { 0, 0, "收费金额不能为空" };

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

                string jzlsh1 = "MZ" + yllb + jzlsh;

                # region 是否门诊医保挂号
                string strSql = string.Format("select * from ybmzzydjdr where jzlsh1='{0}' and cxbz=1", jzlsh1);
                DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                if (ds.Tables[0].Rows.Count == 0)
                {
                    return new object[] { 0, 0, "该患者未办理医保挂号登记" };
                }
                bxh = ds.Tables[0].Rows[0]["grbh"].ToString();
                xm = ds.Tables[0].Rows[0]["xm"].ToString();
                kh = ds.Tables[0].Rows[0]["kh"].ToString();
                dqbh = ds.Tables[0].Rows[0]["tcqh"].ToString();
                    //dgysxm = ds.Tables[0].Rows[0]["dgysxm"].ToString();
                ybjzlsh = ds.Tables[0].Rows[0]["ybjzlsh"].ToString();
                yllb = ds.Tables[0].Rows[0]["yllb"].ToString();
                bzbh = ds.Tables[0].Rows[0]["bzbm"].ToString();
                bzmc = ds.Tables[0].Rows[0]["bzmc"].ToString();
                #endregion

                #region 普通门诊不需要输入病种
                if (yllb.Equals("11"))
                {
                    bzbh = "NULL";
                    bzmc = "NULL";
                }
                #endregion

                #region 获取定岗医生信息
                //strSql = string.Format(@"select * from ybdgyszd where ysbm='{0}'", cfysdm);
                //ds.Tables.Clear();
                //ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                //if (ds.Tables[0].Rows.Count == 0)
                //    dgysxm = "聂爱如";
                //else
                //    dgysxm = ds.Tables[0].Rows[0]["ysxm"].ToString();
                #endregion

                StringBuilder inputData = new StringBuilder();
                //入参:保险号|姓名|卡号|地区编号|门诊号|病种编号|病种名称|开方医生|是否打印票据
                inputData.Append(bxh + "|");
                inputData.Append(xm + "|");
                inputData.Append(kh + "|");
                inputData.Append(dqbh + "|");
                inputData.Append(ybjzlsh + "|");
                inputData.Append(bzbh + "|");
                inputData.Append(bzmc + "|");
                inputData.Append(cfysxm + "|");
                inputData.Append(sfdypj + ";");

                #region 处方明细
                strSql = string.Format(@"select y.ybxmbh, y.ybxmmc, m.dj, sum(m.sl) sl, sum(m.je) je, m.yyxmbh, m.yyxmmc, m.cfh from 
                                        (
                                        --药品
                                        select a.mcypno yyxmbh, a.mcypnm yyxmmc, a.mcpric dj, a.mcquty sl, a.mcamnt je, a.mccfno cfh
                                        from mzcfd a 
                                        where a.mcghno = '{0}' and a.mccfno in ({1})
                                        union all
                                        --检查/治疗
                                        select b.mbitem yyxmbh, b.mbname yyxmmc, b.mbpric dj, b.mbquty sl, b.mbsjam je,b.mbzlno cfh          
                                        from mzb2d b 
                                        where b.mbghno = '{0}' and b.mbzlno in ({1})
                                        union all
                                        --检验
                                        select c.mbitem yyxmbh, c.mbname yyxmmc, c.mbpric dj, c.mbquty sl, c.mbsjam je, c.mbzlno cfh
                                        from mzb4d c 
                                        where c.mbghno = '{0}' and c.mbzlno in ({1})
                                        union all
                                        --注射
                                        select b5item yyxmbh, b5name yyxmmc, b5sfam dj, mddays sl, b5sfam * mddays je, mdzsno cfh
                                        from mzd3d
                                        left join (select distinct mccfno, mcghno from mzcfd) mzcf on mccfno = mdcfno
                                        left join bz09d on b9mbno = mdtwid 
                                        left join bz05d on b5item = b9item where mdtiwe > 0 and mdzsno in ({1})
                                        union all
                                        select b5item yyxmbh, b5name yyxmmc, b5sfam dj, mdtims sl, b5sfam * mdtims je,mdzsno cfh
                                        from mzd3d 
                                        left join bz09d on b9mbno = mdwayid left join bz05d on b5item = b9item
                                        left join (select distinct mccfno, mcghno from mzcfd) mzcf on mccfno = mdcfno 
                                        where mdzsno in ({1})
                                        union all
                                        select b5item yyxmbh, b5name yyxmmc, b5sfam dj, mdpqty sl, b5sfam * mdpqty je,mdzsno cfh
                                        from mzd3d 
                                        left join bz09d on b9mbno = mdpprid 
                                        left join bz05d on b5item = b9item
                                        left join (select distinct mccfno, mcghno from mzcfd) mzcf on mccfno = mdcfno
                                        where mdpqty > 0 and mdzsno in ({1})
                                        union all
                                        --处方划价
                                        select a.ygypno yyxmbh, a.ygypnm yyxmmc, ((a.ygamnt + 0.0) / a.ygslxx) dj, a.ygslxx sl, a.ygamnt je, a.ygshno cfh
                                        from yp17d a 
                                        join yp17h b on a.ygcomp = b.ygcomp and a.ygshno = b.ygshno
                                        join yp01h c on c.y1ypno = a.ygypno
                                        where b.ygghno = '{0}' and a.ygshno in ({1}) and a.ygslxx > 0
                                        ) m 
                                        left join ybhisdzdr y on m.yyxmbh = y.hisxmbh
                                        group by y.ybxmbh, y.ybxmmc, m.dj, m.yyxmbh, m.yyxmmc, y.sfxmzldm, y.sflbdm, m.cfh", jzlsh, cfhs);
                ds.Tables.Clear();
                ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                StringBuilder strMsg = new StringBuilder();
                string cfrq = DateTime.Now.ToString("yyyyMMdd");
                double sfje3 = 0.0000;
                if (ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        if (dr["ybxmbh"] == DBNull.Value)
                            strMsg.Append("项目代码：[" + dr["yyxmbh"].ToString() + "]，名称：[" + dr["yyxmmc"].ToString() + "]未对照，不能上传！");
                        else
                        {
                            string ybxmbh = dr["ybxmbh"].ToString();
                            string ybxmmc = dr["ybxmmc"].ToString();
                            string dj = dr["dj"].ToString();
                            string sl = dr["sl"].ToString();
                            string je = dr["je"].ToString();
                            string yyxmxx = dr["yyxmbh"].ToString() + "&" + dr["yyxmmc"].ToString();
                            sfje3 += Convert.ToDouble(je);

                            //明细入参:项目编号|项目名称|单价|数量|金额|处方日期|医院项目信息
                            inputData.Append(ybxmbh + "|");
                            inputData.Append(ybxmmc + "|");
                            inputData.Append(dj + "|");
                            inputData.Append(sl + "|");
                            inputData.Append(je + "|");
                            inputData.Append(cfrq + "|");
                            inputData.Append(yyxmxx + ";");
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
                ds.Dispose();
                #endregion

                StringBuilder outData = new StringBuilder(102400);
                StringBuilder retMsg = new StringBuilder(102400);

                WriteLog(sysdate + "  " + jzlsh + " 进入门诊收费预结算...");
                WriteLog(sysdate + "  入参|" + inputData.ToString());
                int i = f_UserBargaingApply(Ywlx, inputData, outData, retMsg);
                if (i > 0)
                {
                    #region 出参
                    /*出参:保险号|姓名|卡号|地区编号|地区名称|出生日期|实际年龄|参保日期|个人身份|单位名称|
                     * 性别|医疗人员类别|卡状态|账户余额|门诊(住院)号|单据流水号|医疗类别|科室名称|本次看病次数|住院床号|入院日期|
                     * 入院时间|出院日期|出院时间|出院原因|医疗总费用|本次账户支付|本次现金支付|本次基金支付|大病基金支付|救助金额|
                     * 单位补充医保支付|离休干部单独统筹支付|甲类费用|乙类费用|丙类费用|自费费用|结算人|结算日期|结算时间|起付标准自付|
                     * 非医保自付|乙类药品自付|特检特治自付|进入统筹自付|进入大病自付|重大疾病范围内补偿金额|重大疾病范围外补偿金额|
                     * 医院负担金额|大病二次补偿|民政大病救助基金|政府兜底基金|其中公务员补助部分
                 * 0730397447|陶子恒|36082400902240|360824|新干县|20100217|8|20091201|07579728|陶涛|男|居民-未成年居民|正常|66.72|68999784|72485907|门诊慢性病二类|儿科|1|NULL|20180306|1655|NULL|NULL|NULL|300.88|181.17|119.71|0|0|0|0|0|181.17|0|0|119.71|陶涛|20180306|1743|0|119.71|0|0|0|0
                     */
                    string[] str = outData.ToString().Split(';');
                    string[] str2 = str[0].Split('|');
                    string bxh2 = str2[0].Trim();       //保险号
                    string xm2 = str2[1].Trim();        //姓名
                    string kh2 = str2[2].Trim();        //卡号
                    string dqbh2 = str2[3].Trim();      //地区编号
                    string dqmc = str2[4].Trim();       //地区名称
                    string csrq = str2[5].Trim();       //出生日期
                    string sjnl = str2[6].Trim();       //实际年龄
                    string cbrq = str2[7].Trim();       //参保日期
                    string grsf = str2[8].Trim();       //个人身份
                    string dwmc = str2[9].Trim();       //单位名称
                    string xb = str2[10].Trim();        //性别
                    string ylrylb = str2[11].Trim();    //医疗人员类别
                    string kzt = str2[12].Trim();       //卡状态
                    string zfye = str2[13].Trim();      //账户余额
                    string jzlsh2 = str2[14].Trim();    //门诊(住院)号
                    string jslsh = str2[15].Trim();     //单据流水号
                    string yllb2 = str2[16].Trim();      //医疗类别
                    string ksmc = str2[17].Trim();      //科室名称
                    string zycs = str2[18].Trim();      //本次看病次数
                    string zych = str2[19].Trim();      //住院床号
                    string ryrq = str2[20].Trim();      //入院日期
                    string rysj = str2[21].Trim();      //入院时间
                    string cyrq = str2[22].Trim();      //出院日期
                    string cysj = str2[23].Trim();      //出院时间
                    string cyyy = str2[24].Trim();      //出院原因
                    string ylfze = str2[25].Trim();     //医疗总费用
                    string zhzf = str2[26].Trim();      //本次账户支付
                    string xjzf = str2[27].Trim();      //本次现金支付
                    string tcjjzf = str2[28].Trim();    //本次基金支付
                    string dejjzf = str2[29].Trim();    //大病基金支付
                    string mzjzfy = str2[30].Trim();    //救助金额
                    string dwfdfy = str2[31].Trim();    //单位补充医保支付
                    string lxgbddtczf = str2[32].Trim();//离休干部单独统筹支付
                    string jlfy = str2[33].Trim();      //甲类费用
                    string ylfy = str2[34].Trim();      //乙类费用
                    string blfy = str2[35].Trim();      //丙类费用
                    string zffy = str2[36].Trim();      //自费费用
                    string jsr = str2[37].Trim();       //结算人
                    string jsrq = str2[38].Trim();      //结算日期
                    string jssj2 = str2[39].Trim();      //结算时间
                    string qfbzfy = str2[40].Trim();    //起付标准自付
                    string fybzf = str2[41].Trim();     //非医保自付
                    string ylypzf = str2[42].Trim();    //乙类药品自付
                    string tjtzzf = str2[43].Trim();    //特检特治自付
                    string jrtcfy = str2[44].Trim();    //进入统筹自付
                    string jrdebxfy = str2[45].Trim();  //进入大病自付
                    string zdjbfwnbcje = "0.00"; //重大疾病范围内补偿金额
                    string zdjbfwybcje = "0.00"; //重大疾病范围外补偿金额
                    string yyfdje = "0.00"; //医院负担金额
                    string dbecbc = "0.00"; //大病二次补偿
                    string mzdbjzjj = "0.00"; //民政大病救助基金
                    string zftdjj = "0.00"; //政府兜底基金
                    string gwybzfy = "0.00";   //其中公务员补助部分
                    double bcjsqzhye = Convert.ToDouble(zhzf) + Convert.ToDouble(zfye); //本次结算前帐户余额
                    string jbr = CliUtils.fLoginUser;
                    string xjzf1 = "";
                    if (xjzf.Equals("-.01"))
                    {
                        xjzf1 = "0.00";
                        xjzf = "-0.01";
                    }
                    else
                    {
                        xjzf1 = xjzf;
                    }
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
                    string strValue = ylfze + "|0.00|" + tcjjzf + "|" + dejjzf + "|" + zhzf + "|" + xjzf1 + "|" + gwybzfy + "|" + dwfdfy + "|" + zffy + "|" + dwfdfy + "|" +
                                    yyfdje + "|" + mzjzfy + "|0.00|0.00|0.00|0.00|" + qfbzfy + "|0.00|" + jrtcfy + "|0.00|" +
                                    0.00 + "|" + jrdebxfy + "|0.00|0.00|0.00|" + bcjsqzhye + "|0.00|0.00|0.00|0.00|" +
                                    0.00 + "|" + 0.00 + "|" + zycs + "|" + xm2 + "|" + jsrq + "|" + yllb2 + "|" + ylrylb + "|" + dqbh2 + "||" + jslsh + "||" +
                                    "0000" + "|||1||" + dqbh2 + "|" + dbecbc + "|" + 0.00 + "|0.00|" + bxh2 + "||" +
                                    zftdjj + "|" + 0.00 + "|" + 0.00 + "|" + 0.00 + "|" + 0.00 + "|" + 0.00 + "|";
                    WriteLog(sysdate + "  " + jzlsh + " 进入门诊收费预结算成功|" + strValue);
                    return new object[] { 0, 1, strValue };
                }
                else
                {
                    WriteLog(sysdate + "  " + jzlsh + " 进入门诊收费预结算失败|" + retMsg.ToString());
                    return new object[] { 0, 0, "门诊收费预结算失败|" + retMsg.ToString() };
                }
            }
            catch (Exception ex)
            {
                WriteLog(sysdate + "  门诊收费预结算异常|" + ex.Message);
                return new object[] { 0, 0, "门诊收费预结算异常|" + ex.Message };
            }
        }
        #endregion

        #region 门诊收费结算
        public static object[] YBMZSFJS(object[] objParam)
        {
            string sysdate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string Ywlx = "MZSF";
            try
            {
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
                string sfdypj = "True";                //是否打印票据
                double sfje2 = 0.0000; //金额 
                string ybjzlsh = "";   //医保就诊流水号
                string bxh = "";
                string xm = "";
                string kh = "";
                string dgysxm = ""; //开方医生(定岗医生)
                string jbr = CliUtils.fLoginUser;

                if (string.IsNullOrEmpty(jzlsh))
                    return new object[] { 0, 0, "就诊流水号不能为空" };
                if (string.IsNullOrEmpty(djh))
                    return new object[] { 0, 0, "单据号(发票号)不能为空" };
                if (string.IsNullOrEmpty(cfhs))
                    return new object[] { 0, 0, "处方号集不能为空" };
                if (string.IsNullOrEmpty(sfje))
                    return new object[] { 0, 0, "收费金额不能为空" };

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

                string jzlsh1 = "MZ" + yllb + jzlsh;

                #region 是否门诊医保挂号
                string strSql = string.Format("select * from ybmzzydjdr where jzlsh1='{0}' and cxbz=1", jzlsh1);
                DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                if (ds.Tables[0].Rows.Count == 0)
                    return new object[] { 0, 0, "该患者未办理医保挂号登记" };
                else
                {
                    bxh = ds.Tables[0].Rows[0]["grbh"].ToString();
                    xm = ds.Tables[0].Rows[0]["xm"].ToString();
                    kh = ds.Tables[0].Rows[0]["kh"].ToString();
                    dqbh = ds.Tables[0].Rows[0]["tcqh"].ToString();
                    //dgysxm = ds.Tables[0].Rows[0]["dgysxm"].ToString();
                    ybjzlsh = ds.Tables[0].Rows[0]["ybjzlsh"].ToString();
                    bzbh = ds.Tables[0].Rows[0]["bzbm"].ToString();
                    bzmc = ds.Tables[0].Rows[0]["bzmc"].ToString();
                }
                #endregion

                #region 普通门诊不需要输入病种
                if (yllb.Equals("11"))
                {
                    bzbh = "NULL";
                    bzmc = "NULL";
                }
                #endregion

                #region 获取定岗医生信息
                //strSql = string.Format(@"select * from ybdgyszd where ysbm='{0}'", cfysdm);
                //ds.Tables.Clear();
                //ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                //if (ds.Tables[0].Rows.Count == 0)
                //    dgysxm = "聂爱如";
                //else
                //    dgysxm = ds.Tables[0].Rows[0]["ysxm"].ToString();
                #endregion

                StringBuilder inputData = new StringBuilder();
                //入参:保险号|姓名|卡号|地区编号|门诊号|病种编号|病种名称|开方医生|是否打印票据
                inputData.Append(bxh + "|");
                inputData.Append(xm + "|");
                inputData.Append(kh + "|");
                inputData.Append(dqbh + "|");
                inputData.Append(ybjzlsh + "|");
                inputData.Append(bzbh + "|");
                inputData.Append(bzmc + "|");
                inputData.Append(cfysxm + "|");
                inputData.Append(sfdypj + ";");

                #region 收费明细
                strSql = string.Format(@"select y.ybxmbh, y.ybxmmc, m.dj, sum(m.sl) sl, sum(m.je) je, m.yyxmbh, m.yyxmmc, m.cfh from 
                                        (
                                        --药品
                                        select a.mcypno yyxmbh, a.mcypnm yyxmmc, a.mcpric dj, a.mcquty sl, a.mcamnt je, a.mccfno cfh
                                        from mzcfd a 
                                        where a.mcghno = '{0}' and a.mccfno in ({1})
                                        union all
                                        --检查/治疗
                                        select b.mbitem yyxmbh, b.mbname yyxmmc, b.mbpric dj, b.mbquty sl, b.mbsjam je,b.mbzlno cfh          
                                        from mzb2d b 
                                        where b.mbghno = '{0}' and b.mbzlno in ({1})
                                        union all
                                        --检验
                                        select c.mbitem yyxmbh, c.mbname yyxmmc, c.mbpric dj, c.mbquty sl, c.mbsjam je, c.mbzlno cfh
                                        from mzb4d c 
                                        where c.mbghno = '{0}' and c.mbzlno in ({1})
                                        union all
                                        --注射
                                        select b5item yyxmbh, b5name yyxmmc, b5sfam dj, mddays sl, b5sfam * mddays je, mdzsno cfh
                                        from mzd3d
                                        left join (select distinct mccfno, mcghno from mzcfd) mzcf on mccfno = mdcfno
                                        left join bz09d on b9mbno = mdtwid 
                                        left join bz05d on b5item = b9item where mdtiwe > 0 and mdzsno in ({1})
                                        union all
                                        select b5item yyxmbh, b5name yyxmmc, b5sfam dj, mdtims sl, b5sfam * mdtims je,mdzsno cfh
                                        from mzd3d 
                                        left join bz09d on b9mbno = mdwayid left join bz05d on b5item = b9item
                                        left join (select distinct mccfno, mcghno from mzcfd) mzcf on mccfno = mdcfno 
                                        where mdzsno in ({1})
                                        union all
                                        select b5item yyxmbh, b5name yyxmmc, b5sfam dj, mdpqty sl, b5sfam * mdpqty je,mdzsno cfh
                                        from mzd3d 
                                        left join bz09d on b9mbno = mdpprid 
                                        left join bz05d on b5item = b9item
                                        left join (select distinct mccfno, mcghno from mzcfd) mzcf on mccfno = mdcfno
                                        where mdpqty > 0 and mdzsno in ({1})
                                        union all
                                        --处方划价
                                        select a.ygypno yyxmbh, a.ygypnm yyxmmc, ((a.ygamnt + 0.0) / a.ygslxx) dj, a.ygslxx sl, a.ygamnt je, a.ygshno cfh
                                        from yp17d a 
                                        join yp17h b on a.ygcomp = b.ygcomp and a.ygshno = b.ygshno
                                        join yp01h c on c.y1ypno = a.ygypno
                                        where b.ygghno = '{0}' and a.ygshno in ({1}) and a.ygslxx > 0
                                        ) m 
                                        left join ybhisdzdr y on m.yyxmbh = y.hisxmbh
                                        group by y.ybxmbh, y.ybxmmc, m.dj, m.yyxmbh, m.yyxmmc, y.sfxmzldm, y.sflbdm, m.cfh", jzlsh, cfhs);
                ds.Tables.Clear();
                ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                StringBuilder strMsg = new StringBuilder();
                string cfrq = DateTime.Now.ToString("yyyyMMdd");
                double sfje3 = 0.0000;
                List<string> li_yyxmbh = new List<string>();
                List<string> li_yyxmmc = new List<string>();
                List<string> li_ybxmbh = new List<string>();
                List<string> li_je = new List<string>();
                List<string> li_cfh = new List<string>();


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
                            string ybxmbh = dr["ybxmbh"].ToString();
                            string ybxmmc = dr["ybxmmc"].ToString();
                            string dj = dr["dj"].ToString();
                            string sl = dr["sl"].ToString();
                            string je = dr["je"].ToString();
                            sfje3 += Convert.ToDouble(je);
                            string yyxmxx = dr["yyxmbh"].ToString() + "&" + dr["yyxmmc"].ToString();
                            //明细入参:项目编号|项目名称|单价|数量|金额|处方日期|医院项目信息
                            inputData.Append(ybxmbh + "|");
                            inputData.Append(ybxmmc + "|");
                            inputData.Append(dj + "|");
                            inputData.Append(sl + "|");
                            inputData.Append(je + "|");
                            inputData.Append(cfrq + "|");
                            inputData.Append(yyxmxx + ";");
                            li_yyxmbh.Add(dr["yyxmbh"].ToString());
                            li_yyxmmc.Add(dr["yyxmmc"].ToString());
                            li_ybxmbh.Add(dr["ybxmbh"].ToString());
                            li_cfh.Add(dr["cfh"].ToString());
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

                WriteLog(sysdate + "  " + jzlsh + " 进入门诊收费结算...");
                WriteLog(sysdate + "  入参|" + inputData.ToString());
                int i = f_UserBargaingApply(Ywlx, inputData, outData, retMsg);

                if (i > 0)
                {
                    List<string> liSQL = new List<string>();
                    #region 出参
                    /*出参:保险号|姓名|卡号|地区编号|地区名称|出生日期|实际年龄|参保日期|个人身份|单位名称|
                     * 性别|医疗人员类别|卡状态|账户余额|门诊(住院)号|单据流水号|医疗类别|科室名称|本次看病次数|住院床号|入院日期|
                     * 入院时间|出院日期|出院时间|出院原因|医疗总费用|本次账户支付|本次现金支付|本次基金支付|大病基金支付|救助金额|
                     * 单位补充医保支付|离休干部单独统筹支付|甲类费用|乙类费用|丙类费用|自费费用|结算人|结算日期|结算时间|起付标准自付|
                     * 非医保自付|乙类药品自付|特检特治自付|进入统筹自付|进入大病自付|重大疾病范围内补偿金额|重大疾病范围外补偿金额|
                     * 医院负担金额|大病二次补偿|民政大病救助基金|政府兜底基金|其中公务员补助部分
                     */
                    string[] str = outData.ToString().Split(';');
                    string[] str2 = str[0].Split('|');
                    string bxh2 = str2[0].Trim();       //保险号
                    string xm2 = str2[1].Trim();        //姓名
                    string kh2 = str2[2].Trim();        //卡号
                    string dqbh2 = str2[3].Trim();      //地区编号
                    string dqmc = str2[4].Trim();       //地区名称
                    string csrq = str2[5].Trim();       //出生日期
                    string sjnl = str2[6].Trim();       //实际年龄
                    string cbrq = str2[7].Trim();       //参保日期
                    string grsf = str2[8].Trim();       //个人身份
                    string dwmc = str2[9].Trim();       //单位名称
                    string xb = str2[10].Trim();        //性别
                    string ylrylb = str2[11].Trim();    //医疗人员类别
                    string kzt = str2[12].Trim();       //卡状态
                    string zfye = str2[13].Trim();      //账户余额
                    string jzlsh2 = str2[14].Trim();    //门诊(住院)号
                    string jslsh = str2[15].Trim();     //单据流水号
                    string yllb2 = str2[16].Trim();      //医疗类别
                    string ksmc = str2[17].Trim();      //科室名称
                    string zycs = str2[18].Trim();      //本次看病次数
                    string zych = str2[19].Trim();      //住院床号
                    string ryrq = str2[20].Trim();      //入院日期
                    string rysj = str2[21].Trim();      //入院时间
                    string cyrq = str2[22].Trim();      //出院日期
                    string cysj = str2[23].Trim();      //出院时间
                    string cyyy = str2[24].Trim();      //出院原因
                    string ylfze = str2[25].Trim();     //医疗总费用
                    string zhzf = str2[26].Trim();      //本次账户支付
                    string xjzf = str2[27].Trim();      //本次现金支付
                    string tcjjzf = str2[28].Trim();    //本次基金支付
                    string dejjzf = str2[29].Trim();    //大病基金支付
                    string mzjzfy = str2[30].Trim();    //救助金额
                    string dwfdfy = str2[31].Trim();    //单位补充医保支付
                    string lxgbddtczf = str2[32].Trim();//离休干部单独统筹支付
                    string jlfy = str2[33].Trim();      //甲类费用
                    string ylfy = str2[34].Trim();      //乙类费用
                    string blfy = str2[35].Trim();      //丙类费用
                    string zffy = str2[36].Trim();      //自费费用
                    string jsr = str2[37].Trim();       //结算人
                    string jsrq = str2[38].Trim();      //结算日期
                    string jssj2 = str2[39].Trim();      //结算时间
                    string qfbzfy = str2[40].Trim();    //起付标准自付
                    string fybzf = str2[41].Trim();     //非医保自付
                    string ylypzf = str2[42].Trim();    //乙类药品自付
                    string tjtzzf = str2[43].Trim();    //特检特治自付
                    string jrtcfy = str2[44].Trim();    //进入统筹自付
                    string jrdebxfy = str2[45].Trim();  //进入大病自付
                    string zdjbfwnbcje = "0.00"; //重大疾病范围内补偿金额
                    string zdjbfwybcje = "0.00"; //重大疾病范围外补偿金额
                    string yyfdje = "0.00"; //医院负担金额
                    string dbecbc = "0.00"; //大病二次补偿
                    string mzdbjzjj = "0.00"; //民政大病救助基金
                    string zftdjj = "0.00"; //政府兜底基金
                    string gwybzfy = "0.00";   //其中公务员补助部分
                    double bcjsqzhye = Convert.ToDouble(zhzf) + Convert.ToDouble(zfye);
                    string xjzf1 = "";
                    if (xjzf.Equals("-.01"))
                    {
                        xjzf1 = "0.00";
                        xjzf = "-0.01";
                    }
                    else
                        xjzf1 = xjzf;
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
                    string strValue = ylfze + "|0.00|" + tcjjzf + "|" + dejjzf + "|" + zhzf + "|" + xjzf1 + "|" + gwybzfy + "|" + dwfdfy + "|" + zffy + "|" + dwfdfy + "|" +
                                    yyfdje + "|" + mzjzfy + "|0.00|0.00|0.00|0.00|" + qfbzfy + "|0.00|" + jrtcfy + "|0.00|" +
                                    0.00 + "|" + jrdebxfy + "|0.00|0.00|0.00|" + bcjsqzhye + "|0.00|0.00|0.00|0.00|" +
                                    0.00 + "|" + 0.00 + "|" + zycs + "|" + xm2 + "|" + jsrq + "|" + yllb2 + "|" + ylrylb + "|" + dqbh2 + "||" + jslsh + "||" +
                                    "0000" + "|||1||" + dqbh2 + "|" + dbecbc + "|" + 0.00 + "|0.00|" + bxh2 + "||" +
                                    zftdjj + "|" + 0.00 + "|" + 0.00 + "|" + 0.00 + "|" + 0.00 + "|" + 0.00 + "|";

                    strSql = string.Format(@"insert into ybfyjsdr(jzlsh,jylsh,djhin,cyrq,cyyy,zhsybz,ztjsbz,jbr,xm,kh,
                                        grbh,jsrq,yllb,yldylb,jslsh,ylfze,zhzf,xjzf,tcjjzf,dejjzf,
                                        mzjzfy,dwfdfy,lxgbddtczf,jlfy,ylfy,blfy,zffy,qfbzfy,fybzf,ylypzf,
                                        tjtzzf,jrtczf,jrdbzf,zdjbfwnbcje,zdjbfwybcje,yyfdfy,ecbcje,mzdbjzje,czzcje,gwybzjjzf,
				                        sysdate,djh,ybjzlsh) values(
                                        '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}',
                                        '{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}',
                                        '{20}','{21}','{22}','{23}','{24}','{25}','{26}','{27}','{28}','{29}',
                                        '{30}','{31}','{32}','{33}','{34}','{35}','{36}','{37}','{38}','{39}',
                                        '{40}','{41}','{42}')",
                                    jzlsh, jslsh, djh, cyrq, cyyy, 1, 0, jbr, xm2, kh2,
                                    bxh2, jsrq, yllb, ylrylb, jslsh, ylfze, zhzf, xjzf, tcjjzf, dejjzf,
                                    mzjzfy, dwfdfy, lxgbddtczf, jlfy, ylfy, blfy, zffy, qfbzfy, fybzf, ylypzf,
                                    tjtzzf, jrtcfy, jrdebxfy, zdjbfwnbcje, zdjbfwybcje, yyfdje, dbecbc, mzdbjzjj, zftdjj, gwybzfy,
                                    sysdate, djh, jzlsh2);
                    liSQL.Add(strSql);
                    /*
                     * 门诊(住院)号|单据流水号|项目编号|项目名称|项目等级|收费类别|单价|数量|金额|自付金额
                     */
                    for (int j = 1; j < str.Length; j++)
                    {
                        str2 = str[j].Split('|');
                        string jzlsh3 = str2[0].Trim(); //门诊(住院)号
                        string djlsh = str2[1].Trim();  //单据流水号
                        string ybxmbh = str2[2].Trim(); //项目编号
                        string ybxmmc = str2[3].Trim(); //项目名称
                        string ybxmdj = str2[4].Trim(); //项目等级
                        string ybsflb = str2[5].Trim(); //收费类别
                        string dj = str2[6].Trim();     //单价
                        string sl = str2[7].Trim();     //数量
                        string je = str2[8].Trim();     //金额
                        string zfje = str2[9].Trim();   //自付金额
                        for (int k = 0; k < li_ybxmbh.Count; k++)
                        {
                            if (ybxmbh.Equals(li_ybxmbh[k]))
                            {
                                strSql = string.Format(@"insert into ybcfmxscindr(jzlsh,je,xm,kh,ybcfh,yysfxmbm,yysfxmmc,sysdate,ybjzlsh,djlsh,
                                                    sfxmzxbm,sfxmzxmc,sflb,sfxmzl,dj,sl,zfje,jsdjh) values(
                                                    '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}',
                                                    '{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}')",
                                                        jzlsh, je, xm2, kh2, li_cfh[k], li_yyxmbh[k], li_yyxmmc[k], sysdate, jzlsh3, djlsh,
                                                        ybxmbh, ybxmmc, ybsflb, ybxmdj, dj, sl, zfje, djh);
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
                        object[] objParam2 = new object[] { jzlsh, jslsh };
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
            string Ywlx = "MZSFCX";
            string jzlsh = objParam[0].ToString();  //就诊流水号
            string djh = objParam[1].ToString();    //发票号
            string bxh = string.Empty;
            string xm = string.Empty;
            string kh = string.Empty;
            string dqbh = string.Empty;
            string djlsh = string.Empty;    //医保反馈单据流水号 
            string ybjzlsh = "";
            if (string.IsNullOrEmpty(jzlsh))
                return new object[] { 0, 0, "就诊流水号不能为空" };

            #region 是否结算
            string strSql = string.Format(@"select a.jslsh,a.grbh,a.xm,a.kh,a.ybjzlsh,b.tcqh from ybfyjsdr a 
									left join ybmzzydjdr b on a.jzlsh=b.jzlsh and a.cxbz=b.cxbz and a.ybjzlsh=b.ybjzlsh  
                                    where a.jzlsh='{0}' and a.cxbz=1 and a.djh='{1}'", jzlsh, djh);
            DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            if (ds.Tables[0].Rows.Count == 0)
                return new object[] { 0, 0, "该患者无结算信息" };
            else
            {
                bxh = ds.Tables[0].Rows[0]["grbh"].ToString();
                xm = ds.Tables[0].Rows[0]["xm"].ToString();
                kh = ds.Tables[0].Rows[0]["kh"].ToString();
                dqbh = ds.Tables[0].Rows[0]["tcqh"].ToString();
                ybjzlsh = ds.Tables[0].Rows[0]["ybjzlsh"].ToString();
                djlsh = ds.Tables[0].Rows[0]["jslsh"].ToString();
            }
            ds.Dispose();
            #endregion

            StringBuilder inputData = new StringBuilder();
            //入参:保险号|姓名|卡号|地区编号|门诊号|单据流水号
            inputData.Append(bxh + "|");
            inputData.Append(xm + "|");
            inputData.Append(kh + "|");
            inputData.Append(dqbh + "|");
            inputData.Append(ybjzlsh + "|");
            inputData.Append(djlsh);
            StringBuilder outData = new StringBuilder(1024);
            StringBuilder retMsg = new StringBuilder(1024);

            WriteLog(sysdate + "  " + jzlsh + " 进入门诊收费撤销...");
            WriteLog(sysdate + "  入参|" + inputData.ToString());
            int i = f_UserBargaingApply(Ywlx, inputData, outData, retMsg);
            if (i > 0)
            {
                List<string> liSQL = new List<string>();
                strSql = string.Format(@"insert into ybfyjsdr(jzlsh,jylsh,djhin,cyrq,cyyy,zhsybz,ztjsbz,jbr,xm,kh,
                                        grbh,jsrq,yllb,yldylb,jslsh,ylfze,zhzf,xjzf,tcjjzf,dejjzf,
                                        mzjzfy,dwfdfy,lxgbddtczf,jlfy,ylfy,blfy,zffy,qfbzfy,fybzf,ylypzf,
                                        tjtzzf,jrtczf,jrdbzf,zdjbfwnbcje,zdjbfwybcje,yyfdfy,ecbcje,mzdbjzje,czzcje,gwybzjjzf,
                                        djh,sysdate,cxbz) select 
                                        jzlsh,jylsh,djhin,cyrq,cyyy,zhsybz,ztjsbz,jbr,xm,kh,
                                        grbh,jsrq,yllb,yldylb,jslsh,ylfze,zhzf,xjzf,tcjjzf,dejjzf,
                                        mzjzfy,dwfdfy,lxgbddtczf,jlfy,ylfy,blfy,zffy,qfbzfy,fybzf,ylypzf,
                                        tjtzzf,jrtczf,jrdbzf,zdjbfwnbcje,zdjbfwybcje,yyfdfy,ecbcje,mzdbjzje,czzcje,gwybzjjzf,
                                        djh,'{2}',0 from ybfyjsdr where jslsh='{0}' and jzlsh='{1}' and cxbz=1", djlsh, jzlsh, sysdate);
                liSQL.Add(strSql);
                strSql = string.Format("update ybfyjsdr set cxbz=2 where jslsh='{0}' and jzlsh='{1}' and cxbz=1", djlsh, jzlsh);
                liSQL.Add(strSql);
                strSql = string.Format(@"insert into ybcfmxscindr(jzlsh,je,xm,kh,ybcfh,yysfxmbm,yysfxmmc,ybjzlsh,djlsh,
                                        sfxmzxbm,sfxmzxmc,sflb,sfxmzl,dj,sl,zfje,jsdjh,cxbz,sysdate,jylsh) select
                                        jzlsh,je,xm,kh,ybcfh,yysfxmbm,yysfxmmc,ybjzlsh,djlsh,
                                        sfxmzxbm,sfxmzxmc,sflb,sfxmzl,dj,sl,zfje,jsdjh,0,'{2}',jylsh from ybcfmxscindr where djlsh = '{0}' and jzlsh='{1}' and cxbz=1", djlsh, jzlsh, sysdate);
                liSQL.Add(strSql);
                strSql = string.Format("update ybcfmxscindr set cxbz = 2 where djlsh = '{0}' and cxbz = 1 ", djlsh);
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

        #region 住院读卡
        public static object[] YBZYDK(object[] ojbParam)
        {
            string sysdate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            try
            {
                string Ywlx = "ZYDJSK";
                StringBuilder inputData = new StringBuilder();
                inputData.Append("");
                StringBuilder outData = new StringBuilder(102400);
                StringBuilder retMsg = new StringBuilder(102400);

                WriteLog(sysdate + "  进入住院读卡....");
                WriteLog(sysdate + "  入参|" + inputData.ToString());
                int i = f_UserBargaingApply(Ywlx, inputData, outData, retMsg);
                if (i > 0)
                {
                    /*保险号|姓名|卡号|地区编号|地区名称|出生日期|实际年龄|参保日期|个人身份|
                     * 单位名称|性别|医疗人员类别|卡状态|账户余额|累计住院支付|累计门诊支付|
                     * 累计特殊门诊|身份号码
                     */
                    string[] str = outData.ToString().Split('|');
                    if (str.Length < 10)
                        return new object[] { 0, 0, outData.ToString() + retMsg.ToString() };
                    #region 入参
                    outParams_dk dk = new outParams_dk();
                    dk.Grbh = str[0].Trim();     //保险号
                    dk.Xm = str[1].Trim();      //姓名
                    dk.Kh = str[2].Trim();      //卡号
                    dk.Tcqh = str[3].Trim();    //地区编号
                    dk.Qxmc = str[4].Trim();    //地区名称
                    dk.Csrq = str[5].Trim();    //出生日期
                    dk.Nl = str[6].Trim();    //实际年龄
                    dk.Cbrq = str[7].Trim();    //参保日期
                    dk.Grsf = str[8].Trim();    //个人身份
                    dk.Dwmc = str[9].Trim();    //单位名称
                    dk.Xb = str[10].Trim();     //性别
                    dk.Ylrylb = str[11].Trim();   //医疗人员类别
                    dk.Zkt = str[12].Trim();    //卡状态
                    dk.Zhye = str[13].Trim(); //账户余额
                    dk.Ljzyzf = str[14].Trim();  //累计住院支付
                    dk.Ljmzzf = str[15].Trim();  //累计门诊支付
                    dk.Ljtsmzzf = str[16].Trim();//累计特殊门诊
                    dk.Sfhz = str[17].Trim(); //身份号码
                    string YBKLX = "0";
                    dk.Yllb = "11";
                    dk.Mtbz = "0";
                    dk.Yldylb = dk.Ylrylb;
                    if (dk.Xb.Equals("男"))
                        dk.Xb = "1";
                    else
                        dk.Xb = "2";
                    dk.Csrq = dk.Csrq.Substring(0, 4) + "-" + dk.Csrq.Substring(4, 2) + "-" + dk.Csrq.Substring(6, 2);
                    dk.Rycbzt = dk.Zkt;
                    dk.Ydrybz = "0";
                    if (dk.Ylrylb.Contains("居民"))
                        dk.Jflx = "0203";
                    else
                        dk.Jflx = "0202";
                    /*
                    /*
                    * 个人编号|单位编号|身份证号|姓名|性别|
                    * 民族|出生日期|社会保障卡卡号|医疗待遇类别|人员参保状态|
                    * 异地人员标志|统筹区号|年度|在院状态|帐户余额|
                    * 本年医疗费累计|本年帐户支出累计|本年统筹支出累计|本年救助金支出累计|本年公务员补助基金累计|
                    * 本年城镇居民门诊统筹支付累计|进入统筹费用累计|进入救助金费用累计|起付标准累计|本年住院次数|
                    * 单位名称|年龄|参保单位类型|经办机构编码|缴费类型|
                    * 医保门慢、特资质|医保门慢、特病种说明|医疗类别代码|慢、特病编码|慢、特病名称
                    */
                    string outParams = dk.Grbh + "|" + dk.Dwbh + "|" + dk.Sfhz + "|" + dk.Xm + "|" + dk.Xb + "|" +
                                       dk.Mz + "|" + dk.Csrq + "|" + dk.Kh + "|" + dk.Yldylb + "|" + dk.Rycbzt + "|" +
                                       dk.Ydrybz + "|" + dk.Tcqh + "|" + dk.Nd + "|" + dk.Zyzt + "|" + dk.Zhye + "|" +
                                       dk.Bnylflj + "|" + dk.Bnzhzclj + "|" + dk.Bntczclj + "|" + dk.Bndbyljjzflj + "|" + dk.Bngwybzjjlj + "|" +
                                       dk.Bnczjmmztczflj + "|" + dk.Jrtcfylj + "|" + dk.Jrdbfylj + "|" + dk.Qfbzlj + "|" + dk.Bnzycs + "|" +
                                       dk.Dwmc + "|" + dk.Nl + "|" + dk.Cbdwlx + "|" + dk.Jbjgbm + "|" + dk.Jflx + "|" +
                                       dk.Mtbz + "|" + dk.Mtmsg + "|" + dk.Yllb + "|" + dk.Mtbzbm + "|" + dk.Mtbzmc + "|" + YBKLX + "|";
                    #endregion

                    List<string> liSQL = new List<string>();
                    string strSql = string.Format("delete from YBICKXX where grbh='{0}'", dk.Grbh);
                    liSQL.Add(strSql);
                    strSql = string.Format(@"insert into YBICKXX(grbh,xm,kh,dqbh,qxmc,csrq,sjnl,cbrq,grsf,dwmc,
                                        xb,rylb,kzt,grzhye,ljzyf,ljmzf,ljtsmzf,gmsfhm) values(
                                        '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}',
                                        '{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}')",
                                        dk.Grbh, dk.Xm, dk.Kh, dk.Tcqh, dk.Qxmc, dk.Csrq, dk.Nl, dk.Cbrq, dk.Grsf, dk.Dwmc,
                                        dk.Xb, dk.Ylrylb, dk.Zkt, dk.Zhye, dk.Ljzyzf, dk.Ljmzzf, dk.Ljtsmzzf, dk.Sfhz);
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
                        return new object[] { 0, 0, "住院读卡成功｜保存本地数据失败｜" + obj[2].ToString() };
                    }
                }
                else
                {
                    WriteLog(sysdate + "  住院读卡失败|" + outData.ToString() + "|" + retMsg.ToString());
                    return new object[] { 0, 0, "读卡失败|" + outData.ToString() + "|" + retMsg.ToString() };
                }
            }
            catch (Exception ex)
            {
                WriteLog(sysdate + "  住院读卡异常|" + ex.Message);
                return new object[] { 0, 0, "住院读卡异常|" + ex.Message };
            }
        }
        #endregion

        #region 住院登记
        public static object[] YBZYDJ(object[] objParam)
        {
            string sysdate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            try
            {
                string Ywlx = "ZYDJ";
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
                string ryrq = objParam[10].ToString(); //入院日期

                string jbr = CliUtils.fUserName; //经办人
                string bxh = grbh;    //保险号
                string bzbm_r = string.Empty;
                string bzmc_r = string.Empty;

                string ksbh = "";   //科室编号
                string ksmc = "";   //科室名称
                string zych = "";   //住院床号
                string zyrq = "";
                string zysj = "";
                string hznm = "";
                string jylsh = jzlsh + DateTime.Now.ToString("yyyyMMdd");
                string jbr2 = CliUtils.fLoginUser;
                

                if (string.IsNullOrEmpty(yllb))
                    return new object[] { 0, 0, "医疗类别不能为空" };
                if (string.IsNullOrEmpty(jzlsh))
                    return new object[] { 0, 0, "就诊流水号不能为空" };

                #region 是否办理住院

                string strSql = string.Format(@"select a.z1date as ryrq,z1hznm,a.z1ksno,a.z1ksnm,'' as z1bedn from zy01h a 
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
                }
                #endregion
                #region 时间有效性
                try
                {
                    DateTime dt_zy = Convert.ToDateTime(ryrq);
                    zyrq = dt_zy.ToString("yyyyMMdd");
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
                ds.Dispose();
                #endregion
                StringBuilder inputData = new StringBuilder();
                //入参: 保险号|姓名|卡号|地区编号|医疗类别|科室名称|住院日期|住院时间|住院床号|入院疾病|医院住院流水号
                inputData.Append(bxh + "|");
                inputData.Append(xm + "|");
                inputData.Append(kh + "|");
                inputData.Append(tcqh + "|");
                inputData.Append(yllb + "|");
                inputData.Append(ksmc + "|");
                inputData.Append(zyrq + "|");
                inputData.Append(zysj + "|");
                inputData.Append(zych + "|");
                inputData.Append(bzbm + "|");
                inputData.Append(jzlsh);
                StringBuilder outData = new StringBuilder(1024);
                StringBuilder retMsg = new StringBuilder(1024);

                WriteLog(sysdate + "  " + jzlsh + " 进入住院医保登记...");
                WriteLog(sysdate + "  入参|" + inputData.ToString());
                int i = f_UserBargaingApply(Ywlx, inputData, outData, retMsg);
                if (i > 0)
                {
                    List<string> liSQL = new List<string>();
                    #region 返回参数
                    /*保险号|姓名|卡号|地区编号|地区名称|出生日期|实际年龄|参保日期|个人身份|单位名称|
                 * 性别|医疗人员类别|卡状态|账户余额|门诊(住院号)|医疗类别|科室名称|挂号费|本次看病次数|住院床号|
                 * 入院日期|入院时间|经办人
                 */
                    string[] str = outData.ToString().Split(';');
                    string[] str2 = str[0].Split('|');
                    string bxh2 = str2[0].Trim();   //保险号
                    string xm2 = str2[1].Trim();    //姓名
                    string kh2 = str2[2].Trim();    //卡号
                    string dqbh1 = str2[3].Trim();  //地区编号
                    string dqmc = str2[4].Trim();   //地区名称
                    string csrq2 = str2[5].Trim();   //出生日期
                    string sjnl = str2[6].Trim();   //实际年龄
                    string cbrq = str2[7].Trim();   //参保日期
                    string grsf = str2[8].Trim();   //个人身份
                    string dwmc2 = str2[9].Trim();   //单位名称
                    string xb2 = str2[10].Trim();    //性别
                    string rylb = str2[11].Trim();  //医疗人员类别
                    string kzt = str2[12].Trim();   //卡状态
                    string grzhye = str2[13].Trim();//账户余额
                    string jzlsh2 = str2[14].Trim();//门诊(住院号)
                    string yllb2 = str2[15].Trim(); //医疗类别
                    string ksmc2 = str2[16].Trim(); //科室名称
                    string ghf = str2[17].Trim();   //挂号费
                    string bckbcs = str2[18].Trim();//本次看病次数
                    string zych2 = str2[19].Trim(); //住院床号
                    string djrq = str2[20].Trim();  //入院日期
                    string djsj = str2[21].Trim();  //入院时间
                    string jbr1 = str2[22].Trim();   //经办人
                    ghf = "0.00";                   //住院患者挂号费用0.00
                    #endregion

                    if (str.Length > 1)
                    {
                        for (int j = 1; j < str.Length; j++)
                        {
                            str2 = str[j].Split('|');
                            bzbm_r = str2[0].Trim();
                            bzmc_r = str2[1].Trim();
                            strSql = string.Format(@"insert into ybmxbdj (jzlsh,bxh,xm,kh,mmbzbm,mmbzmc) values(
                                               '{0}','{1}','{2}','{3}','{4}','{5}')",
                                                    jzlsh, bxh2, xm2, kh2, bzbm_r, bzmc_r);
                            liSQL.Add(strSql);
                        }
                    }

                    strSql = string.Format(@"insert into ybmzzydjdr(grbh,xm,kh,tcqh,bq,csrq,nl,nd,sfzh,dwmc,
                                        xb,yldylb,rycbzt,zhye,ybjzlsh,yllb,ksmc,ghf,zycs,bnzycs,
                                        ghdjsj,jbr,dgysdm,dgysxm,ksbh,jzbz,sysdate,jzlsh,bzbm,bzmc,
                                        jylsh,mmbzbm1,mmbzmc1) values(
                                       '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}',
                                       '{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}',
                                       '{20}','{21}','{22}','{23}','{24}','{25}','{26}','{27}','{28}','{29}',
                                       '{30}','{31}','{32}')",
                                        bxh2, xm2, kh2, tcqh, dqmc, csrq2, sjnl, cbrq, grsf, dwmc2,
                                        xb2, rylb, kzt, grzhye, jzlsh2, yllb, ksmc2, ghf, bckbcs, zych2,
                                        djrq, jbr2, dgysdm, dgysxm, ksbh, "z", sysdate, jzlsh, bzbm, bzmc,
                                        jylsh, bzbm_r, bzmc_r);
                    liSQL.Add(strSql);

                    strSql = string.Format(@"update zy01h set z1rylb = '{0}', z1tcdq = '{1}', z1lyjg = '{2}', z1lynm = '{3}', z1ylno = '{4}'
                    , z1ylnm = '{5}', z1bzno = '{6}', z1bznm = '{7}', z1ybno = '{8}' where z1comp = '{9}' and z1zyno = '{10}'"
                         , yldylb, tcqh, lyjgdm, lyjgmc, yllb, yllbmc, bzbm, bzmc, grbh, CliUtils.fSiteCode, jzlsh);
                    liSQL.Add(strSql);

                    object[] obj = liSQL.ToArray();
                    obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);
                    if (obj[1].ToString() == "1")
                    {
                        WriteLog(sysdate + "  " + jzlsh + " 进入住院医保登记成功|" + outData.ToString());
                        return new object[] { 0, 1, "住院医保登记成功" };
                    }
                    else
                    {
                        WriteLog(sysdate + "  " + jzlsh + " 进入住院医保登记成功|操作本地数据失败|" + obj[2].ToString());
                        //住院登记（挂号）撤销
                        object[] objParam2 = new object[] { jzlsh, bxh2, xm2, kh2, dqbh1, jzlsh2 };
                        NYBMZDJCX(objParam2);
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
            catch (Exception ex)
            {
                WriteLog(sysdate + "  住院登记异常|" + ex.Message);
                return new object[] { 0, 0, "住院登记异常|" + ex.Message };
            }
        }
        #endregion

        #region 住院登记撤销
        public static object[] YBZYDJCX(object[] objParam)
        {
            string sysdate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            try
            {
                string Ywlx = "ZYDJCX";
                string jzlsh = objParam[0].ToString();  //就诊流水号
                string jbr = CliUtils.fUserName;   //经办人
                string cxrq = sysdate;                  //撤销日期时间
                string bxh = "";
                string xm = "";
                string kh = "";
                string ybjzlsh = "";
                if (string.IsNullOrEmpty(jzlsh))
                    return new object[] { 0, 0, "就诊流水号不能为空" };
                #region 时间有效性
                try
                {
                    string cxrq2 = Convert.ToDateTime(cxrq).ToString("yyyyMMdd");
                    string cxsj = Convert.ToDateTime(cxrq).ToString("HHmm");
                }
                catch
                {
                    return new object[] { 0, 0, "撤销时间格式错误" };
                }
                #endregion

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
                {
                    return new object[] { 0, 0, "该患者已上传费用明细，请先撤销费用明细" };
                }
                ds.Dispose();
                #endregion

                StringBuilder inputData = new StringBuilder();
                //入参: 保险号|姓名|卡号|地区编号|住院号
                inputData.Append(bxh + "|");
                inputData.Append(xm + "|");
                inputData.Append(kh + "|");
                inputData.Append(dqbh + "|");
                inputData.Append(ybjzlsh);

                StringBuilder outData = new StringBuilder(1024);
                StringBuilder retMsg = new StringBuilder(1024);

                WriteLog(sysdate + "  " + jzlsh + " 进入住院医保登记撤销...");
                WriteLog(sysdate + "  入参|" + inputData.ToString());
                int i = f_UserBargaingApply(Ywlx, inputData, outData, retMsg);
                if (i > 0)
                {
                    List<string> liSQL = new List<string>();
                    strSql = string.Format(@"insert into ybmzzydjdr(
                                       grbh,xm,kh,tcqh,bq,csrq,nl,nd,sfzh,dwmc,
                                       xb,yldylb,rycbzt,zhye,ybjzlsh,yllb,ksmc,ghf,zycs,bnzycs,
                                       ghdjsj,jbr,dgysdm,dgysxm,ksbh,jzbz,jzlsh,bzbm,bzmc,
                                       jylsh,mmbzbm1,mmbzmc1,sysdate,cxbz) select 
                                       grbh,xm,kh,tcqh,bq,csrq,nl,nd,sfzh,dwmc,
                                       xb,yldylb,rycbzt,zhye,ybjzlsh,yllb,ksmc,ghf,zycs,bnzycs,
                                       ghdjsj,jbr,dgysdm,dgysxm,ksbh,jzbz,jzlsh,bzbm,bzmc,
                                       jylsh,mmbzbm1,mmbzmc1,'{2}',0 from ybmzzydjdr where jzlsh='{0}' and ybjzlsh='{1}' and cxbz=1", jzlsh, ybjzlsh, sysdate);
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
            catch (Exception ex)
            {
                WriteLog(sysdate + "  住院登记撤销异常|" + ex.Message);
                return new object[] { 0, 0, "住院登记撤销异常|" + ex.Message };
            }
        }
        #endregion

        #region 住院收费登记
        public static object[] YBZYSFDJ(object[] objParam)
        {
            string sysdate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            try
            {
                string Ywlx = "ZYSFDJ";
                string jzlsh = objParam[0].ToString(); //就诊流水号
                string bxh = "";    //保险号
                string xm = "";     //姓名
                string kh = "";     //卡号
                string dgysxm = ""; //定岗医生姓名
                string ybjzlsh = "";//医保就诊流水号
                DateTime ryrq_tmp;

                if (string.IsNullOrEmpty(jzlsh))
                    return new object[] { 0, 0, "就诊流水号不能为空" };

                string jylsh = jzlsh + DateTime.Now.ToString("yyyyMMddHHmmss");


                #region 判断是否医保登记
                string strSql = string.Format("select convert(date,ghdjsj) as ryrq,* from ybmzzydjdr where jzlsh='{0}' and cxbz=1", jzlsh);
                DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                if (ds.Tables[0].Rows.Count == 0)
                    return new object[] { 0, 0, "该患者未办理医保登记" };
                else
                {
                    bxh = ds.Tables[0].Rows[0]["grbh"].ToString();
                    xm = ds.Tables[0].Rows[0]["xm"].ToString();
                    kh = ds.Tables[0].Rows[0]["kh"].ToString();
                    dqbh = ds.Tables[0].Rows[0]["tcqh"].ToString();
                    string dgysdm = ds.Tables[0].Rows[0]["dgysdm"].ToString();
                    dgysxm = ds.Tables[0].Rows[0]["dgysxm"].ToString();
                    ybjzlsh = ds.Tables[0].Rows[0]["ybjzlsh"].ToString();
                    ryrq_tmp = Convert.ToDateTime(ds.Tables[0].Rows[0]["ryrq"]);
                }
                #endregion

                StringBuilder inputData = new StringBuilder();
                //保险号|姓名|卡号|地区编号|住院号|开方医生
                inputData.Append(bxh + "|");
                inputData.Append(xm + "|");
                inputData.Append(kh + "|");
                inputData.Append(dqbh + "|");
                inputData.Append(ybjzlsh + "|");
                inputData.Append(dgysxm + ";");

                #region 收费明细信息
                List<string> li_yyxmbh = new List<string>();
                List<string> li_yyxmmc = new List<string>();
                List<string> li_inputData = new List<string>();
                List<string> li_ybxmbh = new List<string>();
                List<string> li_sn = new List<string>();
                List<string> li_dj = new List<string>();
                strSql = string.Format(@"select b.ybxmbh,b.ybxmmc,a.z3djxx as dj,
                                        sum(case LEFT(a.z3endv,1) when '4' then -a.z3jzcs else a.z3jzcs end) as sl,
                                        sum(case LEFT(a.z3endv,1) when '4' then -a.z3jzje else a.z3jzje end) as je,
                                        a.z3item as yyxmbh, a.z3name as yyxmmc,max(a.z3date) as yysj, z3sfno as sfno, 
                                        b.sfxmzldm as ybsfxmzldm, b.sflbdm as ybsflbdm,b.sfxmdjdm as xmlx from zy03d a left join ybhisdzdr b on 
                                        a.z3item=b.hisxmbh where a.z3ybup is null and LEFT(a.z3kind,1)in(2,4)  and a.z3zyno='{0}'
                                        group by b.ybxmbh,b.ybxmmc,a.z3djxx,a.z3item,a.z3name,a.z3sfno,b.sfxmzldm,b.sflbdm,b.sfxmdjdm
                                        having sum(case LEFT(a.z3endv,1) when '4' then -a.z3jzcs else a.z3jzcs end)>0
                                        union all
                                        select b.ybxmbh,b.ybxmmc,a.z3djxx as dj,
                                        sum(case LEFT(a.z3endv,1) when '4' then -a.z3jzcs else a.z3jzcs end) as sl,
                                        sum(case LEFT(a.z3endv,1) when '4' then -a.z3jzje else a.z3jzje end) as je,
                                        a.z3item as yyxmbh, a.z3name as yyxmmc,max(a.z3date) as yysj, z3sfno as sfno, 
                                        b.sfxmzldm as ybsfxmzldm, b.sflbdm as ybsflbdm,b.sfxmdjdm as xmlx from zy03dz a left join ybhisdzdr b on 
                                        a.z3item=b.hisxmbh where a.z3ybup is null and LEFT(a.z3kind,1)in(2,4)  and a.z3zyno='{0}'
                                        group by b.ybxmbh,b.ybxmmc,a.z3djxx,a.z3item,a.z3name,a.z3sfno,b.sfxmzldm,b.sflbdm,b.sfxmdjdm
                                        having sum(case LEFT(a.z3endv,1) when '4' then -a.z3jzcs else a.z3jzcs end) !=0", jzlsh);
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
                            double dj = Convert.ToDouble(dr["dj"]);
                            double sl = Convert.ToDouble(dr["sl"]);
                            double je = Convert.ToDouble(dr["je"]);
                            DateTime yyrq = Convert.ToDateTime(dr["yysj"].ToString());
                            string yysj = "";
                            if (DateTime.Compare(ryrq_tmp, yyrq) > 1)
                                yysj = ryrq_tmp.ToString("yyyyMMdd");
                            else
                                yysj = yyrq.ToString("yyyyMMdd");
                            string yyxmxx = dr["yyxmbh"].ToString() + "&" + dr["yyxmmc"].ToString();
                            StringBuilder inputData2 = new StringBuilder();
                            inputData2.Append(ybxmbh + "|");
                            inputData2.Append(ybxmmc + "|");
                            inputData2.Append(dj + "|");
                            inputData2.Append(sl + "|");
                            inputData2.Append(je + "|");
                            inputData2.Append(yysj + "|");
                            inputData2.Append(yyxmxx + ";");
                            li_inputData.Add(inputData2.ToString());
                            li_yyxmbh.Add(dr["yyxmbh"].ToString());
                            li_yyxmmc.Add(dr["yyxmmc"].ToString());
                            li_ybxmbh.Add(dr["ybxmbh"].ToString());
                            //li_sn.Add(dr["sn"].ToString());
                            li_dj.Add(dj.ToString());
                        }
                    }
                    if (!string.IsNullOrEmpty(strMsg.ToString()))
                        return new object[] { 0, 0, strMsg.ToString() };
                }
                else
                    return new object[] { 0, 0, "无费用明细" };
                #endregion

                List<string> liSQL = new List<string>();
                int iTemp = 0;
                #region 分段上传 每次50条
                foreach (string inputData3 in li_inputData)
                {
                    if (iTemp <= 10)
                    {
                        inputData.Append(inputData3);
                        iTemp++;
                    }
                    else
                    {
                        StringBuilder outData = new StringBuilder(102400);
                        StringBuilder retMsg = new StringBuilder(102400);

                        WriteLog(sysdate + "  " + jzlsh + " 进入住院费用明细上传(分段)...");
                        StringBuilder inputData1 = new StringBuilder();
                        inputData1.Append(inputData.ToString().TrimEnd(';'));

                        WriteLog(sysdate + "  入参|" + inputData1.ToString());
                        int i = f_UserBargaingApply(Ywlx, inputData1, outData, retMsg);
                        if (i <= 0)
                        {
                            WriteLog(sysdate + "  " + jzlsh + " 进入住院费用明细上传(分段)失败|" + retMsg.ToString());
                            return new object[] { 0, 0, "住院收费登记失败|" + retMsg.ToString() };
                        }
                        string[] str = outData.ToString().Split(';');
                        string[] array = str;
                        for (int k = 0; k < array.Length; k++)
                        {
                            string s = array[k];
                            string[] str2 = s.Split('|');
                            //出参:住院号|处方号|项目编号|项目名称|项目等级|收费类别|单价|数量|金额|处方日期|处方上传时间
                            string jzlsh2 = str2[0].Trim(); //住院号
                            string ybcfh = str2[1].Trim();  //处方号
                            string ybxmbm = str2[2].Trim(); //项目编号
                            string ybxmmc = str2[3].Trim(); //项目名称
                            string xmdj = str2[4].Trim();   //项目等级
                            string sflb = str2[5].Trim();   //收费类别
                            string dj2 = str2[6].Trim();    //单价
                            string sl2 = str2[7].Trim();    //数量
                            string je2 = str2[8].Trim();    //金额
                            string yyrq = str2[9].Trim();   //处方日期
                            string scsj = str2[10].Trim();  //处方上传时间

                            for (int j = 0; j < li_ybxmbh.Count; j++)
                            {
                                if (li_ybxmbh[j].Equals(ybxmbm) && li_dj[j].Equals(Convert.ToDouble(dj2).ToString()))
                                {
                                    //MessageBox.Show(li_dj[j] + "||" + Convert.ToDouble(dj2).ToString());
                                    strSql = string.Format(@"insert into ybcfmxscindr(jzlsh,jylsh,je,xm,kh,yysfxmbm,yysfxmmc,sysdate,ybjzlsh,ybcfh,sfxmzxbm,
                                                        sfxmzxmc,sfxmzl,sflb,dj,sl,cfrq,cfscsj) values(
                                                        '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}',
                                                        '{11}','{12}','{13}','{14}','{15}','{16}','{17}')",
                                                            jzlsh, jylsh, je2, xm, kh, li_yyxmbh[j], li_yyxmmc[j], sysdate, jzlsh2, ybcfh, ybxmbm,
                                                            ybxmmc, xmdj, sflb, dj2, sl2, yyrq, scsj);
                                    liSQL.Add(strSql);
                                    WriteLog(sysdate + "  " + jzlsh + "上传处方明细-->" + li_yyxmbh[j] + "|" + li_yyxmmc[j] + "|" + s);
                                    break;
                                }
                            }
                        }

                        iTemp = 1;
                        inputData.Remove(0, inputData.Length);
                        inputData.Append(bxh + "|");
                        inputData.Append(xm + "|");
                        inputData.Append(kh + "|");
                        inputData.Append(dqbh + "|");
                        inputData.Append(ybjzlsh + "|");
                        inputData.Append(dgysxm + ";");
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
                    StringBuilder inputData1 = new StringBuilder();
                    inputData1.Append(inputData.ToString().TrimEnd(';'));
                    WriteLog(sysdate + "  入参|" + inputData1.ToString());
                    int i = f_UserBargaingApply(Ywlx, inputData1, outData, retMsg);
                    if (i <= 0)
                    {
                        WriteLog(sysdate + "  " + jzlsh + " 进入住院费用明细上传(补传、一次性上传)失败|" + retMsg.ToString());
                        return new object[] { 0, 0, "住院收费登记失败|" + retMsg.ToString() };

                    }
                    string[] str = outData.ToString().Split(';');
                    string[] array = str;
                    for (int k = 0; k < array.Length; k++)
                    {
                        string s = array[k];
                        string[] str2 = s.Split('|');
                        string jzlsh2 = str2[0].Trim();
                        string ybcfh = str2[1].Trim();
                        string ybxmbm = str2[2].Trim();
                        string ybxmmc = str2[3].Trim();
                        string xmdj = str2[4].Trim();
                        string sflb = str2[5].Trim();
                        string dj2 = str2[6].Trim();
                        string sl2 = str2[7].Trim();
                        string je2 = str2[8].Trim();
                        string yyrq = str2[9].Trim();
                        string scsj = str2[10].Trim();
                        for (int j = 0; j < li_ybxmbh.Count; j++)
                        {
                            if (li_ybxmbh[j].Equals(ybxmbm) && li_dj[j].Equals(Convert.ToDouble(dj2).ToString()))
                            {
                                strSql = string.Format(@"insert into ybcfmxscindr(jzlsh,jylsh,je,xm,kh,yysfxmbm,yysfxmmc,sysdate,ybjzlsh,ybcfh,sfxmzxbm,
                                                    sfxmzxmc,sfxmzl,sflb,dj,sl,cfrq,cfscsj) values(
                                                    '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}',
                                                    '{11}','{12}','{13}','{14}','{15}','{16}','{17}')",
                                                        jzlsh, jylsh, je2, xm, kh, li_yyxmbh[j], li_yyxmmc[j], sysdate, jzlsh2, ybcfh, ybxmbm,
                                                        ybxmmc, xmdj, sflb, dj2, sl2, yyrq, scsj);
                                liSQL.Add(strSql);
                                WriteLog(sysdate + "  " + jzlsh + "上传处方明细-->" + li_yyxmbh[j] + "|" + li_yyxmmc[j] + "|" + s);
                                break;
                            }
                        }
                    }
                }
                #endregion

                strSql = string.Format(@"update zy03d set z3ybup = '{0}' where z3ybup is null and LEFT(z3kind,1)=2 and z3zyno = '{1}' ", ybjzlsh, jzlsh);
                liSQL.Add(strSql);
                strSql = string.Format(@"update zy03dz set z3ybup = '{0}' where z3ybup is null and LEFT(z3kind,1)=2 and z3zyno = '{1}' ", ybjzlsh, jzlsh);
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
                    object[] objParam2 = new object[] { jzlsh, bxh, xm, kh, ybjzlsh };
                    NYBZYSFDJCX(objParam2);
                    return new object[] { 0, 0, "住院收费登记失败" };
                }
            }
            catch (Exception ex)
            {
                WriteLog(sysdate + "  住院收费登记失败异常|" + ex.Message);
                return new object[] { 0, 0, "住院收费登记失败异常|" + ex.Message };
            }
        }
        #endregion

        #region 住院收费登记撤销(全部)
        public static object[] YBZYSFDJCX(object[] objParam)
        {
            string sysdate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            try
            {
                string Ywlx = "ZYSFQBTF";
                string jzlsh = objParam[0].ToString();
                string bxh = "";
                string xm = "";
                string kh = "";
                string ybjzlsh = "";

                if (string.IsNullOrEmpty(jzlsh))
                    return new object[] { 0, 0, "就诊流水号不能为空" };
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

                StringBuilder inputData = new StringBuilder();
                //入参:保险号|姓名|卡号|地区编号|住院号
                inputData.Append(bxh + "|");
                inputData.Append(xm + "|");
                inputData.Append(kh + "|");
                inputData.Append(dqbh + "|");
                inputData.Append(ybjzlsh + ";");
                StringBuilder outData = new StringBuilder(1024);
                StringBuilder retMsg = new StringBuilder(1024);
                WriteLog(sysdate + "  " + jzlsh + " 进入住院费用明细撤销(全部)...");
                int i = f_UserBargaingApply(Ywlx, inputData, outData, retMsg);
                if (i > 0)
                {
                    List<string> liSQL = new List<string>();
                    strSql = string.Format(@"insert into ybcfmxscindr(jzlsh,jylsh,je,xm,kh,yysfxmbm,yysfxmmc,ybjzlsh,ybcfh,sfxmzxbm,
                                         sfxmzxmc,sfxmzl,sflb,dj,sl,cfrq,cfscsj,sysdate,cxbz) select 
                                        jzlsh,jylsh,je,xm,kh,yysfxmbm,yysfxmmc,ybjzlsh,ybcfh,sfxmzxbm,
                                        sfxmzxmc,sfxmzl,sflb,dj,sl,cfrq,cfscsj,'{2}',0 from ybcfmxscindr 
                                        where jzlsh='{0}' and ybjzlsh='{1}' and cxbz=1 ", jzlsh, ybjzlsh, sysdate);
                    liSQL.Add(strSql);
                    strSql = string.Format("update ybcfmxscindr set cxbz=2 where jzlsh='{0}' and ybjzlsh='{1}' and cxbz=1", jzlsh, ybjzlsh);
                    liSQL.Add(strSql);
                    strSql = string.Format("update zy03d set z3ybup = null where z3ybup is not null and z3zyno = '{0}' and left(z3kind, 1) = '2'", jzlsh);
                    liSQL.Add(strSql); 
                    strSql = string.Format("update zy03dz set z3ybup = null where z3ybup is not null and z3zyno = '{0}' and left(z3kind, 1) = '2'", jzlsh);
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
            catch (Exception ex)
            {
                WriteLog(sysdate + "  住院收费登记撤销异常|" + ex.Message);
                return new object[] { 0, 0, "住院收费登记撤销异常|" + ex.Message };
            }
        }
        #endregion

        #region 住院收费预结算
        public static object[] YBZYSFYJS(object[] objParam)
        {
            string sysdate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            try
            {
                string Ywlx = "ZYSFYJS";

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
                string jbr = CliUtils.fLoginUser;
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
                }
                #endregion

                #region 是否已经医保结算
                strSql = string.Format(@"select * from ybfyjsdr where jzlsh='{0}' and cxbz=1", jzlsh);
                ds.Tables.Clear();
                ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                if (ds.Tables[0].Rows.Count > 0)
                    return new object[] { 0, 0, "该患者已办理医保结算" };
                ds.Dispose();
                #endregion

                #region 上传费用总金额
                strSql = string.Format("select SUM(je) from ybcfmxscindr where jzlsh='{0}' and ybjzlsh='{1}' and cxbz=1", jzlsh, ybjzlsh);
                ds.Tables.Clear();
                ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                if (string.IsNullOrEmpty(ds.Tables[0].Rows[0][0].ToString()))
                    return new object[] { 0, 0, "费用明细未上传" };

                double sfje3 = Convert.ToDouble(ds.Tables[0].Rows[0][0].ToString());
                if (Math.Abs(sfje2 - sfje3) > 1)
                    return new object[] { 0, 0, "总费用与医保结算总费用相差" + Math.Abs(sfje2 - sfje3) + ",无法结算，请核实!" };
                #endregion

                StringBuilder inputData = new StringBuilder();
                //入参:保险号|姓名|卡号|地区编号|住院号|病种编号|病种名称|处方起始日期|处方截止日期|出院原因|出院日期|出院时间|是否打印票据|出院诊断
                inputData.Append(bxh + "|");
                inputData.Append(xm + "|");
                inputData.Append(kh + "|");
                inputData.Append(dqbh + "|");
                inputData.Append(ybjzlsh + "|");
                inputData.Append(bzbh + "|");
                inputData.Append(bzmc + "|");
                inputData.Append(cfqsrq + "|");
                inputData.Append(cfjzrq + "|");
                inputData.Append(cyyy + "|");
                inputData.Append(cyrq + "|");
                inputData.Append(cysj + "|");
                inputData.Append(sfdypj + "|");
                inputData.Append(cyzd);
                StringBuilder outData = new StringBuilder(10240);
                StringBuilder retMsg = new StringBuilder(1024);

                WriteLog(sysdate + "  " + jzlsh + " 进入住院收费预结算...");
                WriteLog(sysdate + " 入参|" + inputData.ToString());
                int i = f_UserBargaingApply(Ywlx, inputData, outData, retMsg);
                WriteLog(sysdate + " 出参|" + outData.ToString());
                if (i > 0)
                {
                    #region 出参
                    /*0546970923|周丕鸿|36082117131678|360821|吉安县|19621117|56|20180101|3608217|周丕鸿 - 永和镇西坑村委会|
                     * 男|居民-建档立卡人员|正常|0|76069529|83360246|普通住院|康复科|8|NULL|20180910|
                     * 1034|20180930|1037|NULL|4695.98|0|734.29|1810.67|2151.02|0|
                     * 0|0|3796.13|867.85|0|0|刘伟凤|20180930|1639|0|
                     * 0|86.79|0|452.67|239|0|0|
                     * 0|0|0|0|0|0|0
                     * 
                     * 出参:保险号|姓名|卡号|地区编号|地区名称|出生日期|实际年龄|参保日期|个人身份|单位名称|
                     * 性别|医疗人员类别|卡状态|账户余额|门诊(住院)号|单据流水号|医疗类别|科室名称|本次看病次数|住院床号|入院日期|
                     * 入院时间|出院日期|出院时间|出院原因|医疗总费用|本次账户支付|本次现金支付|本次基金支付|大病基金支付|救助金额|
                     * 单位补充医保支付|离休干部单独统筹支付|甲类费用|乙类费用|丙类费用|自费费用|结算人|结算日期|结算时间|起付标准自付|
                     * 非医保自付|乙类药品自付|特检特治自付|进入统筹自付|进入大病自付|重大疾病范围内补偿金额|重大疾病范围外补偿金额|
                     * 医院负担金额|大病二次补偿|民政大病救助基金|政府兜底基金|其中公务员补助部分
                     */
                    string[] str = outData.ToString().Split(';');
                    outParams_js js = new outParams_js();
                    string[] str2 = str[0].Split('|');
                    js.Grbh = str2[0].Trim();       //保险号
                    js.Xm = str2[1].Trim();        //姓名
                    js.Kh = str2[2].Trim();        //卡号
                    js.Dqbh = str2[3].Trim();      //地区编号
                    js.Dqmc = str2[4].Trim();       //地区名称
                    js.Csrq = str2[5].Trim();       //出生日期
                    string sjnl = str2[6].Trim();       //实际年龄
                    js.Cbrq = str2[7].Trim();       //参保日期
                    string grsf = str2[8].Trim();       //个人身份
                    js.Dwmc = str2[9].Trim();       //单位名称
                    js.Xb = str2[10].Trim();        //性别
                    js.Yldylb = str2[11].Trim();    //医疗人员类别
                    string kzt = str2[12].Trim();       //卡状态
                    js.Bcjshzhye = str2[13].Trim();      //账户余额
                    js.Ybjzlsh = str2[14].Trim();    //门诊(住院)号
                    js.Jslsh = str2[15].Trim();     //单据流水号
                    js.Yllb = str2[16].Trim();      //医疗类别
                    string ksmc = str2[17].Trim();      //科室名称
                    js.Zycs = str2[18].Trim();      //本次看病次数
                    string zych = str2[19].Trim();      //住院床号
                    string ryrq = str2[20].Trim();      //入院日期
                    string rysj = str2[21].Trim();      //入院时间
                    string cyrq2 = str2[22].Trim();      //出院日期
                    string cysj2 = str2[23].Trim();      //出院时间
                    string cyyy2 = str2[24].Trim();      //出院原因
                    js.Ylfze = str2[25].Trim();     //医疗总费用
                    js.Zhzf = str2[26].Trim();      //本次账户支付
                    js.Xjzf = str2[27].Trim();      //本次现金支付
                    js.Tcjjzf = str2[28].Trim();    //本次基金支付
                    js.Dejjzf = str2[29].Trim();    //大病基金支付
                    js.Mzjzfy = str2[30].Trim();    //救助金额
                    js.Dwfdfy = str2[31].Trim();    //单位补充医保支付
                    js.Lxgbdttcjjzf = str2[32].Trim();//离休干部单独统筹支付
                    js.Jlfy = str2[33].Trim();      //甲类费用
                    js.Ylfy = str2[34].Trim();      //乙类费用
                    js.Blfy = str2[35].Trim();      //丙类费用
                    js.Zffy = str2[36].Trim();      //自费费用
                    js.Jbr = str2[37].Trim();       //结算人
                    js.Jsrq = str2[38].Trim();      //结算日期
                    js.Jssj = str2[39].Trim();      //结算时间
                    js.Qfbzfy = str2[40].Trim();    //起付标准自付
                    js.Fybzf = str2[41].Trim();     //非医保自付
                    js.Ypfy = str2[42].Trim();    //乙类药品自付
                    js.Tjzlfy = str2[43].Trim();    //特检特治自付
                    js.Tcfdzffy = str2[44].Trim();    //进入统筹自付
                    js.Defdzffy = str2[45].Trim();  //进入大病自付
                    js.Zdjbfwnbxfy = str2[46].Trim(); //重大疾病范围内补偿金额
                    js.Zdjbfwybxfy = str2[47].Trim(); //重大疾病范围外补偿金额
                    js.Yyfdfy = str2[48].Trim(); //医院负担金额
                    js.Ecbcje = str2[49].Trim(); //大病二次补偿
                    js.Mzjbjzfy = str2[50].Trim(); //民政大病救助基金
                    js.Zftdfy = str2[51].Trim(); //政府兜底基金
                    js.Gwybzjjzf = str2[52].Trim();   //其中公务员补助部分
                    js.Bcjsqzhye = (Convert.ToDecimal(js.Bcjshzhye) + Convert.ToDecimal(js.Zhzf)).ToString(); //本次结算前帐户余额

                    js.Qtybzf = "0.00";
                    js.Qybcylbxjjzf = "0.00";

                    if (js.Xjzf.Equals("-.01"))
                    {
                        js.Ybxjzf = "0.00";
                        js.Xjzf = "-0.01";
                    }
                    else
                    {
                        js.Ybxjzf = js.Xjzf;
                    }
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
                     * 居民个人自付二次补偿金额|体检金额|生育基金支付|其他医保支付
                     */
                    //计算总报销金额
                    js.Zbxje = (Convert.ToDecimal(js.Ylfze) - Convert.ToDecimal(js.Xjzf)).ToString();
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

                    List<string> liSQL = new List<string>();
                    strSql = string.Format(@"delete from ybfyyjsdr where jzlsh='{0}'", jzlsh);
                    liSQL.Add(strSql);
                    strSql = string.Format(@"insert into ybfyyjsdr(jzlsh,jylsh,djhin,cyrq,cyyy,zhsybz,ztjsbz,jbr,xm,kh,
                                            grbh,jsrq,yllb,yldylb,jslsh,ylfze,zhzf,xjzf,tcjjzf,dejjzf,
                                            mzjzfy,dwfdfy,lxgbddtczf,jlfy,ylfy,blfy,zffy,qfbzfy,fybzf,ylypzf,
                                            tjtzzf,tcfdzffy,jrdbzf,zdjbfwnbcje,zdjbfwybcje,yyfdfy,ecbcje,mzdbjzje,czzcje,gwybzjjzf,
				                            sysdate,djh,ryrq,zbxje,bcjsqzhye,ybjzlsh,qtybfy) values(
                                            '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}',
                                            '{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}',
                                            '{20}','{21}','{22}','{23}','{24}','{25}','{26}','{27}','{28}','{29}',
                                            '{30}','{31}','{32}','{33}','{34}','{35}','{36}','{37}','{38}','{39}',
                                            '{40}','{41}','{42}','{43}','{44}','{45}','{46}')",
                                             jzlsh, js.Jslsh, djh, cyrq2 + cysj2, cyyy, 1, 0, jbr, js.Xm, js.Kh,
                                             js.Grbh, js.Jsrq, js.Yllb, js.Yldylb, js.Jslsh, js.Ylfze, js.Zhzf, js.Xjzf, js.Tcjjzf, js.Dejjzf,
                                             js.Mzjzfy, js.Dwfdfy, js.Lxgbdttcjjzf, js.Jlfy, js.Ylfy, js.Blfy, js.Zffy, js.Qfbzfy, js.Fybzf, js.Ypfy,
                                             js.Tjzlfy, js.Tcfdzffy, js.Defdzffy, js.Zdjbfwnbxfy, js.Zdjbfwybxfy, js.Yyfdfy, js.Ecbcje, js.Mzjbjzfy, js.Zftdfy, js.Gwybzjjzf,
                                             sysdate, djh, ryrq + rysj, js.Zbxje, js.Bcjsqzhye, js.Ybjzlsh, js.Qtybzf);
                    liSQL.Add(strSql);
                    object[] obj = liSQL.ToArray();
                    obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);
                    if (obj[1].ToString() == "1")
                    {
                        WriteLog(sysdate + "  " + jzlsh + " 进入住院收费预结算成功" + strValue);
                        //医保预结算单
                        YBZYYJSD(objParam);
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
            string sysdate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string Ywlx = "ZYSFJS";
            try
            {
                string jzlsh = objParam[0].ToString();      //就诊流水号
                string djh = objParam[1].ToString();        //单据号（发票号）
                string cyyy = objParam[2].ToString();       //出院原因
                string zhsybz = objParam[3].ToString();     //账户使用标志
                string ztjsbz = objParam[4].ToString();     //中途结算标志
                string jsrqsj = objParam[5].ToString();     //结算日期时间
                string sfdypj = "FALSE";     //是否打印票据
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
                string jbr = CliUtils.fLoginUser;
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
                }
                #endregion

                #region 是否慢病患者
                strSql = string.Format("select * from ybmxbdj where jzlsh='{0}'", jzlsh);
                ds.Tables.Clear();
                ds = ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    bzbh = ds.Tables[0].Rows[0]["mmbzbm"].ToString();
                    bzmc = ds.Tables[0].Rows[0]["mmbzmc"].ToString();
                }
                #endregion

                #region 出院诊断
                strSql = string.Format(@"select m1xynm from mza1dd where m1mzno='{0}' ", jzlsh);
                ds.Tables.Clear();
                ds = ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    cyzd = ds.Tables[0].Rows[0]["m1xynm"].ToString();
                }
                #endregion

                #region 费用总额
                strSql = string.Format("select SUM(je) from ybcfmxscindr where jzlsh='{0}' and ybjzlsh='{1}' and cxbz=1", jzlsh, ybjzlsh);
                ds.Tables.Clear();
                ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                if (string.IsNullOrEmpty(ds.Tables[0].Rows[0][0].ToString()))
                    return new object[] { 0, 0, "费用明细未上传" };

                double sfje3 = Convert.ToDouble(ds.Tables[0].Rows[0][0].ToString());
                if (Math.Abs(sfje2 - sfje3) > 1.0)
                    return new object[] { 0, 0, "总费用与医保结算总费用相差" + Math.Abs(sfje2 - sfje3) + ",无法结算，请核实!" };
                #endregion

                StringBuilder inputData = new StringBuilder();
                //入参:保险号|姓名|卡号|地区编号|住院号|病种编号|病种名称|处方起始日期|处方截止日期|出院原因|出院日期|出院时间|是否打印票据|出院诊断
                inputData.Append(bxh + "|");
                inputData.Append(xm + "|");
                inputData.Append(kh + "|");
                inputData.Append(dqbh + "|");
                inputData.Append(ybjzlsh + "|");
                inputData.Append(bzbh + "|");
                inputData.Append(bzmc + "|");
                inputData.Append(cfqsrq + "|");
                inputData.Append(cfjzrq + "|");
                inputData.Append(cyyy + "|");
                inputData.Append(cyrq + "|");
                inputData.Append(cysj + "|");
                inputData.Append(sfdypj + "|");
                inputData.Append(cyzd);
                StringBuilder outData = new StringBuilder(10240);
                StringBuilder retMsg = new StringBuilder(1024);

                WriteLog(sysdate + "  " + jzlsh + " 进入住院收费结算...");
                WriteLog(sysdate + " 入参|" + inputData.ToString());
                int i = f_UserBargaingApply(Ywlx, inputData, outData, retMsg);
                if (i > 0)
                {
                    List<string> liSQL = new List<string>();
                    #region 出参
                    /*出参:保险号|姓名|卡号|地区编号|地区名称|出生日期|实际年龄|参保日期|个人身份|单位名称|
                     * 性别|医疗人员类别|卡状态|账户余额|门诊(住院)号|单据流水号|医疗类别|科室名称|本次看病次数|住院床号|入院日期|
                     * 入院时间|出院日期|出院时间|出院原因|医疗总费用|本次账户支付|本次现金支付|本次基金支付|大病基金支付|救助金额|
                     * 单位补充医保支付|离休干部单独统筹支付|甲类费用|乙类费用|丙类费用|自费费用|结算人|结算日期|结算时间|起付标准自付|
                     * 非医保自付|乙类药品自付|特检特治自付|进入统筹自付|进入大病自付|重大疾病范围内补偿金额|重大疾病范围外补偿金额|
                     * 医院负担金额|大病二次补偿|民政大病救助基金|政府兜底基金|其中公务员补助部分
                     */
                    outParams_js js = new outParams_js();
                    string[] str = outData.ToString().Split(';');
                    string[] str2 = str[0].Split('|');
                    js.Grbh = str2[0].Trim();       //保险号
                    js.Xm = str2[1].Trim();        //姓名
                    js.Kh = str2[2].Trim();        //卡号
                    js.Dqbh = str2[3].Trim();      //地区编号
                    js.Dqmc = str2[4].Trim();       //地区名称
                    js.Csrq = str2[5].Trim();       //出生日期
                    string sjnl = str2[6].Trim();       //实际年龄
                    js.Cbrq = str2[7].Trim();       //参保日期
                    string grsf = str2[8].Trim();       //个人身份
                    js.Dwmc = str2[9].Trim();       //单位名称
                    js.Xb = str2[10].Trim();        //性别
                    js.Yldylb = str2[11].Trim();    //医疗人员类别
                    string kzt = str2[12].Trim();       //卡状态
                    js.Bcjshzhye = str2[13].Trim();      //账户余额
                    js.Ybjzlsh = str2[14].Trim();    //门诊(住院)号
                    js.Jslsh = str2[15].Trim();     //单据流水号
                    js.Yllb = str2[16].Trim();      //医疗类别
                    string ksmc = str2[17].Trim();      //科室名称
                    js.Zycs = str2[18].Trim();      //本次看病次数
                    string zych = str2[19].Trim();      //住院床号
                    string ryrq = str2[20].Trim();      //入院日期
                    string rysj = str2[21].Trim();      //入院时间
                    string cyrq2 = str2[22].Trim();      //出院日期
                    string cysj2 = str2[23].Trim();      //出院时间
                    string cyyy2 = str2[24].Trim();      //出院原因
                    js.Ylfze = str2[25].Trim();     //医疗总费用
                    js.Zhzf = str2[26].Trim();      //本次账户支付
                    js.Xjzf = str2[27].Trim();      //本次现金支付
                    js.Tcjjzf = str2[28].Trim();    //本次基金支付
                    js.Dejjzf = str2[29].Trim();    //大病基金支付
                    js.Mzjzfy = str2[30].Trim();    //救助金额
                    js.Dwfdfy = str2[31].Trim();    //单位补充医保支付
                    js.Lxgbdttcjjzf = str2[32].Trim();//离休干部单独统筹支付
                    js.Jlfy = str2[33].Trim();      //甲类费用
                    js.Ylfy = str2[34].Trim();      //乙类费用
                    js.Blfy = str2[35].Trim();      //丙类费用
                    js.Zffy = str2[36].Trim();      //自费费用
                    js.Jbr = str2[37].Trim();       //结算人
                    js.Jsrq = str2[38].Trim();      //结算日期
                    js.Jssj = str2[39].Trim();      //结算时间
                    js.Qfbzfy = str2[40].Trim();    //起付标准自付
                    js.Fybzf = str2[41].Trim();     //非医保自付
                    js.Ypfy = str2[42].Trim();    //乙类药品自付
                    js.Tjzlfy= str2[43].Trim();    //特检特治自付
                    js.Tcfdzffy = str2[44].Trim();    //进入统筹自付
                    js.Defdzffy = str2[45].Trim();  //进入大病自付
                    js.Zdjbfwnbxfy= str2[46].Trim(); //重大疾病范围内补偿金额
                    js.Zdjbfwybxfy = str2[47].Trim(); //重大疾病范围外补偿金额
                    js.Yyfdfy = str2[48].Trim(); //医院负担金额
                    js.Ecbcje = str2[49].Trim(); //大病二次补偿
                    js.Mzjbjzfy = str2[50].Trim(); //民政大病救助基金
                    js.Zftdfy = str2[51].Trim(); //政府兜底基金
                    js.Gwybzjjzf = str2[52].Trim();   //其中公务员补助部分
                    js.Bcjsqzhye = (Convert.ToDecimal(js.Bcjshzhye) + Convert.ToDecimal(js.Zhzf)).ToString(); //本次结算前帐户余额

                    js.Qtybzf = "0.00";
                    js.Qybcylbxjjzf = "0.00";

                    if (js.Xjzf.Equals("-.01"))
                    {
                        js.Ybxjzf = "0.00";
                        js.Xjzf = "-0.01";
                    }
                    else
                    {
                        js.Ybxjzf = js.Xjzf;
                    }
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
                     * 居民个人自付二次补偿金额|体检金额|生育基金支付|其他医保支付
                     */
                    //计算总报销金额
                    js.Zbxje = (Convert.ToDecimal(js.Ylfze) - Convert.ToDecimal(js.Xjzf)).ToString();
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

                    strSql = string.Format(@"insert into ybfyjsdr(jzlsh,jylsh,djhin,cyrq,cyyy,zhsybz,ztjsbz,jbr,xm,kh,
                                            grbh,jsrq,yllb,yldylb,jslsh,ylfze,zhzf,xjzf,tcjjzf,dejjzf,
                                            mzjzfy,dwfdfy,lxgbddtczf,jlfy,ylfy,blfy,zffy,qfbzfy,fybzf,ylypzf,
                                            tjtzzf,tcfdzffy,jrdbzf,zdjbfwnbcje,zdjbfwybcje,yyfdfy,ecbcje,mzdbjzje,czzcje,gwybzjjzf,
				                            sysdate,djh,ryrq,zbxje,bcjsqzhye,ybjzlsh,qtybfy) values(
                                            '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}',
                                            '{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}',
                                            '{20}','{21}','{22}','{23}','{24}','{25}','{26}','{27}','{28}','{29}',
                                            '{30}','{31}','{32}','{33}','{34}','{35}','{36}','{37}','{38}','{39}',
                                            '{40}','{41}','{42}','{43}','{44}','{45}','{46}')",
                                              jzlsh, js.Jslsh, djh, cyrq2 + cysj2, cyyy, 1, 0, jbr, js.Xm, js.Kh,
                                              js.Grbh, js.Jsrq, js.Yllb, js.Yldylb, js.Jslsh, js.Ylfze, js.Zhzf, js.Xjzf, js.Tcjjzf, js.Dejjzf,
                                              js.Mzjzfy, js.Dwfdfy, js.Lxgbdttcjjzf, js.Jlfy, js.Ylfy, js.Blfy, js.Zffy, js.Qfbzfy, js.Fybzf, js.Ypfy,
                                              js.Tjzlfy, js.Tcfdzffy, js.Defdzffy, js.Zdjbfwnbxfy, js.Zdjbfwybxfy, js.Yyfdfy, js.Ecbcje, js.Mzjbjzfy, js.Zftdfy, js.Gwybzjjzf,
                                              sysdate, djh, ryrq + rysj, js.Zbxje, js.Bcjsqzhye, js.Ybjzlsh, js.Qtybzf);
                    liSQL.Add(strSql);
                    object[] obj = liSQL.ToArray();
                    obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);
                    if (obj[1].ToString() == "1")
                    {
                        WriteLog(sysdate + "  " + jzlsh + " 进入住院收费结算成功" + strValue);
                        //打印结算单
                        //object[] objParam1 = { jzlsh };
                        //YBZYJSD(objParam1);
                        return new object[] { 0, 1, strValue };
                    }
                    else
                    {
                        WriteLog(sysdate + "  " + jzlsh + " 进入住院收费结算成功|操作本地数据失败|" + obj[2].ToString());
                        //住院收费结算撤销
                        object[] objParam2 = new object[] { jzlsh, js.Grbh, js.Xm, js.Kh, js.Dqbh, js.Ybjzlsh, js.Jslsh };
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
            catch(Exception ex)
            {
                WriteLog(sysdate+"  住院结算失败"+ex.Message);
                return new object[] { 0, 0, "住院结算失败" };
            }
        }
        #endregion

        #region 住院收费结算撤销
        public static object[] YBZYSFJSCX(object[] objParam)
        {
            string sysdate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            try
            {
                string Ywlx = "ZYSFCX";
                string jzlsh = objParam[0].ToString();
                string djh = objParam[1].ToString();
                string dkbz = objParam[2].ToString(); //读卡标志
                string bxh = "";
                string xm = "";
                string kh = "";
                string ybjzlsh = "";
                string djlsh = "";
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
                strSql = string.Format("select jslsh from ybfyjsdr where jzlsh='{0}' and djh='{1}'", jzlsh, djh);
                ds.Tables.Clear();
                ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                if (ds.Tables[0].Rows.Count == 0)
                    return new object[] { 0, 0, "该患者未办理住院收费结算" };
                else
                    djlsh = ds.Tables[0].Rows[0]["jslsh"].ToString();

                StringBuilder inputData = new StringBuilder();
                //入参:
                inputData.Append(bxh + "|");
                inputData.Append(xm + "|");
                inputData.Append(kh + "|");
                inputData.Append(dqbh + "|");
                inputData.Append(ybjzlsh + "|");
                inputData.Append(djlsh + "|");
                inputData.Append(sfmxsfzf);
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
                    strSql = string.Format(@"insert into ybcfmxscindr(jzlsh,jylsh,je,xm,kh,yysfxmbm,yysfxmmc,ybjzlsh,ybcfh,sfxmzxbm,
                                         sfxmzxmc,sfxmzl,sflb,dj,sl,cfrq,cfscsj,sysdate,cxbz) select 
                                        jzlsh,jylsh,je,xm,kh,yysfxmbm,yysfxmmc,ybjzlsh,ybcfh,sfxmzxbm,
                                        sfxmzxmc,sfxmzl,sflb,dj,sl,cfrq,cfscsj,'{2}',0 from ybcfmxscindr 
                                        where jzlsh='{0}' and ybjzlsh='{1}' and cxbz=1 ", jzlsh, ybjzlsh, sysdate);
                    liSQL.Add(strSql);
                    strSql = string.Format("update ybcfmxscindr set cxbz=2 where jzlsh='{0}' and ybjzlsh='{1}' and cxbz=1", jzlsh, ybjzlsh);
                    liSQL.Add(strSql);
                    strSql = string.Format("update zy03d set z3ybup = null where z3ybup is not null and z3zyno = '{0}' and left(z3kind, 1) = '2'", jzlsh);
                    liSQL.Add(strSql);

                    object[] obj = liSQL.ToArray();
                    obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);
                    if (obj[1].ToString() == "1")
                    {
                        WriteLog(sysdate + "  " + jzlsh + " 进入住院收费结算撤销成功" + outData.ToString());
                        //object[] objParam1 = { jzlsh };
                        //YBZYSFDJCX(objParam1);
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
            catch (Exception ex)
            {
                WriteLog(sysdate + "  住院结算撤销失败" + ex.Message);
                return new object[] { 0, 0, "住院结算撤销失败" };
            }
        }
        #endregion

        #region 打印住院预结算单
        internal static object[] YBZYYJSD(object[] objParam)
        {
            string param = objParam[0].ToString();
            if (string.IsNullOrEmpty(param))
                return new object[] { 0, 0, "住院号不能为空" };
            WriteLog("打印住院预结算单");
            string strSql = string.Format(@"select * from ybfyyjsdr where jzlsh='{0}' and cxbz=1", param);
            DataSet ds_js = CliUtils.ExecuteSql("szy01", "cmd", strSql, CliUtils.fLoginDB.ToString(), true, CliUtils.fCurrentProject.ToString());
            if (ds_js.Tables[0].Rows.Count == 0)
                return new object[] { 0, 0, "该患者未医保预结算" };

            //基本信息
            strSql = string.Format(@"select a.yldylb,'吉安县中医院' as yljgmc,a.grbh,a.ybjzlsh,
                                    a.xm+'['+b.RYLB+']' as xm,a.dwmc,a.ksmc 
                                    from ybmzzydjdr a
                                    left join YBICKXX b on a.grbh=b.GRBH  
                                    where jzlsh='{0}' and cxbz=1", param);
            DataSet ds = CliUtils.ExecuteSql("szy01", "cmd", strSql, CliUtils.fLoginDB.ToString(), true, CliUtils.fCurrentProject.ToString());
            ds.Tables[0].TableName = "base";

            //费用构成
            strSql = string.Format(@"select convert(decimal(8,2),SUM(je)) as je,sflb from ybcfmxscindr where jzlsh='{0}' and cxbz=1 group by sflb", param);
            DataSet ds_detail = CliUtils.ExecuteSql("szy01", "cmd", strSql, CliUtils.fLoginDB.ToString(), true, CliUtils.fCurrentProject.ToString());
            ds_detail.Tables[0].TableName = "detail";
            ds.Tables.Add(ds_detail.Tables[0].Copy());

            //结算信息
            strSql = string.Format(@"with ybjs as
                                    (
                                    select 
                                    jzlsh,
                                    convert(datetime,substring(left(ryrq,8)+' ' + substring(ryrq,9,2)+':' + substring(ryrq,11,2),1,120)) as ryrqsj, --入院时间
                                    convert(datetime,substring(left(cyrq,8)+' ' + substring(cyrq,9,2)+':' + substring(cyrq,11,2),1,120)) as cyrqsj, --出院时间
                                    isnull(ylfze,'0.00') as ylfze, --医疗费用总额
                                    isnull(tcjjzf,'0.00') as tcjjzf, --统筹基金支付
                                    isnull(gwybzjjzf,'0.00') as gwybz,--公务员补助
                                    isnull(yyfdfy,'0.00') as yyfd,	--医院负担
                                    (bcjsqzhye-zhzf) as zhye, --帐户余额
                                    (convert(decimal(8,2),isnull(z3amt1,'0.00'))+CONVERT(decimal(8,2),ISNULL(z3mzzy,'0.00'))) as z3mat1, --预缴金累计
                                    convert(decimal(8,2),isnull(case when z3amtj>0 and  left(z3stbz,1)=0 then -z3amnt when z3amtj=0 then (ylfze-xjzf) else z3amnt end,'0.00')) z3amnt, --预缴金剩余
                                    ISNULL(zhzf,'0.00') as zhzf, --个人账户支付
                                    --case when dejjzf=0 then ISNULL(xjzf+zhzf+gwybzjjzf,'0.00')
                                    --else ISNULL(dejjzf+jrdbzf+zffy,'0.00') end xjzf,	--费用负担总额（现金支付）
                                    ISNULL(qfbzfy,'0.00') as qfbzfy, --起付线
                                    case when dejjzf=0 then isnull(ylypzf+tjtzzf,'0.00')
                                    else '0.00' end as jzfd, --加重负担
                                    isnull(tcfdzffy,'0.00') as ablfd, --按比例负担
                                    ISNULL(zffy,'0.00') as zffy, --自费金额
                                    case when dejjzf=0 then '0.00'
                                    when  yldylb like '%建档立卡%' then isnull(fybzf,0.00)
                                    else '0.00' end as fybzf, --
                                    ISNULL(dejjzf,'0.00') as dejjzf, --大病基金支付
                                    case when dejjzf=0 then '0.00'
                                    when  yldylb like '%建档立卡%' then ISNULL(jrdbzf+dejjzf+xjzf-ylypzf,'0.00')
                                    else ISNULL(jrdbzf+dejjzf-ylypzf,'0.00') end as jrdbzf, --进入大病费用
                                    case when dejjzf=0 then 0.00
                                    else isnull(ylypzf+tjtzzf,'0.00') end  as dbjzfd,--加重负担
                                    ISNULL(ecbcje,'0.00') as dbecbcfy, --大病二次补偿
                                    ISNULL(zdjbfwnbcje,'0.00') as zdjbfwnbcfy, --重大疾病内补
                                    ISNULL(zdjbfwybcje,'0.00') as zdjbfwybcfy,--重大疾病外补
                                    ISNULL(czzcje,'0.00') as zftdfy--政府兜底基金
                                    from ybfyyjsdr a
                                    left join  zy03dw b on  b.z3zyno=a.jzlsh and LEFT(z3endv,1)=1
                                    where jzlsh='{0}' and cxbz=1
                                    )
                                    select 
                                    ryrqsj,cyrqsj,ylfze,tcjjzf,gwybz,yyfd,zhye,z3mat1,z3amnt,zhzf,qfbzfy,
                                    ablfd,zffy,dejjzf,dbecbcfy,zdjbfwnbcfy,zdjbfwybcfy,zftdfy,
                                    case when isnull(b.jzlsh,'')!='' then b.jzfdfy else  a.jzfd end as jzfd,
                                    case when isnull(b.jzlsh,'')!='' then b.dbjzfdfy else  a.dbjzfd end as dbjzfd,
                                    case when isnull(b.jzlsh,'')!='' then b.jrdbfy else  a.jrdbzf end as jrdbzf,
                                    case when isnull(b.jzlsh,'')!='' then qfbzfy+b.jzfdfy+a.ablfd+a.zffy+b.jrdbfy+b.dbjzfdfy-fybzf else  
                                    qfbzfy+a.jzfd+a.ablfd+a.zffy+a.jrdbzf+a.dbjzfd-fybzf end as xjzf --费用负担总额（现金支付）
                                    from ybjs a
                                    left join ybjsdsz b on a.jzlsh=b.jzlsh", param);
            DataSet ds_jsinfo = CliUtils.ExecuteSql("szy01", "cmd", strSql, CliUtils.fLoginDB.ToString(), true, CliUtils.fCurrentProject.ToString());
            ds_jsinfo.Tables[0].TableName = "jsinfo";
            ds.Tables.Add(ds_jsinfo.Tables[0].Copy());
            //ds.WriteXmlSchema(@"D:\mm.xml");
            string s_path = string.Empty;
            s_path = string.IsNullOrEmpty(s_path) ? @"C:\Program Files (x86)\Infolight\EEP2012\EEPNetClient\FastReport" : s_path;//空值取默认

            string c_file = Application.StartupPath + @"\FastReport\YB\医保预结算单_吉安.frx"; //client
            string s_file = s_path + @"\YB\医保预结算单_吉安.frx";    //server  
            CliUtils.DownLoad(s_file, c_file);
            try
            {
                //检查报表文件是否存
                if (!File.Exists(c_file))
                {
                    ds.Dispose();
                    WriteLog("不存在");
                    return new object[] { 0, 0, c_file + "不存在" };
                }
                else
                {
                    Report report = new Report();
                    report.PrintSettings.ShowDialog = false;
                    report.Load(c_file);
                    report.RegisterData(ds);
                    report.Show();
                    report.Dispose();
                    WriteLog("住院预结算单打印成功");
                    return new object[] { 0, 1, "住院预结算单打印成功" };
                }
            }
            catch (Exception ex)
            {
                WriteLog(ex.Message);
                return new object[] { 0, 0, ex.Message };
            }
        }
        #endregion

        #region 打印住院结算单
        public static object[] YBZYJSD(object[] objParam)
        {
            string param = objParam[0].ToString();
            if (string.IsNullOrEmpty(param))
                return new object[] { 0, 0, "住院号不能为空" };

            string strSql = string.Format(@"select * from ybfyjsdr where jzlsh='{0}' and cxbz=1", param);
            DataSet ds_js = CliUtils.ExecuteSql("szy01", "cmd", strSql, CliUtils.fLoginDB.ToString(), true, CliUtils.fCurrentProject.ToString());
            if (ds_js.Tables[0].Rows.Count == 0)
                return new object[] { 0, 0, "该患者未医保结算" };

            //基本信息
            strSql = string.Format(@"select a.yldylb,'吉安县中医院' as yljgmc,a.grbh,a.ybjzlsh,
                                        a.xm+'['+b.RYLB+']' as xm,a.dwmc,a.ksmc 
                                        from ybmzzydjdr a
                                        left join YBICKXX b on a.grbh=b.GRBH  
                                        where jzlsh='{0}' and cxbz=1", param);
            DataSet ds = CliUtils.ExecuteSql("szy01", "cmd", strSql, CliUtils.fLoginDB.ToString(), true, CliUtils.fCurrentProject.ToString());
            ds.Tables[0].TableName = "base";

            //费用构成
            strSql = string.Format(@"select convert(decimal(8,2),SUM(je)) as je,sflb from ybcfmxscindr where jzlsh='{0}' and cxbz=1 group by sflb", param);
            DataSet ds_detail = CliUtils.ExecuteSql("szy01", "cmd", strSql, CliUtils.fLoginDB.ToString(), true, CliUtils.fCurrentProject.ToString());
            ds_detail.Tables[0].TableName = "detail";
            ds.Tables.Add(ds_detail.Tables[0].Copy());

            //结算信息
            strSql = string.Format(@"with ybjs as
                                    (
                                    select 
                                    jzlsh,
                                    convert(datetime,substring(left(ryrq,8)+' ' + substring(ryrq,9,2)+':' + substring(ryrq,11,2),1,120)) as ryrqsj, --入院时间
                                    convert(datetime,substring(left(cyrq,8)+' ' + substring(cyrq,9,2)+':' + substring(cyrq,11,2),1,120)) as cyrqsj, --出院时间
                                    isnull(ylfze,'0.00') as ylfze, --医疗费用总额
                                    isnull(tcjjzf,'0.00') as tcjjzf, --统筹基金支付
                                    isnull(gwybzjjzf,'0.00') as gwybz,--公务员补助
                                    isnull(yyfdfy,'0.00') as yyfd,	--医院负担
                                    (bcjsqzhye-zhzf) as zhye, --帐户余额
                                    (convert(decimal(8,2),isnull(z3amt1,'0.00'))+CONVERT(decimal(8,2),ISNULL(z3mzzy,'0.00'))) as z3mat1, --预缴金累计
                                    convert(decimal(8,2),isnull(case when z3amtj>0 and  left(z3stbz,1)=0 then -z3amnt when z3amtj=0 then (ylfze-xjzf) else z3amnt end,'0.00')) z3amnt, --预缴金剩余
                                    ISNULL(zhzf,'0.00') as zhzf, --个人账户支付
                                    --case when dejjzf=0 then ISNULL(xjzf+zhzf+gwybzjjzf,'0.00')
                                    --else ISNULL(dejjzf+jrdbzf+zffy,'0.00') end xjzf,	--费用负担总额（现金支付）
                                    ISNULL(qfbzfy,'0.00') as qfbzfy, --起付线
                                    case when dejjzf=0 then isnull(ylypzf+tjtzzf,'0.00')
                                    else '0.00' end as jzfd, --加重负担
                                    isnull(tcfdzffy,'0.00') as ablfd, --按比例负担
                                    ISNULL(zffy,'0.00') as zffy, --自费金额
                                    case when dejjzf=0 then '0.00'
                                    when  yldylb like '%建档立卡%' then isnull(fybzf,0.00)
                                    else '0.00' end as fybzf, --
                                    ISNULL(dejjzf,'0.00') as dejjzf, --大病基金支付
                                    case when dejjzf=0 then '0.00'
                                    when  yldylb like '%建档立卡%' then ISNULL(jrdbzf+dejjzf+xjzf-ylypzf,'0.00')
                                    else ISNULL(jrdbzf+dejjzf-ylypzf,'0.00') end as jrdbzf, --进入大病费用
                                    case when dejjzf=0 then 0.00
                                    else isnull(ylypzf+tjtzzf,'0.00') end  as dbjzfd,--加重负担
                                    ISNULL(ecbcje,'0.00') as dbecbcfy, --大病二次补偿
                                    ISNULL(zdjbfwnbcje,'0.00') as zdjbfwnbcfy, --重大疾病内补
                                    ISNULL(zdjbfwybcje,'0.00') as zdjbfwybcfy,--重大疾病外补
                                    ISNULL(czzcje,'0.00') as zftdfy--政府兜底基金
                                    from ybfyjsdr a
                                    left join  zy03dw b on  b.z3zyno=a.jzlsh and LEFT(z3endv,1)=1
                                    where jzlsh='{0}' and cxbz=1
                                    )
                                    select 
                                    ryrqsj,cyrqsj,ylfze,tcjjzf,gwybz,yyfd,zhye,z3mat1,z3amnt,zhzf,qfbzfy,
                                    ablfd,zffy,dejjzf,dbecbcfy,zdjbfwnbcfy,zdjbfwybcfy,zftdfy,
                                    case when isnull(b.jzlsh,'')!='' then b.jzfdfy else  a.jzfd end as jzfd,
                                    case when isnull(b.jzlsh,'')!='' then b.dbjzfdfy else  a.dbjzfd end as dbjzfd,
                                    case when isnull(b.jzlsh,'')!='' then b.jrdbfy else  a.jrdbzf end as jrdbzf,
                                    case when isnull(b.jzlsh,'')!='' then qfbzfy+b.jzfdfy+a.ablfd+a.zffy+b.jrdbfy+b.dbjzfdfy-fybzf else  
                                    qfbzfy+a.jzfd+a.ablfd+a.zffy+a.jrdbzf+a.dbjzfd-fybzf end as xjzf --费用负担总额（现金支付）
                                    from ybjs a
                                    left join ybjsdsz b on a.jzlsh=b.jzlsh", param);
            DataSet ds_jsinfo = CliUtils.ExecuteSql("szy01", "cmd", strSql, CliUtils.fLoginDB.ToString(), true, CliUtils.fCurrentProject.ToString());
            ds_jsinfo.Tables[0].TableName = "jsinfo";
            ds.Tables.Add(ds_jsinfo.Tables[0].Copy());
            //ds.WriteXmlSchema(@"D:\mm.xml");
            string s_path = string.Empty;
            s_path = string.IsNullOrEmpty(s_path) ? @"C:\Program Files (x86)\Infolight\EEP2012\EEPNetClient\FastReport" : s_path;//空值取默认

            string c_file = Application.StartupPath + @"\FastReport\YB\医保结算单_吉安.frx"; //client
            string s_file = s_path + @"\YB\医保结算单_吉安.frx";    //server  
            CliUtils.DownLoad(s_file, c_file);
            try
            {
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
                    report.Show();
                    report.Dispose();
                    return new object[] { 0, 1, "住院结算单打印成功" };
                }
            }
            catch (Exception ex)
            {
                return new object[] { 0, 0, ex.Message };
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
        public object[] YBYLDYFSXXCX(object[] objParam)
        {
            WriteLog("医疗待遇封锁信息未启用");
            return new object[] { 0, 1, "医疗待遇封锁信息未启用" };
        }
        #endregion

        #region 获取病种信息查询
        public static object[] YBBZCX(object[] objParam)
        {
            string yllb = objParam[0].ToString(); // 医疗类别
            string grbh = objParam[1].ToString(); //个人编号
            string jzbz = objParam[2].ToString();   //门诊住院标志 m-门诊 z-住院
            string splb = objParam[3].ToString();   //审批类别

            string[] syl_mz = { "31", "32" };
            WriteLog("  获取病种信息查询|入参|" + yllb + "|" + grbh + "|" + jzbz + "|" + splb);

            string strSql = string.Empty;
            if (jzbz.ToUpper().Equals("M"))
            {
                strSql = string.Format(@"select dm,dmmc,pym,wbm from ybbzmrdr where 1=1");
                if (syl_mz.Contains(yllb))
                    strSql += string.Format(@" and yllb in(31)");
                else
                    strSql += string.Format(@" and yllb in(11)");
            }
            else if (jzbz.ToUpper().Equals("Z"))
            {
                strSql = string.Format(@"select dm,dmmc,pym,wbm from ybbzmrdr where 1=1");
                //if (yllb.Equals("21"))
                //   strSql += string.Format(@" and yllb in(11)");
                //else
                //    strSql += string.Format(@" and ybm=1");
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

        #region 门诊登记(挂号)撤销（内部）
        public static object[] NYBMZDJCX(object[] objParam)
        {
            string sysdate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string Ywlx = "MZGHCX";
            string jzlsh = objParam[0].ToString(); //就诊流水号
            string bxh = objParam[1].ToString(); //保险号(个人编号)
            string xm = objParam[2].ToString();  //姓名
            string kh = objParam[3].ToString(); //卡号
            string dqbh = objParam[4].ToString(); //地区编号
            string ybjzlsh = objParam[5].ToString();//医保就诊流水号
            
            //保险号|姓名|卡号|地区编号|门诊号
            StringBuilder inputData = new StringBuilder();
            inputData.Append(bxh + "|");
            inputData.Append(xm + "|");
            inputData.Append(kh + "|");
            inputData.Append(dqbh + "|");
            inputData.Append(ybjzlsh);
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
            string Ywlx = "MZSFCX";
            string jzlsh = objParam[0].ToString();  //就诊流水号
            string jslsh = objParam[1].ToString();    //单据流水号
            string bxh = string.Empty;
            string xm = string.Empty;
            string kh = string.Empty;
            string dqbh = string.Empty;
            string ybjzlsh = "";
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

            StringBuilder inputData = new StringBuilder();
            //入参:保险号|姓名|卡号|地区编号|门诊号|单据流水号
            inputData.Append(bxh + "|");
            inputData.Append(xm + "|");
            inputData.Append(kh + "|");
            inputData.Append(dqbh + "|");
            inputData.Append(ybjzlsh + "|");
            inputData.Append(jslsh);
            StringBuilder outData = new StringBuilder(1024);
            StringBuilder retMsg = new StringBuilder(1024);

            WriteLog(sysdate + "  " + jzlsh + " 进入门诊收费撤销(内部)...");
            WriteLog(sysdate + "  入参|" + inputData.ToString());
            int i = f_UserBargaingApply(Ywlx, inputData, outData, retMsg);
            if (i > 0)
            {
                WriteLog(sysdate + "  " + jzlsh + " 进入门诊收费撤销(内部)成功|" + retMsg.ToString());
                return new object[] { 0, 1, "门诊收费撤销成功|" + retMsg.ToString() };
            }
            else
            {
                WriteLog(sysdate + "  " + jzlsh + " 进入门诊收费撤销(内部)失败|" + retMsg.ToString());
                return new object[] { 0, 0, "门诊收费撤销失败|" + retMsg.ToString() };
            }
        }
        #endregion

        #region 住院登记撤销(内部)
        public static object[] NYBZYDJCX(object[] objParam)
        {
            string sysdate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string Ywlx = "ZYDJCX";
            string jzlsh = objParam[0].ToString(); //就诊流水号
            string bxh = objParam[1].ToString(); //保险号(个人编号)
            string xm = objParam[2].ToString();  //姓名
            string kh = objParam[3].ToString(); //卡号
            string dqbh = objParam[4].ToString(); //地区编号
            string ybjzlsh = objParam[5].ToString();//医保就诊流水号

            //保险号|姓名|卡号|地区编号|门诊号
            StringBuilder inputData = new StringBuilder();
            inputData.Append(bxh + "|");
            inputData.Append(xm + "|");
            inputData.Append(kh + "|");
            inputData.Append(dqbh + "|");
            inputData.Append(ybjzlsh);
            StringBuilder outData = new StringBuilder(1024);
            StringBuilder retMsg = new StringBuilder(1024);

            WriteLog(sysdate + "  " + jzlsh + " 进入住院医保登记撤销(内部)...");
            WriteLog(sysdate + "  入参|" + inputData.ToString());
            int i = f_UserBargaingApply(Ywlx, inputData, outData, retMsg);
            if (i > 0)
            {
                WriteLog(sysdate + "  " + jzlsh + " 进入住院医保登记撤销(内部)成功|" + outData.ToString());
                return new object[] { 0, 1, "住院医保登记撤销(内部)成功", outData.ToString() };
            }
            else
            {
                WriteLog(sysdate + "  " + jzlsh + " 进入住院医保登记撤销(内部)失败|" + outData.ToString());
                return new object[] { 0, 0, "住院医保登记撤销(内部)失败", outData.ToString() };
            }
        }
        #endregion

        #region 住院收费登记撤销(全部)(内部）
        public static object[] NYBZYSFDJCX(object[] objParam)
        {
            string sysdate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string Ywlx = "ZYSFQBTF";
            string jzlsh = objParam[0].ToString();
            string bxh = objParam[1].ToString();
            string xm = objParam[2].ToString();
            string kh = objParam[3].ToString();
            string ybjzlsh = objParam[4].ToString();
             StringBuilder inputData = new StringBuilder();
            //入参:保险号|姓名|卡号|地区编号|住院号
            inputData.Append(bxh + "|");
            inputData.Append(xm + "|");
            inputData.Append(kh + "|");
            inputData.Append(dqbh + "|");
            inputData.Append(ybjzlsh + ";");
            StringBuilder outData = new StringBuilder(1024);
            StringBuilder retMsg = new StringBuilder(1024);
            WriteLog(sysdate + "  " + jzlsh + " 进入住院收费退费(内部)...");
            int i = f_UserBargaingApply(Ywlx, inputData, outData, retMsg);
            if (i > 0)
            {
                WriteLog(sysdate + "  " + jzlsh + " 进入住院收费退费(内部)成功|" + outData.ToString());
                return new object[] { 0, 1, "住院收费退费成功", outData.ToString() };
            }
            else
            {
                WriteLog(sysdate + "  " + jzlsh + " 进入住院收费退费(内部)失败|" + outData.ToString());
                return new object[] { 0, 0, "住院收费退费失败", outData.ToString() };
            }
        }
        #endregion

        #region 住院收费结算撤销(内部)
        public static object[] NYBZYSFJSCX(object[] objParam)
        {
            string sysdate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string Ywlx = "ZYSFCX";
            string jzlsh = objParam[0].ToString(); //就诊流水号
            string bxh = objParam[1].ToString();    //保险号
            string xm = objParam[2].ToString();     //姓名
            string kh = objParam[3].ToString();     //卡号
            string dqbh = objParam[4].ToString();   //地区编号
            string ybjzlsh = objParam[5].ToString();//医保就诊流水号
            string djlsh = objParam[6].ToString();  //单据流水号
            string sfmxsfzf = "FALSE";


            StringBuilder inputData = new StringBuilder();
            //入参:保险号|姓名|卡号|地区编号|住院号|单据流水号|收费明细是否作废
            inputData.Append(bxh + "|");
            inputData.Append(xm + "|");
            inputData.Append(kh + "|");
            inputData.Append(dqbh + "|");
            inputData.Append(ybjzlsh + "|");
            inputData.Append(djlsh + "|");
            inputData.Append(sfmxsfzf);
            StringBuilder outData = new StringBuilder(1024);
            StringBuilder retMsg = new StringBuilder(1024);
            WriteLog(sysdate + "  " + jzlsh + " 进入住院收费结算撤销...");
            WriteLog(sysdate + " 入参|" + inputData.ToString());
            int i = f_UserBargaingApply(Ywlx, inputData, outData, retMsg);
            if (i > 0)
            {
                WriteLog(sysdate + "  " + jzlsh + " 进入住院收费结算撤销(内部)成功" + retMsg.ToString());
                return new object[] { 0, 1, "住院收费撤销(内部)成功|" + retMsg.ToString() };
            }
            else
            {
                WriteLog(sysdate + "  " + jzlsh + " 进入住院收费结算撤销(内部)失败" + retMsg.ToString());
                return new object[] { 0, 0, "住院收费撤销(内部)失败|" + retMsg.ToString() };
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
