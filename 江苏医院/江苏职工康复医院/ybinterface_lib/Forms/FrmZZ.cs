using Srvtools;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ybinterface_lib.SY.YB;

namespace ybinterface_lib
{
    public partial class FrmZZ : InfoForm
    {
        public FrmZZ()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(quhua.Text.Trim()))
            {
                MessageBox.Show("转外区划不为空！");
                return;
            }
            if (string.IsNullOrEmpty(hosname.Text.Trim()))
            {
                MessageBox.Show("转外医院不为空！");
                return;
            }
            if (string.IsNullOrEmpty(sqly.Text.Trim()))
            {
                MessageBox.Show("申请理由不为空！");
                return;
            }
            if (string.IsNullOrEmpty(ssqx.Text.Trim()))
            {
                MessageBox.Show("所属区县不为空！");
                return;
            }
            YBZZ ybinfo = new YBZZ()
            {
                 actualEndTime=jssj.Value.ToString("yyyyMMdd"),
                actualStartTime = kssj.Value.ToString("yyyyMMdd"),
                outArea= quhua.Text,
                applyReason=sqly.Text,
                outHospitalName=hosname.Text,
                transportation=jtgj.Text,
                stateCode=ssqx.Text,
                applyTime=DateTime.Now.ToString("yyyyMMdd"),

            };
            (int code, string res) = yb_interface_sy_zrNew.YBZZ(ybinfo);
            if (code==-1)
            {
                MessageBox.Show(res);
                return;
            }
            else
            {
                MessageBox.Show("转诊成功！");
            }
        }
    }
}
