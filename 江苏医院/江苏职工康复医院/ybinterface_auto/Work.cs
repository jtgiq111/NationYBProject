using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ybinterface_auto
{
    public class Work : IWork
    {
        /// <summary>
        /// 读取xml.config和设置目标数据库
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public Item getXmlConfig(string path)
        {
            Item it = new Item();
            try
            {
                var xml = XElement.Load(path);
                it.YBUSER = xml.Element("ybuser").Value.Trim();
                it.YBPWD = xml.Element("ybpwd").Value.Trim();
                it.POINT1 = xml.Element("point1").Value.Trim();
                it.POINT2 = xml.Element("point2").Value.Trim();
                it.SRCCON = xml.Element("srccon").Value.Trim();
                it.YBCODE = xml.Element("ybcode").Value.Trim();
                it.YBIP = xml.Element("ybip").Value.Trim();

            }
            catch (Exception ee)
            {
                //Log.WriteLine(ee.Message);
            }
            return it;
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
    public class Item
    {
        public string YBUSER { set; get; }
        public string YBPWD { set; get; }
        public string POINT1 { set; get; }
        public string POINT2 { set; get; }
        public string SRCCON { set; get; }
        public string YBCODE { set; get; }
        public string YBIP { set; get; }
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
    }

}
