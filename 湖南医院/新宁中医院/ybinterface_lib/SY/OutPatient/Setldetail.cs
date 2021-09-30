using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ybinterface_lib.SY.OutPatient
{
   public class Setldetail
    {
        /// <summary>
        /// 基金支付类型
        /// </summary>
        public string fund_pay_type { get; set; }
        /// <summary>
        /// 符合政策范围金额
        /// </summary>
        public decimal inscp_scp_amt { get; set; }
        /// <summary>
        /// 本次可支付限额金额
        /// </summary>
        public decimal crt_payb_lmt_amt { get; set; }
        /// <summary>
        /// 基金支付金额
        /// </summary>
        public decimal fund_payamt { get; set; }
        /// <summary>
        /// 基金支付类型名称
        /// </summary>
        public string fund_pay_type_name { get; set; }
        /// <summary>
        /// 结算过程信息
        /// </summary>
        public string setl_proc_info { get; set; }
    }
}
