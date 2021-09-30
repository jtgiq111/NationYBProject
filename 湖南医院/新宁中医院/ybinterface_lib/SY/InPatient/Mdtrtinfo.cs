using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ybinterface_lib.SY.InPatient
{
    /// <summary>
    /// 就诊信息
    /// </summary>
    public class Mdtrtinfo
    {
        /// <summary>
        /// 人员编号
        /// </summary>
        public string psn_no { get; set; }
        /// <summary>
        /// 险种类型
        /// </summary>
        public string insutype { get; set; }
        /// <summary>
        /// 联系人姓名
        /// </summary>
        public string coner_name { get; set; }
        /// <summary>
        /// 联系电话
        /// </summary>
        public string tel { get; set; }
        /// <summary>
        /// 开始时间
        /// 入院时间
        ///  yyyy-MM-dd HH:mm:ss
        /// </summary>
        public string begntime { get; set; }
        /// <summary>
        /// 就诊凭证类型
        /// </summary>
        public string mdtrt_cert_type { get; set; }
        /// <summary>
        /// 就诊凭证编号
        /// 就诊凭证类型为“01”时填写电子凭证令牌，
        /// 为“02”时填写身份证号，
        /// 为“03”时填写社会保障卡卡号
        /// </summary>
        public string mdtrt_cert_no { get; set; }
        /// <summary>
        /// 医疗类别
        /// </summary>
        public string med_type { get; set; }
        /// <summary>
        /// 住院号
        /// 院内就诊流水号
        /// </summary>
        public string ipt_no { get; set; }
        /// <summary>
        /// 病历号
        /// </summary>
        public string medrcdno { get; set; }
        /// <summary>
        /// 主治医生编码
        /// </summary>
        public string atddr_no { get; set; }
        /// <summary>
        /// 主诊医师姓名
        /// </summary>
        public string chfpdr_name { get; set; }
        /// <summary>
        /// 入院诊断描述
        /// </summary>
        public string adm_diag_dscr { get; set; }
        /// <summary>
        /// 入院科室编码
        /// </summary>
        public string adm_dept_codg { get; set; }
        /// <summary>
        /// 入院科室名称
        /// </summary>
        public string adm_dept_name { get; set; }
        /// <summary>
        /// 入院床位
        /// </summary>
        public string adm_bed { get; set; }
        /// <summary>
        /// 住院主诊断代码
        /// </summary>
        public string dscg_maindiag_code { get; set; }
        /// <summary>
        /// 住院主诊断名称
        /// </summary>
        public string dscg_maindiag_name { get; set; }
        /// <summary>
        /// 主要病情描述
        /// </summary>
        public string main_cond_dscr { get; set; }
        /// <summary>
        /// 病种编码
        /// 按照标准编码填写：
        /// 按病种结算病种目录代码(bydise_setl_list_code)
        ///日间手术病种目录代码(daysrg_dise_list_code)
        /// </summary>
        public string dise_codg { get; set; }
        /// <summary>
        /// 病种名称
        /// </summary>
        public string dise_name { get; set; }
        /// <summary>
        /// 手术操作代码 日间手术病种时必填
        /// </summary>
        public string oprn_oprt_code { get; set; }
        /// <summary>
        /// 手术操作名称
        /// </summary>
        public string oprn_oprt_name { get; set; }
        /// <summary>
        /// 计划生育服务证号
        /// </summary>
        public string fpsc_no { get; set; }
        /// <summary>
        /// 生育类别
        /// </summary>
        public string matn_type { get; set; }
        /// <summary>
        /// 计划生育手术类别
        /// </summary>
        public string birctrl_type { get; set; }
        /// <summary>
        /// 晚育标志
        /// </summary>
        public string latechb_flag { get; set; }
        /// <summary>
        /// 孕周数
        /// </summary>
        public int geso_val { get; set; } = 0;
        /// <summary>
        /// 胎次
        /// </summary>
        public int fetts { get; set; } = 0;
        /// <summary>
        /// 胎儿数
        /// </summary>
        public int fetus_cnt { get; set; } = 0;
        /// <summary>
        /// 早产标志
        /// </summary>
        public string pret_flag { get; set; }
        /// <summary>
        /// 计划生育手术或生育日期yyyy-MM-dd
        /// </summary>
        public string birctrl_matn_date { get; set; }
        /// <summary>
        /// 病种类型代码
        /// </summary>
        public string dise_type_code { get; set; }
    }
}
