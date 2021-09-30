using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace yb_interfaces
{
    /// <summary>
    /// 接口输入报文格式定义
    /// </summary>
    public class INPARAM
    {
        /// <summary>
        /// 交易编号
        /// </summary>
        public string infno { get; set; }
        /// <summary>
        /// 发送方报文 ID
        /// </summary>
        public string msgid { get; set; }
        /// <summary>
        /// 就医地医保区划
        /// </summary>
        public string mdtrtarea_admvs { get; set; }
        /// <summary>
        /// 参保地医保区划
        /// </summary>
        public string insuplc_admdvs { get; set; }
        /// <summary>
        /// 接收方系统代码
        /// </summary>
        public string recer_sys_code { get; set; }
        /// <summary>
        /// 设备编号
        /// </summary>
        public string dev_no { get; set; }
        /// <summary>
        /// 设备安全信息
        /// </summary>
        public string dev_safe_info { get; set; }
        /// <summary>
        /// 数字签名信息
        /// </summary>
        public string cainfo { get; set; }
        /// <summary>
        /// 签名类型
        /// </summary>
        public string signtype { get; set; }
        /// <summary>
        /// 接口版本号
        /// </summary>
        public string infver { get; set; }
        /// <summary>
        /// 经办人类别
        /// </summary>
        public string opter_type { get; set; }
        /// <summary>
        /// 经办人
        /// </summary>
        public string opter { get; set; }
        /// <summary>
        /// 经办人姓名
        /// </summary>
        public string opter_name { get; set; }
        /// <summary>
        /// 交易时间
        /// </summary>
        public string inf_time { get; set; }
        /// <summary>
        /// 定点医药机构编号
        /// </summary>
        public string fixmedins_code { get; set; }
        /// <summary>
        /// 定点医药机构名称
        /// </summary>
        public string fixmedins_name { get; set; }
        /// <summary>
        /// 交易签到流水号
        /// </summary>
        public string sign_no { get; set; }
        /////定点医药机构软件厂商全称
        public string fixmedins_soft_fcty { get; set; }
    }
}
