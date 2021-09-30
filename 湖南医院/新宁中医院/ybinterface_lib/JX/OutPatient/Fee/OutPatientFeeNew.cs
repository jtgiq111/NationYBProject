using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ybinterface_lib.JX.OutPatient.Fee
{
   public class OutPatientFeeNew
    {
        /// <summary>
        /// 门诊流水号
        /// </summary>
        public string ipt_otp_no { get; set; }
        /// <summary>
        /// 三大目录类别
        /// </summary>
        public string list_type { get; set; }
        /// <summary>
        /// 处方号
        /// </summary>
        public string rxno { get; set; }
        /// <summary>
        /// 处方流水号
        /// </summary>
        public string feedetl_sn { get; set; }
        /// <summary>
        /// 处方日期 yyyyMMddHHmmss
        /// </summary>
        public string fee_ocur_time { get; set; }
        /// <summary>
        /// 收费项目中心编码
        /// </summary>
        public string med_list_codg { get; set; }
        /// <summary>
        /// 单价
        /// </summary>
        public decimal pric { get; set; }
        /// <summary>
        /// 数量
        /// </summary>
        public decimal cnt { get; set; }
        /// <summary>
        /// 总金额
        /// </summary>
        public decimal umamt { get; set; }
        /// <summary>
        /// 医生编码
        /// </summary>
        public string bilg_dr_codg { get; set; }
        /// <summary>
        /// 科室编码
        /// </summary>
        public string bilg_dept_codg { get; set; }
        /// <summary>
        /// 是否最小计量单位   空默认为否
        /// </summary>
        public string min_unit { get; set; }

    }
}
