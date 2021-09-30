using Srvtools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;

namespace ybinterface_lib
{
    public class yb_interface_nc_zr
    { 
        #region 变量
        internal static string YWBH = string.Empty; //业务编号
        internal static string CZYBH = string.Empty;//操作员工号
        internal static string YWZQH = string.Empty;//业务周期号
        internal static string JYLSH = string.Empty;//交易流水号
        internal static string ZXBM = "0000";       //中心编码
        internal static string LJBZ = "1";          //联机标志
        internal static string DQJBBZ = "1";        //地区级别标志 1-本市级 2-本省异地  3-其他

        static IWork iWork = new Work();
        static string xmlPath = AppDomain.CurrentDomain.BaseDirectory;
        static List<Item1> lItem = iWork.getXmlConfig1(xmlPath + "EEPNetClient.exe.config");
        internal static string YLGHBH = lItem[0].DDYLJGBH; //医疗机构编号
        internal static string DDYLJGMC = lItem[0].DDYLJGMC;//医院名称
        internal static string YBIP = lItem[0].YBIP;        //医保IP地址
        #endregion

        #region 初始化
        public static object[] YBINIT(object[] objParam)
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
            return new object[] { 0, 0, "测试" };
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
    }
}
