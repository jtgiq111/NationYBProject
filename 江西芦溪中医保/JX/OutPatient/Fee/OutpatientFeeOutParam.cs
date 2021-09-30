using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace yb_interfaces.JX.OutPatient.Fee
{
    public class OutpatientFeeOutParam
    {
        /// <summary>
        /// 费用明细流水号
        /// </summary>
        public string feedetl_sn { get; set; }
        /// <summary>
        /// 明细项目费用总额
        /// </summary>
        public decimal det_item_fee_sumamt { get; set; }
        /// <summary>
        /// 数量
        /// </summary>
        public decimal cnt { get; set; }
        /// <summary>
        /// 单价
        /// </summary>
        public decimal pric { get; set; }
        /// <summary>
        /// 定价上限金额
        /// </summary>
        public decimal pric_uplmt_amt { get; set; }
        /// <summary>
        /// 自付比例
        /// </summary>
        public decimal selfpay_prop { get; set; }
        /// <summary>
        /// 全自费金额
        /// </summary>
        public decimal fulamt_ownpay_amt { get; set; }
        /// <summary>
        /// 超限价金额
        /// </summary>
        public decimal overlmt_amt { get; set; }
        /// <summary>
        /// 先行自付金额
        /// </summary>
        public decimal preselfpay_amt { get; set; }
        /// <summary>
        /// 符合政策范围金额
        /// </summary>
        public decimal inscp_scp_amt { get; set; }
        /// <summary>
        /// 收费项目等级
        /// </summary>
        public string chrgitm_lv { get; set; }
        /// <summary>
        /// 医疗收费项目类别
        /// </summary>
        public string med_chrgitm_type { get; set; }
        /// <summary>
        /// 基本药物标志
        /// </summary>
        public string bas_medn_flag { get; set; }
        /// <summary>
        /// 医保谈判药品标志
        /// </summary>
        public string hi_nego_drug_flag { get; set; }
        /// <summary>
        /// 儿童用药标志
        /// </summary>
        public string chld_medc_flag { get; set; }
        /// <summary>
        /// 目录特项标志
        /// </summary>
        public string list_sp_item_flag { get; set; }
        /// <summary>
        /// 限制使用标志
        /// </summary>
        public string lmt_used_flag { get; set; }
        /// <summary>
        /// 直报标志
        /// </summary>
        public string drt_reim_flag { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string memo { get; set; }

    }
}
