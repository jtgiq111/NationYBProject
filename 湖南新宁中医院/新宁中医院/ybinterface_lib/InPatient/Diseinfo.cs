using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace yb_interfaces.YB.湖南.InPatient
{
   public class Diseinfo
    {
        /// <summary>
        /// 人员编号
        /// </summary>
        public string psn_no { get; set; }
        /// <summary>
        /// 诊断类别
        /// </summary>
        public string diag_type { get; set; }
        /// <summary>
        /// 主诊断标志
        /// </summary>
        public string maindiag_flag { get; set; }
        /// <summary>
        /// 诊断排序号
        /// </summary>
        public int diag_srt_no { get; set; }
        /// <summary>
        /// 诊断代码
        /// </summary>
        public string diag_code { get; set; }
        /// <summary>
        /// 诊断名称
        /// </summary>
        public string diag_name { get; set; }
        /// <summary>
        /// 入院病情
        /// </summary>
        public string adm_cond { get; set; }
        /// <summary>
        /// 诊断科室
        /// </summary>
        public string diag_dept { get; set; }
        /// <summary>
        /// 诊断医生编码
        /// </summary>
        public string dise_dor_no { get; set; }
        /// <summary>
        /// 诊断医生姓名
        /// </summary>
        public string dise_dor_name { get; set; }
        /// <summary>
        /// 诊断时间yyyy-MM-dd HH:mm:ss
        /// </summary>
        public string diag_time { get; set; }
    }
}
