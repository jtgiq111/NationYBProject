using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ybinterface_lib.JX.DZ
{
   public class YBDetail
    {
        /// <summary>
        /// 门诊/住院流水号
        /// </summary>
        public string  ipt_otp_no { get; set; }
        /// <summary>
        /// 单据号
        /// </summary>
        public string medrcdno { get; set; }
        /// <summary>
        /// 医疗费总额
        /// </summary>
        public string medfee_sumamt { get; set; }
        /// <summary>
        /// 统筹支付金额
        /// </summary>
        public string hifp_pay { get; set; }
        /// <summary>
        /// 现金支付总额
        /// </summary>
        public string psn_cash_pay { get; set; }
    }
}
