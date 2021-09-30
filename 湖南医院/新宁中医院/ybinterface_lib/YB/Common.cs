using Srvtools;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Windows.Forms;
using System.Xml.Linq;

namespace ybinterface_lib
{
    class Common
    {
        #region 写医保日志
        public static void WriteYBLog(string data)
        {
            if (!Directory.Exists("YBLog"))
            {
                Directory.CreateDirectory("YBLog");
            }

            string fileName = @"YBLog\" + DateTime.Now.ToString("yyyyMMdd") + ".txt";
            StreamWriter sw = new StreamWriter(fileName, true);
            sw.WriteLine(data);
            sw.Close();
        }
        #endregion

        #region 写日志
        /// <summary>
        /// 写日志
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <param name="data">内容</param>
        public static void WriteLog(string fileName, string data)
        {
            StreamWriter sw = new StreamWriter(fileName, false);
            sw.WriteLine(data);
            sw.Close();
        }
        #endregion

        #region 全角数字变半角数字
        /// <summary>
        /// 全角数字变半角数字
        /// </summary>
        /// <param name="quan">全角数字</param>
        /// <returns>半角数字</returns>
        public static char QuanBianBan(char quan)
        {
            switch (quan)
            {
                case '０':
                    return '0';

                case '１':
                    return '1';

                case '２':
                    return '2';

                case '３':
                    return '3';

                case '４':
                    return '4';

                case '５':
                    return '5';

                case '６':
                    return '6';

                case '７':
                    return '7';

                case '８':
                    return '8';

                case '９':
                    return '9';

                default:
                    return '-';
            }
        }
        #endregion

        /// <summary>
        /// 读取app.config
        /// </summary>
        /// <param name="path"></param>
        /// <returns>List<Item></returns>
        public static Item GetAppConfig(string path)
        {
            Item item = new Item();

            try
            {
                var xml = XElement.Load(path);

                foreach (var t in xml.Elements("appSettings").Elements("add"))
                {
                    string key = t.Attribute("key").Value;

                    if (key.Equals("DDYLJGBH"))
                    {
                        item.DDYLJGBH = t.Attribute("value").Value;
                    }
                    else if (key.Equals("DDYLJGMC"))
                    {
                        item.DDYLJGMC = t.Attribute("value").Value;
                    }
                    else if (key.Equals("YBIP"))
                    {
                        item.YBIP = t.Attribute("value").Value;
                    }
                    else if (key.Equals("Gocent"))
                    {
                        item.Gocent = t.Attribute("value").Value;
                    }
                }
            }
            catch (Exception error)
            {
                WriteLog("GetAppConfig.txt", error.Message);
            }

            return item;
        }

        #region 写医保日志数据库
        /// <summary>
        /// 写医保日志数据库
        /// </summary>
        /// <param name="jzlsh">流水号/单据号</param>
        /// <param name="rc">入参</param>
        /// <param name="cc">出参</param>
        public static void InsertYBLog(string jzlsh, string rc, string cc)
        {
            string sql = string.Format(@"insert into ybrzwx(jzlsh, rc, cc, date) values('{0}', '{1}', '{2}', '{3}')", jzlsh, rc, cc, GetServerTime());
            object[] obj = new object[] { sql };

            try
            {
                obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);
            }
            catch (Exception error)
            {
                MessageBox.Show(error.ToString());
            }
        }
        #endregion 写医保日志数据库

        #region 取出服务器日期时间
        public static string GetServerTime()
        {
            object[] myDateTime = CliUtils.CallMethod("GLModule", "GetServerTime", new object[] { });
            //return (string)myDateTime[3];    //yyyy-MM-dd  
            //return (string)myDateTime[4];   // hh:mm:ss   
            return (string)myDateTime[5];     //yyyy-MM-dd HH:mm:ss
        }
        #endregion

        public static DataSet GetDataSetFromDataGridView(DataGridView gridView)
        {
            DataSet ds = new DataSet();
            DataTable dt = new DataTable("cmd");
            List<DataGridViewColumn> columns = new List<DataGridViewColumn>();
            DataGridViewColumn col = gridView.Columns.GetFirstColumn(DataGridViewElementStates.Visible);
            columns.Add(col);
            List<string> cols = new List<string>();
            cols.Add(col.DataPropertyName);

            while (col != gridView.Columns.GetLastColumn(DataGridViewElementStates.Visible, DataGridViewElementStates.None))
            {
                col = gridView.Columns.GetNextColumn(col, DataGridViewElementStates.Visible, DataGridViewElementStates.None);
                cols.Add(col.DataPropertyName);
                columns.Add(col);
            }

            if (gridView.DataSource is DataTable)
            {
                dt = (gridView.DataSource as DataTable).DefaultView.ToTable(false, cols.ToArray());
            }
            else if (gridView.DataSource is DataView)
            {
                dt = (gridView.DataSource as DataView).ToTable(false, cols.ToArray());
            }

            ds.Tables.Add(dt);
            dt.Columns[0].Caption = gridView.Columns.GetFirstColumn(DataGridViewElementStates.Visible).HeaderText;

            for (int i = 0; i < dt.Columns.Count; i++)
            {
                dt.Columns[i].Caption = columns[i].HeaderText;
                dt.Columns[i].ColumnName = columns[i].HeaderText;
            }
            //       dt.Columns[0].ColumnName = dt.Columns[0].Caption;
            return ds;
        }

        #region 转换大写数字
        /// <summary>
        /// 金额转换成中文大写金额
        /// </summary>
        /// <param name="LowerMoney">eg:10.74</param>
        /// <returns></returns>
        public static string MoneyToUpper(string LowerMoney)
        {
            string functionReturnValue = null;
            bool IsNegative = false; // 是否是负数
            if (LowerMoney.Trim().Substring(0, 1) == "-")
            {
                // 是负数则先转为正数
                LowerMoney = LowerMoney.Trim().Remove(0, 1);
                IsNegative = true;
            }
            string strLower = null;
            string strUpart = null;
            string strUpper = null;
            int iTemp = 0;
            // 保留两位小数 123.489→123.49　　123.4→123.4
            LowerMoney = Math.Round(double.Parse(LowerMoney), 2).ToString();
            if (LowerMoney.IndexOf(".") > 0)
            {
                if (LowerMoney.IndexOf(".") == LowerMoney.Length - 2)
                {
                    LowerMoney = LowerMoney + "0";
                }
            }
            else
            {
                LowerMoney = LowerMoney + ".00";
            }
            strLower = LowerMoney;
            iTemp = 1;
            strUpper = "";
            while (iTemp <= strLower.Length)
            {
                switch (strLower.Substring(strLower.Length - iTemp, 1))
                {
                    case ".":
                        strUpart = "圆";
                        break;
                    case "0":
                        strUpart = "零";
                        break;
                    case "1":
                        strUpart = "壹";
                        break;
                    case "2":
                        strUpart = "贰";
                        break;
                    case "3":
                        strUpart = "叁";
                        break;
                    case "4":
                        strUpart = "肆";
                        break;
                    case "5":
                        strUpart = "伍";
                        break;
                    case "6":
                        strUpart = "陆";
                        break;
                    case "7":
                        strUpart = "柒";
                        break;
                    case "8":
                        strUpart = "捌";
                        break;
                    case "9":
                        strUpart = "玖";
                        break;
                }

                switch (iTemp)
                {
                    case 1:
                        strUpart = strUpart + "分";
                        break;
                    case 2:
                        strUpart = strUpart + "角";
                        break;
                    case 3:
                        strUpart = strUpart + "";
                        break;
                    case 4:
                        strUpart = strUpart + "";
                        break;
                    case 5:
                        strUpart = strUpart + "拾";
                        break;
                    case 6:
                        strUpart = strUpart + "佰";
                        break;
                    case 7:
                        strUpart = strUpart + "仟";
                        break;
                    case 8:
                        strUpart = strUpart + "万";
                        break;
                    case 9:
                        strUpart = strUpart + "拾";
                        break;
                    case 10:
                        strUpart = strUpart + "佰";
                        break;
                    case 11:
                        strUpart = strUpart + "仟";
                        break;
                    case 12:
                        strUpart = strUpart + "亿";
                        break;
                    case 13:
                        strUpart = strUpart + "拾";
                        break;
                    case 14:
                        strUpart = strUpart + "佰";
                        break;
                    case 15:
                        strUpart = strUpart + "仟";
                        break;
                    case 16:
                        strUpart = strUpart + "万";
                        break;
                    default:
                        strUpart = strUpart + "";
                        break;
                }

                strUpper = strUpart + strUpper;
                iTemp = iTemp + 1;
            }

            strUpper = strUpper.Replace("零拾", "零");
            strUpper = strUpper.Replace("零佰", "零");
            strUpper = strUpper.Replace("零仟", "零");
            strUpper = strUpper.Replace("零零零", "零");
            strUpper = strUpper.Replace("零零", "零");
            strUpper = strUpper.Replace("零角零分", "整");
            strUpper = strUpper.Replace("零分", "整");
            strUpper = strUpper.Replace("零角", "零");
            strUpper = strUpper.Replace("零亿零万零圆", "亿圆");
            strUpper = strUpper.Replace("亿零万零圆", "亿圆");
            strUpper = strUpper.Replace("零亿零万", "亿");
            strUpper = strUpper.Replace("零万零圆", "万圆");
            strUpper = strUpper.Replace("零亿", "亿");
            strUpper = strUpper.Replace("零万", "万");
            strUpper = strUpper.Replace("零圆", "圆");
            strUpper = strUpper.Replace("零零", "零");

            // 对壹圆以下的金额的处理
            if (strUpper.Substring(0, 1) == "圆")
            {
                strUpper = strUpper.Substring(1, strUpper.Length - 1);
            }
            if (strUpper.Substring(0, 1) == "零")
            {
                strUpper = strUpper.Substring(1, strUpper.Length - 1);
            }
            if (strUpper.Substring(0, 1) == "角")
            {
                strUpper = strUpper.Substring(1, strUpper.Length - 1);
            }
            if (strUpper.Substring(0, 1) == "分")
            {
                strUpper = strUpper.Substring(1, strUpper.Length - 1);
            }
            if (strUpper.Substring(0, 1) == "整")
            {
                strUpper = "零圆整";
            }
            functionReturnValue = strUpper;

            if (IsNegative == true)
            {
                return "负" + functionReturnValue;
            }
            else
            {
                return functionReturnValue;
            }
        }
        #endregion

    }


    /// <summary>
    /// app.confg属性
    /// </summary>
    public class Item
    {
        public string DDYLJGBH { set; get; }
        public string DDYLJGMC { set; get; }
        public string YBIP { set; get; }
        public string Gocent { set; get; }
    }


    //医保业务代码方法对应
    public enum YBYWDM
    {
        //通用业务代码

        //吉安地区医保业务代码
        YBINIT=9000, //初始化
        YBEXIT = 9100, //退出
        YBMZDK = 2101, //门诊读卡
        YBMZDK1 = 2102, //门诊读卡
        YBZYDK = 2201, //住院读卡
        YBMZDJ = 3100, //门诊登记(挂号)
        YBMZDJCX = 3101, //门诊登记(挂号)撤销
        YBMZDJSF = 3102, //门诊登记(挂号)收费
        YBMZDJSFCX = 3103, //门诊登记(挂号)收费撤销
        YBMZDJ_AUTO=3104,  //门诊登记(挂号)_自动
        YBMZCFMXFHCX=3203, //门诊上传处方查询
        YBMZSFYJS = 3300, //门诊费用预结算
        YBMZSFJS = 3301, //门诊费用结算
        YBMZSFJSCX = 3302, //门诊费用结算撤销
        YBMZJSD = 3303, //门诊费用结算单
        YBZYDJ = 4100, //住院登记
        YBZYDJBG=4101, //住院登记变更
        YBZYDJCX = 4102, //住院登记撤销
        YBZYBQDJ = 4200,  //住院病情登记
        YBZYBQCX = 4201, //病情登记撤销
        YBZYSFDJ = 4300, //住院费用登记
        YBZYSFDJCX = 4301, //住院费用登记撤销
        YBZYSFYJS = 4400, //住院费用预结算
        YBZYSFJS = 4401, //住院费用结算
        YBZYSFJSCX = 4402, //住院费用结算撤销
        YBZYJSD = 4403, //住院结算单打印
        YBZYJSFYD=4404, //离院结算费用报表
        YBZYJSXXCX=4405, //住院结算明细查询
        YBCZJY = 4500,  //冲正交易
        YBYLFXXCX = 5100, //医疗费信息查询
        YBJSXXCX = 5101,  //个人结算信息查询
        YBGRYLFXXCX = 5102, //个人医疗费信息查询
        YBGRJZDJXXCX = 5103,  //个人就诊登记信息查询
        YBFYMXXXXX = 5104,    //费用明细详细信息
        YBYJSFYDZ = 5107, //月结算费用对帐
        YBPLSJCXXZ = 5108,    //批量数据查询下载
        YBYLDYFSXXCX = 5110, //医疗待遇封锁信息查询
        YBYLDYSPXXCX = 5111, //医疗待遇审批信息查询
        YBBZCX=5115,    //医保病种查询
        YBDZXXPLSC=5202, //医保对照上传
        YBKSXXSC=5203,//科室信息上传
        YBYSXXSC=5204, //医师信息上传
        NYBFYJSCX=1100, //门诊费用结算撤销(内部)
        NYBMZCFMXCX = 1101,//门诊费用登记撤销_内部
        NYBMZDJCX = 1102, //门诊登记撤销_内部
        NYBZYCFMXSCCX = 1103, //住院费用登记撤销(内部)
        N_YBMZZYDJCX = 1104, //住院登记撤销(内部)
        N_YBMZZYSFDJCX = 1105 //住院登记撤销(内部)

        //汤山地区通用医保
        //YBINIT = 9000, //初始化
        //YBEXIT = 9100, //退出
        //YBMZDK = 2101, //门诊读卡
        //YBZYDK = 2201, //住院读卡
        //YBMZDJ = 2210, //门诊登记
        //YBMZDJCX = 2240, //门诊登记撤销
        //YBMZFYDJCX = 3201, //门诊费用登记撤销
        //YBMZFYYJS = 3300, //门诊费用预结算
        //YBMZFYJS = 3301, //门诊费用结算
        //YBMZFYJSCX = 3302, //门诊费用结算撤销
        //YBZYDJ = 4100, //住院登记
        //YBZYDJCX = 4102, //住院登记撤销
        //YBZYFYDJ = 4300, //住院费用登记
        //YBZYFYDJCX = 4301, //住院费用登记撤销
        //YBZYFYYJS = 4400, //住院费用预结算
        //YBZYFYJS = 4401, //住院费用结算
        //YBZYFYJSCX = 4402, //住院费用结算撤销
        //YBMZDJSF = 3100, //门诊登记收费
        //YBMZDJSFCX = 3101, //门诊登记收费撤销
    }

}
