using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Text;

namespace ybinterface_auto
{
    public class ybinterface_lib_th
    {
        public static string dqbh = "360826"; //地区编号
        #region 接口DLL加载
        [DllImport("ZRHosJK.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern int f_UserBargaingInit(string UserID, string PassWD, StringBuilder retMsg);

        [DllImport("ZRHosJK.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern int f_UserBargaingClose(StringBuilder retMsg);

        [DllImport("ZRHosJK.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        private static extern int f_UserBargaingApply(string Ywlx, StringBuilder InData, StringBuilder OutData, StringBuilder retMsg);
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

                #region 获取医保帐号密码
                string strSql = string.Format("select b1ybno,b1ybpw from bz01h  where b1empn ='{0}'", CZYBH);
                SqlHelper helper = new SqlHelper();
                DataSet ds = helper.ExecuteDataSet(strSql);
                if (ds == null || ds.Tables[0].Rows.Count == 0)
                    return new object[] { 0, 0, "获取员工信息出错" };
                string UserID = ds.Tables[0].Rows[0]["b1ybno"].ToString(); //用户名
                string PassWD = ds.Tables[0].Rows[0]["b1ybpw"].ToString(); //密码
                StringBuilder retMsg = new StringBuilder(1024);
                #endregion

                int i = f_UserBargaingInit(UserID, PassWD, retMsg);
                if (i > 0)
                {
                    WriteLog(sysdate + "  初始化成功");
                    return new object[] { 0, 1, "医保初始化成功" };
                }
                else
                {
                    WriteLog(sysdate + "  医保初始化失败|" + retMsg.ToString());
                    return new object[] { 0, 0, "医保初始化失败|" + retMsg.ToString() };
                }
               
            }
            catch (Exception ex)
            {
                WriteLog(sysdate + "  医保初始化异常|" + ex.Message);
                return new object[] { 0, 0, "医保初始化异常|" + ex.Message };
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

        #region 住院收费登记每日上传
        public static object[] YBZYSFDJDAY(object[] objParam)
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

                if (string.IsNullOrEmpty(jzlsh))
                    return new object[] { 0, 0, "就诊流水号不能为空" };

                #region 判断是否医保登记
                SqlHelper helper = new SqlHelper();
                string strSql = string.Format("select * from ybmzzydjdr where jzlsh='{0}' and cxbz=1", jzlsh);
                DataSet ds = helper.ExecuteDataSet(strSql);
                if (ds.Tables[0].Rows.Count == 0)
                    return new object[] { 0, 0, "该患者未办理医保登记" };
                else
                {
                    bxh = ds.Tables[0].Rows[0]["bxh"].ToString();
                    xm = ds.Tables[0].Rows[0]["xm"].ToString();
                    kh = ds.Tables[0].Rows[0]["kh"].ToString();
                    dqbh = ds.Tables[0].Rows[0]["dqbh"].ToString();
                    string dgysdm = ds.Tables[0].Rows[0]["dgysdm"].ToString();
                    dgysxm = ds.Tables[0].Rows[0]["dgysxm"].ToString();
                    ybjzlsh = ds.Tables[0].Rows[0]["ybjzlsh"].ToString();
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
                string SN = "";
                strSql = string.Format(@"select b.ybxmbh,b.ybxmmc,a.z3djxx as dj,case LEFT(a.z3endv,1) when '4' then -a.z3jzcs else a.z3jzcs end as sl,
                                    case LEFT(a.z3endv,1) when '4' then -a.z3jzje else a.z3jzje end as je,a.z3mbno as yyxmbh, a.z3item as yyxmmc,
                                    a.z3empn as ysdm, a.z3kdys as ysxm,a.z3date as yysj,a.z3ksno as ksno, a.z3zxks as zxks, z3sfno as sfno, 
                                    b.sfxmzldm as ybsfxmzldm, b.sflbdm as ybsflbdm,b.sfxmdjdm as xmlx,a.z3ghno+a.z3sequ as sn from zy03d a left join ybhisdzdr b on 
                                    a.z3mbno=b.hisxmbh where a.z3ybup is null and LEFT(a.z3kind,1)=2  and a.z3zyno='{0}'", jzlsh);
                ds.Tables.Clear();
                ds = helper.ExecuteDataSet(strSql);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    StringBuilder strMsg = new StringBuilder();
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        if (dr["ybxmbh"] == DBNull.Value)
                            WriteLog(sysdate + "  " + jzlsh + "处方每日上传: 项目代码：[" + dr["yyxmbh"].ToString() + "]，名称：[" + dr["yyxmmc"].ToString() + "]未对照，不能上传！");
                        //strMsg.Append("项目代码：[" + dr["yyxmbh"].ToString() + "]，名称：[" + dr["yyxmmc"].ToString() + "]未对照，不能上传！\r\n");
                        else
                        {
                            string ybxmbh = dr["ybxmbh"].ToString();
                            string ybxmmc = dr["ybxmmc"].ToString();
                            decimal dj = Convert.ToDecimal(dr["dj"]);
                            decimal sl = Convert.ToDecimal(dr["sl"]);
                            decimal je = Convert.ToDecimal(dr["je"]);
                            string yysj = Convert.ToDateTime(dr["yysj"].ToString()).ToString("yyyyMMdd");
                            string yyxmxx = dr["yyxmbh"].ToString() + "&" + dr["yyxmmc"].ToString();
                            StringBuilder inputData2 = new StringBuilder();
                            inputData2.Append(ybxmbh.Trim() + "|");
                            inputData2.Append(ybxmmc.Trim() + "|");
                            inputData2.Append(dj + "|");
                            inputData2.Append(sl + "|");
                            inputData2.Append(je + "|");
                            inputData2.Append(yysj + "|");
                            inputData2.Append(yyxmxx + ";");
                            li_inputData.Add(inputData2.ToString());
                            li_yyxmbh.Add(dr["yyxmbh"].ToString());
                            li_yyxmmc.Add(dr["yyxmmc"].ToString());
                            li_ybxmbh.Add(dr["ybxmbh"].ToString());
                            li_sn.Add(dr["sn"].ToString());
                            SN += "'" + dr["sn"].ToString() + "',";
                        }
                    }
                }
                else
                    return new object[] { 0, 0, "无费用明细" };
                #endregion

                List<string> liSQL = new List<string>();
                int iTemp = 0;
                #region 分段上传 每次50条
                foreach (string inputData3 in li_inputData)
                {
                    if (iTemp <= 50)
                    {
                        inputData.Append(inputData3);
                        iTemp++;
                    }
                    else
                    {
                        StringBuilder outData = new StringBuilder(102400);
                        StringBuilder retMsg = new StringBuilder(102400);

                        WriteLog(sysdate + "  " + jzlsh + " 进入住院费用明细上传(分段)...");
                        WriteLog(sysdate + "  入参|" + inputData.ToString());
                        int i = f_UserBargaingApply(Ywlx, inputData, outData, retMsg);
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
                                if (li_ybxmbh[j].Trim().Equals(ybxmbm.Trim()))
                                {
                                    strSql = string.Format(@"insert into ybcfmxscfhdr(jzlsh,jylsh,je,xm,kh,yyxmdm,yyxmmc,sysdate,ybjzlsh,ybcfh,ybxmbm,
                                                                        ybxmmc,sfxmdj,sflb,dj,sl,yyrq,scsj) values(
                                                                        '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}',
                                                                        '{11}','{12}','{13}','{14}','{15}','{16}','{17}')",
                                                            jzlsh, li_sn[j], je2, xm, kh, li_yyxmbh[j], li_yyxmmc[j], sysdate, jzlsh2, ybcfh, ybxmbm,
                                                            ybxmmc, xmdj, sflb, dj2, sl2, yyrq, scsj);
                                    liSQL.Add(strSql);
                                    WriteLog(sysdate + "  " + jzlsh + "上传处方明细-->" + li_yyxmbh[j] + "|" + li_yyxmmc[j] + "|" + s);
                                    break;
                                }
                            }
                        }

                        iTemp = 0;
                        inputData.Remove(0, inputData.Length);
                        inputData.Append(bxh + "|");
                        inputData.Append(xm + "|");
                        inputData.Append(kh + "|");
                        inputData.Append(dqbh + "|");
                        inputData.Append(ybjzlsh + "|");
                        inputData.Append(dgysxm + ";");
                    }
                }
                #endregion

                #region 明细不足50条时，一次性上传
                if (iTemp > 0)
                {
                    StringBuilder outData = new StringBuilder(102400);
                    StringBuilder retMsg = new StringBuilder(102400);
                    WriteLog(sysdate + "  " + jzlsh + " 进入住院费用明细上传(补传、一次性上传)...");
                    int i = f_UserBargaingApply(Ywlx, inputData, outData, retMsg);
                    if (i <= 0)
                    {
                        WriteLog(sysdate + "  " + jzlsh + " 进入住院费用明细上传失败|" + retMsg.ToString());
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
                            if (li_ybxmbh[j].Trim().Equals(ybxmbm.Trim()))
                            {
                                strSql = string.Format(@"insert into ybcfmxscfhdr(jzlsh,jylsh,je,xm,kh,yyxmdm,yyxmmc,sysdate,ybjzlsh,ybcfh,ybxmbm,
                                                    ybxmmc,sfxmdj,sflb,dj,sl,yyrq,scsj) values(
                                                    '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}',
                                                    '{11}','{12}','{13}','{14}','{15}','{16}','{17}')",
                                                        jzlsh, li_sn[j], je2, xm, kh, li_yyxmbh[j], li_yyxmmc[j], sysdate, jzlsh2, ybcfh, ybxmbm,
                                                        ybxmmc, xmdj, sflb, dj2, sl2, yyrq, scsj);
                                liSQL.Add(strSql);
                                WriteLog(sysdate + "  " + jzlsh + "上传处方明细-->" + li_yyxmbh[j] + "|" + li_yyxmmc[j] + "|" + s);
                                break;
                            }
                        }
                    }
                }
                #endregion

                SN = SN.Substring(0, SN.Length - 1);
                strSql = string.Format(@"update zy03d set z3ybup = '{0}' where z3ybup is null and LEFT(z3kind,1)=2 and z3zyno = '{1}' and z3ghno+z3sequ in({2}) ", ybjzlsh, jzlsh, SN);
                liSQL.Add(strSql);
                string errorMsg = string.Empty;
                bool bFalg = helper.ExecuteSqlTran(liSQL, ref errorMsg);
                if (bFalg)
                {
                    WriteLog(sysdate + "  " + jzlsh + " 进入住院费用明细上传成功｜操作本地数据库成功|");
                    return new object[] { 0, 1, "住院收费登记成功" };
                }
                else
                {
                    WriteLog(sysdate + "  " + jzlsh + " 进入住院费用明细上传成功｜操作本地数据库失败|" + errorMsg);
                    object[] objParam2 = new object[] { jzlsh };
                    object[] objReturn = NYBZYSFQBTF(objParam2);
                    if (objReturn[1].ToString().Equals("1"))
                        return new object[] { 0, 2, "住院收费登记失败|操作本地数据库失败|住院收费登记撤销成功|" };
                    else
                        return new object[] { 0, 0, "住院收费登记失败|操作本地数据库失败|住院收费登记撤销失败|" };
                }
            }
            catch (Exception ex)
            {
                WriteLog(sysdate + "  住院收费登记每日上传异常|" + ex.Message);
                return new object[] { 0, 0, "住院收费登记每日上传异常|" + ex.Message };
            }
        }
        #endregion

        #region 住院收费退费(全部)
        public static object[] YBZYSFQBTF(object[] objParam)
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

                SqlHelper helper = new SqlHelper();
                string strSql = string.Format("select * from ybmzzydjdr where jzlsh='{0}' and cxbz=1", jzlsh);
                DataSet ds = helper.ExecuteDataSet(strSql);
                if (ds.Tables[0].Rows.Count == 0)
                {
                    return new object[] { 0, 0, "该患者未办理医保登记" };
                }
                else
                {
                    bxh = ds.Tables[0].Rows[0]["bxh"].ToString();
                    xm = ds.Tables[0].Rows[0]["xm"].ToString();
                    kh = ds.Tables[0].Rows[0]["kh"].ToString();
                    dqbh = ds.Tables[0].Rows[0]["dqbh"].ToString();
                    ybjzlsh = ds.Tables[0].Rows[0]["ybjzlsh"].ToString();
                }
                strSql = string.Format("select * from ybcfmxscfhdr where jzlsh='{0}' and ybjzlsh='{1}' and cxbz=1", jzlsh, ybjzlsh);
                ds.Tables.Clear();
                ds = helper.ExecuteDataSet(strSql);
                if (ds.Tables[0].Rows.Count == 0)
                    return new object[] { 0, 0, "无上传费用明细" };

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
                    strSql = string.Format(@"insert into ybcfmxscfhdr(jzlsh,je,xm,kh,yyxmdm,yyxmmc,ybjzlsh,ybcfh,ybxmbm,ybxmmc,
                                                    sfxmdj,sflb,dj,sl,yyrq,scsj,cxbz,sysdate,jylsh) select 
                                                    jzlsh,je,xm,kh,yyxmdm,yyxmmc,ybjzlsh,ybcfh,ybxmbm,ybxmmc,
                                                    sfxmdj,sflb,dj,sl,yyrq,scsj,0,'" + sysdate + "',jylsh from ybcfmxscfhdr where jzlsh='{0}' and ybjzlsh='{1}' and cxbz=1 ", jzlsh, ybjzlsh);
                    liSQL.Add(strSql);
                    strSql = string.Format("update ybcfmxscfhdr set cxbz=2 where jzlsh='{0}' and ybjzlsh='{1}' and cxbz=1", jzlsh, ybjzlsh);
                    liSQL.Add(strSql);
                    strSql = string.Format("update zy03d set z3ybup = null where z3ybup is not null and z3zyno = '{0}'", jzlsh);
                    liSQL.Add(strSql);
                    string errorMsg = string.Empty;
                    bool bFalg = helper.ExecuteSqlTran(liSQL, ref errorMsg);
                    if (bFalg)
                    {
                        WriteLog(sysdate + "  " + jzlsh + " 进入住院费用明细撤销(全部)成功|" + outData.ToString());
                        return new object[] { 0, 1, "住院收费退费成功", outData.ToString() };
                    }
                    else
                    {
                        WriteLog(sysdate + "  " + jzlsh + " 进入住院费用明细撤销(全部)成功|操作本地数据失败|" + errorMsg);
                        return new object[] { 0, 0, "住院收费退费失败|" + errorMsg };
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
                WriteLog(sysdate + "  住院费用明细撤销(全部)异常|" + ex.Message);
                return new object[] { 0, 0, "住院费用明细撤销(全部)异常|" + ex.Message };
            }
        }
        #endregion

        #region 住院收费退费(内部)
        public static object[] NYBZYSFQBTF(object[] objParam)
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

                SqlHelper helper = new SqlHelper();
                string strSql = string.Format("select * from ybmzzydjdr where jzlsh='{0}' and cxbz=1", jzlsh);
                DataSet ds = helper.ExecuteDataSet(strSql);
                bxh = ds.Tables[0].Rows[0]["bxh"].ToString();
                xm = ds.Tables[0].Rows[0]["xm"].ToString();
                kh = ds.Tables[0].Rows[0]["kh"].ToString();
                dqbh = ds.Tables[0].Rows[0]["dqbh"].ToString();
                ybjzlsh = ds.Tables[0].Rows[0]["ybjzlsh"].ToString();
              

                StringBuilder inputData = new StringBuilder();
                //入参:保险号|姓名|卡号|地区编号|住院号
                inputData.Append(bxh + "|");
                inputData.Append(xm + "|");
                inputData.Append(kh + "|");
                inputData.Append(dqbh + "|");
                inputData.Append(ybjzlsh + ";");
                StringBuilder outData = new StringBuilder(1024);
                StringBuilder retMsg = new StringBuilder(1024);
                WriteLog(sysdate + "  " + jzlsh + " 进入住院费用明细撤销(内部)...");
                int i = f_UserBargaingApply(Ywlx, inputData, outData, retMsg);
                if (i > 0)
                {
                    WriteLog(sysdate + "  " + jzlsh + " 进入住院费用明细撤销(内部)成功|" + outData.ToString());
                    return new object[] { 0, 1, "住院收费退费成功", outData.ToString() };
                }
                else
                {
                    WriteLog(sysdate + "  " + jzlsh + " 进入住院费用明细撤销(内部)失败|" + retMsg.ToString());
                    return new object[] { 0, 0, "住院收费退费失败|" + retMsg.ToString() };
                }
            }
            catch (Exception ex)
            {
                WriteLog(sysdate + "  住院费用明细撤销(内部)异常|" + ex.Message);
                return new object[] { 0, 0, "住院费用明细撤销(内部)异常|" + ex.Message };
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
