using Srvtools;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using System.Xml.Linq;
using ybinterface_lib.SY.Config;
using ybinterface_lib;

namespace ybinterface_lib
{
    public static class Functions
    {
        public static List<ybinterface_lib.SY.Config.Item> getXmlConfig(string path)
        {
            List<ybinterface_lib.SY.Config.Item> list = new List<ybinterface_lib.SY.Config.Item>();
            try
            {
                XElement xElement = XElement.Load(path);
                ybinterface_lib.SY.Config.Item item = item = new ybinterface_lib.SY.Config.Item();
                foreach (XElement item2 in xElement.Elements("appSettings").Elements("add"))
                {
                    item.TableName = item2.Attribute("key").Value.Trim();
                    if (item.TableName.Equals("DbHelper"))
                    {
                        item.SrcCon = item2.Attribute("value").Value;
                    }

                    if (item.TableName.Equals("DDYLJGBH"))
                    {
                        item.DDYLJGBH = item2.Attribute("value").Value;
                    }

                    if (item.TableName.Equals("DDYLJGMC"))
                    {
                        item.DDYLJGMC = item2.Attribute("value").Value;
                    }

                    if (item.TableName.Equals("YBIP"))
                    {
                        item.YBIP = item2.Attribute("value").Value;
                    }

                    if (item.TableName.Equals("YBPORT"))
                    {
                        item.YBPORT = item2.Attribute("value").Value;
                    }

                    if (item.TableName.Equals("TIMEOUT"))
                    {
                        item.TIMEOUT = item2.Attribute("value").Value;
                    }
                    if (item.TableName.Equals("Url"))
                    {
                        item.YBServiceUrl = item2.Attribute("value").Value;
                    }
                    if (item.TableName.Equals("AreaCode"))
                    {
                        item.YBJYYBQH = item2.Attribute("value").Value;
                    }
                    if (item.TableName.Equals("JSFDM"))
                    {
                        item.JSFDM = item2.Attribute("value").Value;
                    }
                }

                list.Add(item);
            }
            catch (Exception)
            {
            }

            return list;
        }
        public static object[] ybs_interface(string param1, object[] param2)
        {
            Logs.WriteLog("开始医保交易........|交易代码：" + param1 + " ");
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            object[] array = null;
            string empty = string.Empty;
            string empty2 = string.Empty;
            string empty3 = string.Empty;
            string empty4 = string.Empty;
            string fLoginYbNo = CliUtils.fLoginYbNo;
            try
            {
                empty2 = "ybinterface_lib.dll";
                if (string.IsNullOrEmpty(param1))
                {
                    return new object[3]
                    {
                        0,
                        0,
                        "业务代码不能为空|"
                    };
                }

                if (string.IsNullOrEmpty(fLoginYbNo))
                {
                    return new object[3]
                    {
                        0,
                        0,
                        "医保未连接或未初始化"
                    };
                }

                empty3 = fLoginYbNo.Split('|')[1].ToString();
                empty = Enum.GetName(typeof(YBYWDM), int.Parse(param1));
                if (string.IsNullOrEmpty(empty))
                {
                    MessageBox.Show("业务代码错误或业务未启用|");
                    return new object[3]
                    {
                        0,
                        0,
                        "业务代码错误或业务未启用|"
                    };
                }

                string path = AppDomain.CurrentDomain.BaseDirectory + "Solution1\\" + empty2;
                if (!System.IO.Directory.Exists(path))
                {
                    path = AppDomain.CurrentDomain.BaseDirectory + empty2;
                }
                Assembly assembly = Assembly.LoadFile(path);
                Type type = assembly.GetType("ybinterface_lib." + empty3);
                MethodInfo method = type.GetMethod(empty);
                object obj = method.Invoke(null, new object[1]
                {
                    param2
                });
                array = (object[])obj;
            }
            catch (Exception ex)
            {
                MessageBox.Show("运行出错|" + ex.Message);
                return new object[3]
                {
                    0,
                    0,
                    "运行出错|" + ex.Message
                };
            }

            stopwatch.Stop();
            Logs.WriteLog("交易结束........|用时" + stopwatch.Elapsed.TotalMilliseconds + "毫秒");
            return array;
        }
    }
}
