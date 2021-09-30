using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ybinterface_lib.SY.Details
{
  public  class DetailReq
    {
        public string ipt_otp_no { get; set; }
        public string mdtrt_idc { get; set; }
        public int pageNumber { get; set; }
    }

    public  class DetailreqByTime
    {
        /// <summary>
        /// 查询开始日期
        /// 按照日期格式  YYYYMMDDHH24MISS
        /// </summary>
        public string begindate { get; set; }
        /// <summary>
        /// 查询结束日期
        /// 按照日期格式  YYYYMMDDHH24MISS
        /// </summary>
        public string enddate { get; set; }
        /// <summary>
        /// 页码整
        /// 数值  未传默认值为1
        /// </summary>
        public int pageNumber { get; set; }
    }
}
