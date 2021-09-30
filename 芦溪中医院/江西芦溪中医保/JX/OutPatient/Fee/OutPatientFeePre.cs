using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace yb_interfaces.JX.OutPatient.Fee
{
  public  class OutPatientFeePre
    {
        /// <summary>
        /// 门诊流水号
        /// </summary>
        public string ipt_otp_no { get; set; }
        /// <summary>
        /// 单据号
        /// </summary>
        public string mdtrt_id { get; set; }
        /// <summary>
        ///  本次医疗费总额
        /// </summary>
        public decimal medfee_sumamt { get; set; }
        /// <summary>
        /// 本次统筹支付金额
        /// </summary>
        public decimal hifp_pay { get; set; }
        /// <summary>
        /// 本次现金支付总额
        /// </summary>
        public decimal psn_cash_pay { get; set; }
        /// <summary>
        ///  说明信息
        /// </summary>
        public string med_list_codg { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string umamt { get; set; }
        /// <summary>
        /// 药费合计
        /// </summary>
        public decimal drug_fee { get; set; }
        /// <summary>
        /// 诊疗项目费合计 
        /// </summary>
        public decimal dati_fee { get; set; }
        /// <summary>
        ///材料费合计
        /// </summary>
        public decimal ms_fee { get; set; }
    }
}
