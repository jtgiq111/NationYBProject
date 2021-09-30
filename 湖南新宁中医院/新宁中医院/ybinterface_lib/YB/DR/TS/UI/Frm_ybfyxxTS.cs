using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Srvtools;

namespace ybinterface_lib
{
    public partial class Frm_ybfyxxTS : Form
    {
        string jzlsh = string.Empty;
        public Frm_ybfyxxTS()
        {
            InitializeComponent();
        }

        public Frm_ybfyxxTS(string objParam)
        {
            InitializeComponent();
            jzlsh = objParam;
        }

        private void Frm_ybfyxxTS_Load(object sender, EventArgs e)
        {
              string strSql = string.Format(@"select grbh,xm,ylfze,zbxje,zhzf,xjzf,tcjjzf,dejjzf,mzjzfy,dwfdfy,
                                            lxgbddtczf,yyfdfy,zdjbfwnbcje,zdjbfwybcje,mzdbjzje,ecbcje,czzcje,zffy,qfbzfy,jrtczf,
                                            ylfy,ylypzf from ybfyyjsdr where jzlsh='{0}' and cxbz=1 order by sysdate desc ", jzlsh);
            DataSet ds = CliUtils.ExecuteSql("szy02", "cmd", strSql, CliUtils.fLoginDB.ToString(), true, CliUtils.fCurrentProject.ToString());
            if (ds.Tables[0].Rows.Count > 0)
            {

            }
        }
    }
}
