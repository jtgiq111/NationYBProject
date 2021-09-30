
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ybinterface_lib.JX.Enum
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
        普通门诊=11,
        急诊=13,
        门诊慢特病=14,
        定点药店购药=41,
        生育门诊=51,
        生育住院=52,
        普通住院=2101,
        单病种住院=2102,
        生育平产=2106,
        生育剖宫产=2107,
        外伤住院=22,
        门诊两病=9901,
        意外伤害门诊=9903,
        大病特药=9904
    }
    /// <summary>
    /// 中药使用方式
    /// </summary>
    public enum tcmdrug_used_way
    {
        复方=1,
        单方=2
    }
    /// <summary>
    /// 出院带药标志
    /// </summary>
    public enum dscg_tkdrug_flag
    {
        否=0,
        是=1
    }
    /// <summary>
    /// 就诊凭证类型
    /// </summary>
    public enum mdtrt_cert_type
    {
        医保电子凭证 = 01,
        居民身份证 = 02,
        社会保障卡=03
    }

    /// <summary>
    /// 险种类型
    /// </summary>
    public enum insutype
    {
        职工基本医疗保险=310,
        公务员医疗补助=320,
        大额医疗费用补助=330,
        离休人员医疗保障=340,
        一至六级残废军人医疗补助=350,
        老红军医疗保障=360,
        企业补充医疗保险=370,
        新型农村合作医疗=380,
        城乡居民基本医疗保险=390,
        城镇居民基本医疗保险=391,
        城乡居民大病医疗保险=392,
        其他特殊人员医疗保障=399,
        长期照护保险=410,
        生育保险=510
    }
    /// <summary>
    /// 计划生育手术类别
    /// </summary>
    public enum birctrl_type
    {
        放置宫内节育器=1,
        取出宫内节育器=2,
        流产术=3,
        引产术=4,
        绝育手术=5,
        绝育复通手术=6,
        绝育手术输精管=7,
        绝育复通手术输精管=8,
        其他=9
    }

}
