using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ybinterface_lib.JX.Register
{
   public class Request
    {
        /// <summary>
        /// 门诊/住院流水号
        /// </summary>
        public string ipt_otp_no { get; set; }
        /// <summary>
        /// 医疗类别
        /// </summary>
        public string med_type { get; set; }
        /// <summary>
        /// 门诊/住院入院时间
        /// </summary>
        public string adm_time{ get; set; }
        /// <summary>
        /// 诊断疾病编码 
        /// </summary>
        public string adm_diag_dscr { get; set; }
        /// <summary>
        /// 病区名称 
        /// </summary>
        public string wardarea_name { get; set; }
        /// <summary>
        /// 科室编码 
        /// </summary>
        public string adm_dept_codg { get; set; }
        /// <summary>
        /// 床位号
        /// </summary>
        public string adm_bed { get; set; }
        /// <summary>
        /// 医生编码 
        /// </summary>
        public string atddr_no { get; set; }
        /// <summary>
        /// 病人联系电话
        /// </summary>
        public string tel { get; set; }
        /// <summary>
        /// 个人唯一识别码
        /// </summary>
        public string  psn_no { get; set; }
        /// <summary>
        /// 住院号
        /// </summary>
        public string ipt_no{ get; set; }
        /// <summary>
        /// 工伤医疗费资格审核信息ID
        /// </summary>
        public string qualification_id { get; set; }
    }
}
