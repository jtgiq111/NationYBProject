using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ybinterface_lib.JX.Card
{
    /// <summary>
    /// 身份信息列表
    /// </summary>
    public class Idetinfo
    {
        /// <summary>
        /// 人员身份类别
        /// </summary>
        public string psn_idet_type { get; set; }
        /// <summary>
        /// 人员类别等级
        /// </summary>
        public string psn_type_lv { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string memo { get; set; }
        /// <summary>
        /// 开始时间
        /// </summary>
        public string begntime { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        public string endtime { get; set; }
      
    }
}
