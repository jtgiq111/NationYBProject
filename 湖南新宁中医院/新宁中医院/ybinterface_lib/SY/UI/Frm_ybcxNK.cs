using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Srvtools;
using ybinterface_lib;

namespace ybinterface_lib.SY.UI
{
    public partial class Frm_ybcxNK : InfoForm
    {
        public Frm_ybcxNK()
        {
            InitializeComponent();
        }


        private void frm_ybcxSR_Load(object sender, EventArgs e)
        {
            tabPage1.Parent = null;

            #region 绑定检索条件_住院
            alvWHERE.SetColumnParam.AddQueryColumn("jzlsh");
            alvWHERE.SetColumnParam.AddQueryColumn("xm");
            alvWHERE.SetColumnParam.AddQueryColumn("djh");
            alvWHERE.SetColumnParam.SetValueColumn("grbh");
            alvWHERE.SetColumnParam.SetTextColumn("jzlsh");
            alvWHERE.SetColumnParam.AddViewColumn("jzlsh", "就诊流水号", 100);
            alvWHERE.SetColumnParam.AddViewColumn("djh", "结算单号", 100);
            alvWHERE.SetColumnParam.AddViewColumn("xm", "姓名", 80);
            alvWHERE.SetColumnParam.AddViewColumn("grbh", "个人编号", 100);
            alvWHERE.SetColumnParam.AddViewColumn("yldylb", "人员类别", 80);
            alvWHERE.SetColumnParam.AddViewColumn("ksmc", "科室名称", 100);
            alvWHERE.SetColumnParam.AddViewColumn("fysczt", "费用状态", 80);
            alvWHERE.SetColumnParam.AddViewColumn("jszt", "结算状态", 80);
            alvWHERE.SetColumnParam.AddViewColumn("BZMC", "结算时间", 120);

            string strSql = string.Format(@"select distinct a.jzlsh,a.xm, a.grbh, a.yldylb,a.ksmc,a.cwh,a.ysxm,
                                            case when b.cxbz=1 then '已上传'
                                            when b.cxbz=2 then '已撤销'
                                            when ISNULL(b.cxbz,'')='' then '未上传' end as fysczt,
                                            case when c.cxbz=1 then '已结算'
                                            when c.cxbz=2 then '已撤销'
                                            when ISNULL(c.cxbz,'')='' then '未结算' end as jszt,
                                            c.djh,c.sysdate from ybmzzydjdr a
                                            left join ybcfmxscindr b on a.jzlsh=b.jzlsh and b.cxbz=1
                                            left join ybfyjsdr c on a.jzlsh=c.jzlsh and c.cxbz =1
                                            where a.cxbz=1 and LEN(a.jzlsh)=8");
            DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            alvWHERE.SetDataSource(ds.Copy());

            #endregion

            #region 绑定检索条件_门诊
//            alvWHERE_MZ.SetColumnParam.AddQueryColumn("jzlsh");
//            alvWHERE_MZ.SetColumnParam.AddQueryColumn("xm");
//            alvWHERE_MZ.SetColumnParam.AddQueryColumn("djh");
//            alvWHERE_MZ.SetColumnParam.SetValueColumn("grbh");
//            alvWHERE_MZ.SetColumnParam.SetTextColumn("jzlsh");
//            alvWHERE_MZ.SetColumnParam.AddViewColumn("jzlsh", "就诊流水号", 100);
//            alvWHERE_MZ.SetColumnParam.AddViewColumn("djh", "结算单号", 100);
//            alvWHERE_MZ.SetColumnParam.AddViewColumn("xm", "姓名", 80);
//            alvWHERE_MZ.SetColumnParam.AddViewColumn("grbh", "个人编号", 100);
//            alvWHERE_MZ.SetColumnParam.AddViewColumn("yldylb", "人员类别", 80);
//            alvWHERE_MZ.SetColumnParam.AddViewColumn("ksmc", "科室名称", 100);
//            alvWHERE_MZ.SetColumnParam.AddViewColumn("fysczt", "费用状态", 80);
//            alvWHERE_MZ.SetColumnParam.AddViewColumn("jszt", "结算状态", 80);
//            alvWHERE_MZ.SetColumnParam.AddViewColumn("BZMC", "结算时间", 120);

//            strSql = string.Format(@"select distinct a.jzlsh,a.xm, a.grbh, a.yldylb,a.ksmc,a.cwh,a.ysxm,
//                                                case when b.cxbz=1 then '已上传'
//                                                when b.cxbz=2 then '已撤销'
//                                                when ISNULL(b.cxbz,'')='' then '未上传' end as fysczt,
//                                                case when c.cxbz=1 then '已结算'
//                                                when c.cxbz=2 then '已撤销'
//                                                when ISNULL(c.cxbz,'')='' then '未结算' end as jszt,
//                                                c.djh,c.sysdate from ybmzzydjdr a
//                                                left join ybcfmxscindr b on a.jzlsh=b.jzlsh and b.cxbz=1
//                                                left join ybfyjsdr c on a.jzlsh=b.jzlsh and b.jylsh=c.cfmxjylsh and c.cxbz =1
//                                                where a.cxbz=1 and LEN(a.jzlsh)=9 ");
//            DataSet ds_MZ = CliUtils.ExecuteSql("sybdjdr", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
//            alvWHERE_MZ.SetDataSource(ds_MZ.Copy());
            #endregion

        }

        #region 医保住院撤销
        private void alvWHERE_AfterConfirm(object sender, EventArgs e)
        {
            if (alvWHERE.currentDataRow != null)
            {
                txtJZLSH_ZY.Text = alvWHERE.currentDataRow["jzlsh"].ToString();
                txtHZXM_ZY.Text = alvWHERE.currentDataRow["xm"].ToString();
                txtGRBH_ZY.Text = alvWHERE.currentDataRow["grbh"].ToString();
                txtRYLB_ZY.Text = alvWHERE.currentDataRow["yldylb"].ToString();
                txtKSMC_ZY.Text = alvWHERE.currentDataRow["ksmc"].ToString();;
                txtDJH_ZY.Text = alvWHERE.currentDataRow["djh"].ToString();
                txtJSSJ_ZY.Text = alvWHERE.currentDataRow["sysdate"].ToString();
                txtDJSTATUS_ZY.Text = "已登记";
                txtCFSTATUS_ZY.Text = alvWHERE.currentDataRow["fysczt"].ToString();
                txtJSSTATUS_ZY.Text = alvWHERE.currentDataRow["jszt"].ToString();
                #region 获取医疗总费
                string strSql = string.Format(@"select  sum(z3amnt * (case left(z3endv, 1) when '4' then -1 else 1 end)) amt2
                                                from zy03d where  left(z3kind, 1) in ('2', '4')  and z3zyno='{0}'
										        having sum(z3amnt * (case left(z3endv, 1) when '4' then -1 else 1 end)) > 0", txtJZLSH_ZY.Text);
                DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                txtYLFY.Text = ds.Tables[0].Rows[0]["amt2"].ToString();
                #endregion
                #region 获取医保上传总费用
                strSql = string.Format(@"select SUM(je) as ybfy from ybcfmxscindr where jzlsh = '{0}' and cxbz=1", txtJZLSH_ZY.Text);
                ds.Tables.Clear();
                ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
                txtSCFY.Text = ds.Tables[0].Rows[0]["ybfy"].ToString();
                #endregion
            }
        }


        private void btnJSD_ZY_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(this.txtJZLSH_ZY.Text))
            {
                MessageBox.Show("无住院号", "提示");
                txtJZLSH_ZY.Focus();
                return;
            }
            if (string.IsNullOrWhiteSpace(this.txtDJH_ZY.Text))
            {
                MessageBox.Show("无发票号", "提示");
                return;
            }
            object[] obj = { txtJZLSH_ZY.Text.Trim(), txtDJH_ZY.Text.Trim() };
            obj = Functions.ybs_interface("4403", obj);
            MessageBox.Show(obj[2].ToString());
        }

        private void btnDJCX_ZY_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(this.txtJZLSH_ZY.Text))
            {
                MessageBox.Show("无住院号", "提示");
                txtJZLSH_ZY.Focus();
                return;
            }

            DialogResult dr = MessageBox.Show("是否撤销医保登记？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (dr == DialogResult.No)
                return;

            object[] obj = { txtJZLSH_ZY.Text.Trim() };
            obj = Functions.ybs_interface("4102", obj);

            if (obj[1].ToString() == "1")
            {
                txtDJSTATUS_ZY.Text = "已撤销";
                MessageBox.Show("撤销入院登记成功", "提示");
            }
            else
            {
                MessageBox.Show(obj[2].ToString(), "提示");
            }
        }

        private void btnCFSCCX_ZY_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(this.txtJZLSH_ZY.Text))
            {
                MessageBox.Show("无住院号", "提示");
                txtJZLSH_ZY.Focus();
                return;
            }

            DialogResult dr = MessageBox.Show("是否撤销已上传费用明细？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (dr == DialogResult.No)
                return;

            object[] obj = { txtJZLSH_ZY.Text.Trim() };
            obj = Functions.ybs_interface("4301", obj);

            if (obj[1].ToString() == "1")
            {
                txtCFSTATUS_ZY.Text = "已撤销";
                MessageBox.Show("撤销处方明细成功", "提示");
            }
            else
            {
                MessageBox.Show(obj[2].ToString(), "提示");
            }
        }

        private void btnJSCX_ZY_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(this.txtJZLSH_ZY.Text))
            {
                MessageBox.Show("无住院号", "提示");
                txtJZLSH_ZY.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(this.txtDJH_ZY.Text))
            {
                MessageBox.Show("无发票号", "提示");
                txtDJH_ZY.Focus();
                return;
            }

            #region 是否已办理一体结算
            //double tcj = 0.00;
            //string strSql = string.Format(@"select z3amtj from zy03dw where left(z3endv,1)=1 and z3zyno='{0}' ", txtJZLSH_ZY.Text);
            //DataSet ds = CliUtils.ExecuteSql("sybdjdr", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            //if (ds.Tables[0].Rows.Count > 0)
            //    tcj = Convert.ToDouble(ds.Tables[0].Rows[0]["z3amtj"].ToString());

            //if (tcj > 0)
            //{
            //    MessageBox.Show("该患者已办理HIS结算，不能单独撤销医保，需进行出院召回作业！！");
            //    return;
            //}
            #endregion

            DialogResult dr = MessageBox.Show("是否撤销医保结算数据？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (dr == DialogResult.No)
                return;
               string strSQL = string.Format(@" select ybjzlsh,grbh,xm,kh,z1tcdq,jzlsh,z1idno,z1bahx,z1mobi,z1bzno,z1bznm from ybfyjsdr 
            left join zy01h on z1zyno=jzlsh where cxbz=1 and jzlsh='{0}' and bascbz='1'", txtJZLSH_ZY.Text.Trim());
            DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSQL, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            if (ds.Tables[0].Rows.Count > 0)
            {
                //医保电子病历登记撤销
                object[] back_mess, param = new object[] { txtJZLSH_ZY.Text.Trim(), "2" };
                back_mess = Functions.ybs_interface("4600", param);
                if (back_mess[1].ToString() != "1")
                {
                    MessageBox.Show("医保电子病历登记撤销失败,请先手动撤销，再进行费用明细撤销！");
                    return;
                }
            }

            object[] obj = { txtJZLSH_ZY.Text.Trim(), txtDJH_ZY.Text.Trim(), "" };
            obj = Functions.ybs_interface("4402", obj);

            if (obj[1].ToString() == "1")
            {
                txtJSSTATUS_ZY.Text = "已撤销";
                MessageBox.Show("撤销结算成功", "提示");
            }
            else
            {
                MessageBox.Show(obj[2].ToString(), "提示");
            }
        }

        private void bthCLOSE_ZY_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        #endregion

        #region 医保门诊撤销
        private void alvWHERE_MZ_AfterConfirm(object sender, EventArgs e)
        {

            if (alvWHERE_MZ.currentDataRow != null)
            {
                txtJZLSH_MZ.Text = alvWHERE_MZ.currentDataRow["jzlsh"].ToString();
                txtHZXM_MZ.Text = alvWHERE_MZ.currentDataRow["xm"].ToString();
                txtGRBH_MZ.Text = alvWHERE_MZ.currentDataRow["grbh"].ToString();
                txtJSH_MZ.Text = alvWHERE_MZ.currentDataRow["djh"].ToString();
                txtJSSJ_MZ.Text = alvWHERE_MZ.currentDataRow["sysdate"].ToString();
                txtDJSTATUS_ZY.Text = "已登记";
                txtCFSC_MZ.Text = alvWHERE_MZ.currentDataRow["fysczt"].ToString();
                txtJS_MZ.Text = alvWHERE_MZ.currentDataRow["jszt"].ToString();
            }
        }

        private void btnJSD_MZ_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(this.txtJZLSH_MZ.Text))
            {
                MessageBox.Show("无住院号", "提示");
                txtJZLSH_MZ.Focus();
                return;
            }
            if (string.IsNullOrWhiteSpace(this.txtJSH_MZ.Text))
            {
                MessageBox.Show("无结算号", "提示");
                return;
            }
            object[] obj = { txtJZLSH_MZ.Text.Trim(), txtJSH_MZ.Text.Trim() };
            obj = Functions.ybs_interface("3003", obj);
            MessageBox.Show(obj[2].ToString());
        }

        private void btnDJCX_MZ_Click(object sender, EventArgs e)
        {
            
            if (string.IsNullOrWhiteSpace(this.txtJZLSH_MZ.Text))
            {
                MessageBox.Show("无住院号", "提示");
                txtJZLSH_MZ.Focus();
                return;
            }
            if (string.IsNullOrWhiteSpace(this.txtJSH_MZ.Text))
            {
                MessageBox.Show("无结算号", "提示");
                return;
            }
            object[] obj = { txtJZLSH_MZ.Text.Trim(), txtJSH_MZ.Text.Trim() };
            obj = Functions.ybs_interface("3101", obj);
            MessageBox.Show(obj[2].ToString());
        }

        private void btnJSCX_MZ_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(this.txtJZLSH_MZ.Text))
            {
                MessageBox.Show("无住院号", "提示");
                txtJZLSH_MZ.Focus();
                return;
            }
            if (string.IsNullOrWhiteSpace(this.txtJSH_MZ.Text))
            {
                MessageBox.Show("无结算号", "提示");
                return;
            }
            object[] obj = { txtJZLSH_MZ.Text.Trim(), txtJSH_MZ.Text.Trim() };
            obj = Functions.ybs_interface("3302", obj);
            MessageBox.Show(obj[2].ToString());

        }

        private void btnCLOSE_MZ_Click(object sender, EventArgs e)
        {
            this.Close();
        }


        private void btnCFSC_MZ_Click(object sender, EventArgs e)
        {

        }


        #endregion

        //结算审核
        private void button2_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(this.txtJZLSH_ZY.Text))
            {
                MessageBox.Show("无住院号", "提示");
                txtJZLSH_ZY.Focus();
                return;
            }
            DialogResult dr = MessageBox.Show("请确认患者[" + txtJZLSH_ZY.Text + "]所有费用已全部上传及预结算成功！\r\n是否通过审核？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (dr == DialogResult.No)
                return;

            string strSql = string.Format(@"update ybmzzydjdr set shbz=1 where jzlsh='{0}' and cxbz=1", txtWHERE_ZY.Text);
            object[] obj = { strSql };
            obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);
            if (obj[1].ToString().Equals("1"))
            {
                txtSHSTATUS.Text = "已审核";
                MessageBox.Show("审核成功!");
            }
            else
                MessageBox.Show("审核失败!");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(this.txtJZLSH_ZY.Text))
            {
                MessageBox.Show("无住院号", "提示");
                txtJZLSH_ZY.Focus();
                return;
            }

            DialogResult dr = MessageBox.Show("是否撤销结算审核？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (dr == DialogResult.No)
                return;
            string strSql = string.Format(@"update ybmzzydjdr set shbz=0 where jzlsh='{0}' and cxbz=1", txtWHERE_ZY.Text);
            object[] obj = { strSql };
            obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);
            if (obj[1].ToString().Equals("1"))
            {
                txtSHSTATUS.Text = "未审核";
                MessageBox.Show("审核撤销成功!");
            }
            else
                MessageBox.Show("审核撤销失败!");
        }

    }
}
