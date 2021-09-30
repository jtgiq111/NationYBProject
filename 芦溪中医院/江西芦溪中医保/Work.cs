using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace yb_interfaces
{
    public class Work : IWork
    {
        /// <summary>
        /// 读取xml.config和设置目标数据库
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public List<Item1> getXmlConfig(string path)
        {
            List<Item1> lstItem = new List<Item1>();

            try
            {
                var xml = XElement.Load(path);

                foreach (var t in xml.Elements("Item"))
                {
                    Item1 item = new Item1();
                    item.YLJGBH = xml.Element("YLJGBH").Value.Trim();
                    item.ZXBM = xml.Element("ZXBM").Value.Trim();
                    item.LJBS = xml.Element("LJBS").Value.Trim();
                    item.Name = t.Element("Name").Value.Trim();
                    item.TableName = t.Element("TableName").Value.Trim();
                    if (!item.TableName.StartsWith("["))
                    {
                        item.TableName = "[" + item.TableName;
                    }

                    if (!item.TableName.EndsWith("]"))
                    {
                        item.TableName = item.TableName + "]";
                    }
                    item.CreateSql = t.Element("CreateSql").Value.Trim();
                    item.SelectSql = t.Element("SelectSql").Value.Trim();
                    lstItem.Add(item);
                }

            }
            catch (Exception ee)
            {
                //Log.WriteLine(ee.Message);
            }
            return lstItem;
        }

        public List<Item1> getXmlConfig1(string path)
        {
            List<Item1> lstItem = new List<Item1>();

            try
            {
                var xml = XElement.Load(path);
                Item1 item = item = new Item1();
                foreach (var t in xml.Elements("appSettings").Elements("add"))
                {

                    item.TableName = t.Attribute("key").Value.Trim();
                    if (item.TableName.Equals("DbHelper"))
                        item.SrcCon = t.Attribute("value").Value;
                    if (item.TableName.Equals("DDYLJGBH"))
                        item.DDYLJGBH = t.Attribute("value").Value;
                    if (item.TableName.Equals("DDYLJGMC"))
                        item.DDYLJGMC = t.Attribute("value").Value;
                    if (item.TableName.Equals("YBIP"))
                        item.YBIP = t.Attribute("value").Value;
                    if (item.TableName.Equals("YBPORT"))
                        item.YBPORT = t.Attribute("value").Value;
                    if (item.TableName.Equals("TIMEOUT"))
                        item.TIMEOUT = t.Attribute("value").Value;
                }
                lstItem.Add(item);

            }
            catch (Exception ee)
            {
                //Log.WriteLine(ee.Message);
            }
            return lstItem;
        }


    }
    


    /// <summary>
    /// xmlconfg属性
    /// </summary>
    public class Item1
    {
        public string YLJGBH { set; get; }
        public string ZXBM { set; get; }
        public string LJBS { set; get; }
        public string SrcCon { set; get; }
        public string Name { set; get; }
        public string TableName { set; get; }
        public string CreateSql { set; get; }
        public string SelectSql { set; get; }
        public string DDYLJGBH { set; get; }
        public string DDYLJGMC { set; get; }
        public string YBIP { set; get; }
        public string YBPORT { set; get; }
        public string Gocent { set; get; }
        public string TIMEOUT { set; get; }
        public string YBZDSCGH { set; get; }
    }

}
