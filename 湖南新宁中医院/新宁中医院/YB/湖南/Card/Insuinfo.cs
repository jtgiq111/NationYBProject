using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace yb_interfaces.YB.湖南.Card
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
        /// 人员参保状态
        /// </summary>
        public string psn_insu_stas { get; set; }
        /// <summary>
        /// 个人参保日期
        /// </summary>
        public string psn_insu_date { get; set; }
        /// <summary>
        /// 暂停参保日期
        /// </summary>
        public string paus_insu_date { get; set; }
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
