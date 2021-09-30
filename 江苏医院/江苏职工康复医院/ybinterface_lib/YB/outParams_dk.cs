using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ybinterface_lib
{
    public class outParams_dk
    {
        //医保读卡返回
        /*
         * 个人编号|单位编号|身份证号|姓名|性别|
         * 民族|出生日期|社会保障卡卡号|医疗待遇类别|人员参保状态|
         * 异地人员标志|统筹区号|年度|在院状态|帐户余额|
         * 本年医疗费累计|本年帐户支出累计|本年统筹支出累计|本年救助金支出累计|本年公务员补助基金累计|
         * 本年城镇居民门诊统筹支付累计|进入统筹费用累计|进入救助金费用累计|起付标准累计|本年住院次数|
         * 单位名称|年龄|参保单位类型|经办机构编码|缴费类型|
         */
        private string grbh;

        /// <summary>
        /// 个人编号（社会保险号、保险号）
        /// </summary>
        public string Grbh
        {
            get { return grbh; }
            set { grbh = value; }
        }
        private string dwbh;

        /// <summary>
        /// 单位编号
        /// </summary>
        public string Dwbh
        {
            get { return dwbh; }
            set { dwbh = value; }
        }

        private string sfhz;

        /// <summary>
        /// 身份证号
        /// </summary>
        public string Sfhz
        {
            get { return sfhz; }
            set { sfhz = value; }
        }
        private string xm;

        /// <summary>
        /// 姓名
        /// </summary>
        public string Xm
        {
            get { return xm; }
            set { xm = value; }
        }
        private string xb;

        /// <summary>
        /// 性别
        /// </summary>
        public string Xb
        {
            get { return xb; }
            set { xb = value; }
        }
        private string mz;
        /// <summary>
        /// 民族
        /// </summary>
        public string Mz
        {
            get { return mz; }
            set { mz = value; }
        }
        private string csrq;
        /// <summary>
        /// 出生日期
        /// </summary>
        public string Csrq
        {
            get { return csrq; }
            set { csrq = value; }
        }
        private string kh;
        /// <summary>
        /// 社会保障卡号(医保卡号)
        /// </summary>
        public string Kh
        {
            get { return kh; }
            set { kh = value; }
        }
        private string yldylb;
        /// <summary>
        /// 医疗待遇类别
        /// </summary>
        public string Yldylb
        {
            get { return yldylb; }
            set { yldylb = value; }
        }
        private string rycbzt;

        /// <summary>
        /// 人员参保状态（人员状态）
        /// </summary>
        public string Rycbzt
        {
            get { return rycbzt; }
            set { rycbzt = value; }
        }
        private string ydrybz;
        /// <summary>
        /// 异地人员标志
        /// </summary>
        public string Ydrybz
        {
            get { return ydrybz; }
            set { ydrybz = value; }
        }
        private string tcqh;

        /// <summary>
        /// 统筹区号(地区编号)
        /// </summary>
        public string Tcqh
        {
            get { return tcqh; }
            set { tcqh = value; }
        }
        private string nd;
        /// <summary>
        /// 年度
        /// </summary>
        public string Nd
        {
            get { return nd; }
            set { nd = value; }
        }
        private string zyzt;
        /// <summary>
        /// 在院状态
        /// </summary>
        public string Zyzt
        {
            get { return zyzt; }
            set { zyzt = value; }
        }
        private string zhye;
        /// <summary>
        /// 帐户余额
        /// </summary>
        public string Zhye
        {
            get { return zhye; }
            set { zhye = value; }
        }
        private string bnylflj;
        /// <summary>
        /// 本年医疗费累计
        /// </summary>
        public string Bnylflj
        {
            get { return bnylflj; }
            set { bnylflj = value; }
        }
        private string bnxjzclj;
        /// <summary>
        /// 本年现金支出累计
        /// </summary>
        public string Bnxjzclj
        {
            get { return bnxjzclj; }
            set { bnxjzclj = value; }
        }
        private string bnzhzclj;

        /// <summary>
        /// 本年帐户支出累计|
        /// </summary>
        public string Bnzhzclj
        {
            get { return bnzhzclj; }
            set { bnzhzclj = value; }
        }
        private string bntczclj;

        /// <summary>
        ///本年统筹支出累计
        /// </summary>
        public string Bntczclj
        {
            get { return bntczclj; }
            set { bntczclj = value; }
        }
        private string bnjzjzclj;

        /// <summary>
        ///本年救助金支出累计
        /// </summary>
        public string Bnjzjzclj
        {
            get { return bnjzjzclj; }
            set { bnjzjzclj = value; }
        }
        private string bngwybzjjlj;

        /// <summary>
        ///本年公务员补助基金累计
        /// </summary>
        public string Bngwybzjjlj
        {
            get { return bngwybzjjlj; }
            set { bngwybzjjlj = value; }
        }
        private string bnqybczclj;
        /// <summary>
        /// 本年企业补充支出累计
        /// </summary>
        public string Bnqybczclj
        {
            get { return bnqybczclj; }
            set { bnqybczclj = value; }
        }
        private string bnczjmmztczflj;

        /// <summary>
        /// 本年城镇居民门诊统筹支付累计
        /// </summary>
        public string Bnczjmmztczflj
        {
            get { return bnczjmmztczflj; }
            set { bnczjmmztczflj = value; }
        }
        private string jrtcfylj;

        /// <summary>
        /// 进入统筹费用累计
        /// </summary>
        public string Jrtcfylj
        {
            get { return jrtcfylj; }
            set { jrtcfylj = value; }
        }
        private string jrjzjfylj;

        /// <summary>
        /// 进入救助金费用累计
        /// </summary>
        public string Jrjzjfylj
        {
            get { return jrjzjfylj; }
            set { jrjzjfylj = value; }
        }
        private string jrdbfylj;
        /// <summary>
        /// 进入大病费用累计
        /// </summary>
        public string Jrdbfylj
        {
            get { return jrdbfylj; }
            set { jrdbfylj = value; }
        }
        private string jrzffylj;
        /// <summary>
        /// 进入自费累计
        /// </summary>
        public string Jrzffylj
        {
            get { return jrzffylj; }
            set { jrzffylj = value; }
        }

        private string jrzlfylj;
        /// <summary>
        /// 进入自理累计
        /// </summary>
        public string Jrzlfylj
        {
            get { return jrzlfylj; }
            set { jrzlfylj = value; }
        }

        private string qfbzlj;

        /// <summary>
        /// 起付标准累计
        /// </summary>
        public string Qfbzlj
        {
            get { return qfbzlj; }
            set { qfbzlj = value; }
        }
        private string bnzycs;

        /// <summary>
        /// 本年住院次数|
        /// </summary>
        public string Bnzycs
        {
            get { return bnzycs; }
            set { bnzycs = value; }
        }
        private string dwmc;
        /// <summary>
        /// 单位名称
        /// </summary>
        public string Dwmc
        {
            get { return dwmc; }
            set { dwmc = value; }
        }
        private string nl;

        /// <summary>
        /// 年龄|
        /// </summary>
        public string Nl
        {
            get { return nl; }
            set { nl = value; }
        }
        private string cbdwlx;

        /// <summary>
        /// 参保单位类型
        /// </summary>
        public string Cbdwlx
        {
            get { return cbdwlx; }
            set { cbdwlx = value; }
        }
        private string jbjgbm;

        /// <summary>
        /// 经办机构编码
        /// </summary>
        public string Jbjgbm
        {
            get { return jbjgbm; }
            set { jbjgbm = value; }
        }

        private string ljzyzf;

        /// <summary>
        /// 累计住院支付
        /// </summary>
        public string Ljzyzf
        {
            get { return ljzyzf; }
            set { ljzyzf = value; }
        }

        private string ljtsmzzf;
        /// <summary>
        /// 累计特殊门诊支付
        /// </summary>
        public string Ljtsmzzf
        {
            get { return ljtsmzzf; }
            set { ljtsmzzf = value; }
        }


        private string ljmzzf;
        /// <summary>
        /// 累计门诊支付
        /// </summary>
        public string Ljmzzf
        {
            get { return ljmzzf; }
            set { ljmzzf = value; }
        }


        private string jflx;
        /// <summary>
        /// 缴费类型|
        /// </summary>
        public string Jflx
        {
            get { return jflx; }
            set { jflx = value; }
        }

        private string qxmc;
        /// <summary>
        /// 区县名称(地区名称、所在区县)
        /// </summary>
        public string Qxmc
        {
            get { return qxmc; }
            set { qxmc = value; }
        }

        private string ylrylb;

        /// <summary>
        /// 医疗人员类别(个人身份)
        /// </summary>
        public string Ylrylb
        {
            get { return ylrylb; }
            set { ylrylb = value; }
        }


        private string grsf;
        /// <summary>
        /// 个人身份(东软医保)
        /// </summary>
        public string Grsf
        {
            get { return grsf; }
            set { grsf = value; }
        }

        private string gbbz;
        /// <summary>
        /// 干部标志(东软医保)
        /// </summary>
        public string Gbbz
        {
            get { return gbbz; }
            set { gbbz = value; }
        }



        private string xzzwjb;

        /// <summary>
        /// 行政职务级别
        /// </summary>
        public string Xzzwjb
        {
            get { return xzzwjb; }
            set { xzzwjb = value; }
        }

        private string jmlx;
        /// <summary>
        /// 居民类型
        /// </summary>
        public string Jmlx
        {
            get { return jmlx; }
            set { jmlx = value; }
        }

        private string jjlx;
        /// <summary>
        /// 基金类型
        /// </summary>
        public string Jjlx
        {
            get { return jjlx; }
            set { jjlx = value; }
        }


        private string cbrq;
        /// <summary>
        /// 参保日期
        /// </summary>
        public string Cbrq
        {
            get { return cbrq; }
            set { cbrq = value; }
        }

        private string jbylbxcbbz;
        /// <summary>
        /// 基本医疗保险参保标志 
        /// </summary>
        public string Jbylbxcbbz
        {
            get { return jbylbxcbbz; }
            set { jbylbxcbbz = value; }
        }
        private string dbylbxcbbz;
        /// <summary>
        /// 大病医疗保险参保标志、医疗待遇险种
        /// </summary>
        public string Dbylbxcbbz
        {
            get { return dbylbxcbbz; }
            set { dbylbxcbbz = value; }
        }
        private string gwybcylbxcbbz;
        /// <summary>
        /// 公务员补充医疗保险参保标志(公务员标志)
        /// </summary>
        public string Gwybcylbxcbbz
        {
            get { return gwybcylbxcbbz; }
            set { gwybcylbxcbbz = value; }
        }
        private string mzjzrybz;
        /// <summary>
        /// 民政救助人员标志
        /// </summary>
        public string Mzjzrybz
        {
            get { return mzjzrybz; }
            set { mzjzrybz = value; }
        }
        private string jzgbbz;
        /// <summary>
        /// 军转干部标志
        /// </summary>
        public string Jzgbbz
        {
            get { return jzgbbz; }
            set { jzgbbz = value; }
        }
        private string dbdxbz;
        /// <summary>
        /// 低保对象标志
        /// </summary>
        public string Dbdxbz
        {
            get { return dbdxbz; }
            set { dbdxbz = value; }
        }

        private string jdlkrybz;
        /// <summary>
        /// 建档立卡人员标志
        /// </summary>
        public string Jdlkrybz
        {
            get { return jdlkrybz; }
            set { jdlkrybz = value; }
        }

        private string sybxcbbz;
        /// <summary>
        /// 生育保险参保标志
        /// </summary>
        public string Sybxcbbz
        {
            get { return sybxcbbz; }
            set { sybxcbbz = value; }
        }
        private string gsbxcbbz;
        /// <summary>
        /// 工伤保险参保标志
        /// </summary>
        public string Gsbxcbbz
        {
            get { return gsbxcbbz; }
            set { gsbxcbbz = value; }
        }

        private string bnjbyljjzflj;
        /// <summary>
        /// 本年基本医疗基金支付累计
        /// </summary>
        public string Bnjbyljjzflj
        {
            get { return bnjbyljjzflj; }
            set { bnjbyljjzflj = value; }
        }
        private string bndbyljjzflj;
        /// <summary>
        /// 本年大病医疗基金支付累计（本年大病支出累计）
        /// </summary>
        public string Bndbyljjzflj
        {
            get { return bndbyljjzflj; }
            set { bndbyljjzflj = value; }
        }

        private string bngwybcyljjzflj;
        /// <summary>
        /// 本年公务员补充医疗基金支付累计(不可用)
        /// </summary>
        public string Bngwybcyljjzflj
        {
            get { return bngwybcyljjzflj; }
            set { bngwybcyljjzflj = value; }
        }
        private string jbylndsykbje;
        /// <summary>
        /// 基本医疗年度剩余可报金额(医疗账户余额)
        /// </summary>
        public string Jbylndsykbje
        {
            get { return jbylndsykbje; }
            set { jbylndsykbje = value; }
        }
        private string dbylndsykbje;
        /// <summary>
        /// 大病医疗年度剩余可报金额
        /// </summary>
        public string Dbylndsykbje
        {
            get { return dbylndsykbje; }
            set { dbylndsykbje = value; }
        }
        private string gwybcylndsykbje;
        /// <summary>
        /// 公务员补充医疗年度剩余可报金额(公补账户余额)
        /// </summary>
        public string Gwybcylndsykbje
        {
            get { return gwybcylndsykbje; }
            set { gwybcylndsykbje = value; }
        }
        private string jtmztczhye;
        /// <summary>
        /// 家庭门诊统筹账户余额
        /// </summary>
        public string Jtmztczhye
        {
            get { return jtmztczhye; }
            set { jtmztczhye = value; }
        }
        private string mtndsykbje;
        /// <summary>
        /// 门特年度剩余可报金额
        /// </summary>
        public string Mtndsykbje
        {
            get { return mtndsykbje; }
            set { mtndsykbje = value; }
        }


        private string mtbzbm;
        /// <summary>
        /// 门特病种编码 
        /// </summary>
        public string Mtbzbm
        {
            get { return mtbzbm; }
            set { mtbzbm = value; }
        }
        private string mtbzmc;
        /// <summary>
        /// 门特病种名称
        /// </summary>
        public string Mtbzmc
        {
            get { return mtbzmc; }
            set { mtbzmc = value; }
        }

        private string mtbz;
        /// <summary>
        /// 门慢，特病标志、慢性病门诊是否开通标志
        /// </summary>
        public string Mtbz
        {
            get { return mtbz; }
            set { mtbz = value; }
        }

        private string mtmsg;
        /// <summary>
        /// 门慢、特病病种说明
        /// </summary>
        public string Mtmsg
        {
            get { return mtmsg; }
            set { mtmsg = value; }
        }

        private string yllb;
        /// <summary>
        /// 医疗类别
        /// </summary>
        public string Yllb
        {
            get { return yllb; }
            set { yllb = value; }
        }

        private string bzlb;
        /// <summary>
        /// 病种类别
        /// </summary>
        public string Bzlb
        {
            get { return bzlb; }
            set { bzlb = value; }
        }

        private string ispwd;
        /// <summary>
        /// 医保卡是否有密码
        /// </summary>
        public string Ispwd
        {
            get { return ispwd; }
            set { ispwd = value; }
        }

        private string ptmzbz;
        /// <summary>
        /// 普通门诊是否开通
        /// </summary>
        public string Ptmzbz
        {
            get { return ptmzbz; }
            set { ptmzbz = value; }
        }
        private string mztcbz;
        /// <summary>
        /// 门诊统筹是否开通
        /// </summary>
        public string Mztcbz
        {
            get { return mztcbz; }
            set { mztcbz = value; }
        }
        private string ptmzyy;
        /// <summary>
        /// 门诊没开通的原因
        /// </summary>
        public string Ptmzyy
        {
            get { return ptmzyy; }
            set { ptmzyy = value; }
        }
        private string mxbmzyy;
        /// <summary>
        /// 慢性病没有开通的原因
        /// </summary>
        public string Mxbmzyy
        {
            get { return mxbmzyy; }
            set { mxbmzyy = value; }
        }
        private string mztcyy;
        /// <summary>
        /// 门诊统筹没有开通的原因
        /// </summary>
        public string Mztcyy
        {
            get { return mztcyy; }
            set { mztcyy = value; }
        }

        private string ybklx;
        /// <summary>
        /// 医保卡类型 0-正式 1-临时
        /// </summary>
        public string Ybklx
        {
            get { return ybklx; }
            set { ybklx = value; }
        }
        private string zkt;
        /// <summary>
        /// 医保卡状态
        /// </summary>
        public string Zkt
        {
            get { return zkt; }
            set { zkt = value; }
        }

        private string elmmxezc;
        /// <summary>
        /// 二类门慢限额支出
        /// </summary>
        public string Elmmxezc
        {
            get { return elmmxezc; }
            set { elmmxezc = value; }
        }

        private string elmmxesy;
        /// <summary>
        /// 二类门慢限额剩余
        /// </summary>
        public string Elmmxesy
        {
            get { return elmmxesy; }
            set { elmmxesy = value; }
        }

        private string bxgs;
        /// <summary>
        /// 保险公司
        /// </summary>
        public string Bxgs
        {
            get { return bxgs; }
            set { bxgs = value; }
        }


    }
}
