using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ybinterface_lib.SY.YB
{
    public class YBZZ
    {
        /// <summary>
        /// 转院开始日期
        /// YYYYMMDD
        /// </summary>
        public string actualStartTime { get; set; }
        /// <summary>
        /// 转院终止日期
        /// </summary>
        public string actualEndTime { get; set; }
        /// <summary>
        /// 转外区划 
        /// 
        /// </summary>
        public string outArea { get; set; }
        /// <summary>
        /// 转外医院名称
        /// </summary>
        public string outHospitalName { get; set; }
        /// <summary>
        /// 申请理由
        /// </summary>
        public string applyReason { get; set; }
        /// <summary>
        /// 交通工具
        /// </summary>
        public string transportation { get; set; }
        /// <summary>
        /// 所属区县
        /// </summary>
        public string stateCode { get; set; }
        /// <summary>
        /// 申请时间
        /// </summary>
        public string applyTime { get; set; }
        /// <summary>
        /// 工伤医疗费资格审核信息ID
        /// 通过读卡交易出参“工伤诊断结论”中获取
        /// </summary>
        public string qualification_id { get; set; }
    }
}
