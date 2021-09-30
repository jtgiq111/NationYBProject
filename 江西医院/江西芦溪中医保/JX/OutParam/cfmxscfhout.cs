using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace yb_interfaces.JX.OutParam
{
   public class cfmxscfhout
    {
        /// <summary>
        /// 处方号
        /// </summary>
        public string rxno { get; set; }
        /// <summary>
        /// 处方流水号
        /// </summary>
        public string feedetl_sn { get; set; }
        /// <summary>
        /// 处方时间
        /// </summary>
        public string fee_ocur_time { get; set; }
        /// <summary>
        /// 收费项目中心编码 
        /// </summary>
        public string med_list_codg { get; set; }
        /// <summary>
        /// 总金额 
        /// </summary>
        public string umamt { get; set; }
        /// <summary>
        /// 自费金额 
        /// </summary>
        public string ownpay_amt { get; set; }
        /// <summary>
        /// 超限价金额
        /// </summary>
        public string alwpay_amt { get; set; }
        /// <summary>
        /// 支付上限
        /// </summary>
        public string pric_uplmt_amt { get; set; }
        /// <summary>
        /// 收费项目等级 
        /// </summary>
        public string chrgitm_lv { get; set; }
        /// <summary>
        /// 说明信息 
        /// </summary>
        public string memo { get; set; }
    }
}
