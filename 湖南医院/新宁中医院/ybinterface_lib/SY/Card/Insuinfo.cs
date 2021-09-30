using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ybinterface_lib.SY.Card
{
    /// <summary>
    /// 参保信息列表
    /// </summary>
    public class Insuinfo
    {
        /// <summary>
        /// 余额
        /// </summary>
        public string balc { get; set; }
        /// <summary>
        /// 险种类型
        /// </summary>
        public string insutype { get; set; }
        /// <summary>
        /// 人员类别
        /// </summary>
        public string psn_type { get; set; }
        /// <summary>
        /// 公务员标识
        /// </summary>
        public string cvlserv_flag { get; set; }
        /// <summary>
        /// 参保地医保区划
        /// </summary>
        public string insuplc_admdvs { get; set; }
        /// <summary>
        /// 单位名称
        /// </summary>
        public string emp_name { get; set; }
 
    }
}
