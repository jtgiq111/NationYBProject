using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace ybinterface_auto
{
    public class ybinterface_lib_yt
    {

        #region 变量
        internal static string YWBH = string.Empty; //业务编号
        internal static string CZYBH = string.Empty;//操作员工号
        internal static string YWZQH = string.Empty;//业务周期号
        internal static string JYLSH = string.Empty;//交易流水号
        internal static string ZXBM = "0000";       //中心编码
        internal static string LJBZ = "1";          //联机标志
        internal static string YBKLX = "0";                  //医保卡类型    0:正式卡，1：临时卡
        internal static string DQJBBZ = "1";        //地区级别标志 1-本市级 2-本省异地  3-其他
        internal static string DKXX = "";

        static IWork iWork = new Work();
        static string xmlPath = AppDomain.CurrentDomain.BaseDirectory;
        static List<Item1> lItem = iWork.getXmlConfig1(xmlPath + "EEPNetClient.exe.config");
        internal static string YLGHBH = lItem[0].DDYLJGBH; //医疗机构编号
        internal static string DDYLJGMC = lItem[0].DDYLJGMC;//医院名称
        internal static string YBIP = lItem[0].YBIP;        //医保IP地址
        #endregion

        #region 医保dll函数声明
        [DllImport("SiInterface.dll", EntryPoint = "INIT", CharSet = CharSet.Ansi)]
        static extern int INIT(StringBuilder pErrMsg);

        [DllImport("SiInterface.dll", EntryPoint = "BUSINESS_HANDLE", CharSet = CharSet.Ansi)]
        static extern int BUSINESS_HANDLE(StringBuilder inputData, StringBuilder outputData);
        #endregion

        #region 初始化
        public static object[] YBINIT(object[] objParam)
        {
            CZYBH = objParam[0].ToString();  //用户工号
            string sysdate = DateTime.Now.ToString("yyyyMMddHHmmss"); //获取系统时间

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
                return new object[] { 0, 1, YWZQH };
            }
            else
            {
                WriteLog(sysdate + "  用户" + CZYBH + " 进入医保签到失败|" + outputData.ToString());
                return new object[] { 0, 0, outputData.ToString().Split('^')[2].ToString() };
            }
        }
        #endregion

        #region 医保退出
        public static object[] YBEXIT(object[] objParam)
        {
            string sysdate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
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

        #region 住院费用登记
        public static object[] YBZYSFDJ(object[] objParam)
        {
            string sysdate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            WriteLog(sysdate + "  进入住院费用上传...");
            try
            {
                string jzlsh = objParam[0].ToString();  //就诊流水号
                string ztjssj = objParam[1].ToString(); //中途结算时间
                string scrow = objParam[2].ToString();  //上传条数
                string cfrqbz = objParam[3].ToString(); //上传处方日期标志 0-正常处方日期，1-出院日期
                CZYBH = objParam[4].ToString();  //操作员工号
                YWZQH = objParam[5].ToString();  //业务周期号
                string jbr = objParam[6].ToString(); //经办人
                ZXBM = "0000";
                string cfsj = Convert.ToDateTime(sysdate).ToString("yyMMddHHmmss");
                string ztjssj1 = "";
                double scfy = 0.0000;
                string scfy1="";
                if (!string.IsNullOrEmpty(ztjssj))
                {
                    ztjssj1 = Convert.ToDateTime(ztjssj).AddDays(1).ToString("yyyy-MM-dd");
                }

                #region 判断是否医保登记
                string strSql = string.Format("select a.*,b.z1ldat from ybmzzydjdr a left join zy01h b on a.jzlsh=b.z1zyno where a.jzlsh = '{0}' and jzbz='z' and a.cxbz = 1", jzlsh);
                SqlHelper helper = new SqlHelper();
                DataSet ds = helper.ExecuteDataSet(strSql);
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
                #endregion

                #region 上传前先测试已上传费用
                object[] objParam1 = { jzlsh, CZYBH, YWZQH, jbr };
                YBZYSFDJCX(objParam1);
                #endregion

                #region 获取费用明细信息
                strSql = string.Format(@"select y.ybxmbh, y.ybxmmc, a.z3djxx as dj,a.z3yzxh 
                                        , sum(case left(a.z3endv, 1) when '4' then -a.z3jzcs else a.z3jzcs end) as sl
                                        , sum(case left(a.z3endv, 1) when '4' then -a.z3jzje else a.z3jzje end) as je
                                        , a.z3item as yyxmbh, a.z3name as yyxmmc, min(a.z3empn) as ysdm, min(a.z3kdys) as ysxm
                                        , z3sfno as sfno, y.sfxmzldm as ybsfxmzldm, y.sflbdm as ybsflbdm,max(a.z3date) as yysj,y.sfxmdjdm
                                        from zy03d a 
                                        left join ybhisdzdr y on a.z3item = y.hisxmbh 
                                        where a.z3ybup is null and left(a.z3kind, 1) in ('2', '4') and isnull(a.z3jshx,'')=''  and a.z3zyno = '{0}' ", jzlsh);
                if (!string.IsNullOrEmpty(ztjssj))
                    strSql += string.Format(@"and Convert(datetime,z3date)<'{0}' ", ztjssj1);
                strSql += string.Format(@"group by y.ybxmbh, y.ybxmmc, a.z3djxx, a.z3name, a.z3item,a.z3yzxh  
                                        ,z3sfno,y.sfxmzldm, y.sflbdm,y.sfxmdjdm 
                                        having sum(case left(a.z3endv, 1) when '4' then -a.z3jzje else a.z3jzje end) > 0
                                        order by yysj ");
                ds.Tables.Clear();
                ds = helper.ExecuteDataSet(strSql);

                List<string> liSQL = new List<string>();
                List<string> li_inputData = new List<string>();
                List<string> liyyxmbh = new List<string>();
                List<string> liyyxmmc = new List<string>();
                List<string> liybxmbm = new List<string>();
                List<string> liybxmmc = new List<string>();

                string strSQL1 = string.Empty;
                List<string> liErr = new List<string>();
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
                            string yzxh = dr["z3yzxh"].ToString();
                            double dj = Convert.ToDouble(dr["dj"]);
                            double sl = Convert.ToDouble(dr["sl"]);
                            double je = Convert.ToDouble(dr["je"]);
                            scfy += je;
                            string fysj = Convert.ToDateTime(dr["yysj"].ToString()).ToString("yyyyMMddHHmmss");
                            string gg = "";
                            string jx = "";
                            string jldw = "";
                            string yfyl = "";
                            string ybcfh = jzlsh.Substring(4,4)+DateTime.Now.ToString("yyMMddHHmmss")  + index.ToString();
                            string ypjldw = "";
                            decimal mcyl = 1;
                            string pd = "";
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
                                ypjldw = "";
                            }
                            index++;
                            #region 入参
                            /*
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
                                inputParam.Append(ybjzlsh_snyd + "|");
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
                    {
                        strSQL1 = string.Format(@"delete from ybzdscrz where jzlsh='{0}'", jzlsh);
                        liErr.Add(strSQL1);
                        strSQL1 = string.Format(@"insert into ybzdscrz(jzlsh,errmsg) values('{0}','{1}')", jzlsh, strMsg.ToString());
                        liErr.Add(strSQL1);
                        string errMsg1 = "";
                        helper.ExecuteSqlTran(liErr, ref errMsg1);
                        //return new object[] { 0, 0, strMsg.ToString(), strMsg.ToString() };
                    }
                    scfy1 = scfy.ToString("0.0000");
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
                        JYLSH = cfsj + "-" + YLGHBH + "-" + new Random().Next(100).ToString().PadLeft(4, '0');
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

                        //WriteLog(sysdate + "  住院费用上传(分段)|入参|" + inputData1.ToString());
                        int i = BUSINESS_HANDLE(inputData1, outData);
                        if (i != 0)
                        {
                            strSQL1 = string.Format(@"delete from ybzdscrz where jzlsh='{0}'", jzlsh);
                            liErr.Add(strSQL1);
                            strSQL1 = string.Format(@"insert into ybzdscrz(jzlsh,errmsg) values('{0}','{1}')", jzlsh, outData.ToString().Split('^')[2]);
                            liErr.Add(strSQL1);
                            string errMsg1 = "";
                            helper.ExecuteSqlTran(liErr, ref errMsg1);

                            WriteLog(sysdate + "  " + jzlsh + " 住院费用上传(分段)失败|" + outData.ToString());
                            return new object[] { 0, 0, "住院费用上传(分段)失败|" + outData.ToString().Split('^')[2], outData.ToString().Split('^')[2] };
                        }
                        WriteLog(sysdate + "  住院费用上传(分段)|出参|" + outData.ToString());
                        //20181012143357-0002-277^1^^
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
                    JYLSH = cfsj + "-" + YLGHBH + "-" + new Random().Next(100).ToString().PadLeft(4, '0');
                    StringBuilder outData = new StringBuilder(102400);
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

                    //WriteLog(sysdate + " 住院费用上传(补传、一次性上传)|入参|" + inputData1.ToString());
                    int i = BUSINESS_HANDLE(inputData1, outData);
                    if (i != 0)
                    {
                        strSQL1 = string.Format(@"delete from ybzdscrz where jzlsh='{0}'", jzlsh);
                        liErr.Add(strSQL1);
                        strSQL1 = string.Format(@"insert into ybzdscrz(jzlsh,errmsg) values('{0}','{1}')", jzlsh, outData.ToString().Split('^')[2]);
                        liErr.Add(strSQL1);
                        string errMsg1 = "";
                        helper.ExecuteSqlTran(liErr, ref errMsg1);
                        WriteLog(sysdate + "  " + jzlsh + " 住院费用上传(补传、一次性上传)失败|" + outData.ToString());
                        return new object[] { 0, 0, "住院费用上传(补传、一次性上传)失败|" + outData.ToString().Split('^')[2], outData.ToString().Split('^')[2] };

                    }
                    //WriteLog(sysdate + " 住院费用上传(补传、一次性上传)|出参|" + outData.ToString());

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
                string errMsg = "";
                bool bFalg = helper.ExecuteSqlTran(liSQL, ref errMsg);

                if (bFalg)
                {
                    WriteLog(sysdate + "  住院费用上传成功|本地数据操作成功");
                    return new object[] { 0, 1, "费用上传成功", scfy };
                }
                else
                {
                    WriteLog(sysdate + "    住院费用上传成功|本地数据操作失败|" + errMsg);
                    return new object[] { 0, 0, "住院费用上传失败|" + errMsg, errMsg };
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
            string sysdate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            WriteLog(sysdate + "  进入住院费用登记撤销...");
            try
            {
                string jzlsh = objParam[0].ToString(); // 就诊流水号
                CZYBH = objParam[1].ToString(); // 操作员工号 
                YWZQH = objParam[2].ToString(); // 业务周期号
                string jbr = objParam[3].ToString();   // 经办人姓名
                ZXBM = "0000";
                JYLSH = DateTime.Now.ToString("yyyyMMddHHmmss") + "-" + YLGHBH + "-" + new Random().Next(100).ToString().PadLeft(4, '0');

                //是否存在撤销明细
                //string strSql = string.Format(@"select * from ybcfmxscindr a where a.jzlsh = '{0}' and a.cxbz = 1 and isnull(ybdjh,'')=''", jzlsh);
                //SqlHelper helper = new SqlHelper();
                //DataSet ds = helper.ExecuteDataSet(strSql); 
                //if (ds != null && ds.Tables[0].Rows.Count == 0)
                //    return new object[] { 0, 0, "无费用撤销或已经撤销完成" };

                #region 医保登记信息
                string strSql = string.Format("select * from ybmzzydjdr a where a.jzlsh = '{0}' and jzbz='z' and a.cxbz = 1", jzlsh);
                SqlHelper helper = new SqlHelper();
                DataSet ds = helper.ExecuteDataSet(strSql); 
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
                    strSql = string.Format("update zy03d set z3ybup = null where z3ybup is not null and isnull(z3fphx,'')='' and z3zyno = '{0}'", jzlsh);
                    liSql.Add(strSql);
                    string errMsg = "";
                    bool bFalg = helper.ExecuteSqlTran(liSql, ref errMsg);

                    if (bFalg)
                    {
                        WriteLog(sysdate + "  住院费用登记撤销成功|本地数据操作成功|");
                        return new object[] { 0, 1, "住院费用登记撤销成功" };
                    }
                    else
                    {
                        WriteLog(sysdate + "  住院费用登记撤销成功|本地数据操作失败|" + errMsg);
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
            string sysdate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            WriteLog(sysdate + "  进入住院收费预结算...");
            try
            {
                string jzlsh = objParam[0].ToString();      // 就诊流水号
                string cyyy = objParam[1].ToString();       //出院原因
                string zhsybz = objParam[2].ToString();     //账户使用标志 （0或1）
                string ztjsbz = objParam[3].ToString();     //中途结算标志
                string jsrqsj = objParam[4].ToString();     //结算日期时间
                string cyrqsj = objParam[5].ToString();     //出院日期时间
                string sfje = objParam[6].ToString();       //收费金额  
                CZYBH = objParam[7].ToString();   // 操作员工号
                YWZQH = objParam[8].ToString();   // 业务周期号
                string jbr = objParam[9].ToString();     // 经办人姓名
                ZXBM = "0000";
                string grbh = "";
                string djh = "0000";
                if (string.IsNullOrEmpty(jzlsh))
                    return new object[] { 0, 0, "就诊流水号不能为空" };
                if (string.IsNullOrEmpty(cyyy))
                    return new object[] { 0, 0, "出院原因不能为空" };

                jsrqsj = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                WriteLog(sysdate + "  " + jzlsh + " 进入住院收费预结算...");

              
                string cyrq = "";
                string jsrq = Convert.ToDateTime(jsrqsj).ToString("yyyyMMddHHmmss");
                string dqrq = Convert.ToDateTime(sysdate).ToString("yyyyMMddHHmmss");  // 当前日期
                if (ztjsbz.Equals("1"))
                    cyrq = "";  //出院日期
                else
                    cyrq = Convert.ToDateTime(cyrqsj).ToString("yyyyMMddHHmmss");
                string sslxdm = "0";    //手术类型代码  

                JYLSH = dqrq + "-" + YLGHBH + "-" + new Random().Next(100).ToString().PadLeft(4, '0');
                

                #region 是否未办理医保登记
                string strSql = string.Format("select * from ybmzzydjdr where jzlsh='{0}' and cxbz=1", jzlsh);
                SqlHelper helper = new SqlHelper();
                DataSet ds = helper.ExecuteDataSet(strSql); 
                if (ds.Tables[0].Rows.Count == 0)
                    return new object[] { 0, 0, "该患者未办理医保登记" };

                DataRow dr = ds.Tables[0].Rows[0];
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

                #region 是否已经医保结算
                strSql = string.Format(@"select * from ybfyjsdr where jzlsh='{0}' and ztjsbz=0 and cxbz=1", jzlsh);
                ds.Tables.Clear();
                ds = helper.ExecuteDataSet(strSql); 
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
                StringBuilder outputData = new StringBuilder(10240);
                int i = BUSINESS_HANDLE(inputData, outputData);
                WriteLog(sysdate + "  住院收费预结算|出参|" + outputData.ToString());
                List<string> liSQL = new List<string>();
                if (i != 0)
                {
                    List<string> liErr = new List<string>();
                    string strSQL1 = string.Format(@"delete from ybzdscrz where jzlsh='{0}'", jzlsh);
                    liErr.Add(strSQL1);
                    strSQL1 = string.Format(@"insert into ybzdscrz(jzlsh,errmsg) values('{0}','{1}')", jzlsh, outputData.ToString().Split('^')[2]);
                    liErr.Add(strSQL1);
                    string errMsg1 = "";
                    helper.ExecuteSqlTran(liSQL, ref errMsg1);
                    WriteLog(sysdate + "  住院收费预结算失败|" + outputData.ToString());
                    return new object[] { 0, 0, outputData.ToString() };
                }
                #endregion

                #region 出参
                string[] sfjsfh = outputData.ToString().Split('^')[2].Split('|');
                outParams_js js = new outParams_js();
                /*
                1	    医疗费总额	VARCHAR2(16)		2位小数
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
                54		二类门慢限额支出（景德镇） 门慢限额支出（省本级）	VARCHAR2(16)		【景德镇】【省本级】专用
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
                    /*非建档立卡人员（含民政救助对象）住院（含门诊慢性病治疗）的个人实际自付应为：
                     * 大病统筹gYBdbtczfje+居民大病补偿gYBjmdbbcje+同级财政补偿gYBtjczbchj+建档大病费用gYBjddbhj+建档二次补偿gYBjdecbchj+商业补充保险gYBsybcbxhj。
                     */
                    js.Ybxjzf = (Convert.ToDecimal(js.Xjzf) + Convert.ToDecimal(js.Dejjzf) + Convert.ToDecimal(js.Jmdbydje) + Convert.ToDecimal(js.Jmdbedje)
                                 + Convert.ToDecimal(js.Zftdfy) + Convert.ToDecimal(js.Qybcylbxjjzf)
                                + Convert.ToDecimal(js.Jddbbcbcydzfje) + Convert.ToDecimal(js.Jddbbcbcedzfje) + Convert.ToDecimal(js.Jddbbcbcsdzfje)
                                + Convert.ToDecimal(js.Jdecbcbcydzfje) + Convert.ToDecimal(js.Jdecbcbcedzfje) + Convert.ToDecimal(js.Jdecbcbcsdzfje)).ToString();
                    js.Zhzbxje = (Convert.ToDecimal(js.Ylfze) - Convert.ToDecimal(js.Ybxjzf)).ToString();
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

                //WriteLog(sysdate + "  住院收费预结算|整合出参|" + strValue);
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
                                        zhxjzffy,qtybfy,sysdate,zhzbxje,jbr,ztjsbz,ryrq,cyrq,cyyy)
                                        values('{0}','{1}','{2}','{3}','{4}','{5}' ,'{6}','{7}','{8}' ,'{9}' ,
                                        '{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}',
                                        '{20}','{21}','{22}','{23}','{24}','{25}','{26}','{27}','{28}','{29}',
                                        '{30}','{31}','{32}','{33}','{34}','{35}','{36}','{37}','{38}','{39}',
                                        '{40}','{41}','{42}','{43}','{44}','{45}','{46}','{47}','{48}','{49}',
                                        '{50}','{51}','{52}','{53}','{54}','{55}','{56}','{57}','{58}','{59}',
                                        '{60}','{61}','{62}','{63}','{64}','{65}','{66}','{67}','{68}','{69}',
                                        '{70}','{71}','{72}','{73}','{74}','{75}','{76}','{77}','{78}','{79}',
                                        '{80}','{81}','{82}','{83}','{84}','{85}','{86}','{87}','{88}')",
                                        jzlsh, ybjzlsh, JYLSH, djh, js.Ylfze, js.Zbxje, js.Tcjjzf, js.Dejjzf, js.Zhzf, js.Xjzf,
                                        js.Gwybzjjzf, js.Qybcylbxjjzf, js.Zffy, js.Dwfdfy, js.Yyfdfy, js.Mzjzfy, js.Cxjfy, js.Ylzlfy, js.Blzlfy, js.Fhjjylfy,
                                        js.Qfbzfy, js.Zzzyzffy, js.Jrtcfy, js.Tcfdzffy, js.Ctcfdxfy, js.Jrdebxfy, js.Defdzffy, js.Cdefdxfy, js.Rgqgzffy, js.Bcjsqzhye,
                                        js.Bntczflj, js.Bndezflj, js.Bnczjmmztczflj, js.Bngwybzzflj, js.Bnzhzflj, js.Bnzycslj, js.Zycs, js.Xm, js.Kh, js.Jsrq,
                                        js.Yllb, js.Yldylb, js.Jbjgbm, js.Ywzqh, js.Jslsh, js.Tsxx, js.Djh, js.Yyjylsh, js.Grbhgl, js.Yljgbm,
                                        js.Ecbcje, js.Mmqflj, js.Jsfjylsh, js.Grbh, js.Czzcje, js.Elmmxezc, js.Elmmxesy, js.Jmecbc, js.Tjje, js.Syjjzf,
                                        js.Jmdbydje, js.Jmdbedje, js.Jbbcfwnfyzfje, js.Jbbcybbczcfywfyzf, js.Mgwxlcjjzf, js.Jjjmzcfwwkbxfy, js.Zftdfy, js.Mfmzjj, js.Jddbbcbcydzfje, js.Jddbbcbcedzfje,
                                        js.Jddbbcbcsdzfje, js.Jdecbcbcydzfje, js.Jdecbcbcedzfje, js.Jdecbcbcsdzfje, js.Jbbcbxbczcfwnfyydzfje, js.Jbbcbxbczcfwnfyedzfje, js.Bnzftdjjfylj, js.Lxgbdttcjjzf, js.Qtjjzf, js.Scryylbzjj,
                                        js.Ybxjzf, js.Qtybzf, sysdate, js.Zhzbxje, jbr, ztjsbz, ryrq, cyrq, cyyy);
                liSQL.Add(strSql);

                //预效金操作

                string errMsg = "";
                bool bFalg = helper.ExecuteSqlTran(liSQL, ref errMsg);
                if (bFalg)
                {
                    WriteLog(sysdate + "  住院收费预结算成功|本地数据操作成功|");
                    //object[] objtcj = new object[]{ jzlsh, sysdate, js.Zhzbxje };
                    //objtcj=YBSJHX(objtcj);
                    return new object[] { 0, 1, "住院费用预结算成功|" + js.Zhzbxje, js.Zhzbxje, js.Tcjjzf, js.Zhzf, js.Dejjzf, js.Xjzf };
                }
                else
                {
                    WriteLog(sysdate + "  住院收费预结算成功|本地数据操作失败|" + errMsg);
                    return new object[] { 0, 0, errMsg };
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

        #region 统筹担保金
        public static object[] YBSJHX(object[] objParam)
        {
            string strSql = "";
            string autoNum = string.Empty;
            DataSet ds = null;
            bool bfalg = false;
            try
            {
                WriteLog("进入医保实时统筹金...");
                #region 变量赋值
                string comp = "1";       //院别
                string zyno = objParam[0].ToString();       //住院号
                string z3date = objParam[1].ToString();     //记账日期
                string amnt = objParam[2].ToString();       //金额
                string ghno = string.Empty;       //住院流水号
                string kind = "3";       //收费类别
                string payw = "6:统筹金";       //支付方式
                string dat1 = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");  //系统日期
                string dat2 = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");  //系统日期
                string mark = "医保实时统筹金";      //备注
                string user = "001";      //操作员工号
                string endv = "3";      //状态
                string z3ipxx = "127.0.0.1";
                string z3area = string.Empty;//区域（南院 北院...）
                //string tcjjzf = objParam[3].ToString();
                //string zhzf = objParam[4].ToString();
                //string dejjzf = objParam[5].ToString();
                //string xjzf = objParam[6].ToString();
                SqlHelper helper = new SqlHelper();
                #endregion
                #region 读患者区域
                strSql = string.Format(@"select z1area,z1ghno from zy01h where z1comp='{0}' and z1zyno='{1}'", comp, zyno);
                ds = helper.ExecuteDataSet(strSql);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    z3area = ds.Tables[0].Rows[0]["z1area"].ToString();
                    ghno = ds.Tables[0].Rows[0]["z1ghno"].ToString();
                }
                else
                {
                    WriteLog("获取患者信息失败|不能自动更新统筹金");
                    return new object[] { 0, "获取患者信息失败|不能自动更新统筹金", autoNum };
                }
                #endregion

                int tsjh = 1;
                #region 判断是否有医保预结算统筹费用记录
                string beupdateamnt = string.Empty;
                bool isupdate = false;
                strSql = string.Format(@"select z3amnt from zy03da where z3comp='{0}' and left(z3kind,1)='3' and left(z3payw,1)='6' and z3zyno='{1}' and z3ghno='{2}'", comp, zyno, ghno);
                ds.Tables.Clear();
                ds = helper.ExecuteDataSet(strSql);
                if (ds.Tables[0].Rows.Count == 1)
                {
                    beupdateamnt = ds.Tables[0].Rows[0]["z3amnt"].ToString();
                    isupdate = true;
                }
                #endregion

                List<string> liSQL = new List<string>();
                if (isupdate)
                {
                    strSql = string.Format(@"update zy03da set z3amnt='{0}' where z3comp='{1}' and left(z3kind,1)='3' and left(z3payw,1)='6' and left(z3endv,1)='3' and z3zyno='{2}' and z3ghno='{3}';     
                                            update zy01h set z1amt3=z1amt3+{0}-{4},z1amtz=z1amtz+{0}-{4},z1amty=z1amty+{0}-{4} where z1zyno='{2}' and z1ghno='{3}'",
                                            amnt, comp, zyno, ghno, beupdateamnt);
                    liSQL.Add(strSql);
                    autoNum = comp + tsjh.ToString().PadLeft(7, '0');
                }
                else
                {
                    #region 插入zy03da
                    #region z3tsjh自动编号获取
                    strSql = string.Format(@"select * from SYSAUTONUM where AUTOID='z3tsjh' and FIXED='{0}' ", comp);
                    ds.Tables.Clear();
                    ds = helper.ExecuteDataSet(strSql);
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        tsjh = Convert.ToInt32(ds.Tables[0].Rows[0]["CURRNUM"].ToString()) + 1;
                        strSql = string.Format(@" update SYSAUTONUM set CURRNUM='{0}' where AUTOID='z3tsjh' and FIXED='{1}'", tsjh, comp);
                        liSQL.Add(strSql);
                    }
                    else
                    {
                        strSql = string.Format(@"insert into SYSAUTONUM ([AUTOID],[FIXED],[CURRNUM],[DESCRIPTION]) VALUES  ('z3tsjh','{0}','{1}','收据号自动编号')", comp, tsjh);
                        liSQL.Add(strSql);
                    }
                    autoNum = comp + tsjh.ToString().PadLeft(7, '0');
                    #endregion

                    strSql = string.Format(@"INSERT INTO zy03da(z3comp,z3zyno,z3ghno,z3kind,z3amnt,z3payw,z3date,z3mark,z3user,z3endv,z3tsjh,z3ipxx,z3area)
                                            VALUES ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}');
                                            update zy01h set z1amt3=z1amt3+{4},z1amtz=z1amtz+{4},z1amty=z1amty+{4} where z1zyno='{1}' and z1ghno='{2}' ",
                            comp, zyno, ghno, kind, amnt,
                            payw, z3date, mark, user, endv,
                            autoNum, z3ipxx, z3area);
                    liSQL.Add(strSql);

                    #endregion
                }
                string errMsg = "";
                bfalg = helper.ExecuteSqlTran(liSQL, ref errMsg);
                if (bfalg)
                {
                    WriteLog("自动更新医保统筹金:" + amnt);
                    return new object[] { 0, 1, "自动更新医保统筹金:" + amnt };
                }
                else
                {
                    WriteLog("自动更新医保统筹金失败|" + errMsg);
                    return new object[] { 0,0, errMsg, autoNum };
                }
            }
            catch (Exception e)
            {
                WriteLog("自动更新医保统筹金异常|" + e.Message);
                return new object[] { 0, 0, e.Message, "" };
            }
        }
        #endregion
    }
}
