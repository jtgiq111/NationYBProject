using Srvtools;
using System;
using System.Data;
using System.Windows.Forms;

namespace ybinterface_lib
{
    public partial class Frm_ybfyscxgdrjj : InfoForm
    {
        public Frm_ybfyscxgdrjj(string lsh, string xm)
        {
            InitializeComponent();
            lbl_lsh.Text = lsh;
            lbl_xm.Text =  xm;
        }

        public Frm_ybfyscxgdrjj()
        {
            InitializeComponent(); 
        }

        private void btn_close_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void txt_ybxm_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter&&txt_ybxm.Text.Trim()!="")
            {
                string sql;

                if (rbtnPYM.Checked)
                {
                    sql = "select dm, dmmc from ybmrdr where pym like '%" + txt_ybxm.Text.Trim() + "%'";
                }
                else
                {
                    sql = "select dm, dmmc from ybmrdr where dmmc like '%" + txt_ybxm.Text.Trim() + "%'";
                }

                sql += " order by dm, len(dmmc)";
                DataTable dt = CliUtils.ExecuteSql("sybdj", "cmd", sql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject).Tables[0];
                cmb_ybxm.DataSource = dt;
                cmb_ybxm.DisplayMember = "dmmc";
                cmb_ybxm.ValueMember = "dm";
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (dgv_fymx.CurrentRow.Index == -1 || dgv_fymx.CurrentRow.Cells[0].Value == null)
            {
                MessageBox.Show("医保提示：请选择一条记录修改", "提示");
                return;
            }

            if (cmb_ybxm.Text.Trim() == "" || cmb_ybxm.SelectedValue.ToString() == "")
            {
                MessageBox.Show("医保提示：请输入医保项目", "提示");
                return;
            }

            string yysfxmbm = dgv_fymx.CurrentRow.Cells["yysfxmbm"].Value.ToString();
            string sql = string.Format("update ybcfmxscindr set sfxmzxbm = '{0}' where jzlsh = '{1}' and yysfxmbm = '{2}' and jsdjh is null", cmb_ybxm.SelectedValue, lbl_lsh.Text, yysfxmbm);
            
            if (chk_rq.Checked)
            {
                sql += string.Format(" and left(cfrq, 8) between '{0}' and '{1}'", dateTimeStar.Value.ToString("yyyyMMdd"), dateTimeEnd.Value.ToString("yyyyMMdd"));
            }

            object[] obj = { sql };
            obj = CliUtils.CallMethod("sybdj", "BatExecuteSql", obj);

            if (obj[1].ToString() == "1")
            {
                MessageBox.Show("医保提示：修改成功", "提示");
                btncx_Click(null, null);
            }
            else
            {
                MessageBox.Show("医保提示：修改失败", "提示");
            }
        }

        private void btncx_Click(object sender, EventArgs e)
        {
            string sql = string.Format(@"select distinct a.yysfxmbm, a.yysfxmmc yyxmmc, b.dm sfxmzxbm, b.dmmc ybxmmc, b.gg, a.je, a.dj, a.sl
            , b.sflb, b.sfxmdj, a.ysxm, a.cfrq from ybcfmxscindr a join (select y1ypno, y1pymx from yp01h union all select b5item, b5pymx from bz05d) c on a.yysfxmbm = c.y1ypno join (select dm, dmmc, gg, sflb, sfxmdj from ybmrdr union all select dm, dmmc, gg, sflb, sfxmdj from ybmrdr20171231) b on a.sfxmzxbm = b.dm 
            where a.jzlsh = '{0}' and a.cxbz = 1 and a.jsdjh is null", lbl_lsh.Text);

            if (txt_byxm.Text.Trim() != "")
            {
                if (rbtn_bypym.Checked)
                {
                    sql += string.Format(" and c.y1pymx like '%{0}%'", txt_byxm.Text.Trim());
                }
                else
                {
                    sql += string.Format(" and a.yysfxmmc like '%{0}%'", txt_byxm.Text.Trim());
                }           
            }

            if (chk_rq.Checked)
            {
                sql += string.Format(" and left(a.cfrq, 8) between '{0}' and '{1}'", dateTimeStar.Value.ToString("yyyyMMdd"), dateTimeEnd.Value.ToString("yyyyMMdd"));
            }

            DataTable dt = CliUtils.ExecuteSql("sybdj", "cmd", sql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject).Tables[0];
            dgv_fymx.DataSource = dt;
            //sql = string.Format("select isnull(sum(a.je), 0) from ybcfmxscindr a where a.jzlsh = '{0}' and a.cxbz = 1 and a.jsdjh is null", lbl_lsh.Text);

            //if (txt_byxm.Text.Trim() != "")
            //{
            //    if (rbtn_bypym.Checked)
            //    {
            //        sql += string.Format(" and b.hispym like '%{0}%'", txt_byxm.Text.Trim());
            //    }
            //    else
            //    {
            //        sql += string.Format(" and b.hisxmmc like '%{0}%'", txt_byxm.Text.Trim());
            //    }
            //}

            //if (chk_rq.Checked)
            //{
            //    sql += string.Format(" and left(a.cfrq, 8) between '{0}' and '{1}'", dateTimeStar.Value.ToString("yyyyMMdd"), dateTimeEnd.Value.ToString("yyyyMMdd"));
            //}

            //dt = CliUtils.ExecuteSql("sybdj", "cmd", sql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject).Tables[0];
            //lblgfxmzj.Text = dt.Rows[0][0].ToString();
        }

        private void Frm_ybfyscxgdrjj_Load(object sender, EventArgs e)
        {
            string sql = string.Format("select distinct a.yysfxmbm, a.yysfxmmc yyxmmc, a.sfxmzxbm, b.dmmc ybxmmc, b.gg, a.je, a.dj, a.sl, b.sflb, b.sfxmdj, a.ysxm, a.cfrq from ybcfmxscindr a join (select dm, dmmc, gg, sflb, sfxmdj from ybmrdr union all select dm, dmmc, gg, sflb, sfxmdj from ybmrdr20171231) b on a.sfxmzxbm = b.dm where a.jzlsh = '{0}' and a.cxbz = 1 and a.jsdjh is null", lbl_lsh.Text);
            DataTable dt = CliUtils.ExecuteSql("sybdj", "cmd", sql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject).Tables[0];
            dgv_fymx.DataSource = dt;
            sql = string.Format("select isnull(sum(a.je), 0) from ybcfmxscindr a where a.jzlsh = '{0}' and a.cxbz = 1 and a.jsdjh is null", lbl_lsh.Text);
            dt = CliUtils.ExecuteSql("sybdj", "cmd", sql, CliUtils.fLoginDB, true, CliUtils.fCurrentProject).Tables[0];
            lblgfxmzj.Text = dt.Rows[0][0].ToString();
            btn_close.Focus();
        }

        private void dgv_fymx_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex != -1 && dgv_fymx.Rows[e.RowIndex].Cells[0].Value != null)
            {
                DataGridViewRow dgvr = dgv_fymx.Rows[e.RowIndex];
                lbl_byxm.Text = dgvr.Cells["yyxmmc"].Value.ToString();
                lbl_ybxm.Text = dgvr.Cells["ybxmmc"].Value.ToString();
            }
            else
            {
                lbl_byxm.Text = "";
                lbl_ybxm.Text = "";
            }
        }

        private void txt_byxm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (string.IsNullOrWhiteSpace(txt_byxm.Text))
                {
                    MessageBox.Show("请输入本院项目", "提示");
                    return;
                }

                btncx_Click(null, null);
            }
        }

        private void chk_rq_Click(object sender, EventArgs e)
        {
            dateTimeStar.Visible = chk_rq.Checked;
            lbl_to.Visible = chk_rq.Checked;
            dateTimeEnd.Visible = chk_rq.Checked;
        }
    }
}
