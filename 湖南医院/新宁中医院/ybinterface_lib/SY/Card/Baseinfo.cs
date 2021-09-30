using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ybinterface_lib.SY.Card
{
    /// <summary>
    /// 基本信息
    /// </summary>
    public class Baseinfo
    {
        /// <summary>
        /// 人员编号
        /// </summary>
        public string psn_no { get; set; }
        /// <summary>
        /// 人员证件类型
        /// </summary>
        public string psn_cert_type { get; set; }
        /// <summary>
        /// 证件号码
        /// </summary>
        public string certno { get; set; }
        /// <summary>
        /// 人员姓名
        /// </summary>
        public string psn_name { get; set; }
        /// <summary>
        /// 性别
        /// </summary>
        public string gend { get; set; }
        /// <summary>
        /// 民族
        /// </summary>
        public string naty { get; set; }
        /// <summary>
        /// 生日
        /// </summary>
        public string brdy { get; set; }
        /// <summary>
        /// 年龄
        /// </summary>
        public string age { get; set; }
    }
}
