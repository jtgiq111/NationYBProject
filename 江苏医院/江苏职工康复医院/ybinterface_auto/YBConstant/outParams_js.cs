using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ybinterface_auto
{
    [Serializable]
    public class outParams_js
    {
        private string ylfze;
        /// <summary>
        /// 医疗费总额
        /// </summary>
        public string Ylfze
        {
            get { return ylfze; }
            set { ylfze = value; }
        }
        private string zbxje;
        /// <summary>
        /// 总报销金额
        /// </summary>
        public string Zbxje
        {
            get { return zbxje; }
            set { zbxje = value; }
        }
        private string tcjjzf;
        /// <summary>
        /// 统筹基金支付
        /// </summary>
        public string Tcjjzf
        {
            get { return tcjjzf; }
            set { tcjjzf = value; }
        }
        private string dejjzf;
        /// <summary>
        /// 大额基金支付、大病医疗基金支付
        /// </summary>
        public string Dejjzf
        {
            get { return dejjzf; }
            set { dejjzf = value; }
        }
        private string zhzf;
        /// <summary>
        /// 账户支付
        /// </summary>
        public string Zhzf
        {
            get { return zhzf; }
            set { zhzf = value; }
        }
        private string xjzf;
        /// <summary>
        /// 现金支付
        /// </summary>
        public string Xjzf
        {
            get { return xjzf; }
            set { xjzf = value; }
        }
        private string gwybzjjzf;
        /// <summary>
        /// 公务员补助基金支付
        /// </summary>
        public string Gwybzjjzf
        {
            get { return gwybzjjzf; }
            set { gwybzjjzf = value; }
        }
        private string qybcylbxjjzf;
        /// <summary>
        /// 企业补充医疗保险基金支付
        /// </summary>
        public string Qybcylbxjjzf
        {
            get { return qybcylbxjjzf; }
            set { qybcylbxjjzf = value; }
        }
        private string zffy;
        /// <summary>
        /// 自费费用
        /// </summary>
        public string Zffy
        {
            get { return zffy; }
            set { zffy = value; }
        }
        private string dwfdfy;
        /// <summary>
        /// 单位负担费用
        /// </summary>
        public string Dwfdfy
        {
            get { return dwfdfy; }
            set { dwfdfy = value; }
        }
        private string yyfdfy;
        /// <summary>
        /// 医院负担费用 定点机构支付
        /// </summary>
        public string Yyfdfy
        {
            get { return yyfdfy; }
            set { yyfdfy = value; }
        }
        private string mzjzfy;
        /// <summary>
        /// 民政救助费用
        /// </summary>
        public string Mzjzfy
        {
            get { return mzjzfy; }
            set { mzjzfy = value; }
        }
        private string cxjfy;
        /// <summary>
        /// 超限价费用
        /// </summary>
        public string Cxjfy
        {
            get { return cxjfy; }
            set { cxjfy = value; }
        }
        private string ylzlfy;
        /// <summary>
        /// 乙类自理费用、乙类药品自付
        /// </summary>
        public string Ylzlfy
        {
            get { return ylzlfy; }
            set { ylzlfy = value; }
        }
        private string blzlfy;
        /// <summary>
        /// 丙类自理费用
        /// </summary>
        public string Blzlfy
        {
            get { return blzlfy; }
            set { blzlfy = value; }
        }
        private string fhjjylfy;
        /// <summary>
        /// 符合基本医疗费用
        /// </summary>
        public string Fhjjylfy
        {
            get { return fhjjylfy; }
            set { fhjjylfy = value; }
        }
        private string qfbzfy;
        /// <summary>
        /// 起付标准费用、起付段自付
        /// </summary>
        public string Qfbzfy
        {
            get { return qfbzfy; }
            set { qfbzfy = value; }
        }
        private string zzzyzffy;
        /// <summary>
        /// 转诊转院自付费用、转外自付、转诊先自付
        /// </summary>
        public string Zzzyzffy
        {
            get { return zzzyzffy; }
            set { zzzyzffy = value; }
        }
        private string jrtcfy;
        /// <summary>
        /// 进入统筹费用、统筹段基金支付
        /// </summary>
        public string Jrtcfy
        {
            get { return jrtcfy; }
            set { jrtcfy = value; }
        }
        private string tcfdzffy;
        /// <summary>
        /// 统筹分段自付费用、统筹段自付
        /// </summary>
        public string Tcfdzffy
        {
            get { return tcfdzffy; }
            set { tcfdzffy = value; }
        }
        private string ctcfdxfy;
        /// <summary>
        /// 超统筹封顶线费用
        /// </summary>
        public string Ctcfdxfy
        {
            get { return ctcfdxfy; }
            set { ctcfdxfy = value; }
        }
        private string jrdebxfy;
        /// <summary>
        /// 进入大额报销费用、大病段基金支付
        /// </summary>
        public string Jrdebxfy
        {
            get { return jrdebxfy; }
            set { jrdebxfy = value; }
        }
        private string defdzffy;
        /// <summary>
        /// 大额分段自付费用、大病统筹自付
        /// </summary>
        public string Defdzffy
        {
            get { return defdzffy; }
            set { defdzffy = value; }
        }
        private string cdefdxfy;
        /// <summary>
        /// 超大额封顶线费用
        /// </summary>
        public string Cdefdxfy
        {
            get { return cdefdxfy; }
            set { cdefdxfy = value; }
        }
        private string rgqgzffy;
        /// <summary>
        /// 人工器官自付费用、安装器官费用
        /// </summary>
        public string Rgqgzffy
        {
            get { return rgqgzffy; }
            set { rgqgzffy = value; }
        }
        private string bcjsqzhye;
        /// <summary>
        /// 本次结算前帐户余额
        /// </summary>
        public string Bcjsqzhye
        {
            get { return bcjsqzhye; }
            set { bcjsqzhye = value; }
        }

        private string bcjshzhye;
        /// <summary>
        /// 本次结算后账户余额
        /// </summary>
        public string Bcjshzhye
        {
            get { return bcjshzhye; }
            set { bcjshzhye = value; }
        }


        private string bntczflj;
        /// <summary>
        /// 本年统筹支付累计(不含本次)
        /// </summary>
        public string Bntczflj
        {
            get { return bntczflj; }
            set { bntczflj = value; }
        }
        private string bndezflj;
        /// <summary>
        /// 本年大额支付累计(不含本次)
        /// </summary>
        public string Bndezflj
        {
            get { return bndezflj; }
            set { bndezflj = value; }
        }
        private string bnczjmmztczflj;
        /// <summary>
        /// 本年城镇居民门诊统筹支付累计(不含本次)
        /// </summary>
        public string Bnczjmmztczflj
        {
            get { return bnczjmmztczflj; }
            set { bnczjmmztczflj = value; }
        }
        private string bngwybzzflj;
        /// <summary>
        /// 本年公务员补助支付累计(不含本次)
        /// </summary>
        public string Bngwybzzflj
        {
            get { return bngwybzzflj; }
            set { bngwybzzflj = value; }
        }
        private string bnzhzflj;
        /// <summary>
        /// 本年账户支付累计(不含本次)
        /// </summary>
        public string Bnzhzflj
        {
            get { return bnzhzflj; }
            set { bnzhzflj = value; }
        }
        private string bnzycslj;
        /// <summary>
        /// 本年住院次数累计(不含本次)
        /// </summary>
        public string Bnzycslj
        {
            get { return bnzycslj; }
            set { bnzycslj = value; }
        }
        private string zycs;
        /// <summary>
        /// 住院次数
        /// </summary>
        public string Zycs
        {
            get { return zycs; }
            set { zycs = value; }
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
        private string jsrq;
        /// <summary>
        /// 结算日期
        /// </summary>
        public string Jsrq
        {
            get { return jsrq; }
            set { jsrq = value; }
        }


        private string jssj;
        /// <summary>
        /// 结算时间
        /// </summary>
        public string Jssj
        {
            get { return jssj; }
            set { jssj = value; }
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
        private string yldylb;
        /// <summary>
        /// 医疗待遇类别
        /// </summary>
        public string Yldylb
        {
            get { return yldylb; }
            set { yldylb = value; }
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
        private string ywzqh;
        /// <summary>
        /// 业务周期号
        /// </summary>
        public string Ywzqh
        {
            get { return ywzqh; }
            set { ywzqh = value; }
        }
        private string jslsh;
        /// <summary>
        /// 结算流水号
        /// </summary>
        public string Jslsh
        {
            get { return jslsh; }
            set { jslsh = value; }
        }
        private string tsxx;
        /// <summary>
        /// 提示信息
        /// </summary>
        public string Tsxx
        {
            get { return tsxx; }
            set { tsxx = value; }
        }
        private string djh;
        /// <summary>
        /// 单据号
        /// </summary>
        public string Djh
        {
            get { return djh; }
            set { djh = value; }
        }
        private string jyxl;
        /// <summary>
        /// 交易类型
        /// </summary>
        public string Jyxl
        {
            get { return jyxl; }
            set { jyxl = value; }
        }
        private string yyjylsh;
        /// <summary>
        /// 医院交易流水号
        /// </summary>
        public string Yyjylsh
        {
            get { return yyjylsh; }
            set { yyjylsh = value; }
        }
        private string yxbz;
        /// <summary>
        /// 有效标志
        /// </summary>
        public string Yxbz
        {
            get { return yxbz; }
            set { yxbz = value; }
        }
        private string grbhgl;
        /// <summary>
        /// 个人编号管理
        /// </summary>
        public string Grbhgl
        {
            get { return grbhgl; }
            set { grbhgl = value; }
        }
        private string yljgbm;
        /// <summary>
        /// 医疗机构编码
        /// </summary>
        public string Yljgbm
        {
            get { return yljgbm; }
            set { yljgbm = value; }
        }
        private string ecbcje;
        /// <summary>
        /// 二次补偿金额、大病二次补偿
        /// </summary>
        public string Ecbcje
        {
            get { return ecbcje; }
            set { ecbcje = value; }
        }
        private string mmqflj;
        /// <summary>
        /// 门慢起付累计
        /// </summary>
        public string Mmqflj
        {
            get { return mmqflj; }
            set { mmqflj = value; }
        }
        private string jsfjylsh;
        /// <summary>
        /// 接收方交易流水号
        /// </summary>
        public string Jsfjylsh
        {
            get { return jsfjylsh; }
            set { jsfjylsh = value; }
        }
        private string grbh;
        /// <summary>
        /// 个人编号、保险号
        /// </summary>
        public string Grbh
        {
            get { return grbh; }
            set { grbh = value; }
        }
        private string dbzbc;
        /// <summary>
        /// 单病种补差
        /// </summary>
        public string Dbzbc
        {
            get { return dbzbc; }
            set { dbzbc = value; }
        }
        private string czzcje;
        /// <summary>
        /// 财政支出金额
        /// </summary>
        public string Czzcje
        {
            get { return czzcje; }
            set { czzcje = value; }
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
        private string jmgrzfecbcje;
        /// <summary>
        /// 居民个人自付二次补偿金额
        /// </summary>
        public string Jmgrzfecbcje
        {
            get { return jmgrzfecbcje; }
            set { jmgrzfecbcje = value; }
        }
        private string tjje;
        /// <summary>
        /// 体检金额
        /// </summary>
        public string Tjje
        {
            get { return tjje; }
            set { tjje = value; }
        }
        private string syjjzf;
        /// <summary>
        /// 生育基金支付
        /// </summary>
        public string Syjjzf
        {
            get { return syjjzf; }
            set { syjjzf = value; }
        }
        private string bcmzbzzf;
        /// <summary>
        /// 本次民政补助支付
        /// </summary>
        public string Bcmzbzzf
        {
            get { return bcmzbzzf; }
            set { bcmzbzzf = value; }
        }
        private string ybfwnfy;
        /// <summary>
        /// 医保范围内费用
        /// </summary>
        public string Ybfwnfy
        {
            get { return ybfwnfy; }
            set { ybfwnfy = value; }
        }
        private string yblx;
        /// <summary>
        /// 医保类型
        /// </summary>
        public string Yblx
        {
            get { return yblx; }
            set { yblx = value; }
        }

        private string kh;
        /// <summary>
        /// 医保卡号、社会保障卡号
        /// </summary>
        public string Kh
        {
            get { return kh; }
            set { kh = value; }
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

        private string csrq;
        /// <summary>
        /// 出生日期
        /// </summary>
        public string Csrq
        {
            get { return csrq; }
            set { csrq = value; }
        }
        private string dqbh;
        /// <summary>
        /// 地区编码、统筹区号
        /// </summary>
        public string Dqbh
        {
            get { return dqbh; }
            set { dqbh = value; }
        }
        private string dqmc;
        /// <summary>
        /// 地区名称、区县名称
        /// </summary>
        public string Dqmc
        {
            get { return dqmc; }
            set { dqmc = value; }
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
        private string cbsf;
        /// <summary>
        /// 参保身份
        /// </summary>
        public string Cbsf
        {
            get { return cbsf; }
            set { cbsf = value; }
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
        private string yldyxslb;
        /// <summary>
        /// 医疗待遇享受类别
        /// </summary>
        public string Yldyxslb
        {
            get { return yldyxslb; }
            set { yldyxslb = value; }
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
        private string dwlx;
        /// <summary>
        /// 单位类别
        /// </summary>
        public string Dwlx
        {
            get { return dwlx; }
            set { dwlx = value; }
        }
        private string ybjzlsh;
        /// <summary>
        /// 中心交易流水号、医保就诊流水号
        /// </summary>
        public string Ybjzlsh
        {
            get { return ybjzlsh; }
            set { ybjzlsh = value; }
        }
        private string zxjsdjh;
        /// <summary>
        /// 结算单据号、中心结算单据号
        /// </summary>
        public string Zxjsdjh
        {
            get { return zxjsdjh; }
            set { zxjsdjh = value; }
        }

        private string jsqsrq;
        /// <summary>
        /// 结算起始日期
        /// </summary>
        public string Jsqsrq
        {
            get { return jsqsrq; }
            set { jsqsrq = value; }
        }
        private string jsjzrq;
        /// <summary>
        /// 结算截止日期
        /// </summary>
        public string Jsjzrq
        {
            get { return jsjzrq; }
            set { jsjzrq = value; }
        }
        private string zgjbyljjzf;
        /// <summary>
        /// 职工基本医疗基金支付
        /// </summary>
        public string Zgjbyljjzf
        {
            get { return zgjbyljjzf; }
            set { zgjbyljjzf = value; }
        }
        private string jzjmjbyljjzf;
        /// <summary>
        /// 城镇居民基本医疗基金支付
        /// </summary>
        public string Jzjmjbyljjzf
        {
            get { return jzjmjbyljjzf; }
            set { jzjmjbyljjzf = value; }
        }
        private string cxjmjbyljjzf;
        /// <summary>
        /// 城乡居民基本医疗基金支付
        /// </summary>
        public string Cxjmjbyljjzf
        {
            get { return cxjmjbyljjzf; }
            set { cxjmjbyljjzf = value; }
        }

        private string eyzxyljjzf;
        /// <summary>
        /// 二乙专项医疗基金支付
        /// </summary>
        public string Eyzxyljjzf
        {
            get { return eyzxyljjzf; }
            set { eyzxyljjzf = value; }
        }

        private string lhjzxyljjzf;
        /// <summary>
        /// 老红军专项医疗基金支付
        /// </summary>
        public string Lhjzxyljjzf
        {
            get { return lhjzxyljjzf; }
            set { lhjzxyljjzf = value; }
        }

        private string lxgbdttcjjzf;
        /// <summary>
        /// 离休干部单独统筹基金支付
        /// </summary>
        public string Lxgbdttcjjzf
        {
            get { return lxgbdttcjjzf; }
            set { lxgbdttcjjzf = value; }
        }

        private string ylbjzf;
        /// <summary>
        /// 医疗保键支付
        /// </summary>
        public string Ylbjzf
        {
            get { return ylbjzf; }
            set { ylbjzf = value; }
        }

        private string qtjjzf;
        /// <summary>
        /// 其它基金支付
        /// </summary>
        public string Qtjjzf
        {
            get { return qtjjzf; }
            set { qtjjzf = value; }
        }

        private string gsjjzf;
        /// <summary>
        /// 工伤基金支付
        /// </summary>
        public string Gsjjzf
        {
            get { return gsjjzf; }
            set { gsjjzf = value; }
        }
        private string jlfy;
        /// <summary>
        /// 甲类费用
        /// </summary>
        public string Jlfy
        {
            get { return jlfy; }
            set { jlfy = value; }
        }
        private string ylfy;
        /// <summary>
        /// 乙类费用
        /// </summary>
        public string Ylfy
        {
            get { return ylfy; }
            set { ylfy = value; }
        }
        private string blfy;
        /// <summary>
        /// 丙类费用
        /// </summary>
        public string Blfy
        {
            get { return blfy; }
            set { blfy = value; }
        }

        private string gwybcylzf;
        /// <summary>
        /// 公务员补充医疗自付
        /// </summary>
        public string Gwybcylzf
        {
            get { return gwybcylzf; }
            set { gwybcylzf = value; }
        }
        private string cgzgfd;
       /// <summary>
       /// 超过最高封顶线自付
       /// </summary>
        public string Cgzgfd
        {
            get { return cgzgfd; }
            set { cgzgfd = value; }
        }

        private string xxzf;

        /// <summary>
        /// 先行自付
        /// </summary>
        public string Xxzf
        {
            get { return xxzf; }
            set { xxzf = value; }
        }

        private string zzdwfd;
        /// <summary>
        /// 转诊单位负担
        /// </summary>
        public string Zzdwfd
        {
            get { return zzdwfd; }
            set { zzdwfd = value; }
        }

        private string tcddwfd;
        /// <summary>
        /// 统筹段单位负担
        /// </summary>
        public string Tcddwfd
        {
            get { return tcddwfd; }
            set { tcddwfd = value; }
        }

        private string dbddwfd;

        /// <summary>
        /// 大病段单位负担
        /// </summary>
        public string Dbddwfd
        {
            get { return dbddwfd; }
            set { dbddwfd = value; }
        }

        private string zhdkxjzfbz;
        /// <summary>
        /// 个人账户抵扣现金支付标志
        /// </summary>
        public string Zhdkxjzfbz
        {
            get { return zhdkxjzfbz; }
            set { zhdkxjzfbz = value; }
        }

        private string gwybcjbylzf;
        /// <summary>
        /// 公务员补助基本医疗支付（新余)
        /// </summary>
        public string Gwybcjbylzf
        {
            get { return gwybcjbylzf; }
            set { gwybcjbylzf = value; }
        }

        private string gwybzdbylzf;
        /// <summary>
        /// 公务员补助大病医疗支付
        /// </summary>
        public string Gwybzdbylzf
        {
            get { return gwybzdbylzf; }
            set { gwybzdbylzf = value; }
        }

        

        private string jbr;
        /// <summary>
        /// 经办人
        /// </summary>
        public string Jbr
        {
            get { return jbr; }
            set { jbr = value; }
        }
        private string tjzlfy;
        /// <summary>
        /// 特检自理费用
        /// </summary>
        public string Tjzlfy
        {
            get { return tjzlfy; }
            set { tjzlfy = value; }
        }
        private string tzzlfy;
        /// <summary>
        /// 特治自理费用
        /// </summary>
        public string Tzzlfy
        {
            get { return tzzlfy; }
            set { tzzlfy = value; }
        }

        private string ypfy;
        /// <summary>
        /// 药品费用
        /// </summary>
        public string Ypfy
        {
            get { return ypfy; }
            set { ypfy = value; }
        }

        private string zlxmfy;
        /// <summary>
        /// 诊疗项目费用
        /// </summary>
        public string Zlxmfy
        {
            get { return zlxmfy; }
            set { zlxmfy = value; }
        }

        private string sybxfy;
        /// <summary>
        /// 商业补充保险支付费用
        /// </summary>
        public string Sybxfy
        {
            get { return sybxfy; }
            set { sybxfy = value; }
        }

        private string mzzcf;
        /// <summary>
        /// 门诊诊查费
        /// </summary>
        public string Mzzcf
        {
            get { return mzzcf; }
            set { mzzcf = value; }
        }
        
        private string jzgbbzzf;
        /// <summary>
        ///军转干部补助支付 
        /// </summary>
        public string Jzgbbzzf
        {
            get { return jzgbbzzf; }
            set { jzgbbzzf = value; }
        }

        private string jbylzhzf;
        /// <summary>
        /// 基本医疗账户支付
        /// </summary>
        public string Jbylzhzf
        {
            get { return jbylzhzf; }
            set { jbylzhzf = value; }
        }

        private string gfylzhzf;
        /// <summary>
        /// 公费医疗账户支付
        /// </summary>
        public string Gfylzhzf
        {
            get { return gfylzhzf; }
            set { gfylzhzf = value; }
        }

        private string jtmztczf;
        /// <summary>
        /// 家庭门诊统筹支付
        /// </summary>
        public string Jtmztczf
        {
            get { return jtmztczf; }
            set { jtmztczf = value; }
        }

        private string dbjzfy;
        /// <summary>
        /// 大病救助费用
        /// </summary>
        public string Dbjzfy
        {
            get { return dbjzfy; }
            set { dbjzfy = value; }
        }

        private string fybzf;
        /// <summary>
        /// 非医保自付
        /// </summary>
        public string Fybzf
        {
            get { return fybzf; }
            set { fybzf = value; }
        }

        private string ybmzlsh;
        /// <summary>
        /// 医保门诊流水号
        /// </summary>
        public string Ybmzlsh
        {
            get { return ybmzlsh; }
            set { ybmzlsh = value; }
        }
        private string qtybzf;
        /// <summary>
        /// 其他医保支付
        /// </summary>
        public string Qtybzf
        {
            get { return qtybzf; }
            set { qtybzf = value; }
        }
        private string ybxjzf;
        /// <summary>
        /// 整合后现金支付
        /// </summary>
        public string Ybxjzf
        {
            get { return ybxjzf; }
            set { ybxjzf = value; }
        }

        private string zdjbfwnbxfy;
        /// <summary>
        /// 重大疾病范围内补偿金额
        /// </summary>
        public string Zdjbfwnbxfy
        {
            get { return zdjbfwnbxfy; }
            set { zdjbfwnbxfy = value; }
        }

        private string zdjbfwybxfy;
        /// <summary>
        /// 重大疾病范围内补偿金额
        /// </summary>
        public string Zdjbfwybxfy
        {
            get { return zdjbfwybxfy; }
            set { zdjbfwybxfy = value; }
        }

        private string mzjbjzfy;
        /// <summary>
        /// 民政大病救助基金
        /// </summary>
        public string Mzjbjzfy
        {
            get { return mzjbjzfy; }
            set { mzjbjzfy = value; }
        }

        private string zftdfy;
        /// <summary>
        /// 政府兜底基金
        /// </summary>
        public string Zftdfy
        {
            get { return zftdfy; }
            set { zftdfy = value; }
        }

        private string jmecbc;
        /// <summary>
        /// 居民二次补偿
        /// </summary>
        public string Jmecbc
        {
            get { return jmecbc; }
            set { jmecbc = value; }
        }
        private string jmdbydje;
        /// <summary>
        /// 居民大病一段金额
        /// </summary>
        public string Jmdbydje
        {
            get { return jmdbydje; }
            set { jmdbydje = value; }
        }

        private string jmdbedje;
        /// <summary>
        /// 居民大病二段金额
        /// </summary>
        public string Jmdbedje
        {
            get { return jmdbedje; }
            set { jmdbedje = value; }
        }

        private string jbbcfwnfyzfje;
        /// <summary>
        /// 疾病补充范围内费用支付金额
        /// </summary>
        public string Jbbcfwnfyzfje
        {
            get { return jbbcfwnfyzfje; }
            set { jbbcfwnfyzfje = value; }
        }

        private string jbbcybbczcfywfyzf;
        /// <summary>
        /// 疾病补充保险本次政策范围外费用支付金额
        /// </summary>
        public string Jbbcybbczcfywfyzf
        {
            get { return jbbcybbczcfywfyzf; }
            set { jbbcybbczcfywfyzf = value; }
        }
        private string mgwxlcjjzf;
        /// <summary>
        /// 美国微笑列车基金支付
        /// </summary>
        public string Mgwxlcjjzf
        {
            get { return mgwxlcjjzf; }
            set { mgwxlcjjzf = value; }
        }
        private string jjjmzcfwwkbxfy;
        /// <summary>
        /// 九江居民政策范围外可报销费用
        /// </summary>
        public string Jjjmzcfwwkbxfy
        {
            get { return jjjmzcfwwkbxfy; }
            set { jjjmzcfwwkbxfy = value; }
        }
        private string mfmzjj;
        /// <summary>
        /// 免费门诊基金（余江）
        /// </summary>
        public string Mfmzjj
        {
            get { return mfmzjj; }
            set { mfmzjj = value; }
        }

        private string jddbbcbcydzfje;
        /// <summary>
        /// 建档大病补偿本次一段支付金额
        /// </summary>
        public string Jddbbcbcydzfje
        {
            get { return jddbbcbcydzfje; }
            set { jddbbcbcydzfje = value; }
        }


        private string jddbbcbcedzfje;
        /// <summary>
        /// 建档大病补偿本次二段支付金额
        /// </summary>
        public string Jddbbcbcedzfje
        {
            get { return jddbbcbcedzfje; }
            set { jddbbcbcedzfje = value; }
        }

        private string jddbbcbcsdzfje;
        /// <summary>
        /// 建档大病补偿本次三段支付金额
        /// </summary>
        public string Jddbbcbcsdzfje
        {
            get { return jddbbcbcsdzfje; }
            set { jddbbcbcsdzfje = value; }
        }

        private string jdecbcbcydzfje;
        /// <summary>
        /// 建档二次补偿本次一段支付金额
        /// </summary>
        public string Jdecbcbcydzfje
        {
            get { return jdecbcbcydzfje; }
            set { jdecbcbcydzfje = value; }
        }

        private string jdecbcbcedzfje;
        /// <summary>
        /// 建档二次补偿本次二段支付金额
        /// </summary>
        public string Jdecbcbcedzfje
        {
            get { return jdecbcbcedzfje; }
            set { jdecbcbcedzfje = value; }
        }

        private string jdecbcbcsdzfje;
        /// <summary>
        /// 建档二次补偿本次三段支付金额
        /// </summary>
        public string Jdecbcbcsdzfje
        {
            get { return jdecbcbcsdzfje; }
            set { jdecbcbcsdzfje = value; }
        }
        private string jbbcbxbczcfwnfyydzfje;
        /// <summary>
        /// 疾病补充保险本次政策范围内费用一段支付金额
        /// </summary>
        public string Jbbcbxbczcfwnfyydzfje
        {
            get { return jbbcbxbczcfwnfyydzfje; }
            set { jbbcbxbczcfwnfyydzfje = value; }
        }

        private string jbbcbxbczcfwnfyedzfje;
        /// <summary>
        /// 疾病补充保险本次政策范围内费用二段支付金额
        /// </summary>
        public string Jbbcbxbczcfwnfyedzfje
        {
            get { return jbbcbxbczcfwnfyedzfje; }
            set { jbbcbxbczcfwnfyedzfje = value; }
        }
        private string bnzftdjjfylj;
        /// <summary>
        /// 本年政府兜底基金费用累计(不含本次)
        /// </summary>
        public string Bnzftdjjfylj
        {
            get { return bnzftdjjfylj; }
            set { bnzftdjjfylj = value; }
        }
        private string mmxe;
        /// <summary>
        /// 门慢限额
        /// </summary>
        public string Mmxe
        {
            get { return mmxe; }
            set { mmxe = value; }
        }
        private string scryylbzjj;
        /// <summary>
        /// 伤残人员医疗保障基金
        /// </summary>
        public string Scryylbzjj
        {
            get { return scryylbzjj; }
            set { scryylbzjj = value; }
        }
        private string zhzbxje;

        /// <summary>
        /// 整合后总报销金额
        /// </summary>
        public string Zhzbxje
        {
            get { return zhzbxje; }
            set { zhzbxje = value; }
        }

    }
}
