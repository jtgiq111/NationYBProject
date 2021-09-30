using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ybinterface_lib.SY.Config
{
    public class Item
    {
        public string YLJGBH
        {
            get;
            set;
        }

        public string ZXBM
        {
            get;
            set;
        }

        public string LJBS
        {
            get;
            set;
        }

        public string SrcCon
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }

        public string TableName
        {
            get;
            set;
        }

        public string CreateSql
        {
            get;
            set;
        }

        public string SelectSql
        {
            get;
            set;
        }

        public string DDYLJGBH
        {
            get;
            set;
        }

        public string DDYLJGMC
        {
            get;
            set;
        }

        public string YBIP
        {
            get;
            set;
        }
        /// <summary>
        /// 医保ip
        /// </summary>
        public string YBPORT
        {
            get;
            set;
        }

        public string Gocent
        {
            get;
            set;
        }

        public string TIMEOUT
        {
            get;
            set;
        }

        public string YBZDSCGH
        {
            get;
            set;
        }
        /// <summary>
        /// 医保服务地址
        /// </summary>
        public string YBServiceUrl { get; set; }
        /// <summary>
        /// 医保就医地医保区划
        /// </summary>
        public string YBJYYBQH { get; set; }
        /// <summary>
        /// 接收方医院代码
        /// </summary>
        public string JSFDM { get; set; }
    }
}
