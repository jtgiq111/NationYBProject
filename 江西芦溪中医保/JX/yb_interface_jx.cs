using FastReport;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Srvtools;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Windows.Forms;
using yb_interfaces.YB.湖南;
using yb_interfaces.JX.Card;
using yb_interfaces.JX.Config;
using yb_interfaces.JX.CY;
using yb_interfaces.JX.Enum;
using yb_interfaces.JX.InPatient;
using yb_interfaces.JX.OutPatient;
using yb_interfaces.JX.OutPatient.Fee;
using yb_interfaces.JX.Register;
using yb_interfaces.JX.UI;
using yb_interfaces.JX.YB;
using Item = yb_interfaces.JX.Config.Item;
using System.Web.Script.Serialization;

namespace yb_interfaces
{
    public class yb_interface_jx
    {

        internal static int dkinit = 0;
        static yb_interface_jx()
        {
            string sysdate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            WriteLog("读卡地址：" + dkUrl);
            WriteLog("就医地区划：" + YBjyqh);
            dkinit = Init(dkUrl, YBjyqh);
            if (dkinit != 0)
            {
                WriteLog(sysdate + $"|读卡初始化失败入参：请求路径{dkUrl.ToString()},使用地区{YBjyqh.ToString()}");
                MessageBox.Show("读卡初始化失败");
            }
            else
            {
                WriteLog(sysdate + $"|读卡初始化成功入参：请求路径{dkUrl.ToString()},使用地区{YBjyqh.ToString()}");
            }
        }
        #region 老医保接口DLL加载

        [DllImport("ZRHosJK.dll", CharSet = CharSet.Ansi)]
        public static extern int f_UserBargaingInit(string UserID, string PassWD, StringBuilder retMsg);

        [DllImport("ZRHosJK.dll", CharSet = CharSet.Ansi)]
        public static extern int f_UserBargaingClose(StringBuilder retMsg);

        [DllImport("ZRHosJK.dll", CallingConvention = CallingConvention.StdCall)]
        private static extern int f_UserBargaingApply(string Ywlx, StringBuilder InData, StringBuilder OutData, StringBuilder retMsg);

        #endregion


        #region 读卡动态库
        /// <summary>
        /// 读卡初始化
        /// </summary>
        /// <param name="pUrl">医保提供的API初始化动态库地址</param>
        /// <param name="pUser">使用地区（传错会使下载的资源与所使用的设备不匹配，导致无法使用）</param>
        /// <returns></returns>
        [DllImport("SSCard.dll", CallingConvention = CallingConvention.Winapi)]
        private static extern int Init(string pUrl, string pUser);
        /// <summary>
        /// 1.2读社保卡基本信息
        /// </summary> 
        /// <param name="pOutBuff">当函数执行成功时，该输出参数为读出的社保卡信息</param>
        /// <param name="nOutBuffLen">对应pOutBuff内存分配长度，建议4096。</param>
        /// <param name="pSignBuff">当函数执行成功时，该输出参数为数字签名</param>
        /// <param name="nSignBuffLen">对应pSignBuff内存分配长度，建议4096</param>
        /// <returns></returns>
        [DllImport("SSCard.dll",  CallingConvention = CallingConvention.StdCall)]
        private static extern long ReadCardBas(StringBuilder pOutBuff, int nOutBuffLen, StringBuilder pSignBuff, int nSignBuffLen);
        /// <summary>
        /// 1.3检验PIN码
        /// </summary>
        /// <param name="pOutBuff">当函数执行成功时，此参数为空</param>
        /// <param name="nOutBuffLen">由调用者分配内存，并提供分配的长度，建议1024</param>
        /// <returns></returns>
        [DllImport("SSCard.dll", EntryPoint = "VerifyPIN", CallingConvention = CallingConvention.StdCall)]
        private static extern long VerifyPIN(string pOutBuff, int nOutBuffLen);
        /// <summary>
        /// 1.4修改PIN码
        /// </summary>
        /// <param name="pOutBuff">当函数执行成功时，此参数为空</param>
        /// <param name="nOutBuffLen">由调用者分配内存，并提供分配的长度，建议1024</param>
        /// <returns></returns>
        [DllImport("SSCard.dll", EntryPoint = "ChangePIN", CallingConvention = CallingConvention.StdCall)]
        private static extern long ChangePIN(StringBuilder pOutBuff, int nOutBuffLen);

        /// <summary>
        /// 1.5读身份证信息
        /// </summary>
        /// <param name="pOutBuff">当函数执行成功时，该输出参数为读出的身份证信息</param>
        /// <param name="nOutBuffLen">对应pOutBuff内存分配长度，建议4096。</param>
        /// <param name="pSignBuff">当函数执行成功时，该输出参数为数字签名</param>
        /// <param name="nSignBuffLen">对应pSignBuff内存分配长度，建议4096</param>
        /// <returns></returns>
        [DllImport("SSCard.dll", EntryPoint = "ReadSFZ", CallingConvention = CallingConvention.StdCall)]
        private static extern long ReadSFZ(StringBuilder pOutBuff, int nOutBuffLen, StringBuilder pSignBuff, int nSignBuffLen);
        /// <summary>
        /// 1.6读取二维码信息
        /// </summary>
        /// <param name="nTimeout">当函数执行成功时，设置的超时时间</param>
        /// <param name="pOutBuff">当函数执行成功时，该输出参数为读出的二维码信息</param>
        /// <param name="nOutBuffLen">对应pOutBuff内存分配长度，建议1024</param>
        /// <param name="pSignBuff">当函数执行成功时，该输出参数为持身份证就诊登记许可号</param>
        /// <param name="nSignBuffLen">对应pSignBuff内存分配长度，建议1024</param>
        /// <returns></returns>
        [DllImport("SSCard.dll", EntryPoint = "GetQRBase", CallingConvention = CallingConvention.StdCall)]
        private static extern long GetQRBase(int nTimeout, StringBuilder pOutBuff, int nOutBuffLen, StringBuilder pSignBuff, int nSignBuffLen);
        #endregion

        #region 变量
        public static string dqbh = "360782"; //南康

        public static string frmbzbm = "";
        public static string frmbzmc = "";
        private static string dzkh = string.Empty;
        // static IWork iWork = new Work();
        static string xmlPath = AppDomain.CurrentDomain.BaseDirectory;
        //static List<Item1> lItem = iWork.getXmlConfig1(xmlPath + "EEPNetClient.exe.config");
        static List<yb_interfaces.JX.Config.Item> lItem = Function.getXmlConfig(xmlPath + "EEPNetClient.exe.config");
        internal static string YBIP = lItem[0].YBIP;     //医保IP地址
        internal static string YBJGBH = lItem[0].DDYLJGBH;    //医疗机构编码
        internal static string YBJGMC = lItem[0].DDYLJGMC;//医疗机构名称
        internal static string YBJYBH = "";//签到返回的交易编号
        internal static string signKey = lItem[0]._api_signature;//加密规则
        internal static string XZQH = lItem[0].YBJYYBQH;
        internal static Dictionary<string, string> dicOperYbjybm = new Dictionary<string, string>();//操作员Id 对应  交易编码
        internal static string jkVersion = lItem[0].jkVersion;//接口版本号
        static string Url = $"http://{lItem[0].YBIP}" + (string.IsNullOrEmpty(lItem[0].YBPORT) ? "" : $":{lItem[0].YBPORT}") + "/CSB/hsa-fsi-";//服务地址"/fsi/api"
        static string YBjyqh = lItem[0].YBJYYBQH;
        static string DDYLJGMC = lItem[0].DDYLJGMC;
        internal static string isPing = lItem[0].isPing;
        internal static Dictionary<string, string> dicapiName = new Dictionary<string, string>() {
            { "1101","/fsiPsnInfoService/queryPsnInfo"},
            { "1201","/fsiFixMedInsService/queryFixMedIns"},
            { "1301","/catalogdownservice/downCatalog1301"},
            { "1302","/catalogdownservice/downCatalog1302"},
            { "1303","/catalogdownservice/downCatalog1303"},
            { "1304","/catalogqueryservice/queryNatyPreparedByPage"},
            { "1305","/catalogdownservice/downCatalog1305"},
            { "1306","/catalogdownservice/downCatalog1306"},
            { "1307","/catalogdownservice/downCatalog1307"},
            { "1308","/catalogdownservice/downCatalog1308"},
            { "1309","/catalogdownservice/downCatalog1309"},
            { "1310","/catalogdownservice/downCatalog1310"},
            { "1311","/catalogdownservice/downCatalog1311"},
            { "1312","/catalogqueryservice/queryHilist"},
            { "1313","/catalogdownservice/downCatalog1313"},
            { "1314","/catalogdownservice/downCatalog1314"},
            { "1315","/catalogdownservice/downCatalog1315"},
            { "1316","/catalogqueryservice/queryMedinsHilistMapByPage"},
            { "1317","/catalogqueryservice/queryMedListMapByPage"},
            { "1318","/catalogqueryservice/queryLmtprcByPage"},
            { "1319","/catalogqueryservice/querySelfpayByPage"},
            { "1901","/catalogqueryservice/queryDataDic"},
            { "2001","/fsiPsnPriorityInfoService/queryPsnPriorityInfo"},
            { "2201","/outpatientDocInfoService/outpatientRregistration"},
            { "2202","/outpatientDocInfoService/outpatientRegistrationCancel"},
            { "2203","/outpatientDocInfoService/outpatientMdtrtinfoUp"},
            { "2204","/outpatientDocInfoService/outpatientFeeListUp"},
            { "2205","/outpatientDocInfoService/outpatientFeeListUpCancel"},
            { "2206","/outpatientSettleService/preSettletment"},
            { "2207","/outpatientSettleService/saveSettletment"},
            { "2208","/outpatientSettleService/cancleSettletment"},
            { "2301","/hospFeeDtlService/feeDtlUp"},
            { "2302","/hospFeeDtlService/feeDtlCl"},
            { "2303","/hospSettService/preSett"},
            { "2304","/hospSettService/sett"},
            { "2305","/hospSettService/settCl"},
            { "2401","/hospitalRegisterService/hospitalRegisterSave"},
            { "2402","/dscgService/dischargeProcess"},
            { "2403","/hospitalRegisterService/hospitalRegisterEdit"},
            { "2404","/hospitalRegisterService/hospitalRegisterCancel"},
            { "2405","/dscgService/dischargeUndo"},
            { "2501","/hosTransferService/apply"},
            { "2502","/hosTransferService/applyCancel"},
            { "2503","/fsiOpspRegEvtService/addEvtInfo"},
            { "2504","/fsiOpspRegEvtService/revokeReg"},
            { "2505","/fsiPsnFixedRegEvtService/addEvtInfo"},
            { "2506","/fsiPsnFixedRegEvtService/revokeReg"},
            {"2507","" },
            {"2508","" },
            {"2572","http://10.74.127.103:8080/hsa-hgs-adapt/mbs/api/publicMatnTrtRegService/addEvtInfo" },
            {"2573","http://10.74.127.103:8080/hsa-hgs-adapt/mbs/api/publicMatnTrtRegService/revokeEvtReg" },
            { "2601","/reverseService/revsMethod"},
            { "3101","/riskConService/beforeAnalyze"},
            { "3102","/riskConService/courseAnalyze"},
            { "3201","/ybSettlementStmtService/stmtTotal"},
            { "3202","/ybSettlementStmtService/stmtDetail"},
            { "3301","/catalogCompService/catalogCompUp"},
            { "3302","/catalogCompService/deleteCatalogCompAudited"},
            { "3401","/department/saveDepartmentManageInfo"},
            { "3401A","/department/saveDepartmentManageBatch"},
            { "3402","/department/modifyDepartmentInfo"},
            { "3403","/department/pauseDepartmentInfo"},
            { "3501","/goodsService/goodsUpload"},
            { "3502","/goodsService/goodsUpdate"},
            { "3503","/goodsService/goodsBuy"},
            { "3504","/goodsService/goodsBuyReturn"},
            { "3505","/goodsService/goodsSell"},
            { "3506","/goodsService/goodsSellReturn"},
            { "3507","/goodsService/goodsInfoDelete"},
            { "4101","/medinfoupload/setllistinfoupld"},
            { "4201","/medinfoupload/ownpaypatnmdtrtupld"},
            { "4301","/outemergency/addFixmedinsOtpTrt"},
            { "4302","/outemergency/addFixmedinsEmerObhos"},
            { "4401","/iptInfoUploadService/iptInfoUpload"},
            { "4402","/hospDocOrdInfoService/hospOrdUpload"},
            { "4501","/clncasstbizupload/clncexamrcdupld"},
            { "4502","/clncasstbizupload/clnctestrcdupld"},
            { "4503","/clncasstbizupload/bctlclterpotrcdupld"},
            { "4504","/clncasstbizupload/sstbrcdrpotrcdupld"},
            { "4505","/clncasstbizupload/palgexamrpotrcdupld"},
            { "4506","/clncasstbizupload/unstructuredrpotrcdupld"},
            { "4601","/fixmedinsBldRcdCService/saveBldRcdC"},
            { "4602","/fixmedinsBldRcdCService/saveNurscareOprtRcdC "},
            { "4701","/fsiFixmedinsMedrcdService/insertFixmedinsMedrcdInfo "},
            { "5101","/fsiIntegratedQueryService/queryFixmedinsDept "},
            { "5102","/fsiIntegratedQueryService/queryFixmedinsPracPsn"},
            { "5201","/fsiIntegratedQueryService/queryMdtrtInfo"},
            { "5202","/fsiIntegratedQueryService/querySetlDiseList"},
            { "5203","/fsiIntegratedQueryService/querySetlInfo"},
            { "5204","/fsiIntegratedQueryService/queryFeeList"},
            { "5205","/fsiIntegratedQueryService/querypsnmedcrcd"},
            { "5206","/fsiIntegratedQueryService/queryFixmedinsPracPsnSum"},
            { "5301","/fsiIntegratedQueryService/queryPsnOpspReg"},
            { "5302","/fsiIntegratedQueryService/query"},
            { "5303","/fsiIntegratedQueryService/queryInhospInfo"},
            { "5304","/fsiIntegratedQueryService/queryTranferhospRegis "},
            { "5401","/mutualRecognitionResultQueryService/queryProMutualRecognitionInfo"},
            { "5402","/mutualRecognitionResultQueryService/queryRptdetailinfo"},
            { "9001","/signInSignOutService/signIn"},
            { "9002","/signInSignOutService/signOut"},
            { "9101","/fileupload/upload"},
            { "9102","/fileupload/download"}

        };
        internal static string dkUrl = $"http://{lItem[0].YBIP}/1.0.0/agent-card-init";
        string testId = "";
        #region 医保读卡测试数据 
        internal static string mdtrt_cert_type = "02";// 
        internal static string mdtrt_cert_no = lItem[0].IdCard;
        internal static string card_sn = "";
        internal static string begntime = "";
        internal static string psn_cert_type = "";
        internal static string certno = "";
        internal static string psn_name = "";
        #endregion
        /// <summary>
        /// 版本号
        /// </summary>
        private static string _api_version = lItem[0]._api_version;
        /// <summary>
        /// 时间戳
        /// </summary>
        private static string _api_timestamp = System.DateTime.Now.ToString("yyyyMMddHHmmss");//lItem[0]._api_timestamp;
        /// <summary>
        /// 秘钥
        /// </summary>
        private static string _api_access_key = lItem[0]._api_access_key;
        /// <summary>
        /// 签名
        /// </summary>
        private static string _api_signature = lItem[0]._api_signature;
        #endregion

        #region 新医保接口

        /// <summary>
        /// 获取时间戳
        /// </summary>
        /// <returns></returns>
        public static string GetTimeStamp()
        {
            //TimeSpan ts = DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            //return Convert.ToInt64(ts.TotalSeconds).ToString();
            //TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            //return Convert.ToInt64(ts.TotalMilliseconds).ToString();
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1, 0, 0, 0, 0));
            long t = (DateTime.Now.Ticks - startTime.Ticks) / 10000;   //除10000调整为13位      
            return t.ToString();
        }




        #region HMACSHA1加密  将二进制数据直接转为字符串返回
        /// <summary>
        /// HMACSHA1加密
        /// </summary>
        /// <param name="text">要加密的原串</param>
        ///<param name="key">私钥</param>
        /// <returns></returns>
        public static string HMACSHA1Text(string text, string key)
        {
            //HMACSHA1加密
            HMACSHA1 hmacsha1 = new HMACSHA1();
            hmacsha1.Key = System.Text.Encoding.UTF8.GetBytes(key);

            byte[] dataBuffer = System.Text.Encoding.UTF8.GetBytes(text);
            byte[] hashBytes = hmacsha1.ComputeHash(dataBuffer);

            var enText = new StringBuilder();
            foreach (byte iByte in hashBytes)
            {
                enText.AppendFormat("{0:x2}", iByte);
            }
            return enText.ToString();

        }
        #endregion

        #region HMACSHA1加密  对二进制数据转Base64后再返回
        /// <summary>
        /// HMACSHA1加密
        /// </summary>
        /// <param name="text">要加密的原串</param>
        ///<param name="key">私钥</param>
        /// <returns></returns>
        public static string HMACSHA1TextBase64(string text, string key)
        {
            //HMACSHA1加密
            HMACSHA1 hmacsha1 = new HMACSHA1();
            hmacsha1.Key = System.Text.Encoding.UTF8.GetBytes(key);

            byte[] dataBuffer = System.Text.Encoding.UTF8.GetBytes(text);
            byte[] hashBytes = hmacsha1.ComputeHash(dataBuffer);

            return Convert.ToBase64String(hashBytes);

        }
        #endregion

        #region 赋值报文头
        public static void SetHeaderValue(WebHeaderCollection header, string name, string value)
        {
            var property = typeof(WebHeaderCollection).GetProperty("InnerCollection", BindingFlags.Instance | BindingFlags.NonPublic);
            if (property != null)
            {
                if (property.GetValue(header, null) is NameValueCollection)
                {
                    NameValueCollection collection = property.GetValue(header, null) as NameValueCollection;
                    collection[name] = value;
                }
            }
        }
        #endregion

        #region 压缩文件
        /// <summary>
        /// 解压RAR和ZIP文件(需存在Winrar.exe(只要自己电脑上可以解压或压缩文件就存在Winrar.exe))
        /// </summary>
        /// <param name="UnPath">解压后文件保存目录</param>
        /// <param name="rarPathName">待解压文件存放绝对路径（包括文件名称）</param>
        /// <param name="IsCover">所解压的文件是否会覆盖已存在的文件(如果不覆盖,所解压出的文件和已存在的相同名称文件不会共同存在,只保留原已存在文件)</param>
        /// <param name="PassWord">解压密码(如果不需要密码则为空)</param>
        /// <returns>true(解压成功);false(解压失败)</returns>
        public static bool UnRarOrZip(string UnPath, string rarPathName, bool IsCover, string PassWord)
        {
            if (!Directory.Exists(UnPath))
                Directory.CreateDirectory(UnPath);
            Process Process1 = new Process();
            Process1.StartInfo.FileName = "Winrar.exe";
            Process1.StartInfo.CreateNoWindow = true;
            string cmd = "";
            if (!string.IsNullOrEmpty(PassWord) && IsCover)
                //解压加密文件且覆盖已存在文件( -p密码 )
                cmd = string.Format(" x -p{0} -o+ {1} {2} -y", PassWord, rarPathName, UnPath);
            else if (!string.IsNullOrEmpty(PassWord) && !IsCover)
                //解压加密文件且不覆盖已存在文件( -p密码 )
                cmd = string.Format(" x -p{0} -o- {1} {2} -y", PassWord, rarPathName, UnPath);
            else if (IsCover)
                //覆盖命令( x -o+ 代表覆盖已存在的文件)
                cmd = string.Format(" x -o+ {0} {1} -y", rarPathName, UnPath);
            else
                //不覆盖命令( x -o- 代表不覆盖已存在的文件)
                cmd = string.Format(" x -o- {0} {1} -y", rarPathName, UnPath);
            //命令
            Process1.StartInfo.Arguments = cmd;
            Process1.Start();
            Process1.WaitForExit();//无限期等待进程 winrar.exe 退出
            //Process1.ExitCode==0指正常执行，Process1.ExitCode==1则指不正常执行
            if (Process1.ExitCode == 0)
            {
                Process1.Close();
                return true;
            }
            else
            {
                Process1.Close();
                return false;
            }

        }
        #endregion

        #region 新医保接口调用请求的方式
        internal static string filenameXZ = "";
        public static string GetYBPostRequest(string url, string ywCode, string data, ref int status)
        {
            //创建Web访问对象
            try
            {
                string filename = "";
                string baseUrl = "";
                string filePath = "";
                FileStream fs = null;
                if (ywCode == "9102")
                {
                    filename = filenameXZ;
                    baseUrl = System.AppDomain.CurrentDomain.BaseDirectory;
                    filePath = Path.Combine(baseUrl, "YBLog\\" + filename);
                    if (File.Exists(filePath))
                    {
                        fs = new FileStream(filePath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);

                    }
                    else
                    {
                        fs = new FileStream(filePath, FileMode.Create);
                    }
                }
                HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(url);
                //把用户传过来的数据转成“UTF-8”的字节流
                byte[] buf = System.Text.Encoding.GetEncoding("UTF-8").GetBytes(data);
                myRequest.Method = "POST";
                //string contentType = ywCode == "9102" ? "application/x-www-form-urlencoded" : "application/json;charset=utf-8";
                string contentType = "application/json;charset=utf-8";
                myRequest.ContentLength = buf.Length;
                myRequest.ContentType = contentType;
                #region 暂时屏蔽
                myRequest.MaximumAutomaticRedirections = 1;
                myRequest.AllowAutoRedirect = true;
                myRequest.Accept = "*";//添加Accept
                                       //myRequest.UserAgent = @"User - Agent: Mozilla / 5.0(Windows NT 10.0; Win64; x64) AppleWebKit / 537.36(KHTML, like Gecko) Chrome / 64.0.3282.140 Safari / 537.36 Edge / 17.17134"; //添加UA
                                       //myRequest.CookieContainer = new CookieContainer(); //添加cookies
                                       ////if (refer != null) myRequest.Referer = refer; //添加refer 
                #endregion
                #region 加密
                string timeStamp = GetTimeStamp();
                #region headers
                SetHeaderValue(myRequest.Headers, "Content-Type", contentType);

                WriteLog($"Content-Type========{contentType}");
                //SetHeaderValue(myRequest.Headers, "Accept", "application/json");
                //SetHeaderValue(myRequest.Headers, "Accept-Charset", "utf-8");
                SetHeaderValue(myRequest.Headers, "_api_timestamp", timeStamp);
                WriteLog($"_api_timestamp========{timeStamp}");
                SetHeaderValue(myRequest.Headers, "_api_name", "hsa-fsi-" + ywCode);//"yfb-" + 
                WriteLog($"_api_name========{"hsa-fsi-" + ywCode}");
                //SetHeaderValue(myRequest.Headers, "_api_signature", "rEsfRVPQ9oRKo9XkHew9hnF8gfs=");

                var param = "_api_access_key=" + _api_access_key + "&_api_name=hsa-fsi-" + ywCode + "&_api_timestamp=" + timeStamp + "&_api_version=" + _api_version;//yfb-
                var signature = HMACSHA1TextBase64(param, signKey);
                SetHeaderValue(myRequest.Headers, "_api_signature", signature);
                WriteLog($"_api_signature========{signature}");

                SetHeaderValue(myRequest.Headers, "_api_version", _api_version);
                WriteLog($"_api_version========{_api_version}");
                //SetHeaderValue(myRequest.Headers, "_api_access_key", "8a03e558c72542adb01bf5b16428e4c0");
                SetHeaderValue(myRequest.Headers, "_api_access_key", _api_access_key);
                WriteLog($"_api_access_key========{_api_access_key}");
                #endregion
                #endregion

                // myRequest.Timeout = 10000;
                if (fs != null)
                {
                    myRequest.AddRange((int)fs.Length);
                }
                // myRequest.ProtocolVersion = new Version(1, 1);
                myRequest.KeepAlive = true;
                Stream stream1 = myRequest.GetRequestStream();
                stream1.Write(buf, 0, buf.Length);
                stream1.Close();

                //获取接口返回值
                //通过Web访问对象获取响应内容
                HttpWebResponse myResponse = (HttpWebResponse)myRequest.GetResponse();
                long contentLength = myResponse.ContentLength;
                //通过响应内容流创建StreamReader对象，因为StreamReader更高级更快
                StreamReader reader = new StreamReader(myResponse.GetResponseStream(), Encoding.UTF8);

                string returnJson = string.Empty;
                #region 文件下载
                if (ywCode.Equals("9102"))
                {
                    //创建本地文件写入流
                    Stream responseStream = myResponse.GetResponseStream();



                    byte[] bArr = new byte[1024];

                    int size = responseStream.Read(bArr, 0, (int)bArr.Length);

                    while (size > 0)

                    {
                        fs.Write(bArr, 0, size);
                        size = responseStream.Read(bArr, 0, (int)bArr.Length);

                    }
                    fs.Close();
                    responseStream.Close();


                    UnRarOrZip("YBlog", "YBLog\\" + filename, true, "");
                    returnJson = "";
                }
                #endregion
                else
                    returnJson = reader.ReadToEnd();//利用StreamReader就可以从响应内容从头读到尾
                reader.Close();
                myResponse.Close();
                status = 1;
                return returnJson;
            }
            catch (Exception ex)
            {

                status = -1;
                return "发送请求时错误：" + ex.ToString();

            }
        }
        #endregion

        #region 获取时间戳
        public static long getTime13()
        {
            //ToUniversalTime()转换为标准时区的时间,去掉的话直接就用北京时间
            TimeSpan ts = DateTime.Now.ToUniversalTime() - new DateTime(1970, 1, 1);
            //得到精确到毫秒的时间戳（长度13位）
            long time = (long)ts.TotalMilliseconds;
            return time;
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
            string apiName = dicapiName[YwCode].ToString();
            string newStr = $"_api_access_key={_api_access_key}&_api_name={apiName}&_api_timestamp={GetTimeStamp()}&_api_version={_api_version}";
            HMACSHA1 hmacsha1 = new HMACSHA1(Encoding.UTF8.GetBytes(signKey));

            byte[] bytetext = hmacsha1.ComputeHash(Encoding.UTF8.GetBytes(newStr));
            byte[] dataBuffer = System.Text.Encoding.UTF8.GetBytes(signKey);

            return Convert.ToBase64String(dataBuffer);
            #region 方法2
            //HMACSHA1 hMACSHA1 = new HMACSHA1();
            //hMACSHA1.Key = System.Text.Encoding.UTF8.GetBytes(signKey);
            //byte[] dataBuffer = System.Text.Encoding.UTF8.GetBytes(newStr);
            //byte[] hashBytes = hMACSHA1.ComputeHash(dataBuffer);
            //string hmaEcyostr= Convert.ToBase64String(hashBytes);
            //string base64str = Convert.ToBase64String(Encoding.UTF8.GetBytes(hmaEcyostr));
            //return hmaEcyostr; 
            #endregion

            //Encoding encode = Encoding.UTF8;
            //byte[] byteData = encode.GetBytes(newStr);
            //byte[] byteKey = encode.GetBytes(signKey);
            //HMACSHA1 hmac = new HMACSHA1(byteKey);
            //CryptoStream cs = new CryptoStream(Stream.Null, hmac, CryptoStreamMode.Write);
            //cs.Write(byteData, 0, byteData.Length);
            //cs.Close();
            //return Convert.ToBase64String(hmac.Hash);
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
        /// 就医区划  异地就医必填
        /// </summary>
        internal static string insuplc_admdvs = "";
        /// <summary>
        /// 医保服务统一访问方法
        /// </summary>
        /// <param name="YwCode">业务编码</param>
        /// <param name="data">入参</param>
        /// <param name="Result">接收结果返回</param>
        /// <returns></returns>
        public static int YBServiceRequest(string YwCode, dynamic dy, ref string Result)
        {
            string EmplId = CliUtils.fLoginUser;
            string EmplName = CliUtils.fUserName;
            //JObject dy = JsonConvert.DeserializeObject<JObject>(data);
            if (string.IsNullOrEmpty(insuplc_admdvs))
            {
                insuplc_admdvs = XZQH;
            }
            RequestModel req = new RequestModel()
            {
                infno = YwCode,
                msgid = YBJGBH + System.DateTime.Now.ToString("yyyyMMddHHmmss") + getSeq(),
                mdtrtarea_admvs = YBjyqh,
                insuplc_admdvs = insuplc_admdvs,
                recer_sys_code = "his",
                dev_no = "",
                dev_safe_info = "",
                cainfo = "",
                signtype = "",
                infver = jkVersion,
                opter_type = 1,
                opter = EmplId,
                opter_name = EmplName,
                inf_time = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                fixmedins_code = YBJGBH,
                fixmedins_name = YBJGMC,
                sign_no = YbSignNo,
                input = dy
            };

            //将参数反序列化成json
            string RequestData = JsonConvert.SerializeObject(req);
            //  RequestData = RequestData.Replace("\\", ""); //.Replace("\"{", "{").Replace("}\"", "}"); 
            string sysdate = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            WriteLog(sysdate + $"业务编号为{YwCode},请求入参:" + RequestData);
            int requestStatus = 0;

            //if (YwCode == "1101")
            //{
            //    YwCode = "yfb-" + YwCode;
            //} 
            //string apiName = dicapiName[YwCode].ToString();
            //string url = Url + apiName;//+ "yfb-"
            string url = Url + YwCode;
            string[] urlts = { "2572", "2573" };
            if (urlts.Contains(YwCode))
            {
                url = dicapiName[YwCode].ToString();
            }
            WriteLog(sysdate + "请求路径：" + url);
            //string filename = filenameXZ;
            //string baseUrl = System.AppDomain.CurrentDomain.BaseDirectory;
            //string filePath = Path.Combine(baseUrl, "YBLog\\" + filename);
            //医保接口发送请求
            string Err = string.Empty;
            Err = GetYBPostRequest(url, YwCode, RequestData, ref requestStatus);

            WriteLog(sysdate + $"Api接口业务编号为{YwCode},请求回参:" + Err);
            string fileName = YwCode + "_" + DateTime.Now.ToString("yyyyMMddHHmmssfff");
            if (YwCode.Equals("9102") && !string.IsNullOrEmpty(fileName))
            {
                Result = "下载接口调用成功！";
                return 1;
            }
            ResponseModel model = null;
            try
            {
                model = JsonConvert.DeserializeObject<ResponseModel>(Err);
            }
            catch
            {
                WriteLog("进入新回参" + Err);
                JObject j = JObject.Parse(Err);
                string code = j.GetValue("infcode", StringComparison.OrdinalIgnoreCase).ToString();
                WriteLog("进入新回参code:" + code);
                if (code == "0")
                {
                    if (j.Property("output") == null)
                    {
                        Result = "";
                    }
                    else
                    {
                        Result = j.GetValue("output").ToString();
                    }
                    return 1;
                }
                else
                {
                    if (j.Property("output") == null)
                    {
                        Result = "";
                    }
                    else
                    {
                        Result = j.GetValue("output").ToString();
                    }
                    WriteLog(sysdate + "接口返回错误！" + Err);
                    return -1;
                }
            }
            if (model.infcode == "0")
            {
                Result = JsonConvert.SerializeObject(model.output);
                return 1;
            }
            else
            {
                WriteLog(sysdate + "接口返回错误最后！" + Err);
                Result = model.err_msg;
                return -1;
            }
            if (model.infcode == null || model.infcode == "null")
            {
                JObject j = JsonConvert.DeserializeObject<JObject>(Err);
                WriteLog(sysdate + "接口返回错误！" + Err);
                Result = Err;
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


        #region 其它信息获取与下载

        #region 下载
        #region 医药机构信息获取
        public static object[] YBYYJGXXHQ(object[] objParam)
        {
            string sysdate = GetServerDateTime();
            dynamic data = new
            {
                data = new
                {
                    fixmedins_type = "1",
                    fixmedins_name = YBJGMC,
                    fixmedins_code = YBJGBH
                }
            };



            string inputstr = JsonConvert.SerializeObject(data);
            //入参
            WriteLog(sysdate + "  进入医药机构信息获取...");
            WriteLog(sysdate + "  入参" + inputstr.ToString());
            string errstr = "";
            int i = YBServiceRequest("1201", data, ref errstr);
            if (i > 0)
            {

                WriteLog(sysdate + "  医药机构信息获取成功|" + errstr.ToString());
                JObject jobj = JsonConvert.DeserializeObject<JObject>(errstr);
                string fixmedins_type = jobj.GetValue("fixmedins_type", StringComparison.OrdinalIgnoreCase).ToString();
                string uscc = jobj.GetValue("uscc", StringComparison.OrdinalIgnoreCase).ToString();
                string hosp_lv = jobj.GetValue("hosp_lv", StringComparison.OrdinalIgnoreCase).ToString();
                List<string> liSQL = new List<string>();
                string strSql = string.Format(@" update bztbd set bzmem8='{0}',bzmem9='{1}',bzmem10='{1}' where bzcodn='CM' ", fixmedins_type, uscc, hosp_lv);
                liSQL.Add(strSql);
                object[] obj = liSQL.ToArray();
                obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);
                if (obj[1].ToString().Equals("1"))
                {
                    WriteLog(sysdate + "  医药机构信息获取成功" + errstr.ToString());
                    return new object[] { 0, 1, "医药机构信息获取成功！" };
                }
                else
                {
                    WriteLog(sysdate + "  医药机构信息获取失败|" + obj[2].ToString());
                    return new object[] { 0, 0, obj[2].ToString() };
                }
            }
            else
            {
                return new object[] { 0, 0, errstr };
            }
        }
        #endregion

        #region 目录下载
        public static object[] YBMLXZ(object[] objParam)
        {
            string sysdate = GetServerDateTime();
            string xzywbh = objParam[0].ToString();     //业务编号
            string crver = objParam[1].ToString();      //版本号 
            dynamic data = new
            {
                data = new
                {
                    ver = crver
                }
            };


            string errstr = string.Empty;
            //入参
            WriteLog(sysdate + "  进入目录下载...");
            string inJosn = Newtonsoft.Json.JsonConvert.SerializeObject(data);
            WriteLog(sysdate + "  入参" + inJosn.ToString());
            int i = YBServiceRequest(xzywbh, data, ref errstr);
            WriteLog(sysdate + "  出参" + errstr);
            if (i > 0)
            {
                JObject jobj = JObject.Parse(errstr);
                string file_qury_no = jobj.GetValue("file_qury_no", StringComparison.OrdinalIgnoreCase).ToString();
                string filename = jobj.GetValue("filename", StringComparison.OrdinalIgnoreCase).ToString();
                filenameXZ = filename;
                string dld_end_time = jobj.GetValue("dld_end_time", StringComparison.OrdinalIgnoreCase) == null ? "" : jobj.GetValue("dld_end_time", StringComparison.OrdinalIgnoreCase).ToString();
                string data_cnt = jobj.GetValue("data_cnt", StringComparison.OrdinalIgnoreCase).ToString();

                WriteLog(sysdate + "  目录下载成功|" + errstr.ToString());
                List<string> XZBH = new List<string>();
                XZBH.Add("1301");
                XZBH.Add("1302");
                XZBH.Add("1303");
                XZBH.Add("1305");
                XZBH.Add("1306");
                XZBH.Add("1307");
                XZBH.Add("1308");
                XZBH.Add("1309");
                XZBH.Add("1310");
                XZBH.Add("1311");
                XZBH.Add("1313");
                XZBH.Add("1314");
                XZBH.Add("1315");
                if (XZBH.Contains(xzywbh))
                {
                    YBWJXZ(new object[] { file_qury_no, filename });
                }
                return new object[] { 0, 1, filename };
            }
            else
            {
                return new object[] { 0, 0, errstr };
            }

        }
        #endregion

        #region 医疗机构制剂目录下载
        public static object[] YLJGZJMLXZ(object[] objParam)
        {
            string sysdate = GetServerDateTime();
            string xzywbh = objParam[0].ToString();     //业务编号
            string crver = objParam[1].ToString();      //版本号
            dynamic data = new
            {
                data = new
                {
                    ver = crver
                }
            };


            string errstr = string.Empty;
            //入参
            WriteLog(sysdate + "  进入医疗机构制剂目录下载...");
            string inJosn = Newtonsoft.Json.JsonConvert.SerializeObject(data);
            WriteLog(sysdate + "  入参" + inJosn.ToString());
            int i = YBServiceRequest("1303", data, ref errstr);
            WriteLog(sysdate + "  出参" + errstr);
            if (i > 0)
            {
                JObject jobj = JObject.Parse(errstr);
                string file_qury_no = jobj.GetValue("file_qury_no", StringComparison.OrdinalIgnoreCase).ToString();
                string filename = jobj.GetValue("filename", StringComparison.OrdinalIgnoreCase).ToString();
                string dld_end_time = jobj.GetValue("dld_end_time", StringComparison.OrdinalIgnoreCase) == null ? "" : jobj.GetValue("dld_end_time", StringComparison.OrdinalIgnoreCase).ToString();
                string data_cnt = jobj.GetValue("data_cnt", StringComparison.OrdinalIgnoreCase).ToString();

                WriteLog(sysdate + "  医疗机构制剂目录下载成功|" + errstr.ToString());
                List<string> XZBH = new List<string>();
                XZBH.Add("1301");
                XZBH.Add("1302");
                XZBH.Add("1303");
                XZBH.Add("1305");
                XZBH.Add("1306");
                XZBH.Add("1307");
                XZBH.Add("1308");
                XZBH.Add("1309");
                XZBH.Add("1310");
                XZBH.Add("1311");
                XZBH.Add("1313");
                XZBH.Add("1314");
                XZBH.Add("1315");
                if (XZBH.Contains(xzywbh))
                {
                    YBWJXZ(new object[] { file_qury_no, filename });
                }
                return new object[] { 0, 1, filename };
            }
            else
            {
                return new object[] { 0, 0, errstr };
            }

        }
        #endregion

        #region 医疗服务项目目录下载
        public static object[] YLFWXMMLXZ(object[] objParam)
        {
            string sysdate = GetServerDateTime();
            string xzywbh = objParam[0].ToString();     //业务编号
            string crver = objParam[1].ToString();      //版本号
            dynamic data = new
            {
                data = new
                {
                    ver = "0"
                }
            };


            string errstr = string.Empty;
            //入参
            WriteLog(sysdate + "  进入医疗服务项目目录下载...");
            string inJosn = Newtonsoft.Json.JsonConvert.SerializeObject(data);
            WriteLog(sysdate + "  入参" + inJosn.ToString());
            int i = YBServiceRequest("1305", inJosn, ref errstr);
            WriteLog(sysdate + "  出参" + errstr);
            if (i > 0)
            {
                JObject jobj = JObject.Parse(errstr);
                string file_qury_no = jobj.GetValue("file_qury_no", StringComparison.OrdinalIgnoreCase).ToString();
                string filename = jobj.GetValue("filename", StringComparison.OrdinalIgnoreCase).ToString();
                string dld_end_time = jobj.GetValue("dld_end_time", StringComparison.OrdinalIgnoreCase) == null ? "" : jobj.GetValue("dld_end_time", StringComparison.OrdinalIgnoreCase).ToString();
                string data_cnt = jobj.GetValue("data_cnt", StringComparison.OrdinalIgnoreCase).ToString();

                WriteLog(sysdate + "  医疗服务项目目录下载成功|" + errstr.ToString());
                List<string> XZBH = new List<string>();
                XZBH.Add("1301");
                XZBH.Add("1302");
                XZBH.Add("1303");
                XZBH.Add("1305");
                XZBH.Add("1306");
                XZBH.Add("1307");
                XZBH.Add("1308");
                XZBH.Add("1309");
                XZBH.Add("1310");
                XZBH.Add("1311");
                XZBH.Add("1313");
                XZBH.Add("1314");
                XZBH.Add("1315");
                if (XZBH.Contains(xzywbh))
                {
                    YBWJXZ(new object[] { file_qury_no, filename });
                }
                return new object[] { 0, 1, filename };
            }
            else
            {
                return new object[] { 0, 0, errstr };
            }

        }
        #endregion

        #region 医用耗材目录下载
        public static object[] YLHCMLXZ(object[] objParam)
        {
            string sysdate = GetServerDateTime();
            string xzywbh = objParam[0].ToString();     //业务编号
            string crver = objParam[1].ToString();      //版本号
            dynamic data = new
            {
                data = new
                {
                    ver = crver
                }

            };


            string errstr = string.Empty;
            //入参
            WriteLog(sysdate + "  进入医用耗材目录下载...");
            string inJosn = Newtonsoft.Json.JsonConvert.SerializeObject(data);
            WriteLog(sysdate + "  入参" + inJosn.ToString());
            int i = YBServiceRequest("1306", data, ref errstr);
            WriteLog(sysdate + "  出参" + errstr);
            if (i > 0)
            {
                JObject jobj = JObject.Parse(errstr);
                string file_qury_no = jobj.GetValue("file_qury_no", StringComparison.OrdinalIgnoreCase).ToString();
                string filename = jobj.GetValue("filename", StringComparison.OrdinalIgnoreCase).ToString();
                string dld_end_time = jobj.GetValue("dld_end_time", StringComparison.OrdinalIgnoreCase) == null ? "" : jobj.GetValue("dld_end_time", StringComparison.OrdinalIgnoreCase).ToString();
                string data_cnt = jobj.GetValue("data_cnt", StringComparison.OrdinalIgnoreCase).ToString();

                WriteLog(sysdate + "  医用耗材目录下载成功|" + errstr.ToString());
                List<string> XZBH = new List<string>();
                XZBH.Add("1301");
                XZBH.Add("1302");
                XZBH.Add("1303");
                XZBH.Add("1305");
                XZBH.Add("1306");
                XZBH.Add("1307");
                XZBH.Add("1308");
                XZBH.Add("1309");
                XZBH.Add("1310");
                XZBH.Add("1311");
                XZBH.Add("1313");
                XZBH.Add("1314");
                XZBH.Add("1315");
                if (XZBH.Contains(xzywbh))
                {
                    YBWJXZ(new object[] { file_qury_no, filename });
                }
                return new object[] { 0, 1, filename };
            }
            else
            {
                return new object[] { 0, 0, errstr };
            }

        }
        #endregion

        #region 疾病与诊断目录下载
        public static object[] JBYZDMLXZ(object[] objParam)
        {
            string sysdate = GetServerDateTime();
            string xzywbh = objParam[0].ToString();     //业务编号
            string crver = objParam[1].ToString();      //版本号
            dynamic data = new
            {
                data = new
                {
                    ver = crver
                }
            };


            string errstr = string.Empty;
            //入参
            WriteLog(sysdate + "  进入疾病与诊断目录下载...");
            string inJosn = Newtonsoft.Json.JsonConvert.SerializeObject(data);
            WriteLog(sysdate + "  入参" + inJosn.ToString());
            int i = YBServiceRequest("1307", data, ref errstr);
            WriteLog(sysdate + "  出参" + errstr);
            if (i > 0)
            {
                JObject jobj = JObject.Parse(errstr);
                string file_qury_no = jobj.GetValue("file_qury_no", StringComparison.OrdinalIgnoreCase).ToString();
                string filename = jobj.GetValue("filename", StringComparison.OrdinalIgnoreCase).ToString();
                string dld_end_time = jobj.GetValue("dld_end_time", StringComparison.OrdinalIgnoreCase) == null ? "" : jobj.GetValue("dld_end_time", StringComparison.OrdinalIgnoreCase).ToString();
                string data_cnt = jobj.GetValue("data_cnt", StringComparison.OrdinalIgnoreCase).ToString();

                WriteLog(sysdate + "  疾病与诊断目录下载成功|" + errstr.ToString());
                List<string> XZBH = new List<string>();
                XZBH.Add("1301");
                XZBH.Add("1302");
                XZBH.Add("1303");
                XZBH.Add("1305");
                XZBH.Add("1306");
                XZBH.Add("1307");
                XZBH.Add("1308");
                XZBH.Add("1309");
                XZBH.Add("1310");
                XZBH.Add("1311");
                XZBH.Add("1313");
                XZBH.Add("1314");
                XZBH.Add("1315");
                if (XZBH.Contains(xzywbh))
                {
                    YBWJXZ(new object[] { file_qury_no, filename });
                }
                return new object[] { 0, 1, filename };
            }
            else
            {
                return new object[] { 0, 0, errstr };
            }

        }
        #endregion


        #region 门诊慢特病种目录下载
        public static object[] MZMTBZMLXZ(object[] objParam)
        {
            string sysdate = GetServerDateTime();
            string xzywbh = objParam[0].ToString();     //业务编号
            string crver = objParam[1].ToString();      //版本号
            dynamic data = new
            {
                data = new
                {
                    ver = "0"
                }
            };


            string errstr = string.Empty;
            //入参
            WriteLog(sysdate + "  进入门诊慢特病种目录下载...");
            string inJosn = Newtonsoft.Json.JsonConvert.SerializeObject(data);
            WriteLog(sysdate + "  入参" + inJosn.ToString());
            int i = YBServiceRequest(xzywbh, data, ref errstr);
            WriteLog(sysdate + "  出参" + errstr);
            if (i > 0)
            {
                JObject jobj = JObject.Parse(errstr);
                string file_qury_no = jobj.GetValue("file_qury_no", StringComparison.OrdinalIgnoreCase).ToString();
                string filename = jobj.GetValue("filename", StringComparison.OrdinalIgnoreCase).ToString();
                string dld_end_time = jobj.GetValue("dld_end_time", StringComparison.OrdinalIgnoreCase) == null ? "" : jobj.GetValue("dld_end_time", StringComparison.OrdinalIgnoreCase).ToString();
                string data_cnt = jobj.GetValue("data_cnt", StringComparison.OrdinalIgnoreCase).ToString();

                WriteLog(sysdate + "  门诊慢特病种目录下载成功|" + errstr.ToString());
                List<string> XZBH = new List<string>();
                XZBH.Add("1301");
                XZBH.Add("1302");
                XZBH.Add("1303");
                XZBH.Add("1305");
                XZBH.Add("1306");
                XZBH.Add("1307");
                XZBH.Add("1308");
                XZBH.Add("1309");
                XZBH.Add("1310");
                XZBH.Add("1311");
                XZBH.Add("1313");
                XZBH.Add("1314");
                XZBH.Add("1315");
                if (XZBH.Contains(xzywbh))
                {
                    YBWJXZ(new object[] { file_qury_no, filename });
                }
                return new object[] { 0, 1, filename };
            }
            else
            {
                return new object[] { 0, 0, errstr };
            }

        }
        #endregion

        #region 手术操作目录下载
        public static object[] SSCZMLXZ(object[] objParam)
        {
            string sysdate = GetServerDateTime();
            string xzywbh = objParam[0].ToString();     //业务编号
            string crver = objParam[1].ToString();      //版本号
            dynamic data = new
            {
                data = new
                {
                    ver = crver
                }

            };


            string errstr = string.Empty;
            //入参
            WriteLog(sysdate + "  进入手术操作目录下载...");
            string inJosn = Newtonsoft.Json.JsonConvert.SerializeObject(data);
            WriteLog(sysdate + "  入参" + inJosn.ToString());
            int i = YBServiceRequest("1308", data, ref errstr);
            WriteLog(sysdate + "  出参" + errstr);
            if (i > 0)
            {
                JObject jobj = JObject.Parse(errstr);
                string file_qury_no = jobj.GetValue("file_qury_no", StringComparison.OrdinalIgnoreCase).ToString();
                string filename = jobj.GetValue("filename", StringComparison.OrdinalIgnoreCase).ToString();
                string dld_end_time = jobj.GetValue("dld_end_time", StringComparison.OrdinalIgnoreCase) == null ? "" : jobj.GetValue("dld_end_time", StringComparison.OrdinalIgnoreCase).ToString();
                string data_cnt = jobj.GetValue("data_cnt", StringComparison.OrdinalIgnoreCase).ToString();

                WriteLog(sysdate + "  手术操作目录下载成功|" + errstr.ToString());
                List<string> XZBH = new List<string>();
                XZBH.Add("1301");
                XZBH.Add("1302");
                XZBH.Add("1303");
                XZBH.Add("1305");
                XZBH.Add("1306");
                XZBH.Add("1307");
                XZBH.Add("1308");
                XZBH.Add("1309");
                XZBH.Add("1310");
                XZBH.Add("1311");
                XZBH.Add("1313");
                XZBH.Add("1314");
                XZBH.Add("1315");
                if (XZBH.Contains(xzywbh))
                {
                    YBWJXZ(new object[] { file_qury_no, filename });
                }
                return new object[] { 0, 1, filename };
            }
            else
            {
                return new object[] { 0, 0, errstr };
            }

        }
        #endregion

        #region 按病种付费病种目录下载
        public static object[] ABZFFBZMLXZ(object[] objParam)
        {
            string sysdate = GetServerDateTime();
            string xzywbh = objParam[0].ToString();     //业务编号
            string crver = objParam[1].ToString();      //版本号
            dynamic data = new
            {
                data = new
                {
                    ver = crver
                }
            };


            string errstr = string.Empty;
            //入参
            WriteLog(sysdate + "  进入按病种付费病种目录下载...");
            string inJosn = Newtonsoft.Json.JsonConvert.SerializeObject(data);
            WriteLog(sysdate + "  入参" + inJosn.ToString());
            int i = YBServiceRequest("1310", data, ref errstr);
            WriteLog(sysdate + "  出参" + errstr);
            if (i > 0)
            {
                JObject jobj = JObject.Parse(errstr);
                string file_qury_no = jobj.GetValue("file_qury_no", StringComparison.OrdinalIgnoreCase).ToString();
                string filename = jobj.GetValue("filename", StringComparison.OrdinalIgnoreCase).ToString();
                string dld_end_time = jobj.GetValue("dld_end_time", StringComparison.OrdinalIgnoreCase) == null ? "" : jobj.GetValue("dld_end_time", StringComparison.OrdinalIgnoreCase).ToString();
                string data_cnt = jobj.GetValue("data_cnt", StringComparison.OrdinalIgnoreCase).ToString();

                WriteLog(sysdate + "  按病种付费病种目录下载成功|" + errstr.ToString());
                List<string> XZBH = new List<string>();
                XZBH.Add("1301");
                XZBH.Add("1302");
                XZBH.Add("1303");
                XZBH.Add("1305");
                XZBH.Add("1306");
                XZBH.Add("1307");
                XZBH.Add("1308");
                XZBH.Add("1309");
                XZBH.Add("1310");
                XZBH.Add("1311");
                XZBH.Add("1313");
                XZBH.Add("1314");
                XZBH.Add("1315");
                if (XZBH.Contains(xzywbh))
                {
                    YBWJXZ(new object[] { file_qury_no, filename });
                }
                return new object[] { 0, 1, filename };
            }
            else
            {
                return new object[] { 0, 0, errstr };
            }

        }
        #endregion

        #region 日间手术治疗病种目录下载
        public static object[] RJSSZLBZMLXZ(object[] objParam)
        {
            string sysdate = GetServerDateTime();
            string xzywbh = objParam[0].ToString();     //业务编号
            string crver = objParam[1].ToString();      //版本号
            dynamic data = new
            {
                data = new
                {
                    ver = crver
                }
            };


            string errstr = string.Empty;
            //入参
            WriteLog(sysdate + "  进入日间手术治疗病种目录下载...");
            string inJosn = Newtonsoft.Json.JsonConvert.SerializeObject(data);
            WriteLog(sysdate + "  入参" + inJosn.ToString());
            int i = YBServiceRequest("1311", data, ref errstr);
            WriteLog(sysdate + "  出参" + errstr);
            if (i > 0)
            {
                JObject jobj = JObject.Parse(errstr);
                string file_qury_no = jobj.GetValue("file_qury_no", StringComparison.OrdinalIgnoreCase).ToString();
                string filename = jobj.GetValue("filename", StringComparison.OrdinalIgnoreCase).ToString();
                string dld_end_time = jobj.GetValue("dld_end_time", StringComparison.OrdinalIgnoreCase) == null ? "" : jobj.GetValue("dld_end_time", StringComparison.OrdinalIgnoreCase).ToString();
                string data_cnt = jobj.GetValue("data_cnt", StringComparison.OrdinalIgnoreCase).ToString();

                WriteLog(sysdate + "  日间手术治疗病种目录下载成功|" + errstr.ToString());
                List<string> XZBH = new List<string>();
                XZBH.Add("1301");
                XZBH.Add("1302");
                XZBH.Add("1303");
                XZBH.Add("1305");
                XZBH.Add("1306");
                XZBH.Add("1307");
                XZBH.Add("1308");
                XZBH.Add("1309");
                XZBH.Add("1310");
                XZBH.Add("1311");
                XZBH.Add("1313");
                XZBH.Add("1314");
                XZBH.Add("1315");
                if (XZBH.Contains(xzywbh))
                {
                    YBWJXZ(new object[] { file_qury_no, filename });
                }
                return new object[] { 0, 1, filename };
            }
            else
            {
                return new object[] { 0, 0, errstr };
            }

        }
        #endregion

        #region 肿瘤形态学目录下载
        public static object[] ZLXTXMLXZ(object[] objParam)
        {
            string sysdate = GetServerDateTime();
            string xzywbh = objParam[0].ToString();     //业务编号
            string crver = objParam[1].ToString();      //版本号
            dynamic data = new
            {
                data = new
                {
                    ver = crver
                }
            };


            string errstr = string.Empty;
            //入参
            WriteLog(sysdate + "  进入肿瘤形态学目录下载...");
            string inJosn = Newtonsoft.Json.JsonConvert.SerializeObject(data);
            WriteLog(sysdate + "  入参" + inJosn.ToString());
            int i = YBServiceRequest("1313", inJosn, ref errstr);
            WriteLog(sysdate + "  出参" + errstr);
            if (i > 0)
            {
                JObject jobj = JObject.Parse(errstr);
                string file_qury_no = jobj.GetValue("file_qury_no", StringComparison.OrdinalIgnoreCase).ToString();
                string filename = jobj.GetValue("filename", StringComparison.OrdinalIgnoreCase).ToString();
                string dld_end_time = jobj.GetValue("dld_end_time", StringComparison.OrdinalIgnoreCase) == null ? "" : jobj.GetValue("dld_end_time", StringComparison.OrdinalIgnoreCase).ToString();
                string data_cnt = jobj.GetValue("data_cnt", StringComparison.OrdinalIgnoreCase).ToString();

                WriteLog(sysdate + "  肿瘤形态学目录下载成功|" + errstr.ToString());
                List<string> XZBH = new List<string>();
                XZBH.Add("1301");
                XZBH.Add("1302");
                XZBH.Add("1303");
                XZBH.Add("1305");
                XZBH.Add("1306");
                XZBH.Add("1307");
                XZBH.Add("1308");
                XZBH.Add("1309");
                XZBH.Add("1310");
                XZBH.Add("1311");
                XZBH.Add("1313");
                XZBH.Add("1314");
                XZBH.Add("1315");
                if (XZBH.Contains(xzywbh))
                {
                    YBWJXZ(new object[] { file_qury_no, filename });
                }
                return new object[] { 0, 1, filename };
            }
            else
            {
                return new object[] { 0, 0, errstr };
            }

        }
        #endregion

        #region 中医疾病目录下载
        public static object[] ZYJBMLXZ(object[] objParam)
        {
            string sysdate = GetServerDateTime();
            string xzywbh = objParam[0].ToString();     //业务编号
            string crver = objParam[1].ToString();      //版本号
            dynamic data = new
            {
                data = new
                {
                    ver = crver
                }
            };


            string errstr = string.Empty;
            //入参
            WriteLog(sysdate + "  进入中医疾病目录下载...");
            string inJosn = Newtonsoft.Json.JsonConvert.SerializeObject(data);
            WriteLog(sysdate + "  入参" + inJosn.ToString());
            int i = YBServiceRequest("1314", data, ref errstr);
            WriteLog(sysdate + "  出参" + errstr);
            if (i > 0)
            {
                JObject jobj = JObject.Parse(errstr);
                string file_qury_no = jobj.GetValue("file_qury_no", StringComparison.OrdinalIgnoreCase).ToString();
                string filename = jobj.GetValue("filename", StringComparison.OrdinalIgnoreCase).ToString();
                string dld_end_time = jobj.GetValue("dld_end_time", StringComparison.OrdinalIgnoreCase) == null ? "" : jobj.GetValue("dld_end_time", StringComparison.OrdinalIgnoreCase).ToString();
                string data_cnt = jobj.GetValue("data_cnt", StringComparison.OrdinalIgnoreCase).ToString();

                WriteLog(sysdate + "  中医疾病目录下载成功|" + errstr.ToString());
                List<string> XZBH = new List<string>();
                XZBH.Add("1301");
                XZBH.Add("1302");
                XZBH.Add("1303");
                XZBH.Add("1305");
                XZBH.Add("1306");
                XZBH.Add("1307");
                XZBH.Add("1308");
                XZBH.Add("1309");
                XZBH.Add("1310");
                XZBH.Add("1311");
                XZBH.Add("1313");
                XZBH.Add("1314");
                XZBH.Add("1315");
                if (XZBH.Contains(xzywbh))
                {
                    YBWJXZ(new object[] { file_qury_no, filename });
                }
                return new object[] { 0, 1, filename };
            }
            else
            {
                return new object[] { 0, 0, errstr };
            }

        }
        #endregion

        #region 中医证候目录下载
        public static object[] ZYZHMLXZ(object[] objParam)
        {
            string sysdate = GetServerDateTime();
            string xzywbh = objParam[0].ToString();     //业务编号
            string crver = objParam[1].ToString();      //版本号
            dynamic data = new
            {
                data = new
                {
                    ver = crver
                }
            };


            string errstr = string.Empty;
            //入参
            WriteLog(sysdate + "  进入中医证候目录下载...");
            string inJosn = Newtonsoft.Json.JsonConvert.SerializeObject(data);
            WriteLog(sysdate + "  入参" + inJosn.ToString());
            int i = YBServiceRequest("1315", data, ref errstr);
            WriteLog(sysdate + "  出参" + errstr);
            if (i > 0)
            {
                JObject jobj = JObject.Parse(errstr);
                string file_qury_no = jobj.GetValue("file_qury_no", StringComparison.OrdinalIgnoreCase).ToString();
                string filename = jobj.GetValue("filename", StringComparison.OrdinalIgnoreCase).ToString();
                string dld_end_time = jobj.GetValue("dld_end_time", StringComparison.OrdinalIgnoreCase) == null ? "" : jobj.GetValue("dld_end_time", StringComparison.OrdinalIgnoreCase).ToString();
                string data_cnt = jobj.GetValue("data_cnt", StringComparison.OrdinalIgnoreCase).ToString();

                WriteLog(sysdate + "  中医证候目录下载成功|" + errstr.ToString());
                List<string> XZBH = new List<string>();
                XZBH.Add("1301");
                XZBH.Add("1302");
                XZBH.Add("1303");
                XZBH.Add("1305");
                XZBH.Add("1306");
                XZBH.Add("1307");
                XZBH.Add("1308");
                XZBH.Add("1309");
                XZBH.Add("1310");
                XZBH.Add("1311");
                XZBH.Add("1313");
                XZBH.Add("1314");
                XZBH.Add("1315");
                if (XZBH.Contains(xzywbh))
                {
                    YBWJXZ(new object[] { file_qury_no, filename });
                }
                return new object[] { 0, 1, filename };
            }
            else
            {
                return new object[] { 0, 0, errstr };
            }

        }
        #endregion

        #region 中药饮片目录下载
        public static object[] ZYYPMLXZ(object[] objParam)
        {
            string sysdate = GetServerDateTime();
            string xzywbh = objParam[0].ToString();     //业务编号
            string crver = objParam[1].ToString();      //版本号
            dynamic data = new
            {
                data = new
                {
                    ver = crver
                }
            };


            string errstr = string.Empty;
            //入参
            WriteLog(sysdate + "  进入中医证候目录下载...");
            string inJosn = Newtonsoft.Json.JsonConvert.SerializeObject(data);
            WriteLog(sysdate + "  入参" + inJosn.ToString());
            int i = YBServiceRequest("1302", data, ref errstr);
            WriteLog(sysdate + "  出参" + errstr);
            if (i > 0)
            {
                JObject jobj = JObject.Parse(errstr);
                string file_qury_no = jobj.GetValue("file_qury_no", StringComparison.OrdinalIgnoreCase).ToString();
                string filename = jobj.GetValue("filename", StringComparison.OrdinalIgnoreCase).ToString();
                string dld_end_time = jobj.GetValue("dld_end_time", StringComparison.OrdinalIgnoreCase) == null ? "" : jobj.GetValue("dld_end_time", StringComparison.OrdinalIgnoreCase).ToString();
                string data_cnt = jobj.GetValue("data_cnt", StringComparison.OrdinalIgnoreCase).ToString();

                WriteLog(sysdate + "  中医证候目录下载成功|" + errstr.ToString());
                List<string> XZBH = new List<string>();
                XZBH.Add("1301");
                XZBH.Add("1302");
                XZBH.Add("1303");
                XZBH.Add("1305");
                XZBH.Add("1306");
                XZBH.Add("1307");
                XZBH.Add("1308");
                XZBH.Add("1309");
                XZBH.Add("1310");
                XZBH.Add("1311");
                XZBH.Add("1313");
                XZBH.Add("1314");
                XZBH.Add("1315");
                if (XZBH.Contains(xzywbh))
                {
                    YBWJXZ(new object[] { file_qury_no, filename });
                }
                return new object[] { 0, 1, filename };
            }
            else
            {
                return new object[] { 0, 0, errstr };
            }

        }
        #endregion 
        #endregion

        #region 民族药品目录查询
        public static object[] MZYPMLCX(object[] objParam)
        {
            string sysdate = GetServerDateTime();
            string updt_time = objParam[1].ToString();     //业务编号时间
            string page_num = objParam[2].ToString();//当前页数
            string page_size = objParam[3].ToString();//本页数据量
            dynamic input = new
            {
                data = new
                {
                    med_list_codg = "",
                    genname_codg = "",
                    drug_genname = "",
                    drug_prodname = "",
                    reg_name = "",
                    tcmherb_name = "",
                    mlms_name = "",
                    vali_flag = "",
                    rid = "",
                    ver = "",
                    ver_name = "",
                    opt_begn_time = "",
                    opt_end_time = "",
                    updt_time = updt_time,
                    page_num = page_num,
                    page_size = page_size
                }

            };
            string errstr = string.Empty;

            //入参
            WriteLog(sysdate + "  进入民族药品目录查询...");
            string inJosn = Newtonsoft.Json.JsonConvert.SerializeObject(input);
            WriteLog(sysdate + "  入参" + inJosn.ToString());
            int i = YBServiceRequest("1304", input, ref errstr);
            WriteLog(sysdate + "  出参" + errstr);
            if (i > 0)
            {
                JObject jobj = JObject.Parse(errstr);
                List<string> delList = new List<string>();
                List<string> insertList = new List<string>();
                List<JObject> jlist = JsonConvert.DeserializeObject<List<JObject>>(jobj.GetValue("data").ToString());
                foreach (JObject jdata1 in jlist)
                {
                    string med_list_codg = jdata1["med_list_codg"].ToString();
                    string drug_prodname = jdata1["drug_prodname"].ToString();
                    string genname_codg = jdata1["genname_codg"].ToString();
                    string drug_genname = jdata1["drug_genname"].ToString();
                    string ethdrug_type = jdata1["ethdrug_type"].ToString();
                    string chemname = jdata1["chemname"].ToString();
                    string alis = jdata1["alis"].ToString();
                    string eng_name = jdata1["eng_name"].ToString();
                    string dosform = jdata1["dosform"].ToString();
                    string each_dos = jdata1["each_dos"].ToString();
                    string used_frqu = jdata1["used_frqu"].ToString();
                    string nat_drug_no = jdata1["nat_drug_no"].ToString();
                    string used_mtd = jdata1["used_mtd"].ToString();
                    string ing = jdata1["ing"].ToString();
                    string chrt = jdata1["chrt"].ToString();
                    string defs = jdata1["defs"].ToString();
                    string tabo = jdata1["tabo"].ToString();
                    string mnan = jdata1["mnan"].ToString();
                    string stog = jdata1["stog"].ToString();
                    string drug_spec = jdata1["drug_spec"].ToString();
                    string prcunt_type = jdata1["prcunt_type"].ToString();
                    string otc_flag = jdata1["otc_flag"].ToString();
                    string pacmatl = jdata1["pacmatl"].ToString();
                    string pacspec = jdata1["pacspec"].ToString();
                    string min_useunt = jdata1["min_useunt"].ToString();
                    string min_salunt = jdata1["min_salunt"].ToString();
                    string manl = jdata1["manl"].ToString();
                    string rute = jdata1["rute"].ToString();
                    string begndate = jdata1["begndate"].ToString();
                    string enddate = jdata1["enddate"].ToString();
                    string pham_type = jdata1["pham_type"].ToString();
                    string memo = jdata1["memo"].ToString();
                    string pac_cnt = jdata1["pac_cnt"].ToString();
                    string min_unt = jdata1["min_unt"].ToString();
                    string min_pac_cnt = jdata1["min_pac_cnt"].ToString();
                    string min_pacunt = jdata1["min_pacunt"].ToString();
                    string min_prepunt = jdata1["min_prepunt"].ToString();
                    string drug_expy = jdata1["drug_expy"].ToString();
                    string efcc_atd = jdata1["efcc_atd"].ToString();
                    string min_prcunt = jdata1["min_prcunt"].ToString();
                    string wubi = jdata1["wubi"].ToString();
                    string pinyin = jdata1["pinyin"].ToString();
                    string vali_flag = jdata1["vali_flag"].ToString();
                    string rid = jdata1["rid"].ToString();
                    string crte_time = jdata1["crte_time"].ToString();
                    string updt_time1 = jdata1["updt_time"].ToString();
                    string crter_id = jdata1["crter_id"].ToString();
                    string crter_name = jdata1["crter_name"].ToString();
                    string crte_optins_no = jdata1["crte_optins_no"].ToString();
                    string opter_id = jdata1["opter_id"].ToString();
                    string opter_name = jdata1["opter_name"].ToString();
                    string opt_time = jdata1["opt_time"].ToString();
                    string optins_no = jdata1["optins_no"].ToString();
                    string ver = jdata1["ver"].ToString();


                    string delsql = string.Format(@"delete from ybmrdr where dm='{0}'", med_list_codg);
                    delList.Add(delsql);

                    string insertsql = string.Format(@"insert into ybmrdr (dm,spm,dmmc,hxmc,bm,ywmc,jx,mcyl,sypc,yf,cz,gg,jjdwlx,fcfybz,bzcz,bzgg,zxsydw,zxxsdw,sms,gytj,ksrq,jsrq,bz,bzsl,zxjldw,zxbzsl,zxbzdw,zxzjdw,ypyxq,gnzz,zxjjdw,wbm,pym,yxbz,wyjlh,sjcjsj,sjgxsj,jbr,jbrq,bbmc,xzlx,sfxmzldm,sfxmzl)values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}','{20}','{21}','{22}','{23}','{24}','{25}','{26}','{27}','{28}','{29}','{30}','{31}','{32}','{33}','{34}','{35}','{36}','{37}','{38}','{39}','{40}','{41}','{42}')", med_list_codg, drug_prodname, drug_genname, chemname, alis, eng_name, dosform, each_dos, used_frqu, used_mtd, ing, drug_spec, prcunt_type, otc_flag, pacmatl, pacspec, min_useunt, min_salunt, manl, rute, begndate, enddate, memo, pac_cnt, min_unt, min_pac_cnt, min_pacunt, min_prepunt, drug_expy, efcc_atd, min_prcunt, wubi, pinyin, vali_flag, rid, crte_time, updt_time1, opter_name, opt_time, ver, "1304", ((int)list_type.民族药).ToString(), list_type.民族药.ToString());
                    insertList.Add(insertsql);
                }
                object[] execSqls = CliUtils.CallMethod("sybdj", "BatExecuteSql", delList.ToArray());
                if (execSqls[1].ToString() != "1")
                {
                    WriteLog(sysdate + "  民族药品目录查询下载删除失败数据库操作失败|" + execSqls[2].ToString());
                    return new object[] { 0, 0, execSqls[2].ToString() };
                }
                execSqls = CliUtils.CallMethod("sybdj", "BatExecuteSql", insertList.ToArray());
                if (execSqls[1].ToString() != "1")
                {
                    WriteLog(sysdate + "  民族药品目录查询下载新增失败数据库操作失败|" + execSqls[2].ToString());
                    return new object[] { 0, 0, execSqls[2].ToString() };
                }
                WriteLog(sysdate + "  民族药品目录查询下载成功|" + errstr.ToString());
                return new object[] { 0, 1, "  民族药品目录查询下载成功！成功条数：" + jlist.Count, jlist };
            }
            else
            {
                WriteLog(sysdate + "  民族药品目录查询下载失败|" + errstr.ToString());
                return new object[] { 0, 0, errstr.ToString() };
            }
        }
        #endregion

        #region 医疗服务项目目录查询
        public static object[] YLFWXMMLCX(object[] objParam)
        {
            string sysdate = GetServerDateTime();
            string xzywbh = objParam[0].ToString();     //业务编号
            string crver = objParam[1].ToString();      //版本号
            dynamic data = new
            {
                data = new
                {
                    ver = crver
                }
            };
            string errstr = string.Empty;

            //入参
            WriteLog(sysdate + "  进入目录下载...");
            string inJosn = Newtonsoft.Json.JsonConvert.SerializeObject(data);
            WriteLog(sysdate + "  入参" + inJosn.ToString());
            int i = YBServiceRequest("1305", data, ref errstr);
            WriteLog(sysdate + "  出参" + errstr);
            if (i > 0)
            {
                JObject jdata1 = JObject.Parse(errstr.ToString());
                string file_qury_no = jdata1.GetValue("file_qury_no", StringComparison.OrdinalIgnoreCase).ToString();
                string filename = jdata1.GetValue("filename", StringComparison.OrdinalIgnoreCase).ToString();
                string dld_end_time = jdata1.GetValue("dld_end_time", StringComparison.OrdinalIgnoreCase) == null ? "" : jdata1.GetValue("dld_end_time", StringComparison.OrdinalIgnoreCase).ToString();
                string data_cnt = jdata1.GetValue("data_cnt", StringComparison.OrdinalIgnoreCase).ToString();
                WriteLog(sysdate + "  目录下载成功|" + errstr.ToString());
                return new object[] { 0, 1, filename };
            }
            else
            {
                WriteLog(sysdate + "  目录下载失败|" + errstr.ToString());
                return new object[] { 0, 0, errstr.ToString() };
            }
        }
        #endregion

        #region 医保目录信息查询
        public static object[] YBMLXXXZ(object[] objParam)
        {
            string sysdate = GetServerDateTime();
            string xzywbh = objParam[0].ToString();     //业务编号
            string crver = objParam[1].ToString();      //更新时间
            string dqye = objParam[2].ToString();       //当前页数
            string bysjl = objParam[3].ToString();       //本页数据量
            dynamic data = new
            {
                data = new
                {
                    query_date = "",
                    hilist_code = "",
                    insu_admdvs = "",
                    begndate = "",
                    hilist_name = "",
                    wubi = "",
                    pinyin = "",
                    med_chrgitm_type = "",
                    chrgitm_lv = "",
                    lmt_used_flag = "",
                    list_type = "",
                    med_use_flag = "",
                    matn_used_flag = "",
                    hilist_use_type = "",
                    lmt_cpnd_type = "",
                    vali_flag = "",
                    updt_time = crver,
                    page_num = dqye,
                    page_size = bysjl
                }
            };


            string errstr = string.Empty;
            //入参
            WriteLog(sysdate + "  进入目录下载...");
            string inJosn = Newtonsoft.Json.JsonConvert.SerializeObject(data);
            WriteLog(sysdate + "  入参" + inJosn.ToString());
            int i = YBServiceRequest("1312", data, ref errstr);
            WriteLog(sysdate + "  出参" + errstr.ToString());
            if (i > 0)
            {
                JObject jobj = JObject.Parse(errstr);

                List<JObject> jlist = JsonConvert.DeserializeObject<List<JObject>>(jobj.GetValue("data").ToString());
                List<string> delList = new List<string>();
                List<string> insertList = new List<string>();
                foreach (JObject jdata1 in jlist)
                {
                    string hilist_code = jdata1["hilist_code"].ToString();//医保目录项目编码
                    string hilist_name = jdata1["hilist_name"].ToString();//医保目录名称
                    string insu_admdvs = jdata1["insu_admdvs"].ToString();//参保机构医保区划
                    string begndate = jdata1["begndate"].ToString();//开始日期
                    string enddate = jdata1["enddate"].ToString();//enddate
                    string med_chrgitm_type = jdata1["med_chrgitm_type"].ToString();//医保目录项目编码med_chrgitm_type
                    string chrgitm_lv = jdata1["chrgitm_lv"].ToString();//医保目录名称
                    string lmt_used_flag = jdata1["lmt_used_flag"].ToString();//参保机构医保区划
                    string list_type = jdata1["list_type"].ToString();//开始日期
                    string med_use_flag = jdata1["med_use_flag"].ToString();//enddate
                    string matn_used_flag = jdata1["matn_used_flag"].ToString();//医保目录项目编码
                    string hilist_use_type = jdata1["hilist_use_type"].ToString();//医保目录名称
                    string lmt_cpnd_type = jdata1["lmt_cpnd_type"].ToString();//参保机构医保区划
                    string wubi = jdata1["wubi"].ToString();//开始日期
                    string pinyin = jdata1["pinyin"].ToString();//enddate
                    string memo = jdata1["memo"].ToString();//memo
                    string vali_flag = jdata1["vali_flag"].ToString();//医保目录项目编码
                    string rid = jdata1["rid"].ToString();//医保目录名称
                    string updt_time = jdata1["updt_time"].ToString();//参保机构医保区划
                    string crter_id = jdata1["crter_id"].ToString();//开始日期
                    string crter_name = jdata1["crter_name"].ToString();//enddate
                    string crte_time = jdata1["crte_time"].ToString();//医保目录项目编码
                    string crte_optins_no = jdata1["crte_optins_no"].ToString();//医保目录名称
                    string opter_id = jdata1["opter_id"].ToString();//参保机构医保区划
                    string opter_name = jdata1["opter_name"].ToString();//开始日期
                    string opt_time = jdata1["opt_time"].ToString();//enddate
                    string optins_no = jdata1["optins_no"].ToString();//经办机构
                    string poolarea_no = jdata1["poolarea_no"].ToString();//统筹区 

                    string delsql = string.Format(@"delete from ybmrdr where dm='{0}'", hilist_code);
                    insertList.Add(delsql);

                    string insertsql = string.Format(@"insert into ybmrdr(dm,dmmc,ksrq,jsrq,sflbdm,sfxmdj,xzsybz,sfxmzldm,sysybz,pym,wbm,bz,yxbz,jbrq,jbr,wyjlh,bbmc,xzlx)
                        values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}')", hilist_code, hilist_name, begndate, enddate, med_chrgitm_type, chrgitm_lv, lmt_used_flag, list_type, matn_used_flag, pinyin, wubi, memo, vali_flag, updt_time, opter_name, rid, updt_time, "1312");
                    insertList.Add(insertsql);
                }

                // execSql= CliUtils.CallMethod("sybdj", "BatExecuteSql", delList.ToArray());
                //if (execSql[1].ToString() != "1")
                //{
                //    WriteLog(sysdate + "  目录下载删除失败数据库操作失败|" + execSql[2].ToString());
                //    return new object[] { 0, 0, "  目录下载新增失败数据库操作失败|"+execSql[2].ToString() };
                //}
                object[] execSql = CliUtils.CallMethod("sybdj", "BatExecuteSql", insertList.ToArray());
                if (execSql[1].ToString() != "1")
                {
                    WriteLog(sysdate + "  目录下载新增失败数据库操作失败|" + execSql[2].ToString());
                    return new object[] { 0, 0, "  目录下载新增失败数据库操作失败|" + execSql[2].ToString() };
                }
                WriteLog(sysdate + "  目录下载成功, 成功条数:" + jlist.Count + "|" + errstr.ToString());
                return new object[] { 0, 1, "  目录下载成功, 成功条数:" + jlist.Count + "|" + errstr.ToString(), jlist };
            }
            else
            {
                WriteLog(sysdate + "  目录下载失败|" + errstr.ToString());
                return new object[] { 0, 0, errstr.ToString() };
            }
        }
        #endregion

        #region 医疗目录与医保目录匹配信息查询
        public static object[] YLMLYYBMLPPXXXZ(object[] objParam)
        {
            string sysdate = GetServerDateTime();
            string xzywbh = objParam[0].ToString();     //业务编号
            string crver = objParam[1].ToString();      //更新时间
            string dqye = objParam[2].ToString();       //当前页数
            string bysjl = objParam[3].ToString();       //本页数据量
            dynamic data = new
            {
                data = new
                {
                    query_date = "",
                    med_list_codg = "",
                    hilist_code = "",
                    list_type = "",
                    insu_admdvs = "",
                    begndate = "",
                    vali_flag = "",
                    updt_time = crver,
                    page_num = dqye,
                    page_size = bysjl
                }
            };
            string errstr = string.Empty;
            string ywCode = "1316";
            //入参
            WriteLog(sysdate + "  进入医疗目录与医保目录匹配信息查询...");
            string inJosn = Newtonsoft.Json.JsonConvert.SerializeObject(data);
            WriteLog(sysdate + "  入参" + inJosn.ToString());
            int i = YBServiceRequest("1316", data, ref errstr);
            WriteLog(sysdate + "  出参" + errstr);
            if (i > 0)
            {

                JObject jobj = JObject.Parse(errstr);
                string dataContent = jobj.GetValue("data").ToString();
                List<JObject> jlist = JsonConvert.DeserializeObject<List<JObject>>(dataContent);
                List<string> insertList = new List<string>();
                foreach (JObject item in jlist)
                {
                    string med_list_codg = item["med_list_codg"].ToString();
                    string hilist_code = item["hilist_code"].ToString();
                    string list_type = item["list_type"].ToString();
                    string insu_admdvs = item["insu_admdvs"].ToString();
                    string begndate = item["begndate"].ToString();
                    string enddate = item["enddate"].ToString();
                    string memo = item["memo"].ToString();
                    string vali_flag = item["vali_flag"].ToString();
                    string rid = item["rid"].ToString();
                    string updt_time = item["updt_time"].ToString();
                    string crter_id = item["crter_id"].ToString();
                    string crter_name = item["crter_name"].ToString();
                    string crte_time = item["crte_time"].ToString();
                    string crte_optins_no = item["crte_optins_no"].ToString();
                    string opter_id = item["opter_id"].ToString();
                    string opter_name = item["opter_name"].ToString();
                    string opt_time = item["opt_time"].ToString();
                    string optins_no = item["optins_no"].ToString();
                    string poolarea_no = item["poolarea_no"].ToString();

                    string delsql = string.Format("delete from ybyljgmlpp where rid ='{0}' and xzlx='{1}'", rid, ywCode);
                    insertList.Add(delsql);
                    string insertSql = string.Format(@"insert into ybyljgmlpp ( 
rid									  ,
insu_admdvs							  ,
list_type							  ,
med_list_codg						  ,
begndate							  ,
enddate								  ,
memo								  ,
vali_flag							  ,
updt_time							  ,
crter_id							  ,
crter_name							  ,
crte_time							  ,
crte_optins_no						  ,
opter_id							  ,
opter_name							  ,
opt_time							  ,
optins_no							  ,
poolarea_no                           ,
xzlx)
values
(
'{0}',
'{1}',
'{2}',
'{3}',
'{4}',
'{5}',
'{6}',
'{7}',
'{8}',
'{9}',
'{10}',
'{11}',
'{12}',
'{13}',
'{14}',
'{15}',
'{16}',
'{17}',
'{18}'
)", rid,
insu_admdvs,
list_type,
med_list_codg,
begndate,
enddate,
memo,
vali_flag,
updt_time,
crter_id,
crter_name,
crte_time,
crte_optins_no,
opter_id,
opter_name,
opt_time,
optins_no,
poolarea_no,
ywCode);
                    insertList.Add(insertSql);

                }

                string baseUrl = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "字典文件");
                string fileName = "医疗目录与医保目录匹配" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".txt";
                writtxt(dataContent, baseUrl, fileName);
                //数据新增
                object[] obj = null;
                if (insertList.Count > 0)
                {
                    obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", insertList.ToArray());

                    if (obj[1].ToString() == "1")
                    {

                        WriteLog(sysdate + "  医疗目录与医保目录匹配信息查询成功|" + errstr.ToString());
                        return new object[] { 0, 1, "医疗目录与医保目录匹配信息下载成功！成功条数：" + jlist.Count, jlist };
                    }
                    else
                    {
                        WriteLog(sysdate + "  医疗目录与医保目录匹配信息查询失败|数据库操作失败：" + obj[2].ToString());
                        return new object[] { 0, 0, "  医疗目录与医保目录匹配信息查询失败|数据库操作失败：" + obj[2].ToString() };

                    }
                }
                else
                {
                    WriteLog(sysdate + "  医疗目录与医保目录匹配信息查询成功|成功条数：" + jlist.Count);
                    return new object[] { 0, 0, "  医疗目录与医保目录匹配信息查询成功|成功条数：" + jlist.Count };
                }

            }
            else
            {
                WriteLog(sysdate + "  医疗目录与医保目录匹配信息查询失败|" + errstr.ToString());
                return new object[] { 0, 0, errstr.ToString() };
            }
        }
        /// <summary>
        /// 写入文本
        /// </summary>
        /// <param name="Content"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        public static void writtxt(string Content, string path, string file)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string pathFile = Path.Combine(path, file);
            FileStream stream = new FileStream(pathFile, FileMode.Append, FileAccess.Write);
            StreamWriter writer = new StreamWriter(stream);
            writer.WriteLine(Content + "\r\n");
            writer.Close();
            stream.Close();

        }
        #endregion

        #region 医药机构目录匹配信息查询
        public static object[] YYJGMLPPXXCX(object[] objParam)
        {
            string sysdate = GetServerDateTime();
            string xzywbh = objParam[0].ToString();     //业务编号
            string crver = objParam[1].ToString();      //更新时间
            string dqye = objParam[2].ToString();       //当前页数
            string bysjl = objParam[3].ToString();       //本页数据量
            dynamic data = new
            {
                data = new
                {
                    query_date = "",
                    fixmedins_code = "",
                    medins_list_codg = "",
                    medins_list_name = "",
                    insu_admdvs = "",
                    list_type = "",
                    med_list_codg = "",
                    begndate = "",
                    vali_flag = "",
                    updt_time = crver,
                    page_num = dqye,
                    page_size = bysjl
                }
            };
            string errstr = string.Empty;
            string ywCode = "1317";
            //入参
            WriteLog(sysdate + "  进入医药机构目录匹配信息查询...");
            string inJosn = Newtonsoft.Json.JsonConvert.SerializeObject(data);
            WriteLog(sysdate + "  入参" + inJosn.ToString());
            int i = YBServiceRequest("1317", data, ref errstr);
            WriteLog(sysdate + "  出参" + errstr.ToString());
            if (i > 0)
            {
                JObject jobj = JObject.Parse(errstr);

                List<JObject> jlist = JsonConvert.DeserializeObject<List<JObject>>(jobj.GetValue("data").ToString());
                string dataContent = jobj.GetValue("data").ToString();
                List<string> insertList = new List<string>();
                List<string> liSql = new List<string>();

                foreach (JObject item in jlist)
                {
                    string fixmedins_code = item["fixmedins_code"].ToString();
                    string medins_list_codg = item["medins_list_codg"].ToString();
                    string medins_list_name = item["medins_list_name"].ToString();
                    string insu_admdvs = item["insu_admdvs"].ToString();
                    string list_type = item["list_type"].ToString();
                    string med_list_codg = item["med_list_codg"].ToString();
                    string begndate = item["begndate"].ToString();
                    string enddate = item["enddate"].ToString();
                    string aprvno = item["aprvno"].ToString();
                    string dosform = item["dosform"].ToString();
                    string exct_cont = item["exct_cont"].ToString();
                    string item_cont = item["item_cont"].ToString();
                    string prcunt = item["prcunt"].ToString();
                    string spec = item["spec"].ToString();
                    string pacspec = item["pacspec"].ToString();
                    string memo = item["memo"].ToString();
                    string vali_flag = item["vali_flag"].ToString();
                    string rid = item["rid"].ToString();
                    string updt_time = item["updt_time"].ToString();
                    string crter_id = item["crter_id"].ToString();
                    string crter_name = item["crter_name"].ToString();
                    string crte_time = item["crte_time"].ToString();
                    string crte_optins_no = item["crte_optins_no"].ToString();
                    string opter_id = item["opter_id"].ToString();
                    string opter_name = item["opter_name"].ToString();
                    string opt_time = item["opt_time"].ToString();
                    string optins_no = item["optins_no"].ToString();
                    string poolarea_no = item["poolarea_no"].ToString();
                    string delsql = string.Format("delete from ybyljgmlpp where rid ='{0}' and xzlx='{1}'", rid, ywCode);
                    liSql.Add(delsql);
                    string insertSql = string.Format(@"insert into ybyljgmlpp ( 
rid									  ,
fixmedins_code,
medins_list_codg					  ,
medins_list_name					  ,
insu_admdvs							  ,
list_type							  ,
med_list_codg						  ,
begndate							  ,
enddate								  ,
aprvno								  ,
dosform								  ,
exct_cont							  ,
item_cont							  ,
prcunt								  ,
spec								  ,
pacspec								  ,
memo								  ,
vali_flag							  ,
updt_time							  ,
crter_id							  ,
crter_name							  ,
crte_time							  ,
crte_optins_no						  ,
opter_id							  ,
opter_name							  ,
opt_time							  ,
optins_no							  ,
poolarea_no                           ,
xzlx)
values
(
'{0}',
'{1}',
'{2}',
'{3}',
'{4}',
'{5}',
'{6}',
'{7}',
'{8}',
'{9}',
'{10}',
'{11}',
'{12}',
'{13}',
'{14}',
'{15}',
'{16}',
'{17}',
'{18}',
'{19}',
'{20}',
'{21}',
'{22}',
'{23}',
'{24}',
'{25}',
'{26}',
'{27}',
'{28}'
)", rid,
fixmedins_code,
medins_list_codg,
medins_list_name,
insu_admdvs,
list_type,
med_list_codg,
begndate,
enddate,
aprvno,
dosform,
exct_cont,
item_cont,
prcunt,
spec,
pacspec,
memo,
vali_flag,
updt_time,
crter_id,
crter_name,
crte_time,
crte_optins_no,
opter_id,
opter_name,
opt_time,
optins_no,
poolarea_no,
ywCode);
                    liSql.Add(insertSql);
                }
                string baseUrl = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "字典文件");
                string fileName = "医药机构目录匹配信息查询" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".txt";
                writtxt(dataContent, baseUrl, fileName);
                //数据新增
                object[] obj = null;
                if (liSql.Count > 0)
                {
                    obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", liSql.ToArray());

                    if (obj[1].ToString() == "1")
                    {

                        WriteLog(sysdate + "  医药机构目录匹配信息查询成功|" + errstr.ToString());
                        return new object[] { 0, 1, "医药机构目录匹配信息下载成功！成功条数：" + jlist.Count, jlist };
                    }
                    else
                    {
                        WriteLog(sysdate + "  医药机构目录匹配信息查询失败|数据库操作失败：" + obj[2].ToString());
                        return new object[] { 0, 0, "  医药机构目录匹配信息查询失败|数据库操作失败：" + obj[2].ToString() };

                    }
                }
                else
                {
                    WriteLog(sysdate + "  医药机构目录匹配信息查询成功|成功条数：" + jlist.Count);
                    return new object[] { 0, 0, "  医药机构目录匹配信息查询成功|成功条数：" + jlist.Count };
                }

            }
            else
            {
                WriteLog(sysdate + "  医药机构目录匹配信息查询失败|" + errstr.ToString());
                return new object[] { 0, 0, errstr.ToString() };
            }


        }
        #endregion

        #region 医保目录限价信息查询
        public static object[] YBMLXJXXXZ(object[] objParam)
        {
            string sysdate = GetServerDateTime();
            string xzywbh = objParam[0].ToString();     //业务编号
            string crver = objParam[1].ToString();      //更新时间
            string dqye = objParam[2].ToString();       //当前页数
            string bysjl = objParam[3].ToString();       //本页数据量
            dynamic data = new
            {
                data = new
                {
                    query_date = "",
                    hilist_code = "",
                    hilist_lmtpric_type = "",
                    overlmt_dspo_way = "",
                    insu_admdvs = "",
                    begndate = "",
                    enddate = "",
                    vali_flag = "",
                    rid = "",
                    tabname = "",
                    poolarea_no = "",
                    updt_time = crver,
                    page_num = dqye,
                    page_size = bysjl
                }
            };
            string ywCode = "1318";

            string errstr = string.Empty;
            //入参
            WriteLog(sysdate + "  进入医保目录限价信息查询...");
            string inJosn = Newtonsoft.Json.JsonConvert.SerializeObject(data);
            WriteLog(sysdate + "  入参" + inJosn.ToString());
            int i = YBServiceRequest("1318", data, ref errstr);
            WriteLog(sysdate + "  出参" + inJosn.ToString());
            if (i > 0)
            {
                JObject jobj = JObject.Parse(errstr);

                List<JObject> jlist = JsonConvert.DeserializeObject<List<JObject>>(jobj.GetValue("data").ToString());
                List<string> insertList = new List<string>();
                string dataContent = jobj.GetValue("data").ToString();
                foreach (JObject item in jlist)
                {
                    string hilist_code = item["hilist_code"].ToString();
                    string hilist_lmtpric_type = item["hilist_lmtpric_type"].ToString();
                    string overlmt_dspo_way = item["overlmt_dspo_way"].ToString();
                    string insu_admdvs = item["insu_admdvs"].ToString();
                    string begndate = item["begndate"].ToString();
                    string enddate = item["enddate"].ToString();
                    string hilist_pric_uplmt_amt = item["hilist_pric_uplmt_amt"].ToString();
                    string vali_flag = item["vali_flag"].ToString();
                    string rid = item["rid"].ToString();
                    string updt_time = item["updt_time"].ToString();
                    string crter_id = item["crter_id"].ToString();
                    string crter_name = item["crter_name"].ToString();
                    string crte_time = item["crte_time"].ToString();
                    string crte_optins_no = item["crte_optins_no"].ToString();
                    string opter_id = item["opter_id"].ToString();
                    string opter_name = item["opter_name"].ToString();
                    string opt_time = item["opt_time"].ToString();
                    string optins_no = item["optins_no"].ToString();
                    string tabname = item["tabname"].ToString();
                    string poolarea_no = item["poolarea_no"].ToString();
                    string delsql = string.Format("delete from ybyljgmlpp where rid ='{0}' and xzlx='{1}'", rid, ywCode);
                    insertList.Add(delsql);
                    string insertSql = string.Format(@"insert into ybyljgmlpp ( 
rid									  , 
hilist_code                           ,
hilist_lmtpric_type                   ,
overlmt_dspo_way                      ,
insu_admdvs							  , 
begndate							  ,
enddate								  , 
hilist_pric_uplmt_amt                 ,
vali_flag							  ,
updt_time							  ,
crter_id							  ,
crter_name							  ,
crte_time							  ,
crte_optins_no						  ,
opter_id							  ,
opter_name							  ,
opt_time							  ,
optins_no							  ,
tabname                               ,
poolarea_no                           ,
xzlx)
values
(
'{0}',
'{1}',
'{2}',
'{3}',
'{4}',
'{5}',
'{6}',
'{7}',
'{8}',
'{9}',
'{10}',
'{11}',
'{12}',
'{13}',
'{14}',
'{15}',
'{16}',
'{17}',
'{18}',
'{19}',
'{20}'
)", rid,
hilist_code,
hilist_lmtpric_type,
overlmt_dspo_way,
insu_admdvs,
begndate,
enddate,
hilist_pric_uplmt_amt,
vali_flag,
updt_time,
crter_id,
crter_name,
crte_time,
crte_optins_no,
opter_id,
opter_name,
opt_time,
optins_no,
tabname,
poolarea_no,
ywCode);
                    insertList.Add(insertSql);
                }
                string baseUrl = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "字典文件");
                string fileName = "医保目录限价信息查询" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".txt";
                writtxt(dataContent, baseUrl, fileName);
                //数据新增
                object[] obj = null;
                if (insertList.Count > 0)
                {
                    obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", insertList.ToArray());

                    if (obj[1].ToString() == "1")
                    {

                        WriteLog(sysdate + "  医保目录限价信息查询成功|成功条数：" + jlist.Count);
                        return new object[] { 0, 1, "医保目录限价信息下载成功！成功条数：" + jlist.Count, jlist };
                    }
                    else
                    {
                        WriteLog(sysdate + "  医保目录限价信息查询失败|数据库操作失败：" + obj[2].ToString());
                        return new object[] { 0, 0, "  医保目录限价信息查询失败|数据库操作失败：" + obj[2].ToString() };

                    }
                }
                else
                {
                    WriteLog(sysdate + "  医保目录限价信息查询成功|成功条数：" + jlist.Count);
                    return new object[] { 0, 0, "  医保目录限价信息查询成功|成功条数：" + jlist.Count };
                }
            }
            else
            {
                WriteLog(sysdate + "  医保目录限价信息查询失败|" + errstr.ToString());
                return new object[] { 0, 0, errstr.ToString() };
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

        #region 医保目录先自付比例信息查询
        public static object[] YBMLXZFBLXXCX(object[] objParam)
        {
            string sysdate = GetServerDateTime();
            string xzywbh = objParam[0].ToString();     //业务编号
            string crver = objParam[1].ToString();      //更新时间
            string dqye = objParam[2].ToString();       //当前页数
            string bysjl = objParam[3].ToString();       //本页数据量
            dynamic data = new
            {
                data = new
                {
                    query_date = "",
                    hilist_code = "",
                    selfpay_prop_psn_type = "",
                    selfpay_prop_type = "",
                    insu_admdvs = "",
                    begndate = "",
                    enddate = "",
                    vali_flag = "",
                    rid = "",
                    tabname = "",
                    poolarea_no = "",
                    updt_time = crver,
                    page_num = dqye,
                    page_size = bysjl
                }
            };
            string errstr = string.Empty;
            string ywCode = "1319";
            //入参
            WriteLog(sysdate + "  进入医保目录先自付比例信息查询...");
            string inJosn = Newtonsoft.Json.JsonConvert.SerializeObject(data);
            WriteLog(sysdate + "  入参" + inJosn.ToString());
            int i = YBServiceRequest("1319", data, ref errstr);
            WriteLog(sysdate + "  出参" + errstr.ToString());
            if (i > 0)
            {
                JObject jobj = JObject.Parse(errstr);

                List<JObject> jlist = JsonConvert.DeserializeObject<List<JObject>>(jobj.GetValue("data").ToString());
                List<string> insertList = new List<string>();
                string dataContent = jobj.GetValue("data").ToString();
                foreach (JObject item in jlist)
                {
                    string hilist_code = item["hilist_code"].ToString();
                    string selfpay_prop_psn_type = item["selfpay_prop_psn_type"].ToString();
                    string selfpay_prop_type = item["selfpay_prop_type"].ToString();
                    string insu_admdvs = item["insu_admdvs"].ToString();
                    string begndate = item["begndate"].ToString();
                    string enddate = item["enddate"].ToString();
                    string selfpay_prop = item["selfpay_prop"].ToString();
                    string vali_flag = item["vali_flag"].ToString();
                    string rid = item["rid"].ToString();
                    string updt_time = item["updt_time"].ToString();
                    string crter_id = item["crter_id"].ToString();
                    string crter_name = item["crter_name"].ToString();
                    string crte_time = item["crte_time"].ToString();
                    string crte_optins_no = item["crte_optins_no"].ToString();
                    string opter_id = item["opter_id"].ToString();
                    string opter_name = item["opter_name"].ToString();
                    string opt_time = item["opt_time"].ToString();
                    string optins_no = item["optins_no"].ToString();
                    string tabname = item["tabname"].ToString();
                    string poolarea_no = item["poolarea_no"].ToString();
                    string delsql = string.Format("delete from ybyljgmlpp where rid ='{0}' and xzlx='{1}'", rid, ywCode);
                    insertList.Add(delsql);
                    string insertSql = string.Format(@"insert into ybyljgmlpp ( 
rid									  ,
hilist_code                           ,
selfpay_prop_psn_type             ,
selfpay_prop_type                     ,
insu_admdvs          				  ,
begndate             				  ,
enddate              				  ,
selfpay_prop         				  ,
vali_flag            				  , 
updt_time            				  ,
crter_id             				  ,
crter_name           				  ,
crte_time            				  ,
crte_optins_no       				  , 
opter_id             				  ,
opter_name           				  ,
opt_time             				  ,
optins_no            				  ,
tabname              				  ,
poolarea_no          				  ,
xzlx)
values
(
'{0}',
'{1}',
'{2}',
'{3}',
'{4}',
'{5}',
'{6}',
'{7}',
'{8}',
'{9}',
'{10}',
'{11}',
'{12}',
'{13}',
'{14}',
'{15}',
'{16}',
'{17}',
'{18}',
'{19}',
'{20}'
)", rid,
hilist_code,
selfpay_prop_psn_type,
selfpay_prop_type,
insu_admdvs,
begndate,
enddate,
selfpay_prop,
vali_flag,
updt_time,
crter_id,
crter_name,
crte_time,
crte_optins_no,
opter_id,
opter_name,
opt_time,
optins_no,
tabname,
poolarea_no,
ywCode);
                    insertList.Add(insertSql);
                }
                string baseUrl = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "字典文件");
                string fileName = "医保目录先自付比例信息" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".txt";
                writtxt(dataContent, baseUrl, fileName);
                //数据新增
                object[] obj = null;
                if (insertList.Count > 0)
                {
                    obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", insertList.ToArray());

                    if (obj[1].ToString() == "1")
                    {

                        WriteLog(sysdate + "  医保目录先自付比例信息查询成功|！成功条数：" + jlist.Count);
                        return new object[] { 0, 1, "医保目录先自付比例信息下载成功！成功条数：" + jlist.Count, jlist };
                    }
                    else
                    {
                        WriteLog(sysdate + "  医保目录先自付比例信息查询失败|数据库操作失败：" + obj[2].ToString());
                        return new object[] { 0, 0, "  医保目录先自付比例信息查询失败|数据库操作失败：" + obj[2].ToString() };

                    }
                }
                else
                {
                    WriteLog(sysdate + "  医保目录先自付比例信息查询成功|成功条数：" + jlist.Count);
                    return new object[] { 0, 0, "  医保目录先自付比例信息查询成功|成功条数：" + jlist.Count };
                }
            }
            else
            {
                WriteLog(sysdate + "  医保目录先自付比例信息查询失败|" + errstr.ToString());
                return new object[] { 0, 0, errstr.ToString() };
            }
        }
        #endregion

        #region 文件下载
        public static object[] YBWJXZ(object[] objParam)
        {
            string sysdate = GetServerDateTime();
            string wjcxh = objParam[0].ToString();      //文件查询号
            string wjm = objParam[1].ToString();        //文件名
            filenameXZ = wjm;
            dynamic data = new
            {
                fsDownloadIn = new
                {
                    file_qury_no = wjcxh,
                    filename = "plc",
                    fixmedins_code = "plc"
                }
            };

            string errstr = string.Empty;
            //入参
            WriteLog(sysdate + "  进入文件下载...");
            string inJosn = Newtonsoft.Json.JsonConvert.SerializeObject(data);
            WriteLog(sysdate + "  入参" + inJosn.ToString());
            int i = YBServiceRequest("9102", data, ref errstr);
            if (i > 0)
            {
                WriteLog(sysdate + "  文件下载成功！");
                return new object[] { 0, 1, "文件下载成功！" };
            }
            else
            {
                WriteLog(sysdate + "  文件下载失败|" + errstr.ToString());
                return new object[] { 0, 0, errstr.ToString() };
            }
        }
        #endregion

        #region 文件上传
        public static object[] YBWJSC(object[] objParam)
        {
            string sysdate = GetServerDateTime();
            byte[] wj = objParam[0] as byte[];      //文件查询号  
            Dictionary<string, object> pairs = new Dictionary<string, object>();
            pairs.Add("in", wj);
            pairs.Add("filename", "plc");
            pairs.Add("fixmedins_code", "fixmedins_code");

            dynamic data = new
            {
                fsUploadIn = pairs
            };

            string errstr = string.Empty;
            //入参
            WriteLog(sysdate + "  进入文件上传...");
            string inJosn = Newtonsoft.Json.JsonConvert.SerializeObject(data);
            WriteLog(sysdate + "  入参" + inJosn.ToString());
            int i = YBServiceRequest("9101", data, ref errstr);
            if (i > 0)
            {
                JObject jobj = JObject.Parse(errstr);
                string file_qury_no = jobj.GetValue("file_qury_no", StringComparison.OrdinalIgnoreCase).ToString();
                string filename = jobj.GetValue("filename", StringComparison.OrdinalIgnoreCase).ToString();
                string fixmedins_code = jobj.GetValue("file_qury_no", StringComparison.OrdinalIgnoreCase).ToString();
                string dld_endtime = jobj.GetValue("file_qury_no", StringComparison.OrdinalIgnoreCase).ToString();
                WriteLog(sysdate + "  文件上传成功！");
                return new object[] { 0, 1, file_qury_no, filename, fixmedins_code, dld_endtime };
            }
            else
            {
                WriteLog(sysdate + "  文件上传失败|" + errstr.ToString());
                return new object[] { 0, 0, errstr.ToString() };
            }
        }
        #endregion

        #endregion

        #region 接口业务调用方法

        #region 获取Mac地址
        public static string getMac()
        {
            try
            {
                NetworkInterface[] interfaces = NetworkInterface.GetAllNetworkInterfaces();
                foreach (NetworkInterface ni in interfaces)
                {
                    return BitConverter.ToString(ni.GetPhysicalAddress().GetAddressBytes());
                }
            }
            catch (Exception)
            {
            }
            return "00-00-00-00-00-00";
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
            string StrSql = $"select b.ybxmbh from ybhisdzdrnew b where b.hisxmbh='{hisxmbm}'";
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
            MessageBox.Show(JsonConvert.SerializeObject(ipEntry));
            string ip = ipEntry.AddressList[0].ToString();
            return ip;
        }
        #endregion

        #region 初始化（新医保接口）
        public static string YbSignNo = string.Empty;
        public static object[] YBINIT(object[] objParam)
        {


            if (isPing == "yes")
            {//Ping医保网
                Ping ping = new Ping();
                PingReply pingReply = ping.Send(YBIP);
                if (pingReply.Status != IPStatus.Success)
                    return new object[] { 0, 0, "未连接医保网" };
            }

            string sysdate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string hisUserID = CliUtils.fLoginUser;
            string UserID = CliUtils.fLoginUser; //用户名
            string PassWD = string.Empty; //密码

            #region 新医保接口的调用方式 by   hjw

            //string data = "{\"opter_no\":\"" + UserID + "\",\"mac\":\"" + getMac() + "\",\"ip\":\"" + CliUtils.fComputerIp + "\"}";

            dynamic _In_Data = new
            {
                signIn = new
                {
                    opter_no = UserID,
                    mac = getMac(),
                    ip = CliUtils.fComputerIp
                }
            };
            string data = JsonConvert.SerializeObject(_In_Data);

            string Result = string.Empty;
            WriteLog(sysdate + "|医保初始化入参|" + data);
            int i = YBServiceRequest("9001", _In_Data, ref Result);
            //i = 1;
            #endregion
            WriteLog(sysdate + "  入参|用户" + UserID + "");
            if (i > 0)
            {
                JObject jobj = JsonConvert.DeserializeObject<JObject>(Result);
                JObject res = JsonConvert.DeserializeObject<JObject>(jobj.GetValue("signinoutb", StringComparison.OrdinalIgnoreCase).ToString());
                string signCode = res.GetValue("sign_no", StringComparison.OrdinalIgnoreCase).ToString();
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

            string operNo = objParam[0].ToString();
            #region 新医保接口的调用方式 by   hjw
            dynamic dy = new { signOut = new { sign_no = YbSignNo, opter_no = operNo } };
            string data = JsonConvert.SerializeObject(dy);
            string Result = string.Empty;
            WriteLog(sysdate + "|医保退出入参|" + data);
            int i = YBServiceRequest("9002", dy, ref Result);
            WriteLog(sysdate + "|医保退出回参|" + Result);
            #endregion
            if (i > 0)
            {
                WriteLog(sysdate + "  医保退出成功");
                return new object[] { 0, 1, "医保退出成功" };
            }
            else
            {
                WriteLog(sysdate + "  医保退出错误|" + Result.ToString());
                return new object[] { 0, 0, "医保退出错误" + Result };
            }
        }
        #endregion


        #region 1.5读身份证信息(读身份证)
        public static object[] SFZDKRYXX(object[] objParam)
        {
            string sysdate = GetServerDateTime();
            try
            {
                CZYBH = CliUtils.fLoginUser;  //用户工号

                if (string.IsNullOrEmpty(CZYBH))
                    return new object[] { 0, 0, "用户工号不能为空" };


                //入参
                WriteLog(sysdate + "  用户" + CZYBH + " 进入读卡(读身份证)获得个人信息...");
                //string sfzinfo = string.Empty;
                //string signinfo = string.Empty;
                StringBuilder sfzinfo = new StringBuilder(4096);
                StringBuilder signinfo = new StringBuilder(4096);
                long i = ReadSFZ(sfzinfo, 4096, signinfo, 4096);
                string res = sfzinfo.ToString();
                WriteLog(sysdate + "  出参" + sfzinfo.ToString());
                if (i > 0)
                {
                    string[] infoArr = sfzinfo.ToString().Split('^');
                    WriteLog(sysdate + "  用户" + CZYBH + " 读卡(读身份证)获得个人信息|" + sfzinfo.ToString());
                    return new object[] { 0, 1, infoArr };
                }
                else
                {
                    WriteLog(sysdate + "  用户" + CZYBH + " 读卡(读身份证)获得个人信息|" + sfzinfo.ToString());
                    return new object[] { 0, 0, sfzinfo.ToString() };
                }
            }
            catch (Exception ex)
            {
                WriteLog(sysdate + "  用户" + CZYBH + " 读卡(读身份证)获得个人信息|" + ex.Message.ToString());
                return new object[] { 0, 0, "读身份证基本信息失败|" + ex.Message };
            }
        }
        #endregion

        #region 1.2读社保卡基本信息
        public static object[] DSBKJBXX(object[] objParam)
        {
            string sysdate = GetServerDateTime();
            try
            {
                CZYBH = CliUtils.fLoginUser;  //用户工号
                //入参
                WriteLog(sysdate + "  用户" + CZYBH + " 进入读社保卡基本信息...");
                //string sbkinfo = "";
                //string signinfo = "";
                StringBuilder sbkinfo = new StringBuilder(4096);
                StringBuilder signinfo = new StringBuilder(4096);
                int outlen = 4096;
                int signlen = 4096;
                WriteLog(sysdate + "  进入读社保卡基本信息");
                long i = ReadCardBas(sbkinfo, outlen, signinfo, signlen);
                MessageBox.Show(JsonConvert.SerializeObject(sbkinfo));
                // WriteLog(sysdate + "   读社保卡基本信息出参" + sbkinfo.ToString());
                // MessageBox.Show(sbkinfo.ToString());
                if (i > 0)
                {
                    //WriteLog(sysdate + "  用户" + CZYBH + " 读社保卡基本信息成功|" + sbkinfo.ToString());
                    return new object[] { 0, 1, sbkinfo.ToString().Split('|') };
                }
                else
                {
                    //WriteLog(sysdate + "  用户" + CZYBH + " 读社保卡基本信息失败|" + sbkinfo.ToString());
                    return new object[] { 0, 0, sbkinfo.ToString() };
                }
            }
            catch (Exception ex)
            {
                WriteLog(sysdate + "  用户" + CZYBH + " 读社保卡基本信息失败|" + ex.Message.ToString());
                return new object[] { 0, 0, "读社保卡基本信息失败|" + ex.Message };
            }
        }
        #endregion

        #region 门诊读卡（读卡无变化 无需更改）
        public static object[] YBMZDK(object[] objParam)
        {
            string sysdate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            object[] obj = default(object[]);
            try
            {
                string Ywlx = "MZGHSK";//老   string Ywlx="1101"; //新
                StringBuilder inputData = new StringBuilder();
                StringBuilder outData = new StringBuilder(102400);
                StringBuilder retMsg = new StringBuilder(102400);
                WriteLog(sysdate + "  进入门诊读卡....");
                WriteLog(sysdate + "  入参|" + inputData.ToString());

                if (dkinit != 0)
                {
                    return new object[] { 0, 0, "读卡服务初始化失败！" };
                }
                string[] info = default(string[]);
                Frm_dkck adkck = new Frm_dkck();
                adkck.ShowDialog();
                adkck.Close();
                adkck.Dispose();
                if (adkck.dzybFlag == DKType.身份证)
                {
                    if (MessageBox.Show("系统检测到您已手工输入身份证，是否继续读卡操作！\r\n选择【是】将继续读卡，选择【否】将使用您输入身份证进行读卡", "提示", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        #region 读身份证
                        obj = SFZDKRYXX(null);
                        if (obj[1].ToString() != "1")
                        {
                            return obj;
                        }
                        info = obj[2] as string[];
                        mdtrt_cert_type = "02";
                        mdtrt_cert_no = info[0];
                        #endregion
                    }
                    else
                    {

                        mdtrt_cert_type = "02";
                        mdtrt_cert_no = adkck.txtidcard.Text.Trim();
                    }
                    if (string.IsNullOrEmpty(mdtrt_cert_no))
                    {
                        return new object[] { 0, 0, "系统检测没有身份证信息" };
                    }
                }
                else if (adkck.dzybFlag == DKType.社保卡)
                {
                    #region 读社保卡
                    obj = DSBKJBXX(null);
                    if (obj[1].ToString() != "1")
                    {
                        return obj;
                    }
                    info = obj[2] as string[];
                    mdtrt_cert_type = "03";
                    mdtrt_cert_no = info[2];
                    card_sn = info[3];
                    psn_cert_type = "01";
                    certno = info[1];
                    #endregion
                }

                string Errstr = string.Empty;
                //   int i = f_UserBargaingApply(Ywlx, inputData, outData, retMsg);
                var cardinfo = GetYbCardInfo(new object[] { mdtrt_cert_type, mdtrt_cert_no, card_sn, begntime, psn_cert_type, certno, psn_name }, ref Errstr);
                if (cardinfo != null)
                {
                    CardInfo = cardinfo;
                    WriteLog(sysdate + "  门诊读卡成功|" + Errstr);
                    return new object[] { 0, 1, Errstr };

                }
                else
                {
                    WriteLog(sysdate + "  门诊读卡失败|" + Errstr.ToString());
                    return new object[] { 0, 0, "读卡失败|" + Errstr.ToString() };
                }
            }
            catch (Exception ex)
            {
                WriteLog(sysdate + "  门诊读卡异常|" + ex.Message);
                return new object[] { 0, 0, "门诊读卡异常|" + ex.Message };
            }
        }
        #endregion 


        internal static ReadCardInfo CardInfo = new ReadCardInfo();


        #region 门诊获取人员信息（新）（新医保接口）
        internal static string xzlx = "";
        private static ReadCardInfo GetYbCardInfo(object[] Inputparams, ref string ErrStr)
        {
            ReadCardInfo cardInfo = new ReadCardInfo();
            string ywCode = "1101";
            string result = string.Empty;
            //psn_cert_type 1是居民身份证
            dynamic dydata = new
            {
                data = new
                {
                    mdtrt_cert_type = Inputparams[0].ToString(),
                    mdtrt_cert_no = Inputparams[1].ToString(),
                    card_sn = Inputparams[2].ToString(),
                    begntime = Inputparams[3].ToString(),
                    psn_cert_type = Inputparams[4].ToString(),
                    certno = Inputparams[5].ToString(),
                    psn_name = Inputparams[6].ToString()
                }
            };
            string sysdate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");


            WriteLog(sysdate + "  医保读卡，入参|" + JsonConvert.SerializeObject(dydata));
            // data = string.Format(data, Inputparams);
            int i = YBServiceRequest(ywCode, dydata, ref result);

            if (i > 0)
            {
                //JObject j = JsonConvert.DeserializeObject<JObject>(result);
                cardInfo = JsonConvert.DeserializeObject<ReadCardInfo>(result);
                List<string> liSql = new List<string>();
                #region 参保信息
                string sqlStr = string.Format(@" delete from ybrycbxx where sfzh='{0}' ", cardInfo.baseinfo.certno);
                object[] obj_zydk = { sqlStr };
                obj_zydk = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj_zydk);
                if (!obj_zydk[1].ToString().Equals("1"))
                {
                    WriteLog(sysdate + "  医保读卡，数据操作失败|" + obj_zydk[2].ToString());
                    ErrStr = "  医保读卡，数据操作失败|" + obj_zydk[2].ToString();
                    return null;
                }
                foreach (Insuinfo item in cardInfo.insuinfo)
                {
                    string sqlRyxx = string.Format(@" insert into ybrycbxx(balc, insutype, psn_type, psn_insu_stas, psn_insu_date, paus_insu_date, cvlserv_flag, insuplc_admdvs, emp_name, grbh, sfzh, xm)
                                values('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}')",
                                item.balc, item.insutype, item.psn_type, item.psn_insu_stas, item.psn_insu_date, item.paus_insu_date, item.cvlserv_flag, item.insuplc_admdvs, item.emp_name, cardInfo.baseinfo.psn_no, cardInfo.baseinfo.certno, cardInfo.baseinfo.psn_name);
                    DataSet dsCbxx = CliUtils.ExecuteSql("sybdj", "cmd", sqlRyxx, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                }
                #endregion
                #region 身份信息
                string sqlsfxx = string.Format(@" delete from ybrysfxx where sfzh='{0}' ", cardInfo.baseinfo.certno);
                DataSet dssfxx = CliUtils.ExecuteSql("sybdj", "cmd", sqlsfxx, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                foreach (Idetinfo item in cardInfo.idetinfo)
                {
                    sqlsfxx = string.Format(@" insert into ybrysfxx(psn_idet_type, psn_type_lv, memo, begntime, endtime, grbh, sfzh, xm)
                                values('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}')",
                           item.psn_idet_type, item.psn_type_lv, item.memo, item.begntime, item.endtime, cardInfo.baseinfo.psn_no, cardInfo.baseinfo.certno, cardInfo.baseinfo.psn_name);
                    dssfxx = CliUtils.ExecuteSql("sybdj", "cmd", sqlsfxx, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                }
                string jdlkrybz = "0";
                sqlsfxx = string.Format(@" select 1 from ybrysfxx where sfzh='{0}' and psn_idet_type='{1}' ", cardInfo.baseinfo.certno, "2302");
                dssfxx = CliUtils.ExecuteSql("sybdj", "cmd", sqlsfxx, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                if (dssfxx.Tables[0].Rows.Count > 0)
                    //判断是否建档立卡人员
                    jdlkrybz = "1";
                else
                    jdlkrybz = "0";


                Frmcbxxxz cbxx = new Frmcbxxxz(cardInfo.baseinfo.certno);
                cbxx.ShowDialog();
                if (string.IsNullOrEmpty(cbxx.RYCBXX))
                {
                    ErrStr = "未选择人员参保信息！";
                    return null;
                }
                string balc = "0";
                string insutype = "";
                string psn_type = "";
                string psn_insu_stas = "";
                string psn_insu_date = "";
                string paus_insu_date = "";
                string cvlserv_flag = "";
                string emp_name = "";
                string insuplc_admdvs1 = "";
                string psn_idet_type = "";
                string psn_type_lv = "";
                string memo = "";
                string begntime = "";
                string endtime = "";
                string ydrybz = "0";
                string[] rycbxx = cbxx.RYCBXX.Split('|');
                Idetinfo idinfo = cardInfo.idetinfo.Count == 0 ? new Idetinfo()
                {
                    begntime = "",
                    endtime = "",
                    memo = "",
                    psn_idet_type = "",
                    psn_type_lv = ""
                } : cardInfo.idetinfo[0];
                balc = rycbxx[0];
                insutype = rycbxx[1];
                psn_type = rycbxx[2];
                psn_insu_stas = rycbxx[3];
                psn_insu_date = rycbxx[4];
                paus_insu_date = rycbxx[5];
                cvlserv_flag = rycbxx[6];
                insuplc_admdvs1 = rycbxx[7];
                emp_name = rycbxx[8];
                cbxx.Dispose();
                xzlx = insutype;

                object[] objFS = new object[] { cardInfo.baseinfo.psn_no };
                objFS = YBYLDYSPXXCX(objFS);
                if (!insuplc_admdvs1.Substring(0, 4).Equals("3213"))
                    ydrybz = "1";
                if (i > 0)
                {
                    WriteLog(sysdate + "  人员信息获取|" + result.ToString());
                    List<string> liSQL = new List<string>();
                    string strSql = string.Format(@" delete from ybickxx where grbh='{0}'
                            insert into ybickxx(grbh,zjlx,gmsfhm,xm,xb,mz,csrq,nl,zhye,xzlx,yldylb,gwybz,tcqh,dwmc,rysflb,rylbdj,memo,kssj,jssj,jzpzlx,jzpzbh, ylrylb, sjnl, ydrybz,sfjdlkry) values 
                            ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}','{20}','{21}','{22}','{23}','{24}') ",
                        cardInfo.baseinfo.psn_no, cardInfo.baseinfo.psn_cert_type, cardInfo.baseinfo.certno, cardInfo.baseinfo.psn_name, cardInfo.baseinfo.gend,
                        cardInfo.baseinfo.naty, cardInfo.baseinfo.brdy, cardInfo.baseinfo.age, balc, insutype,
                        psn_type, cvlserv_flag, insuplc_admdvs1, emp_name, idinfo.psn_idet_type,
                        idinfo.psn_type_lv, idinfo.memo, idinfo.begntime, idinfo.endtime, mdtrt_cert_type, mdtrt_cert_no, psn_type,
                         cardInfo.baseinfo.age, ydrybz, jdlkrybz);
                    liSQL.Add(strSql);
                    object[] obj = liSQL.ToArray();
                    obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);
                    if (obj[1].ToString().Equals("1"))
                    {
                        string jflx = string.Empty;
                        string[] sV = { "11", "12", "13" };
                        if (psn_type.Length > 1)
                        {
                            if (sV.Contains(psn_type.Substring(0, 2)))
                                jflx = "1302"; //职工医保
                            else
                                jflx = "1303"; //居民医保
                        }
                        #region 有
                        //if (jdlkrybz.Equals("1"))
                        //    jflx = "08"; 
                        #endregion
                        string sqlRyxx = string.Format(@" select bzname from bztbd where bzcodn='psn_type' and bzkeyx='{0}' ", psn_type);
                        DataSet dsCbxx = CliUtils.ExecuteSql("sybdj", "cmd", sqlRyxx, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                        if (dsCbxx.Tables[0].Rows.Count > 0)
                        {
                            psn_type = dsCbxx.Tables[0].Rows[0]["bzname"].ToString();
                        }
                        string strParam = cardInfo.baseinfo.psn_no + "|" + "" + "|" + cardInfo.baseinfo.certno + "|" + cardInfo.baseinfo.psn_name + "|" + cardInfo.baseinfo.gend + "|" +
                                  cardInfo.baseinfo.naty + "|" + cardInfo.baseinfo.brdy + "|" + cardInfo.baseinfo.certno + "|" + psn_type + "|" + "正常" + "|" +
                                  ydrybz + "|" + insuplc_admdvs + "|" + DateTime.Now.ToString("yyyy") + "|" + "0" + "|" + balc + "|" +
                                  "|||||||||" + "0" + "|" +
                                  emp_name + "|" + cardInfo.baseinfo.age + "||" + YBJGBH + "|" + jflx + "|" +
                                  "0" + "|" + "0" + "|" + "0" + "|" + "0" + "|" + "0" + "|" +
                                  "0" + "|" + "0" + "|" + "0" + "|" + "0" + "|" + "0" + "|" +
                                  "0" + "|" + "0" + "|" + "0" + "|" + "0" + "|" + "0" + "|" +
                                  "0" + "|" + "0" + "|" + "0" + "|" + "0" + "|" + "0" + "|" +
                                  "" + "|" + "" + "|" + "" + "|" + "" + "|" + "" + "|" + "" + "|" + jdlkrybz;
                        WriteLog(sysdate + "  人员信息获取出参：" + strParam + "\r\n" + "读卡返回参数：" + result);
                        ErrStr = strParam;
                        return cardInfo;
                    }
                    else
                    {
                        WriteLog(sysdate + "  人员信息获取失败|" + obj[2].ToString());
                        ErrStr = obj[2].ToString();
                        return null;
                    }
                }
                else
                {
                    ErrStr = result;
                    return null;
                }
                #endregion

            }
            else
            {
                ErrStr = result;
                return null;
            }

        }
        #endregion 

        #region 门诊就诊信息上传（新医保接口）
        private static object[] YBMZJZXXSC(object[] objParam)
        {
            string ywCode = "2203";
            string sysdate = GetServerDateTime();
            WriteLog(sysdate + "  进入门诊就诊信息上传/撤销...");
            WriteLog(sysdate + "  HIS入参|" + string.Join(",", objParam));
            try
            {
                string ybjzlsh = objParam[0].ToString(); //就诊ID
                string begntime = objParam[1].ToString();//开始时间
                string vali_flag = objParam[2].ToString(); //撤销标志 1-有效 0-无效

                if (string.IsNullOrEmpty(ybjzlsh))
                    return new object[] { 0, 0, "就诊ID不能为空！" };
                #region 获取医保登记信息
                string strSql = string.Format(@"select jzlsh,grbh,yllb,bzbm,bzmc,sysslb,jhsyrq,ksmc,dgysdm,dgysxm,sysdate,cbdybqh,syfylb,syyzs from ybmzzydjdr where ybjzlsh='{0}' and cxbz=1", ybjzlsh);
                DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                DataRow dr = ds.Tables[0].Rows[0];
                string jzlsh = dr["jzlsh"].ToString();
                string grbh = dr["grbh"].ToString();
                string yllb = dr["yllb"].ToString();
                string dise_codg = dr["bzbm"].ToString();
                string dise_name = dr["bzmc"].ToString();
                string sslb = dr["sysslb"].ToString();
                string jhsyrq = dr["jhsyrq"].ToString();
                string diag_dept = dr["ksmc"].ToString();
                string dise_dor_no = dr["dgysdm"].ToString();
                string dise_dor_name = dr["dgysxm"].ToString();
                string jzsj = dr["sysdate"].ToString();
                string cbdybqh = dr["cbdybqh"].ToString();
                string sylb = dr["syfylb"].ToString();
                string yzs = dr["syyzs"].ToString();
                if (string.IsNullOrEmpty(dise_codg))
                {
                    dise_codg = "R53.x00x005";
                    dise_name = "不适";
                }
                #endregion

                #region 入参赋值
                //就诊信息
                dynamic mdtrtinfo = new
                {
                    mdtrt_id = ybjzlsh,
                    psn_no = grbh,
                    med_type = yllb,
                    begntime = begntime,
                    main_cond_dscr = "",
                    dise_codg = dise_codg,
                    dise_name = dise_name,
                    birctrl_type = sslb,
                    birctrl_matn_date = jhsyrq,
                    matn_type = sylb,
                    geso_val = yzs,
                    expContent = ""
                };

                #region 诊断信息
                List<dynamic> diseList = new List<dynamic>();
                //诊断信息
                strSql = string.Format(@"with tmp as (
                                        select m1ghno, case when m1xybz='Y' then '1' else '2' end zdlb,
                                        case when m1xybz='Y' then '1' else '0' end zdbz, m1xyzd, m1xynm, '' rybq,
                                        isnull(ybksmc,b2ejnm) ksmc, isnull(dgysbm,m1user) ysdm, isnull(ysxm,b1name) ysxm, m1date
                                        from mza1dd 
                                        left join bz01h on m1user=b1empn
                                        left join bz02d on b2ejks=b1ksno
                                        left join ybkszd on b1ksno=ksdm
                                        left join ybdgyszd on ysbm=m1user
                                        where isnull(m1xyzd,'')<>''
                                        union all
                                        select  m1ghno, case when m1zybz='Y' then '3' else '4' end zdlb,
                                        case when m1zybz='Y' then '1' else '0' end zdbz, m1zyzd, m1zynm, '' rybq,
                                        isnull(ybksmc,b2ejnm) ksmc, isnull(dgysbm,m1user) ysdm, isnull(ysxm,b1name) ysxm, m1date
                                        from mza1dd 
                                        left join bz01h on m1user=b1empn
                                        left join bz02d on b2ejks=b1ksno
                                        left join ybkszd on b1ksno=ksdm
                                        left join ybdgyszd on ysbm=m1user
                                        where isnull(m1zyzd,'')<>''
                                        ) select * from tmp where m1ghno='{0}'", jzlsh);
                ds.Tables.Clear();
                ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                if (ds.Tables[0].Rows.Count == 0)
                {
                    dynamic diseinfo = new
                    {
                        diag_type = "1",
                        diag_srt_no = "1",
                        diag_code = dise_codg,
                        diag_name = dise_name,
                        diag_dept = diag_dept,
                        dise_dor_no = dise_dor_no,
                        dise_dor_name = dise_dor_name,
                        diag_time = Convert.ToDateTime(jzsj).ToString("yyyy-MM-dd HH:mm:ss"),
                        vali_flag = vali_flag
                    };
                    diseList.Add(diseinfo);
                }
                else
                {
                    int index = 1;
                    foreach (DataRow drx in ds.Tables[0].Rows)
                    {
                        dynamic diseinfo = new
                        {
                            diag_type = Convert.ToString(drx["zdlb"]),
                            diag_srt_no = index.ToString(),
                            diag_code = Convert.ToString(drx["m1xyzd"]),
                            diag_name = Convert.ToString(drx["m1xynm"]),
                            diag_dept = Convert.ToString(drx["ksmc"]),
                            dise_dor_no = Convert.ToString(drx["ysdm"]),
                            dise_dor_name = Convert.ToString(drx["ysxm"]),
                            diag_time = Convert.ToDateTime(drx["m1date"]).ToString("yyyy-MM-dd HH:mm:ss"),
                            vali_flag = vali_flag
                        };
                        diseList.Add(diseinfo);
                        index++;
                    }
                }

                dynamic input = new
                {
                    mdtrtinfo = mdtrtinfo,
                    diseinfo = diseList
                };
                #endregion


                #endregion

                #region 接口调用
                string ErrStr = string.Empty;
                string inputData = JsonConvert.SerializeObject(input);
                WriteLog(sysdate + "  入参|" + inputData.ToString());
                int i = YBServiceRequest(ywCode, input, ref ErrStr);
                WriteLog(sysdate + "  出参|" + ErrStr.ToString());
                if (i <= 0)
                {
                    WriteLog(sysdate + "  门诊就诊信息上传/撤销|" + ErrStr.ToString());
                    return new object[] { 0, 0, "门诊就诊信息上传/撤销|" + ErrStr.ToString() };
                }
                WriteLog(sysdate + "  门诊就诊信息上传/撤销成功");
                return new object[] { 0, 1, "门诊就诊信息上传/撤销成功" };
                #endregion

            }
            catch (Exception ex)
            {
                WriteLog(sysdate + "  门诊就诊信息上传/撤销异常|" + ex.Message);
                return new object[] { 0, 0, "门诊就诊信息上传/撤销异常|" + ex.Message };
            }
        }
        #endregion

        #region 门诊登记(挂号)(新医保接口)
        public static object[] YBMZDJ(object[] objParam)
        {
            string Ywlx = "2201";
            string sysdate = GetServerDateTime();
            WriteLog(sysdate + "  进入门诊登记...");
            WriteLog(sysdate + "  HIS入参|" + string.Join(",", objParam));
            try
            {
                #region 入参
                string jzlsh = objParam[0].ToString(); //就诊流水号
                string yllb = objParam[1].ToString();   // 医疗类别代码
                string bzbm = objParam[2].ToString();   // 病种编码(icd10)
                string bzmc = objParam[3].ToString();   // 病种名称
                string[] kxx = objParam[4].ToString().Split('|'); //读卡返回信息
                string ghsj = objParam[5].ToString(); // 挂号时间
                //string ysdm = objParam[6].ToString(); //医生代码
                //string ysxm = objParam[7].ToString(); //医生姓名

                if (string.IsNullOrEmpty(jzlsh))
                    return new object[] { 0, 0, "就诊流水号不能为空" };
                if (string.IsNullOrEmpty(yllb))
                    return new object[] { 0, 0, "医疗类别代码不能为空|" };
                if (string.IsNullOrEmpty(ghsj))
                    return new object[] { 0, 0, "挂号时间不能为空|" };

                string[] syllb = { "14", "15" };
                if (string.IsNullOrEmpty(bzbm) && syllb.Contains(yllb))
                    return new object[] { 0, 0, "病种不能为空" };
                #endregion

                #region 变量
                string ydrybz = "0";
                string dqjbbz = "1";
                #region 获取读卡信息
                if (kxx.Length < 10)
                    return new object[] { 0, 0, "入参|读卡信息错误" };
                string grbh = kxx[0].ToString(); //个人编号
                string strSql11 = string.Format(@"select xm,yldylb,dwmc,xb,kh,tcqh,ydrybz,sjnl,csrq,zhye,sfczeyqf,gmsfhm,dwbh,xzlx,jzpzlx,jzpzbh from ybickxx where grbh='{0}'", grbh);
                DataSet dsDK = CliUtils.ExecuteSql("sybdj", "cmd", strSql11, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                DataRow dr = dsDK.Tables[0].Rows[0];
                string xm = dr["xm"].ToString();
                string yldylb = dr["yldylb"].ToString();
                string dwmc = dr["dwmc"].ToString();
                string xb = dr["xb"].ToString();
                string kh = dr["kh"].ToString();
                string tcqh = dr["tcqh"].ToString();
                ydrybz = dr["YDRYBZ"].ToString();
                string nl = dr["sjnl"].ToString();
                string csrq = dr["csrq"].ToString();
                string zhye = dr["zhye"].ToString();
                string sfczeyqf = dr["sfczeyqf"].ToString();
                string sfzh = dr["GMSFHM"].ToString();
                string dwbh = dr["dwbh"].ToString();
                string insutype = dr["xzlx"].ToString();//险种类型
                string begntime = ghsj;//开始时间
                string mdtrt_cert_type = dr["jzpzlx"].ToString(); //就诊凭证类型
                string mdtrt_cert_no = dr["jzpzbh"].ToString();//就诊凭证编号
                string ipt_otp_no = jzlsh;//住院/门诊号
                string insuplc_admvs1 = dr["tcqh"].ToString();//参保地医保区划
                insuplc_admdvs = insuplc_admvs1;
                if (ydrybz.Equals("1"))
                {
                    dqjbbz = "2";
                }
                if (string.IsNullOrEmpty(ydrybz))
                {
                    ydrybz = "0";
                }
                #endregion

                //if (yllb=="1101")
                //{
                //    yllb = ((int)med_type.普通门诊).ToString();
                //}
                //else
                //{
                //    yllb = med_type.门诊慢特病.ToString();
                //}
                string ksmc = string.Empty; //科室名称
                string ksbm = string.Empty; //科室编码
                string ybbh = string.Empty; //医保卡号
                string lxdh = string.Empty; //联系电话
                string cwh = "";    //床位号
                string ysbm = string.Empty;   //医生编码
                string ysmc = string.Empty;   //医生名称
                string jbr = CliUtils.fUserName; //经办人
                string hzxm = string.Empty; //患者姓名
                string blxx = string.Empty; //病历信息
                string ghf = string.Empty;  //挂号费
                string zcf = string.Empty;  //诊查费
                string sslb = ""; //计划生育手术类别
                string ssrq = "";//计划生育手术或生育日期
                string sylb = "";//生育类别
                string yzs = "";//孕周数
                string jzlsh1 = "";
                syllb = new string[] { "14", "15" };
                if (syllb.Contains(yllb))
                    jzlsh1 = yllb + jzlsh + bzbm;
                else
                    jzlsh1 = yllb + jzlsh;

                #endregion

                #region 生育门诊选择
                if (yllb.Equals("51"))
                {
                    Frm_ybmzysdj mzys = new Frm_ybmzysdj();
                    mzys.ShowDialog();
                    sslb = mzys.sslb;
                    ssrq = mzys.ssrq;
                    sylb = mzys.sylb;
                    yzs = mzys.yzs;
                    if (string.IsNullOrEmpty(sslb) || string.IsNullOrEmpty(ssrq))
                        return new object[] { 0, 0, "医疗类别为【41-门诊生育】时，计划生育手术类别/日期不能空" };
                }
                #endregion

                #region 医疗待遇封锁信息查询
                object[] objFS = { grbh, insutype, YBJGBH, yllb, ghsj, "", bzbm, bzmc, "", "", sylb, "" };
                objFS = YBYLDYFSXXCX(objFS);
                if (objFS[1].ToString().Equals("0"))
                {
                    DialogResult drs = MessageBox.Show(objFS[2].ToString() + "\r\n 是否取消当前操作？ 【是】 取消挂号 【否】继续挂号", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (drs == DialogResult.No)
                        return new object[] { 0, 0, "取消当前挂号作业！" };
                }
                #endregion
                //object[] objFS = new object[] { grbh };
                //if (yllb.Contains("14"))
                //{
                //    objFS = YBYLDYSPXXCX(objFS);
                //}

                #region 获取挂号信息
                string strSql = string.Format(@"select a.m1name as name,b.ybksdm,b.ybksmc,b.kb,c.dgysbm,c.ysxm,m1empn,b1name from mz01t a 
												left join bz01h h on h.b1empn=a.m1empn
                                                left join ybkszd b on a.m1ksno=b.ksdm
                                                left join ybdgyszd c on a.m1empn=c.ysbm where a.m1ghno='{0}' 
                                                union all
                                                select a.m1name as name,b.ybksdm,b.ybksmc,b.kb,c.dgysbm,c.ysxm,m1empn,b1name from mz01h a 
												left join bz01h h on h.b1empn=a.m1empn
                                                left join ybkszd b on a.m1ksno=b.ksdm
                                                left join ybdgyszd c on a.m1empn=c.ysbm where a.m1ghno='{0}' ", jzlsh);
                DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                if (ds.Tables[0].Rows.Count == 0)
                    return new object[] { 0, 0, "该患者未挂号" };
                string ysdm = ds.Tables[0].Rows[0]["m1empn"].ToString();
                string ysxm = ds.Tables[0].Rows[0]["b1name"].ToString();
                ybbh = grbh;
                //string caty = ds.Tables[0].Rows[0]["kb"].ToString();//科别
                ksbm = "D99";
                ksmc = "其它";

                if (!string.IsNullOrEmpty(ds.Tables[0].Rows[0]["ybksdm"].ToString()))
                {
                    ksbm = ds.Tables[0].Rows[0]["ybksdm"].ToString();
                    ksmc = ds.Tables[0].Rows[0]["ybksmc"].ToString();
                }
                ysbm = ysdm;
                ysmc = ysxm;
                hzxm = xm;
                string caty = "0101010101";
                if (string.IsNullOrEmpty(ysbm))
                {
                    ysbm = "admin";
                }
                if (string.IsNullOrEmpty(ysmc))
                {
                    ysmc = "管理员";
                }
                string atddr_no = ysbm;//医师编码
                string dr_name = ysmc;//医师姓名
                string dept_code = ksbm;//科室编码
                string dept_name = ksmc;//科室名称

                if (!string.Equals(hzxm, xm))
                    return new object[] { 0, 0, "挂号登记姓名和医保卡姓名不相符" };
                strSql = string.Format(@"select * from ybmzzydjdr where jzlsh1='{0}' and jzbz='m' and cxbz=1", jzlsh1);
                ds.Tables.Clear();
                ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                if (ds.Tables[0].Rows.Count > 0)
                    return new object[] { 0, 0, "患者已进行医保门诊登记，清匆再进行重复操作" };
                #endregion

                #region 入参赋值
                dynamic input = new
                {
                    data = new
                    {
                        psn_no = grbh,
                        insutype = insutype,
                        begntime = begntime,
                        mdtrt_cert_type = mdtrt_cert_type,
                        mdtrt_cert_no = mdtrt_cert_no,
                        ipt_otp_no = ipt_otp_no,
                        atddr_no = atddr_no,
                        dr_name = dr_name,
                        dept_code = dept_code,
                        dept_name = dept_name,
                        caty = caty
                    }
                };

                #endregion
                string ErrStr = string.Empty;
                #region 接口调用
                string inputData = JsonConvert.SerializeObject(input);
                WriteLog(sysdate + "  入参|" + inputData.ToString());
                int i = YBServiceRequest(Ywlx, input, ref ErrStr);
                WriteLog(sysdate + "  出参|" + ErrStr.ToString());
                #endregion

                #region 保存数据
                if (i > 0)
                {
                    JObject resobj = JsonConvert.DeserializeObject<JObject>(JsonConvert.DeserializeObject<JObject>(ErrStr).GetValue("data", StringComparison.OrdinalIgnoreCase).ToString());
                    string ybjzlsh = resobj.GetValue("mdtrt_id", StringComparison.OrdinalIgnoreCase).ToString();

                    string grbh1 = resobj.GetValue("psn_no", StringComparison.OrdinalIgnoreCase).ToString();
                    JYLSH = resobj.GetValue("ipt_otp_no", StringComparison.OrdinalIgnoreCase).ToString() + ybjzlsh;
                    strSql = string.Format(@"insert into ybmzzydjdr(
                                            jzlsh,jylsh,yldylb,ghdjsj,bzbm,bzmc,bq,ksbh,ksmc,cwh,
                                            dgysdm,jbr,dwmc,grbh,xm,xb,ybjzlsh,kh,yllb,cxbz,
                                            sysdate,tcqh,ydrybz,nl,csrq,zhye,jzbz,sfczeyqf,dqjbbz,jzlsh1,
                                            sysslb,jhsyrq,dgysxm,syfylb,syyzs,cbdybqh)
                                            values(
                                            '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}',
                                            '{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}',
                                            '{20}','{21}','{22}','{23}','{24}','{25}','{26}','{27}','{28}','{29}',
                                            '{30}','{31}','{32}','{33}','{34}','{35}')",
                                            jzlsh, JYLSH, yldylb, begntime, bzbm, bzmc, "", ksbm, ksmc, cwh,
                                            ysbm, jbr, dwmc, grbh1, xm, xb, ybjzlsh, kh, yllb, 1,
                                            sysdate, tcqh, ydrybz, nl, csrq, zhye, "m", sfczeyqf, dqjbbz, jzlsh1,
                                            sslb, ssrq, ysmc, sylb, yzs, insuplc_admvs1);
                    object[] obj_dj = { strSql };
                    obj_dj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj_dj);
                    if (obj_dj[1].ToString().Equals("1"))
                    {
                        if (!yllb.Equals("12"))
                        {
                            object[] objDJSC = { ybjzlsh, begntime, 1 };
                            objDJSC = YBMZJZXXSC(objDJSC); //门诊就诊信息上传
                            if (!objDJSC[1].ToString().Equals("1"))
                            {
                                object[] objDJCX = { jzlsh };
                                YBMZDJCX(objDJCX); //门诊挂号撤销
                                WriteLog(sysdate + "  门诊登记失败|就诊信息上传失败！|" + objDJSC[2].ToString());
                                return new object[] { 0, 0, "门诊登记失败|就诊信息上传失败|" + objDJSC[2].ToString() };
                            }
                        }

                        WriteLog(sysdate + "  门诊登记成功|");
                        return new object[] { 0, 1, "门诊登记成功" };
                    }
                    else
                    {
                        WriteLog(sysdate + "  门诊登记失败|数据操作失败|" + obj_dj[2].ToString());
                        //门诊登记撤销
                        object[] objParam1 = { grbh, ybjzlsh, jzlsh, insuplc_admvs1 };
                        objParam1 = N_YBMZDJCX(objParam1);
                        return new object[] { 0, 0, "门诊登记失败|数据操作失败|" + obj_dj[2].ToString() };
                    }
                }
                else
                {
                    WriteLog(sysdate + "  门诊登记失败|" + ErrStr.ToString());
                    return new object[] { 0, 0, ErrStr.ToString() };
                }
                #endregion
            }
            catch (Exception ex)
            {
                WriteLog(sysdate + "  进入门诊登记接口异常|" + ex.Message);
                return new object[] { 0, 0, "进入门诊登记接口异常|" + ex.Message };
            }
        }
        #endregion

        #region 门诊登记(挂号)撤销（新医保接口）


        public static object[] YBMZDJCX(object[] objParam)
        {
            string sysdate = GetServerDateTime();
            WriteLog(sysdate + "  进入门诊登记撤销...");
            WriteLog(sysdate + "  HIS入参|" + string.Join(",", objParam));
            string Ywlx = "2202";
            try
            {
                #region 入参
                string jzlsh = objParam[0].ToString();
                if (string.IsNullOrEmpty(jzlsh))
                    return new object[] { 0, 0, "就诊流水号不能为空" };
                #endregion

                #region 变量 
                CZYBH = CliUtils.fLoginUser;
                YWZQH = CliUtils.fLoginYbNo.Split('|')[0];
                string jbr = CliUtils.fUserName;
                string jylsh = string.Empty;
                #endregion

                #region 判断是否进行门诊登记
                string strSql = string.Format(@"select grbh,ybjzlsh,cbdybqh,ghdjsj,jylsh from ybmzzydjdr a where a.jzlsh='{0}' and a.cxbz=1 
                                                and not exists(select 1 from ybfyjsdr b where a.jzlsh=b.jzlsh and a.ybjzlsh=b.ybjzlsh and b.cxbz=1) ", jzlsh);
                DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                if (ds.Tables[0].Rows.Count == 0)
                {
                    WriteLog(sysdate + "  该患者无医保门诊挂号登记信息!");
                    return new object[] { 0, 0, "该患者无医保门诊挂号登记信息" };
                }
                string begntime = ds.Tables[0].Rows[0]["ghdjsj"].ToString();
                #endregion
                List<string> liSQL = new List<string>();
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    string grbh = dr["grbh"].ToString();
                    string ybjzlsh = dr["ybjzlsh"].ToString();
                    string cbdybqh = dr["cbdybqh"].ToString();
                    jylsh = dr["jylsh"].ToString();

                    #region 门诊就诊信息上传撤销
                    //object[] objDJSC = { ybjzlsh, begntime, 0 };
                    //objDJSC = YBMZJZXXSC(objDJSC); //门诊就诊信息上传
                    //if (!objDJSC[1].ToString().Equals("1"))
                    //    return objDJSC;
                    #endregion


                    //撤销门诊费用明细 
                    NYBMZGHFYMXSCCX(new object[] { jzlsh, ybjzlsh });



                    #region 入参
                    dynamic input = new
                    {
                        data = new
                        {
                            psn_no = grbh,
                            mdtrt_id = ybjzlsh,
                            ipt_otp_no = jzlsh
                        }
                    };
                    #endregion
                    string Err = string.Empty;
                    #region 接口调用
                    string inputData = JsonConvert.SerializeObject(input);
                    WriteLog(sysdate + "  入参|" + inputData.ToString());
                    int i = YBServiceRequest(Ywlx, input, ref Err);
                    WriteLog(sysdate + "  出参|" + Err.ToString());
                    #endregion

                    #region 数据处理
                    if (i != 1)
                    {
                        WriteLog(sysdate + "  医保门诊登记撤销失败|" + Err.ToString());
                        return new object[] { 0, 0, Err.ToString() };
                    }


                    strSql = string.Format(@"insert into ybmzzydjdr(
                                        jzlsh,jylsh,yldylb,ghdjsj,bzbm,bzmc,bq,ksbh,ksmc,cwh,
                                        ysdm,jbr,dwmc,grbh,xm,xb,ybjzlsh,kh,yllb,tcqh,
                                        ydrybz,nl,csrq,zhye,jzbz,cxbz,sysdate,jzlsh1,cbdybqh)
                                        select jzlsh,jylsh,yldylb,ghdjsj,bzbm,bzmc,bq,ksbh,ksmc,cwh,
                                        ysdm,'{1}',dwmc,grbh,xm,xb,ybjzlsh,kh,yllb,tcqh,
                                        ydrybz,nl,csrq,zhye,jzbz,0,'{2}',jzlsh1,cbdybqh 
                                        from ybmzzydjdr where jylsh='{0}' and cxbz=1", jylsh, jbr, sysdate);
                    liSQL.Add(strSql);
                    strSql = string.Format(@"update ybmzzydjdr set cxbz=2 where jylsh='{0}' and cxbz=1", jylsh);
                    liSQL.Add(strSql);
                    #endregion
                }

                #region 数据保存
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
                #endregion
            }
            catch (Exception ex)
            {
                WriteLog(sysdate + "  进入医保门诊登记撤销异常|" + ex.Message);
                return new object[] { 0, 0, ex.Message };
            }
        }

        #endregion

        #region 门诊挂号费用登记(门诊费用明细信息上传)
        public static object[] YBMZGHFDJ(object[] objParam)
        {
            string sysdate = GetServerDateTime();
            WriteLog(sysdate + "   |进入门诊挂号费上传...");
            WriteLog(sysdate + "   HIS入参|" + string.Join(",", objParam));
            try
            {
                #region 入参
                string jzlsh = objParam[0].ToString(); //流水号不能为空
                string ybjzlsh = objParam[1].ToString(); //医保就诊流水号
                if (string.IsNullOrWhiteSpace(jzlsh))
                    return new object[] { 0, 0, "医保提示：就诊流水号为空" };

                CZYBH = CliUtils.fLoginUser;
                string jbr = CZYBH;
                #endregion

                #region 获取医保登记信息
                string strSql = string.Format(@"select bzbm,bzmc,xm,grbh,kh,ybjzlsh,cbdybqh,jylsh from ybmzzydjdr where ybjzlsh='{0}' and cxbz=1 ", ybjzlsh);
                DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                if (ds.Tables[0].Rows.Count == 0)
                    return new object[] { 0, 0, "该患者未医保门诊登记" };
                string bzbm = ds.Tables[0].Rows[0]["bzbm"].ToString();  //病种编码
                string xm = ds.Tables[0].Rows[0]["xm"].ToString();
                string grbh = ds.Tables[0].Rows[0]["grbh"].ToString();
                string kh = ds.Tables[0].Rows[0]["kh"].ToString();
                string cbdybqh = ds.Tables[0].Rows[0]["cbdybqh"].ToString(); //参保地医保区划
                JYLSH = ds.Tables[0].Rows[0]["jylsh"].ToString();
                #endregion

                #region 费用上传前，撤销所有未结算费用
                object[] objGHFYDJCX = { jzlsh, ybjzlsh };
                //费用登记前，撤销费用登记
                objGHFYDJCX = YBMZCFMXSCCX(objGHFYDJCX);
                #endregion

                #region 获取费用明细

                #region 变量定义
                string feedetl_sn = "";// 费用明细流水号
                string mdtrt_id = ybjzlsh;// 就诊 ID
                string psn_no = grbh;// 人员编号
                string chrg_bchno = DateTime.Now.ToString("yyyyMMddHHmmss");// 收费批次号
                string dise_codg = bzbm;// 病种编码
                string rxno = "";// 处方号
                string rx_circ_flag = "0";// 外购处方标志
                string fee_ocur_time = "";// 费用发生时间
                string med_list_codg = "";// 医疗目录编码
                string medins_list_codg = "";// 医药机构目录编码
                string det_item_fee_sumamt = "";// 明细项目费用总额
                string cnt = "";// 数量
                string pric = "";// 单价
                string sin_dos_dscr = "";// 单次剂量描述
                string used_frqu_dscr = "";// 使用频次描述
                string prd_days = "1";// 周期天数
                string medc_way_sdcr = "";// 用药途径描述
                string bilg_dept_codg = "";// 开单科室编码
                string bilg_dept_name = "";
                string bilg_dr_codg = "";// 开单医生编码
                string bilg_dr_name = "";// 开单医师姓名
                string acord_dept_codg = "";// 受单科室编码
                string acord_dept_name = "";// 受单科室名称
                string orders_dr_code = "";// 受单医生编码
                string orders_dr_name = "";// 受单医生姓名
                string hosp_appr_flag = "1";// 医院审批标志
                string tcmdrug_used_way = "";// 中药使用方式
                string etip_flag = "";// 外检标志
                string etip_hosp_code = "";// 外检医院编码
                string dscg_tkdrug_flag = "";// 出院带药标志
                string matn_fee_flag = "";// 生育费用标志

                string yyxmmc = ""; //医院项目名称
                string ybxmmc = ""; //医保项目名称
                string sfxmzl = ""; //收费项目种类
                string sflbdm = ""; //收费类别
                string fph = string.Empty; //发票号
                double blbfy = 0.00; //病历本费用
                double ylkfy = 0.00; //医疗卡费用
                double ghfy = 0.00;    //挂号总费用
                double ghfylj = 0.00;
                double ghfyvv = 0.00;
                #endregion

                int index = 1;
                List<string> liSQL = new List<string>();
                List<dynamic> feedetail = new List<dynamic>();

                #region 获取诊查费

                #region 获取诊查费信息
                strSql = string.Format(@"select z.sfxmzldm,z.sflbdm,'' as cfh, a.m1date as cfrq,z.ybxmbh,z.ybxmmc,c.bzmem2 as yyxmbh, c.bzname as yyxmmc,c.bzmem1 as dj,1 as sl,a.m1gham je,
                                         isnull(f.dgysbm,'admin')   as ysbm,isnull(f.ysxm,'admin') as ysxm, a.m1ksno as ksbm,a.m1ksnm as ksmc,a.m1invo,a.m1gham1,a.m1blam,a.m1kham,a.m1amnt
                                        from mz01t a           
                                        join bztbd c on a.m1zlfb = c.bzkeyx and c.bzcodn = 'A7' 
                                        left join ybhisdzdrnew z on c.bzmem2 = z.hisxmbh
                                        left join ybkszd e on a.m1ksno=e.ksdm
                                        left join ybdgyszd f on a.m1user=f.ysbm                
                                        where a.m1ghno = '{0}'", jzlsh);
                ds.Tables.Clear();
                ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                DataRow dr = ds.Tables[0].Rows[0];
                fph = dr["m1invo"].ToString();
                feedetl_sn = fph + index.ToString();
                rxno = index.ToString() + DateTime.Now.ToString("HHmmss");    //处方号
                fee_ocur_time = dr["cfrq"].ToString();
                med_list_codg = dr["ybxmbh"].ToString();
                medins_list_codg = dr["yyxmbh"].ToString();
                det_item_fee_sumamt = dr["je"].ToString();
                cnt = dr["sl"].ToString();
                pric = dr["dj"].ToString();
                bilg_dept_codg = dr["ksbm"].ToString();
                bilg_dept_name = dr["ksmc"].ToString();
                bilg_dr_codg = dr["ysbm"].ToString();
                bilg_dr_name = dr["ysxm"].ToString();
                hosp_appr_flag = "1";
                sfxmzl = dr["sfxmzldm"].ToString();
                yyxmmc = dr["ybxmmc"].ToString();
                ybxmmc = dr["yyxmmc"].ToString();
                sflbdm = dr["sflbdm"].ToString();

                blbfy = Convert.ToDouble(dr["m1blam"].ToString());
                ylkfy = Convert.ToDouble(dr["m1kham"].ToString());
                ghfy = Convert.ToDouble(dr["m1amnt"].ToString());
                ghfyvv = Convert.ToDouble(dr["m1gham1"].ToString());
                index++;
                #endregion

                #region 入参赋值
                dynamic feed_zc = new
                {
                    feedetl_sn = feedetl_sn,
                    mdtrt_id = mdtrt_id,
                    psn_no = psn_no,
                    chrg_bchno = chrg_bchno,
                    dise_codg = dise_codg,
                    rxno = rxno,
                    rx_circ_flag = rx_circ_flag,
                    fee_ocur_time = fee_ocur_time,
                    med_list_codg = med_list_codg,
                    medins_list_codg = medins_list_codg,
                    det_item_fee_sumamt = det_item_fee_sumamt,
                    cnt = cnt,
                    pric = pric,
                    sin_dos_dscr = sin_dos_dscr,
                    used_frqu_dscr = used_frqu_dscr,
                    prd_days = prd_days,
                    medc_way_sdcr = medc_way_sdcr,
                    bilg_dept_codg = bilg_dept_codg,
                    bilg_dept_name = bilg_dept_name,
                    bilg_dr_codg = bilg_dr_codg,
                    bilg_dr_name = bilg_dr_name,
                    acord_dept_codg = acord_dept_codg,
                    acord_dept_name = acord_dept_name,
                    orders_dr_code = orders_dr_code,
                    orders_dr_name = orders_dr_name,
                    hosp_appr_flag = hosp_appr_flag,
                    tcmdrug_used_way = tcmdrug_used_way,
                    etip_flag = etip_flag,
                    etip_hosp_code = etip_hosp_code,
                    dscg_tkdrug_flag = dscg_tkdrug_flag,
                    matn_fee_flag = matn_fee_flag
                };
                feedetail.Add(feed_zc);
                ghfylj += Convert.ToDouble(det_item_fee_sumamt);

                #endregion

                #region 数据处理
                strSql = string.Format(@"insert into ybcfmxscindr(jzlsh,jylsh,sfxmzl,ybcfh,djlsh,cfrq,yysfxmbm,yysfxmmc,dj,sl,
						                 je,ysbm,ksbh,jbr,sflb,grbh,xm,kh,cxbz,sysdate,
                                         ybjzlsh,sfxmzxbm,sfxmzxmc,ysxm,ksmc,ybdjh) values(
                                         '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}',
                                         '{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}',
                                         '{20}','{21}','{22}','{23}','{24}','{25}')",
                                         jzlsh, JYLSH, sfxmzl, feedetl_sn, index, fee_ocur_time, medins_list_codg, yyxmmc, pric, cnt,
                                         det_item_fee_sumamt, bilg_dr_codg, bilg_dept_codg, jbr, sflbdm, grbh, xm, kh, 1, sysdate,
                                         ybjzlsh, med_list_codg, ybxmmc, bilg_dr_name, bilg_dept_name, chrg_bchno);
                liSQL.Add(strSql);
                #endregion
                #endregion

                #region 获取挂号费
                if (ghfyvv > 0)
                {
                    #region 获取挂号费信息
                    strSql = string.Format(@"select z.sfxmzldm,z.sflbdm,'' as cfh, a.m1date as cfrq,z.ybxmbh,z.ybxmmc,c.bzmem2 as yyxmbh, c.bzname as yyxmmc,c.bzmem1 as dj,1 as sl,a.m1gham je,
                                            f.dgysbm as ysbm,f.ysxm, e.ybksdm as ksbm,e.ksmc
                                            from mz01h a  
                                            join bztbd c on a.m1jzhb = c.bzkeyx and c.bzcodn = 'A6' 
                                            left join ybhisdzdrnew z on c.bzmem2 = z.hisxmbh									
                                            left join ybkszd e on a.m1ksno=e.ksdm
                                            left join ybdgyszd f on a.m1empn=f.ysbm      
                                            where a.m1ghno = '{0}'", jzlsh);
                    ds.Tables.Clear();
                    ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                    dr = ds.Tables[0].Rows[0];

                    fph = dr["m1invo"].ToString();
                    feedetl_sn = fph + index.ToString();
                    rxno = index.ToString() + DateTime.Now.ToString("HHmmss");    //处方号
                    fee_ocur_time = dr["cfrq"].ToString();
                    med_list_codg = dr["ybxmbh"].ToString();
                    medins_list_codg = dr["yyxmbh"].ToString();
                    det_item_fee_sumamt = dr["je"].ToString();
                    cnt = dr["sl"].ToString();
                    pric = dr["dj"].ToString();
                    bilg_dept_codg = dr["ksbm"].ToString();
                    bilg_dept_name = dr["ksmc"].ToString();
                    bilg_dr_codg = dr["ysbm"].ToString();
                    bilg_dr_name = dr["ysxm"].ToString();
                    hosp_appr_flag = "1";
                    sfxmzl = dr["sfxmzldm"].ToString();
                    yyxmmc = dr["ybxmmc"].ToString();
                    ybxmmc = dr["yyxmmc"].ToString();
                    sflbdm = dr["sflbdm"].ToString();
                    index++;
                    #endregion

                    #region 入参赋值
                    dynamic feed_gh = new
                    {
                        feedetl_sn = feedetl_sn,
                        mdtrt_id = mdtrt_id,
                        psn_no = psn_no,
                        chrg_bchno = chrg_bchno,
                        dise_codg = dise_codg,
                        rxno = rxno,
                        rx_circ_flag = rx_circ_flag,
                        fee_ocur_time = fee_ocur_time,
                        med_list_codg = med_list_codg,
                        medins_list_codg = medins_list_codg,
                        det_item_fee_sumamt = det_item_fee_sumamt,
                        cnt = cnt,
                        pric = pric,
                        sin_dos_dscr = sin_dos_dscr,
                        used_frqu_dscr = used_frqu_dscr,
                        prd_days = prd_days,
                        medc_way_sdcr = medc_way_sdcr,
                        bilg_dept_codg = bilg_dept_codg,
                        bilg_dept_name = bilg_dept_name,
                        bilg_dr_codg = bilg_dr_codg,
                        bilg_dr_name = bilg_dr_name,
                        acord_dept_codg = acord_dept_codg,
                        acord_dept_name = acord_dept_name,
                        orders_dr_code = orders_dr_code,
                        orders_dr_name = orders_dr_name,
                        hosp_appr_flag = hosp_appr_flag,
                        tcmdrug_used_way = tcmdrug_used_way,
                        etip_flag = etip_flag,
                        etip_hosp_code = etip_hosp_code,
                        dscg_tkdrug_flag = dscg_tkdrug_flag,
                        matn_fee_flag = matn_fee_flag
                    };
                    feedetail.Add(feed_gh);
                    ghfylj += Convert.ToDouble(det_item_fee_sumamt);
                    #endregion

                    #region 数据处理
                    strSql = string.Format(@"insert into ybcfmxscindr(jzlsh,jylsh,sfxmzl,ybcfh,djlsh,cfrq,yysfxmbm,yysfxmmc,dj,sl,
						                 je,ysbm,ksbh,jbr,sflb,grbh,xm,kh,cxbz,sysdate,
                                         ybjzlsh,sfxmzxbm,sfxmzxmc,ysxm,ksmc,ybdjh) values(
                                         '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}',
                                         '{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}',
                                         '{20}','{21}','{22}','{23}','{24}','{25}')",
                                             jzlsh, JYLSH, sfxmzl, feedetl_sn, index, fee_ocur_time, medins_list_codg, yyxmmc, pric, cnt,
                                             det_item_fee_sumamt, bilg_dr_codg, bilg_dept_codg, jbr, sflbdm, grbh, xm, kh, 1, sysdate,
                                             ybjzlsh, med_list_codg, ybxmmc, bilg_dr_name, bilg_dept_name, chrg_bchno);
                    liSQL.Add(strSql);
                    #endregion
                }

                #endregion

                #region 获取病历本费
                if (blbfy > 0)
                {
                    #region 获取病历本费用信息
                    strSql = string.Format(@"select z.sfxmzldm,z.sflbdm,'' as cfh, a.m1date as cfrq,z.ybxmbh,z.ybxmmc,b.b5item as yyxmbh, b.b5name as yyxmmc,b.b5sfam as dj,1 as sl,b.b5sfam je,
                                            f.dgysbm as ysbm,f.ysxm, e.ybksdm as ksbm,e.ksmc
                                            from mz01h a  
                                            left join bz05d b on b.b5item=(select pamark from patbh where pakind='MZ' and pasequ='0001')
                                            left join ybhisdzdrnew z on b.b5item = z.hisxmbh									
                                            left join ybkszd e on a.m1ksno=e.ksdm
                                            left join ybdgyszd f on a.m1empn=f.ysbm  
                                            where a.m1ghno = '{0}'", jzlsh);

                    ds.Tables.Clear();
                    ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                    dr = ds.Tables[0].Rows[0];

                    fph = dr["m1invo"].ToString();
                    feedetl_sn = fph + index.ToString();
                    rxno = index.ToString() + DateTime.Now.ToString("HHmmss");    //处方号
                    fee_ocur_time = dr["cfrq"].ToString();
                    med_list_codg = dr["ybxmbh"].ToString();
                    medins_list_codg = dr["yyxmbh"].ToString();
                    det_item_fee_sumamt = dr["je"].ToString();
                    cnt = dr["sl"].ToString();
                    pric = dr["dj"].ToString();
                    bilg_dept_codg = dr["ksbm"].ToString();
                    bilg_dept_name = dr["ksmc"].ToString();
                    bilg_dr_codg = dr["ysbm"].ToString();
                    bilg_dr_name = dr["ysxm"].ToString();
                    hosp_appr_flag = "1";
                    sfxmzl = dr["sfxmzldm"].ToString();
                    yyxmmc = dr["ybxmmc"].ToString();
                    ybxmmc = dr["yyxmmc"].ToString();
                    sflbdm = dr["sflbdm"].ToString();
                    index++;
                    #endregion

                    #region 入参赋值
                    dynamic feed_bl = new
                    {
                        feedetl_sn = feedetl_sn,
                        mdtrt_id = mdtrt_id,
                        psn_no = psn_no,
                        chrg_bchno = chrg_bchno,
                        dise_codg = dise_codg,
                        rxno = rxno,
                        rx_circ_flag = rx_circ_flag,
                        fee_ocur_time = fee_ocur_time,
                        med_list_codg = med_list_codg,
                        medins_list_codg = medins_list_codg,
                        det_item_fee_sumamt = det_item_fee_sumamt,
                        cnt = cnt,
                        pric = pric,
                        sin_dos_dscr = sin_dos_dscr,
                        used_frqu_dscr = used_frqu_dscr,
                        prd_days = prd_days,
                        medc_way_sdcr = medc_way_sdcr,
                        bilg_dept_codg = bilg_dept_codg,
                        bilg_dept_name = bilg_dept_name,
                        bilg_dr_codg = bilg_dr_codg,
                        bilg_dr_name = bilg_dr_name,
                        acord_dept_codg = acord_dept_codg,
                        acord_dept_name = acord_dept_name,
                        orders_dr_code = orders_dr_code,
                        orders_dr_name = orders_dr_name,
                        hosp_appr_flag = hosp_appr_flag,
                        tcmdrug_used_way = tcmdrug_used_way,
                        etip_flag = etip_flag,
                        etip_hosp_code = etip_hosp_code,
                        dscg_tkdrug_flag = dscg_tkdrug_flag,
                        matn_fee_flag = matn_fee_flag
                    };
                    feedetail.Add(feed_bl);
                    ghfylj += Convert.ToDouble(det_item_fee_sumamt);
                    #endregion

                    #region 数据处理
                    strSql = string.Format(@"insert into ybcfmxscindr(jzlsh,jylsh,sfxmzl,ybcfh,djlsh,cfrq,yysfxmbm,yysfxmmc,dj,sl,
						                 je,ysbm,ksbh,jbr,sflb,grbh,xm,kh,cxbz,sysdate,
                                         ybjzlsh,sfxmzxbm,sfxmzxmc,ysxm,ksmc,ybdjh) values(
                                         '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}',
                                         '{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}',
                                         '{20}','{21}','{22}','{23}','{24}','{25}')",
                                             jzlsh, JYLSH, sfxmzl, feedetl_sn, index, fee_ocur_time, medins_list_codg, yyxmmc, pric, cnt,
                                             det_item_fee_sumamt, bilg_dr_codg, bilg_dept_codg, jbr, sflbdm, grbh, xm, kh, 1, sysdate,
                                             ybjzlsh, med_list_codg, ybxmmc, bilg_dr_name, bilg_dept_name, chrg_bchno);
                    liSQL.Add(strSql);
                    #endregion
                }
                #endregion

                #region 获取医疗卡费
                if (ylkfy > 0)
                {
                    #region 获取病历本费用信息
                    strSql = string.Format(@"select z.sfxmzldm,z.sflbdm,'' as cfh, a.m1date as cfrq,z.ybxmbh,z.ybxmmc,b.b5item as yyxmbh, b.b5name as yyxmmc,b.b5sfam as dj,1 as sl,b.b5sfam je,
                                            f.dgysbm as ysbm,f.ysxm, e.ybksdm as ksbm,e.ksmc
                                            from mz01h a  
                                            left join bz05d b on b.b5item=(select pamark from patbh where pakind='MZ' and pasequ='0004')
                                            left join ybhisdzdrnew z on b.b5item = z.hisxmbh									
                                            left join ybkszd e on a.m1ksno=e.ksdm
                                            left join ybdgyszd f on a.m1empn=f.ysbm  
                                            where a.m1ghno = '{0}'", jzlsh);

                    ds.Tables.Clear();
                    ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                    dr = ds.Tables[0].Rows[0];

                    fph = dr["m1invo"].ToString();
                    feedetl_sn = fph + index.ToString();
                    rxno = index.ToString() + DateTime.Now.ToString("HHmmss");    //处方号
                    fee_ocur_time = dr["cfrq"].ToString();
                    med_list_codg = dr["ybxmbh"].ToString();
                    medins_list_codg = dr["yyxmbh"].ToString();
                    det_item_fee_sumamt = dr["je"].ToString();
                    cnt = dr["sl"].ToString();
                    pric = dr["dj"].ToString();
                    bilg_dept_codg = dr["ksbm"].ToString();
                    bilg_dept_name = dr["ksmc"].ToString();
                    bilg_dr_codg = dr["ysbm"].ToString();
                    bilg_dr_name = dr["ysxm"].ToString();
                    hosp_appr_flag = "1";
                    sfxmzl = dr["sfxmzldm"].ToString();
                    yyxmmc = dr["ybxmmc"].ToString();
                    ybxmmc = dr["yyxmmc"].ToString();
                    sflbdm = dr["sflbdm"].ToString();
                    index++;
                    #endregion

                    #region 入参赋值
                    dynamic feed_kf = new
                    {
                        feedetl_sn = feedetl_sn,
                        mdtrt_id = mdtrt_id,
                        psn_no = psn_no,
                        chrg_bchno = chrg_bchno,
                        dise_codg = dise_codg,
                        rxno = rxno,
                        rx_circ_flag = rx_circ_flag,
                        fee_ocur_time = fee_ocur_time,
                        med_list_codg = med_list_codg,
                        medins_list_codg = medins_list_codg,
                        det_item_fee_sumamt = det_item_fee_sumamt,
                        cnt = cnt,
                        pric = pric,
                        sin_dos_dscr = sin_dos_dscr,
                        used_frqu_dscr = used_frqu_dscr,
                        prd_days = prd_days,
                        medc_way_sdcr = medc_way_sdcr,
                        bilg_dept_codg = bilg_dept_codg,
                        bilg_dept_name = bilg_dept_name,
                        bilg_dr_codg = bilg_dr_codg,
                        bilg_dr_name = bilg_dr_name,
                        acord_dept_codg = acord_dept_codg,
                        acord_dept_name = acord_dept_name,
                        orders_dr_code = orders_dr_code,
                        orders_dr_name = orders_dr_name,
                        hosp_appr_flag = hosp_appr_flag,
                        tcmdrug_used_way = tcmdrug_used_way,
                        etip_flag = etip_flag,
                        etip_hosp_code = etip_hosp_code,
                        dscg_tkdrug_flag = dscg_tkdrug_flag,
                        matn_fee_flag = matn_fee_flag
                    };
                    feedetail.Add(feed_kf);
                    ghfylj += Convert.ToDouble(det_item_fee_sumamt);
                    #endregion

                    #region 数据处理
                    strSql = string.Format(@"insert into ybcfmxscindr(jzlsh,jylsh,sfxmzl,ybcfh,djlsh,cfrq,yysfxmbm,yysfxmmc,dj,sl,
						                 je,ysbm,ksbh,jbr,sflb,grbh,xm,kh,cxbz,sysdate,
                                         ybjzlsh,sfxmzxbm,sfxmzxmc,ysxm,ksmc,ybdjh) values(
                                         '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}',
                                         '{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}',
                                         '{20}','{21}','{22}','{23}','{24}','{25}')",
                                             jzlsh, JYLSH, sfxmzl, feedetl_sn, index, fee_ocur_time, medins_list_codg, yyxmmc, pric, cnt,
                                             det_item_fee_sumamt, bilg_dr_codg, bilg_dept_codg, jbr, sflbdm, grbh, xm, kh, 1, sysdate,
                                             ybjzlsh, med_list_codg, ybxmmc, bilg_dr_name, bilg_dept_name, chrg_bchno);
                    liSQL.Add(strSql);
                    #endregion
                }
                #endregion

                //判断总费用与累计费用是否相等
                if (Math.Abs(ghfy - ghfylj) > 1.0)
                    return new object[] { 0, 0, "收费金额与医保结算金额相差" + Math.Abs(ghfy - ghfylj) + ",无法结算，请核实!" };
                #endregion

                #region 接口入参
                dynamic input = new
                {
                    feedetail = feedetail
                };

                #endregion
                string Err = string.Empty;
                #region 接口调用
                string inputData = JsonConvert.SerializeObject(input);
                WriteLog(sysdate + "  入参|" + inputData.ToString());

                int i = YBServiceRequest("2204", input, ref Err);
                ///OutpatientFeeOutParam
                WriteLog(sysdate + "  出参|" + Err.ToString());
                #endregion

                #region 数据处理
                if (i != 1)
                {
                    WriteLog(sysdate + "  门诊挂号费上传失败|" + Err);
                    return new object[] { 0, 0, Err };
                }
                JObject j = JsonConvert.DeserializeObject<JObject>(Err);
                List<OutpatientFeeOutParam> outlist = JsonConvert.DeserializeObject<List<OutpatientFeeOutParam>>(j.GetValue("result", StringComparison.OrdinalIgnoreCase).ToString());
                //处理返回明细信息
                foreach (OutpatientFeeOutParam result in outlist)
                {
                    string feedetl_sn1 = result.feedetl_sn;   //费用明细流水号
                    string det_item_fee_sumamt1 = result.det_item_fee_sumamt.ToString();   //明细项目费用总额
                    string cnt1 = result.cnt.ToString();   //数量
                    string pric1 = result.pric.ToString();   //单价
                    string pric_uplmt_amt = result.pric_uplmt_amt.ToString();   //定价上限金额
                    string selfpay_prop = result.selfpay_prop.ToString();   //自付比例
                    string fulamt_ownpay_amt = result.fulamt_ownpay_amt.ToString();   //全自费金额
                    string overlmt_amt = result.overlmt_amt.ToString();   //超限价金额
                    string preselfpay_amt = result.preselfpay_amt.ToString();   //先行自付金额
                    string inscp_scp_amt = result.inscp_scp_amt.ToString();   //符合政策范围金额
                    string chrgitm_lv = result.chrgitm_lv.ToString();   //收费项目等级
                    string med_chrgitm_type = result.med_chrgitm_type.ToString();   //医疗收费项目类别
                    string bas_medn_flag = result.bas_medn_flag;   //基本药物标志
                    string hi_nego_drug_flag = result.hi_nego_drug_flag;   //医保谈判药品标志
                    string chld_medc_flag = result.chld_medc_flag;   //儿童用药标志
                    string list_sp_item_flag = result.list_sp_item_flag;   //目录特项标志
                    string lmt_used_flag = result.lmt_used_flag;   //限制使用标志
                    string drt_reim_flag = result.drt_reim_flag;   //直报标志
                    string memo = result.memo;   //备注

                    strSql = string.Format(@"insert into ybcfmxscfhdr(ybcfh,je,sl,dj,djsxje,zlbl,zfje,cxjzfje,xxzfje,fhzcfwje,
                                            sfxmdj,sflb,jjywbz,ybtbypbz,etyybz,mltsbz,qezfbz,zbbz,bz,sysdate,
                                            jylsh,ybjzlsh,ybdjh,jzlsh) values(
                                            '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}',
                                            '{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}',
                                            '{20}','{21}','{22}','{23}')",
                                            feedetl_sn1, det_item_fee_sumamt1, cnt1, pric1, pric_uplmt_amt, selfpay_prop, fulamt_ownpay_amt, overlmt_amt, preselfpay_amt, inscp_scp_amt,
                                            chrgitm_lv, med_chrgitm_type, bas_medn_flag, hi_nego_drug_flag, chld_medc_flag, list_sp_item_flag, lmt_used_flag, drt_reim_flag, memo, sysdate,
                                            JYLSH, ybjzlsh, chrg_bchno, jzlsh);
                    liSQL.Add(strSql);
                }

                object[] obj = liSQL.ToArray();
                obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);
                if (obj[1].ToString().Equals("1"))
                {
                    WriteLog(sysdate + "  门诊挂号费登记成功|");
                    return new object[] { 0, 1, JYLSH, fph };
                }
                else
                {
                    WriteLog(sysdate + "  门诊挂号费登记失败|数据操作失败|" + obj[2].ToString());
                    //门诊费用上传撤销
                    object[] objParam1 = { ybjzlsh, grbh, cbdybqh };
                    objParam1 = N_YBMZSFSCCX(objParam1);
                    return new object[] { 0, 0, "门诊挂号费登记失败|数据操作失败|" + obj[2].ToString() };
                }
                #endregion
            }
            catch (Exception ex)
            {
                WriteLog(sysdate + "  进入门诊挂号费登记异常|" + ex.Message);
                return new object[] { 0, 0, ex.Message };
            }
        }
        #endregion 

        #region 门诊登记(挂号)预收费
        public static object[] YBMZDJYSF(object[] objParam)
        {
            string sysdate = GetServerDateTime();
            WriteLog(sysdate + "进入门诊登记预收费...");
            WriteLog(sysdate + "  HIS入参|" + string.Join(",", objParam));
            try
            {
                #region 入参
                string jzlsh = objParam[0].ToString();      // 就诊流水号
                string yllb = objParam[1].ToString(); //医疗类别代码
                string dqrq = objParam[2].ToString();  // 结算时间 yyyy-mm-dd hh:mm:ss
                string bzbm = objParam[3].ToString(); //病种编码
                string bzmc = objParam[4].ToString(); //病种名称
                string kxx = objParam[5].ToString(); //读卡返回信息
                string dkbz = objParam[6].ToString();//读卡标志 
                string dgysdm = objParam[7].ToString(); //医生代码
                string dgysxm = objParam[8].ToString(); //医生姓名
                string ydbz = objParam[9].ToString(); //异地标志 (1-市本级 2-异地)
                string jsfs = "01"; //结算方式

                if (string.IsNullOrEmpty(jzlsh))
                    return new object[] { 0, 0, "就诊流水号不能为空" };
                if (string.IsNullOrEmpty(yllb))
                    return new object[] { 0, 0, "医疗类别代码不能为空" };
                if (string.IsNullOrEmpty(jsfs))
                    return new object[] { 0, 0, "结算方式不能为空" };
                #endregion

                #region 变量
                string jzlsh1 = ""; //就诊流水号
                string ghdjsj = Convert.ToDateTime(objParam[2]).ToString("yyyy-MM-dd HH:mm:ss");//挂号时间

                string[] syllb = { "14", "15" };
                if (syllb.Contains(yllb))
                    jzlsh1 = yllb + jzlsh + bzbm;
                else
                    jzlsh1 = yllb + jzlsh;
                #endregion

                #region 判断是否医保门诊登记
                string strSql = string.Format(@"select a.*,b.* from ybmzzydjdr a inner join ybickxx b on a.grbh=b.grbh where a.jzlsh1='{0}' and cxbz=1", jzlsh1);
                DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                if (ds.Tables[0].Rows.Count == 0)
                {
                    object[] objDJ = { jzlsh, yllb, bzbm, bzmc, kxx, dqrq };
                    objDJ = YBMZDJ(objDJ);
                    if (!objDJ[1].ToString().Equals("1"))
                        return objDJ;
                }
                ds.Tables.Clear();
                ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                string xm = ds.Tables[0].Rows[0]["xm"].ToString();
                string grbh = ds.Tables[0].Rows[0]["grbh"].ToString();
                string kh = ds.Tables[0].Rows[0]["kh"].ToString();
                string cbdybqh = ds.Tables[0].Rows[0]["tcqh"].ToString(); //参保地医保区划
                string mdtrt_cert_type = ds.Tables[0].Rows[0]["jzpzlx"].ToString(); //就诊凭证类型
                string mdtrt_cert_no = ds.Tables[0].Rows[0]["jzpzbh"].ToString();//就诊凭证编号
                string insutype = ds.Tables[0].Rows[0]["xzlx"].ToString();//
                string ybjzlsh = ds.Tables[0].Rows[0]["ybjzlsh"].ToString(); //医保就诊流水号
                #endregion

                #region 挂号费上传
                object[] objGHFYDJ = { jzlsh, ybjzlsh };
                objGHFYDJ = YBMZGHFDJ(objGHFYDJ);
                if (!objGHFYDJ[1].ToString().Equals("1"))
                    return objGHFYDJ;
                JYLSH = objGHFYDJ[2].ToString();
                string fph = objGHFYDJ[3].ToString();

                strSql = string.Format(@"select ybdjh,sum(je) ylfze,isnull(sum(zfje),0.00) zfje, isnull(sum(cxjzfje),0.00) as cxjje,isnull(sum(xxzfje),0.00) as xxzfje, isnull(sum(fhzcfwje),0.00) as fhzcfwje
                                        from ybcfmxscfhdr where jylsh='{0}' group by ybdjh ", JYLSH);
                ds.Tables.Clear();
                ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                if (ds.Tables[0].Rows.Count == 0)
                {
                    WriteLog(sysdate + "  获取上传费用明细失败!");
                    return new object[] { 0, 0, "获取上传费用明细失败！" };
                }
                DataRow dr = ds.Tables[0].Rows[0];
                string ylfze = dr["ylfze"].ToString();
                string zfje = dr["zfje"].ToString();
                string cxjje = dr["cxjje"].ToString();
                string xxzfje = dr["xxzfje"].ToString();
                string fhzcfwje = dr["fhzcfwje"].ToString();
                string sfpch = dr["ybdjh"].ToString();
                #endregion

                #region 预结算入参
                dynamic data = new
                {
                    data = new
                    {
                        psn_no = grbh,
                        mdtrt_cert_type = mdtrt_cert_type,
                        mdtrt_cert_no = mdtrt_cert_no,
                        med_type = yllb,
                        medfee_sumamt = ylfze,
                        psn_setlway = jsfs,
                        mdtrt_id = ybjzlsh,
                        chrg_bchno = sfpch,
                        insutype = insutype,
                        acct_used_flag = "0"

                    }
                };
                #endregion

                string Err = string.Empty;
                //2206
                #region 接口调用
                string inputData = JsonConvert.SerializeObject(data);
                WriteLog(sysdate + "  入参|" + inputData.ToString());
                int i = YBServiceRequest("2206", data, ref Err);
                if (i != 1)
                {
                    WriteLog(sysdate + "  门诊登记(挂号)预收费失败|" + Err.ToString());
                    return new object[] { 0, 0, "门诊登记(挂号)预收费失败|" + Err.ToString() };
                }
                #endregion
                JObject j = JsonConvert.DeserializeObject<JObject>(Err);
                #region 数据处理
                //输出-结算信息
                string mdtrt_id = Convert.ToString(j["setlinfo"]["mdtrt_id"]);	//就诊ID
                string psn_no = Convert.ToString(j["setlinfo"]["psn_no"]);	//	人员编号
                string psn_name = Convert.ToString(j["setlinfo"]["psn_name"]);	//	人员姓名
                string psn_cert_type = Convert.ToString(j["setlinfo"]["psn_cert_type"]);	//	人员证件类型
                string certno = Convert.ToString(j["setlinfo"]["certno"]);	//	证件号码
                string gend = Convert.ToString(j["setlinfo"]["gend"]);	//	性别
                string naty = Convert.ToString(j["setlinfo"]["naty"]);	//	民族
                string brdy = Convert.ToString(j["setlinfo"]["brdy"]);	//	出生日期
                string age = Convert.ToString(j["setlinfo"]["age"]);	//	年龄
                string insutype1 = Convert.ToString(j["setlinfo"]["insutype1"]);	//	险种类型
                string psn_type = Convert.ToString(j["setlinfo"]["psn_type"]);	//	人员类别
                string cvlserv_flag = Convert.ToString(j["setlinfo"]["cvlserv_flag"]);	//	公务员标志
                string setl_time = Convert.ToString(j["setlinfo"]["setl_time"]);	//	结算时间
                string mdtrt_cert_type1 = Convert.ToString(j["setlinfo"]["mdtrt_cert_type"]);	//	就诊凭证类型
                string med_type = Convert.ToString(j["setlinfo"]["med_type"]);	//	医疗类别
                string medfee_sumamt = Convert.ToString(j["setlinfo"]["medfee_sumamt"]);	//	医疗费总额
                string fulamt_ownpay_amt = Convert.ToString(j["setlinfo"]["fulamt_ownpay_amt"]);	//	全自费金额
                string overlmt_selfpay = Convert.ToString(j["setlinfo"]["overlmt_selfpay"]);	//	超限价自费费用
                string preselfpay_amt = Convert.ToString(j["setlinfo"]["preselfpay_amt"]);	//	先行自付金额
                string inscp_scp_amt = Convert.ToString(j["setlinfo"]["inscp_scp_amt"]);	//	符合政策范围金额
                string act_pay_dedc = Convert.ToString(j["setlinfo"]["act_pay_dedc"]);	//	实际支付起付线
                string hifp_pay = Convert.ToString(j["setlinfo"]["hifp_pay"]);	//	基本医疗保险统筹基金支出
                string pool_prop_selfpay = Convert.ToString(j["setlinfo"]["pool_prop_selfpay"]);	//	基本医疗保险统筹基金支付比例
                string cvlserv_pay = Convert.ToString(j["setlinfo"]["cvlserv_pay"]);	//	公务员医疗补助资金支出
                string hifes_pay = Convert.ToString(j["setlinfo"]["hifes_pay"]);	//企业补充医疗保险基金支出
                string hifmi_pay = Convert.ToString(j["setlinfo"]["hifmi_pay"]);	//居民大病保险资金支出
                string hifob_pay = Convert.ToString(j["setlinfo"]["hifob_pay"]);	//职工大额医疗费用补助基金支出
                string maf_pay = Convert.ToString(j["setlinfo"]["maf_pay"]);	//医疗救助基金支出
                string oth_pay = Convert.ToString(j["setlinfo"]["oth_pay"]);	//	其他支出
                string fund_pay_sumamt = Convert.ToString(j["setlinfo"]["fund_pay_sumamt"]);	//	基金支付总额
                string psn_part_amt = Convert.ToString(j["setlinfo"]["psn_part_amt"]);	//个人负担总金额
                string acct_pay = Convert.ToString(j["setlinfo"]["acct_pay"]);	//	个人账户支出
                string psn_cash_pay = Convert.ToString(j["setlinfo"]["psn_cash_pay"]);	//	个人现金支出
                string hosp_part_amt = Convert.ToString(j["setlinfo"]["hosp_part_amt"]);	//	医院负担金额
                string balc = Convert.ToString(j["setlinfo"]["balc"]);	//	余额
                string acct_mulaid_pay = Convert.ToString(j["setlinfo"]["acct_mulaid_pay"]);	//	个人账户共济支付金额
                string medins_setl_id = Convert.ToString(j["setlinfo"]["medins_setl_id"]);	//	医药机构结算ID
                string clr_optins = Convert.ToString(j["setlinfo"]["clr_optins"]);	//	清算经办机构
                string clr_way = Convert.ToString(j["setlinfo"]["clr_way"]);	//	清算方式
                string clr_type = Convert.ToString(j["setlinfo"]["clr_type"]);	//	清算类别

                //总报销金额
                string zbxje = (Convert.ToDecimal(medfee_sumamt) - Convert.ToDecimal(psn_cash_pay)).ToString("0.00");
                //结算前账户余额
                string bcjsqzhye = (Convert.ToDecimal(acct_pay) + Convert.ToDecimal(balc)).ToString("0.00");
                //整合现金支付
                string zhxjzf = psn_cash_pay;
                //整合总报销金额
                string zhzbxje = zbxje;
                //其他医保费用
                string qtybfy = (Convert.ToDecimal(medfee_sumamt) - Convert.ToDecimal(psn_cash_pay) - Convert.ToDecimal(acct_pay) - Convert.ToDecimal(hifp_pay)).ToString("0.00");

                #region 医保类型
                string yblx = string.Empty;

                strSql = string.Format(@"select ylrylb from ybickxx where grbh='{0}'", grbh);
                ds.Tables.Clear();
                ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                if (ds.Tables[0].Rows.Count != 0)
                {
                    string[] sValue = { "11", "21", "31" };
                    if (sValue.Contains(ds.Tables[0].Rows[0]["ylrylb"].ToString()))
                        yblx += "(职工)";
                    else
                        yblx += "(居民)";
                }
                #endregion

                #region 结算基金分项信息
                //                List<string> liSql = new List<string>();
                //                JToken jts = Convert.ToString(ret["output"]["setldetail"]);
                //                foreach (JToken jt in jts)
                //                {
                //                    string fund_pay_type = Convert.ToString(jt["fund_pay_type"]);//	基金支付类型
                //                    string inscp_scp_amt1 = Convert.ToString(jt["inscp_scp_amt"]);//	符合政策范围金额
                //                    string crt_payb_lmt_amt = Convert.ToString(jt["crt_payb_lmt_amt"]);//	本次可支付限额金额
                //                    string fund_payamt = Convert.ToString(jt["fund_payamt"]);//	基金支付金额
                //                    string fund_pay_type_name = Convert.ToString(jt["fund_pay_type_name"]);//	基金支付类型名称
                //                    string setl_proc_info = Convert.ToString(jt["setl_proc_info"]);//	结算过程信息

                //                    strSql = string.Format(@"insert into ybfyjsmxdr(jzlsh,jylsh,jslsh,ybjzlsh,cxbz,sysdate,jbr,jjzflx,fhzcfwje,bckzfxeje,jjzfje,jjzflxmc,jsgcxx) values(
                //                                            '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}')",
                //                                            jzlsh, JYLSH, "0000", ybjzlsh, 1, sysdate, CliUtils.fLoginUser, fund_pay_type, inscp_scp_amt1, crt_payb_lmt_amt, fund_payamt, fund_pay_type_name, setl_proc_info);
                //                    liSql.Add(strSql);
                //                }
                #endregion
                #endregion

                #region 数据返回
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

                string strValue = medfee_sumamt + "|" + zbxje + "|" + hifp_pay + "|" + hifob_pay + "|" + acct_pay + "|" +
                                  psn_cash_pay + "|" + cvlserv_pay + "|" + hifes_pay + "|" + fulamt_ownpay_amt + "|" + "0.00" + "|" +
                                  hosp_part_amt + "|" + maf_pay + "|" + overlmt_selfpay + "|" + "0.00" + "|" + "0.00" + "|" +
                                  inscp_scp_amt + "|" + act_pay_dedc + "|" + "0.00" + "|" + "0.00" + "|" + "0.00" + "|" +
                                  "0.00" + "|" + "0.00" + "|" + "0.00" + "|" + "0.00" + "|" + "0.00" + "|" +
                                  bcjsqzhye + "|" + "0.00" + "|" + "0.00" + "|" + "0.00" + "|" + "0.00" + "|" +
                                  "0.00" + "|" + "0" + "|" + "0" + "|" + psn_name + "|" + setl_time + "|" +
                                  yllb + "||" + YBJGBH + "|" + YWZQH + "|" + "0000" + "|" +
                                  "|" + fph + "|" + clr_type + "|" + JYLSH + "|1|" +
                                  "|" + YBJGBH + "|0.00|||" +
                                  grbh + "|0.00|0.00|0.00|0.00|" +
                                  "0.00|0.00|0.00|" + "0.00" + "|" + "0.00" + "|" +
                                  yblx;
                WriteLog(sysdate + "  门诊费用预结算返回参数|" + strValue);
                return new object[] { 0, 1, strValue };
                #endregion
            }
            catch (Exception ex)
            {
                WriteLog(sysdate + "  进入门诊登记预收费异常|" + ex.Message);
                return new object[] { 0, 0, "进入门诊登记预收费异常|" + ex.Message };
            }
        }
        #endregion

        #region 门诊登记(挂号)收费
        public static object[] YBMZDJSF(object[] objParam)
        {
            string sysdate = GetServerDateTime();
            WriteLog(sysdate + "进入门诊登记收费...");
            WriteLog(sysdate + "  HIS入参|" + string.Join(",", objParam));
            try
            {
                #region 入参
                string jzlsh = objParam[0].ToString();      // 就诊流水号
                string yllb = objParam[1].ToString(); //医疗类别代码
                string dqrq = objParam[2].ToString();  // 结算时间 yyyy-mm-dd hh:mm:ss
                string bzbm = objParam[3].ToString(); //病种编码
                string bzmc = objParam[4].ToString(); //病种名称
                string kxx = objParam[5].ToString(); //读卡返回信息
                string dkbz = objParam[6].ToString();//读卡标志 
                string dgysdm = objParam[7].ToString(); //医生代码
                string dgysxm = objParam[8].ToString(); //医生姓名
                string ydbz = objParam[9].ToString(); //异地标志 (1-市本级 2-异地)
                string jsfs = "01"; //结算方式

                if (string.IsNullOrEmpty(jzlsh))
                    return new object[] { 0, 0, "就诊流水号不能为空" };
                if (string.IsNullOrEmpty(yllb))
                    return new object[] { 0, 0, "医疗类别代码不能为空" };
                if (string.IsNullOrEmpty(jsfs))
                    return new object[] { 0, 0, "结算方式不能为空" };
                #endregion

                #region 变量
                string jzlsh1 = ""; //医保就诊流水号
                string ghdjsj = Convert.ToDateTime(objParam[2]).ToString("yyyy-MM-dd HH:mm:ss");//挂号时间

                string[] syllb = { "14", "15" };
                if (syllb.Contains(yllb))
                    jzlsh1 = yllb + jzlsh + bzbm;
                else
                    jzlsh1 = yllb + jzlsh;
                #endregion

                #region 判断是否医保门诊登记
                string strSql = string.Format(@"select a.*,b.* from ybmzzydjdr a inner join ybickxx b on a.grbh=b.grbh where a.jzlsh1='{0}' and cxbz=1", jzlsh1);
                DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                if (ds.Tables[0].Rows.Count == 0)
                {
                    object[] objDJ = { jzlsh, yllb, bzbm, bzmc, kxx, dqrq };
                    objDJ = YBMZDJ(objDJ);
                    if (!objDJ[1].ToString().Equals("1"))
                        return objDJ;
                }
                ds.Tables.Clear();
                ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                string xm = ds.Tables[0].Rows[0]["xm"].ToString();
                string grbh = ds.Tables[0].Rows[0]["grbh"].ToString();
                string kh = ds.Tables[0].Rows[0]["kh"].ToString();
                string ybjzlsh = ds.Tables[0].Rows[0]["ybjzlsh"].ToString();
                string cbdybqh = ds.Tables[0].Rows[0]["tcqh"].ToString(); //参保地医保区划
                string mdtrt_cert_type = ds.Tables[0].Rows[0]["jzpzlx"].ToString(); //就诊凭证类型
                string mdtrt_cert_no = ds.Tables[0].Rows[0]["jzpzbh"].ToString();//就诊凭证编号
                string insutype = ds.Tables[0].Rows[0]["xzlx"].ToString();//
                #endregion

                #region 挂号费上传
                object[] objGHFYDJ = { jzlsh, ybjzlsh };
                objGHFYDJ = YBMZGHFDJ(objGHFYDJ);
                if (!objGHFYDJ[1].ToString().Equals("1"))
                    return objGHFYDJ;
                JYLSH = objGHFYDJ[2].ToString();
                string fph = objGHFYDJ[3].ToString();

                strSql = string.Format(@"select ybdjh,sum(je) ylfze,isnull(sum(zfje),0.00) zfje, isnull(sum(cxjzfje),0.00) as cxjje,isnull(sum(xxzfje),0.00) as xxzfje, isnull(sum(fhzcfwje),0.00) as fhzcfwje
                                        from ybcfmxscfhdr where jylsh='{0}' group by ybdjh ", JYLSH);
                ds.Tables.Clear();
                ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                if (ds.Tables[0].Rows.Count == 0)
                {
                    WriteLog(sysdate + "  获取上传费用明细失败!");
                    return new object[] { 0, 0, "获取上传费用明细失败！" };
                }
                DataRow dr = ds.Tables[0].Rows[0];
                string ylfze = dr["ylfze"].ToString();
                string zfje = dr["zfje"].ToString();
                string cxjje = dr["cxjje"].ToString();
                string xxzfje = dr["xxzfje"].ToString();
                string fhzcfwje = dr["fhzcfwje"].ToString();
                string sfpch = dr["ybdjh"].ToString();
                #endregion

                #region 结算入参
                dynamic data = new
                {
                    data = new
                    {
                        psn_no = grbh,
                        mdtrt_cert_type = mdtrt_cert_type,
                        mdtrt_cert_no = mdtrt_cert_no,
                        med_type = yllb,
                        medfee_sumamt = ylfze,
                        psn_setlway = jsfs,
                        mdtrt_id = ybjzlsh,
                        chrg_bchno = sfpch,
                        insutype = insutype,
                        acct_used_flag = "0",
                        invono = fph,
                        fulamt_ownpay_amt = zfje,
                        overlmt_selfpay = cxjje,
                        preselfpay_amt = xxzfje,
                        inscp_scp_amt = fhzcfwje
                    }
                };
                //2207
                #endregion
                string Err = string.Empty;

                #region 接口调用
                string inputData = JsonConvert.SerializeObject(data);
                WriteLog(sysdate + "  入参|" + inputData.ToString());
                int i = YBServiceRequest("2207", data, ref Err);
                WriteLog(sysdate + "  出参|" + Err.ToString());
                if (i != 1)
                {
                    WriteLog(sysdate + "  门诊登记(挂号)收费失败|" + Err.ToString());
                    return new object[] { 0, 0, "门诊登记(挂号)收费失败|" + Err.ToString() };
                }
                #endregion
                JObject j = JsonConvert.DeserializeObject<JObject>(Err);
                #region 数据处理
                //输出-结算信息
                string mdtrt_id = Convert.ToString(j["setlinfo"]["mdtrt_id"]);	//就诊ID
                string setl_id = Convert.ToString(j["setlinfo"]["setl_id"]);	//	结算ID
                string psn_no = Convert.ToString(j["setlinfo"]["psn_no"]);	//	人员编号
                string psn_name = Convert.ToString(j["setlinfo"]["psn_name"]);	//	人员姓名
                string psn_cert_type = Convert.ToString(j["setlinfo"]["psn_cert_type"]);	//	人员证件类型
                string certno = Convert.ToString(j["setlinfo"]["certno"]);	//	证件号码
                string gend = Convert.ToString(j["setlinfo"]["gend"]);	//	性别
                string naty = Convert.ToString(j["setlinfo"]["naty"]);	//	民族
                string brdy = Convert.ToString(j["setlinfo"]["brdy"]);	//	出生日期
                string age = Convert.ToString(j["setlinfo"]["age"]);	//	年龄
                string insutype1 = Convert.ToString(j["setlinfo"]["insutype1"]);	//	险种类型
                string psn_type = Convert.ToString(j["setlinfo"]["psn_type"]);	//	人员类别
                string cvlserv_flag = Convert.ToString(j["setlinfo"]["cvlserv_flag"]);	//	公务员标志
                string setl_time = Convert.ToString(j["setlinfo"]["setl_time"]);	//	结算时间
                string mdtrt_cert_type1 = Convert.ToString(j["setlinfo"]["mdtrt_cert_type"]);	//	就诊凭证类型
                string med_type = Convert.ToString(j["setlinfo"]["med_type"]);	//	医疗类别
                string medfee_sumamt = Convert.ToString(j["setlinfo"]["medfee_sumamt"]);	//	医疗费总额
                string fulamt_ownpay_amt = Convert.ToString(j["setlinfo"]["fulamt_ownpay_amt"]);	//	全自费金额
                string overlmt_selfpay = Convert.ToString(j["setlinfo"]["overlmt_selfpay"]);	//	超限价自费费用
                string preselfpay_amt = Convert.ToString(j["setlinfo"]["preselfpay_amt"]);	//	先行自付金额
                string inscp_scp_amt = Convert.ToString(j["setlinfo"]["inscp_scp_amt"]);	//	符合政策范围金额
                string act_pay_dedc = Convert.ToString(j["setlinfo"]["act_pay_dedc"]);	//	实际支付起付线
                string hifp_pay = Convert.ToString(j["setlinfo"]["hifp_pay"]);	//	基本医疗保险统筹基金支出
                string pool_prop_selfpay = Convert.ToString(j["setlinfo"]["pool_prop_selfpay"]);	//	基本医疗保险统筹基金支付比例
                string cvlserv_pay = Convert.ToString(j["setlinfo"]["cvlserv_pay"]);	//	公务员医疗补助资金支出
                string hifes_pay = Convert.ToString(j["setlinfo"]["hifes_pay"]);	//企业补充医疗保险基金支出
                string hifmi_pay = Convert.ToString(j["setlinfo"]["hifmi_pay"]);	//居民大病保险资金支出
                string hifob_pay = Convert.ToString(j["setlinfo"]["hifob_pay"]);	//职工大额医疗费用补助基金支出
                string maf_pay = Convert.ToString(j["setlinfo"]["maf_pay"]);	//医疗救助基金支出
                string oth_pay = Convert.ToString(j["setlinfo"]["oth_pay"]);	//	其他支出
                string fund_pay_sumamt = Convert.ToString(j["setlinfo"]["fund_pay_sumamt"]);	//	基金支付总额
                string psn_part_amt = Convert.ToString(j["setlinfo"]["psn_part_amt"]);	//个人负担总金额
                string acct_pay = Convert.ToString(j["setlinfo"]["acct_pay"]);	//	个人账户支出
                string psn_cash_pay = Convert.ToString(j["setlinfo"]["psn_cash_pay"]);	//	个人现金支出
                string hosp_part_amt = Convert.ToString(j["setlinfo"]["hosp_part_amt"]);	//	医院负担金额
                string balc = Convert.ToString(j["setlinfo"]["balc"]);	//	余额
                string acct_mulaid_pay = Convert.ToString(j["setlinfo"]["acct_mulaid_pay"]);	//	个人账户共济支付金额
                string medins_setl_id = Convert.ToString(j["setlinfo"]["medins_setl_id"]);	//	医药机构结算ID
                string clr_optins = Convert.ToString(j["setlinfo"]["clr_optins"]);	//	清算经办机构
                string clr_way = Convert.ToString(j["setlinfo"]["clr_way"]);	//	清算方式
                string clr_type = Convert.ToString(j["setlinfo"]["clr_type"]);	//	清算类别

                //总报销金额
                string zbxje = (Convert.ToDecimal(medfee_sumamt) - Convert.ToDecimal(psn_cash_pay)).ToString("0.00");
                //结算前账户余额
                string bcjsqzhye = (Convert.ToDecimal(acct_pay) + Convert.ToDecimal(balc)).ToString("0.00");
                //整合现金支付
                string zhxjzf = psn_cash_pay;
                //整合总报销金额
                string zhzbxje = zbxje;
                //其他医保费用
                string qtybfy = (Convert.ToDecimal(medfee_sumamt) - Convert.ToDecimal(psn_cash_pay) - Convert.ToDecimal(acct_pay) - Convert.ToDecimal(hifp_pay)).ToString("0.00");

                #region 医保类型
                string yblx = string.Empty;

                strSql = string.Format(@"select ylrylb from ybickxx where grbh='{0}'", grbh);
                ds.Tables.Clear();
                ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                if (ds.Tables[0].Rows.Count != 0)
                {
                    string[] sValue = { "11", "21", "31" };
                    if (sValue.Contains(ds.Tables[0].Rows[0]["ylrylb"].ToString()))
                        yblx += "(职工)";
                    else
                        yblx += "(居民)";
                }
                #endregion

                //结算基金分项信息

                List<string> liSql = new List<string>();
                JToken jts = Convert.ToString(j["setldetail"]);
                foreach (JToken jt in jts)
                {
                    string fund_pay_type = Convert.ToString(jt["fund_pay_type"]);//	基金支付类型
                    string inscp_scp_amt1 = Convert.ToString(jt["inscp_scp_amt"]);//	符合政策范围金额
                    string crt_payb_lmt_amt = Convert.ToString(jt["crt_payb_lmt_amt"]);//	本次可支付限额金额
                    string fund_payamt = Convert.ToString(jt["fund_payamt"]);//	基金支付金额
                    string fund_pay_type_name = Convert.ToString(jt["fund_pay_type_name"]);//	基金支付类型名称
                    string setl_proc_info = Convert.ToString(jt["setl_proc_info"]);//	结算过程信息

                    strSql = string.Format(@"insert into ybfyjsmxdr(jzlsh,jylsh,jslsh,ybjzlsh,cxbz,sysdate,jbr,jjzflx,fhzcfwje,bckzfxeje,jjzfje,jjzflxmc,jsgcxx) values(
                                            '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}')",
                                            jzlsh, JYLSH, setl_id, ybjzlsh, 1, sysdate, CliUtils.fLoginUser, fund_pay_type, inscp_scp_amt1, crt_payb_lmt_amt, fund_payamt, fund_pay_type_name, setl_proc_info);
                    liSql.Add(strSql);
                }


                #endregion

                #region 数据返回
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

                string strValue = medfee_sumamt + "|" + zbxje + "|" + hifp_pay + "|" + hifob_pay + "|" + acct_pay + "|" +
                                  psn_cash_pay + "|" + cvlserv_pay + "|" + hifes_pay + "|" + fulamt_ownpay_amt + "|" + "0.00" + "|" +
                                  hosp_part_amt + "|" + maf_pay + "|" + overlmt_selfpay + "|" + "0.00" + "|" + "0.00" + "|" +
                                  inscp_scp_amt + "|" + act_pay_dedc + "|" + "0.00" + "|" + "0.00" + "|" + "0.00" + "|" +
                                  "0.00" + "|" + "0.00" + "|" + "0.00" + "|" + "0.00" + "|" + "0.00" + "|" +
                                  bcjsqzhye + "|" + "0.00" + "|" + "0.00" + "|" + "0.00" + "|" + "0.00" + "|" +
                                  "0.00" + "|" + "0" + "|" + "0" + "|" + psn_name + "|" + setl_time + "|" +
                                  yllb + "||" + YBJGBH + "|" + YWZQH + "|" + setl_id + "|" +
                                  "|" + fph + "|" + clr_type + "|" + JYLSH + "|1|" +
                                  "|" + YBJGBH + "|0.00|||" +
                                  grbh + "|0.00|0.00|0.00|0.00|" +
                                  "0.00|0.00|0.00|" + "0.00" + "|" + "0.00" + "|" +
                                  yblx;
                WriteLog(sysdate + "  门诊费用结算返回参数|" + strValue);

                strSql = string.Format(@"insert into ybfyjsdr(jzlsh,djhin,djh,jylsh,yblx,cxbz,sysdate,jbr,kh,bzbm,
                                        bzmc,ztjsbz,tcqh,qtybfy,zbxje,zhzbxje,zhxjzffy,ybjzlsh,grbh,xm,
                                        ryzjlx,zjhm,xb,mz,csrq,nl,xzlx,yldylb,gwybz,jsrq,
                                        jzpzlx,yllb,ylfze,zffy,cxjfy,xxzfje,fhjbylfy,qfbzfy,tcjjzf,jbylbxtcjjzfbl,
                                        gwybzjjzf,qybcylbxjjzf,dbjjzf,dejjzf,mzjzfy,qtjjzf,jjzfze,grfdzje,zhzf,xjzf,
                                        yyfdfy,bcjsqzhye,grzhgjzfje,jbjgbm,jsfs,jylx,ywzqh,zhsybz,jslsh,cfmxjylsh) values( 
                                        '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}',
                                        '{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}',
                                        '{20}','{21}','{22}','{23}','{24}','{25}','{26}','{27}','{28}','{29}',
                                        '{30}','{31}','{32}','{33}','{34}','{35}','{36}','{37}','{38}','{39}',
                                        '{40}','{41}','{42}','{43}','{44}','{45}','{46}','{47}','{48}','{49}',
                                        '{50}','{51}','{52}','{53}','{54}','{55}','{56}','{57}','{58}','{59}')",
                                       jzlsh, fph, medins_setl_id, JYLSH, yblx, 1, sysdate, CliUtils.fLoginUser, kh, bzbm,
                                       bzmc, 0, cbdybqh, qtybfy, zbxje, zhzbxje, zhxjzf, ybjzlsh, psn_no, psn_name,
                                       psn_cert_type, certno, gend, naty, brdy, age, insutype1, psn_type, cvlserv_flag, setl_time,
                                       mdtrt_cert_type1, med_type, medfee_sumamt, fulamt_ownpay_amt, overlmt_selfpay, preselfpay_amt, inscp_scp_amt, act_pay_dedc, hifp_pay, pool_prop_selfpay,
                                       cvlserv_pay, hifes_pay, hifmi_pay, hifob_pay, maf_pay, oth_pay, fund_pay_sumamt, psn_part_amt, acct_pay, psn_cash_pay,
                                       hosp_part_amt, bcjsqzhye, acct_mulaid_pay, clr_optins, clr_way, clr_type, YWZQH, 1, setl_id, JYLSH);
                liSql.Add(strSql);

                object[] obj = liSql.ToArray();
                obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);
                if (obj[1].ToString().Equals("1"))
                {
                    WriteLog(sysdate + "  门诊登记(挂号)收费成功|");
                    return new object[] { 0, 1, strValue };
                }
                else
                {
                    WriteLog(sysdate + "  门诊登记(挂号)收费失败|数据操作失败|" + obj[2].ToString());
                    //门诊费用结算撤销(内部)
                    object[] objJSCX = { setl_id, mdtrt_id, psn_no, cbdybqh };
                    N_YBMZFYJSCX(objJSCX);
                    return new object[] { 0, 0, "门诊登记(挂号)收费失败|数据操作失败|" + obj[2].ToString() };
                }

                #endregion
            }
            catch (Exception ex)
            {
                WriteLog(sysdate + "  进入门诊登记(挂号)收费异常|" + ex.Message);
                return new object[] { 0, 0, "进入门诊登记(挂号)收费异常|" + ex.Message };
            }

        }
        #endregion

        #region 门诊登记(挂号)收费撤销
        public static object[] YBMZDJSFCX(object[] objParam)
        {
            string sysdate = GetServerDateTime();
            WriteLog(sysdate + "进入门诊登记(挂号)收费撤销...");
            WriteLog(sysdate + "  HIS入参|" + string.Join(",", objParam));
            try
            {
                #region HIS入参
                string jzlsh = objParam[0].ToString();
                string djh = string.Empty;

                if (string.IsNullOrEmpty(jzlsh))
                    return new object[] { 0, 0, "就诊流水号不能为空" };
                #endregion

                string strSql = string.Format(@"select a.jzlsh,a.ybjzlsh,b.m1invo from ybmzzydjdr a inner join mz01h b on a.jzlsh=b.m1ghno where a.jzlsh='{0}' and cxbz=1 and jzbz='m'", jzlsh);
                DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                if (ds.Tables[0].Rows.Count == 0)
                    return new object[] { 0, 0, "该患者未做医保登记" };
                djh = ds.Tables[0].Rows[0]["m1invo"].ToString();
                string ybjzlsh = ds.Tables[0].Rows[0]["ybjzlsh"].ToString();
                //判断是否结算
                strSql = string.Format(@"select * from ybfyjsdr where jzlsh='{0}' and djhin='{1}' and cxbz=1", jzlsh, djh);
                ds.Tables[0].Clear();
                ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);


                if (ds.Tables[0].Rows.Count == 0)
                {
                    //门诊
                    //只做门诊登记（挂号）撤销
                    object[] objMZDJCX = new object[] { jzlsh };
                    return YBMZDJCX(objMZDJCX);
                }
                else
                {
                    //进行门诊登记（挂号）收费，需撤销费用后再撤销登记
                    object[] objParam1 = new object[] { jzlsh, djh };
                    object[] objReturn = YBMZSFJSCX(objParam1);
                    if (objReturn[1].ToString().Equals("1"))
                    {

                        //撤销门诊登记（挂号）
                        return YBMZDJCX(objParam1);
                    }
                    else
                        return objReturn;
                }
            }
            catch (Exception ex)
            {
                WriteLog(sysdate + "  进入门诊登记(挂号)收费撤销异常|" + ex.Message);
                return new object[] { 0, 0, "进入门诊登记(挂号)收费撤销异常|" + ex.Message };
            }
        }
        #endregion

        #region 门诊挂号费用明细上传撤销（内部）
        /// <summary>
        ///  门诊费用明细上传撤销（新医保）
        /// </summary>
        /// <param name="jzid">就诊id</param>
        /// <param name="sfpch">收费批次号</param>
        /// <param name="psnNo">人员编号</param>
        /// <returns></returns>
        private static object[] NYBMZGHFYMXSCCX(object[] objParam)
        {
            string sysdate = GetServerDateTime();
            WriteLog(sysdate + "  进入门诊费用登记撤销...");
            WriteLog(sysdate + " HIS入参|" + string.Join(",", objParam));
            try
            {
                string jzlsh = objParam[0].ToString();
                string ybjzlsh = objParam[1].ToString();

                if (string.IsNullOrEmpty(jzlsh))
                    return new object[] { 0, 0, "就诊流水号不能为空" };
                DataSet ds = new DataSet();

                #region 获取费用登记信息
                string strSql = string.Format(@"select * from ybmzzydjdr where ybjzlsh='{0}' and cxbz=1", ybjzlsh);
                ds.Tables.Clear();
                ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                string grbh = ds.Tables[0].Rows[0]["grbh"].ToString();
                string cbdybqh = ds.Tables[0].Rows[0]["cbdybqh"].ToString();
                #endregion

                #region 入参赋值
                dynamic data = new
                {
                    data = new
                    {

                        mdtrt_id = ybjzlsh,
                        chrg_bchno = "0000",
                        psn_no = grbh
                    }
                };
                #endregion
                string Err = string.Empty;
                #region 接口调用
                string inputData = JsonConvert.SerializeObject(data);
                WriteLog(sysdate + "  入参|" + inputData.ToString());
                int i = YBServiceRequest("2205", data, ref Err);
                WriteLog(sysdate + "  出参|" + Err.ToString());

                if (i != 1)
                {
                    WriteLog(sysdate + "  门诊挂号费用登记撤销失败|" + Err.ToString());
                    return new object[] { 0, 0, "门诊挂号费用登记撤销失败|" + Err.ToString() };
                }

                List<string> liSQL = new List<string>();
                strSql = string.Format(@"delete from ybcfmxscindr where ybjzlsh='{0}'  and cxbz=1", ybjzlsh);
                liSQL.Add(strSql);
                strSql = string.Format(@"delete from ybcfmxscfhdr where ybjzlsh='{0}' and cxbz=1", ybjzlsh);
                liSQL.Add(strSql);

                object[] obj = liSQL.ToArray();
                obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);
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
                #endregion
            }
            catch (Exception ex)
            {
                WriteLog(sysdate + "  门诊挂号费用登记撤销异常" + ex.Message);
                return new object[] { 0, 0, "门诊挂号费用登记撤销异常" + ex.Message};
            }
        }

        #endregion

        #region 门诊费用登记撤销(门诊费用明细信息上传撤销)
        public static object[] YBMZCFMXSCCX(object[] objParam)
        {
            string sysdate = GetServerDateTime();
            WriteLog(sysdate + "  进入门诊费用登记撤销...");
            WriteLog(sysdate + " HIS入参|" + string.Join(",", objParam));
            try
            {
                string jzlsh = objParam[0].ToString();
                string ybjzlsh = objParam[1].ToString();

                if (string.IsNullOrEmpty(jzlsh))
                    return new object[] { 0, 0, "就诊流水号不能为空" };


                #region 获取费用登记信息
                string strSql = string.Format(@"select distinct jylsh,ybjzlsh from ybcfmxscindr where ybjzlsh='{0}' and cxbz=1 ", ybjzlsh);//and jylsh not in(select isnull(cfmxjylsh,'') as cfjylsh from ybfyjsdr where cxbz=1 and ybjzlsh='{0}')
                DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                if (ds.Tables[0].Rows.Count == 0)
                {
                    WriteLog(sysdate + "  该患者未上传费用信息或已完成结算");
                    return new object[] { 0, 0, "该患者未上传费用信息或已完成结算" };
                }
                string cfjylsh = ds.Tables[0].Rows[0]["jylsh"].ToString();

                strSql = string.Format(@"select * from ybmzzydjdr where ybjzlsh='{0}' and cxbz=1", ybjzlsh);
                ds.Tables.Clear();
                ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                string grbh = ds.Tables[0].Rows[0]["grbh"].ToString();
                string cbdybqh = ds.Tables[0].Rows[0]["cbdybqh"].ToString();
                #endregion

                #region 入参赋值
                dynamic data = new
                {
                    data = new
                    {
                        mdtrt_id = ybjzlsh,
                        chrg_bchno = "0000",
                        psn_no = grbh
                    }
                };

                #endregion
                string Err = string.Empty;

                #region 接口调用
                string inputData = JsonConvert.SerializeObject(data);
                WriteLog(sysdate + "  2205入参|" + inputData.ToString());
                int i = YBServiceRequest("2205", data, ref Err);
                WriteLog(sysdate + "  2205出参|" + Err.ToString());

                if (i != 1)
                {
                    WriteLog(sysdate + "  门诊费用登记撤销失败|" + Err.ToString());
                    return new object[] { 0, 0, "门诊费用登记撤销失败|" + Err.ToString() };
                }

                List<string> liSQL = new List<string>();
                strSql = string.Format(@"delete from ybcfmxscindr where ybjzlsh='{0}' and jylsh='{1}' and cxbz=1", ybjzlsh, cfjylsh);
                liSQL.Add(strSql);
                strSql = string.Format(@"delete from ybcfmxscfhdr where ybjzlsh='{0}' and jylsh='{1}' and cxbz=1", ybjzlsh, cfjylsh);
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
                #endregion
            }
            catch (Exception ex)
            {
                WriteLog(sysdate + "  门诊费用登记撤销异常" + ex.Message);
                return new object[] { 0, 0, "门诊费用登记撤销异常" + ex.Message };
            }
        }
        #endregion

        #region 门诊登记撤销(内部)
        public static object[] N_YBMZDJCX(object[] objParam)
        {
            string sysdate = GetServerDateTime();
            WriteLog(sysdate + "  进入门诊登记撤销(内部)...");
            WriteLog(sysdate + "  HIS入参|" + string.Join(",", objParam));
            try
            {
                #region HIS入参
                string grbh = objParam[0].ToString();
                string ybjzlsh = objParam[1].ToString();
                string jzlsh = objParam[2].ToString();
                string cbdybqh = objParam[3].ToString();
                #endregion

                #region 变量 
                CZYBH = CliUtils.fLoginUser;
                string jbr = CliUtils.fUserName;
                #endregion

                #region 入参
                dynamic input = new
                {
                    data = new
                    {
                        psn_no = grbh,
                        mdtrt_id = ybjzlsh,
                        ipt_otp_no = jzlsh
                    }
                };


                #endregion
                string Err = string.Empty;
                #region 接口调用
                string inputData = JsonConvert.SerializeObject(input);
                WriteLog(sysdate + "  入参|" + inputData.ToString());
                int i = YBServiceRequest("2202", input, ref Err);
                WriteLog(sysdate + "  出参|" + Err.ToString());
                #endregion

                #region 数据处理
                if (i <= 0)
                {
                    WriteLog(sysdate + "  医保门诊登记撤销(内部)失败|" + Err.ToString());
                    return new object[] { 0, 0, Err.ToString() };
                }
                else
                {
                    WriteLog(sysdate + "  医保门诊登记撤销(内部)成功");
                    return new object[] { 0, 1, "医保门诊登记撤销成功(内部)" };
                }
                #endregion
            }
            catch (Exception ex)
            {
                WriteLog(sysdate + "  医保门诊登记撤销(内部)异常|" + ex.Message);
                return new object[] { 0, 0, "医保门诊登记撤销(内部)异常|" + ex.Message };
            }
        }
        #endregion

        #region 门诊费用登记撤销(内部)
        public static object[] N_YBMZSFSCCX(object[] objParam)
        {
            string sysdate = GetServerDateTime();
            WriteLog(sysdate + "  进入门诊费用登记撤销(内部)...");
            WriteLog(sysdate + "  HIS入参|" + string.Join(",", objParam));

            try
            {
                #region 入参
                string ybjzlsh = objParam[0].ToString(); //就诊 ID
                string grbh = objParam[1].ToString(); //人员编号
                string cbdybqh = objParam[2].ToString(); //参保地医保区划
                #endregion

                #region 入参赋值
                dynamic data = new
                {
                    data = new
                    {
                        mdtrt_id = ybjzlsh,
                        chrg_bchno = "0000",
                        psn_no = grbh
                    }
                };


                #endregion
                string Err = string.Empty;
                #region 接口调用
                string inputData = JsonConvert.SerializeObject(data);
                WriteLog(sysdate + "  入参|" + inputData.ToString());
                int i = YBServiceRequest("2205", data, ref Err);
                WriteLog(sysdate + "  出参|" + Err.ToString());

                if (i > 0)
                {
                    WriteLog(sysdate + "  门诊费用登记撤销(内部)成功|");
                    return new object[] { 0, 1, "门诊费用登记撤销(内部)成功" };
                }
                else
                {
                    WriteLog(sysdate + "  门诊费用登记撤销(内部)失败|" + Err.ToString());
                    return new object[] { 0, 0, Err.ToString() };
                }
                #endregion
            }
            catch (Exception ex)
            {
                WriteLog(sysdate + "  进入门诊费用登记撤销(内部)异常|" + ex.Message);
                return new object[] { 0, 0, ex.Message };
            }
        }
        #endregion

        #region 门诊费用结算撤销(内部)
        public static object[] N_YBMZFYJSCX(object[] objParam)
        {
            string sysdate = GetServerDateTime();//当前时间
            WriteLog(sysdate + "  进入门诊费用结算撤销(内部)...");
            WriteLog(sysdate + "  HIS入参|" + string.Join(",", objParam));
            try
            {
                string jslsh = objParam[0].ToString();
                string ybjzlsh = objParam[1].ToString();
                string grbh = objParam[2].ToString();
                string cbdybqh = objParam[3].ToString();

                #region 入参
                dynamic data = new
                {
                    data = new
                    {

                        setl_id = jslsh,
                        mdtrt_id = ybjzlsh,
                        psn_no = grbh
                    }
                };


                #endregion
                string Err = string.Empty;

                #region 接口调用
                string inputData = JsonConvert.SerializeObject(data);
                WriteLog(sysdate + "  入参|" + inputData.ToString());
                int i = YBServiceRequest("2208", data, ref Err);
                WriteLog(sysdate + "  出参|" + Err.ToString());
                if (i != 1)
                {
                    WriteLog(sysdate + "  门诊费用结算撤销(内部)失败|" + Err.ToString());
                    ////处方上传撤销
                    //object[] objFYSCCX = { ybjzlsh, grbh, cbdybqh };
                    //N_YBMZSFDJCX(objFYSCCX);
                    return new object[] { 0, 0, "门诊费用结算撤销(内部)失败|" + Err.ToString() };
                }
                else
                {
                    WriteLog(sysdate + "  门诊费用结算撤销(内部)成功|");
                    return new object[] { 0, 0, "门诊费用结算撤销(内部)成功|" };
                }
                #endregion
            }
            catch (Exception ex)
            {
                WriteLog(sysdate + "  门诊费用结算撤销(内部)异常|" + ex.ToString());
                return new object[] { 0, 0, "门诊费用结算撤销(内部)异常|" + ex.ToString() };
            }

        }
        #endregion


        #region 门诊费用明细上传(新医保)
        //private static DataSet dsinfo = new DataSet();
        private static List<OutpatientFee> feelists = new List<OutpatientFee>();
        private static DataSet datasetdetail = new DataSet();
        private static object[] YBMZFYMXSC(object[] outParam)
        {
            string sysdate = GetServerDateTime();
            WriteLog(sysdate + "进入门诊费用明细上传...");
            WriteLog(sysdate + "  HIS入参|" + string.Join(",", outParam));
            try
            {
                #region 入参
                string jzlsh = outParam[0].ToString(); //流水号不能为空
                string cfhs = outParam[1].ToString();   //处方号
                string ylhjfy = outParam[2].ToString(); //医疗合计费用 (新增)
                string sfymm = outParam[3].ToString(); //医保卡密码 （新增）
                string kbrq1 = outParam[4].ToString(); //看病日期 
                string ybjzlsh = outParam[5].ToString();

                if (string.IsNullOrEmpty(jzlsh))
                    return new object[] { 0, 0, "就诊流水号不能为空！" };
                if (string.IsNullOrEmpty(cfhs))
                    return new object[] { 0, 0, "处方号不能为空！" };
                if (string.IsNullOrEmpty(ylhjfy))
                    return new object[] { 0, 0, "医疗合计费不能为空" };
                if (string.IsNullOrEmpty(kbrq1))
                    return new object[] { 0, 0, "看病日期不能为空,时间格式为:yyyy-MM-dd HH:mm:ss" };
                #endregion

                #region 变量
                double sfje = 0.0000; //收费金额
                #region 金额有效性
                try
                {
                    sfje = Convert.ToDouble(ylhjfy);
                }
                catch
                {
                    return new object[] { 0, 0, "收费金额格式错误" };
                }
                #endregion
                CZYBH = CliUtils.fLoginUser;

                #endregion

                #region 获取医保登记信息
                string strSql = string.Format("select * from ybmzzydjdr a where a.ybjzlsh = '{0}' and a.cxbz = 1", ybjzlsh);
                DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                if (ds == null || ds.Tables[0].Rows.Count == 0)
                    return new object[] { 0, 0, "该患者未医保门诊登记" };
                string grbh = ds.Tables[0].Rows[0]["grbh"].ToString(); //个人编码
                string xm = ds.Tables[0].Rows[0]["xm"].ToString();  //姓名
                string kh = ds.Tables[0].Rows[0]["kh"].ToString();  //卡号
                string bzbm = ds.Tables[0].Rows[0]["bzbm"].ToString(); //病种编码
                string cbdybqh = ds.Tables[0].Rows[0]["tcqh"].ToString();
                string ysbm = string.IsNullOrEmpty(ds.Tables[0].Rows[0]["dgysdm"].ToString()) ? ds.Tables[0].Rows[0]["ysdm"].ToString() : ds.Tables[0].Rows[0]["dgysdm"].ToString();
                string ysxm = string.IsNullOrEmpty(ds.Tables[0].Rows[0]["dgysxm"].ToString()) ? ds.Tables[0].Rows[0]["ysxm"].ToString() : ds.Tables[0].Rows[0]["dgysxm"].ToString();
                JYLSH = ybjzlsh + DateTime.Now.ToString("HHmmssfff");
                insuplc_admdvs = cbdybqh;
                #endregion

                #region 费用上传前，撤销所有未结算费用
                object[] objGHFYDJ = { jzlsh, ybjzlsh };
                //费用登记前，撤销费用登记
                YBMZCFMXSCCX(objGHFYDJ);
                #endregion

                #region 获取处方信息
                strSql = string.Format(@"select m.cfh,m.cfrq,isnull(x.ybxmbh,'') ybxmbh,x.ybxmmc,m.yyxmbh,m.yyxmmc,sum(m.sl) sl, sum(m.dj*m.sl) je,m.dj,e.ybksdm,e.ybksmc,f.dgysbm,f.ysxm,x.sfxmzldm,x.sflbdm,x.cysm,x.clsm from (
                                        --药品
                                        select mcunit as dw,mcwayx as yf,mcusex as yl,y1ggxx as gg,y1jxxx as jx,mcdate as cfrq,y1sflb as sfno,a.mccfno cfh,a.mcseq2 cflsh,a.mcypno yyxmbh, a.mcypnm yyxmmc,convert(decimal(8,4),(a.mcamnt/(a.mcquty/b.y1blfs))) as dj,convert(decimal(8,4),(a.mcquty/b.y1blfs)) as sl, a.mcamnt je, a.mcksno ksno, a.mcuser ysdm, 
                                        case when mcsflb='03' then cast((a.mcquty/a.mcusex) as int) else 0 end as fs,'0' as sfbz,
                                        case when left(mccfno,1)='W' then '2' else '1' end as sfxmzl
                                        from mzcfd a 
                                        left join yp01h b on a.mcypno=b.y1ypno
                                        where a.mcghno = '{0}' and a.mccfno in ({1})
                                        union all
                                        --检查/治疗 
                                        select '' as dw, '' as yf,'' as yl,'' as gg,'' as jx,mbdate as cfrq,mbsfno as sfno,b.mbzlno cfh,b.mbsequ cflsh, b.mbitem yyxmbh, b.mbname yyxmmc, b.mbpric dj, b.mbquty sl, b.mbsjam je, b.mbksno ksno, b.mbuser ysdm, 0 as fs,'0' as sfbz,
                                        '2' as sfxmzl
                                        from mzb2d b where b.mbpric>0 and b.mbghno = '{0}' and b.mbzlno in ({1})
                                        union all
                                        --检验
                                        select '' as dw, '' as yf,'' as yl,'' as gg,'' as jx,mbdate as cfrq,mbsfno as sfno,c.mbzlno cfh,c.mbsequ cflsh, c.mbitem yyxmbh, c.mbname yyxmmc, c.mbpric dj, c.mbquty sl, c.mbsjam je, c.mbksno ksno, c.mbuser ysdm, 0 as fs,'0' as sfbz,'2' as sfxmzl
                                        from mzb4d c where c.mbghno = '{0}' and c.mbzlno in ({1})
                                        union all
                                        --躺椅费
                                        select '' as dw, '' as yf,'' as yl,'' as gg,'' as jx,mddate as cfrq,y.b5sfno as sfno,d.mdzsno as cfh,'' cflsh,y.b5item yyxmbh, y.b5name yyxmmc, b5sfam dj, d.mddays sl, (b5sfam*d.mddays) je, d.mdzsks ksno, d.mdempn ysdm, 0 as fs,'0' as sfbz,'2' as sfxmzl
                                        from mzd3d d 
                                        left join (select distinct mccfno, mcghno from mzcfd) mzcf on mccfno = mdcfno
                                        left join bz09d on b9mbno = mdtwid 
                                        left join bz05d y on b5item = b9item 
                                        where mdtiwe > 0 and d.mdzsno in ({1})
                                        union all
                                        --注射费
                                        select '' as dw, '' as yf,'' as yl,'' as gg,'' as jx,mddate as cfrq,y.b5sfno as sfno,d.mdzsno as cfh,'' cflsh,y.b5item yyxmbh, y.b5name yyxmmc, b5sfam dj, d.mdtims sl, (b5sfam*d.mdtims) je, d.mdzsks ksno, d.mdempn ysdm, 0 as fs,'0' as sfbz,'2' as sfxmzl
                                        from mzd3d d 
                                        left join bz09d on b9mbno = mdwayid 
                                        left join bz05d y on b5item = b9item
                                        left join (select distinct mccfno, mcghno from mzcfd) mzcf on mccfno = mdcfno 
                                        where mdzsno in ({1})
                                        union all
                                        --皮试费
                                        select '' as dw, '' as yf,'' as yl,'' as gg,'' as jx,mddate as cfrq,y.b5sfno as sfno,d.mdzsno as cfh,'' cflsh,y.b5item yyxmbh, y.b5name yyxmmc, b5sfam dj, d.mdpqty sl, (b5sfam*d.mdpqty) je, d.mdzsks ksno, d.mdempn ysdm, 0 as fs,'0' as sfbz,'2' as sfxmzl
                                        from mzd3d d 
                                        left join bz09d on b9mbno = mdpprid 
                                        left join bz05d y on b5item = b9item
                                        left join (select distinct mccfno, mcghno from mzcfd) mzcf on mccfno = mdcfno
                                        where mdpqty > 0 and mdppri>0 and d.mdzsno in ({1})
                                        ) m 
                                        left join ybhisdzdrnew x on m.yyxmbh = x.hisxmbh
                                        left join ybkszd e on m.ksno=e.ksdm
                                        left join ybdgyszd f on m.ysdm=f.ysbm
                                        group by  m.cfh,m.cfrq,x.ybxmbh,x.ybxmmc,m.yyxmbh,m.yyxmmc,m.dj,e.ybksdm,e.ybksmc,f.dgysbm,f.ysxm,x.sfxmzldm,x.sflbdm,x.cysm,x.clsm", jzlsh, cfhs);
                ds.Tables.Clear();
                ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                if (ds.Tables[0].Rows.Count == 0)
                    return new object[] { 0, 0, "无处方信息" };
                List<string> li_Param = new List<string>();
                string dzwxm = "";//对照外项目
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    if (string.IsNullOrEmpty(dr["ybxmbh"].ToString()))
                        dzwxm += dr["yyxmbh"].ToString() + "|" + dr["yyxmmc"].ToString() + "&";
                }
                if (dzwxm != "")
                {
                    dzwxm += "未对照";
                    return new object[] { 0, 0, dzwxm };
                }
                #endregion

                #region 入参处理
                #region 变量定义
                string feedetl_sn = "";// 费用明细流水号
                string mdtrt_id = ybjzlsh;// 就诊 ID
                string psn_no = grbh;// 人员编号
                string chrg_bchno = DateTime.Now.ToString("yyyyMMddHHmmss");// 收费批次号
                string dise_codg = bzbm;// 病种编码
                string rxno = "";// 处方号
                string rx_circ_flag = "0";// 外购处方标志
                string fee_ocur_time = "";// 费用发生时间
                string med_list_codg = "";// 医疗目录编码
                string medins_list_codg = "";// 医药机构目录编码
                string det_item_fee_sumamt = "";// 明细项目费用总额
                string cnt = "";// 数量
                string pric = "";// 单价
                string sin_dos_dscr = "";// 单次剂量描述
                string used_frqu_dscr = "";// 使用频次描述
                string prd_days = "";// 周期天数
                string medc_way_sdcr = "";// 用药途径描述
                string bilg_dept_codg = "";// 开单科室编码
                string bilg_dept_name = "";
                string bilg_dr_codg = "";// 开单医生编码
                string bilg_dr_name = "";// 开单医师姓名
                string acord_dept_codg = "";// 受单科室编码
                string acord_dept_name = "";// 受单科室名称
                string orders_dr_code = "";// 受单医生编码
                string orders_dr_name = "";// 受单医生姓名
                string hosp_appr_flag = "1";// 医院审批标志
                string tcmdrug_used_way = "";// 中药使用方式
                string etip_flag = "";// 外检标志
                string etip_hosp_code = "";// 外检医院编码
                string dscg_tkdrug_flag = "";// 出院带药标志
                string matn_fee_flag = "";// 生育费用标志
                string yyxmmc = ""; //医院项目名称
                string ybxmmc = ""; //医保项目名称
                string sfxmzl = ""; //收费项目种类
                string sflbdm = ""; //收费类别
                string cysm = "";   //草药省码
                string clsm = "";   //材料省码
                double scfy = 0.00; //处方金额累计
                #endregion

                List<string> liSQL = new List<string>();
                List<string> liyyxmbh = new List<string>();
                List<string> liyyxmmc = new List<string>();
                List<string> liybxmbm = new List<string>();
                List<string> liybxmmc = new List<string>();
                List<string> liybsflb = new List<string>();
                List<dynamic> feedetail = new List<dynamic>();
                int index = 1;
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    #region 入参赋值
                    feedetl_sn = Convert.ToString(dr["cfh"]) + index.ToString();
                    rxno = Convert.ToString(dr["cfh"]);
                    fee_ocur_time = Convert.ToString(dr["cfrq"]);
                    med_list_codg = Convert.ToString(dr["ybxmbh"]);
                    ybxmmc = Convert.ToString(dr["ybxmmc"]);
                    medins_list_codg = Convert.ToString(dr["yyxmbh"]);
                    yyxmmc = Convert.ToString(dr["yyxmmc"]);
                    det_item_fee_sumamt = Convert.ToString(dr["je"]);
                    cnt = Convert.ToString(dr["sl"]);
                    pric = Convert.ToString(dr["dj"]);
                    string ybksdm = "D99";
                    string ybksmc = "其它";
                    if (!string.IsNullOrEmpty(dr["ybksdm"].ToString()))
                    {
                        ybksdm = Convert.ToString(dr["ybksdm"]);
                        ybksmc = Convert.ToString(dr["ybksmc"]);
                    }
                    bilg_dept_codg = ybksdm;
                    bilg_dept_name = ybksmc;
                    bilg_dr_codg = string.IsNullOrEmpty(Convert.ToString(dr["dgysbm"])) ? ysbm : Convert.ToString(dr["dgysbm"]);
                    bilg_dr_name = string.IsNullOrEmpty(Convert.ToString(dr["ysxm"])) ? ysxm : Convert.ToString(dr["ysxm"]);
                    sfxmzl = Convert.ToString(dr["sfxmzldm"]);
                    sflbdm = Convert.ToString(dr["sflbdm"]);
                    cysm = Convert.ToString(dr["cysm"]);
                    clsm = Convert.ToString(dr["clsm"]);
                    scfy += Convert.ToDouble(det_item_fee_sumamt);
                    liyyxmbh.Add(medins_list_codg);
                    liyyxmmc.Add(yyxmmc);
                    liybxmbm.Add(med_list_codg);
                    liybxmmc.Add(ybxmmc);
                    liybsflb.Add(sflbdm);
                    index++;
                    string seqNo = DateTime.Now.ToString("yyyyMMddHHmmssfff").Substring(12, 5);
                    dynamic feed_kf = new
                    {
                        feedetl_sn = feedetl_sn + seqNo,
                        mdtrt_id = mdtrt_id,
                        psn_no = psn_no,
                        chrg_bchno = chrg_bchno,
                        dise_codg = dise_codg,
                        rxno = rxno,
                        rx_circ_flag = rx_circ_flag,
                        fee_ocur_time = fee_ocur_time,
                        med_list_codg = med_list_codg,
                        medins_list_codg = medins_list_codg,
                        det_item_fee_sumamt = det_item_fee_sumamt,
                        cnt = cnt,
                        pric = pric,
                        sin_dos_dscr = sin_dos_dscr,
                        used_frqu_dscr = used_frqu_dscr,
                        prd_days = prd_days,
                        medc_way_sdcr = medc_way_sdcr,
                        bilg_dept_codg = bilg_dept_codg,
                        bilg_dept_name = bilg_dept_name,
                        bilg_dr_codg = bilg_dr_codg,
                        bilg_dr_name = bilg_dr_name,
                        acord_dept_codg = acord_dept_codg,
                        acord_dept_name = acord_dept_name,
                        orders_dr_code = orders_dr_code,
                        orders_dr_name = orders_dr_name,
                        hosp_appr_flag = hosp_appr_flag,
                        tcmdrug_used_way = tcmdrug_used_way,
                        etip_flag = etip_flag,
                        etip_hosp_code = etip_hosp_code,
                        dscg_tkdrug_flag = dscg_tkdrug_flag,
                        matn_fee_flag = matn_fee_flag,
                        expContent = new
                        {
                            mcs_prov_code = clsm,
                            tcmherb_prov_code = cysm,
                        }
                    };
                    feedetail.Add(feed_kf);
                    #endregion

                    #region 数据处理
                    strSql = string.Format(@"insert into ybcfmxscindr(jzlsh,jylsh,sfxmzl,ybcfh,djlsh,cfrq,yysfxmbm,yysfxmmc,dj,sl,
						                 je,ysbm,ksbh,jbr,sflb,grbh,xm,kh,cxbz,sysdate,
                                         ybjzlsh,sfxmzxbm,sfxmzxmc,ysxm,ksmc,ybdjh) values(
                                         '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}',
                                         '{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}',
                                         '{20}','{21}','{22}','{23}','{24}','{25}')",
                                             jzlsh, JYLSH, sfxmzl, feedetl_sn, index, fee_ocur_time, medins_list_codg, yyxmmc, pric, cnt,
                                             det_item_fee_sumamt, bilg_dr_codg, bilg_dept_codg, CZYBH, sflbdm, grbh, xm, kh, 1, sysdate,
                                             ybjzlsh, med_list_codg, ybxmmc, bilg_dr_name, bilg_dept_name, chrg_bchno);
                    liSQL.Add(strSql);
                    #endregion
                }

                if (Math.Abs(scfy - sfje) > 1.0)
                    return new object[] { 0, 0, "收费金额与医保结算金额相差" + Math.Abs(scfy - sfje) + ",无法结算，请核实!" };
                #endregion

                #region 总入参
                dynamic input = new
                {
                    feedetail = feedetail
                };
                #endregion

                #region 接口调用
                string inputData = JsonConvert.SerializeObject(input);
                WriteLog(sysdate + "  入参|" + inputData.ToString());
                string Err = string.Empty;
                int i = YBServiceRequest("2204", input, ref Err);
                WriteLog(sysdate + "  出参|" + Err.ToString());
                #endregion

                #region 数据处理
                if (i != 1)
                {
                    WriteLog(sysdate + "  门诊处方明细上传失败|" + Err);
                    return new object[] { 0, 0, Err };
                }

                List<OutpatientFeeOutParam> feelist = JsonConvert.DeserializeObject<List<OutpatientFeeOutParam>>(JsonConvert.DeserializeObject<JObject>(Err).GetValue("result").ToString());

                //处理返回明细信息
                int mxindex = 0;
                foreach (OutpatientFeeOutParam result in feelist)
                {
                    string feedetl_sn1 = result.feedetl_sn;   //费用明细流水号
                    string det_item_fee_sumamt1 = result.det_item_fee_sumamt.ToString();   //明细项目费用总额
                    string cnt1 = result.cnt.ToString();   //数量
                    string pric1 = result.pric.ToString();   //单价
                    string pric_uplmt_amt = result.pric_uplmt_amt.ToString();   //定价上限金额
                    string selfpay_prop = result.selfpay_prop.ToString();   //自付比例
                    string fulamt_ownpay_amt = result.fulamt_ownpay_amt.ToString();   //全自费金额
                    string overlmt_amt = result.overlmt_amt.ToString();   //超限价金额
                    string preselfpay_amt = result.preselfpay_amt.ToString();   //先行自付金额
                    string inscp_scp_amt = result.inscp_scp_amt.ToString();   //符合政策范围金额inscp_scp_amt 
                    string chrgitm_lv = result.chrgitm_lv;   //收费项目等级
                    string med_chrgitm_type = result.med_chrgitm_type;   //医疗收费项目类别
                    string bas_medn_flag = result.bas_medn_flag;   //基本药物标志
                    string hi_nego_drug_flag = result.hi_nego_drug_flag;   //医保谈判药品标志
                    string chld_medc_flag = result.chld_medc_flag;   //儿童用药标志
                    string list_sp_item_flag = result.list_sp_item_flag;   //目录特项标志
                    string lmt_used_flag = result.lmt_used_flag;   //限制使用标志
                    string drt_reim_flag = result.drt_reim_flag;   //直报标志
                    string memo = result.memo;   //备注

                    strSql = string.Format(@"insert into ybcfmxscfhdr(jzlsh,ybcfh,je,sl,dj,djsxje,zlbl,zfje,cxjzfje,xxzfje,fhzcfwje,
                                            sfxmdj,sflb,jjywbz,ybtbypbz,etyybz,mltsbz,qezfbz,zbbz,bz,sysdate,
                                            jylsh,ybjzlsh,ybdjh,yyxmdm,yyxmmc,yybxmbh,ybxmmc) values(
                                            '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}',
                                            '{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}',
                                            '{20}','{21}','{22}','{23}','{24}','{25}','{26}','{27}')",
                                            jzlsh, feedetl_sn1, det_item_fee_sumamt1, cnt1, pric1, pric_uplmt_amt, selfpay_prop, fulamt_ownpay_amt, overlmt_amt, preselfpay_amt, inscp_scp_amt,
                                            chrgitm_lv, med_chrgitm_type, bas_medn_flag, hi_nego_drug_flag, chld_medc_flag, list_sp_item_flag, lmt_used_flag, drt_reim_flag, memo, sysdate,
                                            JYLSH, ybjzlsh, chrg_bchno, liyyxmbh[mxindex], liyyxmmc[mxindex], liybxmbm[mxindex], liybxmmc[mxindex]);
                    liSQL.Add(strSql);
                    mxindex++;
                }

                object[] obj = liSQL.ToArray();
                obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);
                if (obj[1].ToString().Equals("1"))
                {
                    WriteLog(sysdate + "  门诊处方明细上传成功|");
                    return new object[] { 0, 1, JYLSH };
                }
                else
                {
                    WriteLog(sysdate + "  门诊处方明细上传失败|数据操作失败|" + obj[2].ToString());
                    //门诊费用上传撤销
                    object[] objParam1 = { ybjzlsh, grbh, cbdybqh };
                    objParam1 = N_YBMZSFSCCX(objParam1);
                    return new object[] { 0, 0, "门诊处方明细上传失败|数据操作失败|" + obj[2].ToString() };
                }
                #endregion
            }
            catch (Exception ex)
            {
                WriteLog(sysdate + "  门诊处方明细上传异常" + ex.Message);
                return new object[] { 0, 0, "门诊处方明细上传异常" + ex.Message };
            }

        }
        #endregion

        #region 门诊费用撤销上传调用方法
        public static void RollBackMZFY(List<OutpatientFee> feelist)
        {
            //int code = 1;
            //string sysdate = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            //object[] objcx = null;
            //foreach (var item in feelist)
            //{
            //    objcx = YBMZFYMXSCCX(item.mdtrt_id, item.rxno, item.feedetl_sn);
            //    if (objcx[1].ToString() == "0")
            //    {
            //        code = 0;
            //    }
            //}
            //if (code == 1)
            //{
            //    MessageBox.Show("撤销成功！请重新进行结算！");
            //}
            //else
            //{
            //    MessageBox.Show("撤销失败！" + objcx[2].ToString());
            //    WriteLog(sysdate + $"费用明细上传失败！" + objcx[2].ToString());
            //}
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
        private static object[] YBMZFYMXSCCX(object[] objParam)
        {
            string sysdate = GetServerDateTime();
            WriteLog(sysdate + "  进入门诊费用登记撤销...");
            WriteLog(sysdate + " HIS入参|" + string.Join(",", objParam));
            try
            {
                string jzlsh = objParam[0].ToString();
                string ybjzlsh = objParam[1].ToString();

                if (string.IsNullOrEmpty(jzlsh))
                    return new object[] { 0, 0, "就诊流水号不能为空" };


                #region 获取费用登记信息
                string strSql = string.Format(@"select distinct jylsh,ybjzlsh from ybcfmxscindr where ybjzlsh='{0}' and cxbz=1 
                                            and jylsh not in(select isnull(cfmxjylsh,'') as cfjylsh from ybfyjsdr where cxbz=1 and ybjzlsh='{0}')"
                                                , ybjzlsh);
                DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                if (ds.Tables[0].Rows.Count == 0)
                {
                    WriteLog(sysdate + "  该患者未上传费用信息或已完成结算");
                    return new object[] { 0, 0, "该患者未上传费用信息或已完成结算" };
                }
                string cfjylsh = ds.Tables[0].Rows[0]["jylsh"].ToString();

                strSql = string.Format(@"select * from ybmzzydjdr where ybjzlsh='{0}' and cxbz=1", ybjzlsh);
                ds.Tables.Clear();
                ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                string grbh = ds.Tables[0].Rows[0]["grbh"].ToString();
                string cbdybqh = ds.Tables[0].Rows[0]["cbdybqh"].ToString();
                #endregion

                #region 入参赋值
                dynamic data = new
                {
                    data = new
                    {

                        mdtrt_id = ybjzlsh,
                        chrg_bchno = "0000",
                        psn_no = grbh
                    }
                };
                #endregion
                string Err = string.Empty;
                #region 接口调用
                string inputData = JsonConvert.SerializeObject(data);
                WriteLog(sysdate + "  入参|" + inputData.ToString());
                int i = YBServiceRequest("2205", data, ref Err);
                WriteLog(sysdate + "  出参|" + Err.ToString());

                if (i != 1)
                {
                    WriteLog(sysdate + "  门诊费用登记撤销失败|" + Err.ToString());
                    return new object[] { 0, 0, "门诊费用登记撤销失败|" + Err.ToString() };
                }

                List<string> liSQL = new List<string>();
                strSql = string.Format(@"delete from ybcfmxscindr where ybjzlsh='{0}' and jylsh='{1}' and cxbz=1", ybjzlsh, cfjylsh);
                liSQL.Add(strSql);
                strSql = string.Format(@"delete from ybcfmxscfhdr where ybjzlsh='{0}' and jylsh='{1}' and cxbz=1", ybjzlsh, cfjylsh);
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
                #endregion
            }
            catch (Exception ex)
            {
                WriteLog(sysdate + "  门诊费用登记撤销异常" + ex.Message);
                return new object[] { 0, 0, "门诊费用登记撤销异常" + ex.Message
    };
            }
        }

        #endregion

        #region 门诊费用预结算


        public static string isusezh = "0";
        public static object[] YBMZSFYJS(object[] objParam)
        {
            string sysdate = GetServerDateTime();//系统时间
            WriteLog(sysdate + "  进入门诊费用预结算...");
            WriteLog(sysdate + "  HIS入参|" + string.Join(",", objParam));

            try
            {
                #region 入参
                string jzlsh = objParam[0].ToString();      // 就诊流水号
                string zhsybz = objParam[1].ToString();     // 账户使用标志（0或1）
                string dqrq = objParam[2].ToString();  // 结算时间
                string bzbm = objParam[3].ToString(); //病种编码
                string bzmc = objParam[4].ToString(); //病种名称
                string cfhs = objParam[5].ToString();   //处方号集
                string yllb = objParam[6].ToString(); //医疗类别
                string ylfhj1 = objParam[7].ToString(); //医疗费合计 (新增)
                string cfysdm = objParam[8].ToString();
                string cfysxm = objParam[9].ToString();
                //string sfdk = objParam[10].ToString(); //是否读卡		（0-否 1-是）
                //string sybz = objParam[11].ToString(); //生育标志		（1.生育 0.非生育）
                //string ewm = objParam[12].ToString(); //医保电子社保二维码
                string djh = "0000";
                if (objParam.Length > 14)
                {
                    objParam[13].ToString(); //单据号
                }
                string jsfs = "01";
                if (objParam.Length > 14)
                {
                    jsfs = objParam[14].ToString(); //结算方式代码	01-按项目结算  02-按定额结算
                }

                if (string.IsNullOrEmpty(jzlsh))
                    return new object[] { 0, 0, "就诊流水号不能为空" };
                if (string.IsNullOrEmpty(yllb))
                    return new object[] { 0, 0, "医疗类别不能为空" };
                //string[] syllb = { "1401", "15" };
                string[] syllb = { "14", "1402", "1403", "9903" };
                string[] syllbtjtz = { "9904", "9907" };
                if (syllb.Contains(yllb))
                {
                    if (string.IsNullOrEmpty(bzbm))
                        return new object[] { 0, 0, "病种编码不能为空" };
                }
                if (syllbtjtz.Contains(yllb))
                {
                    zhsybz = "0";               //特检特治不能使用账户
                }
                #endregion

                #region 变量
                string jzlsh1 = "";
                if (syllb.Contains(yllb))
                {
                    //ffff = "3";
                    jzlsh1 = yllb + jzlsh + bzbm;
                }
                else
                    jzlsh1 = yllb + jzlsh;
                #endregion

                #region 获取医保登记信息
                string strSql = string.Format(@"select a.*,b.* from ybmzzydjdr a inner join ybickxx b on a.grbh=b.grbh where a.jzlsh1='{0}' and cxbz=1", jzlsh1);
                DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                if (ds.Tables[0].Rows.Count == 0)
                {
                    return new object[] { 0, 0, "该患者未医保门诊登记" };
                }
                yllb = ds.Tables[0].Rows[0]["yllb"].ToString(); //医疗类别
                string xm = ds.Tables[0].Rows[0]["xm"].ToString(); //姓名
                string kh = ds.Tables[0].Rows[0]["kh"].ToString();  //社会保障卡号
                string ksbm = ds.Tables[0].Rows[0]["ksbh"].ToString();  //科室编码
                string grbh = ds.Tables[0].Rows[0]["grbh"].ToString(); //个人编号
                string ysbm = string.IsNullOrEmpty(ds.Tables[0].Rows[0]["ysdm"].ToString()) ? ds.Tables[0].Rows[0]["dgysdm"].ToString() : ds.Tables[0].Rows[0]["ysdm"].ToString(); //医生代码
                string ysmc = string.IsNullOrEmpty(ds.Tables[0].Rows[0]["ysxm"].ToString()) ? ds.Tables[0].Rows[0]["dgysxm"].ToString() : ds.Tables[0].Rows[0]["ysdm"].ToString();
                string ybjzlsh = ds.Tables[0].Rows[0]["ybjzlsh"].ToString(); //医保就诊流水号
                string cbdybqh = ds.Tables[0].Rows[0]["cbdybqh"].ToString(); //参保地医保区划
                string mdtrt_cert_type = ds.Tables[0].Rows[0]["jzpzlx"].ToString(); //就诊凭证类型
                string mdtrt_cert_no = ds.Tables[0].Rows[0]["jzpzbh"].ToString();//就诊凭证编号
                string insutype = ds.Tables[0].Rows[0]["xzlx"].ToString();//
                #endregion
                if (mdtrt_cert_no.Contains('|'))
                    mdtrt_cert_no = mdtrt_cert_no.Split('|')[0];
                #region 费用上传
                //门诊费用登记
                object[] objMZFYDJ = { jzlsh, cfhs, ylfhj1, "", dqrq, ybjzlsh };
                objMZFYDJ = YBMZFYMXSC(objMZFYDJ);
                if (!objMZFYDJ[1].ToString().Equals("1"))
                    return objMZFYDJ;
                JYLSH = objMZFYDJ[2].ToString();

                strSql = string.Format(@"select ybdjh,sum(je) ylfze,isnull(sum(zfje),0.00) zfje, isnull(sum(cxjzfje),0.00) as cxjje,isnull(sum(xxzfje),0.00) as xxzfje, isnull(sum(fhzcfwje),0.00) as fhzcfwje
                                        from ybcfmxscfhdr where jylsh='{0}' group by ybdjh ", JYLSH);
                ds.Tables.Clear();
                ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                if (ds.Tables[0].Rows.Count == 0)
                {
                    WriteLog(sysdate + "  无上传费用明细信息!");
                    return new object[] { 0, 0, "无上传费用明细信息！" };
                }
                DataRow dr = ds.Tables[0].Rows[0];
                string ylfze = dr["ylfze"].ToString();
                string zfje = dr["zfje"].ToString();
                string cxjje = dr["cxjje"].ToString();
                string xxzfje = dr["xxzfje"].ToString();
                string fhzcfwje = dr["fhzcfwje"].ToString();
                string sfpch = dr["ybdjh"].ToString();
                #endregion

                object[] cfcx = new object[] { jzlsh, ybjzlsh };
                if (Math.Abs(Convert.ToDecimal(ylfze) - Convert.ToDecimal(ylfhj1)) > 1)
                {
                    WriteLog(sysdate + "  门诊费用结算失败，收费金额与医保结算金额不等！");
                    return new object[] { 0, 0, "收费金额与医保结算金额相差" + Math.Abs(Convert.ToDecimal(ylfze) - Convert.ToDecimal(ylfhj1)) + ",无法结算，请核实!" };
                }
                DialogResult res = MessageBox.Show("进入医保门诊收费，是否使用个人账户支付??\r\n选择[是]进行医保账户支付选择[否]现金支付", "提示", MessageBoxButtons.YesNo);
                if (res == DialogResult.Yes)
                {
                    isusezh = "1";
                }
                else
                {
                    isusezh = "0";
                }
                #region 预结算入参
                dynamic data = new
                {
                    data = new
                    {
                        psn_no = grbh,
                        mdtrt_cert_type = mdtrt_cert_type,
                        mdtrt_cert_no = mdtrt_cert_no,
                        med_type = yllb,
                        medfee_sumamt = ylfze,
                        psn_setlway = jsfs,
                        mdtrt_id = ybjzlsh,
                        chrg_bchno = sfpch,
                        insutype = insutype,
                        acct_used_flag = isusezh,//zhsybz
                        expContent = ""
                    }
                };
                string Err = string.Empty;
                #endregion
                #region 接口调用
                string inputData = JsonConvert.SerializeObject(data);
                WriteLog(sysdate + "  入参|" + inputData.ToString());
                int i = YBServiceRequest("2206", data, ref Err);
                WriteLog(sysdate + "  出参|" + Err.ToString());
                if (i != 1)
                {
                    YBMZCFMXSCCX(cfcx);
                    WriteLog(sysdate + "  门诊费用预结算失败|" + Err);
                    return new object[] { 0, 0, "门诊费用预结算失败|" + Err.ToString() };
                }
                #endregion

                JObject j = JObject.Parse(Err);
                #region 数据处理
                //输出-结算信息
                string mdtrt_id = Convert.ToString(j["setlinfo"]["mdtrt_id"]);	//就诊ID
                //string setl_id = Convert.ToString(j["setlinfo"]["setl_id"]);	//	结算ID
                string psn_no = Convert.ToString(j["setlinfo"]["psn_no"]);	//	人员编号
                string psn_name = Convert.ToString(j["setlinfo"]["psn_name"]);	//	人员姓名
                string psn_cert_type = Convert.ToString(j["setlinfo"]["psn_cert_type"]);	//	人员证件类型
                string certno = Convert.ToString(j["setlinfo"]["certno"]);	//	证件号码
                string gend = Convert.ToString(j["setlinfo"]["gend"]);	//	性别
                string naty = Convert.ToString(j["setlinfo"]["naty"]);	//	民族
                string brdy = Convert.ToString(j["setlinfo"]["brdy"]);	//	出生日期
                string age = Convert.ToString(j["setlinfo"]["age"]);	//	年龄
                string insutype1 = Convert.ToString(j["setlinfo"]["insutype1"]);	//	险种类型
                string psn_type = Convert.ToString(j["setlinfo"]["psn_type"]);	//	人员类别
                string cvlserv_flag = Convert.ToString(j["setlinfo"]["cvlserv_flag"]);	//	公务员标志
                string setl_time = Convert.ToString(j["setlinfo"]["setl_time"]);	//	结算时间
                string mdtrt_cert_type1 = Convert.ToString(j["setlinfo"]["mdtrt_cert_type"]);	//	就诊凭证类型
                string med_type = Convert.ToString(j["setlinfo"]["med_type"]);	//	医疗类别
                string medfee_sumamt = Convert.ToString(j["setlinfo"]["medfee_sumamt"]);	//	医疗费总额
                string fulamt_ownpay_amt = Convert.ToString(j["setlinfo"]["fulamt_ownpay_amt"]);	//	全自费金额
                string overlmt_selfpay = Convert.ToString(j["setlinfo"]["overlmt_selfpay"]);	//	超限价自费费用
                string preselfpay_amt = Convert.ToString(j["setlinfo"]["preselfpay_amt"]);	//	先行自付金额
                string inscp_scp_amt = Convert.ToString(j["setlinfo"]["inscp_scp_amt"]);	//	符合政策范围金额
                string act_pay_dedc = Convert.ToString(j["setlinfo"]["act_pay_dedc"]);	//	实际支付起付线
                string hifp_pay = Convert.ToString(j["setlinfo"]["hifp_pay"]);	//	基本医疗保险统筹基金支出
                string pool_prop_selfpay = Convert.ToString(j["setlinfo"]["pool_prop_selfpay"]);	//	基本医疗保险统筹基金支付比例
                string cvlserv_pay = Convert.ToString(j["setlinfo"]["cvlserv_pay"]);	//	公务员医疗补助资金支出
                string hifes_pay = Convert.ToString(j["setlinfo"]["hifes_pay"]);	//企业补充医疗保险基金支出
                string hifmi_pay = Convert.ToString(j["setlinfo"]["hifmi_pay"]);	//居民大病保险资金支出
                string hifob_pay = Convert.ToString(j["setlinfo"]["hifob_pay"]);	//职工大额医疗费用补助基金支出
                string maf_pay = Convert.ToString(j["setlinfo"]["maf_pay"]);	//医疗救助基金支出
                string oth_pay = Convert.ToString(j["setlinfo"]["oth_pay"]);	//	其他支出
                string fund_pay_sumamt = Convert.ToString(j["setlinfo"]["fund_pay_sumamt"]);	//	基金支付总额
                string psn_part_amt = Convert.ToString(j["setlinfo"]["psn_part_amt"]);	//个人负担总金额
                string acct_pay = Convert.ToString(j["setlinfo"]["acct_pay"]);	//	个人账户支出
                string psn_cash_pay = Convert.ToString(j["setlinfo"]["psn_cash_pay"]);	//	个人现金支出
                string hosp_part_amt = Convert.ToString(j["setlinfo"]["hosp_part_amt"]);	//	医院负担金额
                string balc = Convert.ToString(j["setlinfo"]["balc"]);	//	余额
                string acct_mulaid_pay = Convert.ToString(j["setlinfo"]["acct_mulaid_pay"]);	//	个人账户共济支付金额
                string medins_setl_id = Convert.ToString(j["setlinfo"]["medins_setl_id"]);	//	医药机构结算ID
                string clr_optins = Convert.ToString(j["setlinfo"]["clr_optins"]);	//	清算经办机构
                string clr_way = Convert.ToString(j["setlinfo"]["clr_way"]);	//	清算方式
                string clr_type = Convert.ToString(j["setlinfo"]["clr_type"]);	//	清算类别

                //总报销金额
                string zbxje = (Convert.ToDecimal(medfee_sumamt) - Convert.ToDecimal(psn_cash_pay)).ToString("0.00");
                //结算前账户余额
                string bcjsqzhye = (Convert.ToDecimal(acct_pay) + Convert.ToDecimal(balc)).ToString("0.00");
                //整合现金支付
                string zhxjzf = psn_cash_pay;
                //整合总报销金额
                string zhzbxje = zbxje;
                //其他医保费用
                string qtybfy = (Convert.ToDecimal(medfee_sumamt) - Convert.ToDecimal(psn_cash_pay) - Convert.ToDecimal(acct_pay) - Convert.ToDecimal(hifp_pay)).ToString("0.00");

                #region 医保类型
                string yblx = string.Empty;

                strSql = string.Format(@"select ylrylb from ybickxx where grbh='{0}'", grbh);
                ds.Tables.Clear();
                ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                if (ds.Tables[0].Rows.Count != 0)
                {
                    string[] sValue = { "11", "21", "31" };
                    if (sValue.Contains(ds.Tables[0].Rows[0]["ylrylb"].ToString()))
                        yblx += "(职工)";
                    else
                        yblx += "(居民)";
                }
                #endregion

                #endregion

                #region 数据返回 
                try
                {
                    setl_time = Convert.ToDateTime(setl_time).ToString("yyyyMMddHHmmss");
                }
                catch (Exception ex)
                {
                    setl_time = DateTime.Now.ToString("yyyyMMddHHmmss");
                }
                if (string.IsNullOrEmpty(pool_prop_selfpay))
                    pool_prop_selfpay = "0";
                if (string.IsNullOrEmpty(hosp_part_amt))
                    hosp_part_amt = "0";
                if (string.IsNullOrEmpty(acct_mulaid_pay))
                    acct_mulaid_pay = "0";
                string bxbl = (Double.Parse(pool_prop_selfpay) * 100).ToString() + "%";
                string strValue = medfee_sumamt + "|" + zbxje + "|" + hifp_pay + "|" + hifob_pay + "|" + acct_pay + "|" +
                                  psn_cash_pay + "|" + cvlserv_pay + "|" + hifes_pay + "|" + fulamt_ownpay_amt + "|" + "0.00" + "|" +
                                  hosp_part_amt + "|" + maf_pay + "|" + overlmt_selfpay + "|" + "0.00" + "|" + "0.00" + "|" +
                                  inscp_scp_amt + "|" + act_pay_dedc + "|" + "0.00" + "|" + "0.00" + "|" + "0.00" + "|" +
                                  "0.00" + "|" + "0.00" + "|" + "0.00" + "|" + "0.00" + "|" + "0.00" + "|" +
                                  bcjsqzhye + "|" + "0.00" + "|" + "0.00" + "|" + "0.00" + "|" + "0.00" + "|" +
                                  "0.00" + "|" + "0" + "|" + "0" + "|" + psn_name + "|" + setl_time + "|" +
                                  yllb + "||" + YBJGBH + "|" + YWZQH + "|" + "0000" + "|" +
                                  "|" + djh + "|" + clr_type + "|" + JYLSH + "|1|" +
                                  "|" + YBJGBH + "|0.00|||" +
                                  grbh + "|0.00|0.00|0.00|0.00|" +
                                  "0.00|0.00|0.00|" + "0.00" + "|" + "0.00" + "|" +
                                  yblx + "|" + bxbl;
                WriteLog(sysdate + "  门诊费用预结算返回参数|" + strValue);
                //return new object[] { 0, 1, strValue };
                List<string> liSql = new List<string>();
                strSql = string.Format(@" delete from ybfyyjsdr where jzlsh='{0}'", jzlsh);
                liSql.Add(strSql);
                strSql = string.Format(@"insert into ybfyyjsdr(jzlsh,djhin,djh,jylsh,yblx,cxbz,sysdate,jbr,kh,bzbm,
                                        bzmc,ztjsbz,tcqh,qtybfy,zbxje,zhzbxje,zhxjzffy,ybjzlsh,grbh,xm,
                                        ryzjlx,zjhm,xb,mz,csrq,nl,xzlx,yldylb,gwybz,jsrq,
                                        jzpzlx,yllb,ylfze,zffy,cxjfy,xxzfje,fhjbylfy,qfbzfy,tcjjzf,jbylbxtcjjzfbl,
                                        gwybzjjzf,qybcylbxjjzf,dbjjzf,dejjzf,mzjzfy,qtjjzf,jjzfze,grfdzje,zhzf,xjzf,
                                        yyfdfy,bcjsqzhye,grzhgjzfje,jbjgbm,jsfs,jylx,ywzqh,zhsybz,jslsh,cfmxjylsh) values( 
                                        '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}',
                                        '{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}',
                                        '{20}','{21}','{22}','{23}','{24}','{25}','{26}','{27}','{28}','{29}',
                                        '{30}','{31}','{32}','{33}','{34}','{35}','{36}','{37}','{38}','{39}',
                                        '{40}','{41}','{42}','{43}','{44}','{45}','{46}','{47}','{48}','{49}',
                                        '{50}','{51}','{52}','{53}','{54}','{55}','{56}','{57}','{58}','{59}')",
                                       jzlsh, djh, medins_setl_id, JYLSH, yblx, 1, sysdate, CliUtils.fLoginUser, kh, bzbm,
                                       bzmc, 0, cbdybqh, qtybfy, zbxje, zhzbxje, zhxjzf, ybjzlsh, psn_no, psn_name,
                                       psn_cert_type, certno, gend, naty, brdy, age, insutype1, psn_type, cvlserv_flag, setl_time,
                                       mdtrt_cert_type1, med_type, medfee_sumamt, fulamt_ownpay_amt, overlmt_selfpay, preselfpay_amt, inscp_scp_amt, act_pay_dedc, hifp_pay, pool_prop_selfpay,
                                       cvlserv_pay, hifes_pay, hifmi_pay, hifob_pay, maf_pay, oth_pay, fund_pay_sumamt, psn_part_amt, acct_pay, psn_cash_pay,
                                       hosp_part_amt, bcjsqzhye, acct_mulaid_pay, clr_optins, clr_way, clr_type, YWZQH, 1, "0000", JYLSH);
                liSql.Add(strSql);

                object[] obj = liSql.ToArray();
                obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);
                if (obj[1].ToString().Equals("1"))
                {
                    WriteLog(sysdate + "  门诊费用预结算成功|");
                    return new object[] { 0, 1, strValue };
                }
                else
                {
                    WriteLog(sysdate + "  门诊费用预结算成功|数据操作失败|" + obj[2].ToString());
                    return new object[] { 0, 0, "门诊费用预结算成功|数据操作失败|" + obj[2].ToString() };
                }
                #endregion
            }
            catch (Exception ex)
            {
                WriteLog(sysdate + "  门诊费用预结算异常" + ex.Message);
                return new object[] { 0, 0, "门诊费用预结算异常" + ex.Message };
            }
        }
        #endregion

        #region 门诊费用结算
        public static object[] YBMZSFJS(object[] objParam)
        {
            string sysdate = GetServerDateTime();//系统时间
            WriteLog(sysdate + "  进入门诊费用结算...");
            WriteLog(sysdate + "  HIS入参|" + string.Join(",", objParam));
            try
            {
                #region 入参
                string jzlsh = objParam[0].ToString();      // 就诊流水号
                string djh = objParam[1].ToString(); //单据号
                string zhsybz = objParam[2].ToString();     // 账户使用标志（0或1）
                string dqrq = objParam[3].ToString();  // 结算时间
                string bzbm = objParam[4].ToString(); //病种编码
                string bzmc = objParam[5].ToString(); //病种名称
                string cfhs = objParam[6].ToString();   //处方号集
                string yllb = objParam[7].ToString(); //医疗类别
                string ylfhj1 = objParam[8].ToString(); //医疗费合计 (新增)
                string cfysdm = objParam[9].ToString();
                string cfysxm = objParam[10].ToString();
                //string sfdk = objParam[11].ToString(); //是否读卡		（0-否 1-是）
                //string sybz = objParam[12].ToString(); //生育标志		（1.生育 0.非生育）
                //string ewm = objParam[13].ToString(); //医保电子社保二维码
                string jsfs = "01";
                if (objParam.Length > 14)
                {
                    jsfs = objParam[14].ToString(); //结算方式代码	01-按项目结算  02-按定额结算
                }
                if (string.IsNullOrEmpty(jzlsh))
                    return new object[] { 0, 0, "就诊流水号不能为空" };
                if (string.IsNullOrEmpty(yllb))
                    return new object[] { 0, 0, "医疗类别不能为空" };
                if (string.IsNullOrEmpty(djh))
                    return new object[] { 0, 0, "单据号不能为空" };
                string[] syllb = { "14", "1402", "1403", "9903" };
                string[] syllbtjtz = { "9904", "9907" };
                if (syllb.Contains(yllb))
                {
                    if (string.IsNullOrEmpty(bzbm))
                        return new object[] { 0, 0, "病种编码不能为空" };
                }
                if (syllbtjtz.Contains(yllb))
                {
                    zhsybz = "0";               //特检特治不能使用账户
                }
                #endregion

                #region 变量
                string jzlsh1 = "";
                if (syllb.Contains(yllb))
                {
                    //ffff = "3";
                    jzlsh1 = yllb + jzlsh + bzbm;
                }
                else
                    jzlsh1 = yllb + jzlsh;
                #endregion

                #region 获取医保登记信息
                string strSql = string.Format(@"select a.*,b.* from ybmzzydjdr a inner join ybickxx b on a.grbh=b.grbh where a.jzlsh1='{0}' and cxbz=1", jzlsh1);
                DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                if (ds.Tables[0].Rows.Count == 0)
                    return new object[] { 0, 0, "该患者未医保门诊登记" };
                yllb = ds.Tables[0].Rows[0]["yllb"].ToString(); //医疗类别
                string xm = ds.Tables[0].Rows[0]["xm"].ToString(); //姓名
                string kh = ds.Tables[0].Rows[0]["kh"].ToString();  //社会保障卡号
                string ksbm = ds.Tables[0].Rows[0]["ksbh"].ToString();  //科室编码
                string grbh = ds.Tables[0].Rows[0]["grbh"].ToString(); //个人编号
                string ysbm = ds.Tables[0].Rows[0]["ysdm"].ToString(); //医生代码
                string ybjzlsh = ds.Tables[0].Rows[0]["ybjzlsh"].ToString(); //医保就诊流水号
                string cbdybqh = ds.Tables[0].Rows[0]["tcqh"].ToString(); //参保地医保区划
                string mdtrt_cert_type = ds.Tables[0].Rows[0]["jzpzlx"].ToString(); //就诊凭证类型
                string mdtrt_cert_no = ds.Tables[0].Rows[0]["jzpzbh"].ToString();//就诊凭证编号
                string insutype = ds.Tables[0].Rows[0]["xzlx"].ToString();//
                #endregion
                //if (mdtrt_cert_no.Contains('|'))
                //    mdtrt_cert_no = mdtrt_cert_no.Split('|')[0];

                #region 费用上传
                //门诊费用登记
                object[] objMZFYDJ = { jzlsh, cfhs, ylfhj1, "", dqrq, ybjzlsh };
                objMZFYDJ = YBMZFYMXSC(objMZFYDJ);
                if (!objMZFYDJ[1].ToString().Equals("1"))
                    return objMZFYDJ;
                JYLSH = objMZFYDJ[2].ToString();

                strSql = string.Format(@"select ybdjh,sum(je) ylfze,isnull(sum(zfje),0.00) zfje, isnull(sum(cxjzfje),0.00) as cxjje,isnull(sum(xxzfje),0.00) as xxzfje, isnull(sum(fhzcfwje),0.00) as fhzcfwje
                                        from ybcfmxscfhdr where jylsh='{0}' group by ybdjh ", JYLSH);
                ds.Tables.Clear();
                ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                if (ds.Tables[0].Rows.Count == 0)
                {
                    WriteLog(sysdate + "  无上传费用明细信息!");
                    return new object[] { 0, 0, "无上传费用明细信息！" };
                }
                DataRow dr = ds.Tables[0].Rows[0];
                string ylfze = dr["ylfze"].ToString();
                string zfje = dr["zfje"].ToString();
                string cxjje = dr["cxjje"].ToString();
                string xxzfje = dr["xxzfje"].ToString();
                string fhzcfwje = dr["fhzcfwje"].ToString();
                string sfpch = dr["ybdjh"].ToString();
                #endregion

                if (Math.Abs(Convert.ToDecimal(ylfze) - Convert.ToDecimal(ylfhj1)) > 1)
                {
                    WriteLog(sysdate + "  门诊费用结算失败，收费金额与医保结算金额不等！");
                    return new object[] { 0, 0, "收费金额与医保结算金额相差" + Math.Abs(Convert.ToDecimal(ylfze) - Convert.ToDecimal(ylfhj1)) + ",无法结算，请核实!" };
                }

                #region 结算入参
                dynamic data = new
                {
                    data = new
                    {
                        psn_no = grbh,
                        mdtrt_cert_type = mdtrt_cert_type,
                        mdtrt_cert_no = mdtrt_cert_no,
                        med_type = yllb,
                        medfee_sumamt = ylfze,
                        psn_setlway = jsfs,
                        mdtrt_id = ybjzlsh,
                        chrg_bchno = sfpch,
                        insutype = insutype,
                        acct_used_flag = isusezh,//zhsybz
                        invono = djh,
                        fulamt_ownpay_amt = zfje,
                        overlmt_selfpay = cxjje,
                        preselfpay_amt = xxzfje,
                        inscp_scp_amt = fhzcfwje
                    }
                };
                string Err = string.Empty;
                #endregion

                #region 接口调用
                string inputData = JsonConvert.SerializeObject(data);
                WriteLog(sysdate + "  入参|" + inputData.ToString());
                int i = YBServiceRequest("2207", data, ref Err);
                WriteLog(sysdate + "  出参|" + Err.ToString());
                if (i != 1)
                {
                    WriteLog(sysdate + "  门诊费用结算失败|" + Err.ToString());
                    return new object[] { 0, 0, "门诊费用结算失败|" + Err.ToString() };
                }
                #endregion
                JObject j = JsonConvert.DeserializeObject<JObject>(Err);
                #region 数据处理
                //输出-结算信息
                string mdtrt_id = Convert.ToString(j["setlinfo"]["mdtrt_id"]);	//就诊ID
                string setl_id = Convert.ToString(j["setlinfo"]["setl_id"]);	//	结算ID
                string psn_no = Convert.ToString(j["setlinfo"]["psn_no"]);	//	人员编号
                string psn_name = Convert.ToString(j["setlinfo"]["psn_name"]);	//	人员姓名
                string psn_cert_type = Convert.ToString(j["setlinfo"]["psn_cert_type"]);	//	人员证件类型
                string certno = Convert.ToString(j["setlinfo"]["certno"]);	//	证件号码
                string gend = Convert.ToString(j["setlinfo"]["gend"]);	//	性别
                string naty = Convert.ToString(j["setlinfo"]["naty"]);	//	民族
                string brdy = Convert.ToString(j["setlinfo"]["brdy"]);	//	出生日期
                string age = Convert.ToString(j["setlinfo"]["age"]);	//	年龄
                string insutype1 = Convert.ToString(j["setlinfo"]["insutype1"]);	//	险种类型
                string psn_type = Convert.ToString(j["setlinfo"]["psn_type"]);	//	人员类别
                string cvlserv_flag = Convert.ToString(j["setlinfo"]["cvlserv_flag"]);	//	公务员标志
                string setl_time = Convert.ToString(j["setlinfo"]["setl_time"]);	//	结算时间
                string mdtrt_cert_type1 = Convert.ToString(j["setlinfo"]["mdtrt_cert_type"]);	//	就诊凭证类型
                string med_type = Convert.ToString(j["setlinfo"]["med_type"]);	//	医疗类别
                string medfee_sumamt = Convert.ToString(j["setlinfo"]["medfee_sumamt"]);	//	医疗费总额
                string fulamt_ownpay_amt = Convert.ToString(j["setlinfo"]["fulamt_ownpay_amt"]);	//	全自费金额
                string overlmt_selfpay = Convert.ToString(j["setlinfo"]["overlmt_selfpay"]);	//	超限价自费费用
                string preselfpay_amt = Convert.ToString(j["setlinfo"]["preselfpay_amt"]);	//	先行自付金额
                string inscp_scp_amt = Convert.ToString(j["setlinfo"]["inscp_scp_amt"]);	//	符合政策范围金额
                string act_pay_dedc = Convert.ToString(j["setlinfo"]["act_pay_dedc"]);	//	实际支付起付线
                string hifp_pay = Convert.ToString(j["setlinfo"]["hifp_pay"]);	//	基本医疗保险统筹基金支出
                string pool_prop_selfpay = Convert.ToString(j["setlinfo"]["pool_prop_selfpay"]);	//	基本医疗保险统筹基金支付比例
                string cvlserv_pay = Convert.ToString(j["setlinfo"]["cvlserv_pay"]);	//	公务员医疗补助资金支出
                string hifes_pay = Convert.ToString(j["setlinfo"]["hifes_pay"]);	//企业补充医疗保险基金支出
                string hifmi_pay = Convert.ToString(j["setlinfo"]["hifmi_pay"]);	//居民大病保险资金支出
                string hifob_pay = Convert.ToString(j["setlinfo"]["hifob_pay"]);	//职工大额医疗费用补助基金支出
                string maf_pay = Convert.ToString(j["setlinfo"]["maf_pay"]);	//医疗救助基金支出
                string oth_pay = Convert.ToString(j["setlinfo"]["oth_pay"]);	//	其他支出
                string fund_pay_sumamt = Convert.ToString(j["setlinfo"]["fund_pay_sumamt"]);	//	基金支付总额
                string psn_part_amt = Convert.ToString(j["setlinfo"]["psn_part_amt"]);	//个人负担总金额
                string acct_pay = Convert.ToString(j["setlinfo"]["acct_pay"]);	//	个人账户支出
                string psn_cash_pay = Convert.ToString(j["setlinfo"]["psn_cash_pay"]);	//	个人现金支出
                string hosp_part_amt = Convert.ToString(j["setlinfo"]["hosp_part_amt"]);	//	医院负担金额
                string balc = Convert.ToString(j["setlinfo"]["balc"]);	//	余额
                string acct_mulaid_pay = Convert.ToString(j["setlinfo"]["acct_mulaid_pay"]);	//	个人账户共济支付金额
                string medins_setl_id = Convert.ToString(j["setlinfo"]["medins_setl_id"]);	//	医药机构结算ID
                string clr_optins = Convert.ToString(j["setlinfo"]["clr_optins"]);	//	清算经办机构
                string clr_way = Convert.ToString(j["setlinfo"]["clr_way"]);	//	清算方式
                string clr_type = Convert.ToString(j["setlinfo"]["clr_type"]);	//	清算类别

                //总报销金额
                string zbxje = (Convert.ToDecimal(medfee_sumamt) - Convert.ToDecimal(psn_cash_pay)).ToString("0.00");
                //结算前账户余额
                string bcjsqzhye = (Convert.ToDecimal(acct_pay) + Convert.ToDecimal(balc)).ToString("0.00");
                //整合现金支付
                string zhxjzf = psn_cash_pay;
                //整合总报销金额
                string zhzbxje = zbxje;
                //其他医保费用
                string qtybfy = (Convert.ToDecimal(medfee_sumamt) - Convert.ToDecimal(psn_cash_pay) - Convert.ToDecimal(acct_pay) - Convert.ToDecimal(hifp_pay)).ToString("0.00");

                #region 医保类型
                string yblx = string.Empty;
                string[] sValue = { "11", "12", "13" };
                if (sValue.Contains(psn_type.ToString().Substring(0, 2)))
                    yblx += "职工医保";
                else
                    yblx += "居民医保";
                #endregion

                //结算基金分项信息

                List<string> liSql = new List<string>();
                JToken jts = Convert.ToString(j["setldetail"]);
                foreach (JToken jt in jts)
                {
                    string fund_pay_type = Convert.ToString(jt["fund_pay_type"]);//	基金支付类型
                    string inscp_scp_amt1 = Convert.ToString(jt["inscp_scp_amt"]);//	符合政策范围金额
                    string crt_payb_lmt_amt = Convert.ToString(jt["crt_payb_lmt_amt"]);//	本次可支付限额金额
                    string fund_payamt = Convert.ToString(jt["fund_payamt"]);//	基金支付金额
                    string fund_pay_type_name = Convert.ToString(jt["fund_pay_type_name"]);//	基金支付类型名称
                    string setl_proc_info = Convert.ToString(jt["setl_proc_info"]);//	结算过程信息

                    strSql = string.Format(@"insert into ybfyjsmxdr(jzlsh,jylsh,jslsh,ybjzlsh,cxbz,sysdate,jbr,jjzflx,fhzcfwje,bckzfxeje,jjzfje,jjzflxmc,jsgcxx) values(
                                            '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}')",
                                            jzlsh, JYLSH, setl_id, ybjzlsh, 1, sysdate, CliUtils.fLoginUser, fund_pay_type, inscp_scp_amt1, crt_payb_lmt_amt, fund_payamt, fund_pay_type_name, setl_proc_info);
                    liSql.Add(strSql);
                }


                #endregion

                #region 数据返回
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
                if (string.IsNullOrEmpty(pool_prop_selfpay))
                    pool_prop_selfpay = "0";
                if (string.IsNullOrEmpty(hosp_part_amt))
                    hosp_part_amt = "0";
                if (string.IsNullOrEmpty(acct_mulaid_pay))
                    acct_mulaid_pay = "0";
                string strValue = medfee_sumamt + "|" + zbxje + "|" + hifp_pay + "|" + hifob_pay + "|" + acct_pay + "|" +
                                  psn_cash_pay + "|" + cvlserv_pay + "|" + hifes_pay + "|" + fulamt_ownpay_amt + "|" + "0.00" + "|" +
                                  hosp_part_amt + "|" + maf_pay + "|" + overlmt_selfpay + "|" + "0.00" + "|" + "0.00" + "|" +
                                  inscp_scp_amt + "|" + act_pay_dedc + "|" + "0.00" + "|" + "0.00" + "|" + "0.00" + "|" +
                                  "0.00" + "|" + "0.00" + "|" + "0.00" + "|" + "0.00" + "|" + "0.00" + "|" +
                                  bcjsqzhye + "|" + "0.00" + "|" + "0.00" + "|" + "0.00" + "|" + "0.00" + "|" +
                                  "0.00" + "|" + "0" + "|" + "0" + "|" + psn_name + "|" + setl_time + "|" +
                                  yllb + "||" + YBJGBH + "|" + YWZQH + "|" + setl_id + "|" +
                                  "|" + djh + "|" + clr_type + "|" + JYLSH + "|1|" +
                                  "|" + YBJGBH + "|0.00|||" +
                                  grbh + "|0.00|0.00|0.00|0.00|" +
                                  "0.00|0.00|0.00|" + "0.00" + "|" + "0.00" + "|" +
                                  yblx;
                WriteLog(sysdate + "  门诊费用结算返回参数|" + strValue);
                try
                {
                    setl_time = Convert.ToDateTime(setl_time).ToString("yyyyMMddHHmmss");
                }
                catch (Exception ex)
                {
                    setl_time = DateTime.Now.ToString("yyyyMMddHHmmss");
                }
                strSql = string.Format(@"insert into ybfyjsdr(jzlsh,djhin,djh,jylsh,yblx,cxbz,sysdate,jbr,kh,bzbm,
                                        bzmc,ztjsbz,tcqh,qtybfy,zbxje,zhzbxje,zhxjzffy,ybjzlsh,grbh,xm,
                                        ryzjlx,zjhm,xb,mz,csrq,nl,xzlx,yldylb,gwybz,jsrq,
                                        jzpzlx,yllb,ylfze,zffy,cxjfy,xxzfje,fhjbylfy,qfbzfy,tcjjzf,jbylbxtcjjzfbl,
                                        gwybzjjzf,qybcylbxjjzf,dbjjzf,dejjzf,mzjzfy,qtjjzf,jjzfze,grfdzje,zhzf,xjzf,
                                        yyfdfy,bcjsqzhye,grzhgjzfje,jbjgbm,jsfs,jylx,ywzqh,zhsybz,jslsh,cfmxjylsh) values( 
                                        '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}',
                                        '{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}',
                                        '{20}','{21}','{22}','{23}','{24}','{25}','{26}','{27}','{28}','{29}',
                                        '{30}','{31}','{32}','{33}','{34}','{35}','{36}','{37}','{38}','{39}',
                                        '{40}','{41}','{42}','{43}','{44}','{45}','{46}','{47}','{48}','{49}',
                                        '{50}','{51}','{52}','{53}','{54}','{55}','{56}','{57}','{58}','{59}')",
                                       jzlsh, djh, medins_setl_id, JYLSH, yblx, 1, sysdate, CliUtils.fLoginUser, kh, bzbm,
                                       bzmc, 0, cbdybqh, qtybfy, zbxje, zhzbxje, zhxjzf, ybjzlsh, psn_no, psn_name,
                                       psn_cert_type, certno, gend, naty, brdy, age, xzlx, psn_type, cvlserv_flag, setl_time,
                                       mdtrt_cert_type1, med_type, medfee_sumamt, fulamt_ownpay_amt, overlmt_selfpay, preselfpay_amt, inscp_scp_amt, act_pay_dedc, hifp_pay, pool_prop_selfpay,
                                       cvlserv_pay, hifes_pay, hifmi_pay, hifob_pay, maf_pay, oth_pay, fund_pay_sumamt, psn_part_amt, acct_pay, psn_cash_pay,
                                       hosp_part_amt, bcjsqzhye, acct_mulaid_pay, clr_optins, clr_way, clr_type, YWZQH, 1, setl_id, JYLSH);
                liSql.Add(strSql);

                object[] obj = liSql.ToArray();
                obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);
                if (obj[1].ToString().Equals("1"))
                {
                    WriteLog(sysdate + "  门诊费用结算成功|");
                    //结算清单上传
                    WriteLog(sysdate + "   门诊结算清单上传,入参" + ybjzlsh);
                    object[] scres = JSQD_4101(new object[] { ybjzlsh });
                    WriteLog(sysdate + "   门诊结算清单上传,回参" + scres[2].ToString());
                    //门诊结算单打印
                    YBMZJSD(new object[] { jzlsh, djh });
                    //门诊结算清单打印 
                    //YBMZJSQD(new object[] { jzlsh, djh });
                    return new object[] { 0, 1, strValue };
                }
                else
                {
                    WriteLog(sysdate + "  门诊费用结算失败|数据操作失败|" + obj[2].ToString());
                    //门诊费用结算撤销(内部)
                    object[] objJSCX = { setl_id, mdtrt_id, psn_no, cbdybqh };
                    N_YBMZFYJSCX(objJSCX);
                    return new object[] { 0, 0, "门诊费用结算失败|数据操作失败|" + obj[2].ToString() };
                }
                #endregion

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
            WriteLog(sysdate + "  进入门诊费用结算撤销...");
            WriteLog(sysdate + "  HIS入参|" + string.Join(",", objParam));
            try
            {
                #region 变量
                string jzlsh = objParam[0].ToString();   // 就诊流水号
                string djh = objParam[1].ToString();     // 单据号(发票号)

                CZYBH = CliUtils.fLoginUser;
                YWZQH = CliUtils.fLoginYbNo.Split('|')[0];

                if (string.IsNullOrEmpty(jzlsh))
                    return new object[] { 0, 0, "就诊流水号不能为空" };
                if (string.IsNullOrEmpty(djh))
                    return new object[] { 0, 0, "单据号不能为空" };
                #endregion

                #region 判断是否已结算
                string strSql = string.Format(@"select * from ybfyjsdr where jzlsh='{0}' and djhin='{1}' and cxbz=1", jzlsh, djh);
                DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                if (ds.Tables[0].Rows.Count == 0)
                    return new object[] { 0, 0, "该患者无医保结算信息" };
                string cfjylsh = ds.Tables[0].Rows[0]["cfmxjylsh"].ToString();
                string ybjzlsh = ds.Tables[0].Rows[0]["ybjzlsh"].ToString();
                string jsrq = ds.Tables[0].Rows[0]["jsrq"].ToString();   //结算日期
                string cyrq = ds.Tables[0].Rows[0]["cyrq"].ToString();  //出院日期
                string jslsh = ds.Tables[0].Rows[0]["jslsh"].ToString(); //结算流水号
                string grbh = ds.Tables[0].Rows[0]["grbh"].ToString();
                string cbdybqh = ds.Tables[0].Rows[0]["tcqh"].ToString();
                #endregion


                #region 入参
                dynamic data = new
                {
                    data = new
                    {
                        setl_id = jslsh,
                        mdtrt_id = ybjzlsh,
                        psn_no = grbh

                    }
                };
                string Err = string.Empty;
                #endregion

                #region 接口调用
                string inputData = JsonConvert.SerializeObject(data);
                WriteLog(sysdate + "  入参|" + inputData.ToString());
                int i = YBServiceRequest("2208", data, ref Err);
                WriteLog(sysdate + "  出参|" + Err.ToString());
                if (i != 1)
                {
                    WriteLog(sysdate + "  门诊费用结算撤销失败|" + Err.ToString());
                    return new object[] { 0, 0, "门诊费用结算撤销失败|" + Err.ToString() };
                }
                #endregion

                #region 数据处理
                List<string> liSQL = new List<string>();
                strSql = string.Format(@"insert into ybfyjsdr (jzlsh,djhin,djh,jylsh,yblx,cxbz,sysdate,jbr,kh,bzbm,
                                        bzmc,ztjsbz,tcqh,qtybfy,zbxje,zhzbxje,zhxjzffy,ybjzlsh,grbh,xm,
                                        ryzjlx,zjhm,xb,mz,csrq,nl,xzlx,yldylb,gwybz,jsrq,
                                        jzpzlx,yllb,ylfze,zffy,cxjfy,xxzfje,fhjbylfy,qfbzfy,tcjjzf,jbylbxtcjjzfbl,
                                        gwybzjjzf,qybcylbxjjzf,dbjjzf,dejjzf,mzjzfy,qtjjzf,jjzfze,grfdzje,zhzf,xjzf,
                                        yyfdfy,bcjsqzhye,grzhgjzfje,jbjgbm,jsfs,jylx,ywzqh,zhsybz,jslsh,cfmxjylsh) 
                                        select jzlsh,djhin,djh,jylsh,yblx,0,'{2}','{3}',kh,bzbm,
                                        bzmc,ztjsbz,tcqh,qtybfy,zbxje,zhzbxje,zhxjzffy,ybjzlsh,grbh,xm,
                                        ryzjlx,zjhm,xb,mz,csrq,nl,xzlx,yldylb,gwybz,jsrq,
                                        jzpzlx,yllb,ylfze,zffy,cxjfy,xxzfje,fhjbylfy,qfbzfy,tcjjzf,jbylbxtcjjzfbl,
                                        gwybzjjzf,qybcylbxjjzf,dbjjzf,dejjzf,mzjzfy,qtjjzf,jjzfze,grfdzje,zhzf,xjzf,
                                        yyfdfy,bcjsqzhye,grzhgjzfje,jbjgbm,jsfs,jylx,ywzqh,zhsybz,jslsh,cfmxjylsh 
                                        from ybfyjsdr where jzlsh='{0}' and djhin='{1}' and cxbz=1", jzlsh, djh, sysdate, CliUtils.fLoginUser);
                liSQL.Add(strSql);
                strSql = string.Format(@"update ybfyjsdr set cxbz=2 where jzlsh='{0}' and djhin='{1}' and cxbz=1", jzlsh, djh);
                liSQL.Add(strSql);

                strSql = string.Format(@"insert into ybfyjsmxdr(jzlsh,jylsh,jslsh,ybjzlsh,cxbz,sysdate,jbr,jjzflx,fhzcfwje,bckzfxeje,jjzfje,jjzflxmc,jsgcxx)
                                        select jzlsh,jylsh,jslsh,ybjzlsh,0,'{1}','{2}',jjzflx,fhzcfwje,bckzfxeje,jjzfje,jjzflxmc,jsgcxx from ybfyjsmxdr where jylsh='{0}' and cxbz=1", cfjylsh, sysdate, CliUtils.fLoginUser);
                liSQL.Add(strSql);
                strSql = string.Format(@"update ybfyjsmxdr set cxbz=2 where jylsh='{0}' and cxbz=1", cfjylsh);
                liSQL.Add(strSql);

                strSql = string.Format(@"delete from ybcfmxscfhdr where jzlsh = '{0}' and jylsh = '{1}' and cxbz = 1", jzlsh, cfjylsh);
                liSQL.Add(strSql);
                strSql = string.Format(@"delete from ybcfmxscindr where jzlsh = '{0}' and jylsh = '{1}' and cxbz = 1", jzlsh, cfjylsh);
                liSQL.Add(strSql);

                object[] obj = liSQL.ToArray();
                obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);
                if (obj[1].ToString().Equals("1"))
                {
                    WriteLog(sysdate + "  门诊费用结算撤销成功|");
                    return new object[] { 0, 1, "  门诊费用结算撤销成功|" };
                }
                else
                {
                    WriteLog(sysdate + "  门诊费用结算撤销失败|数据操作失败|" + obj[2].ToString());
                    return new object[] { 0, 0, "  门诊费用结算撤销失败|数据操作失败|" + obj[2].ToString() };
                }
                #endregion

            }
            catch (Exception ex)
            {
                WriteLog(sysdate + "  门诊费用结算撤销异常|" + ex.Message);
                return new object[] { 0, 0, "门诊费用结算撤销异常|" + ex.Message };
            }
        }
        #endregion

        #region 住院读卡 （新医保接口 ）
        public static object[] YBZYDK(object[] ojbParam)
        {

            string sysdate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            object[] obj = default(object[]);
            string[] info = default(string[]);
            Frm_dkck adkck = new Frm_dkck();
            adkck.ShowDialog();
            adkck.Close();
            adkck.Dispose();
            if (adkck.dzybFlag == DKType.身份证)
            {
                if (MessageBox.Show("系统检测到您已手工输入身份证，是否继续读卡操作！\r\n选择【是】将继续读卡，选择【否】将使用您输入身份证进行读卡", "提示", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    #region 读身份证
                    obj = SFZDKRYXX(null);
                    if (obj[1].ToString() != "1")
                    {
                        return obj;
                    }
                    info = obj[2] as string[];
                    mdtrt_cert_type = "02";
                    mdtrt_cert_no = info[0];
                    #endregion
                }
                else
                {

                    mdtrt_cert_type = "02";
                    mdtrt_cert_no = adkck.txtidcard.Text.Trim();
                }
                if (string.IsNullOrEmpty(mdtrt_cert_no))
                {
                    return new object[] { 0, 0, "系统检测没有身份证信息" };
                }
            }
            else if (adkck.dzybFlag == DKType.社保卡)
            {
                #region 读社保卡
                obj = DSBKJBXX(null);
                if (obj[1].ToString() != "1")
                {
                    return obj;
                }
                info = obj[2] as string[];
                mdtrt_cert_type = "03";
                mdtrt_cert_no = info[2];
                card_sn = info[3];
                psn_cert_type = "01";
                certno = info[1];
                #endregion
            }
            dynamic input = new
            {
                data = new
                {
                    mdtrt_cert_type = mdtrt_cert_type,
                    mdtrt_cert_no = mdtrt_cert_no,
                    card_sn = card_sn,
                    begntime = begntime,
                    psn_cert_type = psn_cert_type,
                    certno = certno,
                    psn_name = psn_name
                }
            };
            //入参
            WriteLog(sysdate + "  人员信息获取...");
            string ErrStr = string.Empty;
            string inJosn = Newtonsoft.Json.JsonConvert.SerializeObject(input);
            WriteLog(sysdate + "  入参" + inJosn.ToString());
            int i = YBServiceRequest("1101", input, ref ErrStr);
            WriteLog(sysdate + "  出参" + ErrStr.ToString());
            if (i > 0)
            {
                ReadCardInfo card = JsonConvert.DeserializeObject<ReadCardInfo>(ErrStr);
                CardInfo = card;
                if (card.baseinfo.age.Contains('.'))
                {
                    card.baseinfo.age = card.baseinfo.age.Split('.')[0];
                }

                string balc = "0";
                string insutype = "";
                string psn_type = "";
                string psn_insu_stas = "";
                string psn_insu_date = "";
                string paus_insu_date = "";
                string cvlserv_flag = "";
                string insuplc_admdvs = "";
                string emp_name = "";
                string psn_idet_type = "";
                string psn_type_lv = "";
                string memo = "";
                string begntime = "";
                string endtime = "";
                string ydrybz = "0";
                string jdlkrybz = "0";
                string sqlStr = string.Format(@" delete from ybrycbxx where sfzh='{0}' ", card.baseinfo.certno);
                object[] obj_zydk = { sqlStr };
                obj_zydk = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj_zydk);
                if (!obj_zydk[1].ToString().Equals("1"))
                {
                    WriteLog(sysdate + "  医保住院读卡，数据操作失败|" + obj_zydk[2].ToString());
                    return new object[] { 0, 0, "医保住院读卡，数据操作失败|" + obj_zydk[2].ToString() };
                }
                #region 参保信息
                foreach (Insuinfo jt in card.insuinfo)
                {

                    string sqlRyxx = string.Format(@" insert into ybrycbxx(balc, insutype, psn_type, psn_insu_stas, psn_insu_date, paus_insu_date, cvlserv_flag, insuplc_admdvs, emp_name, grbh, sfzh, xm)
                                values('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}')",
                            jt.balc, jt.insutype, jt.psn_type, jt.psn_insu_stas, jt.psn_insu_date, jt.paus_insu_date, jt.cvlserv_flag, jt.insuplc_admdvs, jt.emp_name, card.baseinfo.psn_no, card.baseinfo.certno, card.baseinfo.psn_name);
                    DataSet dsCbxx = CliUtils.ExecuteSql("sybdj", "cmd", sqlRyxx, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                }
                #endregion

                #region 身份信息列表 
                string sqlsfxx = string.Format(@" delete from ybrysfxx where sfzh='{0}' ", card.baseinfo.certno);
                DataSet dssfxx = CliUtils.ExecuteSql("sybdj", "cmd", sqlsfxx, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                foreach (Idetinfo jtsf in card.idetinfo)
                {
                    sqlsfxx = string.Format(@" insert into ybrysfxx(psn_idet_type, psn_type_lv, memo, begntime, endtime, grbh, sfzh, xm)
                                values('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}')",
                        jtsf.psn_idet_type, jtsf.psn_type_lv, jtsf.memo, jtsf.begntime, jtsf.endtime, card.baseinfo.psn_no, card.baseinfo.certno, card.baseinfo.psn_name);
                    dssfxx = CliUtils.ExecuteSql("sybdj", "cmd", sqlsfxx, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                }
                sqlsfxx = string.Format(@" select 1 from ybrysfxx where sfzh='{0}' and psn_idet_type='{1}' ", card.baseinfo.certno, "2302");
                dssfxx = CliUtils.ExecuteSql("sybdj", "cmd", sqlsfxx, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                if (dssfxx.Tables[0].Rows.Count > 0)
                    //判断是否建档立卡人员
                    jdlkrybz = "1";
                else
                    jdlkrybz = "0";
                #endregion
                Idetinfo idinfo = card.idetinfo.Count == 0 ? new Idetinfo()
                {
                    begntime = "",
                    endtime = "",
                    memo = "",
                    psn_idet_type = "",
                    psn_type_lv = ""
                } : card.idetinfo[0];
                Frmcbxxxz cbxx = new Frmcbxxxz(card.baseinfo.certno);
                cbxx.ShowDialog();
                if (string.IsNullOrEmpty(cbxx.RYCBXX))
                {
                    return new object[] { 0, 0, "未选择人员参保信息！" };
                }
                string[] rycbxx = cbxx.RYCBXX.Split('|');
                balc = rycbxx[0];
                insutype = rycbxx[1];
                psn_type = rycbxx[2];
                psn_insu_stas = rycbxx[3];
                psn_insu_date = rycbxx[4];
                paus_insu_date = rycbxx[5];
                cvlserv_flag = rycbxx[6];
                insuplc_admdvs = rycbxx[7];
                emp_name = rycbxx[8];
                cbxx.Dispose();
                if (!insuplc_admdvs.Substring(0, 4).Equals("3213"))
                    ydrybz = "1";
                if (i > 0)
                {
                    WriteLog(sysdate + "  人员信息获取|" + ErrStr.ToString());
                    List<string> liSQL = new List<string>();
                    string strSql = string.Format(@" delete from ybickxx where grbh='{0}'
                            insert into ybickxx(grbh,zjlx,gmsfhm,xm,xb,mz,csrq,nl,zhye,xzlx,yldylb,gwybz,tcqh,dwmc,rysflb,rylbdj,memo,kssj,jssj,jzpzlx,jzpzbh, ylrylb, sjnl, ydrybz,sfjdlkry) values 
                            ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}','{20}','{21}','{22}','{23}','{24}') ",
                        card.baseinfo.psn_no, card.baseinfo.psn_cert_type, card.baseinfo.certno, card.baseinfo.psn_name, card.baseinfo.gend,
                        card.baseinfo.naty, card.baseinfo.brdy, card.baseinfo.age, balc, insutype,
                        psn_type, cvlserv_flag, insuplc_admdvs, emp_name, idinfo.psn_idet_type,
                        idinfo.psn_type_lv, idinfo.memo, idinfo.begntime, idinfo.endtime, mdtrt_cert_type, mdtrt_cert_no, psn_type,
                         card.baseinfo.age, ydrybz, jdlkrybz);
                    liSQL.Add(strSql);
                    obj = liSQL.ToArray();
                    obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);
                    if (obj[1].ToString().Equals("1"))
                    {
                        string jflx = string.Empty;
                        string[] sV = { "11", "12", "13" };
                        if (psn_type.Length > 1)
                        {
                            if (sV.Contains(psn_type.Substring(0, 2)))
                                jflx = "1302"; //职工医保
                            else
                                jflx = "1303"; //居民医保
                        }
                        if (jdlkrybz.Equals("1"))
                            jflx = "08";
                        string sqlRyxx = string.Format(@" select bzname from bztbd where bzcodn='psn_type' and bzkeyx='{0}' ", psn_type);
                        DataSet dsCbxx = CliUtils.ExecuteSql("sybdj", "cmd", sqlRyxx, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                        if (dsCbxx.Tables[0].Rows.Count > 0)
                        {
                            psn_type = dsCbxx.Tables[0].Rows[0]["bzname"].ToString();
                        }
                        string strParam = card.baseinfo.psn_no + "|" + "" + "|" + card.baseinfo.certno + "|" + card.baseinfo.psn_name + "|" + card.baseinfo.gend + "|" +
                                   card.baseinfo.naty + "|" + card.baseinfo.brdy + "|" + "" + "|" + psn_type + "|" + "正常" + "|" +
                                  ydrybz + "|" + insuplc_admdvs + "|" + DateTime.Now.ToString("yyyy") + "|" + "0" + "|" + balc + "|" +
                                  "|||||||||" + "0" + "|" +
                                  emp_name + "|" + card.baseinfo.age + "||" + YBJGBH + "|" + jflx + "|" +
                                  "0" + "|" + "0" + "|" + "0" + "|" + "0" + "|" + "0" + "|" +
                                  "0" + "|" + "0" + "|" + "0" + "|" + "0" + "|" + "0" + "|" +
                                  "0" + "|" + "0" + "|" + "0" + "|" + "0" + "|" + "0" + "|" +
                                  "0" + "|" + "0" + "|" + "0" + "|" + "0" + "|" + "0" + "|" +
                                  "" + "|" + "" + "|" + "" + "|" + "" + "|" + "" + "|" + "" + "|" + jdlkrybz;
                        WriteLog(sysdate + "  人员信息获取出参：" + strParam);
                        return new object[] { 0, 1, strParam };
                    }
                    else
                    {
                        WriteLog(sysdate + "  人员信息获取失败|" + obj[2].ToString());
                        return new object[] { 0, 0, obj[2].ToString() };
                    }
                }
                else
                {
                    return new object[] { 0, 0, ErrStr };
                }
            }
            else
            {
                WriteLog(sysdate + "  人员信息获取获取失败|" + ErrStr.ToString());
                return new object[] { 0, 0, ErrStr.ToString() };
            }
            //}
            //else
            //{
            //    WriteLog(sysdate + "  读卡失败|" + objDK[2].ToString());
            //    return new object[] { 0, 0, objDK[2].ToString() };
            //}

        }
        #endregion

        #region 住院登记
        public static object[] YBZYDJ(object[] objParam)
        {
            string sysdate = GetServerDateTime();
            WriteLog(sysdate + "  进入住院登记...");
            WriteLog("进入住院登记|HIS传参|" + string.Join(",", objParam));
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
                string tes = "0";
                if (objParam.Length > 11)
                {
                    tes = objParam[11].ToString(); //胎儿数
                }
                //string zszbm = objParam[10].ToString(); //准生证编号
                //string sylb = objParam[11].ToString();      //生育类别
                //string jhsylb = objParam[12].ToString();    //计划生育类别
                string ybjzlsh_snyd = "";
                string syfylb = ""; //生育类别
                string syfwzh = "";//生育服务证号
                string sysslb = "";//生育手术类别
                string yzs = "";//孕周数
                string tc = "";//胎次
                string sytes = "";//胎儿数
                string jhsyrq = "";//计划生育日期
                string wybz = "";//晚育标志
                string zcbz = "";//早产标志
                string ssdm = "";//手术代码
                string ssmc = "";//手术名称
                string bqno = "";
                string bqnm = "";
                string cwh = "";
                string yldymc = "";
                string xzlx = "";//险种类型
                string jzpzlx = "";//就诊凭证类型
                string jzpzbh = "";//就诊凭证编号
                string dgysdm1 = "";//定岗医生代码
                string dgysxm1 = "";//定岗医生名称
                string dgksdm = "";//定岗科室代码
                string dgksmc = "";//定岗科室名称

                #region 生育住院选择
                if (yllb.Equals("52"))
                {
                    Frm_ybmzysdj mzys = new Frm_ybmzysdj();
                    mzys.ShowDialog();
                    sysslb = mzys.sslb;
                    jhsyrq = mzys.ssrq;
                    syfylb = mzys.sylb;
                    yzs = mzys.yzs;
                    wybz = mzys.wybz;
                    zcbz = mzys.zcbz;
                    if (string.IsNullOrEmpty(sysslb) || string.IsNullOrEmpty(syfylb))
                        return new object[] { 0, 0, "医疗类别为【41-门诊生育】时，计划生育手术类别/日期不能空" };
                }
                #endregion

                if (objParam.Length > 13)
                {
                    syfylb = objParam[13].ToString(); //生育费用类别
                }

                if (objParam.Length > 14)
                {
                    syfwzh = objParam[14].ToString(); //生育服务证号
                }

                if (objParam.Length > 15)
                {
                    sysslb = objParam[15].ToString(); //生育手术类别
                }

                if (objParam.Length > 16)
                {
                    yzs = objParam[16].ToString();//孕周数
                }

                if (objParam.Length > 17)
                {
                    tc = objParam[17].ToString(); //胎次
                }

                if (objParam.Length > 18)
                {
                    sytes = objParam[18].ToString(); //胎儿数
                }

                if (objParam.Length > 19)
                {
                    if (!string.IsNullOrEmpty(objParam[19].ToString()))
                        jhsyrq = objParam[19].ToString().Substring(0, 4) + "-" + objParam[19].ToString().Substring(4, 2) + "-" + objParam[19].ToString().Substring(6, 2);
                    else
                        jhsyrq = objParam[19].ToString(); //计划生育日期
                }

                if (objParam.Length > 20)
                {
                    wybz = objParam[20].ToString(); //晚育标志
                }

                if (objParam.Length > 21)
                {
                    zcbz = objParam[21].ToString(); //早产标志
                }

                if (objParam.Length > 22)
                {
                    ssdm = objParam[22].ToString(); //手术代码
                }

                if (objParam.Length > 23)
                {
                    ssmc = objParam[23].ToString(); //手术名称
                }

                if (objParam.Length > 24)
                {
                    bqno = objParam[24].ToString(); //病区编号
                }

                if (objParam.Length > 25)
                {
                    bqnm = objParam[25].ToString(); //病区名称
                }

                if (objParam.Length > 26)
                {
                    cwh = objParam[26].ToString(); //床位号
                }


                #endregion

                #region 获取医生
                if (string.IsNullOrEmpty(dgysdm))
                {
                    string ysstrSql = string.Format(@"select z1ysno from zy01d where z1zyno='{0}' and (z1endv like '2%' or z1endv like '8%') ", jzlsh);
                    DataSet dsDGYS = CliUtils.ExecuteSql("sybdj", "cmd", ysstrSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                    if (dsDGYS.Tables[0].Rows.Count > 0)
                    {
                        dgysdm = dsDGYS.Tables[0].Rows[0]["z1ysno"].ToString();
                    }
                }
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
                if (ydrybz == "1")
                {
                    DQJBBZ = "2";
                }
                else
                    DQJBBZ = "1";
                #endregion

                #region 判断是否已入院
                string strSql = string.Format("select z1date as rysj,z1hznm,z1ksno,z1ksnm,cast(floor(rand()*100) as int) as z1bedn,z1ryzd as ryzd,In_Diag_Code as ryzddm from zy01h where z1zyno = '{0}'", jzlsh);
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
                string ryzd = ds.Tables[0].Rows[0]["ryzd"].ToString();
                string ryzddm = ds.Tables[0].Rows[0]["ryzddm"].ToString();
                string rysj = Convert.ToDateTime(ds.Tables[0].Rows[0]["rysj"]).ToString("yyyy-MM-dd HH:mm:ss");
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

                #region 判断读卡表数据
                strSql = string.Format(@"select * from ybickxx where grbh='{0}'", grbh);
                ds.Tables.Clear();
                ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    xzlx = ds.Tables[0].Rows[0]["xzlx"].ToString();
                    jzpzlx = ds.Tables[0].Rows[0]["jzpzlx"].ToString();
                    jzpzbh = ds.Tables[0].Rows[0]["jzpzbh"].ToString();
                    yldylb = ds.Tables[0].Rows[0]["yldylb"].ToString();
                    insuplc_admdvs = ds.Tables[0].Rows[0]["tcqh"].ToString();//统筹区划
                }
                #endregion


                #region 获取定岗医生信息
                strSql = string.Format(@"select * from ybdgyszd where ysbm='{0}'", dgysdm);
                ds.Tables.Clear();
                ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    dgysdm1 = ds.Tables[0].Rows[0]["dgysbm"].ToString();
                    dgysxm1 = ds.Tables[0].Rows[0]["ysxm"].ToString();
                }
                else
                {
                    dgysdm1 = dgysdm;
                    dgysxm1 = dgysxm;
                }
                #endregion
                #region 获取对照科室信息
                strSql = string.Format(@"select * from ybkszd where ksdm='{0}'", ksbm);
                ds.Tables.Clear();
                ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    dgksdm = ds.Tables[0].Rows[0]["ybksdm"].ToString();
                    dgksmc = ds.Tables[0].Rows[0]["ybksmc"].ToString();
                }
                else
                {
                    dgksdm = "D99";
                    dgksmc = "其它";
                }
                #endregion

                #region 获取医疗待遇名称
                strSql = string.Format(@"select * from bztbd where bzcodn='dl' and bzmem1='{0}'", yldylb);
                ds.Tables.Clear();
                ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                if (ds.Tables[0].Rows.Count > 0)
                    yldymc = ds.Tables[0].Rows[0]["bzname"].ToString();
                #endregion

                #region 待遇封锁信息查询
                //string dqsj = Convert.ToDateTime(sysdate).ToString("yyyy-MM-dd HH:mm:ss");
                //待遇封锁信息查询         
                //object[] objdyfsxxcx = { grbh, xzlx, YLGHBH, yllb, dqsj, "", bzbm, bzmc, ssdm, ssmc, syfylb, sysslb };
                //objdyfsxxcx = YBYLDYFSXXCX(objdyfsxxcx);
                //if (objdyfsxxcx[1].ToString() != "1")
                //{
                //    DialogResult dr = MessageBox.Show(objdyfsxxcx[2].ToString() + ",是否继续？", "医疗待遇封锁信息异常", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                //    if (dr != DialogResult.Yes) return new object[] { 0, 0, "封锁待遇异常,您已取消继续登记" };
                //}
                #endregion
                string ybjzlshA = "";
                if (ydrybz.Equals("1"))
                    ybjzlshA = jzlsh + new Random().Next(100).ToString().PadLeft(4, '0');//异地同一住院号撤销后不能再次办理入院
                else
                    ybjzlshA = jzlsh;
                #region 入参
                string zydjJson = string.Empty;
                /*
                就诊信息（节点标识：mdtrtinfo）
                    人员编号
                    险种类型
                    联系人姓名
                    联系电话
                    开始时间
                    就诊凭证类型
                    就诊凭证编号
                    医疗类别
                    住院号
                    病历号
                    主治医生编码
                    主诊医师姓名
                    入院诊断描述
                    入院科室编码
                    入院科室名称
                    入院床位
                    住院主诊断代码
                    住院主诊断名称
                    主要病情描述
                    病种编码
                    病种名称
                    手术操作代码
                    手术操作名称
                    计划生育服务证号
                    生育类别
                    计划生育手术类别
                    晚育标志
                    孕周数
                    胎次
                    胎儿数
                    早产标志
                    计划生育手术或生育日期
                    病种类型代码
                 入院诊断信息（节点标识：diseinfo）
                    人员编号
                    诊断类别
                    主诊断标志
                    诊断排序号
                    诊断代码
                    诊断名称
                    入院病情
                    诊断科室
                    诊断医生编码
                    诊断医生姓名
                    诊断时间

                */
                dynamic mdtrtinfo = new
                {
                    psn_no = grbh,
                    insutype = xzlx,
                    coner_name = "",
                    tel = "",
                    begntime = rysj,
                    mdtrt_cert_type = jzpzlx,
                    mdtrt_cert_no = jzpzbh,
                    med_type = yllb,
                    ipt_no = ybjzlshA,
                    medrcdno = jzlsh,
                    atddr_no = dgysdm1,
                    chfpdr_name = dgysxm1,
                    adm_diag_dscr = bzmc,
                    adm_dept_codg = dgksdm,
                    adm_dept_name = dgksmc,
                    adm_bed = cwh,
                    //dscg_maindiag_code = ryzddm,
                    //dscg_maindiag_name = ryzd,
                    dscg_maindiag_code = bzbm,
                    dscg_maindiag_name = bzmc,
                    main_cond_dscr = "无",
                    dise_codg = bzbm,
                    dise_name = bzmc,
                    oprn_oprt_code = ssdm,
                    oprn_oprt_name = ssmc,
                    fpsc_no = syfwzh,
                    matn_type = syfylb,
                    birctrl_type = sysslb,
                    latechb_flag = wybz,
                    geso_val = yzs,
                    fetts = tc,
                    fetus_cnt = sytes,
                    pret_flag = zcbz,
                    birctrl_matn_date = jhsyrq,
                    dise_type_code = "",
                    insuplc_admdvs = insuplc_admdvs,
                    mdtrtarea_admvs = YBjyqh
                };
                List<dynamic> zydj_Diseinfos = new List<dynamic>();

                //diseinfo

                #region 获取所有诊断DataSet
                strSql = string.Format(@"with tmp as(
                select 
                m1mzno,
                case when m1xybz='Y' then '1' else '2' end zdlb,
                --case when m1xybz='Y' then '1' else '0' end zdbz,
                0 zdbz,
                m1xyzd,
                m1xynm,
                '' rybq,
                isnull(ybksmc,b2ejnm) ksmc,
                isnull(dgysbm,m1user) ysdm,
                isnull(ysxm,b1name) ysxm,
                m1date
                 from mza1dd 
                 left join bz01h on m1user=b1empn
                 left join bz02d on b2ejks=b1ksno
                 left join ybkszd on b1ksno=ksdm
                 left join ybdgyszd on ysbm=m1user
                 where isnull(m1xyzd,'')<>''
                 union all
                select 
                m1mzno,
                case when m1zybz='Y' then '3' else '4' end zdlb,
                --case when m1zybz='Y' then '1' else '0' end zdbz,
                0 zdbz,
                m1zyzd,
                m1zynm,
                '' rybq,
                isnull(ybksmc,b2ejnm) ksmc,
                isnull(dgysbm,m1user) ysdm,
                isnull(ysxm,b1name) ysxm,
                m1date
                 from mza1dd 
                 left join bz01h on m1user=b1empn
                 left join bz02d on b2ejks=b1ksno
                 left join ybkszd on b1ksno=ksdm
                 left join ybdgyszd on ysbm=m1user
                 where isnull(m1zyzd,'')<>''

                 ---------------------
                union all
				 select 
                z1zyno,
                '1' zdlb,
                1 zdbz,
                '{0}',
                '{1}',
                '' rybq,
                isnull(ybksmc,b2ejnm) ksmc,
                isnull(dgysbm,z1empn) ysdm,
                isnull(ysxm,b1name) ysxm,
                z1date
                 from zy01h 
                 left join bz01h on z1empn=b1empn
                 left join bz02d on b2ejks=b1ksno
                 left join ybkszd on b1ksno=ksdm
                 left join ybdgyszd on ysbm=z1empn
				 where z1zyno='{2}'
                 )select * from tmp where m1mzno='{2}'", bzbm, bzmc, jzlsh);
                ds.Tables.Clear();
                ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);

                #endregion
                bool zzdty = false;
                for (int index = 0; index < ds.Tables[0].Rows.Count; index++)
                {
                    string diag_type = ds.Tables[0].Rows[index]["zdlb"].ToString();
                    string maindiag_flag = ds.Tables[0].Rows[index]["zdbz"].ToString();
                    string diag_srt_no = (index + 1).ToString();
                    string diag_code = ds.Tables[0].Rows[index]["m1xyzd"].ToString();
                    string diag_name = ds.Tables[0].Rows[index]["m1xynm"].ToString();
                    string adm_cond = ds.Tables[0].Rows[index]["rybq"].ToString();
                    string diag_dept = ds.Tables[0].Rows[index]["ksmc"].ToString();
                    string dise_dor_no = ds.Tables[0].Rows[index]["ysdm"].ToString();
                    string dise_dor_name = ds.Tables[0].Rows[index]["ysxm"].ToString();
                    string diag_time = ds.Tables[0].Rows[index]["m1date"].ToString();

                    //if (diag_code.Equals(ryzddm))
                    //{
                    //    zzdty = true;
                    //    maindiag_flag = "1";
                    //}
                    //else
                    //{
                    //    if (index > 0)
                    //    {
                    //        maindiag_flag = "0";
                    //    }
                    //    else
                    //    {
                    //        if (!zzdty)
                    //            maindiag_flag = "1";
                    //    }
                    //}
                    //maindiag_flag = "0";
                    dynamic diseinfo = new
                    {
                        psn_no = grbh,
                        diag_type = diag_type,
                        maindiag_flag = maindiag_flag,
                        diag_srt_no = diag_srt_no,
                        diag_code = diag_code,
                        diag_name = diag_name,
                        adm_cond = adm_cond,
                        diag_dept = diag_dept,
                        dise_dor_no = dise_dor_no,
                        dise_dor_name = dise_dor_name,
                        diag_time = diag_time
                    };
                    zydj_Diseinfos.Add(diseinfo);
                }
                dynamic data = new
                {
                    mdtrtinfo = mdtrtinfo,
                    diseinfo = zydj_Diseinfos
                };

                zydjJson = JsonConvert.SerializeObject(data);
                #endregion
                string Err = string.Empty;
                #region 医保登记
                List<string> liSQL = new List<string>();
                WriteLog(sysdate + "  住院登记|入参Json|" + zydjJson);
                int i = YBServiceRequest("2401", data, ref Err);

                if (i != 1)
                {
                    WriteLog(sysdate + "  住院登记(市本级)失败|" + Err.ToString());
                    return new object[] { 0, 0, Err.ToString() };
                }
                #endregion
                string dataRs = Err.ToString();
                WriteLog(sysdate + "  住院登记(市本级)|出参Json|" + dataRs);
                JObject jtRet = JsonConvert.DeserializeObject(dataRs) as JObject; //获取响应数据json  
                string ybjzlsh = jtRet["result"]["mdtrt_id"].ToString();
                #region 数据操作
                strSql = string.Format(@"insert into ybmzzydjdr(
                                         jzlsh,jylsh,ybjzlsh,yllb,ghdjsj,bzbm,bzmc,ksbh,ksmc,ysdm,
                                         ysxm,ghf,jbr,xm,grbh,kh,yldylb,xb,tcqh,zhye,
                                         jzbz,ydrybz,dqjbbz,sysdate,ybjzlsh_snyd,zddm,zdmc,cwh,
                                         syfylb,syfwzh,sysslb,syyzs,sytc,tes,jhsyrq,ssczdm,ssczmc,wybz,zcbz) 
                                         values(
                                        '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}',
                                        '{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}',
                                        '{20}','{21}','{22}','{23}','{24}','{25}','{26}','{27}','{28}','{29}',
                                        '{30}','{31}','{32}','{33}','{34}','{35}','{36}','{37}','{38}')"
                                        , jzlsh, JYLSH, ybjzlsh, yllb, djsj, bzbm, bzmc, dgksdm, dgksmc, dgysdm1,
                                        dgysxm1, 0, CZYBH, xm, grbh, kh, yldylb, xb, ssqh, zhye,
                                        "z", ydrybz, DQJBBZ, sysdate, ybjzlsh_snyd, ryzddm, ryzd, cwh,
                                        syfylb, syfwzh, sysslb, yzs, tc, sytes, jhsyrq, ssdm, ssmc, wybz, zcbz);
                liSQL.Add(strSql);
                strSql = string.Format(@"update zy01h set z1rylb = '{0}', z1tcdq = '{1}', z1lyjg = '{2}', z1lynm = '{3}', z1ylno = '{4}'
                                         , z1ylnm = '{5}', z1bzno = '{6}', z1bznm = '{7}', z1ybno = '{8}',z1idno='{9}' where z1comp = '{10}' and z1zyno = '{11}'"
                                        , yldymc, ssqh, lyjgdm, lyjgmc, yllb, yllbmc, bzbm, bzmc, grbh, sfzh, CliUtils.fSiteCode, jzlsh);
                liSQL.Add(strSql);

                object[] obj = liSQL.ToArray();
                obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);
                if (obj[1].ToString() == "1")
                {
                    WriteLog(sysdate + "  住院登记(市本级)成功|本地数据操作成功|" + ybjzlsh);
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
                return new object[] { 0, 0, "Error:" + error.Message };
            }
        }
        #endregion

        #region 住院登记变更
        public static object[] YBZYDJBG(object[] objParam)
        {
            string sysdate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            WriteLog(sysdate + "  进入住院登记变更...");
            WriteLog("进入住院登记变更|HIS传参|" + string.Join(",", objParam));
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
                #region his参数
                string jbr = CliUtils.fUserName;   // 经办人姓名 
                string jzlsh = objParam[0].ToString(); //就诊流水号
                string yllb = objParam[1].ToString(); //医疗类别代码
                string bzbm = objParam[2].ToString(); //病种编码
                string bzmc = objParam[3].ToString(); //病种名称
                //string[] kxx = objParam[4].ToString().Split('|'); //读卡返回信息
                string lyjgdm = objParam[5].ToString();//来源机构代码
                string lyjgmc = objParam[6].ToString();//来源机构名称
                string yllbmc = objParam[7].ToString();//医疗类别名称
                string dgysdm = objParam[8].ToString(); //定岗医生代码
                string dgysxm = objParam[9].ToString(); //定岗医生姓名
                string ryrq = objParam[10].ToString(); //入院日期
                string tes = objParam[11].ToString(); //胎儿数
                string syfylb = objParam[13].ToString(); //生育费用类别
                string syfwzh = objParam[14].ToString(); //生育服务证号
                string sysslb = objParam[15].ToString(); //生育手术类别
                string yzs = objParam[16].ToString();//孕周数
                string tc = objParam[17].ToString(); //胎次
                string sytes = objParam[18].ToString(); //胎儿数
                string jhsyrq = "";//计划生育日期
                if (!string.IsNullOrEmpty(objParam[19].ToString()))
                    jhsyrq = objParam[19].ToString().Substring(0, 4) + "-" + objParam[19].ToString().Substring(4, 2) + "-" + objParam[19].ToString().Substring(6, 2);
                else
                    jhsyrq = objParam[19].ToString(); //计划生育日期
                string wybz = objParam[20].ToString(); //晚育标志           
                string zcbz = objParam[21].ToString(); //早产标志         
                string ssdm = objParam[22].ToString(); //手术代码
                string ssmc = objParam[23].ToString(); //手术名称
                string bqno = objParam[24].ToString(); //病区编号
                string bqnm = objParam[25].ToString(); //病区名称
                string cwh = objParam[26].ToString(); //床位号
                #endregion
                string xzlx = "";//险种类型
                string jzpzlx = "";//就诊凭证类型
                string jzpzbh = "";//就诊凭证编号
                string dgysdm1 = "";//定岗医生代码
                string dgysxm1 = "";//定岗医生名称
                string dgksdm = "";//定岗科室代码
                string dgksmc = "";//定岗科室名称
                string tcqh = "";
                if (string.IsNullOrEmpty(jzlsh))
                    return new object[] { 0, 0, "就诊流水号不能为空" };
                if (string.IsNullOrEmpty(yllb))
                    return new object[] { 0, 0, "医疗类别不能为空" };
                if (string.IsNullOrEmpty(bzbm))
                    return new object[] { 0, 0, "病种不能为空" };
                if (string.IsNullOrEmpty(dgysdm))
                    return new object[] { 0, 0, "医生信息不能为空" };
                #region 判断是否已入院
                string strSql = string.Format("select z1date as rysj,z1hznm,z1ksno,z1ksnm,cast(floor(rand()*100) as int) as z1bedn,z1ryzd as ryzd,In_Diag_Code as ryzddm from zy01h where z1zyno = '{0}'", jzlsh);
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
                string ryzd = ds.Tables[0].Rows[0]["ryzd"].ToString();
                string ryzddm = ds.Tables[0].Rows[0]["ryzddm"].ToString();
                string rysj = Convert.ToDateTime(ds.Tables[0].Rows[0]["rysj"]).ToString("yyyy-MM-dd HH:mm:ss");
                #endregion

                #region 判断是否已登记
                strSql = string.Format(@"select grbh,* from ybmzzydjdr where jzlsh='{0}' and cxbz=1", jzlsh);
                ds.Tables.Clear();
                ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                if (ds.Tables[0].Rows.Count == 0)
                {
                    WriteLog(sysdate + "  就诊流水号" + jzlsh + "没有进行医保住院登记");
                    return new object[] { 0, 0, "就诊流水号" + jzlsh + "没有进行医保住院登记" };
                }



                //获取数据 
                string ybjzlsh_snyd = ds.Tables[0].Rows[0]["ybjzlsh_snyd"].ToString();
                string ybjzlsh = ds.Tables[0].Rows[0]["ybjzlsh"].ToString();
                string grbh = ds.Tables[0].Rows[0]["grbh"].ToString();
                #endregion

                #region 读卡信息
                string ydrybz = "0";
                strSql = string.Format(@"select * from YBICKXX where grbh='{0}'", grbh);
                ds.Tables.Clear();
                ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    DataRow dr = ds.Tables[0].Rows[0];
                    grbh = dr["grbh"].ToString(); //个人编号
                    string dwbm = dr["DWBH"].ToString();  //单位编号
                    string sfzh = dr["GMSFHM"].ToString();  //身份证号
                    string xm = dr["XM"].ToString();  //姓名
                    string xb = dr["XB"].ToString();  //性别
                    string kh = dr["KH"].ToString();  //卡号
                    string yldylb = dr["YLDYLB"].ToString();  //医疗待遇类别
                    ydrybz = dr["YDRYBZ"].ToString();  //异地人员标志
                    string ssqh = dr["DQBH"].ToString();  //所属区号
                    string zhye = dr["GRZHYE"].ToString();  //帐户余额
                    xzlx = ds.Tables[0].Rows[0]["xzlx"].ToString();
                    jzpzlx = ds.Tables[0].Rows[0]["jzpzlx"].ToString();
                    jzpzbh = ds.Tables[0].Rows[0]["jzpzbh"].ToString();
                    tcqh = ds.Tables[0].Rows[0]["tcqh"].ToString();
                }
                else
                    return new object[] { 0, 0, "无读卡信息反馈" };

                #endregion

                #region 获取定岗医生信息
                strSql = string.Format(@"select * from ybdgyszd where ysbm='{0}'", dgysdm);
                ds.Tables.Clear();
                ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    dgysdm1 = ds.Tables[0].Rows[0]["dgysbm"].ToString();
                    dgysxm1 = ds.Tables[0].Rows[0]["dgysxm"].ToString();
                }
                else
                {
                    dgysdm1 = dgysdm;
                    dgysxm1 = dgysxm;
                }
                #endregion

                #region 获取对照科室信息
                strSql = string.Format(@"select * from ybkszd where ksdm='{0}'", ksbm);
                ds.Tables.Clear();
                ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    dgksdm = ds.Tables[0].Rows[0]["ybksdm"].ToString();
                    dgksmc = ds.Tables[0].Rows[0]["ybksmc"].ToString();
                }
                else
                {
                    dgksdm = ksbm;
                    dgksmc = ksmc;
                }
                #endregion

                #region 异地医保
                if (ydrybz == "1")
                    DQJBBZ = "2";
                else
                    DQJBBZ = "1";
                #endregion

                #region 入参
                string zydjbgJson = string.Empty;
                /*
                入院登记信息（节点标识：adminfo）
                    就诊ID
                    人员编号
                    联系人姓名
                    联系电话
                    开始时间
                    结束时间
                    就诊凭证类型
                    医疗类别
                    住院/门诊号
                    病历号
                    主治医生编码
                    主诊医师姓名
                    入院诊断描述
                    入院科室编码
                    入院科室名称
                    入院床位
                    住院主诊断代码
                    住院主诊断名称
                    主要病情描述
                    病种编码
                    病种名称
                    手术操作代码
                    手术操作名称
                    计划生育服务证号
                    生育类别
                    计划生育手术类别
                    晚育标志
                    孕周数
                    胎次
                    胎儿数
                    早产标志
                    计划生育手术或生育日期
                    病种类型代码

                 入院诊断信息（节点标识：diseinfo）
                    人员编号
                    诊断类别
                    主诊断标志
                    诊断排序号
                    诊断代码
                    诊断名称
                    入院病情
                    诊断科室
                    诊断医生编码
                    诊断医生姓名
                    诊断时间

                */
                List<dynamic> rydjbg_diseinfo = new List<dynamic>();
                dynamic adminfo = new
                {
                    mdtrt_id = ybjzlsh,
                    psn_no = grbh,
                    coner_name = "",
                    tel = "",
                    begntime = rysj,
                    endtime = "",
                    mdtrt_cert_type = jzpzlx,
                    med_type = yllb,
                    ipt_otp_no = jzlsh,
                    medrcdno = jzlsh,
                    atddr_no = dgysdm1,
                    chfpdr_name = dgysxm1,
                    adm_diag_dscr = bzbm,
                    adm_dept_codg = dgksdm,
                    adm_dept_name = dgksmc,
                    adm_bed = cwh,
                    dscg_maindiag_code = ryzddm,
                    dscg_maindiag_name = ryzd,
                    main_cond_dscr = "",
                    dise_codg = bzbm,
                    dise_name = bzmc,
                    oprn_oprt_code = ssdm,
                    oprn_oprt_name = ssmc,
                    fpsc_no = syfwzh,
                    matn_type = syfylb,
                    birctrl_type = sysslb,
                    latechb_flag = wybz,
                    geso_val = yzs,
                    fetts = tc,
                    fetus_cnt = tes,
                    pret_flag = zcbz,
                    birctrl_matn_date = jhsyrq,
                    dise_type_code = ""
                };


                //diseinfo

                #region 获取所有诊断DataSet
                strSql = string.Format(@"with tmp as(
                select 
                m1mzno,
                case when m1xybz='Y' then '1' else '2' end zdlb,
                case when m1xybz='Y' then '1' else '0' end zdbz,
                m1xyzd,
                m1xynm,
                '' rybq,
                isnull(ybksmc,b2ejnm) ksmc,
                isnull(dgysbm,m1user) ysdm,
                isnull(ysxm,b1name) ysxm,
                m1date
                 from mza1dd 
                 left join bz01h on m1user=b1empn
                 left join bz02d on b2ejks=b1ksno
                 left join ybkszd on b1ksno=ksdm
                 left join ybdgyszd on ysbm=m1user
                 where isnull(m1xyzd,'')<>''
                 union all
                select 
                m1mzno,
                case when m1zybz='Y' then '3' else '4' end zdlb,
                case when m1zybz='Y' then '1' else '0' end zdbz,
                m1zyzd,
                m1zynm,
                '' rybq,
                isnull(ybksmc,b2ejnm) ksmc,
                isnull(dgysbm,m1user) ysdm,
                isnull(ysxm,b1name) ysxm,
                m1date
                 from mza1dd 
                 left join bz01h on m1user=b1empn
                 left join bz02d on b2ejks=b1ksno
                 left join ybkszd on b1ksno=ksdm
                 left join ybdgyszd on ysbm=m1user
                 where isnull(m1zyzd,'')<>''
                 )select * from tmp where m1mzno='{0}'", jzlsh);
                ds.Tables.Clear();
                ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);

                #endregion

                for (int index = 0; index < ds.Tables[0].Rows.Count; index++)
                {
                    string diag_type = ds.Tables[0].Rows[index]["zdlb"].ToString();
                    string maindiag_flag = ds.Tables[0].Rows[index]["zdbz"].ToString();
                    string diag_srt_no = (index + 1).ToString();
                    string diag_code = ds.Tables[0].Rows[index]["m1xyzd"].ToString();
                    string diag_name = ds.Tables[0].Rows[index]["m1xynm"].ToString();
                    string adm_cond = ds.Tables[0].Rows[index]["rybq"].ToString();
                    string diag_dept = ds.Tables[0].Rows[index]["ksmc"].ToString();
                    string dise_dor_no = ds.Tables[0].Rows[index]["ysdm"].ToString();
                    string dise_dor_name = ds.Tables[0].Rows[index]["ysxm"].ToString();
                    string diag_time = ds.Tables[0].Rows[index]["m1date"].ToString();
                    dynamic diseinfo = new
                    {
                        mdtrt_id = ybjzlsh,
                        psn_no = grbh,
                        diag_type = diag_type,
                        maindiag_flag = maindiag_flag,
                        diag_srt_no = diag_srt_no,
                        diag_code = diag_code,
                        diag_name = diag_name,
                        adm_cond = adm_cond,
                        diag_dept = diag_dept,
                        dise_dor_no = dise_dor_no,
                        dise_dor_name = dise_dor_name,
                        diag_time = diag_time
                    };
                    rydjbg_diseinfo.Add(diseinfo);
                }
                dynamic input = new
                {
                    adminfo = adminfo,
                    diseinfo = rydjbg_diseinfo
                };
                //zydj2401

                zydjbgJson = JsonConvert.SerializeObject(input);
                #endregion
                string Err = string.Empty;
                #region 住院登记变更
                List<string> liSQL = new List<string>();
                WriteLog(sysdate + "  住院登记变更|入参Json|" + zydjbgJson);
                int i = YBServiceRequest("2403", input, ref Err);
                if (i != 1)
                {
                    WriteLog(sysdate + "  住院登记变更失败|" + Err.ToString());
                    return new object[] { 0, 0, "  住院登记变更失败|" + Err.ToString() };
                }
                string dataRs = Err.ToString();
                WriteLog(sysdate + "  住院登记变更|出参Json|" + dataRs);
                JObject jobj = JsonConvert.DeserializeObject(dataRs) as JObject; //获取响应数据json 
                #endregion

                #region 数据操作
                strSql = string.Format(@"update ybmzzydjdr set yllb='{1}', bzbm='{2}',bzmc='{3}',dgysxm='{4}',dgysdm='{5}',ksbh='{6}',ksmc='{7}',tes='{8}' ,cwh='{9}',syfylb='{10}',syfwzh='{11}',sysslb='{12}',syyzs='{13}',sytc='{14}',jhsyrq='{15}',ssczdm='{16}',ssczmc='{17}',wybz='{18}',zcbz='{19}'
                                         where jzlsh='{0}' and cxbz=1", jzlsh, yllb, bzbm, bzmc, dgysxm1, dgysdm1, dgksdm, dgksmc, tes, cwh, syfylb, syfwzh, sysslb, yzs, tc, jhsyrq, ssdm, ssmc, wybz, zcbz);
                liSQL.Add(strSql);
                strSql = string.Format(@"update zy01h set  z1ylno = '{2}', z1ylnm = '{3}', z1bzno = '{4}', z1bznm = '{5}' 
                                        where z1comp = '{0}' and z1zyno = '{1}'", CliUtils.fSiteCode, jzlsh, yllb, yllbmc, bzbm, bzmc, cwh);
                liSQL.Add(strSql);
                object[] obj = liSQL.ToArray();
                obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);
                if (obj[1].ToString() == "1")
                {
                    WriteLog(sysdate + "  住院登记变更成功|本地数据操作成功|");
                    return new object[] { 0, 1, "住院登记变更成功" };
                }
                else
                {
                    WriteLog(sysdate + "  住院登记变更成功|本地数据操作失败|" + obj[2].ToString());
                    return new object[] { 0, 0, "住院登记变更成功|本地数据操作失败|" + obj[2].ToString() };
                }
                #endregion

            }
            catch (Exception ex)
            {
                WriteLog(sysdate + "  住院登记变更|系统异常|" + ex.Message);
                return new object[] { 0, 2, "Error:" + ex.Message };
            }
        }
        #endregion

        #region 住院登记撤销
        public static object[] YBZYDJCX(object[] objParam)
        {
            string sysdate = GetServerDateTime();
            WriteLog(sysdate + "  进入住院登记撤销...");
            WriteLog("进入住院登记撤销|HIS传参|" + string.Join(",", objParam));
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
                string jbr = CliUtils.fLoginUser;
                string jysj = Convert.ToDateTime(sysdate).ToString("yyyyMMddHHmmss");
                //交易流水号 

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

                //#region 出院登记
                //取消出院登记           
                object[] objQXCYDJ = { ybjzlsh, grbh };
                objQXCYDJ = YBCYCX(objQXCYDJ);
                //#endregion
                #region 入参
                string djcxJson = string.Empty;
                /*
                 * 输入（节点标识：data）
                 就诊ID
                 人员编号
                 */
                dynamic data = new
                {
                    data = new
                    {
                        mdtrt_id = ybjzlsh,
                        psn_no = grbh
                    }
                };
                djcxJson = JsonConvert.SerializeObject(data);
                #endregion

                string Err = string.Empty;

                WriteLog(sysdate + "  住院登记撤销|入参|" + djcxJson);
                int i = YBServiceRequest("2404", data, ref Err);
                if (i == 1)
                {
                    string dataRs = Err.ToString();
                    WriteLog(sysdate + "  住院登记撤销|出参Json|" + dataRs);
                    //JObject jobj = JsonConvert.DeserializeObject(dataRs) as JObject; //获取响应数据json

                    List<string> liSql = new List<string>();
                    WriteLog(sysdate + "住院登记撤销|出参|" + Err.ToString());

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
                        WriteLog(sysdate + "  住院登记撤销成功|本地数据操作成功|");
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
                    WriteLog(sysdate + "  住院登记撤销失败|" + Err.ToString());
                    return new object[] { 0, 0, "  住院登记撤销失败|" + Err.ToString() };
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
            string sysdate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            WriteLog(sysdate + "  进入住院费用上传...");
            WriteLog("进入住院费用上传|HIS传参|" + string.Join(",", objParam));
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
                string scrow = "100";
                if (objParam.Length > 2)
                    scrow = objParam[2].ToString();  //上传条数
                string cfrqbz = "0";
                if (objParam.Length > 3)
                    cfrqbz = objParam[3].ToString(); //上传处方日期标志 0-正常处方日期，1-出院日期,2当前时间

                CZYBH = CliUtils.fLoginUser;  //操作员工号
                string jbr = CliUtils.fUserName;
                string cfsj = Convert.ToDateTime(sysdate).ToString("yyyyMMddHHmmss");
                string ztjssj1 = "";

                if (!string.IsNullOrEmpty(ztjssj))
                {
                    ztjssj1 = Convert.ToDateTime(ztjssj).ToString("yyyy-MM-dd HH:mm:ss");
                }

                #region 判断是否医保登记
                string strSql = string.Format("select a.*,b.z1ldat,b.z1date,b.z1lynm,z1cwid,z1bqxx,z1bqnm from ybmzzydjdr a left join zy01h b on a.jzlsh=b.z1zyno where a.jzlsh = '{0}' and jzbz='z' and a.cxbz = 1", jzlsh);
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
                string soldSystem = dr1["ybzl"].ToString();
                string cyrq = dr1["z1ldat"].ToString();
                string oldryrq = dr1["z1date"].ToString();
                string cwh = dr1["cwh"].ToString();
                string yllb = dr1["yllb"].ToString();
                string bzbm = dr1["bzbm"].ToString();
                string bzmc = dr1["bzmc"].ToString();
                string z1lynm = dr1["z1lynm"].ToString();
                string z1cwid = dr1["z1cwid"].ToString();
                string z1bqxx = dr1["z1bqxx"].ToString();
                string z1bqnm = dr1["z1bqnm"].ToString();
                string ydrybz = dr1["ydrybz"].ToString();  //异地人员标志
                if (cfrqbz.Equals("1") && string.IsNullOrEmpty(cyrq))
                    return new object[] { 0, 0, "患者未出院不能按出院日期上传费用明细" };
                #endregion

                List<string> liSQL = new List<string>();
                #region 获取费用明细信息

                string sWhere = "";
                if (!string.IsNullOrEmpty(ztjssj))
                    sWhere = string.Format(@"and Convert(datetime,z3date)<'{0}' ", ztjssj1);
                //                strSql = string.Format(@"select y.ybxmbh, y.ybxmmc, a.z3djxx as dj,a.z3sequ as pch
                //                ,case left(a.z3endv, 1) when '4' then a.z3tseq  else '' end as tpch
                //                ,case left(a.z3endv, 1) when '4' then -a.z3jzcs else a.z3jzcs end as sl
                //                ,a.z3jzje as je
                //                , a.z3item as yyxmbh, a.z3name as yyxmmc, min(isnull(dgysbm,z3empn)) as ysdm, min(isnull(ysxm,a.z3kdys)) as ysxm
                //				,min(isnull(ybksdm,z3ksno)) as ksdm, min(isnull(ybksmc,b2ejnm)) as ksmc
                //                , z3sfno as sfno, y.sfxmzldm as ybsfxmzldm, y.sflbdm as ybsflbdm,max(a.z3date) as yysj,y.sfxmdjdm,y.gg,y.dw,y.dffbz,y.cysm,y.clsm
                //                from zy03d a 
                //                left join ybhisdzdrnew y on a.z3item = y.hisxmbh 
                //                left join yp01h h on a.z3item=y1ypno
                //				left join bz02d on b2ejks=z3ksno
                //				left join ybdgyszd on ysbm=z3empn
                //				left join ybkszd on ksdm=z3ksno
                //                where a.z3ybup is null and left(a.z3kind, 1) in ('2', '4') and isnull(a.z3jshx,'')=''  and a.z3zyno = '{0}'  {1}
                //                group by y.ybxmbh, y.ybxmmc, a.z3djxx,a.z3djxx, a.z3item, a.z3name,a.z3sfno,y.sfxmzldm, y.sflbdm,y.sfxmdjdm,y.gg,y.dw,
                //                (case left(a.z3endv, 1) when '4' then -a.z3jzcs else a.z3jzcs end),a.z3jzje,
                //                (case left(a.z3endv, 1) when '4' then a.z3tseq  else '' end),z3sequ,y.dffbz,y.cysm,y.clsm ", jzlsh, sWhere);

                strSql = string.Format(@" select y.ybxmbh, y.ybxmmc, a.z3djxx as dj,max(z3sequ) as pch
                ,'B' as tpch
                ,sum(case left(a.z3endv, 1) when '4' then -a.z3jzcs else a.z3jzcs end) as sl
                ,sum(case left(a.z3endv, 1) when '4' then -a.z3jzcs* a.z3djxx else a.z3jzcs*z3djxx end) as je
                , a.z3item as yyxmbh, a.z3name as yyxmmc, min(isnull(dgysbm,z3empn)) as ysdm, min(isnull(ysxm,a.z3kdys)) as ysxm
				,min(isnull(ybksdm,z3ksno)) as ksdm, min(isnull(ybksmc,b2ejnm)) as ksmc
                , z3sfno as sfno, y.sfxmzldm as ybsfxmzldm, y.sflbdm as ybsflbdm,max(a.z3date) as yysj,y.sfxmdjdm,y.gg,y.dw,y.dffbz,y.cysm,y.clsm
                from zy03d a 
                left join ybhisdzdrnew y on a.z3item = y.hisxmbh 
                left join yp01h h on a.z3item=y1ypno
				left join bz02d on b2ejks=z3ksno
				left join ybdgyszd on ysbm=z3empn
				left join ybkszd on ksdm=z3ksno
                where a.z3ybup is null and left(a.z3kind, 1) in ('2', '4') and isnull(a.z3jshx,'')=''  and a.z3zyno = '{0}' {1}	 
                group by y.ybxmbh, y.ybxmmc, a.z3djxx,a.z3item, a.z3name,a.z3sfno,y.sfxmzldm, y.sflbdm,y.sfxmdjdm,y.gg,y.dw,
                y.dffbz,y.cysm,y.clsm
				having sum(case left(a.z3endv, 1) when '4' then -a.z3jzcs else a.z3jzcs end) >0 
union all
select y.ybxmbh, y.ybxmmc, a.z3djxx as dj,max(z3sequ) as pch
                ,'B' as tpch
                ,sum(case left(a.z3endv, 1) when '4' then -a.z3jzcs else a.z3jzcs end) as sl
                ,sum(case left(a.z3endv, 1) when '4' then -a.z3jzcs* a.z3djxx else a.z3jzcs*z3djxx end) as je
                , a.z3item as yyxmbh, a.z3name as yyxmmc, min(isnull(dgysbm,z3empn)) as ysdm, min(isnull(ysxm,a.z3kdys)) as ysxm
    ,min(isnull(ybksdm,z3ksno)) as ksdm, min(isnull(ybksmc,b2ejnm)) as ksmc
                , z3sfno as sfno, y.sfxmzldm as ybsfxmzldm, y.sflbdm as ybsflbdm,max(a.z3date) as yysj,y.sfxmdjdm,y.gg,y.dw,y.dffbz,y.cysm,y.clsm
                from zy03dz a 
                left join ybhisdzdrnew y on a.z3item = y.hisxmbh 
                left join yp01h h on a.z3item=y1ypno
    left join bz02d on b2ejks=z3ksno
    left join ybdgyszd on ysbm=z3empn
    left join ybkszd on ksdm=z3ksno
                where a.z3ybup is null and left(a.z3kind, 1) in ('2', '4') and isnull(a.z3jshx,'')=''  and a.z3zyno =  '{0}' {1}	
                group by y.ybxmbh, y.ybxmmc, a.z3djxx,a.z3item, a.z3name,a.z3sfno,y.sfxmzldm, y.sflbdm,y.sfxmdjdm,y.gg,y.dw,
                y.dffbz,y.cysm,y.clsm
    having sum(case left(a.z3endv, 1) when '4' then -a.z3jzcs else a.z3jzcs end) >0", jzlsh, sWhere);

                ds.Tables.Clear();
                ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);

                List<string> li_inputData = new List<string>();
                List<string> liyyxmbh = new List<string>();
                List<string> liyyxmmc = new List<string>();
                List<string> liybxmbm = new List<string>();
                List<string> liybxmmc = new List<string>();
                List<string> liybsflb = new List<string>();
                List<dynamic> feedetails = new List<dynamic>();
                if (ds.Tables[0].Rows.Count > 0)
                {
                    StringBuilder strMsg = new StringBuilder();
                    int index = 1;
                    int m=0;
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
                            string pch = dr["pch"].ToString();
                            string tpch = dr["tpch"].ToString();
                            string dffbz = dr["dffbz"].ToString();
                            double dj = Convert.ToDouble(dr["dj"]);
                            double sl = Convert.ToDouble(dr["sl"]);
                            double je = Convert.ToDouble(dr["je"]);
                            string fysj = Convert.ToDateTime(dr["yysj"].ToString()).ToString("yyyy-MM-dd HH:mm:ss");
                            string gg = dr["gg"].ToString();
                            string jx = "";
                            string jldw = dr["dw"].ToString();
                            string dqysdm = dr["ysdm"].ToString();
                            string dqysxm = dr["ysxm"].ToString();
                            string dqksdm = dr["ksdm"].ToString();
                            string dqksmc = dr["ksmc"].ToString();
                            string yfyl = "";
                            string ybcfh = jzlsh.Substring(4, 4) + DateTime.Now.ToString("yyMMddHHmmss") + index.ToString().PadLeft(3, '0');
                            string ypjldw = dr["dw"].ToString();
                            decimal mcyl = 1;
                            string pd = "qd";
                            string yf = "";
                            string dw = "";
                            string txts = "";
                            string sfxmxm = "";//收费项目新码
                            string hcxm = "";//耗材新码
                            string zcffbz1 = "1";
                            string sfzybfwn = "";
                            string cysm = Convert.ToString(dr["cysm"]);
                            string clsm = Convert.ToString(dr["clsm"]);
                            //string lb = dr["lb"].ToString();
                            liyyxmbh.Add(yyxmbh);
                            liyyxmmc.Add(yyxmmc);
                            liybxmbm.Add(ybxmbh);
                            liybxmmc.Add(ybxmmc);
                            liybsflb.Add(sflb);
                            index++;
                            


                            #region 入参
                            /*
                            （节点标识：feedetail）
                                费用明细流水号
                                原费用流水号
                                就诊ID
                                医嘱号
                                人员编号
                                医疗类别
                                费用发生时间
                                医疗目录编码
                                医药机构目录编码
                                明细项目费用总额
                                数量
                                单价
                                开单科室编码
                                开单科室名称
                                开单医生编码
                                开单医师姓名
                                受单科室编码
                                受单科室名称
                                受单医生编码
                                受单医生姓名
                                医院审批标志
                                中药使用方式
                                外检标志
                                外检医院编码
                                出院带药标志
                                生育费用标志
                                备注

                             */
                            dynamic zyExp = new
                            {
                                mcs_prov_code = clsm,
                                tcmherb_prov_code = cysm,
                            };

                            string seqNo = DateTime.Now.ToString("mmssfff");
                            dynamic zysfdj_Feedetail = new
                            {
                                feedetl_sn = pch + seqNo,
                                init_feedetl_sn = sl < 0 ? tpch : "",
                                mdtrt_id = ybjzlsh,
                                drord_no = "",
                                psn_no = grbh,
                                med_type = yllb,
                                fee_ocur_time = fysj,
                                med_list_codg = ybxmbh,//yyxmbh
                                medins_list_codg = yyxmbh,
                                det_item_fee_sumamt = je.ToString(),
                                cnt = sl.ToString(),
                                pric = dj.ToString(),
                                bilg_dept_codg = dqksdm,
                                bilg_dept_name = dqksmc,
                                bilg_dr_codg = dqysdm,
                                bilg_dr_name = dqysxm,
                                acord_dept_codg = "",
                                acord_dept_name = "",
                                orders_dr_code = "",
                                orders_dr_name = "",
                                hosp_appr_flag = "1",
                                tcmdrug_used_way = dffbz,
                                etip_flag = "",
                                etip_hosp_code = "",
                                dscg_tkdrug_flag = "",
                                matn_fee_flag = "",
                                memo = "",
                                expContent = zyExp
                            };

                            feedetails.Add(zysfdj_Feedetail);
                            #region 补差操作
                            string byzje = ds.Tables[0].Compute("sum(jzje)", "true").ToString();
                            string ybzje = ds.Tables[0].Compute("sum(je)", "true").ToString();
                            decimal sfje = Math.Round(decimal.Parse(byzje), 4);
                            decimal scfy = Math.Round(decimal.Parse(ybzje), 4);
                            decimal xcje = Math.Round(sfje - scfy, 4);
                            string cxreq = DateTime.Now.ToString("ddHHmmssfff");
                            if ((Math.Abs(Math.Round((sfje - scfy), 2, MidpointRounding.AwayFromZero)) > 0) && (m != 9))
                            {
                                m = 9;
                                liyyxmbh.Add(yyxmbh);
                                liyyxmmc.Add(yyxmmc + "医保差额");
                                liybxmbm.Add(ybxmbh);
                                liybxmmc.Add(ybxmmc + "医保差额");
                                liybsflb.Add(sflb);
                                index++;
                                WriteLog(sysdate + "医保补差  |" + sfje + "|" + scfy + "|" + Math.Round((scfy - sfje)) + "|" + Math.Abs(Math.Round((scfy - sfje), 4)) + "m: " + m.ToString());
                                dynamic chaedy = new
                                {
                                    feedetl_sn = pch + cxreq,
                                    init_feedetl_sn = sl < 0 ? tpch : "",
                                    mdtrt_id = ybjzlsh,
                                    drord_no = "",
                                    psn_no = grbh,
                                    med_type = yllb,
                                    fee_ocur_time = fysj,
                                    med_list_codg = ybxmbh,//yyxmbh
                                    medins_list_codg = yyxmbh,
                                    det_item_fee_sumamt = xcje.ToString(),
                                    cnt = "1",
                                    pric = xcje.ToString(),
                                    bilg_dept_codg = dqksdm,
                                    bilg_dept_name = dqksmc,
                                    bilg_dr_codg = dqysdm,
                                    bilg_dr_name = dqysxm,
                                    acord_dept_codg = "",
                                    acord_dept_name = "",
                                    orders_dr_code = "",
                                    orders_dr_name = "",
                                    hosp_appr_flag = "1",
                                    tcmdrug_used_way = dffbz,
                                    etip_flag = "",
                                    etip_hosp_code = "",
                                    dscg_tkdrug_flag = "",
                                    matn_fee_flag = "",
                                    memo = "",
                                    expContent = zyExp
                                };

                                #region 记录上传费用信息
                                strSql = string.Format(@"insert into ybcfmxscindr(jzlsh,jylsh,xm,kh,ybjzlsh,cfrq,yysfxmbm,yysfxmmc,sfxmzxbm,sfxmzxmc,
                                                    dj,sl,je,jbr,sysdate,sflb,sfxmdj,dqxmxh,ybcfh) values(
                                                    '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}',
                                                    '{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}')",
                                                 jzlsh, JYLSH, xm, kh, ybjzlsh, fysj, yyxmbh, yyxmmc + "医保差额", ybxmbh, ybxmmc,
                                                  Math.Round((sfje - scfy), 4), 1, Math.Round((sfje - scfy), 4), jbr, sysdate, sflb, ybxmdj, index - 1, pch);
                                liSQL.Add(strSql);

                                feedetails.Add(chaedy);
                                #endregion
                            }
                            #endregion
                            strSql = string.Format(@"insert into ybcfmxscindr(jzlsh,jylsh,xm,kh,ybjzlsh,cfrq,yysfxmbm,yysfxmmc,sfxmzxbm,sfxmzxmc,
                                                    dj,sl,je,jbr,sysdate,sflb,sfxmdj,dqxmxh,ybcfh) values(
                                                    '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}',
                                                    '{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}')",
                                                  jzlsh, JYLSH, xm, kh, ybjzlsh, fysj, yyxmbh, yyxmmc, ybxmbh, ybxmmc,
                                                  dj, sl, je, jbr, sysdate, sflb, ybxmdj, index - 1, pch);
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
                List<dynamic> list_detail = new List<dynamic>();
                int iscRow = int.Parse(scrow); // 每交上传条数
                int iTemp = 0;
                int mxindex = 0;
                string Err = string.Empty;
                #region 分段上传
                foreach (dynamic inputData3 in feedetails)
                {
                    if (iTemp <= iscRow)
                    {
                        list_detail.Add(inputData3);
                        iTemp++;
                    }
                    else
                    {
                        if (ydrybz.Equals("1"))
                            JYLSH = YBJGBH + DateTime.Now.ToString("yyyyMMddHHmmss") + new Random().Next(100).ToString().PadLeft(4, '0');
                        WriteLog(sysdate + "  " + jzlsh + " 进入住院费用明细上传(分段)...");

                        string zysfdjJson = string.Empty;

                        dynamic input = new
                        {
                            feedetail = list_detail
                        };

                        zysfdjJson = JsonConvert.SerializeObject(input);
                        WriteLog(sysdate + "  住院费用上传(分段)|入参JSON|" + zysfdjJson);
                        int i = YBServiceRequest("2301", input, ref Err);
                        if (i != 1)
                        {
                            object[] objFYMXCX = { ybjzlsh, JYLSH, jbr, grbh, xm, kh, tcqh, ybjzlsh_snyd, DQJBBZ };
                            NYBZYCFMXSCCX(objFYMXCX);
                            WriteLog(sysdate + "  " + jzlsh + " 住院费用上传(分段)失败|" + Err.ToString());
                            return new object[] { 0, 0, "住院费用上传(分段)失败|" + Err.ToString() };
                        }
                        string dataRs = Err.ToString();
                        WriteLog(sysdate + "  住院费用上传(分段)|出参Json|" + dataRs);
                        JObject jobj = JsonConvert.DeserializeObject(dataRs) as JObject; //获取响应数据json

                        JToken jtRet = jobj["result"];
                        foreach (JToken jt in jtRet)
                        {
                            /*
                                费用明细流水号
                                明细项目费用总额
                                数量
                                单价
                                定价上限金额
                                自付比例
                                全自费金额
                                超限价金额
                                先行自付金额
                                符合政策范围金额
                                收费项目等级
                                医疗收费项目类别
                                基本药物标志
                                医保谈判药品标志
                                儿童用药标志
                                目录特项标志
                                限制使用标志
                                直报标志
                                备注
                             */
                            string fhjylsh = jt["feedetl_sn"].ToString();
                            string fhje = jt["det_item_fee_sumamt"].ToString();
                            string fhsl = jt["cnt"].ToString();
                            string fhdj = jt["pric"].ToString();
                            string fhdjsxje = jt["pric_uplmt_amt"].ToString();
                            string fhzfbl = jt["selfpay_prop"].ToString();
                            string fhqzfje = jt["fulamt_ownpay_amt"].ToString();
                            string fhcxjje = jt["overlmt_amt"].ToString();
                            string fhxxzfje = jt["preselfpay_amt"].ToString();
                            string fhfhzcfwnje = jt["inscp_scp_amt"].ToString();
                            string fhsfxmdj = jt["chrgitm_lv"].ToString();
                            string fhsfxmlb = jt["med_chrgitm_type"].ToString();
                            string fhjbywbs = jt["bas_medn_flag"].ToString();
                            string fhybtpypbz = jt["hi_nego_drug_flag"].ToString();
                            string fhetyybz = jt["chld_medc_flag"].ToString();
                            string fhmltxbz = jt["list_sp_item_flag"].ToString();
                            string fhxzsybz = jt["lmt_used_flag"].ToString();
                            string fhzbbz = jt["drt_reim_flag"].ToString();
                            string fhbz = jt["memo"].ToString();
                            strSql = string.Format(@"insert into ybcfmxscfhdr(jzlsh,jylsh,ybjzlsh,yyxmdm,yyxmmc,yybxmbh,ybxmmc,je,zlje,zfje,
                                                        cxjzfje,sflb,sfxmdj,qezfbz,zlbl,xj,bz,dqxmxh,ybcfh,dj,sl, djsxje, xxzfje, fhzcfwje,jjywbz,ybtbypbz,etyybz,mltsbz,zbbz) 
                                                        values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}',
                                                        '{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}','{20}','{21}','{22}','{23}','{24}','{25}','{26}','{27}','{28}')",
                                                     jzlsh, JYLSH, ybjzlsh, liyyxmbh[mxindex], liyyxmmc[mxindex], liybxmbm[mxindex], liybxmmc[mxindex], fhje, fhxxzfje, fhqzfje,
                                                     fhcxjje, fhsfxmlb, fhsfxmdj, fhxzsybz, fhzfbl, "", fhbz, mxindex + 1, fhjylsh, fhdj, fhsl, fhdjsxje, fhxxzfje, fhfhzcfwnje, fhjbywbs, fhybtpypbz, fhetyybz, fhmltxbz, fhzbbz);
                            liSQL.Add(strSql);
                            mxindex++;
                        }
                        iTemp = 1;
                        list_detail.Clear();
                        list_detail.Add(inputData3);
                    }
                }
                #endregion

                #region 明细不足100条时，一次性上传
                if (iTemp > 0)
                {
                    if (ydrybz.Equals("1"))
                        WriteLog(sysdate + "  " + jzlsh + " 住院费用上传(补传、一次性上传)...");
                    string zysfdjJson = string.Empty;
                    dynamic input = new
                    {
                        feedetail = list_detail
                    };
                    zysfdjJson = JsonConvert.SerializeObject(input);
                    WriteLog(sysdate + " 住院费用上传(补传、一次性上传)|入参|" + zysfdjJson);
                    int i = YBServiceRequest("2301", input, ref Err);
                    if (i != 1)
                    {
                        object[] objFYMXCX = { ybjzlsh, JYLSH, jbr, grbh, xm, kh, tcqh, ybjzlsh_snyd, DQJBBZ };
                        NYBZYCFMXSCCX(objFYMXCX);
                        WriteLog(sysdate + "  " + jzlsh + " 住院费用上传(补传、一次性上传)失败|" + Err.ToString());
                        return new object[] { 0, 0, "住院费用上传(补传、一次性上传)失败|" + Err.ToString() };

                    }
                    string dataRs = Err.ToString();
                    WriteLog(sysdate + "  住院费用上传(补传、一次性上传)|出参Json|" + dataRs);
                    JObject jobj = JsonConvert.DeserializeObject(dataRs) as JObject; //获取响应数据json 
                    JToken jtRet = jobj["result"];
                    foreach (JToken jt in jtRet)
                    {
                        /*
                            费用明细流水号
                            明细项目费用总额
                            数量
                            单价
                            定价上限金额
                            自付比例
                            全自费金额
                            超限价金额
                            先行自付金额
                            符合政策范围金额
                            收费项目等级
                            医疗收费项目类别
                            基本药物标志
                            医保谈判药品标志
                            儿童用药标志
                            目录特项标志
                            限制使用标志
                            直报标志
                            备注
                         */
                        string fhjylsh = jt["feedetl_sn"].ToString();
                        string fhje = jt["det_item_fee_sumamt"].ToString();
                        string fhsl = jt["cnt"].ToString();
                        string fhdj = jt["pric"].ToString();
                        string fhdjsxje = jt["pric_uplmt_amt"].ToString();
                        string fhzfbl = jt["selfpay_prop"].ToString();
                        string fhqzfje = jt["fulamt_ownpay_amt"].ToString();
                        string fhcxjje = jt["overlmt_amt"].ToString();
                        string fhxxzfje = jt["preselfpay_amt"].ToString();
                        string fhfhzcfwnje = jt["inscp_scp_amt"].ToString();
                        string fhsfxmdj = jt["chrgitm_lv"].ToString();
                        string fhsfxmlb = jt["med_chrgitm_type"].ToString();
                        string fhjbywbs = jt["bas_medn_flag"].ToString();
                        string fhybtpypbz = jt["hi_nego_drug_flag"].ToString();
                        string fhetyybz = jt["chld_medc_flag"].ToString();
                        string fhmltxbz = jt["list_sp_item_flag"].ToString();
                        string fhxzsybz = jt["lmt_used_flag"].ToString();
                        string fhzbbz = jt["drt_reim_flag"].ToString();
                        string fhbz = jt["memo"].ToString();
                        strSql = string.Format(@"insert into ybcfmxscfhdr(jzlsh,jylsh,ybjzlsh,yyxmdm,yyxmmc,yybxmbh,ybxmmc,je,zlje,zfje,
                                                        cxjzfje,sflb,sfxmdj,qezfbz,zlbl,xj,bz,dqxmxh,ybcfh,dj,sl, djsxje, xxzfje, fhzcfwje,jjywbz,ybtbypbz,etyybz,mltsbz,zbbz) 
                                                        values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}',
                                                        '{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}','{20}','{21}','{22}','{23}','{24}','{25}','{26}','{27}','{28}')",
                                                 jzlsh, JYLSH, ybjzlsh, liyyxmbh[mxindex], liyyxmmc[mxindex], liybxmbm[mxindex], liybxmmc[mxindex], fhje, fhxxzfje, fhqzfje,
                                                 fhcxjje, fhsfxmlb, fhsfxmdj, fhxzsybz, fhzfbl, "", fhbz, mxindex + 1, fhjylsh, fhdj, fhsl, fhdjsxje, fhxxzfje, fhfhzcfwnje, fhjbywbs, fhybtpypbz, fhetyybz, fhmltxbz, fhzbbz);
                        liSQL.Add(strSql);
                        mxindex++;
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
                    WriteLog(sysdate + "    住院费用上传成功|本地数据操作成功|");
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

        #region 住院费用登记撤销(内部)
        public static object[] NYBZYCFMXSCCX(object[] objParam)
        {
            string sysdate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            WriteLog(sysdate + "  进入住院费用登记撤销(内部)...");
            WriteLog("进入住院费用登记撤销(内部)|HIS传参|" + string.Join(",", objParam));
            try
            {
                CZYBH = CliUtils.fLoginUser; //操作员工号
                YWZQH = CliUtils.fLoginYbNo.Split('|')[0].ToString();    //业务周期号
                string ybjzlsh = objParam[0].ToString(); // 就诊流水号
                string cfmxjylsh = objParam[1].ToString();  //处方交易流水号
                string jbr = objParam[2].ToString();   // 经办人姓名
                string grbh = objParam[3].ToString();
                string xm = objParam[4].ToString();
                string kh = objParam[5].ToString();
                string dqbh = objParam[6].ToString();
                string ybjzlsh_sndy = objParam[7].ToString();
                DQJBBZ = objParam[8].ToString();

                string jysj = Convert.ToDateTime(sysdate).ToString("yyyyMMddHHmmss");
                //交易流水号 
                #region 入参
                string nbzyfydjcxJson = string.Empty;
                /*
                 * 输入（节点标识：data）
                 就诊ID
                 人员编号
                 */
                dynamic input = new
                {
                    data = new
                    {
                        feedetl_sn = "0000",
                        mdtrt_id = ybjzlsh,
                        psn_no = grbh
                    }
                };
                nbzyfydjcxJson = JsonConvert.SerializeObject(input);
                #endregion
                string Err = string.Empty;

                WriteLog(sysdate + "  住院费用登记撤销(内部)|入参Json|" + nbzyfydjcxJson);
                int i = YBServiceRequest("2302", input, ref Err);
                if (i == 1)
                {
                    string dataRs = Err.ToString();
                    WriteLog(sysdate + "  住院费用登记撤销(内部)|出参Json|" + dataRs);
                    JObject jobj = JsonConvert.DeserializeObject(dataRs) as JObject; //获取响应数据json

                    WriteLog(sysdate + "  住院费用登记撤销(内部)成功|" + Err.ToString());
                    return new object[] { 0, 1, "  住院费用登记撤销(内部)成功|" + Err.ToString() };
                }
                else
                {
                    WriteLog(sysdate + "  住院费用登记撤销(内部)失败|" + Err.ToString());
                    return new object[] { 0, 0, "  住院费用登记撤销(内部)失败|" + Err.ToString() };
                }
            }
            catch (Exception ex)
            {
                WriteLog(sysdate + "  住院费用登记撤销(内部)|系统异常" + ex.Message);
                return new object[] { 0, 0, "系统异常" + ex.Message };
            }
        }
        #endregion

        #region 住院费用登记撤销
        public static object[] YBZYSFDJCX(object[] objParam)
        {
            string sysdate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            WriteLog(sysdate + "  进入住院费用登记撤销...");
            WriteLog("进入住院费用登记撤销|HIS传参|" + string.Join(",", objParam));
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
                string jbr = CliUtils.fUserName;   // 经办人姓名
                string cfsj = Convert.ToDateTime(sysdate).ToString("yyyyMMddHHmmss");
                //结算情况下不能撤销
                string strSql = string.Format(@"select * from ybfyjsdr a where a.jzlsh = '{0}' and a.cxbz = 1 and ztjsbz=0", jzlsh);
                DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    WriteLog(sysdate + "该患者已做医保结算，请先撤销医保结算后再撤销费用明细");
                    return new object[] { 0, 0, "该患者已做医保结算，请先撤销医保结算后再撤销费用明细" };
                }

                //是否存在撤销明细
                strSql = string.Format(@"select * from ybcfmxscindr a where a.jzlsh = '{0}' and a.cxbz = 1", jzlsh);// and isnull(ybdjh,'')=''
                ds.Tables.Clear();
                ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                if (ds != null && ds.Tables[0].Rows.Count == 0)
                {
                    WriteLog(sysdate + "无费用撤销或已经撤销完成");
                    return new object[] { 0, 0, "无费用撤销或已经撤销完成" };
                }

                #region 医保登记信息
                strSql = string.Format("select * from ybmzzydjdr a where a.jzlsh = '{0}' and jzbz='z' and a.cxbz = 1", jzlsh);
                ds.Tables.Clear();
                ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                if (ds == null || ds.Tables[0].Rows.Count == 0)
                    return new object[] { 0, 0, "无医保登记记录" };
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

                #region 取消出院登记
                //取消出院登记，预防预结算出院登记后撤销处方明细           
                object[] objQXCYDJ = { ybjzlsh, grbh };
                objQXCYDJ = YBCYCX(objQXCYDJ);
                #endregion


                #region 入参
                string zyfydjcxJson = string.Empty;
                /*
                 * 输入（节点标识：data）
                 就诊ID
                 人员编号
                 */
                dynamic input = new
                {
                    data = new
                    {
                        feedetl_sn = "0000",
                        mdtrt_id = ybjzlsh,
                        psn_no = grbh
                    }
                };
                zyfydjcxJson = JsonConvert.SerializeObject(input);
                #endregion
                string Err = string.Empty;
                WriteLog(sysdate + "  住院费用登记撤销|入参JSON|" + zyfydjcxJson);
                int i = YBServiceRequest("2302", input, ref Err);
                if (i == 1)
                {
                    string dataRs = Err.ToString();
                    WriteLog(sysdate + "  住院费用登记撤销|出参Json|" + dataRs);
                    JObject jobj = JsonConvert.DeserializeObject(dataRs) as JObject; //获取响应数据json 
                    List<string> liSql = new List<string>();
                    strSql = string.Format(@"INSERT INTO [dbo].[ybcfmxscindr]
                   (jzlsh ,jylsh ,sfxmzl ,sflb ,ybcfh ,cfrq ,yysfxmbm ,sfxmzxbm  ,yysfxmmc
                   ,dj ,sl ,je ,jx ,gg ,mcyl ,sypc ,ysbm ,ysxm ,yf  ,dw ,ksbh
                   ,ksmc ,zxts  ,cydffbz ,jbr ,ypjldw ,qezfbz ,grbh ,xm ,kh ,jsdjh ,cxbz
                   ,sysdate ,ybjzlsh ,djlsh ,sfxmzxmc ,zfje ,cfscsj ,remark ,sfxmdj ,ybdjh
                   ,ybbz ,dqxmxh)
		           select jzlsh ,jylsh ,sfxmzl ,sflb ,ybcfh ,cfrq ,yysfxmbm ,sfxmzxbm  ,yysfxmmc
                   ,dj ,sl ,je ,jx ,gg ,mcyl ,sypc ,ysbm ,ysxm ,yf  ,dw ,ksbh
                   ,ksmc ,zxts  ,cydffbz ,jbr ,ypjldw ,qezfbz ,grbh ,xm ,kh ,jsdjh ,'0'
                   ,'{1}' ,ybjzlsh ,djlsh ,sfxmzxmc ,zfje ,cfscsj ,remark ,sfxmdj ,ybdjh
                   ,ybbz ,dqxmxh from ybcfmxscindr  where jzlsh = '{0}' and isnull(ybdjh,'')='' and cxbz = 1", jzlsh, sysdate);
                    liSql.Add(strSql);
                    strSql = string.Format(@"update ybcfmxscindr set cxbz='2' where jzlsh = '{0}' and isnull(ybdjh,'')=''  and cxbz = 1", jzlsh, sysdate);//
                    liSql.Add(strSql);
                    strSql = string.Format(@"INSERT INTO [dbo].[ybcfmxscfhdr]
                       (jzlsh,jylsh,ybjzlsh,yyxmdm,yyxmmc,yybxmbh,ybxmmc,je,zlje,zfje,
                    cxjzfje,sflb,sfxmdj,qezfbz,zlbl,xj,bz,dqxmxh,ybcfh,dj,sl, djsxje, xxzfje, fhzcfwje,jjywbz,ybtbypbz,etyybz,mltsbz,zbbz,sysdate,cxbz)
		            select jzlsh,jylsh,ybjzlsh,yyxmdm,yyxmmc,yybxmbh,ybxmmc,je,zlje,zfje,
                    cxjzfje,sflb,sfxmdj,qezfbz,zlbl,xj,bz,dqxmxh,ybcfh,dj,sl, djsxje, xxzfje, fhzcfwje,jjywbz,ybtbypbz,etyybz,mltsbz,zbbz,'{1}','0' from ybcfmxscfhdr where jzlsh = '{0}' and isnull(ybdjh,'')='' and cxbz = 1", jzlsh, sysdate);
                    liSql.Add(strSql);
                    strSql = string.Format(@"update ybcfmxscfhdr set cxbz='2' where jzlsh = '{0}'  and cxbz = 1", jzlsh, sysdate);//and isnull(ybdjh,'')=''
                    liSql.Add(strSql);
                    strSql = string.Format(@"update zy03d set z3ybup = null where z3ybup is not null and z3zyno = '{0}' ", jzlsh);//and isnull(z3jshx,'')=''
                    liSql.Add(strSql);

                    object[] obj = liSql.ToArray();
                    obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);
                    if (obj[1].ToString().Equals("1"))
                    {
                        WriteLog(sysdate + "  住院费用登记撤销成功|本地数据操作成功|");
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
                    WriteLog(sysdate + "  住院费用登记撤销失败|" + Err.ToString());
                    return new object[] { 0, 0, Err.ToString() };
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
            WriteLog(JsonConvert.SerializeObject(objParam));
            string sysdate = GetServerDateTime();
            WriteLog(sysdate + "  进入住院收费预结算...");
            WriteLog("进入住院收费预结算|HIS传参|" + string.Join(",", objParam));
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
                //string jzlsh = objParam[0].ToString();      // 就诊流水号
                //string cyyy = objParam[1].ToString();       //出院原因
                //string zhsybz = objParam[2].ToString();     //账户使用标志 （0或1）
                //string ztjsbz = objParam[3].ToString();     //中途结算标志
                //string jsrqsj = objParam[4].ToString();     //结算日期时间
                //string cyrqsj = objParam[5].ToString();     //出院日期时间
                //string sfje = objParam[6].ToString();       //收费金额                
                //string jsfs = objParam[7].ToString();       //结算方式
                string jzlsh = objParam[0].ToString();   // 就诊流水号
                string cyyy = objParam[1].ToString();    // 出院原因代码
                string zhsybz = objParam[2].ToString();  // 账户使用标志（0或1）
                string ztjsbz = objParam[3].ToString(); //中途结算标志
                string cyrqsj = objParam[4].ToString();//出院日期
                string jsrqsj = objParam[5].ToString();     //结算日期时间
                string sfje = objParam[6].ToString(); //医疗费合计 
                string jsfs = "01"; //结算方式 21-
                string cyzdbm = "";
                if (objParam.Length > 8)
                    cyzdbm = objParam[8].ToString();//出院诊断编码;
                string grbh = "";
                string yjfalx = "1";//月结方案类型
                string syfylb = "";//生育费用类别   
                string syfwzh = "";//生育服务证号
                string djh = "0000";
                string grzhye = "0.00";
                if (string.IsNullOrEmpty(jzlsh))
                    return new object[] { 0, 0, "就诊流水号不能为空" };
                if (string.IsNullOrEmpty(cyyy))
                    return new object[] { 0, 0, "出院原因不能为空" };
                //if (string.IsNullOrEmpty(jsfs))
                //    return new object[] { 0, 0, "泰州结算方式不能为空" };

                yjfalx = jsfs;
                WriteLog(sysdate + "  " + jzlsh + " 进入住院收费预结算...");
                CZYBH = CliUtils.fLoginUser;   // 操作员工号
                string jbr = CliUtils.fUserName;     // 经办人姓名
                string cyrq = "";
                string jsrq = Convert.ToDateTime(jsrqsj).ToString("yyyy-MM-dd");
                string dqrq = Convert.ToDateTime(sysdate).ToString("yyyyMMddHHmmss");  // 当前日期
                if (ztjsbz.Equals("1"))
                    cyrq = jsrq;
                else
                    cyrq = Convert.ToDateTime(cyrqsj).ToString("yyyy-MM-dd");

                //医院交易流水号 
                string fsxx = "1"; //1-普通住院 2-非普通住院
                #region 是否未办理医保登记
                string strSql = string.Format(@"select a.*,b.In_Diag_Code,b.z1ryzd from ybmzzydjdr a 
                    left join zy01h b on b.z1zyno=a.jzlsh 
                    where jzlsh='{0}' and cxbz=1", jzlsh);
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
                string tes = dr["tes"].ToString();
                syfylb = dr["syfylb"].ToString();
                syfwzh = dr["syfwzh"].ToString();

                #endregion

                if (string.IsNullOrEmpty(cyzdbm))
                {
                    cyzdbm = bzbm;
                }
                string strSql2 = string.Format(@"select dmmc from ybbzmrdr where dm='{0}'", cyzdbm);
                DataSet ds2 = CliUtils.ExecuteSql("sybdj", "cmd", strSql2, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                if (ds2.Tables[0].Rows.Count == 0)
                    return new object[] { 0, 0, "未找到" + cyzdbm + "特殊病种" };
                string cyzdmc = ds2.Tables[0].Rows[0]["dmmc"].ToString(); //出院诊断名称


                object[] objZYDK = YBZYDK(null);
                string dkgrbh = objZYDK[2].ToString().Split('|')[0].ToString();
                if (!string.Equals(dkgrbh, grbh))
                    return new object[] { 0, 0, "结算患者姓名和医保卡姓名不相符" };

                #region 获取读卡数据

                strSql = string.Format(@"select * from YBICKXX where grbh='{0}'", grbh);
                ds.Tables.Clear();
                ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                string jzpzlx = "";//就诊凭证类型
                string jzpzbh = "";//就诊凭证编号
                string xzlx = "";//险种类型
                if (ds.Tables[0].Rows.Count > 0)
                {
                    jzpzlx = ds.Tables[0].Rows[0]["jzpzlx"].ToString();
                    jzpzbh = ds.Tables[0].Rows[0]["jzpzbh"].ToString();
                    xzlx = ds.Tables[0].Rows[0]["xzlx"].ToString();
                    grzhye = ds.Tables[0].Rows[0]["GRZHYE"].ToString();
                    insuplc_admdvs = ds.Tables[0].Rows[0]["tcqh"].ToString();
                }
                if (jzpzbh.Contains('|'))
                    jzpzbh = jzpzbh.Split('|')[0];
                #endregion

                #region 是否费用上传
                string zfje = "0";
                string cxjzfje = "0";
                string xxzfje = "0";
                string fhzcfwje = "0";
                strSql = string.Format(@"select sum(isnull(zfje,0)) zfje,sum(isnull(cxjzfje,0)) cxjzfje,sum(isnull(xxzfje,0)) xxzfje,sum(isnull(fhzcfwje,0)) fhzcfwje from ybcfmxscfhdr where jzlsh='{0}' and cxbz=1", jzlsh);
                ds.Tables.Clear();
                ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                if (ds.Tables[0].Rows.Count == 0)
                    return new object[] { 0, 0, "该患者未进行费用上传" };
                zfje = ds.Tables[0].Rows[0]["zfje"].ToString();
                cxjzfje = ds.Tables[0].Rows[0]["cxjzfje"].ToString();
                xxzfje = ds.Tables[0].Rows[0]["xxzfje"].ToString();
                fhzcfwje = ds.Tables[0].Rows[0]["fhzcfwje"].ToString();
                #endregion

                #region 取医保总金额做总额
                string ybzje = "0";
                strSql = string.Format(@"select isnull(sum(isnull(je,0)),0) ybzje from ybcfmxscindr where jzlsh='{0}' and cxbz=1", jzlsh);
                ds.Tables.Clear();
                ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                ybzje = ds.Tables[0].Rows[0]["ybzje"].ToString();
                if (Math.Abs(double.Parse(sfje) - double.Parse(ybzje)) < 0.1)
                {
                    sfje = ybzje;
                }
                #endregion

                #region 是否已经医保结算
                strSql = string.Format(@"select * from ybfyjsdr where jzlsh='{0}' and ztjsbz=0 and cxbz=1", jzlsh);
                ds.Tables.Clear();
                ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                if (ds.Tables[0].Rows.Count > 0)
                    return new object[] { 0, 0, "该患者已办理医保结算" };
                #endregion

                #region 出院登记
                //取消出院登记           
                object[] objQXCYDJ = { ybjzlsh, grbh };
                objQXCYDJ = YBCYCX(objQXCYDJ);
                //出院登记
                object[] objCYDJ = { jzlsh, cyyy, cyrqsj, ybjzlsh };
                objCYDJ = YBCYBL(objCYDJ);
                if (objCYDJ[1].ToString() != "1")
                {
                    return new object[] { 0, 0, "出院登记失败！" + objCYDJ[2].ToString() };
                }
                #endregion

                #region 入参
                string zyyjsJson = string.Empty;
                /*
                 输入（节点标识：data）
                    人员编号
                    就诊凭证类型
                    就诊凭证编号
                    医疗类别
                    医疗费总额
                    个人结算方式
                    就诊ID
                    收费批次号
                    个人账户使用标志
                    险种类型
                    发票号
                    中途结算标志
                    全自费金额
                    超限价金额
                    先行自付金额
                    符合政策范围金额
                    出院时间  yyyy-MM-dd
                 */
                dynamic input = new
                {
                    data = new
                    {
                        psn_no = grbh,
                        mdtrt_cert_type = jzpzlx,
                        mdtrt_cert_no = jzpzbh,
                        medfee_sumamt = sfje,
                        psn_setlway = "01",
                        mdtrt_id = ybjzlsh,
                        acct_used_flag = zhsybz,
                        insutype = xzlx,
                        insuplc_admdvs = insuplc_admdvs,
                        mdtrtarea_admvs = YBjyqh,
                        invono = "0000",
                        mid_setl_flag = ztjsbz,
                        fulamt_ownpay_amt = zfje,
                        overlmt_selfpay = cxjzfje,
                        preselfpay_amt = xxzfje,
                        inscp_scp_amt = fhzcfwje,
                        dscgTime = cyrq
                    }
                };
                string Err = string.Empty;
                #endregion

                #region  预结算
                WriteLog(sysdate + "  住院收费预结算|入参Json|" + zyyjsJson);
                int i = YBServiceRequest("2303", input, ref Err);
                List<string> liSQL = new List<string>();
                if (i != 1)
                {
                    WriteLog(sysdate + "  住院收费预结算失败|" + Err.ToString());
                    return new object[] { 0, 0, "  住院收费预结算失败|" + Err.ToString() };
                }
                string dataRs = Err.ToString();
                WriteLog(sysdate + "  住院收费预结算|出参Json|" + dataRs);
                JObject jobj = JsonConvert.DeserializeObject(dataRs) as JObject; //获取响应数据json 
                #endregion

                #region 出参
                /*
                 输出-结算信息（节点标识：setlinfo）
                      就诊ID
                      人员编号
                      人员姓名
                      人员证件类型
                      证件号码
                      性别
                      民族
                      出生日期
                      年龄
                      险种类型
                      人员类别
                      公务员标志
                      结算时间
                      就诊凭证类型
                      医疗类别
                      医疗费总额
                      全自费金额
                      超限价自费费用
                      先行自付金额
                      符合政策范围金额
                      实际支付起付线
                      基本医疗保险统筹基金支出
                      基本医疗保险统筹基金支付比例
                      公务员医疗补助资金支出
                      企业补充医疗保险基金支出
                      居民大病保险资金支出
                      职工大额医疗费用补助基金支出
                      医疗救助基金支出
                      其他支出
                      基金支付总额
                      个人负担总金额
                      个人账户支出
                      个人现金支出
                      医院负担金额
                      余额
                      个人账户共济支付金额
                      医药机构结算ID
                      清算经办机构
                      清算方式
                      清算类别
                结算基金分项信息（节点标识：setldetail）
                      基金支付类型
                      符合政策范围金额
                      本次可支付限额金额
                      基金支付金额
                      基金支付类型名称
                      结算过程信息
               */
                JToken jtRet = jobj;
                string ybjzlshJS = jtRet["setlinfo"]["mdtrt_id"].ToString();
                string grbhJS = jtRet["setlinfo"]["psn_no"].ToString();
                string xmJS = jtRet["setlinfo"]["psn_name"].ToString();
                string ryzjlxJS = jtRet["setlinfo"]["psn_cert_type"].ToString();
                string zjhmJS = jtRet["setlinfo"]["certno"].ToString();
                string xbJS = jtRet["setlinfo"]["gend"].ToString();
                string mzJS = jtRet["setlinfo"]["naty"].ToString();
                string csrqJS = jtRet["setlinfo"]["brdy"].ToString();
                string nlJS = jtRet["setlinfo"]["age"].ToString();
                string xzlxJS = jtRet["setlinfo"]["insutype"].ToString();
                string rylbJS = jtRet["setlinfo"]["psn_type"].ToString();
                string gwybzJS = jtRet["setlinfo"]["cvlserv_flag"].ToString();
                string jssjJS = jtRet["setlinfo"]["setl_time"].ToString();
                string jzpzlxJS = jtRet["setlinfo"]["mdtrt_cert_type"].ToString();
                string yllbJS = jtRet["setlinfo"]["med_type"].ToString();
                string ylfzeJS = jtRet["setlinfo"]["medfee_sumamt"].ToString();
                string qzfjeJS = jtRet["setlinfo"]["fulamt_ownpay_amt"].ToString();
                string cxjzffyJS = jtRet["setlinfo"]["overlmt_selfpay"].ToString();
                string xxzfjeJS = jtRet["setlinfo"]["preselfpay_amt"].ToString();
                string fhzcfwjeJS = jtRet["setlinfo"]["inscp_scp_amt"].ToString();
                string qfxbzJS = jtRet["setlinfo"]["act_pay_dedc"].ToString();
                string tcjjzfJS = jtRet["setlinfo"]["hifp_pay"].ToString();
                string tcjjzfblJS = jtRet["setlinfo"]["pool_prop_selfpay"].ToString();
                string gwybzjjzfJS = jtRet["setlinfo"]["cvlserv_pay"].ToString();
                string qybcjjzfJS = jtRet["setlinfo"]["hifes_pay"].ToString();
                string jmdbbxjjzfJS = jtRet["setlinfo"]["hifmi_pay"].ToString();
                string dejjzfJS = jtRet["setlinfo"]["hifob_pay"].ToString();
                string mzjzfyJS = jtRet["setlinfo"]["maf_pay"].ToString();
                string qtjjzfJS = jtRet["setlinfo"]["oth_pay"].ToString();
                string jjzfzeJS = jtRet["setlinfo"]["fund_pay_sumamt"].ToString();
                string grfdzjeJS = jtRet["setlinfo"]["psn_part_amt"].ToString();
                string zhzfJS = jtRet["setlinfo"]["acct_pay"].ToString();
                string xjzfJS = jtRet["setlinfo"]["psn_cash_pay"].ToString();
                string yyfdfyJS = jtRet["setlinfo"]["hosp_part_amt"].ToString();
                string zhyeJS = jtRet["setlinfo"]["balc"].ToString();
                string gjzhzfJS = jtRet["setlinfo"]["acct_mulaid_pay"].ToString();
                string djhJS = jtRet["setlinfo"]["medins_setl_id"].ToString();
                string jbjgbmJS = jtRet["setlinfo"]["clr_optins"].ToString();
                string qsfsJS = jtRet["setlinfo"]["clr_way"].ToString();
                string qslbJS = jtRet["setlinfo"]["clr_type"].ToString();

                //明细
                strSql = string.Format(@"delete from ybfyjsmxdr where jzlsh='{0}'", jzlsh);
                liSQL.Add(strSql);
                JToken jts = jtRet["setldetail"];
                foreach (JToken jt in jts)
                {
                    string fund_pay_type = Convert.ToString(jt["fund_pay_type"]);//	基金支付类型
                    string inscp_scp_amt1 = Convert.ToString(jt["inscp_scp_amt"]);//	符合政策范围金额
                    string crt_payb_lmt_amt = Convert.ToString(jt["crt_payb_lmt_amt"]);//	本次可支付限额金额
                    string fund_payamt = Convert.ToString(jt["fund_payamt"]);//	基金支付金额
                    string fund_pay_type_name = Convert.ToString(jt["fund_pay_type_name"]);//	基金支付类型名称
                    string setl_proc_info = Convert.ToString(jt["setl_proc_info"]);//	结算过程信息

                    if (string.IsNullOrEmpty(inscp_scp_amt1))
                        inscp_scp_amt1 = "0";
                    if (string.IsNullOrEmpty(crt_payb_lmt_amt))
                        crt_payb_lmt_amt = "0";
                    if (string.IsNullOrEmpty(fund_payamt))
                        fund_payamt = "0";
                    strSql = string.Format(@"insert into ybfyjsmxdr(jzlsh,jylsh,jslsh,ybjzlsh,cxbz,sysdate,jbr,jjzflx,fhzcfwje,bckzfxeje,jjzfje,jjzflxmc,jsgcxx) values(
                                            '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}')",
                                            jzlsh, JYLSH, "0000", ybjzlsh, 1, sysdate, CZYBH, fund_pay_type, inscp_scp_amt1, crt_payb_lmt_amt, fund_payamt, fund_pay_type_name, setl_proc_info);
                    liSQL.Add(strSql);
                }
                #endregion

                string yblx = "";
                if (rylbJS.Substring(0, 2).Contains("14") || rylbJS.Substring(0, 2).Contains("15") || rylbJS.Substring(0, 2).Contains("16"))
                    yblx = "居民医保";
                else
                    yblx = "职工医保";
                string qtybzf = (Convert.ToDecimal(ylfzeJS) - Convert.ToDecimal(tcjjzfJS) - Convert.ToDecimal(xjzfJS) - Convert.ToDecimal(zhzfJS)).ToString();
                string zbxje = (Convert.ToDecimal(ylfzeJS) - Convert.ToDecimal(xjzfJS)).ToString();
                string zhzbxje = zbxje;
                string zhxjzf = xjzfJS;
                string bcjsqzhye = (Convert.ToDecimal(zhyeJS) + Convert.ToDecimal(zhzfJS)).ToString();

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
                string strValue = ylfzeJS + "|" + zhzbxje + "|" + tcjjzfJS + "|" + dejjzfJS + "|" + zhzfJS + "|" +
                                zhxjzf + "|" + gwybzjjzfJS + "|" + qybcjjzfJS + "|" + qzfjeJS + "|" + "0.00" + "|" +
                                yyfdfyJS + "|" + mzjzfyJS + "|" + cxjzffyJS + "|" + "0.00" + "|" + "0.00" + "|" +
                                fhzcfwjeJS + "|" + qtybzf + "|" + "0.00" + "|" + "0.00" + "|" + "0.00" + "|" +
                                "0.00" + "|" + "0.00" + "|" + "0.00" + "|" + "0.00" + "|" + "0.00" + "|" +
                                bcjsqzhye + "|" + "0.00" + "|" + "0.00" + "|" + "0.00" + "|" + "0.00" + "|" +
                                "0.00" + "|" + "0.00" + "|" + "0.00" + "|" + xmJS + "|" + jsrq + jssjJS + "|" +
                                yllb + "|" + rylbJS + "|" + jbjgbmJS + "|" + YWZQH + "|" + "0000" + "|" +
                                "0" + "|" + djh + "|" + qslbJS + "|" + "" + "|" + "1" + "|" +
                                grbh + "|" + YBJGBH + "|" + "0.00" + "|" + "0.00" + "|" + JYLSH + "|" +
                                grbhJS + "|" + "0.00" + "|" + "0.00" + "|" + "0.00" + "|" + "0.00" + "|" +
                                "0.00" + "|" + "0.00" + "|" + "0.00" + "|" + "0.00" + "|" + "0.00" + "|" +
                                "0.00" + "|" + "0.00" + "|" + "0.00" + "|" + "0.00" + "|" + "0.00" + "|";

                WriteLog(sysdate + "  住院收费预结算|整合出参|" + strValue);
                #region 数据操作
                strSql = string.Format(@"delete from ybfyyjsdr where jzlsh='{0}'", jzlsh);
                liSQL.Add(strSql);

                strSql = string.Format(@"insert into ybfyyjsdr(jzlsh,djhin,djh,jylsh,yblx,cxbz,sysdate,jbr,kh,bzbm,bzmc,zhsybz,ztjsbz,tcqh,qtybfy,zbxje,zhzbxje,zhxjzffy,ybjzlsh,grbh,xm,ryzjlx,zjhm,xb,mz,csrq,nl,xzlx,yldylb,
                                         gwybz,jsrq,jzpzlx,yllb,ylfze,zffy,cxjfy,xxzfje,fhjbylfy,qfbzfy,tcjjzf,jbylbxtcjjzfbl,gwybzjjzf,qybcylbxjjzf,dbjjzf,dejjzf,mzjzfy,qtjjzf,jjzfze,grfdzje,zhzf,xjzf,yyfdfy,
                                         bcjsqzhye,grzhgjzfje,jslsh,jbjgbm,jsfs,jylx,ywzqh) values('{0}','{1}','{2}','{3}','{4}','{5}' ,'{6}','{7}','{8}' ,'{9}' ,
                                        '{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}',
                                        '{20}','{21}','{22}','{23}','{24}','{25}','{26}','{27}','{28}','{29}',
                                        '{30}','{31}','{32}','{33}','{34}','{35}','{36}','{37}','{38}','{39}',
                                        '{40}','{41}','{42}','{43}','{44}','{45}','{46}','{47}','{48}','{49}',
                                        '{50}','{51}','{52}','{53}','{54}','{55}','{56}','{57}','{58}')",
                                        jzlsh, djh, djhJS, JYLSH, yblx, "1", sysdate, CZYBH, kh, bzbm, bzmc, zhsybz, ztjsbz, dqbh, qtybzf, zbxje, zhzbxje, zhxjzf, ybjzlshJS, grbhJS, xmJS, ryzjlxJS, zjhmJS, xbJS, mzJS, csrqJS, nlJS, xzlxJS, rylbJS,
                                        gwybzJS, jssjJS, jzpzlxJS, yllb, ylfzeJS, qzfjeJS, cxjzffyJS, xxzfjeJS, fhzcfwjeJS, qfxbzJS, tcjjzfJS, tcjjzfblJS, gwybzjjzfJS, qybcjjzfJS, jmdbbxjjzfJS, dejjzfJS, mzjzfyJS, qtjjzfJS, jjzfzeJS, grfdzjeJS, zhzfJS, xjzfJS, yyfdfyJS,
                                        bcjsqzhye, gjzhzfJS, "", jbjgbmJS, qsfsJS, qslbJS, YWZQH);
                liSQL.Add(strSql);
                strSql = string.Format("update ybmzzydjdr set cyzdbm = '{0}',cyzdmc = '{1}',fsxx='{3}' where cxbz = 1 and jzlsh = '{2}'", cyzdbm, cyzdmc, jzlsh, fsxx);
                liSQL.Add(strSql);
                object[] obj = liSQL.ToArray();
                obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);
                if (obj[1].ToString() == "1")
                {
                    WriteLog(sysdate + "  住院收费预结算成功|本地数据操作成功|" + obj[2].ToString());
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
                WriteLog(sysdate + "  住院收费预结算|系统异常|" + error.Message + error.StackTrace);
                return new object[] { 0, 2, error.Message };
            }
        }
        #endregion

        #region 住院收费结算
        public static object[] YBZYSFJS(object[] objParam)
        {
            string sysdate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            WriteLog(sysdate + "  进入住院收费结算...");
            WriteLog("进入住院收费结算|HIS传参|" + string.Join(",", objParam));
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
                //string jzlsh = objParam[0].ToString();      // 就诊流水号
                //string djh = objParam[1].ToString();        //单据号（发票号）
                //string cyyy = objParam[2].ToString();       //出院原因
                //string zhsybz = objParam[3].ToString();     //账户使用标志（0或1）
                //string ztjsbz = objParam[4].ToString();     //中途结算标志
                //string jsrqsj = objParam[5].ToString();     //结算日期时间
                //string cyrqsj = objParam[6].ToString();     //出院日期时间
                //string sfje = objParam[7].ToString();       //收费金额
                //string trs = objParam[8].ToString();       //胎儿数
                //string jsfs = objParam[9].ToString();       //结算方式
                string jzlsh = objParam[0].ToString();   // 就诊流水号
                string djh = objParam[1].ToString();   //单据号 
                string cyyy = objParam[2].ToString(); ;    // 出院原因代码
                string zhsybz = objParam[3].ToString();  // 账户使用标志（0或1）
                string ztjsbz = objParam[4].ToString(); //中途结算标志
                string cyrqsj = objParam[5].ToString();//出院日期
                string jsrqsj = objParam[6].ToString();     //结算日期时间
                string sfje = objParam[7].ToString(); //医疗费合计 
                string jsfs = "01"; //结算方式 21-
                string cyzdbm = "";
                if (objParam.Length > 8)
                    cyzdbm = objParam[8].ToString();//出院诊断编码;  
                string ybcyrq = Convert.ToDateTime(cyrqsj).ToString("yyyyMMddHHmmss"); //出院日期
                string grzhye = "0.00";

                string yjfalx = "1";//月结方案类型
                string syfylb = "";//生育费用类         
                string syfwzh = "";//生育服务证号
                if (string.IsNullOrEmpty(jzlsh))
                    return new object[] { 0, 0, "就诊流水号不能为空" };
                if (string.IsNullOrEmpty(djh))
                    return new object[] { 0, 0, "单据号不能为空" };
                if (string.IsNullOrEmpty(cyyy))
                    return new object[] { 0, 0, "出院原因不能为空" };
                if (string.IsNullOrEmpty(jsfs))
                    return new object[] { 0, 0, "泰州结算方式不能为空" };

                yjfalx = jsfs;//获取结算方式


                CZYBH = CliUtils.fLoginUser;   // 操作员工号
                YWZQH = CliUtils.fLoginYbNo.Split('|')[0].ToString();   // 业务周期号
                string jbr = CliUtils.fUserName;     // 经办人姓名
                string cyrq = "";
                string jsrq = Convert.ToDateTime(jsrqsj).ToString("yyyy-MM-dd");
                string dqrq = Convert.ToDateTime(sysdate).ToString("yyyyMMddHHmmss");  // 当前日期
                if (ztjsbz.Equals("1"))
                    cyrq = jsrq;
                else
                    cyrq = Convert.ToDateTime(cyrqsj).ToString("yyyy-MM-dd");
                //医院交易流水号 

                #region 是否未办理医保登记
                string strSql = string.Format(@"select a.*,b.In_Diag_Code,b.z1ryzd from ybmzzydjdr a 
                    left join zy01h b on b.z1zyno=a.jzlsh 
                    where jzlsh='{0}' and cxbz=1", jzlsh);
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
                string tes = dr["tes"].ToString();
                syfylb = dr["syfylb"].ToString();
                syfwzh = dr["syfwzh"].ToString();
                #endregion

                #region 获取读卡数据
                string jzpzlx = "";//就诊凭证类型
                string jzpzbh = "";//就诊凭证编号
                string xzlx = "";//险种类型
                strSql = string.Format(@"select * from ybickxx where grbh='{0}'", grbh);
                ds.Tables.Clear();
                ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    jzpzlx = ds.Tables[0].Rows[0]["jzpzlx"].ToString();
                    jzpzbh = ds.Tables[0].Rows[0]["jzpzbh"].ToString();
                    xzlx = ds.Tables[0].Rows[0]["xzlx"].ToString();
                    grzhye = ds.Tables[0].Rows[0]["GRZHYE"].ToString();
                }
                //if (jzpzbh.Contains('|'))
                //    jzpzbh = jzpzbh.Split('|')[0];
                #endregion

                #region 是否费用上传
                string zfje = "0";
                string cxjzfje = "0";
                string xxzfje = "0";
                string fhzcfwje = "0";
                strSql = string.Format(@"select sum(isnull(zfje,0)) zfje,sum(isnull(cxjzfje,0)) cxjzfje,sum(isnull(xxzfje,0)) xxzfje,sum(isnull(fhzcfwje,0)) fhzcfwje from ybcfmxscfhdr where jzlsh='{0}' and cxbz=1", jzlsh);
                ds.Tables.Clear();
                ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                if (ds.Tables[0].Rows.Count == 0)
                    return new object[] { 0, 0, "该患者未进行费用上传" };
                zfje = ds.Tables[0].Rows[0]["zfje"].ToString();
                cxjzfje = ds.Tables[0].Rows[0]["cxjzfje"].ToString();
                xxzfje = ds.Tables[0].Rows[0]["xxzfje"].ToString();
                fhzcfwje = ds.Tables[0].Rows[0]["fhzcfwje"].ToString();
                #endregion

                #region 取医保总金额做总额
                string ybzje = "0";
                strSql = string.Format(@"select isnull(sum(isnull(je,0)),0) ybzje from ybcfmxscindr where jzlsh='{0}' and cxbz=1", jzlsh);
                ds.Tables.Clear();
                ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                ybzje = ds.Tables[0].Rows[0]["ybzje"].ToString();
                if (Math.Abs(double.Parse(sfje) - double.Parse(ybzje)) < 0.1)
                {
                    sfje = ybzje;
                }
                #endregion

                #region 是否已经医保结算
                strSql = string.Format(@"select * from ybfyjsdr where jzlsh='{0}' and ztjsbz=0 and cxbz=1", jzlsh);
                ds.Tables.Clear();
                ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                if (ds.Tables[0].Rows.Count > 0)
                    return new object[] { 0, 0, "该患者已办理医保结算" };
                #endregion

                #region 出院登记
                //出院登记
                object[] objCYDJ = { jzlsh, cyyy, cyrqsj, ybjzlsh };
                objCYDJ = YBCYBL(objCYDJ);
                //if (objCYDJ[1].ToString() != "1")
                //{
                //    return new object[] { 0, 0, "出院登记失败！" + objCYDJ[2].ToString() };
                //}
                #endregion

                #region 入参
                string zyjsJson = string.Empty;
                /*
                 输入（节点标识：data）
                    人员编号
                    就诊凭证类型
                    就诊凭证编号
                    医疗类别
                    医疗费总额
                    个人结算方式
                    就诊ID
                    收费批次号
                    个人账户使用标志
                    险种类型
                    发票号
                    中途结算标志
                    全自费金额
                    超限价金额
                    先行自付金额
                    符合政策范围金额
                    出院时间  yyyy-MM-dd
                 */
                dynamic input = new
                {
                    data = new
                    {
                        psn_no = grbh,
                        mdtrt_cert_type = jzpzlx,
                        mdtrt_cert_no = jzpzbh,
                        medfee_sumamt = sfje,
                        psn_setlway = "01",
                        mdtrt_id = ybjzlsh,
                        acct_used_flag = zhsybz,
                        insutype = xzlx,
                        insuplc_admdvs = insuplc_admdvs,
                        mdtrtarea_admvs = YBjyqh,
                        invono = djh,
                        mid_setl_flag = ztjsbz,
                        fulamt_ownpay_amt = zfje,
                        overlmt_selfpay = cxjzfje,
                        preselfpay_amt = xxzfje,
                        inscp_scp_amt = fhzcfwje,
                        dscgTime = cyrq
                    }
                };
                zyjsJson = JsonConvert.SerializeObject(input);
                string Err = string.Empty;
                #endregion

                #region 结算
                WriteLog(sysdate + "  住院收费结算|入参Json|" + zyjsJson);
                int i = YBServiceRequest("2304", input, ref Err);
                List<string> liSQL = new List<string>();
                if (i != 1)
                {
                    WriteLog(sysdate + "  住院收费结算失败|" + Err.ToString());
                    return new object[] { 0, 0, Err.ToString() };
                }
                string dataRs = Err.ToString();
                WriteLog(sysdate + "  住院收费结算|出参Json|" + dataRs);
                JObject jobj = JsonConvert.DeserializeObject(dataRs) as JObject; //获取响应数据json 
                #endregion

                #region 出参
                /*
                 输出-结算信息（节点标识：setlinfo）
                      就诊ID
                      结算ID
                      人员编号
                      人员姓名
                      人员证件类型
                      证件号码
                      性别
                      民族
                      出生日期
                      年龄
                      险种类型
                      人员类别
                      公务员标志
                      结算时间
                      就诊凭证类型
                      医疗类别
                      医疗费总额
                      全自费金额
                      超限价自费费用
                      先行自付金额
                      符合政策范围金额
                      实际支付起付线
                      基本医疗保险统筹基金支出
                      基本医疗保险统筹基金支付比例
                      公务员医疗补助资金支出
                      企业补充医疗保险基金支出
                      居民大病保险资金支出
                      职工大额医疗费用补助基金支出
                      医疗救助基金支出
                      其他支出
                      基金支付总额
                      个人负担总金额
                      个人账户支出
                      个人现金支出
                      医院负担金额
                      余额
                      个人账户共济支付金额
                      医药机构结算ID
                      清算经办机构
                      清算方式
                      清算类别
                结算基金分项信息（节点标识：setldetail）
                      基金支付类型
                      符合政策范围金额
                      本次可支付限额金额
                      基金支付金额
                      基金支付类型名称
                      结算过程信息
               */
                JToken jtRet = jobj;
                string ybjzlshJS = jtRet["setlinfo"]["mdtrt_id"].ToString();
                string jslshJS = jtRet["setlinfo"]["setl_id"].ToString();
                string grbhJS = jtRet["setlinfo"]["psn_no"].ToString();
                string xmJS = jtRet["setlinfo"]["psn_name"].ToString();
                string ryzjlxJS = jtRet["setlinfo"]["psn_cert_type"].ToString();
                string zjhmJS = jtRet["setlinfo"]["certno"].ToString();
                string xbJS = jtRet["setlinfo"]["gend"].ToString();
                string mzJS = jtRet["setlinfo"]["naty"].ToString();
                string csrqJS = jtRet["setlinfo"]["brdy"].ToString();
                string nlJS = jtRet["setlinfo"]["age"].ToString();
                string xzlxJS = jtRet["setlinfo"]["insutype"].ToString();
                string rylbJS = jtRet["setlinfo"]["psn_type"].ToString();
                string gwybzJS = jtRet["setlinfo"]["cvlserv_flag"].ToString();
                string jssjJS = jtRet["setlinfo"]["setl_time"].ToString();
                string jzpzlxJS = jtRet["setlinfo"]["mdtrt_cert_type"].ToString();
                string yllbJS = jtRet["setlinfo"]["med_type"].ToString();
                string ylfzeJS = jtRet["setlinfo"]["medfee_sumamt"].ToString();
                string qzfjeJS = jtRet["setlinfo"]["fulamt_ownpay_amt"].ToString();
                string cxjzffyJS = jtRet["setlinfo"]["overlmt_selfpay"].ToString();
                string xxzfjeJS = jtRet["setlinfo"]["preselfpay_amt"].ToString();
                string fhzcfwjeJS = jtRet["setlinfo"]["inscp_scp_amt"].ToString();
                string qfxbzJS = jtRet["setlinfo"]["act_pay_dedc"].ToString();
                string tcjjzfJS = jtRet["setlinfo"]["hifp_pay"].ToString();
                string tcjjzfblJS = jtRet["setlinfo"]["pool_prop_selfpay"].ToString();
                string gwybzjjzfJS = jtRet["setlinfo"]["cvlserv_pay"].ToString();
                string qybcjjzfJS = jtRet["setlinfo"]["hifes_pay"].ToString();
                string jmdbbxjjzfJS = jtRet["setlinfo"]["hifmi_pay"].ToString();
                string dejjzfJS = jtRet["setlinfo"]["hifob_pay"].ToString();
                string mzjzfyJS = jtRet["setlinfo"]["maf_pay"].ToString();
                string qtjjzfJS = jtRet["setlinfo"]["oth_pay"].ToString();
                string jjzfzeJS = jtRet["setlinfo"]["fund_pay_sumamt"].ToString();
                string grfdzjeJS = jtRet["setlinfo"]["psn_part_amt"].ToString();
                string zhzfJS = jtRet["setlinfo"]["acct_pay"].ToString();
                string xjzfJS = jtRet["setlinfo"]["psn_cash_pay"].ToString();
                string yyfdfyJS = jtRet["setlinfo"]["hosp_part_amt"].ToString();
                string zhyeJS = jtRet["setlinfo"]["balc"].ToString();
                string gjzhzfJS = jtRet["setlinfo"]["acct_mulaid_pay"].ToString();
                string djhJS = jtRet["setlinfo"]["medins_setl_id"].ToString();
                string jbjgbmJS = jtRet["setlinfo"]["clr_optins"].ToString();
                string qsfsJS = jtRet["setlinfo"]["clr_way"].ToString();
                string qslbJS = jtRet["setlinfo"]["clr_type"].ToString();

                //明细
                JToken jts = jtRet["setldetail"];
                foreach (JToken jt in jts)
                {
                    string fund_pay_type = Convert.ToString(jt["fund_pay_type"]);//	基金支付类型
                    string inscp_scp_amt1 = Convert.ToString(jt["inscp_scp_amt"]);//	符合政策范围金额
                    string crt_payb_lmt_amt = Convert.ToString(jt["crt_payb_lmt_amt"]);//	本次可支付限额金额
                    string fund_payamt = Convert.ToString(jt["fund_payamt"]);//	基金支付金额
                    string fund_pay_type_name = Convert.ToString(jt["fund_pay_type_name"]);//	基金支付类型名称
                    string setl_proc_info = Convert.ToString(jt["setl_proc_info"]);//	结算过程信息
                    if (string.IsNullOrEmpty(inscp_scp_amt1))
                        inscp_scp_amt1 = "0";
                    if (string.IsNullOrEmpty(crt_payb_lmt_amt))
                        crt_payb_lmt_amt = "0";
                    if (string.IsNullOrEmpty(fund_payamt))
                        fund_payamt = "0";

                    strSql = string.Format(@"insert into ybfyjsmxdr(jzlsh,jylsh,jslsh,ybjzlsh,cxbz,sysdate,jbr,jjzflx,fhzcfwje,bckzfxeje,jjzfje,jjzflxmc,jsgcxx) values(
                                            '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}')",
                                            jzlsh, JYLSH, jslshJS, ybjzlsh, 1, sysdate, CZYBH, fund_pay_type, inscp_scp_amt1, crt_payb_lmt_amt, fund_payamt, fund_pay_type_name, setl_proc_info);
                    liSQL.Add(strSql);
                }
                #endregion



                string yblx = "";
                if (rylbJS.Substring(0, 2).Contains("14") || rylbJS.Substring(0, 2).Contains("15") || rylbJS.Substring(0, 2).Contains("16"))
                    yblx = "居民医保";
                else
                    yblx = "职工医保";
                string qtybzf = (Convert.ToDecimal(ylfzeJS) - Convert.ToDecimal(tcjjzfJS) - Convert.ToDecimal(xjzfJS) - Convert.ToDecimal(zhzfJS)).ToString();
                string zbxje = (Convert.ToDecimal(ylfzeJS) - Convert.ToDecimal(xjzfJS)).ToString();
                string zhzbxje = zbxje;
                string zhxjzf = xjzfJS;
                string bcjsqzhye = (Convert.ToDecimal(zhyeJS) + Convert.ToDecimal(zhzfJS)).ToString();

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
                string strValue = ylfzeJS + "|" + zhzbxje + "|" + tcjjzfJS + "|" + dejjzfJS + "|" + zhzfJS + "|" +
                                    zhxjzf + "|" + gwybzjjzfJS + "|" + qybcjjzfJS + "|" + qzfjeJS + "|" + "0.00" + "|" +
                                    yyfdfyJS + "|" + mzjzfyJS + "|" + cxjzffyJS + "|" + "0.00" + "|" + "0.00" + "|" +
                                    fhzcfwjeJS + "|" + qtybzf + "|" + "0.00" + "|" + "0.00" + "|" + "0.00" + "|" +
                                    "0.00" + "|" + "0.00" + "|" + "0.00" + "|" + "0.00" + "|" + "0.00" + "|" +
                                    bcjsqzhye + "|" + "0.00" + "|" + "0.00" + "|" + "0.00" + "|" + "0.00" + "|" +
                                    "0.00" + "|" + "0.00" + "|" + "0.00" + "|" + xmJS + "|" + jsrq + jssjJS + "|" +
                                    yllb + "|" + rylbJS + "|" + jbjgbmJS + "|" + YWZQH + "|" + "0000" + "|" +
                                    "0" + "|" + djh + "|" + qslbJS + "|" + "" + "|" + "1" + "|" +
                                    grbh + "|" + YBJGBH + "|" + "0.00" + "|" + "0.00" + "|" + JYLSH + "|" +
                                    grbhJS + "|" + "0.00" + "|" + "0.00" + "|" + "0.00" + "|" + "0.00" + "|" +
                                    "0.00" + "|" + "0.00" + "|" + "0.00" + "|" + "0.00" + "|" + "0.00" + "|" +
                                    "0.00" + "|" + "0.00" + "|" + "0.00" + "|" + "0.00" + "|" + "0.00" + "|" +
                                    "0.00" + "|" + "0.00" + "|" + "0.00" + "|" + "0.00" + "|" + "0.00" + "|" +
                                    "0.00" + "|" + "0.00" + "|" + "0.00" + "|" + "0.00" + "|" + "0.00" + "|";

                WriteLog(sysdate + "  住院收费结算|整合出参|" + strValue);
                try
                {
                    jssjJS = Convert.ToDateTime(jssjJS).ToString("yyyyMMddHHmmss");
                }
                catch (Exception ex)
                {
                    jssjJS = DateTime.Now.ToString("yyyyMMddHHmmss");
                }
                #region 数据操作
                strSql = string.Format(@"insert into ybfyjsdr(jzlsh,djhin,djh,jylsh,yblx,cxbz,sysdate,jbr,kh,bzbm,bzmc,zhsybz,ztjsbz,tcqh,qtybfy,zbxje,zhzbxje,zhxjzffy,ybjzlsh,grbh,xm,ryzjlx,zjhm,xb,mz,csrq,nl,xzlx,yldylb,gwybz,jsrq,jzpzlx,yllb,ylfze,zffy,cxjfy,xxzfje,fhjbylfy,qfbzfy,tcjjzf,jbylbxtcjjzfbl,gwybzjjzf,qybcylbxjjzf,dbjjzf,dejjzf,mzjzfy,qtjjzf,jjzfze,grfdzje,zhzf,xjzf,yyfdfy,bcjsqzhye,grzhgjzfje,jslsh,jbjgbm,jsfs,jylx,ywzqh,qfxbz,bcfhyb,cyrq,bcbcbxzffy,bcsydbzffy) values('{0}','{1}','{2}','{3}','{4}','{5}' ,'{6}','{7}','{8}' ,'{9}' ,
                                        '{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}',
                                        '{20}','{21}','{22}','{23}','{24}','{25}','{26}','{27}','{28}','{29}',
                                        '{30}','{31}','{32}','{33}','{34}','{35}','{36}','{37}','{38}','{39}',
                                        '{40}','{41}','{42}','{43}','{44}','{45}','{46}','{47}','{48}','{49}',
                                        '{50}','{51}','{52}','{53}','{54}','{55}','{56}','{57}','{58}','{59}','{60}','{61}','{62}','{63}')",
                                         jzlsh, djh, djhJS, JYLSH, yblx, "1", sysdate, CZYBH, kh, bzbm, bzmc, zhsybz, ztjsbz, dqbh, qtybzf, zbxje, zhzbxje, zhxjzf, ybjzlshJS, grbhJS, xmJS, ryzjlxJS, zjhmJS, xbJS, mzJS, csrqJS, nlJS, xzlxJS, rylbJS,
                                         gwybzJS, jssjJS, jzpzlxJS, yllb, ylfzeJS, qzfjeJS, cxjzffyJS, xxzfjeJS, fhzcfwjeJS, qfxbzJS, tcjjzfJS, tcjjzfblJS, gwybzjjzfJS, qybcjjzfJS, jmdbbxjjzfJS, dejjzfJS, mzjzfyJS, qtjjzfJS, jjzfzeJS, grfdzjeJS, zhzfJS, xjzfJS, yyfdfyJS,bcjsqzhye, gjzhzfJS, jslshJS, jbjgbmJS, qsfsJS, qslbJS, YWZQH, qfxbzJS, fhzcfwjeJS, ybcyrq, '0', '0');
                liSQL.Add(strSql);
                strSql = string.Format("update ybcfmxscfhdr set ybdjh='{0}' where isnull(ybdjh,'')='' and jzlsh='{1}' and cxbz=1", djh, jzlsh);
                liSQL.Add(strSql);
                strSql = string.Format("update ybcfmxscindr set ybdjh='{0}' where isnull(ybdjh,'')='' and jzlsh='{1}' and cxbz=1", djh, jzlsh);
                liSQL.Add(strSql);

                object[] obj = liSQL.ToArray();
                obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);
                if (obj[1].ToString() == "1")
                {
                    //结算清单上传
                    WriteLog(sysdate + "   住院结算清单上传,入参" + ybjzlsh);
                    object[] scres = JSQD_4101(new object[] { ybjzlsh });
                    WriteLog(sysdate + "   住院结算清单上传,回参" + scres[2].ToString());
                    WriteLog(sysdate + "  住院收费结算成功|本地数据操作成功|" + obj[2].ToString());
                    //住院结算单打印
                    YBZYJSD(new object[] { jzlsh, djh });
                    //住院结算清单打印
                    //YBZYJSQD(new object[] { jzlsh, djh });
                    return new object[] { 0, 1, strValue };
                }
                else
                {
                    WriteLog(sysdate + "  住院收费结算成功|本地数据操作失败|" + obj[2].ToString());
                    //撤销收费结算
                    object[] objJSCX = { jzlsh, djh, ybjzlsh, xm, kh, grbh, dqrq, jsrq, jslshJS, ybjzlsh_snyd, DQJBBZ };
                    NYBZYFYJSCX(objJSCX);
                    return new object[] { 0, 0, "住院收费结算成功|本地数据操作失败|" + obj[2].ToString() };
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

        #region  住院收费结算撤销(内部)
        public static object[] NYBZYFYJSCX(object[] objParam)
        {
            string sysdate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            WriteLog(sysdate + "  进入住院收费结算撤销(内部)...");
            WriteLog("进入住院收费结算撤销(内部)|HIS传参|" + string.Join(",", objParam));
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
                string jslsh = objParam[7].ToString();    //结算流水号  
                string ybjzlsh_snyd = objParam[8].ToString();
                string dqbh = objParam[9].ToString();
                DQJBBZ = objParam[10].ToString();    //地区级别标志
                string jysj = Convert.ToDateTime(sysdate).ToString("yyyyMMddHHmmss");
                //交易流水号 

                #region 入参
                string zyjscxJson = string.Empty;
                /*
                 * 输入（节点标识：data）
                 就诊ID
                 人员编号
                 */
                dynamic input = new
                {
                    data = new
                    {
                        mdtrt_id = ybjzlsh,
                        setl_id = jslsh,
                        psn_no = grbh
                    }

                };
                string Err = string.Empty;
                zyjscxJson = JsonConvert.SerializeObject(input);
                #endregion

                WriteLog(sysdate + "  住院收费结算撤销(内部)|入参|" + zyjscxJson);
                int i = YBServiceRequest("2305", input, ref Err);
                if (i == 1)
                {
                    string dataRs = Err.ToString();
                    WriteLog(sysdate + "  住院收费结算撤销(内部)|出参Json|" + dataRs);
                    JObject jobj = JsonConvert.DeserializeObject(dataRs) as JObject; //获取响应数据json 
                    WriteLog(sysdate + "  住院收费结算撤销(内部)成功|出参|" + Err.ToString());
                    return new object[] { 0, 1, "住院收费结算撤销(内部)成功" };

                }
                else
                {
                    WriteLog(sysdate + "   住院收费结算撤销(内部)失败|" + Err.ToString());
                    return new object[] { 0, 0, "   住院收费结算撤销(内部)失败|" + Err.ToString() };
                }
            }
            catch (Exception ex)
            {
                WriteLog(sysdate + "   住院收费结算撤销(内部)|系统异常|" + ex.Message);
                return new object[] { 0, 0, ex.Message };
            }
        }
        #endregion

        #region  住院收费结算撤销
        public static object[] YBZYSFJSCX(object[] objParam)
        {
            string sysdate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            WriteLog(sysdate + "  进入住院收费结算撤销...");
            WriteLog("进入住院收费结算撤销|HIS传参|" + string.Join(",", objParam));
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
                string jbr = CliUtils.fUserName;  //经办人
                string jzlsh = objParam[0].ToString();   // 就诊流水号
                string djh = objParam[1].ToString();     // 结算单据号
                string fphx = "";
                //获取医保结算信息
                string strSql = string.Format(@"select a.jslsh,a.cfmxjylsh,c.z3fphx,a.jsrq,a.djh,a.djhin,b.* from ybfyjsdr a 
                                                left join ybmzzydjdr b on a.jzlsh = b.jzlsh and a.cxbz=b.cxbz
                                                left join zy03dw c on a.jzlsh=c.z3zyno and a.djhin=c.z3jshx and c.z3endv like '1%'
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
                string ybjzlsh_snyd = dr["ybjzlsh_snyd"].ToString();
                string dqbh = dr["tcqh"].ToString();
                string ybdjh = dr["djhin"].ToString();
                string jslsh = dr["jslsh"].ToString();
                DQJBBZ = dr["dqjbbz"].ToString();    //地区级别标志
                fphx = dr["z3fphx"].ToString();

                #region 入参
                string zyjscxJson = string.Empty;
                /*
                 * 输入（节点标识：data）
                 就诊ID
                 人员编号
                 */
                dynamic input = new
                {
                    data = new
                    {
                        mdtrt_id = ybjzlsh,
                        setl_id = jslsh,
                        psn_no = grbh
                    }
                };
                string Err = string.Empty;
                zyjscxJson = JsonConvert.SerializeObject(input);
                #endregion

                WriteLog(sysdate + "  住院收费结算撤销|入参Json|" + zyjscxJson);
                int i = YBServiceRequest("2305", input, ref Err);
                if (i == 1)
                {
                    string dataRs = Err.ToString();
                    WriteLog(sysdate + "  住院收费结算撤销|出参Json|" + dataRs);
                    JObject jobj = JsonConvert.DeserializeObject(dataRs) as JObject; //获取响应数据json 
                    List<string> liSql = new List<string>();
                    //撤销结算信息
                    strSql = string.Format(@"insert into ybfyjsdr(jzlsh,djhin,djh,jylsh,yblx,cxbz,sysdate,jbr,kh,bzbm,bzmc,zhsybz,ztjsbz,tcqh,qtybfy,zbxje,zhzbxje,zhxjzffy,ybjzlsh,grbh,xm,ryzjlx,zjhm,xb,mz,csrq,nl,xzlx,yldylb,
                                         gwybz,jsrq,jzpzlx,yllb,ylfze,zffy,cxjfy,xxzfje,fhjbylfy,qfbzfy,tcjjzf,jbylbxtcjjzfbl,gwybzjjzf,qybcylbxjjzf,dbjjzf,dejjzf,mzjzfy,qtjjzf,jjzfze,grfdzje,zhzf,xjzf,yyfdfy,
                                         bcjsqzhye,grzhgjzfje,jslsh,jbjgbm,jsfs,jylx,ywzqh) 
                                        select jzlsh,djhin,djh,jylsh,yblx,'0','{2}',jbr,kh,bzbm,bzmc,zhsybz,ztjsbz,tcqh,qtybfy,zbxje,zhzbxje,zhxjzffy,ybjzlsh,grbh,xm,ryzjlx,zjhm,xb,mz,csrq,nl,xzlx,yldylb,
                                         gwybz,jsrq,jzpzlx,yllb,ylfze,zffy,cxjfy,xxzfje,fhjbylfy,qfbzfy,tcjjzf,jbylbxtcjjzfbl,gwybzjjzf,qybcylbxjjzf,dbjjzf,dejjzf,mzjzfy,qtjjzf,jjzfze,grfdzje,zhzf,xjzf,yyfdfy,
                                         bcjsqzhye,grzhgjzfje,jslsh,jbjgbm,jsfs,jylx,ywzqh
                                        from ybfyjsdr where jzlsh = '{0}' and djh = '{1}' and cxbz = 1", jzlsh, djh, sysdate);
                    liSql.Add(strSql);

                    strSql = string.Format(@"update ybfyjsdr set cxbz = 2 where jzlsh = '{0}' and djh = '{1}' and cxbz = 1", jzlsh, djh);
                    liSql.Add(strSql);

                    //撤销结算明细返回

                    strSql = string.Format(@"insert into ybfyjsmxdr(jzlsh,jylsh,jslsh,ybjzlsh,cxbz,sysdate,jbr,jjzflx,fhzcfwje,bckzfxeje,jjzfje,jjzflxmc,jsgcxx)
                                        select jzlsh,jylsh,jslsh,ybjzlsh,0,'{1}','{2}',jjzflx,fhzcfwje,bckzfxeje,jjzfje,jjzflxmc,jsgcxx from ybfyjsmxdr where jslsh='{0}' and cxbz=1", jslsh, sysdate, CZYBH);
                    liSql.Add(strSql);
                    strSql = string.Format(@"update ybfyjsmxdr set cxbz=2 where jslsh='{0}' and cxbz=1", jslsh);
                    liSql.Add(strSql);

                    //撤销费用明细
                    strSql = string.Format(@"INSERT INTO [dbo].[ybcfmxscfhdr]
                                       (jzlsh,jylsh,ybjzlsh,yyxmdm,yyxmmc,yybxmbh,ybxmmc,je,zlje,zfje,
                                        cxjzfje,sflb,sfxmdj,qezfbz,zlbl,xj,bz,dqxmxh,ybcfh,dj,sl, djsxje, xxzfje, fhzcfwje,jjywbz,ybtbypbz,etyybz,mltsbz,zbbz,sysdate,cxbz)
                                         select jzlsh,jylsh,ybjzlsh,yyxmdm,yyxmmc,yybxmbh,ybxmmc,je,zlje,zfje,
                                          cxjzfje,sflb,sfxmdj,qezfbz,zlbl,xj,bz,dqxmxh,ybcfh,dj,sl, djsxje, xxzfje, fhzcfwje,jjywbz,ybtbypbz,etyybz,mltsbz,zbbz,'{2}','0'
		                               from ybcfmxscfhdr where jzlsh = '{0}' and ybdjh='{1}' and cxbz = 1", jzlsh, djh, sysdate);
                    liSql.Add(strSql);

                    strSql = string.Format(@"update ybcfmxscfhdr set cxbz=2 where jzlsh = '{0}' and ybdjh='{1}' and cxbz = 1", jzlsh, djh);
                    liSql.Add(strSql);

                    strSql = string.Format(@"INSERT INTO [dbo].[ybcfmxscindr]
                                           (jzlsh,jylsh,sfxmzl,sflb,ybcfh,cfrq,yysfxmbm,sfxmzxbm,yysfxmmc,dj
                                           ,sl,je,jx,gg,mcyl,sypc,ysbm,ysxm,yf,dw,ksbh,ksmc ,zxts
                                           ,cydffbz,jbr,ypjldw,qezfbz,grbh,xm,kh,jsdjh,cxbz,sysdate,ybjzlsh,djlsh
                                           ,sfxmzxmc,zfje,cfscsj,remark,sfxmdj,ybdjh,ybbz,dqxmxh)
                                            select
                                            jzlsh,jylsh,sfxmzl,sflb,ybcfh,cfrq,yysfxmbm,sfxmzxbm,yysfxmmc,dj
                                           ,sl,je,jx,gg,mcyl,sypc,ysbm,ysxm,yf,dw,ksbh,ksmc ,zxts
                                           ,cydffbz,jbr,ypjldw,qezfbz,grbh,xm,kh,jsdjh,0,'{2}',ybjzlsh,djlsh
                                           ,sfxmzxmc,zfje,cfscsj,remark,sfxmdj,ybdjh,ybbz,dqxmxh
                                            from ybcfmxscindr where jzlsh = '{0}' and ybdjh='{1}' and cxbz = 1", jzlsh, djh, sysdate);
                    liSql.Add(strSql);
                    strSql = string.Format(@"update ybcfmxscindr set cxbz=2 where jzlsh = '{0}' and ybdjh='{1}' and cxbz = 1", jzlsh, djh);
                    liSql.Add(strSql);

                    //取消上传标志
                    strSql = string.Format(@"update zy03d set z3ybup = null where z3zyno = '{0}' and z3jshx='{1}'", jzlsh, djh);
                    liSql.Add(strSql);
                    object[] obj = liSql.ToArray();
                    obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);
                    if (obj[1].ToString() == "1")
                    {
                        WriteLog(sysdate + " 住院收费结算撤销成功|本地数据操作成功|");
                        return new object[] { 0, 1, "住院收费结算撤销成功" };
                    }
                    else
                    {
                        WriteLog(sysdate + "   住院收费结算撤销成功|本地数据操作失败|" + obj[2].ToString());
                        return new object[] { 0, 0, obj[2].ToString() };
                    }
                }
                else
                {
                    WriteLog(sysdate + "   住院收费结算撤销失败|" + Err.ToString());
                    return new object[] { 0, 0, Err.ToString() };
                }
            }
            catch (Exception ex)
            {
                WriteLog(sysdate + "   住院收费结算撤销|系统异常|" + ex.Message);
                return new object[] { 0, 0, ex.Message };
            }
        }
        #endregion

        #region 3401 3402医保科室信息上传
        public static object[] YBKSXXPLSC(object[] objParam)
        {
            string sysdate = GetServerDateTime();
            WriteLog(sysdate + "  进入科室信息上传...");
            WriteLog("进入科室信息上传|HIS传参|" + string.Join(",", objParam));
            try
            {
                CZYBH = CliUtils.fLoginUser; //操作员工号
                string jbr = CliUtils.fUserName;
                YWZQH = CliUtils.fLoginYbNo.Split('|')[0].ToString();    //业务周期号
                string jysj = Convert.ToDateTime(sysdate).ToString("yyyyMMddHHmmss");

                string ksbm = objParam[0].ToString();
                string scbz1 = objParam[1].ToString();
                #region 对照数据
                StringBuilder inParam = new StringBuilder();
                string sDate = DateTime.Now.ToString("yyyyMMddHHmmss");
                string strSql = string.Format(@"with tmp as
                        (
                        SELECT 
                        distinct d.b2ejks,
                        d.b2ejnm,
                        h.aprv_bed_cnt,
                        h.hi_crtf_bed_cnt,
                        h.poolarea_no,
                        h.dr_psncnt,
                        h.phar_psncnt,
                        h.nurs_psncnt,
                        h.tecn_psncnt,
                        h.dept_resper_name,
                        h.dept_resper_tel,
                        h.begntime,
                        h.endtime,
                        h.dept_estbdat,
                        h.b2bjno,
                        h.b2bynm
                         FROM dbo.bz02h h
                        LEFT  JOIN bz02d d ON d.b2ksno=h.b2ksno
                        where isnull(d.b2ybup,0)=0
                    
                        )
                        select  b2ejks ksdm,b2ejnm ksmc,aprv_bed_cnt pzcwsl,hi_crtf_bed_cnt ybrkcws,poolarea_no tcqh,dr_psncnt ysrs,
                                phar_psncnt yaosrs,nurs_psncnt hsrs,tecn_psncnt jsrs,dept_resper_name ksfzrxm,dept_resper_tel ksfzrdh,begntime kssj,
                                endtime jssj,b2bjno ybksdm,b2bynm ybksmc,'暂无' jjsm,'' ksylfwfw,dept_estbdat ksclrq from tmp where 1=1");
                if (!string.IsNullOrEmpty(ksbm))
                    strSql += string.Format(@" and b2ejks='{0}'", ksbm);

                DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                if (ds.Tables[0].Rows.Count == 0)
                    return new object[] { 0, 0, "没有查询到已配对的科室信息" };
                string message = "";
                List<string> liSQL = new List<string>();
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    string ksdm = dr["ksdm"].ToString();
                    string ksmc = dr["ksmc"].ToString();
                    string ybksdm = dr["ybksdm"].ToString();
                    string ybksmc = dr["ybksmc"].ToString();
                    string kssj = string.IsNullOrEmpty(dr["kssj"].ToString()) ? DateTime.Now.ToString("yyyy-MM-dd") : dr["kssj"].ToString().Substring(0, 4) + "-" + dr["kssj"].ToString().Substring(4, 2) + "-" + dr["kssj"].ToString().Substring(6, 2);
                    string jssj = string.IsNullOrEmpty(dr["jssj"].ToString()) ? "" : dr["jssj"].ToString().Substring(0, 4) + "-" + dr["jssj"].ToString().Substring(4, 2) + "-" + dr["jssj"].ToString().Substring(6, 2);
                    string jjsm = dr["jjsm"].ToString();
                    string ksfzrxm = dr["ksfzrxm"].ToString();
                    string ksfzrdh = dr["ksfzrdh"].ToString();
                    string ksylfwfw = dr["ksylfwfw"].ToString();
                    string ksclrq = string.IsNullOrEmpty(dr["ksclrq"].ToString()) ? DateTime.Now.ToString("yyyy-MM-dd") : dr["ksclrq"].ToString().Substring(0, 4) + "-" + dr["ksclrq"].ToString().Substring(4, 2) + "-" + dr["ksclrq"].ToString().Substring(6, 2);
                    string pzcwsl = dr["pzcwsl"].ToString();
                    string ybrkcws = dr["ybrkcws"].ToString();
                    string tcqh = dr["tcqh"].ToString();
                    string ysrs = dr["ysrs"].ToString();
                    string yaosrs = dr["yaosrs"].ToString();
                    string hsrs = dr["hsrs"].ToString();
                    string jsrs = dr["jsrs"].ToString();
                    string bz = "";
                    string scbz = scbz1;
                    string ywdm = "";
                    string kb = ybksdm;

                    if (scbz == "1")
                        ywdm = "3402"; //变更
                    else
                        ywdm = "3401";//上传

                    #region 入参
                    /*deptinfo节点
                     hosp_dept_codg	医院科室编码
                     caty	科别
                     hosp_dept_name	医院科室名称
                     begntime	开始时间
                     endtime	结束时间
                     itro	简介
                     dept_resper_name	科室负责人姓名
                     dept_resper_tel	科室负责人电话
                     dept_med_serv_scp	科室医疗服务范围
                     dept_estbdat	科室成立日期
                     aprv_bed_cnt	批准床位数量
                     hi_crtf_bed_cnt	医保认可床位数
                     poolarea_no	统筹区编号
                     dr_psncnt	医师人数
                     phar_psncnt	药师人数
                     nurs_psncnt	护士人数
                     tecn_psncnt	技师人数
                     memo	备注

                    */

                    kssc_deptinfo deptinfo = new kssc_deptinfo();
                    deptinfo.hosp_dept_codg = ksdm;
                    deptinfo.caty = ybksdm;
                    deptinfo.hosp_dept_name = ksmc;
                    deptinfo.begntime = kssj;
                    deptinfo.endtime = jssj;
                    deptinfo.itro = jjsm;
                    deptinfo.dept_resper_name = ksfzrxm;
                    deptinfo.dept_resper_tel = ksfzrdh;
                    deptinfo.dept_med_serv_scp = ksylfwfw;
                    deptinfo.dept_estbdat = ksclrq;
                    deptinfo.aprv_bed_cnt = pzcwsl;
                    deptinfo.hi_crtf_bed_cnt = ybrkcws;
                    deptinfo.poolarea_no = tcqh;
                    deptinfo.dr_psncnt = ysrs;
                    deptinfo.phar_psncnt = yaosrs;
                    deptinfo.nurs_psncnt = hsrs;
                    deptinfo.tecn_psncnt = jsrs;
                    deptinfo.memo = bz;

                    dynamic input = new
                    {
                        deptinfo = deptinfo
                    };

                    string ksscJson = JsonConvert.SerializeObject(deptinfo);
                    #endregion
                    WriteLog(sysdate + "  科室代码：" + ksdm + "  科室名称" + ksmc + "  上传|入参Json|" + ksscJson);

                    string Err = string.Empty;
                    int i = YBServiceRequest(ywdm, input, ref Err);
                    if (i != 1)
                    {
                        WriteLog(sysdate + "  科室代码：" + ksdm + "  科室名称" + ksmc + "上传失败|" + Err.ToString());
                        message += "  科室代码：" + ksdm + "  科室名称" + ksmc + "上传失败|" + Err.ToString() + "\r\n";


                    }
                    else
                    {
                        WriteLog(sysdate + "  科室代码：" + ksdm + "  科室名称" + ksmc + " 科室上传成功|");

                        string strSQL = string.Format(@"update bz02d set b2ybup=1 where b2ejks='{0}' or b2ksno='{0}'", ksdm);
                        liSQL.Add(strSQL);

                        strSQL = string.Format(@"delete ybkszd where ksdm ='{0}'", ksdm);
                        liSQL.Add(strSQL);

                        strSQL = string.Format(@"
                             INSERT INTO [dbo].[ybkszd]
                                   ([ksdm] ,[ksmc],[ybksdm],[ybksmc] ,[kssj],[jssj],[jjsm],[ksfzrxm]
                                   ,[ksfzrdh],[ksylfwfw],[ksclrq],[pzcwsl],[ybrkcws],[tcqh],[ysrs]
                                   ,[yaosrs],[hsrs],[jsrs],[bz],[scbz],[kb])
                                    VALUES
                                   ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}'
                                   ,'{8}','{9}','{10}','{11}','{12}','{13}','{14}'
		                           ,'{15}','{16}','{17}','{18}','{19}','{20}')",
                       ksdm, ksmc, ybksdm, ybksmc, kssj, jssj, jjsm, ksfzrxm,
                       ksfzrdh, ksylfwfw, ksclrq, pzcwsl, ybrkcws, tcqh, ysrs,
                       yaosrs, hsrs, jsrs, bz, '1', kb);
                        liSQL.Add(strSQL);
                    }
                }
                #endregion
                object[] objData = liSQL.ToArray();
                objData = CliUtils.CallMethod("sybdj", "BatExecuteSql", objData);
                if (objData[1].ToString().Equals("1"))
                {
                    WriteLog("  科室信息上传完成" + "\r\n" + message);
                    return new object[] { 0, 1, "  科室信息上传完成" + "\r\n" + message };
                }
                else
                {
                    WriteLog("  科室信息上传失败|" + objData[2].ToString());
                    return new object[] { 0, 1, "  科室信息上传失败|" + objData[2].ToString() };
                }
            }
            catch (Exception ex)
            {
                WriteLog(sysdate + "  科室信息上传异常|" + ex.Message);
                return new object[] { 0, 0, "科室信息上传异常|" + ex.Message };
            }
        }

        public static object[] YBKSXXPLSC_A(object[] objParam)
        {
            string sysdate = GetServerDateTime();
            WriteLog(sysdate + "  进入科室信息上传/变更...");
            WriteLog("进入科室信息上传/变更");
            try
            {
                CZYBH = CliUtils.fLoginUser; //操作员工号
                string jbr = CliUtils.fUserName;
                YWZQH = CliUtils.fLoginYbNo.Split('|')[0].ToString();    //业务周期号
                string jysj = Convert.ToDateTime(sysdate).ToString("yyyyMMddHHmmss");

                string bg = objParam[0].ToString();
                List<kssc_deptinfo> deptinfos = objParam[1] as List<kssc_deptinfo>;
                if (deptinfos == null || deptinfos.Count == 0)
                {
                    return new object[] { 0, 0, "没有查询到已配对的科室信息" };
                }
                string ywcode = bg == "1" ? "3402" : "3401";
                dynamic dy = new { deptinfo = deptinfos[0] };

                string Err = string.Empty;
                string ksscJson = JsonConvert.SerializeObject(dy);
                WriteLog(sysdate + "  入参|" + ksscJson);
                int i1 = YBServiceRequest(ywcode, dy, ref Err);
                WriteLog(sysdate + "  出参|" + Err);
                if (i1 != 1)
                {
                    return new object[] { 0, 0, "科室信息上传/变更失败|" + (Err ?? "") };
                }

                JObject ret = JsonConvert.DeserializeObject(Err) as JObject;
                #region 数据处理
                List<string> liSql = new List<string>();
                for (int i = 0; i < 1; i++)
                {
                    kssc_deptinfo deptinfo = deptinfos[i];

                    string sqlStr = "";
                    sqlStr = string.Format(@"delete ybkszd where ksdm = '{0}';
                        insert into ybkszd (ksdm,ksmc,ybksdm,ybksmc,kb,kssj,jssj,ksfzrxm,ksfzrdh,ksclrq,pzcwsl,ybrkcws,tcqh,ysrs,jsrs,ys1rs,hsrs,bz,itro,scbz) values
                        ('{0}','{1}','{0}','{1}','{2}','{3}','','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}','{15}',1)",
                               deptinfo.hosp_dept_codg, deptinfo.hosp_dept_name, deptinfo.caty, deptinfo.begntime,
                               deptinfo.dept_resper_name, deptinfo.dept_resper_tel, deptinfo.dept_estbdat, deptinfo.aprv_bed_cnt, deptinfo.hi_crtf_bed_cnt,
                               deptinfo.poolarea_no, deptinfo.dr_psncnt, deptinfo.tecn_psncnt, deptinfo.phar_psncnt, deptinfo.nurs_psncnt, deptinfo.memo, deptinfo.itro);

                    if (!string.IsNullOrEmpty(sqlStr))
                    {
                        liSql.Add(sqlStr);
                    }
                }

                object[] obj = liSql.ToArray();
                obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);
                if (obj[1].ToString().Equals("1"))
                {
                    WriteLog(sysdate + "    科室信息上传/变更成功|");
                    return new object[] { 0, 0, "科室信息上传/变更成功|" };
                }
                else
                {
                    WriteLog(sysdate + "    科室信息上传/变更失败|本地数据操作失败");
                    return new object[] { 0, 0, "科室信息上传/变更失败|本地数据操作失败" };
                }
                #endregion
            }
            catch (Exception ex)
            {
                WriteLog(sysdate + "  科室信息上传/变更异常|" + ex.Message);
                return new object[] { 0, 0, "科室信息上传/变更异常|" + ex.Message };
            }
        }
        #endregion

        #region 3403医保科室信息撤销
        public static object[] YBKSXXSCCX(object[] objParam)
        {
            string sysdate = GetServerDateTime();
            WriteLog(sysdate + "  进入科室信息上传撤销...");
            WriteLog("进入科室信息上传撤销|HIS传参|" + string.Join(",", objParam));
            try
            {
                CZYBH = CliUtils.fLoginUser; //操作员工号
                string jbr = CliUtils.fUserName;
                YWZQH = CliUtils.fLoginYbNo.Split('|')[0].ToString();    //业务周期号
                string jysj = Convert.ToDateTime(sysdate).ToString("yyyyMMddHHmmss");

                string ksbm = objParam[0].ToString();

                #region 对照数据
                StringBuilder inParam = new StringBuilder();
                string sDate = DateTime.Now.ToString("yyyyMMddHHmmss");
                string strSql = string.Format(@"select ksdm,ksmc,ybksdm,ybksmc,kssj,tcqh from ybkszd where scbz=1");
                if (!string.IsNullOrEmpty(ksbm))
                    strSql += string.Format(@" and ksdm='{0}'", ksbm);

                DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                if (ds.Tables[0].Rows.Count == 0)
                    return new object[] { 0, 0, "当前科室未上传" };

                List<string> liSQL = new List<string>();

                string ksdm = ds.Tables[0].Rows[0]["ksdm"].ToString();
                string ksmc = ds.Tables[0].Rows[0]["ksmc"].ToString();
                string ybksdm = ds.Tables[0].Rows[0]["ybksdm"].ToString();
                string ybksmc = ds.Tables[0].Rows[0]["ybksmc"].ToString();
                string kssj = ds.Tables[0].Rows[0]["kssj"].ToString();
                string tcqh = ds.Tables[0].Rows[0]["tcqh"].ToString();
                string ywdm = "3403";//上传撤销

                #region 入参
                /*data节点
                医院科室编码
                医院科室名称
                开始时间
                */
                kssccx_data data = new kssccx_data();
                data.hosp_dept_codg = ksdm;
                data.hosp_dept_name = ksmc;
                data.begntime = kssj;
                dynamic input = new
                {
                    data = data
                };
                string ksscJson = JsonConvert.SerializeObject(input);
                #endregion
                WriteLog(sysdate + "  科室代码：" + ksdm + "  科室名称" + ksmc + "  上传撤销|入参Json|" + ksscJson);
                string Err = string.Empty;
                int i1 = YBServiceRequest(ywdm, input, ref Err);
                if (i1 == 1)
                {

                    WriteLog(sysdate + "  科室代码：" + ksdm + "  科室名称" + ksmc + " 科室上传撤销成功|");

                    string strSQL = string.Format(@"delete ybkszd where ksdm='{0}'", ksdm);

                    //CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);

                    //WriteLog("  科室信息上传撤销成功|操作数据库成功");
                    //return new object[] { 0, 1, "  科室信息上传撤销成功|操作数据库成功" };
                    //string strSQL = string.Format(@"update ybkszd set scbz=0 where ksdm='{0}'", ksdm);
                    liSQL.Add(strSQL);
                    strSQL = string.Format(@"update bz02d set b2ybup=0 where b2ejks='{0}' or b2ksno='{0}'", ksdm);
                    liSQL.Add(strSQL);
                    object[] objData = liSQL.ToArray();
                    objData = CliUtils.CallMethod("sybdj", "BatExecuteSql", objData);
                    if (objData[1].ToString().Equals("1"))
                    {
                        WriteLog("  科室信息上传撤销成功|操作数据库成功");
                        return new object[] { 0, 1, "  科室信息上传撤销成功|操作数据库成功" };
                    }
                    else
                    {
                        WriteLog("  科室信息上传撤销失败|" + objData[2].ToString());
                        return new object[] { 0, 1, "  科室信息上传撤销失败|" + objData[2].ToString() };
                    }

                }
                else
                {
                    WriteLog(sysdate + "  科室代码：" + ksdm + "  科室名称" + ksmc + "上传撤销失败|" + Err.ToString());
                    return new object[] { 0, 1, "  科室代码：" + ksdm + "  科室名称" + ksmc + "上传撤销失败|" + Err };
                }
                #endregion

            }
            catch (Exception ex)
            {
                WriteLog(sysdate + "  科室信息上传撤销异常|" + ex.Message);
                return new object[] { 0, 0, "科室信息上传撤销异常|" + ex.Message };
            }
        }
        #endregion


        #region 结算单据
        #region 门诊结算单补打

        static GocentPara Para = new GocentPara();
        public static object[] YBMZJSD(object[] objParam)
        {
            try
            {
                string jzlsh = objParam[0].ToString();
                string djh = objParam[1].ToString();
                string strSql = string.Format(@"select * from ybfyjsdr where jzlsh='{0}' and djhin = '{1}' and cxbz=1", jzlsh, djh);
                DataSet ds;
                ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                if (ds.Tables[0].Rows.Count == 0)
                    return new object[] { 0, 0, "该患者无结算信息" };
                string spath = Para.sys_parameter("BZ", "BZ0007");
                spath = string.IsNullOrEmpty(spath) ? @"C:\Program Files (x86)\Infolight\EEP2012\EEPNetClient\FastReport" : spath;
                string cfile = "";
                string sfile = "";
                string oldNew = "new";
                string A4 = "A";


                #region 新sql
                strSql = @" select a.jzlsh,a.xm, case when b.xb='1' then '男' else '女' end xb,'H36032300004' as yljgbh,y.bzmem8 as yljgdj,y.bzname as yljgmc,
									isnull(c1.GMSFHM,'') as  idcard,a.sysdate as jssj,c1.DWMC as dwmc, c1.CSRQ as csrq,--convert(varchar,convert(date,substring(c1.CSRQ,0,9)))
                                    b.ghdjsj as ryrq,
                                    a.cyrq as cyrq,
									isnull((select b5sfnm from bz05h where b5sfno=cf.sflb),'其他') as xmmc,sum(cf.je) as xmje,
									sum(isnull(cf.fhzcfwje,0.00)) as xmjlje,
									sum(isnull(cf.xxzfje,0.00)) as xmylje,
									sum(isnull(cf.zfje,0.00)) as xmzfje ,
									sum(isnull(cf.cxjzfje,0.00)) as xmqtje,
                                   '' as zyts,
									--(case c.psn_type when '11' then '在职' when '21' then '退休' when '31' then '离休'when '32' then '老红军'
									--when '41' then '学龄前儿童' when '42' then '中小学生'when '43' then '大学生'when '50' then '成年居民' 
									--else '' end) as rylb,
                                    i.bzname rylb,
									(select bzname from bztbd where bzcodn='hi_type' and bzkeyx=c.insutype ) as xz, e.bzname as rysflb, 
									b.ksmc as kb,'' as zych,
                                    a.grbh,z.bzname as yllb,emp_name dwmc,
                                    e.bzname as zlzt,
                                    a.ybjzlsh,a.ylfze,a.qfxbz,a.bcfhyb,isnull(a.cxjfy,0)+isnull(a.zffy,0) zffy,a.xxzfje,zlfy,tcjjzf, a.xjzf,a.zhzf,a.dejjzf,a.mzjzfy,a.bcsydbzffy,a.bcbcbxzffy,a.dbjjzf,a.qtjjzf as qybcylzf,a.qybcylbxjjzf,a.cxjfy,a.yyfdfy,
                                    substring(convert(varchar(10),isnull(jbylbxtcjjzfbl,0)*100),1,4)+'%' bxbl,gwybzjjzf as gwybzzf,
									a.jbr jbr,zgrssbzf,qtjjzf,a.jbr,jsrq,'' as cyzd,b.bzbm,b.bzmc,'' as  sysdate,a.qfbzfy,
									a.jslsh as jsid,h.m1telp as tel,case when a.gwybz = '0' then '否' else '是' end gwybz,'0' as yljzjjzf
									,(select i.DQBH from YBICKXX i where i.grbh=a.grbh) as tcqh,a.zbxje as zbxje,j.bzname tcqmc
                                    from ybfyjsdr a
                                    left join ybmzzydjdr b on a.jzlsh=b.jzlsh and a.ybjzlsh=b.ybjzlsh and a.cxbz=b.cxbz
									left join YBICKXX c1 on a.grbh=c1.grbh  
                                    left join ybrycbxx c on a.grbh=c.grbh   and insutype=c1.xzlx
									left join ybrysfxx s on s.grbh=a.grbh
                                    left join bztbd d on d.bzcodn='YL' and a.yllb=d.bzkeyx
                                    left join bztbd z on z.bzcodn='med_type' and a.yllb=z.bzkeyx
                                    left join bztbd e on e.bzcodn='psn_type' and a.yldylb=e.bzkeyx
                                    left join bztbd i on e.bzcodn='psn_idet_type' and a.yldylb=i.bzkeyx
                                    left join bztbd y on y.bzcodn='CM' 
									left join bz01h f on a.jbr=f.b1empn
                                    left join mz01h h on h.m1ghno=a.jzlsh 
									left join ybcfmxscfhdr cf on cf.jzlsh=a.jzlsh and cf.cxbz = '1' and a.jylsh = cf.jylsh
									left join bztbd j on j.bzcodn='DQ' and j.bzkeyx=a.tcqh
                                   where  a.cxbz=1 and a.jzlsh='{0}' and a.djhin = '{1}'
									group by a.jzlsh,a.xm,b.xb, y.bzmem8,y.bzname ,c1.GMSFHM,jssj
									,c1.DWMC,c1.CSRQ,ryrq, cyrq,cf.sflb,b.ghdjsj,a.cyrq
									,rylb, c.insutype,rysflb,b.ksmc,c.psn_type,s.psn_idet_type,
                                    a.grbh,d.bzname ,emp_name,i.bzname, e.bzname,
                                    a.ybjzlsh,a.ylfze,a.qfxbz,a.bcfhyb,a.cxjfy,a.zffy,a.xxzfje,zlfy,tcjjzf,a.xjzf,a.zhzf,a.dejjzf,a.mzjzfy,
									a.bcsydbzffy,a.bcbcbxzffy,a.dbjjzf,
									a.qtjjzf,a.qybcylbxjjzf,a.cxjfy,a.yyfdfy,gwybzjjzf,
									a.jbr,zgrssbzf,qtjjzf,a.jbr,jsrq  ,b.bzbm,b.bzmc,a.qfbzfy,a.jslsh ,a.sysdate,b.ghdjsj,
									a.jbylbxtcjjzfbl,h.m1telp ,a.gwybz,z.bzname,a.zbxje,j.bzname
union all 
									 select a.jzlsh,a.xm, case when b.xb='1' then '男' else '女' end xb,'H36032300004' as yljgbh,y.bzmem8 as yljgdj,y.bzname as yljgmc,
									isnull(c1.GMSFHM,'') as  idcard,a.sysdate as jssj,c1.DWMC as dwmc, c1.CSRQ as csrq,--convert(varchar,convert(date,substring(c1.CSRQ,0,9)))
                                    b.ghdjsj as ryrq,
                                    a.cyrq as cyrq,
									'合计' as xmmc,
                                   (select sum(isnull(je,0.00)) from ybcfmxscfhdr where jzlsh='{0}' and cxbz =1 and jylsh = a.jylsh ) as xmje, 
									(select sum(isnull(fhzcfwje,0.00)) from ybcfmxscfhdr where jzlsh='{0}' and cxbz =1 and jylsh = a.jylsh ) as xmjlje,
									(select sum(isnull(xxzfje,0.00)) from ybcfmxscfhdr where jzlsh='{0}' and cxbz =1 and jylsh = a.jylsh ) as xmylje,
									(select sum(isnull(zfje,0.00)) from ybcfmxscfhdr where jzlsh='{0}' and cxbz =1 and jylsh = a.jylsh ) as xmzfje ,
									(select sum(isnull(cxjzfje,0.00)) from ybcfmxscfhdr where jzlsh='{0}' and cxbz =1 and jylsh = a.jylsh ) as xmqtje,
                                   '' as zyts,
									--(case c.psn_type when '11' then '在职' when '21' then '退休' when '31' then '离休'when '32' then '老红军'
									--when '41' then '学龄前儿童' when '42' then '中小学生'when '43' then '大学生'when '50' then '成年居民' 
									--else '' end) as rylb,
                                    i.bzname rylb,
									(select bzname from bztbd where bzcodn='hi_type' and bzkeyx=c.insutype ) as xz, e.bzname as rysflb, 
									b.ksmc as kb,'' as zych,
                                    a.grbh,z.bzname as yllb,emp_name dwmc,
                                    e.bzname as zlzt,
                                    a.ybjzlsh,a.ylfze,a.qfxbz,a.bcfhyb,isnull(a.cxjfy,0)+isnull(a.zffy,0) zffy,a.xxzfje,zlfy,tcjjzf, a.xjzf,a.zhzf,a.dejjzf,a.mzjzfy,a.bcsydbzffy,a.bcbcbxzffy,a.dbjjzf,a.qtjjzf as qybcylzf,a.qybcylbxjjzf,a.cxjfy,a.yyfdfy,
                                    substring(convert(varchar(10),isnull(jbylbxtcjjzfbl,0)*100),1,4)+'%' bxbl,gwybzjjzf as gwybzzf,
									a.jbr jbr,zgrssbzf,qtjjzf,a.jbr,jsrq,'' as cyzd,b.bzbm,b.bzmc,'' as  sysdate,a.qfbzfy,
									a.jslsh as jsid,h.m1telp as tel,case when a.gwybz = '0' then '否' else '是' end gwybz,'0' as yljzjjzf,(select i.DQBH from YBICKXX i where i.grbh=a.grbh) as tcqh,a.zbxje,j.bzname tcqmc
                                    from ybfyjsdr a
                                    left join ybmzzydjdr b on a.jzlsh=b.jzlsh and a.ybjzlsh=b.ybjzlsh and a.cxbz=b.cxbz
									left join YBICKXX c1 on a.grbh=c1.grbh  
                                    left join ybrycbxx c on a.grbh=c.grbh   and insutype=c1.xzlx
									left join ybrysfxx s on s.grbh=a.grbh
                                    left join bztbd d on d.bzcodn='YL' and a.yllb=d.bzkeyx
                                    left join bztbd z on z.bzcodn='med_type' and a.yllb=z.bzkeyx
                                    left join bztbd e on e.bzcodn='psn_type' and a.yldylb=e.bzkeyx
                                    left join bztbd i on e.bzcodn='psn_idet_type' and a.yldylb=i.bzkeyx
                                    left join bztbd y on y.bzcodn='CM' 
									left join bz01h f on a.jbr=f.b1empn
                                    left join mz01h h on h.m1ghno=a.jzlsh 
									left join bztbd j on j.bzcodn='DQ' and j.bzkeyx=a.tcqh
                                   where  a.cxbz=1 and a.jzlsh='{0}' and a.djhin = '{1}'";
                strSql = string.Format(strSql, jzlsh, djh); //and a.djhin = '{1}'
                #endregion
                ds.Tables.Clear();

                WriteLog("门诊结算单Sql：" + strSql);
                ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                WriteLog(JsonConvert.SerializeObject(ds));
                cfile = Application.StartupPath + @"\FastReport\YB\江西芦溪县中医医院国家医保结算单.frx";
                sfile = spath + @"\YB\江西芦溪县中医医院国家医保结算单.frx";
                oldNew = "new";
                A4 = "A";
                //ds.WriteXmlSchema(@"D:\mm.xml");         

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
                if (oldNew.Equals("old"))
                    (report.FindObject("zgrssbzf") as TextObject).Text = ds.Tables[0].Rows[0]["zgrssbzf"].ToString();
                if (oldNew.Equals("old") && A4.Equals("A4"))
                {
                    (report.FindObject("zgrssbzf1") as TextObject).Text = ds.Tables[0].Rows[0]["zgrssbzf"].ToString();
                    (report.FindObject("zgrssbzf2") as TextObject).Text = ds.Tables[0].Rows[0]["zgrssbzf"].ToString();
                }
                report.Show();
                report.Dispose();
                return new object[] { 0, 1, "医保提示：打印成功", "提示" };

            }
            catch (Exception e)
            {
                return new object[] { 0, 0, "医保提示：打印失败", "提示" };
            }
            

        }
        #endregion

        #region 住院结算单补打

        public static object[] YBZYJSD(object[] objParam)
        {
            try
            {
                string jzlsh = objParam[0].ToString();
                string djh = objParam[1].ToString();
                string strSql = string.Format(@"select * from ybfyjsdr where jzlsh='{0}' and cxbz=1", jzlsh);
                DataSet ds;
                ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                if (ds.Tables[0].Rows.Count == 0)
                    return new object[] { 0, 0, "该患者无结算信息" };
                string spath = Para.sys_parameter("BZ", "BZ0007");
                spath = string.IsNullOrEmpty(spath) ? @"C:\Program Files (x86)\Infolight\EEP2012\EEPNetClient\FastReport" : spath;
                string cfile = "";
                string sfile = "";
                string oldNew = "new";
                string A4 = "A";
                #region 老sql
                //strSql = string.Format(@"select a.jzlsh,a.xm, case when b.xb='1' then '男' else '女' end xb,'H36032300004' as yljgbh,y.bzmem8 as yljgdj,y.bzname as yljgmc,
                //isnull(c1.GMSFHM,'') as  idcard,a.sysdate as jssj,c1.DWMC as dwmc, convert(varchar,c1.CSRQ) as csrq,
                //                           convert(varchar,convert(date,substring(b.ghdjsj,0,9))) as ryrq,
                //                           convert(varchar,convert(date,substring(a.cyrq,0,9))) as cyrq,
                //cf.yyxmmc as xmmc,cf.je as xmje,(case cf.sfxmdj when '1' then isnull(cf.fhzcfwje,0.00) else 0.00 end) as xmjlje,
                //(case cf.sfxmdj when '2' then isnull(cf.fhzcfwje,0.00) else 0.00 end) as xmylje,cf.zfje as xmzfje ,
                //(cf.je-cf.fhzcfwje-cf.zfje) as xmqtje,
                //                           DATEDIFF(day,convert(date,substring(b.ghdjsj,0,9)),convert(date,substring(a.cyrq,0,9)))+1 as zyts,
                //(case c.psn_type when '11' then '在职' when '21' then '退休' when '31' then '离休'when '32' then '老红军'
                //when '41' then '学龄前儿童' when '42' then '中小学生'when '43' then '大学生'when '50' then '成年居民' 
                //else '' end) as rylb,
                //(case c.insutype when '310' then '职工基本医疗保险 ' when '320' then '公务员医疗补助' when '330' 
                //then '大额医疗费用补助'when '340' then '离休人员医疗保障' when '390' then '城乡居民基本医疗保险' when '392' then '城乡居民大病医疗保险'
                //when '510' then '生育保险' else '' end) as xz,
                //(case s.psn_idet_type when '01' then '城市三无 ' when '02' then '农村五保户' when '03' 
                //then '残疾'when '04' then '失独父母' when '06' then '医疗救助人员' 
                //when '07' then '医疗照顾人员' when '99' then '其他' else '' end) as rysflb, 
                //b.ksmc as kb,h.z1cwid as zych,
                //                           a.grbh,d.bzname as yllb,emp_name dwmc,
                //                           e.bzname as zlzt,
                //                           a.ybjzlsh,a.ylfze,a.qfxbz,a.bcfhyb,isnull(a.cxjfy,0)+isnull(a.zffy,0) zffy,a.xxzfje,zlfy,tcjjzf,a.xjzf,a.zhzf,a.dejjzf,a.mzjzfy,a.bcsydbzffy,a.bcbcbxzffy,a.dbjjzf,a.qtjjzf as qybcylzf,a.qybcylbxjjzf,a.cxjfy,a.yyfdfy,
                //                           substring(convert(varchar(10),isnull(jbylbxtcjjzfbl,0)*100),1,4)+'%' bxbl,gwybzjjzf as gwybzzf,
                //a.jbr jbr,zgrssbzf,qtjjzf,a.jbr,jsrq,h.z1bznm as cyzd,b.bzbm,b.bzmc,getdate() as  sysdate,a.qfbzfy,a.jslsh as jsid
                //                           from ybfyjsdr a
                //                           left join ybmzzydjdr b on a.jzlsh=b.jzlsh and a.ybjzlsh=b.ybjzlsh and a.cxbz=b.cxbz
                //                           right join ybrycbxx c on a.grbh=c.grbh   and insutype=a.xzlx
                //left join YBICKXX c1 on a.grbh=c1.grbh  
                //left join ybrysfxx s on s.grbh=a.grbh
                //                           left join bztbd d on d.bzcodn='YL' and a.yllb=d.bzkeyx
                //                           left join bztbd e on e.bzcodn='psn_type' and a.yldylb=e.bzkeyx
                //                           left join bztbd y on y.bzcodn='CM' 
                //left join bz01h f on a.jbr=f.b1empn
                //                           left join zy01h h on h.z1zyno=a.jzlsh 
                //left join ybcfmxscfhdr cf on cf.jzlsh=a.jzlsh
                //                           where  a.jzlsh='{0}'and a.cxbz=1", jzlsh);
                #endregion

                #region 新sql
                strSql = @"select a.jzlsh,a.xm, case when b.xb='1' then '男' else '女' end xb,'H36032300004' as yljgbh,y.bzmem8 as yljgdj,y.bzname as yljgmc,
									isnull(c1.GMSFHM,'') as  idcard,a.sysdate as jssj,c1.DWMC as dwmc, convert(varchar,c1.CSRQ) as csrq,
                                    convert(varchar,convert(date,substring(b.ghdjsj,0,9))) as ryrq,
                                    convert(varchar,convert(date,substring(a.cyrq,0,9))) as cyrq,
									(select b5sfnm from bz05h where b5sfno=cf.sflb)as xmmc,sum(cf.je) as xmje,--(select distinct b5sfnm from bz05h where b5sfno=cf.sflb)
									sum(isnull(cf.fhzcfwje,0.00)) as xmjlje,
									sum(isnull(cf.xxzfje,0.00)) as xmylje,
									sum(isnull(cf.zfje,0.00)) as xmzfje ,
									sum(isnull(cf.cxjzfje,0.00)) as xmqtje,
                                    DATEDIFF(day,convert(date,substring(b.ghdjsj,0,9)),convert(date,substring(a.cyrq,0,9)))+1 as zyts,
									--(case c.psn_type when '11' then '在职' when '21' then '退休' when '31' then '离休'when '32' then '老红军'
									--when '41' then '学龄前儿童' when '42' then '中小学生'when '43' then '大学生'when '50' then '成年居民' 
									--else '' end) as rylb, 
                                    i.bzname rylb,
									(select bzname from bztbd where bzcodn='hi_type' and bzkeyx=c.insutype ) as xz, e.bzname as rysflb, 
									b.ksmc as kb,h.z1cwid as zych,
                                    a.grbh,z.bzname as yllb,emp_name dwmc,
                                    e.bzname as zlzt,
                                    a.ybjzlsh,a.ylfze,a.qfxbz,a.bcfhyb,isnull(a.cxjfy,0)+isnull(a.zffy,0) zffy,a.xxzfje,zlfy,tcjjzf,a.xjzf,a.zhzf,a.dejjzf,a.mzjzfy,a.bcsydbzffy,a.bcbcbxzffy,a.dbjjzf,a.qtjjzf as qybcylzf,a.qybcylbxjjzf,a.cxjfy,a.yyfdfy,
                                    substring(convert(varchar(10),isnull(jbylbxtcjjzfbl,0)*100),1,4)+'%' bxbl,gwybzjjzf as gwybzzf,
									a.jbr jbr,zgrssbzf,qtjjzf,a.jbr,jsrq,h.z1bznm as cyzd,b.bzbm,b.bzmc,getdate() as  sysdate,a.qfbzfy,a.jslsh as jsid,h.z1lxdh as tel,case when a.gwybz = '0' then '否' else '是' end gwybz,'0' as yljzjjzf
									,(select i.DQBH from YBICKXX i where i.grbh=a.grbh) as tcqh,a.zbxje as zbxje,j.bzname tcqmc
                                    from ybfyjsdr a
                                    left join ybmzzydjdr b on a.jzlsh=b.jzlsh and a.ybjzlsh=b.ybjzlsh and a.cxbz=b.cxbz
									left join YBICKXX c1 on a.grbh=c1.grbh  
                                    left join ybrycbxx c on a.grbh=c.grbh   and insutype=c1.xzlx
									left join ybrysfxx s on s.grbh=a.grbh
                                    left join bztbd d on d.bzcodn='YL' and a.yllb=d.bzkeyx 
                                    left join bztbd z on z.bzcodn='med_type' and a.yllb=z.bzkeyx
                                    left join bztbd e on e.bzcodn='psn_type' and a.yldylb=e.bzkeyx
                                    left join bztbd i on e.bzcodn='psn_idet_type' and a.yldylb=i.bzkeyx
                                    left join bztbd y on y.bzcodn='CM' 
									left join bz01h f on a.jbr=f.b1empn
                                    left join zy01h h on h.z1zyno=a.jzlsh 
									left join ybcfmxscfhdr cf on cf.jzlsh=a.jzlsh and cf.cxbz='1'
									left join bztbd j on j.bzcodn='DQ' and j.bzkeyx=a.tcqh
                                   where  a.cxbz=1 and a.jzlsh='{0}'
									group by a.jzlsh,a.xm,b.xb, y.bzmem8,y.bzname ,c1.GMSFHM,jssj
									,c1.DWMC,c1.CSRQ,ryrq, cyrq,cf.sflb,b.ghdjsj,a.cyrq
									,rylb, c.insutype,rysflb,b.ksmc,h.z1cwid,c.psn_type,s.psn_idet_type,
                                    a.grbh,d.bzname ,emp_name,i.bzname, e.bzname,
                                    a.ybjzlsh,a.ylfze,a.qfxbz,a.bcfhyb,a.cxjfy,a.zffy,a.xxzfje,zlfy,tcjjzf,a.xjzf,a.zhzf,a.dejjzf,a.mzjzfy,a.bcsydbzffy,a.bcbcbxzffy,a.dbjjzf,
									a.qtjjzf,a.qybcylbxjjzf,a.cxjfy,a.yyfdfy,gwybzjjzf,j.bzname,
									a.jbr,zgrssbzf,qtjjzf,a.jbr,jsrq,h.z1bznm ,b.bzbm,b.bzmc,a.qfbzfy,a.jslsh ,a.sysdate,b.ghdjsj,a.jbylbxtcjjzfbl,h.z1lxdh ,a.gwybz,z.bzname,a.zbxje
									union all 
									select a.jzlsh,a.xm, case when b.xb='1' then '男' else '女' end xb,'H36032300004' as yljgbh,y.bzmem8 as yljgdj,y.bzname as yljgmc,
									isnull(c1.GMSFHM,'') as  idcard,a.sysdate as jssj,c1.DWMC as dwmc, convert(varchar,c1.CSRQ) as csrq,
                                    convert(varchar,convert(date,substring(b.ghdjsj,0,9))) as ryrq,
                                    convert(varchar,convert(date,substring(a.cyrq,0,9))) as cyrq,
									'合计' as xmmc,
									(select sum(isnull(je,0.00)) from ybcfmxscfhdr where jzlsh='{0}' and cxbz =1) as xmje,
									(select sum(isnull(fhzcfwje,0.00)) from ybcfmxscfhdr where jzlsh='{0}' and cxbz =1) as xmjlje,
									(select sum(isnull(xxzfje,0.00)) from ybcfmxscfhdr where jzlsh='{0}' and cxbz =1) as xmylje,
									(select sum(isnull(zfje,0.00)) from ybcfmxscfhdr where jzlsh='{0}' and cxbz =1) as xmzfje ,
									(select sum(isnull(cxjzfje,0.00)) from ybcfmxscfhdr where jzlsh='{0}' and cxbz =1) as xmqtje,
                                    DATEDIFF(day,convert(date,substring(b.ghdjsj,0,9)),convert(date,substring(a.cyrq,0,9)))+1 as zyts,
									--(case c.psn_type when '11' then '在职' when '21' then '退休' when '31' then '离休'when '32' then '老红军'
									--when '41' then '学龄前儿童' when '42' then '中小学生'when '43' then '大学生'when '50' then '成年居民' 
									--else '' end) as rylb, 
                                    i.bzname rylb,
									(select bzname from bztbd where bzcodn='hi_type' and bzkeyx=c.insutype ) as xz, e.bzname as rysflb, 
									b.ksmc as kb,h.z1cwid as zych,
                                    a.grbh,z.bzname as yllb,emp_name dwmc,
                                    e.bzname as zlzt,
                                    a.ybjzlsh,a.ylfze,a.qfxbz,a.bcfhyb,isnull(a.cxjfy,0)+isnull(a.zffy,0) zffy,a.xxzfje,zlfy,tcjjzf,a.xjzf,a.zhzf,a.dejjzf,a.mzjzfy,a.bcsydbzffy,a.bcbcbxzffy,a.dbjjzf,a.qtjjzf as qybcylzf,a.qybcylbxjjzf,a.cxjfy,a.yyfdfy,
                                    substring(convert(varchar(10),isnull(jbylbxtcjjzfbl,0)*100),1,4)+'%' bxbl,gwybzjjzf as gwybzzf,
									a.jbr jbr,zgrssbzf,qtjjzf,a.jbr,jsrq,h.z1bznm as cyzd,b.bzbm,b.bzmc,getdate() as  sysdate,a.qfbzfy,a.jslsh as jsid,h.z1lxdh as tel,case when a.gwybz = '0' then '否' else '是' end gwybz,'0' as yljzjjzf
									,(select i.DQBH from YBICKXX i where i.grbh=a.grbh) as tcqh,a.zbxje as zbxje,j.bzname tcqmc
                                    from ybfyjsdr a
                                    left join ybmzzydjdr b on a.jzlsh=b.jzlsh and a.ybjzlsh=b.ybjzlsh and a.cxbz=b.cxbz
									left join YBICKXX c1 on a.grbh=c1.grbh  
                                    left join ybrycbxx c on a.grbh=c.grbh   and insutype=c1.xzlx
									left join ybrysfxx s on s.grbh=a.grbh
                                    left join bztbd d on d.bzcodn='YL' and a.yllb=d.bzkeyx 
                                    left join bztbd z on z.bzcodn='med_type' and a.yllb=z.bzkeyx
                                    left join bztbd e on e.bzcodn='psn_type' and a.yldylb=e.bzkeyx
                                    left join bztbd i on e.bzcodn='psn_idet_type' and a.yldylb=i.bzkeyx
                                    left join bztbd y on y.bzcodn='CM' 
									left join bz01h f on a.jbr=f.b1empn
                                    left join zy01h h on h.z1zyno=a.jzlsh  
									left join bztbd j on j.bzcodn='DQ' and j.bzkeyx=a.tcqh
                                   where  a.cxbz=1 and a.jzlsh='{0}'";
                strSql = string.Format(strSql, jzlsh, djh);
                #endregion
                ds.Tables.Clear();

                WriteLog("住院结算单Sql：" + strSql);
                ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                WriteLog(JsonConvert.SerializeObject(ds));
                cfile = Application.StartupPath + @"\FastReport\YB\江西芦溪县中医医院国家医保结算单.frx";
                sfile = spath + @"\YB\江西芦溪县中医医院国家医保结算单.frx";
                oldNew = "new";
                A4 = "A";
                //ds.WriteXmlSchema(@"D:\mm.xml");         

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
                if (oldNew.Equals("old"))
                    (report.FindObject("zgrssbzf") as TextObject).Text = ds.Tables[0].Rows[0]["zgrssbzf"].ToString();
                if (oldNew.Equals("old") && A4.Equals("A4"))
                {
                    (report.FindObject("zgrssbzf1") as TextObject).Text = ds.Tables[0].Rows[0]["zgrssbzf"].ToString();
                    (report.FindObject("zgrssbzf2") as TextObject).Text = ds.Tables[0].Rows[0]["zgrssbzf"].ToString();
                }
                report.Show();
                report.Dispose();
                return new object[] { 0, 1, "医保提示：打印成功", "提示" };
            }
            catch(Exception e)
            {

                return new object[] { 0, 0, "医保提示：打印失败", "提示" };
            }
           


        }
        #endregion

        #region 门诊结算清单
        public static object[] YBMZJSQD(object[] objParam)
        {
            WriteLog("进入门诊结算单打印.....");
            try
            {
                string jzlsh = objParam[0].ToString();
                string djh = objParam[1].ToString();
                string strSql = string.Format(@"select b.XM,case when b.xb=1 then '男' else '女' end xb, zjlx,a.jslsh jsid,
						b.dwmc,a.jzlsh zyh,b.grbh,b.kh,b.GMSFHM,a.jslsh,isnull(d.bzname,'') yllb,
						isnull(g.bzname,'') tcqh,
						isnull(e.bzname,'') xzlx,
						isnull(f.bzname,'') yldylb,b.nl,convert(varchar(20), a.sysdate,120) fyjssj,
                       c.bzname yldylbnm,d.bzname fylb,bzmc,'' ryrq,'' cyrq,a.cxjzssq,a.yldylb,a.jytxm,
                            isnull(a.xxzfje,0.0) xxzfje,isnull(a.fhjbylfy,0.0) fhjbylfy,isnull(a.grfdzje,0.0) grfdzje,
	                       isnull(a.jjzfze,0.0) jjzfze,isnull(a.tcjjzf,0.0) tcjjzf,isnull(a.gwybzjjzf,0.0) gwybzjjzf,
	                       isnull(a.grzhgjzfje,0.0) grzhgjzfje,
	                       isnull(a.qtjjzf,0.0) qtjjzf,
	                       isnull(a.yyfdfy,0.0) yyfdfy,
                            isnull(a.ylfze,0.0) ylfze,isnull(a.zffy,0.0) zffy,
					       isnull(a.ylzlfy,0.0) ylzlfy,isnull(a.nrbcbxfwndfy,0.0) nrbcbxfwndfy,
					       isnull(a.bcbxbxqqlj,0.0) bcbxbxqqlj,isnull(a.zycs,0.0) zycs,
					       isnull(a.qfbzfy,0.0) qfbzfy,isnull(a.qqlj,0.0) qqlj,
					       isnull(a.tcfdjey,0.0) tcfdjey,isnull(a.tcfdjee,0.0)tcfdjee,
	                       isnull(a.tcfdjes,0.0) tcfdjes, isnull(a.tcfdjesi,0.0) tcfdjesi,isnull(a.tcfdjew,0.0) tcfdjew,
					       isnull(a.tcfdzfy,0.0) tcfdzfy, isnull(a.tcfdzfe,0.0) tcfdzfe, isnull(a.tcfdzfs,0.0) tcfdzfs, isnull(a.tcfdzfsi,0.0) tcfdzfsi,
					       isnull(a.tcfdzfw,0.0) tcfdzfw, isnull(a.tcfdzfbly,0.0) tcfdzfbly, isnull(a.tcfdzfble,0.0) tcfdzfble, isnull(a.tcfdzfbls,0.0) tcfdzfbls,
					       isnull(a.tcfdzfblsi,0.0) tcfdzfblsi, isnull(a.tcfdzfblw,0.0) tcfdzfblw,
	                       isnull(a.defdzfy,0.0) defdzfy, isnull(a.defdzfe,0.0) defdzfe, isnull(a.defdzfs,0.0) defdzfs, isnull(a.defdzfbly,0.0) defdzfbly,
					       isnull(a.defdzfble,0.0) defdzfble, isnull(a.defdzfbls,0.0) defdzfbls, isnull(a.defdjey,0.0) defdjey, isnull(a.defdjee,0.0) defdjee,
					       isnull(a.defdjes,0.0) defdjes,isnull(a.gwybzjjzf,0.0) gwybzjjzf, isnull(a.qybcylbxjjzf,0.0) qybcylbxjjzf,
	                       isnull(a.mzjzfy,0.0) mzjzfy,isnull(a.syjjzf,0.0) syjjzf,isnull(a.jkfpjzzf,0.0) jkfpjzzf,isnull(a.xjzf,0.0) xjzf,
					       isnull(a.dbbxhgfygrzfqqlj,0.0) dbbxhgfygrzfqqlj,isnull(a.jrdbbxfdjey,0.0) jrdbbxfdjey, isnull(a.jrdbbxfdjee,0.0)jrdbbxfdjee,
					       isnull(a.jrdbbxfdjes,0.0)jrdbbxfdjes,
	                       isnull(a.jrdbbxfdzfy,0.0) jrdbbxfdzfy, isnull(a.jrdbbxfdzfe,0.0) jrdbbxfdzfe, isnull(a.jrdbbxfdzfs,0.0) jrdbbxfdzfs,
					       isnull(a.yyjzfy,0.0) yyjzfy, isnull(a.tdbzje,0.0) tdbzje, isnull(a.bcbxbxje,0.0) bcbxbxje, isnull(a.tcjjzf,0.0) tcjjzf,
					       isnull(a.dejjzf,0.0) dejjzf, isnull(a.dbbxzfje,0.0) dbbxzfje, isnull(a.zhzf,0.0) zhzf,
					       a.cxjfy,a.cfmxjylsh, isnull(bcjsqzhye,0.0) bcjsqzhye, isnull(bcjsqzhye-zhzf,0.0) bcjshzhye,convert(nvarchar(20),convert(decimal(12,2),isnull(jbylbxtcjjzfbl,0)) * 100) + '%' jbylbxtcjjzfbl,
					       isnull(a.dbjjzf,0.0) dbjjzf,
					       isnull( (select SUM(jjzfje) from ybfyjsmxdr aa where a.jslsh = aa.jslsh and aa.jjzflx in ('340100','340101')),0.0) lxryylbz,
					       isnull( (select SUM(jjzfje) from ybfyjsmxdr aa where a.jslsh = aa.jslsh and aa.jjzflx in ('310100','310101','310200','310201')),0.0) zgybbcbx,
					       isnull( (select SUM(jjzfje) from ybfyjsmxdr aa where a.jslsh = aa.jslsh and aa.jjzflx in ('510101')),0.0) syjj,
					       isnull( (select SUM(jjzfje) from ybfyjsmxdr aa where a.jslsh = aa.jslsh and aa.jjzflx in ('610105')),0.0) jkfp,
					       isnull( (select SUM(jjzfje) from ybfyjsmxdr aa where a.jslsh = aa.jslsh and aa.jjzflx in ('350100','350101')),0.0) cjjrylbzjj,b.dwmc hkdz
					       from ybfyjsdr a 
                        left join YBICKXX b on a.grbh=b.GRBH
                        left join mz01h on m1ghno=jzlsh 
                        left join bztbd c on a.yldylb=c.bzmem1 and c.bzcodn='dl'
                        left join bztbd d on a.yllb=d.bzkeyx and d.bzcodn='yl'
						left join bztbd e on a.xzlx=e.bzkeyx and e.bzcodn='insutype'
						left join bztbd f on a.yldylb=f.bzkeyx and f.bzcodn='psn_type'
						left join bztbd g on a.tcqh=g.bzkeyx and g.bzcodn='dq'
						left join ybfyjsmxdr h on a.jslsh = h.jslsh
                        where a.jzlsh='{0}' and a.cxbz=1 and djhin ='{1}'", jzlsh, djh);

                DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                if (ds.Tables[0].Rows.Count <= 0) { WriteLog("查不到该病人结算费用，打印失败"); return new object[] { 0, 1, "查不到该病人结算费用，打印失败", "提示" }; }

                string xm = ds.Tables[0].Rows[0]["XM"].ToString();
                string xb = ds.Tables[0].Rows[0]["xb"].ToString();
                string dwmc = ds.Tables[0].Rows[0]["dwmc"].ToString();
                string zyh = ds.Tables[0].Rows[0]["zyh"].ToString();
                string grbh = ds.Tables[0].Rows[0]["grbh"].ToString();
                string kh = ds.Tables[0].Rows[0]["kh"].ToString();
                string GMSFHM = ds.Tables[0].Rows[0]["GMSFHM"].ToString();
                string ybdylb = ds.Tables[0].Rows[0]["yldylb"].ToString();
                string yllb = ds.Tables[0].Rows[0]["yllb"].ToString();
                string yldylbnm = ds.Tables[0].Rows[0]["yldylbnm"].ToString();
                string fylb = ds.Tables[0].Rows[0]["fylb"].ToString();
                string bzmc = ds.Tables[0].Rows[0]["bzmc"].ToString();
                string ryrq = ds.Tables[0].Rows[0]["ryrq"].ToString();
                string cyrq = ds.Tables[0].Rows[0]["cyrq"].ToString();
                string cxjzssq = ds.Tables[0].Rows[0]["cxjzssq"].ToString();
                string ylfze = ds.Tables[0].Rows[0]["ylfze"].ToString();
                string zffy = ds.Tables[0].Rows[0]["zffy"].ToString();
                string ylzlfy = ds.Tables[0].Rows[0]["ylzlfy"].ToString();
                string nrbcbxfwndfy = ds.Tables[0].Rows[0]["nrbcbxfwndfy"].ToString();
                string bcbxbxqqlj = ds.Tables[0].Rows[0]["bcbxbxqqlj"].ToString();
                string zycs = ds.Tables[0].Rows[0]["zycs"].ToString();
                string qfbzfy = ds.Tables[0].Rows[0]["qfbzfy"].ToString();
                string qqlj = ds.Tables[0].Rows[0]["qqlj"].ToString();
                string tcfdjey = ds.Tables[0].Rows[0]["tcfdjey"].ToString();
                string tcfdjee = ds.Tables[0].Rows[0]["tcfdjee"].ToString();
                string tcfdjes = ds.Tables[0].Rows[0]["tcfdjes"].ToString();
                string tcfdjesi = ds.Tables[0].Rows[0]["tcfdjesi"].ToString();
                string tcfdjew = ds.Tables[0].Rows[0]["tcfdjew"].ToString();

                string tcfdzfy = ds.Tables[0].Rows[0]["tcfdzfy"].ToString();
                string tcfdzfe = ds.Tables[0].Rows[0]["tcfdzfe"].ToString();
                string tcfdzfs = ds.Tables[0].Rows[0]["tcfdzfs"].ToString();
                string tcfdzfsi = ds.Tables[0].Rows[0]["tcfdzfsi"].ToString();
                string tcfdzfw = ds.Tables[0].Rows[0]["tcfdzfw"].ToString();
                string tcfdzfbly = ds.Tables[0].Rows[0]["tcfdzfbly"].ToString();
                string tcfdzfble = ds.Tables[0].Rows[0]["tcfdzfble"].ToString();
                string tcfdzfbls = ds.Tables[0].Rows[0]["tcfdzfbls"].ToString();
                string tcfdzfblsi = ds.Tables[0].Rows[0]["tcfdzfblsi"].ToString();
                string tcfdzfblw = ds.Tables[0].Rows[0]["tcfdzfblw"].ToString();
                string defdzfy = ds.Tables[0].Rows[0]["defdzfy"].ToString();
                string defdzfe = ds.Tables[0].Rows[0]["defdzfe"].ToString();
                string defdzfs = ds.Tables[0].Rows[0]["defdzfs"].ToString();
                string defdzfbly = ds.Tables[0].Rows[0]["defdzfbly"].ToString();
                string defdzfble = ds.Tables[0].Rows[0]["defdzfble"].ToString();
                string defdzfbls = ds.Tables[0].Rows[0]["defdzfbls"].ToString();

                string defdjey = ds.Tables[0].Rows[0]["defdjey"].ToString();
                string defdjee = ds.Tables[0].Rows[0]["defdjee"].ToString();
                string defdjes = ds.Tables[0].Rows[0]["defdjes"].ToString();
                string gwybzjjzf = ds.Tables[0].Rows[0]["gwybzjjzf"].ToString();
                string qybcylbxjjzf = ds.Tables[0].Rows[0]["qybcylbxjjzf"].ToString();
                string mzjzfy = ds.Tables[0].Rows[0]["mzjzfy"].ToString();
                string syjjzf = ds.Tables[0].Rows[0]["syjjzf"].ToString();
                string jkfpjzzf = ds.Tables[0].Rows[0]["jkfpjzzf"].ToString();
                string xjzf = ds.Tables[0].Rows[0]["xjzf"].ToString();
                string dbbxhgfygrzfqqlj = ds.Tables[0].Rows[0]["dbbxhgfygrzfqqlj"].ToString();
                string jrdbbxfdjey = ds.Tables[0].Rows[0]["jrdbbxfdjey"].ToString();
                string jrdbbxfdjee = ds.Tables[0].Rows[0]["jrdbbxfdjee"].ToString();
                string jrdbbxfdjes = ds.Tables[0].Rows[0]["jrdbbxfdjes"].ToString();
                string jrdbbxfdzfy = ds.Tables[0].Rows[0]["jrdbbxfdzfy"].ToString();
                string jrdbbxfdzfe = ds.Tables[0].Rows[0]["jrdbbxfdzfe"].ToString();
                string jrdbbxfdzfs = ds.Tables[0].Rows[0]["jrdbbxfdzfs"].ToString();
                string yyjzfy = ds.Tables[0].Rows[0]["yyjzfy"].ToString();
                string tdbzje = ds.Tables[0].Rows[0]["tdbzje"].ToString();
                string bcbxbxje = ds.Tables[0].Rows[0]["bcbxbxje"].ToString();
                string tcjjzf = ds.Tables[0].Rows[0]["tcjjzf"].ToString();
                string dejjzf = ds.Tables[0].Rows[0]["dejjzf"].ToString();
                string dbbxzfje = ds.Tables[0].Rows[0]["dbbxzfje"].ToString();
                string zhzf = ds.Tables[0].Rows[0]["zhzf"].ToString();
                string yldylb = ds.Tables[0].Rows[0]["yldylb"].ToString();
                string jytxm = ds.Tables[0].Rows[0]["jytxm"].ToString();
                string cxjfy = ds.Tables[0].Rows[0]["cxjfy"].ToString();
                string cfmxjylsh = ds.Tables[0].Rows[0]["cfmxjylsh"].ToString();
                string tcqh = ds.Tables[0].Rows[0]["tcqh"].ToString();
                string bcjsqzhye = ds.Tables[0].Rows[0]["bcjsqzhye"].ToString();
                string bcjshzhye = ds.Tables[0].Rows[0]["bcjshzhye"].ToString();
                string zfybfww = (decimal.Parse(zffy) + decimal.Parse(ylzlfy)).ToString();
                string zfybfwn = (decimal.Parse(ylfze) - (decimal.Parse(zffy) + decimal.Parse(ylzlfy))).ToString();
                string zjlx = ds.Tables[0].Rows[0]["zjlx"].ToString();
                string xzlx = ds.Tables[0].Rows[0]["xzlx"].ToString();

                string jslsh = ds.Tables[0].Rows[0]["jslsh"].ToString();
                string nl = ds.Tables[0].Rows[0]["nl"].ToString();
                string fyjssj = ds.Tables[0].Rows[0]["fyjssj"].ToString();

                string jbylbxtcjjzfbl = ds.Tables[0].Rows[0]["jbylbxtcjjzfbl"].ToString();
                string dbjjzf = ds.Tables[0].Rows[0]["dbjjzf"].ToString();
                string lxryylbz = ds.Tables[0].Rows[0]["lxryylbz"].ToString();
                string syjj = ds.Tables[0].Rows[0]["syjj"].ToString();
                string jkfp = ds.Tables[0].Rows[0]["jkfp"].ToString();
                string cjjrylbzjj = ds.Tables[0].Rows[0]["cjjrylbzjj"].ToString();
                string zgybbcbx = ds.Tables[0].Rows[0]["zgybbcbx"].ToString();
                string hkdz = ds.Tables[0].Rows[0]["hkdz"].ToString();


                decimal.TryParse(ds.Tables[0].Rows[0]["xxzfje"].ToString(), out decimal xxzfje);
                decimal.TryParse(ds.Tables[0].Rows[0]["fhjbylfy"].ToString(), out decimal fhjbylfy);

                decimal.TryParse(ds.Tables[0].Rows[0]["grfdzje"].ToString(), out decimal grfdzje);

                decimal.TryParse(ds.Tables[0].Rows[0]["jjzfze"].ToString(), out decimal jjzfze);

                decimal.TryParse(ds.Tables[0].Rows[0]["grzhgjzfje"].ToString(), out decimal grzhgjzfje);

                decimal.TryParse(ds.Tables[0].Rows[0]["qtjjzf"].ToString(), out decimal qtjjzf);

                decimal.TryParse(ds.Tables[0].Rows[0]["yyfdfy"].ToString(), out decimal yyfdfy);


                //DateTime d1 = Convert.ToDateTime(ds.Tables[0].Rows[0]["ryrq"]);
                //DateTime d2 = Convert.ToDateTime(ds.Tables[0].Rows[0]["cyrq"]);

                //string ryrq1 = d1.ToString("yyyy-MM-dd");
                //string cyrq1 = d2.ToString("yyyy-MM-dd");
                //TimeSpan ts = d2.Date - d1.Date;
                //int zyts = ts.Days;

                string yblxmc = "";
                string yblx = "";

                if (!string.IsNullOrWhiteSpace(yldylb))
                {
                    string[] sV1 = { "1", "2", "3", "4", "9" };
                    if (sV1.Contains(yldylb.Substring(0, 1)))
                    {
                        yblx = "城镇职工"; //职工医保
                        yblxmc = "职工医保"; //职工医保
                    }
                    else
                    {
                        yblx = "城乡居民"; //居民医保
                        yblxmc = "居民医保"; //居民医保
                    }
                }

                string cm = string.Format("select bzname from bztbd where bzcodn ='CM'");
                ds = CliUtils.ExecuteSql("sybdj", "cmd", cm, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);

                string ddyymc = ds.Tables[0].Rows[0]["bzname"].ToString();

                string czy = CliUtils.fUserName;
                string czygh = CliUtils.fLoginUser;

                Report report = new Report();

                string strSQL = string.Format(@"select bzname,SUM(a.je) je  from ybcfmxscfhdr a
                            left join bztbd on bzcodn='SF' and bzkeyx=a.sflb
							left join ybcfmxscindr b on a.jylsh=b.jylsh and a.jzlsh=b.jzlsh and a.dqxmxh=b.dqxmxh and a.cxbz=b.cxbz
                            where a.jzlsh='{0}' and a.cxbz=1
                            group by bzname", jzlsh, cfmxjylsh);
                DataSet ds1 = CliUtils.ExecuteSql("sybdj", "cmd", strSQL, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);


                string s_path = string.Empty;
                s_path = string.IsNullOrEmpty(s_path) ? @"C:\Program Files (x86)\Infolight\EEP2012\EEPNetClient\FastReport" : s_path;
                string c_file = Application.StartupPath + @"\FastReport\YB\江西芦溪县中医医院基金结算清单.frx";
                string s_file = s_path + @"\YB\江西芦溪县中医医院基金结算清单.frx";
                CliUtils.DownLoad(s_file, c_file);   //下载远端AP SERVER报表文件  

                //检查报表文件是否存
                if (!File.Exists(c_file))
                {
                    MessageBox.Show(c_file + "不存在!");
                    WriteLog("医保提示：" + c_file + "不存在!");
                    return new object[] { 0, 0, "医保提示：" + c_file + "不存在!", "提示" };
                }

                // report.PrintSettings.ShowDialog = false;
                report.Load(c_file);
                report.RegisterData(ds1);
                WriteLog("12");
                report.SetParameterValue("czy", czy);
                report.SetParameterValue("czygh", czygh);
                report.SetParameterValue("czrq", DateTime.Now.ToString("yyyy-MM-dd"));
                report.SetParameterValue("xm", xm);
                report.SetParameterValue("xb", xb);
                report.SetParameterValue("dwmc", dwmc);
                report.SetParameterValue("zyh", zyh);
                report.SetParameterValue("grbh", grbh);
                report.SetParameterValue("kh", kh);
                report.SetParameterValue("GMSFHM", GMSFHM);
                report.SetParameterValue("yldylb", yldylbnm);
                report.SetParameterValue("fylb", fylb);
                report.SetParameterValue("bzmc", bzmc);
                report.SetParameterValue("ryrq", ryrq);
                report.SetParameterValue("cyrq", cyrq);
                report.SetParameterValue("cxjzssq", cxjzssq);
                report.SetParameterValue("ylfze", decimal.Parse(ylfze));
                report.SetParameterValue("zffy", decimal.Parse(zffy));
                report.SetParameterValue("ylzlfy", decimal.Parse(ylzlfy));
                report.SetParameterValue("bcbxbxqqlj", decimal.Parse(bcbxbxqqlj));
                report.SetParameterValue("zycs", zycs);
                report.SetParameterValue("qfbzfy", decimal.Parse(qfbzfy));
                report.SetParameterValue("qqlj", decimal.Parse(qqlj));
                report.SetParameterValue("tcfdjey", decimal.Parse(tcfdjey));
                report.SetParameterValue("tcfdjee", decimal.Parse(tcfdjee));
                report.SetParameterValue("tcfdjes", decimal.Parse(tcfdjes));
                report.SetParameterValue("tcfdjesi", decimal.Parse(tcfdjesi));
                report.SetParameterValue("tcfdjew", decimal.Parse(tcfdjew));
                report.SetParameterValue("defdjey", decimal.Parse(defdjey));
                report.SetParameterValue("defdjee", decimal.Parse(defdjee));
                report.SetParameterValue("defdjes", decimal.Parse(defdjes));
                report.SetParameterValue("gwybzjjzf", decimal.Parse(gwybzjjzf));
                report.SetParameterValue("qybcylbxjjzf", decimal.Parse(qybcylbxjjzf));
                report.SetParameterValue("mzjzfy", decimal.Parse(mzjzfy));
                report.SetParameterValue("syjjzf", decimal.Parse(syjjzf));
                report.SetParameterValue("jkfpjzzf", decimal.Parse(jkfpjzzf));
                report.SetParameterValue("xjzf", decimal.Parse(xjzf));
                report.SetParameterValue("dbbxhgfygrzfqqlj", decimal.Parse(dbbxhgfygrzfqqlj));
                report.SetParameterValue("jrdbbxfdjey", decimal.Parse(jrdbbxfdjey));
                report.SetParameterValue("jrdbbxfdjee", decimal.Parse(jrdbbxfdjee));
                report.SetParameterValue("jrdbbxfdjes", decimal.Parse(jrdbbxfdjes));
                report.SetParameterValue("jrdbbxfdzfy", decimal.Parse(jrdbbxfdzfy));
                report.SetParameterValue("jrdbbxfdzfe", decimal.Parse(jrdbbxfdzfe));
                report.SetParameterValue("jrdbbxfdzfs", decimal.Parse(jrdbbxfdzfs));
                report.SetParameterValue("yyjzfy", decimal.Parse(yyjzfy));
                report.SetParameterValue("tdbzje", decimal.Parse(tdbzje));
                report.SetParameterValue("bcbxbxje", decimal.Parse(bcbxbxje));
                report.SetParameterValue("tcjjzf", decimal.Parse(tcjjzf));
                report.SetParameterValue("dejjzf", decimal.Parse(dejjzf));
                report.SetParameterValue("dbbxzfje", decimal.Parse(dbbxzfje));
                report.SetParameterValue("zhzf", decimal.Parse(zhzf));
                report.SetParameterValue("jytxm", jytxm);
                report.SetParameterValue("yblx", yblx);
                //report.SetParameterValue("zyts", zyts);
                report.SetParameterValue("zyts", 0);
                report.SetParameterValue("mzzy", "（门诊类）");
                report.SetParameterValue("zfybfww", decimal.Parse(zfybfww));
                report.SetParameterValue("zfybfwn", decimal.Parse(zfybfwn));
                report.SetParameterValue("nrbcbxfwndfy", decimal.Parse(nrbcbxfwndfy));
                report.SetParameterValue("cxjfy", decimal.Parse(cxjfy));
                report.SetParameterValue("sjbx", decimal.Parse(ylfze) - decimal.Parse(xjzf));
                report.SetParameterValue("tcfdzfy", decimal.Parse(tcfdzfy));
                report.SetParameterValue("tcfdzfe", decimal.Parse(tcfdzfe));
                report.SetParameterValue("tcfdzfs", decimal.Parse(tcfdzfs));
                report.SetParameterValue("tcfdzfsi", decimal.Parse(tcfdzfsi));
                report.SetParameterValue("tcfdzfw", decimal.Parse(tcfdzfw));
                report.SetParameterValue("tcfdzfbly", decimal.Parse(tcfdzfbly));
                report.SetParameterValue("tcfdzfble", decimal.Parse(tcfdzfble));
                report.SetParameterValue("tcfdzfbls", decimal.Parse(tcfdzfbls));
                report.SetParameterValue("tcfdzfblsi", decimal.Parse(tcfdzfblsi));
                report.SetParameterValue("tcfdzfblw", decimal.Parse(tcfdzfblw));
                report.SetParameterValue("defdzfy", decimal.Parse(defdzfy));
                report.SetParameterValue("defdzfe", decimal.Parse(defdzfe));
                report.SetParameterValue("defdzfs", decimal.Parse(defdzfs));
                report.SetParameterValue("defdzfbly", decimal.Parse(defdzfbly));
                report.SetParameterValue("defdzfble", decimal.Parse(defdzfble));
                report.SetParameterValue("defdzfbls", decimal.Parse(defdzfbls));

                report.SetParameterValue("bcjsqzhye", decimal.Parse(bcjsqzhye));
                report.SetParameterValue("bcjshzhye", decimal.Parse(bcjshzhye));
                report.SetParameterValue("tcqh", tcqh);
                report.SetParameterValue("yblxmc", yblxmc);
                report.SetParameterValue("ddyymc", ddyymc);

                report.SetParameterValue("zjlx", zjlx);
                report.SetParameterValue("xzlx", xzlx);
                report.SetParameterValue("ddylgjbh", YBJGBH);
                report.SetParameterValue("ddylgjmc", YBJGMC);
                report.SetParameterValue("jslsh", jslsh);
                report.SetParameterValue("nl", nl);

                report.SetParameterValue("xxzfje", xxzfje);
                report.SetParameterValue("fhjbylfy", fhjbylfy);
                report.SetParameterValue("grfdzje", grfdzje);
                report.SetParameterValue("jjzfze", jjzfze);
                report.SetParameterValue("grzhgjzfje", grzhgjzfje);
                report.SetParameterValue("qtjjzf", qtjjzf);
                report.SetParameterValue("yyfdfy", yyfdfy);

                report.SetParameterValue("fyjssj", fyjssj);

                report.SetParameterValue("yldylb", yldylb);
                report.SetParameterValue("yllb", yllb);
                report.SetParameterValue("jbylbxtcjjzfbl", jbylbxtcjjzfbl);
                report.SetParameterValue("dbjjzf", dbjjzf);
                report.SetParameterValue("lxryylbz", lxryylbz);
                report.SetParameterValue("zgybbcbx", zgybbcbx);
                report.SetParameterValue("syjj", syjj);
                report.SetParameterValue("jkfp", jkfp);
                report.SetParameterValue("cjjrylbzjj", cjjrylbzjj);
                report.SetParameterValue("hkdz", hkdz);


                //report.Show();
                //report.Dispose();
                cmain.PrintWindow printwin = new cmain.PrintWindow(report);
                printwin.ShowDialog();

                WriteLog("医保提示：打印成功");
                return new object[] { 0, 1, "医保提示：打印成功", "提示" };

            }
            catch (Exception ex)
            {
                WriteLog("医保提示：打印发生异常" + ex.Message + "\r\n" + ex.StackTrace);
                return new object[] { 0, 0, "打印发生异常", "提示" };
            }

        }
        #endregion

        #region 住院结算清单
        public static object[] YBZYJSQD(object[] objParam)
        {
            WriteLog("进入住院结算清单打印.....");
            try
            {
                string jzlsh = objParam[0].ToString();
                string djh = objParam[1].ToString();
                string strSql = string.Format(@"select b.XM,case when b.xb=1 then '男' else '女' end xb, zjlx,a.jslsh jsid,
						b.dwmc,a.jzlsh zyh,b.grbh,b.kh,b.GMSFHM,a.jslsh,isnull(d.bzname,'') yllb,
						isnull(g.bzname,'') tcqh,
						isnull(e.bzname,'') xzlx,
						isnull(f.bzname,'') yldylb,b.nl,convert(varchar(20), a.sysdate,120) fyjssj,
                       c.bzname yldylbnm,d.bzname fylb,bzmc,left(z1date,10) ryrq,left(z1ldat,10) cyrq,a.cxjzssq,a.yldylb,a.jytxm,
                            isnull(a.xxzfje,0.0) xxzfje,isnull(a.fhjbylfy,0.0) fhjbylfy,isnull(a.grfdzje,0.0) grfdzje,
	                       isnull(a.jjzfze,0.0) jjzfze,isnull(a.tcjjzf,0.0) tcjjzf,isnull(a.gwybzjjzf,0.0) gwybzjjzf,
	                       isnull(a.grzhgjzfje,0.0) grzhgjzfje,
	                       isnull(a.qtjjzf,0.0) qtjjzf,
	                       isnull(a.yyfdfy,0.0) yyfdfy,
                            isnull(a.ylfze,0.0) ylfze,isnull(a.zffy,0.0) zffy,
					       isnull(a.ylzlfy,0.0) ylzlfy,isnull(a.nrbcbxfwndfy,0.0) nrbcbxfwndfy,
					       isnull(a.bcbxbxqqlj,0.0) bcbxbxqqlj,isnull(a.zycs,0.0) zycs,
					       isnull(a.qfbzfy,0.0) qfbzfy,isnull(a.qqlj,0.0) qqlj,
					       isnull(a.tcfdjey,0.0) tcfdjey,isnull(a.tcfdjee,0.0)tcfdjee,
	                       isnull(a.tcfdjes,0.0) tcfdjes, isnull(a.tcfdjesi,0.0) tcfdjesi,isnull(a.tcfdjew,0.0) tcfdjew,
					       isnull(a.tcfdzfy,0.0) tcfdzfy, isnull(a.tcfdzfe,0.0) tcfdzfe, isnull(a.tcfdzfs,0.0) tcfdzfs, isnull(a.tcfdzfsi,0.0) tcfdzfsi,
					       isnull(a.tcfdzfw,0.0) tcfdzfw, isnull(a.tcfdzfbly,0.0) tcfdzfbly, isnull(a.tcfdzfble,0.0) tcfdzfble, isnull(a.tcfdzfbls,0.0) tcfdzfbls,
					       isnull(a.tcfdzfblsi,0.0) tcfdzfblsi, isnull(a.tcfdzfblw,0.0) tcfdzfblw,
	                       isnull(a.defdzfy,0.0) defdzfy, isnull(a.defdzfe,0.0) defdzfe, isnull(a.defdzfs,0.0) defdzfs, isnull(a.defdzfbly,0.0) defdzfbly,
					       isnull(a.defdzfble,0.0) defdzfble, isnull(a.defdzfbls,0.0) defdzfbls, isnull(a.defdjey,0.0) defdjey, isnull(a.defdjee,0.0) defdjee,
					       isnull(a.defdjes,0.0) defdjes,isnull(a.gwybzjjzf,0.0) gwybzjjzf, isnull(a.qybcylbxjjzf,0.0) qybcylbxjjzf,
	                       isnull(a.mzjzfy,0.0) mzjzfy,isnull(a.syjjzf,0.0) syjjzf,isnull(a.jkfpjzzf,0.0) jkfpjzzf,isnull(a.xjzf,0.0) xjzf,
					       isnull(a.dbbxhgfygrzfqqlj,0.0) dbbxhgfygrzfqqlj,isnull(a.jrdbbxfdjey,0.0) jrdbbxfdjey, isnull(a.jrdbbxfdjee,0.0)jrdbbxfdjee,
					       isnull(a.jrdbbxfdjes,0.0)jrdbbxfdjes,
	                       isnull(a.jrdbbxfdzfy,0.0) jrdbbxfdzfy, isnull(a.jrdbbxfdzfe,0.0) jrdbbxfdzfe, isnull(a.jrdbbxfdzfs,0.0) jrdbbxfdzfs,
					       isnull(a.yyjzfy,0.0) yyjzfy, isnull(a.tdbzje,0.0) tdbzje, isnull(a.bcbxbxje,0.0) bcbxbxje, isnull(a.tcjjzf,0.0) tcjjzf,
					       isnull(a.dejjzf,0.0) dejjzf, isnull(a.dbbxzfje,0.0) dbbxzfje, isnull(a.zhzf,0.0) zhzf,
					       a.cxjfy,a.cfmxjylsh, isnull(bcjsqzhye,0.0) bcjsqzhye, isnull(bcjsqzhye-zhzf,0.0) bcjshzhye,convert(nvarchar(20),convert(decimal(12,2),isnull(jbylbxtcjjzfbl,0)) * 100) + '%' jbylbxtcjjzfbl,
					       isnull(a.dbjjzf,0.0) dbjjzf,
					       isnull( (select SUM(jjzfje) from ybfyjsmxdr aa where a.jslsh = aa.jslsh and aa.jjzflx in ('340100','340101')),0.0) lxryylbz,
					       isnull( (select SUM(jjzfje) from ybfyjsmxdr aa where a.jslsh = aa.jslsh and aa.jjzflx in ('310100','310101','310200','310201')),0.0) zgybbcbx,
					       isnull( (select SUM(jjzfje) from ybfyjsmxdr aa where a.jslsh = aa.jslsh and aa.jjzflx in ('510101')),0.0) syjj,
					       isnull( (select SUM(jjzfje) from ybfyjsmxdr aa where a.jslsh = aa.jslsh and aa.jjzflx in ('610105')),0.0) jkfp,
					       isnull( (select SUM(jjzfje) from ybfyjsmxdr aa where a.jslsh = aa.jslsh and aa.jjzflx in ('350100','350101')),0.0) cjjrylbzjj,b.dwmc hkdz
					       from ybfyjsdr a 
                        left join YBICKXX b on a.grbh=b.GRBH
                        left join zy01h on z1zyno=jzlsh 
                        left join bztbd c on a.yldylb=c.bzmem1 and c.bzcodn='dl'
                        left join bztbd d on a.yllb=d.bzkeyx and d.bzcodn='yl'
						left join bztbd e on a.xzlx=e.bzkeyx and e.bzcodn='insutype'
						left join bztbd f on a.yldylb=f.bzkeyx and f.bzcodn='psn_type'
						left join bztbd g on a.tcqh=g.bzkeyx and g.bzcodn='dq'
						left join ybfyjsmxdr h on a.jslsh = h.jslsh
                        where a.jzlsh='{0}' and a.cxbz=1 and djhin ='{1}'", jzlsh, djh);

                DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                if (ds.Tables[0].Rows.Count <= 0) { WriteLog("查不到该病人结算费用，打印失败"); return new object[] { 0, 1, "查不到该病人结算费用，打印失败", "提示" }; }

                string xm = ds.Tables[0].Rows[0]["XM"].ToString();
                string xb = ds.Tables[0].Rows[0]["xb"].ToString();
                string dwmc = ds.Tables[0].Rows[0]["dwmc"].ToString();
                string zyh = ds.Tables[0].Rows[0]["zyh"].ToString();
                string grbh = ds.Tables[0].Rows[0]["grbh"].ToString();
                string kh = ds.Tables[0].Rows[0]["kh"].ToString();
                string GMSFHM = ds.Tables[0].Rows[0]["GMSFHM"].ToString();
                string ybdylb = ds.Tables[0].Rows[0]["yldylb"].ToString();
                string yllb = ds.Tables[0].Rows[0]["yllb"].ToString();
                string yldylbnm = ds.Tables[0].Rows[0]["yldylbnm"].ToString();
                string fylb = ds.Tables[0].Rows[0]["fylb"].ToString();
                string bzmc = ds.Tables[0].Rows[0]["bzmc"].ToString();
                string ryrq = ds.Tables[0].Rows[0]["ryrq"].ToString();
                string cyrq = ds.Tables[0].Rows[0]["cyrq"].ToString();
                string cxjzssq = ds.Tables[0].Rows[0]["cxjzssq"].ToString();
                string ylfze = ds.Tables[0].Rows[0]["ylfze"].ToString();
                string zffy = ds.Tables[0].Rows[0]["zffy"].ToString();
                string ylzlfy = ds.Tables[0].Rows[0]["ylzlfy"].ToString();
                string nrbcbxfwndfy = ds.Tables[0].Rows[0]["nrbcbxfwndfy"].ToString();
                string bcbxbxqqlj = ds.Tables[0].Rows[0]["bcbxbxqqlj"].ToString();
                string zycs = ds.Tables[0].Rows[0]["zycs"].ToString();
                string qfbzfy = ds.Tables[0].Rows[0]["qfbzfy"].ToString();
                string qqlj = ds.Tables[0].Rows[0]["qqlj"].ToString();
                string tcfdjey = ds.Tables[0].Rows[0]["tcfdjey"].ToString();
                string tcfdjee = ds.Tables[0].Rows[0]["tcfdjee"].ToString();
                string tcfdjes = ds.Tables[0].Rows[0]["tcfdjes"].ToString();
                string tcfdjesi = ds.Tables[0].Rows[0]["tcfdjesi"].ToString();
                string tcfdjew = ds.Tables[0].Rows[0]["tcfdjew"].ToString();

                string tcfdzfy = ds.Tables[0].Rows[0]["tcfdzfy"].ToString();
                string tcfdzfe = ds.Tables[0].Rows[0]["tcfdzfe"].ToString();
                string tcfdzfs = ds.Tables[0].Rows[0]["tcfdzfs"].ToString();
                string tcfdzfsi = ds.Tables[0].Rows[0]["tcfdzfsi"].ToString();
                string tcfdzfw = ds.Tables[0].Rows[0]["tcfdzfw"].ToString();
                string tcfdzfbly = ds.Tables[0].Rows[0]["tcfdzfbly"].ToString();
                string tcfdzfble = ds.Tables[0].Rows[0]["tcfdzfble"].ToString();
                string tcfdzfbls = ds.Tables[0].Rows[0]["tcfdzfbls"].ToString();
                string tcfdzfblsi = ds.Tables[0].Rows[0]["tcfdzfblsi"].ToString();
                string tcfdzfblw = ds.Tables[0].Rows[0]["tcfdzfblw"].ToString();
                string defdzfy = ds.Tables[0].Rows[0]["defdzfy"].ToString();
                string defdzfe = ds.Tables[0].Rows[0]["defdzfe"].ToString();
                string defdzfs = ds.Tables[0].Rows[0]["defdzfs"].ToString();
                string defdzfbly = ds.Tables[0].Rows[0]["defdzfbly"].ToString();
                string defdzfble = ds.Tables[0].Rows[0]["defdzfble"].ToString();
                string defdzfbls = ds.Tables[0].Rows[0]["defdzfbls"].ToString();

                string defdjey = ds.Tables[0].Rows[0]["defdjey"].ToString();
                string defdjee = ds.Tables[0].Rows[0]["defdjee"].ToString();
                string defdjes = ds.Tables[0].Rows[0]["defdjes"].ToString();
                string gwybzjjzf = ds.Tables[0].Rows[0]["gwybzjjzf"].ToString();
                string qybcylbxjjzf = ds.Tables[0].Rows[0]["qybcylbxjjzf"].ToString();
                string mzjzfy = ds.Tables[0].Rows[0]["mzjzfy"].ToString();
                string syjjzf = ds.Tables[0].Rows[0]["syjjzf"].ToString();
                string jkfpjzzf = ds.Tables[0].Rows[0]["jkfpjzzf"].ToString();
                string xjzf = ds.Tables[0].Rows[0]["xjzf"].ToString();
                string dbbxhgfygrzfqqlj = ds.Tables[0].Rows[0]["dbbxhgfygrzfqqlj"].ToString();
                string jrdbbxfdjey = ds.Tables[0].Rows[0]["jrdbbxfdjey"].ToString();
                string jrdbbxfdjee = ds.Tables[0].Rows[0]["jrdbbxfdjee"].ToString();
                string jrdbbxfdjes = ds.Tables[0].Rows[0]["jrdbbxfdjes"].ToString();
                string jrdbbxfdzfy = ds.Tables[0].Rows[0]["jrdbbxfdzfy"].ToString();
                string jrdbbxfdzfe = ds.Tables[0].Rows[0]["jrdbbxfdzfe"].ToString();
                string jrdbbxfdzfs = ds.Tables[0].Rows[0]["jrdbbxfdzfs"].ToString();
                string yyjzfy = ds.Tables[0].Rows[0]["yyjzfy"].ToString();
                string tdbzje = ds.Tables[0].Rows[0]["tdbzje"].ToString();
                string bcbxbxje = ds.Tables[0].Rows[0]["bcbxbxje"].ToString();
                string tcjjzf = ds.Tables[0].Rows[0]["tcjjzf"].ToString();
                string dejjzf = ds.Tables[0].Rows[0]["dejjzf"].ToString();
                string dbbxzfje = ds.Tables[0].Rows[0]["dbbxzfje"].ToString();
                string zhzf = ds.Tables[0].Rows[0]["zhzf"].ToString();
                string yldylb = ds.Tables[0].Rows[0]["yldylb"].ToString();
                string jytxm = ds.Tables[0].Rows[0]["jytxm"].ToString();
                string cxjfy = ds.Tables[0].Rows[0]["cxjfy"].ToString();
                string cfmxjylsh = ds.Tables[0].Rows[0]["cfmxjylsh"].ToString();
                string tcqh = ds.Tables[0].Rows[0]["tcqh"].ToString();
                string bcjsqzhye = ds.Tables[0].Rows[0]["bcjsqzhye"].ToString();
                string bcjshzhye = ds.Tables[0].Rows[0]["bcjshzhye"].ToString();
                string zfybfww = (decimal.Parse(zffy) + decimal.Parse(ylzlfy)).ToString();
                string zfybfwn = (decimal.Parse(ylfze) - (decimal.Parse(zffy) + decimal.Parse(ylzlfy))).ToString();
                string zjlx = ds.Tables[0].Rows[0]["zjlx"].ToString();
                string xzlx = ds.Tables[0].Rows[0]["xzlx"].ToString();

                string jslsh = ds.Tables[0].Rows[0]["jslsh"].ToString();
                string nl = ds.Tables[0].Rows[0]["nl"].ToString();
                string fyjssj = ds.Tables[0].Rows[0]["fyjssj"].ToString();

                string jbylbxtcjjzfbl = ds.Tables[0].Rows[0]["jbylbxtcjjzfbl"].ToString();
                string dbjjzf = ds.Tables[0].Rows[0]["dbjjzf"].ToString();
                string lxryylbz = ds.Tables[0].Rows[0]["lxryylbz"].ToString();
                string syjj = ds.Tables[0].Rows[0]["syjj"].ToString();
                string jkfp = ds.Tables[0].Rows[0]["jkfp"].ToString();
                string cjjrylbzjj = ds.Tables[0].Rows[0]["cjjrylbzjj"].ToString();
                string zgybbcbx = ds.Tables[0].Rows[0]["zgybbcbx"].ToString();
                string hkdz = ds.Tables[0].Rows[0]["hkdz"].ToString();


                decimal.TryParse(ds.Tables[0].Rows[0]["xxzfje"].ToString(), out decimal xxzfje);
                decimal.TryParse(ds.Tables[0].Rows[0]["fhjbylfy"].ToString(), out decimal fhjbylfy);

                decimal.TryParse(ds.Tables[0].Rows[0]["grfdzje"].ToString(), out decimal grfdzje);

                decimal.TryParse(ds.Tables[0].Rows[0]["jjzfze"].ToString(), out decimal jjzfze);

                decimal.TryParse(ds.Tables[0].Rows[0]["grzhgjzfje"].ToString(), out decimal grzhgjzfje);

                decimal.TryParse(ds.Tables[0].Rows[0]["qtjjzf"].ToString(), out decimal qtjjzf);

                decimal.TryParse(ds.Tables[0].Rows[0]["yyfdfy"].ToString(), out decimal yyfdfy);


                DateTime d1 = Convert.ToDateTime(ds.Tables[0].Rows[0]["ryrq"]);
                DateTime d2 = Convert.ToDateTime(ds.Tables[0].Rows[0]["cyrq"]);

                string ryrq1 = d1.ToString("yyyy-MM-dd");
                string cyrq1 = d2.ToString("yyyy-MM-dd");
                TimeSpan ts = d2.Date - d1.Date;
                int zyts = ts.Days;
                string yblxmc = "";
                string yblx = "";

                if (!string.IsNullOrWhiteSpace(yldylb))
                {
                    string[] sV1 = { "1", "2", "3", "4", "9" };
                    if (sV1.Contains(yldylb.Substring(0, 1)))
                    {
                        yblx = "城镇职工"; //职工医保
                        yblxmc = "职工医保"; //职工医保
                    }
                    else
                    {
                        yblx = "城乡居民"; //居民医保
                        yblxmc = "居民医保"; //居民医保
                    }
                }

                string cm = string.Format("select bzname from bztbd where bzcodn ='CM'");
                ds = CliUtils.ExecuteSql("sybdj", "cmd", cm, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);

                string ddyymc = ds.Tables[0].Rows[0]["bzname"].ToString();

                string czy = CliUtils.fUserName;
                string czygh = CliUtils.fLoginUser;

                Report report = new Report();

                string strSQL = string.Format(@"select bzname,SUM(a.je) je  from ybcfmxscfhdr a
                            left join bztbd on bzcodn='SF' and bzkeyx=a.sflb
							left join ybcfmxscindr b on a.jylsh=b.jylsh and a.jzlsh=b.jzlsh and a.dqxmxh=b.dqxmxh and a.cxbz=b.cxbz
                            where a.jzlsh='{0}' and a.cxbz=1
                            group by bzname", jzlsh, cfmxjylsh);
                DataSet ds1 = CliUtils.ExecuteSql("sybdj", "cmd", strSQL, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);


                string s_path = string.Empty;
                s_path = string.IsNullOrEmpty(s_path) ? @"C:\Program Files (x86)\Infolight\EEP2012\EEPNetClient\FastReport" : s_path;
                string c_file = Application.StartupPath + @"\FastReport\YB\江西芦溪县中医医院基金结算清单.frx";
                string s_file = s_path + @"\YB\江西芦溪县中医医院基金结算清单.frx";
                CliUtils.DownLoad(s_file, c_file);   //下载远端AP SERVER报表文件  

                //检查报表文件是否存
                if (!File.Exists(c_file))
                {
                    MessageBox.Show(c_file + "不存在!");
                    WriteLog("医保提示：" + c_file + "不存在!");
                    return new object[] { 0, 0, "医保提示：" + c_file + "不存在!", "提示" };
                }

                // report.PrintSettings.ShowDialog = false;
                report.Load(c_file);
                report.RegisterData(ds1);
                WriteLog("12");
                report.SetParameterValue("czy", czy);
                report.SetParameterValue("czygh", czygh);
                report.SetParameterValue("czrq", DateTime.Now.ToString("yyyy-MM-dd"));
                report.SetParameterValue("xm", xm);
                report.SetParameterValue("xb", xb);
                report.SetParameterValue("dwmc", dwmc);
                report.SetParameterValue("zyh", zyh);
                report.SetParameterValue("grbh", grbh);
                report.SetParameterValue("kh", kh);
                report.SetParameterValue("GMSFHM", GMSFHM);
                report.SetParameterValue("yldylb", yldylbnm);
                report.SetParameterValue("fylb", fylb);
                report.SetParameterValue("bzmc", bzmc);
                report.SetParameterValue("ryrq", ryrq);
                report.SetParameterValue("cyrq", cyrq);
                report.SetParameterValue("cxjzssq", cxjzssq);
                report.SetParameterValue("ylfze", decimal.Parse(ylfze));
                report.SetParameterValue("zffy", decimal.Parse(zffy));
                report.SetParameterValue("ylzlfy", decimal.Parse(ylzlfy));
                report.SetParameterValue("bcbxbxqqlj", decimal.Parse(bcbxbxqqlj));
                report.SetParameterValue("zycs", zycs);
                report.SetParameterValue("qfbzfy", decimal.Parse(qfbzfy));
                report.SetParameterValue("qqlj", decimal.Parse(qqlj));
                report.SetParameterValue("tcfdjey", decimal.Parse(tcfdjey));
                report.SetParameterValue("tcfdjee", decimal.Parse(tcfdjee));
                report.SetParameterValue("tcfdjes", decimal.Parse(tcfdjes));
                report.SetParameterValue("tcfdjesi", decimal.Parse(tcfdjesi));
                report.SetParameterValue("tcfdjew", decimal.Parse(tcfdjew));
                report.SetParameterValue("defdjey", decimal.Parse(defdjey));
                report.SetParameterValue("defdjee", decimal.Parse(defdjee));
                report.SetParameterValue("defdjes", decimal.Parse(defdjes));
                report.SetParameterValue("gwybzjjzf", decimal.Parse(gwybzjjzf));
                report.SetParameterValue("qybcylbxjjzf", decimal.Parse(qybcylbxjjzf));
                report.SetParameterValue("mzjzfy", decimal.Parse(mzjzfy));
                report.SetParameterValue("syjjzf", decimal.Parse(syjjzf));
                report.SetParameterValue("jkfpjzzf", decimal.Parse(jkfpjzzf));
                report.SetParameterValue("xjzf", decimal.Parse(xjzf));
                report.SetParameterValue("dbbxhgfygrzfqqlj", decimal.Parse(dbbxhgfygrzfqqlj));
                report.SetParameterValue("jrdbbxfdjey", decimal.Parse(jrdbbxfdjey));
                report.SetParameterValue("jrdbbxfdjee", decimal.Parse(jrdbbxfdjee));
                report.SetParameterValue("jrdbbxfdjes", decimal.Parse(jrdbbxfdjes));
                report.SetParameterValue("jrdbbxfdzfy", decimal.Parse(jrdbbxfdzfy));
                report.SetParameterValue("jrdbbxfdzfe", decimal.Parse(jrdbbxfdzfe));
                report.SetParameterValue("jrdbbxfdzfs", decimal.Parse(jrdbbxfdzfs));
                report.SetParameterValue("yyjzfy", decimal.Parse(yyjzfy));
                report.SetParameterValue("tdbzje", decimal.Parse(tdbzje));
                report.SetParameterValue("bcbxbxje", decimal.Parse(bcbxbxje));
                report.SetParameterValue("tcjjzf", decimal.Parse(tcjjzf));
                report.SetParameterValue("dejjzf", decimal.Parse(dejjzf));
                report.SetParameterValue("dbbxzfje", decimal.Parse(dbbxzfje));
                report.SetParameterValue("zhzf", decimal.Parse(zhzf));
                report.SetParameterValue("jytxm", jytxm);
                report.SetParameterValue("yblx", yblx);
                report.SetParameterValue("zyts", zyts);
                report.SetParameterValue("mzzy", "（住院类）");
                report.SetParameterValue("zfybfww", decimal.Parse(zfybfww));
                report.SetParameterValue("zfybfwn", decimal.Parse(zfybfwn));
                report.SetParameterValue("nrbcbxfwndfy", decimal.Parse(nrbcbxfwndfy));
                report.SetParameterValue("cxjfy", decimal.Parse(cxjfy));
                report.SetParameterValue("sjbx", decimal.Parse(ylfze) - decimal.Parse(xjzf));
                report.SetParameterValue("tcfdzfy", decimal.Parse(tcfdzfy));
                report.SetParameterValue("tcfdzfe", decimal.Parse(tcfdzfe));
                report.SetParameterValue("tcfdzfs", decimal.Parse(tcfdzfs));
                report.SetParameterValue("tcfdzfsi", decimal.Parse(tcfdzfsi));
                report.SetParameterValue("tcfdzfw", decimal.Parse(tcfdzfw));
                report.SetParameterValue("tcfdzfbly", decimal.Parse(tcfdzfbly));
                report.SetParameterValue("tcfdzfble", decimal.Parse(tcfdzfble));
                report.SetParameterValue("tcfdzfbls", decimal.Parse(tcfdzfbls));
                report.SetParameterValue("tcfdzfblsi", decimal.Parse(tcfdzfblsi));
                report.SetParameterValue("tcfdzfblw", decimal.Parse(tcfdzfblw));
                report.SetParameterValue("defdzfy", decimal.Parse(defdzfy));
                report.SetParameterValue("defdzfe", decimal.Parse(defdzfe));
                report.SetParameterValue("defdzfs", decimal.Parse(defdzfs));
                report.SetParameterValue("defdzfbly", decimal.Parse(defdzfbly));
                report.SetParameterValue("defdzfble", decimal.Parse(defdzfble));
                report.SetParameterValue("defdzfbls", decimal.Parse(defdzfbls));

                report.SetParameterValue("bcjsqzhye", decimal.Parse(bcjsqzhye));
                report.SetParameterValue("bcjshzhye", decimal.Parse(bcjshzhye));
                report.SetParameterValue("tcqh", tcqh);
                report.SetParameterValue("yblxmc", yblxmc);
                report.SetParameterValue("ddyymc", ddyymc);

                report.SetParameterValue("zjlx", zjlx);
                report.SetParameterValue("xzlx", xzlx);
                report.SetParameterValue("ddylgjbh", YBJGBH);
                report.SetParameterValue("ddylgjmc", YBJGMC);
                report.SetParameterValue("jslsh", jslsh);
                report.SetParameterValue("nl", nl);

                report.SetParameterValue("xxzfje", xxzfje);
                report.SetParameterValue("fhjbylfy", fhjbylfy);
                report.SetParameterValue("grfdzje", grfdzje);
                report.SetParameterValue("jjzfze", jjzfze);
                report.SetParameterValue("grzhgjzfje", grzhgjzfje);
                report.SetParameterValue("qtjjzf", qtjjzf);
                report.SetParameterValue("yyfdfy", yyfdfy);

                report.SetParameterValue("fyjssj", fyjssj);

                report.SetParameterValue("yldylb", yldylb);
                report.SetParameterValue("yllb", yllb);
                report.SetParameterValue("jbylbxtcjjzfbl", jbylbxtcjjzfbl);
                report.SetParameterValue("dbjjzf", dbjjzf);
                report.SetParameterValue("lxryylbz", lxryylbz);
                report.SetParameterValue("zgybbcbx", zgybbcbx);
                report.SetParameterValue("syjj", syjj);
                report.SetParameterValue("jkfp", jkfp);
                report.SetParameterValue("cjjrylbzjj", cjjrylbzjj);
                report.SetParameterValue("hkdz", hkdz);


                //report.Show();
                //report.Dispose();
                cmain.PrintWindow printwin = new cmain.PrintWindow(report);
                printwin.ShowDialog();

                WriteLog("医保提示：打印成功");
                return new object[] { 0, 1, "医保提示：打印成功", "提示" };

            }
            catch (Exception ex)
            {
                WriteLog("医保提示：打印发生异常" + ex.Message + "\r\n" + ex.StackTrace);
                return new object[] { 0, 0, "打印发生异常", "提示" };
            }
        }
        #endregion
        #endregion

        #endregion

        #region 住院收费结算信息查询
        public static object[] YBZYJSXXCX(object[] objParam)
        {
            string jzlsh = objParam[0].ToString();
            try
            {
                Frm_ybjsmxSQXN jsmx = new Frm_ybjsmxSQXN(jzlsh);
                jsmx.ShowDialog();
                return new object[] { 0, 1, "查询成功" };
            }
            catch
            {
                return new object[] { 0, 0, "查询失败" };
            }
        }
        #endregion

        #region 出院办理
        public static object[] YBCYBL(object[] objParam)
        {
            string sysdate = GetServerDateTime();
            WriteLog("住院出院登记|入参|" + string.Join(",", objParam));
            try
            {
                CZYBH = CliUtils.fLoginUser;    //操作员工号   
                string jbr = CliUtils.fUserName;   // 经办人姓名 
                string jzlsh = objParam[0].ToString(); //就诊流水号
                string cyyy = objParam[1].ToString(); //出院原因
                string cysj = objParam[2].ToString(); //出院时间
                string ybjzlsh = "";//就诊ID
                string grbh = "";   //人员编号
                string xzlx = "";   //险种类型
                string jssj = Convert.ToDateTime(cysj).ToString("yyyy-MM-dd HH:mm:ss");   //结束时间
                string bzbm = ""; //病种编码
                string bzmc = "";   //病种名称
                string ssczdm = "";//手术操作代码
                string ssczmc = "";//手术操作名称
                string syfwzh = "";//计划生育服务证号
                string sylb = "";   //生育类别
                string sysslb = "";//计划生育手术类别
                string wybz = "";// 晚育标志
                string yzs = ""; //孕周数
                string tc = ""; //胎次
                string tes = ""; //胎儿数
                string zcbz = ""; //早产标志
                string jhsyrq = "";// 计划生育手术或生育日期
                string bfzbz = "0"; //伴有并发症标志
                string cyksdm = "";// 出院科室编码
                string cyksmc = ""; //出院科室名称
                string cycw = ""; //出院床位
                string lyfs = cyyy;// 离院方式
                string swrq = ""; //死亡日期
                string tcqh = "";//统筹区号

                #region 判断是否已登记
                string strSql = string.Format(@"select * from ybmzzydjdr where jzlsh='{0}' and cxbz=1", jzlsh);
                DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                if (ds.Tables[0].Rows.Count == 0)
                {
                    WriteLog(sysdate + "  就诊流水号:" + jzlsh + "未进行住院登记");
                    return new object[] { 0, 0, "  就诊流水号:" + jzlsh + "未进行住院登记" };
                }
                ybjzlsh = ds.Tables[0].Rows[0]["ybjzlsh"].ToString();
                grbh = ds.Tables[0].Rows[0]["grbh"].ToString();
                bzbm = ds.Tables[0].Rows[0]["bzbm"].ToString();
                bzmc = ds.Tables[0].Rows[0]["bzmc"].ToString();
                ssczdm = ds.Tables[0].Rows[0]["ssczdm"].ToString();
                ssczmc = ds.Tables[0].Rows[0]["ssczmc"].ToString();
                syfwzh = ds.Tables[0].Rows[0]["syfwzh"].ToString();
                sylb = ds.Tables[0].Rows[0]["syfylb"].ToString();
                sysslb = ds.Tables[0].Rows[0]["sysslb"].ToString();
                wybz = ds.Tables[0].Rows[0]["wybz"].ToString();
                yzs = ds.Tables[0].Rows[0]["syyzs"].ToString();
                tc = ds.Tables[0].Rows[0]["sytc"].ToString();
                tes = ds.Tables[0].Rows[0]["tes"].ToString();
                zcbz = ds.Tables[0].Rows[0]["zcbz"].ToString();
                jhsyrq = ds.Tables[0].Rows[0]["jhsyrq"].ToString();
                cyksdm = ds.Tables[0].Rows[0]["ksbh"].ToString();
                cyksmc = ds.Tables[0].Rows[0]["ksmc"].ToString();
                cycw = ds.Tables[0].Rows[0]["cwh"].ToString();
                tcqh = ds.Tables[0].Rows[0]["tcqh"].ToString();
                #endregion

                #region 判断读卡表数据
                strSql = string.Format(@"select * from ybickxx where grbh='{0}'", grbh);
                ds.Tables.Clear();
                ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    xzlx = ds.Tables[0].Rows[0]["xzlx"].ToString();
                }
                #endregion

                string jysj = Convert.ToDateTime(jssj).ToString("yyyyMMddHHmmss");
                //交易流水号 


                #region 入参
                string cydjJson = string.Empty;
                /*
                出院信息（节点标识：dscginfo）
                    就诊ID
                    人员编号
                    险种类型
                    结束时间
                    病种编码
                    病种名称
                    手术操作代码
                    手术操作名称
                    计划生育服务证号
                    生育类别
                    计划生育手术类别
                    晚育标志
                    孕周数
                    胎次
                    胎儿数
                    早产标志
                    计划生育手术或生育日期
                    伴有并发症标志
                    出院科室编码
                    出院科室名称
                    出院床位
                    离院方式
                    死亡日期

                 出院诊断信息（节点标识：diseinfo）
                    就诊ID
                    人员编号
                    诊断类别
                    主诊断标志
                    诊断排序号
                    诊断代码
                    诊断名称
                    诊断科室
                    诊断医生编码
                    诊断医生姓名
                    诊断时间


                */
                List<dynamic> cydj_Diseinfos = new List<dynamic>();
                //dscginfo
                dynamic dscginfo = new
                {
                    mdtrt_id = ybjzlsh,
                    psn_no = grbh,
                    insutype = xzlx,
                    endtime = jssj,
                    dise_codg = bzbm,
                    dise_name = bzmc,
                    oprn_oprt_code = ssczdm,
                    oprn_oprt_name = ssczmc,
                    fpsc_no = syfwzh,
                    matn_type = sylb,
                    birctrl_type = sysslb,
                    latechb_flag = wybz,
                    geso_val = yzs,
                    fetts = tc,
                    fetus_cnt = tes,
                    pret_flag = zcbz,
                    birctrl_matn_date = jhsyrq,
                    cop_flag = bfzbz,
                    dscg_dept_codg = cyksdm,
                    dscg_dept_name = cyksmc,
                    dscg_bed = cycw,
                    dscg_way = lyfs,
                    die_date = swrq
                };
                //diseinfo

                #region 获取所有诊断DataSet
                strSql = string.Format(@"with tmp as(
                select 
                m1mzno,
                case when m1xybz='Y' then '1' else '2' end zdlb,
                --case when m1xybz='Y' then '1' else '0' end zdbz,
                0 zdbz,
                m1xyzd,
                m1xynm,
                '' rybq,
                isnull(ybksmc,b2ejnm) ksmc,
                isnull(dgysbm,m1user) ysdm,
                isnull(ysxm,b1name) ysxm,
                m1date
                 from mza1dd 
                 left join bz01h on m1user=b1empn
                 left join bz02d on b2ejks=b1ksno
                 left join ybkszd on b1ksno=ksdm
                 left join ybdgyszd on ysbm=m1user
                 where isnull(m1xyzd,'')<>'' and isnull(m1zdks,'')='Y'
                 union all
                select 
                m1mzno,
                case when m1zybz='Y' then '3' else '4' end zdlb,
                --case when m1zybz='Y' then '1' else '0' end zdbz,
                0 zdbz,
                m1zyzd,
                m1zynm,
                '' rybq,
                isnull(ybksmc,b2ejnm) ksmc,
                isnull(dgysbm,m1user) ysdm,
                isnull(ysxm,b1name) ysxm,
                m1date
                 from mza1dd 
                 left join bz01h on m1user=b1empn
                 left join bz02d on b2ejks=b1ksno
                 left join ybkszd on b1ksno=ksdm
                 left join ybdgyszd on ysbm=m1user
                 where isnull(m1zyzd,'')<>''

                union all
				 select 
                z1zyno,
                '1' zdlb,
                1 zdbz,
                '{0}',
                '{1}',
                '' rybq,
                isnull(ybksmc,b2ejnm) ksmc,
                isnull(dgysbm,z1empn) ysdm,
                isnull(ysxm,b1name) ysxm,
                z1date
                 from zy01h 
                 left join bz01h on z1empn=b1empn
                 left join bz02d on b2ejks=b1ksno
                 left join ybkszd on b1ksno=ksdm
                 left join ybdgyszd on ysbm=z1empn
				 where z1zyno='{2}'
                 )select * from tmp where m1mzno='{2}'", bzbm, bzmc, jzlsh);
                ds.Tables.Clear();
                ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);

                #endregion

                for (int index = 0; index < ds.Tables[0].Rows.Count; index++)
                {
                    string diag_type = ds.Tables[0].Rows[index]["zdlb"].ToString();
                    string maindiag_flag = ds.Tables[0].Rows[index]["zdbz"].ToString();
                    string diag_srt_no = (index + 1).ToString();
                    string diag_code = ds.Tables[0].Rows[index]["m1xyzd"].ToString();
                    string diag_name = ds.Tables[0].Rows[index]["m1xynm"].ToString();
                    string adm_cond = ds.Tables[0].Rows[index]["rybq"].ToString();
                    string diag_dept = ds.Tables[0].Rows[index]["ksmc"].ToString();
                    string dise_dor_no = ds.Tables[0].Rows[index]["ysdm"].ToString();
                    string dise_dor_name = ds.Tables[0].Rows[index]["ysxm"].ToString();
                    string diag_time = ds.Tables[0].Rows[index]["m1date"].ToString();
                    dynamic diseinfo = new
                    {
                        mdtrt_id = ybjzlsh,
                        psn_no = grbh,
                        diag_type = diag_type,
                        maindiag_flag = maindiag_flag,
                        diag_srt_no = diag_srt_no,
                        diag_code = diag_code,
                        diag_name = diag_name,
                        diag_dept = diag_dept,
                        dise_dor_no = dise_dor_no,
                        dise_dor_name = dise_dor_name,
                        diag_time = diag_time
                    };
                    cydj_Diseinfos.Add(diseinfo);
                }
                string Err = string.Empty;
                dynamic input = new
                {
                    dscginfo = dscginfo,
                    diseinfo = cydj_Diseinfos
                };
                cydjJson = JsonConvert.SerializeObject(input);
                #endregion

                #region 医保出院登记
                List<string> liSQL = new List<string>();
                WriteLog(sysdate + "  出院登记|入参Json|" + cydjJson);
                int i = YBServiceRequest("2402", input, ref Err);

                if (i != 1)
                {
                    WriteLog(sysdate + "  出院登记失败|" + Err.ToString());
                    return new object[] { 0, 0, Err.ToString() };
                }
                string dataRs = Err.ToString();
                WriteLog(sysdate + "  出院登记|出参Json|" + dataRs);
                JObject jobj = JsonConvert.DeserializeObject(dataRs) as JObject; //获取响应数据json

                WriteLog(sysdate + "出院登记成功|");
                return new object[] { 0, 1, "出院登记成功|" };
                #endregion

            }
            catch (Exception ex)
            {
                WriteLog(sysdate + "  出院登记异常|" + ex.Message);
                return new object[] { 0, 0, "  出院登记异常|" + ex.Message };
            }


        }

        #endregion

        #region 出院撤销
        public static object[] YBCYCX(object[] objParam)
        {
            string sysdate = GetServerDateTime();
            WriteLog(sysdate + "  进入出院登记撤销...");
            WriteLog("出院撤销|入参|" + string.Join(",", objParam));
            try
            {
                CZYBH = CliUtils.fLoginUser;    //操作员工号   
                string jbr = CliUtils.fUserName;   // 经办人姓名 
                string ybjzlsh = objParam[0].ToString();
                string grbh = objParam[1].ToString();
                string strSql = string.Format(@"select * from ybmzzydjdr where ybjzlsh='{0}' and cxbz=1", ybjzlsh);
                DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                if (ds.Tables[0].Rows.Count == 0)
                {
                    WriteLog(sysdate + "  医保流水号:" + ybjzlsh + "未进行住院登记");
                    return new object[] { 0, 0, "  医保流水号:" + ybjzlsh + "未进行住院登记" };
                }
                string tcqh = ds.Tables[0].Rows[0]["tcqh"].ToString();

                string jysj = Convert.ToDateTime(sysdate).ToString("yyyyMMddHHmmss");
                //交易流水号 
                #region 入参
                string cydjcxJson = string.Empty;
                /*
                 * 输入（节点标识：data）
                 就诊ID
                 人员编号
                 */
                dynamic input = new
                {
                    data = new
                    {
                        mdtrt_id = ybjzlsh,
                        psn_no = grbh
                    }
                };

                string Err = string.Empty;
                cydjcxJson = JsonConvert.SerializeObject(input);
                #endregion



                WriteLog(sysdate + "  出院登记撤销|入参|" + cydjcxJson);
                int i = YBServiceRequest("2405", input, ref Err);
                if (i == 1)
                {
                    string dataRs = Err.ToString();
                    WriteLog(sysdate + "  出院登记撤销|出参Json|" + dataRs);
                    JObject jobj = JsonConvert.DeserializeObject(dataRs) as JObject; //
                    WriteLog(sysdate + "  出院登记撤销成功|" + Err.ToString());
                    return new object[] { 0, 1, "  出院登记撤销成功|" + Err.ToString() };
                }
                else
                {
                    WriteLog(sysdate + "  出院登记撤销失败|" + Err.ToString());
                    return new object[] { 0, 0, "  出院登记撤销失败|" + Err.ToString() };
                }

            }
            catch (Exception ex)
            {
                WriteLog(sysdate + "  出院登记撤销异常|" + ex.Message);
                return new object[] { 0, 0, "  出院登记撤销异常|" + ex.Message };
            }

        }

        #endregion

        #region  冲正交易
        public static object[] YBCZJY(object[] objParam)
        {
            string sysdate = GetServerDateTime();
            WriteLog(sysdate + "  进入冲正交易...");
            try
            {
                CZYBH = CliUtils.fLoginUser;   // 操作员工号 
                YWZQH = CliUtils.fLoginYbNo.Split('|')[0].ToString();   // 业务周期号
                string jbr = CliUtils.fUserName;  //经办人
                string ywbm = objParam[0].ToString();   // 冲正业务交易编码
                string jylsh = objParam[1].ToString();     // 被冲正交易发送方交易流水号
                string grbh = objParam[3].ToString();     // 个人编号
                string dqbh = objParam[4].ToString();     // 地区编号
                string jysj = Convert.ToDateTime(sysdate).ToString("yyyyMMddHHmmss");
                //交易流水号 

                #region 入参
                string ywczJson = string.Empty;
                /*
                 * 输入（节点标识：data）
                    人员编号
                    原发送方报文ID
                    原交易编号

                 */
                dynamic input = new
                {
                    data = new
                    {
                        psn_no = grbh,
                        omsgid = jylsh,
                        oinfno = ywbm
                    }

                };
                string Err = string.Empty;
                ywczJson = JsonConvert.SerializeObject(input);
                #endregion
                WriteLog(sysdate + "  冲正交易|入参Json|" + ywczJson);

                List<string> liSQL = new List<string>();
                int i = YBServiceRequest("2601", input, ref Err);
                if (i == 1)
                {
                    string dataRs = Err.ToString();
                    WriteLog(sysdate + "  冲正交易|出参Json|" + dataRs);
                    JObject jobj = JsonConvert.DeserializeObject(dataRs) as JObject; //获取响应数据json

                    WriteLog(sysdate + " 冲正交易成功|");
                    return new object[] { 0, 1, " 冲正交易成功|" };
                }
                else
                {
                    WriteLog(sysdate + "   冲正交易失败|" + Err.ToString());
                    return new object[] { 0, 0, "   冲正交易失败|" + Err.ToString() };
                }
            }
            catch (Exception ex)
            {
                WriteLog(sysdate + "   冲正交易异常|" + ex.Message);
                return new object[] { 0, 0, "   冲正交易异常|" + ex.Message };
            }
        }
        #endregion

        #region  冲正交易
        public static object[] YBCZJY_BY(object[] objParam)
        {
            string sysdate = GetServerDateTime();
            WriteLog(sysdate + "  进入冲正交易...");
            try
            {
                CZYBH = CliUtils.fLoginUser;   // 操作员工号 
                YWZQH = CliUtils.fLoginYbNo.Split('|')[0].ToString();   // 业务周期号
                string jbr = CliUtils.fUserName;  //经办人
                string ywbm = objParam[0].ToString();   // 冲正业务交易编码
                string jzlsh = objParam[1].ToString();   // 冲正业务交易编码
                string strSql = string.Format(@"select jylsh,grbh,tcqh from ybfyjsdr where jzlsh='{0}' and cxbz=1", jzlsh);
                DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                string jylsh = ds.Tables[0].Rows[0]["jylsh"].ToString();     // 被冲正交易发送方交易流水号
                string grbh = ds.Tables[0].Rows[0]["grbh"].ToString();     // 个人编号
                string dqbh = ds.Tables[0].Rows[0]["tcqh"].ToString();     // 地区编号
                string jysj = Convert.ToDateTime(sysdate).ToString("yyyyMMddHHmmss");
                //交易流水号 

                #region 入参
                string ywczJson = string.Empty;
                /*
                 * 输入（节点标识：data）
                    人员编号
                    原发送方报文ID
                    原交易编号

                 */
                dynamic input = new
                {
                    data = new
                    {
                        psn_no = grbh,
                        omsgid = jylsh,
                        oinfno = ywbm
                    }

                };

                string Err = string.Empty;
                ywczJson = JsonConvert.SerializeObject(input);
                #endregion
                WriteLog(sysdate + "  冲正交易|入参Json|" + ywczJson);

                List<string> liSQL = new List<string>();
                int i = YBServiceRequest("2601", input, ref Err);
                if (i == 1)
                {
                    string dataRs = Err.ToString();
                    WriteLog(sysdate + "  冲正交易|出参Json|" + dataRs);
                    JObject jobj = JsonConvert.DeserializeObject(dataRs) as JObject; //获取响应数据json

                    WriteLog(sysdate + " 冲正交易成功|");
                    return new object[] { 0, 1, " 冲正交易成功|" };
                }
                else
                {
                    WriteLog(sysdate + "   冲正交易失败|" + Err.ToString());
                    return new object[] { 0, 0, "   冲正交易失败|" + Err.ToString() };
                }
            }
            catch (Exception ex)
            {
                WriteLog(sysdate + "   冲正交易异常|" + ex.Message);
                return new object[] { 0, 0, "   冲正交易异常|" + ex.Message };
            }
        }
        #endregion


        #region 门诊登记(挂号)撤销（内部）（新医保接口）
        public static object[] NYBMZDJCX(object[] objParam)
        {
            string sysdate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string Ywlx = "2202";
            string jzlsh = objParam[0].ToString(); //就诊流水号
            string grbh = objParam[1].ToString(); //保险号(个人编号)
            string xm = objParam[2].ToString();  //姓名
            string kh = objParam[3].ToString(); //卡号
            string dqbh = objParam[4].ToString(); //地区编号
            string ybjzlsh = objParam[5].ToString();//医保就诊流水号



            #region 新医保接口 by hjw
            string res = string.Empty;
            dynamic input = new
            {
                data = new
                {
                    psn_no = grbh,
                    mdtrt_id = ybjzlsh,
                    ipt_otp_no = jzlsh
                }
            };
            string data = JsonConvert.SerializeObject(input);
            WriteLog(sysdate + "  " + jzlsh + " 进入门诊挂号撤销(内部)...");
            WriteLog(sysdate + "  入参|" + data.ToString());
            int i = YBServiceRequest(Ywlx, input, ref res);
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


        #region 4101医疗保障基金结算清单信息上传 孙志新 2021.05.15
        public static StringBuilder InputJson_Detail(StringBuilder WKstringBuilder, DataTable dt, string YN)
        {

            JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
            List<string> listSql = new List<string>();
            System.Collections.ArrayList dic = new System.Collections.ArrayList();
            foreach (DataRow dr in dt.Rows)
            {
                System.Collections.Generic.Dictionary<string, object> drow = new System.Collections.Generic.Dictionary<string, object>();
                foreach (DataColumn dc in dt.Columns)
                {
                    //drow.Add(dc.ColumnName, dr[dc.ColumnName]);
                    drow.Add(dc.ColumnName.ToLower(), string.IsNullOrWhiteSpace(dr[dc.ColumnName].ToString()) ? "0" : dr[dc.ColumnName]);

                }
                dic.Add(drow);
                javaScriptSerializer.Serialize(dic);
                listSql.Add(javaScriptSerializer.Serialize(dic).TrimStart('[').TrimEnd(']') + ",");
                dic.Clear();
            }
            dt.Clear();
            dt.Dispose();
            StringBuilder inputData = new StringBuilder();
            foreach (string inputParam in listSql)
            {
                inputData.Append(inputParam.ToString());
            }
            if (YN == "Y")
            {
                WKstringBuilder.Append("[" + inputData.ToString().TrimEnd(',') + "]");
            }
            else
            {
                WKstringBuilder.Append(inputData.ToString().TrimEnd(','));
            }
            return WKstringBuilder;

        }

        public static object[] JSQD_4101(object[] objParam)
        {
            #region 变量设定
            string YWBM = "4101";
            string YWMC = "医疗保障基金结算清单信息上传";
            string mdtrt_id = objParam[0].ToString();//住院号、门诊号
            string sign_date = DateTime.Now.ToString("yyyy-MM-dd");

            StringBuilder inputData = new StringBuilder();
            StringBuilder inputData_h = new StringBuilder();//节点名
            StringBuilder inputData_t = new StringBuilder(); //合并后多节点
            string certno = "";
            #endregion

            #region 取Data
            #region 结算清单信息（节点标识setlinfo）
            string strSql = string.Format(@"select top 1 * from DBYBJK..v_4101_setlinfo where mdtrt_id='{0}' ", mdtrt_id);
            DataSet dsJSON = CliUtils.ExecuteSql("scomm", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            if (dsJSON.Tables[0].Rows.Count > 0)
            {
                inputData_h.Append("\"setlinfo\":");
                inputData = InputJson_Detail(inputData_h, dsJSON.Tables[0], "N");
                inputData_t.Append(inputData);
                inputData_t.Append(",");
                dsJSON.Clear();
                dsJSON.Dispose();
                inputData_h.Clear();
                inputData.Clear();

            }
            else
            {
                dsJSON.Clear();
                dsJSON.Dispose();
                inputData_h.Clear();
                inputData.Clear();
                return new object[] { 0, 0, YWBM + YWMC + "失败|无结算清单信息" };
            }
            #endregion
            #region 基金支付信息（节点标识：payinfo）
            strSql = string.Format(@"select * from DBYBJK..v_4101_payinfo where mdtrt_id='{0}' ", mdtrt_id);
            dsJSON = CliUtils.ExecuteSql("scomm", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            if (dsJSON.Tables[0].Rows.Count > 0)
            {
                inputData_h.Append("\"payinfo\":");
                inputData = InputJson_Detail(inputData_h, dsJSON.Tables[0], "Y");
                inputData_t.Append(inputData);
                inputData_t.Append(",");
                dsJSON.Clear();
                dsJSON.Dispose();
                inputData_h.Clear();
                inputData.Clear();

            }
            else
            {
                dsJSON.Clear();
                dsJSON.Dispose();
                inputData_h.Clear();
                inputData.Clear();
                return new object[] { 0, 0, YWBM + YWMC + "失败|无门基金支付信息信息" };
            }

            //门诊慢特病诊断信息（节点标识：opspdiseinfo）
            strSql = string.Format(@"select * from DBYBJK..v_4101_opspdiseinfo where mdtrt_id='{0}' ", mdtrt_id);
            dsJSON = CliUtils.ExecuteSql("scomm", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            if (dsJSON.Tables[0].Rows.Count > 0)
            {
                inputData_h.Append("\"opspdiseinfo\":");
                inputData = InputJson_Detail(inputData_h, dsJSON.Tables[0], "Y");

                inputData_t.Append(inputData);
                inputData_t.Append(",");
                dsJSON.Clear();
                dsJSON.Dispose();
                inputData_h.Clear();
                inputData.Clear();

            }
            else
            {
                dsJSON.Clear();
                dsJSON.Dispose();
                string defaultStrJson = "\"opspdiseinfo\":" + "[{\"diag_name\":\"无\",\"diag_code\":\"0\",\"oprn_oprt_name\":\"无\",\"oprn_oprt_code\":\"0\",\"maindiag_flag\":\"1\"}],";
                inputData_t.Append(defaultStrJson);
                inputData_h.Clear();
                inputData.Clear();
                //return new object[] { 0, 0, YWBM + YWMC + "失败|无门诊慢特病诊断信息信息" };
            }

            //住院诊断信息（节点标识：diseinfo）
            strSql = string.Format(@"select * from DBYBJK..v_4101_diseinfo where mdtrt_id='{0}' ", mdtrt_id);
            dsJSON = CliUtils.ExecuteSql("scomm", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            if (dsJSON.Tables[0].Rows.Count > 0)
            {
                inputData_h.Append("\"diseinfo\":");
                inputData = InputJson_Detail(inputData_h, dsJSON.Tables[0], "Y");

                inputData_t.Append(inputData);
                inputData_t.Append(",");
                dsJSON.Clear();
                dsJSON.Dispose();
                inputData_h.Clear();
                inputData.Clear();

            }
            else
            {
                dsJSON.Clear();
                dsJSON.Dispose();
                inputData_h.Clear();
                inputData.Clear();
                return new object[] { 0, 0, YWBM + YWMC + "失败|无住院诊断信息" };
            }
            #endregion
            #region 收费项目信息（节点标识：iteminfo）
            strSql = string.Format(@"select * from DBYBJK..v_4101_iteminfo where mdtrt_id='{0}' ", mdtrt_id);
            dsJSON = CliUtils.ExecuteSql("scomm", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            if (dsJSON.Tables[0].Rows.Count > 0)
            {
                inputData_h.Append("\"iteminfo\":");
                inputData = InputJson_Detail(inputData_h, dsJSON.Tables[0], "Y");

                inputData_t.Append(inputData);
                inputData_t.Append(",");
                dsJSON.Clear();
                dsJSON.Dispose();
                inputData_h.Clear();
                inputData.Clear();

            }
            else
            {
                dsJSON.Clear();
                dsJSON.Dispose();
                inputData_h.Clear();
                inputData.Clear();
                return new object[] { 0, 0, YWBM + YWMC + "失败|无收费项目信息" };
            }
            #endregion
            #region 手术操作信息（节点标识：oprninfo）
            strSql = string.Format(@"select * from DBYBJK..v_4101_oprninfo where mdtrt_id='{0}' ", mdtrt_id);
            dsJSON = CliUtils.ExecuteSql("scomm", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            WriteLog("手术操作信息（节点标识：oprninfo）Sql: " + strSql);
            if (dsJSON.Tables[0].Rows.Count > 0)
            {
                inputData_h.Append("\"oprninfo\":");
                inputData = InputJson_Detail(inputData_h, dsJSON.Tables[0], "Y");

                inputData_t.Append(inputData);
                inputData_t.Append(",");
                dsJSON.Clear();
                dsJSON.Dispose();
                inputData_h.Clear();
                inputData.Clear();

            }
            else
            {
                dsJSON.Clear();
                dsJSON.Dispose();
                string defaultStrJson = "\"oprninfo\":" + "[{\"oprn_oprt_type\":\"2\",\"oprn_oprt_name\":\"无\",\"oprn_oprt_code\":\"0\",\"oprn_oprt_date\":\"0\",\"anst_way\":\"\",\"oprn_dr_name\":\"-\",\"oprn_dr_code\":\"-\",\"anst_dr_name\":\"张医生\",\"anst_dr_code\":\"admin\"}],";
                inputData_t.Append(defaultStrJson);
                inputData_h.Clear();
                inputData.Clear();
            }
            #endregion
            #region 重症监护信息（节点标识：icuinfo）
            string icuStr = "\"icuinfo\":[{\"scs_cutd_ward_type\":\"1\",\"scs_cutd_inpool_time\":\"" + DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd HH:mm:ss") + "\",\"scs_cutd_exit_time\":\"" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\",\"scs_cutd_sum_dura\":\"0/0/0\"}]";

            inputData_t.Append(icuStr);
            dsJSON.Clear();
            dsJSON.Dispose();
            //strSql = string.Format(@"select * from DBYBJK..v_4101_icuinfo where mdtrt_id='{0}' ", mdtrt_id);
            //dsJSON = CliUtils.ExecuteSql("scomm", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            //if (dsJSON.Tables[0].Rows.Count > 0)
            //{
            //    inputData_h.Append("\"icuinfo\":");
            //    inputData = InputJson_Detail(inputData_h, dsJSON.Tables[0], "Y");

            //    inputData_t.Append(inputData);
            //    dsJSON.Clear();
            //    dsJSON.Dispose();

            //}
            //else
            //{
            //    dsJSON.Clear();
            //    dsJSON.Dispose();
            //    return new object[] { 0, 0, YWBM + YWMC + "失败|无重症监护信息" };
            //}
            #endregion
            #endregion
            string inputStr = "{" + inputData_t.ToString() + "}";

            string sysdate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            WriteLog(sysdate + "4101医保基金结算清单信息上传入参|" + inputStr);
            dynamic dy = JsonConvert.DeserializeObject<dynamic>(inputStr);
            #region 接口调用
            string Result = string.Empty;
            int objBackMess = YBServiceRequest(YWBM, dy, ref Result);

            if (objBackMess == 1)
            {
                return new object[] { 0, 1, YWBM + YWMC + "成功" };

            }
            else
            {
                return new object[] { 0, 0, YWBM + YWMC + "失败|" + Result };
            }
            #endregion

        }
        #endregion

        #region 医保查询接口
        internal static string YWBH = string.Empty; //业务编号
        internal static string YWZQH = string.Empty;//业务周期号
        internal static string JYLSH = string.Empty;//交易流水号
        internal static string aMAC = string.Empty;//交易流水号
        internal static string CZYBH = string.Empty;//交易流水号
        internal static string ZXBM = "0000"; //中心编码 
        internal static string LJBZ = "1"; //联机标识 1：联机 0：脱机 
        internal static string DQJBBZ = string.Empty;//异地


        #region 科室信息查询
        public static object[] YBKSXXCX(object[] objParam)
        {
            string sysdate = GetServerDateTime();
            WriteLog(sysdate + "  进入科室信息查询...");
            WriteLog("进入科室信息查询|HIS传参|" + string.Join(",", objParam));
            try
            {
                CZYBH = CliUtils.fLoginUser; //操作员工号
                string jbr = CliUtils.fUserName;
                YWZQH = CliUtils.fLoginYbNo.Split('|')[0].ToString();    //业务周期号
                string jysj = Convert.ToDateTime(sysdate).ToString("yyyyMMddHHmmss");
                //交易流水号 
                string errstr = "";
                dynamic data = new
                {

                };
                string inputdata = JsonConvert.SerializeObject(data);
                WriteLog(sysdate + "科室查询|入参Json|" + inputdata);
                int i = YBServiceRequest("5101", data, ref errstr);
                if (i > 0)
                {
                    string dataRs = errstr;
                    WriteLog(sysdate + "  科室查询：" + "|出参Json|" + dataRs);
                    JObject jobj = JsonConvert.DeserializeObject(dataRs) as JObject; //获取响应数据json

                    JToken jtRet = jobj["output"]["feedetail"];
                    string strValues = "";
                    foreach (JToken jt in jtRet)
                    {
                        string ksdm = jt["hosp_dept_codg"].ToString();
                        string ksmc = jt["hosp_dept_name"].ToString();
                        string kssj = jt["begntime"].ToString();
                        string jssj = jt["endtime"].ToString();
                        string jjsm = jt["itro"].ToString();
                        string ksfzrxm = jt["dept_resper_name"].ToString();
                        string ksfzrdh = jt["dept_resper_tel"].ToString();
                        string ksylfwfw = jt["dept_med_serv_scp"].ToString();
                        string ybksdm = jt["caty"].ToString();
                        string ksclrq = jt["dept_estbdat"].ToString();
                        string pzcwsl = jt["aprv_bed_cnt"].ToString();
                        string ybrkcws = jt["hi_crtf_bed_cnt"].ToString();
                        string tcqh = jt["poolarea_no"].ToString();
                        string ysrs = jt["dr_psncnt"].ToString();
                        string yaosrs = jt["phar_psncnt"].ToString();
                        string hsrs = jt["nurs_psncnt"].ToString();
                        string jsrs = jt["tecn_psncnt"].ToString();
                        string bz = jt["memo"].ToString();
                        strValues += ksdm + "|" + ksmc + "|" + kssj + "|" + jssj + "|" + jjsm + "|" + ksfzrxm + "|" + ksfzrdh + "|" + ksylfwfw + "|" +
                                           ybksdm + "|" + ksclrq + "|" + pzcwsl + "|" + ybrkcws + "|" + tcqh + "|" + ysrs + "|" + yaosrs + "|" + hsrs + "|" + jsrs + "|" + bz + "$";
                    }
                    WriteLog(sysdate + " 科室查询成功|" + strValues.Trim('$'));
                    return new object[] { 0, 1, strValues };
                }
                else
                {
                    WriteLog(sysdate + "科室查询失败|" + errstr.ToString());
                    return new object[] { 0, 0, "科室查询失败|" + errstr.ToString() };
                }

            }
            catch (Exception ex)
            {
                WriteLog(sysdate + "科室查询异常|" + ex.Message);
                return new object[] { 0, 0, "科室查询异常|" + ex.Message };
            }
        }
        #endregion

        #region 住院费用登记撤销(内部)
        //public static object[] NYBZYCFMXSCCX(object[] objParam)
        //{
        //    string sysdate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        //    WriteLog(sysdate + "  进入住院费用登记撤销(内部)...");
        //    WriteLog("进入住院费用登记撤销(内部)|HIS传参|" + string.Join(",", objParam));
        //    try
        //    {
        //        CZYBH = CliUtils.fLoginUser; //操作员工号
        //        YWZQH = CliUtils.fLoginYbNo.Split('|')[0].ToString();    //业务周期号
        //        string ybjzlsh = objParam[0].ToString(); // 就诊流水号
        //        string cfmxjylsh = objParam[1].ToString();  //处方交易流水号
        //        string jbr = objParam[2].ToString();   // 经办人姓名
        //        string grbh = objParam[3].ToString();
        //        string xm = objParam[4].ToString();
        //        string kh = objParam[5].ToString();
        //        string dqbh = objParam[6].ToString();
        //        string ybjzlsh_sndy = objParam[7].ToString();
        //        DQJBBZ = objParam[8].ToString();

        //        string jysj = Convert.ToDateTime(sysdate).ToString("yyyyMMddHHmmss");
        //        //交易流水号
        //        JYLSH = YBJGBH + jysj + new Random().Next(100).ToString().PadLeft(4, '0');
        //        #region 入参
        //        string nbzyfydjcxJson = string.Empty;
        //        /*
        //         * 输入（节点标识：data）
        //         就诊ID
        //         人员编号
        //         */
        //        ZYFYDJCX_INPARAM zyfydjcx = new ZYFYDJCX_INPARAM();
        //        zysfdjcx_input input = new zysfdjcx_input();
        //        zysfdjcx_data data = new zysfdjcx_data();
        //        data.feedetl_sn = "0000";
        //        data.mdtrt_id = ybjzlsh;
        //        data.psn_no = grbh;
        //        input.data = data;
        //        zyfydjcx.infno = "2302";
        //        zyfydjcx.msgid = JYLSH;
        //        zyfydjcx.mdtrtarea_admvs = MdtrtareaAdmvs;
        //        zyfydjcx.insuplc_admdvs = dqbh;
        //        zyfydjcx.recer_sys_code = RecerSysCode;
        //        zyfydjcx.dev_no = "";
        //        zyfydjcx.dev_safe_info = "";
        //        zyfydjcx.cainfo = "";
        //        zyfydjcx.signtype = "";
        //        zyfydjcx.infver = Infver;
        //        zyfydjcx.opter_type = OpterType;
        //        zyfydjcx.opter = CZYBH;
        //        zyfydjcx.opter_name = jbr;
        //        zyfydjcx.inf_time = sysdate;
        //        zyfydjcx.fixmedins_code = YBJGBH;
        //        zyfydjcx.fixmedins_name = DDYLJGMC;
        //        zyfydjcx.sign_no = YWZQH;
        //        zyfydjcx.fixmedins_soft_fcty = SoftName;
        //        zyfydjcx.input = input;
        //        nbzyfydjcxJson = JsonConvert.SerializeObject(zyfydjcx);
        //        #endregion

        //        WriteLog(sysdate + "  住院费用登记撤销(内部)|入参Json|" + nbzyfydjcxJson);

        //        object[] objJson = pubpost.Post(nbzyfydjcxJson);
        //        if (objJson[1].ToString() == "1")
        //        {
        //            string dataRs = objJson[2].ToString();
        //            WriteLog(sysdate + "  住院费用登记撤销(内部)|出参Json|" + dataRs);
        //            JObject jobj = JsonConvert.DeserializeObject(dataRs) as JObject; //获取响应数据json

        //            WriteLog(sysdate + "  住院费用登记撤销(内部)成功|" + objJson[2].ToString());
        //            return new object[] { 0, 1, "  住院费用登记撤销(内部)成功|" + objJson[2].ToString() };
        //        }
        //        else
        //        {
        //            WriteLog(sysdate + "  住院费用登记撤销(内部)失败|" + objJson[2].ToString());
        //            return new object[] { 0, 0, "  住院费用登记撤销(内部)失败|" + objJson[2].ToString() };
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        WriteLog(sysdate + "  住院费用登记撤销(内部)|系统异常" + ex.Message);
        //        return new object[] { 0, 0, "系统异常" + ex.Message };
        //    }
        //}
        #endregion

        #region  住院收费结算撤销(内部)
        //public static object[] NYBZYFYJSCX(object[] objParam)
        //{
        //    string sysdate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        //    WriteLog(sysdate + "  进入住院收费结算撤销(内部)...");
        //    WriteLog("进入住院收费结算撤销(内部)|HIS传参|" + string.Join(",", objParam));
        //    try
        //    {
        //        CZYBH = CliUtils.fLoginUser;   // 操作员工号 
        //        YWZQH = CliUtils.fLoginYbNo.Split('|')[0].ToString();   // 业务周期号
        //        string jbr = CliUtils.fUserName;  //经办人
        //        string jzlsh = objParam[0].ToString();   // 就诊流水号
        //        string djh = objParam[1].ToString();     // 结算单据号
        //        string ybjzlsh = objParam[2].ToString();   //"1100008927"
        //        string xm = objParam[3].ToString();         // // "徐国强""施美荣";
        //        string kh = objParam[4].ToString();          // ;"520313262"//"469285189";
        //        string grbh = objParam[5].ToString();     //"360402837288" ;//"360403700513";
        //        string jsrq = objParam[6].ToString();      //"20160308180140" ;//"20160129000000";
        //        string jslsh = objParam[7].ToString();    //结算流水号  
        //        string ybjzlsh_snyd = objParam[8].ToString();
        //        string dqbh = objParam[9].ToString();
        //        DQJBBZ = objParam[10].ToString();    //地区级别标志
        //        string jysj = Convert.ToDateTime(sysdate).ToString("yyyyMMddHHmmss");
        //        //交易流水号
        //        JYLSH = YBJGBH + jysj + new Random().Next(100).ToString().PadLeft(4, '0');

        //        #region 入参
        //        string zyjscxJson = string.Empty;
        //        /*
        //         * 输入（节点标识：data）
        //         就诊ID
        //         人员编号
        //         */
        //        ZYJSCX_INPARAM zyjscx = new ZYJSCX_INPARAM();
        //        zyjscx_input input = new zyjscx_input();
        //        zyjscx_data data = new zyjscx_data();
        //        data.mdtrt_id = ybjzlsh;
        //        data.setl_id = jslsh;
        //        data.psn_no = grbh;
        //        input.data = data;
        //        zyjscx.infno = "2305";
        //        zyjscx.msgid = JYLSH;
        //        zyjscx.mdtrtarea_admvs = MdtrtareaAdmvs;
        //        zyjscx.insuplc_admdvs = dqbh;
        //        zyjscx.recer_sys_code = RecerSysCode;
        //        zyjscx.dev_no = "";
        //        zyjscx.dev_safe_info = "";
        //        zyjscx.cainfo = "";
        //        zyjscx.signtype = "";
        //        zyjscx.infver = Infver;
        //        zyjscx.opter_type = OpterType;
        //        zyjscx.opter = CZYBH;
        //        zyjscx.opter_name = jbr;
        //        zyjscx.inf_time = sysdate;
        //        zyjscx.fixmedins_code = YBJGBH;
        //        zyjscx.fixmedins_name = DDYLJGMC;
        //        zyjscx.sign_no = YWZQH;
        //        zyjscx.fixmedins_soft_fcty = SoftName;
        //        zyjscx.input = input;
        //        zyjscxJson = JsonConvert.SerializeObject(zyjscx);
        //        #endregion

        //        WriteLog(sysdate + "  住院收费结算撤销(内部)|入参|" + zyjscxJson);
        //        object[] objJson = pubpost.Post(zyjscxJson);
        //        if (objJson[1].ToString() == "1")
        //        {
        //            string dataRs = objJson[2].ToString();
        //            WriteLog(sysdate + "  住院收费结算撤销(内部)|出参Json|" + dataRs);
        //            JObject jobj = JsonConvert.DeserializeObject(dataRs) as JObject; //获取响应数据json
        //            
        //            WriteLog(sysdate + "  住院收费结算撤销(内部)成功|出参|" + objJson[2].ToString());
        //            return new object[] { 0, 1, "住院收费结算撤销(内部)成功" };

        //        }
        //        else
        //        {
        //            WriteLog(sysdate + "   住院收费结算撤销(内部)失败|" + objJson[2].ToString());
        //            return new object[] { 0, 0, "   住院收费结算撤销(内部)失败|" + objJson[2].ToString() };
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        WriteLog(sysdate + "   住院收费结算撤销(内部)|系统异常|" + ex.Message);
        //        return new object[] { 0, 0, ex.Message };
        //    }
        //}
        #endregion

        #region 获取病种信息查询
        public static object[] YBBZCX(object[] objParam)
        {
            string yllb = objParam[0].ToString(); // 医疗类别
            string grbh = objParam[1].ToString(); //个人编号
            string jzbz = objParam[2].ToString();   //门诊住院标志 m-门诊 z-住院
            string splb = objParam[3].ToString();   //审批类别
            string lb = "";//住院：单病种/特殊病种 
            if (objParam.Length > 4)
                lb = objParam[4].ToString();

            string[] syl_mz = { "1401", "1402", "1403", "9903" };
            string[] rylb_mz = { "11", "21", "31", "32" };

            string strSql = string.Empty;
            if (jzbz.ToUpper().Equals("M"))
            {
                strSql = string.Format(@"select dm,dmmc,pym,wbm from ybbzmrdr where 1=1");
                if (syl_mz.Contains(yllb))
                {
                    strSql += string.Format(@" and (yllb = '{0}' or yllb='14') ", yllb);
                }

            }
            else if (jzbz.ToUpper().Equals("Z"))
            {
                if (syl_mz.Contains(yllb))
                {
                    strSql += string.Format(@" and yllb = '{0}'", yllb);
                }
                strSql = string.Format(@"select dm,dmmc,pym,wbm from ybbzmrdr where yllb='11'");
            }
            else
            {
                return new object[] { 0, 0, "门诊住院标志入参有误" };
            }
            DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            return new object[] { 0, 1, ds.Tables[0] };
        }
        #endregion

        #region 医保对照项目批量上传
        public static object[] YBDZXXPLSC(object[] objParam)
        {
            string sysdate = GetServerDateTime();//当前时间
            WriteLog(sysdate + "  进入医保对照项目批量上传...");
            WriteLog(sysdate + "  HIS入参|" + string.Join(",", objParam));
            try
            {
                #region 入参
                string scms = objParam[0].ToString(); //上传模式 1-全部上传 2-单条上传
                string yyxmbh = "";
                string ybxmbh = "";
                if (objParam.Length > 1)
                    yyxmbh = objParam[1].ToString(); //医院项目代码
                ybxmbh = objParam[2].ToString(); //医保项目代码

                if (scms.Equals("2") && string.IsNullOrEmpty(yyxmbh))
                    return new object[] { 0, 0, "单条项目上传时，医院项目代码不能为空" };
                #endregion

                #region 获取对照信息
                string strSql = string.Format(@"select hisxmbh,hisxmmc,sflbdm,sflb,sfxmzldm,sfxmzl,ybxmbh,ksrq,jsrq,pzwh,jxdm,gg,jx from ybhisdzdrnew where scbz=0 ");
                if (scms.Equals("2"))
                    strSql += string.Format(@"and hisxmbh='{0}'", yyxmbh);
                DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                if (ds.Tables[0].Rows.Count == 0)
                {
                    WriteLog(sysdate + "  无上传费用明细信息！");
                    return new object[] { 0, 0, "无上传费用明细信息" };
                }
                #endregion

                #region 入参处理
                List<string> liSql = new List<string>();
                string err_msg = "";
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    #region 入参
                    dynamic catalogcompin = new
                    {
                        data = new
                        {
                            fixmedins_hilist_id = Convert.ToString(dr["hisxmbh"]),
                            fixmedins_hilist_name = Convert.ToString(dr["hisxmmc"]),
                            list_type = Convert.ToString(dr["sfxmzldm"]),
                            med_list_codg = /*Convert.ToString(dr["ybxmbh"])*/ybxmbh,
                            begndate = Convert.ToDateTime(dr["ksrq"]).ToString("yyyy-MM-dd"),
                            enddate = Convert.ToDateTime(dr["jsrq"]).ToString("yyyy-MM-dd"),
                            aprvno = Convert.ToString(dr["pzwh"]),
                            dosform = Convert.ToString(dr["jx"]),
                            exct_cont = "",
                            item_cont = "",
                            prcunt = "",
                            spec = Convert.ToString(dr["gg"]),
                            pacspec = "",
                            memo = ""
                        }
                    };
                    string Err = string.Empty;
                    #endregion

                    #region 接口调用
                    string inputData = JsonConvert.SerializeObject(catalogcompin);
                    WriteLog(sysdate + "  入参|" + inputData.ToString());
                    int i = YBServiceRequest("3301", catalogcompin, ref Err);
                    WriteLog(sysdate + "  出参|" + Err.ToString());
                    if (i == 1)
                    {
                        if (!string.IsNullOrEmpty(yyxmbh))
                        {
                            strSql = string.Format(@"update ybhisdzdrnew set scbz=1 where hisxmbh='{0}'", yyxmbh);
                        }
                        else
                        {
                            strSql = string.Format(@"update ybhisdzdrnew set scbz=1 where isnull(scbz,'0')!='1' ", yyxmbh);
                        }
                        liSql.Add(strSql);
                    }
                    else
                    {
                        WriteLog(sysdate + "  " + Err.ToString());
                        err_msg += Err.ToString();
                    }
                    #endregion
                }
                #endregion

                #region 数据处理
                object[] obj = liSql.ToArray();
                obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);
                if (obj[1].ToString().Equals("1"))
                {
                    if (string.IsNullOrEmpty(err_msg))
                    {
                        WriteLog(sysdate + "    医保对照项目上传成功");
                        return new object[] { 0, 1, "医保对照项目上传成功" };
                    }
                    else
                    {
                        WriteLog(sysdate + "    医保对照项目上传失败|" + err_msg);
                        return new object[] { 0, 0, "医保对照项目上传失败|" + err_msg };
                    }
                }
                else
                {
                    WriteLog(sysdate + "    医保对照项目上传失败|本地数据操作失败");
                    return new object[] { 0, 0, "医保对照项目上传失败|本地数据操作失败" };
                }
                #endregion

            }
            catch (Exception ex)
            {
                WriteLog(sysdate + "  医保对照项目批量上传异常|" + ex.ToString());
                return new object[] { 0, 0, "医保对照项目批量上传异常|" + ex.ToString() };
            }
        }
        #endregion

        #region 医保对照撤销
        public static object[] YBDZXXSCCX(object[] objParam)
        {
            string sysdate = GetServerDateTime();//当前时间
            WriteLog(sysdate + "  进入医保对照项目批量上传撤销...");
            WriteLog(sysdate + "  HIS入参|" + string.Join(",", objParam));
            try
            {
                #region 入参
                string yyxmbh = objParam[0].ToString();
                if (string.IsNullOrEmpty(yyxmbh))
                    return new object[] { 0, 0, "医院项目编码不能为空" };
                #endregion

                #region 获取对照信息
                string strSql = string.Format(@"select * from ybhisdzdrnew where scbz=1 and hisxmbh='{0}'", yyxmbh);
                DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                if (ds.Tables[0].Rows.Count == 0)
                {
                    WriteLog(sysdate + "  未找到需要撤销数据");
                    return new object[] { 0, 0, "未找到需要撤销数据" };
                }
                DataRow dr = ds.Tables[0].Rows[0];
                #endregion

                #region 入参处理
                dynamic catalogcompin = new
                {
                    data = new
                    {

                        fixmedins_code = YBJGBH,
                        fixmedins_hilist_id = Convert.ToString(dr["hisxmbh"]),
                        list_type = Convert.ToString(dr["sfxmzldm"]),
                        med_list_codg = Convert.ToString(dr["ybxmbh"])
                    }
                };
                string Err = string.Empty;
                #endregion

                #region 接口调用
                string inputData = JsonConvert.SerializeObject(catalogcompin);
                WriteLog(sysdate + "  入参|" + inputData.ToString());
                int i = YBServiceRequest("3302", catalogcompin, ref Err);
                WriteLog(sysdate + "  出参|" + Err.ToString());
                if (i != 1)
                {
                    WriteLog(sysdate + "  " + Err.ToString());
                    return new object[] { 0, 0, "医保对照项目批量上传撤销失败|" + Err.ToString() };
                }
                #endregion

                #region 保存数据
                strSql = string.Format(@"delete from ybhisdzdrnew where hisxmbh='{0}'", yyxmbh);
                object[] obj = new object[] { strSql };
                obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);
                if (obj[1].ToString().Equals("1"))
                {
                    WriteLog(sysdate + "    医保对照项目撤销成功");
                    return new object[] { 0, 1, "医保对照项目撤销成功" };
                }
                else
                {
                    WriteLog(sysdate + "    医保对照项目撤销失败|本地数据操作失败");
                    return new object[] { 0, 0, "医保对照项目撤销失败|本地数据操作失败" };
                }
                #endregion

            }
            catch (Exception ex)
            {
                WriteLog(sysdate + "  医保对照项目批量上传撤销异常|" + ex.ToString());
                return new object[] { 0, 0, "医保对照项目批量上传撤销异常|" + ex.ToString() };
            }
        }
        #endregion

        #region 医疗待遇封锁信息查询
        public static object[] YBYLDYFSXXCX(object[] objParam)
        {
            string sysdate = GetServerDateTime();//当前时间
            WriteLog(sysdate + "  进入医疗待遇封锁信息查询...");
            WriteLog(sysdate + "  HIS入参|" + string.Join(",", objParam));
            try
            {
                #region His入参
                string grbh = objParam[0].ToString(); //人员编号
                string xzlx = objParam[1].ToString(); //险种类型
                string ddyljbbm = objParam[2].ToString(); //定点医药机构编号
                string yllb = objParam[3].ToString(); //医疗类别
                string kssj = objParam[4].ToString(); //开始时间
                string jssj = objParam[5].ToString(); //结束时间
                string bzbm = objParam[6].ToString(); //病种编码
                string bzmc = objParam[7].ToString(); //病种名称
                string ssczdm = objParam[8].ToString(); //手术操作代码
                string ssczmc = objParam[9].ToString(); //手术操作名称
                string sylb = objParam[10].ToString(); //生育类别
                string jhsysslb = objParam[11].ToString(); //计划生育手术类别

                if (string.IsNullOrEmpty(grbh))
                    return new object[] { 0, 0, "人员编号不能为空" };
                if (string.IsNullOrEmpty(xzlx))
                    return new object[] { 0, 0, "险种类型不能为空" };
                if (string.IsNullOrEmpty(ddyljbbm))
                    return new object[] { 0, 0, "定点医药机构编号不能为空" };
                if (string.IsNullOrEmpty(yllb))
                    return new object[] { 0, 0, "医疗类别不能为空" };
                if (string.IsNullOrEmpty(kssj))
                    return new object[] { 0, 0, "开始时间不能为空" };
                #endregion

                #region 入参处理
                dynamic data = new
                {
                    data = new
                    {
                        psn_no = grbh,
                        insutype = xzlx,
                        fixmedins_code = YBJGBH,
                        med_type = yllb,
                        begntime = kssj,
                        endtime = jssj,
                        dise_codg = bzbm,
                        dise_name = bzmc,
                        oprn_oprt_code = ssczdm,
                        oprn_oprt_name = ssczmc,
                        matn_type = sylb,
                        birctrl_type = jhsysslb
                    }

                };

                #endregion
                string Err = string.Empty;

                #region 接口处理
                string inputData = JsonConvert.SerializeObject(Err);
                WriteLog(sysdate + "  入参|" + inputData.ToString());
                int i = YBServiceRequest("2001", data, ref Err);
                WriteLog(sysdate + "  出参|" + Err.ToString());
                if (i != 1)
                {
                    WriteLog(sysdate + "  " + Err.ToString());
                    return new object[] { 0, 0, "医疗待遇封锁信息查询失败|" + Err.ToString() };
                }
                #endregion
                JObject j = JsonConvert.DeserializeObject<JObject>(Err);
                #region 数据处理
                string fsxx = ""; //封锁信息
                JToken jts = Convert.ToString(j["trtinfo"]);
                foreach (JToken jt in jts)
                {
                    fsxx += Convert.ToString(jt["trt_chk_rslt"]);
                }

                if (!string.IsNullOrEmpty(fsxx))
                    return new object[] { 0, 0, fsxx };
                else
                    return new object[] { 0, 1, "待遇检查结果正常!" };
                #endregion
            }
            catch (Exception ex)
            {
                WriteLog(sysdate + "  医疗待遇封锁信息查询异常|" + ex.ToString());
                return new object[] { 0, 0, "医疗待遇封锁信息查询异常|" + ex.ToString() };
            }
        }
        #endregion

        #region 医疗待遇审批信息查询(医疗待遇审批信息查询)
        public static object[] YBYLDYSPXXCX(object[] objParam)
        {
            string sysdate = GetServerDateTime();//当前时间
            WriteLog(sysdate + "  进入医疗待遇审批信息查询...");
            WriteLog(sysdate + "  HIS入参|" + string.Join(",", objParam));
            try
            {
                #region HIS入参
                string grbh = objParam[0].ToString();
                if (string.IsNullOrEmpty(grbh))
                    return new object[] { 0, 0, "个人编码不能为空" };
                #endregion

                #region 获取读卡信息
                string strSql = string.Format(@"select * from ybickxx where grbh='{0}'", grbh);
                DataSet dsDK = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                if (dsDK.Tables[0].Rows.Count == 0)
                    return new object[] { 0, 0, "无法获取读卡信息" };
                DataRow dr = dsDK.Tables[0].Rows[0];
                string xm = dr["xm"].ToString();
                string kh = dr["kh"].ToString();
                string cbdybqh = dr["tcqh"].ToString();

                #endregion

                #region 入参
                dynamic data = new
                {
                    data = new
                    {
                        psn_no = grbh
                    }
                };
                #endregion
                string Err = string.Empty;
                #region 接口调用
                string inputData = JsonConvert.SerializeObject(data);
                WriteLog(sysdate + "  入参|" + inputData.ToString());
                int i = YBServiceRequest("5301", data, ref Err);
                WriteLog(sysdate + "  出参|" + Err.ToString());
                if (i != 1)
                {
                    WriteLog(sysdate + "  医疗待遇审批信息查询失败|" + Err.ToString());
                    return new object[] { 0, 0, Err.ToString() };
                }
                #endregion

                #region 数据处理
                List<string> liSql = new List<string>();
                strSql = string.Format(@"delete from ybmxbdj where bxh='{0}'", grbh);
                liSql.Add(strSql);

                JObject j = JsonConvert.DeserializeObject<JObject>(Err);
                JToken jts = j["feedetail"];
                foreach (JToken jt in jts)
                {
                    string mmbzbm = Convert.ToString(jt["opsp_dise_code"]);
                    string mmbzmc = Convert.ToString(jt["opsp_dise_name"]);
                    string ksrq = Convert.ToString(jt["begndate"]);
                    string jsrq = Convert.ToString(jt["enddate"]);
                    string yllb = "14";
                    string bzlb = "门诊慢特病";

                    strSql = string.Format(@"insert into ybmxbdj(bxh,xm,kh,MMBZBM,MMBZMC,YLLB,BZLB,ksrq,jsrq) values(
                                            '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}')",
                                             grbh, xm, kh, mmbzbm, mmbzmc, yllb, bzlb, ksrq, jsrq);
                    liSql.Add(strSql);
                }

                object[] obj = liSql.ToArray();
                obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);
                if (obj[1].ToString().Equals("1"))
                {
                    WriteLog(sysdate + "  医疗待遇审批信息查询成功|");
                    return new object[] { 0, 1, "医疗待遇审批信息查询成功" };
                }
                else
                {
                    WriteLog(sysdate + "  医疗待遇审批信息查询失败|数据操作失败|" + obj[2].ToString());
                    return new object[] { 0, 0, "医疗待遇审批信息查询失败|数据操作失败|" + obj[2].ToString() };
                }
                #endregion
            }
            catch (Exception ex)
            {
                WriteLog(sysdate + "  医疗待遇审批信息查询异常|" + ex.ToString());
                return new object[] { 0, 0, "医疗待遇审批信息查询异常|" + ex.ToString() };
            }
        }
        #endregion

        #region 医师信息查询(医执人员信息查询)
        public static object[] YBYSXXCX(object[] objParam)
        {
            string sysdate = GetServerDateTime();//当前时间
            WriteLog(sysdate + "  进入医师信息查询...");
            WriteLog(sysdate + "  HIS入参|" + string.Join(",", objParam));
            try
            {
                #region HIS入参
                string zyryfl = objParam[0].ToString(); //执业人员分类  1-医师 2-护士   3-药师 4-医技人员
                string zjlx = objParam[1].ToString();   //人员证件类型
                string jzhm = objParam[2].ToString(); //证件号码

                if (string.IsNullOrEmpty(zyryfl))
                    return new object[] { 0, 0, "执业人员分类不能为空" };
                #endregion

                #region 入参
                dynamic data = new
                {
                    data = new
                    {
                        prac_psn_type = zyryfl,
                        psn_cert_type = zjlx,
                        certno = jzhm,
                        prac_psn_name = "",
                        prac_psn_code = ""
                    }
                };


                #endregion

                #region 接口调用
                string errstr = string.Empty;
                string inputData = JsonConvert.SerializeObject(data);
                WriteLog(sysdate + "  入参|" + inputData.ToString());
                int i = YBServiceRequest("5102", data, ref errstr);
                JObject ret = JsonConvert.DeserializeObject(errstr) as JObject;
                WriteLog(sysdate + "  出参|" + errstr.ToString());
                if (i <= 0)
                {
                    WriteLog(sysdate + "  医师信息查询失败|" + errstr.ToString());
                    return new object[] { 0, 0, errstr.ToString() };
                }
                #endregion

                WriteLog(sysdate + "  医师信息查询成功|" + errstr.ToString());
                return new object[] { 0, 1, errstr.ToString() };

            }
            catch (Exception ex)
            {
                WriteLog(sysdate + "  医师信息查询异常|" + ex.ToString());
                return new object[] { 0, 0, "医师信息查询异常|" + ex.ToString() };
            }

        }
        #endregion

        #region 在院信息查询
        public static object[] YBZYXXCX(object[] objParam)
        {
            string sysdate = GetServerDateTime();//当前时间
            WriteLog(sysdate + "  进入在院信息查询...");
            WriteLog(sysdate + "  HIS入参|" + string.Join(",", objParam));
            try
            {
                CZYBH = CliUtils.fLoginUser;   // 操作员工号 
                YWZQH = CliUtils.fLoginYbNo.Split('|')[0].ToString();   // 业务周期号
                string jbr = CliUtils.fUserName;
                string jysj = Convert.ToDateTime(sysdate).ToString("yyyyMMddHHmmss");
                //交易流水号
                JYLSH = YBJGBH + jysj + new Random().Next(100).ToString().PadLeft(4, '0');
                string grbh = objParam[0].ToString();
                string kssj = objParam[1].ToString();
                string jssj = objParam[2].ToString();
                #region 入参
                string zyxxcxJson = string.Empty;
                dynamic data = new
                {
                    data = new
                    {
                        psn_no = grbh,
                        begntime = kssj,
                        endtime = jssj
                    }
                };

                zyxxcxJson = JsonConvert.SerializeObject(data);
                string errstr = string.Empty;
                #endregion

                #region 在院信息查询
                WriteLog(sysdate + "  在院信息查询|入参Json|" + zyxxcxJson);
                int i = YBServiceRequest("5303", data, ref errstr);
                if (i > 0)
                {
                    string dataRs = errstr.ToString();
                    WriteLog(sysdate + "  在院信息查询|出参Json|" + errstr); //获取响应数据json 
                                                                     //获取返回json转为dataset 
                    List<JObject> ds = JsonConvert.DeserializeObject<List<JObject>>(JObject.Parse(errstr)["data"].ToString());
                    WriteLog(sysdate + "  在院信息查询成功|" + errstr);
                    return new object[] { 0, 1, ds };

                }
                else
                {

                    WriteLog(sysdate + "  在院信息查询失败|" + errstr.ToString());
                    return new object[] { 0, 0, " 在院信息查询失败|" + errstr.ToString() };
                }
                #endregion

            }
            catch (Exception ex)
            {
                WriteLog(sysdate + "  在院信息查询异常|" + ex.Message);
                return new object[] { 0, 0, " 在院信息查询异常|" + ex.Message };
            }
        }

        #endregion

        #region 就诊信息查询
        public static object[] YBJZXXCX(object[] objParam)
        {
            string sysdate = GetServerDateTime();//当前时间
            WriteLog(sysdate + "  进入就诊信息查询...");
            WriteLog(sysdate + "  HIS入参|" + string.Join(",", objParam));
            try
            {
                CZYBH = CliUtils.fLoginUser;   // 操作员工号 
                YWZQH = CliUtils.fLoginYbNo.Split('|')[0].ToString();   // 业务周期号
                string jbr = CliUtils.fUserName;
                string jysj = Convert.ToDateTime(sysdate).ToString("yyyyMMddHHmmss");
                //交易流水号
                JYLSH = YBJGBH + jysj + new Random().Next(100).ToString().PadLeft(4, '0');
                string grbh = objParam[0].ToString();
                string kssj = objParam[1].ToString();
                string jssj = objParam[2].ToString();
                string yllb = objParam[3].ToString();
                string jzid = objParam[4].ToString();
                #region 入参
                string jzxxcxJson = string.Empty;
                dynamic data = new
                {
                    data = new
                    {
                        psn_no = grbh,
                        begntime = kssj,
                        endtime = jssj,
                        med_type = yllb,
                        mdtrt_id = jzid
                    }


                };

                jzxxcxJson = JsonConvert.SerializeObject(data);
                #endregion
                string errstr = string.Empty;
                #region 就诊信息查询
                WriteLog(sysdate + "  就诊信息查询|入参Json|" + jzxxcxJson);
                int i = YBServiceRequest("5201", data, ref errstr);
                if (i > 0)
                {
                    string dataRs = errstr.ToString();
                    WriteLog(sysdate + "  就诊信息查询|出参Json|" + dataRs);
                    var obj = JsonConvert.DeserializeObject<List<JObject>>(JObject.Parse(dataRs)["mdtrtinfo"].ToString());
                    WriteLog(sysdate + "  就诊信息查询成功|" + dataRs);
                    return new object[] { 0, 1, obj };

                }
                else
                {

                    WriteLog(sysdate + "  就诊信息查询失败|" + errstr.ToString());
                    return new object[] { 0, 0, " 就诊信息查询失败|" + errstr.ToString() };
                }
                #endregion

            }
            catch (Exception ex)
            {
                WriteLog(sysdate + "  就诊信息查询异常|" + ex.Message);
                return new object[] { 0, 0, " 就诊信息查询异常|" + ex.Message };
            }
        }

        #endregion

        #region 诊断信息查询
        public static object[] YBZDXXCX(object[] objParam)
        {
            string sysdate = GetServerDateTime();//当前时间
            WriteLog(sysdate + "  进入诊断信息查询...");
            WriteLog(sysdate + "  HIS入参|" + string.Join(",", objParam));
            try
            {
                CZYBH = CliUtils.fLoginUser;   // 操作员工号 
                YWZQH = CliUtils.fLoginYbNo.Split('|')[0].ToString();   // 业务周期号
                string jbr = CliUtils.fUserName;
                string jysj = Convert.ToDateTime(sysdate).ToString("yyyyMMddHHmmss");
                //交易流水号
                JYLSH = YBJGBH + jysj + new Random().Next(100).ToString().PadLeft(4, '0');
                string grbh = objParam[0].ToString();
                string jzid = objParam[1].ToString();
                #region 入参
                string jzxxcxJson = string.Empty;
                dynamic data = new
                {
                    data = new
                    {
                        psn_no = grbh,
                        mdtrt_id = jzid
                    }

                };

                jzxxcxJson = JsonConvert.SerializeObject(data);
                #endregion
                string errstr = string.Empty;
                #region 就诊信息查询
                WriteLog(sysdate + "  诊断信息查询|入参Json|" + jzxxcxJson);
                int i = YBServiceRequest("5202", data, ref errstr);
                if (i > 0)
                {
                    string dataRs = errstr.ToString();
                    WriteLog(sysdate + "  诊断信息查询|出参Json|" + dataRs);
                    var obj = JsonConvert.DeserializeObject<List<JObject>>(JObject.Parse(dataRs)["diseinfo"].ToString());
                    WriteLog(sysdate + "  诊断信息查询成功|" + dataRs);
                    return new object[] { 0, 1, obj };

                }
                else
                {

                    WriteLog(sysdate + "  诊断信息查询失败|" + errstr.ToString());
                    return new object[] { 0, 0, " 诊断信息查询失败|" + errstr.ToString() };
                }
                #endregion

            }
            catch (Exception ex)
            {
                WriteLog(sysdate + "  诊断信息查询异常|" + ex.Message);
                return new object[] { 0, 0, " 诊断信息查询异常|" + ex.Message };
            }
        }
        #endregion

        #region 费用明细查询
        public static object[] YBFYMXCX(object[] objParam)
        {
            string sysdate = GetServerDateTime();//当前时间
            WriteLog(sysdate + "  进入费用明细查询...");
            WriteLog(sysdate + "  HIS入参|" + string.Join(",", objParam));
            try
            {
                CZYBH = CliUtils.fLoginUser;   // 操作员工号 
                YWZQH = CliUtils.fLoginYbNo.Split('|')[0].ToString();   // 业务周期号
                string jbr = CliUtils.fUserName;
                string jysj = Convert.ToDateTime(sysdate).ToString("yyyyMMddHHmmss");
                //交易流水号
                JYLSH = YBJGBH + jysj + new Random().Next(100).ToString().PadLeft(4, '0');
                string grbh = objParam[0].ToString();
                string jsid = objParam[1].ToString();
                string jzid = objParam[2].ToString();
                #region 入参
                string jzxxcxJson = string.Empty;
                dynamic data = new
                {
                    data = new
                    {
                        psn_no = grbh,
                        mdtrt_id = jzid,
                        setl_id = jsid
                    }

                };

                jzxxcxJson = JsonConvert.SerializeObject(data);
                #endregion
                string errstr = string.Empty;
                #region 就诊信息查询
                WriteLog(sysdate + "  费用明细查询|入参Json|" + jzxxcxJson);
                int i = YBServiceRequest("5204", data, ref errstr);
                if (i > 0)
                {
                    string dataRs = errstr.ToString();
                    WriteLog(sysdate + "  费用明细查询|出参Json|" + dataRs);
                    dataRs = dataRs.Replace("\r\n", "").Replace("  ", "").Replace("    ", "");
                    var obj = JsonConvert.DeserializeObject<List<JObject>>(dataRs);
                    WriteLog(sysdate + "  费用明细查询成功|" + dataRs);
                    return new object[] { 0, 1, obj };

                }
                else
                {

                    WriteLog(sysdate + "  费用明细查询失败|" + errstr.ToString());
                    return new object[] { 0, 0, " 费用明细查询失败|" + errstr.ToString() };
                }
                #endregion

            }
            catch (Exception ex)
            {
                WriteLog(sysdate + "  费用明细查询异常|" + ex.Message);
                return new object[] { 0, 0, " 费用明细查询异常|" + ex.Message };
            }
        }
        #endregion

        #region 字典表查询
        public static object[] YBZDBCX(object[] objParam)
        {
            string sysdate = GetServerDateTime();//当前时间
            WriteLog(sysdate + "  进入字典表查询...");
            WriteLog(sysdate + "  HIS入参|" + string.Join(",", objParam));
            try
            {
                CZYBH = CliUtils.fLoginUser;   // 操作员工号 
                YWZQH = CliUtils.fLoginYbNo.Split('|')[0].ToString();   // 业务周期号
                string jbr = CliUtils.fUserName;
                string jysj = Convert.ToDateTime(sysdate).ToString("yyyyMMddHHmmss");
                //交易流水号 
                string type = objParam[0].ToString();
                string parentValue = objParam[1].ToString();
                string admdvs = YBjyqh;
                string date = objParam[2].ToString();
                string valiFlag = "1";
                #region 入参
                string jzxxcxJson = string.Empty;
                dynamic data = new
                {
                    data = new
                    {
                        type = type,
                        parentValue = parentValue,
                        admdvs = admdvs,
                        date = date,
                        valiFlag = valiFlag
                    }
                };

                jzxxcxJson = JsonConvert.SerializeObject(data);
                #endregion
                string errstr = string.Empty;
                #region 就诊信息查询
                WriteLog(sysdate + "  字典表查询|入参Json|" + jzxxcxJson);
                int i = YBServiceRequest("1901", data, ref errstr);
                if (i > 0)
                {
                    string dataRs = errstr.ToString();
                    WriteLog(sysdate + "  字典表查询|出参Json|" + dataRs);

                    List<JObject> ds = JsonConvert.DeserializeObject<List<JObject>>(JObject.Parse(dataRs)["list"].ToString());
                    WriteLog(sysdate + "  字典表查询成功|" + dataRs);
                    return new object[] { 0, 1, ds };

                }
                else
                {

                    WriteLog(sysdate + "  字典表查询失败|" + errstr.ToString());
                    return new object[] { 0, 0, " 字典表查询失败|" + errstr.ToString() };
                }
                #endregion

            }
            catch (Exception ex)
            {
                WriteLog(sysdate + "  字典表查询异常|" + ex.Message);
                return new object[] { 0, 0, " 字典表查询异常|" + ex.Message };
            }
        }
        #endregion

        #region  人员慢特病用药记录查询
        public static object[] YBRYMTBYYJLCX(object[] objParam)
        {
            string sysdate = GetServerDateTime();//当前时间
            WriteLog(sysdate + "  进入人员慢特病用药记录查询...");
            WriteLog(sysdate + "  HIS入参|" + string.Join(",", objParam));
            try
            {
                CZYBH = CliUtils.fLoginUser;   // 操作员工号 
                YWZQH = CliUtils.fLoginYbNo.Split('|')[0].ToString();   // 业务周期号
                string jbr = CliUtils.fUserName;
                string jysj = Convert.ToDateTime(sysdate).ToString("yyyyMMddHHmmss");
                //交易流水号
                JYLSH = YBJGBH + jysj + new Random().Next(100).ToString().PadLeft(4, '0');
                string grbh = objParam[0].ToString();
                string begintime = objParam[1].ToString();
                string endtime = objParam[2].ToString();
                #region 入参
                string jzxxcxJson = string.Empty;
                dynamic data = new
                {
                    data = new
                    {
                        psn_no = grbh,
                        begntime = begintime,
                        endtime = endtime
                    }
                };

                jzxxcxJson = JsonConvert.SerializeObject(data);
                #endregion
                string errstr = string.Empty;
                #region 就诊信息查询
                WriteLog(sysdate + "  人员慢特病用药记录查询|入参Json|" + jzxxcxJson);
                int i = YBServiceRequest("5205", data, ref errstr);
                if (i > 0)
                {
                    string dataRs = errstr.ToString();
                    WriteLog(sysdate + "  人员慢特病用药记录查询|出参Json|" + dataRs);

                    List<JObject> ds = JsonConvert.DeserializeObject<List<JObject>>(JObject.Parse(dataRs)["feedetail"].ToString());
                    WriteLog(sysdate + "  人员慢特病用药记录查询成功|" + dataRs);
                    return new object[] { 0, 1, ds };

                }
                else
                {

                    WriteLog(sysdate + "  人员慢特病用药记录查询失败|" + errstr.ToString());
                    return new object[] { 0, 0, " 人员慢特病用药记录查询失败|" + errstr.ToString() };
                }
                #endregion

            }
            catch (Exception ex)
            {
                WriteLog(sysdate + "  人员慢特病用药记录查询异常|" + ex.Message);
                return new object[] { 0, 0, " 人员慢特病用药记录查询异常|" + ex.Message };
            }
        }
        #endregion

        #region  人员累计信息查询
        public static object[] YBRYLJXJCX(object[] objParam)
        {
            string sysdate = GetServerDateTime();//当前时间
            WriteLog(sysdate + "  进入人员累计信息查询...");
            WriteLog(sysdate + "  HIS入参|" + string.Join(",", objParam));
            try
            {
                CZYBH = CliUtils.fLoginUser;   // 操作员工号 
                YWZQH = CliUtils.fLoginYbNo.Split('|')[0].ToString();   // 业务周期号
                string jbr = CliUtils.fUserName;
                string jysj = Convert.ToDateTime(sysdate).ToString("yyyyMMddHHmmss");
                //交易流水号
                JYLSH = YBJGBH + jysj + new Random().Next(100).ToString().PadLeft(4, '0');
                string grbh = objParam[0].ToString();//个人编号
                string cum_ym = objParam[1].ToString();//累计年月

                #region 入参
                string jzxxcxJson = string.Empty;
                dynamic data = new
                {
                    data = new
                    {
                        psn_no = grbh,
                        cum_ym = cum_ym
                    }

                };

                jzxxcxJson = JsonConvert.SerializeObject(data);
                #endregion
                string errstr = string.Empty;
                #region 就诊信息查询
                WriteLog(sysdate + "  人员累计信息查询|入参Json|" + jzxxcxJson);
                int i = YBServiceRequest("5206", data, ref errstr);
                if (i > 0)
                {
                    string dataRs = errstr.ToString();
                    WriteLog(sysdate + "  人员累计信息查询|出参Json|" + dataRs);

                    List<JObject> ds = JsonConvert.DeserializeObject<List<JObject>>(JObject.Parse(dataRs)["cuminfo"].ToString());
                    WriteLog(sysdate + "  人员累计信息查询成功|" + dataRs);
                    return new object[] { 0, 1, ds };

                }
                else
                {

                    WriteLog(sysdate + "  人员累计信息查询失败|" + errstr.ToString());
                    return new object[] { 0, 0, " 人员累计信息查询失败|" + errstr.ToString() };
                }
                #endregion

            }
            catch (Exception ex)
            {
                WriteLog(sysdate + "  人员累计信息查询异常|" + ex.Message);
                return new object[] { 0, 0, " 人员累计信息查询异常|" + ex.Message };
            }
        }
        #endregion

        #region  人员慢特病备案查询
        public static object[] YBRYMTBBACX(object[] objParam)
        {
            string sysdate = GetServerDateTime();//当前时间
            WriteLog(sysdate + "  进入人员慢特病备案查询...");
            WriteLog(sysdate + "  HIS入参|" + string.Join(",", objParam));
            try
            {
                CZYBH = CliUtils.fLoginUser;   // 操作员工号 
                YWZQH = CliUtils.fLoginYbNo.Split('|')[0].ToString();   // 业务周期号
                string jbr = CliUtils.fUserName;
                string jysj = Convert.ToDateTime(sysdate).ToString("yyyyMMddHHmmss");
                //交易流水号
                JYLSH = YBJGBH + jysj + new Random().Next(100).ToString().PadLeft(4, '0');
                string grbh = objParam[0].ToString();//个人编号 

                #region 入参
                string jzxxcxJson = string.Empty;
                dynamic data = new
                {
                    data = new
                    {
                        psn_no = grbh
                    }
                };

                jzxxcxJson = JsonConvert.SerializeObject(data);
                #endregion
                string errstr = string.Empty;
                #region 就诊信息查询
                WriteLog(sysdate + "  人员慢特病备案查询|入参Json|" + jzxxcxJson);
                int i = YBServiceRequest("5301", data, ref errstr);
                if (i > 0)
                {
                    string dataRs = errstr.ToString();
                    WriteLog(sysdate + "  人员慢特病备案查询|出参Json|" + dataRs);

                    List<JObject> ds = JsonConvert.DeserializeObject<List<JObject>>(JsonConvert.DeserializeObject<JObject>(dataRs)["feedetail"].ToString());
                    WriteLog(sysdate + "  人员慢特病备案查询成功|" + dataRs);
                    return new object[] { 0, 1, ds };

                }
                else
                {

                    WriteLog(sysdate + "  人员慢特病备案查询失败|" + errstr.ToString());
                    return new object[] { 0, 0, " 人员慢特病备案查询失败|" + errstr.ToString() };
                }
                #endregion

            }
            catch (Exception ex)
            {
                WriteLog(sysdate + "  人员慢特病备案查询异常|" + ex.Message);
                return new object[] { 0, 0, " 人员慢特病备案查询异常|" + ex.Message };
            }
        }
        #endregion

        #region  人员定点信息查询
        public static object[] YBRYDDXXCX(object[] objParam)
        {
            string sysdate = GetServerDateTime();//当前时间
            WriteLog(sysdate + "  进入人员定点信息查询...");
            WriteLog(sysdate + "  HIS入参|" + string.Join(",", objParam));
            try
            {
                CZYBH = CliUtils.fLoginUser;   // 操作员工号 
                YWZQH = CliUtils.fLoginYbNo.Split('|')[0].ToString();   // 业务周期号
                string jbr = CliUtils.fUserName;
                string jysj = Convert.ToDateTime(sysdate).ToString("yyyyMMddHHmmss");
                //交易流水号
                JYLSH = YBJGBH + jysj + new Random().Next(100).ToString().PadLeft(4, '0');
                string grbh = objParam[0].ToString();//个人编号 
                string biz_appy_type = objParam[1].ToString();//业务申请类型

                #region 入参
                string jzxxcxJson = string.Empty;
                dynamic data = new
                {
                    data = new
                    {
                        psn_no = grbh,
                        biz_appy_type = biz_appy_type
                    }
                };

                jzxxcxJson = JsonConvert.SerializeObject(data);
                #endregion
                string errstr = string.Empty;
                #region 就诊信息查询
                WriteLog(sysdate + "  人员定点信息查询|入参Json|" + jzxxcxJson);
                int i = YBServiceRequest("5302", data, ref errstr);
                if (i > 0)
                {
                    string dataRs = errstr.ToString();
                    WriteLog(sysdate + "  人员定点信息查询|出参Json|" + dataRs);

                    List<JObject> ds = JsonConvert.DeserializeObject<List<JObject>>(JObject.Parse(dataRs)["psnfixmedin"].ToString());
                    WriteLog(sysdate + "  人员定点信息查询成功|" + dataRs);
                    return new object[] { 0, 1, ds };

                }
                else
                {

                    WriteLog(sysdate + "  人员定点信息查询失败|" + errstr.ToString());
                    return new object[] { 0, 0, " 人员定点信息查询失败|" + errstr.ToString() };
                }
                #endregion

            }
            catch (Exception ex)
            {
                WriteLog(sysdate + "  人员定点信息查询异常|" + ex.Message);
                return new object[] { 0, 0, " 人员定点信息查询异常|" + ex.Message };
            }
        }
        #endregion

        #region 结算信息查询
        public static object[] YBJSXXCX(object[] objParam)
        {
            string sysdate = GetServerDateTime();//当前时间
            WriteLog(sysdate + "  进入结算信息查询...");
            WriteLog(sysdate + "  HIS入参|" + string.Join(",", objParam));
            try
            {
                CZYBH = CliUtils.fLoginUser;   // 操作员工号 
                YWZQH = CliUtils.fLoginYbNo.Split('|')[0].ToString();   // 业务周期号
                string jbr = CliUtils.fUserName;
                string jysj = Convert.ToDateTime(sysdate).ToString("yyyyMMddHHmmss");
                //交易流水号
                JYLSH = YBJGBH + jysj + new Random().Next(100).ToString().PadLeft(4, '0');
                string grbh = objParam[0].ToString();
                string jsid = objParam[1].ToString();
                string jzid = objParam[2].ToString();
                #region 入参
                string jsxxcxJson = string.Empty;
                dynamic data = new
                {
                    data = new
                    {
                        psn_no = grbh,
                        setl_id = jsid,
                        mdtrt_id = jzid

                    }
                };

                jsxxcxJson = JsonConvert.SerializeObject(data);
                #endregion
                string errstr = string.Empty;
                #region 结算信息查询
                WriteLog(sysdate + "  结算信息查询|入参Json|" + jsxxcxJson);
                int i = YBServiceRequest("5203", data, ref errstr);
                if (i > 0)
                {
                    string dataRs = errstr.ToString();
                    WriteLog(sysdate + "  结算信息查询|出参Json|" + dataRs);
                    List<JObject> ds = JsonConvert.DeserializeObject<List<JObject>>(JObject.Parse(dataRs)["setldetail"].ToString());
                    WriteLog(sysdate + "  结算信息查询成功|" + dataRs);
                    return new object[] { 0, 1, ds };

                }
                else
                {

                    WriteLog(sysdate + "  结算信息查询失败|" + errstr.ToString());
                    return new object[] { 0, 0, " 结算信息查询失败|" + errstr.ToString() };
                }
                #endregion

            }
            catch (Exception ex)
            {
                WriteLog(sysdate + "  结算信息查询异常|" + ex.Message);
                return new object[] { 0, 0, " 结算信息查询异常|" + ex.Message };
            }
        }

        #endregion

        #region 转院信息查询
        public static object[] YBZHYXXCX(object[] objParam)
        {
            string sysdate = GetServerDateTime();//当前时间
            WriteLog(sysdate + "  进入转院信息查询...");
            WriteLog(sysdate + "  HIS入参|" + string.Join(",", objParam));
            try
            {
                CZYBH = CliUtils.fLoginUser;   // 操作员工号 
                YWZQH = CliUtils.fLoginYbNo.Split('|')[0].ToString();   // 业务周期号
                string jbr = CliUtils.fUserName;
                string jysj = Convert.ToDateTime(sysdate).ToString("yyyyMMddHHmmss");
                //交易流水号
                JYLSH = YBJGBH + jysj + new Random().Next(100).ToString().PadLeft(4, '0');
                string grbh = objParam[0].ToString();
                string kssj = objParam[1].ToString();
                string jssj = objParam[2].ToString();
                #region 入参
                string zhyxxcxJson = string.Empty;
                dynamic data = new
                {
                    data = new
                    {
                        psn_no = grbh,
                        begntime = kssj,
                        endtime = jssj
                    }

                };

                zhyxxcxJson = JsonConvert.SerializeObject(data);
                #endregion
                string errstr = "";
                #region 转院信息查询
                WriteLog(sysdate + "  转院信息查询|入参Json|" + zhyxxcxJson);
                int i = YBServiceRequest("5304", data, ref errstr);
                if (i > 0)
                {
                    string dataRs = errstr.ToString();
                    WriteLog(sysdate + "  转院信息查询|出参Json|" + dataRs);

                    List<JObject> ds = JsonConvert.DeserializeObject<List<JObject>>(JObject.Parse(dataRs)["refmedin"].ToString());
                    WriteLog(sysdate + "  转院信息查询成功|" + dataRs);
                    return new object[] { 0, 1, ds };

                }
                else
                {

                    WriteLog(sysdate + "  转院信息查询失败|" + errstr.ToString());
                    return new object[] { 0, 0, " 转院信息查询失败|" + errstr.ToString() };
                }
                #endregion

            }
            catch (Exception ex)
            {
                WriteLog(sysdate + "  转院信息查询异常|" + ex.Message);
                return new object[] { 0, 0, " 转院信息查询异常|" + ex.Message };
            }
        }

        #endregion

        #region 项目互认信息查询
        public static object[] YBXMHRXXCX(object[] objParam)
        {
            string sysdate = GetServerDateTime();//当前时间
            WriteLog(sysdate + "  进入项目互认信息查询...");
            WriteLog(sysdate + "  HIS入参|" + string.Join(",", objParam));
            try
            {
                CZYBH = CliUtils.fLoginUser;   // 操作员工号 
                YWZQH = CliUtils.fLoginYbNo.Split('|')[0].ToString();   // 业务周期号
                string jbr = CliUtils.fUserName;
                string jysj = Convert.ToDateTime(sysdate).ToString("yyyyMMddHHmmss");
                //交易流水号
                JYLSH = YBJGBH + jysj + new Random().Next(100).ToString().PadLeft(4, '0');
                string grbh = objParam[0].ToString();
                string jcjgdm = objParam[1].ToString();
                string jcjgmc = objParam[2].ToString();
                string jcxmdm = objParam[3].ToString();
                string jcxmmc = objParam[4].ToString();
                #region 入参
                string xmhrxxcxJson = string.Empty;
                dynamic bilgiteminfo = new
                {
                    data = new
                    {
                        psn_no = grbh,
                        exam_org_code = jcjgdm,
                        exam_org_name = jcjgmc,
                        exam_item_code = jcxmdm,
                        exam_item_name = jcxmmc
                    }

                };

                xmhrxxcxJson = JsonConvert.SerializeObject(bilgiteminfo);
                #endregion
                string errstr = string.Empty;
                #region 项目互认信息查询
                WriteLog(sysdate + "  项目互认信息查询|入参Json|" + xmhrxxcxJson);
                int i = YBServiceRequest("5401", bilgiteminfo, ref errstr);
                if (i > 0)
                {
                    string dataRs = errstr.ToString();
                    WriteLog(sysdate + "  项目互认信息查询|出参Json|" + dataRs);

                    DataSet ds = JsonConvert.DeserializeObject<DataSet>(dataRs);
                    WriteLog(sysdate + "  项目互认信息查询成功|" + dataRs);
                    return new object[] { 0, 1, ds };

                }
                else
                {

                    WriteLog(sysdate + "  项目互认信息查询失败|" + errstr.ToString());
                    return new object[] { 0, 0, " 项目互认信息查询失败|" + errstr.ToString() };
                }
                #endregion

            }
            catch (Exception ex)
            {
                WriteLog(sysdate + "  项目互认信息查询异常|" + ex.Message);
                return new object[] { 0, 0, " 项目互认信息查询异常|" + ex.Message };
            }
        }

        #endregion

        #region 报告明细信息查询
        public static object[] YBBGMXXXCX(object[] objParam)
        {
            string sysdate = GetServerDateTime();//当前时间
            WriteLog(sysdate + "  进入报告明细信息查询...");
            WriteLog(sysdate + "  HIS入参|" + string.Join(",", objParam));
            try
            {
                CZYBH = CliUtils.fLoginUser;   // 操作员工号 
                YWZQH = CliUtils.fLoginYbNo.Split('|')[0].ToString();   // 业务周期号
                string jbr = CliUtils.fUserName;
                string jysj = Convert.ToDateTime(sysdate).ToString("yyyyMMddHHmmss");
                //交易流水号
                JYLSH = YBJGBH + jysj + new Random().Next(100).ToString().PadLeft(4, '0');
                string grbh = objParam[0].ToString();
                string bgdh = objParam[1].ToString();
                string jgbh = objParam[2].ToString();
                #region 入参
                string bgmxxxcxJson = string.Empty;
                dynamic rptdetailinfo = new
                {
                    data = new
                    {
                        psn_no = grbh,
                        rpotc_no = bgdh,
                        fixmedins_code = jgbh
                    }

                };
                bgmxxxcxJson = JsonConvert.SerializeObject(rptdetailinfo);
                #endregion
                string errstr = string.Empty;
                #region 报告明细信息查询
                WriteLog(sysdate + "  报告明细信息查询|入参Json|" + bgmxxxcxJson);
                int i = YBServiceRequest("5402", rptdetailinfo, ref errstr);
                if (i > 0)
                {
                    string dataRs = errstr.ToString();
                    WriteLog(sysdate + "  报告明细信息查询|出参Json|" + dataRs);
                    JObject jobj = JsonConvert.DeserializeObject(dataRs) as JObject; //获
                                                                                     //获取返回json转为dataset
                    DataSet ds = new DataSet();
                    string checkreportdetailsRs = jobj["checkreportdetails"].ToString();
                    DataSet ds1 = JsonConvert.DeserializeObject<DataSet>(checkreportdetailsRs);
                    ds.Tables.Add(ds1.Tables[0]);
                    string inspectionreportinformationRs = jobj["inspectionreportinformation"].ToString();
                    DataSet ds2 = JsonConvert.DeserializeObject<DataSet>(inspectionreportinformationRs);
                    ds.Tables.Add(ds2.Tables[0]);
                    string inspectiondetailsRs = jobj["inspectiondetails"].ToString();
                    DataSet ds3 = JsonConvert.DeserializeObject<DataSet>(inspectiondetailsRs);
                    ds.Tables.Add(ds3.Tables[0]);
                    ds1.Dispose();
                    ds2.Dispose();
                    ds3.Dispose();
                    WriteLog(sysdate + "  报告明细信息查询成功|" + checkreportdetailsRs);
                    return new object[] { 0, 1, ds };

                }
                else
                {

                    WriteLog(sysdate + "  报告明细信息查询失败|" + errstr.ToString());
                    return new object[] { 0, 0, " 报告明细信息查询失败|" + errstr.ToString() };
                }
                #endregion

            }
            catch (Exception ex)
            {
                WriteLog(sysdate + "  报告明细信息查询异常|" + ex.Message);
                return new object[] { 0, 0, " 报告明细信息查询异常|" + ex.Message };
            }
        }

        #endregion

        #region 人员慢特病备案
        public static object[] YBRYMBBA(object[] objParam)
        {
            string sysdate = GetServerDateTime();
            WriteLog(sysdate + "  进入人员慢特病备案...");
            WriteLog("进入人员慢特病备案|HIS传参|" + string.Join(",", objParam));
            try
            {
                CZYBH = CliUtils.fLoginUser; //操作员工号
                string jbr = CliUtils.fUserName;
                YWZQH = CliUtils.fLoginYbNo.Split('|')[0].ToString();    //业务周期号
                string jysj = Convert.ToDateTime(sysdate).ToString("yyyyMMddHHmmss");
                //交易流水号
                JYLSH = YBJGBH + jysj + new Random().Next(100).ToString().PadLeft(4, '0');
                string grbh = objParam[0].ToString();
                string xzlx = objParam[1].ToString();
                string mxbbzdm = objParam[2].ToString();
                string mxbbzmc = objParam[3].ToString();
                string lxdh = objParam[4].ToString();
                string lxdz = objParam[5].ToString();
                string tcqh = objParam[6].ToString();
                string ddyljgbh = objParam[7].ToString();
                string ddyljgmc = objParam[8].ToString();
                string yyjdrq = objParam[9].ToString();
                string zdysbm = objParam[10].ToString();
                string zdysxm = objParam[11].ToString();
                string ksrq = objParam[12].ToString();
                string jsrq = objParam[13].ToString();

                string xm = CardInfo.baseinfo.psn_name;
                string idcard = CardInfo.baseinfo.certno;

                //if (!string.IsNullOrEmpty(yyjdrq))
                //    yyjdrq = yyjdrq.Substring(0, 4) + "-" + yyjdrq.Substring(4, 2) + "-" + yyjdrq.Substring(6, 2);
                //if (!string.IsNullOrEmpty(ksrq))
                //    ksrq = ksrq.Substring(0, 4) + "-" + ksrq.Substring(4, 2) + "-" + ksrq.Substring(6, 2);
                //if (!string.IsNullOrEmpty(jsrq))
                //    jsrq = jsrq.Substring(0, 4) + "-" + jsrq.Substring(4, 2) + "-" + jsrq.Substring(6, 2);

                #region 入参
                string rymbbaJson = string.Empty;
                dynamic data = new
                {
                    data = new
                    {
                        psn_no = grbh,
                        insutype = xzlx,
                        opsp_dise_code = mxbbzdm,
                        opsp_dise_name = mxbbzmc,
                        tel = lxdh,
                        addr = lxdz,
                        insu_optins = tcqh,
                        ide_fixmedins_no = ddyljgbh,
                        ide_fixmedins_name = ddyljgmc,
                        hosp_ide_date = yyjdrq,
                        diag_dr_codg = zdysbm,
                        diag_dr_name = zdysxm,
                        begndate = ksrq,
                        enddate = jsrq
                    }

                };
                string Err = string.Empty;
                rymbbaJson = JsonConvert.SerializeObject(data);
                #endregion

                #region 人员慢特病备案
                WriteLog(sysdate + "  人员慢特病备案|入参Json|" + rymbbaJson);
                int i = YBServiceRequest("2503", data, ref Err);
                if (i == 1)
                {
                    List<string> liSQL = new List<string>();
                    string dataRs = Err.ToString();
                    WriteLog(sysdate + "  人员慢特病备案|出参Json|" + dataRs);
                    JObject jobj = JsonConvert.DeserializeObject(dataRs) as JObject; //
                    string dysblsh = jobj["result"]["trt_dcla_detl_sn"].ToString();
                    #region 数据操作
                    string strSql = string.Format(@"INSERT INTO [dbo].[ybzyba]
                       ([dysblsh],[grbh],[xzlx],[lxdh] ,[lxdz],[tcqh],[zddm],[zdmc],[jbbqms],[zwddyljgbh],[zwddyljgmc],
                  [jytcqh],[yytyzybz],[zylx],[zyrq],[zyyy],[zyyj],[ksrq],[jsrq],[cxbz],[sysdate],[balx],[jbr],[ddyljgbm],[ddyljgmc],[yyjdrq],[zdysbm],[zdysxm],[xm],[sfzh])
                        VALUES
                       ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}'
                       ,'{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}','{20}','{21}','{22}',
                         '{23}','{24}','{25}','{26}','{27}','{28}','{29}')",
                         dysblsh, grbh, xzlx, lxdh, lxdz, tcqh, mxbbzdm, mxbbzmc, "", "", "",
                         "", "", "", "", "", "", ksrq, jsrq, "1", sysdate, "MB", CZYBH,
                         ddyljgbh, ddyljgmc, yyjdrq, zdysbm, zdysxm, xm, idcard);
                    liSQL.Add(strSql);
                    object[] objData = liSQL.ToArray();
                    objData = CliUtils.CallMethod("sybdj", "BatExecuteSql", objData);
                    if (objData[1].ToString().Equals("1"))
                    {
                        WriteLog(sysdate + "  人员慢特病备案成功|数据库操作成功|");
                        return new object[] { 0, 1, "  人员慢特病备案成功|数据库操作成功|" };
                    }
                    else
                    {
                        WriteLog(sysdate + "  人员慢特病备案成功|数据库操作失败|" + objData[2].ToString());
                        return new object[] { 0, 0, "  人员慢特病备案成功|数据库操作失败|" + objData[2].ToString() };
                    }
                    #endregion


                }
                else
                {
                    WriteLog(sysdate + " 人员慢特病备案失败|" + Err.ToString());
                    return new object[] { 0, 0, " 人员慢特病备案失败|" + Err.ToString() };
                }
                #endregion

            }
            catch (Exception ex)
            {
                WriteLog(sysdate + " 人员慢特病备案异常|" + ex.Message);
                return new object[] { 0, 0, " 人员慢特病备案异常|" + ex.Message };
            }
        }
        #endregion

        #region 人员慢病备案撤销
        public static object[] YBRYMBBACX(object[] objParam)
        {
            string sysdate = GetServerDateTime();
            WriteLog(sysdate + "  进入人员慢病备案撤销...");
            WriteLog("进入人员慢病备案撤销|HIS传参|" + string.Join(",", objParam));
            try
            {
                string dysblsh = objParam[0].ToString();//待遇申报流水号
                string grbh = objParam[1].ToString();//人员编号
                string cxyy = "数据填写错误";//撤销原因
                CZYBH = CliUtils.fLoginUser; //操作员工号
                string jbr = CliUtils.fUserName;
                YWZQH = CliUtils.fLoginYbNo.Split('|')[0].ToString();    //业务周期号
                string jysj = Convert.ToDateTime(sysdate).ToString("yyyyMMddHHmmss");
                //交易流水号
                JYLSH = YBJGBH + jysj + new Random().Next(100).ToString().PadLeft(4, '0');
                #region 入参
                string rymbbaJson = string.Empty;
                dynamic data = new
                {
                    data = new
                    {
                        trt_dcla_detl_sn = dysblsh,
                        psn_no = grbh,
                        memo = cxyy
                    }
                };
                string Err = string.Empty;
                rymbbaJson = JsonConvert.SerializeObject(data);
                #endregion

                #region 人员慢病备案撤销
                List<string> liSQL = new List<string>();
                WriteLog(sysdate + "  人员慢病备案撤销|入参Json|" + rymbbaJson);
                int i = YBServiceRequest("2504", data, ref Err);
                if (i == 1)
                {
                    string dataRs = Err.ToString();
                    WriteLog(sysdate + "  人员慢病备案撤销|出参Json|" + dataRs);
                    JObject jobj = JsonConvert.DeserializeObject(dataRs) as JObject; //获取响应数据json

                    #region 数据操作
                    string strSql = string.Format(@"INSERT INTO [dbo].[ybzyba]
                       ([dysblsh],[grbh],[xzlx],[lxdh],[lxdz],[tcqh],[zddm],[zdmc],[jbbqms],[zwddyljgbh],[zwddyljgmc],
                        [jytcqh],[yytyzybz],[zylx],[zyrq],[zyyy],[zyyj],[ksrq],[jsrq],[cxbz],[sysdate],[balx],[jbr],
                        [ddyljgbm],[ddyljgmc],[yyjdrq],[zdysbm],[zdysxm])
                        select [dysblsh],[grbh],[xzlx],[lxdh],[lxdz],[tcqh],[zddm],[zdmc],[jbbqms],[zwddyljgbh],[zwddyljgmc],
                        [jytcqh],[yytyzybz],[zylx],[zyrq],[zyyy],[zyyj],[ksrq],[jsrq],'0','{0}',[balx],'{1}',
                        [ddyljgbm],[ddyljgmc],[yyjdrq],[zdysbm],[zdysxm] from ybzyba where cxbz=1 and balx='MB' and dysblsh='{2}'", sysdate, CZYBH, dysblsh);
                    liSQL.Add(strSql);
                    strSql = string.Format("update ybzyba set cxbz=2 where cxbz=1 and balx='MB' and dysblsh='{0}'", dysblsh);
                    liSQL.Add(strSql);
                    object[] objData = liSQL.ToArray();
                    objData = CliUtils.CallMethod("sybdj", "BatExecuteSql", objData);
                    if (objData[1].ToString().Equals("1"))
                    {
                        WriteLog(sysdate + "  人员慢病备案撤销成功|数据库操作成功|");
                        return new object[] { 0, 1, "  人员慢病备案撤销成功|数据库操作成功|" };
                    }
                    else
                    {
                        WriteLog(sysdate + "  人员慢病备案撤销成功|数据库操作失败|");
                        return new object[] { 0, 0, "  人员慢病备案撤销成功|数据库操作失败|" + objData[2].ToString() };
                    }
                    #endregion
                }
                else
                {
                    WriteLog(sysdate + "   人员慢病备案撤销失败|" + Err.ToString());
                    return new object[] { 0, 0, "   人员慢病备案撤销失败|" + Err };
                }
                #endregion


            }
            catch (Exception ex)
            {
                WriteLog(sysdate + " 人员慢病备案撤销异常|" + ex.Message);
                return new object[] { 0, 0, " 人员慢病备案撤销异常|" + ex.Message };
            }
        }

        #endregion

        #region 人员定点备案
        public static object[] YBRYDDBA(object[] objParam)
        {
            string sysdate = GetServerDateTime();
            WriteLog(sysdate + "  进入人员定点病备案...");
            WriteLog("进入人员定点备案|HIS传参|" + string.Join(",", objParam));
            try
            {
                CZYBH = CliUtils.fLoginUser; //操作员工号
                string jbr = CliUtils.fUserName;
                YWZQH = CliUtils.fLoginYbNo.Split('|')[0].ToString();    //业务周期号
                string jysj = Convert.ToDateTime(sysdate).ToString("yyyyMMddHHmmss");
                //交易流水号
                JYLSH = YBJGBH + jysj + new Random().Next(100).ToString().PadLeft(4, '0');
                string grbh = objParam[0].ToString();
                string lxdh = objParam[1].ToString();
                string lxdz = objParam[2].ToString();
                string ywsqlx = objParam[3].ToString();
                string ksrq = objParam[4].ToString();
                string jsrq = objParam[5].ToString();
                string dbrxm = objParam[6].ToString();
                string dbrzjlx = objParam[7].ToString();
                string dbrzjhm = objParam[8].ToString();
                string dbrlxfs = objParam[9].ToString();
                string dbrlxdz = objParam[10].ToString();
                string dbrgx = objParam[11].ToString();
                string ddpxh = objParam[12].ToString();
                string ddyljgbh = objParam[13].ToString();
                string ddyljgmc = objParam[14].ToString();
                string bz = objParam[15].ToString();

                //if (!string.IsNullOrEmpty(ksrq))
                //    ksrq = ksrq.Substring(0, 4) + "-" + ksrq.Substring(4, 2) + "-" + ksrq.Substring(6, 2);
                //if (!string.IsNullOrEmpty(jsrq))
                //    jsrq = jsrq.Substring(0, 4) + "-" + jsrq.Substring(4, 2) + "-" + jsrq.Substring(6, 2);

                #region 入参
                string ryddbaJson = string.Empty;
                dynamic data = new
                {
                    data = new
                    {
                        psn_no = grbh,
                        tel = lxdh,
                        addr = lxdz,
                        biz_appy_type = ywsqlx,
                        begndate = ksrq,
                        enddate = jsrq,
                        agnter_name = dbrxm,
                        agnter_cert_type = dbrzjlx,
                        agnter_certno = dbrzjhm,
                        agnter_tel = dbrlxfs,
                        agnter_addr = dbrlxdz,
                        agnter_rlts = dbrgx,
                        fix_srt_no = ddpxh,
                        fixmedins_code = ddyljgbh,
                        fixmedins_name = ddyljgmc,
                        memo = bz
                    }
                };
                string Err = string.Empty;
                ryddbaJson = JsonConvert.SerializeObject(data);
                #endregion

                #region 人员定点备案
                WriteLog(sysdate + "  人员定点备案|入参Json|" + ryddbaJson);
                int i = YBServiceRequest("2505", data, ref Err);
                if (i == 1)
                {
                    List<string> liSQL = new List<string>();
                    string dataRs = Err.ToString();
                    WriteLog(sysdate + "  人员定点备案|出参Json|" + dataRs);
                    JObject jobj = JsonConvert.DeserializeObject(dataRs) as JObject; //获取响应数据json

                    string dysblsh = jobj["result"]["trt_dcla_detl_sn"].ToString();
                    #region 数据操作
                    string strSql = string.Format(@"INSERT INTO [dbo].[ybzyba]
                       ([dysblsh],[grbh],[xzlx],[lxdh] ,[lxdz],[tcqh],[zddm],[zdmc],[jbbqms],[zwddyljgbh],[zwddyljgmc],
                  [jytcqh],[yytyzybz],[zylx],[zyrq],[zyyy],[zyyj],[ksrq],[jsrq],[cxbz],[sysdate],[balx],[jbr],[ddyljgbm],[ddyljgmc],[yyjdrq],[zdysbm],[zdysxm],[ywsqlx],[dbrxm],[dbrzjlx],[dbrzjhm],[dbrlxdh],[dbrlxdz],[dbrgx],[ddpxh],[bz])
                        VALUES
                       ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}'
                       ,'{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}','{20}','{21}','{22}', '{23}','{24}','{25}','{26}','{27}','{28}','{29}','{30}','{31}','{32}','{33}','{34}','{35}','{36}')",
                         dysblsh, grbh, "", lxdh, lxdz, "", "", "", "", "", "",
                         "", "", "", "", "", "", ksrq, jsrq, "1", sysdate, "DD", CZYBH,
                         ddyljgbh, ddyljgmc, "", "", "", ywsqlx, dbrxm, dbrzjlx, dbrzjhm, dbrlxfs, dbrlxdz, dbrgx, ddpxh, bz);
                    liSQL.Add(strSql);
                    object[] objData = liSQL.ToArray();
                    objData = CliUtils.CallMethod("sybdj", "BatExecuteSql", objData);
                    if (objData[1].ToString().Equals("1"))
                    {
                        WriteLog(sysdate + "  人员定点备案成功|数据库操作成功|");
                        return new object[] { 0, 1, "  人员定点备案成功|数据库操作成功|" };
                    }
                    else
                    {
                        WriteLog(sysdate + "  人员定点备案成功|数据库操作失败|" + strSql);
                        return new object[] { 0, 0, "  人员定点备案成功|数据库操作失败|" + objData[2].ToString() };
                    }
                    #endregion
                }
                else
                {
                    WriteLog(sysdate + " 人员定点备案失败|" + Err.ToString());
                    return new object[] { 0, 0, " 人员定点备案失败|" + Err.ToString() };
                }
                #endregion

            }
            catch (Exception ex)
            {
                WriteLog(sysdate + " 人员定点备案异常|" + ex.Message);
                return new object[] { 0, 0, " 人员定点备案异常|" + ex.Message };
            }
        }
        #endregion

        #region 人员定点备案撤销
        public static object[] YBRYDDBACX(object[] objParam)
        {
            string sysdate = GetServerDateTime();
            WriteLog(sysdate + "  进入人员定点备案撤销...");
            WriteLog("进入人员定点备案撤销|HIS传参|" + string.Join(",", objParam));
            try
            {
                string dysblsh = objParam[0].ToString();//待遇申报流水号
                string grbh = objParam[1].ToString();//人员编号
                string cxyy = "数据填写错误";//撤销原因
                CZYBH = CliUtils.fLoginUser; //操作员工号
                string jbr = CliUtils.fUserName;
                YWZQH = CliUtils.fLoginYbNo.Split('|')[0].ToString();    //业务周期号
                string jysj = Convert.ToDateTime(sysdate).ToString("yyyyMMddHHmmss");
                //交易流水号
                JYLSH = YBJGBH + jysj + new Random().Next(100).ToString().PadLeft(4, '0');
                #region 入参
                string rymbbaJson = string.Empty;
                dynamic data = new
                {
                    data = new
                    {
                        trt_dcla_detl_sn = dysblsh,
                        psn_no = grbh,
                        memo = cxyy
                    }
                };
                string Err = string.Empty;
                rymbbaJson = JsonConvert.SerializeObject(data);
                #endregion

                #region 人员定点备案撤销
                List<string> liSQL = new List<string>();
                WriteLog(sysdate + "  人员定点备案撤销|入参Json|" + rymbbaJson);
                int i = YBServiceRequest("2506", data, ref Err);
                if (i == 1)
                {
                    string dataRs = Err.ToString();
                    WriteLog(sysdate + "  人员定点备案撤销|出参Json|" + dataRs);
                    JObject jobj = JsonConvert.DeserializeObject(dataRs) as JObject; //获取响应数据json

                    #region 数据操作
                    string strSql = string.Format(@"INSERT INTO [dbo].[ybzyba]
                       ([dysblsh],[grbh],[xzlx],[lxdh],[lxdz],[tcqh],[zddm],[zdmc],[jbbqms],[zwddyljgbh],[zwddyljgmc],
                        [jytcqh],[yytyzybz],[zylx],[zyrq],[zyyy],[zyyj],[ksrq],[jsrq],[cxbz],[sysdate],[balx],[jbr],
                        [ddyljgbm],[ddyljgmc],[yyjdrq],[zdysbm],[zdysxm],[ywsqlx],[dbrxm],[dbrzjlx],[dbrzjhm],[dbrlxdh],[dbrlxdz],[dbrgx],[ddpxh],[bz])
                        select [dysblsh],[grbh],[xzlx],[lxdh],[lxdz],[tcqh],[zddm],[zdmc],[jbbqms],[zwddyljgbh],[zwddyljgmc],
                        [jytcqh],[yytyzybz],[zylx],[zyrq],[zyyy],[zyyj],[ksrq],[jsrq],'0','{0}',[balx],'{1}',
                        [ddyljgbm],[ddyljgmc],[yyjdrq],[zdysbm],[zdysxm],[ywsqlx],[dbrxm],[dbrzjlx],[dbrzjhm],[dbrlxdh],[dbrlxdz],[dbrgx],[ddpxh],[bz] from ybzyba where cxbz=1 and balx='DD' and dysblsh='{2}'", sysdate, CZYBH, dysblsh);
                    liSQL.Add(strSql);
                    strSql = string.Format("update ybzyba set cxbz=2 where cxbz=1 and balx='DD' and dysblsh='{0}'", dysblsh);
                    liSQL.Add(strSql);
                    object[] objData = liSQL.ToArray();
                    objData = CliUtils.CallMethod("sybdj", "BatExecuteSql", objData);
                    if (objData[1].ToString().Equals("1"))
                    {
                        WriteLog(sysdate + "  人员定点备案撤销成功|数据库操作成功|");
                        return new object[] { 0, 1, "  人员定点备案撤销成功|数据库操作成功|" };
                    }
                    else
                    {
                        WriteLog(sysdate + "  人员定点备案撤销成功|数据库操作失败|" + strSql.ToString());
                        return new object[] { 0, 0, "  人员定点备案撤销成功|数据库操作失败|" + strSql.ToString() };
                    }
                    #endregion
                }
                else
                {
                    WriteLog(sysdate + "   人员定点备案撤销失败|" + Err.ToString());
                    return new object[] { 0, 0, "   人员定点备案撤销失败|" + Err.ToString() };
                }
                #endregion


            }
            catch (Exception ex)
            {
                WriteLog(sysdate + " 人员定点备案撤销异常|" + ex.Message);
                return new object[] { 0, 0, " 人员定点备案撤销异常|" + ex.Message };
            }
        }
        #endregion

        #region 重特大疾病备案登记
        public static object[] YBZDJBBADJ(object[] objParam)
        {
            string sysdate = GetServerDateTime();
            WriteLog(sysdate + "  进入重特大疾病备案登记...");
            WriteLog("进入重特大疾病备案登记|HIS传参|" + string.Join(",", objParam));
            try
            {
                if (CardInfo == null)
                {
                    return new object[] { 0, 0, "请先进行读卡！" };
                }
                CZYBH = CliUtils.fLoginUser; //操作员工号
                string jbr = CliUtils.fUserName;
                YWZQH = CliUtils.fLoginYbNo.Split('|')[0].ToString();    //业务周期号
                string jysj = Convert.ToDateTime(sysdate).ToString("yyyyMMddHHmmss");

                string evt_type = objParam[0].ToString();
                string dcla_souc = objParam[1].ToString();
                string bydise_setl_list_code = objParam[2].ToString();
                string bydise_setl_dise_name = objParam[3].ToString();
                string oprn_oprt_code = objParam[4].ToString();
                string oprn_oprt_name = objParam[5].ToString();
                string fixmedins_code = YBJGBH;
                string fix_blng_admdv = YBjyqh;
                string appy_date = objParam[6].ToString();
                string appy_rea = objParam[7].ToString();
                string agnter_name = objParam[8].ToString();
                string agnter_cert_type = objParam[9].ToString();
                string agnter_certno = objParam[10].ToString();
                string agnter_tel = objParam[11].ToString();
                string agnter_addr = objParam[12].ToString();
                string agnter_rlts = objParam[13].ToString();
                string begndate = objParam[14].ToString();
                string enddate = objParam[15].ToString();
                string memo = objParam[16].ToString();
                string psn_no = CardInfo.baseinfo.psn_no;
                string expContent = "";
                #region 入参
                string ryddbaJson = string.Empty;
                dynamic data = new
                {
                    data = new
                    {
                        evt_type = evt_type,
                        dcla_souc = dcla_souc,
                        bydise_setl_list_code = bydise_setl_list_code,
                        bydise_setl_dise_name = bydise_setl_dise_name,
                        oprn_oprt_code = oprn_oprt_code,
                        oprn_oprt_name = oprn_oprt_name,
                        fixmedins_code = fixmedins_code,
                        fix_blng_admdv = fix_blng_admdv,
                        appy_date = appy_date,
                        appy_rea = appy_rea,
                        agnter_name = agnter_name,
                        agnter_cert_type = agnter_cert_type,
                        agnter_certno = agnter_certno,
                        agnter_tel = agnter_tel,
                        agnter_addr = agnter_addr,
                        agnter_rlts = agnter_rlts,
                        begndate = begndate,
                        enddate = enddate,
                        memo = memo,
                        psn_no = psn_no,
                        expContent = expContent

                    }
                };

                #endregion
                string Err = string.Empty;
                ryddbaJson = JsonConvert.SerializeObject(data);
                #region 人员定点备案
                WriteLog(sysdate + "  重特大疾病备案登记|入参Json|" + ryddbaJson);
                int i = YBServiceRequest("2585", data, ref Err);
                if (i == 1)
                {
                    List<string> liSQL = new List<string>();
                    string dataRs = Err.ToString();
                    WriteLog(sysdate + "  重特大疾病备案登记|出参Json|" + dataRs);
                    JObject jobj = JsonConvert.DeserializeObject(dataRs) as JObject; //获取响应数据json

                    string dysblsh = jobj["result"]["trt_dcla_detl_sn"].ToString();
                    #region 数据操作
                    string strSql = string.Format(@"INSERT INTO [dbo].[ybzyba]
                       ([dysblsh],[grbh],[xzlx],[lxdh] ,[lxdz],[tcqh],[zddm],[zdmc],[jbbqms],[zwddyljgbh],[zwddyljgmc],
                  [jytcqh],[yytyzybz],[zylx],[zyrq],[zyyy],[zyyj],[ksrq],[jsrq],[cxbz],[sysdate],[balx],[jbr],
                        [ddyljgbm],[ddyljgmc],[yyjdrq],[zdysbm],[zdysxm],[ywsqlx],[dbrxm],[dbrzjlx],[dbrzjhm],[dbrlxdh],[dbrlxdz],[dbrgx],[ddpxh],[bz],xm,sfzh)
                        VALUES
                       ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}'
                       ,'{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}','{20}','{21}','{22}',
                         '{23}','{24}','{25}','{26}','{27}','{28}','{29}','{30}','{31}','{32}','{33}','{34}','{35}','{36}','{37}')",
                          dysblsh, psn_no, "", "", "", "", "", "", "", "", "",
                          "", "", "", "", "", "", begndate, enddate, "1", sysdate, "DBZ", CZYBH,
                          "", "", "", "", "", "", agnter_name, agnter_cert_type, agnter_certno, agnter_tel, agnter_addr, agnter_rlts, "", memo, psn_name, CardInfo.baseinfo.certno);
                    liSQL.Add(strSql);
                    object[] objData = liSQL.ToArray();
                    objData = CliUtils.CallMethod("sybdj", "BatExecuteSql", objData);
                    if (objData[1].ToString().Equals("1"))
                    {
                        WriteLog(sysdate + "  重特大疾病备案登记成功|数据库操作成功|");
                        return new object[] { 0, 1, "  重特大疾病备案登记成功|数据库操作成功|" };
                    }
                    else
                    {
                        WriteLog(sysdate + "  重特大疾病备案登记成功|数据库操作失败|" + strSql);
                        return new object[] { 0, 0, "  重特大疾病备案登记成功|数据库操作失败|" + objData[2].ToString() };
                    }
                    #endregion
                }
                else
                {
                    WriteLog(sysdate + " 重特大疾病备案登记失败|" + Err.ToString());
                    return new object[] { 0, 0, " 重特大疾病备案登记失败|" + Err.ToString() };
                }
                #endregion

            }
            catch (Exception ex)
            {
                WriteLog(sysdate + " 人员定点备案异常|" + ex.Message);
                return new object[] { 0, 0, " 人员定点备案异常|" + ex.Message };
            }
        }
        #endregion

        #region 重特大疾病备案登记撤销
        public static object[] YBTDJBBACX(object[] objParam)
        {
            string sysdate = GetServerDateTime();
            WriteLog(sysdate + "  进入重特大疾病备案登记撤销...");
            WriteLog("进入重特大疾病备案登记撤销|HIS传参|" + string.Join(",", objParam));
            try
            {
                string dysblsh = objParam[0].ToString();//待遇申报流水号
                string grbh = objParam[1].ToString();//人员编号
                string cxyy = "数据填写错误";//撤销原因
                CZYBH = CliUtils.fLoginUser; //操作员工号
                string jbr = CliUtils.fUserName;
                YWZQH = CliUtils.fLoginYbNo.Split('|')[0].ToString();    //业务周期号
                string jysj = Convert.ToDateTime(sysdate).ToString("yyyyMMddHHmmss");
                //交易流水号
                JYLSH = YBJGBH + jysj + new Random().Next(100).ToString().PadLeft(4, '0');
                #region 入参
                string rymbbaJson = string.Empty;
                dynamic data = new
                {
                    data = new
                    {
                        trt_dcla_detl_sn = dysblsh,
                        psn_no = grbh,
                        memo = cxyy
                    }
                };
                string Err = string.Empty;
                rymbbaJson = JsonConvert.SerializeObject(data);
                #endregion

                #region 重特大疾病备案登记撤销
                List<string> liSQL = new List<string>();
                WriteLog(sysdate + "  重特大疾病备案登记撤销|入参Json|" + rymbbaJson);
                int i = YBServiceRequest("2586", data, ref Err);
                if (i == 1)
                {
                    string dataRs = Err.ToString();
                    WriteLog(sysdate + "  重特大疾病备案登记撤销|出参Json|" + dataRs);
                    JObject jobj = JsonConvert.DeserializeObject(dataRs) as JObject; //获取响应数据json

                    #region 数据操作
                    string strSql = string.Format(@"INSERT INTO [dbo].[ybzyba]
                       ([dysblsh],[grbh],[xzlx],[lxdh],[lxdz],[tcqh],[zddm],[zdmc],[jbbqms],[zwddyljgbh],[zwddyljgmc],
                        [jytcqh],[yytyzybz],[zylx],[zyrq],[zyyy],[zyyj],[ksrq],[jsrq],[cxbz],[sysdate],[balx],[jbr],
                        [ddyljgbm],[ddyljgmc],[yyjdrq],[zdysbm],[zdysxm],[ywsqlx],[dbrxm],[dbrzjlx],[dbrzjhm],[dbrlxdh],[dbrlxdz],[dbrgx],[ddpxh],[bz])
                        select [dysblsh],[grbh],[xzlx],[lxdh],[lxdz],[tcqh],[zddm],[zdmc],[jbbqms],[zwddyljgbh],[zwddyljgmc],
                        [jytcqh],[yytyzybz],[zylx],[zyrq],[zyyy],[zyyj],[ksrq],[jsrq],'0','{0}',[balx],'{1}',
                        [ddyljgbm],[ddyljgmc],[yyjdrq],[zdysbm],[zdysxm],[ywsqlx],[dbrxm],[dbrzjlx],[dbrzjhm],[dbrlxdh],[dbrlxdz],[dbrgx],[ddpxh],[bz] from ybzyba where cxbz=1 and balx='DD' and dysblsh='{2}'", sysdate, CZYBH, dysblsh);
                    liSQL.Add(strSql);
                    strSql = string.Format("update ybzyba set cxbz=2 where cxbz=1 and balx='DBZ' and dysblsh='{0}'", dysblsh);
                    liSQL.Add(strSql);
                    object[] objData = liSQL.ToArray();
                    objData = CliUtils.CallMethod("sybdj", "BatExecuteSql", objData);
                    if (objData[1].ToString().Equals("1"))
                    {
                        WriteLog(sysdate + "  重特大疾病备案登记撤销成功|数据库操作成功|");
                        return new object[] { 0, 1, "  重特大疾病备案登记撤销成功|数据库操作成功|" };
                    }
                    else
                    {
                        WriteLog(sysdate + "  重特大疾病备案登记撤销成功|数据库操作失败|" + strSql.ToString());
                        return new object[] { 0, 0, "  重特大疾病备案登记撤销成功|数据库操作失败|" + strSql.ToString() };
                    }
                    #endregion
                }
                else
                {
                    WriteLog(sysdate + "   重特大疾病备案登记撤销失败|" + Err.ToString());
                    return new object[] { 0, 0, "   重特大疾病备案登记撤销失败|" + Err.ToString() };
                }
                #endregion


            }
            catch (Exception ex)
            {
                WriteLog(sysdate + " 重特大疾病备案登记撤销异常|" + ex.Message);
                return new object[] { 0, 0, " 重特大疾病备案登记撤销异常|" + ex.Message };
            }
        }
        #endregion

        #region 外伤登记备案
        public static object[] YBWSDJBA(object[] objParam)
        {
            string sysdate = GetServerDateTime();
            WriteLog(sysdate + "  进入外伤登记备案...");
            WriteLog("进入外伤登记备案|HIS传参|" + string.Join(",", objParam));
            try
            {
                CZYBH = CliUtils.fLoginUser; //操作员工号
                string jbr = CliUtils.fUserName;
                YWZQH = CliUtils.fLoginYbNo.Split('|')[0].ToString();    //业务周期号
                string jysj = Convert.ToDateTime(sysdate).ToString("yyyyMMddHHmmss");

                string psn_no = objParam[0].ToString();
                string insutype = objParam[1].ToString();
                string begndate = objParam[2].ToString();
                string enddate = objParam[3].ToString();
                string psn_cert_type = objParam[4].ToString();
                string certno = objParam[5].ToString();
                string tel = objParam[6].ToString();
                string addr = objParam[7].ToString();
                string insu_admdvs = objParam[8].ToString();
                string emp_no = objParam[9].ToString();
                string emp_name = objParam[10].ToString();
                string mdtrt_id = objParam[11].ToString();
                string setl_id = objParam[12].ToString();
                string trum_part = objParam[13].ToString();
                string trum_time = objParam[14].ToString();
                string trum_site = objParam[15].ToString();
                string trum_rea = objParam[16].ToString();
                string adm_mtd = objParam[17].ToString();
                string adm_time = objParam[18].ToString();
                string adm_diag_dscr = objParam[19].ToString();
                string agnter_name = objParam[20].ToString();
                string agnter_cert_type = objParam[21].ToString();
                string agnter_certno = objParam[22].ToString();
                string agnter_tel = objParam[23].ToString();
                string agnter_addr = objParam[24].ToString();
                string agnter_rlts = objParam[25].ToString();
                string biz_used_flag = objParam[26].ToString();
                string memo = objParam[27].ToString();
                string ttp_pay_prop = objParam[28].ToString();
                string xm = CardInfo.baseinfo.psn_name;
                string sfzh = CardInfo.baseinfo.certno;

                #region 入参
                string ryddbaJson = string.Empty;
                dynamic data = new
                {
                    data = new
                    {
                        psn_no = psn_no,
                        insutype = insutype,
                        begndate = begndate,
                        enddate = enddate,
                        psn_cert_type = psn_cert_type,
                        certno = certno,
                        tel = tel,
                        addr = addr,
                        insu_admdvs = insu_admdvs,
                        emp_no = emp_no,
                        emp_name = emp_name,
                        mdtrt_id = mdtrt_id,
                        setl_id = setl_id,
                        trum_part = trum_part,
                        trum_time = trum_time,
                        trum_site = trum_site,
                        trum_rea = trum_rea,
                        adm_mtd = adm_mtd,
                        adm_time = adm_time,
                        adm_diag_dscr = adm_diag_dscr,
                        agnter_name = agnter_name,
                        agnter_cert_type = agnter_cert_type,
                        agnter_certno = agnter_certno,
                        agnter_tel = agnter_tel,
                        agnter_addr = agnter_addr,
                        agnter_rlts = agnter_rlts,
                        biz_used_flag = biz_used_flag,
                        memo = memo,
                        ttp_pay_prop = ttp_pay_prop
                    }

                };
                string Err = string.Empty;
                ryddbaJson = JsonConvert.SerializeObject(data);
                #endregion

                #region 外伤登记备案
                WriteLog(sysdate + "  外伤登记备案|入参Json|" + ryddbaJson);
                int i = YBServiceRequest("2560", data, ref Err);
                if (i == 1)
                {
                    List<string> liSQL = new List<string>();
                    string dataRs = Err.ToString();
                    WriteLog(sysdate + "  外伤登记备案|出参Json|" + dataRs);
                    JObject jobj = JsonConvert.DeserializeObject(dataRs) as JObject; //获取响应数据json

                    string dysblsh = jobj["output"]["trtDclaDetlSn"].ToString();
                    #region 数据操作
                    string strSql = string.Format(@"INSERT INTO [dbo].[ybzyba]
                       ([dysblsh],[grbh],[xzlx],[lxdh] ,[lxdz],[tcqh],[zddm],[zdmc],[jbbqms],[zwddyljgbh],[zwddyljgmc],
                  [jytcqh],[yytyzybz],[zylx],[zyrq],[zyyy],[zyyj],[ksrq],[jsrq],[cxbz],[sysdate],[balx],[jbr],[ddyljgbm],[ddyljgmc],[yyjdrq],[zdysbm],[zdysxm],[ywsqlx],[dbrxm],[dbrzjlx],[dbrzjhm],[dbrlxdh],[dbrlxdz],[dbrgx],[ddpxh],[bz],xm)
                        VALUES
                       ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}'
                       ,'{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}','{20}','{21}','{22}','{23}','{24}','{25}','{26}','{27}','{28}','{29}','{30}','{31}','{32}','{33}','{34}','{35}','{36}','{37}')",
                         dysblsh, psn_no, "", tel, addr, "", "", "", "", "", "",
                         "", "", "", "", "", "", begndate, enddate, "1", sysdate, "WS", CZYBH,
                         "", "", "", "", "", "", agnter_name, agnter_cert_type, agnter_certno, agnter_tel, agnter_addr, agnter_rlts, "", memo, xm);
                    liSQL.Add(strSql);
                    object[] objData = liSQL.ToArray();
                    objData = CliUtils.CallMethod("sybdj", "BatExecuteSql", objData);
                    if (objData[1].ToString().Equals("1"))
                    {
                        WriteLog(sysdate + "  外伤登记备案成功|数据库操作成功|");
                        return new object[] { 0, 1, "  外伤登记备案成功|数据库操作成功|" };
                    }
                    else
                    {
                        WriteLog(sysdate + "  外伤登记备案成功|数据库操作失败|" + strSql);
                        return new object[] { 0, 0, "  外伤登记备案成功|数据库操作失败|" + objData[2].ToString() };
                    }
                    #endregion
                }
                else
                {
                    WriteLog(sysdate + " 外伤登记备案失败|" + Err.ToString());
                    return new object[] { 0, 0, " 外伤登记备案失败|" + Err.ToString() };
                }
                #endregion

            }
            catch (Exception ex)
            {
                WriteLog(sysdate + " 外伤登记备案异常|" + ex.Message);
                return new object[] { 0, 0, " 外伤登记备案异常|" + ex.Message };
            }
        }
        #endregion

        #region 外伤登记备案撤销
        public static object[] YBWSDJBACX(object[] objParam)
        {
            string sysdate = GetServerDateTime();
            WriteLog(sysdate + "  进入外伤登记备案撤销...");
            WriteLog("进入外伤登记备案撤销|HIS传参|" + string.Join(",", objParam));
            try
            {
                string dysblsh = objParam[0].ToString();//待遇申报流水号
                string grbh = objParam[1].ToString();//人员编号
                string cxyy = "数据填写错误";//撤销原因
                CZYBH = CliUtils.fLoginUser; //操作员工号
                string jbr = CliUtils.fUserName;
                YWZQH = CliUtils.fLoginYbNo.Split('|')[0].ToString();    //业务周期号
                string jysj = Convert.ToDateTime(sysdate).ToString("yyyyMMddHHmmss");
                //交易流水号
                JYLSH = YBJGBH + jysj + new Random().Next(100).ToString().PadLeft(4, '0');
                #region 入参
                string rymbbaJson = string.Empty;
                dynamic data = new
                {
                    data = new
                    {
                        trt_dcla_detl_sn = dysblsh,
                        psn_no = grbh,
                        memo = cxyy
                    }
                };
                string Err = string.Empty;
                rymbbaJson = JsonConvert.SerializeObject(data);
                #endregion

                #region 外伤登记备案撤销
                List<string> liSQL = new List<string>();
                WriteLog(sysdate + "  外伤登记备案撤销|入参Json|" + rymbbaJson);

                int i = YBServiceRequest("2561", data, ref Err);
                if (i == 1)
                {
                    string dataRs = Err.ToString();
                    WriteLog(sysdate + "  外伤登记备案撤销|出参Json|" + dataRs);
                    JObject jobj = JsonConvert.DeserializeObject(dataRs) as JObject; //获取响应数据json

                    #region 数据操作
                    string strSql = string.Format(@"INSERT INTO [dbo].[ybzyba]
                       ([dysblsh],[grbh],[xzlx],[lxdh],[lxdz],[tcqh],[zddm],[zdmc],[jbbqms],[zwddyljgbh],[zwddyljgmc],
                        [jytcqh],[yytyzybz],[zylx],[zyrq],[zyyy],[zyyj],[ksrq],[jsrq],[cxbz],[sysdate],[balx],[jbr],
                        [ddyljgbm],[ddyljgmc],[yyjdrq],[zdysbm],[zdysxm],[ywsqlx],[dbrxm],[dbrzjlx],[dbrzjhm],[dbrlxdh],[dbrlxdz],[dbrgx],[ddpxh],[bz])
                        select [dysblsh],[grbh],[xzlx],[lxdh],[lxdz],[tcqh],[zddm],[zdmc],[jbbqms],[zwddyljgbh],[zwddyljgmc],
                        [jytcqh],[yytyzybz],[zylx],[zyrq],[zyyy],[zyyj],[ksrq],[jsrq],'0','{0}',[balx],'{1}',
                        [ddyljgbm],[ddyljgmc],[yyjdrq],[zdysbm],[zdysxm],[ywsqlx],[dbrxm],[dbrzjlx],[dbrzjhm],[dbrlxdh],[dbrlxdz],[dbrgx],[ddpxh],[bz] from ybzyba where cxbz=1 and balx='DD' and dysblsh='{2}'", sysdate, CZYBH, dysblsh);
                    liSQL.Add(strSql);
                    strSql = string.Format("update ybzyba set cxbz=2 where cxbz=1 and balx='DD' and dysblsh='{0}'", dysblsh);
                    liSQL.Add(strSql);
                    object[] objData = liSQL.ToArray();
                    objData = CliUtils.CallMethod("sybdj", "BatExecuteSql", objData);
                    if (objData[1].ToString().Equals("1"))
                    {
                        WriteLog(sysdate + "  外伤登记备案撤销成功|数据库操作成功|");
                        return new object[] { 0, 1, "  外伤登记备案撤销成功|数据库操作成功|" };
                    }
                    else
                    {
                        WriteLog(sysdate + "  外伤登记备案撤销成功|数据库操作失败|" + strSql);
                        return new object[] { 0, 0, "  外伤登记备案撤销成功|数据库操作失败|" + objData[2].ToString() };
                    }
                    #endregion
                }
                else
                {
                    WriteLog(sysdate + "   外伤登记备案撤销失败|" + Err.ToString());
                    return new object[] { 0, 0, "   外伤登记备案撤销失败|" + Err.ToString() };
                }
                #endregion


            }
            catch (Exception ex)
            {
                WriteLog(sysdate + " 外伤登记备案撤销异常|" + ex.Message);
                return new object[] { 0, 0, " 外伤登记备案撤销异常|" + ex.Message };
            }
        }
        #endregion

        #region 生育待遇备案
        /*
         {"data":{"rchk_flag":"0","insu_admdvs":"360399","poolarea_no":null,"last_mena_date":null,"emp_name":"普通单位二","memo":"","opter_id":"a94","brdy":null,"naty":null,"rid":"360000202109181654431400260446","crte_optins_no":null,"evt_type":"01","begndate":1631954997000,"certno":"511800199104218406","psn_no":"3600000000002031","fixmedins_code":null,"emp_no":"36000000000000000000002012","opter_name":"市本级经办员","optins_no":"360399","crte_time":null,"fetts":null,"matn_qua_reg_stas":"1","spus_certno":"","tel":"13479257389","fpsc_no":"","psn_cert_type":"01","plan_matn_date":null,"gend":null,"matn_type":"1","trt_dcla_detl_sn":"360000163195528354400000008512","opt_time":1631955283552,"geso_val":null,"spus_cert_type":"","spus_name":"","crter_name":null,"insutype":"310","psn_name":"嘤嘤嘤","serv_matt_inst_id":"142021091816544336000000","psn_insu_rlts_id":"36000000000000007099","enddate":null,"matn_trt_dclaer_type":"1","vali_flag":"1","evtsn":"3600001631955283541000000000000000016958","crter_id":null,"fixmedins_name":null,"dcla_date":1631955283000,"updt_time":null,"appy_psn_name":null}}*/
        public static object[] YBSYDYBA(object[] objParam)
        {
            string sysdate = GetServerDateTime();
            WriteLog(sysdate + "  进入育待遇病备案...");
            WriteLog("进入育待遇备案|HIS传参|" + string.Join(",", objParam));
            ReadCardInfo card = CardInfo;
            if (card == null)
            {
                return new object[] { 0, 0, "备案前应先读卡！" };
            }
            try
            {
                #region 入参赋值
                CZYBH = CliUtils.fLoginUser; //操作员工号
                string jbr = CliUtils.fUserName;
                YWZQH = CliUtils.fLoginYbNo.Split('|')[0].ToString();    //业务周期号
                string jysj = Convert.ToDateTime(sysdate).ToString("yyyyMMddHHmmss");
                //交易流水号
                JYLSH = YBJGBH + jysj + new Random().Next(100).ToString().PadLeft(4, '0');
                string psn_no = objParam[0].ToString();
                string insutype = objParam[1].ToString();
                string psn_cert_type = objParam[2].ToString();
                string certno = objParam[3].ToString();
                string psn_name = objParam[4].ToString();
                string naty = card.baseinfo.naty;
                string gend = card.baseinfo.gend;
                string brdy = objParam[5].ToString();
                string tel = objParam[6].ToString();
                string addr = objParam[7].ToString();
                string insu_admdvs = objParam[8].ToString();
                string emp_no = objParam[9].ToString();
                string emp_name = objParam[10].ToString();
                string geso_val = objParam[11].ToString();
                string matn_trt_dclaer_type = objParam[12].ToString();
                string fpsc_no = objParam[13].ToString();
                string last_mena_date = objParam[14].ToString();
                string plan_matn_date = objParam[15].ToString();
                string dcla_date = objParam[16].ToString();
                string spus_name = objParam[17].ToString();
                string spus_cert_type = objParam[18].ToString();
                string spus_certno = objParam[19].ToString();
                string matn_qua_reg_stas = objParam[20].ToString();
                string begndate = objParam[21].ToString();
                string enddate = objParam[22].ToString();
                string agnter_name = objParam[23].ToString();
                string agnter_cert_type = objParam[24].ToString();
                string agnter_certno = objParam[25].ToString();
                string agnter_tel = objParam[26].ToString();
                string agnter_addr = objParam[27].ToString();
                string agnter_rlts = objParam[28].ToString();
                string memo = objParam[29].ToString();
                string fetts = objParam[30].ToString();
                string matn_type = objParam[31].ToString();
                string otp_exam_reim_std = objParam[32].ToString();
                string sfzh = CardInfo.baseinfo.certno;
                #endregion

                #region 入参
                string ryddbaJson = string.Empty;
                dynamic data = new
                {
                    data = new
                    {
                        psn_no = psn_no,
                        insutype = insutype,
                        psn_cert_type = psn_cert_type,
                        certno = certno,
                        psn_name = psn_name,
                        naty = naty,
                        gend = gend,
                        brdy = brdy,
                        tel = tel,
                        addr = addr,
                        insu_admdvs = insu_admdvs,
                        emp_no = emp_no,
                        emp_name = emp_name,
                        geso_val = geso_val,
                        matn_trt_dclaer_type = matn_trt_dclaer_type,
                        fpsc_no = fpsc_no,
                        last_mena_date = last_mena_date,
                        plan_matn_date = plan_matn_date,
                        dcla_date = dcla_date,
                        spus_name = spus_name,
                        spus_cert_type = spus_cert_type,
                        spus_certno = spus_certno,
                        matn_qua_reg_stas = matn_qua_reg_stas,
                        begndate = begndate,
                        enddate = enddate,
                        agnter_name = agnter_name,
                        agnter_cert_type = agnter_cert_type,
                        agnter_certno = agnter_certno,
                        agnter_tel = agnter_tel,
                        agnter_addr = agnter_addr,
                        agnter_rlts = agnter_rlts,
                        memo = memo,
                        fetts = fetts,
                        matn_type = matn_type,
                        otp_exam_reim_std = otp_exam_reim_std
                    }
                };
                string Err = string.Empty;
                ryddbaJson = JsonConvert.SerializeObject(data);
                #endregion

                #region 育待遇备案
                WriteLog(sysdate + "  育待遇备案|入参Json|" + ryddbaJson);

                int i = YBServiceRequest("2572", data, ref Err);
                if (i == 1)
                {
                    List<string> liSQL = new List<string>();
                    string dataRs = Err.ToString();
                    WriteLog(sysdate + "  育待遇备案|出参Json|" + dataRs);
                    JObject jobj = JsonConvert.DeserializeObject<JObject>(Err);
                    string dysblsh = jobj["data"]["trt_dcla_detl_sn"].ToString();
                    #region 数据操作
                    string strSql = string.Format(@"INSERT INTO [dbo].[ybzyba]
                       ([dysblsh],[grbh],[xzlx],[lxdh] ,[lxdz],[tcqh],[zddm],[zdmc],[jbbqms],[zwddyljgbh],[zwddyljgmc],
                  [jytcqh],[yytyzybz],[zylx],[zyrq],[zyyy],[zyyj],[ksrq],[jsrq],[cxbz],[sysdate],[balx],[jbr],
                        [ddyljgbm],[ddyljgmc],[yyjdrq],[zdysbm],[zdysxm],[ywsqlx],[dbrxm],[dbrzjlx],[dbrzjhm],[dbrlxdh],[dbrlxdz],[dbrgx],[ddpxh],[bz],xm,sfzh)
                        VALUES
                       ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}'
                       ,'{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}','{20}','{21}','{22}',
                         '{23}','{24}','{25}','{26}','{27}','{28}','{29}','{30}','{31}','{32}','{33}','{34}','{35}','{36}','{37}')",
                         dysblsh, psn_no, "", tel, addr, "", "", "", "", "", "",
                         "", "", "", "", "", "", begndate, enddate, "1", sysdate, "SY", CZYBH,
                         "", "", "", "", "", "", agnter_name, agnter_cert_type, agnter_certno, agnter_tel, agnter_addr, agnter_rlts, "", memo, psn_name, sfzh);
                    liSQL.Add(strSql);
                    object[] objData = liSQL.ToArray();
                    objData = CliUtils.CallMethod("sybdj", "BatExecuteSql", objData);
                    if (objData[1].ToString().Equals("1"))
                    {
                        WriteLog(sysdate + "  育待遇备案成功|数据库操作成功|");
                        return new object[] { 0, 1, "  育待遇备案成功|数据库操作成功|" };
                    }
                    else
                    {
                        WriteLog(sysdate + "  育待遇备案成功|数据库操作失败|" + strSql);
                        return new object[] { 0, 0, "  育待遇备案成功|数据库操作失败|" + objData[2].ToString() };
                    }
                    #endregion
                }
                else
                {
                    WriteLog(sysdate + " 育待遇备案失败|" + Err.ToString());
                    return new object[] { 0, 0, " 育待遇备案失败|" + Err.ToString() };
                }
                #endregion

            }
            catch (Exception ex)
            {
                WriteLog(sysdate + " 育待遇备案异常|" + ex.Message);
                return new object[] { 0, 0, " 育待遇备案异常|" + ex.Message };
            }
        }
        #endregion

        #region 生育待遇备案撤销
        public static object[] YBSYDYBACX(object[] objParam)
        {
            string sysdate = GetServerDateTime();
            WriteLog(sysdate + "  进入育待遇备案撤销...");
            WriteLog("进入育待遇备案撤销|HIS传参|" + string.Join(",", objParam));
            try
            {
                string dysblsh = objParam[0].ToString();//待遇申报流水号
                string grbh = objParam[1].ToString();//人员编号
                string cxyy = "数据填写错误";//撤销原因
                CZYBH = CliUtils.fLoginUser; //操作员工号
                string jbr = CliUtils.fUserName;
                YWZQH = CliUtils.fLoginYbNo.Split('|')[0].ToString();    //业务周期号
                string jysj = Convert.ToDateTime(sysdate).ToString("yyyyMMddHHmmss");
                //交易流水号
                JYLSH = YBJGBH + jysj + new Random().Next(100).ToString().PadLeft(4, '0');
                #region 入参
                string rymbbaJson = string.Empty;
                dynamic data = new
                {
                    data = new
                    {

                        trt_dcla_detl_sn = dysblsh,
                        psn_no = grbh,
                        memo = cxyy
                    }
                };
                string Err = string.Empty;
                rymbbaJson = JsonConvert.SerializeObject(data);
                #endregion

                #region 育待遇备案撤销
                List<string> liSQL = new List<string>();
                WriteLog(sysdate + "  育待遇备案撤销|入参Json|" + rymbbaJson);
                int i = YBServiceRequest("2573", data, ref Err);
                if (i == 1)
                {
                    string dataRs = Err.ToString();
                    WriteLog(sysdate + "  育待遇备案撤销|出参Json|" + dataRs);

                    #region 数据操作
                    string strSql = string.Format(@"INSERT INTO [dbo].[ybzyba]
                       ([dysblsh],[grbh],[xzlx],[lxdh],[lxdz],[tcqh],[zddm],[zdmc],[jbbqms],[zwddyljgbh],[zwddyljgmc],
                        [jytcqh],[yytyzybz],[zylx],[zyrq],[zyyy],[zyyj],[ksrq],[jsrq],[cxbz],[sysdate],[balx],[jbr],
                        [ddyljgbm],[ddyljgmc],[yyjdrq],[zdysbm],[zdysxm],[ywsqlx],[dbrxm],[dbrzjlx],[dbrzjhm],[dbrlxdh],[dbrlxdz],[dbrgx],[ddpxh],[bz])
                        select [dysblsh],[grbh],[xzlx],[lxdh],[lxdz],[tcqh],[zddm],[zdmc],[jbbqms],[zwddyljgbh],[zwddyljgmc],
                        [jytcqh],[yytyzybz],[zylx],[zyrq],[zyyy],[zyyj],[ksrq],[jsrq],'0','{0}',[balx],'{1}',
                        [ddyljgbm],[ddyljgmc],[yyjdrq],[zdysbm],[zdysxm],[ywsqlx],[dbrxm],[dbrzjlx],[dbrzjhm],[dbrlxdh],[dbrlxdz],[dbrgx],[ddpxh],[bz] from ybzyba where cxbz=1 and balx='DD' and dysblsh='{2}'", sysdate, CZYBH, dysblsh);
                    liSQL.Add(strSql);
                    strSql = string.Format("update ybzyba set cxbz=2 where cxbz=1 and balx='DD' and dysblsh='{0}'", dysblsh);
                    liSQL.Add(strSql);
                    object[] objData = liSQL.ToArray();
                    objData = CliUtils.CallMethod("sybdj", "BatExecuteSql", objData);
                    if (objData[1].ToString().Equals("1"))
                    {
                        WriteLog(sysdate + "  育待遇备案撤销成功|数据库操作成功|");
                        return new object[] { 0, 1, "  育待遇备案撤销成功|数据库操作成功|" };
                    }
                    else
                    {
                        WriteLog(sysdate + "  育待遇备案撤销成功|数据库操作失败|" + strSql);
                        return new object[] { 0, 0, "  育待遇备案撤销成功|数据库操作失败|" + objData[2].ToString() };
                    }
                    #endregion
                }
                else
                {
                    WriteLog(sysdate + "   育待遇备案撤销失败|" + Err.ToString());
                    return new object[] { 0, 0, "   育待遇备案撤销失败|" + Err.ToString() };
                }
                #endregion


            }
            catch (Exception ex)
            {
                WriteLog(sysdate + " 育待遇备案撤销异常|" + ex.Message);
                return new object[] { 0, 0, " 育待遇备案撤销异常|" + ex.Message };
            }
        }
        #endregion


        #region 转院备案
        public static object[] YBZYBA(object[] objParam)
        {
            string sysdate = GetServerDateTime();
            WriteLog(sysdate + "  进入转院备案...");
            WriteLog("进入转院备案|HIS传参|" + string.Join(",", objParam));
            try
            {
                CZYBH = CliUtils.fLoginUser; //操作员工号
                string jbr = CliUtils.fUserName;
                YWZQH = CliUtils.fLoginYbNo.Split('|')[0].ToString();    //业务周期号
                string jysj = Convert.ToDateTime(sysdate).ToString("yyyyMMddHHmmss");
                //交易流水号 
                string psn_no = objParam[0].ToString();
                string insutype = objParam[1].ToString();
                string tel = objParam[2].ToString();
                string addr = objParam[3].ToString();
                string insu_optins = objParam[4].ToString();
                string diag_code = objParam[5].ToString();
                string diag_name = objParam[6].ToString();
                string dise_cond_dscr = objParam[7].ToString();
                string reflin_medins_no = objParam[8].ToString();
                string reflin_medins_name = objParam[9].ToString();
                string mdtrtarea_admdvs = objParam[10].ToString();
                string hosp_agre_refl_flag = objParam[11].ToString();
                string refl_type = objParam[12].ToString();
                string refl_date = objParam[13].ToString();
                string refl_rea = objParam[14].ToString();
                string refl_opnn = objParam[15].ToString();
                string begndate = objParam[16].ToString();
                string enddate = objParam[17].ToString();
                string refl_used_flag = objParam[18].ToString();
                string xm = CardInfo.baseinfo.psn_name;
                string sfzh = CardInfo.baseinfo.certno;

                string getjzidsql = $"select   z1zyno from zy01h  where z1idno='{CardInfo.baseinfo.certno}'";
                DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", getjzidsql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                if (ds.Tables[0].Rows.Count == 0)
                {
                    WriteLog(sysdate + "  该患者没有找到对应的就诊记录！");
                    return new object[] { 0, 0, "无上传费用明细信息" };
                }
                string refl_old_mdtrt_id = ds.Tables[0].Rows[0]["z1zyno"].ToString();

                #region 入参
                string ryddbaJson = string.Empty;
                dynamic data = new
                {
                    refmedin = new
                    {
                        psn_no = psn_no,
                        insutype = insutype,
                        tel = tel,
                        addr = addr,
                        insu_optins = insu_optins,
                        diag_code = diag_code,
                        diag_name = diag_name,
                        dise_cond_dscr = dise_cond_dscr,
                        reflin_medins_no = reflin_medins_no,
                        reflin_medins_name = reflin_medins_name,
                        mdtrtarea_admdvs = mdtrtarea_admdvs,
                        hosp_agre_refl_flag = hosp_agre_refl_flag,
                        refl_type = refl_type,
                        refl_date = refl_date,
                        refl_rea = refl_rea,
                        refl_opnn = refl_opnn,
                        begndate = begndate,
                        enddate = enddate,
                        refl_used_flag = refl_used_flag

                    }
                };
                ryddbaJson = JsonConvert.SerializeObject(data);
                #endregion
                string Err = string.Empty;
                #region 转院备案
                WriteLog(sysdate + "  转院备案|入参Json|" + ryddbaJson);
                int i = YBServiceRequest("2501", data, ref Err);
                if (i == 1)
                {
                    List<string> liSQL = new List<string>();
                    string dataRs = Err;
                    WriteLog(sysdate + "  转院备案|出参Json|" + dataRs);
                    JObject jobj = JsonConvert.DeserializeObject(dataRs) as JObject; //获取响应数据json 
                    JObject res = JObject.Parse(jobj.GetValue("result").ToString());
                    string dysblsh = res["trt_dcla_detl_sn"].ToString();
                    #region 数据操作
                    string strSql = string.Format(@"INSERT INTO [dbo].[ybzyba]
                       ([dysblsh],[grbh],[xzlx],[lxdh] ,[lxdz],[tcqh],[zddm],[zdmc],[jbbqms],[zwddyljgbh],[zwddyljgmc],
		                [jytcqh],[yytyzybz],[zylx],[zyrq],[zyyy],[zyyj],[ksrq],[jsrq],[cxbz],[sysdate],[balx],[jbr],
                        [ddyljgbm],[ddyljgmc],[yyjdrq],[zdysbm],[zdysxm],[ywsqlx],[dbrxm],[dbrzjlx],[dbrzjhm],[dbrlxdh],[dbrlxdz],[dbrgx],[ddpxh],[bz],xm,sfzh)
                        VALUES
                       ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}'
                       ,'{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}','{20}','{21}','{22}',
                         '{23}','{24}','{25}','{26}','{27}','{28}','{29}','{30}','{31}','{32}','{33}','{34}','{35}','{36}','{37}','{38}')",
                         dysblsh, psn_no, "", tel, addr, insu_optins, diag_code, diag_name, "", reflin_medins_no, reflin_medins_name,
                        mdtrtarea_admdvs, hosp_agre_refl_flag, refl_type, refl_date, refl_rea, refl_opnn, begndate, enddate, "1", sysdate, "ZY", CZYBH,
                         "", "", "", "", "", "", "", "", "", "", "", "", "", "", xm, sfzh);
                    liSQL.Add(strSql);
                    object[] objData = liSQL.ToArray();
                    objData = CliUtils.CallMethod("sybdj", "BatExecuteSql", objData);
                    if (objData[1].ToString().Equals("1"))
                    {
                        WriteLog(sysdate + "  转院备案成功|数据库操作成功|");
                        return new object[] { 0, 1, "  转院备案成功|数据库操作成功|" };
                    }
                    else
                    {
                        WriteLog(sysdate + "  转院备案成功|数据库操作失败|" + strSql);
                        return new object[] { 0, 0, "  转院备案成功|数据库操作失败|" + objData[2].ToString() };
                    }
                    #endregion
                }
                else
                {
                    WriteLog(sysdate + " 转院备案失败|" + Err);
                    return new object[] { 0, 0, " 转院备案失败|" + Err };
                }
                #endregion

            }
            catch (Exception ex)
            {
                WriteLog(sysdate + " 转院备案异常|" + ex.Message);
                return new object[] { 0, 0, " 转院备案异常|" + ex.Message };
            }
        }
        #endregion

        #region 转院备案撤销
        public static object[] YBZYBACX(object[] objParam)
        {
            string sysdate = GetServerDateTime();
            WriteLog(sysdate + "  进入转院备案撤销...");
            WriteLog("进入转院备案撤销|HIS传参|" + string.Join(",", objParam));
            try
            {
                string dysblsh = objParam[0].ToString();//待遇申报流水号
                string grbh = objParam[1].ToString();//人员编号
                string cxyy = "数据填写错误";//撤销原因
                CZYBH = CliUtils.fLoginUser; //操作员工号
                string jbr = CliUtils.fUserName;
                YWZQH = CliUtils.fLoginYbNo.Split('|')[0].ToString();    //业务周期号
                string jysj = Convert.ToDateTime(sysdate).ToString("yyyyMMddHHmmss");
                //交易流水号 
                #region 入参
                string rymbbaJson = string.Empty;
                dynamic data = new
                {
                    data = new
                    {
                        trt_dcla_detl_sn = dysblsh,
                        psn_no = grbh,
                        memo = cxyy
                    }

                };
                rymbbaJson = JsonConvert.SerializeObject(data);
                #endregion
                string Err = string.Empty;
                #region 转院备案撤销
                List<string> liSQL = new List<string>();
                WriteLog(sysdate + "  转院备案撤销|入参Json|" + rymbbaJson);

                int i = YBServiceRequest("2502", data, ref Err);

                if (i == 1)
                {
                    string dataRs = Err.ToString();
                    WriteLog(sysdate + "  转院备案撤销|出参Json|" + dataRs);
                    JObject jobj = JsonConvert.DeserializeObject(dataRs) as JObject; //
                    #region 数据操作
                    string strSql = string.Format(@"INSERT INTO [dbo].[ybzyba]
                       ([dysblsh],[grbh],[xzlx],[lxdh],[lxdz],[tcqh],[zddm],[zdmc],[jbbqms],[zwddyljgbh],[zwddyljgmc],
                        [jytcqh],[yytyzybz],[zylx],[zyrq],[zyyy],[zyyj],[ksrq],[jsrq],[cxbz],[sysdate],[balx],[jbr],
                        [ddyljgbm],[ddyljgmc],[yyjdrq],[zdysbm],[zdysxm],[ywsqlx],[dbrxm],[dbrzjlx],[dbrzjhm],[dbrlxdh],[dbrlxdz],[dbrgx],[ddpxh],[bz])
                        select [dysblsh],[grbh],[xzlx],[lxdh],[lxdz],[tcqh],[zddm],[zdmc],[jbbqms],[zwddyljgbh],[zwddyljgmc],
                        [jytcqh],[yytyzybz],[zylx],[zyrq],[zyyy],[zyyj],[ksrq],[jsrq],'0','{0}',[balx],'{1}',
                        [ddyljgbm],[ddyljgmc],[yyjdrq],[zdysbm],[zdysxm],[ywsqlx],[dbrxm],[dbrzjlx],[dbrzjhm],[dbrlxdh],[dbrlxdz],[dbrgx],[ddpxh],[bz] from ybzyba where cxbz=1 and balx='ZY' and dysblsh='{2}'", sysdate, CZYBH, dysblsh);
                    liSQL.Add(strSql);
                    strSql = string.Format("update ybzyba set cxbz=2 where cxbz=1 and balx='ZY' and dysblsh='{0}'", dysblsh);
                    liSQL.Add(strSql);
                    object[] objData = liSQL.ToArray();
                    objData = CliUtils.CallMethod("sybdj", "BatExecuteSql", objData);
                    if (objData[1].ToString().Equals("1"))
                    {
                        WriteLog(sysdate + "  转院备案撤销成功|数据库操作成功|");
                        return new object[] { 0, 1, "  转院备案撤销成功|数据库操作成功|" };
                    }
                    else
                    {
                        WriteLog(sysdate + "  转院备案撤销成功|数据库操作失败|" + strSql);
                        return new object[] { 0, 0, "  转院备案撤销成功|数据库操作失败|" + objData[2].ToString() };
                    }
                    #endregion
                }
                else
                {
                    WriteLog(sysdate + "   转院备案撤销失败|" + Err.ToString());
                    return new object[] { 0, 0, "   转院备案撤销失败|" + Err.ToString() };
                }
                #endregion


            }
            catch (Exception ex)
            {
                WriteLog(sysdate + " 转院备案撤销异常|" + ex.Message);
                return new object[] { 0, 0, " 转院备案撤销异常|" + ex.Message };
            }
        }
        #endregion 

        #region 对账接口

        #region 总额对账
        /// <summary>
        /// 医药机构费用结算对总账
        /// </summary>
        /// <param name="objParams"></param>
        /// <returns></returns>
        public static object[] YBDZZZ(object[] objParams)
        {
            string sysdate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string insutype = objParams[0].ToString();         //险种
            string clr_type = objParams[1].ToString();         //清算类别
            string setl_optins = YBJGBH;      //结算经办机构
            string stmt_begndate = objParams[2].ToString();    //对账开始日期
            string stmt_enddate = objParams[3].ToString();     //对账结束日期
            string medfee_sumamt = objParams[4].ToString();    //医疗费总额
            string fund_pay_sumamt = objParams[5].ToString();  //基金支付总额
            string acct_pay = objParams[6].ToString();         //个人账户支付金额
            string fixmedins_setl_cnt = objParams[7].ToString();//定点医药机构结算笔数

            dynamic input = new
            {
                data = new
                {
                    insutype = insutype,
                    clr_type = clr_type,
                    setl_optins = YBjyqh,
                    stmt_begndate = stmt_begndate,
                    stmt_enddate = stmt_enddate,
                    medfee_sumamt = medfee_sumamt,
                    fund_pay_sumamt = fund_pay_sumamt,
                    acct_pay = acct_pay,
                    fixmedins_setl_cnt = fixmedins_setl_cnt
                }
            };

            string data = JsonConvert.SerializeObject(input);
            string Err = string.Empty;
            WriteLog(sysdate + "  医药机构费用结算对总账|入参Json|" + data);
            int i = YBServiceRequest("3201", input, ref Err);
            if (i > 0)
            {
                JObject j = JsonConvert.DeserializeObject<JObject>(Err);
                string setl_optin = j["stmtinfo"]["setl_optins"].ToString();//结算经办机构
                string stmt_rslt = j["stmtinfo"]["stmt_rslt"].ToString();//对账结果
                string stmt_rslt_dscr = j["stmtinfo"]["stmt_rslt_dscr"].ToString();//对账结果说明
                WriteLog(sysdate + " 医药机构费用结算对总账成功|" + Err.ToString());
                return new object[] { 0, 1, " 医药机构费用结算对总账成功|" + Err.ToString(), new object[] { setl_optin, stmt_rslt, stmt_rslt_dscr } };

            }
            else
            {
                WriteLog(sysdate + " 医药机构费用结算对总账失败|" + Err.ToString());
                return new object[] { 0, 0, " 医药机构费用结算对总账失败|" + Err.ToString() };
            }
        }
        #endregion

        #region 明细对账
        internal static string wjqz = "";
        /// <summary>
        /// 医药机构费用结算对明细
        /// </summary>
        /// <param name="objParams"></param>
        /// <returns></returns>
        public static object[] YBDZMX(object[] objParams)
        {
            string sysdate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            //调用上传接口将 明细数据先上传
            byte[] scwj = (byte[])objParams[0];//文件流
            object[] res = YBWJSC(new object[] { scwj });
            if (res[1].ToString() != "1")
            {
                WriteLog(sysdate + " 明细费用文件上传失败|" + res[2].ToString());
                return new object[] { 0, 0, " 明细费用文件上传失败|" + res[2].ToString() };
            }
            string file_qury_no = res[2].ToString();//文件名称号
            string filename = res[3].ToString();//文件名称
            string fixmedins_code = res[4].ToString();
            string setl_optins = objParams[8].ToString();      //结算经办机构
            string stmt_begndate = objParams[1].ToString();    //对账开始日期
            string stmt_enddate = objParams[2].ToString();     //对账结束日期
            string medfee_sumamt = objParams[3].ToString();    //医疗费总额
            string fund_pay_sumamt = objParams[4].ToString();  //基金支付总额
            string acct_pay = objParams[5].ToString();         //个人账户支付金额
            string cash_payamt = objParams[6].ToString();   //
            string fixmedins_setl_cnt = objParams[7].ToString();//定点医药机构结算笔数
            string clr_type = objParams[9].ToString();//清算类别
            string refd_setl_flag = objParams[10].ToString();//退费结算标识

            dynamic input = new
            {
                data = new
                {
                    setl_optins = setl_optins,
                    file_qury_no = file_qury_no,
                    stmt_begndate = stmt_begndate,
                    stmt_enddate = stmt_enddate,
                    medfee_sumamt = medfee_sumamt,
                    fund_pay_sumamt = fund_pay_sumamt,
                    acct_pay = acct_pay,
                    cash_payamt = cash_payamt,
                    fixmedins_setl_cnt = fixmedins_setl_cnt,
                    clr_type = clr_type,
                    refd_setl_flag = refd_setl_flag
                }
            };

            string data = JsonConvert.SerializeObject(input);
            string Err = string.Empty;
            WriteLog(sysdate + "  医药机构费用结算对明细|入参Json|" + data);
            int i = YBServiceRequest("3202", input, ref Err);
            if (i > 0)
            {
                JObject j = JsonConvert.DeserializeObject<JObject>(Err);
                string wjcxh = j["fileinfo"]["file_qury_no"].ToString();//文件查询号
                string fileName = j["fileinfo"]["filename"].ToString();//文件名称
                string dld_endtime = j["fileinfo"]["dld_endtime"].ToString();//下载截止时间
                WriteLog(sysdate + " 医药机构费用结算对明细成功|" + Err.ToString());

                //调用下载 下载明细对账结果
                object[] resxz = YBWJXZ(new object[] { wjcxh, fileName });
                return new object[] { 0, 1, " 医药机构费用结算对明细成功|" + Err.ToString(), fileName };

            }
            else
            {
                WriteLog(sysdate + " 医药机构费用结算对明细失败|" + Err.ToString());
                return new object[] { 0, 0, " 医药机构费用结算对明细失败|" + Err.ToString() };
            }
        }
        #endregion

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
