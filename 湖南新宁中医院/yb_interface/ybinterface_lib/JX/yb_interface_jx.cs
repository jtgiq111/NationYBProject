using FastReport;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Srvtools;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using ybinterface_lib.SY;
using ybinterface_lib.SY.Card;
using ybinterface_lib.SY.InPatient;
using ybinterface_lib.SY.InPatient.Fee;
using ybinterface_lib.SY.OutPatient;
using ybinterface_lib.SY.OutPatient.Fee;
using ybinterface_lib;
using ybinterface_lib.SY.YB;
using ybinterface_lib.SY.UI;
using ybinterface_lib.SY.OutParam;
using ybinterface_lib.SY.Register;
using ybinterface_lib.SY.DZ;
using ybinterface_lib.JX.Register;
using ybinterface_lib.JX.OutPatient.Fee;
using OutpatientFee = ybinterface_lib.JX.OutPatient.Fee.OutpatientFee;

namespace ybinterface_lib
{
    public class yb_interface_jx
    {



        #region 老医保接口DLL加载

        [DllImport("ZRHosJK.dll", CharSet = CharSet.Ansi)]
        public static extern int f_UserBargaingInit(string UserID, string PassWD, StringBuilder retMsg);

        [DllImport("ZRHosJK.dll", CharSet = CharSet.Ansi)]
        public static extern int f_UserBargaingClose(StringBuilder retMsg);

        [DllImport("ZRHosJK.dll", CallingConvention = CallingConvention.StdCall)]
        private static extern int f_UserBargaingApply(string Ywlx, StringBuilder InData, StringBuilder OutData, StringBuilder retMsg);

        #endregion

        #region 变量
        public static string dqbh = "360782"; //南康

        public static string frmbzbm = "";
        public static string frmbzmc = "";
        private static string dzkh = string.Empty;
        // static IWork iWork = new Work();
        static string xmlPath = AppDomain.CurrentDomain.BaseDirectory;
        //static List<Item1> lItem = iWork.getXmlConfig1(xmlPath + "EEPNetClient.exe.config");
        static List<ybinterface_lib.JX.Config.Item> lItem = FunctionsJX.getXmlConfig(xmlPath + "EEPNetClient.exe.config");
        static string YBIP = lItem[0].YBIP;     //医保IP地址
        static string YBJGBH = lItem[0].YLJGBH;    //医疗机构编码
        static string YBJGMC = lItem[0].DDYLJGMC;//医疗机构名称
        static string YBJYBH = "";//签到返回的交易编号
        static Dictionary<string, string> dicOperYbjybm = new Dictionary<string, string>();//操作员Id 对应  交易编码
        static string Url = $"http://{lItem[0].YBIP}:{lItem[0].YBPORT}" + "/fsi/api/rsfComIfsService/callService";//服务地址
        static string YBjyqh = lItem[0].YBJYYBQH;
        string testId = "";
        /// <summary>
        /// 版本号
        /// </summary>
        private static string _api_version = lItem[0]._api_version;
        /// <summary>
        /// 时间戳
        /// </summary>
        private static string _api_timestamp = lItem[0]._api_timestamp;
        /// <summary>
        /// 秘钥
        /// </summary>
        private static string _api_access_key = lItem[0]._api_access_key;
        /// <summary>
        /// 签名
        /// </summary>
        private static string _api_signature = lItem[0]._api_signature;
        /// <summary>
        /// 签名算法
        /// </summary>
        private static string signcentKey = "HmacSHA1";
        #endregion

        #region 新医保接口


        #region 新医保接口调用请求的方式
        public static string GetYBPostRequest(string url, string ywCode, string data, ref int status)
        {
            try
            {
                WebRequest request = WebRequest.Create(url);
                request.Method = "POST";
                request.ContentType = "application/json;charset=utf-8";
                request.Headers.Add("_api_name", ywCode);
                request.Headers.Add("_api_version", _api_version);
                request.Headers.Add("_api_timestamp", _api_timestamp);
                request.Headers.Add("_api_access_key", _api_access_key);
                request.Headers.Add("_api_signature", getSign(ywCode));
                byte[] databyte = Encoding.UTF8.GetBytes(data);
                request.ContentLength = databyte.Length;
                Stream stream = request.GetRequestStream();
                stream.Write(databyte, 0, databyte.Length);
                stream.Close();
                WebResponse res = request.GetResponse();
                stream = res.GetResponseStream();
                StreamReader reader = new StreamReader(stream);
                status = 1;
                return reader.ReadToEnd();
            }
            catch (Exception ex)
            {
                status = -1;
                return ex.Message;
            }
        }
        #endregion


        #region api调用签名方法
        /// <summary>
        /// api调用签名方法
        /// </summary>
        /// <param name="YwCode">接口业务id</param>
        /// <returns></returns>
        private static string getSign(string YwCode)
        {
            string newStr = $"_api_access_key={_api_access_key}&_api_name={YwCode}&_api_timestamp={_api_timestamp}&&_api_version={_api_version}";
            HMACSHA1 hmacsha1 = new HMACSHA1();

            hmacsha1.Key = System.Text.Encoding.UTF8.GetBytes(signcentKey);

            byte[] dataBuffer = System.Text.Encoding.UTF8.GetBytes(signcentKey);

            byte[] hashBytes = hmacsha1.ComputeHash(dataBuffer);
            string newsignCode = Convert.ToBase64String(hashBytes);

            return newsignCode;
        }
        #endregion

        #region 使用递归获取序列
        private static List<int> nums = new List<int>();
        private static int num = 0;
        private static int getSeq()
        {
            num += 1;
            if (nums.Contains(num))
            {
                num = getSeq();
            }
            nums.Add(num);
            return num;
        }
        #endregion

        #region 医保服务统一访问方法
        /// <summary>
        /// 医保服务统一访问方法
        /// </summary>
        /// <param name="YwCode">业务编码</param>
        /// <param name="data">入参</param>
        /// <param name="Result">接收结果返回</param>
        /// <returns></returns>
        public static int YBServiceRequest(string YwCode, string data, ref string Result)
        {
            string EmplId = CliUtils.fLoginUser;
            string EmplName = CliUtils.fUserName;
            ybinterface_lib.JX.RequestModel req = new ybinterface_lib.JX.RequestModel()
            {
                infno = YwCode,
                msgid = YBJGBH + System.DateTime.Now.ToString("yyyyMMddHHmmss") + getSeq(),
                mdtrtarea_admvs = YBjyqh,
                insuplc_admdvs = "",
                recer_sys_code = "his",
                dev_no = "",
                dev_safe_info = "",
                cainfo = "",
                signtype = "",
                infver = "V1.0",
                opter_type = 1,
                opter = EmplId,
                opter_name = EmplName,
                inf_time = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                fixmedins_code = YBJGBH,
                fixmedins_name = YBJGMC,
                sign_no = YbSignNo,
                input = data
            };
            //将参数反序列化成json
            string RequestData = JsonConvert.SerializeObject(req);
            int requestStatus = 0;
            //医保接口发送请求
            string res = GetYBPostRequest(Url, YwCode, RequestData, ref requestStatus);
            ResponseModel model = JsonConvert.DeserializeObject<ResponseModel>(res);
            if (model.infcode == "0")
            {
                Result = model.output;
            }
            else
            {
                Result = model.err_msg;
                return -1;
            }
            return 1;
        }
        #endregion
        #endregion


        #region 判断日期
        public static bool IsDate(string strDate)
        {
            try
            {
                DateTime.Parse(strDate);
                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion


        #region 接口业务调用方法
        #region 获取Mac地址
        public static string getMac()
        {
            string macAddresses = "";

            foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (nic.OperationalStatus == OperationalStatus.Up)
                {
                    macAddresses += BitConverter.ToString(nic.GetPhysicalAddress().GetAddressBytes()).Replace("-", ":");
                    break;
                }
            }
            return macAddresses;
        }

        #endregion

        #region 根据工号获取医生名称
        private static string GetEmplNameById(string EmplId)
        {
            string strSql = string.Format(@"select h.b1name from bz01h h where h.b1empn='{0}' ", EmplId);
            DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            if (ds.Tables[0].Rows.Count <= 0)
            {
                return "";
            }
            else
            {
                return ds.Tables[0].Rows[0][0].ToString();
            }
        }
        #endregion

        #region 根据科室编号获取科室名称
        private static string GetDeptNameById(string DeptId)
        {
            string strSql = string.Format(@"select b.b2ksnm from bz02h b where b.b2ksno='{0}'", DeptId);
            DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            if (ds.Tables[0].Rows.Count <= 0)
            {
                return "";
            }
            else
            {
                return ds.Tables[0].Rows[0][0].ToString();
            }
        }
        #endregion

        #region 根据his项目编码获取医保项目中心编码
        private static string GetYbxmbm(string hisxmbm)
        {
            string StrSql = $"select b.ybxmbh from ybhisdzdr b where b.hisxmbh='{hisxmbm}'";
            DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", StrSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            if (ds.Tables[0].Rows.Count <= 0)
            {
                return "";
            }
            else
            {
                return ds.Tables[0].Rows[0][0].ToString();
            }
        }
        #endregion

        #region 获取本地ip
        public static string getIP()
        {
            string strHostName = Dns.GetHostName(); //得到本机的主机名
            IPHostEntry ipEntry = Dns.GetHostByName(strHostName); //取得本机IP
            string ip = ipEntry.AddressList[1].ToString();
            return ip;
        }
        #endregion

        #region 初始化（新医保接口）
        static string YbSignNo = string.Empty;
        public static object[] YBINIT(object[] objParam)
        {
            //Ping医保网

            Ping ping = new Ping();
            PingReply pingReply = ping.Send(YBIP);

            if (pingReply.Status != IPStatus.Success)
                return new object[] { 0, 0, "未连接医保网" };

            string sysdate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string hisUserID = CliUtils.fLoginUser;
            string UserID = CliUtils.fLoginUser; //用户名
            string PassWD = string.Empty; //密码

            #region 新医保接口的调用方式 by   hjw

            string data = "{\"opter_no\":\"" + UserID + "\",\"mac\":\"" + getMac() + "\",\"ip\":\"" + getIP() + "\"}";
            // data = string.Format(data, UserID, getMac(), getIP());
            string Result = string.Empty;
            WriteLog(sysdate + "|医保初始化入参|" + data);
            int i = YBServiceRequest("9001", data, ref Result);

            #endregion
            WriteLog(sysdate + "  入参|用户" + UserID + "");
            if (i > 0)
            {
                JObject jobj = JsonConvert.DeserializeObject<JObject>(Result);
                string signCode = jobj["sign_no"].ToString();
                YbSignNo = signCode;
                WriteLog(sysdate + "  用户" + UserID + " 进入医保初始化成功");
                return new object[] { 0, 1, "医保初始化成功" };
            }
            else
            {
                WriteLog(sysdate + "  医保初始化失败|" + i + "|" + Result.ToString());
                return new object[] { 0, 0, "医保初始化失败;" + Result.ToString() };
            }
        }
        #endregion

        #region 医保退出（新医保接口）
        public static object[] YBEXIT(object[] objParam)
        {
            string sysdate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            #region 老医保接口
            //StringBuilder retMsg = new StringBuilder(1024);
            //int i = f_UserBargaingClose(retMsg); 
            #endregion

            string operNo = objParam[0].ToString();
            string mac = getMac();
            string ip = getIP();
            #region 新医保接口的调用方式 by   hjw

            string data = "{\"opter_no\":\"" + operNo + "\",\"mac\":\"" + mac + "\",\"ip\":\"" + ip + "\"}";
            //data = string.Format(data, objParam[0].ToString(), getMac(), getIP());
            string Result = string.Empty;
            int i = YBServiceRequest("9002", data, ref Result);
            WriteLog(sysdate + "|医保退出入参|" + data);
            #endregion
            if (i > 0)
            {
                WriteLog(sysdate + "  医保退出成功");
                return new object[] { 0, 1, "医保退出成功" };
            }
            else
            {
                WriteLog(sysdate + "  医保退出错误|" + Result.ToString());
                return new object[] { 0, 0, "医保退出错误" };
            }
        }
        #endregion

        #region 门诊读卡（读卡无变化 无需更改）
        public static object[] YBMZDK(object[] objParam)
        {
            string sysdate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            try
            {
                string Ywlx = "MZGHSK";//老   string Ywlx="1101"; //新
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
                    ParamYBDK dk = new ParamYBDK();
                    dk.Grbh = str[0].Trim();     //保险号
                    dk.Xm = str[1].Trim();      //姓名
                    dk.Kh = str[2].Trim();      //卡号
                    dzkh = dk.Kh;
                    dk.Tcqh = str[3].Trim();    //地区编号
                    dk.Qxmc = str[4].Trim();    //地区名称
                    dk.Csrq = str[5].Trim();    //出生日期
                    // dk.Nl = str[6].Trim();    //实际年龄
                    dk.Nl = string.IsNullOrEmpty(str[6]) || str[6] == null || str[6] == "NULL" ? GetAgeByBirthdate(DateTime.Parse(dk.Csrq.Substring(0, 4) + "-" + dk.Csrq.Substring(4, 2) + "-" + dk.Csrq.Substring(6, 2))).ToString() : str[6].ToString(); //年龄
                    dk.Cbrq = str[7].Trim();    //参保日期
                    dk.Grsf = str[8].Trim();    //个人身份
                    dk.Dwmc = str[9].Trim();    //单位名称
                    dk.Xb = str[10].Trim();     //性别
                    dk.Ylrylb = str[11].Trim();   //医疗人员类别
                    dk.Zkt = str[12].Trim();    //卡状态
                    //dk.Zhye = str[13].Trim(); //账户余额
                    dk.Zhye = string.IsNullOrEmpty(str[13]) || str[13] == null || str[13] == "NULL" ? "0" : str[13].Trim();//账户余额
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

                    List<string> liDqbh = new List<string>();//赣州所有县地区编号
                    #region 赣州地区编号
                    //360701	【赣州市】
                    //360721	【赣县】
                    //360733	【会昌县】
                    //360723	【大余县】
                    //360734	【寻乌县】
                    //360782	【南康市】
                    //360730	【宁都县】
                    //360725	【崇义县】
                    //360726	【安远县】
                    //360732	【兴国县】
                    //360728	【定南县】
                    //360724	【上犹县】
                    //360735	【石城县】
                    //360722	【信丰县】
                    //360729	【全南县】
                    //360727	【龙南县】
                    //360781	【瑞金市】
                    //360731	【于都县】
                    //360702	【章贡区】
                    //360703    【开发区】
                    //360704    【蓉江新区】
                    //返回其他值 【异地就医】
                    liDqbh.Add("360701");
                    liDqbh.Add("360721");
                    liDqbh.Add("360733");
                    liDqbh.Add("360723");
                    liDqbh.Add("360734");
                    liDqbh.Add("360782");
                    liDqbh.Add("360730");
                    liDqbh.Add("360725");
                    liDqbh.Add("360726");
                    liDqbh.Add("360732");
                    liDqbh.Add("360728");
                    liDqbh.Add("360724");
                    liDqbh.Add("360735");
                    liDqbh.Add("360722");
                    liDqbh.Add("360729");
                    liDqbh.Add("360727");
                    liDqbh.Add("360781");
                    liDqbh.Add("360731");
                    liDqbh.Add("360702");
                    liDqbh.Add("360703");
                    liDqbh.Add("360704");
                    #endregion

                    if (!liDqbh.Contains(dk.Tcqh)) //异地
                    {
                        dk.Jflx = "09";
                    }
                    else
                    {
                        if (dk.Ylrylb.Contains("扶贫"))
                            dk.Jflx = "04";
                        else if (dk.Ylrylb.Contains("居民"))
                            dk.Jflx = "02";
                        else
                            dk.Jflx = "01";
                    }

                    // MessageBox.Show(dk.Jflx);
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


                        #region 判断患者是不是慢性病  因赣州医保只有登记后才能获取 210310 改
                        #endregion
                        //string jzlsh = objParam[0].ToString(); //就诊流水号
                        //string yllb = objParam[1].ToString(); //医疗类别代码
                        //string bzbm = objParam[2].ToString(); //病种编码
                        //string bzmc = objParam[3].ToString(); //病种名称
                        //string[] kxx = objParam[4].ToString().Split('|'); //读卡信息
                        //string ghdjsj = objParam[5].ToString(); //挂号日期时间(yyyy-MM-dd HH:mm:ss)
                        //string cfysdm = objParam[6].ToString(); //处方医生代码
                        //string cfysxm = objParam[7].ToString(); //处方医生姓名
                        /////
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

        #region 获取人员信息（新）（新医保接口）
        private static ReadCardInfo GetYbCardInfo(object[] Inputparams)
        {
            ReadCardInfo cardInfo = new ReadCardInfo();
            string ywCode = "1101";
            string result = string.Empty;
            string data = "{\"mdtrt_cert_type\":\"" + Inputparams[0].ToString() + "\",\"mdtrt_cert_no\":\"" + Inputparams[1].ToString() + "\",\"card_sn\":\"" + Inputparams[2].ToString() + "\",\"begntime\":\"" + Inputparams[3].ToString() + "\",\"psn_cert_type\":\"" + Inputparams[4].ToString() + "\",\"certno\":\"" + Inputparams[5].ToString() + "\",\"psn_name\":\"" + Inputparams[6].ToString() + "\"}";//psn_cert_type 1是居民身份证
            data = string.Format(data, Inputparams);
            int i = YBServiceRequest(ywCode, data, ref result);
            if (i > 0)
            {
                cardInfo = JsonConvert.DeserializeObject<ReadCardInfo>(result);
            }
            else
            {
                return null;
            }


            return cardInfo;
        }
        #endregion

        #region 门诊登记(挂号) 多病种调用（新医保接口）
        public static object[] YBMZDJBZ(object[] objParam)
        {
            string sysdate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            try
            {
                string Ywlx = "2201";
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
                string strbz = string.Format("select '' as bzbm,'' as bzmc ");
                DataSet dsbz = CliUtils.ExecuteSql("sybdj", "cmd", strbz, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                dsbz.Tables[0].Rows.RemoveAt(0);
                // ds.Tables[0].Rows.Add(object[] {'1','1'});
                //   MessageBox.Show(bzbm + "_" + bzmc);



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
                #region 医保获取个人信息
                ReadCardInfo cardInfo = GetYbCardInfo(new object[] { "02", sfzh, "", "", "1", sfzh, xm });

                #endregion
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

                string jzlsh1 = "";
                if (string.IsNullOrEmpty(bzbm))
                    jzlsh1 = "MZ" + yllb + jzlsh;
                else
                    jzlsh1 = "MZ" + yllb + jzlsh + bzbm;

                #region 判断是否在HIS中挂号
                string strSql = string.Empty;
                strSql = string.Format(@"
select
                (case when isnull(b.m1name,'')='' then a.m1name else b.m1name  end)   as name,
                (case when isnull(b.m1ksno,'')='' then a.m1ksno else b.m1ksno  end)   as ksno,
                (select top 1 b2ejnm from bz02d  where b2ksno=(case when isnull(b.m1ksno,'')='' then a.m1ksno else b.m1ksno  end)) as ksmc from mz01t  a
                left join  mz01h b on a.m1mzno=b.m1mzno and a.m1ghno=b.m1ghno                 
                where a.m1ghno = '{0}'
				union all 
				
select
                (case when isnull(b.m1name,'')='' then a.m1name else b.m1name  end)   as name,
                (case when isnull(b.m1ksno,'')='' then a.m1ksno else b.m1ksno  end)   as ksno,
                (select top 1 b2ejnm from bz02d  where b2ksno=(case when isnull(b.m1ksno,'')='' then a.m1ksno else b.m1ksno  end)) as ksmc from mz01h  b
                left join  mz01t a on a.m1mzno=b.m1mzno and a.m1ghno=b.m1ghno                 
                where b.m1ghno = '{0}'", jzlsh);
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
                    dgysdm = "010091";
                    dgysxm = "江辉";
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


                #region 老医保接口
                //    //入参: 保险号|姓名|卡号|地区编号|医疗类别|科室名称|挂号费|挂号日期|挂号时间|医院门诊流水号
                // StringBuilder inputData = new StringBuilder();
                // inputData.Append(grbh + "|");    //保险号
                // inputData.Append(xm + "|");     //姓名
                // inputData.Append(kh + "|");     //卡号
                // inputData.Append(tcqh + "|");   //地区编号
                // inputData.Append(yllb + "|");   //医疗类别
                // inputData.Append(ksmc + "|");   //科室名称
                // inputData.Append(ghfy + "|");   //挂号费
                // inputData.Append(ghrq + "|");  //挂号日期
                // inputData.Append(ghsj + "|");   //挂号时间
                // inputData.Append(jzlsh);        //医院门诊流水号
                //StringBuilder outData = new StringBuilder(1024);
                //StringBuilder retMsg = new StringBuilder(1024);

                //  int i = f_UserBargaingApply(Ywlx, inputData, outData, retMsg);

                #endregion

                #region 新医保接口 by hjw
                #region 入参
                string beginTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                string data = "{\"psn_no\":\"" + grbh + "\",\"insutype\":\"" + cardInfo.insuinfo.insutype + "\",\"begntime\":\"" + beginTime + "\",\"mdtrt_cert_type\":\"03\",\"mdtrt_cert_no\":\"" + kh + "\",\"card_sn\":\"\",\"psn_cert_type\":\"01\",\"certno\":\"" + sfzh + "\",\"psn_type\":\"" + cardInfo.insuinfo.psn_type + "\",\"psn_name\":\"" + xm + "\",\"ipt_otp_no\":\"" + jzlsh + "\",\"atddr_no\":\"" + cfysdm + "\",\"dr_name\":\"" + cfysxm + "\",\"dept_code\":\"" + ksbh + "\",\"dept_name\":\"" + ksmc + "\",\"caty\":\"\"}";
                //data = string.Format(data, grbh, cardInfo.insuinfo.insutype, System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "03", kh, jzlsh, cfysdm, cfysxm, ksbh, ksmc, "");//科别未填 
                WriteLog(sysdate + "  " + jzlsh + " 进入门诊挂号...");
                WriteLog(sysdate + "  入参|" + data.ToString());
                #endregion
                string res = string.Empty;
                int i = YBServiceRequest(Ywlx, data, ref res);
                JObject jres = JsonConvert.DeserializeObject<JObject>(res);

                #region 回参
                string mdtrt_id = jres["mdtrt_id"].ToString();//就诊ID
                string psn_no = jres["psn_no"].ToString();//人员编号
                string ipt_otp_no = jres["ipt_otp_no"].ToString();//就诊流水号
                #endregion
                #endregion


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
                    //string[] str = outData.ToString().Split(';');
                    //string[] str2 = str[0].Split('|');
                    string bxh2 = grbh.Trim();       //保险号
                    string xm2 = xm.Trim();        //姓名
                    string kh2 = kh.Trim();        //卡号
                    string dqbh1 = tcqh.Trim();      //地区编号
                    string dqmc = cardInfo.insuinfo.insuplc_admdvs.Trim();       //地区名称
                    string csrq1 = csrq.Trim();       //出生日期
                    string sjnl = nl.Trim();       //实际年龄
                    string cbrq = cardInfo.idetinfo.begntime.Trim();       //参保日期
                    string grsf = cardInfo.idetinfo.psn_type_lv.Trim();       //个人身份
                    string dwmc1 = cardInfo.insuinfo.emp_name.Trim();       //单位名称
                    string xb1 = cardInfo.baseinfo.gend;        //性别
                    string rylb = cardInfo.insuinfo.psn_type.Trim();      //医疗人员类别
                    //string kzt = str2[12].Trim();       //卡状态
                    string grzhye = cardInfo.insuinfo.balc.Trim();    //账户余额
                    string jzlsh2 = jzlsh.Trim();    //门诊(住院号)
                    string yllb2 = yllb.Trim();     //医疗类别
                    string ksmc2 = ksmc.Trim();     //科室名称
                                                    // string ghf = str2[17].Trim();       //挂号费
                                                    // string bckbcs = str2[18].Trim();    //本次看病次数
                                                    //string zych = str2[19].Trim();      //住院床号
                                                    //   string djrq = str2[20].Trim();      //入院日期
                                                    //      string djsj = str2[21].Trim();      //入院时间
                    string jbr = GetEmplNameById(CliUtils.fLoginUser).Trim();       //经办人
                    //if (string.IsNullOrEmpty(ghf)) //挂号费为NULL时，传入0
                    //    ghf = "0.00";
                    #endregion

                    string bzbm_r = string.Empty;
                    string bzmc_r = string.Empty;
                    string bzbm_r2 = string.Empty;
                    string bzmc_r2 = string.Empty;
                    string bzbm_r3 = string.Empty;
                    string bzmc_r3 = string.Empty;
                    string bzbm_r4 = string.Empty;
                    string bzmc_r4 = string.Empty;
                    #region 没有找到慢病的信息
                    //if (str.Length > 1)
                    //{

                    //    for (int j = 1; j < str.Length; j++)
                    //    {

                    //        str2 = str[j].Split('|');
                    //        if (j == 1)
                    //        {
                    //            bzbm_r = str2[0].Trim();
                    //            bzmc_r = str2[1].Trim();
                    //            DataRow dr = dsbz.Tables[0].NewRow();
                    //            dr[0] = str2[0].Trim();
                    //            dr[1] = str2[1].Trim();
                    //            dsbz.Tables[0].Rows.Add(dr);
                    //        }
                    //        else if (j == 2)
                    //        {
                    //            bzbm_r2 = str2[0].Trim();
                    //            bzmc_r2 = str2[1].Trim();
                    //            DataRow dr = dsbz.Tables[0].NewRow();
                    //            dr[0] = str2[0].Trim();
                    //            dr[1] = str2[1].Trim();
                    //            dsbz.Tables[0].Rows.Add(dr);
                    //        }
                    //        else if (j == 3)
                    //        {
                    //            bzbm_r3 = str2[0].Trim();
                    //            bzmc_r3 = str2[1].Trim();
                    //            DataRow dr = dsbz.Tables[0].NewRow();
                    //            dr[0] = str2[0].Trim();
                    //            dr[1] = str2[1].Trim();
                    //            dsbz.Tables[0].Rows.Add(dr);
                    //        }
                    //        else if (j == 4)
                    //        {
                    //            bzbm_r4 = str2[0].Trim();
                    //            bzmc_r4 = str2[1].Trim();
                    //            DataRow dr = dsbz.Tables[0].NewRow();
                    //            dr[0] = str2[0].Trim();
                    //            dr[1] = str2[1].Trim();
                    //            dsbz.Tables[0].Rows.Add(dr);
                    //        }
                    //        //bzbm = bzbm_r;
                    //        //bzmc = bzmc_r;
                    //        strValue += "病种编码:【" + bzbm_r + "】 病种名称:【" + bzmc_r + "】\r\n";
                    //        strSql = string.Format(@"insert into ybmxbdj (jzlsh,bxh,xm,kh,mmbzbm,mmbzmc) values(
                    //                            '{0}','{1}','{2}','{3}','{4}','{5}')",
                    //                                jzlsh, bxh2, xm2, kh2, str2[0].Trim(), str2[1].Trim());
                    //        liSQL.Add(strSql);
                    //    }
                    //    //if (str.Length > 2)
                    //    //{
                    //    //    MessageBox.Show("该患者有多病种,病种为：\r\n" + strValue);
                    //    //}
                    //} 
                    #endregion

                    strSql = string.Format(@"insert into ybmzzydjdr(grbh,xm,kh,tcqh,bq,csrq,nl,nd,sfzh,dwmc,
                                        xb,yldylb,rycbzt,zhye,ybjzlsh,yllb,ksmc,ghf,zycs,bnzycs,
                                        ghdjsj,jbr,dgysdm,dgysxm,ksbh,jzbz,sysdate,jzlsh,bzbm,bzmc,
                                        jylsh,mmbzbm1,mmbzmc1,ysdm,ysxm,jzlsh1,mmbzbm2,mmbzmc2,mmbzbm3,mmbzmc3,mmbzbm4,mmbzmc4) values(
                                       '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}',
                                       '{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}',
                                       '{20}','{21}','{22}','{23}','{24}','{25}','{26}','{27}','{28}','{29}',
                                       '{30}','{31}','{32}','{33}','{34}','{35}','{36}','{37}','{38}','{39}','{40}','{41}')",
                                        bxh2, xm2, kh2, tcqh, dqmc, csrq, sjnl, "", sfzh, dwmc1,
                                        xb1, yldylb, rycbzt, grzhye, jzlsh2, yllb, ksmc2, "", "", "",
                                        ghdjsj, jbr, dgysdm, dgysxm, ksbh, "m", sysdate, jzlsh, bzbm, bzmc,
                                        jzlsh2, bzbm_r, bzmc_r, cfysdm, cfysxm, jzlsh1, bzbm_r2, bzmc_r2, bzbm_r3, bzmc_r3, bzbm_r4, bzmc_r4);
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
                                WriteLog(sysdate + "  " + jzlsh + " 进入门诊挂号成功|" + res.ToString());
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
                            //object[] objParam2 = new object[] { jzlsh, bxh2, xm2, kh2, dqbh1, jzlsh2 };
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
                            // bldjdbz = true;
                            WriteLog(sysdate + "  " + jzlsh + " 进入门诊挂号成功|" + res.ToString());
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
                    WriteLog(sysdate + "  " + jzlsh + " 进入门诊挂号失败|" + res.ToString());
                    return new object[] { 0, 0, "门诊挂号失败|" + res.ToString() };
                }
            }
            catch (Exception ex)
            {
                WriteLog(sysdate + "  门诊读卡异常|" + ex.Message);
                return new object[] { 0, 0, "门诊读卡异常|" + ex.Message };
            }
        }
        #endregion

        #region 门诊登记(挂号)撤销多病种调用（新医保接口）

        public static object[] YBMZDJCXBZ(object[] objParam)
        {
            string sysdate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            try
            {
                string Ywlx = "2202";
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

                string strSql = string.Format(@"select a.* from ybmzzydjdr a where a.jzlsh='{0}' and a.cxbz=1  and isnull(bzbm,'')=''
                                                and not exists(select 1 from ybfyjsdr b where a.ybjzlsh=b.ybjzlsh and b.cxbz=1) ", jzlsh);
                DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                if (ds.Tables[0].Rows.Count == 0)
                    return new object[] { 0, 0, "该患者未办理挂号登记或已收费不能撤销登记信息" };

                List<string> liSQL = new List<string>();
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    bxh = dr["grbh"].ToString();
                    xm = dr["xm"].ToString();
                    kh = dr["kh"].ToString();//卡号
                    dqbh = dr["tcqh"].ToString();
                    ybjzlsh = dr["ybjzlsh"].ToString();//医保就诊流水号
                    jbr = dr["jbr"].ToString();
                    yllb = dr["yllb"].ToString();

                    //strSql = string.Format(@"select * from ybfyjsdr where ybjzlsh='{0}' and cxbz=1", ybjzlsh);
                    //ds.Tables.Clear();
                    //ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                    //if (ds.Tables[0].Rows.Count > 0)
                    //    return new object[] { 0, 0, "该患者已进行医保收费，不能撤销医保登记！" };
                    //保险号|姓名|卡号|地区编号|门诊号
                    #region 老医保接口
                    //StringBuilder inputData = new StringBuilder();
                    //inputData.Append(bxh + "|");
                    //inputData.Append(xm + "|");
                    //inputData.Append(kh + "|");
                    //inputData.Append(dqbh + "|");
                    //inputData.Append(ybjzlsh);
                    //StringBuilder outData = new StringBuilder(1024);
                    //StringBuilder retMsg = new StringBuilder(1024);

                    //WriteLog(sysdate + "  " + jzlsh + " 进入门诊挂号撤销...");
                    //WriteLog(sysdate + "  入参|" + inputData.ToString());
                    //int i = f_UserBargaingApply(Ywlx, inputData, outData, retMsg);
                    #endregion
                    string result = string.Empty;
                    #region 新医保接口
                    string NewInputdata = "{\"psn_no\":\"" + bxh + "\",\"mdtrt_id\":\"" + ybjzlsh + "\",\"ipt_otp_no\":\"" + jzlsh + "\"}";
                    //NewInputdata = string.Format(NewInputdata, kh, ybjzlsh, jzlsh);
                    WriteLog(sysdate + "  " + jzlsh + " 进入门诊挂号撤销...");
                    WriteLog(sysdate + "  入参|" + NewInputdata.ToString());
                    int i = YBServiceRequest(YwCode: Ywlx, data: NewInputdata, ref result);
                    #endregion

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
                        strSql = string.Format(@"update mz01h set m1kind='07'  where m1ghno='{0}'", jzlsh);
                        liSQL.Add(strSql);
                    }
                    else
                    {
                        WriteLog(sysdate + "  " + jzlsh + " 进入门诊挂号撤销失败|" + result.ToString());
                        return new object[] { 0, 0, "门诊挂号撤销失败|" + result.ToString() };
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

        #region 门诊就诊信息上传（新医保接口）
        private static object[] YBMZJZXXSC(OutpatientIn inparams)
        {
            string ywCode = "2203";
            string sysdate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string data = JsonConvert.SerializeObject(inparams);
            string res = string.Empty;
            int i = YBServiceRequest(ywCode, data, ref res);
            if (i > 0)
            {
                WriteLog(sysdate + "医保门诊就诊信息上传成功");
                return new object[] { 0, 1, "医保门诊就诊信息上传成功！(●'◡'●)" };
            }
            else
            {
                WriteLog(sysdate + "医保门诊就诊信息上传失败" + res);
                return new object[] { 0, 0, "医保门诊就诊信息上传失败！(*^▽^*)" + res };
            }
        }
        #endregion

        #region 门诊登记(挂号)(新医保接口)
        public static object[] YBMZDJ(object[] objParam)
        {
            string sysdate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            try
            {
                string Ywlx = "2201";
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
                string strbz = string.Format("select '' as bzbm,'' as bzmc ");
                DataSet dsbz = CliUtils.ExecuteSql("sybdj", "cmd", strbz, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                dsbz.Tables[0].Rows.RemoveAt(0);
                // ds.Tables[0].Rows.Add(object[] {'1','1'});

                //实际年龄，参保日期，个人身份，卡状态

                //1095383||632521194606130026|徐桂香|2||1946-06-13|X02460895|
                //    退休|NULL|0|639900|||0.0|||||||||||青海省第六地质勘查院|NULL|||01|0||11|||0|

                #region 获取读卡信息
                if (kxx.Length < 2)
                    return new object[] { 0, 0, "无读卡信息反馈" };
                //string grbh = kxx[0].ToString(); //个人编号
                //string dwbh = kxx[1].ToString(); //单位编号
                //string sfzh = kxx[2].ToString(); //身份证号
                //string xm = kxx[3].ToString(); //姓名
                //string xb = kxx[4].ToString(); //性别
                //string mz = kxx[5].ToString(); //民族
                //string csrq = kxx[6].ToString(); //出生日期
                //string kh = kxx[7].ToString(); //卡号
                //string yldylb = kxx[8].ToString(); //医疗待遇类别
                //string rycbzt = kxx[9].ToString(); //人员参保状态
                //string ydrybz = kxx[10].ToString(); //异地人员标志
                //string tcqh = kxx[11].ToString(); //统筹区号
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

                string grbh = string.IsNullOrEmpty(kxx[0]) || kxx[0] == null || kxx[0] == "NULL" ? "" : kxx[0].ToString(); //个人编号
                string dwbh = string.IsNullOrEmpty(kxx[1]) || kxx[1] == null || kxx[1] == "NULL" ? "" : kxx[1].ToString(); //单位编号
                string sfzh = string.IsNullOrEmpty(kxx[2]) || kxx[2] == null || kxx[2] == "NULL" ? "" : kxx[2].ToString(); //身份证号
                string xm = string.IsNullOrEmpty(kxx[3]) || kxx[3] == null || kxx[3] == "NULL" ? "" : kxx[3].ToString(); //姓名
                string xb = string.IsNullOrEmpty(kxx[4]) || kxx[4] == null || kxx[4] == "NULL" ? "" : kxx[4].ToString(); //性别
                string mz = string.IsNullOrEmpty(kxx[5]) || kxx[5] == null || kxx[5] == "NULL" ? "" : kxx[5].ToString(); //民族
                string csrq = string.IsNullOrEmpty(kxx[6]) || kxx[6] == null || kxx[6] == "NULL" ? "" : kxx[6].ToString(); //出生日期
                string kh = string.IsNullOrEmpty(kxx[7]) || kxx[7] == null || kxx[7] == "NULL" ? "" : kxx[7].ToString(); //卡号
                string yldylb = string.IsNullOrEmpty(kxx[8]) || kxx[8] == null || kxx[8] == "NULL" ? "" : kxx[8].ToString(); //医疗待遇类别
                string rycbzt = string.IsNullOrEmpty(kxx[9]) || kxx[9] == null || kxx[9] == "NULL" ? "" : kxx[9].ToString(); //人员参保状态
                string ydrybz = string.IsNullOrEmpty(kxx[10]) || kxx[10] == null || kxx[10] == "NULL" ? "" : kxx[10].ToString(); //异地人员标志
                string tcqh = string.IsNullOrEmpty(kxx[11]) || kxx[11] == null || kxx[11] == "NULL" ? "" : kxx[11].ToString(); //统筹区号
                string nd = string.IsNullOrEmpty(kxx[12]) || kxx[12] == null || kxx[12] == "NULL" ? "" : kxx[12].ToString(); //年度
                string zyzt = string.IsNullOrEmpty(kxx[13]) || kxx[13] == null || kxx[13] == "NULL" ? "" : kxx[13].ToString(); //在院状态
                string zhye = string.IsNullOrEmpty(kxx[14]) || kxx[14] == null || kxx[14] == "NULL" ? "" : kxx[14].ToString(); //帐户余额
                string bnylflj = string.IsNullOrEmpty(kxx[15]) || kxx[15] == null || kxx[15] == "NULL" ? "" : kxx[15].ToString(); //本年医疗费累计
                string bnzhzclj = string.IsNullOrEmpty(kxx[16]) || kxx[16] == null || kxx[16] == "NULL" ? "" : kxx[16].ToString(); //本年帐户支出累计
                string bntczclj = string.IsNullOrEmpty(kxx[17]) || kxx[17] == null || kxx[17] == "NULL" ? "" : kxx[17].ToString(); //本年统筹支出累计
                string bnjzjzclj = string.IsNullOrEmpty(kxx[18]) || kxx[18] == null || kxx[18] == "NULL" ? "" : kxx[18].ToString(); //本年救助金支出累计
                string bngwybzjjlj = string.IsNullOrEmpty(kxx[19]) || kxx[19] == null || kxx[19] == "NULL" ? "" : kxx[19].ToString(); //本年公务员补助基金累计
                string bnczjmmztczflj = string.IsNullOrEmpty(kxx[20]) || kxx[20] == null || kxx[20] == "NULL" ? "" : kxx[20].ToString(); //本年城镇居民门诊统筹支付累计
                string jrtcfylj = string.IsNullOrEmpty(kxx[21]) || kxx[21] == null || kxx[21] == "NULL" ? "" : kxx[21].ToString(); //进入统筹费用累计
                string jrjzjfylj = string.IsNullOrEmpty(kxx[22]) || kxx[22] == null || kxx[22] == "NULL" ? "" : kxx[22].ToString(); //进入救助金费用累计
                string qfbzlj = string.IsNullOrEmpty(kxx[23]) || kxx[23] == null || kxx[23] == "NULL" ? "" : kxx[23].ToString(); //起付标准累计
                string bnzycs = string.IsNullOrEmpty(kxx[24]) || kxx[24] == null || kxx[24] == "NULL" ? "" : kxx[24].ToString(); //本年住院次数
                string dwmc = string.IsNullOrEmpty(kxx[25]) || kxx[25] == null || kxx[25] == "NULL" ? "" : kxx[25].ToString(); //单位名称
                string nl = string.IsNullOrEmpty(kxx[26]) || kxx[26] == null || kxx[26] == "NULL" ? GetAgeByBirthdate(DateTime.Parse(csrq)).ToString() : kxx[26].ToString(); //年龄
                string cbdwlx = string.IsNullOrEmpty(kxx[27]) || kxx[27] == null || kxx[27] == "NULL" ? "" : kxx[27].ToString(); //参保单位类型
                string jbjgbm = string.IsNullOrEmpty(kxx[28]) || kxx[28] == null || kxx[28] == "NULL" ? "" : kxx[28].ToString(); //经办机构编码


                //  string nl = string.IsNullOrEmpty(kxx[26]) || kxx[26] == null ? "NULL" : kxx[26].ToString(); //年龄

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
                #region 医保获取个人信息
                ReadCardInfo cardInfo = GetYbCardInfo(new object[] { "02", sfzh, "", "", "1", sfzh, xm });
                if (cardInfo == null)
                {
                    return new object[] { 0, 0, "获取医保个人信息出错" };
                }
                #endregion
                #region 判断挂号日期时间有效性
                DateTime ghrqdate = DateTime.Now;
                try
                {
                    ghrqdate = Convert.ToDateTime(ghdjsj);
                    ghrq = Convert.ToDateTime(ghdjsj).ToString("yyyyMMdd");
                    ghsj = Convert.ToDateTime(ghdjsj).ToString("HHmm");
                }
                catch
                {
                    return new object[] { 0, 0, "挂号日期时间格式不正确" };
                }
                #endregion

                string jzlsh1 = "";
                if (string.IsNullOrEmpty(bzbm))
                    jzlsh1 = "MZ" + yllb + jzlsh;
                else
                    jzlsh1 = "MZ" + yllb + jzlsh + bzbm;

                #region 判断是否在HIS中挂号
                string strSql = string.Empty;
                strSql = string.Format(@"
select
                (case when isnull(b.m1name,'')='' then a.m1name else b.m1name  end)   as name,
                (case when isnull(b.m1ksno,'')='' then a.m1ksno else b.m1ksno  end)   as ksno,
                (select top 1 b2ejnm from bz02d  where b2ksno=(case when isnull(b.m1ksno,'')='' then a.m1ksno else b.m1ksno  end)) as ksmc from mz01t  a
                left join  mz01h b on a.m1mzno=b.m1mzno and a.m1ghno=b.m1ghno                 
                where a.m1ghno = '{0}'
				union all 
				
select
                (case when isnull(b.m1name,'')='' then a.m1name else b.m1name  end)   as name,
                (case when isnull(b.m1ksno,'')='' then a.m1ksno else b.m1ksno  end)   as ksno,
                (select top 1 b2ejnm from bz02d  where b2ksno=(case when isnull(b.m1ksno,'')='' then a.m1ksno else b.m1ksno  end)) as ksmc from mz01h  b
                left join  mz01t a on a.m1mzno=b.m1mzno and a.m1ghno=b.m1ghno                 
                where b.m1ghno = '{0}'", jzlsh);
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
                    dgysdm = "010091";
                    dgysxm = "江辉";
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

                #region 老医保接口  暂时屏蔽
                ////入参: 保险号|姓名|卡号|地区编号|医疗类别|科室名称|挂号费|挂号日期|挂号时间|医院门诊流水号
                //StringBuilder inputData = new StringBuilder();
                //inputData.Append(grbh + "|");    //保险号
                //inputData.Append(xm + "|");     //姓名
                //inputData.Append(kh + "|");     //卡号
                //inputData.Append(tcqh + "|");   //地区编号
                //inputData.Append(yllb + "|");   //医疗类别
                //inputData.Append(ksmc + "|");   //科室名称
                //inputData.Append(ghfy + "|");   //挂号费
                //inputData.Append(ghrq + "|");  //挂号日期
                //inputData.Append(ghsj + "|");   //挂号时间
                //inputData.Append(jzlsh);        //医院门诊流水号
                //StringBuilder outData = new StringBuilder(1024);
                //StringBuilder retMsg = new StringBuilder(1024);
                //WriteLog(sysdate + "  " + jzlsh + " 进入门诊挂号...");
                //WriteLog(sysdate + "  入参|" + inputData.ToString());
                //int i = f_UserBargaingApply(Ywlx, inputData, outData, retMsg); 
                #endregion


                #region 新医保接口 by hjw
                #region 入参
                string beginTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                string data = "{\"psn_no\":\"" + grbh + "\",\"insutype\":\"" + cardInfo.insuinfo.insutype + "\",\"begntime\":\"" + beginTime + "\",\"mdtrt_cert_type\":\"03\",\"mdtrt_cert_no\":\"" + kh + "\",\"card_sn\":\"\",\"psn_cert_type\":\"01\",\"certno\":\"" + sfzh + "\",\"psn_type\":\"" + cardInfo.insuinfo.psn_type + "\",\"psn_name\":\"" + xm + "\",\"ipt_otp_no\":\"" + jzlsh + "\",\"atddr_no\":\"" + cfysdm + "\",\"dr_name\":\"" + cfysxm + "\",\"dept_code\":\"" + ksbh + "\",\"dept_name\":\"" + ksmc + "\",\"caty\":\"\"}";
                //data = string.Format(data, grbh, cardInfo.insuinfo.insutype, System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "03", kh, jzlsh, cfysdm, cfysxm, ksbh, ksmc, "");//科别未填 
                WriteLog(sysdate + "  " + jzlsh + " 进入门诊挂号...");
                WriteLog(sysdate + "  入参|" + data.ToString());
                #endregion
                string res = string.Empty;
                int i = YBServiceRequest(Ywlx, data, ref res);



                #endregion
                if (i > 0)
                {

                    JObject jres = JsonConvert.DeserializeObject<JObject>(res);
                    #region 回参
                    string mdtrt_id = jres["mdtrt_id"].ToString();//就诊ID
                    string psn_no = jres["psn_no"].ToString();//人员编号
                    string ipt_otp_no = jres["ipt_otp_no"].ToString();//就诊流水号
                    #endregion

                    #region 门诊就诊信息上传
                    PatientInfo patient = new PatientInfo()
                    {
                        mdtrt_id = jzlsh,//就诊id
                        psn_no = psn_no,//人员编号
                        med_type = cardInfo.insuinfo.psn_type,//人员类别
                        begntime = ghrqdate.ToString("yyyy-MM-dd"),//开始时间
                        main_cond_dscr = "",//主要病情描述
                        dise_codg = bzbm,//病种编码
                        dise_name = bzmc,//病种名称
                        birctrl_type = "",//计划生育手术类别
                        birctrl_matn_date = "",//计划生育手术或生育日期
                    };
                    List<DiagInfo> diaglist = new List<DiagInfo>()
                    {
                        new DiagInfo()
                        {
                         diag_code=bzbm,//诊断编码
                         diag_type="1",//诊断类别
                         diag_dept=ksbh,//诊断科室
                         dise_dor_no=dgysdm,//诊断医生编码
                         dise_dor_name=dgysxm,//诊断医生名称
                         diag_name=bzmc,//诊断名称
                         diag_srt_no="1",//诊断排序号
                         vali_flag="1"//有效标识
                        }
                    };

                    OutpatientIn inparams = new OutpatientIn()
                    {
                        mdtrtinfo = patient,
                        diseinfo = diaglist
                    };
                    object[] jzxxscres = YBMZJZXXSC(inparams);
                    if (jzxxscres[1].ToString() == "0")
                    {
                        object[] objParam2 = new object[] { jzlsh, grbh, xm, kh, dqbh, mdtrt_id };
                        NYBMZDJCX(objParam2);
                        return jzxxscres;
                    }
                    #endregion

                    #region 出参




                    List<string> liSQL = new List<string>();
                    string strValue = "";
                    //string[] str = outData.ToString().Split(';');
                    //string[] str2 = str[0].Split('|');
                    string bxh2 = grbh.Trim();       //保险号
                    string xm2 = xm.Trim();        //姓名
                    string kh2 = kh.Trim();        //卡号
                    string dqbh1 = tcqh.Trim();      //地区编号
                    string dqmc = cardInfo.insuinfo.insuplc_admdvs.Trim();       //地区名称
                    string csrq1 = csrq.Trim();       //出生日期
                    string sjnl = nl.Trim();       //实际年龄
                    string cbrq = cardInfo.idetinfo.begntime.Trim();       //参保日期
                    string grsf = cardInfo.idetinfo.psn_type_lv.Trim();       //个人身份
                    string dwmc1 = cardInfo.insuinfo.emp_name.Trim();       //单位名称
                    string xb1 = cardInfo.baseinfo.gend;        //性别
                    string rylb = cardInfo.insuinfo.psn_type.Trim();      //医疗人员类别
                    //string kzt = str2[12].Trim();       //卡状态
                    string grzhye = cardInfo.insuinfo.balc.Trim();    //账户余额
                    string jzlsh2 = jzlsh.Trim();    //门诊(住院号)
                    string yllb2 = yllb.Trim();     //医疗类别
                    string ksmc2 = ksmc.Trim();     //科室名称
                                                    // string ghf = str2[17].Trim();       //挂号费
                                                    // string bckbcs = str2[18].Trim();    //本次看病次数
                                                    //string zych = str2[19].Trim();      //住院床号
                                                    //   string djrq = str2[20].Trim();      //入院日期
                                                    //      string djsj = str2[21].Trim();      //入院时间
                    string jbr = GetEmplNameById(CliUtils.fLoginUser).Trim();       //经办人
                    //if (string.IsNullOrEmpty(ghf)) //挂号费为NULL时，传入0
                    //    ghf = "0.00";
                    #endregion

                    string bzbm_r = string.Empty;
                    string bzmc_r = string.Empty;
                    string bzbm_r2 = string.Empty;
                    string bzmc_r2 = string.Empty;
                    string bzbm_r3 = string.Empty;
                    string bzmc_r3 = string.Empty;
                    string bzbm_r4 = string.Empty;
                    string bzmc_r4 = string.Empty;
                    #region 暂时屏蔽
                    //if (str.Length > 1)
                    //{
                    //    if (str.Length < 3)
                    //    {
                    //        if (yllb == "31" || yllb == "32")
                    //        {
                    //            str2 = str[1].Split('|');
                    //            bzbm = str2[0].Trim();
                    //            bzmc = str2[1].Trim();
                    //            jzlsh1 = "MZ" + yllb + jzlsh + bzbm;
                    //            #region 判断是否进门诊挂号登记
                    //            strSql = string.Format(@"select jzlsh from ybmzzydjdr where jzlsh1='{0}' and cxbz=1 ", jzlsh1);
                    //            // MessageBox.Show(strSql);
                    //            ds.Tables.Clear();
                    //            ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                    //            if (ds.Tables[0].Rows.Count > 0)
                    //            {

                    //                WriteLog(sysdate + "  " + jzlsh + " 进入门诊挂号成功|该患者已经进行过医保门诊登记(单病种)|");
                    //                ////门诊登记（挂号）撤销
                    //                //// MessageBox.Show(jzlsh1);
                    //                //object[] objParam2 = new object[] { jzlsh1 };
                    //                //objParam2 = YBMZDBZDJCX(objParam2);
                    //                return new object[] { 0, 0, "该患者已经进行过医保门诊登记" };
                    //            }
                    //            #endregion
                    //        }

                    //    }

                    //    for (int j = 1; j < str.Length; j++)
                    //    {

                    //        str2 = str[j].Split('|');
                    //        if (j == 1)
                    //        {
                    //            bzbm_r = str2[0].Trim();
                    //            bzmc_r = str2[1].Trim();
                    //            DataRow dr = dsbz.Tables[0].NewRow();
                    //            dr[0] = str2[0].Trim();
                    //            dr[1] = str2[1].Trim();
                    //            dsbz.Tables[0].Rows.Add(dr);
                    //        }
                    //        else if (j == 2)
                    //        {
                    //            bzbm_r2 = str2[0].Trim();
                    //            bzmc_r2 = str2[1].Trim();
                    //            DataRow dr = dsbz.Tables[0].NewRow();
                    //            dr[0] = str2[0].Trim();
                    //            dr[1] = str2[1].Trim();
                    //            dsbz.Tables[0].Rows.Add(dr);
                    //        }
                    //        else if (j == 3)
                    //        {
                    //            bzbm_r3 = str2[0].Trim();
                    //            bzmc_r3 = str2[1].Trim();
                    //            DataRow dr = dsbz.Tables[0].NewRow();
                    //            dr[0] = str2[0].Trim();
                    //            dr[1] = str2[1].Trim();
                    //            dsbz.Tables[0].Rows.Add(dr);
                    //        }
                    //        else if (j == 4)
                    //        {
                    //            bzbm_r4 = str2[0].Trim();
                    //            bzmc_r4 = str2[1].Trim();
                    //            DataRow dr = dsbz.Tables[0].NewRow();
                    //            dr[0] = str2[0].Trim();
                    //            dr[1] = str2[1].Trim();
                    //            dsbz.Tables[0].Rows.Add(dr);
                    //        }
                    //        //bzbm = bzbm_r;
                    //        //bzmc = bzmc_r;
                    //        strValue += "病种编码:【" + bzbm_r + "】 病种名称:【" + bzmc_r + "】\r\n";
                    //        strSql = string.Format(@"insert into ybmxbdj (jzlsh,bxh,xm,kh,mmbzbm,mmbzmc) values(
                    //                            '{0}','{1}','{2}','{3}','{4}','{5}')",
                    //                                jzlsh, bxh2, xm2, kh2, str2[0].Trim(), str2[1].Trim());
                    //        liSQL.Add(strSql);
                    //    }
                    //    //if (str.Length > 2)
                    //    //{
                    //    //    MessageBox.Show("该患者有多病种,病种为：\r\n" + strValue);
                    //    //}
                    //} 
                    #endregion

                    strSql = string.Format(@"insert into ybmzzydjdr(grbh,xm,kh,tcqh,bq,csrq,nl,nd,sfzh,dwmc,
                                        xb,yldylb,rycbzt,zhye,ybjzlsh,yllb,ksmc,ghf,zycs,bnzycs,
                                        ghdjsj,jbr,dgysdm,dgysxm,ksbh,jzbz,sysdate,jzlsh,bzbm,bzmc,
                                        jylsh,mmbzbm1,mmbzmc1,ysdm,ysxm,jzlsh1,mmbzbm2,mmbzmc2,mmbzbm3,mmbzmc3,mmbzbm4,mmbzmc4) values(
                                       '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}',
                                       '{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}',
                                       '{20}','{21}','{22}','{23}','{24}','{25}','{26}','{27}','{28}','{29}',
                                       '{30}','{31}','{32}','{33}','{34}','{35}','{36}','{37}','{38}','{39}','{40}','{41}')",
                                        bxh2, xm2, kh2, tcqh, dqmc, csrq, sjnl, "", sfzh, dwmc1,
                                        xb1, yldylb, rycbzt, grzhye, jzlsh2, yllb, ksmc2, "", "", "",
                                        ghdjsj, jbr, dgysdm, dgysxm, ksbh, "m", sysdate, jzlsh, bzbm, bzmc,
                                        jzlsh2, bzbm_r, bzmc_r, cfysdm, cfysxm, jzlsh1, bzbm_r2, bzmc_r2, bzbm_r3, bzmc_r3, bzbm_r4, bzmc_r4);
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
                                WriteLog(sysdate + "  " + jzlsh + " 进入门诊挂号成功|" + res.ToString());
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
                            #region 病种判断  由于新医保接口没有提供对应的数据  暂时屏蔽
                            //if (str.Length > 2)
                            //{
                            //    //门诊登记（挂号）撤销
                            //    //if (bldjdbz)
                            //    //{

                            //    //object[] objParamcx = new object[] { jzlsh };
                            //    //objParamcx = YBMZDJCXBZ(objParamcx);
                            //    //if (objParamcx[1].ToString() == "1")
                            //    //{

                            //    if (string.IsNullOrEmpty(bzbm))
                            //    {
                            //        WriteLog(sysdate + "  " + jzlsh + "多病种选择开始");
                            //        string jzlsh0 = "";
                            //        FrmmxbbNK fbz = new FrmmxbbNK();
                            //        fbz.DataGridView.DataSource = dsbz.Tables[0];
                            //        fbz.StartPosition = FormStartPosition.CenterParent;
                            //        fbz.ShowDialog();

                            //        List<string> liSQL2 = new List<string>();
                            //        WriteLog(sysdate + "  " + jzlsh + "多病种选择" + frmbzbm + "_" + frmbzmc);
                            //        if (string.IsNullOrEmpty(frmbzbm))
                            //            jzlsh0 = "MZ" + yllb + jzlsh;
                            //        else
                            //            jzlsh0 = "MZ" + yllb + jzlsh + frmbzbm;

                            //        #region 判断是否进门诊挂号登记
                            //        strSql = string.Format(@"select jzlsh from ybmzzydjdr where jzlsh1='{0}' and cxbz=1 ", jzlsh0);
                            //        ds.Tables.Clear();
                            //        ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                            //        if (ds.Tables[0].Rows.Count > 0)
                            //        {

                            //            WriteLog(sysdate + "  " + jzlsh + " 进入门诊挂号成功|该患者已经进行过医保门诊登记|" + obj[2].ToString());
                            //            //门诊登记（挂号）撤销
                            //            // MessageBox.Show(jzlsh1);
                            //            object[] objParam2 = new object[] { jzlsh1 };
                            //            objParam2 = YBMZDBZDJCX(objParam2);
                            //            return new object[] { 0, 0, "该患者已经进行过医保门诊登记" };
                            //        }
                            //        #endregion

                            //        string strSql2 = string.Format(@"update ybmzzydjdr set bzbm='{0}',bzmc='{1}',jzlsh1='{3}' where jzlsh1='{2}'",
                            //          frmbzbm, frmbzmc, jzlsh1, jzlsh0);
                            //        liSQL2.Add(strSql2);
                            //        object[] obj2 = liSQL2.ToArray();
                            //        obj2 = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj2);
                            //        if (obj[1].ToString() == "1")
                            //        {
                            //            WriteLog(sysdate + "  " + jzlsh + " 进入门诊挂号成功|" + res.ToString());
                            //            return new object[] { 0, 1, "门诊挂号成功" };
                            //        }
                            //        
                            //        else
                            //        {
                            //            WriteLog(sysdate + "  " + jzlsh + " 进入门诊挂号成功|操作本地多病种数据失败|" + obj[2].ToString());
                            //            //门诊登记（挂号）撤销
                            //            object[] objParam2 = new object[] { jzlsh };
                            //            objParam2 = YBMZDJCX(objParam2);
                            //            return new object[] { 0, 0, "门诊挂号失败" };
                            //        }
                            //    }


                            //}

                            //else
                            //{

                            //    WriteLog(sysdate + "  " + jzlsh + " 进入门诊挂号成功|" + res.ToString());
                            //    return new object[] { 0, 1, "门诊挂号成功" };
                            //}
                            #endregion
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
                    WriteLog(sysdate + "  " + jzlsh + " 进入门诊挂号失败|" + res.ToString());
                    return new object[] { 0, 0, "门诊挂号失败|" + res.ToString() };
                }
            }
            catch (Exception ex)
            {
                WriteLog(sysdate + "  门诊读卡异常|" + ex.Message);
                return new object[] { 0, 0, "门诊读卡异常|" + ex.Message };
            }
        }
        #endregion

        #region 门诊登记多病种单个(挂号)撤销（新医保接口）

        public static object[] YBMZDBZDJCX(object[] objParam)
        {
            string sysdate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            try
            {
                string Ywlx = "2202";
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

                string strSql = string.Format(@"select a.* from ybmzzydjdr a where a.jzlsh1='{0}' and a.cxbz=1 
                                                and not exists(select 1 from ybfyjsdr b where a.ybjzlsh=b.ybjzlsh and b.cxbz=1) ", jzlsh);
                // MessageBox.Show(strSql);
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

                    #region 老医保接口
                    ////保险号|姓名|卡号|地区编号|门诊号
                    //StringBuilder inputData = new StringBuilder();
                    //inputData.Append(bxh + "|");
                    //inputData.Append(xm + "|");
                    //inputData.Append(kh + "|");
                    //inputData.Append(dqbh + "|");
                    //inputData.Append(ybjzlsh);
                    //StringBuilder outData = new StringBuilder(1024);
                    //StringBuilder retMsg = new StringBuilder(1024);

                    //WriteLog(sysdate + "  " + jzlsh + " 进入门诊挂号撤销...");
                    //WriteLog(sysdate + "  入参|" + inputData.ToString());
                    //int i = f_UserBargaingApply(Ywlx, inputData, outData, retMsg); 
                    #endregion

                    string result = string.Empty;
                    #region 新医保接口
                    string NewInputdata = "{\"psn_no\":\"" + bxh + "\",\"mdtrt_id\":\"" + ybjzlsh + "\",\"ipt_otp_no\":\"" + jzlsh + "\"}";
                    //NewInputdata = string.Format(NewInputdata, kh, ybjzlsh, jzlsh);
                    WriteLog(sysdate + "  " + jzlsh + " 进入门诊挂号撤销...");
                    WriteLog(sysdate + "  入参|" + NewInputdata.ToString());
                    int i = YBServiceRequest(YwCode: Ywlx, data: NewInputdata, ref result);
                    #endregion
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
                                           where jzlsh1='{0}' and ybjzlsh='{1}' and cxbz=1 ", jzlsh, ybjzlsh, sysdate);
                        liSQL.Add(strSql);
                        strSql = string.Format(@"update ybmzzydjdr set cxbz =2 where jzlsh1 = '{0}' and ybjzlsh='{1}' and cxbz=1", jzlsh, ybjzlsh);
                        liSQL.Add(strSql);
                        //strSql = string.Format(@"delete from ybmxbdj where jzlsh='{0}'", jzlsh);
                        //liSQL.Add(strSql);
                        //strSql = string.Format(@"update mz01h set m1kind='07'  where m1ghno='{0}'", jzlsh);
                        //liSQL.Add(strSql);
                    }
                    else
                    {
                        WriteLog(sysdate + "  " + jzlsh + " 进入门诊挂号撤销失败|" + result.ToString());
                        return new object[] { 0, 0, "门诊挂号撤销失败|" + result.ToString() };
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

        #region 门诊登记(挂号)撤销（新医保接口）


        public static object[] YBMZDJCX(object[] objParam)
        {
            string sysdate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            try
            {
                string Ywlx = "2202";
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
                    #region 老医保接口
                    //StringBuilder inputData = new StringBuilder();
                    //inputData.Append(bxh + "|");
                    //inputData.Append(xm + "|");
                    //inputData.Append(kh + "|");
                    //inputData.Append(dqbh + "|");
                    //inputData.Append(ybjzlsh);
                    //StringBuilder outData = new StringBuilder(1024);
                    //StringBuilder retMsg = new StringBuilder(1024);


                    //WriteLog(sysdate + "  " + jzlsh + " 进入门诊挂号撤销...");
                    //WriteLog(sysdate + "  入参|" + inputData.ToString());
                    //int i = f_UserBargaingApply(Ywlx, inputData, outData, retMsg); 
                    #endregion
                    string res = string.Empty;
                    #region 新医保接口
                    string NewInputdata = "{\"psn_no\":\"" + bxh + "\",\"mdtrt_id\":\"" + ybjzlsh + "\",\"ipt_otp_no\":\"" + jzlsh + "\"}";
                    //NewInputdata = string.Format(NewInputdata, kh, ybjzlsh, jzlsh);
                    WriteLog(sysdate + "  " + jzlsh + " 进入门诊挂号撤销...");
                    WriteLog(sysdate + "  入参|" + NewInputdata.ToString());
                    int i = YBServiceRequest(YwCode: Ywlx, data: NewInputdata, ref res);
                    #endregion
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
                        strSql = string.Format(@"update mz01h set m1kind='07'  where m1ghno='{0}'", jzlsh);
                        liSQL.Add(strSql);
                    }
                    else
                    {
                        WriteLog(sysdate + "  " + jzlsh + " 进入门诊挂号撤销失败|" + res.ToString());
                        return new object[] { 0, 0, "门诊挂号撤销失败|" + res.ToString() };
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

        #region 门诊费用明细上传(新医保)
        //private static DataSet dsinfo = new DataSet();
        private static List<JX.OutPatient.Fee.OutpatientFee> feelists = new List<JX.OutPatient.Fee.OutpatientFee>();
        private static DataSet datasetdetail = new DataSet();
        private static object[] YBMZFYMXSC(object[] outParams)
        {
            #region 参数
            string jzlsh = outParams[0].ToString();  //就诊流水号
            string zhsybz = outParams[1].ToString(); //账户使用标志(；在吉安地区用不到,门诊收费走账户)
            string jssj = outParams[2].ToString();   //结算时间
            string bzbh = outParams[3].ToString();   //病种编号
            string bzmc = outParams[4].ToString();   //病种名称
            string cfhs = outParams[5].ToString();   //处方号集
            string yllb = outParams[6].ToString();   //医疗类别
            string sfje = outParams[7].ToString();   //收费金额
            string cfysdm = outParams[8].ToString(); //处方医生代码
            string cfysxm = outParams[9].ToString(); //处方医生姓名
            string li_xmmxmc = "";//项目名称
            string li_yyrq = "";//处方日期
            string li_yyxmbh = "";//项目编号
            string jylsh = "";
            string xm = dsinfo.Tables[0].Rows[0]["xm"].ToString(); ;// cardinfo.psn_name;//姓名
            string ybjzlsh = dsinfo.Tables[0].Rows[0]["ybjzlsh"].ToString(); ;// dsinfo.Tables[0].Rows[0]["ybjzlsh"].ToString();
            string kh = "";
            string ksno = dsinfo.Tables[0].Rows[0]["ksbh"].ToString();//dsinfo.Tables[0].Rows[0]["ksbh"].ToString();
            string grbh = dsinfo.Tables[0].Rows[0]["grbh"].ToString();
            #endregion

            string sysdate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            decimal sfje2 = decimal.Parse(sfje);
            #region 老Sql语句
            //string strSql = string.Format(@"select b.ybxmbh,b.ybxmmc,a.m4pric as dj,
            //                            sum(a.m4quty) as sl,
            //                            sum(a.m4amnt) as je,
            //                            a.m4item as yyxmbh, a.m4name as yyxmmc,max(a.m4date) as yysj, m4sfno as sfno, 
            //                            b.sfxmzldm as ybsfxmzldm, b.sflbdm as ybsflbdm,b.sfxmdjdm as xmlx,a.m4empn as ysbh,
            //                            a.m4rinvo as invono,a.m4zxks as zxks,a.m4cfno+a.m4sequ as cflsh
            //                            from mz04d a left join ybhisdzdr b on 
            //                            a.m4item=b.hisxmbh where   a.m4ghno='{0}' and b.ybxmbh not in ('001101000010000','001102000010000')  and a.m4cfno in ({1})
            //                            group by b.ybxmbh,b.ybxmmc,a.m4pric,a.m4item,a.m4name,a.m4sfno,b.sfxmzldm,b.sflbdm,b.sfxmdjdm,m4empn,a.m4rinvo,a.m4zxks,a.m4cfno+a.m4sequ
            //                            having sum(a.m4amnt)>0
            //                            union all
            //                            select 
            //                            b.ybxmbh,b.ybxmmc,c.mbpric as dj,
            //                            c.mbquty as sl,
            //                            sum(c.mbsjam) as je,
            //                            c.mbitem as yyxmbh, c.mbname as yyxmmc,max(c.mbdate) as yysj, mbsfno as sfno, 
            //                            b.sfxmzldm as ybsfxmzldm, b.sflbdm as ybsflbdm,b.sfxmdjdm as xmlx,c.mbuser as ysbh,
            //                            c.mbinvo as invono,c.mbzxks as zxks,c.mbzlno+c.mbsequ as cflsh
            //                            from mzb4d  c  left join ybhisdzdr b on c.mbitem=b.hisxmbh 
            //                            where c.mbghno='{0}' and c.mbzlno in ({1})
            //                            group by b.ybxmbh,b.ybxmmc,c.mbpric,c.mbitem,c.mbname,c.mbsfno,b.sfxmzldm,b.sflbdm,b.sfxmdjdm,c.mbuser ,c.mbquty,c.mbinvo,c.mbzxks,c.mbzlno+c.mbsequ
            //                             union all
            //                            select 
            //                            b.ybxmbh,b.ybxmmc,c.mbpric as dj,
            //                            c.mbquty as sl,
            //                            sum(c.mbsjam) as je,
            //                            c.mbitem as yyxmbh, c.mbname as yyxmmc,max(c.mbdate) as yysj, mbsfno as sfno, 
            //                            b.sfxmzldm as ybsfxmzldm, b.sflbdm as ybsflbdm,b.sfxmdjdm as xmlx,c.mbuser as ysbh,
            //                            c.mbinvo as invono,c.mbzxks as zxks,c.mbzlno+c.mbsequ as cflsh
            //                            from mzb2d c  left join ybhisdzdr b on c.mbitem=b.hisxmbh 
            //                            where c.mbghno='{0}' and c.mbzlno in ({1})
            //                            group by b.ybxmbh,b.ybxmmc,c.mbpric,c.mbitem,c.mbname,c.mbsfno,b.sfxmzldm,b.sflbdm,b.sfxmdjdm,c.mbuser,c.mbquty,c.mbinvo,c.mbzxks,c.mbzlno+c.mbsequ
            //                        union all
            //                            select 
            //                            b.ybxmbh,b.ybxmmc,c.mdpric as dj,
            //                            c.mdpqty as sl,
            //                            sum(c.mdamnt) as je,
            //                            c.mdhsdm as yyxmbh, c.mdhsmc as yyxmmc,max(c.mddate) as yysj, c.mdcfno as sfno, 
            //                            b.sfxmzldm as ybsfxmzldm, b.sflbdm as ybsflbdm,b.sfxmdjdm as xmlx,c.mdempn as ysbh,
            //                            c.mdinvo as invono,c.mdzsks as zxks,c.mdcfno+c.mdseq1 as cflsh
            //                            from mzd3d c  left join ybhisdzdr b on c.mdhsdm=b.hisxmbh 
            //                            where c.mdghno='{0}' and c.mdcfno in({1})
            //                            group by b.ybxmbh,b.ybxmmc,c.mdpric,c.mdhsdm,c.mdhsmc,c.mdcfno,b.sfxmzldm,b.sflbdm,b.sfxmdjdm,c.mdempn,c.mdpqty,c.mdinvo,c.mdzsks,c.mdcfno+c.mdseq1", jzlsh, cfhs);
            #endregion

            #region 新Sql语句
            string strSql = string.Format(@"select 
										bb.ybxmbh as ybxmbh,
										bb.ybxmmc as ybxmmc,
										bb.dj as dj,
										sum(bb.sl) as sl,
										sum(bb.je) as je,
										bb.yyxmbh as yyxmbh,
										bb.yyxmmc as yyxmmc,
										max(bb.yysj) as yysj,
										bb.sfno as sfno,
										bb.ybsfxmzldm as ybsfxmzldm,
										bb.ybsflbdm as ybsflbdm,
										bb.xmlx as  xmlx,
										bb.ysbh as ysbh,
										bb.invono as invono,
										bb.zxks as zxks,
										bb.cfh as cfh,
										bb.zxr as zxr,
										bb.yf as yf,
										bb.pc as pc,
										bb.dcjl as dcjl
										 from 
										(select b.ybxmbh,b.ybxmmc,a.mcpric as dj,
                                        a.mcquty as sl,
                                        a.mcamnt as je,
                                        a.mcypno as yyxmbh, a.mcypnm as yyxmmc,a.mcdate as yysj, mcsflb as sfno, 
                                        b.sfxmzldm as ybsfxmzldm, b.sflbdm as ybsflbdm,b.sfxmdjdm as xmlx,a.mcuser as ysbh,
                                        a.mcinvo as invono,a.mcksn3 as zxks,a.mccfno+a.mcseq1+a.mcseq2 as cflsh,
										a.mccfno as cfh,
										a.mcwayx as yf,
										a.mcemp2 as zxr,
										a.mcyphz as pc,
										a.mcusex as dcjl
                                        from mzcfd a left join ybhisdzdr b on  a.mcypno=b.hisxmbh  
										where   a.m4ghno='{0}' and a.m4cfno in ({1})
									union all
									 select 
                                        b.ybxmbh,b.ybxmmc,c.mbpric as dj,
                                        c.mbquty as sl,
                                        c.mbsjam as je,
                                        c.mbitem as yyxmbh, c.mbname as yyxmmc,c.mbdate as yysj, mbsfno as sfno, 
                                        b.sfxmzldm as ybsfxmzldm, b.sflbdm as ybsflbdm,b.sfxmdjdm as xmlx,c.mbuser as ysbh,
                                        c.mbinvo as invono,c.mbzxks as zxks,c.mbzlno+c.mbsequ as cflsh,c.mbzlno as cfh,
										'' as yf,c.mbzxno as zxr,'' as pc , '' as dcjl
                                        from mzb4d  c  left join ybhisdzdr b on c.mbitem=b.hisxmbh 
                                        where c.mbghno='{0}' and c.mbzlno in ({1})
								     union all
                                        select 
                                        b.ybxmbh,b.ybxmmc,c.mbpric as dj,
                                        c.mbquty as sl,
                                        c.mbsjam as je,
                                        c.mbitem as yyxmbh, c.mbname as yyxmmc,c.mbdate as yysj, mbsfno as sfno, 
                                        b.sfxmzldm as ybsfxmzldm, b.sflbdm as ybsflbdm,b.sfxmdjdm as xmlx,c.mbuser as ysbh,
                                        c.mbinvo as invono,c.mbzxks as zxks,c.mbzlno+c.mbsequ as cflsh,c.mbzlno as cfh,
										'' as yf,c.mbzxno as zxr,'' as pc , '' as dcjl
                                        from mzb2d c  left join ybhisdzdr b on c.mbitem=b.hisxmbh 
                                        where c.mbghno='{0}' and c.mbzlno in ({1})
									   union all
                                        select 
                                        b.ybxmbh,b.ybxmmc,c.mdpric as dj,
                                        c.mdpqty as sl,
                                        c.mdamnt as je,
                                        c.mdhsdm as yyxmbh, c.mdhsmc as yyxmmc,c.mddate as yysj, c.mdsflb as sfno, 
                                        b.sfxmzldm as ybsfxmzldm, b.sflbdm as ybsflbdm,b.sfxmdjdm as xmlx,c.mdempn as ysbh,
                                        c.mdinvo as invono,c.mdzsks as zxks,c.mdcfno+c.mdseq1 as cflsh,
										c.mdcfno as cfh,c.mdwayx as yf, c.mdhsdm as zxr,c.mdyphz as pc,c.mdypds as dcjl 
                                        from mzd3d c  left join ybhisdzdr b on c.mdhsdm=b.hisxmbh 
                                      where c.mdghno='{0}' and c.mdcfno in({1})
										) bb
                                        group by bb.ybxmbh,bb.ybxmmc,bb.dj,bb.yyxmbh,bb.yyxmmc,bb.sfno,bb.ybsfxmzldm,bb.ybsflbdm,bb.xmlx,bb.ysbh,bb.invono,bb.zxks,bb.cflsh,bb.cfh,bb.zxr,bb.yf,bb.pc,bb.dcjl
                                        having sum(bb.je)>0", jzlsh, cfhs);
            #endregion

            DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            StringBuilder strMsg = new StringBuilder();
            List<OutpatientFee> feeinfos = new List<OutpatientFee>();
            decimal sfje3 = 0m;
            if (ds.Tables[0].Rows.Count <= 0)
            {
                WriteLog(sysdate + "|无收费明细");
                return new object[] { 0, 0, "无收费明细！" };
            }
            else
            {
                #region 费用明细上传
                datasetdetail = new DataSet();
                datasetdetail = ds;
                for (int f = 0; f < ds.Tables[0].Rows.Count; f++)
                {
                    DataRow dr = ds.Tables[0].Rows[f];
                    if (dr["ybxmbh"] == DBNull.Value)
                        strMsg.Append("项目代码：[" + dr["yyxmbh"].ToString() + "]，名称：[" + dr["yyxmmc"].ToString() + "]未对照，不能上传！");
                    else
                    {
                        sfje3 += decimal.Parse(dr["je"].ToString());

                        li_xmmxmc += f == ds.Tables[0].Rows.Count - 1 ? dr["ybxmmc"].ToString() : dr["ybxmmc"].ToString() + "|";
                        li_yyxmbh += f == ds.Tables[0].Rows.Count - 1 ? dr["yyxmbh"].ToString() : dr["yyxmbh"].ToString() + "|";
                        #region 费用入参
                        OutpatientFee fee = new OutpatientFee()
                        {
                            feedetl_sn = dr["cflsh"].ToString(),//费用明细流水号
                            mdtrt_id = ybjzlsh,//就诊id
                            psn_no = grbh,//个人编号
                            chrg_bchno = dr["sfno"].ToString(),//收费批次号
                            dise_codg = bzbh,//病种编码
                            rxno = dr["cfh"].ToString(),//处方号
                            rx_circ_flag = "",//外购处方标识 字典
                            fee_ocur_time = Convert.ToDateTime(dr["yysj"].ToString()).ToString("yyyyMMddhhmmss"),//费用时间
                            med_list_codg = dr["ybxmbh"].ToString(),//医疗中心编码
                            medins_list_codg = dr["yyxmbh"].ToString(),//医药机构目录编码
                            det_item_fee_sumamt = Decimal.Parse(dr["je"].ToString()),//明细项目费用总额
                            cnt = Decimal.Parse(dr["sl"].ToString()),//数量
                            pric = Decimal.Parse(dr["dj"].ToString()),//单价
                            sin_dos_dscr = dr["dcjl"].ToString(),//单次剂量描述
                            used_frqu_dscr = dr["pc"].ToString(),//使用频次描述
                            prd_days = 1,//周期天数
                            medc_way_dscr = dr["yf"].ToString(),//用药途径描述
                            bilg_dept_codg = ksno,//开单科室编码
                            bilg_dept_name = GetDeptNameById(ksno),//开单科室编码
                            bilg_dr_codg = dr["ysbh"].ToString(),//开单医生编码
                            bilg_dr_name = GetEmplNameById(dr["ysbh"].ToString()),//开单医生名称
                            acord_dept_codg = dr["zxks"].ToString(),//受单科室编码
                            acord_dept_name = GetDeptNameById(dr["zxks"].ToString()),//受单科室名称
                            orders_dr_code = dr["zxr"].ToString(),//受单医生编码
                            orders_dr_name = GetEmplNameById(dr["zxr"].ToString()),//受单医生名称
                            hosp_appr_flag = "1",//医院审批标识 字典
                            tcmdrug_used_way = "",//中药使用方式 字典
                            etip_flag = "",//外检标识 字典
                            etip_hosp_code = "",//外检医院编码
                            dscg_tkdrug_flag = "",//出院带药标识 字典
                            matn_fee_flag = "" //生育费用标志 字典

                        };
                        #endregion

                        #region 新费用
                        //OutPatientFeeNew feeinfo = new OutPatientFeeNew()
                        //{
                        //    ipt_otp_no = ybjzlsh,//门诊流水号
                        //    list_type = dr["ybsfxmzldm"].ToString(),//三大目录类别
                        //    rxno = dr["xmlx"].ToString(),//处方号
                        //    feedetl_sn = dr["cflsh"].ToString(),//处方流水号
                        //    fee_ocur_time = Convert.ToDateTime(dr["yysj"].ToString()).ToString("yyyyMMddhhmmss"),//处方日期 
                        //    med_list_codg = dr["yyxmbh"].ToString(),//收费项目中心编码 
                        //    pric = decimal.Parse(dr["dj"].ToString()),//单价
                        //    cnt = decimal.Parse(dr["sl"].ToString()),//数量
                        //    umamt = decimal.Parse(dr["je"].ToString()),//金额
                        //    bilg_dr_codg = dr["ysbh"].ToString(),//医生编码
                        //    bilg_dept_codg = ksno,//科室编码
                        //    min_unit = " ",//是否最小计量单位  空默认否
                        //};
                        #endregion


                        feeinfos.Add(fee);
                    }

                }
                if (!string.IsNullOrEmpty(strMsg.ToString()))
                {

                    WriteLog(sysdate + "门诊费用查询出现异常" + strMsg.ToString());
                    return new object[] { 0, 0, "门诊费用查询出现异常" + strMsg.ToString() };

                }
                #endregion
            }
            if (Math.Abs(sfje2 - sfje3) > 1.0m)
            {
                return new object[] { 0, 0, "收费金额与医保结算金额相差" + Math.Abs(sfje2 - sfje3) + ",无法结算，请核实!" };
            }
            string Ywlx = "2204";//医保业务类型
            if (feeinfos == null)
            {
                return new object[] { 0, 0, "参数传入有误" };
            }

            if (feeinfos.Count == 0)
            {
                return new object[] { 0, 0, "请上传费用明细" };
            }
            feelists = feeinfos;
            string res = "";//请求回参
            string data = JsonConvert.SerializeObject(feeinfos);
            //日志
            WriteLog(sysdate + "|门诊费用明细上传|");
            WriteLog(sysdate + "|入参|" + data.ToString());

            //新医保接口
            int i = YBServiceRequest(Ywlx, data, ref res);

            ds.Dispose();
            if (i > 0)
            {
                List<OutpatientFeeOutParam> cfmxscfhList = new List<OutpatientFeeOutParam>();
                try
                {
                    cfmxscfhList = JsonConvert.DeserializeObject<List<OutpatientFeeOutParam>>(res);
                }
                catch
                {

                }

                List<string> liSQL = new List<string>();
                for (int m = 0; m < cfmxscfhList.Count; m++)
                {
                    OutpatientFeeOutParam info = cfmxscfhList[m];
                    //出参:住院号|处方号|项目编号|项目名称|项目等级|收费类别|单价|数量|金额|处方日期|处方上传时间
                    DataRow dr1 = ds.Tables[0].Rows[m];
                    string jzlsh2 = jzlsh; //就诊流水号  
                    string ybghmc = li_xmmxmc.Split('|')[m]; //项目名称
                    string xmdj = info.chrgitm_lv.Trim();   //项目等级
                    string sflb = info.med_chrgitm_type;   //收费类别
                    string dj2 = info.pric.ToString();    //单价
                    string sl2 = "1";    //数量 
                    string scsj = sysdate;  //处方上传时间

                    #region 回参的数据
                    string ybcfh = info.feedetl_sn;  //处方号
                    string ybcflsh = info.feedetl_sn;//处方流水号  新加
                    string yyrq = jssj;   //处方日期
                    string ybxmbm = li_yyxmbh.Split('|')[m]; //项目编号
                    string je2 = info.det_item_fee_sumamt.ToString().Trim();    //金额
                    string zfje = info.fulamt_ownpay_amt.ToString();//自费金额  新加
                    string cxjje = info.overlmt_amt.ToString();//超限价金额 新加
                    string zfsx = info.pric_uplmt_amt.ToString();//支付上限 新加
                    string memo = info.memo;//说明信息  新加
                    #endregion

                    if (li_xmmxmc.Split('|')[m].Equals(ybxmbm) && Math.Round(Convert.ToDecimal(dj2), 2, MidpointRounding.AwayFromZero) == Math.Round(Convert.ToDecimal(info.det_item_fee_sumamt), 2, MidpointRounding.AwayFromZero))
                    //if (li_ybxmbh[j].Equals(ybxmbm) && Convert.ToDecimal(dj2) == Math.Round(Convert.ToDecimal(li_dj[j]), 2, MidpointRounding.AwayFromZero))
                    // if (li_ybxmbh[j].Equals(ybxmbm) && li_dj[j].Equals(Convert.ToDouble(dj2).ToString()))
                    {
                        //MessageBox.Show(li_dj[j] + "||" + Convert.ToDouble(dj2).ToString());
                        string Sql = string.Format(@"insert into ybcfmxscindr(jzlsh,jylsh,je,xm,kh,yysfxmbm,yysfxmmc,sysdate,ybjzlsh,ybcfh,sfxmzxbm,
                                                        sfxmzxmc,sfxmzl,sflb,dj,sl,cfrq,cfscsj,cxbz,zfje,remark,ybcflsh,cxjje,zfsx) values(
                                                        '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}',
                                                        '{11}','{12}','{13}','{14}','{15}','{16}','{17}','1','{18}','{19}','{20}','{21}','{22}')",
                                                jzlsh, jylsh, je2, xm, kh, li_yyxmbh.Split('|')[m], li_xmmxmc.Split('|')[m], sysdate, ybjzlsh, ybcfh, ybxmbm,
                                                li_yyxmbh.Split('|')[m], xmdj, sflb, dj2, sl2, yyrq, scsj, zfje, memo, ybcflsh, cxjje, zfsx);
                        liSQL.Add(Sql);
                        WriteLog(sysdate + "  " + jzlsh + "上传处方明细-->" + li_yyxmbh.Split('|')[m] + "|" + li_xmmxmc.Split('|')[m] + "|" + res);
                    }

                }
                object[] obj = liSQL.ToArray();
                obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);
                if (obj[1].ToString().Equals("1"))
                {
                    WriteLog(sysdate + "  上传处方明细|" + "");
                    return new object[] { 0, 1, "上传处方明细成功！" };
                }
                else
                {
                    WriteLog(sysdate + "  上传处方明细|保存本地数据失败|" + obj[2].ToString());
                    return new object[] { 0, 0, "上传处方明细｜保存本地数据失败｜" + obj[2].ToString() };
                }
                WriteLog(sysdate + $"门诊费用明细{0}~{feeinfos.Count}条上传成功！" + res);
                return new object[] { 0, 1, $"门诊费用明细{0}~{feeinfos.Count}条上传成功！" };
            }
            else
            {
                RollBackMZFY(feeinfos);

                WriteLog(sysdate + $"门诊费用明细{0}~{feeinfos.Count}条上传失败！" + res);
                return new object[] { 0, 0, $"门诊费用明细{0}~{feeinfos.Count}条上传失败！" };
            }


        }
        #endregion

        #region 门诊费用撤销上传调用方法
        public static void RollBackMZFY(List<OutpatientFee> feelist)
        {
            int code = 1;
            string sysdate = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            object[] objcx = null;
            foreach (var item in feelist)
            {
                objcx = YBMZFYMXSCCX(item.mdtrt_id, item.rxno, item.feedetl_sn);
                if (objcx[1].ToString() == "0")
                {
                    code = 0;
                }
            }
            if (code == 1)
            {
                MessageBox.Show("撤销成功！请重新进行结算！");
            }
            else
            {
                MessageBox.Show("撤销失败！" + objcx[2].ToString());
                WriteLog(sysdate + $"费用明细上传失败！" + objcx[2].ToString());
            }
        }
        #endregion

        #region 门诊费用明细上传撤销（新医保）
        /// <summary>
        ///  门诊费用明细上传撤销（新医保）
        /// </summary>
        /// <param name="jzid">就诊id</param>
        /// <param name="sfpch">收费批次号</param>
        /// <param name="psnNo">人员编号</param>
        /// <returns></returns>
        private static object[] YBMZFYMXSCCX(string jzid, string sfpch, string psnNo)
        {
            if (string.IsNullOrEmpty(jzid))
            {
                return new object[] { 0, 0, "就诊id不得为空" };
            }
            if (string.IsNullOrEmpty(sfpch))
            {
                return new object[] { 0, 0, "收费批次号不得为空" };
            }
            if (string.IsNullOrEmpty(psnNo))
            {
                return new object[] { 0, 0, "人员编号不得为空" };
            }
            //新医保接口
            string Ywlx = "2205";

            string sysdate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string res = "";
            string data = "{\"mdtrt_id\":\"" + jzid + "\",\"chrg_bchno\":\"" + sfpch + "\",\"psn_no\":\"" + psnNo + "\"}";
            //data = string.Format(data, jzid, sfpch, psnNo);
            WriteLog(sysdate + "|门诊费用明细撤销|");
            WriteLog(sysdate + "|入参|" + data);
            int i = YBServiceRequest(Ywlx, data, ref res);
            if (i > 0)
            {
                return new object[] { 0, 1, "费用明细撤销成功！" };
            }
            else
            {
                return new object[] { 0, 0, "费用明细撤销失败" };
            }
        }

        #endregion

        #region 门诊登记（挂号）收费（新医保接口）
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

                string[] kxxarr = objParam[4].ToString().Split('|'); //读卡信息
                #region 获取读卡信息
                if (kxx.Length < 2)
                    return new object[] { 0, 0, "无读卡信息反馈" };

                string grbh = string.IsNullOrEmpty(kxxarr[0]) || kxxarr[0] == null ? "NULL" : kxxarr[0].ToString(); //个人编号
                string sfzh = string.IsNullOrEmpty(kxxarr[2]) || kxxarr[2] == null ? "NULL" : kxxarr[2].ToString(); //身份证号
                string mz = string.IsNullOrEmpty(kxxarr[5]) || kxxarr[5] == null ? "NULL" : kxxarr[5].ToString(); //民族
                string yldylb = string.IsNullOrEmpty(kxxarr[8]) || kxxarr[8] == null ? "NULL" : kxxarr[8].ToString(); //医疗待遇类别
                string rycbzt = string.IsNullOrEmpty(kxxarr[9]) || kxxarr[9] == null ? "NULL" : kxxarr[9].ToString(); //人员参保状态
                string ydrybz = string.IsNullOrEmpty(kxxarr[10]) || kxxarr[10] == null ? "NULL" : kxxarr[10].ToString(); //异地人员标志
                string tcqh = string.IsNullOrEmpty(kxxarr[11]) || kxxarr[11] == null ? "NULL" : kxxarr[11].ToString(); //统筹区号
                string nd = string.IsNullOrEmpty(kxxarr[12]) || kxxarr[12] == null ? "NULL" : kxxarr[12].ToString(); //年度
                string zyzt = string.IsNullOrEmpty(kxxarr[13]) || kxxarr[13] == null ? "NULL" : kxxarr[13].ToString(); //在院状态
                string zhye = string.IsNullOrEmpty(kxxarr[14]) || kxxarr[14] == null ? "NULL" : kxxarr[14].ToString(); //帐户余额
                string bnylflj = string.IsNullOrEmpty(kxxarr[15]) || kxxarr[15] == null ? "NULL" : kxxarr[15].ToString(); //本年医疗费累计
                string bnzhzclj = string.IsNullOrEmpty(kxxarr[16]) || kxxarr[16] == null ? "NULL" : kxxarr[16].ToString(); //本年帐户支出累计
                string bntczclj = string.IsNullOrEmpty(kxxarr[17]) || kxxarr[17] == null ? "NULL" : kxxarr[17].ToString(); //本年统筹支出累计
                string bnjzjzclj = string.IsNullOrEmpty(kxxarr[18]) || kxxarr[18] == null ? "NULL" : kxxarr[18].ToString(); //本年救助金支出累计
                string bngwybzjjlj = string.IsNullOrEmpty(kxxarr[19]) || kxxarr[19] == null ? "NULL" : kxxarr[19].ToString(); //本年公务员补助基金累计
                string bnczjmmztczflj = string.IsNullOrEmpty(kxxarr[20]) || kxxarr[20] == null ? "NULL" : kxxarr[20].ToString(); //本年城镇居民门诊统筹支付累计
                string jrtcfylj = string.IsNullOrEmpty(kxxarr[21]) || kxxarr[21] == null ? "NULL" : kxxarr[21].ToString(); //进入统筹费用累计
                string jrjzjfylj = string.IsNullOrEmpty(kxxarr[22]) || kxxarr[22] == null ? "NULL" : kxxarr[22].ToString(); //进入救助金费用累计
                string qfbzlj = string.IsNullOrEmpty(kxxarr[23]) || kxxarr[23] == null ? "NULL" : kxxarr[23].ToString(); //起付标准累计
                string bnzycs = string.IsNullOrEmpty(kxxarr[24]) || kxxarr[24] == null ? "NULL" : kxxarr[24].ToString(); //本年住院次数
                string nl = string.IsNullOrEmpty(kxxarr[26]) || kxxarr[26] == null ? "NULL" : kxxarr[26].ToString(); //年龄
                string cbdwlx = string.IsNullOrEmpty(kxxarr[27]) || kxxarr[27] == null ? "NULL" : kxxarr[27].ToString(); //参保单位类型
                string jbjgbm = string.IsNullOrEmpty(kxxarr[28]) || kxxarr[28] == null ? "NULL" : kxxarr[28].ToString(); //经办机构编码
                #endregion
                //医保登记
                objParam1 = YBMZDJ(objParam1);
                if (objParam1[1].ToString().Equals("1"))
                {

                    //string jzlsh1 = "";
                    //if (string.IsNullOrEmpty(bzbm))

                    //    jzlsh1 = "MZ" + yllb + jzlsh;

                    //else
                    //    jzlsh1 = "MZ" + yllb + jzlsh + bzbm;
                    // string jzlsh1 = "MZ" + yllb +bzbm + jzlsh;

                    //门诊登记成功,进入门诊登记（挂号）收费结算
                    #region 获取门诊登记信息
                    string strSql = string.Format("select * from ybmzzydjdr where jzlsh='{0}' and cxbz=1", jzlsh);
                    DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                    if (ds.Tables[0].Rows.Count == 0)
                        return new object[] { 0, 0, "该患者未做医保登记" };
                    string bxh = ds.Tables[0].Rows[0]["grbh"].ToString();//保险号
                    string xm = ds.Tables[0].Rows[0]["xm"].ToString();//姓名
                    string kh = ds.Tables[0].Rows[0]["kh"].ToString();//卡号
                    string dqbh = ds.Tables[0].Rows[0]["tcqh"].ToString();//地区编号
                    string dgysxm1 = ds.Tables[0].Rows[0]["dgysxm"].ToString();//门诊开单医生
                    string ybjzlsh = ds.Tables[0].Rows[0]["ybjzlsh"].ToString();//医保就诊流水号
                    string ghf = string.IsNullOrEmpty(ds.Tables[0].Rows[0]["ghf"].ToString()) ? "0.00" : ds.Tables[0].Rows[0]["ghf"].ToString();//挂号费
                    string ksno = ds.Tables[0].Rows[0]["ksbh"].ToString();//科室编号
                    string ksmc = ds.Tables[0].Rows[0]["ksmc"].ToString();//科室名称
                    string ysmc = ds.Tables[0].Rows[0]["ysxm"].ToString();//医生名称
                    string ysbh = ds.Tables[0].Rows[0]["ysdm"].ToString();//医生编号
                    string bzbm1 = "NULL";//病种编码
                    string bzmc1 = "NULL";//病种名称
                    if (!yllb.Equals("11"))
                    {
                        bzbm1 = ds.Tables[0].Rows[0]["bzbm"].ToString();
                        bzmc1 = ds.Tables[0].Rows[0]["bzmc"].ToString();
                    }
                    string sfdypj = "False"; //是否打印票据
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
                    string invoiceNo = string.Empty;//发票号
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
                    //                    strSql = string.Format(@"select  z.ybxmbh ybxmbh, z.ybxmmc ybxmmc, c.bzmem1 as dj,1 as sl,
                    //                                        a.m1gham je,c.bzmem2 yyxmbh, c.bzname yyxmmc, a.m1invo,a.m1blam,a.m1kham,a.m1amnt
                    //                                        from mz01t a 
                    //                                        left join  mz01h b on a.m1mzno=b.m1mzno and a.m1ghno=b.m1ghno        
                    //                                        join bztbd c on a.m1zlfb = c.bzkeyx and c.bzcodn = 'A7' 
                    //                                        left join ybhisdzdr z on c.bzmem2 = z.hisxmbh                
                    //                                        where a.m1ghno = '{0}'", jzlsh);

                    strSql = string.Format(@"select  z.ybxmbh ybxmbh, z.ybxmmc ybxmmc, c.bzmem1 as dj,1 as sl,
                                case when b.m1gham IS null  then a.m1gham else b.m1gham end  je,
                                c.bzmem2 yyxmbh, c.bzname yyxmmc, 
                                (case when ISNULL(b.m1invo,'')='' then  a.m1invo else b.m1invo end) as m1invo,
                                case when b.m1blam IS null  then a.m1blam else b.m1blam end  m1blam,
                                case when b.m1kham IS null  then a.m1kham else b.m1kham end  m1kham,
                                case when b.m1amnt IS null  then a.m1amnt else b.m1amnt end  m1amnt
                                from mz01t a 
                                left join  mz01h b on a.m1mzno=b.m1mzno and a.m1ghno=b.m1ghno        
                                join bztbd c on (case when ISNULL(b.m1zlfb,'')='' then  a.m1zlfb else b.m1zlfb end) = c.bzkeyx and c.bzcodn = 'A7' 
                                left join ybhisdzdr z on c.bzmem2 = z.hisxmbh                
                                where a.m1ghno = '{0}'
                                union all 
                                select  z.ybxmbh ybxmbh, z.ybxmmc ybxmmc, c.bzmem1 as dj,1 as sl,
                                case when b.m1gham IS null  then a.m1gham else b.m1gham end  je,
                                c.bzmem2 yyxmbh, c.bzname yyxmmc, 
                                (case when ISNULL(b.m1invo,'')='' then  a.m1invo else b.m1invo end) as m1invo,
                                case when b.m1blam IS null  then a.m1blam else b.m1blam end  m1blam,
                                case when b.m1kham IS null  then a.m1kham else b.m1kham end  m1kham,
                                case when b.m1amnt IS null  then a.m1amnt else b.m1amnt end  m1amnt
                                from mz01h b 
                                left join  mz01t a on a.m1mzno=b.m1mzno and a.m1ghno=b.m1ghno        
                                join bztbd c on (case when ISNULL(b.m1zlfb,'')='' then  a.m1zlfb else b.m1zlfb end) = c.bzkeyx and c.bzcodn = 'A7' 
                                left join ybhisdzdr z on c.bzmem2 = z.hisxmbh                
                                where b.m1ghno = '{0}'", jzlsh);
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
                    invoiceNo = dr["m1invo"].ToString();
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
                        dj = Convert.ToDouble(dr["dj"].ToString());//单价
                        sl = Convert.ToDouble(dr["sl"].ToString());//数量
                        je = Convert.ToDouble(dr["je"].ToString());//金额
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

                    #region 医保获取个人信息
                    ReadCardInfo cardInfo = GetYbCardInfo(new object[] { "02", sfzh, "", "", "1", sfzh, xm });
                    if (cardInfo == null)
                    {
                        return new object[] { 0, 0, "获取医保个人信息出错" };
                    }
                    #endregion


                    #region 挂号费费用明细上传
                    List<OutpatientFee> fees = new List<OutpatientFee>();
                    strSql = string.Format(@"select b.ybxmbh,b.ybxmmc,a.m4pric as dj,
                                        sum(a.m4quty) as sl,
                                        sum(a.m4amnt) as je,
                                        a.m4item as yyxmbh, a.m4name as yyxmmc,max(a.m4date) as yysj, m4sfno as sfno, 
                                        b.sfxmzldm as ybsfxmzldm, b.sflbdm as ybsflbdm,b.sfxmdjdm as xmlx,a.m4empn as ysbh,
                                        a.m4rinvo as invono,a.m4zxks as zxks,a.m4cfno+a.m4sequ as cflsh,a.m4cfno as  cfh
                                        from mz04d a left join ybhisdzdr b on 
                                        a.m4item=b.hisxmbh where   a.m4ghno='{0}' and b.ybxmbh  in ('001101000010000','001102000010000')
                                        group by b.ybxmbh,b.ybxmmc,a.m4pric,a.m4item,a.m4name,a.m4sfno,b.sfxmzldm,b.sflbdm,b.sfxmdjdm,m4empn,a.m4rinvo,a.m4zxks,a.m4cfno+a.m4sequ
                                        having sum(a.m4amnt)>0", jzlsh);
                    ds.Tables.Clear();
                    ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                    if (ds.Tables[0].Rows.Count <= 0)
                    {
                        return new object[] { 0, 0, "上传挂号费用时没有找到对应的明细！" };
                    }

                    #region 新费用
                    for (int m = 0; m < ds.Tables[0].Rows.Count; m++)
                    {
                        DataRow dr1 = ds.Tables[0].Rows[m];
                        //OutPatientFeeNew feeinfo = new OutPatientFeeNew()
                        //{
                        //    ipt_otp_no = ybjzlsh,//门诊流水号
                        //    list_type = dr1["ybsfxmzldm"].ToString(),//三大目录类别
                        //    rxno = dr1["xmlx"].ToString(),//处方号
                        //    feedetl_sn = dr1["cflsh"].ToString(),//处方流水号
                        //    fee_ocur_time = Convert.ToDateTime(dr1["yysj"].ToString()).ToString("yyyyMMddhhmmss"),//处方日期 
                        //    med_list_codg = dr1["ybxmbh"].ToString(),//收费项目中心编码 
                        //    pric = decimal.Parse(dr1["dj"].ToString()),//单价
                        //    cnt = decimal.Parse(dr1["sl"].ToString()),//数量
                        //    umamt = decimal.Parse(dr1["je"].ToString()),//金额
                        //    bilg_dr_codg = dgysdm,//医生编码
                        //    bilg_dept_codg = ksbh,//科室编码
                        //    min_unit = " ",//是否最小计量单位  空默认否
                        //};
                        OutpatientFee fee = new OutpatientFee()
                        {
                            feedetl_sn = dr["cflsh"].ToString(),//费用明细流水号
                            mdtrt_id = ybjzlsh,//就诊id
                            psn_no = grbh,//个人编号
                            chrg_bchno = dr["sfno"].ToString(),//收费批次号
                            dise_codg = bzbm,//病种编码
                            rxno = dr["cfh"].ToString(),//处方号
                            rx_circ_flag = "",//外购处方标识 字典
                            fee_ocur_time = Convert.ToDateTime(dr["yysj"].ToString()).ToString("yyyyMMddhhmmss"),//费用时间
                            med_list_codg = dr["ybxmbh"].ToString(),//医疗中心编码
                            medins_list_codg = dr["yyxmbh"].ToString(),//医药机构目录编码
                            det_item_fee_sumamt = Decimal.Parse(dr["je"].ToString()),//明细项目费用总额
                            cnt = Decimal.Parse(dr["sl"].ToString()),//数量
                            pric = Decimal.Parse(dr["dj"].ToString()),//单价
                            sin_dos_dscr = "",//单次剂量描述
                            used_frqu_dscr = "",//使用频次描述
                            prd_days = 1,//周期天数
                            medc_way_dscr = "",//用药途径描述
                            bilg_dept_codg = ksno,//开单科室编码
                            bilg_dept_name = ksmc,//开单科室编码
                            bilg_dr_codg = ysbh,//开单医生编码
                            bilg_dr_name = ysmc,//开单医生名称
                            acord_dept_codg = ksno,//受单科室编码
                            acord_dept_name = ksmc,//受单科室名称
                            orders_dr_code = ysbh,//受单医生编码
                            orders_dr_name = ysmc,//受单医生名称
                            hosp_appr_flag = "1",//医院审批标识 字典
                            tcmdrug_used_way = "",//中药使用方式 字典
                            etip_flag = "",//外检标识 字典
                            etip_hosp_code = "",//外检医院编码
                            dscg_tkdrug_flag = "",//出院带药标识 字典
                            matn_fee_flag = "" //生育费用标志 字典
                        };
                        fees.Add(fee);
                    }

                    #endregion

                    //判断总费用与累计费用是否相等
                    if (Math.Abs(ghfy - ghfylj) > 1.0)
                        return new object[] { 0, 0, "挂号金额与医保结算金额相差" + Math.Abs(ghfy - ghfylj) + ",无法结算，请核实!" };


                    string data1 = JsonConvert.SerializeObject(fees);
                    //日志
                    WriteLog(sysdate + "|门诊挂号费用明细上传|");
                    WriteLog(sysdate + "|入参|" + data1.ToString());
                    string feeres = string.Empty;
                    //新医保接口
                    int i1 = YBServiceRequest("2204", data1, ref feeres);
                    if (i1 > 0)
                    {
                        List<OutpatientFeeOutParam> cfmxscfhList = JsonConvert.DeserializeObject<List<OutpatientFeeOutParam>>(feeres);
                        for (int m = 0; m < cfmxscfhList.Count; m++)
                        {
                            OutpatientFeeOutParam info = cfmxscfhList[m];
                            //出参:住院号|处方号|项目编号|项目名称|项目等级|收费类别|单价|数量|金额|处方日期|处方上传时间
                            DataRow dr1 = ds.Tables[0].Rows[m];
                            string jzlsh2 = jzlsh; //就诊流水号  
                            string ybghmc = dr1["yyxmmc"].ToString(); //项目名称
                            string xmdj = info.chrgitm_lv.Trim();   //项目等级
                            string sflb = info.med_chrgitm_type;   //收费类别
                            string dj2 = info.pric.ToString();    //单价
                            string sl2 = "1";    //数量 
                            string scsj = sysdate;  //处方上传时间

                            #region 回参的数据
                            string ybcfh = info.feedetl_sn;  //处方号
                            string ybcflsh = info.feedetl_sn;//处方流水号  新加
                            string yyrq = jssj;   //处方日期
                            string ybxmbm = dr["ybxmbh"].ToString(); //项目编号
                            string je2 = info.det_item_fee_sumamt.ToString().Trim();    //金额
                            string zfje = info.fulamt_ownpay_amt.ToString();//自费金额  新加
                            string cxjje = info.overlmt_amt.ToString();//超限价金额 新加
                            string zfsx = info.pric_uplmt_amt.ToString();//支付上限 新加
                            string memo = info.memo;//说明信息  新加
                            #endregion

                            List<string> liSQL = new List<string>();
                            if (Math.Round(Convert.ToDecimal(dj2), 2, MidpointRounding.AwayFromZero) == Math.Round(Convert.ToDecimal(info.det_item_fee_sumamt), 2, MidpointRounding.AwayFromZero))
                            {
                                //MessageBox.Show(li_dj[j] + "||" + Convert.ToDouble(dj2).ToString());
                                string Sql = string.Format(@"insert into ybcfmxscindr(jzlsh,jylsh,je,xm,kh,yysfxmbm,yysfxmmc,sysdate,ybjzlsh,ybcfh,sfxmzxbm,
                                                        sfxmzxmc,sfxmzl,sflb,dj,sl,cfrq,cfscsj,cxbz,zfje,remark,ybcflsh,cxjje,zfsx) values(
                                                        '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}',
                                                        '{11}','{12}','{13}','{14}','{15}','{16}','{17}','1','{18}','{19}','{20}','{21}','{22}')",
                                                        jzlsh, jzlsh, je2, xm, kh, ybxmbh, ybxmmc, sysdate, ybjzlsh, ybcfh, ybxmbm,
                                                        ybxmmc, xmdj, sflb, dj2, sl2, yyrq, scsj, zfje, memo, ybcflsh, cxjje, zfsx);
                                liSQL.Add(Sql);
                                WriteLog(sysdate + "  " + jzlsh + ybxmbh + "上传处方明细-->" + "|" + ybxmmc + "|" + feeres);
                            }
                            object[] obj = liSQL.ToArray();
                            obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);
                            if (obj[1].ToString().Equals("1"))
                            {
                                WriteLog(sysdate + "  上传处方明细|" + "");
                                //return new object[] { 0, 1, "上传处方明细成功！" };
                            }
                            else
                            {
                                WriteLog(sysdate + "  上传处方明细|保存本地数据失败|" + obj[2].ToString());
                                return new object[] { 0, 0, "上传处方明细｜保存本地数据失败｜" + obj[2].ToString() };
                            }
                        }

                        WriteLog(sysdate + $"门诊挂号费用明细{0}~{fees.Count}条上传成功！" + feeres);
                        // return new object[] { 0, 1, $"门诊挂号费用明细{0}~{feelist.Count}条上传成功！" };
                    }
                    else
                    {
                        RollBackMZFY(fees);
                        WriteLog(sysdate + $"门诊挂号费用明细{0}~{fees.Count}条上传失败！" + feeres);
                        return new object[] { 0, 0, $"门诊挂号费用明细{0}~{fees.Count}条上传失败！" + feeres };
                    }
                    #endregion



                    //判断总费用与累计费用是否相等
                    if (Math.Abs(ghfy - ghfylj) > 1.0)
                        return new object[] { 0, 0, "收费金额与医保结算金额相差" + Math.Abs(ghfy - ghfylj) + ",无法结算，请核实!" };

                    #region 费用结算
                    string Ywlx = "2207";
                    #region 老医保接口
                    //StringBuilder outData = new StringBuilder(10240);
                    //StringBuilder retMsg = new StringBuilder(1024);
                    //WriteLog(sysdate + "  " + jzlsh + " 进入门诊登记（挂号）收费结算...");
                    //WriteLog(sysdate + "  入参|" + inputData.ToString());
                    //int i = f_UserBargaingApply(Ywlx, inputData, outData, retMsg);

                    //WriteLog(i.ToString() + "|" + outData.ToString() + "|" + retMsg.ToString()); 
                    #endregion
                    string sfpcNo = jzlsh + "|" + System.DateTime.Now.ToString("yyyyMMddhhmmssfff");
                    string result = string.Empty;
                    #region 新医保接口 门诊挂号收费新
                    string data = "{\"psn_no\":\"{0}\",\"mdtrt_cert_type\":\"{1}\",\"mdtrt_cert_no\":\"{2}\",\"med_type\":\"{3}\",\"medfee_sumamt\":\"{4}\",\"psn_setlway\":\"{5}\",\"mdtrt_id\":\"{6}\",\"chrg_bchno\":\"{7}\",\"insutype\":\"{8}\",\"acct_used_flag\":\"{9}\",\"invono\":\"{10}\"}";
                    data = string.Format(data, bxh, "03", kh, "12", decimal.Parse(ghf), "01", jzlsh, sfpcNo, cardInfo.insuinfo.insutype, "0", invoiceNo);
                    WriteLog(sysdate + "  " + jzlsh + " 进入门诊登记（挂号）收费结算...");
                    WriteLog(sysdate + "  入参|" + data.ToString());
                    int i = YBServiceRequest(Ywlx, data, ref result);
                    #endregion

                    if (i > 0)
                    {
                        List<string> liSQL = new List<string>();
                        List<string> liDqbh = new List<string>();//赣州所有县地区编号
                        #region 赣州地区编号
                        //360701	【赣州市】
                        //360721	【赣县】
                        //360733	【会昌县】
                        //360723	【大余县】
                        //360734	【寻乌县】
                        //360782	【南康市】
                        //360730	【宁都县】
                        //360725	【崇义县】
                        //360726	【安远县】
                        //360732	【兴国县】
                        //360728	【定南县】
                        //360724	【上犹县】
                        //360735	【石城县】
                        //360722	【信丰县】
                        //360729	【全南县】
                        //360727	【龙南县】
                        //360781	【瑞金市】
                        //360731	【于都县】
                        //360702	【章贡区】
                        //360703    【开发区】
                        //360704    【蓉江新区】
                        //返回其他值 【异地就医】
                        liDqbh.Add("360701");
                        liDqbh.Add("360721");
                        liDqbh.Add("360733");
                        liDqbh.Add("360723");
                        liDqbh.Add("360734");
                        liDqbh.Add("360782");
                        liDqbh.Add("360730");
                        liDqbh.Add("360725");
                        liDqbh.Add("360726");
                        liDqbh.Add("360732");
                        liDqbh.Add("360728");
                        liDqbh.Add("360724");
                        liDqbh.Add("360735");
                        liDqbh.Add("360722");
                        liDqbh.Add("360729");
                        liDqbh.Add("360727");
                        liDqbh.Add("360781");
                        liDqbh.Add("360731");
                        liDqbh.Add("360702");
                        liDqbh.Add("360703");
                        liDqbh.Add("360704");
                        #endregion

                        #region 以前兴国出参
                        /*出参:保险号|姓名|卡号|地区编号|地区名称|出生日期|实际年龄|参保日期|个人身份|单位名称|
                     * 性别|医疗人员类别|卡状态|账户余额|门诊(住院)号|单据流水号|医疗类别|科室名称|本次看病次数|住院床号|入院日期|
                     * 入院时间|出院日期|出院时间|出院原因|医疗总费用|本次账户支付|本次现金支付|本次基金支付|大病基金支付|救助金额|
                     * 单位补充医保支付|离休干部单独统筹支付|甲类费用|乙类费用|丙类费用|自费费用|结算人|结算日期|结算时间|起付标准自付|
                     * 非医保自付|乙类药品自付|特检特治自付|进入统筹自付|进入大病自付|重大疾病范围内补偿金额|重大疾病范围外补偿金额|
                     * 医院负担金额|大病二次补偿|民政大病救助基金|政府兜底基金|其中公务员补助部分
                     */
                        //string[] str = outData.ToString().Split(';');
                        //string[] str2 = str[0].Split('|');
                        //string bxh2 = str2[0].Trim();       //保险号
                        //string xm2 = str2[1].Trim();        //姓名
                        //string kh2 = str2[2].Trim();        //卡号
                        //string dqbh2 = str2[3].Trim();      //地区编号
                        //string dqmc = str2[4].Trim();       //地区名称
                        //string csrq = str2[5].Trim();       //出生日期
                        //string sjnl = str2[6].Trim();       //实际年龄
                        //string cbrq = str2[7].Trim();       //参保日期
                        //string grsf = str2[8].Trim();       //个人身份
                        //string dwmc = str2[9].Trim();       //单位名称
                        //string xb = str2[10].Trim();        //性别
                        //string ylrylb = str2[11].Trim();    //医疗人员类别
                        //string kzt = str2[12].Trim();       //卡状态
                        //string zfye = str2[13].Trim();      //账户余额
                        //string jzlsh2 = str2[14].Trim();    //门诊(住院)号
                        //string jslsh = str2[15].Trim();     //单据流水号
                        //string yllb2 = str2[16].Trim();      //医疗类别
                        //string ksmc = str2[17].Trim();      //科室名称
                        //string zycs = str2[18].Trim();      //本次看病次数
                        //string zych = str2[19];      //住院床号
                        //string ryrq = str2[20].Trim();      //入院日期
                        //string rysj = str2[21].Trim();      //入院时间
                        //string cyrq = str2[22];      //出院日期
                        //string cysj = str2[23];      //出院时间
                        //string cyyy = str2[24];      //出院原因
                        //string ylfze = str2[25].Trim();     //医疗总费用
                        //string zhzf = str2[26].Trim();      //本次账户支付
                        //string xjzf = str2[27].Trim();      //本次现金支付
                        //string tcjjzf = str2[28].Trim();    //本次基金支付
                        //string dejjzf = str2[29].Trim();    //大病基金支付
                        //string mzjzfy = str2[30].Trim();    //救助金额
                        //string dwfdfy = str2[31].Trim();    //单位补充医保支付
                        //string lxgbddtczf = str2[32].Trim();//离休干部单独统筹支付
                        //string jlfy = str2[33].Trim();      //甲类费用
                        //string ylfy = str2[34].Trim();      //乙类费用
                        //string blfy = str2[35].Trim();      //丙类费用
                        //string zffy = str2[36].Trim();      //自费费用
                        //string jsr = str2[37].Trim();       //结算人
                        //string jsrq = str2[38].Trim();      //结算日期
                        //string jssj2 = str2[39].Trim();      //结算时间
                        //string qfbzfy = str2[40].Trim();    //起付标准自付
                        //string fybzf = str2[41].Trim();     //非医保自付
                        //string ylypzf = str2[42].Trim();    //乙类药品自付
                        //string tjtzzf = str2[43].Trim();    //特检特治自付
                        //string jrtcfy = str2[44].Trim();    //进入统筹自付
                        //string jrdebxfy = str2[45].Trim();  //进入大病自付
                        //string zdjbfwnbcje = str2[46].Trim(); //重大疾病范围内补偿金额
                        //string zdjbfwybcje = str2[47].Trim(); //重大疾病范围外补偿金额
                        //string yyfdje = str2[48].Trim(); //医院负担金额
                        //string dbecbc = str2[49].Trim(); //大病二次补偿
                        //string mzdbjzjj = str2[50].Trim(); //民政大病救助基金
                        //string zftdjj = str2[51].Trim(); //政府兜底基金
                        //string gwybzfy = str2[52].Trim();   //其中公务员补助部分

                        ////string zdjbfwnbcje = "0.00"; //重大疾病范围内补偿金额
                        ////string zdjbfwybcje = "0.00"; //重大疾病范围外补偿金额
                        ////string yyfdje = "0.00"; //医院负担金额
                        ////string dbecbc = "0.00"; //大病二次补偿
                        ////string mzdbjzjj = "0.00"; //民政大病救助基金
                        ////string zftdjj = "0.00"; //政府兜底基金
                        ////string gwybzfy = "0.00";   //其中公务员补助部分
                        //double bcjsqzhye = Convert.ToDouble(zhzf) + Convert.ToDouble(zfye);
                        //string jbr = CliUtils.fLoginUser;
                        #endregion

                        #region 出参 
                        ///新医保出参
                        ///
                        YBBalanceInfo ybinfo = JsonConvert.DeserializeObject<YBBalanceInfo>(result);

                        //string[] str = outData.ToString().Split(';');
                        //string[] str2 = str[0].Split('|');
                        string bxh2 = ybinfo.setlinfo.psn_no.Trim();       //保险号
                        string xm2 = ybinfo.setlinfo.psn_name.Trim();        //姓名
                        string kh2 = kh.Trim();        //卡号
                        string dqbh2 = dqbh.Trim();      //地区编号
                        string dqmc = "".Trim();       //地区名称
                        string csrq = ybinfo.setlinfo.brdy.Trim();       //出生日期
                        string sjnl = ybinfo.setlinfo.age.Trim();       //实际年龄
                        string cbrq = cardInfo.idetinfo.begntime.Trim();       //参保日期
                        string grsf = cardInfo.idetinfo.psn_type_lv.Trim();       //个人身份
                        string dwmc = cardInfo.insuinfo.emp_name.Trim();       //单位名称
                        string xb = ybinfo.setlinfo.gend.Trim();        //性别
                        string ylrylb = ybinfo.setlinfo.psn_type.Trim();    //医疗人员类别
                        string kzt = "";       //卡状态
                        string zfye = string.IsNullOrEmpty(ybinfo.setlinfo.balc) ? "0" : ybinfo.setlinfo.balc.Trim();     //账户余额
                        string jzlsh2 = ybinfo.setlinfo.mdtrt_id.Trim();    //门诊(住院)号
                        string jslsh = jzlsh.Trim();     //单据流水号
                        string yllb2 = ybinfo.setlinfo.med_type.Trim();      //医疗类别
                        //string ksmc = "".Trim();      //科室名称
                        string zycs = "".Trim();      //本次看病次数
                        string zych = "".Trim();      //住院床号
                        string ryrq = "".Trim();      //入院日期
                        string rysj = "".Trim();      //入院时间
                        string cyrq = "".Trim();      //出院日期
                        string cysj = "".Trim();      //出院时间
                        string cyyy = "".Trim();      //出院原因
                        string ylfze = string.IsNullOrEmpty(ybinfo.setlinfo.medfee_sumamt.ToString()) || ybinfo.setlinfo.medfee_sumamt.ToString() == null || ybinfo.setlinfo.medfee_sumamt.ToString() == "NULL" ? "0" : ybinfo.setlinfo.medfee_sumamt.ToString().Trim();     //医疗总费用
                        string zhzf = string.IsNullOrEmpty(ybinfo.setlinfo.acct_pay.ToString()) || ybinfo.setlinfo.acct_pay.ToString() == null || ybinfo.setlinfo.acct_pay.ToString() == "NULL" ? "0" : ybinfo.setlinfo.acct_pay.ToString().Trim();       //本次账户支付
                        string xjzf = string.IsNullOrEmpty(ybinfo.setlinfo.psn_cash_pay.ToString()) || ybinfo.setlinfo.psn_cash_pay.ToString() == null || ybinfo.setlinfo.psn_cash_pay.ToString() == "NULL" ? "0" : ybinfo.setlinfo.psn_cash_pay.ToString().Trim();      //本次现金支付
                        string tcjjzf = string.IsNullOrEmpty(ybinfo.setlinfo.fund_pay_sumamt.ToString()) || ybinfo.setlinfo.fund_pay_sumamt.ToString() == null || ybinfo.setlinfo.fund_pay_sumamt.ToString() == "NULL" ? "0" : ybinfo.setlinfo.fund_pay_sumamt.ToString().Trim();     //本次基金支付
                        string dejjzf = string.IsNullOrEmpty(ybinfo.setlinfo.hifmi_pay.ToString()) || ybinfo.setlinfo.hifmi_pay.ToString() == null || ybinfo.setlinfo.hifmi_pay.ToString() == "NULL" ? "0" : ybinfo.setlinfo.hifmi_pay.ToString().Trim();     //大病基金支付
                        //string mzjzfy = str2[30].Trim(); //民政救助金额
                        string dbecbc = "0";    //二次补偿金额
                        string dwfdfy = string.IsNullOrEmpty(ybinfo.setlinfo.hifes_pay.ToString()) || ybinfo.setlinfo.hifes_pay.ToString() == null || ybinfo.setlinfo.hifes_pay.ToString() == "NULL" ? "0" : ybinfo.setlinfo.hifes_pay.ToString().Trim();     //单位补充医保支付
                        string lxgbddtczf = "0";//离休干部单独统筹支付
                        string jlfy = "0";//甲类费用
                        string ylfy = "0";    //乙类费用
                        string blfy = "0";    //丙类费用
                        string zffy = string.IsNullOrEmpty(ybinfo.setlinfo.fulamt_ownpay_amt.ToString()) || ybinfo.setlinfo.fulamt_ownpay_amt.ToString() == null || ybinfo.setlinfo.fulamt_ownpay_amt.ToString() == "NULL" ? "0" : ybinfo.setlinfo.fulamt_ownpay_amt.ToString().Trim();     //自费费用
                        string jsr = CliUtils.fLoginUser.Trim();       //结算人
                        string jsrq = System.DateTime.Now.ToString("yyyy-MM-dd").Trim();      //结算日期
                        string jssj2 = sysdate.Trim();      //结算时间
                        string qfbzfy = string.IsNullOrEmpty(ybinfo.setlinfo.act_pay_dedc.ToString()) || ybinfo.setlinfo.act_pay_dedc.ToString() == null || ybinfo.setlinfo.act_pay_dedc.ToString() == "NULL" ? "0" : ybinfo.setlinfo.act_pay_dedc.ToString().Trim();   //起付标准自付
                        string fybzf = "0";     //非医保自付
                        string ylypzf = "0"; //乙类药品自付
                        string tjtzzf = "0";   //特检特治自付
                        string jrtcfy = "0";  //进入统筹自付
                        string jrdebxfy = "0"; //进入大病自付

                        string zdjbfwnbcje = "";//重大疾病范围内补偿金额(0)
                        string zdjbfwybcje = ""; //重大疾病范围外补偿金额(0)
                        string mzjzfy = ""; //民政救助金额
                        string gwynbfy = "";//公务员范围内补偿金额
                        string gwywbfy = "";//公务员范围外补偿金额
                        string yyfdje = ""; //医院负担金额(1)
                        string ygbtje = ""; //医改补贴金额
                        if (!liDqbh.Contains(dqbh2))
                        {
                            zdjbfwnbcje = "0"; //重大疾病范围内补偿金额(0)
                            zdjbfwybcje = "0"; //重大疾病范围外补偿金额(0)
                            mzjzfy = "0"; //民政救助金额
                            gwynbfy = "0";//公务员范围内补偿金额
                            gwywbfy = "0";//公务员范围外补偿金额
                            yyfdje = "0"; //医院负担金额(1)
                            ygbtje = "0";
                        }
                        else
                        {
                            //zdjbfwnbcje = string.IsNullOrEmpty(str2[46]) || str2[46] == null || str2[46] == "NULL" ? "0" : str2[46].Trim(); //重大疾病范围内补偿金额(0)
                            //zdjbfwybcje = string.IsNullOrEmpty(str2[47]) || str2[47] == null || str2[47] == "NULL" ? "0" : str2[47].Trim(); //重大疾病范围外补偿金额(0)
                            //mzjzfy = string.IsNullOrEmpty(str2[48]) || str2[48] == null || str2[48] == "NULL" ? "0" : str2[48].Trim(); //民政救助金额
                            //gwynbfy = string.IsNullOrEmpty(str2[49]) || str2[49] == null || str2[49] == "NULL" ? "0" : str2[49].Trim();//公务员范围内补偿金额
                            //gwywbfy = string.IsNullOrEmpty(str2[50]) || str2[50] == null || str2[50] == "NULL" ? "0" : str2[50].Trim();//公务员范围外补偿金额
                            //yyfdje = string.IsNullOrEmpty(str2[51]) || str2[51] == null || str2[51] == "NULL" ? "0" : str2[51].Trim(); //医院负担金额(1)
                            //ygbtje = string.IsNullOrEmpty(str2[52]) || str2[52] == null || str2[52] == "NULL" ? "0" : str2[52].Trim(); //医改补贴金额
                            zdjbfwnbcje = "0"; //重大疾病范围内补偿金额(0)
                            zdjbfwybcje = "0"; //重大疾病范围外补偿金额(0)
                            mzjzfy = "0"; //民政救助金额
                            gwynbfy = "0";//公务员范围内补偿金额
                            gwywbfy = "0";//公务员范围外补偿金额
                            yyfdje = "0"; //医院负担金额(1)
                            ygbtje = "0";//医改补贴金额
                        }

                        string mzdbjzjj = "0.00"; //民政大病救助基金(0)
                        string zftdjj = "0.00"; //政府兜底基金(1)
                        string gwybzfy = "0.00";   //其中公务员补助部分(1)
                        string jbr = CliUtils.fLoginUser;
                        // string dbecbc = str2[49].Trim(); //大病二次补偿(1)

                        //string zdjbfwnbcje = "0.00"; //重大疾病范围内补偿金额
                        //string zdjbfwybcje = "0.00"; //重大疾病范围外补偿金额
                        //string yyfdje = "0.00"; //医院负担金额
                        //string dbecbc = "0.00"; //大病二次补偿
                        //string mzdbjzjj = "0.00"; //民政大病救助基金
                        //string zftdjj = "0.00"; //政府兜底基金
                        //string gwybzfy = "0.00";   //其中公务员补助部分
                        //double bcjsqzhye = Convert.ToDouble(zhzf) + Convert.ToDouble(zfye);
                        double bcjsqzhye = Convert.ToDouble(zfye);
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
                     * 居民个人自付二次补偿金额|体检金额|生育基金支付|其他医保支付|
                     */
                        string Zbxje = (Convert.ToDecimal(ylfze) - Convert.ToDecimal(xjzf)).ToString();//总报销金额
                        gwybzfy = (Convert.ToDecimal(gwynbfy) + Convert.ToDecimal(gwywbfy)).ToString();//公务员内补+公务员外补

                        string strValue = ylfze + "|" + Zbxje + "|" + tcjjzf + "|" + dejjzf + "|" + zhzf + "|" + xjzf1 + "|" + gwybzfy + "|" + dwfdfy + "|" + zffy + "|" + dwfdfy + "|" +
                                        yyfdje + "|" + mzjzfy + "|0.00|0.00|0.00|0.00|" + qfbzfy + "|0.00|" + jrtcfy + "|0.00|" +
                                        0.00 + "|" + jrdebxfy + "|0.00|0.00|0.00|" + bcjsqzhye + "|0.00|0.00|0.00|0.00|" +
                                        0.00 + "|" + 0.00 + "|" + zycs + "|" + xm2 + "|" + jsrq + "|" + yllb2 + "|" + ylrylb + "|" + dqbh2 + "||" + jslsh + "||" +
                                        djhin + "|||1||" + dqbh2 + "|" + dbecbc + "|" + 0.00 + "|0.00|" + bxh2 + "||" +
                                        zftdjj + "|" + 0.00 + "|" + 0.00 + "|" + 0.00 + "|" + 0.00 + "|" + 0.00 + "|" + gwynbfy + "|" + gwywbfy + "|";

                        strSql = string.Format(@"insert into ybfyjsdr(jzlsh,jylsh,djhin,cyrq,cyyy,zhsybz,ztjsbz,jbr,xm,kh,
                                        grbh,jsrq,yllb,yldylb,jslsh,ylfze,zhzf,xjzf,tcjjzf,dejjzf,
                                        mzjzfy,dwfdfy,lxgbddtczf,jlfy,ylfy,blfy,zffy,qfbzfy,fybzf,ylypzf,
                                        tjtzzf,jrtczf,jrdbzf,zdjbfwnbcje,zdjbfwybcje,yyfdfy,ecbcje,mzdbjzje,czzcje,gwybzjjzf,
				                        sysdate,djh,ybjzlsh,zbxje,bcjsqzhye,gwynbfy,gwywbfy,bzbm,bzmc,ygbtje) values(
                                        '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}',
                                        '{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}',
                                        '{20}','{21}','{22}','{23}','{24}','{25}','{26}','{27}','{28}','{29}',
                                        '{30}','{31}','{32}','{33}','{34}','{35}','{36}','{37}','{38}','{39}',
                                        '{40}','{41}','{42}','{43}','{44}','{45}','{46}','{47}','{48}','{49}')",
                                        jzlsh, jslsh, djhin, cyrq, cyyy, 1, 0, jbr, xm2, kh2,
                                        bxh2, jsrq, yllb, ylrylb, jslsh, ylfze, zhzf, xjzf, tcjjzf, dejjzf,
                                        mzjzfy, dwfdfy, lxgbddtczf, jlfy, ylfy, blfy, zffy, qfbzfy, fybzf, ylypzf,
                                        tjtzzf, jrtcfy, jrdebxfy, zdjbfwnbcje, zdjbfwybcje, yyfdje, dbecbc, mzdbjzjj, zftdjj, gwybzfy,
                                        sysdate, djhin, jzlsh2, Zbxje, bcjsqzhye, gwynbfy, gwywbfy, bzbm == "NULL" ? "" : bzbm, bzmc == "NULL" ? "" : bzmc, ygbtje);
                        liSQL.Add(strSql);
                        /*
                         * 门诊(住院)号|单据流水号|项目编号|项目名称|项目等级|收费类别|单价|数量|金额|自付金额
                         */
                        //for (int j = 1; j < str.Length; j++)
                        //{
                        string jzlsh3 = jzlsh.Trim(); //门诊(住院)号
                        string djlsh = ybinfo.setlinfo.setl_id.Trim();  //单据流水号
                        string ybxmbh3 = ybxmbh.Trim(); //项目编号
                        string ybxmmc3 = ybxmmc.Trim(); //项目名称
                        string ybxmdj = "".Trim(); //项目等级
                        string ybsflb = "".Trim(); //收费类别
                        string dj3 = dj.ToString().Trim();     //单价
                        string sl3 = sl.ToString().Trim();     //数量
                        string je3 = je.ToString().Trim();     //金额
                        string zfje = ybinfo.setlinfo.fulamt_ownpay_amt.ToString().Trim();   //自付金额
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
                        //   }

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
                        WriteLog(sysdate + "  " + jzlsh + " 进入门诊登记（挂号）收费结算失败|" + result.ToString());
                        //门诊登记(挂号)撤销
                        object[] objParam2 = new object[] { jzlsh };
                        YBMZDJCX(objParam2);
                        return new object[] { 0, 0, "门诊登记（挂号）收费结算失败|" + result.ToString() };
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

        #region 门诊登记（挂号）收费撤销（新医保接口）
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

        #region 门诊收费预结算（新医保接口）
        public static object[] YBMZSFYJS(object[] objParam)
        {
            string sysdate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            try
            {
                string Ywlx = "2206";
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
                WriteLog(sysdate + "  " + jzlsh + " 进入门诊收费预结算...分解入参");
                string djh = "0000";    //单据号（发票号)
                string sfdypj = "False";                //是否打印票据
                double sfje2 = 0.0000; //金额 
                string ybjzlsh = "";   //医保就诊流水号
                string bxh = "";
                string xm = "";
                string kh = "";
                string dgysxm = ""; //开方医生(定岗医生)
                string sfzh = "";//身份证号

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

                string jzlsh1 = "";
                if (string.IsNullOrEmpty(bzbh))

                    jzlsh1 = "MZ" + yllb + jzlsh;

                else
                    jzlsh1 = "MZ" + yllb + jzlsh + bzbh;

                //  string jzlsh1 = "MZ" + yllb + bzbh + jzlsh;

                #region 是否门诊医保挂号
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
                sfzh = ds.Tables[0].Rows[0]["sfzh"].ToString();
                dsinfo = new DataSet();
                dsinfo = ds;
                #endregion
                WriteLog(sysdate + "  " + jzlsh + " 进入门诊收费预结算...判断医保登记");
                #region 普通门诊不需要输入病种
                if (yllb.Equals("11") && string.IsNullOrEmpty(bzbh))
                {
                    bzbh = "NULL";
                    bzmc = "NULL";
                }
                #endregion           

                #region 获取定岗医生信息
                strSql = string.Format(@"select dgysbm from ybdgyszd where ysbm='{0}'", cfysdm);
                ds.Tables.Clear();
                ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                if (ds.Tables[0].Rows.Count == 0)
                    dgysxm = "D360703015271";
                else
                    dgysxm = ds.Tables[0].Rows[0]["dgysbm"].ToString();
                cfysxm = dgysxm;
                #endregion
                WriteLog(sysdate + "  " + jzlsh + " 进入门诊收费预结算...获取定岗医生");
                #region 20200922 主动发送读卡交易
                strSql = string.Format(@"select kh from YBICKXX where grbh='{0}' ", bxh);
                //ds.Tables.Clear();
                ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    kh = ds.Tables[0].Rows[0]["kh"].ToString();
                }
                #endregion
                WriteLog(sysdate + "  " + jzlsh + " 进入门诊收费预结算...获取KH");
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


                #region 费用明细上传
                object[] fymxRes = YBMZFYMXSC(objParam);

                if (fymxRes[1].ToString() == "0")
                {
                    WriteLog(sysdate + "费用明细上传|" + "费用明细上传失败！" + fymxRes[2].ToString());
                    return new object[] { 0, 0, "费用明细上传失败！" + fymxRes[2].ToString() };
                }
                #endregion

                #region 医保获取个人信息
                ReadCardInfo cardInfo = GetYbCardInfo(new object[] { "02", sfzh, "", "", "1", sfzh, xm });
                if (cardInfo == null)
                {
                    return new object[] { 0, 0, "获取医保个人信息出错" };
                }
                #endregion
                #region 老医保接口 by hjw

                //StringBuilder outData = new StringBuilder(102400);
                //StringBuilder retMsg = new StringBuilder(102400);

                //WriteLog(sysdate + "  " + jzlsh + " 进入门诊收费预结算...");
                //WriteLog(sysdate + "  入参|" + inputData.ToString());
                //int i = f_UserBargaingApply(Ywlx, inputData, outData, retMsg); 
                #endregion

                string res = string.Empty;
                string sfpcNo = jzlsh + "|" + System.DateTime.Now.ToString("yyyyMMddhhmmssfff");
                #region 新医保接口 门诊预结算
                string data = "{\"psn_no\":\"" + bxh + "\",\"mdtrt_cert_type\":\"" + "02" + "\",\"mdtrt_cert_no\":\"" + sfzh + "\",\"card_sn\":\"\",\"psn_cert_type\":\"01\",\"certno\":\"" + sfzh + "\",\"psn_type\":\"" + cardInfo.insuinfo.psn_type + "\",\"psn_name\":\"" + xm + "\",\"medfee_sumamt\":\"" + sfje2 + "\",\"psn_setlway\":\"01\",\"mdtrt_id\":\"" + jzlsh + "\",\"chrg_bchno\":\"" + djh + "\",\"acct_used_flag\":\"1\",\"insutype\":\"" + cardInfo.insuinfo.insutype + "\"}";

                //string data = "{\"psn_no\":\"" + bxh + "\",\"mdtrt_cert_type\":\"" + "03" + "\",\"mdtrt_cert_no\":\"" + kh + "\",\"med_type\":\"12\",\"medfee_sumamt\":\"" + sfje2 + "\",\"psn_setlway\":\"01\",\"mdtrt_id\":\"" + jzlsh + "\",\"chrg_bchno\":\"" + sfpcNo + "\",\"insutype\":\"" + cardInfo.insuinfo.insutype + "\",\"acct_used_flag\":\"" + "0" + "\",\"invono\":\"" + djh + "\"}";
                //data = string.Format(data, bxh, "03", kh, "12", sfje2, "01", jzlsh, sfpcNo, cardInfo.insuinfo.insutype, "0", djh);
                WriteLog(sysdate + "  " + jzlsh + " 进入门诊收费预结算...");
                WriteLog(sysdate + "  入参|" + data.ToString());
                int i = YBServiceRequest(Ywlx, data, ref res);
                #endregion
                YBBalanceInfo ybinfo = new YBBalanceInfo();
                if (i > 0)
                {
                    List<string> liDqbh = new List<string>();//赣州所有县地区编号

                    #region 赣州地区编号
                    //360701	【赣州市】
                    //360721	【赣县】
                    //360733	【会昌县】
                    //360723	【大余县】
                    //360734	【寻乌县】
                    //360782	【南康市】
                    //360730	【宁都县】
                    //360725	【崇义县】
                    //360726	【安远县】
                    //360732	【兴国县】
                    //360728	【定南县】
                    //360724	【上犹县】
                    //360735	【石城县】
                    //360722	【信丰县】
                    //360729	【全南县】
                    //360727	【龙南县】
                    //360781	【瑞金市】
                    //360731	【于都县】
                    //360702	【章贡区】
                    //360703    【开发区】
                    //360704    【蓉江新区】
                    //返回其他值 【异地就医】
                    liDqbh.Add("360701");
                    liDqbh.Add("360721");
                    liDqbh.Add("360733");
                    liDqbh.Add("360723");
                    liDqbh.Add("360734");
                    liDqbh.Add("360782");
                    liDqbh.Add("360730");
                    liDqbh.Add("360725");
                    liDqbh.Add("360726");
                    liDqbh.Add("360732");
                    liDqbh.Add("360728");
                    liDqbh.Add("360724");
                    liDqbh.Add("360735");
                    liDqbh.Add("360722");
                    liDqbh.Add("360729");
                    liDqbh.Add("360727");
                    liDqbh.Add("360781");
                    liDqbh.Add("360731");
                    liDqbh.Add("360702");
                    liDqbh.Add("360703");
                    liDqbh.Add("360704");
                    #endregion

                    #region 出参赋值
                    ybinfo = JsonConvert.DeserializeObject<YBBalanceInfo>(res);
                    #endregion

                    #region 出参
                    /*出参:保险号|姓名|卡号|地区编号|地区名称|出生日期|实际年龄|参保日期|个人身份|单位名称|
                     * 性别|医疗人员类别|卡状态|账户余额|门诊(住院)号|单据流水号|医疗类别|科室名称|本次看病次数|住院床号|入院日期|
                     * 入院时间|出院日期|出院时间|出院原因|医疗总费用|本次账户支付|本次现金支付|本次基金支付|大病基金支付|二次补偿金额|
                     * 单位补充医保支付|离休干部单独统筹支付|甲类费用|乙类费用|丙类费用|自费费用|结算人|结算日期|结算时间|起付标准自付|
                     * 非医保自付|乙类药品自付|特检特治自付|进入统筹自付|进入大病自付|重大疾病范围内补偿金额|重大疾病范围外补偿金额|
                     * 医院负担金额|大病二次补偿|民政大病救助基金|政府兜底基金|其中公务员补助部分
                 * 0730397447|陶子恒|36082400902240|360824|新干县|20100217|8|20091201|07579728|陶涛|男|居民-未成年居民|正常|66.72|68999784|72485907|门诊慢性病二类|儿科|1|NULL|20180306|1655|NULL|NULL|NULL|300.88|181.17|119.71|0|0|0|0|0|181.17|0|0|119.71|陶涛|20180306|1743|0|119.71|0|0|0|0
                     */
                    string bxh2 = ybinfo.setlinfo.psn_no.Trim();       //保险号
                    string xm2 = ybinfo.setlinfo.psn_name.Trim();        //姓名
                    string kh2 = kh.Trim();        //卡号
                    string dqbh2 = dqbh.Trim();      //地区编号
                    string dqmc = "".Trim();       //地区名称
                    string csrq = ybinfo.setlinfo.brdy.Trim();       //出生日期
                    string sjnl = ybinfo.setlinfo.age.Trim();       //实际年龄
                    string cbrq = cardInfo.idetinfo.begntime.Trim();       //参保日期
                    string grsf = cardInfo.idetinfo.psn_type_lv.Trim();       //个人身份
                    string dwmc = cardInfo.insuinfo.emp_name.Trim();       //单位名称
                    string xb = ybinfo.setlinfo.gend.Trim();        //性别
                    string ylrylb = ybinfo.setlinfo.psn_type.Trim();    //医疗人员类别
                    string kzt = "";       //卡状态
                    string zfye = string.IsNullOrEmpty(ybinfo.setlinfo.balc) ? "0" : ybinfo.setlinfo.balc.Trim();     //账户余额
                    string jzlsh2 = ybinfo.setlinfo.mdtrt_id.Trim();    //门诊(住院)号
                    string jslsh = jzlsh.Trim();     //单据流水号
                    string yllb2 = ybinfo.setlinfo.med_type.Trim();      //医疗类别
                    string ksmc = "".Trim();      //科室名称
                    string zycs = "".Trim();      //本次看病次数
                    string zych = "".Trim();      //住院床号
                    string ryrq = "".Trim();      //入院日期
                    string rysj = "".Trim();      //入院时间
                    string cyrq = "".Trim();      //出院日期
                    string cysj = "".Trim();      //出院时间
                    string cyyy = "".Trim();      //出院原因
                    string ylfze = string.IsNullOrEmpty(ybinfo.setlinfo.medfee_sumamt.ToString()) || ybinfo.setlinfo.medfee_sumamt.ToString() == null || ybinfo.setlinfo.medfee_sumamt.ToString() == "NULL" ? "0" : ybinfo.setlinfo.medfee_sumamt.ToString().Trim();     //医疗总费用
                    string zhzf = string.IsNullOrEmpty(ybinfo.setlinfo.acct_pay.ToString()) || ybinfo.setlinfo.acct_pay.ToString() == null || ybinfo.setlinfo.acct_pay.ToString() == "NULL" ? "0" : ybinfo.setlinfo.acct_pay.ToString().Trim();       //本次账户支付
                    string xjzf = string.IsNullOrEmpty(ybinfo.setlinfo.psn_cash_pay.ToString()) || ybinfo.setlinfo.psn_cash_pay.ToString() == null || ybinfo.setlinfo.psn_cash_pay.ToString() == "NULL" ? "0" : ybinfo.setlinfo.psn_cash_pay.ToString().Trim();      //本次现金支付
                    string tcjjzf = string.IsNullOrEmpty(ybinfo.setlinfo.fund_pay_sumamt.ToString()) || ybinfo.setlinfo.fund_pay_sumamt.ToString() == null || ybinfo.setlinfo.fund_pay_sumamt.ToString() == "NULL" ? "0" : ybinfo.setlinfo.fund_pay_sumamt.ToString().Trim();     //本次基金支付
                    string dejjzf = string.IsNullOrEmpty(ybinfo.setlinfo.hifmi_pay.ToString()) || ybinfo.setlinfo.hifmi_pay.ToString() == null || ybinfo.setlinfo.hifmi_pay.ToString() == "NULL" ? "0" : ybinfo.setlinfo.hifmi_pay.ToString().Trim();     //大病基金支付
                                                                                                                                                                                                                                                          //string mzjzfy = str2[30].Trim(); //民政救助金额
                    string dbecbc = "0";    //二次补偿金额
                    string dwfdfy = string.IsNullOrEmpty(ybinfo.setlinfo.hifes_pay.ToString()) || ybinfo.setlinfo.hifes_pay.ToString() == null || ybinfo.setlinfo.hifes_pay.ToString() == "NULL" ? "0" : ybinfo.setlinfo.hifes_pay.ToString().Trim();     //单位补充医保支付
                    string lxgbddtczf = "0";//离休干部单独统筹支付
                    string jlfy = "0";//甲类费用
                    string ylfy = "0";    //乙类费用
                    string blfy = "0";    //丙类费用
                    string zffy = string.IsNullOrEmpty(ybinfo.setlinfo.fulamt_ownpay_amt.ToString()) || ybinfo.setlinfo.fulamt_ownpay_amt.ToString() == null || ybinfo.setlinfo.fulamt_ownpay_amt.ToString() == "NULL" ? "0" : ybinfo.setlinfo.fulamt_ownpay_amt.ToString().Trim();     //自费费用
                    string jsr = CliUtils.fLoginUser.Trim();       //结算人
                    string jsrq = System.DateTime.Now.ToString("yyyy-MM-dd").Trim();      //结算日期
                    string jssj2 = sysdate.Trim();      //结算时间
                    string qfbzfy = string.IsNullOrEmpty(ybinfo.setlinfo.act_pay_dedc.ToString()) || ybinfo.setlinfo.act_pay_dedc.ToString() == null || ybinfo.setlinfo.act_pay_dedc.ToString() == "NULL" ? "0" : ybinfo.setlinfo.act_pay_dedc.ToString().Trim();   //起付标准自付
                    string fybzf = "0";     //非医保自付
                    string ylypzf = "0"; //乙类药品自付
                    string tjtzzf = "0";   //特检特治自付
                    string jrtcfy = "0";  //进入统筹自付
                    string jrdebxfy = "0"; //进入大病自付

                    string zdjbfwnbcje = "";//重大疾病范围内补偿金额(0)
                    string zdjbfwybcje = ""; //重大疾病范围外补偿金额(0)
                    string mzjzfy = ""; //民政救助金额
                    string gwynbfy = "";//公务员范围内补偿金额
                    string gwywbfy = "";//公务员范围外补偿金额
                    string yyfdje = ""; //医院负担金额(1)
                    string ygbtje = ""; //医改补贴金额
                    if (!liDqbh.Contains(dqbh2))
                    {
                        zdjbfwnbcje = "0"; //重大疾病范围内补偿金额(0)
                        zdjbfwybcje = "0"; //重大疾病范围外补偿金额(0)
                        mzjzfy = "0"; //民政救助金额
                        gwynbfy = "0";//公务员范围内补偿金额
                        gwywbfy = "0";//公务员范围外补偿金额
                        yyfdje = "0"; //医院负担金额(1)
                        ygbtje = "0";
                    }
                    else
                    {
                        //zdjbfwnbcje = string.IsNullOrEmpty(str2[46]) || str2[46] == null || str2[46] == "NULL" ? "0" : str2[46].Trim(); //重大疾病范围内补偿金额(0)
                        //zdjbfwybcje = string.IsNullOrEmpty(str2[47]) || str2[47] == null || str2[47] == "NULL" ? "0" : str2[47].Trim(); //重大疾病范围外补偿金额(0)
                        //mzjzfy = string.IsNullOrEmpty(str2[48]) || str2[48] == null || str2[48] == "NULL" ? "0" : str2[48].Trim(); //民政救助金额
                        //gwynbfy = string.IsNullOrEmpty(str2[49]) || str2[49] == null || str2[49] == "NULL" ? "0" : str2[49].Trim();//公务员范围内补偿金额
                        //gwywbfy = string.IsNullOrEmpty(str2[50]) || str2[50] == null || str2[50] == "NULL" ? "0" : str2[50].Trim();//公务员范围外补偿金额
                        //yyfdje = string.IsNullOrEmpty(str2[51]) || str2[51] == null || str2[51] == "NULL" ? "0" : str2[51].Trim(); //医院负担金额(1)
                        //ygbtje = string.IsNullOrEmpty(str2[52]) || str2[52] == null || str2[52] == "NULL" ? "0" : str2[52].Trim(); //医改补贴金额
                        zdjbfwnbcje = "0"; //重大疾病范围内补偿金额(0)
                        zdjbfwybcje = "0"; //重大疾病范围外补偿金额(0)
                        mzjzfy = "0"; //民政救助金额
                        gwynbfy = "0";//公务员范围内补偿金额
                        gwywbfy = "0";//公务员范围外补偿金额
                        yyfdje = "0"; //医院负担金额(1)
                        ygbtje = "0";//医改补贴金额
                    }

                    string mzdbjzjj = "0.00"; //民政大病救助基金(0)
                    string zftdjj = "0.00"; //政府兜底基金(1)
                    string gwybzfy = "0.00";   //其中公务员补助部分(1)
                    string jbr = CliUtils.fLoginUser;
                    // string dbecbc = str2[49].Trim(); //大病二次补偿(1)

                    //string zdjbfwnbcje = "0.00"; //重大疾病范围内补偿金额
                    //string zdjbfwybcje = "0.00"; //重大疾病范围外补偿金额
                    //string yyfdje = "0.00"; //医院负担金额
                    //string dbecbc = "0.00"; //大病二次补偿
                    //string mzdbjzjj = "0.00"; //民政大病救助基金
                    //string zftdjj = "0.00"; //政府兜底基金
                    //string gwybzfy = "0.00";   //其中公务员补助部分
                    //double bcjsqzhye = Convert.ToDouble(zhzf) + Convert.ToDouble(zfye);
                    double bcjsqzhye = Convert.ToDouble(zfye);
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
                     * 居民个人自付二次补偿金额|体检金额|生育基金支付|公务员内补|公务员外补
                     */
                    string Zbxje = (Convert.ToDecimal(ylfze) - Convert.ToDecimal(xjzf)).ToString();//总报销金额
                                                                                                   //  gwybzfy = (Convert.ToDecimal(gwynbfy) + Convert.ToDecimal(gwywbfy)).ToString();//公务员内补+公务员外补
                    gwybzfy = (Convert.ToDecimal(gwynbfy) + Convert.ToDecimal(gwywbfy)).ToString();//公务员内补+公务员外补
                    string strValue = ylfze + "|" + Zbxje + "|" + tcjjzf + "|" + dejjzf + "|" + zhzf + "|" + xjzf1 + "|" + gwybzfy + "|" + dwfdfy + "|" + zffy + "|" + dwfdfy + "|" +
                                    yyfdje + "|" + mzjzfy + "|0.00|0.00|0.00|0.00|" + qfbzfy + "|0.00|" + jrtcfy + "|0.00|" +
                                    0.00 + "|" + jrdebxfy + "|0.00|0.00|0.00|" + bcjsqzhye + "|0.00|0.00|0.00|0.00|" +
                                    0.00 + "|" + 0.00 + "|" + zycs + "|" + xm2 + "|" + jsrq + "|" + yllb2 + "|" + ylrylb + "|" + dqbh2 + "||" + jslsh + "||" +
                                    "0000" + "|||1||" + dqbh2 + "|" + dbecbc + "|" + 0.00 + "|0.00|" + bxh2 + "||" +
                                    zftdjj + "|" + 0.00 + "|" + 0.00 + "|" + 0.00 + "|" + 0.00 + "|" + 0.00 + "|" + gwynbfy + "|" + gwywbfy + "|"; ;
                    WriteLog(sysdate + "  " + jzlsh + " 进入门诊收费预结算成功|" + strValue);

                    //项目明细显示
                    Frm_ybfymxNK fb = new Frm_ybfymxNK();
                    fb.dataGridView.AutoGenerateColumns = false;
                    fb.dataGridView.DataSource = ds.Tables[0];
                    fb.StartPosition = FormStartPosition.CenterParent;
                    fb.ShowDialog();

                    //返回数据显示
                    Frm_myjsjeNK fyjs = new Frm_myjsjeNK();
                    fyjs.zfyje.Text = ylfze; //医疗总费用
                    fyjs.zfje.Text = zhzf;//个人账户支付
                    fyjs.tczfje.Text = jrtcfy;//进入统筹自付
                    fyjs.tjtzzfje.Text = tjtzzf;//特检特治自付
                    fyjs.ylgrfdje.Text = ylypzf;//乙类个人负担
                    fyjs.jlje.Text = jlfy;//甲类费用
                    fyjs.qfbzzfje.Text = qfbzfy;//起付标准自付
                    fyjs.fybzfje.Text = fybzf;//非医保自付
                    fyjs.lxgbddtcje.Text = lxgbddtczf;//离休干部单独统筹支付
                    fyjs.zdjbnbje.Text = zdjbfwnbcje;//重大疾病范围内补金额
                    fyjs.yyfdje.Text = yyfdje;//医院负担金额
                    fyjs.mzdbjzje.Text = mzdbjzjj;//民政大病救助基金
                    fyjs.mzjzje.Text = mzjzfy;//救助金额

                    fyjs.grxjje.Text = xjzf;//个人现金支付
                    fyjs.zffyje.Text = zffy;//自费费用
                    fyjs.dbjjzfje.Text = dejjzf;//大病基金支付
                    fyjs.dwbcybje.Text = dwfdfy;//单位补充医保支付
                    fyjs.ylje.Text = ylfy;//乙类费用
                    fyjs.grjjje.Text = tcjjzf;//个人基金支付
                    fyjs.blje.Text = blfy;//丙类费用
                    fyjs.dbzfje.Text = jrdebxfy;//进入大病自付
                    fyjs.zdjbwbje.Text = zdjbfwybcje;//重大疾病范围外补金额
                    fyjs.dbecbcje.Text = dbecbc;//大病二次补偿
                    fyjs.zfddje.Text = zftdjj;//政府兜底基金
                    fyjs.gwynbfy.Text = gwynbfy;//公务员内补金额
                    fyjs.gwywbfy.Text = gwynbfy;//公务员外补金额
                    fyjs.ygbtje.Text = ygbtje;
                    fyjs.StartPosition = FormStartPosition.CenterParent;
                    fyjs.ShowDialog();
                    return new object[] { 0, 1, strValue };
                }
                else
                {
                    RollBackMZFY(feelists);
                    WriteLog(sysdate + "  " + jzlsh + " 进入门诊收费预结算失败|" + res.ToString());
                    return new object[] { 0, 0, "门诊收费预结算失败|" + res.ToString() };
                }
            }
            catch (Exception ex)
            {
                RollBackMZFY(feelists);
                WriteLog(sysdate + "  门诊收费预结算异常|" + ex.Message);
                return new object[] { 0, 0, "门诊收费预结算异常|" + ex.Message };
            }
        }
        #endregion

        #region APP门诊收费预结算（新医保接口）
        public static object[] APPYBMZSFYJS(object[] objParam)
        {
            string sysdate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            try
            {
                string Ywlx = "2206";
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
                string sfzh = "";//身份证号
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

                string jzlsh1 = "";
                if (string.IsNullOrEmpty(bzbh))

                    jzlsh1 = "MZ" + yllb + jzlsh;

                else
                    jzlsh1 = "MZ" + yllb + jzlsh + bzbh;

                //  string jzlsh1 = "MZ" + yllb + bzbh + jzlsh;

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
                sfzh = ds.Tables[0].Rows[0]["sfzh"].ToString();
                #endregion

                #region 普通门诊不需要输入病种
                if (yllb.Equals("11") && string.IsNullOrEmpty(bzbh))
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
                strSql = string.Format(@"select y.ybxmbh, y.ybxmmc, convert(decimal(18,4),m.dj) as dj, sum(m.sl) sl, sum(m.je) je, m.yyxmbh, m.yyxmmc, m.cfh,y.sfxmdj,y.gg from 
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
                                        group by y.ybxmbh, y.ybxmmc, m.dj, m.yyxmbh, m.yyxmmc, y.sfxmzldm, y.sflbdm, m.cfh,y.sfxmdj,y.gg", jzlsh, cfhs);
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

                #region 医保获取个人信息
                ReadCardInfo cardInfo = GetYbCardInfo(new object[] { "02", sfzh, "", "", "1", sfzh, xm });
                if (cardInfo == null)
                {
                    return new object[] { 0, 0, "获取医保个人信息出错" };
                }
                #endregion

                #region 老医保接口
                //StringBuilder outData = new StringBuilder(102400);
                //StringBuilder retMsg = new StringBuilder(102400);

                //WriteLog(sysdate + "  " + jzlsh + " 进入门诊收费预结算...");
                //WriteLog(sysdate + "  入参|" + inputData.ToString());
                //int i = f_UserBargaingApply(Ywlx, inputData, outData, retMsg);
                #endregion
                string res = string.Empty;
                string sfpcNo = jzlsh + "|" + System.DateTime.Now.ToString("yyyyMMddHHmmssfff");
                #region 新医保接口 APP门诊预结算  by  hjw
                string data = "{\"psn_no\":\"{0}\",\"mdtrt_cert_type\":\"{1}\",\"mdtrt_cert_no\":\"{2}\",\"med_type\":\"{3}\",\"medfee_sumamt\":\"{4}\",\"psn_setlway\":\"{5}\",\"mdtrt_id\":\"{6}\",\"chrg_bchno\":\"{7}\",\"insutype\":\"{8}\",\"acct_used_flag\":\"{9}\",\"invono\":\"{10}\"}";
                data = string.Format(data, bxh, "03", kh, "12", sfje2, "01", jzlsh, sfpcNo, cardInfo.insuinfo.insutype, "0", djh);
                WriteLog(sysdate + "  " + jzlsh + " 进入App门诊收费预结算...");
                WriteLog(sysdate + "  入参|" + data.ToString());
                int i = YBServiceRequest(Ywlx, data, ref res);

                #endregion
                if (i > 0)
                {
                    List<string> liDqbh = new List<string>();//赣州所有县地区编号

                    #region 赣州地区编号
                    //360701	【赣州市】
                    //360721	【赣县】
                    //360733	【会昌县】
                    //360723	【大余县】
                    //360734	【寻乌县】
                    //360782	【南康市】
                    //360730	【宁都县】
                    //360725	【崇义县】
                    //360726	【安远县】
                    //360732	【兴国县】
                    //360728	【定南县】
                    //360724	【上犹县】
                    //360735	【石城县】
                    //360722	【信丰县】
                    //360729	【全南县】
                    //360727	【龙南县】
                    //360781	【瑞金市】
                    //360731	【于都县】
                    //360702	【章贡区】
                    //360703    【开发区】
                    //360704    【蓉江新区】
                    //返回其他值 【异地就医】
                    liDqbh.Add("360701");
                    liDqbh.Add("360721");
                    liDqbh.Add("360733");
                    liDqbh.Add("360723");
                    liDqbh.Add("360734");
                    liDqbh.Add("360782");
                    liDqbh.Add("360730");
                    liDqbh.Add("360725");
                    liDqbh.Add("360726");
                    liDqbh.Add("360732");
                    liDqbh.Add("360728");
                    liDqbh.Add("360724");
                    liDqbh.Add("360735");
                    liDqbh.Add("360722");
                    liDqbh.Add("360729");
                    liDqbh.Add("360727");
                    liDqbh.Add("360781");
                    liDqbh.Add("360731");
                    liDqbh.Add("360702");
                    liDqbh.Add("360703");
                    liDqbh.Add("360704");
                    #endregion

                    #region 出参
                    /*出参:保险号|姓名|卡号|地区编号|地区名称|出生日期|实际年龄|参保日期|个人身份|单位名称|
                     * 性别|医疗人员类别|卡状态|账户余额|门诊(住院)号|单据流水号|医疗类别|科室名称|本次看病次数|住院床号|入院日期|
                     * 入院时间|出院日期|出院时间|出院原因|医疗总费用|本次账户支付|本次现金支付|本次基金支付|大病基金支付|二次补偿金额|
                     * 单位补充医保支付|离休干部单独统筹支付|甲类费用|乙类费用|丙类费用|自费费用|结算人|结算日期|结算时间|起付标准自付|
                     * 非医保自付|乙类药品自付|特检特治自付|进入统筹自付|进入大病自付|重大疾病范围内补偿金额|重大疾病范围外补偿金额|
                     * 医院负担金额|大病二次补偿|民政大病救助基金|政府兜底基金|其中公务员补助部分
                 * 0730397447|陶子恒|36082400902240|360824|新干县|20100217|8|20091201|07579728|陶涛|男|居民-未成年居民|正常|66.72|68999784|72485907|门诊慢性病二类|儿科|1|NULL|20180306|1655|NULL|NULL|NULL|300.88|181.17|119.71|0|0|0|0|0|181.17|0|0|119.71|陶涛|20180306|1743|0|119.71|0|0|0|0
                     */
                    YBBalanceInfo ybinfo = JsonConvert.DeserializeObject<YBBalanceInfo>(res);
                    string bxh2 = ybinfo.setlinfo.psn_no.Trim();       //保险号
                    string xm2 = ybinfo.setlinfo.psn_name.Trim();        //姓名
                    string kh2 = kh.Trim();        //卡号
                    string dqbh2 = dqbh.Trim();      //地区编号
                    string dqmc = "".Trim();       //地区名称
                    string csrq = ybinfo.setlinfo.brdy.Trim();       //出生日期
                    string sjnl = ybinfo.setlinfo.age.Trim();       //实际年龄
                    string cbrq = cardInfo.idetinfo.begntime.Trim();       //参保日期
                    string grsf = cardInfo.idetinfo.psn_type_lv.Trim();       //个人身份
                    string dwmc = cardInfo.insuinfo.emp_name.Trim();       //单位名称
                    string xb = ybinfo.setlinfo.gend.Trim();        //性别
                    string ylrylb = ybinfo.setlinfo.psn_type.Trim();    //医疗人员类别
                    string kzt = "";       //卡状态
                    string zfye = string.IsNullOrEmpty(ybinfo.setlinfo.balc) ? "0" : ybinfo.setlinfo.balc.Trim();     //账户余额
                    string jzlsh2 = ybinfo.setlinfo.mdtrt_id.Trim();    //门诊(住院)号
                    string jslsh = jzlsh.Trim();     //单据流水号
                    string yllb2 = ybinfo.setlinfo.med_type.Trim();      //医疗类别
                    string ksmc = "".Trim();      //科室名称
                    string zycs = "".Trim();      //本次看病次数
                    string zych = "".Trim();      //住院床号
                    string ryrq = "".Trim();      //入院日期
                    string rysj = "".Trim();      //入院时间
                    string cyrq = "".Trim();      //出院日期
                    string cysj = "".Trim();      //出院时间
                    string cyyy = "".Trim();      //出院原因
                    string ylfze = string.IsNullOrEmpty(ybinfo.setlinfo.medfee_sumamt.ToString()) || ybinfo.setlinfo.medfee_sumamt.ToString() == null || ybinfo.setlinfo.medfee_sumamt.ToString() == "NULL" ? "0" : ybinfo.setlinfo.medfee_sumamt.ToString().Trim();     //医疗总费用
                    string zhzf = string.IsNullOrEmpty(ybinfo.setlinfo.acct_pay.ToString()) || ybinfo.setlinfo.acct_pay.ToString() == null || ybinfo.setlinfo.acct_pay.ToString() == "NULL" ? "0" : ybinfo.setlinfo.acct_pay.ToString().Trim();       //本次账户支付
                    string xjzf = string.IsNullOrEmpty(ybinfo.setlinfo.psn_cash_pay.ToString()) || ybinfo.setlinfo.psn_cash_pay.ToString() == null || ybinfo.setlinfo.psn_cash_pay.ToString() == "NULL" ? "0" : ybinfo.setlinfo.psn_cash_pay.ToString().Trim();      //本次现金支付
                    string tcjjzf = string.IsNullOrEmpty(ybinfo.setlinfo.fund_pay_sumamt.ToString()) || ybinfo.setlinfo.fund_pay_sumamt.ToString() == null || ybinfo.setlinfo.fund_pay_sumamt.ToString() == "NULL" ? "0" : ybinfo.setlinfo.fund_pay_sumamt.ToString().Trim();     //本次基金支付
                    string dejjzf = string.IsNullOrEmpty(ybinfo.setlinfo.hifmi_pay.ToString()) || ybinfo.setlinfo.hifmi_pay.ToString() == null || ybinfo.setlinfo.hifmi_pay.ToString() == "NULL" ? "0" : ybinfo.setlinfo.hifmi_pay.ToString().Trim();     //大病基金支付
                                                                                                                                                                                                                                                          //string mzjzfy = str2[30].Trim(); //民政救助金额
                    string dbecbc = "0";    //二次补偿金额
                    string dwfdfy = string.IsNullOrEmpty(ybinfo.setlinfo.hifes_pay.ToString()) || ybinfo.setlinfo.hifes_pay.ToString() == null || ybinfo.setlinfo.hifes_pay.ToString() == "NULL" ? "0" : ybinfo.setlinfo.hifes_pay.ToString().Trim();     //单位补充医保支付
                    string lxgbddtczf = "0";//离休干部单独统筹支付
                    string jlfy = "0";//甲类费用
                    string ylfy = "0";    //乙类费用
                    string blfy = "0";    //丙类费用
                    string zffy = string.IsNullOrEmpty(ybinfo.setlinfo.fulamt_ownpay_amt.ToString()) || ybinfo.setlinfo.fulamt_ownpay_amt.ToString() == null || ybinfo.setlinfo.fulamt_ownpay_amt.ToString() == "NULL" ? "0" : ybinfo.setlinfo.fulamt_ownpay_amt.ToString().Trim();     //自费费用
                    string jsr = CliUtils.fLoginUser.Trim();       //结算人
                    string jsrq = System.DateTime.Now.ToString("yyyy-MM-dd").Trim();      //结算日期
                    string jssj2 = sysdate.Trim();      //结算时间
                    string qfbzfy = string.IsNullOrEmpty(ybinfo.setlinfo.act_pay_dedc.ToString()) || ybinfo.setlinfo.act_pay_dedc.ToString() == null || ybinfo.setlinfo.act_pay_dedc.ToString() == "NULL" ? "0" : ybinfo.setlinfo.act_pay_dedc.ToString().Trim();   //起付标准自付
                    string fybzf = "0";     //非医保自付
                    string ylypzf = "0"; //乙类药品自付
                    string tjtzzf = "0";   //特检特治自付
                    string jrtcfy = "0";  //进入统筹自付
                    string jrdebxfy = "0"; //进入大病自付

                    string zdjbfwnbcje = "";//重大疾病范围内补偿金额(0)
                    string zdjbfwybcje = ""; //重大疾病范围外补偿金额(0)
                    string mzjzfy = ""; //民政救助金额
                    string gwynbfy = "";//公务员范围内补偿金额
                    string gwywbfy = "";//公务员范围外补偿金额
                    string yyfdje = ""; //医院负担金额(1)
                    string ygbtje = ""; //医改补贴金额
                    if (!liDqbh.Contains(dqbh2))
                    {
                        zdjbfwnbcje = "0"; //重大疾病范围内补偿金额(0)
                        zdjbfwybcje = "0"; //重大疾病范围外补偿金额(0)
                        mzjzfy = "0"; //民政救助金额
                        gwynbfy = "0";//公务员范围内补偿金额
                        gwywbfy = "0";//公务员范围外补偿金额
                        yyfdje = "0"; //医院负担金额(1)
                        ygbtje = "0";
                    }
                    else
                    {
                        //zdjbfwnbcje = string.IsNullOrEmpty(str2[46]) || str2[46] == null || str2[46] == "NULL" ? "0" : str2[46].Trim(); //重大疾病范围内补偿金额(0)
                        //zdjbfwybcje = string.IsNullOrEmpty(str2[47]) || str2[47] == null || str2[47] == "NULL" ? "0" : str2[47].Trim(); //重大疾病范围外补偿金额(0)
                        //mzjzfy = string.IsNullOrEmpty(str2[48]) || str2[48] == null || str2[48] == "NULL" ? "0" : str2[48].Trim(); //民政救助金额
                        //gwynbfy = string.IsNullOrEmpty(str2[49]) || str2[49] == null || str2[49] == "NULL" ? "0" : str2[49].Trim();//公务员范围内补偿金额
                        //gwywbfy = string.IsNullOrEmpty(str2[50]) || str2[50] == null || str2[50] == "NULL" ? "0" : str2[50].Trim();//公务员范围外补偿金额
                        //yyfdje = string.IsNullOrEmpty(str2[51]) || str2[51] == null || str2[51] == "NULL" ? "0" : str2[51].Trim(); //医院负担金额(1)
                        //ygbtje = string.IsNullOrEmpty(str2[52]) || str2[52] == null || str2[52] == "NULL" ? "0" : str2[52].Trim(); //医改补贴金额
                        zdjbfwnbcje = "0"; //重大疾病范围内补偿金额(0)
                        zdjbfwybcje = "0"; //重大疾病范围外补偿金额(0)
                        mzjzfy = "0"; //民政救助金额
                        gwynbfy = "0";//公务员范围内补偿金额
                        gwywbfy = "0";//公务员范围外补偿金额
                        yyfdje = "0"; //医院负担金额(1)
                        ygbtje = "0";//医改补贴金额
                    }

                    string mzdbjzjj = "0.00"; //民政大病救助基金(0)
                    string zftdjj = "0.00"; //政府兜底基金(1)
                    string gwybzfy = "0.00";   //其中公务员补助部分(1)
                    string jbr = CliUtils.fLoginUser;
                    // string dbecbc = str2[49].Trim(); //大病二次补偿(1)

                    //string zdjbfwnbcje = "0.00"; //重大疾病范围内补偿金额
                    //string zdjbfwybcje = "0.00"; //重大疾病范围外补偿金额
                    //string yyfdje = "0.00"; //医院负担金额
                    //string dbecbc = "0.00"; //大病二次补偿
                    //string mzdbjzjj = "0.00"; //民政大病救助基金
                    //string zftdjj = "0.00"; //政府兜底基金
                    //string gwybzfy = "0.00";   //其中公务员补助部分
                    //double bcjsqzhye = Convert.ToDouble(zhzf) + Convert.ToDouble(zfye);
                    double bcjsqzhye = Convert.ToDouble(zfye);
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
                     * 居民个人自付二次补偿金额|体检金额|生育基金支付|公务员内补|公务员外补
                     */
                    string Zbxje = (Convert.ToDecimal(ylfze) - Convert.ToDecimal(xjzf)).ToString();//总报销金额
                    //  gwybzfy = (Convert.ToDecimal(gwynbfy) + Convert.ToDecimal(gwywbfy)).ToString();//公务员内补+公务员外补
                    gwybzfy = (Convert.ToDecimal(gwynbfy) + Convert.ToDecimal(gwywbfy)).ToString();//公务员内补+公务员外补
                    string strValue = ylfze + "|" + Zbxje + "|" + tcjjzf + "|" + dejjzf + "|" + zhzf + "|" + xjzf1 + "|" + gwybzfy + "|" + dwfdfy + "|" + zffy + "|" + dwfdfy + "|" +
                                    yyfdje + "|" + mzjzfy + "|0.00|0.00|0.00|0.00|" + qfbzfy + "|0.00|" + jrtcfy + "|0.00|" +
                                    0.00 + "|" + jrdebxfy + "|0.00|0.00|0.00|" + bcjsqzhye + "|0.00|0.00|0.00|0.00|" +
                                    0.00 + "|" + 0.00 + "|" + zycs + "|" + xm2 + "|" + jsrq + "|" + yllb2 + "|" + ylrylb + "|" + dqbh2 + "||" + jslsh + "||" +
                                    "0000" + "|||1||" + dqbh2 + "|" + dbecbc + "|" + 0.00 + "|0.00|" + bxh2 + "||" +
                                    zftdjj + "|" + 0.00 + "|" + 0.00 + "|" + 0.00 + "|" + 0.00 + "|" + 0.00 + "|" + gwynbfy + "|" + gwywbfy + "|"; ;
                    WriteLog(sysdate + "  " + jzlsh + " 进入门诊收费预结算成功|" + strValue);

                    //项目明细显示
                    //Frm_ybfymxXG fb = new Frm_ybfymxXG();
                    //fb.dataGridView.AutoGenerateColumns = false;
                    //fb.dataGridView.DataSource = ds.Tables[0];
                    //fb.StartPosition = FormStartPosition.CenterParent;
                    //fb.ShowDialog();

                    //返回数据显示
                    //Frm_myjsjeXG fyjs = new Frm_myjsjeXG();
                    //fyjs.zfyje.Text = ylfze; //医疗总费用
                    //fyjs.zfje.Text = zhzf;//个人账户支付
                    //fyjs.tczfje.Text = jrtcfy;//进入统筹自付
                    //fyjs.tjtzzfje.Text = tjtzzf;//特检特治自付
                    //fyjs.ylgrfdje.Text = ylypzf;//乙类个人负担
                    //fyjs.jlje.Text = jlfy;//甲类费用
                    //fyjs.qfbzzfje.Text = qfbzfy;//起付标准自付
                    //fyjs.fybzfje.Text = fybzf;//非医保自付
                    //fyjs.lxgbddtcje.Text = lxgbddtczf;//离休干部单独统筹支付
                    //fyjs.zdjbnbje.Text = zdjbfwnbcje;//重大疾病范围内补金额
                    //fyjs.yyfdje.Text = yyfdje;//医院负担金额
                    //fyjs.mzdbjzje.Text = mzdbjzjj;//民政大病救助基金
                    //fyjs.mzjzje.Text = mzjzfy;//救助金额

                    //fyjs.grxjje.Text = xjzf;//个人现金支付
                    //fyjs.zffyje.Text = zffy;//自费费用
                    //fyjs.dbjjzfje.Text = dejjzf;//大病基金支付
                    //fyjs.dwbcybje.Text = dwfdfy;//单位补充医保支付
                    //fyjs.ylje.Text = ylfy;//乙类费用
                    //fyjs.grjjje.Text = tcjjzf;//个人基金支付
                    //fyjs.blje.Text = blfy;//丙类费用
                    //fyjs.dbzfje.Text = jrdebxfy;//进入大病自付
                    //fyjs.zdjbwbje.Text = zdjbfwybcje;//重大疾病范围外补金额
                    //fyjs.dbecbcje.Text = dbecbc;//大病二次补偿
                    //fyjs.zfddje.Text = zftdjj;//政府兜底基金
                    //fyjs.gwynbfy.Text = gwynbfy;//公务员内补金额
                    //fyjs.gwywbfy.Text = gwynbfy;//公务员外补金额
                    //fyjs.StartPosition = FormStartPosition.CenterParent;
                    //fyjs.ShowDialog();
                    return new object[] { 0, 1, strValue };
                }
                else
                {
                    WriteLog(sysdate + "  " + jzlsh + " 进入门诊收费预结算失败|" + res.ToString());
                    return new object[] { 0, 0, "门诊收费预结算失败|" + res.ToString() };
                }
            }
            catch (Exception ex)
            {
                WriteLog(sysdate + "  门诊收费预结算异常|" + ex.Message);
                return new object[] { 0, 0, "门诊收费预结算异常|" + ex.Message };
            }
        }
        #endregion

        #region 门诊收费结算 （新医保接口）
        public static object[] YBMZSFJS(object[] objParam)
        {
            string sysdate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string Ywlx = "2207";
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
                string sfdypj = "FALSE";                //是否打印票据
                double sfje2 = 0.0000; //金额 
                string ybjzlsh = "";   //医保就诊流水号
                string bxh = "";
                string xm = "";
                string kh = "";
                string dgysxm = ""; //开方医生(定岗医生)
                string jbr = CliUtils.fLoginUser;
                string sfzh = "";//身份证号

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

                string jzlsh1 = "";
                if (string.IsNullOrEmpty(bzbh))

                    jzlsh1 = "MZ" + yllb + jzlsh;

                else
                    jzlsh1 = "MZ" + yllb + jzlsh + bzbh;

                //string jzlsh1 = "MZ" + yllb+bzbh + jzlsh;

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
                    sfzh = ds.Tables[0].Rows[0]["sfzh"].ToString();//身份证号
                }
                #endregion

                #region 普通门诊不需要输入病种
                if (yllb.Equals("11") && string.IsNullOrEmpty(bzbh))
                {
                    bzbh = "NULL";
                    bzmc = "NULL";
                }
                #endregion

                #region 获取定岗医生信息

                strSql = string.Format(@"select dgysbm from ybdgyszd where ysbm='{0}'", cfysdm);
                ds.Tables.Clear();
                ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                if (ds.Tables[0].Rows.Count == 0)
                    dgysxm = "D360703015271";
                else
                    dgysxm = ds.Tables[0].Rows[0]["dgysbm"].ToString();
                cfysxm = dgysxm;

                #endregion

                #region 20200922 主动发送读卡交易 
                strSql = string.Format(@"select kh from YBICKXX where grbh='{0}' ", bxh);
                ds.Tables.Clear();
                ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    kh = ds.Tables[0].Rows[0]["kh"].ToString();
                }
                #endregion


                #region 医保获取个人信息
                ReadCardInfo cardInfo = GetYbCardInfo(new object[] { "02", sfzh, "", "", "1", sfzh, xm });
                if (cardInfo == null)
                {
                    return new object[] { 0, 0, "获取医保个人信息出错" };
                }
                #endregion 


                string res = string.Empty;
                string sfpcNo = jzlsh + "|" + System.DateTime.Now.ToString("yyyyMMddHHmmssfff");
                #region 新医保接口 门诊结算  by  hjw
                string data = "{\"psn_no\":\"" + bxh + "\",\"mdtrt_cert_type\":\"" + "02" + "\",\"mdtrt_cert_no\":\"" + sfzh + "\",\"card_sn\":\"\",\"psn_cert_type\":\"01\",\"certno\":\"" + sfzh + "\",\"psn_type\":\"" + cardInfo.insuinfo.psn_type + "\",\"psn_name\":\"" + xm + "\",\"medfee_sumamt\":\"" + sfje2 + "\",\"psn_setlway\":\"01\",\"mdtrt_id\":\"" + jzlsh + "\",\"chrg_bchno\":\"" + djh + "\",\"acct_used_flag\":\"1\",\"insutype\":\"" + cardInfo.insuinfo.insutype + "\",\"acct_used_flag\":\"1\",\"invono\":\"" + djh + "\",\"fulamt_ownpay_amt\":\"0\",\"overlmt_selfpay\":\"0\",\"preselfpay_amt\":\"0\",\"inscp_scp_amt\":\"0\"}";
                //string data = "{\"psn_no\":\"" + bxh + "\",\"mdtrt_cert_type\":\"03\",\"mdtrt_cert_no\":\"" + kh + "\",\"med_type\":\"12\",\"medfee_sumamt\":\"" + sfje2 + "\",\"psn_setlway\":\"01\",\"mdtrt_id\":\"" + jzlsh + "\",\"chrg_bchno\":\"" + sfpcNo + "\",\"insutype\":\"" + cardInfo.insuinfo.insutype + "\",\"acct_used_flag\":\"" + "0" + "\",\"invono\":\"" + djh + "\"}";
                //data = string.Format(data, bxh, "03", kh, "12", sfje2, "01", jzlsh, sfpcNo, cardInfo.insuinfo.insutype, "0", djh);
                WriteLog(sysdate + "  " + jzlsh + " 进入门诊收费结算...");
                WriteLog(sysdate + "  入参|" + data.ToString());
                int i = YBServiceRequest(Ywlx, data, ref res);

                #endregion
                if (i > 0)
                {
                    List<string> liSQL = new List<string>();
                    List<string> liDqbh = new List<string>();//赣州所有县地区编号

                    #region 赣州地区编号
                    //360701	【赣州市】
                    //360721	【赣县】
                    //360733	【会昌县】
                    //360723	【大余县】
                    //360734	【寻乌县】
                    //360782	【南康市】
                    //360730	【宁都县】
                    //360725	【崇义县】
                    //360726	【安远县】
                    //360732	【兴国县】
                    //360728	【定南县】
                    //360724	【上犹县】
                    //360735	【石城县】
                    //360722	【信丰县】
                    //360729	【全南县】
                    //360727	【龙南县】
                    //360781	【瑞金市】
                    //360731	【于都县】
                    //360702	【章贡区】
                    //360703    【开发区】
                    //360704    【蓉江新区】
                    //返回其他值 【异地就医】
                    liDqbh.Add("360701");
                    liDqbh.Add("360721");
                    liDqbh.Add("360733");
                    liDqbh.Add("360723");
                    liDqbh.Add("360734");
                    liDqbh.Add("360782");
                    liDqbh.Add("360730");
                    liDqbh.Add("360725");
                    liDqbh.Add("360726");
                    liDqbh.Add("360732");
                    liDqbh.Add("360728");
                    liDqbh.Add("360724");
                    liDqbh.Add("360735");
                    liDqbh.Add("360722");
                    liDqbh.Add("360729");
                    liDqbh.Add("360727");
                    liDqbh.Add("360781");
                    liDqbh.Add("360731");
                    liDqbh.Add("360702");
                    liDqbh.Add("360703");
                    liDqbh.Add("360704");
                    #endregion

                    #region 出参
                    /*出参:保险号|姓名|卡号|地区编号|地区名称|出生日期|实际年龄|参保日期|个人身份|单位名称|
                     * 性别|医疗人员类别|卡状态|账户余额|门诊(住院)号|单据流水号|医疗类别|科室名称|本次看病次数|住院床号|入院日期|
                     * 入院时间|出院日期|出院时间|出院原因|医疗总费用|本次账户支付|本次现金支付|本次基金支付|大病基金支付|二次补偿金额|
                     * 单位补充医保支付|离休干部单独统筹支付|甲类费用|乙类费用|丙类费用|自费费用|结算人|结算日期|结算时间|起付标准自付|
                     * 非医保自付|乙类药品自付|特检特治自付|进入统筹自付|进入大病自付|重大疾病范围内补偿金额|重大疾病范围外补偿金额|
                     * 民政救助金额|大病二次补偿|民政大病救助基金|政府兜底基金|其中公务员补助部分
                     */
                    YBBalanceInfo ybinfo = JsonConvert.DeserializeObject<YBBalanceInfo>(res);
                    string bxh2 = ybinfo.setlinfo.psn_no.Trim();       //保险号
                    string xm2 = ybinfo.setlinfo.psn_name.Trim();        //姓名
                    string kh2 = kh.Trim();        //卡号
                    string dqbh2 = dqbh.Trim();      //地区编号
                    string dqmc = "".Trim();       //地区名称
                    string csrq = ybinfo.setlinfo.brdy.Trim();       //出生日期
                    string sjnl = ybinfo.setlinfo.age.Trim();       //实际年龄
                    string cbrq = cardInfo.idetinfo.begntime.Trim();       //参保日期
                    string grsf = cardInfo.idetinfo.psn_type_lv.Trim();       //个人身份
                    string dwmc = cardInfo.insuinfo.emp_name.Trim();       //单位名称
                    string xb = ybinfo.setlinfo.gend.Trim();        //性别
                    string ylrylb = ybinfo.setlinfo.psn_type.Trim();    //医疗人员类别
                    string kzt = "";       //卡状态
                    string zfye = string.IsNullOrEmpty(ybinfo.setlinfo.balc) ? "0" : ybinfo.setlinfo.balc.Trim();     //账户余额
                    string jzlsh2 = ybinfo.setlinfo.mdtrt_id.Trim();    //门诊(住院)号
                    string jslsh = jzlsh.Trim();     //单据流水号
                    string yllb2 = ybinfo.setlinfo.med_type.Trim();      //医疗类别
                    string ksmc = "".Trim();      //科室名称
                    string zycs = "".Trim();      //本次看病次数
                    string zych = "".Trim();      //住院床号
                    string ryrq = "".Trim();      //入院日期
                    string rysj = "".Trim();      //入院时间
                    string cyrq = "".Trim();      //出院日期
                    string cysj = "".Trim();      //出院时间
                    string cyyy = "".Trim();      //出院原因
                    string ylfze = string.IsNullOrEmpty(ybinfo.setlinfo.medfee_sumamt.ToString()) || ybinfo.setlinfo.medfee_sumamt.ToString() == null || ybinfo.setlinfo.medfee_sumamt.ToString() == "NULL" ? "0" : ybinfo.setlinfo.medfee_sumamt.ToString().Trim();     //医疗总费用
                    string zhzf = string.IsNullOrEmpty(ybinfo.setlinfo.acct_pay.ToString()) || ybinfo.setlinfo.acct_pay.ToString() == null || ybinfo.setlinfo.acct_pay.ToString() == "NULL" ? "0" : ybinfo.setlinfo.acct_pay.ToString().Trim();       //本次账户支付
                    string xjzf = string.IsNullOrEmpty(ybinfo.setlinfo.psn_cash_pay.ToString()) || ybinfo.setlinfo.psn_cash_pay.ToString() == null || ybinfo.setlinfo.psn_cash_pay.ToString() == "NULL" ? "0" : ybinfo.setlinfo.psn_cash_pay.ToString().Trim();      //本次现金支付
                    string tcjjzf = string.IsNullOrEmpty(ybinfo.setlinfo.fund_pay_sumamt.ToString()) || ybinfo.setlinfo.fund_pay_sumamt.ToString() == null || ybinfo.setlinfo.fund_pay_sumamt.ToString() == "NULL" ? "0" : ybinfo.setlinfo.fund_pay_sumamt.ToString().Trim();     //本次基金支付
                    string dejjzf = string.IsNullOrEmpty(ybinfo.setlinfo.hifmi_pay.ToString()) || ybinfo.setlinfo.hifmi_pay.ToString() == null || ybinfo.setlinfo.hifmi_pay.ToString() == "NULL" ? "0" : ybinfo.setlinfo.hifmi_pay.ToString().Trim();     //大病基金支付
                                                                                                                                                                                                                                                          //string mzjzfy = str2[30].Trim(); //民政救助金额
                    string dbecbc = "0";    //二次补偿金额
                    string dwfdfy = string.IsNullOrEmpty(ybinfo.setlinfo.hifes_pay.ToString()) || ybinfo.setlinfo.hifes_pay.ToString() == null || ybinfo.setlinfo.hifes_pay.ToString() == "NULL" ? "0" : ybinfo.setlinfo.hifes_pay.ToString().Trim();     //单位补充医保支付
                    string lxgbddtczf = "0";//离休干部单独统筹支付
                    string jlfy = "0";//甲类费用
                    string ylfy = "0";    //乙类费用
                    string blfy = "0";    //丙类费用
                    string zffy = string.IsNullOrEmpty(ybinfo.setlinfo.fulamt_ownpay_amt.ToString()) || ybinfo.setlinfo.fulamt_ownpay_amt.ToString() == null || ybinfo.setlinfo.fulamt_ownpay_amt.ToString() == "NULL" ? "0" : ybinfo.setlinfo.fulamt_ownpay_amt.ToString().Trim();     //自费费用
                    string jsr = CliUtils.fLoginUser.Trim();       //结算人
                    string jsrq = System.DateTime.Now.ToString("yyyy-MM-dd").Trim();      //结算日期
                    string jssj2 = sysdate.Trim();      //结算时间
                    string qfbzfy = string.IsNullOrEmpty(ybinfo.setlinfo.act_pay_dedc.ToString()) || ybinfo.setlinfo.act_pay_dedc.ToString() == null || ybinfo.setlinfo.act_pay_dedc.ToString() == "NULL" ? "0" : ybinfo.setlinfo.act_pay_dedc.ToString().Trim();   //起付标准自付
                    string fybzf = "0";     //非医保自付
                    string ylypzf = "0"; //乙类药品自付
                    string tjtzzf = "0";   //特检特治自付
                    string jrtcfy = "0";  //进入统筹自付
                    string jrdebxfy = "0"; //进入大病自付

                    string zdjbfwnbcje = "";//重大疾病范围内补偿金额(0)
                    string zdjbfwybcje = ""; //重大疾病范围外补偿金额(0)
                    string mzjzfy = ""; //民政救助金额
                    string gwynbfy = "";//公务员范围内补偿金额
                    string gwywbfy = "";//公务员范围外补偿金额
                    string yyfdje = ""; //医院负担金额(1)
                    string ygbtje = ""; //医改补贴金额
                    if (!liDqbh.Contains(dqbh2))
                    {
                        zdjbfwnbcje = "0"; //重大疾病范围内补偿金额(0)
                        zdjbfwybcje = "0"; //重大疾病范围外补偿金额(0)
                        mzjzfy = "0"; //民政救助金额
                        gwynbfy = "0";//公务员范围内补偿金额
                        gwywbfy = "0";//公务员范围外补偿金额
                        yyfdje = "0"; //医院负担金额(1)
                        ygbtje = "0";
                    }
                    else
                    {
                        //zdjbfwnbcje = string.IsNullOrEmpty(str2[46]) || str2[46] == null || str2[46] == "NULL" ? "0" : str2[46].Trim(); //重大疾病范围内补偿金额(0)
                        //zdjbfwybcje = string.IsNullOrEmpty(str2[47]) || str2[47] == null || str2[47] == "NULL" ? "0" : str2[47].Trim(); //重大疾病范围外补偿金额(0)
                        //mzjzfy = string.IsNullOrEmpty(str2[48]) || str2[48] == null || str2[48] == "NULL" ? "0" : str2[48].Trim(); //民政救助金额
                        //gwynbfy = string.IsNullOrEmpty(str2[49]) || str2[49] == null || str2[49] == "NULL" ? "0" : str2[49].Trim();//公务员范围内补偿金额
                        //gwywbfy = string.IsNullOrEmpty(str2[50]) || str2[50] == null || str2[50] == "NULL" ? "0" : str2[50].Trim();//公务员范围外补偿金额
                        //yyfdje = string.IsNullOrEmpty(str2[51]) || str2[51] == null || str2[51] == "NULL" ? "0" : str2[51].Trim(); //医院负担金额(1)
                        //ygbtje = string.IsNullOrEmpty(str2[52]) || str2[52] == null || str2[52] == "NULL" ? "0" : str2[52].Trim(); //医改补贴金额
                        zdjbfwnbcje = "0"; //重大疾病范围内补偿金额(0)
                        zdjbfwybcje = "0"; //重大疾病范围外补偿金额(0)
                        mzjzfy = "0"; //民政救助金额
                        gwynbfy = "0";//公务员范围内补偿金额
                        gwywbfy = "0";//公务员范围外补偿金额
                        yyfdje = "0"; //医院负担金额(1)
                        ygbtje = "0";//医改补贴金额
                    }

                    string mzdbjzjj = "0.00"; //民政大病救助基金(0)
                    string zftdjj = "0.00"; //政府兜底基金(1)
                    string gwybzfy = "0.00";   //其中公务员补助部分(1)
                                               // string jbr = CliUtils.fLoginUser;
                                               // string dbecbc = str2[49].Trim(); //大病二次补偿(1)

                    //string zdjbfwnbcje = "0.00"; //重大疾病范围内补偿金额
                    //string zdjbfwybcje = "0.00"; //重大疾病范围外补偿金额
                    //string yyfdje = "0.00"; //医院负担金额
                    //string dbecbc = "0.00"; //大病二次补偿
                    //string mzdbjzjj = "0.00"; //民政大病救助基金
                    //string zftdjj = "0.00"; //政府兜底基金
                    //string gwybzfy = "0.00";   //其中公务员补助部分
                    //double bcjsqzhye = Convert.ToDouble(zhzf) + Convert.ToDouble(zfye);
                    double bcjsqzhye = Convert.ToDouble(zfye);
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

                    string Zbxje = (Convert.ToDecimal(ylfze) - Convert.ToDecimal(xjzf)).ToString();//总报销金额
                    gwybzfy = (Convert.ToDecimal(gwynbfy) + Convert.ToDecimal(gwywbfy)).ToString();//公务员内补+公务员外补

                    string strValue = ylfze + "|" + Zbxje + "|" + tcjjzf + "|" + dejjzf + "|" + zhzf + "|" + xjzf1 + "|" + gwybzfy + "|" + dwfdfy + "|" + zffy + "|" + dwfdfy + "|" +
                                    yyfdje + "|" + mzjzfy + "|0.00|0.00|0.00|0.00|" + qfbzfy + "|0.00|" + jrtcfy + "|0.00|" +
                                    0.00 + "|" + jrdebxfy + "|0.00|0.00|0.00|" + bcjsqzhye + "|0.00|0.00|0.00|0.00|" +
                                    0.00 + "|" + 0.00 + "|" + zycs + "|" + xm2 + "|" + jsrq + "|" + yllb2 + "|" + ylrylb + "|" + dqbh2 + "||" + jslsh + "||" +
                                    djh + "|||1||" + dqbh2 + "|" + dbecbc + "|" + 0.00 + "|0.00|" + bxh2 + "||" +
                                    zftdjj + "|" + 0.00 + "|" + 0.00 + "|" + 0.00 + "|" + 0.00 + "|" + 0.00 + "|" + gwynbfy + "|" + gwywbfy + "|";

                    strSql = string.Format(@"insert into ybfyjsdr(jzlsh,jylsh,djhin,cyrq,cyyy,zhsybz,ztjsbz,jbr,xm,kh,
                                        grbh,jsrq,yllb,yldylb,jslsh,ylfze,zhzf,xjzf,tcjjzf,dejjzf,
                                        mzjzfy,dwfdfy,lxgbddtczf,jlfy,ylfy,blfy,zffy,qfbzfy,fybzf,ylypzf,
                                        tjtzzf,jrtczf,jrdbzf,zdjbfwnbcje,zdjbfwybcje,yyfdfy,ecbcje,mzdbjzje,czzcje,gwybzjjzf,
				                        sysdate,djh,ybjzlsh,zbxje,bcjsqzhye,gwynbfy,gwywbfy,bzbm,bzmc,ygbtje) values(
                                        '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}',
                                        '{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}',
                                        '{20}','{21}','{22}','{23}','{24}','{25}','{26}','{27}','{28}','{29}',
                                        '{30}','{31}','{32}','{33}','{34}','{35}','{36}','{37}','{38}','{39}',
                                        '{40}','{41}','{42}','{43}','{44}','{45}','{46}','{47}','{48}','{49}')",
                                    jzlsh, jslsh, djh, cyrq, cyyy, 1, 0, jbr, xm2, kh2,
                                    bxh2, jsrq, yllb, ylrylb, jslsh, ylfze, zhzf, xjzf, tcjjzf, dejjzf,
                                    mzjzfy, dwfdfy, lxgbddtczf, jlfy, ylfy, blfy, zffy, qfbzfy, fybzf, ylypzf,
                                    tjtzzf, jrtcfy, jrdebxfy, zdjbfwnbcje, zdjbfwybcje, yyfdje, dbecbc, mzdbjzjj, zftdjj, gwybzfy,
                                    sysdate, djh, jzlsh2, Zbxje, bcjsqzhye, gwynbfy, gwywbfy, bzbh == "NULL" ? "" : bzbh, bzmc == "NULL" ? "" : bzmc, ygbtje);



                    liSQL.Add(strSql);
                    #region 没有处方明细的信息
                    ///*
                    // * 门诊(住院)号|单据流水号|项目编号|项目名称|项目等级|收费类别|单价|数量|金额|自付金额
                    // */
                    //for (int j = 1; j < str.Length; j++)
                    //{
                    //    str2 = str[j].Split('|');
                    //    string jzlsh3 = str2[0].Trim(); //门诊(住院)号
                    //    string djlsh = str2[1].Trim();  //单据流水号
                    //    string ybxmbh = str2[2].Trim(); //项目编号
                    //    string ybxmmc = str2[3].Trim(); //项目名称
                    //    string ybxmdj = str2[4].Trim(); //项目等级
                    //    string ybsflb = str2[5].Trim(); //收费类别
                    //    string dj = str2[6].Trim();     //单价
                    //    string sl = str2[7].Trim();     //数量
                    //    string je = str2[8].Trim();     //金额
                    //    string zfje = str2[9].Trim();   //自付金额
                    //    for (int k = 0; k < li_ybxmbh.Count; k++)
                    //    {
                    //        if (ybxmbh.Equals(li_ybxmbh[k]))
                    //        {
                    //            strSql = string.Format(@"insert into ybcfmxscindr(jzlsh,je,xm,kh,ybcfh,yysfxmbm,yysfxmmc,sysdate,ybjzlsh,djlsh,
                    //                                sfxmzxbm,sfxmzxmc,sflb,sfxmzl,dj,sl,zfje,jsdjh) values(
                    //                                '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}',
                    //                                '{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}')",
                    //                                    jzlsh, je, xm2, kh2, li_cfh[k], li_yyxmbh[k], li_yyxmmc[k], sysdate, jzlsh3, djlsh,
                    //                                    ybxmbh, ybxmmc, ybsflb, ybxmdj, dj, sl, zfje, djh);
                    //            liSQL.Add(strSql);
                    //            break;
                    //        }
                    //    }
                    //} 
                    #endregion
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
                    WriteLog(sysdate + "  " + jzlsh + " 进入门诊收费结算失败|" + res.ToString());
                    return new object[] { 0, 0, "门诊收费结算失败|" + res.ToString() };
                }
            }
            catch (Exception ex)
            {
                return new object[] { 0, 0, "门诊收费结算异常" + ex.Message };
            }
        }
        #endregion

        #region 门诊收费结算撤销（新医保接口）
        public static object[] YBMZSFJSCX(object[] objParam)
        {
            string sysdate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string Ywlx = "2208";
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

            #region 20200922 主动发送读卡交易
            //strSql = string.Format(@"select kh from YBICKXX where grbh='{0}' and datediff(minute, sysDate,GETDATE()) > 120 ", bxh);
            //ds.Tables.Clear();
            //ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            //if (ds.Tables[0].Rows.Count > 0)
            //{
            //    YBMZDK(null);
            //    kh = dzkh;
            //}
            //else
            //{
            //    kh = ds.Tables[0].Rows[0]["kh"].ToString();
            //}
            strSql = string.Format(@"select kh from YBICKXX where grbh='{0}' ", bxh);
            ds.Tables.Clear();
            ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            if (ds.Tables[0].Rows.Count > 0)
            {
                kh = ds.Tables[0].Rows[0]["kh"].ToString();
            }
            #endregion

            ds.Dispose();

            #endregion

            #region 老医保接口  by hjw
            //StringBuilder inputData = new StringBuilder();
            ////入参:保险号|姓名|卡号|地区编号|门诊号|单据流水号
            //inputData.Append(bxh + "|");
            //inputData.Append(xm + "|");
            //inputData.Append(kh + "|");
            //inputData.Append(dqbh + "|");
            //inputData.Append(ybjzlsh + "|");
            //inputData.Append(djlsh);
            //StringBuilder outData = new StringBuilder(1024);
            //StringBuilder retMsg = new StringBuilder(1024);

            //WriteLog(sysdate + "  " + jzlsh + " 进入门诊收费撤销...");
            //WriteLog(sysdate + "  入参|" + inputData.ToString());
            //int i = f_UserBargaingApply(Ywlx, inputData, outData, retMsg);
            #endregion
            string res = string.Empty;
            #region 新医保接口 by hjw
            string data = "{\"setl_id\":\"" + djh + "\",\"mdtrt_id\":\"" + jzlsh + "\",\"psn_no\":\"" + bxh + "\"}";
            //data = String.Format(data, djh, jzlsh, ybjzlsh);
            WriteLog(sysdate + "  " + jzlsh + " 进入门诊收费撤销...");
            WriteLog(sysdate + "  入参|" + data.ToString());
            int i = YBServiceRequest(Ywlx, data, ref res);
            YBBalanceInfo ybinfo = new YBBalanceInfo();
            #endregion
            if (i > 0)
            {
                ybinfo = JsonConvert.DeserializeObject<YBBalanceInfo>(res);
                List<string> liSQL = new List<string>();
                strSql = string.Format(@"insert into ybfyjsdr(jzlsh,jylsh,djhin,cyrq,cyyy,zhsybz,ztjsbz,jbr,xm,kh,
                                        grbh,jsrq,yllb,yldylb,jslsh,ylfze,zhzf,xjzf,tcjjzf,dejjzf,
                                        mzjzfy,dwfdfy,lxgbddtczf,jlfy,ylfy,blfy,zffy,qfbzfy,fybzf,ylypzf,
                                        tjtzzf,jrtczf,jrdbzf,zdjbfwnbcje,zdjbfwybcje,yyfdfy,ecbcje,mzdbjzje,czzcje,gwybzjjzf,
                                        djh,sysdate,cxbz,bzbm,bzmc) select 
                                        jzlsh,jylsh,djhin,cyrq,cyyy,zhsybz,ztjsbz,'{3}',xm,kh,
                                        grbh,jsrq,yllb,yldylb,jslsh,ylfze,zhzf,xjzf,tcjjzf,dejjzf,
                                        mzjzfy,dwfdfy,lxgbddtczf,jlfy,ylfy,blfy,zffy,qfbzfy,fybzf,ylypzf,
                                        tjtzzf,jrtczf,jrdbzf,zdjbfwnbcje,zdjbfwybcje,yyfdfy,ecbcje,mzdbjzje,czzcje,gwybzjjzf,
                                        djh,'{2}',0,bzbm,bzmc from ybfyjsdr where jslsh='{0}' and jzlsh='{1}' and cxbz=1", djlsh, jzlsh, sysdate, CliUtils.fLoginUser);
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
                    WriteLog(sysdate + "  " + jzlsh + " 进入门诊收费撤销成功|" + res.ToString());
                    return new object[] { 0, 1, res.ToString() };
                }
                else
                {
                    WriteLog(sysdate + "  " + jzlsh + " 进入门诊收费撤销成功|操作本地数据失败|" + obj[2].ToString());
                    return new object[] { 0, 2, "门诊收费撤销失败|" + obj[2].ToString() };
                }
            }
            else
            {
                WriteLog(sysdate + "  " + jzlsh + " 进入门诊收费撤销失败|" + res.ToString());
                return new object[] { 0, 0, "门诊收费撤销失败|" + res.ToString() };
            }
        }
        #endregion

        #region 住院读卡 （新医保接口 ）
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
                    ParamYBDK dk = new ParamYBDK();
                    dk.Grbh = str[0].Trim();     //保险号
                    dk.Xm = str[1].Trim();      //姓名
                    dk.Kh = str[2].Trim();      //卡号
                    dk.Tcqh = str[3].Trim();    //地区编号
                    dk.Qxmc = str[4].Trim();    //地区名称
                    dk.Csrq = str[5].Trim();    //出生日期
                    // dk.Nl = str[6].Trim();    //实际年龄
                    // MessageBox.Show(dk.Csrq.Substring(0, 4) + "-" + dk.Csrq.Substring(4, 2) + "-" + dk.Csrq.Substring(6, 2));
                    dk.Nl = string.IsNullOrEmpty(str[6]) || str[6] == null || str[6] == "NULL" ? GetAgeByBirthdate(DateTime.Parse(dk.Csrq.Substring(0, 4) + "-" + dk.Csrq.Substring(4, 2) + "-" + dk.Csrq.Substring(6, 2))).ToString() : str[6].ToString(); //年龄
                    // MessageBox.Show(dk.Nl);
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

                    List<string> liDqbh = new List<string>();//赣州所有县地区编号
                    #region 赣州地区编号
                    //360701	【赣州市】
                    //360721	【赣县】
                    //360733	【会昌县】
                    //360723	【大余县】
                    //360734	【寻乌县】
                    //360782	【南康市】
                    //360730	【宁都县】
                    //360725	【崇义县】
                    //360726	【安远县】
                    //360732	【兴国县】
                    //360728	【定南县】
                    //360724	【上犹县】
                    //360735	【石城县】
                    //360722	【信丰县】
                    //360729	【全南县】
                    //360727	【龙南县】
                    //360781	【瑞金市】
                    //360731	【于都县】
                    //360702	【章贡区】
                    //360703    【开发区】
                    //360704    【蓉江新区】
                    //返回其他值 【异地就医】
                    liDqbh.Add("360701");
                    liDqbh.Add("360721");
                    liDqbh.Add("360733");
                    liDqbh.Add("360723");
                    liDqbh.Add("360734");
                    liDqbh.Add("360782");
                    liDqbh.Add("360730");
                    liDqbh.Add("360725");
                    liDqbh.Add("360726");
                    liDqbh.Add("360732");
                    liDqbh.Add("360728");
                    liDqbh.Add("360724");
                    liDqbh.Add("360735");
                    liDqbh.Add("360722");
                    liDqbh.Add("360729");
                    liDqbh.Add("360727");
                    liDqbh.Add("360781");
                    liDqbh.Add("360731");
                    liDqbh.Add("360702");
                    liDqbh.Add("360703");
                    liDqbh.Add("360704");
                    #endregion

                    if (!liDqbh.Contains(dk.Tcqh)) //异地
                    {
                        dk.Jflx = "09";
                    }
                    else
                    {
                        //if (dk.Ylrylb.Contains("扶贫"))
                        //    dk.Jflx = "04";
                        //else 
                        if (dk.Ylrylb.Contains("居民"))
                            dk.Jflx = "02";
                        else
                            dk.Jflx = "01";
                    }
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

        #region 住院登记（新医保接口）
        /// <summary>
        /// 住院登记（新医保接口）
        /// </summary>
        /// <param name="objParam">1就诊流水号2医疗类别代码 3病种编码 4病种名称5读卡返回信息6诊断信息格式：诊断类别1;诊断类别2|主诊断标志1;主诊断标志2|诊断编码1;诊断编码2|诊断名称1;诊断名称2</param>
        /// <returns></returns>
        public static object[] YBZYDJ(object[] objParam)
        {
            string sysdate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            try
            {
                string Ywlx = "2401";
                string jzlsh = objParam[0].ToString(); //就诊流水号
                string yllb = objParam[1].ToString(); //医疗类别代码
                string bzbm = string.IsNullOrEmpty(objParam[2].ToString()) ? "0000000" : objParam[2].ToString(); //病种编码
                string bzmc = string.IsNullOrEmpty(objParam[3].ToString()) ? "其他病种" : objParam[3].ToString(); //病种名称
                string[] kxx = objParam[4].ToString().Split('|'); //读卡返回信息
                string[] zdxx = objParam[5].ToString().Split('|');//诊断信息
                if (zdxx.Length < 1)
                {
                    return new object[] { 0, 0, "无诊断类别信息" };
                }
                List<string> zdlbs = zdxx[0].Split(';').ToList();//诊断类别集合
                if (zdxx.Length < 2)
                {
                    return new object[] { 0, 0, "无主诊断标识信息" };
                }
                List<string> zzdbss = zdxx[1].Split(';').ToList();//主诊断标识集合
                if (zdxx.Length < 3)
                {
                    return new object[] { 0, 0, "无诊断编号信息" };
                }
                List<string> zdbhs = zdxx[2].Split(';').ToList();//诊断编号集合
                if (zdxx.Length < 4)
                {
                    return new object[] { 0, 0, "无诊断名称信息" };
                }
                List<string> zdmcs = zdxx[3].Split(';').ToList();//诊断名称集合

                #region 读卡信息
                if (kxx.Length < 2)
                    return new object[] { 0, 0, "无读卡信息反馈" };
                //string.IsNullOrEmpty(kxx[26]) || kxx[26] == null ? "NULL" :
                string grbh = string.IsNullOrEmpty(kxx[0]) || kxx[0] == null || kxx[0] == "NULL" ? "" : kxx[0].ToString(); //个人编号
                string dwbh = string.IsNullOrEmpty(kxx[1]) || kxx[1] == null || kxx[1] == "NULL" ? "" : kxx[1].ToString(); //单位编号
                string sfzh = string.IsNullOrEmpty(kxx[2]) || kxx[2] == null || kxx[2] == "NULL" ? "" : kxx[2].ToString(); //身份证号
                string xm = string.IsNullOrEmpty(kxx[3]) || kxx[3] == null || kxx[3] == "NULL" ? "" : kxx[3].ToString(); //姓名
                string xb = string.IsNullOrEmpty(kxx[4]) || kxx[4] == null || kxx[4] == "NULL" ? "" : kxx[4].ToString(); //性别
                string mz = string.IsNullOrEmpty(kxx[5]) || kxx[5] == null || kxx[5] == "NULL" ? "" : kxx[5].ToString(); //民族
                string csrq = string.IsNullOrEmpty(kxx[6]) || kxx[6] == null || kxx[6] == "NULL" ? "" : kxx[6].ToString(); //出生日期
                string kh = string.IsNullOrEmpty(kxx[7]) || kxx[7] == null || kxx[7] == "NULL" ? "" : kxx[7].ToString(); //卡号
                string yldylb = string.IsNullOrEmpty(kxx[8]) || kxx[8] == null || kxx[8] == "NULL" ? "" : kxx[8].ToString(); //医疗待遇类别
                string rycbzt = string.IsNullOrEmpty(kxx[9]) || kxx[9] == null || kxx[9] == "NULL" ? "" : kxx[9].ToString(); //人员参保状态
                string ydrybz = string.IsNullOrEmpty(kxx[10]) || kxx[10] == null || kxx[10] == "NULL" ? "" : kxx[10].ToString(); //异地人员标志
                string tcqh = string.IsNullOrEmpty(kxx[11]) || kxx[11] == null || kxx[11] == "NULL" ? "" : kxx[11].ToString(); //统筹区号
                string nd = string.IsNullOrEmpty(kxx[12]) || kxx[12] == null || kxx[12] == "NULL" ? "" : kxx[12].ToString(); //年度
                string zyzt = string.IsNullOrEmpty(kxx[13]) || kxx[13] == null || kxx[13] == "NULL" ? "" : kxx[13].ToString(); //在院状态
                string zhye = string.IsNullOrEmpty(kxx[14]) || kxx[14] == null || kxx[14] == "NULL" ? "" : kxx[14].ToString(); //帐户余额
                string bnylflj = string.IsNullOrEmpty(kxx[15]) || kxx[15] == null || kxx[15] == "NULL" ? "" : kxx[15].ToString(); //本年医疗费累计
                string bnzhzclj = string.IsNullOrEmpty(kxx[16]) || kxx[16] == null || kxx[16] == "NULL" ? "" : kxx[16].ToString(); //本年帐户支出累计
                string bntczclj = string.IsNullOrEmpty(kxx[17]) || kxx[17] == null || kxx[17] == "NULL" ? "" : kxx[17].ToString(); //本年统筹支出累计
                string bnjzjzclj = string.IsNullOrEmpty(kxx[18]) || kxx[18] == null || kxx[18] == "NULL" ? "" : kxx[18].ToString(); //本年救助金支出累计
                string bngwybzjjlj = string.IsNullOrEmpty(kxx[19]) || kxx[19] == null || kxx[19] == "NULL" ? "" : kxx[19].ToString(); //本年公务员补助基金累计
                string bnczjmmztczflj = string.IsNullOrEmpty(kxx[20]) || kxx[20] == null || kxx[20] == "NULL" ? "" : kxx[20].ToString(); //本年城镇居民门诊统筹支付累计
                string jrtcfylj = string.IsNullOrEmpty(kxx[21]) || kxx[21] == null || kxx[21] == "NULL" ? "" : kxx[21].ToString(); //进入统筹费用累计
                string jrjzjfylj = string.IsNullOrEmpty(kxx[22]) || kxx[22] == null || kxx[22] == "NULL" ? "" : kxx[22].ToString(); //进入救助金费用累计
                string qfbzlj = string.IsNullOrEmpty(kxx[23]) || kxx[23] == null || kxx[23] == "NULL" ? "" : kxx[23].ToString(); //起付标准累计
                string bnzycs = string.IsNullOrEmpty(kxx[24]) || kxx[24] == null || kxx[24] == "NULL" ? "" : kxx[24].ToString(); //本年住院次数
                string dwmc = string.IsNullOrEmpty(kxx[25]) || kxx[25] == null || kxx[25] == "NULL" ? "" : kxx[25].ToString(); //单位名称
                string nl = string.IsNullOrEmpty(kxx[26]) || kxx[26] == null || kxx[26] == "NULL" ? GetAgeByBirthdate(DateTime.Parse(csrq)).ToString() : kxx[26].ToString(); //年龄
                string cbdwlx = string.IsNullOrEmpty(kxx[27]) || kxx[27] == null || kxx[27] == "NULL" ? "" : kxx[27].ToString(); //参保单位类型
                string jbjgbm = string.IsNullOrEmpty(kxx[28]) || kxx[28] == null || kxx[28] == "NULL" ? "" : kxx[28].ToString(); //经办机构编码


                #endregion
                string lyjgdm = objParam[5].ToString();//来源机构代码
                string lyjgmc = objParam[6].ToString();//来源机构名称
                string yllbmc = objParam[7].ToString();//医疗类别名称
                string dgysdm = objParam[8] == null ? "" : objParam[8].ToString(); //定岗医生代码
                string dgysxm = objParam[9].ToString(); //定岗医生姓名
                string ryrq = string.IsNullOrEmpty(objParam[10].ToString()) ? DateTime.Now.ToString("yyyy-MM-dd") : objParam[10].ToString(); //入院日期

                if (rycbzt == "冻结")
                {
                    DialogResult dr = MessageBox.Show("该患者卡已冻结，是否继续医保登记?", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (dr != DialogResult.Yes) { return new object[] { 0, 0, "该卡已冻结！" }; }
                }

                #region 获取定岗医生信息
                string strSql2 = string.Format(@"select * from ybdgyszd where ysbm='{0}'", dgysdm);
                DataSet ds2 = CliUtils.ExecuteSql("sybdj", "cmd", strSql2, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                if (ds2.Tables[0].Rows.Count == 0)
                {
                    dgysdm = "010091";
                    dgysxm = "江辉";
                }
                else
                {
                    dgysdm = ds2.Tables[0].Rows[0]["dgysbm"].ToString();
                    dgysxm = ds2.Tables[0].Rows[0]["ysxm"].ToString();
                }
                #endregion


                //string strDate = string.Format("@ select z1date from zy01h where z1zyno='{0}'", jzlsh);
                //DataSet dsDate = CliUtils.ExecuteSql("sybdj", "cmd", strDate, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                //string ryrq = dsDate.Tables[0].Rows.Count > 0 ? DateTime.Parse(dsDate.Tables[0].Rows[0]["z1date"].ToString()).ToString("yyyy-MM-dd") : DateTime.Now.ToString(); //入院日期

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
                string tel = "";//电话号码
                string blh = "";//病历号
                string zzysbh = "";//主治医生编号
                string zzysmc = "";//主治医生名称
                string ryzdqk = "";//入院诊断情况
                string ryzd = "";//入院诊断

                if (string.IsNullOrEmpty(yllb))
                    return new object[] { 0, 0, "医疗类别不能为空" };
                if (string.IsNullOrEmpty(jzlsh))
                    return new object[] { 0, 0, "就诊流水号不能为空" };

                #region 是否办理住院

                string strSql = string.Format(@"select a.z1date as ryrq,z1hznm,a.z1ksno,a.z1ksnm, isnull(ybbfh,z1cwid) as z1bedn,a.z1mobi as tel,a.z1zyno as blh,a.z1ynno as zzysbh,a.z1ynnm as zzysmc,a.z1ryqk as ryzdqk,a.z1ryzd as ryzd  from zy01h a left join ybbfdzdr b on a.z1cwid=bedno and a.z1bqxx=b.bqno and a.z1ksno=b.ksno
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
                    tel = ds.Tables[0].Rows[0]["tel"].ToString();
                    blh = ds.Tables[0].Rows[0]["blh"].ToString();
                    zzysbh = ds.Tables[0].Rows[0]["zzysbh"].ToString();
                    zzysmc = ds.Tables[0].Rows[0]["zzysmc"].ToString();
                    ryzdqk = ds.Tables[0].Rows[0]["ryzdqk"].ToString();
                    ryzd = ds.Tables[0].Rows[0]["ryzd"].ToString();
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


                #region 老版本医保接口
                //StringBuilder inputData = new StringBuilder();
                ////入参: 保险号|姓名|卡号|地区编号|医疗类别|科室名称|住院日期|住院时间|住院床号|入院疾病|医院住院流水号
                //inputData.Append(bxh + "|");
                //inputData.Append(xm + "|");
                //inputData.Append(kh + "|");
                //inputData.Append(tcqh + "|");
                //inputData.Append(yllb + "|");
                //inputData.Append(ksmc + "|");
                //inputData.Append(zyrq + "|");
                //inputData.Append(zysj + "|");
                //inputData.Append(zych + "|");
                //inputData.Append(bzmc + "|");
                //inputData.Append(jzlsh);
                //StringBuilder outData = new StringBuilder(1024);
                //StringBuilder retMsg = new StringBuilder(1024);

                //WriteLog(sysdate + "  " + jzlsh + " 进入住院医保登记...");
                //WriteLog(sysdate + "  入参|" + inputData.ToString());
                //int i = f_UserBargaingApply(Ywlx, inputData, outData, retMsg);
                #endregion

                #region 医保获取个人信息
                ReadCardInfo cardInfo = GetYbCardInfo(new object[] { "02", sfzh, "", "", "1", sfzh, xm });
                if (cardInfo == null)
                {
                    return new object[] { 0, 0, "获取医保个人信息出错" };
                }
                #endregion

                #region 新医保接口 住院登记 by hjw
                string res = string.Empty;
                #region 暂时屏蔽入参
                ////就诊信息
                //string mdtrtinfodata = "{\"psn_no\":\"{0}\",\"insutype\":\"{1}\",\"coner_name\":\"{2}\",\"tel\":\"{3}\",\"begntime\":\"{4}\",\"mdtrt_cert_type\":\"{5}\",\"mdtrt_cert_no\":\"{6}\",\"med_type\":\"{7}\",\"ipt_no\":\"{8}\",\"medrcdno\":\"{9}\",\"atddr_no\":\"{10}\",\"chfpdr_name\":\"{11}\",\"adm_diag_dscr\":\"{12}\",\"adm_dept_codg\":\"{13}\",\"adm_dept_name\":\"{14}\",\"adm_bed\":\"{15}\",\"dscg_maindiag_code\":\"{16}\",\"dscg_maindiag_name\":\"{17}\",\"main_cond_dscr\":\"{18}\",\"dise_codg\":\"{19}\",\"dise_name\":\"{20}\",\"oprn_oprt_code\":\"{21}\",\"oprn_oprt_name\":\"{22}\",\"fpsc_no\":\"{23}\",\"matn_type\":\"{24}\",\"birctrl_type\":\"{25}\",\"latechb_flag\":\"{26}\",\"geso_val\":\"{27}\",\"fetts\":\"{28}\",\"fetus_cnt\":\"{29}\",\"pret_flag\":\"{30}\",\"birctrl_matn_date\":\"{31}\",\"dise_type_code\":\"{32}\"}";

                ////入院诊断信息
                //string diseinfo = "{\"psn_no\":\"{0}\",\"diag_type\":\"{1}\",\"maindiag_flag\":\"{2}\",\"diag_srt_no\":\"{3}\",\"diag_code\":\"{4}\",\"diag_name\":\"{5}\",\"adm_cond\":\"{6}\",\"diag_dept\":\"{7}\",\"dise_dor_no\":\"{8}\",\"dise_dor_name\":\"{9}\",\"diag_time\":\"{10}\"}"; 
                #endregion

                #region 入参赋值
                Mdtrtinfo mdtrtinfo = new Mdtrtinfo();//就诊信息
                mdtrtinfo.psn_no = grbh;//人员编号
                mdtrtinfo.insutype = cardInfo.insuinfo.insutype;//险种类型
                mdtrtinfo.coner_name = xm;//联系人姓名
                mdtrtinfo.tel = tel;//电话号码
                mdtrtinfo.begntime = ryrq;//入院日期
                mdtrtinfo.mdtrt_cert_type = "03";//就诊凭证类型
                mdtrtinfo.mdtrt_cert_no = kh;//就诊凭证编号
                mdtrtinfo.med_type = yllb;//医疗类别
                mdtrtinfo.ipt_no = jzlsh;//院内住院流水号
                mdtrtinfo.medrcdno = blh;//病历号
                mdtrtinfo.atddr_no = zzysbh; //主治医生编号
                mdtrtinfo.chfpdr_name = zzysmc;//主诊医生名称
                mdtrtinfo.adm_diag_dscr = ryzdqk;//入院诊断描述
                mdtrtinfo.adm_dept_codg = ksbh;//入院科室编号
                mdtrtinfo.adm_dept_name = ksmc;//入院科室名称
                mdtrtinfo.adm_bed = zych;//入院床位
                mdtrtinfo.dscg_maindiag_code = bzbm;//住院主诊断代码
                mdtrtinfo.dscg_maindiag_name = bzmc;//住院主诊断名称
                mdtrtinfo.main_cond_dscr = "";//主要病情描述
                mdtrtinfo.dise_codg = bzbm;//病种编码
                mdtrtinfo.dise_name = bzmc;//病种名称
                mdtrtinfo.oprn_oprt_code = "";//手术操作代码
                mdtrtinfo.oprn_oprt_name = "";//手术操作名称
                mdtrtinfo.fpsc_no = "";//计划生育服务证号
                mdtrtinfo.matn_type = "";//生育类别
                mdtrtinfo.birctrl_type = "";//计划生育手术类别
                mdtrtinfo.latechb_flag = "";//晚育标志
                mdtrtinfo.geso_val = 0;//孕周次
                mdtrtinfo.fetts = 0;//胎次
                mdtrtinfo.fetus_cnt = 0;//胎儿数
                mdtrtinfo.pret_flag = "";//早产标志
                mdtrtinfo.birctrl_matn_date = "";//计划生育手术或生育日期
                mdtrtinfo.dise_type_code = "31";//病种类型代码

                List<Diseinfo> diseinfo = new List<Diseinfo>();//诊断信息
                for (int j = 0; j < zdbhs.Count; j++)
                {
                    Diseinfo dis = new Diseinfo();
                    dis.psn_no = grbh;//人员编号
                    dis.diag_type = zdlbs[j];//诊断类别
                    dis.maindiag_flag = zzdbss[j];//主诊断标志
                    dis.diag_srt_no = j + 1;//诊断序号
                    dis.diag_code = zdbhs[j];//诊断编号
                    dis.diag_name = zdmcs[j];//诊断名称
                    dis.adm_cond = ryzdqk;//入院病情
                    dis.diag_dept = ksbh;//诊断科室
                    dis.dise_dor_no = zzysbh;//诊断医生编号
                    dis.dise_dor_name = zzysmc;//诊断医生名称
                    dis.diag_time = sysdate;//诊断时间
                    diseinfo.Add(dis);
                }
                RegisterRequest register = new RegisterRequest()
                {
                    diseinfo = diseinfo,
                    mdtrtinfo = mdtrtinfo
                };
                string data = JsonConvert.SerializeObject(register);
                WriteLog(sysdate + "  " + jzlsh + " 进入住院医保登记...");
                WriteLog(sysdate + "  入参|" + data.ToString());
                int i = YBServiceRequest(Ywlx, data, ref res);

                #endregion


                #endregion
                if (i > 0)
                {
                    List<string> liSQL = new List<string>();
                    JObject jobj = JsonConvert.DeserializeObject<JObject>(res);
                    #region 返回参数
                    /*保险号|姓名|卡号|地区编号|地区名称|出生日期|实际年龄|参保日期|个人身份|单位名称|
                 * 性别|医疗人员类别|卡状态|账户余额|门诊(住院号)|医疗类别|科室名称|挂号费|本次看病次数|住院床号|
                 * 入院日期|入院时间|经办人
                 */
                    string bxh2 = bxh.Trim();   //保险号
                    string xm2 = xm.Trim();    //姓名
                    string kh2 = kh.Trim();    //卡号
                    string dqbh1 = dqbh.Trim();  //地区编号
                    string dqmc = "".Trim();   //地区名称
                    string csrq2 = csrq.Trim();   //出生日期
                    string sjnl = nl.Trim();   //实际年龄
                    string cbrq = cardInfo.idetinfo.begntime.Trim();   //参保日期
                    string grsf = cardInfo.idetinfo.psn_type_lv.Trim();   //个人身份
                    string dwmc2 = dwmc.Trim();   //单位名称
                    string xb2 = xb.Trim();    //性别
                    string rylb = cardInfo.insuinfo.psn_type.Trim();  //医疗人员类别
                    string kzt = rycbzt.Trim();   //卡状态
                    string grzhye = string.IsNullOrEmpty(cardInfo.insuinfo.balc) || cardInfo.insuinfo.balc == null || cardInfo.insuinfo.balc == "NULL" ? "0" : cardInfo.insuinfo.balc;//账户余额
                    string jzlsh2 = jzlsh.Trim();//门诊(住院号)
                    string yllb2 = yllb.Trim(); //医疗类别
                    string ksmc2 = ksmc.Trim(); //科室名称
                    string ghf = "".Trim();   //挂号费
                    string bckbcs = "".Trim();//本次看病次数
                    string zych2 = zych.Trim(); //住院床号
                    //string djrq = ryrq.Trim();  //入院日期
                    //string djsj = sysdate.Trim();  //入院时间
                    string djrq = string.IsNullOrEmpty(ryrq) || ryrq == null || ryrq == "NULL" ? zyrq : ryrq.Trim();//入院日期
                    string djsj = string.IsNullOrEmpty(sysdate) || sysdate == null || sysdate == "NULL" ? zysj : sysdate.Trim();//入院时间

                    //string djrq = string.IsNullOrEmpty(str2[20]) || str2[20] == null || str2[20] == "NULL" ? DateTime.Now.ToString("yyyyMMdd") : str2[20].Trim();//入院日期
                    //string djsj = string.IsNullOrEmpty(str2[21]) || str2[21] == null || str2[21] == "NULL" ? DateTime.Now.ToString("HHmm") : str2[21].Trim();//入院时间
                    string jbr1 = jbr.Trim();   //经办人
                    ghf = "0.00";                   //住院患者挂号费用0.00
                    #endregion

                    #region 医保病种暂时屏蔽
                    //if (str.Length > 1)
                    //{
                    //    for (int j = 1; j < str.Length; j++)
                    //    {
                    //        str2 = str[j].Split('|');
                    //        bzbm_r = str2[0].Trim();
                    //        bzmc_r = str2[1].Trim();
                    //        strSql = string.Format(@"insert into ybmxbdj (jzlsh,bxh,xm,kh,mmbzbm,mmbzmc) values(
                    //                           '{0}','{1}','{2}','{3}','{4}','{5}')",
                    //                                jzlsh, bxh2, xm2, kh2, bzbm_r, bzmc_r);
                    //        liSQL.Add(strSql);
                    //    }
                    //} 
                    #endregion

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
                    , z1ylnm = '{5}', z1bzno = '{6}', z1bznm = '{7}', z1ybno = '{8}' where z1comp = '{9}' and z1zyno = '{10}' "
                         , yldylb, tcqh, lyjgdm, lyjgmc, yllb, yllbmc, bzbm, bzmc, grbh, CliUtils.fSiteCode, jzlsh);

                    liSQL.Add(strSql);

                    object[] obj = liSQL.ToArray();
                    obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);
                    if (obj[1].ToString() == "1")
                    {
                        //string strID = string.Format(@"select * from jk02h where ID_NO='{0}'",sfzh);
                        //DataSet dsID = CliUtils.ExecuteSql("sybdj", "cmd", strID, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                        //if (dsID.Tables[0].Rows.Count > 0) 
                        //{
                        //    MessageBox.Show("该患者为精准扶贫人员！");
                        //}
                        //dsID.Dispose();
                        WriteLog(sysdate + "  " + jzlsh + " 进入住院医保登记成功|" + res.ToString());
                        return new object[] { 0, 1, "住院医保登记成功" };
                    }
                    else
                    {
                        WriteLog(sysdate + "  " + jzlsh + " 进入住院医保登记成功|操作本地数据失败|" + obj[2].ToString());
                        //住院登记（挂号）撤销
                        object[] objParam2 = new object[] { jzlsh, bxh2, xm2, kh2, dqbh1, jzlsh2 };
                        NYBZYDJCX(objParam2);
                        return new object[] { 0, 0, "住院医保登记失败" };
                    }
                }
                else
                {
                    //在医保登记失败情况下，修改缴费类别
                    List<string> liSQL = new List<string>();
                    strSql = string.Format(@"update zy01h set z1lyjg='07',z1lynm='全自费' where z1zyno='{0}'", jzlsh);
                    liSQL.Add(strSql);
                    object[] obj = liSQL.ToArray();
                    obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);

                    WriteLog(sysdate + "  " + jzlsh + " 进入住院医保登记失败|" + res.ToString());
                    return new object[] { 0, 0, "住院医保登记失败|" + res.ToString() };
                }
            }
            catch (Exception ex)
            {
                WriteLog(sysdate + "  住院登记异常|" + ex.Message);
                return new object[] { 0, 0, "住院登记异常|" + ex.Message + "异常行数：" + ex.StackTrace };
            }
        }
        #endregion

        #region 住院登记撤销（新医保接口）
        public static object[] YBZYDJCX(object[] objParam)
        {
            string sysdate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            try
            {
                string Ywlx = "2404";
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

                #region 老医保接口  
                //StringBuilder inputData = new StringBuilder();
                ////入参: 保险号|姓名|卡号|地区编号|住院号
                //inputData.Append(bxh + "|");
                //inputData.Append(xm + "|");
                //inputData.Append(kh + "|");
                //inputData.Append(dqbh + "|");
                //inputData.Append(ybjzlsh);

                //StringBuilder outData = new StringBuilder(1024);
                //StringBuilder retMsg = new StringBuilder(1024);

                //WriteLog(sysdate + "  " + jzlsh + " 进入住院医保登记撤销...");
                //WriteLog(sysdate + "  入参|" + inputData.ToString());
                //int i = f_UserBargaingApply(Ywlx, inputData, outData, retMsg);
                #endregion

                #region 新医保接口
                string res = string.Empty;
                string data = "{\"mdtrt_id\":\"{0}\",\"psn_no\":\"{1}\"}";
                data = string.Format(data, jzlsh, bxh);
                WriteLog(sysdate + "  " + jzlsh + " 进入住院医保登记撤销...");
                WriteLog(sysdate + "  入参|" + data.ToString());
                int i = YBServiceRequest(Ywlx, data, ref res);

                #endregion
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

                    strSql = string.Format(@"update zy01h set z1lyjg='07',z1lynm='全自费' where z1zyno='{0}'", jzlsh);
                    liSQL.Add(strSql);

                    object[] obj = liSQL.ToArray();
                    obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);
                    if (obj[1].ToString() == "1")
                    {
                        WriteLog(sysdate + "  " + jzlsh + " 进入住院医保登记撤销成功|" + res.ToString());
                        return new object[] { 0, 1, "住院登记撤销成功", res.ToString() };
                    }
                    else
                    {
                        WriteLog(sysdate + "  " + jzlsh + " 进入住院医保登记撤销成功|操作本地数据失败|" + obj[2].ToString());
                        return new object[] { 0, 0, "住院登记撤销失败|操作本地数据失败|" + obj[2].ToString() };
                    }
                }
                else
                {
                    WriteLog(sysdate + "  " + jzlsh + " 进入住院医保登记撤销失败|" + res.ToString());
                    return new object[] { 0, 0, "住院登记撤销失败|" + res.ToString() };
                }
            }
            catch (Exception ex)
            {
                WriteLog(sysdate + "  住院登记撤销异常|" + ex.Message);
                return new object[] { 0, 0, "住院登记撤销异常|" + ex.Message };
            }
        }
        #endregion


        #region 住院收费登记（新医保接口）
        public static object[] YBZYSFDJ(object[] objParam)
        {
            string sysdate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string jzlsh = objParam[0].ToString(); //就诊流水号
            try
            {
                object[] obj = YBZYFYMXSC(objParam);
                if (Convert.ToInt32(obj[1]) > 0)
                {
                    WriteLog(sysdate + "  住院收费登记成功！");
                    return new object[] { 0, 1, "住院收费登记成功！" };
                }
                else
                {
                    return new object[] { 0, 0, "住院收费登记失败！" };
                }
            }
            catch (Exception ex)
            {
                WriteLog(sysdate + "  住院收费登记失败异常|" + ex.Message);
                return new object[] { 0, 0, "住院收费登记失败异常|" + ex.Message };
            }
        }
        #endregion

        #region 住院收费登记撤销(全部)（新医保接口）
        public static object[] YBZYSFDJCX(object[] objParam)
        {
            string sysdate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            try
            {
                string Ywlx = "2205";
                string jzlsh = objParam[0].ToString();
                string bxh = "";
                string xm = "";
                string kh = "";
                string ybjzlsh = "";
                string jsid = "";//结算id

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

                #region 老医保接口
                //StringBuilder inputData = new StringBuilder();
                ////入参:保险号|姓名|卡号|地区编号|住院号
                //inputData.Append(bxh + "|");
                //inputData.Append(xm + "|");
                //inputData.Append(kh + "|");
                //inputData.Append(dqbh + "|");
                //inputData.Append(ybjzlsh + ";");
                //StringBuilder outData = new StringBuilder(1024);
                //StringBuilder retMsg = new StringBuilder(1024);
                //WriteLog(sysdate + "  " + jzlsh + " 进入住院费用明细撤销(全部)...");
                //int i = f_UserBargaingApply(Ywlx, inputData, outData, retMsg); 
                #endregion
                #region 新医保接口 住院收费撤销  by hjw
                string res = string.Empty;
                string data = "{\"ipt_otp_no\":\"" + ybjzlsh + "\",\"rxno\":\"\",\"feedetl_sn\":\"\"}";
                WriteLog(sysdate + "  " + jzlsh + " 进入住院费用明细撤销(全部)...");
                int i = YBServiceRequest(Ywlx, data, ref res);
                #endregion
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
                        WriteLog(sysdate + "  " + jzlsh + " 进入住院费用明细撤销(全部)成功|" + res.ToString());
                        return new object[] { 0, 1, "住院收费退费成功", res.ToString() };
                    }
                    else
                    {
                        WriteLog(sysdate + "  " + jzlsh + " 进入住院费用明细撤销(全部)成功|操作本地数据失败|" + obj[2].ToString());
                        return new object[] { 0, 0, "住院收费退费失败|" + obj[2].ToString() };
                    }
                }
                else
                {
                    WriteLog(sysdate + "  " + jzlsh + " 进入住院费用明细撤销(全部)失败|" + res.ToString());
                    return new object[] { 0, 0, "住院收费退费失败|" + res.ToString() };
                }
            }
            catch (Exception ex)
            {
                WriteLog(sysdate + "  住院收费登记撤销异常|" + ex.Message);
                return new object[] { 0, 0, "住院收费登记撤销异常|" + ex.Message };
            }
        }
        #endregion

        #region 住院费用明细上传（新医保）
        #region 分布式上传
        /// <summary>
        /// 使用分布式算法进行分段上传
        /// </summary>
        /// <param name="start">开始索引</param>
        /// <param name="end">结束索引</param>
        /// <param name="ds">数据集</param>
        /// <param name="jzlsh">住院流水号</param>
        /// <param name="yllb">医疗类别代码</param>
        /// <returns></returns>
        private static object[] UpLoadByGroup(int start, int end, DataSet ds, string jzlsh, string yllb)
        {
            string bxh = "";    //保险号
            string xm = "";     //姓名
            string kh = "";     //卡号
            string dgysxm = ""; //定岗医生姓名
            string ybjzlsh = "";//医保就诊流水号
            string zych = "";
            string dgysdm = "";
            string sfzh = "";//身份证号
            DateTime ryrq_tmp;
            decimal sumcost = 0;//费用总额 
            string ksno = "";//科室id
            string ksmc = "";//科室名称
            string bzbm = "";//病种编码
            string bzmc = "";//病种名称
            bxh = dsinfo.Tables[0].Rows[0]["grbh"].ToString();
            xm = dsinfo.Tables[0].Rows[0]["xm"].ToString();
            kh = dsinfo.Tables[0].Rows[0]["kh"].ToString();
            dqbh = dsinfo.Tables[0].Rows[0]["tcqh"].ToString();
            dgysdm = string.IsNullOrEmpty(dsinfo.Tables[0].Rows[0]["dgysdm"].ToString()) ? "010091" : dsinfo.Tables[0].Rows[0]["dgysdm"].ToString();
            dgysxm = string.IsNullOrEmpty(dsinfo.Tables[0].Rows[0]["dgysxm"].ToString()) ? "江辉" : dsinfo.Tables[0].Rows[0]["dgysxm"].ToString();
            ybjzlsh = dsinfo.Tables[0].Rows[0]["ybjzlsh"].ToString();
            ryrq_tmp = Convert.ToDateTime(dsinfo.Tables[0].Rows[0]["ryrq"]);
            zych = dsinfo.Tables[0].Rows[0]["z1bedn"].ToString();
            sfzh = dsinfo.Tables[0].Rows[0]["sfzh"].ToString();
            ksno = dsinfo.Tables[0].Rows[0]["ksbh"].ToString();
            ksmc = dsinfo.Tables[0].Rows[0]["ksmc"].ToString();
            bzbm = dsinfo.Tables[0].Rows[0]["bzbm"].ToString();
            bzmc = dsinfo.Tables[0].Rows[0]["bzmc"].ToString();
            string sysdate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            List<OutpatientFee> feeinfos = new List<OutpatientFee>();
            string ywlx = "2204";
            StringBuilder sb = new StringBuilder();
            string li_yyxmbh = "";
            string li_cfh = "";
            string li_yyxmmc = "";
            string li_yyxmdj = "";
            string li_cfrq = "";
            string li_yyrq = "";
            string jylsh = jzlsh + DateTime.Now.ToString("yyyyMMddHHmmss");
            for (int f = start; f < end; f++)
            {
                DataRow dr = ds.Tables[0].Rows[f];
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
                string yyxmxx = dr["yyxmbh"].ToString() + "&" + dr["yyxmmc"].ToString() + "&" + zych;
                StringBuilder inputData2 = new StringBuilder();
                inputData2.Append(ybxmbh + "|");
                inputData2.Append(ybxmmc + "|");
                inputData2.Append(dj + "|");
                inputData2.Append(sl + "|");
                inputData2.Append(je + "|");
                inputData2.Append(yysj + "|");
                inputData2.Append(yyxmxx + ";");
                //li_dj += f == end - 1 ? dj + "" : dj + "|";
                li_cfh += f == end - 1 ? dr["cfh"].ToString() : dr["cfh"].ToString() + "|";
                li_yyxmbh += f == end - 1 ? ybxmbh : ybxmbh + "|";
                li_yyxmmc += f == end - 1 ? ybxmmc : ybxmmc + "|";
                //li_sl += f == end - 1 ? sl+"" : sl + "|";
                //li_je += f == end - 1 ? je+"" : je + "|";
                li_cfrq += f == end - 1 ? dr["yysj"].ToString() : dr["yysj"].ToString() + "|";
                li_yyrq += f == end - 1 ? dr["yysj"].ToString() : dr["yysj"].ToString() + "|";
                if (dr["ybxmbh"] == DBNull.Value)
                {
                    sb.Append("项目代码：[" + dr["yyxmbh"].ToString() + "]，名称：[" + dr["yyxmmc"].ToString() + "]未对照，不能上传！\r\n");
                }
                else
                {

                    //OutPatientFeeNew feeinfo = new OutPatientFeeNew();
                    //feeinfo.ipt_otp_no = ybjzlsh;
                    //feeinfo.list_type = dr["ybsfxmzldm"].ToString();
                    //feeinfo.rxno = dr["xmlx"].ToString();
                    //feeinfo.feedetl_sn = dr["z3yzxh"].ToString();
                    //feeinfo.fee_ocur_time = Convert.ToDateTime(dr["yysj"].ToString()).ToString("yyyyMMddhhmmss");
                    //feeinfo.med_list_codg = dr["ybxmbh"].ToString();
                    //feeinfo.pric = Convert.ToDecimal(dr["dj"].ToString());
                    //feeinfo.cnt = Convert.ToDecimal(dr["sl"].ToString());
                    //feeinfo.umamt = Convert.ToDecimal(dr["je"].ToString());
                    //feeinfo.bilg_dr_codg = dr["z3empn"].ToString();
                    //feeinfo.bilg_dept_codg = dr["z3ksno"].ToString();
                    //feeinfo.min_unit = "";
                    //feeinfos.Add(feeinfo);
                    OutpatientFee fee = new OutpatientFee()
                    {
                        feedetl_sn = dr["cfh"].ToString() + dr["z3yzxh"].ToString(),//费用明细流水号
                        mdtrt_id = ybjzlsh,//就诊id
                        psn_no = bxh,//个人编号
                        chrg_bchno = dr["sfno"].ToString(),//收费批次号
                        dise_codg = bzbm,//病种编码
                        rxno = dr["cfh"].ToString(),//处方号
                        rx_circ_flag = "",//外购处方标识 字典
                        fee_ocur_time = Convert.ToDateTime(dr["yysj"].ToString()).ToString("yyyyMMddhhmmss"),//费用时间
                        med_list_codg = dr["ybxmbh"].ToString(),//医疗中心编码
                        medins_list_codg = dr["yyxmbh"].ToString(),//医药机构目录编码
                        det_item_fee_sumamt = Decimal.Parse(dr["je"].ToString()),//明细项目费用总额
                        cnt = Decimal.Parse(dr["sl"].ToString()),//数量
                        pric = Decimal.Parse(dr["dj"].ToString()),//单价
                        sin_dos_dscr = dr["dcjl"].ToString(),//单次剂量描述
                        used_frqu_dscr = dr["pc"].ToString(),//使用频次描述
                        prd_days = 1,//周期天数
                        medc_way_dscr = dr["yf"].ToString(),//用药途径描述
                        bilg_dept_codg = ksno,//开单科室编码
                        bilg_dept_name = ksmc,//开单科室编码
                        bilg_dr_codg = dgysdm,//开单医生名称
                        bilg_dr_name = dgysxm,//开单医生名称
                        acord_dept_codg = dr["zxks"].ToString(),//受单科室编码
                        acord_dept_name = GetDeptNameById(dr["zxks"].ToString()),//受单科室名称
                        orders_dr_code = dr["zxr"].ToString(),//受单医生编码
                        orders_dr_name = GetEmplNameById(dr["zxr"].ToString()),//受单医生名称
                        hosp_appr_flag = "1",//医院审批标识 字典
                        tcmdrug_used_way = "",//中药使用方式 字典
                        etip_flag = "",//外检标识 字典
                        etip_hosp_code = "",//外检医院编码
                        dscg_tkdrug_flag = "",//出院带药标识 字典
                        matn_fee_flag = "" //生育费用标志 字典

                    };
                    feeinfos.Add(fee);
                    feelists.Add(fee);
                }

            }
            if (!string.IsNullOrEmpty(sb.ToString()))
            {
                WriteLog(sb.ToString());
                return new object[] { 0, 0, sb.ToString() };
            }
            string res = string.Empty;
            string data = JsonConvert.SerializeObject(feeinfos);
            if (feeinfos == null)
            {
                return new object[] { 0, 0, "入参传入出现错误" + data };
            }
            if (feeinfos.Count == 0)
            {
                return new object[] { 0, 0, "请传入上传的费用明细" + data };
            }
            feelists = feeinfos;

            WriteLog(sysdate + "|住院费用明细上传|");

            WriteLog(sysdate + "|入参|" + data);
            int i = YBServiceRequest(ywlx, data, ref res);
            WriteLog(sysdate + " 分段  出参|" + res.ToString());
            if (i > 0)
            {
                List<OutpatientFeeOutParam> cfmxscfhList = JsonConvert.DeserializeObject<List<OutpatientFeeOutParam>>(res);
                for (int m = 0; m < cfmxscfhList.Count; m++)
                {
                    OutpatientFeeOutParam info = cfmxscfhList[m];
                    //出参:处方号 |处方流水号 |处方日期|收费项目中心编码|总金额 |自费金额|超限价金额|支付上限|收费项目等级|说明信息
                    string jzlsh2 = jzlsh; //住院号


                    string ybxmmc = li_yyxmmc.Split('|')[m].Trim(); //项目名称
                    string xmdj = info.chrgitm_lv.Trim();   //项目等级
                    string sflb = info.med_chrgitm_type;   //收费类别
                    string dj2 = info.pric.ToString().Trim();    //单价
                                                                 //if (ybxmbm.Equals("X-A02BC-L033-E005"))
                                                                 //   MessageBox.Show("111" + dj2);
                    string sl2 = "1";    //数量


                    string scsj = sysdate;  //处方上传时间

                    #region 回参的数据
                    string ybcfh = li_cfh.Split('|')[m];  //处方号
                    string ybcflsh = info.feedetl_sn;//处方流水号  新加
                    string yyrq = li_yyrq.Split('|')[m];   //处方日期
                    string ybxmbm = li_yyxmbh.Split('|')[m]; //项目编号
                    string je2 = info.det_item_fee_sumamt.ToString().Trim();    //金额
                    string zfje = info.fulamt_ownpay_amt.ToString();//自费金额  新加
                    string cxjje = info.overlmt_amt.ToString();//超限价金额 新加
                    string zfsx = info.pric_uplmt_amt.ToString();//支付上限 新加
                    string memo = info.memo;//说明信息  新加
                    #endregion

                    List<string> liSQL = new List<string>();
                    for (int j = 0; j < li_yyxmbh.Split('|').Length; j++)
                    {
                        if (li_yyxmbh.Split('|')[j].Equals(ybxmbm) && Math.Round(Convert.ToDecimal(dj2), 2, MidpointRounding.AwayFromZero) == Math.Round(Convert.ToDecimal(info.det_item_fee_sumamt), 2, MidpointRounding.AwayFromZero))
                        //if (li_ybxmbh[j].Equals(ybxmbm) && Convert.ToDecimal(dj2) == Math.Round(Convert.ToDecimal(li_dj[j]), 2, MidpointRounding.AwayFromZero))
                        // if (li_ybxmbh[j].Equals(ybxmbm) && li_dj[j].Equals(Convert.ToDouble(dj2).ToString()))
                        {
                            //MessageBox.Show(li_dj[j] + "||" + Convert.ToDouble(dj2).ToString());
                            string strSql = string.Format(@"insert into ybcfmxscindr(jzlsh,jylsh,je,xm,kh,yysfxmbm,yysfxmmc,sysdate,ybjzlsh,ybcfh,sfxmzxbm,
                                                        sfxmzxmc,sfxmzl,sflb,dj,sl,cfrq,cfscsj,cxbz,zfje,remark,ybcflsh,cxjje,zfsx) values(
                                                        '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}',
                                                        '{11}','{12}','{13}','{14}','{15}','{16}','{17}','1','{18}','{19}','{20}','{21}','{22}')",
                                                      jzlsh, jylsh, je2, xm, kh, li_yyxmbh[j], li_yyxmmc[j], sysdate, ybjzlsh, ybcfh, ybxmbm,
                                                      ybxmmc, xmdj, sflb, dj2, sl2, yyrq, scsj, zfje, memo, ybcflsh, cxjje, zfsx);
                            liSQL.Add(strSql);
                            WriteLog(sysdate + "  " + jzlsh + "上传处方明细-->" + li_yyxmbh[j] + "|" + li_yyxmmc[j] + "|" + res);
                            break;
                        }
                    }
                    object[] obj = liSQL.ToArray();
                    obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);
                    if (obj[1].ToString().Equals("1"))
                    {
                        WriteLog(sysdate + "  上传处方明细|" + "");
                        return new object[] { 0, 1, "上传处方明细成功！" };
                    }
                    else
                    {
                        WriteLog(sysdate + "  上传处方明细|保存本地数据失败|" + obj[2].ToString());
                        return new object[] { 0, 0, "上传处方明细｜保存本地数据失败｜" + obj[2].ToString() };
                    }
                }

                WriteLog(sysdate + $"住院费用明细{start}~{end}条上传成功！" + res);
                return new object[] { 0, 1, $"住院费用明细{start}~{end}条上传成功！" };
            }
            else
            {
                feelists.Clear();
                WriteLog(sysdate + $"住院费用明细{start}~{end}条上传失败！" + res);
                return new object[] { 0, 1, $"住院费用明细{start}~{end}条上传失败！" };
            }
        }
        #endregion
        /// <summary>
        /// 医保个人信息
        /// </summary>
        private static DataSet dsinfo = new DataSet();
        /// <summary>
        /// 医保明细上传
        /// </summary>
        /// <param name="inParams"></param>
        /// <returns></returns>
        private static object[] YBZYFYMXSC(object[] inParams)
        {
            string sysdate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string jzlsh = inParams[0].ToString(); //就诊流水号
            string sql = string.Format(@"select * from zy03d y where y.z3zyno='{0}'", jzlsh);
            //sql = string.Format(sql, jzlsh);
            DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", sql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            int status = 1;
            int mcxcsl = 0;
            List<string> liSql = new List<string>();
            try
            {
                if (inParams.Length > 2)
                {
                    mcxcsl = int.Parse(inParams[2].ToString()); //每次上传数量
                }
                else
                    mcxcsl = 30; //每次上传数量
            }
            catch (Exception ex)
            {
                WriteLog(sysdate + "  " + jzlsh + " 设置每次上传数量失败|" + "上传数量不对");
                return new object[] { 0, 0, "住院收费登记失败|" + "设置的每次上传数量不对" };
            }

            string bxh = "";    //保险号
            string xm = "";     //姓名
            string kh = "";     //卡号
            string dgysxm = ""; //定岗医生姓名
            string ybjzlsh = "";//医保就诊流水号
            string zych = "";
            string dgysdm = "";
            string sfzh = "";//身份证号
            string yllb = ""; //医疗类别
            DateTime ryrq_tmp;
            decimal sumcost = 0;//费用总额

            if (string.IsNullOrEmpty(jzlsh))
                return new object[] { 0, 0, "就诊流水号不能为空" };

            string jylsh = jzlsh + DateTime.Now.ToString("yyyyMMddHHmmss");


            #region 判断是否医保登记
            string strSql = string.Format(@"select convert(date,ghdjsj) as ryrq, isnull(ybbfh,z1cwid) as z1bedn,c.* from ybmzzydjdr c  left join zy01h a on jzlsh=z1zyno
                    left join ybbfdzdr b on a.z1cwid=bedno and a.z1bqxx=b.bqno and a.z1ksno=b.ksno where jzlsh='{0}' and cxbz=1", jzlsh);
            DataSet ds1 = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            if (ds.Tables[0].Rows.Count == 0)
                return new object[] { 0, 0, "该患者未办理医保登记" };
            else
            {
                dsinfo = ds1;
                bxh = ds1.Tables[0].Rows[0]["grbh"].ToString();
                xm = ds1.Tables[0].Rows[0]["xm"].ToString();
                kh = ds1.Tables[0].Rows[0]["kh"].ToString();
                dqbh = ds1.Tables[0].Rows[0]["tcqh"].ToString();
                dgysdm = string.IsNullOrEmpty(ds1.Tables[0].Rows[0]["dgysdm"].ToString()) ? "010091" : ds1.Tables[0].Rows[0]["dgysdm"].ToString();
                dgysxm = string.IsNullOrEmpty(ds1.Tables[0].Rows[0]["dgysxm"].ToString()) ? "江辉" : ds1.Tables[0].Rows[0]["dgysxm"].ToString();
                ybjzlsh = ds1.Tables[0].Rows[0]["ybjzlsh"].ToString();
                ryrq_tmp = Convert.ToDateTime(ds1.Tables[0].Rows[0]["ryrq"]);
                zych = ds1.Tables[0].Rows[0]["z1bedn"].ToString();
                sfzh = ds1.Tables[0].Rows[0]["sfzh"].ToString();
                yllb = ds1.Tables[0].Rows[0]["yllb"].ToString();
            }
            #endregion

            #region 医保获取个人信息
            //ReadCardInfo cardInfo = GetYbCardInfo(new object[] { "02", sfzh, "", "", "1", sfzh, xm });
            //if (cardInfo == null)
            //{
            //    return new object[] { 0, 0, "获取医保个人信息出错" };
            //}
            #endregion
            #region 收费明细信息 
            #region Sql语句
            //string Sql = string.Format(@"select b.ybxmbh,b.ybxmmc,a.z3djxx as dj,
            //                            sum(case LEFT(a.z3endv,1) when '4' then -a.z3jzcs else a.z3jzcs end) as sl,
            //                            sum(case LEFT(a.z3endv,1) when '4' then -a.z3jzje else a.z3jzje end) as je,
            //                            a.z3item as yyxmbh, a.z3name as yyxmmc,max(a.z3date) as yysj, z3sfno as sfno, 
            //                            b.sfxmzldm as ybsfxmzldm, b.sflbdm as ybsflbdm,b.sfxmdjdm as xmlx,z3yzxh,z3empn,z3ksno
            //                            from zy03d a left join ybhisdzdr b on 
            //                            a.z3item=b.hisxmbh where a.z3ybup is null and LEFT(a.z3kind,1)in(2,4,5)  and a.z3zyno='{0}' 
            //                            group by b.ybxmbh,b.ybxmmc,a.z3djxx,a.z3item,a.z3name,a.z3sfno,b.sfxmzldm,b.sflbdm,b.sfxmdjdm,z3yzxh,z3empn,z3ksno
            //                            having sum(case LEFT(a.z3endv,1) when '4' then -a.z3jzcs else a.z3jzcs end)>0", jzlsh);
            #endregion

            #region Sql语句
            string Sql = string.Format(@"select 
										 bb.ybxmbh as ybxmbh,
										 bb.ybxmmc as ybxmmc, 
										 bb.dj as dj,
										 sum(bb.sl) as sl,
										 sum(bb.je) as je,
										 bb.yyxmbh as yyxmbh,
										 bb.yyxmmc as yyxmmc,
										 max(bb.yysj) as yysj, 
										 bb.sfno as sfno,
										 bb.ybsfxmzldm as ybsfxmzldm,
										 bb.ybsflbdm as ybsflbdm,
										 bb.xmlx as xmlx,
										 bb.z3yzxh as z3yzxh,
										 bb.z3empn as z3empn,
										 bb.zxks as z3ksno,
										 bb.cfh as cfh,
										 bb.zxr as zxr,
										 bb.yf as yf,
										 bb.pc as pc,
										 bb.dcjl as dcjl
										 from
										(select b.ybxmbh,b.ybxmmc,a.z3djxx as dj,
                                        case LEFT(a.z3endv,1) when '4' then -a.z3jzcs else a.z3jzcs end as sl,
                                        case LEFT(a.z3endv,1) when '4' then -a.z3jzje else a.z3jzje end as je,
                                        a.z3item as yyxmbh, a.z3name as yyxmmc,a.z3date as yysj, z3sfno as sfno, 
                                        b.sfxmzldm as ybsfxmzldm, b.sflbdm as ybsflbdm,b.sfxmdjdm as xmlx,z3yzxh,z3empn,z3zxks as zxks,
										(select top 1 h.z4yzno  from zy04h h where h.z4ghno=a.z3ghno and  h.z4sequ=a.z3yzxh  ) as cfh, 
										a.z3zxys as zxr,
										(select top 1 h.z4ypyf  from zy04h h where h.z4ghno=a.z3ghno and  h.z4sequ=a.z3yzxh  ) as yf,
										(select top 1 h.z4pdxx  from zy04h h where h.z4ghno=a.z3ghno and  h.z4sequ=a.z3yzxh ) as pc,
										(select top 1 h.z4jlxx  from zy04h h where h.z4ghno=a.z3ghno and  h.z4sequ=a.z3yzxh ) as dcjl
                                        from zy03d a left join ybhisdzdr b on   a.z3item=b.hisxmbh 
										 where a.z3ybup is null and LEFT(a.z3kind,1)in(2,4,5)  and a.z3ghno='{0}') bb
                                        group by bb.ybxmbh,bb.ybxmmc,bb.dj,bb.yyxmbh,bb.yyxmmc,bb.sfno,bb.ybsfxmzldm,bb.ybsflbdm,bb.xmlx,bb.z3yzxh,bb.z3empn,bb.zxks,bb.cfh,bb.zxr,bb.yf,bb.pc,bb.dcjl
                                        having sum(bb.je)>0", jzlsh);
            #endregion
            ds.Tables.Clear();
            ds = CliUtils.ExecuteSql("sybdj", "cmd", Sql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            if (ds.Tables[0].Rows.Count > 0)
            {
                #region 老医保接口的写法
                //StringBuilder strMsg = new StringBuilder();
                //foreach (DataRow dr in ds.Tables[0].Rows)
                //{

                //    if (dr["ybxmbh"] == DBNull.Value)
                //    {
                //        strMsg.Append("项目代码：[" + dr["yyxmbh"].ToString() + "]，名称：[" + dr["yyxmmc"].ToString() + "]未对照，不能上传！\r\n");
                //    }
                //    else
                //    {
                //        string yysj = "";
                //        if (DateTime.Compare(ryrq_tmp, yyrq) > 1)
                //            yysj = ryrq_tmp.ToString("yyyyMMdd");
                //        else
                //            yysj = yyrq.ToString("yyyyMMdd");
                //        string yyxmxx = dr["yyxmbh"].ToString() + "&" + dr["yyxmmc"].ToString() + "&" + zych;
                //        StringBuilder inputData2 = new StringBuilder();
                //        inputData2.Append(ybxmbh + "|");
                //        inputData2.Append(ybxmmc + "|");
                //        inputData2.Append(dj + "|");
                //        inputData2.Append(sl + "|");
                //        inputData2.Append(je + "|");
                //        inputData2.Append(yysj + "|");
                //        inputData2.Append(yyxmxx + ";");
                //        li_inputData.Add(inputData2.ToString());
                //        li_yyxmbh.Add(dr["yyxmbh"].ToString());
                //        li_yyxmmc.Add(dr["yyxmmc"].ToString());
                //        li_ybxmbh.Add(dr["ybxmbh"].ToString());
                //        //li_sn.Add(dr["sn"].ToString());
                //        li_dj.Add(dj.ToString());


                //    }

                //    if (true)
                //    {

                //    }
                //} 
                #endregion

                #region 新医保接口分段上传 （分布式算法） by  hjw
                int sumCount = ds.Tables[0].Rows.Count;
                //int count = 50;

                #region  利用分布式进行分组上传  
                if (sumCount > mcxcsl)
                {
                    int xhcount = sumCount % mcxcsl > 0 ? (sumCount / mcxcsl) + 1 : sumCount / mcxcsl;
                    for (int i = 0; i < xhcount; i++)
                    {
                        int start = i * mcxcsl;
                        int end = (i + 1) * mcxcsl > sumCount ? sumCount : (i + 1) * mcxcsl;
                        object[] objresult = UpLoadByGroup(start, end, ds, jzlsh, yllb);
                        if (int.Parse(objresult[1].ToString()) > 0)
                        {
                            status = 1;
                        }
                        else
                        {
                            status = -1;
                        }
                    }
                }
                #endregion
                else
                {
                    //如果费用明细数量少于每组的数量就一次性上传
                    int start = 0;
                    int end = sumCount;
                    object[] objresult = UpLoadByGroup(start, end, ds, jzlsh, yllb);
                    if (int.Parse(objresult[1].ToString()) > 0)
                    {
                        status = 1;
                    }
                    else
                    {
                        status = -1;
                    }
                }
                #endregion
            }
            else
                return new object[] { 0, 0, "无费用明细" };
            #endregion
            if (status == 1)
            {
                strSql = string.Format(@"update zy03d set z3ybup = '{0}' where z3ybup is null and LEFT(z3kind,1)=2 and z3zyno = '{1}' ", ybjzlsh, jzlsh);
                liSql.Add(strSql);
                strSql = string.Format(@"update zy03dz set z3ybup = '{0}' where z3ybup is null and LEFT(z3kind,1)=2 and z3zyno = '{1}' ", ybjzlsh, jzlsh);
                liSql.Add(strSql);
                object[] obj = liSql.ToArray();
                obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);
                if (obj[1].ToString() == "1")
                {
                    WriteLog(sysdate + "  " + jzlsh + " 进入住院费用明细上传成功｜操作本地数据库成功|");
                    return new object[] { 0, 1, "住院费用明细上传成功" };
                }
                else
                {
                    WriteLog(sysdate + "  " + jzlsh + " 进入住院费用明细上传成功｜操作本地数据库失败|" + obj[2].ToString());
                    object[] objParam2 = new object[] { jzlsh, bxh, xm, kh, ybjzlsh };
                    NYBZYSFDJCX(objParam2);
                    return new object[] { 0, 0, "住院费用明细上传成功,操作本地数据库失败!" };
                }
            }
            else
            {
                WriteLog(sysdate + "住院费用明细上传失败！");
                return new object[] { 0, 1, "住院费用明细上传失败！" };
            }
        }
        #endregion

        #region 住院费用明细上传撤销(新医保)
        /// <summary>
        /// 住院费用明细上传撤销
        /// </summary>
        /// <param name="feedetl_sn">费用明细流水号</param>
        /// <param name="mdtrt_id">就诊ID</param>
        /// <param name="psn_no">人员编号</param>
        /// <returns></returns>
        private static object[] ZYFYMXSCCX(string feedetl_sn, string mdtrt_id, string psn_no)
        {
            string sysdate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string Ywlx = "2302";

            if (string.IsNullOrEmpty(feedetl_sn))
            {
                return new object[] { 0, 0, "费用明细流水号不得为空" };
            }
            if (string.IsNullOrEmpty(mdtrt_id))
            {
                return new object[] { 0, 0, "就诊id不得为空" };
            }
            if (string.IsNullOrEmpty(psn_no))
            {
                return new object[] { 0, 0, "人员编号不得为空" };
            }
            string data = "{\"feedetl_sn\":\"" + feedetl_sn + "\",\"mdtrt_id\":\"" + mdtrt_id + "\",\"psn_no\":\"" + psn_no + "\"}";
            data = string.Format(data, feedetl_sn, mdtrt_id, psn_no);
            WriteLog(sysdate + "|住院费用明细上传撤销|");
            WriteLog(sysdate + "|入参|" + data);
            string res = "";
            int i = YBServiceRequest(Ywlx, data, ref res);

            if (i > 0)
            {
                return new object[] { 0, 1, res };
            }
            else
            {
                return new object[] { 0, 0, "住院费用明细上传撤销失败" + res };
            }
        }
        #endregion

        #region 住院费用明细批量撤销
        private static (int code, string Res) RollbackZYFY(List<OutpatientFee> fees)
        {
            int code = 1; string Res = string.Empty;
            foreach (OutpatientFee item in fees)
            {
                object[] resobj = ZYFYMXSCCX(item.feedetl_sn, item.mdtrt_id, item.psn_no);
                if (resobj[1].ToString() == "0")
                {
                    code = -1;
                    Res = resobj[2].ToString();
                }
                else
                {
                    code = 1;
                    Res = resobj[2].ToString();
                }
            }
            return (code, Res);
        }
        #endregion

        #region 住院收费预结算（新医保接口）
        public static object[] YBZYSFYJS(object[] objParam)
        {
            string sysdate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            try
            {
                string Ywlx = "2303";
                string yllb = "";
                string jzlsh = objParam[0].ToString();      //就诊流水号
                string cyyy = objParam[1].ToString();       //出院原因 9
                string zhsybz = objParam[2].ToString();     //账户使用标志 1
                string ztjsbz = objParam[3].ToString();     //中途结算标志 0 
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
                string sfzh = "";//身份证号
                string jbr = CliUtils.fLoginUser;
                string kzt1 = "";//卡状态
                string deptName = "";//科室名称
                string jzcs = "";//就诊次数
                string ZYCH = "";//住院床号
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
                DateTime dt_cyrq = DateTime.Now;
                try
                {
                    dt_cyrq = Convert.ToDateTime(cyrqsj);
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
                    sfzh = ds.Tables[0].Rows[0]["sfzh"].ToString();
                    yllb = ds.Tables[0].Rows[0]["yllb"].ToString();
                    kzt1 = ds.Tables[0].Rows[0]["rycbzt"].ToString();
                    deptName = ds.Tables[0].Rows[0]["ksmc"].ToString();
                    jzcs = ds.Tables[0].Rows[0]["zycs"].ToString();
                    ZYCH = ds.Tables[0].Rows[0]["cwh"].ToString();
                    if (yllb == "21" || yllb == "26")
                    {
                        bzbh = "NULL";
                        bzmc = "NULL";
                        cyzd = ds.Tables[0].Rows[0]["bzmc"].ToString();
                    }
                    else
                    {
                        bzbh = ds.Tables[0].Rows[0]["bzbm"].ToString();
                        bzmc = ds.Tables[0].Rows[0]["bzmc"].ToString();
                        cyzd = ds.Tables[0].Rows[0]["bzmc"].ToString();
                    }
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

                #region 20200922 主动发送读卡交易 
                strSql = string.Format(@"select kh from YBICKXX where grbh='{0}' and datediff(minute, sysDate,GETDATE()) > 120 ", bxh);
                ds.Tables.Clear();
                ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                if (ds.Tables[0].Rows.Count > 0)
                //{
                //    YBMZDK(null);
                //    kh = dzkh;
                //}
                //else
                {
                    kh = ds.Tables[0].Rows[0]["kh"].ToString();
                }
                #endregion

                #region 医保获取个人信息
                ReadCardInfo cardInfo = GetYbCardInfo(new object[] { "02", sfzh, "", "", "1", sfzh, xm });
                if (cardInfo == null)
                {
                    return new object[] { 0, 0, "获取医保个人信息出错" };
                }
                #endregion

                #region 新医保接口  住院预结算  by hjw
                string res = string.Empty;
                string data = "{\"psn_no\":\"" + bxh + "\",\"mdtrt_cert_type\":\"" + "02" + "\",\"mdtrt_cert_no\":\"" + sfzh + "\",\"card_sn\":\"\",\"psn_cert_type\":\"01\",\"certno\":\"" + sfzh + "\",\"psn_type\":\"" + cardInfo.insuinfo.psn_type + "\",\"psn_name\":\"" + xm + "\",\"medfee_sumamt\":\"" + sfje3 + "\",\"psn_setlway\":\"01\",\"mdtrt_id\":\"" + jzlsh + "\",\"acct_used_flag\":\"1\",\"insutype\":\"" + cardInfo.insuinfo.insutype + "\",\"invono\":\"" + djh + "\",\"mid_setl_flag\":\"0\",\"fulamt_ownpay_amt\":\"0\",\"overlmt_selfpay\":\"0\",\"preselfpay_amt\":\"0\",\"inscp_scp_amt\":\"0\",\"dscgTime\":\"" + dt_cyrq.ToString("yyyy-MM-dd") + "\"}";
                //data = string.Format(data, bxh, "03", kh, sfje3, "01", jzlsh, "0", cardInfo.insuinfo.insutype, ybjzlsh);
                WriteLog(sysdate + "  " + jzlsh + " 进入住院收费预结算...");
                WriteLog(sysdate + " 入参|" + data.ToString());
                int i = YBServiceRequest(Ywlx, data, ref res);
                WriteLog(sysdate + " 出参|" + res.ToString());
                #endregion
                if (i > 0)
                {
                    List<string> liDqbh = new List<string>();//赣州所有县地区编号

                    #region 赣州地区编号
                    //360701	【赣州市】
                    //360721	【赣县】
                    //360733	【会昌县】
                    //360723	【大余县】
                    //360734	【寻乌县】
                    //360782	【南康市】
                    //360730	【宁都县】
                    //360725	【崇义县】
                    //360726	【安远县】
                    //360732	【兴国县】
                    //360728	【定南县】
                    //360724	【上犹县】
                    //360735	【石城县】
                    //360722	【信丰县】
                    //360729	【全南县】
                    //360727	【龙南县】
                    //360781	【瑞金市】
                    //360731	【于都县】
                    //360702	【章贡区】
                    //360703    【开发区】
                    //360704    【蓉江新区】
                    //返回其他值 【异地就医】
                    liDqbh.Add("360701");
                    liDqbh.Add("360721");
                    liDqbh.Add("360733");
                    liDqbh.Add("360723");
                    liDqbh.Add("360734");
                    liDqbh.Add("360782");
                    liDqbh.Add("360730");
                    liDqbh.Add("360725");
                    liDqbh.Add("360726");
                    liDqbh.Add("360732");
                    liDqbh.Add("360728");
                    liDqbh.Add("360724");
                    liDqbh.Add("360735");
                    liDqbh.Add("360722");
                    liDqbh.Add("360729");
                    liDqbh.Add("360727");
                    liDqbh.Add("360781");
                    liDqbh.Add("360731");
                    liDqbh.Add("360702");
                    liDqbh.Add("360703");
                    liDqbh.Add("360704");
                    #endregion

                    #region 出参
                    //string[] str = outData.ToString().Split(';');
                    YBBalanceInfo ybinfo = JsonConvert.DeserializeObject<YBBalanceInfo>(res);
                    ParamFYJS js = new ParamFYJS();
                    //    string[] str2 = str[0].Split('|');
                    js.Grbh = bxh.Trim();       //保险号
                    js.Xm = xm.Trim();        //姓名
                    js.Kh = kh.Trim();        //卡号
                    js.Dqbh = dqbh.Trim();      //地区编号
                    js.Dqmc = "".Trim();       //地区名称
                    js.Csrq = cardInfo.baseinfo.brdy.Trim();       //出生日期
                    string sjnl = cardInfo.baseinfo.age.Trim();       //实际年龄
                    js.Cbrq = cardInfo.idetinfo.begntime.Trim();       //参保日期
                    string grsf = cardInfo.idetinfo.psn_type_lv.Trim();       //个人身份
                    js.Dwmc = cardInfo.insuinfo.emp_name.Trim();       //单位名称
                    js.Xb = cardInfo.baseinfo.gend.Trim();        //性别
                    js.Yldylb = cardInfo.insuinfo.psn_type.Trim();    //医疗人员类别
                    string kzt = kzt1.Trim();       //卡状态
                    js.Bcjshzhye = string.IsNullOrEmpty(ybinfo.setlinfo.balc) ? "0" : ybinfo.setlinfo.balc.Trim();     //账户余额
                    js.Ybjzlsh = jzlsh.Trim();    //门诊(住院)号
                    js.Jslsh = ybjzlsh.Trim();     //单据流水号
                    js.Yllb = yllb.Trim();      //医疗类别
                    string ksmc = deptName.Trim();      //科室名称
                    js.Zycs = jzcs.Trim();      //本次看病次数
                    string zych = ZYCH.Trim();      //住院床号
                    string ryrq = "".Trim();      //入院日期
                    string rysj = "".Trim();      //入院时间
                    string cyrq2 = cyrq.Trim();      //出院日期
                    string cysj2 = cysj.Trim();      //出院时间
                    if (!IsDate(ryrq))
                    {
                        string strbz = string.Format(" select z1date from zy01h where z1zyno='{0}'", jzlsh);
                        DataSet dsbz = CliUtils.ExecuteSql("sybdj", "cmd", strbz, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                        if (dsbz.Tables[0].Rows.Count > 0)
                        {
                            ryrq = (Convert.ToDateTime(dsbz.Tables[0].Rows[0]["z1date"])).ToString("yyyyMMdd");
                            rysj = (Convert.ToDateTime(dsbz.Tables[0].Rows[0]["z1date"])).ToString("HHmm");
                        }
                    }
                    if (!IsDate(cyrq2))
                    {
                        cyrq2 = cyrq;
                        cysj2 = cysj;
                    }
                    string cyyy2 = cyyy.Trim();      //出院原因
                    string ylfze = string.IsNullOrEmpty(ybinfo.setlinfo.medfee_sumamt.ToString()) || ybinfo.setlinfo.medfee_sumamt.ToString() == null || ybinfo.setlinfo.medfee_sumamt.ToString() == "NULL" ? "0" : ybinfo.setlinfo.medfee_sumamt.ToString().Trim();     //医疗总费用
                    string zhzf = string.IsNullOrEmpty(ybinfo.setlinfo.acct_pay.ToString()) || ybinfo.setlinfo.acct_pay.ToString() == null || ybinfo.setlinfo.acct_pay.ToString() == "NULL" ? "0" : ybinfo.setlinfo.acct_pay.ToString().Trim();       //本次账户支付
                    string xjzf = string.IsNullOrEmpty(ybinfo.setlinfo.psn_cash_pay.ToString()) || ybinfo.setlinfo.psn_cash_pay.ToString() == null || ybinfo.setlinfo.psn_cash_pay.ToString() == "NULL" ? "0" : ybinfo.setlinfo.psn_cash_pay.ToString().Trim();      //本次现金支付
                    string tcjjzf = string.IsNullOrEmpty(ybinfo.setlinfo.fund_pay_sumamt.ToString()) || ybinfo.setlinfo.fund_pay_sumamt.ToString() == null || ybinfo.setlinfo.fund_pay_sumamt.ToString() == "NULL" ? "0" : ybinfo.setlinfo.fund_pay_sumamt.ToString().Trim();     //本次基金支付
                    string dejjzf = string.IsNullOrEmpty(ybinfo.setlinfo.hifmi_pay.ToString()) || ybinfo.setlinfo.hifmi_pay.ToString() == null || ybinfo.setlinfo.hifmi_pay.ToString() == "NULL" ? "0" : ybinfo.setlinfo.hifmi_pay.ToString().Trim();     //大病基金支付

                    string dbecbc = "0";    //二次补偿金额
                    string dwfdfy = string.IsNullOrEmpty(ybinfo.setlinfo.hifes_pay.ToString()) || ybinfo.setlinfo.hifes_pay.ToString() == null || ybinfo.setlinfo.hifes_pay.ToString() == "NULL" ? "0" : ybinfo.setlinfo.hifes_pay.ToString().Trim();     //单位补充医保支付
                    string lxgbddtczf = "0";//离休干部单独统筹支付
                    string jlfy = "0";//甲类费用
                    string ylfy = "0";    //乙类费用
                    string blfy = "0";    //丙类费用
                    string zffy = string.IsNullOrEmpty(ybinfo.setlinfo.fulamt_ownpay_amt.ToString()) || ybinfo.setlinfo.fulamt_ownpay_amt.ToString() == null || ybinfo.setlinfo.fulamt_ownpay_amt.ToString() == "NULL" ? "0" : ybinfo.setlinfo.fulamt_ownpay_amt.ToString().Trim();     //自费费用
                    string jsr = CliUtils.fLoginUser.Trim();       //结算人
                    string jsrq = System.DateTime.Now.ToString("yyyy-MM-dd").Trim();      //结算日期
                    string jssj2 = sysdate.Trim();      //结算时间
                    string qfbzfy = string.IsNullOrEmpty(ybinfo.setlinfo.act_pay_dedc.ToString()) || ybinfo.setlinfo.act_pay_dedc.ToString() == null || ybinfo.setlinfo.act_pay_dedc.ToString() == "NULL" ? "0" : ybinfo.setlinfo.act_pay_dedc.ToString().Trim();   //起付标准自付
                    string fybzf = "0";     //非医保自付
                    string ylypzf = "0"; //乙类药品自付
                    string tjtzzf = "0";   //特检特治自付
                    string jrtcfy = "0";  //进入统筹自付
                    string jrdebxfy = "0"; //进入大病自付

                    string zdjbfwnbcje = "";//重大疾病范围内补偿金额(0)
                    string zdjbfwybcje = ""; //重大疾病范围外补偿金额(0)
                    string mzjzfy = ""; //民政救助金额
                    string gwynbfy = "";//公务员范围内补偿金额
                    string gwywbfy = "";//公务员范围外补偿金额
                    string yyfdje = ""; //医院负担金额(1)
                    string ygbtje = ""; //医改补贴金额


                    string mzdbjzjj = "0.00"; //民政大病救助基金(0)
                    string zftdjj = "0.00"; //政府兜底基金(1)
                    string gwybzfy = "0.00";   //其中公务员补助部分(1)
                    js.Ylfze = ylfze;     //医疗总费用
                    js.Zhzf = zhzf;      //本次账户支付
                    js.Xjzf = xjzf;      //本次现金支付
                    js.Tcjjzf = tjtzzf;    //本次基金支付
                    js.Dejjzf = dejjzf;     //大病基金支付
                    // js.Mzjzfy = str2[30].Trim();    //救助金额
                    js.Ecbcje = dbecbc;     //二次补偿金额
                    js.Dwfdfy = dwfdfy;     //单位补充医保支付
                    js.Lxgbdttcjjzf = lxgbddtczf; //离休干部单独统筹支付
                    js.Jlfy = jlfy;      //甲类费用
                    js.Ylfy = ylfy;      //乙类费用
                    js.Blfy = blfy;       //丙类费用
                    js.Zffy = zffy;      //自费费用
                    js.Jbr = jbr;       //结算人
                    js.Jsrq = jsrq;      //结算日期
                    js.Jssj = jssj2;      //结算时间
                    js.Qfbzfy = qfbzfy;    //起付标准自付
                    js.Fybzf = fybzf;     //非医保自付
                    js.Ypfy = "0";    //乙类药品自付
                    js.Tjzlfy = "0";    //特检特治自付
                    js.Tcfdzffy = "0";    //进入统筹自付
                    js.Defdzffy = "0";   //进入大病自付
                    string Gwynbfy = string.Empty;
                    string Gwywbfy = string.Empty;
                    string Yyfdfy = string.Empty;
                    string Dbbc = string.Empty;
                    //string ygbtje = "";
                    if (!liDqbh.Contains(js.Dqbh))
                    {
                        js.Zdjbfwnbxfy = "0"; //重大疾病范围内补偿金额
                        js.Zdjbfwybxfy = "0"; //重大疾病范围外补偿金额
                        // js.Ecbcje = str2[49].Trim(); //大病二次补偿
                        js.Mzjzfy = "0"; //救助金额
                        Gwynbfy = "0";//公务员内补金额
                        Gwywbfy = "0";//公务员外补金额
                        Yyfdfy = "0"; //医院负担金额 
                        ygbtje = "0";//医改补贴金额
                    }
                    else
                    {
                        //js.Zdjbfwnbxfy = string.IsNullOrEmpty(str2[46]) || str2[46] == null || str2[46] == "NULL" ? "0" : str2[46].Trim(); //重大疾病范围内补偿金额
                        //js.Zdjbfwybxfy = string.IsNullOrEmpty(str2[47]) || str2[47] == null || str2[47] == "NULL" ? "0" : str2[47].Trim(); //重大疾病范围外补偿金额
                        //// js.Ecbcje = str2[49].Trim(); //大病二次补偿
                        //js.Mzjzfy = string.IsNullOrEmpty(str2[48]) || str2[48] == null || str2[48] == "NULL" ? "0" : str2[48].Trim(); //救助金额
                        //Gwynbfy = string.IsNullOrEmpty(str2[49]) || str2[49] == null || str2[49] == "NULL" ? "0" : str2[49].Trim();//公务员内补金额
                        //Gwywbfy = string.IsNullOrEmpty(str2[50]) || str2[50] == null || str2[50] == "NULL" ? "0" : str2[50].Trim();//公务员外补金额
                        //Yyfdfy = string.IsNullOrEmpty(str2[51]) || str2[51] == null || str2[51] == "NULL" ? "0" : str2[51].Trim(); //医院负担金额 
                        //ygbtje = string.IsNullOrEmpty(str2[52]) || str2[52] == null || str2[52] == "NULL" ? "0" : str2[52].Trim(); //医改补贴金额 
                        js.Zdjbfwnbxfy = "0"; //重大疾病范围内补偿金额
                        js.Zdjbfwybxfy = "0"; //重大疾病范围外补偿金额
                        // js.Ecbcje = str2[49].Trim(); //大病二次补偿
                        js.Mzjzfy = "0"; //救助金额
                        Gwynbfy = "0";//公务员内补金额
                        Gwywbfy = "0";//公务员外补金额
                        Yyfdfy = "0"; //医院负担金额 
                        ygbtje = "0";//医改补贴金额
                    }

                    js.Mzjbjzfy = "0.00"; //民政大病救助基金
                    js.Zftdfy = "0.00"; //政府兜底基金
                    js.Gwybzjjzf = (Convert.ToDecimal(Gwynbfy) + Convert.ToDecimal(Gwywbfy)).ToString(); ;   //其中公务员补助部分
                    //js.Gwybzjjzf = str2[52].Trim();   //其中公务员补助部分
                    js.Bcjsqzhye = (Convert.ToDecimal(js.Bcjshzhye) + Convert.ToDecimal(js.Zhzf)).ToString(); //本次结算前帐户余额

                    // js.Bcjsqzhye = (Convert.ToDecimal(js.Bcjshzhye)).ToString(); //本次结算后帐户余额

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

                    //计算总报销金额

                    js.Zbxje = (Convert.ToDecimal(js.Ylfze) - Convert.ToDecimal(js.Xjzf)).ToString();

                    //大病补充
                    Dbbc = (Convert.ToDecimal(js.Tcjjzf) + Convert.ToDecimal(js.Dejjzf)).ToString();

                    string strValue = js.Ylfze + "|" + js.Zbxje + "|" + js.Tcjjzf + "|" + js.Dejjzf + "|" + js.Zhzf + "|" +
                                    js.Ybxjzf + "|" + js.Gwybzjjzf + "|" + js.Qybcylbxjjzf + "|" + js.Zffy + "|" + js.Dwfdfy + "|" +
                                    Yyfdfy + "|" + js.Mzjzfy + "|" + js.Cxjfy + "|" + js.Ylzlfy + "|" + js.Blzlfy + "|" +
                                    js.Fhjjylfy + "|" + js.Qfbzfy + "|" + js.Zzzyzffy + "|" + js.Jrtcfy + "|" + js.Tcfdzffy + "|" +
                                    js.Ctcfdxfy + "|" + js.Jrdebxfy + "|" + js.Defdzffy + "|" + js.Cdefdxfy + "|" + js.Rgqgzffy + "|" +
                                    js.Bcjsqzhye + "|" + js.Bntczflj + "|" + js.Bndezflj + "|" + js.Bnczjmmztczflj + "|" + js.Bngwybzzflj + "|" +
                                    js.Bnzhzflj + "|" + js.Bnzycslj + "|" + js.Zycs + "|" + js.Xm + "|" + js.Jsrq + js.Jssj + "|" +
                                    js.Yllb + "|" + js.Yldylb + "|" + js.Jbjgbm + "|" + js.Ywzqh + "|" + js.Ybjzlsh + "|" +
                                    js.Tsxx + "|" + js.Djh + "|" + js.Jyxl + "|" + js.Yyjylsh + "|" + js.Yxbz + "|" +
                                    js.Grbhgl + "|" + js.Yljgbm + "|" + js.Ecbcje + "|" + js.Mmqflj + "|" + js.Jsfjylsh + "|" +
                                    js.Grbh + "|" + js.Dbzbc + "|" + js.Czzcje + "|" + js.Elmmxezc + "|" + js.Elmmxesy + "|" +
                                    js.Jmgrzfecbcje + "|" + js.Tjje + "|" + js.Syjjzf + "|" + js.Qtybzf + "|" + Gwynbfy + "|" + Gwywbfy + "|";


                    List<string> liSQL = new List<string>();
                    strSql = string.Format(@"delete from ybfyyjsdr where jzlsh='{0}'", jzlsh);
                    liSQL.Add(strSql);
                    strSql = string.Format(@"insert into ybfyyjsdr(jzlsh,jylsh,djhin,cyrq,cyyy,zhsybz,ztjsbz,jbr,xm,kh,
                                            grbh,jsrq,yllb,yldylb,jslsh,ylfze,zhzf,xjzf,tcjjzf,dejjzf,
                                            mzjzfy,dwfdfy,lxgbddtczf,jlfy,ylfy,blfy,zffy,qfbzfy,fybzf,ylypzf,
                                            tjtzzf,tcfdzffy,jrdbzf,zdjbfwnbcje,zdjbfwybcje,yyfdfy,ecbcje,mzdbjzje,czzcje,gwybzjjzf,
				                            sysdate,djh,ryrq,zbxje,bcjsqzhye,ybjzlsh,qtybfy,gwynbfy,gwywbfy,dbbc,ygbtje) values(
                                            '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}',
                                            '{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}',
                                            '{20}','{21}','{22}','{23}','{24}','{25}','{26}','{27}','{28}','{29}',
                                            '{30}','{31}','{32}','{33}','{34}','{35}','{36}','{37}','{38}','{39}',
                                            '{40}','{41}','{42}','{43}','{44}','{45}','{46}','{47}','{48}','{49}','{50}')",
                                             jzlsh, js.Jslsh, djh, cyrq2 + cysj2, cyyy, 1, 0, jbr, js.Xm, js.Kh,
                                             js.Grbh, js.Jsrq, js.Yllb, js.Yldylb, js.Jslsh, js.Ylfze, js.Zhzf, js.Xjzf, js.Tcjjzf, js.Dejjzf,
                                             js.Mzjzfy, js.Dwfdfy, js.Lxgbdttcjjzf, js.Jlfy, js.Ylfy, js.Blfy, js.Zffy, js.Qfbzfy, js.Fybzf, js.Ypfy,
                                             js.Tjzlfy, js.Tcfdzffy, js.Defdzffy, js.Zdjbfwnbxfy, js.Zdjbfwybxfy, Yyfdfy, js.Ecbcje, js.Mzjbjzfy, js.Zftdfy, js.Gwybzjjzf,
                                             sysdate, djh, ryrq + rysj, js.Zbxje, js.Bcjsqzhye, js.Ybjzlsh, js.Qtybzf, Gwynbfy, Gwywbfy, Dbbc, ygbtje);
                    liSQL.Add(strSql);
                    object[] obj = liSQL.ToArray();
                    obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);
                    if (obj[1].ToString() == "1")
                    {
                        WriteLog(sysdate + "  " + jzlsh + " 进入住院收费预结算成功" + strValue);
                        //返回数据显示
                        Frm_zyjsjeNK fyjs = new Frm_zyjsjeNK();
                        fyjs.zfyje.Text = js.Ylfze; //医疗总费用
                        fyjs.zfje.Text = js.Zhzf;//个人账户支付
                        fyjs.tczfje.Text = js.Tcfdzffy;//进入统筹自付
                        fyjs.tjtzzfje.Text = js.Tjzlfy;//特检特治自付
                        fyjs.ylgrfdje.Text = js.Ypfy;//乙类个人负担
                        fyjs.jlje.Text = js.Jlfy;//甲类费用
                        fyjs.qfbzzfje.Text = js.Qfbzfy;//起付标准自付
                        fyjs.fybzfje.Text = js.Fybzf;//非医保自付
                        fyjs.lxgbddtcje.Text = js.Lxgbdttcjjzf;//离休干部单独统筹支付
                        fyjs.zdjbnbje.Text = js.Zdjbfwnbxfy;//重大疾病范围内补金额
                        fyjs.yyfdje.Text = js.Yyfdfy;//医院负担金额
                        fyjs.mzdbjzje.Text = js.Mzjbjzfy;//民政大病救助基金

                        fyjs.grxjje.Text = js.Xjzf;//个人现金支付
                        fyjs.zffyje.Text = js.Zffy;//自费费用
                        fyjs.dbjjzfje.Text = js.Dejjzf;//大病基金支付
                        fyjs.dwbcybje.Text = js.Dwfdfy;//单位补充医保支付
                        fyjs.ylje.Text = js.Ylfy;//乙类费用
                        fyjs.grjjje.Text = js.Tcjjzf;//个人基金支付
                        fyjs.blje.Text = js.Blfy;//丙类费用
                        fyjs.dbzfje.Text = js.Defdzffy;//进入大病自付
                        fyjs.zdjbwbje.Text = js.Zdjbfwybxfy;//重大疾病范围外补金额
                        fyjs.dbecbcje.Text = js.Ecbcje;//大病二次补偿
                        fyjs.zfddje.Text = js.Zftdfy;//政府兜底基金
                        fyjs.gwynbfy.Text = Gwynbfy;//公务员内补费用
                        fyjs.gwywbfy.Text = Gwywbfy;//公务员外补费用
                        fyjs.ygbtje.Text = ygbtje;//医改补贴金额
                        fyjs.StartPosition = FormStartPosition.CenterParent;
                        fyjs.ShowDialog();

                        //医保预结算单
                        // YBZYYJSD(objParam);
                        return new object[] { 0, 1, strValue };
                    }
                    else
                    {
                        WriteLog(sysdate + "  " + jzlsh + " 进入住院收费预结算成功|操作本地数据失败|" + obj[2].ToString());
                        return new object[] { 0, 0, " 进入住院收费预结算成功|操作本地数据失败|" + obj[2].ToString() };
                    }

                }
                else
                {
                    //撤销住院明细上传
                    (int code, string result) = RollbackZYFY(feelists);
                    if (code == 1)
                    {
                        feelists = new List<OutpatientFee>();
                    }
                    WriteLog(sysdate + "  " + jzlsh + " 进入住院收费预结算失败" + res.ToString());
                    return new object[] { 0, 0, "住院收费预结算失败|" + res.ToString() + result };
                }
            }
            catch (Exception ex)
            {
                WriteLog(sysdate + "  住院收费预结算异常|" + ex.Message);
                return new object[] { 0, 0, "住院收费预结算异常|" + ex.Message };
            }
        }
        #endregion

        #region 住院收费结算（新医保接口）
        public static object[] YBZYSFJS(object[] objParam)
        {

            List<string> liDqbh = new List<string>();//赣州所有县地区编号

            #region 赣州地区编号
            liDqbh.Add("360701");
            liDqbh.Add("360721");
            liDqbh.Add("360733");
            liDqbh.Add("360723");
            liDqbh.Add("360734");
            liDqbh.Add("360782");
            liDqbh.Add("360730");
            liDqbh.Add("360725");
            liDqbh.Add("360726");
            liDqbh.Add("360732");
            liDqbh.Add("360728");
            liDqbh.Add("360724");
            liDqbh.Add("360735");
            liDqbh.Add("360722");
            liDqbh.Add("360729");
            liDqbh.Add("360727");
            liDqbh.Add("360781");
            liDqbh.Add("360731");
            liDqbh.Add("360702");
            liDqbh.Add("360703");
            liDqbh.Add("360704");
            #endregion

            string sysdate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string Ywlx = "2304";
            try
            {
                string yllb = "";
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
                string sfzh = "";//身份证
                string kzt1 = "";//卡状态
                string deptName = "";//科室
                string jzcs = "";//住院次数
                string ZYCH = "";//住院床号
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
                DateTime dt_cyrq = DateTime.Now;
                try
                {
                    dt_cyrq = Convert.ToDateTime(cyrqsj);
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
                    sfzh = ds.Tables[0].Rows[0]["sfzh"].ToString();
                    yllb = ds.Tables[0].Rows[0]["yllb"].ToString();
                    kzt1 = ds.Tables[0].Rows[0]["rycbzt"].ToString();
                    deptName = ds.Tables[0].Rows[0]["ksmc"].ToString();
                    jzcs = ds.Tables[0].Rows[0]["zycs"].ToString();
                    ZYCH = ds.Tables[0].Rows[0]["cwh"].ToString();
                    if (yllb == "21" || yllb == "26")
                    {
                        bzbh = "NULL";
                        bzmc = "NULL";
                        cyzd = ds.Tables[0].Rows[0]["bzmc"].ToString();
                    }
                    else
                    {
                        bzbh = ds.Tables[0].Rows[0]["bzbm"].ToString();
                        bzmc = ds.Tables[0].Rows[0]["bzmc"].ToString();
                        cyzd = ds.Tables[0].Rows[0]["bzmc"].ToString();
                    }
                }
                #endregion

                #region 判断是否是异地

                ////属于赣州地区，发票设为true
                //if (liDqbh.Contains(dqbh)) 
                //{
                //    sfdypj = "true";
                //}

                #endregion

                #region 是否慢病患者
                //strSql = string.Format("select * from ybmxbdj where jzlsh='{0}'", jzlsh);
                //ds.Tables.Clear();
                //ds = ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                //if (ds.Tables[0].Rows.Count > 0)
                //{
                //    if (yllb == "21")
                //    {
                //        bzbh = "NULL";
                //        bzmc = "NULL";
                //    }
                //    else
                //    {
                //        bzbh = ds.Tables[0].Rows[0]["mmbzbm"].ToString();
                //        bzmc = ds.Tables[0].Rows[0]["mmbzmc"].ToString();
                //    }

                //}
                #endregion

                #region 出院诊断
                //strSql = string.Format(@"select m1xynm from mza1dd where m1mzno='{0}' ", jzlsh);
                strSql = string.Format(@"select m1xynm from mza1dd where m1mzno='{0}' and m1zdks='y'  and ISNULL(m1xynm,'')<>'' ORDER BY m1date DESC", jzlsh);
                ds.Tables.Clear();
                ds = ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    cyzd = ds.Tables[0].Rows[0]["m1xynm"].ToString();
                }
                #endregion

                #region 医保获取个人信息
                ReadCardInfo cardInfo = GetYbCardInfo(new object[] { "02", sfzh, "", "", "1", sfzh, xm });
                if (cardInfo == null)
                {
                    return new object[] { 0, 0, "获取医保个人信息出错" };
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

                #region 20200922 主动发送读卡交易
                YBMZDK(null);
                strSql = string.Format(@"select kh from YBICKXX where grbh='{0}' and datediff(minute, sysDate,GETDATE()) > 120 ", bxh);
                ds.Tables.Clear();
                ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                if (ds.Tables[0].Rows.Count > 0)
                //{
                //    YBMZDK(null);
                //    kh = dzkh;
                //}
                //else
                {
                    kh = ds.Tables[0].Rows[0]["kh"].ToString();
                }
                #endregion

                #region 老医保接口
                //StringBuilder inputData = new StringBuilder();
                ////入参:保险号|姓名|卡号|地区编号|住院号|病种编号|病种名称|处方起始日期|处方截止日期|出院原因|出院日期|出院时间|是否打印票据|出院诊断
                //inputData.Append(bxh + "|");
                //inputData.Append(xm + "|");
                //inputData.Append(kh + "|");
                //inputData.Append(dqbh + "|");
                //inputData.Append(ybjzlsh + "|");
                //inputData.Append(bzbh + "|");
                //inputData.Append(bzmc + "|");
                //inputData.Append(cfqsrq + "|");
                //inputData.Append(cfjzrq + "|");
                //inputData.Append(cyyy + "|");
                //inputData.Append(cyrq + "|");
                //inputData.Append(cysj + "|");
                //inputData.Append(sfdypj + "|");
                //inputData.Append(cyzd);
                ////inputData.Append(bzbh + "&" + bzmc);
                //StringBuilder outData = new StringBuilder(10240);
                //StringBuilder retMsg = new StringBuilder(1024);

                //WriteLog(sysdate + "  " + jzlsh + " 进入住院收费结算...");
                //WriteLog(sysdate + " 入参|" + inputData.ToString());
                //int i = f_UserBargaingApply(Ywlx, inputData, outData, retMsg); 
                #endregion

                #region 新医保接口  住院结算 by hjw
                string res = string.Empty;
                string data = "{\"psn_no\":\"" + bxh + "\",\"mdtrt_cert_type\":\"" + "02" + "\",\"mdtrt_cert_no\":\"" + sfzh + "\",\"card_sn\":\"\",\"psn_cert_type\":\"01\",\"certno\":\"" + sfzh + "\",\"psn_type\":\"" + cardInfo.insuinfo.psn_type + "\",\"psn_name\":\"" + xm + "\",\"medfee_sumamt\":\"" + sfje3 + "\",\"psn_setlway\":\"01\",\"mdtrt_id\":\"" + jzlsh + "\",\"acct_used_flag\":\"1\",\"insutype\":\"" + cardInfo.insuinfo.insutype + "\",\"invono\":\"" + djh + "\",\"mid_setl_flag\":\"0\",\"fulamt_ownpay_amt\":\"0\",\"overlmt_selfpay\":\"0\",\"preselfpay_amt\":\"0\",\"inscp_scp_amt\":\"0\"}";
                //data = string.Format(data, bxh, "03", kh, sfje3, "01", jzlsh, "0", cardInfo.insuinfo.insutype, ybjzlsh);
                WriteLog(sysdate + "  " + jzlsh + " 进入住院收费结算...");
                WriteLog(sysdate + " 入参|" + data.ToString());
                int i = YBServiceRequest(Ywlx, data, ref res);
                WriteLog(sysdate + " 出参|" + res.ToString());
                #endregion


                if (i > 0)
                {
                    List<string> liSQL = new List<string>();

                    #region 出参 
                    YBBalanceInfo ybinfo = JsonConvert.DeserializeObject<YBBalanceInfo>(res);
                    ParamFYJS js = new ParamFYJS();
                    //    string[] str2 = str[0].Split('|');
                    js.Grbh = bxh.Trim();       //保险号
                    js.Xm = xm.Trim();        //姓名
                    js.Kh = kh.Trim();        //卡号
                    js.Dqbh = dqbh.Trim();      //地区编号
                    js.Dqmc = "".Trim();       //地区名称
                    js.Csrq = cardInfo.baseinfo.brdy.Trim();       //出生日期
                    string sjnl = cardInfo.baseinfo.age.Trim();       //实际年龄
                    js.Cbrq = cardInfo.idetinfo.begntime.Trim();       //参保日期
                    string grsf = cardInfo.idetinfo.psn_type_lv.Trim();       //个人身份
                    js.Dwmc = cardInfo.insuinfo.emp_name.Trim();       //单位名称
                    js.Xb = cardInfo.baseinfo.gend.Trim();        //性别
                    js.Yldylb = cardInfo.insuinfo.psn_type.Trim();    //医疗人员类别
                    string kzt = kzt1.Trim();       //卡状态
                    js.Bcjshzhye = string.IsNullOrEmpty(ybinfo.setlinfo.balc) ? "0" : ybinfo.setlinfo.balc.Trim();     //账户余额
                    js.Ybjzlsh = jzlsh.Trim();    //门诊(住院)号
                    js.Jslsh = ybjzlsh.Trim();     //单据流水号
                    js.Yllb = yllb.Trim();      //医疗类别
                    string ksmc = deptName.Trim();      //科室名称
                    js.Zycs = jzcs.Trim();      //本次看病次数
                    string zych = ZYCH.Trim();      //住院床号
                    string ryrq = "".Trim();      //入院日期
                    string rysj = "".Trim();      //入院时间
                    string cyrq2 = cyrq.Trim();      //出院日期
                    string cysj2 = cysj.Trim();      //出院时间
                    if (!IsDate(ryrq))
                    {
                        string strbz = string.Format(" select z1date from zy01h where z1zyno='{0}'", jzlsh);
                        DataSet dsbz = CliUtils.ExecuteSql("sybdj", "cmd", strbz, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                        if (dsbz.Tables[0].Rows.Count > 0)
                        {
                            ryrq = (Convert.ToDateTime(dsbz.Tables[0].Rows[0]["z1date"])).ToString("yyyyMMdd");
                            rysj = (Convert.ToDateTime(dsbz.Tables[0].Rows[0]["z1date"])).ToString("HHmm");
                        }
                    }
                    if (!IsDate(cyrq2))
                    {
                        cyrq2 = cyrq;
                        cysj2 = cysj;
                    }
                    string cyyy2 = cyyy.Trim();      //出院原因
                    string ylfze = string.IsNullOrEmpty(ybinfo.setlinfo.medfee_sumamt.ToString()) || ybinfo.setlinfo.medfee_sumamt.ToString() == null || ybinfo.setlinfo.medfee_sumamt.ToString() == "NULL" ? "0" : ybinfo.setlinfo.medfee_sumamt.ToString().Trim();     //医疗总费用
                    string zhzf = string.IsNullOrEmpty(ybinfo.setlinfo.acct_pay.ToString()) || ybinfo.setlinfo.acct_pay.ToString() == null || ybinfo.setlinfo.acct_pay.ToString() == "NULL" ? "0" : ybinfo.setlinfo.acct_pay.ToString().Trim();       //本次账户支付
                    string xjzf = string.IsNullOrEmpty(ybinfo.setlinfo.psn_cash_pay.ToString()) || ybinfo.setlinfo.psn_cash_pay.ToString() == null || ybinfo.setlinfo.psn_cash_pay.ToString() == "NULL" ? "0" : ybinfo.setlinfo.psn_cash_pay.ToString().Trim();      //本次现金支付
                    string tcjjzf = string.IsNullOrEmpty(ybinfo.setlinfo.fund_pay_sumamt.ToString()) || ybinfo.setlinfo.fund_pay_sumamt.ToString() == null || ybinfo.setlinfo.fund_pay_sumamt.ToString() == "NULL" ? "0" : ybinfo.setlinfo.fund_pay_sumamt.ToString().Trim();     //本次基金支付
                    string dejjzf = string.IsNullOrEmpty(ybinfo.setlinfo.hifmi_pay.ToString()) || ybinfo.setlinfo.hifmi_pay.ToString() == null || ybinfo.setlinfo.hifmi_pay.ToString() == "NULL" ? "0" : ybinfo.setlinfo.hifmi_pay.ToString().Trim();     //大病基金支付
                                                                                                                                                                                                                                                          //string mzjzfy = str2[30].Trim(); //民政救助金额
                    string dbecbc = "0";    //二次补偿金额
                    string dwfdfy = string.IsNullOrEmpty(ybinfo.setlinfo.hifes_pay.ToString()) || ybinfo.setlinfo.hifes_pay.ToString() == null || ybinfo.setlinfo.hifes_pay.ToString() == "NULL" ? "0" : ybinfo.setlinfo.hifes_pay.ToString().Trim();     //单位补充医保支付
                    string lxgbddtczf = "0";//离休干部单独统筹支付
                    string jlfy = "0";//甲类费用
                    string ylfy = "0";    //乙类费用
                    string blfy = "0";    //丙类费用
                    string zffy = string.IsNullOrEmpty(ybinfo.setlinfo.fulamt_ownpay_amt.ToString()) || ybinfo.setlinfo.fulamt_ownpay_amt.ToString() == null || ybinfo.setlinfo.fulamt_ownpay_amt.ToString() == "NULL" ? "0" : ybinfo.setlinfo.fulamt_ownpay_amt.ToString().Trim();     //自费费用
                    string jsr = CliUtils.fLoginUser.Trim();       //结算人
                    string jsrq = System.DateTime.Now.ToString("yyyy-MM-dd").Trim();      //结算日期
                    string jssj2 = sysdate.Trim();      //结算时间
                    string qfbzfy = string.IsNullOrEmpty(ybinfo.setlinfo.act_pay_dedc.ToString()) || ybinfo.setlinfo.act_pay_dedc.ToString() == null || ybinfo.setlinfo.act_pay_dedc.ToString() == "NULL" ? "0" : ybinfo.setlinfo.act_pay_dedc.ToString().Trim();   //起付标准自付
                    string fybzf = "0";     //非医保自付
                    string ylypzf = "0"; //乙类药品自付
                    string tjtzzf = "0";   //特检特治自付
                    string jrtcfy = "0";  //进入统筹自付
                    string jrdebxfy = "0"; //进入大病自付

                    string zdjbfwnbcje = "";//重大疾病范围内补偿金额(0)
                    string zdjbfwybcje = ""; //重大疾病范围外补偿金额(0)
                    string mzjzfy = ""; //民政救助金额
                    string gwynbfy = "";//公务员范围内补偿金额
                    string gwywbfy = "";//公务员范围外补偿金额
                    string yyfdje = ""; //医院负担金额(1)
                    string ygbtje = ""; //医改补贴金额


                    string mzdbjzjj = "0.00"; //民政大病救助基金(0)
                    string zftdjj = "0.00"; //政府兜底基金(1)
                    string gwybzfy = "0.00";   //其中公务员补助部分(1)
                    js.Ylfze = ylfze;     //医疗总费用
                    js.Zhzf = zhzf;      //本次账户支付
                    js.Xjzf = xjzf;      //本次现金支付
                    js.Tcjjzf = tjtzzf;    //本次基金支付
                    js.Dejjzf = dejjzf;     //大病基金支付
                    // js.Mzjzfy = str2[30].Trim();    //救助金额
                    js.Ecbcje = dbecbc;     //二次补偿金额
                    js.Dwfdfy = dwfdfy;     //单位补充医保支付
                    js.Lxgbdttcjjzf = lxgbddtczf; //离休干部单独统筹支付
                    js.Jlfy = jlfy;      //甲类费用
                    js.Ylfy = ylfy;      //乙类费用
                    js.Blfy = blfy;       //丙类费用
                    js.Zffy = zffy;      //自费费用
                    js.Jbr = jbr;       //结算人
                    js.Jsrq = jsrq;      //结算日期
                    js.Jssj = jssj2;      //结算时间
                    js.Qfbzfy = qfbzfy;    //起付标准自付
                    js.Fybzf = fybzf;     //非医保自付
                    js.Ypfy = "0";    //乙类药品自付
                    js.Tjzlfy = "0";    //特检特治自付
                    js.Tcfdzffy = "0";    //进入统筹自付
                    js.Defdzffy = "0";   //进入大病自付
                    string Gwynbfy = string.Empty;
                    string Gwywbfy = string.Empty;
                    string Yyfdfy = string.Empty;
                    string Dbbc = string.Empty;
                    //string ygbtje = "";
                    if (!liDqbh.Contains(js.Dqbh))
                    {
                        js.Zdjbfwnbxfy = "0"; //重大疾病范围内补偿金额
                        js.Zdjbfwybxfy = "0"; //重大疾病范围外补偿金额
                        // js.Ecbcje = str2[49].Trim(); //大病二次补偿
                        js.Mzjzfy = "0"; //救助金额
                        Gwynbfy = "0";//公务员内补金额
                        Gwywbfy = "0";//公务员外补金额
                        Yyfdfy = "0"; //医院负担金额 
                        ygbtje = "0";//医改补贴金额
                    }
                    else
                    {
                        //js.Zdjbfwnbxfy = string.IsNullOrEmpty(str2[46]) || str2[46] == null || str2[46] == "NULL" ? "0" : str2[46].Trim(); //重大疾病范围内补偿金额
                        //js.Zdjbfwybxfy = string.IsNullOrEmpty(str2[47]) || str2[47] == null || str2[47] == "NULL" ? "0" : str2[47].Trim(); //重大疾病范围外补偿金额
                        //// js.Ecbcje = str2[49].Trim(); //大病二次补偿
                        //js.Mzjzfy = string.IsNullOrEmpty(str2[48]) || str2[48] == null || str2[48] == "NULL" ? "0" : str2[48].Trim(); //救助金额
                        //Gwynbfy = string.IsNullOrEmpty(str2[49]) || str2[49] == null || str2[49] == "NULL" ? "0" : str2[49].Trim();//公务员内补金额
                        //Gwywbfy = string.IsNullOrEmpty(str2[50]) || str2[50] == null || str2[50] == "NULL" ? "0" : str2[50].Trim();//公务员外补金额
                        //Yyfdfy = string.IsNullOrEmpty(str2[51]) || str2[51] == null || str2[51] == "NULL" ? "0" : str2[51].Trim(); //医院负担金额 
                        //ygbtje = string.IsNullOrEmpty(str2[52]) || str2[52] == null || str2[52] == "NULL" ? "0" : str2[52].Trim(); //医改补贴金额 
                        js.Zdjbfwnbxfy = "0"; //重大疾病范围内补偿金额
                        js.Zdjbfwybxfy = "0"; //重大疾病范围外补偿金额
                        // js.Ecbcje = str2[49].Trim(); //大病二次补偿
                        js.Mzjzfy = "0"; //救助金额
                        Gwynbfy = "0";//公务员内补金额
                        Gwywbfy = "0";//公务员外补金额
                        Yyfdfy = "0"; //医院负担金额 
                        ygbtje = "0";//医改补贴金额
                    }

                    js.Mzjbjzfy = "0.00"; //民政大病救助基金
                    js.Zftdfy = "0.00"; //政府兜底基金
                    js.Gwybzjjzf = (Convert.ToDecimal(Gwynbfy) + Convert.ToDecimal(Gwywbfy)).ToString(); ;   //其中公务员补助部分
                    js.Bcjsqzhye = (Convert.ToDecimal(js.Bcjshzhye) + Convert.ToDecimal(js.Zhzf)).ToString(); //本次结算前帐户余额

                    //js.Bcjsqzhye = (Convert.ToDecimal(js.Bcjshzhye)).ToString(); //本次结算后帐户余额

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

                    if (!IsDate(ryrq))
                    {
                        string strbz = string.Format(" select z1date from zy01h where z1zyno='{0}'", jzlsh);
                        DataSet dsbz = CliUtils.ExecuteSql("sybdj", "cmd", strbz, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                        if (dsbz.Tables[0].Rows.Count > 0)
                        {
                            ryrq = (Convert.ToDateTime(dsbz.Tables[0].Rows[0]["z1date"])).ToString("yyyyMMdd");
                            rysj = (Convert.ToDateTime(dsbz.Tables[0].Rows[0]["z1date"])).ToString("HHmm");
                        }
                    }
                    if (!IsDate(cyrq2))
                    {
                        cyrq2 = cyrq;
                        cysj2 = cysj;
                    }
                    #endregion

                    /*医疗费总额|总报销金额|统筹基金支付|大额基金支付|账户支付|
                     * 现金支付|公务员补助基金支付|企业补充医疗保险基金支付|自费费用|单位负担费用|
                     * 医院负担费用|民政救助费用|超限价费用|乙类自理费用|丙类自理费用|
                     * 符合基本医疗费用|起付标准费用|转诊转院自付费用|进入统筹费用|统筹分段自付费用|
                     * 超统筹封顶线费用|进入大额报销费用|大额分段自付费用|超大额封顶线费用|人工器官自付费用|
                     * 本次结算后帐户余额|本年统筹支付累计(不含本次)|本年大额支付累计(不含本次)|本年城镇居民门诊统筹支付累计(不含本次)|本年公务员补助支付累计(不含本次)|
                     * 本年账户支付累计(不含本次)|本年住院次数累计(不含本次)|住院次数|姓名|结算时间|
                     * 医疗类别|医疗待遇类别|经办机构编码|业务周期号|结算流水号|
                     * 提示信息|单据号|交易类型|医院交易流水号|有效标志|
                     * 个人编号管理|医疗机构编码|二次补偿金额|门慢起付累计|接收方交易流水号|
                     * 个人编号|单病种补差|财政支出金额|二类门慢限额支出|二类门慢限额剩余|
                     * 居民个人自付二次补偿金额|体检金额|生育基金支付|其他医保支付|公务员内补|公务员外补
                     */
                    //计算总报销金额
                    js.Zbxje = (Convert.ToDecimal(js.Ylfze) - Convert.ToDecimal(js.Xjzf)).ToString();

                    //大病补充
                    Dbbc = (Convert.ToDecimal("0.00") + Convert.ToDecimal(js.Dejjzf)).ToString();

                    string strValue = js.Ylfze + "|" + js.Zbxje + "|" + js.Tcjjzf + "|" + js.Dejjzf + "|" + js.Zhzf + "|" +
                                    js.Ybxjzf + "|" + js.Gwybzjjzf + "|" + js.Qybcylbxjjzf + "|" + js.Zffy + "|" + js.Dwfdfy + "|" +
                                    Yyfdfy + "|" + js.Mzjzfy + "|" + js.Cxjfy + "|" + js.Ylzlfy + "|" + js.Blzlfy + "|" +
                                    js.Fhjjylfy + "|" + js.Qfbzfy + "|" + js.Zzzyzffy + "|" + js.Jrtcfy + "|" + js.Tcfdzffy + "|" +
                                    js.Ctcfdxfy + "|" + js.Jrdebxfy + "|" + js.Defdzffy + "|" + js.Cdefdxfy + "|" + js.Rgqgzffy + "|" +
                                    js.Bcjsqzhye + "|" + js.Bntczflj + "|" + js.Bndezflj + "|" + js.Bnczjmmztczflj + "|" + js.Bngwybzzflj + "|" +
                                    js.Bnzhzflj + "|" + js.Bnzycslj + "|" + js.Zycs + "|" + js.Xm + "|" + js.Jsrq + js.Jssj + "|" +
                                    js.Yllb + "|" + js.Yldylb + "|" + js.Jbjgbm + "|" + js.Ywzqh + "|" + js.Ybjzlsh + "|" +
                                    js.Tsxx + "|" + js.Djh + "|" + js.Jyxl + "|" + js.Yyjylsh + "|" + js.Yxbz + "|" +
                                    js.Grbhgl + "|" + js.Yljgbm + "|" + js.Ecbcje + "|" + js.Mmqflj + "|" + js.Jsfjylsh + "|" +
                                    js.Grbh + "|" + js.Dbzbc + "|" + js.Czzcje + "|" + js.Elmmxezc + "|" + js.Elmmxesy + "|" +
                                    js.Jmgrzfecbcje + "|" + js.Tjje + "|" + js.Syjjzf + "|" + js.Qtybzf + "|" + Gwynbfy + "|" + Gwywbfy + "|";

                    //MessageBox.Show("医院负担："+js.Yyfdfy);

                    strSql = string.Format(@"insert into ybfyjsdr(jzlsh,jylsh,djhin,cyrq,cyyy,zhsybz,ztjsbz,jbr,xm,kh,
                                            grbh,jsrq,yllb,yldylb,jslsh,ylfze,zhzf,xjzf,tcjjzf,dejjzf,
                                            mzjzfy,dwfdfy,lxgbddtczf,jlfy,ylfy,blfy,zffy,qfbzfy,fybzf,ylypzf,
                                            tjtzzf,tcfdzffy,jrdbzf,zdjbfwnbcje,zdjbfwybcje,yyfdfy,ecbcje,mzdbjzje,czzcje,gwybzjjzf,
				                            sysdate,djh,ryrq,zbxje,bcjsqzhye,ybjzlsh,qtybfy,gwynbfy,gwywbfy,dbbc,dqbh,ygbtje) values(
                                            '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}',
                                            '{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}',
                                            '{20}','{21}','{22}','{23}','{24}','{25}','{26}','{27}','{28}','{29}',
                                            '{30}','{31}','{32}','{33}','{34}','{35}','{36}','{37}','{38}','{39}',
                                            '{40}','{41}','{42}','{43}','{44}','{45}','{46}','{47}','{48}','{49}','{50}','{51}')",
                                              jzlsh, js.Jslsh, djh, cyrq2 + cysj2, cyyy, 1, 0, jbr, js.Xm, js.Kh,
                                              js.Grbh, js.Jsrq, js.Yllb, js.Yldylb, js.Jslsh, js.Ylfze, js.Zhzf, js.Xjzf, js.Tcjjzf, js.Dejjzf,
                                              js.Mzjzfy, js.Dwfdfy, js.Lxgbdttcjjzf, js.Jlfy, js.Ylfy, js.Blfy, js.Zffy, js.Qfbzfy, js.Fybzf, js.Ypfy,
                                              js.Tjzlfy, js.Tcfdzffy, js.Defdzffy, js.Zdjbfwnbxfy, js.Zdjbfwybxfy, Yyfdfy, js.Ecbcje, js.Mzjbjzfy, js.Zftdfy, js.Gwybzjjzf,
                                              sysdate, djh, ryrq + rysj, js.Zbxje, js.Bcjsqzhye, js.Ybjzlsh, js.Qtybzf, Gwynbfy, Gwywbfy, Dbbc, dqbh, ygbtje);
                    liSQL.Add(strSql);
                    object[] obj = liSQL.ToArray();
                    obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);
                    if (obj[1].ToString() == "1")
                    {
                        feelists = new List<OutpatientFee>();
                        WriteLog(sysdate + "  " + jzlsh + " 进入住院收费结算成功" + strValue);
                        // 打印结算单
                        //object[] objParam1 = { jzlsh };
                        //YBZYJSD(objParam1);

                        ////医保电子病历登记
                        //object[] objParam1 = { jzlsh ,"0"};
                        //object[] obdj= YBDZBLDJ(objParam1);
                        //if (obdj[1].ToString() != "1") 
                        //{
                        //    MessageBox.Show("医保电子病历登记失败,请手动登记！");
                        //}
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
                    WriteLog(sysdate + "  " + jzlsh + " 进入住院收费结算失败" + res.ToString());
                    return new object[] { 0, 0, "住院收费结算失败|" + res.ToString() };
                }
            }
            catch (Exception ex)
            {
                WriteLog(sysdate + "  住院结算失败" + ex.Message);
                return new object[] { 0, 0, "住院结算失败" };
            }
        }
        #endregion

        #region 住院收费结算撤销（新医保接口）
        public static object[] YBZYSFJSCX(object[] objParam)
        {
            string sysdate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            try
            {
                string Ywlx = "2305";
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

                #region 20200922 主动发送读卡交易
                strSql = string.Format(@"select kh from YBICKXX where grbh='{0}' and datediff(minute, sysDate,GETDATE()) > 120 ", bxh);
                ds.Tables.Clear();
                ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    YBMZDK(null);
                    kh = dzkh;
                }
                else
                {
                    kh = ds.Tables[0].Rows[0]["kh"].ToString();
                }
                #endregion

                #region 老医保接口
                //StringBuilder inputData = new StringBuilder();
                ////入参:
                //inputData.Append(bxh + "|");
                //inputData.Append(xm + "|");
                //inputData.Append(kh + "|");
                //inputData.Append(dqbh + "|");
                //inputData.Append(ybjzlsh + "|");
                //inputData.Append(djlsh + "|");
                //inputData.Append(sfmxsfzf);
                //StringBuilder outData = new StringBuilder(1024);
                //StringBuilder retMsg = new StringBuilder(1024);
                //WriteLog(sysdate + "  " + jzlsh + " 进入住院收费结算撤销...");
                //WriteLog(sysdate + " 入参|" + inputData.ToString());
                //int i = f_UserBargaingApply(Ywlx, inputData, outData, retMsg);
                #endregion


                #region 新医保接口  住院结算撤销  by hjw
                string res = "";
                string data = "{\"mdtrt_id\":\"" + jzlsh + "\",\"setl_id\":\"" + ybjzlsh + "\",\"psn_no\":\"" + bxh + "\"}";
                data = string.Format(data, jzlsh, jzlsh, bxh);
                int i = YBServiceRequest(Ywlx, data, ref res);
                WriteLog(sysdate + "  " + jzlsh + " 进入住院收费结算撤销...");
                WriteLog(sysdate + " 入参|" + res.ToString());
                #endregion

                if (i > 0)
                {
                    List<string> liSQL = new List<string>();
                    strSql = string.Format(@"insert into ybfyjsdr(jzlsh,jylsh,djhin,cyrq,cyyy,zhsybz,ztjsbz,jbr,xm,kh,
                                        grbh,jsrq,yllb,yldylb,jslsh,ylfze,zhzf,xjzf,tcjjzf,dejjzf,
                                        mzjzfy,dwfdfy,lxgbddtczf,jlfy,ylfy,blfy,zffy,qfbzfy,fybzf,ylypzf,
                                        tjtzzf,jrtczf,jrdbzf,zdjbfwnbcje,zdjbfwybcje,yyfdfy,ecbcje,mzdbjzje,czzcje,gwybzjjzf,
				                        djh,ryrq,bcjsqzhye,sysdate,cxbz)  select 
                                        jzlsh,jylsh,djhin,cyrq,cyyy,zhsybz,ztjsbz,'{3}',xm,kh,
                                        grbh,jsrq,yllb,yldylb,jslsh,ylfze,zhzf,xjzf,tcjjzf,dejjzf,
                                        mzjzfy,dwfdfy,lxgbddtczf,jlfy,ylfy,blfy,zffy,qfbzfy,fybzf,ylypzf,
                                        tjtzzf,jrtczf,jrdbzf,zdjbfwnbcje,zdjbfwybcje,yyfdfy,ecbcje,mzdbjzje,czzcje,gwybzjjzf,
				                        djh,ryrq,bcjsqzhye,'{2}',0 from ybfyjsdr where jzlsh='{0}' and jslsh='{1}' and cxbz=1", jzlsh, djlsh, sysdate, CliUtils.fLoginUser);
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
                        WriteLog(sysdate + "  " + jzlsh + " 进入住院收费结算撤销成功" + res.ToString());
                        //object[] objParam1 = { jzlsh };
                        //YBZYSFDJCX(objParam1);
                        return new object[] { 0, 1, "住院收费结算撤销成功", res.ToString() };
                    }
                    else
                    {
                        WriteLog(sysdate + "  " + jzlsh + " 进入住院收费结算撤销成功|操作本地数据失败" + obj[2].ToString());
                        return new object[] { 0, 0, "住院收费结算撤销失败|" + obj[2].ToString() };
                    }
                }
                else
                {
                    WriteLog(sysdate + "  " + jzlsh + " 进入住院收费结算撤销失败" + res.ToString());
                    return new object[] { 0, 0, "住院收费结算撤销失败|" + res.ToString() };
                }
            }
            catch (Exception ex)
            {
                WriteLog(sysdate + "  住院结算撤销失败" + ex.Message);
                return new object[] { 0, 0, "住院结算撤销失败" };
            }
        }
        #endregion

        #region 打印住院预结算单（无医保接口）
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

        #region 打印住院结算单（无医保接口）
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
            strSql = string.Format(@"select a.yldylb,'安福县中医院' as yljgmc,a.grbh,a.ybjzlsh,
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
            DataSet ds_jsinfo = null;
            try
            {
                ds_jsinfo = CliUtils.ExecuteSql("szy01", "cmd", strSql, CliUtils.fLoginDB.ToString(), true, CliUtils.fCurrentProject.ToString());
            }
            catch (Exception ex)
            {
                return new object[] { 0, 0, "无单据" };
            }
            if (ds_jsinfo.Tables[0].Rows.Count <= 0) return new object[] { 0, 0, "无数据" };
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

        #region 医疗待遇审批信息查询（无医保接口）
        public static object[] YBYLDYSPXXCX(object[] ojbParam)
        {
            string sysdate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            WriteLog(sysdate + "  进入医疗待遇审批信息查询...");
            return new object[] { 0, 0, "无医疗待遇审批信息查询接口" };
        }
        #endregion

        #region 医疗待遇封锁信息查询（无医保接口）
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

        #region 获取病种信息查询1作废（无医保接口）
        public static object[] YBBZCX1(object[] objParam)
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
                {
                    string strSBZ = string.Format(@"SELECT * FROM ybmzzydjdr where grbh='{0}' and cxbz=1 and jzbz='m' order by sysdate desc", grbh);
                    DataSet dsBZ = CliUtils.ExecuteSql("sybdj", "cmd", strSBZ, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                    string bzbm = "";
                    if (dsBZ.Tables[0].Rows.Count > 0)
                    {
                        if (!string.IsNullOrEmpty(dsBZ.Tables[0].Rows[0]["mmbzbm1"].ToString()))
                        {
                            bzbm += dsBZ.Tables[0].Rows[0]["mmbzbm1"].ToString() + ",";
                        }
                        if (!string.IsNullOrEmpty(dsBZ.Tables[0].Rows[0]["mmbzbm2"].ToString()))
                        {
                            bzbm += dsBZ.Tables[0].Rows[0]["mmbzbm2"].ToString() + ",";
                        }
                        if (!string.IsNullOrEmpty(dsBZ.Tables[0].Rows[0]["mmbzbm3"].ToString()))
                        {
                            bzbm += dsBZ.Tables[0].Rows[0]["mmbzbm3"].ToString() + ",";
                        }
                        if (!string.IsNullOrEmpty(dsBZ.Tables[0].Rows[0]["mmbzbm4"].ToString()))
                        {
                            bzbm += dsBZ.Tables[0].Rows[0]["mmbzbm4"].ToString() + ",";
                        }

                        // bzbm = bzbm.Trim(',');
                    }
                    WriteLog("病种为:" + bzbm + "\r\n");
                    if (!string.IsNullOrEmpty(bzbm))
                    {
                        bzbm = bzbm.Trim(',');
                        strSql += string.Format(@" and yllb in(31) and dm in({0})", bzbm);
                    }
                    else
                    {
                        strSql += string.Format(@" and yllb in(31)");
                    }


                }
                else if (yllb == "81")
                {
                    strSql += string.Format(@" and yllb='81'");
                }
                else
                    strSql += string.Format(@" and 1!=1");
            }
            else if (jzbz.ToUpper().Equals("Z"))
            {
                strSql = string.Format(@"select dm,dmmc,pym,wbm from ybbzmrdr where 1=1");
                if (yllb.Equals("21")) //普通住院
                    strSql += string.Format(@" and bz=0");
                else
                    strSql += string.Format(@" and bz=1");
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

        #region 获取病种信息查询（无医保接口）
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
                {
                    //存取异地病种
                    string strbz = string.Format("select '' as dm,'' as dmmc ,'' as pym,'' as wbm");
                    DataSet dsbz = CliUtils.ExecuteSql("sybdj", "cmd", strbz, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                    dsbz.Tables[0].Rows.RemoveAt(0);


                    #region 以前获取已登记的病种
                    //string strSBZ = string.Format(@"SELECT * FROM ybmzzydjdr where grbh='{0}' and cxbz=1 and jzbz='m' order by sysdate desc", grbh);
                    //DataSet dsBZ = CliUtils.ExecuteSql("sybdj", "cmd", strSBZ, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                    //string bzbm = "";
                    //if (dsBZ.Tables[0].Rows.Count > 0)
                    //{
                    //    if (!string.IsNullOrEmpty(dsBZ.Tables[0].Rows[0]["mmbzbm1"].ToString()))
                    //    {
                    //        bzbm += "'" + dsBZ.Tables[0].Rows[0]["mmbzbm1"].ToString() + "'" + ",";
                    //        DataRow dr = dsbz.Tables[0].NewRow();
                    //        dr[0] = dsBZ.Tables[0].Rows[0]["mmbzbm1"].ToString();
                    //        dr[1] = dsBZ.Tables[0].Rows[0]["mmbzmc1"].ToString();
                    //        dr[2] = GetPYWB(dsBZ.Tables[0].Rows[0]["mmbzmc1"].ToString()).Split('|')[0];
                    //        dr[3] = GetPYWB(dsBZ.Tables[0].Rows[0]["mmbzmc1"].ToString()).Split('|')[1];
                    //        dsbz.Tables[0].Rows.Add(dr);
                    //    }
                    //    if (!string.IsNullOrEmpty(dsBZ.Tables[0].Rows[0]["mmbzbm2"].ToString()))
                    //    {
                    //        bzbm += "'" + dsBZ.Tables[0].Rows[0]["mmbzbm2"].ToString() + "'" + ",";
                    //        DataRow dr = dsbz.Tables[0].NewRow();
                    //        dr[0] = dsBZ.Tables[0].Rows[0]["mmbzbm2"].ToString();
                    //        dr[1] = dsBZ.Tables[0].Rows[0]["mmbzmc2"].ToString();
                    //        dr[2] = GetPYWB(dsBZ.Tables[0].Rows[0]["mmbzmc2"].ToString()).Split('|')[0];
                    //        dr[3] = GetPYWB(dsBZ.Tables[0].Rows[0]["mmbzmc2"].ToString()).Split('|')[1];
                    //        dsbz.Tables[0].Rows.Add(dr);
                    //    }
                    //    if (!string.IsNullOrEmpty(dsBZ.Tables[0].Rows[0]["mmbzbm3"].ToString()))
                    //    {
                    //        bzbm += "'" + dsBZ.Tables[0].Rows[0]["mmbzbm3"].ToString() + "'" + ",";
                    //        DataRow dr = dsbz.Tables[0].NewRow();
                    //        dr[0] = dsBZ.Tables[0].Rows[0]["mmbzbm3"].ToString();
                    //        dr[1] = dsBZ.Tables[0].Rows[0]["mmbzmc3"].ToString();
                    //        dr[2] = GetPYWB(dsBZ.Tables[0].Rows[0]["mmbzmc3"].ToString()).Split('|')[0];
                    //        dr[3] = GetPYWB(dsBZ.Tables[0].Rows[0]["mmbzmc3"].ToString()).Split('|')[1];
                    //        dsbz.Tables[0].Rows.Add(dr);
                    //    }
                    //    if (!string.IsNullOrEmpty(dsBZ.Tables[0].Rows[0]["mmbzbm4"].ToString()))
                    //    {
                    //        bzbm += "'" + dsBZ.Tables[0].Rows[0]["mmbzbm4"].ToString() + "'" + ",";
                    //        DataRow dr = dsbz.Tables[0].NewRow();
                    //        dr[0] = dsBZ.Tables[0].Rows[0]["mmbzbm4"].ToString();
                    //        dr[1] = dsBZ.Tables[0].Rows[0]["mmbzmc4"].ToString();
                    //        dr[2] = GetPYWB(dsBZ.Tables[0].Rows[0]["mmbzmc4"].ToString()).Split('|')[0];
                    //        dr[3] = GetPYWB(dsBZ.Tables[0].Rows[0]["mmbzmc4"].ToString()).Split('|')[1];
                    //        dsbz.Tables[0].Rows.Add(dr);
                    //    }

                    //    // bzbm = bzbm.Trim(',');
                    //}
                    #endregion

                    #region 获取ybmxbdj病种
                    string strSBZ = string.Format(@"select distinct MMBZBM,MMBZMC   from ybmxbdj
                    where BXH='{0}'  and ISNULL(MMBZBM,'')<>''", grbh);
                    DataSet dsBZ = CliUtils.ExecuteSql("sybdj", "cmd", strSBZ, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                    string bzbm = "";
                    if (dsBZ.Tables[0].Rows.Count > 0)
                    {
                        for (int j = 0; j < dsBZ.Tables[0].Rows.Count; j++)
                        {
                            bzbm += "'" + dsBZ.Tables[0].Rows[j]["MMBZBM"].ToString() + "'" + ",";
                            DataRow dr = dsbz.Tables[0].NewRow();
                            dr[0] = dsBZ.Tables[0].Rows[j]["MMBZBM"].ToString();
                            dr[1] = dsBZ.Tables[0].Rows[j]["MMBZMC"].ToString();
                            dr[2] = GetPYWB(dsBZ.Tables[j].Rows[0]["MMBZMC"].ToString()).Split('|')[0];
                            dr[3] = GetPYWB(dsBZ.Tables[j].Rows[0]["MMBZMC"].ToString()).Split('|')[1];
                            dsbz.Tables[0].Rows.Add(dr);
                        }
                        bzbm = bzbm.Trim(',');
                    }
                    #endregion

                    WriteLog("病种为:" + bzbm + "\r\n");

                    if (!string.IsNullOrEmpty(bzbm))
                    {
                        bzbm = bzbm.Trim(',');
                        strSql += string.Format(@" and yllb in(31) and dm in({0})", bzbm);
                        WriteLog("获取病种语句为:" + strSql + "\r\n");
                        DataSet dsYD = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                        if (dsYD.Tables[0].Rows.Count > 0)
                        {
                            WriteLog("获取病种信息查询成功|");
                            return new object[] { 0, 1, dsYD.Tables[0] };
                        }
                        else
                        {
                            WriteLog("获取病种信息查询成功|");
                            return new object[] { 0, 1, dsbz.Tables[0] };
                        }
                    }
                    else
                    {
                        strSql += string.Format(@" and yllb in(31)");
                    }


                }
                else if (yllb == "81")
                {
                    strSql += string.Format(@" and yllb='81'");//特殊诊疗
                }
                else if (yllb == "33")
                {
                    strSql += string.Format(@" and yllb='31' and dm in('000001000002')");
                }
                else if (yllb == "36")
                {
                    strSql += string.Format(@" and yllb='36'");//大病特药
                }
                else if (yllb == "51")
                {
                    strSql += string.Format(@" and yllb='51'");//产前检查
                }
                else if (yllb == "26")
                {
                    strSql += string.Format(@" and yllb='26'");//白内障
                }
                else
                    //strSql += string.Format(@" and 1!=1");
                    strSql += string.Format(@" and yllb='{0}' ", yllb);
            }
            else if (jzbz.ToUpper().Equals("Z"))
            {
                strSql = string.Format(@"select dm,dmmc,pym,wbm from ybbzmrdr where 1=1");
                if (yllb.Equals("21")) //普通住院
                    strSql += string.Format(@" and bz=0");
                else
                    strSql += string.Format(@" and bz=1");
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

        #region 电子病历登记（未做医保接口）

        public static object[] YBDZBLDJ(object[] objParam)
        {
            string bxh = string.Empty;//保险号
            string xm = string.Empty;//姓名
            string kh = string.Empty;//卡号
            string tcqh = string.Empty;//地区编号
            string jzlsh = string.Empty;//住院号
            string sfzh = string.Empty;//身份号码
            string bahx = string.Empty;//病案号
            string mobi = string.Empty;//联系电话
            string bzno = string.Empty;//疾病编码
            string bznm = string.Empty;//疾病名称

            string zyno = objParam[0].ToString();
            string bzlx = objParam[1].ToString();//0 新增登记 1 修改变更 2删除
            string YWLX = "JK0DJ90";
            StringBuilder inputData = new StringBuilder(10240);
            StringBuilder outData = new StringBuilder(10240);
            StringBuilder retMsg = new StringBuilder(1024);
            string strSQL = string.Format(@" select ybjzlsh,grbh,xm,kh,z1tcdq,jzlsh,z1idno,z1bahx,z1mobi,z1bzno,z1bznm from ybfyjsdr 
            left join zy01h on z1zyno=jzlsh where cxbz=1 and jzlsh='{0}'", zyno);
            DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSQL, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            if (ds.Tables[0].Rows.Count > 0)
            {
                bxh = ds.Tables[0].Rows[0]["grbh"].ToString();//保险号
                xm = ds.Tables[0].Rows[0]["xm"].ToString();//姓名
                kh = ds.Tables[0].Rows[0]["kh"].ToString();//卡号
                tcqh = ds.Tables[0].Rows[0]["z1tcdq"].ToString();//地区编号
                jzlsh = ds.Tables[0].Rows[0]["ybjzlsh"].ToString();//医保住院号
                                                                   // jzlsh = "0100657959";//住院号               
                sfzh = ds.Tables[0].Rows[0]["z1idno"].ToString();//身份号码
                bahx = ds.Tables[0].Rows[0]["jzlsh"].ToString();//病案号
                mobi = ds.Tables[0].Rows[0]["z1mobi"].ToString();//联系电话
                #region 出院诊断
                string strSqlZD = string.Format(@"select m1xyzd,m1xynm,m1xybz from mza1dd where m1mzno='{0}' and m1zdks='y' and m1xybz='y' ", zyno);
                DataSet dsZD = CliUtils.ExecuteSql("sybdj", "cmd", strSqlZD, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                if (dsZD.Tables[0].Rows.Count > 0)
                {
                    bzno = dsZD.Tables[0].Rows[0]["m1xyzd"].ToString();//出院诊断编码
                    bznm = dsZD.Tables[0].Rows[0]["m1xynm"].ToString();//出院诊断名称
                }
                else
                {
                    bzno = ds.Tables[0].Rows[0]["z1bzno"].ToString();//疾病编码
                    bznm = ds.Tables[0].Rows[0]["z1bznm"].ToString();//疾病名称
                }
                #endregion
                dsZD.Dispose();
                ds.Dispose();

            }
            else
            {
                WriteLog(" 电子病历登记失败_该住院患者未办理结算！");
                return new object[] { 0, 0, "电子病历登记失败_该住院患者未办理结算" };
            }
            //入参
            inputData.Append(bxh + "|");
            inputData.Append(xm + "|");
            inputData.Append(kh + "|");
            inputData.Append(dqbh + "|");
            inputData.Append(jzlsh + "|");
            inputData.Append(sfzh + "|");
            inputData.Append(bahx + "|");
            inputData.Append(mobi + "|");
            inputData.Append(bzno + "|");
            inputData.Append(bznm + "|");
            inputData.Append("系统管理员" + "|");
            inputData.Append(bzlx);
            WriteLog("  电子病历登记/撤销|入参|" + inputData.ToString());

            int i = f_UserBargaingApply(YWLX, inputData, outData, retMsg);
            if (i > 0)
            {
                List<string> liSQL = new List<string>();
                string strSql = "";
                if (bzlx == "0")
                {
                    strSql = string.Format(@"update ybfyjsdr set bascbz='1' where ybjzlsh='{0}'", jzlsh);
                }
                else
                {
                    strSql = string.Format(@"update ybfyjsdr set bascbz='0' where ybjzlsh='{0}'", jzlsh);
                }
                liSQL.Add(strSql);
                object[] obj = liSQL.ToArray();
                obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);
                if (obj[1].ToString() == "1")
                {
                    WriteLog(" 电子病历登记/撤销成功！");

                    return new object[] { 0, 1, "电子病历登记/撤销成功" };
                }
                else
                {
                    WriteLog(" 电子病历登记/撤销失败！");
                    return new object[] { 0, 0, "电子病历登记/撤销失败" + retMsg.ToString() };
                }

            }
            else
            {
                WriteLog(" 电子病历登记/撤销失败！");
                return new object[] { 0, 0, "电子病历登记/撤销失败" + retMsg.ToString() };
            }

        }

        #endregion

        #region 门诊登记(挂号)撤销（内部）（新医保接口）
        public static object[] NYBMZDJCX(object[] objParam)
        {
            string sysdate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string Ywlx = "2202";
            string jzlsh = objParam[0].ToString(); //就诊流水号
            string bxh = objParam[1].ToString(); //保险号(个人编号)
            string xm = objParam[2].ToString();  //姓名
            string kh = objParam[3].ToString(); //卡号
            string dqbh = objParam[4].ToString(); //地区编号
            string ybjzlsh = objParam[5].ToString();//医保就诊流水号



            #region 新医保接口 by hjw
            string res = string.Empty;
            string NewInputdata = "{\"psn_no\":\"" + bxh + "\",\"mdtrt_id\":\"" + ybjzlsh + "\",\"ipt_otp_no\":\"" + jzlsh + "\"}";
            //NewInputdata = string.Format(NewInputdata, kh, ybjzlsh, jzlsh);
            WriteLog(sysdate + "  " + jzlsh + " 进入门诊挂号撤销(内部)...");
            WriteLog(sysdate + "  入参|" + NewInputdata.ToString());
            int i = YBServiceRequest(YwCode: Ywlx, data: NewInputdata, ref res);
            #endregion

            if (i > 0)
            {
                WriteLog(sysdate + "  " + jzlsh + " 进入门诊挂号撤销(内部)成功|" + res.ToString());
                return new object[] { 0, 1, "门诊挂号撤销(内部)成功", res.ToString() };
            }
            else
            {
                WriteLog(sysdate + "  " + jzlsh + " 进入门诊挂号撤销(内部)失败|" + res.ToString());
                return new object[] { 0, 0, "门诊挂号撤销(内部)失败", res.ToString() };
            }
        }
        #endregion

        #region 门诊收费结算撤销(内部)(新医保接口)
        public static object[] NYBMZSFJSCX(object[] objParam)
        {
            string sysdate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string Ywlx = "2208";
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

            #region 新医保接口 by hjw
            string res = "";
            string data = "{\"setl_id\":\"" + jslsh + "\",\"mdtrt_id\":\"" + ybjzlsh + "\",\"psn_no\":\"" + bxh + "\"}";
            data = String.Format(data, jslsh, jzlsh, bxh);
            WriteLog(sysdate + "  " + jzlsh + " 进入门诊收费撤销(内部)...");
            WriteLog(sysdate + "  入参|" + data.ToString());
            int i = YBServiceRequest(Ywlx, data, ref res);
            YBBalanceInfo ybinfo = new YBBalanceInfo();
            #endregion
            if (i > 0)
            {
                WriteLog(sysdate + "  " + jzlsh + " 进入门诊收费撤销(内部)成功|" + res.ToString());
                return new object[] { 0, 1, "门诊收费撤销成功|" + res.ToString() };
            }
            else
            {
                WriteLog(sysdate + "  " + jzlsh + " 进入门诊收费撤销(内部)失败|" + res.ToString());
                return new object[] { 0, 0, "门诊收费撤销失败|" + res.ToString() };
            }
        }
        #endregion

        #region 住院登记撤销(内部)（新医保接口）
        public static object[] NYBZYDJCX(object[] objParam)
        {
            string sysdate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string Ywlx = "2404";
            string jzlsh = objParam[0].ToString(); //就诊流水号
            string bxh = objParam[1].ToString(); //保险号(个人编号)
            string xm = objParam[2].ToString();  //姓名
            string kh = objParam[3].ToString(); //卡号
            string dqbh = objParam[4].ToString(); //地区编号
            string ybjzlsh = objParam[5].ToString();//医保就诊流水号

            string Res = string.Empty;

            WriteLog(sysdate + "  " + jzlsh + " 进入住院医保登记撤销(内部)...");
            string data = "{\"mdtrt_id\":\"" + jzlsh + "\",\"psn_no\":\"" + bxh + "\"}";
            WriteLog(sysdate + "  入参|" + data);
            int i = YBServiceRequest(Ywlx, data, ref Res);
            //int i = f_UserBargaingApply(Ywlx, inputData, outData, retMsg);
            if (i > 0)
            {
                WriteLog(sysdate + "  " + jzlsh + " 进入住院医保登记撤销(内部)成功|" + Res.ToString());
                return new object[] { 0, 1, "住院医保登记撤销(内部)成功", Res.ToString() };
            }
            else
            {
                WriteLog(sysdate + "  " + jzlsh + " 进入住院医保登记撤销(内部)失败|" + Res.ToString());
                return new object[] { 0, 0, "住院医保登记撤销(内部)失败", Res.ToString() };
            }
        }
        #endregion

        #region 住院收费登记撤销(全部)(内部）（新医保接口）
        public static object[] NYBZYSFDJCX(object[] objParam)
        {
            string sysdate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string Ywlx = "2305";
            string jzlsh = objParam[0].ToString();
            string bxh = objParam[1].ToString();
            string xm = objParam[2].ToString();
            string kh = objParam[3].ToString();
            string ybjzlsh = objParam[4].ToString();
            string jsid = "";//结算id

            #region 老医保接口 by hjw
            //StringBuilder inputData = new StringBuilder();
            ////入参:保险号|姓名|卡号|地区编号|住院号
            //inputData.Append(bxh + "|");
            //inputData.Append(xm + "|");
            //inputData.Append(kh + "|");
            //inputData.Append(dqbh + "|");
            //inputData.Append(ybjzlsh + ";");
            //StringBuilder outData = new StringBuilder(1024);
            //StringBuilder retMsg = new StringBuilder(1024);
            //WriteLog(sysdate + "  " + jzlsh + " 进入住院收费退费(内部)...");
            //int i = f_UserBargaingApply(Ywlx, inputData, outData, retMsg);
            #endregion

            #region 新医保接口 住院收费撤销测试  by hjw
            string res = string.Empty;
            string data = "{\"mdtrt_id\":\"" + ybjzlsh + "\",\"setl_id\":\"" + jsid + "\",\"psn_no\":\"" + bxh + "\"}";
            //data = string.Format(data, jzlsh, ybjzlsh, bxh);
            WriteLog(sysdate + "  " + jzlsh + " 进入住院费用明细撤销(全部)...");
            int i = YBServiceRequest(Ywlx, data, ref res);
            #endregion

            if (i > 0)
            {
                WriteLog(sysdate + "  " + jzlsh + " 进入住院收费退费(内部)成功|" + res.ToString());
                return new object[] { 0, 1, "住院收费退费成功", res.ToString() };
            }
            else
            {
                WriteLog(sysdate + "  " + jzlsh + " 进入住院收费退费(内部)失败|" + res.ToString());
                return new object[] { 0, 0, "住院收费退费失败", res.ToString() };
            }
        }
        #endregion

        #region 住院收费结算撤销(内部)（新医保接口）
        public static object[] NYBZYSFJSCX(object[] objParam)
        {
            string sysdate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string Ywlx = "2305";
            string jzlsh = objParam[0].ToString(); //就诊流水号
            string bxh = objParam[1].ToString();    //保险号
            string xm = objParam[2].ToString();     //姓名
            string kh = objParam[3].ToString();     //卡号
            string dqbh = objParam[4].ToString();   //地区编号
            string ybjzlsh = objParam[5].ToString();//医保就诊流水号
            string djlsh = objParam[6].ToString();  //单据流水号
            string sfmxsfzf = "FALSE";



            #region 新医保接口  住院结算撤销  by hjw
            string res = "";
            string data = "{\"mdtrt_id\":\"{0}\",\"setl_id\":\"{1}\",\"psn_no\":\"{2}\"}";
            data = string.Format(data, jzlsh, jzlsh, bxh);
            WriteLog(sysdate + "  " + jzlsh + " 进入住院收费结算撤销...");
            WriteLog(sysdate + " 入参|" + data.ToString());
            int i = YBServiceRequest(Ywlx, data, ref res);

            #endregion

            if (i > 0)
            {
                WriteLog(sysdate + "  " + jzlsh + " 进入住院收费结算撤销(内部)成功" + res.ToString());
                return new object[] { 0, 1, "住院收费撤销(内部)成功|" + res.ToString() };
            }
            else
            {
                WriteLog(sysdate + "  " + jzlsh + " 进入住院收费结算撤销(内部)失败" + res.ToString());
                return new object[] { 0, 0, "住院收费撤销(内部)失败|" + res.ToString() };
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

        #region 根据日期获取年龄
        public static int GetAgeByBirthdate(DateTime birthdate)
        {
            DateTime now = DateTime.Now;
            int age = now.Year - birthdate.Year;
            if (now.Month < birthdate.Month || (now.Month == birthdate.Month && now.Day < birthdate.Day))
            {
                age--;
            }
            return age < 0 ? 0 : age;
        }
        #endregion

        #region 获取拼音码，五笔码

        public static string GetPYWB(string mc)
        {
            string strPYWB = string.Format(@"select [dbo].[f_get_PY]('{0}',10) as pym,[dbo].[f_get_WB]('{0}',10) as wbm ", mc);
            DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strPYWB, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            string PYM = ds.Tables[0].Rows[0]["pym"].ToString();
            string WBM = ds.Tables[0].Rows[0]["wbm"].ToString();
            ds.Dispose();
            return PYM + "|" + WBM;

        }
        #endregion

    }
}
