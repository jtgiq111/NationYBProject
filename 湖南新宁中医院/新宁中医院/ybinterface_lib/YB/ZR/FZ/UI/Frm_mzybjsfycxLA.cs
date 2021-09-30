using Srvtools;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ybinterface_lib
{
    public partial class Frm_mzybjsfycxLA : InfoForm
    {
        public Frm_mzybjsfycxLA()
        {
            InitializeComponent();
        }

        private void Frm_mzybjsfycxXG_Load(object sender, EventArgs e)
        {
            #region 绑定检索条件_门诊
            alvWHERE_MZ.SetColumnParam.AddQueryColumn("jzlsh");
            alvWHERE_MZ.SetColumnParam.AddQueryColumn("xm");
            alvWHERE_MZ.SetColumnParam.AddQueryColumn("djh");
            alvWHERE_MZ.SetColumnParam.SetValueColumn("grbh");
            alvWHERE_MZ.SetColumnParam.SetTextColumn("jzlsh");
            alvWHERE_MZ.SetColumnParam.AddViewColumn("jzlsh", "就诊流水号", 100);
            alvWHERE_MZ.SetColumnParam.AddViewColumn("djh", "结算单号", 100);
            alvWHERE_MZ.SetColumnParam.AddViewColumn("xm", "姓名", 80);
            alvWHERE_MZ.SetColumnParam.AddViewColumn("grbh", "个人编号", 100);
            alvWHERE_MZ.SetColumnParam.AddViewColumn("yldylb", "人员类别", 80);
            alvWHERE_MZ.SetColumnParam.AddViewColumn("ksmc", "科室名称", 100);
            alvWHERE_MZ.SetColumnParam.AddViewColumn("fysczt", "费用状态", 80);
            alvWHERE_MZ.SetColumnParam.AddViewColumn("jszt", "结算状态", 80);
            alvWHERE_MZ.SetColumnParam.AddViewColumn("BZMC", "结算时间", 120);

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
                                                where a.cxbz=1 and LEN(a.jzlsh)=9 ");
            DataSet ds_MZ = CliUtils.ExecuteSql("sybdj", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            alvWHERE_MZ.SetDataSource(ds_MZ.Copy());
            #endregion
        }

        private void btnDJCX_MZ_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(this.txtJZLSH_MZ.Text))
            {
                MessageBox.Show("无住院号", "提示");
                txtJZLSH_MZ.Focus();
                return;
            }
            object[] obj = { txtJZLSH_MZ.Text.Trim() };
            obj = yb_interface.ybs_interface("3101", obj);
            MessageBox.Show(obj[2].ToString());
        }

        private void alvWHERE_MZ_AfterConfirm(object sender, EventArgs e)
        {

            if (alvWHERE_MZ.currentDataRow != null)
            {
                txtJZLSH_MZ.Text = alvWHERE_MZ.currentDataRow["jzlsh"].ToString();
                txtHZXM_MZ.Text = alvWHERE_MZ.currentDataRow["xm"].ToString();
                txtGRBH_MZ.Text = alvWHERE_MZ.currentDataRow["grbh"].ToString();
                txtJSH_MZ.Text = alvWHERE_MZ.currentDataRow["djh"].ToString();
                txtJSSJ_MZ.Text = alvWHERE_MZ.currentDataRow["sysdate"].ToString();
                txtDJSTATUS_MZ.Text = "已登记";
                txtCFSC_MZ.Text = alvWHERE_MZ.currentDataRow["fysczt"].ToString();
                txtJS_MZ.Text = alvWHERE_MZ.currentDataRow["jszt"].ToString();
            }
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
            obj = yb_interface.ybs_interface("3302", obj);
            MessageBox.Show(obj[2].ToString());
        }
    }
}
