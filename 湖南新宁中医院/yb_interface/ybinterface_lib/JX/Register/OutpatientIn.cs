using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ybinterface_lib.JX.Register
{
    /// <summary>
    /// 就诊信息上传
    /// </summary>
    public class OutpatientIn
    {
        /// <summary>
        /// 就诊信息
        /// </summary>
        public PatientInfo mdtrtinfo { get; set; }
        public List<DiagInfo> diseinfo { get; set; }
    }
    /// <summary>
    /// 挂号基本信息
    /// </summary>
    public class PatientInfo
    {
        /// <summary>
        /// 就诊ID
        /// </summary>
        public string mdtrt_id { get; set; }
        /// <summary>
        /// 人员编号
        /// </summary>
        public string psn_no { get; set; }
        /// <summary>
        /// 医疗类别
        /// </summary>
        public string med_type { get; set; }
        /// <summary>
        /// 开始时间
        /// </summary>
        public string begntime { get; set; }
        /// <summary>
        /// 主要病情描述
        /// </summary>
        public string main_cond_dscr { get; set; }
        /// <summary>
        /// 病种编码
        /// </summary>
        public string dise_codg { get; set; }
        /// <summary>
        /// 病种名称
        /// 按照标准编码填写：
        ///按病种结算病种目录代码(bydise_setl_list_code)、
        ///门诊慢特病病种目录代码(opsp_dise_cod)、
        /// </summary>
        public string dise_name { get; set; }
        /// <summary>
        /// 计划生育手术类别
        /// 生育门诊按需录入
        /// </summary>
        public string birctrl_type { get; set; }
        /// <summary>
        /// 计划生育手术或生育日期
        /// 生育门诊按需录入，yyyy-MM-dd
        /// </summary>
        public string birctrl_matn_date { get; set; }
    }
    /// <summary>
    /// 诊断信息
    /// </summary>
    public class DiagInfo
    {
        /// <summary>
        /// 诊断类别 1 西医诊断  2 中医主病诊断 3 中医主证诊断
        /// </summary>
        public string  diag_type { get; set; }
        /// <summary>
        /// 诊断排序号
        /// </summary>
        public string diag_srt_no { get; set; }
        /// <summary>
        /// 诊断代码
        /// </summary>
        public string diag_code { get; set; }
        /// <summary>
        /// 诊断名称
        /// </summary>
        public string diag_name { get; set; }
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
        /// <summary>
        /// 有效标志 0 无效 1 有效
        /// </summary>
        public string vali_flag { get; set; }
    }
}
