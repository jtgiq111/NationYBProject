using Srvtools;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml.Linq;

namespace ybinterface_lib
{
    public partial class Form1 : InfoForm
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btn_dgselect_Click(object sender, EventArgs e)
        {
            string param = txtDGYS.Text.Trim();
            StringBuilder strSQL = new StringBuilder();
            strSQL.Remove(0, strSQL.Length);
            strSQL.Append("select a.b1empn,a.b1name,b.dgysbm from bz01h a left join ybdgyszd b on a.b1empn=b.ysbm where 1=1 ");
            if (!string.IsNullOrEmpty(param))
                strSQL.Append("and  a.b1name like '%" + param + "%'");
            DataSet ds = CliUtils.ExecuteSql("sybdjdr", "cmd", strSQL.ToString(), CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            dgv_dgys.DataSource = ds.Tables[0];
        }

        private void btn_dginsert_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtDGYSBM.Text.Trim()))
            {
                MessageBox.Show("国家码不能为空");
                return;
            }
            StringBuilder strSQL = new StringBuilder();
            strSQL.Remove(0, strSQL.Length);
            strSQL.Append("select dgysbm from ybdgyszd where dgysbm='" + txtDGYSBM.Text.Trim() + "'");
            DataSet ds1 = CliUtils.ExecuteSql("sybdj", "cmd", strSQL.ToString(), CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            if (ds1.Tables[0].Rows.Count > 0)
            {
                MessageBox.Show("国家码重复，请核对！");
                return;
            }
            strSQL.Remove(0, strSQL.Length);
            strSQL.Append("insert into ybdgyszd(ysbm,ysxm,dgysbm) values( '" + txtYSBM.Text + "','" + txtYSXM.Text + "','" + txtDGYSBM.Text.Trim() + "') ");
            List<string> liSQL = new List<string>();
            liSQL.Add(strSQL.ToString());
            object[] obj = liSQL.ToArray();
            obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);
            if (obj[1].ToString().Equals("1"))
            {
                MessageBox.Show("配对成功！");
                txtDGYSBM.Enabled = false;
                btn_dgselect_Click(sender, e);
            }
            else
                MessageBox.Show("配对失败！");
        }

        private void btn_dgdelete_Click(object sender, EventArgs e)
        {
            StringBuilder strSQL = new StringBuilder();
            strSQL.Remove(0, strSQL.Length);
            strSQL.Append("delete from  ybdgyszd where ysbm='" + txtYSBM.Text + "'  ");
            List<string> liSQL = new List<string>();
            liSQL.Add(strSQL.ToString()); 
            object[] obj = liSQL.ToArray();
            obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);
            if (obj[1].ToString().Equals("1"))
            {
                MessageBox.Show("撤销配对成功！");
                btn_dgselect_Click(sender, e);
            }
            else
                MessageBox.Show("撤销配对失败！");
        }

        private void dgv_dgys_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            dgv_dgys.EndEdit();
            txtDGYSBM.Enabled = true;
            txtYSBM.Text = this.dgv_dgys.CurrentRow.Cells["col_ysbm"].Value.ToString();
            txtYSXM.Text = this.dgv_dgys.CurrentRow.Cells["col_ysxm"].Value.ToString();
            txtDGYSBM.Text = this.dgv_dgys.CurrentRow.Cells["col_ybdgysbm"].Value.ToString();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            string strSql = string.Format(@"select count(b1empn) as hisrc,SUM(case when isnull(dgysbm,'')='' then 1 else 0 end) as gjrc,SUM(case when isnull(dgysbm,'')!='' then 1 else 0 end) as gjrc1 from bz01h a left join ybdgyszd b on a.b1empn=b.ysbm ");
            DataSet ds = CliUtils.ExecuteSql("sybdj", "cmd", strSql.ToString(), CliUtils.fLoginDB, true, CliUtils.fCurrentProject);
            label1.Text = "HIS人员数:" + ds.Tables[0].Rows[0]["hisrc"].ToString() + "; 已对照人数:" + ds.Tables[0].Rows[0]["gjrc1"].ToString() + "; 未对照人数:" + ds.Tables[0].Rows[0]["gjrc"].ToString();
        }


    }
}
