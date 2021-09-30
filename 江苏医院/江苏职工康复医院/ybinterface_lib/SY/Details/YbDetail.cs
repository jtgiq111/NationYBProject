using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ybinterface_lib.SY.Details
{
   public class YbDetail
    {
        /// <summary>
        /// 门诊/住院流水号
        /// </summary>
        public string ipt_otp_no { get; set; }
        /// <summary>
        /// 单据号
        /// </summary>
        public string medrcdno { get; set; }
        /// <summary>
        /// 处方号
        /// </summary>
        public string rxno { get; set; }
        /// <summary>
        /// 处方流水号
        /// </summary>
        public string feedetl_sn { get; set; }
        /// <summary>
        /// 处方日期
        /// </summary>
        public string fee_ocur_time { get; set; }
        /// <summary>
        /// 处方中心编码
        /// </summary>
        public string med_list_codg { get; set; }
        /// <summary>
        /// 单价
        /// </summary>
        public decimal pric { get; set; }
        /// <summary>
        /// 数量
        /// </summary>
        public int cnt { get; set; }
        /// <summary>
        /// 金额
        /// </summary>
        public decimal umamt { get; set; }
        /// <summary>
        /// 自费金额
        /// </summary>
        public decimal ownpay_amt { get; set; }
        /// <summary>
        /// 超限价金额
        /// </summary>
        public decimal alwpay_amtc { get; set; }
        /// <summary>
        /// 限价
        /// </summary>
        public decimal pric_uplmt_amt { get; set; }
        /// <summary>
        /// 收费项目等级
        /// </summary>
        public string chrgitm_lv { get; set; }
        /// <summary>
        /// 说明信息
        /// </summary>
        public string memo { get; set; }
        /// <summary>
        /// 收费项目种类
        /// </summary>
        public string list_type { get; set; }
        /// <summary>
        /// 总页数
        /// </summary>
        public int pageCount { get; set; }

    }
    public class YbDetailCost 
    {
        /// <summary>
        /// 门诊/住院流水号
        /// </summary>
        public string ipt_otp_no { get; set; }
        /// <summary>
        /// 单据号
        /// </summary>
        public string medrcdno { get; set; }
        /// <summary>
        /// 中心医疗费总额
        /// </summary>
        public decimal medfee_sumamt { get; set; }
        /// <summary>
        /// 中心统筹支付金额
        /// </summary>
        public decimal hifp_pay { get; set; }
        /// <summary>
        /// 中心现金支付总额
        /// </summary>
        public decimal psn_cash_pay { get; set; }
        /// <summary>
        /// 发送方报文ID
        /// </summary>
        public string msgid { get; set; }
        /// <summary>
        /// 医疗类别
        /// </summary>
        public string med_type { get; set; }
        /// <summary>
        /// 个人唯一识别码
        /// </summary>
        public string psn_no { get; set; }
        /// <summary>
        /// 参保人姓名
        /// </summary>
        public string psn_name { get; set; }
        /// <summary>
        /// 结算日期
        /// </summary>
        public string setl_time { get; set; }
        /// <summary>
        /// HIS操作员编码
        /// </summary>
        public string opter { get; set; }
        /// <summary>
        /// 总页数
        /// </summary>
        public int pageCount { get; set; }
    }

}
