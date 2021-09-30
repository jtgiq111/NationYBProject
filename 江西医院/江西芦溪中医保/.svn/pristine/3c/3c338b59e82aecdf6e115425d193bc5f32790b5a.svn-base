using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Srvtools;
using System.IO;
using FastReport;
using yb_interfaces;

namespace yb_interfaces
{
    public partial class Frm_ybjsmxSQXN : Form
    {

        private string jzlsh = string.Empty;
        public Frm_ybjsmxSQXN()
        {
            InitializeComponent();
        }


        public Frm_ybjsmxSQXN(string param)
        {
            InitializeComponent();
            jzlsh = param;
        }

        private void Frm_ybjsmxJA_Load(object sender, EventArgs e)
        {
            string strSql = string.Format(@"select b.xm,a.grbh,b.kh,b.tcqh,c.bzname as qxmc,d.bzname as yllb,e.bzname as yldylb, a.ylfze,a.zbxje,a.tcjjzf,a.dejjzf,
                                            a.zhzf,a.xjzf,a.gwybzjjzf,a.qybcylbxjjzf,a.zffy,a.dwfdfy,a.yyfdfy,a.mzjzfy,a.cxjfy,
                                            a.ylzlfy,a.blzlfy,a.fhjbylfy,a.qfbzfy,a.zzzyzffy,a.jrtcfy,a.jrdebxfy,a.bcjsqzhye,
                                            a.bntczflj,a.bndezflj,a.bnzhzflj,a.bnzycslj,a.ecbcje,a.zftdjjzf,a.dbzbc,a.syjjzf,a.jbr,left(convert(nvarchar,a.sysdate,120),10) jsrq,z1ldat,a.djhin,a.jzlsh
                                            from ybfyyjsdr a
                                            inner join ybmzzydjdr b on a.ybjzlsh=b.ybjzlsh and a.cxbz=b.cxbz 
                                            left join zy01h h on h.z1zyno=a.jzlsh
                                            left join bztbd c on c.bzcodn='dq' and c.bzkeyx=b.tcqh 
                                            left join bztbd d on  d.bzkeyx=a.yllb and d.bzcodn='yl'
                                            left join bztbd e on  e.bzkeyx=a.yldylb and e.bzcodn='dl'
                                            where a.jzlsh='{0}' and a.cxbz=1", jzlsh);
            DataSet ds = CliUtils.ExecuteSql("shs01", "cmd", strSql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            if (ds.Tables[0].Rows.Count > 0)
            {
                DataRow dr=ds.Tables[0].Rows[0];
                txtparam1.Text = dr["xm"].ToString();
                txtparam2.Text = dr["grbh"].ToString();
                txtparam3.Text = dr["kh"].ToString();
                txtparam4.Text = dr["tcqh"].ToString();
                txtparam5.Text = dr["qxmc"].ToString();
                txtparam6.Text = dr["yllb"].ToString();
                txtparam7.Text = dr["yldylb"].ToString();
                txtparam8.Text = dr["ylfze"].ToString();
                txtparam9.Text = dr["zbxje"].ToString();
                txtparam10.Text = dr["tcjjzf"].ToString();
                txtparam11.Text = dr["dejjzf"].ToString();
                txtparam12.Text = dr["zhzf"].ToString();
                txtparam13.Text = dr["xjzf"].ToString();
                txtparam14.Text = dr["gwybzjjzf"].ToString();
                txtparam15.Text = dr["qybcylbxjjzf"].ToString();
                txtparam16.Text = dr["zffy"].ToString();
                txtparam17.Text = dr["dwfdfy"].ToString();
                txtparam18.Text = dr["yyfdfy"].ToString();
                txtparam19.Text = dr["mzjzfy"].ToString();
                txtparam20.Text = dr["cxjfy"].ToString();
                txtparam21.Text = dr["ylzlfy"].ToString();
                txtparam23.Text = dr["blzlfy"].ToString();
                txtparam25.Text = dr["fhjbylfy"].ToString();
                txtparam26.Text = dr["qfbzfy"].ToString();
                txtparam27.Text = dr["zzzyzffy"].ToString();
                txtparam28.Text = dr["jrtcfy"].ToString();
                txtparam29.Text = dr["jrdebxfy"].ToString();
                txtparam30.Text = dr["bcjsqzhye"].ToString();
                txtparam31.Text = dr["bntczflj"].ToString();
                txtparam32.Text = dr["bndezflj"].ToString();
                txtparam33.Text = dr["bnzhzflj"].ToString();
                txtparam34.Text = dr["bnzycslj"].ToString();
                txtparam35.Text = dr["ecbcje"].ToString();
                txtparam36.Text = dr["zftdjjzf"].ToString();
                txtparam37.Text = dr["dbzbc"].ToString();
                txtparam38.Text = dr["jbr"].ToString();
                txtparam39.Text = dr["jsrq"].ToString();
                txtDjh.Text = dr["djhin"].ToString();
                txtCysj.Text = dr["z1ldat"].ToString();
                txtZyno.Text = dr["jzlsh"].ToString();
            }
            else
            {
                MessageBox.Show("未查询到该患者结算数据！");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

     
    }
}
