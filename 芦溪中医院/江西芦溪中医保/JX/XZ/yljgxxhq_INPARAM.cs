using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace yb_interfaces
{
    public class yljgxxhq_INPARAM : INPARAM
    {
        public yljgxxhq_in_put input { get; set; }
    }

    public class yljgxxhq_in_put
    {
        public yljgxxhq_in_data data { get; set; }
    }

    public class yljgxxhq_in_data
    {
        /// <summary>
        /// 定点医疗服务机构类型
        /// </summary>
        public string fixmedins_type { get; set; }
        /// <summary>
        /// 定点医药机构名称
        /// </summary>
        public string fixmedins_name { get; set; }
        /// <summary>
        /// 定点医药机构编号
        /// </summary>
        public string fixmedins_code { get; set; }
    }
    public class mlxz_in_data
    {
        /// <summary>
        /// 版本号
        /// </summary>
        public string ver { get; set; }
    }
    public class ylfwxmmlxz_in_data
    {
        /// <summary>
        /// 版本号
        /// </summary>
        public string ver { get; set; }
    }
    public class ybmlxxxz_in_data
    {
        /// <summary>
        /// 查询时间点
        /// </summary>
        public string query_date { get; set; }
        /// <summary>
        /// 医保目录编码
        /// </summary>
        public string hilist_code { get; set; }
        /// <summary>
        /// 参保机构医保区划
        /// </summary>
        public string insu_admdvs { get; set; }
        /// <summary>
        /// 开始日期
        /// </summary>
        public string begndate { get; set; }
        /// <summary>
        /// 医保目录名称
        /// </summary>
        public string hilist_name { get; set; }
        /// <summary>
        /// 五笔助记码
        /// </summary>
        public string wubi { get; set; }
        /// <summary>
        /// 拼音助记码
        /// </summary>
        public string pinyin { get; set; }
        /// <summary>
        /// 医疗收费项目类别
        /// </summary>
        public string med_chrgitm_type { get; set; }
        /// <summary>
        /// 收费项目等级
        /// </summary>
        public string chrgitm_lv { get; set; }
        /// <summary>
        /// 限制使用标志
        /// </summary>
        public string lmt_used_flag { get; set; }
        /// <summary>
        /// 目录类别
        /// </summary>
        public string list_type { get; set; }
        /// <summary>
        /// 医疗使用标志
        /// </summary>
        public string med_use_flag { get; set; }
        /// <summary>
        /// 生育使用标志
        /// </summary>
        public string matn_used_flag { get; set; }
        /// <summary>
        /// 医保目录使用类别
        /// </summary>
        public string hilist_use_type { get; set; }
        /// <summary>
        /// 限复方使用类型
        /// </summary>
        public string lmt_cpnd_type { get; set; }
        /// <summary>
        /// 有效标志
        /// </summary>
        public string vali_flag { get; set; }
        /// <summary>
        /// 更新时间
        /// </summary>
        public string updt_time { get; set; }
        /// <summary>
        /// 当前页数
        /// </summary>
        public string page_num { get; set; }
        /// <summary>
        /// 本页数据量
        /// </summary>
        public string page_size { get; set; }
    }
    public class ylmlyybmlppxxxz_in_data
    {
        /// <summary>
        /// 查询时间点
        /// </summary>
        public string query_date { get; set; }
        /// <summary>
        /// 定点医药机构目录编号
        /// </summary>
        public string med_list_codg { get; set; }
        /// <summary>
        /// 医保目录编码
        /// </summary>
        public string hilist_code { get; set; }
        /// <summary>
        /// 目录类别
        /// </summary>
        public string list_type { get; set; }
        /// <summary>
        /// 参保机构医保区划
        /// </summary>
        public string insu_admdvs { get; set; }
        /// <summary>
        /// 开始日期
        /// </summary>
        public string begndate { get; set; }
        /// <summary>
        /// 有效标志
        /// </summary>
        public string vali_flag { get; set; }
        /// <summary>
        /// 更新时间
        /// </summary>
        public string updt_time { get; set; }
        /// <summary>
        /// 当前页数
        /// </summary>
        public string page_num { get; set; }
        /// <summary>
        /// 本页数据量
        /// </summary>
        public string page_size { get; set; }
    }
    public class ybmlxjxxxz_in_data
    {
        /// <summary>
        /// 查询时间点
        /// </summary>
        public string query_date { get; set; }
        /// <summary>
        /// 医保目录编码
        /// </summary>
        public string hilist_code { get; set; }
        /// <summary>
        /// 医保目录限价类型
        /// </summary>
        public string hilist_lmtpric_type { get; set; }
        /// <summary>
        /// 医保目录超限处理方式
        /// </summary>
        public string overlmt_dspo_way { get; set; }
        /// <summary>
        /// 参保机构医保区划
        /// </summary>
        public string insu_admdvs { get; set; }
        /// <summary>
        /// 开始日期
        /// </summary>
        public string begndate { get; set; }
        /// <summary>
        /// 结束日期
        /// </summary>
        public string enddate { get; set; }
        /// <summary>
        /// 有效标志
        /// </summary>
        public string vali_flag { get; set; }
        /// <summary>
        /// 唯一记录号
        /// </summary>
        public string rid { get; set; }
        /// <summary>
        /// 表名
        /// </summary>
        public string tabname { get; set; }
        /// <summary>
        /// 统筹区
        /// </summary>
        public string poolarea_no { get; set; }
        /// <summary>
        /// 更新时间
        /// </summary>
        public string updt_time { get; set; }
        /// <summary>
        /// 当前页数
        /// </summary>
        public string page_num { get; set; }
        /// <summary>
        /// 本页数据量
        /// </summary>
        public string page_size { get; set; }
    }
    public class yyjgmlppxx_in_data
    {
        /// <summary>
        /// 查询时间点
        /// </summary>
        public string query_date { get; set; }
        /// <summary>
        /// 定点医药机构编号
        /// </summary>
        public string fixmedins_code { get; set; }
        /// <summary>
        /// 定点医药机构目录编号
        /// </summary>
        public string medins_list_codg { get; set; }
        /// <summary>
        /// 定点医药机构目录名称
        /// </summary>
        public string medins_list_name { get; set; }
        /// <summary>
        /// 参保机构医保区划
        /// </summary>
        public string insu_admdvs { get; set; }
        /// <summary>
        /// 目录类别
        /// </summary>
        public string list_type { get; set; }
        /// <summary>
        /// 医疗目录编码
        /// </summary>
        public string med_list_codg { get; set; }
        /// <summary>
        /// 开始日期
        /// </summary>
        public string begndate { get; set; }
        /// <summary>
        /// 有效标志
        /// </summary>
        public string vali_flag { get; set; }
        /// <summary>
        /// 更新时间
        /// </summary>
        public string updt_time { get; set; }
        /// <summary>
        /// 当前页数
        /// </summary>
        public string page_num { get; set; }
        /// <summary>
        /// 本页数据量
        /// </summary>
        public string page_size { get; set; }
    }
    public class ybmlxzfblxxxz_in_data
    {
        /// <summary>
        /// 查询时间点
        /// </summary>
        public string query_date { get; set; }
        /// <summary>
        /// 医保目录编码
        /// </summary>
        public string hilist_code { get; set; }
        /// <summary>
        /// 医保目录自付比例人员类别
        /// </summary>
        public string selfpay_prop_psn_type { get; set; }
        /// <summary>
        /// 目录自付比例类别
        /// </summary>
        public string selfpay_prop_type { get; set; }
        /// <summary>
        /// 参保机构医保区划
        /// </summary>
        public string insu_admdvs { get; set; }
        /// <summary>
        /// 开始日期
        /// </summary>
        public string begndate { get; set; }
        /// <summary>
        /// 结束日期
        /// </summary>
        public string enddate { get; set; }
        /// <summary>
        /// 有效标志
        /// </summary>
        public string vali_flag { get; set; }
        /// <summary>
        /// 唯一记录号
        /// </summary>
        public string rid { get; set; }
        /// <summary>
        /// 表名
        /// </summary>
        public string tabname { get; set; }
        /// <summary>
        /// 统筹区
        /// </summary>
        public string poolarea_no { get; set; }
        /// <summary>
        /// 更新时间
        /// </summary>
        public string updt_time { get; set; }
        /// <summary>
        /// 当前页数
        /// </summary>
        public string page_num { get; set; }
        /// <summary>
        /// 本页数据量
        /// </summary>
        public string page_size { get; set; }
    }
    public class filedownload_in_data
    {
        /// <summary>
        /// 文件查询号
        /// </summary>
        public string file_qury_no { get; set; }
        /// <summary>
        /// 文件名
        /// </summary>
        public string filename { get; set; }
        /// <summary>
        /// 医药机构编号
        /// </summary>
        public string fixmedins_code { get; set; }
    }
    public class kssc_deptinfo
    {
        /// <summary>
        /// 医院科室编码
        /// </summary>
        public string hosp_dept_codg { get; set; }
        /// <summary>
        /// 科别
        /// </summary>
        public string caty { get; set; }
        /// <summary>
        /// 医院科室名称
        /// </summary>
        public string hosp_dept_name { get; set; }
        /// <summary>
        /// 开始时间
        /// </summary>
        public string begntime { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        public string endtime { get; set; }
        /// <summary>
        /// 简介
        /// </summary>
        public string itro { get; set; }
        /// <summary>
        /// 科室负责人姓名
        /// </summary>
        public string dept_resper_name { get; set; }
        /// <summary>
        /// 科室负责人电话
        /// </summary>
        public string dept_resper_tel { get; set; }
        /// <summary>
        /// 科室医疗服务范围
        /// </summary>
        public string dept_med_serv_scp { get; set; }
        /// <summary>
        /// 科室成立日期
        /// </summary>
        public string dept_estbdat { get; set; }
        /// <summary>
        /// 批准床位数量
        /// </summary>
        public string aprv_bed_cnt { get; set; }
        /// <summary>
        /// 医保认可床位数
        /// </summary>
        public string hi_crtf_bed_cnt { get; set; }
        /// <summary>
        /// 统筹区编号
        /// </summary>
        public string poolarea_no { get; set; }
        /// <summary>
        /// 医师人数
        /// </summary>
        public string dr_psncnt { get; set; }
        /// <summary>
        /// 药师人数
        /// </summary>
        public string phar_psncnt { get; set; }
        /// <summary>
        /// 护士人数
        /// </summary>
        public string nurs_psncnt { get; set; }
        /// <summary>
        /// 技师人数
        /// </summary>
        public string tecn_psncnt { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string memo { get; set; }


    }
    public class kssccx_data
    {
        /// <summary>
        /// 医院科室编码
        /// </summary>
        public string hosp_dept_codg { get; set; }
        /// <summary>
        /// 医院科室名称
        /// </summary>
        public string hosp_dept_name { get; set; }
        /// <summary>
        /// 开始时间
        /// </summary>
        public string begntime { get; set; }


    }
    public class YBYSXXCX_data
    {
        public string prac_psn_type { get; set; }	//执业人员分类
        public string psn_cert_type { get; set; }	//人员证件类型
        public string certno { get; set; }	        //证件号码
        public string prac_psn_name { get; set; }	//执业人员姓名
        public string prac_psn_code { get; set; }	//执业人员代码
    }
    public class zyxxcx_data
    {
        /// <summary>
        /// 人员编号
        /// </summary>
        public string psn_no { get; set; }
        /// <summary>
        ///      开始时间 日期时间型
        /// </summary>
        public string begntime { get; set; }
        /// <summary>
        /// 结束时间    日期时间型
        /// </summary>
        public string endtime { get; set; }



    }
    public class JZXXCX_data
    {
        /// <summary>
        /// 人员编号
        /// </summary>
        public string psn_no { get; set; }
        /// <summary>
        /// 开始时间 日期时间型
        /// </summary>
        public string begntime { get; set; }
        /// <summary>
        /// 结束时间    日期时间型
        /// </summary>
        public string endtime { get; set; }
        /// <summary>
        /// 医疗类别    
        /// </summary>
        public string med_type { get; set; }
        /// <summary>
        /// 就诊ID    
        /// </summary>
        public string mdtrt_id { get; set; }
    }
    public class ZDXXCX_data
    {

        /// <summary>
        /// 就诊ID    
        /// </summary>
        public string mdtrt_id { get; set; }
        /// <summary>
        /// 人员编号
        /// </summary>
        public string psn_no { get; set; }
    }

    public class FYMXCX_data
    {

        /// <summary>
        /// 人员编号
        /// </summary>
        public string psn_no { get; set; }
        /// <summary>
        /// 结算ID
        /// </summary>
        public string setl_id { get; set; }
        /// <summary>
        /// 就诊ID
        /// </summary>
        public string mdtrt_id { get; set; }
    }
    public class MMYYCX_data
    {

        /// <summary>
        /// 人员编号
        /// </summary>
        public string psn_no { get; set; }
        /// <summary>
        /// 开始时间
        /// </summary>
        public string begntime { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        public string endtime { get; set; }
    }

    public class ryljcx_data
    {

        /// <summary>
        /// 人员编号
        /// </summary>
        public string psn_no { get; set; }
        /// <summary>
        /// 累计年月
        /// </summary>
        public string cum_ym { get; set; }
    }
    public class RYMTBBA_data
    {

        /// <summary>
        /// 人员编号
        /// </summary>
        public string psn_no { get; set; }
    }

    public class RYDDXX_data
    {

        /// <summary>
        /// 人员编号
        /// </summary>
        public string psn_no { get; set; }
        /// <summary>
        /// 业务申请类型
        /// </summary>
        public string biz_appy_type { get; set; }
    }
    public class jsxxcx_in_data
    {
        /// <summary>
        /// 人员编号
        /// </summary>
        public string psn_no { get; set; }
        /// <summary>
        /// 结算ID
        /// </summary>
        public string setl_id { get; set; }
        /// <summary>
        /// 就诊ID
        /// </summary>
        public string mdtrt_id { get; set; }
    }
    public class zhyxxcx_data
    {
        /// <summary>
        /// 人员编号
        /// </summary>
        public string psn_no { get; set; }
        /// <summary>
        ///      开始时间 日期时间型
        /// </summary>
        public string begntime { get; set; }
        /// <summary>
        /// 结束时间    日期时间型
        /// </summary>
        public string endtime { get; set; }



    }
    public class xmhrxxcx_bilgiteminfo
    {
        /// <summary>
        /// 人员编号
        /// </summary>
        public string psn_no { get; set; }
        /// <summary>
        /// 检查机构代码
        /// </summary>
        public string exam_org_code { get; set; }
        /// <summary>
        /// 检查机构名称
        /// </summary>
        public string exam_org_name { get; set; }
        /// <summary>
        /// 检查-项目代码
        /// </summary>
        public string exam_item_code { get; set; }
        /// <summary>
        /// 检查-项目名称
        /// </summary>
        public string exam_item_name { get; set; }


    }
    public class bgmxxxcx_rptdetailinfo
    {
        /// <summary>
        /// 人员编号
        /// </summary>
        public string psn_no { get; set; }
        /// <summary>
        /// 报告单号
        /// </summary>
        public string rpotc_no { get; set; }
        /// <summary>
        /// 机构编号
        /// </summary>
        public string fixmedins_code { get; set; }


    }
    public class _9001_in_data
    {
        /// <summary>
        /// 操作员编号
        /// </summary>
        public string opter_no { get; set; }
        /// <summary>
        /// 签到MAC地址
        /// </summary>
        public string mac { get; set; }
        /// <summary>
        /// 签到IP地址
        /// </summary>
        public string ip { get; set; }
    }
}
