using System;
using System.Configuration;
using System.Reflection;
using System.Windows.Forms;
using Srvtools;
using System.Data;
using yb_interfaces.JX.Enum;

namespace yb_interfaces
{
    public class yb_interface
    {
        //private static DataTable dtYWDM = null;
        

        #region HIS前端调用业务代码方法
        /// <summary>
        /// 调用业务代码方法
        /// </summary>
        /// <param name="param1">业务代码</param>
        /// <param name="param2">入参</param>
        /// <returns></returns>
        public static object[] ybs_interface(string param1, object[] param2)
        {
            object[] returnOBJ = null;
            string dqdm = string.Empty; //地区代码
            string csdm = string.Empty; //厂商代码
            string ywdm = string.Empty; //业务代码
            string ffmc = string.Empty; //方法名称
            string dllmc = string.Empty;//DLL名称
            string lmc = string.Empty;//类名称
            string mmkj = string.Empty; //命名空间
            string ybconfig = CliUtils.fLoginYbNo; 
            try
            {
                if (string.IsNullOrEmpty(param1))
                {
                    MessageBox.Show("业务代码不能为空|");
                    return new object[] { 0, 0, "业务代码不能为空|" };
                }

                if (string.IsNullOrEmpty(ybconfig))
                {
                    //lmc = "yb_interface_ts_dr";
                    return new object[] { 0, 0, "医保未连接或未初始化" };
                }
                else
                {
                    //ybconfig格式   医保返回业务周期号|地区医保类名称
                    lmc = ybconfig.Split('|')[1].ToString();
                }
                dllmc = "yb_interfaces.dll"; //动态库名称固定不做变动

                #region 判断医保业务代码是否有效
                ffmc = Enum.GetName(typeof(YBYWDM), int.Parse(param1));

                if (string.IsNullOrEmpty(ffmc))
                {
                    MessageBox.Show("业务代码错误或业务未启用|");
                    return new object[] { 0, 0, "业务代码错误或业务未启用|" };
                }
                else
                {
                    string path = AppDomain.CurrentDomain.BaseDirectory + "Solution1\\" + dllmc;
                    Assembly asm = Assembly.LoadFile(path);
                    Type type = asm.GetType("yb_interfaces." + lmc);
                    MethodInfo method = type.GetMethod(ffmc);
                    object obj = method.Invoke(null, new object[] { param2 });
                    returnOBJ = (object[])obj;
                }
                #endregion
            }
            catch (Exception ex)
            {
                MessageBox.Show("运行出错|" + ex.Message);
                return new object[] { 0, 0, "运行出错|" };
            }
            return returnOBJ;
        }
        #endregion

    }
}
