using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ybinterface_auto
{
    public class ybinterface_lib_ts
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
        internal static string YLGHBH = lItem[0].DDYLJGBH; //医疗机构编号
        internal static string DDYLJGBH = lItem[0].DDYLJGBH;//医疗机构编号
        internal static string DDYLJGMC = lItem[0].DDYLJGMC;//医院名称
        #endregion

        #region 初始化
        public static object[] YBINIT(object[] objParam)
        {
            string sysdate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            try
            {
                CZYBH = objParam[0].ToString();  //用户工号
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
                WriteLog(sysdate + "  入参" + inputData.ToString());
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
                    return new object[] { 0, 1, YWZQH };
                }
                else
                {
                    WriteLog(sysdate + "  用户" + CZYBH + " 医保签到失败|" + outputData.ToString());
                    return new object[] { 0, 0, outputData.ToString().Split('^')[2].ToString() };
                }
            }
            catch (Exception ex)
            {
                WriteLog(sysdate + "  医保初始化异常|"+ex.Message);
                return new object[] { 0, 0, "医保初始化异常|" + ex.Message };
            }
        }
        #endregion

        #region 退出
        public static object[] YBEXIT(object[] objParam)
        {
            string sysdate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            try
            {
                CZYBH = objParam[0].ToString(); //操作员工号
                YWZQH = objParam[1].ToString();    //业务周期号

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
            catch (Exception ex)
            {
                WriteLog(sysdate + "  医保退出异常|"+ex.Message);
                return new object[] { 0, 0, "医保退出异常|" + ex.Message };
            }
        }
        #endregion

        #region 住院费用登记
        public static object[] YBZYSFDJ(object[] objParam)
        {
            string sysdate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");   //系统时间
            string jzlsh = objParam[0].ToString();  //就诊流水号
            string jzsj = objParam[1].ToString();   //截止时间 中途结算时，截止时间不能为空
            CZYBH = objParam[2].ToString();
            YWZQH = objParam[3].ToString();
            string jbr = objParam[4].ToString();
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

                string strSql = string.Format("select * from ybmzzydjdr a where a.jzlsh = '{0}' and a.cxbz = 1", jzlsh);
                SqlHelper helper = new SqlHelper();
                DataSet ds = helper.ExecuteDataSet(strSql);
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
                                        else 2 end as sfxmzl,z3date as zxsj, a.z3item as yyxmbh, a.z3name as yyxmmc,
                                        a.z3djxx as dj,case LEFT(a.z3endv,1) when '4' then -a.z3jzcs else 1*a.z3jzcs end as sl,
                                        case LEFT(a.z3endv,1) when '4' then -1*a.z3jzje else 1*a.z3jzje end as je, 1 as fs,
                                        a.z3empn as ysdm, a.z3kdys as ysxm,a.z3ksno as ksno, a.z3zxks as zxks, '0' as sfbz,z3sequ
                                        from zy03d a 
                                        where a.z3ybup is null and LEFT(a.z3kind,1) in(2,4)  and a.z3zyno='{0}' {1}", jzlsh, sWhere);
                ds.Tables.Clear();
                ds = helper.ExecuteDataSet(strSql);
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
                    string cflsh = dr["z3sequ"].ToString();  //处方流水号
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
                    #endregion

                    if (string.IsNullOrEmpty(sfxmzl) || string.IsNullOrEmpty(yyxmbm))
                        sMsg = "存在空数据或未配对数据";

                    #region 参数
                    StringBuilder inputParam = new StringBuilder();
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
                    inputParam.Append(by6 + "|$");

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

                string strMsg = string.Empty;
                WriteLog(liYZXX.Count.ToString());
                foreach (string inputData3 in liYZXX)
                {
                    StringBuilder outputData = new StringBuilder(102400);

                    WriteLog(sysdate + "  " + jzlsh + " 进入住院费用登记...");
                    WriteLog(sysdate + "  入参|" + inputData3.ToString().TrimEnd('$'));

                    //入参
                    StringBuilder inputData = new StringBuilder();
                    inputData.Append(YWBH + "^");
                    inputData.Append(YLGHBH + "^");
                    inputData.Append(CZYBH + "^");
                    inputData.Append(YWZQH + "^");
                    inputData.Append(JYLSH + "^");
                    inputData.Append(JRFS + "^");
                    inputData.Append(DKLX + "^");
                    inputData.Append(inputData3.ToString().TrimEnd('$') + "^");
                    int i = BUSINESS_HANDLE(inputData, outputData);
                    if (i != 0)
                    {
                        strMsg += "住院收费登记失败|" + outputData.ToString() + "\r\n";
                        WriteLog(sysdate + "  " + jzlsh + " 住院收费登记(分段)失败|" + outputData.ToString());
                        continue;
                        //return new object[] { 0, 0, "住院收费登记(分段)失败|" + outputData.ToString() };
                    }
                    string[] strVal = inputData3.ToString().TrimEnd('$').Split('|');
                    string sequ = strVal[3].ToString();
                    strSql = string.Format(@"update zy03d set z3ybup='{1}' where z3zyno='{0}' and z3sequ='{3}' {2} and isnull(z3ybup,'')='' ", jzlsh, JYLSH, sWhere,sequ);
                    liSQL.Add(strSql);
                    string[] str = outputData.ToString().Split('^')[1].TrimEnd('$').Split('$');
                    string[] array = str;
                    string s = array[0];
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

                string errMsg = string.Empty;
                bool bFalg = helper.ExecuteSqlTran(liSQL, ref errMsg);

                if (bFalg)
                {
                    if (string.IsNullOrEmpty(strMsg))
                    {
                        WriteLog(sysdate + "  住院费用上传成功|");
                        return new object[] { 0, 1, "费用上传成功|" };
                    }
                    else
                    {
                        WriteLog(sysdate + "  住院费用登记部分失败|\r\n" + strMsg);
                        return new object[] { 0, 1, "费用上传部分失败|" + strMsg };
                    }
                }
                else
                {
                    WriteLog(sysdate + "  住院费用登记失败|数据操作失败|" + errMsg);
                    //门诊费用上传撤销
                    object[] objParam1 = { jzlsh, "", "", jbr, ybjzlsh, CZYBH, YWZQH };
                    objParam1 = N_YBMZZYSFDJCX(objParam1);
                    return new object[] { 0, 0, "住院费用登记失败|数据操作失败|" + errMsg };
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
            string sysdate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");//当前时间
            string jzlsh = objParam[0].ToString(); //就诊流水号
            string cfmxjylsh = ""; //处方交易流水号
            string sWhere = ""; //查询条件
            CZYBH =objParam[1].ToString();
            YWZQH = objParam[2].ToString();
            string jbr = objParam[3].ToString();

            if (string.IsNullOrEmpty(jzlsh))
                return new object[] { 0, 0, "就诊流水号不能为空" };

            if (!string.IsNullOrEmpty(cfmxjylsh))
            {
                sWhere = string.Format(@"and jylsh='{0}'", cfmxjylsh);
            }

            YWBH = "2320";
            JYLSH = DateTime.Now.ToString("yyyyMMddHHmmss") + "-" + YLGHBH + "-" + new Random().Next(100).ToString().PadLeft(4, '0');
          

            //获取上传处方信息
            string strSql = string.Format(@"select distinct jylsh,ybjzlsh from ybcfmxscindr where jzlsh='{0}' and cxbz=1 
                                            and jylsh not in(select isnull(cfmxjylsh,'') as cfjylsh from ybfyjsdr where cxbz=1 and jzlsh='{0}')", jzlsh);
            SqlHelper helper = new SqlHelper();
            DataSet ds = helper.ExecuteDataSet(strSql);
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
                strSql = string.Format(@"update ybcfmxscindr set cxbz=2 where jzlsh='{0}' and jylsh='{1}' and cxbz=1", jzlsh, cfmxjylsh);
                liSQL.Add(strSql);

                strSql = string.Format(@"insert into ybcfmxscfhdr(jzlsh,jylsh,yyxmdm,ybcfh,cfh,je,zfje,zlje,zlbl,qezfbz,
                                         sfxmdj,yyxmmc,grbh,xm,kh,cxbz,sysdate ) select
                                         jzlsh,jylsh,yyxmdm,ybcfh,cfh,je,zfje,zlje,zlbl,qezfbz,
                                         sfxmdj,yyxmmc,grbh,xm,kh,0,'{2}' from ybcfmxscfhdr where jzlsh='{0}' and jylsh='{1}' and cxbz=1",
                                         jzlsh, cfmxjylsh, sysdate);
                liSQL.Add(strSql);
                strSql = string.Format(@"update ybcfmxscfhdr set cxbz=2 where jzlsh='{0}' and jylsh='{1}' and cxbz=1", jzlsh, cfmxjylsh);
                liSQL.Add(strSql);

                strSql = string.Format("update zy03d set z3ybup = null where z3zyno = '{0}' and z3ybup='{1}'", jzlsh, cfmxjylsh);
                liSQL.Add(strSql);

                string errMsg = string.Empty;
                bool bFalg = helper.ExecuteSqlTran(liSQL, ref errMsg);
                if (bFalg)
                {
                    WriteLog(sysdate + "  住院费用登记撤销成功|" + outputData.ToString());
                    return new object[] { 0, 1, "住院费用登记撤销成功" };
                }
                else
                {
                    WriteLog(sysdate + "  住院费用登记撤销失败|数据操作失败|" + errMsg);
                    return new object[] { 0, 0, "住院费用登记撤销失败|数据操作失败|" + errMsg };
                }
            }
            else
            {
                WriteLog(sysdate + "  住院费用登记撤销失败|" + outputData.ToString());
                return new object[] { 0, 0, "住院费用登记撤销失败|数据操作失败|" + outputData.ToString() };
            }

        }
        #endregion

        #region 门诊/住院费用登记撤销(内部)
        public static object[] N_YBMZZYSFDJCX(object[] objParam)
        {
            string sysdate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string jzlsh = objParam[0].ToString();
            string cfh = objParam[1].ToString();
            string cflsh = objParam[2].ToString();
            string jbr = objParam[3].ToString();
            string ybjzlsh = objParam[4].ToString();
            CZYBH =objParam[5].ToString();
            YWZQH = objParam[6].ToString();


            YWBH = "2320";
            JYLSH = DateTime.Now.ToString("yyyyMMddHHmmss") + "-" + YLGHBH + "-" + new Random().Next(100).ToString().PadLeft(4, '0');

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
