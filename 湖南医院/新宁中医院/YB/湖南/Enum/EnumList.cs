
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace yb_interfaces.YB.湖南.Enum
{
    class EnumList
    {
    }
    /// <summary>
    /// 患者证件类别
    /// </summary>
    public enum patn_cert_type
    {
        居民身份证 = 01,
        居民户口簿 = 02,
        护照 = 03,
        军官证 = 04,
        驾驶证 = 05,
        港澳居民来往内地通行证 = 06,
        台湾居民来往内地通行证 = 07,
        其他身份证件 = 08

    }
    /// <summary>
    /// 医疗类别
    /// </summary>
    public enum med_type
    {
        普通门诊 = 11,
        急诊 = 13,
        门诊慢特病 = 14,
        定点药店购药 = 41,
        生育门诊 = 51,
        生育住院 = 52,
        普通住院 = 2101,
        单病种住院 = 2102,
        生育平产 = 2106,
        生育剖宫产 = 2107,
        外伤住院 = 22,
        门诊两病 = 9901,
        意外伤害门诊 = 9903,
        大病特药 = 9904
    }
    /// <summary>
    /// 中药使用方式
    /// </summary>
    public enum tcmdrug_used_way
    {
        复方 = 1,
        单方 = 2
    }
    /// <summary>
    /// 出院带药标志
    /// </summary>
    public enum dscg_tkdrug_flag
    {
        否 = 0,
        是 = 1
    }
    /// <summary>
    /// 就诊凭证类型
    /// </summary>
    public enum mdtrt_cert_type
    {
        医保电子凭证 = 01,
        居民身份证 = 02,
        社会保障卡 = 03
    }

    /// <summary>
    /// 险种类型
    /// </summary>
    public enum insutype
    {
        职工基本医疗保险 = 310,
        公务员医疗补助 = 320,
        大额医疗费用补助 = 330,
        离休人员医疗保障 = 340,
        一至六级残废军人医疗补助 = 350,
        老红军医疗保障 = 360,
        企业补充医疗保险 = 370,
        新型农村合作医疗 = 380,
        城乡居民基本医疗保险 = 390,
        城镇居民基本医疗保险 = 391,
        城乡居民大病医疗保险 = 392,
        其他特殊人员医疗保障 = 399,
        长期照护保险 = 410,
        生育保险 = 510
    }

    /// <summary>
    /// 离院方式
    /// </summary>
    public enum dscg_way
    {
        医嘱离院 = 1,
        医嘱转院 = 2,
        医嘱转社区卫生服务机构 = 3,
        非医嘱离院 = 4,
        死亡 = 5,
        其他 = 9
    }
    /// <summary>
    /// 生育类别
    /// </summary>
    public enum math_type
    {
        正常产 = 1,
        助娩产 = 2,
        剖宫产 = 3,
        难产 = 4,
        流产 = 5,
        引产 = 6,
        产前检查 = 7,
        其他 = 9
    }
    /// <summary>
    /// 计划生育手术类别
    /// </summary>
    public enum birctrl_type
    {
        放置宫内节育器 = 1,
        取出宫内节育器 = 2,
        流产术 = 3,
        引产术 = 4,
        绝育手术 = 5,
        绝育复通手术 = 6,
        绝育手术输精管 = 7,
        绝育复通手术输精管 = 8,
        其他 = 9
    }
    /// <summary>
    /// 新生儿标志
    /// </summary>
    public enum nwb_flag
    {
        是 = 1,
        否 = 0
    }
    /// <summary>
    /// 医院审批标志
    /// </summary>
    public enum hosp_appr_flag
    {
        无须审批 = 0,
        审批通过 = 1,
        审批不通过 = 2
    }
    /// <summary>
    /// 晚育标志
    /// </summary>
    public enum latechb_flag
    {
        是 = 1,
        否 = 0
    }
    /// <summary>
    /// 诊断类别
    /// </summary>
    public enum diag_type
    {
        西医诊断 = 1,
        中医主病诊断 = 2,
        中医主证诊断 = 3
    } 
    ///医保业务代码方法对应
    public enum YBYWDM
    {
        //通用业务代码
        YBINIT = 9000, //初始化
        YBEXIT = 9100, //退出
        YBMZDK = 2101, //门诊读卡
        YBMZDK1 = 2102, //门诊读卡
        YBZYDK = 2201, //住院读卡
        YBMZDJ = 3100, //门诊登记(挂号)
        YBMZDJCX = 3101, //门诊登记(挂号)撤销
        YBMZDJSF = 3102, //门诊登记(挂号)收费
        YBMZDJSFCX = 3103, //门诊登记(挂号)收费撤销
        YBMZZDDJ = 3104,  //门诊登记(挂号)_自动
        YBMZCFMXSC = 3200, //门诊处方明细上传(门诊费用登记)
        YBMZCFMXSCCX = 3201,//门诊处方明细上传撤销(门诊费用登记撤销)
        YBMZCFMXFHCX = 3203, //门诊上传处方查询
        N_YBMZSFSCCX = 3204,//内门诊处方明细上传撤销(门诊费用登记撤销)
        YBMZSFYJS = 3300, //门诊费用预结算
        YBMZSFJS = 3301, //门诊费用结算
        YBMZSFJSCX = 3302, //门诊费用结算撤销
        YBMZJSD = 3303, //门诊费用结算单
        YBZYDJ = 4100, //住院登记
        YBZYDJBG = 4101, //住院登记变更
        YBZYDJCX = 4102, //住院登记撤销
        YBZYBQDJ = 4200,  //住院病情登记
        YBZYBQDJXG = 4201, //住院病情登记修改
        YBZYSFDJ = 4300, //住院费用登记/住院费用明费上传
        YBZYSFDJCX = 4301, //住院费用登记撤销/住院费用明费上传撤销
        YBZYSFYJS = 4400, //住院费用预结算
        YBZYSFJS = 4401, //住院费用结算
        YBZYSFJSCX = 4402, //住院费用结算撤销
        YBZYJSD = 4403, //住院结算单打印
        YBZYJSFYD = 4404, //离院结算费用报表
        YBZYJSXXCX = 4405, //住院结算明细查询
        YBCZJY = 4500,  //冲正交易
        YBFYMXXZ = 5100, //医疗费信息查询
        YBJSXXCX = 5101,  //个人结算信息查询
        YBGRYLFXXCX = 5102, //个人医疗费信息查询
        YBGRJZDJXXCX = 5103,  //个人就诊登记信息查询
        YBYSXXCX = 5104,  //医师信息查询
        YBZEDZ = 5105,  //总额对帐
        YBMXDZ = 5106,  //明细对帐
        YBYJSFYDZ = 5107, //月结算费用对帐
        YBPLSJCXXZ = 5108,    //批量数据查询下载
        YBGRJBXXJZHXXCX = 5109,    //个人基本信息及帐户信息查询
        YBYLDYFSXXCX = 5110, //医疗待遇封锁信息查询
        YBYLDYSPXXCX = 5111, //医疗待遇审批信息查询
        YBZXTZXXCX = 5112, //中心通知信息查询
        YBHQZXDSJCX = 5113, //获取中心端时间查询
        YBBZCX = 5115,    //医保病种查询
        YBDZXXPLSC = 5200, //医保对照信息批量上传
        YBDZXXSCCX = 5201, //医保对照信息上传撤销
        YBKSXXPLSC = 5203,//科室信息批量上传
        YBYSXXPLSC = 5204, //医师信息批量上传
        GetJZLSH = 5500,//获取医保号(南京汤山公费医疗)
        NYBMZDJCX = 6100,	//单边_医保门诊登记撤销
        NYBMZCFMXCX = 6101,	//单边_医保门诊处方明细撤销
        NYBMZFYJSCX = 6102,	//单边_医保门诊费用结算撤销
        NYBZYDJCX = 6200,	//单边_医保住院登记撤销
        NYBZYSFDJCX = 6201,	//单边_医保住院费用明费上传撤销
        NYBZYSFJSCX = 6202,	//单边_医保住院费用结算撤销
        YBWSDJBA = 1008,    //外伤登记备案
        YBWSDJBACX = 1009,   //外伤登记备案撤销
        YBSYDYBA = 1010,    //生育登记备案
        YBSYDYBACX = 1011,   //生育登记备案撤销
        YBZYXXCX = 1012,
        YBJZXXCX = 1013,
    }
    /// <summary>
    /// 目录类别
    /// </summary>
    public enum list_type {
        西药中成药=101,
        中药饮片=102,
        自制剂=103,
        民族药=104,
        医疗服务项目=201,
        医用耗材=301
    }

}
