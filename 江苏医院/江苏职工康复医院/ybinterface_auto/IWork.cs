using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ybinterface_auto
{
    public interface IWork
    {
        /// <summary>
        /// 读取ybConfig.xml内容信息
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        Item getXmlConfig(string path);
        List<Item1> getXmlConfig1(string path);

        /// <summary>
        /// 连接数据库
        /// </summary>
        /// <param name="strCon"></param>
        //void setConnectionDB(string strCon);

        
    }
}
