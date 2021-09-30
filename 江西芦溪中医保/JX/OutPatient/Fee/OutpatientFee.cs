using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace yb_interfaces.JX.OutPatient.Fee
{
    public class OutpatientFee
    {
        /// <summary>
        /// 费用明细流水号
        /// </summary>
        /// <value></value>
        public string feedetl_sn { get; set; }
        /// <summary>
        /// 就诊ID
        /// </summary>
        /// <value></value>
        public string mdtrt_id { get; set; }
        /// <summary>
        /// 人员编号
        /// </summary>
        /// <value></value>
        public string psn_no { get; set; }
        /// <summary>
        /// 收费批次号
        /// </summary>
        /// <value></value>
        public string chrg_bchno { get; set; }
        /// <summary>
        /// 病种编码
        /// </summary>
        /// <value></value>
        public string dise_codg { get; set; }
        /// <summary>
        /// 处方号
        /// </summary>
        /// <value></value>
        public string rxno { get; set; }
        /// <summary>
        /// 外购处方标志
        /// </summary>
        /// <value></value>
        public string rx_circ_flag { get; set; }
        /// <summary>
        /// 费用发生时间
        /// </summary>
        /// <value></value>
        public string fee_ocur_time { get; set; }
        /// <summary>
        /// 医疗目录编码
        /// </summary>
        /// <value></value>
        public string med_list_codg { get; set; }
        /// <summary>
        /// 医药机构目录编码
        /// </summary>
        /// <value></value>
        public string medins_list_codg { get; set; }
        /// <summary>
        /// 明细项目费用总额
        /// </summary>
        /// <value></value>
        public decimal det_item_fee_sumamt { get; set; }
        /// <summary>
        /// 数量
        /// </summary>
        /// <value></value>
        public decimal cnt { get; set; }
        /// <summary>
        /// 单价
        /// </summary>
        /// <value></value>
        public decimal pric { get; set; }
        /// <summary>
        /// 单次剂量描述
        /// </summary>
        /// <value></value>
        public string sin_dos_dscr { get; set; }
        /// <summary>
        /// 使用频次描述
        /// </summary>
        /// <value></value>
        public string used_frqu_dscr { get; set; }
        /// <summary>
        /// 周期天数
        /// </summary>
        /// <value></value>
        public int prd_days { get; set; }
        /// <summary>
        /// 用药途径描述
        /// </summary>
        /// <value></value>
        public string medc_way_dscr { get; set; }
        /// <summary>
        /// 开单科室编码
        /// </summary>
        /// <value></value>
        public string bilg_dept_codg { get; set; }
        /// <summary>
        /// 开单科室名称
        /// </summary>
        /// <value></value>
        public string bilg_dept_name { get; set; }
        /// <summary>
        /// 开单医生编码
        /// </summary>
        /// <value></value>
        public string bilg_dr_codg { get; set; }
        /// <summary>
        /// 开单医师姓名
        /// </summary>
        /// <value></value>
        public string bilg_dr_name { get; set; }
        /// <summary>
        /// 受单科室编码
        /// </summary>
        /// <value></value>
        public string acord_dept_codg { get; set; }
        /// <summary>
        /// 受单科室名称
        /// </summary>
        /// <value></value>
        public string acord_dept_name { get; set; }
        /// <summary>
        /// 受单医生编码
        /// </summary>
        /// <value></value>
        public string orders_dr_code { get; set; }
        /// <summary>
        /// 受单医生姓名
        /// </summary>
        /// <value></value>
        public string orders_dr_name { get; set; }
        /// <summary>
        /// 医院审批标志
        /// </summary>
        /// <value></value>
        public string hosp_appr_flag { get; set; }
        /// <summary>
        /// 中药使用方式
        /// </summary>
        /// <value></value>
        public string tcmdrug_used_way { get; set; }
        /// <summary>
        /// 外检标志
        /// </summary>
        /// <value></value>
        public string etip_flag { get; set; }
        /// <summary>
        /// 外检医院编码
        /// </summary>
        /// <value></value>
        public string etip_hosp_code { get; set; }
        /// <summary>
        /// 出院带药标志
        /// </summary>
        /// <value></value>
        public string dscg_tkdrug_flag { get; set; }
        /// <summary>
        /// 生育费用标志
        /// </summary>
        /// <value></value>
        public string matn_fee_flag { get; set; }
    }
}