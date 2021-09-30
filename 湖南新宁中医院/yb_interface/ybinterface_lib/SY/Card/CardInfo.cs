using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ybinterface_lib.SY.Card
{
   public class CardInfo
    {
        /// <summary>
        /// 个人唯一识别码
        /// </summary>
        public int psn_no { get; set; }
        /// <summary>
        /// 单位唯一识别码 
        /// </summary>
        public object emp_no{ get; set; }
        /// <summary>
        /// 单位名称 
        /// </summary>
        public string emp_name { get; set; }
        /// <summary>
        /// 证件号码
        /// </summary>
        public string certno { get; set; }
        /// <summary>
        /// 姓名
        /// </summary>
        public string psn_name { get; set; }
        /// <summary>
        /// 性别
        /// </summary>
        public string gend { get; set; }
        /// <summary>
        /// 年龄
        /// </summary>
        public int  age { get; set; }
        /// <summary>
        /// 工伤人员类别
        /// </summary>
        public string psn_type { get; set; }
        /// <summary>
        /// 统筹区号
        /// </summary>
        public string insu_admdvs { get; set; }
        /// <summary>
        /// 在院状态
        /// </summary>
        public string inhosp_stas { get; set; }
        /// <summary>
        /// 待遇不享受原因
        /// </summary>
        public string trt_chk_rslt { get; set; }
        /// <summary>
        /// 工伤诊断结论
        /// </summary>
        public string exam_ccls { get; set; }
        /// <summary>
        /// 证件类型
        /// </summary>
        public string certificate_type { get; set; }
        /// <summary>
        /// 出生日期yyyyMMdd
        /// </summary>
        public int birthday { get; set; }
    }
}
