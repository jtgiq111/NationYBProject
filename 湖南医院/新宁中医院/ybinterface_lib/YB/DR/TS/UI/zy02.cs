using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Srvtools;
namespace czy02
{
    public class zy02
    {
          
        public bool getdbj(string zyno)
        {
            #region 当前患者的担保信息
            string strSql = string.Format(@"select COUNT(*)sl from zy03d where substring(z3kind,1,1)='3' and z3jshx is null and z3comp='{0}' 
                                        and z3zyno='{1}' and SUBSTRING(z3endv,1,1)='3' and 
                                        not exists (select 1 from zy03d d where substring(d.z3kind,1,1) ='3' and zy03d.z3comp=d.z3comp and 
                                        zy03d.z3zyno=d.z3zyno and zy03d.z3ghno=d.z3ghno and zy03d.z3sequ=d.z3tseq and z3jshx is null)",
                                    CliUtils.fSiteCode, zyno);
            DataSet strDB = CliUtils.ExecuteSql("szy02", "cmd", strSql, CliUtils.fLoginDB.ToString(), true, CliUtils.fCurrentProject.ToString());
            if (Convert.ToInt32(strDB.Tables[0].Rows[0]["sl"].ToString()) > 0)
            {
                return false;
            }
            return true;
            #endregion
        }
        public DataSet gethzInfo(string zyno)
        {
            #region  获取患者基本信息
            string strSql = string.Format(@"select 1 from zy01d where z1comp='{0}' and z1zyno='{1}'",CliUtils.fSiteCode,zyno);
            DataSet strDB = CliUtils.ExecuteSql("szy02", "cmd", strSql, CliUtils.fLoginDB.ToString(), true, CliUtils.fCurrentProject.ToString());
            if (strDB.Tables[0].Rows.Count == 0)
            {
                MessageBox.Show("该患者没有入科信息","系统提示");
                return strDB;
            }
            strSql = string.Format(@"
                   select h.z1ghno,h.z1zyno,h.z1hznm,h.z1sexx,h.z1agex,h.z1flag,d.z1bedn,
                   h.z1date,h.z1ldat,d.z1ksnm,h.z1lyjg,h.z1lynm,h.z1ybno,h.z1cyyy,bq.bzname z1bqnm,
                   h.z1ylno,dl.bzname rydl,dq.bzname tcdq,h.z1area,h.z1jshx,h.z1jsrq from zy01h h
                   left join zy01d d on h.z1comp=d.z1comp and h.z1ghno=d.z1ghno
                   and h.z1zyno=d.z1zyno
                   left join bz02d b2d on b2d.b2comp=d.z1comp and d.z1ksno=b2d.b2ejks
                   left join bz02h b2h on b2h.b2comp=b2d.b2comp and  b2h.b2ksno=b2d.b2ksno
                   left join bztbd bq  on bq.bzcomp=b2h.b2comp and bq.bzkeyx=b2h.b2bqxx and bq.bzcodn='BQ'
                   left join bztbd dl  on dl.bzcomp=h.z1comp and dl.bzmem1=h.z1rylb and dl.bzcodn='DL'
                   left join bztbd dq  on dq.bzcomp=h.z1comp and dq.bzmem1=h.z1tcdq and dq.bzcodn='DQ'
                   where h.z1comp ='{0}' and h.z1zyno='{1}' and left(d.z1endv,1) in ('2','8')",
                                CliUtils.fSiteCode, zyno);
           strDB = CliUtils.ExecuteSql("szy02", "cmd", strSql, CliUtils.fLoginDB.ToString(), true, CliUtils.fCurrentProject.ToString());
           return strDB;
           #endregion
        }
        #region 办理出院条件查询
        public bool leaveHospitalCondition(string ghno,string zyno)
        {
            DataSet dschk = new DataSet();
            string strchkSql = string.Empty, tishi_info=string.Empty;
            #region 1、检查门诊药房是否发药
            strchkSql = string.Format(@"select a.mckind,a.mccfno,a.mcdate,b.b1name 
                                        from mzcfd a left outer join bz01h b on(a.mcuser=b.b1empn)
                                        where a.mccomp='{0}' and a.mcghno='{1}' and upper(a.mcmzzy)='Z' and substring(a.mcendv,1,1)='3' 
                                        order by a.mckind,a.mccfno",
                                        CliUtils.fSiteCode.Trim(), ghno);
            dschk = CliUtils.ExecuteSql("scomm", "cmd", strchkSql, CliUtils.fLoginDB.ToString(), true, CliUtils.fCurrentProject.ToString());
            if (dschk.Tables[0].Rows.Count > 0)
            {
                tishi_info = "该患者门诊药房还有未发药品：\r\n"; //提示信息
                for (int j = 0; j < dschk.Tables[0].Rows.Count; j++)
                {
                    tishi_info = tishi_info + dschk.Tables[0].Rows[j]["mckind"].ToString() + "处方号：" + dschk.Tables[0].Rows[j]["mccfno"].ToString() +
                         " 开单医生：" + dschk.Tables[0].Rows[j]["b1name"].ToString() + " 日期：" + dschk.Tables[0].Rows[j]["mcdate"].ToString() + "\r\n";
                }
                tishi_info = tishi_info + "请联系药房或者联系医生删除不要的处方单后再办理出院!";
                MessageBox.Show(tishi_info);
                return false;
            }
            #endregion

            #region 2、长期医嘱是否停止
            strchkSql = string.Format(@"  select 1 from zy04h
                                    where z4type='长' and SUBSTRING(z4stau,1,1) in ('1','2','3') and  (substring(z4stau,1,1)='1' or  substring(z4stau,1,1)='2') 
                                    and isnull(z4tzbh,'')='' and isnull(z4endt,'')=''  and z4comp='{0}' and  z4zyno='{1}' and z4ghno='{2}' ",
                      CliUtils.fSiteCode.Trim(), zyno, ghno);
            dschk = CliUtils.ExecuteSql("shs01", "cmd", strchkSql, CliUtils.fLoginDB.ToString(), true, CliUtils.fCurrentProject.ToString());
            if (dschk.Tables[0].Rows.Count > 0)
            {
                MessageBox.Show("该患者还有未停止的长期医嘱，请联系住院医生与护士处理再办理出院", "系统提示");
                return false;
            }
            #endregion

            #region 3、患者有已申领未发的药品或者有已申请退药但未退的药品
            tishi_info = "";
            strchkSql = string.Format(@"select distinct a.z5bqnm,a.z5stoc,a.z5cfdh,b.ypname,a.z5slsj
                                    from zy05h a left join yptbd b on(a.z5stoc=b.ypkeyx and b.ypcodn='ST')
                                    where a.z5comp='{0}' and a.z5zyno='{1}' and a.z5ghno='{2}' and substring(a.z5endv,1,1) in ('1','3') ",
                                    CliUtils.fSiteCode.Trim(), zyno, ghno);
            dschk = CliUtils.ExecuteSql("shs01", "cmd", strchkSql, CliUtils.fLoginDB.ToString(), true, CliUtils.fCurrentProject.ToString());
            if (dschk.Tables[0].Rows.Count > 0)
            {

                tishi_info = "该患者有已申领未发的药品或者有已申请退药但未退的药品： \r\n"; //提示信息
                for (int j = 0; j < dschk.Tables[0].Rows.Count; j++)
                {
                    tishi_info = tishi_info
                         + "药房：" + dschk.Tables[0].Rows[j]["ypname"].ToString()
                         + " 处方号：" + dschk.Tables[0].Rows[j]["z5cfdh"].ToString()
                         + " 申请时间：" + dschk.Tables[0].Rows[j]["z5slsj"].ToString() + " \r\n";
                }
                tishi_info = tishi_info + "请联系上述药房,再办理出院!";
                MessageBox.Show(tishi_info, "系统提示");
                return false;
            }
            #endregion

            dschk.Clear();
            dschk.Dispose();
            return true;
        }
        #endregion

        public bool Checkjsfy(string  ghno,string zyno,string hzfy,string fph,bool ismjs,string mjdate)
        {
            DataSet ds = new DataSet();
            string str = string.Empty;
            string getvalue = string.Empty, strcondition=string.Empty;
            if (ismjs)
            {
                strcondition = string.Format(" and left(z3date,10)<='{0}'", mjdate);
            }
            else
            {
                strcondition = string.Empty;
            }
            str = string.Format(@"with temp as
                                (
                                 select SUM(z3amnt*(case SUBSTRING(z3endv,1,1) when '4' then -1 else 1 end))as z3amnt
                                 from zy03d 
                                 left join bz05h on z3sfno=b5sfno
                                 left join bztbd on bzcodn='A4' and bzkeyx=b5zyno
                                 where substring(z3kind,1,1) in ('2','4') and z3comp='{0}' and z3ghno='{1}'and z3zyno='{2}' and z3fphx='{4}' {3}
                                 group by  bzname having SUM(z3amnt*(case SUBSTRING(z3endv,1,1) when '4' then -1 else 1 end))>0
                                
                                 union all
                                 select SUM(z3amnt*(case SUBSTRING(z3endv,1,1) when '4' then -1 else 1 end))as z3amnt
                                 from zy03dz 
                                 where substring(z3kind,1,1) in ('2','4') and z3comp='{0}' and z3ghno='{1}'and z3zyno='{2}' and z3fphx='{4}' {3}
                                 having SUM(z3amnt*(case SUBSTRING(z3endv,1,1) when '4' then -1 else 1 end))>0
                                 )
                                 select SUM(z3amnt)z3amnt from temp",
                                 CliUtils.fSiteCode, ghno, zyno,strcondition,fph);
            ds = CliUtils.ExecuteSql("szy02", "cmd", str, CliUtils.fLoginDB.ToString(), true, CliUtils.fCurrentProject.ToString());
            if (ds.Tables[0].Rows.Count > 0)
            {
                getvalue = ds.Tables[0].Rows[0]["z3amnt"].ToString();
                if (string.IsNullOrWhiteSpace(getvalue)) getvalue = "0";
                if (Math.Abs(double.Parse(hzfy) - double.Parse(getvalue)) > 0.1)
                {
                    MessageBox.Show(string.Format("当前患者费用有变化请重新输入住院号进行结算"), "系统提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }
            else
            {
                MessageBox.Show(string.Format("当前患者费用有变化请重新输入住院号进行结算"), "系统提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            return true;
        }
        public bool Checkyjjfy(string ghno, string zyno, string yjj)
        {
            string str = string.Empty;
            string getvalue = string.Empty;
            DataSet ds = new DataSet();
            str = string.Format(@"select sum(case when left(z3endv,1)='4' then -isnull(z3amnt,0) else isnull(z3amnt,0) end) as z3amnt
                                from zy03d
                                where left(z3kind,1)='1' and isnull(z3jshx,'')='' and z3comp='{0}'  and z3ghno='{1}' and z3zyno='{2}'",
                           CliUtils.fSiteCode, ghno, zyno);
            ds = CliUtils.ExecuteSql("szy02", "cmd", str, CliUtils.fLoginDB.ToString(), true, CliUtils.fCurrentProject.ToString());
            if (ds.Tables[0].Rows.Count > 0)
            {
                getvalue = ds.Tables[0].Rows[0]["z3amnt"].ToString();
                if (string.IsNullOrWhiteSpace(getvalue)) getvalue = "0";
                if (Math.Abs(double.Parse(yjj) - double.Parse(getvalue)) > 0.1)
                {
                    MessageBox.Show(string.Format("当前患者预交金有变化请重新输入住院号进行结算"), "系统提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }
            return true;
        }
    }
}
