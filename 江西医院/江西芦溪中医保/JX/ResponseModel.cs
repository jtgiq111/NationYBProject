using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace yb_interfaces.YB.湖南
{
   public class ResponseModel
    {
        /// <summary>
        /// 交易状态码0 成功 -1 失败
        /// </summary>
        public string infcode { get; set; }
        /// <summary>
        /// 接收方报文ID
        /// 接收方返回，接收方医保区划代码(6)+时间(14)+流水号(10)
        ///时间格式：yyyyMMddHHmmss
        /// </summary>
        public string inf_refmsgid { get; set; }
        /// <summary>
        /// 接收报文时间
        /// 格式：yyyyMMddHHmmssSSS
        /// </summary>
        public string  refmsg_time { get; set; }
        /// <summary>
        /// 响应报文时间
        /// 格式：yyyyMMddHHmmssSSS
        /// </summary>
        public string respond_time { get; set; }
        /// <summary>
        /// 交易失败状态下，业务返回的错误信息
        /// </summary>
        public string err_msg { get; set; } = null;
        /// <summary>
        /// 交易输出
        /// </summary>
        public JObject output { get; set; }
        /// <summary>
        /// 提示信息
        /// </summary>
        public object warn_msg { get; set; } = null;
        public object cainfo { get; set; } = null;
        public object signtype { get; set; } = null;
    } 

}
