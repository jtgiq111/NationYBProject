using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace yb_interfaces.JX.InPatient
{
  public  class RegisterRequest
    {
        /// <summary>
        /// 就诊信息
        /// </summary>
        public Mdtrtinfo mdtrtinfo { get; set; }
        /// <summary>
        /// 诊断信息
        /// </summary>
        public List<Diseinfo> diseinfo { get; set; }
    }
}
