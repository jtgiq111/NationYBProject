﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace yb_interfaces.YB.湖南.OutPatient
{
   public class YBBalanceInfo
    {
        /// <summary>
        /// 结算信息
        /// </summary>
        public Setlinfo setlinfo { get; set; }
        /// <summary>
        /// 结算基金分项信息
        /// </summary>
        public Setldetail setldetail { get; set; }
    }
}
