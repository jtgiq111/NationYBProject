using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace yb_interfaces.YB.湖南.Card
{
   public class ReadCardInfo
    {
        /// <summary>
        /// 基本信息
        /// </summary>
        public Baseinfo baseinfo { get; set; }
        /// <summary>
        /// 参保信息列表
        /// </summary>
        public Insuinfo insuinfo { get; set; }
        /// <summary>
        /// 身份信息列表
        /// </summary>
        public Idetinfo idetinfo { get; set; }
    }
}
